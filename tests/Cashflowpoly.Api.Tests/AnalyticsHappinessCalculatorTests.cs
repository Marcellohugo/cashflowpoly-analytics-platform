// Fungsi file: Memverifikasi perilaku API, database, atau domain melalui AnalyticsHappinessCalculatorTests.
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;
// Mengimpor namespace `Cashflowpoly.Api.Domain` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Domain;
// Mengimpor namespace `Xunit` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Xunit;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Tests` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Tests;

// Mendefinisikan tipe class `AnalyticsHappinessCalculatorTests`; sealed mencegah tipe ini diturunkan lagi.
public sealed class AnalyticsHappinessCalculatorTests
// Membuka scope tipe AnalyticsHappinessCalculatorTests; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `ComputeByPlayer_IncludesSetupGoldLoanAndMissionFamily` dengan hasil bertipe `void`; operasi ini menangani compute
    // berdasarkan pemain includes setup emas pinjaman dan misi kelompok.
    public void ComputeByPlayer_IncludesSetupGoldLoanAndMissionFamily()
    // Membuka scope metode ComputeByPlayer_IncludesSetupGoldLoanAndMissionFamily; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // ComputeByPlayer_IncludesSetupGoldLoanAndMissionFamily.
    {
        // Menyiapkan variabel lokal `playerId` untuk nilai pemain identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var playerId = Guid.NewGuid();
        // Menyiapkan variabel lokal `events` untuk kumpulan event permainan sebagai sumber riwayat untuk validasi atau perhitungan dengan objek baru
        // bertipe `List<EventDb>` dengan nilai awal sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var events = new List<EventDb>
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ComputeByPlayer_IncludesSetupGoldLoanAndMissionFamily.
        {
            // Melanjutkan pengolahan dengan memanggil `BuildEvent` dengan `playerId`, `”SetupEmasAwal”`, `”””{”qty”:1,”setup”:”INITIAL”}”””`, `0`, `1` dalam
            // ComputeByPlayer_IncludesSetupGoldLoanAndMissionFamily.
            BuildEvent(playerId, "SetupEmasAwal", """{"qty":1,"setup":"INITIAL"}""", 0, 1),
            // Melanjutkan pengolahan dengan memanggil `BuildEvent` dengan `playerId`, `”SetupPinjamanAwal”`,
            // `”””{”loan_id”:”setup-loan”,”principal”:10,”penalty_points”:15}”””`, `0`, `2` dalam ComputeByPlayer_IncludesSetupGoldLoanAndMissionFamily.
            BuildEvent(playerId, "SetupPinjamanAwal", """{"loan_id":"setup-loan","principal":10,"penalty_points":15}""", 0, 2),
            // Melanjutkan pengolahan dengan memanggil `BuildEvent` dengan `playerId`, `”SetupMisiAwal”`,
            // `”””{”mission_id”:”misi-boneka”,”target_tertiary_card_id”:”boneka”,”penalty_points”:10,”require_primary”:true,”require_secondary”:true}”””`, `0`,
            // `3` dalam ComputeByPlayer_IncludesSetupGoldLoanAndMissionFamily.
            BuildEvent(playerId, "SetupMisiAwal", """{"mission_id":"misi-boneka","target_tertiary_card_id":"boneka","penalty_points":10,"require_primary":true,"require_secondary":true}""", 0, 3),
            // Melanjutkan pengolahan dengan memanggil `BuildEvent` dengan `playerId`, `”Kebutuhan”`,
            // `”””{”amount”:2,”card_id”:”primary-food”,”need_tier”:”primer”,”points”:1}”””`, `1`, `4` dalam
            // ComputeByPlayer_IncludesSetupGoldLoanAndMissionFamily.
            BuildEvent(playerId, "Kebutuhan", """{"amount":2,"card_id":"primary-food","need_tier":"primer","points":1}""", 1, 4),
            // Melanjutkan pengolahan dengan memanggil `BuildEvent` dengan `playerId`, `”Kebutuhan”`,
            // `”””{”amount”:3,”card_id”:”secondary-school”,”need_tier”:”sekunder”,”points”:2}”””`, `1`, `5` dalam
            // ComputeByPlayer_IncludesSetupGoldLoanAndMissionFamily.
            BuildEvent(playerId, "Kebutuhan", """{"amount":3,"card_id":"secondary-school","need_tier":"sekunder","points":2}""", 1, 5),
            // Melanjutkan pengolahan dengan memanggil `BuildEvent` dengan `playerId`, `”Kebutuhan”`,
            // `”””{”amount”:6,”card_id”:”boneka_1”,”need_tier”:”tersier”,”points”:5}”””`, `1`, `6` dalam ComputeByPlayer_IncludesSetupGoldLoanAndMissionFamily.
            BuildEvent(playerId, "Kebutuhan", """{"amount":6,"card_id":"boneka_1","need_tier":"tersier","points":5}""", 1, 6),
            // Melanjutkan pengolahan dengan memanggil `BuildEvent` dengan `playerId`, `”TujuanFinansial”`, `”””{”goal_id”:”home”,”points”:8,”cost”:5}”””`, `1`,
            // `7` dalam ComputeByPlayer_IncludesSetupGoldLoanAndMissionFamily.
            BuildEvent(playerId, "TujuanFinansial", """{"goal_id":"home","points":8,"cost":5}""", 1, 7)
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // ComputeByPlayer_IncludesSetupGoldLoanAndMissionFamily.
        };
        // Menyiapkan variabel lokal `config` untuk konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan dengan memanggil `BuildConfig`
        // dengan `new RulesetScoringConfig( [], [new QtyPoint(1, 3), new QtyPoint(2, 5), new QtyPoint(3, 8), new QtyPoint(4, 12)], [])`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var config = BuildConfig(new RulesetScoringConfig(
            // Meneruskan koleksi kosong dengan tipe mengikuti konteks tujuan sebagai argumen ke konstruktor `RulesetScoringConfig`.
            [],
            // Meneruskan koleksi berisi new QtyPoint(1, 3), new QtyPoint(2, 5), new QtyPoint(3, 8), new QtyPoint(4, 12) sebagai argumen ke konstruktor
            // `RulesetScoringConfig`; Meneruskan nilai literal `1` sebagai argumen ke konstruktor `QtyPoint`; Meneruskan nilai literal `3` sebagai argumen ke
            // konstruktor `QtyPoint`; Meneruskan nilai literal `2` sebagai argumen ke konstruktor `QtyPoint`; Meneruskan nilai literal `5` sebagai argumen ke
            // konstruktor `QtyPoint`; Meneruskan nilai literal `3` sebagai argumen ke konstruktor `QtyPoint`; Meneruskan nilai literal `8` sebagai argumen ke
            // konstruktor `QtyPoint`; Meneruskan nilai literal `4` sebagai argumen ke konstruktor `QtyPoint`; Meneruskan nilai literal `12` sebagai argumen ke
            // konstruktor `QtyPoint`.
            [new QtyPoint(1, 3), new QtyPoint(2, 5), new QtyPoint(3, 8), new QtyPoint(4, 12)],
            // Meneruskan koleksi kosong dengan tipe mengikuti konteks tujuan sebagai argumen ke konstruktor `RulesetScoringConfig`.
            []));

        // Menyiapkan variabel lokal `breakdown` untuk nilai breakdown dengan `new HappinessCalculator().ComputeByPlayer(events, [], config)[playerId]`,
        // yaitu elemen koleksi yang dipilih melalui indeks atau kunci tersebut. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var breakdown = new HappinessCalculator().ComputeByPlayer(events, [], config)[playerId];

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`3`, `breakdown.GoldPoints`); pengujian gagal
        // jika keduanya berbeda dalam ComputeByPlayer_IncludesSetupGoldLoanAndMissionFamily.
        Assert.Equal(3, breakdown.GoldPoints);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`0`, `breakdown.MissionPenaltyPoints`);
        // pengujian gagal jika keduanya berbeda dalam ComputeByPlayer_IncludesSetupGoldLoanAndMissionFamily.
        Assert.Equal(0, breakdown.MissionPenaltyPoints);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`15`, `breakdown.LoanPenaltyPoints`); pengujian
        // gagal jika keduanya berbeda dalam ComputeByPlayer_IncludesSetupGoldLoanAndMissionFamily.
        Assert.Equal(15, breakdown.LoanPenaltyPoints);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`0`, `breakdown.SavingGoalPointsEffective`);
        // pengujian gagal jika keduanya berbeda dalam ComputeByPlayer_IncludesSetupGoldLoanAndMissionFamily.
        Assert.Equal(0, breakdown.SavingGoalPointsEffective);
        // Menjalankan pemeriksaan bahwa `breakdown.HasUnpaidLoan` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // ComputeByPlayer_IncludesSetupGoldLoanAndMissionFamily.
        Assert.True(breakdown.HasUnpaidLoan);
    // Menutup scope metode ComputeByPlayer_IncludesSetupGoldLoanAndMissionFamily; bagian berikut berada di luar batas blok tersebut dalam
    // ComputeByPlayer_IncludesSetupGoldLoanAndMissionFamily.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `ComputeBreakdown_AwardsMixedNeedSetBonus` dengan hasil bertipe `void`; operasi ini menangani compute breakdown awards
    // mixed kebutuhan set bonus.
    public void ComputeBreakdown_AwardsMixedNeedSetBonus()
    // Membuka scope metode ComputeBreakdown_AwardsMixedNeedSetBonus; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // ComputeBreakdown_AwardsMixedNeedSetBonus.
    {
        // Menyiapkan variabel lokal `playerEvents` untuk nilai pemain event dengan objek baru bertipe `List<EventDb>` dengan nilai awal sesuai
        // konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var playerEvents = new List<EventDb>
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ComputeBreakdown_AwardsMixedNeedSetBonus.
        {
            // Melanjutkan pengolahan dengan memanggil `BuildEvent` dengan `”Kebutuhan”`, `”””{”amount”:2,”card_id”:”primary-food”,”points”:1}”””` dalam
            // ComputeBreakdown_AwardsMixedNeedSetBonus.
            BuildEvent("Kebutuhan", """{"amount":2,"card_id":"primary-food","points":1}"""),
            // Melanjutkan pengolahan dengan memanggil `BuildEvent` dengan `”Kebutuhan”`, `”””{”amount”:3,”card_id”:”secondary-school”,”points”:2}”””` dalam
            // ComputeBreakdown_AwardsMixedNeedSetBonus.
            BuildEvent("Kebutuhan", """{"amount":3,"card_id":"secondary-school","points":2}"""),
            // Melanjutkan pengolahan dengan memanggil `BuildEvent` dengan `”Kebutuhan”`, `”””{”amount”:4,”card_id”:”tertiary-bike”,”points”:3}”””` dalam
            // ComputeBreakdown_AwardsMixedNeedSetBonus.
            BuildEvent("Kebutuhan", """{"amount":4,"card_id":"tertiary-bike","points":3}""")
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // ComputeBreakdown_AwardsMixedNeedSetBonus.
        };

        // Menyiapkan variabel lokal `breakdown` untuk nilai breakdown dengan memanggil `new HappinessCalculator().ComputeBreakdown` dengan `playerEvents`,
        // `0`, `0`, `0`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var breakdown = new HappinessCalculator().ComputeBreakdown(
            // Meneruskan `playerEvents` (nilai pemain event) sebagai argumen ke `new HappinessCalculator().ComputeBreakdown`.
            playerEvents,
            // Meneruskan nilai literal `0` sebagai argumen bernama `donationPoints`.
            donationPoints: 0,
            // Meneruskan nilai literal `0` sebagai argumen bernama `goldPoints`.
            goldPoints: 0,
            // Meneruskan nilai literal `0` sebagai argumen bernama `pensionPoints`.
            pensionPoints: 0);

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`6`, `breakdown.NeedPoints`); pengujian gagal
        // jika keduanya berbeda dalam ComputeBreakdown_AwardsMixedNeedSetBonus.
        Assert.Equal(6, breakdown.NeedPoints);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`4`, `breakdown.NeedSetBonusPoints`); pengujian
        // gagal jika keduanya berbeda dalam ComputeBreakdown_AwardsMixedNeedSetBonus.
        Assert.Equal(4, breakdown.NeedSetBonusPoints);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`10`, `breakdown.Total`); pengujian gagal jika
        // keduanya berbeda dalam ComputeBreakdown_AwardsMixedNeedSetBonus.
        Assert.Equal(10, breakdown.Total);
    // Menutup scope metode ComputeBreakdown_AwardsMixedNeedSetBonus; bagian berikut berada di luar batas blok tersebut dalam
    // ComputeBreakdown_AwardsMixedNeedSetBonus.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `ComputeBreakdown_CountsMixedAndSameNeedSetsIndependently` dengan hasil bertipe `void`; operasi ini menangani compute
    // breakdown counts mixed dan same kebutuhan sets independently.
    public void ComputeBreakdown_CountsMixedAndSameNeedSetsIndependently()
    // Membuka scope metode ComputeBreakdown_CountsMixedAndSameNeedSetsIndependently; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // ComputeBreakdown_CountsMixedAndSameNeedSetsIndependently.
    {
        // Menyiapkan variabel lokal `playerEvents` untuk nilai pemain event dengan objek baru bertipe `List<EventDb>` dengan nilai awal sesuai
        // konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var playerEvents = new List<EventDb>
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ComputeBreakdown_CountsMixedAndSameNeedSetsIndependently.
        {
            // Melanjutkan pengolahan dengan memanggil `BuildEvent` dengan `”Kebutuhan”`,
            // `”””{”card_id”:”primer-1”,”need_tier”:”primer”,”amount”:1,”points”:1}”””` dalam ComputeBreakdown_CountsMixedAndSameNeedSetsIndependently.
            BuildEvent("Kebutuhan", """{"card_id":"primer-1","need_tier":"primer","amount":1,"points":1}"""),
            // Melanjutkan pengolahan dengan memanggil `BuildEvent` dengan `”Kebutuhan”`,
            // `”””{”card_id”:”primer-2”,”need_tier”:”primer”,”amount”:1,”points”:1}”””` dalam ComputeBreakdown_CountsMixedAndSameNeedSetsIndependently.
            BuildEvent("Kebutuhan", """{"card_id":"primer-2","need_tier":"primer","amount":1,"points":1}"""),
            // Melanjutkan pengolahan dengan memanggil `BuildEvent` dengan `”Kebutuhan”`,
            // `”””{”card_id”:”primer-3”,”need_tier”:”primer”,”amount”:1,”points”:1}”””` dalam ComputeBreakdown_CountsMixedAndSameNeedSetsIndependently.
            BuildEvent("Kebutuhan", """{"card_id":"primer-3","need_tier":"primer","amount":1,"points":1}"""),
            // Melanjutkan pengolahan dengan memanggil `BuildEvent` dengan `”Kebutuhan”`,
            // `”””{”card_id”:”sekunder-1”,”need_tier”:”sekunder”,”amount”:1,”points”:1}”””` dalam ComputeBreakdown_CountsMixedAndSameNeedSetsIndependently.
            BuildEvent("Kebutuhan", """{"card_id":"sekunder-1","need_tier":"sekunder","amount":1,"points":1}"""),
            // Melanjutkan pengolahan dengan memanggil `BuildEvent` dengan `”Kebutuhan”`,
            // `”””{”card_id”:”sekunder-2”,”need_tier”:”sekunder”,”amount”:1,”points”:1}”””` dalam ComputeBreakdown_CountsMixedAndSameNeedSetsIndependently.
            BuildEvent("Kebutuhan", """{"card_id":"sekunder-2","need_tier":"sekunder","amount":1,"points":1}"""),
            // Melanjutkan pengolahan dengan memanggil `BuildEvent` dengan `”Kebutuhan”`,
            // `”””{”card_id”:”tersier-1”,”need_tier”:”tersier”,”amount”:1,”points”:1}”””` dalam ComputeBreakdown_CountsMixedAndSameNeedSetsIndependently.
            BuildEvent("Kebutuhan", """{"card_id":"tersier-1","need_tier":"tersier","amount":1,"points":1}""")
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // ComputeBreakdown_CountsMixedAndSameNeedSetsIndependently.
        };

        // Menyiapkan variabel lokal `breakdown` untuk nilai breakdown dengan memanggil `new HappinessCalculator().ComputeBreakdown` dengan `playerEvents`,
        // `0`, `0`, `0`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var breakdown = new HappinessCalculator().ComputeBreakdown(
            // Meneruskan `playerEvents` (nilai pemain event) sebagai argumen ke `new HappinessCalculator().ComputeBreakdown`.
            playerEvents,
            // Meneruskan nilai literal `0` sebagai argumen bernama `donationPoints`.
            donationPoints: 0,
            // Meneruskan nilai literal `0` sebagai argumen bernama `goldPoints`.
            goldPoints: 0,
            // Meneruskan nilai literal `0` sebagai argumen bernama `pensionPoints`.
            pensionPoints: 0);

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`6`, `breakdown.NeedPoints`); pengujian gagal
        // jika keduanya berbeda dalam ComputeBreakdown_CountsMixedAndSameNeedSetsIndependently.
        Assert.Equal(6, breakdown.NeedPoints);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`6`, `breakdown.NeedSetBonusPoints`); pengujian
        // gagal jika keduanya berbeda dalam ComputeBreakdown_CountsMixedAndSameNeedSetsIndependently.
        Assert.Equal(6, breakdown.NeedSetBonusPoints);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`12`, `breakdown.Total`); pengujian gagal jika
        // keduanya berbeda dalam ComputeBreakdown_CountsMixedAndSameNeedSetsIndependently.
        Assert.Equal(12, breakdown.Total);
    // Menutup scope metode ComputeBreakdown_CountsMixedAndSameNeedSetsIndependently; bagian berikut berada di luar batas blok tersebut dalam
    // ComputeBreakdown_CountsMixedAndSameNeedSetsIndependently.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `ComputeBreakdown_ExcludesNeedSoldForEmergency` dengan hasil bertipe `void`; operasi ini menangani compute breakdown
    // excludes kebutuhan terjual untuk emergency.
    public void ComputeBreakdown_ExcludesNeedSoldForEmergency()
    // Membuka scope metode ComputeBreakdown_ExcludesNeedSoldForEmergency; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // ComputeBreakdown_ExcludesNeedSoldForEmergency.
    {
        // Menyiapkan variabel lokal `playerEvents` untuk nilai pemain event dengan objek baru bertipe `List<EventDb>` dengan nilai awal sesuai
        // konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var playerEvents = new List<EventDb>
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ComputeBreakdown_ExcludesNeedSoldForEmergency.
        {
            // Melanjutkan pengolahan dengan memanggil `BuildEvent` dengan `”Kebutuhan”`, `”””{”amount”:2,”card_id”:”primary-food”,”points”:1}”””` dalam
            // ComputeBreakdown_ExcludesNeedSoldForEmergency.
            BuildEvent("Kebutuhan", """{"amount":2,"card_id":"primary-food","points":1}"""),
            // Melanjutkan pengolahan dengan memanggil `BuildEvent` dengan `”Kebutuhan”`, `”””{”amount”:3,”card_id”:”secondary-school”,”points”:2}”””` dalam
            // ComputeBreakdown_ExcludesNeedSoldForEmergency.
            BuildEvent("Kebutuhan", """{"amount":3,"card_id":"secondary-school","points":2}"""),
            // Melanjutkan pengolahan dengan memanggil `BuildEvent` dengan `”Kebutuhan”`, `”””{”amount”:4,”card_id”:”tertiary-bike”,”points”:3}”””` dalam
            // ComputeBreakdown_ExcludesNeedSoldForEmergency.
            BuildEvent("Kebutuhan", """{"amount":4,"card_id":"tertiary-bike","points":3}"""),
            // Melanjutkan pengolahan dengan memanggil `BuildEvent` dengan `”GunakanOpsiDarurat”`,
            // `”””{”option_type”:”SELL_NEED”,”direction”:”IN”,”amount”:1,”card_id”:”secondary-school”}”””` dalam ComputeBreakdown_ExcludesNeedSoldForEmergency.
            BuildEvent("GunakanOpsiDarurat", """{"option_type":"SELL_NEED","direction":"IN","amount":1,"card_id":"secondary-school"}""")
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // ComputeBreakdown_ExcludesNeedSoldForEmergency.
        };

        // Menyiapkan variabel lokal `breakdown` untuk nilai breakdown dengan memanggil `new HappinessCalculator().ComputeBreakdown` dengan `playerEvents`,
        // `0`, `0`, `0`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var breakdown = new HappinessCalculator().ComputeBreakdown(
            // Meneruskan `playerEvents` (nilai pemain event) sebagai argumen ke `new HappinessCalculator().ComputeBreakdown`.
            playerEvents,
            // Meneruskan nilai literal `0` sebagai argumen bernama `donationPoints`.
            donationPoints: 0,
            // Meneruskan nilai literal `0` sebagai argumen bernama `goldPoints`.
            goldPoints: 0,
            // Meneruskan nilai literal `0` sebagai argumen bernama `pensionPoints`.
            pensionPoints: 0);

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`4`, `breakdown.NeedPoints`); pengujian gagal
        // jika keduanya berbeda dalam ComputeBreakdown_ExcludesNeedSoldForEmergency.
        Assert.Equal(4, breakdown.NeedPoints);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`0`, `breakdown.NeedSetBonusPoints`); pengujian
        // gagal jika keduanya berbeda dalam ComputeBreakdown_ExcludesNeedSoldForEmergency.
        Assert.Equal(0, breakdown.NeedSetBonusPoints);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`4`, `breakdown.Total`); pengujian gagal jika
        // keduanya berbeda dalam ComputeBreakdown_ExcludesNeedSoldForEmergency.
        Assert.Equal(4, breakdown.Total);
    // Menutup scope metode ComputeBreakdown_ExcludesNeedSoldForEmergency; bagian berikut berada di luar batas blok tersebut dalam
    // ComputeBreakdown_ExcludesNeedSoldForEmergency.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `ComputeBreakdown_SoldNeedStillCountsAsMissionPurchaseButNotEndingHappiness` dengan hasil bertipe `void`; operasi ini
    // menangani compute breakdown terjual kebutuhan still counts as misi pembelian but not ending kebahagiaan.
    public void ComputeBreakdown_SoldNeedStillCountsAsMissionPurchaseButNotEndingHappiness()
    // Membuka scope metode ComputeBreakdown_SoldNeedStillCountsAsMissionPurchaseButNotEndingHappiness; pernyataan/deklarasi berikut berada di dalam
    // batas blok ini dalam ComputeBreakdown_SoldNeedStillCountsAsMissionPurchaseButNotEndingHappiness.
    {
        // Menyiapkan variabel lokal `playerEvents` untuk nilai pemain event dengan objek baru bertipe `List<EventDb>` dengan nilai awal sesuai
        // konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var playerEvents = new List<EventDb>
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ComputeBreakdown_SoldNeedStillCountsAsMissionPurchaseButNotEndingHappiness.
        {
            // Melanjutkan pengolahan dengan memanggil `BuildEvent` dengan `”SetupMisiAwal”`,
            // `”””{”mission_id”:”mission-bike”,”target_tertiary_card_id”:”tertiary-bike”,”penalty_points”:10,”require_primary”:true,”require_secondary”:true}””
            // ”` dalam ComputeBreakdown_SoldNeedStillCountsAsMissionPurchaseButNotEndingHappiness.
            BuildEvent("SetupMisiAwal", """{"mission_id":"mission-bike","target_tertiary_card_id":"tertiary-bike","penalty_points":10,"require_primary":true,"require_secondary":true}"""),
            // Melanjutkan pengolahan dengan memanggil `BuildEvent` dengan `”Kebutuhan”`,
            // `”””{”amount”:2,”card_id”:”primary-food”,”need_tier”:”primer”,”points”:1}”””` dalam
            // ComputeBreakdown_SoldNeedStillCountsAsMissionPurchaseButNotEndingHappiness.
            BuildEvent("Kebutuhan", """{"amount":2,"card_id":"primary-food","need_tier":"primer","points":1}"""),
            // Melanjutkan pengolahan dengan memanggil `BuildEvent` dengan `”Kebutuhan”`,
            // `”””{”amount”:3,”card_id”:”secondary-school”,”need_tier”:”sekunder”,”points”:2}”””` dalam
            // ComputeBreakdown_SoldNeedStillCountsAsMissionPurchaseButNotEndingHappiness.
            BuildEvent("Kebutuhan", """{"amount":3,"card_id":"secondary-school","need_tier":"sekunder","points":2}"""),
            // Melanjutkan pengolahan dengan memanggil `BuildEvent` dengan `”Kebutuhan”`,
            // `”””{”amount”:4,”card_id”:”tertiary-bike”,”need_tier”:”tersier”,”points”:3}”””` dalam
            // ComputeBreakdown_SoldNeedStillCountsAsMissionPurchaseButNotEndingHappiness.
            BuildEvent("Kebutuhan", """{"amount":4,"card_id":"tertiary-bike","need_tier":"tersier","points":3}"""),
            // Melanjutkan pengolahan dengan memanggil `BuildEvent` dengan `”GunakanOpsiDarurat”`,
            // `”””{”option_type”:”SELL_NEED”,”direction”:”IN”,”amount”:2,”card_id”:”tertiary-bike”}”””` dalam
            // ComputeBreakdown_SoldNeedStillCountsAsMissionPurchaseButNotEndingHappiness.
            BuildEvent("GunakanOpsiDarurat", """{"option_type":"SELL_NEED","direction":"IN","amount":2,"card_id":"tertiary-bike"}""")
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // ComputeBreakdown_SoldNeedStillCountsAsMissionPurchaseButNotEndingHappiness.
        };

        // Menyiapkan variabel lokal `breakdown` untuk nilai breakdown dengan memanggil `new HappinessCalculator().ComputeBreakdown` dengan `playerEvents`,
        // `0`, `0`, `0`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var breakdown = new HappinessCalculator().ComputeBreakdown(
            // Meneruskan `playerEvents` (nilai pemain event) sebagai argumen ke `new HappinessCalculator().ComputeBreakdown`.
            playerEvents,
            // Meneruskan nilai literal `0` sebagai argumen bernama `donationPoints`.
            donationPoints: 0,
            // Meneruskan nilai literal `0` sebagai argumen bernama `goldPoints`.
            goldPoints: 0,
            // Meneruskan nilai literal `0` sebagai argumen bernama `pensionPoints`.
            pensionPoints: 0);

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`3`, `breakdown.NeedPoints`); pengujian gagal
        // jika keduanya berbeda dalam ComputeBreakdown_SoldNeedStillCountsAsMissionPurchaseButNotEndingHappiness.
        Assert.Equal(3, breakdown.NeedPoints);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`0`, `breakdown.NeedSetBonusPoints`); pengujian
        // gagal jika keduanya berbeda dalam ComputeBreakdown_SoldNeedStillCountsAsMissionPurchaseButNotEndingHappiness.
        Assert.Equal(0, breakdown.NeedSetBonusPoints);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`0`, `breakdown.MissionPenaltyPoints`);
        // pengujian gagal jika keduanya berbeda dalam ComputeBreakdown_SoldNeedStillCountsAsMissionPurchaseButNotEndingHappiness.
        Assert.Equal(0, breakdown.MissionPenaltyPoints);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`3`, `breakdown.Total`); pengujian gagal jika
        // keduanya berbeda dalam ComputeBreakdown_SoldNeedStillCountsAsMissionPurchaseButNotEndingHappiness.
        Assert.Equal(3, breakdown.Total);
    // Menutup scope metode ComputeBreakdown_SoldNeedStillCountsAsMissionPurchaseButNotEndingHappiness; bagian berikut berada di luar batas blok
    // tersebut dalam ComputeBreakdown_SoldNeedStillCountsAsMissionPurchaseButNotEndingHappiness.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `ComputeBreakdown_SuppressesSavingGoalPoints_WhenLoanIsUnpaid` dengan hasil bertipe `void`; operasi ini menangani compute
    // breakdown suppresses tabungan target poin when pinjaman berstatus unpaid.
    public void ComputeBreakdown_SuppressesSavingGoalPoints_WhenLoanIsUnpaid()
    // Membuka scope metode ComputeBreakdown_SuppressesSavingGoalPoints_WhenLoanIsUnpaid; pernyataan/deklarasi berikut berada di dalam batas blok ini
    // dalam ComputeBreakdown_SuppressesSavingGoalPoints_WhenLoanIsUnpaid.
    {
        // Menyiapkan variabel lokal `playerEvents` untuk nilai pemain event dengan objek baru bertipe `List<EventDb>` dengan nilai awal sesuai
        // konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var playerEvents = new List<EventDb>
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ComputeBreakdown_SuppressesSavingGoalPoints_WhenLoanIsUnpaid.
        {
            // Melanjutkan pengolahan dengan memanggil `BuildEvent` dengan `”TujuanFinansial”`, `”””{”goal_id”:”bike”,”points”:8,”cost”:5}”””` dalam
            // ComputeBreakdown_SuppressesSavingGoalPoints_WhenLoanIsUnpaid.
            BuildEvent("TujuanFinansial", """{"goal_id":"bike","points":8,"cost":5}"""),
            // Melanjutkan pengolahan dengan memanggil `BuildEvent` dengan `”PinjamanSyariah”`, `”””{”loan_id”:”loan-1”,”principal”:10,”penalty_points”:15}”””`
            // dalam ComputeBreakdown_SuppressesSavingGoalPoints_WhenLoanIsUnpaid.
            BuildEvent("PinjamanSyariah", """{"loan_id":"loan-1","principal":10,"penalty_points":15}"""),
            // Melanjutkan pengolahan dengan memanggil `BuildEvent` dengan `”BayarPinjaman”`, `”””{”loan_id”:”loan-1”,”amount”:4}”””` dalam
            // ComputeBreakdown_SuppressesSavingGoalPoints_WhenLoanIsUnpaid.
            BuildEvent("BayarPinjaman", """{"loan_id":"loan-1","amount":4}""")
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // ComputeBreakdown_SuppressesSavingGoalPoints_WhenLoanIsUnpaid.
        };

        // Menyiapkan variabel lokal `breakdown` untuk nilai breakdown dengan memanggil `new HappinessCalculator().ComputeBreakdown` dengan `playerEvents`,
        // `0`, `0`, `0`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var breakdown = new HappinessCalculator().ComputeBreakdown(
            // Meneruskan `playerEvents` (nilai pemain event) sebagai argumen ke `new HappinessCalculator().ComputeBreakdown`.
            playerEvents,
            // Meneruskan nilai literal `0` sebagai argumen bernama `donationPoints`.
            donationPoints: 0,
            // Meneruskan nilai literal `0` sebagai argumen bernama `goldPoints`.
            goldPoints: 0,
            // Meneruskan nilai literal `0` sebagai argumen bernama `pensionPoints`.
            pensionPoints: 0);

        // Menjalankan pemeriksaan bahwa `breakdown.HasUnpaidLoan` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // ComputeBreakdown_SuppressesSavingGoalPoints_WhenLoanIsUnpaid.
        Assert.True(breakdown.HasUnpaidLoan);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`0`, `breakdown.SavingGoalPointsEffective`);
        // pengujian gagal jika keduanya berbeda dalam ComputeBreakdown_SuppressesSavingGoalPoints_WhenLoanIsUnpaid.
        Assert.Equal(0, breakdown.SavingGoalPointsEffective);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`15`, `breakdown.LoanPenaltyPoints`); pengujian
        // gagal jika keduanya berbeda dalam ComputeBreakdown_SuppressesSavingGoalPoints_WhenLoanIsUnpaid.
        Assert.Equal(15, breakdown.LoanPenaltyPoints);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`-15`, `breakdown.Total`); pengujian gagal jika
        // keduanya berbeda dalam ComputeBreakdown_SuppressesSavingGoalPoints_WhenLoanIsUnpaid.
        Assert.Equal(-15, breakdown.Total);
    // Menutup scope metode ComputeBreakdown_SuppressesSavingGoalPoints_WhenLoanIsUnpaid; bagian berikut berada di luar batas blok tersebut dalam
    // ComputeBreakdown_SuppressesSavingGoalPoints_WhenLoanIsUnpaid.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `ComputeByPlayer_UsesConfiguredDonationTieBreakerAndGoldQuantity` dengan hasil bertipe `void`; operasi ini menangani
    // compute berdasarkan pemain uses configured donasi tie breaker dan emas jumlah.
    public void ComputeByPlayer_UsesConfiguredDonationTieBreakerAndGoldQuantity()
    // Membuka scope metode ComputeByPlayer_UsesConfiguredDonationTieBreakerAndGoldQuantity; pernyataan/deklarasi berikut berada di dalam batas blok ini
    // dalam ComputeByPlayer_UsesConfiguredDonationTieBreakerAndGoldQuantity.
    {
        // Menyiapkan variabel lokal `firstPlayerId` untuk nilai first pemain identitas dengan memanggil `Guid.Parse` dengan
        // `”aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var firstPlayerId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        // Menyiapkan variabel lokal `secondPlayerId` untuk nilai second pemain identitas dengan memanggil `Guid.Parse` dengan
        // `”bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var secondPlayerId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
        // Menyiapkan variabel lokal `events` untuk kumpulan event permainan sebagai sumber riwayat untuk validasi atau perhitungan dengan objek baru
        // bertipe `List<EventDb>` dengan nilai awal sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var events = new List<EventDb>
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ComputeByPlayer_UsesConfiguredDonationTieBreakerAndGoldQuantity.
        {
            // Melanjutkan pengolahan dengan memanggil `BuildEvent` dengan `firstPlayerId`, `”JumatBerkah”`, `”””{”amount”:5}”””`, `4`, `1` dalam
            // ComputeByPlayer_UsesConfiguredDonationTieBreakerAndGoldQuantity.
            BuildEvent(firstPlayerId, "JumatBerkah", """{"amount":5}""", dayIndex: 4, sequenceNumber: 1),
            // Melanjutkan pengolahan dengan memanggil `BuildEvent` dengan `secondPlayerId`, `”JumatBerkah”`, `”””{”amount”:5}”””`, `4`, `2` dalam
            // ComputeByPlayer_UsesConfiguredDonationTieBreakerAndGoldQuantity.
            BuildEvent(secondPlayerId, "JumatBerkah", """{"amount":5}""", dayIndex: 4, sequenceNumber: 2),
            // Melanjutkan pengolahan dengan memanggil `BuildEvent` dengan `firstPlayerId`, `”BagikanTieBreaker”`, `”””{”number”:1}”””`, `4`, `3` dalam
            // ComputeByPlayer_UsesConfiguredDonationTieBreakerAndGoldQuantity.
            BuildEvent(firstPlayerId, "BagikanTieBreaker", """{"number":1}""", dayIndex: 4, sequenceNumber: 3),
            // Melanjutkan pengolahan dengan memanggil `BuildEvent` dengan `secondPlayerId`, `”BagikanTieBreaker”`, `”””{”number”:9}”””`, `4`, `4` dalam
            // ComputeByPlayer_UsesConfiguredDonationTieBreakerAndGoldQuantity.
            BuildEvent(secondPlayerId, "BagikanTieBreaker", """{"number":9}""", dayIndex: 4, sequenceNumber: 4),
            // Melanjutkan pengolahan dengan memanggil `BuildEvent` dengan `firstPlayerId`, `”InvestasiEmas”`,
            // `”””{”trade_type”:”BUY”,”qty”:2,”unit_price”:5,”amount”:10}”””`, `5`, `5` dalam ComputeByPlayer_UsesConfiguredDonationTieBreakerAndGoldQuantity.
            BuildEvent(firstPlayerId, "InvestasiEmas", """{"trade_type":"BUY","qty":2,"unit_price":5,"amount":10}""", dayIndex: 5, sequenceNumber: 5)
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // ComputeByPlayer_UsesConfiguredDonationTieBreakerAndGoldQuantity.
        };

        // Menyiapkan variabel lokal `config` untuk konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan dengan memanggil `BuildConfig`
        // dengan `new RulesetScoringConfig( [new RankPoint(1, 7), new RankPoint(2, 5)], [new QtyPoint(1, 3), new QtyPoint(2, 5)], [])`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var config = BuildConfig(new RulesetScoringConfig(
            // Meneruskan koleksi berisi new RankPoint(1, 7), new RankPoint(2, 5) sebagai argumen ke konstruktor `RulesetScoringConfig`; Meneruskan nilai
            // literal `1` sebagai argumen ke konstruktor `RankPoint`; Meneruskan nilai literal `7` sebagai argumen ke konstruktor `RankPoint`; Meneruskan nilai
            // literal `2` sebagai argumen ke konstruktor `RankPoint`; Meneruskan nilai literal `5` sebagai argumen ke konstruktor `RankPoint`.
            [new RankPoint(1, 7), new RankPoint(2, 5)],
            // Meneruskan koleksi berisi new QtyPoint(1, 3), new QtyPoint(2, 5) sebagai argumen ke konstruktor `RulesetScoringConfig`; Meneruskan nilai literal
            // `1` sebagai argumen ke konstruktor `QtyPoint`; Meneruskan nilai literal `3` sebagai argumen ke konstruktor `QtyPoint`; Meneruskan nilai literal
            // `2` sebagai argumen ke konstruktor `QtyPoint`; Meneruskan nilai literal `5` sebagai argumen ke konstruktor `QtyPoint`.
            [new QtyPoint(1, 3), new QtyPoint(2, 5)],
            // Meneruskan koleksi kosong dengan tipe mengikuti konteks tujuan sebagai argumen ke konstruktor `RulesetScoringConfig`.
            []));

        // Menyiapkan variabel lokal `byPlayer` untuk nilai berdasarkan pemain dengan memanggil `new HappinessCalculator().ComputeByPlayer` dengan `events`,
        // `[]`, `config`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var byPlayer = new HappinessCalculator().ComputeByPlayer(events, [], config);

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`10`, `byPlayer[firstPlayerId].Total`);
        // pengujian gagal jika keduanya berbeda dalam ComputeByPlayer_UsesConfiguredDonationTieBreakerAndGoldQuantity.
        Assert.Equal(10, byPlayer[firstPlayerId].Total);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`5`, `byPlayer[firstPlayerId].DonationPoints`);
        // pengujian gagal jika keduanya berbeda dalam ComputeByPlayer_UsesConfiguredDonationTieBreakerAndGoldQuantity.
        Assert.Equal(5, byPlayer[firstPlayerId].DonationPoints);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`5`, `byPlayer[firstPlayerId].GoldPoints`);
        // pengujian gagal jika keduanya berbeda dalam ComputeByPlayer_UsesConfiguredDonationTieBreakerAndGoldQuantity.
        Assert.Equal(5, byPlayer[firstPlayerId].GoldPoints);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`7`, `byPlayer[secondPlayerId].Total`);
        // pengujian gagal jika keduanya berbeda dalam ComputeByPlayer_UsesConfiguredDonationTieBreakerAndGoldQuantity.
        Assert.Equal(7, byPlayer[secondPlayerId].Total);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`7`,
        // `byPlayer[secondPlayerId].DonationPoints`); pengujian gagal jika keduanya berbeda dalam
        // ComputeByPlayer_UsesConfiguredDonationTieBreakerAndGoldQuantity.
        Assert.Equal(7, byPlayer[secondPlayerId].DonationPoints);
    // Menutup scope metode ComputeByPlayer_UsesConfiguredDonationTieBreakerAndGoldQuantity; bagian berikut berada di luar batas blok tersebut dalam
    // ComputeByPlayer_UsesConfiguredDonationTieBreakerAndGoldQuantity.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `ComputeByPlayer_CapsGoldScoreAtHighestRulebookTier` dengan hasil bertipe `void`; operasi ini menangani compute berdasarkan
    // pemain caps emas skor at highest rulebook tingkat.
    public void ComputeByPlayer_CapsGoldScoreAtHighestRulebookTier()
    // Membuka scope metode ComputeByPlayer_CapsGoldScoreAtHighestRulebookTier; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // ComputeByPlayer_CapsGoldScoreAtHighestRulebookTier.
    {
        // Menyiapkan variabel lokal `playerId` untuk nilai pemain identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var playerId = Guid.NewGuid();
        // Menyiapkan variabel lokal `events` untuk kumpulan event permainan sebagai sumber riwayat untuk validasi atau perhitungan dengan objek baru
        // bertipe `List<EventDb>` dengan nilai awal sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var events = new List<EventDb>
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ComputeByPlayer_CapsGoldScoreAtHighestRulebookTier.
        {
            // Melanjutkan pengolahan dengan memanggil `BuildEvent` dengan `playerId`, `”InvestasiEmas”`,
            // `”””{”trade_type”:”BUY”,”qty”:5,”unit_price”:5,”amount”:25}”””`, `5`, `1` dalam ComputeByPlayer_CapsGoldScoreAtHighestRulebookTier.
            BuildEvent(playerId, "InvestasiEmas", """{"trade_type":"BUY","qty":5,"unit_price":5,"amount":25}""", dayIndex: 5, sequenceNumber: 1)
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // ComputeByPlayer_CapsGoldScoreAtHighestRulebookTier.
        };
        // Menyiapkan variabel lokal `config` untuk konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan dengan memanggil `BuildConfig`
        // dengan `new RulesetScoringConfig( [], [new QtyPoint(1, 3), new QtyPoint(2, 5), new QtyPoint(3, 8), new QtyPoint(4, 12)], [])`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var config = BuildConfig(new RulesetScoringConfig(
            // Meneruskan koleksi kosong dengan tipe mengikuti konteks tujuan sebagai argumen ke konstruktor `RulesetScoringConfig`.
            [],
            // Meneruskan koleksi berisi new QtyPoint(1, 3), new QtyPoint(2, 5), new QtyPoint(3, 8), new QtyPoint(4, 12) sebagai argumen ke konstruktor
            // `RulesetScoringConfig`; Meneruskan nilai literal `1` sebagai argumen ke konstruktor `QtyPoint`; Meneruskan nilai literal `3` sebagai argumen ke
            // konstruktor `QtyPoint`; Meneruskan nilai literal `2` sebagai argumen ke konstruktor `QtyPoint`; Meneruskan nilai literal `5` sebagai argumen ke
            // konstruktor `QtyPoint`; Meneruskan nilai literal `3` sebagai argumen ke konstruktor `QtyPoint`; Meneruskan nilai literal `8` sebagai argumen ke
            // konstruktor `QtyPoint`; Meneruskan nilai literal `4` sebagai argumen ke konstruktor `QtyPoint`; Meneruskan nilai literal `12` sebagai argumen ke
            // konstruktor `QtyPoint`.
            [new QtyPoint(1, 3), new QtyPoint(2, 5), new QtyPoint(3, 8), new QtyPoint(4, 12)],
            // Meneruskan koleksi kosong dengan tipe mengikuti konteks tujuan sebagai argumen ke konstruktor `RulesetScoringConfig`.
            []));

        // Menyiapkan variabel lokal `byPlayer` untuk nilai berdasarkan pemain dengan memanggil `new HappinessCalculator().ComputeByPlayer` dengan `events`,
        // `[]`, `config`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var byPlayer = new HappinessCalculator().ComputeByPlayer(events, [], config);

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`12`, `byPlayer[playerId].GoldPoints`);
        // pengujian gagal jika keduanya berbeda dalam ComputeByPlayer_CapsGoldScoreAtHighestRulebookTier.
        Assert.Equal(12, byPlayer[playerId].GoldPoints);
    // Menutup scope metode ComputeByPlayer_CapsGoldScoreAtHighestRulebookTier; bagian berikut berada di luar batas blok tersebut dalam
    // ComputeByPlayer_CapsGoldScoreAtHighestRulebookTier.
    }

    // Mendefinisikan metode `BuildEvent` dengan hasil bertipe `EventDb`; operasi ini menangani build event. Masukan: Parameter `actionType` bertipe
    // `string` membawa nilai aksi jenis; Parameter `payload` bertipe `string` membawa muatan detail event dalam format JSON.
    private static EventDb BuildEvent(string actionType, string payload)
    // Membuka scope metode BuildEvent; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildEvent.
    {
        // Mengembalikan memanggil `BuildEvent` dengan `Guid.NewGuid()`, `actionType`, `payload`, `0`, `1` kepada pemanggil dalam BuildEvent; eksekusi jalur
        // ini selesai setelah nilai hasil ditentukan.
        return BuildEvent(Guid.NewGuid(), actionType, payload, dayIndex: 0, sequenceNumber: 1);
    // Menutup scope metode BuildEvent; bagian berikut berada di luar batas blok tersebut dalam BuildEvent.
    }

    // Mendefinisikan metode `BuildEvent` dengan hasil bertipe `EventDb`; operasi ini menangani build event. Masukan: Parameter `playerId` bertipe
    // `Guid` membawa nilai pemain identitas; Parameter `actionType` bertipe `string` membawa nilai aksi jenis; Parameter `payload` bertipe `string`
    // membawa muatan detail event dalam format JSON; Parameter `dayIndex` bertipe `int` membawa nilai hari index; Parameter `sequenceNumber` bertipe
    // `long` membawa nomor urut event yang menentukan urutan pemrosesan riwayat permainan.
    private static EventDb BuildEvent(Guid playerId, string actionType, string payload, int dayIndex, long sequenceNumber)
    // Membuka scope metode BuildEvent; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildEvent.
    {
        // Mengembalikan objek baru bertipe `EventDb` dengan nilai awal sesuai konstruktornya kepada pemanggil dalam BuildEvent; eksekusi jalur ini selesai
        // setelah nilai hasil ditentukan.
        return new EventDb
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildEvent.
        {
            // Memperbarui `EventId` menggunakan memanggil `Guid.NewGuid` dengan tanpa argumen dalam BuildEvent.
            EventId = Guid.NewGuid(),
            // Memperbarui `SessionId` menggunakan memanggil `Guid.NewGuid` dengan tanpa argumen dalam BuildEvent.
            SessionId = Guid.NewGuid(),
            // Memperbarui `UserId` menggunakan `playerId` (nilai pemain identitas) dalam BuildEvent.
            UserId = playerId,
            // Memperbarui `ActionType` menggunakan `actionType` (nilai aksi jenis) dalam BuildEvent.
            ActionType = actionType,
            // Memperbarui `Payload` menggunakan `payload` (muatan detail event dalam format JSON) dalam BuildEvent.
            Payload = payload,
            // Memperbarui `DayIndex` menggunakan `dayIndex` (nilai hari index) dalam BuildEvent.
            DayIndex = dayIndex,
            // Memperbarui `SequenceNumber` menggunakan `sequenceNumber` (nomor urut event yang menentukan urutan pemrosesan riwayat permainan) dalam
            // BuildEvent.
            SequenceNumber = sequenceNumber
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam BuildEvent.
        };
    // Menutup scope metode BuildEvent; bagian berikut berada di luar batas blok tersebut dalam BuildEvent.
    }

    // Mendefinisikan metode `BuildConfig` dengan hasil bertipe `RulesetConfig`; operasi ini menangani build konfigurasi. Masukan: Parameter `scoring`
    // bertipe `RulesetScoringConfig` membawa nilai scoring.
    private static RulesetConfig BuildConfig(RulesetScoringConfig scoring)
    // Membuka scope metode BuildConfig; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildConfig.
    {
        // Mengembalikan objek baru bertipe `RulesetConfig` dengan argumen ( ”PEMULA”, 2, 20, PlayerOrdering.PlayerOrder, 0, 6, 3, 1, true, true, true,
        // true, 1, 999, true, true, false, false, false, 1, scoring) kepada pemanggil dalam BuildConfig; eksekusi jalur ini selesai setelah nilai hasil
        // ditentukan.
        return new RulesetConfig(
            // Meneruskan nilai literal `”PEMULA”` sebagai argumen ke konstruktor `RulesetConfig`.
            "PEMULA",
            // Meneruskan nilai literal `2` sebagai argumen ke konstruktor `RulesetConfig`.
            2,
            // Meneruskan nilai literal `20` sebagai argumen ke konstruktor `RulesetConfig`.
            20,
            // Meneruskan `PlayerOrdering.PlayerOrder` (nomor urut pemain untuk menentukan urutan tindakan) sebagai argumen ke konstruktor `RulesetConfig`.
            PlayerOrdering.PlayerOrder,
            // Meneruskan nilai literal `0` sebagai argumen ke konstruktor `RulesetConfig`.
            0,
            // Meneruskan nilai literal `6` sebagai argumen ke konstruktor `RulesetConfig`.
            6,
            // Meneruskan nilai literal `3` sebagai argumen ke konstruktor `RulesetConfig`.
            3,
            // Meneruskan nilai literal `1` sebagai argumen ke konstruktor `RulesetConfig`.
            1,
            // Meneruskan true, yaitu kondisi aktif/terpenuhi sebagai argumen ke konstruktor `RulesetConfig`.
            true,
            // Meneruskan true, yaitu kondisi aktif/terpenuhi sebagai argumen ke konstruktor `RulesetConfig`.
            true,
            // Meneruskan true, yaitu kondisi aktif/terpenuhi sebagai argumen ke konstruktor `RulesetConfig`.
            true,
            // Meneruskan true, yaitu kondisi aktif/terpenuhi sebagai argumen ke konstruktor `RulesetConfig`.
            true,
            // Meneruskan nilai literal `1` sebagai argumen ke konstruktor `RulesetConfig`.
            1,
            // Meneruskan nilai literal `999` sebagai argumen ke konstruktor `RulesetConfig`.
            999,
            // Meneruskan true, yaitu kondisi aktif/terpenuhi sebagai argumen ke konstruktor `RulesetConfig`.
            true,
            // Meneruskan true, yaitu kondisi aktif/terpenuhi sebagai argumen ke konstruktor `RulesetConfig`.
            true,
            // Meneruskan false, yaitu kondisi nonaktif/tidak terpenuhi sebagai argumen ke konstruktor `RulesetConfig`.
            false,
            // Meneruskan false, yaitu kondisi nonaktif/tidak terpenuhi sebagai argumen ke konstruktor `RulesetConfig`.
            false,
            // Meneruskan false, yaitu kondisi nonaktif/tidak terpenuhi sebagai argumen ke konstruktor `RulesetConfig`.
            false,
            // Meneruskan nilai literal `1` sebagai argumen ke konstruktor `RulesetConfig`.
            1,
            // Meneruskan `scoring` (nilai scoring) sebagai argumen ke konstruktor `RulesetConfig`.
            scoring);
    // Menutup scope metode BuildConfig; bagian berikut berada di luar batas blok tersebut dalam BuildConfig.
    }
// Menutup scope tipe AnalyticsHappinessCalculatorTests; bagian berikut berada di luar batas blok tersebut.
}
