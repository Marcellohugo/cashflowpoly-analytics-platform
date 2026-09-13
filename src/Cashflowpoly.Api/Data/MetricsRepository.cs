// Fungsi file: Mengelola pemetaan dan akses PostgreSQL untuk MetricsRepository.
// Mengimpor namespace `Dapper` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Dapper;
// Mengimpor namespace `Npgsql` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Npgsql;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Data` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Data;

/// <summary>
/// Repository untuk penyimpanan snapshot metrik.
/// </summary>
// Mendefinisikan tipe class `MetricsRepository`; sealed mencegah tipe ini diturunkan lagi.
public sealed class MetricsRepository
{
    private readonly NpgsqlDataSource _dataSource;

    /// <summary>
    /// Menerima NpgsqlDataSource untuk koneksi ke tabel metric_snapshots.
    /// </summary>
    // Mendefinisikan konstruktor MetricsRepository yang menyiapkan objek dan menerima dependency/nilai awal dari pemanggil; parameter: Parameter
    // `dataSource` bertipe `NpgsqlDataSource` membawa sumber koneksi PostgreSQL yang mengelola pembuatan dan penggunaan ulang koneksi.
    public MetricsRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    /// <summary>
    /// Menyisipkan batch snapshot metrik ke database.
    /// </summary>
    // Mendefinisikan metode `InsertSnapshotsAsync` dengan hasil bertipe `Task`. Menyisipkan batch snapshot metrik ke database. async memungkinkan
    // metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `snapshots` bertipe
    // `IEnumerable<MetricSnapshotDb>` membawa nilai snapshots; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat
    // dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
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
    /// Mengambil snapshot gameplay JSON terbaru (variabel mentah dan metrik turunan) per pemain.
    /// </summary>
    // Mendefinisikan metode `GetLatestGameplaySnapshotsAsync` dengan hasil bertipe `Task<List<MetricSnapshotJsonDb>>`. Mengambil snapshot gameplay JSON
    // terbaru (variabel mentah dan metrik turunan) per pemain. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan
    // penyelesaian melalui Task. Masukan: Parameter `sessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi
    // ini; Parameter `userId` bertipe `Guid` membawa identitas akun pengguna yang datanya sedang diproses; Parameter `ct` bertipe `CancellationToken`
    // membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
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
    // Mendefinisikan metode `GetLatestMetricNumericAsync` dengan hasil bertipe `Task<double?>`. Mengambil nilai numerik terbaru dari metrik tertentu
    // per pemain. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter
    // `sessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini; Parameter `userId` bertipe `Guid` membawa
    // identitas akun pengguna yang datanya sedang diproses; Parameter `metricName` bertipe `string` membawa nilai metric nama; Parameter `ct` bertipe
    // `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
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
