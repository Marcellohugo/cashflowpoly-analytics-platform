// Fungsi file: Menguji kalkulasi timeline kas gameplay dari proyeksi cashflow per event.
using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Domain;
using Xunit;

namespace Cashflowpoly.Api.Tests;

public sealed class AnalyticsCashTimelineCalculatorTests
{
    [Fact]
    public void Compute_GroupsCashflowByEventTurnAndBuildsCoinProgression()
    {
        var sessionId = Guid.NewGuid();
        var playerId = Guid.NewGuid();
        var firstEventId = Guid.NewGuid();
        var secondEventId = Guid.NewGuid();
        var events = new List<EventDb>
        {
            CreateEvent(firstEventId, sessionId, playerId, turnNumber: 1),
            CreateEvent(secondEventId, sessionId, playerId, turnNumber: 2)
        };
        var projections = new List<CashflowProjectionDb>
        {
            CreateProjection(firstEventId, sessionId, playerId, "IN", 10),
            CreateProjection(firstEventId, sessionId, playerId, "OUT", 4),
            CreateProjection(secondEventId, sessionId, playerId, "OUT", 3),
            CreateProjection(Guid.NewGuid(), sessionId, playerId, "OUT", 99)
        };

        var timeline = new CashTimelineCalculator().Compute(events, projections, startingCoins: 20);

        Assert.Equal(20, timeline.StartingCoins);
        Assert.Equal(10, timeline.CashInTotal);
        Assert.Equal(106, timeline.CashOutTotal);
        Assert.Equal(-76, timeline.CoinsNetEndGame);
        Assert.Equal(-76, timeline.CoinsHeldCurrent);

        Assert.Collection(timeline.CoinsSpentPerTurn,
            item =>
            {
                Assert.Equal(1, item.TurnNumber);
                Assert.Equal(4, item.Amount);
            },
            item =>
            {
                Assert.Equal(2, item.TurnNumber);
                Assert.Equal(3, item.Amount);
            });

        Assert.Collection(timeline.CoinsEarnedPerTurn,
            item =>
            {
                Assert.Equal(1, item.TurnNumber);
                Assert.Equal(10, item.Amount);
            });

        Assert.Collection(timeline.NetIncomePerTurn,
            item =>
            {
                Assert.Equal(1, item.TurnNumber);
                Assert.Equal(6, item.Net);
            },
            item =>
            {
                Assert.Equal(2, item.TurnNumber);
                Assert.Equal(-3, item.Net);
            });

        Assert.Collection(timeline.CoinsProgression,
            item =>
            {
                Assert.Equal(1, item.TurnNumber);
                Assert.Equal(26, item.Coins);
            },
            item =>
            {
                Assert.Equal(2, item.TurnNumber);
                Assert.Equal(23, item.Coins);
            });
    }

    private static EventDb CreateEvent(Guid eventId, Guid sessionId, Guid playerId, int turnNumber)
    {
        return new EventDb
        {
            EventId = eventId,
            SessionId = sessionId,
            PlayerId = playerId,
            ActorType = "PLAYER",
            Timestamp = new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero),
            DayIndex = 0,
            Weekday = "MON",
            TurnNumber = turnNumber,
            SequenceNumber = turnNumber,
            ActionType = "transaction.recorded",
            RulesetVersionId = Guid.NewGuid(),
            Payload = "{}"
        };
    }

    private static CashflowProjectionDb CreateProjection(
        Guid eventId,
        Guid sessionId,
        Guid playerId,
        string direction,
        int amount)
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
            Category = "TEST"
        };
    }
}
