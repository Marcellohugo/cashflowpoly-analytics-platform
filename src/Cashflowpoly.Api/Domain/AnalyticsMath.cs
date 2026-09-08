// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui AnalyticsMath.
// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Domain;

/// <summary>
/// Helper matematika yang dipakai perhitungan metrik analitik.
/// </summary>
// Mendefinisikan tipe class `AnalyticsMath`.
internal static class AnalyticsMath
// Membuka scope tipe AnalyticsMath; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    /// <summary>
    /// Membatasi nilai numerik dalam rentang minimum dan maksimum.
    /// </summary>
    // Mendefinisikan metode `Clamp` dengan hasil bertipe `double`. Membatasi nilai numerik dalam rentang minimum dan maksimum. Masukan: Parameter
    // `value` bertipe `double` membawa nilai nilai; Parameter `min` bertipe `double` membawa nilai minimum; Parameter `max` bertipe `double` membawa
    // nilai maksimum.
    internal static double Clamp(double value, double min, double max)
    // Membuka scope metode Clamp; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Clamp.
    {
        // Mengembalikan menentukan nilai terkecil dari `max`, `Math.Max(min, value)` kepada pemanggil dalam Clamp; eksekusi jalur ini selesai setelah nilai
        // hasil ditentukan.
        return Math.Min(max, Math.Max(min, value));
    // Menutup scope metode Clamp; bagian berikut berada di luar batas blok tersebut dalam Clamp.
    }

    /// <summary>
    /// Menghitung rasio aman dengan perlindungan pembagian nol; opsional dikalikan 100 untuk persen.
    /// </summary>
    // Mendefinisikan metode `SafeRatio` dengan hasil bertipe `double?`. Menghitung rasio aman dengan perlindungan pembagian nol; opsional dikalikan 100
    // untuk persen. Masukan: Parameter `numerator` bertipe `double` membawa nilai numerator; Parameter `denominator` bertipe `double` membawa nilai
    // denominator; Parameter `percent` bertipe `bool` membawa nilai percent; bila argumen tidak diberikan digunakan false, yaitu kondisi nonaktif/tidak
    // terpenuhi.
    internal static double? SafeRatio(double numerator, double denominator, bool percent = false)
    // Membuka scope metode SafeRatio; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam SafeRatio.
    {
        // Memeriksa pemeriksaan lebih kecil antara `Math.Abs(denominator)` dan `0.000001`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // SafeRatio.
        if (Math.Abs(denominator) < 0.000001)
        // Membuka scope cabang if untuk kondisi `Math.Abs(denominator) < 0.000001`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // SafeRatio.
        {
            // Mengembalikan null, yaitu penanda tidak ada nilai kepada pemanggil dalam SafeRatio; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return null;
        // Menutup scope cabang if untuk kondisi `Math.Abs(denominator) < 0.000001`; bagian berikut berada di luar batas blok tersebut dalam SafeRatio.
        }

        // Menyiapkan variabel lokal `value` untuk nilai nilai dengan pembagian antara `numerator` dan `denominator`. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var value = numerator / denominator;
        // Mengembalikan hasil pemilihan bersyarat: ketika `percent` benar gunakan `value * 100`, jika tidak gunakan `value` kepada pemanggil dalam
        // SafeRatio; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return percent ? value * 100 : value;
    // Menutup scope metode SafeRatio; bagian berikut berada di luar batas blok tersebut dalam SafeRatio.
    }

    /// <summary>
    /// Menghitung standar deviasi populasi dari kumpulan nilai numerik.
    /// </summary>
    // Mendefinisikan metode `StdDev` dengan hasil bertipe `double`. Menghitung standar deviasi populasi dari kumpulan nilai numerik. Masukan: Parameter
    // `values` bertipe `IReadOnlyList<double>` membawa nilai nilai.
    internal static double StdDev(IReadOnlyList<double> values)
    // Membuka scope metode StdDev; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam StdDev.
    {
        // Memeriksa perbandingan kesamaan antara `values.Count` dan `0`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam StdDev.
        if (values.Count == 0)
        // Membuka scope cabang if untuk kondisi `values.Count == 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam StdDev.
        {
            // Mengembalikan nilai literal `0` kepada pemanggil dalam StdDev; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return 0;
        // Menutup scope cabang if untuk kondisi `values.Count == 0`; bagian berikut berada di luar batas blok tersebut dalam StdDev.
        }

        // Menyiapkan variabel lokal `mean` untuk nilai mean dengan menghitung rata-rata nilai `values`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var mean = values.Average();
        // Menyiapkan variabel lokal `variance` untuk nilai variance dengan pembagian antara `values.Sum(v => Math.Pow(v - mean, 2))` dan `values.Count`.
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var variance = values.Sum(v => Math.Pow(v - mean, 2)) / values.Count;
        // Mengembalikan memanggil `Math.Sqrt` dengan `variance` kepada pemanggil dalam StdDev; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return Math.Sqrt(variance);
    // Menutup scope metode StdDev; bagian berikut berada di luar batas blok tersebut dalam StdDev.
    }
// Menutup scope tipe AnalyticsMath; bagian berikut berada di luar batas blok tersebut.
}
