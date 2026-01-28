-- Opsional: aktifkan pgcrypto jika kamu ingin default UUID dari DB.
-- create extension if not exists pgcrypto;

create table if not exists players (
  player_id uuid primary key,
  display_name varchar(80) not null,
  created_at timestamptz not null default now()
);

create table if not exists sessions (
  session_id uuid primary key,
  session_name varchar(120) not null,
  mode varchar(10) not null check (mode in ('PEMULA','MAHIR')),
  status varchar(10) not null check (status in ('CREATED','STARTED','ENDED')),
  started_at timestamptz null,
  ended_at timestamptz null,
  created_at timestamptz not null default now()
);

create index if not exists ix_sessions_status on sessions(status);
create index if not exists ix_sessions_created_at on sessions(created_at desc);

create table if not exists rulesets (
  ruleset_id uuid primary key,
  name varchar(120) not null,
  description text null,
  is_archived boolean not null default false,
  created_at timestamptz not null default now(),
  created_by varchar(80) null
);

create index if not exists ix_rulesets_archived on rulesets(is_archived);
create index if not exists ix_rulesets_created_at on rulesets(created_at desc);

create table if not exists ruleset_versions (
  ruleset_version_id uuid primary key,
  ruleset_id uuid not null references rulesets(ruleset_id) on delete cascade,
  version int not null check (version >= 1),
  status varchar(10) not null check (status in ('DRAFT','ACTIVE','RETIRED')),
  config_json jsonb not null,
  config_hash varchar(128) not null,
  created_at timestamptz not null default now(),
  created_by varchar(80) null,
  unique(ruleset_id, version),
  unique(ruleset_id, config_hash)
);

create index if not exists ix_ruleset_versions_ruleset on ruleset_versions(ruleset_id, version desc);
create index if not exists ix_ruleset_versions_status on ruleset_versions(status);
create index if not exists ix_ruleset_versions_config_gin on ruleset_versions using gin (config_json);

create table if not exists session_players (
  session_player_id uuid primary key,
  session_id uuid not null references sessions(session_id) on delete cascade,
  player_id uuid not null references players(player_id) on delete restrict,
  join_order int not null default 0,
  role varchar(20) not null default 'PLAYER',
  created_at timestamptz not null default now(),
  unique(session_id, player_id)
);

create index if not exists ix_session_players_session on session_players(session_id, join_order);
create index if not exists ix_session_players_player on session_players(player_id);

create table if not exists session_ruleset_activations (
  activation_id uuid primary key,
  session_id uuid not null references sessions(session_id) on delete cascade,
  ruleset_version_id uuid not null references ruleset_versions(ruleset_version_id) on delete restrict,
  activated_at timestamptz not null default now(),
  activated_by varchar(80) null
);

create index if not exists ix_sra_session on session_ruleset_activations(session_id, activated_at desc);
create index if not exists ix_sra_ruleset_version on session_ruleset_activations(ruleset_version_id);

create table if not exists events (
  event_pk uuid primary key,
  event_id uuid not null,
  session_id uuid not null references sessions(session_id) on delete cascade,
  player_id uuid null references players(player_id) on delete restrict,
  actor_type varchar(10) not null check (actor_type in ('PLAYER','SYSTEM')),
  timestamp timestamptz not null,
  day_index int not null check (day_index >= 0),
  weekday varchar(3) not null check (weekday in ('MON','TUE','WED','THU','FRI','SAT','SUN')),
  turn_number int not null check (turn_number >= 0),
  sequence_number bigint not null check (sequence_number >= 0),
  action_type varchar(64) not null,
  ruleset_version_id uuid not null references ruleset_versions(ruleset_version_id) on delete restrict,
  payload jsonb not null,
  received_at timestamptz not null default now(),
  client_request_id varchar(120) null,
  unique(session_id, event_id),
  unique(session_id, sequence_number)
);

create index if not exists ix_events_session_seq on events(session_id, sequence_number);
create index if not exists ix_events_session_time on events(session_id, timestamp);
create index if not exists ix_events_session_action on events(session_id, action_type);
create index if not exists ix_events_player_time on events(player_id, timestamp);
create index if not exists ix_events_payload_gin on events using gin (payload);

create table if not exists event_cashflow_projections (
  projection_id uuid primary key,
  session_id uuid not null references sessions(session_id) on delete cascade,
  player_id uuid not null references players(player_id) on delete restrict,
  event_pk uuid not null references events(event_pk) on delete restrict,
  event_id uuid not null,
  timestamp timestamptz not null,
  direction varchar(3) not null check (direction in ('IN','OUT')),
  amount int not null check (amount > 0),
  category varchar(40) not null,
  counterparty varchar(20) null,
  reference varchar(80) null,
  note varchar(160) null,
  unique(session_id, event_id)
);

create index if not exists ix_ecp_session_time on event_cashflow_projections(session_id, timestamp desc);
create index if not exists ix_ecp_session_player_time on event_cashflow_projections(session_id, player_id, timestamp desc);
create index if not exists ix_ecp_category on event_cashflow_projections(category);

create table if not exists metric_snapshots (
  metric_snapshot_id uuid primary key,
  session_id uuid not null references sessions(session_id) on delete cascade,
  player_id uuid null references players(player_id) on delete restrict,
  computed_at timestamptz not null default now(),
  metric_name varchar(80) not null,
  metric_value_numeric double precision null,
  metric_value_json jsonb null,
  ruleset_version_id uuid not null references ruleset_versions(ruleset_version_id) on delete restrict
);

create index if not exists ix_metrics_session_name_time on metric_snapshots(session_id, metric_name, computed_at desc);
create index if not exists ix_metrics_session_player_name_time on metric_snapshots(session_id, player_id, metric_name, computed_at desc);
create index if not exists ix_metrics_session_player_time on metric_snapshots(session_id, player_id, computed_at desc);
create index if not exists ix_metrics_ruleset_version on metric_snapshots(ruleset_version_id);

create table if not exists validation_logs (
  validation_log_id uuid primary key,
  session_id uuid not null references sessions(session_id) on delete cascade,
  event_pk uuid null references events(event_pk) on delete restrict,
  event_id uuid not null,
  is_valid boolean not null,
  error_code varchar(40) null,
  error_message varchar(200) null,
  details_json jsonb null,
  created_at timestamptz not null default now(),
  unique(session_id, event_id)
);

create index if not exists ix_validation_session_time on validation_logs(session_id, created_at desc);
create index if not exists ix_validation_valid on validation_logs(is_valid);
