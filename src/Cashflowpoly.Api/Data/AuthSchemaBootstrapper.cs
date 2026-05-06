// Fungsi file: HostedService bootstrap autentikasi melalui migrasi skema dan seed akun default.
using Dapper;
using Cashflowpoly.Api.Security;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Cashflowpoly.Api.Data;

/// <summary>
/// Menjamin migrasi autentikasi tersedia untuk deployment lama tanpa reset database.
/// </summary>
public sealed class AuthSchemaBootstrapper : IHostedService
{
    private readonly NpgsqlDataSource _dataSource;
    private readonly AuthBootstrapOptions _options;

    /// <summary>
    /// Menerima NpgsqlDataSource dan opsi bootstrap untuk menjalankan migrasi dan seed user.
    /// </summary>
    public AuthSchemaBootstrapper(NpgsqlDataSource dataSource, IOptions<AuthBootstrapOptions> options)
    {
        _dataSource = dataSource;
        _options = options.Value;
    }

    /// <summary>
    /// Menerapkan migrasi autentikasi lalu menyemai akun default saat diaktifkan.
    /// </summary>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var conn = await _dataSource.OpenConnectionAsync(cancellationToken);
        await AuthSchemaMigrator.ApplyAsync(conn, AuthSchemaMigrations.All, cancellationToken);

        if (!_options.SeedDefaultUsers)
        {
            return;
        }

        await SeedUserAsync(conn, _options.InstructorUsername, _options.InstructorPassword, "INSTRUCTOR", cancellationToken);
        await SeedUserAsync(conn, _options.PlayerUsername, _options.PlayerPassword, "PLAYER", cancellationToken);
    }

    /// <summary>
    /// Tidak ada tugas cleanup yang dibutuhkan saat shutdown.
    /// </summary>
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    /// <summary>
    /// Menyisipkan satu akun default (jika belum ada) dan memastikan profil/link hanya untuk role PLAYER.
    /// </summary>
    private static async Task SeedUserAsync(
        NpgsqlConnection conn,
        string? username,
        string? password,
        string role,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(username) && string.IsNullOrWhiteSpace(password))
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException($"AuthBootstrap untuk role {role} harus mengisi username dan password.");
        }

        if (password.Length < PasswordPolicy.MinPasswordLength)
        {
            throw new InvalidOperationException(
                $"AuthBootstrap password role {role} minimal {PasswordPolicy.MinPasswordLength} karakter.");
        }

        const string insertSql = """
            insert into app_users (user_id, username, password_hash, role, is_active)
            select gen_random_uuid(), @username, crypt(@password, gen_salt('bf', 10)), @role, true
            where not exists (select 1 from app_users where lower(username) = lower(@username));
            """;

        const string ensureProfileSql = """
            insert into players (player_id, display_name, instructor_user_id, created_at)
            select u.user_id, u.username, u.user_id, now()
            from app_users u
            where lower(u.username) = lower(@username)
              and not exists (
                  select 1
                  from players p
                  where p.player_id = u.user_id
              );

            insert into user_player_links (link_id, user_id, player_id, created_at)
            select gen_random_uuid(), u.user_id, u.user_id, now()
            from app_users u
            where lower(u.username) = lower(@username)
              and not exists (
                  select 1
                  from user_player_links upl
                  where upl.user_id = u.user_id
              );
            """;

        await conn.ExecuteAsync(
            new CommandDefinition(
                insertSql,
                new { username, password, role },
                cancellationToken: cancellationToken));

        if (string.Equals(role, "PLAYER", StringComparison.OrdinalIgnoreCase))
        {
            await conn.ExecuteAsync(
                new CommandDefinition(
                    ensureProfileSql,
                    new { username },
                    cancellationToken: cancellationToken));
        }
    }
}
