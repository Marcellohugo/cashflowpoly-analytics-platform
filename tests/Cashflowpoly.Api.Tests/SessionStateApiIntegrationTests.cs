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
        Assert.Equal(13, root.GetProperty("gameConfig").GetProperty("finishDay").GetInt32());

        var rulesetId = root.GetProperty("ruleset_id").GetGuid();
        using var detailResponse = await SendJsonAsync(HttpMethod.Get, $"/api/v1/rulesets/{rulesetId}", null, token);
        Assert.Equal(HttpStatusCode.OK, detailResponse.StatusCode);
        using var detailBody = await ReadJsonAsync(detailResponse);
        Assert.Equal("MAHIR", detailBody.RootElement.GetProperty("mode").GetString());
        Assert.Equal(root.GetProperty("ruleset_version_id").GetGuid(), detailBody.RootElement.GetProperty("ruleset_version_id").GetGuid());
        Assert.Equal(10, detailBody.RootElement.GetProperty("sections").GetProperty("gameConfig").GetProperty("initialCoins").GetInt32());
        var config = detailBody.RootElement.GetProperty("config_json");
        var componentCatalog = config.GetProperty("component_catalog");
        Assert.True(componentCatalog.TryGetProperty("bahan", out _));
        Assert.True(componentCatalog.TryGetProperty("resep", out _));
        Assert.True(componentCatalog.TryGetProperty("quest", out _));

        using var componentsResponse = await SendJsonAsync(HttpMethod.Get, $"/api/v1/rulesets/{rulesetId}/components", null, token);
        Assert.Equal(HttpStatusCode.OK, componentsResponse.StatusCode);
        using var componentsBody = await ReadJsonAsync(componentsResponse);
        Assert.Equal("MAHIR", componentsBody.RootElement.GetProperty("mode").GetString());
        Assert.Equal(root.GetProperty("ruleset_version_id").GetGuid(), componentsBody.RootElement.GetProperty("ruleset_version_id").GetGuid());
        Assert.Equal(10, componentsBody.RootElement.GetProperty("sections").GetProperty("gameConfig").GetProperty("initialCoins").GetInt32());
        Assert.True(componentsBody.RootElement.GetProperty("sections").GetProperty("quest").GetArrayLength() > 0);

        using var defaultsResponse = await SendJsonAsync(HttpMethod.Get, "/api/v1/rulesets/components/defaults?mode=MAHIR", null, token);
        Assert.Equal(HttpStatusCode.OK, defaultsResponse.StatusCode);
        using var defaultsBody = await ReadJsonAsync(defaultsResponse);
        var defaultItems = defaultsBody.RootElement.GetProperty("items").EnumerateArray().ToList();
        var selectedDefault = Assert.Single(
            defaultItems,
            item => item.GetProperty("ruleset_id").GetGuid() == rulesetId);
        Assert.Equal(root.GetProperty("ruleset_version_id").GetGuid(), selectedDefault.GetProperty("ruleset_version_id").GetGuid());
        Assert.Equal(10, selectedDefault.GetProperty("sections").GetProperty("gameConfig").GetProperty("initialCoins").GetInt32());
        Assert.True(selectedDefault.GetProperty("sections").GetProperty("targetKebutuhan").GetArrayLength() > 0);

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

        var quest = Assert.Single(
            root.GetProperty("quest").EnumerateArray(),
            item => item.GetProperty("id").GetString() == "mahir_jual_3_masakan");
        Assert.Equal(5, quest.GetProperty("rewardCoins").GetInt32());
        Assert.Equal(2, quest.GetProperty("rewardHappiness").GetInt32());
    }

    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public async Task CreateSession_InitializesStartedStateForSupportedPlayerCounts(int playerCount)
    {
        var token = await RegisterInstructorAndGetTokenAsync();
        var names = Enumerable.Range(1, playerCount).Select(index => $"P{index}").ToArray();

        using var createResponse = await SendJsonAsync(
            HttpMethod.Post,
            "/api/v1/sessions",
            new
            {
                session_name = $"Session IT {Guid.NewGuid():N}",
                mode = "MAHIR",
                player_names = names
            },
            token);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        using var createBody = await ReadJsonAsync(createResponse);
        var createdState = createBody.RootElement.GetProperty("state");
        AssertInitialState(createdState, playerCount, names);

        var sessionId = createBody.RootElement.GetProperty("session_id").GetGuid();
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
    public async Task PutState_ReplacesSnapshot_RejectsStaleVersion_AndValidatesCatalogReferences()
    {
        var token = await RegisterInstructorAndGetTokenAsync();
        var createPayload = new
        {
            session_name = $"Session Save {Guid.NewGuid():N}",
            mode = "MAHIR",
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
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        using var updateBody = await ReadJsonAsync(updateResponse);
        Assert.Equal(2, updateBody.RootElement.GetProperty("state_version").GetInt64());
        Assert.Equal(2, updateBody.RootElement.GetProperty("day").GetInt32());
        Assert.Equal(2, updateBody.RootElement.GetProperty("turn").GetInt32());

        var savedFirstPlayer = updateBody.RootElement.GetProperty("players")[0];
        Assert.Equal(17, savedFirstPlayer.GetProperty("coins").GetInt32());
        Assert.Equal(3, savedFirstPlayer.GetProperty("happiness").GetInt32());
        Assert.Equal(5, savedFirstPlayer.GetProperty("saving").GetInt32());
        Assert.Equal("Nasi Putih", savedFirstPlayer.GetProperty("bahan")[0].GetProperty("nama").GetString());
        Assert.Equal(2, savedFirstPlayer.GetProperty("bahan")[0].GetProperty("jumlah").GetInt32());
        Assert.Equal("buku", savedFirstPlayer.GetProperty("kebutuhan")[0].GetProperty("nama").GetString());
        Assert.Equal("beli rumah", savedFirstPlayer.GetProperty("tujuanFinansial")[0].GetProperty("nama").GetString());
        Assert.Equal("mahir_jual_3_masakan", savedFirstPlayer.GetProperty("questProgress")[0].GetProperty("id").GetString());
        Assert.Equal(4, savedFirstPlayer.GetProperty("totalDonasi").GetInt32());
        Assert.Equal(1, updateBody.RootElement.GetProperty("donationEvents")[0].GetProperty("event_ke").GetInt32());

        using var getResponse = await SendJsonAsync(
            HttpMethod.Get,
            $"/api/v1/sessions/{sessionId}/state",
            null,
            token);
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        using var getBody = await ReadJsonAsync(getResponse);
        Assert.Equal(2, getBody.RootElement.GetProperty("state_version").GetInt64());
        Assert.Equal("Nasi Putih", getBody.RootElement.GetProperty("players")[0].GetProperty("bahan")[0].GetProperty("nama").GetString());

        using var staleResponse = await SendJsonAsync(
            HttpMethod.Put,
            $"/api/v1/sessions/{sessionId}/state",
            updatedState,
            token);
        Assert.Equal(HttpStatusCode.Conflict, staleResponse.StatusCode);

        var unknownBahanState = BuildStatePayload(
            stateVersion: 2,
            firstPlayerId,
            secondPlayerId,
            thirdPlayerId,
            firstBahanNama: "UnknownBahan");
        using var unknownBahanResponse = await SendJsonAsync(
            HttpMethod.Put,
            $"/api/v1/sessions/{sessionId}/state",
            unknownBahanState,
            token);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, unknownBahanResponse.StatusCode);
    }

    [Fact]
    public async Task SessionStateEndpoints_RejectInvalidPlayerCountNegativeValuesAndWrongOwner()
    {
        var ownerToken = await RegisterInstructorAndGetTokenAsync();
        var otherInstructorToken = await RegisterInstructorAndGetTokenAsync();

        using var invalidLowCountResponse = await SendJsonAsync(
            HttpMethod.Post,
            "/api/v1/sessions",
            new
            {
                session_name = $"Invalid Low Count {Guid.NewGuid():N}",
                mode = "MAHIR",
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
        Assert.Equal(HttpStatusCode.UnprocessableEntity, negativeCoinsResponse.StatusCode);
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
            moves_left = 1,
            finish_day = 13,
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
                    player_index = 1,
                    name = "Doni",
                    coins = firstCoins,
                    happiness = 3,
                    saving = 5,
                    bahan = new[] { new { nama = firstBahanNama, jumlah = 2 } },
                    kebutuhan = new[] { new { nama = "buku", tipe = "primer" } },
                    tujuanFinansial = new[] { new { nama = "beli rumah", purchased_at_day = 2 } },
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
                    questProgress = new[]
                    {
                        new
                        {
                            id = "mahir_jual_3_masakan",
                            progress = 1,
                            target = 3,
                            is_completed = false,
                            is_reward_claimed = false
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
                player_index = 1,
                payload = new { resep = "nasi goreng" }
            }
        };
    }

    private static object BuildEmptyPlayer(Guid sessionPlayerId, int playerIndex, string name)
    {
        return new
        {
            session_player_id = sessionPlayerId,
            player_index = playerIndex,
            name,
            coins = 10,
            happiness = 0,
            saving = 0,
            bahan = Array.Empty<object>(),
            kebutuhan = Array.Empty<object>(),
            tujuanFinansial = Array.Empty<object>(),
            targetKebutuhan = Array.Empty<object>(),
            questProgress = Array.Empty<object>(),
            actionCounters = Array.Empty<object>(),
            totalDonasi = 0
        };
    }

    private static void AssertInitialState(JsonElement state, int playerCount, string[] names)
    {
        Assert.Equal(1, state.GetProperty("state_version").GetInt64());
        Assert.Equal(1, state.GetProperty("day").GetInt32());
        Assert.Equal(1, state.GetProperty("turn").GetInt32());
        Assert.Equal(2, state.GetProperty("moves_left").GetInt32());
        Assert.Equal(13, state.GetProperty("finish_day").GetInt32());
        Assert.False(state.GetProperty("is_game_over").GetBoolean());
        Assert.Equal(JsonValueKind.Object, state.GetProperty("ui_state").ValueKind);

        var players = state.GetProperty("players").EnumerateArray().ToList();
        Assert.Equal(playerCount, players.Count);
        var expectedMissionIds = new HashSet<string>(StringComparer.Ordinal)
        {
            "misi_boneka",
            "misi_gameboy",
            "misi_hiburan",
            "misi_jam"
        };
        for (var i = 0; i < playerCount; i++)
        {
            Assert.NotEqual(Guid.Empty, players[i].GetProperty("session_player_id").GetGuid());
            Assert.NotEqual(Guid.Empty, players[i].GetProperty("user_id").GetGuid());
            Assert.Equal(i + 1, players[i].GetProperty("player_index").GetInt32());
            Assert.Equal(names[i], players[i].GetProperty("name").GetString());
            Assert.Equal(10, players[i].GetProperty("coins").GetInt32());
            Assert.Equal(0, players[i].GetProperty("happiness").GetInt32());
            Assert.Equal(0, players[i].GetProperty("saving").GetInt32());
            Assert.Equal(0, players[i].GetProperty("bahan").GetArrayLength());
            var targetKebutuhan = players[i].GetProperty("targetKebutuhan").EnumerateArray().ToList();
            var mission = Assert.Single(targetKebutuhan);
            Assert.Contains(mission.GetProperty("id").GetString()!, expectedMissionIds);
            Assert.Contains(
                players[i].GetProperty("questProgress").EnumerateArray(),
                item => item.GetProperty("id").GetString() == "mahir_jual_3_masakan" &&
                        item.GetProperty("target").GetInt32() == 3);
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
