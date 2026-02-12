using Dapper;
using Npgsql;

namespace Cashflowpoly.Api.Data;

public sealed record AuthenticatedUserDb(Guid UserId, string Username, string Role, bool IsActive);

/// <summary>
/// Repository untuk autentikasi user aplikasi.
/// </summary>
public sealed class UserRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public UserRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<AuthenticatedUserDb?> AuthenticateAsync(string username, string password, CancellationToken ct)
    {
        const string sql = """
            select user_id, username, role, is_active
            from app_users
            where lower(username) = lower(@username)
              and is_active = true
              and password_hash = crypt(@password, password_hash)
            limit 1
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        return await conn.QueryFirstOrDefaultAsync<AuthenticatedUserDb>(
            new CommandDefinition(sql, new { username, password }, cancellationToken: ct));
    }

    public async Task<bool> UsernameExistsAsync(string username, CancellationToken ct)
    {
        const string sql = """
            select 1
            from app_users
            where lower(username) = lower(@username)
            limit 1
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        var result = await conn.ExecuteScalarAsync<int?>(
            new CommandDefinition(sql, new { username }, cancellationToken: ct));
        return result.HasValue;
    }

    public async Task<AuthenticatedUserDb> CreateUserAsync(string username, string password, string role, CancellationToken ct)
    {
        const string insertUserSql = """
            insert into app_users (user_id, username, password_hash, role, is_active, created_at)
            values (@userId, @username, crypt(@password, gen_salt('bf', 10)), @role, true, now())
            returning user_id, username, role, is_active
            """;

        const string insertPlayerSql = """
            insert into players (player_id, display_name, created_at)
            values (@playerId, @displayName, @createdAt)
            """;

        const string insertUserPlayerLinkSql = """
            insert into user_player_links (link_id, user_id, player_id, created_at)
            values (@linkId, @userId, @playerId, @createdAt)
            """;

        var userId = Guid.NewGuid();
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        await using var tx = await conn.BeginTransactionAsync(ct);

        var created = await conn.QuerySingleAsync<AuthenticatedUserDb>(new CommandDefinition(insertUserSql, new
        {
            userId,
            username,
            password,
            role
        }, tx, cancellationToken: ct));

        if (string.Equals(role, "PLAYER", StringComparison.OrdinalIgnoreCase))
        {
            var playerId = Guid.NewGuid();
            var createdAt = DateTimeOffset.UtcNow;
            await conn.ExecuteAsync(new CommandDefinition(insertPlayerSql, new
            {
                playerId,
                displayName = username,
                createdAt
            }, tx, cancellationToken: ct));

            await conn.ExecuteAsync(new CommandDefinition(insertUserPlayerLinkSql, new
            {
                linkId = Guid.NewGuid(),
                userId,
                playerId,
                createdAt
            }, tx, cancellationToken: ct));
        }

        await tx.CommitAsync(ct);
        return created;
    }

    public async Task<Guid?> GetLinkedPlayerIdAsync(Guid userId, CancellationToken ct)
    {
        const string sql = """
            select player_id
            from user_player_links
            where user_id = @userId
            limit 1
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        return await conn.QuerySingleOrDefaultAsync<Guid?>(new CommandDefinition(sql, new { userId }, cancellationToken: ct));
    }

    public async Task<Guid> EnsurePlayerLinkAsync(Guid userId, string username, CancellationToken ct)
    {
        const string lockUserSql = """
            select user_id
            from app_users
            where user_id = @userId
            for update
            """;

        const string getLinkSql = """
            select player_id
            from user_player_links
            where user_id = @userId
            limit 1
            """;

        const string insertPlayerSql = """
            insert into players (player_id, display_name, created_at)
            values (@playerId, @displayName, @createdAt)
            """;

        const string insertLinkSql = """
            insert into user_player_links (link_id, user_id, player_id, created_at)
            values (@linkId, @userId, @playerId, @createdAt)
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        await using var tx = await conn.BeginTransactionAsync(ct);

        await conn.ExecuteAsync(new CommandDefinition(lockUserSql, new { userId }, tx, cancellationToken: ct));

        var linked = await conn.QuerySingleOrDefaultAsync<Guid?>(new CommandDefinition(getLinkSql, new { userId }, tx, cancellationToken: ct));
        if (linked.HasValue)
        {
            await tx.CommitAsync(ct);
            return linked.Value;
        }

        var playerId = Guid.NewGuid();
        var createdAt = DateTimeOffset.UtcNow;
        await conn.ExecuteAsync(new CommandDefinition(insertPlayerSql, new
        {
            playerId,
            displayName = username,
            createdAt
        }, tx, cancellationToken: ct));

        await conn.ExecuteAsync(new CommandDefinition(insertLinkSql, new
        {
            linkId = Guid.NewGuid(),
            userId,
            playerId,
            createdAt
        }, tx, cancellationToken: ct));

        await tx.CommitAsync(ct);
        return playerId;
    }
}
