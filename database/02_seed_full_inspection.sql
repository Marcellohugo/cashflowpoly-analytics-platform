-- Fungsi file: Menyemai dataset inspeksi penuh yang kaya variasi untuk pengujian UI, API, analitik, ruleset, audit, dan validasi.
create extension if not exists pgcrypto;

begin;

-- =========================================================
-- 0) Bersihkan ulang seluruh data inspeksi deterministik
-- =========================================================
drop table if exists seed_cleanup_sessions;
drop table if exists seed_cleanup_rulesets;
drop table if exists seed_cleanup_users;
drop table if exists seed_cleanup_players;

create temporary table seed_cleanup_sessions on commit drop as
select session_id
from sessions
where session_id::text like any (array[
  '80000000-%',
  '89000000-%',
  '81000000-%'
]);

create temporary table seed_cleanup_rulesets on commit drop as
select ruleset_id
from rulesets
where ruleset_id::text like any (array[
  '70000000-%',
  '78000000-%',
  '79000000-%',
  '72000000-%'
]);

create temporary table seed_cleanup_users on commit drop as
select user_id
from app_users
where user_id::text like any (array[
  '10000000-%',
  '12000000-%',
  '20000000-%',
  '22000000-%'
]);

create temporary table seed_cleanup_players on commit drop as
select player_id
from players
where player_id::text like any (array[
  '20000000-%',
  '22000000-%'
]);

delete from security_audit_logs
where security_audit_log_id::text like any (array['f5%', 'f9%'])
   or trace_id like 'seed-inspection%'
   or trace_id like 'seed-complex%'
   or user_id in (select user_id from seed_cleanup_users);

delete from event_cashflow_projections
where session_id in (select session_id from seed_cleanup_sessions);

delete from validation_logs
where session_id in (select session_id from seed_cleanup_sessions);

delete from metric_snapshots
where session_id in (select session_id from seed_cleanup_sessions);

delete from events
where session_id in (select session_id from seed_cleanup_sessions);

delete from session_ruleset_activations
where session_id in (select session_id from seed_cleanup_sessions);

delete from session_players
where session_id in (select session_id from seed_cleanup_sessions);

delete from sessions
where session_id in (select session_id from seed_cleanup_sessions);

delete from ruleset_versions
where ruleset_id in (select ruleset_id from seed_cleanup_rulesets)
   or ruleset_version_id::text like any (array[
     '71000000-%',
     '72000000-%',
     '73000000-%',
     '74000000-%',
     '78100000-%',
     '79100000-%',
     '72100000-%'
   ]);

delete from rulesets
where ruleset_id in (select ruleset_id from seed_cleanup_rulesets);

delete from user_player_links
where user_id in (select user_id from seed_cleanup_users)
   or player_id in (select player_id from seed_cleanup_players)
   or link_id::text like any (array['21000000-%', '23000000-%']);

-- Akun dan profil seed sengaja tidak dihapus karena bisa dipakai ulang oleh sesi
-- lokal di luar rentang session_id seed. Bagian insert memakai ON CONFLICT untuk
-- menyegarkan data deterministik tanpa memutus foreign key sesi lokal.

-- =========================================================
-- 1) Akun, profil pemain, dan link login
-- =========================================================
insert into app_users (
  user_id,
  username,
  password_hash,
  role,
  is_active,
  created_at
)
values
  ('12000000-0000-0000-0000-000000000001', 'mira.hartanto', crypt('MiraAudit!2026', gen_salt('bf', 10)), 'INSTRUCTOR', true, '2026-05-10T07:30:00+07:00'),
  ('12000000-0000-0000-0000-000000000002', 'bayu.prakasa', crypt('BayuAudit!2026', gen_salt('bf', 10)), 'INSTRUCTOR', true, '2026-05-10T07:35:00+07:00'),
  ('12000000-0000-0000-0000-000000000003', 'sindy.lestari', crypt('SindyAudit!2026', gen_salt('bf', 10)), 'INSTRUCTOR', true, '2026-05-10T07:40:00+07:00'),
  ('12000000-0000-0000-0000-000000000004', 'arman.wijaya', crypt('ArmanAudit!2026', gen_salt('bf', 10)), 'INSTRUCTOR', false, '2026-05-10T07:45:00+07:00'),
  ('22000000-0000-0000-0000-000000000001', 'nadia.putri', crypt('NadiaAudit!2026', gen_salt('bf', 10)), 'PLAYER', true, '2026-05-10T08:00:00+07:00'),
  ('22000000-0000-0000-0000-000000000002', 'rangga.maulana', crypt('RanggaAudit!2026', gen_salt('bf', 10)), 'PLAYER', true, '2026-05-10T08:01:00+07:00'),
  ('22000000-0000-0000-0000-000000000003', 'safira.anindya', crypt('SafiraAudit!2026', gen_salt('bf', 10)), 'PLAYER', true, '2026-05-10T08:02:00+07:00'),
  ('22000000-0000-0000-0000-000000000004', 'teo.prasetyo', crypt('TeoAudit!2026', gen_salt('bf', 10)), 'PLAYER', true, '2026-05-10T08:03:00+07:00'),
  ('22000000-0000-0000-0000-000000000005', 'ulfa.ramadhani', crypt('UlfaAudit!2026', gen_salt('bf', 10)), 'PLAYER', true, '2026-05-10T08:04:00+07:00'),
  ('22000000-0000-0000-0000-000000000006', 'vina.anggraini', crypt('VinaAudit!2026', gen_salt('bf', 10)), 'PLAYER', true, '2026-05-10T08:05:00+07:00'),
  ('22000000-0000-0000-0000-000000000007', 'wahyu.firmansyah', crypt('WahyuAudit!2026', gen_salt('bf', 10)), 'PLAYER', true, '2026-05-10T08:06:00+07:00'),
  ('22000000-0000-0000-0000-000000000008', 'xenia.kusuma', crypt('XeniaAudit!2026', gen_salt('bf', 10)), 'PLAYER', true, '2026-05-10T08:07:00+07:00'),
  ('22000000-0000-0000-0000-000000000009', 'yudha.permana', crypt('YudhaAudit!2026', gen_salt('bf', 10)), 'PLAYER', true, '2026-05-10T08:08:00+07:00'),
  ('22000000-0000-0000-0000-000000000010', 'zara.nuraini', crypt('ZaraAudit!2026', gen_salt('bf', 10)), 'PLAYER', true, '2026-05-10T08:09:00+07:00'),
  ('22000000-0000-0000-0000-000000000011', 'adit.suryana', crypt('AditAudit!2026', gen_salt('bf', 10)), 'PLAYER', true, '2026-05-10T08:10:00+07:00'),
  ('22000000-0000-0000-0000-000000000012', 'bella.kartika', crypt('BellaAudit!2026', gen_salt('bf', 10)), 'PLAYER', false, '2026-05-10T08:11:00+07:00'),
  ('22000000-0000-0000-0000-000000000013', 'chandra.gunawan', crypt('ChandraAudit!2026', gen_salt('bf', 10)), 'PLAYER', true, '2026-05-10T08:12:00+07:00'),
  ('22000000-0000-0000-0000-000000000014', 'dinda.ayu', crypt('DindaAudit!2026', gen_salt('bf', 10)), 'PLAYER', true, '2026-05-10T08:13:00+07:00'),
  ('22000000-0000-0000-0000-000000000015', 'elang.nugroho', crypt('ElangAudit!2026', gen_salt('bf', 10)), 'PLAYER', true, '2026-05-10T08:14:00+07:00'),
  ('22000000-0000-0000-0000-000000000016', 'fiona.melati', crypt('FionaAudit!2026', gen_salt('bf', 10)), 'PLAYER', true, '2026-05-10T08:15:00+07:00')
on conflict (user_id) do update
set username = excluded.username,
    password_hash = excluded.password_hash,
    role = excluded.role,
    is_active = excluded.is_active,
    created_at = excluded.created_at;

insert into players (
  player_id,
  display_name,
  instructor_user_id,
  created_at
)
values
  ('22000000-0000-0000-0000-000000000001', 'Nadia Putri', '12000000-0000-0000-0000-000000000001', '2026-05-10T08:00:00+07:00'),
  ('22000000-0000-0000-0000-000000000002', 'Rangga Maulana', '12000000-0000-0000-0000-000000000001', '2026-05-10T08:01:00+07:00'),
  ('22000000-0000-0000-0000-000000000003', 'Safira Anindya', '12000000-0000-0000-0000-000000000001', '2026-05-10T08:02:00+07:00'),
  ('22000000-0000-0000-0000-000000000004', 'Teo Prasetyo', '12000000-0000-0000-0000-000000000001', '2026-05-10T08:03:00+07:00'),
  ('22000000-0000-0000-0000-000000000005', 'Ulfa Ramadhani', '12000000-0000-0000-0000-000000000001', '2026-05-10T08:04:00+07:00'),
  ('22000000-0000-0000-0000-000000000006', 'Vina Anggraini', '12000000-0000-0000-0000-000000000001', '2026-05-10T08:05:00+07:00'),
  ('22000000-0000-0000-0000-000000000007', 'Wahyu Firmansyah', '12000000-0000-0000-0000-000000000001', '2026-05-10T08:06:00+07:00'),
  ('22000000-0000-0000-0000-000000000008', 'Xenia Kusuma', '12000000-0000-0000-0000-000000000001', '2026-05-10T08:07:00+07:00'),
  ('22000000-0000-0000-0000-000000000009', 'Yudha Permana', '12000000-0000-0000-0000-000000000002', '2026-05-10T08:08:00+07:00'),
  ('22000000-0000-0000-0000-000000000010', 'Zara Nuraini', '12000000-0000-0000-0000-000000000002', '2026-05-10T08:09:00+07:00'),
  ('22000000-0000-0000-0000-000000000011', 'Adit Suryana', '12000000-0000-0000-0000-000000000002', '2026-05-10T08:10:00+07:00'),
  ('22000000-0000-0000-0000-000000000012', 'Bella Kartika', '12000000-0000-0000-0000-000000000002', '2026-05-10T08:11:00+07:00'),
  ('22000000-0000-0000-0000-000000000013', 'Chandra Gunawan', '12000000-0000-0000-0000-000000000003', '2026-05-10T08:12:00+07:00'),
  ('22000000-0000-0000-0000-000000000014', 'Dinda Ayu', '12000000-0000-0000-0000-000000000003', '2026-05-10T08:13:00+07:00'),
  ('22000000-0000-0000-0000-000000000015', 'Elang Nugroho', '12000000-0000-0000-0000-000000000003', '2026-05-10T08:14:00+07:00'),
  ('22000000-0000-0000-0000-000000000016', 'Fiona Melati', '12000000-0000-0000-0000-000000000003', '2026-05-10T08:15:00+07:00')
on conflict (player_id) do update
set display_name = excluded.display_name,
    instructor_user_id = excluded.instructor_user_id,
    created_at = excluded.created_at;

insert into user_player_links (
  link_id,
  user_id,
  player_id,
  created_at
)
values
  ('23000000-0000-0000-0000-000000000001', '22000000-0000-0000-0000-000000000001', '22000000-0000-0000-0000-000000000001', '2026-05-10T08:00:30+07:00'),
  ('23000000-0000-0000-0000-000000000002', '22000000-0000-0000-0000-000000000002', '22000000-0000-0000-0000-000000000002', '2026-05-10T08:01:30+07:00'),
  ('23000000-0000-0000-0000-000000000003', '22000000-0000-0000-0000-000000000003', '22000000-0000-0000-0000-000000000003', '2026-05-10T08:02:30+07:00'),
  ('23000000-0000-0000-0000-000000000004', '22000000-0000-0000-0000-000000000004', '22000000-0000-0000-0000-000000000004', '2026-05-10T08:03:30+07:00'),
  ('23000000-0000-0000-0000-000000000005', '22000000-0000-0000-0000-000000000005', '22000000-0000-0000-0000-000000000005', '2026-05-10T08:04:30+07:00'),
  ('23000000-0000-0000-0000-000000000006', '22000000-0000-0000-0000-000000000006', '22000000-0000-0000-0000-000000000006', '2026-05-10T08:05:30+07:00'),
  ('23000000-0000-0000-0000-000000000007', '22000000-0000-0000-0000-000000000007', '22000000-0000-0000-0000-000000000007', '2026-05-10T08:06:30+07:00'),
  ('23000000-0000-0000-0000-000000000008', '22000000-0000-0000-0000-000000000008', '22000000-0000-0000-0000-000000000008', '2026-05-10T08:07:30+07:00'),
  ('23000000-0000-0000-0000-000000000009', '22000000-0000-0000-0000-000000000009', '22000000-0000-0000-0000-000000000009', '2026-05-10T08:08:30+07:00'),
  ('23000000-0000-0000-0000-000000000010', '22000000-0000-0000-0000-000000000010', '22000000-0000-0000-0000-000000000010', '2026-05-10T08:09:30+07:00'),
  ('23000000-0000-0000-0000-000000000011', '22000000-0000-0000-0000-000000000011', '22000000-0000-0000-0000-000000000011', '2026-05-10T08:10:30+07:00'),
  ('23000000-0000-0000-0000-000000000012', '22000000-0000-0000-0000-000000000012', '22000000-0000-0000-0000-000000000012', '2026-05-10T08:11:30+07:00'),
  ('23000000-0000-0000-0000-000000000013', '22000000-0000-0000-0000-000000000013', '22000000-0000-0000-0000-000000000013', '2026-05-10T08:12:30+07:00'),
  ('23000000-0000-0000-0000-000000000014', '22000000-0000-0000-0000-000000000014', '22000000-0000-0000-0000-000000000014', '2026-05-10T08:13:30+07:00'),
  ('23000000-0000-0000-0000-000000000015', '22000000-0000-0000-0000-000000000015', '22000000-0000-0000-0000-000000000015', '2026-05-10T08:14:30+07:00'),
  ('23000000-0000-0000-0000-000000000016', '22000000-0000-0000-0000-000000000016', '22000000-0000-0000-0000-000000000016', '2026-05-10T08:15:30+07:00')
on conflict (user_id) do update
set link_id = excluded.link_id,
    player_id = excluded.player_id,
    created_at = excluded.created_at;

-- =========================================================
-- 2) Ruleset inspeksi dengan status, mode, dan konfigurasi berbeda
-- =========================================================
insert into rulesets (
  ruleset_id,
  name,
  description,
  instructor_user_id,
  created_at,
  created_by
)
values
  ('72000000-0000-0000-0000-000000000001', 'Pemula Ketat - Primer Dahulu', 'Ruleset pemula dengan constraint primer ketat, versi retired, dan versi aktif.', '12000000-0000-0000-0000-000000000001', '2026-05-11T09:00:00+07:00', 'system-seed-inspection-v4'),
  ('72000000-0000-0000-0000-000000000002', 'Mahir Risiko Tinggi - Investasi dan Proteksi', 'Ruleset mahir penuh untuk tabungan, pinjaman, asuransi, risiko, emas, dan donasi.', '12000000-0000-0000-0000-000000000001', '2026-05-11T09:10:00+07:00', 'system-seed-inspection-v4'),
  ('72000000-0000-0000-0000-000000000003', 'Mahir Hemat - Sabtu Emas Nonaktif', 'Ruleset mahir dengan perdagangan emas nonaktif agar validasi negatif mudah diperiksa.', '12000000-0000-0000-0000-000000000002', '2026-05-11T09:20:00+07:00', 'system-seed-inspection-v4'),
  ('72000000-0000-0000-0000-000000000004', 'Pemula Keluarga - Freelance Lebih Kuat', 'Ruleset pemula untuk kelas keluarga dengan pendapatan freelance lebih besar.', '12000000-0000-0000-0000-000000000002', '2026-05-11T09:30:00+07:00', 'system-seed-inspection-v4'),
  ('72000000-0000-0000-0000-000000000005', 'Draft Turnamen Semester', 'Ruleset draft yang sengaja belum aktif untuk menguji status dan penolakan aktivasi.', '12000000-0000-0000-0000-000000000003', '2026-05-11T09:40:00+07:00', 'system-seed-inspection-v4')
on conflict (ruleset_id) do update
set name = excluded.name,
    description = excluded.description,
    instructor_user_id = excluded.instructor_user_id,
    created_at = excluded.created_at,
    created_by = excluded.created_by;

with cfg as (
  select
    $json$
    {
      "mode": "PEMULA",
      "actions_per_turn": 2,
      "starting_cash": 18,
      "player_ordering": "JOIN_ORDER",
      "weekday_rules": {
        "friday": { "enabled": true },
        "saturday": { "enabled": true },
        "sunday": { "enabled": true }
      },
      "constraints": {
        "cash_min": 0,
        "max_ingredient_total": 5,
        "max_same_ingredient": 2,
        "primary_need_max_per_day": 1,
        "require_primary_before_others": true
      },
      "donation": { "min_amount": 1, "max_amount": 8 },
      "gold_trade": { "allow_buy": true, "allow_sell": true },
      "advanced": {
        "loan": { "enabled": false },
        "insurance": { "enabled": false },
        "saving_goal": { "enabled": false }
      },
      "freelance": { "income": 1 },
      "scoring": {
        "donation_rank_points": [{ "rank": 1, "points": 7 }, { "rank": 2, "points": 5 }, { "rank": 3, "points": 2 }],
        "gold_points_by_qty": [{ "qty": 1, "points": 3 }, { "qty": 2, "points": 5 }, { "qty": 3, "points": 8 }],
        "pension_rank_points": [{ "rank": 1, "points": 7 }, { "rank": 2, "points": 5 }, { "rank": 3, "points": 2 }]
      },
      "scenario_tags": ["retired-baseline", "low-cash", "strict-primary"]
    }
    $json$::jsonb as config_json
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
  '72100000-0000-0000-0000-000000000001',
  '72000000-0000-0000-0000-000000000001',
  1,
  'RETIRED',
  config_json,
  encode(digest(config_json::text, 'sha256'), 'hex'),
  '2026-05-11T09:00:30+07:00',
  'system-seed-inspection-v4'
from cfg
on conflict (ruleset_id, version) do update
set status = excluded.status,
    config_json = excluded.config_json,
    config_hash = excluded.config_hash,
    created_at = excluded.created_at,
    created_by = excluded.created_by;

with cfg as (
  select
    $json$
    {
      "mode": "PEMULA",
      "actions_per_turn": 2,
      "starting_cash": 24,
      "player_ordering": "INSTRUCTOR_ORDER",
      "instructor_player_usernames": ["nadia.putri", "rangga.maulana", "safira.anindya", "teo.prasetyo"],
      "weekday_rules": {
        "friday": { "enabled": true },
        "saturday": { "enabled": true },
        "sunday": { "enabled": true }
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
      "freelance": { "income": 2 },
      "scoring": {
        "donation_rank_points": [{ "rank": 1, "points": 7 }, { "rank": 2, "points": 5 }, { "rank": 3, "points": 2 }],
        "gold_points_by_qty": [{ "qty": 1, "points": 3 }, { "qty": 2, "points": 5 }, { "qty": 3, "points": 8 }, { "qty": 4, "points": 12 }],
        "pension_rank_points": [{ "rank": 1, "points": 7 }, { "rank": 2, "points": 5 }, { "rank": 3, "points": 2 }]
      },
      "scenario_tags": ["active-beginner", "manual-order", "balanced"]
    }
    $json$::jsonb as config_json
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
  '72100000-0000-0000-0000-000000000002',
  '72000000-0000-0000-0000-000000000001',
  2,
  'ACTIVE',
  config_json,
  encode(digest(config_json::text, 'sha256'), 'hex'),
  '2026-05-11T09:05:00+07:00',
  'system-seed-inspection-v4'
from cfg
on conflict (ruleset_id, version) do update
set status = excluded.status,
    config_json = excluded.config_json,
    config_hash = excluded.config_hash,
    created_at = excluded.created_at,
    created_by = excluded.created_by;

with cfg as (
  select
    $json$
    {
      "mode": "MAHIR",
      "actions_per_turn": 2,
      "starting_cash": 12,
      "player_ordering": "EVENT_SEQUENCE",
      "weekday_rules": {
        "friday": { "enabled": true },
        "saturday": { "enabled": true },
        "sunday": { "enabled": true }
      },
      "constraints": {
        "cash_min": 0,
        "max_ingredient_total": 8,
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
      "freelance": { "income": 2 },
      "scoring": {
        "donation_rank_points": [{ "rank": 1, "points": 9 }, { "rank": 2, "points": 6 }, { "rank": 3, "points": 3 }],
        "gold_points_by_qty": [{ "qty": 1, "points": 3 }, { "qty": 2, "points": 5 }, { "qty": 3, "points": 8 }, { "qty": 5, "points": 15 }],
        "pension_rank_points": [{ "rank": 1, "points": 8 }, { "rank": 2, "points": 5 }, { "rank": 3, "points": 3 }]
      },
      "scenario_tags": ["advanced", "risk-heavy", "insurance", "loan", "saving-goal"]
    }
    $json$::jsonb as config_json
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
  '72100000-0000-0000-0000-000000000003',
  '72000000-0000-0000-0000-000000000002',
  1,
  'ACTIVE',
  config_json,
  encode(digest(config_json::text, 'sha256'), 'hex'),
  '2026-05-11T09:10:30+07:00',
  'system-seed-inspection-v4'
from cfg
on conflict (ruleset_id, version) do update
set status = excluded.status,
    config_json = excluded.config_json,
    config_hash = excluded.config_hash,
    created_at = excluded.created_at,
    created_by = excluded.created_by;

with cfg as (
  select
    $json$
    {
      "mode": "MAHIR",
      "actions_per_turn": 2,
      "starting_cash": 16,
      "player_ordering": "USERNAME",
      "weekday_rules": {
        "friday": { "enabled": true },
        "saturday": { "enabled": false },
        "sunday": { "enabled": true }
      },
      "constraints": {
        "cash_min": 0,
        "max_ingredient_total": 6,
        "max_same_ingredient": 2,
        "primary_need_max_per_day": 1,
        "require_primary_before_others": true
      },
      "donation": { "min_amount": 1, "max_amount": 5 },
      "gold_trade": { "allow_buy": false, "allow_sell": false },
      "advanced": {
        "loan": { "enabled": true },
        "insurance": { "enabled": true },
        "saving_goal": { "enabled": true }
      },
      "freelance": { "income": 1 },
      "scoring": {
        "donation_rank_points": [{ "rank": 1, "points": 7 }, { "rank": 2, "points": 4 }, { "rank": 3, "points": 1 }],
        "gold_points_by_qty": [{ "qty": 1, "points": 2 }, { "qty": 2, "points": 4 }, { "qty": 3, "points": 7 }],
        "pension_rank_points": [{ "rank": 1, "points": 7 }, { "rank": 2, "points": 5 }, { "rank": 3, "points": 2 }]
      },
      "scenario_tags": ["advanced", "negative-validation", "username-order", "gold-disabled"]
    }
    $json$::jsonb as config_json
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
  '72100000-0000-0000-0000-000000000004',
  '72000000-0000-0000-0000-000000000003',
  1,
  'ACTIVE',
  config_json,
  encode(digest(config_json::text, 'sha256'), 'hex'),
  '2026-05-11T09:20:30+07:00',
  'system-seed-inspection-v4'
from cfg
on conflict (ruleset_id, version) do update
set status = excluded.status,
    config_json = excluded.config_json,
    config_hash = excluded.config_hash,
    created_at = excluded.created_at,
    created_by = excluded.created_by;

with cfg as (
  select
    $json$
    {
      "mode": "PEMULA",
      "actions_per_turn": 3,
      "starting_cash": 30,
      "player_ordering": "JOIN_ORDER",
      "weekday_rules": {
        "friday": { "enabled": true },
        "saturday": { "enabled": true },
        "sunday": { "enabled": true }
      },
      "constraints": {
        "cash_min": 0,
        "max_ingredient_total": 8,
        "max_same_ingredient": 4,
        "primary_need_max_per_day": 2,
        "require_primary_before_others": false
      },
      "donation": { "min_amount": 1, "max_amount": 999999 },
      "gold_trade": { "allow_buy": true, "allow_sell": true },
      "advanced": {
        "loan": { "enabled": false },
        "insurance": { "enabled": false },
        "saving_goal": { "enabled": false }
      },
      "freelance": { "income": 3 },
      "scoring": {
        "donation_rank_points": [{ "rank": 1, "points": 6 }, { "rank": 2, "points": 4 }, { "rank": 3, "points": 2 }],
        "gold_points_by_qty": [{ "qty": 1, "points": 2 }, { "qty": 2, "points": 5 }, { "qty": 4, "points": 10 }],
        "pension_rank_points": [{ "rank": 1, "points": 6 }, { "rank": 2, "points": 4 }, { "rank": 3, "points": 2 }]
      },
      "scenario_tags": ["beginner", "high-cash", "family-class", "three-actions"]
    }
    $json$::jsonb as config_json
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
  '72100000-0000-0000-0000-000000000005',
  '72000000-0000-0000-0000-000000000004',
  1,
  'ACTIVE',
  config_json,
  encode(digest(config_json::text, 'sha256'), 'hex'),
  '2026-05-11T09:30:30+07:00',
  'system-seed-inspection-v4'
from cfg
on conflict (ruleset_id, version) do update
set status = excluded.status,
    config_json = excluded.config_json,
    config_hash = excluded.config_hash,
    created_at = excluded.created_at,
    created_by = excluded.created_by;

with cfg as (
  select
    $json$
    {
      "mode": "MAHIR",
      "actions_per_turn": 3,
      "starting_cash": 20,
      "player_ordering": "PLAYER_ID",
      "weekday_rules": {
        "friday": { "enabled": false },
        "saturday": { "enabled": true },
        "sunday": { "enabled": false }
      },
      "constraints": {
        "cash_min": 0,
        "max_ingredient_total": 9,
        "max_same_ingredient": 3,
        "primary_need_max_per_day": 1,
        "require_primary_before_others": true
      },
      "donation": { "min_amount": 2, "max_amount": 10 },
      "gold_trade": { "allow_buy": true, "allow_sell": true },
      "advanced": {
        "loan": { "enabled": true },
        "insurance": { "enabled": true },
        "saving_goal": { "enabled": true }
      },
      "freelance": { "income": 2 },
      "scoring": {
        "donation_rank_points": [{ "rank": 1, "points": 10 }, { "rank": 2, "points": 6 }, { "rank": 3, "points": 2 }],
        "gold_points_by_qty": [{ "qty": 1, "points": 4 }, { "qty": 2, "points": 7 }, { "qty": 3, "points": 11 }],
        "pension_rank_points": [{ "rank": 1, "points": 9 }, { "rank": 2, "points": 6 }, { "rank": 3, "points": 3 }]
      },
      "scenario_tags": ["draft", "tournament", "mixed-weekday"]
    }
    $json$::jsonb as config_json
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
  '72100000-0000-0000-0000-000000000006',
  '72000000-0000-0000-0000-000000000005',
  1,
  'DRAFT',
  config_json,
  encode(digest(config_json::text, 'sha256'), 'hex'),
  '2026-05-11T09:40:30+07:00',
  'system-seed-inspection-v4'
from cfg
on conflict (ruleset_id, version) do update
set status = excluded.status,
    config_json = excluded.config_json,
    config_hash = excluded.config_hash,
    created_at = excluded.created_at,
    created_by = excluded.created_by;

-- =========================================================
-- 3) Sesi, peserta, dan aktivasi ruleset
-- =========================================================
insert into sessions (
  session_id,
  session_name,
  mode,
  status,
  started_at,
  ended_at,
  instructor_user_id,
  created_at
)
values
  ('81000000-0000-0000-0000-000000000001', 'Pemula Ketat - Sesi Selesai Variatif', 'PEMULA', 'ENDED', '2026-05-12T08:00:00+07:00', '2026-05-12T09:35:00+07:00', '12000000-0000-0000-0000-000000000001', '2026-05-11T13:00:00+07:00'),
  ('81000000-0000-0000-0000-000000000002', 'Mahir Risiko Tinggi - Final Kaya Event', 'MAHIR', 'ENDED', '2026-05-13T08:00:00+07:00', '2026-05-13T10:20:00+07:00', '12000000-0000-0000-0000-000000000001', '2026-05-12T13:00:00+07:00'),
  ('81000000-0000-0000-0000-000000000003', 'Mahir Sabtu Nonaktif - Banyak Pelanggaran', 'MAHIR', 'ENDED', '2026-05-14T13:00:00+07:00', '2026-05-14T14:55:00+07:00', '12000000-0000-0000-0000-000000000002', '2026-05-13T13:00:00+07:00'),
  ('81000000-0000-0000-0000-000000000004', 'Pemula Live - Kelas Sore', 'PEMULA', 'STARTED', '2026-05-15T15:30:00+07:00', null, '12000000-0000-0000-0000-000000000002', '2026-05-14T13:00:00+07:00'),
  ('81000000-0000-0000-0000-000000000005', 'Mahir Live - Tabungan dan Risiko', 'MAHIR', 'STARTED', '2026-05-16T09:15:00+07:00', null, '12000000-0000-0000-0000-000000000003', '2026-05-15T13:00:00+07:00'),
  ('81000000-0000-0000-0000-000000000006', 'Draft Belum Dimulai - Cek Penolakan', 'MAHIR', 'CREATED', null, null, '12000000-0000-0000-0000-000000000003', '2026-05-16T13:00:00+07:00'),
  ('81000000-0000-0000-0000-000000000007', 'Pemula Tanpa Event - Empty State', 'PEMULA', 'CREATED', null, null, '12000000-0000-0000-0000-000000000001', '2026-05-17T13:00:00+07:00'),
  ('81000000-0000-0000-0000-000000000008', 'Mahir Ended Mini - Username Ordering', 'MAHIR', 'ENDED', '2026-05-18T08:00:00+07:00', '2026-05-18T09:05:00+07:00', '12000000-0000-0000-0000-000000000002', '2026-05-17T14:00:00+07:00')
on conflict (session_id) do update
set session_name = excluded.session_name,
    mode = excluded.mode,
    status = excluded.status,
    started_at = excluded.started_at,
    ended_at = excluded.ended_at,
    instructor_user_id = excluded.instructor_user_id,
    created_at = excluded.created_at;

insert into session_players (
  session_player_id,
  session_id,
  player_id,
  join_order,
  role,
  created_at
)
values
  ('81100000-0000-0000-0000-000000000001', '81000000-0000-0000-0000-000000000001', '22000000-0000-0000-0000-000000000001', 1, 'PLAYER', '2026-05-12T07:45:00+07:00'),
  ('81100000-0000-0000-0000-000000000002', '81000000-0000-0000-0000-000000000001', '22000000-0000-0000-0000-000000000002', 2, 'PLAYER', '2026-05-12T07:46:00+07:00'),
  ('81100000-0000-0000-0000-000000000003', '81000000-0000-0000-0000-000000000001', '22000000-0000-0000-0000-000000000003', 3, 'PLAYER', '2026-05-12T07:47:00+07:00'),
  ('81100000-0000-0000-0000-000000000004', '81000000-0000-0000-0000-000000000001', '22000000-0000-0000-0000-000000000004', 4, 'PLAYER', '2026-05-12T07:48:00+07:00'),
  ('81100000-0000-0000-0000-000000000005', '81000000-0000-0000-0000-000000000002', '22000000-0000-0000-0000-000000000005', 1, 'PLAYER', '2026-05-13T07:45:00+07:00'),
  ('81100000-0000-0000-0000-000000000006', '81000000-0000-0000-0000-000000000002', '22000000-0000-0000-0000-000000000006', 2, 'PLAYER', '2026-05-13T07:46:00+07:00'),
  ('81100000-0000-0000-0000-000000000007', '81000000-0000-0000-0000-000000000002', '22000000-0000-0000-0000-000000000007', 3, 'PLAYER', '2026-05-13T07:47:00+07:00'),
  ('81100000-0000-0000-0000-000000000008', '81000000-0000-0000-0000-000000000002', '22000000-0000-0000-0000-000000000008', 4, 'PLAYER', '2026-05-13T07:48:00+07:00'),
  ('81100000-0000-0000-0000-000000000009', '81000000-0000-0000-0000-000000000003', '22000000-0000-0000-0000-000000000009', 1, 'PLAYER', '2026-05-14T12:45:00+07:00'),
  ('81100000-0000-0000-0000-000000000010', '81000000-0000-0000-0000-000000000003', '22000000-0000-0000-0000-000000000010', 2, 'PLAYER', '2026-05-14T12:46:00+07:00'),
  ('81100000-0000-0000-0000-000000000011', '81000000-0000-0000-0000-000000000003', '22000000-0000-0000-0000-000000000011', 3, 'PLAYER', '2026-05-14T12:47:00+07:00'),
  ('81100000-0000-0000-0000-000000000012', '81000000-0000-0000-0000-000000000003', '22000000-0000-0000-0000-000000000012', 4, 'PLAYER', '2026-05-14T12:48:00+07:00'),
  ('81100000-0000-0000-0000-000000000013', '81000000-0000-0000-0000-000000000004', '22000000-0000-0000-0000-000000000009', 3, 'PLAYER', '2026-05-15T15:10:00+07:00'),
  ('81100000-0000-0000-0000-000000000014', '81000000-0000-0000-0000-000000000004', '22000000-0000-0000-0000-000000000010', 1, 'PLAYER', '2026-05-15T15:11:00+07:00'),
  ('81100000-0000-0000-0000-000000000015', '81000000-0000-0000-0000-000000000004', '22000000-0000-0000-0000-000000000011', 4, 'PLAYER', '2026-05-15T15:12:00+07:00'),
  ('81100000-0000-0000-0000-000000000016', '81000000-0000-0000-0000-000000000004', '22000000-0000-0000-0000-000000000012', 2, 'PLAYER', '2026-05-15T15:13:00+07:00'),
  ('81100000-0000-0000-0000-000000000017', '81000000-0000-0000-0000-000000000005', '22000000-0000-0000-0000-000000000013', 1, 'PLAYER', '2026-05-16T09:00:00+07:00'),
  ('81100000-0000-0000-0000-000000000018', '81000000-0000-0000-0000-000000000005', '22000000-0000-0000-0000-000000000014', 2, 'PLAYER', '2026-05-16T09:01:00+07:00'),
  ('81100000-0000-0000-0000-000000000019', '81000000-0000-0000-0000-000000000005', '22000000-0000-0000-0000-000000000015', 3, 'PLAYER', '2026-05-16T09:02:00+07:00'),
  ('81100000-0000-0000-0000-000000000020', '81000000-0000-0000-0000-000000000005', '22000000-0000-0000-0000-000000000016', 4, 'PLAYER', '2026-05-16T09:03:00+07:00'),
  ('81100000-0000-0000-0000-000000000021', '81000000-0000-0000-0000-000000000006', '22000000-0000-0000-0000-000000000013', 1, 'PLAYER', '2026-05-16T13:05:00+07:00'),
  ('81100000-0000-0000-0000-000000000022', '81000000-0000-0000-0000-000000000006', '22000000-0000-0000-0000-000000000014', 2, 'PLAYER', '2026-05-16T13:06:00+07:00'),
  ('81100000-0000-0000-0000-000000000023', '81000000-0000-0000-0000-000000000007', '22000000-0000-0000-0000-000000000001', 1, 'PLAYER', '2026-05-17T13:05:00+07:00'),
  ('81100000-0000-0000-0000-000000000024', '81000000-0000-0000-0000-000000000008', '22000000-0000-0000-0000-000000000002', 4, 'PLAYER', '2026-05-18T07:45:00+07:00'),
  ('81100000-0000-0000-0000-000000000025', '81000000-0000-0000-0000-000000000008', '22000000-0000-0000-0000-000000000008', 1, 'PLAYER', '2026-05-18T07:46:00+07:00'),
  ('81100000-0000-0000-0000-000000000026', '81000000-0000-0000-0000-000000000008', '22000000-0000-0000-0000-000000000012', 2, 'PLAYER', '2026-05-18T07:47:00+07:00'),
  ('81100000-0000-0000-0000-000000000027', '81000000-0000-0000-0000-000000000008', '22000000-0000-0000-0000-000000000016', 3, 'PLAYER', '2026-05-18T07:48:00+07:00')
on conflict (session_id, player_id) do update
set session_player_id = excluded.session_player_id,
    join_order = excluded.join_order,
    role = excluded.role,
    created_at = excluded.created_at;

insert into session_ruleset_activations (
  activation_id,
  session_id,
  ruleset_version_id,
  activated_at,
  activated_by
)
values
  ('81200000-0000-0000-0000-000000000001', '81000000-0000-0000-0000-000000000001', '72100000-0000-0000-0000-000000000002', '2026-05-12T07:40:00+07:00', 'system-seed-inspection-v4'),
  ('81200000-0000-0000-0000-000000000002', '81000000-0000-0000-0000-000000000002', '72100000-0000-0000-0000-000000000003', '2026-05-13T07:40:00+07:00', 'system-seed-inspection-v4'),
  ('81200000-0000-0000-0000-000000000003', '81000000-0000-0000-0000-000000000003', '72100000-0000-0000-0000-000000000004', '2026-05-14T12:40:00+07:00', 'system-seed-inspection-v4'),
  ('81200000-0000-0000-0000-000000000004', '81000000-0000-0000-0000-000000000004', '72100000-0000-0000-0000-000000000005', '2026-05-15T15:00:00+07:00', 'system-seed-inspection-v4'),
  ('81200000-0000-0000-0000-000000000005', '81000000-0000-0000-0000-000000000005', '72100000-0000-0000-0000-000000000003', '2026-05-16T08:55:00+07:00', 'system-seed-inspection-v4'),
  ('81200000-0000-0000-0000-000000000006', '81000000-0000-0000-0000-000000000006', '72100000-0000-0000-0000-000000000006', '2026-05-16T13:00:00+07:00', 'system-seed-inspection-v4'),
  ('81200000-0000-0000-0000-000000000007', '81000000-0000-0000-0000-000000000007', '72100000-0000-0000-0000-000000000002', '2026-05-17T13:00:00+07:00', 'system-seed-inspection-v4'),
  ('81200000-0000-0000-0000-000000000008', '81000000-0000-0000-0000-000000000008', '72100000-0000-0000-0000-000000000003', '2026-05-18T07:40:00+07:00', 'system-seed-inspection-v4')
on conflict (activation_id) do update
set ruleset_version_id = excluded.ruleset_version_id,
    activated_at = excluded.activated_at,
    activated_by = excluded.activated_by;

-- =========================================================
-- 4) Event gameplay kompleks lintas fitur
-- =========================================================
with seed_sessions as (
  select
    s.session_id,
    s.mode,
    s.status,
    coalesce(s.started_at, s.created_at) as base_time,
    s.ended_at,
    sra.ruleset_version_id,
    right(s.session_id::text, 12)::int as session_no
  from sessions s
  join session_ruleset_activations sra on sra.session_id = s.session_id
  where s.session_id::text like '81000000-%'
),
player_scope as (
  select
    ss.session_id,
    ss.mode,
    ss.status,
    ss.base_time,
    ss.ended_at,
    ss.ruleset_version_id,
    ss.session_no,
    sp.player_id,
    sp.join_order
  from seed_sessions ss
  join session_players sp on sp.session_id = ss.session_id
  where ss.status in ('STARTED', 'ENDED')
),
action_template(action_order, action_type, advanced_only) as (
  values
    (10, 'tie_breaker.assigned', false),
    (11, 'mission.assigned', false),
    (12, 'ingredient.purchased', false),
    (13, 'ingredient.purchased', false),
    (14, 'order.claimed', false),
    (15, 'order.passed', false),
    (16, 'need.primary.purchased', false),
    (17, 'need.secondary.purchased', false),
    (18, 'need.tertiary.purchased', false),
    (19, 'work.freelance.completed', false),
    (20, 'day.friday.donation', false),
    (21, 'day.saturday.gold_trade', false),
    (22, 'turn.action.used', false),
    (23, 'ingredient.discarded', false),
    (24, 'saving.deposit.created', true),
    (25, 'saving.deposit.withdrawn', true),
    (26, 'loan.syariah.taken', true),
    (27, 'insurance.multirisk.purchased', true),
    (28, 'risk.life.drawn', true),
    (29, 'insurance.multirisk.used', true),
    (30, 'risk.emergency.used', true),
    (31, 'loan.syariah.repaid', true),
    (32, 'saving.goal.achieved', true),
    (33, 'gold.points.awarded', false),
    (34, 'pension.rank.awarded', false),
    (35, 'donation.rank.awarded', false)
),
event_source as (
  select
    ss.session_no * 10000 + 1 as source_code,
    ss.session_id,
    null::uuid as player_id,
    'SYSTEM' as actor_type,
    ss.base_time,
    ss.ruleset_version_id,
    0 as join_order,
    1 as action_order,
    'session.created' as action_type,
    ss.status,
    ss.mode,
    jsonb_build_object('seed_case', 'session-created', 'status', ss.status, 'mode', ss.mode) as payload
  from seed_sessions ss
  union all
  select
    ss.session_no * 10000 + 2,
    ss.session_id,
    null::uuid,
    'SYSTEM',
    ss.base_time,
    ss.ruleset_version_id,
    0,
    2,
    'session.started',
    ss.status,
    ss.mode,
    jsonb_build_object('seed_case', 'session-started', 'mode', ss.mode)
  from seed_sessions ss
  where ss.status in ('STARTED', 'ENDED')
  union all
  select
    ps.session_no * 10000 + ps.join_order * 100 + at.action_order,
    ps.session_id,
    ps.player_id,
    case
      when at.action_type in ('tie_breaker.assigned', 'mission.assigned', 'gold.points.awarded', 'pension.rank.awarded', 'donation.rank.awarded') then 'SYSTEM'
      else 'PLAYER'
    end,
    ps.base_time,
    ps.ruleset_version_id,
    ps.join_order,
    at.action_order,
    at.action_type,
    ps.status,
    ps.mode,
    case at.action_type
      when 'tie_breaker.assigned' then jsonb_build_object('number', 40 - ps.join_order, 'seed_case', 'manual-tie-breaker')
      when 'mission.assigned' then jsonb_build_object('mission_id', 'mission-' || ps.session_no || '-' || ps.join_order, 'target_tertiary_card_id', (array['need-tertiary-gameboy','need-tertiary-bike','need-tertiary-watch','need-tertiary-trip'])[ps.join_order], 'penalty_points', 3 + ps.join_order, 'require_primary', ps.join_order <> 4, 'require_secondary', ps.join_order <> 3)
      when 'ingredient.purchased' then jsonb_build_object('card_id', 'ing-' || ps.session_no || '-' || ps.join_order || '-' || at.action_order, 'ingredient_name', (array['nasi','sayur','telur','daging','tahu-tempe'])[((ps.join_order + at.action_order) % 5) + 1], 'amount', ((ps.join_order + at.action_order) % 5) + 1)
      when 'order.claimed' then jsonb_build_object('order_id', 'order-' || ps.session_no || '-' || ps.join_order, 'required_ingredient_card_ids', jsonb_build_array('ing-' || ps.session_no || '-' || ps.join_order || '-12', 'ing-' || ps.session_no || '-' || ps.join_order || '-13'), 'income', 10 + ps.session_no + ps.join_order * 2)
      when 'order.passed' then jsonb_build_object('order_id', 'order-pass-' || ps.session_no || '-' || ps.join_order, 'reason', case when ps.join_order % 2 = 0 then 'ingredient-shortage' else 'cash-priority' end)
      when 'need.primary.purchased' then jsonb_build_object('card_id', 'need-primary-' || ps.join_order, 'amount', 2 + (ps.join_order % 2), 'points', 1 + (ps.join_order % 2))
      when 'need.secondary.purchased' then jsonb_build_object('card_id', 'need-secondary-' || ps.join_order, 'amount', 4 + (ps.join_order % 2), 'points', 3 + (ps.join_order % 2))
      when 'need.tertiary.purchased' then jsonb_build_object('card_id', (array['need-tertiary-gameboy','need-tertiary-bike','need-tertiary-watch','need-tertiary-trip'])[ps.join_order], 'amount', 6 + (ps.join_order % 2), 'points', 5 + (ps.join_order % 2))
      when 'work.freelance.completed' then jsonb_build_object('amount', 1 + (ps.join_order % 3), 'job_type', (array['jualan-kue','kurir','desain-poster','les-privat'])[ps.join_order])
      when 'day.friday.donation' then jsonb_build_object('amount', 1 + ((ps.session_no + ps.join_order) % 6), 'donation_box', 'friday-' || ps.session_no)
      when 'day.saturday.gold_trade' then jsonb_build_object('trade_type', case when ps.join_order % 3 = 0 then 'SELL' else 'BUY' end, 'qty', 1 + (ps.join_order % 2), 'unit_price', 5 + ((ps.session_no + ps.join_order) % 4), 'amount', (1 + (ps.join_order % 2)) * (5 + ((ps.session_no + ps.join_order) % 4)))
      when 'turn.action.used' then jsonb_build_object('used', case when ps.join_order = 4 then 1 else 2 end, 'remaining', case when ps.join_order = 4 then 1 else 0 end, 'action_slot', ps.join_order)
      when 'ingredient.discarded' then jsonb_build_object('card_id', 'ing-' || ps.session_no || '-' || ps.join_order || '-13', 'amount', 1, 'reason', 'inventory-limit')
      when 'saving.deposit.created' then jsonb_build_object('goal_id', 'goal-' || ps.session_no || '-' || ps.join_order, 'amount', 4 + ps.join_order)
      when 'saving.deposit.withdrawn' then jsonb_build_object('goal_id', 'goal-' || ps.session_no || '-' || ps.join_order, 'amount', case when ps.join_order = 4 then 2 else 1 end)
      when 'loan.syariah.taken' then jsonb_build_object('loan_id', 'loan-' || ps.session_no || '-' || ps.join_order, 'principal', 8 + ps.join_order * 2, 'installment', 2 + (ps.join_order % 2), 'duration_turn', 4 + ps.join_order, 'penalty_points', 15)
      when 'insurance.multirisk.purchased' then jsonb_build_object('premium', 1 + (ps.join_order % 2), 'policy_id', 'policy-' || ps.session_no || '-' || ps.join_order)
      when 'risk.life.drawn' then jsonb_build_object('risk_id', 'risk-' || ps.session_no || '-' || ps.join_order, 'direction', case when ps.join_order = 2 then 'IN' else 'OUT' end, 'amount', 3 + ps.join_order, 'risk_type', (array['health','vehicle','education','business'])[ps.join_order])
      when 'insurance.multirisk.used' then jsonb_build_object('risk_event_id', (('e9' || lpad((ps.session_no * 10000 + ps.join_order * 100 + 28)::text, 6, '0') || '-0000-0000-0000-' || lpad((ps.session_no * 10000 + ps.join_order * 100 + 28)::text, 12, '0'))::uuid)::text, 'amount', 2 + ps.join_order)
      when 'risk.emergency.used' then jsonb_build_object('risk_event_id', (('e9' || lpad((ps.session_no * 10000 + ps.join_order * 100 + 28)::text, 6, '0') || '-0000-0000-0000-' || lpad((ps.session_no * 10000 + ps.join_order * 100 + 28)::text, 12, '0'))::uuid)::text, 'option_type', case when ps.join_order % 2 = 0 then 'SELL_NEEDS' else 'GOLD_SELL' end, 'direction', 'IN', 'amount', 2 + ps.join_order)
      when 'loan.syariah.repaid' then jsonb_build_object('loan_id', 'loan-' || ps.session_no || '-' || ps.join_order, 'amount', case when ps.join_order in (2, 4) then 8 + ps.join_order * 2 else 3 + ps.join_order end)
      when 'saving.goal.achieved' then jsonb_build_object('goal_id', 'goal-' || ps.session_no || '-' || ps.join_order, 'points', 12 + ps.join_order, 'cost', case when ps.join_order in (1, 2) then 4 + ps.join_order else 0 end)
      when 'gold.points.awarded' then jsonb_build_object('points', 2 + ps.join_order, 'basis', 'seed-gold-rank')
      when 'pension.rank.awarded' then jsonb_build_object('rank', ps.join_order, 'points', greatest(1, 8 - ps.join_order))
      when 'donation.rank.awarded' then jsonb_build_object('rank', ps.join_order, 'points', greatest(1, 7 - ps.join_order))
      else jsonb_build_object('seed_case', 'generic')
    end
  from player_scope ps
  join action_template at on not at.advanced_only or ps.mode = 'MAHIR'
  union all
  select
    ss.session_no * 10000 + 99,
    ss.session_id,
    null::uuid,
    'SYSTEM',
    ss.base_time,
    ss.ruleset_version_id,
    0,
    99,
    'session.ended',
    ss.status,
    ss.mode,
    jsonb_build_object('seed_case', 'session-ended', 'ended_at', ss.ended_at)
  from seed_sessions ss
  where ss.status = 'ENDED'
),
numbered_events as (
  select
    event_source.*,
    row_number() over (partition by session_id order by source_code) - 1 as sequence_number
  from event_source
)
insert into events (
  event_pk,
  event_id,
  session_id,
  player_id,
  actor_type,
  timestamp,
  day_index,
  weekday,
  turn_number,
  sequence_number,
  action_type,
  ruleset_version_id,
  payload,
  received_at,
  client_request_id
)
select
  ('e8' || lpad(source_code::text, 6, '0') || '-0000-0000-0000-' || lpad(source_code::text, 12, '0'))::uuid,
  ('e9' || lpad(source_code::text, 6, '0') || '-0000-0000-0000-' || lpad(source_code::text, 12, '0'))::uuid,
  session_id,
  player_id,
  actor_type,
  base_time + (sequence_number || ' minutes')::interval,
  floor(sequence_number / 8.0)::int,
  (array['MON','TUE','WED','THU','FRI','SAT','SUN'])[((sequence_number % 7) + 1)::int],
  greatest(1, floor(sequence_number / 2.0)::int + 1),
  sequence_number,
  action_type,
  ruleset_version_id,
  payload,
  base_time + (sequence_number || ' minutes')::interval + interval '3 seconds',
  'seed-complex-v4-' || lpad(source_code::text, 6, '0')
from numbered_events
on conflict (session_id, event_id) do update
set player_id = excluded.player_id,
    actor_type = excluded.actor_type,
    timestamp = excluded.timestamp,
    day_index = excluded.day_index,
    weekday = excluded.weekday,
    turn_number = excluded.turn_number,
    sequence_number = excluded.sequence_number,
    action_type = excluded.action_type,
    ruleset_version_id = excluded.ruleset_version_id,
    payload = excluded.payload,
    received_at = excluded.received_at,
    client_request_id = excluded.client_request_id;

-- =========================================================
-- 5) Proyeksi arus kas untuk event yang berdampak finansial
-- =========================================================
with projection_source as (
  select
    row_number() over (order by e.session_id, e.sequence_number) as n,
    e.event_pk,
    e.event_id,
    e.session_id,
    e.player_id,
    e.timestamp,
    e.action_type,
    e.payload
  from events e
  where e.session_id::text like '81000000-%'
    and e.player_id is not null
    and e.action_type in (
      'ingredient.purchased',
      'order.claimed',
      'need.primary.purchased',
      'need.secondary.purchased',
      'need.tertiary.purchased',
      'work.freelance.completed',
      'day.friday.donation',
      'day.saturday.gold_trade',
      'saving.deposit.created',
      'saving.deposit.withdrawn',
      'saving.goal.achieved',
      'loan.syariah.taken',
      'loan.syariah.repaid',
      'insurance.multirisk.purchased',
      'insurance.multirisk.used',
      'risk.life.drawn',
      'risk.emergency.used'
    )
),
projection_rows as (
  select
    n,
    event_pk,
    event_id,
    session_id,
    player_id,
    timestamp,
    case
      when action_type in ('order.claimed', 'work.freelance.completed', 'saving.deposit.withdrawn', 'loan.syariah.taken', 'insurance.multirisk.used', 'risk.emergency.used') then 'IN'
      when action_type = 'day.saturday.gold_trade' and payload->>'trade_type' = 'SELL' then 'IN'
      when action_type = 'risk.life.drawn' then payload->>'direction'
      else 'OUT'
    end as direction,
    case
      when action_type = 'order.claimed' then (payload->>'income')::int
      when action_type = 'saving.goal.achieved' then greatest(1, (payload->>'cost')::int)
      when action_type = 'loan.syariah.taken' then (payload->>'principal')::int
      when action_type = 'insurance.multirisk.purchased' then (payload->>'premium')::int
      else (payload->>'amount')::int
    end as amount,
    case action_type
      when 'ingredient.purchased' then 'INGREDIENT'
      when 'order.claimed' then 'ORDER_INCOME'
      when 'need.primary.purchased' then 'NEED_PRIMARY'
      when 'need.secondary.purchased' then 'NEED_SECONDARY'
      when 'need.tertiary.purchased' then 'NEED_TERTIARY'
      when 'work.freelance.completed' then 'FREELANCE'
      when 'day.friday.donation' then 'DONATION'
      when 'day.saturday.gold_trade' then 'GOLD_TRADE'
      when 'saving.deposit.created' then 'SAVING_DEPOSIT'
      when 'saving.deposit.withdrawn' then 'SAVING_WITHDRAW'
      when 'saving.goal.achieved' then 'SAVING_GOAL'
      when 'loan.syariah.taken' then 'LOAN_TAKEN'
      when 'loan.syariah.repaid' then 'LOAN_REPAID'
      when 'insurance.multirisk.purchased' then 'INSURANCE_PREMIUM'
      when 'insurance.multirisk.used' then 'INSURANCE_CLAIM'
      when 'risk.life.drawn' then 'RISK_LIFE'
      when 'risk.emergency.used' then 'RISK_LIFE'
      else 'MISC'
    end as category,
    action_type
  from projection_source
)
insert into event_cashflow_projections (
  projection_id,
  session_id,
  player_id,
  event_pk,
  event_id,
  timestamp,
  direction,
  amount,
  category,
  counterparty,
  reference,
  note
)
select
  ('f8' || lpad(n::text, 6, '0') || '-0000-0000-0000-' || lpad(n::text, 12, '0'))::uuid,
  session_id,
  player_id,
  event_pk,
  event_id,
  timestamp,
  direction,
  amount,
  category,
  case
    when category in ('ORDER_INCOME', 'FREELANCE') then 'MARKET'
    when category in ('LOAN_TAKEN', 'LOAN_REPAID') then 'BANK'
    when category in ('INSURANCE_PREMIUM', 'INSURANCE_CLAIM') then 'INSURANCE'
    else null
  end,
  'seed-complex-v4-' || lpad(n::text, 4, '0'),
  'cashflow projection generated from complex inspection seed'
from projection_rows
where amount > 0
on conflict (session_id, event_id) do update
set player_id = excluded.player_id,
    event_pk = excluded.event_pk,
    timestamp = excluded.timestamp,
    direction = excluded.direction,
    amount = excluded.amount,
    category = excluded.category,
    counterparty = excluded.counterparty,
    reference = excluded.reference,
    note = excluded.note;

-- =========================================================
-- 6) Metric snapshot eksplisit dan matrix numerik
-- =========================================================
with gameplay_snapshot_players as (
  select
    sp.session_id,
    sp.player_id,
    sp.join_order,
    s.mode,
    s.status,
    sra.ruleset_version_id,
    rv.config_json,
    row_number() over (order by sp.session_id, sp.join_order, sp.player_id) as player_row
  from session_players sp
  join sessions s on s.session_id = sp.session_id
  join session_ruleset_activations sra on sra.session_id = sp.session_id
  join ruleset_versions rv on rv.ruleset_version_id = sra.ruleset_version_id
  where sp.session_id::text like '81000000-%'
    and exists (
      select 1
      from events e
      where e.session_id = sp.session_id
        and e.player_id = sp.player_id
    )
),
event_rollup as (
  select
    e.session_id,
    e.player_id,
    count(*) as event_count,
    count(distinct e.action_type) as action_type_count,
    coalesce(max(e.turn_number), 0) as max_turn,
    max(e.timestamp) as latest_event_at,
    sum(case when e.action_type = 'ingredient.purchased' then coalesce((e.payload->>'amount')::int, 0) else 0 end) as ingredients_collected,
    sum(case when e.action_type = 'ingredient.discarded' then coalesce((e.payload->>'amount')::int, 0) else 0 end) as ingredients_wasted,
    count(*) filter (where e.action_type = 'order.claimed') as meal_orders_claimed,
    count(*) filter (where e.action_type = 'order.passed') as meal_orders_passed,
    sum(case when e.action_type = 'order.claimed' then coalesce((e.payload->>'income')::int, 0) else 0 end) as meal_order_income_total,
    count(*) filter (where e.action_type = 'need.primary.purchased') as primary_needs_owned,
    count(*) filter (where e.action_type = 'need.secondary.purchased') as secondary_needs_owned,
    count(*) filter (where e.action_type = 'need.tertiary.purchased') as tertiary_needs_owned,
    sum(case when e.action_type like 'need.%.purchased' then coalesce((e.payload->>'amount')::int, 0) else 0 end) as need_cards_coins_spent,
    sum(case when e.action_type = 'day.friday.donation' then coalesce((e.payload->>'amount')::int, 0) else 0 end) as donation_total_coins,
    count(*) filter (where e.action_type = 'donation.rank.awarded') as donation_champion_cards_earned,
    sum(case when e.action_type = 'day.saturday.gold_trade' and e.payload->>'trade_type' = 'BUY' then coalesce((e.payload->>'qty')::int, 0) else 0 end) as gold_cards_purchased,
    sum(case when e.action_type = 'day.saturday.gold_trade' and e.payload->>'trade_type' = 'SELL' then coalesce((e.payload->>'qty')::int, 0) else 0 end) as gold_cards_sold,
    sum(case when e.action_type = 'day.saturday.gold_trade' and e.payload->>'trade_type' = 'BUY' then coalesce((e.payload->>'amount')::int, 0) else 0 end) as gold_investment_coins_spent,
    sum(case when e.action_type = 'day.saturday.gold_trade' and e.payload->>'trade_type' = 'SELL' then coalesce((e.payload->>'amount')::int, 0) else 0 end) as gold_investment_coins_earned,
    count(*) filter (where e.action_type = 'risk.life.drawn') as life_risk_cards_drawn,
    sum(case when e.action_type = 'risk.life.drawn' then coalesce((e.payload->>'amount')::int, 0) else 0 end) as life_risk_costs_total,
    count(*) filter (where e.action_type = 'insurance.multirisk.purchased') as insurance_payments_made,
    count(*) filter (where e.action_type = 'insurance.multirisk.used') as life_risk_mitigated_with_insurance,
    count(*) filter (where e.action_type = 'risk.emergency.used') as emergency_options_used,
    count(*) filter (where e.action_type = 'saving.deposit.created') as financial_goals_attempted,
    count(*) filter (where e.action_type = 'saving.goal.achieved') as financial_goals_completed,
    sum(case when e.action_type = 'saving.deposit.created' then coalesce((e.payload->>'amount')::int, 0) else 0 end) as financial_goals_coins_total_invested,
    sum(case when e.action_type = 'saving.deposit.withdrawn' then coalesce((e.payload->>'amount')::int, 0) else 0 end) as coins_withdrawn_from_goals,
    count(*) filter (where e.action_type = 'loan.syariah.taken') as sharia_loans_taken,
    count(*) filter (where e.action_type = 'loan.syariah.repaid') as sharia_loans_repaid,
    sum(case when e.action_type = 'loan.syariah.taken' then coalesce((e.payload->>'principal')::int, 0) else 0 end) as sharia_loans_principal,
    sum(case when e.action_type = 'loan.syariah.repaid' then coalesce((e.payload->>'amount')::int, 0) else 0 end) as sharia_loans_repaid_amount,
    sum(case when e.action_type = 'turn.action.used' then coalesce((e.payload->>'used')::int, 0) else 0 end) as actions_used_total,
    sum(case when e.action_type = 'turn.action.used' then coalesce((e.payload->>'remaining')::int, 0) else 0 end) as actions_skipped,
    count(*) filter (where e.action_type = 'mission.assigned') as missions_assigned,
    count(*) filter (where e.action_type = 'session.ended') as session_ended_events,
    max(case when e.action_type = 'pension.rank.awarded' then coalesce((e.payload->>'rank')::int, 0) else null end) as pension_fund_rank_per_game,
    sum(case when e.action_type = 'pension.rank.awarded' then coalesce((e.payload->>'points')::int, 0) else 0 end) as pension_fund_happiness_points,
    sum(case when e.action_type = 'gold.points.awarded' then coalesce((e.payload->>'points')::int, 0) else 0 end) as gold_happiness_points,
    sum(case when e.action_type = 'donation.rank.awarded' then coalesce((e.payload->>'points')::int, 0) else 0 end) as donation_happiness_points,
    sum(case when e.action_type like 'need.%.purchased' then coalesce((e.payload->>'points')::int, 0) else 0 end) as need_cards_points
  from events e
  where e.session_id::text like '81000000-%'
    and e.player_id is not null
  group by e.session_id, e.player_id
),
projection_rollup as (
  select
    p.session_id,
    p.player_id,
    sum(case when p.direction = 'IN' then p.amount else 0 end) as cash_in_total,
    sum(case when p.direction = 'OUT' then p.amount else 0 end) as cash_out_total,
    sum(case when p.category = 'ORDER_INCOME' and p.direction = 'IN' then p.amount else 0 end) as meal_income,
    sum(case when p.category = 'FREELANCE' and p.direction = 'IN' then p.amount else 0 end) as freelance_income,
    sum(case when p.category = 'GOLD_TRADE' and p.direction = 'IN' then p.amount else 0 end) as gold_income,
    sum(case when p.category = 'INGREDIENT' and p.direction = 'OUT' then p.amount else 0 end) as ingredient_expenses,
    sum(case when p.category like 'NEED_%' and p.direction = 'OUT' then p.amount else 0 end) as need_expenses,
    sum(case when p.category = 'DONATION' and p.direction = 'OUT' then p.amount else 0 end) as donation_expenses,
    sum(case when p.category = 'SAVING_DEPOSIT' and p.direction = 'OUT' then p.amount else 0 end) as saving_deposits,
    sum(case when p.category = 'SAVING_WITHDRAW' and p.direction = 'IN' then p.amount else 0 end) as saving_withdrawals
  from event_cashflow_projections p
  where p.session_id::text like '81000000-%'
  group by p.session_id, p.player_id
),
gameplay_snapshot_rows as (
  select
    g.session_id,
    g.player_id,
    g.join_order,
    g.mode,
    g.status,
    g.ruleset_version_id,
    g.player_row,
    coalesce((g.config_json->>'starting_cash')::int, case when g.mode = 'MAHIR' then 12 else 24 end) as starting_cash,
    coalesce((g.config_json->>'actions_per_turn')::int, 2) as actions_per_turn,
    coalesce(er.event_count, 0) as event_count,
    coalesce(er.action_type_count, 0) as action_type_count,
    coalesce(er.max_turn, 0) as max_turn,
    er.latest_event_at,
    coalesce(er.ingredients_collected, 0) as ingredients_collected,
    greatest(coalesce(er.ingredients_collected, 0) - coalesce(er.ingredients_wasted, 0) - coalesce(er.meal_orders_claimed, 0) * 2, 0) as ingredients_held_current,
    coalesce(er.ingredients_wasted, 0) as ingredients_wasted,
    coalesce(er.meal_orders_claimed, 0) as meal_orders_claimed,
    coalesce(er.meal_orders_passed, 0) as meal_orders_passed,
    coalesce(er.meal_order_income_total, 0) as meal_order_income_total,
    coalesce(er.primary_needs_owned, 0) as primary_needs_owned,
    coalesce(er.secondary_needs_owned, 0) as secondary_needs_owned,
    coalesce(er.tertiary_needs_owned, 0) as tertiary_needs_owned,
    coalesce(er.need_cards_coins_spent, 0) as need_cards_coins_spent,
    coalesce(er.donation_total_coins, 0) as donation_total_coins,
    coalesce(er.donation_champion_cards_earned, 0) as donation_champion_cards_earned,
    coalesce(er.gold_cards_purchased, 0) as gold_cards_purchased,
    coalesce(er.gold_cards_sold, 0) as gold_cards_sold,
    coalesce(er.gold_investment_coins_spent, 0) as gold_investment_coins_spent,
    coalesce(er.gold_investment_coins_earned, 0) as gold_investment_coins_earned,
    coalesce(er.life_risk_cards_drawn, 0) as life_risk_cards_drawn,
    coalesce(er.life_risk_costs_total, 0) as life_risk_costs_total,
    coalesce(er.insurance_payments_made, 0) as insurance_payments_made,
    coalesce(er.life_risk_mitigated_with_insurance, 0) as life_risk_mitigated_with_insurance,
    coalesce(er.emergency_options_used, 0) as emergency_options_used,
    coalesce(er.financial_goals_attempted, 0) as financial_goals_attempted,
    coalesce(er.financial_goals_completed, 0) as financial_goals_completed,
    coalesce(er.financial_goals_coins_total_invested, 0) as financial_goals_coins_total_invested,
    greatest(coalesce(er.financial_goals_coins_total_invested, 0) - coalesce(er.coins_withdrawn_from_goals, 0), 0) as coins_in_savings_goal,
    coalesce(er.sharia_loans_taken, 0) as sharia_loans_taken,
    coalesce(er.sharia_loans_repaid, 0) as sharia_loans_repaid,
    greatest(coalesce(er.sharia_loans_principal, 0) - coalesce(er.sharia_loans_repaid_amount, 0), 0) as sharia_loans_outstanding_coins,
    coalesce(er.actions_used_total, 0) as actions_used_total,
    coalesce(er.actions_skipped, 0) as actions_skipped,
    coalesce(er.missions_assigned, 0) as missions_assigned,
    coalesce(er.session_ended_events, 0) as session_ended_events,
    er.pension_fund_rank_per_game,
    coalesce(er.pension_fund_happiness_points, 0) as pension_fund_happiness_points,
    coalesce(er.gold_happiness_points, 0) as gold_happiness_points,
    coalesce(er.donation_happiness_points, 0) as donation_happiness_points,
    coalesce(er.need_cards_points, 0) as need_cards_points,
    coalesce(pr.cash_in_total, 0) as cash_in_total,
    coalesce(pr.cash_out_total, 0) as cash_out_total,
    coalesce(pr.meal_income, 0) as meal_income,
    coalesce(pr.freelance_income, 0) as freelance_income,
    coalesce(pr.gold_income, 0) as gold_income,
    coalesce(pr.ingredient_expenses, 0) as ingredient_expenses,
    coalesce(pr.need_expenses, 0) as need_expenses,
    coalesce(pr.donation_expenses, 0) as donation_expenses,
    coalesce(pr.saving_deposits, 0) as saving_deposits,
    coalesce(pr.saving_withdrawals, 0) as saving_withdrawals
  from gameplay_snapshot_players g
  join event_rollup er on er.session_id = g.session_id and er.player_id = g.player_id
  left join projection_rollup pr on pr.session_id = g.session_id and pr.player_id = g.player_id
),
metric_types(metric_order, metric_name) as (
  values
    (1, 'gameplay.raw.variables'),
    (2, 'gameplay.derived.metrics')
)
insert into metric_snapshots (
  metric_snapshot_id,
  session_id,
  player_id,
  computed_at,
  metric_name,
  metric_value_numeric,
  metric_value_json,
  ruleset_version_id
)
select
  case
    when mt.metric_name = 'gameplay.raw.variables'
      then ('f710' || lpad(g.player_row::text, 4, '0') || '-0000-0000-0000-' || lpad(g.player_row::text, 12, '0'))::uuid
    else ('f720' || lpad(g.player_row::text, 4, '0') || '-0000-0000-0000-' || lpad(g.player_row::text, 12, '0'))::uuid
  end,
  g.session_id,
  g.player_id,
  '2026-05-19T09:00:00+07:00'::timestamptz
    + (g.player_row || ' minutes')::interval
    + case when mt.metric_order = 2 then interval '1 second' else interval '0 seconds' end,
  mt.metric_name,
  null::double precision,
  case
    when mt.metric_name = 'gameplay.raw.variables' then jsonb_build_object(
      'metadata', jsonb_build_object(
        'game_id', g.session_id,
        'session_id', g.session_id,
        'player_id', g.player_id,
        'player_alias', null,
        'game_mode', case when g.mode = 'MAHIR' then 'advanced' else 'beginner' end,
        'turn_number', nullif(g.max_turn, 0),
        'day_label', case ((g.max_turn + g.join_order) % 7) when 0 then 'MON' when 1 then 'TUE' when 2 then 'WED' when 3 then 'THU' when 4 then 'FRI' when 5 then 'SAT' else 'SUN' end,
        'action_slot', g.join_order,
        'action_slot_timeline', jsonb_build_array(1, 2, g.join_order),
        'event_timestamp', g.latest_event_at
      ),
      'coins', jsonb_build_object(
        'starting_coins', g.starting_cash,
        'coins_held_current', g.starting_cash + g.cash_in_total - g.cash_out_total,
        'coins_spent_per_turn', jsonb_build_array(g.cash_out_total / greatest(g.max_turn, 1), greatest(g.need_expenses + g.ingredient_expenses, 1), greatest(g.donation_expenses, 0)),
        'coins_earned_per_turn', jsonb_build_array(g.cash_in_total / greatest(g.max_turn, 1), greatest(g.meal_income, 0), greatest(g.freelance_income, 0)),
        'coins_donated', g.donation_total_coins,
        'coins_saved', g.coins_in_savings_goal,
        'coins_net_end_game', g.starting_cash + g.cash_in_total - g.cash_out_total
      ),
      'ingredients', jsonb_build_object(
        'ingredients_collected', g.ingredients_collected,
        'ingredients_held_current', g.ingredients_held_current,
        'ingredient_types_held', jsonb_build_object('nasi', greatest(g.join_order, 1), 'sayur', greatest(g.ingredients_held_current - g.join_order, 0), 'telur', g.join_order % 2),
        'ingredients_used_per_meal', case when g.meal_orders_claimed = 0 then 0 else greatest((g.ingredients_collected - g.ingredients_held_current - g.ingredients_wasted) / greatest(g.meal_orders_claimed, 1), 0) end,
        'ingredients_wasted', g.ingredients_wasted,
        'ingredient_investment_coins_total', g.ingredient_expenses
      ),
      'meal_orders', jsonb_build_object(
        'meal_orders_claimed', g.meal_orders_claimed,
        'meal_orders_available_passed', g.meal_orders_passed,
        'meal_order_income_per_order', jsonb_build_array(g.meal_income / greatest(g.meal_orders_claimed, 1)),
        'meal_order_income_total', g.meal_income,
        'meal_orders_per_turn_average', (g.meal_orders_claimed::numeric / greatest(g.max_turn, 1))
      ),
      'needs', jsonb_build_object(
        'need_cards_purchased', g.primary_needs_owned + g.secondary_needs_owned + g.tertiary_needs_owned,
        'primary_needs_owned', g.primary_needs_owned,
        'secondary_needs_owned', g.secondary_needs_owned,
        'tertiary_needs_owned', g.tertiary_needs_owned,
        'need_profile', jsonb_build_object('basic_profile', g.primary_needs_owned > 0, 'collector_profile', g.tertiary_needs_owned > 0, 'specialist_profile', g.join_order in (1, 3)),
        'specific_tertiary_need', g.tertiary_needs_owned > 0,
        'collection_mission_complete', g.tertiary_needs_owned > 0 and g.missions_assigned > 0,
        'need_cards_coins_spent', g.need_cards_coins_spent
      ),
      'donations', jsonb_build_object(
        'donation_amount_per_friday', jsonb_build_array(g.donation_total_coins),
        'donation_rank_per_friday', jsonb_build_array(g.join_order),
        'donation_total_coins', g.donation_total_coins,
        'donation_champion_cards_earned', g.donation_champion_cards_earned,
        'donation_happiness_points', g.donation_happiness_points
      ),
      'gold', jsonb_build_object(
        'gold_cards_purchased', g.gold_cards_purchased,
        'gold_cards_sold', g.gold_cards_sold,
        'gold_cards_held_end', g.gold_cards_purchased - g.gold_cards_sold,
        'gold_prices_per_purchase', jsonb_build_array(case when g.gold_cards_purchased = 0 then 0 else g.gold_investment_coins_spent / greatest(g.gold_cards_purchased, 1) end),
        'gold_price_per_sale', jsonb_build_array(case when g.gold_cards_sold = 0 then 0 else g.gold_investment_coins_earned / greatest(g.gold_cards_sold, 1) end),
        'gold_investment_coins_spent', g.gold_investment_coins_spent,
        'gold_investment_coins_earned', g.gold_investment_coins_earned,
        'gold_investment_net', g.gold_investment_coins_earned - g.gold_investment_coins_spent
      ),
      'pension', jsonb_build_object(
        'leftover_coins_end_game', g.starting_cash + g.cash_in_total - g.cash_out_total,
        'ingredient_cards_value_end', g.ingredients_held_current,
        'coins_in_savings_goal', g.coins_in_savings_goal,
        'pension_fund_total', g.starting_cash + g.cash_in_total - g.cash_out_total + g.ingredients_held_current + g.coins_in_savings_goal,
        'pension_fund_rank_per_game', g.pension_fund_rank_per_game,
        'pension_fund_happiness_points', g.pension_fund_happiness_points
      ),
      'life_risk', jsonb_build_object(
        'life_risks_available', g.life_risk_cards_drawn,
        'life_risk_cards_drawn', g.life_risk_cards_drawn,
        'life_risks_accepted', g.life_risk_cards_drawn,
        'life_risk_costs_per_card', jsonb_build_array(case when g.life_risk_cards_drawn = 0 then 0 else g.life_risk_costs_total / greatest(g.life_risk_cards_drawn, 1) end),
        'life_risk_costs_total', g.life_risk_costs_total,
        'life_risk_mitigated_with_insurance', g.life_risk_mitigated_with_insurance,
        'insurance_payments_made', g.insurance_payments_made,
        'emergency_options_used', g.emergency_options_used
      ),
      'financial_goals', jsonb_build_object(
        'financial_goals_available_total', greatest(g.financial_goals_attempted, 1),
        'financial_goals_attempted', g.financial_goals_attempted,
        'financial_goals_completed', g.financial_goals_completed,
        'financial_goals_coins_per_goal', jsonb_build_object('goal-' || g.join_order, g.coins_in_savings_goal),
        'financial_goals_coins_total_invested', g.financial_goals_coins_total_invested,
        'financial_goals_incomplete_coins_wasted', greatest(g.financial_goals_coins_total_invested - g.coins_in_savings_goal, 0),
        'sharia_loans_taken', g.sharia_loans_taken,
        'sharia_loan_cards_taken', g.sharia_loans_taken,
        'sharia_loans_repaid', g.sharia_loans_repaid,
        'sharia_loans_unpaid_end', case when g.sharia_loans_outstanding_coins > 0 then 1 else 0 end,
        'sharia_loans_outstanding_coins', g.sharia_loans_outstanding_coins,
        'loan_penalty_if_unpaid', case when g.sharia_loans_outstanding_coins > 0 then 15 else 0 end
      ),
      'actions', jsonb_build_object(
        'actions_per_turn', g.actions_per_turn,
        'action_repetitions_per_turn', jsonb_build_object('turn-' || greatest(g.max_turn, 1), greatest(g.actions_used_total - g.action_type_count, 0)),
        'action_sequence', jsonb_build_array(jsonb_build_object('turn', greatest(g.max_turn, 1), 'actions', jsonb_build_array('work.freelance.completed', 'need.primary.purchased'))),
        'actions_skipped', g.actions_skipped
      ),
      'turns', jsonb_build_object(
        'coins_per_turn_progression', jsonb_build_array(g.starting_cash, g.starting_cash + g.cash_in_total - g.cash_out_total / 2, g.starting_cash + g.cash_in_total - g.cash_out_total),
        'net_income_per_turn', jsonb_build_array(g.cash_in_total - g.cash_out_total, g.cash_in_total / greatest(g.max_turn, 1)),
        'turn_number_when_debt_introduced', case when g.sharia_loans_taken > 0 then greatest(g.max_turn - 2, 1) else null end,
        'turn_number_when_first_risk_hit', case when g.life_risk_cards_drawn > 0 then greatest(g.max_turn - 1, 1) else null end,
        'turn_number_game_completion', nullif(g.max_turn, 0)
      ),
      'outcomes', jsonb_build_object(
        'total_happiness_points', g.need_cards_points + g.donation_happiness_points + g.gold_happiness_points + g.pension_fund_happiness_points - case when g.sharia_loans_outstanding_coins > 0 then 15 else 0 end,
        'final_rank', case when g.status = 'ENDED' then g.join_order else null end,
        'winner_flag', g.status = 'ENDED' and g.join_order = 1,
        'finish_line_reached', g.session_ended_events > 0,
        'dnf_flag', g.status = 'ENDED' and g.session_ended_events = 0
      ),
      'notes', jsonb_build_array('inspection_seed_generated', 'player_detail_diagram_ready')
    )
    else jsonb_build_object(
      'net_worth_index', ((g.starting_cash + g.cash_in_total - g.cash_out_total)::numeric / greatest(g.starting_cash, 1)),
      'income_diversification_index', least(1.0, (case when g.freelance_income > 0 then 0.34 else 0 end) + (case when g.meal_income > 0 then 0.33 else 0 end) + (case when g.gold_income > 0 then 0.33 else 0 end)),
      'income_diversification_ratio', least(1.0, (case when g.freelance_income > 0 then 0.34 else 0 end) + (case when g.meal_income > 0 then 0.33 else 0 end) + (case when g.gold_income > 0 then 0.33 else 0 end)),
      'income_diversification_components', jsonb_build_object(
        'freelance_income', g.freelance_income,
        'meal_income', g.meal_income,
        'gold_income', g.gold_income,
        'donations_received', 0,
        'total_income', g.cash_in_total,
        'N_active_income_sources', (case when g.freelance_income > 0 then 1 else 0 end) + (case when g.meal_income > 0 then 1 else 0 end) + (case when g.gold_income > 0 then 1 else 0 end),
        'Income_Share_i', jsonb_build_object('freelance', case when g.cash_in_total = 0 then 0 else g.freelance_income::numeric / g.cash_in_total end, 'meal', case when g.cash_in_total = 0 then 0 else g.meal_income::numeric / g.cash_in_total end, 'gold', case when g.cash_in_total = 0 then 0 else g.gold_income::numeric / g.cash_in_total end)
      ),
      'expense_management_efficiency', case when g.cash_out_total = 0 then 1 else least(1.0, (g.ingredient_expenses + g.need_expenses)::numeric / g.cash_out_total) end,
      'expense_management_components', jsonb_build_object('essential_expenses', g.ingredient_expenses + g.need_expenses, 'total_expenses', g.cash_out_total),
      'business_profit_margin', case when g.meal_income = 0 then 0 else (g.meal_income - g.ingredient_expenses)::numeric / g.meal_income end,
      'business_efficiency_ratio', case when g.ingredient_expenses = 0 then 0 else g.meal_income::numeric / g.ingredient_expenses end,
      'gold_roi_percentage', case when g.gold_investment_coins_spent = 0 then 0 else ((g.gold_investment_coins_earned - g.gold_investment_coins_spent)::numeric / g.gold_investment_coins_spent) * 100 end,
      'risk_exposure_percentage', case when g.starting_cash = 0 then 0 else (g.life_risk_costs_total::numeric / greatest(g.starting_cash, 1)) * 100 end,
      'risk_mitigation_effectiveness', case when g.life_risk_cards_drawn = 0 then 0 else g.life_risk_mitigated_with_insurance::numeric / g.life_risk_cards_drawn end,
      'risk_appetite_score', g.life_risk_cards_drawn * 10 + g.emergency_options_used * 5,
      'risk_appetite_components', jsonb_build_object(
        'life_risks_accepted', g.life_risk_cards_drawn,
        'life_risks_available', g.life_risk_cards_drawn,
        'risk_acceptance_rate', case when g.life_risk_cards_drawn = 0 then 0 else 1 end,
        'average_risk_cost', case when g.life_risk_cards_drawn = 0 then 0 else g.life_risk_costs_total::numeric / g.life_risk_cards_drawn end,
        'insurance_activation_rate', case when g.life_risk_cards_drawn = 0 then 0 else g.life_risk_mitigated_with_insurance::numeric / g.life_risk_cards_drawn end,
        'Insurance_Coverage_Rate', case when g.life_risk_cards_drawn = 0 then 0 else g.insurance_payments_made::numeric / g.life_risk_cards_drawn end,
        'Risk_Cost_Intensity', case when g.starting_cash = 0 then 0 else g.life_risk_costs_total::numeric / g.starting_cash end
      ),
      'debt_leverage_ratio', case when g.cash_in_total = 0 then 0 else (g.sharia_loans_outstanding_coins::numeric / g.cash_in_total) * 100 end,
      'loan_repayment_discipline', case when g.sharia_loans_taken = 0 then 1 else least(1.0, g.sharia_loans_repaid::numeric / g.sharia_loans_taken) end,
      'debt_ratio', case when g.starting_cash + g.cash_in_total = 0 then 0 else g.sharia_loans_outstanding_coins::numeric / (g.starting_cash + g.cash_in_total) end,
      'goal_ambition', case when g.financial_goals_attempted = 0 then 0 else least(1.0, g.financial_goals_coins_total_invested::numeric / greatest(g.starting_cash, 1)) end,
      'goal_setting_ambition', case when g.financial_goals_attempted = 0 then 0 else least(1.0, g.financial_goals_coins_total_invested::numeric / greatest(g.starting_cash, 1)) end,
      'goal_setting_components', jsonb_build_object('Goal_Attempt_Rate', case when g.financial_goals_attempted = 0 then 0 else 1 end, 'Goal_Investment_Rate', g.financial_goals_coins_total_invested::numeric / greatest(g.starting_cash, 1)),
      'action_efficiency', case when g.max_turn = 0 then 0 else g.actions_used_total::numeric / (g.max_turn * g.actions_per_turn) end,
      'action_efficiency_percent', case when g.max_turn = 0 then 0 else (g.actions_used_total::numeric / (g.max_turn * g.actions_per_turn)) * 100 end,
      'action_diversity_score_avg', case when g.event_count = 0 then 0 else g.action_type_count::numeric / g.event_count end,
      'action_diversity_score', case when g.event_count = 0 then 0 else g.action_type_count::numeric / g.event_count end,
      'meal_order_success_rate', case when g.meal_orders_claimed + g.meal_orders_passed = 0 then 0 else g.meal_orders_claimed::numeric / (g.meal_orders_claimed + g.meal_orders_passed) end,
      'planning_horizon', case when g.financial_goals_attempted > 0 then 1 else 0 end,
      'planning_horizon_percent', case when g.financial_goals_attempted > 0 then 100 else 0 end,
      'fulfillment_diversity', (case when g.primary_needs_owned > 0 then 0.34 else 0 end) + (case when g.secondary_needs_owned > 0 then 0.33 else 0 end) + (case when g.tertiary_needs_owned > 0 then 0.33 else 0 end),
      'need_fulfillment_diversity_index', (case when g.primary_needs_owned > 0 then 0.34 else 0 end) + (case when g.secondary_needs_owned > 0 then 0.33 else 0 end) + (case when g.tertiary_needs_owned > 0 then 0.33 else 0 end),
      'fulfillment_diversity_components', jsonb_build_object('p_primary', g.primary_needs_owned, 'p_secondary', g.secondary_needs_owned, 'p_tertiary', g.tertiary_needs_owned),
      'mission_achievement', case when g.missions_assigned = 0 then 0 else case when g.tertiary_needs_owned > 0 then 1 else 0 end end,
      'growth_pattern_ratio', case when g.starting_cash = 0 then 0 else (g.starting_cash + g.cash_in_total - g.cash_out_total)::numeric / g.starting_cash end,
      'growth_pattern', case when g.cash_in_total >= g.cash_out_total then 'steady_growth' else 'volatile_recovery' end,
      'donation_aggressiveness_percent', case when g.cash_out_total = 0 then 0 else (g.donation_total_coins::numeric / g.cash_out_total) * 100 end,
      'donation_stability_std_deviation', g.join_order::numeric / 10,
      'donation_ratio', case when g.cash_out_total = 0 then 0 else g.donation_total_coins::numeric / g.cash_out_total end,
      'friday_participation_rate', case when g.donation_total_coins > 0 then 1 else 0 end,
      'donation_commitment_score', case when g.donation_total_coins > 0 then least(1.0, g.donation_total_coins::numeric / greatest(g.starting_cash, 1)) else 0 end,
      'donation_consistency_score', case when g.donation_total_coins > 0 then least(1.0, g.donation_total_coins::numeric / greatest(g.starting_cash, 1)) else 0 end,
      'donation_commitment_components', jsonb_build_object('donation_stability', 1 - (g.join_order::numeric / 10), 'donation_ratio', case when g.cash_out_total = 0 then 0 else g.donation_total_coins::numeric / g.cash_out_total end, 'friday_participation_rate', case when g.donation_total_coins > 0 then 1 else 0 end),
      'risk_appetite_score_normalized', least(1.0, (g.life_risk_cards_drawn * 10 + g.emergency_options_used * 5)::numeric / 100),
      'sharia_loans_outstanding_coins', g.sharia_loans_outstanding_coins,
      'happiness_portfolio', jsonb_build_object(
        'need_cards_pts', g.need_cards_points,
        'donations_pts', g.donation_happiness_points,
        'gold_pts', g.gold_happiness_points,
        'pension_pts', g.pension_fund_happiness_points,
        'financial_goals_pts', g.financial_goals_completed * 5,
        'mission_bonus_pts', case when g.missions_assigned > 0 and g.tertiary_needs_owned = 0 then -4 else 0 end
      ),
      'notes', jsonb_build_array('inspection_seed_generated', case when g.life_risk_cards_drawn = 0 then 'risk_appetite_requires_risk_events' else 'risk_events_available' end)
    )
  end,
  g.ruleset_version_id
from gameplay_snapshot_rows g
cross join metric_types mt
where mt.metric_name in ('gameplay.raw.variables', 'gameplay.derived.metrics')
on conflict (metric_snapshot_id) do update
set session_id = excluded.session_id,
    player_id = excluded.player_id,
    computed_at = excluded.computed_at,
    metric_name = excluded.metric_name,
    metric_value_numeric = excluded.metric_value_numeric,
    metric_value_json = excluded.metric_value_json,
    ruleset_version_id = excluded.ruleset_version_id;

insert into metric_snapshots (
  metric_snapshot_id,
  session_id,
  player_id,
  computed_at,
  metric_name,
  metric_value_numeric,
  metric_value_json,
  ruleset_version_id
)
values
  ('f7000000-0000-0000-0000-000000000005', '81000000-0000-0000-0000-000000000001', '22000000-0000-0000-0000-000000000002', '2026-05-12T09:45:02+07:00', 'compliance.primary_need.rate', 0.82, '{"evaluated_days":5,"compliant_days":4,"missed_days":[3]}'::jsonb, '72100000-0000-0000-0000-000000000002'),
  ('f7000000-0000-0000-0000-000000000006', '81000000-0000-0000-0000-000000000002', '22000000-0000-0000-0000-000000000006', '2026-05-13T10:30:02+07:00', 'cashflow.net.total', -6, '{"income_sources":["order","loan","freelance"],"expense_pressure":"risk-heavy"}'::jsonb, '72100000-0000-0000-0000-000000000003'),
  ('f7000000-0000-0000-0000-000000000007', '81000000-0000-0000-0000-000000000003', '22000000-0000-0000-0000-000000000009', '2026-05-14T15:05:00+07:00', 'happiness.points.total', 18, '{"penalty_sources":["gold-disabled","loan-open"],"portfolio_balance":"thin"}'::jsonb, '72100000-0000-0000-0000-000000000004'),
  ('f7000000-0000-0000-0000-000000000008', '81000000-0000-0000-0000-000000000003', null, '2026-05-14T15:05:01+07:00', 'rules.violations.count', 4, '{"errors":["SATURDAY_DISABLED","DONATION_LIMIT_EXCEEDED","LOAN_REPAYMENT_EXCEEDS_BALANCE","PRIMARY_NEED_ORDER"]}'::jsonb, '72100000-0000-0000-0000-000000000004')
on conflict (metric_snapshot_id) do update
set session_id = excluded.session_id,
    player_id = excluded.player_id,
    computed_at = excluded.computed_at,
    metric_name = excluded.metric_name,
    metric_value_numeric = excluded.metric_value_numeric,
    metric_value_json = excluded.metric_value_json,
    ruleset_version_id = excluded.ruleset_version_id;

with scoped_players as (
  select
    sp.session_id,
    sp.player_id,
    sp.join_order,
    sra.ruleset_version_id,
    row_number() over (order by sp.session_id, sp.join_order, sp.player_id) as player_row
  from session_players sp
  join session_ruleset_activations sra on sra.session_id = sp.session_id
  where sp.session_id::text like '81000000-%'
),
metric_names(metric_order, metric_name) as (
  values
    (1, 'cashflow.in.total'),
    (2, 'cashflow.out.total'),
    (3, 'cashflow.net.total'),
    (4, 'donation.total'),
    (5, 'gold.qty.current'),
    (6, 'orders.completed.count'),
    (7, 'inventory.ingredient.total'),
    (8, 'actions.used.total'),
    (9, 'rules.violations.count'),
    (10, 'happiness.points.total')
),
metric_rows as (
  select
    row_number() over (order by sp.session_id, sp.player_row, mn.metric_order) as n,
    sp.session_id,
    sp.player_id,
    sp.ruleset_version_id,
    mn.metric_name,
    case mn.metric_name
      when 'cashflow.in.total' then 20 + sp.join_order * 3
      when 'cashflow.out.total' then 12 + sp.join_order * 4
      when 'cashflow.net.total' then (20 + sp.join_order * 3) - (12 + sp.join_order * 4)
      when 'donation.total' then 1 + sp.join_order
      when 'gold.qty.current' then case when sp.join_order = 3 then -1 else sp.join_order end
      when 'orders.completed.count' then 1
      when 'inventory.ingredient.total' then 2 + sp.join_order
      when 'actions.used.total' then 12 + sp.join_order
      when 'rules.violations.count' then case when sp.session_id = '81000000-0000-0000-0000-000000000003' then sp.join_order else 0 end
      when 'happiness.points.total' then 15 + sp.join_order * 5
      else 0
    end::double precision as metric_value_numeric
  from scoped_players sp
  cross join metric_names mn
)
insert into metric_snapshots (
  metric_snapshot_id,
  session_id,
  player_id,
  computed_at,
  metric_name,
  metric_value_numeric,
  metric_value_json,
  ruleset_version_id
)
select
  ('f7' || lpad((1000 + n)::text, 6, '0') || '-0000-0000-0000-' || lpad((1000 + n)::text, 12, '0'))::uuid,
  session_id,
  player_id,
  '2026-05-19T10:00:00+07:00'::timestamptz + (n || ' seconds')::interval,
  metric_name,
  metric_value_numeric,
  jsonb_build_object('seed_matrix', 'complex-v4', 'row', n, 'metric_family', split_part(metric_name, '.', 1)),
  ruleset_version_id
from metric_rows
on conflict (metric_snapshot_id) do update
set session_id = excluded.session_id,
    player_id = excluded.player_id,
    computed_at = excluded.computed_at,
    metric_name = excluded.metric_name,
    metric_value_numeric = excluded.metric_value_numeric,
    metric_value_json = excluded.metric_value_json,
    ruleset_version_id = excluded.ruleset_version_id;

-- =========================================================
-- 7) Validation log untuk jalur valid dan invalid
-- =========================================================
insert into validation_logs (
  validation_log_id,
  session_id,
  event_pk,
  event_id,
  is_valid,
  error_code,
  error_message,
  details_json,
  created_at
)
values
  ('f6000000-0000-0000-0000-000000000001', '81000000-0000-0000-0000-000000000003', null, 'd6000000-0000-0000-0000-000000000001', false, 'SATURDAY_DISABLED', 'Aksi emas ditolak karena aturan Sabtu nonaktif.', jsonb_build_object('player_id', '22000000-0000-0000-0000-000000000009', 'action_type', 'day.saturday.gold_trade'), '2026-05-14T13:35:00+07:00'),
  ('f6000000-0000-0000-0000-000000000002', '81000000-0000-0000-0000-000000000003', null, 'd6000000-0000-0000-0000-000000000002', false, 'DONATION_LIMIT_EXCEEDED', 'Nominal donasi melebihi batas ruleset hemat.', jsonb_build_object('amount', 9, 'max_amount', 5), '2026-05-14T13:36:00+07:00'),
  ('f6000000-0000-0000-0000-000000000003', '81000000-0000-0000-0000-000000000003', null, 'd6000000-0000-0000-0000-000000000003', false, 'LOAN_REPAYMENT_EXCEEDS_BALANCE', 'Pelunasan pinjaman melebihi saldo pokok tersisa.', jsonb_build_object('loan_id', 'loan-3-2', 'remaining', 5, 'attempted', 11), '2026-05-14T13:37:00+07:00'),
  ('f6000000-0000-0000-0000-000000000004', '81000000-0000-0000-0000-000000000004', null, 'd6000000-0000-0000-0000-000000000004', false, 'PRIMARY_NEED_ORDER', 'Kebutuhan sekunder dibeli sebelum kebutuhan primer saat mode strict.', jsonb_build_object('player_id', '22000000-0000-0000-0000-000000000010'), '2026-05-15T15:45:00+07:00'),
  ('f6000000-0000-0000-0000-000000000005', '81000000-0000-0000-0000-000000000005', null, 'd6000000-0000-0000-0000-000000000005', false, 'RISK_EVENT_ALREADY_CLAIMED', 'Klaim asuransi risiko yang sama sudah pernah dipakai.', jsonb_build_object('risk_event_id', 'e9000528-0000-0000-0000-000000000528'), '2026-05-16T10:02:00+07:00'),
  ('f6000000-0000-0000-0000-000000000006', '81000000-0000-0000-0000-000000000005', null, 'd6000000-0000-0000-0000-000000000006', false, 'SAVING_WITHDRAW_EXCEEDS_BALANCE', 'Penarikan tabungan melebihi saldo tujuan keuangan.', jsonb_build_object('goal_id', 'goal-5-4', 'balance', 2, 'withdraw', 5), '2026-05-16T10:05:00+07:00'),
  ('f6000000-0000-0000-0000-000000000007', '81000000-0000-0000-0000-000000000006', null, 'd6000000-0000-0000-0000-000000000007', false, 'DRAFT_RULESET_NOT_ACTIVE', 'Ruleset draft belum dapat dipakai untuk sesi berjalan.', jsonb_build_object('ruleset_version_id', '72100000-0000-0000-0000-000000000006'), '2026-05-16T13:08:00+07:00'),
  ('f6000000-0000-0000-0000-000000000008', '81000000-0000-0000-0000-000000000007', null, 'd6000000-0000-0000-0000-000000000008', false, 'SESSION_NOT_STARTED', 'Aksi pemain ditolak karena sesi masih CREATED.', jsonb_build_object('session_status', 'CREATED'), '2026-05-17T13:10:00+07:00'),
  ('f6000000-0000-0000-0000-000000000009', '81000000-0000-0000-0000-000000000002', null, 'd6000000-0000-0000-0000-000000000009', true, null, null, jsonb_build_object('batch', 'risk-heavy', 'accepted_events', 104), '2026-05-13T10:25:00+07:00')
on conflict (session_id, event_id) do update
set validation_log_id = excluded.validation_log_id,
    event_pk = excluded.event_pk,
    is_valid = excluded.is_valid,
    error_code = excluded.error_code,
    error_message = excluded.error_message,
    details_json = excluded.details_json,
    created_at = excluded.created_at;

-- =========================================================
-- 8) Audit keamanan dan observability
-- =========================================================
with audit_rows as (
  select n
  from generate_series(1, 48) as gs(n)
)
insert into security_audit_logs (
  security_audit_log_id,
  occurred_at,
  trace_id,
  event_type,
  outcome,
  user_id,
  username,
  role,
  ip_address,
  user_agent,
  method,
  path,
  status_code,
  detail_json
)
select
  ('f5' || lpad(n::text, 6, '0') || '-0000-0000-0000-' || lpad(n::text, 12, '0'))::uuid,
  '2026-05-18T10:00:00+07:00'::timestamptz + (n || ' minutes')::interval,
  'seed-complex-v4-' || lpad(n::text, 3, '0'),
  (array['LOGIN_SUCCESS','LOGIN_FAILURE','SESSION_CREATED','SESSION_STARTED','GAMEPLAY_EVENT_ACCEPTED','GAMEPLAY_EVENT_REJECTED','ANALYTICS_VIEWED','RULESET_CREATED','RULESET_ACTIVATED','AUTH_CHALLENGE','PLAYER_SCOPE_DENIED','RATE_LIMIT_NEAR_LIMIT'])[((n - 1) % 12) + 1],
  case when n % 11 = 0 then 'FAILURE' when n % 5 = 0 then 'DENIED' else 'SUCCESS' end,
  case
    when n % 4 = 0 then '12000000-0000-0000-0000-000000000001'::uuid
    when n % 4 = 1 then '12000000-0000-0000-0000-000000000002'::uuid
    when n % 4 = 2 then '22000000-0000-0000-0000-000000000005'::uuid
    else '22000000-0000-0000-0000-000000000013'::uuid
  end,
  case
    when n % 4 = 0 then 'mira.hartanto'
    when n % 4 = 1 then 'bayu.prakasa'
    when n % 4 = 2 then 'ulfa.ramadhani'
    else 'chandra.gunawan'
  end,
  case when n % 4 in (0, 1) then 'INSTRUCTOR' else 'PLAYER' end,
  '10.40.' || ((n - 1) % 6 + 1) || '.' || (30 + n),
  case when n % 3 = 0 then 'Cashflowpoly Mobile QA' when n % 3 = 1 then 'Mozilla/5.0 Seed Browser' else 'PostmanRuntime Seed' end,
  case when n % 4 = 0 then 'POST' when n % 4 = 1 then 'GET' when n % 4 = 2 then 'PUT' else 'DELETE' end,
  case
    when n % 8 = 0 then '/api/v1/events'
    when n % 8 = 1 then '/api/v1/analytics/sessions/81000000-0000-0000-0000-000000000002'
    when n % 8 = 2 then '/api/v1/sessions/81000000-0000-0000-0000-000000000003'
    when n % 8 = 3 then '/api/v1/rulesets/72000000-0000-0000-0000-000000000002'
    when n % 8 = 4 then '/api/v1/auth/login'
    when n % 8 = 5 then '/api/v1/players/22000000-0000-0000-0000-000000000013'
    when n % 8 = 6 then '/api/v1/observability/metrics'
    else '/api/v1/analytics/rulesets/72000000-0000-0000-0000-000000000003'
  end,
  case when n % 11 = 0 then 401 when n % 5 = 0 then 403 when n % 7 = 0 then 429 else 200 end,
  jsonb_build_object(
    'surface', 'complex_seed',
    'sequence', n,
    'session_family', case when n % 2 = 0 then 'ended-or-live' else 'auth-or-ruleset' end,
    'correlation_hint', 'audit-v4-' || lpad(n::text, 3, '0')
  )
from audit_rows
on conflict (security_audit_log_id) do update
set occurred_at = excluded.occurred_at,
    trace_id = excluded.trace_id,
    event_type = excluded.event_type,
    outcome = excluded.outcome,
    user_id = excluded.user_id,
    username = excluded.username,
    role = excluded.role,
    ip_address = excluded.ip_address,
    user_agent = excluded.user_agent,
    method = excluded.method,
    path = excluded.path,
    status_code = excluded.status_code,
    detail_json = excluded.detail_json;

commit;
