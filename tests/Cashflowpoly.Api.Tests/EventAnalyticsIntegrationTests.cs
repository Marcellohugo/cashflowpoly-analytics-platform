// Fungsi file: Menguji alur integrasi ingest event, analitik sesi, urutan pemain, batas pemain, dan validasi domain.
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Cashflowpoly.Api.Models;
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
            config = BuildRulesetConfig(startingCash: 20)
        };

        var createRulesetResponse = await SendJsonAsync(
            HttpMethod.Post,
            "/api/v1/rulesets",
            createRulesetPayload,
            instructorToken);
        Assert.Equal(HttpStatusCode.Created, createRulesetResponse.StatusCode);

        var createdRuleset = await createRulesetResponse.Content.ReadFromJsonAsync<CreateRulesetResponse>();
        Assert.NotNull(createdRuleset);

        var updateRulesetPayload = new
        {
            name = $"Ruleset Event IT {suffix} V2",
            description = "Integration event analytics v2",
            config = BuildRulesetConfig(startingCash: 21)
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
            ruleset_id = createdRuleset.RulesetId
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

        var createdPlayer = await createPlayerResponse.Content.ReadFromJsonAsync<PlayerResponse>();
        Assert.NotNull(createdPlayer);

        var addPlayerPayload = new
        {
            player_id = createdPlayer.PlayerId,
            role = "PLAYER",
            join_order = 1
        };

        var addPlayerResponse = await SendJsonAsync(
            HttpMethod.Post,
            $"/api/v1/sessions/{createdSession.SessionId}/players",
            addPlayerPayload,
            instructorToken);
        Assert.Equal(HttpStatusCode.OK, addPlayerResponse.StatusCode);

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

        var now = DateTimeOffset.UtcNow;
        var event1Payload = new
        {
            event_id = Guid.NewGuid(),
            session_id = createdSession.SessionId,
            player_id = createdPlayer.PlayerId,
            actor_type = "PLAYER",
            timestamp = now.ToString("O"),
            day_index = 0,
            weekday = "MON",
            turn_number = 1,
            sequence_number = 1,
            action_type = "transaction.recorded",
            ruleset_version_id = activeVersion.RulesetVersionId,
            payload = new
            {
                direction = "IN",
                amount = 10,
                category = "NEED_PRIMARY",
                counterparty = "BANK"
            }
        };
        var event1Response = await SendJsonAsync(HttpMethod.Post, "/api/v1/events", event1Payload, instructorToken);
        Assert.Equal(HttpStatusCode.Created, event1Response.StatusCode);

        var event2Payload = new
        {
            event_id = Guid.NewGuid(),
            session_id = createdSession.SessionId,
            player_id = createdPlayer.PlayerId,
            actor_type = "PLAYER",
            timestamp = now.AddSeconds(1).ToString("O"),
            day_index = 0,
            weekday = "MON",
            turn_number = 1,
            sequence_number = 2,
            action_type = "transaction.recorded",
            ruleset_version_id = activeVersion.RulesetVersionId,
            payload = new
            {
                direction = "OUT",
                amount = 3,
                category = "NEED_PRIMARY",
                counterparty = "BANK"
            }
        };
        var event2Response = await SendJsonAsync(HttpMethod.Post, "/api/v1/events", event2Payload, instructorToken);
        Assert.Equal(HttpStatusCode.Created, event2Response.StatusCode);

        var analyticsResponse = await SendJsonAsync(
            HttpMethod.Get,
            $"/api/v1/analytics/sessions/{createdSession.SessionId}",
            body: null,
            instructorToken);
        Assert.Equal(HttpStatusCode.OK, analyticsResponse.StatusCode);

        var analytics = await analyticsResponse.Content.ReadFromJsonAsync<AnalyticsSessionResponse>();
        Assert.NotNull(analytics);
        Assert.Equal(2, analytics.Summary.EventCount);
        Assert.Equal(10d, analytics.Summary.CashInTotal, 6);
        Assert.Equal(3d, analytics.Summary.CashOutTotal, 6);
        Assert.Equal(7d, analytics.Summary.CashflowNetTotal, 6);

        var byPlayer = analytics.ByPlayer.Single(p => p.PlayerId == createdPlayer.PlayerId);
        Assert.Equal(10d, byPlayer.CashInTotal, 6);
        Assert.Equal(3d, byPlayer.CashOutTotal, 6);
        Assert.Equal(0, byPlayer.OrdersCompletedCount);
        Assert.Equal(0, byPlayer.InventoryIngredientTotal);
        Assert.Equal(0, byPlayer.ActionsUsedTotal);
        Assert.Equal(1d, byPlayer.CompliancePrimaryNeedRate, 6);
        Assert.Equal(0, byPlayer.RulesViolationsCount);

        var transactionsResponse = await SendJsonAsync(
            HttpMethod.Get,
            $"/api/v1/analytics/sessions/{createdSession.SessionId}/transactions?playerId={createdPlayer.PlayerId}",
            body: null,
            instructorToken);
        Assert.Equal(HttpStatusCode.OK, transactionsResponse.StatusCode);

        var transactions = await transactionsResponse.Content.ReadFromJsonAsync<TransactionHistoryResponse>();
        Assert.NotNull(transactions);
        Assert.Equal(2, transactions.Items.Count);
        Assert.Equal(10d, transactions.Items[0].Amount, 6);
        Assert.Equal(3d, transactions.Items[1].Amount, 6);

        var recomputeResponse = await SendJsonAsync(
            HttpMethod.Post,
            $"/api/v1/analytics/sessions/{createdSession.SessionId}/recompute",
            body: null,
            instructorToken);
        Assert.Equal(HttpStatusCode.OK, recomputeResponse.StatusCode);
    }

    [Fact]
    /// <summary>
    /// Memvalidasi bahwa pemain yang ditambahkan tanpa join_order eksplisit
    /// akan diurutkan berdasarkan PlayerId dan menerima join order 1, 2, 3.
    /// </summary>
    public async Task AddPlayersWithoutJoinOrder_AssignsPlayerTurnOrderByPlayerId()
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];
        var instructorUsername = $"it_evt_order_instructor_{suffix}";
        const string instructorPassword = "IntegrationOrderInstructorPass!123";

        var instructorToken = (await RegisterAsync(instructorUsername, instructorPassword, "INSTRUCTOR")).AccessToken;

        var createRulesetPayload = new
        {
            name = $"Ruleset Order IT {suffix}",
            description = "Integration player order assignment",
            config = BuildRulesetConfig(startingCash: 20)
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
            session_name = $"Session Order IT {suffix}",
            mode = "PEMULA",
            ruleset_id = createdRuleset.RulesetId
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
                player_id = createdPlayer.PlayerId,
                role = "PLAYER"
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

        var orderedByPlayerId = players.OrderBy(x => x.PlayerId).ToList();
        var now = DateTimeOffset.UtcNow;
        long sequence = 1;
        foreach (var player in orderedByPlayerId.AsEnumerable().Reverse())
        {
            var eventPayload = new
            {
                event_id = Guid.NewGuid(),
                session_id = createdSession.SessionId,
                player_id = player.PlayerId,
                actor_type = "PLAYER",
                timestamp = now.AddSeconds(sequence).ToString("O"),
                day_index = 0,
                weekday = "MON",
                turn_number = 1,
                sequence_number = sequence,
                action_type = "transaction.recorded",
                ruleset_version_id = activeVersion.RulesetVersionId,
                payload = new
                {
                    direction = "IN",
                    amount = 5,
                    category = "NEED_PRIMARY",
                    counterparty = "BANK"
                }
            };

            var eventResponse = await SendJsonAsync(HttpMethod.Post, "/api/v1/events", eventPayload, instructorToken);
            Assert.Equal(HttpStatusCode.Created, eventResponse.StatusCode);
            sequence += 1;
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

        var expectedPlayerOrder = orderedByPlayerId.Select(item => item.PlayerId).ToList();
        var actualPlayerOrder = analytics.ByPlayer.Select(item => item.PlayerId).ToList();
        var actualJoinOrders = analytics.ByPlayer.Select(item => item.JoinOrder).ToList();
        Assert.Equal(expectedPlayerOrder, actualPlayerOrder);
        Assert.Equal(new[] { 1, 2, 3 }, actualJoinOrders);
    }

    [Fact]
    /// <summary>
    /// Memvalidasi bahwa urutan pemain mengikuti konfigurasi INSTRUCTOR_ORDER pada ruleset
    /// ketika pemain ditambahkan via username dengan urutan manual yang ditentukan instruktur.
    /// </summary>
    public async Task AddPlayersByUsername_WithInstructorOrderRuleset_AssignsManualTurnOrder()
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];
        var instructorUsername = $"it_evt_manual_order_instructor_{suffix}";
        const string instructorPassword = "IntegrationManualOrderInstructorPass!123";
        var instructorToken = (await RegisterAsync(instructorUsername, instructorPassword, "INSTRUCTOR")).AccessToken;
        var firstPlayerUsername = $"it_evt_manual_order_player_1_{suffix}";
        var secondPlayerUsername = $"it_evt_manual_order_player_2_{suffix}";
        var thirdPlayerUsername = $"it_evt_manual_order_player_3_{suffix}";

        var createRulesetPayload = new
        {
            name = $"Ruleset Manual Order IT {suffix}",
            description = "Integration instructor manual order by username",
            config = BuildRulesetConfig(
                startingCash: 20,
                playerOrdering: "INSTRUCTOR_ORDER",
                instructorPlayerUsernames: new[] { thirdPlayerUsername, firstPlayerUsername, secondPlayerUsername, "" })
        };

        var createRulesetResponse = await SendJsonAsync(
            HttpMethod.Post,
            "/api/v1/rulesets",
            createRulesetPayload,
            instructorToken);
        Assert.Equal(HttpStatusCode.Created, createRulesetResponse.StatusCode);

        var createdRuleset = await createRulesetResponse.Content.ReadFromJsonAsync<CreateRulesetResponse>();
        Assert.NotNull(createdRuleset);

        var createSessionResponse = await SendJsonAsync(
            HttpMethod.Post,
            "/api/v1/sessions",
            new
            {
                session_name = $"Session Manual Order IT {suffix}",
                mode = "PEMULA",
                ruleset_id = createdRuleset.RulesetId
            },
            instructorToken);
        Assert.Equal(HttpStatusCode.Created, createSessionResponse.StatusCode);

        var createdSession = await createSessionResponse.Content.ReadFromJsonAsync<CreateSessionResponse>();
        Assert.NotNull(createdSession);

        var players = new List<(string Username, Guid PlayerId)>();
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
                    display_name = $"Player Manual Order {i} {suffix}",
                    username,
                    password = "IntegrationManualOrderPlayerPass!123"
                },
                instructorToken);
            Assert.Equal(HttpStatusCode.Created, createPlayerResponse.StatusCode);

            var createdPlayer = await createPlayerResponse.Content.ReadFromJsonAsync<PlayerResponse>();
            Assert.NotNull(createdPlayer);
            players.Add((username, createdPlayer.PlayerId));
        }

        var firstPlayer = players[0];
        var secondPlayer = players[1];
        var thirdPlayer = players[2];

        var addFirst = await SendJsonAsync(
            HttpMethod.Post,
            $"/api/v1/sessions/{createdSession.SessionId}/players",
            new { username = firstPlayer.Username, role = "PLAYER" },
            instructorToken);
        Assert.Equal(HttpStatusCode.OK, addFirst.StatusCode);

        var addSecond = await SendJsonAsync(
            HttpMethod.Post,
            $"/api/v1/sessions/{createdSession.SessionId}/players",
            new { username = secondPlayer.Username, role = "PLAYER" },
            instructorToken);
        Assert.Equal(HttpStatusCode.OK, addSecond.StatusCode);

        var addThird = await SendJsonAsync(
            HttpMethod.Post,
            $"/api/v1/sessions/{createdSession.SessionId}/players",
            new { username = thirdPlayer.Username, role = "PLAYER" },
            instructorToken);
        Assert.Equal(HttpStatusCode.OK, addThird.StatusCode);

        var analyticsResponse = await SendJsonAsync(
            HttpMethod.Get,
            $"/api/v1/analytics/sessions/{createdSession.SessionId}",
            body: null,
            instructorToken);
        Assert.Equal(HttpStatusCode.OK, analyticsResponse.StatusCode);

        var analytics = await analyticsResponse.Content.ReadFromJsonAsync<AnalyticsSessionResponse>();
        Assert.NotNull(analytics);
        Assert.Equal(3, analytics.ByPlayer.Count);

        var expectedPlayerOrder = new[] { thirdPlayer.PlayerId, firstPlayer.PlayerId, secondPlayer.PlayerId };
        var actualPlayerOrder = analytics.ByPlayer.Select(item => item.PlayerId).ToArray();
        var actualJoinOrders = analytics.ByPlayer.Select(item => item.JoinOrder).ToArray();

        Assert.Equal(expectedPlayerOrder, actualPlayerOrder);
        Assert.Equal(new[] { 1, 2, 3 }, actualJoinOrders);
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
            config = BuildRulesetConfig(startingCash: 20)
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
            ruleset_id = createdRuleset.RulesetId
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
                    player_id = createdPlayer.PlayerId,
                    role = "PLAYER"
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
                player_id = fifthPlayer.PlayerId,
                role = "PLAYER"
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
    /// Memvalidasi bahwa path API legacy (/api/...) mengembalikan 404 Not Found
    /// dan path versioned (/api/v1/...) berfungsi dengan benar.
    /// </summary>
    public async Task LegacyApiRoute_ReturnsNotFound_AndV1AuthWorks()
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];
        var username = $"it_legacy_{suffix}";
        const string password = "LegacyRoutePass!123";

        var registerPayload = new RegisterRequest(username, password, "INSTRUCTOR", null);
        var legacyRegisterResponse = await _client.PostAsJsonAsync("/api/auth/register", registerPayload);
        Assert.Equal(HttpStatusCode.NotFound, legacyRegisterResponse.StatusCode);

        var registerResponse = await _client.PostAsJsonAsync("/api/v1/auth/register", registerPayload);
        Assert.Equal(HttpStatusCode.Created, registerResponse.StatusCode);

        var loginPayload = new LoginRequest(username, password);
        var loginResponse = await _client.PostAsJsonAsync("/api/v1/auth/login", loginPayload);
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var login = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.NotNull(login);
        Assert.False(string.IsNullOrWhiteSpace(login.AccessToken));

        var legacySessionsResponse = await SendJsonAsync(HttpMethod.Get, "/api/sessions", null, login.AccessToken);
        Assert.Equal(HttpStatusCode.NotFound, legacySessionsResponse.StatusCode);

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
            player_id = setup.PlayerId,
            actor_type = "PLAYER",
            timestamp = now.ToString("O"),
            day_index = 0,
            weekday = "MON",
            turn_number = 1,
            sequence_number = 1,
            action_type = "transaction.recorded",
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
            player_id = setup.PlayerId,
            actor_type = "PLAYER",
            timestamp = now.AddSeconds(1).ToString("O"),
            day_index = -1,
            weekday = "MON",
            turn_number = 1,
            sequence_number = 1,
            action_type = "transaction.recorded",
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
            player_id = (Guid?)null,
            actor_type = "SYSTEM",
            timestamp = now.ToString("O"),
            day_index = 0,
            weekday = "MON",
            turn_number = 1,
            sequence_number = 1,
            action_type = "transaction.recorded",
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
    public async Task AddPlayerToSession_InvalidRole_ReturnsBadRequest()
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
                player_id = createdPlayer.PlayerId,
                role = "ADMIN"
            },
            instructorToken);
        Assert.Equal(HttpStatusCode.BadRequest, addWithInvalidRoleResponse.StatusCode);

        var error = await addWithInvalidRoleResponse.Content.ReadFromJsonAsync<ErrorResponse>();
        Assert.NotNull(error);
        Assert.Equal("VALIDATION_ERROR", error.ErrorCode);
        Assert.Contains(error.Details, detail =>
            string.Equals(detail.Field, "role", StringComparison.OrdinalIgnoreCase) &&
            string.Equals(detail.Issue, "INVALID_ENUM", StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Helper yang membuat ruleset, sesi, player, dan menjalankan sesi hingga siap
    /// untuk menerima event, lalu mengembalikan ID sesi, player, dan versi ruleset aktif.
    /// </summary>
    private async Task<(Guid SessionId, Guid PlayerId, Guid RulesetVersionId)> CreateReadySessionAsync(
        string instructorToken,
        string suffix)
    {
        var createRulesetPayload = new
        {
            name = $"Ruleset Invalid IT {suffix}",
            description = "Integration invalid event validation",
            config = BuildRulesetConfig(startingCash: 50)
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
            mode = "PEMULA",
            ruleset_id = createdRuleset.RulesetId
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
            player_id = createdPlayer.PlayerId,
            role = "PLAYER"
        };

        var addPlayerResponse = await SendJsonAsync(
            HttpMethod.Post,
            $"/api/v1/sessions/{createdSession.SessionId}/players",
            addPlayerPayload,
            instructorToken);
        Assert.Equal(HttpStatusCode.OK, addPlayerResponse.StatusCode);

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

        return (createdSession.SessionId, createdPlayer.PlayerId, activeVersion.RulesetVersionId);
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

    /// <summary>
    /// Helper yang membangun objek konfigurasi ruleset lengkap untuk mode PEMULA
    /// dengan parameter starting cash dan opsional pengaturan urutan pemain.
    /// </summary>
    private static object BuildRulesetConfig(
        int startingCash,
        string playerOrdering = "JOIN_ORDER",
        string[]? instructorPlayerUsernames = null)
    {
        return new
        {
            mode = "PEMULA",
            actions_per_turn = 2,
            starting_cash = startingCash,
            player_ordering = playerOrdering,
            instructor_player_usernames = instructorPlayerUsernames ?? Array.Empty<string>(),
            weekday_rules = new
            {
                friday = new { feature = "DONATION", enabled = true },
                saturday = new { feature = "GOLD_TRADE", enabled = true },
                sunday = new { feature = "REST", enabled = true }
            },
            constraints = new
            {
                cash_min = 0,
                max_ingredient_total = 6,
                max_same_ingredient = 3,
                primary_need_max_per_day = 1,
                require_primary_before_others = true
            },
            donation = new { min_amount = 1, max_amount = 999999 },
            gold_trade = new { allow_buy = true, allow_sell = true },
            advanced = new
            {
                loan = new { enabled = false },
                insurance = new { enabled = false },
                saving_goal = new { enabled = false }
            },
            freelance = new { income = 1 },
            scoring = new
            {
                donation_rank_points = new[]
                {
                    new { rank = 1, points = 7 },
                    new { rank = 2, points = 5 },
                    new { rank = 3, points = 2 }
                },
                gold_points_by_qty = new[]
                {
                    new { qty = 1, points = 3 },
                    new { qty = 2, points = 5 },
                    new { qty = 3, points = 8 },
                    new { qty = 4, points = 12 }
                },
                pension_rank_points = new[]
                {
                    new { rank = 1, points = 5 },
                    new { rank = 2, points = 3 },
                    new { rank = 3, points = 1 }
                }
            }
        };
    }
}
