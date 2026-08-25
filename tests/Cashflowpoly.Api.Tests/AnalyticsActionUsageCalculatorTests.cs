// Fungsi file: Memverifikasi perilaku API, database, atau domain melalui AnalyticsActionUsageCalculatorTests.
using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Domain;
using Xunit;

namespace Cashflowpoly.Api.Tests;

public sealed class AnalyticsActionUsageCalculatorTests
{
    [Fact]
    public void Compute_SummarizesActionSequencesRepetitionsAndEfficiency()
    {
        var playerId = Guid.NewGuid();
        var sessionId = Guid.NewGuid();
        var freelanceEventId = Guid.NewGuid();
        var orderEventId = Guid.NewGuid();
        var events = new List<EventDb>
        {
            CreateEvent(Guid.NewGuid(), sessionId, playerId, "BahanMasakan", dayIndex: 0, actionSlot: 1, sequence: 1),
            CreateEvent(Guid.NewGuid(), sessionId, playerId, "BahanMasakan", dayIndex: 0, actionSlot: 2, sequence: 2),
            CreateEvent(Guid.NewGuid(), sessionId, playerId, "AkhirGiliran", dayIndex: 0, actionSlot: 2, sequence: 3),
            CreateEvent(freelanceEventId, sessionId, playerId, "KerjaLepas", dayIndex: 1, actionSlot: 1, sequence: 4),
            CreateEvent(orderEventId, sessionId, playerId, "JualMasakan", dayIndex: 1, actionSlot: 2, sequence: 5)
        };
        var projections = new List<CashflowProjectionDb>
        {
            CreateProjection(freelanceEventId, sessionId, playerId, "IN", 5, "FREELANCE"),
            CreateProjection(orderEventId, sessionId, playerId, "IN", 12, "ORDER_INCOME")
        };

        var metrics = new ActionUsageCalculator().Compute(events, projections, latestDayIndex: 1, actionsPerTurn: 2);

        Assert.Equal(2, metrics.ActionSequences.Count);
        Assert.Equal(0, metrics.ActionSequences[0].DayIndex);
        Assert.Equal(new[] { "BahanMasakan", "BahanMasakan" }, metrics.ActionSequences[0].Actions);
        Assert.Equal(1, metrics.ActionSequences[1].DayIndex);
        Assert.Equal(new[] { "KerjaLepas", "JualMasakan" }, metrics.ActionSequences[1].Actions);

        Assert.Equal(2, metrics.ActionRepetitions.Count);
        Assert.Equal(0, metrics.ActionRepetitions[0].DayIndex);
        Assert.Equal(2, metrics.ActionRepetitions[0].TotalActions);
        Assert.Equal(1, metrics.ActionRepetitions[0].DistinctActions);
        Assert.Equal(1, metrics.ActionRepetitions[0].RepeatedActions);
        Assert.Equal(0.5, metrics.ActionRepetitions[0].DiversityScore);
        Assert.Equal(1, metrics.ActionRepetitions[1].DiversityScore);

        Assert.Equal(4, metrics.ActionSlotTimeline.Count);
        Assert.Equal(2, metrics.ActionSlotTimeline[3].ActionSlot);
        Assert.Equal("JualMasakan", metrics.ActionSlotTimeline[3].ActionType);
        Assert.Equal(2, metrics.LatestActionSlot);
        Assert.Equal(1, metrics.LatestDayIndex);
        Assert.Equal(4, metrics.ActionEventCount);
        Assert.Equal(2, metrics.IncomeActions);
        Assert.Equal(0.5, metrics.ActionEfficiency);
        Assert.Equal(50, metrics.ActionEfficiencyPercent);
        Assert.Equal(0.75, metrics.ActionDiversityAverage);
    }

    [Fact]
    public void Compute_IgnoresLegacySkippedOrderAndExcludesFreeActions()
    {
        var playerId = Guid.NewGuid();
        var sessionId = Guid.NewGuid();
        var events = new List<EventDb>
        {
            CreateEvent(Guid.NewGuid(), sessionId, playerId, "LewatiOrder", dayIndex: 0, actionSlot: 1, sequence: 1),
            CreateEvent(Guid.NewGuid(), sessionId, playerId, "BuangBahanMasakan", dayIndex: 0, actionSlot: 2, sequence: 2),
            CreateEvent(Guid.NewGuid(), sessionId, playerId, "RisikoKehidupan", dayIndex: 0, actionSlot: 0, sequence: 3)
        };

        var metrics = new ActionUsageCalculator().Compute(events, [], latestDayIndex: 0, actionsPerTurn: 2);

        Assert.Equal(new[] { "BuangBahanMasakan" }, metrics.ActionSequences.Single().Actions);
        Assert.Equal(1, metrics.ActionEventCount);
    }

    private static EventDb CreateEvent(
        Guid eventId,
        Guid sessionId,
        Guid playerId,
        string actionType,
        int dayIndex,
        int actionSlot,
        long sequence)
    {
        return new EventDb
        {
            EventId = eventId,
            SessionId = sessionId,
            UserId = playerId,
            ActorType = "PLAYER",
            Timestamp = new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero),
            DayIndex = dayIndex,
            Weekday = "MON",
            ActionSlot = actionSlot,
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
            UserId = playerId,
            EventPk = Guid.NewGuid(),
            EventId = eventId,
            Timestamp = new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero),
            Direction = direction,
            Amount = amount,
            Category = category
        };
    }
}
