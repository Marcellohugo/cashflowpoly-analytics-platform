// Fungsi file: Memverifikasi perilaku API, database, atau domain melalui EventPlayerBalanceCalculatorTests.
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;
// Mengimpor namespace `Cashflowpoly.Api.Domain` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Domain;
// Mengimpor namespace `Xunit` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Xunit;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Tests` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Tests;

// Mendefinisikan tipe class `EventPlayerBalanceCalculatorTests`; sealed mencegah tipe ini diturunkan lagi.
public sealed class EventPlayerBalanceCalculatorTests
// Membuka scope tipe EventPlayerBalanceCalculatorTests; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `Compute_ReturnsStartingCashPlusPlayerNetCashflow` dengan hasil bertipe `void`; operasi ini menangani compute returns
    // starting uang tunai plus pemain net arus kas.
    public void Compute_ReturnsStartingCashPlusPlayerNetCashflow()
    // Membuka scope metode Compute_ReturnsStartingCashPlusPlayerNetCashflow; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // Compute_ReturnsStartingCashPlusPlayerNetCashflow.
    {
        // Menyiapkan variabel lokal `playerId` untuk nilai pemain identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var playerId = Guid.NewGuid();
        // Menyiapkan variabel lokal `otherPlayerId` untuk nilai other pemain identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var otherPlayerId = Guid.NewGuid();
        // Menyiapkan variabel lokal `projections` untuk proyeksi transaksi arus kas yang diturunkan dari event permainan dengan objek baru bertipe
        // `List<CashflowProjectionDb>` dengan nilai awal sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var projections = new List<CashflowProjectionDb>
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // Compute_ReturnsStartingCashPlusPlayerNetCashflow.
        {
            // Melanjutkan pengolahan dengan memanggil `CreateProjection` dengan `playerId`, `”IN”`, `12` dalam
            // Compute_ReturnsStartingCashPlusPlayerNetCashflow.
            CreateProjection(playerId, "IN", 12),
            // Melanjutkan pengolahan dengan memanggil `CreateProjection` dengan `playerId`, `”OUT”`, `5` dalam
            // Compute_ReturnsStartingCashPlusPlayerNetCashflow.
            CreateProjection(playerId, "OUT", 5),
            // Melanjutkan pengolahan dengan memanggil `CreateProjection` dengan `otherPlayerId`, `”OUT”`, `99` dalam
            // Compute_ReturnsStartingCashPlusPlayerNetCashflow.
            CreateProjection(otherPlayerId, "OUT", 99)
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // Compute_ReturnsStartingCashPlusPlayerNetCashflow.
        };

        // Menyiapkan variabel lokal `balance` untuk saldo uang pemain pada keadaan yang sedang diproses dengan memanggil `new
        // EventPlayerBalanceCalculator().Compute` dengan `playerId`, `20`, `projections`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var balance = new EventPlayerBalanceCalculator().Compute(playerId, startingCash: 20, projections);

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`27`, `balance`); pengujian gagal jika keduanya
        // berbeda dalam Compute_ReturnsStartingCashPlusPlayerNetCashflow.
        Assert.Equal(27, balance);
    // Menutup scope metode Compute_ReturnsStartingCashPlusPlayerNetCashflow; bagian berikut berada di luar batas blok tersebut dalam
    // Compute_ReturnsStartingCashPlusPlayerNetCashflow.
    }

    // Mendefinisikan metode `CreateProjection` dengan hasil bertipe `CashflowProjectionDb`; operasi ini menangani create projection. Masukan: Parameter
    // `playerId` bertipe `Guid` membawa nilai pemain identitas; Parameter `direction` bertipe `string` membawa nilai direction; Parameter `amount`
    // bertipe `int` membawa nominal uang atau nilai transaksi yang dipakai dalam operasi.
    private static CashflowProjectionDb CreateProjection(Guid playerId, string direction, int amount)
    // Membuka scope metode CreateProjection; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CreateProjection.
    {
        // Mengembalikan objek baru bertipe `CashflowProjectionDb` dengan nilai awal sesuai konstruktornya kepada pemanggil dalam CreateProjection; eksekusi
        // jalur ini selesai setelah nilai hasil ditentukan.
        return new CashflowProjectionDb
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CreateProjection.
        {
            // Memperbarui `ProjectionId` menggunakan memanggil `Guid.NewGuid` dengan tanpa argumen dalam CreateProjection.
            ProjectionId = Guid.NewGuid(),
            // Memperbarui `SessionId` menggunakan memanggil `Guid.NewGuid` dengan tanpa argumen dalam CreateProjection.
            SessionId = Guid.NewGuid(),
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
            // Memperbarui `Category` menggunakan nilai literal `”TEST”` dalam CreateProjection.
            Category = "TEST"
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam CreateProjection.
        };
    // Menutup scope metode CreateProjection; bagian berikut berada di luar batas blok tersebut dalam CreateProjection.
    }
// Menutup scope tipe EventPlayerBalanceCalculatorTests; bagian berikut berada di luar batas blok tersebut.
}
