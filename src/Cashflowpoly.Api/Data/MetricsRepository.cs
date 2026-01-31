using Dapper;
using Npgsql;

namespace Cashflowpoly.Api.Data;

internal sealed class MetricsRepository
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
}
