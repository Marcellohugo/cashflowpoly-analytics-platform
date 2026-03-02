// Fungsi file: Repository akses data event gameplay — penyimpanan event, sequence number, log validasi, dan proyeksi cashflow.
using Dapper;
using Npgsql;

namespace Cashflowpoly.Api.Data;

/// <summary>
/// Repository untuk event, log validasi, dan proyeksi arus kas.
/// </summary>
public sealed class EventRepository
{
    private readonly NpgsqlDataSource _dataSource;

    /// <summary>
    /// Menerima NpgsqlDataSource untuk koneksi ke tabel events, validation_logs, dan cashflow_projections.
    /// </summary>
    public EventRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    /// <summary>
    /// Memeriksa apakah event_id sudah ada di database (untuk idempotency).
    /// </summary>
    public async Task<bool> EventIdExistsAsync(Guid sessionId, Guid eventId, CancellationToken ct)
    {
        const string sql = """
            select 1
            from events
            where session_id = @sessionId and event_id = @eventId
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        var result = await conn.ExecuteScalarAsync<int?>(new CommandDefinition(sql, new { sessionId, eventId }, cancellationToken: ct));
        return result.HasValue;
    }

    /// <summary>
    /// Memeriksa apakah sequence_number sudah digunakan dalam satu sesi.
    /// </summary>
    public async Task<bool> SequenceNumberExistsAsync(Guid sessionId, long sequenceNumber, CancellationToken ct)
    {
        const string sql = """
            select 1
            from events
            where session_id = @sessionId and sequence_number = @sequenceNumber
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        var result = await conn.ExecuteScalarAsync<int?>(new CommandDefinition(sql, new { sessionId, sequenceNumber }, cancellationToken: ct));
        return result.HasValue;
    }

    /// <summary>
    /// Mengambil sequence_number tertinggi pada satu sesi untuk penentuan urutan event berikutnya.
    /// </summary>
    public async Task<long?> GetMaxSequenceNumberAsync(Guid sessionId, CancellationToken ct)
    {
        const string sql = """
            select max(sequence_number)
            from events
            where session_id = @sessionId
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        return await conn.ExecuteScalarAsync<long?>(new CommandDefinition(sql, new { sessionId }, cancellationToken: ct));
    }

    /// <summary>
    /// SQL INSERT untuk tabel events, digunakan oleh kedua overload InsertEventAsync.
    /// </summary>
    private const string InsertEventSql = """
        insert into events (
            event_pk,
            event_id,
            session_id,
            player_id,
            actor_type,
            timestamp,
            day_index,
            weekday,
            turn_number,
            sequence_number,
            action_type,
            ruleset_version_id,
            payload,
            received_at,
            client_request_id
        )
        values (
            @EventPk,
            @EventId,
            @SessionId,
            @PlayerId,
            @ActorType,
            @Timestamp,
            @DayIndex,
            @Weekday,
            @TurnNumber,
            @SequenceNumber,
            @ActionType,
            @RulesetVersionId,
            @Payload::jsonb,
            @ReceivedAt,
            @ClientRequestId
        )
        """;

    public async Task InsertEventAsync(EventDb record, CancellationToken ct)
    {
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        await conn.ExecuteAsync(new CommandDefinition(InsertEventSql, record, cancellationToken: ct));
    }

    /// <summary>
    /// Menyisipkan event menggunakan koneksi dan transaksi yang sudah ada (untuk atomisitas dengan proyeksi).
    /// </summary>
    internal async Task InsertEventAsync(EventDb record, NpgsqlConnection conn, NpgsqlTransaction tx, CancellationToken ct)
    {
        await conn.ExecuteAsync(new CommandDefinition(InsertEventSql, record, tx, cancellationToken: ct));
    }

    /// <summary>
    /// Menyimpan catatan log validasi (valid/gagal) untuk satu event ke tabel validation_logs.
    /// </summary>
    public async Task InsertValidationLogAsync(
        Guid sessionId,
        Guid eventId,
        Guid? eventPk,
        bool isValid,
        string? errorCode,
        string? errorMessage,
        string? detailsJson,
        CancellationToken ct)
    {
        const string sql = """
            insert into validation_logs (
                validation_log_id,
                session_id,
                event_pk,
                event_id,
                is_valid,
                error_code,
                error_message,
                details_json,
                created_at
            )
            values (
                @validationLogId,
                @sessionId,
                @eventPk,
                @eventId,
                @isValid,
                @errorCode,
                @errorMessage,
                @detailsJson::jsonb,
                @createdAt
            )
            on conflict (session_id, event_id) do nothing
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        await conn.ExecuteAsync(new CommandDefinition(sql, new
        {
            validationLogId = Guid.NewGuid(),
            sessionId,
            eventPk,
            eventId,
            isValid,
            errorCode,
            errorMessage,
            detailsJson,
            createdAt = DateTimeOffset.UtcNow
        }, cancellationToken: ct));
    }

    /// <summary>
    /// SQL INSERT untuk tabel event_cashflow_projections, digunakan oleh kedua overload.
    /// </summary>
    private const string InsertProjectionSql = """
        insert into event_cashflow_projections (
            projection_id,
            session_id,
            player_id,
            event_pk,
            event_id,
            timestamp,
            direction,
            amount,
            category,
            counterparty,
            reference,
            note
        )
        values (
            @ProjectionId,
            @SessionId,
            @PlayerId,
            @EventPk,
            @EventId,
            @Timestamp,
            @Direction,
            @Amount,
            @Category,
            @Counterparty,
            @Reference,
            @Note
        )
        on conflict (session_id, event_id) do nothing
        """;

    /// <summary>
    /// Menyimpan proyeksi arus kas (cashflow projection) yang dihasilkan dari satu event.
    /// </summary>
    public async Task InsertCashflowProjectionAsync(CashflowProjectionDb projection, CancellationToken ct)
    {
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        await conn.ExecuteAsync(new CommandDefinition(InsertProjectionSql, projection, cancellationToken: ct));
    }

    /// <summary>
    /// Menyisipkan proyeksi arus kas menggunakan koneksi dan transaksi yang sudah ada.
    /// </summary>
    internal async Task InsertCashflowProjectionAsync(CashflowProjectionDb projection, NpgsqlConnection conn, NpgsqlTransaction tx, CancellationToken ct)
    {
        await conn.ExecuteAsync(new CommandDefinition(InsertProjectionSql, projection, tx, cancellationToken: ct));
    }

    /// <summary>
    /// Membuka koneksi database untuk digunakan dengan transaksi eksternal.
    /// </summary>
    internal async Task<NpgsqlConnection> OpenConnectionAsync(CancellationToken ct)
    {
        return await _dataSource.OpenConnectionAsync(ct);
    }

    /// <summary>
    /// Mengambil seluruh proyeksi arus kas dalam satu sesi.
    /// </summary>
    public async Task<List<CashflowProjectionDb>> GetCashflowProjectionsAsync(Guid sessionId, CancellationToken ct)
    {
        const string sql = """
            select projection_id,
                   session_id,
                   player_id,
                   event_pk,
                   event_id,
                   timestamp,
                   direction,
                   amount,
                   category,
                   counterparty,
                   reference,
                   note
            from event_cashflow_projections
            where session_id = @sessionId
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        var items = await conn.QueryAsync<CashflowProjectionDb>(new CommandDefinition(sql, new { sessionId }, cancellationToken: ct));
        return items.ToList();
    }

    /// <summary>
    /// Mengambil daftar event pada satu sesi mulai dari fromSeq dengan batas jumlah tertentu.
    /// </summary>
    public async Task<List<EventDb>> GetEventsBySessionAsync(Guid sessionId, long fromSeq, int limit, CancellationToken ct)
    {
        const string sql = """
            select event_pk,
                   event_id,
                   session_id,
                   player_id,
                   actor_type,
                   timestamp,
                   day_index,
                   weekday,
                   turn_number,
                   sequence_number,
                   action_type,
                   ruleset_version_id,
                   payload::text as payload,
                   received_at,
                   client_request_id
            from events
            where session_id = @sessionId
              and sequence_number >= @fromSeq
            order by sequence_number
            limit @limit
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        var items = await conn.QueryAsync<EventDb>(new CommandDefinition(sql, new { sessionId, fromSeq, limit }, cancellationToken: ct));
        return items.ToList();
    }

    /// <summary>
    /// Mengambil seluruh event pada satu sesi, diurutkan berdasarkan sequence_number.
    /// </summary>
    public async Task<List<EventDb>> GetAllEventsBySessionAsync(Guid sessionId, CancellationToken ct)
    {
        const string sql = """
            select event_pk,
                   event_id,
                   session_id,
                   player_id,
                   actor_type,
                   timestamp,
                   day_index,
                   weekday,
                   turn_number,
                   sequence_number,
                   action_type,
                   ruleset_version_id,
                   payload::text as payload,
                   received_at,
                   client_request_id
            from events
            where session_id = @sessionId
            order by sequence_number
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        var items = await conn.QueryAsync<EventDb>(new CommandDefinition(sql, new { sessionId }, cancellationToken: ct));
        return items.ToList();
    }

    /// <summary>
    /// Mengambil satu event berdasarkan session_id dan event_id.
    /// </summary>
    public async Task<EventDb?> GetEventByIdAsync(Guid sessionId, Guid eventId, CancellationToken ct)
    {
        const string sql = """
            select event_pk,
                   event_id,
                   session_id,
                   player_id,
                   actor_type,
                   timestamp,
                   day_index,
                   weekday,
                   turn_number,
                   sequence_number,
                   action_type,
                   ruleset_version_id,
                   payload::text as payload,
                   received_at,
                   client_request_id
            from events
            where session_id = @sessionId
              and event_id = @eventId
            limit 1
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        return await conn.QueryFirstOrDefaultAsync<EventDb>(new CommandDefinition(sql, new { sessionId, eventId }, cancellationToken: ct));
    }
}
