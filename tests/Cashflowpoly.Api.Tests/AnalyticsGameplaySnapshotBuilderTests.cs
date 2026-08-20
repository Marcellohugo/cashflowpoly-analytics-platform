// Fungsi file: Memverifikasi perilaku API, database, atau domain melalui AnalyticsGameplaySnapshotBuilderTests.
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
            config: null,
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
        Assert.Equal(5, raw.GetProperty("coins").GetProperty("coins_net_end_game").GetDouble());
        Assert.False(raw.GetProperty("outcomes").GetProperty("finish_line_reached").GetBoolean());
        Assert.Equal(2, raw.GetProperty("outcomes").GetProperty("final_rank").GetInt32());
        Assert.Equal(4, raw.GetProperty("pension").GetProperty("pension_fund_rank_per_game").GetInt32());
        Assert.False(raw.GetProperty("outcomes").GetProperty("winner_flag").GetBoolean());
        Assert.True(raw.GetProperty("outcomes").GetProperty("dnf_flag").GetBoolean());
        Assert.Equal(1, raw.GetProperty("life_risk").GetProperty("life_risk_cards_drawn").GetInt32());
        Assert.Equal(3, raw.GetProperty("life_risk").GetProperty("life_risk_costs_total").GetInt32());
        Assert.Equal("KerjaLepas", raw.GetProperty("actions").GetProperty("action_sequence")[0].GetProperty("actions")[0].GetString());
        Assert.Empty(raw.GetProperty("ingredients").GetProperty("ingredients_used_per_meal").EnumerateArray());
        Assert.Equal(1, raw.GetProperty("turns").GetProperty("turn_number_when_first_risk_hit").GetInt32());
        Assert.Equal(1, raw.GetProperty("turns").GetProperty("turn_number_game_completion").GetInt32());
        Assert.Equal(30, derived.GetProperty("risk_exposure_percentage").GetDouble());
        Assert.Equal(0, derived.GetProperty("income_diversification_index").GetDouble());

        AssertProperties(raw.GetProperty("coins"), "starting_coins", "coins_held_current", "coins_spent_per_turn", "coins_earned_per_turn", "coins_donated", "coins_saved", "coins_net_end_game");
        AssertProperties(raw.GetProperty("ingredients"), "ingredients_collected", "ingredients_held_current", "ingredient_types_held", "ingredients_used_per_meal", "ingredients_wasted", "ingredient_investment_coins_total");
        AssertProperties(raw.GetProperty("meal_orders"), "meal_orders_claimed", "meal_orders_available_passed", "meal_order_income_per_order", "meal_order_income_total", "meal_orders_per_turn_average");
        AssertProperties(raw.GetProperty("needs"), "need_cards_purchased", "need_cards_owned_current", "primary_needs_owned", "secondary_needs_owned", "tertiary_needs_owned", "specific_tertiary_need", "collection_mission_complete", "need_cards_coins_spent");
        AssertProperties(raw.GetProperty("donations"), "donation_amount_per_friday", "donation_rank_per_friday", "donation_total_coins", "donation_champion_cards_earned", "donation_happiness_points");
        AssertProperties(raw.GetProperty("gold"), "gold_cards_purchased", "gold_cards_sold", "gold_cards_held_end", "gold_prices_per_purchase", "gold_price_per_sale", "gold_investment_coins_spent", "gold_investment_coins_earned", "gold_investment_net");
        AssertProperties(raw.GetProperty("pension"), "leftover_coins_end_game", "ingredient_cards_value_end", "coins_in_savings_goal", "pension_fund_total", "pension_fund_rank_per_game", "pension_fund_happiness_points");
        AssertProperties(raw.GetProperty("life_risk"), "life_risk_cards_drawn", "life_risk_costs_per_card", "life_risk_costs_total", "life_risk_mitigated_with_insurance", "insurance_payments_made", "emergency_options_used");
        AssertProperties(raw.GetProperty("financial_goals"), "financial_goals_attempted", "financial_goals_completed", "financial_goals_coins_per_goal", "financial_goals_balance_per_goal", "financial_goals_coins_total_invested", "financial_goals_incomplete_coins_wasted", "sharia_loan_cards_taken", "sharia_loans_repaid", "sharia_loans_unpaid_end", "loan_penalty_if_unpaid");
        AssertProperties(raw.GetProperty("actions"), "actions_per_turn", "action_repetitions_per_turn", "action_sequence", "actions_skipped", "action_slots_unused");
        AssertProperties(raw.GetProperty("turns"), "coins_per_turn_progression", "net_income_per_turn", "turn_number_when_debt_introduced", "turn_number_when_first_risk_hit", "turn_number_game_completion");
        AssertProperties(derived, "net_worth_index", "income_diversification_ratio", "expense_management_efficiency", "business_profit_margin", "risk_appetite_score", "debt_leverage_ratio", "loan_repayment_discipline", "goal_ambition_index", "goal_setting_ambition", "action_efficiency_percent", "meal_order_success_rate", "planning_horizon_percent", "fulfillment_diversity", "fulfillment_diversity_document_formula", "mission_achievement", "donation_stability", "donation_stability_index", "donation_commitment_score", "happiness_portfolio");
        AssertProperties(derived.GetProperty("action_efficiency_components"), "income_producing_actions", "all_player_actions");
        AssertProperties(derived.GetProperty("planning_horizon_components"), "savings_actions", "financial_goal_actions", "insurance_premium_actions", "all_player_actions");
        AssertProperties(derived.GetProperty("donation_commitment_components"), "donation_stability_index", "donation_ratio", "friday_participation_rate");
        AssertProperties(derived.GetProperty("happiness_portfolio"), "total_happiness_pts", "need_cards_pts", "need_set_bonus_pts", "donations_pts", "gold_pts", "pension_pts", "financial_goals_pts", "mission_bonus_pts", "loan_penalty_pts");
        Assert.Equal(happiness.Total, derived.GetProperty("happiness_portfolio").GetProperty("total_happiness_pts").GetDouble());
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
}
