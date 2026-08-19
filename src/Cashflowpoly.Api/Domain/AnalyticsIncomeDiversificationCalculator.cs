// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui AnalyticsIncomeDiversificationCalculator.
using Cashflowpoly.Api.Data;

namespace Cashflowpoly.Api.Domain;

public sealed record AnalyticsIncomeDiversificationMetrics(
    double FreelanceIncome,
    double MealIncome,
    double GoldIncome,
    double DonationIncome,
    double OtherIncome,
    int ActiveIncomeSourceCount,
    IReadOnlyDictionary<string, double> IncomeShares,
    double? IncomeDiversificationIndex,
    double? IncomeDiversificationRatio,
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
            .Where(e => e.ActionType == "KerjaLepas")
            .Select(e => _payloadReader.TryReadAmount(e.Payload, out var amount) ? amount : 0)
            .Sum();
        var mealIncome = (double)mealOrderIncomeTotal;
        var goldIncome = (double)goldInvestmentEarned;
        var donationIncome = donationsReceived;
        var operatingCashIn = playerProjections
            .Where(p => p.Direction == "IN" && !IsFinancingReceipt(p.Category))
            .Sum(p => (double)p.Amount);
        var otherIncome = Math.Max(0, operatingCashIn - freelanceIncome - mealIncome - goldIncome - donationIncome);

        var incomeSourceTotals = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
        {
            ["freelance_income"] = freelanceIncome,
            ["meal_income"] = mealIncome,
            ["gold_income"] = goldIncome,
            ["donations_received"] = donationIncome,
            ["other_income"] = otherIncome
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
        var documentedIncome = freelanceIncome + mealIncome + goldIncome + donationIncome;
        var incomeDiversificationRatio = totalIncome > 0
            ? documentedIncome / totalIncome * 100
            : (double?)null;

        return new AnalyticsIncomeDiversificationMetrics(
            freelanceIncome,
            mealIncome,
            goldIncome,
            donationIncome,
            otherIncome,
            activeIncomeSourceCount,
            incomeShares,
            incomeDiversificationIndex,
            incomeDiversificationRatio,
            totalIncome <= 0);
    }

    private static bool IsFinancingReceipt(string category)
        => category.Equals("LOAN_TAKEN", StringComparison.OrdinalIgnoreCase) ||
           category.Equals("SAVING_WITHDRAW", StringComparison.OrdinalIgnoreCase) ||
           category.Equals("EMERGENCY_OPTION", StringComparison.OrdinalIgnoreCase) ||
           category.Equals("RISK_LIFE", StringComparison.OrdinalIgnoreCase);
}
