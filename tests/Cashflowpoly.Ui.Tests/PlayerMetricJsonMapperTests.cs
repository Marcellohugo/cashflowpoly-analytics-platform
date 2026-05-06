// Fungsi file: Menguji mapper JSON metrik pemain yang dipakai halaman detail pemain UI.
using System.Text.Json;
using Cashflowpoly.Ui.Infrastructure;
using Xunit;

namespace Cashflowpoly.Ui.Tests;

/// <summary>
/// Kelas pengujian unit untuk transformasi JSON metrik pemain menjadi grup dan baris tampilan.
/// </summary>
public sealed class PlayerMetricJsonMapperTests
{
    [Fact]
    /// <summary>
    /// Memvalidasi flatten JSON menghasilkan path daun untuk object, array, dan scalar.
    /// </summary>
    public void FlattenJsonLeaves_ReturnsLeafRowsForNestedJson()
    {
        using var doc = JsonDocument.Parse("""
            {
              "coins": { "start": 10, "history": [10, 12] },
              "active": true,
              "notes": null
            }
            """);

        var rows = PlayerMetricJsonMapper.FlattenJsonLeaves(
            doc.RootElement,
            trueText: "Benar",
            falseText: "Salah",
            nullText: "Kosong");

        Assert.Contains(rows, row => row.Path == "coins.start" && row.Value == "10");
        Assert.Contains(rows, row => row.Path == "coins.history[0]" && row.Value == "10");
        Assert.Contains(rows, row => row.Path == "coins.history[1]" && row.Value == "12");
        Assert.Contains(rows, row => row.Path == "active" && row.Value == "Benar");
        Assert.Contains(rows, row => row.Path == "notes" && row.Value == "Kosong");
    }

    [Fact]
    /// <summary>
    /// Memvalidasi grouping memisahkan scalar summary dari object/array group.
    /// </summary>
    public void BuildMetricGroups_SeparatesScalarSummaryAndNestedGroups()
    {
        using var doc = JsonDocument.Parse("""{"cash":5,"coins":{"start":10},"series":[1,2]}""");

        var groups = PlayerMetricJsonMapper.BuildMetricGroups(
            doc.RootElement,
            trueText: "true",
            falseText: "false",
            nullText: "null");

        Assert.Equal("summary", groups[0].GroupKey);
        Assert.Contains(groups[0].Rows, row => row.Path == "cash" && row.Value == "5");
        Assert.Contains(groups, group => group.GroupKey == "coins");
        Assert.Contains(groups, group => group.GroupKey == "series");
    }

    [Fact]
    /// <summary>
    /// Memvalidasi map elemen grup meng-clone JsonElement agar tetap valid setelah JsonDocument asal dispose.
    /// </summary>
    public void BuildMetricGroupElements_ClonesTopLevelElements()
    {
        Dictionary<string, JsonElement> map;
        using (var doc = JsonDocument.Parse("""{"coins":{"start":10}}"""))
        {
            map = PlayerMetricJsonMapper.BuildMetricGroupElements(doc.RootElement);
        }

        Assert.True(map.TryGetValue("coins", out var coins));
        Assert.Equal(10, coins.GetProperty("start").GetInt32());
    }

    [Theory]
    [InlineData("42", true, 42)]
    [InlineData("\"42.5\"", true, 42.5)]
    [InlineData("true", false, 0)]
    /// <summary>
    /// Memvalidasi pembacaan nilai numerik dari JsonElement.
    /// </summary>
    public void TryGetNumericValue_ParsesNumberAndNumericString(string json, bool expectedResult, double expectedValue)
    {
        using var doc = JsonDocument.Parse(json);

        var result = PlayerMetricJsonMapper.TryGetNumericValue(doc.RootElement, out var value);

        Assert.Equal(expectedResult, result);
        Assert.Equal(expectedValue, value);
    }
}
