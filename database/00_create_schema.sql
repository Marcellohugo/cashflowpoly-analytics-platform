create extension if not exists pgcrypto;

drop table if exists user_player_links cascade;
drop table if exists players cascade;

-- ============================================================
-- 1. USER, PLAYER, ROLE, DAN MENU
-- ============================================================

create table if not exists app_users (
  user_id uuid primary key default gen_random_uuid(),
  username varchar(80) not null unique,
  display_name varchar(80) not null,
  password_hash text not null,
  role varchar(20) not null check (role in ('INSTRUCTOR','PLAYER')),
  is_active boolean not null default true,
  created_at timestamptz not null default now()
);

create index if not exists ix_app_users_role_active
on app_users(role, is_active);

create table if not exists app_menus (
  menu_id uuid primary key default gen_random_uuid(),
  menu_code varchar(80) not null unique,
  menu_name varchar(120) not null,
  path varchar(160) not null,
  parent_menu_id uuid null references app_menus(menu_id) on delete cascade,
  sort_order int not null default 1 check (sort_order >= 1),
  is_active boolean not null default true
);

create table if not exists role_menu_permissions (
  role varchar(20) not null check (role in ('INSTRUCTOR','PLAYER')),
  menu_id uuid not null references app_menus(menu_id) on delete cascade,
  can_view boolean not null default true,
  can_create boolean not null default false,
  can_update boolean not null default false,
  can_delete boolean not null default false,
  primary key(role, menu_id)
);

-- ============================================================
-- 2. RULESET, MASTER, DAN KATALOG
-- ============================================================

create table if not exists rulesets (
  ruleset_id uuid primary key default gen_random_uuid(),
  name varchar(120) not null,
  description text null,
  instructor_user_id uuid null references app_users(user_id) on delete set null,
  created_at timestamptz not null default now(),
  created_by varchar(80) null
);

create index if not exists ix_rulesets_created_at
on rulesets(created_at desc);

create index if not exists ix_rulesets_instructor_user
on rulesets(instructor_user_id, created_at desc);

create table if not exists ruleset_versions (
  ruleset_version_id uuid primary key default gen_random_uuid(),
  ruleset_id uuid not null references rulesets(ruleset_id) on delete cascade,
  version int not null check (version >= 1),
  status varchar(10) not null check (status in ('DRAFT','ACTIVE','RETIRED')),
  mode varchar(10) not null check (mode in ('PEMULA','MAHIR')),
  config_json jsonb not null,
  schema_version varchar(20) not null default '2.0.0',
  config_hash varchar(128) not null,
  change_note text null,
  published_at timestamptz null,
  created_at timestamptz not null default now(),
  created_by varchar(80) null,
  unique(ruleset_id, version),
  unique(ruleset_id, config_hash)
);

create index if not exists ix_ruleset_versions_ruleset
on ruleset_versions(ruleset_id, version desc);

create index if not exists ix_ruleset_versions_status
on ruleset_versions(status);

create index if not exists ix_ruleset_versions_config_gin
on ruleset_versions using gin(config_json);

create table if not exists actions (
  action_id varchar(80) primary key,
  action_name varchar(120) not null,
  mode varchar(10) not null default 'BOTH' check (mode in ('PEMULA','MAHIR','BOTH')),
  cashflow_direction varchar(3) null check (cashflow_direction in ('IN','OUT')),
  affects_coin boolean not null default false,
  affects_happiness boolean not null default false,
  affects_saving boolean not null default false,
  affects_inventory boolean not null default false,
  affects_quest boolean not null default false,
  is_active boolean not null default true,
  created_at timestamptz not null default now()
);

create table if not exists ingredients (
  ingredient_id varchar(80) primary key,
  ingredient_name varchar(120) not null unique,
  display_name varchar(120) not null,
  is_active boolean not null default true,
  created_at timestamptz not null default now()
);

create table if not exists game_components (
  component_id uuid primary key default gen_random_uuid(),
  component_code varchar(120) not null unique,
  component_name varchar(160) not null,
  component_type varchar(40) not null check (
    component_type in ('BOARD','TOKEN','COIN','CARD','SHEET','TABLE_ITEM')
  ),
  mode varchar(10) not null check (mode in ('PEMULA','MAHIR','BOTH')),
  quantity int not null default 0 check (quantity >= 0),
  metadata_json jsonb not null default '{}'::jsonb,
  is_active boolean not null default true,
  created_at timestamptz not null default now()
);

create table if not exists ruleset_catalog_items (
  ruleset_catalog_item_id uuid primary key default gen_random_uuid(),
  ruleset_version_id uuid not null references ruleset_versions(ruleset_version_id) on delete cascade,
  item_type varchar(40) not null check (
    item_type in (
      'INGREDIENT',
      'ORDER',
      'NEED',
      'COLLECTION_MISSION',
      'FINANCIAL_GOAL',
      'GOLD_PRICE',
      'GOLD',
      'DONATION_RANK',
      'PENSION_RANK',
      'TIE_BREAKER',
      'SHARIA_LOAN',
      'INSURANCE',
      'LIFE_RISK',
      'NARRATIVE',
      'QUEST'
    )
  ),
  item_code varchar(120) not null,
  item_name varchar(160) not null,
  sort_order int not null default 1 check (sort_order >= 1),
  card_qty int null check (card_qty >= 0),
  is_active boolean not null default true,
  payload_json jsonb not null default '{}'::jsonb,
  created_at timestamptz not null default now(),
  unique(ruleset_version_id, item_type, item_code)
);

create index if not exists ix_ruleset_catalog_items_ruleset_type
on ruleset_catalog_items(ruleset_version_id, item_type, sort_order, item_code);

create index if not exists ix_ruleset_catalog_items_payload_gin
on ruleset_catalog_items using gin(payload_json);

create table if not exists ruleset_catalog_item_requirements (
  requirement_id uuid primary key default gen_random_uuid(),
  ruleset_catalog_item_id uuid not null references ruleset_catalog_items(ruleset_catalog_item_id) on delete cascade,
  requirement_order int not null default 1 check (requirement_order >= 1),
  requirement_type varchar(40) not null check (
    requirement_type in ('INGREDIENT','MISSION_RULE')
  ),
  requirement_value varchar(120) not null,
  qty_required int null check (qty_required >= 0),
  payload_json jsonb not null default '{}'::jsonb,
  unique(ruleset_catalog_item_id, requirement_order, requirement_type, requirement_value)
);

create index if not exists ix_ruleset_catalog_item_requirements_item
on ruleset_catalog_item_requirements(ruleset_catalog_item_id, requirement_order);

-- ============================================================
-- 3. SESSION DAN STATE PERMAINAN
-- ============================================================

create table if not exists sessions (
  session_id uuid primary key default gen_random_uuid(),
  session_name varchar(120) not null,
  mode varchar(10) not null check (mode in ('PEMULA','MAHIR')),
  status varchar(10) not null check (status in ('CREATED','STARTED','ENDED')),
  player_count int null check (player_count between 2 and 4),
  started_at timestamptz null,
  ended_at timestamptz null,
  instructor_user_id uuid null references app_users(user_id) on delete set null,
  created_at timestamptz not null default now()
);

create index if not exists ix_sessions_status
on sessions(status);

create index if not exists ix_sessions_created_at
on sessions(created_at desc);

create index if not exists ix_sessions_instructor_user
on sessions(instructor_user_id, created_at desc);

create table if not exists session_ruleset_activations (
  activation_id uuid primary key default gen_random_uuid(),
  session_id uuid not null references sessions(session_id) on delete cascade,
  ruleset_version_id uuid not null references ruleset_versions(ruleset_version_id) on delete restrict,
  activated_at timestamptz not null default now(),
  activated_by varchar(80) null
);

create index if not exists ix_sra_session
on session_ruleset_activations(session_id, activated_at desc);

create index if not exists ix_sra_ruleset_version
on session_ruleset_activations(ruleset_version_id);

create table if not exists session_players (
  session_player_id uuid primary key default gen_random_uuid(),
  session_id uuid not null references sessions(session_id) on delete cascade,
  user_id uuid not null references app_users(user_id) on delete restrict,
  player_index int null check (player_index between 1 and 4),
  join_order int not null default 1 check (join_order >= 1),
  role varchar(20) not null default 'PLAYER' check (role in ('PLAYER')),
  created_at timestamptz not null default now(),
  unique(session_id, user_id)
);

create unique index if not exists uq_session_players_session_player_index
on session_players(session_id, player_index)
where player_index is not null;

create index if not exists ix_session_players_session
on session_players(session_id, join_order);

create index if not exists ix_session_players_user
on session_players(user_id);

create table if not exists session_states (
  session_id uuid primary key references sessions(session_id) on delete cascade,
  day int not null check (day >= 1),
  weekday varchar(3) not null check (weekday in ('MON','TUE','WED','THU','FRI','SAT','SUN')),
  turn_number int not null check (turn_number >= 1),
  current_session_player_id uuid null references session_players(session_player_id),
  current_action_index int not null default 1 check (current_action_index >= 1),
  moves_left int not null check (moves_left >= 0),
  finish_day int not null check (finish_day >= 1),
  phase varchar(30) not null default 'PLAYER_TURN' check (
    phase in ('SETUP','PLAYER_TURN','DONATION_DAY','GOLD_INVESTMENT_DAY','DAY_END','GAME_END')
  ),
  is_game_over boolean not null default false,
  state_version bigint not null default 1 check (state_version >= 1),
  ui_state_json jsonb not null default '{}'::jsonb,
  created_at timestamptz not null default now(),
  updated_at timestamptz not null default now()
);

create table if not exists session_player_states (
  session_player_id uuid primary key references session_players(session_player_id) on delete cascade,
  coins int not null check (coins >= 0),
  happiness int not null,
  saving int not null check (saving >= 0),
  created_at timestamptz not null default now(),
  updated_at timestamptz not null default now()
);

create table if not exists session_player_ingredients (
  session_player_id uuid not null references session_players(session_player_id) on delete cascade,
  ingredient_id varchar(80) not null references ingredients(ingredient_id) on delete restrict,
  qty int not null check (qty >= 0),
  updated_at timestamptz not null default now(),
  primary key (session_player_id, ingredient_id)
);

create table if not exists session_player_needs (
  session_player_need_id uuid primary key default gen_random_uuid(),
  session_player_id uuid not null references session_players(session_player_id) on delete cascade,
  ruleset_catalog_item_id uuid not null references ruleset_catalog_items(ruleset_catalog_item_id) on delete restrict,
  sort_order int not null check (sort_order >= 1),
  paid_amount int not null check (paid_amount >= 0),
  happiness_delta int not null,
  purchased_at_day int not null check (purchased_at_day >= 1),
  created_at timestamptz not null default now()
);

create index if not exists ix_session_player_needs_player
on session_player_needs(session_player_id, sort_order);

create table if not exists session_player_financial_goals (
  session_player_id uuid not null references session_players(session_player_id) on delete cascade,
  ruleset_catalog_item_id uuid not null references ruleset_catalog_items(ruleset_catalog_item_id) on delete restrict,
  purchased_at_day int not null check (purchased_at_day >= 1),
  created_at timestamptz not null default now(),
  primary key (session_player_id, ruleset_catalog_item_id)
);

create table if not exists session_player_collection_missions (
  session_player_id uuid not null references session_players(session_player_id) on delete cascade,
  ruleset_catalog_item_id uuid not null references ruleset_catalog_items(ruleset_catalog_item_id) on delete restrict,
  is_completed boolean not null default false,
  is_failed boolean not null default false,
  reward_applied boolean not null default false,
  assigned_at timestamptz not null default now(),
  primary key (session_player_id, ruleset_catalog_item_id)
);

create table if not exists session_player_quest_progress (
  session_player_id uuid not null references session_players(session_player_id) on delete cascade,
  quest_id varchar(120) not null,
  progress int not null check (progress >= 0),
  target int not null check (target >= 0),
  is_completed boolean not null default false,
  is_reward_claimed boolean not null default false,
  primary key (session_player_id, quest_id)
);

create table if not exists session_player_action_counters (
  session_player_id uuid not null references session_players(session_player_id) on delete cascade,
  action_id varchar(80) not null references actions(action_id) on delete restrict,
  count int not null check (count >= 0),
  primary key (session_player_id, action_id)
);

create table if not exists session_player_assets (
  session_player_asset_id uuid primary key default gen_random_uuid(),
  session_player_id uuid not null references session_players(session_player_id) on delete cascade,
  asset_type varchar(20) not null check (asset_type in ('GOLD','LOAN','INSURANCE')),
  asset_code varchar(120) not null,
  quantity int null check (quantity >= 0),
  amount int null,
  metadata_json jsonb not null default '{}'::jsonb,
  created_at timestamptz not null default now(),
  updated_at timestamptz not null default now(),
  unique(session_player_id, asset_type, asset_code)
);

create index if not exists ix_session_player_assets_player
on session_player_assets(session_player_id, asset_type);

create table if not exists session_player_peduli_donasi (
  session_player_id uuid primary key references session_players(session_player_id) on delete cascade,
  total_donasi int not null default 0 check (total_donasi >= 0)
);

create table if not exists session_donation_events (
  donation_event_id uuid primary key default gen_random_uuid(),
  session_id uuid not null references sessions(session_id) on delete cascade,
  event_ke int not null check (event_ke >= 1),
  day int not null check (day >= 1),
  rankings_json jsonb not null default '[]'::jsonb,
  created_at timestamptz not null default now(),
  unique(session_id, event_ke)
);

create index if not exists ix_session_donation_events_session
on session_donation_events(session_id, event_ke);

create table if not exists session_card_positions (
  card_position_id uuid primary key default gen_random_uuid(),
  session_id uuid not null references sessions(session_id) on delete cascade,
  session_player_id uuid null references session_players(session_player_id) on delete cascade,
  card_type varchar(40) not null,
  card_ref_id varchar(120) not null,
  position_zone varchar(80) null,
  position_order int null,
  slot_code varchar(80) null,
  slot_group varchar(80) null,
  created_at timestamptz not null default now(),
  updated_at timestamptz not null default now()
);

create index if not exists ix_session_card_positions_session
on session_card_positions(session_id, session_player_id, card_type, position_order);

-- ============================================================
-- 4. EVENT, PROYEKSI, DAN ANALITIK
-- ============================================================

create table if not exists events (
  event_pk uuid primary key default gen_random_uuid(),
  event_id uuid not null,
  session_id uuid not null references sessions(session_id) on delete cascade,
  session_player_id uuid null references session_players(session_player_id) on delete set null,
  user_id uuid null references app_users(user_id) on delete restrict,
  actor_type varchar(10) not null check (actor_type in ('PLAYER','SYSTEM')),
  timestamp timestamptz not null,
  day_index int not null check (day_index >= 0),
  weekday varchar(3) not null check (weekday in ('MON','TUE','WED','THU','FRI','SAT','SUN')),
  turn_number int not null check (turn_number >= 1),
  sequence_number bigint not null check (sequence_number >= 0),
  action_id varchar(80) null references actions(action_id) on delete restrict,
  action_type varchar(80) not null,
  ruleset_version_id uuid not null references ruleset_versions(ruleset_version_id) on delete restrict,
  payload jsonb not null default '{}'::jsonb,
  received_at timestamptz not null default now(),
  client_request_id varchar(120) null,
  unique(session_id, event_id),
  unique(session_id, sequence_number)
);

create index if not exists ix_events_session_seq
on events(session_id, sequence_number);

create index if not exists ix_events_session_time
on events(session_id, timestamp);

create index if not exists ix_events_session_action
on events(session_id, action_type);

create index if not exists ix_events_user_time
on events(user_id, timestamp);

create index if not exists ix_events_payload_gin
on events using gin(payload);

create table if not exists event_cashflow_projections (
  projection_id uuid primary key default gen_random_uuid(),
  session_id uuid not null references sessions(session_id) on delete cascade,
  user_id uuid not null references app_users(user_id) on delete restrict,
  event_pk uuid not null references events(event_pk) on delete restrict,
  event_id uuid not null,
  timestamp timestamptz not null,
  direction varchar(3) not null check (direction in ('IN','OUT')),
  amount int not null check (amount > 0),
  category varchar(40) not null,
  counterparty varchar(40) null,
  reference varchar(120) null,
  note varchar(200) null,
  unique(session_id, event_id)
);

create index if not exists ix_ecp_session_time
on event_cashflow_projections(session_id, timestamp desc);

create index if not exists ix_ecp_session_user_time
on event_cashflow_projections(session_id, user_id, timestamp desc);

create index if not exists ix_ecp_category
on event_cashflow_projections(category);

create table if not exists metric_snapshots (
  metric_snapshot_id uuid primary key default gen_random_uuid(),
  session_id uuid not null references sessions(session_id) on delete cascade,
  user_id uuid null references app_users(user_id) on delete restrict,
  session_player_id uuid null references session_players(session_player_id) on delete cascade,
  computed_at timestamptz not null default now(),
  metric_name varchar(120) not null,
  metric_value_numeric double precision null,
  metric_value_text text null,
  metric_value_boolean boolean null,
  metric_payload_json jsonb null,
  ruleset_version_id uuid not null references ruleset_versions(ruleset_version_id) on delete restrict
);

create index if not exists ix_metrics_session_name_time
on metric_snapshots(session_id, metric_name, computed_at desc);

create index if not exists ix_metrics_session_user_name_time
on metric_snapshots(session_id, user_id, metric_name, computed_at desc);

create index if not exists ix_metrics_ruleset_version
on metric_snapshots(ruleset_version_id);

-- ============================================================
-- 5. VALIDASI, ACTION LOG, DAN AUDIT
-- ============================================================

create table if not exists validation_logs (
  validation_log_id uuid primary key default gen_random_uuid(),
  session_id uuid not null references sessions(session_id) on delete cascade,
  event_pk uuid null references events(event_pk) on delete restrict,
  event_id uuid not null,
  is_valid boolean not null,
  error_code varchar(40) null,
  error_message varchar(240) null,
  details_json jsonb null,
  created_at timestamptz not null default now(),
  unique(session_id, event_id)
);

create index if not exists ix_validation_session_time
on validation_logs(session_id, created_at desc);

create index if not exists ix_validation_valid
on validation_logs(is_valid);

create table if not exists session_action_logs (
  action_log_id uuid primary key default gen_random_uuid(),
  session_id uuid not null references sessions(session_id) on delete cascade,
  session_player_id uuid null references session_players(session_player_id) on delete set null,
  state_version bigint not null check (state_version >= 1),
  action_id varchar(80) null references actions(action_id) on delete restrict,
  action_json jsonb not null default '{}'::jsonb,
  created_at timestamptz not null default now()
);

create index if not exists ix_session_action_logs_session
on session_action_logs(session_id, state_version desc, created_at desc);

create table if not exists security_audit_logs (
  security_audit_log_id uuid primary key default gen_random_uuid(),
  occurred_at timestamptz not null default now(),
  trace_id varchar(64) not null,
  event_type varchar(80) not null,
  outcome varchar(40) not null,
  user_id uuid null references app_users(user_id) on delete set null,
  username varchar(80) null,
  role varchar(20) null,
  ip_address varchar(80) null,
  user_agent varchar(300) null,
  method varchar(16) not null,
  path varchar(240) not null,
  status_code int not null check (status_code >= 100 and status_code <= 599),
  detail_json jsonb null
);

create index if not exists ix_security_audit_logs_occurred
on security_audit_logs(occurred_at desc);

create index if not exists ix_security_audit_logs_event
on security_audit_logs(event_type, occurred_at desc);

create index if not exists ix_security_audit_logs_user
on security_audit_logs(user_id, occurred_at desc);

-- ============================================================
-- 6. TRIGGER updated_at
-- ============================================================

create or replace function set_updated_at()
returns trigger as $$
begin
  new.updated_at = now();
  return new;
end;
$$ language plpgsql;

drop trigger if exists trg_session_states_updated_at on session_states;
create trigger trg_session_states_updated_at
before update on session_states
for each row execute function set_updated_at();

drop trigger if exists trg_session_player_states_updated_at on session_player_states;
create trigger trg_session_player_states_updated_at
before update on session_player_states
for each row execute function set_updated_at();

drop trigger if exists trg_session_player_ingredients_updated_at on session_player_ingredients;
create trigger trg_session_player_ingredients_updated_at
before update on session_player_ingredients
for each row execute function set_updated_at();

drop trigger if exists trg_session_player_assets_updated_at on session_player_assets;
create trigger trg_session_player_assets_updated_at
before update on session_player_assets
for each row execute function set_updated_at();

drop trigger if exists trg_session_card_positions_updated_at on session_card_positions;
create trigger trg_session_card_positions_updated_at
before update on session_card_positions
for each row execute function set_updated_at();

-- ============================================================
-- 7. VALIDASI JUMLAH PEMAIN MAKSIMAL 4
-- ============================================================

create or replace function enforce_max_session_players()
returns trigger as $$
declare
  player_count_now int;
begin
  select count(*)
  into player_count_now
  from session_players
  where session_id = new.session_id;

  if player_count_now >= 4 then
    raise exception 'Satu sesi hanya boleh memiliki maksimal 4 pemain';
  end if;

  return new;
end;
$$ language plpgsql;

create or replace function enforce_session_player_user_role()
returns trigger as $$
declare
  resolved_role varchar(20);
begin
  select role
  into resolved_role
  from app_users
  where user_id = new.user_id;

  if resolved_role is distinct from 'PLAYER' then
    raise exception 'Hanya user dengan role PLAYER yang boleh bergabung ke session_players';
  end if;

  return new;
end;
$$ language plpgsql;

drop trigger if exists trg_max_session_players on session_players;
create trigger trg_max_session_players
before insert on session_players
for each row execute function enforce_max_session_players();

drop trigger if exists trg_session_players_user_role on session_players;
create trigger trg_session_players_user_role
before insert or update on session_players
for each row execute function enforce_session_player_user_role();
