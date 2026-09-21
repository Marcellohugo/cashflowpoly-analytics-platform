-- Fungsi file: Membentuk 24 sesi demo Hadziq dan Pratama, memainkan kejadian yang konsisten, lalu menyiapkan hasil untuk rekalkulasi API.
-- Pilihan pseudoacak memakai hash tetap agar hasil dapat direproduksi; dua sesi pertama menjadi acuan regresi.
begin;

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
      'hadziq',
      'pratama',
      'nadia',
      'farhan',
      'marco',
      'marcello',
      'hugo',
      'manalu'
    )
    and user_id not in (
      '90000000-0000-0000-0000-000000000001' :: uuid,
      '90000000-0000-0000-0000-000000000002' :: uuid,
      '90000000-0000-0000-0000-000000000021' :: uuid,
      '90000000-0000-0000-0000-000000000022' :: uuid,
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

-- Dua puluh empat identitas tetap; tidak menyentuh sesi di luar scope demo ini.
insert into seed_session_scope(session_id)
select ('91000000-0000-0000-0000-' || lpad(n::text,12,'0'))::uuid
from generate_series(1,24) n;

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
    'hadziq',
    'Hadziq',
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

-- Akun kedua memiliki peserta sendiri serta dua peserta bersama untuk menguji isolasi instruktur.
insert into app_users(user_id,username,display_name,password_hash,role,is_active,is_demo,created_at)
values
('90000000-0000-0000-0000-000000000002','pratama','Pratama',crypt('SeedLocal!2026',gen_salt('bf',10)),'INSTRUCTOR',true,true,'2026-01-01'),
('90000000-0000-0000-0000-000000000021','nadia','Nadia',crypt('SeedLocal!2026',gen_salt('bf',10)),'PLAYER',true,true,'2026-01-01'),
('90000000-0000-0000-0000-000000000022','farhan','Farhan',crypt('SeedLocal!2026',gen_salt('bf',10)),'PLAYER',true,true,'2026-01-01')
on conflict(user_id) do update set username=excluded.username, display_name=excluded.display_name,
password_hash=excluded.password_hash, role=excluded.role, is_active=true, is_demo=true;
update app_users set is_demo=true where user_id in (
'90000000-0000-0000-0000-000000000001','90000000-0000-0000-0000-000000000011',
'90000000-0000-0000-0000-000000000012','90000000-0000-0000-0000-000000000013','90000000-0000-0000-0000-000000000014');

-- Sepuluh set aturan pribadi per instruktur: lima Pemula dan lima Mahir.
-- Semua dipakai oleh sesi selesai, persiapan, atau sedang berjalan.
-- Versi pertama tetap utuh agar sesi pengguna yang sudah memakainya tidak berubah.
create temporary table seed_rulesets on commit drop as
select n, (n-1)%10+1 as ordinal,
  ('97000000-0000-0000-0000-'||lpad(n::text,12,'0'))::uuid as ruleset_id,
  ('98000000-0000-0000-0000-'||lpad(n::text,12,'0'))::uuid as legacy_version_id,
  ((case when n<=2 then '98000000' else '98100000' end)||'-0000-0000-0000-'||lpad(n::text,12,'0'))::uuid as version_id,
  ('90000000-0000-0000-0000-'||lpad((case when n<=10 then 1 else 2 end)::text,12,'0'))::uuid as instructor_id,
  (case when n%2=1 then 'f5b4c67b-0825-4970-9f07-3b68e8fcb524' else '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' end)::uuid as source_version
from generate_series(1,20) n;

insert into rulesets(ruleset_id,name,description,instructor_user_id,created_by_user_id,created_at)
select r.ruleset_id,u.display_name||' - '||initcap(v.mode)||' - '||
  (array['Mengatur Belanja','Mencoba Usaha','Menyiapkan Kebutuhan','Meninjau Keputusan','Latihan Mandiri'])[(r.ordinal+1)/2],
  'Set latihan '||initcap(v.mode)||' milik '||u.display_name||'. '||
  'Digunakan pada sesi latihan dengan kondisi dan modal yang bervariasi.',
  r.instructor_id,r.instructor_id,timestamptz '2026-01-01 00:00:00+00'+r.n*interval '1 minute'
from seed_rulesets r join app_users u on u.user_id=r.instructor_id
join ruleset_versions v on v.ruleset_version_id=r.source_version
on conflict(ruleset_id) do nothing;

insert into ruleset_versions(ruleset_version_id,ruleset_id,version,status,mode,schema_version,config_hash,change_note,published_at,created_at,created_by_user_id)
select r.legacy_version_id,r.ruleset_id,1,case when r.n<=2 then 'ACTIVE' else 'ARCHIVED' end,v.mode,v.schema_version,v.config_hash,
  'Acuan mode untuk latihan seed 2.',timestamptz '2026-01-01 00:00:00+00',timestamptz '2026-01-01 00:00:00+00',r.instructor_id
from seed_rulesets r join ruleset_versions v on v.ruleset_version_id=r.source_version
on conflict(ruleset_version_id) do nothing;

-- Versi kedua hanya mengganti modal dan hasil kerja lepas. Harga kartu serta batas aksi
-- tetap mengikuti acuan supaya perbedaan hasil masih mudah dibandingkan.
update ruleset_versions v set status='ARCHIVED'
from seed_rulesets r where r.n>2 and v.ruleset_version_id=r.legacy_version_id and v.status='ACTIVE';
insert into ruleset_versions(ruleset_version_id,ruleset_id,version,status,mode,schema_version,config_hash,change_note,published_at,created_at,created_by_user_id)
select r.version_id,r.ruleset_id,2,'ACTIVE',v.mode,v.schema_version,
  encode(digest(v.config_hash||':seed2-v2:'||r.n::text,'sha256'),'hex'),
  'Variasi modal awal dan pendapatan kerja lepas untuk pengujian.',
  timestamptz '2026-01-02',timestamptz '2026-01-02',r.instructor_id
from seed_rulesets r join ruleset_versions v on v.ruleset_version_id=r.source_version where r.n>2
on conflict(ruleset_version_id) do nothing;

-- Salin komponen dalam urutan dependensi. Seluruh ID dan FK UUID dipetakan konsisten
-- ke versi tujuan; katalog global actions tetap memakai kode teks yang sama.
do $$
declare r record; component_table text; columns_sql text; values_sql text;
begin
  for r in
    select legacy_version_id as version_id,source_version from seed_rulesets
    union select version_id,source_version from seed_rulesets
  loop
    foreach component_table in array array[
      'ruleset_game_settings','ruleset_player_ordering_rules','ruleset_actions','ruleset_game_assets',
      'ruleset_ingredients','ruleset_orders','ruleset_order_requirements','ruleset_needs','ruleset_need_set_bonuses',
      'ruleset_collection_missions','ruleset_collection_mission_requirements','ruleset_financial_goals',
      'ruleset_narratives','ruleset_narrative_scenes','ruleset_trigger_conditions','ruleset_gold_prices',
      'ruleset_gold_assets','ruleset_rank_points','ruleset_tie_breakers','ruleset_sharia_loans',
      'ruleset_insurance_products','ruleset_life_risks'] loop
      select string_agg(format('%I',attname),',' order by attnum),
        string_agg(case when attname='ruleset_version_id' then '$1'
          when atttypid='uuid'::regtype then format('md5($1::text || '':'' || src.%I::text)::uuid',attname)
          else format('src.%I',attname) end,',' order by attnum)
      into columns_sql,values_sql
      from pg_attribute where attrelid=component_table::regclass and attnum>0 and not attisdropped;
      execute format('insert into %I (%s) select %s from %I src where src.ruleset_version_id=$2 on conflict do nothing',
        component_table,columns_sql,values_sql,component_table) using r.version_id,r.source_version;
    end loop;
  end loop;
end $$;

update ruleset_game_settings target
set starting_cash=source.starting_cash+(r.ordinal+1)/2+case when r.n>10 then 2 else 0 end,
    freelance_income=1+(r.ordinal/2+r.n/10)%3
from seed_rulesets r join ruleset_game_settings source on source.ruleset_version_id=r.source_version
where r.n>2 and target.ruleset_version_id=r.version_id;

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
    '98000000-0000-0000-0000-000000000001' :: uuid,
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
    '98000000-0000-0000-0000-000000000002' :: uuid,
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
    when '91000000-0000-0000-0000-000000000001' :: uuid then '2026-01-29T02:20:00Z' :: timestamptz
    when '91000000-0000-0000-0000-000000000002' :: uuid then '2026-02-26T02:20:00Z' :: timestamptz
    else ended_at
  end
where
  session_id in (
    '91000000-0000-0000-0000-000000000001' :: uuid,
    '91000000-0000-0000-0000-000000000002' :: uuid
  );

-- Harga contoh mengikuti katalog dan risiko yang benar-benar sudah terjadi pada sesi tersebut.
create or replace function pg_temp.seed_ingredient_price(sid uuid, version_id uuid, participant_id uuid, day_no int, card_id text)
returns integer language sql as $fn$
  select greatest(0, ingredient.purchase_price + coalesce((
    select sum(effect.value_delta) from session_rule_effects effect
    join events source on source.session_id = effect.session_id and source.event_id = effect.source_event_id
    where effect.session_id = sid and effect.effect_type = 'INGREDIENT_PRICE_MODIFIER'
      and effect.is_active and day_no between effect.starts_day and effect.ends_day
      and (effect.target_scope = 'ALL_PLAYERS'
        or (effect.target_scope = 'SELF' and source.session_player_id = participant_id)
        or (effect.target_scope = 'OTHER_PLAYERS' and source.session_player_id is distinct from participant_id))
  ), 0))::integer
  from ruleset_ingredients ingredient
  where ingredient.ruleset_version_id = version_id and ingredient.ingredient_code = card_id
$fn$;

do $$ declare v_event record;

begin
perform ensure_session_card_positions_initialized(scope.session_id)
from seed_session_scope scope;

for v_event in with session_context as (
  select
    *
  from
    (
      values
        (
          'PEMULA',
          '91000000-0000-0000-0000-000000000001' :: uuid,
          '98000000-0000-0000-0000-000000000001' :: uuid,
          '95000000-0000-0000-0000-',
          '2026-01-05T01:00:00Z' :: timestamptz
        ),
        (
          'MAHIR',
          '91000000-0000-0000-0000-000000000002' :: uuid,
          '98000000-0000-0000-0000-000000000002' :: uuid,
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
scenario_event_seed_raw as (
  select
    *
  from
    (
      values
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
          0,
          -40,
          1,
          1,
          'SYSTEM',
          'BagikanTieBreaker',
          'BagikanTieBreaker',
          '{"number":1,"card_code":"tie_breaker_1"}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          0,
          -39,
          1,
          2,
          'SYSTEM',
          'BagikanTieBreaker',
          'BagikanTieBreaker',
          '{"number":2,"card_code":"tie_breaker_2"}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          0,
          -38,
          1,
          3,
          'SYSTEM',
          'BagikanTieBreaker',
          'BagikanTieBreaker',
          '{"number":3,"card_code":"tie_breaker_3"}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          0,
          -37,
          1,
          4,
          'SYSTEM',
          'BagikanTieBreaker',
          'BagikanTieBreaker',
          '{"number":4,"card_code":"tie_breaker_4"}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          0,
          -8,
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
          -7,
          1,
          2,
          'SYSTEM',
          'SetupBahanAwal',
          'SetupBahanAwal',
          '{"card_id":"sayur","ingredient_name":"Sayur","amount":2,"setup":"INITIAL"}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          0,
          -6,
          1,
          3,
          'SYSTEM',
          'SetupBahanAwal',
          'SetupBahanAwal',
          '{"card_id":"tahu_tempe","ingredient_name":"Tahu Tempe","amount":3,"setup":"INITIAL"}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          0,
          -5,
          1,
          4,
          'SYSTEM',
          'SetupBahanAwal',
          'SetupBahanAwal',
          '{"card_id":"nasi_putih","ingredient_name":"Nasi Putih","amount":1,"setup":"INITIAL"}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          0,
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
          0,
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
          0,
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
          0,
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
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"nasi_putih","ingredient_name":"Nasi Putih","amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          0,
          6,
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
          0,
          7,
          1,
          2,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"nasi_putih","ingredient_name":"Nasi Putih","amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          0,
          8,
          2,
          2,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"telur","ingredient_name":"Telur","amount":4}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          0,
          9,
          1,
          3,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"daging","ingredient_name":"Daging","amount":5}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          0,
          10,
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
          0,
          12,
          1,
          4,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          0,
          13,
          2,
          4,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"sayur","ingredient_name":"Sayur","amount":2}' :: jsonb
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
          '{"amount":2}' :: jsonb
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
          '{"amount":3}' :: jsonb
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
          null,
          'LewatiTransaksiEmas',
          '{"note":"Menjaga saldo setelah pembelian bahan Hari 1"}' :: jsonb
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
          null,
          'LewatiTransaksiEmas',
          '{"note":"Saldo tidak cukup untuk membeli emas"}' :: jsonb
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
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"nasi_putih","ingredient_name":"Nasi Putih","amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          7,
          1,
          8,
          1,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"sayur","ingredient_name":"Sayur","amount":2}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          7,
          2,
          8,
          2,
          'PLAYER',
          'JualMasakan',
          'JualMasakan',
          '{"order_card_id":"gado_gado","required_ingredient_card_ids":["nasi_putih","sayur","tahu_tempe","telur"],"income":26}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          7,
          3,
          8,
          2,
          'PLAYER',
          'Kebutuhan',
          'Kebutuhan',
          '{"card_id":"gadget_2","amount":5,"points":4}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          7,
          4,
          8,
          3,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"nasi_putih","ingredient_name":"Nasi Putih","amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          7,
          5,
          8,
          3,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"telur","ingredient_name":"Telur","amount":4}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          7,
          6,
          8,
          4,
          'PLAYER',
          'JualMasakan',
          'JualMasakan',
          '{"order_card_id":"sego_penyet","required_ingredient_card_ids":["nasi_putih","tahu_tempe","telur"],"income":22}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          7,
          7,
          8,
          4,
          'PLAYER',
          'Kebutuhan',
          'Kebutuhan',
          '{"card_id":"sepeda_2","amount":5,"points":4}' :: jsonb
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
          '{"card_id":"jam_1","amount":6,"points":5}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          9,
          0,
          10,
          1,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"tahu_tempe","ingredient_name":"Tahu Tempe","amount":3}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          9,
          1,
          10,
          1,
          'PLAYER',
          'JualMasakan',
          'JualMasakan',
          '{"order_card_id":"nasi_pecel","required_ingredient_card_ids":["nasi_putih","sayur","tahu_tempe"],"income":20}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          9,
          2,
          10,
          2,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"daging","ingredient_name":"Daging","amount":5}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          9,
          3,
          10,
          2,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"telur","ingredient_name":"Telur","amount":4}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          9,
          4,
          10,
          3,
          'PLAYER',
          'JualMasakan',
          'JualMasakan',
          '{"order_card_id":"sego_penyet","required_ingredient_card_ids":["nasi_putih","tahu_tempe","telur"],"income":22}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          9,
          5,
          10,
          3,
          'PLAYER',
          'Kebutuhan',
          'Kebutuhan',
          '{"card_id":"buku_1","amount":2,"points":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          9,
          6,
          10,
          4,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"daging","ingredient_name":"Daging","amount":5}' :: jsonb
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
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"nasi_putih","ingredient_name":"Nasi Putih","amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          10,
          1,
          11,
          1,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"sayur","ingredient_name":"Sayur","amount":2}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          10,
          2,
          11,
          2,
          'PLAYER',
          'JualMasakan',
          'JualMasakan',
          '{"order_card_id":"soto_daging","required_ingredient_card_ids":["daging","telur"],"income":17}' :: jsonb
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
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"daging","ingredient_name":"Daging","amount":5}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          10,
          5,
          11,
          3,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"telur","ingredient_name":"Telur","amount":4}' :: jsonb
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
          '{"amount":3}' :: jsonb
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
          '{"amount":4}' :: jsonb
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
          4,
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
          3,
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
          '{"summary":"Marco Juara 1, Manalu Juara 2, Hugo Juara 3","winners":[{"rank":1,"player_name":"Marco","player_order_no":1,"points":7},{"rank":2,"player_name":"Manalu","player_order_no":4,"points":5},{"rank":3,"player_name":"Hugo","player_order_no":3,"points":2}]}' :: jsonb
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
          'JualMasakan',
          'JualMasakan',
          '{"order_card_id":"lontong_balap","required_ingredient_card_ids":["nasi_putih","sayur"],"income":13}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          14,
          1,
          15,
          1,
          'PLAYER',
          'Kebutuhan',
          'Kebutuhan',
          '{"card_id":"gadget_1","amount":4,"points":3}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          14,
          2,
          15,
          2,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"nasi_putih","ingredient_name":"Nasi Putih","amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          14,
          3,
          15,
          2,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"tahu_tempe","ingredient_name":"Tahu Tempe","amount":3}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          14,
          4,
          15,
          3,
          'PLAYER',
          'JualMasakan',
          'JualMasakan',
          '{"order_card_id":"soto_daging","required_ingredient_card_ids":["daging","telur"],"income":17}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          14,
          5,
          15,
          3,
          'PLAYER',
          'Kebutuhan',
          'Kebutuhan',
          '{"card_id":"tempat_pensil_1","amount":4,"points":3}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          14,
          6,
          15,
          4,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"nasi_putih","ingredient_name":"Nasi Putih","amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          14,
          7,
          15,
          4,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"telur","ingredient_name":"Telur","amount":4}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          15,
          0,
          16,
          1,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"sayur","ingredient_name":"Sayur","amount":2}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          15,
          1,
          16,
          1,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"sayur","ingredient_name":"Sayur","amount":2}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          15,
          2,
          16,
          2,
          'PLAYER',
          'JualMasakan',
          'JualMasakan',
          '{"order_card_id":"lontong_balap","required_ingredient_card_ids":["nasi_putih","sayur"],"income":13}' :: jsonb
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
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"telur","ingredient_name":"Telur","amount":4}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          15,
          5,
          16,
          3,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"tahu_tempe","ingredient_name":"Tahu Tempe","amount":3}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          15,
          6,
          16,
          4,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"tahu_tempe","ingredient_name":"Tahu Tempe","amount":3}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          15,
          7,
          16,
          4,
          'PLAYER',
          'JualMasakan',
          'JualMasakan',
          '{"order_card_id":"nasi_campur","required_ingredient_card_ids":["daging","nasi_putih","sayur","telur"],"income":27}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          16,
          0,
          17,
          1,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"tahu_tempe","ingredient_name":"Tahu Tempe","amount":3}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          16,
          1,
          17,
          1,
          'PLAYER',
          'JualMasakan',
          'JualMasakan',
          '{"order_card_id":"semanggi_surabaya","required_ingredient_card_ids":["sayur","sayur"],"income":14}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          16,
          2,
          17,
          2,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"daging","ingredient_name":"Daging","amount":5}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          16,
          3,
          17,
          2,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"telur","ingredient_name":"Telur","amount":4}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          16,
          4,
          17,
          3,
          'PLAYER',
          'JualMasakan',
          'JualMasakan',
          '{"order_card_id":"tahu_telur","required_ingredient_card_ids":["tahu_tempe","telur","telur"],"income":25}' :: jsonb
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
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"telur","ingredient_name":"Telur","amount":4}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          17,
          1,
          18,
          1,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"daging","ingredient_name":"Daging","amount":5}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          17,
          2,
          18,
          2,
          'PLAYER',
          'JualMasakan',
          'JualMasakan',
          '{"order_card_id":"rawon","required_ingredient_card_ids":["daging","tahu_tempe","telur"],"income":24}' :: jsonb
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
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"sayur","ingredient_name":"Sayur","amount":2}' :: jsonb
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
          'JualMasakan',
          'JualMasakan',
          '{"order_card_id":"rawon","required_ingredient_card_ids":["daging","tahu_tempe","telur"],"income":24}' :: jsonb
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
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"sayur","ingredient_name":"Sayur","amount":2}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          21,
          3,
          22,
          2,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"sayur","ingredient_name":"Sayur","amount":2}' :: jsonb
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
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"nasi_putih","ingredient_name":"Nasi Putih","amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          21,
          7,
          22,
          4,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"daging","ingredient_name":"Daging","amount":5}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          22,
          0,
          23,
          1,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"daging","ingredient_name":"Daging","amount":5}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          22,
          1,
          23,
          1,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"daging","ingredient_name":"Daging","amount":5}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          22,
          2,
          23,
          2,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"daging","ingredient_name":"Daging","amount":5}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          22,
          3,
          23,
          2,
          'PLAYER',
          'JualMasakan',
          'JualMasakan',
          '{"order_card_id":"semanggi_surabaya","required_ingredient_card_ids":["sayur","sayur"],"income":14}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          22,
          4,
          23,
          3,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"daging","ingredient_name":"Daging","amount":5}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          22,
          5,
          23,
          3,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"daging","ingredient_name":"Daging","amount":5}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          22,
          6,
          23,
          4,
          'PLAYER',
          'JualMasakan',
          'JualMasakan',
          '{"order_card_id":"rujak_cingur","required_ingredient_card_ids":["daging","nasi_putih","sayur","tahu_tempe"],"income":28}' :: jsonb
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
          'JualMasakan',
          'JualMasakan',
          '{"order_card_id":"sate_klopo","required_ingredient_card_ids":["daging","daging","nasi_putih"],"income":26}' :: jsonb
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
          'JualMasakan',
          'JualMasakan',
          '{"order_card_id":"sate_klopo","required_ingredient_card_ids":["daging","daging","nasi_putih"],"income":26}' :: jsonb
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
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"telur","ingredient_name":"Telur","amount":4}' :: jsonb
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
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"daging","ingredient_name":"Daging","amount":5}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          24,
          3,
          25,
          2,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"telur","ingredient_name":"Telur","amount":4}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          24,
          4,
          25,
          3,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"nasi_putih","ingredient_name":"Nasi Putih","amount":1}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          24,
          5,
          25,
          3,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"tahu_tempe","ingredient_name":"Tahu Tempe","amount":3}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          24,
          6,
          25,
          4,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"daging","ingredient_name":"Daging","amount":5}' :: jsonb
        ),
        (
          'PEMULA',
          null,
          24,
          7,
          25,
          4,
          'PLAYER',
          'BuangBahanMasakan',
          'BuangBahanMasakan',
          '{"card_id":"daging","amount":1}' :: jsonb
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
          '{"loan_code":"loan_syariah_10","loan_id":"loan-setup-mahir-001","principal":10,"repayment_amount":10,"duration_days":25,"penalty_points":15,"setup":"INITIAL"}' :: jsonb
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
          '{"loan_code":"loan_syariah_10","loan_id":"loan-setup-mahir-002","principal":10,"repayment_amount":10,"duration_days":25,"penalty_points":15,"setup":"INITIAL"}' :: jsonb
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
          '{"loan_code":"loan_syariah_10","loan_id":"loan-setup-mahir-003","principal":10,"repayment_amount":10,"duration_days":25,"penalty_points":15,"setup":"INITIAL"}' :: jsonb
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
          '{"loan_code":"loan_syariah_10","loan_id":"loan-setup-mahir-004","principal":10,"repayment_amount":10,"duration_days":25,"penalty_points":15,"setup":"INITIAL"}' :: jsonb
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
          0,
          -8,
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
          -7,
          1,
          2,
          'SYSTEM',
          'SetupBahanAwal',
          'SetupBahanAwal',
          '{"card_id":"sayur","ingredient_name":"Sayur","amount":2,"setup":"INITIAL"}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          0,
          -6,
          1,
          3,
          'SYSTEM',
          'SetupBahanAwal',
          'SetupBahanAwal',
          '{"card_id":"tahu_tempe","ingredient_name":"Tahu Tempe","amount":3,"setup":"INITIAL"}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          0,
          -5,
          1,
          4,
          'SYSTEM',
          'SetupBahanAwal',
          'SetupBahanAwal',
          '{"card_id":"nasi_putih","ingredient_name":"Nasi Putih","amount":1,"setup":"INITIAL"}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          0,
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
          0,
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
          0,
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
          0,
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
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"nasi_putih","ingredient_name":"Nasi Putih","amount":1}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          0,
          6,
          2,
          1,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"telur","ingredient_name":"Telur","amount":4}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          0,
          7,
          1,
          2,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"daging","ingredient_name":"Daging","amount":5}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          0,
          8,
          2,
          2,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"tahu_tempe","ingredient_name":"Tahu Tempe","amount":3}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          0,
          9,
          1,
          3,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"nasi_putih","ingredient_name":"Nasi Putih","amount":1}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          0,
          10,
          2,
          3,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"telur","ingredient_name":"Telur","amount":4}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          0,
          12,
          1,
          4,
          'PLAYER',
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          0,
          13,
          2,
          4,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"tahu_tempe","ingredient_name":"Tahu Tempe","amount":3}' :: jsonb
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
          null,
          1,
          1,
          2,
          1,
          'PLAYER',
          'JualMasakan',
          'JualMasakan',
          '{"order_card_id":"lontong_balap","required_ingredient_card_ids":["sayur","nasi_putih"],"income":13}' :: jsonb
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
          '{"card_id":"tahu_tempe","ingredient_name":"Tahu Tempe","amount":3}' :: jsonb
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
          3,
          2,
          2,
          'PLAYER',
          'JualMasakan',
          'JualMasakan',
          '{"order_card_id":"tahu_campur","required_ingredient_card_ids":["daging","tahu_tempe"],"income":16}' :: jsonb
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
          '{"card_id":"sayur","ingredient_name":"Sayur","amount":2}' :: jsonb
        ),
        (
          'MAHIR',
          'mahir-risk-003',
          1,
          6,
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
          6,
          2,
          3,
          'PLAYER',
          'JualMasakan',
          'JualMasakan',
          '{"order_card_id":"lontong_balap","required_ingredient_card_ids":["sayur","nasi_putih"],"income":13}' :: jsonb
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
          10,
          2,
          4,
          'PLAYER',
          'JualMasakan',
          'JualMasakan',
          '{"order_card_id":"tahu_campur","required_ingredient_card_ids":["daging","tahu_tempe"],"income":16}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          2,
          0,
          3,
          1,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"nasi_putih","ingredient_name":"Nasi Putih","amount":1}' :: jsonb
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
          '{"card_id":"baju_1","amount":2,"points":1}' :: jsonb
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
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
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
          '{"order_card_id":"soto_daging","required_ingredient_card_ids":["daging","telur"],"income":17}' :: jsonb
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
          '{"amount":3}' :: jsonb
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
          '{"gold_price":5}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          5,
          0,
          6,
          1,
          'PLAYER',
          null,
          'LewatiTransaksiEmas',
          '{"note":"Saldo tidak cukup untuk membeli emas"}' :: jsonb
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
          'JualEmas',
          'JualEmas',
          '{"trade_type":"SELL","unit_price":5,"qty":1,"amount":5,"note":"Menyiapkan pelunasan pinjaman sebelum opsi darurat"}' :: jsonb
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
          '{"trade_type":"BUY","unit_price":5,"qty":1,"amount":5}' :: jsonb
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
          'mahir-risk-022',
          7,
          4,
          8,
          null,
          'SYSTEM',
          'BukaHargaEmas',
          'BukaHargaEmas',
          '{"gold_price":5}' :: jsonb
        ),
        (
          'MAHIR',
          'mahir-risk-022',
          7,
          4,
          8,
          1,
          'PLAYER',
          'InvestasiEmas',
          'InvestasiEmas',
          '{"trade_type":"BUY","unit_price":5,"qty":1,"amount":5,"source":"risk_investasi_emas"}' :: jsonb
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
          'JualMasakan',
          'JualMasakan',
          '{"order_card_id":"nasi_goreng","required_ingredient_card_ids":["nasi_putih","telur"],"income":15}' :: jsonb
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
          null,
          7,
          11,
          8,
          4,
          'PLAYER',
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"daging","ingredient_name":"Daging","amount":5}' :: jsonb
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
          '{"card_id":"sayur","ingredient_name":"Sayur","amount":2}' :: jsonb
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
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
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
          'Menabung',
          'Menabung',
          '{"goal_id":"tujuan_28","amount":1}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          8,
          11,
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
          'mahir-risk-011',
          9,
          9,
          10,
          3,
          'PLAYER',
          'BayarRisiko',
          'BayarRisiko',
          '{}' :: jsonb
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
          'KerjaLepas',
          'KerjaLepas',
          '{"amount":1}' :: jsonb
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
          '{"card_id":"buku_1","amount":2,"points":1}' :: jsonb
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
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"daging","ingredient_name":"Daging","amount":5}' :: jsonb
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
          'BahanMasakan',
          'BahanMasakan',
          '{"card_id":"telur","ingredient_name":"Telur","amount":4}' :: jsonb
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
          null,
          15,
          7,
          16,
          2,
          'PLAYER',
          'Asuransi',
          'Asuransi',
          '{"policy_id":"INS-SEED-012","policy_instance_id":"INS-SEED-012","product_code":"multirisk_basic","premium":1,"coverage_type":"MULTIRISK"}' :: jsonb
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
          '{"card_id":"telur","ingredient_name":"Telur","amount":4}' :: jsonb
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
          '{"order_card_id":"rawon","required_ingredient_card_ids":["daging","telur","tahu_tempe"],"income":24}' :: jsonb
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
          '{"goal_id":"tujuan_35","amount":15}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          16,
          2,
          17,
          1,
          'SYSTEM',
          'TujuanFinansial',
          'TujuanFinansial',
          '{"goal_id":"tujuan_35","cost":35,"points":35}' :: jsonb
        ),
        (
          'MAHIR',
          null,
          16,
          1,
          17,
          1,
          'PLAYER',
          'Menabung',
          'Menabung',
          '{"goal_id":"tujuan_35","amount":5}' :: jsonb
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
          'mahir-risk-010',
          17,
          0,
          18,
          1,
          'PLAYER',
          'Asuransi',
          'Asuransi',
          '{}' :: jsonb
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
          'mahir-risk-014',
          17,
          2,
          18,
          2,
          'PLAYER',
          'Asuransi',
          'Asuransi',
          '{}' :: jsonb
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
          'mahir-risk-015',
          17,
          4,
          18,
          3,
          'PLAYER',
          'Asuransi',
          'Asuransi',
          '{}' :: jsonb
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
          'mahir-risk-016',
          17,
          6,
          18,
          4,
          'PLAYER',
          'Asuransi',
          'Asuransi',
          '{}' :: jsonb
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
          '{"card_id":"sayur","ingredient_name":"Sayur","amount":2}' :: jsonb
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
          null,
          22,
          2,
          23,
          1,
          'PLAYER',
          'Asuransi',
          'Asuransi',
          '{"policy_id":"INS-SEED-014","policy_instance_id":"INS-SEED-014","product_code":"multirisk_basic","premium":1,"coverage_type":"MULTIRISK"}' :: jsonb
        ),
        (
          'MAHIR',
          'mahir-risk-017',
          22,
          3,
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
          4,
          23,
          2,
          'PLAYER',
          'JualMasakan',
          'JualMasakan',
          '{"order_card_id":"nasi_campur","required_ingredient_card_ids":["nasi_putih","telur","daging","sayur"],"income":27}' :: jsonb
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
          null,
          22,
          6,
          23,
          2,
          'PLAYER',
          'Asuransi',
          'Asuransi',
          '{"policy_id":"INS-SEED-015","policy_instance_id":"INS-SEED-015","product_code":"multirisk_basic","premium":1,"coverage_type":"MULTIRISK"}' :: jsonb
        ),
        (
          'MAHIR',
          'mahir-risk-018',
          22,
          7,
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
          null,
          22,
          10,
          23,
          3,
          'PLAYER',
          'Asuransi',
          'Asuransi',
          '{"policy_id":"INS-SEED-016","policy_instance_id":"INS-SEED-016","product_code":"multirisk_basic","premium":1,"coverage_type":"MULTIRISK"}' :: jsonb
        ),
        (
          'MAHIR',
          'mahir-risk-019',
          22,
          11,
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
          null,
          22,
          14,
          23,
          4,
          'PLAYER',
          'Asuransi',
          'Asuransi',
          '{"policy_id":"INS-SEED-017","policy_instance_id":"INS-SEED-017","product_code":"multirisk_basic","premium":1,"coverage_type":"MULTIRISK"}' :: jsonb
        ),
        (
          'MAHIR',
          'mahir-risk-020',
          22,
          15,
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
          'mahir-risk-008',
          8,
          10,
          9,
          4,
          'PLAYER',
          'BayarRisiko',
          'BayarRisiko',
          '{}' :: jsonb
        ),
        (
          'MAHIR',
          'mahir-risk-006',
          8,
          99,
          9,
          3,
          'PLAYER',
          'GunakanOpsiDarurat',
          'GunakanOpsiDarurat',
          '{"option_type":"TAKE_SHARIA_LOAN","amount":10,"loan_code":"loan_syariah_10","loan_id":"loan-setup-mahir-003","principal":10,"repayment_amount":10,"duration_days":25,"penalty_points":15}' :: jsonb
        ),
        (
          'MAHIR',
          'mahir-risk-005',
          14,
          99,
          15,
          2,
          'PLAYER',
          'Asuransi',
          'Asuransi',
          '{}' :: jsonb
        ),
        (
          'MAHIR',
          'mahir-risk-013',
          21,
          99,
          22,
          1,
          'PLAYER',
          'GunakanOpsiDarurat',
          'GunakanOpsiDarurat',
          '{"option_type":"SELL_NEED","amount":1,"card_id":"buku_1"}' :: jsonb
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
scenario_event_seed_base as (
  select
    raw.session_key,
    raw.ref_key,
    raw.day_index,
    case
      when raw.action_type = 'Asuransi'
       and raw.ref_key is null then coalesce(
        (
          select risk_event.event_order * 10 - 1
          from scenario_event_seed_raw risk_event
          join scenario_event_seed_raw insurance_claim
            on insurance_claim.session_key = risk_event.session_key
           and insurance_claim.ref_key = risk_event.ref_key
           and insurance_claim.action_type = 'Asuransi'
          where risk_event.session_key = raw.session_key
            and risk_event.player_no = raw.player_no
            and risk_event.day_index = raw.day_index
            and risk_event.event_order = raw.event_order - 1
            and risk_event.action_type = 'RisikoKehidupan'
          limit 1
        ),
        raw.event_order * 10
      )
      else raw.event_order * 10
    end as event_order,
    raw.action_slot,
    raw.player_no,
    raw.actor_type,
    raw.action_id,
    raw.action_type,
    raw.payload
  from scenario_event_seed_raw raw
  where not (
    raw.ref_key is not null
    and raw.action_type in ('Asuransi', 'GunakanOpsiDarurat', 'BayarRisiko')
    and exists (
      select 1
      from scenario_event_seed_raw risk_raw
      join session_context sc on sc.session_key = risk_raw.session_key
      join ruleset_life_risks risk
        on risk.ruleset_version_id = sc.ruleset_version_id
       and lower(risk.risk_code) = lower(risk_raw.payload ->> 'risk_id')
      where risk_raw.session_key = raw.session_key
        and risk_raw.ref_key = raw.ref_key
        and risk_raw.action_type = 'RisikoKehidupan'
        and risk.effect_type = 'COIN_EFFECT'
        and risk.direction = 'OUT'
        and risk.target_scope = 'SELF'
    )
  )

  union all

  select
    raw.session_key,
    null,
    raw.day_index,
    raw.event_order * 10 - 2,
    0,
    null,
    'SYSTEM',
    'KartuMasukDiscard',
    'KartuMasukDiscard',
    jsonb_build_object(
      'slot_group',
      case raw.action_type
        when 'BahanMasakan' then 'INGREDIENT_MARKET'
        when 'Kebutuhan' then 'NEED_MARKET'
        else 'ORDER_MARKET'
      end,
      'slot_code',
      'SLOT_1',
      'reason',
      'MARKET_ROTATION'
    )
  from scenario_event_seed_raw raw
  where raw.actor_type = 'PLAYER'
    and raw.action_type in ('BahanMasakan', 'Kebutuhan', 'JualMasakan')

  union all

  select
    raw.session_key,
    null,
    raw.day_index,
    raw.event_order * 10 - 1,
    0,
    null,
    'SYSTEM',
    'IsiUlangPasar',
    'IsiUlangPasar',
    jsonb_build_object(
      'slot_group',
      case raw.action_type
        when 'BahanMasakan' then 'INGREDIENT_MARKET'
        when 'Kebutuhan' then 'NEED_MARKET'
        else 'ORDER_MARKET'
      end,
      'slot_code',
      'SLOT_1',
      'asset_type',
      case raw.action_type
        when 'BahanMasakan' then 'INGREDIENT'
        when 'Kebutuhan' then 'NEED'
        else 'ORDER'
      end,
      'asset_code',
      case
        when raw.action_type = 'JualMasakan' then raw.payload ->> 'order_card_id'
        else raw.payload ->> 'card_id'
      end
    )
  from scenario_event_seed_raw raw
  where raw.actor_type = 'PLAYER'
    and raw.action_type in ('BahanMasakan', 'Kebutuhan', 'JualMasakan')
),
risk_resolution_seed as (
  select
    resolution.session_key,
    resolution.ref_key,
    risk_event.day_index,
    risk_event.event_order * 10 + 1 as event_order,
    0 as action_slot,
    risk_event.player_no,
    'PLAYER' :: text as actor_type,
    case
      when resolution.ref_key in ('mahir-risk-006', 'mahir-risk-013') then 'BayarRisiko'
      else resolution.action_id
    end as action_id,
    case
      when resolution.ref_key in ('mahir-risk-006', 'mahir-risk-013') then 'BayarRisiko'
      else resolution.action_type
    end as action_type,
    case
      when resolution.ref_key in ('mahir-risk-006', 'mahir-risk-013') then '{}' :: jsonb
      else resolution.payload
    end as payload
  from scenario_event_seed_raw resolution
  join scenario_event_seed_raw risk_event
    on risk_event.session_key = resolution.session_key
   and risk_event.ref_key = resolution.ref_key
   and risk_event.action_type = 'RisikoKehidupan'
  join session_context sc on sc.session_key = risk_event.session_key
  join ruleset_life_risks risk
    on risk.ruleset_version_id = sc.ruleset_version_id
   and lower(risk.risk_code) = lower(risk_event.payload ->> 'risk_id')
  where resolution.action_type in ('Asuransi', 'GunakanOpsiDarurat', 'BayarRisiko')
    and resolution.ref_key is not null
    and risk.effect_type = 'COIN_EFFECT'
    and risk.direction = 'OUT'
    and risk.target_scope = 'SELF'
),
scenario_event_seed as (
  select * from scenario_event_seed_base
  union all
  select * from risk_resolution_seed
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
          'nasi_pecel'
        ),
        (
          7,
          'ORDER_MARKET',
          'SLOT_2',
          'ORDER',
          'rujak_cingur'
        ),
        (
          8,
          'ORDER_MARKET',
          'SLOT_3',
          'ORDER',
          'gado_gado'
        ),
        (
          9,
          'ORDER_MARKET',
          'SLOT_4',
          'ORDER',
          'nasi_campur'
        ),
        (
          10,
          'ORDER_MARKET',
          'SLOT_5',
          'ORDER',
          'tahu_telur'
        ),
        (11, 'NEED_MARKET', 'SLOT_1', 'NEED', 'tempat_makan_1'),
        (12, 'NEED_MARKET', 'SLOT_2', 'NEED', 'tempat_makan_2'),
        (13, 'NEED_MARKET', 'SLOT_3', 'NEED', 'tas_1'),
        (14, 'NEED_MARKET', 'SLOT_4', 'NEED', 'tas_2'),
        (15, 'NEED_MARKET', 'SLOT_5', 'NEED', 'sepeda_1')
    ) slots(
      sort_order,
      slot_group,
      slot_code,
      asset_type,
      asset_code
    )
),
final_market_seed as (
  select
    setup.session_key,
    null :: text as ref_key,
    24 as day_index,
    78 as event_order,
    0 as action_slot,
    null :: int as player_no,
    'SYSTEM' :: text as actor_type,
    'KartuMasukDiscard' :: text as action_id,
    'KartuMasukDiscard' :: text as action_type,
    jsonb_build_object(
      'slot_group', setup.payload ->> 'slot_group',
      'slot_code', setup.payload ->> 'slot_code',
      'reason', 'FINAL_MARKET_RESET'
    ) as payload
  from setup_market_seed setup

  union all

  select
    setup.session_key,
    null,
    24,
    79,
    0,
    null,
    'SYSTEM',
    'IsiUlangPasar',
    'IsiUlangPasar',
    (setup.payload - 'setup') || jsonb_build_object('reason', 'FINAL_MARKET_RESET')
  from setup_market_seed setup
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
          and (
            action_type in (
              'BahanMasakan',
              'BuangBahanMasakan',
              'JualMasakan',
              'Kebutuhan',
              'KerjaLepas',
              'Menabung',
              'TarikTabungan',
              'TujuanFinansial',
              'BayarPinjaman'
            )
            or (
            action_type in ('Asuransi', 'PinjamanSyariah')
              and ref_key is null
              and not (payload ? 'risk_event_id')
              and not coalesce(payload ->> 'setup' = 'INITIAL', false)
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
          'BahanMasakan',
          'BuangBahanMasakan',
          'JualMasakan',
          'Kebutuhan',
          'KerjaLepas',
          'Menabung',
          'TarikTabungan',
          'TujuanFinansial',
          'BayarPinjaman'
        )
        or (
          es.action_type in ('Asuransi', 'PinjamanSyariah')
          and es.ref_key is null
          and not (es.payload ? 'risk_event_id')
          and not coalesce(es.payload ->> 'setup' = 'INITIAL', false)
        )
      ) then count(*) filter (
          where
            es.actor_type = 'PLAYER'
            and (
              es.action_type in (
                'BahanMasakan',
                'BuangBahanMasakan',
                'JualMasakan',
                'Kebutuhan',
                'KerjaLepas',
                'Menabung',
                'TarikTabungan',
                'TujuanFinansial',
                'BayarPinjaman'
              )
              or (
              es.action_type in ('Asuransi', 'PinjamanSyariah')
                and es.ref_key is null
                and not (es.payload ? 'risk_event_id')
                and not coalesce(es.payload ->> 'setup' = 'INITIAL', false)
              )
            )
        ) over (
          partition by es.session_key,
          es.day_index,
          es.player_no
          order by
            es.event_order,
            es.action_type,
            coalesce(es.ref_key, '')
          rows between unbounded preceding and current row
        )
      :: int
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
  where es.action_type not in ('AmbilKartuDariDeck', 'KartuMasukDiscard', 'IsiUlangPasar')
),
numbered_events as (
  select
    *,
    event_number - 1 as sequence_number,
    (
      event_uuid_prefix || lpad(event_number :: text, 12, '0')
    ) :: uuid as event_id,
    case
      when action_type = 'MulaiSesi'
      or action_type = 'BagikanTieBreaker'
      or action_type like 'Setup%'
      or coalesce(payload ->> 'setup', '') like 'INITIAL%'
      then 0
      else day_index + 1
    end as stored_day_index,
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
      when e.action_type in (
        'Asuransi',
        'GunakanOpsiDarurat',
        'BayarRisiko',
        'BukaHargaEmas',
        'InvestasiEmas',
        'JualEmas'
      )
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
  re.sequence_number loop
  if v_event.action_type = 'BahanMasakan' then
    v_event.payload := jsonb_set(v_event.payload, '{amount}', to_jsonb(pg_temp.seed_ingredient_price(
      v_event.session_id, v_event.ruleset_version_id, v_event.session_player_id, v_event.day_index, v_event.payload->>'card_id')));
  end if;
  perform apply_game_event(
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

-- Ulangi skenario legal dengan peran pemain dan keputusan sukarela yang bervariasi.
-- Semua nominal kartu/harga/pinjaman tetap mengikuti acuan; tidak ada angka snapshot buatan.
create temporary table seed_variants on commit drop as
select n, ('98100000-0000-0000-0000-'||lpad((case when n<=8 then n else n+2 end)::text,12,'0'))::uuid as version_id,
       ('91000000-0000-0000-0000-'||lpad(n::text,12,'0'))::uuid as session_id,
       ('91000000-0000-0000-0000-'||lpad((1+(n+1)%2)::text,12,'0'))::uuid as template_id,
       ('90000000-0000-0000-0000-'||lpad((case when n<=8 then 1 else 2 end)::text,12,'0'))::uuid as instructor_id,
       (case when n<=8 then n-1 else n-9 end) as ordinal,
       timestamptz '2026-01-05 01:00:00+00' +
         (case when n<=8 then (n-1)*28 else (n-9)*28+7 end)*interval '1 day' +
         (case when n<=8 then 0 else 150 end)*interval '1 minute' as starts,
       'ENDED'::text as status,25 as last_day
from generate_series(3,16) n;

-- Tambahan tetap mempertahankan empat sesi selesai per mode dan instruktur.
-- Dua sesi persiapan dan dua sesi berjalan melengkapi contoh filter serta data parsial.
insert into seed_variants(n,version_id,session_id,template_id,instructor_id,ordinal,starts,status,last_day)
select n, ('98100000-0000-0000-0000-'||lpad((case when n<=20 then 9 else 19 end+(n-17)%2)::text,12,'0'))::uuid,
  ('91000000-0000-0000-0000-'||lpad(n::text,12,'0'))::uuid,
  ('91000000-0000-0000-0000-'||lpad((1+(n-17)%2)::text,12,'0'))::uuid,
  ('90000000-0000-0000-0000-'||lpad((case when n<=20 then 1 else 2 end)::text,12,'0'))::uuid,
  (n-17)%4,timestamptz '2026-08-10 01:00:00+00'+(n-17)*interval '1 day',
  case when (n-17)%4<2 then 'CREATED' else 'STARTED' end,case when n%2=0 then 3 else 2 end
from generate_series(17,24) n;

insert into sessions(session_id,session_name,ruleset_version_id,mode,status,player_count,instructor_user_id,created_at)
select v.session_id, (case when v.status='CREATED' then 'Persiapan ' when v.status='STARTED' then 'Latihan Berjalan ' else 'Latihan ' end)||u.display_name||' - '||initcap(t.mode)||' - Pertemuan '||((v.ordinal/2)+1)||' - '||
       (array['Mengatur belanja','Mencoba usaha','Menyiapkan kebutuhan','Meninjau keputusan'])[(v.ordinal/2)%4+1],
       v.version_id,t.mode,'CREATED',0,v.instructor_id,v.starts-interval '15 minutes'
from seed_variants v join sessions t on t.session_id=v.template_id join app_users u on u.user_id=v.instructor_id;

create temporary table seed_variant_players on commit drop as
with pool as (
  select v.*, u.user_id,u.display_name,
         row_number() over(partition by v.session_id order by md5(v.n::text||':'||u.user_id::text))::int as seat
  from seed_variants v join app_users u on u.user_id = any(case when v.instructor_id='90000000-0000-0000-0000-000000000001'::uuid then array[
    '90000000-0000-0000-0000-000000000011'::uuid,'90000000-0000-0000-0000-000000000012'::uuid,
    '90000000-0000-0000-0000-000000000013'::uuid,'90000000-0000-0000-0000-000000000014'::uuid] else array[
    '90000000-0000-0000-0000-000000000011'::uuid,'90000000-0000-0000-0000-000000000013'::uuid,
    '90000000-0000-0000-0000-000000000021'::uuid,'90000000-0000-0000-0000-000000000022'::uuid] end)
)
select pool.*, md5('seed2-participant:'||pool.session_id::text||':'||pool.user_id::text)::uuid as participant_id,
       sp.user_id as template_user_id,sp.session_participant_id as template_participant_id
from pool join session_participants sp on sp.session_id=pool.template_id and sp.player_order_no=pool.seat;
insert into session_participants(session_participant_id,session_id,user_id,player_name,player_order_no,joined_at)
select participant_id,session_id,user_id,display_name,seat,starts-interval '2 minutes' from seed_variant_players;
update sessions s set status=v.status,player_count=4,
started_at=case when v.status<>'CREATED' then v.starts end,
ended_at=case when v.status='ENDED' then v.starts+interval '24 days 2 hours' end
from seed_variants v where s.session_id=v.session_id;

-- Menyusun peringkat berdasarkan donasi sesi yang sedang dimainkan, termasuk pemecah seri.
create or replace function pg_temp.seed_donation_ranks(sid uuid, day_no int)
returns table(participant_id uuid, user_id uuid, player_name text, seat int, rank_no bigint, points int)
language sql as $fn$
  select ranked.participant_id,ranked.user_id,ranked.player_name,ranked.seat,ranked.rank_no,coalesce(r.points,0)
  from (
    select sp.session_participant_id as participant_id,sp.user_id,sp.player_name::text,sp.player_order_no as seat,
      row_number() over(order by sum((e.payload->>'amount')::int) desc,tb.tie_number desc,sp.user_id) as rank_no
    from events e join session_participants sp on sp.session_participant_id=e.session_player_id
    join session_participant_tie_breakers tb on tb.session_participant_id=sp.session_participant_id
    where e.session_id=sid and e.day_index=day_no and e.action_type='JumatBerkah'
    group by sp.session_participant_id,sp.user_id,sp.player_name,sp.player_order_no,tb.tie_number
  ) ranked join sessions s on s.session_id=sid
  left join ruleset_rank_points r on r.ruleset_version_id=s.ruleset_version_id and r.rank_type='DONATION' and r.rank_no=ranked.rank_no
$fn$;

do $$
declare v record; e record; p record; win record;
  body jsonb; target_action uuid; target_event uuid; target_player uuid; target_user uuid;
  seq bigint; slot int; turn_no int; coins_now int; future_cost int; choice int; actions_used int;
  at_time timestamptz; previous_time timestamptz;
begin
  for v in select sv.*,s.started_at as template_start,gs.starting_cash,gs.freelance_income
    from seed_variants sv join sessions s on s.session_id=sv.template_id
    join ruleset_game_settings gs on gs.ruleset_version_id=sv.version_id
    where sv.status<>'CREATED' order by sv.n loop
    perform ensure_session_card_positions_initialized(v.session_id);
    seq:=0; previous_time:=v.starts-interval '1 hour';
    for e in select * from events where session_id=v.template_id and day_index<=v.last_day order by sequence_number loop
      target_event:=md5('seed2-event:'||v.session_id::text||':'||e.event_id::text)::uuid;
      select * into p from seed_variant_players where session_id=v.session_id and template_user_id=e.user_id;
      target_player:=p.participant_id; target_user:=p.user_id;
      body:=e.payload;
      if e.action_type='BahanMasakan' then
        body:=jsonb_set(body,'{amount}',to_jsonb(pg_temp.seed_ingredient_price(
          v.session_id,v.version_id,target_player,e.day_index,body->>'card_id')));
      elsif e.action_type='KerjaLepas' then
        body:=jsonb_set(body,'{amount}',to_jsonb(v.freelance_income));
      elsif e.action_type='SetupModalAwal' then
        body:=jsonb_set(body,'{amount}',to_jsonb(v.starting_cash));
      end if;
      choice:=get_byte(decode(md5(v.n::text||':'||e.event_id::text),'hex'),0);
      if body ? 'risk_event_id' then
        body:=jsonb_set(body,'{risk_event_id}',to_jsonb(md5('seed2-event:'||v.session_id::text||':'||(body->>'risk_event_id'))::uuid::text));
      end if;
      if e.action_type='JumatBerkah' then
        -- Sumbangan dipilih dari satu koin sampai nominal acuan, sesuai kemampuan masing-masing.
        body:=jsonb_set(body,'{amount}',to_jsonb(1+choice%greatest(1,(body->>'amount')::int)));
      elsif e.action_type='KerjaLepas' and e.day_index>=15 and choice%3=0 then
        select coins into coins_now from session_participant_balances where session_participant_id=target_player;
        select coalesce(sum(cp.amount),0) into future_cost from event_cashflow_projections cp
        join events remaining on remaining.event_pk=cp.event_pk
        where remaining.session_id=v.template_id and remaining.user_id=e.user_id
          and remaining.sequence_number>e.sequence_number and cp.direction='OUT';
        -- Pemain yang sudah punya cadangan kadang tidak menghabiskan seluruh jatah aksi.
        if coins_now>=future_cost+5 then continue; end if;
      elsif e.action_type='PoinPeringkatDonasi' then
        select * into win from pg_temp.seed_donation_ranks(v.session_id,e.day_index) where rank_no=(body->>'rank')::int;
        target_player:=win.participant_id; target_user:=win.user_id;
        body:=jsonb_build_object('rank',win.rank_no,'points',win.points);
      elsif e.action_type='UmumkanJuaraDonasi' then
        select jsonb_build_object('summary',string_agg(player_name||' Juara '||rank_no,', ' order by rank_no),
          'winners',jsonb_agg(jsonb_build_object('rank',rank_no,'points',points,'player_name',player_name,'player_order_no',seat) order by rank_no))
        into body from pg_temp.seed_donation_ranks(v.session_id,e.day_index) where rank_no<=3;
      elsif e.action_type='AkhirGiliran' then
        select count(*) into actions_used from events where session_id=v.session_id
          and day_index=e.day_index and actor_type='PLAYER' and action_slot>0;
        body:=jsonb_set(body,'{used}',to_jsonb(actions_used));
      elsif e.action_type='MulaiSesi' then
        body:=jsonb_build_object('start_note','Latihan dengan keputusan pemain bervariasi; skenario demo dapat diulang.');
      elsif e.action_type='AkhiriSesi' then
        body:=jsonb_build_object('end_note','Sesi latihan selesai; hasil dihitung dari transaksi dan kepemilikan pemain.');
      end if;
      slot:=0;
      if e.action_slot>0 then
        select count(*)+1 into slot from events where session_id=v.session_id and session_player_id=target_player
          and day_index=e.day_index and actor_type='PLAYER' and action_slot>0;
      end if;
      turn_no:=case when e.actor_type='PLAYER' then p.seat else 0 end;
      -- Jeda keputusan bervariasi, tetapi waktu dan urutan kejadian tetap maju.
      at_time:=greatest(e.timestamp-v.template_start+v.starts+(choice%39)*interval '1 second',previous_time+interval '5 seconds');
      select target.ruleset_action_id into target_action from ruleset_actions source
      join ruleset_actions target on target.action_id=source.action_id and target.ruleset_version_id=v.version_id
      where source.ruleset_action_id=e.ruleset_action_id;
      perform apply_game_event(target_event,v.session_id,target_player,target_user,e.actor_type,at_time,e.day_index,
        e.weekday,turn_no,slot,seq,target_action,e.action_type,v.version_id,body,null,target_event);
      previous_time:=at_time; seq:=seq+1;
    end loop;
    update sessions set ended_at=previous_time where session_id=v.session_id and status='ENDED';
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
    sp.session_id in (select scope.session_id from seed_session_scope scope join sessions finished using(session_id) where finished.status='ENDED')
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
  session_id in (select session_id from seed_session_scope);

delete from
  session_final_scores
where
  session_id in (select session_id from seed_session_scope);

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
          resolve_gold_points(coalesce(sum(spgh.quantity), 0) :: int, s.ruleset_version_id)
        from
          session_participant_gold_holdings spgh
        where
          spgh.session_participant_id = sp.session_participant_id
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
    join sessions s on s.session_id = sp.session_id
    left join events e on e.session_id = sp.session_id
    and e.user_id = sp.user_id
    left join session_participant_tie_breakers sptb on sptb.session_id = sp.session_id
    and sptb.session_participant_id = sp.session_participant_id
    left join seed_pension_rank_points spr on spr.session_id = sp.session_id
    and spr.session_participant_id = sp.session_participant_id
  where
    sp.session_id in (select scope.session_id from seed_session_scope scope join sessions finished using(session_id) where finished.status='ENDED')
  group by
    sp.session_id,
    sp.session_participant_id,
    s.ruleset_version_id,
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
        (primary_need_count / 3) :: int * 2
      ) + (
        (secondary_need_count / 3) :: int * 2
      ) + (
        (tertiary_need_count / 3) :: int * 2
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

-- Snapshot analitika wajib dihitung melalui --recalculate-analytics setelah seed.

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
