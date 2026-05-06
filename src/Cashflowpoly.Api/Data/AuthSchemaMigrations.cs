// Fungsi file: Menyimpan daftar migrasi SQL autentikasi yang dijalankan saat startup aplikasi.
namespace Cashflowpoly.Api.Data;

/// <summary>
/// Migrasi skema autentikasi dan kompatibilitas data untuk deployment lama.
/// </summary>
public static class AuthSchemaMigrations
{
    /// <summary>
    /// Daftar migrasi berurutan. ID tidak boleh diubah setelah migrasi dirilis.
    /// </summary>
    public static IReadOnlyList<DatabaseMigration> All { get; } =
    [
        new("202405010001_auth_core_schema", AuthCoreSchemaSql),
        new("202405010002_session_event_constraints", SessionEventConstraintSql),
        new("202405010003_player_identity_backfill", PlayerIdentityBackfillSql),
        new("202405010004_ruleset_owner_backfill", RulesetOwnerBackfillSql),
        new("202405010005_security_audit_schema", SecurityAuditSchemaSql)
    ];

    private const string AuthCoreSchemaSql = """
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
        """;

    private const string SessionEventConstraintSql = """
        do $$
        begin
          if to_regclass('public.session_players') is not null then
            with sessions_needing_fix as (
              select session_id
              from session_players
              group by session_id
              having bool_or(join_order <= 0)
                 or count(distinct join_order) <> count(*)
            ),
            ranked as (
              select sp.session_player_id,
                     row_number() over (
                       partition by sp.session_id
                       order by
                         case when sp.join_order > 0 then 0 else 1 end,
                         sp.join_order asc,
                         sp.created_at asc,
                         sp.player_id asc
                     )::int as new_join_order
              from session_players sp
              join sessions_needing_fix nf on nf.session_id = sp.session_id
            )
            update session_players sp
            set join_order = ranked.new_join_order
            from ranked
            where sp.session_player_id = ranked.session_player_id;

            update session_players
            set role = 'PLAYER'
            where coalesce(upper(role), '') <> 'PLAYER';

            alter table session_players
            alter column join_order set default 1;

            if not exists (
              select 1
              from pg_constraint
              where conname = 'ck_session_players_join_order_positive'
                and conrelid = 'public.session_players'::regclass
            ) then
              alter table session_players
              add constraint ck_session_players_join_order_positive
              check (join_order >= 1)
              not valid;
            end if;

            if not exists (
              select 1
              from pg_constraint
              where conname = 'ck_session_players_role'
                and conrelid = 'public.session_players'::regclass
            ) then
              alter table session_players
              add constraint ck_session_players_role
              check (role in ('PLAYER'))
              not valid;
            end if;

            if exists (
              select 1
              from pg_constraint
              where conname = 'ck_session_players_join_order_positive'
                and conrelid = 'public.session_players'::regclass
            ) then
              alter table session_players
              validate constraint ck_session_players_join_order_positive;
            end if;

            if exists (
              select 1
              from pg_constraint
              where conname = 'ck_session_players_role'
                and conrelid = 'public.session_players'::regclass
            ) then
              alter table session_players
              validate constraint ck_session_players_role;
            end if;
          end if;
        end $$;

        do $$
        begin
          if to_regclass('public.events') is not null then
            update events
            set turn_number = 1
            where turn_number <= 0;

            if not exists (
              select 1
              from pg_constraint
              where conname = 'ck_events_turn_number_positive'
                and conrelid = 'public.events'::regclass
            ) then
              alter table events
              add constraint ck_events_turn_number_positive
              check (turn_number >= 1)
              not valid;
            end if;

            if exists (
              select 1
              from pg_constraint
              where conname = 'ck_events_turn_number_positive'
                and conrelid = 'public.events'::regclass
            ) then
              alter table events
              validate constraint ck_events_turn_number_positive;
            end if;
          end if;
        end $$;
        """;

    private const string PlayerIdentityBackfillSql = """
        delete from user_player_links upl
        using app_users u
        where upl.user_id = u.user_id
          and upper(u.role) <> 'PLAYER';

        update players p
        set instructor_user_id = null
        from app_users u
        where p.player_id = u.user_id
          and upper(u.role) <> 'PLAYER'
          and p.instructor_user_id = u.user_id;

        update players p
        set instructor_user_id = upl.user_id
        from user_player_links upl
        join app_users u on u.user_id = upl.user_id
        where p.player_id = upl.player_id
          and upper(u.role) = 'PLAYER'
          and p.instructor_user_id is null;

        insert into players (player_id, display_name, instructor_user_id, created_at)
        select u.user_id, u.username, u.user_id, u.created_at
        from app_users u
        where upper(u.role) = 'PLAYER'
          and not exists (
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
        where upper(u.role) = 'PLAYER'
          and not exists (
            select 1
            from user_player_links upl
            where upl.user_id = u.user_id
        )
          and exists (
            select 1
            from players p
            where p.player_id = u.user_id
        );
        """;

    private const string RulesetOwnerBackfillSql = """
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
        """;

    private const string SecurityAuditSchemaSql = """
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
}
