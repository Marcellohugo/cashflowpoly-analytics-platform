// Fungsi file: Menguji kalkulator happiness analitik yang dipakai ringkasan dan snapshot gameplay.
using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Domain;
using Xunit;

namespace Cashflowpoly.Api.Tests;

public sealed class AnalyticsHappinessCalculatorTests
{
    [Fact]
    public void ComputeBreakdown_AwardsMixedNeedSetBonus()
    {
        var playerEvents = new List<EventDb>
        {
            BuildEvent("need.primary.purchased", """{"amount":2,"card_id":"primary-food","points":1}"""),
            BuildEvent("need.secondary.purchased", """{"amount":3,"card_id":"secondary-school","points":2}"""),
            BuildEvent("need.tertiary.purchased", """{"amount":4,"card_id":"tertiary-bike","points":3}""")
        };

        var breakdown = AnalyticsHappinessCalculator.ComputeBreakdown(
            playerEvents,
            donationPoints: 0,
            goldPoints: 0,
            pensionPoints: 0);

        Assert.Equal(6, breakdown.NeedPoints);
        Assert.Equal(4, breakdown.NeedSetBonusPoints);
        Assert.Equal(10, breakdown.Total);
    }

    [Fact]
    public void ComputeBreakdown_SuppressesSavingGoalPoints_WhenLoanIsUnpaid()
    {
        var playerEvents = new List<EventDb>
        {
            BuildEvent("saving.goal.achieved", """{"goal_id":"bike","points":8,"cost":5}"""),
            BuildEvent("loan.syariah.taken", """{"loan_id":"loan-1","principal":10,"penalty_points":15}"""),
            BuildEvent("loan.syariah.repaid", """{"loan_id":"loan-1","amount":4}""")
        };

        var breakdown = AnalyticsHappinessCalculator.ComputeBreakdown(
            playerEvents,
            donationPoints: 0,
            goldPoints: 0,
            pensionPoints: 0);

        Assert.True(breakdown.HasUnpaidLoan);
        Assert.Equal(0, breakdown.SavingGoalPointsEffective);
        Assert.Equal(15, breakdown.LoanPenaltyPoints);
        Assert.Equal(-15, breakdown.Total);
    }

    [Fact]
    public void ComputeByPlayer_UsesConfiguredDonationTieBreakerAndGoldQuantity()
    {
        var firstPlayerId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var secondPlayerId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
        var events = new List<EventDb>
        {
            BuildEvent(firstPlayerId, "day.friday.donation", """{"amount":5}""", dayIndex: 4, sequenceNumber: 1),
            BuildEvent(secondPlayerId, "day.friday.donation", """{"amount":5}""", dayIndex: 4, sequenceNumber: 2),
            BuildEvent(firstPlayerId, "tie_breaker.assigned", """{"number":1}""", dayIndex: 4, sequenceNumber: 3),
            BuildEvent(secondPlayerId, "tie_breaker.assigned", """{"number":9}""", dayIndex: 4, sequenceNumber: 4),
            BuildEvent(firstPlayerId, "day.saturday.gold_trade", """{"trade_type":"BUY","qty":2,"unit_price":5,"amount":10}""", dayIndex: 5, sequenceNumber: 5)
        };

        var config = BuildConfig(new RulesetScoringConfig(
            [new RankPoint(1, 7), new RankPoint(2, 5)],
            [new QtyPoint(1, 3), new QtyPoint(2, 5)],
            []));

        var byPlayer = AnalyticsHappinessCalculator.ComputeByPlayer(events, [], config);

        Assert.Equal(10, byPlayer[firstPlayerId].Total);
        Assert.Equal(5, byPlayer[firstPlayerId].DonationPoints);
        Assert.Equal(5, byPlayer[firstPlayerId].GoldPoints);
        Assert.Equal(7, byPlayer[secondPlayerId].Total);
        Assert.Equal(7, byPlayer[secondPlayerId].DonationPoints);
    }

    private static EventDb BuildEvent(string actionType, string payload)
    {
        return BuildEvent(Guid.NewGuid(), actionType, payload, dayIndex: 0, sequenceNumber: 1);
    }

    private static EventDb BuildEvent(Guid playerId, string actionType, string payload, int dayIndex, long sequenceNumber)
    {
        return new EventDb
        {
            EventId = Guid.NewGuid(),
            SessionId = Guid.NewGuid(),
            PlayerId = playerId,
            ActionType = actionType,
            Payload = payload,
            DayIndex = dayIndex,
            SequenceNumber = sequenceNumber
        };
    }

    private static RulesetConfig BuildConfig(RulesetScoringConfig scoring)
    {
        return new RulesetConfig(
            "PEMULA",
            2,
            20,
            PlayerOrdering.JoinOrder,
            0,
            6,
            3,
            1,
            true,
            true,
            true,
            true,
            1,
            999,
            true,
            true,
            false,
            false,
            false,
            1,
            scoring);
    }
}
