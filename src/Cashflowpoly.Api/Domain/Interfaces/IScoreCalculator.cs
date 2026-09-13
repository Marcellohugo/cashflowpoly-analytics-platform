// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui IScoreCalculator.
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;
// Mengimpor namespace `Cashflowpoly.Api.Contracts` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Contracts;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Domain;

public interface IScoreCalculator
{
    AnalyticsSessionSummary BuildSummary(
        // Parameter `events` bertipe `List<EventDb>` membawa kumpulan event permainan sebagai sumber riwayat untuk validasi atau perhitungan.
        List<EventDb> events,
        // Parameter `projections` bertipe `List<CashflowProjectionDb>` membawa proyeksi transaksi arus kas yang diturunkan dari event permainan.
        List<CashflowProjectionDb> projections);

    double? ComputeLearningPerformanceScore(
        // Parameter `cashInTotal` bertipe `double` membawa jumlah seluruh pemasukan arus kas.
        double cashInTotal,
        // Parameter `cashOutTotal` bertipe `double` membawa jumlah seluruh pengeluaran arus kas.
        double cashOutTotal,
        // Parameter `happinessPointsTotal` bertipe `double` membawa akumulasi poin kebahagiaan pemain.
        double happinessPointsTotal,
        // Parameter `fulfillmentDiversity` bertipe `double?` membawa tingkat keberagaman kategori kebutuhan yang telah dipenuhi; nilai null diizinkan
        // ketika data opsional belum tersedia.
        double? fulfillmentDiversity);

    double ComputeMissionPerformanceScore(double missionPenaltyTotal, double loanPenaltyTotal);

    double? AverageNullable(IEnumerable<double?> values);
}
