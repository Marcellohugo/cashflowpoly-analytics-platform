namespace Cashflowpoly.Api.Domain;

/// <summary>
/// Helper matematika yang dipakai perhitungan metrik analitik.
/// </summary>
internal static class AnalyticsMath
{
    /// <summary>
    /// Membatasi nilai numerik dalam rentang minimum dan maksimum.
    /// </summary>
    internal static double Clamp(double value, double min, double max)
    {
        return Math.Min(max, Math.Max(min, value));
    }

    /// <summary>
    /// Menghitung rasio aman dengan perlindungan pembagian nol; opsional dikalikan 100 untuk persen.
    /// </summary>
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
