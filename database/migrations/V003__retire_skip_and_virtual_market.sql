-- Fungsi file: Menonaktifkan aksi lewati pesanan dan pengelolaan pasar virtual tanpa menghapus histori event lama.
update ruleset_actions
set is_active = false
where action_id in ('LewatiOrder', 'AmbilKartuDariDeck', 'KartuMasukDiscard', 'IsiUlangPasar');

update actions
set is_active = false
where action_id in ('LewatiOrder', 'AmbilKartuDariDeck', 'KartuMasukDiscard', 'IsiUlangPasar');

create or replace function enforce_event_session_scope_v2()
returns trigger
language plpgsql
as $$
declare
  v_session_status varchar(20);
  v_active_ruleset_version_id uuid;
  v_participant_user_id uuid;
  v_player_order_no integer;
begin
  if new.action_type in ('LewatiOrder', 'AmbilKartuDariDeck', 'KartuMasukDiscard', 'IsiUlangPasar') then
    raise exception 'Action % is retired and cannot be ingested', new.action_type using errcode = '23514';
  end if;

  select status, ruleset_version_id
  into v_session_status, v_active_ruleset_version_id
  from sessions
  where session_id = new.session_id;

  if v_active_ruleset_version_id is null then
    raise exception 'Session % does not exist', new.session_id using errcode = '23503';
  end if;

  if new.ruleset_version_id is distinct from v_active_ruleset_version_id then
    raise exception 'Event ruleset version does not match session ruleset version' using errcode = '23514';
  end if;

  if not exists (
    select 1
    from ruleset_actions ra
    join actions a on a.action_id = ra.action_id and a.is_active
    where ra.ruleset_version_id = new.ruleset_version_id
      and ra.ruleset_action_id = new.ruleset_action_id
      and ra.action_id = new.action_type
      and ra.is_active
  ) then
    raise exception 'Action % is not active in the session ruleset', new.action_type using errcode = '23514';
  end if;

  if new.actor_type = 'PLAYER' then
    if v_session_status = 'CREATED' then
      raise exception 'PLAYER events require a session that has started' using errcode = '23514';
    end if;

    if new.session_player_id is null or new.user_id is null then
      raise exception 'PLAYER event requires session_player_id and user_id' using errcode = '23514';
    end if;

    select user_id, player_order_no
    into v_participant_user_id, v_player_order_no
    from session_participants
    where session_id = new.session_id
      and session_participant_id = new.session_player_id;

    if v_participant_user_id is null or new.user_id is distinct from v_participant_user_id then
      raise exception 'Event player does not match the session participant' using errcode = '23514';
    end if;

    if new.turn_number <> v_player_order_no then
      raise exception 'PLAYER event turn_number must match player_order_no' using errcode = '23514';
    end if;
  elsif new.actor_type = 'SYSTEM' then
    if new.action_slot <> 0 then
      raise exception 'SYSTEM event cannot consume an action slot' using errcode = '23514';
    end if;

    if new.session_player_id is not null then
      select user_id
      into v_participant_user_id
      from session_participants
      where session_id = new.session_id
        and session_participant_id = new.session_player_id;

      if v_participant_user_id is null or new.user_id is distinct from v_participant_user_id then
        raise exception 'SYSTEM event player does not match the session participant' using errcode = '23514';
      end if;
    elsif new.user_id is not null then
      raise exception 'SYSTEM event user_id requires session_player_id' using errcode = '23514';
    end if;
  else
    raise exception 'Unsupported actor type %', new.actor_type using errcode = '23514';
  end if;

  return new;
end
$$;

drop trigger if exists trg_events_session_scope on events;

create trigger trg_events_session_scope
before insert or update of
  session_id,
  session_player_id,
  user_id,
  ruleset_action_id,
  action_type,
  ruleset_version_id,
  actor_type,
  turn_number,
  action_slot
on events
for each row execute function enforce_event_session_scope_v2();
