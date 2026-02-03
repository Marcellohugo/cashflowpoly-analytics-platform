using Dapper;
using Npgsql;

namespace Cashflowpoly.Api.Data;

/// <summary>
/// Repository untuk event, log validasi, dan proyeksi arus kas.
/// </summary>
public sealed class EventRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public EventRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

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

    public async Task InsertEventAsync(EventDb record, CancellationToken ct)
    {
        const string sql = """
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

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        await conn.ExecuteAsync(new CommandDefinition(sql, record, cancellationToken: ct));
    }

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

    public async Task InsertCashflowProjectionAsync(CashflowProjectionDb projection, CancellationToken ct)
    {
        const string sql = """
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

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        await conn.ExecuteAsync(new CommandDefinition(sql, projection, cancellationToken: ct));
    }

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
}
