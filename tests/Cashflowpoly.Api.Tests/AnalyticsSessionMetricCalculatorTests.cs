// Fungsi file: Memverifikasi perilaku API, database, atau domain melalui AnalyticsSessionMetricCalculatorTests.
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;
// Mengimpor namespace `Cashflowpoly.Api.Domain` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Domain;
// Mengimpor namespace `Xunit` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Xunit;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Tests` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Tests;

// Mendefinisikan tipe class `AnalyticsSessionMetricCalculatorTests`; sealed mencegah tipe ini diturunkan lagi.
public sealed class AnalyticsSessionMetricCalculatorTests
// Membuka scope tipe AnalyticsSessionMetricCalculatorTests; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `ComputeSessionMetrics_ComputesCashflowDonationAndHappinessTotals` dengan hasil bertipe `void`; operasi ini menangani
    // compute sesi metrics computes arus kas donasi dan kebahagiaan totals.
    public void ComputeSessionMetrics_ComputesCashflowDonationAndHappinessTotals()
    // Membuka scope metode ComputeSessionMetrics_ComputesCashflowDonationAndHappinessTotals; pernyataan/deklarasi berikut berada di dalam batas blok
    // ini dalam ComputeSessionMetrics_ComputesCashflowDonationAndHappinessTotals.
    {
        // Menyiapkan variabel lokal `events` untuk kumpulan event permainan sebagai sumber riwayat untuk validasi atau perhitungan dengan objek baru
        // bertipe `List<EventDb>` dengan nilai awal sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var events = new List<EventDb>
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ComputeSessionMetrics_ComputesCashflowDonationAndHappinessTotals.
        {
            // Menggunakan objek baru dengan tipe mengikuti konteks tujuan dan argumen () sebagai bagian ekspresi yang sedang disusun dalam
            // ComputeSessionMetrics_ComputesCashflowDonationAndHappinessTotals.
            new()
            // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ComputeSessionMetrics_ComputesCashflowDonationAndHappinessTotals.
            {
                // Memperbarui `EventId` menggunakan memanggil `Guid.NewGuid` dengan tanpa argumen dalam
                // ComputeSessionMetrics_ComputesCashflowDonationAndHappinessTotals.
                EventId = Guid.NewGuid(),
                // Memperbarui `ActionType` menggunakan nilai literal `”JumatBerkah”` dalam ComputeSessionMetrics_ComputesCashflowDonationAndHappinessTotals.
                ActionType = "JumatBerkah",
                // Memperbarui `Payload` menggunakan nilai literal `”””{”amount”:5}”””` dalam ComputeSessionMetrics_ComputesCashflowDonationAndHappinessTotals.
                Payload = """{"amount":5}"""
            // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
            // ComputeSessionMetrics_ComputesCashflowDonationAndHappinessTotals.
            },
            // Menggunakan objek baru dengan tipe mengikuti konteks tujuan dan argumen () sebagai bagian ekspresi yang sedang disusun dalam
            // ComputeSessionMetrics_ComputesCashflowDonationAndHappinessTotals.
            new()
            // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ComputeSessionMetrics_ComputesCashflowDonationAndHappinessTotals.
            {
                // Memperbarui `EventId` menggunakan memanggil `Guid.NewGuid` dengan tanpa argumen dalam
                // ComputeSessionMetrics_ComputesCashflowDonationAndHappinessTotals.
                EventId = Guid.NewGuid(),
                // Memperbarui `ActionType` menggunakan nilai literal `”KerjaLepas”` dalam ComputeSessionMetrics_ComputesCashflowDonationAndHappinessTotals.
                ActionType = "KerjaLepas",
                // Memperbarui `Payload` menggunakan nilai literal `”””{”amount”:3}”””` dalam ComputeSessionMetrics_ComputesCashflowDonationAndHappinessTotals.
                Payload = """{"amount":3}"""
            // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
            // ComputeSessionMetrics_ComputesCashflowDonationAndHappinessTotals.
            }
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // ComputeSessionMetrics_ComputesCashflowDonationAndHappinessTotals.
        };
        // Menyiapkan variabel lokal `projections` untuk proyeksi transaksi arus kas yang diturunkan dari event permainan dengan objek baru bertipe
        // `List<CashflowProjectionDb>` dengan nilai awal sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var projections = new List<CashflowProjectionDb>
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ComputeSessionMetrics_ComputesCashflowDonationAndHappinessTotals.
        {
            // Menggunakan objek baru dengan tipe mengikuti konteks tujuan dan argumen () sebagai bagian ekspresi yang sedang disusun dalam
            // ComputeSessionMetrics_ComputesCashflowDonationAndHappinessTotals.
            new() { Direction = "IN", Amount = 10 },
            // Menggunakan objek baru dengan tipe mengikuti konteks tujuan dan argumen () sebagai bagian ekspresi yang sedang disusun dalam
            // ComputeSessionMetrics_ComputesCashflowDonationAndHappinessTotals.
            new() { Direction = "OUT", Amount = 4 },
            // Menggunakan objek baru dengan tipe mengikuti konteks tujuan dan argumen () sebagai bagian ekspresi yang sedang disusun dalam
            // ComputeSessionMetrics_ComputesCashflowDonationAndHappinessTotals.
            new() { Direction = "IN", Amount = 2 }
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // ComputeSessionMetrics_ComputesCashflowDonationAndHappinessTotals.
        };
        // Menyiapkan variabel lokal `happinessByPlayer` untuk nilai kebahagiaan berdasarkan pemain dengan objek baru bertipe `Dictionary<Guid,
        // AnalyticsHappinessBreakdown>` dengan nilai awal sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var happinessByPlayer = new Dictionary<Guid, AnalyticsHappinessBreakdown>
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ComputeSessionMetrics_ComputesCashflowDonationAndHappinessTotals.
        {
            // Memperbarui `[Guid.NewGuid()]` menggunakan objek baru dengan tipe mengikuti konteks tujuan dan argumen (11, 1, 2, 3, 4, 5, 6, 7, 8, false) dalam
            // ComputeSessionMetrics_ComputesCashflowDonationAndHappinessTotals.
            [Guid.NewGuid()] = new(11, 1, 2, 3, 4, 5, 6, 7, 8, false),
            // Memperbarui `[Guid.NewGuid()]` menggunakan objek baru dengan tipe mengikuti konteks tujuan dan argumen (-2, 0, 0, 0, 0, 0, 0, 1, 1, true) dalam
            // ComputeSessionMetrics_ComputesCashflowDonationAndHappinessTotals.
            [Guid.NewGuid()] = new(-2, 0, 0, 0, 0, 0, 0, 1, 1, true)
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // ComputeSessionMetrics_ComputesCashflowDonationAndHappinessTotals.
        };

        // Menyiapkan variabel lokal `metrics` untuk nilai metrics dengan memanggil `new SessionMetricCalculator().ComputeSessionMetrics` dengan `events`,
        // `projections`, `happinessByPlayer`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var metrics = new SessionMetricCalculator().ComputeSessionMetrics(events, projections, happinessByPlayer);

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`12`, `metrics[”cashflow.in.total”].Numeric`);
        // pengujian gagal jika keduanya berbeda dalam ComputeSessionMetrics_ComputesCashflowDonationAndHappinessTotals.
        Assert.Equal(12, metrics["cashflow.in.total"].Numeric);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`4`, `metrics[”cashflow.out.total”].Numeric`);
        // pengujian gagal jika keduanya berbeda dalam ComputeSessionMetrics_ComputesCashflowDonationAndHappinessTotals.
        Assert.Equal(4, metrics["cashflow.out.total"].Numeric);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`8`, `metrics[”cashflow.net.total”].Numeric`);
        // pengujian gagal jika keduanya berbeda dalam ComputeSessionMetrics_ComputesCashflowDonationAndHappinessTotals.
        Assert.Equal(8, metrics["cashflow.net.total"].Numeric);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`5`, `metrics[”donation.total”].Numeric`);
        // pengujian gagal jika keduanya berbeda dalam ComputeSessionMetrics_ComputesCashflowDonationAndHappinessTotals.
        Assert.Equal(5, metrics["donation.total"].Numeric);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`9`,
        // `metrics[”happiness.points.total”].Numeric`); pengujian gagal jika keduanya berbeda dalam
        // ComputeSessionMetrics_ComputesCashflowDonationAndHappinessTotals.
        Assert.Equal(9, metrics["happiness.points.total"].Numeric);
    // Menutup scope metode ComputeSessionMetrics_ComputesCashflowDonationAndHappinessTotals; bagian berikut berada di luar batas blok tersebut dalam
    // ComputeSessionMetrics_ComputesCashflowDonationAndHappinessTotals.
    }
// Menutup scope tipe AnalyticsSessionMetricCalculatorTests; bagian berikut berada di luar batas blok tersebut.
}
