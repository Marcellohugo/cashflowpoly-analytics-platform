using Cashflowpoly.Api.Data;

namespace Cashflowpoly.Api.Domain;

internal interface IMetricSnapshotBuilder
{
    List<MetricSnapshotDb> BuildMetricSnapshots(
        Guid sessionId,
        Guid? playerId,
        Guid rulesetVersionId,
        DateTimeOffset computedAt,
        Dictionary<string, (double? Numeric, string? Json)> metrics);
}
