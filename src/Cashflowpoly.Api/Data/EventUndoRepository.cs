// Fungsi file: Membatalkan event terakhir dengan memulihkan snapshot atomik dan mempertahankan audit asli.
using System.Text.Json;
using Cashflowpoly.Api.Contracts;
using Cashflowpoly.Api.Infrastructure;
using Dapper;
using Npgsql;

namespace Cashflowpoly.Api.Data;

public sealed class EventUndoRepository(NpgsqlDataSource dataSource)
{
    public sealed record Result(EventUndoResponse? Response, int StatusCode, string? ErrorCode = null, string? Message = null);

    public static Task CaptureBeforeEventAsync(Guid sessionId, Guid eventId, NpgsqlConnection conn, NpgsqlTransaction tx, CancellationToken ct) =>
        conn.ExecuteAsync(new CommandDefinition("""
            insert into event_undo_snapshots (session_id, event_id, before_state)
            values (@sessionId, @eventId, capture_session_undo_state(@sessionId))
            """, new { sessionId, eventId }, tx, cancellationToken: ct));

    public async Task<Result> UndoAsync(Guid sessionId, Guid eventId, Guid instructorUserId, UndoEventRequest request, CancellationToken ct)
    {
        await using var conn = await dataSource.OpenConnectionAsync(ct);
        await using var tx = await conn.BeginTransactionAsync(ct);
        await conn.ExecuteAsync(new CommandDefinition(
            "select pg_advisory_xact_lock(hashtextextended(@sessionId::text, 0))", new { sessionId }, tx, cancellationToken: ct));

        // A committed receipt can be retried even after timeout removes a now-empty session.
        var retry = await conn.QuerySingleOrDefaultAsync<UndoRow>(new CommandDefinition("""
            select undo_id, session_id, event_id, client_request_id, expected_state_version, reason, state_version, next_sequence_number
            from event_undos where instructor_user_id = @instructorUserId and client_request_id = @ClientRequestId
            """, new { instructorUserId, request.ClientRequestId }, tx, cancellationToken: ct));
        if (retry is not null)
            return retry.SessionId == sessionId && retry.EventId == eventId && retry.Reason == request.Reason &&
                retry.ExpectedStateVersion == request.ExpectedStateVersion
                ? new Result(ToResponse(retry), 200)
                : new Result(null, 409, "CLIENT_REQUEST_ID_CONFLICT", "client_request_id undo sudah dipakai untuk permintaan lain");

        var status = await conn.QuerySingleOrDefaultAsync<string>(new CommandDefinition("""
            select status from sessions where session_id = @sessionId and instructor_user_id = @instructorUserId for update
            """, new { sessionId, instructorUserId }, tx, cancellationToken: ct));
        if (status is null) return new(null, 404, "NOT_FOUND", "Sesi tidak ditemukan");
        if (status != "STARTED") return new(null, 422, "SESSION_NOT_STARTED", "Undo hanya tersedia saat sesi sedang berjalan");
        if (await conn.ExecuteScalarAsync<bool>(new CommandDefinition(
            "select exists (select 1 from event_undos where session_id = @sessionId and event_id = @eventId)",
            new { sessionId, eventId }, tx, cancellationToken: ct)))
            return new(null, 409, "EVENT_ALREADY_UNDONE", "Event sudah dibatalkan");

        var target = await conn.QuerySingleOrDefaultAsync<TargetRow>(new CommandDefinition("""
            select e.sequence_number, e.actor_type, e.day_index, e.action_type, to_jsonb(e)::text as original_event,
                snapshot.before_state::text as before_state
            from events e left join event_undo_snapshots snapshot using (session_id, event_id)
            where e.session_id = @sessionId and e.event_id = @eventId
            """, new { sessionId, eventId }, tx, cancellationToken: ct));
        if (target is null) return new(null, 404, "NOT_FOUND", "Event tidak ditemukan");
        if (target.ActorType == "SYSTEM" && target.DayIndex == 0 &&
            target.ActionType is "MulaiSesi" or "BagikanTieBreaker" or "SetupBahanAwal" or "SetupEmasAwal" or "SetupMisiAwal" or "SetupPinjamanAwal" or "SetupAsuransiAwal")
            return new(null, 422, "SETUP_UNDO_NOT_ALLOWED", "Pembagian awal tidak dapat dibatalkan melalui undo event");

        var currentVersion = await conn.ExecuteScalarAsync<long>(new CommandDefinition(
            "select state_version from session_states where session_id = @sessionId", new { sessionId }, tx, cancellationToken: ct));
        if (currentVersion != request.ExpectedStateVersion)
            return new(null, 409, "STATE_VERSION_CONFLICT", "State sesi berubah; muat ulang sebelum undo");
        if (await conn.ExecuteScalarAsync<bool>(new CommandDefinition(
            "select exists (select 1 from events where session_id = @sessionId and sequence_number > @SequenceNumber)",
            new { sessionId, target.SequenceNumber }, tx, cancellationToken: ct)))
            return new(null, 409, "EVENT_NOT_LAST", "Hanya aktivitas terakhir yang dapat dibatalkan");
        if (target.BeforeState is null)
            return new(null, 422, "UNDO_SNAPSHOT_UNAVAILABLE", "Event ini belum memiliki snapshot sebelum aktivitas");

        var nextSequence = await conn.ExecuteScalarAsync<long>(new CommandDefinition("""
            select coalesce(max(sequence_number), -1) + 1 from (
                select sequence_number from events where session_id = @sessionId
                union all
                select (original_event->>'sequence_number')::bigint from event_undos where session_id = @sessionId
            ) history
            """, new { sessionId }, tx, cancellationToken: ct));
        var undoId = Guid.NewGuid();
        var newVersion = checked(currentVersion + 1);
        var inserted = await conn.ExecuteScalarAsync<Guid?>(new CommandDefinition("""
            insert into event_undos (undo_id, session_id, event_id, instructor_user_id, client_request_id,
                expected_state_version, reason, original_event, original_cashflows, original_asset_references,
                state_version, next_sequence_number)
            values (@undoId, @sessionId, @eventId, @instructorUserId, @ClientRequestId, @ExpectedStateVersion, @Reason,
                @OriginalEvent::jsonb,
                (select coalesce(jsonb_agg(to_jsonb(p)), '[]'::jsonb) from event_cashflow_projections p where session_id = @sessionId and event_id = @eventId),
                (select coalesce(jsonb_agg(to_jsonb(a)), '[]'::jsonb) from event_asset_references a where session_id = @sessionId and event_id = @eventId),
                @newVersion, @nextSequence)
            on conflict (instructor_user_id, client_request_id) do nothing returning undo_id
            """, new { undoId, sessionId, eventId, instructorUserId, request.ClientRequestId,
                request.ExpectedStateVersion, request.Reason, target.OriginalEvent, newVersion, nextSequence }, tx, cancellationToken: ct));
        if (!inserted.HasValue)
            return new(null, 409, "CLIENT_REQUEST_ID_CONFLICT", "client_request_id undo sudah dipakai untuk permintaan lain");

        await conn.ExecuteAsync(new CommandDefinition("""
            delete from metric_snapshots where session_id = @sessionId;
            delete from event_cashflow_projections where session_id = @sessionId and event_id = @eventId;
            delete from event_asset_references where session_id = @sessionId and event_id = @eventId;
            delete from events where session_id = @sessionId and event_id = @eventId;
            select restore_session_undo_state(@sessionId, @BeforeState::jsonb, @newVersion);
            update sessions set last_activity_at = clock_timestamp() where session_id = @sessionId;
            """, new { sessionId, eventId, target.BeforeState, newVersion }, tx, cancellationToken: ct));
        await tx.CommitAsync(ct);
        return new(new EventUndoResponse(undoId, eventId, "UNDONE", newVersion, nextSequence), 201);
    }

    public async Task<EventUndoAuditResponse?> ListAsync(Guid sessionId, Guid instructorUserId,
        DateTimeOffset? afterTimestamp, Guid afterId, int limit, CancellationToken ct)
    {
        await using var conn = await dataSource.OpenConnectionAsync(ct);
        var accessible = await conn.ExecuteScalarAsync<bool>(new CommandDefinition("""
            select exists(select 1 from sessions where session_id = @sessionId and instructor_user_id = @instructorUserId)
                or exists(select 1 from event_undos where session_id = @sessionId and instructor_user_id = @instructorUserId)
            """, new { sessionId, instructorUserId }, cancellationToken: ct));
        if (!accessible) return null;
        var rows = (await conn.QueryAsync<AuditRow>(new CommandDefinition("""
            select undo_id, event_id, instructor_user_id, client_request_id, reason, created_at,
                original_event::text as original_event, original_cashflows::text as original_cashflows,
                original_asset_references::text as original_asset_references
            from event_undos where session_id = @sessionId and instructor_user_id = @instructorUserId
                and (@afterTimestamp::timestamptz is null or (created_at, undo_id) > (@afterTimestamp, @afterId))
            order by created_at, undo_id limit @take
            """, new { sessionId, instructorUserId, afterTimestamp, afterId, take = limit + 1 }, cancellationToken: ct))).ToList();
        var hasMore = rows.Count > limit;
        var items = rows.Take(limit).Select(row => new EventUndoAuditItem(row.UndoId, row.EventId, row.InstructorUserId,
            row.ClientRequestId, row.Reason, row.CreatedAt, JsonSerializer.Deserialize<JsonElement>(row.OriginalEvent),
            JsonSerializer.Deserialize<JsonElement>(row.OriginalCashflows), JsonSerializer.Deserialize<JsonElement>(row.OriginalAssetReferences))).ToList();
        return new(items, hasMore ? OpaqueCursor.EncodeTransaction(items[^1].CreatedAt, items[^1].UndoId) : null, hasMore);
    }

    private static EventUndoResponse ToResponse(UndoRow row) => new(row.UndoId, row.EventId, "UNDONE", row.StateVersion, row.NextSequenceNumber);
    private sealed class UndoRow
    {
        public Guid UndoId { get; set; }
        public Guid SessionId { get; set; }
        public Guid EventId { get; set; }
        public string Reason { get; set; } = string.Empty;
        public long ExpectedStateVersion { get; set; }
        public long StateVersion { get; set; }
        public long NextSequenceNumber { get; set; }
    }
    private sealed class TargetRow
    {
        public long SequenceNumber { get; set; }
        public string ActorType { get; set; } = string.Empty;
        public int DayIndex { get; set; }
        public string ActionType { get; set; } = string.Empty;
        public string OriginalEvent { get; set; } = string.Empty;
        public string? BeforeState { get; set; }
    }
    private sealed class AuditRow
    {
        public Guid UndoId { get; set; }
        public Guid EventId { get; set; }
        public Guid InstructorUserId { get; set; }
        public string ClientRequestId { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; }
        public string OriginalEvent { get; set; } = string.Empty;
        public string OriginalCashflows { get; set; } = string.Empty;
        public string OriginalAssetReferences { get; set; } = string.Empty;
    }
}
