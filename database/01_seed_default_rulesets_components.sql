create extension if not exists pgcrypto;

begin;

-- ============================================================
-- 1. MENU DAN HAK AKSES
-- ============================================================

insert into app_menus (menu_code, menu_name, path, sort_order, is_active)
values
  ('dashboard_analytics', 'Dashboard Analytics', '/dashboard/analytics', 1, true),
  ('sessions', 'Sesi Permainan', '/sessions', 2, true),
  ('rulesets', 'Ruleset', '/rulesets', 3, true),
  ('events', 'Event Log', '/events', 4, true),
  ('validations', 'Validasi Event', '/validations', 5, true),
  ('player_dashboard', 'Dashboard Pemain', '/player/dashboard', 6, true),
  ('player_history', 'Histori Keputusan Pemain', '/player/history', 7, true)
on conflict (menu_code) do update
set menu_name = excluded.menu_name,
    path = excluded.path,
    sort_order = excluded.sort_order,
    is_active = excluded.is_active;

insert into role_menu_permissions (role, menu_id, can_view, can_create, can_update, can_delete)
select
  'INSTRUCTOR',
  menu_id,
  true,
  case when menu_code in ('sessions', 'rulesets') then true else false end,
  case when menu_code in ('sessions', 'rulesets') then true else false end,
  case when menu_code in ('sessions', 'rulesets') then true else false end
from app_menus
on conflict (role, menu_id) do update
set can_view = excluded.can_view,
    can_create = excluded.can_create,
    can_update = excluded.can_update,
    can_delete = excluded.can_delete;

insert into role_menu_permissions (role, menu_id, can_view, can_create, can_update, can_delete)
select 'PLAYER', menu_id, true, false, false, false
from app_menus
where menu_code in ('player_dashboard', 'player_history')
on conflict (role, menu_id) do update
set can_view = excluded.can_view,
    can_create = excluded.can_create,
    can_update = excluded.can_update,
    can_delete = excluded.can_delete;

-- ============================================================
-- 2. MASTER ACTION, INGREDIENT, DAN KOMPONEN
-- ============================================================

insert into actions (
  action_id,
  action_name,
  mode,
  cashflow_direction,
  affects_coin,
  affects_happiness,
  affects_saving,
  affects_inventory,
  affects_quest,
  is_active
)
values
  ('BahanMasakan', 'Beli Bahan Masakan', 'BOTH', 'OUT', true, false, false, true, true, true),
  ('ingredient.discarded', 'Buang Bahan Masakan', 'BOTH', null, false, false, false, true, false, true),
  ('JualMasakan', 'Jual Masakan', 'BOTH', 'IN', true, false, false, true, true, true),
  ('order.passed', 'Lewati Order', 'BOTH', null, false, false, false, false, false, true),
  ('Kebutuhan', 'Beli Kebutuhan', 'BOTH', 'OUT', true, true, false, false, false, true),
  ('KerjaLepas', 'Kerja Lepas', 'BOTH', 'IN', true, false, false, false, false, true),
  ('transaction.recorded', 'Catat Transaksi', 'BOTH', null, true, false, true, false, false, true),
  ('Menabung', 'Menabung', 'BOTH', 'OUT', false, false, true, false, false, true),
  ('saving.deposit.withdrawn', 'Tarik Tabungan', 'BOTH', 'IN', true, false, true, false, false, true),
  ('TujuanFinansial', 'Tujuan Finansial', 'BOTH', 'OUT', true, true, true, false, false, true),
  ('JumatBerkah', 'Peduli Donasi', 'BOTH', 'OUT', true, false, false, false, false, true),
  ('InvestasiEmas', 'Investasi Emas', 'BOTH', 'OUT', true, false, false, false, false, true),
  ('JualEmas', 'Jual Emas', 'BOTH', 'IN', true, false, false, false, false, true),
  ('PinjamanSyariah', 'Pinjaman Syariah', 'MAHIR', 'IN', true, false, false, false, false, true),
  ('BayarPinjaman', 'Bayar Pinjaman', 'MAHIR', 'OUT', true, false, false, false, false, true),
  ('Asuransi', 'Asuransi', 'MAHIR', 'OUT', true, false, false, false, false, true),
  ('RisikoKehidupan', 'Risiko Kehidupan', 'MAHIR', null, true, false, false, false, false, true),
  ('risk.emergency.used', 'Gunakan Opsi Darurat', 'MAHIR', null, true, false, false, false, false, true),
  ('donation.rank.awarded', 'Poin Peringkat Donasi', 'BOTH', null, false, true, false, false, false, true),
  ('gold.points.awarded', 'Poin Emas', 'MAHIR', null, false, true, false, false, false, true),
  ('pension.rank.awarded', 'Poin Peringkat Pensiun', 'MAHIR', null, false, true, false, false, false, true),
  ('TieBreakerAssigned', 'Sistem: Bagikan Tie Breaker', 'BOTH', null, false, false, false, false, false, true),
  ('MissionAssigned', 'Sistem: Bagikan Misi Koleksi', 'BOTH', null, false, false, false, false, false, true),
  ('SessionEnded', 'Sistem: Akhiri Sesi', 'BOTH', null, false, false, false, false, false, true),
  ('AkhirGiliran', 'Akhir Giliran', 'BOTH', null, false, false, false, false, false, true)
on conflict (action_id) do update
set action_name = excluded.action_name,
    mode = excluded.mode,
    cashflow_direction = excluded.cashflow_direction,
    affects_coin = excluded.affects_coin,
    affects_happiness = excluded.affects_happiness,
    affects_saving = excluded.affects_saving,
    affects_inventory = excluded.affects_inventory,
    affects_quest = excluded.affects_quest,
    is_active = excluded.is_active;

insert into ingredients (ingredient_id, ingredient_name, display_name, is_active)
values
  ('nasi_putih', 'nasi_putih', 'Nasi Putih', true),
  ('sayur', 'sayur', 'Sayur', true),
  ('tahu_tempe', 'tahu_tempe', 'Tahu Tempe', true),
  ('telur', 'telur', 'Telur', true),
  ('daging', 'daging', 'Daging', true)
on conflict (ingredient_id) do update
set ingredient_name = excluded.ingredient_name,
    display_name = excluded.display_name,
    is_active = excluded.is_active;

insert into game_components (component_code, component_name, component_type, mode, quantity, metadata_json, is_active)
values
  ('board_main', 'Board Permainan', 'BOARD', 'BOTH', 1, '{"surface":"main"}', true),
  ('coin_stack', 'Set Koin', 'COIN', 'BOTH', 100, '{"nominals":[1,5,10]}', true),
  ('recipe_cards', 'Kartu Resep', 'CARD', 'BOTH', 40, '{"category":"recipe"}', true),
  ('advanced_risk_cards', 'Kartu Risiko Kehidupan', 'CARD', 'MAHIR', 12, '{"category":"life_risk"}', true)
on conflict (component_code) do update
set component_name = excluded.component_name,
    component_type = excluded.component_type,
    mode = excluded.mode,
    quantity = excluded.quantity,
    metadata_json = excluded.metadata_json,
    is_active = excluded.is_active;

-- ============================================================
-- 3. RULESET DEFAULT BERBASIS CONFIG_JSON
-- ============================================================

create temporary table seed_rulesets (
  ruleset_id uuid not null,
  ruleset_version_id uuid not null,
  mode varchar(10) not null,
  ruleset_name varchar(160) not null,
  ruleset_description text not null,
  config_json jsonb not null
) on commit drop;

insert into seed_rulesets (ruleset_id, ruleset_version_id, mode, ruleset_name, ruleset_description, config_json)
values
(
  '2f4d94db-2a9f-4d4d-9a8a-53b58c598f71',
  'f5b4c67b-0825-4970-9f07-3b68e8fcb524',
  'PEMULA',
  'Cashflowpoly Default - Mode Pemula',
  'Seed ruleset mode pemula dalam config_json terpadu dan katalog generik.',
  $json$
  {
    "mode": "PEMULA",
    "actions_per_turn": 2,
    "starting_cash": 20,
    "player_ordering": "JOIN_ORDER",
    "weekday_rules": {
      "FRI": { "feature": "DONATION", "enabled": true },
      "SAT": { "feature": "GOLD_TRADE", "enabled": true },
      "SUN": { "feature": "REST", "enabled": true }
    },
    "constraints": {
      "cash_min": 0,
      "max_ingredient_total": 6,
      "max_same_ingredient": 3,
      "primary_need_max_per_day": 1,
      "require_primary_before_others": true
    },
    "donation": { "min_amount": 1, "max_amount": 999999 },
    "gold_trade": { "allow_buy": true, "allow_sell": true },
    "advanced": {
      "loan": { "enabled": false },
      "insurance": { "enabled": false },
      "saving_goal": { "enabled": false }
    },
    "freelance": { "income": 1 },
    "scoring": {
      "donation_rank_points": [
        { "rank": 1, "points": 7 },
        { "rank": 2, "points": 5 },
        { "rank": 3, "points": 2 }
      ],
      "gold_points_by_qty": [
        { "qty": 1, "points": 3 },
        { "qty": 2, "points": 5 },
        { "qty": 3, "points": 8 },
        { "qty": 4, "points": 12 }
      ],
      "pension_rank_points": [
        { "rank": 1, "points": 5 },
        { "rank": 2, "points": 3 },
        { "rank": 3, "points": 1 }
      ],
      "score_matrix": [
        { "score_source": "DONATION", "rank": 1, "points": 7 },
        { "score_source": "DONATION", "rank": 2, "points": 5 },
        { "score_source": "DONATION", "rank": 3, "points": 2 },
        { "score_source": "PENSION", "rank": 1, "points": 5 },
        { "score_source": "PENSION", "rank": 2, "points": 3 },
        { "score_source": "PENSION", "rank": 3, "points": 1 }
      ]
    },
    "component_catalog": {
      "gameConfig": {
        "initialCoins": 20,
        "initialHappiness": 0,
        "initialSaving": 0,
        "actionsPerTurn": 2,
        "finishDay": 13,
        "minPlayers": 2,
        "maxPlayers": 4
      },
      "bahan": [
        { "id": "nasi_putih", "nama": "Nasi Putih", "hargaBeli": 1 },
        { "id": "sayur", "nama": "Sayur", "hargaBeli": 2 },
        { "id": "tahu_tempe", "nama": "Tahu Tempe", "hargaBeli": 3 },
        { "id": "telur", "nama": "Telur", "hargaBeli": 4 },
        { "id": "daging", "nama": "Daging", "hargaBeli": 5 }
      ],
      "resep": [
        { "id": "lontong_balap", "nama": "lontong balap", "hargaJual": 13, "poinKebahagiaan": 0, "bahan": ["Sayur", "Nasi Putih"] },
        { "id": "nasi_goreng", "nama": "nasi goreng", "hargaJual": 15, "poinKebahagiaan": 0, "bahan": ["Nasi Putih", "Telur"] },
        { "id": "tahu_campur", "nama": "tahu campur", "hargaJual": 16, "poinKebahagiaan": 0, "bahan": ["Daging", "Tahu Tempe"] },
        { "id": "rawon", "nama": "rawon", "hargaJual": 24, "poinKebahagiaan": 0, "bahan": ["Daging", "Telur", "Tahu Tempe"] }
      ],
      "kebutuhan": [
        { "id": "buku", "nama": "buku", "tipe": "primer", "hargaBeli": 2, "poinKebahagiaan": 1 },
        { "id": "sepatu", "nama": "sepatu", "tipe": "sekunder", "hargaBeli": 4, "poinKebahagiaan": 2 },
        { "id": "boneka", "nama": "boneka", "tipe": "tersier", "hargaBeli": 6, "poinKebahagiaan": 3 },
        { "id": "gameboy", "nama": "gameboy", "tipe": "tersier", "hargaBeli": 8, "poinKebahagiaan": 4 },
        { "id": "hiburan", "nama": "hiburan", "tipe": "tersier", "hargaBeli": 5, "poinKebahagiaan": 2 }
      ],
      "targetKebutuhan": [
        {
          "id": "misi_jam",
          "nama": "jam",
          "success_points": 0,
          "failure_points": -10,
          "penaltyPoints": 10,
          "kebutuhanTarget": [
            { "order": 1, "type": "TIER", "value": "primer" },
            { "order": 2, "type": "TIER", "value": "sekunder" },
            { "order": 3, "type": "NAME", "value": "jam" }
          ]
        },
        {
          "id": "misi_boneka",
          "nama": "boneka",
          "success_points": 0,
          "failure_points": -10,
          "penaltyPoints": 10,
          "kebutuhanTarget": [
            { "order": 1, "type": "TIER", "value": "primer" },
            { "order": 2, "type": "TIER", "value": "sekunder" },
            { "order": 3, "type": "NAME", "value": "boneka" }
          ]
        },
        {
          "id": "misi_gameboy",
          "nama": "gameboy",
          "success_points": 0,
          "failure_points": -10,
          "penaltyPoints": 10,
          "kebutuhanTarget": [
            { "order": 1, "type": "TIER", "value": "primer" },
            { "order": 2, "type": "TIER", "value": "sekunder" },
            { "order": 3, "type": "NAME", "value": "gameboy" }
          ]
        },
        {
          "id": "misi_hiburan",
          "nama": "hiburan",
          "success_points": 0,
          "failure_points": -10,
          "penaltyPoints": 10,
          "kebutuhanTarget": [
            { "order": 1, "type": "TIER", "value": "primer" },
            { "order": 2, "type": "TIER", "value": "sekunder" },
            { "order": 3, "type": "NAME", "value": "hiburan" }
          ]
        }
      ],
      "tujuanFinansial": [
        { "id": "beli_rumah", "nama": "beli rumah", "hargaBeli": 12, "poinKebahagiaan": 6 },
        { "id": "beli_motor", "nama": "beli motor", "hargaBeli": 8, "poinKebahagiaan": 4 }
      ],
      "narasi": [
        {
          "id": "jual_pertama",
          "nama": "jual_pertama",
          "teks": [
            "Penjualan pertama membuka kepercayaan diri.",
            "Momentum baik harus dijaga."
          ],
          "prerequisiteAksi": [
            { "aksi": "JualMasakan", "value": 1 }
          ]
        }
      ],
      "quest": [
        {
          "id": "mahir_jual_3_masakan",
          "nama": "jualan konsisten",
          "deskripsi": "Jual 3 masakan dalam satu sesi.",
          "aksi": "JualMasakan",
          "target": 3,
          "rewardCoins": 5,
          "rewardHappiness": 2
        }
      ]
    }
  }
  $json$::jsonb
),
(
  'a68f53f9-92a2-446f-9f62-5a4f502a0199',
  '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d',
  'MAHIR',
  'Cashflowpoly Default - Mode Mahir',
  'Seed ruleset mode mahir dalam config_json terpadu dan katalog generik.',
  $json$
  {
    "mode": "MAHIR",
    "actions_per_turn": 2,
    "starting_cash": 10,
    "player_ordering": "JOIN_ORDER",
    "weekday_rules": {
      "friday": { "feature": "DONATION", "enabled": true },
      "saturday": { "feature": "GOLD_TRADE", "enabled": true },
      "sunday": { "feature": "REST", "enabled": true }
    },
    "constraints": {
      "cash_min": 0,
      "max_ingredient_total": 6,
      "max_same_ingredient": 3,
      "primary_need_max_per_day": 1,
      "require_primary_before_others": true
    },
    "donation": { "min_amount": 1, "max_amount": 999999 },
    "gold_trade": { "allow_buy": true, "allow_sell": true },
    "advanced": {
      "loan": { "enabled": true },
      "insurance": { "enabled": true },
      "saving_goal": { "enabled": true }
    },
    "freelance": { "income": 1 },
    "scoring": {
      "donation_rank_points": [
        { "rank": 1, "points": 7 },
        { "rank": 2, "points": 5 },
        { "rank": 3, "points": 2 }
      ],
      "gold_points_by_qty": [
        { "qty": 1, "points": 3 },
        { "qty": 2, "points": 5 },
        { "qty": 3, "points": 8 },
        { "qty": 4, "points": 12 }
      ],
      "pension_rank_points": [
        { "rank": 1, "points": 5 },
        { "rank": 2, "points": 3 },
        { "rank": 3, "points": 1 }
      ],
      "score_matrix": [
        { "score_source": "DONATION", "rank": 1, "points": 7 },
        { "score_source": "DONATION", "rank": 2, "points": 5 },
        { "score_source": "DONATION", "rank": 3, "points": 2 },
        { "score_source": "PENSION", "rank": 1, "points": 5 },
        { "score_source": "PENSION", "rank": 2, "points": 3 },
        { "score_source": "PENSION", "rank": 3, "points": 1 }
      ]
    },
    "component_catalog": {
      "gameConfig": {
        "initialCoins": 10,
        "initialHappiness": 0,
        "initialSaving": 0,
        "actionsPerTurn": 2,
        "finishDay": 13,
        "minPlayers": 2,
        "maxPlayers": 4
      },
      "bahan": [
        { "id": "nasi_putih", "nama": "Nasi Putih", "hargaBeli": 1 },
        { "id": "sayur", "nama": "Sayur", "hargaBeli": 2 },
        { "id": "tahu_tempe", "nama": "Tahu Tempe", "hargaBeli": 3 },
        { "id": "telur", "nama": "Telur", "hargaBeli": 4 },
        { "id": "daging", "nama": "Daging", "hargaBeli": 5 }
      ],
      "resep": [
        { "id": "lontong_balap", "nama": "lontong balap", "hargaJual": 13, "poinKebahagiaan": 0, "bahan": ["Sayur", "Nasi Putih"] },
        { "id": "nasi_goreng", "nama": "nasi goreng", "hargaJual": 15, "poinKebahagiaan": 0, "bahan": ["Nasi Putih", "Telur"] },
        { "id": "tahu_campur", "nama": "tahu campur", "hargaJual": 16, "poinKebahagiaan": 0, "bahan": ["Daging", "Tahu Tempe"] },
        { "id": "rawon", "nama": "rawon", "hargaJual": 24, "poinKebahagiaan": 0, "bahan": ["Daging", "Telur", "Tahu Tempe"] }
      ],
      "kebutuhan": [
        { "id": "buku", "nama": "buku", "tipe": "primer", "hargaBeli": 2, "poinKebahagiaan": 1 },
        { "id": "sepatu", "nama": "sepatu", "tipe": "sekunder", "hargaBeli": 4, "poinKebahagiaan": 2 },
        { "id": "boneka", "nama": "boneka", "tipe": "tersier", "hargaBeli": 6, "poinKebahagiaan": 3 },
        { "id": "gameboy", "nama": "gameboy", "tipe": "tersier", "hargaBeli": 8, "poinKebahagiaan": 4 },
        { "id": "hiburan", "nama": "hiburan", "tipe": "tersier", "hargaBeli": 5, "poinKebahagiaan": 2 }
      ],
      "targetKebutuhan": [
        {
          "id": "misi_jam",
          "nama": "jam",
          "success_points": 0,
          "failure_points": -10,
          "penaltyPoints": 10,
          "kebutuhanTarget": [
            { "order": 1, "type": "TIER", "value": "primer" },
            { "order": 2, "type": "TIER", "value": "sekunder" },
            { "order": 3, "type": "NAME", "value": "jam" }
          ]
        },
        {
          "id": "misi_boneka",
          "nama": "boneka",
          "success_points": 0,
          "failure_points": -10,
          "penaltyPoints": 10,
          "kebutuhanTarget": [
            { "order": 1, "type": "TIER", "value": "primer" },
            { "order": 2, "type": "TIER", "value": "sekunder" },
            { "order": 3, "type": "NAME", "value": "boneka" }
          ]
        },
        {
          "id": "misi_gameboy",
          "nama": "gameboy",
          "success_points": 0,
          "failure_points": -10,
          "penaltyPoints": 10,
          "kebutuhanTarget": [
            { "order": 1, "type": "TIER", "value": "primer" },
            { "order": 2, "type": "TIER", "value": "sekunder" },
            { "order": 3, "type": "NAME", "value": "gameboy" }
          ]
        },
        {
          "id": "misi_hiburan",
          "nama": "hiburan",
          "success_points": 0,
          "failure_points": -10,
          "penaltyPoints": 10,
          "kebutuhanTarget": [
            { "order": 1, "type": "TIER", "value": "primer" },
            { "order": 2, "type": "TIER", "value": "sekunder" },
            { "order": 3, "type": "NAME", "value": "hiburan" }
          ]
        }
      ],
      "tujuanFinansial": [
        { "id": "beli_rumah", "nama": "beli rumah", "hargaBeli": 12, "poinKebahagiaan": 6 },
        { "id": "beli_motor", "nama": "beli motor", "hargaBeli": 8, "poinKebahagiaan": 4 }
      ],
      "narasi": [
        {
          "id": "jual_pertama",
          "nama": "jual_pertama",
          "teks": [
            "Penjualan pertama membuka kepercayaan diri.",
            "Momentum baik harus dijaga."
          ],
          "prerequisiteAksi": [
            { "aksi": "JualMasakan", "value": 1 }
          ]
        }
      ],
      "quest": [
        {
          "id": "mahir_jual_3_masakan",
          "nama": "jualan konsisten",
          "deskripsi": "Jual 3 masakan dalam satu sesi.",
          "aksi": "JualMasakan",
          "target": 3,
          "rewardCoins": 5,
          "rewardHappiness": 2
        }
      ]
    }
  }
  $json$::jsonb
);

insert into rulesets (ruleset_id, name, description, instructor_user_id, created_at, created_by)
select
  ruleset_id,
  ruleset_name,
  ruleset_description,
  null,
  now(),
  'system-seed-relational-v2'
from seed_rulesets
on conflict (ruleset_id) do update
set name = excluded.name,
    description = excluded.description,
    created_by = excluded.created_by;

insert into ruleset_versions (
  ruleset_version_id,
  ruleset_id,
  version,
  status,
  mode,
  config_json,
  schema_version,
  config_hash,
  change_note,
  published_at,
  created_at,
  created_by
)
select
  ruleset_version_id,
  ruleset_id,
  1,
  'ACTIVE',
  mode,
  config_json,
  '2.0.0',
  encode(digest(config_json::text, 'sha256'), 'hex'),
  'Canonical JSON catalog seed',
  now(),
  now(),
  'system-seed-relational-v2'
from seed_rulesets
on conflict (ruleset_id, version) do update
set status = excluded.status,
    mode = excluded.mode,
    config_json = excluded.config_json,
    schema_version = excluded.schema_version,
    config_hash = excluded.config_hash,
    change_note = excluded.change_note,
    published_at = excluded.published_at,
    created_by = excluded.created_by;

delete from ruleset_catalog_item_requirements
where ruleset_catalog_item_id in (
  select ruleset_catalog_item_id
  from ruleset_catalog_items
  where ruleset_version_id in (select ruleset_version_id from seed_rulesets)
);

delete from ruleset_catalog_items
where ruleset_version_id in (select ruleset_version_id from seed_rulesets);

insert into ruleset_catalog_items (
  ruleset_version_id,
  item_type,
  item_code,
  item_name,
  sort_order,
  card_qty,
  is_active,
  payload_json
)
select
  sr.ruleset_version_id,
  'INGREDIENT',
  item->>'id',
  item->>'nama',
  ord::int,
  null,
  true,
  jsonb_build_object(
    'ingredient_id', item->>'id',
    'display_name', item->>'nama',
    'hargaBeli', coalesce((item->>'hargaBeli')::int, 0)
  )
from seed_rulesets sr
cross join lateral jsonb_array_elements(sr.config_json->'component_catalog'->'bahan') with ordinality as x(item, ord);

insert into ruleset_catalog_items (
  ruleset_version_id,
  item_type,
  item_code,
  item_name,
  sort_order,
  card_qty,
  is_active,
  payload_json
)
select
  sr.ruleset_version_id,
  'ORDER',
  item->>'id',
  item->>'nama',
  ord::int,
  1,
  true,
  jsonb_build_object(
    'hargaJual', coalesce((item->>'hargaJual')::int, 0),
    'poinKebahagiaan', coalesce((item->>'poinKebahagiaan')::int, 0)
  )
from seed_rulesets sr
cross join lateral jsonb_array_elements(sr.config_json->'component_catalog'->'resep') with ordinality as x(item, ord);

insert into ruleset_catalog_items (
  ruleset_version_id,
  item_type,
  item_code,
  item_name,
  sort_order,
  card_qty,
  is_active,
  payload_json
)
select
  sr.ruleset_version_id,
  'NEED',
  item->>'id',
  item->>'nama',
  ord::int,
  1,
  true,
  jsonb_build_object(
    'tipe', item->>'tipe',
    'hargaBeli', coalesce((item->>'hargaBeli')::int, 0),
    'poinKebahagiaan', coalesce((item->>'poinKebahagiaan')::int, 0)
  )
from seed_rulesets sr
cross join lateral jsonb_array_elements(sr.config_json->'component_catalog'->'kebutuhan') with ordinality as x(item, ord);

insert into ruleset_catalog_items (
  ruleset_version_id,
  item_type,
  item_code,
  item_name,
  sort_order,
  card_qty,
  is_active,
  payload_json
)
select
  sr.ruleset_version_id,
  'COLLECTION_MISSION',
  item->>'id',
  item->>'nama',
  ord::int,
  1,
  true,
  jsonb_build_object(
    'success_points', coalesce((item->>'success_points')::int, 0),
    'failure_points', coalesce((item->>'failure_points')::int, 0),
    'penaltyPoints', coalesce((item->>'penaltyPoints')::int, 0)
  )
from seed_rulesets sr
cross join lateral jsonb_array_elements(sr.config_json->'component_catalog'->'targetKebutuhan') with ordinality as x(item, ord);

insert into ruleset_catalog_items (
  ruleset_version_id,
  item_type,
  item_code,
  item_name,
  sort_order,
  card_qty,
  is_active,
  payload_json
)
select
  sr.ruleset_version_id,
  'FINANCIAL_GOAL',
  item->>'id',
  item->>'nama',
  ord::int,
  1,
  true,
  jsonb_build_object(
    'hargaBeli', coalesce((item->>'hargaBeli')::int, 0),
    'poinKebahagiaan', coalesce((item->>'poinKebahagiaan')::int, 0)
  )
from seed_rulesets sr
cross join lateral jsonb_array_elements(sr.config_json->'component_catalog'->'tujuanFinansial') with ordinality as x(item, ord);

insert into ruleset_catalog_items (
  ruleset_version_id,
  item_type,
  item_code,
  item_name,
  sort_order,
  card_qty,
  is_active,
  payload_json
)
select
  sr.ruleset_version_id,
  'NARRATIVE',
  item->>'id',
  item->>'nama',
  ord::int,
  1,
  true,
  jsonb_build_object(
    'teks', coalesce(item->'teks', '[]'::jsonb),
    'prerequisiteAksi', coalesce(item->'prerequisiteAksi', '[]'::jsonb)
  )
from seed_rulesets sr
cross join lateral jsonb_array_elements(sr.config_json->'component_catalog'->'narasi') with ordinality as x(item, ord);

insert into ruleset_catalog_items (
  ruleset_version_id,
  item_type,
  item_code,
  item_name,
  sort_order,
  card_qty,
  is_active,
  payload_json
)
select
  sr.ruleset_version_id,
  'QUEST',
  item->>'id',
  item->>'nama',
  ord::int,
  1,
  true,
  jsonb_build_object(
    'deskripsi', item->>'deskripsi',
    'aksi', item->>'aksi',
    'target', coalesce((item->>'target')::int, 0),
    'rewardCoins', coalesce((item->>'rewardCoins')::int, 0),
    'rewardHappiness', coalesce((item->>'rewardHappiness')::int, 0)
  )
from seed_rulesets sr
cross join lateral jsonb_array_elements(sr.config_json->'component_catalog'->'quest') with ordinality as x(item, ord);

insert into ruleset_catalog_item_requirements (
  ruleset_catalog_item_id,
  requirement_order,
  requirement_type,
  requirement_value,
  qty_required,
  payload_json
)
select
  rci.ruleset_catalog_item_id,
  row_number() over (
    partition by sr.ruleset_version_id, recipe->>'id'
    order by ingredient_name
  )::int,
  'INGREDIENT',
  ingredient_name,
  ingredient_qty,
  jsonb_build_object('ingredient_name', ingredient_name)
from seed_rulesets sr
cross join lateral jsonb_array_elements(sr.config_json->'component_catalog'->'resep') as recipe(recipe)
cross join lateral (
  select ingredient_name, count(*)::int as ingredient_qty
  from jsonb_array_elements_text(recipe.recipe->'bahan') as z(ingredient_name)
  group by ingredient_name
) req
join ruleset_catalog_items rci
  on rci.ruleset_version_id = sr.ruleset_version_id
 and rci.item_type = 'ORDER'
 and rci.item_code = recipe.recipe->>'id';

insert into ruleset_catalog_item_requirements (
  ruleset_catalog_item_id,
  requirement_order,
  requirement_type,
  requirement_value,
  qty_required,
  payload_json
)
select
  rci.ruleset_catalog_item_id,
  coalesce((rule->>'order')::int, ord::int),
  'MISSION_RULE',
  rule->>'value',
  null,
  jsonb_build_object(
    'type', rule->>'type',
    'value', rule->>'value'
  )
from seed_rulesets sr
cross join lateral jsonb_array_elements(sr.config_json->'component_catalog'->'targetKebutuhan') as mission(mission)
cross join lateral jsonb_array_elements(coalesce(mission.mission->'kebutuhanTarget', '[]'::jsonb)) with ordinality as x(rule, ord)
join ruleset_catalog_items rci
  on rci.ruleset_version_id = sr.ruleset_version_id
 and rci.item_type = 'COLLECTION_MISSION'
 and rci.item_code = mission.mission->>'id';

insert into ruleset_catalog_items (
  ruleset_version_id,
  item_type,
  item_code,
  item_name,
  sort_order,
  card_qty,
  is_active,
  payload_json
)
select
  ruleset_version_id,
  item_type,
  item_code,
  item_name,
  sort_order,
  card_qty,
  true,
  payload_json
from (
  values
    ('f5b4c67b-0825-4970-9f07-3b68e8fcb524'::uuid, 'DONATION_RANK', 'donation_rank_1', 'Donasi Rank 1', 201, 1, '{"rank":1,"points":7}'::jsonb),
    ('f5b4c67b-0825-4970-9f07-3b68e8fcb524'::uuid, 'DONATION_RANK', 'donation_rank_2', 'Donasi Rank 2', 202, 1, '{"rank":2,"points":5}'::jsonb),
    ('f5b4c67b-0825-4970-9f07-3b68e8fcb524'::uuid, 'DONATION_RANK', 'donation_rank_3', 'Donasi Rank 3', 203, 1, '{"rank":3,"points":2}'::jsonb),
    ('f5b4c67b-0825-4970-9f07-3b68e8fcb524'::uuid, 'PENSION_RANK', 'pension_rank_1', 'Pensiun Rank 1', 211, 1, '{"rank":1,"points":5}'::jsonb),
    ('f5b4c67b-0825-4970-9f07-3b68e8fcb524'::uuid, 'PENSION_RANK', 'pension_rank_2', 'Pensiun Rank 2', 212, 1, '{"rank":2,"points":3}'::jsonb),
    ('f5b4c67b-0825-4970-9f07-3b68e8fcb524'::uuid, 'PENSION_RANK', 'pension_rank_3', 'Pensiun Rank 3', 213, 1, '{"rank":3,"points":1}'::jsonb),
    ('f5b4c67b-0825-4970-9f07-3b68e8fcb524'::uuid, 'GOLD_PRICE', 'gold_price_1', 'Harga Emas 1', 221, 1, '{"qty":1,"price":3}'::jsonb),
    ('f5b4c67b-0825-4970-9f07-3b68e8fcb524'::uuid, 'GOLD', 'gold_card_1', 'Emas 1', 231, 1, '{"qty":1,"points":3}'::jsonb),
    ('f5b4c67b-0825-4970-9f07-3b68e8fcb524'::uuid, 'TIE_BREAKER', 'tie_breaker_1', 'Tie Breaker 1', 241, 1, '{"number":1}'::jsonb),
    ('7c3bfd8a-27d7-4468-b8d7-cf90131bc61d'::uuid, 'DONATION_RANK', 'donation_rank_1', 'Donasi Rank 1', 201, 1, '{"rank":1,"points":7}'::jsonb),
    ('7c3bfd8a-27d7-4468-b8d7-cf90131bc61d'::uuid, 'DONATION_RANK', 'donation_rank_2', 'Donasi Rank 2', 202, 1, '{"rank":2,"points":5}'::jsonb),
    ('7c3bfd8a-27d7-4468-b8d7-cf90131bc61d'::uuid, 'DONATION_RANK', 'donation_rank_3', 'Donasi Rank 3', 203, 1, '{"rank":3,"points":2}'::jsonb),
    ('7c3bfd8a-27d7-4468-b8d7-cf90131bc61d'::uuid, 'PENSION_RANK', 'pension_rank_1', 'Pensiun Rank 1', 211, 1, '{"rank":1,"points":5}'::jsonb),
    ('7c3bfd8a-27d7-4468-b8d7-cf90131bc61d'::uuid, 'PENSION_RANK', 'pension_rank_2', 'Pensiun Rank 2', 212, 1, '{"rank":2,"points":3}'::jsonb),
    ('7c3bfd8a-27d7-4468-b8d7-cf90131bc61d'::uuid, 'PENSION_RANK', 'pension_rank_3', 'Pensiun Rank 3', 213, 1, '{"rank":3,"points":1}'::jsonb),
    ('7c3bfd8a-27d7-4468-b8d7-cf90131bc61d'::uuid, 'GOLD_PRICE', 'gold_price_1', 'Harga Emas 1', 221, 1, '{"qty":1,"price":3}'::jsonb),
    ('7c3bfd8a-27d7-4468-b8d7-cf90131bc61d'::uuid, 'GOLD', 'gold_card_1', 'Emas 1', 231, 1, '{"qty":1,"points":3}'::jsonb),
    ('7c3bfd8a-27d7-4468-b8d7-cf90131bc61d'::uuid, 'TIE_BREAKER', 'tie_breaker_1', 'Tie Breaker 1', 241, 1, '{"number":1}'::jsonb),
    ('7c3bfd8a-27d7-4468-b8d7-cf90131bc61d'::uuid, 'SHARIA_LOAN', 'loan_syariah_10', 'Pinjaman Syariah 10', 251, 1, '{"principal":10,"installment":2,"duration":5,"penalty_points":15}'::jsonb),
    ('7c3bfd8a-27d7-4468-b8d7-cf90131bc61d'::uuid, 'INSURANCE', 'multirisk_basic', 'Asuransi Multirisk', 261, 1, '{"premium":4,"usage_limit":1}'::jsonb),
    ('7c3bfd8a-27d7-4468-b8d7-cf90131bc61d'::uuid, 'LIFE_RISK', 'risk_hospital', 'Biaya Rumah Sakit', 271, 1, '{"effect_type":"COIN_EFFECT","direction":"OUT","amount":6}'::jsonb)
) as extra(ruleset_version_id, item_type, item_code, item_name, sort_order, card_qty, payload_json)
on conflict (ruleset_version_id, item_type, item_code) do update
set item_name = excluded.item_name,
    sort_order = excluded.sort_order,
    card_qty = excluded.card_qty,
    is_active = excluded.is_active,
    payload_json = excluded.payload_json;

commit;
