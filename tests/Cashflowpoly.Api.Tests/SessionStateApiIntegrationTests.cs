// Fungsi file: Memverifikasi perilaku API, database, atau domain melalui SessionStateApiIntegrationTests.
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Cashflowpoly.Api.Tests.Infrastructure;
using Cashflowpoly.Api.Contracts;
using Xunit;

namespace Cashflowpoly.Api.Tests;

[Collection("ApiIntegration")]
[Trait("Category", "Integration")]
public sealed class SessionStateApiIntegrationTests
{
    private readonly HttpClient _client;

    public SessionStateApiIntegrationTests(ApiIntegrationTestFixture fixture)
    {
        _client = fixture.Client;
    }

    [Fact]
    public async Task RulesetSections_ReturnsUnityFriendlyCatalogWithoutExtraDataWrapper()
    {
        var token = await RegisterInstructorAndGetTokenAsync();

        using var response = await SendJsonAsync(HttpMethod.Get, "/api/v1/rulesets/sections?mode=MAHIR", null, token);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var body = await ReadJsonAsync(response);
        var root = body.RootElement;

        Assert.Equal("MAHIR", root.GetProperty("mode").GetString());
        Assert.NotEqual(Guid.Empty, root.GetProperty("ruleset_id").GetGuid());
        Assert.NotEqual(Guid.Empty, root.GetProperty("ruleset_version_id").GetGuid());
        Assert.Equal(10, root.GetProperty("gameConfig").GetProperty("initialCoins").GetInt32());
        Assert.Equal(2, root.GetProperty("gameConfig").GetProperty("actionsPerTurn").GetInt32());
        Assert.Equal(25, root.GetProperty("gameConfig").GetProperty("finishDay").GetInt32());

        var rulesetId = root.GetProperty("ruleset_id").GetGuid();
        using var detailResponse = await SendJsonAsync(HttpMethod.Get, $"/api/v1/rulesets/{rulesetId}", null, token);
        Assert.Equal(HttpStatusCode.OK, detailResponse.StatusCode);
        using var detailBody = await ReadJsonAsync(detailResponse);
        Assert.Equal("MAHIR", detailBody.RootElement.GetProperty("mode").GetString());
        Assert.Equal(root.GetProperty("ruleset_version_id").GetGuid(), detailBody.RootElement.GetProperty("ruleset_version_id").GetGuid());
        Assert.False(detailBody.RootElement.TryGetProperty("config_json", out _));
        var definition = detailBody.RootElement.GetProperty("definition");
        Assert.Equal(10, definition.GetProperty("settings").GetProperty("initial_coins").GetInt32());
        Assert.True(definition.GetProperty("ingredients").GetArrayLength() > 0);
        Assert.True(definition.GetProperty("orders").GetArrayLength() > 0);
        Assert.False(definition.TryGetProperty("quests", out _));

        using var componentsResponse = await SendJsonAsync(HttpMethod.Get, $"/api/v1/rulesets/{rulesetId}/components", null, token);
        Assert.Equal(HttpStatusCode.OK, componentsResponse.StatusCode);
        using var componentsBody = await ReadJsonAsync(componentsResponse);
        Assert.Equal("MAHIR", componentsBody.RootElement.GetProperty("mode").GetString());
        Assert.Equal(root.GetProperty("ruleset_version_id").GetGuid(), componentsBody.RootElement.GetProperty("ruleset_version_id").GetGuid());
        var componentsDefinition = componentsBody.RootElement.GetProperty("definition");
        Assert.Equal(10, componentsDefinition.GetProperty("settings").GetProperty("initial_coins").GetInt32());
        Assert.False(componentsDefinition.TryGetProperty("quests", out _));

        using var defaultsResponse = await SendJsonAsync(HttpMethod.Get, "/api/v1/rulesets/components/defaults?mode=MAHIR", null, token);
        Assert.Equal(HttpStatusCode.OK, defaultsResponse.StatusCode);
        using var defaultsBody = await ReadJsonAsync(defaultsResponse);
        var defaultItems = defaultsBody.RootElement.GetProperty("items").EnumerateArray().ToList();
        var selectedDefault = Assert.Single(
            defaultItems,
            item => item.GetProperty("ruleset_id").GetGuid() == rulesetId);
        Assert.Equal(root.GetProperty("ruleset_version_id").GetGuid(), selectedDefault.GetProperty("ruleset_version_id").GetGuid());
        var defaultDefinition = selectedDefault.GetProperty("definition");
        Assert.Equal(10, defaultDefinition.GetProperty("settings").GetProperty("initial_coins").GetInt32());
        Assert.True(defaultDefinition.GetProperty("collection_missions").GetArrayLength() > 0);

        var bahan = root.GetProperty("bahan").EnumerateArray().ToList();
        Assert.Equal(5, bahan.Count);
        var nasi = Assert.Single(bahan, item => item.GetProperty("nama").GetString() == "Nasi Putih");
        Assert.Equal(1, nasi.GetProperty("hargaBeli").GetInt32());

        var resep = root.GetProperty("resep").EnumerateArray().ToList();
        var nasiGoreng = Assert.Single(resep, item => item.GetProperty("nama").GetString() == "nasi goreng");
        Assert.Equal(15, nasiGoreng.GetProperty("hargaJual").GetInt32());
        Assert.Equal(0, nasiGoreng.GetProperty("poinKebahagiaan").GetInt32());
        Assert.Equal(
            new[] { "Nasi Putih", "Telur" },
            nasiGoreng.GetProperty("bahan").EnumerateArray().Select(item => item.GetString()).ToArray());

        var target = Assert.Single(
            root.GetProperty("targetKebutuhan").EnumerateArray(),
            item => item.GetProperty("id").GetString() == "misi_boneka");
        Assert.Equal("boneka", target.GetProperty("nama").GetString());
        Assert.Equal(3, target.GetProperty("kebutuhanTarget").GetArrayLength());
        Assert.Equal(10, Math.Abs(target.GetProperty("penaltyPoints").GetInt32()));

        var narasi = Assert.Single(
            root.GetProperty("narasi").EnumerateArray(),
            item => item.GetProperty("nama").GetString() == "jual_pertama");
        Assert.Equal("JualMasakan", narasi.GetProperty("prerequisiteAksi")[0].GetProperty("aksi").GetString());
        Assert.Equal(1, narasi.GetProperty("prerequisiteAksi")[0].GetProperty("value").GetInt32());

        Assert.False(root.TryGetProperty("quest", out _));
    }

    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public async Task CreateSession_InitializesStartedStateForSupportedPlayerCounts(int playerCount)
    {
        var token = await RegisterInstructorAndGetTokenAsync();
        var names = Enumerable.Range(1, playerCount).Select(index => $"P{index}").ToArray();
        var rulesetVersionId = await GetDefaultRulesetVersionIdAsync("MAHIR", token);
        using var sectionsResponse = await SendJsonAsync(
            HttpMethod.Get,
            "/api/v1/rulesets/sections?mode=MAHIR",
            null,
            token);
        using var sectionsBody = await ReadJsonAsync(sectionsResponse);
        var primaryNeedIds = sectionsBody.RootElement.GetProperty("kebutuhan")
            .EnumerateArray()
            .Where(item => item.GetProperty("tipe").GetString() == "primer")
            .Select(item => item.GetProperty("id").GetString()!)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        using var createResponse = await SendJsonAsync(
            HttpMethod.Post,
            "/api/v1/sessions",
            new
            {
                session_name = $"Session IT {Guid.NewGuid():N}",
                mode = "MAHIR",
                ruleset_version_id = rulesetVersionId,
                player_names = names
            },
            token);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        using var createBody = await ReadJsonAsync(createResponse);
        var createdState = createBody.RootElement.GetProperty("state");
        AssertInitialState(createdState, playerCount, names);

        var sessionId = createBody.RootElement.GetProperty("session_id").GetGuid();
        using var eventsResponse = await SendJsonAsync(
            HttpMethod.Get,
            $"/api/v1/sessions/{sessionId}/events?fromSeq=0&limit=100",
            null,
            token);
        Assert.Equal(HttpStatusCode.OK, eventsResponse.StatusCode);
        using var eventsBody = await ReadJsonAsync(eventsResponse);
        var setupEvents = eventsBody.RootElement.GetProperty("events").EnumerateArray().ToList();
        Assert.Equal(1 + (playerCount * 6) + 15, setupEvents.Count);
        Assert.Single(setupEvents, item => item.GetProperty("action_type").GetString() == "MulaiSesi");
        var initialMarketEvents = setupEvents
            .Where(item => item.GetProperty("action_type").GetString() == "AmbilKartuDariDeck")
            .ToList();
        Assert.Equal(15, initialMarketEvents.Count);
        Assert.All(
            new[] { "INGREDIENT_MARKET", "ORDER_MARKET", "NEED_MARKET" },
            group => Assert.Equal(
                5,
                initialMarketEvents.Count(item =>
                    item.GetProperty("payload").GetProperty("slot_group").GetString() == group)));
        Assert.All(
            initialMarketEvents.Where(item =>
                item.GetProperty("payload").GetProperty("slot_group").GetString() == "NEED_MARKET"),
            item => Assert.Contains(
                item.GetProperty("payload").GetProperty("asset_code").GetString()!,
                primaryNeedIds));

        var tieNumbers = setupEvents
            .Where(item => item.GetProperty("action_type").GetString() == "BagikanTieBreaker")
            .Select(item => item.GetProperty("payload").GetProperty("number").GetInt32())
            .OrderBy(number => number)
            .ToArray();
        Assert.Equal(Enumerable.Range(1, playerCount), tieNumbers);
        var turnOrderByUserId = createdState.GetProperty("players")
            .EnumerateArray()
            .ToDictionary(
                item => item.GetProperty("user_id").GetGuid(),
                item => item.GetProperty("player_order_no").GetInt32());
        Assert.All(
            setupEvents.Where(item => item.GetProperty("action_type").GetString() == "BagikanTieBreaker"),
            item => Assert.Equal(
                item.GetProperty("payload").GetProperty("number").GetInt32(),
                turnOrderByUserId[item.GetProperty("user_id").GetGuid()]));
        var missionIds = setupEvents
            .Where(item => item.GetProperty("action_type").GetString() == "SetupMisiAwal")
            .Select(item => item.GetProperty("payload").GetProperty("mission_id").GetString())
            .ToList();
        Assert.Equal(playerCount, missionIds.Distinct(StringComparer.OrdinalIgnoreCase).Count());

        using var getResponse = await SendJsonAsync(
            HttpMethod.Get,
            $"/api/v1/sessions/{sessionId}/state",
            null,
            token);
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        using var getBody = await ReadJsonAsync(getResponse);
        AssertInitialState(getBody.RootElement, playerCount, names);
    }

    [Fact]
    public async Task PutState_ReturnsGone_AndDoesNotMutateState()
    {
        var token = await RegisterInstructorAndGetTokenAsync();
        var rulesetVersionId = await GetDefaultRulesetVersionIdAsync("MAHIR", token);
        var createPayload = new
        {
            session_name = $"Session Save {Guid.NewGuid():N}",
            mode = "MAHIR",
            ruleset_version_id = rulesetVersionId,
            player_names = new[] { "Doni", "Rani", "Bimo" }
        };

        using var createResponse = await SendJsonAsync(HttpMethod.Post, "/api/v1/sessions", createPayload, token);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        using var createBody = await ReadJsonAsync(createResponse);

        var sessionId = createBody.RootElement.GetProperty("session_id").GetGuid();
        var initialState = createBody.RootElement.GetProperty("state");
        var players = initialState.GetProperty("players").EnumerateArray().ToList();
        var firstPlayerId = players[0].GetProperty("session_player_id").GetGuid();
        var secondPlayerId = players[1].GetProperty("session_player_id").GetGuid();
        var thirdPlayerId = players[2].GetProperty("session_player_id").GetGuid();

        var updatedState = BuildStatePayload(
            stateVersion: 1,
            firstPlayerId,
            secondPlayerId,
            thirdPlayerId,
            firstBahanNama: "Nasi Putih");
        using var updateResponse = await SendJsonAsync(
            HttpMethod.Put,
            $"/api/v1/sessions/{sessionId}/state",
            updatedState,
            token);
        Assert.Equal(HttpStatusCode.Gone, updateResponse.StatusCode);

        using var errorBody = await ReadJsonAsync(updateResponse);
        Assert.Equal("STATE_WRITE_DISABLED", errorBody.RootElement.GetProperty("error_code").GetString());

        using var getResponse = await SendJsonAsync(
            HttpMethod.Get,
            $"/api/v1/sessions/{sessionId}/state",
            null,
            token);
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        using var getBody = await ReadJsonAsync(getResponse);
        Assert.Equal(1, getBody.RootElement.GetProperty("state_version").GetInt64());
        Assert.Equal(
            players.Select(item => item.GetProperty("name").GetString()),
            getBody.RootElement.GetProperty("players").EnumerateArray().Select(item => item.GetProperty("name").GetString()));
    }

    [Fact]
    public async Task SessionStateEndpoints_RejectInvalidPlayerCountNegativeValuesAndWrongOwner()
    {
        var ownerToken = await RegisterInstructorAndGetTokenAsync();
        var otherInstructorToken = await RegisterInstructorAndGetTokenAsync();
        var rulesetVersionId = await GetDefaultRulesetVersionIdAsync("MAHIR", ownerToken);

        using var invalidLowCountResponse = await SendJsonAsync(
            HttpMethod.Post,
            "/api/v1/sessions",
            new
            {
                session_name = $"Invalid Low Count {Guid.NewGuid():N}",
                mode = "MAHIR",
                ruleset_version_id = rulesetVersionId,
                player_names = new[] { "A" }
            },
            ownerToken);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, invalidLowCountResponse.StatusCode);

        using var invalidHighCountResponse = await SendJsonAsync(
            HttpMethod.Post,
            "/api/v1/sessions",
            new
            {
                session_name = $"Invalid High Count {Guid.NewGuid():N}",
                mode = "MAHIR",
                ruleset_version_id = rulesetVersionId,
                player_names = new[] { "A", "B", "C", "D", "E" }
            },
            ownerToken);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, invalidHighCountResponse.StatusCode);

        using var createResponse = await SendJsonAsync(
            HttpMethod.Post,
            "/api/v1/sessions",
            new
            {
                session_name = $"Owner Check {Guid.NewGuid():N}",
                mode = "MAHIR",
                ruleset_version_id = rulesetVersionId,
                player_names = new[] { "Doni", "Rani", "Bimo" }
            },
            ownerToken);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        using var createBody = await ReadJsonAsync(createResponse);

        var sessionId = createBody.RootElement.GetProperty("session_id").GetGuid();
        var players = createBody.RootElement.GetProperty("state").GetProperty("players").EnumerateArray().ToList();
        var firstPlayerId = players[0].GetProperty("session_player_id").GetGuid();
        var secondPlayerId = players[1].GetProperty("session_player_id").GetGuid();
        var thirdPlayerId = players[2].GetProperty("session_player_id").GetGuid();

        using var wrongOwnerResponse = await SendJsonAsync(
            HttpMethod.Get,
            $"/api/v1/sessions/{sessionId}/state",
            null,
            otherInstructorToken);
        Assert.Equal(HttpStatusCode.NotFound, wrongOwnerResponse.StatusCode);

        using var negativeCoinsResponse = await SendJsonAsync(
            HttpMethod.Put,
            $"/api/v1/sessions/{sessionId}/state",
            BuildStatePayload(
                stateVersion: 1,
                firstPlayerId,
                secondPlayerId,
                thirdPlayerId,
            firstBahanNama: "Nasi Putih",
            firstCoins: -1),
            ownerToken);
        Assert.Equal(HttpStatusCode.Gone, negativeCoinsResponse.StatusCode);
    }

    private static object BuildStatePayload(
        long stateVersion,
        Guid firstPlayerId,
        Guid secondPlayerId,
        Guid thirdPlayerId,
        string firstBahanNama,
        int firstCoins = 17)
    {
        return new
        {
            state_version = stateVersion,
            day = 2,
            turn = 2,
            action_slots_left = 1,
            finish_day = 25,
            is_game_over = false,
            ui_state = new
            {
                SavingText = "5",
                JumatBerkah = false,
                kebutuhanSelected = "buku"
            },
            players = new object[]
            {
                new
                {
                    session_player_id = firstPlayerId,
                    player_order_no = 1,
                    name = "Doni",
                    coins = firstCoins,
                    happiness = 3,
                    saving = 5,
                    bahan = new[] { new { nama = firstBahanNama, jumlah = 2 } },
                    kebutuhan = new[] { new { nama = "buku", tipe = "primer" } },
                    tujuanFinansial = new[]
                    {
                        new
                        {
                            nama = "beli rumah",
                            current_amount = 12,
                            target_amount = 20,
                            status = "COMPLETED",
                            purchased_at_day = 2
                        }
                    },
                    targetKebutuhan = new[]
                    {
                        new
                        {
                            id = "misi_boneka",
                            is_completed = false,
                            is_failed = false,
                            reward_applied = false
                        }
                    },
                    actionCounters = new[] { new { aksi = "JualMasakan", count = 1 } },
                    totalDonasi = 4
                },
                BuildEmptyPlayer(secondPlayerId, 2, "Rani"),
                BuildEmptyPlayer(thirdPlayerId, 3, "Bimo")
            },
            donationEvents = new[]
            {
                new
                {
                    event_ke = 1,
                    day = 5,
                    rankings = new[]
                    {
                        new { rank = 1, session_player_id = firstPlayerId, total_donasi = 4 },
                        new { rank = 2, session_player_id = secondPlayerId, total_donasi = 0 },
                        new { rank = 3, session_player_id = thirdPlayerId, total_donasi = 0 }
                    }
                }
            },
            last_action = new
            {
                aksi = "JualMasakan",
                player_order_no = 1,
                payload = new { resep = "nasi goreng" }
            }
        };
    }

    private static object BuildEmptyPlayer(Guid sessionPlayerId, int playerIndex, string name)
    {
        return new
        {
            session_player_id = sessionPlayerId,
            player_order_no = playerIndex,
            name,
            coins = 10,
            happiness = 0,
            saving = 0,
            bahan = Array.Empty<object>(),
            kebutuhan = Array.Empty<object>(),
            tujuanFinansial = Array.Empty<object>(),
            targetKebutuhan = Array.Empty<object>(),
            actionCounters = Array.Empty<object>(),
            totalDonasi = 0
        };
    }

    private static void AssertInitialState(JsonElement state, int playerCount, string[] names)
    {
        Assert.Equal(1, state.GetProperty("state_version").GetInt64());
        Assert.Equal(1 + (playerCount * 6) + 15, state.GetProperty("next_sequence_number").GetInt64());
        Assert.Equal(1, state.GetProperty("day").GetInt32());
        Assert.Equal(1, state.GetProperty("turn").GetInt32());
        Assert.Equal(2, state.GetProperty("action_slots_left").GetInt32());
        Assert.Equal(25, state.GetProperty("finish_day").GetInt32());
        Assert.False(state.GetProperty("is_game_over").GetBoolean());

        var players = state.GetProperty("players").EnumerateArray().ToList();
        Assert.Equal(playerCount, players.Count);
        Assert.Equal(
            names.OrderBy(name => name, StringComparer.Ordinal),
            players.Select(item => item.GetProperty("name").GetString()!).OrderBy(name => name, StringComparer.Ordinal));
        var expectedMissionIds = new HashSet<string>(StringComparer.Ordinal)
        {
            "misi_boneka",
            "misi_gameboy",
            "misi_hiburan",
            "misi_jam"
        };
        var ingredientPrices = new Dictionary<string, int>(StringComparer.Ordinal)
        {
            ["Nasi Putih"] = 1,
            ["Sayur"] = 2,
            ["Tahu Tempe"] = 3,
            ["Telur"] = 4,
            ["Daging"] = 5
        };
        for (var i = 0; i < playerCount; i++)
        {
            Assert.NotEqual(Guid.Empty, players[i].GetProperty("session_player_id").GetGuid());
            Assert.NotEqual(Guid.Empty, players[i].GetProperty("user_id").GetGuid());
            Assert.Equal(i + 1, players[i].GetProperty("player_order_no").GetInt32());
            var initialIngredient = Assert.Single(players[i].GetProperty("bahan").EnumerateArray());
            var ingredientName = initialIngredient.GetProperty("nama").GetString()!;
            Assert.Equal(1, initialIngredient.GetProperty("jumlah").GetInt32());
            Assert.Equal(20 - ingredientPrices[ingredientName], players[i].GetProperty("coins").GetInt32());
            Assert.Equal(0, players[i].GetProperty("happiness").GetInt32());
            Assert.Equal(0, players[i].GetProperty("saving").GetInt32());
            Assert.Equal(0, players[i].GetProperty("actionCounters").GetArrayLength());
            var targetKebutuhan = players[i].GetProperty("targetKebutuhan").EnumerateArray().ToList();
            var mission = Assert.Single(targetKebutuhan);
            Assert.Contains(mission.GetProperty("id").GetString()!, expectedMissionIds);
            Assert.False(players[i].TryGetProperty("questProgress", out _));
            Assert.Equal(0, players[i].GetProperty("totalDonasi").GetInt32());
        }
    }

    private async Task<string> RegisterInstructorAndGetTokenAsync()
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];
        using var response = await _client.PostAsJsonAsync(
            "/api/v1/auth/register",
            new RegisterRequest($"session_instructor_{suffix}", "SessionInstructorPass!123", "INSTRUCTOR", null));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<RegisterResponse>();
        Assert.NotNull(body);
        return body.AccessToken;
    }

    private async Task<Guid> GetDefaultRulesetVersionIdAsync(string mode, string accessToken)
    {
        using var response = await SendJsonAsync(HttpMethod.Get, $"/api/v1/rulesets/sections?mode={mode}", null, accessToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var body = await ReadJsonAsync(response);
        return body.RootElement.GetProperty("ruleset_version_id").GetGuid();
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

    private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)
    {
        await using var stream = await response.Content.ReadAsStreamAsync();
        return await JsonDocument.ParseAsync(stream);
    }
}
