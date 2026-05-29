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

    private static string Translate(string key)
    {
        return key switch
        {
            "players.details.metric_fallback" => "Metric",
            "players.details.item" => "Item",
            "players.details.series" => "Series",
            "common.value" => "Value",
            "players.raw.coins" => "Coins",
            "players.raw.coins_net_end_game" => "Ending Coins",
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
