// Fungsi file: Membangun record snapshot metrik analitik dari hasil kalkulasi.
using Cashflowpoly.Api.Data;

namespace Cashflowpoly.Api.Domain;

/// <summary>
/// Builder snapshot metrik yang siap disimpan ke tabel metric_snapshots.
/// </summary>
internal sealed class MetricSnapshotBuilder : IMetricSnapshotBuilder
{
    /// <summary>
    /// Mengonversi dictionary metrik menjadi daftar record MetricSnapshotDb siap disimpan.
    /// </summary>
    public List<MetricSnapshotDb> BuildMetricSnapshots(
        Guid sessionId,
        Guid? playerId,
        Guid rulesetVersionId,
        DateTimeOffset computedAt,
        Dictionary<string, (double? Numeric, string? Json)> metrics)
    {
        var list = new List<MetricSnapshotDb>();
        foreach (var item in metrics)
        {
            list.Add(new MetricSnapshotDb
            {
                MetricSnapshotId = Guid.NewGuid(),
                SessionId = sessionId,
                PlayerId = playerId,
                ComputedAt = computedAt,
                MetricName = item.Key,
                MetricValueNumeric = item.Value.Numeric,
                MetricValueJson = item.Value.Json,
                RulesetVersionId = rulesetVersionId
            });
        }

        return list;
    }
}
