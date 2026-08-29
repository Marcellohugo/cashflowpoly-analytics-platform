// Fungsi file: Memverifikasi perilaku, lokalisasi, atau tata letak UI melalui PlayerMetricJsonMapperTests.
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
    public void BuildMetricVariableGroups_KeepsEachSeriesAsOneVariable()
    {
        using var doc = JsonDocument.Parse("""
            {
              "turns": { "coins_per_turn_progression": [10,12,9] },
              "needs": { "need_profile": { "basic_profile": true, "collector_profile": false, "specialist_profile": true } }
            }
            """);

        var groups = PlayerMetricJsonMapper.BuildMetricVariableGroups(
            doc.RootElement,
            trueText: "Ya",
            falseText: "Tidak",
            nullText: "—");

        var turns = Assert.Single(groups, group => group.GroupKey == "turns");
        var series = Assert.Single(turns.Rows);
        Assert.Equal("coins_per_turn_progression", series.Path);
        Assert.Equal("[10,12,9]", series.Value);

        var needs = Assert.Single(groups, group => group.GroupKey == "needs");
        Assert.Contains(needs.Rows, row => row.Path == "need_profile.basic_profile" && row.Value == "Ya");
        Assert.Contains(needs.Rows, row => row.Path == "need_profile.collector_profile" && row.Value == "Tidak");
        Assert.Contains(needs.Rows, row => row.Path == "need_profile.specialist_profile" && row.Value == "Ya");
    }

    [Fact]
    public void BuildCollectionTable_ConvertsObjectSeriesIntoReadableRows()
    {
        var table = PlayerMetricJsonMapper.BuildCollectionTable(
            """[{"action_slot":0,"amount":25},{"action_slot":1,"amount":56}]""",
            trueText: "Ya",
            falseText: "Tidak",
            nullText: "—");

        Assert.NotNull(table);
        Assert.Equal(new[] { "action_slot", "amount" }, table.Columns);
        Assert.Equal(new[] { "0", "25" }, table.Rows[0]);
        Assert.Equal(new[] { "1", "56" }, table.Rows[1]);
    }

    [Fact]
    public void BuildCollectionTable_AddsOneBasedOrderToPrimitiveSeries()
    {
        var table = PlayerMetricJsonMapper.BuildCollectionTable(
            """[5,7,8]""",
            trueText: "Ya",
            falseText: "Tidak",
            nullText: "—");

        Assert.NotNull(table);
        Assert.Equal(new[] { "item_index", "value" }, table.Columns);
        Assert.Equal(new[] { "1", "5" }, table.Rows[0]);
        Assert.Equal(new[] { "2", "7" }, table.Rows[1]);
        Assert.Equal(new[] { "3", "8" }, table.Rows[2]);
    }

    [Fact]
    public void BuildDonationHistoryJson_MatchesDaysInsteadOfArrayPositions()
    {
        var json = PlayerMetricJsonMapper.BuildDonationHistoryJson(
            """[{"day_index":19,"amount":1},{"day_index":5,"amount":1},{"day_index":12,"amount":4}]""",
            """[{"day_index":12,"rank":2},{"day_index":19,"rank":4},{"day_index":5,"rank":4}]""");

        var table = PlayerMetricJsonMapper.BuildCollectionTable(json, "Ya", "Tidak", "—");
        Assert.NotNull(table);
        Assert.Equal(new[] { "day_index", "amount", "rank" }, table.Columns);
        Assert.Equal(3, table.Rows.Count);
        Assert.Equal(new[] { "5", "1", "4" }, table.Rows[0]);
        Assert.Equal(new[] { "12", "4", "2" }, table.Rows[1]);
        Assert.Equal(new[] { "19", "1", "4" }, table.Rows[2]);
    }

    [Fact]
    public void BuildDonationHistoryJson_PreservesZeroAndMissingValues()
    {
        var json = PlayerMetricJsonMapper.BuildDonationHistoryJson(
            """[{"day_index":5,"amount":0},{"day_index":12,"amount":4}]""",
            """[{"day_index":12,"rank":null},{"day_index":19,"rank":2}]""");

        var table = PlayerMetricJsonMapper.BuildCollectionTable(json, "Ya", "Tidak", "—");
        Assert.NotNull(table);
        Assert.Equal(3, table.Rows.Count);
        Assert.Equal(new[] { "5", "0", "—" }, table.Rows[0]);
        Assert.Equal(new[] { "12", "4", "—" }, table.Rows[1]);
        Assert.Equal(new[] { "19", "—", "2" }, table.Rows[2]);
    }

    [Theory]
    [InlineData("[]")]
    [InlineData("null")]
    [InlineData("not json")]
    [InlineData("[{\"day_index\":null},{\"day_index\":\"5\"},{\"day_index\":0},{\"day_index\":5.5},false,{}]")]
    public void BuildDonationHistoryJson_HandlesUnavailableSeriesWithoutLosingTheOther(string unavailable)
    {
        const string amounts = """[{"day_index":5,"amount":1}]""";
        const string ranks = """[{"day_index":5,"rank":4}]""";
        var amountTable = PlayerMetricJsonMapper.BuildCollectionTable(
            PlayerMetricJsonMapper.BuildDonationHistoryJson(amounts, unavailable), "Ya", "Tidak", "—");
        var rankTable = PlayerMetricJsonMapper.BuildCollectionTable(
            PlayerMetricJsonMapper.BuildDonationHistoryJson(unavailable, ranks), "Ya", "Tidak", "—");

        Assert.NotNull(amountTable);
        Assert.NotNull(rankTable);
        Assert.Equal(new[] { "5", "1", "—" }, Assert.Single(amountTable.Rows));
        Assert.Equal(new[] { "5", "—", "4" }, Assert.Single(rankTable.Rows));
        Assert.Equal("[]", PlayerMetricJsonMapper.BuildDonationHistoryJson(unavailable, unavailable));
    }

    [Fact]
    public void BuildTransactionHistoryJson_MergesCashMovementAndBalanceByEventSequence()
    {
        var json = PlayerMetricJsonMapper.BuildTransactionHistoryJson(
            """[{"day_index":2,"action_slot":1,"sequence_number":35,"cashflow_category":"BUY","amount":4}]""",
            """[{"day_index":0,"action_slot":0,"sequence_number":0,"cashflow_category":"LOAN","amount":10}]""",
            """[{"day_index":0,"action_slot":0,"sequence_number":0,"cashflow_category":"LOAN","net":10},{"day_index":2,"action_slot":1,"sequence_number":35,"cashflow_category":"BUY","net":-4}]""",
            """[{"day_index":0,"action_slot":0,"sequence_number":0,"cashflow_category":"LOAN","coins":20},{"day_index":2,"action_slot":1,"sequence_number":35,"cashflow_category":"BUY","coins":16}]""");

        Assert.NotNull(json);
        var table = PlayerMetricJsonMapper.BuildCollectionTable(json, "Ya", "Tidak", "—");
        Assert.NotNull(table);
        Assert.Equal(
            new[] { "day_index", "action_slot", "sequence_number", "cashflow_category", "coins_in_event", "coins_out_event", "coin_change", "coin_balance_after_event" },
            table.Columns);
        Assert.Equal(new[] { "0", "0", "0", "LOAN", "10", "0", "10", "20" }, table.Rows[0]);
        Assert.Equal(new[] { "2", "1", "35", "BUY", "0", "4", "-4", "16" }, table.Rows[1]);
    }

    [Fact]
    public void BuildActionUsageHistoryJson_MergesSequenceAndRepetitionByDay()
    {
        var json = PlayerMetricJsonMapper.BuildActionUsageHistoryJson(
            """[{"day_index":2,"actions":["BahanMasakan","PesananMakanan"]},{"day_index":1,"actions":["KerjaLepas","KerjaLepas"]}]""",
            """[{"day_index":1,"total_actions":2,"distinct_actions":1,"repeated_actions":1,"diversity_score":0.5},{"day_index":2,"total_actions":2,"distinct_actions":2,"repeated_actions":0,"diversity_score":1}]""");

        Assert.NotNull(json);
        var table = PlayerMetricJsonMapper.BuildCollectionTable(json, "Ya", "Tidak", "—");
        Assert.NotNull(table);
        Assert.Equal(
            new[] { "day_index", "actions", "total_actions", "distinct_actions", "repeated_actions", "diversity_score" },
            table.Columns);
        Assert.Equal(new[] { "1", "KerjaLepas, KerjaLepas", "2", "1", "1", "0.5" }, table.Rows[0]);
        Assert.Equal(new[] { "2", "BahanMasakan, PesananMakanan", "2", "2", "0", "1" }, table.Rows[1]);
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
