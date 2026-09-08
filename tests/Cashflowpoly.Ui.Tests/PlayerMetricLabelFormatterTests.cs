// Fungsi file: Memverifikasi perilaku, lokalisasi, atau tata letak UI melalui PlayerMetricLabelFormatterTests.
// Mengimpor namespace `System.Globalization` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Globalization;
// Mengimpor namespace `Cashflowpoly.Ui.Infrastructure` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Ui.Infrastructure;
// Mengimpor namespace `Xunit` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Xunit;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Tests` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Tests;

// Mendefinisikan tipe class `PlayerMetricLabelFormatterTests`; sealed mencegah tipe ini diturunkan lagi.
public sealed class PlayerMetricLabelFormatterTests
// Membuka scope tipe PlayerMetricLabelFormatterTests; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    [Theory]
    [InlineData("99.5", "risk_readiness_most")]
    [InlineData("100", "risk_readiness_complete")]
    public void DescribeMetric_RiskResolutionIsCompleteOnlyWhenEveryRiskWasResolved(string value, string guidance)
    {
        var metric = PlayerMetricLabelFormatter.DescribeMetric("risk_readiness_percent", value, true, true, "N/A", key => key);
        Assert.Equal("players.support.guide." + guidance, metric.Guidance);
    }

    [Theory]
    [InlineData("120", "+20", "wealth_growing", "recorded")]
    [InlineData("80", "-20", "wealth_declining", "recorded")]
    [InlineData("100", "0", "wealth_unchanged", "zero")]
    [InlineData("0", "-100", "wealth_declining", "recorded")]
    [InlineData("200", "+100", "wealth_strong", "recorded")]
    [InlineData("300", "+200", "wealth_exceptional", "recorded")]
    [InlineData("—", "players.support.value.unavailable", "unavailable", "unavailable")]
    public void DescribeMetric_ShowsSignedCoinChangeFromTheStartingBalance(
        string rawValue, string displayValue, string guidance, string state)
    {
        foreach (var metricKey in new[] { "cash_growth_percent", "net_worth_index" })
        {
            var metric = PlayerMetricLabelFormatter.DescribeMetric(
                metricKey, rawValue, true, true, "—", key => key);

            Assert.Equal(displayValue, metric.DisplayValue);
            Assert.Equal($"players.support.guide.{guidance}", metric.Guidance);
            Assert.Equal(state, metric.State);
            Assert.Equal(state == "unavailable" ? string.Empty : "players.support.unit.percent", metric.Unit);
        }
    }

    // menandai metode sebagai pengujian xUnit yang dijalankan untuk setiap kombinasi data.
    [Theory]
    // menyediakan satu kombinasi masukan pengujian (”id”, ”5”, ”recorded”).
    [InlineData("id", "5", "recorded")]
    // menyediakan satu kombinasi masukan pengujian (”id”, ”0”, ”zero”).
    [InlineData("id", "0", "zero")]
    // menyediakan satu kombinasi masukan pengujian (”id”, ”-10”, ”recorded”).
    [InlineData("id", "-10", "recorded")]
    // menyediakan satu kombinasi masukan pengujian (”id”, ”—”, ”unavailable”).
    [InlineData("id", "—", "unavailable")]
    // menyediakan satu kombinasi masukan pengujian (”en”, ”5”, ”recorded”).
    [InlineData("en", "5", "recorded")]
    // menyediakan satu kombinasi masukan pengujian (”en”, ”—”, ”unavailable”).
    [InlineData("en", "—", "unavailable")]
    // Mendefinisikan metode `DescribeMetric_UsesHappinessPointLabelAndUnitWithoutChangingTheValue` dengan hasil bertipe `void`; operasi ini menangani
    // describe metric uses kebahagiaan point label dan unit tanpa changing the nilai. Masukan: Parameter `language` bertipe `string` membawa nilai
    // language; Parameter `value` bertipe `string` membawa nilai nilai; Parameter `state` bertipe `string` membawa keadaan permainan yang menjadi
    // sumber atau hasil pembaruan.
    public void DescribeMetric_UsesHappinessPointLabelAndUnitWithoutChangingTheValue(
        // Parameter `language` bertipe `string` membawa nilai language.
        string language, string value, string state)
    // Membuka scope metode DescribeMetric_UsesHappinessPointLabelAndUnitWithoutChangingTheValue; pernyataan/deklarasi berikut berada di dalam batas
    // blok ini dalam DescribeMetric_UsesHappinessPointLabelAndUnitWithoutChangingTheValue.
    {
        // Menyiapkan variabel lokal `result` untuk nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya dengan memanggil
        // `PlayerMetricLabelFormatter.DescribeMetric` dengan `”pension_fund_happiness_points”`, `value`, `false`, `true`, `”—”`, `key =>
        // UiText.Translate(language, key)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var result = PlayerMetricLabelFormatter.DescribeMetric(
            // Meneruskan nilai literal `”pension_fund_happiness_points”` sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`; Meneruskan `value`
            // (nilai nilai) sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`; Meneruskan false, yaitu kondisi nonaktif/tidak terpenuhi sebagai
            // argumen ke `PlayerMetricLabelFormatter.DescribeMetric`; Meneruskan true, yaitu kondisi aktif/terpenuhi sebagai argumen ke
            // `PlayerMetricLabelFormatter.DescribeMetric`; Meneruskan nilai literal `”—”` sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`.
            "pension_fund_happiness_points", value, false, true, "—",
            // Parameter `key` bertipe `` membawa nilai kunci.
            key => UiText.Translate(language, key));

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`language == ”id” ? ”Poin Kebahagiaan Dana
        // Pensiun” : ”Pension Happiness Points”`, `result.Label`); pengujian gagal jika keduanya berbeda dalam
        // DescribeMetric_UsesHappinessPointLabelAndUnitWithoutChangingTheValue.
        Assert.Equal(language == "id" ? "Poin Kebahagiaan Dana Pensiun" : "Pension Happiness Points", result.Label);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`value`, `result.DisplayValue`); pengujian
        // gagal jika keduanya berbeda dalam DescribeMetric_UsesHappinessPointLabelAndUnitWithoutChangingTheValue.
        Assert.Equal(value, result.DisplayValue);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`state`, `result.State`); pengujian gagal jika
        // keduanya berbeda dalam DescribeMetric_UsesHappinessPointLabelAndUnitWithoutChangingTheValue.
        Assert.Equal(state, result.State);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`state == ”unavailable” ? string.Empty :
        // language == ”id” ? ”poin kebahagiaan” : ”happiness points”`, `result.Unit`); pengujian gagal jika keduanya berbeda dalam
        // DescribeMetric_UsesHappinessPointLabelAndUnitWithoutChangingTheValue.
        Assert.Equal(state == "unavailable" ? string.Empty :
            // Meneruskan `result.Unit` (nilai unit) sebagai argumen ke `Assert.Equal`.
            language == "id" ? "poin kebahagiaan" : "happiness points", result.Unit);
    // Menutup scope metode DescribeMetric_UsesHappinessPointLabelAndUnitWithoutChangingTheValue; bagian berikut berada di luar batas blok tersebut
    // dalam DescribeMetric_UsesHappinessPointLabelAndUnitWithoutChangingTheValue.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `FormatMetricPathLabel_LocalizesKnownSegmentsAndArrayIndexes` dengan hasil bertipe `void`; operasi ini menangani format
    // metric path label localizes known segments dan array indexes.
    public void FormatMetricPathLabel_LocalizesKnownSegmentsAndArrayIndexes()
    // Membuka scope metode FormatMetricPathLabel_LocalizesKnownSegmentsAndArrayIndexes; pernyataan/deklarasi berikut berada di dalam batas blok ini
    // dalam FormatMetricPathLabel_LocalizesKnownSegmentsAndArrayIndexes.
    {
        // Menyiapkan variabel lokal `label` untuk nilai label dengan memanggil `PlayerMetricLabelFormatter.FormatMetricPathLabel` dengan
        // `”coins.coins_net_end_game.history[1]”`, `Translate`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var label = PlayerMetricLabelFormatter.FormatMetricPathLabel(
            // Meneruskan nilai literal `”coins.coins_net_end_game.history[1]”` sebagai argumen ke `PlayerMetricLabelFormatter.FormatMetricPathLabel`.
            "coins.coins_net_end_game.history[1]",
            // Meneruskan `Translate` (nilai translate) sebagai argumen ke `PlayerMetricLabelFormatter.FormatMetricPathLabel`.
            Translate);

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”Coins - Ending Coins - History - Item 2”`,
        // `label`); pengujian gagal jika keduanya berbeda dalam FormatMetricPathLabel_LocalizesKnownSegmentsAndArrayIndexes.
        Assert.Equal("Coins - Ending Coins - History - Item 2", label);
    // Menutup scope metode FormatMetricPathLabel_LocalizesKnownSegmentsAndArrayIndexes; bagian berikut berada di luar batas blok tersebut dalam
    // FormatMetricPathLabel_LocalizesKnownSegmentsAndArrayIndexes.
    }

    // menandai metode sebagai pengujian xUnit yang dijalankan untuk setiap kombinasi data.
    [Theory]
    // menyediakan satu kombinasi masukan pengujian (”CoinsSpentPerTurn[0].Amount”, ”Outgoing Coins per Event - Item 1 - Amount”).
    [InlineData("CoinsSpentPerTurn[0].Amount", "Outgoing Coins per Event - Item 1 - Amount")]
    // menyediakan satu kombinasi masukan pengujian (”ingredientTypesHeld.White Rice”, ”Ingredient Types Held - White Rice”).
    [InlineData("ingredientTypesHeld.White Rice", "Ingredient Types Held - White Rice")]
    // menyediakan satu kombinasi masukan pengujian (”incomeDiversificationComponents.FreelanceIncome”, ”Income Diversification Components - Freelance
    // Income”).
    [InlineData("incomeDiversificationComponents.FreelanceIncome", "Income Diversification Components - Freelance Income")]
    // Mendefinisikan metode `FormatMetricPathLabel_NormalizesGameplayMetricKeysBeforeLocalization` dengan hasil bertipe `void`; operasi ini menangani
    // format metric path label normalizes gameplay metric kunci before localization. Masukan: Parameter `path` bertipe `string` membawa nilai path;
    // Parameter `expected` bertipe `string` membawa nilai yang diharapkan.
    public void FormatMetricPathLabel_NormalizesGameplayMetricKeysBeforeLocalization(string path, string expected)
    // Membuka scope metode FormatMetricPathLabel_NormalizesGameplayMetricKeysBeforeLocalization; pernyataan/deklarasi berikut berada di dalam batas
    // blok ini dalam FormatMetricPathLabel_NormalizesGameplayMetricKeysBeforeLocalization.
    {
        // Menyiapkan variabel lokal `label` untuk nilai label dengan memanggil `PlayerMetricLabelFormatter.FormatMetricPathLabel` dengan `path`,
        // `Translate`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var label = PlayerMetricLabelFormatter.FormatMetricPathLabel(path, Translate);

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`expected`, `label`); pengujian gagal jika
        // keduanya berbeda dalam FormatMetricPathLabel_NormalizesGameplayMetricKeysBeforeLocalization.
        Assert.Equal(expected, label);
    // Menutup scope metode FormatMetricPathLabel_NormalizesGameplayMetricKeysBeforeLocalization; bagian berikut berada di luar batas blok tersebut
    // dalam FormatMetricPathLabel_NormalizesGameplayMetricKeysBeforeLocalization.
    }

    // menandai metode sebagai pengujian xUnit yang dijalankan untuk setiap kombinasi data.
    [Theory]
    // menyediakan satu kombinasi masukan pengujian (”true”, true, 1).
    [InlineData("true", true, 1)]
    // menyediakan satu kombinasi masukan pengujian (”false”, true, 0).
    [InlineData("false", true, 0)]
    // menyediakan satu kombinasi masukan pengujian (”12.5”, true, 12.5).
    [InlineData("12.5", true, 12.5)]
    // menyediakan satu kombinasi masukan pengujian (”not-a-number”, false, 0).
    [InlineData("not-a-number", false, 0)]
    // Mendefinisikan metode `TryParseMetricNumber_ParsesBooleanAndNumericValues` dengan hasil bertipe `void`; operasi ini menangani try parse metric
    // number parses boolean dan numerik nilai. Masukan: Parameter `rawValue` bertipe `string` membawa nilai raw nilai; Parameter `expectedResult`
    // bertipe `bool` membawa nilai yang diharapkan hasil; Parameter `expectedValue` bertipe `double` membawa nilai yang diharapkan nilai.
    public void TryParseMetricNumber_ParsesBooleanAndNumericValues(string rawValue, bool expectedResult, double expectedValue)
    // Membuka scope metode TryParseMetricNumber_ParsesBooleanAndNumericValues; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // TryParseMetricNumber_ParsesBooleanAndNumericValues.
    {
        // Menyiapkan variabel lokal `previousCulture` untuk nilai previous culture dengan `CultureInfo.CurrentCulture` (nilai saat ini culture). Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var previousCulture = CultureInfo.CurrentCulture;
        // Memulai blok try dalam TryParseMetricNumber_ParsesBooleanAndNumericValues; exception dari blok ini dapat dialihkan ke catch, sedangkan finally
        // (jika ada) tetap dijalankan saat keluar.
        try
        // Membuka scope penanganan operasi try; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // TryParseMetricNumber_ParsesBooleanAndNumericValues.
        {
            // Memperbarui `CultureInfo.CurrentCulture` menggunakan memanggil `CultureInfo.GetCultureInfo` dengan `”id-ID”` dalam
            // TryParseMetricNumber_ParsesBooleanAndNumericValues.
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("id-ID");

            // Menyiapkan variabel lokal `result` untuk nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya dengan memanggil
            // `PlayerMetricLabelFormatter.TryParseMetricNumber` dengan `rawValue`, `var value`. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var result = PlayerMetricLabelFormatter.TryParseMetricNumber(rawValue, out var value);

            // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`expectedResult`, `result`); pengujian gagal
            // jika keduanya berbeda dalam TryParseMetricNumber_ParsesBooleanAndNumericValues.
            Assert.Equal(expectedResult, result);
            // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`expectedValue`, `value`); pengujian gagal jika
            // keduanya berbeda dalam TryParseMetricNumber_ParsesBooleanAndNumericValues.
            Assert.Equal(expectedValue, value);
        // Menutup scope penanganan operasi try; bagian berikut berada di luar batas blok tersebut dalam TryParseMetricNumber_ParsesBooleanAndNumericValues.
        }
        // Menjalankan blok finally ketika alur meninggalkan try/catch, termasuk saat terjadi exception dalam
        // TryParseMetricNumber_ParsesBooleanAndNumericValues; bagian ini dipakai untuk pekerjaan penutup yang harus tetap dilakukan.
        finally
        // Membuka scope pekerjaan penutup finally; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // TryParseMetricNumber_ParsesBooleanAndNumericValues.
        {
            // Memperbarui `CultureInfo.CurrentCulture` menggunakan `previousCulture` (nilai previous culture) dalam
            // TryParseMetricNumber_ParsesBooleanAndNumericValues.
            CultureInfo.CurrentCulture = previousCulture;
        // Menutup scope pekerjaan penutup finally; bagian berikut berada di luar batas blok tersebut dalam
        // TryParseMetricNumber_ParsesBooleanAndNumericValues.
        }
    // Menutup scope metode TryParseMetricNumber_ParsesBooleanAndNumericValues; bagian berikut berada di luar batas blok tersebut dalam
    // TryParseMetricNumber_ParsesBooleanAndNumericValues.
    }

    // menandai metode sebagai pengujian xUnit yang dijalankan untuk setiap kombinasi data.
    [Theory]
    // menyediakan satu kombinasi masukan pengujian (”summary”, true, true).
    [InlineData("summary", true, true)]
    // menyediakan satu kombinasi masukan pengujian (”coins.total”, false, true).
    [InlineData("coins.total", false, true)]
    // menyediakan satu kombinasi masukan pengujian (”coins.history[0]”, false, false).
    [InlineData("coins.history[0]", false, false)]
    // Mendefinisikan metode `CombinedSummaryPathFilters_ClassifyCompactRows` dengan hasil bertipe `void`; operasi ini menangani combined summary path
    // filters classify compact baris. Masukan: Parameter `path` bertipe `string` membawa nilai path; Parameter `expectedPreferred` bertipe `bool`
    // membawa nilai yang diharapkan preferred; Parameter `expectedFallback` bertipe `bool` membawa nilai yang diharapkan fallback.
    public void CombinedSummaryPathFilters_ClassifyCompactRows(string path, bool expectedPreferred, bool expectedFallback)
    // Membuka scope metode CombinedSummaryPathFilters_ClassifyCompactRows; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // CombinedSummaryPathFilters_ClassifyCompactRows.
    {
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`expectedPreferred`,
        // `PlayerMetricLabelFormatter.IsPreferredCombinedSummaryPath(path)`); pengujian gagal jika keduanya berbeda dalam
        // CombinedSummaryPathFilters_ClassifyCompactRows.
        Assert.Equal(expectedPreferred, PlayerMetricLabelFormatter.IsPreferredCombinedSummaryPath(path));
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`expectedFallback`,
        // `PlayerMetricLabelFormatter.IsFallbackCombinedSummaryPath(path)`); pengujian gagal jika keduanya berbeda dalam
        // CombinedSummaryPathFilters_ClassifyCompactRows.
        Assert.Equal(expectedFallback, PlayerMetricLabelFormatter.IsFallbackCombinedSummaryPath(path));
    // Menutup scope metode CombinedSummaryPathFilters_ClassifyCompactRows; bagian berikut berada di luar batas blok tersebut dalam
    // CombinedSummaryPathFilters_ClassifyCompactRows.
    }

    // menandai metode sebagai pengujian xUnit yang dijalankan untuk setiap kombinasi data.
    [Theory]
    // menyediakan satu kombinasi masukan pengujian (”transaction_history.coins_in_event”).
    [InlineData("transaction_history.coins_in_event")]
    // menyediakan satu kombinasi masukan pengujian (”transaction_history.coins_out_event”).
    [InlineData("transaction_history.coins_out_event")]
    // menyediakan satu kombinasi masukan pengujian (”transaction_history.coin_change”).
    [InlineData("transaction_history.coin_change")]
    // menyediakan satu kombinasi masukan pengujian (”transaction_history.coin_balance_after_event”).
    [InlineData("transaction_history.coin_balance_after_event")]
    // Mendefinisikan metode `DescribeMetric_UsesCoinUnitForComprehensiveTransactionColumns` dengan hasil bertipe `void`; operasi ini menangani describe
    // metric uses coin unit untuk comprehensive transaction columns. Masukan: Parameter `path` bertipe `string` membawa nilai path.
    public void DescribeMetric_UsesCoinUnitForComprehensiveTransactionColumns(string path)
    // Membuka scope metode DescribeMetric_UsesCoinUnitForComprehensiveTransactionColumns; pernyataan/deklarasi berikut berada di dalam batas blok ini
    // dalam DescribeMetric_UsesCoinUnitForComprehensiveTransactionColumns.
    {
        // Menyiapkan variabel lokal `result` untuk nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya dengan memanggil
        // `PlayerMetricLabelFormatter.DescribeMetric` dengan `path`, `”10”`, `false`, `true`, `”Unavailable”`, `key => key`. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var result = PlayerMetricLabelFormatter.DescribeMetric(
            // Meneruskan `path` (nilai path) sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`.
            path,
            // Meneruskan nilai literal `”10”` sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`.
            "10",
            // Meneruskan false, yaitu kondisi nonaktif/tidak terpenuhi sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`.
            false,
            // Meneruskan true, yaitu kondisi aktif/terpenuhi sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`.
            true,
            // Meneruskan nilai literal `”Unavailable”` sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`.
            "Unavailable",
            // Parameter `key` bertipe `` membawa nilai kunci.
            key => key);

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”players.support.unit.coins”`, `result.Unit`);
        // pengujian gagal jika keduanya berbeda dalam DescribeMetric_UsesCoinUnitForComprehensiveTransactionColumns.
        Assert.Equal("players.support.unit.coins", result.Unit);
    // Menutup scope metode DescribeMetric_UsesCoinUnitForComprehensiveTransactionColumns; bagian berikut berada di luar batas blok tersebut dalam
    // DescribeMetric_UsesCoinUnitForComprehensiveTransactionColumns.
    }

    // menandai metode sebagai pengujian xUnit yang dijalankan untuk setiap kombinasi data.
    [Theory]
    // menyediakan satu kombinasi masukan pengujian (”active_income_source_count”, ”players.support.unit.sources”).
    [InlineData("active_income_source_count", "players.support.unit.sources")]
    // menyediakan satu kombinasi masukan pengujian (”income_main_actions”, ”players.support.unit.actions”).
    [InlineData("income_main_actions", "players.support.unit.action_tokens")]
    // menyediakan satu kombinasi masukan pengujian (”loan_repayment_actions”, ”players.support.unit.actions”).
    [InlineData("loan_repayment_actions", "players.support.unit.action_tokens")]
    [InlineData("total_main_actions", "players.support.unit.action_tokens")]
    [InlineData("saving_and_goal_actions", "players.support.unit.action_tokens")]
    [InlineData("action_usage_history.total_actions", "players.support.unit.action_tokens")]
    [InlineData("action_usage_history.distinct_actions", "players.support.unit.actions")]
    // menyediakan satu kombinasi masukan pengujian (”outstanding_loan”, ”players.support.unit.coins”).
    [InlineData("outstanding_loan", "players.support.unit.coins")]
    // menyediakan satu kombinasi masukan pengujian (”attempted_goal_target_total”, ”players.support.unit.coins”).
    [InlineData("attempted_goal_target_total", "players.support.unit.coins")]
    // menyediakan satu kombinasi masukan pengujian (”risks_resolved_without_emergency”, ”players.support.unit.risk_events”).
    [InlineData("risks_resolved_without_emergency", "players.support.unit.risk_events")]
    // Mendefinisikan metode `DescribeMetric_UsesThePhysicalUnitOfDerivedInputs` dengan hasil bertipe `void`; operasi ini menangani describe metric uses
    // the physical unit of derived inputs. Masukan: Parameter `path` bertipe `string` membawa nilai path; Parameter `expectedUnit` bertipe `string`
    // membawa nilai yang diharapkan unit.
    public void DescribeMetric_UsesThePhysicalUnitOfDerivedInputs(string path, string expectedUnit)
    // Membuka scope metode DescribeMetric_UsesThePhysicalUnitOfDerivedInputs; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // DescribeMetric_UsesThePhysicalUnitOfDerivedInputs.
    {
        // Menyiapkan variabel lokal `result` untuk nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya dengan memanggil
        // `PlayerMetricLabelFormatter.DescribeMetric` dengan `path`, `”3”`, `true`, `true`, `”Unavailable”`, `key => key`. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var result = PlayerMetricLabelFormatter.DescribeMetric(
            // Meneruskan `path` (nilai path) sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`.
            path,
            // Meneruskan nilai literal `”3”` sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`.
            "3",
            // Meneruskan true, yaitu kondisi aktif/terpenuhi sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`.
            true,
            // Meneruskan true, yaitu kondisi aktif/terpenuhi sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`.
            true,
            // Meneruskan nilai literal `”Unavailable”` sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`.
            "Unavailable",
            // Parameter `key` bertipe `` membawa nilai kunci.
            key => key);

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`expectedUnit`, `result.Unit`); pengujian gagal
        // jika keduanya berbeda dalam DescribeMetric_UsesThePhysicalUnitOfDerivedInputs.
        Assert.Equal(expectedUnit, result.Unit);
    // Menutup scope metode DescribeMetric_UsesThePhysicalUnitOfDerivedInputs; bagian berikut berada di luar batas blok tersebut dalam
    // DescribeMetric_UsesThePhysicalUnitOfDerivedInputs.
    }

    // menandai metode sebagai pengujian xUnit yang dijalankan untuk setiap kombinasi data.
    [Theory]
    // menyediakan satu kombinasi masukan pengujian (”coins_saved”, false, ”savings”).
    [InlineData("coins_saved", false, "savings")]
    // menyediakan satu kombinasi masukan pengujian (”cash_in_total”, false, ”income”).
    [InlineData("cash_in_total", false, "income")]
    // menyediakan satu kombinasi masukan pengujian (”coins_donated”, false, ”donation”).
    [InlineData("coins_donated", false, "donation")]
    // menyediakan satu kombinasi masukan pengujian (”donation_events”, false, ”donation”).
    [InlineData("donation_events", false, "donation")]
    // menyediakan satu kombinasi masukan pengujian (”action_sequence[0].action_type”, false, ”timeline”).
    [InlineData("action_sequence[0].action_type", false, "timeline")]
    // menyediakan satu kombinasi masukan pengujian (”sharia_loans_unpaid_end”, false, ”debt”).
    [InlineData("sharia_loans_unpaid_end", false, "debt")]
    // menyediakan satu kombinasi masukan pengujian (”average_risk_cost”, true, ”risk”).
    [InlineData("average_risk_cost", true, "risk")]
    // Mendefinisikan metode `DescribeMetric_UsesTheDataFunctionInsteadOfAGenericUnitDescription` dengan hasil bertipe `void`; operasi ini menangani
    // describe metric uses the data function instead of a generic unit description. Masukan: Parameter `path` bertipe `string` membawa nilai path;
    // Parameter `isDerived` bertipe `bool` membawa nilai berstatus derived; Parameter `expectedCategory` bertipe `string` membawa nilai yang diharapkan
    // category.
    public void DescribeMetric_UsesTheDataFunctionInsteadOfAGenericUnitDescription(
        // Parameter `path` bertipe `string` membawa nilai path.
        string path,
        // Parameter `isDerived` bertipe `bool` membawa nilai berstatus derived.
        bool isDerived,
        // Parameter `expectedCategory` bertipe `string` membawa nilai yang diharapkan category.
        string expectedCategory)
    // Membuka scope metode DescribeMetric_UsesTheDataFunctionInsteadOfAGenericUnitDescription; pernyataan/deklarasi berikut berada di dalam batas blok
    // ini dalam DescribeMetric_UsesTheDataFunctionInsteadOfAGenericUnitDescription.
    {
        // Menyiapkan variabel lokal `result` untuk nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya dengan memanggil
        // `PlayerMetricLabelFormatter.DescribeMetric` dengan `path`, `”5”`, `isDerived`, `true`, `”Unavailable”`, `key => key`. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var result = PlayerMetricLabelFormatter.DescribeMetric(
            // Meneruskan `path` (nilai path) sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`.
            path,
            // Meneruskan nilai literal `”5”` sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`.
            "5",
            // Meneruskan `isDerived` (nilai berstatus derived) sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`.
            isDerived,
            // Meneruskan true, yaitu kondisi aktif/terpenuhi sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`.
            true,
            // Meneruskan nilai literal `”Unavailable”` sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`.
            "Unavailable",
            // Parameter `key` bertipe `` membawa nilai kunci.
            key => key);

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui
        // Assert.Equal(`$”players.support.meaning.{expectedCategory}”`, `result.Explanation`); pengujian gagal jika keduanya berbeda dalam
        // DescribeMetric_UsesTheDataFunctionInsteadOfAGenericUnitDescription.
        Assert.Equal($"players.support.meaning.{expectedCategory}", result.Explanation);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`$”players.support.guide.{expectedCategory}”`,
        // `result.Guidance`); pengujian gagal jika keduanya berbeda dalam DescribeMetric_UsesTheDataFunctionInsteadOfAGenericUnitDescription.
        Assert.Equal($"players.support.guide.{expectedCategory}", result.Guidance);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui
        // Assert.Equal(`$”players.support.recommendation.{expectedCategory}”`, `result.Recommendation`); pengujian gagal jika keduanya berbeda dalam
        // DescribeMetric_UsesTheDataFunctionInsteadOfAGenericUnitDescription.
        Assert.Equal($"players.support.recommendation.{expectedCategory}", result.Recommendation);
    // Menutup scope metode DescribeMetric_UsesTheDataFunctionInsteadOfAGenericUnitDescription; bagian berikut berada di luar batas blok tersebut dalam
    // DescribeMetric_UsesTheDataFunctionInsteadOfAGenericUnitDescription.
    }

    // menandai metode sebagai pengujian xUnit yang dijalankan untuk setiap kombinasi data.
    [Theory]
    // menyediakan satu kombinasi masukan pengujian (”ingredients_used_per_meal”, ”ingredients_per_order”).
    [InlineData("ingredients_used_per_meal", "ingredients_per_order")]
    // menyediakan satu kombinasi masukan pengujian (”meal_order_income_per_order”, ”income_per_order”).
    [InlineData("meal_order_income_per_order", "income_per_order")]
    // Mendefinisikan metode `DescribeMetric_ExplainsPerOrderSeries` dengan hasil bertipe `void`; operasi ini menangani describe metric explains per
    // urutan/pesanan series. Masukan: Parameter `path` bertipe `string` membawa nilai path; Parameter `explanation` bertipe `string` membawa nilai
    // explanation.
    public void DescribeMetric_ExplainsPerOrderSeries(string path, string explanation)
    // Membuka scope metode DescribeMetric_ExplainsPerOrderSeries; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // DescribeMetric_ExplainsPerOrderSeries.
    {
        // Menyiapkan variabel lokal `result` untuk nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya dengan memanggil
        // `PlayerMetricLabelFormatter.DescribeMetric` dengan `path`, `”[2,3]”`, `false`, `true`, `”Unavailable”`, `key => key`. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var result = PlayerMetricLabelFormatter.DescribeMetric(
            // Meneruskan `path` (nilai path) sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`; Meneruskan nilai literal `”[2,3]”` sebagai argumen
            // ke `PlayerMetricLabelFormatter.DescribeMetric`; Meneruskan false, yaitu kondisi nonaktif/tidak terpenuhi sebagai argumen ke
            // `PlayerMetricLabelFormatter.DescribeMetric`; Meneruskan true, yaitu kondisi aktif/terpenuhi sebagai argumen ke
            // `PlayerMetricLabelFormatter.DescribeMetric`; Meneruskan nilai literal `”Unavailable”` sebagai argumen ke
            // `PlayerMetricLabelFormatter.DescribeMetric`; Meneruskan fungsi lambda `key => key` yang dijalankan oleh operasi pemanggil untuk memproses setiap
            // masukan sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`.
            path, "[2,3]", false, true, "Unavailable", key => key);

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`$”players.support.meaning.{explanation}”`,
        // `result.Explanation`); pengujian gagal jika keduanya berbeda dalam DescribeMetric_ExplainsPerOrderSeries.
        Assert.Equal($"players.support.meaning.{explanation}", result.Explanation);
    // Menutup scope metode DescribeMetric_ExplainsPerOrderSeries; bagian berikut berada di luar batas blok tersebut dalam
    // DescribeMetric_ExplainsPerOrderSeries.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `DescribeMetric_ExplainsDiscardedIngredientsSeparatelyFromRemainingInventory` dengan hasil bertipe `void`; operasi ini
    // menangani describe metric explains discarded bahan separately dari tersisa inventory.
    public void DescribeMetric_ExplainsDiscardedIngredientsSeparatelyFromRemainingInventory()
    // Membuka scope metode DescribeMetric_ExplainsDiscardedIngredientsSeparatelyFromRemainingInventory; pernyataan/deklarasi berikut berada di dalam
    // batas blok ini dalam DescribeMetric_ExplainsDiscardedIngredientsSeparatelyFromRemainingInventory.
    {
        // Menyiapkan variabel lokal `result` untuk nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya dengan memanggil
        // `PlayerMetricLabelFormatter.DescribeMetric` dengan `”ingredients_wasted”`, `”2”`, `false`, `true`, `”Unavailable”`, `key => key`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var result = PlayerMetricLabelFormatter.DescribeMetric(
            // Meneruskan nilai literal `”ingredients_wasted”` sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`.
            "ingredients_wasted",
            // Meneruskan nilai literal `”2”` sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`.
            "2",
            // Meneruskan false, yaitu kondisi nonaktif/tidak terpenuhi sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`.
            false,
            // Meneruskan true, yaitu kondisi aktif/terpenuhi sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`.
            true,
            // Meneruskan nilai literal `”Unavailable”` sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`.
            "Unavailable",
            // Parameter `key` bertipe `` membawa nilai kunci.
            key => key);

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui
        // Assert.Equal(`”players.support.meaning.ingredients_discarded”`, `result.Explanation`); pengujian gagal jika keduanya berbeda dalam
        // DescribeMetric_ExplainsDiscardedIngredientsSeparatelyFromRemainingInventory.
        Assert.Equal("players.support.meaning.ingredients_discarded", result.Explanation);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”players.support.guide.inventory”`,
        // `result.Guidance`); pengujian gagal jika keduanya berbeda dalam DescribeMetric_ExplainsDiscardedIngredientsSeparatelyFromRemainingInventory.
        Assert.Equal("players.support.guide.inventory", result.Guidance);
    // Menutup scope metode DescribeMetric_ExplainsDiscardedIngredientsSeparatelyFromRemainingInventory; bagian berikut berada di luar batas blok
    // tersebut dalam DescribeMetric_ExplainsDiscardedIngredientsSeparatelyFromRemainingInventory.
    }

    // menandai metode sebagai pengujian xUnit yang dijalankan untuk setiap kombinasi data.
    [Theory]
    // menyediakan satu kombinasi masukan pengujian (”net_worth_index”, ”players.support.recommendation.cash_recover”).
    [InlineData("net_worth_index", "players.support.recommendation.cash_recover")]
    // menyediakan satu kombinasi masukan pengujian (”income_diversification_ratio”, ”players.support.recommendation.income_mix_recover”).
    [InlineData("income_diversification_ratio", "players.support.recommendation.income_mix_recover")]
    // menyediakan satu kombinasi masukan pengujian (”insurance_coverage_rate”, ”players.support.recommendation.protection”).
    [InlineData("insurance_coverage_rate", "players.support.recommendation.protection")]
    // menyediakan satu kombinasi masukan pengujian (”friday_participation_rate”, ”players.support.recommendation.donation”).
    [InlineData("friday_participation_rate", "players.support.recommendation.donation")]
    // menyediakan satu kombinasi masukan pengujian (”planning_horizon”, ”players.support.recommendation.planning”).
    [InlineData("planning_horizon", "players.support.recommendation.planning")]
    // menyediakan satu kombinasi masukan pengujian (”unknown_result”, ”players.support.recommendation.balance”).
    [InlineData("unknown_result", "players.support.recommendation.balance")]
    // Mendefinisikan metode `DescribeMetric_AddsARelevantFinancialLiteracyConsideration` dengan hasil bertipe `void`; operasi ini menangani describe
    // metric adds a relevant keuangan literacy consideration. Masukan: Parameter `path` bertipe `string` membawa nilai path; Parameter
    // `expectedRecommendation` bertipe `string` membawa nilai yang diharapkan recommendation.
    public void DescribeMetric_AddsARelevantFinancialLiteracyConsideration(
        // Parameter `path` bertipe `string` membawa nilai path.
        string path,
        // Parameter `expectedRecommendation` bertipe `string` membawa nilai yang diharapkan recommendation.
        string expectedRecommendation)
    // Membuka scope metode DescribeMetric_AddsARelevantFinancialLiteracyConsideration; pernyataan/deklarasi berikut berada di dalam batas blok ini
    // dalam DescribeMetric_AddsARelevantFinancialLiteracyConsideration.
    {
        // Menyiapkan variabel lokal `result` untuk nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya dengan memanggil
        // `PlayerMetricLabelFormatter.DescribeMetric` dengan `path`, `”5”`, `true`, `true`, `”Unavailable”`, `key => key`. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var result = PlayerMetricLabelFormatter.DescribeMetric(
            // Meneruskan `path` (nilai path) sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`.
            path,
            // Meneruskan nilai literal `”5”` sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`.
            "5",
            // Meneruskan true, yaitu kondisi aktif/terpenuhi sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`.
            true,
            // Meneruskan true, yaitu kondisi aktif/terpenuhi sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`.
            true,
            // Meneruskan nilai literal `”Unavailable”` sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`.
            "Unavailable",
            // Parameter `key` bertipe `` membawa nilai kunci.
            key => key);

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`expectedRecommendation`,
        // `result.Recommendation`); pengujian gagal jika keduanya berbeda dalam DescribeMetric_AddsARelevantFinancialLiteracyConsideration.
        Assert.Equal(expectedRecommendation, result.Recommendation);
    // Menutup scope metode DescribeMetric_AddsARelevantFinancialLiteracyConsideration; bagian berikut berada di luar batas blok tersebut dalam
    // DescribeMetric_AddsARelevantFinancialLiteracyConsideration.
    }

    // menandai metode sebagai pengujian xUnit yang dijalankan untuk setiap kombinasi data.
    [Theory]
    // menyediakan satu kombinasi masukan pengujian (”net_worth_index”, ”120”, ”players.support.recommendation.wealth”).
    [InlineData("net_worth_index", "120", "players.support.recommendation.wealth")]
    // menyediakan satu kombinasi masukan pengujian (”business_profit_margin”, ”-1”, ”players.support.recommendation.business_recover”).
    [InlineData("business_profit_margin", "-1", "players.support.recommendation.business_recover")]
    // menyediakan satu kombinasi masukan pengujian (”business_profit_margin”, ”10”, ”players.support.recommendation.business_activity”).
    [InlineData("business_profit_margin", "10", "players.support.recommendation.business_activity")]
    // menyediakan satu kombinasi masukan pengujian (”action_efficiency_percent”, ”20”, ”players.support.recommendation.action_income”).
    [InlineData("action_efficiency_percent", "20", "players.support.recommendation.action_income")]
    // menyediakan satu kombinasi masukan pengujian (”action_efficiency_percent”, ”60”, ”players.support.recommendation.action_balance”).
    [InlineData("action_efficiency_percent", "60", "players.support.recommendation.action_balance")]
    // menyediakan satu kombinasi masukan pengujian (”fulfillment_diversity”, ”0.2”, ”players.support.recommendation.needs_balance”).
    [InlineData("fulfillment_diversity", "0.2", "players.support.recommendation.needs_balance")]
    // menyediakan satu kombinasi masukan pengujian (”donation_commitment_score”, ”20”, ”players.support.recommendation.donation_stabilize”).
    [InlineData("donation_commitment_score", "20", "players.support.recommendation.donation_stabilize")]
    // Mendefinisikan metode `DescribeMetric_AdaptsTheConsiderationToTheResult` dengan hasil bertipe `void`; operasi ini menangani describe metric
    // adapts the consideration ke the hasil. Masukan: Parameter `path` bertipe `string` membawa nilai path; Parameter `value` bertipe `string` membawa
    // nilai nilai; Parameter `expectedRecommendation` bertipe `string` membawa nilai yang diharapkan recommendation.
    public void DescribeMetric_AdaptsTheConsiderationToTheResult(
        // Parameter `path` bertipe `string` membawa nilai path.
        string path,
        // Parameter `value` bertipe `string` membawa nilai nilai.
        string value,
        // Parameter `expectedRecommendation` bertipe `string` membawa nilai yang diharapkan recommendation.
        string expectedRecommendation)
    // Membuka scope metode DescribeMetric_AdaptsTheConsiderationToTheResult; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // DescribeMetric_AdaptsTheConsiderationToTheResult.
    {
        // Menyiapkan variabel lokal `result` untuk nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya dengan memanggil
        // `PlayerMetricLabelFormatter.DescribeMetric` dengan `path`, `value`, `true`, `true`, `”Unavailable”`, `key => key`. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var result = PlayerMetricLabelFormatter.DescribeMetric(
            // Meneruskan `path` (nilai path) sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`.
            path,
            // Meneruskan `value` (nilai nilai) sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`.
            value,
            // Meneruskan true, yaitu kondisi aktif/terpenuhi sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`.
            true,
            // Meneruskan true, yaitu kondisi aktif/terpenuhi sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`.
            true,
            // Meneruskan nilai literal `”Unavailable”` sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`.
            "Unavailable",
            // Parameter `key` bertipe `` membawa nilai kunci.
            key => key);

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`expectedRecommendation`,
        // `result.Recommendation`); pengujian gagal jika keduanya berbeda dalam DescribeMetric_AdaptsTheConsiderationToTheResult.
        Assert.Equal(expectedRecommendation, result.Recommendation);
    // Menutup scope metode DescribeMetric_AdaptsTheConsiderationToTheResult; bagian berikut berada di luar batas blok tersebut dalam
    // DescribeMetric_AdaptsTheConsiderationToTheResult.
    }

    // menandai metode sebagai pengujian xUnit yang dijalankan untuk setiap kombinasi data.
    [Theory]
    // menyediakan satu kombinasi masukan pengujian (”expense_management_efficiency”, ”50”, ”players.support.guide.expense_efficiency”).
    [InlineData("expense_management_efficiency", "50", "players.support.guide.expense_efficiency")]
    // menyediakan satu kombinasi masukan pengujian (”income_diversification_ratio”, ”50”, ”players.support.guide.income_mixed”).
    [InlineData("income_diversification_ratio", "50", "players.support.guide.income_mixed")]
    // menyediakan satu kombinasi masukan pengujian (”planning_horizon”, ”0.5”, ”players.support.guide.planning_long”).
    [InlineData("planning_horizon", "0.5", "players.support.guide.planning_long")]
    // Mendefinisikan metode `DescribeMetric_UsesFunctionalGuidanceForDerivedMetrics` dengan hasil bertipe `void`; operasi ini menangani describe metric
    // uses functional guidance untuk derived metrics. Masukan: Parameter `path` bertipe `string` membawa nilai path; Parameter `value` bertipe `string`
    // membawa nilai nilai; Parameter `expectedGuidance` bertipe `string` membawa nilai yang diharapkan guidance.
    public void DescribeMetric_UsesFunctionalGuidanceForDerivedMetrics(
        // Parameter `path` bertipe `string` membawa nilai path.
        string path,
        // Parameter `value` bertipe `string` membawa nilai nilai.
        string value,
        // Parameter `expectedGuidance` bertipe `string` membawa nilai yang diharapkan guidance.
        string expectedGuidance)
    // Membuka scope metode DescribeMetric_UsesFunctionalGuidanceForDerivedMetrics; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // DescribeMetric_UsesFunctionalGuidanceForDerivedMetrics.
    {
        // Menyiapkan variabel lokal `result` untuk nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya dengan memanggil
        // `PlayerMetricLabelFormatter.DescribeMetric` dengan `path`, `value`, `true`, `true`, `”Unavailable”`, `key => key`. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var result = PlayerMetricLabelFormatter.DescribeMetric(
            // Meneruskan `path` (nilai path) sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`.
            path,
            // Meneruskan `value` (nilai nilai) sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`.
            value,
            // Meneruskan true, yaitu kondisi aktif/terpenuhi sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`.
            true,
            // Meneruskan true, yaitu kondisi aktif/terpenuhi sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`.
            true,
            // Meneruskan nilai literal `”Unavailable”` sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`.
            "Unavailable",
            // Parameter `key` bertipe `` membawa nilai kunci.
            key => key);

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`expectedGuidance`, `result.Guidance`);
        // pengujian gagal jika keduanya berbeda dalam DescribeMetric_UsesFunctionalGuidanceForDerivedMetrics.
        Assert.Equal(expectedGuidance, result.Guidance);
    // Menutup scope metode DescribeMetric_UsesFunctionalGuidanceForDerivedMetrics; bagian berikut berada di luar batas blok tersebut dalam
    // DescribeMetric_UsesFunctionalGuidanceForDerivedMetrics.
    }

    // menandai metode sebagai pengujian xUnit yang dijalankan untuk setiap kombinasi data.
    [Theory]
    // menyediakan satu kombinasi masukan pengujian (”action_efficiency”, ”0.45”, ”45”, ”players.support.unit.percent”).
    [InlineData("action_efficiency", "0.45", "45", "players.support.unit.percent")]
    // menyediakan satu kombinasi masukan pengujian (”growth_pattern_ratio”, ”1.5”, ”1.50”, ”players.support.unit.multiplier”).
    [InlineData("growth_pattern_ratio", "1.5", "1.50", "players.support.unit.multiplier")]
    // menyediakan satu kombinasi masukan pengujian (”meal_orders_per_turn_average”, ”0.25”, ”0.25”, ”players.support.unit.orders_per_turn”).
    [InlineData("meal_orders_per_turn_average", "0.25", "0.25", "players.support.unit.orders_per_turn")]
    // menyediakan satu kombinasi masukan pengujian (”ingredients_used_per_meal_average”, ”2”, ”2”, ”players.support.unit.ingredients_per_order”).
    [InlineData("ingredients_used_per_meal_average", "2", "2", "players.support.unit.ingredients_per_order")]
    // menyediakan satu kombinasi masukan pengujian (”donation_stability_std_deviation”, ”1.2”, ”1.20”, ”players.support.unit.coins”).
    [InlineData("donation_stability_std_deviation", "1.2", "1.20", "players.support.unit.coins")]
    // menyediakan satu kombinasi masukan pengujian (”action_repetitions_per_turn[0].diversity_score”, ”0.5”, ”50”, ”players.support.unit.percent”).
    [InlineData("action_repetitions_per_turn[0].diversity_score", "0.5", "50", "players.support.unit.percent")]
    // menyediakan satu kombinasi masukan pengujian (”coins_spent_per_turn[0].action_slot”, ”2”, ”2”, ”players.support.unit.action_slot”).
    [InlineData("coins_spent_per_turn[0].action_slot", "2", "2", "players.support.unit.action_slot")]
    // menyediakan satu kombinasi masukan pengujian (”coins_spent_per_turn[0].amount”, ”12”, ”12”, ”players.support.unit.coins”).
    [InlineData("coins_spent_per_turn[0].amount", "12", "12", "players.support.unit.coins")]
    // menyediakan satu kombinasi masukan pengujian (”N_active_income_sources”, ”3”, ”3”, ”players.support.unit.sources”).
    [InlineData("N_active_income_sources", "3", "3", "players.support.unit.sources")]
    // menyediakan satu kombinasi masukan pengujian (”goal_ambition_index”, ”62.5”, ”62.50”, ”players.support.unit.percent”).
    [InlineData("goal_ambition_index", "62.5", "62.50", "players.support.unit.percent")]
    // menyediakan satu kombinasi masukan pengujian (”goal_setting_ambition”, ”255.33”, ”255.33”, ””).
    [InlineData("goal_setting_ambition", "255.33", "255.33", "")]
    // menyediakan satu kombinasi masukan pengujian (”fulfillment_diversity_document_formula”, ”0.625”, ”62.50”, ”players.support.unit.percent”).
    [InlineData("fulfillment_diversity_document_formula", "0.625", "62.50", "players.support.unit.percent")]
    // menyediakan satu kombinasi masukan pengujian (”donation_stability_index”, ”88.5”, ”88.50”, ”players.support.unit.percent”).
    [InlineData("donation_stability_index", "88.5", "88.50", "players.support.unit.percent")]
    // menyediakan satu kombinasi masukan pengujian (”income_producing_actions”, ”10”, ”10”, ”players.support.unit.actions”).
    [InlineData("income_producing_actions", "10", "10", "players.support.unit.action_tokens")]
    // menyediakan satu kombinasi masukan pengujian (”planning_horizon_components.financial_goal_actions”, ”0”, ”0”, ”players.support.unit.actions”).
    [InlineData("planning_horizon_components.financial_goal_actions", "0", "0", "players.support.unit.action_tokens")]
    // menyediakan satu kombinasi masukan pengujian (”financial_goals_balance_per_goal.tujuan_35”, ”3”, ”3”, ”players.support.unit.coins”).
    [InlineData("financial_goals_balance_per_goal.tujuan_35", "3", "3", "players.support.unit.coins")]
    // Mendefinisikan metode `DescribeMetric_UsesDimensionallyCorrectDisplayValuesAndUnits` dengan hasil bertipe `void`; operasi ini menangani describe
    // metric uses dimensionally correct display nilai dan units. Masukan: Parameter `path` bertipe `string` membawa nilai path; Parameter `value`
    // bertipe `string` membawa nilai nilai; Parameter `expectedDisplay` bertipe `string` membawa nilai yang diharapkan display; Parameter
    // `expectedUnit` bertipe `string` membawa nilai yang diharapkan unit.
    public void DescribeMetric_UsesDimensionallyCorrectDisplayValuesAndUnits(
        // Parameter `path` bertipe `string` membawa nilai path.
        string path,
        // Parameter `value` bertipe `string` membawa nilai nilai.
        string value,
        // Parameter `expectedDisplay` bertipe `string` membawa nilai yang diharapkan display.
        string expectedDisplay,
        // Parameter `expectedUnit` bertipe `string` membawa nilai yang diharapkan unit.
        string expectedUnit)
    // Membuka scope metode DescribeMetric_UsesDimensionallyCorrectDisplayValuesAndUnits; pernyataan/deklarasi berikut berada di dalam batas blok ini
    // dalam DescribeMetric_UsesDimensionallyCorrectDisplayValuesAndUnits.
    {
        // Menyiapkan variabel lokal `previousCulture` untuk nilai previous culture dengan `CultureInfo.CurrentCulture` (nilai saat ini culture). Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var previousCulture = CultureInfo.CurrentCulture;
        // Memulai blok try dalam DescribeMetric_UsesDimensionallyCorrectDisplayValuesAndUnits; exception dari blok ini dapat dialihkan ke catch, sedangkan
        // finally (jika ada) tetap dijalankan saat keluar.
        try
        // Membuka scope penanganan operasi try; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // DescribeMetric_UsesDimensionallyCorrectDisplayValuesAndUnits.
        {
            // Memperbarui `CultureInfo.CurrentCulture` menggunakan `CultureInfo.InvariantCulture` (nilai invariant culture) dalam
            // DescribeMetric_UsesDimensionallyCorrectDisplayValuesAndUnits.
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            // Menyiapkan variabel lokal `result` untuk nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya dengan memanggil
            // `PlayerMetricLabelFormatter.DescribeMetric` dengan `path`, `value`, `true`, `true`, `”Unavailable”`, `key => key`. Tipe variabel disimpulkan dari
            // ekspresi nilai awal.
            var result = PlayerMetricLabelFormatter.DescribeMetric(
                // Meneruskan `path` (nilai path) sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`.
                path,
                // Meneruskan `value` (nilai nilai) sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`.
                value,
                // Meneruskan true, yaitu kondisi aktif/terpenuhi sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`.
                true,
                // Meneruskan true, yaitu kondisi aktif/terpenuhi sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`.
                true,
                // Meneruskan nilai literal `”Unavailable”` sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`.
                "Unavailable",
                // Parameter `key` bertipe `` membawa nilai kunci.
                key => key);

            // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`expectedDisplay`, `result.DisplayValue`);
            // pengujian gagal jika keduanya berbeda dalam DescribeMetric_UsesDimensionallyCorrectDisplayValuesAndUnits.
            Assert.Equal(expectedDisplay, result.DisplayValue);
            // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`expectedUnit`, `result.Unit`); pengujian gagal
            // jika keduanya berbeda dalam DescribeMetric_UsesDimensionallyCorrectDisplayValuesAndUnits.
            Assert.Equal(expectedUnit, result.Unit);
        // Menutup scope penanganan operasi try; bagian berikut berada di luar batas blok tersebut dalam
        // DescribeMetric_UsesDimensionallyCorrectDisplayValuesAndUnits.
        }
        // Menjalankan blok finally ketika alur meninggalkan try/catch, termasuk saat terjadi exception dalam
        // DescribeMetric_UsesDimensionallyCorrectDisplayValuesAndUnits; bagian ini dipakai untuk pekerjaan penutup yang harus tetap dilakukan.
        finally
        // Membuka scope pekerjaan penutup finally; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // DescribeMetric_UsesDimensionallyCorrectDisplayValuesAndUnits.
        {
            // Memperbarui `CultureInfo.CurrentCulture` menggunakan `previousCulture` (nilai previous culture) dalam
            // DescribeMetric_UsesDimensionallyCorrectDisplayValuesAndUnits.
            CultureInfo.CurrentCulture = previousCulture;
        // Menutup scope pekerjaan penutup finally; bagian berikut berada di luar batas blok tersebut dalam
        // DescribeMetric_UsesDimensionallyCorrectDisplayValuesAndUnits.
        }
    // Menutup scope metode DescribeMetric_UsesDimensionallyCorrectDisplayValuesAndUnits; bagian berikut berada di luar batas blok tersebut dalam
    // DescribeMetric_UsesDimensionallyCorrectDisplayValuesAndUnits.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `DescribeMetric_DoesNotAppendAUnitToUnavailableValues` dengan hasil bertipe `void`; operasi ini menangani describe metric
    // does not append a unit ke unavailable nilai.
    public void DescribeMetric_DoesNotAppendAUnitToUnavailableValues()
    // Membuka scope metode DescribeMetric_DoesNotAppendAUnitToUnavailableValues; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // DescribeMetric_DoesNotAppendAUnitToUnavailableValues.
    {
        // Menyiapkan variabel lokal `result` untuk nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya dengan memanggil
        // `PlayerMetricLabelFormatter.DescribeMetric` dengan `”fulfillment_diversity”`, `”Unavailable”`, `true`, `true`, `”Unavailable”`, `key => key`.
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var result = PlayerMetricLabelFormatter.DescribeMetric(
            // Meneruskan nilai literal `”fulfillment_diversity”` sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`.
            "fulfillment_diversity",
            // Meneruskan nilai literal `”Unavailable”` sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`.
            "Unavailable",
            // Meneruskan true, yaitu kondisi aktif/terpenuhi sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`.
            true,
            // Meneruskan true, yaitu kondisi aktif/terpenuhi sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`.
            true,
            // Meneruskan nilai literal `”Unavailable”` sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`.
            "Unavailable",
            // Parameter `key` bertipe `` membawa nilai kunci.
            key => key);

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”players.support.value.unavailable”`,
        // `result.DisplayValue`); pengujian gagal jika keduanya berbeda dalam DescribeMetric_DoesNotAppendAUnitToUnavailableValues.
        Assert.Equal("players.support.value.unavailable", result.DisplayValue);
        // Menjalankan pemeriksaan hasil dengan `Assert.Empty` menggunakan `result.Unit`; ketidaksesuaian dengan ekspektasi membuat pengujian gagal dalam
        // DescribeMetric_DoesNotAppendAUnitToUnavailableValues.
        Assert.Empty(result.Unit);
    // Menutup scope metode DescribeMetric_DoesNotAppendAUnitToUnavailableValues; bagian berikut berada di luar batas blok tersebut dalam
    // DescribeMetric_DoesNotAppendAUnitToUnavailableValues.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `DescribeMetric_ExplainsWhatEmergencyActionsCount` dengan hasil bertipe `void`; operasi ini menangani describe metric
    // explains what emergency aksi jumlah.
    public void DescribeMetric_ExplainsWhatEmergencyActionsCount()
    // Membuka scope metode DescribeMetric_ExplainsWhatEmergencyActionsCount; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // DescribeMetric_ExplainsWhatEmergencyActionsCount.
    {
        // Menyiapkan variabel lokal `result` untuk nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya dengan memanggil
        // `PlayerMetricLabelFormatter.DescribeMetric` dengan `”emergency_options_used”`, `”1”`, `false`, `true`, `”Unavailable”`, `key => key`. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var result = PlayerMetricLabelFormatter.DescribeMetric(
            // Meneruskan nilai literal `”emergency_options_used”` sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`.
            "emergency_options_used",
            // Meneruskan nilai literal `”1”` sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`.
            "1",
            // Meneruskan false, yaitu kondisi nonaktif/tidak terpenuhi sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`.
            false,
            // Meneruskan true, yaitu kondisi aktif/terpenuhi sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`.
            true,
            // Meneruskan nilai literal `”Unavailable”` sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`.
            "Unavailable",
            // Parameter `key` bertipe `` membawa nilai kunci.
            key => key);

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”players.support.meaning.emergency_actions”`,
        // `result.Explanation`); pengujian gagal jika keduanya berbeda dalam DescribeMetric_ExplainsWhatEmergencyActionsCount.
        Assert.Equal("players.support.meaning.emergency_actions", result.Explanation);
    // Menutup scope metode DescribeMetric_ExplainsWhatEmergencyActionsCount; bagian berikut berada di luar batas blok tersebut dalam
    // DescribeMetric_ExplainsWhatEmergencyActionsCount.
    }

    // menandai metode sebagai pengujian xUnit yang dijalankan untuk setiap kombinasi data.
    [Theory]
    // menyediakan satu kombinasi masukan pengujian (”meal_orders_per_turn_average”, ”players.support.meaning.orders_per_active_day”).
    [InlineData("meal_orders_per_turn_average", "players.support.meaning.orders_per_active_day")]
    // menyediakan satu kombinasi masukan pengujian (”gold_investment_net”, ”players.support.meaning.gold_cashflow”).
    [InlineData("gold_investment_net", "players.support.meaning.gold_cashflow")]
    // menyediakan satu kombinasi masukan pengujian (”ingredient_cards_value_end”, ”players.support.meaning.pension_ingredient_value”).
    [InlineData("ingredient_cards_value_end", "players.support.meaning.pension_ingredient_value")]
    // menyediakan satu kombinasi masukan pengujian (”life_risk_costs_per_card”, ”players.support.meaning.risk_nominal_cost”).
    [InlineData("life_risk_costs_per_card", "players.support.meaning.risk_nominal_cost")]
    // menyediakan satu kombinasi masukan pengujian (”life_risk_costs_total”, ”players.support.meaning.risk_nominal_cost”).
    [InlineData("life_risk_costs_total", "players.support.meaning.risk_nominal_cost")]
    // Mendefinisikan metode `DescribeMetric_UsesSpecificExplanationsForPotentiallyAmbiguousValues` dengan hasil bertipe `void`; operasi ini menangani
    // describe metric uses specific explanations untuk potentially ambiguous nilai. Masukan: Parameter `path` bertipe `string` membawa nilai path;
    // Parameter `expectedExplanation` bertipe `string` membawa nilai yang diharapkan explanation.
    public void DescribeMetric_UsesSpecificExplanationsForPotentiallyAmbiguousValues(
        // Parameter `path` bertipe `string` membawa nilai path.
        string path,
        // Parameter `expectedExplanation` bertipe `string` membawa nilai yang diharapkan explanation.
        string expectedExplanation)
    // Membuka scope metode DescribeMetric_UsesSpecificExplanationsForPotentiallyAmbiguousValues; pernyataan/deklarasi berikut berada di dalam batas
    // blok ini dalam DescribeMetric_UsesSpecificExplanationsForPotentiallyAmbiguousValues.
    {
        // Menyiapkan variabel lokal `result` untuk nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya dengan memanggil
        // `PlayerMetricLabelFormatter.DescribeMetric` dengan `path`, `”2”`, `false`, `true`, `”Unavailable”`, `key => key`. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var result = PlayerMetricLabelFormatter.DescribeMetric(
            // Meneruskan `path` (nilai path) sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`.
            path,
            // Meneruskan nilai literal `”2”` sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`.
            "2",
            // Meneruskan false, yaitu kondisi nonaktif/tidak terpenuhi sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`.
            false,
            // Meneruskan true, yaitu kondisi aktif/terpenuhi sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`.
            true,
            // Meneruskan nilai literal `”Unavailable”` sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`.
            "Unavailable",
            // Parameter `key` bertipe `` membawa nilai kunci.
            key => key);

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`expectedExplanation`, `result.Explanation`);
        // pengujian gagal jika keduanya berbeda dalam DescribeMetric_UsesSpecificExplanationsForPotentiallyAmbiguousValues.
        Assert.Equal(expectedExplanation, result.Explanation);
    // Menutup scope metode DescribeMetric_UsesSpecificExplanationsForPotentiallyAmbiguousValues; bagian berikut berada di luar batas blok tersebut
    // dalam DescribeMetric_UsesSpecificExplanationsForPotentiallyAmbiguousValues.
    }

    // menandai metode sebagai pengujian xUnit yang dijalankan untuk setiap kombinasi data.
    [Theory]
    // menyediakan satu kombinasi masukan pengujian (”day_when_debt_introduced”, ”0”, ”players.support.value.preparation_loan”).
    [InlineData("day_when_debt_introduced", "0", "players.support.value.preparation_loan")]
    // menyediakan satu kombinasi masukan pengujian (”day_when_first_risk_hit”, ”2”, ”players.support.value.day_index”).
    [InlineData("day_when_first_risk_hit", "2", "players.support.value.day_index")]
    // menyediakan satu kombinasi masukan pengujian (”day_game_completion”, ”25”, ”players.support.value.day_index”).
    [InlineData("day_game_completion", "25", "players.support.value.day_index")]
    // Mendefinisikan metode `DescribeMetric_FormatsGameDaysAsBoardPositions` dengan hasil bertipe `void`; operasi ini menangani describe metric formats
    // game hari as board positions. Masukan: Parameter `path` bertipe `string` membawa nilai path; Parameter `value` bertipe `string` membawa nilai
    // nilai; Parameter `expectedValueKey` bertipe `string` membawa nilai yang diharapkan nilai kunci.
    public void DescribeMetric_FormatsGameDaysAsBoardPositions(
        // Parameter `path` bertipe `string` membawa nilai path.
        string path,
        // Parameter `value` bertipe `string` membawa nilai nilai.
        string value,
        // Parameter `expectedValueKey` bertipe `string` membawa nilai yang diharapkan nilai kunci.
        string expectedValueKey)
    // Membuka scope metode DescribeMetric_FormatsGameDaysAsBoardPositions; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // DescribeMetric_FormatsGameDaysAsBoardPositions.
    {
        // Menyiapkan variabel lokal `result` untuk nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya dengan memanggil
        // `PlayerMetricLabelFormatter.DescribeMetric` dengan `path`, `value`, `false`, `true`, `”Unavailable”`, `key => key`. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var result = PlayerMetricLabelFormatter.DescribeMetric(
            // Meneruskan `path` (nilai path) sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`.
            path,
            // Meneruskan `value` (nilai nilai) sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`.
            value,
            // Meneruskan false, yaitu kondisi nonaktif/tidak terpenuhi sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`.
            false,
            // Meneruskan true, yaitu kondisi aktif/terpenuhi sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`.
            true,
            // Meneruskan nilai literal `”Unavailable”` sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`.
            "Unavailable",
            // Parameter `key` bertipe `` membawa nilai kunci.
            key => key);

        // Menjalankan pemeriksaan hasil dengan `Assert.StartsWith` menggunakan `expectedValueKey`, `result.DisplayValue`, `StringComparison.Ordinal`;
        // ketidaksesuaian dengan ekspektasi membuat pengujian gagal dalam DescribeMetric_FormatsGameDaysAsBoardPositions.
        Assert.StartsWith(expectedValueKey, result.DisplayValue, StringComparison.Ordinal);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”recorded”`, `result.State`); pengujian gagal
        // jika keduanya berbeda dalam DescribeMetric_FormatsGameDaysAsBoardPositions.
        Assert.Equal("recorded", result.State);
        // Menjalankan pemeriksaan hasil dengan `Assert.Empty` menggunakan `result.Unit`; ketidaksesuaian dengan ekspektasi membuat pengujian gagal dalam
        // DescribeMetric_FormatsGameDaysAsBoardPositions.
        Assert.Empty(result.Unit);
    // Menutup scope metode DescribeMetric_FormatsGameDaysAsBoardPositions; bagian berikut berada di luar batas blok tersebut dalam
    // DescribeMetric_FormatsGameDaysAsBoardPositions.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `DescribeMetric_TreatsAMissingLoanDayAsNoRecordedLoan` dengan hasil bertipe `void`; operasi ini menangani describe metric
    // treats a missing pinjaman hari as no recorded pinjaman.
    public void DescribeMetric_TreatsAMissingLoanDayAsNoRecordedLoan()
    // Membuka scope metode DescribeMetric_TreatsAMissingLoanDayAsNoRecordedLoan; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // DescribeMetric_TreatsAMissingLoanDayAsNoRecordedLoan.
    {
        // Menyiapkan variabel lokal `result` untuk nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya dengan memanggil
        // `PlayerMetricLabelFormatter.DescribeMetric` dengan `”day_when_debt_introduced”`, `”Unavailable”`, `false`, `true`, `”Unavailable”`, `key => key`.
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var result = PlayerMetricLabelFormatter.DescribeMetric(
            // Meneruskan nilai literal `”day_when_debt_introduced”` sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`.
            "day_when_debt_introduced",
            // Meneruskan nilai literal `”Unavailable”` sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`.
            "Unavailable",
            // Meneruskan false, yaitu kondisi nonaktif/tidak terpenuhi sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`.
            false,
            // Meneruskan true, yaitu kondisi aktif/terpenuhi sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`.
            true,
            // Meneruskan nilai literal `”Unavailable”` sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`.
            "Unavailable",
            // Parameter `key` bertipe `` membawa nilai kunci.
            key => key);

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”players.support.value.no_loan_recorded”`,
        // `result.DisplayValue`); pengujian gagal jika keduanya berbeda dalam DescribeMetric_TreatsAMissingLoanDayAsNoRecordedLoan.
        Assert.Equal("players.support.value.no_loan_recorded", result.DisplayValue);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”recorded”`, `result.State`); pengujian gagal
        // jika keduanya berbeda dalam DescribeMetric_TreatsAMissingLoanDayAsNoRecordedLoan.
        Assert.Equal("recorded", result.State);
        // Menjalankan pemeriksaan hasil dengan `Assert.Empty` menggunakan `result.Unit`; ketidaksesuaian dengan ekspektasi membuat pengujian gagal dalam
        // DescribeMetric_TreatsAMissingLoanDayAsNoRecordedLoan.
        Assert.Empty(result.Unit);
    // Menutup scope metode DescribeMetric_TreatsAMissingLoanDayAsNoRecordedLoan; bagian berikut berada di luar batas blok tersebut dalam
    // DescribeMetric_TreatsAMissingLoanDayAsNoRecordedLoan.
    }

    // menandai metode sebagai pengujian xUnit yang dijalankan untuk setiap kombinasi data.
    [Theory]
    // menyediakan satu kombinasi masukan pengujian (”id”, ”Donasi Setiap Jumat”, ”Jumlah Donasi”, ”koin”).
    [InlineData("id", "Donasi Setiap Jumat", "Jumlah Donasi", "koin")]
    // menyediakan satu kombinasi masukan pengujian (”en”, ”Donations Each Friday”, ”Donation Amount”, ”coins”).
    [InlineData("en", "Donations Each Friday", "Donation Amount", "coins")]
    // Mendefinisikan metode `DescribeMetric_LocalizesCombinedDonationHistory` dengan hasil bertipe `void`; operasi ini menangani describe metric
    // localizes combined donasi history. Masukan: Parameter `language` bertipe `string` membawa nilai language; Parameter `title` bertipe `string`
    // membawa nilai title; Parameter `amountLabel` bertipe `string` membawa nilai nominal label; Parameter `coins` bertipe `string` membawa nilai
    // coins.
    public void DescribeMetric_LocalizesCombinedDonationHistory(string language, string title, string amountLabel, string coins)
    // Membuka scope metode DescribeMetric_LocalizesCombinedDonationHistory; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // DescribeMetric_LocalizesCombinedDonationHistory.
    {
        // Mendefinisikan fungsi lokal Localize dengan hasil `string`; fungsi ini dipakai oleh alur di dalam scope yang sama. Ekspresi hasilnya adalah
        // memanggil `UiText.Translate` dengan `language`, `key`.
        string Localize(string key) => UiText.Translate(language, key);
        // Menyiapkan variabel lokal `history` untuk nilai history dengan memanggil `PlayerMetricLabelFormatter.DescribeMetric` dengan `”donation_history”`,
        // `”[{\”day_index\”:5,\”amount\”:1,\”rank\”:4}]”`, `false`, `true`, `”—”`, `Localize`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var history = PlayerMetricLabelFormatter.DescribeMetric(
            // Meneruskan nilai literal `”donation_history”` sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`; Meneruskan nilai literal
            // `”[{\”day_index\”:5,\”amount\”:1,\”rank\”:4}]”` sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`; Meneruskan false, yaitu kondisi
            // nonaktif/tidak terpenuhi sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`; Meneruskan true, yaitu kondisi aktif/terpenuhi sebagai
            // argumen ke `PlayerMetricLabelFormatter.DescribeMetric`; Meneruskan nilai literal `”—”` sebagai argumen ke
            // `PlayerMetricLabelFormatter.DescribeMetric`; Meneruskan `Localize` (nilai localize) sebagai argumen ke
            // `PlayerMetricLabelFormatter.DescribeMetric`.
            "donation_history", "[{\"day_index\":5,\"amount\":1,\"rank\":4}]", false, true, "—", Localize);
        // Menyiapkan variabel lokal `amount` untuk nominal uang atau nilai transaksi yang dipakai dalam operasi dengan memanggil
        // `PlayerMetricLabelFormatter.DescribeMetric` dengan `”donation_history.amount”`, `”1”`, `false`, `true`, `”—”`, `Localize`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var amount = PlayerMetricLabelFormatter.DescribeMetric(
            // Meneruskan nilai literal `”donation_history.amount”` sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`; Meneruskan nilai literal
            // `”1”` sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`; Meneruskan false, yaitu kondisi nonaktif/tidak terpenuhi sebagai argumen ke
            // `PlayerMetricLabelFormatter.DescribeMetric`; Meneruskan true, yaitu kondisi aktif/terpenuhi sebagai argumen ke
            // `PlayerMetricLabelFormatter.DescribeMetric`; Meneruskan nilai literal `”—”` sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`;
            // Meneruskan `Localize` (nilai localize) sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`.
            "donation_history.amount", "1", false, true, "—", Localize);
        // Menyiapkan variabel lokal `empty` untuk nilai empty dengan memanggil `PlayerMetricLabelFormatter.DescribeMetric` dengan `”donation_history”`,
        // `”[]”`, `false`, `true`, `”—”`, `Localize`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var empty = PlayerMetricLabelFormatter.DescribeMetric(
            // Meneruskan nilai literal `”donation_history”` sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`; Meneruskan nilai literal `”[]”`
            // sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`; Meneruskan false, yaitu kondisi nonaktif/tidak terpenuhi sebagai argumen ke
            // `PlayerMetricLabelFormatter.DescribeMetric`; Meneruskan true, yaitu kondisi aktif/terpenuhi sebagai argumen ke
            // `PlayerMetricLabelFormatter.DescribeMetric`; Meneruskan nilai literal `”—”` sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`;
            // Meneruskan `Localize` (nilai localize) sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`.
            "donation_history", "[]", false, true, "—", Localize);

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`title`, `history.Label`); pengujian gagal jika
        // keduanya berbeda dalam DescribeMetric_LocalizesCombinedDonationHistory.
        Assert.Equal(title, history.Label);
        // Menjalankan pemeriksaan hasil dengan `Assert.StartsWith` menggunakan `title`, `history.Explanation`, `StringComparison.Ordinal`; ketidaksesuaian
        // dengan ekspektasi membuat pengujian gagal dalam DescribeMetric_LocalizesCombinedDonationHistory.
        Assert.StartsWith(title, history.Explanation, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”Tie Breaker”`, `history.Explanation`,
        // `StringComparison.Ordinal` dalam DescribeMetric_LocalizesCombinedDonationHistory.
        Assert.Contains("Tie Breaker", history.Explanation, StringComparison.Ordinal);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`amountLabel`,
        // `Localize(”players.details.donations.amount”)`); pengujian gagal jika keduanya berbeda dalam DescribeMetric_LocalizesCombinedDonationHistory.
        Assert.Equal(amountLabel, Localize("players.details.donations.amount"));
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`coins`, `amount.Unit`); pengujian gagal jika
        // keduanya berbeda dalam DescribeMetric_LocalizesCombinedDonationHistory.
        Assert.Equal(coins, amount.Unit);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”unavailable”`, `empty.State`); pengujian
        // gagal jika keduanya berbeda dalam DescribeMetric_LocalizesCombinedDonationHistory.
        Assert.Equal("unavailable", empty.State);
        // Menjalankan pemeriksaan hasil dengan `Assert.Empty` menggunakan `empty.Unit`; ketidaksesuaian dengan ekspektasi membuat pengujian gagal dalam
        // DescribeMetric_LocalizesCombinedDonationHistory.
        Assert.Empty(empty.Unit);
    // Menutup scope metode DescribeMetric_LocalizesCombinedDonationHistory; bagian berikut berada di luar batas blok tersebut dalam
    // DescribeMetric_LocalizesCombinedDonationHistory.
    }

    // menandai metode sebagai pengujian xUnit yang dijalankan untuk setiap kombinasi data.
    [Theory]
    // menyediakan satu kombinasi masukan pengujian (”id”, ”4”, ”ke-4”, ”recorded”).
    [InlineData("id", "4", "ke-4", "recorded")]
    // menyediakan satu kombinasi masukan pengujian (”id”, ”1”, ”ke-1”, ”recorded”).
    [InlineData("id", "1", "ke-1", "recorded")]
    // menyediakan satu kombinasi masukan pengujian (”en”, ”4”, ”Rank 4”, ”recorded”).
    [InlineData("en", "4", "Rank 4", "recorded")]
    // menyediakan satu kombinasi masukan pengujian (”id”, ”—”, ”—”, ”unavailable”).
    [InlineData("id", "—", "—", "unavailable")]
    // menyediakan satu kombinasi masukan pengujian (”en”, ”—”, ”—”, ”unavailable”).
    [InlineData("en", "—", "—", "unavailable")]
    // Mendefinisikan metode `DescribeMetric_FormatsPensionRankAsAPosition` dengan hasil bertipe `void`; operasi ini menangani describe metric formats
    // pension rank as a position. Masukan: Parameter `language` bertipe `string` membawa nilai language; Parameter `value` bertipe `string` membawa
    // nilai nilai; Parameter `expected` bertipe `string` membawa nilai yang diharapkan; Parameter `state` bertipe `string` membawa keadaan permainan
    // yang menjadi sumber atau hasil pembaruan.
    public void DescribeMetric_FormatsPensionRankAsAPosition(string language, string value, string expected, string state)
    // Membuka scope metode DescribeMetric_FormatsPensionRankAsAPosition; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // DescribeMetric_FormatsPensionRankAsAPosition.
    {
        // Menyiapkan variabel lokal `result` untuk nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya dengan memanggil
        // `PlayerMetricLabelFormatter.DescribeMetric` dengan `”pension_fund_rank_per_game”`, `value`, `false`, `true`, `”—”`, `key =>
        // UiText.Translate(language, key)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var result = PlayerMetricLabelFormatter.DescribeMetric(
            // Meneruskan nilai literal `”pension_fund_rank_per_game”` sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`; Meneruskan `value` (nilai
            // nilai) sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`; Meneruskan false, yaitu kondisi nonaktif/tidak terpenuhi sebagai argumen
            // ke `PlayerMetricLabelFormatter.DescribeMetric`; Meneruskan true, yaitu kondisi aktif/terpenuhi sebagai argumen ke
            // `PlayerMetricLabelFormatter.DescribeMetric`; Meneruskan nilai literal `”—”` sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`;
            // Meneruskan fungsi lambda `key => UiText.Translate(language, key)` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai
            // argumen ke `PlayerMetricLabelFormatter.DescribeMetric`; Meneruskan `language` (nilai language) sebagai argumen ke `UiText.Translate`; Meneruskan
            // `key` (nilai kunci) sebagai argumen ke `UiText.Translate`.
            "pension_fund_rank_per_game", value, false, true, "—", key => UiText.Translate(language, key));

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`expected`, `result.DisplayValue`); pengujian
        // gagal jika keduanya berbeda dalam DescribeMetric_FormatsPensionRankAsAPosition.
        Assert.Equal(expected, result.DisplayValue);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`state`, `result.State`); pengujian gagal jika
        // keduanya berbeda dalam DescribeMetric_FormatsPensionRankAsAPosition.
        Assert.Equal(state, result.State);
        // Menjalankan pemeriksaan hasil dengan `Assert.Empty` menggunakan `result.Unit`; ketidaksesuaian dengan ekspektasi membuat pengujian gagal dalam
        // DescribeMetric_FormatsPensionRankAsAPosition.
        Assert.Empty(result.Unit);
    // Menutup scope metode DescribeMetric_FormatsPensionRankAsAPosition; bagian berikut berada di luar batas blok tersebut dalam
    // DescribeMetric_FormatsPensionRankAsAPosition.
    }

    // menandai metode sebagai pengujian xUnit yang dijalankan untuk setiap kombinasi data.
    [Theory]
    // menyediakan satu kombinasi masukan pengujian (”id”, ”financial_goals_completed”, ”1”, ”recorded”, ”Jumlah target finansial”).
    [InlineData("id", "financial_goals_completed", "1", "recorded", "Jumlah target finansial")]
    // menyediakan satu kombinasi masukan pengujian (”id”, ”financial_goals_completed”, ”0”, ”zero”, ”Jumlah target finansial”).
    [InlineData("id", "financial_goals_completed", "0", "zero", "Jumlah target finansial")]
    // menyediakan satu kombinasi masukan pengujian (”en”, ”financial_goals_completed”, ”1”, ”recorded”, ”Number of financial goals”).
    [InlineData("en", "financial_goals_completed", "1", "recorded", "Number of financial goals")]
    // menyediakan satu kombinasi masukan pengujian (”id”, ”coins_saved”, ”3”, ”recorded”, ”Koin yang masih tersimpan”).
    [InlineData("id", "coins_saved", "3", "recorded", "Koin yang masih tersimpan")]
    // menyediakan satu kombinasi masukan pengujian (”en”, ”coins_saved”, ”3”, ”recorded”, ”Coins still saved”).
    [InlineData("en", "coins_saved", "3", "recorded", "Coins still saved")]
    // menyediakan satu kombinasi masukan pengujian (”id”, ”sharia_loans_outstanding_coins”, ”0”, ”zero”, ”Koin pinjaman yang masih harus
    // dikembalikan”).
    [InlineData("id", "sharia_loans_outstanding_coins", "0", "zero", "Koin pinjaman yang masih harus dikembalikan")]
    // menyediakan satu kombinasi masukan pengujian (”id”, ”sharia_loans_outstanding_coins”, ”10”, ”recorded”, ”Koin pinjaman yang masih harus
    // dikembalikan”).
    [InlineData("id", "sharia_loans_outstanding_coins", "10", "recorded", "Koin pinjaman yang masih harus dikembalikan")]
    // menyediakan satu kombinasi masukan pengujian (”en”, ”sharia_loans_outstanding_coins”, ”0”, ”zero”, ”Borrowed coins still to be repaid”).
    [InlineData("en", "sharia_loans_outstanding_coins", "0", "zero", "Borrowed coins still to be repaid")]
    // menyediakan satu kombinasi masukan pengujian (”id”, ”sharia_loans_outstanding_coins”, ”—”, ”unavailable”, ”Koin pinjaman yang masih harus
    // dikembalikan”).
    [InlineData("id", "sharia_loans_outstanding_coins", "—", "unavailable", "Koin pinjaman yang masih harus dikembalikan")]
    // menyediakan satu kombinasi masukan pengujian (”id”, ”coins_saved”, ”—”, ”unavailable”, ”Koin yang masih tersimpan”).
    [InlineData("id", "coins_saved", "—", "unavailable", "Koin yang masih tersimpan")]
    // menyediakan satu kombinasi masukan pengujian (”id”, ”financial_goals_completed”, ”—”, ”unavailable”, ”Jumlah target finansial”).
    [InlineData("id", "financial_goals_completed", "—", "unavailable", "Jumlah target finansial")]
    // Mendefinisikan metode `DescribeMetric_ExplainsEssentialGoalValuesWithoutChangingOrInventingData` dengan hasil bertipe `void`; operasi ini
    // menangani describe metric explains essential target nilai tanpa changing atau inventing data. Masukan: Parameter `language` bertipe `string`
    // membawa nilai language; Parameter `path` bertipe `string` membawa nilai path; Parameter `value` bertipe `string` membawa nilai nilai; Parameter
    // `state` bertipe `string` membawa keadaan permainan yang menjadi sumber atau hasil pembaruan; Parameter `explanation` bertipe `string` membawa
    // nilai explanation.
    public void DescribeMetric_ExplainsEssentialGoalValuesWithoutChangingOrInventingData(
        // Parameter `language` bertipe `string` membawa nilai language.
        string language, string path, string value, string state, string explanation)
    // Membuka scope metode DescribeMetric_ExplainsEssentialGoalValuesWithoutChangingOrInventingData; pernyataan/deklarasi berikut berada di dalam batas
    // blok ini dalam DescribeMetric_ExplainsEssentialGoalValuesWithoutChangingOrInventingData.
    {
        // Menyiapkan variabel lokal `result` untuk nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya dengan memanggil
        // `PlayerMetricLabelFormatter.DescribeMetric` dengan `path`, `value`, `false`, `true`, `”—”`, `key => UiText.Translate(language, key)`. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var result = PlayerMetricLabelFormatter.DescribeMetric(
            // Meneruskan `path` (nilai path) sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`; Meneruskan `value` (nilai nilai) sebagai argumen
            // ke `PlayerMetricLabelFormatter.DescribeMetric`; Meneruskan false, yaitu kondisi nonaktif/tidak terpenuhi sebagai argumen ke
            // `PlayerMetricLabelFormatter.DescribeMetric`; Meneruskan true, yaitu kondisi aktif/terpenuhi sebagai argumen ke
            // `PlayerMetricLabelFormatter.DescribeMetric`; Meneruskan nilai literal `”—”` sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`;
            // Meneruskan fungsi lambda `key => UiText.Translate(language, key)` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai
            // argumen ke `PlayerMetricLabelFormatter.DescribeMetric`; Meneruskan `language` (nilai language) sebagai argumen ke `UiText.Translate`; Meneruskan
            // `key` (nilai kunci) sebagai argumen ke `UiText.Translate`.
            path, value, false, true, "—", key => UiText.Translate(language, key));

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`value`, `result.DisplayValue`); pengujian
        // gagal jika keduanya berbeda dalam DescribeMetric_ExplainsEssentialGoalValuesWithoutChangingOrInventingData.
        Assert.Equal(value, result.DisplayValue);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`state`, `result.State`); pengujian gagal jika
        // keduanya berbeda dalam DescribeMetric_ExplainsEssentialGoalValuesWithoutChangingOrInventingData.
        Assert.Equal(state, result.State);
        // Menjalankan pemeriksaan hasil dengan `Assert.StartsWith` menggunakan `explanation`, `result.Explanation`, `StringComparison.Ordinal`;
        // ketidaksesuaian dengan ekspektasi membuat pengujian gagal dalam DescribeMetric_ExplainsEssentialGoalValuesWithoutChangingOrInventingData.
        Assert.StartsWith(explanation, result.Explanation, StringComparison.Ordinal);
    // Menutup scope metode DescribeMetric_ExplainsEssentialGoalValuesWithoutChangingOrInventingData; bagian berikut berada di luar batas blok tersebut
    // dalam DescribeMetric_ExplainsEssentialGoalValuesWithoutChangingOrInventingData.
    }

    // menandai metode sebagai pengujian xUnit yang dijalankan untuk setiap kombinasi data.
    [Theory]
    // menyediakan satu kombinasi masukan pengujian (”id”, true, ”0”).
    [InlineData("id", true, "0")]
    // menyediakan satu kombinasi masukan pengujian (”id”, false, ”3”).
    [InlineData("id", false, "3")]
    // menyediakan satu kombinasi masukan pengujian (”en”, true, ”0”).
    [InlineData("en", true, "0")]
    // menyediakan satu kombinasi masukan pengujian (”en”, false, ”3”).
    [InlineData("en", false, "3")]
    // menyediakan satu kombinasi masukan pengujian (”id”, true, ”—”).
    [InlineData("id", true, "—")]
    // Mendefinisikan metode `DescribeMetric_DistinguishesNeedCardsOwnedFromPurchases` dengan hasil bertipe `void`; operasi ini menangani describe
    // metric distinguishes kebutuhan kartu dimiliki dari purchases. Masukan: Parameter `language` bertipe `string` membawa nilai language; Parameter
    // `advanced` bertipe `bool` membawa nilai advanced; Parameter `value` bertipe `string` membawa nilai nilai.
    public void DescribeMetric_DistinguishesNeedCardsOwnedFromPurchases(string language, bool advanced, string value)
    // Membuka scope metode DescribeMetric_DistinguishesNeedCardsOwnedFromPurchases; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // DescribeMetric_DistinguishesNeedCardsOwnedFromPurchases.
    {
        // Mendefinisikan fungsi lokal Localize dengan hasil `string`; fungsi ini dipakai oleh alur di dalam scope yang sama. Ekspresi hasilnya adalah
        // memanggil `UiText.Translate` dengan `language`, `key`.
        string Localize(string key) => UiText.Translate(language, key);
        // Menyiapkan variabel lokal `owned` untuk nilai dimiliki dengan memanggil `PlayerMetricLabelFormatter.DescribeMetric` dengan
        // `”need_cards_owned_current”`, `value`, `false`, `advanced`, `”—”`, `Localize`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var owned = PlayerMetricLabelFormatter.DescribeMetric(
            // Meneruskan nilai literal `”need_cards_owned_current”` sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`; Meneruskan `value` (nilai
            // nilai) sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`; Meneruskan false, yaitu kondisi nonaktif/tidak terpenuhi sebagai argumen
            // ke `PlayerMetricLabelFormatter.DescribeMetric`; Meneruskan `advanced` (nilai advanced) sebagai argumen ke
            // `PlayerMetricLabelFormatter.DescribeMetric`; Meneruskan nilai literal `”—”` sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`;
            // Meneruskan `Localize` (nilai localize) sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`.
            "need_cards_owned_current", value, false, advanced, "—", Localize);
        // Menyiapkan variabel lokal `purchased` untuk nilai dibeli dengan memanggil `PlayerMetricLabelFormatter.DescribeMetric` dengan
        // `”need_cards_purchased”`, `”1”`, `false`, `advanced`, `”—”`, `Localize`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var purchased = PlayerMetricLabelFormatter.DescribeMetric(
            // Meneruskan nilai literal `”need_cards_purchased”` sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`; Meneruskan nilai literal `”1”`
            // sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`; Meneruskan false, yaitu kondisi nonaktif/tidak terpenuhi sebagai argumen ke
            // `PlayerMetricLabelFormatter.DescribeMetric`; Meneruskan `advanced` (nilai advanced) sebagai argumen ke
            // `PlayerMetricLabelFormatter.DescribeMetric`; Meneruskan nilai literal `”—”` sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`;
            // Meneruskan `Localize` (nilai localize) sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`.
            "need_cards_purchased", "1", false, advanced, "—", Localize);

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`value`, `owned.DisplayValue`); pengujian gagal
        // jika keduanya berbeda dalam DescribeMetric_DistinguishesNeedCardsOwnedFromPurchases.
        Assert.Equal(value, owned.DisplayValue);
        // Menjalankan pemeriksaan hasil dengan `Assert.StartsWith` menggunakan `Localize(”players.support.meaning.need_cards_owned”)`, `owned.Explanation`,
        // `StringComparison.Ordinal`; ketidaksesuaian dengan ekspektasi membuat pengujian gagal dalam
        // DescribeMetric_DistinguishesNeedCardsOwnedFromPurchases.
        Assert.StartsWith(Localize("players.support.meaning.need_cards_owned"), owned.Explanation, StringComparison.Ordinal);
        // Menyiapkan variabel lokal `saleNote` untuk nilai penjualan note dengan memanggil `Localize` dengan
        // `”players.support.meaning.need_cards_sold_note”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var saleNote = Localize("players.support.meaning.need_cards_sold_note");
        // Memeriksa `advanced` (nilai advanced); blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // DescribeMetric_DistinguishesNeedCardsOwnedFromPurchases.
        if (advanced)
            // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `saleNote`, `owned.Explanation`,
            // `StringComparison.Ordinal` dalam DescribeMetric_DistinguishesNeedCardsOwnedFromPurchases.
            Assert.Contains(saleNote, owned.Explanation, StringComparison.Ordinal);
        // Menjalankan cabang alternatif ketika kondisi if sebelumnya tidak terpenuhi dalam DescribeMetric_DistinguishesNeedCardsOwnedFromPurchases.
        else
            // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `saleNote`, `owned.Explanation`,
            // `StringComparison.Ordinal` dalam DescribeMetric_DistinguishesNeedCardsOwnedFromPurchases.
            Assert.DoesNotContain(saleNote, owned.Explanation, StringComparison.Ordinal);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”1”`, `purchased.DisplayValue`); pengujian
        // gagal jika keduanya berbeda dalam DescribeMetric_DistinguishesNeedCardsOwnedFromPurchases.
        Assert.Equal("1", purchased.DisplayValue);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui
        // Assert.Equal(`Localize(”players.support.meaning.need_cards_purchased”)`, `purchased.Explanation`); pengujian gagal jika keduanya berbeda dalam
        // DescribeMetric_DistinguishesNeedCardsOwnedFromPurchases.
        Assert.Equal(Localize("players.support.meaning.need_cards_purchased"), purchased.Explanation);
    // Menutup scope metode DescribeMetric_DistinguishesNeedCardsOwnedFromPurchases; bagian berikut berada di luar batas blok tersebut dalam
    // DescribeMetric_DistinguishesNeedCardsOwnedFromPurchases.
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
            // Untuk pola `”players.details.metric_fallback”`, menghasilkan nilai literal `”Data”` sebagai hasil switch.
            "players.details.metric_fallback" => "Data",
            // Untuk pola `”players.details.item”`, menghasilkan nilai literal `”Item”` sebagai hasil switch.
            "players.details.item" => "Item",
            // Untuk pola `”players.details.series”`, menghasilkan nilai literal `”Series”` sebagai hasil switch.
            "players.details.series" => "Series",
            // Untuk pola `”common.value”`, menghasilkan nilai literal `”Value”` sebagai hasil switch.
            "common.value" => "Value",
            // Untuk pola `”players.raw.coins”`, menghasilkan nilai literal `”Coins”` sebagai hasil switch.
            "players.raw.coins" => "Coins",
            // Untuk pola `”players.raw.coins_net_end_game”`, menghasilkan nilai literal `”Ending Coins”` sebagai hasil switch.
            "players.raw.coins_net_end_game" => "Ending Coins",
            // Untuk pola `”players.raw.coins_spent_per_turn”`, menghasilkan nilai literal `”Outgoing Coins per Event”` sebagai hasil switch.
            "players.raw.coins_spent_per_turn" => "Outgoing Coins per Event",
            // Untuk pola `”players.raw.amount”`, menghasilkan nilai literal `”Amount”` sebagai hasil switch.
            "players.raw.amount" => "Amount",
            // Untuk pola `”players.raw.ingredient_types_held”`, menghasilkan nilai literal `”Ingredient Types Held”` sebagai hasil switch.
            "players.raw.ingredient_types_held" => "Ingredient Types Held",
            // Untuk pola `”players.raw.white_rice”`, menghasilkan nilai literal `”White Rice”` sebagai hasil switch.
            "players.raw.white_rice" => "White Rice",
            // Untuk pola `”players.metric.income_diversification_components”`, menghasilkan nilai literal `”Income Diversification Components”` sebagai hasil
            // switch.
            "players.metric.income_diversification_components" => "Income Diversification Components",
            // Untuk pola `”players.metric.freelance_income”`, menghasilkan nilai literal `”Freelance Income”` sebagai hasil switch.
            "players.metric.freelance_income" => "Freelance Income",
            // Untuk pola `”players.details.transaction_label”`, menghasilkan nilai literal `”Transaction”` sebagai hasil switch.
            "players.details.transaction_label" => "Transaction",
            // Untuk pola `”players.details.transaction.opening_cash”`, menghasilkan nilai literal `”Opening Cash”` sebagai hasil switch.
            "players.details.transaction.opening_cash" => "Opening Cash",
            // Untuk pola `”players.details.transaction.opening_cash_with_amount”`, menghasilkan nilai literal `”Opening Cash ({0})”` sebagai hasil switch.
            "players.details.transaction.opening_cash_with_amount" => "Opening Cash ({0})",
            // Untuk pola `”players.details.transaction.cash_in”`, menghasilkan nilai literal `”Cash In”` sebagai hasil switch.
            "players.details.transaction.cash_in" => "Cash In",
            // Untuk pola `”players.details.transaction.cash_out”`, menghasilkan nilai literal `”Cash Out”` sebagai hasil switch.
            "players.details.transaction.cash_out" => "Cash Out",
            // Untuk pola `”players.details.transaction.category.gold_trade”`, menghasilkan nilai literal `”Gold Trade”` sebagai hasil switch.
            "players.details.transaction.category.gold_trade" => "Gold Trade",
            // Untuk pola `_`, menghasilkan `key` (nilai kunci) sebagai hasil switch.
            _ => key
        // Menutup scope pemetaan switch atas `key`; bagian berikut berada di luar batas blok tersebut dalam Translate.
        };
    // Menutup scope metode Translate; bagian berikut berada di luar batas blok tersebut dalam Translate.
    }
// Menutup scope tipe PlayerMetricLabelFormatterTests; bagian berikut berada di luar batas blok tersebut.
}
