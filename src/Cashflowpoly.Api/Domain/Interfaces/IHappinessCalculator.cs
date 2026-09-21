// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui IHappinessCalculator.
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Domain;

public interface IHappinessCalculator
{
    Dictionary<Guid, AnalyticsHappinessBreakdown> ComputeByPlayer(
        // Parameter `events` bertipe `List<EventDb>` membawa kumpulan event permainan sebagai sumber riwayat untuk validasi atau perhitungan.
        List<EventDb> events,
        // Parameter `projections` bertipe `List<CashflowProjectionDb>` membawa proyeksi transaksi arus kas yang diturunkan dari event permainan.
        List<CashflowProjectionDb> projections,
        // Parameter `config` bertipe `RulesetConfig?` membawa konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan; nilai null
        // diizinkan ketika data opsional belum tersedia.
        RulesetConfig? config,
        IEnumerable<Guid>? participantIds = null);

    Dictionary<Guid, int> ComputePensionRanks(
        // Parameter `events` bertipe `List<EventDb>` membawa kumpulan event permainan sebagai sumber riwayat untuk validasi atau perhitungan.
        List<EventDb> events,
        // Parameter `projections` bertipe `List<CashflowProjectionDb>` membawa proyeksi transaksi arus kas yang diturunkan dari event permainan.
        List<CashflowProjectionDb> projections,
        // Parameter `config` bertipe `RulesetConfig` membawa konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan.
        RulesetConfig config,
        IEnumerable<Guid>? participantIds = null);

    AnalyticsHappinessBreakdown ComputeBreakdown(
        // Parameter `playerEvents` bertipe `List<EventDb>` membawa nilai pemain event.
        List<EventDb> playerEvents,
        // Parameter `donationPoints` bertipe `double` membawa nilai donasi poin.
        double donationPoints,
        // Parameter `goldPoints` bertipe `double` membawa nilai emas poin.
        double goldPoints,
        // Parameter `pensionPoints` bertipe `double` membawa nilai pension poin.
        double pensionPoints);

    double SumRankAwarded(IEnumerable<EventDb> events, string actionType);

    double SumPointsAwarded(IEnumerable<EventDb> events, string actionType);
}
