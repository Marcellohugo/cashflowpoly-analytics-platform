// Fungsi file: Memastikan hadiah Ulang Tahun memindahkan koin antar saldo pemain tepat satu kali.
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Cashflowpoly.Api.Contracts;
using Cashflowpoly.Api.Tests.Infrastructure;
using Xunit;

namespace Cashflowpoly.Api.Tests;

[Collection("ApiIntegration")]
[Trait("Category", "Integration")]
public sealed class LifeRiskTransferIntegrationTests(ApiIntegrationTestFixture fixture)
{
    [Fact]
    public async Task BirthdayTransfersOneCoinFromEachOtherPlayerAndReplayDoesNotRepeatIt()
    {
        var suffix = Guid.NewGuid().ToString("N")[..10];
        using var registration = await fixture.Client.PostAsJsonAsync("/api/v1/auth/register", new
        {
            username = $"birthday_{suffix}", password = "BirthdayRegression!2026", role = "INSTRUCTOR"
        });
        registration.EnsureSuccessStatusCode();
        var account = (await registration.Content.ReadFromJsonAsync<RegisterResponse>())!;
        var definition = EventAnalyticsIntegrationTests.BuildRulesetDefinition(50, mode: "MAHIR");
        definition.LifeRisks.Add(new RulesetLifeRiskDto
        {
            RiskCode = "risk_ulang_tahun", ItemName = "Ulang Tahun",
            EffectType = "PLAYER_TO_PLAYER_TRANSFER", Direction = "IN", Amount = 1,
            TargetScope = "OTHER_PLAYERS", DurationDays = 1, CardQty = 1
        });
        var setup = await new EventAnalyticsIntegrationTests(fixture)
            .CreateReadySessionAsync(account.AccessToken, suffix, definition);
        var orderEventId = Guid.NewGuid();
        using var order = await Send(HttpMethod.Post, "/api/v1/events", new
        {
            event_id = orderEventId, session_id = setup.SessionId, user_id = setup.ActingUserId,
            actor_type = "PLAYER", timestamp = DateTimeOffset.UtcNow, day_index = 1, weekday = "MON",
            turn_number = 1, action_slot = 1, sequence_number = setup.NextSequenceNumber,
            action_type = "JualMasakan", ruleset_version_id = setup.RulesetVersionId,
            payload = new { order_card_id = "nasi_goreng" }
        });
        Assert.True(order.StatusCode == HttpStatusCode.Created, await order.Content.ReadAsStringAsync());
        var before = await ReadBalances();
        Assert.Equal(3, before.Count);

        var risk = new
        {
            event_id = Guid.NewGuid(), session_id = setup.SessionId, user_id = setup.ActingUserId,
            actor_type = "PLAYER", timestamp = DateTimeOffset.UtcNow, day_index = 1, weekday = "MON",
            turn_number = 1, action_slot = 0, sequence_number = setup.NextSequenceNumber + 1,
            action_type = "RisikoKehidupan", ruleset_version_id = setup.RulesetVersionId,
            payload = new { risk_id = "risk_ulang_tahun", source_order_event_id = orderEventId }
        };
        for (var attempt = 0; attempt < 2; attempt++)
        {
            using var response = await Send(HttpMethod.Post, "/api/v1/events", risk);
            var expectedStatus = attempt == 0 ? HttpStatusCode.Created : HttpStatusCode.Conflict;
            Assert.True(response.StatusCode == expectedStatus, await response.Content.ReadAsStringAsync());
            var after = await ReadBalances();
            Assert.Equal(before.Values.Sum(), after.Values.Sum());
            foreach (var player in before)
            {
                var delta = player.Key == setup.ActingUserId ? before.Count - 1 : -1;
                Assert.Equal(player.Value + delta, after[player.Key]);
            }
        }

        async Task<Dictionary<Guid, int>> ReadBalances()
        {
            using var response = await Send(HttpMethod.Get, $"/api/v1/sessions/{setup.SessionId}/state");
            response.EnsureSuccessStatusCode();
            var state = (await response.Content.ReadFromJsonAsync<SessionStateResponse>())!;
            return state.Players.ToDictionary(player => player.UserId!.Value, player => player.Coins);
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
