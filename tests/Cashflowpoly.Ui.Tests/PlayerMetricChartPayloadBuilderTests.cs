// Fungsi file: Memverifikasi perilaku, lokalisasi, atau tata letak UI melalui PlayerMetricChartPayloadBuilderTests.
// Mengimpor namespace `System.Text.Json` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.Json;
// Mengimpor namespace `Cashflowpoly.Ui.Infrastructure` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Ui.Infrastructure;
// Mengimpor namespace `Xunit` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Xunit;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Tests` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Tests;

// Mendefinisikan tipe class `PlayerMetricChartPayloadBuilderTests`; sealed mencegah tipe ini diturunkan lagi.
public sealed class PlayerMetricChartPayloadBuilderTests
// Membuka scope tipe PlayerMetricChartPayloadBuilderTests; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `BuildMergedRowChart_UsesPreferredScalarRowsBeforeNestedRows` dengan hasil bertipe `void`; operasi ini menangani build
    // merged baris chart uses preferred scalar baris before nested baris.
    public void BuildMergedRowChart_UsesPreferredScalarRowsBeforeNestedRows()
    // Membuka scope metode BuildMergedRowChart_UsesPreferredScalarRowsBeforeNestedRows; pernyataan/deklarasi berikut berada di dalam batas blok ini
    // dalam BuildMergedRowChart_UsesPreferredScalarRowsBeforeNestedRows.
    {
        // Menyiapkan variabel lokal `chart` untuk nilai chart dengan memanggil `PlayerMetricChartPayloadBuilder.BuildMergedRowChart` dengan `”Financial”`,
        // `[(”coins_net_end_game”, ”22”), (”coins.history.total”, ”30”), (”notes[0]”, ”skip”)]`, `true`, `Translate`. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var chart = PlayerMetricChartPayloadBuilder.BuildMergedRowChart(
            // Meneruskan nilai literal `”Financial”` sebagai argumen ke `PlayerMetricChartPayloadBuilder.BuildMergedRowChart`.
            "Financial",
            // Meneruskan koleksi berisi (”coins_net_end_game”, ”22”), (”coins.history.total”, ”30”), (”notes[0]”, ”skip”) sebagai argumen ke
            // `PlayerMetricChartPayloadBuilder.BuildMergedRowChart`; Meneruskan nilai literal `”coins_net_end_game”` sebagai argumen ke
            // `PlayerMetricChartPayloadBuilder.BuildMergedRowChart`; Meneruskan nilai literal `”22”` sebagai argumen ke
            // `PlayerMetricChartPayloadBuilder.BuildMergedRowChart`; Meneruskan nilai literal `”coins.history.total”` sebagai argumen ke
            // `PlayerMetricChartPayloadBuilder.BuildMergedRowChart`; Meneruskan nilai literal `”30”` sebagai argumen ke
            // `PlayerMetricChartPayloadBuilder.BuildMergedRowChart`; Meneruskan nilai literal `”notes[0]”` sebagai argumen ke
            // `PlayerMetricChartPayloadBuilder.BuildMergedRowChart`; Meneruskan nilai literal `”skip”` sebagai argumen ke
            // `PlayerMetricChartPayloadBuilder.BuildMergedRowChart`.
            [("coins_net_end_game", "22"), ("coins.history.total", "30"), ("notes[0]", "skip")],
            // Meneruskan true, yaitu kondisi aktif/terpenuhi sebagai argumen bernama `isRawDomain`.
            isRawDomain: true,
            // Meneruskan `Translate` (nilai translate) sebagai argumen ke `PlayerMetricChartPayloadBuilder.BuildMergedRowChart`.
            Translate);

        // Menjalankan pemeriksaan NotNull atas `chart` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // BuildMergedRowChart_UsesPreferredScalarRowsBeforeNestedRows.
        Assert.NotNull(chart);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”Financial: Combined Snapshot”`,
        // `chart.Value.Title`); pengujian gagal jika keduanya berbeda dalam BuildMergedRowChart_UsesPreferredScalarRowsBeforeNestedRows.
        Assert.Equal("Financial: Combined Snapshot", chart.Value.Title);

        // Menyiapkan variabel lokal `doc` untuk nilai doc dengan memanggil `JsonDocument.Parse` dengan `chart.Value.Json`. Tipe variabel disimpulkan dari
        // ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var doc = JsonDocument.Parse(chart.Value.Json);
        // Menyiapkan variabel lokal `root` untuk nilai root dengan `doc.RootElement` (nilai root element). Tipe variabel disimpulkan dari ekspresi nilai
        // awal.
        var root = doc.RootElement;
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”bar”`,
        // `root.GetProperty(”chartType”).GetString()`); pengujian gagal jika keduanya berbeda dalam
        // BuildMergedRowChart_UsesPreferredScalarRowsBeforeNestedRows.
        Assert.Equal("bar", root.GetProperty("chartType").GetString());
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”Ending Coins”`,
        // `root.GetProperty(”labels”)[0].GetString()`); pengujian gagal jika keduanya berbeda dalam
        // BuildMergedRowChart_UsesPreferredScalarRowsBeforeNestedRows.
        Assert.Equal("Ending Coins", root.GetProperty("labels")[0].GetString());
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”coins_net_end_game”`,
        // `root.GetProperty(”keys”)[0].GetString()`); pengujian gagal jika keduanya berbeda dalam
        // BuildMergedRowChart_UsesPreferredScalarRowsBeforeNestedRows.
        Assert.Equal("coins_net_end_game", root.GetProperty("keys")[0].GetString());
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`22`,
        // `root.GetProperty(”series”)[0].GetProperty(”values”)[0].GetDouble()`); pengujian gagal jika keduanya berbeda dalam
        // BuildMergedRowChart_UsesPreferredScalarRowsBeforeNestedRows.
        Assert.Equal(22, root.GetProperty("series")[0].GetProperty("values")[0].GetDouble());
    // Menutup scope metode BuildMergedRowChart_UsesPreferredScalarRowsBeforeNestedRows; bagian berikut berada di luar batas blok tersebut dalam
    // BuildMergedRowChart_UsesPreferredScalarRowsBeforeNestedRows.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `BuildLineChartPayload_BuildsSeriesFromNumericArray` dengan hasil bertipe `void`; operasi ini menangani build line chart
    // payload builds series dari numerik array.
    public void BuildLineChartPayload_BuildsSeriesFromNumericArray()
    // Membuka scope metode BuildLineChartPayload_BuildsSeriesFromNumericArray; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // BuildLineChartPayload_BuildsSeriesFromNumericArray.
    {
        // Menyiapkan variabel lokal `doc` untuk nilai doc dengan memanggil `JsonDocument.Parse` dengan `”[10,12]”`. Tipe variabel disimpulkan dari ekspresi
        // nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var doc = JsonDocument.Parse("[10,12]");

        // Menyiapkan variabel lokal `chart` untuk nilai chart dengan memanggil `PlayerMetricChartPayloadBuilder.BuildLineChartPayload` dengan
        // `”coins.coins_per_turn_progression”`, `doc.RootElement`, `true`, `Translate`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var chart = PlayerMetricChartPayloadBuilder.BuildLineChartPayload(
            // Meneruskan nilai literal `”coins.coins_per_turn_progression”` sebagai argumen ke `PlayerMetricChartPayloadBuilder.BuildLineChartPayload`.
            "coins.coins_per_turn_progression",
            // Meneruskan `doc.RootElement` (nilai root element) sebagai argumen ke `PlayerMetricChartPayloadBuilder.BuildLineChartPayload`.
            doc.RootElement,
            // Meneruskan true, yaitu kondisi aktif/terpenuhi sebagai argumen bernama `isRawDomain`.
            isRawDomain: true,
            // Meneruskan `Translate` (nilai translate) sebagai argumen ke `PlayerMetricChartPayloadBuilder.BuildLineChartPayload`.
            Translate);

        // Menjalankan pemeriksaan NotNull atas `chart` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // BuildLineChartPayload_BuildsSeriesFromNumericArray.
        Assert.NotNull(chart);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”Coins: Coins per turn progression”`,
        // `chart.Value.Title`); pengujian gagal jika keduanya berbeda dalam BuildLineChartPayload_BuildsSeriesFromNumericArray.
        Assert.Equal("Coins: Coins per turn progression", chart.Value.Title);

        // Menyiapkan variabel lokal `payload` untuk muatan detail event dalam format JSON dengan memanggil `JsonDocument.Parse` dengan `chart.Value.Json`.
        // Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var payload = JsonDocument.Parse(chart.Value.Json);
        // Menyiapkan variabel lokal `root` untuk nilai root dengan `payload.RootElement` (nilai root element). Tipe variabel disimpulkan dari ekspresi
        // nilai awal.
        var root = payload.RootElement;
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”1”`,
        // `root.GetProperty(”labels”)[0].GetString()`); pengujian gagal jika keduanya berbeda dalam BuildLineChartPayload_BuildsSeriesFromNumericArray.
        Assert.Equal("1", root.GetProperty("labels")[0].GetString());
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”2”`,
        // `root.GetProperty(”labels”)[1].GetString()`); pengujian gagal jika keduanya berbeda dalam BuildLineChartPayload_BuildsSeriesFromNumericArray.
        Assert.Equal("2", root.GetProperty("labels")[1].GetString());
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”Value”`,
        // `root.GetProperty(”series”)[0].GetProperty(”name”).GetString()`); pengujian gagal jika keduanya berbeda dalam
        // BuildLineChartPayload_BuildsSeriesFromNumericArray.
        Assert.Equal("Value", root.GetProperty("series")[0].GetProperty("name").GetString());
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`10`,
        // `root.GetProperty(”series”)[0].GetProperty(”values”)[0].GetDouble()`); pengujian gagal jika keduanya berbeda dalam
        // BuildLineChartPayload_BuildsSeriesFromNumericArray.
        Assert.Equal(10, root.GetProperty("series")[0].GetProperty("values")[0].GetDouble());
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`12`,
        // `root.GetProperty(”series”)[0].GetProperty(”values”)[1].GetDouble()`); pengujian gagal jika keduanya berbeda dalam
        // BuildLineChartPayload_BuildsSeriesFromNumericArray.
        Assert.Equal(12, root.GetProperty("series")[0].GetProperty("values")[1].GetDouble());
    // Menutup scope metode BuildLineChartPayload_BuildsSeriesFromNumericArray; bagian berikut berada di luar batas blok tersebut dalam
    // BuildLineChartPayload_BuildsSeriesFromNumericArray.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `BuildLineChartsForGroup_ReturnsOnlyRenderableChildren` dengan hasil bertipe `void`; operasi ini menangani build line
    // charts untuk group returns only renderable children.
    public void BuildLineChartsForGroup_ReturnsOnlyRenderableChildren()
    // Membuka scope metode BuildLineChartsForGroup_ReturnsOnlyRenderableChildren; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // BuildLineChartsForGroup_ReturnsOnlyRenderableChildren.
    {
        // Menyiapkan variabel lokal `doc` untuk nilai doc dengan memanggil `JsonDocument.Parse` dengan `”””{”trend”:[1,2],”text”:”ignored”}”””`. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var doc = JsonDocument.Parse("""{"trend":[1,2],"text":"ignored"}""");

        // Menyiapkan variabel lokal `charts` untuk nilai charts dengan memanggil `PlayerMetricChartPayloadBuilder.BuildLineChartsForGroup` dengan
        // `”coins”`, `doc.RootElement`, `false`, `Translate`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var charts = PlayerMetricChartPayloadBuilder.BuildLineChartsForGroup(
            // Meneruskan nilai literal `”coins”` sebagai argumen ke `PlayerMetricChartPayloadBuilder.BuildLineChartsForGroup`.
            "coins",
            // Meneruskan `doc.RootElement` (nilai root element) sebagai argumen ke `PlayerMetricChartPayloadBuilder.BuildLineChartsForGroup`.
            doc.RootElement,
            // Meneruskan false, yaitu kondisi nonaktif/tidak terpenuhi sebagai argumen bernama `isRawDomain`.
            isRawDomain: false,
            // Meneruskan `Translate` (nilai translate) sebagai argumen ke `PlayerMetricChartPayloadBuilder.BuildLineChartsForGroup`.
            Translate);

        // Menjalankan pemeriksaan hasil dengan `Assert.Single` menggunakan `charts`; ketidaksesuaian dengan ekspektasi membuat pengujian gagal dalam
        // BuildLineChartsForGroup_ReturnsOnlyRenderableChildren.
        Assert.Single(charts);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”Coins: Trend”`, `charts[0].Title`); pengujian
        // gagal jika keduanya berbeda dalam BuildLineChartsForGroup_ReturnsOnlyRenderableChildren.
        Assert.Equal("Coins: Trend", charts[0].Title);
    // Menutup scope metode BuildLineChartsForGroup_ReturnsOnlyRenderableChildren; bagian berikut berada di luar batas blok tersebut dalam
    // BuildLineChartsForGroup_ReturnsOnlyRenderableChildren.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `BuildMergedRowChart_FallsBackToZeroBarsWhenRowsAreNonNumeric` dengan hasil bertipe `void`; operasi ini menangani build
    // merged baris chart falls back ke zero bars when baris are non numerik.
    public void BuildMergedRowChart_FallsBackToZeroBarsWhenRowsAreNonNumeric()
    // Membuka scope metode BuildMergedRowChart_FallsBackToZeroBarsWhenRowsAreNonNumeric; pernyataan/deklarasi berikut berada di dalam batas blok ini
    // dalam BuildMergedRowChart_FallsBackToZeroBarsWhenRowsAreNonNumeric.
    {
        // Menyiapkan variabel lokal `chart` untuk nilai chart dengan memanggil `PlayerMetricChartPayloadBuilder.BuildMergedRowChart` dengan `”Financial”`,
        // `[(”coins_net_end_game”, ”n/a”)]`, `true`, `Translate`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var chart = PlayerMetricChartPayloadBuilder.BuildMergedRowChart(
            // Meneruskan nilai literal `”Financial”` sebagai argumen ke `PlayerMetricChartPayloadBuilder.BuildMergedRowChart`.
            "Financial",
            // Meneruskan koleksi berisi (”coins_net_end_game”, ”n/a”) sebagai argumen ke `PlayerMetricChartPayloadBuilder.BuildMergedRowChart`; Meneruskan
            // nilai literal `”coins_net_end_game”` sebagai argumen ke `PlayerMetricChartPayloadBuilder.BuildMergedRowChart`; Meneruskan nilai literal `”n/a”`
            // sebagai argumen ke `PlayerMetricChartPayloadBuilder.BuildMergedRowChart`.
            [("coins_net_end_game", "n/a")],
            // Meneruskan true, yaitu kondisi aktif/terpenuhi sebagai argumen bernama `isRawDomain`.
            isRawDomain: true,
            // Meneruskan `Translate` (nilai translate) sebagai argumen ke `PlayerMetricChartPayloadBuilder.BuildMergedRowChart`.
            Translate);

        // Menjalankan pemeriksaan NotNull atas `chart` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // BuildMergedRowChart_FallsBackToZeroBarsWhenRowsAreNonNumeric.
        Assert.NotNull(chart);

        // Menyiapkan variabel lokal `doc` untuk nilai doc dengan memanggil `JsonDocument.Parse` dengan `chart.Value.Json`. Tipe variabel disimpulkan dari
        // ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var doc = JsonDocument.Parse(chart.Value.Json);
        // Menyiapkan variabel lokal `root` untuk nilai root dengan `doc.RootElement` (nilai root element). Tipe variabel disimpulkan dari ekspresi nilai
        // awal.
        var root = doc.RootElement;
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”bar”`,
        // `root.GetProperty(”chartType”).GetString()`); pengujian gagal jika keduanya berbeda dalam
        // BuildMergedRowChart_FallsBackToZeroBarsWhenRowsAreNonNumeric.
        Assert.Equal("bar", root.GetProperty("chartType").GetString());
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”Ending Coins”`,
        // `root.GetProperty(”labels”)[0].GetString()`); pengujian gagal jika keduanya berbeda dalam
        // BuildMergedRowChart_FallsBackToZeroBarsWhenRowsAreNonNumeric.
        Assert.Equal("Ending Coins", root.GetProperty("labels")[0].GetString());
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`0`,
        // `root.GetProperty(”series”)[0].GetProperty(”values”)[0].GetDouble()`); pengujian gagal jika keduanya berbeda dalam
        // BuildMergedRowChart_FallsBackToZeroBarsWhenRowsAreNonNumeric.
        Assert.Equal(0, root.GetProperty("series")[0].GetProperty("values")[0].GetDouble());
    // Menutup scope metode BuildMergedRowChart_FallsBackToZeroBarsWhenRowsAreNonNumeric; bagian berikut berada di luar batas blok tersebut dalam
    // BuildMergedRowChart_FallsBackToZeroBarsWhenRowsAreNonNumeric.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `BuildMergedRowChart_ReturnsGenericZeroChartWhenRowsAreMissing` dengan hasil bertipe `void`; operasi ini menangani build
    // merged baris chart returns generic zero chart when baris are missing.
    public void BuildMergedRowChart_ReturnsGenericZeroChartWhenRowsAreMissing()
    // Membuka scope metode BuildMergedRowChart_ReturnsGenericZeroChartWhenRowsAreMissing; pernyataan/deklarasi berikut berada di dalam batas blok ini
    // dalam BuildMergedRowChart_ReturnsGenericZeroChartWhenRowsAreMissing.
    {
        // Menyiapkan variabel lokal `chart` untuk nilai chart dengan memanggil `PlayerMetricChartPayloadBuilder.BuildMergedRowChart` dengan `”Financial”`,
        // `[]`, `false`, `Translate`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var chart = PlayerMetricChartPayloadBuilder.BuildMergedRowChart(
            // Meneruskan nilai literal `”Financial”` sebagai argumen ke `PlayerMetricChartPayloadBuilder.BuildMergedRowChart`.
            "Financial",
            // Meneruskan koleksi kosong dengan tipe mengikuti konteks tujuan sebagai argumen ke `PlayerMetricChartPayloadBuilder.BuildMergedRowChart`.
            [],
            // Meneruskan false, yaitu kondisi nonaktif/tidak terpenuhi sebagai argumen bernama `isRawDomain`.
            isRawDomain: false,
            // Meneruskan `Translate` (nilai translate) sebagai argumen ke `PlayerMetricChartPayloadBuilder.BuildMergedRowChart`.
            Translate);

        // Menjalankan pemeriksaan NotNull atas `chart` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // BuildMergedRowChart_ReturnsGenericZeroChartWhenRowsAreMissing.
        Assert.NotNull(chart);

        // Menyiapkan variabel lokal `doc` untuk nilai doc dengan memanggil `JsonDocument.Parse` dengan `chart.Value.Json`. Tipe variabel disimpulkan dari
        // ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var doc = JsonDocument.Parse(chart.Value.Json);
        // Menyiapkan variabel lokal `root` untuk nilai root dengan `doc.RootElement` (nilai root element). Tipe variabel disimpulkan dari ekspresi nilai
        // awal.
        var root = doc.RootElement;
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”Metric”`,
        // `root.GetProperty(”labels”)[0].GetString()`); pengujian gagal jika keduanya berbeda dalam
        // BuildMergedRowChart_ReturnsGenericZeroChartWhenRowsAreMissing.
        Assert.Equal("Metric", root.GetProperty("labels")[0].GetString());
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`0`,
        // `root.GetProperty(”series”)[0].GetProperty(”values”)[0].GetDouble()`); pengujian gagal jika keduanya berbeda dalam
        // BuildMergedRowChart_ReturnsGenericZeroChartWhenRowsAreMissing.
        Assert.Equal(0, root.GetProperty("series")[0].GetProperty("values")[0].GetDouble());
    // Menutup scope metode BuildMergedRowChart_ReturnsGenericZeroChartWhenRowsAreMissing; bagian berikut berada di luar batas blok tersebut dalam
    // BuildMergedRowChart_ReturnsGenericZeroChartWhenRowsAreMissing.
    }

    // Mendefinisikan metode `Translate` dengan hasil bertipe `string`; operasi ini menangani translate. Masukan: Parameter `key` bertipe `string`
    // membawa nilai kunci.
    private static string Translate(string key)
    // Membuka scope metode Translate; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Translate.
    {
        // Mengembalikan hasil pemetaan `key` melalui cabang pola switch yang cocok kepada pemanggil dalam Translate; eksekusi jalur ini selesai setelah
        // nilai hasil ditentukan.
        return key switch
        // Membuka scope pemetaan switch atas `key`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Translate.
        {
            // Untuk pola `”common.value”`, menghasilkan nilai literal `”Value”` sebagai hasil switch.
            "common.value" => "Value",
            // Untuk pola `”players.details.series”`, menghasilkan nilai literal `”Series”` sebagai hasil switch.
            "players.details.series" => "Series",
            // Untuk pola `”players.details.metric_fallback”`, menghasilkan nilai literal `”Metric”` sebagai hasil switch.
            "players.details.metric_fallback" => "Metric",
            // Untuk pola `”players.details.item”`, menghasilkan nilai literal `”Item”` sebagai hasil switch.
            "players.details.item" => "Item",
            // Untuk pola `”players.details.raw_title”`, menghasilkan nilai literal `”Raw Metrics”` sebagai hasil switch.
            "players.details.raw_title" => "Raw Metrics",
            // Untuk pola `”players.details.derived_title”`, menghasilkan nilai literal `”Derived Metrics”` sebagai hasil switch.
            "players.details.derived_title" => "Derived Metrics",
            // Untuk pola `”players.details.combined_snapshot”`, menghasilkan nilai literal `”Combined Snapshot”` sebagai hasil switch.
            "players.details.combined_snapshot" => "Combined Snapshot",
            // Untuk pola `”players.details.source_calc_label”`, menghasilkan nilai literal `”Data Source”` sebagai hasil switch.
            "players.details.source_calc_label" => "Data Source",
            // Untuk pola `”players.details.source_calc_template”`, menghasilkan nilai literal `”{0}. Details: {1}”` sebagai hasil switch.
            "players.details.source_calc_template" => "{0}. Details: {1}",
            // Untuk pola `”players.details.source_raw_summary”`, menghasilkan nilai literal `”Raw summary”` sebagai hasil switch.
            "players.details.source_raw_summary" => "Raw summary",
            // Untuk pola `”players.details.source_derived_summary”`, menghasilkan nilai literal `”Derived summary”` sebagai hasil switch.
            "players.details.source_derived_summary" => "Derived summary",
            // Untuk pola `”players.details.source_line_raw_template”`, menghasilkan nilai literal `”{0}.”` sebagai hasil switch.
            "players.details.source_line_raw_template" => "{0}.",
            // Untuk pola `”players.details.source_line_derived_template”`, menghasilkan nilai literal `”{0}. Derived.”` sebagai hasil switch.
            "players.details.source_line_derived_template" => "{0}. Derived.",
            // Untuk pola `”players.details.formula.coins_net_end”`, menghasilkan nilai literal `”Starting + in - out.”` sebagai hasil switch.
            "players.details.formula.coins_net_end" => "Starting + in - out.",
            // Untuk pola `”players.raw.coins”`, menghasilkan nilai literal `”Coins”` sebagai hasil switch.
            "players.raw.coins" => "Coins",
            // Untuk pola `”players.raw.coins_net_end_game”`, menghasilkan nilai literal `”Ending Coins”` sebagai hasil switch.
            "players.raw.coins_net_end_game" => "Ending Coins",
            // Untuk pola `”players.raw.trend”`, menghasilkan nilai literal `”Trend”` sebagai hasil switch.
            "players.raw.trend" => "Trend",
            // Untuk pola `_`, menghasilkan `key` (nilai kunci) sebagai hasil switch.
            _ => key
        // Menutup scope pemetaan switch atas `key`; bagian berikut berada di luar batas blok tersebut dalam Translate.
        };
    // Menutup scope metode Translate; bagian berikut berada di luar batas blok tersebut dalam Translate.
    }
// Menutup scope tipe PlayerMetricChartPayloadBuilderTests; bagian berikut berada di luar batas blok tersebut.
}
