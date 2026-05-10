using Cashflowpoly.Ui.Infrastructure;
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
}
