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
            [("starting_coins", "10"), ("coins_held_current", "15")],
            "150",
            "%",
            "N/A",
            CultureInfo.InvariantCulture);
        var incomeDiversification = PlayerMetricCollectionHelper.BuildActualCalculation(
            "income-diversification",
            [
                ("N_active_income_sources", "2"),
                ("Income_Share_i.a", "0.75"),
                ("Income_Share_i.b", "0.25")
            ],
            "75",
            "%",
            "N/A",
            CultureInfo.InvariantCulture);
        var happiness = PlayerMetricCollectionHelper.BuildActualCalculation(
            "happiness-portfolio",
            [
                ("need_cards_pts", "5"),
                ("need_set_bonus_pts", "10"),
                ("donations_pts", "3"),
                ("gold_pts", "2"),
                ("pension_pts", "1"),
                ("financial_goals_pts", "4"),
                ("mission_bonus_pts", "-6"),
                ("loan_penalty_pts", "-2")
            ],
            "17",
            "points",
            "N/A",
            CultureInfo.InvariantCulture);
        var beginnerHappiness = PlayerMetricCollectionHelper.BuildActualCalculation(
            "happiness-portfolio-beginner",
            [
                ("need_cards_pts", "5"),
                ("need_set_bonus_pts", "10"),
                ("donations_pts", "3"),
                ("gold_pts", "2"),
                ("pension_pts", "1"),
                ("mission_bonus_pts", "-6")
            ],
            "15",
            "points",
            "N/A",
            CultureInfo.InvariantCulture);
        var unavailable = PlayerMetricCollectionHelper.BuildActualCalculation(
            "expense-efficiency",
            [("essential_expenses", "0"), ("total_expenses", "0")],
            "N/A",
            "%",
            "N/A",
            CultureInfo.InvariantCulture);

        Assert.Equal("15 ÷ 10 × 100% = 150%", netWorth);
        Assert.Equal("[1 − ((75 ÷ 100)² + (25 ÷ 100)²)] ÷ [1 − (1 ÷ 2)] × 100% = 75%", incomeDiversification);
        Assert.Equal("5 + 10 + 3 + 2 + 1 + 4 − 6 − 2 = 17 points", happiness);
        Assert.Equal("5 + 10 + 3 + 2 + 1 − 6 = 15 points", beginnerHappiness);
        Assert.EndsWith("= N/A", unavailable, StringComparison.Ordinal);
    }

    [Fact]
    public void BuildActualCalculation_CoversEveryPlayerAnalysisCard()
    {
        var rows = new (string Path, string Value)[]
        {
            ("coins_held_current", "15"),
            ("starting_coins", "10"),
            ("N_active_income_sources", "2"),
            ("Income_Share_i.a", "0.5"),
            ("Income_Share_i.b", "0.5"),
            ("essential_expenses", "4"),
            ("total_expenses", "8"),
            ("meal_order_income_total", "20"),
            ("ingredient_investment_coins_total", "5"),
            ("risk_acceptance_rate", "0.5"),
            ("Risk_Cost_Intensity", "0.4"),
            ("sharia_loans_outstanding_coins", "2"),
            ("Goal_Attempt_Rate", "0.5"),
            ("Goal_Investment_Rate", "0.5"),
            ("income_producing_actions", "4"),
            ("all_player_actions", "8"),
            ("meal_orders_claimed", "3"),
            ("meal_orders_available_passed", "1"),
            ("savings_actions", "1"),
            ("financial_goal_actions", "1"),
            ("insurance_premium_actions", "1"),
            ("p_primary", "0.34"),
            ("p_secondary", "0.33"),
            ("p_tertiary", "0.33"),
            ("donation_stability_index", "80"),
            ("donation_ratio", "0.2"),
            ("friday_participation_rate", "0.5"),
            ("need_cards_pts", "1"),
            ("need_set_bonus_pts", "1"),
            ("donations_pts", "1"),
            ("gold_pts", "1"),
            ("pension_pts", "1"),
            ("financial_goals_pts", "1"),
            ("mission_bonus_pts", "0"),
            ("loan_penalty_pts", "0")
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
