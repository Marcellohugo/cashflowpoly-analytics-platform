-- Fungsi file: Menambahkan riwayat revisi setup, penanda akun demo, dan indeks cursor.
create table if not exists session_setup_revisions (
  session_id uuid not null,
  revision integer not null,
  ruleset_version_id uuid not null,
  client_request_id varchar(120) not null,
  setup_json jsonb not null,
  saved_at timestamptz not null default now(),
  locked_at timestamptz null,
  created_by_user_id uuid not null,
  constraint pk_session_setup_revisions primary key (session_id, revision),
  constraint uq_session_setup_revisions_request unique (created_by_user_id, client_request_id),
  constraint ck_session_setup_revisions_revision check (revision > 0),
  constraint ck_session_setup_revisions_request check (nullif(btrim(client_request_id), '') is not null),
  constraint fk_session_setup_revisions_session foreign key (session_id) references sessions (session_id) on delete restrict,
  constraint fk_session_setup_revisions_ruleset_version foreign key (ruleset_version_id) references ruleset_versions (ruleset_version_id) on delete restrict,
  constraint fk_session_setup_revisions_created_by foreign key (created_by_user_id) references app_users (user_id) on delete restrict
);

do $$
begin
  if to_regclass('public.session_setups') is not null then
    if exists (
      select 1
      from information_schema.columns
      where table_schema = 'public'
        and table_name = 'session_setups'
        and column_name = 'setup_json'
    ) then
      execute $migration$
      insert into session_setup_revisions (
        session_id,
        revision,
        ruleset_version_id,
        client_request_id,
        setup_json,
        saved_at,
        locked_at,
        created_by_user_id
      )
      select
        session_id,
        1,
        ruleset_version_id,
        client_request_id,
        setup_json,
        locked_at,
        locked_at,
        created_by_user_id
      from session_setups
      on conflict do nothing
      $migration$;
    elsif exists (
      select 1
      from information_schema.columns
      where table_schema = 'public'
        and table_name = 'session_setups'
        and column_name = 'setup_payload'
    ) then
      execute $migration$
        insert into session_setup_revisions (
          session_id,
          revision,
          ruleset_version_id,
          client_request_id,
          setup_json,
          saved_at,
          locked_at,
          created_by_user_id
        )
        select
          session_id,
          1,
          ruleset_version_id,
          client_request_id,
          setup_payload,
          locked_at,
          locked_at,
          created_by_user_id
        from session_setups
        on conflict do nothing
      $migration$;
    end if;
  end if;
end
$$;

alter table app_users
  add column if not exists is_demo boolean not null default false;

update app_users
set is_demo = true
where lower(username::text) in ('rina.kartika', 'marco', 'marcello', 'hugo', 'manalu');

create index if not exists ix_session_setup_revisions_latest
  on session_setup_revisions (session_id, revision desc);

create index if not exists ix_events_session_received_cursor
  on events (session_id, received_at, event_pk);
