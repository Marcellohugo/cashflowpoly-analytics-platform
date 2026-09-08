// Fungsi file: Memverifikasi konsistensi nama variabel, rumus, dan sumber data analitik pemain.
// Mengimpor namespace `Cashflowpoly.Ui.Infrastructure` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Ui.Infrastructure;
// Mengimpor namespace `Xunit` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Xunit;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Tests` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Tests;

// Mendefinisikan tipe class `PlayerMetricNamingConsistencyTests`; sealed mencegah tipe ini diturunkan lagi.
public sealed class PlayerMetricNamingConsistencyTests
// Membuka scope tipe PlayerMetricNamingConsistencyTests; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan properti `Formulas` bertipe `TheoryData<string, string, string, string[]>` untuk nilai formulas; nilainya dihitung dari objek baru
    // dengan tipe mengikuti konteks tujuan dan argumen ().
    public static TheoryData<string, string, string, string[]> Formulas => new()
    // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini.
    {
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini.
        { "net-worth", "cash_growth_percent", "net_worth", ["coins_net_end_game", "starting_coins"] },
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini.
        { "income-diversification", "income_diversification_index", "income_diversification_index", ["income_shares", "active_income_source_count"] },
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini.
        { "expense-efficiency", "business_expense_share_percent", "expense_efficiency", ["ingredient_investment_coins_total", "total_cash_out"] },
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini.
        { "business-margin", "meal_order_profit_margin_percent", "business_margin", ["meal_order_income_total", "ingredient_cost_used"] },
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini.
        { "risk-appetite", "risk_readiness_percent", "risk_appetite", ["risks_resolved_without_emergency", "life_risk_cards_drawn"] },
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini.
        { "debt-discipline", "loan_burden_percent", "debt_leverage", ["outstanding_loan", "liquid_assets"] },
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini.
        { "goal-ambition", "financial_goals_completed", "goal_purchases", ["financial_goals_purchase_cost_total", "financial_goals_incomplete_coins_wasted"] },
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini.
        { "action-efficiency", "income_action_focus_percent", "action_efficiency", ["income_main_actions", "total_main_actions"] },
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini.
        { "meal-success", "ingredient_utilization_percent", "order_success", ["ingredients_used_in_completed_orders", "ingredients_collected"] },
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini.
        { "planning-horizon", "long_term_action_share_percent", "planning", ["saving_and_goal_actions", "insurance_actions", "loan_repayment_actions", "total_main_actions"] },
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini.
        { "fulfillment-diversity", "need_fulfillment_diversity_percent", "fulfillment", ["primary_need_share", "secondary_need_share", "tertiary_need_share"] },
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini.
        { "donation-commitment", "donation_commitment_score", "donation_commitment", ["donation_stability_index", "donated_resource_share", "friday_participation_rate"] },
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini.
        { "happiness-portfolio", "happiness_source_diversity_percent", "happiness_portfolio", ["need_card_points", "need_set_bonus_points", "donation_points", "gold_points", "pension_points", "financial_goal_points"] },
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini.
        { "happiness-portfolio.beginner", "happiness_source_diversity_percent", "happiness_portfolio.beginner", ["need_card_points", "need_set_bonus_points", "donation_points", "gold_points", "pension_points"] }
    // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut.
    };

    // menandai metode sebagai pengujian xUnit yang dijalankan untuk setiap kombinasi data.
    [Theory]
    // menerapkan metadata `MemberData(nameof(Formulas))` pada deklarasi berikut agar framework/compiler dapat mengenali pengaturannya.
    [MemberData(nameof(Formulas))]
    // Mendefinisikan metode `FormulaAndSource_UseTheSameNamesAsTheirCards` dengan hasil bertipe `void`; operasi ini menangani formula dan source use
    // the same nama as their kartu. Masukan: Parameter `analysisKey` bertipe `string` membawa nilai analysis kunci; Parameter `metricKey` bertipe
    // `string` membawa nilai metric kunci; Parameter `formulaKey` bertipe `string` membawa nilai formula kunci; Parameter `inputs` bertipe `string[]`
    // membawa nilai inputs.
    public void FormulaAndSource_UseTheSameNamesAsTheirCards(
        // Parameter `analysisKey` bertipe `string` membawa nilai analysis kunci.
        string analysisKey, string metricKey, string formulaKey, string[] inputs)
    // Membuka scope metode FormulaAndSource_UseTheSameNamesAsTheirCards; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // FormulaAndSource_UseTheSameNamesAsTheirCards.
    {
        // Mengulangi setiap elemen `new[] { ”id”, ”en” }`; elemen saat ini disimpan sebagai `language` bertipe `var` untuk diproses oleh badan loop dalam
        // FormulaAndSource_UseTheSameNamesAsTheirCards.
        foreach (var language in new[] { "id", "en" })
        // Membuka scope loop setiap language dari `new[] { ”id”, ”en” }`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // FormulaAndSource_UseTheSameNamesAsTheirCards.
        {
            // Mendefinisikan fungsi lokal Translate dengan hasil `string`; fungsi ini dipakai oleh alur di dalam scope yang sama. Ekspresi hasilnya adalah
            // memanggil `UiText.Translate` dengan `language`, `key`.
            string Translate(string key) => UiText.Translate(language, key);
            // Mendefinisikan fungsi lokal Label dengan hasil `string`; fungsi ini dipakai oleh alur di dalam scope yang sama. Ekspresi hasilnya adalah
            // memanggil `PlayerMetricLabelFormatter.HumanizeMetricKey` dengan `key`, `Translate`.
            string Label(string key) => PlayerMetricLabelFormatter.HumanizeMetricKey(key, Translate);
            // Menyiapkan variabel lokal `formula` untuk nilai formula dengan memanggil `Translate` dengan `$”players.support.formula.{formulaKey}”`. Tipe
            // variabel disimpulkan dari ekspresi nilai awal.
            var formula = Translate($"players.support.formula.{formulaKey}");
            // Menyiapkan variabel lokal `source` untuk nilai source dengan memanggil `Translate` dengan `$”players.analysis.source.{analysisKey}”`. Tipe
            // variabel disimpulkan dari ekspresi nilai awal.
            var source = Translate($"players.analysis.source.{analysisKey}");

            // Menjalankan pemeriksaan hasil dengan `Assert.StartsWith` menggunakan `$”{Label(metricKey)} = ”`, `formula`, `StringComparison.Ordinal`;
            // ketidaksesuaian dengan ekspektasi membuat pengujian gagal dalam FormulaAndSource_UseTheSameNamesAsTheirCards.
            Assert.StartsWith($"{Label(metricKey)} = ", formula, StringComparison.Ordinal);
            // Mengulangi setiap elemen `inputs`; elemen saat ini disimpan sebagai `input` bertipe `var` untuk diproses oleh badan loop dalam
            // FormulaAndSource_UseTheSameNamesAsTheirCards.
            foreach (var input in inputs)
            // Membuka scope loop setiap input dari `inputs`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // FormulaAndSource_UseTheSameNamesAsTheirCards.
            {
                // Menyiapkan variabel lokal `label` untuk nilai label dengan memanggil `Label` dengan `input`. Tipe variabel disimpulkan dari ekspresi nilai awal.
                var label = Label(input);
                // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”_”`, `label`,
                // `StringComparison.Ordinal` dalam FormulaAndSource_UseTheSameNamesAsTheirCards.
                Assert.DoesNotContain("_", label, StringComparison.Ordinal);
                // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `label`, `formula`,
                // `StringComparison.Ordinal` dalam FormulaAndSource_UseTheSameNamesAsTheirCards.
                Assert.Contains(label, formula, StringComparison.Ordinal);
                // Memeriksa kebalikan kondisi `analysisKey.StartsWith(”happiness-portfolio”, StringComparison.Ordinal)`; blok if hanya dijalankan ketika kondisi
                // ini bernilai benar dalam FormulaAndSource_UseTheSameNamesAsTheirCards.
                if (!analysisKey.StartsWith("happiness-portfolio", StringComparison.Ordinal))
                // Membuka scope cabang if untuk kondisi `!analysisKey.StartsWith(”happiness-portfolio”, StringComparison.Ordinal)`; pernyataan/deklarasi berikut
                // berada di dalam batas blok ini dalam FormulaAndSource_UseTheSameNamesAsTheirCards.
                {
                    // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `label`, `source`,
                    // `StringComparison.Ordinal` dalam FormulaAndSource_UseTheSameNamesAsTheirCards.
                    Assert.Contains(label, source, StringComparison.Ordinal);
                // Menutup scope cabang if untuk kondisi `!analysisKey.StartsWith(”happiness-portfolio”, StringComparison.Ordinal)`; bagian berikut berada di luar
                // batas blok tersebut dalam FormulaAndSource_UseTheSameNamesAsTheirCards.
                }
            // Menutup scope loop setiap input dari `inputs`; bagian berikut berada di luar batas blok tersebut dalam
            // FormulaAndSource_UseTheSameNamesAsTheirCards.
            }
        // Menutup scope loop setiap language dari `new[] { ”id”, ”en” }`; bagian berikut berada di luar batas blok tersebut dalam
        // FormulaAndSource_UseTheSameNamesAsTheirCards.
        }
    // Menutup scope metode FormulaAndSource_UseTheSameNamesAsTheirCards; bagian berikut berada di luar batas blok tersebut dalam
    // FormulaAndSource_UseTheSameNamesAsTheirCards.
    }

    // menandai metode sebagai pengujian xUnit yang dijalankan untuk setiap kombinasi data.
    [Theory]
    // menyediakan satu kombinasi masukan pengujian (”players.raw.coins_held_current”, ”players.raw.coins_net_end_game”).
    [InlineData("players.raw.coins_held_current", "players.raw.coins_net_end_game")]
    // menyediakan satu kombinasi masukan pengujian (”players.raw.coins_net_end_game”, ”players.cashflow_journey.ending_cash”).
    [InlineData("players.raw.coins_net_end_game", "players.cashflow_journey.ending_cash")]
    // menyediakan satu kombinasi masukan pengujian (”players.raw.starting_coins”, ”players.cashflow_journey.starting_cash”).
    [InlineData("players.raw.starting_coins", "players.cashflow_journey.starting_cash")]
    // menyediakan satu kombinasi masukan pengujian (”players.raw.cash_out_total”, ”players.metric.total_cash_out”).
    [InlineData("players.raw.cash_out_total", "players.metric.total_cash_out")]
    // menyediakan satu kombinasi masukan pengujian (”players.raw.cash_out_total”, ”metric.cash_out”).
    [InlineData("players.raw.cash_out_total", "metric.cash_out")]
    // menyediakan satu kombinasi masukan pengujian (”players.raw.cash_in_total”, ”metric.cash_in”).
    [InlineData("players.raw.cash_in_total", "metric.cash_in")]
    // menyediakan satu kombinasi masukan pengujian (”players.raw.ingredients_used_total”, ”players.metric.ingredients_used_in_completed_orders”).
    [InlineData("players.raw.ingredients_used_total", "players.metric.ingredients_used_in_completed_orders")]
    // menyediakan satu kombinasi masukan pengujian (”players.raw.meal_order_income_total”, ”players.metric.meal_order_income”).
    [InlineData("players.raw.meal_order_income_total", "players.metric.meal_order_income")]
    // menyediakan satu kombinasi masukan pengujian (”players.raw.sharia_loans_outstanding_coins”, ”players.metric.outstanding_loan”).
    [InlineData("players.raw.sharia_loans_outstanding_coins", "players.metric.outstanding_loan")]
    // menyediakan satu kombinasi masukan pengujian (”players.raw.financial_goals_coins_total_invested”, ”players.metric.coins_committed_to_goals”).
    [InlineData("players.raw.financial_goals_coins_total_invested", "players.metric.coins_committed_to_goals")]
    // menyediakan satu kombinasi masukan pengujian (”players.metric.need_fulfillment_diversity_percent”, ”metric.fulfillment_diversity”).
    [InlineData("players.metric.need_fulfillment_diversity_percent", "metric.fulfillment_diversity")]
    // menyediakan satu kombinasi masukan pengujian (”players.raw.pension_fund_happiness_points”, ”players.metric.pension_points”).
    [InlineData("players.raw.pension_fund_happiness_points", "players.metric.pension_points")]
    // menyediakan satu kombinasi masukan pengujian (”players.raw.loan_penalty_if_unpaid”, ”players.metric.loan_penalty_points”).
    [InlineData("players.raw.loan_penalty_if_unpaid", "players.metric.loan_penalty_points")]
    // menyediakan satu kombinasi masukan pengujian (”metric.mission_penalty”, ”players.metric.mission_penalty_points”).
    [InlineData("metric.mission_penalty", "players.metric.mission_penalty_points")]
    // menyediakan satu kombinasi masukan pengujian (”metric.loan_penalty”, ”players.metric.loan_penalty_points”).
    [InlineData("metric.loan_penalty", "players.metric.loan_penalty_points")]
    // Mendefinisikan metode `SharedValues_KeepTheSameNameAcrossSummaryRawDataAndAnalysis` dengan hasil bertipe `void`; operasi ini menangani shared
    // nilai keep the same nama across summary raw data dan analysis. Masukan: Parameter `first` bertipe `string` membawa nilai first; Parameter
    // `second` bertipe `string` membawa nilai second.
    public void SharedValues_KeepTheSameNameAcrossSummaryRawDataAndAnalysis(string first, string second)
    // Membuka scope metode SharedValues_KeepTheSameNameAcrossSummaryRawDataAndAnalysis; pernyataan/deklarasi berikut berada di dalam batas blok ini
    // dalam SharedValues_KeepTheSameNameAcrossSummaryRawDataAndAnalysis.
    {
        // Mengulangi setiap elemen `new[] { ”id”, ”en” }`; elemen saat ini disimpan sebagai `language` bertipe `var` untuk diproses oleh badan loop dalam
        // SharedValues_KeepTheSameNameAcrossSummaryRawDataAndAnalysis.
        foreach (var language in new[] { "id", "en" })
        // Membuka scope loop setiap language dari `new[] { ”id”, ”en” }`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // SharedValues_KeepTheSameNameAcrossSummaryRawDataAndAnalysis.
        {
            // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`UiText.Translate(language, first)`,
            // `UiText.Translate(language, second)`); pengujian gagal jika keduanya berbeda dalam SharedValues_KeepTheSameNameAcrossSummaryRawDataAndAnalysis.
            Assert.Equal(UiText.Translate(language, first), UiText.Translate(language, second));
        // Menutup scope loop setiap language dari `new[] { ”id”, ”en” }`; bagian berikut berada di luar batas blok tersebut dalam
        // SharedValues_KeepTheSameNameAcrossSummaryRawDataAndAnalysis.
        }
    // Menutup scope metode SharedValues_KeepTheSameNameAcrossSummaryRawDataAndAnalysis; bagian berikut berada di luar batas blok tersebut dalam
    // SharedValues_KeepTheSameNameAcrossSummaryRawDataAndAnalysis.
    }

    // menandai metode sebagai pengujian xUnit yang dijalankan untuk setiap kombinasi data.
    [Theory]
    // menyediakan satu kombinasi masukan pengujian (”risk_readiness_percent”, ”risk_readiness”).
    [InlineData("risk_readiness_percent", "risk_readiness")]
    // menyediakan satu kombinasi masukan pengujian (”loan_burden_percent”, ”loan_burden”).
    [InlineData("loan_burden_percent", "loan_burden")]
    // menyediakan satu kombinasi masukan pengujian (”ingredient_utilization_percent”, ”ingredient_utilization”).
    [InlineData("ingredient_utilization_percent", "ingredient_utilization")]
    // Mendefinisikan metode `CurrentMetrics_DoNotUseAnUnrelatedLegacyExplanation` dengan hasil bertipe `void`; operasi ini menangani saat ini metrics
    // do not use an unrelated legacy explanation. Masukan: Parameter `key` bertipe `string` membawa nilai kunci; Parameter `explanation` bertipe
    // `string` membawa nilai explanation.
    public void CurrentMetrics_DoNotUseAnUnrelatedLegacyExplanation(string key, string explanation)
    // Membuka scope metode CurrentMetrics_DoNotUseAnUnrelatedLegacyExplanation; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // CurrentMetrics_DoNotUseAnUnrelatedLegacyExplanation.
    {
        // Menyiapkan variabel lokal `metric` untuk nilai metric dengan memanggil `PlayerMetricLabelFormatter.DescribeMetric` dengan `key`, `”50”`, `true`,
        // `true`, `”—”`, `text => text`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var metric = PlayerMetricLabelFormatter.DescribeMetric(key, "50", true, true, "—", text => text);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`$”players.support.meaning.{explanation}”`,
        // `metric.Explanation`); pengujian gagal jika keduanya berbeda dalam CurrentMetrics_DoNotUseAnUnrelatedLegacyExplanation.
        Assert.Equal($"players.support.meaning.{explanation}", metric.Explanation);
    // Menutup scope metode CurrentMetrics_DoNotUseAnUnrelatedLegacyExplanation; bagian berikut berada di luar batas blok tersebut dalam
    // CurrentMetrics_DoNotUseAnUnrelatedLegacyExplanation.
    }

    // menandai metode sebagai pengujian xUnit yang dijalankan untuk setiap kombinasi data.
    [Theory]
    // menyediakan satu kombinasi masukan pengujian (”id”).
    [InlineData("id")]
    // menyediakan satu kombinasi masukan pengujian (”en”).
    [InlineData("en")]
    // Mendefinisikan metode `DonationCommitment_IsNotNamedAsAmountRegularity` dengan hasil bertipe `void`; operasi ini menangani donasi commitment
    // berstatus not named as nominal regularity. Masukan: Parameter `language` bertipe `string` membawa nilai language.
    public void DonationCommitment_IsNotNamedAsAmountRegularity(string language)
    // Membuka scope metode DonationCommitment_IsNotNamedAsAmountRegularity; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // DonationCommitment_IsNotNamedAsAmountRegularity.
    {
        // Menjalankan pemeriksaan hasil dengan `Assert.NotEqual` menggunakan `UiText.Translate(language, ”players.metric.donation_commitment_score”)`,
        // `UiText.Translate(language, ”players.metric.donation_stability_index”)`; ketidaksesuaian dengan ekspektasi membuat pengujian gagal dalam
        // DonationCommitment_IsNotNamedAsAmountRegularity.
        Assert.NotEqual(
            // Meneruskan memanggil `UiText.Translate` dengan `language`, `”players.metric.donation_commitment_score”` sebagai argumen ke `Assert.NotEqual`;
            // Meneruskan `language` (nilai language) sebagai argumen ke `UiText.Translate`; Meneruskan nilai literal
            // `”players.metric.donation_commitment_score”` sebagai argumen ke `UiText.Translate`.
            UiText.Translate(language, "players.metric.donation_commitment_score"),
            // Meneruskan memanggil `UiText.Translate` dengan `language`, `”players.metric.donation_stability_index”` sebagai argumen ke `Assert.NotEqual`;
            // Meneruskan `language` (nilai language) sebagai argumen ke `UiText.Translate`; Meneruskan nilai literal
            // `”players.metric.donation_stability_index”` sebagai argumen ke `UiText.Translate`.
            UiText.Translate(language, "players.metric.donation_stability_index"));
    // Menutup scope metode DonationCommitment_IsNotNamedAsAmountRegularity; bagian berikut berada di luar batas blok tersebut dalam
    // DonationCommitment_IsNotNamedAsAmountRegularity.
    }

    // menandai metode sebagai pengujian xUnit yang dijalankan untuk setiap kombinasi data.
    [Theory]
    // menyediakan satu kombinasi masukan pengujian (”id”, ”Perubahan Koin per Kejadian”, ”Konteks Aksi”, ”Perubahan Koin”).
    [InlineData("id", "Perubahan Koin per Kejadian", "Konteks Aksi", "Perubahan Koin")]
    // menyediakan satu kombinasi masukan pengujian (”en”, ”Coin Change per Event”, ”Action Context”, ”Coin Change”).
    [InlineData("en", "Coin Change per Event", "Action Context", "Coin Change")]
    // Mendefinisikan metode `EventCashTimeline_UsesEventBasedNames` dengan hasil bertipe `void`; operasi ini menangani event uang tunai timeline uses
    // event based nama. Masukan: Parameter `language` bertipe `string` membawa nilai language; Parameter `timelineLabel` bertipe `string` membawa nilai
    // timeline label; Parameter `actionContextLabel` bertipe `string` membawa nilai aksi context label; Parameter `changeLabel` bertipe `string`
    // membawa nilai change label.
    public void EventCashTimeline_UsesEventBasedNames(
        // Parameter `language` bertipe `string` membawa nilai language.
        string language,
        // Parameter `timelineLabel` bertipe `string` membawa nilai timeline label.
        string timelineLabel,
        // Parameter `actionContextLabel` bertipe `string` membawa nilai aksi context label.
        string actionContextLabel,
        // Parameter `changeLabel` bertipe `string` membawa nilai change label.
        string changeLabel)
    // Membuka scope metode EventCashTimeline_UsesEventBasedNames; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // EventCashTimeline_UsesEventBasedNames.
    {
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`timelineLabel`, `UiText.Translate(language,
        // ”players.raw.net_income_per_turn”)`); pengujian gagal jika keduanya berbeda dalam EventCashTimeline_UsesEventBasedNames.
        Assert.Equal(timelineLabel, UiText.Translate(language, "players.raw.net_income_per_turn"));
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`actionContextLabel`,
        // `UiText.Translate(language, ”players.raw.action_slot”)`); pengujian gagal jika keduanya berbeda dalam EventCashTimeline_UsesEventBasedNames.
        Assert.Equal(actionContextLabel, UiText.Translate(language, "players.raw.action_slot"));
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`changeLabel`, `UiText.Translate(language,
        // ”players.raw.net”)`); pengujian gagal jika keduanya berbeda dalam EventCashTimeline_UsesEventBasedNames.
        Assert.Equal(changeLabel, UiText.Translate(language, "players.raw.net"));

        // Menyiapkan variabel lokal `metric` untuk nilai metric dengan memanggil `PlayerMetricLabelFormatter.DescribeMetric` dengan
        // `”net_income_per_turn”`, `”1”`, `false`, `true`, `”—”`, `key => UiText.Translate(language, key)`. Tipe variabel disimpulkan dari ekspresi nilai
        // awal.
        var metric = PlayerMetricLabelFormatter.DescribeMetric(
            // Meneruskan nilai literal `”net_income_per_turn”` sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`.
            "net_income_per_turn",
            // Meneruskan nilai literal `”1”` sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`.
            "1",
            // Meneruskan false, yaitu kondisi nonaktif/tidak terpenuhi sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`.
            false,
            // Meneruskan true, yaitu kondisi aktif/terpenuhi sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`.
            true,
            // Meneruskan nilai literal `”—”` sebagai argumen ke `PlayerMetricLabelFormatter.DescribeMetric`.
            "—",
            // Parameter `key` bertipe `` membawa nilai kunci.
            key => UiText.Translate(language, key));

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `language == ”id” ? ”Nilai positif” : ”A
        // positive value”`, `metric.Explanation` dalam EventCashTimeline_UsesEventBasedNames.
        Assert.Contains(language == "id" ? "Nilai positif" : "A positive value", metric.Explanation);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `language == ”id” ? ”nilai negatif” :
        // ”negative value”`, `metric.Explanation` dalam EventCashTimeline_UsesEventBasedNames.
        Assert.Contains(language == "id" ? "nilai negatif" : "negative value", metric.Explanation);
    // Menutup scope metode EventCashTimeline_UsesEventBasedNames; bagian berikut berada di luar batas blok tersebut dalam
    // EventCashTimeline_UsesEventBasedNames.
    }
// Menutup scope tipe PlayerMetricNamingConsistencyTests; bagian berikut berada di luar batas blok tersebut.
}
