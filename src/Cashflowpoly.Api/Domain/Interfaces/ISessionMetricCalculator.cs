// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui ISessionMetricCalculator.
using Cashflowpoly.Api.Data;

namespace Cashflowpoly.Api.Domain;

public interface ISessionMetricCalculator
{
    Dictionary<string, (double? Numeric, string? Json)> ComputeSessionMetrics(
        List<EventDb> events,
        List<CashflowProjectionDb> projections,
        Dictionary<Guid, AnalyticsHappinessBreakdown> happinessByPlayer);
}
