using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Cashflowpoly.Api.Models;
using Cashflowpoly.Api.Tests.Infrastructure;
using Xunit;

namespace Cashflowpoly.Api.Tests;

[Collection("ApiIntegration")]
[Trait("Category", "Integration")]
public sealed class AuthRbacRulesetIntegrationTests
{
    private readonly HttpClient _client;

    public AuthRbacRulesetIntegrationTests(ApiIntegrationTestFixture fixture)
    {
        _client = fixture.Client;
    }

    [Fact]
    public async Task Auth_Rbac_And_RulesetFlow_Work_EndToEnd()
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];
        var instructorUsername = $"it_instructor_{suffix}";
        var playerUsername = $"it_player_{suffix}";
        const string instructorPassword = "IntegrationInstructorPass!123";
        const string playerPassword = "IntegrationPlayerPass!123";

        var instructorRegister = await RegisterAsync(instructorUsername, instructorPassword, "INSTRUCTOR");
        var playerRegister = await RegisterAsync(playerUsername, playerPassword, "PLAYER");

        Assert.Equal("INSTRUCTOR", instructorRegister.Role);
        Assert.Equal("PLAYER", playerRegister.Role);

        var instructorLogin = await LoginAsync(instructorUsername, instructorPassword);
        var playerLogin = await LoginAsync(playerUsername, playerPassword);

        Assert.False(string.IsNullOrWhiteSpace(instructorLogin.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(playerLogin.AccessToken));

        var withoutToken = await _client.GetAsync("/api/v1/sessions");
        Assert.Equal(HttpStatusCode.Unauthorized, withoutToken.StatusCode);

        var createRulesetPayload = new
        {
            name = $"Ruleset IT {suffix}",
            description = "Integration test ruleset",
            config = BuildRulesetConfig(startingCash: 20)
        };

        var playerCreateRuleset = await SendJsonAsync(
            HttpMethod.Post,
            "/api/v1/rulesets",
            createRulesetPayload,
            playerLogin.AccessToken);
        Assert.Equal(HttpStatusCode.Forbidden, playerCreateRuleset.StatusCode);

        var instructorCreateRuleset = await SendJsonAsync(
            HttpMethod.Post,
            "/api/v1/rulesets",
            createRulesetPayload,
            instructorLogin.AccessToken);
        Assert.Equal(HttpStatusCode.Created, instructorCreateRuleset.StatusCode);

        var createdRuleset = await instructorCreateRuleset.Content.ReadFromJsonAsync<CreateRulesetResponse>();
        Assert.NotNull(createdRuleset);
        Assert.NotEqual(Guid.Empty, createdRuleset.RulesetId);
        Assert.Equal(1, createdRuleset.Version);

        var updateRulesetPayload = new
        {
            name = $"Ruleset IT {suffix} V2",
            description = "Integration test ruleset v2",
            config = BuildRulesetConfig(startingCash: 21)
        };

        var instructorUpdateRuleset = await SendJsonAsync(
            HttpMethod.Put,
            $"/api/v1/rulesets/{createdRuleset.RulesetId}",
            updateRulesetPayload,
            instructorLogin.AccessToken);
        Assert.Equal(HttpStatusCode.OK, instructorUpdateRuleset.StatusCode);

        var updatedRuleset = await instructorUpdateRuleset.Content.ReadFromJsonAsync<CreateRulesetResponse>();
        Assert.NotNull(updatedRuleset);
        Assert.Equal(createdRuleset.RulesetId, updatedRuleset.RulesetId);
        Assert.True(updatedRuleset.Version >= 2);

        var playerActivateVersion = await SendJsonAsync(
            HttpMethod.Post,
            $"/api/v1/rulesets/{createdRuleset.RulesetId}/versions/{updatedRuleset.Version}/activate",
            body: null,
            playerLogin.AccessToken);
        Assert.Equal(HttpStatusCode.Forbidden, playerActivateVersion.StatusCode);

        var instructorActivateVersion = await SendJsonAsync(
            HttpMethod.Post,
            $"/api/v1/rulesets/{createdRuleset.RulesetId}/versions/{updatedRuleset.Version}/activate",
            body: null,
            instructorLogin.AccessToken);
        Assert.Equal(HttpStatusCode.OK, instructorActivateVersion.StatusCode);

        var createSessionPayload = new
        {
            session_name = $"Session IT {suffix}",
            mode = "PEMULA",
            ruleset_id = createdRuleset.RulesetId
        };

        var playerCreateSession = await SendJsonAsync(
            HttpMethod.Post,
            "/api/v1/sessions",
            createSessionPayload,
            playerLogin.AccessToken);
        Assert.Equal(HttpStatusCode.Forbidden, playerCreateSession.StatusCode);

        var instructorCreateSession = await SendJsonAsync(
            HttpMethod.Post,
            "/api/v1/sessions",
            createSessionPayload,
            instructorLogin.AccessToken);
        Assert.Equal(HttpStatusCode.Created, instructorCreateSession.StatusCode);

        var createdSession = await instructorCreateSession.Content.ReadFromJsonAsync<CreateSessionResponse>();
        Assert.NotNull(createdSession);
        Assert.NotEqual(Guid.Empty, createdSession.SessionId);

        var playerStartSession = await SendJsonAsync(
            HttpMethod.Post,
            $"/api/v1/sessions/{createdSession.SessionId}/start",
            body: null,
            playerLogin.AccessToken);
        Assert.Equal(HttpStatusCode.Forbidden, playerStartSession.StatusCode);

        var instructorStartSession = await SendJsonAsync(
            HttpMethod.Post,
            $"/api/v1/sessions/{createdSession.SessionId}/start",
            body: null,
            instructorLogin.AccessToken);
        Assert.Equal(HttpStatusCode.OK, instructorStartSession.StatusCode);

        var startResponse = await instructorStartSession.Content.ReadFromJsonAsync<SessionStatusResponse>();
        Assert.NotNull(startResponse);
        Assert.Equal("STARTED", startResponse.Status);
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

    private async Task<LoginResponse> LoginAsync(string username, string password)
    {
        var payload = new LoginRequest(username, password);
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", payload);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<LoginResponse>();
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
