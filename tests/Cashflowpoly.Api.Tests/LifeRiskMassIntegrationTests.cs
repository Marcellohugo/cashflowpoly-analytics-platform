// Fungsi file: Memverifikasi setiap pemain dapat membayar atau memakai asuransi pada kartu risiko massal.
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Cashflowpoly.Api.Contracts;
using Cashflowpoly.Api.Tests.Infrastructure;
using Dapper;
using Npgsql;
using Xunit;

namespace Cashflowpoly.Api.Tests;

[Collection("ApiIntegration")]
[Trait("Category", "Integration")]
public sealed class LifeRiskMassIntegrationTests(ApiIntegrationTestFixture fixture)
{
    [Theory]
    [InlineData("risk_bencana_banjir", "Bencana Banjir")]
    [InlineData("risk_pemadaman_listrik", "Pemadaman Listrik")]
    public async Task MassRiskLetsEachPlayerChooseInsuranceOrPayment(string riskCode, string title)
    {
        var suffix = Guid.NewGuid().ToString("N")[..10];
        using var registration = await fixture.Client.PostAsJsonAsync("/api/v1/auth/register", new
        {
            username = $"mass_risk_{suffix}", password = "MassRiskRegression!2026", role = "INSTRUCTOR"
        });
        registration.EnsureSuccessStatusCode();
        var account = (await registration.Content.ReadFromJsonAsync<RegisterResponse>())!;
        var definition = EventAnalyticsIntegrationTests.BuildRulesetDefinition(50, mode: "MAHIR");
        definition.LifeRisks.Add(new RulesetLifeRiskDto
        {
            RiskCode = riskCode, ItemName = title, EffectType = "ALL_PLAYERS_COIN_EFFECT",
            Direction = "OUT", Amount = 3, TargetScope = "ALL_PLAYERS", DurationDays = 1, CardQty = 1
        });
        var setup = await new EventAnalyticsIntegrationTests(fixture)
            .CreateReadySessionAsync(account.AccessToken, suffix, definition);
        await using var db = new NpgsqlConnection(Environment.GetEnvironmentVariable("ConnectionStrings__Default"));
        await db.OpenAsync();
        var players = await ReadState();
        var nextSequence = setup.NextSequenceNumber;
        var orderId = await SendEvent(setup.ActingUserId, "JualMasakan", 1, new { order_card_id = "nasi_goreng" });
        var before = await ReadState();
        var riskId = await SendEvent(setup.ActingUserId, "RisikoKehidupan", 0,
            new { risk_id = riskCode, source_order_event_id = orderId });
        var parameters = new { setup.SessionId, riskId };

        var afterDraw = await ReadState();
        Assert.Equal(3, afterDraw.Count);
        Assert.All(afterDraw.Values, player => Assert.Equal(before[player.UserId!.Value].Coins, player.Coins));
        Assert.Equal("PER_PLAYER", await db.QuerySingleAsync<string>("""
            select payload->>'resolution_mode' from events where session_id=@SessionId and event_id=@riskId
            """, parameters));
        Assert.Equal(0, await db.ExecuteScalarAsync<int>("""
            select count(*) from event_cashflow_projections where session_id=@SessionId and event_id=@riskId
            """, parameters));
        Assert.All(await ReadPending(), player => Assert.True(player.Value));
        foreach (var player in players.Keys)
            await SendEvent(player, "KerjaLepas", 2, new { amount = 1 }, HttpStatusCode.UnprocessableEntity,
                "Selesaikan risiko pengeluaran");

        using var endPending = await Send(HttpMethod.Post, $"/api/v1/sessions/{setup.SessionId}/end");
        Assert.Equal(HttpStatusCode.UnprocessableEntity, endPending.StatusCode);
        var otherPlayers = players.Keys.Where(id => id != setup.ActingUserId).ToArray();
        await SendEvent(setup.ActingUserId, "Asuransi", 0, new { risk_event_id = riskId });
        var partiallyResolved = await ReadPending();
        Assert.False(partiallyResolved[setup.ActingUserId]);
        Assert.All(otherPlayers, player => Assert.True(partiallyResolved[player]));
        await SendEvent(otherPlayers[0], "Asuransi", 0, new { risk_event_id = riskId });
        await SendEvent(otherPlayers[1], "BayarRisiko", 0, new { risk_event_id = riskId });
        await SendEvent(setup.ActingUserId, "BayarRisiko", 0, new { risk_event_id = riskId }, HttpStatusCode.UnprocessableEntity);
        await SendEvent(otherPlayers[0], "Asuransi", 0, new { risk_event_id = riskId }, HttpStatusCode.UnprocessableEntity);
        await SendEvent(otherPlayers[1], "BayarRisiko", 0, new { risk_event_id = riskId }, HttpStatusCode.UnprocessableEntity);

        var after = await ReadState();
        Assert.All(after.Values, player => Assert.Equal(
            before[player.UserId!.Value].Coins - (player.UserId == otherPlayers[1] ? 3 : 0), player.Coins));
        Assert.All(await ReadPending(), player => Assert.False(player.Value));
        var insurance = await db.QueryAsync<(Guid UserId, int RemainingUses)>("""
            select player.user_id, insurance.remaining_uses
            from session_participant_insurances insurance
            join session_participants player using(session_id, session_participant_id)
            where player.session_id=@SessionId
            """, parameters);
        Assert.Equal(3, insurance.Count());
        Assert.All(insurance, item => Assert.Equal(item.UserId == otherPlayers[1] ? 1 : 0, item.RemainingUses));
        var ledger = (await db.QueryAsync<(Guid UserId, string Direction, int Amount, string Category)>("""
            select user_id, direction, amount, category from event_cashflow_projections
            where session_id=@SessionId and reference=@riskId::text
            """, parameters)).ToList();
        Assert.Equal(5, ledger.Count);
        Assert.All(ledger, item => Assert.Equal(3, item.Amount));
        Assert.Equal(3, ledger.Count(item => item.Direction == "OUT" && item.Category == "RISK_LIFE"));
        Assert.Equal(2, ledger.Count(item => item.Direction == "IN" && item.Category == "INSURANCE_OFFSET"));
        foreach (var player in players.Keys)
            Assert.Equal(player == otherPlayers[1] ? -3 : 0,
                ledger.Where(item => item.UserId == player).Sum(item => item.Direction == "IN" ? item.Amount : -item.Amount));
        using var endResolved = await Send(HttpMethod.Post, $"/api/v1/sessions/{setup.SessionId}/end");
        Assert.True(endResolved.StatusCode == HttpStatusCode.OK, await endResolved.Content.ReadAsStringAsync());

        async Task<Dictionary<Guid, bool>> ReadPending() => (await db.QueryAsync<(Guid UserId, bool Pending)>("""
            select user_id, has_pending_life_risk(session_id, user_id)
            from session_participants where session_id=@SessionId
            """, parameters)).ToDictionary(item => item.UserId, item => item.Pending);

        async Task<Dictionary<Guid, SessionPlayerStateDto>> ReadState()
        {
            using var response = await Send(HttpMethod.Get, $"/api/v1/sessions/{setup.SessionId}/state");
            response.EnsureSuccessStatusCode();
            var state = (await response.Content.ReadFromJsonAsync<SessionStateResponse>())!;
            return state.Players.ToDictionary(player => player.UserId!.Value);
        }

        async Task<Guid> SendEvent(Guid userId, string action, int slot, object payload,
            HttpStatusCode expected = HttpStatusCode.Created, string? errorText = null)
        {
            var eventId = Guid.NewGuid();
            using var response = await Send(HttpMethod.Post, "/api/v1/events", new
            {
                event_id = eventId, session_id = setup.SessionId, user_id = userId,
                actor_type = "PLAYER", timestamp = DateTimeOffset.UtcNow, day_index = 1, weekday = "MON",
                turn_number = players[userId].PlayerIndex, action_slot = slot, sequence_number = nextSequence,
                action_type = action, ruleset_version_id = setup.RulesetVersionId, payload
            });
            var content = await response.Content.ReadAsStringAsync();
            Assert.True(response.StatusCode == expected, $"{action}: {content}");
            if (errorText is not null) Assert.Contains(errorText, content);
            if (response.StatusCode == HttpStatusCode.Created) nextSequence++;
            return eventId;
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
