// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui IScoreCalculator.
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;
// Mengimpor namespace `Cashflowpoly.Api.Contracts` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Contracts;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Domain;

// Mendefinisikan interface sebagai kontrak operasi `IScoreCalculator`.
public interface IScoreCalculator
// Membuka scope tipe IScoreCalculator; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan metode `BuildSummary` dengan hasil bertipe `AnalyticsSessionSummary`; operasi ini menangani build summary. Masukan: Parameter
    // `events` bertipe `List<EventDb>` membawa kumpulan event permainan sebagai sumber riwayat untuk validasi atau perhitungan; Parameter `projections`
    // bertipe `List<CashflowProjectionDb>` membawa proyeksi transaksi arus kas yang diturunkan dari event permainan.
    AnalyticsSessionSummary BuildSummary(
        // Parameter `events` bertipe `List<EventDb>` membawa kumpulan event permainan sebagai sumber riwayat untuk validasi atau perhitungan.
        List<EventDb> events,
        // Parameter `projections` bertipe `List<CashflowProjectionDb>` membawa proyeksi transaksi arus kas yang diturunkan dari event permainan.
        List<CashflowProjectionDb> projections);

    // Mendefinisikan metode `ComputeLearningPerformanceScore` dengan hasil bertipe `double?`; operasi ini menangani compute pembelajaran performa skor.
    // Masukan: Parameter `cashInTotal` bertipe `double` membawa jumlah seluruh pemasukan arus kas; Parameter `cashOutTotal` bertipe `double` membawa
    // jumlah seluruh pengeluaran arus kas; Parameter `happinessPointsTotal` bertipe `double` membawa akumulasi poin kebahagiaan pemain; Parameter
    // `fulfillmentDiversity` bertipe `double?` membawa tingkat keberagaman kategori kebutuhan yang telah dipenuhi; nilai null diizinkan ketika data
    // opsional belum tersedia.
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

    // Mendefinisikan metode `ComputeMissionPerformanceScore` dengan hasil bertipe `double`; operasi ini menangani compute misi performa skor. Masukan:
    // Parameter `missionPenaltyTotal` bertipe `double` membawa nilai misi penalti total; Parameter `loanPenaltyTotal` bertipe `double` membawa nilai
    // pinjaman penalti total.
    double ComputeMissionPerformanceScore(double missionPenaltyTotal, double loanPenaltyTotal);

    // Mendefinisikan metode `AverageNullable` dengan hasil bertipe `double?`; operasi ini menangani rata-rata nullable. Masukan: Parameter `values`
    // bertipe `IEnumerable<double?>` membawa nilai nilai.
    double? AverageNullable(IEnumerable<double?> values);
// Menutup scope tipe IScoreCalculator; bagian berikut berada di luar batas blok tersebut.
}
