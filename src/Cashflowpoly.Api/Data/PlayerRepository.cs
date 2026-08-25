// Fungsi file: Mengelola pemetaan dan akses PostgreSQL untuk PlayerRepository.
using Dapper;
using Npgsql;

namespace Cashflowpoly.Api.Data;

/// <summary>
/// Repository domain pemain dengan sumber identitas app_users role PLAYER.
/// </summary>
public sealed class PlayerRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public PlayerRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<PlayerDb?> GetPlayerAsync(Guid userId, CancellationToken ct)
    {
        const string sql = """
            select
                user_id,
                username,
                display_name,
                null::uuid as instructor_user_id,
                role,
                is_active,
                created_at
            from app_users
            where user_id = @userId
              and role = 'PLAYER'
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        return await conn.QuerySingleOrDefaultAsync<PlayerDb>(new CommandDefinition(sql, new { userId }, cancellationToken: ct));
    }

    public async Task<PlayerDb?> GetPlayerByUsernameAsync(string username, CancellationToken ct)
    {
        const string sql = """
            select
                user_id,
                username,
                display_name,
                null::uuid as instructor_user_id,
                role,
                is_active,
                created_at
            from app_users
            where username = @username
              and role = 'PLAYER'
            limit 1
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        return await conn.QuerySingleOrDefaultAsync<PlayerDb>(
            new CommandDefinition(sql, new { username }, cancellationToken: ct));
    }

    public async Task<List<PlayerDb>> ListPlayersAsync(CancellationToken ct)
    {
        const string sql = """
            select
                user_id,
                username,
                display_name,
                null::uuid as instructor_user_id,
                role,
                is_active,
                created_at
            from app_users
            where role = 'PLAYER'
            order by created_at desc
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        var items = await conn.QueryAsync<PlayerDb>(new CommandDefinition(sql, cancellationToken: ct));
        return items.ToList();
    }

    public async Task<List<PlayerDb>> ListPlayersByPlayerScopeAsync(Guid userId, CancellationToken ct)
    {
        const string sql = """
            select distinct
                u.user_id,
                u.username,
                u.display_name,
                null::uuid as instructor_user_id,
                u.role,
                u.is_active,
                u.created_at
            from app_users u
            where u.role = 'PLAYER'
              and (
                  u.user_id = @userId
                  or exists (
                      select 1
                      from session_participants me
                      join session_participants peer on peer.session_id = me.session_id
                      where me.user_id = @userId
                        and peer.user_id = u.user_id
                  )
              )
            order by u.display_name asc, u.username asc
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        var items = await conn.QueryAsync<PlayerDb>(new CommandDefinition(sql, new { userId }, cancellationToken: ct));
        return items.ToList();
    }

    public async Task<List<SessionPlayerDb>> ListSessionPlayersAsync(Guid sessionId, CancellationToken ct)
    {
        const string sql = """
            select
                sp.session_participant_id as session_player_id,
                sp.user_id,
                coalesce(nullif(sp.player_name, ''), u.display_name) as display_name,
                sp.player_order_no as player_order
            from session_participants sp
            join app_users u on u.user_id = sp.user_id
            where sp.session_id = @sessionId
            order by sp.player_order_no asc, sp.joined_at asc, sp.user_id asc
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        var items = await conn.QueryAsync<SessionPlayerDb>(
            new CommandDefinition(sql, new { sessionId }, cancellationToken: ct));
        return items.ToList();
    }

    public async Task<int> AddPlayerToSessionAndAssignPlayerOrderAsync(
        Guid sessionId,
        Guid userId,
        int? playerOrder,
        CancellationToken ct)
    {
        const string insertSql = """
            insert into session_participants (session_participant_id, session_id, user_id, player_order_no, player_name, joined_at)
            values (
                @sessionPlayerId,
                @sessionId,
                @userId,
                @initialPlayerOrder,
                (select display_name from app_users where user_id = @userId),
                @createdAt
            )
            on conflict (session_id, user_id) do update
            set player_name = excluded.player_name
            """;

        const string reorderByJoinedAtSql = """
            with ranked as (
                select session_participant_id,
                       row_number() over (order by joined_at asc, user_id asc)::int as new_player_order
                from session_participants
                where session_id = @sessionId
            )
            update session_participants sp
            set player_order_no = ranked.new_player_order
            from ranked
            where sp.session_participant_id = ranked.session_participant_id
            """;

        const string applyRequestedPlayerOrderSql = """
            update session_participants
            set player_order_no = player_order_no + 1
            where session_id = @sessionId
              and user_id <> @userId
              and player_order_no >= @playerOrder;

            update session_participants
            set player_order_no = @playerOrder
            where session_id = @sessionId
              and user_id = @userId;
            """;

        const string normalizePlayerOrderSql = """
            with ranked as (
                select session_participant_id,
                       row_number() over (
                           order by player_order_no asc, joined_at asc, user_id asc
                       )::int as new_player_order
                from session_participants
                where session_id = @sessionId
            )
            update session_participants sp
            set player_order_no = ranked.new_player_order
            from ranked
            where sp.session_participant_id = ranked.session_participant_id
            """;

        const string selectSql = """
            select
                session_participant_id as SessionParticipantId,
                player_order_no as PlayerOrderNo
            from session_participants
            where session_id = @sessionId
              and user_id = @userId
            limit 1
            """;

        const string initializeBalanceSql = """
            insert into session_participant_balances (
                session_id,
                session_participant_id,
                coins,
                happiness,
                saving,
                total_donasi,
                created_at,
                updated_at
            )
            select
                @sessionId,
                @sessionParticipantId,
                rgs.starting_cash,
                rgs.starting_happiness,
                rgs.starting_saving,
                0,
                now(),
                now()
            from sessions s
            join ruleset_game_settings rgs on rgs.ruleset_version_id = s.ruleset_version_id
            where s.session_id = @sessionId
            on conflict (session_participant_id) do nothing
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        await using var tx = await conn.BeginTransactionAsync(ct);

        await conn.ExecuteAsync(new CommandDefinition(
            "set constraints uq_session_participants_session_seat deferred",
            transaction: tx,
            cancellationToken: ct));

        await conn.ExecuteAsync(new CommandDefinition(insertSql, new
        {
            sessionPlayerId = Guid.NewGuid(),
            sessionId,
            userId,
            initialPlayerOrder = playerOrder ?? 1,
            createdAt = DateTimeOffset.UtcNow
        }, tx, cancellationToken: ct));

        if (playerOrder.HasValue)
        {
            await conn.ExecuteAsync(new CommandDefinition(
                applyRequestedPlayerOrderSql,
                new { sessionId, userId, playerOrder = playerOrder.Value },
                tx,
                cancellationToken: ct));
            await conn.ExecuteAsync(new CommandDefinition(normalizePlayerOrderSql, new { sessionId }, tx, cancellationToken: ct));
        }
        else
        {
            await conn.ExecuteAsync(new CommandDefinition(reorderByJoinedAtSql, new { sessionId }, tx, cancellationToken: ct));
        }

        var assignment = await conn.QuerySingleOrDefaultAsync<SessionParticipantAssignmentDb>(
            new CommandDefinition(selectSql, new { sessionId, userId }, tx, cancellationToken: ct));

        if (assignment is not null)
        {
            var parameters = new
            {
                sessionId,
                sessionParticipantId = assignment.SessionParticipantId
            };
            await conn.ExecuteAsync(new CommandDefinition(initializeBalanceSql, parameters, tx, cancellationToken: ct));
        }

        await tx.CommitAsync(ct);

        if (assignment is null)
        {
            throw new InvalidOperationException("Pemain gagal terdaftar pada sesi.");
        }

        return assignment.PlayerOrderNo;
    }

    public async Task<Dictionary<Guid, int>> GetSessionPlayerPlayerOrderMapAsync(Guid sessionId, CancellationToken ct)
    {
        const string sql = """
            select user_id, player_order_no as player_order
            from session_participants
            where session_id = @sessionId
            order by player_order_no asc, joined_at asc
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        var rows = await conn.QueryAsync<SessionPlayerPlayerOrderDb>(
            new CommandDefinition(sql, new { sessionId }, cancellationToken: ct));
        return rows.ToDictionary(row => row.UserId, row => row.PlayerOrder);
    }

    public async Task<Dictionary<Guid, int>> GetSessionParticipantPlayerOrderMapAsync(Guid sessionId, CancellationToken ct)
    {
        const string sql = """
            select session_participant_id, player_order_no as player_order
            from session_participants
            where session_id = @sessionId
            order by player_order_no asc, joined_at asc
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        var rows = await conn.QueryAsync<SessionParticipantPlayerOrderDb>(
            new CommandDefinition(sql, new { sessionId }, cancellationToken: ct));
        return rows.ToDictionary(row => row.SessionParticipantId, row => row.PlayerOrder);
    }

    public async Task<Guid?> GetSessionParticipantIdAsync(Guid sessionId, Guid userId, CancellationToken ct)
    {
        const string sql = """
            select session_participant_id
            from session_participants
            where session_id = @sessionId
              and user_id = @userId
            limit 1
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        return await conn.QuerySingleOrDefaultAsync<Guid?>(
            new CommandDefinition(sql, new { sessionId, userId }, cancellationToken: ct));
    }

    public async Task<int> CountPlayersInSessionAsync(Guid sessionId, CancellationToken ct)
    {
        const string sql = """
            select count(*)::int
            from session_participants
            where session_id = @sessionId
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        return await conn.ExecuteScalarAsync<int>(new CommandDefinition(sql, new { sessionId }, cancellationToken: ct));
    }

    public async Task<bool> IsPlayerInSessionAsync(Guid sessionId, Guid userId, CancellationToken ct)
    {
        const string sql = """
            select 1
            from session_participants
            where session_id = @sessionId and user_id = @userId
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        var result = await conn.ExecuteScalarAsync<int?>(new CommandDefinition(sql, new { sessionId, userId }, cancellationToken: ct));
        return result.HasValue;
    }

    private sealed class SessionPlayerPlayerOrderDb
    {
        public Guid UserId { get; init; }
        public int PlayerOrder { get; init; }
    }

    private sealed class SessionParticipantPlayerOrderDb
    {
        public Guid SessionParticipantId { get; init; }
        public int PlayerOrder { get; init; }
    }

    private sealed class SessionParticipantAssignmentDb
    {
        public Guid SessionParticipantId { get; init; }
        public int PlayerOrderNo { get; init; }
    }
}
