-- Fungsi file: Menyelesaikan biaya kartu massal per pemain dengan pilihan bayar/asuransi dan proyeksi ulang yang konsisten.
create or replace function has_pending_life_risk(p_session_id uuid, p_user_id uuid default null)
returns boolean language sql stable as $$
  select exists (
    select 1
    from events risk_event
    join ruleset_life_risks risk on risk.ruleset_version_id = risk_event.ruleset_version_id
      and lower(risk.risk_code) = lower(risk_event.payload ->> 'risk_id')
    join session_participants player on player.session_id = risk_event.session_id
      and ((risk.effect_type = 'COIN_EFFECT' and risk.target_scope = 'SELF' and player.user_id = risk_event.user_id)
        or (risk.effect_type = 'ALL_PLAYERS_COIN_EFFECT' and risk.target_scope = 'ALL_PLAYERS'))
    where risk_event.session_id = p_session_id and risk_event.action_type = 'RisikoKehidupan'
      and risk.direction = 'OUT' and risk.amount > 0
      and (p_user_id is null or player.user_id = p_user_id)
      and not exists (
        select 1 from event_cashflow_projections paid
        where paid.session_id = risk_event.session_id and paid.user_id = player.user_id
          and paid.category = 'RISK_LIFE' and paid.direction = 'OUT'
          and (paid.event_id = risk_event.event_id or paid.reference = risk_event.event_id::text)
      )
  );
$$;

-- Preserve the rest of the canonical validators/projector, failing startup if an expected block moved.
create or replace function pg_temp.risk_replace(source text, old_block text, new_block text)
returns text language plpgsql as $$
begin
  source := replace(source, chr(13), '');
  old_block := replace(old_block, chr(13), '');
  new_block := replace(new_block, chr(13), '');
  if position(old_block in source) = 0 then
    raise exception 'Life risk migration could not locate expected function block: %', left(old_block, 100);
  end if;
  return replace(source, old_block, new_block);
end;
$$;

do $migration$
declare definition text;
begin
  select pg_get_functiondef('enforce_event_session_scope()'::regprocedure) into definition;
  definition := pg_temp.risk_replace(definition,
    'and risk_evt.user_id = new.user_id',
    $block$and ((risk_catalog.effect_type = 'COIN_EFFECT' and risk_catalog.target_scope = 'SELF' and risk_evt.user_id = new.user_id)
    or (risk_catalog.effect_type = 'ALL_PLAYERS_COIN_EFFECT' and risk_catalog.target_scope = 'ALL_PLAYERS'))$block$);
  definition := pg_temp.risk_replace(definition,
    $block$or v_risk_effect_type <> 'COIN_EFFECT'
or v_risk_target_scope <> 'SELF'$block$,
    $block$or not ((v_risk_effect_type = 'COIN_EFFECT' and v_risk_target_scope = 'SELF')
  or (v_risk_effect_type = 'ALL_PLAYERS_COIN_EFFECT' and v_risk_target_scope = 'ALL_PLAYERS'))$block$);
  definition := pg_temp.risk_replace(definition,
    'or v_risk_amount <= 0 then', 'or coalesce(v_risk_amount, 0) <= 0 then');
  definition := pg_temp.risk_replace(definition,
    $block$and projection.category = 'RISK_LIFE'$block$,
    $block$and projection.user_id = new.user_id
    and projection.category = 'RISK_LIFE'$block$);
  definition := pg_temp.risk_replace(definition,
    $block$if v_actor_type = 'SYSTEM' then raise exception 'Insurance cannot be used to mitigate global (SYSTEM) risks' using errcode = '23514';

end if;$block$, '');
  -- API validation precedes the session lock. Repeat pending/duplicate checks inside the SQL write lock.
  definition := pg_temp.risk_replace(definition,
    $block$if new.action_type = 'BayarRisiko' then declare v_risk_event_id uuid;$block$,
    $block$if new.action_type in ('AkhirGiliran', 'AkhiriSesi') and has_pending_life_risk(new.session_id, null) then
  raise exception 'All players must resolve their life risks before ending the turn/session' using errcode = '23514';
end if;
if new.actor_type = 'PLAYER' and has_pending_life_risk(new.session_id, new.user_id)
  and not (new.action_type in ('BayarRisiko', 'GunakanOpsiDarurat', 'Asuransi', 'PinjamanSyariah') and new.payload ? 'risk_event_id') then
  raise exception 'Player must resolve pending life risks first' using errcode = '23514';
end if;
if new.action_type in ('BayarRisiko', 'GunakanOpsiDarurat', 'Asuransi', 'PinjamanSyariah') and new.payload ? 'risk_event_id'
  and exists (select 1 from event_cashflow_projections paid
    where paid.session_id = new.session_id and paid.user_id = new.user_id
      and paid.category = 'RISK_LIFE' and paid.direction = 'OUT'
      and (paid.event_id = fn_safe_cast_to_uuid(new.payload ->> 'risk_event_id')
        or paid.reference = fn_safe_cast_to_uuid(new.payload ->> 'risk_event_id')::text)) then
  raise exception 'Risk event is already resolved for this player' using errcode = '23514';
end if;

if new.action_type = 'BayarRisiko' then declare v_risk_event_id uuid;$block$);
  execute definition;

  select pg_get_functiondef('project_session_event(uuid)'::regprocedure) into definition;
  definition := pg_temp.risk_replace(definition,
    'and risk_evt.user_id = v_event.user_id',
    $block$and ((risk.effect_type = 'COIN_EFFECT' and risk.target_scope = 'SELF' and risk_evt.user_id = v_event.user_id)
      or (risk.effect_type = 'ALL_PLAYERS_COIN_EFFECT' and risk.target_scope = 'ALL_PLAYERS'))$block$);
  -- Legacy stored/seeded draws already debited everyone; new API draws explicitly record the per-player choice.
  definition := pg_temp.risk_replace(definition,
    $block$and risk.effect_type = 'ALL_PLAYERS_COIN_EFFECT'
  union$block$,
    $block$and risk.effect_type = 'ALL_PLAYERS_COIN_EFFECT'
    and not (risk.direction = 'OUT' and coalesce(v_event.payload ->> 'resolution_mode', '') = 'PER_PLAYER')
  union$block$);
  definition := pg_temp.risk_replace(definition,
    $block$v_event.action_type in ('Asuransi', 'GunakanOpsiDarurat')$block$,
    $block$v_event.action_type in ('Asuransi', 'GunakanOpsiDarurat', 'PinjamanSyariah')$block$);
  definition := pg_temp.risk_replace(definition,
    $block$and risk.effect_type = 'COIN_EFFECT'
    and risk.direction = 'OUT'$block$,
    $block$and risk.effect_type in ('COIN_EFFECT', 'ALL_PLAYERS_COIN_EFFECT')
    and risk.direction = 'OUT'$block$);
  definition := pg_temp.risk_replace(definition,
    $block$when upper(v_event.payload ->> 'option_type') = 'TAKE_SHARIA_LOAN'
          then$block$,
    $block$when v_event.action_type = 'PinjamanSyariah' or upper(v_event.payload ->> 'option_type') = 'TAKE_SHARIA_LOAN'
          then$block$);
  definition := pg_temp.risk_replace(definition,
    $block$and resolved.category = 'RISK_LIFE'$block$,
    $block$and resolved.user_id = v_event.user_id
        and resolved.category = 'RISK_LIFE'$block$);
  execute definition;
end;
$migration$;

-- Repair live balance snapshots from the existing ledger, without reopening settled historical cards.
with corrected as (
  select player.session_participant_id, settings.starting_cash + coalesce(sum(
    case ledger.direction when 'IN' then ledger.amount else -ledger.amount end), 0)::int as coins
  from session_participants player
  join sessions session on session.session_id = player.session_id and session.status = 'STARTED'
  join ruleset_game_settings settings on settings.ruleset_version_id = session.ruleset_version_id
  left join event_cashflow_projections ledger on ledger.session_id = player.session_id and ledger.user_id = player.user_id
  where exists (select 1 from events draw join ruleset_life_risks risk
    on risk.ruleset_version_id = draw.ruleset_version_id and lower(risk.risk_code) = lower(draw.payload ->> 'risk_id')
    where draw.session_id = player.session_id and draw.action_type = 'RisikoKehidupan'
      and risk.effect_type in ('ALL_PLAYERS_COIN_EFFECT', 'PLAYER_TO_PLAYER_TRANSFER'))
  group by player.session_participant_id, settings.starting_cash
)
update session_participant_balances balance set coins = corrected.coins, updated_at = now()
from corrected where corrected.session_participant_id = balance.session_participant_id
  and balance.coins is distinct from corrected.coins;
