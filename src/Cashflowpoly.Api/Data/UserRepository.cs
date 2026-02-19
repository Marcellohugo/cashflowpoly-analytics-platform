// Fungsi file: Menyediakan akses data PostgreSQL untuk domain UserRepository melalui query dan command terenkapsulasi.
using Dapper;
using Npgsql;

namespace Cashflowpoly.Api.Data;

/// <summary>
/// Menyatakan peran utama tipe AuthenticatedUserDb pada modul ini.
/// </summary>
public sealed record AuthenticatedUserDb(Guid UserId, string Username, string Role, bool IsActive);

/// <summary>
/// Repository untuk autentikasi user aplikasi.
/// </summary>
public sealed class UserRepository
{
    private readonly NpgsqlDataSource _dataSource;

    /// <summary>
    /// Menjalankan fungsi UserRepository sebagai bagian dari alur file ini.
    /// </summary>
    public UserRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    /// <summary>
    /// Menjalankan fungsi AuthenticateAsync sebagai bagian dari alur file ini.
    /// </summary>
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

    /// <summary>
    /// Menjalankan fungsi UsernameExistsAsync sebagai bagian dari alur file ini.
    /// </summary>
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

    /// <summary>
    /// Menjalankan fungsi CreateUserAsync sebagai bagian dari alur file ini.
    /// </summary>
    public async Task<AuthenticatedUserDb> CreateUserAsync(
        string username,
        string password,
        string role,
        string? displayName,
        CancellationToken ct)
    {
        const string insertUserSql = """
            insert into app_users (user_id, username, password_hash, role, is_active, created_at)
            values (@userId, @username, crypt(@password, gen_salt('bf', 10)), @role, true, now())
            returning user_id, username, role, is_active
            """;

        const string upsertPlayerSql = """
            insert into players (player_id, display_name, instructor_user_id, created_at)
            values (@playerId, @displayName, @instructorUserId, @createdAt)
            on conflict (player_id) do update
            set display_name = excluded.display_name
            """;

        const string upsertUserPlayerLinkSql = """
            insert into user_player_links (link_id, user_id, player_id, created_at)
            values (@linkId, @userId, @playerId, @createdAt)
            on conflict (user_id) do nothing
            """;

        var userId = Guid.NewGuid();
        var profileDisplayName = string.IsNullOrWhiteSpace(displayName) ? username : displayName.Trim();
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        await using var tx = await conn.BeginTransactionAsync(ct);

        var created = await conn.QuerySingleAsync<AuthenticatedUserDb>(new CommandDefinition(insertUserSql, new
        {
            userId,
            username,
            password,
            role
        }, tx, cancellationToken: ct));

        var createdAt = DateTimeOffset.UtcNow;
        var canonicalPlayerId = userId;
        await conn.ExecuteAsync(new CommandDefinition(upsertPlayerSql, new
        {
            playerId = canonicalPlayerId,
            displayName = profileDisplayName,
            instructorUserId = userId,
            createdAt
        }, tx, cancellationToken: ct));

        await conn.ExecuteAsync(new CommandDefinition(upsertUserPlayerLinkSql, new
        {
            linkId = Guid.NewGuid(),
            userId,
            playerId = canonicalPlayerId,
            createdAt
        }, tx, cancellationToken: ct));

        await tx.CommitAsync(ct);
        return created;
    }

    /// <summary>
    /// Menjalankan fungsi GetLinkedPlayerIdAsync sebagai bagian dari alur file ini.
    /// </summary>
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

    /// <summary>
    /// Menjalankan fungsi GetUsernamesByPlayerIdsAsync sebagai bagian dari alur file ini.
    /// </summary>
    public async Task<Dictionary<Guid, string>> GetUsernamesByPlayerIdsAsync(IReadOnlyCollection<Guid> playerIds, CancellationToken ct)
    {
        if (playerIds.Count == 0)
        {
            return new Dictionary<Guid, string>();
        }

        var normalizedPlayerIds = playerIds
            .Where(playerId => playerId != Guid.Empty)
            .Distinct()
            .ToArray();
        if (normalizedPlayerIds.Length == 0)
        {
            return new Dictionary<Guid, string>();
        }

        const string sql = """
            select upl.player_id as PlayerId, u.username as Username
            from user_player_links upl
            join app_users u on u.user_id = upl.user_id
            where upl.player_id = any(@playerIds)
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        var rows = await conn.QueryAsync<PlayerUsernameRow>(
            new CommandDefinition(sql, new { playerIds = normalizedPlayerIds }, cancellationToken: ct));

        return rows
            .Where(row => row.PlayerId != Guid.Empty && !string.IsNullOrWhiteSpace(row.Username))
            .GroupBy(row => row.PlayerId)
            .ToDictionary(group => group.Key, group => group.First().Username.Trim());
    }

    /// <summary>
    /// Menjalankan fungsi EnsurePlayerLinkAsync sebagai bagian dari alur file ini.
    /// </summary>
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

        const string upsertPlayerSql = """
            insert into players (player_id, display_name, instructor_user_id, created_at)
            values (@playerId, @displayName, @instructorUserId, @createdAt)
            on conflict (player_id) do nothing
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

        var playerId = userId;
        var createdAt = DateTimeOffset.UtcNow;
        await conn.ExecuteAsync(new CommandDefinition(upsertPlayerSql, new
        {
            playerId,
            displayName = username,
            instructorUserId = userId,
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

    /// <summary>
    /// Menyatakan peran utama tipe PlayerUsernameRow pada modul ini.
    /// </summary>
    private sealed class PlayerUsernameRow
    {
        public Guid PlayerId { get; init; }
        public string Username { get; init; } = string.Empty;
    }
}
