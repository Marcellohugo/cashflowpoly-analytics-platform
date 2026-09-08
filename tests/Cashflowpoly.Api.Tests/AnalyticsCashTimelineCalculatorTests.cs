// Fungsi file: Memverifikasi perilaku API, database, atau domain melalui AnalyticsCashTimelineCalculatorTests.
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;
// Mengimpor namespace `Cashflowpoly.Api.Domain` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Domain;
// Mengimpor namespace `Xunit` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Xunit;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Tests` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Tests;

// Mendefinisikan tipe class `AnalyticsCashTimelineCalculatorTests`; sealed mencegah tipe ini diturunkan lagi.
public sealed class AnalyticsCashTimelineCalculatorTests
// Membuka scope tipe AnalyticsCashTimelineCalculatorTests; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    [Theory]
    [InlineData("SetupBahanAwal", "INITIAL_INGREDIENT")]
    [InlineData("BahanMasakan", "INGREDIENT")]
    public void Compute_DistinguishesInitialIngredientPaymentFromPlayerPurchases(string action, string category)
    {
        var sessionId = Guid.NewGuid();
        var playerId = Guid.NewGuid();
        var evt = CreateEvent(Guid.NewGuid(), sessionId, playerId, actionSlot: 0);
        evt.ActionType = action;
        var projection = CreateProjection(evt.EventId, sessionId, playerId, "OUT", 1);
        projection.Category = "INGREDIENT";
        var timeline = new CashTimelineCalculator().Compute([evt], [projection], startingCoins: 10);

        Assert.Equal(9, timeline.CoinsNetEndGame);
        Assert.Equal(0, timeline.CoinsSpentPerTurn.Single().DayIndex);
        Assert.Equal(category, timeline.CoinsSpentPerTurn.Single().CashflowCategory);
        Assert.Equal(category, timeline.CoinsProgression.Single().CashflowCategory);
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `Compute_GroupsCashflowByEventTurnAndBuildsCoinProgression` dengan hasil bertipe `void`; operasi ini menangani compute
    // groups arus kas berdasarkan event giliran dan builds coin progression.
    public void Compute_GroupsCashflowByEventTurnAndBuildsCoinProgression()
    // Membuka scope metode Compute_GroupsCashflowByEventTurnAndBuildsCoinProgression; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // Compute_GroupsCashflowByEventTurnAndBuildsCoinProgression.
    {
        // Menyiapkan variabel lokal `sessionId` untuk identitas unik sesi permainan yang menjadi batas data operasi ini dengan memanggil `Guid.NewGuid`
        // dengan tanpa argumen. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var sessionId = Guid.NewGuid();
        // Menyiapkan variabel lokal `playerId` untuk nilai pemain identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var playerId = Guid.NewGuid();
        // Menyiapkan variabel lokal `firstEventId` untuk nilai first event identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var firstEventId = Guid.NewGuid();
        // Menyiapkan variabel lokal `secondEventId` untuk nilai second event identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var secondEventId = Guid.NewGuid();
        // Menyiapkan variabel lokal `events` untuk kumpulan event permainan sebagai sumber riwayat untuk validasi atau perhitungan dengan objek baru
        // bertipe `List<EventDb>` dengan nilai awal sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var events = new List<EventDb>
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // Compute_GroupsCashflowByEventTurnAndBuildsCoinProgression.
        {
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `firstEventId`, `sessionId`, `playerId`, `1` dalam
            // Compute_GroupsCashflowByEventTurnAndBuildsCoinProgression.
            CreateEvent(firstEventId, sessionId, playerId, actionSlot: 1),
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `secondEventId`, `sessionId`, `playerId`, `2` dalam
            // Compute_GroupsCashflowByEventTurnAndBuildsCoinProgression.
            CreateEvent(secondEventId, sessionId, playerId, actionSlot: 2)
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // Compute_GroupsCashflowByEventTurnAndBuildsCoinProgression.
        };
        // Menyiapkan variabel lokal `projections` untuk proyeksi transaksi arus kas yang diturunkan dari event permainan dengan objek baru bertipe
        // `List<CashflowProjectionDb>` dengan nilai awal sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var projections = new List<CashflowProjectionDb>
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // Compute_GroupsCashflowByEventTurnAndBuildsCoinProgression.
        {
            // Melanjutkan pengolahan dengan memanggil `CreateProjection` dengan `firstEventId`, `sessionId`, `playerId`, `”IN”`, `10` dalam
            // Compute_GroupsCashflowByEventTurnAndBuildsCoinProgression.
            CreateProjection(firstEventId, sessionId, playerId, "IN", 10),
            // Melanjutkan pengolahan dengan memanggil `CreateProjection` dengan `firstEventId`, `sessionId`, `playerId`, `”OUT”`, `4` dalam
            // Compute_GroupsCashflowByEventTurnAndBuildsCoinProgression.
            CreateProjection(firstEventId, sessionId, playerId, "OUT", 4),
            // Melanjutkan pengolahan dengan memanggil `CreateProjection` dengan `secondEventId`, `sessionId`, `playerId`, `”OUT”`, `3` dalam
            // Compute_GroupsCashflowByEventTurnAndBuildsCoinProgression.
            CreateProjection(secondEventId, sessionId, playerId, "OUT", 3),
            // Melanjutkan pengolahan dengan memanggil `CreateProjection` dengan `Guid.NewGuid()`, `sessionId`, `playerId`, `”OUT”`, `99` dalam
            // Compute_GroupsCashflowByEventTurnAndBuildsCoinProgression.
            CreateProjection(Guid.NewGuid(), sessionId, playerId, "OUT", 99)
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // Compute_GroupsCashflowByEventTurnAndBuildsCoinProgression.
        };

        // Menyiapkan variabel lokal `timeline` untuk nilai timeline dengan memanggil `new CashTimelineCalculator().Compute` dengan `events`, `projections`,
        // `20`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var timeline = new CashTimelineCalculator().Compute(events, projections, startingCoins: 20);

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`20`, `timeline.StartingCoins`); pengujian
        // gagal jika keduanya berbeda dalam Compute_GroupsCashflowByEventTurnAndBuildsCoinProgression.
        Assert.Equal(20, timeline.StartingCoins);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`10`, `timeline.CashInTotal`); pengujian gagal
        // jika keduanya berbeda dalam Compute_GroupsCashflowByEventTurnAndBuildsCoinProgression.
        Assert.Equal(10, timeline.CashInTotal);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`7`, `timeline.CashOutTotal`); pengujian gagal
        // jika keduanya berbeda dalam Compute_GroupsCashflowByEventTurnAndBuildsCoinProgression.
        Assert.Equal(7, timeline.CashOutTotal);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`23`, `timeline.CoinsNetEndGame`); pengujian
        // gagal jika keduanya berbeda dalam Compute_GroupsCashflowByEventTurnAndBuildsCoinProgression.
        Assert.Equal(23, timeline.CoinsNetEndGame);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`23`, `timeline.CoinsHeldCurrent`); pengujian
        // gagal jika keduanya berbeda dalam Compute_GroupsCashflowByEventTurnAndBuildsCoinProgression.
        Assert.Equal(23, timeline.CoinsHeldCurrent);

        // Menjalankan pemeriksaan hasil dengan `Assert.Collection` menggunakan `timeline.CoinsSpentPerTurn`, `item => { Assert.Equal(1, item.ActionSlot);
        // Assert.Equal(”TEST”, item.CashflowCategory); Assert.Equal(4, item.Amount); }`, `item => { Assert.Equal(2, item.ActionSlot); Assert.Equal(3,
        // item.Amount); }`; ketidaksesuaian dengan ekspektasi membuat pengujian gagal dalam Compute_GroupsCashflowByEventTurnAndBuildsCoinProgression.
        Assert.Collection(timeline.CoinsSpentPerTurn,
            // Parameter `item` bertipe `` membawa nilai elemen.
            item =>
            // Membuka scope fungsi lambda yang dipasok ke `Assert.Collection`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // Compute_GroupsCashflowByEventTurnAndBuildsCoinProgression.
            {
                // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`1`, `item.ActionSlot`); pengujian gagal jika
                // keduanya berbeda dalam Compute_GroupsCashflowByEventTurnAndBuildsCoinProgression.
                Assert.Equal(1, item.ActionSlot);
                // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”TEST”`, `item.CashflowCategory`); pengujian
                // gagal jika keduanya berbeda dalam Compute_GroupsCashflowByEventTurnAndBuildsCoinProgression.
                Assert.Equal("TEST", item.CashflowCategory);
                // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`4`, `item.Amount`); pengujian gagal jika
                // keduanya berbeda dalam Compute_GroupsCashflowByEventTurnAndBuildsCoinProgression.
                Assert.Equal(4, item.Amount);
            // Menutup scope fungsi lambda yang dipasok ke `Assert.Collection`; bagian berikut berada di luar batas blok tersebut dalam
            // Compute_GroupsCashflowByEventTurnAndBuildsCoinProgression.
            },
            // Parameter `item` bertipe `` membawa nilai elemen.
            item =>
            // Membuka scope fungsi lambda yang dipasok ke `Assert.Collection`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // Compute_GroupsCashflowByEventTurnAndBuildsCoinProgression.
            {
                // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`2`, `item.ActionSlot`); pengujian gagal jika
                // keduanya berbeda dalam Compute_GroupsCashflowByEventTurnAndBuildsCoinProgression.
                Assert.Equal(2, item.ActionSlot);
                // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`3`, `item.Amount`); pengujian gagal jika
                // keduanya berbeda dalam Compute_GroupsCashflowByEventTurnAndBuildsCoinProgression.
                Assert.Equal(3, item.Amount);
            // Menutup scope fungsi lambda yang dipasok ke `Assert.Collection`; bagian berikut berada di luar batas blok tersebut dalam
            // Compute_GroupsCashflowByEventTurnAndBuildsCoinProgression.
            });

        // Menjalankan pemeriksaan hasil dengan `Assert.Collection` menggunakan `timeline.CoinsEarnedPerTurn`, `item => { Assert.Equal(1, item.ActionSlot);
        // Assert.Equal(10, item.Amount); }`; ketidaksesuaian dengan ekspektasi membuat pengujian gagal dalam
        // Compute_GroupsCashflowByEventTurnAndBuildsCoinProgression.
        Assert.Collection(timeline.CoinsEarnedPerTurn,
            // Parameter `item` bertipe `` membawa nilai elemen.
            item =>
            // Membuka scope fungsi lambda yang dipasok ke `Assert.Collection`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // Compute_GroupsCashflowByEventTurnAndBuildsCoinProgression.
            {
                // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`1`, `item.ActionSlot`); pengujian gagal jika
                // keduanya berbeda dalam Compute_GroupsCashflowByEventTurnAndBuildsCoinProgression.
                Assert.Equal(1, item.ActionSlot);
                // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`10`, `item.Amount`); pengujian gagal jika
                // keduanya berbeda dalam Compute_GroupsCashflowByEventTurnAndBuildsCoinProgression.
                Assert.Equal(10, item.Amount);
            // Menutup scope fungsi lambda yang dipasok ke `Assert.Collection`; bagian berikut berada di luar batas blok tersebut dalam
            // Compute_GroupsCashflowByEventTurnAndBuildsCoinProgression.
            });

        // Menjalankan pemeriksaan hasil dengan `Assert.Collection` menggunakan `timeline.NetIncomePerTurn`, `item => { Assert.Equal(1, item.ActionSlot);
        // Assert.Equal(”TEST”, item.CashflowCategory); Assert.Equal(6, item.Net); }`, `item => { Assert.Equal(2, item.ActionSlot); Assert.Equal(-3,
        // item.Net); }`; ketidaksesuaian dengan ekspektasi membuat pengujian gagal dalam Compute_GroupsCashflowByEventTurnAndBuildsCoinProgression.
        Assert.Collection(timeline.NetIncomePerTurn,
            // Parameter `item` bertipe `` membawa nilai elemen.
            item =>
            // Membuka scope fungsi lambda yang dipasok ke `Assert.Collection`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // Compute_GroupsCashflowByEventTurnAndBuildsCoinProgression.
            {
                // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`1`, `item.ActionSlot`); pengujian gagal jika
                // keduanya berbeda dalam Compute_GroupsCashflowByEventTurnAndBuildsCoinProgression.
                Assert.Equal(1, item.ActionSlot);
                // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”TEST”`, `item.CashflowCategory`); pengujian
                // gagal jika keduanya berbeda dalam Compute_GroupsCashflowByEventTurnAndBuildsCoinProgression.
                Assert.Equal("TEST", item.CashflowCategory);
                // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`6`, `item.Net`); pengujian gagal jika keduanya
                // berbeda dalam Compute_GroupsCashflowByEventTurnAndBuildsCoinProgression.
                Assert.Equal(6, item.Net);
            // Menutup scope fungsi lambda yang dipasok ke `Assert.Collection`; bagian berikut berada di luar batas blok tersebut dalam
            // Compute_GroupsCashflowByEventTurnAndBuildsCoinProgression.
            },
            // Parameter `item` bertipe `` membawa nilai elemen.
            item =>
            // Membuka scope fungsi lambda yang dipasok ke `Assert.Collection`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // Compute_GroupsCashflowByEventTurnAndBuildsCoinProgression.
            {
                // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`2`, `item.ActionSlot`); pengujian gagal jika
                // keduanya berbeda dalam Compute_GroupsCashflowByEventTurnAndBuildsCoinProgression.
                Assert.Equal(2, item.ActionSlot);
                // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`-3`, `item.Net`); pengujian gagal jika
                // keduanya berbeda dalam Compute_GroupsCashflowByEventTurnAndBuildsCoinProgression.
                Assert.Equal(-3, item.Net);
            // Menutup scope fungsi lambda yang dipasok ke `Assert.Collection`; bagian berikut berada di luar batas blok tersebut dalam
            // Compute_GroupsCashflowByEventTurnAndBuildsCoinProgression.
            });

        // Menjalankan pemeriksaan hasil dengan `Assert.Collection` menggunakan `timeline.CoinsProgression`, `item => { Assert.Equal(1, item.ActionSlot);
        // Assert.Equal(26, item.Coins); }`, `item => { Assert.Equal(2, item.ActionSlot); Assert.Equal(23, item.Coins); }`; ketidaksesuaian dengan
        // ekspektasi membuat pengujian gagal dalam Compute_GroupsCashflowByEventTurnAndBuildsCoinProgression.
        Assert.Collection(timeline.CoinsProgression,
            // Parameter `item` bertipe `` membawa nilai elemen.
            item =>
            // Membuka scope fungsi lambda yang dipasok ke `Assert.Collection`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // Compute_GroupsCashflowByEventTurnAndBuildsCoinProgression.
            {
                // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`1`, `item.ActionSlot`); pengujian gagal jika
                // keduanya berbeda dalam Compute_GroupsCashflowByEventTurnAndBuildsCoinProgression.
                Assert.Equal(1, item.ActionSlot);
                // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`26`, `item.Coins`); pengujian gagal jika
                // keduanya berbeda dalam Compute_GroupsCashflowByEventTurnAndBuildsCoinProgression.
                Assert.Equal(26, item.Coins);
            // Menutup scope fungsi lambda yang dipasok ke `Assert.Collection`; bagian berikut berada di luar batas blok tersebut dalam
            // Compute_GroupsCashflowByEventTurnAndBuildsCoinProgression.
            },
            // Parameter `item` bertipe `` membawa nilai elemen.
            item =>
            // Membuka scope fungsi lambda yang dipasok ke `Assert.Collection`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // Compute_GroupsCashflowByEventTurnAndBuildsCoinProgression.
            {
                // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`2`, `item.ActionSlot`); pengujian gagal jika
                // keduanya berbeda dalam Compute_GroupsCashflowByEventTurnAndBuildsCoinProgression.
                Assert.Equal(2, item.ActionSlot);
                // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`23`, `item.Coins`); pengujian gagal jika
                // keduanya berbeda dalam Compute_GroupsCashflowByEventTurnAndBuildsCoinProgression.
                Assert.Equal(23, item.Coins);
            // Menutup scope fungsi lambda yang dipasok ke `Assert.Collection`; bagian berikut berada di luar batas blok tersebut dalam
            // Compute_GroupsCashflowByEventTurnAndBuildsCoinProgression.
            });
    // Menutup scope metode Compute_GroupsCashflowByEventTurnAndBuildsCoinProgression; bagian berikut berada di luar batas blok tersebut dalam
    // Compute_GroupsCashflowByEventTurnAndBuildsCoinProgression.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `Compute_IncludesCashflowCausedByAnotherPlayersEvent` dengan hasil bertipe `void`; operasi ini menangani compute includes
    // arus kas caused berdasarkan another pemain event.
    public void Compute_IncludesCashflowCausedByAnotherPlayersEvent()
    // Membuka scope metode Compute_IncludesCashflowCausedByAnotherPlayersEvent; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // Compute_IncludesCashflowCausedByAnotherPlayersEvent.
    {
        // Menyiapkan variabel lokal `sessionId` untuk identitas unik sesi permainan yang menjadi batas data operasi ini dengan memanggil `Guid.NewGuid`
        // dengan tanpa argumen. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var sessionId = Guid.NewGuid();
        // Menyiapkan variabel lokal `playerId` untuk nilai pemain identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var playerId = Guid.NewGuid();
        // Menyiapkan variabel lokal `otherPlayerId` untuk nilai other pemain identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var otherPlayerId = Guid.NewGuid();
        // Menyiapkan variabel lokal `ownEventId` untuk nilai own event identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var ownEventId = Guid.NewGuid();
        // Menyiapkan variabel lokal `sharedEventId` untuk nilai shared event identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var sharedEventId = Guid.NewGuid();
        // Menyiapkan variabel lokal `events` untuk kumpulan event permainan sebagai sumber riwayat untuk validasi atau perhitungan dengan objek baru
        // bertipe `List<EventDb>` dengan nilai awal sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var events = new List<EventDb>
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // Compute_IncludesCashflowCausedByAnotherPlayersEvent.
        {
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `ownEventId`, `sessionId`, `playerId`, `1` dalam
            // Compute_IncludesCashflowCausedByAnotherPlayersEvent.
            CreateEvent(ownEventId, sessionId, playerId, actionSlot: 1),
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `sharedEventId`, `sessionId`, `otherPlayerId`, `2` dalam
            // Compute_IncludesCashflowCausedByAnotherPlayersEvent.
            CreateEvent(sharedEventId, sessionId, otherPlayerId, actionSlot: 2)
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // Compute_IncludesCashflowCausedByAnotherPlayersEvent.
        };
        // Memperbarui `events[1].SequenceNumber` menggunakan nilai literal `2` dalam Compute_IncludesCashflowCausedByAnotherPlayersEvent.
        events[1].SequenceNumber = 2;

        // Menyiapkan variabel lokal `timeline` untuk nilai timeline dengan memanggil `new CashTimelineCalculator().Compute` dengan `events`, `[
        // CreateProjection(ownEventId, sessionId, playerId, ”IN”, 5), CreateProjection(sharedEventId, sessionId, playerId, ”OUT”, 3) ]`, `10`. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var timeline = new CashTimelineCalculator().Compute(
            // Meneruskan `events` (kumpulan event permainan sebagai sumber riwayat untuk validasi atau perhitungan) sebagai argumen ke `new
            // CashTimelineCalculator().Compute`.
            events,
            // Meneruskan koleksi berisi CreateProjection(ownEventId, sessionId, playerId, ..., CreateProjection(sharedEventId, sessionId, playerI... sebagai
            // argumen ke `new CashTimelineCalculator().Compute`.
            [
                // Meneruskan `ownEventId` (nilai own event identitas) sebagai argumen ke `CreateProjection`; Meneruskan `sessionId` (identitas unik sesi permainan
                // yang menjadi batas data operasi ini) sebagai argumen ke `CreateProjection`; Meneruskan `playerId` (nilai pemain identitas) sebagai argumen ke
                // `CreateProjection`; Meneruskan nilai literal `”IN”` sebagai argumen ke `CreateProjection`; Meneruskan nilai literal `5` sebagai argumen ke
                // `CreateProjection`.
                CreateProjection(ownEventId, sessionId, playerId, "IN", 5),
                // Meneruskan `sharedEventId` (nilai shared event identitas) sebagai argumen ke `CreateProjection`; Meneruskan `sessionId` (identitas unik sesi
                // permainan yang menjadi batas data operasi ini) sebagai argumen ke `CreateProjection`; Meneruskan `playerId` (nilai pemain identitas) sebagai
                // argumen ke `CreateProjection`; Meneruskan nilai literal `”OUT”` sebagai argumen ke `CreateProjection`; Meneruskan nilai literal `3` sebagai
                // argumen ke `CreateProjection`.
                CreateProjection(sharedEventId, sessionId, playerId, "OUT", 3)
            // Meneruskan koleksi berisi CreateProjection(ownEventId, sessionId, playerId, ..., CreateProjection(sharedEventId, sessionId, playerI... sebagai
            // argumen ke `new CashTimelineCalculator().Compute`.
            ],
            // Meneruskan nilai literal `10` sebagai argumen bernama `startingCoins`.
            startingCoins: 10);

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`12`, `timeline.CoinsNetEndGame`); pengujian
        // gagal jika keduanya berbeda dalam Compute_IncludesCashflowCausedByAnotherPlayersEvent.
        Assert.Equal(12, timeline.CoinsNetEndGame);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`12`, `timeline.CoinsProgression[^1].Coins`);
        // pengujian gagal jika keduanya berbeda dalam Compute_IncludesCashflowCausedByAnotherPlayersEvent.
        Assert.Equal(12, timeline.CoinsProgression[^1].Coins);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`2`, `timeline.CoinsProgression.Count`);
        // pengujian gagal jika keduanya berbeda dalam Compute_IncludesCashflowCausedByAnotherPlayersEvent.
        Assert.Equal(2, timeline.CoinsProgression.Count);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`3`,
        // `timeline.CoinsSpentPerTurn.Single().Amount`); pengujian gagal jika keduanya berbeda dalam Compute_IncludesCashflowCausedByAnotherPlayersEvent.
        Assert.Equal(3, timeline.CoinsSpentPerTurn.Single().Amount);
    // Menutup scope metode Compute_IncludesCashflowCausedByAnotherPlayersEvent; bagian berikut berada di luar batas blok tersebut dalam
    // Compute_IncludesCashflowCausedByAnotherPlayersEvent.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `Compute_KeepsRepeatedActionSlotsOnDifferentDaysSeparate` dengan hasil bertipe `void`; operasi ini menangani compute keeps
    // repeated aksi slots on different hari separate.
    public void Compute_KeepsRepeatedActionSlotsOnDifferentDaysSeparate()
    // Membuka scope metode Compute_KeepsRepeatedActionSlotsOnDifferentDaysSeparate; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // Compute_KeepsRepeatedActionSlotsOnDifferentDaysSeparate.
    {
        // Menyiapkan variabel lokal `sessionId` untuk identitas unik sesi permainan yang menjadi batas data operasi ini dengan memanggil `Guid.NewGuid`
        // dengan tanpa argumen. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var sessionId = Guid.NewGuid();
        // Menyiapkan variabel lokal `playerId` untuk nilai pemain identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var playerId = Guid.NewGuid();
        // Menyiapkan variabel lokal `firstEventId` untuk nilai first event identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var firstEventId = Guid.NewGuid();
        // Menyiapkan variabel lokal `secondEventId` untuk nilai second event identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var secondEventId = Guid.NewGuid();
        // Menyiapkan variabel lokal `first` untuk nilai first dengan memanggil `CreateEvent` dengan `firstEventId`, `sessionId`, `playerId`, `1`. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var first = CreateEvent(firstEventId, sessionId, playerId, actionSlot: 1);
        // Menyiapkan variabel lokal `second` untuk nilai second dengan memanggil `CreateEvent` dengan `secondEventId`, `sessionId`, `playerId`, `1`. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var second = CreateEvent(secondEventId, sessionId, playerId, actionSlot: 1);
        // Memperbarui `second.DayIndex` menggunakan nilai literal `1` dalam Compute_KeepsRepeatedActionSlotsOnDifferentDaysSeparate.
        second.DayIndex = 1;
        // Memperbarui `second.SequenceNumber` menggunakan nilai literal `2` dalam Compute_KeepsRepeatedActionSlotsOnDifferentDaysSeparate.
        second.SequenceNumber = 2;

        // Menyiapkan variabel lokal `timeline` untuk nilai timeline dengan memanggil `new CashTimelineCalculator().Compute` dengan `[first, second]`, `[
        // CreateProjection(firstEventId, sessionId, playerId, ”IN”, 3), CreateProjection(secondEventId, sessionId, playerId, ”IN”, 4) ]`, `10`. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var timeline = new CashTimelineCalculator().Compute(
            // Meneruskan koleksi berisi first, second sebagai argumen ke `new CashTimelineCalculator().Compute`.
            [first, second],
            // Meneruskan koleksi berisi CreateProjection(firstEventId, sessionId, playerId..., CreateProjection(secondEventId, sessionId, playerI... sebagai
            // argumen ke `new CashTimelineCalculator().Compute`.
            [
                // Meneruskan `firstEventId` (nilai first event identitas) sebagai argumen ke `CreateProjection`; Meneruskan `sessionId` (identitas unik sesi
                // permainan yang menjadi batas data operasi ini) sebagai argumen ke `CreateProjection`; Meneruskan `playerId` (nilai pemain identitas) sebagai
                // argumen ke `CreateProjection`; Meneruskan nilai literal `”IN”` sebagai argumen ke `CreateProjection`; Meneruskan nilai literal `3` sebagai
                // argumen ke `CreateProjection`.
                CreateProjection(firstEventId, sessionId, playerId, "IN", 3),
                // Meneruskan `secondEventId` (nilai second event identitas) sebagai argumen ke `CreateProjection`; Meneruskan `sessionId` (identitas unik sesi
                // permainan yang menjadi batas data operasi ini) sebagai argumen ke `CreateProjection`; Meneruskan `playerId` (nilai pemain identitas) sebagai
                // argumen ke `CreateProjection`; Meneruskan nilai literal `”IN”` sebagai argumen ke `CreateProjection`; Meneruskan nilai literal `4` sebagai
                // argumen ke `CreateProjection`.
                CreateProjection(secondEventId, sessionId, playerId, "IN", 4)
            // Meneruskan koleksi berisi CreateProjection(firstEventId, sessionId, playerId..., CreateProjection(secondEventId, sessionId, playerI... sebagai
            // argumen ke `new CashTimelineCalculator().Compute`.
            ],
            // Meneruskan nilai literal `10` sebagai argumen bernama `startingCoins`.
            startingCoins: 10);

        // Menjalankan pemeriksaan hasil dengan `Assert.Collection` menggunakan `timeline.CoinsEarnedPerTurn`, `item => { Assert.Equal(0, item.DayIndex);
        // Assert.Equal(3, item.Amount); }`, `item => { Assert.Equal(1, item.DayIndex); Assert.Equal(4, item.Amount); }`; ketidaksesuaian dengan ekspektasi
        // membuat pengujian gagal dalam Compute_KeepsRepeatedActionSlotsOnDifferentDaysSeparate.
        Assert.Collection(
            // Meneruskan `timeline.CoinsEarnedPerTurn` (nilai coins earned per giliran) sebagai argumen ke `Assert.Collection`.
            timeline.CoinsEarnedPerTurn,
            // Parameter `item` bertipe `` membawa nilai elemen.
            item =>
            // Membuka scope fungsi lambda yang dipasok ke `Assert.Collection`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // Compute_KeepsRepeatedActionSlotsOnDifferentDaysSeparate.
            {
                // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`0`, `item.DayIndex`); pengujian gagal jika
                // keduanya berbeda dalam Compute_KeepsRepeatedActionSlotsOnDifferentDaysSeparate.
                Assert.Equal(0, item.DayIndex);
                // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`3`, `item.Amount`); pengujian gagal jika
                // keduanya berbeda dalam Compute_KeepsRepeatedActionSlotsOnDifferentDaysSeparate.
                Assert.Equal(3, item.Amount);
            // Menutup scope fungsi lambda yang dipasok ke `Assert.Collection`; bagian berikut berada di luar batas blok tersebut dalam
            // Compute_KeepsRepeatedActionSlotsOnDifferentDaysSeparate.
            },
            // Parameter `item` bertipe `` membawa nilai elemen.
            item =>
            // Membuka scope fungsi lambda yang dipasok ke `Assert.Collection`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // Compute_KeepsRepeatedActionSlotsOnDifferentDaysSeparate.
            {
                // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`1`, `item.DayIndex`); pengujian gagal jika
                // keduanya berbeda dalam Compute_KeepsRepeatedActionSlotsOnDifferentDaysSeparate.
                Assert.Equal(1, item.DayIndex);
                // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`4`, `item.Amount`); pengujian gagal jika
                // keduanya berbeda dalam Compute_KeepsRepeatedActionSlotsOnDifferentDaysSeparate.
                Assert.Equal(4, item.Amount);
            // Menutup scope fungsi lambda yang dipasok ke `Assert.Collection`; bagian berikut berada di luar batas blok tersebut dalam
            // Compute_KeepsRepeatedActionSlotsOnDifferentDaysSeparate.
            });
    // Menutup scope metode Compute_KeepsRepeatedActionSlotsOnDifferentDaysSeparate; bagian berikut berada di luar batas blok tersebut dalam
    // Compute_KeepsRepeatedActionSlotsOnDifferentDaysSeparate.
    }

    // Mendefinisikan metode `CreateEvent` dengan hasil bertipe `EventDb`; operasi ini menangani create event. Masukan: Parameter `eventId` bertipe
    // `Guid` membawa identitas unik event untuk pencatatan dan pemeriksaan duplikasi; Parameter `sessionId` bertipe `Guid` membawa identitas unik sesi
    // permainan yang menjadi batas data operasi ini; Parameter `playerId` bertipe `Guid` membawa nilai pemain identitas; Parameter `actionSlot` bertipe
    // `int` membawa nilai aksi slot.
    private static EventDb CreateEvent(Guid eventId, Guid sessionId, Guid playerId, int actionSlot)
    // Membuka scope metode CreateEvent; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CreateEvent.
    {
        // Mengembalikan objek baru bertipe `EventDb` dengan nilai awal sesuai konstruktornya kepada pemanggil dalam CreateEvent; eksekusi jalur ini selesai
        // setelah nilai hasil ditentukan.
        return new EventDb
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CreateEvent.
        {
            // Memperbarui `EventId` menggunakan `eventId` (identitas unik event untuk pencatatan dan pemeriksaan duplikasi) dalam CreateEvent.
            EventId = eventId,
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
            // Memperbarui `ActionSlot` menggunakan `actionSlot` (nilai aksi slot) dalam CreateEvent.
            ActionSlot = actionSlot,
            // Memperbarui `SequenceNumber` menggunakan `actionSlot` (nilai aksi slot) dalam CreateEvent.
            SequenceNumber = actionSlot,
            // Memperbarui `ActionType` menggunakan nilai literal `”CatatTransaksi”` dalam CreateEvent.
            ActionType = "CatatTransaksi",
            // Memperbarui `RulesetVersionId` menggunakan memanggil `Guid.NewGuid` dengan tanpa argumen dalam CreateEvent.
            RulesetVersionId = Guid.NewGuid(),
            // Memperbarui `Payload` menggunakan nilai literal `”{}”` dalam CreateEvent.
            Payload = "{}"
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam CreateEvent.
        };
    // Menutup scope metode CreateEvent; bagian berikut berada di luar batas blok tersebut dalam CreateEvent.
    }

    // Mendefinisikan metode `CreateProjection` dengan hasil bertipe `CashflowProjectionDb`; operasi ini menangani create projection. Masukan: Parameter
    // `eventId` bertipe `Guid` membawa identitas unik event untuk pencatatan dan pemeriksaan duplikasi; Parameter `sessionId` bertipe `Guid` membawa
    // identitas unik sesi permainan yang menjadi batas data operasi ini; Parameter `playerId` bertipe `Guid` membawa nilai pemain identitas; Parameter
    // `direction` bertipe `string` membawa nilai direction; Parameter `amount` bertipe `int` membawa nominal uang atau nilai transaksi yang dipakai
    // dalam operasi.
    private static CashflowProjectionDb CreateProjection(
        // Parameter `eventId` bertipe `Guid` membawa identitas unik event untuk pencatatan dan pemeriksaan duplikasi.
        Guid eventId,
        // Parameter `sessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini.
        Guid sessionId,
        // Parameter `playerId` bertipe `Guid` membawa nilai pemain identitas.
        Guid playerId,
        // Parameter `direction` bertipe `string` membawa nilai direction.
        string direction,
        // Parameter `amount` bertipe `int` membawa nominal uang atau nilai transaksi yang dipakai dalam operasi.
        int amount)
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
            // Memperbarui `EventId` menggunakan `eventId` (identitas unik event untuk pencatatan dan pemeriksaan duplikasi) dalam CreateProjection.
            EventId = eventId,
            // Memperbarui `Timestamp` menggunakan objek baru bertipe `DateTimeOffset` dengan argumen (2026, 1, 2, 3, 4, 5, TimeSpan.Zero) dalam
            // CreateProjection.
            Timestamp = new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero),
            // Memperbarui `Direction` menggunakan `direction` (nilai direction) dalam CreateProjection.
            Direction = direction,
            // Memperbarui `Amount` menggunakan `amount` (nominal uang atau nilai transaksi yang dipakai dalam operasi) dalam CreateProjection.
            Amount = amount,
            // Memperbarui `Category` menggunakan nilai literal `”TEST”` dalam CreateProjection.
            Category = "TEST"
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam CreateProjection.
        };
    // Menutup scope metode CreateProjection; bagian berikut berada di luar batas blok tersebut dalam CreateProjection.
    }
// Menutup scope tipe AnalyticsCashTimelineCalculatorTests; bagian berikut berada di luar batas blok tersebut.
}
