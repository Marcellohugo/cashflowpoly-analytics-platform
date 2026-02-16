using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Cashflowpoly.Api.Models;
using Cashflowpoly.Api.Tests.Infrastructure;
using Xunit;

namespace Cashflowpoly.Api.Tests;

[Collection("ApiIntegration")]
[Trait("Category", "Integration")]
public sealed class EventAnalyticsIntegrationTests
{
    private readonly HttpClient _client;

    public EventAnalyticsIntegrationTests(ApiIntegrationTestFixture fixture)
    {
        _client = fixture.Client;
    }

    [Fact]
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

    private async Task<RegisterResponse> RegisterAsync(string username, string password, string role)
    {
        var payload = new RegisterRequest(username, password, role, null);
        var response = await _client.PostAsJsonAsync("/api/v1/auth/register", payload);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<RegisterResponse>();
        Assert.NotNull(body);
        return body;
    }

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

    private static object BuildRulesetConfig(int startingCash)
    {
        return new
        {
            mode = "PEMULA",
            actions_per_turn = 2,
            starting_cash = startingCash,
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
