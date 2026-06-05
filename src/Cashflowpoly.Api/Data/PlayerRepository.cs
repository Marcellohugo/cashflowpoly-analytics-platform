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

    public Task<PlayerDb?> GetPlayerForInstructorAsync(Guid userId, Guid instructorUserId, CancellationToken ct)
    {
        return GetPlayerAsync(userId, ct);
    }

    public async Task<PlayerDb?> GetPlayerForInstructorByUsernameAsync(string username, Guid instructorUserId, CancellationToken ct)
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
            where lower(username) = lower(@username)
              and role = 'PLAYER'
            limit 1
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        return await conn.QuerySingleOrDefaultAsync<PlayerDb>(
            new CommandDefinition(sql, new { username }, cancellationToken: ct));
    }

    public async Task<List<PlayerDb>> ListPlayersAsync(Guid instructorUserId, CancellationToken ct)
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
                      from session_players me
                      join session_players peer on peer.session_id = me.session_id
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

    public async Task<int> AddPlayerToSessionAndAssignJoinOrderAsync(
        Guid sessionId,
        Guid userId,
        string role,
        int? joinOrder,
        CancellationToken ct)
    {
        const string insertSql = """
            insert into session_players (session_player_id, session_id, user_id, join_order, role, created_at)
            values (@sessionPlayerId, @sessionId, @userId, @initialJoinOrder, @role, @createdAt)
            on conflict (session_id, user_id) do update
            set role = excluded.role
            """;

        const string reorderByUserIdSql = """
            with ranked as (
                select session_player_id,
                       row_number() over (order by user_id asc)::int as new_join_order
                from session_players
                where session_id = @sessionId
            )
            update session_players sp
            set join_order = ranked.new_join_order
            from ranked
            where sp.session_player_id = ranked.session_player_id
            """;

        const string applyRequestedJoinOrderSql = """
            update session_players
            set join_order = join_order + 1
            where session_id = @sessionId
              and user_id <> @userId
              and join_order >= @joinOrder;

            update session_players
            set join_order = @joinOrder
            where session_id = @sessionId
              and user_id = @userId;
            """;

        const string normalizeJoinOrderSql = """
            with ranked as (
                select session_player_id,
                       row_number() over (
                           order by join_order asc, created_at asc, user_id asc
                       )::int as new_join_order
                from session_players
                where session_id = @sessionId
            )
            update session_players sp
            set join_order = ranked.new_join_order
            from ranked
            where sp.session_player_id = ranked.session_player_id
            """;

        const string selectSql = """
            select join_order
            from session_players
            where session_id = @sessionId
              and user_id = @userId
            limit 1
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        await using var tx = await conn.BeginTransactionAsync(ct);

        await conn.ExecuteAsync(new CommandDefinition(insertSql, new
        {
            sessionPlayerId = Guid.NewGuid(),
            sessionId,
            userId,
            role,
            initialJoinOrder = joinOrder ?? 1,
            createdAt = DateTimeOffset.UtcNow
        }, tx, cancellationToken: ct));

        if (joinOrder.HasValue)
        {
            await conn.ExecuteAsync(new CommandDefinition(
                applyRequestedJoinOrderSql,
                new { sessionId, userId, joinOrder = joinOrder.Value },
                tx,
                cancellationToken: ct));
            await conn.ExecuteAsync(new CommandDefinition(normalizeJoinOrderSql, new { sessionId }, tx, cancellationToken: ct));
        }
        else
        {
            await conn.ExecuteAsync(new CommandDefinition(reorderByUserIdSql, new { sessionId }, tx, cancellationToken: ct));
        }

        var assignedJoinOrder = await conn.QuerySingleOrDefaultAsync<int?>(
            new CommandDefinition(selectSql, new { sessionId, userId }, tx, cancellationToken: ct));

        await tx.CommitAsync(ct);

        if (!assignedJoinOrder.HasValue)
        {
            throw new InvalidOperationException("Pemain gagal terdaftar pada sesi.");
        }

        return assignedJoinOrder.Value;
    }

    public async Task<Dictionary<Guid, int>> GetSessionPlayerJoinOrderMapAsync(Guid sessionId, CancellationToken ct)
    {
        const string sql = """
            select user_id, join_order
            from session_players
            where session_id = @sessionId
            order by join_order asc, created_at asc
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        var rows = await conn.QueryAsync<SessionPlayerJoinOrderDb>(
            new CommandDefinition(sql, new { sessionId }, cancellationToken: ct));
        return rows.ToDictionary(row => row.UserId, row => row.JoinOrder);
    }

    public async Task<int> CountPlayersInSessionAsync(Guid sessionId, CancellationToken ct)
    {
        const string sql = """
            select count(*)::int
            from session_players
            where session_id = @sessionId
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        return await conn.ExecuteScalarAsync<int>(new CommandDefinition(sql, new { sessionId }, cancellationToken: ct));
    }

    public async Task<bool> IsPlayerInSessionAsync(Guid sessionId, Guid userId, CancellationToken ct)
    {
        const string sql = """
            select 1
            from session_players
            where session_id = @sessionId and user_id = @userId
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        var result = await conn.ExecuteScalarAsync<int?>(new CommandDefinition(sql, new { sessionId, userId }, cancellationToken: ct));
        return result.HasValue;
    }

    private sealed class SessionPlayerJoinOrderDb
    {
        public Guid UserId { get; init; }
        public int JoinOrder { get; init; }
    }
}
