-- Fungsi file: Menjadikan tabungan dana bersama pemain dan mengunci stok kartu tujuan hanya saat pembelian berhasil.
do $migration$
declare
  definition text;
  baseline text := replace(pg_get_functiondef('enforce_event_session_scope()'::regprocedure), chr(13), '');
  start_at integer;
  stop_at integer;
  goal_guard text;
begin
  -- Reuse catalog, balance, duplicate-ownership, and physical-supply checks in the active validator.
  start_at := position('if new.action_type = ''TujuanFinansial'' then declare v_goal_price int;' in baseline);
  stop_at := position(E'if new.action_type = ''JumatBerkah''\nand exists (' in baseline);
  if start_at = 0 or stop_at <= start_at then
    raise exception 'Cannot locate financial goal purchase guard';
  end if;
  goal_guard := substring(baseline from start_at for stop_at - start_at);
  definition := replace(pg_get_functiondef('enforce_event_session_scope_v2()'::regprocedure), chr(13), '');
  if position('  return new;' in definition) = 0 then
    raise exception 'Cannot locate active validator return';
  end if;
  execute replace(definition, '  return new;', goal_guard || E'\n  return new;');

  -- Deposits only increase the shared saving balance; they never assign a card to a player.
  definition := replace(pg_get_functiondef('project_session_event(uuid)'::regprocedure), chr(13), '');
  start_at := position(E'if nullif(v_event.payload ->> ''goal_id'', '''') is not null then\ninsert into\n  session_participant_financial_goals (' in definition);
  stop_at := position('if v_event.action_type = ''TarikTabungan'' then' in definition);
  if start_at = 0 or stop_at <= start_at then
    raise exception 'Cannot locate earmarked saving projection';
  end if;
  execute overlay(definition placing E'end if;\n\n' from start_at for stop_at - start_at);
end
$migration$;

-- These are derived progress rows, not purchases; savings themselves remain in participant balances and events.
delete from session_participant_financial_goals where status = 'ONGOING';
