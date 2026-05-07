// Fungsi file: Menguji builder snapshot gameplay mentah dan turunan dari event/proyeksi pemain.
using System.Text.Json;
using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Domain;
using Xunit;

namespace Cashflowpoly.Api.Tests;

public sealed class AnalyticsGameplaySnapshotBuilderTests
{
    [Fact]
    public void Build_CreatesRawAndDerivedGameplayJson()
    {
        var sessionId = Guid.NewGuid();
        var playerId = Guid.NewGuid();
        var freelanceEventId = Guid.NewGuid();
        var donationEventId = Guid.NewGuid();
        var riskEventId = Guid.NewGuid();
        var playerEvents = new List<EventDb>
        {
            CreateEvent(freelanceEventId, sessionId, playerId, "work.freelance.completed", """{"amount":10}""", turn: 1, sequence: 1),
            CreateEvent(donationEventId, sessionId, playerId, "day.friday.donation", """{"amount":2}""", turn: 1, sequence: 2, weekday: "FRI"),
            CreateEvent(riskEventId, sessionId, playerId, "risk.life.drawn", """{"risk_id":"risk-a","direction":"OUT","amount":3}""", turn: 2, sequence: 3)
        };
        var allEvents = playerEvents
            .Concat(new[] { CreateEvent(Guid.NewGuid(), sessionId, null, "session.ended", "{}", turn: 3, sequence: 4) })
            .ToList();
        var projections = new List<CashflowProjectionDb>
        {
            CreateProjection(freelanceEventId, sessionId, playerId, "IN", 10, "FREELANCE"),
            CreateProjection(donationEventId, sessionId, playerId, "OUT", 2, "DONATION"),
            CreateProjection(riskEventId, sessionId, playerId, "OUT", 3, "RISK_LIFE")
        };
        var happiness = new AnalyticsHappinessBreakdown(
            Total: 4,
            NeedPoints: 1,
            NeedSetBonusPoints: 0,
            DonationPoints: 2,
            GoldPoints: 0,
            PensionPoints: 1,
            SavingGoalPointsEffective: 0,
            MissionPenaltyPoints: 0,
            LoanPenaltyPoints: 0,
            HasUnpaidLoan: false);

        var snapshot = new GameplaySnapshotBuilder().Build(playerEvents, projections, allEvents, config: null, happiness);

        using var rawDoc = JsonDocument.Parse(snapshot.RawJson);
        using var derivedDoc = JsonDocument.Parse(snapshot.DerivedJson);
        var raw = rawDoc.RootElement;
        var derived = derivedDoc.RootElement;

        Assert.Equal(sessionId, raw.GetProperty("metadata").GetProperty("session_id").GetGuid());
        Assert.Equal(playerId, raw.GetProperty("metadata").GetProperty("player_id").GetGuid());
        Assert.Equal(5, raw.GetProperty("coins").GetProperty("coins_net_end_game").GetDouble());
        Assert.True(raw.GetProperty("outcomes").GetProperty("finish_line_reached").GetBoolean());
        Assert.Equal(1, raw.GetProperty("life_risk").GetProperty("life_risk_cards_drawn").GetInt32());
        Assert.Equal(3, raw.GetProperty("life_risk").GetProperty("life_risk_costs_total").GetInt32());
        Assert.Equal("work.freelance.completed", raw.GetProperty("actions").GetProperty("action_sequence")[0].GetProperty("actions")[0].GetString());
        Assert.Equal(30, derived.GetProperty("risk_exposure_percentage").GetDouble());
        Assert.Equal(0, derived.GetProperty("income_diversification_index").GetDouble());
    }

    private static EventDb CreateEvent(
        Guid eventId,
        Guid sessionId,
        Guid? playerId,
        string actionType,
        string payload,
        int turn,
        long sequence,
        string weekday = "MON")
    {
        return new EventDb
        {
            EventId = eventId,
            SessionId = sessionId,
            PlayerId = playerId,
            ActorType = playerId.HasValue ? "PLAYER" : "SYSTEM",
            Timestamp = new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero).AddMinutes(sequence),
            DayIndex = turn - 1,
            Weekday = weekday,
            TurnNumber = turn,
            SequenceNumber = sequence,
            ActionType = actionType,
            RulesetVersionId = Guid.NewGuid(),
            Payload = payload
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
