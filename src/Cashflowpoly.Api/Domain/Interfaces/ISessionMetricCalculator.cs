using Cashflowpoly.Api.Data;

namespace Cashflowpoly.Api.Domain;

internal interface ISessionMetricCalculator
{
    Dictionary<string, (double? Numeric, string? Json)> ComputeSessionMetrics(
        List<EventDb> events,
        List<CashflowProjectionDb> projections,
        Dictionary<Guid, AnalyticsHappinessBreakdown> happinessByPlayer);
}
