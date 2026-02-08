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
        const string sql = """
            insert into app_users (user_id, username, password_hash, role, is_active, created_at)
            values (@userId, @username, crypt(@password, gen_salt('bf', 10)), @role, true, now())
            returning user_id, username, role, is_active
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        return await conn.QuerySingleAsync<AuthenticatedUserDb>(new CommandDefinition(sql, new
        {
            userId = Guid.NewGuid(),
            username,
            password,
            role
        }, cancellationToken: ct));
    }
}
