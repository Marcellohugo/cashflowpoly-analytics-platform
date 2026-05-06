// Fungsi file: Menguji builder payload chart metrik pemain tanpa bergantung pada Razor/browser.
using System.Text.Json;
using Cashflowpoly.Ui.Infrastructure;
using Xunit;

namespace Cashflowpoly.Ui.Tests;

public sealed class PlayerMetricChartPayloadBuilderTests
{
    [Fact]
    public void BuildMergedRowChart_UsesPreferredScalarRowsBeforeNestedRows()
    {
        var chart = PlayerMetricChartPayloadBuilder.BuildMergedRowChart(
            "Financial",
            [("coins_net_end_game", "22"), ("coins.history.total", "30"), ("notes[0]", "skip")],
            isRawDomain: true,
            Translate);

        Assert.NotNull(chart);
        Assert.Equal("Financial: Combined Snapshot", chart.Value.Title);

        using var doc = JsonDocument.Parse(chart.Value.Json);
        var root = doc.RootElement;
        Assert.Equal("bar", root.GetProperty("chartType").GetString());
        Assert.Equal("Ending Coins", root.GetProperty("labels")[0].GetString());
        Assert.Equal("coins_net_end_game", root.GetProperty("keys")[0].GetString());
        Assert.Equal(22, root.GetProperty("series")[0].GetProperty("values")[0].GetDouble());
    }

    [Fact]
    public void BuildLineChartPayload_BuildsSeriesFromNumericArray()
    {
        using var doc = JsonDocument.Parse("[10,12]");

        var chart = PlayerMetricChartPayloadBuilder.BuildLineChartPayload(
            "coins.coins_per_turn_progression",
            doc.RootElement,
            isRawDomain: true,
            Translate);

        Assert.NotNull(chart);
        Assert.Equal("Coins: Coins per turn progression", chart.Value.Title);

        using var payload = JsonDocument.Parse(chart.Value.Json);
        var root = payload.RootElement;
        Assert.Equal("1", root.GetProperty("labels")[0].GetString());
        Assert.Equal("2", root.GetProperty("labels")[1].GetString());
        Assert.Equal("Value", root.GetProperty("series")[0].GetProperty("name").GetString());
        Assert.Equal(10, root.GetProperty("series")[0].GetProperty("values")[0].GetDouble());
        Assert.Equal(12, root.GetProperty("series")[0].GetProperty("values")[1].GetDouble());
    }

    [Fact]
    public void BuildLineChartsForGroup_ReturnsOnlyRenderableChildren()
    {
        using var doc = JsonDocument.Parse("""{"trend":[1,2],"text":"ignored"}""");

        var charts = PlayerMetricChartPayloadBuilder.BuildLineChartsForGroup(
            "coins",
            doc.RootElement,
            isRawDomain: false,
            Translate);

        Assert.Single(charts);
        Assert.Equal("Coins: Trend", charts[0].Title);
    }

    private static string Translate(string key)
    {
        return key switch
        {
            "common.value" => "Value",
            "players.details.series" => "Series",
            "players.details.metric_fallback" => "Metric",
            "players.details.item" => "Item",
            "players.details.raw_title" => "Raw Metrics",
            "players.details.derived_title" => "Derived Metrics",
            "players.details.combined_snapshot" => "Combined Snapshot",
            "players.details.source_calc_label" => "Data Source",
            "players.details.source_calc_template" => "{0}. Details: {1}",
            "players.details.source_raw_summary" => "Raw summary",
            "players.details.source_derived_summary" => "Derived summary",
            "players.details.source_line_raw_template" => "{0}.",
            "players.details.source_line_derived_template" => "{0}. Derived.",
            "players.details.formula.coins_net_end" => "Starting + in - out.",
            "players.raw.coins" => "Coins",
            "players.raw.coins_net_end_game" => "Ending Coins",
            "players.raw.trend" => "Trend",
            _ => key
        };
    }
}
