// Fungsi file: Memverifikasi perilaku, lokalisasi, atau tata letak UI melalui PlayerMetricCollectionHelperTests.
// Mengimpor namespace `Cashflowpoly.Ui.Infrastructure` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Ui.Infrastructure;
// Mengimpor namespace `System.Globalization` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Globalization;
// Mengimpor namespace `Xunit` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Xunit;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Tests` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Tests;

// Mendefinisikan tipe class `PlayerMetricCollectionHelperTests`; sealed mencegah tipe ini diturunkan lagi.
public sealed class PlayerMetricCollectionHelperTests
// Membuka scope tipe PlayerMetricCollectionHelperTests; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    [Fact]
    public void BuildActualCalculation_SingleIncomeSourceDoesNotDisplayDivisionByZero()
    {
        var calculation = PlayerMetricCollectionHelper.BuildActualCalculation(
            "income-diversification",
            [("active_income_source_count", "1"), ("income_shares.freelance_income", "1")],
            "0", "%", "N/A", CultureInfo.InvariantCulture);

        Assert.Equal("0%", calculation);
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `MergeUniqueRows_KeepsFirstPathAndSkipsBlankPath` dengan hasil bertipe `void`; operasi ini menangani merge unique baris
    // keeps first path dan skips blank path.
    public void MergeUniqueRows_KeepsFirstPathAndSkipsBlankPath()
    // Membuka scope metode MergeUniqueRows_KeepsFirstPathAndSkipsBlankPath; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // MergeUniqueRows_KeepsFirstPathAndSkipsBlankPath.
    {
        // Menyiapkan variabel lokal `rows` untuk nilai baris dengan memanggil `PlayerMetricCollectionHelper.MergeUniqueRows` dengan `[(”cash”, ”10”), (””,
        // ”ignored”)]`, `[(”cash”, ”20”), (”gold”, ”2”)]`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var rows = PlayerMetricCollectionHelper.MergeUniqueRows(
            // Meneruskan koleksi berisi (”cash”, ”10”), (””, ”ignored”) sebagai argumen ke `PlayerMetricCollectionHelper.MergeUniqueRows`; Meneruskan nilai
            // literal `”cash”` sebagai argumen ke `PlayerMetricCollectionHelper.MergeUniqueRows`; Meneruskan nilai literal `”10”` sebagai argumen ke
            // `PlayerMetricCollectionHelper.MergeUniqueRows`; Meneruskan nilai literal `””` sebagai argumen ke `PlayerMetricCollectionHelper.MergeUniqueRows`;
            // Meneruskan nilai literal `”ignored”` sebagai argumen ke `PlayerMetricCollectionHelper.MergeUniqueRows`.
            [("cash", "10"), ("", "ignored")],
            // Meneruskan koleksi berisi (”cash”, ”20”), (”gold”, ”2”) sebagai argumen ke `PlayerMetricCollectionHelper.MergeUniqueRows`; Meneruskan nilai
            // literal `”cash”` sebagai argumen ke `PlayerMetricCollectionHelper.MergeUniqueRows`; Meneruskan nilai literal `”20”` sebagai argumen ke
            // `PlayerMetricCollectionHelper.MergeUniqueRows`; Meneruskan nilai literal `”gold”` sebagai argumen ke
            // `PlayerMetricCollectionHelper.MergeUniqueRows`; Meneruskan nilai literal `”2”` sebagai argumen ke `PlayerMetricCollectionHelper.MergeUniqueRows`.
            [("cash", "20"), ("gold", "2")]);

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`2`, `rows.Count`); pengujian gagal jika
        // keduanya berbeda dalam MergeUniqueRows_KeepsFirstPathAndSkipsBlankPath.
        Assert.Equal(2, rows.Count);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`(”cash”, ”10”)`, `rows[0]`); pengujian gagal
        // jika keduanya berbeda dalam MergeUniqueRows_KeepsFirstPathAndSkipsBlankPath.
        Assert.Equal(("cash", "10"), rows[0]);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`(”gold”, ”2”)`, `rows[1]`); pengujian gagal
        // jika keduanya berbeda dalam MergeUniqueRows_KeepsFirstPathAndSkipsBlankPath.
        Assert.Equal(("gold", "2"), rows[1]);
    // Menutup scope metode MergeUniqueRows_KeepsFirstPathAndSkipsBlankPath; bagian berikut berada di luar batas blok tersebut dalam
    // MergeUniqueRows_KeepsFirstPathAndSkipsBlankPath.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `GetGroupRows_CombinesRequestedGroupsInOrder` dengan hasil bertipe `void`; operasi ini menangani get group baris combines
    // yang diminta groups in urutan/pesanan.
    public void GetGroupRows_CombinesRequestedGroupsInOrder()
    // Membuka scope metode GetGroupRows_CombinesRequestedGroupsInOrder; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // GetGroupRows_CombinesRequestedGroupsInOrder.
    {
        // Menyiapkan variabel lokal `source` untuk nilai source dengan objek baru bertipe `Dictionary<string, List<(string Path, string Value)>>` dengan
        // argumen (StringComparer.OrdinalIgnoreCase). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var source = new Dictionary<string, List<(string Path, string Value)>>(StringComparer.OrdinalIgnoreCase)
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // GetGroupRows_CombinesRequestedGroupsInOrder.
        {
            // Memperbarui `[”coins”]` menggunakan koleksi berisi (”cash”, ”10”) dalam GetGroupRows_CombinesRequestedGroupsInOrder.
            ["coins"] = [("cash", "10")],
            // Memperbarui `[”gold”]` menggunakan koleksi berisi (”qty”, ”2”) dalam GetGroupRows_CombinesRequestedGroupsInOrder.
            ["gold"] = [("qty", "2")]
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // GetGroupRows_CombinesRequestedGroupsInOrder.
        };

        // Menyiapkan variabel lokal `rows` untuk nilai baris dengan memanggil `PlayerMetricCollectionHelper.GetGroupRows` dengan `source`, `”gold”`,
        // `”missing”`, `”coins”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var rows = PlayerMetricCollectionHelper.GetGroupRows(source, "gold", "missing", "coins");

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`[(”qty”, ”2”), (”cash”, ”10”)]`, `rows`);
        // pengujian gagal jika keduanya berbeda dalam GetGroupRows_CombinesRequestedGroupsInOrder.
        Assert.Equal([("qty", "2"), ("cash", "10")], rows);
    // Menutup scope metode GetGroupRows_CombinesRequestedGroupsInOrder; bagian berikut berada di luar batas blok tersebut dalam
    // GetGroupRows_CombinesRequestedGroupsInOrder.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `MergeUniqueCharts_KeepsFirstTitleAndSkipsBlankTitle` dengan hasil bertipe `void`; operasi ini menangani merge unique
    // charts keeps first title dan skips blank title.
    public void MergeUniqueCharts_KeepsFirstTitleAndSkipsBlankTitle()
    // Membuka scope metode MergeUniqueCharts_KeepsFirstTitleAndSkipsBlankTitle; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // MergeUniqueCharts_KeepsFirstTitleAndSkipsBlankTitle.
    {
        // Menyiapkan variabel lokal `charts` untuk nilai charts dengan memanggil `PlayerMetricCollectionHelper.MergeUniqueCharts` dengan `[(”Chart A”,
        // ”{}”), (””, ”{}”)]`, `[(”Chart A”, ”{\”duplicate\”:true}”), (”Chart B”, ”{}”)]`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var charts = PlayerMetricCollectionHelper.MergeUniqueCharts(
            // Meneruskan koleksi berisi (”Chart A”, ”{}”), (””, ”{}”) sebagai argumen ke `PlayerMetricCollectionHelper.MergeUniqueCharts`; Meneruskan nilai
            // literal `”Chart A”` sebagai argumen ke `PlayerMetricCollectionHelper.MergeUniqueCharts`; Meneruskan nilai literal `”{}”` sebagai argumen ke
            // `PlayerMetricCollectionHelper.MergeUniqueCharts`; Meneruskan nilai literal `””` sebagai argumen ke
            // `PlayerMetricCollectionHelper.MergeUniqueCharts`; Meneruskan nilai literal `”{}”` sebagai argumen ke
            // `PlayerMetricCollectionHelper.MergeUniqueCharts`.
            [("Chart A", "{}"), ("", "{}")],
            // Meneruskan koleksi berisi (”Chart A”, ”{\”duplicate\”:true}”), (”Chart B”, ”{}”) sebagai argumen ke
            // `PlayerMetricCollectionHelper.MergeUniqueCharts`; Meneruskan nilai literal `”Chart A”` sebagai argumen ke
            // `PlayerMetricCollectionHelper.MergeUniqueCharts`; Meneruskan nilai literal `”{\”duplicate\”:true}”` sebagai argumen ke
            // `PlayerMetricCollectionHelper.MergeUniqueCharts`; Meneruskan nilai literal `”Chart B”` sebagai argumen ke
            // `PlayerMetricCollectionHelper.MergeUniqueCharts`; Meneruskan nilai literal `”{}”` sebagai argumen ke
            // `PlayerMetricCollectionHelper.MergeUniqueCharts`.
            [("Chart A", "{\"duplicate\":true}"), ("Chart B", "{}")]);

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`2`, `charts.Count`); pengujian gagal jika
        // keduanya berbeda dalam MergeUniqueCharts_KeepsFirstTitleAndSkipsBlankTitle.
        Assert.Equal(2, charts.Count);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`(”Chart A”, ”{}”)`, `charts[0]`); pengujian
        // gagal jika keduanya berbeda dalam MergeUniqueCharts_KeepsFirstTitleAndSkipsBlankTitle.
        Assert.Equal(("Chart A", "{}"), charts[0]);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`(”Chart B”, ”{}”)`, `charts[1]`); pengujian
        // gagal jika keduanya berbeda dalam MergeUniqueCharts_KeepsFirstTitleAndSkipsBlankTitle.
        Assert.Equal(("Chart B", "{}"), charts[1]);
    // Menutup scope metode MergeUniqueCharts_KeepsFirstTitleAndSkipsBlankTitle; bagian berikut berada di luar batas blok tersebut dalam
    // MergeUniqueCharts_KeepsFirstTitleAndSkipsBlankTitle.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `FilterRowsAndChartsByKeywords_UsesCaseInsensitiveContains` dengan hasil bertipe `void`; operasi ini menangani filter baris
    // dan charts berdasarkan keywords uses case insensitive contains.
    public void FilterRowsAndChartsByKeywords_UsesCaseInsensitiveContains()
    // Membuka scope metode FilterRowsAndChartsByKeywords_UsesCaseInsensitiveContains; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // FilterRowsAndChartsByKeywords_UsesCaseInsensitiveContains.
    {
        // Menyiapkan variabel lokal `rows` untuk nilai baris dengan memanggil `PlayerMetricCollectionHelper.FilterRowsByKeywords` dengan `[(”gold_roi”,
        // ”10”), (”cashflow_net”, ”4”)]`, `”GOLD”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var rows = PlayerMetricCollectionHelper.FilterRowsByKeywords(
            // Meneruskan koleksi berisi (”gold_roi”, ”10”), (”cashflow_net”, ”4”) sebagai argumen ke `PlayerMetricCollectionHelper.FilterRowsByKeywords`;
            // Meneruskan nilai literal `”gold_roi”` sebagai argumen ke `PlayerMetricCollectionHelper.FilterRowsByKeywords`; Meneruskan nilai literal `”10”`
            // sebagai argumen ke `PlayerMetricCollectionHelper.FilterRowsByKeywords`; Meneruskan nilai literal `”cashflow_net”` sebagai argumen ke
            // `PlayerMetricCollectionHelper.FilterRowsByKeywords`; Meneruskan nilai literal `”4”` sebagai argumen ke
            // `PlayerMetricCollectionHelper.FilterRowsByKeywords`.
            [("gold_roi", "10"), ("cashflow_net", "4")],
            // Meneruskan nilai literal `”GOLD”` sebagai argumen ke `PlayerMetricCollectionHelper.FilterRowsByKeywords`.
            "GOLD");
        // Menyiapkan variabel lokal `charts` untuk nilai charts dengan memanggil `PlayerMetricCollectionHelper.FilterChartsByKeywords` dengan `[(”Gold
        // ROI”, ”{}”), (”Cashflow”, ”{}”)]`, `”gold”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var charts = PlayerMetricCollectionHelper.FilterChartsByKeywords(
            // Meneruskan koleksi berisi (”Gold ROI”, ”{}”), (”Cashflow”, ”{}”) sebagai argumen ke `PlayerMetricCollectionHelper.FilterChartsByKeywords`;
            // Meneruskan nilai literal `”Gold ROI”` sebagai argumen ke `PlayerMetricCollectionHelper.FilterChartsByKeywords`; Meneruskan nilai literal `”{}”`
            // sebagai argumen ke `PlayerMetricCollectionHelper.FilterChartsByKeywords`; Meneruskan nilai literal `”Cashflow”` sebagai argumen ke
            // `PlayerMetricCollectionHelper.FilterChartsByKeywords`; Meneruskan nilai literal `”{}”` sebagai argumen ke
            // `PlayerMetricCollectionHelper.FilterChartsByKeywords`.
            [("Gold ROI", "{}"), ("Cashflow", "{}")],
            // Meneruskan nilai literal `”gold”` sebagai argumen ke `PlayerMetricCollectionHelper.FilterChartsByKeywords`.
            "gold");

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`[(”gold_roi”, ”10”)]`, `rows`); pengujian
        // gagal jika keduanya berbeda dalam FilterRowsAndChartsByKeywords_UsesCaseInsensitiveContains.
        Assert.Equal([("gold_roi", "10")], rows);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`[(”Gold ROI”, ”{}”)]`, `charts`); pengujian
        // gagal jika keduanya berbeda dalam FilterRowsAndChartsByKeywords_UsesCaseInsensitiveContains.
        Assert.Equal([("Gold ROI", "{}")], charts);
    // Menutup scope metode FilterRowsAndChartsByKeywords_UsesCaseInsensitiveContains; bagian berikut berada di luar batas blok tersebut dalam
    // FilterRowsAndChartsByKeywords_UsesCaseInsensitiveContains.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `BuildActualCalculation_SubstitutesPlayerNumbersAndDisplayedResult` dengan hasil bertipe `void`; operasi ini menangani
    // build aktual calculation substitutes pemain numbers dan displayed hasil.
    public void BuildActualCalculation_SubstitutesPlayerNumbersAndDisplayedResult()
    // Membuka scope metode BuildActualCalculation_SubstitutesPlayerNumbersAndDisplayedResult; pernyataan/deklarasi berikut berada di dalam batas blok
    // ini dalam BuildActualCalculation_SubstitutesPlayerNumbersAndDisplayedResult.
    {
        // Menyiapkan variabel lokal `netWorth` untuk nilai net worth dengan memanggil `PlayerMetricCollectionHelper.BuildActualCalculation` dengan
        // `”net-worth”`, `[(”starting_coins”, ”10”), (”coins_net_end_game”, ”15”)]`, `”150”`, `”%”`, `”N/A”`, `CultureInfo.InvariantCulture`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var netWorth = PlayerMetricCollectionHelper.BuildActualCalculation(
            // Meneruskan nilai literal `”net-worth”` sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`.
            "net-worth",
            // Meneruskan koleksi berisi (”starting_coins”, ”10”), (”coins_net_end_game”, ”15”) sebagai argumen ke
            // `PlayerMetricCollectionHelper.BuildActualCalculation`; Meneruskan nilai literal `”starting_coins”` sebagai argumen ke
            // `PlayerMetricCollectionHelper.BuildActualCalculation`; Meneruskan nilai literal `”10”` sebagai argumen ke
            // `PlayerMetricCollectionHelper.BuildActualCalculation`; Meneruskan nilai literal `”coins_net_end_game”` sebagai argumen ke
            // `PlayerMetricCollectionHelper.BuildActualCalculation`; Meneruskan nilai literal `”15”` sebagai argumen ke
            // `PlayerMetricCollectionHelper.BuildActualCalculation`.
            [("starting_coins", "10"), ("coins_net_end_game", "15")],
            // Meneruskan nilai literal `”150”` sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`.
            "+50",
            // Meneruskan nilai literal `”%”` sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`.
            "%",
            // Meneruskan nilai literal `”N/A”` sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`.
            "N/A",
            // Meneruskan `CultureInfo.InvariantCulture` (nilai invariant culture) sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`.
            CultureInfo.InvariantCulture);
        // Menyiapkan variabel lokal `incomeDiversification` untuk nilai pemasukan diversification dengan memanggil
        // `PlayerMetricCollectionHelper.BuildActualCalculation` dengan `”income-diversification”`, `[ (”active_income_source_count”, ”2”),
        // (”income_shares.a”, ”0.75”), (”income_shares.b”, ”0.25”) ]`, `”75”`, `”%”`, `”N/A”`, `CultureInfo.InvariantCulture`. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var incomeDiversification = PlayerMetricCollectionHelper.BuildActualCalculation(
            // Meneruskan nilai literal `”income-diversification”` sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`.
            "income-diversification",
            // Meneruskan koleksi berisi (”active_income_source_count”, ”2”), (”income_shares.a”, ”0.75”), (”income_shares.b”, ”0.25”) sebagai argumen ke
            // `PlayerMetricCollectionHelper.BuildActualCalculation`.
            [
                // Meneruskan nilai literal `”active_income_source_count”` sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`; Meneruskan
                // nilai literal `”2”` sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`.
                ("active_income_source_count", "2"),
                // Meneruskan nilai literal `”income_shares.a”` sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`; Meneruskan nilai literal
                // `”0.75”` sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`.
                ("income_shares.a", "0.75"),
                // Meneruskan nilai literal `”income_shares.b”` sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`; Meneruskan nilai literal
                // `”0.25”` sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`.
                ("income_shares.b", "0.25")
            // Meneruskan koleksi berisi (”active_income_source_count”, ”2”), (”income_shares.a”, ”0.75”), (”income_shares.b”, ”0.25”) sebagai argumen ke
            // `PlayerMetricCollectionHelper.BuildActualCalculation`.
            ],
            // Meneruskan nilai literal `”75”` sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`.
            "75",
            // Meneruskan nilai literal `”%”` sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`.
            "%",
            // Meneruskan nilai literal `”N/A”` sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`.
            "N/A",
            // Meneruskan `CultureInfo.InvariantCulture` (nilai invariant culture) sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`.
            CultureInfo.InvariantCulture);
        // Menyiapkan variabel lokal `happiness` untuk nilai kebahagiaan dengan memanggil `PlayerMetricCollectionHelper.BuildActualCalculation` dengan
        // `”happiness-portfolio”`, `[ (”need_card_points”, ”5”), (”need_set_bonus_points”, ”10”), (”donation_points”, ”3”), (”gold_points”, ”2”),
        // (”pension_points”, ”1”), (”financial_goal_points”, ”4”), (”missio...`, `”17”`, `”points”`, `”N/A”`, `CultureInfo.InvariantCulture`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var happiness = PlayerMetricCollectionHelper.BuildActualCalculation(
            // Meneruskan nilai literal `”happiness-portfolio”` sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`.
            "happiness-portfolio",
            // Meneruskan koleksi berisi (”need_card_points”, ”5”), (”need_set_bonus_points”, ”10”), (”donation_points”, ”3”), (”gold_points”, ”2”),
            // (”pension_points”, ”1”), (”financial_goal_points”, ”4”), (”mission_penalty_points”, ”-6”), (”loan_penalty_points”, ”-2”) sebagai argumen ke
            // `PlayerMetricCollectionHelper.BuildActualCalculation`.
            [
                // Meneruskan nilai literal `”need_card_points”` sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`; Meneruskan nilai literal
                // `”5”` sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`.
                ("need_card_points", "5"),
                // Meneruskan nilai literal `”need_set_bonus_points”` sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`; Meneruskan nilai
                // literal `”10”` sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`.
                ("need_set_bonus_points", "10"),
                // Meneruskan nilai literal `”donation_points”` sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`; Meneruskan nilai literal
                // `”3”` sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`.
                ("donation_points", "3"),
                // Meneruskan nilai literal `”gold_points”` sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`; Meneruskan nilai literal `”2”`
                // sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`.
                ("gold_points", "2"),
                // Meneruskan nilai literal `”pension_points”` sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`; Meneruskan nilai literal
                // `”1”` sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`.
                ("pension_points", "1"),
                // Meneruskan nilai literal `”financial_goal_points”` sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`; Meneruskan nilai
                // literal `”4”` sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`.
                ("financial_goal_points", "4"),
                // Meneruskan nilai literal `”mission_penalty_points”` sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`; Meneruskan nilai
                // literal `”-6”` sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`.
                ("mission_penalty_points", "-6"),
                // Meneruskan nilai literal `”loan_penalty_points”` sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`; Meneruskan nilai
                // literal `”-2”` sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`.
                ("loan_penalty_points", "-2")
            // Meneruskan koleksi berisi (”need_card_points”, ”5”), (”need_set_bonus_points”, ”10”), (”donation_points”, ”3”), (”gold_points”, ”2”),
            // (”pension_points”, ”1”), (”financial_goal_points”, ”4”), (”mission_penalty_points”, ”-6”), (”loan_penalty_points”, ”-2”) sebagai argumen ke
            // `PlayerMetricCollectionHelper.BuildActualCalculation`.
            ],
            // Meneruskan nilai literal `”17”` sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`.
            "90.24",
            // Meneruskan nilai literal `”points”` sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`.
            "%",
            // Meneruskan nilai literal `”N/A”` sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`.
            "N/A",
            // Meneruskan `CultureInfo.InvariantCulture` (nilai invariant culture) sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`.
            CultureInfo.InvariantCulture);
        // Menyiapkan variabel lokal `beginnerHappiness` untuk nilai beginner kebahagiaan dengan memanggil
        // `PlayerMetricCollectionHelper.BuildActualCalculation` dengan `”happiness-portfolio-beginner”`, `[ (”need_card_points”, ”5”),
        // (”need_set_bonus_points”, ”10”), (”donation_points”, ”3”), (”gold_points”, ”2”), (”pension_points”, ”1”), (”mission_penalty_points”, ”-6”) ]`,
        // `”15”`, `”points”`, `”N/A”`, `CultureInfo.InvariantCulture`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var beginnerHappiness = PlayerMetricCollectionHelper.BuildActualCalculation(
            // Meneruskan nilai literal `”happiness-portfolio-beginner”` sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`.
            "happiness-portfolio-beginner",
            // Meneruskan koleksi berisi (”need_card_points”, ”5”), (”need_set_bonus_points”, ”10”), (”donation_points”, ”3”), (”gold_points”, ”2”),
            // (”pension_points”, ”1”), (”mission_penalty_points”, ”-6”) sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`.
            [
                // Meneruskan nilai literal `”need_card_points”` sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`; Meneruskan nilai literal
                // `”5”` sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`.
                ("need_card_points", "5"),
                // Meneruskan nilai literal `”need_set_bonus_points”` sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`; Meneruskan nilai
                // literal `”10”` sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`.
                ("need_set_bonus_points", "10"),
                // Meneruskan nilai literal `”donation_points”` sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`; Meneruskan nilai literal
                // `”3”` sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`.
                ("donation_points", "3"),
                // Meneruskan nilai literal `”gold_points”` sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`; Meneruskan nilai literal `”2”`
                // sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`.
                ("gold_points", "2"),
                // Meneruskan nilai literal `”pension_points”` sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`; Meneruskan nilai literal
                // `”1”` sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`.
                ("pension_points", "1"),
                // Meneruskan nilai literal `”mission_penalty_points”` sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`; Meneruskan nilai
                // literal `”-6”` sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`.
                ("mission_penalty_points", "-6")
            // Meneruskan koleksi berisi (”need_card_points”, ”5”), (”need_set_bonus_points”, ”10”), (”donation_points”, ”3”), (”gold_points”, ”2”),
            // (”pension_points”, ”1”), (”mission_penalty_points”, ”-6”) sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`.
            ],
            // Meneruskan nilai literal `”15”` sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`.
            "85.60",
            // Meneruskan nilai literal `”points”` sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`.
            "%",
            // Meneruskan nilai literal `”N/A”` sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`.
            "N/A",
            // Meneruskan `CultureInfo.InvariantCulture` (nilai invariant culture) sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`.
            CultureInfo.InvariantCulture);
        // Menyiapkan variabel lokal `unavailable` untuk nilai unavailable dengan memanggil `PlayerMetricCollectionHelper.BuildActualCalculation` dengan
        // `”expense-efficiency”`, `[(”ingredient_investment_coins_total”, ”0”), (”total_cash_out”, ”0”)]`, `”N/A”`, `”%”`, `”N/A”`,
        // `CultureInfo.InvariantCulture`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var unavailable = PlayerMetricCollectionHelper.BuildActualCalculation(
            // Meneruskan nilai literal `”expense-efficiency”` sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`.
            "expense-efficiency",
            // Meneruskan koleksi berisi (”ingredient_investment_coins_total”, ”0”), (”total_cash_out”, ”0”) sebagai argumen ke
            // `PlayerMetricCollectionHelper.BuildActualCalculation`; Meneruskan nilai literal `”ingredient_investment_coins_total”` sebagai argumen ke
            // `PlayerMetricCollectionHelper.BuildActualCalculation`; Meneruskan nilai literal `”0”` sebagai argumen ke
            // `PlayerMetricCollectionHelper.BuildActualCalculation`; Meneruskan nilai literal `”total_cash_out”` sebagai argumen ke
            // `PlayerMetricCollectionHelper.BuildActualCalculation`; Meneruskan nilai literal `”0”` sebagai argumen ke
            // `PlayerMetricCollectionHelper.BuildActualCalculation`.
            [("ingredient_investment_coins_total", "0"), ("total_cash_out", "0")],
            // Meneruskan nilai literal `”N/A”` sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`.
            "N/A",
            // Meneruskan nilai literal `”%”` sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`.
            "%",
            // Meneruskan nilai literal `”N/A”` sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`.
            "N/A",
            // Meneruskan `CultureInfo.InvariantCulture` (nilai invariant culture) sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`.
            CultureInfo.InvariantCulture);

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”15 ÷ 10 × 100% = 150%”`, `netWorth`);
        // pengujian gagal jika keduanya berbeda dalam BuildActualCalculation_SubstitutesPlayerNumbersAndDisplayedResult.
        Assert.Equal("(15 − 10) ÷ 10 × 100% = +50%", netWorth);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”[1 − ((75 ÷ 100)² + (25 ÷ 100)²)] ÷ [1 − (1 ÷
        // 2)] × 100% = 75%”`, `incomeDiversification`); pengujian gagal jika keduanya berbeda dalam
        // BuildActualCalculation_SubstitutesPlayerNumbersAndDisplayedResult.
        Assert.Equal("[1 − ((75 ÷ 100)² + (25 ÷ 100)²)] ÷ [1 − (1 ÷ 2)] × 100% = 75%", incomeDiversification);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”5 + 10 + 3 + 2 + 1 + 4 − 6 − 2 = 17 points”`,
        // `happiness`); pengujian gagal jika keduanya berbeda dalam BuildActualCalculation_SubstitutesPlayerNumbersAndDisplayedResult.
        Assert.Equal("[1 − ((5 ÷ 25)² + (10 ÷ 25)² + (3 ÷ 25)² + (2 ÷ 25)² + (1 ÷ 25)² + (4 ÷ 25)²)] ÷ (1 − 1/6) × 100% = 90.24%", happiness);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”5 + 10 + 3 + 2 + 1 − 6 = 15 points”`,
        // `beginnerHappiness`); pengujian gagal jika keduanya berbeda dalam BuildActualCalculation_SubstitutesPlayerNumbersAndDisplayedResult.
        Assert.Equal("[1 − ((5 ÷ 21)² + (10 ÷ 21)² + (3 ÷ 21)² + (2 ÷ 21)² + (1 ÷ 21)²)] ÷ (1 − 1/5) × 100% = 85.60%", beginnerHappiness);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”N/A”`, `unavailable`); pengujian gagal jika
        // keduanya berbeda dalam BuildActualCalculation_SubstitutesPlayerNumbersAndDisplayedResult.
        Assert.Equal("N/A", unavailable);
    // Menutup scope metode BuildActualCalculation_SubstitutesPlayerNumbersAndDisplayedResult; bagian berikut berada di luar batas blok tersebut dalam
    // BuildActualCalculation_SubstitutesPlayerNumbersAndDisplayedResult.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `BuildActualCalculation_PreservesEnoughPrecisionToExplainRoundedResult` dengan hasil bertipe `void`; operasi ini menangani
    // build aktual calculation preserves enough precision ke explain rounded hasil.
    public void BuildActualCalculation_PreservesEnoughPrecisionToExplainRoundedResult()
    // Membuka scope metode BuildActualCalculation_PreservesEnoughPrecisionToExplainRoundedResult; pernyataan/deklarasi berikut berada di dalam batas
    // blok ini dalam BuildActualCalculation_PreservesEnoughPrecisionToExplainRoundedResult.
    {
        // Menyiapkan variabel lokal `calculation` untuk nilai calculation dengan memanggil `PlayerMetricCollectionHelper.BuildActualCalculation` dengan
        // `”donation-commitment”`, `[ (”donation_stability_index”, ”29.28932188134524”), (”donated_resource_share”, ”0.272727272727”),
        // (”friday_participation_rate”, ”1”) ]`, `”7.99”`, `”score”`, `”N/A”`, `CultureInfo.InvariantCulture`. Tipe variabel disimpulkan dari ekspresi
        // nilai awal.
        var calculation = PlayerMetricCollectionHelper.BuildActualCalculation(
            // Meneruskan nilai literal `”donation-commitment”` sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`.
            "donation-commitment",
            // Meneruskan koleksi berisi (”donation_stability_index”, ”29.28932188134524”), (”donated_resource_share”, ”0.272727272727”),
            // (”friday_participation_rate”, ”1”) sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`.
            [
                // Meneruskan nilai literal `”donation_stability_index”` sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`; Meneruskan nilai
                // literal `”29.28932188134524”` sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`.
                ("donation_stability_index", "29.28932188134524"),
                // Meneruskan nilai literal `”donated_resource_share”` sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`; Meneruskan nilai
                // literal `”0.272727272727”` sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`.
                ("donated_resource_share", "0.272727272727"),
                // Meneruskan nilai literal `”friday_participation_rate”` sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`; Meneruskan nilai
                // literal `”1”` sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`.
                ("friday_participation_rate", "1")
            // Meneruskan koleksi berisi (”donation_stability_index”, ”29.28932188134524”), (”donated_resource_share”, ”0.272727272727”),
            // (”friday_participation_rate”, ”1”) sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`.
            ],
            // Meneruskan nilai literal `”7.99”` sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`.
            "7.99",
            // Meneruskan nilai literal `”score”` sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`.
            "score",
            // Meneruskan nilai literal `”N/A”` sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`.
            "N/A",
            // Meneruskan `CultureInfo.InvariantCulture` (nilai invariant culture) sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`.
            CultureInfo.InvariantCulture);

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”min(100, max(0, 29.289322 × 0.272727 × 1)) =
        // 7.99 score”`, `calculation`); pengujian gagal jika keduanya berbeda dalam BuildActualCalculation_PreservesEnoughPrecisionToExplainRoundedResult.
        Assert.Equal("min(100, max(0, 29.289322 × 0.272727 × 1)) = 7.99 score", calculation);
    // Menutup scope metode BuildActualCalculation_PreservesEnoughPrecisionToExplainRoundedResult; bagian berikut berada di luar batas blok tersebut
    // dalam BuildActualCalculation_PreservesEnoughPrecisionToExplainRoundedResult.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `BuildActualCalculation_CoversEveryPlayerAnalysisCard` dengan hasil bertipe `void`; operasi ini menangani build aktual
    // calculation covers every pemain analysis kartu.
    public void BuildActualCalculation_CoversEveryPlayerAnalysisCard()
    // Membuka scope metode BuildActualCalculation_CoversEveryPlayerAnalysisCard; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // BuildActualCalculation_CoversEveryPlayerAnalysisCard.
    {
        // Menyiapkan variabel lokal `rows` untuk nilai baris dengan array baru bertipe `(string Path, string Value)[]` dengan elemen sesuai initializer.
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var rows = new (string Path, string Value)[]
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // BuildActualCalculation_CoversEveryPlayerAnalysisCard.
        {
            // Menggunakan tuple yang membawa bagian 1: ”coins_net_end_game”; bagian 2: ”15” sebagai bagian ekspresi yang sedang disusun dalam
            // BuildActualCalculation_CoversEveryPlayerAnalysisCard.
            ("coins_net_end_game", "15"),
            // Menggunakan tuple yang membawa bagian 1: ”starting_coins”; bagian 2: ”10” sebagai bagian ekspresi yang sedang disusun dalam
            // BuildActualCalculation_CoversEveryPlayerAnalysisCard.
            ("starting_coins", "10"),
            // Menggunakan tuple yang membawa bagian 1: ”active_income_source_count”; bagian 2: ”2” sebagai bagian ekspresi yang sedang disusun dalam
            // BuildActualCalculation_CoversEveryPlayerAnalysisCard.
            ("active_income_source_count", "2"),
            // Menggunakan tuple yang membawa bagian 1: ”income_shares.a”; bagian 2: ”0.5” sebagai bagian ekspresi yang sedang disusun dalam
            // BuildActualCalculation_CoversEveryPlayerAnalysisCard.
            ("income_shares.a", "0.5"),
            // Menggunakan tuple yang membawa bagian 1: ”income_shares.b”; bagian 2: ”0.5” sebagai bagian ekspresi yang sedang disusun dalam
            // BuildActualCalculation_CoversEveryPlayerAnalysisCard.
            ("income_shares.b", "0.5"),
            // Menggunakan tuple yang membawa bagian 1: ”ingredient_investment_coins_total”; bagian 2: ”4” sebagai bagian ekspresi yang sedang disusun dalam
            // BuildActualCalculation_CoversEveryPlayerAnalysisCard.
            ("ingredient_investment_coins_total", "4"),
            // Menggunakan tuple yang membawa bagian 1: ”total_cash_out”; bagian 2: ”8” sebagai bagian ekspresi yang sedang disusun dalam
            // BuildActualCalculation_CoversEveryPlayerAnalysisCard.
            ("total_cash_out", "8"),
            // Menggunakan tuple yang membawa bagian 1: ”meal_order_income_total”; bagian 2: ”20” sebagai bagian ekspresi yang sedang disusun dalam
            // BuildActualCalculation_CoversEveryPlayerAnalysisCard.
            ("meal_order_income_total", "20"),
            // Menggunakan tuple yang membawa bagian 1: ”ingredient_cost_used”; bagian 2: ”5” sebagai bagian ekspresi yang sedang disusun dalam
            // BuildActualCalculation_CoversEveryPlayerAnalysisCard.
            ("ingredient_cost_used", "5"),
            // Menggunakan tuple yang membawa bagian 1: ”risks_resolved_without_emergency”; bagian 2: ”1” sebagai bagian ekspresi yang sedang disusun dalam
            // BuildActualCalculation_CoversEveryPlayerAnalysisCard.
            ("risks_resolved_without_emergency", "1"),
            // Menggunakan tuple yang membawa bagian 1: ”life_risk_cards_drawn”; bagian 2: ”2” sebagai bagian ekspresi yang sedang disusun dalam
            // BuildActualCalculation_CoversEveryPlayerAnalysisCard.
            ("life_risk_cards_drawn", "2"),
            // Menggunakan tuple yang membawa bagian 1: ”outstanding_loan”; bagian 2: ”2” sebagai bagian ekspresi yang sedang disusun dalam
            // BuildActualCalculation_CoversEveryPlayerAnalysisCard.
            ("outstanding_loan", "2"),
            // Menggunakan tuple yang membawa bagian 1: ”liquid_assets”; bagian 2: ”8” sebagai bagian ekspresi yang sedang disusun dalam
            // BuildActualCalculation_CoversEveryPlayerAnalysisCard.
            ("liquid_assets", "8"),
            // Menggunakan tuple yang membawa bagian 1: ”coins_committed_to_goals”; bagian 2: ”5” sebagai bagian ekspresi yang sedang disusun dalam
            // BuildActualCalculation_CoversEveryPlayerAnalysisCard.
            ("coins_committed_to_goals", "5"),
            // Menggunakan tuple yang membawa bagian 1: ”attempted_goal_target_total”; bagian 2: ”10” sebagai bagian ekspresi yang sedang disusun dalam
            // BuildActualCalculation_CoversEveryPlayerAnalysisCard.
            ("attempted_goal_target_total", "10"),
            // Menggunakan tuple yang membawa bagian 1: ”income_main_actions”; bagian 2: ”4” sebagai bagian ekspresi yang sedang disusun dalam
            // BuildActualCalculation_CoversEveryPlayerAnalysisCard.
            ("income_main_actions", "4"),
            // Menggunakan tuple yang membawa bagian 1: ”total_main_actions”; bagian 2: ”8” sebagai bagian ekspresi yang sedang disusun dalam
            // BuildActualCalculation_CoversEveryPlayerAnalysisCard.
            ("total_main_actions", "8"),
            // Menggunakan tuple yang membawa bagian 1: ”ingredients_used_in_completed_orders”; bagian 2: ”3” sebagai bagian ekspresi yang sedang disusun dalam
            // BuildActualCalculation_CoversEveryPlayerAnalysisCard.
            ("ingredients_used_in_completed_orders", "3"),
            // Menggunakan tuple yang membawa bagian 1: ”ingredients_collected”; bagian 2: ”4” sebagai bagian ekspresi yang sedang disusun dalam
            // BuildActualCalculation_CoversEveryPlayerAnalysisCard.
            ("ingredients_collected", "4"),
            // Menggunakan tuple yang membawa bagian 1: ”saving_actions”; bagian 2: ”1” sebagai bagian ekspresi yang sedang disusun dalam
            // BuildActualCalculation_CoversEveryPlayerAnalysisCard.
            ("saving_actions", "1"),
            ("saving_and_goal_actions", "2"),
            // Menggunakan tuple yang membawa bagian 1: ”financial_goal_actions”; bagian 2: ”1” sebagai bagian ekspresi yang sedang disusun dalam
            // BuildActualCalculation_CoversEveryPlayerAnalysisCard.
            ("financial_goal_actions", "1"),
            // Menggunakan tuple yang membawa bagian 1: ”insurance_actions”; bagian 2: ”1” sebagai bagian ekspresi yang sedang disusun dalam
            // BuildActualCalculation_CoversEveryPlayerAnalysisCard.
            ("insurance_actions", "1"),
            // Menggunakan tuple yang membawa bagian 1: ”loan_repayment_actions”; bagian 2: ”1” sebagai bagian ekspresi yang sedang disusun dalam
            // BuildActualCalculation_CoversEveryPlayerAnalysisCard.
            ("loan_repayment_actions", "1"),
            // Menggunakan tuple yang membawa bagian 1: ”primary_need_share”; bagian 2: ”0.34” sebagai bagian ekspresi yang sedang disusun dalam
            // BuildActualCalculation_CoversEveryPlayerAnalysisCard.
            ("primary_need_share", "0.34"),
            // Menggunakan tuple yang membawa bagian 1: ”secondary_need_share”; bagian 2: ”0.33” sebagai bagian ekspresi yang sedang disusun dalam
            // BuildActualCalculation_CoversEveryPlayerAnalysisCard.
            ("secondary_need_share", "0.33"),
            // Menggunakan tuple yang membawa bagian 1: ”tertiary_need_share”; bagian 2: ”0.33” sebagai bagian ekspresi yang sedang disusun dalam
            // BuildActualCalculation_CoversEveryPlayerAnalysisCard.
            ("tertiary_need_share", "0.33"),
            // Menggunakan tuple yang membawa bagian 1: ”donation_stability_index”; bagian 2: ”80” sebagai bagian ekspresi yang sedang disusun dalam
            // BuildActualCalculation_CoversEveryPlayerAnalysisCard.
            ("donation_stability_index", "80"),
            // Menggunakan tuple yang membawa bagian 1: ”donated_resource_share”; bagian 2: ”0.2” sebagai bagian ekspresi yang sedang disusun dalam
            // BuildActualCalculation_CoversEveryPlayerAnalysisCard.
            ("donated_resource_share", "0.2"),
            // Menggunakan tuple yang membawa bagian 1: ”friday_participation_rate”; bagian 2: ”0.5” sebagai bagian ekspresi yang sedang disusun dalam
            // BuildActualCalculation_CoversEveryPlayerAnalysisCard.
            ("friday_participation_rate", "0.5"),
            // Menggunakan tuple yang membawa bagian 1: ”need_card_points”; bagian 2: ”1” sebagai bagian ekspresi yang sedang disusun dalam
            // BuildActualCalculation_CoversEveryPlayerAnalysisCard.
            ("need_card_points", "1"),
            // Menggunakan tuple yang membawa bagian 1: ”need_set_bonus_points”; bagian 2: ”1” sebagai bagian ekspresi yang sedang disusun dalam
            // BuildActualCalculation_CoversEveryPlayerAnalysisCard.
            ("need_set_bonus_points", "1"),
            // Menggunakan tuple yang membawa bagian 1: ”donation_points”; bagian 2: ”1” sebagai bagian ekspresi yang sedang disusun dalam
            // BuildActualCalculation_CoversEveryPlayerAnalysisCard.
            ("donation_points", "1"),
            // Menggunakan tuple yang membawa bagian 1: ”gold_points”; bagian 2: ”1” sebagai bagian ekspresi yang sedang disusun dalam
            // BuildActualCalculation_CoversEveryPlayerAnalysisCard.
            ("gold_points", "1"),
            // Menggunakan tuple yang membawa bagian 1: ”pension_points”; bagian 2: ”1” sebagai bagian ekspresi yang sedang disusun dalam
            // BuildActualCalculation_CoversEveryPlayerAnalysisCard.
            ("pension_points", "1"),
            // Menggunakan tuple yang membawa bagian 1: ”financial_goal_points”; bagian 2: ”1” sebagai bagian ekspresi yang sedang disusun dalam
            // BuildActualCalculation_CoversEveryPlayerAnalysisCard.
            ("financial_goal_points", "1"),
            // Menggunakan tuple yang membawa bagian 1: ”mission_penalty_points”; bagian 2: ”0” sebagai bagian ekspresi yang sedang disusun dalam
            // BuildActualCalculation_CoversEveryPlayerAnalysisCard.
            ("mission_penalty_points", "0"),
            // Menggunakan tuple yang membawa bagian 1: ”loan_penalty_points”; bagian 2: ”0” sebagai bagian ekspresi yang sedang disusun dalam
            // BuildActualCalculation_CoversEveryPlayerAnalysisCard.
            ("loan_penalty_points", "0")
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // BuildActualCalculation_CoversEveryPlayerAnalysisCard.
        };
        // Menyiapkan variabel lokal `analysisKeys` untuk nilai analysis kunci dengan array baru dengan tipe elemen disimpulkan dari nilai initializer. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var analysisKeys = new[]
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // BuildActualCalculation_CoversEveryPlayerAnalysisCard.
        {
            // Menggunakan nilai literal `”net-worth”` sebagai bagian ekspresi yang sedang disusun dalam BuildActualCalculation_CoversEveryPlayerAnalysisCard.
            "net-worth",
            // Menggunakan nilai literal `”income-diversification”` sebagai bagian ekspresi yang sedang disusun dalam
            // BuildActualCalculation_CoversEveryPlayerAnalysisCard.
            "income-diversification",
            // Menggunakan nilai literal `”expense-efficiency”` sebagai bagian ekspresi yang sedang disusun dalam
            // BuildActualCalculation_CoversEveryPlayerAnalysisCard.
            "expense-efficiency",
            // Menggunakan nilai literal `”business-margin”` sebagai bagian ekspresi yang sedang disusun dalam
            // BuildActualCalculation_CoversEveryPlayerAnalysisCard.
            "business-margin",
            // Menggunakan nilai literal `”risk-appetite”` sebagai bagian ekspresi yang sedang disusun dalam
            // BuildActualCalculation_CoversEveryPlayerAnalysisCard.
            "risk-appetite",
            // Menggunakan nilai literal `”debt-discipline”` sebagai bagian ekspresi yang sedang disusun dalam
            // BuildActualCalculation_CoversEveryPlayerAnalysisCard.
            "debt-discipline",
            // Menggunakan nilai literal `”goal-ambition”` sebagai bagian ekspresi yang sedang disusun dalam
            // BuildActualCalculation_CoversEveryPlayerAnalysisCard.
            "goal-ambition",
            // Menggunakan nilai literal `”action-efficiency”` sebagai bagian ekspresi yang sedang disusun dalam
            // BuildActualCalculation_CoversEveryPlayerAnalysisCard.
            "action-efficiency",
            // Menggunakan nilai literal `”meal-success”` sebagai bagian ekspresi yang sedang disusun dalam
            // BuildActualCalculation_CoversEveryPlayerAnalysisCard.
            "meal-success",
            // Menggunakan nilai literal `”planning-horizon”` sebagai bagian ekspresi yang sedang disusun dalam
            // BuildActualCalculation_CoversEveryPlayerAnalysisCard.
            "planning-horizon",
            // Menggunakan nilai literal `”fulfillment-diversity”` sebagai bagian ekspresi yang sedang disusun dalam
            // BuildActualCalculation_CoversEveryPlayerAnalysisCard.
            "fulfillment-diversity",
            // Menggunakan nilai literal `”donation-commitment”` sebagai bagian ekspresi yang sedang disusun dalam
            // BuildActualCalculation_CoversEveryPlayerAnalysisCard.
            "donation-commitment",
            // Menggunakan nilai literal `”happiness-portfolio”` sebagai bagian ekspresi yang sedang disusun dalam
            // BuildActualCalculation_CoversEveryPlayerAnalysisCard.
            "happiness-portfolio"
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // BuildActualCalculation_CoversEveryPlayerAnalysisCard.
        };

        // Mengulangi setiap elemen `analysisKeys`; elemen saat ini disimpan sebagai `analysisKey` bertipe `var` untuk diproses oleh badan loop dalam
        // BuildActualCalculation_CoversEveryPlayerAnalysisCard.
        foreach (var analysisKey in analysisKeys)
        // Membuka scope loop setiap analysisKey dari `analysisKeys`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // BuildActualCalculation_CoversEveryPlayerAnalysisCard.
        {
            // Menyiapkan variabel lokal `calculation` untuk nilai calculation dengan memanggil `PlayerMetricCollectionHelper.BuildActualCalculation` dengan
            // `analysisKey`, `rows`, `”50”`, `”%”`, `”N/A”`, `CultureInfo.InvariantCulture`. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var calculation = PlayerMetricCollectionHelper.BuildActualCalculation(
                // Meneruskan `analysisKey` (nilai analysis kunci) sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`.
                analysisKey,
                // Meneruskan `rows` (nilai baris) sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`.
                rows,
                // Meneruskan nilai literal `”50”` sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`.
                analysisKey == "goal-ambition" ? "1" : "50",
                // Meneruskan nilai literal `”%”` sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`.
                analysisKey == "goal-ambition" ? "target" : "%",
                // Meneruskan nilai literal `”N/A”` sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`.
                "N/A",
                // Meneruskan `CultureInfo.InvariantCulture` (nilai invariant culture) sebagai argumen ke `PlayerMetricCollectionHelper.BuildActualCalculation`.
                CultureInfo.InvariantCulture);

            // Menjalankan pemeriksaan bahwa `calculation.StartsWith(”N/A”, StringComparison.Ordinal)` bernilai salah; pengujian gagal jika kondisi justru
            // terpenuhi dalam BuildActualCalculation_CoversEveryPlayerAnalysisCard.
            Assert.False(calculation.StartsWith("N/A", StringComparison.Ordinal));
            // Menjalankan pemeriksaan hasil dengan `Assert.EndsWith` menggunakan `”= 50%”`, `calculation`, `StringComparison.Ordinal`; ketidaksesuaian dengan
            // ekspektasi membuat pengujian gagal dalam BuildActualCalculation_CoversEveryPlayerAnalysisCard.
            if (analysisKey == "goal-ambition") Assert.Equal("1 target", calculation);
            else Assert.EndsWith("= 50%", calculation, StringComparison.Ordinal);
        // Menutup scope loop setiap analysisKey dari `analysisKeys`; bagian berikut berada di luar batas blok tersebut dalam
        // BuildActualCalculation_CoversEveryPlayerAnalysisCard.
        }
    // Menutup scope metode BuildActualCalculation_CoversEveryPlayerAnalysisCard; bagian berikut berada di luar batas blok tersebut dalam
    // BuildActualCalculation_CoversEveryPlayerAnalysisCard.
    }
// Menutup scope tipe PlayerMetricCollectionHelperTests; bagian berikut berada di luar batas blok tersebut.
}
