using Dapper;
using Npgsql;

namespace Cashflowpoly.Api.Data;

/// <summary>
/// Repository untuk data pemain dan relasi pemain ke sesi.
/// </summary>
public sealed class PlayerRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public PlayerRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

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

    public async Task<int> AddPlayerToSessionAndAssignJoinOrderByPlayerIdAsync(
        Guid sessionId,
        Guid playerId,
        string role,
        CancellationToken ct)
    {
        const string insertSql = """
            insert into session_players (session_player_id, session_id, player_id, join_order, role, created_at)
            values (@sessionPlayerId, @sessionId, @playerId, 0, @role, @createdAt)
            on conflict (session_id, player_id) do nothing
            """;

        const string reorderSql = """
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
            createdAt = DateTimeOffset.UtcNow
        }, tx, cancellationToken: ct));

        await conn.ExecuteAsync(new CommandDefinition(reorderSql, new { sessionId }, tx, cancellationToken: ct));

        var joinOrder = await conn.QuerySingleOrDefaultAsync<int?>(
            new CommandDefinition(selectSql, new { sessionId, playerId }, tx, cancellationToken: ct));

        await tx.CommitAsync(ct);

        if (!joinOrder.HasValue)
        {
            throw new InvalidOperationException("Pemain gagal terdaftar pada sesi.");
        }

        return joinOrder.Value;
    }

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

    private sealed class SessionPlayerJoinOrderDb
    {
        public Guid PlayerId { get; init; }
        public int JoinOrder { get; init; }
    }
}
