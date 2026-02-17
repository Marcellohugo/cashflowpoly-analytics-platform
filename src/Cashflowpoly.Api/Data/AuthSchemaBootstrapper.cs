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
            alter table if exists rulesets add column if not exists instructor_user_id uuid null references app_users(user_id) on delete set null;
            do $$
            begin
              if to_regclass('public.sessions') is not null then
                execute 'create index if not exists ix_sessions_instructor_user on sessions(instructor_user_id, created_at desc)';
              end if;
            end $$;
            do $$
            begin
              if to_regclass('public.rulesets') is not null then
                execute 'create index if not exists ix_rulesets_instructor_user on rulesets(instructor_user_id, created_at desc)';
              end if;
            end $$;

            do $$
            begin
              if to_regclass('public.session_players') is not null then
                with ranked as (
                  select session_player_id,
                         row_number() over (partition by session_id order by player_id asc)::int as new_join_order
                  from session_players
                )
                update session_players sp
                set join_order = ranked.new_join_order
                from ranked
                where sp.session_player_id = ranked.session_player_id;
              end if;
            end $$;

            update players p
            set instructor_user_id = upl.user_id
            from user_player_links upl
            where p.player_id = upl.player_id
              and p.instructor_user_id is null;

            insert into players (player_id, display_name, instructor_user_id, created_at)
            select u.user_id, u.username, u.user_id, u.created_at
            from app_users u
            where not exists (
                select 1
                from user_player_links upl
                where upl.user_id = u.user_id
            )
              and not exists (
                select 1
                from players p
                where p.player_id = u.user_id
            );

            insert into user_player_links (link_id, user_id, player_id, created_at)
            select gen_random_uuid(), u.user_id, u.user_id, now()
            from app_users u
            where not exists (
                select 1
                from user_player_links upl
                where upl.user_id = u.user_id
            )
              and exists (
                select 1
                from players p
                where p.player_id = u.user_id
            );

            do $$
            begin
              if to_regclass('public.rulesets') is not null then
                update rulesets r
                set instructor_user_id = u.user_id
                from app_users u
                where r.instructor_user_id is null
                  and r.created_by is not null
                  and lower(u.username) = lower(r.created_by);
              end if;
            end $$;

            do $$
            begin
              if to_regclass('public.rulesets') is not null
                 and to_regclass('public.ruleset_versions') is not null
                 and to_regclass('public.session_ruleset_activations') is not null
                 and to_regclass('public.sessions') is not null then
                update rulesets r
                set instructor_user_id = src.instructor_user_id
                from (
                  select rv.ruleset_id, min(s.instructor_user_id::text)::uuid as instructor_user_id
                  from ruleset_versions rv
                  join session_ruleset_activations sra on sra.ruleset_version_id = rv.ruleset_version_id
                  join sessions s on s.session_id = sra.session_id
                  where s.instructor_user_id is not null
                  group by rv.ruleset_id
                  having count(distinct s.instructor_user_id) = 1
                ) src
                where r.ruleset_id = src.ruleset_id
                  and r.instructor_user_id is null;
              end if;
            end $$;

            create table if not exists security_audit_logs (
              security_audit_log_id uuid primary key,
              occurred_at timestamptz not null default now(),
              trace_id varchar(64) not null,
              event_type varchar(80) not null,
              outcome varchar(20) not null check (outcome in ('SUCCESS','FAILURE','DENIED')),
              user_id uuid null references app_users(user_id) on delete set null,
              username varchar(80) null,
              role varchar(20) null,
              ip_address varchar(64) null,
              user_agent varchar(300) null,
              method varchar(16) not null,
              path varchar(240) not null,
              status_code int not null check (status_code >= 100 and status_code <= 599),
              detail_json jsonb null
            );
            create index if not exists ix_security_audit_logs_occurred on security_audit_logs(occurred_at desc);
            create index if not exists ix_security_audit_logs_event on security_audit_logs(event_type, occurred_at desc);
            create index if not exists ix_security_audit_logs_user on security_audit_logs(user_id, occurred_at desc);
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

        await conn.ExecuteAsync(
            new CommandDefinition(
                ensureProfileSql,
                new { username },
                cancellationToken: cancellationToken));
    }
}
