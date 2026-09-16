-- Fungsi file: Memisahkan akses field trigger per tabel agar narasi tetap divalidasi tanpa membaca turn_number yang tidak tersedia.
create or replace function enforce_action_slot_within_ruleset_limit()
returns trigger language plpgsql as $$
declare
  v_ruleset_version_id uuid;
  v_actions_per_turn int;
  v_action_type varchar(50);
  v_payload jsonb;
  v_ref_event_id uuid;
begin
  if tg_table_name = 'events' then
    if new.actor_type = 'SYSTEM' then return new; end if;
  end if;

  -- PostgreSQL resolves record fields before Boolean short-circuit evaluation.
  if tg_table_name = 'session_states' then
    if coalesce(new.turn_number, 0) = 0 then return new; end if;
    select ruleset_version_id into v_ruleset_version_id
    from sessions where session_id = new.session_id;
  else
    v_ruleset_version_id := new.ruleset_version_id;
  end if;

  select actions_per_turn into v_actions_per_turn
  from ruleset_game_settings where ruleset_version_id = v_ruleset_version_id;
  if v_actions_per_turn is null then
    raise exception 'Cannot resolve actions_per_turn for action slot validation' using errcode = '23514';
  end if;

  if tg_table_name = 'events' then
    v_action_type := new.action_type;
    v_payload := new.payload::jsonb;
  elsif tg_table_name = 'session_states' then
    v_ref_event_id := new.last_event_id;
  elsif tg_table_name = 'session_narrative_logs' then
    v_ref_event_id := new.source_event_id;
  end if;
  if v_ref_event_id is not null then
    select action_type, payload::jsonb into v_action_type, v_payload
    from events where session_id = new.session_id and event_id = v_ref_event_id;
  end if;

  if v_action_type is not null and (
    v_action_type in ('JumatBerkah', 'RisikoKehidupan', 'BayarRisiko', 'GunakanOpsiDarurat',
      'InvestasiEmas', 'JualEmas', 'LewatiTransaksiEmas', 'HariMingguLibur')
    or (v_action_type in ('Asuransi', 'PinjamanSyariah') and v_payload ? 'risk_event_id')
  ) then return new; end if;

  if new.action_slot < 1 then
    raise exception 'PLAYER action_slot must be >= 1' using errcode = '23514';
  end if;
  if new.action_slot > v_actions_per_turn then
    raise exception 'action_slot % exceeds actions_per_turn %', new.action_slot, v_actions_per_turn using errcode = '23514';
  end if;
  if tg_table_name = 'session_states' then
    if new.current_action_slot > v_actions_per_turn then
      raise exception 'current_action_slot % exceeds actions_per_turn %', new.current_action_slot, v_actions_per_turn using errcode = '23514';
    end if;
  end if;
  return new;
end;
$$;
