// Fungsi file: Memverifikasi perilaku API, database, atau domain melalui AnalyticsGameplaySnapshotBuilderTests.
using System.Text.Json;
using Cashflowpoly.Api.Contracts;
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
            CreateEvent(freelanceEventId, sessionId, playerId, "KerjaLepas", """{"amount":10}""", turn: 1, sequence: 1),
            CreateEvent(donationEventId, sessionId, playerId, "JumatBerkah", """{"amount":2}""", turn: 1, sequence: 2, weekday: "FRI"),
            CreateEvent(riskEventId, sessionId, playerId, "RisikoKehidupan", """{"risk_id":"risk-a","direction":"OUT","amount":3}""", turn: 2, sequence: 3)
        };
        var allEvents = playerEvents
            .Concat(new[] { CreateEvent(Guid.NewGuid(), sessionId, null, "AkhiriSesi", "{}", turn: 3, sequence: 4) })
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

        var finalScore = new SessionFinalScoreDb
        {
            UserId = playerId,
            PlayerOrder = 1,
            Rank = 2,
            TotalPoints = happiness.Total,
            PensionPoints = happiness.PensionPoints
        };
        var snapshot = new GameplaySnapshotBuilder().Build(
            playerEvents,
            projections,
            allEvents,
            config: BuildAdvancedConfig(),
            happiness,
            finalScore,
            pensionRank: 4,
            playerAlias: "Marco",
            sessionEnded: true);

        using var rawDoc = JsonDocument.Parse(snapshot.RawJson);
        using var derivedDoc = JsonDocument.Parse(snapshot.DerivedJson);
        var raw = rawDoc.RootElement;
        var derived = derivedDoc.RootElement;

        Assert.Equal(sessionId, raw.GetProperty("metadata").GetProperty("session_id").GetGuid());
        Assert.Equal(playerId, raw.GetProperty("metadata").GetProperty("user_id").GetGuid());
        Assert.Equal("Marco", raw.GetProperty("metadata").GetProperty("player_alias").GetString());
        Assert.Equal(15, raw.GetProperty("coins").GetProperty("coins_net_end_game").GetDouble());
        Assert.False(raw.GetProperty("outcomes").GetProperty("finish_line_reached").GetBoolean());
        Assert.Equal(2, raw.GetProperty("outcomes").GetProperty("final_rank").GetInt32());
        Assert.Equal(4, raw.GetProperty("pension").GetProperty("pension_fund_rank_per_game").GetInt32());
        Assert.False(raw.GetProperty("outcomes").GetProperty("winner_flag").GetBoolean());
        Assert.True(raw.GetProperty("outcomes").GetProperty("dnf_flag").GetBoolean());
        Assert.Equal(1, raw.GetProperty("life_risk").GetProperty("life_risk_cards_drawn").GetInt32());
        Assert.Equal(3, raw.GetProperty("life_risk").GetProperty("life_risk_costs_total").GetInt32());
        Assert.Equal("KerjaLepas", raw.GetProperty("actions").GetProperty("action_sequence")[0].GetProperty("actions")[0].GetString());
        Assert.Empty(raw.GetProperty("ingredients").GetProperty("ingredients_used_per_meal").EnumerateArray());
        Assert.Equal(1, raw.GetProperty("turns").GetProperty("day_when_first_risk_hit").GetInt32());
        Assert.Equal(1, raw.GetProperty("turns").GetProperty("day_game_completion").GetInt32());
        Assert.Equal(100, derived.GetProperty("risk_readiness_percent").GetDouble());
        Assert.Equal(0, derived.GetProperty("income_diversification_index").GetDouble());

        AssertProperties(raw.GetProperty("coins"), "starting_coins", "coins_held_current", "coins_spent_per_turn", "coins_earned_per_turn", "coins_donated", "coins_saved", "coins_net_end_game");
        AssertProperties(raw.GetProperty("ingredients"), "ingredients_collected", "ingredients_held_current", "ingredient_types_held", "ingredients_used_per_meal", "ingredients_wasted", "ingredient_investment_coins_total");
        AssertProperties(raw.GetProperty("meal_orders"), "meal_orders_claimed", "meal_order_income_per_order", "meal_order_income_total", "meal_orders_per_turn_average");
        AssertProperties(raw.GetProperty("needs"), "need_cards_purchased", "need_cards_owned_current", "primary_needs_owned", "secondary_needs_owned", "tertiary_needs_owned", "specific_tertiary_need", "collection_mission_complete", "need_cards_coins_spent");
        AssertProperties(raw.GetProperty("donations"), "donation_amount_per_friday", "donation_rank_per_friday", "donation_total_coins", "donation_champion_cards_earned", "donation_happiness_points");
        AssertProperties(raw.GetProperty("gold"), "gold_cards_initial", "gold_cards_purchased", "gold_cards_sold", "gold_cards_held_end", "gold_prices_per_purchase", "gold_price_per_sale", "gold_investment_coins_spent", "gold_investment_coins_earned", "gold_investment_net");
        AssertProperties(raw.GetProperty("pension"), "leftover_coins_end_game", "ingredient_cards_value_end", "coins_in_savings_goal", "pension_fund_total", "pension_fund_rank_per_game", "pension_fund_happiness_points");
        AssertProperties(raw.GetProperty("life_risk"), "life_risk_cards_drawn", "life_risk_costs_per_card", "life_risk_costs_total", "life_risk_mitigated_with_insurance", "insurance_payments_made", "emergency_options_used");
        AssertProperties(raw.GetProperty("financial_goals"), "financial_goals_attempted", "financial_goals_completed", "financial_goals_coins_per_goal", "financial_goals_balance_per_goal", "financial_goals_coins_total_invested", "financial_goals_incomplete_coins_wasted", "sharia_loans_taken", "sharia_loans_repaid", "sharia_loans_unpaid_end", "loan_penalty_if_unpaid");
        AssertProperties(raw.GetProperty("actions"), "actions_per_turn", "action_repetitions_per_turn", "action_sequence");
        AssertProperties(raw.GetProperty("turns"), "coins_per_turn_progression", "net_income_per_turn", "day_when_debt_introduced", "day_when_first_risk_hit", "day_game_completion");
        AssertProperties(derived,
            "cash_growth_percent",
            "income_diversification_index",
            "business_expense_share_percent",
            "meal_order_profit_margin_percent",
            "risk_readiness_percent",
            "loan_burden_percent",
            "financial_goal_progress_percent",
            "income_action_focus_percent",
            "ingredient_utilization_percent",
            "long_term_action_share_percent",
            "need_fulfillment_diversity_percent",
            "donation_commitment_score",
            "happiness_points_composition");
        AssertProperties(derived.GetProperty("income_action_focus_components"), "income_main_actions", "total_main_actions");
        AssertProperties(derived.GetProperty("long_term_action_share_components"), "saving_actions", "financial_goal_actions", "insurance_actions", "loan_repayment_actions", "total_main_actions");
        AssertProperties(derived.GetProperty("donation_commitment_components"), "donation_stability_index", "donated_resource_share", "friday_participation_rate");
        AssertProperties(derived.GetProperty("happiness_points_composition"), "total_happiness_points", "need_card_points", "need_set_bonus_points", "donation_points", "gold_points", "pension_points", "financial_goal_points", "mission_penalty_points", "loan_penalty_points");
        Assert.Equal(happiness.Total, derived.GetProperty("happiness_points_composition").GetProperty("total_happiness_points").GetDouble());
    }

    [Fact]
    public void Build_BeginnerModeOmitsAdvancedOnlyDataAndMetrics()
    {
        var happiness = new AnalyticsHappinessBreakdown(
            Total: 0,
            NeedPoints: 0,
            NeedSetBonusPoints: 0,
            DonationPoints: 0,
            GoldPoints: 0,
            PensionPoints: 0,
            SavingGoalPointsEffective: 0,
            MissionPenaltyPoints: 0,
            LoanPenaltyPoints: 0,
            HasUnpaidLoan: false);
        var config = BuildAdvancedConfig() with { Mode = "PEMULA", StartingCash = 20 };

        var snapshot = new GameplaySnapshotBuilder().Build(
            [],
            [],
            [],
            config,
            happiness);

        using var rawDoc = JsonDocument.Parse(snapshot.RawJson);
        using var derivedDoc = JsonDocument.Parse(snapshot.DerivedJson);
        var raw = rawDoc.RootElement;
        var derived = derivedDoc.RootElement;

        Assert.False(raw.TryGetProperty("life_risk", out _));
        Assert.False(raw.TryGetProperty("financial_goals", out _));
        Assert.False(derived.TryGetProperty("risk_readiness_percent", out _));
        Assert.False(derived.TryGetProperty("loan_burden_percent", out _));
        Assert.False(derived.TryGetProperty("financial_goal_progress_percent", out _));
        Assert.False(derived.TryGetProperty("long_term_action_share_percent", out _));
        Assert.True(derived.TryGetProperty("cash_growth_percent", out _));
        Assert.True(derived.TryGetProperty("happiness_points_composition", out _));
    }

    [Fact]
    public void Build_CapsFinancialGoalFundingAtTheAttemptedTargetCost()
    {
        var sessionId = Guid.NewGuid();
        var playerId = Guid.NewGuid();
        var events = new List<EventDb>
        {
            CreateEvent(Guid.NewGuid(), sessionId, playerId, "Menabung", """{"goal_id":"goal-a","amount":20}""", turn: 1, sequence: 1),
            CreateEvent(Guid.NewGuid(), sessionId, playerId, "Menabung", """{"goal_id":"goal-a","amount":18}""", turn: 2, sequence: 2),
            CreateEvent(Guid.NewGuid(), sessionId, playerId, "TujuanFinansial", """{"goal_id":"goal-a","points":10,"cost":35}""", turn: 2, sequence: 3)
        };
        var config = BuildAdvancedConfig() with
        {
            FinancialGoals =
            [
                new RulesetFinancialGoalDto
                {
                    Id = "goal-a",
                    Nama = "Goal A",
                    HargaBeli = 35,
                    PoinKebahagiaan = 10
                }
            ]
        };
        var happiness = new AnalyticsHappinessBreakdown(10, 0, 0, 0, 0, 0, 10, 0, 0, false);

        var snapshot = new GameplaySnapshotBuilder().Build(
            events,
            [],
            events,
            config,
            happiness);

        using var derivedDoc = JsonDocument.Parse(snapshot.DerivedJson);
        var derived = derivedDoc.RootElement;
        Assert.Equal(100, derived.GetProperty("financial_goal_progress_percent").GetDouble());
        Assert.Equal(
            35,
            derived.GetProperty("financial_goal_progress_components")
                .GetProperty("coins_committed_to_goals")
                .GetInt32());
        Assert.Equal(
            35,
            derived.GetProperty("financial_goal_progress_components")
                .GetProperty("attempted_goal_target_total")
                .GetInt32());
    }

    [Theory]
    [InlineData("SetupPinjamanAwal", "{\"loan_id\":\"setup-loan\",\"principal\":10,\"penalty_points\":15}", 0)]
    [InlineData("PinjamanSyariah", "{\"loan_id\":\"regular-loan\",\"principal\":10,\"penalty_points\":15}", 3)]
    [InlineData("GunakanOpsiDarurat", "{\"option_type\":\"TAKE_SHARIA_LOAN\",\"loan_id\":\"emergency-loan\",\"principal\":10,\"penalty_points\":15}", 5)]
    public void Build_ReportsTheFirstDayForEverySupportedLoanSource(
        string actionType,
        string payload,
        int dayIndex)
    {
        var sessionId = Guid.NewGuid();
        var playerId = Guid.NewGuid();
        var loanEvent = CreateEvent(
            Guid.NewGuid(),
            sessionId,
            playerId,
            actionType,
            payload,
            turn: dayIndex + 1,
            sequence: 1);
        var happiness = new AnalyticsHappinessBreakdown(0, 0, 0, 0, 0, 0, 0, 0, 15, true);

        var snapshot = new GameplaySnapshotBuilder().Build(
            [loanEvent],
            [],
            [loanEvent],
            BuildAdvancedConfig(),
            happiness);

        using var rawDoc = JsonDocument.Parse(snapshot.RawJson);
        Assert.Equal(
            dayIndex,
            rawDoc.RootElement.GetProperty("turns").GetProperty("day_when_debt_introduced").GetInt32());
    }

    [Fact]
    public void Build_RiskReadinessCountsOnlyResolvedRisksWithoutEmergency()
    {
        var sessionId = Guid.NewGuid();
        var playerId = Guid.NewGuid();
        var paidRiskId = Guid.NewGuid();
        var pendingRiskId = Guid.NewGuid();
        var emergencyRiskId = Guid.NewGuid();
        var events = new List<EventDb>
        {
            CreateEvent(paidRiskId, sessionId, playerId, GameActionCatalog.RisikoKehidupan, """{"risk_id":"paid"}""", 1, 1),
            CreateEvent(pendingRiskId, sessionId, playerId, GameActionCatalog.RisikoKehidupan, """{"risk_id":"pending"}""", 2, 2),
            CreateEvent(emergencyRiskId, sessionId, playerId, GameActionCatalog.RisikoKehidupan, """{"risk_id":"emergency"}""", 3, 3),
            CreateEvent(Guid.NewGuid(), sessionId, playerId, GameActionCatalog.RiskEmergencyUsed, $$"""{"risk_event_id":"{{emergencyRiskId}}","option_type":"SELL_NEED"}""", 3, 4)
        };
        var projections = new List<CashflowProjectionDb>
        {
            CreateProjection(paidRiskId, sessionId, playerId, "OUT", 3, "RISK_LIFE")
        };

        var snapshot = new GameplaySnapshotBuilder().Build(
            events,
            projections,
            events,
            BuildAdvancedConfig(),
            new AnalyticsHappinessBreakdown(0, 0, 0, 0, 0, 0, 0, 0, 0, false));

        using var derivedDoc = JsonDocument.Parse(snapshot.DerivedJson);
        var derived = derivedDoc.RootElement;
        Assert.Equal(1, derived.GetProperty("risk_readiness_components").GetProperty("risks_resolved_without_emergency").GetInt32());
        Assert.Equal(100d / 3d, derived.GetProperty("risk_readiness_percent").GetDouble(), precision: 8);
    }

    [Fact]
    public void Build_LongTermActionShareExcludesFreeInsuranceClaims()
    {
        var sessionId = Guid.NewGuid();
        var playerId = Guid.NewGuid();
        var riskEventId = Guid.NewGuid();
        var events = new List<EventDb>
        {
            CreateEvent(Guid.NewGuid(), sessionId, playerId, GameActionCatalog.Menabung, """{"amount":5}""", 1, 1),
            CreateEvent(Guid.NewGuid(), sessionId, playerId, GameActionCatalog.Asuransi, """{"premium":1}""", 1, 2),
            CreateEvent(Guid.NewGuid(), sessionId, playerId, GameActionCatalog.Asuransi, $$"""{"risk_event_id":"{{riskEventId}}"}""", 2, 3)
        };

        var snapshot = new GameplaySnapshotBuilder().Build(
            events,
            [],
            events,
            BuildAdvancedConfig(),
            new AnalyticsHappinessBreakdown(0, 0, 0, 0, 0, 0, 0, 0, 0, false));

        using var derivedDoc = JsonDocument.Parse(snapshot.DerivedJson);
        var derived = derivedDoc.RootElement;
        Assert.Equal(100, derived.GetProperty("long_term_action_share_percent").GetDouble());
        Assert.Equal(1, derived.GetProperty("long_term_action_share_components").GetProperty("insurance_actions").GetInt32());
        Assert.Equal(2, derived.GetProperty("long_term_action_share_components").GetProperty("total_main_actions").GetInt32());
    }

    private static void AssertProperties(JsonElement element, params string[] names)
    {
        foreach (var name in names)
        {
            Assert.True(element.TryGetProperty(name, out _), $"Missing gameplay metric: {name}");
        }
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
            UserId = playerId,
            ActorType = playerId.HasValue ? "PLAYER" : "SYSTEM",
            Timestamp = new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero).AddMinutes(sequence),
            DayIndex = turn - 1,
            Weekday = weekday,
            ActionSlot = turn,
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
            UserId = playerId,
            EventPk = Guid.NewGuid(),
            EventId = eventId,
            Timestamp = new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero),
            Direction = direction,
            Amount = amount,
            Category = category
        };
    }

    private static RulesetConfig BuildAdvancedConfig() => new(
        "MAHIR",
        2,
        10,
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
        true,
        true,
        true,
        1,
        null);
}
