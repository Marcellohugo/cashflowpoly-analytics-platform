// Fungsi file: Memverifikasi perilaku API, database, atau domain melalui AnalyticsIncomeDiversificationCalculatorTests.
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;
// Mengimpor namespace `Cashflowpoly.Api.Domain` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Domain;
// Mengimpor namespace `Xunit` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Xunit;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Tests` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Tests;

// Mendefinisikan tipe class `AnalyticsIncomeDiversificationCalculatorTests`; sealed mencegah tipe ini diturunkan lagi.
public sealed class AnalyticsIncomeDiversificationCalculatorTests
// Membuka scope tipe AnalyticsIncomeDiversificationCalculatorTests; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `Compute_CalculatesIncomeSharesAndDiversificationIndex` dengan hasil bertipe `void`; operasi ini menangani compute
    // calculates pemasukan shares dan diversification index.
    public void Compute_CalculatesIncomeSharesAndDiversificationIndex()
    // Membuka scope metode Compute_CalculatesIncomeSharesAndDiversificationIndex; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // Compute_CalculatesIncomeSharesAndDiversificationIndex.
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
        // Compute_CalculatesIncomeSharesAndDiversificationIndex.
        {
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `sessionId`, `playerId`, `”KerjaLepas”`, `”””{”amount”:5}”””` dalam
            // Compute_CalculatesIncomeSharesAndDiversificationIndex.
            CreateEvent(sessionId, playerId, "KerjaLepas", """{"amount":5}"""),
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `sessionId`, `playerId`, `”KerjaLepas”`, `”””{”amount”:5}”””` dalam
            // Compute_CalculatesIncomeSharesAndDiversificationIndex.
            CreateEvent(sessionId, playerId, "KerjaLepas", """{"amount":5}""")
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // Compute_CalculatesIncomeSharesAndDiversificationIndex.
        };
        // Menyiapkan variabel lokal `projections` untuk proyeksi transaksi arus kas yang diturunkan dari event permainan dengan objek baru bertipe
        // `List<CashflowProjectionDb>` dengan nilai awal sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var projections = new List<CashflowProjectionDb>
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // Compute_CalculatesIncomeSharesAndDiversificationIndex.
        {
            // Melanjutkan pengolahan dengan memanggil `CreateProjection` dengan `sessionId`, `playerId`, `”IN”`, `10`, `”DONATION_RECEIVED”` dalam
            // Compute_CalculatesIncomeSharesAndDiversificationIndex.
            CreateProjection(sessionId, playerId, "IN", 10, "DONATION_RECEIVED"),
            // Melanjutkan pengolahan dengan memanggil `CreateProjection` dengan `sessionId`, `playerId`, `”IN”`, `60`, `”LOAN_TAKEN”` dalam
            // Compute_CalculatesIncomeSharesAndDiversificationIndex.
            CreateProjection(sessionId, playerId, "IN", 60, "LOAN_TAKEN")
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // Compute_CalculatesIncomeSharesAndDiversificationIndex.
        };

        // Menyiapkan variabel lokal `metrics` untuk nilai metrics dengan memanggil `new IncomeDiversificationCalculator().Compute` dengan `events`, `20`,
        // `0`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var metrics = new IncomeDiversificationCalculator().Compute(
            // Meneruskan `events` (kumpulan event permainan sebagai sumber riwayat untuk validasi atau perhitungan) sebagai argumen ke `new
            // IncomeDiversificationCalculator().Compute`.
            events,
            // Meneruskan nilai literal `20` sebagai argumen bernama `mealOrderIncomeTotal`.
            mealOrderIncomeTotal: 20,
            // Meneruskan nilai literal `0` sebagai argumen bernama `goldInvestmentEarned`.
            goldInvestmentEarned: 0);

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`10`, `metrics.FreelanceIncome`); pengujian
        // gagal jika keduanya berbeda dalam Compute_CalculatesIncomeSharesAndDiversificationIndex.
        Assert.Equal(10, metrics.FreelanceIncome);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`20`, `metrics.MealIncome`); pengujian gagal
        // jika keduanya berbeda dalam Compute_CalculatesIncomeSharesAndDiversificationIndex.
        Assert.Equal(20, metrics.MealIncome);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`0`, `metrics.GoldIncome`); pengujian gagal
        // jika keduanya berbeda dalam Compute_CalculatesIncomeSharesAndDiversificationIndex.
        Assert.Equal(0, metrics.GoldIncome);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`2`, `metrics.ActiveIncomeSourceCount`);
        // pengujian gagal jika keduanya berbeda dalam Compute_CalculatesIncomeSharesAndDiversificationIndex.
        Assert.Equal(2, metrics.ActiveIncomeSourceCount);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`1d / 3d`,
        // `metrics.IncomeShares[”freelance_income”]`, `6`); pengujian gagal jika keduanya berbeda dalam
        // Compute_CalculatesIncomeSharesAndDiversificationIndex.
        Assert.Equal(1d / 3d, metrics.IncomeShares["freelance_income"], precision: 6);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`2d / 3d`,
        // `metrics.IncomeShares[”meal_order_income”]`, `6`); pengujian gagal jika keduanya berbeda dalam
        // Compute_CalculatesIncomeSharesAndDiversificationIndex.
        Assert.Equal(2d / 3d, metrics.IncomeShares["meal_order_income"], precision: 6);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`88.888889`,
        // `metrics.IncomeDiversificationIndex!.Value`, `6`); pengujian gagal jika keduanya berbeda dalam
        // Compute_CalculatesIncomeSharesAndDiversificationIndex.
        Assert.Equal(88.888889, metrics.IncomeDiversificationIndex!.Value, precision: 6);
        // Menjalankan pemeriksaan bahwa `metrics.RequiresIncomeNote` bernilai salah; pengujian gagal jika kondisi justru terpenuhi dalam
        // Compute_CalculatesIncomeSharesAndDiversificationIndex.
        Assert.False(metrics.RequiresIncomeNote);
    // Menutup scope metode Compute_CalculatesIncomeSharesAndDiversificationIndex; bagian berikut berada di luar batas blok tersebut dalam
    // Compute_CalculatesIncomeSharesAndDiversificationIndex.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `Compute_ReportsIncomeNoteWhenTotalIncomeIsZero` dengan hasil bertipe `void`; operasi ini menangani compute reports
    // pemasukan note when total pemasukan berstatus zero.
    public void Compute_ReportsIncomeNoteWhenTotalIncomeIsZero()
    // Membuka scope metode Compute_ReportsIncomeNoteWhenTotalIncomeIsZero; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // Compute_ReportsIncomeNoteWhenTotalIncomeIsZero.
    {
        // Menyiapkan variabel lokal `metrics` untuk nilai metrics dengan memanggil `new IncomeDiversificationCalculator().Compute` dengan
        // `Array.Empty<EventDb>()`, `0`, `0`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var metrics = new IncomeDiversificationCalculator().Compute(
            // Meneruskan memanggil `Array.Empty<EventDb>` dengan tanpa argumen sebagai argumen ke `new IncomeDiversificationCalculator().Compute`.
            Array.Empty<EventDb>(),
            // Meneruskan nilai literal `0` sebagai argumen bernama `mealOrderIncomeTotal`.
            mealOrderIncomeTotal: 0,
            // Meneruskan nilai literal `0` sebagai argumen bernama `goldInvestmentEarned`.
            goldInvestmentEarned: 0);

        // Menjalankan pemeriksaan hasil dengan `Assert.Empty` menggunakan `metrics.IncomeShares`; ketidaksesuaian dengan ekspektasi membuat pengujian gagal
        // dalam Compute_ReportsIncomeNoteWhenTotalIncomeIsZero.
        Assert.Empty(metrics.IncomeShares);
        // Menjalankan pemeriksaan Null atas `metrics.IncomeDiversificationIndex` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // Compute_ReportsIncomeNoteWhenTotalIncomeIsZero.
        Assert.Null(metrics.IncomeDiversificationIndex);
        // Menjalankan pemeriksaan bahwa `metrics.RequiresIncomeNote` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // Compute_ReportsIncomeNoteWhenTotalIncomeIsZero.
        Assert.True(metrics.RequiresIncomeNote);
    // Menutup scope metode Compute_ReportsIncomeNoteWhenTotalIncomeIsZero; bagian berikut berada di luar batas blok tersebut dalam
    // Compute_ReportsIncomeNoteWhenTotalIncomeIsZero.
    }

    // Mendefinisikan metode `CreateEvent` dengan hasil bertipe `EventDb`; operasi ini menangani create event. Masukan: Parameter `sessionId` bertipe
    // `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini; Parameter `playerId` bertipe `Guid` membawa nilai pemain
    // identitas; Parameter `actionType` bertipe `string` membawa nilai aksi jenis; Parameter `payload` bertipe `string` membawa muatan detail event
    // dalam format JSON.
    private static EventDb CreateEvent(Guid sessionId, Guid playerId, string actionType, string payload)
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
// Menutup scope tipe AnalyticsIncomeDiversificationCalculatorTests; bagian berikut berada di luar batas blok tersebut.
}
