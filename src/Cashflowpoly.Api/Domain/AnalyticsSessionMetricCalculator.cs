// Fungsi file: Menghitung metrik agregat level sesi untuk snapshot analitik.
using Cashflowpoly.Api.Data;
using static Cashflowpoly.Api.Domain.AnalyticsPayloadReader;

namespace Cashflowpoly.Api.Domain;

/// <summary>
/// Kalkulator murni untuk metric snapshot level sesi.
/// </summary>
internal static class AnalyticsSessionMetricCalculator
{
    /// <summary>
    /// Menghitung metrik agregat level sesi: cashflow total, donasi, dan happiness.
    /// </summary>
    internal static Dictionary<string, (double? Numeric, string? Json)> ComputeSessionMetrics(
        List<EventDb> events,
        List<CashflowProjectionDb> projections,
        Dictionary<Guid, AnalyticsHappinessBreakdown> happinessByPlayer)
    {
        var metrics = new Dictionary<string, (double? Numeric, string? Json)>();

        var cashIn = projections.Where(p => p.Direction == "IN").Sum(p => (double)p.Amount);
        var cashOut = projections.Where(p => p.Direction == "OUT").Sum(p => (double)p.Amount);
        metrics["cashflow.in.total"] = (cashIn, null);
        metrics["cashflow.out.total"] = (cashOut, null);
        metrics["cashflow.net.total"] = (cashIn - cashOut, null);

        var donationTotal = events.Where(e => e.ActionType == "day.friday.donation")
            .Select(e => TryReadAmount(e.Payload, out var amount) ? amount : 0)
            .Sum();
        metrics["donation.total"] = (donationTotal, null);

        var happinessTotal = happinessByPlayer.Values.Sum(item => item.Total);
        metrics["happiness.points.total"] = (happinessTotal, null);

        return metrics;
    }
}
