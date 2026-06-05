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
    where lower(username) in ('seed.instructor', 'seed.player1', 'seed.player2', 'seed.player3')
      and user_id not in (
        '90000000-0000-0000-0000-000000000001'::uuid,
        '90000000-0000-0000-0000-000000000011'::uuid,
        '90000000-0000-0000-0000-000000000012'::uuid,
        '90000000-0000-0000-0000-000000000013'::uuid
      )
  ) then
    raise exception 'Username seed demo sudah dipakai oleh user lain. Bersihkan atau ganti username sebelum menjalankan seed ini.';
  end if;
end $$;

delete from sessions
where session_id in (
  '91000000-0000-0000-0000-000000000001'::uuid,
  '91000000-0000-0000-0000-000000000002'::uuid
);

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
    'seed.instructor',
    'Seed Instructor',
    crypt('SeedLocal!2026', gen_salt('bf', 10)),
    'INSTRUCTOR',
    true,
    '2026-01-01T00:00:00Z'
  ),
  (
    '90000000-0000-0000-0000-000000000011'::uuid,
    'seed.player1',
    'Seed Player 1',
    crypt('SeedLocal!2026', gen_salt('bf', 10)),
    'PLAYER',
    true,
    '2026-01-01T00:00:00Z'
  ),
  (
    '90000000-0000-0000-0000-000000000012'::uuid,
    'seed.player2',
    'Seed Player 2',
    crypt('SeedLocal!2026', gen_salt('bf', 10)),
    'PLAYER',
    true,
    '2026-01-01T00:00:00Z'
  ),
  (
    '90000000-0000-0000-0000-000000000013'::uuid,
    'seed.player3',
    'Seed Player 3',
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
    '[SEED] Simulasi Pemula',
    'PEMULA',
    'ENDED',
    3,
    '2026-01-05T01:00:00Z',
    '2026-01-29T02:00:00Z',
    '90000000-0000-0000-0000-000000000001'::uuid,
    '2026-01-05T00:50:00Z'
  ),
  (
    '91000000-0000-0000-0000-000000000002'::uuid,
    '[SEED] Simulasi Mahir',
    'MAHIR',
    'ENDED',
    3,
    '2026-02-02T01:00:00Z',
    '2026-02-26T02:00:00Z',
    '90000000-0000-0000-0000-000000000001'::uuid,
    '2026-02-02T00:50:00Z'
  );

insert into session_ruleset_activations (
  activation_id,
  session_id,
  ruleset_version_id,
  activated_at,
  activated_by
)
values
  (
    '92000000-0000-0000-0000-000000000001'::uuid,
    '91000000-0000-0000-0000-000000000001'::uuid,
    'f5b4c67b-0825-4970-9f07-3b68e8fcb524'::uuid,
    '2026-01-05T00:55:00Z',
    'seed.instructor'
  ),
  (
    '92000000-0000-0000-0000-000000000002'::uuid,
    '91000000-0000-0000-0000-000000000002'::uuid,
    '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d'::uuid,
    '2026-02-02T00:55:00Z',
    'seed.instructor'
  );

insert into session_players (
  session_player_id,
  session_id,
  user_id,
  player_index,
  join_order,
  role,
  created_at
)
values
  (
    '93000000-0000-0000-0000-000000000011'::uuid,
    '91000000-0000-0000-0000-000000000001'::uuid,
    '90000000-0000-0000-0000-000000000011'::uuid,
    1,
    1,
    'PLAYER',
    '2026-01-05T00:58:00Z'
  ),
  (
    '93000000-0000-0000-0000-000000000012'::uuid,
    '91000000-0000-0000-0000-000000000001'::uuid,
    '90000000-0000-0000-0000-000000000012'::uuid,
    2,
    2,
    'PLAYER',
    '2026-01-05T00:58:00Z'
  ),
  (
    '93000000-0000-0000-0000-000000000013'::uuid,
    '91000000-0000-0000-0000-000000000001'::uuid,
    '90000000-0000-0000-0000-000000000013'::uuid,
    3,
    3,
    'PLAYER',
    '2026-01-05T00:58:00Z'
  ),
  (
    '93000000-0000-0000-0000-000000000021'::uuid,
    '91000000-0000-0000-0000-000000000002'::uuid,
    '90000000-0000-0000-0000-000000000011'::uuid,
    1,
    1,
    'PLAYER',
    '2026-02-02T00:58:00Z'
  ),
  (
    '93000000-0000-0000-0000-000000000022'::uuid,
    '91000000-0000-0000-0000-000000000002'::uuid,
    '90000000-0000-0000-0000-000000000012'::uuid,
    2,
    2,
    'PLAYER',
    '2026-02-02T00:58:00Z'
  ),
  (
    '93000000-0000-0000-0000-000000000023'::uuid,
    '91000000-0000-0000-0000-000000000002'::uuid,
    '90000000-0000-0000-0000-000000000013'::uuid,
    3,
    3,
    'PLAYER',
    '2026-02-02T00:58:00Z'
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
      ('MAHIR', 1, '93000000-0000-0000-0000-000000000021'::uuid, '90000000-0000-0000-0000-000000000011'::uuid),
      ('MAHIR', 2, '93000000-0000-0000-0000-000000000022'::uuid, '90000000-0000-0000-0000-000000000012'::uuid),
      ('MAHIR', 3, '93000000-0000-0000-0000-000000000023'::uuid, '90000000-0000-0000-0000-000000000013'::uuid)
  ) as x(session_key, player_no, session_player_id, user_id)
),
event_seed as (
  select *
  from (
    values
      -- PEMULA: 25 hari kalender kerja.
      ('PEMULA', null, 0, 0, 1, null, 'SYSTEM', null, 'session.started', '{"start_note":"Mulai sesi seed pemula 25 hari"}'::jsonb),
      ('PEMULA', null, 0, 1, 1, 1, 'SYSTEM', 'MissionAssigned', 'mission.assigned', '{"mission_id":"misi_boneka","target_tertiary_card_id":"boneka","penalty_points":10,"require_primary":true,"require_secondary":true}'::jsonb),
      ('PEMULA', null, 0, 2, 1, 2, 'SYSTEM', 'MissionAssigned', 'mission.assigned', '{"mission_id":"misi_gameboy","target_tertiary_card_id":"gameboy","penalty_points":10,"require_primary":true,"require_secondary":true}'::jsonb),
      ('PEMULA', null, 0, 3, 1, 3, 'SYSTEM', 'MissionAssigned', 'mission.assigned', '{"mission_id":"misi_hiburan","target_tertiary_card_id":"hiburan","penalty_points":10,"require_primary":true,"require_secondary":true}'::jsonb),
      ('PEMULA', null, 0, 4, 1, 1, 'PLAYER', 'Kebutuhan', 'need.primary.purchased', '{"card_id":"buku","amount":2,"points":1}'::jsonb),
      ('PEMULA', null, 0, 5, 1, 1, 'PLAYER', 'Kebutuhan', 'need.secondary.purchased', '{"card_id":"sepatu","amount":4,"points":2}'::jsonb),
      ('PEMULA', null, 1, 0, 2, 2, 'PLAYER', 'Kebutuhan', 'need.primary.purchased', '{"card_id":"buku","amount":2,"points":1}'::jsonb),
      ('PEMULA', null, 1, 1, 2, 2, 'PLAYER', 'Kebutuhan', 'need.tertiary.purchased', '{"card_id":"hiburan","amount":5,"points":2}'::jsonb),
      ('PEMULA', null, 2, 0, 3, 3, 'PLAYER', 'KerjaLepas', 'work.freelance.completed', '{"amount":1}'::jsonb),
      ('PEMULA', null, 2, 1, 3, 3, 'PLAYER', 'BahanMasakan', 'ingredient.purchased', '{"card_id":"nasi_putih","ingredient_name":"Nasi Putih","amount":1}'::jsonb),
      ('PEMULA', null, 3, 0, 4, 1, 'PLAYER', 'BahanMasakan', 'ingredient.purchased', '{"card_id":"sayur","ingredient_name":"Sayur","amount":2}'::jsonb),
      ('PEMULA', null, 3, 1, 4, 1, 'PLAYER', 'JualMasakan', 'order.claimed', '{"order_card_id":"resep-sayur-nasi","required_ingredient_card_ids":["nasi_putih","sayur"],"income":13}'::jsonb),
      ('PEMULA', null, 4, 0, 5, 1, 'PLAYER', 'JumatBerkah', 'day.friday.donation', '{"amount":4}'::jsonb),
      ('PEMULA', null, 4, 1, 5, 2, 'PLAYER', 'JumatBerkah', 'day.friday.donation', '{"amount":2}'::jsonb),
      ('PEMULA', null, 4, 2, 5, 3, 'PLAYER', 'JumatBerkah', 'day.friday.donation', '{"amount":1}'::jsonb),
      ('PEMULA', null, 5, 0, 6, 1, 'PLAYER', 'InvestasiEmas', 'day.saturday.gold_trade', '{"trade_type":"BUY","unit_price":6,"qty":1,"amount":6}'::jsonb),
      ('PEMULA', null, 5, 1, 6, 2, 'PLAYER', 'InvestasiEmas', 'day.saturday.gold_trade', '{"trade_type":"BUY","unit_price":6,"qty":1,"amount":6}'::jsonb),
      ('PEMULA', null, 6, 0, 7, null, 'SYSTEM', null, 'day.sunday.rest', '{"note":"Minggu libur, lanjut ke hari berikutnya"}'::jsonb),
      ('PEMULA', null, 7, 0, 8, 2, 'PLAYER', 'BahanMasakan', 'ingredient.purchased', '{"card_id":"tahu_tempe","ingredient_name":"Tahu Tempe","amount":2}'::jsonb),
      ('PEMULA', null, 7, 1, 8, 2, 'PLAYER', 'JualMasakan', 'order.claimed', '{"order_card_id":"resep-tempe","required_ingredient_card_ids":["tahu_tempe"],"income":7}'::jsonb),
      ('PEMULA', null, 8, 0, 9, 3, 'PLAYER', 'Kebutuhan', 'need.secondary.purchased', '{"card_id":"tas","amount":3,"points":2}'::jsonb),
      ('PEMULA', null, 8, 1, 9, 3, 'PLAYER', 'Kebutuhan', 'need.tertiary.purchased', '{"card_id":"gameboy","amount":6,"points":3}'::jsonb),
      ('PEMULA', null, 9, 0, 10, 1, 'PLAYER', 'KerjaLepas', 'work.freelance.completed', '{"amount":1}'::jsonb),
      ('PEMULA', null, 9, 1, 10, 1, 'PLAYER', 'BahanMasakan', 'ingredient.purchased', '{"card_id":"telur","ingredient_name":"Telur","amount":2}'::jsonb),
      ('PEMULA', null, 10, 0, 11, 2, 'PLAYER', 'KerjaLepas', 'work.freelance.completed', '{"amount":1}'::jsonb),
      ('PEMULA', null, 10, 1, 11, 2, 'PLAYER', 'Kebutuhan', 'need.secondary.purchased', '{"card_id":"jaket","amount":4,"points":2}'::jsonb),
      ('PEMULA', null, 11, 0, 12, 1, 'PLAYER', 'JumatBerkah', 'day.friday.donation', '{"amount":3}'::jsonb),
      ('PEMULA', null, 11, 1, 12, 2, 'PLAYER', 'JumatBerkah', 'day.friday.donation', '{"amount":4}'::jsonb),
      ('PEMULA', null, 11, 2, 12, 3, 'PLAYER', 'JumatBerkah', 'day.friday.donation', '{"amount":2}'::jsonb),
      ('PEMULA', null, 12, 0, 13, 1, 'PLAYER', 'JualEmas', 'day.saturday.gold_trade', '{"trade_type":"SELL","unit_price":8,"qty":1,"amount":8}'::jsonb),
      ('PEMULA', null, 13, 0, 14, null, 'SYSTEM', null, 'day.sunday.rest', '{"note":"Minggu libur, tidak ada aksi pemain"}'::jsonb),
      ('PEMULA', null, 14, 0, 15, 3, 'PLAYER', 'BahanMasakan', 'ingredient.purchased', '{"card_id":"daging","ingredient_name":"Daging","amount":4}'::jsonb),
      ('PEMULA', null, 14, 1, 15, 3, 'PLAYER', 'JualMasakan', 'order.claimed', '{"order_card_id":"resep-daging","required_ingredient_card_ids":["daging","telur"],"income":16}'::jsonb),
      ('PEMULA', null, 15, 0, 16, 1, 'PLAYER', 'Kebutuhan', 'need.tertiary.purchased', '{"card_id":"boneka","amount":6,"points":3}'::jsonb),
      ('PEMULA', null, 16, 0, 17, 2, 'PLAYER', 'BahanMasakan', 'ingredient.purchased', '{"card_id":"sayur","ingredient_name":"Sayur","amount":2}'::jsonb),
      ('PEMULA', null, 17, 0, 18, 3, 'PLAYER', 'KerjaLepas', 'work.freelance.completed', '{"amount":1}'::jsonb),
      ('PEMULA', null, 17, 1, 18, 3, 'PLAYER', 'Kebutuhan', 'need.primary.purchased', '{"card_id":"buku","amount":2,"points":1}'::jsonb),
      ('PEMULA', null, 18, 0, 19, 1, 'PLAYER', 'JumatBerkah', 'day.friday.donation', '{"amount":5}'::jsonb),
      ('PEMULA', null, 18, 1, 19, 2, 'PLAYER', 'JumatBerkah', 'day.friday.donation', '{"amount":1}'::jsonb),
      ('PEMULA', null, 18, 2, 19, 3, 'PLAYER', 'JumatBerkah', 'day.friday.donation', '{"amount":3}'::jsonb),
      ('PEMULA', null, 19, 0, 20, 3, 'PLAYER', 'InvestasiEmas', 'day.saturday.gold_trade', '{"trade_type":"BUY","unit_price":7,"qty":1,"amount":7}'::jsonb),
      ('PEMULA', null, 20, 0, 21, null, 'SYSTEM', null, 'day.sunday.rest', '{"note":"Minggu libur menjelang akhir simulasi"}'::jsonb),
      ('PEMULA', null, 21, 0, 22, 1, 'PLAYER', 'order.passed', 'order.passed', '{"order_card_id":"resep-sulit","required_ingredient_card_ids":["daging","sayur","telur"],"income":18}'::jsonb),
      ('PEMULA', null, 22, 0, 23, 2, 'PLAYER', 'Kebutuhan', 'need.tertiary.purchased', '{"card_id":"hiburan","amount":5,"points":2}'::jsonb),
      ('PEMULA', null, 23, 0, 24, 3, 'PLAYER', 'ingredient.discarded', 'ingredient.discarded', '{"card_id":"sayur","amount":1,"reason":"HAND_LIMIT"}'::jsonb),
      ('PEMULA', null, 24, 0, 25, 1, 'PLAYER', 'KerjaLepas', 'work.freelance.completed', '{"amount":1}'::jsonb),
      ('PEMULA', null, 24, 1, 25, null, 'SYSTEM', 'SessionEnded', 'session.ended', '{"end_note":"Selesai 25 hari seed pemula"}'::jsonb),

      -- MAHIR: 25 hari kalender kerja dengan pinjaman, asuransi, risiko, dan tabungan.
      ('MAHIR', null, 0, 0, 1, null, 'SYSTEM', null, 'session.started', '{"start_note":"Mulai sesi seed mahir 25 hari"}'::jsonb),
      ('MAHIR', null, 0, 1, 1, 1, 'SYSTEM', 'MissionAssigned', 'mission.assigned', '{"mission_id":"misi_boneka","target_tertiary_card_id":"boneka","penalty_points":10,"require_primary":true,"require_secondary":true}'::jsonb),
      ('MAHIR', null, 0, 2, 1, 2, 'SYSTEM', 'MissionAssigned', 'mission.assigned', '{"mission_id":"misi_gameboy","target_tertiary_card_id":"gameboy","penalty_points":10,"require_primary":true,"require_secondary":true}'::jsonb),
      ('MAHIR', null, 0, 3, 1, 3, 'SYSTEM', 'MissionAssigned', 'mission.assigned', '{"mission_id":"misi_hiburan","target_tertiary_card_id":"hiburan","penalty_points":10,"require_primary":true,"require_secondary":true}'::jsonb),
      ('MAHIR', null, 0, 4, 1, 1, 'PLAYER', 'PinjamanSyariah', 'loan.syariah.taken', '{"loan_id":"loan-seed-mahir-001","principal":10,"installment":2,"duration_turn":5,"penalty_points":15}'::jsonb),
      ('MAHIR', null, 0, 5, 1, 1, 'PLAYER', 'Asuransi', 'insurance.multirisk.purchased', '{"policy_id":"INS-SEED-001","premium":1,"coverage_type":"MULTIRISK"}'::jsonb),
      ('MAHIR', null, 1, 0, 2, 1, 'PLAYER', 'Kebutuhan', 'need.primary.purchased', '{"card_id":"buku","amount":2,"points":1}'::jsonb),
      ('MAHIR', null, 1, 1, 2, 1, 'PLAYER', 'Kebutuhan', 'need.secondary.purchased', '{"card_id":"sepatu","amount":4,"points":2}'::jsonb),
      ('MAHIR', null, 2, 0, 3, 2, 'PLAYER', 'PinjamanSyariah', 'loan.syariah.taken', '{"loan_id":"loan-seed-mahir-002","principal":10,"installment":2,"duration_turn":5,"penalty_points":15}'::jsonb),
      ('MAHIR', null, 2, 1, 3, 2, 'PLAYER', 'Asuransi', 'insurance.multirisk.purchased', '{"policy_id":"INS-SEED-002","premium":1,"coverage_type":"MULTIRISK"}'::jsonb),
      ('MAHIR', null, 3, 0, 4, 1, 'PLAYER', 'BahanMasakan', 'ingredient.purchased', '{"card_id":"nasi_putih","ingredient_name":"Nasi Putih","amount":1}'::jsonb),
      ('MAHIR', null, 3, 1, 4, 1, 'PLAYER', 'JualMasakan', 'order.claimed', '{"order_card_id":"resep-nasi","required_ingredient_card_ids":["nasi_putih"],"income":6}'::jsonb),
      ('MAHIR', null, 4, 0, 5, 1, 'PLAYER', 'JumatBerkah', 'day.friday.donation', '{"amount":2}'::jsonb),
      ('MAHIR', null, 4, 1, 5, 2, 'PLAYER', 'JumatBerkah', 'day.friday.donation', '{"amount":1}'::jsonb),
      ('MAHIR', null, 4, 2, 5, 3, 'PLAYER', 'JumatBerkah', 'day.friday.donation', '{"amount":4}'::jsonb),
      ('MAHIR', null, 5, 0, 6, 1, 'PLAYER', 'InvestasiEmas', 'day.saturday.gold_trade', '{"trade_type":"BUY","unit_price":6,"qty":1,"amount":6}'::jsonb),
      ('MAHIR', null, 5, 1, 6, 2, 'PLAYER', 'InvestasiEmas', 'day.saturday.gold_trade', '{"trade_type":"BUY","unit_price":6,"qty":1,"amount":6}'::jsonb),
      ('MAHIR', null, 6, 0, 7, null, 'SYSTEM', null, 'day.sunday.rest', '{"note":"Minggu libur, lanjut ke hari berikutnya"}'::jsonb),
      ('MAHIR', 'mahir-risk-hospital', 7, 0, 8, 1, 'PLAYER', 'RisikoKehidupan', 'risk.life.drawn', '{"risk_id":"risk_hospital","direction":"OUT","amount":3,"note":"Biaya kesehatan"}'::jsonb),
      ('MAHIR', null, 7, 1, 8, 1, 'PLAYER', 'Asuransi', 'insurance.multirisk.used', '{"risk_event_ref":"mahir-risk-hospital"}'::jsonb),
      ('MAHIR', null, 8, 0, 9, 1, 'PLAYER', 'Menabung', 'saving.deposit.created', '{"goal_id":"beli_motor","amount":8}'::jsonb),
      ('MAHIR', null, 8, 1, 9, 1, 'PLAYER', 'TujuanFinansial', 'saving.goal.achieved', '{"goal_id":"beli_motor","points":4,"cost":8}'::jsonb),
      ('MAHIR', null, 9, 0, 10, 2, 'PLAYER', 'Kebutuhan', 'need.primary.purchased', '{"card_id":"buku","amount":2,"points":1}'::jsonb),
      ('MAHIR', null, 9, 1, 10, 2, 'PLAYER', 'Kebutuhan', 'need.tertiary.purchased', '{"card_id":"boneka","amount":6,"points":3}'::jsonb),
      ('MAHIR', null, 10, 0, 11, 3, 'PLAYER', 'PinjamanSyariah', 'loan.syariah.taken', '{"loan_id":"loan-seed-mahir-003","principal":10,"installment":2,"duration_turn":5,"penalty_points":15}'::jsonb),
      ('MAHIR', null, 10, 1, 11, 3, 'PLAYER', 'Asuransi', 'insurance.multirisk.purchased', '{"policy_id":"INS-SEED-003","premium":1,"coverage_type":"MULTIRISK"}'::jsonb),
      ('MAHIR', null, 11, 0, 12, 1, 'PLAYER', 'JumatBerkah', 'day.friday.donation', '{"amount":3}'::jsonb),
      ('MAHIR', null, 11, 1, 12, 2, 'PLAYER', 'JumatBerkah', 'day.friday.donation', '{"amount":5}'::jsonb),
      ('MAHIR', null, 11, 2, 12, 3, 'PLAYER', 'JumatBerkah', 'day.friday.donation', '{"amount":2}'::jsonb),
      ('MAHIR', null, 12, 0, 13, 3, 'PLAYER', 'InvestasiEmas', 'day.saturday.gold_trade', '{"trade_type":"BUY","unit_price":7,"qty":1,"amount":7}'::jsonb),
      ('MAHIR', null, 13, 0, 14, null, 'SYSTEM', null, 'day.sunday.rest', '{"note":"Minggu libur, tidak ada aksi pemain"}'::jsonb),
      ('MAHIR', null, 14, 0, 15, 1, 'PLAYER', 'BayarPinjaman', 'loan.syariah.repaid', '{"loan_id":"loan-seed-mahir-001","amount":10}'::jsonb),
      ('MAHIR', null, 15, 0, 16, 2, 'PLAYER', 'BahanMasakan', 'ingredient.purchased', '{"card_id":"sayur","ingredient_name":"Sayur","amount":2}'::jsonb),
      ('MAHIR', null, 15, 1, 16, 2, 'PLAYER', 'JualMasakan', 'order.claimed', '{"order_card_id":"resep-sayur","required_ingredient_card_ids":["sayur"],"income":7}'::jsonb),
      ('MAHIR', null, 16, 0, 17, 2, 'PLAYER', 'RisikoKehidupan', 'risk.life.drawn', '{"risk_id":"risk_dompet","direction":"OUT","amount":2,"note":"Dompet tertinggal"}'::jsonb),
      ('MAHIR', null, 16, 1, 17, 2, 'PLAYER', 'risk.emergency.used', 'risk.emergency.used', '{"risk_event_id":"risk_dompet_manual","option_type":"SELL_NEED","direction":"IN","amount":2,"note":"Menjual kebutuhan darurat"}'::jsonb),
      ('MAHIR', null, 17, 0, 18, 3, 'PLAYER', 'Menabung', 'saving.deposit.created', '{"goal_id":"dana_pensiun","amount":6}'::jsonb),
      ('MAHIR', null, 18, 0, 19, 1, 'PLAYER', 'JumatBerkah', 'day.friday.donation', '{"amount":4}'::jsonb),
      ('MAHIR', null, 18, 1, 19, 2, 'PLAYER', 'JumatBerkah', 'day.friday.donation', '{"amount":3}'::jsonb),
      ('MAHIR', null, 18, 2, 19, 3, 'PLAYER', 'JumatBerkah', 'day.friday.donation', '{"amount":1}'::jsonb),
      ('MAHIR', null, 19, 0, 20, 1, 'PLAYER', 'JualEmas', 'day.saturday.gold_trade', '{"trade_type":"SELL","unit_price":8,"qty":1,"amount":8}'::jsonb),
      ('MAHIR', null, 20, 0, 21, null, 'SYSTEM', null, 'day.sunday.rest', '{"note":"Minggu libur menjelang akhir simulasi"}'::jsonb),
      ('MAHIR', null, 21, 0, 22, 3, 'PLAYER', 'Kebutuhan', 'need.primary.purchased', '{"card_id":"buku","amount":2,"points":1}'::jsonb),
      ('MAHIR', null, 21, 1, 22, 3, 'PLAYER', 'Kebutuhan', 'need.secondary.purchased', '{"card_id":"jaket","amount":4,"points":2}'::jsonb),
      ('MAHIR', null, 22, 0, 23, 2, 'PLAYER', 'BayarPinjaman', 'loan.syariah.repaid', '{"loan_id":"loan-seed-mahir-002","amount":10}'::jsonb),
      ('MAHIR', null, 23, 0, 24, 3, 'PLAYER', 'BayarPinjaman', 'loan.syariah.repaid', '{"loan_id":"loan-seed-mahir-003","amount":10}'::jsonb),
      ('MAHIR', null, 24, 0, 25, 3, 'PLAYER', 'TujuanFinansial', 'saving.goal.achieved', '{"goal_id":"dana_pensiun","points":5,"cost":6}'::jsonb),
      ('MAHIR', null, 24, 1, 25, null, 'SYSTEM', 'SessionEnded', 'session.ended', '{"end_note":"Selesai 25 hari seed mahir"}'::jsonb)
  ) as x(session_key, ref_key, day_index, event_order, turn_number, player_no, actor_type, action_id, action_type, payload)
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
    es.turn_number,
    es.actor_type,
    es.action_id,
    es.action_type,
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
    e.sequence_number,
    e.action_id,
    e.action_type,
    e.ruleset_version_id,
    case
      when e.action_type = 'insurance.multirisk.used' then
        jsonb_build_object('risk_event_id', risk_event.event_id::text)
      else e.payload
    end as payload
  from numbered_events e
  left join numbered_events risk_event
    on risk_event.session_key = e.session_key
   and risk_event.ref_key = e.payload->>'risk_event_ref'
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
  sequence_number,
  action_id,
  action_type,
  ruleset_version_id,
  payload,
  received_at,
  client_request_id
)
select
  event_id,
  event_id,
  session_id,
  session_player_id,
  user_id,
  actor_type,
  event_timestamp,
  day_index,
  weekday,
  turn_number,
  sequence_number,
  action_id,
  action_type,
  ruleset_version_id,
  payload,
  event_timestamp,
  null
from resolved_events
order by session_id, sequence_number;

with cashflow_candidates as (
  select
    e.event_pk,
    e.event_id,
    e.session_id,
    e.user_id,
    e.timestamp,
    e.sequence_number,
    case
      when e.action_type = 'transaction.recorded' then upper(nullif(e.payload->>'direction', ''))
      when e.action_type = 'day.friday.donation' then 'OUT'
      when e.action_type = 'day.saturday.gold_trade' and upper(e.payload->>'trade_type') = 'BUY' then 'OUT'
      when e.action_type = 'day.saturday.gold_trade' and upper(e.payload->>'trade_type') = 'SELL' then 'IN'
      when e.action_type = 'ingredient.purchased' then 'OUT'
      when e.action_type = 'order.claimed' then 'IN'
      when e.action_type = 'work.freelance.completed' then 'IN'
      when e.action_type in ('need.primary.purchased', 'need.secondary.purchased', 'need.tertiary.purchased') then 'OUT'
      when e.action_type = 'saving.deposit.created' then 'OUT'
      when e.action_type = 'saving.deposit.withdrawn' then 'IN'
      when e.action_type = 'risk.life.drawn' then upper(nullif(e.payload->>'direction', ''))
      when e.action_type = 'loan.syariah.taken' then 'IN'
      when e.action_type = 'loan.syariah.repaid' then 'OUT'
      when e.action_type = 'insurance.multirisk.purchased' then 'OUT'
      when e.action_type = 'risk.emergency.used' then upper(nullif(e.payload->>'direction', ''))
      else null
    end as direction,
    case
      when e.action_type = 'loan.syariah.taken' then round((e.payload->>'principal')::numeric)::int
      when e.action_type = 'order.claimed' then round((e.payload->>'income')::numeric)::int
      when e.action_type = 'insurance.multirisk.purchased' then round((e.payload->>'premium')::numeric)::int
      else round((e.payload->>'amount')::numeric)::int
    end as amount,
    case
      when e.action_type = 'transaction.recorded' then upper(coalesce(nullif(e.payload->>'category', ''), 'TRANSACTION'))
      when e.action_type = 'day.friday.donation' then 'DONATION'
      when e.action_type = 'day.saturday.gold_trade' then 'GOLD_TRADE'
      when e.action_type = 'ingredient.purchased' then 'INGREDIENT'
      when e.action_type = 'order.claimed' then 'ORDER'
      when e.action_type = 'work.freelance.completed' then 'FREELANCE'
      when e.action_type = 'need.primary.purchased' then 'NEED_PRIMARY'
      when e.action_type = 'need.secondary.purchased' then 'NEED_SECONDARY'
      when e.action_type = 'need.tertiary.purchased' then 'NEED_TERTIARY'
      when e.action_type = 'saving.deposit.created' then 'SAVING_DEPOSIT'
      when e.action_type = 'saving.deposit.withdrawn' then 'SAVING_WITHDRAW'
      when e.action_type = 'risk.life.drawn' then 'RISK_LIFE'
      when e.action_type = 'loan.syariah.taken' then 'LOAN_TAKEN'
      when e.action_type = 'loan.syariah.repaid' then 'LOAN_REPAID'
      when e.action_type = 'insurance.multirisk.purchased' then 'INSURANCE_PREMIUM'
      when e.action_type = 'risk.emergency.used' then 'EMERGENCY_OPTION'
      else null
    end as category,
    nullif(e.payload->>'counterparty', '') as counterparty,
    coalesce(
      nullif(e.payload->>'loan_id', ''),
      nullif(e.payload->>'policy_id', ''),
      nullif(e.payload->>'goal_id', ''),
      nullif(e.payload->>'order_card_id', ''),
      nullif(e.payload->>'card_id', ''),
      nullif(e.payload->>'risk_id', ''),
      nullif(e.payload->>'risk_event_id', '')
    ) as reference,
    nullif(e.payload->>'note', '') as note
  from events e
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
  timestamp,
  direction,
  amount,
  category,
  counterparty,
  reference,
  note
from cashflow_rows
order by session_id, projection_number
on conflict (session_id, event_id) do nothing;

with seed_sessions as (
  select
    s.session_id,
    s.session_name,
    s.mode,
    s.ended_at,
    sra.ruleset_version_id,
    case when s.mode = 'MAHIR' then 10 else 20 end as starting_coins,
    case when s.mode = 'MAHIR' then 'advanced' else 'beginner' end as gameplay_mode
  from sessions s
  join session_ruleset_activations sra on sra.session_id = s.session_id
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
    sp.session_player_id,
    sp.user_id,
    sp.player_index,
    au.display_name
  from seed_sessions ss
  join session_players sp on sp.session_id = ss.session_id
  join app_users au on au.user_id = sp.user_id
),
event_agg as (
  select
    pb.session_id,
    pb.user_id,
    count(e.event_pk) filter (where e.actor_type = 'PLAYER')::int as player_event_count,
    count(e.event_pk)::int as event_count,
    coalesce(max(e.turn_number), 0)::int as latest_turn_number,
    coalesce(max(e.day_index), 1)::int as latest_day_index,
    coalesce(max(e.weekday), 'MON') as latest_weekday,
    coalesce(max(e.timestamp), pb.ended_at) as latest_event_timestamp,
    coalesce(jsonb_agg(
      jsonb_build_object(
        'day_index', e.day_index,
        'turn_number', e.turn_number,
        'sequence_number', e.sequence_number,
        'action_type', e.action_type
      )
      order by e.sequence_number
    ) filter (where e.actor_type = 'PLAYER'), '[]'::jsonb) as action_sequence,
    coalesce(sum(case when e.action_type = 'ingredient.purchased' then (e.payload->>'amount')::int else 0 end), 0)::int as ingredients_collected,
    coalesce(sum(case when e.action_type = 'ingredient.discarded' then (e.payload->>'amount')::int else 0 end), 0)::int as ingredients_wasted,
    count(*) filter (where e.action_type = 'order.claimed')::int as meal_orders_claimed,
    count(*) filter (where e.action_type = 'order.passed')::int as meal_orders_passed,
    coalesce(sum(case when e.action_type = 'order.claimed' then (e.payload->>'income')::int else 0 end), 0)::int as meal_order_income_total,
    count(*) filter (where e.action_type = 'need.primary.purchased')::int as primary_needs_owned,
    count(*) filter (where e.action_type = 'need.secondary.purchased')::int as secondary_needs_owned,
    count(*) filter (where e.action_type = 'need.tertiary.purchased')::int as tertiary_needs_owned,
    coalesce(sum(case when e.action_type in ('need.primary.purchased', 'need.secondary.purchased', 'need.tertiary.purchased') then (e.payload->>'amount')::int else 0 end), 0)::int as need_cards_coins_spent,
    coalesce(sum(case when e.action_type in ('need.primary.purchased', 'need.secondary.purchased', 'need.tertiary.purchased') then (e.payload->>'points')::int else 0 end), 0)::int as need_points,
    coalesce(sum(case when e.action_type = 'day.friday.donation' then (e.payload->>'amount')::int else 0 end), 0)::int as donation_total_coins,
    count(*) filter (where e.action_type = 'day.friday.donation')::int as donation_events,
    coalesce(sum(case when e.action_type = 'day.saturday.gold_trade' and upper(e.payload->>'trade_type') = 'BUY' then (e.payload->>'qty')::int else 0 end), 0)::int as gold_cards_purchased,
    coalesce(sum(case when e.action_type = 'day.saturday.gold_trade' and upper(e.payload->>'trade_type') = 'SELL' then (e.payload->>'qty')::int else 0 end), 0)::int as gold_cards_sold,
    coalesce(sum(case when e.action_type = 'day.saturday.gold_trade' and upper(e.payload->>'trade_type') = 'BUY' then (e.payload->>'amount')::int else 0 end), 0)::int as gold_investment_coins_spent,
    coalesce(sum(case when e.action_type = 'day.saturday.gold_trade' and upper(e.payload->>'trade_type') = 'SELL' then (e.payload->>'amount')::int else 0 end), 0)::int as gold_investment_coins_earned,
    count(*) filter (where e.action_type = 'risk.life.drawn')::int as life_risk_cards_drawn,
    coalesce(sum(case when e.action_type = 'risk.life.drawn' and upper(coalesce(e.payload->>'direction', '')) = 'OUT' then (e.payload->>'amount')::int else 0 end), 0)::int as life_risk_costs_total,
    count(*) filter (where e.action_type = 'insurance.multirisk.used')::int as life_risk_mitigated_with_insurance,
    count(*) filter (where e.action_type = 'insurance.multirisk.purchased')::int as insurance_payments_made,
    count(*) filter (where e.action_type = 'risk.emergency.used')::int as emergency_options_used,
    count(*) filter (where e.action_type = 'saving.deposit.created')::int as financial_goals_attempted,
    count(*) filter (where e.action_type = 'saving.goal.achieved')::int as financial_goals_completed,
    coalesce(sum(case when e.action_type = 'saving.deposit.created' then (e.payload->>'amount')::int else 0 end), 0)::int as coins_saved,
    coalesce(sum(case when e.action_type = 'saving.goal.achieved' then (e.payload->>'points')::int else 0 end), 0)::int as saving_goal_points,
    count(*) filter (where e.action_type = 'loan.syariah.taken')::int as sharia_loans_taken,
    count(*) filter (where e.action_type = 'loan.syariah.repaid')::int as sharia_loans_repaid,
    coalesce(sum(case when e.action_type = 'loan.syariah.taken' then (e.payload->>'principal')::int else 0 end), 0)::int as sharia_loan_principal_total,
    coalesce(sum(case when e.action_type = 'loan.syariah.repaid' then (e.payload->>'amount')::int else 0 end), 0)::int as sharia_loan_repaid_total,
    count(*) filter (where e.action_type = 'mission.assigned')::int as mission_assigned_count
  from player_base pb
  left join events e
    on e.session_id = pb.session_id
   and e.user_id = pb.user_id
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
    ea.latest_turn_number,
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
        'turn_number', latest_turn_number,
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
        'meal_orders_per_turn_average', round(meal_orders_claimed::numeric / greatest(latest_turn_number, 1), 2)
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
        'turn_number_game_completion', latest_turn_number,
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
      'planning_horizon', latest_turn_number,
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
    metric_payload_json
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
  ruleset_version_id
)
select
  ('97000000-0000-0000-0000-' || lpad(snapshot_number::text, 12, '0'))::uuid,
  session_id,
  user_id,
  session_player_id,
  computed_at,
  metric_name,
  null,
  metric_payload_json,
  ruleset_version_id
from numbered_snapshots
order by snapshot_number;

commit;
