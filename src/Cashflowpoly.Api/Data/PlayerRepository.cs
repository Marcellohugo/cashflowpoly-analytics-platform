// Fungsi file: Menyediakan akses data PostgreSQL untuk domain PlayerRepository melalui query dan command terenkapsulasi.
using Dapper;
using Npgsql;

namespace Cashflowpoly.Api.Data;

/// <summary>
/// Repository untuk data pemain dan relasi pemain ke sesi.
/// </summary>
public sealed class PlayerRepository
{
    private readonly NpgsqlDataSource _dataSource;

    /// <summary>
    /// Menjalankan fungsi PlayerRepository sebagai bagian dari alur file ini.
    /// </summary>
    public PlayerRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    /// <summary>
    /// Menjalankan fungsi GetPlayerAsync sebagai bagian dari alur file ini.
    /// </summary>
    public async Task<PlayerDb?> GetPlayerAsync(Guid playerId, CancellationToken ct)
    {
        const string sql = """
            select player_id, display_name, instructor_user_id, created_at
            from players
            where player_id = @playerId
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        return await conn.QuerySingleOrDefaultAsync<PlayerDb>(new CommandDefinition(sql, new { playerId }, cancellationToken: ct));
    }

    /// <summary>
    /// Menjalankan fungsi GetPlayerForInstructorAsync sebagai bagian dari alur file ini.
    /// </summary>
    public async Task<PlayerDb?> GetPlayerForInstructorAsync(Guid playerId, Guid instructorUserId, CancellationToken ct)
    {
        const string sql = """
            select player_id, display_name, instructor_user_id, created_at
            from players
            where player_id = @playerId
              and instructor_user_id = @instructorUserId
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        return await conn.QuerySingleOrDefaultAsync<PlayerDb>(
            new CommandDefinition(sql, new { playerId, instructorUserId }, cancellationToken: ct));
    }

    /// <summary>
    /// Menjalankan fungsi GetPlayerForInstructorByUsernameAsync sebagai bagian dari alur file ini.
    /// </summary>
    public async Task<PlayerDb?> GetPlayerForInstructorByUsernameAsync(string username, Guid instructorUserId, CancellationToken ct)
    {
        const string sql = """
            select p.player_id, p.display_name, p.instructor_user_id, p.created_at
            from players p
            join user_player_links upl on upl.player_id = p.player_id
            join app_users u on u.user_id = upl.user_id
            where lower(u.username) = lower(@username)
              and p.instructor_user_id = @instructorUserId
            limit 1
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        return await conn.QuerySingleOrDefaultAsync<PlayerDb>(
            new CommandDefinition(sql, new { username, instructorUserId }, cancellationToken: ct));
    }

    /// <summary>
    /// Menjalankan fungsi CreatePlayerAsync sebagai bagian dari alur file ini.
    /// </summary>
    public async Task<Guid> CreatePlayerAsync(string displayName, Guid instructorUserId, CancellationToken ct)
    {
        var playerId = Guid.NewGuid();

        const string sql = """
            insert into players (player_id, display_name, instructor_user_id, created_at)
            values (@playerId, @displayName, @instructorUserId, @createdAt)
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        await conn.ExecuteAsync(new CommandDefinition(sql, new
        {
            playerId,
            displayName,
            instructorUserId,
            createdAt = DateTimeOffset.UtcNow
        }, cancellationToken: ct));

        return playerId;
    }

    /// <summary>
    /// Menjalankan fungsi UpdatePlayerProfileAsync sebagai bagian dari alur file ini.
    /// </summary>
    public async Task<bool> UpdatePlayerProfileAsync(Guid playerId, string displayName, Guid? instructorUserId, CancellationToken ct)
    {
        const string sql = """
            update players
            set display_name = @displayName,
                instructor_user_id = @instructorUserId
            where player_id = @playerId
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        var rows = await conn.ExecuteAsync(new CommandDefinition(sql, new
        {
            playerId,
            displayName,
            instructorUserId
        }, cancellationToken: ct));

        return rows > 0;
    }

    /// <summary>
    /// Menjalankan fungsi ListPlayersAsync sebagai bagian dari alur file ini.
    /// </summary>
    public async Task<List<PlayerDb>> ListPlayersAsync(Guid instructorUserId, CancellationToken ct)
    {
        const string sql = """
            select player_id, display_name, instructor_user_id, created_at
            from players
            where instructor_user_id = @instructorUserId
            order by created_at desc
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        var items = await conn.QueryAsync<PlayerDb>(new CommandDefinition(sql, new { instructorUserId }, cancellationToken: ct));
        return items.ToList();
    }

    /// <summary>
    /// Menjalankan fungsi ListPlayersByPlayerScopeAsync sebagai bagian dari alur file ini.
    /// </summary>
    public async Task<List<PlayerDb>> ListPlayersByPlayerScopeAsync(Guid playerId, CancellationToken ct)
    {
        const string sql = """
            select distinct p.player_id, p.display_name, p.instructor_user_id, p.created_at
            from players p
            where p.player_id = @playerId
               or exists (
                   select 1
                   from session_players me
                   join session_players peer on peer.session_id = me.session_id
                   where me.player_id = @playerId
                     and peer.player_id = p.player_id
               )
            order by p.display_name asc
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        var items = await conn.QueryAsync<PlayerDb>(new CommandDefinition(sql, new { playerId }, cancellationToken: ct));
        return items.ToList();
    }

    /// <summary>
    /// Menjalankan fungsi AddPlayerToSessionAndAssignJoinOrderAsync sebagai bagian dari alur file ini.
    /// </summary>
    public async Task<int> AddPlayerToSessionAndAssignJoinOrderAsync(
        Guid sessionId,
        Guid playerId,
        string role,
        int? joinOrder,
        CancellationToken ct)
    {
        const string insertSql = """
            insert into session_players (session_player_id, session_id, player_id, join_order, role, created_at)
            values (@sessionPlayerId, @sessionId, @playerId, @initialJoinOrder, @role, @createdAt)
            on conflict (session_id, player_id) do update
            set role = excluded.role
            """;

        const string reorderByPlayerIdSql = """
            with ranked as (
                select session_player_id,
                       row_number() over (order by player_id asc)::int as new_join_order
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
              and player_id <> @playerId
              and join_order >= @joinOrder;

            update session_players
            set join_order = @joinOrder
            where session_id = @sessionId
              and player_id = @playerId;
            """;

        const string normalizeJoinOrderSql = """
            with ranked as (
                select session_player_id,
                       row_number() over (
                           order by join_order asc, created_at asc, player_id asc
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
              and player_id = @playerId
            limit 1
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        await using var tx = await conn.BeginTransactionAsync(ct);

        await conn.ExecuteAsync(new CommandDefinition(insertSql, new
        {
            sessionPlayerId = Guid.NewGuid(),
            sessionId,
            playerId,
            role,
            initialJoinOrder = joinOrder ?? 0,
            createdAt = DateTimeOffset.UtcNow
        }, tx, cancellationToken: ct));

        if (joinOrder.HasValue)
        {
            await conn.ExecuteAsync(new CommandDefinition(
                applyRequestedJoinOrderSql,
                new { sessionId, playerId, joinOrder = joinOrder.Value },
                tx,
                cancellationToken: ct));
            await conn.ExecuteAsync(new CommandDefinition(normalizeJoinOrderSql, new { sessionId }, tx, cancellationToken: ct));
        }
        else
        {
            await conn.ExecuteAsync(new CommandDefinition(reorderByPlayerIdSql, new { sessionId }, tx, cancellationToken: ct));
        }

        var assignedJoinOrder = await conn.QuerySingleOrDefaultAsync<int?>(
            new CommandDefinition(selectSql, new { sessionId, playerId }, tx, cancellationToken: ct));

        await tx.CommitAsync(ct);

        if (!assignedJoinOrder.HasValue)
        {
            throw new InvalidOperationException("Pemain gagal terdaftar pada sesi.");
        }

        return assignedJoinOrder.Value;
    }

    /// <summary>
    /// Menjalankan fungsi GetSessionPlayerJoinOrderMapAsync sebagai bagian dari alur file ini.
    /// </summary>
    public async Task<Dictionary<Guid, int>> GetSessionPlayerJoinOrderMapAsync(Guid sessionId, CancellationToken ct)
    {
        const string sql = """
            select player_id, join_order
            from session_players
            where session_id = @sessionId
            order by join_order asc, created_at asc
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        var rows = await conn.QueryAsync<SessionPlayerJoinOrderDb>(
            new CommandDefinition(sql, new { sessionId }, cancellationToken: ct));
        return rows.ToDictionary(row => row.PlayerId, row => row.JoinOrder);
    }

    /// <summary>
    /// Menjalankan fungsi CountPlayersInSessionAsync sebagai bagian dari alur file ini.
    /// </summary>
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

    /// <summary>
    /// Menjalankan fungsi IsPlayerInSessionAsync sebagai bagian dari alur file ini.
    /// </summary>
    public async Task<bool> IsPlayerInSessionAsync(Guid sessionId, Guid playerId, CancellationToken ct)
    {
        const string sql = """
            select 1
            from session_players
            where session_id = @sessionId and player_id = @playerId
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        var result = await conn.ExecuteScalarAsync<int?>(new CommandDefinition(sql, new { sessionId, playerId }, cancellationToken: ct));
        return result.HasValue;
    }

    /// <summary>
    /// Menyatakan peran utama tipe SessionPlayerJoinOrderDb pada modul ini.
    /// </summary>
    private sealed class SessionPlayerJoinOrderDb
    {
        public Guid PlayerId { get; init; }
        public int JoinOrder { get; init; }
    }
}
