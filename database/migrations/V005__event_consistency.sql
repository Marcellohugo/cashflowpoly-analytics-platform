-- Fungsi file: Menjaga cakupan event, harga risiko, dan tanggal proyeksi tanpa mengubah migrasi terdahulu.
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

  if new.action_type in (
    'SetupModalAwal', 'SetupBahanAwal', 'SetupEmasAwal', 'SetupMisiAwal',
    'SetupPinjamanAwal', 'SetupAsuransiAwal', 'BagikanTieBreaker', 'MulaiSesi',
    'AkhiriSesi', 'UmumkanJuaraDonasi', 'PoinPeringkatDonasi', 'PoinEmas',
    'PoinPeringkatPensiun', 'CatatTransaksi', 'HariMingguLibur', 'BukaHargaEmas', 'AkhirGiliran'
  ) then
    if new.actor_type <> 'SYSTEM' then
      raise exception 'Action % requires SYSTEM actor', new.action_type using errcode = '23514';
    end if;
  elsif new.actor_type = 'SYSTEM' and new.action_type not in ('TujuanFinansial', 'RisikoKehidupan') then
    raise exception 'Action % requires PLAYER actor', new.action_type using errcode = '23514';
  end if;

  if new.actor_type = 'SYSTEM' and new.turn_number <> 0 then
    raise exception 'SYSTEM event must use turn_number = 0' using errcode = '23514';
  end if;

  if new.action_type = 'BahanMasakan' then
    declare
      v_base_price integer;
      v_modifier integer;
      v_expected_price integer;
    begin
      select purchase_price into v_base_price
      from ruleset_ingredients
      where ruleset_version_id = new.ruleset_version_id
        and ingredient_code = new.payload ->> 'card_id';

      select coalesce(sum(effect.value_delta), 0)::integer into v_modifier
      from session_rule_effects effect
      join events source on source.session_id = effect.session_id and source.event_id = effect.source_event_id
      where effect.session_id = new.session_id
        and effect.effect_type = 'INGREDIENT_PRICE_MODIFIER'
        and effect.is_active
        and new.day_index between effect.starts_day and effect.ends_day
        and (
          effect.target_scope = 'ALL_PLAYERS'
          or (effect.target_scope = 'SELF' and source.session_player_id = new.session_player_id)
          or (effect.target_scope = 'OTHER_PLAYERS' and source.session_player_id is distinct from new.session_player_id)
        );

      v_expected_price := greatest(0, v_base_price + v_modifier);
      if v_base_price is null or (new.payload ->> 'amount')::numeric is distinct from v_expected_price::numeric then
        raise exception 'Ingredient purchase must use catalog price with applicable risk effects' using errcode = '23514';
      end if;
    end;
  end if;
  return new;
end
$$;


-- Existing projections retain their source events; correct only event-derived dates.
update session_narrative_logs projection set day = greatest(1, source.day_index)
from events source where source.session_id = projection.session_id
  and source.event_id = projection.source_event_id
  and projection.day is distinct from greatest(1, source.day_index);

update session_participant_need_purchases projection set purchased_at_day = greatest(1, source.day_index)
from events source where source.session_id = projection.session_id
  and source.event_id = projection.source_event_id
  and projection.purchased_at_day is distinct from greatest(1, source.day_index);

update session_participant_financial_goals projection set purchased_at_day = greatest(1, source.day_index)
from events source where source.session_id = projection.session_id
  and source.event_id = projection.last_event_id and source.action_type = 'TujuanFinansial'
  and projection.purchased_at_day is distinct from greatest(1, source.day_index);

update session_donation_events projection set day = greatest(1, source.day_index)
from events source where source.session_id = projection.session_id
  and source.event_id = projection.source_event_id
  and projection.day is distinct from greatest(1, source.day_index);
create
or replace function fn_process_event_side_effects() returns trigger as $$ declare v_sold_need_id uuid;

begin if new.action_type = 'GunakanOpsiDarurat'
and new.payload ->> 'option_type' = 'SELL_NEED' then -- Find the oldest purchased, active (not sold) need card of this player
select
  spnp.session_participant_need_purchase_id into v_sold_need_id
from
  session_participant_need_purchases spnp
  join ruleset_needs rn on rn.ruleset_need_id = spnp.ruleset_need_id
where
  spnp.session_participant_id = new.session_player_id
  and rn.need_code = new.payload ->> 'card_id'
  and not spnp.is_sold
order by
  spnp.purchased_at_day,
  spnp.sort_order
limit
  1;

if v_sold_need_id is not null then
update
  session_participant_need_purchases
set
  is_sold = true,
  sold_at_day_index = new.day_index,
  sold_event_id = new.event_id
where
  session_participant_need_purchase_id = v_sold_need_id;

end if;

end if;

if new.action_type = 'RisikoKehidupan' then declare v_risk_id uuid;

v_effect_type varchar(60);

v_direction varchar(10);

v_amount int;

v_value_delta int;
v_duration_days int;

v_target_scope varchar(40);

begin
select
  ruleset_life_risk_id,
  effect_type,
  direction,
  amount,
  nullif(payload_json ->> 'value_delta', '')::int,
  duration_days,
  target_scope into v_risk_id,
  v_effect_type,
  v_direction,
  v_amount,
  v_value_delta,
  v_duration_days,
  v_target_scope
from
  ruleset_life_risks
where
  ruleset_version_id = new.ruleset_version_id
  and lower(risk_code) = lower(new.payload ->> 'risk_id');

if v_risk_id is not null then if v_effect_type in ('INGREDIENT_PRICE_MODIFIER', 'GOLD_TRADE')
or coalesce(v_duration_days, 1) > 1 then
insert into
  session_rule_effects (
    session_id,
    source_event_id,
    source_risk_id,
    effect_type,
    scope,
    target_scope,
    value_delta,
    starts_day,
    ends_day,
    is_active,
    metadata_json
  )
values
  (
    new.session_id,
    new.event_id,
    v_risk_id,
    v_effect_type,
    'PLAYER',
    coalesce(v_target_scope, 'SELF'),
    coalesce(v_value_delta, case
      v_direction
      when 'IN' then v_amount
      when 'OUT' then - v_amount
      else 0
    end),
    new.day_index,
    new.day_index + coalesce(v_duration_days, 1) - 1,
    true,
    jsonb_build_object(
      'source',
      'fn_process_event_side_effects',
      'risk_id',
      new.payload ->> 'risk_id'
    )
  ) on conflict do nothing;

end if;

end if;

end;

end if;

return new;

end;

$$ language plpgsql;
-- Repair already projected price effects from their locked risk catalog.
update session_rule_effects effect
set value_delta = coalesce(nullif(risk.payload_json ->> 'value_delta', '')::int,
    case risk.direction when 'IN' then risk.amount when 'OUT' then -risk.amount else 0 end)
from ruleset_life_risks risk
where risk.ruleset_life_risk_id = effect.source_risk_id
  and effect.effect_type = 'INGREDIENT_PRICE_MODIFIER';
