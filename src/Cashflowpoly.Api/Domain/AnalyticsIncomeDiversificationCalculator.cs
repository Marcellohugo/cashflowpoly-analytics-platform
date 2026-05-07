// Fungsi file: Menghitung kontribusi sumber income dan indeks diversifikasi income gameplay.
using Cashflowpoly.Api.Data;

namespace Cashflowpoly.Api.Domain;

public sealed record AnalyticsIncomeDiversificationMetrics(
    double FreelanceIncome,
    double MealIncome,
    double GoldIncome,
    double DonationIncome,
    int ActiveIncomeSourceCount,
    IReadOnlyDictionary<string, double> IncomeShares,
    double? IncomeDiversification,
    bool RequiresIncomeNote);

internal sealed class IncomeDiversificationCalculator : IIncomeDiversificationCalculator
{
    private static readonly AnalyticsPayloadReader _payloadReader = new();

    public AnalyticsIncomeDiversificationMetrics Compute(
        IReadOnlyCollection<EventDb> playerEvents,
        IReadOnlyCollection<CashflowProjectionDb> playerProjections,
        double totalIncome,
        int mealOrderIncomeTotal,
        int goldInvestmentEarned)
    {
        var donationsReceived = playerProjections
            .Where(p => p.Direction == "IN" && p.Category.Contains("DONATION", StringComparison.OrdinalIgnoreCase))
            .Sum(p => (double)p.Amount);
        var freelanceIncome = playerEvents
            .Where(e => e.ActionType == "work.freelance.completed")
            .Select(e => _payloadReader.TryReadAmount(e.Payload, out var amount) ? amount : 0)
            .Sum();
        var mealIncome = (double)mealOrderIncomeTotal;
        var goldIncome = (double)goldInvestmentEarned;
        var donationIncome = donationsReceived;

        var incomeSourceTotals = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
        {
            ["freelance_income"] = freelanceIncome,
            ["meal_income"] = mealIncome,
            ["gold_income"] = goldIncome,
            ["donations_received"] = donationIncome
        };

        var incomeShares = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
        if (totalIncome > 0)
        {
            foreach (var (source, value) in incomeSourceTotals)
            {
                if (value <= 0)
                {
                    continue;
                }

                incomeShares[source] = value / totalIncome;
            }
        }

        var activeIncomeSourceCount = incomeShares.Count;
        double? incomeDiversification = null;
        if (totalIncome > 0)
        {
            if (activeIncomeSourceCount <= 1)
            {
                incomeDiversification = 0;
            }
            else
            {
                var concentration = incomeShares.Values.Sum(share => share * share);
                var normalizationDenominator = 1 - (1d / activeIncomeSourceCount);
                incomeDiversification = normalizationDenominator <= 0
                    ? 0
                    : ((1 - concentration) / normalizationDenominator) * 100;
            }
        }

        return new AnalyticsIncomeDiversificationMetrics(
            freelanceIncome,
            mealIncome,
            goldIncome,
            donationIncome,
            activeIncomeSourceCount,
            incomeShares,
            incomeDiversification,
            totalIncome <= 0);
    }
}
