// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui AnalyticsIncomeDiversificationCalculator.
using Cashflowpoly.Api.Data;

namespace Cashflowpoly.Api.Domain;

public sealed record AnalyticsIncomeDiversificationMetrics(
    double FreelanceIncome,
    double MealIncome,
    double GoldIncome,
    int ActiveIncomeSourceCount,
    IReadOnlyDictionary<string, double> IncomeShares,
    double? IncomeDiversificationIndex,
    bool RequiresIncomeNote);

internal sealed class IncomeDiversificationCalculator : IIncomeDiversificationCalculator
{
    private static readonly AnalyticsPayloadReader _payloadReader = new();

    public AnalyticsIncomeDiversificationMetrics Compute(
        IReadOnlyCollection<EventDb> playerEvents,
        int mealOrderIncomeTotal,
        int goldInvestmentEarned)
    {
        var freelanceIncome = playerEvents
            .Where(e => e.ActionType == "KerjaLepas")
            .Select(e => _payloadReader.TryReadAmount(e.Payload, out var amount) ? amount : 0)
            .Sum();
        var mealIncome = (double)mealOrderIncomeTotal;
        var goldIncome = (double)goldInvestmentEarned;

        var incomeSourceTotals = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
        {
            ["freelance_income"] = freelanceIncome,
            ["meal_order_income"] = mealIncome,
            ["gold_sale_income"] = goldIncome
        };

        var incomeShares = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
        var diversifiableIncome = incomeSourceTotals.Values.Sum();
        if (diversifiableIncome > 0)
        {
            foreach (var (source, value) in incomeSourceTotals)
            {
                if (value <= 0)
                {
                    continue;
                }

                incomeShares[source] = value / diversifiableIncome;
            }
        }

        var activeIncomeSourceCount = incomeShares.Count;
        double? incomeDiversificationIndex = null;
        if (diversifiableIncome > 0)
        {
            if (activeIncomeSourceCount <= 1)
            {
                incomeDiversificationIndex = 0;
            }
            else
            {
                var concentration = incomeShares.Values.Sum(share => share * share);
                var normalizationDenominator = 1 - (1d / activeIncomeSourceCount);
                incomeDiversificationIndex = normalizationDenominator <= 0
                    ? 0
                    : ((1 - concentration) / normalizationDenominator) * 100;
            }
        }
        return new AnalyticsIncomeDiversificationMetrics(
            freelanceIncome,
            mealIncome,
            goldIncome,
            activeIncomeSourceCount,
            incomeShares,
            incomeDiversificationIndex,
            diversifiableIncome <= 0);
    }
}
