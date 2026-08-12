// Fungsi file: Mengelola pemetaan dan akses PostgreSQL untuk EventRepository.
using Dapper;
using Npgsql;

namespace Cashflowpoly.Api.Data;

/// <summary>
/// Repository untuk event, log validasi, dan proyeksi arus kas.
/// </summary>
public sealed class EventRepository
{
    private readonly NpgsqlDataSource _dataSource;

    private const string EventSelectColumns = """
        select
            e.event_pk,
            e.event_id,
            e.session_id,
            e.session_player_id,
            e.user_id,
            e.actor_type,
            e.timestamp,
            e.day_index,
            e.weekday,
            e.turn_number,
            e.action_slot,
            e.sequence_number,
            e.ruleset_action_id,
            ra.action_id,
            e.action_type,
            e.ruleset_version_id,
            e.payload_version,
            coalesce(e.payload::text, '{}') as payload,
            e.received_at,
            e.client_request_id
        from events e
        join ruleset_actions ra
          on ra.ruleset_version_id = e.ruleset_version_id
         and ra.ruleset_action_id = e.ruleset_action_id
        """;

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
            session_player_id,
            user_id,
            actor_type,
            timestamp,
            day_index,
            weekday,
            turn_number,
            action_slot,
            sequence_number,
            ruleset_action_id,
            action_type,
            ruleset_version_id,
            payload_version,
            payload,
            received_at,
            client_request_id
        )
        select
            @EventPk,
            @EventId,
            @SessionId,
            @SessionPlayerId,
            @UserId,
            @ActorType,
            @Timestamp,
            @DayIndex,
            @Weekday,
            @TurnNumber,
            @ActionSlot,
            @SequenceNumber,
            ra.ruleset_action_id,
            @ActionType,
            @RulesetVersionId,
            @PayloadVersion,
            @Payload::jsonb,
            @ReceivedAt,
            @ClientRequestId
        from ruleset_actions ra
        where ra.ruleset_version_id = @RulesetVersionId
          and lower(ra.action_id) = lower(@ActionId)
          and ra.is_active
        limit 1
        returning ruleset_action_id
        """;

    public async Task InsertEventAsync(EventDb record, CancellationToken ct)
    {
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        var rulesetActionId = await conn.QuerySingleOrDefaultAsync<Guid?>(
            new CommandDefinition(InsertEventSql, BuildEventParameters(record), cancellationToken: ct));
        record.RulesetActionId = EnsureActionResolved(rulesetActionId, record);
    }

    /// <summary>
    /// Menyisipkan event menggunakan koneksi dan transaksi yang sudah ada (untuk atomisitas dengan proyeksi).
    /// </summary>
    internal async Task InsertEventAsync(EventDb record, NpgsqlConnection conn, NpgsqlTransaction tx, CancellationToken ct)
    {
        var rulesetActionId = await conn.QuerySingleOrDefaultAsync<Guid?>(
            new CommandDefinition(InsertEventSql, BuildEventParameters(record), tx, cancellationToken: ct));
        record.RulesetActionId = EnsureActionResolved(rulesetActionId, record);
    }

    internal async Task InsertEventAssetReferencesAsync(
        EventDb record,
        IReadOnlyCollection<EventAssetReferenceInput> references,
        NpgsqlConnection conn,
        NpgsqlTransaction tx,
        CancellationToken ct)
    {
        if (references.Count == 0)
        {
            return;
        }

        const string resolveSql = """
            select asset.ruleset_game_asset_id
            from ruleset_game_assets asset
            where asset.ruleset_version_id = @rulesetVersionId
              and asset.asset_type = @assetType
              and (
                lower(asset.asset_code) = lower(@assetCode)
                or lower(asset.display_name) = lower(@assetCode)
              )
              and asset.is_active
            limit 1
            """;

        const string insertSql = """
            insert into event_asset_references (
                event_asset_reference_id,
                session_id,
                event_id,
                ruleset_version_id,
                ruleset_game_asset_id,
                reference_role,
                payload_path,
                created_at
            )
            values (
                @referenceId,
                @sessionId,
                @eventId,
                @rulesetVersionId,
                @rulesetGameAssetId,
                @referenceRole,
                @payloadPath,
                now()
            )
            on conflict (session_id, event_id, ruleset_game_asset_id, reference_role) do nothing
            """;

        foreach (var reference in references
                     .GroupBy(reference => new
                     {
                         AssetType = reference.AssetType.Trim().ToUpperInvariant(),
                         AssetCode = reference.AssetCode.Trim().ToUpperInvariant(),
                         ReferenceRole = reference.ReferenceRole.Trim().ToUpperInvariant()
                     })
                     .Select(group => group.First()))
        {
            var rulesetGameAssetId = await conn.QuerySingleOrDefaultAsync<Guid?>(
                new CommandDefinition(
                    resolveSql,
                    new
                    {
                        rulesetVersionId = record.RulesetVersionId,
                        reference.AssetType,
                        reference.AssetCode
                    },
                    tx,
                    cancellationToken: ct));

            if (!rulesetGameAssetId.HasValue)
            {
                throw new InvalidOperationException(
                    $"Asset '{reference.AssetType}/{reference.AssetCode}' tidak ditemukan pada ruleset event.");
            }

            await conn.ExecuteAsync(new CommandDefinition(
                insertSql,
                new
                {
                    referenceId = Guid.NewGuid(),
                    sessionId = record.SessionId,
                    eventId = record.EventId,
                    rulesetVersionId = record.RulesetVersionId,
                    rulesetGameAssetId = rulesetGameAssetId.Value,
                    reference.ReferenceRole,
                    reference.PayloadPath
                },
                tx,
                cancellationToken: ct));
        }
    }

    /// <summary>
    /// Menyimpan catatan validasi gagal. Event valid hanya masuk stream events.
    /// </summary>
    public async Task InsertValidationLogAsync(
        Guid sessionId,
        Guid eventId,
        Guid? rulesetVersionId,
        string rawPayloadJson,
        string? errorCode,
        string? errorMessage,
        string? detailsJson,
        CancellationToken ct)
    {
        const string sql = """
            insert into validation_logs (
                validation_log_id,
                session_id,
                ruleset_version_id,
                event_id,
                raw_payload_json,
                error_code,
                error_message,
                details_json,
                created_at
            )
            values (
                @validationLogId,
                @sessionId,
                @rulesetVersionId,
                @eventId,
                @rawPayloadJson::jsonb,
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
            eventId,
            rulesetVersionId,
            rawPayloadJson = string.IsNullOrWhiteSpace(rawPayloadJson) ? "{}" : rawPayloadJson,
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
            user_id,
            event_pk,
            event_id,
            projection_order,
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
            @UserId,
            @EventPk,
            @EventId,
            @ProjectionOrder,
            @Timestamp,
            @Direction,
            @Amount,
            @Category,
            @Counterparty,
            @Reference,
            @Note
        )
        on conflict (session_id, event_id, projection_order) do nothing
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

    internal async Task<Guid?> ResolveSessionParticipantIdAsync(
        Guid sessionId,
        Guid userId,
        NpgsqlConnection conn,
        NpgsqlTransaction tx,
        CancellationToken ct)
    {
        const string sql = """
            select session_participant_id
            from session_participants
            where session_id = @sessionId
              and user_id = @userId
            limit 1
            """;

        return await conn.QuerySingleOrDefaultAsync<Guid?>(
            new CommandDefinition(sql, new { sessionId, userId }, tx, cancellationToken: ct));
    }

    /// <summary>
    /// Mengambil seluruh proyeksi arus kas dalam satu sesi.
    /// </summary>
    public async Task<List<CashflowProjectionDb>> GetCashflowProjectionsAsync(Guid sessionId, CancellationToken ct)
    {
        const string sql = """
            select projection_id,
                   session_id,
                   user_id,
                   event_pk,
                   event_id,
                   projection_order,
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
        var sql = EventSelectColumns + """

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
        var sql = EventSelectColumns + """

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
        var sql = EventSelectColumns + """

            where session_id = @sessionId
              and event_id = @eventId
            limit 1
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        return await conn.QuerySingleOrDefaultAsync<EventDb>(
            new CommandDefinition(sql, new { sessionId, eventId }, cancellationToken: ct));
    }

    internal async Task<bool> IsRiskResolvedAsync(Guid sessionId, Guid riskEventId, CancellationToken ct)
    {
        const string sql = """
            select exists (
                select 1
                from event_cashflow_projections
                where session_id = @sessionId
                  and category = 'RISK_LIFE'
                  and (event_id = @riskEventId or reference = @riskEventId::text)
            )
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        return await conn.ExecuteScalarAsync<bool>(
            new CommandDefinition(sql, new { sessionId, riskEventId }, cancellationToken: ct));
    }

    internal async Task<bool> HasPendingLifeRiskAsync(Guid sessionId, CancellationToken ct)
    {
        const string sql = """
            select exists (
                select 1
                from events risk_event
                join ruleset_life_risks risk
                  on risk.ruleset_version_id = risk_event.ruleset_version_id
                 and lower(risk.risk_code) = lower(risk_event.payload ->> 'risk_id')
                where risk_event.session_id = @sessionId
                  and risk_event.action_type = 'RisikoKehidupan'
                  and risk.effect_type = 'COIN_EFFECT'
                  and risk.direction = 'OUT'
                  and not exists (
                      select 1
                      from event_cashflow_projections projection
                      where projection.session_id = risk_event.session_id
                        and projection.category = 'RISK_LIFE'
                        and projection.direction = 'OUT'
                        and projection.reference = risk_event.event_id::text
                  )
            )
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        return await conn.ExecuteScalarAsync<bool>(
            new CommandDefinition(sql, new { sessionId }, cancellationToken: ct));
    }

    internal async Task<int?> GetOwnedNeedSaleAmountAsync(
        Guid sessionId,
        Guid userId,
        string cardId,
        CancellationToken ct)
    {
        const string sql = """
            select floor(spnp.paid_amount / 2.0)::int
            from session_participant_need_purchases spnp
            join session_participants sp on sp.session_participant_id = spnp.session_participant_id
            join ruleset_needs rn on rn.ruleset_need_id = spnp.ruleset_need_id
            where sp.session_id = @sessionId
              and sp.user_id = @userId
              and lower(rn.need_code) = lower(@cardId)
              and not spnp.is_sold
            order by spnp.purchased_at_day, spnp.sort_order
            limit 1
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        return await conn.ExecuteScalarAsync<int?>(
            new CommandDefinition(sql, new { sessionId, userId, cardId }, cancellationToken: ct));
    }

    internal async Task<int> GetGoldQuantityAsync(Guid sessionId, Guid userId, CancellationToken ct)
    {
        const string sql = """
            select coalesce(sum(spgh.quantity), 0)::int
            from session_participant_gold_holdings spgh
            join session_participants sp on sp.session_participant_id = spgh.session_participant_id
            where sp.session_id = @sessionId
              and sp.user_id = @userId
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        return await conn.ExecuteScalarAsync<int>(
            new CommandDefinition(sql, new { sessionId, userId }, cancellationToken: ct));
    }

    internal async Task<bool> HasActiveInsuranceAsync(Guid sessionId, Guid userId, CancellationToken ct)
    {
        const string sql = """
            select exists (
                select 1
                from session_participant_insurances spi
                join session_participants sp on sp.session_participant_id = spi.session_participant_id
                where sp.session_id = @sessionId
                  and sp.user_id = @userId
                  and spi.status = 'ACTIVE'
                  and spi.remaining_uses > 0
            )
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        return await conn.ExecuteScalarAsync<bool>(
            new CommandDefinition(sql, new { sessionId, userId }, cancellationToken: ct));
    }

    internal async Task<int?> GetActiveLoanOutstandingAsync(
        Guid sessionId,
        Guid userId,
        string loanInstanceId,
        CancellationToken ct)
    {
        const string sql = """
            select spl.outstanding_amount
            from session_participant_loans spl
            join session_participants sp on sp.session_participant_id = spl.session_participant_id
            where sp.session_id = @sessionId
              and sp.user_id = @userId
              and lower(spl.loan_instance_id) = lower(@loanInstanceId)
              and spl.status = 'ACTIVE'
            limit 1
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        return await conn.ExecuteScalarAsync<int?>(
            new CommandDefinition(sql, new { sessionId, userId, loanInstanceId }, cancellationToken: ct));
    }

    private static object BuildEventParameters(EventDb record)
    {
        return new
        {
            record.EventPk,
            record.EventId,
            record.SessionId,
            record.SessionPlayerId,
            record.UserId,
            record.ActorType,
            record.Timestamp,
            record.DayIndex,
            record.Weekday,
            record.TurnNumber,
            record.ActionSlot,
            record.SequenceNumber,
            ActionId = ResolveActionId(record),
            ActionType = ResolveActionId(record) ?? record.ActionType,
            record.RulesetVersionId,
            PayloadVersion = string.IsNullOrWhiteSpace(record.PayloadVersion) ? "1.0" : record.PayloadVersion,
            record.Payload,
            record.ReceivedAt,
            record.ClientRequestId
        };
    }

    private static Guid EnsureActionResolved(Guid? rulesetActionId, EventDb record)
    {
        if (!rulesetActionId.HasValue)
        {
            var actionId = string.IsNullOrWhiteSpace(record.ActionId)
                ? EventActionIdResolver.Resolve(record.ActionType, record.Payload)
                : record.ActionId;
            throw new InvalidOperationException(
                $"Action '{actionId}' tidak aktif atau tidak terdaftar pada ruleset event.");
        }

        return rulesetActionId.Value;
    }

    private static string? ResolveActionId(EventDb record)
    {
        var actionId = string.IsNullOrWhiteSpace(record.ActionId)
            ? EventActionIdResolver.Resolve(record.ActionType, record.Payload)
            : record.ActionId;

        if (!string.IsNullOrWhiteSpace(actionId))
        {
            record.ActionId = actionId;
            record.ActionType = actionId;
        }

        return actionId;
    }
}

internal sealed record EventAssetReferenceInput(
    string AssetType,
    string AssetCode,
    string ReferenceRole,
    string PayloadPath);
