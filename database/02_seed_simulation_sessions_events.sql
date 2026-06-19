begin;

do $$
begin
  if not exists (select 1 from pg_extension where extname = 'pgcrypto') then
    raise exception 'pgcrypto belum aktif. Jalankan database/00_create_schema.sql terlebih dahulu.';
  end if;

  if not exists (
    select 1
    from ruleset_versions
    where ruleset_version_id = 'f5b4c67b-0825-4970-9f07-3b68e8fcb524'::uuid
      and mode = 'PEMULA'
  ) then
    raise exception 'Ruleset default PEMULA dari 01_seed_default_rulesets_components.sql tidak ditemukan.';
  end if;

  if not exists (
    select 1
    from ruleset_versions
    where ruleset_version_id = '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d'::uuid
      and mode = 'MAHIR'
  ) then
    raise exception 'Ruleset default MAHIR dari 01_seed_default_rulesets_components.sql tidak ditemukan.';
  end if;

  if exists (
    select 1
    from app_users
    where lower(username) in ('rina.kartika', 'marco', 'marcello', 'hugo', 'manalu')
      and user_id not in (
        '90000000-0000-0000-0000-000000000001'::uuid,
        '90000000-0000-0000-0000-000000000011'::uuid,
        '90000000-0000-0000-0000-000000000012'::uuid,
        '90000000-0000-0000-0000-000000000013'::uuid,
        '90000000-0000-0000-0000-000000000014'::uuid
      )
  ) then
    raise exception 'Username seed demo sudah dipakai oleh user lain. Bersihkan atau ganti username sebelum menjalankan seed ini.';
  end if;
end $$;

drop table if exists seed_session_scope;
create temporary table seed_session_scope (
  session_id uuid primary key
) on commit drop;

insert into seed_session_scope (session_id)
values
  ('91000000-0000-0000-0000-000000000001'::uuid),
  ('91000000-0000-0000-0000-000000000002'::uuid);

update sessions
set status = 'CREATED',
    player_count = 0,
    started_at = null,
    ended_at = null
where session_id in (select session_id from seed_session_scope);

delete from session_final_score_components
where session_id in (select session_id from seed_session_scope);

delete from session_final_scores
where session_id in (select session_id from seed_session_scope);

delete from metric_snapshots
where session_id in (select session_id from seed_session_scope);

delete from event_cashflow_projections
where session_id in (select session_id from seed_session_scope);

delete from event_asset_references
where session_id in (select session_id from seed_session_scope);

delete from session_narrative_logs
where session_id in (select session_id from seed_session_scope);

delete from session_projection_checkpoints
where session_id in (select session_id from seed_session_scope);

delete from session_card_positions
where session_id in (select session_id from seed_session_scope);

delete from session_donation_events
where session_id in (select session_id from seed_session_scope);

delete from session_participant_action_counters
where session_participant_id in (
  select sp.session_participant_id
  from session_participants sp
  join seed_session_scope ss on ss.session_id = sp.session_id
);

delete from session_participant_collection_missions
where session_id in (select session_id from seed_session_scope);

delete from session_participant_financial_goals
where session_id in (select session_id from seed_session_scope);

delete from session_participant_need_purchases
where session_id in (select session_id from seed_session_scope);

delete from session_participant_inventory
where session_id in (select session_id from seed_session_scope);

delete from session_participant_gold_holdings
where session_id in (select session_id from seed_session_scope);

delete from session_participant_loans
where session_id in (select session_id from seed_session_scope);

delete from session_participant_insurances
where session_id in (select session_id from seed_session_scope);

delete from session_participant_tie_breakers
where session_id in (select session_id from seed_session_scope);

delete from session_participant_balances
where session_id in (select session_id from seed_session_scope);

delete from session_states
where session_id in (select session_id from seed_session_scope);

delete from validation_logs
where session_id in (select session_id from seed_session_scope);

delete from events
where session_id in (select session_id from seed_session_scope);

delete from session_participants
where session_id in (select session_id from seed_session_scope);

delete from sessions
where session_id in (select session_id from seed_session_scope);

insert into app_users (
  user_id,
  username,
  display_name,
  password_hash,
  role,
  is_active,
  created_at
)
values
  (
    '90000000-0000-0000-0000-000000000001'::uuid,
    'rina.kartika',
    'Ibu Rina Kartika, S.Pd.',
    crypt('SeedLocal!2026', gen_salt('bf', 10)),
    'INSTRUCTOR',
    true,
    '2026-01-01T00:00:00Z'
  ),
  (
    '90000000-0000-0000-0000-000000000011'::uuid,
    'marco',
    'Marco',
    crypt('SeedLocal!2026', gen_salt('bf', 10)),
    'PLAYER',
    true,
    '2026-01-01T00:00:00Z'
  ),
  (
    '90000000-0000-0000-0000-000000000012'::uuid,
    'marcello',
    'Marcello',
    crypt('SeedLocal!2026', gen_salt('bf', 10)),
    'PLAYER',
    true,
    '2026-01-01T00:00:00Z'
  ),
  (
    '90000000-0000-0000-0000-000000000013'::uuid,
    'hugo',
    'Hugo',
    crypt('SeedLocal!2026', gen_salt('bf', 10)),
    'PLAYER',
    true,
    '2026-01-01T00:00:00Z'
  ),
  (
    '90000000-0000-0000-0000-000000000014'::uuid,
    'manalu',
    'Manalu',
    crypt('SeedLocal!2026', gen_salt('bf', 10)),
    'PLAYER',
    true,
    '2026-01-01T00:00:00Z'
  )
on conflict (user_id) do update
set username = excluded.username,
    display_name = excluded.display_name,
    password_hash = excluded.password_hash,
    role = excluded.role,
    is_active = excluded.is_active;

insert into sessions (
  session_id,
  session_name,
  ruleset_version_id,
  mode,
  status,
  player_count,
  started_at,
  ended_at,
  instructor_user_id,
  created_at
)
values
  (
    '91000000-0000-0000-0000-000000000001'::uuid,
    'Simulasi Cashflowpoly Kelas XI IPS 2 - Mode Pemula - Kelompok A',
    'f5b4c67b-0825-4970-9f07-3b68e8fcb524'::uuid,
    'PEMULA',
    'CREATED',
    0,
    null,
    null,
    '90000000-0000-0000-0000-000000000001'::uuid,
    '2026-01-05T00:50:00Z'
  ),
  (
    '91000000-0000-0000-0000-000000000002'::uuid,
    'Simulasi Cashflowpoly Kelas XI IPS 2 - Mode Mahir - Kelompok B',
    '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d'::uuid,
    'MAHIR',
    'CREATED',
    0,
    null,
    null,
    '90000000-0000-0000-0000-000000000001'::uuid,
    '2026-02-02T00:50:00Z'
  );

insert into session_participants (
  session_participant_id,
  session_id,
  user_id,
  player_name,
  player_order_no,
  joined_at
)
values
  (
    '93000000-0000-0000-0000-000000000011'::uuid,
    '91000000-0000-0000-0000-000000000001'::uuid,
    '90000000-0000-0000-0000-000000000011'::uuid,
    'Marco',
    1,
    '2026-01-05T00:58:00Z'
  ),
  (
    '93000000-0000-0000-0000-000000000012'::uuid,
    '91000000-0000-0000-0000-000000000001'::uuid,
    '90000000-0000-0000-0000-000000000012'::uuid,
    'Marcello',
    2,
    '2026-01-05T00:58:00Z'
  ),
  (
    '93000000-0000-0000-0000-000000000013'::uuid,
    '91000000-0000-0000-0000-000000000001'::uuid,
    '90000000-0000-0000-0000-000000000013'::uuid,
    'Hugo',
    3,
    '2026-01-05T00:58:00Z'
  ),
  (
    '93000000-0000-0000-0000-000000000014'::uuid,
    '91000000-0000-0000-0000-000000000001'::uuid,
    '90000000-0000-0000-0000-000000000014'::uuid,
    'Manalu',
    4,
    '2026-01-05T00:58:00Z'
  ),
  (
    '93000000-0000-0000-0000-000000000021'::uuid,
    '91000000-0000-0000-0000-000000000002'::uuid,
    '90000000-0000-0000-0000-000000000011'::uuid,
    'Marco',
    1,
    '2026-02-02T00:58:00Z'
  ),
  (
    '93000000-0000-0000-0000-000000000022'::uuid,
    '91000000-0000-0000-0000-000000000002'::uuid,
    '90000000-0000-0000-0000-000000000012'::uuid,
    'Marcello',
    2,
    '2026-02-02T00:58:00Z'
  ),
  (
    '93000000-0000-0000-0000-000000000023'::uuid,
    '91000000-0000-0000-0000-000000000002'::uuid,
    '90000000-0000-0000-0000-000000000013'::uuid,
    'Hugo',
    3,
    '2026-02-02T00:58:00Z'
  ),
  (
    '93000000-0000-0000-0000-000000000024'::uuid,
    '91000000-0000-0000-0000-000000000002'::uuid,
    '90000000-0000-0000-0000-000000000014'::uuid,
    'Manalu',
    4,
    '2026-02-02T00:58:00Z'
  );

update sessions
set status = 'ENDED',
    player_count = 4,
    started_at = case session_id
      when '91000000-0000-0000-0000-000000000001'::uuid then '2026-01-05T01:00:00Z'::timestamptz
      when '91000000-0000-0000-0000-000000000002'::uuid then '2026-02-02T01:00:00Z'::timestamptz
      else started_at
    end,
    ended_at = case session_id
      when '91000000-0000-0000-0000-000000000001'::uuid then '2026-01-29T02:00:00Z'::timestamptz
      when '91000000-0000-0000-0000-000000000002'::uuid then '2026-02-26T02:00:00Z'::timestamptz
      else ended_at
    end
where session_id in (
  '91000000-0000-0000-0000-000000000001'::uuid,
  '91000000-0000-0000-0000-000000000002'::uuid
);

with session_context as (
  select *
  from (
    values
      (
        'PEMULA',
        '91000000-0000-0000-0000-000000000001'::uuid,
        'f5b4c67b-0825-4970-9f07-3b68e8fcb524'::uuid,
        '95000000-0000-0000-0000-',
        '2026-01-05T01:00:00Z'::timestamptz
      ),
      (
        'MAHIR',
        '91000000-0000-0000-0000-000000000002'::uuid,
        '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d'::uuid,
        '96000000-0000-0000-0000-',
        '2026-02-02T01:00:00Z'::timestamptz
      )
  ) as x(session_key, session_id, ruleset_version_id, event_uuid_prefix, base_timestamp)
),
player_context as (
  select *
  from (
    values
      ('PEMULA', 1, '93000000-0000-0000-0000-000000000011'::uuid, '90000000-0000-0000-0000-000000000011'::uuid),
      ('PEMULA', 2, '93000000-0000-0000-0000-000000000012'::uuid, '90000000-0000-0000-0000-000000000012'::uuid),
      ('PEMULA', 3, '93000000-0000-0000-0000-000000000013'::uuid, '90000000-0000-0000-0000-000000000013'::uuid),
      ('PEMULA', 4, '93000000-0000-0000-0000-000000000014'::uuid, '90000000-0000-0000-0000-000000000014'::uuid),
      ('MAHIR', 1, '93000000-0000-0000-0000-000000000021'::uuid, '90000000-0000-0000-0000-000000000011'::uuid),
      ('MAHIR', 2, '93000000-0000-0000-0000-000000000022'::uuid, '90000000-0000-0000-0000-000000000012'::uuid),
      ('MAHIR', 3, '93000000-0000-0000-0000-000000000023'::uuid, '90000000-0000-0000-0000-000000000013'::uuid),
      ('MAHIR', 4, '93000000-0000-0000-0000-000000000024'::uuid, '90000000-0000-0000-0000-000000000014'::uuid)
  ) as x(session_key, player_no, session_player_id, user_id)
),
scenario_event_seed as (
  select *
  from (
    values
      -- PEMULA: sesuai dokumen skenario 4 pemain selama 25 hari.
      ('PEMULA', null, 0, 0, 1, null, 'SYSTEM', null, 'MulaiSesi', '{"start_note":"Mulai sesi simulasi pemula sesuai dokumen skenario"}'::jsonb),
      ('PEMULA', null, 0, 0, 1, 1, 'SYSTEM', 'BagikanTieBreaker', 'BagikanTieBreaker', '{"number":1,"card_code":"tie_breaker_1"}'::jsonb),
      ('PEMULA', null, 0, 0, 1, 2, 'SYSTEM', 'BagikanTieBreaker', 'BagikanTieBreaker', '{"number":2,"card_code":"tie_breaker_2"}'::jsonb),
      ('PEMULA', null, 0, 0, 1, 3, 'SYSTEM', 'BagikanTieBreaker', 'BagikanTieBreaker', '{"number":3,"card_code":"tie_breaker_3"}'::jsonb),
      ('PEMULA', null, 0, 0, 1, 4, 'SYSTEM', 'BagikanTieBreaker', 'BagikanTieBreaker', '{"number":4,"card_code":"tie_breaker_4"}'::jsonb),
      ('PEMULA', null, 0, 1, 1, 1, 'SYSTEM', 'BagikanMisiKoleksi', 'BagikanMisiKoleksi', '{"mission_id":"misi_boneka","target_tertiary_card_id":"boneka","penalty_points":10,"require_primary":true,"require_secondary":true}'::jsonb),
      ('PEMULA', null, 0, 2, 1, 2, 'SYSTEM', 'BagikanMisiKoleksi', 'BagikanMisiKoleksi', '{"mission_id":"misi_gameboy","target_tertiary_card_id":"gameboy","penalty_points":10,"require_primary":true,"require_secondary":true}'::jsonb),
      ('PEMULA', null, 0, 3, 1, 3, 'SYSTEM', 'BagikanMisiKoleksi', 'BagikanMisiKoleksi', '{"mission_id":"misi_hiburan","target_tertiary_card_id":"hiburan","penalty_points":10,"require_primary":true,"require_secondary":true}'::jsonb),
      ('PEMULA', null, 0, 4, 1, 4, 'SYSTEM', 'BagikanMisiKoleksi', 'BagikanMisiKoleksi', '{"mission_id":"misi_boneka","target_tertiary_card_id":"boneka","penalty_points":10,"require_primary":true,"require_secondary":true}'::jsonb),
      ('PEMULA', null, 0, 5, 1, 1, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"nasi_putih","ingredient_name":"Nasi Putih","amount":1}'::jsonb),
      ('PEMULA', null, 0, 6, 1, 1, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"telur","ingredient_name":"Telur","amount":4}'::jsonb),
      ('PEMULA', null, 0, 7, 1, 2, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"nasi_putih","ingredient_name":"Nasi Putih","amount":1}'::jsonb),
      ('PEMULA', null, 0, 8, 1, 2, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"telur","ingredient_name":"Telur","amount":4}'::jsonb),
      ('PEMULA', null, 0, 9, 1, 3, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"daging","ingredient_name":"Daging","amount":5}'::jsonb),
      ('PEMULA', null, 0, 10, 1, 3, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"tahu_tempe","ingredient_name":"Tahu Tempe","amount":3}'::jsonb),
      ('PEMULA', null, 0, 11, 1, 4, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"nasi_putih","ingredient_name":"Nasi Putih","amount":1}'::jsonb),
      ('PEMULA', null, 0, 12, 1, 4, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"sayur","ingredient_name":"Sayur","amount":2}'::jsonb),
      ('PEMULA', null, 0, 13, 1, null, 'SYSTEM', 'AmbilKartuDariDeck', 'AmbilKartuDariDeck', '{"slot_group":"INGREDIENT_MARKET","slot_code":"SLOT_1","asset_type":"INGREDIENT","asset_code":"nasi_putih"}'::jsonb),
      ('PEMULA', null, 0, 14, 1, 1, 'SYSTEM', 'KartuDiambilDariPasar', 'KartuDiambilDariPasar', '{"slot_group":"INGREDIENT_MARKET","slot_code":"SLOT_1","asset_type":"INGREDIENT","asset_code":"nasi_putih"}'::jsonb),
      ('PEMULA', null, 0, 15, 1, null, 'SYSTEM', 'KartuMasukDiscard', 'KartuMasukDiscard', '{"asset_type":"ORDER","asset_code":"order_setup_placeholder"}'::jsonb),
      ('PEMULA', null, 0, 16, 1, null, 'SYSTEM', 'IsiUlangPasar', 'IsiUlangPasar', '{"slot_group":"INGREDIENT_MARKET","slot_code":"SLOT_1","asset_type":"INGREDIENT","asset_code":"sayur"}'::jsonb),
      ('PEMULA', null, 1, 0, 2, 1, 'PLAYER', 'JualMasakan', 'JualMasakan', '{"order_card_id":"nasi_goreng","required_ingredient_card_ids":["nasi_putih","telur"],"income":15}'::jsonb),
      ('PEMULA', null, 1, 1, 2, 1, 'PLAYER', 'Kebutuhan', 'Kebutuhan', '{"card_id":"buku","amount":2,"points":1}'::jsonb),
      ('PEMULA', null, 1, 2, 2, 2, 'PLAYER', 'JualMasakan', 'JualMasakan', '{"order_card_id":"nasi_goreng","required_ingredient_card_ids":["nasi_putih","telur"],"income":15}'::jsonb),
      ('PEMULA', null, 1, 3, 2, 2, 'PLAYER', 'Kebutuhan', 'Kebutuhan', '{"card_id":"buku","amount":2,"points":1}'::jsonb),
      ('PEMULA', null, 1, 4, 2, 3, 'PLAYER', 'JualMasakan', 'JualMasakan', '{"order_card_id":"tahu_campur","required_ingredient_card_ids":["daging","tahu_tempe"],"income":16}'::jsonb),
      ('PEMULA', null, 1, 5, 2, 3, 'PLAYER', 'Kebutuhan', 'Kebutuhan', '{"card_id":"buku","amount":2,"points":1}'::jsonb),
      ('PEMULA', null, 1, 6, 2, 4, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"telur","ingredient_name":"Telur","amount":4}'::jsonb),
      ('PEMULA', null, 1, 7, 2, 4, 'PLAYER', 'Kebutuhan', 'Kebutuhan', '{"card_id":"buku","amount":2,"points":1}'::jsonb),
      ('PEMULA', null, 2, 0, 3, 1, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"daging","ingredient_name":"Daging","amount":5}'::jsonb),
      ('PEMULA', null, 2, 1, 3, 1, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"tahu_tempe","ingredient_name":"Tahu Tempe","amount":3}'::jsonb),
      ('PEMULA', null, 2, 2, 3, 2, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"sayur","ingredient_name":"Sayur","amount":2}'::jsonb),
      ('PEMULA', null, 2, 3, 3, 2, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"tahu_tempe","ingredient_name":"Tahu Tempe","amount":3}'::jsonb),
      ('PEMULA', null, 2, 4, 3, 3, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"nasi_putih","ingredient_name":"Nasi Putih","amount":1}'::jsonb),
      ('PEMULA', null, 2, 5, 3, 3, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"telur","ingredient_name":"Telur","amount":4}'::jsonb),
      ('PEMULA', null, 2, 6, 3, 4, 'PLAYER', 'JualMasakan', 'JualMasakan', '{"order_card_id":"nasi_goreng","required_ingredient_card_ids":["nasi_putih","telur"],"income":15}'::jsonb),
      ('PEMULA', null, 2, 7, 3, 4, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"tahu_tempe","ingredient_name":"Tahu Tempe","amount":3}'::jsonb),
      ('PEMULA', null, 3, 0, 4, 1, 'PLAYER', 'JualMasakan', 'JualMasakan', '{"order_card_id":"tahu_campur","required_ingredient_card_ids":["daging","tahu_tempe"],"income":16}'::jsonb),
      ('PEMULA', null, 3, 1, 4, 1, 'PLAYER', 'Kebutuhan', 'Kebutuhan', '{"card_id":"sepatu","amount":4,"points":2}'::jsonb),
      ('PEMULA', null, 3, 2, 4, 2, 'PLAYER', 'JualMasakan', 'JualMasakan', '{"order_card_id":"resep-sayur-bumbu","required_ingredient_card_ids":["sayur","tahu_tempe"],"income":12}'::jsonb),
      ('PEMULA', null, 3, 3, 4, 2, 'PLAYER', 'Kebutuhan', 'Kebutuhan', '{"card_id":"sepatu","amount":4,"points":2}'::jsonb),
      ('PEMULA', null, 3, 4, 4, 3, 'PLAYER', 'JualMasakan', 'JualMasakan', '{"order_card_id":"nasi_goreng","required_ingredient_card_ids":["nasi_putih","telur"],"income":15}'::jsonb),
      ('PEMULA', null, 3, 5, 4, 3, 'PLAYER', 'Kebutuhan', 'Kebutuhan', '{"card_id":"sepatu","amount":4,"points":2}'::jsonb),
      ('PEMULA', null, 3, 6, 4, 4, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"daging","ingredient_name":"Daging","amount":5}'::jsonb),
      ('PEMULA', null, 3, 7, 4, 4, 'PLAYER', 'Kebutuhan', 'Kebutuhan', '{"card_id":"sepatu","amount":4,"points":2}'::jsonb),
      ('PEMULA', null, 4, 0, 5, 1, 'PLAYER', 'JumatBerkah', 'JumatBerkah', '{"amount":2}'::jsonb),
      ('PEMULA', null, 4, 1, 5, 2, 'PLAYER', 'JumatBerkah', 'JumatBerkah', '{"amount":3}'::jsonb),
      ('PEMULA', null, 4, 2, 5, 3, 'PLAYER', 'JumatBerkah', 'JumatBerkah', '{"amount":1}'::jsonb),
      ('PEMULA', null, 4, 3, 5, 4, 'PLAYER', 'JumatBerkah', 'JumatBerkah', '{"amount":4}'::jsonb),
      ('PEMULA', null, 4, 4, 5, 4, 'SYSTEM', 'PoinPeringkatDonasi', 'PoinPeringkatDonasi', '{"rank":1,"points":7}'::jsonb),
      ('PEMULA', null, 4, 5, 5, 2, 'SYSTEM', 'PoinPeringkatDonasi', 'PoinPeringkatDonasi', '{"rank":2,"points":5}'::jsonb),
      ('PEMULA', null, 4, 6, 5, 1, 'SYSTEM', 'PoinPeringkatDonasi', 'PoinPeringkatDonasi', '{"rank":3,"points":2}'::jsonb),
      ('PEMULA', null, 4, 7, 5, null, 'SYSTEM', 'UmumkanJuaraDonasi', 'UmumkanJuaraDonasi', '{"summary":"Manalu Juara 1, Marcello Juara 2, Marco Juara 3","winners":[{"rank":1,"player_name":"Manalu","player_order_no":4,"points":7},{"rank":2,"player_name":"Marcello","player_order_no":2,"points":5},{"rank":3,"player_name":"Marco","player_order_no":1,"points":2}]}'::jsonb),
      ('PEMULA', null, 5, 0, 6, 1, 'PLAYER', 'InvestasiEmas', 'InvestasiEmas', '{"trade_type":"BUY","unit_price":6,"qty":1,"amount":6}'::jsonb),
      ('PEMULA', null, 5, 1, 6, 2, 'PLAYER', 'InvestasiEmas', 'InvestasiEmas', '{"trade_type":"BUY","unit_price":6,"qty":1,"amount":6}'::jsonb),
      ('PEMULA', null, 5, 2, 6, 3, 'PLAYER', null, 'LewatiTransaksiEmas', '{"note":"Tidak membeli dan tidak menjual emas"}'::jsonb),
      ('PEMULA', null, 5, 3, 6, 4, 'PLAYER', 'InvestasiEmas', 'InvestasiEmas', '{"trade_type":"BUY","unit_price":6,"qty":1,"amount":6}'::jsonb),
      ('PEMULA', null, 6, 0, 7, null, 'SYSTEM', null, 'HariMingguLibur', '{"note":"Hari Minggu libur"}'::jsonb),
      ('PEMULA', null, 7, 0, 8, 1, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"nasi_putih","ingredient_name":"Nasi Putih","amount":1}'::jsonb),
      ('PEMULA', null, 7, 1, 8, 1, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"telur","ingredient_name":"Telur","amount":4}'::jsonb),
      ('PEMULA', null, 7, 2, 8, 2, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"daging","ingredient_name":"Daging","amount":5}'::jsonb),
      ('PEMULA', null, 7, 3, 8, 2, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"tahu_tempe","ingredient_name":"Tahu Tempe","amount":3}'::jsonb),
      ('PEMULA', null, 7, 4, 8, 3, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"sayur","ingredient_name":"Sayur","amount":2}'::jsonb),
      ('PEMULA', null, 7, 5, 8, 3, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"tahu_tempe","ingredient_name":"Tahu Tempe","amount":3}'::jsonb),
      ('PEMULA', null, 7, 6, 8, 4, 'PLAYER', 'JualMasakan', 'JualMasakan', '{"order_card_id":"tahu_campur","required_ingredient_card_ids":["daging","tahu_tempe"],"income":16}'::jsonb),
      ('PEMULA', null, 7, 7, 8, 4, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"nasi_putih","ingredient_name":"Nasi Putih","amount":1}'::jsonb),
      ('PEMULA', null, 8, 0, 9, 1, 'PLAYER', 'JualMasakan', 'JualMasakan', '{"order_card_id":"nasi_goreng","required_ingredient_card_ids":["nasi_putih","telur"],"income":15}'::jsonb),
      ('PEMULA', null, 8, 1, 9, 1, 'PLAYER', 'Kebutuhan', 'Kebutuhan', '{"card_id":"boneka","amount":6,"points":3}'::jsonb),
      ('PEMULA', null, 8, 2, 9, 2, 'PLAYER', 'JualMasakan', 'JualMasakan', '{"order_card_id":"tahu_campur","required_ingredient_card_ids":["daging","tahu_tempe"],"income":16}'::jsonb),
      ('PEMULA', null, 8, 3, 9, 2, 'PLAYER', 'Kebutuhan', 'Kebutuhan', '{"card_id":"gameboy","amount":8,"points":4}'::jsonb),
      ('PEMULA', null, 8, 4, 9, 3, 'PLAYER', 'JualMasakan', 'JualMasakan', '{"order_card_id":"resep-sayur-bumbu","required_ingredient_card_ids":["sayur","tahu_tempe"],"income":12}'::jsonb),
      ('PEMULA', null, 8, 5, 9, 3, 'PLAYER', 'Kebutuhan', 'Kebutuhan', '{"card_id":"hiburan","amount":5,"points":2}'::jsonb),
      ('PEMULA', null, 8, 6, 9, 4, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"telur","ingredient_name":"Telur","amount":4}'::jsonb),
      ('PEMULA', null, 8, 7, 9, 4, 'PLAYER', 'Kebutuhan', 'Kebutuhan', '{"card_id":"boneka","amount":6,"points":3}'::jsonb),
      ('PEMULA', null, 9, 0, 10, 1, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"daging","ingredient_name":"Daging","amount":5}'::jsonb),
      ('PEMULA', null, 9, 1, 10, 1, 'PLAYER', 'KerjaLepas', 'KerjaLepas', '{"amount":1}'::jsonb),
      ('PEMULA', null, 9, 2, 10, 2, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"nasi_putih","ingredient_name":"Nasi Putih","amount":1}'::jsonb),
      ('PEMULA', null, 9, 3, 10, 2, 'PLAYER', 'KerjaLepas', 'KerjaLepas', '{"amount":1}'::jsonb),
      ('PEMULA', null, 9, 4, 10, 3, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"telur","ingredient_name":"Telur","amount":4}'::jsonb),
      ('PEMULA', null, 9, 5, 10, 3, 'PLAYER', 'KerjaLepas', 'KerjaLepas', '{"amount":1}'::jsonb),
      ('PEMULA', null, 9, 6, 10, 4, 'PLAYER', 'JualMasakan', 'JualMasakan', '{"order_card_id":"nasi_goreng","required_ingredient_card_ids":["nasi_putih","telur"],"income":15}'::jsonb),
      ('PEMULA', null, 9, 7, 10, 4, 'PLAYER', 'KerjaLepas', 'KerjaLepas', '{"amount":1}'::jsonb),
      ('PEMULA', null, 10, 0, 11, 1, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"tahu_tempe","ingredient_name":"Tahu Tempe","amount":3}'::jsonb),
      ('PEMULA', null, 10, 1, 11, 1, 'PLAYER', 'JualMasakan', 'JualMasakan', '{"order_card_id":"tahu_campur","required_ingredient_card_ids":["daging","tahu_tempe"],"income":16}'::jsonb),
      ('PEMULA', null, 10, 2, 11, 2, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"telur","ingredient_name":"Telur","amount":4}'::jsonb),
      ('PEMULA', null, 10, 3, 11, 2, 'PLAYER', 'JualMasakan', 'JualMasakan', '{"order_card_id":"nasi_goreng","required_ingredient_card_ids":["nasi_putih","telur"],"income":15}'::jsonb),
      ('PEMULA', null, 10, 4, 11, 3, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"nasi_putih","ingredient_name":"Nasi Putih","amount":1}'::jsonb),
      ('PEMULA', null, 10, 5, 11, 3, 'PLAYER', 'JualMasakan', 'JualMasakan', '{"order_card_id":"nasi_goreng","required_ingredient_card_ids":["nasi_putih","telur"],"income":15}'::jsonb),
      ('PEMULA', null, 10, 6, 11, 4, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"sayur","ingredient_name":"Sayur","amount":2}'::jsonb),
      ('PEMULA', null, 10, 7, 11, 4, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"tahu_tempe","ingredient_name":"Tahu Tempe","amount":3}'::jsonb),
      ('PEMULA', null, 11, 0, 12, 1, 'PLAYER', 'JumatBerkah', 'JumatBerkah', '{"amount":5}'::jsonb),
      ('PEMULA', null, 11, 1, 12, 2, 'PLAYER', 'JumatBerkah', 'JumatBerkah', '{"amount":2}'::jsonb),
      ('PEMULA', null, 11, 2, 12, 3, 'PLAYER', 'JumatBerkah', 'JumatBerkah', '{"amount":4}'::jsonb),
      ('PEMULA', null, 11, 3, 12, 4, 'PLAYER', 'JumatBerkah', 'JumatBerkah', '{"amount":3}'::jsonb),
      ('PEMULA', null, 11, 4, 12, 1, 'SYSTEM', 'PoinPeringkatDonasi', 'PoinPeringkatDonasi', '{"rank":1,"points":7}'::jsonb),
      ('PEMULA', null, 11, 5, 12, 3, 'SYSTEM', 'PoinPeringkatDonasi', 'PoinPeringkatDonasi', '{"rank":2,"points":5}'::jsonb),
      ('PEMULA', null, 11, 6, 12, 4, 'SYSTEM', 'PoinPeringkatDonasi', 'PoinPeringkatDonasi', '{"rank":3,"points":2}'::jsonb),
      ('PEMULA', null, 11, 7, 12, null, 'SYSTEM', 'UmumkanJuaraDonasi', 'UmumkanJuaraDonasi', '{"summary":"Marco Juara 1, Hugo Juara 2, Manalu Juara 3","winners":[{"rank":1,"player_name":"Marco","player_order_no":1,"points":7},{"rank":2,"player_name":"Hugo","player_order_no":3,"points":5},{"rank":3,"player_name":"Manalu","player_order_no":4,"points":2}]}'::jsonb),
      ('PEMULA', null, 12, 0, 13, 1, 'PLAYER', 'InvestasiEmas', 'InvestasiEmas', '{"trade_type":"BUY","unit_price":8,"qty":1,"amount":8}'::jsonb),
      ('PEMULA', null, 12, 1, 13, 2, 'PLAYER', 'JualEmas', 'JualEmas', '{"trade_type":"SELL","unit_price":8,"qty":1,"amount":8}'::jsonb),
      ('PEMULA', null, 12, 2, 13, 3, 'PLAYER', 'InvestasiEmas', 'InvestasiEmas', '{"trade_type":"BUY","unit_price":8,"qty":1,"amount":8}'::jsonb),
      ('PEMULA', null, 12, 3, 13, 4, 'PLAYER', 'InvestasiEmas', 'InvestasiEmas', '{"trade_type":"BUY","unit_price":8,"qty":1,"amount":8}'::jsonb),
      ('PEMULA', null, 13, 0, 14, null, 'SYSTEM', null, 'HariMingguLibur', '{"note":"Hari Minggu libur"}'::jsonb),
      ('PEMULA', null, 14, 0, 15, 1, 'PLAYER', 'Kebutuhan', 'Kebutuhan', '{"card_id":"buku","amount":2,"points":1}'::jsonb),
      ('PEMULA', null, 14, 1, 15, 1, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"nasi_putih","ingredient_name":"Nasi Putih","amount":1}'::jsonb),
      ('PEMULA', null, 14, 2, 15, 2, 'PLAYER', 'Kebutuhan', 'Kebutuhan', '{"card_id":"buku","amount":2,"points":1}'::jsonb),
      ('PEMULA', null, 14, 3, 15, 2, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"daging","ingredient_name":"Daging","amount":5}'::jsonb),
      ('PEMULA', null, 14, 4, 15, 3, 'PLAYER', 'Kebutuhan', 'Kebutuhan', '{"card_id":"buku","amount":2,"points":1}'::jsonb),
      ('PEMULA', null, 14, 5, 15, 3, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"tahu_tempe","ingredient_name":"Tahu Tempe","amount":3}'::jsonb),
      ('PEMULA', null, 14, 6, 15, 4, 'PLAYER', 'JualMasakan', 'JualMasakan', '{"order_card_id":"resep-sayur-bumbu","required_ingredient_card_ids":["sayur","tahu_tempe"],"income":12}'::jsonb),
      ('PEMULA', null, 14, 7, 15, 4, 'PLAYER', 'Kebutuhan', 'Kebutuhan', '{"card_id":"buku","amount":2,"points":1}'::jsonb),
      ('PEMULA', null, 15, 0, 16, 1, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"telur","ingredient_name":"Telur","amount":4}'::jsonb),
      ('PEMULA', null, 15, 1, 16, 1, 'PLAYER', 'JualMasakan', 'JualMasakan', '{"order_card_id":"nasi_goreng","required_ingredient_card_ids":["nasi_putih","telur"],"income":15}'::jsonb),
      ('PEMULA', null, 15, 2, 16, 2, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"tahu_tempe","ingredient_name":"Tahu Tempe","amount":3}'::jsonb),
      ('PEMULA', null, 15, 3, 16, 2, 'PLAYER', 'JualMasakan', 'JualMasakan', '{"order_card_id":"tahu_campur","required_ingredient_card_ids":["daging","tahu_tempe"],"income":16}'::jsonb),
      ('PEMULA', null, 15, 4, 16, 3, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"daging","ingredient_name":"Daging","amount":5}'::jsonb),
      ('PEMULA', null, 15, 5, 16, 3, 'PLAYER', 'JualMasakan', 'JualMasakan', '{"order_card_id":"tahu_campur","required_ingredient_card_ids":["daging","tahu_tempe"],"income":16}'::jsonb),
      ('PEMULA', null, 15, 6, 16, 4, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"nasi_putih","ingredient_name":"Nasi Putih","amount":1}'::jsonb),
      ('PEMULA', null, 15, 7, 16, 4, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"telur","ingredient_name":"Telur","amount":4}'::jsonb),
      ('PEMULA', null, 16, 0, 17, 1, 'PLAYER', 'Kebutuhan', 'Kebutuhan', '{"card_id":"sepatu","amount":4,"points":2}'::jsonb),
      ('PEMULA', null, 16, 1, 17, 1, 'PLAYER', 'KerjaLepas', 'KerjaLepas', '{"amount":1}'::jsonb),
      ('PEMULA', null, 16, 2, 17, 2, 'PLAYER', 'Kebutuhan', 'Kebutuhan', '{"card_id":"sepatu","amount":4,"points":2}'::jsonb),
      ('PEMULA', null, 16, 3, 17, 2, 'PLAYER', 'KerjaLepas', 'KerjaLepas', '{"amount":1}'::jsonb),
      ('PEMULA', null, 16, 4, 17, 3, 'PLAYER', 'Kebutuhan', 'Kebutuhan', '{"card_id":"sepatu","amount":4,"points":2}'::jsonb),
      ('PEMULA', null, 16, 5, 17, 3, 'PLAYER', 'KerjaLepas', 'KerjaLepas', '{"amount":1}'::jsonb),
      ('PEMULA', null, 16, 6, 17, 4, 'PLAYER', 'JualMasakan', 'JualMasakan', '{"order_card_id":"nasi_goreng","required_ingredient_card_ids":["nasi_putih","telur"],"income":15}'::jsonb),
      ('PEMULA', null, 16, 7, 17, 4, 'PLAYER', 'Kebutuhan', 'Kebutuhan', '{"card_id":"sepatu","amount":4,"points":2}'::jsonb),
      ('PEMULA', null, 17, 0, 18, 1, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"sayur","ingredient_name":"Sayur","amount":2}'::jsonb),
      ('PEMULA', null, 17, 1, 18, 1, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"tahu_tempe","ingredient_name":"Tahu Tempe","amount":3}'::jsonb),
      ('PEMULA', null, 17, 2, 18, 2, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"nasi_putih","ingredient_name":"Nasi Putih","amount":1}'::jsonb),
      ('PEMULA', null, 17, 3, 18, 2, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"telur","ingredient_name":"Telur","amount":4}'::jsonb),
      ('PEMULA', null, 17, 4, 18, 3, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"nasi_putih","ingredient_name":"Nasi Putih","amount":1}'::jsonb),
      ('PEMULA', null, 17, 5, 18, 3, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"telur","ingredient_name":"Telur","amount":4}'::jsonb),
      ('PEMULA', null, 17, 6, 18, 4, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"daging","ingredient_name":"Daging","amount":5}'::jsonb),
      ('PEMULA', null, 17, 7, 18, 4, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"tahu_tempe","ingredient_name":"Tahu Tempe","amount":3}'::jsonb),
      ('PEMULA', null, 18, 0, 19, 1, 'PLAYER', 'JumatBerkah', 'JumatBerkah', '{"amount":3}'::jsonb),
      ('PEMULA', null, 18, 1, 19, 2, 'PLAYER', 'JumatBerkah', 'JumatBerkah', '{"amount":5}'::jsonb),
      ('PEMULA', null, 18, 2, 19, 3, 'PLAYER', 'JumatBerkah', 'JumatBerkah', '{"amount":2}'::jsonb),
      ('PEMULA', null, 18, 3, 19, 4, 'PLAYER', 'JumatBerkah', 'JumatBerkah', '{"amount":4}'::jsonb),
      ('PEMULA', null, 18, 4, 19, 2, 'SYSTEM', 'PoinPeringkatDonasi', 'PoinPeringkatDonasi', '{"rank":1,"points":7}'::jsonb),
      ('PEMULA', null, 18, 5, 19, 4, 'SYSTEM', 'PoinPeringkatDonasi', 'PoinPeringkatDonasi', '{"rank":2,"points":5}'::jsonb),
      ('PEMULA', null, 18, 6, 19, 1, 'SYSTEM', 'PoinPeringkatDonasi', 'PoinPeringkatDonasi', '{"rank":3,"points":2}'::jsonb),
      ('PEMULA', null, 18, 7, 19, null, 'SYSTEM', 'UmumkanJuaraDonasi', 'UmumkanJuaraDonasi', '{"summary":"Marcello Juara 1, Manalu Juara 2, Marco Juara 3","winners":[{"rank":1,"player_name":"Marcello","player_order_no":2,"points":7},{"rank":2,"player_name":"Manalu","player_order_no":4,"points":5},{"rank":3,"player_name":"Marco","player_order_no":1,"points":2}]}'::jsonb),
      ('PEMULA', null, 19, 0, 20, 1, 'PLAYER', 'JualEmas', 'JualEmas', '{"trade_type":"SELL","unit_price":7,"qty":1,"amount":7}'::jsonb),
      ('PEMULA', null, 19, 1, 20, 2, 'PLAYER', 'InvestasiEmas', 'InvestasiEmas', '{"trade_type":"BUY","unit_price":7,"qty":1,"amount":7}'::jsonb),
      ('PEMULA', null, 19, 2, 20, 3, 'PLAYER', 'InvestasiEmas', 'InvestasiEmas', '{"trade_type":"BUY","unit_price":7,"qty":1,"amount":7}'::jsonb),
      ('PEMULA', null, 19, 3, 20, 4, 'PLAYER', null, 'LewatiTransaksiEmas', '{"note":"Tidak membeli dan tidak menjual emas"}'::jsonb),
      ('PEMULA', null, 20, 0, 21, null, 'SYSTEM', null, 'HariMingguLibur', '{"note":"Hari Minggu libur"}'::jsonb),
      ('PEMULA', null, 21, 0, 22, 1, 'PLAYER', 'JualMasakan', 'JualMasakan', '{"order_card_id":"resep-sayur-bumbu","required_ingredient_card_ids":["sayur","tahu_tempe"],"income":12}'::jsonb),
      ('PEMULA', null, 21, 1, 22, 1, 'PLAYER', 'Kebutuhan', 'Kebutuhan', '{"card_id":"boneka","amount":6,"points":3}'::jsonb),
      ('PEMULA', null, 21, 2, 22, 2, 'PLAYER', 'JualMasakan', 'JualMasakan', '{"order_card_id":"nasi_goreng","required_ingredient_card_ids":["nasi_putih","telur"],"income":15}'::jsonb),
      ('PEMULA', null, 21, 3, 22, 2, 'PLAYER', 'Kebutuhan', 'Kebutuhan', '{"card_id":"gameboy","amount":8,"points":4}'::jsonb),
      ('PEMULA', null, 21, 4, 22, 3, 'PLAYER', 'JualMasakan', 'JualMasakan', '{"order_card_id":"nasi_goreng","required_ingredient_card_ids":["nasi_putih","telur"],"income":15}'::jsonb),
      ('PEMULA', null, 21, 5, 22, 3, 'PLAYER', 'Kebutuhan', 'Kebutuhan', '{"card_id":"hiburan","amount":5,"points":2}'::jsonb),
      ('PEMULA', null, 21, 6, 22, 4, 'PLAYER', 'JualMasakan', 'JualMasakan', '{"order_card_id":"tahu_campur","required_ingredient_card_ids":["daging","tahu_tempe"],"income":16}'::jsonb),
      ('PEMULA', null, 21, 7, 22, 4, 'PLAYER', 'Kebutuhan', 'Kebutuhan', '{"card_id":"boneka","amount":6,"points":3}'::jsonb),
      ('PEMULA', null, 22, 0, 23, 1, 'PLAYER', 'KerjaLepas', 'KerjaLepas', '{"amount":1}'::jsonb),
      ('PEMULA', null, 22, 1, 23, 1, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"nasi_putih","ingredient_name":"Nasi Putih","amount":1}'::jsonb),
      ('PEMULA', null, 22, 2, 23, 2, 'PLAYER', 'KerjaLepas', 'KerjaLepas', '{"amount":1}'::jsonb),
      ('PEMULA', null, 22, 3, 23, 2, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"tahu_tempe","ingredient_name":"Tahu Tempe","amount":3}'::jsonb),
      ('PEMULA', null, 22, 4, 23, 3, 'PLAYER', 'KerjaLepas', 'KerjaLepas', '{"amount":1}'::jsonb),
      ('PEMULA', null, 22, 5, 23, 3, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"daging","ingredient_name":"Daging","amount":5}'::jsonb),
      ('PEMULA', null, 22, 6, 23, 4, 'PLAYER', 'KerjaLepas', 'KerjaLepas', '{"amount":1}'::jsonb),
      ('PEMULA', null, 22, 7, 23, 4, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"telur","ingredient_name":"Telur","amount":4}'::jsonb),
      ('PEMULA', null, 23, 0, 24, 1, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"telur","ingredient_name":"Telur","amount":4}'::jsonb),
      ('PEMULA', null, 23, 1, 24, 1, 'PLAYER', 'JualMasakan', 'JualMasakan', '{"order_card_id":"nasi_goreng","required_ingredient_card_ids":["nasi_putih","telur"],"income":15}'::jsonb),
      ('PEMULA', null, 23, 2, 24, 2, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"daging","ingredient_name":"Daging","amount":5}'::jsonb),
      ('PEMULA', null, 23, 3, 24, 2, 'PLAYER', 'JualMasakan', 'JualMasakan', '{"order_card_id":"tahu_campur","required_ingredient_card_ids":["daging","tahu_tempe"],"income":16}'::jsonb),
      ('PEMULA', null, 23, 4, 24, 3, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"tahu_tempe","ingredient_name":"Tahu Tempe","amount":3}'::jsonb),
      ('PEMULA', null, 23, 5, 24, 3, 'PLAYER', 'JualMasakan', 'JualMasakan', '{"order_card_id":"tahu_campur","required_ingredient_card_ids":["daging","tahu_tempe"],"income":16}'::jsonb),
      ('PEMULA', null, 23, 6, 24, 4, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"nasi_putih","ingredient_name":"Nasi Putih","amount":1}'::jsonb),
      ('PEMULA', null, 23, 7, 24, 4, 'PLAYER', 'JualMasakan', 'JualMasakan', '{"order_card_id":"nasi_goreng","required_ingredient_card_ids":["nasi_putih","telur"],"income":15}'::jsonb),
      ('PEMULA', null, 24, 0, 25, 1, 'PLAYER', 'Kebutuhan', 'Kebutuhan', '{"card_id":"buku","amount":2,"points":1}'::jsonb),
      ('PEMULA', null, 24, 1, 25, 1, 'PLAYER', 'KerjaLepas', 'KerjaLepas', '{"amount":1}'::jsonb),
      ('PEMULA', null, 24, 2, 25, 2, 'PLAYER', 'Kebutuhan', 'Kebutuhan', '{"card_id":"buku","amount":2,"points":1}'::jsonb),
      ('PEMULA', null, 24, 3, 25, 2, 'PLAYER', 'KerjaLepas', 'KerjaLepas', '{"amount":1}'::jsonb),
      ('PEMULA', null, 24, 4, 25, 3, 'PLAYER', 'Kebutuhan', 'Kebutuhan', '{"card_id":"buku","amount":2,"points":1}'::jsonb),
      ('PEMULA', null, 24, 5, 25, 3, 'PLAYER', 'KerjaLepas', 'KerjaLepas', '{"amount":1}'::jsonb),
      ('PEMULA', null, 24, 6, 25, 4, 'PLAYER', 'Kebutuhan', 'Kebutuhan', '{"card_id":"buku","amount":2,"points":1}'::jsonb),
      ('PEMULA', null, 24, 7, 25, 4, 'PLAYER', 'KerjaLepas', 'KerjaLepas', '{"amount":1}'::jsonb),
      ('PEMULA', null, 24, 8, 25, null, 'SYSTEM', 'AkhiriSesi', 'AkhiriSesi', '{"end_note":"Selesai sesi pemula sesuai dokumen skenario"}'::jsonb),

      -- MAHIR: sesuai dokumen skenario 4 pemain selama 25 hari.
      ('MAHIR', null, 0, 0, 1, null, 'SYSTEM', null, 'MulaiSesi', '{"start_note":"Mulai sesi simulasi mahir sesuai dokumen skenario"}'::jsonb),
      ('MAHIR', null, 0, 0, 1, 1, 'SYSTEM', 'BagikanTieBreaker', 'BagikanTieBreaker', '{"number":1,"card_code":"tie_breaker_1"}'::jsonb),
      ('MAHIR', null, 0, 0, 1, 2, 'SYSTEM', 'BagikanTieBreaker', 'BagikanTieBreaker', '{"number":2,"card_code":"tie_breaker_2"}'::jsonb),
      ('MAHIR', null, 0, 0, 1, 3, 'SYSTEM', 'BagikanTieBreaker', 'BagikanTieBreaker', '{"number":3,"card_code":"tie_breaker_3"}'::jsonb),
      ('MAHIR', null, 0, 0, 1, 4, 'SYSTEM', 'BagikanTieBreaker', 'BagikanTieBreaker', '{"number":4,"card_code":"tie_breaker_4"}'::jsonb),
      ('MAHIR', null, 0, 1, 1, 1, 'SYSTEM', 'BagikanMisiKoleksi', 'BagikanMisiKoleksi', '{"mission_id":"misi_boneka","target_tertiary_card_id":"boneka","penalty_points":10,"require_primary":true,"require_secondary":true}'::jsonb),
      ('MAHIR', null, 0, 2, 1, 2, 'SYSTEM', 'BagikanMisiKoleksi', 'BagikanMisiKoleksi', '{"mission_id":"misi_gameboy","target_tertiary_card_id":"gameboy","penalty_points":10,"require_primary":true,"require_secondary":true}'::jsonb),
      ('MAHIR', null, 0, 3, 1, 3, 'SYSTEM', 'BagikanMisiKoleksi', 'BagikanMisiKoleksi', '{"mission_id":"misi_hiburan","target_tertiary_card_id":"hiburan","penalty_points":10,"require_primary":true,"require_secondary":true}'::jsonb),
      ('MAHIR', null, 0, 4, 1, 4, 'SYSTEM', 'BagikanMisiKoleksi', 'BagikanMisiKoleksi', '{"mission_id":"misi_boneka","target_tertiary_card_id":"boneka","penalty_points":10,"require_primary":true,"require_secondary":true}'::jsonb),
      ('MAHIR', null, 0, 5, 1, 1, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"nasi_putih","ingredient_name":"Nasi Putih","amount":1}'::jsonb),
      ('MAHIR', null, 0, 6, 1, 1, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"telur","ingredient_name":"Telur","amount":4}'::jsonb),
      ('MAHIR', null, 0, 7, 1, 2, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"daging","ingredient_name":"Daging","amount":5}'::jsonb),
      ('MAHIR', null, 0, 8, 1, 2, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"tahu_tempe","ingredient_name":"Tahu Tempe","amount":3}'::jsonb),
      ('MAHIR', null, 0, 9, 1, 3, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"nasi_putih","ingredient_name":"Nasi Putih","amount":1}'::jsonb),
      ('MAHIR', null, 0, 10, 1, 3, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"telur","ingredient_name":"Telur","amount":4}'::jsonb),
      ('MAHIR', null, 0, 11, 1, 4, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"sayur","ingredient_name":"Sayur","amount":2}'::jsonb),
      ('MAHIR', null, 0, 12, 1, 4, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"tahu_tempe","ingredient_name":"Tahu Tempe","amount":3}'::jsonb),
      ('MAHIR', null, 0, 21, 1, null, 'SYSTEM', 'AmbilKartuDariDeck', 'AmbilKartuDariDeck', '{"slot_group":"INGREDIENT_MARKET","slot_code":"SLOT_1","asset_type":"INGREDIENT","asset_code":"nasi_putih"}'::jsonb),
      ('MAHIR', null, 0, 22, 1, 1, 'SYSTEM', 'KartuDiambilDariPasar', 'KartuDiambilDariPasar', '{"slot_group":"INGREDIENT_MARKET","slot_code":"SLOT_1","asset_type":"INGREDIENT","asset_code":"nasi_putih"}'::jsonb),
      ('MAHIR', null, 0, 23, 1, null, 'SYSTEM', 'KartuMasukDiscard', 'KartuMasukDiscard', '{"asset_type":"ORDER","asset_code":"order_setup_placeholder"}'::jsonb),
      ('MAHIR', null, 0, 24, 1, null, 'SYSTEM', 'IsiUlangPasar', 'IsiUlangPasar', '{"slot_group":"INGREDIENT_MARKET","slot_code":"SLOT_1","asset_type":"INGREDIENT","asset_code":"sayur"}'::jsonb),
      ('MAHIR', null, 1, 0, 2, 1, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"telur","ingredient_name":"Telur","amount":4}'::jsonb),
      ('MAHIR', 'mahir-risk-001', 1, 1, 2, 1, 'PLAYER', 'RisikoKehidupan', 'RisikoKehidupan', '{"risk_id":"risk_bonus_tunjangan","note":"Dapat tunjangan keluarga"}'::jsonb),
      ('MAHIR', null, 1, 2, 2, 1, 'PLAYER', 'Asuransi', 'Asuransi', '{"policy_id":"INS-SEED-001","premium":1,"coverage_type":"MULTIRISK"}'::jsonb),
      ('MAHIR', null, 1, 3, 2, 2, 'PLAYER', 'JualMasakan', 'JualMasakan', '{"order_card_id":"tahu_campur","required_ingredient_card_ids":["daging","tahu_tempe"],"income":16}'::jsonb),
      ('MAHIR', 'mahir-risk-002', 1, 4, 2, 2, 'PLAYER', 'RisikoKehidupan', 'RisikoKehidupan', '{"risk_id":"risk_biaya_perbaikan","note":"Biaya perbaikan mendadak"}'::jsonb),
      ('MAHIR', null, 1, 5, 2, 2, 'PLAYER', 'Kebutuhan', 'Kebutuhan', '{"card_id":"buku","amount":2,"points":1}'::jsonb),
      ('MAHIR', null, 1, 6, 2, 3, 'PLAYER', 'JualMasakan', 'JualMasakan', '{"order_card_id":"nasi_goreng","required_ingredient_card_ids":["nasi_putih","telur"],"income":15}'::jsonb),
      ('MAHIR', 'mahir-risk-003', 1, 7, 2, 3, 'PLAYER', 'RisikoKehidupan', 'RisikoKehidupan', '{"risk_id":"risk_bonus_kompetisi","note":"Hadiah kompetisi"}'::jsonb),
      ('MAHIR', null, 1, 8, 2, 3, 'PLAYER', 'Kebutuhan', 'Kebutuhan', '{"card_id":"buku","amount":2,"points":1}'::jsonb),
      ('MAHIR', null, 1, 9, 2, 4, 'PLAYER', 'JualMasakan', 'JualMasakan', '{"order_card_id":"resep-sayur-bumbu","required_ingredient_card_ids":["sayur","tahu_tempe"],"income":12}'::jsonb),
      ('MAHIR', 'mahir-risk-004', 1, 10, 2, 4, 'PLAYER', 'RisikoKehidupan', 'RisikoKehidupan', '{"risk_id":"risk_bonus_tetangga","note":"Tetangga berbagi rezeki"}'::jsonb),
      ('MAHIR', null, 1, 11, 2, 4, 'PLAYER', 'Asuransi', 'Asuransi', '{"policy_id":"INS-SEED-002","premium":1,"coverage_type":"MULTIRISK"}'::jsonb),
      ('MAHIR', null, 2, 0, 3, 1, 'PLAYER', 'Kebutuhan', 'Kebutuhan', '{"card_id":"buku","amount":2,"points":1}'::jsonb),
      ('MAHIR', null, 2, 1, 3, 1, 'PLAYER', 'Menabung', 'Menabung', '{"goal_id":"beli_rumah","amount":5}'::jsonb),
      ('MAHIR', null, 2, 2, 3, 2, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"nasi_putih","ingredient_name":"Nasi Putih","amount":1}'::jsonb),
      ('MAHIR', null, 2, 3, 3, 2, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"telur","ingredient_name":"Telur","amount":4}'::jsonb),
      ('MAHIR', null, 2, 4, 3, 3, 'PLAYER', 'Menabung', 'Menabung', '{"goal_id":"beli_rumah","amount":5}'::jsonb),
      ('MAHIR', null, 2, 5, 3, 3, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"daging","ingredient_name":"Daging","amount":5}'::jsonb),
      ('MAHIR', null, 2, 6, 3, 4, 'PLAYER', 'Kebutuhan', 'Kebutuhan', '{"card_id":"buku","amount":2,"points":1}'::jsonb),
      ('MAHIR', null, 2, 7, 3, 4, 'PLAYER', 'Menabung', 'Menabung', '{"goal_id":"beli_motor","amount":5}'::jsonb),
      ('MAHIR', null, 3, 0, 4, 1, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"daging","ingredient_name":"Daging","amount":5}'::jsonb),
      ('MAHIR', null, 3, 1, 4, 1, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"tahu_tempe","ingredient_name":"Tahu Tempe","amount":3}'::jsonb),
      ('MAHIR', null, 3, 2, 4, 2, 'PLAYER', 'JualMasakan', 'JualMasakan', '{"order_card_id":"nasi_goreng","required_ingredient_card_ids":["nasi_putih","telur"],"income":15}'::jsonb),
      ('MAHIR', 'mahir-risk-005', 3, 3, 4, 2, 'PLAYER', 'RisikoKehidupan', 'RisikoKehidupan', '{"risk_id":"risk_bayar_obat","note":"Biaya obat keluarga"}'::jsonb),
      ('MAHIR', null, 3, 4, 4, 2, 'PLAYER', 'Menabung', 'Menabung', '{"goal_id":"beli_motor","amount":5}'::jsonb),
      ('MAHIR', null, 3, 5, 4, 3, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"tahu_tempe","ingredient_name":"Tahu Tempe","amount":3}'::jsonb),
      ('MAHIR', null, 3, 6, 4, 3, 'PLAYER', 'JualMasakan', 'JualMasakan', '{"order_card_id":"tahu_campur","required_ingredient_card_ids":["daging","tahu_tempe"],"income":16}'::jsonb),
      ('MAHIR', 'mahir-risk-006', 3, 7, 4, 3, 'PLAYER', 'RisikoKehidupan', 'RisikoKehidupan', '{"risk_id":"risk_bonus_proyek","note":"Dapat bonus proyek"}'::jsonb),
      ('MAHIR', null, 3, 8, 4, 4, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"nasi_putih","ingredient_name":"Nasi Putih","amount":1}'::jsonb),
      ('MAHIR', null, 3, 9, 4, 4, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"telur","ingredient_name":"Telur","amount":4}'::jsonb),
      ('MAHIR', null, 4, 0, 5, 1, 'PLAYER', 'JumatBerkah', 'JumatBerkah', '{"amount":1}'::jsonb),
      ('MAHIR', null, 4, 1, 5, 2, 'PLAYER', 'JumatBerkah', 'JumatBerkah', '{"amount":3}'::jsonb),
      ('MAHIR', null, 4, 2, 5, 3, 'PLAYER', 'JumatBerkah', 'JumatBerkah', '{"amount":2}'::jsonb),
      ('MAHIR', null, 4, 3, 5, 4, 'PLAYER', 'JumatBerkah', 'JumatBerkah', '{"amount":4}'::jsonb),
      ('MAHIR', null, 4, 4, 5, 4, 'SYSTEM', 'PoinPeringkatDonasi', 'PoinPeringkatDonasi', '{"rank":1,"points":7}'::jsonb),
      ('MAHIR', null, 4, 5, 5, 2, 'SYSTEM', 'PoinPeringkatDonasi', 'PoinPeringkatDonasi', '{"rank":2,"points":5}'::jsonb),
      ('MAHIR', null, 4, 6, 5, 3, 'SYSTEM', 'PoinPeringkatDonasi', 'PoinPeringkatDonasi', '{"rank":3,"points":2}'::jsonb),
      ('MAHIR', null, 4, 7, 5, null, 'SYSTEM', 'UmumkanJuaraDonasi', 'UmumkanJuaraDonasi', '{"summary":"Manalu Juara 1, Marcello Juara 2, Hugo Juara 3","winners":[{"rank":1,"player_name":"Manalu","player_order_no":4,"points":7},{"rank":2,"player_name":"Marcello","player_order_no":2,"points":5},{"rank":3,"player_name":"Hugo","player_order_no":3,"points":2}]}'::jsonb),
      ('MAHIR', null, 5, 0, 6, 1, 'PLAYER', 'InvestasiEmas', 'InvestasiEmas', '{"trade_type":"BUY","unit_price":6,"qty":1,"amount":6}'::jsonb),
      ('MAHIR', null, 5, 1, 6, 2, 'PLAYER', null, 'LewatiTransaksiEmas', '{"note":"Tidak membeli dan tidak menjual emas"}'::jsonb),
      ('MAHIR', null, 5, 2, 6, 3, 'PLAYER', 'InvestasiEmas', 'InvestasiEmas', '{"trade_type":"BUY","unit_price":6,"qty":1,"amount":6}'::jsonb),
      ('MAHIR', null, 5, 3, 6, 4, 'PLAYER', 'InvestasiEmas', 'InvestasiEmas', '{"trade_type":"BUY","unit_price":6,"qty":1,"amount":6}'::jsonb),
      ('MAHIR', null, 6, 0, 7, null, 'SYSTEM', null, 'HariMingguLibur', '{"note":"Hari Minggu libur"}'::jsonb),
      ('MAHIR', null, 7, 0, 8, 1, 'PLAYER', 'JualMasakan', 'JualMasakan', '{"order_card_id":"tahu_campur","required_ingredient_card_ids":["daging","tahu_tempe"],"income":16}'::jsonb),
      ('MAHIR', 'mahir-risk-007', 7, 1, 8, 1, 'PLAYER', 'RisikoKehidupan', 'RisikoKehidupan', '{"risk_id":"risk_sekolah_adik","note":"Bantu biaya sekolah adik"}'::jsonb),
      ('MAHIR', null, 7, 2, 8, 1, 'PLAYER', 'PinjamanSyariah', 'PinjamanSyariah', '{"loan_id":"loan-seed-mahir-2-004","principal":10,"installment":2,"duration_turn":5,"penalty_points":15}'::jsonb),
      ('MAHIR', null, 7, 3, 8, 1, 'PLAYER', 'Kebutuhan', 'Kebutuhan', '{"card_id":"sepatu","amount":4,"points":2}'::jsonb),
      ('MAHIR', null, 7, 4, 8, 2, 'PLAYER', 'Kebutuhan', 'Kebutuhan', '{"card_id":"sepatu","amount":4,"points":2}'::jsonb),
      ('MAHIR', null, 7, 5, 8, 2, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"sayur","ingredient_name":"Sayur","amount":2}'::jsonb),
      ('MAHIR', null, 7, 6, 8, 3, 'PLAYER', 'Asuransi', 'Asuransi', '{"policy_id":"INS-SEED-003","premium":1,"coverage_type":"MULTIRISK"}'::jsonb),
      ('MAHIR', null, 7, 7, 8, 3, 'PLAYER', 'Kebutuhan', 'Kebutuhan', '{"card_id":"sepatu","amount":4,"points":2}'::jsonb),
      ('MAHIR', null, 7, 8, 8, 4, 'PLAYER', 'JualMasakan', 'JualMasakan', '{"order_card_id":"nasi_goreng","required_ingredient_card_ids":["nasi_putih","telur"],"income":15}'::jsonb),
      ('MAHIR', 'mahir-risk-008', 7, 9, 8, 4, 'PLAYER', 'RisikoKehidupan', 'RisikoKehidupan', '{"risk_id":"risk_motor_bocor","note":"Servis motor darurat"}'::jsonb),
      ('MAHIR', null, 7, 10, 8, 4, 'PLAYER', 'Asuransi', 'Asuransi', '{"risk_event_ref":"mahir-risk-008"}'::jsonb),
      ('MAHIR', null, 7, 11, 8, 4, 'PLAYER', 'Kebutuhan', 'Kebutuhan', '{"card_id":"sepatu","amount":4,"points":2}'::jsonb),
      ('MAHIR', null, 8, 0, 9, 1, 'PLAYER', 'Menabung', 'Menabung', '{"goal_id":"beli_rumah","amount":10}'::jsonb),
      ('MAHIR', null, 8, 1, 9, 1, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"nasi_putih","ingredient_name":"Nasi Putih","amount":1}'::jsonb),
      ('MAHIR', null, 8, 2, 9, 2, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"tahu_tempe","ingredient_name":"Tahu Tempe","amount":3}'::jsonb),
      ('MAHIR', null, 8, 3, 9, 2, 'PLAYER', 'JualMasakan', 'JualMasakan', '{"order_card_id":"resep-sayur-bumbu","required_ingredient_card_ids":["sayur","tahu_tempe"],"income":12}'::jsonb),
      ('MAHIR', 'mahir-risk-009', 8, 4, 9, 2, 'PLAYER', 'RisikoKehidupan', 'RisikoKehidupan', '{"risk_id":"risk_uang_saku_hilang","note":"Kehilangan uang saku"}'::jsonb),
      ('MAHIR', null, 8, 5, 9, 2, 'PLAYER', 'PinjamanSyariah', 'PinjamanSyariah', '{"loan_id":"loan-seed-mahir-2-005","principal":10,"installment":2,"duration_turn":5,"penalty_points":15}'::jsonb),
      ('MAHIR', null, 8, 6, 9, 2, 'PLAYER', 'Asuransi', 'Asuransi', '{"policy_id":"INS-SEED-004","premium":1,"coverage_type":"MULTIRISK"}'::jsonb),
      ('MAHIR', null, 8, 7, 9, 3, 'PLAYER', 'Menabung', 'Menabung', '{"goal_id":"beli_rumah","amount":10}'::jsonb),
      ('MAHIR', null, 8, 8, 9, 3, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"nasi_putih","ingredient_name":"Nasi Putih","amount":1}'::jsonb),
      ('MAHIR', null, 8, 9, 9, 4, 'PLAYER', 'Menabung', 'Menabung', '{"goal_id":"beli_motor","amount":10}'::jsonb),
      ('MAHIR', null, 8, 10, 9, 4, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"daging","ingredient_name":"Daging","amount":5}'::jsonb),
      ('MAHIR', null, 9, 0, 10, 1, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"telur","ingredient_name":"Telur","amount":4}'::jsonb),
      ('MAHIR', null, 9, 1, 10, 1, 'PLAYER', 'JualMasakan', 'JualMasakan', '{"order_card_id":"nasi_goreng","required_ingredient_card_ids":["nasi_putih","telur"],"income":15}'::jsonb),
      ('MAHIR', 'mahir-risk-010', 9, 2, 10, 1, 'PLAYER', 'RisikoKehidupan', 'RisikoKehidupan', '{"risk_id":"risk_tagihan_medis","note":"Tagihan medis ringan"}'::jsonb),
      ('MAHIR', null, 9, 3, 10, 1, 'PLAYER', 'Asuransi', 'Asuransi', '{"risk_event_ref":"mahir-risk-010"}'::jsonb),
      ('MAHIR', null, 9, 4, 10, 2, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"nasi_putih","ingredient_name":"Nasi Putih","amount":1}'::jsonb),
      ('MAHIR', null, 9, 5, 10, 2, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"telur","ingredient_name":"Telur","amount":4}'::jsonb),
      ('MAHIR', null, 9, 6, 10, 3, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"telur","ingredient_name":"Telur","amount":4}'::jsonb),
      ('MAHIR', null, 9, 7, 10, 3, 'PLAYER', 'JualMasakan', 'JualMasakan', '{"order_card_id":"nasi_goreng","required_ingredient_card_ids":["nasi_putih","telur"],"income":15}'::jsonb),
      ('MAHIR', 'mahir-risk-011', 9, 8, 10, 3, 'PLAYER', 'RisikoKehidupan', 'RisikoKehidupan', '{"risk_id":"risk_biaya_perjalanan","note":"Biaya perjalanan mendadak"}'::jsonb),
      ('MAHIR', null, 9, 9, 10, 3, 'PLAYER', 'PinjamanSyariah', 'PinjamanSyariah', '{"loan_id":"loan-seed-mahir-2-006","principal":10,"installment":2,"duration_turn":5,"penalty_points":15}'::jsonb),
      ('MAHIR', null, 9, 10, 10, 4, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"tahu_tempe","ingredient_name":"Tahu Tempe","amount":3}'::jsonb),
      ('MAHIR', null, 9, 11, 10, 4, 'PLAYER', 'JualMasakan', 'JualMasakan', '{"order_card_id":"tahu_campur","required_ingredient_card_ids":["daging","tahu_tempe"],"income":16}'::jsonb),
      ('MAHIR', 'mahir-risk-012', 9, 12, 10, 4, 'PLAYER', 'RisikoKehidupan', 'RisikoKehidupan', '{"risk_id":"risk_peralatan_rusak","note":"Peralatan rumah rusak"}'::jsonb),
      ('MAHIR', null, 9, 13, 10, 4, 'PLAYER', 'PinjamanSyariah', 'PinjamanSyariah', '{"loan_id":"loan-seed-mahir-2-007","principal":10,"installment":2,"duration_turn":5,"penalty_points":15}'::jsonb),
      ('MAHIR', null, 10, 0, 11, 1, 'PLAYER', 'BayarPinjaman', 'BayarPinjaman', '{"loan_id":"loan-seed-mahir-2-004","amount":10}'::jsonb),
      ('MAHIR', null, 10, 1, 11, 1, 'PLAYER', 'Asuransi', 'Asuransi', '{"policy_id":"INS-SEED-005","premium":1,"coverage_type":"MULTIRISK"}'::jsonb),
      ('MAHIR', null, 10, 2, 11, 2, 'PLAYER', 'BayarPinjaman', 'BayarPinjaman', '{"loan_id":"loan-seed-mahir-2-005","amount":10}'::jsonb),
      ('MAHIR', null, 10, 3, 11, 2, 'PLAYER', 'Menabung', 'Menabung', '{"goal_id":"beli_motor","amount":5}'::jsonb),
      ('MAHIR', null, 10, 4, 11, 3, 'PLAYER', 'BayarPinjaman', 'BayarPinjaman', '{"loan_id":"loan-seed-mahir-2-006","amount":10}'::jsonb),
      ('MAHIR', null, 10, 5, 11, 3, 'PLAYER', 'Asuransi', 'Asuransi', '{"policy_id":"INS-SEED-006","premium":1,"coverage_type":"MULTIRISK"}'::jsonb),
      ('MAHIR', null, 10, 6, 11, 4, 'PLAYER', 'BayarPinjaman', 'BayarPinjaman', '{"loan_id":"loan-seed-mahir-2-007","amount":10}'::jsonb),
      ('MAHIR', null, 10, 7, 11, 4, 'PLAYER', 'Asuransi', 'Asuransi', '{"policy_id":"INS-SEED-007","premium":1,"coverage_type":"MULTIRISK"}'::jsonb),
      ('MAHIR', null, 11, 0, 12, 1, 'PLAYER', 'JumatBerkah', 'JumatBerkah', '{"amount":4}'::jsonb),
      ('MAHIR', null, 11, 1, 12, 2, 'PLAYER', 'JumatBerkah', 'JumatBerkah', '{"amount":2}'::jsonb),
      ('MAHIR', null, 11, 2, 12, 3, 'PLAYER', 'JumatBerkah', 'JumatBerkah', '{"amount":5}'::jsonb),
      ('MAHIR', null, 11, 3, 12, 4, 'PLAYER', 'JumatBerkah', 'JumatBerkah', '{"amount":3}'::jsonb),
      ('MAHIR', null, 11, 4, 12, 3, 'SYSTEM', 'PoinPeringkatDonasi', 'PoinPeringkatDonasi', '{"rank":1,"points":7}'::jsonb),
      ('MAHIR', null, 11, 5, 12, 1, 'SYSTEM', 'PoinPeringkatDonasi', 'PoinPeringkatDonasi', '{"rank":2,"points":5}'::jsonb),
      ('MAHIR', null, 11, 6, 12, 4, 'SYSTEM', 'PoinPeringkatDonasi', 'PoinPeringkatDonasi', '{"rank":3,"points":2}'::jsonb),
      ('MAHIR', null, 11, 7, 12, null, 'SYSTEM', 'UmumkanJuaraDonasi', 'UmumkanJuaraDonasi', '{"summary":"Hugo Juara 1, Marco Juara 2, Manalu Juara 3","winners":[{"rank":1,"player_name":"Hugo","player_order_no":3,"points":7},{"rank":2,"player_name":"Marco","player_order_no":1,"points":5},{"rank":3,"player_name":"Manalu","player_order_no":4,"points":2}]}'::jsonb),
      ('MAHIR', null, 12, 0, 13, 1, 'PLAYER', 'JualEmas', 'JualEmas', '{"trade_type":"SELL","unit_price":8,"qty":1,"amount":8}'::jsonb),
      ('MAHIR', null, 12, 1, 13, 2, 'PLAYER', 'InvestasiEmas', 'InvestasiEmas', '{"trade_type":"BUY","unit_price":8,"qty":1,"amount":8}'::jsonb),
      ('MAHIR', null, 12, 2, 13, 3, 'PLAYER', 'InvestasiEmas', 'InvestasiEmas', '{"trade_type":"BUY","unit_price":8,"qty":1,"amount":8}'::jsonb),
      ('MAHIR', null, 12, 3, 13, 4, 'PLAYER', 'JualEmas', 'JualEmas', '{"trade_type":"SELL","unit_price":8,"qty":1,"amount":8}'::jsonb),
      ('MAHIR', null, 13, 0, 14, null, 'SYSTEM', null, 'HariMingguLibur', '{"note":"Hari Minggu libur"}'::jsonb),
      ('MAHIR', null, 14, 0, 15, 1, 'PLAYER', 'Kebutuhan', 'Kebutuhan', '{"card_id":"boneka","amount":6,"points":3}'::jsonb),
      ('MAHIR', null, 14, 1, 15, 1, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"sayur","ingredient_name":"Sayur","amount":2}'::jsonb),
      ('MAHIR', null, 14, 2, 15, 2, 'PLAYER', 'Kebutuhan', 'Kebutuhan', '{"card_id":"gameboy","amount":8,"points":4}'::jsonb),
      ('MAHIR', null, 14, 3, 15, 2, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"tahu_tempe","ingredient_name":"Tahu Tempe","amount":3}'::jsonb),
      ('MAHIR', null, 14, 4, 15, 3, 'PLAYER', 'Kebutuhan', 'Kebutuhan', '{"card_id":"hiburan","amount":5,"points":2}'::jsonb),
      ('MAHIR', null, 14, 5, 15, 3, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"daging","ingredient_name":"Daging","amount":5}'::jsonb),
      ('MAHIR', null, 14, 6, 15, 4, 'PLAYER', 'Kebutuhan', 'Kebutuhan', '{"card_id":"boneka","amount":6,"points":3}'::jsonb),
      ('MAHIR', null, 14, 7, 15, 4, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"nasi_putih","ingredient_name":"Nasi Putih","amount":1}'::jsonb),
      ('MAHIR', null, 15, 0, 16, 1, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"tahu_tempe","ingredient_name":"Tahu Tempe","amount":3}'::jsonb),
      ('MAHIR', null, 15, 1, 16, 1, 'PLAYER', 'JualMasakan', 'JualMasakan', '{"order_card_id":"resep-sayur-bumbu","required_ingredient_card_ids":["sayur","tahu_tempe"],"income":12}'::jsonb),
      ('MAHIR', 'mahir-risk-013', 15, 2, 16, 1, 'PLAYER', 'RisikoKehidupan', 'RisikoKehidupan', '{"risk_id":"risk_kebutuhan_keluarga","note":"Bantu kebutuhan keluarga"}'::jsonb),
      ('MAHIR', null, 15, 3, 16, 1, 'PLAYER', 'KerjaLepas', 'KerjaLepas', '{"amount":1,"note":"Pengganti pinjaman yang tidak tersedia di stok"}'::jsonb),
      ('MAHIR', null, 15, 4, 16, 2, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"daging","ingredient_name":"Daging","amount":5}'::jsonb),
      ('MAHIR', null, 15, 5, 16, 2, 'PLAYER', 'JualMasakan', 'JualMasakan', '{"order_card_id":"tahu_campur","required_ingredient_card_ids":["daging","tahu_tempe"],"income":16}'::jsonb),
      ('MAHIR', 'mahir-risk-014', 15, 6, 16, 2, 'PLAYER', 'RisikoKehidupan', 'RisikoKehidupan', '{"risk_id":"risk_servis_sepeda","note":"Servis sepeda mendadak"}'::jsonb),
      ('MAHIR', null, 15, 7, 16, 2, 'PLAYER', 'Asuransi', 'Asuransi', '{"risk_event_ref":"mahir-risk-014"}'::jsonb),
      ('MAHIR', null, 15, 8, 16, 3, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"tahu_tempe","ingredient_name":"Tahu Tempe","amount":3}'::jsonb),
      ('MAHIR', null, 15, 9, 16, 3, 'PLAYER', 'JualMasakan', 'JualMasakan', '{"order_card_id":"tahu_campur","required_ingredient_card_ids":["daging","tahu_tempe"],"income":16}'::jsonb),
      ('MAHIR', 'mahir-risk-015', 15, 10, 16, 3, 'PLAYER', 'RisikoKehidupan', 'RisikoKehidupan', '{"risk_id":"risk_biaya_listrik","note":"Tagihan listrik membesar"}'::jsonb),
      ('MAHIR', null, 15, 11, 16, 3, 'PLAYER', 'KerjaLepas', 'KerjaLepas', '{"amount":1,"note":"Pengganti pinjaman yang tidak tersedia di stok"}'::jsonb),
      ('MAHIR', null, 15, 12, 16, 4, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"telur","ingredient_name":"Telur","amount":4}'::jsonb),
      ('MAHIR', null, 15, 13, 16, 4, 'PLAYER', 'JualMasakan', 'JualMasakan', '{"order_card_id":"nasi_goreng","required_ingredient_card_ids":["nasi_putih","telur"],"income":15}'::jsonb),
      ('MAHIR', 'mahir-risk-016', 15, 14, 16, 4, 'PLAYER', 'RisikoKehidupan', 'RisikoKehidupan', '{"risk_id":"risk_biaya_kesehatan","note":"Biaya kesehatan keluarga"}'::jsonb),
      ('MAHIR', null, 15, 15, 16, 4, 'PLAYER', 'Asuransi', 'Asuransi', '{"risk_event_ref":"mahir-risk-016"}'::jsonb),
      ('MAHIR', null, 16, 0, 17, 1, 'PLAYER', 'Menabung', 'Menabung', '{"goal_id":"beli_rumah","amount":10}'::jsonb),
      ('MAHIR', null, 16, 1, 17, 1, 'PLAYER', 'TujuanFinansial', 'TujuanFinansial', '{"goal_id":"beli_rumah","cost":12,"points":6}'::jsonb),
      ('MAHIR', null, 16, 2, 17, 2, 'PLAYER', 'Menabung', 'Menabung', '{"goal_id":"beli_motor","amount":10}'::jsonb),
      ('MAHIR', null, 16, 3, 17, 2, 'PLAYER', 'TujuanFinansial', 'TujuanFinansial', '{"goal_id":"beli_motor","cost":8,"points":4}'::jsonb),
      ('MAHIR', null, 16, 4, 17, 3, 'PLAYER', 'Menabung', 'Menabung', '{"goal_id":"beli_rumah","amount":10}'::jsonb),
      ('MAHIR', null, 16, 5, 17, 3, 'PLAYER', 'TujuanFinansial', 'TujuanFinansial', '{"goal_id":"beli_rumah","cost":12,"points":6}'::jsonb),
      ('MAHIR', null, 16, 6, 17, 4, 'PLAYER', 'Menabung', 'Menabung', '{"goal_id":"beli_motor","amount":10}'::jsonb),
      ('MAHIR', null, 16, 7, 17, 4, 'PLAYER', 'TujuanFinansial', 'TujuanFinansial', '{"goal_id":"beli_motor","cost":8,"points":4}'::jsonb),
      ('MAHIR', null, 17, 0, 18, 1, 'PLAYER', 'Asuransi', 'Asuransi', '{"policy_id":"INS-SEED-008","premium":1,"coverage_type":"MULTIRISK"}'::jsonb),
      ('MAHIR', null, 17, 1, 18, 1, 'PLAYER', 'KerjaLepas', 'KerjaLepas', '{"amount":1}'::jsonb),
      ('MAHIR', null, 17, 2, 18, 2, 'PLAYER', 'Asuransi', 'Asuransi', '{"policy_id":"INS-SEED-009","premium":1,"coverage_type":"MULTIRISK"}'::jsonb),
      ('MAHIR', null, 17, 3, 18, 2, 'PLAYER', 'KerjaLepas', 'KerjaLepas', '{"amount":1}'::jsonb),
      ('MAHIR', null, 17, 4, 18, 3, 'PLAYER', 'Asuransi', 'Asuransi', '{"policy_id":"INS-SEED-010","premium":1,"coverage_type":"MULTIRISK"}'::jsonb),
      ('MAHIR', null, 17, 5, 18, 3, 'PLAYER', 'KerjaLepas', 'KerjaLepas', '{"amount":1}'::jsonb),
      ('MAHIR', null, 17, 6, 18, 4, 'PLAYER', 'Asuransi', 'Asuransi', '{"policy_id":"INS-SEED-011","premium":1,"coverage_type":"MULTIRISK"}'::jsonb),
      ('MAHIR', null, 17, 7, 18, 4, 'PLAYER', 'KerjaLepas', 'KerjaLepas', '{"amount":1}'::jsonb),
      ('MAHIR', null, 18, 0, 19, 1, 'PLAYER', 'JumatBerkah', 'JumatBerkah', '{"amount":2}'::jsonb),
      ('MAHIR', null, 18, 1, 19, 2, 'PLAYER', 'JumatBerkah', 'JumatBerkah', '{"amount":4}'::jsonb),
      ('MAHIR', null, 18, 2, 19, 3, 'PLAYER', 'JumatBerkah', 'JumatBerkah', '{"amount":3}'::jsonb),
      ('MAHIR', null, 18, 3, 19, 4, 'PLAYER', 'JumatBerkah', 'JumatBerkah', '{"amount":5}'::jsonb),
      ('MAHIR', null, 18, 4, 19, 4, 'SYSTEM', 'PoinPeringkatDonasi', 'PoinPeringkatDonasi', '{"rank":1,"points":7}'::jsonb),
      ('MAHIR', null, 18, 5, 19, 2, 'SYSTEM', 'PoinPeringkatDonasi', 'PoinPeringkatDonasi', '{"rank":2,"points":5}'::jsonb),
      ('MAHIR', null, 18, 6, 19, 3, 'SYSTEM', 'PoinPeringkatDonasi', 'PoinPeringkatDonasi', '{"rank":3,"points":2}'::jsonb),
      ('MAHIR', null, 18, 7, 19, null, 'SYSTEM', 'UmumkanJuaraDonasi', 'UmumkanJuaraDonasi', '{"summary":"Manalu Juara 1, Marcello Juara 2, Hugo Juara 3","winners":[{"rank":1,"player_name":"Manalu","player_order_no":4,"points":7},{"rank":2,"player_name":"Marcello","player_order_no":2,"points":5},{"rank":3,"player_name":"Hugo","player_order_no":3,"points":2}]}'::jsonb),
      ('MAHIR', null, 19, 0, 20, 1, 'PLAYER', 'InvestasiEmas', 'InvestasiEmas', '{"trade_type":"BUY","unit_price":7,"qty":1,"amount":7}'::jsonb),
      ('MAHIR', null, 19, 1, 20, 2, 'PLAYER', 'JualEmas', 'JualEmas', '{"trade_type":"SELL","unit_price":7,"qty":1,"amount":7}'::jsonb),
      ('MAHIR', null, 19, 2, 20, 3, 'PLAYER', 'InvestasiEmas', 'InvestasiEmas', '{"trade_type":"BUY","unit_price":7,"qty":1,"amount":7}'::jsonb),
      ('MAHIR', null, 19, 3, 20, 4, 'PLAYER', null, 'LewatiTransaksiEmas', '{"note":"Tidak membeli dan tidak menjual emas"}'::jsonb),
      ('MAHIR', null, 20, 0, 21, null, 'SYSTEM', null, 'HariMingguLibur', '{"note":"Hari Minggu libur"}'::jsonb),
      ('MAHIR', null, 21, 0, 22, 1, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"nasi_putih","ingredient_name":"Nasi Putih","amount":1}'::jsonb),
      ('MAHIR', null, 21, 1, 22, 1, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"telur","ingredient_name":"Telur","amount":4}'::jsonb),
      ('MAHIR', null, 21, 2, 22, 2, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"sayur","ingredient_name":"Sayur","amount":2}'::jsonb),
      ('MAHIR', null, 21, 3, 22, 2, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"tahu_tempe","ingredient_name":"Tahu Tempe","amount":3}'::jsonb),
      ('MAHIR', null, 21, 4, 22, 3, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"nasi_putih","ingredient_name":"Nasi Putih","amount":1}'::jsonb),
      ('MAHIR', null, 21, 5, 22, 3, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"telur","ingredient_name":"Telur","amount":4}'::jsonb),
      ('MAHIR', null, 21, 6, 22, 4, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"daging","ingredient_name":"Daging","amount":5}'::jsonb),
      ('MAHIR', null, 21, 7, 22, 4, 'PLAYER', 'BahanMasakan', 'BahanMasakan', '{"card_id":"tahu_tempe","ingredient_name":"Tahu Tempe","amount":3}'::jsonb),
      ('MAHIR', null, 22, 0, 23, 1, 'PLAYER', 'JualMasakan', 'JualMasakan', '{"order_card_id":"nasi_goreng","required_ingredient_card_ids":["nasi_putih","telur"],"income":15}'::jsonb),
      ('MAHIR', 'mahir-risk-017', 22, 1, 23, 1, 'PLAYER', 'RisikoKehidupan', 'RisikoKehidupan', '{"risk_id":"risk_uang_kas","note":"Uang kas keluarga berkurang"}'::jsonb),
      ('MAHIR', null, 22, 2, 23, 1, 'PLAYER', 'Asuransi', 'Asuransi', '{"risk_event_ref":"mahir-risk-017"}'::jsonb),
      ('MAHIR', null, 22, 3, 23, 1, 'PLAYER', 'KerjaLepas', 'KerjaLepas', '{"amount":1}'::jsonb),
      ('MAHIR', null, 22, 4, 23, 2, 'PLAYER', 'JualMasakan', 'JualMasakan', '{"order_card_id":"resep-sayur-bumbu","required_ingredient_card_ids":["sayur","tahu_tempe"],"income":12}'::jsonb),
      ('MAHIR', 'mahir-risk-018', 22, 5, 23, 2, 'PLAYER', 'RisikoKehidupan', 'RisikoKehidupan', '{"risk_id":"risk_biaya_transport","note":"Biaya transport bertambah"}'::jsonb),
      ('MAHIR', null, 22, 6, 23, 2, 'PLAYER', 'KerjaLepas', 'KerjaLepas', '{"amount":1,"note":"Pengganti pinjaman yang tidak tersedia di stok"}'::jsonb),
      ('MAHIR', null, 22, 7, 23, 2, 'PLAYER', 'KerjaLepas', 'KerjaLepas', '{"amount":1}'::jsonb),
      ('MAHIR', null, 22, 8, 23, 3, 'PLAYER', 'JualMasakan', 'JualMasakan', '{"order_card_id":"nasi_goreng","required_ingredient_card_ids":["nasi_putih","telur"],"income":15}'::jsonb),
      ('MAHIR', 'mahir-risk-019', 22, 9, 23, 3, 'PLAYER', 'RisikoKehidupan', 'RisikoKehidupan', '{"risk_id":"risk_tagihan_air","note":"Tagihan air meningkat"}'::jsonb),
      ('MAHIR', null, 22, 10, 23, 3, 'PLAYER', 'Asuransi', 'Asuransi', '{"risk_event_ref":"mahir-risk-019"}'::jsonb),
      ('MAHIR', null, 22, 11, 23, 3, 'PLAYER', 'KerjaLepas', 'KerjaLepas', '{"amount":1}'::jsonb),
      ('MAHIR', null, 22, 12, 23, 4, 'PLAYER', 'JualMasakan', 'JualMasakan', '{"order_card_id":"tahu_campur","required_ingredient_card_ids":["daging","tahu_tempe"],"income":16}'::jsonb),
      ('MAHIR', 'mahir-risk-020', 22, 13, 23, 4, 'PLAYER', 'RisikoKehidupan', 'RisikoKehidupan', '{"risk_id":"risk_perbaikan_atap","note":"Perbaikan atap rumah"}'::jsonb),
      ('MAHIR', null, 22, 14, 23, 4, 'PLAYER', 'KerjaLepas', 'KerjaLepas', '{"amount":1,"note":"Pengganti pinjaman yang tidak tersedia di stok"}'::jsonb),
      ('MAHIR', null, 22, 15, 23, 4, 'PLAYER', 'KerjaLepas', 'KerjaLepas', '{"amount":1}'::jsonb),
      ('MAHIR', null, 23, 0, 24, 1, 'PLAYER', 'Kebutuhan', 'Kebutuhan', '{"card_id":"buku","amount":2,"points":1}'::jsonb),
      ('MAHIR', null, 23, 1, 24, 1, 'PLAYER', 'Kebutuhan', 'Kebutuhan', '{"card_id":"sepatu","amount":4,"points":2}'::jsonb),
      ('MAHIR', null, 23, 2, 24, 2, 'PLAYER', 'Kebutuhan', 'Kebutuhan', '{"card_id":"buku","amount":2,"points":1}'::jsonb),
      ('MAHIR', null, 23, 3, 24, 2, 'PLAYER', 'Kebutuhan', 'Kebutuhan', '{"card_id":"sepatu","amount":4,"points":2}'::jsonb),
      ('MAHIR', null, 23, 4, 24, 3, 'PLAYER', 'Kebutuhan', 'Kebutuhan', '{"card_id":"buku","amount":2,"points":1}'::jsonb),
      ('MAHIR', null, 23, 5, 24, 3, 'PLAYER', 'Kebutuhan', 'Kebutuhan', '{"card_id":"sepatu","amount":4,"points":2}'::jsonb),
      ('MAHIR', null, 23, 6, 24, 4, 'PLAYER', 'Kebutuhan', 'Kebutuhan', '{"card_id":"buku","amount":2,"points":1}'::jsonb),
      ('MAHIR', null, 23, 7, 24, 4, 'PLAYER', 'Kebutuhan', 'Kebutuhan', '{"card_id":"sepatu","amount":4,"points":2}'::jsonb),
      ('MAHIR', null, 24, 0, 25, 1, 'PLAYER', 'KerjaLepas', 'KerjaLepas', '{"amount":1,"note":"Tidak ada cicilan karena pinjaman tidak diambil"}'::jsonb),
      ('MAHIR', null, 24, 1, 25, 1, 'PLAYER', 'KerjaLepas', 'KerjaLepas', '{"amount":1}'::jsonb),
      ('MAHIR', null, 24, 2, 25, 2, 'PLAYER', 'KerjaLepas', 'KerjaLepas', '{"amount":1,"note":"Tidak ada cicilan karena pinjaman tidak diambil"}'::jsonb),
      ('MAHIR', null, 24, 3, 25, 2, 'PLAYER', 'KerjaLepas', 'KerjaLepas', '{"amount":1}'::jsonb),
      ('MAHIR', null, 24, 4, 25, 3, 'PLAYER', 'KerjaLepas', 'KerjaLepas', '{"amount":1,"note":"Tidak ada cicilan karena pinjaman tidak diambil"}'::jsonb),
      ('MAHIR', null, 24, 5, 25, 3, 'PLAYER', 'KerjaLepas', 'KerjaLepas', '{"amount":1}'::jsonb),
      ('MAHIR', null, 24, 6, 25, 4, 'PLAYER', 'KerjaLepas', 'KerjaLepas', '{"amount":1,"note":"Tidak ada cicilan karena pinjaman tidak diambil"}'::jsonb),
      ('MAHIR', null, 24, 7, 25, 4, 'PLAYER', 'KerjaLepas', 'KerjaLepas', '{"amount":1}'::jsonb),
      ('MAHIR', null, 24, 8, 25, null, 'SYSTEM', 'AkhiriSesi', 'AkhiriSesi', '{"end_note":"Selesai sesi mahir sesuai dokumen skenario"}'::jsonb)
  ) as x(session_key, ref_key, day_index, event_order, action_slot, player_no, actor_type, action_id, action_type, payload)
),
transition_seed as (
  select
    session_key,
    null::text as ref_key,
    day_index,
    max(event_order) + 1 as event_order,
    0 as action_slot,
    null::int as player_no,
    'SYSTEM'::text as actor_type,
    'AkhirGiliran'::text as action_id,
    'AkhirGiliran'::text as action_type,
    jsonb_build_object(
      'from_day', day_index + 1,
      'to_day', day_index + 2,
      'completed_players', 4,
      'used', count(*) filter (
        where actor_type = 'PLAYER'
          and action_type <> 'RisikoKehidupan'
          and not (action_type = 'Asuransi' and payload ? 'risk_event_ref')
      ),
      'remaining', 0
    ) as payload
  from scenario_event_seed
  where day_index between 0 and 23
  group by session_key, day_index
),
event_seed as (
  select *
  from scenario_event_seed
  union all
  select *
  from transition_seed
),
ordered_events as (
  select
    sc.session_id,
    sc.ruleset_version_id,
    sc.event_uuid_prefix,
    sc.base_timestamp,
    es.session_key,
    es.ref_key,
    es.day_index,
    es.event_order,
    case
      when es.actor_type = 'PLAYER' and es.player_no is not null then
        least(2, row_number() over (
          partition by
            es.session_key,
            es.day_index,
            case when es.actor_type = 'PLAYER' then es.player_no end
          order by es.event_order, es.action_type
        ))::int
      else 0
    end as action_slot,
    es.actor_type,
    coalesce(es.action_id, es.action_type) as action_id,
    coalesce(es.action_id, es.action_type) as action_type,
    case
      when es.actor_type = 'PLAYER' then coalesce(es.player_no, 0)
      else 0
    end::int as turn_number,
    es.payload,
    pc.session_player_id,
    pc.user_id,
    row_number() over (
      partition by es.session_key
      order by es.day_index, es.event_order, es.action_type, coalesce(es.player_no, 0)
    ) as event_number
  from event_seed es
  join session_context sc on sc.session_key = es.session_key
  left join player_context pc
    on pc.session_key = es.session_key
   and pc.player_no = es.player_no
),
numbered_events as (
  select
    *,
    event_number - 1 as sequence_number,
    (event_uuid_prefix || lpad(event_number::text, 12, '0'))::uuid as event_id,
    day_index + 1 as stored_day_index,
    base_timestamp + (day_index * interval '1 day') + (event_order * interval '1 minute') as event_timestamp,
    case day_index % 7
      when 0 then 'MON'
      when 1 then 'TUE'
      when 2 then 'WED'
      when 3 then 'THU'
      when 4 then 'FRI'
      when 5 then 'SAT'
      else 'SUN'
    end as weekday
  from ordered_events
),
resolved_events as (
  select
    e.event_id,
    e.session_id,
    e.session_player_id,
    e.user_id,
    e.actor_type,
    e.event_timestamp,
    e.stored_day_index as day_index,
    e.weekday,
    e.turn_number,
    e.action_slot,
    e.sequence_number,
    e.action_id,
    e.action_type,
    e.ruleset_version_id,
    case
      when e.action_type = 'Asuransi' and ((e.payload::jsonb ? 'risk_event_ref') or (e.payload::jsonb ? 'risk_event_id')) then
        jsonb_build_object('risk_event_id', risk_event.event_id::text)
      when e.action_type = 'Kebutuhan' then
        e.payload::jsonb || jsonb_build_object('need_tier', rn.need_tier)
      else e.payload
    end as payload
  from numbered_events e
  left join numbered_events risk_event
    on risk_event.session_key = e.session_key
   and risk_event.ref_key = (e.payload::jsonb)->>'risk_event_ref'
  left join ruleset_needs rn
    on rn.ruleset_version_id = e.ruleset_version_id
   and lower(rn.need_code) = lower((e.payload::jsonb)->>'card_id')
)
insert into events (
  event_pk,
  event_id,
  session_id,
  session_player_id,
  user_id,
  actor_type,
  timestamp,
  day_index,
  weekday,
  turn_number,
  action_slot,
  sequence_number,
  ruleset_action_id,
  action_type,
  ruleset_version_id,
  payload,
  received_at,
  client_request_id
)
select
  re.event_id,
  re.event_id,
  re.session_id,
  re.session_player_id,
  re.user_id,
  re.actor_type,
  re.event_timestamp,
  re.day_index,
  re.weekday,
  re.turn_number,
  re.action_slot,
  re.sequence_number,
  ra.ruleset_action_id,
  re.action_type,
  re.ruleset_version_id,
  re.payload::jsonb,
  re.event_timestamp,
  null
from resolved_events re
join ruleset_actions ra
  on ra.ruleset_version_id = re.ruleset_version_id
 and lower(ra.action_id) = lower(coalesce(re.action_id, re.action_type))
 and ra.is_active
order by re.session_id, re.sequence_number;

insert into session_projection_checkpoints (
  session_id,
  last_sequence_number,
  last_event_id,
  projected_at,
  status,
  rebuild_started_at,
  rebuild_completed_at,
  metadata_json
)
select
  e.session_id,
  max(e.sequence_number),
  (array_agg(e.event_id order by e.sequence_number desc))[1],
  now(),
  'IDLE',
  now(),
  now(),
  jsonb_build_object(
    'source', '02_seed_simulation_sessions_events',
    'projected_events', count(*)
  )
from events e
where e.session_id in (
    '91000000-0000-0000-0000-000000000001'::uuid,
    '91000000-0000-0000-0000-000000000002'::uuid
  )
group by e.session_id
on conflict (session_id) do update
set last_sequence_number = excluded.last_sequence_number,
    last_event_id = excluded.last_event_id,
    projected_at = excluded.projected_at,
    status = excluded.status,
    rebuild_started_at = excluded.rebuild_started_at,
    rebuild_completed_at = excluded.rebuild_completed_at,
    error_message = null,
    metadata_json = excluded.metadata_json;

with cashflow_candidates as (
  select
    e.event_pk,
    e.event_id,
    e.session_id,
    e.user_id,
    e.timestamp,
    e.sequence_number,
    case
      when e.action_type = 'CatatTransaksi' then upper(nullif((e.payload::jsonb)->>'direction', ''))
      when e.action_type = 'JumatBerkah' then 'OUT'
      when e.action_type = 'InvestasiEmas' then 'OUT'
      when e.action_type = 'JualEmas' then 'IN'
      when e.action_type = 'BahanMasakan' then 'OUT'
      when e.action_type = 'JualMasakan' then 'IN'
      when e.action_type = 'KerjaLepas' then 'IN'
      when e.action_type = 'Kebutuhan' then 'OUT'
      when e.action_type = 'Menabung' then 'OUT'
      when e.action_type = 'TarikTabungan' then 'IN'
      when e.action_type = 'RisikoKehidupan' and risk_catalog.direction = 'OUT' and insurance_used.event_pk is not null then null
      when e.action_type = 'RisikoKehidupan' then risk_catalog.direction
      when e.action_type = 'PinjamanSyariah' then 'IN'
      when e.action_type = 'BayarPinjaman' then 'OUT'
      when e.action_type = 'Asuransi' and e.payload::jsonb ? 'premium' then 'OUT'
      when e.action_type = 'GunakanOpsiDarurat' then upper(nullif((e.payload::jsonb)->>'direction', ''))
      else null
    end as direction,
    case
      when e.action_type = 'PinjamanSyariah' then round(((e.payload::jsonb)->>'principal')::numeric)::int
      when e.action_type = 'JualMasakan' then round(((e.payload::jsonb)->>'income')::numeric)::int
      when e.action_type = 'Asuransi' and e.payload::jsonb ? 'premium' then round(((e.payload::jsonb)->>'premium')::numeric)::int
      when e.action_type = 'RisikoKehidupan' then risk_catalog.amount
      else round(((e.payload::jsonb)->>'amount')::numeric)::int
    end as amount,
    case
      when e.action_type = 'CatatTransaksi' then upper(coalesce(nullif((e.payload::jsonb)->>'category', ''), 'TRANSACTION'))
      when e.action_type = 'JumatBerkah' then 'DONATION'
      when e.action_type in ('InvestasiEmas', 'JualEmas') then 'GOLD_TRADE'
      when e.action_type = 'BahanMasakan' then 'INGREDIENT'
      when e.action_type = 'JualMasakan' then 'ORDER'
      when e.action_type = 'KerjaLepas' then 'FREELANCE'
      when e.action_type = 'Kebutuhan' then case lower((e.payload::jsonb)->>'need_tier')
        when 'primer' then 'NEED_PRIMARY'
        when 'sekunder' then 'NEED_SECONDARY'
        when 'tersier' then 'NEED_TERTIARY'
        else 'NEED'
      end
      when e.action_type = 'Menabung' then 'SAVING_DEPOSIT'
      when e.action_type = 'TarikTabungan' then 'SAVING_WITHDRAW'
      when e.action_type = 'RisikoKehidupan' then 'RISK_LIFE'
      when e.action_type = 'PinjamanSyariah' then 'LOAN_TAKEN'
      when e.action_type = 'BayarPinjaman' then 'LOAN_REPAID'
      when e.action_type = 'Asuransi' and e.payload::jsonb ? 'premium' then 'INSURANCE_PREMIUM'
      when e.action_type = 'GunakanOpsiDarurat' then 'EMERGENCY_OPTION'
      else null
    end as category,
    nullif((e.payload::jsonb)->>'counterparty', '') as counterparty,
    coalesce(
      nullif((e.payload::jsonb)->>'loan_id', ''),
      nullif((e.payload::jsonb)->>'policy_id', ''),
      nullif((e.payload::jsonb)->>'goal_id', ''),
      nullif((e.payload::jsonb)->>'order_card_id', ''),
      nullif((e.payload::jsonb)->>'card_id', ''),
      nullif((e.payload::jsonb)->>'risk_id', ''),
      nullif((e.payload::jsonb)->>'risk_event_id', '')
    ) as reference,
    nullif((e.payload::jsonb)->>'note', '') as note
  from events e
  left join ruleset_life_risks risk_catalog
    on risk_catalog.ruleset_version_id = e.ruleset_version_id
   and lower(risk_catalog.risk_code) = lower(e.payload::jsonb->>'risk_id')
  left join events insurance_used
    on insurance_used.session_id = e.session_id
   and insurance_used.action_type = 'Asuransi'
   and (insurance_used.payload::jsonb->>'risk_event_id')::uuid = e.event_id
  where e.session_id in (
      '91000000-0000-0000-0000-000000000001'::uuid,
      '91000000-0000-0000-0000-000000000002'::uuid
    )
    and e.user_id is not null
    and e.actor_type = 'PLAYER'
),
cashflow_rows as (
  select
    row_number() over (order by session_id, sequence_number) as projection_number,
    event_pk,
    event_id,
    session_id,
    user_id,
    timestamp,
    direction,
    amount,
    category,
    counterparty,
    reference,
    note
  from cashflow_candidates
  where direction in ('IN', 'OUT')
    and amount > 0
    and category is not null
)
insert into event_cashflow_projections (
  projection_id,
  session_id,
  user_id,
  event_pk,
  event_id,
  projection_order,
  timestamp,
  direction,
  amount,
  category,
  counterparty,
  reference,
  note
)
select
  ('94000000-0000-0000-0000-' || lpad(projection_number::text, 12, '0'))::uuid,
  session_id,
  user_id,
  event_pk,
  event_id,
  1,
  timestamp,
  direction,
  amount,
  category,
  counterparty,
  reference,
  note
from cashflow_rows
order by session_id, projection_number
on conflict (session_id, event_id, projection_order) do nothing;

create temporary table seed_participant_asset_projection on commit drop as
with seed_asset_events as (
  select
    e.session_id,
    e.ruleset_version_id,
    e.session_player_id as session_participant_id,
    e.action_type,
    e.payload::jsonb as payload_json,
    e.timestamp as event_timestamp
  from events e
  where e.session_id in (
      '91000000-0000-0000-0000-000000000001'::uuid,
      '91000000-0000-0000-0000-000000000002'::uuid
    )
    and e.session_player_id is not null
),
gold_assets as (
  select
    session_id,
    session_participant_id,
    ruleset_version_id,
    'GOLD'::varchar(20) as asset_type,
    'GOLD_POSITION'::varchar(120) as asset_code,
    greatest(
      coalesce(sum(case when action_type = 'BagikanEmasAwal' then coalesce((payload_json->>'qty')::int, 1) else 0 end), 0)
      + coalesce(sum(case when upper(payload_json->>'trade_type') = 'BUY' then (payload_json->>'qty')::int else 0 end), 0)
      - coalesce(sum(case when upper(payload_json->>'trade_type') = 'SELL' then (payload_json->>'qty')::int else 0 end), 0),
      0
    )::int as quantity,
    greatest(
      coalesce(sum(case when upper(payload_json->>'trade_type') = 'BUY' then (payload_json->>'amount')::int else 0 end), 0)
      - coalesce(sum(case when upper(payload_json->>'trade_type') = 'SELL' then (payload_json->>'amount')::int else 0 end), 0),
      0
    )::int as amount,
    case
      when greatest(
        coalesce(sum(case when action_type = 'BagikanEmasAwal' then coalesce((payload_json->>'qty')::int, 1) else 0 end), 0)
        + coalesce(sum(case when upper(payload_json->>'trade_type') = 'BUY' then (payload_json->>'qty')::int else 0 end), 0)
        - coalesce(sum(case when upper(payload_json->>'trade_type') = 'SELL' then (payload_json->>'qty')::int else 0 end), 0),
        0
      ) > 0 then 'ACTIVE'::varchar(20)
      else 'INACTIVE'::varchar(20)
    end as status,
    jsonb_build_object(
      'initial_qty', coalesce(sum(case when action_type = 'BagikanEmasAwal' then coalesce((payload_json->>'qty')::int, 1) else 0 end), 0),
      'buy_qty', coalesce(sum(case when upper(payload_json->>'trade_type') = 'BUY' then (payload_json->>'qty')::int else 0 end), 0),
      'sell_qty', coalesce(sum(case when upper(payload_json->>'trade_type') = 'SELL' then (payload_json->>'qty')::int else 0 end), 0),
      'buy_amount', coalesce(sum(case when upper(payload_json->>'trade_type') = 'BUY' then (payload_json->>'amount')::int else 0 end), 0),
      'sell_amount', coalesce(sum(case when upper(payload_json->>'trade_type') = 'SELL' then (payload_json->>'amount')::int else 0 end), 0)
    ) as metadata_json,
    max(event_timestamp) as updated_at
  from seed_asset_events
  where action_type = 'BagikanEmasAwal'
     or (
      action_type in ('InvestasiEmas', 'JualEmas')
      and upper(coalesce(payload_json->>'trade_type', '')) in ('BUY', 'SELL')
    )
  group by session_id, ruleset_version_id, session_participant_id
),
loan_taken as (
  select
    session_id,
    ruleset_version_id,
    session_participant_id,
    payload_json->>'loan_id' as loan_id,
    (payload_json->>'principal')::int as principal,
    (payload_json->>'installment')::int as installment,
    (payload_json->>'duration_turn')::int as duration_turn,
    (payload_json->>'penalty_points')::int as penalty_points,
    event_timestamp as taken_at
  from seed_asset_events
  where action_type = 'PinjamanSyariah'
    and nullif(payload_json->>'loan_id', '') is not null
),
loan_repaid as (
  select
    session_id,
    ruleset_version_id,
    session_participant_id,
    payload_json->>'loan_id' as loan_id,
    sum((payload_json->>'amount')::int)::int as repaid_amount,
    max(event_timestamp) as repaid_at
  from seed_asset_events
  where action_type = 'BayarPinjaman'
    and nullif(payload_json->>'loan_id', '') is not null
  group by session_id, ruleset_version_id, session_participant_id, payload_json->>'loan_id'
),
loan_assets as (
  select
    lt.session_id,
    lt.session_participant_id,
    lt.ruleset_version_id,
    'LOAN'::varchar(20) as asset_type,
    lt.loan_id::varchar(120) as asset_code,
    null::int as quantity,
    greatest(lt.principal - coalesce(lr.repaid_amount, 0), 0)::int as amount,
    case
      when greatest(lt.principal - coalesce(lr.repaid_amount, 0), 0) = 0 then 'PAID'::varchar(20)
      else 'ACTIVE'::varchar(20)
    end as status,
    jsonb_build_object(
      'principal', lt.principal,
      'installment', lt.installment,
      'duration_turn', lt.duration_turn,
      'penalty_points', lt.penalty_points,
      'repaid_amount', coalesce(lr.repaid_amount, 0),
      'taken_at', lt.taken_at,
      'repaid_at', lr.repaid_at
    ) as metadata_json,
    greatest(lt.taken_at, coalesce(lr.repaid_at, lt.taken_at)) as updated_at
  from loan_taken lt
  left join loan_repaid lr
    on lr.session_participant_id = lt.session_participant_id
   and lr.session_id = lt.session_id
   and lr.ruleset_version_id = lt.ruleset_version_id
   and lr.loan_id = lt.loan_id
),
insurance_usage as (
  select
    session_id,
    ruleset_version_id,
    session_participant_id,
    count(*)::int as used_count,
    max(event_timestamp) as last_used_at
  from seed_asset_events
  where action_type = 'Asuransi'
    and ((payload_json ? 'risk_event_id') or (payload_json ? 'risk_event_ref'))
  group by session_id, ruleset_version_id, session_participant_id
),
insurance_assets as (
  select
    e.session_id,
    e.session_participant_id,
    e.ruleset_version_id,
    'INSURANCE'::varchar(20) as asset_type,
    (e.payload_json->>'policy_id')::varchar(120) as asset_code,
    1::int as quantity,
    (e.payload_json->>'premium')::int as amount,
    case
      when coalesce(iu.used_count, 0) > 0 and e.event_timestamp <= iu.last_used_at then 'INACTIVE'::varchar(20)
      else 'ACTIVE'::varchar(20)
    end as status,
    jsonb_build_object(
      'premium', (e.payload_json->>'premium')::int,
      'coverage_type', e.payload_json->>'coverage_type',
      'used_count_on_participant', coalesce(iu.used_count, 0),
      'purchased_at', e.event_timestamp,
      'last_used_at', iu.last_used_at
    ) as metadata_json,
    greatest(e.event_timestamp, coalesce(iu.last_used_at, e.event_timestamp)) as updated_at
  from seed_asset_events e
  left join insurance_usage iu
    on iu.session_participant_id = e.session_participant_id
   and iu.session_id = e.session_id
   and iu.ruleset_version_id = e.ruleset_version_id
  where e.action_type = 'Asuransi'
    and nullif(e.payload_json->>'policy_id', '') is not null
),
asset_rows as (
  select * from gold_assets
  union all
  select * from loan_assets
  union all
  select * from insurance_assets
)
select *
from asset_rows;

insert into session_participant_gold_holdings (
  session_id,
  session_participant_id,
  ruleset_version_id,
  ruleset_game_asset_id,
  quantity,
  created_at,
  updated_at
)
select
  ar.session_id,
  ar.session_participant_id,
  ar.ruleset_version_id,
  rga.ruleset_game_asset_id,
  ar.quantity,
  ar.updated_at,
  ar.updated_at
from seed_participant_asset_projection ar
join ruleset_game_assets rga
  on rga.ruleset_version_id = ar.ruleset_version_id
 and rga.asset_type = 'GOLD'
 and rga.asset_code = 'gold_card_1'
where ar.asset_type = 'GOLD'
on conflict (session_participant_id, ruleset_game_asset_id) do update
set quantity = excluded.quantity,
    updated_at = excluded.updated_at;

insert into session_participant_loans (
  session_id,
  session_participant_id,
  ruleset_version_id,
  ruleset_sharia_loan_id,
  principal_amount,
  outstanding_amount,
  installment_amount,
  status,
  metadata_json,
  created_at,
  updated_at
)
select
  ar.session_id,
  ar.session_participant_id,
  ar.ruleset_version_id,
  rsl.ruleset_sharia_loan_id,
  coalesce((ar.metadata_json->>'principal')::int, ar.amount, 0),
  coalesce(ar.amount, 0),
  nullif(ar.metadata_json->>'installment', '')::int,
  ar.status,
  ar.metadata_json,
  ar.updated_at,
  ar.updated_at
from seed_participant_asset_projection ar
join ruleset_sharia_loans rsl
  on rsl.ruleset_version_id = ar.ruleset_version_id
 and rsl.loan_code = ar.asset_code
where ar.asset_type = 'LOAN'
on conflict (session_participant_id, ruleset_sharia_loan_id) do update
set outstanding_amount = excluded.outstanding_amount,
    installment_amount = excluded.installment_amount,
    status = excluded.status,
    metadata_json = excluded.metadata_json,
    updated_at = excluded.updated_at;

insert into session_participant_insurances (
  session_id,
  session_participant_id,
  ruleset_version_id,
  ruleset_insurance_product_id,
  premium_paid,
  remaining_uses,
  status,
  metadata_json,
  created_at,
  updated_at
)
select
  ar.session_id,
  ar.session_participant_id,
  ar.ruleset_version_id,
  rip.ruleset_insurance_product_id,
  coalesce(ar.amount, 0),
  greatest(coalesce(ar.quantity, 0) - coalesce((ar.metadata_json->>'used_count_on_participant')::int, 0), 0),
  ar.status,
  ar.metadata_json,
  ar.updated_at,
  ar.updated_at
from seed_participant_asset_projection ar
join ruleset_insurance_products rip
  on rip.ruleset_version_id = ar.ruleset_version_id
 and rip.product_code = ar.asset_code
where ar.asset_type = 'INSURANCE'
on conflict (session_participant_id, ruleset_insurance_product_id) do update
set premium_paid = excluded.premium_paid,
    remaining_uses = excluded.remaining_uses,
    status = excluded.status,
    metadata_json = excluded.metadata_json,
    updated_at = excluded.updated_at;

insert into session_states (
  session_id,
  day,
  weekday,
  turn_number,
  action_slot,
  current_session_player_id,
  current_action_slot,
  action_slots_left,
  finish_day,
  phase,
  is_game_over,
  state_version,
  ui_state_json,
  created_at,
  updated_at
)
select
  s.session_id,
  latest.day_index,
  latest.weekday,
  0,
  latest.action_slot,
  null,
  1,
  0,
  rgs.finish_day,
  'GAME_END',
  true,
  greatest(latest.sequence_number + 1, 1),
  jsonb_build_object('source', '02_seed_simulation_sessions_events'),
  s.started_at,
  s.ended_at
from sessions s
join ruleset_game_settings rgs
  on rgs.ruleset_version_id = s.ruleset_version_id
join lateral (
  select e.day_index, e.weekday, e.action_slot, e.sequence_number
  from events e
  where e.session_id = s.session_id
  order by e.sequence_number desc
  limit 1
) latest on true
where s.session_id in (
    '91000000-0000-0000-0000-000000000001'::uuid,
    '91000000-0000-0000-0000-000000000002'::uuid
  )
on conflict (session_id) do update
set day = excluded.day,
    weekday = excluded.weekday,
    turn_number = excluded.turn_number,
    action_slot = excluded.action_slot,
    current_session_player_id = null,
    current_action_slot = excluded.current_action_slot,
    action_slots_left = excluded.action_slots_left,
    finish_day = excluded.finish_day,
    phase = excluded.phase,
    is_game_over = excluded.is_game_over,
    state_version = excluded.state_version,
    ui_state_json = excluded.ui_state_json,
    updated_at = excluded.updated_at;

with participant_cashflow as (
  select
    sp.session_id,
    sp.session_participant_id,
    sp.user_id,
    rgs.starting_cash,
    rgs.starting_happiness,
    rgs.starting_saving,
    coalesce(sum(case when ecp.direction = 'IN' then ecp.amount else -ecp.amount end), 0)::int as net_cashflow
  from session_participants sp
  join sessions s
    on s.session_id = sp.session_id
  join ruleset_game_settings rgs
    on rgs.ruleset_version_id = s.ruleset_version_id
  left join event_cashflow_projections ecp
    on ecp.session_id = sp.session_id
   and ecp.user_id = sp.user_id
  where sp.session_id in (
      '91000000-0000-0000-0000-000000000001'::uuid,
      '91000000-0000-0000-0000-000000000002'::uuid
    )
  group by
    sp.session_id,
    sp.session_participant_id,
    sp.user_id,
    rgs.starting_cash,
    rgs.starting_happiness,
    rgs.starting_saving
),
participant_event_totals as (
  select
    sp.session_id,
    sp.session_participant_id,
    coalesce(sum(case
      when e.action_type = 'Kebutuhan'
        then coalesce((e.payload->>'points')::int, 0)
      when e.action_type = 'PoinPeringkatDonasi'
        then coalesce((e.payload->>'points')::int, 0)
      when e.action_type = 'TujuanFinansial'
        then coalesce((e.payload->>'points')::int, 0)
      else 0
    end), 0)::int as happiness_delta,
    coalesce(sum(case
      when e.action_type = 'Menabung' then (e.payload->>'amount')::int
      when e.action_type = 'TarikTabungan' then -(e.payload->>'amount')::int
      when e.action_type = 'TujuanFinansial' then -(e.payload->>'cost')::int
      else 0
    end), 0)::int as saving_delta,
    coalesce(sum(case
      when e.action_type = 'JumatBerkah' then (e.payload->>'amount')::int
      else 0
    end), 0)::int as donation_total
  from session_participants sp
  left join events e
    on e.session_id = sp.session_id
   and e.user_id = sp.user_id
  where sp.session_id in (
      '91000000-0000-0000-0000-000000000001'::uuid,
      '91000000-0000-0000-0000-000000000002'::uuid
    )
  group by sp.session_id, sp.session_participant_id
)
insert into session_participant_balances (
  session_id,
  session_participant_id,
  coins,
  happiness,
  saving,
  total_donasi,
  created_at,
  updated_at
)
select
  pc.session_id,
  pc.session_participant_id,
  greatest(pc.starting_cash + pc.net_cashflow, 0),
  greatest(pc.starting_happiness + pet.happiness_delta, 0),
  greatest(pc.starting_saving + pet.saving_delta, 0),
  greatest(pet.donation_total, 0),
  now(),
  now()
from participant_cashflow pc
join participant_event_totals pet
  on pet.session_id = pc.session_id
 and pet.session_participant_id = pc.session_participant_id
on conflict (session_participant_id) do update
set coins = excluded.coins,
    happiness = excluded.happiness,
    saving = excluded.saving,
    total_donasi = excluded.total_donasi,
    updated_at = excluded.updated_at;

with ingredient_moves as (
  select
    e.session_id,
    sp.session_participant_id,
    e.ruleset_version_id,
    rga.ruleset_game_asset_id,
    1::int as quantity_delta,
    e.timestamp as moved_at
  from events e
  join session_participants sp
    on sp.session_id = e.session_id
   and sp.user_id = e.user_id
  join ruleset_game_assets rga
    on rga.ruleset_version_id = e.ruleset_version_id
   and rga.asset_type = 'INGREDIENT'
   and (
     lower(rga.asset_code) = lower(coalesce(e.payload->>'card_id', e.payload->>'ingredient_id', e.payload->>'ingredient_name'))
     or lower(rga.display_name) = lower(coalesce(e.payload->>'card_id', e.payload->>'ingredient_id', e.payload->>'ingredient_name'))
   )
  where e.action_type = 'BahanMasakan'
    and e.session_id in (
      '91000000-0000-0000-0000-000000000001'::uuid,
      '91000000-0000-0000-0000-000000000002'::uuid
    )

  union all

  select
    e.session_id,
    sp.session_participant_id,
    e.ruleset_version_id,
    rga.ruleset_game_asset_id,
    -1::int as quantity_delta,
    e.timestamp as moved_at
  from events e
  join session_participants sp
    on sp.session_id = e.session_id
   and sp.user_id = e.user_id
  cross join lateral jsonb_array_elements_text(
    coalesce(e.payload->'required_ingredient_card_ids', '[]'::jsonb)
  ) as required(ingredient_id)
  join ruleset_game_assets rga
    on rga.ruleset_version_id = e.ruleset_version_id
   and rga.asset_type = 'INGREDIENT'
   and lower(rga.asset_code) = lower(required.ingredient_id)
  where e.action_type = 'JualMasakan'
    and e.session_id in (
      '91000000-0000-0000-0000-000000000001'::uuid,
      '91000000-0000-0000-0000-000000000002'::uuid
    )

  union all

  select
    e.session_id,
    sp.session_participant_id,
    e.ruleset_version_id,
    rga.ruleset_game_asset_id,
    -greatest(1, coalesce((e.payload->>'amount')::int, 1))::int as quantity_delta,
    e.timestamp as moved_at
  from events e
  join session_participants sp
    on sp.session_id = e.session_id
   and sp.user_id = e.user_id
  join ruleset_game_assets rga
    on rga.ruleset_version_id = e.ruleset_version_id
   and rga.asset_type = 'INGREDIENT'
   and (
     lower(rga.asset_code) = lower(coalesce(e.payload->>'card_id', e.payload->>'ingredient_id', e.payload->>'ingredient_name'))
     or lower(rga.display_name) = lower(coalesce(e.payload->>'card_id', e.payload->>'ingredient_id', e.payload->>'ingredient_name'))
   )
  where e.action_type = 'BuangBahanMasakan'
    and e.session_id in (
      '91000000-0000-0000-0000-000000000001'::uuid,
      '91000000-0000-0000-0000-000000000002'::uuid
    )
),
ingredient_totals as (
  select
    session_id,
    session_participant_id,
    ruleset_version_id,
    ruleset_game_asset_id,
    greatest(sum(quantity_delta), 0)::int as qty,
    max(moved_at) as updated_at
  from ingredient_moves
  group by session_id, session_participant_id, ruleset_version_id, ruleset_game_asset_id
)
insert into session_participant_inventory (
  session_id,
  session_participant_id,
  ruleset_version_id,
  ruleset_game_asset_id,
  qty,
  updated_at
)
select
  session_id,
  session_participant_id,
  ruleset_version_id,
  ruleset_game_asset_id,
  qty,
  updated_at
from ingredient_totals
on conflict (session_participant_id, ruleset_game_asset_id) do update
set qty = excluded.qty,
    updated_at = excluded.updated_at;

with need_events as (
  select
    e.session_id,
    sp.session_participant_id,
    e.ruleset_version_id,
    e.sequence_number,
    e.day_index,
    e.timestamp,
    e.payload,
    e.payload->>'card_id' as need_code,
    row_number() over (
      partition by e.session_id, sp.session_participant_id
      order by e.sequence_number
    )::int as sort_order
  from events e
  join session_participants sp
    on sp.session_id = e.session_id
   and sp.user_id = e.user_id
  where e.action_type = 'Kebutuhan'
    and e.session_id in (
      '91000000-0000-0000-0000-000000000001'::uuid,
      '91000000-0000-0000-0000-000000000002'::uuid
    )
)
insert into session_participant_need_purchases (
  session_participant_need_purchase_id,
  session_id,
  session_participant_id,
  ruleset_version_id,
  ruleset_need_id,
  sort_order,
  paid_amount,
  happiness_delta,
  purchased_at_day,
  created_at
)
select
  gen_random_uuid(),
  ne.session_id,
  ne.session_participant_id,
  ne.ruleset_version_id,
  rn.ruleset_need_id,
  ne.sort_order,
  coalesce((ne.payload->>'amount')::int, rn.purchase_price),
  coalesce((ne.payload->>'points')::int, rn.happiness_points),
  ne.day_index,
  ne.timestamp
from need_events ne
join ruleset_needs rn
  on rn.ruleset_version_id = ne.ruleset_version_id
 and lower(rn.need_code) = lower(ne.need_code)
 and rn.is_active;

with goal_events as (
  select
    e.session_id,
    sp.session_participant_id,
    e.ruleset_version_id,
    e.action_type,
    e.day_index,
    e.timestamp,
    e.payload->>'goal_id' as goal_code,
    coalesce((e.payload->>'amount')::int, 0) as amount
  from events e
  join session_participants sp
    on sp.session_id = e.session_id
   and sp.user_id = e.user_id
  where e.action_type in (
      'Menabung',
      'TarikTabungan',
      'TujuanFinansial'
    )
    and nullif(e.payload->>'goal_id', '') is not null
    and e.session_id in (
      '91000000-0000-0000-0000-000000000001'::uuid,
      '91000000-0000-0000-0000-000000000002'::uuid
    )
),
goal_totals as (
  select
    session_id,
    session_participant_id,
    ruleset_version_id,
    goal_code,
    count(*) filter (where action_type = 'Menabung')::int as deposit_count,
    count(*) filter (where action_type = 'TujuanFinansial')::int as achieved_count,
    coalesce(sum(case
      when action_type = 'Menabung' then amount
      when action_type = 'TarikTabungan' then -amount
      else 0
    end), 0)::int as current_amount,
    min(day_index) filter (where action_type = 'TujuanFinansial') as achieved_day_index,
    min(timestamp) as created_at,
    max(timestamp) as updated_at
  from goal_events
  group by
    session_id,
    session_participant_id,
    ruleset_version_id,
    goal_code
)
insert into session_participant_financial_goals (
  session_id,
  session_participant_id,
  ruleset_version_id,
  ruleset_financial_goal_id,
  current_amount,
  target_amount,
  status,
  purchased_at_day,
  created_at,
  updated_at
)
select
  gt.session_id,
  gt.session_participant_id,
  gt.ruleset_version_id,
  rfg.ruleset_financial_goal_id,
  case
    when gt.achieved_count > 0 then rfg.purchase_price
    else least(rfg.purchase_price, greatest(gt.current_amount, 0))
  end,
  rfg.purchase_price,
  case when gt.achieved_count > 0 then 'COMPLETED' else 'ONGOING' end,
  case when gt.achieved_count > 0 then gt.achieved_day_index end,
  gt.created_at,
  gt.updated_at
from goal_totals gt
join ruleset_financial_goals rfg
  on rfg.ruleset_version_id = gt.ruleset_version_id
 and lower(rfg.goal_code) = lower(gt.goal_code)
 and rfg.is_active
where gt.deposit_count > 0
   or gt.achieved_count > 0
on conflict (session_participant_id, ruleset_financial_goal_id) do update
set current_amount = excluded.current_amount,
    target_amount = excluded.target_amount,
    status = excluded.status,
    purchased_at_day = excluded.purchased_at_day,
    updated_at = excluded.updated_at;

insert into session_participant_action_counters (
  session_id,
  session_participant_id,
  ruleset_version_id,
  ruleset_action_id,
  count,
  last_event_id
)
select
  e.session_id,
  sp.session_participant_id,
  e.ruleset_version_id,
  e.ruleset_action_id,
  count(*)::int,
  (array_agg(e.event_id order by e.sequence_number desc))[1]
from events e
join session_participants sp
  on sp.session_id = e.session_id
 and sp.user_id = e.user_id
where e.session_id in (
    '91000000-0000-0000-0000-000000000001'::uuid,
    '91000000-0000-0000-0000-000000000002'::uuid
  )
  and e.session_player_id is not null
group by e.session_id, sp.session_participant_id, e.ruleset_version_id, e.ruleset_action_id
on conflict (session_participant_id, ruleset_action_id) do update
set count = excluded.count,
    last_event_id = excluded.last_event_id;

with donation_rank_events as (
  select
    e.session_id,
    e.event_id,
    e.sequence_number,
    e.day_index as day,
    greatest(1, ((e.day_index + 2) / 7))::int as event_ke
  from events e
  where e.action_type = 'PoinPeringkatDonasi'
    and e.session_id in (
      '91000000-0000-0000-0000-000000000001'::uuid,
      '91000000-0000-0000-0000-000000000002'::uuid
    )
),
donation_event_rows as (
  select distinct on (session_id, event_ke)
    gen_random_uuid() as donation_event_id,
    session_id,
    event_ke,
    day,
    event_id as source_event_id
  from donation_rank_events
  order by session_id, event_ke, sequence_number
)
insert into session_donation_events (
  donation_event_id,
  session_id,
  event_ke,
  day,
  source_event_id,
  created_at
)
select
  donation_event_id,
  session_id,
  event_ke,
  day,
  source_event_id,
  now()
from donation_event_rows
on conflict (session_id, event_ke) do update
set day = excluded.day,
    source_event_id = excluded.source_event_id;

insert into session_participant_tie_breakers (
  session_id,
  session_participant_id,
  ruleset_version_id,
  ruleset_game_asset_id,
  tie_number,
  source_event_id,
  assigned_at,
  metadata_json
)
select
  e.session_id,
  e.session_player_id,
  e.ruleset_version_id,
  rga.ruleset_game_asset_id,
  (e.payload->>'number')::int,
  e.event_id,
  e.timestamp,
  jsonb_build_object('source', 'BagikanTieBreaker')
from events e
join ruleset_game_assets rga
  on rga.ruleset_version_id = e.ruleset_version_id
 and rga.asset_type = 'TIE_BREAKER'
 and rga.asset_code = coalesce(e.payload->>'card_code', e.payload->>'tie_breaker_code')
where e.action_type = 'BagikanTieBreaker'
  and e.session_player_id is not null
  and e.session_id in (
    '91000000-0000-0000-0000-000000000001'::uuid,
    '91000000-0000-0000-0000-000000000002'::uuid
  )
on conflict (session_participant_id) do update
set ruleset_version_id = excluded.ruleset_version_id,
    ruleset_game_asset_id = excluded.ruleset_game_asset_id,
    tie_number = excluded.tie_number,
    source_event_id = excluded.source_event_id,
    assigned_at = excluded.assigned_at,
    metadata_json = excluded.metadata_json;

with assigned_missions as (
  select
    e.session_id,
    e.session_player_id as session_participant_id,
    e.ruleset_version_id,
    e.timestamp as assigned_at,
    rcm.ruleset_collection_mission_id
  from events e
  join ruleset_collection_missions rcm
    on rcm.ruleset_version_id = e.ruleset_version_id
   and lower(rcm.mission_code) = lower(e.payload->>'mission_id')
   and rcm.is_active
  where e.action_type = 'BagikanMisiKoleksi'
    and e.session_player_id is not null
    and e.session_id in (
      '91000000-0000-0000-0000-000000000001'::uuid,
      '91000000-0000-0000-0000-000000000002'::uuid
    )
),
mission_status as (
  select
    am.*,
    not exists (
      select 1
      from ruleset_collection_mission_requirements req
      where req.ruleset_collection_mission_id = am.ruleset_collection_mission_id
        and not exists (
          select 1
          from session_participant_need_purchases purchase
          join ruleset_needs need
            on need.ruleset_need_id = purchase.ruleset_need_id
          where purchase.session_id = am.session_id
            and purchase.session_participant_id = am.session_participant_id
            and (
              (upper(req.requirement_type) = 'NEED_TIER' and lower(need.need_tier) = lower(req.required_need_tier))
              or
              (upper(req.requirement_type) = 'ASSET' and need.ruleset_game_asset_id = req.required_asset_id)
            )
        )
    ) as is_completed
  from assigned_missions am
)
insert into session_participant_collection_missions (
  session_id,
  session_participant_id,
  ruleset_version_id,
  ruleset_collection_mission_id,
  is_completed,
  is_failed,
  reward_applied,
  assigned_at
)
select
  session_id,
  session_participant_id,
  ruleset_version_id,
  ruleset_collection_mission_id,
  is_completed,
  not is_completed,
  false,
  assigned_at
from mission_status
on conflict (session_participant_id, ruleset_collection_mission_id) do update
set is_completed = excluded.is_completed,
    is_failed = excluded.is_failed,
    reward_applied = excluded.reward_applied,
    assigned_at = excluded.assigned_at;

with catalog_copies as (
  select
    s.session_id,
    rci.ruleset_version_id,
    rci.ruleset_catalog_item_id as ruleset_game_asset_id,
    rci.item_type,
    rci.item_code,
    copy_number,
    row_number() over (
      partition by s.session_id, rci.item_type
      order by rci.sort_order, rci.item_code, copy_number
    )::int as market_rank,
    row_number() over (
      partition by s.session_id
      order by rci.sort_order, rci.item_type, rci.item_code, copy_number
    )::int as position_order
  from sessions s
  join ruleset_catalog_items rci
    on rci.ruleset_version_id = s.ruleset_version_id
   and rci.is_active
   and coalesce(rci.card_qty, 0) > 0
  cross join lateral generate_series(1, rci.card_qty) as copies(copy_number)
  where s.session_id in (
      '91000000-0000-0000-0000-000000000001'::uuid,
      '91000000-0000-0000-0000-000000000002'::uuid
    )
)
insert into session_card_positions (
  card_instance_id,
  session_id,
  ruleset_version_id,
  owner_session_participant_id,
  ruleset_game_asset_id,
  copy_number,
  zone,
  position_order,
  slot_code,
  slot_group,
  status,
  last_event_id,
  created_at,
  updated_at
)
select
  gen_random_uuid(),
  cc.session_id,
  cc.ruleset_version_id,
  case when cc.item_type = 'TIE_BREAKER' then sp.session_participant_id end,
  cc.ruleset_game_asset_id,
  cc.copy_number,
  case
    when cc.item_type = 'TIE_BREAKER' then 'PLAYER'
    when cc.item_type in ('INGREDIENT', 'ORDER', 'NEED') and cc.market_rank <= 5 then 'MARKET'
    else 'DECK'
  end,
  cc.position_order,
  case
    when cc.item_type = 'TIE_BREAKER' then concat('TIE_BREAKER_', sp.player_order_no)
    when cc.item_type in ('INGREDIENT', 'ORDER', 'NEED') and cc.market_rank <= 5 then concat('SLOT_', cc.market_rank)
  end,
  case
    when cc.item_type = 'TIE_BREAKER' then 'PLAYER_SETUP'
    when cc.item_type in ('INGREDIENT', 'ORDER', 'NEED') and cc.market_rank <= 5 then concat(cc.item_type, '_MARKET')
  end,
  'ACTIVE',
  (
    select e.event_id
    from events e
    where e.session_id = cc.session_id
    order by e.sequence_number desc
    limit 1
  ),
  now(),
  now()
from catalog_copies cc
left join ruleset_tie_breakers rtb
  on rtb.ruleset_version_id = cc.ruleset_version_id
 and rtb.tie_breaker_code = cc.item_code
left join session_participants sp
  on sp.session_id = cc.session_id
 and sp.player_order_no = rtb.tie_number
on conflict (session_id, ruleset_game_asset_id, copy_number) do update
set owner_session_participant_id = excluded.owner_session_participant_id,
    zone = excluded.zone,
    position_order = excluded.position_order,
    slot_code = excluded.slot_code,
    slot_group = excluded.slot_group,
    status = excluded.status,
    last_event_id = excluded.last_event_id,
    updated_at = excluded.updated_at;

create temporary table seed_pension_rank_points on commit drop as
with ranked as (
  select
    sp.session_id,
    sp.session_participant_id,
    spb.coins + spb.saving + coalesce(ingredients.leftover_qty, 0) as cash_remaining,
    row_number() over (
      partition by sp.session_id
      order by (spb.coins + spb.saving + coalesce(ingredients.leftover_qty, 0)) desc, sptb.tie_number desc, sp.user_id
    )::int as rank_no
  from session_participants sp
  join session_participant_balances spb
    on spb.session_id = sp.session_id
   and spb.session_participant_id = sp.session_participant_id
  left join (
    select session_id, session_participant_id, sum(qty)::int as leftover_qty
    from session_participant_inventory
    group by session_id, session_participant_id
  ) ingredients
    on ingredients.session_id = sp.session_id
   and ingredients.session_participant_id = sp.session_participant_id
  left join session_participant_tie_breakers sptb
    on sptb.session_id = sp.session_id
   and sptb.session_participant_id = sp.session_participant_id
  where sp.session_id in (
      '91000000-0000-0000-0000-000000000001'::uuid,
      '91000000-0000-0000-0000-000000000002'::uuid
    )
)
select
  ranked.session_id,
  ranked.session_participant_id,
  ranked.cash_remaining,
  ranked.rank_no,
  coalesce(rprp.points, 0)::int as points_awarded
from ranked
join sessions s
  on s.session_id = ranked.session_id
left join ruleset_rank_points rprp
  on rprp.ruleset_version_id = s.ruleset_version_id
 and rprp.rank_type = 'PENSION'
 and rprp.rank_no = ranked.rank_no;

delete from session_final_score_components
where session_id in (
  '91000000-0000-0000-0000-000000000001'::uuid,
  '91000000-0000-0000-0000-000000000002'::uuid
);

delete from session_final_scores
where session_id in (
  '91000000-0000-0000-0000-000000000001'::uuid,
  '91000000-0000-0000-0000-000000000002'::uuid
);

with component_values as (
  select
    sp.session_id,
    sp.session_participant_id,
    sptb.tie_number,
    count(*) filter (where e.action_type = 'Kebutuhan' and lower((e.payload::jsonb)->>'need_tier') = 'primer')::int as primary_need_count,
    count(*) filter (where e.action_type = 'Kebutuhan' and lower((e.payload::jsonb)->>'need_tier') = 'sekunder')::int as secondary_need_count,
    count(*) filter (where e.action_type = 'Kebutuhan' and lower((e.payload::jsonb)->>'need_tier') = 'tersier')::int as tertiary_need_count,
    coalesce(sum(case
      when e.action_type = 'Kebutuhan'
        then coalesce((e.payload->>'points')::int, 0)
      else 0
    end), 0)::int as need_points,
    coalesce(sum(case
      when e.action_type = 'PoinPeringkatDonasi'
        then coalesce((e.payload->>'points')::int, 0)
      else 0
    end), 0)::int as donation_points,
    coalesce(sum(case
      when e.action_type = 'TujuanFinansial'
        then coalesce((e.payload->>'points')::int, 0)
      else 0
    end), 0)::int as saving_goal_points,
    coalesce((
      select sum(rcm.penalty_points)
      from session_participant_collection_missions spcm
      join ruleset_collection_missions rcm
        on rcm.ruleset_collection_mission_id = spcm.ruleset_collection_mission_id
      where spcm.session_id = sp.session_id
        and spcm.session_participant_id = sp.session_participant_id
        and spcm.is_failed
    ), 0)::int as mission_penalty_points,
    coalesce(spr.points_awarded, 0)::int as pension_points,
    coalesce((
      select rga.points
      from session_participant_gold_holdings spgh
      join ruleset_game_assets asset
        on asset.ruleset_game_asset_id = spgh.ruleset_game_asset_id
      join ruleset_gold_assets rga
        on rga.ruleset_version_id = asset.ruleset_version_id
       and rga.quantity = spgh.quantity
      where spgh.session_participant_id = sp.session_participant_id
      limit 1
    ), 0)::int as gold_points,
    coalesce((
      select sum((spl.metadata_json->>'penalty_points')::int)
      from session_participant_loans spl
      where spl.session_participant_id = sp.session_participant_id
        and spl.status = 'ACTIVE'
    ), 0)::int as loan_penalty_points
  from session_participants sp
  left join events e
    on e.session_id = sp.session_id
   and e.user_id = sp.user_id
  left join session_participant_tie_breakers sptb
    on sptb.session_id = sp.session_id
   and sptb.session_participant_id = sp.session_participant_id
  left join seed_pension_rank_points spr
    on spr.session_id = sp.session_id
   and spr.session_participant_id = sp.session_participant_id
  where sp.session_id in (
      '91000000-0000-0000-0000-000000000001'::uuid,
      '91000000-0000-0000-0000-000000000002'::uuid
    )
  group by
    sp.session_id,
    sp.session_participant_id,
    sptb.tie_number,
    spr.points_awarded
),
scored as (
  select
    *,
    least(primary_need_count, least(secondary_need_count, tertiary_need_count))::int as mixed_need_sets
  from component_values
),
scored_with_bonuses as (
  select
    *,
    (
      mixed_need_sets * 4
      + (((primary_need_count - mixed_need_sets) / 3)::int * 2)
      + (((secondary_need_count - mixed_need_sets) / 3)::int * 2)
      + (((tertiary_need_count - mixed_need_sets) / 3)::int * 2)
    )::int as need_set_bonus_points,
    case when loan_penalty_points > 0 then 0 else saving_goal_points end as saving_goal_points_effective
  from scored
),
final_component_values as (
  select
    *,
    need_points + need_set_bonus_points + donation_points + saving_goal_points_effective + pension_points + gold_points - mission_penalty_points - loan_penalty_points as total_points
  from scored_with_bonuses
),
ranked_scores as (
  select
    *,
    row_number() over (
      partition by session_id
      order by total_points desc, tie_number desc, session_participant_id
    )::int as rank_no
  from final_component_values
),
upserted_scores as (
  insert into session_final_scores (
    session_final_score_id,
    session_id,
    session_participant_id,
    total_points,
    rank_no,
    tie_breaker_number,
    has_unpaid_loan,
    computed_at,
    source_event_id
  )
  select
    gen_random_uuid(),
    session_id,
    session_participant_id,
    total_points,
    rank_no,
    tie_number,
    loan_penalty_points > 0,
    now(),
    (
      select e.event_id
      from events e
      where e.session_id = ranked_scores.session_id
      order by e.sequence_number desc
      limit 1
    )
  from ranked_scores
  on conflict (session_id, session_participant_id) do update
  set total_points = excluded.total_points,
      rank_no = excluded.rank_no,
      tie_breaker_number = excluded.tie_breaker_number,
      has_unpaid_loan = excluded.has_unpaid_loan,
      computed_at = excluded.computed_at,
      source_event_id = excluded.source_event_id
  returning
    session_final_score_id,
    session_id,
    session_participant_id,
    computed_at,
    source_event_id
)
insert into session_final_score_components (
  session_id,
  session_participant_id,
  session_final_score_id,
  component_code,
  points,
  source_event_id,
  created_at
)
select
  score.session_id,
  score.session_participant_id,
  score.session_final_score_id,
  component.component_code,
  component.points,
  score.source_event_id,
  score.computed_at
from upserted_scores score
join ranked_scores ranked
  on ranked.session_id = score.session_id
 and ranked.session_participant_id = score.session_participant_id
cross join lateral (
  values
    ('NEED_POINTS'::varchar(80), ranked.need_points),
    ('NEED_SET_BONUS'::varchar(80), ranked.need_set_bonus_points),
    ('DONATION'::varchar(80), ranked.donation_points),
    ('GOLD'::varchar(80), ranked.gold_points),
    ('PENSION'::varchar(80), ranked.pension_points),
    ('SAVING_GOAL'::varchar(80), ranked.saving_goal_points_effective),
    ('MISSION_PENALTY'::varchar(80), -ranked.mission_penalty_points),
    ('LOAN_PENALTY'::varchar(80), -ranked.loan_penalty_points)
) as component(component_code, points)
on conflict (session_final_score_id, component_code) do update
set points = excluded.points,
    source_event_id = excluded.source_event_id,
    created_at = excluded.created_at;

with seed_sessions as (
  select
    s.session_id,
    s.session_name,
    s.mode,
    s.ended_at,
    s.ruleset_version_id,
    case when s.mode = 'MAHIR' then 10 else 20 end as starting_coins,
    case when s.mode = 'MAHIR' then 'advanced' else 'beginner' end as gameplay_mode
  from sessions s
  where s.session_id in (
      '91000000-0000-0000-0000-000000000001'::uuid,
      '91000000-0000-0000-0000-000000000002'::uuid
    )
),
player_base as (
  select
    ss.session_id,
    ss.session_name,
    ss.mode,
    ss.ended_at,
    ss.ruleset_version_id,
    ss.starting_coins,
    ss.gameplay_mode,
    sp.session_participant_id as session_player_id,
    sp.user_id,
    sp.player_order_no,
    au.display_name
  from seed_sessions ss
  join session_participants sp on sp.session_id = ss.session_id
  join app_users au on au.user_id = sp.user_id
),
event_agg as (
  select
    pb.session_id,
    pb.user_id,
    count(e.event_pk) filter (where e.actor_type = 'PLAYER')::int as player_event_count,
    count(e.event_pk)::int as event_count,
    coalesce(max(e.action_slot), 0)::int as latest_action_slot,
    coalesce(max(e.day_index), 1)::int as latest_day_index,
    coalesce(max(e.weekday), 'MON') as latest_weekday,
    coalesce(max(e.timestamp), pb.ended_at) as latest_event_timestamp,
    coalesce(jsonb_agg(
      jsonb_build_object(
        'day_index', e.day_index,
        'action_slot', e.action_slot,
        'sequence_number', e.sequence_number,
        'action_type', e.action_type
      )
      order by e.sequence_number
    ) filter (where e.actor_type = 'PLAYER'), '[]'::jsonb) as action_sequence,
    coalesce(sum(case when e.action_type = 'BahanMasakan' then ((e.payload::jsonb)->>'amount')::int else 0 end), 0)::int as ingredients_collected,
    coalesce(sum(case when e.action_type = 'BuangBahanMasakan' then ((e.payload::jsonb)->>'amount')::int else 0 end), 0)::int as ingredients_wasted,
    count(*) filter (where e.action_type = 'JualMasakan')::int as meal_orders_claimed,
    count(*) filter (where e.action_type = 'LewatiOrder')::int as meal_orders_passed,
    coalesce(sum(case when e.action_type = 'JualMasakan' then ((e.payload::jsonb)->>'income')::int else 0 end), 0)::int as meal_order_income_total,
    count(*) filter (where e.action_type = 'Kebutuhan' and lower((e.payload::jsonb)->>'need_tier') = 'primer')::int as primary_needs_owned,
    count(*) filter (where e.action_type = 'Kebutuhan' and lower((e.payload::jsonb)->>'need_tier') = 'sekunder')::int as secondary_needs_owned,
    count(*) filter (where e.action_type = 'Kebutuhan' and lower((e.payload::jsonb)->>'need_tier') = 'tersier')::int as tertiary_needs_owned,
    coalesce(sum(case when e.action_type = 'Kebutuhan' then ((e.payload::jsonb)->>'amount')::int else 0 end), 0)::int as need_cards_coins_spent,
    coalesce(sum(case when e.action_type = 'Kebutuhan' then ((e.payload::jsonb)->>'points')::int else 0 end), 0)::int as need_points,
    coalesce(sum(case when e.action_type = 'JumatBerkah' then ((e.payload::jsonb)->>'amount')::int else 0 end), 0)::int as donation_total_coins,
    count(*) filter (where e.action_type = 'JumatBerkah')::int as donation_events,
    coalesce(sum(case when e.action_type = 'InvestasiEmas' and upper((e.payload::jsonb)->>'trade_type') = 'BUY' then ((e.payload::jsonb)->>'qty')::int else 0 end), 0)::int as gold_cards_purchased,
    coalesce(sum(case when e.action_type = 'JualEmas' then ((e.payload::jsonb)->>'qty')::int else 0 end), 0)::int as gold_cards_sold,
    coalesce(sum(case when e.action_type = 'InvestasiEmas' and upper((e.payload::jsonb)->>'trade_type') = 'BUY' then ((e.payload::jsonb)->>'amount')::int else 0 end), 0)::int as gold_investment_coins_spent,
    coalesce(sum(case when e.action_type = 'JualEmas' then ((e.payload::jsonb)->>'amount')::int else 0 end), 0)::int as gold_investment_coins_earned,
    count(*) filter (where e.action_type = 'RisikoKehidupan')::int as life_risk_cards_drawn,
    coalesce(sum(case when e.action_type = 'RisikoKehidupan' and risk_catalog.direction = 'OUT' then risk_catalog.amount else 0 end), 0)::int as life_risk_costs_total,
    count(*) filter (where e.action_type = 'Asuransi' and ((e.payload::jsonb ? 'risk_event_id') or (e.payload::jsonb ? 'risk_event_ref')))::int as life_risk_mitigated_with_insurance,
    count(*) filter (where e.action_type = 'Asuransi' and e.payload::jsonb ? 'premium')::int as insurance_payments_made,
    count(*) filter (where e.action_type = 'GunakanOpsiDarurat')::int as emergency_options_used,
    count(*) filter (where e.action_type = 'Menabung')::int as financial_goals_attempted,
    count(*) filter (where e.action_type = 'TujuanFinansial')::int as financial_goals_completed,
    coalesce(sum(case when e.action_type = 'Menabung' then ((e.payload::jsonb)->>'amount')::int else 0 end), 0)::int as coins_saved,
    coalesce(sum(case when e.action_type = 'TujuanFinansial' then ((e.payload::jsonb)->>'points')::int else 0 end), 0)::int as saving_goal_points,
    count(*) filter (where e.action_type = 'PinjamanSyariah')::int as sharia_loans_taken,
    count(*) filter (where e.action_type = 'BayarPinjaman')::int as sharia_loans_repaid,
    coalesce(sum(case when e.action_type = 'PinjamanSyariah' then ((e.payload::jsonb)->>'principal')::int else 0 end), 0)::int as sharia_loan_principal_total,
    coalesce(sum(case when e.action_type = 'BayarPinjaman' then ((e.payload::jsonb)->>'amount')::int else 0 end), 0)::int as sharia_loan_repaid_total,
    count(*) filter (where e.action_type = 'BagikanMisiKoleksi')::int as mission_assigned_count
  from player_base pb
  left join events e
    on e.session_id = pb.session_id
   and e.user_id = pb.user_id
  left join ruleset_life_risks risk_catalog
    on risk_catalog.ruleset_version_id = e.ruleset_version_id
   and lower(risk_catalog.risk_code) = lower(e.payload::jsonb->>'risk_id')
  group by pb.session_id, pb.user_id, pb.ended_at
),
projection_agg as (
  select
    pb.session_id,
    pb.user_id,
    coalesce(sum(p.amount) filter (where p.direction = 'IN'), 0)::int as cash_in_total,
    coalesce(sum(p.amount) filter (where p.direction = 'OUT'), 0)::int as cash_out_total,
    count(p.projection_id)::int as transaction_count
  from player_base pb
  left join event_cashflow_projections p
    on p.session_id = pb.session_id
   and p.user_id = pb.user_id
  group by pb.session_id, pb.user_id
),
player_metric_base as (
  select
    pb.*,
    ea.player_event_count,
    ea.event_count,
    ea.latest_action_slot,
    ea.latest_day_index,
    ea.latest_weekday,
    ea.latest_event_timestamp,
    ea.action_sequence,
    ea.ingredients_collected,
    ea.ingredients_wasted,
    ea.meal_orders_claimed,
    ea.meal_orders_passed,
    ea.meal_order_income_total,
    ea.primary_needs_owned,
    ea.secondary_needs_owned,
    ea.tertiary_needs_owned,
    ea.need_cards_coins_spent,
    ea.need_points,
    ea.donation_total_coins,
    ea.donation_events,
    ea.gold_cards_purchased,
    ea.gold_cards_sold,
    ea.gold_investment_coins_spent,
    ea.gold_investment_coins_earned,
    (ea.gold_cards_purchased - ea.gold_cards_sold) as gold_cards_held_end,
    (ea.gold_investment_coins_earned - ea.gold_investment_coins_spent) as gold_investment_net,
    ea.life_risk_cards_drawn,
    ea.life_risk_costs_total,
    ea.life_risk_mitigated_with_insurance,
    ea.insurance_payments_made,
    ea.emergency_options_used,
    ea.financial_goals_attempted,
    ea.financial_goals_completed,
    ea.coins_saved,
    ea.saving_goal_points,
    ea.sharia_loans_taken,
    ea.sharia_loans_repaid,
    ea.sharia_loan_principal_total,
    ea.sharia_loan_repaid_total,
    greatest(ea.sharia_loan_principal_total - ea.sharia_loan_repaid_total, 0) as sharia_loans_outstanding_coins,
    greatest(ea.sharia_loans_taken - ea.sharia_loans_repaid, 0) as sharia_loans_unpaid_end,
    ea.mission_assigned_count,
    pa.cash_in_total,
    pa.cash_out_total,
    (pa.cash_in_total - pa.cash_out_total) as cash_net_total,
    (pb.starting_coins + pa.cash_in_total - pa.cash_out_total) as coins_held_current,
    pa.transaction_count
  from player_base pb
  join event_agg ea on ea.session_id = pb.session_id and ea.user_id = pb.user_id
  join projection_agg pa on pa.session_id = pb.session_id and pa.user_id = pb.user_id
),
snapshot_rows as (
  select
    session_id,
    user_id,
    session_player_id,
    ruleset_version_id,
    coalesce(ended_at, latest_event_timestamp) as computed_at,
    'gameplay.raw.variables' as metric_name,
    1 as metric_sort,
    jsonb_build_object(
      'metadata', jsonb_build_object(
        'game_id', session_id,
        'session_id', session_id,
        'user_id', user_id,
        'player_alias', display_name,
        'game_mode', gameplay_mode,
        'action_slot', latest_action_slot,
        'day_index', latest_day_index,
        'day_label', latest_weekday,
        'event_timestamp', latest_event_timestamp,
        'seed_source', '02_seed_simulation_sessions_events'
      ),
      'coins', jsonb_build_object(
        'starting_coins', starting_coins,
        'cash_in_total', cash_in_total,
        'cash_out_total', cash_out_total,
        'coins_held_current', coins_held_current,
        'coins_spent_total', cash_out_total,
        'coins_earned_total', cash_in_total,
        'coins_donated', donation_total_coins,
        'coins_saved', coins_saved,
        'coins_net_end_game', cash_net_total
      ),
      'ingredients', jsonb_build_object(
        'ingredients_collected', ingredients_collected,
        'ingredients_held_current', greatest(ingredients_collected - ingredients_wasted, 0),
        'ingredients_used_per_meal', meal_orders_claimed,
        'ingredients_wasted', ingredients_wasted,
        'ingredient_investment_coins_total', coalesce(need_cards_coins_spent, 0)
      ),
      'meal_orders', jsonb_build_object(
        'meal_orders_claimed', meal_orders_claimed,
        'meal_orders_available_passed', meal_orders_passed,
        'meal_order_income_total', meal_order_income_total,
        'meal_orders_per_turn_average', round(meal_orders_claimed::numeric / greatest(latest_action_slot, 1), 2)
      ),
      'needs', jsonb_build_object(
        'need_cards_purchased', primary_needs_owned + secondary_needs_owned + tertiary_needs_owned,
        'primary_needs_owned', primary_needs_owned,
        'secondary_needs_owned', secondary_needs_owned,
        'tertiary_needs_owned', tertiary_needs_owned,
        'collection_mission_complete', tertiary_needs_owned > 0,
        'need_cards_coins_spent', need_cards_coins_spent
      ),
      'donations', jsonb_build_object(
        'donation_events', donation_events,
        'donation_total_coins', donation_total_coins,
        'donation_happiness_points', donation_total_coins
      ),
      'gold', jsonb_build_object(
        'gold_cards_purchased', gold_cards_purchased,
        'gold_cards_sold', gold_cards_sold,
        'gold_cards_held_end', gold_cards_held_end,
        'gold_investment_coins_spent', gold_investment_coins_spent,
        'gold_investment_coins_earned', gold_investment_coins_earned,
        'gold_investment_net', gold_investment_net
      ),
      'pension', jsonb_build_object(
        'leftover_coins_end_game', coins_held_current,
        'ingredient_cards_value_end', greatest(ingredients_collected - ingredients_wasted, 0),
        'coins_in_savings_goal', coins_saved,
        'pension_fund_total', coins_held_current + greatest(ingredients_collected - ingredients_wasted, 0) + coins_saved
      ),
      'life_risk', jsonb_build_object(
        'life_risks_available', life_risk_cards_drawn,
        'life_risk_cards_drawn', life_risk_cards_drawn,
        'life_risks_accepted', life_risk_cards_drawn,
        'life_risk_costs_total', life_risk_costs_total,
        'life_risk_mitigated_with_insurance', life_risk_mitigated_with_insurance,
        'insurance_payments_made', insurance_payments_made,
        'emergency_options_used', emergency_options_used
      ),
      'financial_goals', jsonb_build_object(
        'financial_goals_attempted', financial_goals_attempted,
        'financial_goals_completed', financial_goals_completed,
        'financial_goals_coins_total_invested', coins_saved,
        'sharia_loans_taken', sharia_loans_taken,
        'sharia_loans_repaid', sharia_loans_repaid,
        'sharia_loans_unpaid_end', sharia_loans_unpaid_end,
        'sharia_loans_outstanding_coins', sharia_loans_outstanding_coins
      ),
      'actions', jsonb_build_object(
        'actions_per_turn', 2,
        'action_events_total', player_event_count,
        'action_sequence', action_sequence,
        'actions_skipped', 0
      ),
      'turns', jsonb_build_object(
        'action_slot_game_completion', latest_action_slot,
        'latest_day_index', latest_day_index,
        'transaction_count', transaction_count,
        'event_count', event_count
      ),
      'outcomes', jsonb_build_object(
        'total_happiness_points', need_points + donation_total_coins + saving_goal_points,
        'finish_line_reached', true,
        'dnf_flag', false
      ),
      'notes', '[]'::jsonb
    ) as metric_payload_json
  from player_metric_base
  union all
  select
    session_id,
    user_id,
    session_player_id,
    ruleset_version_id,
    coalesce(ended_at, latest_event_timestamp) as computed_at,
    'gameplay.derived.metrics' as metric_name,
    2 as metric_sort,
    jsonb_build_object(
      'net_worth_index', round((coins_held_current::numeric / greatest(starting_coins, 1)) * 100, 2),
      'income_diversification_index', case when cash_in_total > 0 then 1 else 0 end,
      'income_diversification_ratio', case when cash_in_total > 0 then 1 else 0 end,
      'income_diversification_components', jsonb_build_object(
        'total_income', cash_in_total,
        'transaction_count', transaction_count
      ),
      'expense_management_efficiency', round((greatest(starting_coins + cash_in_total - cash_out_total, 0)::numeric / greatest(starting_coins + cash_in_total, 1)) * 100, 2),
      'expense_management_components', jsonb_build_object(
        'essential_expenses', need_cards_coins_spent,
        'total_expenses', cash_out_total
      ),
      'business_profit_margin', round(((meal_order_income_total - greatest(ingredients_collected, 0))::numeric / greatest(meal_order_income_total, 1)) * 100, 2),
      'business_efficiency_ratio', round(meal_order_income_total::numeric / greatest(ingredients_collected, 1), 2),
      'gold_roi_percentage', round((gold_investment_net::numeric / greatest(gold_investment_coins_spent, 1)) * 100, 2),
      'risk_exposure_percentage', round((life_risk_cards_drawn::numeric / greatest(player_event_count, 1)) * 100, 2),
      'risk_mitigation_effectiveness', round((life_risk_mitigated_with_insurance::numeric / greatest(life_risk_cards_drawn, 1)) * 100, 2),
      'risk_appetite_score', life_risk_cards_drawn,
      'risk_appetite_components', jsonb_build_object(
        'life_risks_accepted', life_risk_cards_drawn,
        'life_risks_available', life_risk_cards_drawn,
        'insurance_activation_rate', round((life_risk_mitigated_with_insurance::numeric / greatest(life_risk_cards_drawn, 1)) * 100, 2)
      ),
      'debt_leverage_ratio', round((sharia_loan_principal_total::numeric / greatest(starting_coins, 1)) * 100, 2),
      'loan_repayment_discipline', round((sharia_loans_repaid::numeric / greatest(sharia_loans_taken, 1)) * 100, 2),
      'debt_ratio', round((sharia_loans_outstanding_coins::numeric / greatest(starting_coins + cash_in_total, 1)) * 100, 2),
      'goal_ambition', financial_goals_attempted,
      'goal_setting_ambition', financial_goals_attempted,
      'goal_setting_components', jsonb_build_object(
        'Goal_Attempt_Rate', financial_goals_attempted,
        'Goal_Investment_Rate', coins_saved
      ),
      'action_efficiency', round((transaction_count::numeric / greatest(player_event_count, 1)) * 100, 2),
      'action_efficiency_percent', round((transaction_count::numeric / greatest(player_event_count, 1)) * 100, 2),
      'action_diversity_score_avg', transaction_count,
      'meal_order_success_rate', round((meal_orders_claimed::numeric / greatest(meal_orders_claimed + meal_orders_passed, 1)) * 100, 2),
      'planning_horizon', latest_action_slot,
      'planning_horizon_percent', round((latest_day_index::numeric / 25) * 100, 2),
      'fulfillment_diversity', primary_needs_owned + secondary_needs_owned + tertiary_needs_owned,
      'fulfillment_diversity_components', jsonb_build_object(
        'p_primary', primary_needs_owned,
        'p_secondary', secondary_needs_owned,
        'p_tertiary', tertiary_needs_owned
      ),
      'mission_achievement', case when tertiary_needs_owned > 0 then 1 else 0 end,
      'growth_pattern_ratio', round((cash_net_total::numeric / greatest(starting_coins, 1)) * 100, 2),
      'donation_aggressiveness_percent', round((donation_total_coins::numeric / greatest(cash_out_total, 1)) * 100, 2),
      'donation_stability_std_deviation', 0,
      'donation_ratio', round((donation_total_coins::numeric / greatest(cash_out_total, 1)) * 100, 2),
      'friday_participation_rate', donation_events,
      'donation_commitment_score', donation_total_coins,
      'donation_commitment_components', jsonb_build_object(
        'donation_stability', donation_events,
        'donation_ratio', round((donation_total_coins::numeric / greatest(cash_out_total, 1)) * 100, 2),
        'friday_participation_rate', donation_events
      ),
      'risk_appetite_score_normalized', round((life_risk_cards_drawn::numeric / greatest(player_event_count, 1)) * 100, 2),
      'sharia_loans_outstanding_coins', sharia_loans_outstanding_coins,
      'happiness_portfolio', jsonb_build_object(
        'need_cards_pts', need_points,
        'donations_pts', donation_total_coins,
        'gold_pts', greatest(gold_investment_net, 0),
        'pension_pts', greatest(coins_held_current, 0),
        'financial_goals_pts', saving_goal_points,
        'mission_bonus_pts', 0
      ),
      'notes', '[]'::jsonb
    ) as metric_payload_json
  from player_metric_base
),
numbered_snapshots as (
  select
    row_number() over (order by session_id, user_id, metric_sort) as snapshot_number,
    session_id,
    user_id,
    session_player_id,
    ruleset_version_id,
    computed_at,
    metric_name,
    metric_payload_json,
    (
      select e.event_id
      from events e
      where e.session_id = snapshot_rows.session_id
      order by e.sequence_number desc
      limit 1
    ) as last_event_id
  from snapshot_rows
)
insert into metric_snapshots (
  metric_snapshot_id,
  session_id,
  user_id,
  session_player_id,
  computed_at,
  metric_name,
  metric_value_numeric,
  metric_payload_json,
  ruleset_version_id,
  last_event_id
)
select
  ('97000000-0000-0000-0000-' || lpad(snapshot_number::text, 12, '0'))::uuid,
  session_id,
  user_id,
  session_player_id,
  computed_at,
  metric_name,
  null,
  metric_payload_json::jsonb,
  ruleset_version_id,
  last_event_id
from numbered_snapshots
order by snapshot_number;

commit;
