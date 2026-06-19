using Dapper;
using Npgsql;

namespace Cashflowpoly.Api.Data;

/// <summary>
/// Repository untuk penyimpanan snapshot metrik.
/// </summary>
public sealed class MetricsRepository
{
    private readonly NpgsqlDataSource _dataSource;

    /// <summary>
    /// Menerima NpgsqlDataSource untuk koneksi ke tabel metric_snapshots.
    /// </summary>
    public MetricsRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    /// <summary>
    /// Menyisipkan batch snapshot metrik ke database.
    /// </summary>
    public async Task InsertSnapshotsAsync(IEnumerable<MetricSnapshotDb> snapshots, CancellationToken ct)
    {
        const string sql = """
            insert into metric_snapshots (
                metric_snapshot_id,
                session_id,
                user_id,
                session_player_id,
                computed_at,
                metric_name,
                metric_value_numeric,
                metric_payload_json,
                ruleset_version_id,
                last_event_id
            )
            values (
                @MetricSnapshotId,
                @SessionId,
                @UserId,
                @SessionPlayerId,
                @ComputedAt,
                @MetricName,
                @MetricValueNumeric,
                @MetricValueJson::jsonb,
                @RulesetVersionId,
                @LastEventId
            )
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        await conn.ExecuteAsync(new CommandDefinition(sql, snapshots, cancellationToken: ct));
    }

    /// <summary>
    /// Menghitung jumlah pelanggaran validasi pada sesi dengan filter opsional per pemain.
    /// </summary>
    public async Task<int> CountValidationViolationsAsync(Guid sessionId, Guid? userId, CancellationToken ct)
    {
        var sql = """
            select count(*)
            from validation_logs
            where session_id = @sessionId
            """;

        if (userId.HasValue)
        {
            sql += """

                 and details_json->>'user_id' = @userIdText
                 """;
        }

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        return await conn.ExecuteScalarAsync<int>(new CommandDefinition(sql, new { sessionId, userIdText = userId?.ToString() }, cancellationToken: ct));
    }

    /// <summary>
    /// Mengambil snapshot gameplay JSON terbaru (variabel mentah dan metrik turunan) per pemain.
    /// </summary>
    public async Task<List<MetricSnapshotJsonDb>> GetLatestGameplaySnapshotsAsync(Guid sessionId, Guid userId, CancellationToken ct)
    {
        const string sql = """
            select distinct on (metric_name)
                   metric_name,
                   metric_payload_json::text as metric_value_json,
                   computed_at
            from metric_snapshots
            where session_id = @sessionId
              and user_id = @userId
              and metric_name = any(@metricNames)
            order by metric_name, computed_at desc
            """;

        var metricNames = new[] { "gameplay.raw.variables", "gameplay.derived.metrics" };
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        var items = await conn.QueryAsync<MetricSnapshotJsonDb>(
            new CommandDefinition(sql, new { sessionId, userId, metricNames }, cancellationToken: ct));
        return items.ToList();
    }

    public async Task<List<MetricSnapshotValueDb>> GetLatestMetricValuesAsync(Guid sessionId, Guid userId, IReadOnlyCollection<string> metricNames, CancellationToken ct)
    {
        const string sql = """
            select distinct on (metric_name)
                   metric_name,
                   metric_value_numeric,
                   computed_at
            from metric_snapshots
            where session_id = @sessionId
              and user_id = @userId
              and metric_name = any(@metricNames)
            order by metric_name, computed_at desc
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        var items = await conn.QueryAsync<MetricSnapshotValueDb>(
            new CommandDefinition(sql, new { sessionId, userId, metricNames = metricNames.ToArray() }, cancellationToken: ct));
        return items.ToList();
    }

    /// <summary>
    /// Mengambil nilai numerik terbaru dari metrik tertentu per pemain.
    /// </summary>
    public async Task<double?> GetLatestMetricNumericAsync(Guid sessionId, Guid userId, string metricName, CancellationToken ct)
    {
        const string sql = """
            select metric_value_numeric
            from metric_snapshots
            where session_id = @sessionId
              and user_id = @userId
              and metric_name = @metricName
              and metric_value_numeric is not null
            order by computed_at desc
            limit 1
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        return await conn.ExecuteScalarAsync<double?>(
            new CommandDefinition(sql, new { sessionId, userId, metricName }, cancellationToken: ct));
    }
}
