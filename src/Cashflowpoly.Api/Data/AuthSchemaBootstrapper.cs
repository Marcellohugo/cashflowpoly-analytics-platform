using Dapper;
using Cashflowpoly.Api.Security;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Cashflowpoly.Api.Data;

/// <summary>
/// Menjamin tabel user autentikasi tersedia untuk deployment lama tanpa reset database.
/// </summary>
public sealed class AuthSchemaBootstrapper : IHostedService
{
    private readonly NpgsqlDataSource _dataSource;
    private readonly AuthBootstrapOptions _options;

    public AuthSchemaBootstrapper(NpgsqlDataSource dataSource, IOptions<AuthBootstrapOptions> options)
    {
        _dataSource = dataSource;
        _options = options.Value;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        const string sql = """
            create extension if not exists pgcrypto;

            create table if not exists app_users (
              user_id uuid primary key,
              username varchar(80) not null unique,
              password_hash text not null,
              role varchar(20) not null check (role in ('INSTRUCTOR','PLAYER')),
              is_active boolean not null default true,
              created_at timestamptz not null default now()
            );

            create index if not exists ix_app_users_role_active on app_users(role, is_active);

            create table if not exists players (
              player_id uuid primary key,
              display_name varchar(80) not null,
              instructor_user_id uuid null references app_users(user_id) on delete set null,
              created_at timestamptz not null default now()
            );
            create table if not exists user_player_links (
              link_id uuid primary key,
              user_id uuid not null unique references app_users(user_id) on delete cascade,
              player_id uuid not null unique references players(player_id) on delete cascade,
              created_at timestamptz not null default now()
            );

            create index if not exists ix_user_player_links_user on user_player_links(user_id);
            create index if not exists ix_user_player_links_player on user_player_links(player_id);

            alter table players add column if not exists instructor_user_id uuid null references app_users(user_id) on delete set null;
            create index if not exists ix_players_instructor_user on players(instructor_user_id, created_at desc);
            alter table if exists sessions add column if not exists instructor_user_id uuid null references app_users(user_id) on delete set null;
            do $$
            begin
              if to_regclass('public.sessions') is not null then
                execute 'create index if not exists ix_sessions_instructor_user on sessions(instructor_user_id, created_at desc)';
              end if;
            end $$;

            update players p
            set instructor_user_id = upl.user_id
            from user_player_links upl
            where p.player_id = upl.player_id
              and p.instructor_user_id is null;
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(cancellationToken);
        await conn.ExecuteAsync(new CommandDefinition(sql, cancellationToken: cancellationToken));

        if (!_options.SeedDefaultUsers)
        {
            return;
        }

        await SeedUserAsync(conn, _options.InstructorUsername, _options.InstructorPassword, "INSTRUCTOR", cancellationToken);
        await SeedUserAsync(conn, _options.PlayerUsername, _options.PlayerPassword, "PLAYER", cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

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

        if (password.Length < 12)
        {
            throw new InvalidOperationException($"AuthBootstrap password role {role} minimal 12 karakter.");
        }

        const string insertSql = """
            insert into app_users (user_id, username, password_hash, role, is_active)
            select gen_random_uuid(), @username, crypt(@password, gen_salt('bf', 10)), @role, true
            where not exists (select 1 from app_users where lower(username) = lower(@username));
            """;

        await conn.ExecuteAsync(
            new CommandDefinition(
                insertSql,
                new { username, password, role },
                cancellationToken: cancellationToken));
    }
}
