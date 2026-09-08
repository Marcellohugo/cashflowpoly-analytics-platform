// Fungsi file: Memverifikasi perilaku API, database, atau domain melalui AnalyticsActionUsageCalculatorTests.
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;
// Mengimpor namespace `Cashflowpoly.Api.Domain` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Domain;
// Mengimpor namespace `Xunit` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Xunit;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Tests` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Tests;

// Mendefinisikan tipe class `AnalyticsActionUsageCalculatorTests`; sealed mencegah tipe ini diturunkan lagi.
public sealed class AnalyticsActionUsageCalculatorTests
// Membuka scope tipe AnalyticsActionUsageCalculatorTests; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    [Fact]
    public void Compute_DistinguishesEarnedIncomeFromBorrowingWithdrawalsAndFreeGoldSales()
    {
        var playerId = Guid.NewGuid();
        var sessionId = Guid.NewGuid();
        var actions = new[] { "KerjaLepas", "JualMasakan", "PinjamanSyariah", "TarikTabungan", "JualEmas" };
        var events = actions.Select((action, index) => CreateEvent(
            Guid.NewGuid(), sessionId, playerId, action, 1, action == "JualEmas" ? 0 : 1, index)).ToList();
        var projections = events.Select(evt => CreateProjection(
            evt.EventId, sessionId, playerId, "IN", 10, evt.ActionType)).ToList();

        var metrics = new ActionUsageCalculator().Compute(events, projections, 1, 2);

        Assert.Equal(4, metrics.ActionEventCount);
        Assert.Equal(2, metrics.IncomeActions);
        Assert.Equal(50, metrics.ActionEfficiencyPercent);
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `Compute_SummarizesActionSequencesRepetitionsAndEfficiency` dengan hasil bertipe `void`; operasi ini menangani compute
    // summarizes aksi sequences repetitions dan efficiency.
    public void Compute_SummarizesActionSequencesRepetitionsAndEfficiency()
    // Membuka scope metode Compute_SummarizesActionSequencesRepetitionsAndEfficiency; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // Compute_SummarizesActionSequencesRepetitionsAndEfficiency.
    {
        // Menyiapkan variabel lokal `playerId` untuk nilai pemain identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var playerId = Guid.NewGuid();
        // Menyiapkan variabel lokal `sessionId` untuk identitas unik sesi permainan yang menjadi batas data operasi ini dengan memanggil `Guid.NewGuid`
        // dengan tanpa argumen. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var sessionId = Guid.NewGuid();
        // Menyiapkan variabel lokal `freelanceEventId` untuk nilai freelance event identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var freelanceEventId = Guid.NewGuid();
        // Menyiapkan variabel lokal `orderEventId` untuk nilai urutan/pesanan event identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var orderEventId = Guid.NewGuid();
        // Menyiapkan variabel lokal `events` untuk kumpulan event permainan sebagai sumber riwayat untuk validasi atau perhitungan dengan objek baru
        // bertipe `List<EventDb>` dengan nilai awal sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var events = new List<EventDb>
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // Compute_SummarizesActionSequencesRepetitionsAndEfficiency.
        {
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `Guid.NewGuid()`, `sessionId`, `playerId`, `”BahanMasakan”`, `0`, `1`, `1` dalam
            // Compute_SummarizesActionSequencesRepetitionsAndEfficiency.
            CreateEvent(Guid.NewGuid(), sessionId, playerId, "BahanMasakan", dayIndex: 0, actionSlot: 1, sequence: 1),
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `Guid.NewGuid()`, `sessionId`, `playerId`, `”BahanMasakan”`, `0`, `2`, `2` dalam
            // Compute_SummarizesActionSequencesRepetitionsAndEfficiency.
            CreateEvent(Guid.NewGuid(), sessionId, playerId, "BahanMasakan", dayIndex: 0, actionSlot: 2, sequence: 2),
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `Guid.NewGuid()`, `sessionId`, `playerId`, `”AkhirGiliran”`, `0`, `2`, `3` dalam
            // Compute_SummarizesActionSequencesRepetitionsAndEfficiency.
            CreateEvent(Guid.NewGuid(), sessionId, playerId, "AkhirGiliran", dayIndex: 0, actionSlot: 2, sequence: 3),
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `freelanceEventId`, `sessionId`, `playerId`, `”KerjaLepas”`, `1`, `1`, `4` dalam
            // Compute_SummarizesActionSequencesRepetitionsAndEfficiency.
            CreateEvent(freelanceEventId, sessionId, playerId, "KerjaLepas", dayIndex: 1, actionSlot: 1, sequence: 4),
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `orderEventId`, `sessionId`, `playerId`, `”JualMasakan”`, `1`, `2`, `5` dalam
            // Compute_SummarizesActionSequencesRepetitionsAndEfficiency.
            CreateEvent(orderEventId, sessionId, playerId, "JualMasakan", dayIndex: 1, actionSlot: 2, sequence: 5)
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // Compute_SummarizesActionSequencesRepetitionsAndEfficiency.
        };
        // Menyiapkan variabel lokal `projections` untuk proyeksi transaksi arus kas yang diturunkan dari event permainan dengan objek baru bertipe
        // `List<CashflowProjectionDb>` dengan nilai awal sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var projections = new List<CashflowProjectionDb>
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // Compute_SummarizesActionSequencesRepetitionsAndEfficiency.
        {
            // Melanjutkan pengolahan dengan memanggil `CreateProjection` dengan `freelanceEventId`, `sessionId`, `playerId`, `”IN”`, `5`, `”FREELANCE”` dalam
            // Compute_SummarizesActionSequencesRepetitionsAndEfficiency.
            CreateProjection(freelanceEventId, sessionId, playerId, "IN", 5, "FREELANCE"),
            // Melanjutkan pengolahan dengan memanggil `CreateProjection` dengan `orderEventId`, `sessionId`, `playerId`, `”IN”`, `12`, `”ORDER_INCOME”` dalam
            // Compute_SummarizesActionSequencesRepetitionsAndEfficiency.
            CreateProjection(orderEventId, sessionId, playerId, "IN", 12, "ORDER_INCOME")
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // Compute_SummarizesActionSequencesRepetitionsAndEfficiency.
        };

        // Menyiapkan variabel lokal `metrics` untuk nilai metrics dengan memanggil `new ActionUsageCalculator().Compute` dengan `events`, `projections`,
        // `1`, `2`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var metrics = new ActionUsageCalculator().Compute(events, projections, latestDayIndex: 1, actionsPerTurn: 2);

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`2`, `metrics.ActionSequences.Count`);
        // pengujian gagal jika keduanya berbeda dalam Compute_SummarizesActionSequencesRepetitionsAndEfficiency.
        Assert.Equal(2, metrics.ActionSequences.Count);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`0`, `metrics.ActionSequences[0].DayIndex`);
        // pengujian gagal jika keduanya berbeda dalam Compute_SummarizesActionSequencesRepetitionsAndEfficiency.
        Assert.Equal(0, metrics.ActionSequences[0].DayIndex);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`new[] { ”BahanMasakan”, ”BahanMasakan” }`,
        // `metrics.ActionSequences[0].Actions`); pengujian gagal jika keduanya berbeda dalam Compute_SummarizesActionSequencesRepetitionsAndEfficiency.
        Assert.Equal(new[] { "BahanMasakan", "BahanMasakan" }, metrics.ActionSequences[0].Actions);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`1`, `metrics.ActionSequences[1].DayIndex`);
        // pengujian gagal jika keduanya berbeda dalam Compute_SummarizesActionSequencesRepetitionsAndEfficiency.
        Assert.Equal(1, metrics.ActionSequences[1].DayIndex);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`new[] { ”KerjaLepas”, ”JualMasakan” }`,
        // `metrics.ActionSequences[1].Actions`); pengujian gagal jika keduanya berbeda dalam Compute_SummarizesActionSequencesRepetitionsAndEfficiency.
        Assert.Equal(new[] { "KerjaLepas", "JualMasakan" }, metrics.ActionSequences[1].Actions);

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`2`, `metrics.ActionRepetitions.Count`);
        // pengujian gagal jika keduanya berbeda dalam Compute_SummarizesActionSequencesRepetitionsAndEfficiency.
        Assert.Equal(2, metrics.ActionRepetitions.Count);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`0`, `metrics.ActionRepetitions[0].DayIndex`);
        // pengujian gagal jika keduanya berbeda dalam Compute_SummarizesActionSequencesRepetitionsAndEfficiency.
        Assert.Equal(0, metrics.ActionRepetitions[0].DayIndex);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`2`,
        // `metrics.ActionRepetitions[0].TotalActions`); pengujian gagal jika keduanya berbeda dalam
        // Compute_SummarizesActionSequencesRepetitionsAndEfficiency.
        Assert.Equal(2, metrics.ActionRepetitions[0].TotalActions);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`1`,
        // `metrics.ActionRepetitions[0].DistinctActions`); pengujian gagal jika keduanya berbeda dalam
        // Compute_SummarizesActionSequencesRepetitionsAndEfficiency.
        Assert.Equal(1, metrics.ActionRepetitions[0].DistinctActions);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`1`,
        // `metrics.ActionRepetitions[0].RepeatedActions`); pengujian gagal jika keduanya berbeda dalam
        // Compute_SummarizesActionSequencesRepetitionsAndEfficiency.
        Assert.Equal(1, metrics.ActionRepetitions[0].RepeatedActions);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`0.5`,
        // `metrics.ActionRepetitions[0].DiversityScore`); pengujian gagal jika keduanya berbeda dalam
        // Compute_SummarizesActionSequencesRepetitionsAndEfficiency.
        Assert.Equal(0.5, metrics.ActionRepetitions[0].DiversityScore);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`1`,
        // `metrics.ActionRepetitions[1].DiversityScore`); pengujian gagal jika keduanya berbeda dalam
        // Compute_SummarizesActionSequencesRepetitionsAndEfficiency.
        Assert.Equal(1, metrics.ActionRepetitions[1].DiversityScore);

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`4`, `metrics.ActionSlotTimeline.Count`);
        // pengujian gagal jika keduanya berbeda dalam Compute_SummarizesActionSequencesRepetitionsAndEfficiency.
        Assert.Equal(4, metrics.ActionSlotTimeline.Count);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`2`,
        // `metrics.ActionSlotTimeline[3].ActionSlot`); pengujian gagal jika keduanya berbeda dalam
        // Compute_SummarizesActionSequencesRepetitionsAndEfficiency.
        Assert.Equal(2, metrics.ActionSlotTimeline[3].ActionSlot);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”JualMasakan”`,
        // `metrics.ActionSlotTimeline[3].ActionType`); pengujian gagal jika keduanya berbeda dalam
        // Compute_SummarizesActionSequencesRepetitionsAndEfficiency.
        Assert.Equal("JualMasakan", metrics.ActionSlotTimeline[3].ActionType);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`2`, `metrics.LatestActionSlot`); pengujian
        // gagal jika keduanya berbeda dalam Compute_SummarizesActionSequencesRepetitionsAndEfficiency.
        Assert.Equal(2, metrics.LatestActionSlot);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`1`, `metrics.LatestDayIndex`); pengujian gagal
        // jika keduanya berbeda dalam Compute_SummarizesActionSequencesRepetitionsAndEfficiency.
        Assert.Equal(1, metrics.LatestDayIndex);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`4`, `metrics.ActionEventCount`); pengujian
        // gagal jika keduanya berbeda dalam Compute_SummarizesActionSequencesRepetitionsAndEfficiency.
        Assert.Equal(4, metrics.ActionEventCount);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`2`, `metrics.IncomeActions`); pengujian gagal
        // jika keduanya berbeda dalam Compute_SummarizesActionSequencesRepetitionsAndEfficiency.
        Assert.Equal(2, metrics.IncomeActions);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`0.5`, `metrics.ActionEfficiency`); pengujian
        // gagal jika keduanya berbeda dalam Compute_SummarizesActionSequencesRepetitionsAndEfficiency.
        Assert.Equal(0.5, metrics.ActionEfficiency);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`50`, `metrics.ActionEfficiencyPercent`);
        // pengujian gagal jika keduanya berbeda dalam Compute_SummarizesActionSequencesRepetitionsAndEfficiency.
        Assert.Equal(50, metrics.ActionEfficiencyPercent);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`0.75`, `metrics.ActionDiversityAverage`);
        // pengujian gagal jika keduanya berbeda dalam Compute_SummarizesActionSequencesRepetitionsAndEfficiency.
        Assert.Equal(0.75, metrics.ActionDiversityAverage);
    // Menutup scope metode Compute_SummarizesActionSequencesRepetitionsAndEfficiency; bagian berikut berada di luar batas blok tersebut dalam
    // Compute_SummarizesActionSequencesRepetitionsAndEfficiency.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `Compute_IgnoresLegacySkippedOrderAndExcludesFreeActions` dengan hasil bertipe `void`; operasi ini menangani compute
    // ignores legacy skipped urutan/pesanan dan excludes free aksi.
    public void Compute_IgnoresLegacySkippedOrderAndExcludesFreeActions()
    // Membuka scope metode Compute_IgnoresLegacySkippedOrderAndExcludesFreeActions; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // Compute_IgnoresLegacySkippedOrderAndExcludesFreeActions.
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
        // Compute_IgnoresLegacySkippedOrderAndExcludesFreeActions.
        {
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `Guid.NewGuid()`, `sessionId`, `playerId`, `”LewatiOrder”`, `0`, `1`, `1` dalam
            // Compute_IgnoresLegacySkippedOrderAndExcludesFreeActions.
            CreateEvent(Guid.NewGuid(), sessionId, playerId, "LewatiOrder", dayIndex: 0, actionSlot: 1, sequence: 1),
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `Guid.NewGuid()`, `sessionId`, `playerId`, `”BuangBahanMasakan”`, `0`, `2`, `2`
            // dalam Compute_IgnoresLegacySkippedOrderAndExcludesFreeActions.
            CreateEvent(Guid.NewGuid(), sessionId, playerId, "BuangBahanMasakan", dayIndex: 0, actionSlot: 2, sequence: 2),
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `Guid.NewGuid()`, `sessionId`, `playerId`, `”RisikoKehidupan”`, `0`, `0`, `3` dalam
            // Compute_IgnoresLegacySkippedOrderAndExcludesFreeActions.
            CreateEvent(Guid.NewGuid(), sessionId, playerId, "RisikoKehidupan", dayIndex: 0, actionSlot: 0, sequence: 3)
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // Compute_IgnoresLegacySkippedOrderAndExcludesFreeActions.
        };

        // Menyiapkan variabel lokal `metrics` untuk nilai metrics dengan memanggil `new ActionUsageCalculator().Compute` dengan `events`, `[]`, `0`, `2`.
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var metrics = new ActionUsageCalculator().Compute(events, [], latestDayIndex: 0, actionsPerTurn: 2);

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`new[] { ”BuangBahanMasakan” }`,
        // `metrics.ActionSequences.Single().Actions`); pengujian gagal jika keduanya berbeda dalam Compute_IgnoresLegacySkippedOrderAndExcludesFreeActions.
        Assert.Equal(new[] { "BuangBahanMasakan" }, metrics.ActionSequences.Single().Actions);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`1`, `metrics.ActionEventCount`); pengujian
        // gagal jika keduanya berbeda dalam Compute_IgnoresLegacySkippedOrderAndExcludesFreeActions.
        Assert.Equal(1, metrics.ActionEventCount);
    // Menutup scope metode Compute_IgnoresLegacySkippedOrderAndExcludesFreeActions; bagian berikut berada di luar batas blok tersebut dalam
    // Compute_IgnoresLegacySkippedOrderAndExcludesFreeActions.
    }

    // Mendefinisikan metode `CreateEvent` dengan hasil bertipe `EventDb`; operasi ini menangani create event. Masukan: Parameter `eventId` bertipe
    // `Guid` membawa identitas unik event untuk pencatatan dan pemeriksaan duplikasi; Parameter `sessionId` bertipe `Guid` membawa identitas unik sesi
    // permainan yang menjadi batas data operasi ini; Parameter `playerId` bertipe `Guid` membawa nilai pemain identitas; Parameter `actionType` bertipe
    // `string` membawa nilai aksi jenis; Parameter `dayIndex` bertipe `int` membawa nilai hari index; Parameter `actionSlot` bertipe `int` membawa
    // nilai aksi slot; Parameter `sequence` bertipe `long` membawa nomor urut event yang menentukan urutan pemrosesan riwayat permainan.
    private static EventDb CreateEvent(
        // Parameter `eventId` bertipe `Guid` membawa identitas unik event untuk pencatatan dan pemeriksaan duplikasi.
        Guid eventId,
        // Parameter `sessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini.
        Guid sessionId,
        // Parameter `playerId` bertipe `Guid` membawa nilai pemain identitas.
        Guid playerId,
        // Parameter `actionType` bertipe `string` membawa nilai aksi jenis.
        string actionType,
        // Parameter `dayIndex` bertipe `int` membawa nilai hari index.
        int dayIndex,
        // Parameter `actionSlot` bertipe `int` membawa nilai aksi slot.
        int actionSlot,
        // Parameter `sequence` bertipe `long` membawa nomor urut event yang menentukan urutan pemrosesan riwayat permainan.
        long sequence)
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
            // Memperbarui `DayIndex` menggunakan `dayIndex` (nilai hari index) dalam CreateEvent.
            DayIndex = dayIndex,
            // Memperbarui `Weekday` menggunakan nilai literal `”MON”` dalam CreateEvent.
            Weekday = "MON",
            // Memperbarui `ActionSlot` menggunakan `actionSlot` (nilai aksi slot) dalam CreateEvent.
            ActionSlot = actionSlot,
            // Memperbarui `SequenceNumber` menggunakan `sequence` (nomor urut event yang menentukan urutan pemrosesan riwayat permainan) dalam CreateEvent.
            SequenceNumber = sequence,
            // Memperbarui `ActionType` menggunakan `actionType` (nilai aksi jenis) dalam CreateEvent.
            ActionType = actionType,
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
    // dalam operasi; Parameter `category` bertipe `string` membawa nilai category.
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
            // Memperbarui `EventId` menggunakan `eventId` (identitas unik event untuk pencatatan dan pemeriksaan duplikasi) dalam CreateProjection.
            EventId = eventId,
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
// Menutup scope tipe AnalyticsActionUsageCalculatorTests; bagian berikut berada di luar batas blok tersebut.
}
