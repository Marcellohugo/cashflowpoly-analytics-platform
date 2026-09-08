// Fungsi file: Memverifikasi perilaku, lokalisasi, atau tata letak UI melalui PlayerMetricJsonMapperTests.
// Mengimpor namespace `System.Text.Json` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.Json;
// Mengimpor namespace `Cashflowpoly.Ui.Infrastructure` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Ui.Infrastructure;
// Mengimpor namespace `Xunit` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Xunit;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Tests` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Tests;

/// <summary>
/// Kelas pengujian unit untuk transformasi JSON metrik pemain menjadi grup dan baris tampilan.
/// </summary>
// Mendefinisikan tipe class `PlayerMetricJsonMapperTests`; sealed mencegah tipe ini diturunkan lagi.
public sealed class PlayerMetricJsonMapperTests
// Membuka scope tipe PlayerMetricJsonMapperTests; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    /// <summary>
    /// Memvalidasi flatten JSON menghasilkan path daun untuk object, array, dan scalar.
    /// </summary>
    // Mendefinisikan metode `FlattenJsonLeaves_ReturnsLeafRowsForNestedJson` dengan hasil bertipe `void`; operasi ini menangani flatten JSON leaves
    // returns leaf baris untuk nested JSON.
    public void FlattenJsonLeaves_ReturnsLeafRowsForNestedJson()
    // Membuka scope metode FlattenJsonLeaves_ReturnsLeafRowsForNestedJson; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // FlattenJsonLeaves_ReturnsLeafRowsForNestedJson.
    {
        // Menyiapkan variabel lokal `doc` untuk nilai doc dengan memanggil `JsonDocument.Parse` dengan `””” { ”coins”: { ”start”: 10, ”history”: [10, 12]
        // }, ”active”: true, ”notes”: null } ”””`. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat
        // scope berakhir.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `using var doc =
        // JsonDocument.Parse(”””`.
        // Baris literal 2: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `{`.
        // Baris literal 3: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”coins”: { ”start”: 10, ”history”: [10, 12]
        // },`.
        // Baris literal 4: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”active”: true,`.
        // Baris literal 5: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”notes”: null`.
        // Baris literal 6: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `}`.
        // Baris literal 7: Pembatas literal/penutup `”””);`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        using var doc = JsonDocument.Parse("""
            {
              "coins": { "start": 10, "history": [10, 12] },
              "active": true,
              "notes": null
            }
            """);

        // Menyiapkan variabel lokal `rows` untuk nilai baris dengan memanggil `PlayerMetricJsonMapper.FlattenJsonLeaves` dengan `doc.RootElement`,
        // `”Benar”`, `”Salah”`, `”Kosong”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var rows = PlayerMetricJsonMapper.FlattenJsonLeaves(
            // Meneruskan `doc.RootElement` (nilai root element) sebagai argumen ke `PlayerMetricJsonMapper.FlattenJsonLeaves`.
            doc.RootElement,
            // Meneruskan nilai literal `”Benar”` sebagai argumen bernama `trueText`.
            trueText: "Benar",
            // Meneruskan nilai literal `”Salah”` sebagai argumen bernama `falseText`.
            falseText: "Salah",
            // Meneruskan nilai literal `”Kosong”` sebagai argumen bernama `nullText`.
            nullText: "Kosong");

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `rows`, `row => row.Path == ”coins.start”
        // && row.Value == ”10”` dalam FlattenJsonLeaves_ReturnsLeafRowsForNestedJson.
        Assert.Contains(rows, row => row.Path == "coins.start" && row.Value == "10");
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `rows`, `row => row.Path ==
        // ”coins.history[0]” && row.Value == ”10”` dalam FlattenJsonLeaves_ReturnsLeafRowsForNestedJson.
        Assert.Contains(rows, row => row.Path == "coins.history[0]" && row.Value == "10");
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `rows`, `row => row.Path ==
        // ”coins.history[1]” && row.Value == ”12”` dalam FlattenJsonLeaves_ReturnsLeafRowsForNestedJson.
        Assert.Contains(rows, row => row.Path == "coins.history[1]" && row.Value == "12");
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `rows`, `row => row.Path == ”active” &&
        // row.Value == ”Benar”` dalam FlattenJsonLeaves_ReturnsLeafRowsForNestedJson.
        Assert.Contains(rows, row => row.Path == "active" && row.Value == "Benar");
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `rows`, `row => row.Path == ”notes” &&
        // row.Value == ”Kosong”` dalam FlattenJsonLeaves_ReturnsLeafRowsForNestedJson.
        Assert.Contains(rows, row => row.Path == "notes" && row.Value == "Kosong");
    // Menutup scope metode FlattenJsonLeaves_ReturnsLeafRowsForNestedJson; bagian berikut berada di luar batas blok tersebut dalam
    // FlattenJsonLeaves_ReturnsLeafRowsForNestedJson.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    /// <summary>
    /// Memvalidasi grouping memisahkan scalar summary dari object/array group.
    /// </summary>
    // Mendefinisikan metode `BuildMetricGroups_SeparatesScalarSummaryAndNestedGroups` dengan hasil bertipe `void`; operasi ini menangani build metric
    // groups separates scalar summary dan nested groups.
    public void BuildMetricGroups_SeparatesScalarSummaryAndNestedGroups()
    // Membuka scope metode BuildMetricGroups_SeparatesScalarSummaryAndNestedGroups; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // BuildMetricGroups_SeparatesScalarSummaryAndNestedGroups.
    {
        // Menyiapkan variabel lokal `doc` untuk nilai doc dengan memanggil `JsonDocument.Parse` dengan
        // `”””{”cash”:5,”coins”:{”start”:10},”series”:[1,2]}”””`. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas
        // otomatis saat scope berakhir.
        using var doc = JsonDocument.Parse("""{"cash":5,"coins":{"start":10},"series":[1,2]}""");

        // Menyiapkan variabel lokal `groups` untuk nilai groups dengan memanggil `PlayerMetricJsonMapper.BuildMetricGroups` dengan `doc.RootElement`,
        // `”true”`, `”false”`, `”null”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var groups = PlayerMetricJsonMapper.BuildMetricGroups(
            // Meneruskan `doc.RootElement` (nilai root element) sebagai argumen ke `PlayerMetricJsonMapper.BuildMetricGroups`.
            doc.RootElement,
            // Meneruskan nilai literal `”true”` sebagai argumen bernama `trueText`.
            trueText: "true",
            // Meneruskan nilai literal `”false”` sebagai argumen bernama `falseText`.
            falseText: "false",
            // Meneruskan nilai literal `”null”` sebagai argumen bernama `nullText`.
            nullText: "null");

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”summary”`, `groups[0].GroupKey`); pengujian
        // gagal jika keduanya berbeda dalam BuildMetricGroups_SeparatesScalarSummaryAndNestedGroups.
        Assert.Equal("summary", groups[0].GroupKey);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `groups[0].Rows`, `row => row.Path ==
        // ”cash” && row.Value == ”5”` dalam BuildMetricGroups_SeparatesScalarSummaryAndNestedGroups.
        Assert.Contains(groups[0].Rows, row => row.Path == "cash" && row.Value == "5");
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `groups`, `group => group.GroupKey ==
        // ”coins”` dalam BuildMetricGroups_SeparatesScalarSummaryAndNestedGroups.
        Assert.Contains(groups, group => group.GroupKey == "coins");
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `groups`, `group => group.GroupKey ==
        // ”series”` dalam BuildMetricGroups_SeparatesScalarSummaryAndNestedGroups.
        Assert.Contains(groups, group => group.GroupKey == "series");
    // Menutup scope metode BuildMetricGroups_SeparatesScalarSummaryAndNestedGroups; bagian berikut berada di luar batas blok tersebut dalam
    // BuildMetricGroups_SeparatesScalarSummaryAndNestedGroups.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `BuildMetricVariableGroups_KeepsEachSeriesAsOneVariable` dengan hasil bertipe `void`; operasi ini menangani build metric
    // variable groups keeps each series as one variable.
    public void BuildMetricVariableGroups_KeepsEachSeriesAsOneVariable()
    // Membuka scope metode BuildMetricVariableGroups_KeepsEachSeriesAsOneVariable; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // BuildMetricVariableGroups_KeepsEachSeriesAsOneVariable.
    {
        // Menyiapkan variabel lokal `doc` untuk nilai doc dengan memanggil `JsonDocument.Parse` dengan `””” { ”turns”: { ”coins_per_turn_progression”:
        // [10,12,9] }, ”needs”: { ”need_profile”: { ”basic_profile”: true, ”collector_profile”: false, ”specialist_profile”: true } } } ””...`. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `using var doc =
        // JsonDocument.Parse(”””`.
        // Baris literal 2: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `{`.
        // Baris literal 3: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”turns”: { ”coins_per_turn_progression”:
        // [10,12,9] },`.
        // Baris literal 4: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”needs”: { ”need_profile”: {
        // ”basic_profile”: true, ”collector_profile”: false, ”specialist_profile”: true } }`.
        // Baris literal 5: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `}`.
        // Baris literal 6: Pembatas literal/penutup `”””);`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        using var doc = JsonDocument.Parse("""
            {
              "turns": { "coins_per_turn_progression": [10,12,9] },
              "needs": { "need_profile": { "basic_profile": true, "collector_profile": false, "specialist_profile": true } }
            }
            """);

        // Menyiapkan variabel lokal `groups` untuk nilai groups dengan memanggil `PlayerMetricJsonMapper.BuildMetricVariableGroups` dengan
        // `doc.RootElement`, `”Ya”`, `”Tidak”`, `”—”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var groups = PlayerMetricJsonMapper.BuildMetricVariableGroups(
            // Meneruskan `doc.RootElement` (nilai root element) sebagai argumen ke `PlayerMetricJsonMapper.BuildMetricVariableGroups`.
            doc.RootElement,
            // Meneruskan nilai literal `”Ya”` sebagai argumen bernama `trueText`.
            trueText: "Ya",
            // Meneruskan nilai literal `”Tidak”` sebagai argumen bernama `falseText`.
            falseText: "Tidak",
            // Meneruskan nilai literal `”—”` sebagai argumen bernama `nullText`.
            nullText: "—");

        // Menyiapkan variabel lokal `turns` untuk nilai turns dengan pemeriksaan hasil dengan `Assert.Single` menggunakan `groups`, `group =>
        // group.GroupKey == ”turns”`; ketidaksesuaian dengan ekspektasi membuat pengujian gagal. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var turns = Assert.Single(groups, group => group.GroupKey == "turns");
        // Menyiapkan variabel lokal `series` untuk nilai series dengan pemeriksaan hasil dengan `Assert.Single` menggunakan `turns.Rows`; ketidaksesuaian
        // dengan ekspektasi membuat pengujian gagal. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var series = Assert.Single(turns.Rows);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”coins_per_turn_progression”`, `series.Path`);
        // pengujian gagal jika keduanya berbeda dalam BuildMetricVariableGroups_KeepsEachSeriesAsOneVariable.
        Assert.Equal("coins_per_turn_progression", series.Path);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”[10,12,9]”`, `series.Value`); pengujian gagal
        // jika keduanya berbeda dalam BuildMetricVariableGroups_KeepsEachSeriesAsOneVariable.
        Assert.Equal("[10,12,9]", series.Value);

        // Menyiapkan variabel lokal `needs` untuk nilai kebutuhan dengan pemeriksaan hasil dengan `Assert.Single` menggunakan `groups`, `group =>
        // group.GroupKey == ”needs”`; ketidaksesuaian dengan ekspektasi membuat pengujian gagal. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var needs = Assert.Single(groups, group => group.GroupKey == "needs");
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `needs.Rows`, `row => row.Path ==
        // ”need_profile.basic_profile” && row.Value == ”Ya”` dalam BuildMetricVariableGroups_KeepsEachSeriesAsOneVariable.
        Assert.Contains(needs.Rows, row => row.Path == "need_profile.basic_profile" && row.Value == "Ya");
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `needs.Rows`, `row => row.Path ==
        // ”need_profile.collector_profile” && row.Value == ”Tidak”` dalam BuildMetricVariableGroups_KeepsEachSeriesAsOneVariable.
        Assert.Contains(needs.Rows, row => row.Path == "need_profile.collector_profile" && row.Value == "Tidak");
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `needs.Rows`, `row => row.Path ==
        // ”need_profile.specialist_profile” && row.Value == ”Ya”` dalam BuildMetricVariableGroups_KeepsEachSeriesAsOneVariable.
        Assert.Contains(needs.Rows, row => row.Path == "need_profile.specialist_profile" && row.Value == "Ya");
    // Menutup scope metode BuildMetricVariableGroups_KeepsEachSeriesAsOneVariable; bagian berikut berada di luar batas blok tersebut dalam
    // BuildMetricVariableGroups_KeepsEachSeriesAsOneVariable.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `BuildCollectionTable_ConvertsObjectSeriesIntoReadableRows` dengan hasil bertipe `void`; operasi ini menangani build
    // collection table converts object series into readable baris.
    public void BuildCollectionTable_ConvertsObjectSeriesIntoReadableRows()
    // Membuka scope metode BuildCollectionTable_ConvertsObjectSeriesIntoReadableRows; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // BuildCollectionTable_ConvertsObjectSeriesIntoReadableRows.
    {
        // Menyiapkan variabel lokal `table` untuk nilai table dengan memanggil `PlayerMetricJsonMapper.BuildCollectionTable` dengan
        // `”””[{”action_slot”:0,”amount”:25},{”action_slot”:1,”amount”:56}]”””`, `”Ya”`, `”Tidak”`, `”—”`. Tipe variabel disimpulkan dari ekspresi nilai
        // awal.
        var table = PlayerMetricJsonMapper.BuildCollectionTable(
            // Meneruskan nilai literal `”””[{”action_slot”:0,”amount”:25},{”action_slot”:1,”amount”:56}]”””` sebagai argumen ke
            // `PlayerMetricJsonMapper.BuildCollectionTable`.
            """[{"action_slot":0,"amount":25},{"action_slot":1,"amount":56}]""",
            // Meneruskan nilai literal `”Ya”` sebagai argumen bernama `trueText`.
            trueText: "Ya",
            // Meneruskan nilai literal `”Tidak”` sebagai argumen bernama `falseText`.
            falseText: "Tidak",
            // Meneruskan nilai literal `”—”` sebagai argumen bernama `nullText`.
            nullText: "—");

        // Menjalankan pemeriksaan NotNull atas `table` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // BuildCollectionTable_ConvertsObjectSeriesIntoReadableRows.
        Assert.NotNull(table);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`new[] { ”action_slot”, ”amount” }`,
        // `table.Columns`); pengujian gagal jika keduanya berbeda dalam BuildCollectionTable_ConvertsObjectSeriesIntoReadableRows.
        Assert.Equal(new[] { "action_slot", "amount" }, table.Columns);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`new[] { ”0”, ”25” }`, `table.Rows[0]`);
        // pengujian gagal jika keduanya berbeda dalam BuildCollectionTable_ConvertsObjectSeriesIntoReadableRows.
        Assert.Equal(new[] { "0", "25" }, table.Rows[0]);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`new[] { ”1”, ”56” }`, `table.Rows[1]`);
        // pengujian gagal jika keduanya berbeda dalam BuildCollectionTable_ConvertsObjectSeriesIntoReadableRows.
        Assert.Equal(new[] { "1", "56" }, table.Rows[1]);
    // Menutup scope metode BuildCollectionTable_ConvertsObjectSeriesIntoReadableRows; bagian berikut berada di luar batas blok tersebut dalam
    // BuildCollectionTable_ConvertsObjectSeriesIntoReadableRows.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `BuildCollectionTable_AddsOneBasedOrderToPrimitiveSeries` dengan hasil bertipe `void`; operasi ini menangani build
    // collection table adds one based urutan/pesanan ke primitive series.
    public void BuildCollectionTable_AddsOneBasedOrderToPrimitiveSeries()
    // Membuka scope metode BuildCollectionTable_AddsOneBasedOrderToPrimitiveSeries; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // BuildCollectionTable_AddsOneBasedOrderToPrimitiveSeries.
    {
        // Menyiapkan variabel lokal `table` untuk nilai table dengan memanggil `PlayerMetricJsonMapper.BuildCollectionTable` dengan `”””[5,7,8]”””`,
        // `”Ya”`, `”Tidak”`, `”—”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var table = PlayerMetricJsonMapper.BuildCollectionTable(
            // Meneruskan nilai literal `”””[5,7,8]”””` sebagai argumen ke `PlayerMetricJsonMapper.BuildCollectionTable`.
            """[5,7,8]""",
            // Meneruskan nilai literal `”Ya”` sebagai argumen bernama `trueText`.
            trueText: "Ya",
            // Meneruskan nilai literal `”Tidak”` sebagai argumen bernama `falseText`.
            falseText: "Tidak",
            // Meneruskan nilai literal `”—”` sebagai argumen bernama `nullText`.
            nullText: "—");

        // Menjalankan pemeriksaan NotNull atas `table` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // BuildCollectionTable_AddsOneBasedOrderToPrimitiveSeries.
        Assert.NotNull(table);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`new[] { ”item_index”, ”value” }`,
        // `table.Columns`); pengujian gagal jika keduanya berbeda dalam BuildCollectionTable_AddsOneBasedOrderToPrimitiveSeries.
        Assert.Equal(new[] { "item_index", "value" }, table.Columns);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`new[] { ”1”, ”5” }`, `table.Rows[0]`);
        // pengujian gagal jika keduanya berbeda dalam BuildCollectionTable_AddsOneBasedOrderToPrimitiveSeries.
        Assert.Equal(new[] { "1", "5" }, table.Rows[0]);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`new[] { ”2”, ”7” }`, `table.Rows[1]`);
        // pengujian gagal jika keduanya berbeda dalam BuildCollectionTable_AddsOneBasedOrderToPrimitiveSeries.
        Assert.Equal(new[] { "2", "7" }, table.Rows[1]);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`new[] { ”3”, ”8” }`, `table.Rows[2]`);
        // pengujian gagal jika keduanya berbeda dalam BuildCollectionTable_AddsOneBasedOrderToPrimitiveSeries.
        Assert.Equal(new[] { "3", "8" }, table.Rows[2]);
    // Menutup scope metode BuildCollectionTable_AddsOneBasedOrderToPrimitiveSeries; bagian berikut berada di luar batas blok tersebut dalam
    // BuildCollectionTable_AddsOneBasedOrderToPrimitiveSeries.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `BuildDonationHistoryJson_MatchesDaysInsteadOfArrayPositions` dengan hasil bertipe `void`; operasi ini menangani build
    // donasi history JSON matches hari instead of array positions.
    public void BuildDonationHistoryJson_MatchesDaysInsteadOfArrayPositions()
    // Membuka scope metode BuildDonationHistoryJson_MatchesDaysInsteadOfArrayPositions; pernyataan/deklarasi berikut berada di dalam batas blok ini
    // dalam BuildDonationHistoryJson_MatchesDaysInsteadOfArrayPositions.
    {
        // Menyiapkan variabel lokal `json` untuk nilai JSON dengan memanggil `PlayerMetricJsonMapper.BuildDonationHistoryJson` dengan
        // `”””[{”day_index”:19,”amount”:1},{”day_index”:5,”amount”:1},{”day_index”:12,”amount”:4}]”””`,
        // `”””[{”day_index”:12,”rank”:2},{”day_index”:19,”rank”:4},{”day_index”:5,”rank”:4}]”””`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var json = PlayerMetricJsonMapper.BuildDonationHistoryJson(
            // Meneruskan nilai literal `”””[{”day_index”:19,”amount”:1},{”day_index”:5,”amount”:1},{”day_index”:12,”amount”:4}]”””` sebagai argumen ke
            // `PlayerMetricJsonMapper.BuildDonationHistoryJson`.
            """[{"day_index":19,"amount":1},{"day_index":5,"amount":1},{"day_index":12,"amount":4}]""",
            // Meneruskan nilai literal `”””[{”day_index”:12,”rank”:2},{”day_index”:19,”rank”:4},{”day_index”:5,”rank”:4}]”””` sebagai argumen ke
            // `PlayerMetricJsonMapper.BuildDonationHistoryJson`.
            """[{"day_index":12,"rank":2},{"day_index":19,"rank":4},{"day_index":5,"rank":4}]""");

        // Menyiapkan variabel lokal `table` untuk nilai table dengan memanggil `PlayerMetricJsonMapper.BuildCollectionTable` dengan `json`, `”Ya”`,
        // `”Tidak”`, `”—”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var table = PlayerMetricJsonMapper.BuildCollectionTable(json, "Ya", "Tidak", "—");
        // Menjalankan pemeriksaan NotNull atas `table` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // BuildDonationHistoryJson_MatchesDaysInsteadOfArrayPositions.
        Assert.NotNull(table);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`new[] { ”day_index”, ”amount”, ”rank” }`,
        // `table.Columns`); pengujian gagal jika keduanya berbeda dalam BuildDonationHistoryJson_MatchesDaysInsteadOfArrayPositions.
        Assert.Equal(new[] { "day_index", "amount", "rank" }, table.Columns);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`3`, `table.Rows.Count`); pengujian gagal jika
        // keduanya berbeda dalam BuildDonationHistoryJson_MatchesDaysInsteadOfArrayPositions.
        Assert.Equal(3, table.Rows.Count);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`new[] { ”5”, ”1”, ”4” }`, `table.Rows[0]`);
        // pengujian gagal jika keduanya berbeda dalam BuildDonationHistoryJson_MatchesDaysInsteadOfArrayPositions.
        Assert.Equal(new[] { "5", "1", "4" }, table.Rows[0]);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`new[] { ”12”, ”4”, ”2” }`, `table.Rows[1]`);
        // pengujian gagal jika keduanya berbeda dalam BuildDonationHistoryJson_MatchesDaysInsteadOfArrayPositions.
        Assert.Equal(new[] { "12", "4", "2" }, table.Rows[1]);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`new[] { ”19”, ”1”, ”4” }`, `table.Rows[2]`);
        // pengujian gagal jika keduanya berbeda dalam BuildDonationHistoryJson_MatchesDaysInsteadOfArrayPositions.
        Assert.Equal(new[] { "19", "1", "4" }, table.Rows[2]);
    // Menutup scope metode BuildDonationHistoryJson_MatchesDaysInsteadOfArrayPositions; bagian berikut berada di luar batas blok tersebut dalam
    // BuildDonationHistoryJson_MatchesDaysInsteadOfArrayPositions.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `BuildDonationHistoryJson_PreservesZeroAndMissingValues` dengan hasil bertipe `void`; operasi ini menangani build donasi
    // history JSON preserves zero dan missing nilai.
    public void BuildDonationHistoryJson_PreservesZeroAndMissingValues()
    // Membuka scope metode BuildDonationHistoryJson_PreservesZeroAndMissingValues; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // BuildDonationHistoryJson_PreservesZeroAndMissingValues.
    {
        // Menyiapkan variabel lokal `json` untuk nilai JSON dengan memanggil `PlayerMetricJsonMapper.BuildDonationHistoryJson` dengan
        // `”””[{”day_index”:5,”amount”:0},{”day_index”:12,”amount”:4}]”””`, `”””[{”day_index”:12,”rank”:null},{”day_index”:19,”rank”:2}]”””`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var json = PlayerMetricJsonMapper.BuildDonationHistoryJson(
            // Meneruskan nilai literal `”””[{”day_index”:5,”amount”:0},{”day_index”:12,”amount”:4}]”””` sebagai argumen ke
            // `PlayerMetricJsonMapper.BuildDonationHistoryJson`.
            """[{"day_index":5,"amount":0},{"day_index":12,"amount":4}]""",
            // Meneruskan nilai literal `”””[{”day_index”:12,”rank”:null},{”day_index”:19,”rank”:2}]”””` sebagai argumen ke
            // `PlayerMetricJsonMapper.BuildDonationHistoryJson`.
            """[{"day_index":12,"rank":null},{"day_index":19,"rank":2}]""");

        // Menyiapkan variabel lokal `table` untuk nilai table dengan memanggil `PlayerMetricJsonMapper.BuildCollectionTable` dengan `json`, `”Ya”`,
        // `”Tidak”`, `”—”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var table = PlayerMetricJsonMapper.BuildCollectionTable(json, "Ya", "Tidak", "—");
        // Menjalankan pemeriksaan NotNull atas `table` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // BuildDonationHistoryJson_PreservesZeroAndMissingValues.
        Assert.NotNull(table);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`3`, `table.Rows.Count`); pengujian gagal jika
        // keduanya berbeda dalam BuildDonationHistoryJson_PreservesZeroAndMissingValues.
        Assert.Equal(3, table.Rows.Count);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`new[] { ”5”, ”0”, ”—” }`, `table.Rows[0]`);
        // pengujian gagal jika keduanya berbeda dalam BuildDonationHistoryJson_PreservesZeroAndMissingValues.
        Assert.Equal(new[] { "5", "0", "—" }, table.Rows[0]);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`new[] { ”12”, ”4”, ”—” }`, `table.Rows[1]`);
        // pengujian gagal jika keduanya berbeda dalam BuildDonationHistoryJson_PreservesZeroAndMissingValues.
        Assert.Equal(new[] { "12", "4", "—" }, table.Rows[1]);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`new[] { ”19”, ”—”, ”2” }`, `table.Rows[2]`);
        // pengujian gagal jika keduanya berbeda dalam BuildDonationHistoryJson_PreservesZeroAndMissingValues.
        Assert.Equal(new[] { "19", "—", "2" }, table.Rows[2]);
    // Menutup scope metode BuildDonationHistoryJson_PreservesZeroAndMissingValues; bagian berikut berada di luar batas blok tersebut dalam
    // BuildDonationHistoryJson_PreservesZeroAndMissingValues.
    }

    // menandai metode sebagai pengujian xUnit yang dijalankan untuk setiap kombinasi data.
    [Theory]
    // menyediakan satu kombinasi masukan pengujian (”[]”).
    [InlineData("[]")]
    // menyediakan satu kombinasi masukan pengujian (”null”).
    [InlineData("null")]
    // menyediakan satu kombinasi masukan pengujian (”not json”).
    [InlineData("not json")]
    // menyediakan satu kombinasi masukan pengujian (”[{\”day_index\”:null},{\”day_index\”:\”5\”},{\”day_index\”:0},{\”day_index\”:5.5},false,{}]”).
    [InlineData("[{\"day_index\":null},{\"day_index\":\"5\"},{\"day_index\":0},{\"day_index\":5.5},false,{}]")]
    // Mendefinisikan metode `BuildDonationHistoryJson_HandlesUnavailableSeriesWithoutLosingTheOther` dengan hasil bertipe `void`; operasi ini menangani
    // build donasi history JSON handles unavailable series tanpa losing the other. Masukan: Parameter `unavailable` bertipe `string` membawa nilai
    // unavailable.
    public void BuildDonationHistoryJson_HandlesUnavailableSeriesWithoutLosingTheOther(string unavailable)
    // Membuka scope metode BuildDonationHistoryJson_HandlesUnavailableSeriesWithoutLosingTheOther; pernyataan/deklarasi berikut berada di dalam batas
    // blok ini dalam BuildDonationHistoryJson_HandlesUnavailableSeriesWithoutLosingTheOther.
    {
        // Menyiapkan variabel lokal `amounts` untuk nilai amounts dengan nilai literal `”””[{”day_index”:5,”amount”:1}]”””`. Tipe yang dipakai adalah
        // `string`.
        const string amounts = """[{"day_index":5,"amount":1}]""";
        // Menyiapkan variabel lokal `ranks` untuk nilai ranks dengan nilai literal `”””[{”day_index”:5,”rank”:4}]”””`. Tipe yang dipakai adalah `string`.
        const string ranks = """[{"day_index":5,"rank":4}]""";
        // Menyiapkan variabel lokal `amountTable` untuk nilai nominal table dengan memanggil `PlayerMetricJsonMapper.BuildCollectionTable` dengan
        // `PlayerMetricJsonMapper.BuildDonationHistoryJson(amounts, unavailable)`, `”Ya”`, `”Tidak”`, `”—”`. Tipe variabel disimpulkan dari ekspresi nilai
        // awal.
        var amountTable = PlayerMetricJsonMapper.BuildCollectionTable(
            // Meneruskan memanggil `PlayerMetricJsonMapper.BuildDonationHistoryJson` dengan `amounts`, `unavailable` sebagai argumen ke
            // `PlayerMetricJsonMapper.BuildCollectionTable`; Meneruskan `amounts` (nilai amounts) sebagai argumen ke
            // `PlayerMetricJsonMapper.BuildDonationHistoryJson`; Meneruskan `unavailable` (nilai unavailable) sebagai argumen ke
            // `PlayerMetricJsonMapper.BuildDonationHistoryJson`; Meneruskan nilai literal `”Ya”` sebagai argumen ke
            // `PlayerMetricJsonMapper.BuildCollectionTable`; Meneruskan nilai literal `”Tidak”` sebagai argumen ke
            // `PlayerMetricJsonMapper.BuildCollectionTable`; Meneruskan nilai literal `”—”` sebagai argumen ke `PlayerMetricJsonMapper.BuildCollectionTable`.
            PlayerMetricJsonMapper.BuildDonationHistoryJson(amounts, unavailable), "Ya", "Tidak", "—");
        // Menyiapkan variabel lokal `rankTable` untuk nilai rank table dengan memanggil `PlayerMetricJsonMapper.BuildCollectionTable` dengan
        // `PlayerMetricJsonMapper.BuildDonationHistoryJson(unavailable, ranks)`, `”Ya”`, `”Tidak”`, `”—”`. Tipe variabel disimpulkan dari ekspresi nilai
        // awal.
        var rankTable = PlayerMetricJsonMapper.BuildCollectionTable(
            // Meneruskan memanggil `PlayerMetricJsonMapper.BuildDonationHistoryJson` dengan `unavailable`, `ranks` sebagai argumen ke
            // `PlayerMetricJsonMapper.BuildCollectionTable`; Meneruskan `unavailable` (nilai unavailable) sebagai argumen ke
            // `PlayerMetricJsonMapper.BuildDonationHistoryJson`; Meneruskan `ranks` (nilai ranks) sebagai argumen ke
            // `PlayerMetricJsonMapper.BuildDonationHistoryJson`; Meneruskan nilai literal `”Ya”` sebagai argumen ke
            // `PlayerMetricJsonMapper.BuildCollectionTable`; Meneruskan nilai literal `”Tidak”` sebagai argumen ke
            // `PlayerMetricJsonMapper.BuildCollectionTable`; Meneruskan nilai literal `”—”` sebagai argumen ke `PlayerMetricJsonMapper.BuildCollectionTable`.
            PlayerMetricJsonMapper.BuildDonationHistoryJson(unavailable, ranks), "Ya", "Tidak", "—");

        // Menjalankan pemeriksaan NotNull atas `amountTable` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // BuildDonationHistoryJson_HandlesUnavailableSeriesWithoutLosingTheOther.
        Assert.NotNull(amountTable);
        // Menjalankan pemeriksaan NotNull atas `rankTable` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // BuildDonationHistoryJson_HandlesUnavailableSeriesWithoutLosingTheOther.
        Assert.NotNull(rankTable);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`new[] { ”5”, ”1”, ”—” }`,
        // `Assert.Single(amountTable.Rows)`); pengujian gagal jika keduanya berbeda dalam
        // BuildDonationHistoryJson_HandlesUnavailableSeriesWithoutLosingTheOther.
        Assert.Equal(new[] { "5", "1", "—" }, Assert.Single(amountTable.Rows));
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`new[] { ”5”, ”—”, ”4” }`,
        // `Assert.Single(rankTable.Rows)`); pengujian gagal jika keduanya berbeda dalam
        // BuildDonationHistoryJson_HandlesUnavailableSeriesWithoutLosingTheOther.
        Assert.Equal(new[] { "5", "—", "4" }, Assert.Single(rankTable.Rows));
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”[]”`,
        // `PlayerMetricJsonMapper.BuildDonationHistoryJson(unavailable, unavailable)`); pengujian gagal jika keduanya berbeda dalam
        // BuildDonationHistoryJson_HandlesUnavailableSeriesWithoutLosingTheOther.
        Assert.Equal("[]", PlayerMetricJsonMapper.BuildDonationHistoryJson(unavailable, unavailable));
    // Menutup scope metode BuildDonationHistoryJson_HandlesUnavailableSeriesWithoutLosingTheOther; bagian berikut berada di luar batas blok tersebut
    // dalam BuildDonationHistoryJson_HandlesUnavailableSeriesWithoutLosingTheOther.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `BuildTransactionHistoryJson_MergesCashMovementAndBalanceByEventSequence` dengan hasil bertipe `void`; operasi ini
    // menangani build transaction history JSON merges uang tunai movement dan saldo berdasarkan event sequence.
    public void BuildTransactionHistoryJson_MergesCashMovementAndBalanceByEventSequence()
    // Membuka scope metode BuildTransactionHistoryJson_MergesCashMovementAndBalanceByEventSequence; pernyataan/deklarasi berikut berada di dalam batas
    // blok ini dalam BuildTransactionHistoryJson_MergesCashMovementAndBalanceByEventSequence.
    {
        // Menyiapkan variabel lokal `json` untuk nilai JSON dengan memanggil `PlayerMetricJsonMapper.BuildTransactionHistoryJson` dengan
        // `”””[{”day_index”:2,”action_slot”:1,”sequence_number”:35,”cashflow_category”:”BUY”,”amount”:4}]”””`,
        // `”””[{”day_index”:0,”action_slot”:0,”sequence_number”:0,”cashflow_category”:”LOAN”,”amount”:10}]”””`,
        // `”””[{”day_index”:0,”action_slot”:0,”sequence_number”:0,”cashflow_category”:”LOAN”,”net”:10},{”day_index”:2,”action_slot”:1,”sequence_number”:35,
        // ”cashflow_category”:”BUY”,”net”...`, `”””[{”day_index”:0,”action_slot”:0,”sequence_number”:0,”cashflow_category”:”LOAN”,”coins”:20},{”day_index”:
        // 2,”action_slot”:1,”sequence_number”:35,”cashflow_category”:”BUY”,”co...`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var json = PlayerMetricJsonMapper.BuildTransactionHistoryJson(
            // Meneruskan nilai literal `”””[{”day_index”:2,”action_slot”:1,”sequence_number”:35,”cashflow_category”:”BUY”,”amount”:4}]”””` sebagai argumen ke
            // `PlayerMetricJsonMapper.BuildTransactionHistoryJson`.
            """[{"day_index":2,"action_slot":1,"sequence_number":35,"cashflow_category":"BUY","amount":4}]""",
            // Meneruskan nilai literal `”””[{”day_index”:0,”action_slot”:0,”sequence_number”:0,”cashflow_category”:”LOAN”,”amount”:10}]”””` sebagai argumen ke
            // `PlayerMetricJsonMapper.BuildTransactionHistoryJson`.
            """[{"day_index":0,"action_slot":0,"sequence_number":0,"cashflow_category":"LOAN","amount":10}]""",
            // Meneruskan nilai literal `”””[{”day_index”:0,”action_slot”:0,”sequence_number”:0,”cashflow_category”:”LOAN”,”net”:10},{”day_index”:2,”action_slot
            // ”:1,”sequence_number”:35,”cashflow_category”:”BUY”,”net”...` sebagai argumen ke `PlayerMetricJsonMapper.BuildTransactionHistoryJson`.
            """[{"day_index":0,"action_slot":0,"sequence_number":0,"cashflow_category":"LOAN","net":10},{"day_index":2,"action_slot":1,"sequence_number":35,"cashflow_category":"BUY","net":-4}]""",
            // Meneruskan nilai literal `”””[{”day_index”:0,”action_slot”:0,”sequence_number”:0,”cashflow_category”:”LOAN”,”coins”:20},{”day_index”:2,”action_sl
            // ot”:1,”sequence_number”:35,”cashflow_category”:”BUY”,”co...` sebagai argumen ke `PlayerMetricJsonMapper.BuildTransactionHistoryJson`.
            """[{"day_index":0,"action_slot":0,"sequence_number":0,"cashflow_category":"LOAN","coins":20},{"day_index":2,"action_slot":1,"sequence_number":35,"cashflow_category":"BUY","coins":16}]""");

        // Menjalankan pemeriksaan NotNull atas `json` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // BuildTransactionHistoryJson_MergesCashMovementAndBalanceByEventSequence.
        Assert.NotNull(json);
        // Menyiapkan variabel lokal `table` untuk nilai table dengan memanggil `PlayerMetricJsonMapper.BuildCollectionTable` dengan `json`, `”Ya”`,
        // `”Tidak”`, `”—”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var table = PlayerMetricJsonMapper.BuildCollectionTable(json, "Ya", "Tidak", "—");
        // Menjalankan pemeriksaan NotNull atas `table` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // BuildTransactionHistoryJson_MergesCashMovementAndBalanceByEventSequence.
        Assert.NotNull(table);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`new[] { ”day_index”, ”action_slot”,
        // ”sequence_number”, ”cashflow_category”, ”coins_in_event”, ”coins_out_event”, ”coin_change”, ”coin_balance_after_event” }`, `table.Columns`);
        // pengujian gagal jika keduanya berbeda dalam BuildTransactionHistoryJson_MergesCashMovementAndBalanceByEventSequence.
        Assert.Equal(
            // Meneruskan array baru dengan tipe elemen disimpulkan dari nilai initializer sebagai argumen ke `Assert.Equal`.
            new[] { "day_index", "action_slot", "sequence_number", "cashflow_category", "coins_in_event", "coins_out_event", "coin_change", "coin_balance_after_event" },
            // Meneruskan `table.Columns` (nilai columns) sebagai argumen ke `Assert.Equal`.
            table.Columns);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`new[] { ”0”, ”0”, ”0”, ”LOAN”, ”10”, ”0”,
        // ”10”, ”20” }`, `table.Rows[0]`); pengujian gagal jika keduanya berbeda dalam
        // BuildTransactionHistoryJson_MergesCashMovementAndBalanceByEventSequence.
        Assert.Equal(new[] { "0", "0", "0", "LOAN", "10", "0", "10", "20" }, table.Rows[0]);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`new[] { ”2”, ”1”, ”35”, ”BUY”, ”0”, ”4”, ”-4”,
        // ”16” }`, `table.Rows[1]`); pengujian gagal jika keduanya berbeda dalam BuildTransactionHistoryJson_MergesCashMovementAndBalanceByEventSequence.
        Assert.Equal(new[] { "2", "1", "35", "BUY", "0", "4", "-4", "16" }, table.Rows[1]);
    // Menutup scope metode BuildTransactionHistoryJson_MergesCashMovementAndBalanceByEventSequence; bagian berikut berada di luar batas blok tersebut
    // dalam BuildTransactionHistoryJson_MergesCashMovementAndBalanceByEventSequence.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `BuildActionUsageHistoryJson_MergesSequenceAndRepetitionByDay` dengan hasil bertipe `void`; operasi ini menangani build
    // aksi usage history JSON merges sequence dan repetition berdasarkan hari.
    public void BuildActionUsageHistoryJson_MergesSequenceAndRepetitionByDay()
    // Membuka scope metode BuildActionUsageHistoryJson_MergesSequenceAndRepetitionByDay; pernyataan/deklarasi berikut berada di dalam batas blok ini
    // dalam BuildActionUsageHistoryJson_MergesSequenceAndRepetitionByDay.
    {
        // Menyiapkan variabel lokal `json` untuk nilai JSON dengan memanggil `PlayerMetricJsonMapper.BuildActionUsageHistoryJson` dengan
        // `”””[{”day_index”:2,”actions”:[”BahanMasakan”,”PesananMakanan”]},{”day_index”:1,”actions”:[”KerjaLepas”,”KerjaLepas”]}]”””`,
        // `”””[{”day_index”:1,”total_actions”:2,”distinct_actions”:1,”repeated_actions”:1,”diversity_score”:0.5},{”day_index”:2,”total_actions”:2,”distinct
        // _actions”:2,”repeated_actions”:...`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var json = PlayerMetricJsonMapper.BuildActionUsageHistoryJson(
            // Meneruskan nilai literal `”””[{”day_index”:2,”actions”:[”BahanMasakan”,”PesananMakanan”]},{”day_index”:1,”actions”:[”KerjaLepas”,”KerjaLepas”]}]”
            // ””` sebagai argumen ke `PlayerMetricJsonMapper.BuildActionUsageHistoryJson`.
            """[{"day_index":2,"actions":["BahanMasakan","PesananMakanan"]},{"day_index":1,"actions":["KerjaLepas","KerjaLepas"]}]""",
            // Meneruskan nilai literal `”””[{”day_index”:1,”total_actions”:2,”distinct_actions”:1,”repeated_actions”:1,”diversity_score”:0.5},{”day_index”:2,”t
            // otal_actions”:2,”distinct_actions”:2,”repeated_actions”:...` sebagai argumen ke `PlayerMetricJsonMapper.BuildActionUsageHistoryJson`.
            """[{"day_index":1,"total_actions":2,"distinct_actions":1,"repeated_actions":1,"diversity_score":0.5},{"day_index":2,"total_actions":2,"distinct_actions":2,"repeated_actions":0,"diversity_score":1}]""");

        // Menjalankan pemeriksaan NotNull atas `json` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // BuildActionUsageHistoryJson_MergesSequenceAndRepetitionByDay.
        Assert.NotNull(json);
        // Menyiapkan variabel lokal `table` untuk nilai table dengan memanggil `PlayerMetricJsonMapper.BuildCollectionTable` dengan `json`, `”Ya”`,
        // `”Tidak”`, `”—”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var table = PlayerMetricJsonMapper.BuildCollectionTable(json, "Ya", "Tidak", "—");
        // Menjalankan pemeriksaan NotNull atas `table` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // BuildActionUsageHistoryJson_MergesSequenceAndRepetitionByDay.
        Assert.NotNull(table);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`new[] { ”day_index”, ”actions”,
        // ”total_actions”, ”distinct_actions”, ”repeated_actions”, ”diversity_score” }`, `table.Columns`); pengujian gagal jika keduanya berbeda dalam
        // BuildActionUsageHistoryJson_MergesSequenceAndRepetitionByDay.
        Assert.Equal(
            // Meneruskan array baru dengan tipe elemen disimpulkan dari nilai initializer sebagai argumen ke `Assert.Equal`.
            new[] { "day_index", "actions", "total_actions", "distinct_actions", "action_pattern" },
            // Meneruskan `table.Columns` (nilai columns) sebagai argumen ke `Assert.Equal`.
            table.Columns);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`new[] { ”1”, ”KerjaLepas, KerjaLepas”, ”2”,
        // ”1”, ”1”, ”0.5” }`, `table.Rows[0]`); pengujian gagal jika keduanya berbeda dalam BuildActionUsageHistoryJson_MergesSequenceAndRepetitionByDay.
        Assert.Equal(new[] { "1", "KerjaLepas, KerjaLepas", "2", "1", "repeated" }, table.Rows[0]);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`new[] { ”2”, ”BahanMasakan, PesananMakanan”,
        // ”2”, ”2”, ”0”, ”1” }`, `table.Rows[1]`); pengujian gagal jika keduanya berbeda dalam
        // BuildActionUsageHistoryJson_MergesSequenceAndRepetitionByDay.
        Assert.Equal(new[] { "2", "BahanMasakan, PesananMakanan", "2", "2", "different" }, table.Rows[1]);
    // Menutup scope metode BuildActionUsageHistoryJson_MergesSequenceAndRepetitionByDay; bagian berikut berada di luar batas blok tersebut dalam
    // BuildActionUsageHistoryJson_MergesSequenceAndRepetitionByDay.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    /// <summary>
    /// Memvalidasi map elemen grup meng-clone JsonElement agar tetap valid setelah JsonDocument asal dispose.
    /// </summary>
    // Mendefinisikan metode `BuildMetricGroupElements_ClonesTopLevelElements` dengan hasil bertipe `void`; operasi ini menangani build metric group
    // elements clones top level elements.
    public void BuildMetricGroupElements_ClonesTopLevelElements()
    // Membuka scope metode BuildMetricGroupElements_ClonesTopLevelElements; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // BuildMetricGroupElements_ClonesTopLevelElements.
    {
        // Menyiapkan variabel lokal `map` untuk nilai pemetaan tanpa nilai awal pada deklarasi ini. Tipe yang dipakai adalah `Dictionary<string,
        // JsonElement>`.
        Dictionary<string, JsonElement> map;
        // Membatasi masa pakai `var doc = JsonDocument.Parse(”””{”coins”:{”start”:10}}”””)` pada blok using; sumber daya dilepas ketika blok berakhir
        // melalui Dispose.
        using (var doc = JsonDocument.Parse("""{"coins":{"start":10}}"""))
        // Membuka scope scope pemakaian sumber daya using; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // BuildMetricGroupElements_ClonesTopLevelElements.
        {
            // Memperbarui `map` menggunakan memanggil `PlayerMetricJsonMapper.BuildMetricGroupElements` dengan `doc.RootElement` dalam
            // BuildMetricGroupElements_ClonesTopLevelElements.
            map = PlayerMetricJsonMapper.BuildMetricGroupElements(doc.RootElement);
        // Menutup scope scope pemakaian sumber daya using; bagian berikut berada di luar batas blok tersebut dalam
        // BuildMetricGroupElements_ClonesTopLevelElements.
        }

        // Menjalankan pemeriksaan bahwa `map.TryGetValue(”coins”, out var coins)` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // BuildMetricGroupElements_ClonesTopLevelElements.
        Assert.True(map.TryGetValue("coins", out var coins));
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`10`, `coins.GetProperty(”start”).GetInt32()`);
        // pengujian gagal jika keduanya berbeda dalam BuildMetricGroupElements_ClonesTopLevelElements.
        Assert.Equal(10, coins.GetProperty("start").GetInt32());
    // Menutup scope metode BuildMetricGroupElements_ClonesTopLevelElements; bagian berikut berada di luar batas blok tersebut dalam
    // BuildMetricGroupElements_ClonesTopLevelElements.
    }

    // menandai metode sebagai pengujian xUnit yang dijalankan untuk setiap kombinasi data.
    [Theory]
    // menyediakan satu kombinasi masukan pengujian (”42”, true, 42).
    [InlineData("42", true, 42)]
    // menyediakan satu kombinasi masukan pengujian (”\”42.5\””, true, 42.5).
    [InlineData("\"42.5\"", true, 42.5)]
    // menyediakan satu kombinasi masukan pengujian (”true”, false, 0).
    [InlineData("true", false, 0)]
    /// <summary>
    /// Memvalidasi pembacaan nilai numerik dari JsonElement.
    /// </summary>
    // Mendefinisikan metode `TryGetNumericValue_ParsesNumberAndNumericString` dengan hasil bertipe `void`; operasi ini menangani try get numerik nilai
    // parses number dan numerik string. Masukan: Parameter `json` bertipe `string` membawa nilai JSON; Parameter `expectedResult` bertipe `bool`
    // membawa nilai yang diharapkan hasil; Parameter `expectedValue` bertipe `double` membawa nilai yang diharapkan nilai.
    public void TryGetNumericValue_ParsesNumberAndNumericString(string json, bool expectedResult, double expectedValue)
    // Membuka scope metode TryGetNumericValue_ParsesNumberAndNumericString; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // TryGetNumericValue_ParsesNumberAndNumericString.
    {
        // Menyiapkan variabel lokal `doc` untuk nilai doc dengan memanggil `JsonDocument.Parse` dengan `json`. Tipe variabel disimpulkan dari ekspresi
        // nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var doc = JsonDocument.Parse(json);

        // Menyiapkan variabel lokal `result` untuk nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya dengan memanggil
        // `PlayerMetricJsonMapper.TryGetNumericValue` dengan `doc.RootElement`, `var value`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var result = PlayerMetricJsonMapper.TryGetNumericValue(doc.RootElement, out var value);

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`expectedResult`, `result`); pengujian gagal
        // jika keduanya berbeda dalam TryGetNumericValue_ParsesNumberAndNumericString.
        Assert.Equal(expectedResult, result);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`expectedValue`, `value`); pengujian gagal jika
        // keduanya berbeda dalam TryGetNumericValue_ParsesNumberAndNumericString.
        Assert.Equal(expectedValue, value);
    // Menutup scope metode TryGetNumericValue_ParsesNumberAndNumericString; bagian berikut berada di luar batas blok tersebut dalam
    // TryGetNumericValue_ParsesNumberAndNumericString.
    }
// Menutup scope tipe PlayerMetricJsonMapperTests; bagian berikut berada di luar batas blok tersebut.
}
