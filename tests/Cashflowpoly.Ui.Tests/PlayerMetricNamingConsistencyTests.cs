// Fungsi file: Memverifikasi konsistensi nama variabel, rumus, dan sumber data analitik pemain.
using Cashflowpoly.Ui.Infrastructure;
using Xunit;

namespace Cashflowpoly.Ui.Tests;

public sealed class PlayerMetricNamingConsistencyTests
{
    public static TheoryData<string, string, string, string[]> Formulas => new()
    {
        { "net-worth", "cash_growth_percent", "net_worth", ["coins_net_end_game", "starting_coins"] },
        { "income-diversification", "income_diversification_index", "income_diversification_index", ["income_shares", "active_income_source_count"] },
        { "expense-efficiency", "business_expense_share_percent", "expense_efficiency", ["ingredient_investment_coins_total", "total_cash_out"] },
        { "business-margin", "meal_order_profit_margin_percent", "business_margin", ["meal_order_income_total", "ingredient_cost_used"] },
        { "risk-appetite", "risk_readiness_percent", "risk_appetite", ["risks_resolved_without_emergency", "life_risk_cards_drawn"] },
        { "debt-discipline", "loan_burden_percent", "debt_leverage", ["outstanding_loan", "liquid_assets"] },
        { "goal-ambition", "financial_goal_progress_percent", "goal_ambition_index", ["coins_committed_to_goals", "attempted_goal_target_total"] },
        { "action-efficiency", "income_action_focus_percent", "action_efficiency", ["income_main_actions", "total_main_actions"] },
        { "meal-success", "ingredient_utilization_percent", "order_success", ["ingredients_used_in_completed_orders", "ingredients_collected"] },
        { "planning-horizon", "long_term_action_share_percent", "planning", ["saving_actions", "financial_goal_actions", "insurance_actions", "loan_repayment_actions", "total_main_actions"] },
        { "fulfillment-diversity", "need_fulfillment_diversity_percent", "fulfillment", ["primary_need_share", "secondary_need_share", "tertiary_need_share"] },
        { "donation-commitment", "donation_commitment_score", "donation_commitment", ["donation_stability_index", "donated_resource_share", "friday_participation_rate"] },
        { "happiness-portfolio", "total_happiness_points", "happiness_portfolio", ["need_card_points", "need_set_bonus_points", "donation_points", "gold_points", "pension_points", "financial_goal_points", "mission_penalty_points", "loan_penalty_points"] },
        { "happiness-portfolio.beginner", "total_happiness_points", "happiness_portfolio.beginner", ["need_card_points", "need_set_bonus_points", "donation_points", "gold_points", "pension_points", "mission_penalty_points"] }
    };

    [Theory]
    [MemberData(nameof(Formulas))]
    public void FormulaAndSource_UseTheSameNamesAsTheirCards(
        string analysisKey, string metricKey, string formulaKey, string[] inputs)
    {
        foreach (var language in new[] { "id", "en" })
        {
            string Translate(string key) => UiText.Translate(language, key);
            string Label(string key) => PlayerMetricLabelFormatter.HumanizeMetricKey(key, Translate);
            var formula = Translate($"players.support.formula.{formulaKey}");
            var source = Translate($"players.analysis.source.{analysisKey}");

            Assert.StartsWith($"{Label(metricKey)} = ", formula, StringComparison.Ordinal);
            foreach (var input in inputs)
            {
                var label = Label(input);
                Assert.DoesNotContain("_", label, StringComparison.Ordinal);
                Assert.Contains(label, formula, StringComparison.Ordinal);
                if (!analysisKey.StartsWith("happiness-portfolio", StringComparison.Ordinal))
                {
                    Assert.Contains(label, source, StringComparison.Ordinal);
                }
            }
        }
    }

    [Theory]
    [InlineData("players.raw.coins_held_current", "players.raw.coins_net_end_game")]
    [InlineData("players.raw.coins_net_end_game", "players.cashflow_journey.ending_cash")]
    [InlineData("players.raw.starting_coins", "players.cashflow_journey.starting_cash")]
    [InlineData("players.raw.cash_out_total", "players.metric.total_cash_out")]
    [InlineData("players.raw.cash_out_total", "metric.cash_out")]
    [InlineData("players.raw.cash_in_total", "metric.cash_in")]
    [InlineData("players.raw.ingredients_used_total", "players.metric.ingredients_used_in_completed_orders")]
    [InlineData("players.raw.meal_order_income_total", "players.metric.meal_order_income")]
    [InlineData("players.raw.sharia_loans_outstanding_coins", "players.metric.outstanding_loan")]
    [InlineData("players.raw.financial_goals_coins_total_invested", "players.metric.coins_committed_to_goals")]
    [InlineData("players.metric.need_fulfillment_diversity_percent", "metric.fulfillment_diversity")]
    [InlineData("players.raw.pension_fund_happiness_points", "players.metric.pension_points")]
    [InlineData("players.raw.loan_penalty_if_unpaid", "players.metric.loan_penalty_points")]
    [InlineData("metric.mission_penalty", "players.metric.mission_penalty_points")]
    [InlineData("metric.loan_penalty", "players.metric.loan_penalty_points")]
    public void SharedValues_KeepTheSameNameAcrossSummaryRawDataAndAnalysis(string first, string second)
    {
        foreach (var language in new[] { "id", "en" })
        {
            Assert.Equal(UiText.Translate(language, first), UiText.Translate(language, second));
        }
    }

    [Theory]
    [InlineData("risk_readiness_percent", "risk_readiness")]
    [InlineData("loan_burden_percent", "loan_burden")]
    [InlineData("ingredient_utilization_percent", "ingredient_utilization")]
    public void CurrentMetrics_DoNotUseAnUnrelatedLegacyExplanation(string key, string explanation)
    {
        var metric = PlayerMetricLabelFormatter.DescribeMetric(key, "50", true, true, "—", text => text);
        Assert.Equal($"players.support.meaning.{explanation}", metric.Explanation);
    }

    [Theory]
    [InlineData("id")]
    [InlineData("en")]
    public void DonationCommitment_IsNotNamedAsAmountRegularity(string language)
    {
        Assert.NotEqual(
            UiText.Translate(language, "players.metric.donation_commitment_score"),
            UiText.Translate(language, "players.metric.donation_stability_index"));
    }

    [Theory]
    [InlineData("id", "Perubahan Koin per Kejadian", "Konteks Aksi", "Perubahan Koin")]
    [InlineData("en", "Coin Change per Event", "Action Context", "Coin Change")]
    public void EventCashTimeline_UsesEventBasedNames(
        string language,
        string timelineLabel,
        string actionContextLabel,
        string changeLabel)
    {
        Assert.Equal(timelineLabel, UiText.Translate(language, "players.raw.net_income_per_turn"));
        Assert.Equal(actionContextLabel, UiText.Translate(language, "players.raw.action_slot"));
        Assert.Equal(changeLabel, UiText.Translate(language, "players.raw.net"));

        var metric = PlayerMetricLabelFormatter.DescribeMetric(
            "net_income_per_turn",
            "1",
            false,
            true,
            "—",
            key => UiText.Translate(language, key));

        Assert.Contains(language == "id" ? "Nilai positif" : "A positive value", metric.Explanation);
        Assert.Contains(language == "id" ? "nilai negatif" : "negative value", metric.Explanation);
    }
}
