// Fungsi file: Menyediakan transformasi, lokalisasi, atau koneksi UI melalui PlayerMetricLabelFormatter.
// Mengimpor namespace `System.Globalization` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Globalization;
// Mengimpor namespace `System.Text` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Infrastructure` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Infrastructure;

/// <summary>
/// Teks siap tampil untuk satu variabel pendukung pemain.
/// </summary>
// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `PlayerMetricPresentation`; sealed mencegah tipe ini diturunkan lagi.
public sealed record PlayerMetricPresentation(
    // Parameter `Label` bertipe `string` membawa nilai label.
    string Label,
    // Parameter `DisplayValue` bertipe `string` membawa nilai display nilai.
    string DisplayValue,
    // Parameter `Unit` bertipe `string` membawa nilai unit.
    string Unit,
    // Parameter `Explanation` bertipe `string` membawa nilai explanation.
    string Explanation,
    // Parameter `Guidance` bertipe `string` membawa nilai guidance.
    string Guidance,
    // Parameter `Recommendation` bertipe `string` membawa nilai recommendation.
    string Recommendation,
    // Parameter `Formula` bertipe `string` membawa nilai formula.
    string Formula,
    // Parameter `IsAdvancedOnly` bertipe `bool` membawa nilai berstatus advanced only.
    bool IsAdvancedOnly,
    // Parameter `State` bertipe `string` membawa keadaan permainan yang menjadi sumber atau hasil pembaruan.
    string State);

/// <summary>
/// Formatter label metrik pemain yang tidak bergantung pada Razor runtime.
/// </summary>
// Mendefinisikan tipe class `PlayerMetricLabelFormatter`.
public static class PlayerMetricLabelFormatter
// Membuka scope tipe PlayerMetricLabelFormatter; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendeklarasikan field bertipe `HashSet<string>`: `PercentageMetricKeys` menyimpan nilai percentage metric kunci dengan nilai awal objek baru
    // dengan tipe mengikuti konteks tujuan dan argumen (StringComparer.OrdinalIgnoreCase). readonly membatasi penggantian referensi/nilai field pada
    // deklarasi atau konstruktor. static membuat field menjadi milik tipe dan dibagikan antar instance.
    private static readonly HashSet<string> PercentageMetricKeys = new(StringComparer.OrdinalIgnoreCase)
    // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini.
    {
        // Menggunakan nilai literal `”cash_growth_percent”` sebagai bagian ekspresi yang sedang disusun.
        "cash_growth_percent",
        "happiness_source_diversity_percent",
        // Menggunakan nilai literal `”business_expense_share_percent”` sebagai bagian ekspresi yang sedang disusun.
        "business_expense_share_percent",
        // Menggunakan nilai literal `”meal_order_profit_margin_percent”` sebagai bagian ekspresi yang sedang disusun.
        "meal_order_profit_margin_percent",
        // Menggunakan nilai literal `”risk_readiness_percent”` sebagai bagian ekspresi yang sedang disusun.
        "risk_readiness_percent",
        // Menggunakan nilai literal `”loan_burden_percent”` sebagai bagian ekspresi yang sedang disusun.
        "loan_burden_percent",
        // Menggunakan nilai literal `”financial_goal_progress_percent”` sebagai bagian ekspresi yang sedang disusun.
        "financial_goal_progress_percent",
        // Menggunakan nilai literal `”income_action_focus_percent”` sebagai bagian ekspresi yang sedang disusun.
        "income_action_focus_percent",
        // Menggunakan nilai literal `”ingredient_utilization_percent”` sebagai bagian ekspresi yang sedang disusun.
        "ingredient_utilization_percent",
        // Menggunakan nilai literal `”long_term_action_share_percent”` sebagai bagian ekspresi yang sedang disusun.
        "long_term_action_share_percent",
        // Menggunakan nilai literal `”need_fulfillment_diversity_percent”` sebagai bagian ekspresi yang sedang disusun.
        "need_fulfillment_diversity_percent",
        // Menggunakan nilai literal `”net_worth_index”` sebagai bagian ekspresi yang sedang disusun.
        "net_worth_index",
        // Menggunakan nilai literal `”income_diversification_index”` sebagai bagian ekspresi yang sedang disusun.
        "income_diversification_index",
        // Menggunakan nilai literal `”income_diversification_ratio”` sebagai bagian ekspresi yang sedang disusun.
        "income_diversification_ratio",
        // Menggunakan nilai literal `”expense_management_efficiency”` sebagai bagian ekspresi yang sedang disusun.
        "expense_management_efficiency",
        // Menggunakan nilai literal `”business_profit_margin”` sebagai bagian ekspresi yang sedang disusun.
        "business_profit_margin",
        // Menggunakan nilai literal `”gold_roi_percentage”` sebagai bagian ekspresi yang sedang disusun.
        "gold_roi_percentage",
        // Menggunakan nilai literal `”risk_exposure_percentage”` sebagai bagian ekspresi yang sedang disusun.
        "risk_exposure_percentage",
        // Menggunakan nilai literal `”risk_mitigation_effectiveness”` sebagai bagian ekspresi yang sedang disusun.
        "risk_mitigation_effectiveness",
        // Menggunakan nilai literal `”debt_leverage_ratio”` sebagai bagian ekspresi yang sedang disusun.
        "debt_leverage_ratio",
        // Menggunakan nilai literal `”loan_repayment_discipline”` sebagai bagian ekspresi yang sedang disusun.
        "loan_repayment_discipline",
        // Menggunakan nilai literal `”goal_ambition”` sebagai bagian ekspresi yang sedang disusun.
        "goal_ambition",
        // Menggunakan nilai literal `”goal_ambition_index”` sebagai bagian ekspresi yang sedang disusun.
        "goal_ambition_index",
        // Menggunakan nilai literal `”action_efficiency_percent”` sebagai bagian ekspresi yang sedang disusun.
        "action_efficiency_percent",
        // Menggunakan nilai literal `”meal_order_success_rate”` sebagai bagian ekspresi yang sedang disusun.
        "meal_order_success_rate",
        // Menggunakan nilai literal `”planning_horizon_percent”` sebagai bagian ekspresi yang sedang disusun.
        "planning_horizon_percent",
        // Menggunakan nilai literal `”donation_aggressiveness_percent”` sebagai bagian ekspresi yang sedang disusun.
        "donation_aggressiveness_percent",
        // Menggunakan nilai literal `”donation_stability”` sebagai bagian ekspresi yang sedang disusun.
        "donation_stability",
        // Menggunakan nilai literal `”donation_stability_index”` sebagai bagian ekspresi yang sedang disusun.
        "donation_stability_index",
        // Menggunakan nilai literal `”risk_appetite_score_normalized”` sebagai bagian ekspresi yang sedang disusun.
        "risk_appetite_score_normalized"
    // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut.
    };

    // Mendeklarasikan field bertipe `HashSet<string>`: `FractionPercentageMetricKeys` menyimpan nilai fraction percentage metric kunci dengan nilai
    // awal objek baru dengan tipe mengikuti konteks tujuan dan argumen (StringComparer.OrdinalIgnoreCase). readonly membatasi penggantian
    // referensi/nilai field pada deklarasi atau konstruktor. static membuat field menjadi milik tipe dan dibagikan antar instance.
    private static readonly HashSet<string> FractionPercentageMetricKeys = new(StringComparer.OrdinalIgnoreCase)
    // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini.
    {
        // Menggunakan nilai literal `”risk_acceptance_rate”` sebagai bagian ekspresi yang sedang disusun.
        "risk_acceptance_rate",
        // Menggunakan nilai literal `”insurance_activation_rate”` sebagai bagian ekspresi yang sedang disusun.
        "insurance_activation_rate",
        // Menggunakan nilai literal `”insurance_coverage_rate”` sebagai bagian ekspresi yang sedang disusun.
        "insurance_coverage_rate",
        // Menggunakan nilai literal `”risk_cost_intensity”` sebagai bagian ekspresi yang sedang disusun.
        "risk_cost_intensity",
        // Menggunakan nilai literal `”debt_ratio”` sebagai bagian ekspresi yang sedang disusun.
        "debt_ratio",
        // Menggunakan nilai literal `”goal_attempt_rate”` sebagai bagian ekspresi yang sedang disusun.
        "goal_attempt_rate",
        // Menggunakan nilai literal `”goal_investment_rate”` sebagai bagian ekspresi yang sedang disusun.
        "goal_investment_rate",
        // Menggunakan nilai literal `”action_efficiency”` sebagai bagian ekspresi yang sedang disusun.
        "action_efficiency",
        // Menggunakan nilai literal `”diversity_score”` sebagai bagian ekspresi yang sedang disusun.
        "diversity_score",
        // Menggunakan nilai literal `”action_diversity_score_avg”` sebagai bagian ekspresi yang sedang disusun.
        "action_diversity_score_avg",
        // Menggunakan nilai literal `”planning_horizon”` sebagai bagian ekspresi yang sedang disusun.
        "planning_horizon",
        // Menggunakan nilai literal `”fulfillment_diversity”` sebagai bagian ekspresi yang sedang disusun.
        "fulfillment_diversity",
        // Menggunakan nilai literal `”fulfillment_diversity_document_formula”` sebagai bagian ekspresi yang sedang disusun.
        "fulfillment_diversity_document_formula",
        // Menggunakan nilai literal `”p_primary”` sebagai bagian ekspresi yang sedang disusun.
        "p_primary",
        // Menggunakan nilai literal `”p_secondary”` sebagai bagian ekspresi yang sedang disusun.
        "p_secondary",
        // Menggunakan nilai literal `”p_tertiary”` sebagai bagian ekspresi yang sedang disusun.
        "p_tertiary",
        // Menggunakan nilai literal `”donation_ratio”` sebagai bagian ekspresi yang sedang disusun.
        "donation_ratio",
        // Menggunakan nilai literal `”friday_participation_rate”` sebagai bagian ekspresi yang sedang disusun.
        "friday_participation_rate",
        // Menggunakan nilai literal `”income_share_i”` sebagai bagian ekspresi yang sedang disusun.
        "income_share_i",
        // Menggunakan nilai literal `”income_shares”` sebagai bagian ekspresi yang sedang disusun.
        "income_shares",
        // Menggunakan nilai literal `”primary_need_share”` sebagai bagian ekspresi yang sedang disusun.
        "primary_need_share",
        // Menggunakan nilai literal `”secondary_need_share”` sebagai bagian ekspresi yang sedang disusun.
        "secondary_need_share",
        // Menggunakan nilai literal `”tertiary_need_share”` sebagai bagian ekspresi yang sedang disusun.
        "tertiary_need_share",
        // Menggunakan nilai literal `”donated_resource_share”` sebagai bagian ekspresi yang sedang disusun.
        "donated_resource_share"
    // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut.
    };

    // Mendeklarasikan field bertipe `HashSet<string>`: `MultiplierMetricKeys` menyimpan nilai multiplier metric kunci dengan nilai awal objek baru
    // dengan tipe mengikuti konteks tujuan dan argumen (StringComparer.OrdinalIgnoreCase). readonly membatasi penggantian referensi/nilai field pada
    // deklarasi atau konstruktor. static membuat field menjadi milik tipe dan dibagikan antar instance.
    private static readonly HashSet<string> MultiplierMetricKeys = new(StringComparer.OrdinalIgnoreCase)
    // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini.
    {
        // Menggunakan nilai literal `”business_efficiency_ratio”` sebagai bagian ekspresi yang sedang disusun.
        "business_efficiency_ratio",
        // Menggunakan nilai literal `”growth_pattern_ratio”` sebagai bagian ekspresi yang sedang disusun.
        "growth_pattern_ratio"
    // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut.
    };

    // Mendeklarasikan field bertipe `string[]`: `AdvancedMetricFragments` menyimpan nilai advanced metric fragments dengan nilai awal `{ ”life_risk”,
    // ”insurance”, ”emergency”, ”financial_goal”, ”sharia_loan”, ”loan_”, ”risk_”, ”debt_”, ”goal_”, ”planning_horizon”, ”long_term” }`. readonly
    // membatasi penggantian referensi/nilai field pada deklarasi atau konstruktor. static membuat field menjadi milik tipe dan dibagikan antar
    // instance.
    private static readonly string[] AdvancedMetricFragments =
    // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini.
    {
        // Menggunakan nilai literal `”life_risk”` sebagai bagian ekspresi yang sedang disusun.
        "life_risk",
        // Menggunakan nilai literal `”insurance”` sebagai bagian ekspresi yang sedang disusun.
        "insurance",
        // Menggunakan nilai literal `”emergency”` sebagai bagian ekspresi yang sedang disusun.
        "emergency",
        // Menggunakan nilai literal `”financial_goal”` sebagai bagian ekspresi yang sedang disusun.
        "financial_goal",
        // Menggunakan nilai literal `”sharia_loan”` sebagai bagian ekspresi yang sedang disusun.
        "sharia_loan",
        // Menggunakan nilai literal `”loan_”` sebagai bagian ekspresi yang sedang disusun.
        "loan_",
        // Menggunakan nilai literal `”risk_”` sebagai bagian ekspresi yang sedang disusun.
        "risk_",
        // Menggunakan nilai literal `”debt_”` sebagai bagian ekspresi yang sedang disusun.
        "debt_",
        // Menggunakan nilai literal `”goal_”` sebagai bagian ekspresi yang sedang disusun.
        "goal_",
        // Menggunakan nilai literal `”planning_horizon”` sebagai bagian ekspresi yang sedang disusun.
        "planning_horizon",
        // Menggunakan nilai literal `”long_term”` sebagai bagian ekspresi yang sedang disusun.
        "long_term"
    // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut.
    };

    /// <summary>
    /// Mengubah key metrik menjadi label manusiawi dengan dukungan lokalisasi.
    /// </summary>
    // Mendefinisikan metode `HumanizeMetricKey` dengan hasil bertipe `string`. Mengubah key metrik menjadi label manusiawi dengan dukungan lokalisasi.
    // Masukan: Parameter `key` bertipe `string` membawa nilai kunci; Parameter `translate` bertipe `Func<string, string>` membawa nilai translate.
    public static string HumanizeMetricKey(string key, Func<string, string> translate)
    // Membuka scope metode HumanizeMetricKey; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam HumanizeMetricKey.
    {
        // Memeriksa memeriksa apakah `key` null, kosong, atau hanya berisi karakter spasi; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // HumanizeMetricKey.
        if (string.IsNullOrWhiteSpace(key))
        // Membuka scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(key)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // HumanizeMetricKey.
        {
            // Mengembalikan memanggil `translate` dengan `”players.details.metric_fallback”` kepada pemanggil dalam HumanizeMetricKey; eksekusi jalur ini
            // selesai setelah nilai hasil ditentukan.
            return translate("players.details.metric_fallback");
        // Menutup scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(key)`; bagian berikut berada di luar batas blok tersebut dalam
        // HumanizeMetricKey.
        }

        // Memeriksa membandingkan kesamaan `string` dengan `key`, `”value”`, `StringComparison.OrdinalIgnoreCase`; aturan perbandingan mengikuti overload
        // dan comparer yang diberikan; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam HumanizeMetricKey.
        if (string.Equals(key, "value", StringComparison.OrdinalIgnoreCase))
        // Membuka scope cabang if untuk kondisi `string.Equals(key, ”value”, StringComparison.OrdinalIgnoreCase)`; pernyataan/deklarasi berikut berada di
        // dalam batas blok ini dalam HumanizeMetricKey.
        {
            // Mengembalikan memanggil `translate` dengan `”common.value”` kepada pemanggil dalam HumanizeMetricKey; eksekusi jalur ini selesai setelah nilai
            // hasil ditentukan.
            return translate("common.value");
        // Menutup scope cabang if untuk kondisi `string.Equals(key, ”value”, StringComparison.OrdinalIgnoreCase)`; bagian berikut berada di luar batas blok
        // tersebut dalam HumanizeMetricKey.
        }

        // Memeriksa membandingkan kesamaan `string` dengan `key`, `”series”`, `StringComparison.OrdinalIgnoreCase`; aturan perbandingan mengikuti overload
        // dan comparer yang diberikan; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam HumanizeMetricKey.
        if (string.Equals(key, "series", StringComparison.OrdinalIgnoreCase))
        // Membuka scope cabang if untuk kondisi `string.Equals(key, ”series”, StringComparison.OrdinalIgnoreCase)`; pernyataan/deklarasi berikut berada di
        // dalam batas blok ini dalam HumanizeMetricKey.
        {
            // Mengembalikan memanggil `translate` dengan `”players.details.series”` kepada pemanggil dalam HumanizeMetricKey; eksekusi jalur ini selesai
            // setelah nilai hasil ditentukan.
            return translate("players.details.series");
        // Menutup scope cabang if untuk kondisi `string.Equals(key, ”series”, StringComparison.OrdinalIgnoreCase)`; bagian berikut berada di luar batas
        // blok tersebut dalam HumanizeMetricKey.
        }

        // Menyiapkan variabel lokal `normalized` untuk nilai normalized dengan memanggil `NormalizeMetricLexiconKey` dengan `key`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var normalized = NormalizeMetricLexiconKey(key);
        // Menyiapkan variabel lokal `aliasLexiconKey` untuk nilai alias lexicon kunci dengan hasil pemetaan `normalized` melalui cabang pola switch yang
        // cocok. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var aliasLexiconKey = normalized switch
        // Membuka scope pemetaan switch atas `normalized`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam HumanizeMetricKey.
        {
            // Untuk pola `”coins_net_end”`, menghasilkan nilai literal `”players.raw.coins_net_end_game”` sebagai hasil switch.
            "coins_net_end" => "players.raw.coins_net_end_game",
            // Untuk pola `”coins_net_end_game”`, menghasilkan nilai literal `”players.raw.coins_net_end_game”` sebagai hasil switch.
            "coins_net_end_game" => "players.raw.coins_net_end_game",
            // Untuk pola `_`, menghasilkan `string.Empty`, yaitu nilai kosong bawaan tipe terkait sebagai hasil switch.
            _ => string.Empty
        // Menutup scope pemetaan switch atas `normalized`; bagian berikut berada di luar batas blok tersebut dalam HumanizeMetricKey.
        };

        // Menyiapkan variabel lokal `localizedLabel` untuk nilai localized label dengan `TryTranslateMetricLexicon(aliasLexiconKey, translate)` bila tidak
        // null; jika null gunakan `TryTranslateMetricLexicon($”players.raw.{normalized}”, translate) ??
        // TryTranslateMetricLexicon($”players.group.{normalized}”, translate) ?? TryTranslateMetricLexicon($”players...` sebagai nilai pengganti. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var localizedLabel =
            // Melanjutkan pengolahan dengan memanggil `TryTranslateMetricLexicon` dengan `aliasLexiconKey`, `translate` dalam HumanizeMetricKey.
            TryTranslateMetricLexicon(aliasLexiconKey, translate) ??
            // Melanjutkan pengolahan dengan memanggil `TryTranslateMetricLexicon` dengan `$”players.raw.{normalized}”`, `translate` dalam HumanizeMetricKey.
            TryTranslateMetricLexicon($"players.raw.{normalized}", translate) ??
            // Melanjutkan pengolahan dengan memanggil `TryTranslateMetricLexicon` dengan `$”players.group.{normalized}”`, `translate` dalam HumanizeMetricKey.
            TryTranslateMetricLexicon($"players.group.{normalized}", translate) ??
            // Melanjutkan pengolahan dengan memanggil `TryTranslateMetricLexicon` dengan `$”players.metric.{normalized}”`, `translate` dalam HumanizeMetricKey.
            TryTranslateMetricLexicon($"players.metric.{normalized}", translate);
        // Memeriksa kebalikan kondisi `string.IsNullOrWhiteSpace(localizedLabel)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // HumanizeMetricKey.
        if (!string.IsNullOrWhiteSpace(localizedLabel))
        // Membuka scope cabang if untuk kondisi `!string.IsNullOrWhiteSpace(localizedLabel)`; pernyataan/deklarasi berikut berada di dalam batas blok ini
        // dalam HumanizeMetricKey.
        {
            // Mengembalikan `localizedLabel` (nilai localized label) kepada pemanggil dalam HumanizeMetricKey; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return localizedLabel;
        // Menutup scope cabang if untuk kondisi `!string.IsNullOrWhiteSpace(localizedLabel)`; bagian berikut berada di luar batas blok tersebut dalam
        // HumanizeMetricKey.
        }

        // Menyiapkan variabel lokal `text` untuk nilai text dengan membersihkan karakter tepi pada `normalized.Replace('_', ' ')` memakai tanpa argumen.
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var text = normalized.Replace('_', ' ').Trim();
        // Memeriksa perbandingan kesamaan antara `text.Length` dan `0`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam HumanizeMetricKey.
        if (text.Length == 0)
        // Membuka scope cabang if untuk kondisi `text.Length == 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam HumanizeMetricKey.
        {
            // Mengembalikan memanggil `translate` dengan `”players.details.metric_fallback”` kepada pemanggil dalam HumanizeMetricKey; eksekusi jalur ini
            // selesai setelah nilai hasil ditentukan.
            return translate("players.details.metric_fallback");
        // Menutup scope cabang if untuk kondisi `text.Length == 0`; bagian berikut berada di luar batas blok tersebut dalam HumanizeMetricKey.
        }

        // Mengembalikan penjumlahan/penggabungan antara `char.ToUpperInvariant(text[0])` dan `text[1..]` kepada pemanggil dalam HumanizeMetricKey; eksekusi
        // jalur ini selesai setelah nilai hasil ditentukan.
        return char.ToUpperInvariant(text[0]) + text[1..];
    // Menutup scope metode HumanizeMetricKey; bagian berikut berada di luar batas blok tersebut dalam HumanizeMetricKey.
    }

    /// <summary>
    /// Mengubah path metrik bertingkat menjadi label bersegmen.
    /// </summary>
    // Mendefinisikan metode `FormatMetricPathLabel` dengan hasil bertipe `string`. Mengubah path metrik bertingkat menjadi label bersegmen. Masukan:
    // Parameter `rawPath` bertipe `string` membawa nilai raw path; Parameter `translate` bertipe `Func<string, string>` membawa nilai translate.
    public static string FormatMetricPathLabel(string rawPath, Func<string, string> translate)
    // Membuka scope metode FormatMetricPathLabel; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam FormatMetricPathLabel.
    {
        // Memeriksa memeriksa apakah `rawPath` null, kosong, atau hanya berisi karakter spasi; blok if hanya dijalankan ketika kondisi ini bernilai benar
        // dalam FormatMetricPathLabel.
        if (string.IsNullOrWhiteSpace(rawPath))
        // Membuka scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(rawPath)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // FormatMetricPathLabel.
        {
            // Mengembalikan memanggil `translate` dengan `”players.details.metric_fallback”` kepada pemanggil dalam FormatMetricPathLabel; eksekusi jalur ini
            // selesai setelah nilai hasil ditentukan.
            return translate("players.details.metric_fallback");
        // Menutup scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(rawPath)`; bagian berikut berada di luar batas blok tersebut dalam
        // FormatMetricPathLabel.
        }

        // Menyiapkan variabel lokal `tokens` untuk nilai tokens dengan objek baru bertipe `List<string>` dengan nilai awal sesuai konstruktornya. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var tokens = new List<string>();
        // Menyiapkan variabel lokal `buffer` untuk nilai buffer dengan objek baru bertipe `StringBuilder` dengan nilai awal sesuai konstruktornya. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var buffer = new StringBuilder();

        // Mendefinisikan fungsi lokal FlushBuffer dengan hasil `void`; fungsi ini dipakai oleh alur di dalam scope yang sama.
        void FlushBuffer()
        // Membuka scope fungsi lokal FlushBuffer; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam FlushBuffer.
        {
            // Memeriksa perbandingan kesamaan antara `buffer.Length` dan `0`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam FlushBuffer.
            if (buffer.Length == 0)
            // Membuka scope cabang if untuk kondisi `buffer.Length == 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam FlushBuffer.
            {
                // Mengakhiri eksekusi lebih awal dalam FlushBuffer tanpa mengembalikan nilai; pernyataan sesudah return pada jalur ini tidak dijalankan.
                return;
            // Menutup scope cabang if untuk kondisi `buffer.Length == 0`; bagian berikut berada di luar batas blok tersebut dalam FlushBuffer.
            }

            // Menjalankan menambahkan `HumanizeMetricKey(buffer.ToString(), translate)` ke `tokens` dalam FlushBuffer.
            tokens.Add(HumanizeMetricKey(buffer.ToString(), translate));
            // Menjalankan mengosongkan seluruh elemen `buffer` dalam FlushBuffer.
            buffer.Clear();
        // Menutup scope fungsi lokal FlushBuffer; bagian berikut berada di luar batas blok tersebut dalam FlushBuffer.
        }

        // Memulai loop dengan inisialisasi `var index = 0`, berjalan selama `index < rawPath.Length`, lalu memperbarui pencacah melalui `index++` dalam
        // FormatMetricPathLabel.
        for (var index = 0; index < rawPath.Length; index++)
        // Membuka scope loop dengan syarat `index < rawPath.Length`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // FormatMetricPathLabel.
        {
            // Menyiapkan variabel lokal `current` untuk nilai saat ini dengan `rawPath[index]`, yaitu elemen koleksi yang dipilih melalui indeks atau kunci
            // tersebut. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var current = rawPath[index];
            // Memeriksa perbandingan kesamaan antara `current` dan `'.'`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
            // FormatMetricPathLabel.
            if (current == '.')
            // Membuka scope cabang if untuk kondisi `current == '.'`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam FormatMetricPathLabel.
            {
                // Menjalankan memanggil `FlushBuffer` dengan tanpa argumen dalam FormatMetricPathLabel.
                FlushBuffer();
                // Melewati sisa pernyataan pada iterasi saat ini dan melanjutkan ke elemen/iterasi berikutnya dalam FormatMetricPathLabel.
                continue;
            // Menutup scope cabang if untuk kondisi `current == '.'`; bagian berikut berada di luar batas blok tersebut dalam FormatMetricPathLabel.
            }

            // Memeriksa perbandingan kesamaan antara `current` dan `'['`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
            // FormatMetricPathLabel.
            if (current == '[')
            // Membuka scope cabang if untuk kondisi `current == '['`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam FormatMetricPathLabel.
            {
                // Menjalankan memanggil `FlushBuffer` dengan tanpa argumen dalam FormatMetricPathLabel.
                FlushBuffer();

                // Menyiapkan variabel lokal `closeIndex` untuk nilai close index dengan memanggil `rawPath.IndexOf` dengan `']'`, `index`. Tipe variabel
                // disimpulkan dari ekspresi nilai awal.
                var closeIndex = rawPath.IndexOf(']', index);
                // Memeriksa pemeriksaan lebih besar antara `closeIndex` dan `index`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
                // FormatMetricPathLabel.
                if (closeIndex > index)
                // Membuka scope cabang if untuk kondisi `closeIndex > index`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // FormatMetricPathLabel.
                {
                    // Menyiapkan variabel lokal `indexToken` untuk nilai index token dengan memanggil `rawPath.Substring` dengan `index + 1`, `closeIndex - index - 1`.
                    // Tipe variabel disimpulkan dari ekspresi nilai awal.
                    var indexToken = rawPath.Substring(index + 1, closeIndex - index - 1);
                    // Memeriksa mencoba mengonversi `indexToken`, `var parsedIndex` melalui `int.TryParse`; keberhasilan dilaporkan sebagai boolean dan hasil
                    // ditempatkan pada argumen out; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam FormatMetricPathLabel.
                    if (int.TryParse(indexToken, out var parsedIndex))
                    // Membuka scope cabang if untuk kondisi `int.TryParse(indexToken, out var parsedIndex)`; pernyataan/deklarasi berikut berada di dalam batas blok
                    // ini dalam FormatMetricPathLabel.
                    {
                        // Menjalankan menambahkan `$”{translate(”players.details.item”)} {parsedIndex + 1}”` ke `tokens` dalam FormatMetricPathLabel.
                        tokens.Add($"{translate("players.details.item")} {parsedIndex + 1}");
                    // Menutup scope cabang if untuk kondisi `int.TryParse(indexToken, out var parsedIndex)`; bagian berikut berada di luar batas blok tersebut dalam
                    // FormatMetricPathLabel.
                    }
                    // Menjalankan cabang alternatif ketika kondisi if sebelumnya tidak terpenuhi dalam FormatMetricPathLabel.
                    else if (!string.IsNullOrWhiteSpace(indexToken))
                    // Membuka scope cabang if untuk kondisi `!string.IsNullOrWhiteSpace(indexToken)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                    // FormatMetricPathLabel.
                    {
                        // Menjalankan menambahkan `$”{translate(”players.details.item”)} {indexToken}”` ke `tokens` dalam FormatMetricPathLabel.
                        tokens.Add($"{translate("players.details.item")} {indexToken}");
                    // Menutup scope cabang if untuk kondisi `!string.IsNullOrWhiteSpace(indexToken)`; bagian berikut berada di luar batas blok tersebut dalam
                    // FormatMetricPathLabel.
                    }

                    // Memperbarui `index` menggunakan `closeIndex` (nilai close index) dalam FormatMetricPathLabel.
                    index = closeIndex;
                    // Melewati sisa pernyataan pada iterasi saat ini dan melanjutkan ke elemen/iterasi berikutnya dalam FormatMetricPathLabel.
                    continue;
                // Menutup scope cabang if untuk kondisi `closeIndex > index`; bagian berikut berada di luar batas blok tersebut dalam FormatMetricPathLabel.
                }
            // Menutup scope cabang if untuk kondisi `current == '['`; bagian berikut berada di luar batas blok tersebut dalam FormatMetricPathLabel.
            }

            // Menjalankan menggabungkan `buffer` dengan `current` pada urutan hasil dalam FormatMetricPathLabel.
            buffer.Append(current);
        // Menutup scope loop dengan syarat `index < rawPath.Length`; bagian berikut berada di luar batas blok tersebut dalam FormatMetricPathLabel.
        }

        // Menjalankan memanggil `FlushBuffer` dengan tanpa argumen dalam FormatMetricPathLabel.
        FlushBuffer();

        // Menyiapkan variabel lokal `displayTokens` untuk nilai display tokens dengan mematerialisasi urutan `tokens .Where(token =>
        // !string.IsNullOrWhiteSpace(token))` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var displayTokens = tokens
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(token => !string.IsNullOrWhiteSpace(token)) dalam FormatMetricPathLabel;
            // token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Where(token => !string.IsNullOrWhiteSpace(token))
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList(); dalam FormatMetricPathLabel; token pada baris ini menyambungkan
            // bagian kode sebelum dan sesudahnya.
            .ToList();

        // Memeriksa perbandingan kesamaan antara `displayTokens.Count` dan `0`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // FormatMetricPathLabel.
        if (displayTokens.Count == 0)
        // Membuka scope cabang if untuk kondisi `displayTokens.Count == 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // FormatMetricPathLabel.
        {
            // Mengembalikan memanggil `HumanizeMetricKey` dengan `rawPath`, `translate` kepada pemanggil dalam FormatMetricPathLabel; eksekusi jalur ini
            // selesai setelah nilai hasil ditentukan.
            return HumanizeMetricKey(rawPath, translate);
        // Menutup scope cabang if untuk kondisi `displayTokens.Count == 0`; bagian berikut berada di luar batas blok tersebut dalam FormatMetricPathLabel.
        }

        // Mengembalikan memanggil `string.Join` dengan `” - ”`, `displayTokens` kepada pemanggil dalam FormatMetricPathLabel; eksekusi jalur ini selesai
        // setelah nilai hasil ditentukan.
        return string.Join(" - ", displayTokens);
    // Menutup scope metode FormatMetricPathLabel; bagian berikut berada di luar batas blok tersebut dalam FormatMetricPathLabel.
    }

    /// <summary>
    /// Menambahkan satuan, arti, cara baca, dan status mode pada nilai metrik mentah maupun turunan.
    /// </summary>
    // Mendefinisikan metode `DescribeMetric` dengan hasil bertipe `PlayerMetricPresentation`. Menambahkan satuan, arti, cara baca, dan status mode pada
    // nilai metrik mentah maupun turunan. Masukan: Parameter `rawPath` bertipe `string` membawa nilai raw path; Parameter `rawValue` bertipe `string`
    // membawa nilai raw nilai; Parameter `isDerived` bertipe `bool` membawa nilai berstatus derived; Parameter `isAdvancedMode` bertipe `bool` membawa
    // nilai berstatus advanced mode; Parameter `nullText` bertipe `string` membawa nilai null text; Parameter `translate` bertipe `Func<string,
    // string>` membawa nilai translate.
    public static PlayerMetricPresentation DescribeMetric(
        // Parameter `rawPath` bertipe `string` membawa nilai raw path.
        string rawPath,
        // Parameter `rawValue` bertipe `string` membawa nilai raw nilai.
        string rawValue,
        // Parameter `isDerived` bertipe `bool` membawa nilai berstatus derived.
        bool isDerived,
        // Parameter `isAdvancedMode` bertipe `bool` membawa nilai berstatus advanced mode.
        bool isAdvancedMode,
        // Parameter `nullText` bertipe `string` membawa nilai null text.
        string nullText,
        // Parameter `translate` bertipe `Func<string, string>` membawa nilai translate.
        Func<string, string> translate)
    // Membuka scope metode DescribeMetric; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam DescribeMetric.
    {
        // Menyiapkan variabel lokal `label` untuk nilai label dengan memanggil `FormatMetricPathLabel` dengan `rawPath`, `translate`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var label = FormatMetricPathLabel(rawPath, translate);
        // Menyiapkan variabel lokal `metricKey` untuk nilai metric kunci dengan memanggil `GetRootMetricKey` dengan `rawPath`. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var metricKey = GetRootMetricKey(rawPath);
        // Menyiapkan variabel lokal `leafMetricKey` untuk nilai leaf metric kunci dengan memanggil `GetLeafMetricKey` dengan `rawPath`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var leafMetricKey = GetLeafMetricKey(rawPath);
        // Menyiapkan variabel lokal `normalizedPath` untuk nilai normalized path dengan memanggil `NormalizeMetricLexiconKey` dengan `rawPath`. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var normalizedPath = NormalizeMetricLexiconKey(rawPath);
        // Menyiapkan variabel lokal `isAdvancedOnly` untuk nilai berstatus advanced only dengan memeriksa apakah `AdvancedMetricFragments` memiliki
        // setidaknya satu elemen yang memenuhi `fragment => normalizedPath.Contains(fragment, StringComparison.OrdinalIgnoreCase)`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var isAdvancedOnly = AdvancedMetricFragments.Any(fragment =>
            // Meneruskan `fragment` (nilai fragment) sebagai argumen ke `normalizedPath.Contains`; Meneruskan `StringComparison.OrdinalIgnoreCase` (nilai
            // ordinal ignore case) sebagai argumen ke `normalizedPath.Contains`.
            normalizedPath.Contains(fragment, StringComparison.OrdinalIgnoreCase));
        // Menyiapkan variabel lokal `isUnavailable` untuk nilai berstatus unavailable dengan gabungan syarat OR: setidaknya satu kondisi wajib benar antara
        // `string.IsNullOrWhiteSpace(rawValue) || string.Equals(rawValue, nullText, StringComparison.OrdinalIgnoreCase)` dan `rawValue is ”[]” or ”{}”`;
        // sisi kanan diperiksa hanya jika sisi kiri salah. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var isUnavailable = string.IsNullOrWhiteSpace(rawValue) ||
                            // Melanjutkan pengolahan dengan membandingkan kesamaan `string` dengan `rawValue`, `nullText`, `StringComparison.OrdinalIgnoreCase`; aturan
                            // perbandingan mengikuti overload dan comparer yang diberikan dalam DescribeMetric.
                            string.Equals(rawValue, nullText, StringComparison.OrdinalIgnoreCase) ||
                            // Menggunakan `rawValue` (nilai raw nilai) sebagai bagian ekspresi yang sedang disusun dalam DescribeMetric.
                            rawValue is "[]" or "{}";

        // Menyiapkan variabel lokal `unitKey` untuk nilai unit kunci dengan memanggil `ResolveUnitKey` dengan `metricKey`, `leafMetricKey`,
        // `normalizedPath`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var unitKey = ResolveUnitKey(metricKey, leafMetricKey, normalizedPath);
        // Menyiapkan variabel lokal `unit` untuk nilai unit dengan hasil pemilihan bersyarat: ketika `string.IsNullOrWhiteSpace(unitKey)` benar gunakan
        // `string.Empty`, jika tidak gunakan `translate(unitKey)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var unit = string.IsNullOrWhiteSpace(unitKey) ? string.Empty : translate(unitKey);
        // Menyiapkan variabel lokal `hasNumericValue` untuk nilai memiliki numerik nilai dengan memanggil `TryParseMetricNumber` dengan `rawValue`, `var
        // numericValue`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var hasNumericValue = TryParseMetricNumber(rawValue, out var numericValue);
        var isCashGrowthMetric = metricKey is "cash_growth_percent" or "net_worth_index";
        if (hasNumericValue && isCashGrowthMetric)
        {
            // Snapshot menyimpan perbandingan saldo (100 = koin awal); tampilkan perubahan dari awal.
            numericValue -= 100;
        }
        // Menyiapkan variabel lokal `noLoanRecorded` untuk nilai no pinjaman recorded dengan gabungan syarat AND: kedua kondisi wajib benar antara
        // `metricKey == ”day_when_debt_introduced” && isUnavailable` dan `isAdvancedMode`; sisi kanan diperiksa hanya jika sisi kiri benar. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var noLoanRecorded = metricKey == "day_when_debt_introduced" &&
                             // Menggunakan `isUnavailable` (nilai berstatus unavailable) sebagai bagian ekspresi yang sedang disusun dalam DescribeMetric.
                             isUnavailable &&
                             // Menggunakan `isAdvancedMode` (nilai berstatus advanced mode) sebagai bagian ekspresi yang sedang disusun dalam DescribeMetric.
                             isAdvancedMode;
        // Menyiapkan variabel lokal `isGameDayMetric` untuk nilai berstatus game hari metric dengan memanggil `IsGameDayMetric` dengan `metricKey`. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var isGameDayMetric = IsGameDayMetric(metricKey);
        // Menyiapkan variabel lokal `state` untuk keadaan permainan yang menjadi sumber atau hasil pembaruan dengan hasil pemilihan bersyarat: ketika
        // `isAdvancedOnly && !isAdvancedMode` benar gunakan `”not_applicable”`, jika tidak gunakan `noLoanRecorded ? ”recorded” : isUnavailable ?
        // ”unavailable” : hasNumericValue && isGameDayMetric ? ”recorded” : hasNumericValue && Math.Abs(numericValue) < 0.0000001 ? ”zero” ...`. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var state = isAdvancedOnly && !isAdvancedMode
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: ”not_applicable” dalam DescribeMetric.
            ? "not_applicable"
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: noLoanRecorded dalam DescribeMetric.
            : noLoanRecorded
                // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: ”recorded” dalam DescribeMetric.
                ? "recorded"
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: isUnavailable dalam DescribeMetric.
            : isUnavailable
                // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: ”unavailable” dalam DescribeMetric.
                ? "unavailable"
                // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: hasNumericValue && isGameDayMetric dalam DescribeMetric.
                : hasNumericValue && isGameDayMetric
                    // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: ”recorded” dalam DescribeMetric.
                    ? "recorded"
                // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: hasNumericValue && Math.Abs(numericValue) < 0.0000001 dalam
                // DescribeMetric.
                : hasNumericValue && Math.Abs(numericValue) < 0.0000001
                    // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: ”zero” dalam DescribeMetric.
                    ? "zero"
                    // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: ”recorded”; dalam DescribeMetric.
                    : "recorded";

        // Menyiapkan variabel lokal `displayNumericValue` untuk nilai display numerik nilai dengan hasil pemilihan bersyarat: ketika
        // `FractionPercentageMetricKeys.Contains(metricKey) || FractionPercentageMetricKeys.Contains(leafMetricKey)` benar gunakan `numericValue * 100`,
        // jika tidak gunakan `numericValue`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var displayNumericValue = FractionPercentageMetricKeys.Contains(metricKey) ||
                                  // Melanjutkan pengolahan dengan memeriksa apakah `FractionPercentageMetricKeys` memuat `leafMetricKey` dalam DescribeMetric.
                                  FractionPercentageMetricKeys.Contains(leafMetricKey)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: numericValue * 100 dalam DescribeMetric.
            ? numericValue * 100
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: numericValue; dalam DescribeMetric.
            : numericValue;
        // Menyiapkan variabel lokal `displayValue` untuk nilai display nilai dengan hasil pemilihan bersyarat: ketika `state is ”not_applicable” or
        // ”unavailable”` benar gunakan `translate(”players.support.value.unavailable”)`, jika tidak gunakan `hasNumericValue ?
        // FormatDisplayNumber(displayNumericValue) : rawValue`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var displayValue = state is "not_applicable" or "unavailable"
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: translate(”players.support.value.unavailable”) dalam DescribeMetric.
            ? translate("players.support.value.unavailable")
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: hasNumericValue dalam DescribeMetric.
            : hasNumericValue
                // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: FormatDisplayNumber(displayNumericValue) dalam DescribeMetric.
                ? isCashGrowthMetric
                    ? displayNumericValue.ToString("+0.##;-0.##;0", CultureInfo.CurrentCulture)
                    : FormatDisplayNumber(displayNumericValue)
                // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: rawValue; dalam DescribeMetric.
                : rawValue;
        // Memeriksa `noLoanRecorded` (nilai no pinjaman recorded); blok if hanya dijalankan ketika kondisi ini bernilai benar dalam DescribeMetric.
        if (noLoanRecorded)
        // Membuka scope cabang if untuk kondisi `noLoanRecorded`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam DescribeMetric.
        {
            // Memperbarui `displayValue` menggunakan memanggil `translate` dengan `”players.support.value.no_loan_recorded”` dalam DescribeMetric.
            displayValue = translate("players.support.value.no_loan_recorded");
            // Memperbarui `unit` menggunakan `string.Empty`, yaitu nilai kosong bawaan tipe terkait dalam DescribeMetric.
            unit = string.Empty;
        // Menutup scope cabang if untuk kondisi `noLoanRecorded`; bagian berikut berada di luar batas blok tersebut dalam DescribeMetric.
        }
        // Menjalankan cabang alternatif ketika kondisi if sebelumnya tidak terpenuhi dalam DescribeMetric.
        else if (hasNumericValue && isGameDayMetric)
        // Membuka scope cabang if untuk kondisi `hasNumericValue && isGameDayMetric`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // DescribeMetric.
        {
            // Memperbarui `displayValue` menggunakan hasil pemilihan bersyarat: ketika `numericValue <= 0` benar gunakan `translate(metricKey ==
            // ”day_when_debt_introduced” ? ”players.support.value.preparation_loan” : ”players.support.value.preparation”)`, jika tidak gunakan `string.Format(
            // CultureInfo.CurrentCulture, translate(”players.support.value.day_index”), FormatDisplayNumber(numericValue))` dalam DescribeMetric.
            displayValue = numericValue <= 0
                // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: translate(metricKey == ”day_when_debt_introduced” dalam
                // DescribeMetric.
                ? translate(metricKey == "day_when_debt_introduced"
                    // Meneruskan hasil pemilihan bersyarat: ketika `metricKey == ”day_when_debt_introduced”` benar gunakan `”players.support.value.preparation_loan”`,
                    // jika tidak gunakan `”players.support.value.preparation”` sebagai argumen ke `translate`.
                    ? "players.support.value.preparation_loan"
                    // Meneruskan hasil pemilihan bersyarat: ketika `metricKey == ”day_when_debt_introduced”` benar gunakan `”players.support.value.preparation_loan”`,
                    // jika tidak gunakan `”players.support.value.preparation”` sebagai argumen ke `translate`.
                    : "players.support.value.preparation")
                // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: string.Format( dalam DescribeMetric.
                : string.Format(
                    // Meneruskan `CultureInfo.CurrentCulture` (nilai saat ini culture) sebagai argumen ke `string.Format`.
                    CultureInfo.CurrentCulture,
                    // Meneruskan memanggil `translate` dengan `”players.support.value.day_index”` sebagai argumen ke `string.Format`; Meneruskan nilai literal
                    // `”players.support.value.day_index”` sebagai argumen ke `translate`.
                    translate("players.support.value.day_index"),
                    // Meneruskan memanggil `FormatDisplayNumber` dengan `numericValue` sebagai argumen ke `string.Format`; Meneruskan `numericValue` (nilai numerik
                    // nilai) sebagai argumen ke `FormatDisplayNumber`.
                    FormatDisplayNumber(numericValue));
            // Memperbarui `unit` menggunakan `string.Empty`, yaitu nilai kosong bawaan tipe terkait dalam DescribeMetric.
            unit = string.Empty;
        // Menutup scope cabang if untuk kondisi `hasNumericValue && isGameDayMetric`; bagian berikut berada di luar batas blok tersebut dalam
        // DescribeMetric.
        }
        // Menjalankan cabang alternatif ketika kondisi if sebelumnya tidak terpenuhi dalam DescribeMetric.
        else if (metricKey == "pension_fund_rank_per_game" && hasNumericValue && numericValue > 0)
        // Membuka scope cabang if untuk kondisi `metricKey == ”pension_fund_rank_per_game” && hasNumericValue && numericValue > 0`; pernyataan/deklarasi
        // berikut berada di dalam batas blok ini dalam DescribeMetric.
        {
            // Memperbarui `displayValue` menggunakan memanggil `string.Format` dengan `CultureInfo.CurrentCulture`,
            // `translate(”players.support.value.rank_position”)`, `FormatDisplayNumber(numericValue)` dalam DescribeMetric.
            displayValue = string.Format(
                // Meneruskan `CultureInfo.CurrentCulture` (nilai saat ini culture) sebagai argumen ke `string.Format`.
                CultureInfo.CurrentCulture,
                // Meneruskan memanggil `translate` dengan `”players.support.value.rank_position”` sebagai argumen ke `string.Format`; Meneruskan nilai literal
                // `”players.support.value.rank_position”` sebagai argumen ke `translate`.
                translate("players.support.value.rank_position"),
                // Meneruskan memanggil `FormatDisplayNumber` dengan `numericValue` sebagai argumen ke `string.Format`; Meneruskan `numericValue` (nilai numerik
                // nilai) sebagai argumen ke `FormatDisplayNumber`.
                FormatDisplayNumber(numericValue));
            // Memperbarui `unit` menggunakan `string.Empty`, yaitu nilai kosong bawaan tipe terkait dalam DescribeMetric.
            unit = string.Empty;
        // Menutup scope cabang if untuk kondisi `metricKey == ”pension_fund_rank_per_game” && hasNumericValue && numericValue > 0`; bagian berikut berada
        // di luar batas blok tersebut dalam DescribeMetric.
        }
        // Memeriksa hasil pencocokan `state` dengan pola `”not_applicable” or ”unavailable”`; blok if hanya dijalankan ketika kondisi ini bernilai benar
        // dalam DescribeMetric.
        if (state is "not_applicable" or "unavailable")
        // Membuka scope cabang if untuk kondisi `state is ”not_applicable” or ”unavailable”`; pernyataan/deklarasi berikut berada di dalam batas blok ini
        // dalam DescribeMetric.
        {
            // Memperbarui `unit` menggunakan `string.Empty`, yaitu nilai kosong bawaan tipe terkait dalam DescribeMetric.
            unit = string.Empty;
        // Menutup scope cabang if untuk kondisi `state is ”not_applicable” or ”unavailable”`; bagian berikut berada di luar batas blok tersebut dalam
        // DescribeMetric.
        }

        // Menyiapkan variabel lokal `explanationKey` untuk nilai explanation kunci dengan memanggil `ResolveExplanationKey` dengan `metricKey`,
        // `isDerived`, `unitKey`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var explanationKey = ResolveExplanationKey(metricKey, isDerived, unitKey);
        // Menyiapkan variabel lokal `explanation` untuk nilai explanation dengan memanggil `string.Format` dengan `CultureInfo.CurrentCulture`,
        // `translate(explanationKey)`, `label`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var explanation = string.Format(
            // Meneruskan `CultureInfo.CurrentCulture` (nilai saat ini culture) sebagai argumen ke `string.Format`.
            CultureInfo.CurrentCulture,
            // Meneruskan memanggil `translate` dengan `explanationKey` sebagai argumen ke `string.Format`; Meneruskan `explanationKey` (nilai explanation
            // kunci) sebagai argumen ke `translate`.
            translate(explanationKey),
            // Meneruskan `label` (nilai label) sebagai argumen ke `string.Format`.
            label);
        // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `metricKey == ”need_cards_owned_current”` dan `isAdvancedMode`; sisi kanan
        // diperiksa hanya jika sisi kiri benar; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam DescribeMetric.
        if (metricKey == "need_cards_owned_current" && isAdvancedMode)
        // Membuka scope cabang if untuk kondisi `metricKey == ”need_cards_owned_current” && isAdvancedMode`; pernyataan/deklarasi berikut berada di dalam
        // batas blok ini dalam DescribeMetric.
        {
            // Memperbarui `explanation` dengan menambahkan penjumlahan/penggabungan antara `” ”` dan
            // `translate(”players.support.meaning.need_cards_sold_note”)` dalam DescribeMetric.
            explanation += " " + translate("players.support.meaning.need_cards_sold_note");
        // Menutup scope cabang if untuk kondisi `metricKey == ”need_cards_owned_current” && isAdvancedMode`; bagian berikut berada di luar batas blok
        // tersebut dalam DescribeMetric.
        }
        // Menyiapkan variabel lokal `guidanceKey` untuk nilai guidance kunci dengan hasil pemetaan `state` melalui cabang pola switch yang cocok. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var guidanceKey = state switch
        // Membuka scope pemetaan switch atas `state`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam DescribeMetric.
        {
            // Untuk pola `”not_applicable”`, menghasilkan nilai literal `”players.support.guide.not_applicable”` sebagai hasil switch.
            "not_applicable" => "players.support.guide.not_applicable",
            // Untuk pola `”unavailable”`, menghasilkan nilai literal `”players.support.guide.unavailable”` sebagai hasil switch.
            "unavailable" => "players.support.guide.unavailable",
            // Untuk pola `_`, menghasilkan memanggil `ResolveGuidanceKey` dengan `metricKey`, `numericValue`, `hasNumericValue`, `isDerived`, `unitKey` sebagai
            // hasil switch.
            _ => ResolveGuidanceKey(metricKey, numericValue, hasNumericValue, isDerived, unitKey)
        // Menutup scope pemetaan switch atas `state`; bagian berikut berada di luar batas blok tersebut dalam DescribeMetric.
        };
        // Menyiapkan variabel lokal `recommendationKey` untuk nilai recommendation kunci dengan hasil pemetaan `state` melalui cabang pola switch yang
        // cocok. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var recommendationKey = state switch
        // Membuka scope pemetaan switch atas `state`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam DescribeMetric.
        {
            // Untuk pola `”not_applicable”`, menghasilkan nilai literal `”players.support.recommendation.not_applicable”` sebagai hasil switch.
            "not_applicable" => "players.support.recommendation.not_applicable",
            // Untuk pola `”unavailable”`, menghasilkan nilai literal `”players.support.recommendation.unavailable”` sebagai hasil switch.
            "unavailable" => "players.support.recommendation.unavailable",
            // Untuk pola `_`, menghasilkan memanggil `ResolveRecommendationKey` dengan `metricKey`, `numericValue`, `hasNumericValue` sebagai hasil switch.
            _ => ResolveRecommendationKey(metricKey, numericValue, hasNumericValue)
        // Menutup scope pemetaan switch atas `state`; bagian berikut berada di luar batas blok tersebut dalam DescribeMetric.
        };
        // Menyiapkan variabel lokal `formulaKey` untuk nilai formula kunci dengan hasil pemilihan bersyarat: ketika `isDerived` benar gunakan
        // `ResolveFormulaKey(metricKey)`, jika tidak gunakan `string.Empty`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var formulaKey = isDerived ? ResolveFormulaKey(metricKey) : string.Empty;

        // Mengembalikan objek baru bertipe `PlayerMetricPresentation` dengan argumen ( label, displayValue, unit, explanation, translate(guidanceKey),
        // translate(recommendationKey), string.IsNullOrWhiteSpace(formulaKey) ? string.Empty : translate... kepada pemanggil dalam DescribeMetric; eksekusi
        // jalur ini selesai setelah nilai hasil ditentukan.
        return new PlayerMetricPresentation(
            // Meneruskan `label` (nilai label) sebagai argumen ke konstruktor `PlayerMetricPresentation`.
            label,
            // Meneruskan `displayValue` (nilai display nilai) sebagai argumen ke konstruktor `PlayerMetricPresentation`.
            displayValue,
            // Meneruskan `unit` (nilai unit) sebagai argumen ke konstruktor `PlayerMetricPresentation`.
            unit,
            // Meneruskan `explanation` (nilai explanation) sebagai argumen ke konstruktor `PlayerMetricPresentation`.
            explanation,
            // Meneruskan memanggil `translate` dengan `guidanceKey` sebagai argumen ke konstruktor `PlayerMetricPresentation`; Meneruskan `guidanceKey` (nilai
            // guidance kunci) sebagai argumen ke `translate`.
            translate(guidanceKey),
            // Meneruskan memanggil `translate` dengan `recommendationKey` sebagai argumen ke konstruktor `PlayerMetricPresentation`; Meneruskan
            // `recommendationKey` (nilai recommendation kunci) sebagai argumen ke `translate`.
            translate(recommendationKey),
            // Meneruskan hasil pemilihan bersyarat: ketika `string.IsNullOrWhiteSpace(formulaKey)` benar gunakan `string.Empty`, jika tidak gunakan
            // `translate(formulaKey)` sebagai argumen ke konstruktor `PlayerMetricPresentation`; Meneruskan `formulaKey` (nilai formula kunci) sebagai argumen
            // ke `string.IsNullOrWhiteSpace`; Meneruskan `formulaKey` (nilai formula kunci) sebagai argumen ke `translate`.
            string.IsNullOrWhiteSpace(formulaKey) ? string.Empty : translate(formulaKey),
            // Meneruskan `isAdvancedOnly` (nilai berstatus advanced only) sebagai argumen ke konstruktor `PlayerMetricPresentation`.
            isAdvancedOnly,
            // Meneruskan `state` (keadaan permainan yang menjadi sumber atau hasil pembaruan) sebagai argumen ke konstruktor `PlayerMetricPresentation`.
            state);
    // Menutup scope metode DescribeMetric; bagian berikut berada di luar batas blok tersebut dalam DescribeMetric.
    }

    /// <summary>
    /// Mengubah kategori transaksi menjadi label lokal.
    /// </summary>
    // Mendefinisikan metode `HumanizeTransactionCategory` dengan hasil bertipe `string`. Mengubah kategori transaksi menjadi label lokal. Masukan:
    // Parameter `category` bertipe `string` membawa nilai category; Parameter `translate` bertipe `Func<string, string>` membawa nilai translate.
    public static string HumanizeTransactionCategory(string category, Func<string, string> translate)
    // Membuka scope metode HumanizeTransactionCategory; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam HumanizeTransactionCategory.
    {
        // Memeriksa memeriksa apakah `category` null, kosong, atau hanya berisi karakter spasi; blok if hanya dijalankan ketika kondisi ini bernilai benar
        // dalam HumanizeTransactionCategory.
        if (string.IsNullOrWhiteSpace(category))
        // Membuka scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(category)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // HumanizeTransactionCategory.
        {
            // Mengembalikan memanggil `translate` dengan `”players.details.transaction_label”` kepada pemanggil dalam HumanizeTransactionCategory; eksekusi
            // jalur ini selesai setelah nilai hasil ditentukan.
            return translate("players.details.transaction_label");
        // Menutup scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(category)`; bagian berikut berada di luar batas blok tersebut dalam
        // HumanizeTransactionCategory.
        }

        // Menyiapkan variabel lokal `normalized` untuk nilai normalized dengan menormalisasi `category.Trim()` menjadi huruf besar dengan aturan kultur
        // invariant. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var normalized = category.Trim().ToUpperInvariant();
        // Menyiapkan variabel lokal `mapped` untuk nilai mapped dengan hasil pemetaan `normalized` melalui cabang pola switch yang cocok. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var mapped = normalized switch
        // Membuka scope pemetaan switch atas `normalized`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam HumanizeTransactionCategory.
        {
            // Untuk pola `”DONATION”`, menghasilkan memanggil `translate` dengan `”players.details.transaction.category.donation”` sebagai hasil switch.
            "DONATION" => translate("players.details.transaction.category.donation"),
            // Untuk pola `”GOLD_TRADE”`, menghasilkan memanggil `translate` dengan `”players.details.transaction.category.gold_trade”` sebagai hasil switch.
            "GOLD_TRADE" => translate("players.details.transaction.category.gold_trade"),
            // Untuk pola `”INGREDIENT”`, menghasilkan memanggil `translate` dengan `”players.details.transaction.category.ingredient”` sebagai hasil switch.
            "INGREDIENT" => translate("players.details.transaction.category.ingredient"),
            "INITIAL_INGREDIENT" => translate("players.details.transaction.category.initial_ingredient"),
            // Untuk pola `”ORDER”`, menghasilkan memanggil `translate` dengan `”players.details.transaction.category.order”` sebagai hasil switch.
            "ORDER" => translate("players.details.transaction.category.order"),
            // Untuk pola `”FREELANCE”`, menghasilkan memanggil `translate` dengan `”players.details.transaction.category.freelance”` sebagai hasil switch.
            "FREELANCE" => translate("players.details.transaction.category.freelance"),
            // Untuk pola `”NEED_PRIMARY”`, menghasilkan memanggil `translate` dengan `”players.details.transaction.category.need_primary”` sebagai hasil
            // switch.
            "NEED_PRIMARY" => translate("players.details.transaction.category.need_primary"),
            // Untuk pola `”NEED_SECONDARY”`, menghasilkan memanggil `translate` dengan `”players.details.transaction.category.need_secondary”` sebagai hasil
            // switch.
            "NEED_SECONDARY" => translate("players.details.transaction.category.need_secondary"),
            // Untuk pola `”NEED_TERTIARY”`, menghasilkan memanggil `translate` dengan `”players.details.transaction.category.need_tertiary”` sebagai hasil
            // switch.
            "NEED_TERTIARY" => translate("players.details.transaction.category.need_tertiary"),
            // Untuk pola `”SAVING_DEPOSIT”`, menghasilkan memanggil `translate` dengan `”players.details.transaction.category.saving_deposit”` sebagai hasil
            // switch.
            "SAVING_DEPOSIT" => translate("players.details.transaction.category.saving_deposit"),
            // Untuk pola `”SAVING_WITHDRAW”`, menghasilkan memanggil `translate` dengan `”players.details.transaction.category.saving_withdraw”` sebagai hasil
            // switch.
            "SAVING_WITHDRAW" => translate("players.details.transaction.category.saving_withdraw"),
            // Untuk pola `”RISK_LIFE”`, menghasilkan memanggil `translate` dengan `”players.details.transaction.category.risk_life”` sebagai hasil switch.
            "RISK_LIFE" => translate("players.details.transaction.category.risk_life"),
            // Untuk pola `”LOAN_TAKEN”`, menghasilkan memanggil `translate` dengan `”players.details.transaction.category.loan_taken”` sebagai hasil switch.
            "LOAN_TAKEN" => translate("players.details.transaction.category.loan_taken"),
            // Untuk pola `”LOAN_REPAID”`, menghasilkan memanggil `translate` dengan `”players.details.transaction.category.loan_repaid”` sebagai hasil switch.
            "LOAN_REPAID" => translate("players.details.transaction.category.loan_repaid"),
            // Untuk pola `”INSURANCE_PREMIUM”`, menghasilkan memanggil `translate` dengan `”players.details.transaction.category.insurance_premium”` sebagai
            // hasil switch.
            "INSURANCE_PREMIUM" => translate("players.details.transaction.category.insurance_premium"),
            // Untuk pola `”INSURANCE_OFFSET”`, menghasilkan memanggil `translate` dengan `”players.details.transaction.category.insurance_claim”` sebagai hasil
            // switch.
            "INSURANCE_OFFSET" => translate("players.details.transaction.category.insurance_claim"),
            // Untuk pola `”EMERGENCY_OPTION”`, menghasilkan memanggil `translate` dengan `”players.details.transaction.category.emergency_option”` sebagai
            // hasil switch.
            "EMERGENCY_OPTION" => translate("players.details.transaction.category.emergency_option"),
            // Untuk pola `_`, menghasilkan `string.Empty`, yaitu nilai kosong bawaan tipe terkait sebagai hasil switch.
            _ => string.Empty
        // Menutup scope pemetaan switch atas `normalized`; bagian berikut berada di luar batas blok tersebut dalam HumanizeTransactionCategory.
        };

        // Memeriksa kebalikan kondisi `string.IsNullOrWhiteSpace(mapped)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // HumanizeTransactionCategory.
        if (!string.IsNullOrWhiteSpace(mapped))
        // Membuka scope cabang if untuk kondisi `!string.IsNullOrWhiteSpace(mapped)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // HumanizeTransactionCategory.
        {
            // Mengembalikan `mapped` (nilai mapped) kepada pemanggil dalam HumanizeTransactionCategory; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return mapped;
        // Menutup scope cabang if untuk kondisi `!string.IsNullOrWhiteSpace(mapped)`; bagian berikut berada di luar batas blok tersebut dalam
        // HumanizeTransactionCategory.
        }

        // Mengembalikan memanggil `HumanizeMetricKey` dengan `category.Trim().ToLowerInvariant()`, `translate` kepada pemanggil dalam
        // HumanizeTransactionCategory; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return HumanizeMetricKey(category.Trim().ToLowerInvariant(), translate);
    // Menutup scope metode HumanizeTransactionCategory; bagian berikut berada di luar batas blok tersebut dalam HumanizeTransactionCategory.
    }

    /// <summary>
    /// Mencoba membaca teks metrik menjadi angka untuk chart.
    /// </summary>
    // Mendefinisikan metode `TryParseMetricNumber` dengan hasil bertipe `bool`. Mencoba membaca teks metrik menjadi angka untuk chart. Masukan:
    // Parameter `rawValue` bertipe `string` membawa nilai raw nilai; Parameter `numericValue` bertipe `double` membawa nilai numerik nilai; out
    // mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    public static bool TryParseMetricNumber(string rawValue, out double numericValue)
    // Membuka scope metode TryParseMetricNumber; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryParseMetricNumber.
    {
        // Memeriksa memeriksa apakah `rawValue` null, kosong, atau hanya berisi karakter spasi; blok if hanya dijalankan ketika kondisi ini bernilai benar
        // dalam TryParseMetricNumber.
        if (string.IsNullOrWhiteSpace(rawValue))
        // Membuka scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(rawValue)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // TryParseMetricNumber.
        {
            // Memperbarui `numericValue` menggunakan nilai literal `0` dalam TryParseMetricNumber.
            numericValue = 0;
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryParseMetricNumber; eksekusi jalur ini selesai setelah nilai
            // hasil ditentukan.
            return false;
        // Menutup scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(rawValue)`; bagian berikut berada di luar batas blok tersebut dalam
        // TryParseMetricNumber.
        }

        // Memeriksa membandingkan kesamaan `string` dengan `rawValue`, `”true”`, `StringComparison.OrdinalIgnoreCase`; aturan perbandingan mengikuti
        // overload dan comparer yang diberikan; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam TryParseMetricNumber.
        if (string.Equals(rawValue, "true", StringComparison.OrdinalIgnoreCase))
        // Membuka scope cabang if untuk kondisi `string.Equals(rawValue, ”true”, StringComparison.OrdinalIgnoreCase)`; pernyataan/deklarasi berikut berada
        // di dalam batas blok ini dalam TryParseMetricNumber.
        {
            // Memperbarui `numericValue` menggunakan nilai literal `1` dalam TryParseMetricNumber.
            numericValue = 1;
            // Mengembalikan true, yaitu kondisi aktif/terpenuhi kepada pemanggil dalam TryParseMetricNumber; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return true;
        // Menutup scope cabang if untuk kondisi `string.Equals(rawValue, ”true”, StringComparison.OrdinalIgnoreCase)`; bagian berikut berada di luar batas
        // blok tersebut dalam TryParseMetricNumber.
        }

        // Memeriksa membandingkan kesamaan `string` dengan `rawValue`, `”false”`, `StringComparison.OrdinalIgnoreCase`; aturan perbandingan mengikuti
        // overload dan comparer yang diberikan; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam TryParseMetricNumber.
        if (string.Equals(rawValue, "false", StringComparison.OrdinalIgnoreCase))
        // Membuka scope cabang if untuk kondisi `string.Equals(rawValue, ”false”, StringComparison.OrdinalIgnoreCase)`; pernyataan/deklarasi berikut berada
        // di dalam batas blok ini dalam TryParseMetricNumber.
        {
            // Memperbarui `numericValue` menggunakan nilai literal `0` dalam TryParseMetricNumber.
            numericValue = 0;
            // Mengembalikan true, yaitu kondisi aktif/terpenuhi kepada pemanggil dalam TryParseMetricNumber; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return true;
        // Menutup scope cabang if untuk kondisi `string.Equals(rawValue, ”false”, StringComparison.OrdinalIgnoreCase)`; bagian berikut berada di luar batas
        // blok tersebut dalam TryParseMetricNumber.
        }

        // Memeriksa mencoba mengonversi `rawValue`, `NumberStyles.Float`, `CultureInfo.InvariantCulture`, `numericValue` melalui `double.TryParse`;
        // keberhasilan dilaporkan sebagai boolean dan hasil ditempatkan pada argumen out; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // TryParseMetricNumber.
        if (double.TryParse(rawValue, NumberStyles.Float, CultureInfo.InvariantCulture, out numericValue))
        // Membuka scope cabang if untuk kondisi `double.TryParse(rawValue, NumberStyles.Float, CultureInfo.InvariantCulture, out numericValue)`;
        // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryParseMetricNumber.
        {
            // Mengembalikan true, yaitu kondisi aktif/terpenuhi kepada pemanggil dalam TryParseMetricNumber; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return true;
        // Menutup scope cabang if untuk kondisi `double.TryParse(rawValue, NumberStyles.Float, CultureInfo.InvariantCulture, out numericValue)`; bagian
        // berikut berada di luar batas blok tersebut dalam TryParseMetricNumber.
        }

        // Memeriksa mencoba mengonversi `rawValue`, `NumberStyles.Float`, `CultureInfo.CurrentCulture`, `numericValue` melalui `double.TryParse`;
        // keberhasilan dilaporkan sebagai boolean dan hasil ditempatkan pada argumen out; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // TryParseMetricNumber.
        if (double.TryParse(rawValue, NumberStyles.Float, CultureInfo.CurrentCulture, out numericValue))
        // Membuka scope cabang if untuk kondisi `double.TryParse(rawValue, NumberStyles.Float, CultureInfo.CurrentCulture, out numericValue)`;
        // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryParseMetricNumber.
        {
            // Mengembalikan true, yaitu kondisi aktif/terpenuhi kepada pemanggil dalam TryParseMetricNumber; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return true;
        // Menutup scope cabang if untuk kondisi `double.TryParse(rawValue, NumberStyles.Float, CultureInfo.CurrentCulture, out numericValue)`; bagian
        // berikut berada di luar batas blok tersebut dalam TryParseMetricNumber.
        }

        // Memperbarui `numericValue` menggunakan nilai literal `0` dalam TryParseMetricNumber.
        numericValue = 0;
        // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryParseMetricNumber; eksekusi jalur ini selesai setelah nilai
        // hasil ditentukan.
        return false;
    // Menutup scope metode TryParseMetricNumber; bagian berikut berada di luar batas blok tersebut dalam TryParseMetricNumber.
    }

    /// <summary>
    /// Mengklasifikasikan path scalar compact untuk ringkasan gabungan utama.
    /// </summary>
    // Mendefinisikan metode `IsPreferredCombinedSummaryPath` dengan hasil bertipe `bool`. Mengklasifikasikan path scalar compact untuk ringkasan
    // gabungan utama. Masukan: Parameter `path` bertipe `string` membawa nilai path.
    public static bool IsPreferredCombinedSummaryPath(string path)
    // Membuka scope metode IsPreferredCombinedSummaryPath; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // IsPreferredCombinedSummaryPath.
    {
        // Memeriksa memeriksa apakah `path` null, kosong, atau hanya berisi karakter spasi; blok if hanya dijalankan ketika kondisi ini bernilai benar
        // dalam IsPreferredCombinedSummaryPath.
        if (string.IsNullOrWhiteSpace(path))
        // Membuka scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(path)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // IsPreferredCombinedSummaryPath.
        {
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam IsPreferredCombinedSummaryPath; eksekusi jalur ini selesai
            // setelah nilai hasil ditentukan.
            return false;
        // Menutup scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(path)`; bagian berikut berada di luar batas blok tersebut dalam
        // IsPreferredCombinedSummaryPath.
        }

        // Mengembalikan gabungan syarat AND: kedua kondisi wajib benar antara `!path.Contains('[')` dan `!path.Contains('.')`; sisi kanan diperiksa hanya
        // jika sisi kiri benar kepada pemanggil dalam IsPreferredCombinedSummaryPath; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return !path.Contains('[') && !path.Contains('.');
    // Menutup scope metode IsPreferredCombinedSummaryPath; bagian berikut berada di luar batas blok tersebut dalam IsPreferredCombinedSummaryPath.
    }

    /// <summary>
    /// Mengklasifikasikan path fallback ringkasan gabungan yang masih menolak array.
    /// </summary>
    // Mendefinisikan metode `IsFallbackCombinedSummaryPath` dengan hasil bertipe `bool`. Mengklasifikasikan path fallback ringkasan gabungan yang masih
    // menolak array. Masukan: Parameter `path` bertipe `string` membawa nilai path.
    public static bool IsFallbackCombinedSummaryPath(string path)
    // Membuka scope metode IsFallbackCombinedSummaryPath; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // IsFallbackCombinedSummaryPath.
    {
        // Memeriksa memeriksa apakah `path` null, kosong, atau hanya berisi karakter spasi; blok if hanya dijalankan ketika kondisi ini bernilai benar
        // dalam IsFallbackCombinedSummaryPath.
        if (string.IsNullOrWhiteSpace(path))
        // Membuka scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(path)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // IsFallbackCombinedSummaryPath.
        {
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam IsFallbackCombinedSummaryPath; eksekusi jalur ini selesai
            // setelah nilai hasil ditentukan.
            return false;
        // Menutup scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(path)`; bagian berikut berada di luar batas blok tersebut dalam
        // IsFallbackCombinedSummaryPath.
        }

        // Mengembalikan kebalikan kondisi `path.Contains('[')` kepada pemanggil dalam IsFallbackCombinedSummaryPath; eksekusi jalur ini selesai setelah
        // nilai hasil ditentukan.
        return !path.Contains('[');
    // Menutup scope metode IsFallbackCombinedSummaryPath; bagian berikut berada di luar batas blok tersebut dalam IsFallbackCombinedSummaryPath.
    }

    // Mendefinisikan metode `TryTranslateMetricLexicon` dengan hasil bertipe `string?`; operasi ini menangani try translate metric lexicon. Masukan:
    // Parameter `lexiconKey` bertipe `string` membawa nilai lexicon kunci; Parameter `translate` bertipe `Func<string, string>` membawa nilai
    // translate.
    private static string? TryTranslateMetricLexicon(string lexiconKey, Func<string, string> translate)
    // Membuka scope metode TryTranslateMetricLexicon; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryTranslateMetricLexicon.
    {
        // Memeriksa memeriksa apakah `lexiconKey` null, kosong, atau hanya berisi karakter spasi; blok if hanya dijalankan ketika kondisi ini bernilai
        // benar dalam TryTranslateMetricLexicon.
        if (string.IsNullOrWhiteSpace(lexiconKey))
        // Membuka scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(lexiconKey)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // TryTranslateMetricLexicon.
        {
            // Mengembalikan null, yaitu penanda tidak ada nilai kepada pemanggil dalam TryTranslateMetricLexicon; eksekusi jalur ini selesai setelah nilai
            // hasil ditentukan.
            return null;
        // Menutup scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(lexiconKey)`; bagian berikut berada di luar batas blok tersebut dalam
        // TryTranslateMetricLexicon.
        }

        // Menyiapkan variabel lokal `translated` untuk nilai translated dengan memanggil `translate` dengan `lexiconKey`. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var translated = translate(lexiconKey);
        // Memeriksa membandingkan kesamaan `string` dengan `translated`, `lexiconKey`, `StringComparison.OrdinalIgnoreCase`; aturan perbandingan mengikuti
        // overload dan comparer yang diberikan; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam TryTranslateMetricLexicon.
        if (string.Equals(translated, lexiconKey, StringComparison.OrdinalIgnoreCase))
        // Membuka scope cabang if untuk kondisi `string.Equals(translated, lexiconKey, StringComparison.OrdinalIgnoreCase)`; pernyataan/deklarasi berikut
        // berada di dalam batas blok ini dalam TryTranslateMetricLexicon.
        {
            // Mengembalikan null, yaitu penanda tidak ada nilai kepada pemanggil dalam TryTranslateMetricLexicon; eksekusi jalur ini selesai setelah nilai
            // hasil ditentukan.
            return null;
        // Menutup scope cabang if untuk kondisi `string.Equals(translated, lexiconKey, StringComparison.OrdinalIgnoreCase)`; bagian berikut berada di luar
        // batas blok tersebut dalam TryTranslateMetricLexicon.
        }

        // Mengembalikan `translated` (nilai translated) kepada pemanggil dalam TryTranslateMetricLexicon; eksekusi jalur ini selesai setelah nilai hasil
        // ditentukan.
        return translated;
    // Menutup scope metode TryTranslateMetricLexicon; bagian berikut berada di luar batas blok tersebut dalam TryTranslateMetricLexicon.
    }

    // Mendefinisikan metode `GetRootMetricKey` dengan hasil bertipe `string`; operasi ini menangani get root metric kunci. Masukan: Parameter `rawPath`
    // bertipe `string` membawa nilai raw path.
    private static string GetRootMetricKey(string rawPath)
    // Membuka scope metode GetRootMetricKey; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam GetRootMetricKey.
    {
        // Memeriksa memeriksa apakah `rawPath` null, kosong, atau hanya berisi karakter spasi; blok if hanya dijalankan ketika kondisi ini bernilai benar
        // dalam GetRootMetricKey.
        if (string.IsNullOrWhiteSpace(rawPath))
        // Membuka scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(rawPath)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // GetRootMetricKey.
        {
            // Mengembalikan `string.Empty`, yaitu nilai kosong bawaan tipe terkait kepada pemanggil dalam GetRootMetricKey; eksekusi jalur ini selesai setelah
            // nilai hasil ditentukan.
            return string.Empty;
        // Menutup scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(rawPath)`; bagian berikut berada di luar batas blok tersebut dalam
        // GetRootMetricKey.
        }

        // Menyiapkan variabel lokal `separatorIndex` untuk nilai separator index dengan memanggil `rawPath.IndexOfAny` dengan `['.', '[']`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var separatorIndex = rawPath.IndexOfAny(['.', '[']);
        // Menyiapkan variabel lokal `root` untuk nilai root dengan hasil pemilihan bersyarat: ketika `separatorIndex < 0` benar gunakan `rawPath`, jika
        // tidak gunakan `rawPath[..separatorIndex]`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var root = separatorIndex < 0 ? rawPath : rawPath[..separatorIndex];
        // Mengembalikan memanggil `NormalizeMetricLexiconKey` dengan `root` kepada pemanggil dalam GetRootMetricKey; eksekusi jalur ini selesai setelah
        // nilai hasil ditentukan.
        return NormalizeMetricLexiconKey(root);
    // Menutup scope metode GetRootMetricKey; bagian berikut berada di luar batas blok tersebut dalam GetRootMetricKey.
    }

    // Mendefinisikan metode `GetLeafMetricKey` dengan hasil bertipe `string`; operasi ini menangani get leaf metric kunci. Masukan: Parameter `rawPath`
    // bertipe `string` membawa nilai raw path.
    private static string GetLeafMetricKey(string rawPath)
    // Membuka scope metode GetLeafMetricKey; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam GetLeafMetricKey.
    {
        // Memeriksa memeriksa apakah `rawPath` null, kosong, atau hanya berisi karakter spasi; blok if hanya dijalankan ketika kondisi ini bernilai benar
        // dalam GetLeafMetricKey.
        if (string.IsNullOrWhiteSpace(rawPath))
        // Membuka scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(rawPath)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // GetLeafMetricKey.
        {
            // Mengembalikan `string.Empty`, yaitu nilai kosong bawaan tipe terkait kepada pemanggil dalam GetLeafMetricKey; eksekusi jalur ini selesai setelah
            // nilai hasil ditentukan.
            return string.Empty;
        // Menutup scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(rawPath)`; bagian berikut berada di luar batas blok tersebut dalam
        // GetLeafMetricKey.
        }

        // Menyiapkan variabel lokal `leaf` untuk nilai leaf dengan `rawPath[(rawPath.LastIndexOf('.') + 1)..]`, yaitu elemen koleksi yang dipilih melalui
        // indeks atau kunci tersebut. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var leaf = rawPath[(rawPath.LastIndexOf('.') + 1)..];
        // Menyiapkan variabel lokal `arrayIndex` untuk nilai array index dengan memanggil `leaf.IndexOf` dengan `'['`. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var arrayIndex = leaf.IndexOf('[');
        // Mengembalikan memanggil `NormalizeMetricLexiconKey` dengan `arrayIndex < 0 ? leaf : leaf[..arrayIndex]` kepada pemanggil dalam GetLeafMetricKey;
        // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return NormalizeMetricLexiconKey(arrayIndex < 0 ? leaf : leaf[..arrayIndex]);
    // Menutup scope metode GetLeafMetricKey; bagian berikut berada di luar batas blok tersebut dalam GetLeafMetricKey.
    }

    // Mendefinisikan metode `ResolveUnitKey` dengan hasil bertipe `string`; operasi ini menangani resolve unit kunci. Masukan: Parameter `metricKey`
    // bertipe `string` membawa nilai metric kunci; Parameter `leafMetricKey` bertipe `string` membawa nilai leaf metric kunci; Parameter
    // `normalizedPath` bertipe `string` membawa nilai normalized path.
    private static string ResolveUnitKey(string metricKey, string leafMetricKey, string normalizedPath)
    // Membuka scope metode ResolveUnitKey; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveUnitKey.
    {
        // Memeriksa memeriksa apakah `PercentageMetricKeys` memuat `metricKey`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // ResolveUnitKey.
        if (PercentageMetricKeys.Contains(metricKey))
        // Membuka scope cabang if untuk kondisi `PercentageMetricKeys.Contains(metricKey)`; pernyataan/deklarasi berikut berada di dalam batas blok ini
        // dalam ResolveUnitKey.
        {
            // Mengembalikan nilai literal `”players.support.unit.percent”` kepada pemanggil dalam ResolveUnitKey; eksekusi jalur ini selesai setelah nilai
            // hasil ditentukan.
            return "players.support.unit.percent";
        // Menutup scope cabang if untuk kondisi `PercentageMetricKeys.Contains(metricKey)`; bagian berikut berada di luar batas blok tersebut dalam
        // ResolveUnitKey.
        }

        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `FractionPercentageMetricKeys.Contains(metricKey)` dan
        // `FractionPercentageMetricKeys.Contains(leafMetricKey)`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi
        // ini bernilai benar dalam ResolveUnitKey.
        if (FractionPercentageMetricKeys.Contains(metricKey) ||
            // Melanjutkan pengolahan dengan memeriksa apakah `FractionPercentageMetricKeys` memuat `leafMetricKey` dalam ResolveUnitKey.
            FractionPercentageMetricKeys.Contains(leafMetricKey))
        // Membuka scope cabang if untuk kondisi `FractionPercentageMetricKeys.Contains(metricKey) || FractionPercentageMetricKeys.Contains(leafMetricKey)`;
        // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveUnitKey.
        {
            // Mengembalikan nilai literal `”players.support.unit.percent”` kepada pemanggil dalam ResolveUnitKey; eksekusi jalur ini selesai setelah nilai
            // hasil ditentukan.
            return "players.support.unit.percent";
        // Menutup scope cabang if untuk kondisi `FractionPercentageMetricKeys.Contains(metricKey) || FractionPercentageMetricKeys.Contains(leafMetricKey)`;
        // bagian berikut berada di luar batas blok tersebut dalam ResolveUnitKey.
        }

        // Memeriksa memeriksa apakah `MultiplierMetricKeys` memuat `metricKey`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // ResolveUnitKey.
        if (MultiplierMetricKeys.Contains(metricKey))
        // Membuka scope cabang if untuk kondisi `MultiplierMetricKeys.Contains(metricKey)`; pernyataan/deklarasi berikut berada di dalam batas blok ini
        // dalam ResolveUnitKey.
        {
            // Mengembalikan nilai literal `”players.support.unit.multiplier”` kepada pemanggil dalam ResolveUnitKey; eksekusi jalur ini selesai setelah nilai
            // hasil ditentukan.
            return "players.support.unit.multiplier";
        // Menutup scope cabang if untuk kondisi `MultiplierMetricKeys.Contains(metricKey)`; bagian berikut berada di luar batas blok tersebut dalam
        // ResolveUnitKey.
        }

        // Memeriksa kebalikan kondisi `string.Equals(metricKey, leafMetricKey, StringComparison.OrdinalIgnoreCase)`; blok if hanya dijalankan ketika
        // kondisi ini bernilai benar dalam ResolveUnitKey.
        if (!string.Equals(metricKey, leafMetricKey, StringComparison.OrdinalIgnoreCase))
        // Membuka scope cabang if untuk kondisi `!string.Equals(metricKey, leafMetricKey, StringComparison.OrdinalIgnoreCase)`; pernyataan/deklarasi
        // berikut berada di dalam batas blok ini dalam ResolveUnitKey.
        {
            // Memeriksa perbandingan kesamaan antara `leafMetricKey` dan `”day_index”`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
            // ResolveUnitKey.
            if (leafMetricKey == "day_index") return "players.support.unit.day";
            // Memeriksa perbandingan kesamaan antara `leafMetricKey` dan `”action_slot”`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
            // ResolveUnitKey.
            if (leafMetricKey == "action_slot") return "players.support.unit.action_slot";
            // Memeriksa perbandingan kesamaan antara `leafMetricKey` dan `”rank”`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
            // ResolveUnitKey.
            if (leafMetricKey == "rank") return "players.support.unit.rank";
            // Memeriksa hasil pencocokan `leafMetricKey` dengan pola `”amount” or ”net” or ”coins”`; blok if hanya dijalankan ketika kondisi ini bernilai benar
            // dalam ResolveUnitKey.
            if (leafMetricKey is "amount" or "net" or "coins") return "players.support.unit.coins";
            // Memeriksa hasil pencocokan `leafMetricKey` dengan pola `”total_actions” or ”distinct_actions” or ”repeated_actions”`; blok if hanya dijalankan
            // ketika kondisi ini bernilai benar dalam ResolveUnitKey.
            if (leafMetricKey == "total_actions") return "players.support.unit.action_tokens";
            if (leafMetricKey is "distinct_actions" or "repeated_actions") return "players.support.unit.actions";
            // Memeriksa hasil pencocokan `leafMetricKey` dengan pola `”action_type” or ”action_index” or ”actions” or ”basic_profile” or ”collector_profile” or
            // ”specialist_profile”`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ResolveUnitKey.
            if (leafMetricKey is "action_type" or "action_index" or "actions" or
                // Menggunakan nilai literal `”basic_profile”` sebagai bagian ekspresi yang sedang disusun dalam ResolveUnitKey.
                "basic_profile" or "collector_profile" or "specialist_profile") return string.Empty;
        // Menutup scope cabang if untuk kondisi `!string.Equals(metricKey, leafMetricKey, StringComparison.OrdinalIgnoreCase)`; bagian berikut berada di
        // luar batas blok tersebut dalam ResolveUnitKey.
        }

        // Memeriksa memeriksa apakah `metricKey` memuat `”rank”`, `StringComparison.OrdinalIgnoreCase`; blok if hanya dijalankan ketika kondisi ini
        // bernilai benar dalam ResolveUnitKey.
        if (metricKey.Contains("rank", StringComparison.OrdinalIgnoreCase))
        // Membuka scope cabang if untuk kondisi `metricKey.Contains(”rank”, StringComparison.OrdinalIgnoreCase)`; pernyataan/deklarasi berikut berada di
        // dalam batas blok ini dalam ResolveUnitKey.
        {
            // Mengembalikan nilai literal `”players.support.unit.rank”` kepada pemanggil dalam ResolveUnitKey; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return "players.support.unit.rank";
        // Menutup scope cabang if untuk kondisi `metricKey.Contains(”rank”, StringComparison.OrdinalIgnoreCase)`; bagian berikut berada di luar batas blok
        // tersebut dalam ResolveUnitKey.
        }

        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `metricKey.Contains(”point”, StringComparison.OrdinalIgnoreCase) ||
        // metricKey.EndsWith(”_pts”, StringComparison.OrdinalIgnoreCase)` dan `metricKey.Contains(”penalty”, StringComparison.OrdinalIgnoreCase)`; sisi
        // kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ResolveUnitKey.
        if (metricKey.Contains("point", StringComparison.OrdinalIgnoreCase) ||
            // Melanjutkan pengolahan dengan memanggil `metricKey.EndsWith` dengan `”_pts”`, `StringComparison.OrdinalIgnoreCase` dalam ResolveUnitKey.
            metricKey.EndsWith("_pts", StringComparison.OrdinalIgnoreCase) ||
            // Melanjutkan pengolahan dengan memeriksa apakah `metricKey` memuat `”penalty”`, `StringComparison.OrdinalIgnoreCase` dalam ResolveUnitKey.
            metricKey.Contains("penalty", StringComparison.OrdinalIgnoreCase))
        // Membuka scope cabang if untuk kondisi `metricKey.Contains(”point”, StringComparison.OrdinalIgnoreCase) || metricKey.EndsWith(”_pts”,
        // StringComparison.OrdinalIgnoreCase) || metricKey.Contains(”penalty”, StringCompar...`; pernyataan/deklarasi berikut berada di dalam batas blok
        // ini dalam ResolveUnitKey.
        {
            // Mengembalikan nilai literal `”players.support.unit.points”` kepada pemanggil dalam ResolveUnitKey; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return "players.support.unit.points";
        // Menutup scope cabang if untuk kondisi `metricKey.Contains(”point”, StringComparison.OrdinalIgnoreCase) || metricKey.EndsWith(”_pts”,
        // StringComparison.OrdinalIgnoreCase) || metricKey.Contains(”penalty”, StringCompar...`; bagian berikut berada di luar batas blok tersebut dalam
        // ResolveUnitKey.
        }

        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `metricKey.Contains(”day_”, StringComparison.OrdinalIgnoreCase)` dan
        // `metricKey is ”day_index” or ”latest_day_index”`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini
        // bernilai benar dalam ResolveUnitKey.
        if (metricKey.Contains("day_", StringComparison.OrdinalIgnoreCase) ||
            // Menggunakan `metricKey` (nilai metric kunci) sebagai bagian ekspresi yang sedang disusun dalam ResolveUnitKey.
            metricKey is "day_index" or "latest_day_index")
        // Membuka scope cabang if untuk kondisi `metricKey.Contains(”day_”, StringComparison.OrdinalIgnoreCase) || metricKey is ”day_index” or
        // ”latest_day_index”`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveUnitKey.
        {
            // Mengembalikan nilai literal `”players.support.unit.day”` kepada pemanggil dalam ResolveUnitKey; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return "players.support.unit.day";
        // Menutup scope cabang if untuk kondisi `metricKey.Contains(”day_”, StringComparison.OrdinalIgnoreCase) || metricKey is ”day_index” or
        // ”latest_day_index”`; bagian berikut berada di luar batas blok tersebut dalam ResolveUnitKey.
        }

        // Memeriksa memanggil `metricKey.StartsWith` dengan `”turn_number_”`, `StringComparison.OrdinalIgnoreCase`; blok if hanya dijalankan ketika kondisi
        // ini bernilai benar dalam ResolveUnitKey.
        if (metricKey.StartsWith("turn_number_", StringComparison.OrdinalIgnoreCase))
        // Membuka scope cabang if untuk kondisi `metricKey.StartsWith(”turn_number_”, StringComparison.OrdinalIgnoreCase)`; pernyataan/deklarasi berikut
        // berada di dalam batas blok ini dalam ResolveUnitKey.
        {
            // Mengembalikan nilai literal `”players.support.unit.turn”` kepada pemanggil dalam ResolveUnitKey; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return "players.support.unit.turn";
        // Menutup scope cabang if untuk kondisi `metricKey.StartsWith(”turn_number_”, StringComparison.OrdinalIgnoreCase)`; bagian berikut berada di luar
        // batas blok tersebut dalam ResolveUnitKey.
        }

        // Memeriksa memeriksa apakah `metricKey` memuat `”action_slot”`, `StringComparison.OrdinalIgnoreCase`; blok if hanya dijalankan ketika kondisi ini
        // bernilai benar dalam ResolveUnitKey.
        if (metricKey.Contains("action_slot", StringComparison.OrdinalIgnoreCase))
        // Membuka scope cabang if untuk kondisi `metricKey.Contains(”action_slot”, StringComparison.OrdinalIgnoreCase)`; pernyataan/deklarasi berikut
        // berada di dalam batas blok ini dalam ResolveUnitKey.
        {
            // Mengembalikan nilai literal `”players.support.unit.action_slot”` kepada pemanggil dalam ResolveUnitKey; eksekusi jalur ini selesai setelah nilai
            // hasil ditentukan.
            return "players.support.unit.action_slot";
        // Menutup scope cabang if untuk kondisi `metricKey.Contains(”action_slot”, StringComparison.OrdinalIgnoreCase)`; bagian berikut berada di luar
        // batas blok tersebut dalam ResolveUnitKey.
        }

        // Memeriksa hasil pencocokan `metricKey` dengan pola `”meal_orders_per_turn_average”`; blok if hanya dijalankan ketika kondisi ini bernilai benar
        // dalam ResolveUnitKey.
        if (metricKey is "meal_orders_per_turn_average")
        // Membuka scope cabang if untuk kondisi `metricKey is ”meal_orders_per_turn_average”`; pernyataan/deklarasi berikut berada di dalam batas blok ini
        // dalam ResolveUnitKey.
        {
            // Mengembalikan nilai literal `”players.support.unit.orders_per_turn”` kepada pemanggil dalam ResolveUnitKey; eksekusi jalur ini selesai setelah
            // nilai hasil ditentukan.
            return "players.support.unit.orders_per_turn";
        // Menutup scope cabang if untuk kondisi `metricKey is ”meal_orders_per_turn_average”`; bagian berikut berada di luar batas blok tersebut dalam
        // ResolveUnitKey.
        }

        // Memeriksa hasil pencocokan `metricKey` dengan pola `”ingredients_used_per_meal_average”`; blok if hanya dijalankan ketika kondisi ini bernilai
        // benar dalam ResolveUnitKey.
        if (metricKey is "ingredients_used_per_meal_average")
        // Membuka scope cabang if untuk kondisi `metricKey is ”ingredients_used_per_meal_average”`; pernyataan/deklarasi berikut berada di dalam batas blok
        // ini dalam ResolveUnitKey.
        {
            // Mengembalikan nilai literal `”players.support.unit.ingredients_per_order”` kepada pemanggil dalam ResolveUnitKey; eksekusi jalur ini selesai
            // setelah nilai hasil ditentukan.
            return "players.support.unit.ingredients_per_order";
        // Menutup scope cabang if untuk kondisi `metricKey is ”ingredients_used_per_meal_average”`; bagian berikut berada di luar batas blok tersebut dalam
        // ResolveUnitKey.
        }

        // Memeriksa hasil pencocokan `metricKey` dengan pola `”donation_stability_std_deviation”`; blok if hanya dijalankan ketika kondisi ini bernilai
        // benar dalam ResolveUnitKey.
        if (metricKey is "donation_stability_std_deviation")
        // Membuka scope cabang if untuk kondisi `metricKey is ”donation_stability_std_deviation”`; pernyataan/deklarasi berikut berada di dalam batas blok
        // ini dalam ResolveUnitKey.
        {
            // Mengembalikan nilai literal `”players.support.unit.coins”` kepada pemanggil dalam ResolveUnitKey; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return "players.support.unit.coins";
        // Menutup scope cabang if untuk kondisi `metricKey is ”donation_stability_std_deviation”`; bagian berikut berada di luar batas blok tersebut dalam
        // ResolveUnitKey.
        }

        // Memeriksa hasil pencocokan `metricKey` dengan pola `”donation_events”`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // ResolveUnitKey.
        if (metricKey is "donation_events")
        // Membuka scope cabang if untuk kondisi `metricKey is ”donation_events”`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ResolveUnitKey.
        {
            // Mengembalikan nilai literal `”players.support.unit.day”` kepada pemanggil dalam ResolveUnitKey; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return "players.support.unit.day";
        // Menutup scope cabang if untuk kondisi `metricKey is ”donation_events”`; bagian berikut berada di luar batas blok tersebut dalam ResolveUnitKey.
        }

        // Memeriksa hasil pencocokan `metricKey` dengan pola `”n_active_income_sources” or ”active_income_source_count”`; blok if hanya dijalankan ketika
        // kondisi ini bernilai benar dalam ResolveUnitKey.
        if (metricKey is "n_active_income_sources" or "active_income_source_count")
        // Membuka scope cabang if untuk kondisi `metricKey is ”n_active_income_sources” or ”active_income_source_count”`; pernyataan/deklarasi berikut
        // berada di dalam batas blok ini dalam ResolveUnitKey.
        {
            // Mengembalikan nilai literal `”players.support.unit.sources”` kepada pemanggil dalam ResolveUnitKey; eksekusi jalur ini selesai setelah nilai
            // hasil ditentukan.
            return "players.support.unit.sources";
        // Menutup scope cabang if untuk kondisi `metricKey is ”n_active_income_sources” or ”active_income_source_count”`; bagian berikut berada di luar
        // batas blok tersebut dalam ResolveUnitKey.
        }

        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `metricKey is ”income_producing_actions” or ”income_main_actions” or
        // ”all_player_actions” or ”total_main_actions” or ”savings_actions” or ”saving_actions” or ”financial_goal_ac...` dan `leafMetricKey is
        // ”income_producing_actions” or ”income_main_actions” or ”all_player_actions” or ”total_main_actions” or ”savings_actions” or ”saving_actions” or
        // ”financial_goa...`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // ResolveUnitKey.
        if (metricKey is "saving_and_goal_actions" or "total_actions" or "actions_per_turn" or "income_producing_actions" or "income_main_actions" or
                // Menggunakan nilai literal `”all_player_actions”` sebagai bagian ekspresi yang sedang disusun dalam ResolveUnitKey.
                "all_player_actions" or "total_main_actions" or
                // Menggunakan nilai literal `”savings_actions”` sebagai bagian ekspresi yang sedang disusun dalam ResolveUnitKey.
                "savings_actions" or "saving_actions" or "financial_goal_actions" or
                // Menggunakan nilai literal `”insurance_premium_actions”` sebagai bagian ekspresi yang sedang disusun dalam ResolveUnitKey.
                "insurance_premium_actions" or "insurance_actions" or "loan_repayment_actions" ||
            // Menggunakan `leafMetricKey` (nilai leaf metric kunci) sebagai bagian ekspresi yang sedang disusun dalam ResolveUnitKey.
            leafMetricKey is "saving_and_goal_actions" or "income_producing_actions" or "income_main_actions" or
                // Menggunakan nilai literal `”all_player_actions”` sebagai bagian ekspresi yang sedang disusun dalam ResolveUnitKey.
                "all_player_actions" or "total_main_actions" or
                // Menggunakan nilai literal `”savings_actions”` sebagai bagian ekspresi yang sedang disusun dalam ResolveUnitKey.
                "savings_actions" or "saving_actions" or "financial_goal_actions" or
                // Menggunakan nilai literal `”insurance_premium_actions”` sebagai bagian ekspresi yang sedang disusun dalam ResolveUnitKey.
                "insurance_premium_actions" or "insurance_actions" or "loan_repayment_actions")
        // Membuka scope cabang if untuk kondisi `metricKey is ”income_producing_actions” or ”income_main_actions” or ”all_player_actions” or
        // ”total_main_actions” or ”savings_actions” or ”saving_actions” or ”financial_goal_ac...`; pernyataan/deklarasi berikut berada di dalam batas blok
        // ini dalam ResolveUnitKey.
        {
            return "players.support.unit.action_tokens";
        // Menutup scope cabang if untuk kondisi `metricKey is ”income_producing_actions” or ”income_main_actions” or ”all_player_actions” or
        // ”total_main_actions” or ”savings_actions” or ”saving_actions” or ”financial_goal_ac...`; bagian berikut berada di luar batas blok tersebut dalam
        // ResolveUnitKey.
        }

        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `metricKey is ”outstanding_loan” or ”liquid_assets” or
        // ”attempted_goal_target_total”` dan `leafMetricKey is ”outstanding_loan” or ”liquid_assets” or ”attempted_goal_target_total”`; sisi kanan
        // diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ResolveUnitKey.
        if (metricKey is "outstanding_loan" or "liquid_assets" or "attempted_goal_target_total" ||
            // Menggunakan `leafMetricKey` (nilai leaf metric kunci) sebagai bagian ekspresi yang sedang disusun dalam ResolveUnitKey.
            leafMetricKey is "outstanding_loan" or "liquid_assets" or "attempted_goal_target_total")
        // Membuka scope cabang if untuk kondisi `metricKey is ”outstanding_loan” or ”liquid_assets” or ”attempted_goal_target_total” || leafMetricKey is
        // ”outstanding_loan” or ”liquid_assets” or ”attempted_goal_target_total”`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ResolveUnitKey.
        {
            // Mengembalikan nilai literal `”players.support.unit.coins”` kepada pemanggil dalam ResolveUnitKey; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return "players.support.unit.coins";
        // Menutup scope cabang if untuk kondisi `metricKey is ”outstanding_loan” or ”liquid_assets” or ”attempted_goal_target_total” || leafMetricKey is
        // ”outstanding_loan” or ”liquid_assets” or ”attempted_goal_target_total”`; bagian berikut berada di luar batas blok tersebut dalam ResolveUnitKey.
        }

        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `metricKey is ”risks_resolved_without_emergency” or
        // ”life_risk_cards_drawn”` dan `leafMetricKey is ”risks_resolved_without_emergency” or ”life_risk_cards_drawn”`; sisi kanan diperiksa hanya jika
        // sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ResolveUnitKey.
        if (metricKey is "risks_resolved_without_emergency" or "life_risk_cards_drawn" ||
            // Menggunakan `leafMetricKey` (nilai leaf metric kunci) sebagai bagian ekspresi yang sedang disusun dalam ResolveUnitKey.
            leafMetricKey is "risks_resolved_without_emergency" or "life_risk_cards_drawn")
        // Membuka scope cabang if untuk kondisi `metricKey is ”risks_resolved_without_emergency” or ”life_risk_cards_drawn” || leafMetricKey is
        // ”risks_resolved_without_emergency” or ”life_risk_cards_drawn”`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveUnitKey.
        {
            // Mengembalikan nilai literal `”players.support.unit.risk_events”` kepada pemanggil dalam ResolveUnitKey; eksekusi jalur ini selesai setelah nilai
            // hasil ditentukan.
            return "players.support.unit.risk_events";
        // Menutup scope cabang if untuk kondisi `metricKey is ”risks_resolved_without_emergency” or ”life_risk_cards_drawn” || leafMetricKey is
        // ”risks_resolved_without_emergency” or ”life_risk_cards_drawn”`; bagian berikut berada di luar batas blok tersebut dalam ResolveUnitKey.
        }

        // Memeriksa memeriksa apakah `normalizedPath` memuat `”financial_goals_balance_per_goal”`, `StringComparison.OrdinalIgnoreCase`; blok if hanya
        // dijalankan ketika kondisi ini bernilai benar dalam ResolveUnitKey.
        if (normalizedPath.Contains("financial_goals_balance_per_goal", StringComparison.OrdinalIgnoreCase))
        // Membuka scope cabang if untuk kondisi `normalizedPath.Contains(”financial_goals_balance_per_goal”, StringComparison.OrdinalIgnoreCase)`;
        // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveUnitKey.
        {
            // Mengembalikan nilai literal `”players.support.unit.coins”` kepada pemanggil dalam ResolveUnitKey; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return "players.support.unit.coins";
        // Menutup scope cabang if untuk kondisi `normalizedPath.Contains(”financial_goals_balance_per_goal”, StringComparison.OrdinalIgnoreCase)`; bagian
        // berikut berada di luar batas blok tersebut dalam ResolveUnitKey.
        }

        // Memeriksa hasil pencocokan `metricKey` dengan pola `”specific_tertiary_need” or ”collection_mission_complete”`; blok if hanya dijalankan ketika
        // kondisi ini bernilai benar dalam ResolveUnitKey.
        if (metricKey is "specific_tertiary_need" or "collection_mission_complete")
        // Membuka scope cabang if untuk kondisi `metricKey is ”specific_tertiary_need” or ”collection_mission_complete”`; pernyataan/deklarasi berikut
        // berada di dalam batas blok ini dalam ResolveUnitKey.
        {
            // Mengembalikan `string.Empty`, yaitu nilai kosong bawaan tipe terkait kepada pemanggil dalam ResolveUnitKey; eksekusi jalur ini selesai setelah
            // nilai hasil ditentukan.
            return string.Empty;
        // Menutup scope cabang if untuk kondisi `metricKey is ”specific_tertiary_need” or ”collection_mission_complete”`; bagian berikut berada di luar
        // batas blok tersebut dalam ResolveUnitKey.
        }

        // Menyiapkan variabel lokal `isMoney` untuk nilai berstatus money dengan gabungan syarat OR: setidaknya satu kondisi wajib benar antara
        // `metricKey.Contains(”coin”, StringComparison.OrdinalIgnoreCase) || leafMetricKey.Contains(”coin”, StringComparison.OrdinalIgnoreCase) ||
        // metricKey.Contains(”cash”, StringCompar...` dan `metricKey is ”amount” or ”pension_fund_total” or ”donation_total_coins” or
        // ”insurance_payments_made” or ”ingredient_cards_value_end”`; sisi kanan diperiksa hanya jika sisi kiri salah. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var isMoney = metricKey.Contains("coin", StringComparison.OrdinalIgnoreCase) ||
                      // Melanjutkan pengolahan dengan memeriksa apakah `leafMetricKey` memuat `”coin”`, `StringComparison.OrdinalIgnoreCase` dalam ResolveUnitKey.
                      leafMetricKey.Contains("coin", StringComparison.OrdinalIgnoreCase) ||
                      // Melanjutkan pengolahan dengan memeriksa apakah `metricKey` memuat `”cash”`, `StringComparison.OrdinalIgnoreCase` dalam ResolveUnitKey.
                      metricKey.Contains("cash", StringComparison.OrdinalIgnoreCase) ||
                      // Melanjutkan pengolahan dengan memeriksa apakah `metricKey` memuat `”income”`, `StringComparison.OrdinalIgnoreCase` dalam ResolveUnitKey.
                      metricKey.Contains("income", StringComparison.OrdinalIgnoreCase) ||
                      // Melanjutkan pengolahan dengan memeriksa apakah `metricKey` memuat `”expense”`, `StringComparison.OrdinalIgnoreCase` dalam ResolveUnitKey.
                      metricKey.Contains("expense", StringComparison.OrdinalIgnoreCase) ||
                      // Melanjutkan pengolahan dengan memeriksa apakah `metricKey` memuat `”cost”`, `StringComparison.OrdinalIgnoreCase` dalam ResolveUnitKey.
                      metricKey.Contains("cost", StringComparison.OrdinalIgnoreCase) ||
                      // Melanjutkan pengolahan dengan memeriksa apakah `metricKey` memuat `”price”`, `StringComparison.OrdinalIgnoreCase` dalam ResolveUnitKey.
                      metricKey.Contains("price", StringComparison.OrdinalIgnoreCase) ||
                      // Melanjutkan pengolahan dengan memeriksa apakah `metricKey` memuat `”investment”`, `StringComparison.OrdinalIgnoreCase` dalam ResolveUnitKey.
                      metricKey.Contains("investment", StringComparison.OrdinalIgnoreCase) ||
                      // Menggunakan `metricKey` (nilai metric kunci) sebagai bagian ekspresi yang sedang disusun dalam ResolveUnitKey.
                      metricKey is "amount" or "pension_fund_total" or "donation_total_coins" or
                          // Menggunakan nilai literal `”insurance_payments_made”` sebagai bagian ekspresi yang sedang disusun dalam ResolveUnitKey.
                          "insurance_payments_made" or "ingredient_cards_value_end";
        // Memeriksa `isMoney` (nilai berstatus money); blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ResolveUnitKey.
        if (isMoney)
        // Membuka scope cabang if untuk kondisi `isMoney`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveUnitKey.
        {
            // Mengembalikan nilai literal `”players.support.unit.coins”` kepada pemanggil dalam ResolveUnitKey; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return "players.support.unit.coins";
        // Menutup scope cabang if untuk kondisi `isMoney`; bagian berikut berada di luar batas blok tersebut dalam ResolveUnitKey.
        }

        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `metricKey.Contains(”score”, StringComparison.OrdinalIgnoreCase)` dan
        // `metricKey is ”donation_stability” or ”mission_achievement”`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika
        // kondisi ini bernilai benar dalam ResolveUnitKey.
        if (metricKey.Contains("score", StringComparison.OrdinalIgnoreCase) ||
            // Menggunakan `metricKey` (nilai metric kunci) sebagai bagian ekspresi yang sedang disusun dalam ResolveUnitKey.
            metricKey is "donation_stability" or "mission_achievement")
        // Membuka scope cabang if untuk kondisi `metricKey.Contains(”score”, StringComparison.OrdinalIgnoreCase) || metricKey is ”donation_stability” or
        // ”mission_achievement”`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveUnitKey.
        {
            // Mengembalikan nilai literal `”players.support.unit.score”` kepada pemanggil dalam ResolveUnitKey; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return "players.support.unit.score";
        // Menutup scope cabang if untuk kondisi `metricKey.Contains(”score”, StringComparison.OrdinalIgnoreCase) || metricKey is ”donation_stability” or
        // ”mission_achievement”`; bagian berikut berada di luar batas blok tersebut dalam ResolveUnitKey.
        }

        // Memeriksa memeriksa apakah `normalizedPath` memuat `”ingredient”`, `StringComparison.OrdinalIgnoreCase`; blok if hanya dijalankan ketika kondisi
        // ini bernilai benar dalam ResolveUnitKey.
        if (normalizedPath.Contains("ingredient", StringComparison.OrdinalIgnoreCase)) return "players.support.unit.ingredient_cards";
        // Memeriksa memeriksa apakah `normalizedPath` memuat `”meal_order”`, `StringComparison.OrdinalIgnoreCase`; blok if hanya dijalankan ketika kondisi
        // ini bernilai benar dalam ResolveUnitKey.
        if (normalizedPath.Contains("meal_order", StringComparison.OrdinalIgnoreCase)) return "players.support.unit.orders";
        // Memeriksa memeriksa apakah `normalizedPath` memuat `”need”`, `StringComparison.OrdinalIgnoreCase`; blok if hanya dijalankan ketika kondisi ini
        // bernilai benar dalam ResolveUnitKey.
        if (normalizedPath.Contains("need", StringComparison.OrdinalIgnoreCase)) return "players.support.unit.need_cards";
        // Memeriksa memeriksa apakah `normalizedPath` memuat `”gold_card”`, `StringComparison.OrdinalIgnoreCase`; blok if hanya dijalankan ketika kondisi
        // ini bernilai benar dalam ResolveUnitKey.
        if (normalizedPath.Contains("gold_card", StringComparison.OrdinalIgnoreCase)) return "players.support.unit.gold_cards";
        // Memeriksa memeriksa apakah `normalizedPath` memuat `”card”`, `StringComparison.OrdinalIgnoreCase`; blok if hanya dijalankan ketika kondisi ini
        // bernilai benar dalam ResolveUnitKey.
        if (normalizedPath.Contains("card", StringComparison.OrdinalIgnoreCase)) return "players.support.unit.cards";
        // Memeriksa memeriksa apakah `normalizedPath` memuat `”risk”`, `StringComparison.OrdinalIgnoreCase`; blok if hanya dijalankan ketika kondisi ini
        // bernilai benar dalam ResolveUnitKey.
        if (normalizedPath.Contains("risk", StringComparison.OrdinalIgnoreCase)) return "players.support.unit.risk_events";
        // Memeriksa memeriksa apakah `normalizedPath` memuat `”financial_goal”`, `StringComparison.OrdinalIgnoreCase`; blok if hanya dijalankan ketika
        // kondisi ini bernilai benar dalam ResolveUnitKey.
        if (normalizedPath.Contains("financial_goal", StringComparison.OrdinalIgnoreCase)) return "players.support.unit.goals";
        // Memeriksa memeriksa apakah `normalizedPath` memuat `”loan”`, `StringComparison.OrdinalIgnoreCase`; blok if hanya dijalankan ketika kondisi ini
        // bernilai benar dalam ResolveUnitKey.
        if (normalizedPath.Contains("loan", StringComparison.OrdinalIgnoreCase)) return "players.support.unit.loans";
        // Memeriksa memeriksa apakah `normalizedPath` memuat `”action”`, `StringComparison.OrdinalIgnoreCase`; blok if hanya dijalankan ketika kondisi ini
        // bernilai benar dalam ResolveUnitKey.
        if (normalizedPath.Contains("action", StringComparison.OrdinalIgnoreCase)) return "players.support.unit.actions";
        // Memeriksa memeriksa apakah `normalizedPath` memuat `”event”`, `StringComparison.OrdinalIgnoreCase`; blok if hanya dijalankan ketika kondisi ini
        // bernilai benar dalam ResolveUnitKey.
        if (normalizedPath.Contains("event", StringComparison.OrdinalIgnoreCase)) return "players.support.unit.events";
        // Memeriksa memeriksa apakah `normalizedPath` memuat `”transaction”`, `StringComparison.OrdinalIgnoreCase`; blok if hanya dijalankan ketika kondisi
        // ini bernilai benar dalam ResolveUnitKey.
        if (normalizedPath.Contains("transaction", StringComparison.OrdinalIgnoreCase)) return "players.support.unit.transactions";
        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `normalizedPath.Contains(”donation”,
        // StringComparison.OrdinalIgnoreCase)` dan `normalizedPath.Contains(”option”, StringComparison.OrdinalIgnoreCase)`; sisi kanan diperiksa hanya jika
        // sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ResolveUnitKey.
        if (normalizedPath.Contains("donation", StringComparison.OrdinalIgnoreCase) ||
            // Melanjutkan pengolahan dengan memeriksa apakah `normalizedPath` memuat `”option”`, `StringComparison.OrdinalIgnoreCase` dalam ResolveUnitKey.
            normalizedPath.Contains("option", StringComparison.OrdinalIgnoreCase)) return "players.support.unit.times";
        // Mengembalikan `string.Empty`, yaitu nilai kosong bawaan tipe terkait kepada pemanggil dalam ResolveUnitKey; eksekusi jalur ini selesai setelah
        // nilai hasil ditentukan.
        return string.Empty;
    // Menutup scope metode ResolveUnitKey; bagian berikut berada di luar batas blok tersebut dalam ResolveUnitKey.
    }

    // Mendefinisikan metode `IsGameDayMetric` dengan hasil bertipe `bool`; operasi ini menangani berstatus game hari metric. Masukan: Parameter
    // `metricKey` bertipe `string` membawa nilai metric kunci. Nilai hasil langsung berasal dari hasil pencocokan `metricKey` dengan pola
    // `”day_when_debt_introduced” or ”day_when_first_risk_hit” or ”day_game_completion” or ”latest_day_index”`.
    private static bool IsGameDayMetric(string metricKey) => metricKey is
        // Menggunakan nilai literal `”day_when_debt_introduced”` sebagai bagian ekspresi yang sedang disusun dalam IsGameDayMetric.
        "day_when_debt_introduced" or
        // Menggunakan nilai literal `”day_when_first_risk_hit”` sebagai bagian ekspresi yang sedang disusun dalam IsGameDayMetric.
        "day_when_first_risk_hit" or
        // Menggunakan nilai literal `”day_game_completion”` sebagai bagian ekspresi yang sedang disusun dalam IsGameDayMetric.
        "day_game_completion" or
        // Menggunakan nilai literal `”latest_day_index”` sebagai bagian ekspresi yang sedang disusun dalam IsGameDayMetric.
        "latest_day_index";

    // Mendefinisikan metode `ResolveExplanationKey` dengan hasil bertipe `string`; operasi ini menangani resolve explanation kunci. Masukan: Parameter
    // `metricKey` bertipe `string` membawa nilai metric kunci; Parameter `isDerived` bertipe `bool` membawa nilai berstatus derived; Parameter
    // `unitKey` bertipe `string` membawa nilai unit kunci.
    private static string ResolveExplanationKey(string metricKey, bool isDerived, string unitKey)
    // Membuka scope metode ResolveExplanationKey; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveExplanationKey.
    {
        // Menyiapkan variabel lokal `timelineExplanationKey` untuk nilai timeline explanation kunci dengan hasil pemetaan `metricKey` melalui cabang pola
        // switch yang cocok. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var timelineExplanationKey = metricKey switch
        // Membuka scope pemetaan switch atas `metricKey`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveExplanationKey.
        {
            // Untuk pola `”day_when_debt_introduced”`, menghasilkan nilai literal `”players.support.meaning.first_loan_day”` sebagai hasil switch.
            "day_when_debt_introduced" => "players.support.meaning.first_loan_day",
            // Untuk pola `”day_when_first_risk_hit”`, menghasilkan nilai literal `”players.support.meaning.first_risk_day”` sebagai hasil switch.
            "day_when_first_risk_hit" => "players.support.meaning.first_risk_day",
            // Untuk pola `”day_game_completion” or ”latest_day_index”`, menghasilkan nilai literal `”players.support.meaning.last_activity_day”` sebagai hasil
            // switch.
            "day_game_completion" or "latest_day_index" => "players.support.meaning.last_activity_day",
            // Untuk pola `_`, menghasilkan `string.Empty`, yaitu nilai kosong bawaan tipe terkait sebagai hasil switch.
            _ => string.Empty
        // Menutup scope pemetaan switch atas `metricKey`; bagian berikut berada di luar batas blok tersebut dalam ResolveExplanationKey.
        };
        // Memeriksa kebalikan kondisi `string.IsNullOrWhiteSpace(timelineExplanationKey)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // ResolveExplanationKey.
        if (!string.IsNullOrWhiteSpace(timelineExplanationKey))
        // Membuka scope cabang if untuk kondisi `!string.IsNullOrWhiteSpace(timelineExplanationKey)`; pernyataan/deklarasi berikut berada di dalam batas
        // blok ini dalam ResolveExplanationKey.
        {
            // Mengembalikan `timelineExplanationKey` (nilai timeline explanation kunci) kepada pemanggil dalam ResolveExplanationKey; eksekusi jalur ini
            // selesai setelah nilai hasil ditentukan.
            return timelineExplanationKey;
        // Menutup scope cabang if untuk kondisi `!string.IsNullOrWhiteSpace(timelineExplanationKey)`; bagian berikut berada di luar batas blok tersebut
        // dalam ResolveExplanationKey.
        }

        // Memeriksa perbandingan kesamaan antara `metricKey` dan `”net_income_per_turn”`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // ResolveExplanationKey.
        if (metricKey == "net_income_per_turn")
        // Membuka scope cabang if untuk kondisi `metricKey == ”net_income_per_turn”`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ResolveExplanationKey.
        {
            // Mengembalikan nilai literal `”players.support.meaning.coin_change_event”` kepada pemanggil dalam ResolveExplanationKey; eksekusi jalur ini
            // selesai setelah nilai hasil ditentukan.
            return "players.support.meaning.coin_change_event";
        // Menutup scope cabang if untuk kondisi `metricKey == ”net_income_per_turn”`; bagian berikut berada di luar batas blok tersebut dalam
        // ResolveExplanationKey.
        }

        // Memeriksa perbandingan kesamaan antara `metricKey` dan `”transaction_history”`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // ResolveExplanationKey.
        if (metricKey == "transaction_history")
        // Membuka scope cabang if untuk kondisi `metricKey == ”transaction_history”`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ResolveExplanationKey.
        {
            // Mengembalikan nilai literal `”players.support.meaning.transaction_history”` kepada pemanggil dalam ResolveExplanationKey; eksekusi jalur ini
            // selesai setelah nilai hasil ditentukan.
            return "players.support.meaning.transaction_history";
        // Menutup scope cabang if untuk kondisi `metricKey == ”transaction_history”`; bagian berikut berada di luar batas blok tersebut dalam
        // ResolveExplanationKey.
        }

        // Memeriksa perbandingan kesamaan antara `metricKey` dan `”action_usage_history”`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // ResolveExplanationKey.
        if (metricKey == "action_usage_history")
        // Membuka scope cabang if untuk kondisi `metricKey == ”action_usage_history”`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ResolveExplanationKey.
        {
            // Mengembalikan nilai literal `”players.support.meaning.action_usage_history”` kepada pemanggil dalam ResolveExplanationKey; eksekusi jalur ini
            // selesai setelah nilai hasil ditentukan.
            return "players.support.meaning.action_usage_history";
        // Menutup scope cabang if untuk kondisi `metricKey == ”action_usage_history”`; bagian berikut berada di luar batas blok tersebut dalam
        // ResolveExplanationKey.
        }

        // Memeriksa perbandingan kesamaan antara `metricKey` dan `”emergency_options_used”`; blok if hanya dijalankan ketika kondisi ini bernilai benar
        // dalam ResolveExplanationKey.
        if (metricKey == "emergency_options_used")
        // Membuka scope cabang if untuk kondisi `metricKey == ”emergency_options_used”`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ResolveExplanationKey.
        {
            // Mengembalikan nilai literal `”players.support.meaning.emergency_actions”` kepada pemanggil dalam ResolveExplanationKey; eksekusi jalur ini
            // selesai setelah nilai hasil ditentukan.
            return "players.support.meaning.emergency_actions";
        // Menutup scope cabang if untuk kondisi `metricKey == ”emergency_options_used”`; bagian berikut berada di luar batas blok tersebut dalam
        // ResolveExplanationKey.
        }

        // Menyiapkan variabel lokal `rawExplanationKey` untuk nilai raw explanation kunci dengan hasil pemetaan `metricKey` melalui cabang pola switch yang
        // cocok. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var rawExplanationKey = metricKey switch
        // Membuka scope pemetaan switch atas `metricKey`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveExplanationKey.
        {
            // Untuk pola `”ingredients_held_current”`, menghasilkan nilai literal `”players.support.meaning.ingredients_remaining”` sebagai hasil switch.
            "ingredients_held_current" => "players.support.meaning.ingredients_remaining",
            // Untuk pola `”ingredients_used_per_meal”`, menghasilkan nilai literal `”players.support.meaning.ingredients_per_order”` sebagai hasil switch.
            "ingredients_used_per_meal" => "players.support.meaning.ingredients_per_order",
            // Untuk pola `”meal_orders_per_turn_average”`, menghasilkan nilai literal `”players.support.meaning.orders_per_active_day”` sebagai hasil switch.
            "meal_orders_per_turn_average" => "players.support.meaning.orders_per_active_day",
            // Untuk pola `”meal_order_income_per_order”`, menghasilkan nilai literal `”players.support.meaning.income_per_order”` sebagai hasil switch.
            "meal_order_income_per_order" => "players.support.meaning.income_per_order",
            // Untuk pola `”ingredient_types_held”`, menghasilkan nilai literal `”players.support.meaning.ingredient_types_remaining”` sebagai hasil switch.
            "ingredient_types_held" => "players.support.meaning.ingredient_types_remaining",
            // Untuk pola `”ingredients_wasted”`, menghasilkan nilai literal `”players.support.meaning.ingredients_discarded”` sebagai hasil switch.
            "ingredients_wasted" => "players.support.meaning.ingredients_discarded",
            // Untuk pola `”gold_cards_purchased”`, menghasilkan nilai literal `”players.support.meaning.gold_purchased”` sebagai hasil switch.
            "gold_cards_purchased" => "players.support.meaning.gold_purchased",
            // Untuk pola `”gold_cards_held_end”`, menghasilkan nilai literal `”players.support.meaning.gold_remaining”` sebagai hasil switch.
            "gold_cards_held_end" => "players.support.meaning.gold_remaining",
            // Untuk pola `”gold_investment_net”`, menghasilkan nilai literal `”players.support.meaning.gold_cashflow”` sebagai hasil switch.
            "gold_investment_net" => "players.support.meaning.gold_cashflow",
            // Untuk pola `”ingredient_cards_value_end”`, menghasilkan nilai literal `”players.support.meaning.pension_ingredient_value”` sebagai hasil switch.
            "ingredient_cards_value_end" => "players.support.meaning.pension_ingredient_value",
            // Untuk pola `”life_risk_costs_per_card” or ”life_risk_costs_total”`, menghasilkan nilai literal `”players.support.meaning.risk_nominal_cost”`
            // sebagai hasil switch.
            "life_risk_costs_per_card" or "life_risk_costs_total" => "players.support.meaning.risk_nominal_cost",
            // Untuk pola `”donation_rank_per_friday”`, menghasilkan nilai literal `”players.support.meaning.donation_rank”` sebagai hasil switch.
            "donation_rank_per_friday" => "players.support.meaning.donation_rank",
            // Untuk pola `”donation_history”`, menghasilkan nilai literal `”players.support.meaning.donation_history”` sebagai hasil switch.
            "donation_history" => "players.support.meaning.donation_history",
            // Untuk pola `”donation_happiness_points” or ”donations_pts”`, menghasilkan nilai literal `”players.support.meaning.donation_happiness”` sebagai
            // hasil switch.
            "donation_happiness_points" or "donations_pts" => "players.support.meaning.donation_happiness",
            // Untuk pola `”financial_goals_completed”`, menghasilkan nilai literal `”players.support.meaning.completed_goals”` sebagai hasil switch.
            "financial_goals_completed" => "players.support.meaning.completed_goals",
            // Untuk pola `”sharia_loans_outstanding_coins”`, menghasilkan nilai literal `”players.support.meaning.outstanding_loan”` sebagai hasil switch.
            "sharia_loans_outstanding_coins" => "players.support.meaning.outstanding_loan",
            // Untuk pola `”need_cards_purchased”`, menghasilkan nilai literal `”players.support.meaning.need_cards_purchased”` sebagai hasil switch.
            "need_cards_purchased" => "players.support.meaning.need_cards_purchased",
            // Untuk pola `”need_cards_owned_current”`, menghasilkan nilai literal `”players.support.meaning.need_cards_owned”` sebagai hasil switch.
            "need_cards_owned_current" => "players.support.meaning.need_cards_owned",
            // Untuk pola `”primary_needs_owned” or ”secondary_needs_owned” or ”tertiary_needs_owned”`, menghasilkan nilai literal
            // `”players.support.meaning.need_level_owned”` sebagai hasil switch.
            "primary_needs_owned" or "secondary_needs_owned" or "tertiary_needs_owned" => "players.support.meaning.need_level_owned",
            // Untuk pola `”specific_tertiary_need”`, menghasilkan nilai literal `”players.support.meaning.mission_need_owned”` sebagai hasil switch.
            "specific_tertiary_need" => "players.support.meaning.mission_need_owned",
            // Untuk pola `”collection_mission_complete”`, menghasilkan nilai literal `”players.support.meaning.collection_mission”` sebagai hasil switch.
            "collection_mission_complete" => "players.support.meaning.collection_mission",
            // Untuk pola `”need_cards_coins_spent”`, menghasilkan nilai literal `”players.support.meaning.need_purchase_cost”` sebagai hasil switch.
            "need_cards_coins_spent" => "players.support.meaning.need_purchase_cost",
            // Untuk pola `”ingredients_collected”`, menghasilkan nilai literal `”players.support.meaning.ingredients_collected”` sebagai hasil switch.
            "ingredients_collected" => "players.support.meaning.ingredients_collected",
            // Untuk pola `”ingredients_used_total”`, menghasilkan nilai literal `”players.support.meaning.ingredients_used_total”` sebagai hasil switch.
            "ingredients_used_total" => "players.support.meaning.ingredients_used_total",
            // Untuk pola `”ingredients_used_per_meal_average”`, menghasilkan nilai literal `”players.support.meaning.ingredients_used_average”` sebagai hasil
            // switch.
            "ingredients_used_per_meal_average" => "players.support.meaning.ingredients_used_average",
            // Untuk pola `”ingredient_investment_coins_total”`, menghasilkan nilai literal `”players.support.meaning.ingredient_purchase_cost”` sebagai hasil
            // switch.
            "ingredient_investment_coins_total" => "players.support.meaning.ingredient_purchase_cost",
            // Untuk pola `”meal_orders_claimed”`, menghasilkan nilai literal `”players.support.meaning.orders_completed”` sebagai hasil switch.
            "meal_orders_claimed" => "players.support.meaning.orders_completed",
            // Untuk pola `”meal_order_income_total”`, menghasilkan nilai literal `”players.support.meaning.order_income_total”` sebagai hasil switch.
            "meal_order_income_total" => "players.support.meaning.order_income_total",
            // Untuk pola `”gold_cards_initial”`, menghasilkan nilai literal `”players.support.meaning.gold_initial”` sebagai hasil switch.
            "gold_cards_initial" => "players.support.meaning.gold_initial",
            // Untuk pola `”gold_cards_sold”`, menghasilkan nilai literal `”players.support.meaning.gold_sold”` sebagai hasil switch.
            "gold_cards_sold" => "players.support.meaning.gold_sold",
            // Untuk pola `”gold_prices_per_purchase”`, menghasilkan nilai literal `”players.support.meaning.gold_purchase_prices”` sebagai hasil switch.
            "gold_prices_per_purchase" => "players.support.meaning.gold_purchase_prices",
            // Untuk pola `”gold_price_per_sale”`, menghasilkan nilai literal `”players.support.meaning.gold_sale_prices”` sebagai hasil switch.
            "gold_price_per_sale" => "players.support.meaning.gold_sale_prices",
            // Untuk pola `”gold_investment_coins_spent”`, menghasilkan nilai literal `”players.support.meaning.gold_purchase_cost”` sebagai hasil switch.
            "gold_investment_coins_spent" => "players.support.meaning.gold_purchase_cost",
            // Untuk pola `”gold_investment_coins_earned”`, menghasilkan nilai literal `”players.support.meaning.gold_sale_income”` sebagai hasil switch.
            "gold_investment_coins_earned" => "players.support.meaning.gold_sale_income",
            // Untuk pola `”pension_fund_total”`, menghasilkan nilai literal `”players.support.meaning.pension_total”` sebagai hasil switch.
            "pension_fund_total" => "players.support.meaning.pension_total",
            // Untuk pola `”pension_fund_rank_per_game”`, menghasilkan nilai literal `”players.support.meaning.pension_rank”` sebagai hasil switch.
            "pension_fund_rank_per_game" => "players.support.meaning.pension_rank",
            // Untuk pola `”pension_fund_happiness_points”`, menghasilkan nilai literal `”players.support.meaning.pension_happiness”` sebagai hasil switch.
            "pension_fund_happiness_points" => "players.support.meaning.pension_happiness",
            // Untuk pola `”life_risk_cards_drawn”`, menghasilkan nilai literal `”players.support.meaning.risk_cards_drawn”` sebagai hasil switch.
            "life_risk_cards_drawn" => "players.support.meaning.risk_cards_drawn",
            // Untuk pola `”life_risk_mitigated_with_insurance”`, menghasilkan nilai literal `”players.support.meaning.risk_insured”` sebagai hasil switch.
            "life_risk_mitigated_with_insurance" => "players.support.meaning.risk_insured",
            // Untuk pola `”insurance_payments_made”`, menghasilkan nilai literal `”players.support.meaning.insurance_premium”` sebagai hasil switch.
            "insurance_payments_made" => "players.support.meaning.insurance_premium",
            // Untuk pola `_`, menghasilkan `string.Empty`, yaitu nilai kosong bawaan tipe terkait sebagai hasil switch.
            _ => string.Empty
        // Menutup scope pemetaan switch atas `metricKey`; bagian berikut berada di luar batas blok tersebut dalam ResolveExplanationKey.
        };
        // Memeriksa kebalikan kondisi `string.IsNullOrWhiteSpace(rawExplanationKey)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // ResolveExplanationKey.
        if (!string.IsNullOrWhiteSpace(rawExplanationKey))
        // Membuka scope cabang if untuk kondisi `!string.IsNullOrWhiteSpace(rawExplanationKey)`; pernyataan/deklarasi berikut berada di dalam batas blok
        // ini dalam ResolveExplanationKey.
        {
            // Mengembalikan `rawExplanationKey` (nilai raw explanation kunci) kepada pemanggil dalam ResolveExplanationKey; eksekusi jalur ini selesai setelah
            // nilai hasil ditentukan.
            return rawExplanationKey;
        // Menutup scope cabang if untuk kondisi `!string.IsNullOrWhiteSpace(rawExplanationKey)`; bagian berikut berada di luar batas blok tersebut dalam
        // ResolveExplanationKey.
        }

        // Memeriksa `isDerived` (nilai berstatus derived); blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ResolveExplanationKey.
        if (isDerived)
        // Membuka scope cabang if untuk kondisi `isDerived`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveExplanationKey.
        {
            // Menyiapkan variabel lokal `derivedKey` untuk nilai derived kunci dengan hasil pemetaan `metricKey` melalui cabang pola switch yang cocok. Tipe
            // variabel disimpulkan dari ekspresi nilai awal.
            var derivedKey = metricKey switch
            // Membuka scope pemetaan switch atas `metricKey`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveExplanationKey.
            {
                // Untuk pola `”cash_growth_percent” or ”net_worth_index” or ”growth_pattern_ratio”`, menghasilkan nilai literal `”players.support.meaning.growth”`
                // sebagai hasil switch.
                "cash_growth_percent" or "net_worth_index" => "players.support.meaning.cash_growth",
                "happiness_source_diversity_percent" => "players.support.meaning.happiness_diversity",
                "growth_pattern_ratio" => "players.support.meaning.growth",
                // Untuk pola `”income_diversification_index” or ”income_diversification_ratio” or ”income_share_i” or ”n_active_income_sources”`, menghasilkan
                // nilai literal `”players.support.meaning.income_mix”` sebagai hasil switch.
                "income_diversification_index" or "income_diversification_ratio" or "income_share_i" or
                    // Menggunakan nilai literal `”n_active_income_sources”` sebagai bagian ekspresi yang sedang disusun dalam ResolveExplanationKey.
                    "n_active_income_sources" => "players.support.meaning.income_mix",
                // Untuk pola `”business_expense_share_percent” or ”expense_management_efficiency”`, menghasilkan nilai literal
                // `”players.support.meaning.expense_efficiency”` sebagai hasil switch.
                "business_expense_share_percent" or "expense_management_efficiency" => "players.support.meaning.expense_efficiency",
                // Untuk pola `”meal_order_profit_margin_percent” or ”business_profit_margin” or ”business_efficiency_ratio”`, menghasilkan nilai literal
                // `”players.support.meaning.business”` sebagai hasil switch.
                "meal_order_profit_margin_percent" or "business_profit_margin" or "business_efficiency_ratio" => "players.support.meaning.business",
                // Untuk pola `”gold_roi_percentage”`, menghasilkan nilai literal `”players.support.meaning.gold_return”` sebagai hasil switch.
                "gold_roi_percentage" => "players.support.meaning.gold_return",
                // Untuk pola `”risk_exposure_percentage” or ”risk_cost_intensity”`, menghasilkan nilai literal `”players.support.meaning.risk_impact”` sebagai
                // hasil switch.
                "risk_exposure_percentage" or "risk_cost_intensity" => "players.support.meaning.risk_impact",
                // Untuk pola `”risk_readiness_percent”`, menghasilkan nilai literal `”players.support.meaning.risk_readiness”` sebagai hasil switch.
                "risk_readiness_percent" => "players.support.meaning.risk_readiness",
                // Untuk pola `”risk_mitigation_effectiveness” or ”insurance_activation_rate” or ”insurance_coverage_rate”`, menghasilkan nilai literal
                // `”players.support.meaning.risk_protection”` sebagai hasil switch.
                "risk_mitigation_effectiveness" or "insurance_activation_rate" or "insurance_coverage_rate" => "players.support.meaning.risk_protection",
                // Untuk pola `”risk_appetite_score” or ”risk_appetite_score_normalized” or ”risk_acceptance_rate”`, menghasilkan nilai literal
                // `”players.support.meaning.risk_appetite”` sebagai hasil switch.
                "risk_appetite_score" or "risk_appetite_score_normalized" or "risk_acceptance_rate" => "players.support.meaning.risk_appetite",
                // Untuk pola `”loan_burden_percent”`, menghasilkan nilai literal `”players.support.meaning.loan_burden”` sebagai hasil switch.
                "loan_burden_percent" => "players.support.meaning.loan_burden",
                // Untuk pola `”debt_leverage_ratio” or ”debt_ratio” or ”loan_repayment_discipline”`, menghasilkan nilai literal `”players.support.meaning.debt”`
                // sebagai hasil switch.
                "debt_leverage_ratio" or "debt_ratio" or "loan_repayment_discipline" => "players.support.meaning.debt",
                // Untuk pola `”financial_goal_progress_percent” or ”goal_ambition” or ”goal_ambition_index” or ”goal_setting_ambition” or ”goal_attempt_rate” or
                // ”goal_investment_rate”`, menghasilkan nilai literal `”players.support.meaning.goals”` sebagai hasil switch.
                "financial_goal_progress_percent" or "goal_ambition" or "goal_ambition_index" or "goal_setting_ambition" or "goal_attempt_rate" or "goal_investment_rate" => "players.support.meaning.goals",
                // Untuk pola `”income_action_focus_percent” or ”action_efficiency” or ”action_efficiency_percent” or ”action_diversity_score_avg”`, menghasilkan
                // nilai literal `”players.support.meaning.actions”` sebagai hasil switch.
                "income_action_focus_percent" or "action_efficiency" or "action_efficiency_percent" or "action_diversity_score_avg" => "players.support.meaning.actions",
                // Untuk pola `”ingredient_utilization_percent”`, menghasilkan nilai literal `”players.support.meaning.ingredient_utilization”` sebagai hasil
                // switch.
                "ingredient_utilization_percent" => "players.support.meaning.ingredient_utilization",
                // Untuk pola `”meal_order_success_rate”`, menghasilkan nilai literal `”players.support.meaning.orders”` sebagai hasil switch.
                "meal_order_success_rate" => "players.support.meaning.orders",
                // Untuk pola `”long_term_action_share_percent” or ”planning_horizon” or ”planning_horizon_percent”`, menghasilkan nilai literal
                // `”players.support.meaning.planning”` sebagai hasil switch.
                "long_term_action_share_percent" or "planning_horizon" or "planning_horizon_percent" => "players.support.meaning.planning",
                // Untuk pola `”need_fulfillment_diversity_percent” or ”fulfillment_diversity” or ”fulfillment_diversity_document_formula” or ”p_primary” or
                // ”p_secondary” or ”p_tertiary”`, menghasilkan nilai literal `”players.support.meaning.need_balance”` sebagai hasil switch.
                "need_fulfillment_diversity_percent" or "fulfillment_diversity" or "fulfillment_diversity_document_formula" or "p_primary" or "p_secondary" or "p_tertiary" => "players.support.meaning.need_balance",
                // Untuk pola `”mission_achievement”`, menghasilkan nilai literal `”players.support.meaning.needs”` sebagai hasil switch.
                "mission_achievement" => "players.support.meaning.needs",
                // Untuk pola `”donation_aggressiveness_percent” or ”donation_stability_std_deviation” or ”donation_ratio” or ”friday_participation_rate” or
                // ”donation_commitment_score” or ”donation_stabilit...`, menghasilkan nilai literal `”players.support.meaning.donation”` sebagai hasil switch.
                "donation_aggressiveness_percent" or "donation_stability_std_deviation" or "donation_ratio" or
                    // Menggunakan nilai literal `”friday_participation_rate”` sebagai bagian ekspresi yang sedang disusun dalam ResolveExplanationKey.
                    "friday_participation_rate" or "donation_commitment_score" or "donation_stability" or "donation_stability_index" => "players.support.meaning.donation",
                // Untuk pola `_`, menghasilkan `string.Empty`, yaitu nilai kosong bawaan tipe terkait sebagai hasil switch.
                _ => string.Empty
            // Menutup scope pemetaan switch atas `metricKey`; bagian berikut berada di luar batas blok tersebut dalam ResolveExplanationKey.
            };

            // Memeriksa kebalikan kondisi `string.IsNullOrWhiteSpace(derivedKey)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
            // ResolveExplanationKey.
            if (!string.IsNullOrWhiteSpace(derivedKey))
            // Membuka scope cabang if untuk kondisi `!string.IsNullOrWhiteSpace(derivedKey)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ResolveExplanationKey.
            {
                // Mengembalikan `derivedKey` (nilai derived kunci) kepada pemanggil dalam ResolveExplanationKey; eksekusi jalur ini selesai setelah nilai hasil
                // ditentukan.
                return derivedKey;
            // Menutup scope cabang if untuk kondisi `!string.IsNullOrWhiteSpace(derivedKey)`; bagian berikut berada di luar batas blok tersebut dalam
            // ResolveExplanationKey.
            }
        // Menutup scope cabang if untuk kondisi `isDerived`; bagian berikut berada di luar batas blok tersebut dalam ResolveExplanationKey.
        }

        // Menyiapkan variabel lokal `functionCategory` untuk nilai function category dengan memanggil `ResolveFunctionCategory` dengan `metricKey`. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var functionCategory = ResolveFunctionCategory(metricKey);
        // Memeriksa kebalikan kondisi `string.IsNullOrWhiteSpace(functionCategory)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // ResolveExplanationKey.
        if (!string.IsNullOrWhiteSpace(functionCategory))
        // Membuka scope cabang if untuk kondisi `!string.IsNullOrWhiteSpace(functionCategory)`; pernyataan/deklarasi berikut berada di dalam batas blok ini
        // dalam ResolveExplanationKey.
        {
            // Mengembalikan teks interpolasi `$”players.support.meaning.{functionCategory}”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program
            // berjalan kepada pemanggil dalam ResolveExplanationKey; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return $"players.support.meaning.{functionCategory}";
        // Menutup scope cabang if untuk kondisi `!string.IsNullOrWhiteSpace(functionCategory)`; bagian berikut berada di luar batas blok tersebut dalam
        // ResolveExplanationKey.
        }

        // Memeriksa `isDerived` (nilai berstatus derived); blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ResolveExplanationKey.
        if (isDerived)
        // Membuka scope cabang if untuk kondisi `isDerived`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveExplanationKey.
        {
            // Mengembalikan nilai literal `”players.support.meaning.derived”` kepada pemanggil dalam ResolveExplanationKey; eksekusi jalur ini selesai setelah
            // nilai hasil ditentukan.
            return "players.support.meaning.derived";
        // Menutup scope cabang if untuk kondisi `isDerived`; bagian berikut berada di luar batas blok tersebut dalam ResolveExplanationKey.
        }

        // Mengembalikan hasil pemetaan `unitKey` melalui cabang pola switch yang cocok kepada pemanggil dalam ResolveExplanationKey; eksekusi jalur ini
        // selesai setelah nilai hasil ditentukan.
        return unitKey switch
        // Membuka scope pemetaan switch atas `unitKey`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveExplanationKey.
        {
            // Untuk pola `”players.support.unit.coins”`, menghasilkan nilai literal `”players.support.meaning.raw_coins”` sebagai hasil switch.
            "players.support.unit.coins" => "players.support.meaning.raw_coins",
            // Untuk pola `”players.support.unit.points”`, menghasilkan nilai literal `”players.support.meaning.raw_points”` sebagai hasil switch.
            "players.support.unit.points" => "players.support.meaning.raw_points",
            // Untuk pola `”players.support.unit.rank”`, menghasilkan nilai literal `”players.support.meaning.raw_rank”` sebagai hasil switch.
            "players.support.unit.rank" => "players.support.meaning.raw_rank",
            // Untuk pola `”players.support.unit.day” or ”players.support.unit.turn” or ”players.support.unit.action_slot”`, menghasilkan nilai literal
            // `”players.support.meaning.raw_timeline”` sebagai hasil switch.
            "players.support.unit.day" or "players.support.unit.turn" or "players.support.unit.action_slot" => "players.support.meaning.raw_timeline",
            // Untuk pola `_` dengan syarat tambahan `unitKey.StartsWith(”players.support.unit.”, StringComparison.Ordinal) && unitKey !=
            // ”players.support.unit.coins” && unitKey != ”players.support.unit.points” && unitKey != ”pla...`, menghasilkan nilai literal
            // `”players.support.meaning.raw_count”` sebagai hasil switch.
            _ when unitKey.StartsWith("players.support.unit.", StringComparison.Ordinal) &&
                   // Melanjutkan ekspresi dengan perbandingan ketidaksamaan antara `unitKey` dan `”players.support.unit.coins”` dalam ResolveExplanationKey.
                   unitKey != "players.support.unit.coins" &&
                   // Melanjutkan ekspresi dengan perbandingan ketidaksamaan antara `unitKey` dan `”players.support.unit.points”` dalam ResolveExplanationKey.
                   unitKey != "players.support.unit.points" &&
                   // Melanjutkan ekspresi dengan perbandingan ketidaksamaan antara `unitKey` dan `”players.support.unit.percent”` dalam ResolveExplanationKey.
                   unitKey != "players.support.unit.percent" &&
                   // Melanjutkan ekspresi dengan perbandingan ketidaksamaan antara `unitKey` dan `”players.support.unit.multiplier”` dalam ResolveExplanationKey.
                   unitKey != "players.support.unit.multiplier" => "players.support.meaning.raw_count",
            // Untuk pola `_`, menghasilkan nilai literal `”players.support.meaning.raw”` sebagai hasil switch.
            _ => "players.support.meaning.raw"
        // Menutup scope pemetaan switch atas `unitKey`; bagian berikut berada di luar batas blok tersebut dalam ResolveExplanationKey.
        };
    // Menutup scope metode ResolveExplanationKey; bagian berikut berada di luar batas blok tersebut dalam ResolveExplanationKey.
    }

    // Mendefinisikan metode `ResolveGuidanceKey` dengan hasil bertipe `string`; operasi ini menangani resolve guidance kunci. Masukan: Parameter
    // `metricKey` bertipe `string` membawa nilai metric kunci; Parameter `numericValue` bertipe `double` membawa nilai numerik nilai; Parameter
    // `hasNumericValue` bertipe `bool` membawa nilai memiliki numerik nilai; Parameter `isDerived` bertipe `bool` membawa nilai berstatus derived;
    // Parameter `unitKey` bertipe `string` membawa nilai unit kunci.
    private static string ResolveGuidanceKey(
        // Parameter `metricKey` bertipe `string` membawa nilai metric kunci.
        string metricKey,
        // Parameter `numericValue` bertipe `double` membawa nilai numerik nilai.
        double numericValue,
        // Parameter `hasNumericValue` bertipe `bool` membawa nilai memiliki numerik nilai.
        bool hasNumericValue,
        // Parameter `isDerived` bertipe `bool` membawa nilai berstatus derived.
        bool isDerived,
        // Parameter `unitKey` bertipe `string` membawa nilai unit kunci.
        string unitKey)
    // Membuka scope metode ResolveGuidanceKey; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveGuidanceKey.
    {
        // Memeriksa `hasNumericValue` (nilai memiliki numerik nilai); blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ResolveGuidanceKey.
        if (hasNumericValue)
        // Membuka scope cabang if untuk kondisi `hasNumericValue`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveGuidanceKey.
        {
            // Menyiapkan variabel lokal `normalizedRatio` untuk nilai normalized ratio dengan `numericValue` (nilai numerik nilai). Tipe variabel disimpulkan
            // dari ekspresi nilai awal.
            var normalizedRatio = numericValue;

            // Mengembalikan hasil pemilihan bersyarat: ketika `metricKey switch { ”cash_growth_percent” or ”net_worth_index” when numericValue >= 300 =>
            // ”players.support.guide.wealth_exceptional”, ”cash_growth_percent” or ”net_worth_index...` benar gunakan `specificKey`, jika tidak gunakan
            // `ResolveGenericGuidanceKey(metricKey, isDerived, unitKey)` kepada pemanggil dalam ResolveGuidanceKey; eksekusi jalur ini selesai setelah nilai
            // hasil ditentukan.
            return metricKey switch
            // Membuka scope pemetaan switch atas `metricKey`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveGuidanceKey.
            {
                // Untuk pola `”cash_growth_percent” or ”net_worth_index”` dengan syarat tambahan `numericValue >= 300`, menghasilkan nilai literal
                // `”players.support.guide.wealth_exceptional”` sebagai hasil switch.
                "cash_growth_percent" or "net_worth_index" when numericValue >= 200 => "players.support.guide.wealth_exceptional",
                // Untuk pola `”cash_growth_percent” or ”net_worth_index”` dengan syarat tambahan `numericValue >= 200`, menghasilkan nilai literal
                // `”players.support.guide.wealth_strong”` sebagai hasil switch.
                "cash_growth_percent" or "net_worth_index" when numericValue >= 100 => "players.support.guide.wealth_strong",
                // Untuk pola `”cash_growth_percent” or ”net_worth_index”` dengan syarat tambahan `numericValue >= 100`, menghasilkan nilai literal
                // `”players.support.guide.wealth_growing”` sebagai hasil switch.
                "cash_growth_percent" or "net_worth_index" when numericValue > 0 => "players.support.guide.wealth_growing",
                "cash_growth_percent" or "net_worth_index" when numericValue == 0 => "players.support.guide.wealth_unchanged",
                // Untuk pola `”cash_growth_percent” or ”net_worth_index”`, menghasilkan nilai literal `”players.support.guide.wealth_declining”` sebagai hasil
                // switch.
                "cash_growth_percent" or "net_worth_index" => "players.support.guide.wealth_declining",
                "happiness_source_diversity_percent" when numericValue >= 67 => "players.support.guide.happiness_diverse",
                "happiness_source_diversity_percent" when numericValue >= 34 => "players.support.guide.happiness_mixed",
                "happiness_source_diversity_percent" => "players.support.guide.happiness_concentrated",
                // Untuk pola `”income_diversification_index” or ”income_diversification_ratio”` dengan syarat tambahan `numericValue >= 67`, menghasilkan nilai
                // literal `”players.support.guide.income_diverse”` sebagai hasil switch.
                "income_diversification_index" or "income_diversification_ratio" when numericValue >= 67 => "players.support.guide.income_diverse",
                // Untuk pola `”income_diversification_index” or ”income_diversification_ratio”` dengan syarat tambahan `numericValue < 34`, menghasilkan nilai
                // literal `”players.support.guide.income_concentrated”` sebagai hasil switch.
                "income_diversification_index" or "income_diversification_ratio" when numericValue < 34 => "players.support.guide.income_concentrated",
                // Untuk pola `”income_diversification_index” or ”income_diversification_ratio”`, menghasilkan nilai literal `”players.support.guide.income_mixed”`
                // sebagai hasil switch.
                "income_diversification_index" or "income_diversification_ratio" => "players.support.guide.income_mixed",
                // Untuk pola `”business_expense_share_percent” or ”expense_management_efficiency”`, menghasilkan nilai literal
                // `”players.support.guide.expense_efficiency”` sebagai hasil switch.
                "business_expense_share_percent" or "expense_management_efficiency" => "players.support.guide.expense_efficiency",
                // Untuk pola `”meal_order_profit_margin_percent” or ”business_profit_margin” or ”gold_roi_percentage”` dengan syarat tambahan `numericValue > 0`,
                // menghasilkan nilai literal `”players.support.guide.return_positive”` sebagai hasil switch.
                "meal_order_profit_margin_percent" or "business_profit_margin" or "gold_roi_percentage" when numericValue > 0 => "players.support.guide.return_positive",
                // Untuk pola `”meal_order_profit_margin_percent” or ”business_profit_margin” or ”gold_roi_percentage”` dengan syarat tambahan `numericValue < 0`,
                // menghasilkan nilai literal `”players.support.guide.return_negative”` sebagai hasil switch.
                "meal_order_profit_margin_percent" or "business_profit_margin" or "gold_roi_percentage" when numericValue < 0 => "players.support.guide.return_negative",
                // Untuk pola `”meal_order_profit_margin_percent” or ”business_profit_margin” or ”gold_roi_percentage”`, menghasilkan nilai literal
                // `”players.support.guide.return_even”` sebagai hasil switch.
                "meal_order_profit_margin_percent" or "business_profit_margin" or "gold_roi_percentage" => "players.support.guide.return_even",
                // Untuk pola `”business_efficiency_ratio”` dengan syarat tambahan `numericValue > 2`, menghasilkan nilai literal
                // `”players.support.guide.business_efficient”` sebagai hasil switch.
                "business_efficiency_ratio" when numericValue > 2 => "players.support.guide.business_efficient",
                // Untuk pola `”business_efficiency_ratio”` dengan syarat tambahan `numericValue < 1.5`, menghasilkan nilai literal
                // `”players.support.guide.business_inefficient”` sebagai hasil switch.
                "business_efficiency_ratio" when numericValue < 1.5 => "players.support.guide.business_inefficient",
                // Untuk pola `”business_efficiency_ratio”`, menghasilkan nilai literal `”players.support.guide.business_moderate”` sebagai hasil switch.
                "business_efficiency_ratio" => "players.support.guide.business_moderate",
                // Untuk pola `”risk_exposure_percentage”` dengan syarat tambahan `numericValue > 30`, menghasilkan nilai literal
                // `”players.support.guide.risk_high”` sebagai hasil switch.
                "risk_exposure_percentage" when numericValue > 30 => "players.support.guide.risk_high",
                // Untuk pola `”risk_exposure_percentage”` dengan syarat tambahan `numericValue < 10`, menghasilkan nilai literal `”players.support.guide.risk_low”`
                // sebagai hasil switch.
                "risk_exposure_percentage" when numericValue < 10 => "players.support.guide.risk_low",
                // Untuk pola `”risk_exposure_percentage”`, menghasilkan nilai literal `”players.support.guide.risk_moderate”` sebagai hasil switch.
                "risk_exposure_percentage" => "players.support.guide.risk_moderate",
                // Untuk pola `”risk_appetite_score” or ”risk_appetite_score_normalized”` dengan syarat tambahan `numericValue < 25`, menghasilkan nilai literal
                // `”players.support.guide.appetite_cautious”` sebagai hasil switch.
                "risk_appetite_score" or "risk_appetite_score_normalized" when numericValue < 25 => "players.support.guide.appetite_cautious",
                // Untuk pola `”risk_appetite_score” or ”risk_appetite_score_normalized”` dengan syarat tambahan `numericValue <= 75`, menghasilkan nilai literal
                // `”players.support.guide.appetite_balanced”` sebagai hasil switch.
                "risk_appetite_score" or "risk_appetite_score_normalized" when numericValue <= 75 => "players.support.guide.appetite_balanced",
                // Untuk pola `”risk_appetite_score” or ”risk_appetite_score_normalized”`, menghasilkan nilai literal `”players.support.guide.appetite_high”`
                // sebagai hasil switch.
                "risk_appetite_score" or "risk_appetite_score_normalized" => "players.support.guide.appetite_high",
                // Untuk pola `”risk_readiness_percent”` dengan syarat tambahan `numericValue >= 99.5`, menghasilkan nilai literal
                // `”players.support.guide.risk_readiness_complete”` sebagai hasil switch.
                "risk_readiness_percent" when numericValue >= 100 => "players.support.guide.risk_readiness_complete",
                // Untuk pola `”risk_readiness_percent”` dengan syarat tambahan `numericValue >= 50`, menghasilkan nilai literal
                // `”players.support.guide.risk_readiness_most”` sebagai hasil switch.
                "risk_readiness_percent" when numericValue >= 50 => "players.support.guide.risk_readiness_most",
                // Untuk pola `”risk_readiness_percent”`, menghasilkan nilai literal `”players.support.guide.risk_readiness_limited”` sebagai hasil switch.
                "risk_readiness_percent" => "players.support.guide.risk_readiness_limited",
                // Untuk pola `”loan_burden_percent” or ”debt_leverage_ratio”` dengan syarat tambahan `numericValue > 75`, menghasilkan nilai literal
                // `”players.support.guide.debt_high”` sebagai hasil switch.
                "loan_burden_percent" or "debt_leverage_ratio" when numericValue > 75 => "players.support.guide.debt_high",
                // Untuk pola `”loan_burden_percent” or ”debt_leverage_ratio”` dengan syarat tambahan `numericValue <= 25`, menghasilkan nilai literal
                // `”players.support.guide.debt_low”` sebagai hasil switch.
                "loan_burden_percent" or "debt_leverage_ratio" when numericValue <= 25 => "players.support.guide.debt_low",
                // Untuk pola `”loan_burden_percent” or ”debt_leverage_ratio”`, menghasilkan nilai literal `”players.support.guide.debt_moderate”` sebagai hasil
                // switch.
                "loan_burden_percent" or "debt_leverage_ratio" => "players.support.guide.debt_moderate",
                // Untuk pola `”financial_goal_progress_percent” or ”goal_ambition” or ”goal_ambition_index”` dengan syarat tambahan `numericValue >= 99.5`,
                // menghasilkan nilai literal `”players.support.guide.goals_complete”` sebagai hasil switch.
                "financial_goal_progress_percent" or "goal_ambition" or "goal_ambition_index" when numericValue >= 99.5 => "players.support.guide.goals_complete",
                // Untuk pola `”financial_goal_progress_percent” or ”goal_ambition” or ”goal_ambition_index”` dengan syarat tambahan `numericValue >= 67`,
                // menghasilkan nilai literal `”players.support.guide.goals_strong”` sebagai hasil switch.
                "financial_goal_progress_percent" or "goal_ambition" or "goal_ambition_index" when numericValue >= 67 => "players.support.guide.goals_strong",
                // Untuk pola `”financial_goal_progress_percent” or ”goal_ambition” or ”goal_ambition_index”` dengan syarat tambahan `numericValue < 34`,
                // menghasilkan nilai literal `”players.support.guide.goals_limited”` sebagai hasil switch.
                "financial_goal_progress_percent" or "goal_ambition" or "goal_ambition_index" when numericValue < 34 => "players.support.guide.goals_limited",
                // Untuk pola `”financial_goal_progress_percent” or ”goal_ambition” or ”goal_ambition_index”`, menghasilkan nilai literal
                // `”players.support.guide.goals_moderate”` sebagai hasil switch.
                "financial_goal_progress_percent" or "goal_ambition" or "goal_ambition_index" => "players.support.guide.goals_moderate",
                // Untuk pola `”loan_repayment_discipline”` dengan syarat tambahan `numericValue >= 99.5`, menghasilkan nilai literal
                // `”players.support.guide.loan_repaid”` sebagai hasil switch.
                "loan_repayment_discipline" when numericValue >= 99.5 => "players.support.guide.loan_repaid",
                // Untuk pola `”loan_repayment_discipline”`, menghasilkan nilai literal `”players.support.guide.loan_remaining”` sebagai hasil switch.
                "loan_repayment_discipline" => "players.support.guide.loan_remaining",
                // Untuk pola `”income_action_focus_percent” or ”action_efficiency_percent”` dengan syarat tambahan `numericValue > 60`, menghasilkan nilai literal
                // `”players.support.guide.action_income”` sebagai hasil switch.
                "income_action_focus_percent" or "action_efficiency_percent" when numericValue > 60 => "players.support.guide.action_income",
                // Untuk pola `”income_action_focus_percent” or ”action_efficiency_percent”` dengan syarat tambahan `numericValue < 40`, menghasilkan nilai literal
                // `”players.support.guide.action_exploration”` sebagai hasil switch.
                "income_action_focus_percent" or "action_efficiency_percent" when numericValue < 40 => "players.support.guide.action_exploration",
                // Untuk pola `”income_action_focus_percent” or ”action_efficiency_percent”`, menghasilkan nilai literal `”players.support.guide.action_balanced”`
                // sebagai hasil switch.
                "income_action_focus_percent" or "action_efficiency_percent" => "players.support.guide.action_balanced",
                // Untuk pola `”ingredient_utilization_percent”` dengan syarat tambahan `numericValue >= 80`, menghasilkan nilai literal
                // `”players.support.guide.ingredient_use_high”` sebagai hasil switch.
                "ingredient_utilization_percent" when numericValue >= 80 => "players.support.guide.ingredient_use_high",
                // Untuk pola `”ingredient_utilization_percent”` dengan syarat tambahan `numericValue < 60`, menghasilkan nilai literal
                // `”players.support.guide.ingredient_use_low”` sebagai hasil switch.
                "ingredient_utilization_percent" when numericValue < 60 => "players.support.guide.ingredient_use_low",
                // Untuk pola `”ingredient_utilization_percent”`, menghasilkan nilai literal `”players.support.guide.ingredient_use_moderate”` sebagai hasil switch.
                "ingredient_utilization_percent" => "players.support.guide.ingredient_use_moderate",
                // Untuk pola `”meal_order_success_rate”` dengan syarat tambahan `numericValue >= 80`, menghasilkan nilai literal
                // `”players.support.guide.orders_strong”` sebagai hasil switch.
                "meal_order_success_rate" when numericValue >= 80 => "players.support.guide.orders_strong",
                // Untuk pola `”meal_order_success_rate”` dengan syarat tambahan `numericValue < 60`, menghasilkan nilai literal
                // `”players.support.guide.orders_review”` sebagai hasil switch.
                "meal_order_success_rate" when numericValue < 60 => "players.support.guide.orders_review",
                // Untuk pola `”meal_order_success_rate”`, menghasilkan nilai literal `”players.support.guide.orders_moderate”` sebagai hasil switch.
                "meal_order_success_rate" => "players.support.guide.orders_moderate",
                // Untuk pola `”long_term_action_share_percent” or ”planning_horizon_percent”` dengan syarat tambahan `numericValue > 40`, menghasilkan nilai
                // literal `”players.support.guide.planning_long”` sebagai hasil switch.
                "long_term_action_share_percent" or "planning_horizon_percent" when numericValue > 40 => "players.support.guide.planning_long",
                // Untuk pola `”long_term_action_share_percent” or ”planning_horizon_percent”` dengan syarat tambahan `numericValue < 20`, menghasilkan nilai
                // literal `”players.support.guide.planning_short”` sebagai hasil switch.
                "long_term_action_share_percent" or "planning_horizon_percent" when numericValue < 20 => "players.support.guide.planning_short",
                // Untuk pola `”long_term_action_share_percent” or ”planning_horizon_percent”`, menghasilkan nilai literal
                // `”players.support.guide.planning_balanced”` sebagai hasil switch.
                "long_term_action_share_percent" or "planning_horizon_percent" => "players.support.guide.planning_balanced",
                // Untuk pola `”planning_horizon”` dengan syarat tambahan `normalizedRatio > 0.4`, menghasilkan nilai literal
                // `”players.support.guide.planning_long”` sebagai hasil switch.
                "planning_horizon" when normalizedRatio > 0.4 => "players.support.guide.planning_long",
                // Untuk pola `”planning_horizon”` dengan syarat tambahan `normalizedRatio < 0.2`, menghasilkan nilai literal
                // `”players.support.guide.planning_short”` sebagai hasil switch.
                "planning_horizon" when normalizedRatio < 0.2 => "players.support.guide.planning_short",
                // Untuk pola `”planning_horizon”`, menghasilkan nilai literal `”players.support.guide.planning_balanced”` sebagai hasil switch.
                "planning_horizon" => "players.support.guide.planning_balanced",
                // Untuk pola `”need_fulfillment_diversity_percent”` dengan syarat tambahan `numericValue >= 67`, menghasilkan nilai literal
                // `”players.support.guide.need_balance_high”` sebagai hasil switch.
                "need_fulfillment_diversity_percent" when numericValue >= 67 => "players.support.guide.need_balance_high",
                // Untuk pola `”need_fulfillment_diversity_percent”` dengan syarat tambahan `numericValue < 34`, menghasilkan nilai literal
                // `”players.support.guide.need_balance_low”` sebagai hasil switch.
                "need_fulfillment_diversity_percent" when numericValue < 34 => "players.support.guide.need_balance_low",
                // Untuk pola `”need_fulfillment_diversity_percent”`, menghasilkan nilai literal `”players.support.guide.need_balance_moderate”` sebagai hasil
                // switch.
                "need_fulfillment_diversity_percent" => "players.support.guide.need_balance_moderate",
                // Untuk pola `”fulfillment_diversity”` dengan syarat tambahan `numericValue >= 0.67`, menghasilkan nilai literal
                // `”players.support.guide.need_balance_high”` sebagai hasil switch.
                "fulfillment_diversity" when numericValue >= 0.67 => "players.support.guide.need_balance_high",
                // Untuk pola `”fulfillment_diversity”` dengan syarat tambahan `numericValue < 0.34`, menghasilkan nilai literal
                // `”players.support.guide.need_balance_low”` sebagai hasil switch.
                "fulfillment_diversity" when numericValue < 0.34 => "players.support.guide.need_balance_low",
                // Untuk pola `”fulfillment_diversity”`, menghasilkan nilai literal `”players.support.guide.need_balance_moderate”` sebagai hasil switch.
                "fulfillment_diversity" => "players.support.guide.need_balance_moderate",
                // Untuk pola `”growth_pattern_ratio”` dengan syarat tambahan `numericValue > 3`, menghasilkan nilai literal `”players.support.guide.growth_strong”`
                // sebagai hasil switch.
                "growth_pattern_ratio" when numericValue > 3 => "players.support.guide.growth_strong",
                // Untuk pola `”growth_pattern_ratio”` dengan syarat tambahan `numericValue < 1`, menghasilkan nilai literal
                // `”players.support.guide.growth_declining”` sebagai hasil switch.
                "growth_pattern_ratio" when numericValue < 1 => "players.support.guide.growth_declining",
                // Untuk pola `”growth_pattern_ratio”`, menghasilkan nilai literal `”players.support.guide.growth_steady”` sebagai hasil switch.
                "growth_pattern_ratio" => "players.support.guide.growth_steady",
                // Untuk pola `”donation_aggressiveness_percent”` dengan syarat tambahan `numericValue > 30`, menghasilkan nilai literal
                // `”players.support.guide.donation_aggressive”` sebagai hasil switch.
                "donation_aggressiveness_percent" when numericValue > 30 => "players.support.guide.donation_aggressive",
                // Untuk pola `”donation_aggressiveness_percent”` dengan syarat tambahan `numericValue < 10`, menghasilkan nilai literal
                // `”players.support.guide.donation_conservative”` sebagai hasil switch.
                "donation_aggressiveness_percent" when numericValue < 10 => "players.support.guide.donation_conservative",
                // Untuk pola `”donation_aggressiveness_percent”`, menghasilkan nilai literal `”players.support.guide.donation_moderate”` sebagai hasil switch.
                "donation_aggressiveness_percent" => "players.support.guide.donation_moderate",
                // Untuk pola `”donation_stability_std_deviation”` dengan syarat tambahan `numericValue <= 1`, menghasilkan nilai literal
                // `”players.support.guide.donation_stable”` sebagai hasil switch.
                "donation_stability_std_deviation" when numericValue <= 1 => "players.support.guide.donation_stable",
                // Untuk pola `”donation_stability_std_deviation”`, menghasilkan nilai literal `”players.support.guide.donation_variable”` sebagai hasil switch.
                "donation_stability_std_deviation" => "players.support.guide.donation_variable",
                // Untuk pola `”donation_commitment_score”` dengan syarat tambahan `numericValue >= 67`, menghasilkan nilai literal
                // `”players.support.guide.donation_commitment_strong”` sebagai hasil switch.
                "donation_commitment_score" when numericValue >= 67 => "players.support.guide.donation_commitment_strong",
                // Untuk pola `”donation_commitment_score”` dengan syarat tambahan `numericValue < 34`, menghasilkan nilai literal
                // `”players.support.guide.donation_commitment_weak”` sebagai hasil switch.
                "donation_commitment_score" when numericValue < 34 => "players.support.guide.donation_commitment_weak",
                // Untuk pola `”donation_commitment_score”`, menghasilkan nilai literal `”players.support.guide.donation_commitment_moderate”` sebagai hasil switch.
                "donation_commitment_score" => "players.support.guide.donation_commitment_moderate",
                // Untuk pola `”mission_achievement”` dengan syarat tambahan `numericValue >= 1`, menghasilkan nilai literal
                // `”players.support.guide.mission_complete”` sebagai hasil switch.
                "mission_achievement" when numericValue >= 1 => "players.support.guide.mission_complete",
                // Untuk pola `”mission_achievement”`, menghasilkan nilai literal `”players.support.guide.mission_incomplete”` sebagai hasil switch.
                "mission_achievement" => "players.support.guide.mission_incomplete",
                // Untuk pola `”friday_participation_rate”` dengan syarat tambahan `normalizedRatio >= 0.8`, menghasilkan nilai literal
                // `”players.support.guide.participation_high”` sebagai hasil switch.
                "friday_participation_rate" when normalizedRatio >= 0.8 => "players.support.guide.participation_high",
                // Untuk pola `”friday_participation_rate”`, menghasilkan nilai literal `”players.support.guide.participation_partial”` sebagai hasil switch.
                "friday_participation_rate" => "players.support.guide.participation_partial",
                // Untuk pola `_`, menghasilkan `string.Empty`, yaitu nilai kosong bawaan tipe terkait sebagai hasil switch.
                _ => string.Empty
            // Menutup scope pemetaan switch atas `metricKey`; bagian berikut berada di luar batas blok tersebut dalam ResolveGuidanceKey.
            } is { Length: > 0 } specificKey
                // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: specificKey dalam ResolveGuidanceKey.
                ? specificKey
                // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: ResolveGenericGuidanceKey(metricKey, isDerived, unitKey); dalam
                // ResolveGuidanceKey.
                : ResolveGenericGuidanceKey(metricKey, isDerived, unitKey);
        // Menutup scope cabang if untuk kondisi `hasNumericValue`; bagian berikut berada di luar batas blok tersebut dalam ResolveGuidanceKey.
        }

        // Mengembalikan memanggil `ResolveGenericGuidanceKey` dengan `metricKey`, `isDerived`, `unitKey` kepada pemanggil dalam ResolveGuidanceKey;
        // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return ResolveGenericGuidanceKey(metricKey, isDerived, unitKey);
    // Menutup scope metode ResolveGuidanceKey; bagian berikut berada di luar batas blok tersebut dalam ResolveGuidanceKey.
    }

    // Mendefinisikan metode `ResolveGenericGuidanceKey` dengan hasil bertipe `string`; operasi ini menangani resolve generic guidance kunci. Masukan:
    // Parameter `metricKey` bertipe `string` membawa nilai metric kunci; Parameter `isDerived` bertipe `bool` membawa nilai berstatus derived;
    // Parameter `unitKey` bertipe `string` membawa nilai unit kunci.
    private static string ResolveGenericGuidanceKey(string metricKey, bool isDerived, string unitKey)
    // Membuka scope metode ResolveGenericGuidanceKey; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveGenericGuidanceKey.
    {
        // Menyiapkan variabel lokal `functionCategory` untuk nilai function category dengan memanggil `ResolveFunctionCategory` dengan `metricKey`. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var functionCategory = ResolveFunctionCategory(metricKey);
        // Memeriksa kebalikan kondisi `string.IsNullOrWhiteSpace(functionCategory)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // ResolveGenericGuidanceKey.
        if (!string.IsNullOrWhiteSpace(functionCategory))
        // Membuka scope cabang if untuk kondisi `!string.IsNullOrWhiteSpace(functionCategory)`; pernyataan/deklarasi berikut berada di dalam batas blok ini
        // dalam ResolveGenericGuidanceKey.
        {
            // Mengembalikan teks interpolasi `$”players.support.guide.{functionCategory}”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program
            // berjalan kepada pemanggil dalam ResolveGenericGuidanceKey; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return $"players.support.guide.{functionCategory}";
        // Menutup scope cabang if untuk kondisi `!string.IsNullOrWhiteSpace(functionCategory)`; bagian berikut berada di luar batas blok tersebut dalam
        // ResolveGenericGuidanceKey.
        }

        // Memeriksa `isDerived` (nilai berstatus derived); blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ResolveGenericGuidanceKey.
        if (isDerived)
        // Membuka scope cabang if untuk kondisi `isDerived`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveGenericGuidanceKey.
        {
            // Mengembalikan hasil pemetaan `unitKey` melalui cabang pola switch yang cocok kepada pemanggil dalam ResolveGenericGuidanceKey; eksekusi jalur ini
            // selesai setelah nilai hasil ditentukan.
            return unitKey switch
            // Membuka scope pemetaan switch atas `unitKey`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveGenericGuidanceKey.
            {
                // Untuk pola `”players.support.unit.percent”`, menghasilkan nilai literal `”players.support.guide.percent”` sebagai hasil switch.
                "players.support.unit.percent" => "players.support.guide.percent",
                // Untuk pola `”players.support.unit.multiplier”`, menghasilkan nilai literal `”players.support.guide.ratio”` sebagai hasil switch.
                "players.support.unit.multiplier" => "players.support.guide.ratio",
                // Untuk pola `_`, menghasilkan nilai literal `”players.support.guide.derived”` sebagai hasil switch.
                _ => "players.support.guide.derived"
            // Menutup scope pemetaan switch atas `unitKey`; bagian berikut berada di luar batas blok tersebut dalam ResolveGenericGuidanceKey.
            };
        // Menutup scope cabang if untuk kondisi `isDerived`; bagian berikut berada di luar batas blok tersebut dalam ResolveGenericGuidanceKey.
        }

        // Mengembalikan hasil pemetaan `unitKey` melalui cabang pola switch yang cocok kepada pemanggil dalam ResolveGenericGuidanceKey; eksekusi jalur ini
        // selesai setelah nilai hasil ditentukan.
        return unitKey switch
        // Membuka scope pemetaan switch atas `unitKey`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveGenericGuidanceKey.
        {
            // Untuk pola `”players.support.unit.coins”`, menghasilkan nilai literal `”players.support.guide.coins”` sebagai hasil switch.
            "players.support.unit.coins" => "players.support.guide.coins",
            // Untuk pola `”players.support.unit.points”`, menghasilkan nilai literal `”players.support.guide.points”` sebagai hasil switch.
            "players.support.unit.points" => "players.support.guide.points",
            // Untuk pola `”players.support.unit.rank”`, menghasilkan nilai literal `”players.support.guide.rank”` sebagai hasil switch.
            "players.support.unit.rank" => "players.support.guide.rank",
            // Untuk pola `”players.support.unit.day” or ”players.support.unit.turn” or ”players.support.unit.action_slot”`, menghasilkan nilai literal
            // `”players.support.guide.turn”` sebagai hasil switch.
            "players.support.unit.day" or "players.support.unit.turn" or "players.support.unit.action_slot" => "players.support.guide.turn",
            // Untuk pola `_` dengan syarat tambahan `unitKey.StartsWith(”players.support.unit.”, StringComparison.Ordinal)`, menghasilkan nilai literal
            // `”players.support.guide.count”` sebagai hasil switch.
            _ when unitKey.StartsWith("players.support.unit.", StringComparison.Ordinal) => "players.support.guide.count",
            // Untuk pola `_`, menghasilkan nilai literal `”players.support.guide.raw”` sebagai hasil switch.
            _ => "players.support.guide.raw"
        // Menutup scope pemetaan switch atas `unitKey`; bagian berikut berada di luar batas blok tersebut dalam ResolveGenericGuidanceKey.
        };
    // Menutup scope metode ResolveGenericGuidanceKey; bagian berikut berada di luar batas blok tersebut dalam ResolveGenericGuidanceKey.
    }

    // Mendefinisikan metode `ResolveFunctionCategory` dengan hasil bertipe `string`; operasi ini menangani resolve function category. Masukan:
    // Parameter `metricKey` bertipe `string` membawa nilai metric kunci.
    private static string ResolveFunctionCategory(string metricKey)
    // Membuka scope metode ResolveFunctionCategory; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveFunctionCategory.
    {
        // Mengembalikan hasil pemetaan `metricKey` melalui cabang pola switch yang cocok kepada pemanggil dalam ResolveFunctionCategory; eksekusi jalur ini
        // selesai setelah nilai hasil ditentukan.
        return metricKey switch
        // Membuka scope pemetaan switch atas `metricKey`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveFunctionCategory.
        {
            // Untuk pola `”starting_coins”`, menghasilkan nilai literal `”baseline”` sebagai hasil switch.
            "starting_coins" => "baseline",
            // Untuk pola `”cash_in_total” or ”coins_earned_total” or ”freelance_income” or ”meal_income” or ”gold_income” or ”donations_received” or
            // ”other_income” or ”total_income”`, menghasilkan nilai literal `”income”` sebagai hasil switch.
            "cash_in_total" or "coins_earned_total" or "freelance_income" or "meal_income" or
                // Menggunakan nilai literal `”gold_income”` sebagai bagian ekspresi yang sedang disusun dalam ResolveFunctionCategory.
                "gold_income" or "donations_received" or "other_income" or "total_income" => "income",
            // Untuk pola `”cash_out_total” or ”coins_spent_total” or ”essential_expenses” or ”total_expenses”`, menghasilkan nilai literal `”spending”` sebagai
            // hasil switch.
            "cash_out_total" or "coins_spent_total" or "essential_expenses" or "total_expenses" => "spending",
            // Untuk pola `”coins_held_current” or ”coins_net_end” or ”coins_net_end_game” or ”cash_net_total”`, menghasilkan nilai literal `”cash_position”`
            // sebagai hasil switch.
            "coins_held_current" or "coins_net_end" or "coins_net_end_game" or "cash_net_total" => "cash_position",
            // Untuk pola `”coins_saved” or ”coins_in_savings_goal”`, menghasilkan nilai literal `”savings”` sebagai hasil switch.
            "coins_saved" or "coins_in_savings_goal" => "savings",
            // Untuk pola `”mission_bonus_pts” or ”total_happiness_points” or ”final_rank” or ”winner_flag” or ”finish_line_reached” or ”dnf_flag” or ”value”`,
            // menghasilkan nilai literal `”outcome”` sebagai hasil switch.
            "mission_bonus_pts" or "total_happiness_points" or "final_rank" or "winner_flag" or
                // Menggunakan nilai literal `”finish_line_reached”` sebagai bagian ekspresi yang sedang disusun dalam ResolveFunctionCategory.
                "finish_line_reached" or "dnf_flag" or "value" => "outcome",
            // Untuk pola `”game_id” or ”session_id” or ”user_id” or ”player_alias” or ”game_mode” or ”seed_source”`, menghasilkan nilai literal
            // `”session_context”` sebagai hasil switch.
            "game_id" or "session_id" or "user_id" or "player_alias" or "game_mode" or "seed_source" => "session_context",
            // Untuk pola `_` dengan syarat tambahan `metricKey.Contains(”ingredient”, StringComparison.OrdinalIgnoreCase)`, menghasilkan nilai literal
            // `”inventory”` sebagai hasil switch.
            _ when metricKey.Contains("ingredient", StringComparison.OrdinalIgnoreCase) => "inventory",
            // Untuk pola `_` dengan syarat tambahan `metricKey.Contains(”meal_order”, StringComparison.OrdinalIgnoreCase) || metricKey.Contains(”business”,
            // StringComparison.OrdinalIgnoreCase)`, menghasilkan nilai literal `”business_activity”` sebagai hasil switch.
            _ when metricKey.Contains("meal_order", StringComparison.OrdinalIgnoreCase) ||
                   // Melanjutkan pengolahan dengan memeriksa apakah `metricKey` memuat `”business”`, `StringComparison.OrdinalIgnoreCase` dalam
                   // ResolveFunctionCategory.
                   metricKey.Contains("business", StringComparison.OrdinalIgnoreCase) => "business_activity",
            // Untuk pola `_` dengan syarat tambahan `metricKey.Contains(”need”, StringComparison.OrdinalIgnoreCase) || metricKey.Contains(”fulfillment”,
            // StringComparison.OrdinalIgnoreCase) || metricKey is ”collection_mission_com...`, menghasilkan nilai literal `”needs”` sebagai hasil switch.
            _ when metricKey.Contains("need", StringComparison.OrdinalIgnoreCase) ||
                   // Melanjutkan pengolahan dengan memeriksa apakah `metricKey` memuat `”fulfillment”`, `StringComparison.OrdinalIgnoreCase` dalam
                   // ResolveFunctionCategory.
                   metricKey.Contains("fulfillment", StringComparison.OrdinalIgnoreCase) ||
                   // Menggunakan `metricKey` (nilai metric kunci) sebagai bagian ekspresi yang sedang disusun dalam ResolveFunctionCategory.
                   metricKey is "collection_mission_complete" or "specific_tertiary_need" or "mission_achievement" or
                       // Menggunakan nilai literal `”p_primary”` sebagai bagian ekspresi yang sedang disusun dalam ResolveFunctionCategory.
                       "p_primary" or "p_secondary" or "p_tertiary" => "needs",
            // Untuk pola `_` dengan syarat tambahan `metricKey.Contains(”donat”, StringComparison.OrdinalIgnoreCase) || metricKey.Contains(”friday”,
            // StringComparison.OrdinalIgnoreCase)`, menghasilkan nilai literal `”donation”` sebagai hasil switch.
            _ when metricKey.Contains("donat", StringComparison.OrdinalIgnoreCase) ||
                   // Melanjutkan pengolahan dengan memeriksa apakah `metricKey` memuat `”friday”`, `StringComparison.OrdinalIgnoreCase` dalam ResolveFunctionCategory.
                   metricKey.Contains("friday", StringComparison.OrdinalIgnoreCase) => "donation",
            // Untuk pola `_` dengan syarat tambahan `metricKey.Contains(”per_turn”, StringComparison.OrdinalIgnoreCase) || metricKey.Contains(”action”,
            // StringComparison.OrdinalIgnoreCase) || metricKey.Contains(”event”, StringCom...`, menghasilkan nilai literal `”timeline”` sebagai hasil switch.
            _ when metricKey.Contains("per_turn", StringComparison.OrdinalIgnoreCase) ||
                   // Melanjutkan pengolahan dengan memeriksa apakah `metricKey` memuat `”action”`, `StringComparison.OrdinalIgnoreCase` dalam ResolveFunctionCategory.
                   metricKey.Contains("action", StringComparison.OrdinalIgnoreCase) ||
                   // Melanjutkan pengolahan dengan memeriksa apakah `metricKey` memuat `”event”`, `StringComparison.OrdinalIgnoreCase` dalam ResolveFunctionCategory.
                   metricKey.Contains("event", StringComparison.OrdinalIgnoreCase) ||
                   // Melanjutkan pengolahan dengan memeriksa apakah `metricKey` memuat `”transaction”`, `StringComparison.OrdinalIgnoreCase` dalam
                   // ResolveFunctionCategory.
                   metricKey.Contains("transaction", StringComparison.OrdinalIgnoreCase) ||
                   // Melanjutkan pengolahan dengan memeriksa apakah `metricKey` memuat `”sequence”`, `StringComparison.OrdinalIgnoreCase` dalam
                   // ResolveFunctionCategory.
                   metricKey.Contains("sequence", StringComparison.OrdinalIgnoreCase) ||
                   // Melanjutkan pengolahan dengan memanggil `metricKey.StartsWith` dengan `”day_”`, `StringComparison.OrdinalIgnoreCase` dalam
                   // ResolveFunctionCategory.
                   metricKey.StartsWith("day_", StringComparison.OrdinalIgnoreCase) ||
                   // Melanjutkan pengolahan dengan memeriksa apakah `metricKey` memuat `”_day_”`, `StringComparison.OrdinalIgnoreCase` dalam ResolveFunctionCategory.
                   metricKey.Contains("_day_", StringComparison.OrdinalIgnoreCase) ||
                   // Melanjutkan pengolahan dengan memeriksa apakah `metricKey` memuat `”timestamp”`, `StringComparison.OrdinalIgnoreCase` dalam
                   // ResolveFunctionCategory.
                   metricKey.Contains("timestamp", StringComparison.OrdinalIgnoreCase) => "timeline",
            // Untuk pola `_` dengan syarat tambahan `metricKey.Contains(”gold”, StringComparison.OrdinalIgnoreCase)`, menghasilkan nilai literal
            // `”gold_investment”` sebagai hasil switch.
            _ when metricKey.Contains("gold", StringComparison.OrdinalIgnoreCase) => "gold_investment",
            // Untuk pola `_` dengan syarat tambahan `metricKey.Contains(”pension”, StringComparison.OrdinalIgnoreCase) || metricKey ==
            // ”leftover_coins_end_game”`, menghasilkan nilai literal `”pension”` sebagai hasil switch.
            _ when metricKey.Contains("pension", StringComparison.OrdinalIgnoreCase) ||
                   // Melanjutkan ekspresi dengan perbandingan kesamaan antara `metricKey` dan `”leftover_coins_end_game”` dalam ResolveFunctionCategory.
                   metricKey == "leftover_coins_end_game" => "pension",
            // Untuk pola `_` dengan syarat tambahan `metricKey.Contains(”risk”, StringComparison.OrdinalIgnoreCase) || metricKey.Contains(”insurance”,
            // StringComparison.OrdinalIgnoreCase) || metricKey.Contains(”emergency”, String...`, menghasilkan nilai literal `”risk”` sebagai hasil switch.
            _ when metricKey.Contains("risk", StringComparison.OrdinalIgnoreCase) ||
                   // Melanjutkan pengolahan dengan memeriksa apakah `metricKey` memuat `”insurance”`, `StringComparison.OrdinalIgnoreCase` dalam
                   // ResolveFunctionCategory.
                   metricKey.Contains("insurance", StringComparison.OrdinalIgnoreCase) ||
                   // Melanjutkan pengolahan dengan memeriksa apakah `metricKey` memuat `”emergency”`, `StringComparison.OrdinalIgnoreCase` dalam
                   // ResolveFunctionCategory.
                   metricKey.Contains("emergency", StringComparison.OrdinalIgnoreCase) => "risk",
            // Untuk pola `_` dengan syarat tambahan `metricKey.Contains(”financial_goal”, StringComparison.OrdinalIgnoreCase) || metricKey.StartsWith(”goal_”,
            // StringComparison.OrdinalIgnoreCase)`, menghasilkan nilai literal `”goals”` sebagai hasil switch.
            _ when metricKey.Contains("financial_goal", StringComparison.OrdinalIgnoreCase) ||
                   // Melanjutkan pengolahan dengan memanggil `metricKey.StartsWith` dengan `”goal_”`, `StringComparison.OrdinalIgnoreCase` dalam
                   // ResolveFunctionCategory.
                   metricKey.StartsWith("goal_", StringComparison.OrdinalIgnoreCase) => "goals",
            // Untuk pola `_` dengan syarat tambahan `metricKey.Contains(”loan”, StringComparison.OrdinalIgnoreCase) || metricKey.Contains(”debt”,
            // StringComparison.OrdinalIgnoreCase)`, menghasilkan nilai literal `”debt”` sebagai hasil switch.
            _ when metricKey.Contains("loan", StringComparison.OrdinalIgnoreCase) ||
                   // Melanjutkan pengolahan dengan memeriksa apakah `metricKey` memuat `”debt”`, `StringComparison.OrdinalIgnoreCase` dalam ResolveFunctionCategory.
                   metricKey.Contains("debt", StringComparison.OrdinalIgnoreCase) => "debt",
            // Untuk pola `_` dengan syarat tambahan `metricKey.Contains(”point”, StringComparison.OrdinalIgnoreCase) || metricKey.EndsWith(”_pts”,
            // StringComparison.OrdinalIgnoreCase) || metricKey.Contains(”rank”, StringCompariso...`, menghasilkan nilai literal `”outcome”` sebagai hasil
            // switch.
            _ when metricKey.Contains("point", StringComparison.OrdinalIgnoreCase) ||
                   // Melanjutkan pengolahan dengan memanggil `metricKey.EndsWith` dengan `”_pts”`, `StringComparison.OrdinalIgnoreCase` dalam ResolveFunctionCategory.
                   metricKey.EndsWith("_pts", StringComparison.OrdinalIgnoreCase) ||
                   // Melanjutkan pengolahan dengan memeriksa apakah `metricKey` memuat `”rank”`, `StringComparison.OrdinalIgnoreCase` dalam ResolveFunctionCategory.
                   metricKey.Contains("rank", StringComparison.OrdinalIgnoreCase) => "outcome",
            // Untuk pola `_`, menghasilkan `string.Empty`, yaitu nilai kosong bawaan tipe terkait sebagai hasil switch.
            _ => string.Empty
        // Menutup scope pemetaan switch atas `metricKey`; bagian berikut berada di luar batas blok tersebut dalam ResolveFunctionCategory.
        };
    // Menutup scope metode ResolveFunctionCategory; bagian berikut berada di luar batas blok tersebut dalam ResolveFunctionCategory.
    }

    // Mendefinisikan metode `ResolveRecommendationKey` dengan hasil bertipe `string`; operasi ini menangani resolve recommendation kunci. Masukan:
    // Parameter `metricKey` bertipe `string` membawa nilai metric kunci; Parameter `numericValue` bertipe `double` membawa nilai numerik nilai;
    // Parameter `hasNumericValue` bertipe `bool` membawa nilai memiliki numerik nilai.
    private static string ResolveRecommendationKey(string metricKey, double numericValue, bool hasNumericValue)
    // Membuka scope metode ResolveRecommendationKey; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveRecommendationKey.
    {
        // Memeriksa `hasNumericValue` (nilai memiliki numerik nilai); blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // ResolveRecommendationKey.
        if (hasNumericValue)
        // Membuka scope cabang if untuk kondisi `hasNumericValue`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ResolveRecommendationKey.
        {
            // Menyiapkan variabel lokal `resultSpecificKey` untuk nilai hasil specific kunci dengan hasil pemetaan `metricKey` melalui cabang pola switch yang
            // cocok. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var resultSpecificKey = metricKey switch
            // Membuka scope pemetaan switch atas `metricKey`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveRecommendationKey.
            {
                // Untuk pola `”cash_growth_percent” or ”net_worth_index”` dengan syarat tambahan `numericValue < 100`, menghasilkan nilai literal
                // `”players.support.recommendation.cash_recover”` sebagai hasil switch.
                "cash_growth_percent" or "net_worth_index" when numericValue < 0 => "players.support.recommendation.cash_recover",
                // Untuk pola `”income_diversification_index” or ”income_diversification_ratio”` dengan syarat tambahan `numericValue < 34`, menghasilkan nilai
                // literal `”players.support.recommendation.income_mix_recover”` sebagai hasil switch.
                "income_diversification_index" or "income_diversification_ratio" when numericValue < 34 => "players.support.recommendation.income_mix_recover",
                // Untuk pola `”meal_order_profit_margin_percent” or ”business_profit_margin”` dengan syarat tambahan `numericValue <= 0`, menghasilkan nilai
                // literal `”players.support.recommendation.business_recover”` sebagai hasil switch.
                "meal_order_profit_margin_percent" or "business_profit_margin" when numericValue <= 0 => "players.support.recommendation.business_recover",
                // Untuk pola `”risk_appetite_score” or ”risk_appetite_score_normalized”` dengan syarat tambahan `numericValue > 75`, menghasilkan nilai literal
                // `”players.support.recommendation.risk_reduce”` sebagai hasil switch.
                "risk_appetite_score" or "risk_appetite_score_normalized" when numericValue > 75 => "players.support.recommendation.risk_reduce",
                // Untuk pola `”loan_burden_percent” or ”debt_leverage_ratio”` dengan syarat tambahan `numericValue > 75`, menghasilkan nilai literal
                // `”players.support.recommendation.debt_reduce”` sebagai hasil switch.
                "loan_burden_percent" or "debt_leverage_ratio" when numericValue > 75 => "players.support.recommendation.debt_reduce",
                // Untuk pola `”financial_goal_progress_percent” or ”goal_ambition” or ”goal_ambition_index”` dengan syarat tambahan `numericValue < 50`,
                // menghasilkan nilai literal `”players.support.recommendation.goals_focus”` sebagai hasil switch.
                "financial_goal_progress_percent" or "goal_ambition" or "goal_ambition_index" when numericValue < 50 => "players.support.recommendation.goals_focus",
                // Untuk pola `”income_action_focus_percent” or ”action_efficiency_percent”` dengan syarat tambahan `numericValue < 40`, menghasilkan nilai literal
                // `”players.support.recommendation.action_income”` sebagai hasil switch.
                "income_action_focus_percent" or "action_efficiency_percent" when numericValue < 40 => "players.support.recommendation.action_income",
                // Untuk pola `”ingredient_utilization_percent” or ”meal_order_success_rate”` dengan syarat tambahan `numericValue < 60`, menghasilkan nilai literal
                // `”players.support.recommendation.orders_improve”` sebagai hasil switch.
                "ingredient_utilization_percent" or "meal_order_success_rate" when numericValue < 60 => "players.support.recommendation.orders_improve",
                // Untuk pola `”long_term_action_share_percent” or ”planning_horizon_percent”` dengan syarat tambahan `numericValue < 20`, menghasilkan nilai
                // literal `”players.support.recommendation.planning_build”` sebagai hasil switch.
                "long_term_action_share_percent" or "planning_horizon_percent" when numericValue < 20 => "players.support.recommendation.planning_build",
                // Untuk pola `”planning_horizon”` dengan syarat tambahan `numericValue < 0.2`, menghasilkan nilai literal
                // `”players.support.recommendation.planning_build”` sebagai hasil switch.
                "planning_horizon" when numericValue < 0.2 => "players.support.recommendation.planning_build",
                // Untuk pola `”need_fulfillment_diversity_percent”` dengan syarat tambahan `numericValue < 67`, menghasilkan nilai literal
                // `”players.support.recommendation.needs_balance”` sebagai hasil switch.
                "need_fulfillment_diversity_percent" when numericValue < 67 => "players.support.recommendation.needs_balance",
                // Untuk pola `”fulfillment_diversity”` dengan syarat tambahan `numericValue < 0.67`, menghasilkan nilai literal
                // `”players.support.recommendation.needs_balance”` sebagai hasil switch.
                "fulfillment_diversity" when numericValue < 0.67 => "players.support.recommendation.needs_balance",
                // Untuk pola `”donation_commitment_score”` dengan syarat tambahan `numericValue < 34`, menghasilkan nilai literal
                // `”players.support.recommendation.donation_stabilize”` sebagai hasil switch.
                "donation_commitment_score" when numericValue < 34 => "players.support.recommendation.donation_stabilize",
                // Untuk pola `”total_happiness_pts”` dengan syarat tambahan `numericValue < 0`, menghasilkan nilai literal
                // `”players.support.recommendation.outcome_recover”` sebagai hasil switch.
                "total_happiness_pts" when numericValue < 0 => "players.support.recommendation.outcome_recover",
                // Untuk pola `_`, menghasilkan `string.Empty`, yaitu nilai kosong bawaan tipe terkait sebagai hasil switch.
                _ => string.Empty
            // Menutup scope pemetaan switch atas `metricKey`; bagian berikut berada di luar batas blok tersebut dalam ResolveRecommendationKey.
            };

            // Memeriksa kebalikan kondisi `string.IsNullOrWhiteSpace(resultSpecificKey)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
            // ResolveRecommendationKey.
            if (!string.IsNullOrWhiteSpace(resultSpecificKey))
            // Membuka scope cabang if untuk kondisi `!string.IsNullOrWhiteSpace(resultSpecificKey)`; pernyataan/deklarasi berikut berada di dalam batas blok
            // ini dalam ResolveRecommendationKey.
            {
                // Mengembalikan `resultSpecificKey` (nilai hasil specific kunci) kepada pemanggil dalam ResolveRecommendationKey; eksekusi jalur ini selesai
                // setelah nilai hasil ditentukan.
                return resultSpecificKey;
            // Menutup scope cabang if untuk kondisi `!string.IsNullOrWhiteSpace(resultSpecificKey)`; bagian berikut berada di luar batas blok tersebut dalam
            // ResolveRecommendationKey.
            }
        // Menutup scope cabang if untuk kondisi `hasNumericValue`; bagian berikut berada di luar batas blok tersebut dalam ResolveRecommendationKey.
        }

        // Menyiapkan variabel lokal `recommendationCategory` untuk nilai recommendation category dengan hasil pemetaan `metricKey` melalui cabang pola
        // switch yang cocok. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var recommendationCategory = metricKey switch
        // Membuka scope pemetaan switch atas `metricKey`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveRecommendationKey.
        {
            // Untuk pola `”cash_growth_percent” or ”net_worth_index” or ”growth_pattern_ratio”`, menghasilkan nilai literal `”wealth”` sebagai hasil switch.
            "cash_growth_percent" or "net_worth_index" or "growth_pattern_ratio" => "wealth",
            "happiness_source_diversity_percent" => "happiness_diversity",
            // Untuk pola `”income_diversification_index” or ”income_diversification_ratio” or ”income_share_i” or ”n_active_income_sources”`, menghasilkan
            // nilai literal `”income_mix”` sebagai hasil switch.
            "income_diversification_index" or "income_diversification_ratio" or "income_share_i" or
                // Menggunakan nilai literal `”n_active_income_sources”` sebagai bagian ekspresi yang sedang disusun dalam ResolveRecommendationKey.
                "n_active_income_sources" => "income_mix",
            // Untuk pola `”business_expense_share_percent” or ”expense_management_efficiency”`, menghasilkan nilai literal `”productive_spending”` sebagai
            // hasil switch.
            "business_expense_share_percent" or "expense_management_efficiency" => "productive_spending",
            // Untuk pola `”risk_readiness_percent” or ”risk_mitigation_effectiveness” or ”insurance_activation_rate” or ”insurance_coverage_rate”`,
            // menghasilkan nilai literal `”protection”` sebagai hasil switch.
            "risk_readiness_percent" or "risk_mitigation_effectiveness" or "insurance_activation_rate" or "insurance_coverage_rate" => "protection",
            // Untuk pola `”income_action_focus_percent” or ”action_efficiency” or ”action_efficiency_percent”`, menghasilkan nilai literal `”action_balance”`
            // sebagai hasil switch.
            "income_action_focus_percent" or "action_efficiency" or "action_efficiency_percent" => "action_balance",
            // Untuk pola `”long_term_action_share_percent” or ”planning_horizon” or ”planning_horizon_percent”`, menghasilkan nilai literal `”planning”`
            // sebagai hasil switch.
            "long_term_action_share_percent" or "planning_horizon" or "planning_horizon_percent" => "planning",
            // Untuk pola `_`, menghasilkan memanggil `ResolveFunctionCategory` dengan `metricKey` sebagai hasil switch.
            _ => ResolveFunctionCategory(metricKey)
        // Menutup scope pemetaan switch atas `metricKey`; bagian berikut berada di luar batas blok tersebut dalam ResolveRecommendationKey.
        };

        // Mengembalikan hasil pemilihan bersyarat: ketika `string.IsNullOrWhiteSpace(recommendationCategory)` benar gunakan
        // `”players.support.recommendation.balance”`, jika tidak gunakan `$”players.support.recommendation.{recommendationCategory}”` kepada pemanggil
        // dalam ResolveRecommendationKey; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return string.IsNullOrWhiteSpace(recommendationCategory)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: ”players.support.recommendation.balance” dalam
            // ResolveRecommendationKey.
            ? "players.support.recommendation.balance"
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: $”players.support.recommendation.{recommendationCategory}”; dalam
            // ResolveRecommendationKey.
            : $"players.support.recommendation.{recommendationCategory}";
    // Menutup scope metode ResolveRecommendationKey; bagian berikut berada di luar batas blok tersebut dalam ResolveRecommendationKey.
    }

    // Mendefinisikan metode `ResolveFormulaKey` dengan hasil bertipe `string`; operasi ini menangani resolve formula kunci. Masukan: Parameter
    // `metricKey` bertipe `string` membawa nilai metric kunci.
    private static string ResolveFormulaKey(string metricKey)
    // Membuka scope metode ResolveFormulaKey; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveFormulaKey.
    {
        // Mengembalikan hasil pemetaan `metricKey` melalui cabang pola switch yang cocok kepada pemanggil dalam ResolveFormulaKey; eksekusi jalur ini
        // selesai setelah nilai hasil ditentukan.
        return metricKey switch
        // Membuka scope pemetaan switch atas `metricKey`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveFormulaKey.
        {
            // Untuk pola `”cash_growth_percent” or ”net_worth_index”`, menghasilkan nilai literal `”players.support.formula.net_worth”` sebagai hasil switch.
            "cash_growth_percent" or "net_worth_index" => "players.support.formula.net_worth",
            "happiness_source_diversity_percent" => "players.support.formula.happiness_portfolio",
            // Untuk pola `”income_diversification_index”`, menghasilkan nilai literal `”players.support.formula.income_diversification_index”` sebagai hasil
            // switch.
            "income_diversification_index" => "players.support.formula.income_diversification_index",
            // Untuk pola `”income_diversification_ratio”`, menghasilkan nilai literal `”players.support.formula.income_diversification”` sebagai hasil switch.
            "income_diversification_ratio" => "players.support.formula.income_diversification",
            // Untuk pola `”business_expense_share_percent” or ”expense_management_efficiency”`, menghasilkan nilai literal
            // `”players.support.formula.expense_efficiency”` sebagai hasil switch.
            "business_expense_share_percent" or "expense_management_efficiency" => "players.support.formula.expense_efficiency",
            // Untuk pola `”meal_order_profit_margin_percent” or ”business_profit_margin”`, menghasilkan nilai literal
            // `”players.support.formula.business_margin”` sebagai hasil switch.
            "meal_order_profit_margin_percent" or "business_profit_margin" => "players.support.formula.business_margin",
            // Untuk pola `”business_efficiency_ratio”`, menghasilkan nilai literal `”players.support.formula.business_efficiency”` sebagai hasil switch.
            "business_efficiency_ratio" => "players.support.formula.business_efficiency",
            // Untuk pola `”gold_roi_percentage”`, menghasilkan nilai literal `”players.support.formula.gold_roi”` sebagai hasil switch.
            "gold_roi_percentage" => "players.support.formula.gold_roi",
            // Untuk pola `”risk_exposure_percentage”`, menghasilkan nilai literal `”players.support.formula.risk_exposure”` sebagai hasil switch.
            "risk_exposure_percentage" => "players.support.formula.risk_exposure",
            // Untuk pola `”risk_mitigation_effectiveness”`, menghasilkan nilai literal `”players.support.formula.risk_mitigation”` sebagai hasil switch.
            "risk_mitigation_effectiveness" => "players.support.formula.risk_mitigation",
            // Untuk pola `”risk_readiness_percent” or ”risk_appetite_score” or ”risk_appetite_score_normalized”`, menghasilkan nilai literal
            // `”players.support.formula.risk_appetite”` sebagai hasil switch.
            "risk_readiness_percent" or "risk_appetite_score" or "risk_appetite_score_normalized" => "players.support.formula.risk_appetite",
            // Untuk pola `”loan_burden_percent” or ”debt_leverage_ratio”`, menghasilkan nilai literal `”players.support.formula.debt_leverage”` sebagai hasil
            // switch.
            "loan_burden_percent" or "debt_leverage_ratio" => "players.support.formula.debt_leverage",
            // Untuk pola `”loan_repayment_discipline”`, menghasilkan nilai literal `”players.support.formula.loan_discipline”` sebagai hasil switch.
            "loan_repayment_discipline" => "players.support.formula.loan_discipline",
            // Untuk pola `”debt_ratio”`, menghasilkan nilai literal `”players.support.formula.debt_ratio”` sebagai hasil switch.
            "debt_ratio" => "players.support.formula.debt_ratio",
            // Untuk pola `”financial_goal_progress_percent” or ”goal_ambition” or ”goal_ambition_index”`, menghasilkan nilai literal
            // `”players.support.formula.goal_ambition_index”` sebagai hasil switch.
            "financial_goal_progress_percent" or "goal_ambition" or "goal_ambition_index" => "players.support.formula.goal_ambition_index",
            // Untuk pola `”goal_setting_ambition”`, menghasilkan nilai literal `”players.support.formula.goal_ambition”` sebagai hasil switch.
            "goal_setting_ambition" => "players.support.formula.goal_ambition",
            // Untuk pola `”income_action_focus_percent” or ”action_efficiency” or ”action_efficiency_percent”`, menghasilkan nilai literal
            // `”players.support.formula.action_efficiency”` sebagai hasil switch.
            "income_action_focus_percent" or "action_efficiency" or "action_efficiency_percent" => "players.support.formula.action_efficiency",
            // Untuk pola `”ingredient_utilization_percent” or ”meal_order_success_rate”`, menghasilkan nilai literal `”players.support.formula.order_success”`
            // sebagai hasil switch.
            "ingredient_utilization_percent" or "meal_order_success_rate" => "players.support.formula.order_success",
            // Untuk pola `”long_term_action_share_percent” or ”planning_horizon” or ”planning_horizon_percent”`, menghasilkan nilai literal
            // `”players.support.formula.planning”` sebagai hasil switch.
            "long_term_action_share_percent" or "planning_horizon" or "planning_horizon_percent" => "players.support.formula.planning",
            // Untuk pola `”need_fulfillment_diversity_percent” or ”fulfillment_diversity”`, menghasilkan nilai literal `”players.support.formula.fulfillment”`
            // sebagai hasil switch.
            "need_fulfillment_diversity_percent" or "fulfillment_diversity" => "players.support.formula.fulfillment",
            // Untuk pola `”fulfillment_diversity_document_formula”`, menghasilkan nilai literal `”players.support.formula.fulfillment_document”` sebagai hasil
            // switch.
            "fulfillment_diversity_document_formula" => "players.support.formula.fulfillment_document",
            // Untuk pola `”growth_pattern_ratio”`, menghasilkan nilai literal `”players.support.formula.growth”` sebagai hasil switch.
            "growth_pattern_ratio" => "players.support.formula.growth",
            // Untuk pola `”donation_aggressiveness_percent”`, menghasilkan nilai literal `”players.support.formula.donation_aggressiveness”` sebagai hasil
            // switch.
            "donation_aggressiveness_percent" => "players.support.formula.donation_aggressiveness",
            // Untuk pola `”donation_stability” or ”donation_stability_std_deviation”`, menghasilkan nilai literal
            // `”players.support.formula.donation_stability”` sebagai hasil switch.
            "donation_stability" or "donation_stability_std_deviation" => "players.support.formula.donation_stability",
            // Untuk pola `”donation_stability_index”`, menghasilkan nilai literal `”players.support.formula.donation_stability_index”` sebagai hasil switch.
            "donation_stability_index" => "players.support.formula.donation_stability_index",
            // Untuk pola `”donation_commitment_score”`, menghasilkan nilai literal `”players.support.formula.donation_commitment”` sebagai hasil switch.
            "donation_commitment_score" => "players.support.formula.donation_commitment",
            // Untuk pola `_`, menghasilkan `string.Empty`, yaitu nilai kosong bawaan tipe terkait sebagai hasil switch.
            _ => string.Empty
        // Menutup scope pemetaan switch atas `metricKey`; bagian berikut berada di luar batas blok tersebut dalam ResolveFormulaKey.
        };
    // Menutup scope metode ResolveFormulaKey; bagian berikut berada di luar batas blok tersebut dalam ResolveFormulaKey.
    }

    // Mendefinisikan metode `FormatDisplayNumber` dengan hasil bertipe `string`; operasi ini menangani format display number. Masukan: Parameter
    // `value` bertipe `double` membawa nilai nilai.
    private static string FormatDisplayNumber(double value)
    // Membuka scope metode FormatDisplayNumber; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam FormatDisplayNumber.
    {
        // Mengembalikan hasil pemilihan bersyarat: ketika `Math.Abs(value - Math.Round(value)) < 0.0000001` benar gunakan `value.ToString(”N0”,
        // CultureInfo.CurrentCulture)`, jika tidak gunakan `value.ToString(”N2”, CultureInfo.CurrentCulture)` kepada pemanggil dalam FormatDisplayNumber;
        // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return Math.Abs(value - Math.Round(value)) < 0.0000001
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: value.ToString(”N0”, CultureInfo.CurrentCulture) dalam
            // FormatDisplayNumber.
            ? value.ToString("N0", CultureInfo.CurrentCulture)
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: value.ToString(”N2”, CultureInfo.CurrentCulture); dalam
            // FormatDisplayNumber.
            : value.ToString("N2", CultureInfo.CurrentCulture);
    // Menutup scope metode FormatDisplayNumber; bagian berikut berada di luar batas blok tersebut dalam FormatDisplayNumber.
    }

    // Mendefinisikan metode `NormalizeMetricLexiconKey` dengan hasil bertipe `string`; operasi ini menangani normalize metric lexicon kunci. Masukan:
    // Parameter `key` bertipe `string` membawa nilai kunci.
    private static string NormalizeMetricLexiconKey(string key)
    // Membuka scope metode NormalizeMetricLexiconKey; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam NormalizeMetricLexiconKey.
    {
        // Menyiapkan variabel lokal `trimmed` untuk nilai trimmed dengan membersihkan karakter tepi pada `key` memakai tanpa argumen. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var trimmed = key.Trim();
        // Menyiapkan variabel lokal `builder` untuk nilai pembentuk dengan objek baru bertipe `StringBuilder` dengan argumen (trimmed.Length). Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var builder = new StringBuilder(trimmed.Length);

        // Memulai loop dengan inisialisasi `var index = 0`, berjalan selama `index < trimmed.Length`, lalu memperbarui pencacah melalui `index++` dalam
        // NormalizeMetricLexiconKey.
        for (var index = 0; index < trimmed.Length; index++)
        // Membuka scope loop dengan syarat `index < trimmed.Length`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // NormalizeMetricLexiconKey.
        {
            // Menyiapkan variabel lokal `current` untuk nilai saat ini dengan `trimmed[index]`, yaitu elemen koleksi yang dipilih melalui indeks atau kunci
            // tersebut. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var current = trimmed[index];
            // Memeriksa memanggil `char.IsLetterOrDigit` dengan `current`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
            // NormalizeMetricLexiconKey.
            if (char.IsLetterOrDigit(current))
            // Membuka scope cabang if untuk kondisi `char.IsLetterOrDigit(current)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // NormalizeMetricLexiconKey.
            {
                // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `builder.Length > 0 && char.IsUpper(current)` dan
                // `ShouldInsertWordBoundary(trimmed, index)`; sisi kanan diperiksa hanya jika sisi kiri benar; blok if hanya dijalankan ketika kondisi ini bernilai
                // benar dalam NormalizeMetricLexiconKey.
                if (builder.Length > 0 &&
                    // Melanjutkan pengolahan dengan memanggil `char.IsUpper` dengan `current` dalam NormalizeMetricLexiconKey.
                    char.IsUpper(current) &&
                    // Melanjutkan pengolahan dengan memanggil `ShouldInsertWordBoundary` dengan `trimmed`, `index` dalam NormalizeMetricLexiconKey.
                    ShouldInsertWordBoundary(trimmed, index))
                // Membuka scope cabang if untuk kondisi `builder.Length > 0 && char.IsUpper(current) && ShouldInsertWordBoundary(trimmed, index)`;
                // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam NormalizeMetricLexiconKey.
                {
                    // Menjalankan memanggil `AppendSeparator` dengan `builder` dalam NormalizeMetricLexiconKey.
                    AppendSeparator(builder);
                // Menutup scope cabang if untuk kondisi `builder.Length > 0 && char.IsUpper(current) && ShouldInsertWordBoundary(trimmed, index)`; bagian berikut
                // berada di luar batas blok tersebut dalam NormalizeMetricLexiconKey.
                }

                // Menjalankan menggabungkan `builder` dengan `char.ToLowerInvariant(current)` pada urutan hasil dalam NormalizeMetricLexiconKey.
                builder.Append(char.ToLowerInvariant(current));
                // Melewati sisa pernyataan pada iterasi saat ini dan melanjutkan ke elemen/iterasi berikutnya dalam NormalizeMetricLexiconKey.
                continue;
            // Menutup scope cabang if untuk kondisi `char.IsLetterOrDigit(current)`; bagian berikut berada di luar batas blok tersebut dalam
            // NormalizeMetricLexiconKey.
            }

            // Menjalankan memanggil `AppendSeparator` dengan `builder` dalam NormalizeMetricLexiconKey.
            AppendSeparator(builder);
        // Menutup scope loop dengan syarat `index < trimmed.Length`; bagian berikut berada di luar batas blok tersebut dalam NormalizeMetricLexiconKey.
        }

        // Mengembalikan membersihkan karakter tepi pada `builder.ToString()` memakai `'_'` kepada pemanggil dalam NormalizeMetricLexiconKey; eksekusi jalur
        // ini selesai setelah nilai hasil ditentukan.
        return builder.ToString().Trim('_');
    // Menutup scope metode NormalizeMetricLexiconKey; bagian berikut berada di luar batas blok tersebut dalam NormalizeMetricLexiconKey.
    }

    // Mendefinisikan metode `ShouldInsertWordBoundary` dengan hasil bertipe `bool`; operasi ini menangani should insert word boundary. Masukan:
    // Parameter `value` bertipe `string` membawa nilai nilai; Parameter `index` bertipe `int` membawa nilai index.
    private static bool ShouldInsertWordBoundary(string value, int index)
    // Membuka scope metode ShouldInsertWordBoundary; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ShouldInsertWordBoundary.
    {
        // Memeriksa perbandingan kesamaan antara `index` dan `0`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // ShouldInsertWordBoundary.
        if (index == 0)
        // Membuka scope cabang if untuk kondisi `index == 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ShouldInsertWordBoundary.
        {
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam ShouldInsertWordBoundary; eksekusi jalur ini selesai setelah
            // nilai hasil ditentukan.
            return false;
        // Menutup scope cabang if untuk kondisi `index == 0`; bagian berikut berada di luar batas blok tersebut dalam ShouldInsertWordBoundary.
        }

        // Menyiapkan variabel lokal `previous` untuk nilai previous dengan `value[index - 1]`, yaitu elemen koleksi yang dipilih melalui indeks atau kunci
        // tersebut. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var previous = value[index - 1];
        // Memeriksa kebalikan kondisi `char.IsLetterOrDigit(previous)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // ShouldInsertWordBoundary.
        if (!char.IsLetterOrDigit(previous))
        // Membuka scope cabang if untuk kondisi `!char.IsLetterOrDigit(previous)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ShouldInsertWordBoundary.
        {
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam ShouldInsertWordBoundary; eksekusi jalur ini selesai setelah
            // nilai hasil ditentukan.
            return false;
        // Menutup scope cabang if untuk kondisi `!char.IsLetterOrDigit(previous)`; bagian berikut berada di luar batas blok tersebut dalam
        // ShouldInsertWordBoundary.
        }

        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `char.IsLower(previous)` dan `char.IsDigit(previous)`; sisi kanan
        // diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ShouldInsertWordBoundary.
        if (char.IsLower(previous) || char.IsDigit(previous))
        // Membuka scope cabang if untuk kondisi `char.IsLower(previous) || char.IsDigit(previous)`; pernyataan/deklarasi berikut berada di dalam batas blok
        // ini dalam ShouldInsertWordBoundary.
        {
            // Mengembalikan true, yaitu kondisi aktif/terpenuhi kepada pemanggil dalam ShouldInsertWordBoundary; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return true;
        // Menutup scope cabang if untuk kondisi `char.IsLower(previous) || char.IsDigit(previous)`; bagian berikut berada di luar batas blok tersebut dalam
        // ShouldInsertWordBoundary.
        }

        // Mengembalikan gabungan syarat AND: kedua kondisi wajib benar antara `index + 1 < value.Length` dan `char.IsLower(value[index + 1])`; sisi kanan
        // diperiksa hanya jika sisi kiri benar kepada pemanggil dalam ShouldInsertWordBoundary; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return index + 1 < value.Length && char.IsLower(value[index + 1]);
    // Menutup scope metode ShouldInsertWordBoundary; bagian berikut berada di luar batas blok tersebut dalam ShouldInsertWordBoundary.
    }

    // Mendefinisikan metode `AppendSeparator` dengan hasil bertipe `void`; operasi ini menangani append separator. Masukan: Parameter `builder` bertipe
    // `StringBuilder` membawa nilai pembentuk.
    private static void AppendSeparator(StringBuilder builder)
    // Membuka scope metode AppendSeparator; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam AppendSeparator.
    {
        // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `builder.Length > 0` dan `builder[^1] != '_'`; sisi kanan diperiksa hanya jika
        // sisi kiri benar; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam AppendSeparator.
        if (builder.Length > 0 && builder[^1] != '_')
        // Membuka scope cabang if untuk kondisi `builder.Length > 0 && builder[^1] != '_'`; pernyataan/deklarasi berikut berada di dalam batas blok ini
        // dalam AppendSeparator.
        {
            // Menjalankan menggabungkan `builder` dengan `'_'` pada urutan hasil dalam AppendSeparator.
            builder.Append('_');
        // Menutup scope cabang if untuk kondisi `builder.Length > 0 && builder[^1] != '_'`; bagian berikut berada di luar batas blok tersebut dalam
        // AppendSeparator.
        }
    // Menutup scope metode AppendSeparator; bagian berikut berada di luar batas blok tersebut dalam AppendSeparator.
    }
// Menutup scope tipe PlayerMetricLabelFormatter; bagian berikut berada di luar batas blok tersebut.
}
