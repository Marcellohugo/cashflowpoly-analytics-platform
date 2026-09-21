-- Fungsi file: Menerapkan validasi risiko dan pembaruan harga emas pada trigger aktif tanpa mengubah migrasi yang sudah terpasang.
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
  select pg_get_functiondef('enforce_event_session_scope_v2()'::regprocedure) into definition;
  -- API validation precedes the session lock. Repeat pending/duplicate checks inside the SQL write lock.
  definition := pg_temp.risk_replace(definition,
    '  return new;',
    $block$  perform pg_advisory_xact_lock(hashtextextended(new.session_id::text, 0));
if new.action_type in ('AkhirGiliran', 'AkhiriSesi') and has_pending_life_risk(new.session_id, null) then
  raise exception 'All players must resolve their life risks before ending the turn/session' using errcode = '23514';
end if;
if new.actor_type = 'PLAYER' and has_pending_life_risk(new.session_id, new.user_id)
  and not (new.action_type in ('BayarRisiko', 'GunakanOpsiDarurat', 'Asuransi', 'PinjamanSyariah') and new.payload ? 'risk_event_id') then
  raise exception 'Player must resolve pending life risks first' using errcode = '23514';
end if;
if new.action_type in ('BayarRisiko', 'GunakanOpsiDarurat', 'Asuransi', 'PinjamanSyariah') and new.payload ? 'risk_event_id' then
  if not exists (
    select 1 from events source
    join ruleset_life_risks risk on risk.ruleset_version_id = source.ruleset_version_id
      and lower(risk.risk_code) = lower(source.payload ->> 'risk_id')
    where source.session_id = new.session_id and source.event_id = fn_safe_cast_to_uuid(new.payload ->> 'risk_event_id')
      and source.action_type = 'RisikoKehidupan' and risk.direction = 'OUT' and risk.amount > 0
      and ((risk.effect_type = 'COIN_EFFECT' and risk.target_scope = 'SELF' and source.user_id = new.user_id)
        or (risk.effect_type = 'ALL_PLAYERS_COIN_EFFECT' and risk.target_scope = 'ALL_PLAYERS'))
      and (new.action_type <> 'BayarRisiko' or not (new.payload ? 'amount') or (new.payload ->> 'amount')::numeric = risk.amount)
  ) then
    raise exception 'Risk response must match an applicable cost-based life risk and its amount' using errcode = '23514';
  end if;
  if exists (select 1 from event_cashflow_projections paid
    where paid.session_id = new.session_id and paid.user_id = new.user_id
      and paid.category = 'RISK_LIFE' and paid.direction = 'OUT'
      and (paid.event_id = fn_safe_cast_to_uuid(new.payload ->> 'risk_event_id')
        or paid.reference = fn_safe_cast_to_uuid(new.payload ->> 'risk_event_id')::text)) then
  raise exception 'Risk event is already resolved for this player' using errcode = '23514';
  end if;
  if new.action_type = 'Asuransi' and not exists (
    select 1 from session_participant_insurances insurance
    where insurance.session_participant_id = new.session_player_id
      and insurance.status = 'ACTIVE' and insurance.remaining_uses > 0
  ) then
    raise exception 'Insurance claim requires active policy with remaining uses' using errcode = '23514';
  end if;
end if;

  return new;$block$);
  execute definition;

end;
$migration$;
-- Fungsi file: Memperbarui harga emas setelah setiap kartu Investasi Emas dan menolak perdagangan pada harga lama.
do $migration$
declare
  v_definition text := replace(pg_get_functiondef('enforce_event_session_scope_v2()'::regprocedure), chr(13), '');
  v_guard text := $guard$
  if new.action_type in ('BukaHargaEmas', 'InvestasiEmas', 'JualEmas')
    or (new.action_type = 'GunakanOpsiDarurat' and upper(new.payload ->> 'option_type') = 'SELL_GOLD') then
    declare
      v_risk_sequence bigint;
      v_price_sequence bigint;
      v_price integer;
    begin
      perform pg_advisory_xact_lock(hashtextextended(new.session_id::text, 0));
      select max(source.sequence_number) into v_risk_sequence
      from session_rule_effects effect
      join events source on source.session_id = effect.session_id and source.event_id = effect.source_event_id
      where effect.session_id = new.session_id and effect.effect_type = 'GOLD_TRADE'
        and effect.is_active and new.day_index between effect.starts_day and effect.ends_day;

      select price_event.sequence_number, (price_event.payload ->> 'gold_price')::int
      into v_price_sequence, v_price
      from events price_event
      where price_event.session_id = new.session_id and price_event.day_index = new.day_index
        and price_event.action_type = 'BukaHargaEmas'
      order by price_event.sequence_number desc limit 1;

      if new.action_type = 'BukaHargaEmas' then
        if not exists (
          select 1 from ruleset_gold_prices price
          where price.ruleset_version_id = new.ruleset_version_id
            and price.unit_price = (new.payload ->> 'gold_price')::numeric and price.is_active
        ) then
          raise exception 'Gold price must match an active ruleset price' using errcode = '23514';
        end if;
        if new.weekday <> 'SAT' and v_risk_sequence is null then
          raise exception 'Gold price can only be opened on SAT or during an active gold risk' using errcode = '23514';
        end if;
        if v_price_sequence is not null and coalesce(v_risk_sequence, 0) <= v_price_sequence then
          raise exception 'Gold price is already opened on day %', new.day_index using errcode = '23514';
        end if;
      else
        if v_price is null or coalesce(v_risk_sequence, 0) >= v_price_sequence then
          raise exception 'Gold trade requires BukaHargaEmas after the latest gold risk on day %', new.day_index using errcode = '23514';
        end if;
        if (new.payload ->> 'unit_price')::numeric is distinct from v_price::numeric then
          raise exception 'Gold unit_price must match active price %', v_price using errcode = '23514';
        end if;
      end if;
    end;
  end if;
  return new;
$guard$;
begin
  if position('  return new;' in v_definition) = 0 then
    raise exception 'Cannot locate return in active event validator';
  end if;
  execute replace(v_definition, '  return new;', replace(v_guard, chr(13), ''));
end
$migration$;
