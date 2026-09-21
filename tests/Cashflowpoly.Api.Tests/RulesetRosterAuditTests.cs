// Fungsi file: Menguji batas input, scope ruleset, dan konsistensi roster saat permintaan bersamaan.
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using Cashflowpoly.Api.Contracts;
using Cashflowpoly.Api.Tests.Infrastructure;
using Dapper;
using Npgsql;
using Xunit;

namespace Cashflowpoly.Api.Tests;

[Collection("ApiIntegration")]
[Trait("Category", "Integration")]
public sealed class RulesetRosterAuditTests(ApiIntegrationTestFixture fixture)
{
    [Fact]
    public async Task NullRulesetCollectionsAndElementsReturn400OnCreateAndUpdate()
    {
        var token = (await Register("INSTRUCTOR")).AccessToken;
        var definition = EventAnalyticsIntegrationTests.BuildRulesetDefinition(50);
        using var created = await Send(HttpMethod.Post, "/api/v1/rulesets", new { name = "Shape control", definition }, token);
        created.EnsureSuccessStatusCode();
        var ruleset = (await created.Content.ReadFromJsonAsync<CreateRulesetResponse>())!;
        foreach (var field in new[] { "actions", "narratives", "ingredients", "collection_missions", "donation_rank_points" })
        foreach (var missingCollection in new[] { false, true })
        {
            var malformed = JsonSerializer.SerializeToNode(definition)!;
            malformed[field] = missingCollection ? null : new JsonArray((JsonNode?)null);
            foreach (var method in new[] { HttpMethod.Post, HttpMethod.Put })
            {
                var path = method == HttpMethod.Post ? "/api/v1/rulesets" : $"/api/v1/rulesets/{ruleset.RulesetId}";
                using var response = await Send(method, path, new { name = "Invalid shape", definition = malformed }, token);
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }
        foreach (var nested in new[] { "prerequisiteAksi", "teks" })
        {
            var malformed = JsonSerializer.SerializeToNode(definition)!;
            malformed["narratives"] = new JsonArray(new JsonObject { [nested] = new JsonArray((JsonNode?)null) });
            using var response = await Send(HttpMethod.Post, "/api/v1/rulesets", new { name = "Invalid nested", definition = malformed }, token);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }

    [Fact]
    public async Task PlayerAndSessionNamesRespectDatabaseBoundaries()
    {
        var token = (await Register("INSTRUCTOR")).AccessToken;
        foreach (var length in new[] { 80, 81 })
        {
            using var response = await Send(HttpMethod.Post, "/api/v1/players", new
            {
                display_name = new string('a', length), username = $"boundary_{Guid.NewGuid():N}", password = "BoundaryTest!2026"
            }, token);
            Assert.Equal(length == 80 ? HttpStatusCode.Created : HttpStatusCode.BadRequest, response.StatusCode);
        }
        var definition = EventAnalyticsIntegrationTests.BuildRulesetDefinition(50);
        using var rulesetResponse = await Send(HttpMethod.Post, "/api/v1/rulesets", new { name = "Length control", definition }, token);
        var ruleset = (await rulesetResponse.Content.ReadFromJsonAsync<CreateRulesetResponse>())!;
        foreach (var length in new[] { 120, 121 })
        {
            using var response = await Send(HttpMethod.Post, "/api/v1/sessions", new
            {
                session_name = new string('b', length), mode = definition.Mode, ruleset_version_id = ruleset.RulesetVersionId
            }, token);
            Assert.Equal(length == 120 ? HttpStatusCode.Created : HttpStatusCode.BadRequest, response.StatusCode);
        }
    }

    [Fact]
    public async Task RulesetSectionsUseMembershipScopeIncludingDefaultRulesets()
    {
        var setup = await CreateRoster();
        var member = setup.Players[0];
        var foreign = await Register("PLAYER");
        foreach (var account in new[] { member, foreign })
        {
            using var response = await Send(HttpMethod.Get,
                $"/api/v1/rulesets/sections?mode={setup.Definition.Mode}&rulesetId={setup.Ruleset.RulesetId}", null, account.AccessToken);
            Assert.Equal(account.UserId == member.UserId ? HttpStatusCode.OK : HttpStatusCode.NotFound, response.StatusCode);
        }
        using var defaults = await Send(HttpMethod.Get, "/api/v1/rulesets/sections?mode=PEMULA", null, foreign.AccessToken);
        defaults.EnsureSuccessStatusCode();
        using var defaultJson = JsonDocument.Parse(await defaults.Content.ReadAsStringAsync());
        var defaultId = defaultJson.RootElement.GetProperty("ruleset_id").GetGuid();
        using var explicitDefault = await Send(HttpMethod.Get,
            $"/api/v1/rulesets/sections?mode=PEMULA&rulesetId={defaultId}", null, foreign.AccessToken);
        Assert.Equal(HttpStatusCode.OK, explicitDefault.StatusCode);
    }

    [Fact]
    public async Task ConcurrentAdditionsAndReorderingNeverExceedCapacityOrReturn500()
    {
        var setup = await CreateRoster();
        var fourth = await Register("PLAYER");
        var fifth = await Register("PLAYER");
        var responses = await Task.WhenAll(new[] { fourth, fifth }.Select(player =>
            Send(HttpMethod.Post, $"/api/v1/sessions/{setup.SessionId}/players", new { user_id = player.UserId }, setup.Token)));
        Assert.Single(responses, response => response.StatusCode == HttpStatusCode.OK);
        Assert.Single(responses, response => response.StatusCode == HttpStatusCode.UnprocessableEntity);
        foreach (var response in responses) response.Dispose();

        using var moved = await Send(HttpMethod.Post, $"/api/v1/sessions/{setup.SessionId}/players",
            new { user_id = setup.Players[2].UserId, player_order_no = 1 }, setup.Token);
        Assert.True(moved.StatusCode == HttpStatusCode.OK, await moved.Content.ReadAsStringAsync());
        await using var connection = new NpgsqlConnection(Environment.GetEnvironmentVariable("ConnectionStrings__Default"));
        var orders = (await connection.QueryAsync<int>(
            "select player_order_no from session_participants where session_id = @id order by player_order_no", new { id = setup.SessionId })).ToArray();
        Assert.Equal([1, 2, 3, 4], orders);
    }

    [Fact]
    public async Task SetupRacingAdditionAlwaysMatchesCommittedRoster()
    {
        var setup = await CreateRoster();
        var fourth = await Register("PLAYER");
        var saveTask = SessionSetupTestHelper.SaveAsync(fixture.Client, setup.Token, setup.SessionId, setup.Definition, TestContext.Current.CancellationToken);
        var addTask = Send(HttpMethod.Post, $"/api/v1/sessions/{setup.SessionId}/players", new { user_id = fourth.UserId }, setup.Token);
        await Task.WhenAll(saveTask, addTask);
        using var saved = await saveTask;
        using var added = await addTask;
        Assert.Contains(saved.StatusCode, new[] { HttpStatusCode.Created, HttpStatusCode.Conflict, HttpStatusCode.UnprocessableEntity });
        Assert.Contains(added.StatusCode, new[] { HttpStatusCode.OK, HttpStatusCode.Conflict });
        await using var connection = new NpgsqlConnection(Environment.GetEnvironmentVariable("ConnectionStrings__Default"));
        var inconsistent = await connection.ExecuteScalarAsync<int>("""
            select count(*) from session_setup_revisions r
            where r.session_id = @id and
                jsonb_array_length(r.setup_json->'players') <>
                (select count(*) from session_participants p where p.session_id = r.session_id)
            """, new { id = setup.SessionId });
        Assert.Equal(0, inconsistent);
        if (saved.IsSuccessStatusCode)
        {
            using var start = await Send(HttpMethod.Post, $"/api/v1/sessions/{setup.SessionId}/start", null, setup.Token);
            Assert.True(start.StatusCode == HttpStatusCode.OK, await start.Content.ReadAsStringAsync());
        }
    }

    private async Task<(string Token, Guid SessionId, CreateRulesetResponse Ruleset, RulesetDefinitionDto Definition, List<RegisterResponse> Players)> CreateRoster()
    {
        var token = (await Register("INSTRUCTOR")).AccessToken;
        var definition = EventAnalyticsIntegrationTests.BuildRulesetDefinition(50);
        using var rulesetResponse = await Send(HttpMethod.Post, "/api/v1/rulesets", new { name = "Roster control", definition }, token);
        rulesetResponse.EnsureSuccessStatusCode();
        var ruleset = (await rulesetResponse.Content.ReadFromJsonAsync<CreateRulesetResponse>())!;
        using var sessionResponse = await Send(HttpMethod.Post, "/api/v1/sessions", new
        {
            session_name = "Roster race", mode = definition.Mode, ruleset_version_id = ruleset.RulesetVersionId
        }, token);
        sessionResponse.EnsureSuccessStatusCode();
        var session = (await sessionResponse.Content.ReadFromJsonAsync<CreateSessionResponse>())!;
        var players = new List<RegisterResponse>();
        for (var index = 0; index < 3; index++)
        {
            var player = await Register("PLAYER");
            players.Add(player);
            using var added = await Send(HttpMethod.Post, $"/api/v1/sessions/{session.SessionId}/players", new { user_id = player.UserId }, token);
            added.EnsureSuccessStatusCode();
        }
        return (token, session.SessionId, ruleset, definition, players);
    }

    private async Task<RegisterResponse> Register(string role)
    {
        using var response = await fixture.Client.PostAsJsonAsync("/api/v1/auth/register",
            new RegisterRequest($"audit_{Guid.NewGuid():N}", "AuditRegression!2026", role, "Audit User"));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<RegisterResponse>())!;
    }

    private async Task<HttpResponseMessage> Send(HttpMethod method, string path, object? body, string token)
    {
        using var request = new HttpRequestMessage(method, path);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        if (body is not null) request.Content = JsonContent.Create(body);
        return await fixture.Client.SendAsync(request, TestContext.Current.CancellationToken);
    }
}
