// Fungsi file: Menguji builder snapshot metrik analitik dari dictionary metric.
using Cashflowpoly.Api.Domain;
using Xunit;

namespace Cashflowpoly.Api.Tests;

public sealed class AnalyticsMetricSnapshotBuilderTests
{
    [Fact]
    public void BuildMetricSnapshots_MapsMetricValuesToSnapshotRows()
    {
        var sessionId = Guid.NewGuid();
        var playerId = Guid.NewGuid();
        var rulesetVersionId = Guid.NewGuid();
        var computedAt = DateTimeOffset.Parse("2026-05-04T01:02:03Z");
        var metrics = new Dictionary<string, (double? Numeric, string? Json)>
        {
            ["cashflow.net.total"] = (7, null),
            ["gameplay.raw.variables"] = (null, """{"coins":20}""")
        };

        var snapshots = new MetricSnapshotBuilder().BuildMetricSnapshots(
            sessionId,
            playerId,
            rulesetVersionId,
            computedAt,
            metrics);

        Assert.Equal(2, snapshots.Count);
        Assert.All(snapshots, snapshot =>
        {
            Assert.NotEqual(Guid.Empty, snapshot.MetricSnapshotId);
            Assert.Equal(sessionId, snapshot.SessionId);
            Assert.Equal(playerId, snapshot.PlayerId);
            Assert.Equal(rulesetVersionId, snapshot.RulesetVersionId);
            Assert.Equal(computedAt, snapshot.ComputedAt);
        });

        Assert.Contains(snapshots, snapshot =>
            snapshot.MetricName == "cashflow.net.total" &&
            snapshot.MetricValueNumeric == 7 &&
            snapshot.MetricValueJson is null);
        Assert.Contains(snapshots, snapshot =>
            snapshot.MetricName == "gameplay.raw.variables" &&
            snapshot.MetricValueNumeric is null &&
            snapshot.MetricValueJson == """{"coins":20}""");
    }
}
