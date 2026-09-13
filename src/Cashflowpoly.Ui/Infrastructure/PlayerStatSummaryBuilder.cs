// Fungsi file: Menyediakan transformasi, lokalisasi, atau koneksi UI melalui PlayerStatSummaryBuilder.
// Mengimpor namespace `System.Text.Json` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.Json;
// Mengimpor namespace `Cashflowpoly.Ui.Contracts` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Ui.Contracts;
// Mengimpor namespace `Cashflowpoly.Ui.Models` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Ui.Models;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Infrastructure` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Infrastructure;

/// <summary>
/// Builder ringkasan statistik pemain untuk membantu instruktur membaca performa inti.
/// </summary>
// Mendefinisikan tipe class `PlayerStatSummaryBuilder`.
public static class PlayerStatSummaryBuilder
{
    public static PlayerStatSummaryViewModel Build(
        // Parameter `gameplay` bertipe `GameplayMetricsResponse?` membawa nilai gameplay; nilai null diizinkan ketika data opsional belum tersedia.
        GameplayMetricsResponse? gameplay,
        // Parameter `analyticsSummary` bertipe `AnalyticsByPlayerItem?` membawa nilai analytics summary; nilai null diizinkan ketika data opsional belum
        // tersedia.
        AnalyticsByPlayerItem? analyticsSummary,
        // Parameter `cashflowJourney` bertipe `PlayerCashflowJourneyStatsViewModel?` membawa nilai arus kas journey; nilai null diizinkan ketika data
        // opsional belum tersedia.
        // Parameter `translate` bertipe `Func<string, string>` membawa nilai translate.
        Func<string, string> translate)
    {
        var insights = new List<PlayerInstructorInsightViewModel>();

        double? netCashflow = analyticsSummary is not null
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: analyticsSummary.CashInTotal - analyticsSummary.CashOutTotal dalam
            // Build.
            ? analyticsSummary.CashInTotal - analyticsSummary.CashOutTotal
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: gameplay?.Economy.CashflowNetTotal;
            // dalam Build.
            : gameplay?.Economy.CashflowNetTotal;
        double? happiness = analyticsSummary?.HappinessPointsTotal ?? gameplay?.Score.HappinessPointsTotal;
        double? fulfillmentDiversity = analyticsSummary?.FulfillmentDiversity ?? gameplay?.Needs.FulfillmentDiversity;
        var needCardsOwned = ReadNeedCardsOwned(gameplay?.RawJson);
        bool? hasUnpaidLoan = analyticsSummary?.HasUnpaidLoan ?? gameplay?.Score.HasUnpaidLoan;

        if (netCashflow is < 0)
        {
            insights.Add(Insight(
                "cashflow_negative",
                translate("players.stats.insight.cashflow_negative.title"),
                translate("players.stats.insight.cashflow_negative.desc"),
                "warning"));
        }

        if (hasUnpaidLoan == true)
        {
            insights.Add(Insight(
                "loan_unpaid",
                translate("players.stats.insight.loan_unpaid.title"),
                translate("players.stats.insight.loan_unpaid.desc"),
                "danger"));
        }

        if (happiness is < 20)
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
        else if (fulfillmentDiversity is < 0.4)
        {
            insights.Add(Insight(
                "need_diversity_low",
                translate("players.stats.insight.need_diversity_low.title"),
                translate("players.stats.insight.need_diversity_low.desc"),
                "warning"));
        }

        var hasCompleteEvaluationData =
            netCashflow.HasValue &&
            happiness.HasValue &&
            fulfillmentDiversity.HasValue &&
            hasUnpaidLoan.HasValue;

        if (insights.Count == 0 && !hasCompleteEvaluationData)
        {
            insights.Add(Insight(
                "data_unavailable",
                translate("players.stats.insight.data_unavailable.title"),
                translate("players.stats.insight.data_unavailable.desc"),
                "neutral"));
        }
        else if (insights.Count == 0)
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
