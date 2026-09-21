// Fungsi file: Memeriksa saldo awal dan misi custom melalui API, finalisasi, dan penghitungan ulang.
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
public sealed class AnalyticsAuditIntegrationTests(ApiIntegrationTestFixture fixture)
{
    [Theory]
    [InlineData(0)]
    [InlineData(20)]
    public async Task InitialSavingAndAssetMissionStayConsistentThroughFinalizationAndRecompute(int initialSaving)
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];
        using var register = await fixture.Client.PostAsJsonAsync("/api/v1/auth/register",
            new RegisterRequest($"metrics_{suffix}", "IntegrationMetrics!2026", "INSTRUCTOR", "Metrics Instructor"));
        Assert.Equal(HttpStatusCode.Created, register.StatusCode);
        var account = await register.Content.ReadFromJsonAsync<RegisterResponse>();
        Assert.NotNull(account);

        var definitionJson = JsonSerializer.SerializeToNode(
            EventAnalyticsIntegrationTests.BuildRulesetDefinition(50, mode: "MAHIR"))!;
        definitionJson["settings"]!["initial_saving"] = initialSaving;
        var definition = definitionJson.Deserialize<RulesetDefinitionDto>()!;
        for (var index = 0; index < definition.CollectionMissions.Count; index++)
        {
            var mission = definition.CollectionMissions[index];
            definition.CollectionMissions[index] = new RulesetCollectionMissionDto
            {
                Id = mission.Id, Nama = "Koleksi Buku", PenaltyPoints = 10, FailurePoints = -10,
                KebutuhanTarget = [new() { Order = 1, Type = "ASSET", Value = "buku" }]
            };
        }

        var setup = await new EventAnalyticsIntegrationTests(fixture)
            .CreateReadySessionAsync(account.AccessToken, suffix, definition);
        await using var connection = new NpgsqlConnection(Environment.GetEnvironmentVariable("ConnectionStrings__Default"));
        await connection.OpenAsync();
        var initialBalance = await connection.QuerySingleAsync<(int Coins, int Saving)>("""
            select b.coins, b.saving from session_participant_balances b
            join session_participants p using(session_id, session_participant_id)
            where p.session_id=@sessionId and p.user_id=@userId
            """, new { setup.SessionId, userId = setup.ActingUserId });
        Assert.Equal(initialSaving, initialBalance.Saving);

        using var purchase = await SendAsync(HttpMethod.Post, "/api/v1/events", account.AccessToken, new
        {
            event_id = Guid.NewGuid(), session_id = setup.SessionId, user_id = setup.ActingUserId,
            actor_type = "PLAYER", timestamp = DateTimeOffset.UtcNow, day_index = 1, weekday = "MON",
            turn_number = 1, action_slot = 1, sequence_number = setup.NextSequenceNumber,
            action_type = "Kebutuhan", ruleset_version_id = setup.RulesetVersionId,
            payload = new { card_id = "buku", amount = 3, points = 1, need_tier = "primer" }
        });
        Assert.True(purchase.StatusCode == HttpStatusCode.Created, await purchase.Content.ReadAsStringAsync());

        // Verify live reads, final snapshots, and a repeated recompute against persisted game state.
        for (var stage = 0; stage < 3; stage++)
        {
            if (stage > 0)
            {
                var path = stage == 1
                    ? $"/api/v1/sessions/{setup.SessionId}/end"
                    : $"/api/v1/analytics/sessions/{setup.SessionId}/recompute";
                using var update = await SendAsync(HttpMethod.Post, path, account.AccessToken);
                Assert.True(update.StatusCode == HttpStatusCode.OK, await update.Content.ReadAsStringAsync());
            }

            using var response = await SendAsync(HttpMethod.Get,
                $"/api/v1/analytics/sessions/{setup.SessionId}/players/{setup.ActingUserId}/gameplay", account.AccessToken);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var gameplay = await response.Content.ReadFromJsonAsync<GameplayMetricsResponse>();
            Assert.NotNull(gameplay?.RawJson);
            Assert.NotNull(gameplay.DerivedJson);
            var raw = gameplay.RawJson.Value;
            var coins = raw.GetProperty("coins");
            var held = initialBalance.Coins - 3;
            Assert.Equal(held, coins.GetProperty("coins_held_current").GetInt32());
            Assert.Equal(initialSaving, coins.GetProperty("coins_saved").GetInt32());
            Assert.Equal(held + initialSaving + 1, raw.GetProperty("pension").GetProperty("pension_fund_total").GetInt32());
            Assert.Equal(held + initialSaving, gameplay.DerivedJson.Value
                .GetProperty("loan_burden_components").GetProperty("liquid_assets").GetDouble());
            Assert.True(raw.GetProperty("needs").GetProperty("collection_mission_complete").GetBoolean());
            Assert.Equal(0, gameplay.DerivedJson.Value
                .GetProperty("happiness_points_composition").GetProperty("mission_penalty_points").GetDouble());
            Assert.True(await connection.QuerySingleAsync<bool>("""
                select m.is_completed and not m.is_failed
                from session_participant_collection_missions m
                join session_participants p using(session_id, session_participant_id)
                where p.session_id=@sessionId and p.user_id=@userId
                """, new { setup.SessionId, userId = setup.ActingUserId }));
        }
    }

    private async Task<HttpResponseMessage> SendAsync(HttpMethod method, string path, string token, object? body = null)
    {
        using var request = new HttpRequestMessage(method, path);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        if (body is not null) request.Content = JsonContent.Create(body);
        return await fixture.Client.SendAsync(request, TestContext.Current.CancellationToken);
    }
}
