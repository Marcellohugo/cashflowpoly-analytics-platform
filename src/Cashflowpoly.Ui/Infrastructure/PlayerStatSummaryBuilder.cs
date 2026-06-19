using System.Globalization;
using Cashflowpoly.Ui.Contracts;
using Cashflowpoly.Ui.Models;

namespace Cashflowpoly.Ui.Infrastructure;

/// <summary>
/// Builder ringkasan statistik pemain untuk membantu instruktur membaca performa inti.
/// </summary>
public static class PlayerStatSummaryBuilder
{
    public static PlayerStatSummaryViewModel Build(
        GameplayMetricsResponse? gameplay,
        PlayerCashflowJourneyStatsViewModel? cashflowJourney,
        Func<string, string> translate)
    {
        var keyMetrics = new List<PlayerStatKeyMetricViewModel>();
        var insights = new List<PlayerInstructorInsightViewModel>();

        var netCashflow = cashflowJourney?.NetCashflow ?? gameplay?.Economy.CashflowNetTotal ?? 0d;
        var totalCashIn = cashflowJourney?.TotalCashIn ?? gameplay?.Economy.CashInTotal ?? 0d;
        var totalCashOut = cashflowJourney?.TotalCashOut ?? gameplay?.Economy.CashOutTotal ?? 0d;
        var endingCash = cashflowJourney?.EndingCash ??
            ((gameplay?.Economy.StartingCash ?? 0d) + netCashflow);
        var transactionCount = cashflowJourney?.TransactionCount ?? 0;
        var happiness = gameplay?.Score.HappinessPointsTotal ?? 0d;
        var primaryNeedRate = gameplay?.Compliance.PrimaryNeedRate ?? 0d;
        var goldQty = gameplay?.Progress.GoldQty ?? 0;
        var donationTotal = gameplay?.Economy.DonationTotal ?? 0d;
        var hasUnpaidLoan = gameplay?.Score.HasUnpaidLoan ?? false;

        keyMetrics.Add(Metric("ending_cash", translate("players.cashflow_journey.ending_cash"), FormatNumber(endingCash), ToneForSigned(endingCash)));
        keyMetrics.Add(Metric("net_cashflow", translate("players.cashflow_journey.net"), FormatNumber(netCashflow), ToneForSigned(netCashflow)));
        keyMetrics.Add(Metric("happiness_points", translate("metric.happiness"), FormatNumber(happiness), happiness >= 60 ? "positive" : "neutral"));
        keyMetrics.Add(Metric("primary_need_compliance", translate("metric.primary_need_compliance"), FormatPercent(primaryNeedRate), primaryNeedRate < 0.75 ? "warning" : "positive"));
        keyMetrics.Add(Metric("cash_in_total", translate("metric.cash_in"), FormatNumber(totalCashIn), "positive"));
        keyMetrics.Add(Metric("cash_out_total", translate("metric.cash_out"), FormatNumber(totalCashOut), totalCashOut > totalCashIn ? "warning" : "neutral"));
        keyMetrics.Add(Metric("gold_qty", translate("metric.gold_qty"), goldQty.ToString("N0", CultureInfo.CurrentCulture), goldQty > 0 ? "positive" : "neutral"));
        keyMetrics.Add(Metric("donation_total", translate("metric.donation"), FormatNumber(donationTotal), donationTotal > 0 ? "positive" : "neutral"));
        keyMetrics.Add(Metric("transaction_count", translate("players.cashflow_journey.total_transactions"), transactionCount.ToString("N0", CultureInfo.CurrentCulture), "neutral"));
        keyMetrics.Add(Metric("loan_status", translate("metric.loan_unpaid"), hasUnpaidLoan ? translate("players.stats.unpaid") : translate("players.stats.clear"), hasUnpaidLoan ? "danger" : "positive"));

        if (netCashflow < 0)
        {
            insights.Add(Insight(
                "cashflow_negative",
                translate("players.stats.insight.cashflow_negative.title"),
                translate("players.stats.insight.cashflow_negative.desc"),
                "warning"));
        }
        else if (netCashflow > 0)
        {
            insights.Add(Insight(
                "cashflow_positive",
                translate("players.stats.insight.cashflow_positive.title"),
                translate("players.stats.insight.cashflow_positive.desc"),
                "positive"));
        }

        if (hasUnpaidLoan)
        {
            insights.Add(Insight(
                "loan_unpaid",
                translate("players.stats.insight.loan_unpaid.title"),
                translate("players.stats.insight.loan_unpaid.desc"),
                "danger"));
        }

        if (happiness >= 60)
        {
            insights.Add(Insight(
                "happiness_strong",
                translate("players.stats.insight.happiness_strong.title"),
                translate("players.stats.insight.happiness_strong.desc"),
                "positive"));
        }
        else if (happiness < 20)
        {
            insights.Add(Insight(
                "happiness_low",
                translate("players.stats.insight.happiness_low.title"),
                translate("players.stats.insight.happiness_low.desc"),
                "warning"));
        }

        if (primaryNeedRate < 0.75)
        {
            insights.Add(Insight(
                "primary_need_low",
                translate("players.stats.insight.primary_need_low.title"),
                translate("players.stats.insight.primary_need_low.desc"),
                "warning"));
        }

        if (insights.Count == 0)
        {
            insights.Add(Insight(
                "stable_profile",
                translate("players.stats.insight.stable_profile.title"),
                translate("players.stats.insight.stable_profile.desc"),
                "neutral"));
        }

        return new PlayerStatSummaryViewModel
        {
            KeyMetrics = keyMetrics,
            Insights = insights.Take(5).ToList()
        };
    }

    private static PlayerStatKeyMetricViewModel Metric(string key, string label, string value, string tone)
    {
        return new PlayerStatKeyMetricViewModel
        {
            Key = key,
            Label = label,
            Value = value,
            Tone = tone
        };
    }

    private static PlayerInstructorInsightViewModel Insight(string key, string title, string description, string tone)
    {
        return new PlayerInstructorInsightViewModel
        {
            Key = key,
            Title = title,
            Description = description,
            Tone = tone
        };
    }

    private static string FormatNumber(double value)
    {
        return value.ToString("N0", CultureInfo.CurrentCulture);
    }

    private static string FormatPercent(double value)
    {
        return value.ToString("P0", CultureInfo.CurrentCulture);
    }

    private static string ToneForSigned(double value)
    {
        return value < 0 ? "warning" : value > 0 ? "positive" : "neutral";
    }
}
