create extension if not exists citext;

create extension if not exists pgcrypto;

begin;

do $$ declare username_udt text;

begin select c.udt_name into username_udt from information_schema.columns c where c.table_schema = 'public'
and c.table_name = 'app_users'
and c.column_name = 'username';

if username_udt is not null
and username_udt <> 'citext' then raise exception using errcode = 'P0001',
message = 'Legacy schema detected. app_users.username must be reset to citext on a clean database.';

end if;

if exists (
  select
    1 from information_schema.columns where table_schema = 'public'
    and table_name = 'ruleset_versions'
    and column_name = 'config_json'
) then raise exception using errcode = 'P0001',
message = 'Obsolete ruleset_versions.config_json detected. Database reset required before applying canonical relational baseline.';

end if;

end;

$$;

create table if not exists schema_baseline_versions (
  baseline_name varchar(80) not null,
  schema_version varchar(20) not null,
  checksum varchar(128) not null,
  applied_at timestamptz not null default now(),
  constraint pk_schema_baseline_versions primary key (baseline_name, schema_version),
  constraint ck_schema_baseline_versions_name check (nullif(btrim(baseline_name), '') is not null),
  constraint ck_schema_baseline_versions_version check (nullif(btrim(schema_version), '') is not null),
  constraint ck_schema_baseline_versions_checksum check (nullif(btrim(checksum), '') is not null)
);

create table if not exists app_users (
  user_id uuid not null default gen_random_uuid(),
  username citext not null,
  display_name varchar(80) not null,
  password_hash text not null,
  role varchar(20) not null,
  is_active boolean not null default true,
  created_at timestamptz not null default now(),
  constraint pk_app_users primary key (user_id),
  constraint uq_app_users_username unique (username),
  constraint ck_app_users_role check (role in ('INSTRUCTOR', 'PLAYER'))
);

create index if not exists ix_app_users_role_active on app_users (role, is_active);

create table if not exists actions (
  action_id varchar(80) not null,
  action_name varchar(120) not null,
  behavior_id varchar(80) not null,
  mode varchar(10) not null default 'BOTH',
  cashflow_direction varchar(3) null,
  affects_coin boolean not null default false,
  affects_happiness boolean not null default false,
  affects_saving boolean not null default false,
  affects_inventory boolean not null default false,
  is_active boolean not null default true,
  created_at timestamptz not null default now(),
  constraint pk_actions primary key (action_id),
  constraint ck_actions_behavior_id check (nullif(btrim(behavior_id), '') is not null),
  constraint ck_actions_mode check (mode in ('PEMULA', 'MAHIR', 'BOTH')),
  constraint ck_actions_cashflow_direction check (cashflow_direction in ('IN', 'OUT'))
);

create table if not exists rulesets (
  ruleset_id uuid not null default gen_random_uuid(),
  name varchar(120) not null,
  description text null,
  instructor_user_id uuid null,
  is_archived boolean not null default false,
  archived_at timestamptz null,
  created_at timestamptz not null default now(),
  created_by_user_id uuid null,
  constraint pk_rulesets primary key (ruleset_id),
  constraint ck_rulesets_archive_state check (
    (
      is_archived
      and archived_at is not null
    )
    or (
      not is_archived
      and archived_at is null
    )
  ),
  constraint fk_rulesets_instructor_user_id foreign key (instructor_user_id) references app_users (user_id) on delete
  set
    null,
    constraint fk_rulesets_created_by_user_id foreign key (created_by_user_id) references app_users (user_id) on delete
  set
    null
);

create index if not exists ix_rulesets_created_at on rulesets (created_at desc);

create index if not exists ix_rulesets_instructor_user on rulesets (instructor_user_id, is_archived, created_at desc);

create table if not exists ruleset_versions (
  ruleset_version_id uuid not null default gen_random_uuid(),
  ruleset_id uuid not null,
  version int not null,
  status varchar(10) not null,
  mode varchar(10) not null,
  schema_version varchar(20) not null default '3.0.0',
  config_hash varchar(128) not null,
  change_note text null,
  published_at timestamptz null,
  created_at timestamptz not null default now(),
  created_by_user_id uuid null,
  constraint pk_ruleset_versions primary key (ruleset_version_id),
  constraint uq_ruleset_versions_ruleset_version unique (ruleset_id, version),
  constraint uq_ruleset_versions_ruleset_config_hash unique (ruleset_id, config_hash),
  constraint ck_ruleset_versions_version check (version >= 1),
  constraint ck_ruleset_versions_status check (status in ('DRAFT', 'ACTIVE', 'ARCHIVED')),
  constraint ck_ruleset_versions_mode check (mode in ('PEMULA', 'MAHIR')),
  constraint fk_ruleset_versions_ruleset_id foreign key (ruleset_id) references rulesets (ruleset_id) on delete restrict,
  constraint fk_ruleset_versions_created_by_user_id foreign key (created_by_user_id) references app_users (user_id) on delete
  set
    null
);

create index if not exists ix_ruleset_versions_status_mode on ruleset_versions (status, mode, created_at desc);

create unique index if not exists uq_ruleset_versions_one_active_per_ruleset on ruleset_versions (ruleset_id)
where
  status = 'ACTIVE';

create table if not exists ruleset_game_settings (
  ruleset_version_id uuid not null,
  starting_cash int not null,
  starting_happiness int not null default 0,
  starting_saving int not null default 0,
  actions_per_turn int not null,
  finish_day int not null,
  min_players int not null,
  max_players int not null,
  cash_min int not null default 0,
  max_ingredient_total int not null default 0,
  max_same_ingredient int not null default 0,
  primary_need_max_per_day int null default 1,
  require_primary_before_others boolean not null default true,
  donation_min_amount int not null default 1,
  donation_max_amount int not null default 1,
  gold_trade_allow_buy boolean not null default true,
  gold_trade_allow_sell boolean not null default true,
  loan_enabled boolean not null default false,
  insurance_enabled boolean not null default false,
  saving_goal_enabled boolean not null default false,
  freelance_income int not null default 1,
  created_at timestamptz not null default now(),
  updated_at timestamptz not null default now(),
  constraint pk_ruleset_game_settings primary key (ruleset_version_id),
  constraint ck_ruleset_game_settings_starting_cash check (starting_cash >= 0),
  constraint ck_ruleset_game_settings_starting_happiness check (starting_happiness >= 0),
  constraint ck_ruleset_game_settings_starting_saving check (starting_saving >= 0),
  constraint ck_ruleset_game_settings_actions_per_turn check (
    actions_per_turn between 1
    and 10
  ),
  constraint ck_ruleset_game_settings_finish_day check (finish_day >= 1),
  constraint ck_ruleset_game_settings_min_players check (
    min_players between 2
    and 4
  ),
  constraint ck_ruleset_game_settings_max_players check (
    max_players between 1
    and 4
  ),
  constraint ck_ruleset_game_settings_player_bounds check (min_players <= max_players),
  constraint ck_ruleset_game_settings_cash_min check (cash_min >= 0),
  constraint ck_ruleset_game_settings_max_ingredient_total check (max_ingredient_total >= 0),
  constraint ck_ruleset_game_settings_max_same_ingredient check (max_same_ingredient >= 0),
  constraint ck_ruleset_game_settings_primary_need_max_per_day check (
    primary_need_max_per_day is null
    or primary_need_max_per_day >= 0
  ),
  constraint ck_ruleset_game_settings_donation_bounds check (
    donation_min_amount >= 1
    and donation_max_amount >= donation_min_amount
  ),
  constraint ck_ruleset_game_settings_freelance_income check (freelance_income >= 1),
  constraint fk_ruleset_game_settings_ruleset_version_id foreign key (ruleset_version_id) references ruleset_versions (ruleset_version_id) on delete cascade
);

create table if not exists ruleset_player_ordering_rules (
  ruleset_player_ordering_rule_id uuid not null default gen_random_uuid(),
  ruleset_version_id uuid not null,
  sort_order int not null,
  ordering_code varchar(40) null,
  weekday_code varchar(8) null,
  feature_code varchar(40) null,
  is_enabled boolean not null default true,
  created_at timestamptz not null default now(),
  constraint pk_ruleset_player_ordering_rules primary key (ruleset_player_ordering_rule_id),
  constraint uq_ruleset_player_ordering_rules_scope unique (ruleset_version_id, sort_order),
  constraint ck_ruleset_player_ordering_rules_sort_order check (sort_order >= 1),
  constraint ck_ruleset_player_ordering_rules_payload check (
    ordering_code is not null
    or weekday_code is not null
  ),
  constraint ck_ruleset_player_ordering_rules_ordering_code check (
    ordering_code is null
    or ordering_code in (
      'PLAYER_ORDER',
      'INSTRUCTOR_ORDER',
      'MANUAL_ORDER',
      'EVENT_SEQUENCE',
      'PLAYER_ID',
      'USERNAME',
      'IDN',
      'USERNAME_IDN'
    )
  ),
  constraint ck_ruleset_player_ordering_rules_weekday_code check (
    weekday_code is null
    or weekday_code in ('FRI', 'SAT', 'SUN')
  ),
  constraint fk_ruleset_player_ordering_rules_ruleset_version_id foreign key (ruleset_version_id) references ruleset_versions (ruleset_version_id) on delete cascade
);

create table if not exists ruleset_actions (
  ruleset_action_id uuid not null default gen_random_uuid(),
  ruleset_version_id uuid not null,
  action_id varchar(80) not null,
  behavior_id varchar(80) not null,
  sort_order int not null,
  is_active boolean not null default true,
  created_at timestamptz not null default now(),
  constraint pk_ruleset_actions primary key (ruleset_action_id),
  constraint uq_ruleset_actions_scope unique (ruleset_version_id, ruleset_action_id),
  constraint uq_ruleset_actions_ruleset_action unique (ruleset_version_id, action_id),
  constraint ck_ruleset_actions_behavior_id check (nullif(btrim(behavior_id), '') is not null),
  constraint ck_ruleset_actions_sort_order check (sort_order >= 1),
  constraint fk_ruleset_actions_ruleset_version_id foreign key (ruleset_version_id) references ruleset_versions (ruleset_version_id) on delete cascade,
  constraint fk_ruleset_actions_action_id foreign key (action_id) references actions (action_id) on delete restrict
);

create index if not exists ix_ruleset_actions_ruleset on ruleset_actions (ruleset_version_id, sort_order, action_id);

create table if not exists ruleset_game_assets (
  ruleset_game_asset_id uuid not null default gen_random_uuid(),
  ruleset_version_id uuid not null,
  asset_type varchar(40) not null,
  asset_code varchar(120) not null,
  display_name varchar(160) not null,
  sort_order int not null default 1,
  is_active boolean not null default true,
  metadata_json jsonb not null default '{}' :: jsonb,
  created_at timestamptz not null default now(),
  updated_at timestamptz not null default now(),
  constraint pk_ruleset_game_assets primary key (ruleset_game_asset_id),
  constraint uq_ruleset_game_assets_scope unique (ruleset_version_id, asset_type, asset_code),
  constraint uq_ruleset_game_assets_version_asset unique (ruleset_version_id, ruleset_game_asset_id),
  constraint ck_ruleset_game_assets_type check (
    asset_type in (
      'INGREDIENT',
      'ORDER',
      'NEED',
      'GOLD_PRICE',
      'GOLD',
      'RISK',
      'TIE_BREAKER'
    )
  ),
  constraint ck_ruleset_game_assets_sort_order check (sort_order >= 1),
  constraint fk_ruleset_game_assets_ruleset_version_id foreign key (ruleset_version_id) references ruleset_versions (ruleset_version_id) on delete cascade
);

alter table ruleset_game_assets drop constraint if exists ck_ruleset_game_assets_type;
alter table ruleset_game_assets add constraint ck_ruleset_game_assets_type check (
  asset_type in (
    'INGREDIENT',
    'ORDER',
    'NEED',
    'GOLD_PRICE',
    'GOLD',
    'RISK',
    'DONATION_AWARD',
    'PENSION_AWARD',
    'TIE_BREAKER',
    'COLLECTION_MISSION',
    'FINANCIAL_GOAL',
    'SHARIA_LOAN',
    'INSURANCE'
  )
);

create index if not exists ix_ruleset_game_assets_ruleset on ruleset_game_assets (
  ruleset_version_id,
  asset_type,
  sort_order,
  asset_code
);

create table if not exists ruleset_ingredients (
  ruleset_ingredient_id uuid not null default gen_random_uuid(),
  ruleset_version_id uuid not null,
  ruleset_game_asset_id uuid not null,
  ingredient_code varchar(120) not null,
  item_name varchar(160) not null,
  display_name varchar(160) not null,
  purchase_price int not null default 0,
  sort_order int not null,
  card_qty int null,
  is_active boolean not null default true,
  payload_json jsonb not null default '{}' :: jsonb,
  created_at timestamptz not null default now(),
  constraint pk_ruleset_ingredients primary key (ruleset_ingredient_id),
  constraint uq_ruleset_ingredients_ruleset_code unique (ruleset_version_id, ingredient_code),
  constraint uq_ruleset_ingredients_ruleset_asset unique (ruleset_version_id, ruleset_game_asset_id),
  constraint ck_ruleset_ingredients_sort_order check (sort_order >= 1),
  constraint ck_ruleset_ingredients_purchase_price check (purchase_price >= 0),
  constraint ck_ruleset_ingredients_card_qty check (
    card_qty is null
    or card_qty >= 0
  ),
  constraint fk_ruleset_ingredients_ruleset_version_id foreign key (ruleset_version_id) references ruleset_versions (ruleset_version_id) on delete cascade,
  constraint fk_ruleset_ingredients_asset_id foreign key (ruleset_version_id, ruleset_game_asset_id) references ruleset_game_assets (ruleset_version_id, ruleset_game_asset_id) on delete restrict
);

create index if not exists ix_ruleset_ingredients_ruleset on ruleset_ingredients (ruleset_version_id, sort_order, ingredient_code);

create table if not exists ruleset_orders (
  ruleset_order_id uuid not null default gen_random_uuid(),
  ruleset_version_id uuid not null,
  ruleset_game_asset_id uuid not null,
  order_code varchar(120) not null,
  item_name varchar(160) not null,
  sell_price int not null default 0,
  happiness_points int not null default 0,
  sort_order int not null,
  card_qty int null,
  is_active boolean not null default true,
  payload_json jsonb not null default '{}' :: jsonb,
  created_at timestamptz not null default now(),
  constraint pk_ruleset_orders primary key (ruleset_order_id),
  constraint uq_ruleset_orders_ruleset_code unique (ruleset_version_id, order_code),
  constraint uq_ruleset_orders_version_id unique (ruleset_version_id, ruleset_order_id),
  constraint uq_ruleset_orders_ruleset_asset unique (ruleset_version_id, ruleset_game_asset_id),
  constraint ck_ruleset_orders_sort_order check (sort_order >= 1),
  constraint ck_ruleset_orders_sell_price check (sell_price >= 0),
  constraint ck_ruleset_orders_card_qty check (
    card_qty is null
    or card_qty >= 0
  ),
  constraint fk_ruleset_orders_ruleset_version_id foreign key (ruleset_version_id) references ruleset_versions (ruleset_version_id) on delete cascade,
  constraint fk_ruleset_orders_asset_id foreign key (ruleset_version_id, ruleset_game_asset_id) references ruleset_game_assets (ruleset_version_id, ruleset_game_asset_id) on delete restrict
);

create index if not exists ix_ruleset_orders_ruleset on ruleset_orders (ruleset_version_id, sort_order, order_code);

create table if not exists ruleset_order_requirements (
  ruleset_order_requirement_id uuid not null default gen_random_uuid(),
  ruleset_version_id uuid not null,
  ruleset_order_id uuid not null,
  requirement_order int not null,
  required_asset_id uuid not null,
  qty_required int not null default 1,
  payload_json jsonb not null default '{}' :: jsonb,
  created_at timestamptz not null default now(),
  constraint pk_ruleset_order_requirements primary key (ruleset_order_requirement_id),
  constraint uq_ruleset_order_requirements_order_asset unique (
    ruleset_order_id,
    requirement_order,
    required_asset_id
  ),
  constraint ck_ruleset_order_requirements_requirement_order check (requirement_order >= 1),
  constraint ck_ruleset_order_requirements_qty_required check (qty_required >= 1),
  constraint fk_ruleset_order_requirements_ruleset_order_id foreign key (ruleset_version_id, ruleset_order_id) references ruleset_orders (ruleset_version_id, ruleset_order_id) on delete cascade,
  constraint fk_ruleset_order_requirements_required_asset_id foreign key (ruleset_version_id, required_asset_id) references ruleset_game_assets (ruleset_version_id, ruleset_game_asset_id) on delete restrict
);

create index if not exists ix_ruleset_order_requirements_order on ruleset_order_requirements (ruleset_order_id, requirement_order);

create table if not exists ruleset_needs (
  ruleset_need_id uuid not null default gen_random_uuid(),
  ruleset_version_id uuid not null,
  ruleset_game_asset_id uuid not null,
  need_code varchar(120) not null,
  item_name varchar(160) not null,
  need_tier varchar(40) not null,
  purchase_price int not null default 0,
  happiness_points int not null default 0,
  sort_order int not null,
  card_qty int null,
  is_active boolean not null default true,
  payload_json jsonb not null default '{}' :: jsonb,
  created_at timestamptz not null default now(),
  constraint pk_ruleset_needs primary key (ruleset_need_id),
  constraint uq_ruleset_needs_ruleset_code unique (ruleset_version_id, need_code),
  constraint uq_ruleset_needs_version_id unique (ruleset_version_id, ruleset_need_id),
  constraint uq_ruleset_needs_ruleset_asset unique (ruleset_version_id, ruleset_game_asset_id),
  constraint ck_ruleset_needs_sort_order check (sort_order >= 1),
  constraint ck_ruleset_needs_tier check (
    lower(need_tier) in ('primer', 'sekunder', 'tersier')
  ),
  constraint ck_ruleset_needs_purchase_price check (purchase_price >= 0),
  constraint ck_ruleset_needs_card_qty check (
    card_qty is null
    or card_qty >= 0
  ),
  constraint fk_ruleset_needs_ruleset_version_id foreign key (ruleset_version_id) references ruleset_versions (ruleset_version_id) on delete cascade,
  constraint fk_ruleset_needs_asset_id foreign key (ruleset_version_id, ruleset_game_asset_id) references ruleset_game_assets (ruleset_version_id, ruleset_game_asset_id) on delete cascade
);

create index if not exists ix_ruleset_needs_ruleset on ruleset_needs (ruleset_version_id, sort_order, need_code);

create table if not exists ruleset_need_set_bonuses (
  ruleset_need_set_bonus_id uuid not null default gen_random_uuid(),
  ruleset_version_id uuid not null,
  pattern_code varchar(40) not null,
  required_count int not null,
  points int not null,
  sort_order int not null,
  payload_json jsonb not null default '{}' :: jsonb,
  created_at timestamptz not null default now(),
  constraint pk_ruleset_need_set_bonuses primary key (ruleset_need_set_bonus_id),
  constraint uq_ruleset_need_set_bonuses_scope unique (ruleset_version_id, pattern_code),
  constraint ck_ruleset_need_set_bonuses_pattern check (
    pattern_code in ('THREE_DIFFERENT', 'THREE_SAME')
  ),
  constraint ck_ruleset_need_set_bonuses_required_count check (required_count >= 1),
  constraint ck_ruleset_need_set_bonuses_points check (points >= 0),
  constraint ck_ruleset_need_set_bonuses_sort_order check (sort_order >= 1),
  constraint fk_ruleset_need_set_bonuses_ruleset_version_id foreign key (ruleset_version_id) references ruleset_versions (ruleset_version_id) on delete cascade
);

create index if not exists ix_ruleset_need_set_bonuses_ruleset on ruleset_need_set_bonuses (ruleset_version_id, sort_order, pattern_code);

create table if not exists ruleset_collection_missions (
  ruleset_collection_mission_id uuid not null default gen_random_uuid(),
  ruleset_version_id uuid not null,
  mission_code varchar(120) not null,
  item_name varchar(160) not null,
  success_points int not null default 0,
  failure_points int not null default 0,
  penalty_points int not null default 0,
  sort_order int not null,
  card_qty int null,
  is_active boolean not null default true,
  payload_json jsonb not null default '{}' :: jsonb,
  created_at timestamptz not null default now(),
  constraint pk_ruleset_collection_missions primary key (ruleset_collection_mission_id),
  constraint uq_ruleset_collection_missions_ruleset_code unique (ruleset_version_id, mission_code),
  constraint uq_ruleset_collection_missions_version_id unique (
    ruleset_version_id,
    ruleset_collection_mission_id
  ),
  constraint ck_ruleset_collection_missions_sort_order check (sort_order >= 1),
  constraint ck_ruleset_collection_missions_card_qty check (
    card_qty is null
    or card_qty >= 0
  ),
  constraint fk_ruleset_collection_missions_ruleset_version_id foreign key (ruleset_version_id) references ruleset_versions (ruleset_version_id) on delete cascade
);

create index if not exists ix_ruleset_collection_missions_ruleset on ruleset_collection_missions (ruleset_version_id, sort_order, mission_code);

create table if not exists ruleset_collection_mission_requirements (
  ruleset_collection_mission_requirement_id uuid not null default gen_random_uuid(),
  ruleset_version_id uuid not null,
  ruleset_collection_mission_id uuid not null,
  requirement_order int not null,
  requirement_type varchar(40) not null,
  required_asset_id uuid null,
  required_need_tier varchar(40) null,
  qty_required int null,
  payload_json jsonb not null default '{}' :: jsonb,
  created_at timestamptz not null default now(),
  constraint pk_ruleset_collection_mission_requirements primary key (ruleset_collection_mission_requirement_id),
  constraint uq_ruleset_collection_mission_requirements_scope unique (ruleset_collection_mission_id, requirement_order),
  constraint ck_ruleset_collection_mission_requirements_requirement_order check (requirement_order >= 1),
  constraint ck_ruleset_collection_mission_requirements_type check (requirement_type in ('ASSET', 'NEED_TIER')),
  constraint ck_ruleset_collection_mission_requirements_shape check (
    (
      requirement_type = 'ASSET'
      and required_asset_id is not null
      and required_need_tier is null
    )
    or (
      requirement_type = 'NEED_TIER'
      and required_asset_id is null
      and required_need_tier in ('primer', 'sekunder', 'tersier')
    )
  ),
  constraint ck_ruleset_collection_mission_requirements_qty_required check (
    qty_required is null
    or qty_required >= 0
  ),
  constraint fk_ruleset_collection_mission_requirements_mission_id foreign key (
    ruleset_version_id,
    ruleset_collection_mission_id
  ) references ruleset_collection_missions (
    ruleset_version_id,
    ruleset_collection_mission_id
  ) on delete cascade,
  constraint fk_ruleset_collection_mission_requirements_required_asset_id foreign key (ruleset_version_id, required_asset_id) references ruleset_game_assets (ruleset_version_id, ruleset_game_asset_id) on delete restrict
);

create index if not exists ix_ruleset_collection_mission_requirements_mission on ruleset_collection_mission_requirements (ruleset_collection_mission_id, requirement_order);

create table if not exists ruleset_financial_goals (
  ruleset_financial_goal_id uuid not null default gen_random_uuid(),
  ruleset_version_id uuid not null,
  goal_code varchar(120) not null,
  item_name varchar(160) not null,
  purchase_price int not null default 0,
  happiness_points int not null default 0,
  sort_order int not null,
  card_qty int null,
  is_active boolean not null default true,
  payload_json jsonb not null default '{}' :: jsonb,
  created_at timestamptz not null default now(),
  constraint pk_ruleset_financial_goals primary key (ruleset_financial_goal_id),
  constraint uq_ruleset_financial_goals_ruleset_code unique (ruleset_version_id, goal_code),
  constraint uq_ruleset_financial_goals_version_id unique (ruleset_version_id, ruleset_financial_goal_id),
  constraint ck_ruleset_financial_goals_sort_order check (sort_order >= 1),
  constraint ck_ruleset_financial_goals_purchase_price check (purchase_price >= 0),
  constraint ck_ruleset_financial_goals_card_qty check (
    card_qty is null
    or card_qty >= 0
  ),
  constraint fk_ruleset_financial_goals_ruleset_version_id foreign key (ruleset_version_id) references ruleset_versions (ruleset_version_id) on delete cascade
);

create index if not exists ix_ruleset_financial_goals_ruleset on ruleset_financial_goals (ruleset_version_id, sort_order, goal_code);

create table if not exists ruleset_narratives (
  ruleset_narrative_id uuid not null default gen_random_uuid(),
  ruleset_version_id uuid not null,
  narrative_code varchar(120) not null,
  item_name varchar(160) not null,
  repeatable boolean not null default false,
  cooldown_turns int null,
  sort_order int not null,
  is_active boolean not null default true,
  payload_json jsonb not null default '{}' :: jsonb,
  created_at timestamptz not null default now(),
  constraint pk_ruleset_narratives primary key (ruleset_narrative_id),
  constraint uq_ruleset_narratives_ruleset_code unique (ruleset_version_id, narrative_code),
  constraint uq_ruleset_narratives_version_id unique (ruleset_version_id, ruleset_narrative_id),
  constraint ck_ruleset_narratives_sort_order check (sort_order >= 1),
  constraint ck_ruleset_narratives_cooldown_turns check (
    cooldown_turns is null
    or cooldown_turns >= 0
  ),
  constraint fk_ruleset_narratives_ruleset_version_id foreign key (ruleset_version_id) references ruleset_versions (ruleset_version_id) on delete cascade
);

create index if not exists ix_ruleset_narratives_ruleset on ruleset_narratives (ruleset_version_id, sort_order, narrative_code);

create table if not exists ruleset_narrative_scenes (
  ruleset_narrative_scene_id uuid not null default gen_random_uuid(),
  ruleset_version_id uuid not null,
  ruleset_narrative_id uuid not null,
  scene_code varchar(120) not null,
  scene_order int not null,
  text_lines jsonb not null default '[]' :: jsonb,
  media_json jsonb not null default '{}' :: jsonb,
  payload_json jsonb not null default '{}' :: jsonb,
  created_at timestamptz not null default now(),
  constraint pk_ruleset_narrative_scenes primary key (ruleset_narrative_scene_id),
  constraint uq_ruleset_narrative_scenes_code unique (ruleset_narrative_id, scene_code),
  constraint uq_ruleset_narrative_scenes_order unique (ruleset_narrative_id, scene_order),
  constraint uq_ruleset_narrative_scenes_version_id unique (ruleset_version_id, ruleset_narrative_scene_id),
  constraint ck_ruleset_narrative_scenes_scene_order check (scene_order >= 1),
  constraint fk_ruleset_narrative_scenes_narrative_id foreign key (ruleset_version_id, ruleset_narrative_id) references ruleset_narratives (ruleset_version_id, ruleset_narrative_id) on delete cascade
);

create index if not exists ix_ruleset_narrative_scenes_narrative on ruleset_narrative_scenes (ruleset_narrative_id, scene_order);

create table if not exists ruleset_trigger_conditions (
  ruleset_trigger_condition_id uuid not null default gen_random_uuid(),
  ruleset_version_id uuid not null,
  trigger_owner_type varchar(20) not null,
  ruleset_narrative_id uuid not null,
  ruleset_action_id uuid not null,
  reference_asset_id uuid null,
  operator varchar(20) not null default 'OCCURS',
  threshold_numeric int not null default 1,
  sort_order int not null default 1,
  condition_json jsonb not null default '{}' :: jsonb,
  is_active boolean not null default true,
  created_at timestamptz not null default now(),
  constraint pk_ruleset_trigger_conditions primary key (ruleset_trigger_condition_id),
  constraint uq_ruleset_trigger_conditions_scope unique (
    ruleset_version_id,
    ruleset_narrative_id,
    sort_order
  ),
  constraint ck_ruleset_trigger_conditions_owner_type check (trigger_owner_type = 'NARRATIVE'),
  constraint ck_ruleset_trigger_conditions_operator check (
    operator in (
      'OCCURS',
      'COUNT_GTE',
      'AMOUNT_GTE',
      'AMOUNT_LTE'
    )
  ),
  constraint ck_ruleset_trigger_conditions_threshold check (threshold_numeric >= 0),
  constraint ck_ruleset_trigger_conditions_sort_order check (sort_order >= 1),
  constraint fk_ruleset_trigger_conditions_ruleset_version_id foreign key (ruleset_version_id) references ruleset_versions (ruleset_version_id) on delete cascade,
  constraint fk_ruleset_trigger_conditions_narrative_id foreign key (ruleset_version_id, ruleset_narrative_id) references ruleset_narratives (ruleset_version_id, ruleset_narrative_id) on delete cascade,
  constraint fk_ruleset_trigger_conditions_action_id foreign key (ruleset_version_id, ruleset_action_id) references ruleset_actions (ruleset_version_id, ruleset_action_id) on delete restrict,
  constraint fk_ruleset_trigger_conditions_reference_asset_id foreign key (ruleset_version_id, reference_asset_id) references ruleset_game_assets (ruleset_version_id, ruleset_game_asset_id) on delete restrict
);

create index if not exists ix_ruleset_trigger_conditions_ruleset on ruleset_trigger_conditions (
  ruleset_version_id,
  trigger_owner_type,
  ruleset_action_id,
  is_active
);

create table if not exists ruleset_gold_prices (
  ruleset_gold_price_id uuid not null default gen_random_uuid(),
  ruleset_version_id uuid not null,
  ruleset_game_asset_id uuid not null,
  price_code varchar(120) not null,
  quantity int not null,
  unit_price int not null,
  sort_order int not null,
  card_qty int null,
  is_active boolean not null default true,
  payload_json jsonb not null default '{}' :: jsonb,
  created_at timestamptz not null default now(),
  constraint pk_ruleset_gold_prices primary key (ruleset_gold_price_id),
  constraint uq_ruleset_gold_prices_ruleset_code unique (ruleset_version_id, price_code),
  constraint uq_ruleset_gold_prices_ruleset_asset unique (ruleset_version_id, ruleset_game_asset_id),
  constraint ck_ruleset_gold_prices_quantity check (quantity >= 1),
  constraint ck_ruleset_gold_prices_unit_price check (unit_price >= 0),
  constraint ck_ruleset_gold_prices_sort_order check (sort_order >= 1),
  constraint ck_ruleset_gold_prices_card_qty check (
    card_qty is null
    or card_qty >= 0
  ),
  constraint fk_ruleset_gold_prices_ruleset_version_id foreign key (ruleset_version_id) references ruleset_versions (ruleset_version_id) on delete cascade,
  constraint fk_ruleset_gold_prices_asset_id foreign key (ruleset_version_id, ruleset_game_asset_id) references ruleset_game_assets (ruleset_version_id, ruleset_game_asset_id) on delete cascade
);

create index if not exists ix_ruleset_gold_prices_ruleset on ruleset_gold_prices (ruleset_version_id, sort_order, price_code);

create table if not exists ruleset_gold_assets (
  ruleset_gold_asset_id uuid not null default gen_random_uuid(),
  ruleset_version_id uuid not null,
  ruleset_game_asset_id uuid not null,
  asset_code varchar(120) not null,
  quantity int not null,
  points int not null,
  sort_order int not null,
  card_qty int null,
  is_active boolean not null default true,
  payload_json jsonb not null default '{}' :: jsonb,
  created_at timestamptz not null default now(),
  constraint pk_ruleset_gold_assets primary key (ruleset_gold_asset_id),
  constraint uq_ruleset_gold_assets_ruleset_code unique (ruleset_version_id, asset_code),
  constraint uq_ruleset_gold_assets_ruleset_asset unique (ruleset_version_id, ruleset_game_asset_id),
  constraint ck_ruleset_gold_assets_quantity check (quantity >= 1),
  constraint ck_ruleset_gold_assets_points check (points >= 0),
  constraint ck_ruleset_gold_assets_sort_order check (sort_order >= 1),
  constraint ck_ruleset_gold_assets_card_qty check (
    card_qty is null
    or card_qty >= 0
  ),
  constraint fk_ruleset_gold_assets_ruleset_version_id foreign key (ruleset_version_id) references ruleset_versions (ruleset_version_id) on delete cascade,
  constraint fk_ruleset_gold_assets_asset_id foreign key (ruleset_version_id, ruleset_game_asset_id) references ruleset_game_assets (ruleset_version_id, ruleset_game_asset_id) on delete cascade
);

create index if not exists ix_ruleset_gold_assets_ruleset on ruleset_gold_assets (ruleset_version_id, sort_order, asset_code);

create table if not exists ruleset_rank_points (
  ruleset_rank_point_id uuid not null default gen_random_uuid(),
  ruleset_version_id uuid not null,
  rank_type varchar(20) not null,
  rank_no int not null,
  points int not null,
  sort_order int not null,
  created_at timestamptz not null default now(),
  constraint pk_ruleset_rank_points primary key (ruleset_rank_point_id),
  constraint uq_ruleset_rank_points_rank unique (ruleset_version_id, rank_type, rank_no),
  constraint ck_ruleset_rank_points_type check (rank_type in ('DONATION', 'PENSION')),
  constraint ck_ruleset_rank_points_rank check (rank_no >= 1),
  constraint ck_ruleset_rank_points_points check (points >= 0),
  constraint ck_ruleset_rank_points_sort_order check (sort_order >= 1),
  constraint fk_ruleset_rank_points_ruleset_version_id foreign key (ruleset_version_id) references ruleset_versions (ruleset_version_id) on delete cascade
);

create index if not exists ix_ruleset_rank_points_ruleset on ruleset_rank_points (
  ruleset_version_id,
  rank_type,
  sort_order,
  rank_no
);

create table if not exists ruleset_tie_breakers (
  ruleset_tie_breaker_id uuid not null default gen_random_uuid(),
  ruleset_version_id uuid not null,
  ruleset_game_asset_id uuid not null,
  tie_breaker_code varchar(120) not null,
  tie_number int not null,
  sort_order int not null,
  card_qty int null,
  payload_json jsonb not null default '{}' :: jsonb,
  created_at timestamptz not null default now(),
  constraint pk_ruleset_tie_breakers primary key (ruleset_tie_breaker_id),
  constraint uq_ruleset_tie_breakers_ruleset_code unique (ruleset_version_id, tie_breaker_code),
  constraint uq_ruleset_tie_breakers_ruleset_asset unique (ruleset_version_id, ruleset_game_asset_id),
  constraint ck_ruleset_tie_breakers_tie_number check (tie_number >= 1),
  constraint ck_ruleset_tie_breakers_sort_order check (sort_order >= 1),
  constraint ck_ruleset_tie_breakers_card_qty check (
    card_qty is null
    or card_qty >= 0
  ),
  constraint fk_ruleset_tie_breakers_ruleset_version_id foreign key (ruleset_version_id) references ruleset_versions (ruleset_version_id) on delete cascade,
  constraint fk_ruleset_tie_breakers_asset_id foreign key (ruleset_version_id, ruleset_game_asset_id) references ruleset_game_assets (ruleset_version_id, ruleset_game_asset_id) on delete cascade
);

create index if not exists ix_ruleset_tie_breakers_ruleset on ruleset_tie_breakers (ruleset_version_id, sort_order, tie_breaker_code);

create table if not exists ruleset_sharia_loans (
  ruleset_sharia_loan_id uuid not null default gen_random_uuid(),
  ruleset_version_id uuid not null,
  loan_code varchar(120) not null,
  item_name varchar(160) not null,
  principal int not null,
  installment int not null,
  duration_days int null,
  penalty_points int not null default 0,
  sort_order int not null,
  card_qty int null,
  payload_json jsonb not null default '{}' :: jsonb,
  created_at timestamptz not null default now(),
  constraint pk_ruleset_sharia_loans primary key (ruleset_sharia_loan_id),
  constraint uq_ruleset_sharia_loans_ruleset_code unique (ruleset_version_id, loan_code),
  constraint uq_ruleset_sharia_loans_version_id unique (ruleset_version_id, ruleset_sharia_loan_id),
  constraint ck_ruleset_sharia_loans_principal check (principal >= 0),
  constraint ck_ruleset_sharia_loans_installment check (installment >= 0),
  constraint ck_ruleset_sharia_loans_duration_days check (
    duration_days is null
    or duration_days >= 1
  ),
  constraint ck_ruleset_sharia_loans_penalty_points check (penalty_points >= 0),
  constraint ck_ruleset_sharia_loans_sort_order check (sort_order >= 1),
  constraint ck_ruleset_sharia_loans_card_qty check (
    card_qty is null
    or card_qty >= 0
  ),
  constraint fk_ruleset_sharia_loans_ruleset_version_id foreign key (ruleset_version_id) references ruleset_versions (ruleset_version_id) on delete cascade
);

create index if not exists ix_ruleset_sharia_loans_ruleset on ruleset_sharia_loans (ruleset_version_id, sort_order, loan_code);

create table if not exists ruleset_insurance_products (
  ruleset_insurance_product_id uuid not null default gen_random_uuid(),
  ruleset_version_id uuid not null,
  product_code varchar(120) not null,
  item_name varchar(160) not null,
  premium int not null,
  usage_limit int not null,
  sort_order int not null,
  card_qty int null,
  payload_json jsonb not null default '{}' :: jsonb,
  created_at timestamptz not null default now(),
  constraint pk_ruleset_insurance_products primary key (ruleset_insurance_product_id),
  constraint uq_ruleset_insurance_products_ruleset_code unique (ruleset_version_id, product_code),
  constraint uq_ruleset_insurance_products_version_id unique (ruleset_version_id, ruleset_insurance_product_id),
  constraint ck_ruleset_insurance_products_premium check (premium >= 0),
  constraint ck_ruleset_insurance_products_usage_limit check (usage_limit >= 0),
  constraint ck_ruleset_insurance_products_sort_order check (sort_order >= 1),
  constraint ck_ruleset_insurance_products_card_qty check (
    card_qty is null
    or card_qty >= 0
  ),
  constraint fk_ruleset_insurance_products_ruleset_version_id foreign key (ruleset_version_id) references ruleset_versions (ruleset_version_id) on delete cascade
);

create index if not exists ix_ruleset_insurance_products_ruleset on ruleset_insurance_products (ruleset_version_id, sort_order, product_code);

create table if not exists ruleset_life_risks (
  ruleset_life_risk_id uuid not null default gen_random_uuid(),
  ruleset_version_id uuid not null,
  ruleset_game_asset_id uuid not null,
  risk_code varchar(120) not null,
  item_name varchar(160) not null,
  effect_type varchar(40) not null,
  direction varchar(10) null,
  amount int not null default 0,
  sort_order int not null,
  card_qty int null,
  payload_json jsonb not null default '{}' :: jsonb,
  created_at timestamptz not null default now(),
  constraint pk_ruleset_life_risks primary key (ruleset_life_risk_id),
  constraint uq_ruleset_life_risks_ruleset_code unique (ruleset_version_id, risk_code),
  constraint uq_ruleset_life_risks_ruleset_asset unique (ruleset_version_id, ruleset_game_asset_id),
  constraint ck_ruleset_life_risks_direction check (
    direction is null
    or direction in ('IN', 'OUT')
  ),
  constraint ck_ruleset_life_risks_amount check (amount >= 0),
  constraint ck_ruleset_life_risks_sort_order check (sort_order >= 1),
  constraint ck_ruleset_life_risks_card_qty check (
    card_qty is null
    or card_qty >= 0
  ),
  constraint fk_ruleset_life_risks_ruleset_version_id foreign key (ruleset_version_id) references ruleset_versions (ruleset_version_id) on delete cascade,
  constraint fk_ruleset_life_risks_asset_id foreign key (ruleset_version_id, ruleset_game_asset_id) references ruleset_game_assets (ruleset_version_id, ruleset_game_asset_id) on delete cascade
);

create index if not exists ix_ruleset_life_risks_ruleset on ruleset_life_risks (ruleset_version_id, sort_order, risk_code);

create table if not exists sessions (
  session_id uuid not null default gen_random_uuid(),
  session_name varchar(120) not null,
  ruleset_version_id uuid not null,
  mode varchar(10) not null,
  status varchar(10) not null,
  player_count int not null default 0,
  started_at timestamptz null,
  ended_at timestamptz null,
  instructor_user_id uuid null,
  is_archived boolean not null default false,
  archived_at timestamptz null,
  created_at timestamptz not null default now(),
  constraint pk_sessions primary key (session_id),
  constraint ck_sessions_mode check (mode in ('PEMULA', 'MAHIR')),
  constraint ck_sessions_status check (status in ('CREATED', 'STARTED', 'ENDED')),
  constraint ck_sessions_player_count check (
    player_count between 0
    and 4
  ),
  constraint ck_sessions_archive_state check (
    (
      is_archived
      and archived_at is not null
    )
    or (
      not is_archived
      and archived_at is null
    )
  ),
  constraint fk_sessions_instructor_user_id foreign key (instructor_user_id) references app_users (user_id) on delete
  set
    null,
    constraint fk_sessions_ruleset_version_id foreign key (ruleset_version_id) references ruleset_versions (ruleset_version_id) on delete restrict
);

create index if not exists ix_sessions_status on sessions (status);

create index if not exists ix_sessions_created_at on sessions (created_at desc);

create index if not exists ix_sessions_instructor_user on sessions (instructor_user_id, is_archived, created_at desc);

create index if not exists ix_sessions_ruleset_version on sessions (ruleset_version_id);

create table if not exists session_participants (
  session_participant_id uuid not null default gen_random_uuid(),
  session_id uuid not null,
  user_id uuid not null,
  player_order_no int not null,
  player_name varchar(80) null,
  joined_at timestamptz not null default now(),
  constraint pk_session_participants primary key (session_participant_id),
  constraint uq_session_participants_session_user unique (session_id, user_id),
  constraint uq_session_participants_session_seat unique (session_id, player_order_no) deferrable initially immediate,
  constraint uq_session_participants_session_participant unique (session_id, session_participant_id),
  constraint ck_session_participants_player_order_no check (
    player_order_no between 1
    and 4
  ),
  constraint ck_session_participants_player_name_not_blank check (
    player_name is null
    or nullif(btrim(player_name), '') is not null
  ),
  constraint fk_session_participants_session_id foreign key (session_id) references sessions (session_id) on delete restrict,
  constraint fk_session_participants_user_id foreign key (user_id) references app_users (user_id) on delete restrict
);

create index if not exists ix_session_participants_user on session_participants (user_id);

create table if not exists session_states (
  session_id uuid not null,
  day int not null,
  weekday varchar(3) not null,
  turn_number int not null default 0,
  action_slot int not null,
  current_session_player_id uuid null,
  current_action_slot int not null default 1,
  action_slots_left int not null,
  finish_day int not null,
  phase varchar(30) not null default 'PLAYER_TURN',
  is_game_over boolean not null default false,
  state_version bigint not null default 1,
  last_event_id uuid null,
  ui_state_json jsonb not null default '{}' :: jsonb,
  created_at timestamptz not null default now(),
  updated_at timestamptz not null default now(),
  constraint pk_session_states primary key (session_id),
  constraint ck_session_states_day check (day >= 1),
  constraint ck_session_states_weekday check (
    weekday in ('MON', 'TUE', 'WED', 'THU', 'FRI', 'SAT', 'SUN')
  ),
  constraint ck_session_states_turn_number check (turn_number between 0 and 4),
  constraint ck_session_states_action_slot check (action_slot >= 0),
  constraint ck_session_states_current_action_slot check (current_action_slot >= 0),
  constraint ck_session_states_action_slots_left check (action_slots_left >= 0),
  constraint ck_session_states_finish_day check (finish_day >= 1),
  constraint ck_session_states_phase check (
    phase in (
      'SETUP',
      'PLAYER_TURN',
      'DONATION_DAY',
      'GOLD_INVESTMENT_DAY',
      'DAY_END',
      'GAME_END'
    )
  ),
  constraint ck_session_states_state_version check (state_version >= 1),
  constraint fk_session_states_session_id foreign key (session_id) references sessions (session_id) on delete restrict,
  constraint fk_session_states_current_session_player foreign key (session_id, current_session_player_id) references session_participants (session_id, session_participant_id) on delete
  set
    null (current_session_player_id)
);

create table if not exists session_participant_balances (
  session_id uuid not null,
  session_participant_id uuid not null,
  coins int not null default 0,
  happiness int not null default 0,
  saving int not null default 0,
  total_donasi int not null default 0,
  last_event_id uuid null,
  created_at timestamptz not null default now(),
  updated_at timestamptz not null default now(),
  constraint pk_session_participant_balances primary key (session_participant_id),
  constraint ck_session_participant_balances_coins check (coins >= 0),
  constraint ck_session_participant_balances_happiness check (happiness >= 0),
  constraint ck_session_participant_balances_saving check (saving >= 0),
  constraint ck_session_participant_balances_total_donasi check (total_donasi >= 0),
  constraint fk_session_participant_balances_session_participant_id foreign key (session_id, session_participant_id) references session_participants (session_id, session_participant_id) on delete cascade
);

create table if not exists session_participant_inventory (
  session_id uuid not null,
  session_participant_id uuid not null,
  ruleset_version_id uuid not null,
  ruleset_game_asset_id uuid not null,
  qty int not null,
  last_event_id uuid null,
  created_at timestamptz not null default now(),
  updated_at timestamptz not null default now(),
  constraint pk_session_participant_inventory primary key (session_participant_id, ruleset_game_asset_id),
  constraint ck_session_participant_inventory_qty check (qty >= 0),
  constraint fk_session_participant_inventory_session_participant_id foreign key (session_id, session_participant_id) references session_participants (session_id, session_participant_id) on delete cascade,
  constraint fk_session_participant_inventory_asset_id foreign key (ruleset_version_id, ruleset_game_asset_id) references ruleset_game_assets (ruleset_version_id, ruleset_game_asset_id) on delete restrict
);

create table if not exists session_participant_need_purchases (
  session_participant_need_purchase_id uuid not null default gen_random_uuid(),
  session_id uuid not null,
  session_participant_id uuid not null,
  ruleset_version_id uuid not null,
  ruleset_need_id uuid not null,
  sort_order int not null,
  paid_amount int not null,
  happiness_delta int not null,
  purchased_at_day int not null,
  source_event_id uuid null,
  created_at timestamptz not null default now(),
  constraint pk_session_participant_need_purchases primary key (session_participant_need_purchase_id),
  constraint ck_session_participant_need_purchases_sort_order check (sort_order >= 1),
  constraint ck_session_participant_need_purchases_paid_amount check (paid_amount >= 0),
  constraint ck_session_participant_need_purchases_purchased_at_day check (purchased_at_day >= 1),
  constraint fk_session_participant_need_purchases_session_participant_id foreign key (session_id, session_participant_id) references session_participants (session_id, session_participant_id) on delete cascade,
  constraint fk_session_participant_need_purchases_ruleset_need_id foreign key (ruleset_version_id, ruleset_need_id) references ruleset_needs (ruleset_version_id, ruleset_need_id) on delete restrict
);

create index if not exists ix_session_participant_need_purchases_participant on session_participant_need_purchases (session_participant_id, sort_order);

create table if not exists session_participant_financial_goals (
  session_id uuid not null,
  session_participant_id uuid not null,
  ruleset_version_id uuid not null,
  ruleset_financial_goal_id uuid not null,
  current_amount int not null default 0,
  target_amount int null,
  status varchar(20) not null default 'ONGOING',
  purchased_at_day int null,
  last_event_id uuid null,
  created_at timestamptz not null default now(),
  updated_at timestamptz not null default now(),
  constraint pk_session_participant_financial_goals primary key (
    session_participant_id,
    ruleset_financial_goal_id
  ),
  constraint ck_session_participant_financial_goals_current_amount check (current_amount >= 0),
  constraint ck_session_participant_financial_goals_target_amount check (
    target_amount is null
    or target_amount >= 0
  ),
  constraint ck_session_participant_financial_goals_current_target check (
    target_amount is null
    or current_amount <= target_amount
  ),
  constraint ck_session_participant_financial_goals_purchased_at_day check (
    purchased_at_day is null
    or purchased_at_day >= 1
  ),
  constraint ck_session_participant_financial_goals_status check (status in ('ONGOING', 'COMPLETED', 'FAILED')),
  constraint ck_session_participant_financial_goals_status_purchase_day check (
    (
      status in ('ONGOING', 'FAILED')
      and purchased_at_day is null
    )
    or (
      status = 'COMPLETED'
      and purchased_at_day is not null
    )
  ),
  constraint fk_session_participant_financial_goals_session_participant_id foreign key (session_id, session_participant_id) references session_participants (session_id, session_participant_id) on delete cascade,
  constraint fk_session_participant_financial_goals_goal_id foreign key (ruleset_version_id, ruleset_financial_goal_id) references ruleset_financial_goals (ruleset_version_id, ruleset_financial_goal_id) on delete restrict
);

create table if not exists session_participant_collection_missions (
  session_id uuid not null,
  session_participant_id uuid not null,
  ruleset_version_id uuid not null,
  ruleset_collection_mission_id uuid not null,
  is_completed boolean not null default false,
  is_failed boolean not null default false,
  reward_applied boolean not null default false,
  last_event_id uuid null,
  assigned_at timestamptz not null default now(),
  updated_at timestamptz not null default now(),
  constraint pk_session_participant_collection_missions primary key (
    session_participant_id,
    ruleset_collection_mission_id
  ),
  constraint ck_session_participant_collection_missions_terminal check (
    not (
      is_completed
      and is_failed
    )
  ),
  constraint ck_session_participant_collection_missions_reward check (
    not reward_applied
    or is_completed
  ),
  constraint fk_session_participant_collection_missions_session_participant_id foreign key (session_id, session_participant_id) references session_participants (session_id, session_participant_id) on delete cascade,
  constraint fk_session_participant_collection_missions_mission_id foreign key (
    ruleset_version_id,
    ruleset_collection_mission_id
  ) references ruleset_collection_missions (
    ruleset_version_id,
    ruleset_collection_mission_id
  ) on delete restrict
);

create table if not exists session_participant_action_counters (
  session_id uuid not null,
  session_participant_id uuid not null,
  ruleset_version_id uuid not null,
  ruleset_action_id uuid not null,
  count int not null,
  last_event_id uuid null,
  constraint pk_session_participant_action_counters primary key (session_participant_id, ruleset_action_id),
  constraint ck_session_participant_action_counters_count check (count >= 0),
  constraint fk_session_participant_action_counters_session_participant_id foreign key (session_id, session_participant_id) references session_participants (session_id, session_participant_id) on delete cascade,
  constraint fk_session_participant_action_counters_action_id foreign key (ruleset_version_id, ruleset_action_id) references ruleset_actions (ruleset_version_id, ruleset_action_id) on delete restrict
);

create table if not exists session_participant_gold_holdings (
  session_id uuid not null,
  session_participant_id uuid not null,
  ruleset_version_id uuid not null,
  ruleset_game_asset_id uuid not null,
  quantity int not null default 0,
  last_event_id uuid null,
  created_at timestamptz not null default now(),
  updated_at timestamptz not null default now(),
  constraint pk_session_participant_gold_holdings primary key (session_participant_id, ruleset_game_asset_id),
  constraint ck_session_participant_gold_holdings_quantity check (quantity >= 0),
  constraint fk_session_participant_gold_holdings_participant_id foreign key (session_id, session_participant_id) references session_participants (session_id, session_participant_id) on delete cascade,
  constraint fk_session_participant_gold_holdings_asset_id foreign key (ruleset_version_id, ruleset_game_asset_id) references ruleset_game_assets (ruleset_version_id, ruleset_game_asset_id) on delete restrict
);

create table if not exists session_participant_loans (
  session_participant_loan_id uuid not null default gen_random_uuid(),
  session_id uuid not null,
  session_participant_id uuid not null,
  ruleset_version_id uuid not null,
  ruleset_sharia_loan_id uuid not null,
  principal_amount int not null,
  outstanding_amount int not null,
  installment_amount int null,
  status varchar(20) not null default 'ACTIVE',
  source_event_id uuid null,
  last_event_id uuid null,
  metadata_json jsonb not null default '{}' :: jsonb,
  created_at timestamptz not null default now(),
  updated_at timestamptz not null default now(),
  constraint pk_session_participant_loans primary key (session_participant_loan_id),
  constraint uq_session_participant_loans_scope unique (session_participant_id, ruleset_sharia_loan_id),
  constraint ck_session_participant_loans_principal_amount check (principal_amount >= 0),
  constraint ck_session_participant_loans_outstanding_amount check (outstanding_amount >= 0),
  constraint ck_session_participant_loans_installment_amount check (
    installment_amount is null
    or installment_amount >= 0
  ),
  constraint ck_session_participant_loans_status check (status in ('ACTIVE', 'PAID', 'DEFAULTED')),
  constraint fk_session_participant_loans_participant_id foreign key (session_id, session_participant_id) references session_participants (session_id, session_participant_id) on delete cascade,
  constraint fk_session_participant_loans_ruleset_loan_id foreign key (ruleset_version_id, ruleset_sharia_loan_id) references ruleset_sharia_loans (ruleset_version_id, ruleset_sharia_loan_id) on delete restrict
);

create table if not exists session_participant_insurances (
  session_participant_insurance_id uuid not null default gen_random_uuid(),
  session_id uuid not null,
  session_participant_id uuid not null,
  ruleset_version_id uuid not null,
  ruleset_insurance_product_id uuid not null,
  premium_paid int not null default 0,
  remaining_uses int not null default 0,
  status varchar(20) not null default 'ACTIVE',
  source_event_id uuid null,
  last_event_id uuid null,
  metadata_json jsonb not null default '{}' :: jsonb,
  created_at timestamptz not null default now(),
  updated_at timestamptz not null default now(),
  constraint pk_session_participant_insurances primary key (session_participant_insurance_id),
  constraint uq_session_participant_insurances_scope unique (
    session_participant_id,
    ruleset_insurance_product_id
  ),
  constraint ck_session_participant_insurances_premium_paid check (premium_paid >= 0),
  constraint ck_session_participant_insurances_remaining_uses check (remaining_uses >= 0),
  constraint ck_session_participant_insurances_status check (status in ('ACTIVE', 'INACTIVE', 'EXPIRED')),
  constraint fk_session_participant_insurances_participant_id foreign key (session_id, session_participant_id) references session_participants (session_id, session_participant_id) on delete cascade,
  constraint fk_session_participant_insurances_ruleset_product_id foreign key (ruleset_version_id, ruleset_insurance_product_id) references ruleset_insurance_products (ruleset_version_id, ruleset_insurance_product_id) on delete restrict
);

create table if not exists session_participant_tie_breakers (
  session_participant_tie_breaker_id uuid not null default gen_random_uuid(),
  session_id uuid not null,
  session_participant_id uuid not null,
  ruleset_version_id uuid not null,
  ruleset_game_asset_id uuid not null,
  tie_number int not null,
  source_event_id uuid null,
  assigned_at timestamptz not null default now(),
  metadata_json jsonb not null default '{}' :: jsonb,
  constraint pk_session_participant_tie_breakers primary key (session_participant_tie_breaker_id),
  constraint uq_session_participant_tie_breakers_participant unique (session_participant_id),
  constraint uq_session_participant_tie_breakers_session_number unique (session_id, tie_number),
  constraint ck_session_participant_tie_breakers_tie_number check (tie_number >= 1),
  constraint fk_session_participant_tie_breakers_participant_id foreign key (session_id, session_participant_id) references session_participants (session_id, session_participant_id) on delete cascade,
  constraint fk_session_participant_tie_breakers_ruleset_version_id foreign key (ruleset_version_id) references ruleset_versions (ruleset_version_id) on delete restrict,
  constraint fk_session_participant_tie_breakers_asset_id foreign key (ruleset_version_id, ruleset_game_asset_id) references ruleset_game_assets (ruleset_version_id, ruleset_game_asset_id) on delete restrict
);

create table if not exists session_donation_events (
  donation_event_id uuid not null default gen_random_uuid(),
  session_id uuid not null,
  event_ke int not null,
  day int not null,
  source_event_id uuid null,
  created_at timestamptz not null default now(),
  constraint pk_session_donation_events primary key (donation_event_id),
  constraint uq_session_donation_events_session_event unique (session_id, event_ke),
  constraint ck_session_donation_events_event_ke check (event_ke >= 1),
  constraint ck_session_donation_events_day check (day >= 1),
  constraint fk_session_donation_events_session_id foreign key (session_id) references sessions (session_id) on delete cascade
);

create table if not exists session_card_positions (
  card_position_id uuid not null default gen_random_uuid(),
  card_instance_id uuid not null default gen_random_uuid(),
  session_id uuid not null,
  ruleset_version_id uuid not null,
  owner_session_participant_id uuid null,
  ruleset_game_asset_id uuid not null,
  copy_number int not null default 1,
  zone varchar(40) not null,
  position_order int null,
  slot_code varchar(80) null,
  slot_group varchar(80) null,
  status varchar(20) not null default 'ACTIVE',
  last_event_id uuid null,
  created_at timestamptz not null default now(),
  updated_at timestamptz not null default now(),
  constraint pk_session_card_positions primary key (card_position_id),
  constraint uq_session_card_positions_instance unique (card_instance_id),
  constraint uq_session_card_positions_copy unique (session_id, ruleset_game_asset_id, copy_number),
  constraint ck_session_card_positions_copy_number check (copy_number >= 1),
  constraint ck_session_card_positions_zone check (zone in ('DECK', 'MARKET', 'PLAYER', 'DISCARD')),
  constraint ck_session_card_positions_status check (status in ('ACTIVE', 'REMOVED')),
  constraint fk_session_card_positions_session_id foreign key (session_id) references sessions (session_id) on delete cascade,
  constraint fk_session_card_positions_ruleset_version_id foreign key (ruleset_version_id) references ruleset_versions (ruleset_version_id) on delete restrict,
  constraint fk_session_card_positions_asset_id foreign key (ruleset_version_id, ruleset_game_asset_id) references ruleset_game_assets (ruleset_version_id, ruleset_game_asset_id) on delete restrict,
  constraint fk_session_card_positions_owner_participant foreign key (session_id, owner_session_participant_id) references session_participants (session_id, session_participant_id) on delete
  set
    null (owner_session_participant_id)
);

create index if not exists ix_session_card_positions_session on session_card_positions (
  session_id,
  owner_session_participant_id,
  ruleset_game_asset_id,
  position_order
);

create unique index if not exists uq_session_card_positions_slot on session_card_positions (session_id, zone, slot_group, slot_code)
where
  slot_group is not null
  and slot_code is not null
  and status = 'ACTIVE';

create table if not exists events (
  event_pk uuid not null default gen_random_uuid(),
  event_id uuid not null,
  session_id uuid not null,
  session_player_id uuid null,
  user_id uuid null,
  actor_type varchar(10) not null,
  "timestamp" timestamptz not null,
  day_index int not null,
  weekday varchar(3) not null,
  turn_number int not null default 0,
  action_slot int not null,
  sequence_number bigint not null,
  ruleset_action_id uuid not null,
  action_type varchar(80) not null,
  ruleset_version_id uuid not null,
  payload_version varchar(20) not null default '1.0',
  payload jsonb not null default '{}' :: jsonb,
  received_at timestamptz not null default now(),
  client_request_id varchar(120) null,
  constraint pk_events primary key (event_pk),
  constraint uq_events_session_event unique (session_id, event_id),
  constraint uq_events_session_event_ruleset unique (session_id, event_id, ruleset_version_id),
  constraint uq_events_session_sequence unique (session_id, sequence_number),
  constraint ck_events_actor_type check (actor_type in ('PLAYER', 'SYSTEM')),
  constraint ck_events_day_index check (day_index >= 0),
  constraint ck_events_weekday check (
    weekday in ('MON', 'TUE', 'WED', 'THU', 'FRI', 'SAT', 'SUN')
  ),
  constraint ck_events_turn_number check (turn_number between 0 and 4),
  constraint ck_events_action_slot check (action_slot >= 0),
  constraint ck_events_actor_turn_slot_shape check (
    (
      actor_type = 'SYSTEM'
      and turn_number = 0
      and action_slot = 0
    )
    or
    (
      actor_type = 'PLAYER'
      and turn_number between 1 and 4
      and action_slot >= 1
      and session_player_id is not null
    )
  ),
  constraint ck_events_sequence_number check (sequence_number >= 0),
  constraint fk_events_session_id foreign key (session_id) references sessions (session_id) on delete restrict,
  constraint fk_events_session_player foreign key (session_id, session_player_id) references session_participants (session_id, session_participant_id) on delete
  set
    null (session_player_id),
    constraint fk_events_user_id foreign key (user_id) references app_users (user_id) on delete restrict,
    constraint fk_events_ruleset_action_id foreign key (ruleset_version_id, ruleset_action_id) references ruleset_actions (ruleset_version_id, ruleset_action_id) on delete restrict,
    constraint fk_events_ruleset_version_id foreign key (ruleset_version_id) references ruleset_versions (ruleset_version_id) on delete restrict
);

create index if not exists ix_events_session_player_seq on events (session_id, session_player_id, sequence_number);

create index if not exists ix_events_session_time on events (session_id, "timestamp");

create index if not exists ix_events_session_action on events (session_id, action_type);

create index if not exists ix_events_user_time on events (user_id, "timestamp");

create index if not exists ix_events_received_at on events (received_at);

create unique index if not exists uq_events_session_client_request on events (session_id, client_request_id)
where
  client_request_id is not null;

create table if not exists event_asset_references (
  event_asset_reference_id uuid not null default gen_random_uuid(),
  session_id uuid not null,
  event_id uuid not null,
  ruleset_version_id uuid not null,
  ruleset_game_asset_id uuid not null,
  reference_role varchar(40) not null,
  payload_path varchar(160) null,
  created_at timestamptz not null default now(),
  constraint pk_event_asset_references primary key (event_asset_reference_id),
  constraint uq_event_asset_references_scope unique (
    session_id,
    event_id,
    ruleset_game_asset_id,
    reference_role
  ),
  constraint fk_event_asset_references_event_id foreign key (session_id, event_id, ruleset_version_id) references events (session_id, event_id, ruleset_version_id) on delete restrict,
  constraint fk_event_asset_references_asset_id foreign key (ruleset_version_id, ruleset_game_asset_id) references ruleset_game_assets (ruleset_version_id, ruleset_game_asset_id) on delete restrict
);

create index if not exists ix_event_asset_references_event on event_asset_references (session_id, event_id, reference_role);

create index if not exists ix_event_asset_references_asset on event_asset_references (ruleset_game_asset_id, reference_role);

do $$ begin if not exists (
  select
    1
  from
    pg_constraint
  where
    conname = 'fk_session_states_last_event_id'
) then alter table session_states add constraint fk_session_states_last_event_id foreign key (session_id, last_event_id) references events (session_id, event_id) on delete
set
  null (last_event_id);

end if;

if not exists (
  select
    1
  from
    pg_constraint
  where
    conname = 'fk_session_participant_balances_last_event_id'
) then alter table session_participant_balances add constraint fk_session_participant_balances_last_event_id foreign key (session_id, last_event_id) references events (session_id, event_id) on delete
set
  null (last_event_id);

end if;

if not exists (
  select
    1
  from
    pg_constraint
  where
    conname = 'fk_session_participant_tie_breakers_source_event_id'
) then alter table session_participant_tie_breakers add constraint fk_session_participant_tie_breakers_source_event_id foreign key (session_id, source_event_id) references events (session_id, event_id) on delete
set
  null (source_event_id);

end if;

if not exists (
  select
    1
  from
    pg_constraint
  where
    conname = 'fk_session_card_positions_last_event_id'
) then alter table session_card_positions add constraint fk_session_card_positions_last_event_id foreign key (session_id, last_event_id) references events (session_id, event_id) on delete
set
  null (last_event_id);

end if;

if not exists (
  select
    1
  from
    pg_constraint
  where
    conname = 'fk_session_participant_inventory_last_event_id'
) then alter table session_participant_inventory add constraint fk_session_participant_inventory_last_event_id foreign key (session_id, last_event_id) references events (session_id, event_id) on delete
set
  null (last_event_id);

end if;

if not exists (
  select
    1
  from
    pg_constraint
  where
    conname = 'fk_session_participant_need_purchases_source_event_id'
) then alter table session_participant_need_purchases add constraint fk_session_participant_need_purchases_source_event_id foreign key (session_id, source_event_id) references events (session_id, event_id) on delete
set
  null (source_event_id);

end if;

if not exists (
  select
    1
  from
    pg_constraint
  where
    conname = 'fk_session_participant_financial_goals_last_event_id'
) then alter table session_participant_financial_goals add constraint fk_session_participant_financial_goals_last_event_id foreign key (session_id, last_event_id) references events (session_id, event_id) on delete
set
  null (last_event_id);

end if;

if not exists (
  select
    1
  from
    pg_constraint
  where
    conname = 'fk_session_participant_collection_missions_last_event_id'
) then alter table session_participant_collection_missions add constraint fk_session_participant_collection_missions_last_event_id foreign key (session_id, last_event_id) references events (session_id, event_id) on delete
set
  null (last_event_id);

end if;

if not exists (
  select
    1
  from
    pg_constraint
  where
    conname = 'fk_session_participant_action_counters_last_event_id'
) then alter table session_participant_action_counters add constraint fk_session_participant_action_counters_last_event_id foreign key (session_id, last_event_id) references events (session_id, event_id) on delete
set
  null (last_event_id);

end if;

if not exists (
  select
    1
  from
    pg_constraint
  where
    conname = 'fk_session_participant_gold_holdings_last_event_id'
) then alter table session_participant_gold_holdings add constraint fk_session_participant_gold_holdings_last_event_id foreign key (session_id, last_event_id) references events (session_id, event_id) on delete
set
  null (last_event_id);

end if;

if not exists (
  select
    1
  from
    pg_constraint
  where
    conname = 'fk_session_participant_loans_source_event_id'
) then alter table session_participant_loans add constraint fk_session_participant_loans_source_event_id foreign key (session_id, source_event_id) references events (session_id, event_id) on delete
set
  null (source_event_id);

end if;

if not exists (
  select
    1
  from
    pg_constraint
  where
    conname = 'fk_session_participant_loans_last_event_id'
) then alter table session_participant_loans add constraint fk_session_participant_loans_last_event_id foreign key (session_id, last_event_id) references events (session_id, event_id) on delete
set
  null (last_event_id);

end if;

if not exists (
  select
    1
  from
    pg_constraint
  where
    conname = 'fk_session_participant_insurances_source_event_id'
) then alter table session_participant_insurances add constraint fk_session_participant_insurances_source_event_id foreign key (session_id, source_event_id) references events (session_id, event_id) on delete
set
  null (source_event_id);

end if;

if not exists (
  select
    1
  from
    pg_constraint
  where
    conname = 'fk_session_participant_insurances_last_event_id'
) then alter table session_participant_insurances add constraint fk_session_participant_insurances_last_event_id foreign key (session_id, last_event_id) references events (session_id, event_id) on delete
set
  null (last_event_id);

end if;

if not exists (
  select
    1
  from
    pg_constraint
  where
    conname = 'fk_session_donation_events_source_event_id'
) then alter table session_donation_events add constraint fk_session_donation_events_source_event_id foreign key (session_id, source_event_id) references events (session_id, event_id) on delete
set
  null (source_event_id);

end if;

end;

$$;

create table if not exists event_cashflow_projections (
  projection_id uuid not null default gen_random_uuid(),
  session_id uuid not null,
  user_id uuid not null,
  event_pk uuid not null,
  event_id uuid not null,
  projection_order int not null default 1,
  "timestamp" timestamptz not null,
  direction varchar(3) not null,
  amount int not null,
  category varchar(40) not null,
  counterparty varchar(40) null,
  reference varchar(120) null,
  note varchar(200) null,
  constraint pk_event_cashflow_projections primary key (projection_id),
  constraint uq_event_cashflow_projections_session_event_order unique (session_id, event_id, projection_order),
  constraint ck_event_cashflow_projections_order check (projection_order >= 1),
  constraint ck_event_cashflow_projections_direction check (direction in ('IN', 'OUT')),
  constraint ck_event_cashflow_projections_amount check (amount > 0),
  constraint fk_event_cashflow_projections_session_id foreign key (session_id) references sessions (session_id) on delete restrict,
  constraint fk_event_cashflow_projections_user_id foreign key (user_id) references app_users (user_id) on delete restrict,
  constraint fk_event_cashflow_projections_event_pk foreign key (event_pk) references events (event_pk) on delete restrict
);

create index if not exists ix_ecp_session_time on event_cashflow_projections (session_id, "timestamp" desc);

create index if not exists ix_ecp_session_user_time on event_cashflow_projections (session_id, user_id, "timestamp" desc);

create index if not exists ix_ecp_category on event_cashflow_projections (category);

create index if not exists ix_event_cashflow_projections_event_pk on event_cashflow_projections (event_pk);

create table if not exists session_projection_checkpoints (
  session_id uuid not null,
  last_sequence_number bigint not null default -1,
  last_event_id uuid null,
  projected_at timestamptz not null default now(),
  status varchar(20) not null default 'IDLE',
  rebuild_started_at timestamptz null,
  rebuild_completed_at timestamptz null,
  error_message text null,
  metadata_json jsonb not null default '{}' :: jsonb,
  constraint pk_session_projection_checkpoints primary key (session_id),
  constraint ck_session_projection_checkpoints_last_sequence check (last_sequence_number >= -1),
  constraint ck_session_projection_checkpoints_status check (
    status in ('IDLE', 'PROJECTING', 'REBUILDING', 'FAILED')
  ),
  constraint fk_session_projection_checkpoints_session_id foreign key (session_id) references sessions (session_id) on delete cascade,
  constraint fk_session_projection_checkpoints_last_event_id foreign key (session_id, last_event_id) references events (session_id, event_id) on delete
  set
    null (last_event_id)
);

create index if not exists ix_session_projection_checkpoints_status on session_projection_checkpoints (status, projected_at desc);

create table if not exists metric_snapshots (
  metric_snapshot_id uuid not null default gen_random_uuid(),
  session_id uuid not null,
  user_id uuid null,
  session_player_id uuid null,
  computed_at timestamptz not null default now(),
  metric_name varchar(120) not null,
  metric_value_numeric double precision null,
  metric_value_text text null,
  metric_value_boolean boolean null,
  metric_payload_json jsonb null,
  ruleset_version_id uuid not null,
  last_event_id uuid null,
  constraint pk_metric_snapshots primary key (metric_snapshot_id),
  constraint fk_metric_snapshots_session_id foreign key (session_id) references sessions (session_id) on delete restrict,
  constraint fk_metric_snapshots_user_id foreign key (user_id) references app_users (user_id) on delete restrict,
  constraint fk_metric_snapshots_session_player foreign key (session_id, session_player_id) references session_participants (session_id, session_participant_id) on delete
  set
    null (session_player_id),
    constraint fk_metric_snapshots_ruleset_version_id foreign key (ruleset_version_id) references ruleset_versions (ruleset_version_id) on delete restrict,
    constraint fk_metric_snapshots_last_event_id foreign key (session_id, last_event_id) references events (session_id, event_id) on delete
  set
    null (last_event_id)
);

create index if not exists ix_metrics_session_name_time on metric_snapshots (session_id, metric_name, computed_at desc);

create index if not exists ix_metrics_session_user_name_time on metric_snapshots (
  session_id,
  user_id,
  metric_name,
  computed_at desc
);

create index if not exists ix_metric_snapshots_session_player_time on metric_snapshots (session_id, session_player_id, computed_at desc);

create index if not exists ix_metrics_ruleset_version on metric_snapshots (ruleset_version_id);

create index if not exists ix_metric_snapshots_computed_at on metric_snapshots (computed_at);

create table if not exists validation_logs (
  validation_log_id uuid not null default gen_random_uuid(),
  session_id uuid not null,
  ruleset_version_id uuid null,
  event_id uuid not null,
  error_code varchar(40) null,
  error_message varchar(240) null,
  raw_payload_json jsonb not null,
  details_json jsonb null,
  created_at timestamptz not null default now(),
  constraint pk_validation_logs primary key (validation_log_id),
  constraint uq_validation_logs_session_event unique (session_id, event_id),
  constraint fk_validation_logs_session_id foreign key (session_id) references sessions (session_id) on delete cascade,
  constraint fk_validation_logs_ruleset_version_id foreign key (ruleset_version_id) references ruleset_versions (ruleset_version_id) on delete
  set
    null
);

create index if not exists ix_validation_session_time on validation_logs (session_id, created_at desc);

create index if not exists ix_validation_logs_error_code on validation_logs (error_code, created_at desc);

create index if not exists ix_validation_logs_created_at on validation_logs (created_at);

create table if not exists session_final_scores (
  session_final_score_id uuid not null default gen_random_uuid(),
  session_id uuid not null,
  session_participant_id uuid not null,
  total_points int not null,
  rank_no int not null,
  tie_breaker_number int null,
  has_unpaid_loan boolean not null default false,
  computed_at timestamptz not null default now(),
  source_event_id uuid null,
  constraint pk_session_final_scores primary key (session_final_score_id),
  constraint uq_session_final_scores_scope unique (session_id, session_final_score_id),
  constraint uq_session_final_scores_participant unique (session_id, session_participant_id),
  constraint uq_session_final_scores_component_scope unique (
    session_id,
    session_participant_id,
    session_final_score_id
  ),
  constraint uq_session_final_scores_rank unique (session_id, rank_no),
  constraint ck_session_final_scores_rank_no check (rank_no >= 1),
  constraint ck_session_final_scores_tie_breaker_number check (
    tie_breaker_number is null
    or tie_breaker_number >= 1
  ),
  constraint fk_session_final_scores_participant_id foreign key (session_id, session_participant_id) references session_participants (session_id, session_participant_id) on delete restrict,
  constraint fk_session_final_scores_source_event_id foreign key (session_id, source_event_id) references events (session_id, event_id) on delete
  set
    null (source_event_id)
);

create table if not exists session_final_score_components (
  session_final_score_component_id uuid not null default gen_random_uuid(),
  session_id uuid not null,
  session_participant_id uuid not null,
  session_final_score_id uuid not null,
  component_code varchar(80) not null,
  points int not null,
  source_event_id uuid null,
  created_at timestamptz not null default now(),
  constraint pk_session_final_score_components primary key (session_final_score_component_id),
  constraint uq_session_final_score_components_scope unique (session_final_score_id, component_code),
  constraint fk_session_final_score_components_score_id foreign key (
    session_id,
    session_participant_id,
    session_final_score_id
  ) references session_final_scores (
    session_id,
    session_participant_id,
    session_final_score_id
  ) on delete cascade,
  constraint fk_session_final_score_components_participant_id foreign key (session_id, session_participant_id) references session_participants (session_id, session_participant_id) on delete cascade,
  constraint fk_session_final_score_components_source_event_id foreign key (session_id, source_event_id) references events (session_id, event_id) on delete
  set
    null (source_event_id)
);

create table if not exists session_narrative_logs (
  narrative_log_id uuid not null default gen_random_uuid(),
  session_id uuid not null,
  session_participant_id uuid null,
  ruleset_version_id uuid not null,
  ruleset_narrative_id uuid not null,
  ruleset_narrative_scene_id uuid null,
  source_event_id uuid null,
  shown_at timestamptz not null default now(),
  day int not null,
  action_slot int not null,
  payload_json jsonb not null default '{}' :: jsonb,
  constraint pk_session_narrative_logs primary key (narrative_log_id),
  constraint uq_session_narrative_logs_trigger unique nulls not distinct (
    session_id,
    session_participant_id,
    ruleset_narrative_id,
    source_event_id
  ),
  constraint ck_session_narrative_logs_day check (day >= 1),
  constraint ck_session_narrative_logs_action_slot check (action_slot >= 1),
  constraint fk_session_narrative_logs_session_id foreign key (session_id) references sessions (session_id) on delete cascade,
  constraint fk_session_narrative_logs_participant_id foreign key (session_id, session_participant_id) references session_participants (session_id, session_participant_id) on delete
  set
    null (session_participant_id),
    constraint fk_session_narrative_logs_ruleset_narrative_id foreign key (ruleset_version_id, ruleset_narrative_id) references ruleset_narratives (ruleset_version_id, ruleset_narrative_id) on delete restrict,
    constraint fk_session_narrative_logs_ruleset_narrative_scene_id foreign key (ruleset_version_id, ruleset_narrative_scene_id) references ruleset_narrative_scenes (ruleset_version_id, ruleset_narrative_scene_id) on delete
  set
    null (ruleset_narrative_scene_id),
    constraint fk_session_narrative_logs_source_event_id foreign key (session_id, source_event_id) references events (session_id, event_id) on delete
  set
    null (source_event_id)
);

create index if not exists ix_session_narrative_logs_session_shown on session_narrative_logs (
  session_id,
  session_participant_id,
  shown_at desc
);

create table if not exists security_audit_logs (
  security_audit_log_id uuid not null default gen_random_uuid(),
  occurred_at timestamptz not null default now(),
  trace_id varchar(64) not null,
  event_type varchar(80) not null,
  outcome varchar(40) not null,
  user_id uuid null,
  username varchar(80) null,
  role varchar(20) null,
  ip_address varchar(80) null,
  user_agent varchar(300) null,
  method varchar(16) not null,
  path varchar(240) not null,
  status_code int not null,
  detail_json jsonb null,
  constraint pk_security_audit_logs primary key (security_audit_log_id),
  constraint ck_security_audit_logs_outcome check (outcome in ('SUCCESS', 'FAILURE', 'DENIED')),
  constraint ck_security_audit_logs_status_code check (
    status_code between 100
    and 599
  ),
  constraint fk_security_audit_logs_user_id foreign key (user_id) references app_users (user_id) on delete
  set
    null
);

create index if not exists ix_security_audit_logs_occurred on security_audit_logs (occurred_at desc);

create index if not exists ix_security_audit_logs_event on security_audit_logs (event_type, occurred_at desc);

create index if not exists ix_security_audit_logs_user on security_audit_logs (user_id, occurred_at desc);

create table if not exists log_retention_policies (
  table_name varchar(80) not null,
  retention_days int not null,
  created_at timestamptz not null default now(),
  updated_at timestamptz not null default now(),
  constraint pk_log_retention_policies primary key (table_name),
  constraint ck_log_retention_policies_retention_days check (retention_days >= 1)
);

create or replace function set_updated_at() returns trigger language plpgsql as $$ begin new.updated_at = now();

return new;

end;

$$;

create or replace function apply_default_log_retention_policies() returns void language plpgsql as $$ begin insert into log_retention_policies (table_name, retention_days)
values
  ('metric_snapshots', 365),
  ('validation_logs', 90),
  ('security_audit_logs', 365) on conflict (table_name) do update set retention_days = excluded.retention_days,
  updated_at = now();

end;

$$;

create or replace function purge_logs_by_retention(p_table_name varchar) returns int language plpgsql as $$ declare retention_days_value int;

deleted_rows int := 0;

purge_sql text;

begin select retention_days into retention_days_value from log_retention_policies where table_name = p_table_name;

if retention_days_value is null then return 0;

end if;

purge_sql := case
  p_table_name when 'metric_snapshots' then 'delete from metric_snapshots where computed_at < now() - make_interval(days => $1)'
  when 'validation_logs' then 'delete from validation_logs where created_at < now() - make_interval(days => $1)'
  when 'security_audit_logs' then 'delete from security_audit_logs where occurred_at < now() - make_interval(days => $1)'
  else null end;

if purge_sql is null then return 0;

end if;

execute purge_sql using retention_days_value;

get diagnostics deleted_rows = row_count;

return deleted_rows;

end;

$$;

create or replace function enforce_session_player_count_for_status() returns trigger language plpgsql as $$ declare v_min_players int := 2;

v_max_players int := 4;

begin if new.status in ('STARTED', 'ENDED') then select rgs.min_players,
rgs.max_players into v_min_players,
v_max_players from ruleset_game_settings rgs where rgs.ruleset_version_id = new.ruleset_version_id limit 1;

if coalesce(new.player_count, 0) < coalesce(v_min_players, 2)
or coalesce(new.player_count, 0) > coalesce(v_max_players, 4) then raise exception 'Session status % requires % to % participants',
new.status,
coalesce(v_min_players, 2),
coalesce(v_max_players, 4);

end if;

end if;

return new;

end;

$$;

create or replace function enforce_session_ruleset_version() returns trigger language plpgsql as $$ declare v_ruleset_mode varchar(10);

v_ruleset_status varchar(10);

begin select mode,
status into v_ruleset_mode,
v_ruleset_status from ruleset_versions where ruleset_version_id = new.ruleset_version_id;

if v_ruleset_mode is null then raise exception 'Ruleset version % not found for session',
new.ruleset_version_id;

end if;

if v_ruleset_status <> 'ACTIVE' then raise exception 'Ruleset version must be ACTIVE before session use';

end if;

if new.mode <> v_ruleset_mode then raise exception 'Session mode % does not match ruleset mode %',
new.mode,
v_ruleset_mode;

end if;

return new;

end;

$$;

create or replace function enforce_session_participant_user_role() returns trigger language plpgsql as $$ declare resolved_role varchar(20);

begin select role into resolved_role from app_users where user_id = new.user_id;

if resolved_role is distinct
from
  'PLAYER' then raise exception 'Only PLAYER accounts may join session_participants';

end if;

return new;

end;

$$;

create or replace function sync_session_player_count_from_participants() returns trigger language plpgsql as $$ declare affected_session_id uuid;

begin if tg_op = 'UPDATE'
and old.session_id is distinct
from
  new.session_id then update sessions s set player_count = (
    select
      count(*) :: int from session_participants sp where sp.session_id = old.session_id
  )
where
  s.session_id = old.session_id;

update
  sessions s
set
  player_count = (
    select
      count(*) :: int
    from
      session_participants sp
    where
      sp.session_id = new.session_id
  )
where
  s.session_id = new.session_id;

return null;

end if;

affected_session_id := coalesce(new.session_id, old.session_id);

update
  sessions s set player_count = (
    select
      count(*) :: int from session_participants sp where sp.session_id = affected_session_id
  )
where
  s.session_id = affected_session_id;

return null;

end;

$$;

create or replace function purge_ruleset_version_content(p_ruleset_version_id uuid) returns void language plpgsql as $$ begin if exists (
  select
    1 from sessions where ruleset_version_id = p_ruleset_version_id
) then raise exception 'Cannot purge ruleset version already used by a session' using errcode = '23503';

end if;

delete from
  ruleset_trigger_conditions where ruleset_version_id = p_ruleset_version_id;

delete from
  ruleset_narrative_scenes where ruleset_narrative_id in (
    select
      ruleset_narrative_id from ruleset_narratives where ruleset_version_id = p_ruleset_version_id
  );

delete from
  ruleset_order_requirements where ruleset_version_id = p_ruleset_version_id;

delete from
  ruleset_collection_mission_requirements where ruleset_version_id = p_ruleset_version_id;

delete from
  ruleset_actions
where
  ruleset_version_id = p_ruleset_version_id;

delete from
  ruleset_player_ordering_rules
where
  ruleset_version_id = p_ruleset_version_id;

delete from
  ruleset_game_settings
where
  ruleset_version_id = p_ruleset_version_id;

delete from
  ruleset_ingredients
where
  ruleset_version_id = p_ruleset_version_id;

delete from
  ruleset_orders
where
  ruleset_version_id = p_ruleset_version_id;

delete from
  ruleset_needs
where
  ruleset_version_id = p_ruleset_version_id;

delete from
  ruleset_need_set_bonuses
where
  ruleset_version_id = p_ruleset_version_id;

delete from
  ruleset_collection_missions
where
  ruleset_version_id = p_ruleset_version_id;

delete from
  ruleset_financial_goals
where
  ruleset_version_id = p_ruleset_version_id;

delete from
  ruleset_narratives
where
  ruleset_version_id = p_ruleset_version_id;

delete from
  ruleset_gold_prices
where
  ruleset_version_id = p_ruleset_version_id;

delete from
  ruleset_gold_assets
where
  ruleset_version_id = p_ruleset_version_id;

delete from
  ruleset_rank_points
where
  ruleset_version_id = p_ruleset_version_id;

delete from
  ruleset_tie_breakers
where
  ruleset_version_id = p_ruleset_version_id;

delete from
  ruleset_sharia_loans
where
  ruleset_version_id = p_ruleset_version_id;

delete from
  ruleset_insurance_products
where
  ruleset_version_id = p_ruleset_version_id;

delete from
  ruleset_life_risks
where
  ruleset_version_id = p_ruleset_version_id;

delete from
  ruleset_game_assets
where
  ruleset_version_id = p_ruleset_version_id;

end;

$$;

create or replace view ruleset_catalog_items as select rga.ruleset_game_asset_id as ruleset_catalog_item_id,
rga.ruleset_version_id,
rga.asset_type as item_type,
rga.asset_code as item_code,
rga.display_name as item_name,
rga.sort_order,
coalesce(
  ri.card_qty,
  ro.card_qty,
  rn.card_qty,
  rgp.card_qty,
  rgold.card_qty,
  rtb.card_qty,
  rlr.card_qty,
  rcm.card_qty,
  rfg.card_qty,
  rsl.card_qty,
  rip.card_qty,
  nullif(rga.metadata_json->>'card_qty', '')::int,
  1
) as card_qty,
rga.is_active
and case
  rga.asset_type when 'INGREDIENT' then coalesce(ri.is_active, false)
  when 'ORDER' then coalesce(ro.is_active, false)
  when 'NEED' then coalesce(rn.is_active, false)
  when 'GOLD_PRICE' then coalesce(rgp.is_active, false)
  when 'GOLD' then coalesce(rgold.is_active, false)
  when 'TIE_BREAKER' then rtb.ruleset_tie_breaker_id is not null
  when 'RISK' then rlr.ruleset_life_risk_id is not null
  when 'COLLECTION_MISSION' then coalesce(rcm.is_active, false)
  when 'FINANCIAL_GOAL' then coalesce(rfg.is_active, false)
  when 'SHARIA_LOAN' then true
  when 'INSURANCE' then true
  when 'DONATION_AWARD' then true
  when 'PENSION_AWARD' then true
  else false end as is_active,
  rga.metadata_json 
  || coalesce(ri.payload_json, '{}' :: jsonb) 
  || coalesce(ro.payload_json, '{}' :: jsonb) 
  || coalesce(rn.payload_json, '{}' :: jsonb) 
  || coalesce(rgp.payload_json, '{}' :: jsonb) 
  || coalesce(rgold.payload_json, '{}' :: jsonb) 
  || coalesce(rtb.payload_json, '{}' :: jsonb) 
  || coalesce(rlr.payload_json, '{}' :: jsonb)
  || coalesce(rcm.payload_json, '{}' :: jsonb)
  || coalesce(rfg.payload_json, '{}' :: jsonb)
  || coalesce(rsl.payload_json, '{}' :: jsonb)
  || coalesce(rip.payload_json, '{}' :: jsonb) as payload_json,
  rga.created_at from ruleset_game_assets rga 
  left join ruleset_ingredients ri on ri.ruleset_game_asset_id = rga.ruleset_game_asset_id 
  left join ruleset_orders ro on ro.ruleset_game_asset_id = rga.ruleset_game_asset_id 
  left join ruleset_needs rn on rn.ruleset_game_asset_id = rga.ruleset_game_asset_id 
  left join ruleset_gold_prices rgp on rgp.ruleset_game_asset_id = rga.ruleset_game_asset_id 
  left join ruleset_gold_assets rgold on rgold.ruleset_game_asset_id = rga.ruleset_game_asset_id 
  left join ruleset_tie_breakers rtb on rtb.ruleset_game_asset_id = rga.ruleset_game_asset_id 
  left join ruleset_life_risks rlr on rlr.ruleset_game_asset_id = rga.ruleset_game_asset_id
  left join ruleset_collection_missions rcm on rcm.ruleset_version_id = rga.ruleset_version_id and rcm.mission_code = rga.asset_code
  left join ruleset_financial_goals rfg on rfg.ruleset_version_id = rga.ruleset_version_id and rfg.goal_code = rga.asset_code
  left join ruleset_sharia_loans rsl on rsl.ruleset_version_id = rga.ruleset_version_id and rsl.loan_code = rga.asset_code
  left join ruleset_insurance_products rip on rip.ruleset_version_id = rga.ruleset_version_id and rip.product_code = rga.asset_code;

create or replace view ruleset_catalog_item_requirements as select ror.ruleset_order_requirement_id as requirement_id,
ro.ruleset_game_asset_id as ruleset_catalog_item_id,
ror.requirement_order,
'INGREDIENT' :: varchar(40) as requirement_type,
ror.required_asset_id,
null :: varchar(40) as required_need_tier,
ror.qty_required,
ror.payload_json from ruleset_order_requirements ror join ruleset_orders ro on ro.ruleset_version_id = ror.ruleset_version_id and ro.ruleset_order_id = ror.ruleset_order_id;

create or replace view ruleset_collection_mission_requirement_items as select rcmr.ruleset_collection_mission_requirement_id as requirement_id,
rcmr.ruleset_collection_mission_id,
rcmr.requirement_order,
rcmr.requirement_type,
rcmr.required_asset_id,
rcmr.required_need_tier,
rcmr.qty_required,
rcmr.payload_json from ruleset_collection_mission_requirements rcmr;

create or replace function enforce_session_card_position_catalog() returns trigger language plpgsql as $$ declare allowed_asset_type text := 'CARD_POSITION';

v_card_qty int;
v_total_cards int;
v_same_ingredient_count int;

begin if not exists (
  select
    1 from sessions s where s.session_id = new.session_id and s.ruleset_version_id = new.ruleset_version_id
) then raise exception 'Card ruleset version must be the active ruleset for the session' using errcode = '23514';

end if;

if not exists (
  select
    1 from ruleset_catalog_items rci where rci.ruleset_version_id = new.ruleset_version_id and rci.ruleset_catalog_item_id = new.ruleset_game_asset_id and rci.is_active and rci.card_qty > 0
) then raise exception 'Card asset % is not part of the active card catalog',
new.ruleset_game_asset_id using errcode = '23503';

end if;

select
  rci.card_qty into v_card_qty from ruleset_catalog_items rci where rci.ruleset_version_id = new.ruleset_version_id and rci.ruleset_catalog_item_id = new.ruleset_game_asset_id;

if new.copy_number > coalesce(v_card_qty, 0) then raise exception 'Card copy_number % exceeds card_qty %',
new.copy_number,
coalesce(v_card_qty, 0) using errcode = '23514';

end if;

-- 1. Ensure total active positions for this card in the session does not exceed catalog count
if new.status = 'ACTIVE' then
  select count(*) into v_total_cards
  from session_card_positions
  where session_id = new.session_id
    and ruleset_game_asset_id = new.ruleset_game_asset_id
    and status = 'ACTIVE'
    and card_position_id <> new.card_position_id;

  if (v_total_cards + 1) > coalesce(v_card_qty, 0) then
    raise exception 'Total active positions for card % exceeds card_qty %',
      new.ruleset_game_asset_id, coalesce(v_card_qty, 0) using errcode = '23514';
  end if;
end if;

-- 2. Enforce market slot limit of 5
if new.status = 'ACTIVE' and new.zone = 'MARKET' then
  if new.slot_code not in ('SLOT_1', 'SLOT_2', 'SLOT_3', 'SLOT_4', 'SLOT_5') then
    raise exception 'Market slot code % is invalid. Must be SLOT_1 to SLOT_5',
      new.slot_code using errcode = '23514';
  end if;
end if;

-- 3. Enforce maximum of 2 of the same ingredient type in the ingredient market
if new.status = 'ACTIVE' and new.zone = 'MARKET' and new.slot_group = 'INGREDIENT_MARKET' then
  select count(*) into v_same_ingredient_count
  from session_card_positions
  where session_id = new.session_id
    and zone = 'MARKET'
    and slot_group = 'INGREDIENT_MARKET'
    and ruleset_game_asset_id = new.ruleset_game_asset_id
    and status = 'ACTIVE'
    and card_position_id <> new.card_position_id;

  if (v_same_ingredient_count + 1) > 2 then
    raise exception 'Ingredient market cannot contain more than 2 cards of the same ingredient type',
      new.ruleset_game_asset_id using errcode = '23514';
  end if;
end if;

if new.zone = 'PLAYER' and new.owner_session_participant_id is null then raise exception 'PLAYER card zone requires owner_session_participant_id' using errcode = '23514';

end if;

if new.zone <> 'PLAYER' and new.owner_session_participant_id is not null then raise exception 'Non-PLAYER card zone must not have owner_session_participant_id' using errcode = '23514';

end if;

return new;

end;

$$;

create or replace function enforce_session_projection_asset_catalog() returns trigger language plpgsql as $$ declare allowed_asset_type text;

begin allowed_asset_type := case
  tg_table_name when 'session_participant_inventory' then 'INGREDIENT'
  when 'session_participant_gold_holdings' then 'GOLD'
  when 'session_participant_tie_breakers' then 'TIE_BREAKER'
  when 'session_card_positions' then 'CARD_POSITION'
  when 'event_asset_references' then 'EVENT_REFERENCE'
  else null end;

if not exists (
  select
    1 from sessions s where s.session_id = new.session_id and s.ruleset_version_id = new.ruleset_version_id
) then raise exception 'Projection ruleset version must match the session ruleset' using errcode = '23514';

end if;

if not exists (
  select
    1 from ruleset_game_assets rga where rga.ruleset_version_id = new.ruleset_version_id and rga.ruleset_game_asset_id = new.ruleset_game_asset_id and rga.is_active and (
      allowed_asset_type is null or allowed_asset_type in ('CARD_POSITION', 'EVENT_REFERENCE')
      or rga.asset_type = allowed_asset_type
    )
) then raise exception 'Projection asset % is not part of the session ruleset catalog',
new.ruleset_game_asset_id using errcode = '23503';

end if;

return new;

end;

$$;

create or replace function validate_ingredient_inventory_limits() returns trigger language plpgsql as $$
declare
  v_total_ingredients int;
  v_asset_type varchar(40);
begin
  select asset_type into v_asset_type
  from ruleset_game_assets
  where ruleset_version_id = new.ruleset_version_id
    and ruleset_game_asset_id = new.ruleset_game_asset_id;

  if v_asset_type = 'INGREDIENT' then
    if new.qty > 3 then
      raise exception 'Ingredient quantity limit exceeded: max 3 of the same type' using errcode = '23514';
    end if;

    select coalesce(sum(qty), 0) into v_total_ingredients
    from session_participant_inventory spi
    join ruleset_game_assets rga on spi.ruleset_version_id = rga.ruleset_version_id and spi.ruleset_game_asset_id = rga.ruleset_game_asset_id
    where spi.session_participant_id = new.session_participant_id
      and spi.ruleset_game_asset_id <> new.ruleset_game_asset_id
      and rga.asset_type = 'INGREDIENT';

    if v_total_ingredients + new.qty > 6 then
      raise exception 'Total ingredient limit exceeded: max 6 total cards' using errcode = '23514';
    end if;
  end if;

  return new;
end;
$$;

create or replace function enforce_event_asset_reference_catalog() returns trigger language plpgsql as $$ declare allowed_asset_type text := 'EVENT_REFERENCE';

begin if not exists (
  select
    1 from events e where e.session_id = new.session_id and e.event_id = new.event_id and e.ruleset_version_id = new.ruleset_version_id
) then raise exception 'Event asset reference must point to an event in the same ruleset' using errcode = '23514';

end if;

if not exists (
  select
    1 from sessions s where s.session_id = new.session_id and s.ruleset_version_id = new.ruleset_version_id
) then raise exception 'Event asset reference ruleset version must match session ruleset' using errcode = '23514';

end if;

if not exists (
  select
    1 from ruleset_game_assets rga where rga.ruleset_version_id = new.ruleset_version_id and rga.ruleset_game_asset_id = new.ruleset_game_asset_id and rga.is_active
) then raise exception 'Event asset reference % is not active in ruleset',
new.ruleset_game_asset_id using errcode = '23503';

end if;

return new;

end;

$$;

create or replace function enforce_event_session_scope()
returns trigger
language plpgsql
as $$ declare
  v_active_ruleset_version_id uuid;
  v_session_mode varchar(10);
  v_participant_user_id uuid;
  v_player_order_no int;
  v_action_mode varchar(10);
  v_behavior_id varchar(80);
  v_expected_weekday varchar(3);
begin
  if exists (
    select 1
    from session_projection_checkpoints spc
    where spc.session_id = new.session_id
      and spc.status in ('PROJECTING', 'REBUILDING')
  ) then
    raise exception 'Session projection is currently rebuilding or replaying'
      using errcode = '55000';
  end if;

  select s.ruleset_version_id, s.mode
  into v_active_ruleset_version_id, v_session_mode
  from sessions s
  where s.session_id = new.session_id
  limit 1;

  if v_active_ruleset_version_id is null then
    raise exception 'Event session must have an active ruleset'
      using errcode = '23514';
  end if;

  if new.ruleset_version_id is distinct from v_active_ruleset_version_id then
    raise exception 'Event ruleset version must match active session ruleset'
      using errcode = '23514';
  end if;

  -- 1. Check action type and actor type compatibility (PLAYER vs SYSTEM)
  if new.action_type in (
    'SetupModalAwal', 'SetupBahanAwal', 'SetupEmasAwal', 'SetupMisiAwal', 'SetupPinjamanAwal', 'SetupAsuransiAwal',
    'BagikanEmasAwal', 'BagikanTieBreaker', 'BagikanMisiKoleksi', 'IsiUlangPasar', 'AmbilKartuDariDeck',
    'KartuDiambilDariPasar', 'KartuMasukDiscard', 'MulaiSesi', 'AkhiriSesi', 'UmumkanJuaraDonasi',
    'PoinPeringkatDonasi', 'PoinEmas', 'PoinPeringkatPensiun'
  ) then
    if new.actor_type <> 'SYSTEM' then
      raise exception 'Action % must be executed by SYSTEM', new.action_type using errcode = '23514';
    end if;
  else
    if new.actor_type <> 'PLAYER' then
      raise exception 'Action % must be executed by PLAYER', new.action_type using errcode = '23514';
    end if;
  end if;

  if new.actor_type = 'SYSTEM' then
    if new.turn_number <> 0 then
      raise exception 'SYSTEM event must use turn_number = 0'
        using errcode = '23514';
    end if;

    if new.action_slot <> 0 then
      raise exception 'SYSTEM event must use action_slot = 0'
        using errcode = '23514';
    end if;
  end if;

  if new.actor_type = 'PLAYER' then
    if new.session_player_id is null then
      raise exception 'PLAYER event requires session_player_id'
        using errcode = '23514';
    end if;

    if new.user_id is null then
      raise exception 'PLAYER event requires user_id'
        using errcode = '23514';
    end if;

    select sp.user_id, sp.player_order_no
    into v_participant_user_id, v_player_order_no
    from session_participants sp
    where sp.session_id = new.session_id
      and sp.session_participant_id = new.session_player_id;

    if v_participant_user_id is null then
      raise exception 'Event participant must belong to the event session'
        using errcode = '23503';
    end if;

    if new.user_id is distinct from v_participant_user_id then
      raise exception 'Event user_id must match session participant user_id'
        using errcode = '23514';
    end if;

    if new.turn_number <> v_player_order_no then
      raise exception 'PLAYER event turn_number % must match player_order_no %',
        new.turn_number,
        v_player_order_no
        using errcode = '23514';
    end if;

    -- 2. Enforce free action slots check
    if (
      new.action_type in ('JumatBerkah', 'RisikoKehidupan', 'GunakanOpsiDarurat', 'InvestasiEmas', 'JualEmas', 'LewatiTransaksiEmas', 'HariMingguLibur')
      or (
        new.action_type in ('Asuransi', 'PinjamanSyariah')
        and (new.payload ? 'risk_event_id' or new.payload ? 'risk_event_ref')
      )
    ) then
      if new.action_slot <> 0 then
        raise exception 'Free action % must have action_slot = 0', new.action_type using errcode = '23514';
      end if;
    else
      if new.action_slot < 1 then
        raise exception 'Non-free PLAYER action % must have action_slot >= 1', new.action_type using errcode = '23514';
      end if;
    end if;
  end if;

  select a.mode, ra.behavior_id
  into v_action_mode, v_behavior_id
  from ruleset_actions ra
  join actions a
    on a.action_id = ra.action_id
   and a.is_active
  where ra.ruleset_version_id = new.ruleset_version_id
    and ra.ruleset_action_id = new.ruleset_action_id
    and ra.is_active;

  if v_action_mode is null then
    raise exception 'Event ruleset_action_id % is not active for ruleset version %',
      new.ruleset_action_id,
      new.ruleset_version_id
      using errcode = '23503';
  end if;

  if v_action_mode <> 'BOTH' and v_action_mode <> v_session_mode then
    raise exception 'Event ruleset_action_id % is not allowed in mode %',
      new.ruleset_action_id,
      v_session_mode
      using errcode = '23514';
  end if;

  if new.action_type is distinct from v_behavior_id then
    raise exception 'Event action_type % must match ruleset action behavior_id %',
      new.action_type,
      v_behavior_id
      using errcode = '23514';
  end if;

  if new.day_index > 0 then
    v_expected_weekday := case (((new.day_index - 1) % 7 + 7) % 7)
      when 0 then 'MON'
      when 1 then 'TUE'
      when 2 then 'WED'
      when 3 then 'THU'
      when 4 then 'FRI'
      when 5 then 'SAT'
      else 'SUN'
    end;

    if new.weekday is distinct from v_expected_weekday then
      raise exception 'Event day_index % must use weekday %',
        new.day_index,
        v_expected_weekday
        using errcode = '23514';
    end if;
  end if;

  if v_session_mode = 'PEMULA' and (
       new.action_type in ('PinjamanSyariah', 'BayarPinjaman', 'Asuransi', 'RisikoKehidupan', 'GunakanOpsiDarurat', 'Menabung', 'TujuanFinansial')
     ) then
    raise exception 'Event action % is not allowed in PEMULA mode', new.action_type
      using errcode = '23514';
  end if;

  -- 3. Payload card catalog reference checks
  if new.action_type = 'Kebutuhan' then
    if not (new.payload ? 'card_id') then
      raise exception 'Kebutuhan event payload must contain card_id' using errcode = '23514';
    end if;
    if not exists (
      select 1
      from ruleset_catalog_items
      where ruleset_version_id = new.ruleset_version_id
        and item_type = 'NEED'
        and item_code = new.payload->>'card_id'
        and is_active
    ) then
      raise exception 'Need card % is not in catalog or is inactive', new.payload->>'card_id' using errcode = '23514';
    end if;
  end if;

  if new.action_type in ('BahanMasakan', 'SetupBahanAwal') then
    if not (new.payload ? 'card_id') then
      raise exception 'BahanMasakan event payload must contain card_id' using errcode = '23514';
    end if;
    if not exists (
      select 1
      from ruleset_catalog_items
      where ruleset_version_id = new.ruleset_version_id
        and item_type = 'INGREDIENT'
        and item_code = new.payload->>'card_id'
        and is_active
    ) then
      raise exception 'Ingredient card % is not in catalog or is inactive', new.payload->>'card_id' using errcode = '23514';
    end if;
  end if;

  if new.action_type = 'JualMasakan' then
    if not (new.payload ? 'order_card_id') then
      raise exception 'JualMasakan event payload must contain order_card_id' using errcode = '23514';
    end if;
    if not exists (
      select 1
      from ruleset_catalog_items
      where ruleset_version_id = new.ruleset_version_id
        and item_type = 'ORDER'
        and item_code = new.payload->>'order_card_id'
        and is_active
    ) then
      raise exception 'Order card % is not in catalog or is inactive', new.payload->>'order_card_id' using errcode = '23514';
    end if;
  end if;

  if new.action_type in ('InvestasiEmas', 'JualEmas') then
    if not (new.payload ? 'qty' and new.payload ? 'unit_price') then
      raise exception 'Gold trade must specify qty and unit_price' using errcode = '23514';
    end if;
  end if;

  -- 4. JualMasakan ingredient requirements check
  if new.action_type = 'JualMasakan' then
    declare
      v_missing_ingredients boolean;
    begin
      select exists (
        select 1
        from ruleset_order_requirements req
        join ruleset_orders ro on ro.ruleset_order_id = req.ruleset_order_id
        left join session_participant_inventory spi
          on spi.session_participant_id = new.session_player_id
         and spi.ruleset_game_asset_id = req.required_asset_id
        where ro.ruleset_version_id = new.ruleset_version_id
          and ro.order_code = new.payload->>'order_card_id'
          and (spi.qty is null or spi.qty < req.qty_required)
      ) into v_missing_ingredients;

      if v_missing_ingredients then
        raise exception 'Player does not have required ingredients for order %', new.payload->>'order_card_id' using errcode = '23514';
      end if;
    end;
  end if;

  -- 5. Need purchases: Primer before Sekunder/Tersier, price/points match
  if new.action_type = 'Kebutuhan' then
    declare
      v_need_tier varchar(40);
      v_purchase_price int;
      v_happiness_points int;
      v_has_primary boolean;
    begin
      select need_tier, purchase_price, happiness_points
      into v_need_tier, v_purchase_price, v_happiness_points
      from ruleset_needs rn
      where rn.ruleset_version_id = new.ruleset_version_id
        and rn.need_code = new.payload->>'card_id';

      if v_need_tier is not null then
        if (new.payload->>'amount')::int <> v_purchase_price then
          raise exception 'Payment amount % does not match need card price %',
            (new.payload->>'amount')::int, v_purchase_price using errcode = '23514';
        end if;

        if (new.payload->>'points')::int <> v_happiness_points then
          raise exception 'Happiness points % does not match need card points %',
            (new.payload->>'points')::int, v_happiness_points using errcode = '23514';
        end if;

        if v_need_tier in ('sekunder', 'tersier') then
          select exists (
            select 1
            from session_participant_need_purchases spnp
            join ruleset_needs rn on rn.ruleset_need_id = spnp.ruleset_need_id
            where spnp.session_participant_id = new.session_player_id
              and rn.need_tier = 'primer'
          ) into v_has_primary;

          if not v_has_primary then
            raise exception 'Must purchase a primary need (Primer) before secondary/tertiary needs'
              using errcode = '23514';
          end if;
        end if;
      end if;
    end;
  end if;

  -- 6. MAHIR mode JualMasakan -> RisikoKehidupan sequence validation
  if v_session_mode = 'MAHIR' then
    declare
      v_last_action_type varchar(80);
      v_last_player_id uuid;
    begin
      select action_type, session_player_id
      into v_last_action_type, v_last_player_id
      from events
      where session_id = new.session_id
        and actor_type = 'PLAYER'
      order by sequence_number desc
      limit 1;

      if v_last_action_type = 'JualMasakan' then
        if new.action_type <> 'RisikoKehidupan' or new.session_player_id is distinct from v_last_player_id then
          raise exception 'JualMasakan in MAHIR mode must be followed immediately by RisikoKehidupan for the same player'
            using errcode = '23514';
        end if;
      end if;
    end;
  end if;

  -- 7. GunakanOpsiDarurat validation
  if new.action_type = 'GunakanOpsiDarurat' then
    declare
      v_risk_event_id uuid;
      v_risk_amount int;
      v_risk_direction varchar(10);
      v_current_coins int;
    begin
      v_risk_event_id := coalesce(
        (new.payload->>'risk_event_id')::uuid,
        (new.payload->>'risk_event_ref')::uuid
      );

      if v_risk_event_id is null then
        raise exception 'Emergency option must reference a risk event' using errcode = '23514';
      end if;

      select (payload->>'amount')::int, (payload->>'direction')::varchar(10)
      into v_risk_amount, v_risk_direction
      from events
      where event_id = v_risk_event_id;

      if v_risk_direction <> 'OUT' or v_risk_amount <= 0 then
        raise exception 'Emergency options can only be used for cost-based risks' using errcode = '23514';
      end if;

      select coins into v_current_coins
      from session_participant_balances
      where session_participant_id = new.session_player_id;

      if coalesce(v_current_coins, 0) >= v_risk_amount then
        raise exception 'Emergency options are only allowed when player cash is insufficient' using errcode = '23514';
      end if;

      if new.payload->>'option_type' = 'SELL_NEED' then
        declare
          v_owns_need boolean;
        begin
          select exists (
            select 1
            from session_participant_need_purchases spnp
            join ruleset_needs rn on rn.ruleset_need_id = spnp.ruleset_need_id
            where spnp.session_participant_id = new.session_player_id
              and rn.need_code = new.payload->>'card_id'
          ) into v_owns_need;

          if not v_owns_need then
            raise exception 'Player does not own need card % to sell', new.payload->>'card_id' using errcode = '23514';
          end if;
        end;
      end if;
    end;
  end if;

  -- 8. PinjamanSyariah validation (cash insufficient + active loan limit)
  if new.action_type = 'PinjamanSyariah' then
    declare
      v_loan_card_qty int;
      v_active_loans_count int;
    begin
      if (new.payload ? 'risk_event_ref' or new.payload ? 'risk_event_id') then
        declare
          v_risk_event_id uuid;
          v_risk_amount int;
          v_risk_direction varchar(10);
          v_current_coins int;
        begin
          v_risk_event_id := coalesce(
            (new.payload->>'risk_event_id')::uuid,
            (new.payload->>'risk_event_ref')::uuid
          );

          select (payload->>'amount')::int, (payload->>'direction')::varchar(10)
          into v_risk_amount, v_risk_direction
          from events
          where event_id = v_risk_event_id;

          if v_risk_direction <> 'OUT' or v_risk_amount <= 0 then
            raise exception 'Sharia loans can only be taken for cost-based risks' using errcode = '23514';
          end if;

          select coins into v_current_coins
          from session_participant_balances
          where session_participant_id = new.session_player_id;

          if coalesce(v_current_coins, 0) >= v_risk_amount then
            raise exception 'Sharia loans with risk reference are only allowed when player cash is insufficient' using errcode = '23514';
          end if;
        end;
      end if;

      select card_qty into v_loan_card_qty
      from ruleset_catalog_items
      where ruleset_version_id = new.ruleset_version_id
        and item_type = 'SHARIA_LOAN'
        and item_code = new.payload->>'loan_code';

      select count(*) into v_active_loans_count
      from session_participant_loans spl
      join ruleset_sharia_loans rsl on rsl.ruleset_sharia_loan_id = spl.ruleset_sharia_loan_id
      where spl.session_id = new.session_id
        and rsl.loan_code = new.payload->>'loan_code'
        and spl.status = 'ACTIVE';

      if (v_active_loans_count + 1) > coalesce(v_loan_card_qty, 0) then
        raise exception 'Active loan count for % exceeds catalog limit %',
          new.payload->>'loan_code', coalesce(v_loan_card_qty, 0) using errcode = '23514';
      end if;
    end;
  end if;

  -- 9. Insurance validation (reject global/SYSTEM risks + ACTIVE status / uses remaining check)
  if new.action_type = 'Asuransi' and (new.payload ? 'risk_event_id' or new.payload ? 'risk_event_ref') then
    declare
      v_risk_event_id uuid;
      v_actor_type varchar(10);
      v_ins_status varchar(20);
      v_ins_uses int;
    begin
      v_risk_event_id := coalesce(
        (new.payload->>'risk_event_id')::uuid,
        (new.payload->>'risk_event_ref')::uuid
      );

      select actor_type into v_actor_type
      from events
      where session_id = new.session_id
        and event_id = v_risk_event_id;

      if v_actor_type = 'SYSTEM' then
        raise exception 'Insurance cannot be used to mitigate global (SYSTEM) risks'
          using errcode = '23514';
      end if;

      select status, remaining_uses
      into v_ins_status, v_ins_uses
      from session_participant_insurances
      where session_participant_id = new.session_player_id
        and status = 'ACTIVE';

      if v_ins_status is null or v_ins_uses < 1 then
        raise exception 'Insurance claim requires active policy with remaining uses' using errcode = '23514';
      end if;
    end;
  end if;

  -- 10. Menabung validation (max 15 coins per action)
  if new.action_type = 'Menabung' then
    declare
      v_amount int;
    begin
      if not (new.payload ? 'amount') then
        raise exception 'Menabung event payload must contain amount' using errcode = '23514';
      end if;
      v_amount := (new.payload->>'amount')::int;
      if v_amount < 1 or v_amount > 15 then
        raise exception 'Saving amount % must be between 1 and 15 coins per action', v_amount using errcode = '23514';
      end if;
    end;
  end if;

  if new.action_type in ('JumatBerkah', 'PoinPeringkatDonasi', 'UmumkanJuaraDonasi') and new.weekday <> 'FRI' then
    raise exception 'Donation events must occur on FRI'
      using errcode = '23514';
  end if;

  if new.action_type in ('InvestasiEmas', 'JualEmas', 'LewatiTransaksiEmas') and new.weekday <> 'SAT' then
    raise exception 'Gold trade events must occur on SAT'
      using errcode = '23514';
  end if;

  if new.action_type = 'HariMingguLibur' and new.weekday <> 'SUN' then
    raise exception 'Sunday rest event must occur on SUN'
      using errcode = '23514';
  end if;

  if new.actor_type = 'PLAYER' and new.weekday = 'SUN' then
    raise exception 'Player events are not allowed on SUN'
      using errcode = '23514';
  end if;

  return new;
end;
$$;

create or replace function enforce_action_slot_within_ruleset_limit()
returns trigger
language plpgsql
as $$ declare
  v_ruleset_version_id uuid;
  v_actions_per_turn int;
begin
  -- Equivalent guard: tg_table_name = 'events' and new.actor_type = 'SYSTEM'.
  -- Keep nested access so non-events triggers do not resolve a missing actor_type field.
  if tg_table_name = 'events' then
    if new.actor_type = 'SYSTEM' then
      return new;
    end if;
  end if;

  if tg_table_name = 'session_states' and coalesce(new.turn_number, 0) = 0 then
    return new;
  end if;

  if tg_table_name = 'session_states' then
    select s.ruleset_version_id
    into v_ruleset_version_id
    from sessions s
    where s.session_id = new.session_id;
  else
    v_ruleset_version_id := new.ruleset_version_id;
  end if;

  select rgs.actions_per_turn
  into v_actions_per_turn
  from ruleset_game_settings rgs
  where rgs.ruleset_version_id = v_ruleset_version_id;

  if v_actions_per_turn is null then
    raise exception 'Cannot resolve actions_per_turn for action slot validation'
      using errcode = '23514';
  end if;

  if tg_table_name = 'events'
     and new.action_slot = 0
     and (
       new.action_type in ('JumatBerkah', 'RisikoKehidupan', 'GunakanOpsiDarurat', 'InvestasiEmas', 'JualEmas', 'LewatiTransaksiEmas', 'HariMingguLibur')
       or (
         new.action_type in ('Asuransi', 'PinjamanSyariah')
         and (new.payload ? 'risk_event_id' or new.payload ? 'risk_event_ref')
       )
     ) then
    return new;
  end if;

  if new.action_slot < 1 then
    raise exception 'PLAYER action_slot must be >= 1'
      using errcode = '23514';
  end if;

  if new.action_slot > v_actions_per_turn then
    raise exception 'action_slot % exceeds actions_per_turn %',
      new.action_slot,
      v_actions_per_turn
      using errcode = '23514';
  end if;

  if tg_table_name = 'session_states' then
    if new.current_action_slot > v_actions_per_turn then
      raise exception 'current_action_slot % exceeds actions_per_turn %',
        new.current_action_slot,
        v_actions_per_turn
        using errcode = '23514';
    end if;
  end if;

  return new;
end;
$$;

create or replace function enforce_event_cashflow_projection_consistency() returns trigger language plpgsql as $$ declare v_session_id uuid;

v_event_id uuid;

begin select e.session_id,
e.event_id into v_session_id,
v_event_id from events e where e.event_pk = new.event_pk;

if v_session_id is null then raise exception 'Cashflow projection event_pk % does not exist',
new.event_pk using errcode = '23503';

end if;

if new.session_id is distinct
from
  v_session_id
  or new.event_id is distinct
from
  v_event_id then raise exception 'Cashflow projection event identity does not match events row' using errcode = '23514';

end if;

if not exists (
  select
    1 from session_participants sp where sp.session_id = new.session_id and sp.user_id = new.user_id
) then raise exception 'Cashflow projection user_id must belong to the projection session' using errcode = '23514';

end if;

return new;

end;

$$;

create or replace function validate_player_cashflow_running_balances() returns trigger language plpgsql as $$
declare
  v_running_balance int := 0;
  v_rec record;
begin
  select rgs.starting_cash
  into v_running_balance
  from session_participants sp
  join sessions s on s.session_id = sp.session_id
  join ruleset_game_settings rgs on rgs.ruleset_version_id = s.ruleset_version_id
  where sp.session_id = new.session_id
    and sp.user_id = new.user_id;

  v_running_balance := coalesce(v_running_balance, 0);

  for v_rec in
    select 
      ecp.direction,
      ecp.amount,
      e.sequence_number,
      ecp.projection_order
    from event_cashflow_projections ecp
    join events e on ecp.event_pk = e.event_pk
    where ecp.session_id = new.session_id
      and ecp.user_id = new.user_id
    order by e.sequence_number asc, ecp.projection_order asc
  loop
    if v_rec.direction = 'IN' then
      v_running_balance := v_running_balance + v_rec.amount;
    elsif v_rec.direction = 'OUT' then
      v_running_balance := v_running_balance - v_rec.amount;
    end if;

    if v_running_balance < 0 then
      raise exception 'Insufficent cash balance: running balance would drop to % coins', v_running_balance using errcode = '23514';
    end if;
  end loop;

  return new;
end;
$$;

create or replace function enforce_ruleset_order_requirement_asset_type() returns trigger language plpgsql as $$ begin if not exists (
  select
    1 from ruleset_game_assets rga where rga.ruleset_version_id = new.ruleset_version_id and rga.ruleset_game_asset_id = new.required_asset_id and rga.asset_type = 'INGREDIENT'
    and rga.is_active
) then raise exception 'Order requirement asset must be INGREDIENT' using errcode = '23514';

end if;

return new;

end;

$$;

create or replace function enforce_collection_mission_requirement_asset_type() returns trigger language plpgsql as $$ begin if new.requirement_type = 'ASSET'
and not exists (
  select
    1 from ruleset_game_assets rga where rga.ruleset_version_id = new.ruleset_version_id and rga.ruleset_game_asset_id = new.required_asset_id and rga.asset_type = 'NEED'
    and rga.is_active
) then raise exception 'Collection mission asset requirement must reference NEED' using errcode = '23514';

end if;

return new;

end;

$$;

create or replace function project_session_event(p_event_pk uuid) returns void language plpgsql as $$ declare v_session_id uuid;

v_sequence_number bigint;

v_event_id uuid;

begin select session_id,
sequence_number,
event_id into v_session_id,
v_sequence_number,
v_event_id from events where event_pk = p_event_pk;

if v_session_id is null then raise exception 'Event % not found for projection',
p_event_pk using errcode = '23503';

end if;

insert into session_projection_checkpoints (
    session_id,
    last_sequence_number,
    last_event_id,
    projected_at,
    status,
    metadata_json
  )
values
  (
    v_session_id,
    v_sequence_number,
    v_event_id,
    now(),
    'IDLE',
    jsonb_build_object('source', 'project_session_event')
  ) on conflict (session_id) do update set last_sequence_number = greatest(
    session_projection_checkpoints.last_sequence_number,
    excluded.last_sequence_number
  ),
  last_event_id = case when excluded.last_sequence_number >= session_projection_checkpoints.last_sequence_number then excluded.last_event_id else session_projection_checkpoints.last_event_id end,
  projected_at = now(),
  status = 'IDLE',
  error_message = null,
  metadata_json = session_projection_checkpoints.metadata_json || excluded.metadata_json;

end;

$$;

create or replace function project_session_events(
  p_session_id uuid,
  p_from_sequence bigint default null
) returns int language plpgsql as $$ declare v_from_sequence bigint;

v_projected int := 0;

v_event record;

begin perform pg_advisory_xact_lock(hashtextextended(p_session_id :: text, 0));

select
  coalesce(p_from_sequence, last_sequence_number + 1, 0) into v_from_sequence from session_projection_checkpoints where session_id = p_session_id;

v_from_sequence := coalesce(v_from_sequence, 0);

insert into session_projection_checkpoints (
    session_id,
    last_sequence_number,
    status,
    projected_at
  )
values
  (
    p_session_id,
    v_from_sequence - 1,
    'PROJECTING',
    now()
  ) on conflict (session_id) do update set status = 'PROJECTING',
  projected_at = now(),
  error_message = null;

for v_event in select event_pk from events where session_id = p_session_id and sequence_number >= v_from_sequence order by sequence_number loop perform project_session_event(v_event.event_pk);

v_projected := v_projected + 1;

end loop;

update
  session_projection_checkpoints set status = 'IDLE',
  projected_at = now()
where
  session_id = p_session_id;

return v_projected;

exception when others then insert into session_projection_checkpoints (
  session_id,
  last_sequence_number,
  status,
  projected_at,
  error_message
)
values
  (p_session_id, -1, 'FAILED', now(), sqlerrm) on conflict (session_id) do update set status = 'FAILED',
  projected_at = now(),
  error_message = excluded.error_message;

raise;

end;

$$;

create or replace function rebuild_session_projection(p_session_id uuid) returns int language plpgsql as $$ begin perform pg_advisory_xact_lock(hashtextextended(p_session_id :: text, 0));

if not exists (
  select
    1 from sessions where session_id = p_session_id
) then raise exception 'Session % not found for projection rebuild',
p_session_id using errcode = '23503';

end if;

raise exception 'SQL-only projection rebuild is disabled to prevent destructive state loss; reset and reseed the canonical baseline instead' using errcode = '0A000';

end;

$$;

create or replace function compute_schema_fingerprint() returns text language sql stable as $$
select
  encode(
    digest(
      string_agg(
        entry,
        E'\n'
        order by
          entry
      ),
      'sha256'
    ),
    'hex'
  )
from
  (
    select
      concat_ws(
        ':',
        'column',
        table_schema,
        table_name,
        column_name,
        ordinal_position,
        udt_name,
        is_nullable,
        column_default
      ) as entry from information_schema.columns where table_schema = 'public'
    union
    all select concat_ws(
      ':',
      'constraint',
      conrelid :: regclass :: text,
      conname,
      contype,
      pg_get_constraintdef(oid)
    ) as entry from pg_constraint where connamespace = 'public' :: regnamespace union all select concat_ws(
      ':',
      'index',
      schemaname,
      tablename,
      indexname,
      indexdef
    ) as entry from pg_indexes where schemaname = 'public'
    union
    all select concat_ws(
      ':',
      'trigger',
      tgrelid :: regclass :: text,
      tgname,
      pg_get_triggerdef(oid)
    ) as entry from pg_trigger where not tgisinternal union all select concat_ws(
      ':',
      'function',
      p.oid :: regprocedure :: text,
      pg_get_functiondef(p.oid)
    ) as entry from pg_proc p join pg_namespace n on n.oid = p.pronamespace where n.nspname = 'public'
    and p.prokind in ('f', 'p')
    and not exists (
      select
        1 from pg_depend d where d.classid = 'pg_proc' :: regclass and d.objid = p.oid and d.deptype = 'e'
    )
    union
    all select concat_ws(':', 'view', schemaname, viewname, definition) as entry from pg_views where schemaname = 'public'
  ) fingerprint_parts;

$$;

create or replace function assert_schema_baseline(
  p_baseline_name varchar,
  p_schema_version varchar
) returns void language plpgsql as $$ declare v_checksum varchar(128);

v_existing varchar(128);

begin v_checksum := compute_schema_fingerprint();

select
  checksum into v_existing from schema_baseline_versions where baseline_name = p_baseline_name and schema_version = p_schema_version;

if v_existing is null then insert into schema_baseline_versions (
  baseline_name,
  schema_version,
  checksum,
  applied_at
)
values
  (
    p_baseline_name,
    p_schema_version,
    v_checksum,
    now()
  );

return;

end if;

if v_existing <> v_checksum then raise exception 'Schema fingerprint mismatch for baseline %. Expected %, got %',
p_baseline_name,
v_existing,
v_checksum using errcode = 'P0001';

end if;

end;

$$;

drop trigger if exists trg_session_states_updated_at on session_states;

create trigger trg_session_states_updated_at before
update
  on session_states for each row execute function set_updated_at();

drop trigger if exists trg_session_states_action_slot_limit on session_states;

create trigger trg_session_states_action_slot_limit
before insert or update of action_slot, current_action_slot, turn_number, session_id
on session_states
for each row execute function enforce_action_slot_within_ruleset_limit();

drop trigger if exists trg_sessions_player_count_status on sessions;

create trigger trg_sessions_player_count_status before
insert
  or
update
  of status,
  player_count,
  ruleset_version_id,
  mode on sessions for each row execute function enforce_session_player_count_for_status();

drop trigger if exists trg_sessions_ruleset_version_validate on sessions;

create trigger trg_sessions_ruleset_version_validate before
insert
  or
update
  of ruleset_version_id,
  mode on sessions for each row execute function enforce_session_ruleset_version();

drop trigger if exists trg_ruleset_game_settings_updated_at on ruleset_game_settings;

create trigger trg_ruleset_game_settings_updated_at before
update
  on ruleset_game_settings for each row execute function set_updated_at();

drop trigger if exists trg_session_participant_balances_updated_at on session_participant_balances;

create trigger trg_session_participant_balances_updated_at before
update
  on session_participant_balances for each row execute function set_updated_at();

drop trigger if exists trg_session_participant_inventory_updated_at on session_participant_inventory;

create trigger trg_session_participant_inventory_updated_at before
update
  on session_participant_inventory for each row execute function set_updated_at();

drop trigger if exists trg_session_participant_inventory_catalog on session_participant_inventory;

create trigger trg_session_participant_inventory_catalog before
insert
  or
update
  of session_id,
  ruleset_version_id,
  ruleset_game_asset_id on session_participant_inventory for each row execute function enforce_session_projection_asset_catalog();

drop trigger if exists trg_session_participant_inventory_limits on session_participant_inventory;

create trigger trg_session_participant_inventory_limits before
insert
  or
update
  on session_participant_inventory for each row execute function validate_ingredient_inventory_limits();

drop trigger if exists trg_session_participant_gold_holdings_updated_at on session_participant_gold_holdings;

create trigger trg_session_participant_gold_holdings_updated_at before
update
  on session_participant_gold_holdings for each row execute function set_updated_at();

drop trigger if exists trg_session_participant_gold_holdings_catalog on session_participant_gold_holdings;

create trigger trg_session_participant_gold_holdings_catalog before
insert
  or
update
  of session_id,
  ruleset_version_id,
  ruleset_game_asset_id on session_participant_gold_holdings for each row execute function enforce_session_projection_asset_catalog();

drop trigger if exists trg_session_participant_loans_updated_at on session_participant_loans;

create trigger trg_session_participant_loans_updated_at before
update
  on session_participant_loans for each row execute function set_updated_at();

drop trigger if exists trg_session_participant_insurances_updated_at on session_participant_insurances;

create trigger trg_session_participant_insurances_updated_at before
update
  on session_participant_insurances for each row execute function set_updated_at();

drop trigger if exists trg_session_participant_financial_goals_updated_at on session_participant_financial_goals;

create trigger trg_session_participant_financial_goals_updated_at before
update
  on session_participant_financial_goals for each row execute function set_updated_at();

drop trigger if exists trg_session_participant_collection_missions_updated_at on session_participant_collection_missions;

create trigger trg_session_participant_collection_missions_updated_at before
update
  on session_participant_collection_missions for each row execute function set_updated_at();

drop trigger if exists trg_session_card_positions_updated_at on session_card_positions;

create trigger trg_session_card_positions_updated_at before
update
  on session_card_positions for each row execute function set_updated_at();

drop trigger if exists trg_session_card_positions_catalog on session_card_positions;

create trigger trg_session_card_positions_catalog before
insert
  or
update
  of session_id,
  ruleset_version_id,
  ruleset_game_asset_id,
  copy_number,
  zone,
  owner_session_participant_id on session_card_positions for each row execute function enforce_session_card_position_catalog();

drop trigger if exists trg_events_session_scope on events;

create trigger trg_events_session_scope
before insert or update of session_id, session_player_id, user_id, ruleset_action_id, action_type, ruleset_version_id, weekday, actor_type, turn_number, action_slot
on events
for each row execute function enforce_event_session_scope();

drop trigger if exists trg_events_action_slot_limit on events;

create trigger trg_events_action_slot_limit
before insert or update of session_id, ruleset_version_id, action_slot, actor_type
on events
for each row execute function enforce_action_slot_within_ruleset_limit();

drop trigger if exists trg_session_narrative_logs_action_slot_limit on session_narrative_logs;

create trigger trg_session_narrative_logs_action_slot_limit before
insert
  or
update
  of session_id,
  ruleset_version_id,
  action_slot on session_narrative_logs for each row execute function enforce_action_slot_within_ruleset_limit();

drop trigger if exists trg_event_asset_references_catalog on event_asset_references;

create trigger trg_event_asset_references_catalog before
insert
  or
update
  of session_id,
  event_id,
  ruleset_version_id,
  ruleset_game_asset_id on event_asset_references for each row execute function enforce_event_asset_reference_catalog();

drop trigger if exists trg_event_cashflow_projection_consistency on event_cashflow_projections;

create trigger trg_event_cashflow_projection_consistency before
insert
  or
update
  of session_id,
  user_id,
  event_pk,
  event_id on event_cashflow_projections for each row execute function enforce_event_cashflow_projection_consistency();

drop trigger if exists trg_validate_player_cashflow_running_balances on event_cashflow_projections;

create trigger trg_validate_player_cashflow_running_balances after
insert
  or
update
  on event_cashflow_projections for each row execute function validate_player_cashflow_running_balances();

drop trigger if exists trg_ruleset_order_requirements_asset_type on ruleset_order_requirements;

create trigger trg_ruleset_order_requirements_asset_type before
insert
  or
update
  of ruleset_version_id,
  required_asset_id on ruleset_order_requirements for each row execute function enforce_ruleset_order_requirement_asset_type();

drop trigger if exists trg_ruleset_collection_mission_requirements_asset_type on ruleset_collection_mission_requirements;

create trigger trg_ruleset_collection_mission_requirements_asset_type before
insert
  or
update
  of ruleset_version_id,
  requirement_type,
  required_asset_id on ruleset_collection_mission_requirements for each row execute function enforce_collection_mission_requirement_asset_type();

drop trigger if exists trg_ruleset_game_assets_updated_at on ruleset_game_assets;

create trigger trg_ruleset_game_assets_updated_at before
update
  on ruleset_game_assets for each row execute function set_updated_at();

drop trigger if exists trg_session_participants_user_role on session_participants;

create trigger trg_session_participants_user_role before
insert
  or
update
  on session_participants for each row execute function enforce_session_participant_user_role();

drop trigger if exists trg_session_participants_player_count_insert on session_participants;

create trigger trg_session_participants_player_count_insert after
insert
  or
update
  or delete on session_participants for each row execute function sync_session_player_count_from_participants();

create or replace function validate_ruleset_component_counts(p_ruleset_version_id uuid)
returns void language plpgsql as $$
declare
  v_ingredient_total int;
  v_need_total int;
  v_risk_total int;
  v_donation_award_total int;
  v_pension_award_total int;
  v_order_total int;
  v_gold_price_total int;
  v_gold_total int;
  v_sharia_loan_total int;
  v_financial_goal_total int;
  v_collection_mission_total int;
begin
  -- 1. Ingredients total cards must be exactly 25 (5 copies of 5 ingredient types)
  select coalesce(sum(card_qty), 0) into v_ingredient_total
  from ruleset_ingredients
  where ruleset_version_id = p_ruleset_version_id;

  if v_ingredient_total <> 25 then
    raise exception 'Component audit failed: Total ingredients must be exactly 25, got %', v_ingredient_total using errcode = '23514';
  end if;

  -- 2. Needs total cards must be exactly 25
  select coalesce(sum(card_qty), 0) into v_need_total
  from ruleset_needs
  where ruleset_version_id = p_ruleset_version_id;

  if v_need_total <> 25 then
    raise exception 'Component audit failed: Total needs must be exactly 25, got %', v_need_total using errcode = '23514';
  end if;

  -- 3. Physical life risks must be exactly 24
  select coalesce(sum(card_qty), 0) into v_risk_total
  from ruleset_life_risks
  where ruleset_version_id = p_ruleset_version_id;

  if v_risk_total <> 24 then
    raise exception 'Component audit failed: Total physical life risks must be exactly 24, got %', v_risk_total using errcode = '23514';
  end if;

  -- 4. DONATION_AWARD cards must be exactly 3 per rank (total 9)
  select coalesce(sum(coalesce(nullif(metadata_json->>'card_qty', '')::int, 1)), 0) into v_donation_award_total
  from ruleset_game_assets
  where ruleset_version_id = p_ruleset_version_id
    and asset_type = 'DONATION_AWARD';

  if v_donation_award_total <> 9 then
    raise exception 'Component audit failed: Total donation award cards must be exactly 9, got %', v_donation_award_total using errcode = '23514';
  end if;

  -- 5. PENSION_AWARD cards must be exactly 1 per rank (total 3)
  select coalesce(sum(coalesce(nullif(metadata_json->>'card_qty', '')::int, 1)), 0) into v_pension_award_total
  from ruleset_game_assets
  where ruleset_version_id = p_ruleset_version_id
    and asset_type = 'PENSION_AWARD';

  if v_pension_award_total <> 3 then
    raise exception 'Component audit failed: Total pension award cards must be exactly 3, got %', v_pension_award_total using errcode = '23514';
  end if;

  -- 6. Orders total cards must be exactly 25
  select coalesce(sum(card_qty), 0) into v_order_total
  from ruleset_orders
  where ruleset_version_id = p_ruleset_version_id;

  if v_order_total <> 25 then
    raise exception 'Component audit failed: Total orders must be exactly 25, got %', v_order_total using errcode = '23514';
  end if;

  -- 7. Gold price cards must be exactly 6
  select coalesce(sum(card_qty), 0) into v_gold_price_total
  from ruleset_gold_prices
  where ruleset_version_id = p_ruleset_version_id;

  if v_gold_price_total <> 6 then
    raise exception 'Component audit failed: Total gold price cards must be exactly 6, got %', v_gold_price_total using errcode = '23514';
  end if;

  -- 8. Physical gold cards must be exactly 20 (read from metadata_json to match ruleset_gold_assets card_qty)
  select coalesce(sum(coalesce(nullif(metadata_json->>'card_qty', '')::int, 1)), 0) into v_gold_total
  from ruleset_game_assets
  where ruleset_version_id = p_ruleset_version_id
    and asset_type = 'GOLD';

  if v_gold_total <> 20 then
    raise exception 'Component audit failed: Total physical gold cards must be exactly 20, got %', v_gold_total using errcode = '23514';
  end if;

  -- 9. Sharia loan cards must be exactly 8
  select coalesce(sum(card_qty), 0) into v_sharia_loan_total
  from ruleset_sharia_loans
  where ruleset_version_id = p_ruleset_version_id;

  if v_sharia_loan_total <> 8 then
    raise exception 'Component audit failed: Total sharia loan cards must be exactly 8, got %', v_sharia_loan_total using errcode = '23514';
  end if;

  -- 10. Financial goal cards must be exactly 5
  select coalesce(sum(card_qty), 0) into v_financial_goal_total
  from ruleset_financial_goals
  where ruleset_version_id = p_ruleset_version_id;

  if v_financial_goal_total <> 5 then
    raise exception 'Component audit failed: Total financial goal cards must be exactly 5, got %', v_financial_goal_total using errcode = '23514';
  end if;

  -- 11. Collection mission cards must be exactly 4
  select coalesce(sum(card_qty), 0) into v_collection_mission_total
  from ruleset_collection_missions
  where ruleset_version_id = p_ruleset_version_id;

  if v_collection_mission_total <> 4 then
    raise exception 'Component audit failed: Total collection mission cards must be exactly 4, got %', v_collection_mission_total using errcode = '23514';
  end if;
end;
$$;

select
  apply_default_log_retention_policies();

select
  assert_schema_baseline('canonical_relational_baseline', '3.0.0');

commit;

