-- Fungsi file: Menyimpan aktivitas IDN dan membedakan event setup dari aktivitas bermain untuk penutupan sesi.
alter table sessions
    add column last_activity_at timestamptz not null default clock_timestamp(),
    add column end_reason varchar(32) null;

create index ix_sessions_active_last_activity on sessions (last_activity_at)
    where status in ('CREATED', 'STARTED');

-- Hanya event setup otomatis pada hari 0 yang dikecualikan; event SYSTEM saat bermain tetap dihitung.
create function session_has_gameplay(target_session_id uuid) returns boolean
language sql stable as $$
    select exists (
        select 1 from events where session_id = target_session_id
        and not (actor_type = 'SYSTEM' and day_index = 0 and action_type in (
            'MulaiSesi', 'BagikanTieBreaker', 'SetupBahanAwal', 'SetupEmasAwal',
            'SetupMisiAwal', 'SetupPinjamanAwal', 'SetupAsuransiAwal'))
    );
$$;
