using System.Text.Json;
using Cashflowpoly.Api.Contracts;
using Dapper;
using Npgsql;

namespace Cashflowpoly.Api.Data;

public sealed class SessionStateRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public SessionStateRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<RulesetSectionDb?> GetRulesetSectionAsync(
        string mode,
        Guid? rulesetId,
        Guid? instructorUserId,
        CancellationToken ct)
    {
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        var requested = await ResolveRequestedRulesetVersionAsync(
            conn,
            mode.ToUpperInvariant(),
            rulesetId,
            instructorUserId,
            ct);
        if (requested is null)
        {
            return null;
        }

        return new RulesetSectionDb
        {
            RulesetId = requested.RulesetId,
            RulesetVersionId = requested.RulesetVersionId,
            Mode = requested.Mode,
            ConfigJson = string.IsNullOrWhiteSpace(requested.ConfigJson) ? "{}" : requested.ConfigJson
        };
    }

    public async Task<CreateSessionWithStateResult?> CreateSessionAsync(
        string sessionName,
        string mode,
        IReadOnlyList<string> playerNames,
        Guid? rulesetId,
        Guid instructorUserId,
        string? createdBy,
        CancellationToken ct)
    {
        var ruleset = await GetRulesetSectionAsync(mode, rulesetId, instructorUserId, ct);
        if (ruleset is null)
        {
            return null;
        }

        var catalog = RulesetSectionCatalog.FromConfig(ruleset.ConfigJson);
        var gameConfig = catalog.GameConfigValues;
        var sessionId = Guid.NewGuid();
        var now = DateTimeOffset.UtcNow;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        var sourceRulesetVersionId = await ResolveDefaultRelationalRulesetVersionIdAsync(conn, ruleset.Mode, ct);
        var collectionMissionCards = sourceRulesetVersionId.HasValue
            ? (await conn.QueryAsync<CollectionMissionSectionRow>(
                new CommandDefinition(
                    """
                    select
                        ruleset_catalog_item_id as CollectionMissionCardId,
                        item_code as MissionCode,
                        item_name as MissionName
                    from ruleset_catalog_items
                    where ruleset_version_id = @rulesetVersionId
                      and item_type = 'COLLECTION_MISSION'
                      and is_active
                    order by sort_order asc, item_code asc
                    """,
                    new { rulesetVersionId = sourceRulesetVersionId.Value },
                    cancellationToken: ct))).ToList()
            : [];
        await using var tx = await conn.BeginTransactionAsync(ct);

        const string insertSessionSql = """
            insert into sessions (session_id, session_name, mode, status, started_at, ended_at, instructor_user_id, created_at)
            values (@sessionId, @sessionName, @mode, 'STARTED', @startedAt, null, @instructorUserId, @createdAt)
            """;

        await conn.ExecuteAsync(new CommandDefinition(
            insertSessionSql,
            new
            {
                sessionId,
                sessionName,
                mode = mode.ToUpperInvariant(),
                startedAt = now,
                instructorUserId,
                createdAt = now
            },
            tx,
            cancellationToken: ct));

        const string insertActivationSql = """
            insert into session_ruleset_activations (activation_id, session_id, ruleset_version_id, activated_at, activated_by)
            values (@activationId, @sessionId, @rulesetVersionId, @activatedAt, @activatedBy)
            """;

        await conn.ExecuteAsync(new CommandDefinition(
            insertActivationSql,
            new
            {
                activationId = Guid.NewGuid(),
                sessionId,
                rulesetVersionId = ruleset.RulesetVersionId,
                activatedAt = now,
                activatedBy = createdBy
            },
            tx,
            cancellationToken: ct));

        const string insertSessionStateSql = """
            insert into session_states (
                session_id, day, weekday, turn_number, current_session_player_id, current_action_index,
                moves_left, finish_day, phase, is_game_over, state_version, created_at, updated_at
            )
            values (@sessionId, 1, @weekday, 1, null, 1, @movesLeft, @finishDay, 'PLAYER_TURN', false, 1, @createdAt, @createdAt)
            """;

        await conn.ExecuteAsync(new CommandDefinition(
            insertSessionStateSql,
            new
            {
                sessionId,
                weekday = ResolveWeekdayCode(1),
                movesLeft = gameConfig.ActionsPerTurn,
                finishDay = gameConfig.FinishDay,
                createdAt = now
            },
            tx,
            cancellationToken: ct));

        const string insertPlayerSql = """
            insert into app_users (user_id, username, display_name, password_hash, role, is_active, created_at)
            values (@userId, @username, @displayName, crypt(@password, gen_salt('bf', 10)), 'PLAYER', true, @createdAt)
            """;

        const string insertSessionPlayerSql = """
            insert into session_players (session_player_id, session_id, user_id, join_order, role, created_at)
            values (@sessionPlayerId, @sessionId, @userId, @joinOrder, 'PLAYER', @createdAt)
            """;

        const string insertPlayerStateSql = """
            insert into session_player_states (session_player_id, coins, happiness, saving, created_at, updated_at)
            values (@sessionPlayerId, @coins, @happiness, @saving, @createdAt, @createdAt)
            """;

        const string insertDonationTotalSql = """
            insert into session_player_peduli_donasi (session_player_id, total_donasi)
            values (@sessionPlayerId, 0)
            """;

        const string insertQuestSql = """
            insert into session_player_quest_progress (
                session_player_id, quest_id, progress, target, is_completed, is_reward_claimed
            )
            values (@sessionPlayerId, @questId, 0, @target, false, false)
            """;

        const string insertMissionSql = """
            insert into session_player_collection_missions (
                session_player_id, ruleset_catalog_item_id, is_completed, is_failed, reward_applied, assigned_at
            )
            values (@sessionPlayerId, @collectionMissionCardId, false, false, false, @assignedAt)
            """;

        for (var index = 0; index < playerNames.Count; index++)
        {
            var userId = Guid.NewGuid();
            var sessionPlayerId = Guid.NewGuid();
            var playerName = playerNames[index].Trim();
            var joinOrder = index + 1;
            var username = BuildSyntheticPlayerUsername(sessionId, joinOrder);
            var password = $"dev-only-{sessionId:N}-{joinOrder}";

            await conn.ExecuteAsync(new CommandDefinition(
                insertPlayerSql,
                new { userId, username, displayName = playerName, password, createdAt = now },
                tx,
                cancellationToken: ct));

            await conn.ExecuteAsync(new CommandDefinition(
                insertSessionPlayerSql,
                new { sessionPlayerId, sessionId, userId, joinOrder, createdAt = now },
                tx,
                cancellationToken: ct));

            await conn.ExecuteAsync(new CommandDefinition(
                insertPlayerStateSql,
                new
                {
                    sessionPlayerId,
                    coins = gameConfig.InitialCoins,
                    happiness = gameConfig.InitialHappiness,
                    saving = gameConfig.InitialSaving,
                    createdAt = now
                },
                tx,
                cancellationToken: ct));
            await conn.ExecuteAsync(new CommandDefinition(insertDonationTotalSql, new { sessionPlayerId }, tx, cancellationToken: ct));

            foreach (var quest in catalog.QuestTargets)
            {
                await conn.ExecuteAsync(new CommandDefinition(
                    insertQuestSql,
                    new { sessionPlayerId, questId = quest.Key, target = quest.Value },
                    tx,
                    cancellationToken: ct));
            }

            if (collectionMissionCards.Count > 0)
            {
                var mission = collectionMissionCards[index % collectionMissionCards.Count];
                await conn.ExecuteAsync(new CommandDefinition(
                    insertMissionSql,
                    new
                    {
                        sessionPlayerId,
                        collectionMissionCardId = mission.CollectionMissionCardId,
                        assignedAt = now
                    },
                    tx,
                    cancellationToken: ct));
            }
        }

        await tx.CommitAsync(ct);

        var state = await GetStateAsync(sessionId, ct);
        if (state is null)
        {
            throw new InvalidOperationException("Session state gagal dibuat.");
        }

        return new CreateSessionWithStateResult(sessionId, ruleset.RulesetId, ruleset.RulesetVersionId, state);
    }

    public async Task<SessionStateResponse?> GetStateAsync(Guid sessionId, CancellationToken ct)
    {
        const string stateSql = """
            select
                session_id,
                state_version,
                day,
                turn_number as turn,
                moves_left,
                finish_day,
                is_game_over,
                '{}'::jsonb::text as ui_state_json
            from session_states
            where session_id = @sessionId
            """;

        const string playersSql = """
            select
                sp.session_player_id,
                sp.user_id,
                sp.join_order as player_index,
                u.display_name as name,
                coalesce(ps.coins, 0) as coins,
                coalesce(ps.happiness, 0) as happiness,
                coalesce(ps.saving, 0) as saving,
                coalesce(pd.total_donasi, 0) as total_donasi
            from session_players sp
            join app_users u on u.user_id = sp.user_id
            left join session_player_states ps on ps.session_player_id = sp.session_player_id
            left join session_player_peduli_donasi pd on pd.session_player_id = sp.session_player_id
            where sp.session_id = @sessionId
            order by sp.join_order asc, sp.created_at asc
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        var state = await conn.QuerySingleOrDefaultAsync<SessionStateRow>(
            new CommandDefinition(stateSql, new { sessionId }, cancellationToken: ct));
        if (state is null)
        {
            return null;
        }

        var playerRows = (await conn.QueryAsync<SessionPlayerStateRow>(
            new CommandDefinition(playersSql, new { sessionId }, cancellationToken: ct))).ToList();

        var players = playerRows.Select(row => new SessionPlayerStateDto
        {
            SessionPlayerId = row.SessionPlayerId,
            UserId = row.UserId,
            PlayerIndex = row.PlayerIndex,
            Name = row.Name,
            Coins = row.Coins,
            Happiness = row.Happiness,
            Saving = row.Saving,
            TotalDonasi = row.TotalDonasi
        }).ToList();

        var bySessionPlayerId = players.ToDictionary(player => player.SessionPlayerId);
        var sessionPlayerIds = bySessionPlayerId.Keys.ToArray();

        if (sessionPlayerIds.Length > 0)
        {
            await LoadPlayerChildrenAsync(conn, sessionPlayerIds, bySessionPlayerId, ct);
        }

        var donationEvents = await LoadDonationEventsAsync(conn, sessionId, ct);

        return new SessionStateResponse
        {
            SessionId = state.SessionId,
            StateVersion = state.StateVersion,
            Day = state.Day,
            Turn = state.Turn,
            MovesLeft = state.MovesLeft,
            FinishDay = state.FinishDay,
            IsGameOver = state.IsGameOver,
            UiState = ParseJsonOrEmpty(state.UiStateJson),
            Players = players,
            DonationEvents = donationEvents
        };
    }

    public async Task<SaveSessionStateResult> SaveStateAsync(
        Guid sessionId,
        SaveSessionStateRequest request,
        CancellationToken ct)
    {
        const string lockSql = """
            select state_version
            from session_states
            where session_id = @sessionId
            for update
            """;

        const string updateSessionStateSql = """
            update session_states
            set day = @day,
                weekday = @weekday,
                turn_number = @turn,
                current_session_player_id = @currentSessionPlayerId,
                current_action_index = @currentActionIndex,
                moves_left = @movesLeft,
                finish_day = @finishDay,
                phase = @phase,
                is_game_over = @isGameOver,
                state_version = @newVersion,
                updated_at = now()
            where session_id = @sessionId
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        await using var tx = await conn.BeginTransactionAsync(ct);

        var currentVersion = await conn.QuerySingleOrDefaultAsync<long?>(
            new CommandDefinition(lockSql, new { sessionId }, tx, cancellationToken: ct));
        if (!currentVersion.HasValue)
        {
            await tx.RollbackAsync(ct);
            return SaveSessionStateResult.NotFound();
        }

        if (currentVersion.Value != request.StateVersion)
        {
            await tx.RollbackAsync(ct);
            return SaveSessionStateResult.Stale(currentVersion.Value);
        }

        var newVersion = currentVersion.Value + 1;
        var weekday = ResolveWeekdayCode(request.Day);
        var currentSessionPlayerId = request.Players?
            .FirstOrDefault(player => player.PlayerIndex == request.Turn)
            ?.SessionPlayerId;
        await DeleteSnapshotChildrenAsync(conn, tx, sessionId, ct);

        await conn.ExecuteAsync(new CommandDefinition(
            updateSessionStateSql,
            new
            {
                sessionId,
                day = request.Day,
                weekday,
                turn = request.Turn,
                currentSessionPlayerId,
                currentActionIndex = ResolveCurrentActionIndex(request.MovesLeft),
                movesLeft = request.MovesLeft,
                finishDay = request.FinishDay,
                phase = ResolvePhase(weekday, request.IsGameOver),
                isGameOver = request.IsGameOver,
                newVersion
            },
            tx,
            cancellationToken: ct));

        foreach (var player in request.Players ?? [])
        {
            await SavePlayerSnapshotAsync(conn, tx, sessionId, player, ct);
        }

        foreach (var donationEvent in request.DonationEvents ?? [])
        {
            await SaveDonationEventAsync(conn, tx, sessionId, donationEvent, ct);
        }

        await SaveLastActionAsync(conn, tx, sessionId, newVersion, request, ct);

        await tx.CommitAsync(ct);

        var state = await GetStateAsync(sessionId, ct);
        if (state is null)
        {
            return SaveSessionStateResult.NotFound();
        }

        return SaveSessionStateResult.Saved(state);
    }

    private static async Task LoadPlayerChildrenAsync(
        NpgsqlConnection conn,
        Guid[] sessionPlayerIds,
        IReadOnlyDictionary<Guid, SessionPlayerStateDto> players,
        CancellationToken ct)
    {
        const string bahanSql = """
            select spi.session_player_id, i.display_name as nama, spi.qty as jumlah
            from session_player_ingredients spi
            join ingredients i on i.ingredient_id = spi.ingredient_id
            where session_player_id = any(@sessionPlayerIds)
            order by i.display_name asc
            """;

        const string kebutuhanSql = """
            select
                spn.session_player_id,
                rci.item_name as nama,
                coalesce(rci.payload_json->>'tipe', '') as tipe
            from session_player_needs spn
            join ruleset_catalog_items rci on rci.ruleset_catalog_item_id = spn.ruleset_catalog_item_id
            where spn.session_player_id = any(@sessionPlayerIds)
              and rci.item_type = 'NEED'
            order by spn.sort_order asc, rci.item_name asc
            """;

        const string tujuanSql = """
            select spfg.session_player_id, rci.item_name as nama, spfg.purchased_at_day
            from session_player_financial_goals spfg
            join ruleset_catalog_items rci on rci.ruleset_catalog_item_id = spfg.ruleset_catalog_item_id
            where spfg.session_player_id = any(@sessionPlayerIds)
              and rci.item_type = 'FINANCIAL_GOAL'
            order by spfg.purchased_at_day asc, rci.item_name asc
            """;

        const string targetSql = """
            select spcm.session_player_id, rci.item_code as id, spcm.is_completed, spcm.is_failed, spcm.reward_applied
            from session_player_collection_missions spcm
            join ruleset_catalog_items rci on rci.ruleset_catalog_item_id = spcm.ruleset_catalog_item_id
            where spcm.session_player_id = any(@sessionPlayerIds)
              and rci.item_type = 'COLLECTION_MISSION'
            order by rci.item_code asc
            """;

        const string questSql = """
            select session_player_id, quest_id as id, progress, target, is_completed, is_reward_claimed
            from session_player_quest_progress
            where session_player_id = any(@sessionPlayerIds)
            order by quest_id asc
            """;

        const string counterSql = """
            select session_player_id, action_id as aksi, count
            from session_player_action_counters
            where session_player_id = any(@sessionPlayerIds)
            order by action_id asc
            """;

        foreach (var row in await conn.QueryAsync<BahanRow>(new CommandDefinition(bahanSql, new { sessionPlayerIds }, cancellationToken: ct)))
        {
            if (players.TryGetValue(row.SessionPlayerId, out var player))
            {
                player.Bahan.Add(new BahanItemDto(row.Nama, row.Jumlah));
            }
        }

        foreach (var row in await conn.QueryAsync<KebutuhanRow>(new CommandDefinition(kebutuhanSql, new { sessionPlayerIds }, cancellationToken: ct)))
        {
            if (players.TryGetValue(row.SessionPlayerId, out var player))
            {
                player.Kebutuhan.Add(new KebutuhanItemDto(row.Nama, row.Tipe));
            }
        }

        foreach (var row in await conn.QueryAsync<TujuanFinansialRow>(new CommandDefinition(tujuanSql, new { sessionPlayerIds }, cancellationToken: ct)))
        {
            if (players.TryGetValue(row.SessionPlayerId, out var player))
            {
                player.TujuanFinansial.Add(new TujuanFinansialItemDto(row.Nama, row.PurchasedAtDay));
            }
        }

        foreach (var row in await conn.QueryAsync<TargetKebutuhanRow>(new CommandDefinition(targetSql, new { sessionPlayerIds }, cancellationToken: ct)))
        {
            if (players.TryGetValue(row.SessionPlayerId, out var player))
            {
                player.TargetKebutuhan.Add(new TargetKebutuhanProgressDto(row.Id, row.IsCompleted, row.IsFailed, row.RewardApplied));
            }
        }

        foreach (var row in await conn.QueryAsync<QuestProgressRow>(new CommandDefinition(questSql, new { sessionPlayerIds }, cancellationToken: ct)))
        {
            if (players.TryGetValue(row.SessionPlayerId, out var player))
            {
                player.QuestProgress.Add(new QuestProgressDto(row.Id, row.Progress, row.Target, row.IsCompleted, row.IsRewardClaimed));
            }
        }

        foreach (var row in await conn.QueryAsync<ActionCounterRow>(new CommandDefinition(counterSql, new { sessionPlayerIds }, cancellationToken: ct)))
        {
            if (players.TryGetValue(row.SessionPlayerId, out var player))
            {
                player.ActionCounters.Add(new ActionCounterDto(row.Aksi, row.Count));
            }
        }
    }

    private static async Task<List<DonationEventDto>> LoadDonationEventsAsync(
        NpgsqlConnection conn,
        Guid sessionId,
        CancellationToken ct)
    {
        const string eventsSql = """
            select donation_event_id as event_id, event_ke, day, rankings_json::text as rankings_json
            from session_donation_events
            where session_id = @sessionId
            order by event_ke asc
            """;

        var eventRows = (await conn.QueryAsync<DonationEventRow>(
            new CommandDefinition(eventsSql, new { sessionId }, cancellationToken: ct))).ToList();
        var results = new List<DonationEventDto>(eventRows.Count);

        foreach (var row in eventRows)
        {
            var item = new DonationEventDto
            {
                EventKe = row.EventKe,
                Day = row.Day
            };

            if (!string.IsNullOrWhiteSpace(row.RankingsJson))
            {
                using var document = JsonDocument.Parse(row.RankingsJson);
                foreach (var ranking in document.RootElement.EnumerateArray())
                {
                    item.Rankings.Add(new DonationRankingDto
                    {
                        Rank = ranking.TryGetProperty("rank", out var rankProp) ? rankProp.GetInt32() : 0,
                        SessionPlayerId = ranking.TryGetProperty("session_player_id", out var sessionPlayerProp)
                            ? sessionPlayerProp.GetGuid()
                            : Guid.Empty,
                        PlayerIndex = ranking.TryGetProperty("player_index", out var playerIndexProp) &&
                                      playerIndexProp.ValueKind == JsonValueKind.Number
                            ? playerIndexProp.GetInt32()
                            : 0,
                        TotalDonasi = ranking.TryGetProperty("total_donasi", out var totalDonasiProp) &&
                                      totalDonasiProp.ValueKind == JsonValueKind.Number
                            ? totalDonasiProp.GetInt32()
                            : 0
                    });
                }
            }

            results.Add(item);
        }

        return results;
    }

    private static async Task DeleteSnapshotChildrenAsync(
        NpgsqlConnection conn,
        NpgsqlTransaction tx,
        Guid sessionId,
        CancellationToken ct)
    {
        const string sql = """
            delete from session_donation_events
            where session_id = @sessionId;

            delete from session_player_ingredients
            where session_player_id in (
                select session_player_id
                from session_players
                where session_id = @sessionId
            );

            delete from session_player_needs
            where session_player_id in (
                select session_player_id
                from session_players
                where session_id = @sessionId
            );

            delete from session_player_financial_goals
            where session_player_id in (
                select session_player_id
                from session_players
                where session_id = @sessionId
            );

            delete from session_player_collection_missions
            where session_player_id in (
                select session_player_id
                from session_players
                where session_id = @sessionId
            );

            delete from session_player_quest_progress
            where session_player_id in (
                select session_player_id
                from session_players
                where session_id = @sessionId
            );

            delete from session_player_action_counters
            where session_player_id in (
                select session_player_id
                from session_players
                where session_id = @sessionId
            );
            """;

        await conn.ExecuteAsync(new CommandDefinition(sql, new { sessionId }, tx, cancellationToken: ct));
    }

    private static async Task SavePlayerSnapshotAsync(
        NpgsqlConnection conn,
        NpgsqlTransaction tx,
        Guid sessionId,
        SessionPlayerStateDto player,
        CancellationToken ct)
    {
        const string upsertPlayerStateSql = """
            insert into session_player_states (session_player_id, coins, happiness, saving, created_at, updated_at)
            values (@sessionPlayerId, @coins, @happiness, @saving, now(), now())
            on conflict (session_player_id) do update
            set coins = excluded.coins,
                happiness = excluded.happiness,
                saving = excluded.saving,
                updated_at = now()
            """;

        const string updatePlayerNameSql = """
            update app_users u
            set display_name = @name
            from session_players sp
            where sp.user_id = u.user_id
              and sp.session_id = @sessionId
              and sp.session_player_id = @sessionPlayerId
            """;

        await conn.ExecuteAsync(new CommandDefinition(
            upsertPlayerStateSql,
            new
            {
                sessionPlayerId = player.SessionPlayerId,
                coins = player.Coins,
                happiness = player.Happiness,
                saving = player.Saving
            },
            tx,
            cancellationToken: ct));

        await conn.ExecuteAsync(new CommandDefinition(
            updatePlayerNameSql,
            new
            {
                sessionId,
                sessionPlayerId = player.SessionPlayerId,
                name = player.Name.Trim()
            },
            tx,
            cancellationToken: ct));

        const string insertBahanSql = """
            insert into session_player_ingredients (session_player_id, ingredient_id, qty, updated_at)
            select @sessionPlayerId, i.ingredient_id, @jumlah, now()
            from ingredients i
            where lower(i.display_name) = lower(@nama)
            """;

        foreach (var item in player.Bahan)
        {
            await conn.ExecuteAsync(new CommandDefinition(
                insertBahanSql,
                new { sessionPlayerId = player.SessionPlayerId, nama = item.Nama, jumlah = item.Jumlah },
                tx,
                cancellationToken: ct));
        }

        const string insertKebutuhanSql = """
            insert into session_player_needs (
                session_player_need_id, session_player_id, ruleset_catalog_item_id, sort_order, paid_amount, happiness_delta, purchased_at_day, created_at
            )
            select
                @entryId,
                @sessionPlayerId,
                rci.ruleset_catalog_item_id,
                @sortOrder,
                coalesce((rci.payload_json->>'hargaBeli')::int, 0),
                coalesce((rci.payload_json->>'poinKebahagiaan')::int, 0),
                @purchasedAtDay,
                now()
            from ruleset_catalog_items rci
            where rci.ruleset_version_id = (
                select sra.ruleset_version_id
                from session_ruleset_activations sra
                where sra.session_id = @sessionId
                order by sra.activated_at desc
                limit 1
            )
              and rci.item_type = 'NEED'
              and lower(rci.item_name) = lower(@nama)
              and lower(coalesce(rci.payload_json->>'tipe', '')) = lower(@tipe)
            limit 1
            """;

        for (var i = 0; i < player.Kebutuhan.Count; i++)
        {
            var item = player.Kebutuhan[i];
            await conn.ExecuteAsync(new CommandDefinition(
                insertKebutuhanSql,
                new
                {
                    entryId = Guid.NewGuid(),
                    sessionId,
                    sessionPlayerId = player.SessionPlayerId,
                    nama = item.Nama,
                    tipe = item.Tipe,
                    sortOrder = i + 1,
                    purchasedAtDay = 1
                },
                tx,
                cancellationToken: ct));
        }

        const string insertTujuanSql = """
            insert into session_player_financial_goals (session_player_id, ruleset_catalog_item_id, purchased_at_day, created_at)
            select @sessionPlayerId, rci.ruleset_catalog_item_id, @purchasedAtDay, now()
            from ruleset_catalog_items rci
            where rci.ruleset_version_id = (
                select sra.ruleset_version_id
                from session_ruleset_activations sra
                where sra.session_id = @sessionId
                order by sra.activated_at desc
                limit 1
            )
              and rci.item_type = 'FINANCIAL_GOAL'
              and lower(rci.item_name) = lower(@nama)
            limit 1
            """;

        foreach (var item in player.TujuanFinansial)
        {
            await conn.ExecuteAsync(new CommandDefinition(
                insertTujuanSql,
                new { sessionId, sessionPlayerId = player.SessionPlayerId, nama = item.Nama, purchasedAtDay = item.PurchasedAtDay },
                tx,
                cancellationToken: ct));
        }

        const string insertTargetSql = """
            insert into session_player_collection_missions (
                session_player_id, ruleset_catalog_item_id, is_completed, is_failed, reward_applied, assigned_at
            )
            select
                @sessionPlayerId,
                rci.ruleset_catalog_item_id,
                @isCompleted,
                @isFailed,
                @rewardApplied,
                now()
            from ruleset_catalog_items rci
            where rci.ruleset_version_id = (
                select sra.ruleset_version_id
                from session_ruleset_activations sra
                where sra.session_id = @sessionId
                order by sra.activated_at desc
                limit 1
            )
              and rci.item_type = 'COLLECTION_MISSION'
              and lower(rci.item_code) = lower(@id)
            limit 1
            """;

        foreach (var item in player.TargetKebutuhan)
        {
            await conn.ExecuteAsync(new CommandDefinition(
                insertTargetSql,
                new
                {
                    sessionId,
                    sessionPlayerId = player.SessionPlayerId,
                    id = item.Id,
                    isCompleted = item.IsCompleted,
                    isFailed = item.IsFailed,
                    rewardApplied = item.RewardApplied
                },
                tx,
                cancellationToken: ct));
        }

        const string insertQuestSql = """
            insert into session_player_quest_progress (
                session_player_id, quest_id, progress, target, is_completed, is_reward_claimed
            )
            values (@sessionPlayerId, @id, @progress, @target, @isCompleted, @isRewardClaimed)
            """;

        foreach (var item in player.QuestProgress)
        {
            await conn.ExecuteAsync(new CommandDefinition(
                insertQuestSql,
                new
                {
                    sessionPlayerId = player.SessionPlayerId,
                    id = item.Id,
                    progress = item.Progress,
                    target = item.Target,
                    isCompleted = item.IsCompleted,
                    isRewardClaimed = item.IsRewardClaimed
                },
                tx,
                cancellationToken: ct));
        }

        const string insertCounterSql = """
            insert into session_player_action_counters (session_player_id, action_id, count)
            values (@sessionPlayerId, @aksi, @count)
            """;

        foreach (var item in player.ActionCounters)
        {
            await conn.ExecuteAsync(new CommandDefinition(
                insertCounterSql,
                new { sessionPlayerId = player.SessionPlayerId, aksi = item.Aksi, count = item.Count },
                tx,
                cancellationToken: ct));
        }

        const string upsertDonationTotalSql = """
            insert into session_player_peduli_donasi (session_player_id, total_donasi)
            values (@sessionPlayerId, @totalDonasi)
            on conflict (session_player_id) do update
            set total_donasi = excluded.total_donasi
            """;

        await conn.ExecuteAsync(new CommandDefinition(
            upsertDonationTotalSql,
            new { sessionPlayerId = player.SessionPlayerId, totalDonasi = player.TotalDonasi },
            tx,
            cancellationToken: ct));
    }

    private static async Task SaveDonationEventAsync(
        NpgsqlConnection conn,
        NpgsqlTransaction tx,
        Guid sessionId,
        DonationEventDto donationEvent,
        CancellationToken ct)
    {
        var eventId = Guid.NewGuid();
        var rankingsJson = JsonSerializer.Serialize(donationEvent.Rankings.Select(ranking => new
        {
            rank = ranking.Rank,
            session_player_id = ranking.SessionPlayerId,
            player_index = ranking.PlayerIndex,
            total_donasi = ranking.TotalDonasi
        }));

        const string insertEventSql = """
            insert into session_donation_events (donation_event_id, session_id, event_ke, day, rankings_json, created_at)
            values (@eventId, @sessionId, @eventKe, @day, @rankingsJson::jsonb, now())
            """;

        await conn.ExecuteAsync(new CommandDefinition(
            insertEventSql,
            new
            {
                eventId,
                sessionId,
                eventKe = donationEvent.EventKe,
                day = donationEvent.Day,
                rankingsJson
            },
            tx,
            cancellationToken: ct));
    }

    private static async Task SaveLastActionAsync(
        NpgsqlConnection conn,
        NpgsqlTransaction tx,
        Guid sessionId,
        long newVersion,
        SaveSessionStateRequest request,
        CancellationToken ct)
    {
        if (!TryGetJson(request.LastAction, out var actionJson))
        {
            return;
        }

        string? aksi = null;
        Guid? sessionPlayerId = null;
        try
        {
            using var document = JsonDocument.Parse(actionJson);
            var root = document.RootElement;
            if (root.TryGetProperty("aksi", out var aksiProp) && aksiProp.ValueKind == JsonValueKind.String)
            {
                aksi = aksiProp.GetString();
            }

            if (root.TryGetProperty("session_player_id", out var sessionPlayerProp) &&
                sessionPlayerProp.ValueKind == JsonValueKind.String &&
                Guid.TryParse(sessionPlayerProp.GetString(), out var parsedSessionPlayerId))
            {
                sessionPlayerId = parsedSessionPlayerId;
            }
            else if (root.TryGetProperty("player_index", out var playerIndexProp) &&
                     playerIndexProp.ValueKind == JsonValueKind.Number &&
                     playerIndexProp.TryGetInt32(out var playerIndex))
            {
                sessionPlayerId = request.Players?
                    .FirstOrDefault(player => player.PlayerIndex == playerIndex)
                    ?.SessionPlayerId;
            }
        }
        catch (JsonException)
        {
            return;
        }

        const string insertSql = """
            insert into session_action_logs (
                action_log_id, session_id, session_player_id, state_version, action_id, action_json, created_at
            )
            values (@actionLogId, @sessionId, @sessionPlayerId, @stateVersion, @aksi, @actionJson::jsonb, now())
            """;

        await conn.ExecuteAsync(new CommandDefinition(
            insertSql,
            new
            {
                actionLogId = Guid.NewGuid(),
                sessionId,
                sessionPlayerId,
                stateVersion = newVersion,
                aksi,
                actionJson
            },
            tx,
            cancellationToken: ct));
    }

    private static string GetJsonOrEmpty(JsonElement? element)
    {
        return TryGetJson(element, out var json) ? json : "{}";
    }

    private static string ResolveWeekdayCode(int day)
    {
        var normalized = ((day - 1) % 7 + 7) % 7;
        return normalized switch
        {
            0 => "MON",
            1 => "TUE",
            2 => "WED",
            3 => "THU",
            4 => "FRI",
            5 => "SAT",
            _ => "SUN"
        };
    }

    private static string ResolvePhase(string weekday, bool isGameOver)
    {
        if (isGameOver)
        {
            return "GAME_END";
        }

        return weekday switch
        {
            "FRI" => "DONATION_DAY",
            "SAT" => "GOLD_INVESTMENT_DAY",
            "SUN" => "DAY_END",
            _ => "PLAYER_TURN"
        };
    }

    private static int ResolveCurrentActionIndex(int movesLeft)
    {
        return Math.Clamp(3 - Math.Max(0, movesLeft), 1, 2);
    }

    private static string BuildSyntheticPlayerUsername(Guid sessionId, int joinOrder)
    {
        return $"dev-player-{sessionId:N}-{joinOrder}";
    }

    private static bool TryGetJson(JsonElement? element, out string json)
    {
        json = string.Empty;
        if (!element.HasValue || element.Value.ValueKind is JsonValueKind.Undefined or JsonValueKind.Null)
        {
            return false;
        }

        json = element.Value.GetRawText();
        return true;
    }

    private static JsonElement ParseJsonOrEmpty(string? json)
    {
        using var document = JsonDocument.Parse(string.IsNullOrWhiteSpace(json) ? "{}" : json);
        return document.RootElement.Clone();
    }

    private static async Task<RequestedRulesetVersionRow?> ResolveRequestedRulesetVersionAsync(
        NpgsqlConnection conn,
        string mode,
        Guid? rulesetId,
        Guid? instructorUserId,
        CancellationToken ct)
    {
        const string sql = """
            with ranked_versions as (
                select
                    rv.ruleset_id,
                    rv.ruleset_version_id,
                    rv.mode,
                    rv.version,
                    rv.config_json,
                    row_number() over (
                        partition by rv.ruleset_id
                        order by
                            case when rv.status = 'ACTIVE' then 0 else 1 end,
                            rv.version desc
                    ) as rn
                from ruleset_versions rv
                where upper(rv.mode) = @mode
            )
            select
                r.ruleset_id,
                rv.ruleset_version_id,
                upper(rv.mode) as mode,
                coalesce(rv.config_json::text, '') as config_json
            from rulesets r
            join ranked_versions rv on rv.ruleset_id = r.ruleset_id and rv.rn = 1
            where (
                    @rulesetId is not null
                    and r.ruleset_id = @rulesetId
                    and (
                        r.instructor_user_id is null
                        or (@instructorUserId is not null and r.instructor_user_id = @instructorUserId)
                    )
                )
                or (
                    @rulesetId is null
                    and lower(coalesce(r.created_by, '')) like 'system-seed-relational%'
                    and r.instructor_user_id is null
                )
            order by case when r.instructor_user_id is null then 0 else 1 end,
                     r.created_at desc
            limit 1
            """;

        return await conn.QuerySingleOrDefaultAsync<RequestedRulesetVersionRow>(
            new CommandDefinition(
                sql,
                new
                {
                    mode,
                    rulesetId,
                    instructorUserId
                },
                cancellationToken: ct));
    }

    private static async Task<Guid?> ResolveDefaultRelationalRulesetVersionIdAsync(
        NpgsqlConnection conn,
        string mode,
        CancellationToken ct)
    {
        const string sql = """
            with ranked_defaults as (
                select
                    rv.ruleset_id,
                    rv.ruleset_version_id,
                    rv.mode,
                    rv.version,
                    row_number() over (
                        partition by rv.ruleset_id
                        order by
                            case when rv.status = 'ACTIVE' then 0 else 1 end,
                            rv.version desc
                    ) as rn
                from ruleset_versions rv
                where upper(rv.mode) = @mode
            )
            select rv.ruleset_version_id
            from rulesets r
            join ranked_defaults rv on rv.ruleset_id = r.ruleset_id and rv.rn = 1
            where lower(coalesce(r.created_by, '')) like 'system-seed-relational%'
            order by rv.version desc, r.created_at desc
            limit 1
            """;

        return await conn.ExecuteScalarAsync<Guid?>(
            new CommandDefinition(sql, new { mode }, cancellationToken: ct));
    }

    private sealed class SessionStateRow
    {
        public Guid SessionId { get; init; }
        public long StateVersion { get; init; }
        public int Day { get; init; }
        public int Turn { get; init; }
        public int MovesLeft { get; init; }
        public int FinishDay { get; init; }
        public bool IsGameOver { get; init; }
        public string UiStateJson { get; init; } = "{}";
    }

    private sealed class SessionPlayerStateRow
    {
        public Guid SessionPlayerId { get; init; }
        public Guid UserId { get; init; }
        public int PlayerIndex { get; init; }
        public string Name { get; init; } = string.Empty;
        public int Coins { get; init; }
        public int Happiness { get; init; }
        public int Saving { get; init; }
        public int TotalDonasi { get; init; }
    }

    private sealed class BahanRow
    {
        public Guid SessionPlayerId { get; init; }
        public string Nama { get; init; } = string.Empty;
        public int Jumlah { get; init; }
    }

    private sealed class KebutuhanRow
    {
        public Guid SessionPlayerId { get; init; }
        public string Nama { get; init; } = string.Empty;
        public string Tipe { get; init; } = string.Empty;
    }

    private sealed class TujuanFinansialRow
    {
        public Guid SessionPlayerId { get; init; }
        public string Nama { get; init; } = string.Empty;
        public int PurchasedAtDay { get; init; }
    }

    private sealed class TargetKebutuhanRow
    {
        public Guid SessionPlayerId { get; init; }
        public string Id { get; init; } = string.Empty;
        public bool IsCompleted { get; init; }
        public bool IsFailed { get; init; }
        public bool RewardApplied { get; init; }
    }

    private sealed class QuestProgressRow
    {
        public Guid SessionPlayerId { get; init; }
        public string Id { get; init; } = string.Empty;
        public int Progress { get; init; }
        public int Target { get; init; }
        public bool IsCompleted { get; init; }
        public bool IsRewardClaimed { get; init; }
    }

    private sealed class ActionCounterRow
    {
        public Guid SessionPlayerId { get; init; }
        public string Aksi { get; init; } = string.Empty;
        public int Count { get; init; }
    }

    private sealed class DonationEventRow
    {
        public Guid EventId { get; init; }
        public int EventKe { get; init; }
        public int Day { get; init; }
        public string RankingsJson { get; init; } = "[]";
    }

    private sealed class RequestedRulesetVersionRow
    {
        public Guid RulesetId { get; init; }
        public Guid RulesetVersionId { get; init; }
        public string Mode { get; init; } = string.Empty;
        public string ConfigJson { get; init; } = "{}";
    }

    private sealed class CollectionMissionSectionRow
    {
        public Guid CollectionMissionCardId { get; init; }
        public string MissionCode { get; init; } = string.Empty;
        public string MissionName { get; init; } = string.Empty;
    }

}

public sealed class RulesetSectionDb
{
    public Guid RulesetId { get; init; }
    public Guid RulesetVersionId { get; init; }
    public string Mode { get; init; } = string.Empty;
    public string ConfigJson { get; init; } = string.Empty;
}

public sealed record CreateSessionWithStateResult(
    Guid SessionId,
    Guid RulesetId,
    Guid RulesetVersionId,
    SessionStateResponse State);

public enum SaveSessionStateStatus
{
    Saved,
    NotFound,
    Stale
}

public sealed record SaveSessionStateResult(
    SaveSessionStateStatus Status,
    SessionStateResponse? State,
    long? CurrentVersion)
{
    public static SaveSessionStateResult Saved(SessionStateResponse state) =>
        new(SaveSessionStateStatus.Saved, state, state.StateVersion);

    public static SaveSessionStateResult NotFound() =>
        new(SaveSessionStateStatus.NotFound, null, null);

    public static SaveSessionStateResult Stale(long currentVersion) =>
        new(SaveSessionStateStatus.Stale, null, currentVersion);
}

public sealed class RulesetSectionCatalog
{
    private RulesetSectionCatalog(
        JsonElement gameConfig,
        JsonElement bahan,
        JsonElement resep,
        JsonElement kebutuhan,
        JsonElement targetKebutuhan,
        JsonElement tujuanFinansial,
        JsonElement narasi,
        JsonElement quest,
        RulesetSectionGameConfig gameConfigValues,
        HashSet<string> bahanNames,
        Dictionary<string, string> kebutuhanTypes,
        HashSet<string> targetKebutuhanIds,
        HashSet<string> tujuanFinansialNames,
        Dictionary<string, int> questTargets,
        HashSet<string> actions)
    {
        GameConfig = gameConfig;
        Bahan = bahan;
        Resep = resep;
        Kebutuhan = kebutuhan;
        TargetKebutuhan = targetKebutuhan;
        TujuanFinansial = tujuanFinansial;
        Narasi = narasi;
        Quest = quest;
        GameConfigValues = gameConfigValues;
        BahanNames = bahanNames;
        KebutuhanTypes = kebutuhanTypes;
        TargetKebutuhanIds = targetKebutuhanIds;
        TujuanFinansialNames = tujuanFinansialNames;
        QuestTargets = questTargets;
        Actions = actions;
    }

    public JsonElement GameConfig { get; }
    public JsonElement Bahan { get; }
    public JsonElement Resep { get; }
    public JsonElement Kebutuhan { get; }
    public JsonElement TargetKebutuhan { get; }
    public JsonElement TujuanFinansial { get; }
    public JsonElement Narasi { get; }
    public JsonElement Quest { get; }
    public RulesetSectionGameConfig GameConfigValues { get; }
    public HashSet<string> BahanNames { get; }
    public Dictionary<string, string> KebutuhanTypes { get; }
    public HashSet<string> TargetKebutuhanIds { get; }
    public HashSet<string> TujuanFinansialNames { get; }
    public Dictionary<string, int> QuestTargets { get; }
    public HashSet<string> Actions { get; }

    public static RulesetSectionCatalog FromConfig(string configJson)
    {
        using var document = JsonDocument.Parse(configJson);
        var componentCatalog = document.RootElement.GetProperty("component_catalog");
        var gameConfig = componentCatalog.GetProperty("gameConfig").Clone();
        var gameConfigValues = new RulesetSectionGameConfig(
            ReadInt(gameConfig, "initialCoins", 20),
            ReadInt(gameConfig, "initialHappiness", 0),
            ReadInt(gameConfig, "initialSaving", 0),
            ReadInt(gameConfig, "actionsPerTurn", 2),
            ReadInt(gameConfig, "finishDay", 13),
            ReadInt(gameConfig, "minPlayers", 2),
            ReadInt(gameConfig, "maxPlayers", 4));
        var bahan = componentCatalog.GetProperty("bahan").Clone();
        var resep = componentCatalog.GetProperty("resep").Clone();
        var kebutuhan = componentCatalog.GetProperty("kebutuhan").Clone();
        var targetKebutuhan = componentCatalog.GetProperty("targetKebutuhan").Clone();
        var tujuanFinansial = componentCatalog.GetProperty("tujuanFinansial").Clone();
        var narasi = componentCatalog.GetProperty("narasi").Clone();
        var quest = componentCatalog.GetProperty("quest").Clone();

        var bahanNames = ReadStringSet(bahan, "nama");
        var kebutuhanTypes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var item in kebutuhan.EnumerateArray())
        {
            var nama = item.GetProperty("nama").GetString();
            var tipe = item.GetProperty("tipe").GetString();
            if (!string.IsNullOrWhiteSpace(nama) && !string.IsNullOrWhiteSpace(tipe))
            {
                kebutuhanTypes[nama] = tipe;
            }
        }

        var targetKebutuhanIds = ReadStringSet(targetKebutuhan, "id");
        var tujuanFinansialNames = ReadStringSet(tujuanFinansial, "nama");
        var questTargets = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var actions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "BahanMasakan",
            "JualMasakan",
            "Kebutuhan",
            "KerjaLepas",
            "TujuanFinansial",
            "Menabung",
            "JumatBerkah"
        };

        foreach (var item in quest.EnumerateArray())
        {
            var id = item.GetProperty("id").GetString();
            if (!string.IsNullOrWhiteSpace(id))
            {
                questTargets[id] = item.TryGetProperty("target", out var targetProp) && targetProp.TryGetInt32(out var target)
                    ? target
                    : 0;
            }

            if (item.TryGetProperty("aksi", out var aksiProp) && aksiProp.ValueKind == JsonValueKind.String)
            {
                var aksi = aksiProp.GetString();
                if (!string.IsNullOrWhiteSpace(aksi))
                {
                    actions.Add(aksi);
                }
            }
        }

        foreach (var item in narasi.EnumerateArray())
        {
            if (!item.TryGetProperty("prerequisiteAksi", out var prerequisites) ||
                prerequisites.ValueKind != JsonValueKind.Array)
            {
                continue;
            }

            foreach (var prerequisite in prerequisites.EnumerateArray())
            {
                if (prerequisite.TryGetProperty("aksi", out var aksiProp) && aksiProp.ValueKind == JsonValueKind.String)
                {
                    var aksi = aksiProp.GetString();
                    if (!string.IsNullOrWhiteSpace(aksi))
                    {
                        actions.Add(aksi);
                    }
                }
            }
        }

        return new RulesetSectionCatalog(
            gameConfig,
            bahan,
            resep,
            kebutuhan,
            targetKebutuhan,
            tujuanFinansial,
            narasi,
            quest,
            gameConfigValues,
            bahanNames,
            kebutuhanTypes,
            targetKebutuhanIds,
            tujuanFinansialNames,
            questTargets,
            actions);
    }

    private static int ReadInt(JsonElement root, string propertyName, int defaultValue)
    {
        return root.TryGetProperty(propertyName, out var property) &&
               property.ValueKind == JsonValueKind.Number &&
               property.TryGetInt32(out var value)
            ? value
            : defaultValue;
    }

    private static HashSet<string> ReadStringSet(JsonElement array, string propertyName)
    {
        var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var item in array.EnumerateArray())
        {
            if (item.TryGetProperty(propertyName, out var property) && property.ValueKind == JsonValueKind.String)
            {
                var value = property.GetString();
                if (!string.IsNullOrWhiteSpace(value))
                {
                    result.Add(value);
                }
            }
        }

        return result;
    }
}

public sealed record RulesetSectionGameConfig(
    int InitialCoins,
    int InitialHappiness,
    int InitialSaving,
    int ActionsPerTurn,
    int FinishDay,
    int MinPlayers,
    int MaxPlayers);
