// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui IHappinessCalculator.
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Domain;

// Mendefinisikan interface sebagai kontrak operasi `IHappinessCalculator`.
public interface IHappinessCalculator
// Membuka scope tipe IHappinessCalculator; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan metode `ComputeByPlayer` dengan hasil bertipe `Dictionary<Guid, AnalyticsHappinessBreakdown>`; operasi ini menangani compute
    // berdasarkan pemain. Masukan: Parameter `events` bertipe `List<EventDb>` membawa kumpulan event permainan sebagai sumber riwayat untuk validasi
    // atau perhitungan; Parameter `projections` bertipe `List<CashflowProjectionDb>` membawa proyeksi transaksi arus kas yang diturunkan dari event
    // permainan; Parameter `config` bertipe `RulesetConfig?` membawa konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan; nilai
    // null diizinkan ketika data opsional belum tersedia.
    Dictionary<Guid, AnalyticsHappinessBreakdown> ComputeByPlayer(
        // Parameter `events` bertipe `List<EventDb>` membawa kumpulan event permainan sebagai sumber riwayat untuk validasi atau perhitungan.
        List<EventDb> events,
        // Parameter `projections` bertipe `List<CashflowProjectionDb>` membawa proyeksi transaksi arus kas yang diturunkan dari event permainan.
        List<CashflowProjectionDb> projections,
        // Parameter `config` bertipe `RulesetConfig?` membawa konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan; nilai null
        // diizinkan ketika data opsional belum tersedia.
        RulesetConfig? config);

    // Mendefinisikan metode `ComputePensionRanks` dengan hasil bertipe `Dictionary<Guid, int>`; operasi ini menangani compute pension ranks. Masukan:
    // Parameter `events` bertipe `List<EventDb>` membawa kumpulan event permainan sebagai sumber riwayat untuk validasi atau perhitungan; Parameter
    // `projections` bertipe `List<CashflowProjectionDb>` membawa proyeksi transaksi arus kas yang diturunkan dari event permainan; Parameter `config`
    // bertipe `RulesetConfig` membawa konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan.
    Dictionary<Guid, int> ComputePensionRanks(
        // Parameter `events` bertipe `List<EventDb>` membawa kumpulan event permainan sebagai sumber riwayat untuk validasi atau perhitungan.
        List<EventDb> events,
        // Parameter `projections` bertipe `List<CashflowProjectionDb>` membawa proyeksi transaksi arus kas yang diturunkan dari event permainan.
        List<CashflowProjectionDb> projections,
        // Parameter `config` bertipe `RulesetConfig` membawa konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan.
        RulesetConfig config);

    // Mendefinisikan metode `ComputeBreakdown` dengan hasil bertipe `AnalyticsHappinessBreakdown`; operasi ini menangani compute breakdown. Masukan:
    // Parameter `playerEvents` bertipe `List<EventDb>` membawa nilai pemain event; Parameter `donationPoints` bertipe `double` membawa nilai donasi
    // poin; Parameter `goldPoints` bertipe `double` membawa nilai emas poin; Parameter `pensionPoints` bertipe `double` membawa nilai pension poin.
    AnalyticsHappinessBreakdown ComputeBreakdown(
        // Parameter `playerEvents` bertipe `List<EventDb>` membawa nilai pemain event.
        List<EventDb> playerEvents,
        // Parameter `donationPoints` bertipe `double` membawa nilai donasi poin.
        double donationPoints,
        // Parameter `goldPoints` bertipe `double` membawa nilai emas poin.
        double goldPoints,
        // Parameter `pensionPoints` bertipe `double` membawa nilai pension poin.
        double pensionPoints);

    // Mendefinisikan metode `SumRankAwarded` dengan hasil bertipe `double`; operasi ini menangani sum rank awarded. Masukan: Parameter `events` bertipe
    // `IEnumerable<EventDb>` membawa kumpulan event permainan sebagai sumber riwayat untuk validasi atau perhitungan; Parameter `actionType` bertipe
    // `string` membawa nilai aksi jenis.
    double SumRankAwarded(IEnumerable<EventDb> events, string actionType);

    // Mendefinisikan metode `SumPointsAwarded` dengan hasil bertipe `double`; operasi ini menangani sum poin awarded. Masukan: Parameter `events`
    // bertipe `IEnumerable<EventDb>` membawa kumpulan event permainan sebagai sumber riwayat untuk validasi atau perhitungan; Parameter `actionType`
    // bertipe `string` membawa nilai aksi jenis.
    double SumPointsAwarded(IEnumerable<EventDb> events, string actionType);
// Menutup scope tipe IHappinessCalculator; bagian berikut berada di luar batas blok tersebut.
}
