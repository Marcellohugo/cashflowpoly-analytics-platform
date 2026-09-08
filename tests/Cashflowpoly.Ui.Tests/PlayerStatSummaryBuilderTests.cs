// Fungsi file: Memverifikasi perilaku, lokalisasi, atau tata letak UI melalui PlayerStatSummaryBuilderTests.
// Mengimpor namespace `System.Text.Json` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.Json;
// Mengimpor namespace `Cashflowpoly.Ui.Contracts` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Ui.Contracts;
// Mengimpor namespace `Cashflowpoly.Ui.Infrastructure` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Ui.Infrastructure;
// Mengimpor namespace `Cashflowpoly.Ui.Models` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Ui.Models;
// Mengimpor namespace `Xunit` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Xunit;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Tests` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Tests;

// Mendefinisikan tipe class `PlayerStatSummaryBuilderTests`; sealed mencegah tipe ini diturunkan lagi.
public sealed class PlayerStatSummaryBuilderTests
// Membuka scope tipe PlayerStatSummaryBuilderTests; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // menandai metode sebagai pengujian xUnit yang dijalankan untuk setiap kombinasi data.
    [Theory]
    // menyediakan satu kombinasi masukan pengujian (”{\”needs\”:{\”collection_mission_complete\”:true}}”, true).
    [InlineData("{\"needs\":{\"collection_mission_complete\":true}}", true)]
    // menyediakan satu kombinasi masukan pengujian (”{\”needs\”:{\”collection_mission_complete\”:false}}”, false).
    [InlineData("{\"needs\":{\"collection_mission_complete\":false}}", false)]
    // menyediakan satu kombinasi masukan pengujian (”{\”needs\”:{\”collection_mission_complete\”:null}}”, null).
    [InlineData("{\"needs\":{\"collection_mission_complete\":null}}", null)]
    // menyediakan satu kombinasi masukan pengujian (”{\”needs\”:{\”collection_mission_complete\”:\”true\”}}”, null).
    [InlineData("{\"needs\":{\"collection_mission_complete\":\"true\"}}", null)]
    // menyediakan satu kombinasi masukan pengujian (”{\”needs\”:{}}”, null).
    [InlineData("{\"needs\":{}}", null)]
    // menyediakan satu kombinasi masukan pengujian (”{\”needs\”:null}”, null).
    [InlineData("{\"needs\":null}", null)]
    // menyediakan satu kombinasi masukan pengujian (”{}”, null).
    [InlineData("{}", null)]
    // menyediakan satu kombinasi masukan pengujian (”null”, null).
    [InlineData("null", null)]
    // Mendefinisikan metode `Build_UsesExplicitMissionResultWithoutTreatingMissingDataAsIncomplete` dengan hasil bertipe `void`; operasi ini menangani
    // build uses explicit misi hasil tanpa treating missing data as incomplete. Masukan: Parameter `rawJson` bertipe `string` membawa nilai raw JSON;
    // Parameter `expected` bertipe `bool?` membawa nilai yang diharapkan; nilai null diizinkan ketika data opsional belum tersedia.
    public void Build_UsesExplicitMissionResultWithoutTreatingMissingDataAsIncomplete(string rawJson, bool? expected)
    // Membuka scope metode Build_UsesExplicitMissionResultWithoutTreatingMissingDataAsIncomplete; pernyataan/deklarasi berikut berada di dalam batas
    // blok ini dalam Build_UsesExplicitMissionResultWithoutTreatingMissingDataAsIncomplete.
    {
        // Menyiapkan variabel lokal `gameplay` untuk nilai gameplay dengan `BuildGameplay(8, 30, 0.9, false) with { RawJson =
        // JsonSerializer.Deserialize<JsonElement>(rawJson) }`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var gameplay = BuildGameplay(8, 30, 0.9, false) with
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // Build_UsesExplicitMissionResultWithoutTreatingMissingDataAsIncomplete.
        {
            // Memperbarui `RawJson` menggunakan membaca `rawJson` menjadi objek bertipe sesuai kontrak JSON melalui `JsonSerializer.Deserialize<JsonElement>`
            // dalam Build_UsesExplicitMissionResultWithoutTreatingMissingDataAsIncomplete.
            RawJson = JsonSerializer.Deserialize<JsonElement>(rawJson)
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // Build_UsesExplicitMissionResultWithoutTreatingMissingDataAsIncomplete.
        };

        // Menyiapkan variabel lokal `summary` untuk nilai summary dengan memanggil `PlayerStatSummaryBuilder.Build` dengan `gameplay`, `null`, `null`,
        // `Translate`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var summary = PlayerStatSummaryBuilder.Build(gameplay, null, null, Translate);

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`expected`,
        // `summary.CollectionMissionComplete`); pengujian gagal jika keduanya berbeda dalam
        // Build_UsesExplicitMissionResultWithoutTreatingMissingDataAsIncomplete.
        Assert.Equal(expected, summary.CollectionMissionComplete);
    // Menutup scope metode Build_UsesExplicitMissionResultWithoutTreatingMissingDataAsIncomplete; bagian berikut berada di luar batas blok tersebut
    // dalam Build_UsesExplicitMissionResultWithoutTreatingMissingDataAsIncomplete.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `Build_MissingGameplayHasUnknownMissionResult` dengan hasil bertipe `void`; operasi ini menangani build missing gameplay
    // memiliki unknown misi hasil.
    public void Build_MissingGameplayHasUnknownMissionResult()
    // Membuka scope metode Build_MissingGameplayHasUnknownMissionResult; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // Build_MissingGameplayHasUnknownMissionResult.
    {
        // Menyiapkan variabel lokal `summary` untuk nilai summary dengan memanggil `PlayerStatSummaryBuilder.Build` dengan `null`, `null`, `null`,
        // `Translate`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var summary = PlayerStatSummaryBuilder.Build(null, null, null, Translate);

        // Menjalankan pemeriksaan Null atas `summary.CollectionMissionComplete` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // Build_MissingGameplayHasUnknownMissionResult.
        Assert.Null(summary.CollectionMissionComplete);
        // Menjalankan pemeriksaan hasil dengan `Assert.Single` menggunakan `summary.Insights`; ketidaksesuaian dengan ekspektasi membuat pengujian gagal
        // dalam Build_MissingGameplayHasUnknownMissionResult.
        Assert.Single(summary.Insights);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”data_unavailable”`,
        // `summary.Insights[0].Key`); pengujian gagal jika keduanya berbeda dalam Build_MissingGameplayHasUnknownMissionResult.
        Assert.Equal("data_unavailable", summary.Insights[0].Key);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `summary.Insights`, `item =>
        // item.Key is ”cashflow_negative” or ”loan_unpaid” or ”happiness_low” or ”need_diversity_low”` dalam Build_MissingGameplayHasUnknownMissionResult.
        Assert.DoesNotContain(summary.Insights, item => item.Key is "cashflow_negative" or "loan_unpaid" or "happiness_low" or "need_diversity_low");
    // Menutup scope metode Build_MissingGameplayHasUnknownMissionResult; bagian berikut berada di luar batas blok tersebut dalam
    // Build_MissingGameplayHasUnknownMissionResult.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `Build_AddsRiskInsightWhenCashflowIsNegative` dengan hasil bertipe `void`; operasi ini menangani build adds risiko insight
    // when arus kas berstatus negative.
    public void Build_AddsRiskInsightWhenCashflowIsNegative()
    // Membuka scope metode Build_AddsRiskInsightWhenCashflowIsNegative; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // Build_AddsRiskInsightWhenCashflowIsNegative.
    {
        // Menyiapkan variabel lokal `summary` untuk nilai summary dengan memanggil `PlayerStatSummaryBuilder.Build` dengan `BuildGameplay(cashflowNetTotal:
        // -12, happinessPointsTotal: 30, fulfillmentDiversity: 0.9, hasUnpaidLoan: false)`, `null`, `null`, `Translate`. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var summary = PlayerStatSummaryBuilder.Build(
            // Meneruskan memanggil `BuildGameplay` dengan `-12`, `30`, `0.9`, `false` sebagai argumen ke `PlayerStatSummaryBuilder.Build`; Meneruskan `-12`
            // sebagai argumen bernama `cashflowNetTotal`; Meneruskan nilai literal `30` sebagai argumen bernama `happinessPointsTotal`; Meneruskan nilai
            // literal `0.9` sebagai argumen bernama `fulfillmentDiversity`; Meneruskan false, yaitu kondisi nonaktif/tidak terpenuhi sebagai argumen bernama
            // `hasUnpaidLoan`.
            BuildGameplay(cashflowNetTotal: -12, happinessPointsTotal: 30, fulfillmentDiversity: 0.9, hasUnpaidLoan: false),
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen ke `PlayerStatSummaryBuilder.Build`.
            null,
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen ke `PlayerStatSummaryBuilder.Build`.
            null,
            // Meneruskan `Translate` (nilai translate) sebagai argumen ke `PlayerStatSummaryBuilder.Build`.
            Translate);

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `summary.Insights`, `item => item.Key ==
        // ”cashflow_negative” && item.Tone == ”warning”` dalam Build_AddsRiskInsightWhenCashflowIsNegative.
        Assert.Contains(summary.Insights, item => item.Key == "cashflow_negative" && item.Tone == "warning");
    // Menutup scope metode Build_AddsRiskInsightWhenCashflowIsNegative; bagian berikut berada di luar batas blok tersebut dalam
    // Build_AddsRiskInsightWhenCashflowIsNegative.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `Build_AddsRiskInsightWhenLoanIsUnpaid` dengan hasil bertipe `void`; operasi ini menangani build adds risiko insight when
    // pinjaman berstatus unpaid.
    public void Build_AddsRiskInsightWhenLoanIsUnpaid()
    // Membuka scope metode Build_AddsRiskInsightWhenLoanIsUnpaid; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // Build_AddsRiskInsightWhenLoanIsUnpaid.
    {
        // Menyiapkan variabel lokal `summary` untuk nilai summary dengan memanggil `PlayerStatSummaryBuilder.Build` dengan `BuildGameplay(cashflowNetTotal:
        // 8, happinessPointsTotal: 30, fulfillmentDiversity: 0.9, hasUnpaidLoan: true)`, `null`, `null`, `Translate`. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var summary = PlayerStatSummaryBuilder.Build(
            // Meneruskan memanggil `BuildGameplay` dengan `8`, `30`, `0.9`, `true` sebagai argumen ke `PlayerStatSummaryBuilder.Build`; Meneruskan nilai
            // literal `8` sebagai argumen bernama `cashflowNetTotal`; Meneruskan nilai literal `30` sebagai argumen bernama `happinessPointsTotal`; Meneruskan
            // nilai literal `0.9` sebagai argumen bernama `fulfillmentDiversity`; Meneruskan true, yaitu kondisi aktif/terpenuhi sebagai argumen bernama
            // `hasUnpaidLoan`.
            BuildGameplay(cashflowNetTotal: 8, happinessPointsTotal: 30, fulfillmentDiversity: 0.9, hasUnpaidLoan: true),
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen ke `PlayerStatSummaryBuilder.Build`.
            null,
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen ke `PlayerStatSummaryBuilder.Build`.
            null,
            // Meneruskan `Translate` (nilai translate) sebagai argumen ke `PlayerStatSummaryBuilder.Build`.
            Translate);

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `summary.Insights`, `item => item.Key ==
        // ”loan_unpaid” && item.Tone == ”danger”` dalam Build_AddsRiskInsightWhenLoanIsUnpaid.
        Assert.Contains(summary.Insights, item => item.Key == "loan_unpaid" && item.Tone == "danger");
    // Menutup scope metode Build_AddsRiskInsightWhenLoanIsUnpaid; bagian berikut berada di luar batas blok tersebut dalam
    // Build_AddsRiskInsightWhenLoanIsUnpaid.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `Build_DoesNotRepeatPositivePillarsAsDiscussionPriorities` dengan hasil bertipe `void`; operasi ini menangani build does
    // not repeat positive pillars as discussion priorities.
    public void Build_DoesNotRepeatPositivePillarsAsDiscussionPriorities()
    // Membuka scope metode Build_DoesNotRepeatPositivePillarsAsDiscussionPriorities; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // Build_DoesNotRepeatPositivePillarsAsDiscussionPriorities.
    {
        // Menyiapkan variabel lokal `summary` untuk nilai summary dengan memanggil `PlayerStatSummaryBuilder.Build` dengan `BuildGameplay(cashflowNetTotal:
        // 8, happinessPointsTotal: 72, fulfillmentDiversity: 0.9, hasUnpaidLoan: false)`, `null`, `null`, `Translate`. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var summary = PlayerStatSummaryBuilder.Build(
            // Meneruskan memanggil `BuildGameplay` dengan `8`, `72`, `0.9`, `false` sebagai argumen ke `PlayerStatSummaryBuilder.Build`; Meneruskan nilai
            // literal `8` sebagai argumen bernama `cashflowNetTotal`; Meneruskan nilai literal `72` sebagai argumen bernama `happinessPointsTotal`; Meneruskan
            // nilai literal `0.9` sebagai argumen bernama `fulfillmentDiversity`; Meneruskan false, yaitu kondisi nonaktif/tidak terpenuhi sebagai argumen
            // bernama `hasUnpaidLoan`.
            BuildGameplay(cashflowNetTotal: 8, happinessPointsTotal: 72, fulfillmentDiversity: 0.9, hasUnpaidLoan: false),
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen ke `PlayerStatSummaryBuilder.Build`.
            null,
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen ke `PlayerStatSummaryBuilder.Build`.
            null,
            // Meneruskan `Translate` (nilai translate) sebagai argumen ke `PlayerStatSummaryBuilder.Build`.
            Translate);

        // Menjalankan pemeriksaan hasil dengan `Assert.Single` menggunakan `summary.Insights`; ketidaksesuaian dengan ekspektasi membuat pengujian gagal
        // dalam Build_DoesNotRepeatPositivePillarsAsDiscussionPriorities.
        Assert.Single(summary.Insights);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”stable_profile”`, `summary.Insights[0].Key`);
        // pengujian gagal jika keduanya berbeda dalam Build_DoesNotRepeatPositivePillarsAsDiscussionPriorities.
        Assert.Equal("stable_profile", summary.Insights[0].Key);
    // Menutup scope metode Build_DoesNotRepeatPositivePillarsAsDiscussionPriorities; bagian berikut berada di luar batas blok tersebut dalam
    // Build_DoesNotRepeatPositivePillarsAsDiscussionPriorities.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `Build_AddsWarningInsightWhenNeedDiversityIsLow` dengan hasil bertipe `void`; operasi ini menangani build adds warning
    // insight when kebutuhan keberagaman berstatus low.
    public void Build_AddsWarningInsightWhenNeedDiversityIsLow()
    // Membuka scope metode Build_AddsWarningInsightWhenNeedDiversityIsLow; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // Build_AddsWarningInsightWhenNeedDiversityIsLow.
    {
        // Menyiapkan variabel lokal `summary` untuk nilai summary dengan memanggil `PlayerStatSummaryBuilder.Build` dengan `BuildGameplay(cashflowNetTotal:
        // 8, happinessPointsTotal: 30, fulfillmentDiversity: 0.35, hasUnpaidLoan: false)`, `null`, `null`, `Translate`. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var summary = PlayerStatSummaryBuilder.Build(
            // Meneruskan memanggil `BuildGameplay` dengan `8`, `30`, `0.35`, `false` sebagai argumen ke `PlayerStatSummaryBuilder.Build`; Meneruskan nilai
            // literal `8` sebagai argumen bernama `cashflowNetTotal`; Meneruskan nilai literal `30` sebagai argumen bernama `happinessPointsTotal`; Meneruskan
            // nilai literal `0.35` sebagai argumen bernama `fulfillmentDiversity`; Meneruskan false, yaitu kondisi nonaktif/tidak terpenuhi sebagai argumen
            // bernama `hasUnpaidLoan`.
            BuildGameplay(cashflowNetTotal: 8, happinessPointsTotal: 30, fulfillmentDiversity: 0.35, hasUnpaidLoan: false),
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen ke `PlayerStatSummaryBuilder.Build`.
            null,
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen ke `PlayerStatSummaryBuilder.Build`.
            null,
            // Meneruskan `Translate` (nilai translate) sebagai argumen ke `PlayerStatSummaryBuilder.Build`.
            Translate);

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `summary.Insights`, `item => item.Key ==
        // ”need_diversity_low” && item.Tone == ”warning”` dalam Build_AddsWarningInsightWhenNeedDiversityIsLow.
        Assert.Contains(summary.Insights, item => item.Key == "need_diversity_low" && item.Tone == "warning");
    // Menutup scope metode Build_AddsWarningInsightWhenNeedDiversityIsLow; bagian berikut berada di luar batas blok tersebut dalam
    // Build_AddsWarningInsightWhenNeedDiversityIsLow.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `Build_ExplainsWhenNeedBalanceCannotBeCalculated` dengan hasil bertipe `void`; operasi ini menangani build explains when
    // kebutuhan saldo cannot be calculated.
    public void Build_ExplainsWhenNeedBalanceCannotBeCalculated()
    // Membuka scope metode Build_ExplainsWhenNeedBalanceCannotBeCalculated; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // Build_ExplainsWhenNeedBalanceCannotBeCalculated.
    {
        // Menyiapkan variabel lokal `summary` untuk nilai summary dengan memanggil `PlayerStatSummaryBuilder.Build` dengan `BuildGameplay(cashflowNetTotal:
        // 8, happinessPointsTotal: 30, fulfillmentDiversity: 0, hasUnpaidLoan: false, needCardsOwned: 0)`, `null`, `null`, `Translate`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var summary = PlayerStatSummaryBuilder.Build(
            // Meneruskan memanggil `BuildGameplay` dengan `8`, `30`, `0`, `false`, `0` sebagai argumen ke `PlayerStatSummaryBuilder.Build`; Meneruskan nilai
            // literal `8` sebagai argumen bernama `cashflowNetTotal`; Meneruskan nilai literal `30` sebagai argumen bernama `happinessPointsTotal`; Meneruskan
            // nilai literal `0` sebagai argumen bernama `fulfillmentDiversity`; Meneruskan false, yaitu kondisi nonaktif/tidak terpenuhi sebagai argumen
            // bernama `hasUnpaidLoan`; Meneruskan nilai literal `0` sebagai argumen bernama `needCardsOwned`.
            BuildGameplay(cashflowNetTotal: 8, happinessPointsTotal: 30, fulfillmentDiversity: 0, hasUnpaidLoan: false, needCardsOwned: 0),
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen ke `PlayerStatSummaryBuilder.Build`.
            null,
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen ke `PlayerStatSummaryBuilder.Build`.
            null,
            // Meneruskan `Translate` (nilai translate) sebagai argumen ke `PlayerStatSummaryBuilder.Build`.
            Translate);

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `summary.Insights`, `item => item.Key ==
        // ”need_cards_missing” && item.Tone == ”warning”` dalam Build_ExplainsWhenNeedBalanceCannotBeCalculated.
        Assert.Contains(summary.Insights, item => item.Key == "need_cards_missing" && item.Tone == "warning");
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `summary.Insights`, `item =>
        // item.Key == ”need_diversity_low”` dalam Build_ExplainsWhenNeedBalanceCannotBeCalculated.
        Assert.DoesNotContain(summary.Insights, item => item.Key == "need_diversity_low");
    // Menutup scope metode Build_ExplainsWhenNeedBalanceCannotBeCalculated; bagian berikut berada di luar batas blok tersebut dalam
    // Build_ExplainsWhenNeedBalanceCannotBeCalculated.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `Build_PrioritizesRisksWithoutPositiveDuplicates` dengan hasil bertipe `void`; operasi ini menangani build prioritizes
    // risks tanpa positive duplicates.
    public void Build_PrioritizesRisksWithoutPositiveDuplicates()
    // Membuka scope metode Build_PrioritizesRisksWithoutPositiveDuplicates; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // Build_PrioritizesRisksWithoutPositiveDuplicates.
    {
        // Menyiapkan variabel lokal `summary` untuk nilai summary dengan memanggil `PlayerStatSummaryBuilder.Build` dengan `BuildGameplay(cashflowNetTotal:
        // 8, happinessPointsTotal: 72, fulfillmentDiversity: 0.35, hasUnpaidLoan: true)`, `null`, `null`, `Translate`. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var summary = PlayerStatSummaryBuilder.Build(
            // Meneruskan memanggil `BuildGameplay` dengan `8`, `72`, `0.35`, `true` sebagai argumen ke `PlayerStatSummaryBuilder.Build`; Meneruskan nilai
            // literal `8` sebagai argumen bernama `cashflowNetTotal`; Meneruskan nilai literal `72` sebagai argumen bernama `happinessPointsTotal`; Meneruskan
            // nilai literal `0.35` sebagai argumen bernama `fulfillmentDiversity`; Meneruskan true, yaitu kondisi aktif/terpenuhi sebagai argumen bernama
            // `hasUnpaidLoan`.
            BuildGameplay(cashflowNetTotal: 8, happinessPointsTotal: 72, fulfillmentDiversity: 0.35, hasUnpaidLoan: true),
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen ke `PlayerStatSummaryBuilder.Build`.
            null,
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen ke `PlayerStatSummaryBuilder.Build`.
            null,
            // Meneruskan `Translate` (nilai translate) sebagai argumen ke `PlayerStatSummaryBuilder.Build`.
            Translate);

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`2`, `summary.Insights.Count`); pengujian gagal
        // jika keduanya berbeda dalam Build_PrioritizesRisksWithoutPositiveDuplicates.
        Assert.Equal(2, summary.Insights.Count);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”loan_unpaid”`, `summary.Insights[0].Key`);
        // pengujian gagal jika keduanya berbeda dalam Build_PrioritizesRisksWithoutPositiveDuplicates.
        Assert.Equal("loan_unpaid", summary.Insights[0].Key);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `summary.Insights`, `item => item.Key ==
        // ”need_diversity_low”` dalam Build_PrioritizesRisksWithoutPositiveDuplicates.
        Assert.Contains(summary.Insights, item => item.Key == "need_diversity_low");
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `summary.Insights`, `item =>
        // item.Tone == ”positive”` dalam Build_PrioritizesRisksWithoutPositiveDuplicates.
        Assert.DoesNotContain(summary.Insights, item => item.Tone == "positive");
    // Menutup scope metode Build_PrioritizesRisksWithoutPositiveDuplicates; bagian berikut berada di luar batas blok tersebut dalam
    // Build_PrioritizesRisksWithoutPositiveDuplicates.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `Build_PrefersTheAuthoritativePlayerAnalyticsSummary` dengan hasil bertipe `void`; operasi ini menangani build prefers the
    // authoritative pemain analytics summary.
    public void Build_PrefersTheAuthoritativePlayerAnalyticsSummary()
    // Membuka scope metode Build_PrefersTheAuthoritativePlayerAnalyticsSummary; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // Build_PrefersTheAuthoritativePlayerAnalyticsSummary.
    {
        // Menyiapkan variabel lokal `summary` untuk nilai summary dengan memanggil `PlayerStatSummaryBuilder.Build` dengan `BuildGameplay(cashflowNetTotal:
        // 0, happinessPointsTotal: 0, fulfillmentDiversity: 0, hasUnpaidLoan: false)`, `BuildAnalyticsSummary(cashIn: 50, cashOut: 20, happiness: 72,
        // fulfillmentDiversity: 0.9, hasUnpaidLoan: false)`, `null`, `Translate`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var summary = PlayerStatSummaryBuilder.Build(
            // Meneruskan memanggil `BuildGameplay` dengan `0`, `0`, `0`, `false` sebagai argumen ke `PlayerStatSummaryBuilder.Build`; Meneruskan nilai literal
            // `0` sebagai argumen bernama `cashflowNetTotal`; Meneruskan nilai literal `0` sebagai argumen bernama `happinessPointsTotal`; Meneruskan nilai
            // literal `0` sebagai argumen bernama `fulfillmentDiversity`; Meneruskan false, yaitu kondisi nonaktif/tidak terpenuhi sebagai argumen bernama
            // `hasUnpaidLoan`.
            BuildGameplay(cashflowNetTotal: 0, happinessPointsTotal: 0, fulfillmentDiversity: 0, hasUnpaidLoan: false),
            // Meneruskan memanggil `BuildAnalyticsSummary` dengan `50`, `20`, `72`, `0.9`, `false` sebagai argumen ke `PlayerStatSummaryBuilder.Build`;
            // Meneruskan nilai literal `50` sebagai argumen bernama `cashIn`; Meneruskan nilai literal `20` sebagai argumen bernama `cashOut`; Meneruskan nilai
            // literal `72` sebagai argumen bernama `happiness`; Meneruskan nilai literal `0.9` sebagai argumen bernama `fulfillmentDiversity`; Meneruskan
            // false, yaitu kondisi nonaktif/tidak terpenuhi sebagai argumen bernama `hasUnpaidLoan`.
            BuildAnalyticsSummary(cashIn: 50, cashOut: 20, happiness: 72, fulfillmentDiversity: 0.9, hasUnpaidLoan: false),
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen ke `PlayerStatSummaryBuilder.Build`.
            null,
            // Meneruskan `Translate` (nilai translate) sebagai argumen ke `PlayerStatSummaryBuilder.Build`.
            Translate);

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `summary.Insights`, `item => item.Key ==
        // ”stable_profile”` dalam Build_PrefersTheAuthoritativePlayerAnalyticsSummary.
        Assert.Contains(summary.Insights, item => item.Key == "stable_profile");
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `summary.Insights`, `item =>
        // item.Key == ”happiness_low”` dalam Build_PrefersTheAuthoritativePlayerAnalyticsSummary.
        Assert.DoesNotContain(summary.Insights, item => item.Key == "happiness_low");
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `summary.Insights`, `item =>
        // item.Key == ”need_diversity_low”` dalam Build_PrefersTheAuthoritativePlayerAnalyticsSummary.
        Assert.DoesNotContain(summary.Insights, item => item.Key == "need_diversity_low");
    // Menutup scope metode Build_PrefersTheAuthoritativePlayerAnalyticsSummary; bagian berikut berada di luar batas blok tersebut dalam
    // Build_PrefersTheAuthoritativePlayerAnalyticsSummary.
    }

    // Mendefinisikan metode `BuildAnalyticsSummary` dengan hasil bertipe `AnalyticsByPlayerItem`; operasi ini menangani build analytics summary.
    // Masukan: Parameter `cashIn` bertipe `double` membawa nilai uang tunai in; Parameter `cashOut` bertipe `double` membawa nilai uang tunai out;
    // Parameter `happiness` bertipe `double` membawa nilai kebahagiaan; Parameter `fulfillmentDiversity` bertipe `double` membawa tingkat keberagaman
    // kategori kebutuhan yang telah dipenuhi; Parameter `hasUnpaidLoan` bertipe `bool` membawa nilai memiliki unpaid pinjaman.
    private static AnalyticsByPlayerItem BuildAnalyticsSummary(
        // Parameter `cashIn` bertipe `double` membawa nilai uang tunai in.
        double cashIn,
        // Parameter `cashOut` bertipe `double` membawa nilai uang tunai out.
        double cashOut,
        // Parameter `happiness` bertipe `double` membawa nilai kebahagiaan.
        double happiness,
        // Parameter `fulfillmentDiversity` bertipe `double` membawa tingkat keberagaman kategori kebutuhan yang telah dipenuhi.
        double fulfillmentDiversity,
        // Parameter `hasUnpaidLoan` bertipe `bool` membawa nilai memiliki unpaid pinjaman.
        bool hasUnpaidLoan)
    // Membuka scope metode BuildAnalyticsSummary; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildAnalyticsSummary.
    {
        // Mengembalikan objek baru bertipe `AnalyticsByPlayerItem` dengan argumen ( Guid.NewGuid(), 1, cashIn, cashOut, 0, 0, 0, 0, 0,
        // fulfillmentDiversity, happiness, 0, 0, 0, 0, 0, 0, 0, hasUnpaidLoan ? 4 : 0, hasUnpaidLoan) kepada pemanggil dalam BuildAnalyticsSummary;
        // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return new AnalyticsByPlayerItem(
            // Meneruskan memanggil `Guid.NewGuid` dengan tanpa argumen sebagai argumen ke konstruktor `AnalyticsByPlayerItem`; Meneruskan nilai literal `1`
            // sebagai argumen ke konstruktor `AnalyticsByPlayerItem`; Meneruskan `cashIn` (nilai uang tunai in) sebagai argumen ke konstruktor
            // `AnalyticsByPlayerItem`; Meneruskan `cashOut` (nilai uang tunai out) sebagai argumen ke konstruktor `AnalyticsByPlayerItem`; Meneruskan nilai
            // literal `0` sebagai argumen ke konstruktor `AnalyticsByPlayerItem`; Meneruskan nilai literal `0` sebagai argumen ke konstruktor
            // `AnalyticsByPlayerItem`; Meneruskan nilai literal `0` sebagai argumen ke konstruktor `AnalyticsByPlayerItem`; Meneruskan nilai literal `0`
            // sebagai argumen ke konstruktor `AnalyticsByPlayerItem`; Meneruskan nilai literal `0` sebagai argumen ke konstruktor `AnalyticsByPlayerItem`.
            Guid.NewGuid(), 1, cashIn, cashOut, 0, 0, 0, 0, 0,
            // Meneruskan `fulfillmentDiversity` (tingkat keberagaman kategori kebutuhan yang telah dipenuhi) sebagai argumen ke konstruktor
            // `AnalyticsByPlayerItem`; Meneruskan `happiness` (nilai kebahagiaan) sebagai argumen ke konstruktor `AnalyticsByPlayerItem`; Meneruskan nilai
            // literal `0` sebagai argumen ke konstruktor `AnalyticsByPlayerItem`; Meneruskan nilai literal `0` sebagai argumen ke konstruktor
            // `AnalyticsByPlayerItem`; Meneruskan nilai literal `0` sebagai argumen ke konstruktor `AnalyticsByPlayerItem`; Meneruskan nilai literal `0`
            // sebagai argumen ke konstruktor `AnalyticsByPlayerItem`; Meneruskan nilai literal `0` sebagai argumen ke konstruktor `AnalyticsByPlayerItem`;
            // Meneruskan nilai literal `0` sebagai argumen ke konstruktor `AnalyticsByPlayerItem`; Meneruskan nilai literal `0` sebagai argumen ke konstruktor
            // `AnalyticsByPlayerItem`.
            fulfillmentDiversity, happiness, 0, 0, 0, 0, 0, 0, 0,
            // Meneruskan hasil pemilihan bersyarat: ketika `hasUnpaidLoan` benar gunakan `4`, jika tidak gunakan `0` sebagai argumen ke konstruktor
            // `AnalyticsByPlayerItem`; Meneruskan `hasUnpaidLoan` (nilai memiliki unpaid pinjaman) sebagai argumen ke konstruktor `AnalyticsByPlayerItem`.
            hasUnpaidLoan ? 4 : 0, hasUnpaidLoan);
    // Menutup scope metode BuildAnalyticsSummary; bagian berikut berada di luar batas blok tersebut dalam BuildAnalyticsSummary.
    }

    // Mendefinisikan metode `BuildGameplay` dengan hasil bertipe `GameplayMetricsResponse`; operasi ini menangani build gameplay. Masukan: Parameter
    // `cashflowNetTotal` bertipe `double` membawa selisih pemasukan terhadap pengeluaran arus kas; Parameter `happinessPointsTotal` bertipe `double`
    // membawa akumulasi poin kebahagiaan pemain; Parameter `fulfillmentDiversity` bertipe `double` membawa tingkat keberagaman kategori kebutuhan yang
    // telah dipenuhi; Parameter `hasUnpaidLoan` bertipe `bool` membawa nilai memiliki unpaid pinjaman; Parameter `needCardsOwned` bertipe `int?`
    // membawa nilai kebutuhan kartu dimiliki; nilai null diizinkan ketika data opsional belum tersedia; bila argumen tidak diberikan digunakan null,
    // yaitu penanda tidak ada nilai.
    private static GameplayMetricsResponse BuildGameplay(
        // Parameter `cashflowNetTotal` bertipe `double` membawa selisih pemasukan terhadap pengeluaran arus kas.
        double cashflowNetTotal,
        // Parameter `happinessPointsTotal` bertipe `double` membawa akumulasi poin kebahagiaan pemain.
        double happinessPointsTotal,
        // Parameter `fulfillmentDiversity` bertipe `double` membawa tingkat keberagaman kategori kebutuhan yang telah dipenuhi.
        double fulfillmentDiversity,
        // Parameter `hasUnpaidLoan` bertipe `bool` membawa nilai memiliki unpaid pinjaman.
        bool hasUnpaidLoan,
        // Parameter `needCardsOwned` bertipe `int?` membawa nilai kebutuhan kartu dimiliki; nilai null diizinkan ketika data opsional belum tersedia; bila
        // argumen tidak diberikan digunakan null, yaitu penanda tidak ada nilai.
        int? needCardsOwned = null)
    // Membuka scope metode BuildGameplay; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildGameplay.
    {
        // Menyiapkan variabel lokal `rawJson` untuk nilai raw JSON dengan hasil pemilihan bersyarat: ketika `needCardsOwned.HasValue` benar gunakan
        // `JsonSerializer.SerializeToElement(new { needs = new { need_cards_owned_current = needCardsOwned.Value } })`, jika tidak gunakan `null`. Tipe
        // yang dipakai adalah `JsonElement?`.
        JsonElement? rawJson = needCardsOwned.HasValue
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: JsonSerializer.SerializeToElement(new dalam BuildGameplay.
            ? JsonSerializer.SerializeToElement(new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildGameplay.
            {
                // Meneruskan objek anonim yang mengelompokkan needs sebagai satu nilai sebagai argumen ke `JsonSerializer.SerializeToElement`.
                needs = new { need_cards_owned_current = needCardsOwned.Value }
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam BuildGameplay.
            })
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: null; dalam BuildGameplay.
            : null;

        // Mengembalikan objek baru bertipe `GameplayMetricsResponse` dengan argumen ( Guid.NewGuid(), Guid.NewGuid(), DateTimeOffset.UtcNow, new
        // GameplayEconomyMetrics(20, 50, 50 - cashflowNetTotal, cashflowNetTotal, 6), new GameplayProgressMet... kepada pemanggil dalam BuildGameplay;
        // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return new GameplayMetricsResponse(
            // Meneruskan memanggil `Guid.NewGuid` dengan tanpa argumen sebagai argumen ke konstruktor `GameplayMetricsResponse`.
            Guid.NewGuid(),
            // Meneruskan memanggil `Guid.NewGuid` dengan tanpa argumen sebagai argumen ke konstruktor `GameplayMetricsResponse`.
            Guid.NewGuid(),
            // Meneruskan `DateTimeOffset.UtcNow`, yaitu waktu UTC saat operasi dilakukan sebagai argumen ke konstruktor `GameplayMetricsResponse`.
            DateTimeOffset.UtcNow,
            // Meneruskan objek baru bertipe `GameplayEconomyMetrics` dengan argumen (20, 50, 50 - cashflowNetTotal, cashflowNetTotal, 6) sebagai argumen ke
            // konstruktor `GameplayMetricsResponse`; Meneruskan nilai literal `20` sebagai argumen ke konstruktor `GameplayEconomyMetrics`; Meneruskan nilai
            // literal `50` sebagai argumen ke konstruktor `GameplayEconomyMetrics`; Meneruskan selisih antara `50` dan `cashflowNetTotal` sebagai argumen ke
            // konstruktor `GameplayEconomyMetrics`; Meneruskan `cashflowNetTotal` (selisih pemasukan terhadap pengeluaran arus kas) sebagai argumen ke
            // konstruktor `GameplayEconomyMetrics`; Meneruskan nilai literal `6` sebagai argumen ke konstruktor `GameplayEconomyMetrics`.
            new GameplayEconomyMetrics(20, 50, 50 - cashflowNetTotal, cashflowNetTotal, 6),
            // Meneruskan objek baru bertipe `GameplayProgressMetrics` dengan argumen (3, 2, 5, 11) sebagai argumen ke konstruktor `GameplayMetricsResponse`;
            // Meneruskan nilai literal `3` sebagai argumen ke konstruktor `GameplayProgressMetrics`; Meneruskan nilai literal `2` sebagai argumen ke
            // konstruktor `GameplayProgressMetrics`; Meneruskan nilai literal `5` sebagai argumen ke konstruktor `GameplayProgressMetrics`; Meneruskan nilai
            // literal `11` sebagai argumen ke konstruktor `GameplayProgressMetrics`.
            new GameplayProgressMetrics(3, 2, 5, 11),
            // Meneruskan objek baru bertipe `GameplayScoreMetrics` dengan argumen (happinessPointsTotal, 12, 2, 6, 4, 3, 5, 0, hasUnpaidLoan ? 4 : 0,
            // hasUnpaidLoan) sebagai argumen ke konstruktor `GameplayMetricsResponse`; Meneruskan `happinessPointsTotal` (akumulasi poin kebahagiaan pemain)
            // sebagai argumen ke konstruktor `GameplayScoreMetrics`; Meneruskan nilai literal `12` sebagai argumen ke konstruktor `GameplayScoreMetrics`;
            // Meneruskan nilai literal `2` sebagai argumen ke konstruktor `GameplayScoreMetrics`; Meneruskan nilai literal `6` sebagai argumen ke konstruktor
            // `GameplayScoreMetrics`; Meneruskan nilai literal `4` sebagai argumen ke konstruktor `GameplayScoreMetrics`; Meneruskan nilai literal `3` sebagai
            // argumen ke konstruktor `GameplayScoreMetrics`; Meneruskan nilai literal `5` sebagai argumen ke konstruktor `GameplayScoreMetrics`; Meneruskan
            // nilai literal `0` sebagai argumen ke konstruktor `GameplayScoreMetrics`; Meneruskan hasil pemilihan bersyarat: ketika `hasUnpaidLoan` benar
            // gunakan `4`, jika tidak gunakan `0` sebagai argumen ke konstruktor `GameplayScoreMetrics`; Meneruskan `hasUnpaidLoan` (nilai memiliki unpaid
            // pinjaman) sebagai argumen ke konstruktor `GameplayScoreMetrics`.
            new GameplayScoreMetrics(happinessPointsTotal, 12, 2, 6, 4, 3, 5, 0, hasUnpaidLoan ? 4 : 0, hasUnpaidLoan),
            // Meneruskan objek baru bertipe `GameplayNeedMetrics` dengan argumen (fulfillmentDiversity) sebagai argumen ke konstruktor
            // `GameplayMetricsResponse`; Meneruskan `fulfillmentDiversity` (tingkat keberagaman kategori kebutuhan yang telah dipenuhi) sebagai argumen ke
            // konstruktor `GameplayNeedMetrics`.
            new GameplayNeedMetrics(fulfillmentDiversity),
            // Meneruskan `rawJson` (nilai raw JSON) sebagai argumen ke konstruktor `GameplayMetricsResponse`.
            rawJson);
    // Menutup scope metode BuildGameplay; bagian berikut berada di luar batas blok tersebut dalam BuildGameplay.
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
            // Untuk pola `”players.stats.clear”`, menghasilkan nilai literal `”Clear”` sebagai hasil switch.
            "players.stats.clear" => "Clear",
            // Untuk pola `”players.stats.unpaid”`, menghasilkan nilai literal `”Unpaid”` sebagai hasil switch.
            "players.stats.unpaid" => "Unpaid",
            // Untuk pola `_`, menghasilkan `key` (nilai kunci) sebagai hasil switch.
            _ => key
        // Menutup scope pemetaan switch atas `key`; bagian berikut berada di luar batas blok tersebut dalam Translate.
        };
    // Menutup scope metode Translate; bagian berikut berada di luar batas blok tersebut dalam Translate.
    }
// Menutup scope tipe PlayerStatSummaryBuilderTests; bagian berikut berada di luar batas blok tersebut.
}
