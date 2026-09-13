// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui AnalyticsSessionMetricCalculator.
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Domain;

/// <summary>
/// Kalkulator murni untuk metric snapshot level sesi.
/// </summary>
// Mendefinisikan tipe class `SessionMetricCalculator` yang mewarisi atau menerapkan `ISessionMetricCalculator`; sealed mencegah tipe ini diturunkan
// lagi.
internal sealed class SessionMetricCalculator : ISessionMetricCalculator
{
    private static readonly AnalyticsPayloadReader _payloadReader = new();
    /// <summary>
    /// Menghitung metrik agregat level sesi: cashflow total, donasi, dan happiness.
    /// </summary>
    // Mendefinisikan metode `ComputeSessionMetrics` dengan hasil bertipe `Dictionary<string, (double? Numeric, string? Json)>`. Menghitung metrik
    // agregat level sesi: cashflow total, donasi, dan happiness. Masukan: Parameter `events` bertipe `List<EventDb>` membawa kumpulan event permainan
    // sebagai sumber riwayat untuk validasi atau perhitungan; Parameter `projections` bertipe `List<CashflowProjectionDb>` membawa proyeksi transaksi
    // arus kas yang diturunkan dari event permainan; Parameter `happinessByPlayer` bertipe `Dictionary<Guid, AnalyticsHappinessBreakdown>` membawa
    // nilai kebahagiaan berdasarkan pemain.
    public Dictionary<string, (double? Numeric, string? Json)> ComputeSessionMetrics(
        // Parameter `events` bertipe `List<EventDb>` membawa kumpulan event permainan sebagai sumber riwayat untuk validasi atau perhitungan.
        List<EventDb> events,
        // Parameter `projections` bertipe `List<CashflowProjectionDb>` membawa proyeksi transaksi arus kas yang diturunkan dari event permainan.
        List<CashflowProjectionDb> projections,
        // Parameter `happinessByPlayer` bertipe `Dictionary<Guid, AnalyticsHappinessBreakdown>` membawa nilai kebahagiaan berdasarkan pemain.
        Dictionary<Guid, AnalyticsHappinessBreakdown> happinessByPlayer)
    {
        var metrics = new Dictionary<string, (double? Numeric, string? Json)>();

        var cashIn = projections.Where(p => p.Direction == "IN").Sum(p => (double)p.Amount);
        var cashOut = projections.Where(p => p.Direction == "OUT").Sum(p => (double)p.Amount);
        metrics["cashflow.in.total"] = (cashIn, null);
        metrics["cashflow.out.total"] = (cashOut, null);
        metrics["cashflow.net.total"] = (cashIn - cashOut, null);

        var donationTotal = events.Where(e => e.ActionType == "JumatBerkah")
            .Select(e => _payloadReader.TryReadAmount(e.Payload, out var amount) ? amount : 0)
            .Sum();
        metrics["donation.total"] = (donationTotal, null);

        var happinessTotal = happinessByPlayer.Values.Sum(item => item.Total);
        metrics["happiness.points.total"] = (happinessTotal, null);

        return metrics;
    }
}
