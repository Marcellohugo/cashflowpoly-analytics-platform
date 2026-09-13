// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui AnalyticsActionUsageCalculator.
// Mengimpor namespace `System.Text.Json.Serialization` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.Json.Serialization;
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;
// Mengimpor namespace `Cashflowpoly.Api.Domain.AnalyticsMath` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using static Cashflowpoly.Api.Domain.AnalyticsMath;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Domain;

public sealed record AnalyticsActionUsageMetrics(
    // Parameter `ActionSequences` bertipe `IReadOnlyList<AnalyticsActionSequence>` membawa nilai aksi sequences.
    IReadOnlyList<AnalyticsActionSequence> ActionSequences,
    // Parameter `ActionRepetitions` bertipe `IReadOnlyList<AnalyticsActionRepetition>` membawa nilai aksi repetitions.
    IReadOnlyList<AnalyticsActionRepetition> ActionRepetitions,
    // Parameter `ActionSlotTimeline` bertipe `IReadOnlyList<AnalyticsActionSlot>` membawa nilai aksi slot timeline.
    IReadOnlyList<AnalyticsActionSlot> ActionSlotTimeline,
    // Parameter `LatestDayIndex` bertipe `int?` membawa nilai latest hari index; nilai null diizinkan ketika data opsional belum tersedia.
    int? LatestDayIndex,
    // Parameter `LatestActionSlot` bertipe `int?` membawa nilai latest aksi slot; nilai null diizinkan ketika data opsional belum tersedia.
    int? LatestActionSlot,
    // Parameter `ActionEventCount` bertipe `int` membawa nilai aksi event jumlah.
    int ActionEventCount,
    // Parameter `IncomeActions` bertipe `int` membawa nilai pemasukan aksi.
    int IncomeActions,
    // Parameter `ActionEfficiency` bertipe `double?` membawa nilai aksi efficiency; nilai null diizinkan ketika data opsional belum tersedia.
    double? ActionEfficiency,
    // Parameter `ActionEfficiencyPercent` bertipe `double?` membawa nilai aksi efficiency percent; nilai null diizinkan ketika data opsional belum
    // tersedia.
    double? ActionEfficiencyPercent,
    // Parameter `ActionDiversityAverage` bertipe `double` membawa nilai aksi keberagaman rata-rata.
    double ActionDiversityAverage);

public sealed record AnalyticsActionSequence(
    // Parameter `DayIndex` bertipe `int` membawa nilai hari index; memetakan nama properti JSON menjadi (”day_index”).
    [property: JsonPropertyName("day_index")] int DayIndex,
    // Parameter `Actions` bertipe `IReadOnlyList<string>` membawa nilai aksi; memetakan nama properti JSON menjadi (”actions”).
    [property: JsonPropertyName("actions")] IReadOnlyList<string> Actions);

public sealed record AnalyticsActionRepetition(
    // Parameter `DayIndex` bertipe `int` membawa nilai hari index; memetakan nama properti JSON menjadi (”day_index”).
    [property: JsonPropertyName("day_index")] int DayIndex,
    // Parameter `TotalActions` bertipe `int` membawa nilai total aksi; memetakan nama properti JSON menjadi (”total_actions”).
    [property: JsonPropertyName("total_actions")] int TotalActions,
    // Parameter `DistinctActions` bertipe `int` membawa nilai distinct aksi; memetakan nama properti JSON menjadi (”distinct_actions”).
    [property: JsonPropertyName("distinct_actions")] int DistinctActions,
    // Parameter `RepeatedActions` bertipe `int` membawa nilai repeated aksi; memetakan nama properti JSON menjadi (”repeated_actions”).
    [property: JsonPropertyName("repeated_actions")] int RepeatedActions,
    // Parameter `DiversityScore` bertipe `double` membawa nilai keberagaman skor; memetakan nama properti JSON menjadi (”diversity_score”).
    [property: JsonPropertyName("diversity_score")] double DiversityScore);

public sealed record AnalyticsActionSlot(
    // Parameter `DayIndex` bertipe `int` membawa nilai hari index; memetakan nama properti JSON menjadi (”day_index”).
    [property: JsonPropertyName("day_index")] int DayIndex,
    // Parameter `ActionSlot` bertipe `int` membawa nilai aksi slot; memetakan nama properti JSON menjadi (”action_slot”).
    [property: JsonPropertyName("action_slot")] int ActionSlot,
    // Parameter `ActionIndex` bertipe `int` membawa nilai aksi index; memetakan nama properti JSON menjadi (”action_index”).
    [property: JsonPropertyName("action_index")] int ActionIndex,
    // Parameter `ActionType` bertipe `string` membawa nilai aksi jenis; memetakan nama properti JSON menjadi (”action_type”).
    [property: JsonPropertyName("action_type")] string ActionType,
    // Parameter `SequenceNumber` bertipe `long` membawa nomor urut event yang menentukan urutan pemrosesan riwayat permainan; mengatur pengabaian
    // properti JSON sesuai saat serialisasi/deserialisasi.
    [property: JsonIgnore] long SequenceNumber);

internal sealed class ActionUsageCalculator : IActionUsageCalculator
{
    private static readonly EventPayloadReader _payloadReader = new();

    public AnalyticsActionUsageMetrics Compute(
        // Parameter `playerEvents` bertipe `IReadOnlyCollection<EventDb>` membawa nilai pemain event.
        IReadOnlyCollection<EventDb> playerEvents,
        // Parameter `playerProjections` bertipe `IReadOnlyCollection<CashflowProjectionDb>` membawa nilai pemain projections.
        IReadOnlyCollection<CashflowProjectionDb> playerProjections,
        // Parameter `latestDayIndex` bertipe `int` membawa nilai latest hari index.
        int latestDayIndex,
        // Parameter `actionsPerTurn` bertipe `int` membawa nilai aksi per giliran.
        int actionsPerTurn)
    {
        var actionEvents = playerEvents
            .Where(e => string.Equals(e.ActorType, "PLAYER", StringComparison.OrdinalIgnoreCase) &&
                        GameActionCatalog.GetPlayerActionSlotPolicy(
                            e.ActionType,
                            _payloadReader.ReadPayload(string.IsNullOrWhiteSpace(e.Payload) ? "{}" : e.Payload)) == PlayerActionSlotPolicy.Consumes)
            .OrderBy(e => e.SequenceNumber)
            .ToList();

        var actionSequences = actionEvents
            .GroupBy(e => e.DayIndex)
            .OrderBy(g => g.Key)
            .Select(g => new AnalyticsActionSequence(
                g.Key,
                g.OrderBy(e => e.SequenceNumber).Select(e => e.ActionType).ToList()))
            .ToList();

        var actionRepetitions = actionEvents
            .GroupBy(e => e.DayIndex)
            .OrderBy(g => g.Key)
            .Select(g =>
            {
                var distinctActions = g.Select(e => e.ActionType).Distinct(StringComparer.OrdinalIgnoreCase).Count();
                var totalActions = g.Count();
                var repeatedActions = Math.Max(0, totalActions - distinctActions);
                var diversityScore = actionsPerTurn > 0
                    ? Math.Min(1, (double)distinctActions / actionsPerTurn)
                    : 0;
                return new AnalyticsActionRepetition(
                    g.Key,
                    totalActions,
                    distinctActions,
                    repeatedActions,
                    diversityScore);
            })
            .ToList();

        var actionSlotTimeline = actionEvents
            .GroupBy(e => e.DayIndex)
            .OrderBy(g => g.Key)
            .SelectMany(g => g
                .OrderBy(e => e.SequenceNumber)
                .Select((e, index) => new AnalyticsActionSlot(
                    e.DayIndex,
                    e.ActionSlot,
                    index + 1,
                    e.ActionType,
                    e.SequenceNumber)))
            .ToList();
        var latestActionSlot = actionSlotTimeline
            .OrderByDescending(item => item.SequenceNumber)
            .Select(item => (int?)item.ActionSlot)
            .FirstOrDefault();

        var incomeActionEventIds = playerProjections
            .Where(p => p.Direction == "IN")
            .Select(p => p.EventId)
            .ToHashSet();
        var incomeActions = actionEvents.Count(e =>
            e.ActionType is GameActionCatalog.KerjaLepas or GameActionCatalog.JualMasakan &&
            incomeActionEventIds.Contains(e.EventId));
        var actionEfficiency = SafeRatio(incomeActions, actionEvents.Count);
        var actionDiversityAverage = actionRepetitions.Count > 0
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: actionRepetitions.Average(item => item.DiversityScore) dalam Compute.
            ? actionRepetitions.Average(item => item.DiversityScore)
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: 0; dalam Compute.
            : 0;

        return new AnalyticsActionUsageMetrics(
            actionSequences,
            actionRepetitions,
            actionSlotTimeline,
            actionEvents.Count == 0 ? null : actionEvents.Max(e => e.DayIndex),
            latestActionSlot,
            actionEvents.Count,
            incomeActions,
            actionEfficiency,
            actionEfficiency.HasValue ? actionEfficiency.Value * 100 : null,
            actionDiversityAverage);
    }
}
