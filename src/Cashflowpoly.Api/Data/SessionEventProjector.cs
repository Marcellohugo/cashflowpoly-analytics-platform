using System.Text.Json;
using Cashflowpoly.Api.Contracts;
using Cashflowpoly.Api.Domain;
using Dapper;
using Npgsql;

namespace Cashflowpoly.Api.Data;

public sealed class SessionEventProjector
{
    private readonly IEventPayloadReader _payloadReader;

    public SessionEventProjector(IEventPayloadReader payloadReader)
    {
        _payloadReader = payloadReader;
    }

    public async Task ProjectAsync(
        EventRequest request,
        EventDb storedEvent,
        IReadOnlyCollection<CashflowProjectionDb> cashflowProjections,
        NpgsqlConnection conn,
        NpgsqlTransaction tx,
        CancellationToken ct)
    {
        await UpdateSessionStateAsync(request, storedEvent.SessionPlayerId, conn, tx, ct);

        if (!storedEvent.SessionPlayerId.HasValue)
        {
            if (GameActionCatalog.Is(request.ActionType, request.Payload, GameActionCatalog.SessionEnded))
            {
                await MarkIncompleteMissionsFailedAsync(request.SessionId, storedEvent.EventId, conn, tx, ct);
            }

            await UpdateProjectionCheckpointAsync(request.SessionId, storedEvent.SequenceNumber, storedEvent.EventId, conn, tx, ct);
            return;
        }

        var participantId = storedEvent.SessionPlayerId.Value;
        await EnsureParticipantBalanceAsync(request, participantId, conn, tx, ct);
        await ApplyCashflowAsync(participantId, storedEvent.EventId, cashflowProjections, conn, tx, ct);
        await IncrementActionCounterAsync(
            request.SessionId,
            participantId,
            request.RulesetVersionId,
            storedEvent.RulesetActionId,
            storedEvent.EventId,
            conn,
            tx,
            ct);
        await ProjectNarrativeTriggersAsync(
            request,
            participantId,
            storedEvent.RulesetActionId,
            storedEvent.EventId,
            conn,
            tx,
            ct);

        var canonicalAction = GameActionCatalog.ResolveGameActionId(request.ActionType, request.Payload) ?? request.ActionType.Trim();
        switch (canonicalAction)
        {
            case GameActionCatalog.BahanMasakan:
                await ProjectIngredientPurchaseAsync(request, participantId, conn, tx, ct);
                break;
            case GameActionCatalog.IngredientDiscarded:
                await ProjectIngredientDiscardAsync(request, participantId, conn, tx, ct);
                break;
            case GameActionCatalog.JualMasakan:
                await ProjectOrderClaimAsync(request, participantId, conn, tx, ct);
                break;
            case GameActionCatalog.Kebutuhan:
                await ProjectNeedPurchaseAsync(request, participantId, conn, tx, ct);
                break;
            case GameActionCatalog.MissionAssigned:
                await ProjectMissionAssignmentAsync(request, participantId, conn, tx, ct);
                break;
            case GameActionCatalog.JumatBerkah:
                await ProjectDonationAsync(request, participantId, conn, tx, ct);
                break;
            case GameActionCatalog.DonationRankAwarded:
                await ProjectDonationRankingAsync(request, participantId, conn, tx, ct);
                break;
            case GameActionCatalog.InvestasiEmas:
            case GameActionCatalog.JualEmas:
                await ProjectGoldTradeAsync(request, participantId, conn, tx, ct);
                break;
            case GameActionCatalog.GoldInitialGranted:
                await ProjectInitialGoldAsync(request, participantId, conn, tx, ct);
                break;
            case GameActionCatalog.GoldPointsAwarded:
                await ProjectAwardedPointsAsync(request, participantId, conn, tx, ct);
                break;
            case GameActionCatalog.Menabung:
                await ProjectSavingDepositAsync(request, participantId, conn, tx, ct);
                break;
            case GameActionCatalog.SavingDepositWithdrawn:
                await ProjectSavingWithdrawalAsync(request, participantId, conn, tx, ct);
                break;
            case GameActionCatalog.TujuanFinansial:
                await ProjectSavingGoalAchievedAsync(request, participantId, conn, tx, ct);
                break;
            case GameActionCatalog.PinjamanSyariah:
                await ProjectLoanTakenAsync(request, participantId, conn, tx, ct);
                break;
            case GameActionCatalog.BayarPinjaman:
                await ProjectLoanRepaidAsync(request, participantId, conn, tx, ct);
                break;
            case GameActionCatalog.Asuransi:
                if (_payloadReader.TryReadInsuranceUsed(request.Payload, out _))
                {
                    await ProjectInsuranceUsedAsync(request, participantId, conn, tx, ct);
                }
                else
                {
                    await ProjectInsurancePurchasedAsync(request, participantId, conn, tx, ct);
                }
                break;
            case GameActionCatalog.TieBreakerAssigned:
                await ProjectTieBreakerAsync(request, participantId, storedEvent.EventId, conn, tx, ct);
                break;
            case GameActionCatalog.PensionRankAwarded:
                await ProjectPensionRankingAsync(request, participantId, conn, tx, ct);
                break;
            case GameActionCatalog.SessionEnded:
                await MarkIncompleteMissionsFailedAsync(request.SessionId, storedEvent.EventId, conn, tx, ct);
                break;
        }

        await UpdateProjectionCheckpointAsync(request.SessionId, storedEvent.SequenceNumber, storedEvent.EventId, conn, tx, ct);
    }

    private async Task UpdateSessionStateAsync(
        EventRequest request,
        Guid? participantId,
        NpgsqlConnection conn,
        NpgsqlTransaction tx,
        CancellationToken ct)
    {
        int? actionSlotsLeft = null;
        int? currentActionSlot = null;
        if (_payloadReader.TryReadActionUsed(request.Payload, out var used, out var remaining))
        {
            actionSlotsLeft = remaining;
            currentActionSlot = Math.Max(1, used + 1);
        }

        var day = Math.Max(1, request.DayIndex + 1);
        var isGameOver = GameActionCatalog.Is(request.ActionType, request.Payload, GameActionCatalog.SessionEnded);
        var phase = ResolvePhase(request.Weekday, isGameOver);

        const string sql = """
            insert into session_states (
                session_id,
                day,
                weekday,
                turn_number,
                action_slot,
                current_session_player_id,
                current_action_slot,
                action_slots_left,
                finish_day,
                phase,
                is_game_over,
                state_version,
                last_event_id,
                ui_state_json,
                created_at,
                updated_at
            )
            select
                @sessionId,
                @day,
                @weekday,
                @turnNumber,
                @actionSlot,
                @participantId,
                coalesce(@currentActionSlot, 1),
                coalesce(@actionSlotsLeft, rgs.actions_per_turn),
                rgs.finish_day,
                @phase,
                @isGameOver,
                1,
                @eventId,
                '{}'::jsonb,
                now(),
                now()
            from ruleset_game_settings rgs
            where rgs.ruleset_version_id = @rulesetVersionId
            on conflict (session_id) do update
            set day = excluded.day,
                weekday = excluded.weekday,
                turn_number = excluded.turn_number,
                action_slot = excluded.action_slot,
                current_session_player_id = coalesce(excluded.current_session_player_id, session_states.current_session_player_id),
                current_action_slot = coalesce(@currentActionSlot, session_states.current_action_slot),
                action_slots_left = coalesce(@actionSlotsLeft, session_states.action_slots_left),
                finish_day = excluded.finish_day,
                phase = excluded.phase,
                is_game_over = excluded.is_game_over,
                state_version = session_states.state_version + 1,
                last_event_id = excluded.last_event_id,
                updated_at = now()
            """;

        await conn.ExecuteAsync(new CommandDefinition(
            sql,
            new
            {
                sessionId = request.SessionId,
                day,
                weekday = request.Weekday.ToUpperInvariant(),
                turnNumber = request.ActorType.Equals("SYSTEM", StringComparison.OrdinalIgnoreCase) ? 0 : request.TurnNumber,
                actionSlot = Math.Max(1, request.ActionSlot),
                participantId,
                currentActionSlot,
                actionSlotsLeft,
                rulesetVersionId = request.RulesetVersionId,
                phase,
                isGameOver,
                eventId = request.EventId
            },
            tx,
            cancellationToken: ct));
    }

    private static async Task EnsureParticipantBalanceAsync(
        EventRequest request,
        Guid participantId,
        NpgsqlConnection conn,
        NpgsqlTransaction tx,
        CancellationToken ct)
    {
        const string sql = """
            insert into session_participant_balances (
                session_id,
                session_participant_id,
                coins,
                happiness,
                saving,
                total_donasi,
                last_event_id,
                created_at,
                updated_at
            )
            select
                @sessionId,
                @participantId,
                rgs.starting_cash,
                rgs.starting_happiness,
                rgs.starting_saving,
                0,
                @eventId,
                now(),
                now()
            from ruleset_game_settings rgs
            where rgs.ruleset_version_id = @rulesetVersionId
            on conflict (session_participant_id) do nothing
            """;

        await conn.ExecuteAsync(new CommandDefinition(
            sql,
            new
            {
                sessionId = request.SessionId,
                participantId,
                rulesetVersionId = request.RulesetVersionId,
                eventId = request.EventId
            },
            tx,
            cancellationToken: ct));
    }

    private static async Task ApplyCashflowAsync(
        Guid participantId,
        Guid eventId,
        IReadOnlyCollection<CashflowProjectionDb> projections,
        NpgsqlConnection conn,
        NpgsqlTransaction tx,
        CancellationToken ct)
    {
        var delta = projections.Sum(item => string.Equals(item.Direction, "IN", StringComparison.OrdinalIgnoreCase)
            ? item.Amount
            : -item.Amount);
        if (delta == 0)
        {
            return;
        }

        await conn.ExecuteAsync(new CommandDefinition(
            """
            update session_participant_balances
            set coins = coins + @delta,
                last_event_id = @eventId,
                updated_at = now()
            where session_participant_id = @participantId
            """,
            new { participantId, delta, eventId },
            tx,
            cancellationToken: ct));
    }

    private static async Task IncrementActionCounterAsync(
        Guid sessionId,
        Guid participantId,
        Guid rulesetVersionId,
        Guid rulesetActionId,
        Guid eventId,
        NpgsqlConnection conn,
        NpgsqlTransaction tx,
        CancellationToken ct)
    {
        if (rulesetActionId == Guid.Empty)
        {
            return;
        }

        await conn.ExecuteAsync(new CommandDefinition(
            """
            insert into session_participant_action_counters (
                session_id,
                session_participant_id,
                ruleset_version_id,
                ruleset_action_id,
                count,
                last_event_id
            )
            values (@sessionId, @participantId, @rulesetVersionId, @rulesetActionId, 1, @eventId)
            on conflict (session_participant_id, ruleset_action_id) do update
            set count = session_participant_action_counters.count + 1,
                last_event_id = excluded.last_event_id
            """,
            new { sessionId, participantId, rulesetVersionId, rulesetActionId, eventId },
            tx,
            cancellationToken: ct));
    }

    private static async Task ProjectNarrativeTriggersAsync(
        EventRequest request,
        Guid participantId,
        Guid rulesetActionId,
        Guid eventId,
        NpgsqlConnection conn,
        NpgsqlTransaction tx,
        CancellationToken ct)
    {
        if (rulesetActionId == Guid.Empty)
        {
            return;
        }

        await conn.ExecuteAsync(new CommandDefinition(
            """
            insert into session_narrative_logs (
                narrative_log_id,
                session_id,
                session_participant_id,
                ruleset_version_id,
                ruleset_narrative_id,
                ruleset_narrative_scene_id,
                source_event_id,
                shown_at,
                day,
                action_slot,
                payload_json
            )
            select
                gen_random_uuid(),
                @sessionId,
                @participantId,
                @rulesetVersionId,
                rtc.ruleset_narrative_id,
                scene.ruleset_narrative_scene_id,
                @eventId,
                @eventTimestamp,
                @day,
                @actionSlot,
                jsonb_build_object(
                    'trigger_condition_id', rtc.ruleset_trigger_condition_id,
                    'ruleset_action_id', rtc.ruleset_action_id
                )
            from ruleset_trigger_conditions rtc
            left join lateral (
                select rns.ruleset_narrative_scene_id
                from ruleset_narrative_scenes rns
                where rns.ruleset_version_id = rtc.ruleset_version_id
                  and rns.ruleset_narrative_id = rtc.ruleset_narrative_id
                order by rns.scene_order asc
                limit 1
            ) scene on true
            where rtc.ruleset_version_id = @rulesetVersionId
              and rtc.trigger_owner_type = 'NARRATIVE'
              and rtc.ruleset_action_id = @rulesetActionId
              and rtc.is_active
              and rtc.threshold_numeric <= (
                  select count(*)
                  from events e
                  where e.session_id = @sessionId
                    and e.session_player_id = @participantId
                    and e.ruleset_action_id = @rulesetActionId
              )
            on conflict (
                session_id,
                session_participant_id,
                ruleset_narrative_id,
                source_event_id
            ) do nothing
            """,
            new
            {
                sessionId = request.SessionId,
                participantId,
                rulesetVersionId = request.RulesetVersionId,
                rulesetActionId,
                eventId,
                eventTimestamp = request.Timestamp.ToUniversalTime(),
                day = Math.Max(1, request.DayIndex + 1),
                actionSlot = Math.Max(1, request.ActionSlot)
            },
            tx,
            cancellationToken: ct));
    }

    private async Task ProjectIngredientPurchaseAsync(
        EventRequest request,
        Guid participantId,
        NpgsqlConnection conn,
        NpgsqlTransaction tx,
        CancellationToken ct)
    {
        if (!_payloadReader.TryReadIngredientPurchase(request.Payload, out var cardId, out _))
        {
            return;
        }

        await ChangeIngredientQuantityAsync(request.SessionId, request.RulesetVersionId, participantId, cardId, 1, request.EventId, conn, tx, ct);
    }

    private async Task ProjectIngredientDiscardAsync(
        EventRequest request,
        Guid participantId,
        NpgsqlConnection conn,
        NpgsqlTransaction tx,
        CancellationToken ct)
    {
        if (!_payloadReader.TryGetString(request.Payload, "card_id", out var cardId))
        {
            return;
        }

        var quantity = _payloadReader.TryGetInt32(request.Payload, "amount", out var amount) ? amount : 1;
        await ChangeIngredientQuantityAsync(request.SessionId, request.RulesetVersionId, participantId, cardId, -Math.Max(1, quantity), request.EventId, conn, tx, ct);
    }

    private async Task ProjectOrderClaimAsync(
        EventRequest request,
        Guid participantId,
        NpgsqlConnection conn,
        NpgsqlTransaction tx,
        CancellationToken ct)
    {
        if (!_payloadReader.TryReadOrderClaim(request.Payload, out var requiredCards, out _))
        {
            return;
        }

        foreach (var cardId in requiredCards)
        {
            await ChangeIngredientQuantityAsync(request.SessionId, request.RulesetVersionId, participantId, cardId, -1, request.EventId, conn, tx, ct);
        }
    }

    private static async Task ChangeIngredientQuantityAsync(
        Guid sessionId,
        Guid rulesetVersionId,
        Guid participantId,
        string ingredientId,
        int delta,
        Guid eventId,
        NpgsqlConnection conn,
        NpgsqlTransaction tx,
        CancellationToken ct)
    {
        const string sql = """
            insert into session_participant_inventory (
                session_id,
                session_participant_id,
                ruleset_version_id,
                ruleset_game_asset_id,
                qty,
                last_event_id,
                created_at,
                updated_at
            )
            select
                @sessionId,
                @participantId,
                rga.ruleset_version_id,
                rga.ruleset_game_asset_id,
                greatest(0, @delta),
                @eventId,
                now(),
                now()
            from ruleset_game_assets rga
            where rga.ruleset_version_id = @rulesetVersionId
              and rga.asset_type = 'INGREDIENT'
              and (
                lower(rga.asset_code) = lower(@ingredientId)
                or lower(rga.display_name) = lower(@ingredientId)
              )
              and rga.is_active
            limit 1
            on conflict (session_participant_id, ruleset_game_asset_id) do update
            set qty = greatest(0, session_participant_inventory.qty + @delta),
                last_event_id = @eventId,
                updated_at = now()
            """;

        await conn.ExecuteAsync(new CommandDefinition(
            sql,
            new { sessionId, rulesetVersionId, participantId, ingredientId, delta, eventId },
            tx,
            cancellationToken: ct));
    }

    private async Task ProjectNeedPurchaseAsync(
        EventRequest request,
        Guid participantId,
        NpgsqlConnection conn,
        NpgsqlTransaction tx,
        CancellationToken ct)
    {
        if (!_payloadReader.TryReadNeedPurchase(request.Payload, out var cardId, out var amount, out var points))
        {
            return;
        }

        await conn.ExecuteAsync(new CommandDefinition(
            """
            insert into session_participant_need_purchases (
                session_participant_need_purchase_id,
                session_id,
                session_participant_id,
                ruleset_version_id,
                ruleset_need_id,
                sort_order,
                paid_amount,
                happiness_delta,
                purchased_at_day,
                source_event_id,
                created_at
            )
            select
                @entryId,
                @sessionId,
                @participantId,
                rn.ruleset_version_id,
                rn.ruleset_need_id,
                coalesce((
                    select max(existing.sort_order) + 1
                    from session_participant_need_purchases existing
                    where existing.session_participant_id = @participantId
                ), 1),
                @amount,
                @points,
                @day,
                @eventId,
                now()
            from ruleset_needs rn
            where rn.ruleset_version_id = @rulesetVersionId
              and lower(rn.need_code) = lower(@cardId)
              and rn.is_active
            limit 1
            """,
            new
            {
                entryId = Guid.NewGuid(),
                sessionId = request.SessionId,
                participantId,
                rulesetVersionId = request.RulesetVersionId,
                cardId,
                amount,
                points,
                day = Math.Max(1, request.DayIndex + 1),
                eventId = request.EventId
            },
            tx,
            cancellationToken: ct));

        await AddHappinessAsync(participantId, points, request.EventId, conn, tx, ct);
        await RefreshMissionCompletionAsync(request.SessionId, participantId, request.EventId, conn, tx, ct);
    }

    private async Task ProjectMissionAssignmentAsync(
        EventRequest request,
        Guid participantId,
        NpgsqlConnection conn,
        NpgsqlTransaction tx,
        CancellationToken ct)
    {
        if (!_payloadReader.TryReadMissionAssigned(request.Payload, out var missionId, out _, out _))
        {
            return;
        }

        await conn.ExecuteAsync(new CommandDefinition(
            """
            insert into session_participant_collection_missions (
                session_id,
                session_participant_id,
                ruleset_version_id,
                ruleset_collection_mission_id,
                is_completed,
                is_failed,
                reward_applied,
                last_event_id,
                assigned_at
            )
            select
                @sessionId,
                @participantId,
                rcm.ruleset_version_id,
                rcm.ruleset_collection_mission_id,
                false,
                false,
                false,
                @eventId,
                @assignedAt
            from ruleset_collection_missions rcm
            where rcm.ruleset_version_id = @rulesetVersionId
              and lower(rcm.mission_code) = lower(@missionId)
              and rcm.is_active
            limit 1
            on conflict (session_participant_id, ruleset_collection_mission_id) do update
            set assigned_at = excluded.assigned_at,
                is_completed = false,
                is_failed = false,
                reward_applied = false,
                last_event_id = excluded.last_event_id
            """,
            new
            {
                sessionId = request.SessionId,
                participantId,
                rulesetVersionId = request.RulesetVersionId,
                missionId,
                eventId = request.EventId,
                assignedAt = request.Timestamp.ToUniversalTime()
            },
            tx,
            cancellationToken: ct));

        await RefreshMissionCompletionAsync(request.SessionId, participantId, request.EventId, conn, tx, ct);
    }

    private static async Task RefreshMissionCompletionAsync(
        Guid sessionId,
        Guid participantId,
        Guid eventId,
        NpgsqlConnection conn,
        NpgsqlTransaction tx,
        CancellationToken ct)
    {
        await conn.ExecuteAsync(new CommandDefinition(
            """
            update session_participant_collection_missions spcm
            set is_completed = not exists (
                    select 1
                    from ruleset_collection_mission_requirements requirement
                    where requirement.ruleset_collection_mission_id = spcm.ruleset_collection_mission_id
                      and not exists (
                          select 1
                          from session_participant_need_purchases purchase
                          join ruleset_needs need on need.ruleset_need_id = purchase.ruleset_need_id
                          where purchase.session_id = @sessionId
                            and purchase.session_participant_id = @participantId
                            and (
                                (
                                    upper(requirement.requirement_type) = 'NEED_TIER'
                                    and lower(need.need_tier) = lower(requirement.required_need_tier)
                                )
                                or (
                                    upper(requirement.requirement_type) = 'ASSET'
                                    and need.ruleset_game_asset_id = requirement.required_asset_id
                                )
                            )
                      )
                ),
                is_failed = false,
                last_event_id = @eventId
            where spcm.session_id = @sessionId
              and spcm.session_participant_id = @participantId
            """,
            new { sessionId, participantId, eventId },
            tx,
            cancellationToken: ct));
    }

    private async Task ProjectDonationAsync(
        EventRequest request,
        Guid participantId,
        NpgsqlConnection conn,
        NpgsqlTransaction tx,
        CancellationToken ct)
    {
        if (!_payloadReader.TryReadAmount(request.Payload, out var amount))
        {
            return;
        }

        await conn.ExecuteAsync(new CommandDefinition(
            """
            update session_participant_balances
            set total_donasi = total_donasi + @amount,
                last_event_id = @eventId,
                updated_at = now()
            where session_participant_id = @participantId
            """,
            new
            {
                participantId,
                amount = (int)Math.Round(amount, MidpointRounding.AwayFromZero),
                eventId = request.EventId
            },
            tx,
            cancellationToken: ct));
    }

    private async Task ProjectDonationRankingAsync(
        EventRequest request,
        Guid participantId,
        NpgsqlConnection conn,
        NpgsqlTransaction tx,
        CancellationToken ct)
    {
        if (!_payloadReader.TryReadRankAwarded(request.Payload, out _, out var points))
        {
            return;
        }

        var day = Math.Max(1, request.DayIndex + 1);
        var eventNumber = Math.Max(1, (day + 2) / 7);
        await conn.ExecuteAsync(new CommandDefinition(
            """
            insert into session_donation_events (
                donation_event_id,
                session_id,
                event_ke,
                day,
                source_event_id,
                created_at
            )
            values (
                @donationEventId,
                @sessionId,
                @eventNumber,
                @day,
                @eventId,
                now()
            )
            on conflict (session_id, event_ke) do update
            set day = excluded.day,
                source_event_id = excluded.source_event_id
            """,
            new
            {
                donationEventId = Guid.NewGuid(),
                sessionId = request.SessionId,
                eventNumber,
                day,
                eventId = request.EventId
            },
            tx,
            cancellationToken: ct));

        await AddHappinessAsync(participantId, points, request.EventId, conn, tx, ct);
    }

    private async Task ProjectGoldTradeAsync(
        EventRequest request,
        Guid participantId,
        NpgsqlConnection conn,
        NpgsqlTransaction tx,
        CancellationToken ct)
    {
        var canonicalAction = GameActionCatalog.ResolveGameActionId(request.ActionType, request.Payload);
        if (!_payloadReader.TryReadGoldTrade(request.Payload, out var tradeType, out var qty, out var unitPrice, out var amount))
        {
            if (!_payloadReader.TryGetInt32(request.Payload, "qty", out qty) ||
                !_payloadReader.TryGetInt32(request.Payload, "unit_price", out unitPrice) ||
                !_payloadReader.TryGetInt32(request.Payload, "amount", out amount))
            {
                return;
            }

            tradeType = string.Equals(canonicalAction, GameActionCatalog.JualEmas, StringComparison.OrdinalIgnoreCase)
                ? "SELL"
                : "BUY";
        }

        var isBuy = string.Equals(canonicalAction, GameActionCatalog.InvestasiEmas, StringComparison.OrdinalIgnoreCase) ||
                    (!string.Equals(canonicalAction, GameActionCatalog.JualEmas, StringComparison.OrdinalIgnoreCase) &&
                     string.Equals(tradeType, "BUY", StringComparison.OrdinalIgnoreCase));
        var quantityDelta = isBuy ? qty : -qty;
        var amountDelta = isBuy ? amount : -amount;
        var metadataJson = JsonSerializer.Serialize(new
        {
            last_trade_type = tradeType.ToUpperInvariant(),
            unit_price = unitPrice,
            last_event_id = request.EventId
        });

        await conn.ExecuteAsync(new CommandDefinition(
            """
            insert into session_participant_gold_holdings (
                session_id,
                session_participant_id,
                ruleset_version_id,
                ruleset_game_asset_id,
                quantity,
                last_event_id,
                created_at,
                updated_at
            )
            select
                @sessionId,
                @participantId,
                rga.ruleset_version_id,
                rga.ruleset_game_asset_id,
                greatest(0, @quantityDelta),
                @eventId,
                now(),
                now()
            from ruleset_game_assets rga
            where rga.ruleset_version_id = @rulesetVersionId
              and rga.asset_type = 'GOLD'
              and rga.asset_code = 'gold_card_1'
            limit 1
            on conflict (session_participant_id, ruleset_game_asset_id) do update
            set quantity = greatest(0, coalesce(session_participant_gold_holdings.quantity, 0) + @quantityDelta),
                last_event_id = @eventId,
                updated_at = now()
            """,
            new
            {
                sessionId = request.SessionId,
                participantId,
                rulesetVersionId = request.RulesetVersionId,
                quantityDelta,
                eventId = request.EventId
            },
            tx,
            cancellationToken: ct));
    }

    private async Task ProjectInitialGoldAsync(
        EventRequest request,
        Guid participantId,
        NpgsqlConnection conn,
        NpgsqlTransaction tx,
        CancellationToken ct)
    {
        var quantity = _payloadReader.TryGetInt32(request.Payload, "qty", out var qty)
            ? Math.Max(1, qty)
            : 1;
        var assetCode = ReadOptionalCode(request.Payload, "asset_code") ?? "gold_card_1";

        await conn.ExecuteAsync(new CommandDefinition(
            """
            insert into session_participant_gold_holdings (
                session_id,
                session_participant_id,
                ruleset_version_id,
                ruleset_game_asset_id,
                quantity,
                last_event_id,
                created_at,
                updated_at
            )
            select
                @sessionId,
                @participantId,
                rga.ruleset_version_id,
                rga.ruleset_game_asset_id,
                @quantity,
                @eventId,
                now(),
                now()
            from ruleset_game_assets rga
            where rga.ruleset_version_id = @rulesetVersionId
              and rga.asset_type = 'GOLD'
              and rga.asset_code = @assetCode
            limit 1
            on conflict (session_participant_id, ruleset_game_asset_id) do update
            set quantity = coalesce(session_participant_gold_holdings.quantity, 0) + @quantity,
                last_event_id = @eventId,
                updated_at = now()
            """,
            new
            {
                sessionId = request.SessionId,
                participantId,
                rulesetVersionId = request.RulesetVersionId,
                assetCode,
                quantity,
                eventId = request.EventId
            },
            tx,
            cancellationToken: ct));
    }

    private async Task ProjectAwardedPointsAsync(
        EventRequest request,
        Guid participantId,
        NpgsqlConnection conn,
        NpgsqlTransaction tx,
        CancellationToken ct)
    {
        if (_payloadReader.TryReadPointsAwarded(request.Payload, out var points))
        {
            await AddHappinessAsync(participantId, points, request.EventId, conn, tx, ct);
        }
    }

    private async Task ProjectSavingDepositAsync(
        EventRequest request,
        Guid participantId,
        NpgsqlConnection conn,
        NpgsqlTransaction tx,
        CancellationToken ct)
    {
        if (!_payloadReader.TryReadSavingDeposit(request.Payload, out var goalId, out var amount))
        {
            return;
        }

        await conn.ExecuteAsync(new CommandDefinition(
            """
            insert into session_participant_financial_goals (
                session_id,
                session_participant_id,
                ruleset_version_id,
                ruleset_financial_goal_id,
                current_amount,
                target_amount,
                status,
                purchased_at_day,
                last_event_id,
                created_at,
                updated_at
            )
            select
                @sessionId,
                @participantId,
                rfg.ruleset_version_id,
                rfg.ruleset_financial_goal_id,
                least(@amount, rfg.purchase_price),
                rfg.purchase_price,
                'ONGOING',
                null,
                @eventId,
                now(),
                now()
            from ruleset_financial_goals rfg
            where rfg.ruleset_version_id = @rulesetVersionId
              and lower(rfg.goal_code) = lower(@goalId)
              and rfg.is_active
            limit 1
            on conflict (session_participant_id, ruleset_financial_goal_id) do update
            set current_amount = least(
                    session_participant_financial_goals.target_amount,
                    session_participant_financial_goals.current_amount + @amount
                ),
                last_event_id = @eventId,
                updated_at = now()
            """,
            new
            {
                sessionId = request.SessionId,
                participantId,
                rulesetVersionId = request.RulesetVersionId,
                goalId,
                amount,
                eventId = request.EventId
            },
            tx,
            cancellationToken: ct));

        await conn.ExecuteAsync(new CommandDefinition(
            """
            update session_participant_balances
            set saving = saving + @amount,
                last_event_id = @eventId,
                updated_at = now()
            where session_participant_id = @participantId
            """,
            new { participantId, amount, eventId = request.EventId },
            tx,
            cancellationToken: ct));
    }

    private async Task ProjectSavingWithdrawalAsync(
        EventRequest request,
        Guid participantId,
        NpgsqlConnection conn,
        NpgsqlTransaction tx,
        CancellationToken ct)
    {
        if (!_payloadReader.TryGetString(request.Payload, "goal_id", out var goalId) ||
            !_payloadReader.TryGetInt32(request.Payload, "amount", out var amount))
        {
            return;
        }

        await conn.ExecuteAsync(new CommandDefinition(
            """
            update session_participant_financial_goals goal
            set current_amount = greatest(0, goal.current_amount - @amount),
                last_event_id = @eventId,
                updated_at = now()
            from ruleset_financial_goals rfg
            where goal.session_participant_id = @participantId
              and goal.ruleset_financial_goal_id = rfg.ruleset_financial_goal_id
              and lower(rfg.goal_code) = lower(@goalId);

            update session_participant_balances
            set saving = greatest(0, saving - @amount),
                last_event_id = @eventId,
                updated_at = now()
            where session_participant_id = @participantId;
            """,
            new { participantId, goalId, amount, eventId = request.EventId },
            tx,
            cancellationToken: ct));
    }

    private async Task ProjectSavingGoalAchievedAsync(
        EventRequest request,
        Guid participantId,
        NpgsqlConnection conn,
        NpgsqlTransaction tx,
        CancellationToken ct)
    {
        if (!_payloadReader.TryReadSavingGoalAchieved(request.Payload, out var goalId, out var points, out var cost))
        {
            return;
        }

        await conn.ExecuteAsync(new CommandDefinition(
            """
            insert into session_participant_financial_goals (
                session_id,
                session_participant_id,
                ruleset_version_id,
                ruleset_financial_goal_id,
                current_amount,
                target_amount,
                status,
                purchased_at_day,
                last_event_id,
                created_at,
                updated_at
            )
            select
                @sessionId,
                @participantId,
                rfg.ruleset_version_id,
                rfg.ruleset_financial_goal_id,
                rfg.purchase_price,
                rfg.purchase_price,
                'COMPLETED',
                @day,
                @eventId,
                now(),
                now()
            from ruleset_financial_goals rfg
            where rfg.ruleset_version_id = @rulesetVersionId
              and lower(rfg.goal_code) = lower(@goalId)
              and rfg.is_active
            limit 1
            on conflict (session_participant_id, ruleset_financial_goal_id) do update
            set current_amount = excluded.current_amount,
                target_amount = excluded.target_amount,
                status = 'COMPLETED',
                purchased_at_day = excluded.purchased_at_day,
                last_event_id = @eventId,
                updated_at = now()
            """,
            new
            {
                sessionId = request.SessionId,
                participantId,
                rulesetVersionId = request.RulesetVersionId,
                goalId,
                eventId = request.EventId,
                day = Math.Max(1, request.DayIndex + 1)
            },
            tx,
            cancellationToken: ct));

        await conn.ExecuteAsync(new CommandDefinition(
            """
            update session_participant_balances
            set saving = greatest(0, saving - @cost),
                happiness = happiness + @points,
                last_event_id = @eventId,
                updated_at = now()
            where session_participant_id = @participantId
            """,
            new { participantId, cost, points, eventId = request.EventId },
            tx,
            cancellationToken: ct));
    }

    private async Task ProjectLoanTakenAsync(
        EventRequest request,
        Guid participantId,
        NpgsqlConnection conn,
        NpgsqlTransaction tx,
        CancellationToken ct)
    {
        if (!_payloadReader.TryReadLoanTaken(
                request.Payload,
                out var loanId,
                out var principal,
                out var repaymentAmount,
                out var duration,
                out var penaltyPoints))
        {
            return;
        }

        var metadataJson = JsonSerializer.Serialize(new
        {
            principal,
            repayment_amount = repaymentAmount,
            duration_days = duration,
            penalty_points = penaltyPoints,
            repaid_amount = 0
        });

        await conn.ExecuteAsync(new CommandDefinition(
            """
            insert into session_participant_loans (
                session_participant_loan_id,
                session_id,
                session_participant_id,
                ruleset_version_id,
                ruleset_sharia_loan_id,
                principal_amount,
                outstanding_amount,
                repayment_amount,
                status,
                source_event_id,
                last_event_id,
                metadata_json,
                created_at,
                updated_at
            )
            select
                @loanHoldingId,
                @sessionId,
                @participantId,
                rsl.ruleset_version_id,
                rsl.ruleset_sharia_loan_id,
                @principal,
                @principal,
                @repaymentAmount,
                'ACTIVE',
                @eventId,
                @eventId,
                @metadataJson::jsonb,
                now(),
                now()
            from ruleset_sharia_loans rsl
            where rsl.ruleset_version_id = @rulesetVersionId
              and lower(rsl.loan_code) = lower(@loanId)
            limit 1
            on conflict (session_participant_id, ruleset_sharia_loan_id) do update
            set principal_amount = excluded.principal_amount,
                outstanding_amount = excluded.outstanding_amount,
                repayment_amount = excluded.repayment_amount,
                status = excluded.status,
                last_event_id = excluded.last_event_id,
                metadata_json = session_participant_loans.metadata_json || excluded.metadata_json,
                updated_at = now()
            """,
            new
            {
                loanHoldingId = Guid.NewGuid(),
                sessionId = request.SessionId,
                participantId,
                rulesetVersionId = request.RulesetVersionId,
                loanId,
                principal,
                repaymentAmount,
                metadataJson,
                eventId = request.EventId
            },
            tx,
            cancellationToken: ct));
    }

    private async Task ProjectLoanRepaidAsync(
        EventRequest request,
        Guid participantId,
        NpgsqlConnection conn,
        NpgsqlTransaction tx,
        CancellationToken ct)
    {
        if (!_payloadReader.TryReadLoanRepay(request.Payload, out var loanId, out var amount))
        {
            return;
        }

        await conn.ExecuteAsync(new CommandDefinition(
            """
            update session_participant_loans
            set outstanding_amount = greatest(0, coalesce(outstanding_amount, 0) - @amount),
                status = case when coalesce(outstanding_amount, 0) - @amount <= 0 then 'PAID' else 'ACTIVE' end,
                last_event_id = @eventId,
                metadata_json = metadata_json || jsonb_build_object(
                    'last_repayment', @amount,
                    'last_repaid_at', @repaidAt
                ),
                updated_at = now()
            where session_participant_id = @participantId
              and ruleset_sharia_loan_id = (
                select ruleset_sharia_loan_id
                from ruleset_sharia_loans
                where ruleset_version_id = @rulesetVersionId
                  and lower(loan_code) = lower(@loanId)
                limit 1
              )
            """,
            new
            {
                participantId,
                rulesetVersionId = request.RulesetVersionId,
                loanId,
                amount,
                eventId = request.EventId,
                repaidAt = request.Timestamp.ToUniversalTime()
            },
            tx,
            cancellationToken: ct));
    }

    private async Task ProjectInsurancePurchasedAsync(
        EventRequest request,
        Guid participantId,
        NpgsqlConnection conn,
        NpgsqlTransaction tx,
        CancellationToken ct)
    {
        if (!_payloadReader.TryReadInsurance(request.Payload, out var premium))
        {
            return;
        }

        var productCode = ReadOptionalCode(request.Payload, "policy_id")
                          ?? ReadOptionalCode(request.Payload, "product_code")
                          ?? "multirisk_basic";
        var usageLimit = await conn.QuerySingleOrDefaultAsync<int?>(
            new CommandDefinition(
                """
                select usage_limit
                from ruleset_insurance_products
                where ruleset_version_id = @rulesetVersionId
                  and lower(product_code) = lower(@productCode)
                order by sort_order asc
                limit 1
                """,
                new { rulesetVersionId = request.RulesetVersionId, productCode },
                tx,
                cancellationToken: ct)) ?? 1;
        var metadataJson = JsonSerializer.Serialize(new
        {
            premium,
            usage_limit = usageLimit,
            used_count = 0
        });

        await conn.ExecuteAsync(new CommandDefinition(
            """
            insert into session_participant_insurances (
                session_participant_insurance_id,
                session_id,
                session_participant_id,
                ruleset_version_id,
                ruleset_insurance_product_id,
                premium_paid,
                remaining_uses,
                status,
                source_event_id,
                last_event_id,
                metadata_json,
                created_at,
                updated_at
            )
            select
                @insuranceHoldingId,
                @sessionId,
                @participantId,
                rip.ruleset_version_id,
                rip.ruleset_insurance_product_id,
                @premium,
                greatest(@usageLimit, rip.usage_limit),
                'ACTIVE',
                @eventId,
                @eventId,
                @metadataJson::jsonb,
                now(),
                now()
            from ruleset_insurance_products rip
            where rip.ruleset_version_id = @rulesetVersionId
              and lower(rip.product_code) = lower(@productCode)
            limit 1
            on conflict (session_participant_id, ruleset_insurance_product_id) do update
            set premium_paid = excluded.premium_paid,
                remaining_uses = excluded.remaining_uses,
                status = excluded.status,
                last_event_id = excluded.last_event_id,
                metadata_json = session_participant_insurances.metadata_json || excluded.metadata_json,
                updated_at = now()
            """,
            new
            {
                insuranceHoldingId = Guid.NewGuid(),
                sessionId = request.SessionId,
                participantId,
                rulesetVersionId = request.RulesetVersionId,
                productCode,
                premium,
                usageLimit,
                metadataJson,
                eventId = request.EventId
            },
            tx,
            cancellationToken: ct));
    }

    private static async Task ProjectInsuranceUsedAsync(
        EventRequest request,
        Guid participantId,
        NpgsqlConnection conn,
        NpgsqlTransaction tx,
        CancellationToken ct)
    {
        await conn.ExecuteAsync(new CommandDefinition(
            """
            update session_participant_insurances asset
            set remaining_uses = greatest(remaining_uses - 1, 0),
                last_event_id = @eventId,
                metadata_json = asset.metadata_json || jsonb_build_object(
                    'used_count', coalesce((asset.metadata_json->>'used_count')::int, 0) + 1,
                    'last_used_event_id', @eventId,
                    'last_used_at', @usedAt
                ),
                status = case
                    when greatest(remaining_uses - 1, 0) = 0 then 'INACTIVE'
                    else 'ACTIVE'
                end,
                updated_at = now()
            where asset.session_participant_insurance_id = (
                select candidate.session_participant_insurance_id
                from session_participant_insurances candidate
                where candidate.session_participant_id = @participantId
                  and candidate.status = 'ACTIVE'
                order by candidate.created_at asc
                limit 1
            )
            """,
            new
            {
                participantId,
                eventId = request.EventId,
                usedAt = request.Timestamp.ToUniversalTime()
            },
            tx,
            cancellationToken: ct));
    }

    private async Task ProjectTieBreakerAsync(
        EventRequest request,
        Guid participantId,
        Guid eventId,
        NpgsqlConnection conn,
        NpgsqlTransaction tx,
        CancellationToken ct)
    {
        if (!_payloadReader.TryReadTieBreaker(request.Payload, out var tieNumber))
        {
            return;
        }

        var cardCode = ReadOptionalCode(request.Payload, "card_code")
                       ?? ReadOptionalCode(request.Payload, "tie_breaker_code");
        await conn.ExecuteAsync(new CommandDefinition(
            """
            insert into session_participant_tie_breakers (
                session_participant_tie_breaker_id,
                session_id,
                session_participant_id,
                ruleset_version_id,
                ruleset_game_asset_id,
                tie_number,
                source_event_id,
                assigned_at,
                metadata_json
            )
            select
                @tieBreakerId,
                @sessionId,
                @participantId,
                @rulesetVersionId,
                rga.ruleset_game_asset_id,
                @tieNumber,
                @eventId,
                @assignedAt,
                '{}'::jsonb
            from ruleset_game_assets rga
            where rga.ruleset_version_id = @rulesetVersionId
              and rga.asset_type = 'TIE_BREAKER'
              and (
                  lower(rga.asset_code) = lower(@cardCode)
                  or (
                      @cardCode is null
                      and (rga.metadata_json->>'tie_number')::int = @tieNumber
                  )
              )
            limit 1
            on conflict (session_participant_id) do update
            set tie_number = excluded.tie_number,
                ruleset_game_asset_id = excluded.ruleset_game_asset_id,
                source_event_id = excluded.source_event_id,
                assigned_at = excluded.assigned_at
            """,
            new
            {
                tieBreakerId = Guid.NewGuid(),
                sessionId = request.SessionId,
                participantId,
                rulesetVersionId = request.RulesetVersionId,
                tieNumber,
                cardCode,
                eventId,
                assignedAt = request.Timestamp.ToUniversalTime()
            },
            tx,
            cancellationToken: ct));
    }

    private async Task ProjectPensionRankingAsync(
        EventRequest request,
        Guid participantId,
        NpgsqlConnection conn,
        NpgsqlTransaction tx,
        CancellationToken ct)
    {
        if (!_payloadReader.TryReadRankAwarded(request.Payload, out _, out var points))
        {
            return;
        }

        await AddHappinessAsync(participantId, points, request.EventId, conn, tx, ct);
    }

    private static async Task AddHappinessAsync(
        Guid participantId,
        int points,
        Guid eventId,
        NpgsqlConnection conn,
        NpgsqlTransaction tx,
        CancellationToken ct)
    {
        if (points == 0)
        {
            return;
        }

        await conn.ExecuteAsync(new CommandDefinition(
            """
            update session_participant_balances
            set happiness = greatest(0, happiness + @points),
                last_event_id = @eventId,
                updated_at = now()
            where session_participant_id = @participantId
            """,
            new { participantId, points, eventId },
            tx,
            cancellationToken: ct));
    }

    private static async Task MarkIncompleteMissionsFailedAsync(
        Guid sessionId,
        Guid eventId,
        NpgsqlConnection conn,
        NpgsqlTransaction tx,
        CancellationToken ct)
    {
        await conn.ExecuteAsync(new CommandDefinition(
            """
            update session_participant_collection_missions
            set is_failed = true,
                reward_applied = false,
                last_event_id = @eventId
            where session_id = @sessionId
              and not is_completed
            """,
            new { sessionId, eventId },
            tx,
            cancellationToken: ct));
    }

    private static async Task UpdateProjectionCheckpointAsync(
        Guid sessionId,
        long sequenceNumber,
        Guid eventId,
        NpgsqlConnection conn,
        NpgsqlTransaction tx,
        CancellationToken ct)
    {
        await conn.ExecuteAsync(new CommandDefinition(
            """
            insert into session_projection_checkpoints (
                session_id,
                last_sequence_number,
                last_event_id,
                projected_at,
                status,
                metadata_json
            )
            values (
                @sessionId,
                @sequenceNumber,
                @eventId,
                now(),
                'IDLE',
                jsonb_build_object('source', 'SessionEventProjector')
            )
            on conflict (session_id) do update
            set last_sequence_number = greatest(session_projection_checkpoints.last_sequence_number, excluded.last_sequence_number),
                last_event_id = case
                    when excluded.last_sequence_number >= session_projection_checkpoints.last_sequence_number then excluded.last_event_id
                    else session_projection_checkpoints.last_event_id
                end,
                projected_at = now(),
                status = 'IDLE',
                error_message = null,
                metadata_json = session_projection_checkpoints.metadata_json || excluded.metadata_json
            """,
            new { sessionId, sequenceNumber, eventId },
            tx,
            cancellationToken: ct));
    }

    private string? ReadOptionalCode(JsonElement payload, string propertyName)
    {
        return _payloadReader.TryGetOptionalString(payload, propertyName, out var value) &&
               !string.IsNullOrWhiteSpace(value)
            ? value.Trim()
            : null;
    }

    private static string ResolvePhase(string weekday, bool isGameOver)
    {
        if (isGameOver)
        {
            return "GAME_END";
        }

        return weekday.Trim().ToUpperInvariant() switch
        {
            "FRI" => "DONATION_DAY",
            "SAT" => "GOLD_INVESTMENT_DAY",
            "SUN" => "DAY_END",
            _ => "PLAYER_TURN"
        };
    }
}
