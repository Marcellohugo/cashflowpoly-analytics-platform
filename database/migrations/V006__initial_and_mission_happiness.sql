-- Fungsi file: Menambahkan poin awal dan hadiah misi ke state serta skor final tanpa menghitung ulang hadiah.
-- Apply mission rewards exactly once for both API projection and SQL replay.
create or replace function apply_collection_mission_reward()
returns trigger language plpgsql as $$
declare
  v_reward integer;
begin
  if tg_op = 'UPDATE' and old.is_completed then
    new.is_completed := true;
    new.is_failed := false;
  end if;
  if tg_op = 'UPDATE' and old.reward_applied then
    new.reward_applied := true;
    return new;
  end if;
  if new.is_completed then
    select greatest(0, success_points) into v_reward
    from ruleset_collection_missions
    where ruleset_collection_mission_id = new.ruleset_collection_mission_id;
    if v_reward > 0 then
      update session_participant_balances
      set happiness = happiness + v_reward,
          last_event_id = new.last_event_id,
          updated_at = now()
      where session_participant_id = new.session_participant_id;
      new.reward_applied := true;
    end if;
  end if;
  return new;
end
$$;

create trigger trg_collection_mission_reward
before insert or update of is_completed on session_participant_collection_missions
for each row execute function apply_collection_mission_reward();

-- Existing completed missions have a locked catalog reward; no display-text inference.
update session_participant_collection_missions
set is_completed = is_completed
where is_completed and not reward_applied;

-- Fill missing score components on old finalizations, preserving every other component.
create temporary table scoring_corrections on commit drop as
with missing as (
  select score.session_final_score_id, score.session_id, score.session_participant_id,
         score.source_event_id, 'INITIAL_HAPPINESS'::varchar(80) as component_code,
         settings.starting_happiness as points
  from session_final_scores score
  join sessions session using (session_id)
  join ruleset_game_settings settings on settings.ruleset_version_id = session.ruleset_version_id
  union all
  select score.session_final_score_id, score.session_id, score.session_participant_id,
         score.source_event_id, 'MISSION_REWARD'::varchar(80),
         coalesce(sum(greatest(0, catalog.success_points)) filter (where mission.is_completed), 0)::int
  from session_final_scores score
  left join session_participant_collection_missions mission using (session_id, session_participant_id)
  left join ruleset_collection_missions catalog using (ruleset_collection_mission_id)
  group by score.session_final_score_id
), inserted as (
  insert into session_final_score_components (
    session_id, session_participant_id, session_final_score_id, component_code, points, source_event_id)
  select session_id, session_participant_id, session_final_score_id, component_code, points, source_event_id
  from missing
  on conflict (session_final_score_id, component_code) do nothing
  returning session_final_score_id, session_id, points
)
select session_final_score_id, session_id, sum(points)::int as delta
from inserted group by session_final_score_id, session_id;

update session_final_scores score
set total_points = total_points + correction.delta
from scoring_corrections correction
where correction.session_final_score_id = score.session_final_score_id;

-- Move ranks to a disjoint positive range before applying a changed ordering.
create temporary table corrected_ranks on commit drop as
select score.session_final_score_id, score.session_id,
       row_number() over (partition by score.session_id
         order by score.total_points desc, coalesce(score.tie_breaker_number, 0) desc, player.user_id)::int as new_rank,
       max(score.rank_no) over (partition by score.session_id) as rank_offset
from session_final_scores score
join session_participants player using (session_id, session_participant_id)
where score.session_id in (select session_id from scoring_corrections where delta <> 0);

update session_final_scores score set rank_no = rank_no + ranking.rank_offset
from corrected_ranks ranking where ranking.session_final_score_id = score.session_final_score_id;
update session_final_scores score set rank_no = ranking.new_rank
from corrected_ranks ranking where ranking.session_final_score_id = score.session_final_score_id;
