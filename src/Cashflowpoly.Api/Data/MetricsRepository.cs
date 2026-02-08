using Dapper;
using Npgsql;

namespace Cashflowpoly.Api.Data;

/// <summary>
/// Repository untuk penyimpanan snapshot metrik.
/// </summary>
public sealed class MetricsRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public MetricsRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task InsertSnapshotsAsync(IEnumerable<MetricSnapshotDb> snapshots, CancellationToken ct)
    {
        const string sql = """
            insert into metric_snapshots (
                metric_snapshot_id,
                session_id,
                player_id,
                computed_at,
                metric_name,
                metric_value_numeric,
                metric_value_json,
                ruleset_version_id
            )
            values (
                @MetricSnapshotId,
                @SessionId,
                @PlayerId,
                @ComputedAt,
                @MetricName,
                @MetricValueNumeric,
                @MetricValueJson::jsonb,
                @RulesetVersionId
            )
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        await conn.ExecuteAsync(new CommandDefinition(sql, snapshots, cancellationToken: ct));
    }

    public async Task<int> CountValidationViolationsAsync(Guid sessionId, Guid? playerId, CancellationToken ct)
    {
        var sql = """
            select count(*)
            from validation_logs
            where session_id = @sessionId
              and is_valid = false
            """;

        if (playerId.HasValue)
        {
            sql += " and details_json ->> 'player_id' = @playerId";
        }

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        return await conn.ExecuteScalarAsync<int>(new CommandDefinition(sql, new { sessionId, playerId = playerId?.ToString() }, cancellationToken: ct));
    }

    public async Task<List<MetricSnapshotJsonDb>> GetLatestGameplaySnapshotsAsync(Guid sessionId, Guid playerId, CancellationToken ct)
    {
        const string sql = """
            select distinct on (metric_name)
                   metric_name,
                   metric_value_json::text as metric_value_json,
                   computed_at
            from metric_snapshots
            where session_id = @sessionId
              and player_id = @playerId
              and metric_name = any(@metricNames)
            order by metric_name, computed_at desc
            """;

        var metricNames = new[] { "gameplay.raw.variables", "gameplay.derived.metrics" };
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        var items = await conn.QueryAsync<MetricSnapshotJsonDb>(
            new CommandDefinition(sql, new { sessionId, playerId, metricNames }, cancellationToken: ct));
        return items.ToList();
    }

    public async Task<double?> GetLatestMetricNumericAsync(Guid sessionId, Guid playerId, string metricName, CancellationToken ct)
    {
        const string sql = """
            select metric_value_numeric
            from metric_snapshots
            where session_id = @sessionId
              and player_id = @playerId
              and metric_name = @metricName
              and metric_value_numeric is not null
            order by computed_at desc
            limit 1
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        return await conn.ExecuteScalarAsync<double?>(
            new CommandDefinition(sql, new { sessionId, playerId, metricName }, cancellationToken: ct));
    }
}
