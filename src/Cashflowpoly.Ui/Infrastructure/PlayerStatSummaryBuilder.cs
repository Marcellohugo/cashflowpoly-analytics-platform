// Fungsi file: Menyediakan transformasi, lokalisasi, atau koneksi UI melalui PlayerStatSummaryBuilder.
using System.Text.Json;
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
        AnalyticsByPlayerItem? analyticsSummary,
        PlayerCashflowJourneyStatsViewModel? cashflowJourney,
        Func<string, string> translate)
    {
        var insights = new List<PlayerInstructorInsightViewModel>();

        var netCashflow = analyticsSummary is not null
            ? analyticsSummary.CashInTotal - analyticsSummary.CashOutTotal
            : cashflowJourney?.NetCashflow ?? gameplay?.Economy.CashflowNetTotal ?? 0d;
        var happiness = analyticsSummary?.HappinessPointsTotal ?? gameplay?.Score.HappinessPointsTotal ?? 0d;
        var fulfillmentDiversity = analyticsSummary?.FulfillmentDiversity ?? gameplay?.Needs.FulfillmentDiversity ?? 0d;
        var needCardsOwned = ReadNeedCardsOwned(gameplay?.RawJson);
        var hasUnpaidLoan = analyticsSummary?.HasUnpaidLoan ?? gameplay?.Score.HasUnpaidLoan ?? false;

        if (netCashflow < 0)
        {
            insights.Add(Insight(
                "cashflow_negative",
                translate("players.stats.insight.cashflow_negative.title"),
                translate("players.stats.insight.cashflow_negative.desc"),
                "warning"));
        }

        if (hasUnpaidLoan)
        {
            insights.Add(Insight(
                "loan_unpaid",
                translate("players.stats.insight.loan_unpaid.title"),
                translate("players.stats.insight.loan_unpaid.desc"),
                "danger"));
        }

        if (happiness < 20)
        {
            insights.Add(Insight(
                "happiness_low",
                translate("players.stats.insight.happiness_low.title"),
                translate("players.stats.insight.happiness_low.desc"),
                "warning"));
        }

        if (needCardsOwned == 0)
        {
            insights.Add(Insight(
                "need_cards_missing",
                translate("players.stats.insight.need_cards_missing.title"),
                translate("players.stats.insight.need_cards_missing.desc"),
                "warning"));
        }
        else if (fulfillmentDiversity < 0.4)
        {
            insights.Add(Insight(
                "need_diversity_low",
                translate("players.stats.insight.need_diversity_low.title"),
                translate("players.stats.insight.need_diversity_low.desc"),
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
            CollectionMissionComplete = ReadCollectionMissionComplete(gameplay?.RawJson),
            Insights = insights
                .OrderBy(item => item.Tone == "danger" ? 0 : item.Tone == "warning" ? 1 : item.Tone == "neutral" ? 2 : 3)
                .Take(3)
                .ToList()
        };
    }

    private static bool? ReadCollectionMissionComplete(JsonElement? rawJson)
    {
        if (rawJson is not { ValueKind: JsonValueKind.Object } raw ||
            !raw.TryGetProperty("needs", out var needs) ||
            needs.ValueKind != JsonValueKind.Object ||
            !needs.TryGetProperty("collection_mission_complete", out var complete) ||
            complete.ValueKind is not (JsonValueKind.True or JsonValueKind.False))
        {
            return null;
        }

        return complete.GetBoolean();
    }

    private static int? ReadNeedCardsOwned(JsonElement? rawJson)
    {
        if (!rawJson.HasValue || rawJson.Value.ValueKind != JsonValueKind.Object ||
            !rawJson.Value.TryGetProperty("needs", out var needs) ||
            needs.ValueKind != JsonValueKind.Object ||
            !needs.TryGetProperty("need_cards_owned_current", out var owned) ||
            !owned.TryGetInt32(out var count))
        {
            return null;
        }

        return count;
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

}
