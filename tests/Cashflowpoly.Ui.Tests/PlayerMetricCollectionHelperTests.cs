// Fungsi file: Memverifikasi perilaku, lokalisasi, atau tata letak UI melalui PlayerMetricCollectionHelperTests.
using Cashflowpoly.Ui.Infrastructure;
using System.Globalization;
using Xunit;

namespace Cashflowpoly.Ui.Tests;

public sealed class PlayerMetricCollectionHelperTests
{
    [Fact]
    public void MergeUniqueRows_KeepsFirstPathAndSkipsBlankPath()
    {
        var rows = PlayerMetricCollectionHelper.MergeUniqueRows(
            [("cash", "10"), ("", "ignored")],
            [("cash", "20"), ("gold", "2")]);

        Assert.Equal(2, rows.Count);
        Assert.Equal(("cash", "10"), rows[0]);
        Assert.Equal(("gold", "2"), rows[1]);
    }

    [Fact]
    public void GetGroupRows_CombinesRequestedGroupsInOrder()
    {
        var source = new Dictionary<string, List<(string Path, string Value)>>(StringComparer.OrdinalIgnoreCase)
        {
            ["coins"] = [("cash", "10")],
            ["gold"] = [("qty", "2")]
        };

        var rows = PlayerMetricCollectionHelper.GetGroupRows(source, "gold", "missing", "coins");

        Assert.Equal([("qty", "2"), ("cash", "10")], rows);
    }

    [Fact]
    public void MergeUniqueCharts_KeepsFirstTitleAndSkipsBlankTitle()
    {
        var charts = PlayerMetricCollectionHelper.MergeUniqueCharts(
            [("Chart A", "{}"), ("", "{}")],
            [("Chart A", "{\"duplicate\":true}"), ("Chart B", "{}")]);

        Assert.Equal(2, charts.Count);
        Assert.Equal(("Chart A", "{}"), charts[0]);
        Assert.Equal(("Chart B", "{}"), charts[1]);
    }

    [Fact]
    public void FilterRowsAndChartsByKeywords_UsesCaseInsensitiveContains()
    {
        var rows = PlayerMetricCollectionHelper.FilterRowsByKeywords(
            [("gold_roi", "10"), ("cashflow_net", "4")],
            "GOLD");
        var charts = PlayerMetricCollectionHelper.FilterChartsByKeywords(
            [("Gold ROI", "{}"), ("Cashflow", "{}")],
            "gold");

        Assert.Equal([("gold_roi", "10")], rows);
        Assert.Equal([("Gold ROI", "{}")], charts);
    }

    [Fact]
    public void BuildActualCalculation_SubstitutesPlayerNumbersAndDisplayedResult()
    {
        var netWorth = PlayerMetricCollectionHelper.BuildActualCalculation(
            "net-worth",
            [("starting_coins", "10"), ("coins_net_end_game", "15")],
            "150",
            "%",
            "N/A",
            CultureInfo.InvariantCulture);
        var incomeDiversification = PlayerMetricCollectionHelper.BuildActualCalculation(
            "income-diversification",
            [
                ("active_income_source_count", "2"),
                ("income_shares.a", "0.75"),
                ("income_shares.b", "0.25")
            ],
            "75",
            "%",
            "N/A",
            CultureInfo.InvariantCulture);
        var happiness = PlayerMetricCollectionHelper.BuildActualCalculation(
            "happiness-portfolio",
            [
                ("need_card_points", "5"),
                ("need_set_bonus_points", "10"),
                ("donation_points", "3"),
                ("gold_points", "2"),
                ("pension_points", "1"),
                ("financial_goal_points", "4"),
                ("mission_penalty_points", "-6"),
                ("loan_penalty_points", "-2")
            ],
            "17",
            "points",
            "N/A",
            CultureInfo.InvariantCulture);
        var beginnerHappiness = PlayerMetricCollectionHelper.BuildActualCalculation(
            "happiness-portfolio-beginner",
            [
                ("need_card_points", "5"),
                ("need_set_bonus_points", "10"),
                ("donation_points", "3"),
                ("gold_points", "2"),
                ("pension_points", "1"),
                ("mission_penalty_points", "-6")
            ],
            "15",
            "points",
            "N/A",
            CultureInfo.InvariantCulture);
        var unavailable = PlayerMetricCollectionHelper.BuildActualCalculation(
            "expense-efficiency",
            [("ingredient_investment_coins_total", "0"), ("total_cash_out", "0")],
            "N/A",
            "%",
            "N/A",
            CultureInfo.InvariantCulture);

        Assert.Equal("15 ÷ 10 × 100% = 150%", netWorth);
        Assert.Equal("[1 − ((75 ÷ 100)² + (25 ÷ 100)²)] ÷ [1 − (1 ÷ 2)] × 100% = 75%", incomeDiversification);
        Assert.Equal("5 + 10 + 3 + 2 + 1 + 4 − 6 − 2 = 17 points", happiness);
        Assert.Equal("5 + 10 + 3 + 2 + 1 − 6 = 15 points", beginnerHappiness);
        Assert.Equal("N/A", unavailable);
    }

    [Fact]
    public void BuildActualCalculation_PreservesEnoughPrecisionToExplainRoundedResult()
    {
        var calculation = PlayerMetricCollectionHelper.BuildActualCalculation(
            "donation-commitment",
            [
                ("donation_stability_index", "29.28932188134524"),
                ("donated_resource_share", "0.272727272727"),
                ("friday_participation_rate", "1")
            ],
            "7.99",
            "score",
            "N/A",
            CultureInfo.InvariantCulture);

        Assert.Equal("min(100, max(0, 29.289322 × 0.272727 × 1)) = 7.99 score", calculation);
    }

    [Fact]
    public void BuildActualCalculation_CoversEveryPlayerAnalysisCard()
    {
        var rows = new (string Path, string Value)[]
        {
            ("coins_net_end_game", "15"),
            ("starting_coins", "10"),
            ("active_income_source_count", "2"),
            ("income_shares.a", "0.5"),
            ("income_shares.b", "0.5"),
            ("ingredient_investment_coins_total", "4"),
            ("total_cash_out", "8"),
            ("meal_order_income_total", "20"),
            ("ingredient_cost_used", "5"),
            ("risks_resolved_without_emergency", "1"),
            ("life_risk_cards_drawn", "2"),
            ("outstanding_loan", "2"),
            ("liquid_assets", "8"),
            ("coins_committed_to_goals", "5"),
            ("attempted_goal_target_total", "10"),
            ("income_main_actions", "4"),
            ("total_main_actions", "8"),
            ("ingredients_used_in_completed_orders", "3"),
            ("ingredients_collected", "4"),
            ("saving_actions", "1"),
            ("financial_goal_actions", "1"),
            ("insurance_actions", "1"),
            ("loan_repayment_actions", "1"),
            ("primary_need_share", "0.34"),
            ("secondary_need_share", "0.33"),
            ("tertiary_need_share", "0.33"),
            ("donation_stability_index", "80"),
            ("donated_resource_share", "0.2"),
            ("friday_participation_rate", "0.5"),
            ("need_card_points", "1"),
            ("need_set_bonus_points", "1"),
            ("donation_points", "1"),
            ("gold_points", "1"),
            ("pension_points", "1"),
            ("financial_goal_points", "1"),
            ("mission_penalty_points", "0"),
            ("loan_penalty_points", "0")
        };
        var analysisKeys = new[]
        {
            "net-worth",
            "income-diversification",
            "expense-efficiency",
            "business-margin",
            "risk-appetite",
            "debt-discipline",
            "goal-ambition",
            "action-efficiency",
            "meal-success",
            "planning-horizon",
            "fulfillment-diversity",
            "donation-commitment",
            "happiness-portfolio"
        };

        foreach (var analysisKey in analysisKeys)
        {
            var calculation = PlayerMetricCollectionHelper.BuildActualCalculation(
                analysisKey,
                rows,
                "50",
                "%",
                "N/A",
                CultureInfo.InvariantCulture);

            Assert.False(calculation.StartsWith("N/A", StringComparison.Ordinal));
            Assert.EndsWith("= 50%", calculation, StringComparison.Ordinal);
        }
    }
}
