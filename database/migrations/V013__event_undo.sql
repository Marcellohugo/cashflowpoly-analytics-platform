-- Fungsi file: Menyimpan snapshot sebelum event, audit undo permanen, dan pemulihan projection sesi secara atomik.
create table public.event_undo_snapshots (
    session_id uuid not null references public.sessions (session_id) on delete cascade,
    event_id uuid not null,
    before_state jsonb not null,
    primary key (session_id, event_id),
    constraint ck_event_undo_snapshots_state check (jsonb_typeof(before_state) = 'object')
);

-- Audit sengaja tanpa foreign key sesi/pengguna: bukti undo tetap ada setelah sesi kosong dihapus.
create table public.event_undos (
    undo_id uuid primary key,
    session_id uuid not null,
    event_id uuid not null,
    instructor_user_id uuid not null,
    client_request_id varchar(120) not null,
    expected_state_version bigint not null,
    reason varchar(500) not null,
    original_event jsonb not null,
    original_cashflows jsonb not null,
    original_asset_references jsonb not null,
    state_version bigint not null,
    next_sequence_number bigint not null,
    created_at timestamptz not null default clock_timestamp(),
    unique (session_id, event_id),
    unique (instructor_user_id, client_request_id),
    constraint ck_event_undos_request check (nullif(btrim(client_request_id), '') is not null),
    constraint ck_event_undos_reason check (nullif(btrim(reason), '') is not null),
    constraint ck_event_undos_versions check (expected_state_version >= 1 and state_version = expected_state_version + 1),
    constraint ck_event_undos_sequence check (next_sequence_number > (original_event->>'sequence_number')::bigint),
    constraint ck_event_undos_original check (
        jsonb_typeof(original_event) = 'object'
        and original_event ?& array['event_id', 'session_id', 'sequence_number']
        and original_event->>'event_id' = event_id::text
        and original_event->>'session_id' = session_id::text
        and jsonb_typeof(original_event->'sequence_number') = 'number'
        and (original_event->>'sequence_number')::bigint >= 0
        and jsonb_typeof(original_cashflows) = 'array'
        and jsonb_typeof(original_asset_references) = 'array'
    )
);

create unique index uq_event_undos_session_sequence
    on public.event_undos (session_id, ((original_event->>'sequence_number')::bigint));
create unique index uq_event_undos_original_client_request
    on public.event_undos (session_id, (original_event->>'client_request_id'))
    where original_event->>'client_request_id' is not null;
create index ix_event_undos_session_created
    on public.event_undos (session_id, created_at, undo_id);

create function public.reject_event_undo_audit_mutation() returns trigger
language plpgsql set search_path = pg_catalog, public as $$
begin
    raise exception 'Event undo audit is append-only' using errcode = '23514';
end
$$;

create trigger trg_event_undos_append_only
    before update or delete or truncate on public.event_undos
    for each statement execute function public.reject_event_undo_audit_mutation();

create function public.reject_reused_undone_event() returns trigger
language plpgsql set search_path = pg_catalog, public as $$
begin
    perform pg_advisory_xact_lock(hashtextextended(new.session_id::text, 0));
    if exists (
        select 1 from public.event_undos undo
        where undo.session_id = new.session_id
          and (undo.event_id = new.event_id
            or (undo.original_event->>'sequence_number')::bigint = new.sequence_number
            or (new.client_request_id is not null
                and undo.original_event->>'client_request_id' = new.client_request_id))
    ) then
        raise exception 'Event identity was already used by an undone event' using errcode = '23505';
    end if;
    return new;
end
$$;

-- Jalankan sebelum validator domain agar retry lama dikenali sebagai duplikat, bukan aktivitas baru.
create trigger trg_events_00_undo_identity
    before insert on public.events
    for each row execute function public.reject_reused_undone_event();

create function public.capture_session_undo_state(p_session_id uuid) returns jsonb
language plpgsql set search_path = pg_catalog, public as $$
declare
    v_table text;
    v_rows jsonb;
    v_tables jsonb := '{}'::jsonb;
begin
    perform pg_advisory_xact_lock(hashtextextended(p_session_id::text, 0));
    if not exists (select 1 from public.session_states where session_id = p_session_id) then
        raise exception 'Session state is required for event undo snapshot' using errcode = '23514';
    end if;

    -- ponytail: snapshot penuh satu sesi; ganti ke before-image per baris bila volume event membesar.
    foreach v_table in array array[
        'session_states', 'session_participant_balances', 'session_participant_inventory',
        'session_participant_need_purchases', 'session_participant_financial_goals',
        'session_participant_collection_missions', 'session_participant_action_counters',
        'session_participant_gold_holdings', 'session_participant_loans',
        'session_participant_insurances', 'session_participant_tie_breakers',
        'session_donation_events', 'session_card_positions', 'session_rule_effects',
        'session_narrative_logs', 'session_projection_checkpoints'
    ] loop
        execute format(
            'select coalesce(jsonb_agg(to_jsonb(row) order by to_jsonb(row)::text), ''[]''::jsonb) from public.%I row where session_id = $1',
            v_table) into v_rows using p_session_id;
        v_tables := v_tables || jsonb_build_object(v_table, v_rows);
    end loop;

    return jsonb_build_object('version', 1, 'session_id', p_session_id, 'tables', v_tables);
end
$$;

create function public.restore_session_undo_state(
    p_session_id uuid, p_snapshot jsonb, p_new_state_version bigint) returns void
language plpgsql set search_path = pg_catalog, public as $$
declare
    v_table text;
    v_rows jsonb;
    v_current_version bigint;
    -- Whitelist tetap; nama tabel tidak pernah berasal dari input snapshot.
    v_tables text[] := array[
        'session_states', 'session_participant_inventory', 'session_participant_need_purchases',
        'session_participant_financial_goals', 'session_participant_collection_missions',
        'session_participant_action_counters', 'session_participant_gold_holdings',
        'session_participant_loans', 'session_participant_insurances',
        'session_participant_tie_breakers', 'session_donation_events', 'session_card_positions',
        'session_rule_effects', 'session_narrative_logs', 'session_projection_checkpoints',
        'session_participant_balances'
    ];
begin
    perform pg_advisory_xact_lock(hashtextextended(p_session_id::text, 0));
    select state_version into v_current_version
    from public.session_states where session_id = p_session_id for update;
    if v_current_version is null or p_new_state_version is distinct from v_current_version + 1 then
        raise exception 'Undo must advance the current state version by one' using errcode = '23514';
    end if;
    if p_snapshot is null
        or p_snapshot->'version' is distinct from '1'::jsonb
        or p_snapshot->>'session_id' is distinct from p_session_id::text
        or jsonb_typeof(p_snapshot->'tables') is distinct from 'object' then
        raise exception 'Unsupported or mismatched event undo snapshot' using errcode = '23514';
    end if;
    if (select count(*) from jsonb_object_keys(p_snapshot->'tables')) <> cardinality(v_tables) then
        raise exception 'Event undo snapshot has an unexpected projection schema' using errcode = '23514';
    end if;

    -- Periksa seluruh snapshot sebelum penghapusan pertama agar data sesi lain tidak dapat dipulihkan.
    foreach v_table in array v_tables loop
        v_rows := p_snapshot->'tables'->v_table;
        if jsonb_typeof(v_rows) is distinct from 'array' then
            raise exception 'Missing event undo projection %', v_table using errcode = '23514';
        end if;
        if exists (select 1 from jsonb_array_elements(v_rows) row
            where jsonb_typeof(row) is distinct from 'object'
              or row->>'session_id' is distinct from p_session_id::text) then
            raise exception 'Mismatched session in event undo projection %', v_table using errcode = '23514';
        end if;
    end loop;
    if jsonb_array_length(p_snapshot->'tables'->'session_states') <> 1 then
        raise exception 'Event undo snapshot requires exactly one session state' using errcode = '23514';
    end if;

    foreach v_table in array v_tables loop
        execute format('delete from public.%I where session_id = $1', v_table) using p_session_id;
    end loop;
    foreach v_table in array v_tables loop
        v_rows := p_snapshot->'tables'->v_table;
        if v_table = 'session_states' then
            v_rows := jsonb_set(v_rows, '{0,state_version}', to_jsonb(p_new_state_version));
        end if;
        -- INSERT mempertahankan timestamps snapshot. Misi dipulihkan saat balances kosong sehingga
        -- trigger hadiah misi tidak menggandakan happiness; balances snapshot selalu dipulihkan terakhir.
        execute format('insert into public.%I select * from jsonb_populate_recordset(null::public.%I, $1)',
            v_table, v_table) using v_rows;
    end loop;
end
$$;
