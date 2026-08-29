// Fungsi file: Memverifikasi perilaku API, database, atau domain melalui AnalyticsHappinessCalculatorTests.
using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Domain;
using Xunit;

namespace Cashflowpoly.Api.Tests;

public sealed class AnalyticsHappinessCalculatorTests
{
    [Fact]
    public void ComputeByPlayer_IncludesSetupGoldLoanAndMissionFamily()
    {
        var playerId = Guid.NewGuid();
        var events = new List<EventDb>
        {
            BuildEvent(playerId, "SetupEmasAwal", """{"qty":1,"setup":"INITIAL"}""", 0, 1),
            BuildEvent(playerId, "SetupPinjamanAwal", """{"loan_id":"setup-loan","principal":10,"penalty_points":15}""", 0, 2),
            BuildEvent(playerId, "SetupMisiAwal", """{"mission_id":"misi-boneka","target_tertiary_card_id":"boneka","penalty_points":10,"require_primary":true,"require_secondary":true}""", 0, 3),
            BuildEvent(playerId, "Kebutuhan", """{"amount":2,"card_id":"primary-food","need_tier":"primer","points":1}""", 1, 4),
            BuildEvent(playerId, "Kebutuhan", """{"amount":3,"card_id":"secondary-school","need_tier":"sekunder","points":2}""", 1, 5),
            BuildEvent(playerId, "Kebutuhan", """{"amount":6,"card_id":"boneka_1","need_tier":"tersier","points":5}""", 1, 6),
            BuildEvent(playerId, "TujuanFinansial", """{"goal_id":"home","points":8,"cost":5}""", 1, 7)
        };
        var config = BuildConfig(new RulesetScoringConfig(
            [],
            [new QtyPoint(1, 3), new QtyPoint(2, 5), new QtyPoint(3, 8), new QtyPoint(4, 12)],
            []));

        var breakdown = new HappinessCalculator().ComputeByPlayer(events, [], config)[playerId];

        Assert.Equal(3, breakdown.GoldPoints);
        Assert.Equal(0, breakdown.MissionPenaltyPoints);
        Assert.Equal(15, breakdown.LoanPenaltyPoints);
        Assert.Equal(0, breakdown.SavingGoalPointsEffective);
        Assert.True(breakdown.HasUnpaidLoan);
    }

    [Fact]
    public void ComputeBreakdown_AwardsMixedNeedSetBonus()
    {
        var playerEvents = new List<EventDb>
        {
            BuildEvent("Kebutuhan", """{"amount":2,"card_id":"primary-food","points":1}"""),
            BuildEvent("Kebutuhan", """{"amount":3,"card_id":"secondary-school","points":2}"""),
            BuildEvent("Kebutuhan", """{"amount":4,"card_id":"tertiary-bike","points":3}""")
        };

        var breakdown = new HappinessCalculator().ComputeBreakdown(
            playerEvents,
            donationPoints: 0,
            goldPoints: 0,
            pensionPoints: 0);

        Assert.Equal(6, breakdown.NeedPoints);
        Assert.Equal(4, breakdown.NeedSetBonusPoints);
        Assert.Equal(10, breakdown.Total);
    }

    [Fact]
    public void ComputeBreakdown_CountsMixedAndSameNeedSetsIndependently()
    {
        var playerEvents = new List<EventDb>
        {
            BuildEvent("Kebutuhan", """{"card_id":"primer-1","need_tier":"primer","amount":1,"points":1}"""),
            BuildEvent("Kebutuhan", """{"card_id":"primer-2","need_tier":"primer","amount":1,"points":1}"""),
            BuildEvent("Kebutuhan", """{"card_id":"primer-3","need_tier":"primer","amount":1,"points":1}"""),
            BuildEvent("Kebutuhan", """{"card_id":"sekunder-1","need_tier":"sekunder","amount":1,"points":1}"""),
            BuildEvent("Kebutuhan", """{"card_id":"sekunder-2","need_tier":"sekunder","amount":1,"points":1}"""),
            BuildEvent("Kebutuhan", """{"card_id":"tersier-1","need_tier":"tersier","amount":1,"points":1}""")
        };

        var breakdown = new HappinessCalculator().ComputeBreakdown(
            playerEvents,
            donationPoints: 0,
            goldPoints: 0,
            pensionPoints: 0);

        Assert.Equal(6, breakdown.NeedPoints);
        Assert.Equal(6, breakdown.NeedSetBonusPoints);
        Assert.Equal(12, breakdown.Total);
    }

    [Fact]
    public void ComputeBreakdown_ExcludesNeedSoldForEmergency()
    {
        var playerEvents = new List<EventDb>
        {
            BuildEvent("Kebutuhan", """{"amount":2,"card_id":"primary-food","points":1}"""),
            BuildEvent("Kebutuhan", """{"amount":3,"card_id":"secondary-school","points":2}"""),
            BuildEvent("Kebutuhan", """{"amount":4,"card_id":"tertiary-bike","points":3}"""),
            BuildEvent("GunakanOpsiDarurat", """{"option_type":"SELL_NEED","direction":"IN","amount":1,"card_id":"secondary-school"}""")
        };

        var breakdown = new HappinessCalculator().ComputeBreakdown(
            playerEvents,
            donationPoints: 0,
            goldPoints: 0,
            pensionPoints: 0);

        Assert.Equal(4, breakdown.NeedPoints);
        Assert.Equal(0, breakdown.NeedSetBonusPoints);
        Assert.Equal(4, breakdown.Total);
    }

    [Fact]
    public void ComputeBreakdown_SoldNeedStillCountsAsMissionPurchaseButNotEndingHappiness()
    {
        var playerEvents = new List<EventDb>
        {
            BuildEvent("SetupMisiAwal", """{"mission_id":"mission-bike","target_tertiary_card_id":"tertiary-bike","penalty_points":10,"require_primary":true,"require_secondary":true}"""),
            BuildEvent("Kebutuhan", """{"amount":2,"card_id":"primary-food","need_tier":"primer","points":1}"""),
            BuildEvent("Kebutuhan", """{"amount":3,"card_id":"secondary-school","need_tier":"sekunder","points":2}"""),
            BuildEvent("Kebutuhan", """{"amount":4,"card_id":"tertiary-bike","need_tier":"tersier","points":3}"""),
            BuildEvent("GunakanOpsiDarurat", """{"option_type":"SELL_NEED","direction":"IN","amount":2,"card_id":"tertiary-bike"}""")
        };

        var breakdown = new HappinessCalculator().ComputeBreakdown(
            playerEvents,
            donationPoints: 0,
            goldPoints: 0,
            pensionPoints: 0);

        Assert.Equal(3, breakdown.NeedPoints);
        Assert.Equal(0, breakdown.NeedSetBonusPoints);
        Assert.Equal(0, breakdown.MissionPenaltyPoints);
        Assert.Equal(3, breakdown.Total);
    }

    [Fact]
    public void ComputeBreakdown_SuppressesSavingGoalPoints_WhenLoanIsUnpaid()
    {
        var playerEvents = new List<EventDb>
        {
            BuildEvent("TujuanFinansial", """{"goal_id":"bike","points":8,"cost":5}"""),
            BuildEvent("PinjamanSyariah", """{"loan_id":"loan-1","principal":10,"penalty_points":15}"""),
            BuildEvent("BayarPinjaman", """{"loan_id":"loan-1","amount":4}""")
        };

        var breakdown = new HappinessCalculator().ComputeBreakdown(
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
            BuildEvent(firstPlayerId, "JumatBerkah", """{"amount":5}""", dayIndex: 4, sequenceNumber: 1),
            BuildEvent(secondPlayerId, "JumatBerkah", """{"amount":5}""", dayIndex: 4, sequenceNumber: 2),
            BuildEvent(firstPlayerId, "BagikanTieBreaker", """{"number":1}""", dayIndex: 4, sequenceNumber: 3),
            BuildEvent(secondPlayerId, "BagikanTieBreaker", """{"number":9}""", dayIndex: 4, sequenceNumber: 4),
            BuildEvent(firstPlayerId, "InvestasiEmas", """{"trade_type":"BUY","qty":2,"unit_price":5,"amount":10}""", dayIndex: 5, sequenceNumber: 5)
        };

        var config = BuildConfig(new RulesetScoringConfig(
            [new RankPoint(1, 7), new RankPoint(2, 5)],
            [new QtyPoint(1, 3), new QtyPoint(2, 5)],
            []));

        var byPlayer = new HappinessCalculator().ComputeByPlayer(events, [], config);

        Assert.Equal(10, byPlayer[firstPlayerId].Total);
        Assert.Equal(5, byPlayer[firstPlayerId].DonationPoints);
        Assert.Equal(5, byPlayer[firstPlayerId].GoldPoints);
        Assert.Equal(7, byPlayer[secondPlayerId].Total);
        Assert.Equal(7, byPlayer[secondPlayerId].DonationPoints);
    }

    [Fact]
    public void ComputeByPlayer_CapsGoldScoreAtHighestRulebookTier()
    {
        var playerId = Guid.NewGuid();
        var events = new List<EventDb>
        {
            BuildEvent(playerId, "InvestasiEmas", """{"trade_type":"BUY","qty":5,"unit_price":5,"amount":25}""", dayIndex: 5, sequenceNumber: 1)
        };
        var config = BuildConfig(new RulesetScoringConfig(
            [],
            [new QtyPoint(1, 3), new QtyPoint(2, 5), new QtyPoint(3, 8), new QtyPoint(4, 12)],
            []));

        var byPlayer = new HappinessCalculator().ComputeByPlayer(events, [], config);

        Assert.Equal(12, byPlayer[playerId].GoldPoints);
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
            UserId = playerId,
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
            PlayerOrdering.PlayerOrder,
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
