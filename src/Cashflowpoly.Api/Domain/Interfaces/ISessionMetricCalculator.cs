// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui ISessionMetricCalculator.
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Domain;

// Mendefinisikan interface sebagai kontrak operasi `ISessionMetricCalculator`.
public interface ISessionMetricCalculator
// Membuka scope tipe ISessionMetricCalculator; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan metode `ComputeSessionMetrics` dengan hasil bertipe `Dictionary<string, (double? Numeric, string? Json)>`; operasi ini menangani
    // compute sesi metrics. Masukan: Parameter `events` bertipe `List<EventDb>` membawa kumpulan event permainan sebagai sumber riwayat untuk validasi
    // atau perhitungan; Parameter `projections` bertipe `List<CashflowProjectionDb>` membawa proyeksi transaksi arus kas yang diturunkan dari event
    // permainan; Parameter `happinessByPlayer` bertipe `Dictionary<Guid, AnalyticsHappinessBreakdown>` membawa nilai kebahagiaan berdasarkan pemain.
    Dictionary<string, (double? Numeric, string? Json)> ComputeSessionMetrics(
        // Parameter `events` bertipe `List<EventDb>` membawa kumpulan event permainan sebagai sumber riwayat untuk validasi atau perhitungan.
        List<EventDb> events,
        // Parameter `projections` bertipe `List<CashflowProjectionDb>` membawa proyeksi transaksi arus kas yang diturunkan dari event permainan.
        List<CashflowProjectionDb> projections,
        // Parameter `happinessByPlayer` bertipe `Dictionary<Guid, AnalyticsHappinessBreakdown>` membawa nilai kebahagiaan berdasarkan pemain.
        Dictionary<Guid, AnalyticsHappinessBreakdown> happinessByPlayer);
// Menutup scope tipe ISessionMetricCalculator; bagian berikut berada di luar batas blok tersebut.
}
