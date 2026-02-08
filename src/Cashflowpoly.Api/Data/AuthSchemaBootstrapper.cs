using Dapper;
using Microsoft.Extensions.Hosting;
using Npgsql;

namespace Cashflowpoly.Api.Data;

/// <summary>
/// Menjamin tabel user autentikasi tersedia untuk deployment lama tanpa reset database.
/// </summary>
public sealed class AuthSchemaBootstrapper : IHostedService
{
    private readonly NpgsqlDataSource _dataSource;

    public AuthSchemaBootstrapper(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
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

            insert into app_users (user_id, username, password_hash, role, is_active)
            select gen_random_uuid(), 'instructor', crypt('instructor123', gen_salt('bf', 10)), 'INSTRUCTOR', true
            where not exists (select 1 from app_users where username = 'instructor');

            insert into app_users (user_id, username, password_hash, role, is_active)
            select gen_random_uuid(), 'player', crypt('player123', gen_salt('bf', 10)), 'PLAYER', true
            where not exists (select 1 from app_users where username = 'player');
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(cancellationToken);
        await conn.ExecuteAsync(new CommandDefinition(sql, cancellationToken: cancellationToken));
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
