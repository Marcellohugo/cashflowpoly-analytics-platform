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
            select player_id, display_name, created_at
            from players
            where player_id = @playerId
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        return await conn.QuerySingleOrDefaultAsync<PlayerDb>(new CommandDefinition(sql, new { playerId }, cancellationToken: ct));
    }

    public async Task<Guid> CreatePlayerAsync(string displayName, CancellationToken ct)
    {
        var playerId = Guid.NewGuid();

        const string sql = """
            insert into players (player_id, display_name, created_at)
            values (@playerId, @displayName, @createdAt)
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        await conn.ExecuteAsync(new CommandDefinition(sql, new
        {
            playerId,
            displayName,
            createdAt = DateTimeOffset.UtcNow
        }, cancellationToken: ct));

        return playerId;
    }

    public async Task<List<PlayerDb>> ListPlayersAsync(CancellationToken ct)
    {
        const string sql = """
            select player_id, display_name, created_at
            from players
            order by created_at desc
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        var items = await conn.QueryAsync<PlayerDb>(new CommandDefinition(sql, cancellationToken: ct));
        return items.ToList();
    }

    public async Task AddPlayerToSessionAsync(Guid sessionId, Guid playerId, string role, int joinOrder, CancellationToken ct)
    {
        const string sql = """
            insert into session_players (session_player_id, session_id, player_id, join_order, role, created_at)
            values (@sessionPlayerId, @sessionId, @playerId, @joinOrder, @role, @createdAt)
            on conflict (session_id, player_id) do nothing
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        await conn.ExecuteAsync(new CommandDefinition(sql, new
        {
            sessionPlayerId = Guid.NewGuid(),
            sessionId,
            playerId,
            joinOrder,
            role,
            createdAt = DateTimeOffset.UtcNow
        }, cancellationToken: ct));
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
}
