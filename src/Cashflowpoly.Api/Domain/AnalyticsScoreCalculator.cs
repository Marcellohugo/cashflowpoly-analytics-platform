// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui AnalyticsScoreCalculator.
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;
// Mengimpor namespace `Cashflowpoly.Api.Contracts` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Contracts;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Domain;

/// <summary>
/// Kalkulator murni untuk ringkasan sesi dan skor performa analitik.
/// </summary>
// Mendefinisikan tipe class `ScoreCalculator` yang mewarisi atau menerapkan `IScoreCalculator`; sealed mencegah tipe ini diturunkan lagi.
internal sealed class ScoreCalculator : IScoreCalculator
{
    /// <summary>
    /// Membangun ringkasan analitik sesi dari event dan proyeksi cashflow.
    /// </summary>
    // Mendefinisikan metode `BuildSummary` dengan hasil bertipe `AnalyticsSessionSummary`. Membangun ringkasan analitik sesi dari event dan proyeksi
    // cashflow. Masukan: Parameter `events` bertipe `List<EventDb>` membawa kumpulan event permainan sebagai sumber riwayat untuk validasi atau
    // perhitungan; Parameter `projections` bertipe `List<CashflowProjectionDb>` membawa proyeksi transaksi arus kas yang diturunkan dari event
    // permainan.
    public AnalyticsSessionSummary BuildSummary(
        // Parameter `events` bertipe `List<EventDb>` membawa kumpulan event permainan sebagai sumber riwayat untuk validasi atau perhitungan.
        List<EventDb> events,
        // Parameter `projections` bertipe `List<CashflowProjectionDb>` membawa proyeksi transaksi arus kas yang diturunkan dari event permainan.
        List<CashflowProjectionDb> projections)
    {
        var cashInTotal = projections.Where(p => p.Direction == "IN").Sum(p => (double)p.Amount);
        var cashOutTotal = projections.Where(p => p.Direction == "OUT").Sum(p => (double)p.Amount);
        var cashflowNetTotal = cashInTotal - cashOutTotal;
        return new AnalyticsSessionSummary(events.Count, cashInTotal, cashOutTotal, cashflowNetTotal);
    }

    /// <summary>
    /// Menghitung skor performa pembelajaran berdasarkan cashflow, keberagaman kebutuhan, dan happiness dengan bobot tertimbang.
    /// </summary>
    // Mendefinisikan metode `ComputeLearningPerformanceScore` dengan hasil bertipe `double?`. Menghitung skor performa pembelajaran berdasarkan
    // cashflow, keberagaman kebutuhan, dan happiness dengan bobot tertimbang. Masukan: Parameter `cashInTotal` bertipe `double` membawa jumlah seluruh
    // pemasukan arus kas; Parameter `cashOutTotal` bertipe `double` membawa jumlah seluruh pengeluaran arus kas; Parameter `happinessPointsTotal`
    // bertipe `double` membawa akumulasi poin kebahagiaan pemain; Parameter `fulfillmentDiversity` bertipe `double?` membawa tingkat keberagaman
    // kategori kebutuhan yang telah dipenuhi; nilai null diizinkan ketika data opsional belum tersedia.
    public double? ComputeLearningPerformanceScore(
        // Parameter `cashInTotal` bertipe `double` membawa jumlah seluruh pemasukan arus kas.
        double cashInTotal,
        // Parameter `cashOutTotal` bertipe `double` membawa jumlah seluruh pengeluaran arus kas.
        double cashOutTotal,
        // Parameter `happinessPointsTotal` bertipe `double` membawa akumulasi poin kebahagiaan pemain.
        double happinessPointsTotal,
        // Parameter `fulfillmentDiversity` bertipe `double?` membawa tingkat keberagaman kategori kebutuhan yang telah dipenuhi; nilai null diizinkan
        // ketika data opsional belum tersedia.
        double? fulfillmentDiversity)
    {
        var cashflowComponent = AnalyticsMath.Clamp(50 + 5 * (cashInTotal - cashOutTotal), 0, 100);
        var fulfillmentComponent = fulfillmentDiversity.HasValue ? AnalyticsMath.Clamp(fulfillmentDiversity.Value * 100, 0, 100) : (double?)null;
        var happinessComponent = AnalyticsMath.Clamp(5 * happinessPointsTotal, 0, 100);

        return WeightedAverage(new (double? value, double weight)[]
        {
            (cashflowComponent, 0.40),
            (fulfillmentComponent, 0.35),
            (happinessComponent, 0.25)
        });
    }

    /// <summary>
    /// Menghitung skor performa misi berdasarkan penalti misi dan penalti pinjaman.
    /// </summary>
    // Mendefinisikan metode `ComputeMissionPerformanceScore` dengan hasil bertipe `double`. Menghitung skor performa misi berdasarkan penalti misi dan
    // penalti pinjaman. Masukan: Parameter `missionPenaltyTotal` bertipe `double` membawa nilai misi penalti total; Parameter `loanPenaltyTotal`
    // bertipe `double` membawa nilai pinjaman penalti total.
    public double ComputeMissionPerformanceScore(double missionPenaltyTotal, double loanPenaltyTotal)
    {
        var missionPenaltyComponent = AnalyticsMath.Clamp(missionPenaltyTotal + loanPenaltyTotal, 0, 100);
        return AnalyticsMath.Clamp(100 - missionPenaltyComponent, 0, 100);
    }

    /// <summary>
    /// Menghitung rata-rata dari nilai-nilai nullable, mengabaikan null.
    /// </summary>
    // Mendefinisikan metode `AverageNullable` dengan hasil bertipe `double?`. Menghitung rata-rata dari nilai-nilai nullable, mengabaikan null.
    // Masukan: Parameter `values` bertipe `IEnumerable<double?>` membawa nilai nilai.
    public double? AverageNullable(IEnumerable<double?> values)
    {
        var nonNull = values.Where(value => value.HasValue).Select(value => value!.Value).ToList();
        if (nonNull.Count == 0)
        {
            return null;
        }

        return Math.Round(nonNull.Average(), 2);
    }

    /// <summary>
    /// Menghitung rata-rata tertimbang dari komponen yang memiliki nilai, mengabaikan komponen null.
    /// </summary>
    // Mendefinisikan metode `WeightedAverage` dengan hasil bertipe `double?`. Menghitung rata-rata tertimbang dari komponen yang memiliki nilai,
    // mengabaikan komponen null. Masukan: Parameter `components` bertipe `IEnumerable<(double? value, double weight)>` membawa nilai komponen.
    private double? WeightedAverage(IEnumerable<(double? value, double weight)> components)
    {
        var active = components.Where(item => item.value.HasValue).ToList();
        if (active.Count == 0)
        {
            return null;
        }

        var weightTotal = active.Sum(item => item.weight);
        if (weightTotal <= 0)
        {
            return null;
        }

        var weightedScore = active.Sum(item => item.value!.Value * item.weight) / weightTotal;
        return Math.Round(weightedScore, 2);
    }
}
