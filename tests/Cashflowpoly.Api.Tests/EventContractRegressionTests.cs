// Fungsi file: Menguji batas kontrak event, hak aktor, pengulangan, dan efek harga risiko.
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
public sealed class EventContractRegressionTests(ApiIntegrationTestFixture fixture)
{
    private readonly HttpClient _client = fixture.Client;
    private string _token = string.Empty;

    [Fact]
    public async Task PlayerCannotBypassAmountValidationUsingWhitespaceInActionName()
    {
        var setup = await CreateSessionAsync();
        await using var db = Connection();
        var username = await db.QuerySingleAsync<string>("select username from app_users where user_id=@PlayerId", setup);
        var password = username.StartsWith("it_evt_invalid_player_extra_", StringComparison.Ordinal)
            ? "IntegrationInvalidExtraPlayerPass!123" : "IntegrationInvalidPlayerPass!123";
        using var response = await _client.PostAsJsonAsync("/api/v1/auth/login", new { username, password });
        response.EnsureSuccessStatusCode();
        var login = await response.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.NotNull(login);
        _token = login.AccessToken;
        var request = Event(setup, "  kErJaLePaS  ", 1, new { amount = 999 });
        await ExpectAsync(request, HttpStatusCode.UnprocessableEntity);
        await ExpectAsync(request with { Payload = JsonSerializer.SerializeToElement(new { amount = 1 }) }, HttpStatusCode.Created);
        Assert.Equal("KerjaLepas", await db.QuerySingleAsync<string>(
            "select action_type from events where session_id=@SessionId and event_id=@EventId", request));
    }

    [Fact]
    public async Task Ingestion_CanonicalizesActionsAndRejectsInvalidActorsFractionsAndNullBatchBeforeWrites()
    {
        var setup = await CreateSessionAsync();
        var request = Event(setup, "KerjaLepas", 1, new { amount = 999 });
        await ExpectAsync(request, HttpStatusCode.UnprocessableEntity);
        await ExpectAsync(request with { ActionType = "  kErJaLePaS  " }, HttpStatusCode.UnprocessableEntity);
        await ExpectAsync(request with { Payload = JsonSerializer.SerializeToElement(new { amount = 1.4 }) }, HttpStatusCode.BadRequest);
        await ExpectAsync(request with { ActorType = "SYSTEM", ActionSlot = 0, TurnNumber = 0,
            Payload = JsonSerializer.SerializeToElement(new { amount = 1 }) }, HttpStatusCode.BadRequest);

        request = request with { Payload = JsonSerializer.SerializeToElement(new { amount = 1 }), ActionType = "  kErJaLePaS  " };
        using var batch = await PostAsync("/api/v1/events/batch",
            new { events = new EventRequest?[] { request, null, request with { EventId = Guid.NewGuid(), SequenceNumber = request.SequenceNumber + 1, ActionSlot = 2 } } });
        Assert.Equal(HttpStatusCode.BadRequest, batch.StatusCode);
        await using var db = Connection();
        Assert.Equal(0, await db.ExecuteScalarAsync<int>("select count(*) from events where session_id=@SessionId and event_id=@EventId", request));

        await ExpectAsync(request, HttpStatusCode.Created);
        await ExpectAsync(request with { EventId = Guid.NewGuid(), SequenceNumber = request.SequenceNumber + 1, ActionSlot = 2 }, HttpStatusCode.Created);
        await ExpectAsync(request, HttpStatusCode.Conflict);
        Assert.Equal("KerjaLepas", await db.ExecuteScalarAsync<string>("select action_type from events where session_id=@SessionId and event_id=@EventId", request));
        await ExpectAsync(Event(setup, "AkhiriSesi", 0, new { }) with
        { ActorType = "SYSTEM", UserId = null, TurnNumber = 0, SequenceNumber = request.SequenceNumber + 2 }, HttpStatusCode.UnprocessableEntity);
        Assert.Equal("STARTED", await db.ExecuteScalarAsync<string>("select status from sessions where session_id=@SessionId", request));
    }

    [Fact]
    public async Task SystemGoalPurchase_DoesNotConsumePlayerSlotAndUsesEventDay()
    {
        var definition = EventAnalyticsIntegrationTests.BuildRulesetDefinition(50, mode: "MAHIR");
        definition.FinancialGoals[0] = new RulesetFinancialGoalDto { Id = "beli_rumah", Nama = "Rumah", HargaBeli = 10, PoinKebahagiaan = 5 };
        definition.Actions.Add(new RulesetActionDto { ActionId = "Menabung" });
        definition.Actions.Add(new RulesetActionDto { ActionId = "TujuanFinansial" });
        var setup = await CreateSessionAsync(definition);
        await ExpectAsync(Event(setup, "Menabung", 1, new { goal_id = "beli_rumah", amount = 10 }), HttpStatusCode.Created);
        await ExpectAsync(Event(setup, "TujuanFinansial", 0, new { goal_id = "beli_rumah", cost = 10, points = 5 }) with
        { ActorType = "SYSTEM", TurnNumber = 0, SequenceNumber = setup.Sequence + 1 }, HttpStatusCode.Created);
        await ExpectAsync(Event(setup, "KerjaLepas", 2, new { amount = 1 }) with { SequenceNumber = setup.Sequence + 2 }, HttpStatusCode.Created);
        await using var db = Connection();
        Assert.Equal(1, await db.ExecuteScalarAsync<int>("""
            select goal.purchased_at_day from session_participant_financial_goals goal
            join session_participants player on player.session_participant_id=goal.session_participant_id
            where goal.session_id=@SessionId and player.user_id=@PlayerId
            """, setup));
    }

    [Fact]
    public async Task OrderClaim_ConsumesInitialIngredientWithoutAdditionalPurchase()
    {
        var definition = EventAnalyticsIntegrationTests.BuildRulesetDefinition(50);
        definition.Orders[0] = new RulesetOrderDto { Id = "nasi_goreng", Nama = "nasi goreng", HargaJual = 15, Bahan = ["Nasi Putih"], CardQty = 6 };
        var setup = await CreateSessionAsync(definition);
        var claim = Event(setup, "JualMasakan", 1, new { order_card_id = "nasi_goreng" });
        await ExpectAsync(claim, HttpStatusCode.Created);
        await ExpectAsync(claim with { EventId = Guid.NewGuid(), SequenceNumber = setup.Sequence + 1, ActionSlot = 2 }, HttpStatusCode.UnprocessableEntity);
        await using var db = Connection();
        Assert.Equal(0, await db.ExecuteScalarAsync<int>("""
            select coalesce(sum(ingredient.qty),0) from session_participant_inventory ingredient
            join session_participants player on player.session_participant_id=ingredient.session_participant_id
            where player.session_id=@SessionId and player.user_id=@PlayerId
            """, setup));
    }

    [Theory]
    [InlineData("ALL_PLAYERS", -1, "", 0, 0, 0)]
    [InlineData("SELF", null, "IN", 2, 3, 1)]
    [InlineData("OTHER_PLAYERS", null, "OUT", 1, 1, 0)]
    public async Task IngredientPrices_UseValueDeltaAndRiskSourceTargetInApiAndSql(
        string target, int? valueDelta, string direction, int amount, int firstPrice, int secondPrice)
    {
        var definition = EventAnalyticsIntegrationTests.BuildRulesetDefinition(50, mode: "MAHIR");
        definition.LifeRisks.Add(new RulesetLifeRiskDto { RiskCode = "price_risk", ItemName = "Harga bahan", EffectType = "INGREDIENT_PRICE_MODIFIER",
            TargetScope = target, ValueDelta = valueDelta, Direction = direction, Amount = amount, DurationDays = 2, CardQty = 1 });
        var setup = await CreateSessionAsync(definition);
        var order = Event(setup, "JualMasakan", 1, new { order_card_id = "nasi_goreng" });
        await ExpectAsync(order, HttpStatusCode.Created);
        await ExpectAsync(Event(setup, "RisikoKehidupan", 0, new { risk_id = "price_risk", source_order_event_id = order.EventId }) with
        { SequenceNumber = setup.Sequence + 1 }, HttpStatusCode.Created);
        var purchase = Event(setup, "BahanMasakan", 2, new { card_id = "nasi_putih", amount = firstPrice }) with { SequenceNumber = setup.Sequence + 2 };
        await ExpectAsync(purchase with { Payload = JsonSerializer.SerializeToElement(new { card_id = "nasi_putih", amount = firstPrice + 1 }) }, HttpStatusCode.UnprocessableEntity);
        await ExpectAsync(purchase, HttpStatusCode.Created);
        await using var db = Connection();
        var secondPlayer = await db.ExecuteScalarAsync<Guid>("select user_id from session_participants where session_id=@SessionId and player_order_no=2", setup);
        var secondPurchase = purchase with { EventId = Guid.NewGuid(), UserId = secondPlayer, TurnNumber = 2, ActionSlot = 1,
            SequenceNumber = setup.Sequence + 3, Payload = JsonSerializer.SerializeToElement(new { card_id = "nasi_putih", amount = secondPrice }) };
        await ExpectAsync(secondPurchase with { Payload = JsonSerializer.SerializeToElement(new { card_id = "nasi_putih", amount = secondPrice + 1 }) }, HttpStatusCode.UnprocessableEntity);
        await ExpectAsync(secondPurchase, HttpStatusCode.Created);
        Assert.Equal(valueDelta ?? (direction == "IN" ? amount : -amount), await db.ExecuteScalarAsync<int>(
            "select value_delta from session_rule_effects where session_id=@SessionId and effect_type='INGREDIENT_PRICE_MODIFIER'", setup));
    }

    [Fact]
    public async Task FreeCatalogIngredient_AcceptsZeroAndChecksCatalogPrice()
    {
        var definition = EventAnalyticsIntegrationTests.BuildRulesetDefinition(50);
        definition.Ingredients[0] = new RulesetIngredientDto { Id = "nasi_putih", Nama = "Nasi Putih", HargaBeli = 0 };
        var setup = await CreateSessionAsync(definition);
        var purchase = Event(setup, "BahanMasakan", 1, new { card_id = "nasi_putih", amount = 0 });
        await ExpectAsync(purchase with { Payload = JsonSerializer.SerializeToElement(new { card_id = "nasi_putih", amount = -1 }) }, HttpStatusCode.BadRequest);
        await ExpectAsync(purchase with { Payload = JsonSerializer.SerializeToElement(new { card_id = "nasi_putih", amount = 1 }) }, HttpStatusCode.UnprocessableEntity);
        await ExpectAsync(purchase, HttpStatusCode.Created);
    }

    private async Task<Setup> CreateSessionAsync(RulesetDefinitionDto? definition = null)
    {
        var suffix = Guid.NewGuid().ToString("N")[..12];
        var username = $"event_fix_{suffix}";
        const string password = "EventFixRegression!2026";
        using var registered = await _client.PostAsJsonAsync("/api/v1/auth/register", new { username, password, role = "INSTRUCTOR" });
        registered.EnsureSuccessStatusCode();
        using var loggedIn = await _client.PostAsJsonAsync("/api/v1/auth/login", new { username, password });
        var login = await loggedIn.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.NotNull(login);
        _token = login.AccessToken;
        var result = await new EventAnalyticsIntegrationTests(fixture).CreateReadySessionAsync(login.AccessToken, suffix, definition);
        return new Setup(result.SessionId, result.ActingUserId, result.RulesetVersionId, result.NextSequenceNumber);
    }

    private static EventRequest Event(Setup setup, string action, int slot, object payload) => new(
        Guid.NewGuid(), setup.SessionId, setup.PlayerId, "PLAYER", DateTimeOffset.UtcNow, 1, "MON", slot,
        setup.Sequence, action, setup.RulesetVersionId, JsonSerializer.SerializeToElement(payload), null, 1);

    private async Task ExpectAsync(EventRequest request, HttpStatusCode expected)
    {
        using var response = await PostAsync("/api/v1/events", request);
        Assert.True(response.StatusCode == expected, $"{request.ActionType}: expected {expected}, got {response.StatusCode}: {await response.Content.ReadAsStringAsync()}");
    }

    private async Task<HttpResponseMessage> PostAsync(string path, object body)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, path) { Content = JsonContent.Create(body) };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        return await _client.SendAsync(request);
    }

    private static NpgsqlConnection Connection() => new(Environment.GetEnvironmentVariable("ConnectionStrings__Default"));
    private sealed record Setup(Guid SessionId, Guid PlayerId, Guid RulesetVersionId, long Sequence);
}
