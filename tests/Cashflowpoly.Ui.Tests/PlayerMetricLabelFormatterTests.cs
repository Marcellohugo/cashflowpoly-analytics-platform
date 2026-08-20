// Fungsi file: Memverifikasi perilaku, lokalisasi, atau tata letak UI melalui PlayerMetricLabelFormatterTests.
using System.Globalization;
using Cashflowpoly.Ui.Infrastructure;
using Xunit;

namespace Cashflowpoly.Ui.Tests;

public sealed class PlayerMetricLabelFormatterTests
{
    [Fact]
    public void FormatMetricPathLabel_LocalizesKnownSegmentsAndArrayIndexes()
    {
        var label = PlayerMetricLabelFormatter.FormatMetricPathLabel(
            "coins.coins_net_end_game.history[1]",
            Translate);

        Assert.Equal("Coins - Ending Coins - History - Item 2", label);
    }

    [Theory]
    [InlineData("CoinsSpentPerTurn[0].Amount", "Coins Spent per Turn - Item 1 - Amount")]
    [InlineData("ingredientTypesHeld.White Rice", "Ingredient Types Held - White Rice")]
    [InlineData("incomeDiversificationComponents.FreelanceIncome", "Income Diversification Components - Freelance Income")]
    public void FormatMetricPathLabel_NormalizesGameplayMetricKeysBeforeLocalization(string path, string expected)
    {
        var label = PlayerMetricLabelFormatter.FormatMetricPathLabel(path, Translate);

        Assert.Equal(expected, label);
    }

    [Fact]
    public void LocalizeTransactionDetail_FormatsOpeningCashAndCashflowCategories()
    {
        var opening = PlayerMetricLabelFormatter.LocalizeTransactionDetail("START - CASH (20)", Translate);
        var outflow = PlayerMetricLabelFormatter.LocalizeTransactionDetail("OUT - GOLD_TRADE (12)", Translate);

        Assert.Equal("Opening Cash (20)", opening);
        Assert.Equal("Cash Out - Gold Trade (12)", outflow);
    }

    [Theory]
    [InlineData("true", true, 1)]
    [InlineData("false", true, 0)]
    [InlineData("12.5", true, 12.5)]
    [InlineData("not-a-number", false, 0)]
    public void TryParseMetricNumber_ParsesBooleanAndNumericValues(string rawValue, bool expectedResult, double expectedValue)
    {
        var previousCulture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("id-ID");

            var result = PlayerMetricLabelFormatter.TryParseMetricNumber(rawValue, out var value);

            Assert.Equal(expectedResult, result);
            Assert.Equal(expectedValue, value);
        }
        finally
        {
            CultureInfo.CurrentCulture = previousCulture;
        }
    }

    [Theory]
    [InlineData("summary", true, true)]
    [InlineData("coins.total", false, true)]
    [InlineData("coins.history[0]", false, false)]
    public void CombinedSummaryPathFilters_ClassifyCompactRows(string path, bool expectedPreferred, bool expectedFallback)
    {
        Assert.Equal(expectedPreferred, PlayerMetricLabelFormatter.IsPreferredCombinedSummaryPath(path));
        Assert.Equal(expectedFallback, PlayerMetricLabelFormatter.IsFallbackCombinedSummaryPath(path));
    }

    [Theory]
    [InlineData("coins_saved", false, "savings")]
    [InlineData("cash_in_total", false, "income")]
    [InlineData("coins_donated", false, "donation")]
    [InlineData("ingredients_wasted", false, "inventory")]
    [InlineData("meal_orders_per_turn_average", false, "business_activity")]
    [InlineData("donation_events", false, "donation")]
    [InlineData("action_sequence[0].action_type", false, "timeline")]
    [InlineData("sharia_loans_unpaid_end", false, "debt")]
    [InlineData("average_risk_cost", true, "risk")]
    public void DescribeMetric_UsesTheDataFunctionInsteadOfAGenericUnitDescription(
        string path,
        bool isDerived,
        string expectedCategory)
    {
        var result = PlayerMetricLabelFormatter.DescribeMetric(
            path,
            "5",
            isDerived,
            true,
            "Unavailable",
            key => key);

        Assert.Equal($"players.support.meaning.{expectedCategory}", result.Explanation);
        Assert.Equal($"players.support.guide.{expectedCategory}", result.Guidance);
        Assert.Equal($"players.support.recommendation.{expectedCategory}", result.Recommendation);
    }

    [Theory]
    [InlineData("net_worth_index", "players.support.recommendation.cash_recover")]
    [InlineData("income_diversification_ratio", "players.support.recommendation.income_mix_recover")]
    [InlineData("insurance_coverage_rate", "players.support.recommendation.protection")]
    [InlineData("friday_participation_rate", "players.support.recommendation.donation")]
    [InlineData("planning_horizon", "players.support.recommendation.planning")]
    [InlineData("unknown_result", "players.support.recommendation.balance")]
    public void DescribeMetric_AddsARelevantFinancialLiteracyConsideration(
        string path,
        string expectedRecommendation)
    {
        var result = PlayerMetricLabelFormatter.DescribeMetric(
            path,
            "5",
            true,
            true,
            "Unavailable",
            key => key);

        Assert.Equal(expectedRecommendation, result.Recommendation);
    }

    [Theory]
    [InlineData("net_worth_index", "120", "players.support.recommendation.wealth")]
    [InlineData("business_profit_margin", "-1", "players.support.recommendation.business_recover")]
    [InlineData("business_profit_margin", "10", "players.support.recommendation.business_activity")]
    [InlineData("action_efficiency_percent", "20", "players.support.recommendation.action_income")]
    [InlineData("action_efficiency_percent", "60", "players.support.recommendation.action_balance")]
    [InlineData("fulfillment_diversity", "0.2", "players.support.recommendation.needs_balance")]
    [InlineData("donation_commitment_score", "20", "players.support.recommendation.donation_stabilize")]
    public void DescribeMetric_AdaptsTheConsiderationToTheResult(
        string path,
        string value,
        string expectedRecommendation)
    {
        var result = PlayerMetricLabelFormatter.DescribeMetric(
            path,
            value,
            true,
            true,
            "Unavailable",
            key => key);

        Assert.Equal(expectedRecommendation, result.Recommendation);
    }

    [Theory]
    [InlineData("expense_management_efficiency", "50", "players.support.guide.expense_efficiency")]
    [InlineData("income_diversification_ratio", "50", "players.support.guide.income_mixed")]
    [InlineData("planning_horizon", "0.5", "players.support.guide.planning_long")]
    public void DescribeMetric_UsesFunctionalGuidanceForDerivedMetrics(
        string path,
        string value,
        string expectedGuidance)
    {
        var result = PlayerMetricLabelFormatter.DescribeMetric(
            path,
            value,
            true,
            true,
            "Unavailable",
            key => key);

        Assert.Equal(expectedGuidance, result.Guidance);
    }

    [Theory]
    [InlineData("action_efficiency", "0.45", "45", "players.support.unit.percent")]
    [InlineData("growth_pattern_ratio", "1.5", "1.50", "players.support.unit.multiplier")]
    [InlineData("meal_orders_per_turn_average", "0.25", "0.25", "players.support.unit.orders_per_turn")]
    [InlineData("ingredients_used_per_meal_average", "2", "2", "players.support.unit.ingredients_per_order")]
    [InlineData("donation_stability_std_deviation", "1.2", "1.20", "players.support.unit.coins")]
    [InlineData("action_repetitions_per_turn[0].diversity_score", "0.5", "50", "players.support.unit.percent")]
    [InlineData("coins_spent_per_turn[0].action_slot", "2", "2", "players.support.unit.action_slot")]
    [InlineData("coins_spent_per_turn[0].amount", "12", "12", "players.support.unit.coins")]
    [InlineData("N_active_income_sources", "3", "3", "players.support.unit.sources")]
    [InlineData("actions_skipped", "2", "2", "players.support.unit.day")]
    [InlineData("action_slots_unused", "3", "3", "players.support.unit.action_slot")]
    [InlineData("goal_ambition_index", "62.5", "62.50", "players.support.unit.percent")]
    [InlineData("goal_setting_ambition", "255.33", "255.33", "")]
    [InlineData("fulfillment_diversity_document_formula", "0.625", "62.50", "players.support.unit.percent")]
    [InlineData("donation_stability_index", "88.5", "88.50", "players.support.unit.percent")]
    [InlineData("income_producing_actions", "10", "10", "players.support.unit.actions")]
    [InlineData("planning_horizon_components.financial_goal_actions", "0", "0", "players.support.unit.actions")]
    [InlineData("financial_goals_balance_per_goal.tujuan_35", "3", "3", "players.support.unit.coins")]
    public void DescribeMetric_UsesDimensionallyCorrectDisplayValuesAndUnits(
        string path,
        string value,
        string expectedDisplay,
        string expectedUnit)
    {
        var previousCulture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            var result = PlayerMetricLabelFormatter.DescribeMetric(
                path,
                value,
                true,
                true,
                "Unavailable",
                key => key);

            Assert.Equal(expectedDisplay, result.DisplayValue);
            Assert.Equal(expectedUnit, result.Unit);
        }
        finally
        {
            CultureInfo.CurrentCulture = previousCulture;
        }
    }

    [Fact]
    public void DescribeMetric_DoesNotAppendAUnitToUnavailableValues()
    {
        var result = PlayerMetricLabelFormatter.DescribeMetric(
            "fulfillment_diversity",
            "Unavailable",
            true,
            true,
            "Unavailable",
            key => key);

        Assert.Equal("players.support.value.unavailable", result.DisplayValue);
        Assert.Empty(result.Unit);
    }

    private static string Translate(string key)
    {
        return key switch
        {
            "players.details.metric_fallback" => "Data",
            "players.details.item" => "Item",
            "players.details.series" => "Series",
            "common.value" => "Value",
            "players.raw.coins" => "Coins",
            "players.raw.coins_net_end_game" => "Ending Coins",
            "players.raw.coins_spent_per_turn" => "Coins Spent per Turn",
            "players.raw.amount" => "Amount",
            "players.raw.ingredient_types_held" => "Ingredient Types Held",
            "players.raw.white_rice" => "White Rice",
            "players.metric.income_diversification_components" => "Income Diversification Components",
            "players.metric.freelance_income" => "Freelance Income",
            "players.details.transaction_label" => "Transaction",
            "players.details.transaction.opening_cash" => "Opening Cash",
            "players.details.transaction.opening_cash_with_amount" => "Opening Cash ({0})",
            "players.details.transaction.cash_in" => "Cash In",
            "players.details.transaction.cash_out" => "Cash Out",
            "players.details.transaction.category.gold_trade" => "Gold Trade",
            _ => key
        };
    }
}
