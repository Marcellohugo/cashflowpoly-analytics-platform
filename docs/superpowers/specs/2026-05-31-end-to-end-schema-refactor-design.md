# End-to-End Schema Refactor Design

## Tujuan

Menyederhanakan model data Cashflowpoly secara menyeluruh untuk bootstrap database baru dari nol dengan prinsip:

- aturan ruleset scalar dipusatkan di `ruleset_versions.config_json`
- katalog ruleset dipusatkan di tabel generik
- detail event dipusatkan di `events.payload`
- detail field-level log/agregat dipusatkan di kolom JSON parent
- seluruh fitur gameplay yang sudah ada tetap dipertahankan
- kontrak API boleh berubah total
- migrasi data lama bukan target

## Scope

Refactor ini mencakup:

- `database/00_create_schema.sql`
- seed SQL default dan inspeksi
- repository Dapper
- model EF Core dan snapshot/migration yang dipakai bootstrap skema baru
- bootstrap database
- kontrak DTO/API yang terdampak
- test unit/integration yang mengasumsikan skema lama

Refactor ini tidak mencakup:

- migrasi data dari skema lama
- kompatibilitas mundur API
- kompatibilitas terhadap database existing

## Keputusan Arsitektur

Dipilih pendekatan `core-relational + JSON payloads`.

Alasannya:

- query identitas, sesi, aktivasi ruleset, event timeline, dan proyeksi analitik tetap lebih aman bila relasional
- struktur ruleset, detail event, dan detail log saat ini terlalu terfragmentasi dan lebih cocok diperlakukan sebagai dokumen terstruktur
- seluruh fitur gameplay masih bisa dijaga tanpa mempertahankan ledakan tabel subtype

## Target Schema

### 1. Entitas inti yang dipertahankan

Tabel berikut tetap menjadi entitas inti:

- `app_users`
- `players`
- `user_player_links`
- `rulesets`
- `ruleset_versions`
- `sessions`
- `session_ruleset_activations`
- `session_players`
- `session_states`
- `session_player_states`
- `events`
- `event_cashflow_projections`
- `metric_snapshots`
- `validation_logs`
- `session_action_logs`
- `security_audit_logs`

### 2. Aturan ruleset scalar dipindah ke `ruleset_versions.config_json`

Tabel berikut dihapus:

- `ruleset_settings`
- `ruleset_features`
- `ruleset_weekday_rules`
- `ruleset_rank_points`
- `ruleset_gold_points`

Semua datanya menjadi bagian dari `ruleset_versions.config_json` dengan struktur minimal:

```json
{
  "mode": "PEMULA",
  "actions_per_turn": 2,
  "starting_cash": 20,
  "player_ordering": "JOIN_ORDER",
  "constraints": {
    "cash_min": 0,
    "max_ingredient_total": 6,
    "max_same_ingredient": 3,
    "primary_need_max_per_day": 1,
    "require_primary_before_others": true
  },
  "weekday_rules": {
    "FRI": ["DONATION_DAY"],
    "SAT": ["GOLD_TRADE_DAY"],
    "SUN": ["REST_DAY"]
  },
  "features": {
    "DONATION": true,
    "GOLD_TRADE": true,
    "LOAN": false,
    "INSURANCE": false,
    "SAVING_GOAL": false,
    "LIFE_RISK": false
  },
  "scoring": {
    "donation_rank_points": [
      { "rank": 1, "points": 7 }
    ],
    "pension_rank_points": [
      { "rank": 1, "points": 5 }
    ],
    "gold_points_by_qty": [
      { "qty": 1, "points": 3 }
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
    }
  }
}
```

### 3. Katalog game disatukan ke `ruleset_catalog_items`

Tabel baru:

- `ruleset_catalog_items`

Kolom inti:

- `ruleset_catalog_item_id uuid primary key`
- `ruleset_version_id uuid not null`
- `item_type varchar(50) not null`
- `item_code varchar(120) not null`
- `item_name varchar(160) not null`
- `mode varchar(10) null`
- `sort_order int not null default 1`
- `is_active boolean not null default true`
- `payload_json jsonb not null default '{}'::jsonb`
- `created_at timestamptz not null default now()`
- `updated_at timestamptz not null default now()`

Constraint utama:

- `unique(ruleset_version_id, item_type, item_code)`

Tabel ini menggantikan:

- `ingredient_cards`
- `order_cards`
- `need_cards`
- `collection_mission_cards`
- `financial_goal_cards`
- `gold_price_cards`
- `gold_cards`
- `donation_rank_cards`
- `pension_rank_cards`
- `tie_breaker_cards`
- `sharia_loan_cards`
- `insurance_cards`
- `life_risk_cards`
- `narratives`
- `quests`

`ingredients`, `actions`, `app_menus`, `role_menu_permissions`, dan `game_components` tetap diperlakukan sebagai master seed.

Nilai `item_type` yang dipakai:

- `INGREDIENT_CARD`
- `ORDER_CARD`
- `NEED_CARD`
- `COLLECTION_MISSION_CARD`
- `FINANCIAL_GOAL_CARD`
- `GOLD_PRICE_CARD`
- `GOLD_CARD`
- `DONATION_RANK_CARD`
- `PENSION_RANK_CARD`
- `TIE_BREAKER_CARD`
- `SHARIA_LOAN_CARD`
- `INSURANCE_CARD`
- `LIFE_RISK_CARD`
- `NARRATIVE`
- `QUEST`

Contoh `payload_json`:

```json
{
  "price_coins": 5,
  "happiness_points": 3,
  "card_qty": 2,
  "effect_type": "COIN_EFFECT",
  "coin_effect": -3,
  "usage_limit": 1,
  "texts": ["narasi 1", "narasi 2", "narasi 3"],
  "action_id": "JualMasakan",
  "target": 3,
  "reward_coins": 5,
  "reward_happiness": 2
}
```

### 4. Requirement katalog disatukan ke `ruleset_catalog_item_requirements`

Tabel baru:

- `ruleset_catalog_item_requirements`

Kolom inti:

- `requirement_id uuid primary key`
- `ruleset_catalog_item_id uuid not null`
- `requirement_group varchar(50) not null`
- `requirement_order int not null default 1`
- `requirement_type varchar(40) not null`
- `requirement_ref_code varchar(120) null`
- `payload_json jsonb not null default '{}'::jsonb`

Tabel ini menggantikan:

- `order_card_ingredients`
- `collection_mission_requirements`
- `narrative_triggers`

Pemetaan:

- resep memakai `requirement_group='ORDER_RECIPE'`
- misi koleksi memakai `requirement_group='MISSION_TARGET'`
- trigger narasi memakai `requirement_group='NARRATIVE_TRIGGER'`

### 5. Detail event dipindah ke `events.payload`

Tabel berikut dihapus:

- `event_buy_ingredient_details`
- `event_sell_order_details`
- `event_sell_order_ingredients`
- `event_buy_need_details`
- `event_freelance_details`
- `event_saving_details`
- `event_financial_goal_details`
- `event_donation_details`
- `event_gold_trade_details`
- `event_life_risk_details`
- `event_loan_details`
- `event_insurance_details`
- `event_turn_details`

`events` tetap dipertahankan dan ditambah/dirapikan untuk menampung:

- `action_id`
- `payload jsonb not null default '{}'::jsonb`

Contoh payload per aksi:

```json
{
  "ingredient_id": "beras",
  "qty": 2,
  "price_each": 1,
  "total_price": 2
}
```

```json
{
  "trade_type": "BUY",
  "gold_qty": 1,
  "price_each": 5,
  "total_amount": 5
}
```

### 6. Field-level log/agregat dilebur ke JSON parent

Tabel berikut dihapus:

- `metric_snapshot_fields`
- `validation_rule_results`
- `session_action_log_fields`
- `security_audit_log_fields`

Parent table berubah menjadi:

- `metric_snapshots` punya `metric_payload_json jsonb`
- `validation_logs` punya `details_json jsonb`
- `session_action_logs` punya `action_json jsonb`
- `security_audit_logs` punya `detail_json jsonb`

### 7. Aset pemain digabung ke `session_player_assets`

Tabel berikut dihapus:

- `session_player_gold`
- `session_player_loans`
- `session_player_insurances`

Tabel baru:

- `session_player_assets`

Kolom inti:

- `session_player_asset_id uuid primary key`
- `session_player_id uuid not null`
- `asset_type varchar(40) not null`
- `asset_ref_code varchar(120) null`
- `quantity int not null default 0`
- `status varchar(40) null`
- `value_json jsonb not null default '{}'::jsonb`
- `created_at timestamptz not null default now()`
- `updated_at timestamptz not null default now()`

`asset_type` minimal:

- `GOLD`
- `LOAN`
- `INSURANCE`

### 8. Slot papan digabung ke `session_card_positions`

`session_board_slots` dihapus.

`session_card_positions` menjadi satu-satunya sumber posisi kartu dan wajib punya:

- `board_code`
- `slot_index`
- `location_type`
- `position_order`
- `card_type`
- `card_ref_code`
- `state_json`

Dengan ini satu baris bisa merepresentasikan:

- kartu di deck
- kartu di discard
- kartu di bank
- kartu di player area
- kartu di board slot tertentu

### 9. Histori donasi digabung ke agregat tunggal

Tabel berikut dihapus:

- `peduli_donasi_events`
- `peduli_donasi_rankings`

Tabel baru:

- `session_donation_events`

Kolom inti:

- `session_donation_event_id uuid primary key`
- `session_id uuid not null`
- `event_ke int not null`
- `day int not null`
- `rankings_json jsonb not null`
- `created_at timestamptz not null default now()`

`rankings_json` menyimpan snapshot ranking saat event donasi tersebut selesai.

## Master Data

Master berikut tetap sebagai tabel seed DML:

- `app_menus`
- `role_menu_permissions`
- `actions`
- `ingredients`
- `game_components`

Tabel-tabel ini tetap di DDL, tetapi diperlakukan sebagai katalog/master yang hanya diisi dari seed.

## Boundary Runtime

### RulesetRepository

Membaca:

- `rulesets`
- `ruleset_versions`
- `ruleset_catalog_items`
- `ruleset_catalog_item_requirements`

Tidak lagi membaca tabel typed ruleset lama.

### SessionStateRepository

Berubah dari pola “query banyak tabel lalu sintesis config” menjadi:

1. load `ruleset_versions.config_json`
2. load seluruh katalog ruleset generik
3. materialize ke model domain runtime
4. simpan state sesi ke tabel state aktif

Tabel state aktif yang tetap dipertahankan:

- `session_states`
- `session_player_states`
- `session_player_ingredients`
- `session_player_needs`
- `session_player_financial_goals`
- `session_player_collection_missions`
- `session_player_quest_progress`
- `session_player_action_counters`
- `session_player_peduli_donasi`
- `session_player_assets`
- `session_card_positions`
- `session_donation_events`

Tabel-tabel ini dipertahankan karena mereka merepresentasikan state aktif yang sering dibaca/ditulis selama sesi.

### EventRepository

Seluruh read/write subtype event akan memakai:

- `events.action_id`
- `events.payload`

Repository tidak lagi mengandalkan `left join` ke tabel detail subtype.

### Analytics

`event_cashflow_projections` dan `metric_snapshots` tetap relasional, tetapi kalkulasi sumbernya berubah menjadi parsing `events.payload`.

### Audit dan validation

Repository audit/validation membaca detail dari kolom JSON parent, bukan tabel anak field-level.

## Pemetaan Fitur Lama ke Struktur Baru

Semua fitur gameplay yang ada harus tetap hidup:

- quest
- narasi
- misi koleksi
- tie-breaker
- donasi dan ranking
- investasi emas
- pinjaman syariah
- asuransi
- risiko kehidupan
- tabungan/tujuan finansial
- event timeline
- validasi
- audit keamanan

Aturan implementasi:

- bila fitur bersifat konfigurasi ruleset, ia hidup di `config_json` atau `ruleset_catalog_items`
- bila fitur bersifat detail event, ia hidup di `events.payload`
- bila fitur bersifat state aktif pemain/sesi, ia hidup di tabel session state relasional

## Bootstrap dan Seed

Karena target adalah bootstrap dari nol:

- `database/00_create_schema.sql` ditulis langsung dalam bentuk skema baru
- `database/01_seed_default_rulesets_components.sql` dirombak agar mengisi `config_json` dan katalog generik
- `database/02_seed_full_inspection.sql` dirombak agar memakai `events.payload`, `session_player_assets`, `session_card_positions`, dan `session_donation_events`
- `DatabaseInitialization` hanya perlu tahu struktur baru

## Kontrak API

Kontrak API boleh berubah total.

Implikasinya:

- DTO boleh disederhanakan agar selaras dengan skema baru
- response ruleset dapat mengacu langsung ke config/katalog baru
- response event dapat mengembalikan payload yang lebih dekat ke `events.payload`
- test integration tidak perlu mempertahankan bentuk lama selama perilaku gameplay tetap tersedia

## Strategi Implementasi

Karena perubahan menyentuh seluruh stack, implementasi harus dibagi menjadi beberapa fase:

1. kunci target schema baru lewat test teks dan test bootstrap
2. ubah DDL dan seed dasar
3. ubah EF model/snapshot agar bootstrap konsisten
4. ubah repository ruleset dan session state ke katalog generik
5. ubah repository event ke payload JSON
6. ubah analytics/logging/audit ke JSON parent
7. ubah DTO/API/test integration yang terdampak
8. jalankan verifikasi bootstrap dan gameplay utama

## Risiko

### Risiko utama

- parsing JSON menjadi pusat kompleksitas baru
- struktur payload yang longgar bisa menimbulkan bug runtime yang sulit dilacak
- repository state dan event akan mengalami perubahan besar sekaligus

### Mitigasi

- definisikan kontrak JSON yang ketat di kode
- buat helper parser/domain factory terpusat, jangan parsing JSON bebas di banyak tempat
- tambah test integration untuk setiap capability gameplay utama
- prioritaskan query relasional tetap ada untuk area yang sering diakses: sesi, pemain, timeline event, proyeksi cashflow, snapshot metrik

## Verifikasi Wajib

Sebelum refactor dianggap selesai, semua hal berikut harus lolos:

- bootstrap database baru dari nol berhasil
- seed default ruleset berhasil menghasilkan konfigurasi lengkap
- pembuatan session state berhasil untuk mode `PEMULA` dan `MAHIR`
- save/load session state tetap menjaga fitur quest, narasi, misi, tie-breaker, dan aset pemain
- event ingestion dan event readback berhasil lewat `events.payload`
- analytics tetap bisa menghasilkan `event_cashflow_projections` dan `metric_snapshots`
- validation log, session action log, dan security audit tetap menyimpan detail yang cukup
- test yang mengunci fitur gameplay utama lulus pada skema baru

## Non-Goal Teknis

- tidak menjaga kompatibilitas migration chain lama
- tidak menjaga kompatibilitas seed lama
- tidak mempertahankan nama/shape DTO lama bila itu menghambat penyederhanaan
- tidak mendesain jalur dual-write atau compatibility adapter untuk schema lama
