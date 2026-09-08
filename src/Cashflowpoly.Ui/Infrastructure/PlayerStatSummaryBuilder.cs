// Fungsi file: Menyediakan transformasi, lokalisasi, atau koneksi UI melalui PlayerStatSummaryBuilder.
// Mengimpor namespace `System.Text.Json` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.Json;
// Mengimpor namespace `Cashflowpoly.Ui.Contracts` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Ui.Contracts;
// Mengimpor namespace `Cashflowpoly.Ui.Models` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Ui.Models;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Infrastructure` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Infrastructure;

/// <summary>
/// Builder ringkasan statistik pemain untuk membantu instruktur membaca performa inti.
/// </summary>
// Mendefinisikan tipe class `PlayerStatSummaryBuilder`.
public static class PlayerStatSummaryBuilder
// Membuka scope tipe PlayerStatSummaryBuilder; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan metode `Build` dengan hasil bertipe `PlayerStatSummaryViewModel`; operasi ini menangani build. Masukan: Parameter `gameplay`
    // bertipe `GameplayMetricsResponse?` membawa nilai gameplay; nilai null diizinkan ketika data opsional belum tersedia; Parameter `analyticsSummary`
    // bertipe `AnalyticsByPlayerItem?` membawa nilai analytics summary; nilai null diizinkan ketika data opsional belum tersedia; Parameter
    // `cashflowJourney` bertipe `PlayerCashflowJourneyStatsViewModel?` membawa nilai arus kas journey; nilai null diizinkan ketika data opsional belum
    // tersedia; Parameter `translate` bertipe `Func<string, string>` membawa nilai translate.
    public static PlayerStatSummaryViewModel Build(
        // Parameter `gameplay` bertipe `GameplayMetricsResponse?` membawa nilai gameplay; nilai null diizinkan ketika data opsional belum tersedia.
        GameplayMetricsResponse? gameplay,
        // Parameter `analyticsSummary` bertipe `AnalyticsByPlayerItem?` membawa nilai analytics summary; nilai null diizinkan ketika data opsional belum
        // tersedia.
        AnalyticsByPlayerItem? analyticsSummary,
        // Parameter `cashflowJourney` bertipe `PlayerCashflowJourneyStatsViewModel?` membawa nilai arus kas journey; nilai null diizinkan ketika data
        // opsional belum tersedia.
        PlayerCashflowJourneyStatsViewModel? cashflowJourney,
        // Parameter `translate` bertipe `Func<string, string>` membawa nilai translate.
        Func<string, string> translate)
    // Membuka scope metode Build; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Build.
    {
        // Menyiapkan variabel lokal `insights` untuk nilai insights dengan objek baru bertipe `List<PlayerInstructorInsightViewModel>` dengan nilai awal
        // sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var insights = new List<PlayerInstructorInsightViewModel>();

        // Menyiapkan variabel lokal `netCashflow` untuk nilai net arus kas dengan hasil pemilihan bersyarat: ketika `analyticsSummary is not null` benar
        // gunakan `analyticsSummary.CashInTotal - analyticsSummary.CashOutTotal`, jika tidak gunakan `cashflowJourney?.NetCashflow ??
        // gameplay?.Economy.CashflowNetTotal`. Tipe yang dipakai adalah `double?`.
        double? netCashflow = analyticsSummary is not null
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: analyticsSummary.CashInTotal - analyticsSummary.CashOutTotal dalam
            // Build.
            ? analyticsSummary.CashInTotal - analyticsSummary.CashOutTotal
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: cashflowJourney?.NetCashflow ?? gameplay?.Economy.CashflowNetTotal;
            // dalam Build.
            : cashflowJourney?.NetCashflow ?? gameplay?.Economy.CashflowNetTotal;
        // Menyiapkan variabel lokal `happiness` untuk nilai kebahagiaan dengan `analyticsSummary?.HappinessPointsTotal` bila tidak null; jika null gunakan
        // `gameplay?.Score.HappinessPointsTotal` sebagai nilai pengganti. Tipe yang dipakai adalah `double?`.
        double? happiness = analyticsSummary?.HappinessPointsTotal ?? gameplay?.Score.HappinessPointsTotal;
        // Menyiapkan variabel lokal `fulfillmentDiversity` untuk tingkat keberagaman kategori kebutuhan yang telah dipenuhi dengan
        // `analyticsSummary?.FulfillmentDiversity` bila tidak null; jika null gunakan `gameplay?.Needs.FulfillmentDiversity` sebagai nilai pengganti. Tipe
        // yang dipakai adalah `double?`.
        double? fulfillmentDiversity = analyticsSummary?.FulfillmentDiversity ?? gameplay?.Needs.FulfillmentDiversity;
        // Menyiapkan variabel lokal `needCardsOwned` untuk nilai kebutuhan kartu dimiliki dengan memanggil `ReadNeedCardsOwned` dengan `gameplay?.RawJson`.
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var needCardsOwned = ReadNeedCardsOwned(gameplay?.RawJson);
        // Menyiapkan variabel lokal `hasUnpaidLoan` untuk nilai memiliki unpaid pinjaman dengan `analyticsSummary?.HasUnpaidLoan` bila tidak null; jika
        // null gunakan `gameplay?.Score.HasUnpaidLoan` sebagai nilai pengganti. Tipe yang dipakai adalah `bool?`.
        bool? hasUnpaidLoan = analyticsSummary?.HasUnpaidLoan ?? gameplay?.Score.HasUnpaidLoan;

        // Memeriksa hasil pencocokan `netCashflow` dengan pola `< 0`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam Build.
        if (netCashflow is < 0)
        // Membuka scope cabang if untuk kondisi `netCashflow is < 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Build.
        {
            // Menjalankan menambahkan `Insight( ”cashflow_negative”, translate(”players.stats.insight.cashflow_negative.title”),
            // translate(”players.stats.insight.cashflow_negative.desc”), ”warning”)` ke `insights` dalam Build.
            insights.Add(Insight(
                // Meneruskan nilai literal `”cashflow_negative”` sebagai argumen ke `Insight`.
                "cashflow_negative",
                // Meneruskan memanggil `translate` dengan `”players.stats.insight.cashflow_negative.title”` sebagai argumen ke `Insight`; Meneruskan nilai literal
                // `”players.stats.insight.cashflow_negative.title”` sebagai argumen ke `translate`.
                translate("players.stats.insight.cashflow_negative.title"),
                // Meneruskan memanggil `translate` dengan `”players.stats.insight.cashflow_negative.desc”` sebagai argumen ke `Insight`; Meneruskan nilai literal
                // `”players.stats.insight.cashflow_negative.desc”` sebagai argumen ke `translate`.
                translate("players.stats.insight.cashflow_negative.desc"),
                // Meneruskan nilai literal `”warning”` sebagai argumen ke `Insight`.
                "warning"));
        // Menutup scope cabang if untuk kondisi `netCashflow is < 0`; bagian berikut berada di luar batas blok tersebut dalam Build.
        }

        // Memeriksa perbandingan kesamaan antara `hasUnpaidLoan` dan `true`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam Build.
        if (hasUnpaidLoan == true)
        // Membuka scope cabang if untuk kondisi `hasUnpaidLoan == true`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Build.
        {
            // Menjalankan menambahkan `Insight( ”loan_unpaid”, translate(”players.stats.insight.loan_unpaid.title”),
            // translate(”players.stats.insight.loan_unpaid.desc”), ”danger”)` ke `insights` dalam Build.
            insights.Add(Insight(
                // Meneruskan nilai literal `”loan_unpaid”` sebagai argumen ke `Insight`.
                "loan_unpaid",
                // Meneruskan memanggil `translate` dengan `”players.stats.insight.loan_unpaid.title”` sebagai argumen ke `Insight`; Meneruskan nilai literal
                // `”players.stats.insight.loan_unpaid.title”` sebagai argumen ke `translate`.
                translate("players.stats.insight.loan_unpaid.title"),
                // Meneruskan memanggil `translate` dengan `”players.stats.insight.loan_unpaid.desc”` sebagai argumen ke `Insight`; Meneruskan nilai literal
                // `”players.stats.insight.loan_unpaid.desc”` sebagai argumen ke `translate`.
                translate("players.stats.insight.loan_unpaid.desc"),
                // Meneruskan nilai literal `”danger”` sebagai argumen ke `Insight`.
                "danger"));
        // Menutup scope cabang if untuk kondisi `hasUnpaidLoan == true`; bagian berikut berada di luar batas blok tersebut dalam Build.
        }

        // Memeriksa hasil pencocokan `happiness` dengan pola `< 20`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam Build.
        if (happiness is < 20)
        // Membuka scope cabang if untuk kondisi `happiness is < 20`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Build.
        {
            // Menjalankan menambahkan `Insight( ”happiness_low”, translate(”players.stats.insight.happiness_low.title”),
            // translate(”players.stats.insight.happiness_low.desc”), ”warning”)` ke `insights` dalam Build.
            insights.Add(Insight(
                // Meneruskan nilai literal `”happiness_low”` sebagai argumen ke `Insight`.
                "happiness_low",
                // Meneruskan memanggil `translate` dengan `”players.stats.insight.happiness_low.title”` sebagai argumen ke `Insight`; Meneruskan nilai literal
                // `”players.stats.insight.happiness_low.title”` sebagai argumen ke `translate`.
                translate("players.stats.insight.happiness_low.title"),
                // Meneruskan memanggil `translate` dengan `”players.stats.insight.happiness_low.desc”` sebagai argumen ke `Insight`; Meneruskan nilai literal
                // `”players.stats.insight.happiness_low.desc”` sebagai argumen ke `translate`.
                translate("players.stats.insight.happiness_low.desc"),
                // Meneruskan nilai literal `”warning”` sebagai argumen ke `Insight`.
                "warning"));
        // Menutup scope cabang if untuk kondisi `happiness is < 20`; bagian berikut berada di luar batas blok tersebut dalam Build.
        }

        // Memeriksa perbandingan kesamaan antara `needCardsOwned` dan `0`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam Build.
        if (needCardsOwned == 0)
        // Membuka scope cabang if untuk kondisi `needCardsOwned == 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Build.
        {
            // Menjalankan menambahkan `Insight( ”need_cards_missing”, translate(”players.stats.insight.need_cards_missing.title”),
            // translate(”players.stats.insight.need_cards_missing.desc”), ”warning”)` ke `insights` dalam Build.
            insights.Add(Insight(
                // Meneruskan nilai literal `”need_cards_missing”` sebagai argumen ke `Insight`.
                "need_cards_missing",
                // Meneruskan memanggil `translate` dengan `”players.stats.insight.need_cards_missing.title”` sebagai argumen ke `Insight`; Meneruskan nilai literal
                // `”players.stats.insight.need_cards_missing.title”` sebagai argumen ke `translate`.
                translate("players.stats.insight.need_cards_missing.title"),
                // Meneruskan memanggil `translate` dengan `”players.stats.insight.need_cards_missing.desc”` sebagai argumen ke `Insight`; Meneruskan nilai literal
                // `”players.stats.insight.need_cards_missing.desc”` sebagai argumen ke `translate`.
                translate("players.stats.insight.need_cards_missing.desc"),
                // Meneruskan nilai literal `”warning”` sebagai argumen ke `Insight`.
                "warning"));
        // Menutup scope cabang if untuk kondisi `needCardsOwned == 0`; bagian berikut berada di luar batas blok tersebut dalam Build.
        }
        // Menjalankan cabang alternatif ketika kondisi if sebelumnya tidak terpenuhi dalam Build.
        else if (fulfillmentDiversity is < 0.4)
        // Membuka scope cabang if untuk kondisi `fulfillmentDiversity is < 0.4`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Build.
        {
            // Menjalankan menambahkan `Insight( ”need_diversity_low”, translate(”players.stats.insight.need_diversity_low.title”),
            // translate(”players.stats.insight.need_diversity_low.desc”), ”warning”)` ke `insights` dalam Build.
            insights.Add(Insight(
                // Meneruskan nilai literal `”need_diversity_low”` sebagai argumen ke `Insight`.
                "need_diversity_low",
                // Meneruskan memanggil `translate` dengan `”players.stats.insight.need_diversity_low.title”` sebagai argumen ke `Insight`; Meneruskan nilai literal
                // `”players.stats.insight.need_diversity_low.title”` sebagai argumen ke `translate`.
                translate("players.stats.insight.need_diversity_low.title"),
                // Meneruskan memanggil `translate` dengan `”players.stats.insight.need_diversity_low.desc”` sebagai argumen ke `Insight`; Meneruskan nilai literal
                // `”players.stats.insight.need_diversity_low.desc”` sebagai argumen ke `translate`.
                translate("players.stats.insight.need_diversity_low.desc"),
                // Meneruskan nilai literal `”warning”` sebagai argumen ke `Insight`.
                "warning"));
        // Menutup scope cabang if untuk kondisi `fulfillmentDiversity is < 0.4`; bagian berikut berada di luar batas blok tersebut dalam Build.
        }

        // Menyiapkan variabel lokal `hasCompleteEvaluationData` untuk nilai memiliki complete evaluation data dengan gabungan syarat AND: kedua kondisi
        // wajib benar antara `netCashflow.HasValue && happiness.HasValue && fulfillmentDiversity.HasValue` dan `hasUnpaidLoan.HasValue`; sisi kanan
        // diperiksa hanya jika sisi kiri benar. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var hasCompleteEvaluationData =
            // Melanjutkan ekspresi dengan gabungan syarat AND: kedua kondisi wajib benar antara `netCashflow.HasValue` dan `happiness.HasValue`; sisi kanan
            // diperiksa hanya jika sisi kiri benar dalam Build.
            netCashflow.HasValue &&
            // Menggunakan `happiness` (nilai kebahagiaan) sebagai bagian ekspresi yang sedang disusun dalam Build.
            happiness.HasValue &&
            // Menggunakan `fulfillmentDiversity` (tingkat keberagaman kategori kebutuhan yang telah dipenuhi) sebagai bagian ekspresi yang sedang disusun dalam
            // Build.
            fulfillmentDiversity.HasValue &&
            // Menggunakan `hasUnpaidLoan` (nilai memiliki unpaid pinjaman) sebagai bagian ekspresi yang sedang disusun dalam Build.
            hasUnpaidLoan.HasValue;

        // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `insights.Count == 0` dan `!hasCompleteEvaluationData`; sisi kanan diperiksa
        // hanya jika sisi kiri benar; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam Build.
        if (insights.Count == 0 && !hasCompleteEvaluationData)
        // Membuka scope cabang if untuk kondisi `insights.Count == 0 && !hasCompleteEvaluationData`; pernyataan/deklarasi berikut berada di dalam batas
        // blok ini dalam Build.
        {
            // Menjalankan menambahkan `Insight( ”data_unavailable”, translate(”players.stats.insight.data_unavailable.title”),
            // translate(”players.stats.insight.data_unavailable.desc”), ”neutral”)` ke `insights` dalam Build.
            insights.Add(Insight(
                // Meneruskan nilai literal `”data_unavailable”` sebagai argumen ke `Insight`.
                "data_unavailable",
                // Meneruskan memanggil `translate` dengan `”players.stats.insight.data_unavailable.title”` sebagai argumen ke `Insight`; Meneruskan nilai literal
                // `”players.stats.insight.data_unavailable.title”` sebagai argumen ke `translate`.
                translate("players.stats.insight.data_unavailable.title"),
                // Meneruskan memanggil `translate` dengan `”players.stats.insight.data_unavailable.desc”` sebagai argumen ke `Insight`; Meneruskan nilai literal
                // `”players.stats.insight.data_unavailable.desc”` sebagai argumen ke `translate`.
                translate("players.stats.insight.data_unavailable.desc"),
                // Meneruskan nilai literal `”neutral”` sebagai argumen ke `Insight`.
                "neutral"));
        // Menutup scope cabang if untuk kondisi `insights.Count == 0 && !hasCompleteEvaluationData`; bagian berikut berada di luar batas blok tersebut
        // dalam Build.
        }
        // Menjalankan cabang alternatif ketika kondisi if sebelumnya tidak terpenuhi dalam Build.
        else if (insights.Count == 0)
        // Membuka scope cabang if untuk kondisi `insights.Count == 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Build.
        {
            // Menjalankan menambahkan `Insight( ”stable_profile”, translate(”players.stats.insight.stable_profile.title”),
            // translate(”players.stats.insight.stable_profile.desc”), ”neutral”)` ke `insights` dalam Build.
            insights.Add(Insight(
                // Meneruskan nilai literal `”stable_profile”` sebagai argumen ke `Insight`.
                "stable_profile",
                // Meneruskan memanggil `translate` dengan `”players.stats.insight.stable_profile.title”` sebagai argumen ke `Insight`; Meneruskan nilai literal
                // `”players.stats.insight.stable_profile.title”` sebagai argumen ke `translate`.
                translate("players.stats.insight.stable_profile.title"),
                // Meneruskan memanggil `translate` dengan `”players.stats.insight.stable_profile.desc”` sebagai argumen ke `Insight`; Meneruskan nilai literal
                // `”players.stats.insight.stable_profile.desc”` sebagai argumen ke `translate`.
                translate("players.stats.insight.stable_profile.desc"),
                // Meneruskan nilai literal `”neutral”` sebagai argumen ke `Insight`.
                "neutral"));
        // Menutup scope cabang if untuk kondisi `insights.Count == 0`; bagian berikut berada di luar batas blok tersebut dalam Build.
        }

        // Mengembalikan objek baru bertipe `PlayerStatSummaryViewModel` dengan nilai awal sesuai konstruktornya kepada pemanggil dalam Build; eksekusi
        // jalur ini selesai setelah nilai hasil ditentukan.
        return new PlayerStatSummaryViewModel
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Build.
        {
            // Memperbarui `CollectionMissionComplete` menggunakan memanggil `ReadCollectionMissionComplete` dengan `gameplay?.RawJson` dalam Build.
            CollectionMissionComplete = ReadCollectionMissionComplete(gameplay?.RawJson),
            // Memperbarui `Insights` menggunakan mematerialisasi urutan `insights .OrderBy(item => item.Tone == ”danger” ? 0 : item.Tone == ”warning” ? 1 :
            // item.Tone == ”neutral” ? 2 : 3) .Take(3)` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori dalam Build.
            Insights = insights
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .OrderBy(item => item.Tone == ”danger” ? 0 : item.Tone == ”warning” ? 1 :
                // item.Tone == ”neutral” ? 2 : 3) dalam Build; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
                .OrderBy(item => item.Tone == "danger" ? 0 : item.Tone == "warning" ? 1 : item.Tone == "neutral" ? 2 : 3)
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Take(3) dalam Build; token pada baris ini menyambungkan bagian kode sebelum
                // dan sesudahnya.
                .Take(3)
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList() dalam Build; token pada baris ini menyambungkan bagian kode sebelum
                // dan sesudahnya.
                .ToList()
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam Build.
        };
    // Menutup scope metode Build; bagian berikut berada di luar batas blok tersebut dalam Build.
    }

    // Mendefinisikan metode `ReadCollectionMissionComplete` dengan hasil bertipe `bool?`; operasi ini menangani read collection misi complete. Masukan:
    // Parameter `rawJson` bertipe `JsonElement?` membawa nilai raw JSON; nilai null diizinkan ketika data opsional belum tersedia.
    private static bool? ReadCollectionMissionComplete(JsonElement? rawJson)
    // Membuka scope metode ReadCollectionMissionComplete; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // ReadCollectionMissionComplete.
    {
        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `rawJson is not { ValueKind: JsonValueKind.Object } raw ||
        // !raw.TryGetProperty(”needs”, out var needs) || needs.ValueKind != JsonValueKind.Object || !needs.TryGetProperty(”coll...` dan `complete.ValueKind
        // is not (JsonValueKind.True or JsonValueKind.False)`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini
        // bernilai benar dalam ReadCollectionMissionComplete.
        if (rawJson is not { ValueKind: JsonValueKind.Object } raw ||
            // Menggunakan kebalikan kondisi `raw.TryGetProperty(”needs”, out var needs)` sebagai bagian ekspresi yang sedang disusun dalam
            // ReadCollectionMissionComplete.
            !raw.TryGetProperty("needs", out var needs) ||
            // Melanjutkan ekspresi dengan perbandingan ketidaksamaan antara `needs.ValueKind` dan `JsonValueKind.Object` dalam ReadCollectionMissionComplete.
            needs.ValueKind != JsonValueKind.Object ||
            // Menggunakan kebalikan kondisi `needs.TryGetProperty(”collection_mission_complete”, out var complete)` sebagai bagian ekspresi yang sedang disusun
            // dalam ReadCollectionMissionComplete.
            !needs.TryGetProperty("collection_mission_complete", out var complete) ||
            // Menggunakan `complete` (nilai complete) sebagai bagian ekspresi yang sedang disusun dalam ReadCollectionMissionComplete.
            complete.ValueKind is not (JsonValueKind.True or JsonValueKind.False))
        // Membuka scope cabang if untuk kondisi `rawJson is not { ValueKind: JsonValueKind.Object } raw || !raw.TryGetProperty(”needs”, out var needs) ||
        // needs.ValueKind != JsonValueKind.Object || !needs.TryGetProperty(”coll...`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ReadCollectionMissionComplete.
        {
            // Mengembalikan null, yaitu penanda tidak ada nilai kepada pemanggil dalam ReadCollectionMissionComplete; eksekusi jalur ini selesai setelah nilai
            // hasil ditentukan.
            return null;
        // Menutup scope cabang if untuk kondisi `rawJson is not { ValueKind: JsonValueKind.Object } raw || !raw.TryGetProperty(”needs”, out var needs) ||
        // needs.ValueKind != JsonValueKind.Object || !needs.TryGetProperty(”coll...`; bagian berikut berada di luar batas blok tersebut dalam
        // ReadCollectionMissionComplete.
        }

        // Mengembalikan memanggil `complete.GetBoolean` dengan tanpa argumen kepada pemanggil dalam ReadCollectionMissionComplete; eksekusi jalur ini
        // selesai setelah nilai hasil ditentukan.
        return complete.GetBoolean();
    // Menutup scope metode ReadCollectionMissionComplete; bagian berikut berada di luar batas blok tersebut dalam ReadCollectionMissionComplete.
    }

    // Mendefinisikan metode `ReadNeedCardsOwned` dengan hasil bertipe `int?`; operasi ini menangani read kebutuhan kartu dimiliki. Masukan: Parameter
    // `rawJson` bertipe `JsonElement?` membawa nilai raw JSON; nilai null diizinkan ketika data opsional belum tersedia.
    private static int? ReadNeedCardsOwned(JsonElement? rawJson)
    // Membuka scope metode ReadNeedCardsOwned; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ReadNeedCardsOwned.
    {
        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!rawJson.HasValue || rawJson.Value.ValueKind != JsonValueKind.Object ||
        // !rawJson.Value.TryGetProperty(”needs”, out var needs) || needs.ValueKind != JsonValueKind.Object || !ne...` dan `!owned.TryGetInt32(out var
        // count)`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ReadNeedCardsOwned.
        if (!rawJson.HasValue || rawJson.Value.ValueKind != JsonValueKind.Object ||
            // Menggunakan kebalikan kondisi `rawJson.Value.TryGetProperty(”needs”, out var needs)` sebagai bagian ekspresi yang sedang disusun dalam
            // ReadNeedCardsOwned.
            !rawJson.Value.TryGetProperty("needs", out var needs) ||
            // Melanjutkan ekspresi dengan perbandingan ketidaksamaan antara `needs.ValueKind` dan `JsonValueKind.Object` dalam ReadNeedCardsOwned.
            needs.ValueKind != JsonValueKind.Object ||
            // Menggunakan kebalikan kondisi `needs.TryGetProperty(”need_cards_owned_current”, out var owned)` sebagai bagian ekspresi yang sedang disusun dalam
            // ReadNeedCardsOwned.
            !needs.TryGetProperty("need_cards_owned_current", out var owned) ||
            // Menggunakan kebalikan kondisi `owned.TryGetInt32(out var count)` sebagai bagian ekspresi yang sedang disusun dalam ReadNeedCardsOwned.
            !owned.TryGetInt32(out var count))
        // Membuka scope cabang if untuk kondisi `!rawJson.HasValue || rawJson.Value.ValueKind != JsonValueKind.Object ||
        // !rawJson.Value.TryGetProperty(”needs”, out var needs) || needs.ValueKind != JsonValueKind.Object || !ne...`; pernyataan/deklarasi berikut berada
        // di dalam batas blok ini dalam ReadNeedCardsOwned.
        {
            // Mengembalikan null, yaitu penanda tidak ada nilai kepada pemanggil dalam ReadNeedCardsOwned; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return null;
        // Menutup scope cabang if untuk kondisi `!rawJson.HasValue || rawJson.Value.ValueKind != JsonValueKind.Object ||
        // !rawJson.Value.TryGetProperty(”needs”, out var needs) || needs.ValueKind != JsonValueKind.Object || !ne...`; bagian berikut berada di luar batas
        // blok tersebut dalam ReadNeedCardsOwned.
        }

        // Mengembalikan `count` (nilai jumlah) kepada pemanggil dalam ReadNeedCardsOwned; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return count;
    // Menutup scope metode ReadNeedCardsOwned; bagian berikut berada di luar batas blok tersebut dalam ReadNeedCardsOwned.
    }

    // Mendefinisikan metode `Insight` dengan hasil bertipe `PlayerInstructorInsightViewModel`; operasi ini menangani insight. Masukan: Parameter `key`
    // bertipe `string` membawa nilai kunci; Parameter `title` bertipe `string` membawa nilai title; Parameter `description` bertipe `string` membawa
    // nilai description; Parameter `tone` bertipe `string` membawa nilai tone.
    private static PlayerInstructorInsightViewModel Insight(string key, string title, string description, string tone)
    // Membuka scope metode Insight; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Insight.
    {
        // Mengembalikan objek baru bertipe `PlayerInstructorInsightViewModel` dengan nilai awal sesuai konstruktornya kepada pemanggil dalam Insight;
        // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return new PlayerInstructorInsightViewModel
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Insight.
        {
            // Memperbarui `Key` menggunakan `key` (nilai kunci) dalam Insight.
            Key = key,
            // Memperbarui `Title` menggunakan `title` (nilai title) dalam Insight.
            Title = title,
            // Memperbarui `Description` menggunakan `description` (nilai description) dalam Insight.
            Description = description,
            // Memperbarui `Tone` menggunakan `tone` (nilai tone) dalam Insight.
            Tone = tone
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam Insight.
        };
    // Menutup scope metode Insight; bagian berikut berada di luar batas blok tersebut dalam Insight.
    }

// Menutup scope tipe PlayerStatSummaryBuilder; bagian berikut berada di luar batas blok tersebut.
}
