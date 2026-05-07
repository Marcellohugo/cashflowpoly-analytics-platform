// Fungsi file: Menguji kalkulasi penggunaan aksi gameplay dari event dan proyeksi pemain.
using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Domain;
using Xunit;

namespace Cashflowpoly.Api.Tests;

public sealed class AnalyticsActionUsageCalculatorTests
{
    [Fact]
    public void Compute_SummarizesActionSequencesRepetitionsSlotsAndEfficiency()
    {
        var playerId = Guid.NewGuid();
        var sessionId = Guid.NewGuid();
        var freelanceEventId = Guid.NewGuid();
        var orderEventId = Guid.NewGuid();
        var events = new List<EventDb>
        {
            CreateEvent(freelanceEventId, sessionId, playerId, "work.freelance.completed", turn: 1, sequence: 1),
            CreateEvent(Guid.NewGuid(), sessionId, playerId, "ingredient.purchased", turn: 1, sequence: 2),
            CreateEvent(Guid.NewGuid(), sessionId, playerId, "ingredient.purchased", turn: 1, sequence: 3),
            CreateEvent(Guid.NewGuid(), sessionId, playerId, "turn.action.used", turn: 1, sequence: 4),
            CreateEvent(Guid.NewGuid(), sessionId, playerId, "order.passed", turn: 2, sequence: 5),
            CreateEvent(orderEventId, sessionId, playerId, "order.claimed", turn: 3, sequence: 6)
        };
        var projections = new List<CashflowProjectionDb>
        {
            CreateProjection(freelanceEventId, sessionId, playerId, "IN", 5, "FREELANCE"),
            CreateProjection(orderEventId, sessionId, playerId, "IN", 12, "ORDER_INCOME")
        };

        var metrics = new ActionUsageCalculator().Compute(events, projections, maxTurnNumber: 4, actionsPerTurn: 2);

        Assert.Equal(2, metrics.ActionSequences.Count);
        Assert.Equal(1, metrics.ActionSequences[0].TurnNumber);
        Assert.Equal(new[] { "work.freelance.completed", "ingredient.purchased", "ingredient.purchased" }, metrics.ActionSequences[0].Actions);
        Assert.Equal(3, metrics.ActionSequences[1].TurnNumber);
        Assert.Equal(new[] { "order.claimed" }, metrics.ActionSequences[1].Actions);

        Assert.Equal(2, metrics.ActionRepetitions.Count);
        Assert.Equal(1, metrics.ActionRepetitions[0].TurnNumber);
        Assert.Equal(3, metrics.ActionRepetitions[0].TotalActions);
        Assert.Equal(2, metrics.ActionRepetitions[0].DistinctActions);
        Assert.Equal(1, metrics.ActionRepetitions[0].RepeatedActions);
        Assert.Equal(1, metrics.ActionRepetitions[0].DiversityScore);
        Assert.Equal(0.5, metrics.ActionRepetitions[1].DiversityScore);

        Assert.Equal(4, metrics.ActionSlotTimeline.Count);
        Assert.Equal(3, metrics.ActionSlotTimeline[2].ActionSlot);
        Assert.Equal("ingredient.purchased", metrics.ActionSlotTimeline[2].ActionType);
        Assert.Equal(1, metrics.LatestActionSlot);
        Assert.Equal(2, metrics.ActionsSkipped);
        Assert.Equal(4, metrics.ActionEventCount);
        Assert.Equal(2, metrics.IncomeActions);
        Assert.Equal(0.5, metrics.ActionEfficiency);
        Assert.Equal(50, metrics.ActionEfficiencyPercent);
        Assert.Equal(0.75, metrics.ActionDiversityAverage);
    }

    private static EventDb CreateEvent(
        Guid eventId,
        Guid sessionId,
        Guid playerId,
        string actionType,
        int turn,
        long sequence)
    {
        return new EventDb
        {
            EventId = eventId,
            SessionId = sessionId,
            PlayerId = playerId,
            ActorType = "PLAYER",
            Timestamp = new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero),
            DayIndex = turn - 1,
            Weekday = "MON",
            TurnNumber = turn,
            SequenceNumber = sequence,
            ActionType = actionType,
            RulesetVersionId = Guid.NewGuid(),
            Payload = "{}"
        };
    }

    private static CashflowProjectionDb CreateProjection(
        Guid eventId,
        Guid sessionId,
        Guid playerId,
        string direction,
        int amount,
        string category)
    {
        return new CashflowProjectionDb
        {
            ProjectionId = Guid.NewGuid(),
            SessionId = sessionId,
            PlayerId = playerId,
            EventPk = Guid.NewGuid(),
            EventId = eventId,
            Timestamp = new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero),
            Direction = direction,
            Amount = amount,
            Category = category
        };
    }
}
