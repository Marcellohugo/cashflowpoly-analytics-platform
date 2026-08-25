// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui AnalyticsActionUsageCalculator.
using System.Text.Json.Serialization;
using Cashflowpoly.Api.Data;
using static Cashflowpoly.Api.Domain.AnalyticsMath;

namespace Cashflowpoly.Api.Domain;

public sealed record AnalyticsActionUsageMetrics(
    IReadOnlyList<AnalyticsActionSequence> ActionSequences,
    IReadOnlyList<AnalyticsActionRepetition> ActionRepetitions,
    IReadOnlyList<AnalyticsActionSlot> ActionSlotTimeline,
    int? LatestDayIndex,
    int? LatestActionSlot,
    int ActionEventCount,
    int IncomeActions,
    double? ActionEfficiency,
    double? ActionEfficiencyPercent,
    double ActionDiversityAverage);

public sealed record AnalyticsActionSequence(
    [property: JsonPropertyName("day_index")] int DayIndex,
    [property: JsonPropertyName("actions")] IReadOnlyList<string> Actions);

public sealed record AnalyticsActionRepetition(
    [property: JsonPropertyName("day_index")] int DayIndex,
    [property: JsonPropertyName("total_actions")] int TotalActions,
    [property: JsonPropertyName("distinct_actions")] int DistinctActions,
    [property: JsonPropertyName("repeated_actions")] int RepeatedActions,
    [property: JsonPropertyName("diversity_score")] double DiversityScore);

public sealed record AnalyticsActionSlot(
    [property: JsonPropertyName("day_index")] int DayIndex,
    [property: JsonPropertyName("action_slot")] int ActionSlot,
    [property: JsonPropertyName("action_index")] int ActionIndex,
    [property: JsonPropertyName("action_type")] string ActionType,
    [property: JsonIgnore] long SequenceNumber);

internal sealed class ActionUsageCalculator : IActionUsageCalculator
{
    private static readonly EventPayloadReader _payloadReader = new();

    public AnalyticsActionUsageMetrics Compute(
        IReadOnlyCollection<EventDb> playerEvents,
        IReadOnlyCollection<CashflowProjectionDb> playerProjections,
        int latestDayIndex,
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
        var incomeActions = actionEvents.Count(e => incomeActionEventIds.Contains(e.EventId));
        var actionEfficiency = SafeRatio(incomeActions, actionEvents.Count);
        var actionDiversityAverage = actionRepetitions.Count > 0
            ? actionRepetitions.Average(item => item.DiversityScore)
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
