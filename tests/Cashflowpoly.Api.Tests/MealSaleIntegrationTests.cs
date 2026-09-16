// Fungsi file: Menguji penjualan masakan dengan katalog dan narasi ruleset bawaan.
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Cashflowpoly.Api.Contracts;
using Cashflowpoly.Api.Tests.Infrastructure;
using Xunit;

namespace Cashflowpoly.Api.Tests;

[Collection("ApiIntegration")]
public sealed class MealSaleIntegrationTests(ApiIntegrationTestFixture fixture)
{
    [Theory]
    [InlineData("PEMULA")]
    [InlineData("MAHIR")]
    public async Task DefaultCatalog_SellingMealAfterBuyingRequiredIngredients_ReturnsCreated(string mode)
    {
        var client = fixture.Client;
        var suffix = Guid.NewGuid().ToString("N")[..12];
        using var registration = await client.PostAsJsonAsync("/api/v1/auth/register", new
        {
            username = $"meal_sale_{suffix}", password = "MealSaleRegression!2026", role = "INSTRUCTOR"
        });
        registration.EnsureSuccessStatusCode();
        var account = await registration.Content.ReadFromJsonAsync<RegisterResponse>();
        Assert.NotNull(account);

        using var defaultsRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/rulesets/components/defaults?mode={mode}");
        defaultsRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", account.AccessToken);
        using var defaultsResponse = await client.SendAsync(defaultsRequest);
        defaultsResponse.EnsureSuccessStatusCode();
        var defaults = await defaultsResponse.Content.ReadFromJsonAsync<DefaultRulesetComponentsResponse>();
        var definitionJson = JsonSerializer.SerializeToNode(Assert.Single(defaults!.Items).Definition)!;
        // Buy both recipe ingredients explicitly before selling in the same turn.
        definitionJson["settings"]!["actions_per_turn"] = 3;
        var definition = definitionJson.Deserialize<RulesetDefinitionDto>()!;
        var setup = await new EventAnalyticsIntegrationTests(fixture).CreateReadySessionAsync(account.AccessToken, suffix, definition);

        async Task SendAsync(string action, int slot, object payload)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/events");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", account.AccessToken);
            request.Content = JsonContent.Create(new
            {
                event_id = Guid.NewGuid(), session_id = setup.SessionId, user_id = setup.ActingUserId,
                actor_type = "PLAYER", timestamp = DateTimeOffset.UtcNow, day_index = 1, weekday = "MON",
                turn_number = 1, action_slot = slot, sequence_number = setup.NextSequenceNumber + slot - 1,
                action_type = action, ruleset_version_id = setup.RulesetVersionId, payload
            });
            using var response = await client.SendAsync(request);
            Assert.True(response.StatusCode == HttpStatusCode.Created,
                $"{mode} {action}: {(int)response.StatusCode} {await response.Content.ReadAsStringAsync()}");
        }

        var rice = Assert.Single(definition.Ingredients, ingredient => ingredient.Id == "nasi_putih");
        var egg = Assert.Single(definition.Ingredients, ingredient => ingredient.Id == "telur");
        await SendAsync("BahanMasakan", 1, new { card_id = rice.Id, amount = rice.HargaBeli });
        await SendAsync("BahanMasakan", 2, new { card_id = egg.Id, amount = egg.HargaBeli });
        await SendAsync("JualMasakan", 3, new { order_card_id = "nasi_goreng" });
    }
}
