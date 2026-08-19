// Fungsi file: Mengelola pemetaan dan akses PostgreSQL untuk UserRepository.
using Dapper;
using Npgsql;

namespace Cashflowpoly.Api.Data;

/// <summary>
/// Record data user yang berhasil diautentikasi: ID, username, display name, role, dan status aktif.
/// </summary>
public sealed record AuthenticatedUserDb(Guid UserId, string Username, string DisplayName, string Role, bool IsActive);

/// <summary>
/// Repository untuk autentikasi user aplikasi dan identitas player berbasis app_users.
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
            select user_id, username, display_name, role, is_active
            from app_users
            where username = @username
              and is_active = true
              and password_hash = crypt(@password, password_hash)
            limit 1
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        return await conn.QueryFirstOrDefaultAsync<AuthenticatedUserDb>(
            new CommandDefinition(sql, new { username, password }, cancellationToken: ct));
    }

    public Task<AuthenticatedUserDb> CreatePlayerUserAsync(
        string username,
        string password,
        string displayName,
        CancellationToken ct)
    {
        return CreateUserAsync(username, password, "PLAYER", displayName, ct);
    }

    public async Task<AuthenticatedUserDb> CreateUserAsync(
        string username,
        string password,
        string role,
        string? displayName,
        CancellationToken ct)
    {
        const string sql = """
            insert into app_users (user_id, username, display_name, password_hash, role, is_active, created_at)
            values (@userId, @username, @displayName, crypt(@password, gen_salt('bf', 10)), @role, true, now())
            returning user_id, username, display_name, role, is_active
            """;

        var userId = Guid.NewGuid();
        var resolvedDisplayName = string.IsNullOrWhiteSpace(displayName) ? username : displayName.Trim();

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        return await conn.QuerySingleAsync<AuthenticatedUserDb>(new CommandDefinition(sql, new
        {
            userId,
            username,
            displayName = resolvedDisplayName,
            password,
            role
        }, cancellationToken: ct));
    }

    public async Task<Guid?> GetPlayerUserIdAsync(Guid userId, CancellationToken ct)
    {
        const string sql = """
            select user_id
            from app_users
            where user_id = @userId
              and role = 'PLAYER'
              and is_active = true
            limit 1
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        return await conn.QuerySingleOrDefaultAsync<Guid?>(
            new CommandDefinition(sql, new { userId }, cancellationToken: ct));
    }

    public async Task<Dictionary<Guid, string>> GetUsernamesByUserIdsAsync(IReadOnlyCollection<Guid> userIds, CancellationToken ct)
    {
        if (userIds.Count == 0)
        {
            return new Dictionary<Guid, string>();
        }

        var normalizedUserIds = userIds
            .Where(userId => userId != Guid.Empty)
            .Distinct()
            .ToArray();
        if (normalizedUserIds.Length == 0)
        {
            return new Dictionary<Guid, string>();
        }

        const string sql = """
            select user_id as UserId, username as Username
            from app_users
            where user_id = any(@userIds)
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        var rows = await conn.QueryAsync<UserNameRow>(
            new CommandDefinition(sql, new { userIds = normalizedUserIds }, cancellationToken: ct));

        return rows
            .Where(row => row.UserId != Guid.Empty && !string.IsNullOrWhiteSpace(row.Username))
            .GroupBy(row => row.UserId)
            .ToDictionary(group => group.Key, group => group.First().Username.Trim());
    }

    private sealed class UserNameRow
    {
        public Guid UserId { get; init; }
        public string Username { get; init; } = string.Empty;
    }
}
