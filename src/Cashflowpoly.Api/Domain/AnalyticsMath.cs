// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui AnalyticsMath.
// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Domain;

/// <summary>
/// Helper matematika yang dipakai perhitungan metrik analitik.
/// </summary>
// Mendefinisikan tipe class `AnalyticsMath`.
internal static class AnalyticsMath
{
    /// <summary>
    /// Membatasi nilai numerik dalam rentang minimum dan maksimum.
    /// </summary>
    // Mendefinisikan metode `Clamp` dengan hasil bertipe `double`. Membatasi nilai numerik dalam rentang minimum dan maksimum. Masukan: Parameter
    // `value` bertipe `double` membawa nilai nilai; Parameter `min` bertipe `double` membawa nilai minimum; Parameter `max` bertipe `double` membawa
    // nilai maksimum.
    internal static double Clamp(double value, double min, double max)
    {
        return Math.Min(max, Math.Max(min, value));
    }

    /// <summary>
    /// Menghitung rasio aman dengan perlindungan pembagian nol; opsional dikalikan 100 untuk persen.
    /// </summary>
    // Mendefinisikan metode `SafeRatio` dengan hasil bertipe `double?`. Menghitung rasio aman dengan perlindungan pembagian nol; opsional dikalikan 100
    // untuk persen. Masukan: Parameter `numerator` bertipe `double` membawa nilai numerator; Parameter `denominator` bertipe `double` membawa nilai
    // denominator; Parameter `percent` bertipe `bool` membawa nilai percent; bila argumen tidak diberikan digunakan false, yaitu kondisi nonaktif/tidak
    // terpenuhi.
    internal static double? SafeRatio(double numerator, double denominator, bool percent = false)
    {
        if (Math.Abs(denominator) < 0.000001)
        {
            return null;
        }

        var value = numerator / denominator;
        return percent ? value * 100 : value;
    }

    /// <summary>
    /// Menghitung standar deviasi populasi dari kumpulan nilai numerik.
    /// </summary>
    // Mendefinisikan metode `StdDev` dengan hasil bertipe `double`. Menghitung standar deviasi populasi dari kumpulan nilai numerik. Masukan: Parameter
    // `values` bertipe `IReadOnlyList<double>` membawa nilai nilai.
    internal static double StdDev(IReadOnlyList<double> values)
    {
        if (values.Count == 0)
        {
            return 0;
        }

        var mean = values.Average();
        var variance = values.Sum(v => Math.Pow(v - mean, 2)) / values.Count;
        return Math.Sqrt(variance);
    }
}
