using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Domain;
using Xunit;

namespace Cashflowpoly.Api.Tests;

public sealed class AnalyticsNeedMissionCalculatorTests
{
    [Fact]
    public void Compute_SummarizesNeedProfileMissionAndNeedSpending()
    {
        var playerId = Guid.NewGuid();
        var sessionId = Guid.NewGuid();
        var events = new List<EventDb>
        {
            CreateEvent(playerId, sessionId, "need.primary.purchased", """{"card_id":"rice","amount":3,"points":1}"""),
            CreateEvent(playerId, sessionId, "need.secondary.purchased", """{"card_id":"book","amount":4,"points":1}"""),
            CreateEvent(playerId, sessionId, "need.tertiary.purchased", """{"card_id":"bike","amount":7,"points":2}"""),
            CreateEvent(playerId, sessionId, "mission.assigned", """{"mission_id":"mission-1","target_tertiary_card_id":"bike","penalty_points":10,"require_primary":true,"require_secondary":true}""")
        };
        var projections = new List<CashflowProjectionDb>
        {
            CreateProjection(sessionId, playerId, "OUT", 3, "NEED_PRIMARY"),
            CreateProjection(sessionId, playerId, "OUT", 4, "NEED_SECONDARY"),
            CreateProjection(sessionId, playerId, "OUT", 7, "NEED_TERTIARY")
        };

        var metrics = new NeedMissionCalculator().Compute(events, projections);

        Assert.Equal(3, metrics.NeedCardsPurchased);
        Assert.Equal(1, metrics.PrimaryNeeds);
        Assert.Equal(1, metrics.SecondaryNeeds);
        Assert.Equal(1, metrics.TertiaryNeeds);
        Assert.True(metrics.HasBasicNeedProfile);
        Assert.False(metrics.IsCollectorNeedProfile);
        Assert.False(metrics.IsSpecialistNeedProfile);
        Assert.True(metrics.SpecificTertiaryAcquired);
        Assert.True(metrics.CollectionMissionComplete);
        Assert.Equal(14, metrics.NeedCoinsSpent);
        Assert.Equal(1, metrics.FulfillmentDiversity);
        Assert.Equal(1, metrics.MissionAchievement);
    }

    [Fact]
    public void Compute_SpecialistProfileWhenOneNeedDominates()
    {
        var playerId = Guid.NewGuid();
        var sessionId = Guid.NewGuid();
        var events = new List<EventDb>
        {
            CreateEvent(playerId, sessionId, "need.primary.purchased", """{"card_id":"rice-1","amount":1,"points":1}"""),
            CreateEvent(playerId, sessionId, "need.primary.purchased", """{"card_id":"rice-2","amount":1,"points":1}"""),
            CreateEvent(playerId, sessionId, "need.primary.purchased", """{"card_id":"rice-3","amount":1,"points":1}"""),
            CreateEvent(playerId, sessionId, "need.secondary.purchased", """{"card_id":"book","amount":1,"points":1}""")
        };

        var metrics = new NeedMissionCalculator().Compute(events, Array.Empty<CashflowProjectionDb>());

        Assert.True(metrics.IsCollectorNeedProfile);
        Assert.True(metrics.IsSpecialistNeedProfile);
        Assert.Null(metrics.SpecificTertiaryAcquired);
        Assert.Null(metrics.CollectionMissionComplete);
        Assert.Null(metrics.MissionAchievement);
    }

    private static EventDb CreateEvent(Guid playerId, Guid sessionId, string actionType, string payload)
    {
        return new EventDb
        {
            EventId = Guid.NewGuid(),
            SessionId = sessionId,
            PlayerId = playerId,
            ActorType = "PLAYER",
            Timestamp = new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero),
            DayIndex = 0,
            Weekday = "MON",
            TurnNumber = 1,
            SequenceNumber = 1,
            ActionType = actionType,
            RulesetVersionId = Guid.NewGuid(),
            Payload = payload
        };
    }

    private static CashflowProjectionDb CreateProjection(
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
            EventId = Guid.NewGuid(),
            Timestamp = new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero),
            Direction = direction,
            Amount = amount,
            Category = category
        };
    }
}
