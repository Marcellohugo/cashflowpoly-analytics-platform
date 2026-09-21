// Fungsi file: Memverifikasi undo atomik seluruh state, audit, idempotensi, dan pembatasan akses.
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Cashflowpoly.Api.Contracts;
using Cashflowpoly.Api.Tests.Infrastructure;
using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Xunit;

namespace Cashflowpoly.Api.Tests;

[Collection("ApiIntegration")]
[Trait("Category", "Integration")]
public sealed class EventUndoIntegrationTests(ApiIntegrationTestFixture fixture)
{
    [Theory]
    [InlineData("BahanMasakan", "session_participant_inventory")]
    [InlineData("Kebutuhan", "session_participant_collection_missions")]
    [InlineData("PinjamanSyariah", "session_participant_loans")]
    public async Task Undo_RestoresEveryGameplayTableIncludingCardsBalancesAndMissionRewards(string action, string changedTable)
    {
        var definition = EventAnalyticsIntegrationTests.BuildRulesetDefinition(50, mode: "MAHIR");
        for (var i = 0; i < definition.CollectionMissions.Count; i++)
        {
            var mission = definition.CollectionMissions[i];
            definition.CollectionMissions[i] = new RulesetCollectionMissionDto
            {
                Id = mission.Id, Nama = mission.Nama, SuccessPoints = 8, FailurePoints = mission.FailurePoints,
                PenaltyPoints = mission.PenaltyPoints,
                KebutuhanTarget = [new RulesetCollectionMissionRequirementDto { Order = 1, Type = "primer", Value = "buku" }]
            };
        }
        var game = await CreateGameAsync(definition);
        var before = await ReadTablesAsync(game.SessionId);
        object payload = action switch
        {
            "BahanMasakan" => new { card_id = "telur", amount = 4 },
            "Kebutuhan" => new { card_id = "buku", amount = 3, points = 1, need_tier = "primer" },
            _ => new { loan_id = "loan_syariah_10", loan_code = "loan_syariah_10", principal = 10, repayment_amount = 10, duration_days = 1, penalty_points = 10 }
        };
        var eventId = await PostEventAsync(game, action, payload);
        var afterAction = await ReadTablesAsync(game.SessionId);
        Assert.NotEqual(before[changedTable], afterAction[changedTable]);
        Assert.NotEqual(before["session_participant_balances"], afterAction["session_participant_balances"]);
        if (action == "Kebutuhan")
        {
            var state = await ReadStateAsync(game);
            Assert.Equal(9, state.Players.Single(player => player.UserId == game.ActingUserId).Happiness);
        }

        // Populate the derived analytics cache before undo, so stale totals cannot survive it.
        using var analytics = await SendAsync(game.Token, HttpMethod.Post, $"/api/v1/analytics/sessions/{game.SessionId}/recompute");
        analytics.EnsureSuccessStatusCode();
        await using var conn = await OpenConnectionAsync();
        Assert.True(await conn.ExecuteScalarAsync<int>("select count(*) from metric_snapshots where session_id = @id", new { id = game.SessionId }) > 0);
        var originalEvent = await conn.ExecuteScalarAsync<string>("select to_jsonb(e)::text from events e where session_id = @id and event_id = @eventId", new { id = game.SessionId, eventId });
        var originalAssetCount = await conn.ExecuteScalarAsync<int>("select count(*) from event_asset_references where session_id = @id and event_id = @eventId", new { id = game.SessionId, eventId });
        var stateBeforeUndo = await ReadStateAsync(game);
        var request = UndoRequest(stateBeforeUndo.StateVersion);
        using var undone = await SendUndoAsync(game, eventId, request);
        Assert.True(undone.StatusCode == HttpStatusCode.Created, await undone.Content.ReadAsStringAsync());
        var receipt = (await undone.Content.ReadFromJsonAsync<EventUndoResponse>())!;
        Assert.Equal(stateBeforeUndo.StateVersion + 1, receipt.StateVersion);
        Assert.Equal(stateBeforeUndo.NextSequenceNumber, receipt.NextSequenceNumber);
        AssertTablesEqual(before, await ReadTablesAsync(game.SessionId));

        Assert.Equal(0, await conn.ExecuteScalarAsync<int>("select count(*) from metric_snapshots where session_id = @id", new { id = game.SessionId }));
        using var auditResponse = await SendAsync(game.Token, HttpMethod.Get, $"/api/v1/sessions/{game.SessionId}/event-undos");
        auditResponse.EnsureSuccessStatusCode();
        var audit = (await auditResponse.Content.ReadFromJsonAsync<EventUndoAuditResponse>())!;
        var entry = Assert.Single(audit.Items);
        Assert.Equal(eventId, entry.EventId);
        Assert.Equal(request.ClientRequestId, entry.ClientRequestId);
        Assert.Equal(action, entry.OriginalEvent.GetProperty("action_type").GetString());
        Assert.Equal(eventId, entry.OriginalEvent.GetProperty("event_id").GetGuid());
        Assert.True(JsonElement.DeepEquals(JsonSerializer.Deserialize<JsonElement>(originalEvent!), entry.OriginalEvent));
        Assert.NotEmpty(entry.OriginalCashflows.EnumerateArray());
        Assert.Equal(originalAssetCount, entry.OriginalAssetReferences.GetArrayLength());

        using var retry = await SendUndoAsync(game, eventId, request);
        Assert.Equal(HttpStatusCode.OK, retry.StatusCode);
        Assert.Equal(receipt, await retry.Content.ReadFromJsonAsync<EventUndoResponse>());
        AssertTablesEqual(before, await ReadTablesAsync(game.SessionId));
        var restored = await ReadStateAsync(game);
        Assert.Equal(receipt.StateVersion, restored.StateVersion);
        Assert.Equal(receipt.NextSequenceNumber, restored.NextSequenceNumber);
    }

    [Fact]
    public async Task Undo_RestoresTransfersForAllPlayers_AndSupportsRepeatedLifoUndo()
    {
        var definition = EventAnalyticsIntegrationTests.BuildRulesetDefinition(50, mode: "MAHIR");
        definition.LifeRisks.Add(new RulesetLifeRiskDto
        {
            RiskCode = "birthday_undo", ItemName = "Ulang Tahun", EffectType = "PLAYER_TO_PLAYER_TRANSFER",
            Direction = "IN", Amount = 1, TargetScope = "OTHER_PLAYERS", DurationDays = 1, CardQty = 1
        });
        var game = await CreateGameAsync(definition);
        var initial = await ReadTablesAsync(game.SessionId);
        var orderId = await PostEventAsync(game, "JualMasakan", new { order_card_id = "nasi_goreng" });
        var beforeRisk = await ReadTablesAsync(game.SessionId);
        var balances = (await ReadStateAsync(game)).Players.ToDictionary(player => player.UserId!.Value, player => player.Coins);
        var riskId = await PostEventAsync(game, "RisikoKehidupan", new { risk_id = "birthday_undo", source_order_event_id = orderId }, slot: 0);
        var afterRisk = await ReadStateAsync(game);
        foreach (var player in afterRisk.Players)
            Assert.Equal(balances[player.UserId!.Value] + (player.UserId == game.ActingUserId ? 2 : -1), player.Coins);

        using var notLast = await SendUndoAsync(game, orderId, UndoRequest(afterRisk.StateVersion));
        await AssertErrorAsync(notLast, HttpStatusCode.Conflict, "EVENT_NOT_LAST");
        using var stale = await SendUndoAsync(game, riskId, UndoRequest(afterRisk.StateVersion - 1));
        await AssertErrorAsync(stale, HttpStatusCode.Conflict, "STATE_VERSION_CONFLICT");
        using var riskUndo = await SendUndoAsync(game, riskId, UndoRequest(afterRisk.StateVersion));
        Assert.True(riskUndo.StatusCode == HttpStatusCode.Created, await riskUndo.Content.ReadAsStringAsync());
        AssertTablesEqual(beforeRisk, await ReadTablesAsync(game.SessionId));
        var state = await ReadStateAsync(game);
        Assert.Equal(afterRisk.StateVersion + 1, state.StateVersion);
        using var orderUndo = await SendUndoAsync(game, orderId, UndoRequest(state.StateVersion));
        Assert.True(orderUndo.StatusCode == HttpStatusCode.Created, await orderUndo.Content.ReadAsStringAsync());
        AssertTablesEqual(initial, await ReadTablesAsync(game.SessionId));
        var finalState = await ReadStateAsync(game);
        Assert.Equal(afterRisk.StateVersion + 2, finalState.StateVersion);
        Assert.Equal(afterRisk.NextSequenceNumber, finalState.NextSequenceNumber);
    }

    [Fact]
    public async Task Undo_ConcurrentRetriesHaveOneReceipt_AndReserveOriginalEventIdentifiers()
    {
        var game = await CreateGameAsync();
        var originalState = await ReadStateAsync(game);
        var originalRequest = BuildEvent(game, originalState, "KerjaLepas", new { amount = 1 }, Guid.NewGuid(), 1, "original-request");
        using var accepted = await SendAsync(game.Token, HttpMethod.Post, "/api/v1/events", originalRequest);
        Assert.True(accepted.StatusCode == HttpStatusCode.Created, await accepted.Content.ReadAsStringAsync());
        var state = await ReadStateAsync(game);
        var undoRequest = UndoRequest(state.StateVersion);
        var tasks = Enumerable.Range(0, 2).Select(_ => SendUndoAsync(game, originalRequest.EventId, undoRequest)).ToArray();
        var responses = await Task.WhenAll(tasks);
        try
        {
            Assert.Single(responses, response => response.StatusCode == HttpStatusCode.Created);
            Assert.Single(responses, response => response.StatusCode == HttpStatusCode.OK);
            Assert.Equal(await responses[0].Content.ReadFromJsonAsync<EventUndoResponse>(), await responses[1].Content.ReadFromJsonAsync<EventUndoResponse>());
        }
        finally { foreach (var response in responses) response.Dispose(); }

        using var differentReason = await SendUndoAsync(game, originalRequest.EventId, new UndoEventRequest
        {
            ClientRequestId = undoRequest.ClientRequestId, ExpectedStateVersion = state.StateVersion, Reason = "Different request"
        });
        await AssertErrorAsync(differentReason, HttpStatusCode.Conflict, "CLIENT_REQUEST_ID_CONFLICT");
        using var alreadyUndone = await SendUndoAsync(game, originalRequest.EventId, UndoRequest(state.StateVersion + 1));
        await AssertErrorAsync(alreadyUndone, HttpStatusCode.Conflict, "EVENT_ALREADY_UNDONE");
        using var replay = await SendAsync(game.Token, HttpMethod.Post, "/api/v1/events", originalRequest);
        Assert.Equal(HttpStatusCode.Conflict, replay.StatusCode);

        var restored = await ReadStateAsync(game);
        var reusedKey = BuildEvent(game, restored, "KerjaLepas", new { amount = 1 }, Guid.NewGuid(), 1, "original-request");
        using var keyConflict = await SendAsync(game.Token, HttpMethod.Post, "/api/v1/events", reusedKey);
        Assert.Equal(HttpStatusCode.Conflict, keyConflict.StatusCode);
        var reusedSequence = BuildEvent(game, originalState, "KerjaLepas", new { amount = 1 }, Guid.NewGuid(), 1, "new-request");
        using var sequenceConflict = await SendAsync(game.Token, HttpMethod.Post, "/api/v1/events", reusedSequence);
        Assert.Equal(HttpStatusCode.Conflict, sequenceConflict.StatusCode);

        var replacementId = await PostEventAsync(game, "KerjaLepas", new { amount = 1 });
        var replaced = await ReadStateAsync(game);
        Assert.Equal(restored.StateVersion + 1, replaced.StateVersion);
        Assert.Equal(restored.NextSequenceNumber + 1, replaced.NextSequenceNumber);
        Assert.Equal(state.Players.Single(player => player.UserId == game.ActingUserId).Coins,
            replaced.Players.Single(player => player.UserId == game.ActingUserId).Coins);
        using var history = await SendAsync(game.Token, HttpMethod.Get, $"/api/v1/sessions/{game.SessionId}/events?limit=100");
        history.EnsureSuccessStatusCode();
        using var body = JsonDocument.Parse(await history.Content.ReadAsStringAsync());
        Assert.DoesNotContain(body.RootElement.GetProperty("items").EnumerateArray(), item => item.GetProperty("event_id").GetGuid() == originalRequest.EventId);
        Assert.Contains(body.RootElement.GetProperty("items").EnumerateArray(), item => item.GetProperty("event_id").GetGuid() == replacementId);
        Assert.Contains(body.RootElement.GetProperty("undone_sequence_numbers").EnumerateArray(), item => item.GetInt64() == originalState.NextSequenceNumber);
    }

    [Fact]
    public async Task Undo_RejectsOtherOwnersPlayersSetupAndEventsWithoutSnapshot()
    {
        var game = await CreateGameAsync();
        var initial = await ReadStateAsync(game);
        await using var conn = await OpenConnectionAsync();
        var setupEvent = await conn.ExecuteScalarAsync<Guid>("select event_id from events where session_id = @id order by sequence_number desc limit 1", new { id = game.SessionId });
        using var setupUndo = await SendUndoAsync(game, setupEvent, UndoRequest(initial.StateVersion));
        await AssertErrorAsync(setupUndo, HttpStatusCode.UnprocessableEntity, "SETUP_UNDO_NOT_ALLOWED");
        var eventId = await PostEventAsync(game, "KerjaLepas", new { amount = 1 });
        var state = await ReadStateAsync(game);
        var otherToken = await RegisterAsync("INSTRUCTOR");
        using var otherOwner = await SendAsync(otherToken, HttpMethod.Post, UndoPath(game, eventId), UndoRequest(state.StateVersion));
        Assert.Equal(HttpStatusCode.NotFound, otherOwner.StatusCode);
        using var otherAudit = await SendAsync(otherToken, HttpMethod.Get, $"/api/v1/sessions/{game.SessionId}/event-undos");
        Assert.Equal(HttpStatusCode.NotFound, otherAudit.StatusCode);
        var playerToken = await RegisterAsync("PLAYER");
        using var playerUndo = await SendAsync(playerToken, HttpMethod.Post, UndoPath(game, eventId), UndoRequest(state.StateVersion));
        Assert.Equal(HttpStatusCode.Forbidden, playerUndo.StatusCode);
        using var playerAudit = await SendAsync(playerToken, HttpMethod.Get, $"/api/v1/sessions/{game.SessionId}/event-undos");
        Assert.Equal(HttpStatusCode.Forbidden, playerAudit.StatusCode);

        await conn.ExecuteAsync("delete from event_undo_snapshots where session_id = @id and event_id = @eventId", new { id = game.SessionId, eventId });
        var beforeRejected = await ReadTablesAsync(game.SessionId);
        using var legacy = await SendUndoAsync(game, eventId, UndoRequest(state.StateVersion));
        await AssertErrorAsync(legacy, HttpStatusCode.UnprocessableEntity, "UNDO_SNAPSHOT_UNAVAILABLE");
        AssertTablesEqual(beforeRejected, await ReadTablesAsync(game.SessionId));
        using var ended = await SendAsync(game.Token, HttpMethod.Post, $"/api/v1/sessions/{game.SessionId}/end");
        ended.EnsureSuccessStatusCode();
        using var endedUndo = await SendUndoAsync(game, eventId, UndoRequest(state.StateVersion));
        await AssertErrorAsync(endedUndo, HttpStatusCode.UnprocessableEntity, "SESSION_NOT_STARTED");
    }

    [Fact]
    public async Task Undo_ReceiptAndAuditSurviveDeletionOfNowEmptySession()
    {
        var game = await CreateGameAsync();
        var eventId = await PostEventAsync(game, "KerjaLepas", new { amount = 1 });
        var request = UndoRequest((await ReadStateAsync(game)).StateVersion);
        using var undo = await SendUndoAsync(game, eventId, request);
        Assert.True(undo.StatusCode == HttpStatusCode.Created, await undo.Content.ReadAsStringAsync());
        var receipt = await undo.Content.ReadFromJsonAsync<EventUndoResponse>();
        using var ended = await SendAsync(game.Token, HttpMethod.Post, $"/api/v1/sessions/{game.SessionId}/end");
        ended.EnsureSuccessStatusCode();
        Assert.Equal("DELETED", (await ended.Content.ReadFromJsonAsync<SessionStatusResponse>())!.Status);
        using var missing = await SendAsync(game.Token, HttpMethod.Get, $"/api/v1/sessions/{game.SessionId}/state");
        Assert.Equal(HttpStatusCode.NotFound, missing.StatusCode);
        using var retry = await SendUndoAsync(game, eventId, request);
        Assert.Equal(HttpStatusCode.OK, retry.StatusCode);
        Assert.Equal(receipt, await retry.Content.ReadFromJsonAsync<EventUndoResponse>());
        using var audit = await SendAsync(game.Token, HttpMethod.Get, $"/api/v1/sessions/{game.SessionId}/event-undos");
        audit.EnsureSuccessStatusCode();
        Assert.Equal(eventId, Assert.Single((await audit.Content.ReadFromJsonAsync<EventUndoAuditResponse>())!.Items).EventId);
        await using var conn = await OpenConnectionAsync();
        var mutation = await Assert.ThrowsAsync<PostgresException>(() => conn.ExecuteAsync(
            "update event_undos set reason = 'tampered' where session_id = @id", new { id = game.SessionId }));
        Assert.Equal("23514", mutation.SqlState);
    }

    [Fact]
    public async Task Undo_RestoreFailureRollsBackDeletedEventAuditAndState()
    {
        var game = await CreateGameAsync();
        var eventId = await PostEventAsync(game, "BahanMasakan", new { card_id = "telur", amount = 4 });
        var state = await ReadStateAsync(game);
        var before = await ReadTablesAsync(game.SessionId);
        await using var conn = await OpenConnectionAsync();
        var snapshot = await conn.ExecuteScalarAsync<string>("select before_state::text from event_undo_snapshots where session_id = @id and event_id = @eventId", new { id = game.SessionId, eventId });
        await conn.ExecuteAsync("update event_undo_snapshots set before_state = jsonb_set(before_state, '{version}', '999'::jsonb) where session_id = @id and event_id = @eventId", new { id = game.SessionId, eventId });
        var request = UndoRequest(state.StateVersion);
        using var failed = await SendUndoAsync(game, eventId, request);
        Assert.Equal(HttpStatusCode.InternalServerError, failed.StatusCode);
        AssertTablesEqual(before, await ReadTablesAsync(game.SessionId));
        Assert.Equal(state.StateVersion, (await ReadStateAsync(game)).StateVersion);
        Assert.Equal(0, await conn.ExecuteScalarAsync<int>("select count(*) from event_undos where session_id = @id", new { id = game.SessionId }));
        await conn.ExecuteAsync("update event_undo_snapshots set before_state = @snapshot::jsonb where session_id = @id and event_id = @eventId", new { id = game.SessionId, eventId, snapshot });
        using var retried = await SendUndoAsync(game, eventId, request);
        Assert.True(retried.StatusCode == HttpStatusCode.Created, await retried.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Undo_FinalFridayDonationRestoresEveryPlayersBalancesAndSealedRound()
    {
        var game = await CreateGameAsync();
        var players = (await ReadStateAsync(game)).Players.OrderBy(player => player.PlayerIndex).ToList();
        for (var day = 1; day <= 4; day++)
        {
            foreach (var player in players)
                for (var slot = 1; slot <= 2; slot++)
                {
                    var request = BuildEvent(game, await ReadStateAsync(game), "KerjaLepas", new { amount = 1 }, Guid.NewGuid(), slot)
                        with { UserId = player.UserId, TurnNumber = player.PlayerIndex };
                    using var action = await SendAsync(game.Token, HttpMethod.Post, "/api/v1/events", request);
                    Assert.True(action.StatusCode == HttpStatusCode.Created, await action.Content.ReadAsStringAsync());
                }
            var endRequest = BuildEvent(game, await ReadStateAsync(game), "AkhirGiliran", new { }, Guid.NewGuid(), 0)
                with { ActorType = "SYSTEM", UserId = null, TurnNumber = 0 };
            using var end = await SendAsync(game.Token, HttpMethod.Post, "/api/v1/events", endRequest);
            Assert.True(end.StatusCode == HttpStatusCode.Created, await end.Content.ReadAsStringAsync());
        }

        Dictionary<string, string>? beforeLast = null;
        Guid lastEventId = default;
        foreach (var player in players)
        {
            beforeLast = await ReadTablesAsync(game.SessionId);
            var request = BuildEvent(game, await ReadStateAsync(game), "JumatBerkah", new { amount = player.PlayerIndex }, Guid.NewGuid(), 0)
                with { UserId = player.UserId, TurnNumber = player.PlayerIndex };
            using var donation = await SendAsync(game.Token, HttpMethod.Post, "/api/v1/events", request);
            Assert.True(donation.StatusCode == HttpStatusCode.Created, await donation.Content.ReadAsStringAsync());
            lastEventId = request.EventId;
        }
        var after = await ReadTablesAsync(game.SessionId);
        Assert.NotEqual(beforeLast!["session_participant_balances"], after["session_participant_balances"]);
        using var undo = await SendUndoAsync(game, lastEventId, UndoRequest((await ReadStateAsync(game)).StateVersion));
        Assert.True(undo.StatusCode == HttpStatusCode.Created, await undo.Content.ReadAsStringAsync());
        AssertTablesEqual(beforeLast, await ReadTablesAsync(game.SessionId));
        using var events = await SendAsync(game.Token, HttpMethod.Get, $"/api/v1/sessions/{game.SessionId}/events?limit=100");
        events.EnsureSuccessStatusCode();
        using var history = JsonDocument.Parse(await events.Content.ReadAsStringAsync());
        var sealedDonations = history.RootElement.GetProperty("items").EnumerateArray().Where(item => item.GetProperty("action_type").GetString() == "JumatBerkah").ToArray();
        Assert.Equal(2, sealedDonations.Length);
        Assert.All(sealedDonations, item => Assert.True(item.GetProperty("payload").TryGetProperty("status", out _)));

        var finalPlayer = players[^1];
        var replacement = BuildEvent(game, await ReadStateAsync(game), "JumatBerkah", new { amount = finalPlayer.PlayerIndex }, Guid.NewGuid(), 0)
            with { UserId = finalPlayer.UserId, TurnNumber = finalPlayer.PlayerIndex };
        using var replaced = await SendAsync(game.Token, HttpMethod.Post, "/api/v1/events", replacement);
        Assert.True(replaced.StatusCode == HttpStatusCode.Created, await replaced.Content.ReadAsStringAsync());
        var beforeEnd = await ReadTablesAsync(game.SessionId);
        var endDay = BuildEvent(game, await ReadStateAsync(game), "AkhirGiliran", new { }, Guid.NewGuid(), 0)
            with { ActorType = "SYSTEM", UserId = null, TurnNumber = 0 };
        using var endFriday = await SendAsync(game.Token, HttpMethod.Post, "/api/v1/events", endDay);
        Assert.True(endFriday.StatusCode == HttpStatusCode.Created, await endFriday.Content.ReadAsStringAsync());
        Assert.Equal(6, (await ReadStateAsync(game)).Day);
        Assert.NotEqual(beforeEnd["session_states"], (await ReadTablesAsync(game.SessionId))["session_states"]);
        using var undoEnd = await SendUndoAsync(game, endDay.EventId, UndoRequest((await ReadStateAsync(game)).StateVersion));
        Assert.True(undoEnd.StatusCode == HttpStatusCode.Created, await undoEnd.Content.ReadAsStringAsync());
        AssertTablesEqual(beforeEnd, await ReadTablesAsync(game.SessionId));
        Assert.Equal(5, (await ReadStateAsync(game)).Day);
    }

    private sealed record Game(string Token, Guid SessionId, Guid ActingUserId, Guid RulesetVersionId);

    private async Task<Game> CreateGameAsync(RulesetDefinitionDto? definition = null)
    {
        var token = await RegisterAsync("INSTRUCTOR");
        var setup = await new EventAnalyticsIntegrationTests(fixture).CreateReadySessionAsync(token, Guid.NewGuid().ToString("N")[..10], definition);
        return new(token, setup.SessionId, setup.ActingUserId, setup.RulesetVersionId);
    }

    private async Task<string> RegisterAsync(string role)
    {
        using var response = await fixture.Client.PostAsJsonAsync("/api/v1/auth/register", new
        {
            username = $"undo_{Guid.NewGuid():N}", password = "UndoIntegration!2026", role
        });
        Assert.True(response.StatusCode == HttpStatusCode.Created, await response.Content.ReadAsStringAsync());
        return (await response.Content.ReadFromJsonAsync<RegisterResponse>())!.AccessToken;
    }

    private async Task<SessionStateResponse> ReadStateAsync(Game game)
    {
        using var response = await SendAsync(game.Token, HttpMethod.Get, $"/api/v1/sessions/{game.SessionId}/state");
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<SessionStateResponse>())!;
    }

    private async Task<Guid> PostEventAsync(Game game, string action, object payload, int slot = 1)
    {
        var eventId = Guid.NewGuid();
        var request = BuildEvent(game, await ReadStateAsync(game), action, payload, eventId, slot);
        using var response = await SendAsync(game.Token, HttpMethod.Post, "/api/v1/events", request);
        Assert.True(response.StatusCode == HttpStatusCode.Created, await response.Content.ReadAsStringAsync());
        return eventId;
    }

    private static EventRequest BuildEvent(Game game, SessionStateResponse state, string action, object payload, Guid eventId, int slot, string? requestId = null) => new(
        EventId: eventId, SessionId: game.SessionId, UserId: game.ActingUserId,
        ActorType: "PLAYER", Timestamp: DateTimeOffset.UtcNow, DayIndex: state.Day,
        Weekday: new[] { "MON", "TUE", "WED", "THU", "FRI", "SAT", "SUN" }[(state.Day - 1) % 7],
        TurnNumber: 1, ActionSlot: slot, SequenceNumber: state.NextSequenceNumber,
        ActionType: action, RulesetVersionId: game.RulesetVersionId, ClientRequestId: requestId,
        Payload: JsonSerializer.SerializeToElement(payload));

    private static UndoEventRequest UndoRequest(long version) => new()
    {
        ClientRequestId = Guid.NewGuid().ToString("N"), ExpectedStateVersion = version, Reason = "Correct an accidental activity"
    };

    private static string UndoPath(Game game, Guid eventId) => $"/api/v1/sessions/{game.SessionId}/events/{eventId}/undo";
    private Task<HttpResponseMessage> SendUndoAsync(Game game, Guid eventId, UndoEventRequest request) => SendAsync(game.Token, HttpMethod.Post, UndoPath(game, eventId), request);

    private async Task<HttpResponseMessage> SendAsync(string token, HttpMethod method, string path, object? body = null)
    {
        using var request = new HttpRequestMessage(method, path);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        if (body is not null) request.Content = JsonContent.Create(body);
        return await fixture.Client.SendAsync(request, TestContext.Current.CancellationToken);
    }

    private ValueTask<NpgsqlConnection> OpenConnectionAsync() => fixture.Services.GetRequiredService<NpgsqlDataSource>().OpenConnectionAsync(TestContext.Current.CancellationToken);

    private async Task<Dictionary<string, string>> ReadTablesAsync(Guid sessionId)
    {
        await using var conn = await OpenConnectionAsync();
        // Discover the actual relational tables independently of the production snapshot function.
        var tables = await conn.QueryAsync<string>("""
            select c.table_name from information_schema.columns c
            join information_schema.tables t using (table_schema, table_name)
            where c.table_schema = 'public' and c.column_name = 'session_id' and t.table_type = 'BASE TABLE'
                and (c.table_name like 'session\_%' escape '\' or c.table_name in ('events', 'event_cashflow_projections', 'event_asset_references'))
            order by c.table_name
            """);
        var result = new Dictionary<string, string>();
        foreach (var table in tables)
        {
            var row = table == "session_states" ? "to_jsonb(t) - 'state_version' - 'updated_at'" : "to_jsonb(t)";
            var quotedTable = new NpgsqlCommandBuilder().QuoteIdentifier(table);
            result[table] = await conn.ExecuteScalarAsync<string>($"select coalesce(jsonb_agg(row order by row::text), '[]'::jsonb)::text from (select {row} as row from {quotedTable} t where session_id = @sessionId) rows", new { sessionId }) ?? "[]";
        }
        return result;
    }

    private static void AssertTablesEqual(Dictionary<string, string> expected, Dictionary<string, string> actual)
    {
        Assert.Equal(expected.Keys, actual.Keys);
        foreach (var table in expected.Keys)
            Assert.True(expected[table] == actual[table], $"Undo did not restore {table}.\nBefore: {expected[table]}\nAfter: {actual[table]}");
    }

    private static async Task AssertErrorAsync(HttpResponseMessage response, HttpStatusCode status, string errorCode)
    {
        Assert.True(response.StatusCode == status, await response.Content.ReadAsStringAsync());
        Assert.Equal(errorCode, (await response.Content.ReadFromJsonAsync<ErrorResponse>())!.ErrorCode);
    }
}
