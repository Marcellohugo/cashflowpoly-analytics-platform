using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Cashflowpoly.Api.Contracts;
using Cashflowpoly.Api.Tests.Infrastructure;
using Xunit;

namespace Cashflowpoly.Api.Tests;

[Collection("ApiIntegration")]
[Trait("Category", "Integration")]
/// <summary>
/// Kelas pengujian integrasi yang memvalidasi alur lengkap ingest event gameplay,
/// perhitungan analitik sesi, riwayat transaksi, pengurutan pemain, dan pembatasan akses.
/// </summary>
public sealed class EventAnalyticsIntegrationTests
{
    private readonly HttpClient _client;

    /// <summary>
    /// Menginisialisasi instance pengujian dengan HttpClient dari fixture integrasi bersama.
    /// </summary>
    public EventAnalyticsIntegrationTests(ApiIntegrationTestFixture fixture)
    {
        _client = fixture.Client;
    }

    [Fact]
    /// <summary>
    /// Memvalidasi alur lengkap: ingest dua event transaksi lalu memverifikasi hasil
    /// analitik sesi (cash in/out/net), data per pemain, riwayat transaksi, dan recompute.
    /// </summary>
    public async Task IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData()
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];
        var instructorUsername = $"it_evt_instructor_{suffix}";
        const string instructorPassword = "IntegrationEventInstructorPass!123";
        var playerUsername = $"it_evt_player_{suffix}";
        const string playerPassword = "IntegrationEventPlayerPass!123";

        var instructorToken = (await RegisterAsync(instructorUsername, instructorPassword, "INSTRUCTOR")).AccessToken;

        var createRulesetPayload = new
        {
            name = $"Ruleset Event IT {suffix}",
            description = "Integration event analytics",
            definition = BuildRulesetDefinition(startingCash: 20)
        };

        var createRulesetResponse = await SendJsonAsync(
            HttpMethod.Post,
            "/api/v1/rulesets",
            createRulesetPayload,
            instructorToken);
        Assert.True(
            createRulesetResponse.StatusCode == HttpStatusCode.Created,
            await createRulesetResponse.Content.ReadAsStringAsync());

        var createdRuleset = await createRulesetResponse.Content.ReadFromJsonAsync<CreateRulesetResponse>();
        Assert.NotNull(createdRuleset);

        var updateRulesetPayload = new
        {
            name = $"Ruleset Event IT {suffix} V2",
            description = "Integration event analytics v2",
            definition = BuildRulesetDefinition(startingCash: 21)
        };
        var updateRulesetResponse = await SendJsonAsync(
            HttpMethod.Put,
            $"/api/v1/rulesets/{createdRuleset.RulesetId}",
            updateRulesetPayload,
            instructorToken);
        Assert.Equal(HttpStatusCode.OK, updateRulesetResponse.StatusCode);

        var updatedRuleset = await updateRulesetResponse.Content.ReadFromJsonAsync<CreateRulesetResponse>();
        Assert.NotNull(updatedRuleset);

        var activateRulesetResponse = await SendJsonAsync(
            HttpMethod.Post,
            $"/api/v1/rulesets/{createdRuleset.RulesetId}/versions/{updatedRuleset.Version}/activate",
            body: null,
            instructorToken);
        Assert.Equal(HttpStatusCode.OK, activateRulesetResponse.StatusCode);

        var createSessionPayload = new
        {
            session_name = $"Session Event IT {suffix}",
            mode = "PEMULA",
            ruleset_version_id = updatedRuleset.RulesetVersionId
        };

        var createSessionResponse = await SendJsonAsync(
            HttpMethod.Post,
            "/api/v1/sessions",
            createSessionPayload,
            instructorToken);
        Assert.Equal(HttpStatusCode.Created, createSessionResponse.StatusCode);

        var createdSession = await createSessionResponse.Content.ReadFromJsonAsync<CreateSessionResponse>();
        Assert.NotNull(createdSession);

        var createPlayerPayload = new
        {
            display_name = $"Player Event {suffix}",
            username = playerUsername,
            password = playerPassword
        };

        var createPlayerResponse = await SendJsonAsync(
            HttpMethod.Post,
            "/api/v1/players",
            createPlayerPayload,
            instructorToken);
        Assert.Equal(HttpStatusCode.Created, createPlayerResponse.StatusCode);

        using var createdPlayerBody = await ReadJsonAsync(createPlayerResponse);
        var createdUserId = createdPlayerBody.RootElement.GetProperty("user_id").GetGuid();
        Assert.NotEqual(Guid.Empty, createdUserId);

        var addPlayerPayload = new
        {
            user_id = createdUserId,
            player_order_no = 1
        };

        var addPlayerResponse = await SendJsonAsync(
            HttpMethod.Post,
            $"/api/v1/sessions/{createdSession.SessionId}/players",
            addPlayerPayload,
            instructorToken);
        Assert.Equal(HttpStatusCode.OK, addPlayerResponse.StatusCode);

        for (var i = 2; i <= 3; i++)
        {
            var extraPlayerResponse = await SendJsonAsync(
                HttpMethod.Post,
                "/api/v1/players",
                new
                {
                    display_name = $"Player Event Extra {i} {suffix}",
                    username = $"it_evt_player_extra_{i}_{suffix}",
                    password = "IntegrationEventExtraPlayerPass!123"
                },
                instructorToken);
            Assert.Equal(HttpStatusCode.Created, extraPlayerResponse.StatusCode);

            var extraPlayer = await extraPlayerResponse.Content.ReadFromJsonAsync<PlayerResponse>();
            Assert.NotNull(extraPlayer);

            var addExtraPlayerResponse = await SendJsonAsync(
                HttpMethod.Post,
                $"/api/v1/sessions/{createdSession.SessionId}/players",
                new
                {
                    user_id = extraPlayer.UserId,
                    player_order_no = i
                },
                instructorToken);
            Assert.Equal(HttpStatusCode.OK, addExtraPlayerResponse.StatusCode);
        }

        var startSessionResponse = await SendJsonAsync(
            HttpMethod.Post,
            $"/api/v1/sessions/{createdSession.SessionId}/start",
            body: null,
            instructorToken);
        Assert.Equal(HttpStatusCode.OK, startSessionResponse.StatusCode);

        var rulesetDetailResponse = await SendJsonAsync(
            HttpMethod.Get,
            $"/api/v1/rulesets/{createdRuleset.RulesetId}",
            body: null,
            instructorToken);
        Assert.Equal(HttpStatusCode.OK, rulesetDetailResponse.StatusCode);

        var rulesetDetail = await rulesetDetailResponse.Content.ReadFromJsonAsync<RulesetDetailResponse>();
        Assert.NotNull(rulesetDetail);

        var activeVersion = rulesetDetail.Versions
            .Where(v => string.Equals(v.Status, "ACTIVE", StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(v => v.Version)
            .First();

        var nextSequence = await GetNextSequenceNumberAsync(createdSession.SessionId, instructorToken);
        using var setupEventsResponse = await SendJsonAsync(
            HttpMethod.Get,
            $"/api/v1/sessions/{createdSession.SessionId}/events?fromSeq=0&limit=100",
            null,
            instructorToken);
        using var setupEventsBody = await ReadJsonAsync(setupEventsResponse);
        var setupEvents = setupEventsBody.RootElement.GetProperty("events")
            .EnumerateArray()
            .Select(item => item.Clone())
            .ToList();
        var actingUserId = setupEvents
            .Single(item =>
                item.GetProperty("action_type").GetString() == "BagikanTieBreaker" &&
                item.GetProperty("payload").GetProperty("number").GetInt32() == 1)
            .GetProperty("user_id")
            .GetGuid();
        var setupIngredientEvents = setupEvents
            .Where(item => item.GetProperty("action_type").GetString() == "SetupBahanAwal")
            .ToList();
        var setupIngredientCost = setupIngredientEvents.Sum(item =>
            item.GetProperty("payload").GetProperty("amount").GetInt32());
        var playerSetupIngredientCost = setupIngredientEvents
            .Single(item => item.GetProperty("user_id").GetGuid() == actingUserId)
            .GetProperty("payload")
            .GetProperty("amount")
            .GetInt32();
        var now = DateTimeOffset.UtcNow;
        var event1Payload = new
        {
            event_id = Guid.NewGuid(),
            session_id = createdSession.SessionId,
            user_id = actingUserId,
            actor_type = "PLAYER",
            turn_number = 1,
            timestamp = now.ToString("O"),
            day_index = 0,
            weekday = "MON",
            action_slot = 1,
            sequence_number = nextSequence,
            action_type = "KerjaLepas",
            ruleset_version_id = activeVersion.RulesetVersionId,
            payload = new
            {
                amount = 1
            }
        };
        var event1Response = await SendJsonAsync(HttpMethod.Post, "/api/v1/events", event1Payload, instructorToken);
        Assert.True(
            event1Response.StatusCode == HttpStatusCode.Created,
            await event1Response.Content.ReadAsStringAsync());

        var event2Payload = new
        {
            event_id = Guid.NewGuid(),
            session_id = createdSession.SessionId,
            user_id = actingUserId,
            actor_type = "PLAYER",
            turn_number = 1,
            timestamp = now.AddSeconds(1).ToString("O"),
            day_index = 0,
            weekday = "MON",
            action_slot = 2,
            sequence_number = nextSequence + 1,
            action_type = "Kebutuhan",
            ruleset_version_id = activeVersion.RulesetVersionId,
            payload = new
            {
                card_id = "buku",
                amount = 3,
                points = 1,
                need_tier = "primer"
            }
        };
        var event2Response = await SendJsonAsync(HttpMethod.Post, "/api/v1/events", event2Payload, instructorToken);
        Assert.True(
            event2Response.StatusCode == HttpStatusCode.Created,
            await event2Response.Content.ReadAsStringAsync());

        var analyticsResponse = await SendJsonAsync(
            HttpMethod.Get,
            $"/api/v1/analytics/sessions/{createdSession.SessionId}",
            body: null,
            instructorToken);
        Assert.Equal(HttpStatusCode.OK, analyticsResponse.StatusCode);

        using var analyticsBody = await ReadJsonAsync(analyticsResponse);
        var analyticsRoot = analyticsBody.RootElement;
        var summary = analyticsRoot.GetProperty("summary");
        Assert.Equal((int)(nextSequence + 2), summary.GetProperty("event_count").GetInt32());
        Assert.Equal(1d, summary.GetProperty("cash_in_total").GetDouble(), 6);
        Assert.Equal(setupIngredientCost + 3d, summary.GetProperty("cash_out_total").GetDouble(), 6);
        Assert.Equal(-(setupIngredientCost + 2d), summary.GetProperty("cashflow_net_total").GetDouble(), 6);

        var byPlayer = analyticsRoot.GetProperty("by_player")
            .EnumerateArray()
            .Single(item => item.GetProperty("user_id").GetGuid() == actingUserId);
        Assert.Equal(1d, byPlayer.GetProperty("cash_in_total").GetDouble(), 6);
        Assert.Equal(playerSetupIngredientCost + 3d, byPlayer.GetProperty("cash_out_total").GetDouble(), 6);
        Assert.Equal(0, byPlayer.GetProperty("orders_completed_count").GetInt32());
        Assert.Equal(1, byPlayer.GetProperty("inventory_ingredient_total").GetInt32());
        Assert.Equal(0, byPlayer.GetProperty("actions_used_total").GetInt32());
        Assert.Equal(1d, byPlayer.GetProperty("compliance_primary_need_rate").GetDouble(), 6);
        Assert.Equal(0, byPlayer.GetProperty("rules_violations_count").GetInt32());

        var transactionsResponse = await SendJsonAsync(
            HttpMethod.Get,
            $"/api/v1/analytics/sessions/{createdSession.SessionId}/transactions?userId={actingUserId}",
            body: null,
            instructorToken);
        Assert.Equal(HttpStatusCode.OK, transactionsResponse.StatusCode);

        var transactions = await transactionsResponse.Content.ReadFromJsonAsync<TransactionHistoryResponse>();
        Assert.NotNull(transactions);
        Assert.Equal(3, transactions.Items.Count);
        Assert.Equal(playerSetupIngredientCost, transactions.Items[0].Amount, 6);
        Assert.Equal(1d, transactions.Items[1].Amount, 6);
        Assert.Equal(3d, transactions.Items[2].Amount, 6);

        var recomputeResponse = await SendJsonAsync(
            HttpMethod.Post,
            $"/api/v1/analytics/sessions/{createdSession.SessionId}/recompute",
            body: null,
            instructorToken);
        Assert.Equal(HttpStatusCode.OK, recomputeResponse.StatusCode);
    }

    [Fact]
    /// <summary>
    /// Memvalidasi bahwa urutan aksi dan analitika sesi yang sudah dimulai
    /// mengikuti hasil pembagian Tie Breaker, bukan urutan pendaftaran awal.
    /// </summary>
    public async Task StartedSession_UsesShuffledTieBreakerAsPlayerTurnOrder()
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];
        var instructorUsername = $"it_evt_order_instructor_{suffix}";
        const string instructorPassword = "IntegrationOrderInstructorPass!123";

        var instructorToken = (await RegisterAsync(instructorUsername, instructorPassword, "INSTRUCTOR")).AccessToken;

        var createRulesetPayload = new
        {
            name = $"Ruleset Order IT {suffix}",
            description = "Integration player order assignment",
            definition = BuildRulesetDefinition(startingCash: 20)
        };

        var createRulesetResponse = await SendJsonAsync(
            HttpMethod.Post,
            "/api/v1/rulesets",
            createRulesetPayload,
            instructorToken);
        Assert.True(
            createRulesetResponse.StatusCode == HttpStatusCode.Created,
            await createRulesetResponse.Content.ReadAsStringAsync());

        var createdRuleset = await createRulesetResponse.Content.ReadFromJsonAsync<CreateRulesetResponse>();
        Assert.NotNull(createdRuleset);

        var createSessionPayload = new
        {
            session_name = $"Session Order IT {suffix}",
            mode = "PEMULA",
            ruleset_version_id = createdRuleset.RulesetVersionId
        };

        var createSessionResponse = await SendJsonAsync(
            HttpMethod.Post,
            "/api/v1/sessions",
            createSessionPayload,
            instructorToken);
        Assert.Equal(HttpStatusCode.Created, createSessionResponse.StatusCode);

        var createdSession = await createSessionResponse.Content.ReadFromJsonAsync<CreateSessionResponse>();
        Assert.NotNull(createdSession);

        var players = new List<PlayerResponse>();
        for (var i = 1; i <= 3; i++)
        {
            var createPlayerPayload = new
            {
                display_name = $"Player Order {i} {suffix}",
                username = $"it_evt_order_player_{i}_{suffix}",
                password = "IntegrationOrderPlayerPass!123"
            };

            var createPlayerResponse = await SendJsonAsync(
                HttpMethod.Post,
                "/api/v1/players",
                createPlayerPayload,
                instructorToken);
            Assert.Equal(HttpStatusCode.Created, createPlayerResponse.StatusCode);

            var createdPlayer = await createPlayerResponse.Content.ReadFromJsonAsync<PlayerResponse>();
            Assert.NotNull(createdPlayer);
            players.Add(createdPlayer);

            var addPlayerPayload = new
            {
                user_id = createdPlayer.UserId,
                player_order_no = i
            };

            var addPlayerResponse = await SendJsonAsync(
                HttpMethod.Post,
                $"/api/v1/sessions/{createdSession.SessionId}/players",
                addPlayerPayload,
                instructorToken);
            Assert.Equal(HttpStatusCode.OK, addPlayerResponse.StatusCode);
        }

        var startSessionResponse = await SendJsonAsync(
            HttpMethod.Post,
            $"/api/v1/sessions/{createdSession.SessionId}/start",
            body: null,
            instructorToken);
        Assert.Equal(HttpStatusCode.OK, startSessionResponse.StatusCode);

        var rulesetDetailResponse = await SendJsonAsync(
            HttpMethod.Get,
            $"/api/v1/rulesets/{createdRuleset.RulesetId}",
            body: null,
            instructorToken);
        Assert.Equal(HttpStatusCode.OK, rulesetDetailResponse.StatusCode);

        var rulesetDetail = await rulesetDetailResponse.Content.ReadFromJsonAsync<RulesetDetailResponse>();
        Assert.NotNull(rulesetDetail);

        var activeVersion = rulesetDetail.Versions
            .Where(v => string.Equals(v.Status, "ACTIVE", StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(v => v.Version)
            .First();

        using var setupEventsResponse = await SendJsonAsync(
            HttpMethod.Get,
            $"/api/v1/sessions/{createdSession.SessionId}/events?fromSeq=0&limit=100",
            null,
            instructorToken);
        using var setupEventsBody = await ReadJsonAsync(setupEventsResponse);
        var playerOrderByUserId = setupEventsBody.RootElement.GetProperty("events")
            .EnumerateArray()
            .Where(item => item.GetProperty("action_type").GetString() == "BagikanTieBreaker")
            .ToDictionary(
                item => item.GetProperty("user_id").GetGuid(),
                item => item.GetProperty("payload").GetProperty("number").GetInt32());
        var playersInTurnOrder = players.OrderBy(player => playerOrderByUserId[player.UserId]).ToList();
        var now = DateTimeOffset.UtcNow;
        long sequence = await GetNextSequenceNumberAsync(createdSession.SessionId, instructorToken);
        foreach (var player in playersInTurnOrder)
        {
            for (var slot = 1; slot <= 2; slot++)
            {
                var eventPayload = new
                {
                    event_id = Guid.NewGuid(),
                    session_id = createdSession.SessionId,
                    user_id = player.UserId,
                    actor_type = "PLAYER",
                    turn_number = playerOrderByUserId[player.UserId],
                    timestamp = now.AddSeconds(sequence).ToString("O"),
                    day_index = 0,
                    weekday = "MON",
                    action_slot = slot,
                    sequence_number = sequence,
                    action_type = "KerjaLepas",
                    ruleset_version_id = activeVersion.RulesetVersionId,
                    payload = new
                    {
                        amount = 1
                    }
                };

                var eventResponse = await SendJsonAsync(HttpMethod.Post, "/api/v1/events", eventPayload, instructorToken);
                Assert.True(
                    eventResponse.StatusCode == HttpStatusCode.Created,
                    await eventResponse.Content.ReadAsStringAsync());
                sequence += 1;
            }
        }

        var analyticsResponse = await SendJsonAsync(
            HttpMethod.Get,
            $"/api/v1/analytics/sessions/{createdSession.SessionId}",
            body: null,
            instructorToken);
        Assert.Equal(HttpStatusCode.OK, analyticsResponse.StatusCode);

        var analytics = await analyticsResponse.Content.ReadFromJsonAsync<AnalyticsSessionResponse>();
        Assert.NotNull(analytics);
        Assert.Equal(3, analytics.ByPlayer.Count);

        var expectedPlayerOrder = playersInTurnOrder.Select(item => item.UserId).ToList();
        var actualPlayerOrder = analytics.ByPlayer.Select(item => item.UserId).ToList();
        var actualPlayerOrders = analytics.ByPlayer.Select(item => item.PlayerOrder).ToList();
        Assert.Equal(expectedPlayerOrder, actualPlayerOrder);
        Assert.Equal(new[] { 1, 2, 3 }, actualPlayerOrders);
    }

    [Fact]
    /// <summary>
    /// Memvalidasi bahwa penambahan pemain via username tanpa player_order_no tetap
    /// mengikuti urutan assignment sesi.
    /// </summary>
    public async Task AddPlayersByUsername_WithoutExplicitOrder_UsesAssignmentOrder()
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];
        var instructorUsername = $"it_evt_assignment_order_instructor_{suffix}";
        const string instructorPassword = "IntegrationAssignmentOrderInstructorPass!123";
        var instructorToken = (await RegisterAsync(instructorUsername, instructorPassword, "INSTRUCTOR")).AccessToken;
        var firstPlayerUsername = $"it_evt_assignment_order_player_1_{suffix}";
        var secondPlayerUsername = $"it_evt_assignment_order_player_2_{suffix}";
        var thirdPlayerUsername = $"it_evt_assignment_order_player_3_{suffix}";

        var createRulesetPayload = new
        {
            name = $"Ruleset Assignment Order IT {suffix}",
            description = "Integration assignment order by username",
            definition = BuildRulesetDefinition(startingCash: 20)
        };

        var createRulesetResponse = await SendJsonAsync(
            HttpMethod.Post,
            "/api/v1/rulesets",
            createRulesetPayload,
            instructorToken);
        Assert.True(
            createRulesetResponse.StatusCode == HttpStatusCode.Created,
            await createRulesetResponse.Content.ReadAsStringAsync());

        var createdRuleset = await createRulesetResponse.Content.ReadFromJsonAsync<CreateRulesetResponse>();
        Assert.NotNull(createdRuleset);

        var createSessionResponse = await SendJsonAsync(
            HttpMethod.Post,
            "/api/v1/sessions",
            new
            {
                session_name = $"Session Assignment Order IT {suffix}",
                mode = "PEMULA",
                ruleset_version_id = createdRuleset.RulesetVersionId
            },
            instructorToken);
        Assert.Equal(HttpStatusCode.Created, createSessionResponse.StatusCode);

        var createdSession = await createSessionResponse.Content.ReadFromJsonAsync<CreateSessionResponse>();
        Assert.NotNull(createdSession);

        var players = new List<(string Username, Guid UserId)>();
        for (var i = 1; i <= 3; i++)
        {
            var username = i switch
            {
                1 => firstPlayerUsername,
                2 => secondPlayerUsername,
                _ => thirdPlayerUsername
            };
            var createPlayerResponse = await SendJsonAsync(
                HttpMethod.Post,
                "/api/v1/players",
                new
                {
                    display_name = $"Player Assignment Order {i} {suffix}",
                    username,
                    password = "IntegrationAssignmentOrderPlayerPass!123"
                },
                instructorToken);
            Assert.Equal(HttpStatusCode.Created, createPlayerResponse.StatusCode);

            var createdPlayer = await createPlayerResponse.Content.ReadFromJsonAsync<PlayerResponse>();
            Assert.NotNull(createdPlayer);
            players.Add((username, createdPlayer.UserId));
        }

        var firstPlayer = players[0];
        var secondPlayer = players[1];
        var thirdPlayer = players[2];

        var addFirst = await SendJsonAsync(
            HttpMethod.Post,
            $"/api/v1/sessions/{createdSession.SessionId}/players",
            new { username = firstPlayer.Username },
            instructorToken);
        Assert.Equal(HttpStatusCode.OK, addFirst.StatusCode);
        var addedFirst = await addFirst.Content.ReadFromJsonAsync<AddSessionPlayerResponse>();
        Assert.NotNull(addedFirst);
        Assert.Equal(1, addedFirst.PlayerOrder);

        var addSecond = await SendJsonAsync(
            HttpMethod.Post,
            $"/api/v1/sessions/{createdSession.SessionId}/players",
            new { username = secondPlayer.Username },
            instructorToken);
        Assert.Equal(HttpStatusCode.OK, addSecond.StatusCode);
        var addedSecond = await addSecond.Content.ReadFromJsonAsync<AddSessionPlayerResponse>();
        Assert.NotNull(addedSecond);
        Assert.Equal(2, addedSecond.PlayerOrder);

        var addThird = await SendJsonAsync(
            HttpMethod.Post,
            $"/api/v1/sessions/{createdSession.SessionId}/players",
            new { username = thirdPlayer.Username },
            instructorToken);
        Assert.Equal(HttpStatusCode.OK, addThird.StatusCode);
        var addedThird = await addThird.Content.ReadFromJsonAsync<AddSessionPlayerResponse>();
        Assert.NotNull(addedThird);
        Assert.Equal(3, addedThird.PlayerOrder);

        var analyticsResponse = await SendJsonAsync(
            HttpMethod.Get,
            $"/api/v1/analytics/sessions/{createdSession.SessionId}",
            body: null,
            instructorToken);
        Assert.Equal(HttpStatusCode.OK, analyticsResponse.StatusCode);

        var analytics = await analyticsResponse.Content.ReadFromJsonAsync<AnalyticsSessionResponse>();
        Assert.NotNull(analytics);
        Assert.Equal(3, analytics.ByPlayer.Count);

        var expectedPlayerOrder = new[] { firstPlayer.UserId, secondPlayer.UserId, thirdPlayer.UserId };
        var actualPlayerOrder = analytics.ByPlayer.Select(item => item.UserId).ToArray();
        var actualPlayerOrders = analytics.ByPlayer.Select(item => item.PlayerOrder).ToArray();

        Assert.Equal(expectedPlayerOrder, actualPlayerOrder);
        Assert.Equal(new[] { 1, 2, 3 }, actualPlayerOrders);
    }

    [Fact]
    /// <summary>
    /// Memvalidasi bahwa penambahan pemain kelima ke sesi ditolak dengan error
    /// DOMAIN_RULE_VIOLATION karena melebihi batas maksimal 4 pemain per sesi.
    /// </summary>
    public async Task AddPlayerToSession_FifthPlayer_IsRejectedWithDomainRuleViolation()
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];
        var instructorUsername = $"it_evt_limit_instructor_{suffix}";
        const string instructorPassword = "IntegrationLimitInstructorPass!123";
        var instructorToken = (await RegisterAsync(instructorUsername, instructorPassword, "INSTRUCTOR")).AccessToken;

        var createRulesetPayload = new
        {
            name = $"Ruleset Limit IT {suffix}",
            description = "Integration max player per session",
            definition = BuildRulesetDefinition(startingCash: 20)
        };

        var createRulesetResponse = await SendJsonAsync(
            HttpMethod.Post,
            "/api/v1/rulesets",
            createRulesetPayload,
            instructorToken);
        Assert.Equal(HttpStatusCode.Created, createRulesetResponse.StatusCode);

        var createdRuleset = await createRulesetResponse.Content.ReadFromJsonAsync<CreateRulesetResponse>();
        Assert.NotNull(createdRuleset);

        var createSessionPayload = new
        {
            session_name = $"Session Limit IT {suffix}",
            mode = "PEMULA",
            ruleset_version_id = createdRuleset.RulesetVersionId
        };

        var createSessionResponse = await SendJsonAsync(
            HttpMethod.Post,
            "/api/v1/sessions",
            createSessionPayload,
            instructorToken);
        Assert.Equal(HttpStatusCode.Created, createSessionResponse.StatusCode);

        var createdSession = await createSessionResponse.Content.ReadFromJsonAsync<CreateSessionResponse>();
        Assert.NotNull(createdSession);

        for (var i = 1; i <= 4; i++)
        {
            var createPlayerPayload = new
            {
                display_name = $"Player Limit {i} {suffix}",
                username = $"it_evt_limit_player_{i}_{suffix}",
                password = "IntegrationLimitPlayerPass!123"
            };

            var createPlayerResponse = await SendJsonAsync(
                HttpMethod.Post,
                "/api/v1/players",
                createPlayerPayload,
                instructorToken);
            Assert.Equal(HttpStatusCode.Created, createPlayerResponse.StatusCode);

            var createdPlayer = await createPlayerResponse.Content.ReadFromJsonAsync<PlayerResponse>();
            Assert.NotNull(createdPlayer);

            var addPlayerResponse = await SendJsonAsync(
                HttpMethod.Post,
                $"/api/v1/sessions/{createdSession.SessionId}/players",
                new
                {
                    user_id = createdPlayer.UserId,
                    player_order_no = i
                },
                instructorToken);
            Assert.Equal(HttpStatusCode.OK, addPlayerResponse.StatusCode);
        }

        var createFifthPlayerResponse = await SendJsonAsync(
            HttpMethod.Post,
            "/api/v1/players",
            new
            {
                display_name = $"Player Limit 5 {suffix}",
                username = $"it_evt_limit_player_5_{suffix}",
                password = "IntegrationLimitPlayerPass!123"
            },
            instructorToken);
        Assert.Equal(HttpStatusCode.Created, createFifthPlayerResponse.StatusCode);

        var fifthPlayer = await createFifthPlayerResponse.Content.ReadFromJsonAsync<PlayerResponse>();
        Assert.NotNull(fifthPlayer);

        var addFifthPlayerResponse = await SendJsonAsync(
            HttpMethod.Post,
            $"/api/v1/sessions/{createdSession.SessionId}/players",
            new
            {
                user_id = fifthPlayer.UserId,
                player_order_no = 5
            },
            instructorToken);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, addFifthPlayerResponse.StatusCode);

        var addFifthPlayerError = await addFifthPlayerResponse.Content.ReadFromJsonAsync<ErrorResponse>();
        Assert.NotNull(addFifthPlayerError);
        Assert.Equal("DOMAIN_RULE_VIOLATION", addFifthPlayerError.ErrorCode);

        var startSessionResponse = await SendJsonAsync(
            HttpMethod.Post,
            $"/api/v1/sessions/{createdSession.SessionId}/start",
            body: null,
            instructorToken);
        Assert.Equal(HttpStatusCode.OK, startSessionResponse.StatusCode);
    }

    [Fact]
    /// <summary>
    /// Memvalidasi bahwa path API tanpa versi (/api/...) mengembalikan 404 Not Found
    /// dan path versioned (/api/v1/...) berfungsi dengan benar.
    /// </summary>
    public async Task UnversionedApiRoute_ReturnsNotFound_AndV1AuthWorks()
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];
        var username = $"it_unversioned_{suffix}";
        const string password = "UnversionedRoutePass!123";

        var registerPayload = new RegisterRequest(username, password, "INSTRUCTOR", null);
        var unversionedRegisterResponse = await _client.PostAsJsonAsync("/api/auth/register", registerPayload);
        Assert.Equal(HttpStatusCode.NotFound, unversionedRegisterResponse.StatusCode);

        var registerResponse = await _client.PostAsJsonAsync("/api/v1/auth/register", registerPayload);
        Assert.Equal(HttpStatusCode.Created, registerResponse.StatusCode);

        var loginPayload = new LoginRequest(username, password);
        var loginResponse = await _client.PostAsJsonAsync("/api/v1/auth/login", loginPayload);
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var login = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.NotNull(login);
        Assert.False(string.IsNullOrWhiteSpace(login.AccessToken));

        var unversionedSessionsResponse = await SendJsonAsync(HttpMethod.Get, "/api/sessions", null, login.AccessToken);
        Assert.Equal(HttpStatusCode.NotFound, unversionedSessionsResponse.StatusCode);

        var sessionsResponse = await SendJsonAsync(HttpMethod.Get, "/api/v1/sessions", null, login.AccessToken);
        Assert.Equal(HttpStatusCode.OK, sessionsResponse.StatusCode);
    }

    [Fact]
    /// <summary>
    /// Memvalidasi bahwa ingest event dengan payload tidak valid (amount bukan angka,
    /// day_index negatif) dan query parameter tidak valid mengembalikan 400 Bad Request.
    /// </summary>
    public async Task IngestEvent_InvalidPayloadAndQuery_ReturnsBadRequest()
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];
        var instructorUsername = $"it_evt_invalid_instr_{suffix}";
        const string instructorPassword = "IntegrationInvalidInstructorPass!123";
        var instructorToken = (await RegisterAsync(instructorUsername, instructorPassword, "INSTRUCTOR")).AccessToken;
        var setup = await CreateReadySessionAsync(instructorToken, suffix);

        var now = DateTimeOffset.UtcNow;
        var invalidAmountPayload = new
        {
            event_id = Guid.NewGuid(),
            session_id = setup.SessionId,
            user_id = setup.ActingUserId,
            actor_type = "PLAYER",
            timestamp = now.ToString("O"),
            day_index = 0,
            weekday = "MON",
            action_slot = 1,
            sequence_number = 1,
            action_type = "CatatTransaksi",
            ruleset_version_id = setup.RulesetVersionId,
            payload = new
            {
                direction = "IN",
                amount = "invalid-number",
                category = "NEED_PRIMARY"
            }
        };

        var invalidAmountResponse = await SendJsonAsync(HttpMethod.Post, "/api/v1/events", invalidAmountPayload, instructorToken);
        Assert.Equal(HttpStatusCode.BadRequest, invalidAmountResponse.StatusCode);

        var invalidDayIndexPayload = new
        {
            event_id = Guid.NewGuid(),
            session_id = setup.SessionId,
            user_id = setup.ActingUserId,
            actor_type = "PLAYER",
            timestamp = now.AddSeconds(1).ToString("O"),
            day_index = -1,
            weekday = "MON",
            action_slot = 1,
            sequence_number = 1,
            action_type = "CatatTransaksi",
            ruleset_version_id = setup.RulesetVersionId,
            payload = new
            {
                direction = "IN",
                amount = 1,
                category = "NEED_PRIMARY"
            }
        };

        var invalidDayResponse = await SendJsonAsync(HttpMethod.Post, "/api/v1/events", invalidDayIndexPayload, instructorToken);
        Assert.Equal(HttpStatusCode.BadRequest, invalidDayResponse.StatusCode);

        var invalidQueryResponse = await SendJsonAsync(
            HttpMethod.Get,
            $"/api/v1/sessions/{setup.SessionId}/events?fromSeq=0&limit=-1",
            null,
            instructorToken);
        Assert.Equal(HttpStatusCode.BadRequest, invalidQueryResponse.StatusCode);
    }

    [Fact]
    public async Task FridayDonations_RemainSealedUntilEveryPlayerSubmits()
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];
        var instructorToken = (await RegisterAsync(
            $"it_donation_instructor_{suffix}",
            "IntegrationDonationInstructorPass!123",
            "INSTRUCTOR")).AccessToken;
        var setup = await CreateReadySessionAsync(instructorToken, $"donation_{suffix}");

        async Task<List<JsonElement>> GetEventsAsync()
        {
            using var response = await SendJsonAsync(
                HttpMethod.Get,
                $"/api/v1/sessions/{setup.SessionId}/events?fromSeq=0&limit=100",
                null,
                instructorToken);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            using var body = await ReadJsonAsync(response);
            return body.RootElement.GetProperty("events")
                .EnumerateArray()
                .Select(item => item.Clone())
                .ToList();
        }

        var setupEvents = await GetEventsAsync();
        var players = setupEvents
            .Where(item => item.GetProperty("action_type").GetString() == "BagikanTieBreaker")
            .OrderBy(item => item.GetProperty("payload").GetProperty("number").GetInt32())
            .Select(item => (
                UserId: item.GetProperty("user_id").GetGuid(),
                Turn: item.GetProperty("payload").GetProperty("number").GetInt32()))
            .ToList();
        Assert.Equal(3, players.Count);

        for (var index = 0; index < players.Count; index++)
        {
            var player = players[index];
            using var donationResponse = await SendJsonAsync(HttpMethod.Post, "/api/v1/events", new
            {
                event_id = Guid.NewGuid(),
                session_id = setup.SessionId,
                user_id = player.UserId,
                actor_type = "PLAYER",
                timestamp = DateTimeOffset.UtcNow.AddSeconds(index),
                day_index = 5,
                weekday = "FRI",
                turn_number = player.Turn,
                action_slot = 0,
                sequence_number = setup.NextSequenceNumber + index,
                action_type = "JumatBerkah",
                ruleset_version_id = setup.RulesetVersionId,
                payload = new { amount = index + 1 }
            }, instructorToken);
            Assert.True(
                donationResponse.StatusCode == HttpStatusCode.Created,
                await donationResponse.Content.ReadAsStringAsync());

            var donationEvents = (await GetEventsAsync())
                .Where(item => item.GetProperty("action_type").GetString() == "JumatBerkah")
                .ToList();
            Assert.Equal(index + 1, donationEvents.Count);
            if (index < players.Count - 1)
            {
                Assert.All(donationEvents, item =>
                {
                    var payload = item.GetProperty("payload");
                    Assert.Equal("SEALED", payload.GetProperty("status").GetString());
                    Assert.False(payload.TryGetProperty("amount", out _));
                });
            }
            else
            {
                Assert.All(donationEvents, item =>
                {
                    var payload = item.GetProperty("payload");
                    Assert.True(payload.GetProperty("amount").GetInt32() > 0);
                    Assert.False(payload.TryGetProperty("status", out _));
                });
            }
        }
    }

    [Fact]
    public async Task MarketRefill_IsRejectedBeforeSecondAction_AndAppliedAtomicallyAfterIt()
    {
        var suffix = $"market_refill_{Guid.NewGuid():N}";
        var instructorToken = (await RegisterAsync(
            $"it_market_instructor_{Guid.NewGuid():N}",
            "IntegrationMarketInstructorPass!123",
            "INSTRUCTOR")).AccessToken;
        var setup = await CreateReadySessionAsync(instructorToken, suffix);
        var now = DateTimeOffset.UtcNow;

        using var setupEventsResponse = await SendJsonAsync(
            HttpMethod.Get,
            $"/api/v1/sessions/{setup.SessionId}/events?fromSeq=0&limit=100",
            null,
            instructorToken);
        using var setupEventsBody = await ReadJsonAsync(setupEventsResponse);
        var marketCard = setupEventsBody.RootElement.GetProperty("events")
            .EnumerateArray()
            .First(item =>
                item.GetProperty("action_type").GetString() == "AmbilKartuDariDeck" &&
                item.GetProperty("payload").GetProperty("slot_group").GetString() == "INGREDIENT_MARKET");
        var marketPayload = marketCard.GetProperty("payload");
        var cardId = marketPayload.GetProperty("asset_code").GetString()!;
        var slotCode = marketPayload.GetProperty("slot_code").GetString()!;
        var prices = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            ["nasi_putih"] = 1,
            ["telur"] = 4,
            ["sayur"] = 2
        };
        var purchaseEventId = Guid.NewGuid();

        using var purchaseResponse = await SendJsonAsync(HttpMethod.Post, "/api/v1/events", new
        {
            event_id = purchaseEventId,
            session_id = setup.SessionId,
            user_id = setup.ActingUserId,
            actor_type = "PLAYER",
            timestamp = now,
            day_index = 1,
            weekday = "MON",
            turn_number = 1,
            action_slot = 1,
            sequence_number = setup.NextSequenceNumber,
            action_type = "BahanMasakan",
            ruleset_version_id = setup.RulesetVersionId,
            payload = new { card_id = cardId, amount = prices[cardId] }
        }, instructorToken);
        Assert.True(purchaseResponse.StatusCode == HttpStatusCode.Created, await purchaseResponse.Content.ReadAsStringAsync());

        using var prematureRefillResponse = await SendJsonAsync(HttpMethod.Post, "/api/v1/events", new
        {
            event_id = Guid.NewGuid(),
            session_id = setup.SessionId,
            user_id = (Guid?)null,
            actor_type = "SYSTEM",
            timestamp = now.AddSeconds(1),
            day_index = 1,
            weekday = "MON",
            turn_number = 0,
            action_slot = 0,
            sequence_number = setup.NextSequenceNumber + 1,
            action_type = "IsiUlangPasar",
            ruleset_version_id = setup.RulesetVersionId,
            payload = new
            {
                slot_group = "INGREDIENT_MARKET",
                slot_code = slotCode,
                asset_type = "INGREDIENT",
                asset_code = cardId
            }
        }, instructorToken);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, prematureRefillResponse.StatusCode);

        var secondActionEventId = Guid.NewGuid();
        using var secondActionResponse = await SendJsonAsync(HttpMethod.Post, "/api/v1/events", new
        {
            event_id = secondActionEventId,
            session_id = setup.SessionId,
            user_id = setup.ActingUserId,
            actor_type = "PLAYER",
            timestamp = now.AddSeconds(2),
            day_index = 1,
            weekday = "MON",
            turn_number = 1,
            action_slot = 2,
            sequence_number = setup.NextSequenceNumber + 1,
            action_type = "KerjaLepas",
            ruleset_version_id = setup.RulesetVersionId,
            payload = new { amount = 1 }
        }, instructorToken);
        Assert.True(secondActionResponse.StatusCode == HttpStatusCode.Created, await secondActionResponse.Content.ReadAsStringAsync());

        using var eventsResponse = await SendJsonAsync(
            HttpMethod.Get,
            $"/api/v1/sessions/{setup.SessionId}/events?fromSeq={setup.NextSequenceNumber}&limit=10",
            null,
            instructorToken);
        using var eventsBody = await ReadJsonAsync(eventsResponse);
        var secondAction = Assert.Single(
            eventsBody.RootElement.GetProperty("events").EnumerateArray(),
            item => item.GetProperty("event_id").GetGuid() == secondActionEventId);
        var refills = secondAction.GetProperty("payload").GetProperty("market_refills").EnumerateArray().ToList();
        var refill = Assert.Single(refills, item =>
            item.GetProperty("slot_group").GetString() == "INGREDIENT_MARKET" &&
            item.GetProperty("slot_code").GetString() == slotCode);
        Assert.False(string.IsNullOrWhiteSpace(refill.GetProperty("asset_code").GetString()));
    }

    [Fact]
    public async Task MahirRisk_WithInsufficientCash_IsResolvedByCatalogEmergencyLoan()
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];
        var instructorToken = (await RegisterAsync(
            $"it_risk_instructor_{suffix}",
            "IntegrationRiskInstructorPass!123",
            "INSTRUCTOR")).AccessToken;
        var setup = await CreateReadySessionAsync(
            instructorToken,
            $"risk_{suffix}",
            BuildRulesetDefinition(startingCash: 5, mode: "MAHIR", riskAmount: 20));
        var orderEventId = Guid.NewGuid();
        var riskEventId = Guid.NewGuid();
        var now = DateTimeOffset.UtcNow;
        var sequence = setup.NextSequenceNumber;

        var orderResponse = await SendJsonAsync(HttpMethod.Post, "/api/v1/events", new
        {
            event_id = orderEventId,
            session_id = setup.SessionId,
            user_id = setup.ActingUserId,
            actor_type = "PLAYER",
            timestamp = now,
            day_index = 0,
            weekday = "MON",
            turn_number = 1,
            action_slot = 1,
            sequence_number = sequence,
            action_type = "JualMasakan",
            ruleset_version_id = setup.RulesetVersionId,
            payload = new { order_card_id = "nasi_goreng" }
        }, instructorToken);
        Assert.Equal(HttpStatusCode.Created, orderResponse.StatusCode);

        var riskResponse = await SendJsonAsync(HttpMethod.Post, "/api/v1/events", new
        {
            event_id = riskEventId,
            session_id = setup.SessionId,
            user_id = setup.ActingUserId,
            actor_type = "PLAYER",
            timestamp = now.AddSeconds(1),
            day_index = 0,
            weekday = "MON",
            turn_number = 1,
            action_slot = 0,
            sequence_number = sequence + 1,
            action_type = "RisikoKehidupan",
            ruleset_version_id = setup.RulesetVersionId,
            payload = new
            {
                risk_id = "risk_cost_4",
                source_order_event_id = orderEventId
            }
        }, instructorToken);
        Assert.Equal(HttpStatusCode.Created, riskResponse.StatusCode);

        var pendingTurnEnd = await SendJsonAsync(HttpMethod.Post, "/api/v1/events", new
        {
            event_id = Guid.NewGuid(),
            session_id = setup.SessionId,
            user_id = (Guid?)null,
            actor_type = "SYSTEM",
            timestamp = now.AddSeconds(2),
            day_index = 0,
            weekday = "MON",
            turn_number = 0,
            action_slot = 0,
            sequence_number = sequence + 2,
            action_type = "AkhirGiliran",
            ruleset_version_id = setup.RulesetVersionId,
            payload = new { }
        }, instructorToken);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, pendingTurnEnd.StatusCode);

        var emergencyResponse = await SendJsonAsync(HttpMethod.Post, "/api/v1/events", new
        {
            event_id = Guid.NewGuid(),
            session_id = setup.SessionId,
            user_id = setup.ActingUserId,
            actor_type = "PLAYER",
            timestamp = now.AddSeconds(2),
            day_index = 0,
            weekday = "MON",
            turn_number = 1,
            action_slot = 0,
            sequence_number = sequence + 2,
            action_type = "GunakanOpsiDarurat",
            ruleset_version_id = setup.RulesetVersionId,
            payload = new
            {
                risk_event_id = riskEventId,
                option_type = "TAKE_SHARIA_LOAN",
                loan_code = "loan_syariah_10",
                direction = "IN",
                amount = 100
            }
        }, instructorToken);
        Assert.True(emergencyResponse.StatusCode == HttpStatusCode.Created, await emergencyResponse.Content.ReadAsStringAsync());

        var duplicateLoanResponse = await SendJsonAsync(HttpMethod.Post, "/api/v1/events", new
        {
            event_id = Guid.NewGuid(),
            session_id = setup.SessionId,
            user_id = setup.ActingUserId,
            actor_type = "PLAYER",
            timestamp = now.AddSeconds(3),
            day_index = 0,
            weekday = "MON",
            turn_number = 1,
            action_slot = 2,
            sequence_number = sequence + 3,
            action_type = "PinjamanSyariah",
            ruleset_version_id = setup.RulesetVersionId,
            payload = new
            {
                loan_id = "loan_syariah_10",
                loan_code = "loan_syariah_10",
                principal = 10,
                repayment_amount = 10,
                duration_days = 1,
                penalty_points = 10
            }
        }, instructorToken);
        Assert.True(
            duplicateLoanResponse.StatusCode == HttpStatusCode.Created,
            await duplicateLoanResponse.Content.ReadAsStringAsync());

        var resolvedTurnEnd = await SendJsonAsync(HttpMethod.Post, "/api/v1/events", new
        {
            event_id = Guid.NewGuid(),
            session_id = setup.SessionId,
            user_id = (Guid?)null,
            actor_type = "SYSTEM",
            timestamp = now.AddSeconds(4),
            day_index = 0,
            weekday = "MON",
            turn_number = 0,
            action_slot = 0,
            sequence_number = sequence + 4,
            action_type = "AkhirGiliran",
            ruleset_version_id = setup.RulesetVersionId,
            payload = new { }
        }, instructorToken);
        Assert.Equal(HttpStatusCode.Created, resolvedTurnEnd.StatusCode);

        var transactionsResponse = await SendJsonAsync(
            HttpMethod.Get,
            $"/api/v1/analytics/sessions/{setup.SessionId}/transactions?userId={setup.ActingUserId}",
            null,
            instructorToken);
        var transactions = await transactionsResponse.Content.ReadFromJsonAsync<TransactionHistoryResponse>();
        Assert.NotNull(transactions);
        Assert.Contains(transactions.Items, item => item.Category == "EMERGENCY_OPTION" && item.Direction == "IN" && item.Amount == 10);
        Assert.Contains(transactions.Items, item => item.Category == "RISK_LIFE" && item.Direction == "OUT" && item.Amount == 20);
        Assert.DoesNotContain(transactions.Items, item => item.Amount == 100);
    }

    [Fact]
    public async Task MahirInsurance_CannotBeUsedAfterPolicyBecomesInactive()
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];
        var instructorToken = (await RegisterAsync(
            $"it_insurance_instructor_{suffix}",
            "IntegrationInsuranceInstructorPass!123",
            "INSTRUCTOR")).AccessToken;
        var setup = await CreateReadySessionAsync(
            instructorToken,
            $"insurance_{suffix}",
            BuildRulesetDefinition(startingCash: 15, mode: "MAHIR", riskAmount: 10));
        var sequence = setup.NextSequenceNumber - 1;
        var now = DateTimeOffset.UtcNow;

        async Task<HttpResponseMessage> SendEventAsync(
            string actionType,
            int dayIndex,
            int actionSlot,
            object payload,
            Guid? eventId = null)
        {
            sequence++;
            return await SendJsonAsync(HttpMethod.Post, "/api/v1/events", new
            {
                event_id = eventId ?? Guid.NewGuid(),
                session_id = setup.SessionId,
                user_id = setup.ActingUserId,
                actor_type = "PLAYER",
                timestamp = now.AddSeconds(sequence),
                day_index = dayIndex,
                weekday = "MON",
                turn_number = 1,
                action_slot = actionSlot,
                sequence_number = sequence,
                action_type = actionType,
                ruleset_version_id = setup.RulesetVersionId,
                payload
            }, instructorToken);
        }

        var firstOrderId = Guid.NewGuid();
        Assert.Equal(HttpStatusCode.Created, (await SendEventAsync(
            "JualMasakan", 0, 1, new { order_card_id = "nasi_goreng" }, firstOrderId)).StatusCode);
        var firstRiskId = Guid.NewGuid();
        Assert.Equal(HttpStatusCode.Created, (await SendEventAsync(
            "RisikoKehidupan",
            0,
            0,
            new { risk_id = "risk_cost_4", source_order_event_id = firstOrderId },
            firstRiskId)).StatusCode);
        Assert.Equal(HttpStatusCode.Created, (await SendEventAsync(
            "Asuransi",
            0,
            0,
            new { risk_event_id = firstRiskId })).StatusCode);
        Assert.Equal(HttpStatusCode.Created, (await SendEventAsync(
            "KerjaLepas", 0, 2, new { amount = 1 })).StatusCode);

        var secondOrderId = Guid.NewGuid();
        Assert.Equal(HttpStatusCode.Created, (await SendEventAsync(
            "JualMasakan", 1, 1, new { order_card_id = "nasi_goreng" }, secondOrderId)).StatusCode);
        var secondRiskId = Guid.NewGuid();
        Assert.Equal(HttpStatusCode.Created, (await SendEventAsync(
            "RisikoKehidupan",
            1,
            0,
            new { risk_id = "risk_cost_4", source_order_event_id = secondOrderId },
            secondRiskId)).StatusCode);

        var secondUse = await SendEventAsync("Asuransi", 1, 0, new { risk_event_id = secondRiskId });
        Assert.Equal(HttpStatusCode.UnprocessableEntity, secondUse.StatusCode);
        sequence--;
        Assert.Equal(HttpStatusCode.Created, (await SendEventAsync(
            "BayarRisiko", 1, 0, new { risk_event_id = secondRiskId })).StatusCode);

        var transactionsResponse = await SendJsonAsync(
            HttpMethod.Get,
            $"/api/v1/analytics/sessions/{setup.SessionId}/transactions?userId={setup.ActingUserId}",
            null,
            instructorToken);
        var transactions = await transactionsResponse.Content.ReadFromJsonAsync<TransactionHistoryResponse>();
        Assert.NotNull(transactions);
        Assert.Single(transactions.Items, item => item.Category == "INSURANCE_OFFSET" && item.Amount == 10);
        Assert.DoesNotContain(transactions.Items, item => item.Category == "EMERGENCY_OPTION" && item.Amount == 10);
    }

    [Fact]
    public async Task MahirGoldSell_UsesRelationalHoldingIncludingInitialGold()
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];
        var instructorToken = (await RegisterAsync(
            $"it_gold_instructor_{suffix}",
            "IntegrationGoldInstructorPass!123",
            "INSTRUCTOR")).AccessToken;
        var setup = await CreateReadySessionAsync(
            instructorToken,
            $"gold_{suffix}",
            BuildRulesetDefinition(startingCash: 10, mode: "MAHIR"));
        var now = DateTimeOffset.UtcNow;
        var nextSequence = setup.NextSequenceNumber;

        async Task<HttpResponseMessage> SendEventAsync(
            string actionType,
            string actorType,
            Guid? userId,
            long sequence,
            object payload)
        {
            return await SendJsonAsync(HttpMethod.Post, "/api/v1/events", new
            {
                event_id = Guid.NewGuid(),
                session_id = setup.SessionId,
                user_id = userId,
                actor_type = actorType,
                timestamp = now.AddSeconds(sequence),
                day_index = 6,
                weekday = "SAT",
                turn_number = actorType == "SYSTEM" ? 0 : 1,
                action_slot = 0,
                sequence_number = sequence,
                action_type = actionType,
                ruleset_version_id = setup.RulesetVersionId,
                payload
            }, instructorToken);
        }

        Assert.Equal(HttpStatusCode.Created, (await SendEventAsync(
            "BukaHargaEmas", "SYSTEM", null, nextSequence, new { gold_price = 6 })).StatusCode);
        var firstSell = await SendEventAsync(
            "JualEmas", "PLAYER", setup.ActingUserId, nextSequence + 1, new { trade_type = "SELL", qty = 1, unit_price = 6, amount = 6, asset_code = "gold_card" });
        Assert.True(firstSell.StatusCode == HttpStatusCode.Created, await firstSell.Content.ReadAsStringAsync());

        var secondSell = await SendEventAsync(
            "JualEmas", "PLAYER", setup.ActingUserId, nextSequence + 2, new { trade_type = "SELL", qty = 1, unit_price = 6, amount = 6, asset_code = "gold_card" });
        Assert.Equal(HttpStatusCode.UnprocessableEntity, secondSell.StatusCode);
    }

    [Fact]
    public async Task Player_SeesOnlyOwnMissionUntilSessionEnds()
    {
        var suffix = $"mission_{Guid.NewGuid():N}"[..17];
        var instructorToken = (await RegisterAsync(
            $"it_mission_instructor_{Guid.NewGuid():N}",
            "IntegrationMissionInstructorPass!123",
            "INSTRUCTOR")).AccessToken;
        var setup = await CreateReadySessionAsync(instructorToken, suffix);
        var playerToken = (await LoginAsync(
            $"it_evt_invalid_player_{suffix}",
            "IntegrationInvalidPlayerPass!123")).AccessToken;

        async Task<List<JsonElement>> ReadMissionsAsync(string token)
        {
            using var response = await SendJsonAsync(
                HttpMethod.Get,
                $"/api/v1/sessions/{setup.SessionId}/events?fromSeq=0&limit=100",
                null,
                token);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            using var body = await ReadJsonAsync(response);
            return body.RootElement.GetProperty("events")
                .EnumerateArray()
                .Where(item => item.GetProperty("action_type").GetString() == "SetupMisiAwal")
                .Select(item => item.Clone())
                .ToList();
        }

        var playerMissions = await ReadMissionsAsync(playerToken);
        Assert.Equal(3, playerMissions.Count);
        var ownMission = Assert.Single(
            playerMissions,
            item => item.GetProperty("user_id").GetGuid() == setup.UserId);
        Assert.True(ownMission.GetProperty("payload").TryGetProperty("mission_id", out _));
        Assert.All(
            playerMissions.Where(item => item.GetProperty("user_id").GetGuid() != setup.UserId),
            item =>
            {
                var payload = item.GetProperty("payload");
                Assert.Equal("HIDDEN", payload.GetProperty("status").GetString());
                Assert.False(payload.TryGetProperty("mission_id", out _));
            });

        Assert.All(
            await ReadMissionsAsync(instructorToken),
            item => Assert.True(item.GetProperty("payload").TryGetProperty("mission_id", out _)));

        using var endResponse = await SendJsonAsync(
            HttpMethod.Post,
            $"/api/v1/sessions/{setup.SessionId}/end",
            null,
            instructorToken);
        Assert.Equal(HttpStatusCode.OK, endResponse.StatusCode);
        Assert.All(
            await ReadMissionsAsync(playerToken),
            item => Assert.True(item.GetProperty("payload").TryGetProperty("mission_id", out _)));
    }

    [Fact]
    /// <summary>
    /// Memvalidasi bahwa PLAYER tidak dapat mengakses event atau mengirim event ke sesi
    /// yang bukan miliknya, dan menerima error FORBIDDEN.
    /// </summary>
    public async Task Player_CannotAccessOrIngestEvents_OnForeignSession()
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];
        var instructorAUsername = $"it_evt_scope_instructor_a_{suffix}";
        var instructorBUsername = $"it_evt_scope_instructor_b_{suffix}";
        const string instructorPassword = "IntegrationScopeInstructorPass!123";
        const string playerPassword = "IntegrationInvalidPlayerPass!123";
        var ownScopeSuffix = $"scope_own_{suffix}";
        var foreignScopeSuffix = $"scope_foreign_{suffix}";

        var instructorAToken = (await RegisterAsync(instructorAUsername, instructorPassword, "INSTRUCTOR")).AccessToken;
        var instructorBToken = (await RegisterAsync(instructorBUsername, instructorPassword, "INSTRUCTOR")).AccessToken;

        var ownSession = await CreateReadySessionAsync(instructorAToken, ownScopeSuffix);
        var foreignSession = await CreateReadySessionAsync(instructorBToken, foreignScopeSuffix);

        var playerUsername = $"it_evt_invalid_player_{ownScopeSuffix}";
        var playerToken = (await LoginAsync(playerUsername, playerPassword)).AccessToken;

        var ownEventsResponse = await SendJsonAsync(
            HttpMethod.Get,
            $"/api/v1/sessions/{ownSession.SessionId}/events?fromSeq=0&limit=10",
            null,
            playerToken);
        Assert.Equal(HttpStatusCode.OK, ownEventsResponse.StatusCode);

        var foreignEventsResponse = await SendJsonAsync(
            HttpMethod.Get,
            $"/api/v1/sessions/{foreignSession.SessionId}/events?fromSeq=0&limit=10",
            null,
            playerToken);
        Assert.Equal(HttpStatusCode.Forbidden, foreignEventsResponse.StatusCode);

        var foreignEventsError = await foreignEventsResponse.Content.ReadFromJsonAsync<ErrorResponse>();
        Assert.NotNull(foreignEventsError);
        Assert.Equal("FORBIDDEN", foreignEventsError.ErrorCode);

        var now = DateTimeOffset.UtcNow;
        var foreignSystemEventPayload = new
        {
            event_id = Guid.NewGuid(),
            session_id = foreignSession.SessionId,
            user_id = (Guid?)null,
            actor_type = "SYSTEM",
            timestamp = now.ToString("O"),
            day_index = 0,
            weekday = "MON",
            action_slot = 1,
            sequence_number = 1,
            action_type = "CatatTransaksi",
            ruleset_version_id = foreignSession.RulesetVersionId,
            payload = new
            {
                direction = "IN",
                amount = 1,
                category = "NEED_PRIMARY",
                counterparty = "BANK"
            }
        };

        var foreignIngestResponse = await SendJsonAsync(
            HttpMethod.Post,
            "/api/v1/events",
            foreignSystemEventPayload,
            playerToken);
        Assert.Equal(HttpStatusCode.Forbidden, foreignIngestResponse.StatusCode);

        var foreignIngestError = await foreignIngestResponse.Content.ReadFromJsonAsync<ErrorResponse>();
        Assert.NotNull(foreignIngestError);
        Assert.Equal("FORBIDDEN", foreignIngestError.ErrorCode);
    }

    [Fact]
    /// <summary>
    /// Memvalidasi bahwa penambahan pemain dengan role tidak valid (bukan PLAYER/OBSERVER)
    /// ditolak dengan error VALIDATION_ERROR dan detail field "role" INVALID_ENUM.
    /// </summary>
    public async Task AddPlayerToSession_ExtraRoleField_IsIgnoredByParticipantContract()
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];
        var instructorUsername = $"it_evt_role_guard_instructor_{suffix}";
        const string instructorPassword = "IntegrationRoleGuardInstructorPass!123";
        var instructorToken = (await RegisterAsync(instructorUsername, instructorPassword, "INSTRUCTOR")).AccessToken;

        var setup = await CreateReadySessionAsync(instructorToken, $"role_guard_{suffix}");
        var createPlayerResponse = await SendJsonAsync(
            HttpMethod.Post,
            "/api/v1/players",
            new
            {
                display_name = $"Player Role Guard {suffix}",
                username = $"it_evt_role_guard_player_{suffix}",
                password = "IntegrationRoleGuardPlayerPass!123"
            },
            instructorToken);
        Assert.Equal(HttpStatusCode.Created, createPlayerResponse.StatusCode);

        var createdPlayer = await createPlayerResponse.Content.ReadFromJsonAsync<PlayerResponse>();
        Assert.NotNull(createdPlayer);

        var addWithInvalidRoleResponse = await SendJsonAsync(
            HttpMethod.Post,
            $"/api/v1/sessions/{setup.SessionId}/players",
            new
            {
                user_id = createdPlayer.UserId,
                role = "ADMIN"
            },
            instructorToken);
        Assert.Equal(HttpStatusCode.OK, addWithInvalidRoleResponse.StatusCode);

        var added = await addWithInvalidRoleResponse.Content.ReadFromJsonAsync<AddSessionPlayerResponse>();
        Assert.NotNull(added);
        Assert.Equal(createdPlayer.UserId, added.UserId);
    }

    /// <summary>
    /// Helper yang membuat ruleset, sesi, player, dan menjalankan sesi hingga siap
    /// untuk menerima event, lalu mengembalikan ID sesi, player, dan versi ruleset aktif.
    /// </summary>
    private async Task<(Guid SessionId, Guid UserId, Guid ActingUserId, Guid RulesetVersionId, long NextSequenceNumber)> CreateReadySessionAsync(
        string instructorToken,
        string suffix,
        RulesetDefinitionDto? definition = null)
    {
        definition ??= BuildRulesetDefinition(startingCash: 50);
        var createRulesetPayload = new
        {
            name = $"Ruleset Invalid IT {suffix}",
            description = "Integration invalid event validation",
            definition
        };

        var createRulesetResponse = await SendJsonAsync(
            HttpMethod.Post,
            "/api/v1/rulesets",
            createRulesetPayload,
            instructorToken);
        Assert.Equal(HttpStatusCode.Created, createRulesetResponse.StatusCode);

        var createdRuleset = await createRulesetResponse.Content.ReadFromJsonAsync<CreateRulesetResponse>();
        Assert.NotNull(createdRuleset);

        var createSessionPayload = new
        {
            session_name = $"Session Invalid IT {suffix}",
            mode = definition.Mode,
            ruleset_version_id = createdRuleset.RulesetVersionId
        };

        var createSessionResponse = await SendJsonAsync(
            HttpMethod.Post,
            "/api/v1/sessions",
            createSessionPayload,
            instructorToken);
        Assert.Equal(HttpStatusCode.Created, createSessionResponse.StatusCode);

        var createdSession = await createSessionResponse.Content.ReadFromJsonAsync<CreateSessionResponse>();
        Assert.NotNull(createdSession);

        var createPlayerPayload = new
        {
            display_name = $"Player Invalid {suffix}",
            username = $"it_evt_invalid_player_{suffix}",
            password = "IntegrationInvalidPlayerPass!123"
        };

        var createPlayerResponse = await SendJsonAsync(
            HttpMethod.Post,
            "/api/v1/players",
            createPlayerPayload,
            instructorToken);
        Assert.Equal(HttpStatusCode.Created, createPlayerResponse.StatusCode);

        var createdPlayer = await createPlayerResponse.Content.ReadFromJsonAsync<PlayerResponse>();
        Assert.NotNull(createdPlayer);

        var addPlayerPayload = new
        {
            user_id = createdPlayer.UserId,
            player_order_no = 1
        };

        var addPlayerResponse = await SendJsonAsync(
            HttpMethod.Post,
            $"/api/v1/sessions/{createdSession.SessionId}/players",
            addPlayerPayload,
            instructorToken);
        Assert.Equal(HttpStatusCode.OK, addPlayerResponse.StatusCode);

        for (var i = 2; i <= 3; i++)
        {
            var extraPlayerResponse = await SendJsonAsync(
                HttpMethod.Post,
                "/api/v1/players",
                new
                {
                    display_name = $"Player Invalid Extra {i} {suffix}",
                    username = $"it_evt_invalid_player_extra_{i}_{suffix}",
                    password = "IntegrationInvalidExtraPlayerPass!123"
                },
                instructorToken);
            Assert.Equal(HttpStatusCode.Created, extraPlayerResponse.StatusCode);

            var extraPlayer = await extraPlayerResponse.Content.ReadFromJsonAsync<PlayerResponse>();
            Assert.NotNull(extraPlayer);

            var addExtraPlayerResponse = await SendJsonAsync(
                HttpMethod.Post,
                $"/api/v1/sessions/{createdSession.SessionId}/players",
                new
                {
                    user_id = extraPlayer.UserId,
                    player_order_no = i
                },
                instructorToken);
            Assert.Equal(HttpStatusCode.OK, addExtraPlayerResponse.StatusCode);
        }

        var startSessionResponse = await SendJsonAsync(
            HttpMethod.Post,
            $"/api/v1/sessions/{createdSession.SessionId}/start",
            body: null,
            instructorToken);
        Assert.Equal(HttpStatusCode.OK, startSessionResponse.StatusCode);

        var rulesetDetailResponse = await SendJsonAsync(
            HttpMethod.Get,
            $"/api/v1/rulesets/{createdRuleset.RulesetId}",
            body: null,
            instructorToken);
        Assert.Equal(HttpStatusCode.OK, rulesetDetailResponse.StatusCode);

        var rulesetDetail = await rulesetDetailResponse.Content.ReadFromJsonAsync<RulesetDetailResponse>();
        Assert.NotNull(rulesetDetail);
        var activeVersion = rulesetDetail.Versions
            .Where(v => string.Equals(v.Status, "ACTIVE", StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(v => v.Version)
            .First();

        using var setupEventsResponse = await SendJsonAsync(
            HttpMethod.Get,
            $"/api/v1/sessions/{createdSession.SessionId}/events?fromSeq=0&limit=100",
            null,
            instructorToken);
        using var setupEventsBody = await ReadJsonAsync(setupEventsResponse);
        var actingUserId = setupEventsBody.RootElement.GetProperty("events")
            .EnumerateArray()
            .Single(item =>
                item.GetProperty("action_type").GetString() == "BagikanTieBreaker" &&
                item.GetProperty("payload").GetProperty("number").GetInt32() == 1)
            .GetProperty("user_id")
            .GetGuid();
        var nextSequenceNumber = await GetNextSequenceNumberAsync(createdSession.SessionId, instructorToken);
        return (createdSession.SessionId, createdPlayer.UserId, actingUserId, activeVersion.RulesetVersionId, nextSequenceNumber);
    }

    private async Task<long> GetNextSequenceNumberAsync(Guid sessionId, string accessToken)
    {
        using var response = await SendJsonAsync(
            HttpMethod.Get,
            $"/api/v1/sessions/{sessionId}/state",
            null,
            accessToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var body = await ReadJsonAsync(response);
        return body.RootElement.GetProperty("next_sequence_number").GetInt64();
    }

    /// <summary>
    /// Helper untuk mendaftarkan pengguna baru dan mengembalikan data registrasi.
    /// </summary>
    private async Task<RegisterResponse> RegisterAsync(string username, string password, string role)
    {
        var payload = new RegisterRequest(username, password, role, null);
        var response = await _client.PostAsJsonAsync("/api/v1/auth/register", payload);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<RegisterResponse>();
        Assert.NotNull(body);
        return body;
    }

    /// <summary>
    /// Helper untuk melakukan login dan mengembalikan data token akses.
    /// </summary>
    private async Task<LoginResponse> LoginAsync(string username, string password)
    {
        var payload = new LoginRequest(username, password);
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", payload);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.NotNull(body);
        return body;
    }

    /// <summary>
    /// Helper untuk mengirim HTTP request dengan body JSON dan header Bearer token.
    /// </summary>
    private async Task<HttpResponseMessage> SendJsonAsync(
        HttpMethod method,
        string path,
        object? body,
        string accessToken)
    {
        var request = new HttpRequestMessage(method, path);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        if (body is not null)
        {
            request.Content = JsonContent.Create(body);
        }

        return await _client.SendAsync(request);
    }

    private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)
    {
        await using var stream = await response.Content.ReadAsStreamAsync();
        return await JsonDocument.ParseAsync(stream);
    }

    /// <summary>
    /// Helper yang membangun definition ruleset lengkap untuk mode PEMULA
    /// dengan parameter starting cash dan opsional pengaturan urutan pemain.
    /// </summary>
    private static RulesetDefinitionDto BuildRulesetDefinition(
        int startingCash,
        string playerOrdering = "PLAYER_ORDER",
        string mode = "PEMULA",
        int riskAmount = 4)
    {
        return new RulesetDefinitionDto
        {
            Mode = mode,
            Settings = new RulesetSettingsDto
            {
                ActionsPerTurn = 2,
                StartingCash = startingCash,
                InitialCoins = startingCash,
                InitialHappiness = 0,
                InitialSaving = 0,
                FinishDay = 25,
                MinPlayers = 2,
                MaxPlayers = 4,
                CashMin = 0,
                MaxIngredientTotal = 6,
                MaxSameIngredient = 3,
                PrimaryNeedMaxPerDay = 1,
                RequirePrimaryBeforeOthers = true,
                DonationMinAmount = 1,
                DonationMaxAmount = 999999,
                GoldTradeAllowBuy = true,
                GoldTradeAllowSell = true,
                LoanEnabled = mode == "MAHIR",
                InsuranceEnabled = mode == "MAHIR",
                SavingGoalEnabled = mode == "MAHIR",
                FreelanceIncome = 1
            },
            PlayerOrdering = new RulesetPlayerOrderingDto
            {
                OrderingCode = playerOrdering,
                FridayFeature = "DONATION",
                FridayEnabled = true,
                SaturdayFeature = "GOLD_TRADE",
                SaturdayEnabled = true,
                SundayFeature = "REST",
                SundayEnabled = true
            },
            Actions =
            [
                new RulesetActionDto { ActionId = "BahanMasakan" },
                new RulesetActionDto { ActionId = "JualMasakan" },
                new RulesetActionDto { ActionId = "Kebutuhan" },
                new RulesetActionDto { ActionId = "KerjaLepas" },
                new RulesetActionDto { ActionId = "RisikoKehidupan" },
                new RulesetActionDto { ActionId = "GunakanOpsiDarurat" },
                new RulesetActionDto { ActionId = "PinjamanSyariah" },
                new RulesetActionDto { ActionId = "BayarPinjaman" },
                new RulesetActionDto { ActionId = "Asuransi" },
                new RulesetActionDto { ActionId = "BayarRisiko" },
                new RulesetActionDto { ActionId = "BukaHargaEmas" },
                new RulesetActionDto { ActionId = "InvestasiEmas" },
                new RulesetActionDto { ActionId = "JualEmas" }
            ],
            Ingredients =
            [
                new RulesetIngredientDto { Id = "nasi_putih", Nama = "Nasi Putih", HargaBeli = 1 },
                new RulesetIngredientDto { Id = "telur", Nama = "Telur", HargaBeli = 4 },
                new RulesetIngredientDto { Id = "sayur", Nama = "Sayur", HargaBeli = 2 }
            ],
            Orders =
            [
                new RulesetOrderDto
                {
                    Id = "nasi_goreng",
                    Nama = "nasi goreng",
                    HargaJual = mode == "MAHIR" ? 1 : 15,
                    PoinKebahagiaan = 0,
                    Bahan = mode == "MAHIR" ? [] : ["Nasi Putih", "Telur"],
                    CardQty = 6
                }
            ],
            Needs =
            [
                new RulesetNeedDto { Id = "buku", Nama = "buku", Tipe = "primer", HargaBeli = 3, PoinKebahagiaan = 1 },
                new RulesetNeedDto { Id = "baju", Nama = "baju", Tipe = "primer", HargaBeli = 3, PoinKebahagiaan = 1 },
                new RulesetNeedDto { Id = "sepatu", Nama = "sepatu", Tipe = "primer", HargaBeli = 3, PoinKebahagiaan = 1 },
                new RulesetNeedDto { Id = "tempat_makan", Nama = "tempat makan", Tipe = "primer", HargaBeli = 3, PoinKebahagiaan = 1 },
                new RulesetNeedDto { Id = "alat_tulis", Nama = "alat tulis", Tipe = "primer", HargaBeli = 3, PoinKebahagiaan = 1 },
                new RulesetNeedDto { Id = "boneka", Nama = "boneka", Tipe = "tersier", HargaBeli = 6, PoinKebahagiaan = 3 },
                new RulesetNeedDto { Id = "gameboy", Nama = "gameboy", Tipe = "tersier", HargaBeli = 6, PoinKebahagiaan = 3 },
                new RulesetNeedDto { Id = "hiburan", Nama = "hiburan", Tipe = "tersier", HargaBeli = 6, PoinKebahagiaan = 3 },
                new RulesetNeedDto { Id = "jam", Nama = "jam", Tipe = "tersier", HargaBeli = 6, PoinKebahagiaan = 3 }
            ],
            CollectionMissions =
            [
                new RulesetCollectionMissionDto
                {
                    Id = "misi_boneka",
                    Nama = "boneka",
                    SuccessPoints = 0,
                    FailurePoints = -10,
                    PenaltyPoints = 10,
                    KebutuhanTarget =
                    [
                        new RulesetCollectionMissionRequirementDto { Order = 1, Type = "primer", Value = "buku" },
                        new RulesetCollectionMissionRequirementDto { Order = 2, Type = "tersier", Value = "boneka" }
                    ]
                },
                new RulesetCollectionMissionDto
                {
                    Id = "misi_gameboy", Nama = "gameboy", PenaltyPoints = 10,
                    KebutuhanTarget =
                    [
                        new RulesetCollectionMissionRequirementDto { Order = 1, Type = "primer", Value = "buku" },
                        new RulesetCollectionMissionRequirementDto { Order = 2, Type = "tersier", Value = "gameboy" }
                    ]
                },
                new RulesetCollectionMissionDto
                {
                    Id = "misi_hiburan", Nama = "hiburan", PenaltyPoints = 10,
                    KebutuhanTarget =
                    [
                        new RulesetCollectionMissionRequirementDto { Order = 1, Type = "primer", Value = "buku" },
                        new RulesetCollectionMissionRequirementDto { Order = 2, Type = "tersier", Value = "hiburan" }
                    ]
                },
                new RulesetCollectionMissionDto
                {
                    Id = "misi_jam", Nama = "jam", PenaltyPoints = 10,
                    KebutuhanTarget =
                    [
                        new RulesetCollectionMissionRequirementDto { Order = 1, Type = "primer", Value = "buku" },
                        new RulesetCollectionMissionRequirementDto { Order = 2, Type = "tersier", Value = "jam" }
                    ]
                }
            ],
            FinancialGoals =
            [
                new RulesetFinancialGoalDto
                {
                    Id = "beli_rumah",
                    Nama = "beli rumah",
                    HargaBeli = 20,
                    PoinKebahagiaan = 5
                }
            ],
            DonationRankPoints =
            [
                new RulesetDonationRankPointDto { Rank = 1, Points = 7 },
                new RulesetDonationRankPointDto { Rank = 2, Points = 5 },
                new RulesetDonationRankPointDto { Rank = 3, Points = 2 }
            ],
            GoldPointsByQty =
            [
                new RulesetGoldPointDto { Qty = 1, Points = 3 },
                new RulesetGoldPointDto { Qty = 2, Points = 5 },
                new RulesetGoldPointDto { Qty = 3, Points = 8 },
                new RulesetGoldPointDto { Qty = 4, Points = 12 }
            ],
            PensionRankPoints =
            [
                new RulesetPensionRankPointDto { Rank = 1, Points = 5 },
                new RulesetPensionRankPointDto { Rank = 2, Points = 3 },
                new RulesetPensionRankPointDto { Rank = 3, Points = 1 }
            ],
            TieBreakers =
            [
                new RulesetTieBreakerDto { TieBreakerCode = "tie_breaker_1", TieNumber = 1, CardQty = 1 },
                new RulesetTieBreakerDto { TieBreakerCode = "tie_breaker_2", TieNumber = 2, CardQty = 1 },
                new RulesetTieBreakerDto { TieBreakerCode = "tie_breaker_3", TieNumber = 3, CardQty = 1 },
                new RulesetTieBreakerDto { TieBreakerCode = "tie_breaker_4", TieNumber = 4, CardQty = 1 }
            ],
            ShariaLoans =
            [
                new RulesetShariaLoanDto
                {
                    LoanCode = "loan_syariah_10",
                    ItemName = "Pinjaman Syariah 10",
                    Principal = 10,
                    RepaymentAmount = 10,
                    DurationDays = 1,
                    PenaltyPoints = 10,
                    CardQty = 8
                }
            ],
            InsuranceProducts =
            [
                new RulesetInsuranceProductDto
                {
                    ProductCode = "multirisk_basic",
                    ItemName = "Asuransi",
                    Premium = 1,
                    UsageLimit = 1,
                    CardQty = 4
                }
            ],
            LifeRisks =
            [
                new RulesetLifeRiskDto
                {
                    RiskCode = "risk_cost_4",
                    ItemName = "Biaya 4",
                    EffectType = "COIN_EFFECT",
                    Direction = "OUT",
                    Amount = riskAmount,
                    TargetScope = "SELF",
                    DurationDays = 1,
                    CardQty = 4
                }
            ]
        };
    }
}
