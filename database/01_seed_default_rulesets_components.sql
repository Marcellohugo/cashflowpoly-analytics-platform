-- Fungsi file: Menyemai ruleset default Cashflowpoly beserta katalog komponen mode pemula dan mode mahir.
create extension if not exists pgcrypto;

begin;

insert into rulesets (
  ruleset_id,
  name,
  description,
  instructor_user_id,
  created_at,
  created_by
)
values (
  '2f4d94db-2a9f-4d4d-9a8a-53b58c598f71',
  'Cashflowpoly Default - Mode Pemula (Komponen Lengkap)',
  'Seed ruleset mode pemula dengan susunan komponen lengkap.',
  null,
  now(),
  'system-seed-components-v1'
)
on conflict (ruleset_id) do nothing;

with beginner_config as (
  select
    $json$
    {
      "mode": "PEMULA",
      "actions_per_turn": 2,
      "starting_cash": 20,
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
      "donation": {
        "min_amount": 1,
        "max_amount": 999999
      },
      "gold_trade": {
        "allow_buy": true,
        "allow_sell": true
      },
      "advanced": {
        "loan": { "enabled": false },
        "insurance": { "enabled": false },
        "saving_goal": { "enabled": false }
      },
      "freelance": {
        "income": 1
      },
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
          { "rank": 1, "points": 7 },
          { "rank": 2, "points": 5 },
          { "rank": 3, "points": 2 }
        ]
      },
      "component_catalog": {
        "mode": "PEMULA",
        "tokens_and_coins": {
          "mr_cashflowpoly_token": { "qty": 1 },
          "player_action_tokens": {
            "qty": 8,
            "qty_per_player": 2,
            "max_players": 4
          },
          "coins": {
            "total_qty": 48,
            "denominations": [
              { "nominal": 10, "qty": 16 },
              { "nominal": 5, "qty": 16 },
              { "nominal": 1, "qty": 16 }
            ]
          }
        },
        "boards_and_table_items": [
          { "item": "tent card tujuan permainan", "qty": 1 },
          { "item": "papan kalender kerja", "qty": 1 },
          { "item": "papan juara peduli donasi", "qty": 1 },
          { "item": "papan investasi emas", "qty": 1 },
          { "item": "papan pesanan masakan", "qty": 1 },
          { "item": "papan aneka kebutuhan", "qty": 1 },
          { "item": "papan bahan masakan", "qty": 1 },
          { "item": "layar pemain", "qty": 4 }
        ],
        "cards": {
          "pension_champion_cards": {
            "total_qty": 3,
            "items": [
              { "rank": 1, "qty": 1, "happiness_points": 7 },
              { "rank": 2, "qty": 1, "happiness_points": 5 },
              { "rank": 3, "qty": 1, "happiness_points": 2 }
            ]
          },
          "tie_breaker_cards": {
            "total_qty": 4,
            "type_alias": "kartu asuransi",
            "price_coins": 1
          },
          "collection_mission_cards": {
            "total_qty": 4,
            "items": [
              { "name": "jam", "qty": 1 },
              { "name": "boneka", "qty": 1 },
              { "name": "gameboy", "qty": 1 },
              { "name": "hiburan", "qty": 1 }
            ]
          },
          "gold_price_cards": {
            "total_qty": 6,
            "items": [
              { "price_coins": 5, "qty": 2 },
              { "price_coins": 6, "qty": 2 },
              { "price_coins": 7, "qty": 1 },
              { "price_coins": 8, "qty": 1 }
            ]
          },
          "donation_champion_cards": {
            "total_qty": 9,
            "items": [
              { "rank": 1, "qty": 3, "happiness_points": 7 },
              { "rank": 2, "qty": 3, "happiness_points": 5 },
              { "rank": 3, "qty": 3, "happiness_points": 2 }
            ]
          },
          "gold_cards": {
            "total_qty": 20
          },
          "needs_cards": {
            "total_qty": 25,
            "tiers": {
              "primer": [
                { "name": "buku", "qty": 2, "happiness_points": 1, "price_coins": 2 },
                { "name": "tempat makan", "qty": 1, "happiness_points": 1, "price_coins": 2 },
                { "name": "baju", "qty": 1, "happiness_points": 1, "price_coins": 2 },
                { "name": "sepatu", "qty": 1, "happiness_points": 1, "price_coins": 2 },
                { "name": "buku", "qty": 1, "happiness_points": 2, "price_coins": 3 },
                { "name": "tempat makan", "qty": 1, "happiness_points": 2, "price_coins": 3 },
                { "name": "baju", "qty": 1, "happiness_points": 2, "price_coins": 3 },
                { "name": "sepatu", "qty": 1, "happiness_points": 2, "price_coins": 3 }
              ],
              "sekunder": [
                { "name": "tas", "qty": 1, "happiness_points": 3, "price_coins": 4 },
                { "name": "tab", "qty": 1, "happiness_points": 3, "price_coins": 4 },
                { "name": "tempat pensil", "qty": 1, "happiness_points": 3, "price_coins": 4 },
                { "name": "sepeda", "qty": 1, "happiness_points": 3, "price_coins": 4 },
                { "name": "tas", "qty": 1, "happiness_points": 4, "price_coins": 5 },
                { "name": "tab", "qty": 1, "happiness_points": 4, "price_coins": 5 },
                { "name": "tempat pensil", "qty": 1, "happiness_points": 4, "price_coins": 5 },
                { "name": "sepeda", "qty": 1, "happiness_points": 4, "price_coins": 5 }
              ],
              "tersier": [
                { "name": "jam", "qty": 1, "happiness_points": 5, "price_coins": 6 },
                { "name": "boneka", "qty": 1, "happiness_points": 5, "price_coins": 6 },
                { "name": "gameboy", "qty": 1, "happiness_points": 5, "price_coins": 6 },
                { "name": "hiburan", "qty": 1, "happiness_points": 5, "price_coins": 6 },
                { "name": "jam", "qty": 1, "happiness_points": 6, "price_coins": 7 },
                { "name": "boneka", "qty": 1, "happiness_points": 6, "price_coins": 7 },
                { "name": "gameboy", "qty": 1, "happiness_points": 6, "price_coins": 7 },
                { "name": "hiburan", "qty": 1, "happiness_points": 6, "price_coins": 7 }
              ]
            }
          },
          "order_cards": {
            "total_qty": 25,
            "items": [
              { "name": "lontong balap", "qty": 2, "requirements": ["sayur", "nasi"], "price_coins": 13 },
              { "name": "semanggi suroboyo", "qty": 2, "requirements": ["sayur", "sayur"], "price_coins": 14 },
              { "name": "nasi goreng", "qty": 1, "requirements": ["nasi", "telur"], "price_coins": 15 },
              { "name": "tahu campur", "qty": 2, "requirements": ["daging", "tahu tempe"], "price_coins": 16 },
              { "name": "soto daging", "qty": 2, "requirements": ["daging", "telur"], "price_coins": 17 },
              { "name": "nasi pecel", "qty": 2, "requirements": ["tahu tempe", "sayur", "nasi"], "price_coins": 20 },
              { "name": "sego penyet", "qty": 2, "requirements": ["telur", "tahu tempe", "nasi"], "price_coins": 22 },
              { "name": "rawon", "qty": 2, "requirements": ["daging", "telur", "tahu tempe"], "price_coins": 24 },
              { "name": "tahu telur", "qty": 2, "requirements": ["telur", "telur", "tahu tempe"], "price_coins": 25 },
              { "name": "gado-gado", "qty": 2, "requirements": ["telur", "tahu tempe", "sayur", "nasi"], "price_coins": 26 },
              { "name": "sate klopo", "qty": 2, "requirements": ["daging", "daging", "nasi"], "price_coins": 26 },
              { "name": "nasi campur", "qty": 2, "requirements": ["daging", "telur", "sayur", "nasi"], "price_coins": 27 },
              { "name": "rujak cingur", "qty": 2, "requirements": ["daging", "tahu tempe", "sayur", "nasi"], "price_coins": 28 }
            ]
          },
          "ingredient_cards": {
            "total_qty": 25,
            "items": [
              { "name": "nasi putih", "qty": 5, "price_coins": 1 },
              { "name": "sayur", "qty": 5, "price_coins": 2 },
              { "name": "tahu tempe", "qty": 5, "price_coins": 3 },
              { "name": "telur", "qty": 5, "price_coins": 4 },
              { "name": "daging", "qty": 5, "price_coins": 5 }
            ]
          }
        }
      }
    }
    $json$::jsonb as cfg
)
insert into ruleset_versions (
  ruleset_version_id,
  ruleset_id,
  version,
  status,
  config_json,
  config_hash,
  created_at,
  created_by
)
select
  'f5b4c67b-0825-4970-9f07-3b68e8fcb524',
  '2f4d94db-2a9f-4d4d-9a8a-53b58c598f71',
  1,
  'ACTIVE',
  cfg,
  encode(digest(cfg::text, 'sha256'), 'hex'),
  now(),
  'system-seed-components-v1'
from beginner_config
on conflict (ruleset_id, version) do nothing;

insert into rulesets (
  ruleset_id,
  name,
  description,
  instructor_user_id,
  created_at,
  created_by
)
values (
  'a68f53f9-92a2-446f-9f62-5a4f502a0199',
  'Cashflowpoly Default - Mode Mahir (Ekspansi Komponen Lengkap)',
  'Seed ruleset mode mahir dengan ekspansi komponen lengkap.',
  null,
  now(),
  'system-seed-components-v1'
)
on conflict (ruleset_id) do nothing;

with advanced_config as (
  select
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
      "donation": {
        "min_amount": 1,
        "max_amount": 999999
      },
      "gold_trade": {
        "allow_buy": true,
        "allow_sell": true
      },
      "advanced": {
        "loan": { "enabled": true },
        "insurance": { "enabled": true },
        "saving_goal": { "enabled": true }
      },
      "freelance": {
        "income": 1
      },
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
          { "rank": 1, "points": 7 },
          { "rank": 2, "points": 5 },
          { "rank": 3, "points": 2 }
        ]
      },
      "component_catalog": {
        "mode": "MAHIR",
        "requires_beginner_components": true,
        "advanced_expansion": {
          "boards_and_table_items": [
            { "item": "papan tabungan tujuan keuangan", "qty": 1 },
            { "item": "papan risiko kehidupan", "qty": 1 },
            { "item": "papan asuransi dan bank syariah", "qty": 1 }
          ],
          "cards": {
            "financial_goal_cards": {
              "total_qty": 5,
              "items": [
                { "name": "ngumpul keluarga", "qty": 1, "happiness_points": 20, "price_coins": 25 },
                { "name": "ke kebun binatang", "qty": 1, "happiness_points": 25, "price_coins": 28 },
                { "name": "keluar kota", "qty": 1, "happiness_points": 28, "price_coins": 30 },
                { "name": "beli kendaraan", "qty": 1, "happiness_points": 30, "price_coins": 32 },
                { "name": "beli rumah", "qty": 1, "happiness_points": 35, "price_coins": 35 }
              ]
            },
            "sharia_loan_cards": {
              "total_qty": 8,
              "happiness_points_penalty_per_card": -15
            },
            "life_risk_cards": {
              "total_qty": 24,
              "categories": [
                {
                  "name": "kesehatan",
                  "total_qty": 6,
                  "items": [
                    { "name": "depresi dan sakit perut", "qty": 2, "coin_effect": -3 },
                    { "name": "sakit gigi dan asam lambung", "qty": 2, "coin_effect": -4 },
                    { "name": "kecelakaan", "qty": 1, "coin_effect": -5 },
                    { "name": "operasi usus buntu", "qty": 1, "coin_effect": -6 }
                  ]
                },
                {
                  "name": "pendidikan",
                  "total_qty": 4,
                  "items": [
                    { "name": "ekstrakulikuler anak", "qty": 1, "coin_effect": -3 },
                    { "name": "studytour", "qty": 1, "coin_effect": -4 },
                    { "name": "tahun ajaran baru", "qty": 1, "coin_effect": -5 },
                    { "name": "wisuda kelulusan", "qty": 1, "coin_effect": -6 }
                  ]
                },
                {
                  "name": "usaha",
                  "total_qty": 4,
                  "items": [
                    { "name": "pemadaman listrik", "qty": 1, "coin_effect": -3 },
                    { "name": "bencana banjir", "qty": 1, "coin_effect": -3 },
                    { "name": "beli peralatan dapur", "qty": 1, "coin_effect": -3 },
                    { "name": "gudang terbakar", "qty": 1, "coin_effect": -6 }
                  ]
                },
                {
                  "name": "kendaraan",
                  "total_qty": 4,
                  "items": [
                    { "name": "ganti oli", "qty": 1, "coin_effect": -3 },
                    { "name": "ban bocor", "qty": 1, "coin_effect": -3 },
                    { "name": "ganti aki", "qty": 1, "coin_effect": -3 },
                    { "name": "mobil tabrakan", "qty": 1, "coin_effect": -6 }
                  ]
                },
                {
                  "name": "peristiwa positif",
                  "total_qty": 2,
                  "items": [
                    { "name": "menang undian", "qty": 1, "coin_effect": 2 },
                    { "name": "ulang tahun", "qty": 1, "coin_effect_rule": "+1 coin dari tiap pemain lain" }
                  ]
                },
                {
                  "name": "momen pasar dan harga",
                  "total_qty": 2,
                  "items": [
                    { "name": "panen melimpah", "qty": 1, "ingredient_price_delta_per_week": -1 },
                    { "name": "BBM naik", "qty": 1, "ingredient_price_delta_per_week": 1 }
                  ]
                },
                {
                  "name": "aksi emas",
                  "total_qty": 2,
                  "items": [
                    {
                      "name": "investasi emas",
                      "qty": 2,
                      "effect": "perbarui harga emas lalu semua pemain boleh jual-beli emas"
                    }
                  ]
                }
              ]
            }
          }
        }
      }
    }
    $json$::jsonb as cfg
)
insert into ruleset_versions (
  ruleset_version_id,
  ruleset_id,
  version,
  status,
  config_json,
  config_hash,
  created_at,
  created_by
)
select
  '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d',
  'a68f53f9-92a2-446f-9f62-5a4f502a0199',
  1,
  'ACTIVE',
  cfg,
  encode(digest(cfg::text, 'sha256'), 'hex'),
  now(),
  'system-seed-components-v1'
from advanced_config
on conflict (ruleset_id, version) do nothing;

commit;
