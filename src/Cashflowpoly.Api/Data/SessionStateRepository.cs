using System.Security.Cryptography;
using System.Text.Json;
using Cashflowpoly.Api.Contracts;
using Cashflowpoly.Api.Domain;
using Dapper;
using Npgsql;

namespace Cashflowpoly.Api.Data;

public sealed class SessionStateRepository
{
    private readonly NpgsqlDataSource _dataSource;
    private readonly RulesetRepository _rulesets;
    private readonly EventRepository _events;
    private readonly IHappinessCalculator _happiness;

    public SessionStateRepository(
        NpgsqlDataSource dataSource,
        RulesetRepository rulesets,
        EventRepository events,
        IHappinessCalculator happiness)
    {
        _dataSource = dataSource;
        _rulesets = rulesets;
        _events = events;
        _happiness = happiness;
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

        var definition = await _rulesets.GetRulesetDefinitionAsync(requested.RulesetVersionId, ct);
        if (definition is null)
        {
            return null;
        }

        return new RulesetSectionDb
        {
            RulesetId = requested.RulesetId,
            RulesetVersionId = requested.RulesetVersionId,
            Mode = requested.Mode,
            Definition = definition
        };
    }

    public async Task<RulesetSectionDb?> GetRulesetSectionByVersionIdAsync(
        string mode,
        Guid rulesetVersionId,
        Guid? instructorUserId,
        CancellationToken ct)
    {
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        var requested = await ResolveRulesetVersionByIdAsync(
            conn,
            mode.ToUpperInvariant(),
            rulesetVersionId,
            instructorUserId,
            ct);
        if (requested is null)
        {
            return null;
        }

        var definition = await _rulesets.GetRulesetDefinitionAsync(requested.RulesetVersionId, ct);
        if (definition is null)
        {
            return null;
        }

        return new RulesetSectionDb
        {
            RulesetId = requested.RulesetId,
            RulesetVersionId = requested.RulesetVersionId,
            Mode = requested.Mode,
            Definition = definition
        };
    }

    public async Task<CreateSessionWithStateResult?> CreateSessionAsync(
        string sessionName,
        string mode,
        IReadOnlyList<string> playerNames,
        Guid rulesetVersionId,
        Guid instructorUserId,
        string? createdBy,
        CancellationToken ct)
    {
        var ruleset = await GetRulesetSectionByVersionIdAsync(mode, rulesetVersionId, instructorUserId, ct);
        if (ruleset is null)
        {
            return null;
        }

        var catalog = RulesetSectionCatalog.FromDefinition(ruleset.Definition);
        var gameConfig = catalog.GameConfigValues;
        var sessionId = Guid.NewGuid();
        var now = DateTimeOffset.UtcNow;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        await using var tx = await conn.BeginTransactionAsync(ct);

        const string insertSessionSql = """
            insert into sessions (session_id, session_name, mode, status, started_at, ended_at, instructor_user_id, ruleset_version_id, created_at)
            values (@sessionId, @sessionName, @mode, 'CREATED', null, null, @instructorUserId, @rulesetVersionId, @createdAt)
            """;

        await conn.ExecuteAsync(new CommandDefinition(
            insertSessionSql,
            new
            {
                sessionId,
                sessionName,
                mode = mode.ToUpperInvariant(),
                instructorUserId,
                rulesetVersionId = ruleset.RulesetVersionId,
                createdAt = now
            },
            tx,
            cancellationToken: ct));

        const string insertSessionStateSql = """
            insert into session_states (
                session_id, day, weekday, turn_number, action_slot, current_session_player_id, current_action_slot,
                action_slots_left, finish_day, phase, is_game_over, state_version, created_at, updated_at
            )
            values (@sessionId, 1, @weekday, 0, 1, null, 1, @actionSlotsLeft, @finishDay, 'PLAYER_TURN', false, 1, @createdAt, @createdAt)
            """;

        await conn.ExecuteAsync(new CommandDefinition(
            insertSessionStateSql,
            new
            {
                sessionId,
                weekday = ResolveWeekdayCode(1),
                actionSlotsLeft = gameConfig.ActionsPerTurn,
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
            insert into session_participants (session_participant_id, session_id, user_id, player_order_no, player_name, joined_at)
            values (@sessionPlayerId, @sessionId, @userId, @playerOrder, @playerName, @createdAt)
            """;

        var setupPlayers = new List<SetupParticipant>(playerNames.Count);
        for (var index = 0; index < playerNames.Count; index++)
        {
            var userId = Guid.NewGuid();
            var sessionPlayerId = Guid.NewGuid();
            var playerName = playerNames[index].Trim();
            var playerOrder = index + 1;
            var username = BuildSyntheticPlayerUsername(sessionId, playerOrder);
            var password = $"dev-only-{sessionId:N}-{playerOrder}";

            await conn.ExecuteAsync(new CommandDefinition(
                insertPlayerSql,
                new { userId, username, displayName = playerName, password, createdAt = now },
                tx,
                cancellationToken: ct));

            await conn.ExecuteAsync(new CommandDefinition(
                insertSessionPlayerSql,
                new { sessionPlayerId, sessionId, userId, playerOrder, playerName, createdAt = now },
                tx,
                cancellationToken: ct));

            setupPlayers.Add(new SetupParticipant(sessionPlayerId, userId, playerOrder));
        }

        var setup = await InitializeSetupAsync(
            conn,
            tx,
            sessionId,
            ruleset.RulesetVersionId,
            mode,
            ruleset.Definition,
            setupPlayers,
            now,
            ct);

        await conn.ExecuteAsync(new CommandDefinition(
            """
            update sessions
            set status = 'STARTED',
                started_at = @startedAt
            where session_id = @sessionId;

            update session_states
            set day = 1,
                weekday = 'MON',
                turn_number = 1,
                action_slot = 1,
                current_session_player_id = @firstPlayerId,
                current_action_slot = 1,
                action_slots_left = @actionsPerTurn,
                phase = 'PLAYER_TURN',
                is_game_over = false,
                state_version = 1,
                updated_at = @startedAt
            where session_id = @sessionId;
            """,
            new
            {
                sessionId,
                startedAt = now,
                firstPlayerId = setup.FirstPlayerId,
                actionsPerTurn = gameConfig.ActionsPerTurn
            },
            tx,
            cancellationToken: ct));

        await tx.CommitAsync(ct);

        var state = await GetStateAsync(sessionId, ct);
        if (state is null)
        {
            throw new InvalidOperationException("Session state gagal dibuat.");
        }

        state.NextSequenceNumber = setup.NextSequenceNumber;
        return new CreateSessionWithStateResult(sessionId, ruleset.RulesetId, ruleset.RulesetVersionId, state);
    }

    private static async Task<(long NextSequenceNumber, Guid FirstPlayerId)> InitializeSetupAsync(
        NpgsqlConnection conn,
        NpgsqlTransaction tx,
        Guid sessionId,
        Guid rulesetVersionId,
        string mode,
        RulesetDefinitionDto definition,
        IReadOnlyList<SetupParticipant> players,
        DateTimeOffset timestamp,
        CancellationToken ct)
    {
        var orderedPlayers = players.OrderBy(item => item.PlayerOrder).ToList();
        if (orderedPlayers.Count == 0)
        {
            throw new InvalidOperationException("Session wajib memiliki pemain sebelum setup dijalankan.");
        }

        var ingredients = definition.Ingredients
            .Where(item => !string.IsNullOrWhiteSpace(item.Id))
            .DistinctBy(item => item.Id, StringComparer.OrdinalIgnoreCase)
            .ToList();
        var orders = definition.Orders.Where(item => !string.IsNullOrWhiteSpace(item.Id)).ToList();
        var primaryNeeds = definition.Needs
            .Where(item => !string.IsNullOrWhiteSpace(item.Id) &&
                           item.Tipe.Equals("primer", StringComparison.OrdinalIgnoreCase))
            .DistinctBy(item => item.Id, StringComparer.OrdinalIgnoreCase)
            .ToList();
        var missions = definition.CollectionMissions
            .Where(item => !string.IsNullOrWhiteSpace(item.Id))
            .DistinctBy(item => item.Id, StringComparer.OrdinalIgnoreCase)
            .ToList();
        var tieBreakers = definition.TieBreakers
            .Where(item => item.TieNumber >= 1)
            .ToList();
        var duplicateTieNumber = tieBreakers
            .GroupBy(item => item.TieNumber)
            .FirstOrDefault(group => group.Count() > 1);
        var tieBreakersByNumber = duplicateTieNumber is null
            ? tieBreakers.ToDictionary(item => item.TieNumber)
            : [];
        var orderDeck = orders
            .SelectMany(item => Enumerable.Repeat(item, Math.Max(1, item.CardQty ?? 1)))
            .ToList();

        if (ingredients.Count < 3)
        {
            throw new InvalidOperationException("Ruleset membutuhkan minimal 3 jenis bahan untuk mengisi 5 slot pasar tanpa lebih dari 2 kartu sejenis.");
        }

        if (orderDeck.Count < 5)
        {
            throw new InvalidOperationException("Ruleset membutuhkan minimal 5 kartu pesanan untuk pasar awal.");
        }

        if (primaryNeeds.Count < 5)
        {
            throw new InvalidOperationException("Ruleset membutuhkan minimal 5 Kartu Aneka Kebutuhan Primer untuk pasar awal.");
        }

        if (missions.Count < orderedPlayers.Count)
        {
            throw new InvalidOperationException("Jumlah Kartu Misi Koleksi tidak mencukupi untuk seluruh pemain.");
        }

        if (duplicateTieNumber is not null ||
            Enumerable.Range(1, orderedPlayers.Count).Any(number => !tieBreakersByNumber.ContainsKey(number)))
        {
            throw new InvalidOperationException($"Ruleset wajib memiliki Tie Breaker unik #1 sampai #{orderedPlayers.Count}.");
        }

        var tieBreakerDeck = Enumerable.Range(1, orderedPlayers.Count)
            .Select(number => tieBreakersByNumber[number])
            .ToList();
        Shuffle(tieBreakerDeck);
        var tieBreakerByPlayerId = orderedPlayers
            .Select((player, index) => new { player.SessionParticipantId, TieBreaker = tieBreakerDeck[index] })
            .ToDictionary(item => item.SessionParticipantId, item => item.TieBreaker);
        var firstPlayerId = tieBreakerByPlayerId.Single(item => item.Value.TieNumber == 1).Key;
        await ApplyTieBreakerTurnOrderAsync(conn, tx, sessionId, tieBreakerByPlayerId, ct);
        var ingredientDrawPile = ingredients.SelectMany(item => Enumerable.Repeat(item, 2)).ToList();
        Shuffle(ingredientDrawPile);
        Shuffle(missions);
        Shuffle(orderDeck);
        Shuffle(primaryNeeds);

        var loan = definition.ShariaLoans.FirstOrDefault();
        var insurance = definition.InsuranceProducts.FirstOrDefault();
        var isMahir = string.Equals(mode, "MAHIR", StringComparison.OrdinalIgnoreCase);
        if (isMahir && (loan is null || insurance is null))
        {
            throw new InvalidOperationException("Ruleset MAHIR wajib memiliki produk pinjaman dan asuransi.");
        }

        long sequence = 0;
        await InsertAndProjectSetupEventAsync(
            conn, tx, sessionId, rulesetVersionId, null, null, sequence++, "MulaiSesi",
            new { setup = "INITIAL" }, timestamp, ct);

        for (var index = 0; index < orderedPlayers.Count; index++)
        {
            var player = orderedPlayers[index];
            var tieBreaker = tieBreakerByPlayerId[player.SessionParticipantId];
            var ingredient = ingredientDrawPile[index];
            var mission = missions[index];
            var targetFamily = mission.KebutuhanTarget.FirstOrDefault(item =>
                    item.Type.Equals("FAMILY", StringComparison.OrdinalIgnoreCase) ||
                    item.Type.Equals("NEED_FAMILY", StringComparison.OrdinalIgnoreCase))
                ?.Value ?? mission.Nama;
            var requirePrimary = mission.KebutuhanTarget.Any(item =>
                (item.Type.Equals("TIER", StringComparison.OrdinalIgnoreCase) ||
                 item.Type.Equals("NEED_TIER", StringComparison.OrdinalIgnoreCase)) &&
                item.Value.Equals("primer", StringComparison.OrdinalIgnoreCase));
            var requireSecondary = mission.KebutuhanTarget.Any(item =>
                (item.Type.Equals("TIER", StringComparison.OrdinalIgnoreCase) ||
                 item.Type.Equals("NEED_TIER", StringComparison.OrdinalIgnoreCase)) &&
                item.Value.Equals("sekunder", StringComparison.OrdinalIgnoreCase));

            await InsertAndProjectSetupEventAsync(
                conn, tx, sessionId, rulesetVersionId, player.SessionParticipantId, player.UserId,
                sequence++, "BagikanTieBreaker",
                new { number = tieBreaker.TieNumber, card_code = tieBreaker.TieBreakerCode },
                timestamp, ct);
            await InsertAndProjectSetupEventAsync(
                conn, tx, sessionId, rulesetVersionId, player.SessionParticipantId, player.UserId,
                sequence++, "SetupBahanAwal",
                new { card_id = ingredient.Id, ingredient_name = ingredient.Nama, amount = ingredient.HargaBeli, setup = "INITIAL" },
                timestamp, ct);
            await InsertAndProjectSetupEventAsync(
                conn, tx, sessionId, rulesetVersionId, player.SessionParticipantId, player.UserId,
                sequence++, "SetupEmasAwal",
                new { qty = 1, asset_code = "gold_card", setup = "INITIAL" },
                timestamp, ct);
            await InsertAndProjectSetupEventAsync(
                conn, tx, sessionId, rulesetVersionId, player.SessionParticipantId, player.UserId,
                sequence++, "SetupMisiAwal",
                new
                {
                    mission_id = mission.Id,
                    target_tertiary_card_id = targetFamily,
                    penalty_points = mission.PenaltyPoints,
                    require_primary = requirePrimary,
                    require_secondary = requireSecondary,
                    setup = "INITIAL"
                },
                timestamp, ct);

            if (!isMahir)
            {
                continue;
            }

            var loanInstanceId = $"{loan!.LoanCode}:setup:{player.SessionParticipantId:N}";
            await InsertAndProjectSetupEventAsync(
                conn, tx, sessionId, rulesetVersionId, player.SessionParticipantId, player.UserId,
                sequence++, "SetupPinjamanAwal",
                new
                {
                    loan_code = loan.LoanCode,
                    loan_id = loanInstanceId,
                    principal = loan.Principal,
                    repayment_amount = loan.RepaymentAmount,
                    duration_days = loan.DurationDays,
                    penalty_points = loan.PenaltyPoints,
                    setup = "INITIAL"
                },
                timestamp, ct);
            await InsertAndProjectSetupEventAsync(
                conn, tx, sessionId, rulesetVersionId, player.SessionParticipantId, player.UserId,
                sequence++, "SetupAsuransiAwal",
                new
                {
                    product_code = insurance!.ProductCode,
                    policy_id = $"{insurance.ProductCode}:setup:{player.SessionParticipantId:N}",
                    premium = 0,
                    coverage_type = "MULTIRISK",
                    setup = "INITIAL"
                },
                timestamp, ct);
        }

        var marketGroups = new[]
        {
            (
                SlotGroup: "INGREDIENT_MARKET",
                AssetType: "INGREDIENT",
                Codes: ingredientDrawPile.Skip(orderedPlayers.Count).Concat(ingredientDrawPile).Take(5).Select(item => item.Id).ToList()),
            (
                SlotGroup: "ORDER_MARKET",
                AssetType: "ORDER",
                Codes: orderDeck.Take(5).Select(item => item.Id).ToList()),
            (
                SlotGroup: "NEED_MARKET",
                AssetType: "NEED",
                Codes: primaryNeeds.Take(5).Select(item => item.Id).ToList())
        };
        foreach (var market in marketGroups)
        {
            for (var index = 0; index < market.Codes.Count; index++)
            {
                await InsertAndProjectSetupEventAsync(
                    conn, tx, sessionId, rulesetVersionId, null, null,
                    sequence++, GameActionCatalog.CardDrawn,
                    new
                    {
                        setup = "INITIAL_MARKET",
                        slot_group = market.SlotGroup,
                        slot_code = $"SLOT_{index + 1}",
                        asset_type = market.AssetType,
                        asset_code = market.Codes[index]
                    },
                    timestamp, ct);
            }
        }

        return (sequence, firstPlayerId);
    }

    private static void Shuffle<T>(IList<T> items)
    {
        for (var index = items.Count - 1; index > 0; index--)
        {
            var swapIndex = RandomNumberGenerator.GetInt32(index + 1);
            (items[index], items[swapIndex]) = (items[swapIndex], items[index]);
        }
    }

    private static Task ApplyTieBreakerTurnOrderAsync(
        NpgsqlConnection conn,
        NpgsqlTransaction tx,
        Guid sessionId,
        IReadOnlyDictionary<Guid, RulesetTieBreakerDto> tieBreakerByPlayerId,
        CancellationToken ct)
    {
        return conn.ExecuteAsync(new CommandDefinition(
            """
            set constraints uq_session_participants_session_seat deferred;

            update session_participants participant
            set player_order_no = assignment.tie_number
            from unnest(@participantIds::uuid[], @tieNumbers::int[]) as assignment(session_participant_id, tie_number)
            where participant.session_id = @sessionId
              and participant.session_participant_id = assignment.session_participant_id;
            """,
            new
            {
                sessionId,
                participantIds = tieBreakerByPlayerId.Keys.ToArray(),
                tieNumbers = tieBreakerByPlayerId.Values.Select(item => item.TieNumber).ToArray()
            },
            tx,
            cancellationToken: ct));
    }

    private static Task InsertAndProjectSetupEventAsync(
        NpgsqlConnection conn,
        NpgsqlTransaction tx,
        Guid sessionId,
        Guid rulesetVersionId,
        Guid? sessionParticipantId,
        Guid? userId,
        long sequenceNumber,
        string actionType,
        object payload,
        DateTimeOffset timestamp,
        CancellationToken ct)
    {
        const string sql = """
            insert into events (
                event_pk, event_id, session_id, session_player_id, user_id, actor_type, "timestamp",
                day_index, weekday, turn_number, action_slot, sequence_number, ruleset_action_id,
                action_type, ruleset_version_id, payload_version, payload, received_at, client_request_id
            )
            select
                @eventPk, @eventId, @sessionId, @sessionParticipantId, @userId, 'SYSTEM', @eventTimestamp,
                0, 'MON', 0, 0, @sequenceNumber, ra.ruleset_action_id,
                @actionType, @rulesetVersionId, '1.0', @payload::jsonb, @eventTimestamp, @clientRequestId
            from ruleset_actions ra
            where ra.ruleset_version_id = @rulesetVersionId
              and lower(ra.action_id) = lower(@actionType)
              and ra.is_active
            limit 1;

            select project_session_event(@eventPk);
            """;

        var eventPk = Guid.NewGuid();
        return conn.ExecuteAsync(new CommandDefinition(
            sql,
            new
            {
                eventPk,
                eventId = Guid.NewGuid(),
                sessionId,
                sessionParticipantId,
                userId,
                eventTimestamp = timestamp.AddMilliseconds(sequenceNumber),
                sequenceNumber,
                actionType,
                rulesetVersionId,
                payload = JsonSerializer.Serialize(payload),
                clientRequestId = $"setup:{sequenceNumber}:{actionType}"
            },
            tx,
            cancellationToken: ct));
    }

    public async Task<long> StartSessionWithSetupAsync(
        Guid sessionId,
        string mode,
        Guid rulesetVersionId,
        RulesetDefinitionDto definition,
        DateTimeOffset startedAt,
        CancellationToken ct)
    {
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        await using var tx = await conn.BeginTransactionAsync(ct);

        var players = (await conn.QueryAsync<SetupParticipant>(new CommandDefinition(
            """
            select
                session_participant_id as SessionParticipantId,
                user_id as UserId,
                player_order_no as PlayerOrder
            from session_participants
            where session_id = @sessionId
            order by player_order_no
            """,
            new { sessionId },
            tx,
            cancellationToken: ct))).ToList();

        var setup = await InitializeSetupAsync(
            conn,
            tx,
            sessionId,
            rulesetVersionId,
            mode,
            definition,
            players,
            startedAt,
            ct);

        await conn.ExecuteAsync(new CommandDefinition(
            """
            update sessions
            set status = 'STARTED',
                started_at = @startedAt
            where session_id = @sessionId
              and status = 'CREATED';

            update session_states state
            set day = 1,
                weekday = 'MON',
                turn_number = 1,
                action_slot = 1,
                current_session_player_id = @firstPlayerId,
                current_action_slot = 1,
                action_slots_left = settings.actions_per_turn,
                phase = 'PLAYER_TURN',
                is_game_over = false,
                state_version = 1,
                updated_at = @startedAt
            from ruleset_game_settings settings
            where state.session_id = @sessionId
              and settings.ruleset_version_id = @rulesetVersionId;
            """,
            new
            {
                sessionId,
                rulesetVersionId,
                firstPlayerId = setup.FirstPlayerId,
                startedAt
            },
            tx,
            cancellationToken: ct));

        await tx.CommitAsync(ct);
        return setup.NextSequenceNumber;
    }

    public async Task<SessionStateResponse?> GetStateAsync(Guid sessionId, CancellationToken ct)
    {
        const string stateSql = """
            select
                session_id,
                state_version,
                day,
                action_slot as turn,
                action_slots_left,
                finish_day,
                is_game_over,
                (
                    select coalesce(max(e.sequence_number) + 1, 0)
                    from events e
                    where e.session_id = session_states.session_id
                ) as next_sequence_number,
                '{}'::jsonb::text as ui_state_json
            from session_states
            where session_id = @sessionId
            """;

        const string playersSql = """
            select
                sp.session_participant_id as session_player_id,
                sp.user_id,
                sp.player_order_no as player_index,
                coalesce(sp.player_name, u.display_name) as name,
                coalesce(ps.coins, 0) as coins,
                coalesce(ps.happiness, 0) as happiness,
                coalesce(ps.saving, 0) as saving,
                coalesce(ps.total_donasi, 0) as total_donasi
            from session_participants sp
            join app_users u on u.user_id = sp.user_id
            left join session_participant_balances ps on ps.session_participant_id = sp.session_participant_id
            where sp.session_id = @sessionId
            order by sp.player_order_no asc, sp.joined_at asc
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
            NextSequenceNumber = state.NextSequenceNumber,
            Day = state.Day,
            Turn = state.Turn,
            ActionSlotsLeft = state.ActionSlotsLeft,
            FinishDay = state.FinishDay,
            IsGameOver = state.IsGameOver,
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
                turn_number = coalesce((
                    select sp.player_order_no
                    from session_participants sp
                    where sp.session_id = @sessionId
                      and sp.session_participant_id = @currentSessionPlayerId
                ), 0),
                action_slot = @turn,
                current_session_player_id = @currentSessionPlayerId,
                current_action_slot = @currentActionSlot,
                action_slots_left = @actionSlotsLeft,
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
                currentActionSlot = ResolveCurrentActionSlot(request.ActionSlotsLeft),
                actionSlotsLeft = request.ActionSlotsLeft,
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

    public async Task ComputeFinalScoresAsync(Guid sessionId, CancellationToken ct)
    {
        const string activeRulesetSql = """
            select ruleset_version_id
            from sessions
            where session_id = @sessionId
            limit 1
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        var rulesetVersionId = await conn.QuerySingleOrDefaultAsync<Guid?>(
            new CommandDefinition(activeRulesetSql, new { sessionId }, cancellationToken: ct));
        if (!rulesetVersionId.HasValue)
        {
            throw new InvalidOperationException("Session belum memiliki ruleset aktif.");
        }

        var rulesetVersion = await _rulesets.GetRulesetVersionByIdAsync(rulesetVersionId.Value, ct);
        if (rulesetVersion?.Definition is null ||
            !RulesetRuntimeMapper.TryBuildConfig(rulesetVersion.Definition, out var config, out _))
        {
            throw new InvalidOperationException("Definition ruleset aktif tidak valid.");
        }

        var events = await _events.GetAllEventsBySessionAsync(sessionId, ct);
        var sourceEventId = events
            .OrderByDescending(item => item.SequenceNumber)
            .Select(item => (Guid?)item.EventId)
            .FirstOrDefault();
        var projections = await _events.GetCashflowProjectionsAsync(sessionId, ct);
        var breakdownByUserId = _happiness.ComputeByPlayer(events, projections, config);
        var emptyBreakdown = _happiness.ComputeBreakdown([], 0, 0, 0);

        var participants = (await conn.QueryAsync<FinalScoreParticipantRow>(
            new CommandDefinition(
                """
                select
                    sp.session_participant_id as SessionParticipantId,
                    sp.user_id as UserId,
                    sptb.tie_number as TieBreakerNumber,
                    coalesce(spb.coins, 0)
                        + coalesce(spb.saving, 0)
                        + coalesce(ingredients.leftover_qty, 0) as PensionFund
                from session_participants sp
                left join session_participant_balances spb
                  on spb.session_id = sp.session_id
                 and spb.session_participant_id = sp.session_participant_id
                left join (
                    select session_id, session_participant_id, sum(qty)::int as leftover_qty
                    from session_participant_inventory
                    group by session_id, session_participant_id
                ) ingredients
                  on ingredients.session_id = sp.session_id
                 and ingredients.session_participant_id = sp.session_participant_id
                left join session_participant_tie_breakers sptb
                  on sptb.session_id = sp.session_id
                 and sptb.session_participant_id = sp.session_participant_id
                where sp.session_id = @sessionId
                order by sp.player_order_no asc
                """,
                new { sessionId },
                cancellationToken: ct))).ToList();

        var pensionPointsByRank = config!.Scoring?.PensionRankPoints
            .ToDictionary(item => item.Rank, item => item.Points)
            ?? new Dictionary<int, int>();

        var pensionRanking = participants
            .Select(participant =>
            {
                return new FinalScoreParticipantValue(
                    participant,
                    participant.PensionFund,
                    breakdownByUserId.TryGetValue(participant.UserId, out var breakdown)
                        ? breakdown
                        : emptyBreakdown);
            })
            .OrderByDescending(item => item.CashRemaining)
            .ThenByDescending(item => item.Participant.TieBreakerNumber ?? 0)
            .ThenBy(item => item.Participant.UserId)
            .ToList();

        await using var tx = await conn.BeginTransactionAsync(ct);
        await conn.ExecuteAsync(
            new CommandDefinition(
                """
                update session_participant_collection_missions
                set is_failed = true,
                    updated_at = now()
                where session_id = @sessionId
                  and not is_completed;

                delete from session_final_score_components
                where session_final_score_id in (
                    select session_final_score_id
                    from session_final_scores
                    where session_id = @sessionId
                );

                delete from session_final_scores
                where session_id = @sessionId;

                """,
                new { sessionId },
                tx,
                cancellationToken: ct));

        var finalRanking = pensionRanking
            .Select((value, index) =>
            {
                pensionPointsByRank.TryGetValue(index + 1, out var pensionPoints);
                var correctedBreakdown = value.Breakdown with
                {
                    PensionPoints = pensionPoints
                };
                correctedBreakdown = correctedBreakdown with
                {
                    Total = correctedBreakdown.Total - value.Breakdown.PensionPoints + pensionPoints
                };

                return new
                {
                    Value = value,
                    CorrectedBreakdown = correctedBreakdown
                };
            })
            .OrderByDescending(item => item.CorrectedBreakdown.Total)
            .ThenByDescending(item => item.Value.Participant.TieBreakerNumber ?? 0)
            .ThenBy(item => item.Value.Participant.UserId)
            .ToList();

        for (var index = 0; index < finalRanking.Count; index++)
        {
            var value = finalRanking[index];
            var correctedBreakdown = value.CorrectedBreakdown;
            var scoreId = Guid.NewGuid();
            var totalPoints = (int)Math.Round(correctedBreakdown.Total, MidpointRounding.AwayFromZero);

            await conn.ExecuteAsync(
                new CommandDefinition(
                    """
                    insert into session_final_scores (
                        session_final_score_id,
                        session_id,
                        session_participant_id,
                        total_points,
                        rank_no,
                        tie_breaker_number,
                        has_unpaid_loan,
                        computed_at,
                        source_event_id
                    )
                    values (
                        @scoreId,
                        @sessionId,
                        @sessionParticipantId,
                        @totalPoints,
                        @rankNo,
                        @tieBreakerNumber,
                        @hasUnpaidLoan,
                        now(),
                        @sourceEventId
                    )
                    """,
                    new
                    {
                        scoreId,
                        sessionId,
                        sessionParticipantId = value.Value.Participant.SessionParticipantId,
                        totalPoints,
                        rankNo = index + 1,
                        value.Value.Participant.TieBreakerNumber,
                        correctedBreakdown.HasUnpaidLoan,
                        sourceEventId
                    },
                    tx,
                    cancellationToken: ct));

            var components = new[]
            {
                new FinalScoreComponentValue("NEED_POINTS", correctedBreakdown.NeedPoints),
                new FinalScoreComponentValue("NEED_SET_BONUS", correctedBreakdown.NeedSetBonusPoints),
                new FinalScoreComponentValue("DONATION", correctedBreakdown.DonationPoints),
                new FinalScoreComponentValue("GOLD", correctedBreakdown.GoldPoints),
                new FinalScoreComponentValue("PENSION", correctedBreakdown.PensionPoints),
                new FinalScoreComponentValue("SAVING_GOAL", correctedBreakdown.SavingGoalPointsEffective),
                new FinalScoreComponentValue("MISSION_PENALTY", -correctedBreakdown.MissionPenaltyPoints),
                new FinalScoreComponentValue("LOAN_PENALTY", -correctedBreakdown.LoanPenaltyPoints)
            };

            foreach (var component in components)
            {
                await conn.ExecuteAsync(
                    new CommandDefinition(
                        """
                        insert into session_final_score_components (
                            session_final_score_component_id,
                            session_id,
                            session_participant_id,
                            session_final_score_id,
                            component_code,
                            points,
                            source_event_id,
                            created_at
                        )
                        values (
                            @componentId,
                            @sessionId,
                            @sessionParticipantId,
                            @scoreId,
                            @componentCode,
                            @points,
                            @sourceEventId,
                            now()
                        )
                        """,
                        new
                        {
                            componentId = Guid.NewGuid(),
                            sessionId,
                            sessionParticipantId = value.Value.Participant.SessionParticipantId,
                            scoreId,
                            componentCode = component.Code,
                            points = (int)Math.Round(component.Points, MidpointRounding.AwayFromZero),
                            sourceEventId
                        },
                        tx,
                        cancellationToken: ct));
            }
        }

        await tx.CommitAsync(ct);
    }

    private static async Task LoadPlayerChildrenAsync(
        NpgsqlConnection conn,
        Guid[] sessionPlayerIds,
        IReadOnlyDictionary<Guid, SessionPlayerStateDto> players,
        CancellationToken ct)
    {
        const string bahanSql = """
            select spi.session_participant_id as session_player_id, asset.display_name as nama, spi.qty as jumlah
            from session_participant_inventory spi
            join ruleset_game_assets asset on asset.ruleset_game_asset_id = spi.ruleset_game_asset_id
            where spi.session_participant_id = any(@sessionPlayerIds)
            order by asset.display_name asc
            """;

        const string kebutuhanSql = """
            select
                spn.session_participant_id as session_player_id,
                rn.item_name as nama,
                coalesce(rn.need_tier, '') as tipe
            from session_participant_need_purchases spn
            join ruleset_needs rn on rn.ruleset_need_id = spn.ruleset_need_id
            where spn.session_participant_id = any(@sessionPlayerIds)
            order by spn.sort_order asc, rn.item_name asc
            """;

        const string tujuanSql = """
            select
                spfg.session_participant_id as session_player_id,
                rfg.item_name as nama,
                spfg.current_amount,
                spfg.target_amount,
                spfg.status,
                spfg.purchased_at_day
            from session_participant_financial_goals spfg
            join ruleset_financial_goals rfg on rfg.ruleset_financial_goal_id = spfg.ruleset_financial_goal_id
            where spfg.session_participant_id = any(@sessionPlayerIds)
            order by coalesce(spfg.purchased_at_day, 2147483647) asc, rfg.item_name asc
            """;

        const string targetSql = """
            select spcm.session_participant_id as session_player_id, rcm.mission_code as id, spcm.is_completed, spcm.is_failed, spcm.reward_applied
            from session_participant_collection_missions spcm
            join ruleset_collection_missions rcm on rcm.ruleset_collection_mission_id = spcm.ruleset_collection_mission_id
            where spcm.session_participant_id = any(@sessionPlayerIds)
            order by rcm.mission_code asc
            """;

        const string counterSql = """
            select
                spac.session_participant_id as session_player_id,
                ra.action_id as aksi,
                spac.count
            from session_participant_action_counters spac
            join ruleset_actions ra
              on ra.ruleset_version_id = spac.ruleset_version_id
             and ra.ruleset_action_id = spac.ruleset_action_id
            where spac.session_participant_id = any(@sessionPlayerIds)
            order by ra.action_id asc
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
                player.TujuanFinansial.Add(new TujuanFinansialItemDto
                {
                    Nama = row.Nama,
                    CurrentAmount = row.CurrentAmount,
                    TargetAmount = row.TargetAmount,
                    Status = row.Status,
                    PurchasedAtDay = row.PurchasedAtDay
                });
            }
        }

        foreach (var row in await conn.QueryAsync<TargetKebutuhanRow>(new CommandDefinition(targetSql, new { sessionPlayerIds }, cancellationToken: ct)))
        {
            if (players.TryGetValue(row.SessionPlayerId, out var player))
            {
                player.TargetKebutuhan.Add(new TargetKebutuhanProgressDto(row.Id, row.IsCompleted, row.IsFailed, row.RewardApplied));
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
            select
                donation.donation_event_id as event_id,
                donation.event_ke,
                donation.day,
                coalesce((
                    select jsonb_agg(
                        jsonb_build_object(
                            'rank', coalesce((event.payload->>'rank')::int, 0),
                            'session_player_id', event.session_player_id,
                            'player_order_no', participant.player_order_no,
                            'total_donasi', coalesce(balance.total_donasi, 0)
                        )
                        order by coalesce((event.payload->>'rank')::int, 0)
                    )
                    from events event
                    join session_participants participant
                      on participant.session_id = event.session_id
                     and participant.session_participant_id = event.session_player_id
                    left join session_participant_balances balance
                      on balance.session_id = event.session_id
                     and balance.session_participant_id = event.session_player_id
                    where event.session_id = donation.session_id
                      and event.action_type = 'PoinPeringkatDonasi'
                      and greatest(1, ((event.day_index + 3) / 7)) = donation.event_ke
                ), '[]'::jsonb)::text as rankings_json
            from session_donation_events donation
            where donation.session_id = @sessionId
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
                        PlayerIndex = ranking.TryGetProperty("player_order_no", out var playerIndexProp) &&
                                      playerIndexProp.ValueKind == JsonValueKind.Number
                            ? playerIndexProp.GetInt32()
                            : ranking.TryGetProperty("player_index", out var playerIndexAliasProp) &&
                              playerIndexAliasProp.ValueKind == JsonValueKind.Number
                            ? playerIndexAliasProp.GetInt32()
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

            delete from session_participant_inventory
            where session_participant_id in (
                select session_participant_id
                from session_participants
                where session_id = @sessionId
            );

            delete from session_participant_need_purchases
            where session_participant_id in (
                select session_participant_id
                from session_participants
                where session_id = @sessionId
            );

            delete from session_participant_financial_goals
            where session_participant_id in (
                select session_participant_id
                from session_participants
                where session_id = @sessionId
            );

            delete from session_participant_collection_missions
            where session_participant_id in (
                select session_participant_id
                from session_participants
                where session_id = @sessionId
            );

            delete from session_participant_action_counters
            where session_participant_id in (
                select session_participant_id
                from session_participants
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
            insert into session_participant_balances (session_id, session_participant_id, coins, happiness, saving, total_donasi, created_at, updated_at)
            values (@sessionId, @sessionPlayerId, @coins, @happiness, @saving, @totalDonasi, now(), now())
            on conflict (session_participant_id) do update
            set coins = excluded.coins,
                happiness = excluded.happiness,
                saving = excluded.saving,
                total_donasi = excluded.total_donasi,
                updated_at = now()
            """;

        const string updatePlayerNameSql = """
            update session_participants
            set player_name = @name
            where session_id = @sessionId
              and session_participant_id = @sessionPlayerId
            """;

        await conn.ExecuteAsync(new CommandDefinition(
            upsertPlayerStateSql,
            new
            {
                sessionId,
                sessionPlayerId = player.SessionPlayerId,
                coins = player.Coins,
                happiness = player.Happiness,
                saving = player.Saving,
                totalDonasi = player.TotalDonasi
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
            insert into session_participant_inventory (
                session_id,
                session_participant_id,
                ruleset_version_id,
                ruleset_game_asset_id,
                qty,
                created_at,
                updated_at
            )
            select @sessionId, @sessionPlayerId, asset.ruleset_version_id, asset.ruleset_game_asset_id, @jumlah, now(), now()
            from ruleset_game_assets asset
            where asset.ruleset_version_id = (
                select ruleset_version_id
                from sessions
                where session_id = @sessionId
                limit 1
            )
              and asset.asset_type = 'INGREDIENT'
              and lower(asset.display_name) = lower(@nama)
            """;

        foreach (var item in player.Bahan)
        {
            await conn.ExecuteAsync(new CommandDefinition(
                insertBahanSql,
                new { sessionId, sessionPlayerId = player.SessionPlayerId, nama = item.Nama, jumlah = item.Jumlah },
                tx,
                cancellationToken: ct));
        }

        const string insertKebutuhanSql = """
            insert into session_participant_need_purchases (
                session_participant_need_purchase_id, session_id, session_participant_id, ruleset_version_id, ruleset_need_id, sort_order, paid_amount, happiness_delta, purchased_at_day, created_at
            )
            select
                @entryId,
                @sessionId,
                @sessionPlayerId,
                rn.ruleset_version_id,
                rn.ruleset_need_id,
                @sortOrder,
                rn.purchase_price,
                rn.happiness_points,
                @purchasedAtDay,
                now()
            from ruleset_needs rn
            where rn.ruleset_version_id = (
                select ruleset_version_id
                from sessions
                where session_id = @sessionId
                limit 1
            )
              and lower(rn.item_name) = lower(@nama)
              and lower(coalesce(rn.need_tier, '')) = lower(@tipe)
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
            insert into session_participant_financial_goals (
                session_id,
                session_participant_id,
                ruleset_version_id,
                ruleset_financial_goal_id,
                current_amount,
                target_amount,
                status,
                purchased_at_day,
                created_at,
                updated_at
            )
            select
                @sessionId,
                @sessionPlayerId,
                rfg.ruleset_version_id,
                rfg.ruleset_financial_goal_id,
                @currentAmount,
                @targetAmount,
                @status,
                @purchasedAtDay,
                now(),
                now()
            from ruleset_financial_goals rfg
            where rfg.ruleset_version_id = (
                select ruleset_version_id
                from sessions
                where session_id = @sessionId
                limit 1
            )
              and lower(rfg.item_name) = lower(@nama)
            limit 1
            """;

        foreach (var item in player.TujuanFinansial)
        {
            await conn.ExecuteAsync(new CommandDefinition(
                insertTujuanSql,
                new
                {
                    sessionId,
                    sessionPlayerId = player.SessionPlayerId,
                    nama = item.Nama,
                    currentAmount = item.CurrentAmount,
                    targetAmount = item.TargetAmount,
                    status = item.Status,
                    purchasedAtDay = item.PurchasedAtDay
                },
                tx,
                cancellationToken: ct));
        }

        const string insertTargetSql = """
            insert into session_participant_collection_missions (
                session_id, session_participant_id, ruleset_version_id, ruleset_collection_mission_id, is_completed, is_failed, reward_applied, assigned_at
            )
            select
                @sessionId,
                @sessionPlayerId,
                rcm.ruleset_version_id,
                rcm.ruleset_collection_mission_id,
                @isCompleted,
                @isFailed,
                @rewardApplied,
                now()
            from ruleset_collection_missions rcm
            where rcm.ruleset_version_id = (
                select ruleset_version_id
                from sessions
                where session_id = @sessionId
                limit 1
            )
              and lower(rcm.mission_code) = lower(@id)
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

        const string insertCounterSql = """
            insert into session_participant_action_counters (
                session_id,
                session_participant_id,
                ruleset_version_id,
                ruleset_action_id,
                count
            )
            select
                @sessionId,
                @sessionPlayerId,
                s.ruleset_version_id,
                ra.ruleset_action_id,
                @count
            from sessions s
            join ruleset_actions ra
              on ra.ruleset_version_id = s.ruleset_version_id
             and lower(ra.action_id) = lower(@aksi)
             and ra.is_active
            where s.session_id = @sessionId
            """;

        foreach (var item in player.ActionCounters)
        {
            await conn.ExecuteAsync(new CommandDefinition(
                insertCounterSql,
                new { sessionId, sessionPlayerId = player.SessionPlayerId, aksi = item.Aksi, count = item.Count },
                tx,
                cancellationToken: ct));
        }
    }

    private static async Task SaveDonationEventAsync(
        NpgsqlConnection conn,
        NpgsqlTransaction tx,
        Guid sessionId,
        DonationEventDto donationEvent,
        CancellationToken ct)
    {
        var eventId = Guid.NewGuid();

        const string insertEventSql = """
            insert into session_donation_events (donation_event_id, session_id, event_ke, day, created_at)
            values (@eventId, @sessionId, @eventKe, @day, now())
            """;

        await conn.ExecuteAsync(new CommandDefinition(
            insertEventSql,
            new
            {
                eventId,
                sessionId,
                eventKe = donationEvent.EventKe,
                day = donationEvent.Day
            },
            tx,
            cancellationToken: ct));
    }

    private static Task SaveLastActionAsync(
        NpgsqlConnection conn,
        NpgsqlTransaction tx,
        Guid sessionId,
        long newVersion,
        SaveSessionStateRequest request,
        CancellationToken ct)
    {
        return Task.CompletedTask;
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

    private static int ResolveCurrentActionSlot(int actionSlotsLeft)
    {
        return Math.Clamp(3 - Math.Max(0, actionSlotsLeft), 1, 2);
    }

    private static string BuildSyntheticPlayerUsername(Guid sessionId, int playerOrder)
    {
        return $"dev-player-{sessionId:N}-{playerOrder}";
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
                upper(rv.mode) as mode
            from rulesets r
            join ranked_versions rv on rv.ruleset_id = r.ruleset_id and rv.rn = 1
            where (
                    @rulesetId is not null
                    and r.ruleset_id = @rulesetId
                    and not r.is_archived
                    and (
                        r.instructor_user_id is null
                        or (@instructorUserId is not null and r.instructor_user_id = @instructorUserId)
                    )
                )
                or (
                    @rulesetId is null
                    and r.instructor_user_id is null
                    and not r.is_archived
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
            where r.instructor_user_id is null
              and not r.is_archived
            order by rv.version desc, r.created_at desc
            limit 1
            """;

        return await conn.ExecuteScalarAsync<Guid?>(
            new CommandDefinition(sql, new { mode }, cancellationToken: ct));
    }

    private static async Task<RequestedRulesetVersionRow?> ResolveRulesetVersionByIdAsync(
        NpgsqlConnection conn,
        string mode,
        Guid rulesetVersionId,
        Guid? instructorUserId,
        CancellationToken ct)
    {
        const string sql = """
            select
                r.ruleset_id,
                rv.ruleset_version_id,
                upper(rv.mode) as mode
            from ruleset_versions rv
            join rulesets r on r.ruleset_id = rv.ruleset_id
            where rv.ruleset_version_id = @rulesetVersionId
              and upper(rv.mode) = @mode
              and rv.status = 'ACTIVE'
              and not r.is_archived
              and (
                  r.instructor_user_id is null
                  or (@instructorUserId is not null and r.instructor_user_id = @instructorUserId)
              )
            limit 1
            """;

        return await conn.QuerySingleOrDefaultAsync<RequestedRulesetVersionRow>(
            new CommandDefinition(
                sql,
                new
                {
                    mode,
                    rulesetVersionId,
                    instructorUserId
                },
                cancellationToken: ct));
    }

    private sealed class SessionStateRow
    {
        public Guid SessionId { get; init; }
        public long StateVersion { get; init; }
        public long NextSequenceNumber { get; init; }
        public int Day { get; init; }
        public int Turn { get; init; }
        public int ActionSlotsLeft { get; init; }
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
        public int CurrentAmount { get; init; }
        public int? TargetAmount { get; init; }
        public string Status { get; init; } = "ONGOING";
        public int? PurchasedAtDay { get; init; }
    }

    private sealed class TargetKebutuhanRow
    {
        public Guid SessionPlayerId { get; init; }
        public string Id { get; init; } = string.Empty;
        public bool IsCompleted { get; init; }
        public bool IsFailed { get; init; }
        public bool RewardApplied { get; init; }
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
    }

    private sealed record SetupParticipant(Guid SessionParticipantId, Guid UserId, int PlayerOrder);

    private sealed class FinalScoreParticipantRow
    {
        public Guid SessionParticipantId { get; init; }
        public Guid UserId { get; init; }
        public int? TieBreakerNumber { get; init; }
        public int PensionFund { get; init; }
    }

    private sealed record FinalScoreParticipantValue(
        FinalScoreParticipantRow Participant,
        int CashRemaining,
        AnalyticsHappinessBreakdown Breakdown);

    private sealed record FinalScoreComponentValue(string Code, double Points);

}

public sealed class RulesetSectionDb
{
    public Guid RulesetId { get; init; }
    public Guid RulesetVersionId { get; init; }
    public string Mode { get; init; } = string.Empty;
    public RulesetDefinitionDto Definition { get; init; } = new();
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
        RulesetSectionGameConfig gameConfigValues,
        HashSet<string> bahanNames,
        Dictionary<string, string> kebutuhanTypes,
        HashSet<string> targetKebutuhanIds,
        HashSet<string> tujuanFinansialNames,
        HashSet<string> actions)
    {
        GameConfig = gameConfig;
        Bahan = bahan;
        Resep = resep;
        Kebutuhan = kebutuhan;
        TargetKebutuhan = targetKebutuhan;
        TujuanFinansial = tujuanFinansial;
        Narasi = narasi;
        GameConfigValues = gameConfigValues;
        BahanNames = bahanNames;
        KebutuhanTypes = kebutuhanTypes;
        TargetKebutuhanIds = targetKebutuhanIds;
        TujuanFinansialNames = tujuanFinansialNames;
        Actions = actions;
    }

    public JsonElement GameConfig { get; }
    public JsonElement Bahan { get; }
    public JsonElement Resep { get; }
    public JsonElement Kebutuhan { get; }
    public JsonElement TargetKebutuhan { get; }
    public JsonElement TujuanFinansial { get; }
    public JsonElement Narasi { get; }
    public RulesetSectionGameConfig GameConfigValues { get; }
    public HashSet<string> BahanNames { get; }
    public Dictionary<string, string> KebutuhanTypes { get; }
    public HashSet<string> TargetKebutuhanIds { get; }
    public HashSet<string> TujuanFinansialNames { get; }
    public HashSet<string> Actions { get; }

    public static RulesetSectionCatalog FromDefinition(RulesetDefinitionDto definition)
    {
        var gameConfig = SerializeElement(new
        {
            initialCoins = definition.Settings.InitialCoins == 0
                ? definition.Settings.StartingCash
                : definition.Settings.InitialCoins,
            initialHappiness = definition.Settings.InitialHappiness,
            initialSaving = definition.Settings.InitialSaving,
            actionsPerTurn = definition.Settings.ActionsPerTurn,
            finishDay = definition.Settings.FinishDay,
            minPlayers = definition.Settings.MinPlayers,
            maxPlayers = definition.Settings.MaxPlayers
        });
        var bahan = SerializeElement(definition.Ingredients);
        var resep = SerializeElement(definition.Orders);
        var kebutuhan = SerializeElement(definition.Needs);
        var targetKebutuhan = SerializeElement(definition.CollectionMissions);
        var tujuanFinansial = SerializeElement(definition.FinancialGoals);
        var narasi = SerializeElement(definition.Narratives);

        var gameConfigValues = new RulesetSectionGameConfig(
            definition.Settings.InitialCoins == 0 ? definition.Settings.StartingCash : definition.Settings.InitialCoins,
            definition.Settings.InitialHappiness,
            definition.Settings.InitialSaving,
            definition.Settings.ActionsPerTurn,
            definition.Settings.FinishDay,
            definition.Settings.MinPlayers,
            definition.Settings.MaxPlayers);

        var bahanNames = definition.Ingredients
            .Select(item => item.Nama)
            .Where(item => !string.IsNullOrWhiteSpace(item))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var kebutuhanTypes = definition.Needs
            .Where(item => !string.IsNullOrWhiteSpace(item.Nama) && !string.IsNullOrWhiteSpace(item.Tipe))
            .GroupBy(item => item.Nama, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.First().Tipe, StringComparer.OrdinalIgnoreCase);
        var targetKebutuhanIds = definition.CollectionMissions
            .Select(item => item.Id)
            .Where(item => !string.IsNullOrWhiteSpace(item))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var tujuanFinansialNames = definition.FinancialGoals
            .Select(item => item.Nama)
            .Where(item => !string.IsNullOrWhiteSpace(item))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
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

        foreach (var actionId in definition.Actions.Select(item => item.ActionId).Where(item => !string.IsNullOrWhiteSpace(item)))
        {
            actions.Add(actionId);
        }

        foreach (var actionId in definition.Narratives
                     .SelectMany(item => item.PrerequisiteAksi)
                     .Select(item => item.Aksi)
                     .Where(item => !string.IsNullOrWhiteSpace(item)))
        {
            actions.Add(actionId);
        }

        return new RulesetSectionCatalog(
            gameConfig,
            bahan,
            resep,
            kebutuhan,
            targetKebutuhan,
            tujuanFinansial,
            narasi,
            gameConfigValues,
            bahanNames,
            kebutuhanTypes,
            targetKebutuhanIds,
            tujuanFinansialNames,
            actions);
    }

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
            ReadInt(gameConfig, "finishDay", 25),
            ReadInt(gameConfig, "minPlayers", 2),
            ReadInt(gameConfig, "maxPlayers", 4));
        var bahan = componentCatalog.GetProperty("bahan").Clone();
        var resep = componentCatalog.GetProperty("resep").Clone();
        var kebutuhan = componentCatalog.GetProperty("kebutuhan").Clone();
        var targetKebutuhan = componentCatalog.GetProperty("targetKebutuhan").Clone();
        var tujuanFinansial = componentCatalog.GetProperty("tujuanFinansial").Clone();
        var narasi = componentCatalog.GetProperty("narasi").Clone();

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
            gameConfigValues,
            bahanNames,
            kebutuhanTypes,
            targetKebutuhanIds,
            tujuanFinansialNames,
            actions);
    }

    private static JsonElement SerializeElement<T>(T value)
    {
        using var document = JsonDocument.Parse(JsonSerializer.Serialize(value));
        return document.RootElement.Clone();
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
