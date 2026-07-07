begin;

-- Seed simulasi baseline dengan trigger dan validasi aktif.
do $$ begin if not exists (
  select
    1
  from
    pg_extension
  where
    extname = 'pgcrypto'
) then raise exception 'pgcrypto belum aktif. Jalankan database/00_create_schema.sql terlebih dahulu.';

end if;

if not exists (
  select
    1
  from
    ruleset_versions
  where
    ruleset_version_id = 'f5b4c67b-0825-4970-9f07-3b68e8fcb524' :: uuid
    and mode = 'PEMULA'
) then raise exception 'Ruleset default PEMULA dari 01_seed_default_rulesets_components.sql tidak ditemukan.';

end if;

if not exists (
  select
    1
  from
    ruleset_versions
  where
    ruleset_version_id = '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid
    and mode = 'MAHIR'
) then raise exception 'Ruleset default MAHIR dari 01_seed_default_rulesets_components.sql tidak ditemukan.';

end if;

if exists (
  select
    1
  from
    app_users
  where
    lower(username) in (
      'rina.kartika',
      'marco',
      'marcello',
      'hugo',
      'manalu'
    )
    and user_id not in (
      '90000000-0000-0000-0000-000000000001' :: uuid,
      '90000000-0000-0000-0000-000000000011' :: uuid,
      '90000000-0000-0000-0000-000000000012' :: uuid,
      '90000000-0000-0000-0000-000000000013' :: uuid,
      '90000000-0000-0000-0000-000000000014' :: uuid
    )
) then raise exception 'Username seed demo sudah dipakai oleh user lain. Bersihkan atau ganti username sebelum menjalankan seed ini.';

end if;

end $$;

drop table if exists seed_session_scope;

create temporary table seed_session_scope (session_id uuid primary key) on commit drop;

insert into
  seed_session_scope (session_id)
values
  ('91000000-0000-0000-0000-000000000001' :: uuid),
  ('91000000-0000-0000-0000-000000000002' :: uuid);

update
  sessions
set
  status = 'CREATED',
  player_count = 0,
  started_at = null,
  ended_at = null
where
  session_id in (
    select
      session_id
    from
      seed_session_scope
  );

delete from
  session_final_score_components
where
  session_id in (
    select
      session_id
    from
      seed_session_scope
  );

delete from
  session_final_scores
where
  session_id in (
    select
      session_id
    from
      seed_session_scope
  );

delete from
  metric_snapshots
where
  session_id in (
    select
      session_id
    from
      seed_session_scope
  );

delete from
  event_cashflow_projections
where
  session_id in (
    select
      session_id
    from
      seed_session_scope
  );

delete from
  session_rule_effects
where
  session_id in (
    select
      session_id
    from
      seed_session_scope
  );

delete from
  event_asset_references
where
  session_id in (
    select
      session_id
    from
      seed_session_scope
  );

delete from
  session_narrative_logs
where
  session_id in (
    select
      session_id
    from
      seed_session_scope
  );

delete from
  session_projection_checkpoints
where
  session_id in (
    select
      session_id
    from
      seed_session_scope
  );

delete from
  session_card_positions
where
  session_id in (
    select
      session_id
    from
      seed_session_scope
  );

delete from
  session_donation_events
where
  session_id in (
    select
      session_id
    from
      seed_session_scope
  );

delete from
  session_participant_action_counters
where
  session_participant_id in (
    select
      sp.session_participant_id
    from
      session_participants sp
      join seed_session_scope ss on ss.session_id = sp.session_id
  );

delete from
  session_participant_collection_missions
where
  session_id in (
    select
      session_id
    from
      seed_session_scope
  );

delete from
  session_participant_financial_goals
where
  session_id in (
    select
      session_id
    from
      seed_session_scope
  );

delete from
  session_participant_need_purchases
where
  session_id in (
    select
      session_id
    from
      seed_session_scope
  );

delete from
  session_participant_inventory
where
  session_id in (
    select
      session_id
    from
      seed_session_scope
  );

delete from
  session_participant_gold_holdings
where
  session_id in (
    select
      session_id
    from
      seed_session_scope
  );

delete from
  session_participant_loans
where
  session_id in (
    select
      session_id
    from
      seed_session_scope
  );

delete from
  session_participant_insurances
where
  session_id in (
    select
      session_id
    from
      seed_session_scope
  );

delete from
  session_participant_tie_breakers
where
  session_id in (
    select
      session_id
    from
      seed_session_scope
  );

delete from
  session_participant_balances
where
  session_id in (
    select
      session_id
    from
      seed_session_scope
  );

delete from
  session_states
where
  session_id in (
    select
      session_id
    from
      seed_session_scope
  );

delete from
  validation_logs
where
  session_id in (
    select
      session_id
    from
      seed_session_scope
  );

delete from
  events
where
  session_id in (
    select
      session_id
    from
      seed_session_scope
  );

delete from
  session_participants
where
  session_id in (
    select
      session_id
    from
      seed_session_scope
  );

delete from
  sessions
where
  session_id in (
    select
      session_id
    from
      seed_session_scope
  );

insert into
  app_users (
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
    '90000000-0000-0000-0000-000000000001' :: uuid,
    'rina.kartika',
    'Ibu Rina Kartika, S.Pd.',
    crypt('SeedLocal!2026', gen_salt('bf', 10)),
    'INSTRUCTOR',
    true,
    '2026-01-01T00:00:00Z'
  ),
  (
    '90000000-0000-0000-0000-000000000011' :: uuid,
    'marco',
    'Marco',
    crypt('SeedLocal!2026', gen_salt('bf', 10)),
    'PLAYER',
    true,
    '2026-01-01T00:00:00Z'
  ),
  (
    '90000000-0000-0000-0000-000000000012' :: uuid,
    'marcello',
    'Marcello',
    crypt('SeedLocal!2026', gen_salt('bf', 10)),
    'PLAYER',
    true,
    '2026-01-01T00:00:00Z'
  ),
  (
    '90000000-0000-0000-0000-000000000013' :: uuid,
    'hugo',
    'Hugo',
    crypt('SeedLocal!2026', gen_salt('bf', 10)),
    'PLAYER',
    true,
    '2026-01-01T00:00:00Z'
  ),
  (
    '90000000-0000-0000-0000-000000000014' :: uuid,
    'manalu',
    'Manalu',
    crypt('SeedLocal!2026', gen_salt('bf', 10)),
    'PLAYER',
    true,
    '2026-01-01T00:00:00Z'
  ) on conflict (user_id) do
update
set
  username = excluded.username,
  display_name = excluded.display_name,
  password_hash = excluded.password_hash,
  role = excluded.role,
  is_active = excluded.is_active;

insert into
  sessions (
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
    '91000000-0000-0000-0000-000000000001' :: uuid,
    'Simulasi Cashflowpoly Kelas XI IPS 2 - Mode Pemula - Kelompok A',
    'f5b4c67b-0825-4970-9f07-3b68e8fcb524' :: uuid,
    'PEMULA',
    'CREATED',
    0,
    null,
    null,
    '90000000-0000-0000-0000-000000000001' :: uuid,
    '2026-01-05T00:50:00Z'
  ),
  (
    '91000000-0000-0000-0000-000000000002' :: uuid,
    'Simulasi Cashflowpoly Kelas XI IPS 2 - Mode Mahir - Kelompok B',
    '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
    'MAHIR',
    'CREATED',
    0,
    null,
    null,
    '90000000-0000-0000-0000-000000000001' :: uuid,
    '2026-02-02T00:50:00Z'
  );

insert into
  session_participants (
    session_participant_id,
    session_id,
    user_id,
    player_name,
    player_order_no,
    joined_at
  )
values
  (
    '93000000-0000-0000-0000-000000000011' :: uuid,
    '91000000-0000-0000-0000-000000000001' :: uuid,
    '90000000-0000-0000-0000-000000000011' :: uuid,
    'Marco',
    1,
    '2026-01-05T00:58:00Z'
  ),
  (
    '93000000-0000-0000-0000-000000000012' :: uuid,
    '91000000-0000-0000-0000-000000000001' :: uuid,
    '90000000-0000-0000-0000-000000000012' :: uuid,
    'Marcello',
    2,
    '2026-01-05T00:58:00Z'
  ),
  (
    '93000000-0000-0000-0000-000000000013' :: uuid,
    '91000000-0000-0000-0000-000000000001' :: uuid,
    '90000000-0000-0000-0000-000000000013' :: uuid,
    'Hugo',
    3,
    '2026-01-05T00:58:00Z'
  ),
  (
    '93000000-0000-0000-0000-000000000014' :: uuid,
    '91000000-0000-0000-0000-000000000001' :: uuid,
    '90000000-0000-0000-0000-000000000014' :: uuid,
    'Manalu',
    4,
    '2026-01-05T00:58:00Z'
  ),
  (
    '93000000-0000-0000-0000-000000000021' :: uuid,
    '91000000-0000-0000-0000-000000000002' :: uuid,
    '90000000-0000-0000-0000-000000000011' :: uuid,
    'Marco',
    1,
    '2026-02-02T00:58:00Z'
  ),
  (
    '93000000-0000-0000-0000-000000000022' :: uuid,
    '91000000-0000-0000-0000-000000000002' :: uuid,
    '90000000-0000-0000-0000-000000000012' :: uuid,
    'Marcello',
    2,
    '2026-02-02T00:58:00Z'
  ),
  (
    '93000000-0000-0000-0000-000000000023' :: uuid,
    '91000000-0000-0000-0000-000000000002' :: uuid,
    '90000000-0000-0000-0000-000000000013' :: uuid,
    'Hugo',
    3,
    '2026-02-02T00:58:00Z'
  ),
  (
    '93000000-0000-0000-0000-000000000024' :: uuid,
    '91000000-0000-0000-0000-000000000002' :: uuid,
    '90000000-0000-0000-0000-000000000014' :: uuid,
    'Manalu',
    4,
    '2026-02-02T00:58:00Z'
  );

update
  sessions
set
  status = 'ENDED',
  player_count = 4,
  started_at = case
    session_id
    when '91000000-0000-0000-0000-000000000001' :: uuid then '2026-01-05T01:00:00Z' :: timestamptz
    when '91000000-0000-0000-0000-000000000002' :: uuid then '2026-02-02T01:00:00Z' :: timestamptz
    else started_at
  end,
  ended_at = case
    session_id
    when '91000000-0000-0000-0000-000000000001' :: uuid then '2026-01-29T02:00:00Z' :: timestamptz
    when '91000000-0000-0000-0000-000000000002' :: uuid then '2026-02-26T02:00:00Z' :: timestamptz
    else ended_at
  end
where
  session_id in (
    '91000000-0000-0000-0000-000000000001' :: uuid,
    '91000000-0000-0000-0000-000000000002' :: uuid
  );

do $$ declare v_event record;

begin for v_event in with session_context as (
  select
    *
  from
    (
      values
        (
          'PEMULA',
          '91000000-0000-0000-0000-000000000001' :: uuid,
          'f5b4c67b-0825-4970-9f07-3b68e8fcb524' :: uuid,
          '95000000-0000-0000-0000-',
          '2026-01-05T01:00:00Z' :: timestamptz
        ),
        (
          'MAHIR',
          '91000000-0000-0000-0000-000000000002' :: uuid,
          '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
          '96000000-0000-0000-0000-',
          '2026-02-02T01:00:00Z' :: timestamptz
        )
    ) as x(
      session_key,
      session_id,
      ruleset_version_id,
      event_uuid_prefix,
      base_timestamp
    )
),
player_context as (
  select
    *
  from
    (
      values
        (
          'PEMULA',
          1,
          '93000000-0000-0000-0000-000000000011' :: uuid,
          '90000000-0000-0000-0000-000000000011' :: uuid
        ),
        (
          'PEMULA',
          2,
          '93000000-0000-0000-0000-000000000012' :: uuid,
          '90000000-0000-0000-0000-000000000012' :: uuid
        ),
        (
          'PEMULA',
          3,
          '93000000-0000-0000-0000-000000000013' :: uuid,
          '90000000-0000-0000-0000-000000000013' :: uuid
        ),
        (
          'PEMULA',
          4,
          '93000000-0000-0000-0000-000000000014' :: uuid,
          '90000000-0000-0000-0000-000000000014' :: uuid
        ),
        (
          'MAHIR',
          1,
          '93000000-0000-0000-0000-000000000021' :: uuid,
          '90000000-0000-0000-0000-000000000011' :: uuid
        ),
        (
          'MAHIR',
          2,
          '93000000-0000-0000-0000-000000000022' :: uuid,
          '90000000-0000-0000-0000-000000000012' :: uuid
        ),
        (
          'MAHIR',
          3,
          '93000000-0000-0000-0000-000000000023' :: uuid,
          '90000000-0000-0000-0000-000000000013' :: uuid
        ),
        (
          'MAHIR',
          4,
          '93000000-0000-0000-0000-000000000024' :: uuid,
          '90000000-0000-0000-0000-000000000014' :: uuid
        )
    ) as x(
      session_key,
      player_no,
      session_player_id,
      user_id
    )
),
scenario_event_seed as (
  select
    *
  from
    (
      values
        -- PEMULA: sesuai dokumen skenario 4 pemain selama 25 hari.
        (
          'PEMULA',
          null,
          0,
          0,
          1,
          null,
          'SYSTEM',
          null,
          'MulaiSesi',
          '{"start_note":"Mulai sesi simulasi pemula sesuai dokumen skenario"}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          1,
          -4,
          1,
          1,
          'SYSTEM',
          'SetupEmasAwal',
          'SetupEmasAwal',
          '{"qty":1,"setup":"INITIAL"}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          1,
          -3,
          1,
          2,
          'SYSTEM',
          'SetupEmasAwal',
          'SetupEmasAwal',
          '{"qty":1,"setup":"INITIAL"}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          1,
          -2,
          1,
          3,
          'SYSTEM',
          'SetupEmasAwal',
          'SetupEmasAwal',
          '{"qty":1,"setup":"INITIAL"}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          1,
          -1,
          1,
          4,
          'SYSTEM',
          'SetupEmasAwal',
          'SetupEmasAwal',
          '{"qty":1,"setup":"INITIAL"}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          0,
          1,
          1,
          1,
          'SYSTEM',
          'SetupMisiAwal',
          'SetupMisiAwal',
          '{"mission_id":"misi_boneka","target_tertiary_card_id":"boneka","penalty_points":10,"require_primary":true,"require_secondary":true}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          0,
          2,
          1,
          2,
          'SYSTEM',
          'SetupMisiAwal',
          'SetupMisiAwal',
          '{"mission_id":"misi_gameboy","target_tertiary_card_id":"gameboy","penalty_points":10,"require_primary":true,"require_secondary":true}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          0,
          3,
          1,
          3,
          'SYSTEM',
          'SetupMisiAwal',
          'SetupMisiAwal',
          '{"mission_id":"misi_hiburan","target_tertiary_card_id":"hiburan","penalty_points":10,"require_primary":true,"require_secondary":true}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          0,
          4,
          1,
          4,
          'SYSTEM',
          'SetupMisiAwal',
          'SetupMisiAwal',
          '{"mission_id":"misi_jam","target_tertiary_card_id":"jam","penalty_points":10,"require_primary":true,"require_secondary":true}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          0,
          5,
          1,
          1,
          'SYSTEM',
          'SetupBahanAwal',
          'SetupBahanAwal',
          '{"card_id":"nasi_putih","ingredient_name":"Nasi Putih","amount":1,"setup":"INITIAL"}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          0,
          7,
          1,
          2,
          'SYSTEM',
          'SetupBahanAwal',
          'SetupBahanAwal',
          '{"card_id":"nasi_putih","ingredient_name":"Nasi Putih","amount":1,"setup":"INITIAL"}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          0,
          9,
          1,
          3,
          'SYSTEM',
          'SetupBahanAwal',
          'SetupBahanAwal',
          '{"card_id":"daging","ingredient_name":"Daging","amount":5,"setup":"INITIAL"}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          0,
          12,
          1,
          4,
          'SYSTEM',
          'SetupBahanAwal',
          'SetupBahanAwal',
          '{"card_id":"sayur","ingredient_name":"Sayur","amount":2,"setup":"INITIAL"}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          0,
          13,
          1,
          null,
          'SYSTEM',
          'AmbilKartuDariDeck',
          'AmbilKartuDariDeck',
          '{"slot_group":"INGREDIENT_MARKET","slot_code":"SLOT_1","asset_type":"INGREDIENT","asset_code":"nasi_putih"}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          0,
          14,
          1,
          1,
          'SYSTEM',
          'KartuDiambilDariPasar',
          'KartuDiambilDariPasar',
          '{"slot_group":"INGREDIENT_MARKET","slot_code":"SLOT_1","asset_type":"INGREDIENT","asset_code":"nasi_putih"}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          0,
          15,
          1,
          null,
          'SYSTEM',
          'KartuMasukDiscard',
          'KartuMasukDiscard',
          '{"asset_type":"ORDER","asset_code":"lontong_balap"}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          0,
          16,
          1,
          null,
          'SYSTEM',
          'IsiUlangPasar',
          'IsiUlangPasar',
          '{"slot_group":"INGREDIENT_MARKET","slot_code":"SLOT_1","asset_type":"INGREDIENT","asset_code":"sayur"}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          1,
          0,
          2,
          1,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"telur","ingredient_name":"Telur","amount":4}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          1,
          1,
          2,
          1,
          'PLAYER',
          'JualMasakan',
          'JualMasakan',
          '{"order_card_id":"nasi_goreng","required_ingredient_card_ids":["nasi_putih","telur"],"income":15}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          1,
          2,
          2,
          2,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          1,
          3,
          2,
          2,
          'PLAYER',
          'Kebutuhan',
          'Kebutuhan',
          '{"card_id":"buku_1","amount":2,"points":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          1,
          4,
          2,
          3,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"tahu_tempe","ingredient_name":"Tahu Tempe","amount":3}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          1,
          5,
          2,
          3,
          'PLAYER',
          'JualMasakan',
          'JualMasakan',
          '{"order_card_id":"tahu_campur","required_ingredient_card_ids":["daging","tahu_tempe"],"income":16}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          1,
          6,
          2,
          4,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"telur","ingredient_name":"Telur","amount":4}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          1,
          7,
          2,
          4,
          'PLAYER',
          'Kebutuhan',
          'Kebutuhan',
          '{"card_id":"buku_2","amount":3,"points":2}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          2,
          0,
          3,
          1,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"daging","ingredient_name":"Daging","amount":5}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          2,
          1,
          3,
          1,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"tahu_tempe","ingredient_name":"Tahu Tempe","amount":3}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          2,
          2,
          3,
          2,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"sayur","ingredient_name":"Sayur","amount":2}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          2,
          3,
          3,
          2,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"tahu_tempe","ingredient_name":"Tahu Tempe","amount":3}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          2,
          4,
          3,
          3,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"nasi_putih","ingredient_name":"Nasi Putih","amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          2,
          5,
          3,
          3,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"telur","ingredient_name":"Telur","amount":4}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          2,
          6,
          3,
          4,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          2,
          7,
          3,
          4,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"tahu_tempe","ingredient_name":"Tahu Tempe","amount":3}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          3,
          0,
          4,
          1,
          'PLAYER',
          'JualMasakan',
          'JualMasakan',
          '{"order_card_id":"tahu_campur","required_ingredient_card_ids":["daging","tahu_tempe"],"income":16}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          3,
          1,
          4,
          1,
          'PLAYER',
          'Kebutuhan',
          'Kebutuhan',
          '{"card_id":"baju_1","amount":2,"points":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          3,
          2,
          4,
          2,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          3,
          3,
          4,
          2,
          'PLAYER',
          'Kebutuhan',
          'Kebutuhan',
          '{"card_id":"sepatu_2","amount":3,"points":2}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          3,
          4,
          4,
          3,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          3,
          5,
          4,
          3,
          'PLAYER',
          'Kebutuhan',
          'Kebutuhan',
          '{"card_id":"sepatu_1","amount":2,"points":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          3,
          6,
          4,
          4,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          3,
          7,
          4,
          4,
          'PLAYER',
          'Kebutuhan',
          'Kebutuhan',
          '{"card_id":"baju_2","amount":3,"points":2}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          4,
          0,
          5,
          1,
          'PLAYER',
          'JumatBerkah',
          'JumatBerkah',
          '{"amount":2}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          4,
          1,
          5,
          2,
          'PLAYER',
          'JumatBerkah',
          'JumatBerkah',
          '{"amount":3}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          4,
          2,
          5,
          3,
          'PLAYER',
          'JumatBerkah',
          'JumatBerkah',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          4,
          3,
          5,
          4,
          'PLAYER',
          'JumatBerkah',
          'JumatBerkah',
          '{"amount":4}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          4,
          4,
          5,
          4,
          'SYSTEM',
          'PoinPeringkatDonasi',
          'PoinPeringkatDonasi',
          '{"rank":1,"points":7}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          4,
          5,
          5,
          2,
          'SYSTEM',
          'PoinPeringkatDonasi',
          'PoinPeringkatDonasi',
          '{"rank":2,"points":5}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          4,
          6,
          5,
          1,
          'SYSTEM',
          'PoinPeringkatDonasi',
          'PoinPeringkatDonasi',
          '{"rank":3,"points":2}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          4,
          7,
          5,
          null,
          'SYSTEM',
          'UmumkanJuaraDonasi',
          'UmumkanJuaraDonasi',
          '{"summary":"Manalu Juara 1, Marcello Juara 2, Marco Juara 3","winners":[{"rank":1,"player_name":"Manalu","player_order_no":4,"points":7},{"rank":2,"player_name":"Marcello","player_order_no":2,"points":5},{"rank":3,"player_name":"Marco","player_order_no":1,"points":2}]}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          5,
          -1,
          0,
          null,
          'SYSTEM',
          'BukaHargaEmas',
          'BukaHargaEmas',
          '{"gold_price":6}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          5,
          0,
          6,
          1,
          'PLAYER',
          'InvestasiEmas',
          'InvestasiEmas',
          '{"trade_type":"BUY","unit_price":6,"qty":1,"amount":6}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          5,
          1,
          6,
          2,
          'PLAYER',
          'InvestasiEmas',
          'InvestasiEmas',
          '{"trade_type":"BUY","unit_price":6,"qty":1,"amount":6}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          5,
          2,
          6,
          3,
          'PLAYER',
          null,
          'LewatiTransaksiEmas',
          '{"note":"Tidak membeli dan tidak menjual emas"}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          5,
          3,
          6,
          4,
          'PLAYER',
          'InvestasiEmas',
          'InvestasiEmas',
          '{"trade_type":"BUY","unit_price":6,"qty":1,"amount":6}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          6,
          0,
          7,
          null,
          'SYSTEM',
          null,
          'HariMingguLibur',
          '{"note":"Hari Minggu libur"}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          7,
          0,
          8,
          1,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          7,
          1,
          8,
          1,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          7,
          2,
          8,
          2,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          7,
          3,
          8,
          2,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          7,
          4,
          8,
          3,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          7,
          5,
          8,
          3,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          7,
          6,
          8,
          4,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          7,
          7,
          8,
          4,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          8,
          0,
          9,
          1,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          8,
          1,
          9,
          1,
          'PLAYER',
          'Kebutuhan',
          'Kebutuhan',
          '{"card_id":"boneka_1","amount":6,"points":5}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          8,
          2,
          9,
          2,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          8,
          3,
          9,
          2,
          'PLAYER',
          'Kebutuhan',
          'Kebutuhan',
          '{"card_id":"gameboy_1","amount":6,"points":5}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          8,
          4,
          9,
          3,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          8,
          5,
          9,
          3,
          'PLAYER',
          'Kebutuhan',
          'Kebutuhan',
          '{"card_id":"hiburan_1","amount":6,"points":5}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          8,
          6,
          9,
          4,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          8,
          7,
          9,
          4,
          'PLAYER',
          'Kebutuhan',
          'Kebutuhan',
          '{"card_id":"jam_2","amount":7,"points":6}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          9,
          0,
          10,
          1,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          9,
          1,
          10,
          1,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          9,
          2,
          10,
          2,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          9,
          3,
          10,
          2,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          9,
          4,
          10,
          3,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          9,
          5,
          10,
          3,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          9,
          6,
          10,
          4,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          9,
          7,
          10,
          4,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          10,
          0,
          11,
          1,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          10,
          1,
          11,
          1,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          10,
          2,
          11,
          2,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          10,
          3,
          11,
          2,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          10,
          4,
          11,
          3,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          10,
          5,
          11,
          3,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          10,
          6,
          11,
          4,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          10,
          7,
          11,
          4,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          11,
          0,
          12,
          1,
          'PLAYER',
          'JumatBerkah',
          'JumatBerkah',
          '{"amount":5}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          11,
          1,
          12,
          2,
          'PLAYER',
          'JumatBerkah',
          'JumatBerkah',
          '{"amount":2}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          11,
          2,
          12,
          3,
          'PLAYER',
          'JumatBerkah',
          'JumatBerkah',
          '{"amount":4}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          11,
          3,
          12,
          4,
          'PLAYER',
          'JumatBerkah',
          'JumatBerkah',
          '{"amount":3}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          11,
          4,
          12,
          1,
          'SYSTEM',
          'PoinPeringkatDonasi',
          'PoinPeringkatDonasi',
          '{"rank":1,"points":7}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          11,
          5,
          12,
          3,
          'SYSTEM',
          'PoinPeringkatDonasi',
          'PoinPeringkatDonasi',
          '{"rank":2,"points":5}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          11,
          6,
          12,
          4,
          'SYSTEM',
          'PoinPeringkatDonasi',
          'PoinPeringkatDonasi',
          '{"rank":3,"points":2}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          11,
          7,
          12,
          null,
          'SYSTEM',
          'UmumkanJuaraDonasi',
          'UmumkanJuaraDonasi',
          '{"summary":"Marco Juara 1, Hugo Juara 2, Manalu Juara 3","winners":[{"rank":1,"player_name":"Marco","player_order_no":1,"points":7},{"rank":2,"player_name":"Hugo","player_order_no":3,"points":5},{"rank":3,"player_name":"Manalu","player_order_no":4,"points":2}]}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          12,
          -1,
          0,
          null,
          'SYSTEM',
          'BukaHargaEmas',
          'BukaHargaEmas',
          '{"gold_price":8}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          12,
          0,
          13,
          1,
          'PLAYER',
          'InvestasiEmas',
          'InvestasiEmas',
          '{"trade_type":"BUY","unit_price":8,"qty":1,"amount":8}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          12,
          1,
          13,
          2,
          'PLAYER',
          'JualEmas',
          'JualEmas',
          '{"trade_type":"SELL","unit_price":8,"qty":1,"amount":8}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          12,
          2,
          13,
          3,
          'PLAYER',
          'InvestasiEmas',
          'InvestasiEmas',
          '{"trade_type":"BUY","unit_price":8,"qty":1,"amount":8}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          12,
          3,
          13,
          4,
          'PLAYER',
          null,
          'LewatiTransaksiEmas',
          '{"note":"Tidak membeli dan tidak menjual emas"}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          13,
          0,
          14,
          null,
          'SYSTEM',
          null,
          'HariMingguLibur',
          '{"note":"Hari Minggu libur"}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          14,
          0,
          15,
          1,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          14,
          1,
          15,
          1,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          14,
          2,
          15,
          2,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          14,
          3,
          15,
          2,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          14,
          4,
          15,
          3,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          14,
          5,
          15,
          3,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          14,
          6,
          15,
          4,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          14,
          7,
          15,
          4,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          15,
          0,
          16,
          1,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          15,
          1,
          16,
          1,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          15,
          2,
          16,
          2,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          15,
          3,
          16,
          2,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          15,
          4,
          16,
          3,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          15,
          5,
          16,
          3,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          15,
          6,
          16,
          4,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          15,
          7,
          16,
          4,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          16,
          0,
          17,
          1,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          16,
          1,
          17,
          1,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          16,
          2,
          17,
          2,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          16,
          3,
          17,
          2,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          16,
          4,
          17,
          3,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          16,
          5,
          17,
          3,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          16,
          6,
          17,
          4,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          16,
          7,
          17,
          4,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          17,
          0,
          18,
          1,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          17,
          1,
          18,
          1,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          17,
          2,
          18,
          2,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          17,
          3,
          18,
          2,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          17,
          4,
          18,
          3,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          17,
          5,
          18,
          3,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          17,
          6,
          18,
          4,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          17,
          7,
          18,
          4,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          18,
          0,
          19,
          1,
          'PLAYER',
          'JumatBerkah',
          'JumatBerkah',
          '{"amount":3}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          18,
          1,
          19,
          2,
          'PLAYER',
          'JumatBerkah',
          'JumatBerkah',
          '{"amount":5}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          18,
          2,
          19,
          3,
          'PLAYER',
          'JumatBerkah',
          'JumatBerkah',
          '{"amount":2}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          18,
          3,
          19,
          4,
          'PLAYER',
          'JumatBerkah',
          'JumatBerkah',
          '{"amount":4}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          18,
          4,
          19,
          2,
          'SYSTEM',
          'PoinPeringkatDonasi',
          'PoinPeringkatDonasi',
          '{"rank":1,"points":7}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          18,
          5,
          19,
          4,
          'SYSTEM',
          'PoinPeringkatDonasi',
          'PoinPeringkatDonasi',
          '{"rank":2,"points":5}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          18,
          6,
          19,
          1,
          'SYSTEM',
          'PoinPeringkatDonasi',
          'PoinPeringkatDonasi',
          '{"rank":3,"points":2}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          18,
          7,
          19,
          null,
          'SYSTEM',
          'UmumkanJuaraDonasi',
          'UmumkanJuaraDonasi',
          '{"summary":"Marcello Juara 1, Manalu Juara 2, Marco Juara 3","winners":[{"rank":1,"player_name":"Marcello","player_order_no":2,"points":7},{"rank":2,"player_name":"Manalu","player_order_no":4,"points":5},{"rank":3,"player_name":"Marco","player_order_no":1,"points":2}]}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          19,
          -1,
          0,
          null,
          'SYSTEM',
          'BukaHargaEmas',
          'BukaHargaEmas',
          '{"gold_price":7}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          19,
          0,
          20,
          1,
          'PLAYER',
          'JualEmas',
          'JualEmas',
          '{"trade_type":"SELL","unit_price":7,"qty":1,"amount":7}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          19,
          1,
          20,
          2,
          'PLAYER',
          'InvestasiEmas',
          'InvestasiEmas',
          '{"trade_type":"BUY","unit_price":7,"qty":1,"amount":7}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          19,
          2,
          20,
          3,
          'PLAYER',
          'InvestasiEmas',
          'InvestasiEmas',
          '{"trade_type":"BUY","unit_price":7,"qty":1,"amount":7}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          19,
          3,
          20,
          4,
          'PLAYER',
          null,
          'LewatiTransaksiEmas',
          '{"note":"Tidak membeli dan tidak menjual emas"}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          20,
          0,
          21,
          null,
          'SYSTEM',
          null,
          'HariMingguLibur',
          '{"note":"Hari Minggu libur"}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          21,
          0,
          22,
          1,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          21,
          1,
          22,
          1,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          21,
          2,
          22,
          2,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          21,
          3,
          22,
          2,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          21,
          4,
          22,
          3,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          21,
          5,
          22,
          3,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          21,
          6,
          22,
          4,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          21,
          7,
          22,
          4,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          22,
          0,
          23,
          1,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          22,
          1,
          23,
          1,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          22,
          2,
          23,
          2,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          22,
          3,
          23,
          2,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          22,
          4,
          23,
          3,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          22,
          5,
          23,
          3,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          22,
          6,
          23,
          4,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          22,
          7,
          23,
          4,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          23,
          0,
          24,
          1,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          23,
          1,
          24,
          1,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          23,
          2,
          24,
          2,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          23,
          3,
          24,
          2,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          23,
          4,
          24,
          3,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          23,
          5,
          24,
          3,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          23,
          6,
          24,
          4,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          23,
          7,
          24,
          4,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          24,
          0,
          25,
          1,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          24,
          1,
          25,
          1,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          24,
          2,
          25,
          2,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          24,
          3,
          25,
          2,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          24,
          4,
          25,
          3,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          24,
          5,
          25,
          3,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          24,
          6,
          25,
          4,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          24,
          7,
          25,
          4,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          24,
          8,
          25,
          null,
          'SYSTEM',
          'AkhiriSesi',
          'AkhiriSesi',
          '{"end_note":"Selesai sesi pemula sesuai dokumen skenario"}' :: jsonb
        ),
        -- MAHIR: sesuai dokumen skenario 4 pemain selama 25 hari.
        (
          'MAHIR',
          null,
          0,
          0,
          1,
          null,
          'SYSTEM',
          null,
          'MulaiSesi',
          '{"start_note":"Mulai sesi simulasi mahir sesuai dokumen skenario"}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          0,
          0,
          1,
          1,
          'SYSTEM',
          'BagikanTieBreaker',
          'BagikanTieBreaker',
          '{"number":1,"card_code":"tie_breaker_1"}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          0,
          0,
          1,
          2,
          'SYSTEM',
          'BagikanTieBreaker',
          'BagikanTieBreaker',
          '{"number":2,"card_code":"tie_breaker_2"}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          0,
          0,
          1,
          3,
          'SYSTEM',
          'BagikanTieBreaker',
          'BagikanTieBreaker',
          '{"number":3,"card_code":"tie_breaker_3"}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          0,
          0,
          1,
          4,
          'SYSTEM',
          'BagikanTieBreaker',
          'BagikanTieBreaker',
          '{"number":4,"card_code":"tie_breaker_4"}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          0,
          -24,
          1,
          1,
          'SYSTEM',
          'SetupPinjamanAwal',
          'SetupPinjamanAwal',
          '{"loan_code":"loan_syariah_10","loan_id":"loan-setup-mahir-001","principal":10,"repayment_amount":10,"duration_turn":null,"penalty_points":15,"setup":"INITIAL"}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          0,
          -23,
          1,
          2,
          'SYSTEM',
          'SetupPinjamanAwal',
          'SetupPinjamanAwal',
          '{"loan_code":"loan_syariah_10","loan_id":"loan-setup-mahir-002","principal":10,"repayment_amount":10,"duration_turn":null,"penalty_points":15,"setup":"INITIAL"}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          0,
          -22,
          1,
          3,
          'SYSTEM',
          'SetupPinjamanAwal',
          'SetupPinjamanAwal',
          '{"loan_code":"loan_syariah_10","loan_id":"loan-setup-mahir-003","principal":10,"repayment_amount":10,"duration_turn":null,"penalty_points":15,"setup":"INITIAL"}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          0,
          -21,
          1,
          4,
          'SYSTEM',
          'SetupPinjamanAwal',
          'SetupPinjamanAwal',
          '{"loan_code":"loan_syariah_10","loan_id":"loan-setup-mahir-004","principal":10,"repayment_amount":10,"duration_turn":null,"penalty_points":15,"setup":"INITIAL"}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          0,
          -20,
          1,
          1,
          'SYSTEM',
          'SetupAsuransiAwal',
          'SetupAsuransiAwal',
          '{"product_code":"multirisk_basic","policy_id":"INS-SETUP-001","premium":0,"coverage_type":"MULTIRISK","setup":"INITIAL"}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          0,
          -19,
          1,
          2,
          'SYSTEM',
          'SetupAsuransiAwal',
          'SetupAsuransiAwal',
          '{"product_code":"multirisk_basic","policy_id":"INS-SETUP-002","premium":0,"coverage_type":"MULTIRISK","setup":"INITIAL"}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          0,
          -18,
          1,
          3,
          'SYSTEM',
          'SetupAsuransiAwal',
          'SetupAsuransiAwal',
          '{"product_code":"multirisk_basic","policy_id":"INS-SETUP-003","premium":0,"coverage_type":"MULTIRISK","setup":"INITIAL"}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          0,
          -17,
          1,
          4,
          'SYSTEM',
          'SetupAsuransiAwal',
          'SetupAsuransiAwal',
          '{"product_code":"multirisk_basic","policy_id":"INS-SETUP-004","premium":0,"coverage_type":"MULTIRISK","setup":"INITIAL"}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          1,
          -4,
          1,
          1,
          'SYSTEM',
          'SetupEmasAwal',
          'SetupEmasAwal',
          '{"qty":1,"setup":"INITIAL"}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          1,
          -3,
          1,
          2,
          'SYSTEM',
          'SetupEmasAwal',
          'SetupEmasAwal',
          '{"qty":1,"setup":"INITIAL"}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          1,
          -2,
          1,
          3,
          'SYSTEM',
          'SetupEmasAwal',
          'SetupEmasAwal',
          '{"qty":1,"setup":"INITIAL"}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          1,
          -1,
          1,
          4,
          'SYSTEM',
          'SetupEmasAwal',
          'SetupEmasAwal',
          '{"qty":1,"setup":"INITIAL"}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          0,
          1,
          1,
          1,
          'SYSTEM',
          'SetupMisiAwal',
          'SetupMisiAwal',
          '{"mission_id":"misi_boneka","target_tertiary_card_id":"boneka","penalty_points":10,"require_primary":true,"require_secondary":true}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          0,
          2,
          1,
          2,
          'SYSTEM',
          'SetupMisiAwal',
          'SetupMisiAwal',
          '{"mission_id":"misi_gameboy","target_tertiary_card_id":"gameboy","penalty_points":10,"require_primary":true,"require_secondary":true}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          0,
          3,
          1,
          3,
          'SYSTEM',
          'SetupMisiAwal',
          'SetupMisiAwal',
          '{"mission_id":"misi_hiburan","target_tertiary_card_id":"hiburan","penalty_points":10,"require_primary":true,"require_secondary":true}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          0,
          4,
          1,
          4,
          'SYSTEM',
          'SetupMisiAwal',
          'SetupMisiAwal',
          '{"mission_id":"misi_jam","target_tertiary_card_id":"jam","penalty_points":10,"require_primary":true,"require_secondary":true}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          0,
          5,
          1,
          1,
          'SYSTEM',
          'SetupBahanAwal',
          'SetupBahanAwal',
          '{"card_id":"nasi_putih","ingredient_name":"Nasi Putih","amount":1,"setup":"INITIAL"}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          0,
          7,
          1,
          2,
          'SYSTEM',
          'SetupBahanAwal',
          'SetupBahanAwal',
          '{"card_id":"daging","ingredient_name":"Daging","amount":5,"setup":"INITIAL"}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          0,
          9,
          1,
          3,
          'SYSTEM',
          'SetupBahanAwal',
          'SetupBahanAwal',
          '{"card_id":"nasi_putih","ingredient_name":"Nasi Putih","amount":1,"setup":"INITIAL"}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          0,
          12,
          1,
          4,
          'SYSTEM',
          'SetupBahanAwal',
          'SetupBahanAwal',
          '{"card_id":"tahu_tempe","ingredient_name":"Tahu Tempe","amount":3,"setup":"INITIAL"}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          0,
          21,
          1,
          null,
          'SYSTEM',
          'AmbilKartuDariDeck',
          'AmbilKartuDariDeck',
          '{"slot_group":"INGREDIENT_MARKET","slot_code":"SLOT_1","asset_type":"INGREDIENT","asset_code":"nasi_putih"}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          0,
          22,
          1,
          1,
          'SYSTEM',
          'KartuDiambilDariPasar',
          'KartuDiambilDariPasar',
          '{"slot_group":"INGREDIENT_MARKET","slot_code":"SLOT_1","asset_type":"INGREDIENT","asset_code":"nasi_putih"}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          0,
          23,
          1,
          null,
          'SYSTEM',
          'KartuMasukDiscard',
          'KartuMasukDiscard',
          '{"asset_type":"ORDER","asset_code":"lontong_balap"}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          0,
          24,
          1,
          null,
          'SYSTEM',
          'IsiUlangPasar',
          'IsiUlangPasar',
          '{"slot_group":"INGREDIENT_MARKET","slot_code":"SLOT_1","asset_type":"INGREDIENT","asset_code":"sayur"}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          1,
          0,
          2,
          1,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"sayur","ingredient_name":"Sayur","amount":2}' :: jsonb
        ),
        (
          'MAHIR',
          'mahir-risk-001',
          1,
          1,
          2,
          1,
          'PLAYER',
          'RisikoKehidupan',
          'RisikoKehidupan',
          '{"risk_id":"risk_pemadaman_listrik","note":"Pemadaman listrik"}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          1,
          2,
          2,
          2,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"sayur","ingredient_name":"Sayur","amount":2}' :: jsonb
        ),
        (
          'MAHIR',
          'mahir-risk-002',
          1,
          3,
          2,
          2,
          'PLAYER',
          'RisikoKehidupan',
          'RisikoKehidupan',
          '{"risk_id":"risk_bbm_naik","note":"BBM naik"}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          1,
          5,
          2,
          2,
          'PLAYER',
          'Kebutuhan',
          'Kebutuhan',
          '{"card_id":"buku_1","amount":2,"points":1}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          1,
          6,
          2,
          3,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"telur","ingredient_name":"Telur","amount":5}' :: jsonb
        ),
        (
          'MAHIR',
          'mahir-risk-003',
          1,
          7,
          2,
          3,
          'PLAYER',
          'RisikoKehidupan',
          'RisikoKehidupan',
          '{"risk_id":"risk_panen_melimpah","note":"Panen melimpah"}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          1,
          8,
          2,
          3,
          'PLAYER',
          'Kebutuhan',
          'Kebutuhan',
          '{"card_id":"buku_2","amount":3,"points":2}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          1,
          9,
          2,
          4,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"daging","ingredient_name":"Daging","amount":5}' :: jsonb
        ),
        (
          'MAHIR',
          'mahir-risk-004',
          1,
          10,
          2,
          4,
          'PLAYER',
          'RisikoKehidupan',
          'RisikoKehidupan',
          '{"risk_id":"risk_bencana_banjir","note":"Banjir"}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          1,
          11,
          2,
          4,
          'PLAYER',
          'JualMasakan',
          'JualMasakan',
          '{"order_card_id":"tahu_campur","required_ingredient_card_ids":["daging","tahu_tempe"],"income":16}' :: jsonb
        ),
        (
          'MAHIR',
          'mahir-risk-021',
          1,
          12,
          2,
          4,
          'PLAYER',
          'RisikoKehidupan',
          'RisikoKehidupan',
          '{"risk_id":"risk_sakit_perut","note":"Sakit perut"}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          2,
          0,
          3,
          1,
          'PLAYER',
          'Kebutuhan',
          'Kebutuhan',
          '{"card_id":"baju_1","amount":2,"points":1}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          2,
          1,
          3,
          1,
          'PLAYER',
          'Menabung',
          'Menabung',
          '{"goal_id":"tujuan_35","amount":5}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          2,
          2,
          3,
          2,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"daging","ingredient_name":"Daging","amount":5}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          2,
          3,
          3,
          2,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"telur","ingredient_name":"Telur","amount":4}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          2,
          4,
          3,
          3,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"telur","ingredient_name":"Telur","amount":4}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          2,
          5,
          3,
          3,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"daging","ingredient_name":"Daging","amount":5}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          2,
          6,
          3,
          4,
          'PLAYER',
          'Kebutuhan',
          'Kebutuhan',
          '{"card_id":"baju_2","amount":3,"points":2}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          2,
          7,
          3,
          4,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"daging","ingredient_name":"Daging","amount":5}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          3,
          0,
          4,
          1,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"nasi_putih","ingredient_name":"Nasi Putih","amount":1}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          3,
          1,
          4,
          1,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"tahu_tempe","ingredient_name":"Tahu Tempe","amount":3}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          3,
          2,
          4,
          2,
          'PLAYER',
          'JualMasakan',
          'JualMasakan',
          '{"order_card_id":"soto_daging","required_ingredient_card_ids":["daging","telur"],"income":17}' :: jsonb
        ),
        (
          'MAHIR',
          'mahir-risk-005',
          3,
          3,
          4,
          2,
          'PLAYER',
          'RisikoKehidupan',
          'RisikoKehidupan',
          '{"risk_id":"risk_mobil_tabrakan","note":"Bencana banjir"}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          3,
          4,
          4,
          2,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"tahu_tempe","ingredient_name":"Tahu Tempe","amount":3}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          3,
          5,
          4,
          3,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"tahu_tempe","ingredient_name":"Tahu Tempe","amount":3}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          3,
          6,
          4,
          3,
          'PLAYER',
          'JualMasakan',
          'JualMasakan',
          '{"order_card_id":"tahu_campur","required_ingredient_card_ids":["daging","tahu_tempe"],"income":16}' :: jsonb
        ),
        (
          'MAHIR',
          'mahir-risk-006',
          3,
          7,
          4,
          3,
          'PLAYER',
          'RisikoKehidupan',
          'RisikoKehidupan',
          '{"risk_id":"risk_study_tour","note":"Study tour"}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          3,
          8,
          4,
          4,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"nasi_putih","ingredient_name":"Nasi Putih","amount":1}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          3,
          9,
          4,
          4,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"telur","ingredient_name":"Telur","amount":4}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          4,
          0,
          5,
          1,
          'PLAYER',
          'JumatBerkah',
          'JumatBerkah',
          '{"amount":1}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          4,
          1,
          5,
          2,
          'PLAYER',
          'JumatBerkah',
          'JumatBerkah',
          '{"amount":3}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          4,
          2,
          5,
          3,
          'PLAYER',
          'JumatBerkah',
          'JumatBerkah',
          '{"amount":2}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          4,
          3,
          5,
          4,
          'PLAYER',
          'JumatBerkah',
          'JumatBerkah',
          '{"amount":4}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          4,
          4,
          5,
          4,
          'SYSTEM',
          'PoinPeringkatDonasi',
          'PoinPeringkatDonasi',
          '{"rank":1,"points":7}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          4,
          5,
          5,
          2,
          'SYSTEM',
          'PoinPeringkatDonasi',
          'PoinPeringkatDonasi',
          '{"rank":2,"points":5}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          4,
          6,
          5,
          3,
          'SYSTEM',
          'PoinPeringkatDonasi',
          'PoinPeringkatDonasi',
          '{"rank":3,"points":2}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          4,
          7,
          5,
          null,
          'SYSTEM',
          'UmumkanJuaraDonasi',
          'UmumkanJuaraDonasi',
          '{"summary":"Manalu Juara 1, Marcello Juara 2, Hugo Juara 3","winners":[{"rank":1,"player_name":"Manalu","player_order_no":4,"points":7},{"rank":2,"player_name":"Marcello","player_order_no":2,"points":5},{"rank":3,"player_name":"Hugo","player_order_no":3,"points":2}]}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          5,
          -1,
          0,
          null,
          'SYSTEM',
          'BukaHargaEmas',
          'BukaHargaEmas',
          '{"gold_price":6}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          5,
          0,
          6,
          1,
          'PLAYER',
          'InvestasiEmas',
          'InvestasiEmas',
          '{"trade_type":"BUY","unit_price":6,"qty":1,"amount":6}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          5,
          1,
          6,
          2,
          'PLAYER',
          null,
          'LewatiTransaksiEmas',
          '{"note":"Tidak membeli dan tidak menjual emas"}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          5,
          2,
          6,
          3,
          'PLAYER',
          'InvestasiEmas',
          'InvestasiEmas',
          '{"trade_type":"BUY","unit_price":6,"qty":1,"amount":6}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          5,
          3,
          6,
          4,
          'PLAYER',
          'InvestasiEmas',
          'InvestasiEmas',
          '{"trade_type":"BUY","unit_price":6,"qty":1,"amount":6}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          6,
          0,
          7,
          null,
          'SYSTEM',
          null,
          'HariMingguLibur',
          '{"note":"Hari Minggu libur"}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          7,
          0,
          8,
          1,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"telur","ingredient_name":"Telur","amount":4}' :: jsonb
        ),
        (
          'MAHIR',
          'mahir-risk-007',
          7,
          1,
          8,
          1,
          'PLAYER',
          'RisikoKehidupan',
          'RisikoKehidupan',
          '{"risk_id":"risk_menang_undian","note":"Menang undian"}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          7,
          2,
          8,
          1,
          'PLAYER',
          'JualMasakan',
          'JualMasakan',
          '{"order_card_id":"sego_penyet","required_ingredient_card_ids":["telur","tahu_tempe","nasi_putih"],"income":22}' :: jsonb
        ),
        (
          'MAHIR',
          'mahir-risk-022',
          7,
          3,
          8,
          1,
          'PLAYER',
          'RisikoKehidupan',
          'RisikoKehidupan',
          '{"risk_id":"risk_investasi_emas","note":"Investasi emas terbuka"}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          7,
          4,
          8,
          1,
          'PLAYER',
          'InvestasiEmas',
          'InvestasiEmas',
          '{"trade_type":"BUY","unit_price":6,"qty":1,"amount":6,"source":"risk_investasi_emas"}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          7,
          4,
          8,
          2,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"telur","ingredient_name":"Telur","amount":4}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          7,
          5,
          8,
          2,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"sayur","ingredient_name":"Sayur","amount":2}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          7,
          6,
          8,
          3,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"daging","ingredient_name":"Daging","amount":5}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          7,
          7,
          8,
          3,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"daging","ingredient_name":"Daging","amount":5}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          7,
          8,
          8,
          4,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"daging","ingredient_name":"Daging","amount":5}' :: jsonb
        ),
        (
          'MAHIR',
          'mahir-risk-008',
          7,
          9,
          8,
          4,
          'PLAYER',
          'RisikoKehidupan',
          'RisikoKehidupan',
          '{"risk_id":"risk_beli_peralatan_dapur","note":"Beli peralatan dapur"}' :: jsonb
        ),
        (
          'MAHIR',
          'mahir-risk-008',
          7,
          10,
          8,
          4,
          'PLAYER',
          'GunakanOpsiDarurat',
          'GunakanOpsiDarurat',
          '{"option_type":"USE_INSURANCE"}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          7,
          11,
          8,
          4,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"sayur","ingredient_name":"Sayur","amount":2}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          8,
          0,
          9,
          1,
          'PLAYER',
          'Menabung',
          'Menabung',
          '{"goal_id":"tujuan_35","amount":10}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          8,
          1,
          9,
          1,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"tahu_tempe","ingredient_name":"Tahu Tempe","amount":3}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          8,
          2,
          9,
          2,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"telur","ingredient_name":"Telur","amount":4}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          8,
          3,
          9,
          2,
          'PLAYER',
          'JualMasakan',
          'JualMasakan',
          '{"order_card_id":"semanggi_surabaya","required_ingredient_card_ids":["sayur","sayur"],"income":14}' :: jsonb
        ),
        (
          'MAHIR',
          'mahir-risk-009',
          8,
          4,
          9,
          2,
          'PLAYER',
          'RisikoKehidupan',
          'RisikoKehidupan',
          '{"risk_id":"risk_ulang_tahun","note":"Ulang tahun"}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          8,
          7,
          9,
          3,
          'PLAYER',
          'Menabung',
          'Menabung',
          '{"goal_id":"tujuan_30","amount":10}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          8,
          8,
          9,
          3,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"nasi_putih","ingredient_name":"Nasi Putih","amount":1}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          8,
          9,
          9,
          4,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          8,
          10,
          9,
          4,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          9,
          0,
          10,
          1,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"telur","ingredient_name":"Telur","amount":4}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          9,
          1,
          10,
          1,
          'PLAYER',
          'JualMasakan',
          'JualMasakan',
          '{"order_card_id":"sego_penyet","required_ingredient_card_ids":["telur","tahu_tempe","nasi_putih"],"income":22}' :: jsonb
        ),
        (
          'MAHIR',
          'mahir-risk-010',
          9,
          2,
          10,
          1,
          'PLAYER',
          'RisikoKehidupan',
          'RisikoKehidupan',
          '{"risk_id":"risk_ekstrakurikuler_anak","note":"Ekstrakurikuler anak"}' :: jsonb
        ),
        (
          'MAHIR',
          'mahir-risk-010',
          9,
          3,
          10,
          1,
          'PLAYER',
          'GunakanOpsiDarurat',
          'GunakanOpsiDarurat',
          '{"option_type":"USE_INSURANCE"}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          9,
          4,
          10,
          2,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"nasi_putih","ingredient_name":"Nasi Putih","amount":1}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          9,
          5,
          10,
          2,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"telur","ingredient_name":"Telur","amount":4}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          9,
          6,
          10,
          3,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          9,
          7,
          10,
          3,
          'PLAYER',
          'JualMasakan',
          'JualMasakan',
          '{"order_card_id":"sate_klopo","required_ingredient_card_ids":["daging","daging","nasi_putih"],"income":26}' :: jsonb
        ),
        (
          'MAHIR',
          'mahir-risk-011',
          9,
          8,
          10,
          3,
          'PLAYER',
          'RisikoKehidupan',
          'RisikoKehidupan',
          '{"risk_id":"risk_ban_bocor","note":"Ban bocor"}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          9,
          10,
          10,
          4,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"tahu_tempe","ingredient_name":"Tahu Tempe","amount":3}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          9,
          11,
          10,
          4,
          'PLAYER',
          'JualMasakan',
          'JualMasakan',
          '{"order_card_id":"nasi_pecel","required_ingredient_card_ids":["nasi_putih","tahu_tempe","sayur"],"income":20}' :: jsonb
        ),
        (
          'MAHIR',
          'mahir-risk-012',
          9,
          12,
          10,
          4,
          'PLAYER',
          'RisikoKehidupan',
          'RisikoKehidupan',
          '{"risk_id":"risk_sakit_gigi","note":"Sakit gigi"}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          10,
          0,
          11,
          1,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"sayur","ingredient_name":"Sayur","amount":2}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          10,
          1,
          11,
          1,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          10,
          2,
          11,
          2,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          10,
          3,
          11,
          2,
          'PLAYER',
          'Menabung',
          'Menabung',
          '{"goal_id":"tujuan_32","amount":5}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          10,
          4,
          11,
          3,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          10,
          5,
          11,
          3,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          10,
          6,
          11,
          4,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          10,
          7,
          11,
          4,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          11,
          0,
          12,
          1,
          'PLAYER',
          'JumatBerkah',
          'JumatBerkah',
          '{"amount":4}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          11,
          1,
          12,
          2,
          'PLAYER',
          'JumatBerkah',
          'JumatBerkah',
          '{"amount":2}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          11,
          2,
          12,
          3,
          'PLAYER',
          'JumatBerkah',
          'JumatBerkah',
          '{"amount":5}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          11,
          3,
          12,
          4,
          'PLAYER',
          'JumatBerkah',
          'JumatBerkah',
          '{"amount":3}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          11,
          4,
          12,
          3,
          'SYSTEM',
          'PoinPeringkatDonasi',
          'PoinPeringkatDonasi',
          '{"rank":1,"points":7}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          11,
          5,
          12,
          1,
          'SYSTEM',
          'PoinPeringkatDonasi',
          'PoinPeringkatDonasi',
          '{"rank":2,"points":5}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          11,
          6,
          12,
          4,
          'SYSTEM',
          'PoinPeringkatDonasi',
          'PoinPeringkatDonasi',
          '{"rank":3,"points":2}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          11,
          7,
          12,
          null,
          'SYSTEM',
          'UmumkanJuaraDonasi',
          'UmumkanJuaraDonasi',
          '{"summary":"Hugo Juara 1, Marco Juara 2, Manalu Juara 3","winners":[{"rank":1,"player_name":"Hugo","player_order_no":3,"points":7},{"rank":2,"player_name":"Marco","player_order_no":1,"points":5},{"rank":3,"player_name":"Manalu","player_order_no":4,"points":2}]}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          12,
          -1,
          0,
          null,
          'SYSTEM',
          'BukaHargaEmas',
          'BukaHargaEmas',
          '{"gold_price":8}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          12,
          0,
          13,
          1,
          'PLAYER',
          'JualEmas',
          'JualEmas',
          '{"trade_type":"SELL","unit_price":8,"qty":1,"amount":8}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          12,
          1,
          13,
          2,
          'PLAYER',
          'InvestasiEmas',
          'InvestasiEmas',
          '{"trade_type":"BUY","unit_price":8,"qty":1,"amount":8}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          12,
          2,
          13,
          3,
          'PLAYER',
          'InvestasiEmas',
          'InvestasiEmas',
          '{"trade_type":"BUY","unit_price":8,"qty":1,"amount":8}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          12,
          3,
          13,
          4,
          'PLAYER',
          'JualEmas',
          'JualEmas',
          '{"trade_type":"SELL","unit_price":8,"qty":1,"amount":8}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          13,
          0,
          14,
          null,
          'SYSTEM',
          null,
          'HariMingguLibur',
          '{"note":"Hari Minggu libur"}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          14,
          0,
          15,
          1,
          'PLAYER',
          'Kebutuhan',
          'Kebutuhan',
          '{"card_id":"boneka_1","amount":6,"points":5}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          14,
          1,
          15,
          1,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"sayur","ingredient_name":"Sayur","amount":2}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          14,
          2,
          15,
          2,
          'PLAYER',
          'Kebutuhan',
          'Kebutuhan',
          '{"card_id":"gameboy_1","amount":6,"points":5}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          14,
          3,
          15,
          2,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          14,
          4,
          15,
          3,
          'PLAYER',
          'Kebutuhan',
          'Kebutuhan',
          '{"card_id":"hiburan_1","amount":6,"points":5}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          14,
          5,
          15,
          3,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"daging","ingredient_name":"Daging","amount":5}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          14,
          6,
          15,
          4,
          'PLAYER',
          'Kebutuhan',
          'Kebutuhan',
          '{"card_id":"boneka_2","amount":7,"points":6}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          14,
          7,
          15,
          4,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"nasi_putih","ingredient_name":"Nasi Putih","amount":1}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          15,
          0,
          16,
          1,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"tahu_tempe","ingredient_name":"Tahu Tempe","amount":3}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          15,
          1,
          16,
          1,
          'PLAYER',
          'JualMasakan',
          'JualMasakan',
          '{"order_card_id":"semanggi_surabaya","required_ingredient_card_ids":["sayur","sayur"],"income":14}' :: jsonb
        ),
        (
          'MAHIR',
          'mahir-risk-013',
          15,
          2,
          16,
          1,
          'PLAYER',
          'RisikoKehidupan',
          'RisikoKehidupan',
          '{"risk_id":"risk_operasi_usus_buntu","note":"Operasi usus buntu"}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          15,
          4,
          16,
          2,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          15,
          5,
          16,
          2,
          'PLAYER',
          'JualMasakan',
          'JualMasakan',
          '{"order_card_id":"rawon","required_ingredient_card_ids":["daging","telur","tahu_tempe"],"income":24}' :: jsonb
        ),
        (
          'MAHIR',
          'mahir-risk-014',
          15,
          6,
          16,
          2,
          'PLAYER',
          'RisikoKehidupan',
          'RisikoKehidupan',
          '{"risk_id":"risk_ganti_oli","note":"Ganti oli"}' :: jsonb
        ),
        (
          'MAHIR',
          'mahir-risk-014',
          15,
          7,
          16,
          2,
          'PLAYER',
          'Asuransi',
          'Asuransi',
          '{}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          15,
          8,
          16,
          3,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"tahu_tempe","ingredient_name":"Tahu Tempe","amount":3}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          15,
          9,
          16,
          3,
          'PLAYER',
          'JualMasakan',
          'JualMasakan',
          '{"order_card_id":"tahu_telur","required_ingredient_card_ids":["telur","telur","tahu_tempe"],"income":25}' :: jsonb
        ),
        (
          'MAHIR',
          'mahir-risk-015',
          15,
          10,
          16,
          3,
          'PLAYER',
          'RisikoKehidupan',
          'RisikoKehidupan',
          '{"risk_id":"risk_ganti_aki","note":"Ganti aki"}' :: jsonb
        ),
        (
          'MAHIR',
          'mahir-risk-015',
          15,
          11,
          16,
          3,
          'PLAYER',
          'Asuransi',
          'Asuransi',
          '{}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          15,
          12,
          16,
          4,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"telur","ingredient_name":"Telur","amount":4}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          15,
          13,
          16,
          4,
          'PLAYER',
          'JualMasakan',
          'JualMasakan',
          '{"order_card_id":"soto_daging","required_ingredient_card_ids":["daging","telur"],"income":17}' :: jsonb
        ),
        (
          'MAHIR',
          'mahir-risk-016',
          15,
          14,
          16,
          4,
          'PLAYER',
          'RisikoKehidupan',
          'RisikoKehidupan',
          '{"risk_id":"risk_kecelakaan","note":"Biaya kesehatan keluarga"}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          16,
          0,
          17,
          1,
          'PLAYER',
          'Menabung',
          'Menabung',
          '{"goal_id":"tujuan_35","amount":10}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          16,
          1,
          17,
          1,
          'SYSTEM',
          'TujuanFinansial',
          'TujuanFinansial',
          '{"goal_id":"tujuan_25","cost":25,"points":20}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          16,
          2,
          17,
          1,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          16,
          3,
          17,
          2,
          'PLAYER',
          'Menabung',
          'Menabung',
          '{"goal_id":"tujuan_32","amount":10}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          16,
          4,
          17,
          2,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          16,
          5,
          17,
          3,
          'PLAYER',
          'Menabung',
          'Menabung',
          '{"goal_id":"tujuan_30","amount":10}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          16,
          6,
          17,
          3,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          16,
          7,
          17,
          4,
          'PLAYER',
          'Menabung',
          'Menabung',
          '{"goal_id":"tujuan_28","amount":10}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          16,
          8,
          17,
          4,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          17,
          0,
          18,
          1,
          'PLAYER',
          'Asuransi',
          'Asuransi',
          '{"policy_id":"INS-SEED-008","policy_instance_id":"INS-SEED-008","product_code":"multirisk_basic","premium":1,"coverage_type":"MULTIRISK"}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          17,
          1,
          18,
          1,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          17,
          2,
          18,
          2,
          'PLAYER',
          'Asuransi',
          'Asuransi',
          '{"policy_id":"INS-SEED-009","policy_instance_id":"INS-SEED-009","product_code":"multirisk_basic","premium":1,"coverage_type":"MULTIRISK"}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          17,
          3,
          18,
          2,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          17,
          4,
          18,
          3,
          'PLAYER',
          'Asuransi',
          'Asuransi',
          '{"policy_id":"INS-SEED-010","policy_instance_id":"INS-SEED-010","product_code":"multirisk_basic","premium":1,"coverage_type":"MULTIRISK"}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          17,
          5,
          18,
          3,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          17,
          6,
          18,
          4,
          'PLAYER',
          'Asuransi',
          'Asuransi',
          '{"policy_id":"INS-SEED-011","policy_instance_id":"INS-SEED-011","product_code":"multirisk_basic","premium":1,"coverage_type":"MULTIRISK"}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          17,
          7,
          18,
          4,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          18,
          0,
          19,
          1,
          'PLAYER',
          'JumatBerkah',
          'JumatBerkah',
          '{"amount":1}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          18,
          1,
          19,
          2,
          'PLAYER',
          'JumatBerkah',
          'JumatBerkah',
          '{"amount":4}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          18,
          2,
          19,
          3,
          'PLAYER',
          'JumatBerkah',
          'JumatBerkah',
          '{"amount":2}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          18,
          3,
          19,
          4,
          'PLAYER',
          'JumatBerkah',
          'JumatBerkah',
          '{"amount":5}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          18,
          4,
          19,
          4,
          'SYSTEM',
          'PoinPeringkatDonasi',
          'PoinPeringkatDonasi',
          '{"rank":1,"points":7}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          18,
          5,
          19,
          2,
          'SYSTEM',
          'PoinPeringkatDonasi',
          'PoinPeringkatDonasi',
          '{"rank":2,"points":5}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          18,
          6,
          19,
          3,
          'SYSTEM',
          'PoinPeringkatDonasi',
          'PoinPeringkatDonasi',
          '{"rank":3,"points":2}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          18,
          7,
          19,
          null,
          'SYSTEM',
          'UmumkanJuaraDonasi',
          'UmumkanJuaraDonasi',
          '{"summary":"Manalu Juara 1, Marcello Juara 2, Hugo Juara 3","winners":[{"rank":1,"player_name":"Manalu","player_order_no":4,"points":7},{"rank":2,"player_name":"Marcello","player_order_no":2,"points":5},{"rank":3,"player_name":"Hugo","player_order_no":3,"points":2}]}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          19,
          -1,
          0,
          null,
          'SYSTEM',
          'BukaHargaEmas',
          'BukaHargaEmas',
          '{"gold_price":7}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          19,
          0,
          20,
          1,
          'PLAYER',
          'InvestasiEmas',
          'InvestasiEmas',
          '{"trade_type":"BUY","unit_price":7,"qty":1,"amount":7}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          19,
          1,
          20,
          2,
          'PLAYER',
          'JualEmas',
          'JualEmas',
          '{"trade_type":"SELL","unit_price":7,"qty":1,"amount":7}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          19,
          2,
          20,
          3,
          'PLAYER',
          'InvestasiEmas',
          'InvestasiEmas',
          '{"trade_type":"BUY","unit_price":7,"qty":1,"amount":7}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          19,
          3,
          20,
          4,
          'PLAYER',
          null,
          'LewatiTransaksiEmas',
          '{"note":"Tidak membeli dan tidak menjual emas"}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          20,
          0,
          21,
          null,
          'SYSTEM',
          null,
          'HariMingguLibur',
          '{"note":"Hari Minggu libur"}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          21,
          0,
          22,
          1,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"nasi_putih","ingredient_name":"Nasi Putih","amount":1}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          21,
          1,
          22,
          1,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"sayur","ingredient_name":"Sayur","amount":2}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          21,
          2,
          22,
          2,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"daging","ingredient_name":"Daging","amount":5}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          21,
          3,
          22,
          2,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"tahu_tempe","ingredient_name":"Tahu Tempe","amount":3}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          21,
          4,
          22,
          3,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"nasi_putih","ingredient_name":"Nasi Putih","amount":1}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          21,
          5,
          22,
          3,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"daging","ingredient_name":"Daging","amount":5}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          21,
          6,
          22,
          4,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"sayur","ingredient_name":"Sayur","amount":2}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          21,
          7,
          22,
          4,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"tahu_tempe","ingredient_name":"Tahu Tempe","amount":3}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          22,
          0,
          23,
          1,
          'PLAYER',
          'JualMasakan',
          'JualMasakan',
          '{"order_card_id":"nasi_pecel","required_ingredient_card_ids":["nasi_putih","tahu_tempe","sayur"],"income":20}' :: jsonb
        ),
        (
          'MAHIR',
          'mahir-risk-017',
          22,
          1,
          23,
          1,
          'PLAYER',
          'RisikoKehidupan',
          'RisikoKehidupan',
          '{"risk_id":"risk_depresi","note":"Uang kas keluarga berkurang"}' :: jsonb
        ),
        (
          'MAHIR',
          'mahir-risk-017',
          22,
          2,
          23,
          1,
          'PLAYER',
          'Asuransi',
          'Asuransi',
          '{}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          22,
          3,
          23,
          1,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          22,
          4,
          23,
          2,
          'PLAYER',
          'JualMasakan',
          'JualMasakan',
          '{"order_card_id":"rawon","required_ingredient_card_ids":["daging","telur","tahu_tempe"],"income":24}' :: jsonb
        ),
        (
          'MAHIR',
          'mahir-risk-018',
          22,
          5,
          23,
          2,
          'PLAYER',
          'RisikoKehidupan',
          'RisikoKehidupan',
          '{"risk_id":"risk_sakit_asam_lambung","note":"Sakit asam lambung"}' :: jsonb
        ),
        (
          'MAHIR',
          'mahir-risk-018',
          22,
          6,
          23,
          2,
          'PLAYER',
          'Asuransi',
          'Asuransi',
          '{}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          22,
          7,
          23,
          2,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          22,
          8,
          23,
          3,
          'PLAYER',
          'JualMasakan',
          'JualMasakan',
          '{"order_card_id":"sate_klopo","required_ingredient_card_ids":["daging","daging","nasi_putih"],"income":26}' :: jsonb
        ),
        (
          'MAHIR',
          'mahir-risk-019',
          22,
          9,
          23,
          3,
          'PLAYER',
          'RisikoKehidupan',
          'RisikoKehidupan',
          '{"risk_id":"risk_wisuda_kelulusan","note":"Wisuda kelulusan"}' :: jsonb
        ),
        (
          'MAHIR',
          'mahir-risk-019',
          22,
          10,
          23,
          3,
          'PLAYER',
          'Asuransi',
          'Asuransi',
          '{}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          22,
          11,
          23,
          3,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          22,
          12,
          23,
          4,
          'PLAYER',
          'JualMasakan',
          'JualMasakan',
          '{"order_card_id":"rujak_cingur","required_ingredient_card_ids":["sayur","tahu_tempe","nasi_putih","daging"],"income":28}' :: jsonb
        ),
        (
          'MAHIR',
          'mahir-risk-020',
          22,
          13,
          23,
          4,
          'PLAYER',
          'RisikoKehidupan',
          'RisikoKehidupan',
          '{"risk_id":"risk_tahun_ajaran_baru","note":"Tahun ajaran baru"}' :: jsonb
        ),
        (
          'MAHIR',
          'mahir-risk-020',
          22,
          14,
          23,
          4,
          'PLAYER',
          'Asuransi',
          'Asuransi',
          '{}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          22,
          15,
          23,
          4,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          23,
          0,
          24,
          1,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          23,
          1,
          24,
          1,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          23,
          2,
          24,
          2,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          23,
          3,
          24,
          2,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          23,
          4,
          24,
          3,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          23,
          5,
          24,
          3,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          23,
          6,
          24,
          4,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          23,
          7,
          24,
          4,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          24,
          0,
          25,
          1,
          'PLAYER',
          'BayarPinjaman',
          'BayarPinjaman',
          '{"loan_code":"loan_syariah_10","loan_id":"loan-setup-mahir-001","amount":10}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          24,
          1,
          25,
          1,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          24,
          2,
          25,
          2,
          'PLAYER',
          'BayarPinjaman',
          'BayarPinjaman',
          '{"loan_code":"loan_syariah_10","loan_id":"loan-setup-mahir-002","amount":10}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          24,
          3,
          25,
          2,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          24,
          4,
          25,
          3,
          'PLAYER',
          'BayarPinjaman',
          'BayarPinjaman',
          '{"loan_code":"loan_syariah_10","loan_id":"loan-setup-mahir-003","amount":10}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          24,
          5,
          25,
          3,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          24,
          6,
          25,
          4,
          'PLAYER',
          'BayarPinjaman',
          'BayarPinjaman',
          '{"loan_code":"loan_syariah_10","loan_id":"loan-setup-mahir-004","amount":10}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          24,
          7,
          25,
          4,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          24,
          8,
          25,
          null,
          'SYSTEM',
          'AkhiriSesi',
          'AkhiriSesi',
          '{"end_note":"Selesai sesi mahir sesuai dokumen skenario"}' :: jsonb
        )
    ) as x(
      session_key,
      ref_key,
      day_index,
      event_order,
      action_slot,
      player_no,
      actor_type,
      action_id,
      action_type,
      payload
    )
),
setup_market_seed as (
  select
    session_key,
    null :: text as ref_key,
    0 as day_index,
    -200 + sort_order as event_order,
    0 as action_slot,
    null :: int as player_no,
    'SYSTEM' :: text as actor_type,
    'AmbilKartuDariDeck' :: text as action_id,
    'AmbilKartuDariDeck' :: text as action_type,
    jsonb_build_object(
      'setup',
      'INITIAL_MARKET',
      'slot_group',
      slot_group,
      'slot_code',
      slot_code,
      'asset_type',
      asset_type,
      'asset_code',
      asset_code
    ) as payload
  from
    (
      values
        ('PEMULA'),
        ('MAHIR')
    ) sessions(session_key)
    cross join (
      values
        (
          1,
          'INGREDIENT_MARKET',
          'SLOT_1',
          'INGREDIENT',
          'nasi_putih'
        ),
        (
          2,
          'INGREDIENT_MARKET',
          'SLOT_2',
          'INGREDIENT',
          'sayur'
        ),
        (
          3,
          'INGREDIENT_MARKET',
          'SLOT_3',
          'INGREDIENT',
          'tahu_tempe'
        ),
        (
          4,
          'INGREDIENT_MARKET',
          'SLOT_4',
          'INGREDIENT',
          'telur'
        ),
        (
          5,
          'INGREDIENT_MARKET',
          'SLOT_5',
          'INGREDIENT',
          'daging'
        ),
        (
          6,
          'ORDER_MARKET',
          'SLOT_1',
          'ORDER',
          'lontong_balap'
        ),
        (
          7,
          'ORDER_MARKET',
          'SLOT_2',
          'ORDER',
          'semanggi_surabaya'
        ),
        (
          8,
          'ORDER_MARKET',
          'SLOT_3',
          'ORDER',
          'nasi_goreng'
        ),
        (
          9,
          'ORDER_MARKET',
          'SLOT_4',
          'ORDER',
          'tahu_campur'
        ),
        (
          10,
          'ORDER_MARKET',
          'SLOT_5',
          'ORDER',
          'soto_daging'
        ),
        (11, 'NEED_MARKET', 'SLOT_1', 'NEED', 'buku_1'),
        (12, 'NEED_MARKET', 'SLOT_2', 'NEED', 'buku_2'),
        (13, 'NEED_MARKET', 'SLOT_3', 'NEED', 'baju_1'),
        (14, 'NEED_MARKET', 'SLOT_4', 'NEED', 'sepatu_1'),
        (15, 'NEED_MARKET', 'SLOT_5', 'NEED', 'sepatu_2')
    ) slots(
      sort_order,
      slot_group,
      slot_code,
      asset_type,
      asset_code
    )
),
transition_seed as (
  select
    session_key,
    null :: text as ref_key,
    day_index,
    max(event_order) + 1 as event_order,
    0 as action_slot,
    null :: int as player_no,
    'SYSTEM' :: text as actor_type,
    'AkhirGiliran' :: text as action_id,
    'AkhirGiliran' :: text as action_type,
    jsonb_build_object(
      'from_day',
      day_index + 1,
      'to_day',
      day_index + 2,
      'completed_players',
      4,
      'used',
      count(*) filter (
        where
          actor_type = 'PLAYER'
          and action_type not in (
            'JumatBerkah',
            'RisikoKehidupan',
            'GunakanOpsiDarurat',
            'InvestasiEmas',
            'JualEmas',
            'LewatiTransaksiEmas',
            'HariMingguLibur'
          )
          and not (
            action_type in ('Asuransi', 'PinjamanSyariah')
            and (
              ref_key is not null
              or payload ? 'risk_event_id'
            )
          )
      ),
      'remaining',
      0
    ) as payload
  from
    scenario_event_seed
  where
    day_index between 0
    and 23
  group by
    session_key,
    day_index
),
event_seed as (
  select
    *
  from
    scenario_event_seed
  union
  all
  select
    *
  from
    setup_market_seed
  union
  all
  select
    *
  from
    transition_seed
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
      when es.actor_type = 'PLAYER'
      and es.player_no is not null
      and (
        es.action_type in (
          'JumatBerkah',
          'RisikoKehidupan',
          'GunakanOpsiDarurat',
          'InvestasiEmas',
          'JualEmas',
          'LewatiTransaksiEmas',
          'HariMingguLibur'
        )
        or (
          es.action_type in ('Asuransi', 'PinjamanSyariah')
          and (
            es.ref_key is not null
            or es.payload ? 'risk_event_id'
            or es.payload ->> 'setup' = 'INITIAL'
          )
        )
      ) then 0
      when es.actor_type = 'PLAYER'
      and es.player_no is not null then least(
        2,
        count(*) filter (
          where
            es.actor_type = 'PLAYER'
            and es.action_type not in (
              'JumatBerkah',
              'RisikoKehidupan',
              'GunakanOpsiDarurat',
              'InvestasiEmas',
              'JualEmas',
              'LewatiTransaksiEmas',
              'HariMingguLibur'
            )
            and not (
              es.action_type in ('Asuransi', 'PinjamanSyariah')
              and (
                es.ref_key is not null
                or es.payload ? 'risk_event_id'
                or es.payload ->> 'setup' = 'INITIAL'
              )
            )
        ) over (
          partition by es.session_key,
          es.day_index,
          es.player_no
          order by
            es.event_order
        )
      ) :: int
      else 0
    end as action_slot,
    es.actor_type,
    coalesce(es.action_id, es.action_type) as action_id,
    coalesce(es.action_id, es.action_type) as action_type,
    case
      when es.actor_type = 'PLAYER' then coalesce(es.player_no, 0)
      else 0
    end :: int as turn_number,
    es.payload,
    pc.session_player_id,
    pc.user_id,
    row_number() over (
      partition by es.session_key
      order by
        es.day_index,
        es.event_order,
        es.action_type,
        coalesce(es.player_no, 0)
    ) as event_number
  from
    event_seed es
    join session_context sc on sc.session_key = es.session_key
    left join player_context pc on pc.session_key = es.session_key
    and pc.player_no = es.player_no
),
numbered_events as (
  select
    *,
    event_number - 1 as sequence_number,
    (
      event_uuid_prefix || lpad(event_number :: text, 12, '0')
    ) :: uuid as event_id,
    day_index + 1 as stored_day_index,
    base_timestamp + (day_index * interval '1 day') + (event_order * interval '1 minute') as event_timestamp,
    case
      day_index % 7
      when 0 then 'MON'
      when 1 then 'TUE'
      when 2 then 'WED'
      when 3 then 'THU'
      when 4 then 'FRI'
      when 5 then 'SAT'
      else 'SUN'
    end as weekday
  from
    ordered_events
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
      when e.action_type = 'Asuransi'
      and (
        e.ref_key is not null
        or (e.payload :: jsonb ? 'risk_event_id')
      ) then e.payload :: jsonb || jsonb_build_object(
        'risk_event_id',
        coalesce(
          risk_event.event_id,
          fn_safe_cast_to_uuid(e.payload ->> 'risk_event_id')
        ) :: text
      )
      when e.action_type = 'GunakanOpsiDarurat'
      and (
        e.ref_key is not null
        or (e.payload :: jsonb ? 'risk_event_id')
      ) then e.payload :: jsonb || jsonb_build_object(
        'risk_event_id',
        coalesce(
          risk_event.event_id,
          fn_safe_cast_to_uuid(e.payload ->> 'risk_event_id')
        ) :: text
      )
      when e.action_type = 'Kebutuhan' then e.payload :: jsonb || jsonb_build_object('need_tier', rn.need_tier)
      when e.action_type = 'RisikoKehidupan' then e.payload :: jsonb || jsonb_build_object(
        'risk_id',
        case
          e.payload ->> 'risk_id'
          when 'risk_biaya_listrik' then 'risk_pemadaman_listrik'
          when 'risk_servis_sepeda' then 'risk_ban_bocor'
          when 'risk_biaya_kesehatan' then 'risk_sakit_gigi'
          when 'risk_uang_kas' then 'risk_depresi'
          when 'risk_biaya_transport' then 'risk_ganti_oli'
          when 'risk_tagihan_air' then 'risk_ganti_aki'
          when 'risk_perbaikan_atap' then 'risk_mobil_tabrakan'
          when 'risk_kebutuhan_keluarga' then 'risk_ekstrakurikuler_anak'
          when 'risk_tagihan_internet' then 'risk_study_tour'
          else e.payload ->> 'risk_id'
        end
      )
      else e.payload
    end as payload
  from
    numbered_events e
    left join numbered_events risk_event on risk_event.session_key = e.session_key
    and risk_event.ref_key = e.ref_key
    and risk_event.action_type = 'RisikoKehidupan'
    left join ruleset_needs rn on rn.ruleset_version_id = e.ruleset_version_id
    and lower(rn.need_code) = lower((e.payload :: jsonb) ->> 'card_id')
)
select
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
  re.payload :: jsonb as payload
from
  resolved_events re
  join ruleset_actions ra on ra.ruleset_version_id = re.ruleset_version_id
  and lower(ra.action_id) = lower(coalesce(re.action_id, re.action_type))
  and ra.is_active
order by
  re.session_id,
  re.sequence_number loop perform apply_game_event(
    v_event.event_id,
    v_event.session_id,
    v_event.session_player_id,
    v_event.user_id,
    v_event.actor_type,
    v_event.event_timestamp,
    v_event.day_index,
    v_event.weekday,
    v_event.turn_number,
    v_event.action_slot,
    v_event.sequence_number,
    v_event.ruleset_action_id,
    v_event.action_type,
    v_event.ruleset_version_id,
    v_event.payload,
    null,
    v_event.event_id
  );

end loop;

end $$;

create temporary table seed_pension_rank_points on commit drop as with ranked as (
  select
    sp.session_id,
    sp.session_participant_id,
    spb.coins + spb.saving + coalesce(ingredients.leftover_qty, 0) as cash_remaining,
    row_number() over (
      partition by sp.session_id
      order by
        (
          spb.coins + spb.saving + coalesce(ingredients.leftover_qty, 0)
        ) desc,
        sptb.tie_number desc,
        sp.user_id
    ) :: int as rank_no
  from
    session_participants sp
    join session_participant_balances spb on spb.session_id = sp.session_id
    and spb.session_participant_id = sp.session_participant_id
    left join (
      select
        session_id,
        session_participant_id,
        sum(qty) :: int as leftover_qty
      from
        session_participant_inventory
      group by
        session_id,
        session_participant_id
    ) ingredients on ingredients.session_id = sp.session_id
    and ingredients.session_participant_id = sp.session_participant_id
    left join session_participant_tie_breakers sptb on sptb.session_id = sp.session_id
    and sptb.session_participant_id = sp.session_participant_id
  where
    sp.session_id in (
      '91000000-0000-0000-0000-000000000001' :: uuid,
      '91000000-0000-0000-0000-000000000002' :: uuid
    )
)
select
  ranked.session_id,
  ranked.session_participant_id,
  ranked.cash_remaining,
  ranked.rank_no,
  coalesce(rprp.points, 0) :: int as points_awarded
from
  ranked
  join sessions s on s.session_id = ranked.session_id
  left join ruleset_rank_points rprp on rprp.ruleset_version_id = s.ruleset_version_id
  and rprp.rank_type = 'PENSION'
  and rprp.rank_no = ranked.rank_no;

delete from
  session_final_score_components
where
  session_id in (
    '91000000-0000-0000-0000-000000000001' :: uuid,
    '91000000-0000-0000-0000-000000000002' :: uuid
  );

delete from
  session_final_scores
where
  session_id in (
    '91000000-0000-0000-0000-000000000001' :: uuid,
    '91000000-0000-0000-0000-000000000002' :: uuid
  );

with component_values as (
  select
    sp.session_id,
    sp.session_participant_id,
    sptb.tie_number,
    (
      select
        count(*) :: int
      from
        session_participant_need_purchases spnp
        join ruleset_needs rn on rn.ruleset_need_id = spnp.ruleset_need_id
      where
        spnp.session_participant_id = sp.session_participant_id
        and rn.need_tier = 'primer'
        and not spnp.is_sold
    ) as primary_need_count,
    (
      select
        count(*) :: int
      from
        session_participant_need_purchases spnp
        join ruleset_needs rn on rn.ruleset_need_id = spnp.ruleset_need_id
      where
        spnp.session_participant_id = sp.session_participant_id
        and rn.need_tier = 'sekunder'
        and not spnp.is_sold
    ) as secondary_need_count,
    (
      select
        count(*) :: int
      from
        session_participant_need_purchases spnp
        join ruleset_needs rn on rn.ruleset_need_id = spnp.ruleset_need_id
      where
        spnp.session_participant_id = sp.session_participant_id
        and rn.need_tier = 'tersier'
        and not spnp.is_sold
    ) as tertiary_need_count,
    coalesce(
      (
        select
          sum(rn.happiness_points) :: int
        from
          session_participant_need_purchases spnp
          join ruleset_needs rn on rn.ruleset_need_id = spnp.ruleset_need_id
        where
          spnp.session_participant_id = sp.session_participant_id
          and not spnp.is_sold
      ),
      0
    ) :: int as need_points,
    coalesce(
      sum(
        case
          when e.action_type = 'PoinPeringkatDonasi' then coalesce((e.payload ->> 'points') :: int, 0)
          else 0
        end
      ),
      0
    ) :: int as donation_points,
    coalesce(
      sum(
        case
          when e.action_type = 'TujuanFinansial' then coalesce((e.payload ->> 'points') :: int, 0)
          else 0
        end
      ),
      0
    ) :: int as saving_goal_points,
    coalesce(
      (
        select
          sum(rcm.penalty_points)
        from
          session_participant_collection_missions spcm
          join ruleset_collection_missions rcm on rcm.ruleset_collection_mission_id = spcm.ruleset_collection_mission_id
        where
          spcm.session_id = sp.session_id
          and spcm.session_participant_id = sp.session_participant_id
          and spcm.is_failed
      ),
      0
    ) :: int as mission_penalty_points,
    coalesce(spr.points_awarded, 0) :: int as pension_points,
    coalesce(
      (
        select
          rga.points
        from
          session_participant_gold_holdings spgh
          join ruleset_game_assets asset on asset.ruleset_game_asset_id = spgh.ruleset_game_asset_id
          join ruleset_gold_assets rga on rga.ruleset_version_id = asset.ruleset_version_id
          and rga.quantity = spgh.quantity
        where
          spgh.session_participant_id = sp.session_participant_id
        limit
          1
      ), 0
    ) :: int as gold_points,
    coalesce(
      (
        select
          sum((spl.metadata_json ->> 'penalty_points') :: int)
        from
          session_participant_loans spl
        where
          spl.session_participant_id = sp.session_participant_id
          and spl.status = 'ACTIVE'
      ),
      0
    ) :: int as loan_penalty_points
  from
    session_participants sp
    left join events e on e.session_id = sp.session_id
    and e.user_id = sp.user_id
    left join session_participant_tie_breakers sptb on sptb.session_id = sp.session_id
    and sptb.session_participant_id = sp.session_participant_id
    left join seed_pension_rank_points spr on spr.session_id = sp.session_id
    and spr.session_participant_id = sp.session_participant_id
  where
    sp.session_id in (
      '91000000-0000-0000-0000-000000000001' :: uuid,
      '91000000-0000-0000-0000-000000000002' :: uuid
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
    least(
      primary_need_count,
      least(secondary_need_count, tertiary_need_count)
    ) :: int as mixed_need_sets
  from
    component_values
),
scored_with_bonuses as (
  select
    *,
    (
      mixed_need_sets * 4 + (
        ((primary_need_count - mixed_need_sets) / 3) :: int * 2
      ) + (
        ((secondary_need_count - mixed_need_sets) / 3) :: int * 2
      ) + (
        ((tertiary_need_count - mixed_need_sets) / 3) :: int * 2
      )
    ) :: int as need_set_bonus_points,
    case
      when loan_penalty_points > 0 then 0
      else saving_goal_points
    end as saving_goal_points_effective
  from
    scored
),
final_component_values as (
  select
    *,
    need_points + need_set_bonus_points + donation_points + saving_goal_points_effective + pension_points + gold_points - mission_penalty_points - loan_penalty_points as total_points
  from
    scored_with_bonuses
),
ranked_scores as (
  select
    *,
    row_number() over (
      partition by session_id
      order by
        total_points desc,
        tie_number desc,
        session_participant_id
    ) :: int as rank_no
  from
    final_component_values
),
upserted_scores as (
  insert into
    session_final_scores (
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
      select
        e.event_id
      from
        events e
      where
        e.session_id = ranked_scores.session_id
      order by
        e.sequence_number desc
      limit
        1
    )
  from
    ranked_scores on conflict (session_id, session_participant_id) do
  update
  set
    total_points = excluded.total_points,
    rank_no = excluded.rank_no,
    tie_breaker_number = excluded.tie_breaker_number,
    has_unpaid_loan = excluded.has_unpaid_loan,
    computed_at = excluded.computed_at,
    source_event_id = excluded.source_event_id returning session_final_score_id,
    session_id,
    session_participant_id,
    computed_at,
    source_event_id
)
insert into
  session_final_score_components (
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
from
  upserted_scores score
  join ranked_scores ranked on ranked.session_id = score.session_id
  and ranked.session_participant_id = score.session_participant_id
  cross join lateral (
    values
      ('NEED_POINTS' :: varchar(80), ranked.need_points),
      (
        'NEED_SET_BONUS' :: varchar(80),
        ranked.need_set_bonus_points
      ),
      ('DONATION' :: varchar(80), ranked.donation_points),
      ('GOLD' :: varchar(80), ranked.gold_points),
      ('PENSION' :: varchar(80), ranked.pension_points),
      (
        'SAVING_GOAL' :: varchar(80),
        ranked.saving_goal_points_effective
      ),
      (
        'MISSION_PENALTY' :: varchar(80),
        - ranked.mission_penalty_points
      ),
      (
        'LOAN_PENALTY' :: varchar(80),
        - ranked.loan_penalty_points
      )
  ) as component(component_code, points) on conflict (session_final_score_id, component_code) do
update
set
  points = excluded.points,
  source_event_id = excluded.source_event_id,
  created_at = excluded.created_at;

with seed_sessions as (
  select
    s.session_id,
    s.session_name,
    s.mode,
    s.ended_at,
    s.ruleset_version_id,
    case
      when s.mode = 'MAHIR' then 10
      else 20
    end as starting_coins,
    case
      when s.mode = 'MAHIR' then 'advanced'
      else 'beginner'
    end as gameplay_mode
  from
    sessions s
  where
    s.session_id in (
      '91000000-0000-0000-0000-000000000001' :: uuid,
      '91000000-0000-0000-0000-000000000002' :: uuid
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
  from
    seed_sessions ss
    join session_participants sp on sp.session_id = ss.session_id
    join app_users au on au.user_id = sp.user_id
),
event_agg as (
  select
    pb.session_id,
    pb.user_id,
    count(e.event_pk) filter (
      where
        e.actor_type = 'PLAYER'
    ) :: int as player_event_count,
    count(e.event_pk) :: int as event_count,
    coalesce(max(e.action_slot), 0) :: int as latest_action_slot,
    coalesce(max(e.day_index), 1) :: int as latest_day_index,
    coalesce(max(e.weekday), 'MON') as latest_weekday,
    coalesce(max(e.timestamp), pb.ended_at) as latest_event_timestamp,
    coalesce(
      jsonb_agg(
        jsonb_build_object(
          'day_index',
          e.day_index,
          'action_slot',
          e.action_slot,
          'sequence_number',
          e.sequence_number,
          'action_type',
          e.action_type
        )
        order by
          e.sequence_number
      ) filter (
        where
          e.actor_type = 'PLAYER'
      ),
      '[]' :: jsonb
    ) as action_sequence,
    coalesce(
      sum(
        case
          when e.action_type = 'BahanMasakan' then ((e.payload :: jsonb) ->> 'amount') :: int
          else 0
        end
      ),
      0
    ) :: int as ingredients_collected,
    coalesce(
      sum(
        case
          when e.action_type = 'BuangBahanMasakan' then ((e.payload :: jsonb) ->> 'amount') :: int
          else 0
        end
      ),
      0
    ) :: int as ingredients_wasted,
    count(*) filter (
      where
        e.action_type = 'JualMasakan'
    ) :: int as meal_orders_claimed,
    count(*) filter (
      where
        e.action_type = 'LewatiOrder'
    ) :: int as meal_orders_passed,
    coalesce(
      sum(
        case
          when e.action_type = 'JualMasakan' then ((e.payload :: jsonb) ->> 'income') :: int
          else 0
        end
      ),
      0
    ) :: int as meal_order_income_total,
    count(*) filter (
      where
        e.action_type = 'Kebutuhan'
        and lower((e.payload :: jsonb) ->> 'need_tier') = 'primer'
    ) :: int as primary_needs_owned,
    count(*) filter (
      where
        e.action_type = 'Kebutuhan'
        and lower((e.payload :: jsonb) ->> 'need_tier') = 'sekunder'
    ) :: int as secondary_needs_owned,
    count(*) filter (
      where
        e.action_type = 'Kebutuhan'
        and lower((e.payload :: jsonb) ->> 'need_tier') = 'tersier'
    ) :: int as tertiary_needs_owned,
    coalesce(
      sum(
        case
          when e.action_type = 'Kebutuhan' then ((e.payload :: jsonb) ->> 'amount') :: int
          else 0
        end
      ),
      0
    ) :: int as need_cards_coins_spent,
    coalesce(
      sum(
        case
          when e.action_type = 'Kebutuhan' then ((e.payload :: jsonb) ->> 'points') :: int
          else 0
        end
      ),
      0
    ) :: int as need_points,
    coalesce(
      sum(
        case
          when e.action_type = 'JumatBerkah' then ((e.payload :: jsonb) ->> 'amount') :: int
          else 0
        end
      ),
      0
    ) :: int as donation_total_coins,
    count(*) filter (
      where
        e.action_type = 'JumatBerkah'
    ) :: int as donation_events,
    coalesce(
      sum(
        case
          when e.action_type = 'InvestasiEmas'
          and upper((e.payload :: jsonb) ->> 'trade_type') = 'BUY' then ((e.payload :: jsonb) ->> 'qty') :: int
          else 0
        end
      ),
      0
    ) :: int as gold_cards_purchased,
    coalesce(
      sum(
        case
          when e.action_type = 'JualEmas' then ((e.payload :: jsonb) ->> 'qty') :: int
          else 0
        end
      ),
      0
    ) :: int as gold_cards_sold,
    coalesce(
      sum(
        case
          when e.action_type = 'InvestasiEmas'
          and upper((e.payload :: jsonb) ->> 'trade_type') = 'BUY' then ((e.payload :: jsonb) ->> 'amount') :: int
          else 0
        end
      ),
      0
    ) :: int as gold_investment_coins_spent,
    coalesce(
      sum(
        case
          when e.action_type = 'JualEmas' then ((e.payload :: jsonb) ->> 'amount') :: int
          else 0
        end
      ),
      0
    ) :: int as gold_investment_coins_earned,
    count(*) filter (
      where
        e.action_type = 'RisikoKehidupan'
    ) :: int as life_risk_cards_drawn,
    coalesce(
      sum(
        case
          when e.action_type = 'RisikoKehidupan'
          and risk_catalog.direction = 'OUT' then risk_catalog.amount
          else 0
        end
      ),
      0
    ) :: int as life_risk_costs_total,
    count(*) filter (
      where
        e.action_type = 'Asuransi'
        and e.payload :: jsonb ? 'risk_event_id'
    ) :: int as life_risk_mitigated_with_insurance,
    count(*) filter (
      where
        e.action_type = 'Asuransi'
        and e.payload :: jsonb ? 'premium'
    ) :: int as insurance_payments_made,
    count(*) filter (
      where
        e.action_type = 'GunakanOpsiDarurat'
    ) :: int as emergency_options_used,
    count(*) filter (
      where
        e.action_type = 'Menabung'
    ) :: int as financial_goals_attempted,
    count(*) filter (
      where
        e.action_type = 'TujuanFinansial'
    ) :: int as financial_goals_completed,
    coalesce(
      sum(
        case
          when e.action_type = 'Menabung' then ((e.payload :: jsonb) ->> 'amount') :: int
          else 0
        end
      ),
      0
    ) :: int as coins_saved,
    coalesce(
      sum(
        case
          when e.action_type = 'TujuanFinansial' then ((e.payload :: jsonb) ->> 'points') :: int
          else 0
        end
      ),
      0
    ) :: int as saving_goal_points,
    count(*) filter (
      where
        e.action_type = 'PinjamanSyariah'
    ) :: int as sharia_loans_taken,
    count(*) filter (
      where
        e.action_type = 'BayarPinjaman'
    ) :: int as sharia_loans_repaid,
    coalesce(
      sum(
        case
          when e.action_type = 'PinjamanSyariah' then ((e.payload :: jsonb) ->> 'principal') :: int
          else 0
        end
      ),
      0
    ) :: int as sharia_loan_principal_total,
    coalesce(
      sum(
        case
          when e.action_type = 'BayarPinjaman' then ((e.payload :: jsonb) ->> 'amount') :: int
          else 0
        end
      ),
      0
    ) :: int as sharia_loan_repaid_total,
    count(*) filter (
      where
        e.action_type = 'BagikanMisiKoleksi'
    ) :: int as mission_assigned_count
  from
    player_base pb
    left join events e on e.session_id = pb.session_id
    and e.user_id = pb.user_id
    left join ruleset_life_risks risk_catalog on risk_catalog.ruleset_version_id = e.ruleset_version_id
    and lower(risk_catalog.risk_code) = lower(e.payload :: jsonb ->> 'risk_id')
  group by
    pb.session_id,
    pb.user_id,
    pb.ended_at
),
projection_agg as (
  select
    pb.session_id,
    pb.user_id,
    coalesce(
      sum(p.amount) filter (
        where
          p.direction = 'IN'
      ),
      0
    ) :: int as cash_in_total,
    coalesce(
      sum(p.amount) filter (
        where
          p.direction = 'OUT'
      ),
      0
    ) :: int as cash_out_total,
    count(p.projection_id) :: int as transaction_count
  from
    player_base pb
    left join event_cashflow_projections p on p.session_id = pb.session_id
    and p.user_id = pb.user_id
  group by
    pb.session_id,
    pb.user_id
),
gold_holding_agg as (
  select
    pb.session_id,
    pb.user_id,
    coalesce(sum(spgh.quantity), 0) :: int as gold_qty
  from
    player_base pb
    left join session_participant_gold_holdings spgh on spgh.session_id = pb.session_id
    and spgh.session_participant_id = pb.session_player_id
  group by
    pb.session_id,
    pb.user_id
),
score_component_agg as (
  select
    fs.session_id,
    fs.session_participant_id,
    fs.total_points,
    coalesce(
      sum(fsc.points) filter (
        where
          fsc.component_code in ('NEED_POINTS', 'NEED_SET_BONUS')
      ),
      0
    ) :: int as need_points,
    coalesce(
      sum(fsc.points) filter (
        where
          fsc.component_code = 'DONATION'
      ),
      0
    ) :: int as donation_points,
    coalesce(
      sum(fsc.points) filter (
        where
          fsc.component_code = 'GOLD'
      ),
      0
    ) :: int as gold_points,
    coalesce(
      sum(fsc.points) filter (
        where
          fsc.component_code = 'PENSION'
      ),
      0
    ) :: int as pension_points,
    coalesce(
      sum(fsc.points) filter (
        where
          fsc.component_code = 'SAVING_GOAL'
      ),
      0
    ) :: int as saving_goal_points,
    coalesce(
      sum(fsc.points) filter (
        where
          fsc.component_code in ('MISSION_PENALTY', 'LOAN_PENALTY')
      ),
      0
    ) :: int as penalty_points
  from
    session_final_scores fs
    join session_final_score_components fsc on fsc.session_id = fs.session_id
    and fsc.session_final_score_id = fs.session_final_score_id
  where
    fs.session_id in (
      '91000000-0000-0000-0000-000000000001' :: uuid,
      '91000000-0000-0000-0000-000000000002' :: uuid
    )
  group by
    fs.session_id,
    fs.session_participant_id,
    fs.total_points
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
    gh.gold_qty as gold_cards_held_end,
    (
      ea.gold_investment_coins_earned - ea.gold_investment_coins_spent
    ) as gold_investment_net,
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
    greatest(
      ea.sharia_loan_principal_total - ea.sharia_loan_repaid_total,
      0
    ) as sharia_loans_outstanding_coins,
    greatest(
      ea.sharia_loans_taken - ea.sharia_loans_repaid,
      0
    ) as sharia_loans_unpaid_end,
    ea.mission_assigned_count,
    sca.total_points as score_total_points,
    sca.need_points as score_need_points,
    sca.donation_points as score_donation_points,
    sca.gold_points as score_gold_points,
    sca.pension_points as score_pension_points,
    sca.saving_goal_points as score_saving_goal_points,
    sca.penalty_points as score_penalty_points,
    pa.cash_in_total,
    pa.cash_out_total,
    (pa.cash_in_total - pa.cash_out_total) as cash_net_total,
    (
      pb.starting_coins + pa.cash_in_total - pa.cash_out_total
    ) as coins_held_current,
    pa.transaction_count
  from
    player_base pb
    join event_agg ea on ea.session_id = pb.session_id
    and ea.user_id = pb.user_id
    join projection_agg pa on pa.session_id = pb.session_id
    and pa.user_id = pb.user_id
    join gold_holding_agg gh on gh.session_id = pb.session_id
    and gh.user_id = pb.user_id
    join score_component_agg sca on sca.session_id = pb.session_id
    and sca.session_participant_id = pb.session_player_id
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
      'metadata',
      jsonb_build_object(
        'game_id',
        session_id,
        'session_id',
        session_id,
        'user_id',
        user_id,
        'player_alias',
        display_name,
        'game_mode',
        gameplay_mode,
        'action_slot',
        latest_action_slot,
        'day_index',
        latest_day_index,
        'day_label',
        latest_weekday,
        'event_timestamp',
        latest_event_timestamp,
        'seed_source',
        '02_seed_simulation_sessions_events'
      ),
      'coins',
      jsonb_build_object(
        'starting_coins',
        starting_coins,
        'cash_in_total',
        cash_in_total,
        'cash_out_total',
        cash_out_total,
        'coins_held_current',
        coins_held_current,
        'coins_spent_total',
        cash_out_total,
        'coins_earned_total',
        cash_in_total,
        'coins_donated',
        donation_total_coins,
        'coins_saved',
        coins_saved,
        'coins_net_end_game',
        cash_net_total
      ),
      'ingredients',
      jsonb_build_object(
        'ingredients_collected',
        ingredients_collected,
        'ingredients_held_current',
        greatest(ingredients_collected - ingredients_wasted, 0),
        'ingredients_used_per_meal',
        meal_orders_claimed,
        'ingredients_wasted',
        ingredients_wasted,
        'ingredient_investment_coins_total',
        coalesce(need_cards_coins_spent, 0)
      ),
      'meal_orders',
      jsonb_build_object(
        'meal_orders_claimed',
        meal_orders_claimed,
        'meal_orders_available_passed',
        meal_orders_passed,
        'meal_order_income_total',
        meal_order_income_total,
        'meal_orders_per_turn_average',
        round(
          meal_orders_claimed :: numeric / greatest(latest_action_slot, 1),
          2
        )
      ),
      'needs',
      jsonb_build_object(
        'need_cards_purchased',
        primary_needs_owned + secondary_needs_owned + tertiary_needs_owned,
        'primary_needs_owned',
        primary_needs_owned,
        'secondary_needs_owned',
        secondary_needs_owned,
        'tertiary_needs_owned',
        tertiary_needs_owned,
        'collection_mission_complete',
        tertiary_needs_owned > 0,
        'need_cards_coins_spent',
        need_cards_coins_spent
      ),
      'donations',
      jsonb_build_object(
        'donation_events',
        donation_events,
        'donation_total_coins',
        donation_total_coins,
        'donation_happiness_points',
        score_donation_points
      ),
      'gold',
      jsonb_build_object(
        'gold_cards_purchased',
        gold_cards_purchased,
        'gold_cards_sold',
        gold_cards_sold,
        'gold_cards_held_end',
        gold_cards_held_end,
        'gold_investment_coins_spent',
        gold_investment_coins_spent,
        'gold_investment_coins_earned',
        gold_investment_coins_earned,
        'gold_investment_net',
        gold_investment_net
      ),
      'pension',
      jsonb_build_object(
        'leftover_coins_end_game',
        coins_held_current,
        'ingredient_cards_value_end',
        greatest(ingredients_collected - ingredients_wasted, 0),
        'coins_in_savings_goal',
        coins_saved,
        'pension_fund_total',
        coins_held_current + greatest(ingredients_collected - ingredients_wasted, 0) + coins_saved,
        'pension_happiness_points',
        score_pension_points
      ),
      'life_risk',
      jsonb_build_object(
        'life_risks_available',
        life_risk_cards_drawn,
        'life_risk_cards_drawn',
        life_risk_cards_drawn,
        'life_risks_accepted',
        life_risk_cards_drawn,
        'life_risk_costs_total',
        life_risk_costs_total,
        'life_risk_mitigated_with_insurance',
        life_risk_mitigated_with_insurance,
        'insurance_payments_made',
        insurance_payments_made,
        'emergency_options_used',
        emergency_options_used
      ),
      'financial_goals',
      jsonb_build_object(
        'financial_goals_attempted',
        financial_goals_attempted,
        'financial_goals_completed',
        financial_goals_completed,
        'financial_goals_coins_total_invested',
        coins_saved,
        'sharia_loans_taken',
        sharia_loans_taken,
        'sharia_loans_repaid',
        sharia_loans_repaid,
        'sharia_loans_unpaid_end',
        sharia_loans_unpaid_end,
        'sharia_loans_outstanding_coins',
        sharia_loans_outstanding_coins
      ),
      'actions',
      jsonb_build_object(
        'actions_per_turn',
        2,
        'action_events_total',
        player_event_count,
        'action_sequence',
        action_sequence,
        'actions_skipped',
        0
      ),
      'turns',
      jsonb_build_object(
        'action_slot_game_completion',
        latest_action_slot,
        'latest_day_index',
        latest_day_index,
        'transaction_count',
        transaction_count,
        'event_count',
        event_count
      ),
      'outcomes',
      jsonb_build_object(
        'total_happiness_points',
        score_total_points,
        'finish_line_reached',
        true,
        'dnf_flag',
        false
      ),
      'notes',
      '[]' :: jsonb
    ) as metric_payload_json
  from
    player_metric_base
  union
  all
  select
    session_id,
    user_id,
    session_player_id,
    ruleset_version_id,
    coalesce(ended_at, latest_event_timestamp) as computed_at,
    'gameplay.derived.metrics' as metric_name,
    2 as metric_sort,
    jsonb_build_object(
      'net_worth_index',
      round(
        (
          coins_held_current :: numeric / greatest(starting_coins, 1)
        ) * 100,
        2
      ),
      'income_diversification_index',
      case
        when cash_in_total > 0 then 1
        else 0
      end,
      'income_diversification_ratio',
      case
        when cash_in_total > 0 then 1
        else 0
      end,
      'income_diversification_components',
      jsonb_build_object(
        'total_income',
        cash_in_total,
        'transaction_count',
        transaction_count
      ),
      'expense_management_efficiency',
      round(
        (
          greatest(
            starting_coins + cash_in_total - cash_out_total,
            0
          ) :: numeric / greatest(starting_coins + cash_in_total, 1)
        ) * 100,
        2
      ),
      'expense_management_components',
      jsonb_build_object(
        'essential_expenses',
        need_cards_coins_spent,
        'total_expenses',
        cash_out_total
      ),
      'business_profit_margin',
      round(
        (
          (
            meal_order_income_total - greatest(ingredients_collected, 0)
          ) :: numeric / greatest(meal_order_income_total, 1)
        ) * 100,
        2
      ),
      'business_efficiency_ratio',
      round(
        meal_order_income_total :: numeric / greatest(ingredients_collected, 1),
        2
      ),
      'gold_roi_percentage',
      round(
        (
          gold_investment_net :: numeric / greatest(gold_investment_coins_spent, 1)
        ) * 100,
        2
      ),
      'risk_exposure_percentage',
      round(
        (
          life_risk_cards_drawn :: numeric / greatest(player_event_count, 1)
        ) * 100,
        2
      ),
      'risk_mitigation_effectiveness',
      round(
        (
          life_risk_mitigated_with_insurance :: numeric / greatest(life_risk_cards_drawn, 1)
        ) * 100,
        2
      ),
      'risk_appetite_score',
      life_risk_cards_drawn,
      'risk_appetite_components',
      jsonb_build_object(
        'life_risks_accepted',
        life_risk_cards_drawn,
        'life_risks_available',
        life_risk_cards_drawn,
        'insurance_activation_rate',
        round(
          (
            life_risk_mitigated_with_insurance :: numeric / greatest(life_risk_cards_drawn, 1)
          ) * 100,
          2
        )
      ),
      'debt_leverage_ratio',
      round(
        (
          sharia_loan_principal_total :: numeric / greatest(starting_coins, 1)
        ) * 100,
        2
      ),
      'loan_repayment_discipline',
      round(
        (
          sharia_loans_repaid :: numeric / greatest(sharia_loans_taken, 1)
        ) * 100,
        2
      ),
      'debt_ratio',
      round(
        (
          sharia_loans_outstanding_coins :: numeric / greatest(starting_coins + cash_in_total, 1)
        ) * 100,
        2
      ),
      'goal_ambition',
      financial_goals_attempted,
      'goal_setting_ambition',
      financial_goals_attempted,
      'goal_setting_components',
      jsonb_build_object(
        'Goal_Attempt_Rate',
        financial_goals_attempted,
        'Goal_Investment_Rate',
        coins_saved
      ),
      'action_efficiency',
      round(
        (
          transaction_count :: numeric / greatest(player_event_count, 1)
        ) * 100,
        2
      ),
      'action_efficiency_percent',
      round(
        (
          transaction_count :: numeric / greatest(player_event_count, 1)
        ) * 100,
        2
      ),
      'action_diversity_score_avg',
      transaction_count,
      'meal_order_success_rate',
      round(
        (
          meal_orders_claimed :: numeric / greatest(meal_orders_claimed + meal_orders_passed, 1)
        ) * 100,
        2
      ),
      'planning_horizon',
      latest_action_slot,
      'planning_horizon_percent',
      round((latest_day_index :: numeric / 25) * 100, 2),
      'fulfillment_diversity',
      primary_needs_owned + secondary_needs_owned + tertiary_needs_owned,
      'fulfillment_diversity_components',
      jsonb_build_object(
        'p_primary',
        primary_needs_owned,
        'p_secondary',
        secondary_needs_owned,
        'p_tertiary',
        tertiary_needs_owned
      ),
      'mission_achievement',
      case
        when tertiary_needs_owned > 0 then 1
        else 0
      end,
      'growth_pattern_ratio',
      round(
        (
          cash_net_total :: numeric / greatest(starting_coins, 1)
        ) * 100,
        2
      ),
      'donation_aggressiveness_percent',
      round(
        (
          donation_total_coins :: numeric / greatest(cash_out_total, 1)
        ) * 100,
        2
      ),
      'donation_stability_std_deviation',
      0,
      'donation_ratio',
      round(
        (
          donation_total_coins :: numeric / greatest(cash_out_total, 1)
        ) * 100,
        2
      ),
      'friday_participation_rate',
      donation_events,
      'donation_commitment_score',
      donation_total_coins,
      'donation_commitment_components',
      jsonb_build_object(
        'donation_stability',
        donation_events,
        'donation_ratio',
        round(
          (
            donation_total_coins :: numeric / greatest(cash_out_total, 1)
          ) * 100,
          2
        ),
        'friday_participation_rate',
        donation_events
      ),
      'risk_appetite_score_normalized',
      round(
        (
          life_risk_cards_drawn :: numeric / greatest(player_event_count, 1)
        ) * 100,
        2
      ),
      'sharia_loans_outstanding_coins',
      sharia_loans_outstanding_coins,
      'happiness_portfolio',
      jsonb_build_object(
        'need_cards_pts',
        score_need_points,
        'donations_pts',
        score_donation_points,
        'gold_pts',
        score_gold_points,
        'pension_pts',
        score_pension_points,
        'financial_goals_pts',
        score_saving_goal_points,
        'mission_bonus_pts',
        score_penalty_points
      ),
      'notes',
      '[]' :: jsonb
    ) as metric_payload_json
  from
    player_metric_base
),
numbered_snapshots as (
  select
    row_number() over (
      order by
        session_id,
        user_id,
        metric_sort
    ) as snapshot_number,
    session_id,
    user_id,
    session_player_id,
    ruleset_version_id,
    computed_at,
    metric_name,
    metric_payload_json,
    (
      select
        e.event_id
      from
        events e
      where
        e.session_id = snapshot_rows.session_id
      order by
        e.sequence_number desc
      limit
        1
    ) as last_event_id
  from
    snapshot_rows
)
insert into
  metric_snapshots (
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
  (
    '97000000-0000-0000-0000-' || lpad(snapshot_number :: text, 12, '0')
  ) :: uuid,
  session_id,
  user_id,
  session_player_id,
  computed_at,
  metric_name,
  null,
  metric_payload_json :: jsonb,
  ruleset_version_id,
  last_event_id
from
  numbered_snapshots
order by
  snapshot_number;

do $$ begin if exists (
  select
    1
  from
    session_participant_balances
  where
    coins < 0
) then raise exception 'Audit failed: Negative participant cash balance detected!';

end if;

if exists (
  select
    1
  from
    session_participant_need_purchases
  where
    ruleset_need_id is null
) then raise exception 'Audit failed: Some need purchases are not mapped to ruleset needs!';

end if;

if exists (
  select
    1
  from
    session_participant_financial_goals
  where
    ruleset_financial_goal_id is null
) then raise exception 'Audit failed: Some financial goals are not mapped to ruleset goals!';

end if;

if exists (
  select
    1
  from
    session_participant_loans
  where
    ruleset_sharia_loan_id is null
) then raise exception 'Audit failed: Some sharia loans are not mapped to ruleset sharia loans!';

end if;

if exists (
  select
    1
  from
    session_participant_insurances
  where
    ruleset_insurance_product_id is null
) then raise exception 'Audit failed: Some insurances are not mapped to ruleset insurance products!';

end if;

end;

$$;

commit;

