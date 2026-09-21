// Fungsi file: Memastikan tabungan awal dapat digunakan sekali lintas tujuan dan tetap konsisten setelah finalisasi serta recompute.
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Cashflowpoly.Api.Contracts;
using Cashflowpoly.Api.Tests.Infrastructure;
using Dapper;
using Npgsql;
using Xunit;

namespace Cashflowpoly.Api.Tests;

[Collection("ApiIntegration")]
[Trait("Category", "Integration")]
public sealed class InitialSavingIntegrationTests(ApiIntegrationTestFixture fixture)
{
    [Fact]
    public async Task InitialSavingCanFundMultipleGoalsButCannotBeSpentAgain()
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];
        using var register = await fixture.Client.PostAsJsonAsync("/api/v1/auth/register",
            new RegisterRequest($"saving_{suffix}", "IntegrationSavings!2026", "INSTRUCTOR", "Savings instructor"));
        register.EnsureSuccessStatusCode();
        var account = (await register.Content.ReadFromJsonAsync<RegisterResponse>())!;
        var json = JsonSerializer.SerializeToNode(EventAnalyticsIntegrationTests.BuildRulesetDefinition(50, mode: "MAHIR"))!;
        json["settings"]!["initial_saving"] = 20;
        var definition = json.Deserialize<RulesetDefinitionDto>()!;
        definition.Actions.Add(new() { ActionId = "TujuanFinansial" });
        definition.FinancialGoals.Clear();
        foreach (var id in new[] { "first", "second", "third" })
            definition.FinancialGoals.Add(new() { Id = id, Nama = id, HargaBeli = 10, PoinKebahagiaan = 5, CardQty = 1 });
        var setup = await new EventAnalyticsIntegrationTests(fixture)
            .CreateReadySessionAsync(account.AccessToken, suffix, definition);

        await using var db = new NpgsqlConnection(Environment.GetEnvironmentVariable("ConnectionStrings__Default"));
        await db.OpenAsync();
        var sequence = setup.NextSequenceNumber;
        foreach (var id in new[] { "first", "second", "third" })
        {
            using var response = await Send(HttpMethod.Post, "/api/v1/events", new
            {
                event_id = Guid.NewGuid(), session_id = setup.SessionId, user_id = setup.ActingUserId,
                actor_type = "SYSTEM", timestamp = DateTimeOffset.UtcNow, day_index = 1, weekday = "MON",
                turn_number = 0, action_slot = 0, sequence_number = sequence++, action_type = "TujuanFinansial",
                ruleset_version_id = setup.RulesetVersionId, payload = new { goal_id = id, cost = 10, points = 5 }
            });
            Assert.True(response.StatusCode == (id == "third" ? HttpStatusCode.UnprocessableEntity : HttpStatusCode.Created),
                await response.Content.ReadAsStringAsync());
            Assert.Equal(id == "first" ? 10 : 0, await db.QuerySingleAsync<int>("""
                select b.saving from session_participant_balances b
                join session_participants p using(session_id, session_participant_id)
                where p.session_id=@SessionId and p.user_id=@ActingUserId
                """, new { setup.SessionId, setup.ActingUserId }));
        }

        foreach (var stage in new[] { "live", "ended", "recomputed" })
        {
            if (stage != "live")
            {
                using var transition = await Send(HttpMethod.Post, stage == "ended"
                    ? $"/api/v1/sessions/{setup.SessionId}/end" : $"/api/v1/analytics/sessions/{setup.SessionId}/recompute");
                Assert.True(transition.IsSuccessStatusCode, await transition.Content.ReadAsStringAsync());
            }
            using var read = await Send(HttpMethod.Get,
                $"/api/v1/analytics/sessions/{setup.SessionId}/players/{setup.ActingUserId}/gameplay");
            read.EnsureSuccessStatusCode();
            var metrics = (await read.Content.ReadFromJsonAsync<GameplayMetricsResponse>())!;
            Assert.Equal(0, metrics.RawJson!.Value.GetProperty("coins").GetProperty("coins_saved").GetInt32());
            Assert.Equal(2, await db.QuerySingleAsync<int>("""
                select count(*)::int from session_participant_financial_goals g
                join session_participants p using(session_id, session_participant_id)
                where p.session_id=@SessionId and p.user_id=@ActingUserId and g.status='COMPLETED'
                """, new { setup.SessionId, setup.ActingUserId }));
        }

        async Task<HttpResponseMessage> Send(HttpMethod method, string path, object? body = null)
        {
            using var request = new HttpRequestMessage(method, path);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", account.AccessToken);
            if (body is not null) request.Content = JsonContent.Create(body);
            return await fixture.Client.SendAsync(request, TestContext.Current.CancellationToken);
        }
    }
}
