// Fungsi file: Memverifikasi perilaku API, database, atau domain melalui AnalyticsNeedMissionCalculatorTests.
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;
// Mengimpor namespace `Cashflowpoly.Api.Domain` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Domain;
// Mengimpor namespace `Xunit` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Xunit;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Tests` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Tests;

// Mendefinisikan tipe class `AnalyticsNeedMissionCalculatorTests`; sealed mencegah tipe ini diturunkan lagi.
public sealed class AnalyticsNeedMissionCalculatorTests
// Membuka scope tipe AnalyticsNeedMissionCalculatorTests; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `Compute_SummarizesNeedProfileMissionAndNeedSpending` dengan hasil bertipe `void`; operasi ini menangani compute summarizes
    // kebutuhan profile misi dan kebutuhan spending.
    public void Compute_SummarizesNeedProfileMissionAndNeedSpending()
    // Membuka scope metode Compute_SummarizesNeedProfileMissionAndNeedSpending; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // Compute_SummarizesNeedProfileMissionAndNeedSpending.
    {
        // Menyiapkan variabel lokal `playerId` untuk nilai pemain identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var playerId = Guid.NewGuid();
        // Menyiapkan variabel lokal `sessionId` untuk identitas unik sesi permainan yang menjadi batas data operasi ini dengan memanggil `Guid.NewGuid`
        // dengan tanpa argumen. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var sessionId = Guid.NewGuid();
        // Menyiapkan variabel lokal `events` untuk kumpulan event permainan sebagai sumber riwayat untuk validasi atau perhitungan dengan objek baru
        // bertipe `List<EventDb>` dengan nilai awal sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var events = new List<EventDb>
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // Compute_SummarizesNeedProfileMissionAndNeedSpending.
        {
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `playerId`, `sessionId`, `”Kebutuhan”`,
            // `”””{”card_id”:”rice”,”amount”:3,”points”:1}”””` dalam Compute_SummarizesNeedProfileMissionAndNeedSpending.
            CreateEvent(playerId, sessionId, "Kebutuhan", """{"card_id":"rice","amount":3,"points":1}"""),
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `playerId`, `sessionId`, `”Kebutuhan”`,
            // `”””{”card_id”:”book”,”amount”:4,”points”:1}”””` dalam Compute_SummarizesNeedProfileMissionAndNeedSpending.
            CreateEvent(playerId, sessionId, "Kebutuhan", """{"card_id":"book","amount":4,"points":1}"""),
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `playerId`, `sessionId`, `”Kebutuhan”`,
            // `”””{”card_id”:”bike”,”amount”:7,”points”:2}”””` dalam Compute_SummarizesNeedProfileMissionAndNeedSpending.
            CreateEvent(playerId, sessionId, "Kebutuhan", """{"card_id":"bike","amount":7,"points":2}"""),
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `playerId`, `sessionId`, `”SetupMisiAwal”`,
            // `”””{”mission_id”:”mission-1”,”target_tertiary_card_id”:”bike”,”penalty_points”:10,”require_primary”:true,”require_secondary”:true}”””` dalam
            // Compute_SummarizesNeedProfileMissionAndNeedSpending.
            CreateEvent(playerId, sessionId, "SetupMisiAwal", """{"mission_id":"mission-1","target_tertiary_card_id":"bike","penalty_points":10,"require_primary":true,"require_secondary":true}""")
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // Compute_SummarizesNeedProfileMissionAndNeedSpending.
        };
        // Menyiapkan variabel lokal `projections` untuk proyeksi transaksi arus kas yang diturunkan dari event permainan dengan objek baru bertipe
        // `List<CashflowProjectionDb>` dengan nilai awal sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var projections = new List<CashflowProjectionDb>
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // Compute_SummarizesNeedProfileMissionAndNeedSpending.
        {
            // Melanjutkan pengolahan dengan memanggil `CreateProjection` dengan `sessionId`, `playerId`, `”OUT”`, `3`, `”NEED_PRIMARY”` dalam
            // Compute_SummarizesNeedProfileMissionAndNeedSpending.
            CreateProjection(sessionId, playerId, "OUT", 3, "NEED_PRIMARY"),
            // Melanjutkan pengolahan dengan memanggil `CreateProjection` dengan `sessionId`, `playerId`, `”OUT”`, `4`, `”NEED_SECONDARY”` dalam
            // Compute_SummarizesNeedProfileMissionAndNeedSpending.
            CreateProjection(sessionId, playerId, "OUT", 4, "NEED_SECONDARY"),
            // Melanjutkan pengolahan dengan memanggil `CreateProjection` dengan `sessionId`, `playerId`, `”OUT”`, `7`, `”NEED_TERTIARY”` dalam
            // Compute_SummarizesNeedProfileMissionAndNeedSpending.
            CreateProjection(sessionId, playerId, "OUT", 7, "NEED_TERTIARY")
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // Compute_SummarizesNeedProfileMissionAndNeedSpending.
        };

        // Menyiapkan variabel lokal `metrics` untuk nilai metrics dengan memanggil `new NeedMissionCalculator().Compute` dengan `events`, `projections`.
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var metrics = new NeedMissionCalculator().Compute(events, projections);

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`3`, `metrics.NeedCardsPurchased`); pengujian
        // gagal jika keduanya berbeda dalam Compute_SummarizesNeedProfileMissionAndNeedSpending.
        Assert.Equal(3, metrics.NeedCardsPurchased);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`3`, `metrics.NeedCardsOwnedCurrent`);
        // pengujian gagal jika keduanya berbeda dalam Compute_SummarizesNeedProfileMissionAndNeedSpending.
        Assert.Equal(3, metrics.NeedCardsOwnedCurrent);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`1`, `metrics.PrimaryNeeds`); pengujian gagal
        // jika keduanya berbeda dalam Compute_SummarizesNeedProfileMissionAndNeedSpending.
        Assert.Equal(1, metrics.PrimaryNeeds);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`1`, `metrics.SecondaryNeeds`); pengujian gagal
        // jika keduanya berbeda dalam Compute_SummarizesNeedProfileMissionAndNeedSpending.
        Assert.Equal(1, metrics.SecondaryNeeds);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`1`, `metrics.TertiaryNeeds`); pengujian gagal
        // jika keduanya berbeda dalam Compute_SummarizesNeedProfileMissionAndNeedSpending.
        Assert.Equal(1, metrics.TertiaryNeeds);
        // Menjalankan pemeriksaan bahwa `metrics.HasBasicNeedProfile` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // Compute_SummarizesNeedProfileMissionAndNeedSpending.
        Assert.True(metrics.HasBasicNeedProfile);
        // Menjalankan pemeriksaan bahwa `metrics.IsCollectorNeedProfile` bernilai salah; pengujian gagal jika kondisi justru terpenuhi dalam
        // Compute_SummarizesNeedProfileMissionAndNeedSpending.
        Assert.False(metrics.IsCollectorNeedProfile);
        // Menjalankan pemeriksaan bahwa `metrics.IsSpecialistNeedProfile` bernilai salah; pengujian gagal jika kondisi justru terpenuhi dalam
        // Compute_SummarizesNeedProfileMissionAndNeedSpending.
        Assert.False(metrics.IsSpecialistNeedProfile);
        // Menjalankan pemeriksaan bahwa `metrics.SpecificTertiaryAcquired` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // Compute_SummarizesNeedProfileMissionAndNeedSpending.
        Assert.True(metrics.SpecificTertiaryAcquired);
        // Menjalankan pemeriksaan bahwa `metrics.CollectionMissionComplete` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // Compute_SummarizesNeedProfileMissionAndNeedSpending.
        Assert.True(metrics.CollectionMissionComplete);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`14`, `metrics.NeedCoinsSpent`); pengujian
        // gagal jika keduanya berbeda dalam Compute_SummarizesNeedProfileMissionAndNeedSpending.
        Assert.Equal(14, metrics.NeedCoinsSpent);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`1`, `metrics.FulfillmentDiversity`); pengujian
        // gagal jika keduanya berbeda dalam Compute_SummarizesNeedProfileMissionAndNeedSpending.
        Assert.Equal(1, metrics.FulfillmentDiversity);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`Math.Sqrt(3) / 3`,
        // `metrics.FulfillmentDiversityDocumentFormula!.Value`, `12`); pengujian gagal jika keduanya berbeda dalam
        // Compute_SummarizesNeedProfileMissionAndNeedSpending.
        Assert.Equal(Math.Sqrt(3) / 3, metrics.FulfillmentDiversityDocumentFormula!.Value, precision: 12);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`1`, `metrics.MissionAchievement`); pengujian
        // gagal jika keduanya berbeda dalam Compute_SummarizesNeedProfileMissionAndNeedSpending.
        Assert.Equal(1, metrics.MissionAchievement);
    // Menutup scope metode Compute_SummarizesNeedProfileMissionAndNeedSpending; bagian berikut berada di luar batas blok tersebut dalam
    // Compute_SummarizesNeedProfileMissionAndNeedSpending.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `Compute_SpecialistProfileWhenOneNeedDominates` dengan hasil bertipe `void`; operasi ini menangani compute specialist
    // profile when one kebutuhan dominates.
    public void Compute_SpecialistProfileWhenOneNeedDominates()
    // Membuka scope metode Compute_SpecialistProfileWhenOneNeedDominates; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // Compute_SpecialistProfileWhenOneNeedDominates.
    {
        // Menyiapkan variabel lokal `playerId` untuk nilai pemain identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var playerId = Guid.NewGuid();
        // Menyiapkan variabel lokal `sessionId` untuk identitas unik sesi permainan yang menjadi batas data operasi ini dengan memanggil `Guid.NewGuid`
        // dengan tanpa argumen. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var sessionId = Guid.NewGuid();
        // Menyiapkan variabel lokal `events` untuk kumpulan event permainan sebagai sumber riwayat untuk validasi atau perhitungan dengan objek baru
        // bertipe `List<EventDb>` dengan nilai awal sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var events = new List<EventDb>
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // Compute_SpecialistProfileWhenOneNeedDominates.
        {
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `playerId`, `sessionId`, `”Kebutuhan”`,
            // `”””{”card_id”:”rice-1”,”amount”:1,”points”:1}”””` dalam Compute_SpecialistProfileWhenOneNeedDominates.
            CreateEvent(playerId, sessionId, "Kebutuhan", """{"card_id":"rice-1","amount":1,"points":1}"""),
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `playerId`, `sessionId`, `”Kebutuhan”`,
            // `”””{”card_id”:”rice-2”,”amount”:1,”points”:1}”””` dalam Compute_SpecialistProfileWhenOneNeedDominates.
            CreateEvent(playerId, sessionId, "Kebutuhan", """{"card_id":"rice-2","amount":1,"points":1}"""),
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `playerId`, `sessionId`, `”Kebutuhan”`,
            // `”””{”card_id”:”rice-3”,”amount”:1,”points”:1}”””` dalam Compute_SpecialistProfileWhenOneNeedDominates.
            CreateEvent(playerId, sessionId, "Kebutuhan", """{"card_id":"rice-3","amount":1,"points":1}"""),
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `playerId`, `sessionId`, `”Kebutuhan”`,
            // `”””{”card_id”:”book”,”amount”:1,”points”:1}”””` dalam Compute_SpecialistProfileWhenOneNeedDominates.
            CreateEvent(playerId, sessionId, "Kebutuhan", """{"card_id":"book","amount":1,"points":1}""")
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // Compute_SpecialistProfileWhenOneNeedDominates.
        };

        // Menyiapkan variabel lokal `metrics` untuk nilai metrics dengan memanggil `new NeedMissionCalculator().Compute` dengan `events`,
        // `Array.Empty<CashflowProjectionDb>()`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var metrics = new NeedMissionCalculator().Compute(events, Array.Empty<CashflowProjectionDb>());

        // Menjalankan pemeriksaan bahwa `metrics.IsCollectorNeedProfile` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // Compute_SpecialistProfileWhenOneNeedDominates.
        Assert.True(metrics.IsCollectorNeedProfile);
        // Menjalankan pemeriksaan bahwa `metrics.IsSpecialistNeedProfile` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // Compute_SpecialistProfileWhenOneNeedDominates.
        Assert.True(metrics.IsSpecialistNeedProfile);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`4`, `metrics.NeedCardsOwnedCurrent`);
        // pengujian gagal jika keduanya berbeda dalam Compute_SpecialistProfileWhenOneNeedDominates.
        Assert.Equal(4, metrics.NeedCardsOwnedCurrent);
        // Menjalankan pemeriksaan Null atas `metrics.SpecificTertiaryAcquired` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // Compute_SpecialistProfileWhenOneNeedDominates.
        Assert.Null(metrics.SpecificTertiaryAcquired);
        // Menjalankan pemeriksaan Null atas `metrics.CollectionMissionComplete` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // Compute_SpecialistProfileWhenOneNeedDominates.
        Assert.Null(metrics.CollectionMissionComplete);
        // Menjalankan pemeriksaan Null atas `metrics.MissionAchievement` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // Compute_SpecialistProfileWhenOneNeedDominates.
        Assert.Null(metrics.MissionAchievement);
    // Menutup scope metode Compute_SpecialistProfileWhenOneNeedDominates; bagian berikut berada di luar batas blok tersebut dalam
    // Compute_SpecialistProfileWhenOneNeedDominates.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `Compute_NoOwnedNeeds_HasNoFulfillmentDiversity` dengan hasil bertipe `void`; operasi ini menangani compute no dimiliki
    // kebutuhan memiliki no pemenuhan keberagaman.
    public void Compute_NoOwnedNeeds_HasNoFulfillmentDiversity()
    // Membuka scope metode Compute_NoOwnedNeeds_HasNoFulfillmentDiversity; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // Compute_NoOwnedNeeds_HasNoFulfillmentDiversity.
    {
        // Menyiapkan variabel lokal `metrics` untuk nilai metrics dengan memanggil `new NeedMissionCalculator().Compute` dengan `Array.Empty<EventDb>()`,
        // `Array.Empty<CashflowProjectionDb>()`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var metrics = new NeedMissionCalculator().Compute(
            // Meneruskan memanggil `Array.Empty<EventDb>` dengan tanpa argumen sebagai argumen ke `new NeedMissionCalculator().Compute`.
            Array.Empty<EventDb>(),
            // Meneruskan memanggil `Array.Empty<CashflowProjectionDb>` dengan tanpa argumen sebagai argumen ke `new NeedMissionCalculator().Compute`.
            Array.Empty<CashflowProjectionDb>());

        // Menjalankan pemeriksaan Null atas `metrics.FulfillmentDiversity` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // Compute_NoOwnedNeeds_HasNoFulfillmentDiversity.
        Assert.Null(metrics.FulfillmentDiversity);
        // Menjalankan pemeriksaan Null atas `metrics.FulfillmentDiversityDocumentFormula` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // Compute_NoOwnedNeeds_HasNoFulfillmentDiversity.
        Assert.Null(metrics.FulfillmentDiversityDocumentFormula);
    // Menutup scope metode Compute_NoOwnedNeeds_HasNoFulfillmentDiversity; bagian berikut berada di luar batas blok tersebut dalam
    // Compute_NoOwnedNeeds_HasNoFulfillmentDiversity.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `Compute_ExcludesSoldNeedFromCurrentProfileButKeepsCompletedPurchaseMission` dengan hasil bertipe `void`; operasi ini
    // menangani compute excludes terjual kebutuhan dari saat ini profile but keeps selesai pembelian misi.
    public void Compute_ExcludesSoldNeedFromCurrentProfileButKeepsCompletedPurchaseMission()
    // Membuka scope metode Compute_ExcludesSoldNeedFromCurrentProfileButKeepsCompletedPurchaseMission; pernyataan/deklarasi berikut berada di dalam
    // batas blok ini dalam Compute_ExcludesSoldNeedFromCurrentProfileButKeepsCompletedPurchaseMission.
    {
        // Menyiapkan variabel lokal `playerId` untuk nilai pemain identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var playerId = Guid.NewGuid();
        // Menyiapkan variabel lokal `sessionId` untuk identitas unik sesi permainan yang menjadi batas data operasi ini dengan memanggil `Guid.NewGuid`
        // dengan tanpa argumen. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var sessionId = Guid.NewGuid();
        // Menyiapkan variabel lokal `events` untuk kumpulan event permainan sebagai sumber riwayat untuk validasi atau perhitungan dengan objek baru
        // bertipe `List<EventDb>` dengan nilai awal sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var events = new List<EventDb>
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // Compute_ExcludesSoldNeedFromCurrentProfileButKeepsCompletedPurchaseMission.
        {
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `playerId`, `sessionId`, `”Kebutuhan”`,
            // `”””{”card_id”:”rice”,”amount”:3,”points”:1}”””` dalam Compute_ExcludesSoldNeedFromCurrentProfileButKeepsCompletedPurchaseMission.
            CreateEvent(playerId, sessionId, "Kebutuhan", """{"card_id":"rice","amount":3,"points":1}"""),
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `playerId`, `sessionId`, `”Kebutuhan”`,
            // `”””{”card_id”:”book”,”amount”:4,”points”:1}”””` dalam Compute_ExcludesSoldNeedFromCurrentProfileButKeepsCompletedPurchaseMission.
            CreateEvent(playerId, sessionId, "Kebutuhan", """{"card_id":"book","amount":4,"points":1}"""),
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `playerId`, `sessionId`, `”Kebutuhan”`,
            // `”””{”card_id”:”bike”,”amount”:7,”points”:2}”””` dalam Compute_ExcludesSoldNeedFromCurrentProfileButKeepsCompletedPurchaseMission.
            CreateEvent(playerId, sessionId, "Kebutuhan", """{"card_id":"bike","amount":7,"points":2}"""),
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `playerId`, `sessionId`, `”GunakanOpsiDarurat”`,
            // `”””{”option_type”:”SELL_NEED”,”direction”:”IN”,”amount”:3,”card_id”:”bike”}”””` dalam
            // Compute_ExcludesSoldNeedFromCurrentProfileButKeepsCompletedPurchaseMission.
            CreateEvent(playerId, sessionId, "GunakanOpsiDarurat", """{"option_type":"SELL_NEED","direction":"IN","amount":3,"card_id":"bike"}"""),
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `playerId`, `sessionId`, `”SetupMisiAwal”`,
            // `”””{”mission_id”:”mission-1”,”target_tertiary_card_id”:”bike”,”penalty_points”:10,”require_primary”:true,”require_secondary”:true}”””` dalam
            // Compute_ExcludesSoldNeedFromCurrentProfileButKeepsCompletedPurchaseMission.
            CreateEvent(playerId, sessionId, "SetupMisiAwal", """{"mission_id":"mission-1","target_tertiary_card_id":"bike","penalty_points":10,"require_primary":true,"require_secondary":true}""")
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // Compute_ExcludesSoldNeedFromCurrentProfileButKeepsCompletedPurchaseMission.
        };

        // Menyiapkan variabel lokal `metrics` untuk nilai metrics dengan memanggil `new NeedMissionCalculator().Compute` dengan `events`,
        // `Array.Empty<CashflowProjectionDb>()`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var metrics = new NeedMissionCalculator().Compute(events, Array.Empty<CashflowProjectionDb>());

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`3`, `metrics.NeedCardsPurchased`); pengujian
        // gagal jika keduanya berbeda dalam Compute_ExcludesSoldNeedFromCurrentProfileButKeepsCompletedPurchaseMission.
        Assert.Equal(3, metrics.NeedCardsPurchased);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`2`, `metrics.NeedCardsOwnedCurrent`);
        // pengujian gagal jika keduanya berbeda dalam Compute_ExcludesSoldNeedFromCurrentProfileButKeepsCompletedPurchaseMission.
        Assert.Equal(2, metrics.NeedCardsOwnedCurrent);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`1`, `metrics.PrimaryNeeds`); pengujian gagal
        // jika keduanya berbeda dalam Compute_ExcludesSoldNeedFromCurrentProfileButKeepsCompletedPurchaseMission.
        Assert.Equal(1, metrics.PrimaryNeeds);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`1`, `metrics.SecondaryNeeds`); pengujian gagal
        // jika keduanya berbeda dalam Compute_ExcludesSoldNeedFromCurrentProfileButKeepsCompletedPurchaseMission.
        Assert.Equal(1, metrics.SecondaryNeeds);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`0`, `metrics.TertiaryNeeds`); pengujian gagal
        // jika keduanya berbeda dalam Compute_ExcludesSoldNeedFromCurrentProfileButKeepsCompletedPurchaseMission.
        Assert.Equal(0, metrics.TertiaryNeeds);
        // Menjalankan pemeriksaan bahwa `metrics.HasBasicNeedProfile` bernilai salah; pengujian gagal jika kondisi justru terpenuhi dalam
        // Compute_ExcludesSoldNeedFromCurrentProfileButKeepsCompletedPurchaseMission.
        Assert.False(metrics.HasBasicNeedProfile);
        // Menjalankan pemeriksaan bahwa `metrics.SpecificTertiaryAcquired` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // Compute_ExcludesSoldNeedFromCurrentProfileButKeepsCompletedPurchaseMission.
        Assert.True(metrics.SpecificTertiaryAcquired);
        // Menjalankan pemeriksaan bahwa `metrics.CollectionMissionComplete` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // Compute_ExcludesSoldNeedFromCurrentProfileButKeepsCompletedPurchaseMission.
        Assert.True(metrics.CollectionMissionComplete);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`1`, `metrics.MissionAchievement`); pengujian
        // gagal jika keduanya berbeda dalam Compute_ExcludesSoldNeedFromCurrentProfileButKeepsCompletedPurchaseMission.
        Assert.Equal(1, metrics.MissionAchievement);
    // Menutup scope metode Compute_ExcludesSoldNeedFromCurrentProfileButKeepsCompletedPurchaseMission; bagian berikut berada di luar batas blok
    // tersebut dalam Compute_ExcludesSoldNeedFromCurrentProfileButKeepsCompletedPurchaseMission.
    }

    // Mendefinisikan metode `CreateEvent` dengan hasil bertipe `EventDb`; operasi ini menangani create event. Masukan: Parameter `playerId` bertipe
    // `Guid` membawa nilai pemain identitas; Parameter `sessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi
    // ini; Parameter `actionType` bertipe `string` membawa nilai aksi jenis; Parameter `payload` bertipe `string` membawa muatan detail event dalam
    // format JSON.
    private static EventDb CreateEvent(Guid playerId, Guid sessionId, string actionType, string payload)
    // Membuka scope metode CreateEvent; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CreateEvent.
    {
        // Mengembalikan objek baru bertipe `EventDb` dengan nilai awal sesuai konstruktornya kepada pemanggil dalam CreateEvent; eksekusi jalur ini selesai
        // setelah nilai hasil ditentukan.
        return new EventDb
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CreateEvent.
        {
            // Memperbarui `EventId` menggunakan memanggil `Guid.NewGuid` dengan tanpa argumen dalam CreateEvent.
            EventId = Guid.NewGuid(),
            // Memperbarui `SessionId` menggunakan `sessionId` (identitas unik sesi permainan yang menjadi batas data operasi ini) dalam CreateEvent.
            SessionId = sessionId,
            // Memperbarui `UserId` menggunakan `playerId` (nilai pemain identitas) dalam CreateEvent.
            UserId = playerId,
            // Memperbarui `ActorType` menggunakan nilai literal `”PLAYER”` dalam CreateEvent.
            ActorType = "PLAYER",
            // Memperbarui `Timestamp` menggunakan objek baru bertipe `DateTimeOffset` dengan argumen (2026, 1, 2, 3, 4, 5, TimeSpan.Zero) dalam CreateEvent.
            Timestamp = new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero),
            // Memperbarui `DayIndex` menggunakan nilai literal `0` dalam CreateEvent.
            DayIndex = 0,
            // Memperbarui `Weekday` menggunakan nilai literal `”MON”` dalam CreateEvent.
            Weekday = "MON",
            // Memperbarui `ActionSlot` menggunakan nilai literal `1` dalam CreateEvent.
            ActionSlot = 1,
            // Memperbarui `SequenceNumber` menggunakan nilai literal `1` dalam CreateEvent.
            SequenceNumber = 1,
            // Memperbarui `ActionType` menggunakan `actionType` (nilai aksi jenis) dalam CreateEvent.
            ActionType = actionType,
            // Memperbarui `RulesetVersionId` menggunakan memanggil `Guid.NewGuid` dengan tanpa argumen dalam CreateEvent.
            RulesetVersionId = Guid.NewGuid(),
            // Memperbarui `Payload` menggunakan `payload` (muatan detail event dalam format JSON) dalam CreateEvent.
            Payload = payload
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam CreateEvent.
        };
    // Menutup scope metode CreateEvent; bagian berikut berada di luar batas blok tersebut dalam CreateEvent.
    }

    // Mendefinisikan metode `CreateProjection` dengan hasil bertipe `CashflowProjectionDb`; operasi ini menangani create projection. Masukan: Parameter
    // `sessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini; Parameter `playerId` bertipe `Guid` membawa
    // nilai pemain identitas; Parameter `direction` bertipe `string` membawa nilai direction; Parameter `amount` bertipe `int` membawa nominal uang
    // atau nilai transaksi yang dipakai dalam operasi; Parameter `category` bertipe `string` membawa nilai category.
    private static CashflowProjectionDb CreateProjection(
        // Parameter `sessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini.
        Guid sessionId,
        // Parameter `playerId` bertipe `Guid` membawa nilai pemain identitas.
        Guid playerId,
        // Parameter `direction` bertipe `string` membawa nilai direction.
        string direction,
        // Parameter `amount` bertipe `int` membawa nominal uang atau nilai transaksi yang dipakai dalam operasi.
        int amount,
        // Parameter `category` bertipe `string` membawa nilai category.
        string category)
    // Membuka scope metode CreateProjection; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CreateProjection.
    {
        // Mengembalikan objek baru bertipe `CashflowProjectionDb` dengan nilai awal sesuai konstruktornya kepada pemanggil dalam CreateProjection; eksekusi
        // jalur ini selesai setelah nilai hasil ditentukan.
        return new CashflowProjectionDb
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CreateProjection.
        {
            // Memperbarui `ProjectionId` menggunakan memanggil `Guid.NewGuid` dengan tanpa argumen dalam CreateProjection.
            ProjectionId = Guid.NewGuid(),
            // Memperbarui `SessionId` menggunakan `sessionId` (identitas unik sesi permainan yang menjadi batas data operasi ini) dalam CreateProjection.
            SessionId = sessionId,
            // Memperbarui `UserId` menggunakan `playerId` (nilai pemain identitas) dalam CreateProjection.
            UserId = playerId,
            // Memperbarui `EventPk` menggunakan memanggil `Guid.NewGuid` dengan tanpa argumen dalam CreateProjection.
            EventPk = Guid.NewGuid(),
            // Memperbarui `EventId` menggunakan memanggil `Guid.NewGuid` dengan tanpa argumen dalam CreateProjection.
            EventId = Guid.NewGuid(),
            // Memperbarui `Timestamp` menggunakan objek baru bertipe `DateTimeOffset` dengan argumen (2026, 1, 2, 3, 4, 5, TimeSpan.Zero) dalam
            // CreateProjection.
            Timestamp = new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero),
            // Memperbarui `Direction` menggunakan `direction` (nilai direction) dalam CreateProjection.
            Direction = direction,
            // Memperbarui `Amount` menggunakan `amount` (nominal uang atau nilai transaksi yang dipakai dalam operasi) dalam CreateProjection.
            Amount = amount,
            // Memperbarui `Category` menggunakan `category` (nilai category) dalam CreateProjection.
            Category = category
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam CreateProjection.
        };
    // Menutup scope metode CreateProjection; bagian berikut berada di luar batas blok tersebut dalam CreateProjection.
    }
// Menutup scope tipe AnalyticsNeedMissionCalculatorTests; bagian berikut berada di luar batas blok tersebut.
}
