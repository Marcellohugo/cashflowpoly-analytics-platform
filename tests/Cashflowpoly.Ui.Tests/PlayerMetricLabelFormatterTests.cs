// Fungsi file: Memverifikasi perilaku, lokalisasi, atau tata letak UI melalui PlayerMetricLabelFormatterTests.
using System.Globalization;
using Cashflowpoly.Ui.Infrastructure;
using Xunit;

namespace Cashflowpoly.Ui.Tests;

public sealed class PlayerMetricLabelFormatterTests
{
    [Theory]
    [InlineData("id", "5", "recorded")]
    [InlineData("id", "0", "zero")]
    [InlineData("id", "-10", "recorded")]
    [InlineData("id", "—", "unavailable")]
    [InlineData("en", "5", "recorded")]
    [InlineData("en", "—", "unavailable")]
    public void DescribeMetric_UsesHappinessPointLabelAndUnitWithoutChangingTheValue(
        string language, string value, string state)
    {
        var result = PlayerMetricLabelFormatter.DescribeMetric(
            "pension_fund_happiness_points", value, false, true, "—",
            key => UiText.Translate(language, key));

        Assert.Equal(language == "id" ? "Poin Kebahagiaan Dana Pensiun" : "Pension Happiness Points", result.Label);
        Assert.Equal(value, result.DisplayValue);
        Assert.Equal(state, result.State);
        Assert.Equal(state == "unavailable" ? string.Empty :
            language == "id" ? "poin kebahagiaan" : "happiness points", result.Unit);
    }

    [Fact]
    public void FormatMetricPathLabel_LocalizesKnownSegmentsAndArrayIndexes()
    {
        var label = PlayerMetricLabelFormatter.FormatMetricPathLabel(
            "coins.coins_net_end_game.history[1]",
            Translate);

        Assert.Equal("Coins - Ending Coins - History - Item 2", label);
    }

    [Theory]
    [InlineData("CoinsSpentPerTurn[0].Amount", "Outgoing Coins per Event - Item 1 - Amount")]
    [InlineData("ingredientTypesHeld.White Rice", "Ingredient Types Held - White Rice")]
    [InlineData("incomeDiversificationComponents.FreelanceIncome", "Income Diversification Components - Freelance Income")]
    public void FormatMetricPathLabel_NormalizesGameplayMetricKeysBeforeLocalization(string path, string expected)
    {
        var label = PlayerMetricLabelFormatter.FormatMetricPathLabel(path, Translate);

        Assert.Equal(expected, label);
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
    [InlineData("transaction_history.coins_in_event")]
    [InlineData("transaction_history.coins_out_event")]
    [InlineData("transaction_history.coin_change")]
    [InlineData("transaction_history.coin_balance_after_event")]
    public void DescribeMetric_UsesCoinUnitForComprehensiveTransactionColumns(string path)
    {
        var result = PlayerMetricLabelFormatter.DescribeMetric(
            path,
            "10",
            false,
            true,
            "Unavailable",
            key => key);

        Assert.Equal("players.support.unit.coins", result.Unit);
    }

    [Theory]
    [InlineData("active_income_source_count", "players.support.unit.sources")]
    [InlineData("income_main_actions", "players.support.unit.actions")]
    [InlineData("loan_repayment_actions", "players.support.unit.actions")]
    [InlineData("outstanding_loan", "players.support.unit.coins")]
    [InlineData("attempted_goal_target_total", "players.support.unit.coins")]
    [InlineData("risks_resolved_without_emergency", "players.support.unit.risk_events")]
    public void DescribeMetric_UsesThePhysicalUnitOfDerivedInputs(string path, string expectedUnit)
    {
        var result = PlayerMetricLabelFormatter.DescribeMetric(
            path,
            "3",
            true,
            true,
            "Unavailable",
            key => key);

        Assert.Equal(expectedUnit, result.Unit);
    }

    [Theory]
    [InlineData("coins_saved", false, "savings")]
    [InlineData("cash_in_total", false, "income")]
    [InlineData("coins_donated", false, "donation")]
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
    [InlineData("ingredients_used_per_meal", "ingredients_per_order")]
    [InlineData("meal_order_income_per_order", "income_per_order")]
    public void DescribeMetric_ExplainsPerOrderSeries(string path, string explanation)
    {
        var result = PlayerMetricLabelFormatter.DescribeMetric(
            path, "[2,3]", false, true, "Unavailable", key => key);

        Assert.Equal($"players.support.meaning.{explanation}", result.Explanation);
    }

    [Fact]
    public void DescribeMetric_ExplainsDiscardedIngredientsSeparatelyFromRemainingInventory()
    {
        var result = PlayerMetricLabelFormatter.DescribeMetric(
            "ingredients_wasted",
            "2",
            false,
            true,
            "Unavailable",
            key => key);

        Assert.Equal("players.support.meaning.ingredients_discarded", result.Explanation);
        Assert.Equal("players.support.guide.inventory", result.Guidance);
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

    [Fact]
    public void DescribeMetric_ExplainsWhatEmergencyActionsCount()
    {
        var result = PlayerMetricLabelFormatter.DescribeMetric(
            "emergency_options_used",
            "1",
            false,
            true,
            "Unavailable",
            key => key);

        Assert.Equal("players.support.meaning.emergency_actions", result.Explanation);
    }

    [Theory]
    [InlineData("meal_orders_per_turn_average", "players.support.meaning.orders_per_active_day")]
    [InlineData("gold_investment_net", "players.support.meaning.gold_cashflow")]
    [InlineData("ingredient_cards_value_end", "players.support.meaning.pension_ingredient_value")]
    [InlineData("life_risk_costs_per_card", "players.support.meaning.risk_nominal_cost")]
    [InlineData("life_risk_costs_total", "players.support.meaning.risk_nominal_cost")]
    public void DescribeMetric_UsesSpecificExplanationsForPotentiallyAmbiguousValues(
        string path,
        string expectedExplanation)
    {
        var result = PlayerMetricLabelFormatter.DescribeMetric(
            path,
            "2",
            false,
            true,
            "Unavailable",
            key => key);

        Assert.Equal(expectedExplanation, result.Explanation);
    }

    [Theory]
    [InlineData("day_when_debt_introduced", "0", "players.support.value.preparation_loan")]
    [InlineData("day_when_first_risk_hit", "2", "players.support.value.day_index")]
    [InlineData("day_game_completion", "25", "players.support.value.day_index")]
    public void DescribeMetric_FormatsGameDaysAsBoardPositions(
        string path,
        string value,
        string expectedValueKey)
    {
        var result = PlayerMetricLabelFormatter.DescribeMetric(
            path,
            value,
            false,
            true,
            "Unavailable",
            key => key);

        Assert.StartsWith(expectedValueKey, result.DisplayValue, StringComparison.Ordinal);
        Assert.Equal("recorded", result.State);
        Assert.Empty(result.Unit);
    }

    [Fact]
    public void DescribeMetric_TreatsAMissingLoanDayAsNoRecordedLoan()
    {
        var result = PlayerMetricLabelFormatter.DescribeMetric(
            "day_when_debt_introduced",
            "Unavailable",
            false,
            true,
            "Unavailable",
            key => key);

        Assert.Equal("players.support.value.no_loan_recorded", result.DisplayValue);
        Assert.Equal("recorded", result.State);
        Assert.Empty(result.Unit);
    }

    [Theory]
    [InlineData("id", "Donasi Setiap Jumat", "Jumlah Donasi", "koin")]
    [InlineData("en", "Donations Each Friday", "Donation Amount", "coins")]
    public void DescribeMetric_LocalizesCombinedDonationHistory(string language, string title, string amountLabel, string coins)
    {
        string Localize(string key) => UiText.Translate(language, key);
        var history = PlayerMetricLabelFormatter.DescribeMetric(
            "donation_history", "[{\"day_index\":5,\"amount\":1,\"rank\":4}]", false, true, "—", Localize);
        var amount = PlayerMetricLabelFormatter.DescribeMetric(
            "donation_history.amount", "1", false, true, "—", Localize);
        var empty = PlayerMetricLabelFormatter.DescribeMetric(
            "donation_history", "[]", false, true, "—", Localize);

        Assert.Equal(title, history.Label);
        Assert.StartsWith(title, history.Explanation, StringComparison.Ordinal);
        Assert.Contains("Tie Breaker", history.Explanation, StringComparison.Ordinal);
        Assert.Equal(amountLabel, Localize("players.details.donations.amount"));
        Assert.Equal(coins, amount.Unit);
        Assert.Equal("unavailable", empty.State);
        Assert.Empty(empty.Unit);
    }

    [Theory]
    [InlineData("id", "4", "ke-4", "recorded")]
    [InlineData("id", "1", "ke-1", "recorded")]
    [InlineData("en", "4", "Rank 4", "recorded")]
    [InlineData("id", "—", "—", "unavailable")]
    [InlineData("en", "—", "—", "unavailable")]
    public void DescribeMetric_FormatsPensionRankAsAPosition(string language, string value, string expected, string state)
    {
        var result = PlayerMetricLabelFormatter.DescribeMetric(
            "pension_fund_rank_per_game", value, false, true, "—", key => UiText.Translate(language, key));

        Assert.Equal(expected, result.DisplayValue);
        Assert.Equal(state, result.State);
        Assert.Empty(result.Unit);
    }

    [Theory]
    [InlineData("id", "financial_goals_completed", "1", "recorded", "Jumlah target finansial")]
    [InlineData("id", "financial_goals_completed", "0", "zero", "Jumlah target finansial")]
    [InlineData("en", "financial_goals_completed", "1", "recorded", "Number of financial goals")]
    [InlineData("id", "coins_saved", "3", "recorded", "Koin yang masih tersimpan")]
    [InlineData("en", "coins_saved", "3", "recorded", "Coins still saved")]
    [InlineData("id", "sharia_loans_outstanding_coins", "0", "zero", "Koin pinjaman yang masih harus dikembalikan")]
    [InlineData("id", "sharia_loans_outstanding_coins", "10", "recorded", "Koin pinjaman yang masih harus dikembalikan")]
    [InlineData("en", "sharia_loans_outstanding_coins", "0", "zero", "Borrowed coins still to be repaid")]
    [InlineData("id", "sharia_loans_outstanding_coins", "—", "unavailable", "Koin pinjaman yang masih harus dikembalikan")]
    [InlineData("id", "coins_saved", "—", "unavailable", "Koin yang masih tersimpan")]
    [InlineData("id", "financial_goals_completed", "—", "unavailable", "Jumlah target finansial")]
    public void DescribeMetric_ExplainsEssentialGoalValuesWithoutChangingOrInventingData(
        string language, string path, string value, string state, string explanation)
    {
        var result = PlayerMetricLabelFormatter.DescribeMetric(
            path, value, false, true, "—", key => UiText.Translate(language, key));

        Assert.Equal(value, result.DisplayValue);
        Assert.Equal(state, result.State);
        Assert.StartsWith(explanation, result.Explanation, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("id", true, "0")]
    [InlineData("id", false, "3")]
    [InlineData("en", true, "0")]
    [InlineData("en", false, "3")]
    [InlineData("id", true, "—")]
    public void DescribeMetric_DistinguishesNeedCardsOwnedFromPurchases(string language, bool advanced, string value)
    {
        string Localize(string key) => UiText.Translate(language, key);
        var owned = PlayerMetricLabelFormatter.DescribeMetric(
            "need_cards_owned_current", value, false, advanced, "—", Localize);
        var purchased = PlayerMetricLabelFormatter.DescribeMetric(
            "need_cards_purchased", "1", false, advanced, "—", Localize);

        Assert.Equal(value, owned.DisplayValue);
        Assert.StartsWith(Localize("players.support.meaning.need_cards_owned"), owned.Explanation, StringComparison.Ordinal);
        var saleNote = Localize("players.support.meaning.need_cards_sold_note");
        if (advanced)
            Assert.Contains(saleNote, owned.Explanation, StringComparison.Ordinal);
        else
            Assert.DoesNotContain(saleNote, owned.Explanation, StringComparison.Ordinal);
        Assert.Equal("1", purchased.DisplayValue);
        Assert.Equal(Localize("players.support.meaning.need_cards_purchased"), purchased.Explanation);
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
            "players.raw.coins_spent_per_turn" => "Outgoing Coins per Event",
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
