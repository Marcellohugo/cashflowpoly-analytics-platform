// Fungsi file: Memastikan bahan pembagian awal dapat dipakai menjual masakan sesuai resep dan saldo tercatat dengan benar.
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
public sealed class MealSaleIntegrationTests(ApiIntegrationTestFixture fixture)
{
    [Theory]
    [InlineData("PEMULA", "telur", "nasi_goreng")]
    [InlineData("MAHIR", "telur", "nasi_goreng")]
    [InlineData("PEMULA", "sayur", "lontong_balap")]
    [InlineData("MAHIR", "sayur", "lontong_balap")]
    [InlineData("PEMULA", null, "initial_meal")]
    [InlineData("MAHIR", null, "initial_meal")]
    public async Task SellingMealConsumesSetupIngredientAndCreditsCatalogIncome(
        string mode, string? purchasedIngredientId, string orderId)
    {
        var suffix = Guid.NewGuid().ToString("N")[..12];
        using var registration = await fixture.Client.PostAsJsonAsync("/api/v1/auth/register", new
        {
            username = $"meal_sale_{suffix}", password = "MealSaleRegression!2026", role = "INSTRUCTOR"
        });
        registration.EnsureSuccessStatusCode();
        var account = (await registration.Content.ReadFromJsonAsync<RegisterResponse>())!;

        RulesetDefinitionDto definition;
        if (purchasedIngredientId is null)
        {
            definition = EventAnalyticsIntegrationTests.BuildRulesetDefinition(50, mode: mode);
            definition.Orders.Clear();
            definition.Orders.Add(new RulesetOrderDto
            {
                Id = orderId, Nama = "Masakan Satu Bahan", HargaJual = 5,
                Bahan = ["Nasi Putih"], CardQty = 2
            });
        }
        else
        {
            using var defaultsResponse = await Send(HttpMethod.Get, $"/api/v1/rulesets/components/defaults?mode={mode}");
            defaultsResponse.EnsureSuccessStatusCode();
            var defaults = (await defaultsResponse.Content.ReadFromJsonAsync<DefaultRulesetComponentsResponse>())!;
            definition = Assert.Single(defaults.Items).Definition!;
        }

        var setup = await new EventAnalyticsIntegrationTests(fixture)
            .CreateReadySessionAsync(account.AccessToken, suffix, definition);
        await using var db = new NpgsqlConnection(Environment.GetEnvironmentVariable("ConnectionStrings__Default"));
        await db.OpenAsync(TestContext.Current.CancellationToken);
        var parameters = new { setup.SessionId, UserId = setup.ActingUserId };
        var nextSequence = setup.NextSequenceNumber;
        var initial = await ReadPlayer();
        var initialIngredient = Assert.Single(initial.Bahan);
        Assert.Equal("Nasi Putih", initialIngredient.Nama);
        Assert.Equal(1, initialIngredient.Jumlah);
        Assert.Equal(1, await db.ExecuteScalarAsync<int>("""
            select count(*) from events where session_id=@SessionId and user_id=@UserId
              and action_type='SetupBahanAwal' and payload->>'card_id'='nasi_putih'
            """, parameters));

        var purchaseCost = 0;
        if (purchasedIngredientId is not null)
        {
            // Resep bawaan memerlukan dua bahan; penolakan ini tidak boleh menghabiskan bahan awal atau slot aksi.
            await SendEvent("JualMasakan", 1, new { order_card_id = orderId }, HttpStatusCode.UnprocessableEntity,
                "Bahan tidak mencukupi");
            var rejectedState = await ReadPlayer();
            Assert.Equal(initial.Coins, rejectedState.Coins);
            Assert.Equal(initialIngredient, Assert.Single(rejectedState.Bahan));
            Assert.Equal(0, await db.ExecuteScalarAsync<int>("""
                select count(*) from events where session_id=@SessionId and user_id=@UserId and action_type='JualMasakan'
                """, parameters));

            var ingredient = Assert.Single(definition.Ingredients, item => item.Id == purchasedIngredientId);
            purchaseCost = ingredient.HargaBeli;
            await SendEvent("BahanMasakan", 1, new { card_id = ingredient.Id, amount = purchaseCost });
        }

        var order = Assert.Single(definition.Orders, item => item.Id == orderId);
        var saleId = await SendEvent("JualMasakan", purchasedIngredientId is null ? 1 : 2,
            new { order_card_id = orderId });
        var soldState = await ReadPlayer();
        Assert.Equal(initial.Coins - purchaseCost + order.HargaJual, soldState.Coins);
        Assert.Equal(0, soldState.Bahan.Sum(item => item.Jumlah));
        var ledger = Assert.Single(await db.QueryAsync<(Guid UserId, string Direction, int Amount, string Category)>("""
            select user_id, direction, amount, category from event_cashflow_projections
            where session_id=@SessionId and event_id=@saleId
            """, new { setup.SessionId, saleId }));
        Assert.Equal(setup.ActingUserId, ledger.UserId);
        Assert.Equal("IN", ledger.Direction);
        Assert.Equal(order.HargaJual, ledger.Amount);
        Assert.Equal("ORDER", ledger.Category);

        var purchasedCards = (await db.QueryAsync<string>("""
            select payload->>'card_id' from events
            where session_id=@SessionId and user_id=@UserId and action_type='BahanMasakan'
            """, parameters)).ToList();
        if (purchasedIngredientId is null)
            Assert.Empty(purchasedCards);
        else
            Assert.Equal(purchasedIngredientId, Assert.Single(purchasedCards));

        if (mode == "MAHIR")
        {
            var risk = definition.LifeRisks.First(item => item.EffectType == "COIN_EFFECT" && item.TargetScope == "SELF");
            await SendEvent("RisikoKehidupan", 0, new { risk_id = risk.RiskCode, source_order_event_id = saleId });
        }

        async Task<SessionPlayerStateDto> ReadPlayer()
        {
            using var response = await Send(HttpMethod.Get, $"/api/v1/sessions/{setup.SessionId}/state");
            response.EnsureSuccessStatusCode();
            var state = (await response.Content.ReadFromJsonAsync<SessionStateResponse>())!;
            return Assert.Single(state.Players, player => player.UserId == setup.ActingUserId);
        }

        async Task<Guid> SendEvent(string action, int slot, object payload,
            HttpStatusCode expectedStatus = HttpStatusCode.Created, string? errorText = null)
        {
            var eventId = Guid.NewGuid();
            using var response = await Send(HttpMethod.Post, "/api/v1/events", new
            {
                event_id = eventId, session_id = setup.SessionId, user_id = setup.ActingUserId,
                actor_type = "PLAYER", timestamp = DateTimeOffset.UtcNow, day_index = 1, weekday = "MON",
                turn_number = 1, action_slot = slot, sequence_number = nextSequence,
                action_type = action, ruleset_version_id = setup.RulesetVersionId, payload
            });
            var content = await response.Content.ReadAsStringAsync();
            Assert.True(response.StatusCode == expectedStatus, $"{mode} {action}: {(int)response.StatusCode} {content}");
            if (errorText is not null) Assert.Contains(errorText, content);
            if (response.IsSuccessStatusCode) nextSequence++;
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
