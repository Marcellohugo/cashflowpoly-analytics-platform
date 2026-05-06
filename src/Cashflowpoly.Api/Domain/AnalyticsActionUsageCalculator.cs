// Fungsi file: Menghitung metrik penggunaan aksi, repetisi, slot aksi, dan efisiensi aksi gameplay.
using System.Text.Json.Serialization;
using Cashflowpoly.Api.Data;
using static Cashflowpoly.Api.Domain.AnalyticsMath;
using static Cashflowpoly.Api.Domain.AnalyticsPayloadReader;

namespace Cashflowpoly.Api.Domain;

internal sealed record AnalyticsActionUsageMetrics(
    IReadOnlyList<AnalyticsActionSequence> ActionSequences,
    IReadOnlyList<AnalyticsActionRepetition> ActionRepetitions,
    IReadOnlyList<AnalyticsActionSlot> ActionSlotTimeline,
    int ActionsSkipped,
    int? LatestActionSlot,
    int ActionEventCount,
    int IncomeActions,
    double? ActionEfficiency,
    double? ActionEfficiencyPercent,
    double ActionDiversityAverage);

internal sealed record AnalyticsActionSequence(
    [property: JsonPropertyName("turn_number")] int TurnNumber,
    [property: JsonPropertyName("actions")] IReadOnlyList<string> Actions);

internal sealed record AnalyticsActionRepetition(
    [property: JsonPropertyName("turn_number")] int TurnNumber,
    [property: JsonPropertyName("total_actions")] int TotalActions,
    [property: JsonPropertyName("distinct_actions")] int DistinctActions,
    [property: JsonPropertyName("repeated_actions")] int RepeatedActions,
    [property: JsonPropertyName("diversity_score")] double DiversityScore);

internal sealed record AnalyticsActionSlot(
    [property: JsonPropertyName("turn_number")] int TurnNumber,
    [property: JsonPropertyName("action_slot")] int ActionSlot,
    [property: JsonPropertyName("action_type")] string ActionType,
    [property: JsonIgnore] long SequenceNumber);

internal static class AnalyticsActionUsageCalculator
{
    internal static AnalyticsActionUsageMetrics Compute(
        IReadOnlyCollection<EventDb> playerEvents,
        IReadOnlyCollection<CashflowProjectionDb> playerProjections,
        int maxTurnNumber,
        int actionsPerTurn)
    {
        var actionEvents = playerEvents
            .Where(e => IsActionEvent(e.ActionType))
            .OrderBy(e => e.SequenceNumber)
            .ToList();

        var actionSequences = actionEvents
            .GroupBy(e => e.TurnNumber)
            .OrderBy(g => g.Key)
            .Select(g => new AnalyticsActionSequence(
                g.Key,
                g.Select(e => e.ActionType).ToList()))
            .ToList();

        var actionRepetitions = actionEvents
            .GroupBy(e => e.TurnNumber)
            .OrderBy(g => g.Key)
            .Select(g =>
            {
                var distinctActions = g.Select(e => e.ActionType).Distinct(StringComparer.OrdinalIgnoreCase).Count();
                var totalActions = g.Count();
                var repeatedActions = Math.Max(0, totalActions - distinctActions);
                var diversityScore = actionsPerTurn > 0 ? (double)distinctActions / actionsPerTurn : 0;
                return new AnalyticsActionRepetition(
                    g.Key,
                    totalActions,
                    distinctActions,
                    repeatedActions,
                    diversityScore);
            })
            .ToList();

        var actionTurns = actionRepetitions.Select(item => item.TurnNumber).ToHashSet();
        var actionsSkipped = maxTurnNumber > 0 ? Math.Max(0, maxTurnNumber - actionTurns.Count) : 0;
        var actionSlotTimeline = actionEvents
            .GroupBy(e => e.TurnNumber)
            .OrderBy(g => g.Key)
            .SelectMany(g => g
                .OrderBy(e => e.SequenceNumber)
                .Select((e, index) => new AnalyticsActionSlot(
                    g.Key,
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
            actionsSkipped,
            latestActionSlot,
            actionEvents.Count,
            incomeActions,
            actionEfficiency,
            actionEfficiency.HasValue ? actionEfficiency.Value * 100 : null,
            actionDiversityAverage);
    }
}
