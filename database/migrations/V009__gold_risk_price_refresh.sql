-- Fungsi file: Memperbarui harga emas setelah setiap kartu Investasi Emas dan menolak perdagangan pada harga lama.
do $migration$
declare
  v_definition text := replace(pg_get_functiondef('enforce_event_session_scope()'::regprocedure), chr(13), '');
  v_old text;
  v_new text;
begin
  v_old := $old$if exists (
  select 1
  from events price_event
  where price_event.session_id = new.session_id
    and price_event.day_index = new.day_index
    and price_event.action_type = 'BukaHargaEmas'
) then raise exception 'Gold price is already opened on day %',$old$;
  v_new := $new$if exists (
  select 1
  from events price_event
  where price_event.session_id = new.session_id
    and price_event.day_index = new.day_index
    and price_event.action_type = 'BukaHargaEmas'
)
and not exists (
  select 1
  from session_rule_effects effect
  join events risk_event on risk_event.session_id = effect.session_id
    and risk_event.event_id = effect.source_event_id
  where effect.session_id = new.session_id
    and effect.effect_type = 'GOLD_TRADE'
    and effect.is_active
    and new.day_index between effect.starts_day and effect.ends_day
    and risk_event.sequence_number > (
      select max(price_event.sequence_number)
      from events price_event
      where price_event.session_id = new.session_id
        and price_event.day_index = new.day_index
        and price_event.action_type = 'BukaHargaEmas'
    )
) then raise exception 'Gold price is already opened on day %',$new$;
  v_old := replace(v_old, chr(13), '');
  v_new := replace(v_new, chr(13), '');
  if position(v_old in v_definition) = 0 then
    raise exception 'Cannot locate gold price opening guard in enforce_event_session_scope';
  end if;
  v_definition := replace(v_definition, v_old, v_new);

  v_old := $old$  and price_evt.day_index = new.day_index
order by
  price_evt.day_index desc,
  price_evt.sequence_number desc
limit
  1;

if v_active_gold_price is null then raise exception 'Gold trade requires BukaHargaEmas on day %, sequence %',$old$;
  v_new := $new$  and price_evt.day_index = new.day_index
  and not exists (
    select 1
    from session_rule_effects effect
    join events risk_event on risk_event.session_id = effect.session_id
      and risk_event.event_id = effect.source_event_id
    where effect.session_id = new.session_id
      and effect.effect_type = 'GOLD_TRADE'
      and effect.is_active
      and new.day_index between effect.starts_day and effect.ends_day
      and risk_event.sequence_number >= price_evt.sequence_number
  )
order by
  price_evt.day_index desc,
  price_evt.sequence_number desc
limit
  1;

if v_active_gold_price is null then raise exception 'Gold trade requires BukaHargaEmas on day %, sequence %',$new$;
  v_old := replace(v_old, chr(13), '');
  v_new := replace(v_new, chr(13), '');
  if position(v_old in v_definition) = 0 then
    raise exception 'Cannot locate active gold price guard in enforce_event_session_scope';
  end if;
  v_definition := replace(v_definition, v_old, v_new);
  execute v_definition;
end
$migration$;
