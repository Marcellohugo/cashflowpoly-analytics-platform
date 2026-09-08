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
// Membuka scope tipe ScoreCalculator; pernyataan/deklarasi berikut berada di dalam batas blok ini.
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
    // Membuka scope metode BuildSummary; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildSummary.
    {
        // Menyiapkan variabel lokal `cashInTotal` untuk jumlah seluruh pemasukan arus kas dengan menjumlahkan nilai `projections.Where(p => p.Direction ==
        // ”IN”)` berdasarkan `p => (double)p.Amount`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var cashInTotal = projections.Where(p => p.Direction == "IN").Sum(p => (double)p.Amount);
        // Menyiapkan variabel lokal `cashOutTotal` untuk jumlah seluruh pengeluaran arus kas dengan menjumlahkan nilai `projections.Where(p => p.Direction
        // == ”OUT”)` berdasarkan `p => (double)p.Amount`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var cashOutTotal = projections.Where(p => p.Direction == "OUT").Sum(p => (double)p.Amount);
        // Menyiapkan variabel lokal `cashflowNetTotal` untuk selisih pemasukan terhadap pengeluaran arus kas dengan selisih antara `cashInTotal` dan
        // `cashOutTotal`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var cashflowNetTotal = cashInTotal - cashOutTotal;
        // Mengembalikan objek baru bertipe `AnalyticsSessionSummary` dengan argumen (events.Count, cashInTotal, cashOutTotal, cashflowNetTotal) kepada
        // pemanggil dalam BuildSummary; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return new AnalyticsSessionSummary(events.Count, cashInTotal, cashOutTotal, cashflowNetTotal);
    // Menutup scope metode BuildSummary; bagian berikut berada di luar batas blok tersebut dalam BuildSummary.
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
    // Membuka scope metode ComputeLearningPerformanceScore; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // ComputeLearningPerformanceScore.
    {
        // Menyiapkan variabel lokal `cashflowComponent` untuk nilai arus kas komponen dengan membatasi `50 + 5 * (cashInTotal - cashOutTotal)` agar tidak
        // lebih kecil dari `0` dan tidak lebih besar dari `100`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var cashflowComponent = AnalyticsMath.Clamp(50 + 5 * (cashInTotal - cashOutTotal), 0, 100);
        // Menyiapkan variabel lokal `fulfillmentComponent` untuk nilai pemenuhan komponen dengan hasil pemilihan bersyarat: ketika
        // `fulfillmentDiversity.HasValue` benar gunakan `AnalyticsMath.Clamp(fulfillmentDiversity.Value * 100, 0, 100)`, jika tidak gunakan
        // `(double?)null`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var fulfillmentComponent = fulfillmentDiversity.HasValue ? AnalyticsMath.Clamp(fulfillmentDiversity.Value * 100, 0, 100) : (double?)null;
        // Menyiapkan variabel lokal `happinessComponent` untuk nilai kebahagiaan komponen dengan membatasi `5 * happinessPointsTotal` agar tidak lebih
        // kecil dari `0` dan tidak lebih besar dari `100`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var happinessComponent = AnalyticsMath.Clamp(5 * happinessPointsTotal, 0, 100);

        // Mengembalikan memanggil `WeightedAverage` dengan `new (double? value, double weight)[] { (cashflowComponent, 0.40), (fulfillmentComponent, 0.35),
        // (happinessComponent, 0.25) }` kepada pemanggil dalam ComputeLearningPerformanceScore; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return WeightedAverage(new (double? value, double weight)[]
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ComputeLearningPerformanceScore.
        {
            // Meneruskan `cashflowComponent` (nilai arus kas komponen) sebagai argumen ke `WeightedAverage`; Meneruskan nilai literal `0.40` sebagai argumen ke
            // `WeightedAverage`.
            (cashflowComponent, 0.40),
            // Meneruskan `fulfillmentComponent` (nilai pemenuhan komponen) sebagai argumen ke `WeightedAverage`; Meneruskan nilai literal `0.35` sebagai
            // argumen ke `WeightedAverage`.
            (fulfillmentComponent, 0.35),
            // Meneruskan `happinessComponent` (nilai kebahagiaan komponen) sebagai argumen ke `WeightedAverage`; Meneruskan nilai literal `0.25` sebagai
            // argumen ke `WeightedAverage`.
            (happinessComponent, 0.25)
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // ComputeLearningPerformanceScore.
        });
    // Menutup scope metode ComputeLearningPerformanceScore; bagian berikut berada di luar batas blok tersebut dalam ComputeLearningPerformanceScore.
    }

    /// <summary>
    /// Menghitung skor performa misi berdasarkan penalti misi dan penalti pinjaman.
    /// </summary>
    // Mendefinisikan metode `ComputeMissionPerformanceScore` dengan hasil bertipe `double`. Menghitung skor performa misi berdasarkan penalti misi dan
    // penalti pinjaman. Masukan: Parameter `missionPenaltyTotal` bertipe `double` membawa nilai misi penalti total; Parameter `loanPenaltyTotal`
    // bertipe `double` membawa nilai pinjaman penalti total.
    public double ComputeMissionPerformanceScore(double missionPenaltyTotal, double loanPenaltyTotal)
    // Membuka scope metode ComputeMissionPerformanceScore; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // ComputeMissionPerformanceScore.
    {
        // Menyiapkan variabel lokal `missionPenaltyComponent` untuk nilai misi penalti komponen dengan membatasi `missionPenaltyTotal + loanPenaltyTotal`
        // agar tidak lebih kecil dari `0` dan tidak lebih besar dari `100`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var missionPenaltyComponent = AnalyticsMath.Clamp(missionPenaltyTotal + loanPenaltyTotal, 0, 100);
        // Mengembalikan membatasi `100 - missionPenaltyComponent` agar tidak lebih kecil dari `0` dan tidak lebih besar dari `100` kepada pemanggil dalam
        // ComputeMissionPerformanceScore; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return AnalyticsMath.Clamp(100 - missionPenaltyComponent, 0, 100);
    // Menutup scope metode ComputeMissionPerformanceScore; bagian berikut berada di luar batas blok tersebut dalam ComputeMissionPerformanceScore.
    }

    /// <summary>
    /// Menghitung rata-rata dari nilai-nilai nullable, mengabaikan null.
    /// </summary>
    // Mendefinisikan metode `AverageNullable` dengan hasil bertipe `double?`. Menghitung rata-rata dari nilai-nilai nullable, mengabaikan null.
    // Masukan: Parameter `values` bertipe `IEnumerable<double?>` membawa nilai nilai.
    public double? AverageNullable(IEnumerable<double?> values)
    // Membuka scope metode AverageNullable; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam AverageNullable.
    {
        // Menyiapkan variabel lokal `nonNull` untuk nilai non null dengan mematerialisasi urutan `values.Where(value => value.HasValue).Select(value =>
        // value!.Value)` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var nonNull = values.Where(value => value.HasValue).Select(value => value!.Value).ToList();
        // Memeriksa perbandingan kesamaan antara `nonNull.Count` dan `0`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam AverageNullable.
        if (nonNull.Count == 0)
        // Membuka scope cabang if untuk kondisi `nonNull.Count == 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam AverageNullable.
        {
            // Mengembalikan null, yaitu penanda tidak ada nilai kepada pemanggil dalam AverageNullable; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return null;
        // Menutup scope cabang if untuk kondisi `nonNull.Count == 0`; bagian berikut berada di luar batas blok tersebut dalam AverageNullable.
        }

        // Mengembalikan membulatkan `nonNull.Average()`, `2` sesuai jumlah digit atau aturan pembulatan yang diberikan kepada pemanggil dalam
        // AverageNullable; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return Math.Round(nonNull.Average(), 2);
    // Menutup scope metode AverageNullable; bagian berikut berada di luar batas blok tersebut dalam AverageNullable.
    }

    /// <summary>
    /// Menghitung rata-rata tertimbang dari komponen yang memiliki nilai, mengabaikan komponen null.
    /// </summary>
    // Mendefinisikan metode `WeightedAverage` dengan hasil bertipe `double?`. Menghitung rata-rata tertimbang dari komponen yang memiliki nilai,
    // mengabaikan komponen null. Masukan: Parameter `components` bertipe `IEnumerable<(double? value, double weight)>` membawa nilai komponen.
    private double? WeightedAverage(IEnumerable<(double? value, double weight)> components)
    // Membuka scope metode WeightedAverage; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam WeightedAverage.
    {
        // Menyiapkan variabel lokal `active` untuk nilai aktif dengan mematerialisasi urutan `components.Where(item => item.value.HasValue)` menjadi List;
        // enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var active = components.Where(item => item.value.HasValue).ToList();
        // Memeriksa perbandingan kesamaan antara `active.Count` dan `0`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam WeightedAverage.
        if (active.Count == 0)
        // Membuka scope cabang if untuk kondisi `active.Count == 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam WeightedAverage.
        {
            // Mengembalikan null, yaitu penanda tidak ada nilai kepada pemanggil dalam WeightedAverage; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return null;
        // Menutup scope cabang if untuk kondisi `active.Count == 0`; bagian berikut berada di luar batas blok tersebut dalam WeightedAverage.
        }

        // Menyiapkan variabel lokal `weightTotal` untuk nilai bobot total dengan menjumlahkan nilai `active` berdasarkan `item => item.weight`. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var weightTotal = active.Sum(item => item.weight);
        // Memeriksa pemeriksaan lebih kecil atau sama antara `weightTotal` dan `0`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // WeightedAverage.
        if (weightTotal <= 0)
        // Membuka scope cabang if untuk kondisi `weightTotal <= 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam WeightedAverage.
        {
            // Mengembalikan null, yaitu penanda tidak ada nilai kepada pemanggil dalam WeightedAverage; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return null;
        // Menutup scope cabang if untuk kondisi `weightTotal <= 0`; bagian berikut berada di luar batas blok tersebut dalam WeightedAverage.
        }

        // Menyiapkan variabel lokal `weightedScore` untuk nilai tertimbang skor dengan pembagian antara `active.Sum(item => item.value!.Value *
        // item.weight)` dan `weightTotal`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var weightedScore = active.Sum(item => item.value!.Value * item.weight) / weightTotal;
        // Mengembalikan membulatkan `weightedScore`, `2` sesuai jumlah digit atau aturan pembulatan yang diberikan kepada pemanggil dalam WeightedAverage;
        // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return Math.Round(weightedScore, 2);
    // Menutup scope metode WeightedAverage; bagian berikut berada di luar batas blok tersebut dalam WeightedAverage.
    }
// Menutup scope tipe ScoreCalculator; bagian berikut berada di luar batas blok tersebut.
}
