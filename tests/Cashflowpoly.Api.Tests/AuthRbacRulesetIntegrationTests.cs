// Fungsi file: Memverifikasi perilaku API, database, atau domain melalui AuthRbacRulesetIntegrationTests.
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Cashflowpoly.Api.Contracts;
using Cashflowpoly.Api.Tests.Infrastructure;
using Xunit;

namespace Cashflowpoly.Api.Tests;

[Collection("ApiIntegration")]
[Trait("Category", "Integration")]
/// <summary>
/// Kelas pengujian integrasi yang memvalidasi alur registrasi, login, pembatasan akses RBAC,
/// serta operasi CRUD dan versioning ruleset secara end-to-end melalui API.
/// </summary>
public sealed class AuthRbacRulesetIntegrationTests
{
    private readonly HttpClient _client;

    /// <summary>
    /// Menginisialisasi instance pengujian dengan HttpClient dari fixture integrasi bersama.
    /// </summary>
    public AuthRbacRulesetIntegrationTests(ApiIntegrationTestFixture fixture)
    {
        _client = fixture.Client;
    }

    [Fact]
    /// <summary>
    /// Memvalidasi alur lengkap: registrasi INSTRUCTOR/PLAYER, login, pembatasan akses tanpa token,
    /// CRUD ruleset dengan RBAC, aktivasi versi, penghapusan versi draft, dan pembuatan sesi permainan.
    /// </summary>
    public async Task Auth_Rbac_And_RulesetFlow_Work_EndToEnd()
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];
        var instructorUsername = $"it_instructor_{suffix}";
        var playerUsername = $"it_player_{suffix}";
        var unrelatedPlayerUsername = $"it_player_unrelated_{suffix}";
        const string instructorPassword = "IntegrationInstructorPass!123";
        const string playerPassword = "IntegrationPlayerPass!123";

        var instructorRegister = await RegisterAsync(instructorUsername, instructorPassword, "INSTRUCTOR");
        var playerRegister = await RegisterAsync(playerUsername, playerPassword, "PLAYER");
        var unrelatedPlayerRegister = await RegisterAsync(unrelatedPlayerUsername, playerPassword, "PLAYER");

        Assert.Equal("INSTRUCTOR", instructorRegister.Role);
        Assert.Equal("PLAYER", playerRegister.Role);
        Assert.Equal("PLAYER", unrelatedPlayerRegister.Role);

        var instructorLogin = await LoginAsync(instructorUsername, instructorPassword);
        var playerLogin = await LoginAsync(playerUsername, playerPassword);
        var unrelatedPlayerLogin = await LoginAsync(unrelatedPlayerUsername, playerPassword);

        Assert.False(string.IsNullOrWhiteSpace(instructorLogin.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(playerLogin.AccessToken));

        var withoutToken = await _client.GetAsync("/api/v1/sessions");
        Assert.Equal(HttpStatusCode.Unauthorized, withoutToken.StatusCode);

        var instructorRulesetsBeforeCreate = await SendJsonAsync(
            HttpMethod.Get,
            "/api/v1/rulesets",
            body: null,
            instructorLogin.AccessToken);
        Assert.Equal(HttpStatusCode.OK, instructorRulesetsBeforeCreate.StatusCode);
        await AssertRulesetListIncludesDefaultRowsAsync(instructorRulesetsBeforeCreate);

        var playerRulesetsBeforeSession = await SendJsonAsync(
            HttpMethod.Get,
            "/api/v1/rulesets",
            body: null,
            playerLogin.AccessToken);
        Assert.Equal(HttpStatusCode.OK, playerRulesetsBeforeSession.StatusCode);
        await AssertRulesetListIncludesDefaultRowsAsync(playerRulesetsBeforeSession);

        var defaultComponentsByInstructor = await SendJsonAsync(
            HttpMethod.Get,
            "/api/v1/rulesets/components/defaults",
            body: null,
            instructorLogin.AccessToken);
        Assert.Equal(HttpStatusCode.OK, defaultComponentsByInstructor.StatusCode);

        var defaultComponentsInstructorPayload =
            await defaultComponentsByInstructor.Content.ReadFromJsonAsync<DefaultRulesetComponentsResponse>();
        Assert.NotNull(defaultComponentsInstructorPayload);
        Assert.NotNull(defaultComponentsInstructorPayload.Items);
        if (defaultComponentsInstructorPayload.Items.Count > 0)
        {
            var defaultRuleset = defaultComponentsInstructorPayload.Items[0];
            var defaultRulesetDetailByInstructor = await SendJsonAsync(
                HttpMethod.Get,
                $"/api/v1/rulesets/{defaultRuleset.RulesetId}",
                body: null,
                instructorLogin.AccessToken);
            Assert.Equal(HttpStatusCode.OK, defaultRulesetDetailByInstructor.StatusCode);

            var defaultRulesetComponentsByInstructor = await SendJsonAsync(
                HttpMethod.Get,
                $"/api/v1/rulesets/{defaultRuleset.RulesetId}/components",
                body: null,
                instructorLogin.AccessToken);
            Assert.Equal(HttpStatusCode.OK, defaultRulesetComponentsByInstructor.StatusCode);
        }

        var defaultComponentsByPlayer = await SendJsonAsync(
            HttpMethod.Get,
            "/api/v1/rulesets/components/defaults",
            body: null,
            playerLogin.AccessToken);
        Assert.Equal(HttpStatusCode.OK, defaultComponentsByPlayer.StatusCode);

        var gameComponentsPemula = await SendJsonAsync(
            HttpMethod.Get,
            "/api/v1/game-components?mode=PEMULA",
            body: null,
            instructorLogin.AccessToken);
        Assert.Equal(HttpStatusCode.OK, gameComponentsPemula.StatusCode);

        var gameComponentsPemulaPayload =
            await gameComponentsPemula.Content.ReadFromJsonAsync<DefaultRulesetComponentsResponse>();
        Assert.NotNull(gameComponentsPemulaPayload);
        Assert.NotNull(gameComponentsPemulaPayload.Items);
        Assert.All(gameComponentsPemulaPayload.Items, item =>
            Assert.Equal("PEMULA", (item.Mode ?? string.Empty).ToUpperInvariant()));

        var gameComponentsInvalidMode = await SendJsonAsync(
            HttpMethod.Get,
            "/api/v1/game-components?mode=INVALID",
            body: null,
            instructorLogin.AccessToken);
        Assert.Equal(HttpStatusCode.BadRequest, gameComponentsInvalidMode.StatusCode);

        var createRulesetPayload = new
        {
            name = $"Ruleset IT {suffix}",
            description = "Integration test ruleset",
            definition = BuildRulesetDefinition(startingCash: 20)
        };

        var playerCreateRuleset = await SendJsonAsync(
            HttpMethod.Post,
            "/api/v1/rulesets",
            createRulesetPayload,
            playerLogin.AccessToken);
        Assert.Equal(HttpStatusCode.Forbidden, playerCreateRuleset.StatusCode);

        var instructorCreateRuleset = await SendJsonAsync(
            HttpMethod.Post,
            "/api/v1/rulesets",
            createRulesetPayload,
            instructorLogin.AccessToken);
        Assert.Equal(HttpStatusCode.Created, instructorCreateRuleset.StatusCode);

        var createdRuleset = await instructorCreateRuleset.Content.ReadFromJsonAsync<CreateRulesetResponse>();
        Assert.NotNull(createdRuleset);
        Assert.NotEqual(Guid.Empty, createdRuleset.RulesetId);
        Assert.Equal(1, createdRuleset.Version);

        var getComponentsResponse = await SendJsonAsync(
            HttpMethod.Get,
            $"/api/v1/rulesets/{createdRuleset.RulesetId}/components",
            body: null,
            instructorLogin.AccessToken);
        Assert.Equal(HttpStatusCode.OK, getComponentsResponse.StatusCode);

        var components = await getComponentsResponse.Content.ReadFromJsonAsync<RulesetComponentsResponse>();
        Assert.NotNull(components);
        Assert.Equal(createdRuleset.RulesetId, components.RulesetId);
        Assert.Equal(1, components.Version);
        Assert.Equal("PEMULA", components.Mode);
        Assert.NotNull(components.Definition);
        Assert.NotEmpty(components.Definition!.Actions);
        Assert.Contains(
            components.Definition.Narratives.SelectMany(item => item.PrerequisiteAksi),
            item => item.Aksi == "JualMasakan" && item.Value == 1);

        var duplicateConfigUpdatePayload = new
        {
            name = $"Ruleset IT {suffix} duplicate",
            description = "Integration test duplicate config",
            definition = BuildRulesetDefinition(startingCash: 20)
        };

        var duplicateConfigUpdate = await SendJsonAsync(
            HttpMethod.Put,
            $"/api/v1/rulesets/{createdRuleset.RulesetId}",
            duplicateConfigUpdatePayload,
            instructorLogin.AccessToken);
        Assert.Equal(HttpStatusCode.Conflict, duplicateConfigUpdate.StatusCode);

        var updateRulesetPayload = new
        {
            name = $"Ruleset IT {suffix} V2",
            description = "Integration test ruleset v2",
            definition = BuildRulesetDefinition(startingCash: 21)
        };

        var instructorUpdateRuleset = await SendJsonAsync(
            HttpMethod.Put,
            $"/api/v1/rulesets/{createdRuleset.RulesetId}",
            updateRulesetPayload,
            instructorLogin.AccessToken);
        Assert.Equal(HttpStatusCode.OK, instructorUpdateRuleset.StatusCode);

        var updatedRuleset = await instructorUpdateRuleset.Content.ReadFromJsonAsync<CreateRulesetResponse>();
        Assert.NotNull(updatedRuleset);
        Assert.Equal(createdRuleset.RulesetId, updatedRuleset.RulesetId);
        Assert.True(updatedRuleset.Version >= 2);

        var playerActivateVersion = await SendJsonAsync(
            HttpMethod.Post,
            $"/api/v1/rulesets/{createdRuleset.RulesetId}/versions/{updatedRuleset.Version}/activate",
            body: null,
            playerLogin.AccessToken);
        Assert.Equal(HttpStatusCode.Forbidden, playerActivateVersion.StatusCode);

        var instructorActivateVersion = await SendJsonAsync(
            HttpMethod.Post,
            $"/api/v1/rulesets/{createdRuleset.RulesetId}/versions/{updatedRuleset.Version}/activate",
            body: null,
            instructorLogin.AccessToken);
        Assert.Equal(HttpStatusCode.OK, instructorActivateVersion.StatusCode);

        var updateRulesetPayloadV3 = new
        {
            name = $"Ruleset IT {suffix} V3",
            description = "Integration test ruleset v3",
            definition = BuildRulesetDefinition(startingCash: 22)
        };

        var instructorUpdateRulesetV3 = await SendJsonAsync(
            HttpMethod.Put,
            $"/api/v1/rulesets/{createdRuleset.RulesetId}",
            updateRulesetPayloadV3,
            instructorLogin.AccessToken);
        Assert.Equal(HttpStatusCode.OK, instructorUpdateRulesetV3.StatusCode);

        var updatedRulesetV3 = await instructorUpdateRulesetV3.Content.ReadFromJsonAsync<CreateRulesetResponse>();
        Assert.NotNull(updatedRulesetV3);
        Assert.True(updatedRulesetV3.Version > updatedRuleset.Version);

        var playerDeleteVersion = await SendJsonAsync(
            HttpMethod.Delete,
            $"/api/v1/rulesets/{createdRuleset.RulesetId}/versions/{updatedRulesetV3.Version}",
            body: null,
            playerLogin.AccessToken);
        Assert.Equal(HttpStatusCode.Forbidden, playerDeleteVersion.StatusCode);

        var instructorDeleteActiveVersion = await SendJsonAsync(
            HttpMethod.Delete,
            $"/api/v1/rulesets/{createdRuleset.RulesetId}/versions/{updatedRuleset.Version}",
            body: null,
            instructorLogin.AccessToken);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, instructorDeleteActiveVersion.StatusCode);

        var instructorDeleteDraftVersion = await SendJsonAsync(
            HttpMethod.Delete,
            $"/api/v1/rulesets/{createdRuleset.RulesetId}/versions/{updatedRulesetV3.Version}",
            body: null,
            instructorLogin.AccessToken);
        Assert.Equal(HttpStatusCode.NoContent, instructorDeleteDraftVersion.StatusCode);

        var rulesetDetailAfterDeleteVersion = await SendJsonAsync(
            HttpMethod.Get,
            $"/api/v1/rulesets/{createdRuleset.RulesetId}",
            body: null,
            instructorLogin.AccessToken);
        Assert.Equal(HttpStatusCode.OK, rulesetDetailAfterDeleteVersion.StatusCode);

        var rulesetDetailPayload = await rulesetDetailAfterDeleteVersion.Content.ReadFromJsonAsync<RulesetDetailResponse>();
        Assert.NotNull(rulesetDetailPayload);
        Assert.DoesNotContain(rulesetDetailPayload.Versions, v => v.Version == updatedRulesetV3.Version);

        var createSessionPayload = new
        {
            session_name = $"Session IT {suffix}",
            mode = "PEMULA",
            ruleset_version_id = updatedRuleset.RulesetVersionId
        };

        var playerCreateSession = await SendJsonAsync(
            HttpMethod.Post,
            "/api/v1/sessions",
            createSessionPayload,
            playerLogin.AccessToken);
        Assert.Equal(HttpStatusCode.Forbidden, playerCreateSession.StatusCode);

        var instructorCreateSession = await SendJsonAsync(
            HttpMethod.Post,
            "/api/v1/sessions",
            createSessionPayload,
            instructorLogin.AccessToken);
        Assert.Equal(HttpStatusCode.Created, instructorCreateSession.StatusCode);

        var createdSession = await instructorCreateSession.Content.ReadFromJsonAsync<CreateSessionResponse>();
        Assert.NotNull(createdSession);
        Assert.NotEqual(Guid.Empty, createdSession.SessionId);

        var extraPlayerOneUsername = $"it_player_extra1_{suffix}";
        var extraPlayerTwoUsername = $"it_player_extra2_{suffix}";
        await CreatePlayerAsync(instructorLogin.AccessToken, $"Player Extra 1 {suffix}", extraPlayerOneUsername);
        await CreatePlayerAsync(instructorLogin.AccessToken, $"Player Extra 2 {suffix}", extraPlayerTwoUsername);

        foreach (var assignment in new[]
                 {
                     new { username = playerUsername, player_order_no = 1 },
                     new { username = extraPlayerOneUsername, player_order_no = 2 },
                     new { username = extraPlayerTwoUsername, player_order_no = 3 }
                 })
        {
            var addPlayerResponse = await SendJsonAsync(
                HttpMethod.Post,
                $"/api/v1/sessions/{createdSession.SessionId}/players",
                assignment,
                instructorLogin.AccessToken);
            Assert.Equal(HttpStatusCode.OK, addPlayerResponse.StatusCode);
        }

        var sessionPlayersResponse = await SendJsonAsync(
            HttpMethod.Get,
            $"/api/v1/sessions/{createdSession.SessionId}/players",
            body: null,
            playerLogin.AccessToken);
        Assert.Equal(HttpStatusCode.OK, sessionPlayersResponse.StatusCode);

        var sessionPlayers = await sessionPlayersResponse.Content.ReadFromJsonAsync<SessionPlayerListResponse>();
        Assert.NotNull(sessionPlayers);
        Assert.Equal(3, sessionPlayers.Items.Count);
        Assert.Equal(new[] { 1, 2, 3 }, sessionPlayers.Items.Select(item => item.PlayerOrder));
        Assert.Contains(sessionPlayers.Items, item => item.DisplayName.Contains(suffix, StringComparison.Ordinal));

        var unrelatedSessionPlayersResponse = await SendJsonAsync(
            HttpMethod.Get,
            $"/api/v1/sessions/{createdSession.SessionId}/players",
            body: null,
            unrelatedPlayerLogin.AccessToken);
        Assert.Equal(HttpStatusCode.Forbidden, unrelatedSessionPlayersResponse.StatusCode);

        var playerStartSession = await SendJsonAsync(
            HttpMethod.Post,
            $"/api/v1/sessions/{createdSession.SessionId}/start",
            body: null,
            playerLogin.AccessToken);
        Assert.Equal(HttpStatusCode.Forbidden, playerStartSession.StatusCode);

        var instructorStartSession = await SendJsonAsync(
            HttpMethod.Post,
            $"/api/v1/sessions/{createdSession.SessionId}/start",
            body: null,
            instructorLogin.AccessToken);
        Assert.Equal(HttpStatusCode.OK, instructorStartSession.StatusCode);

        var startResponse = await instructorStartSession.Content.ReadFromJsonAsync<SessionStatusResponse>();
        Assert.NotNull(startResponse);
        Assert.Equal("STARTED", startResponse.Status);
    }

    [Fact]
    public async Task DefaultRulesets_AreReadonly_AndExposeLockMetadata()
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];
        var instructor = await RegisterAsync($"it_default_guard_{suffix}", "IntegrationInstructorPass!123", "INSTRUCTOR");

        var listResponse = await SendJsonAsync(
            HttpMethod.Get,
            "/api/v1/rulesets",
            body: null,
            instructor.AccessToken);
        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);

        using var listDocument = JsonDocument.Parse(await listResponse.Content.ReadAsStringAsync());
        var defaultItem = listDocument.RootElement.GetProperty("items")
            .EnumerateArray()
            .First(item => item.GetProperty("is_default").GetBoolean());
        Assert.False(defaultItem.GetProperty("is_locked_by_session").GetBoolean());

        var defaultRulesetId = defaultItem.GetProperty("ruleset_id").GetGuid();
        var defaultVersion = defaultItem.GetProperty("latest_version").GetInt32();

        var detailResponse = await SendJsonAsync(
            HttpMethod.Get,
            $"/api/v1/rulesets/{defaultRulesetId}",
            body: null,
            instructor.AccessToken);
        Assert.Equal(HttpStatusCode.OK, detailResponse.StatusCode);

        using var detailDocument = JsonDocument.Parse(await detailResponse.Content.ReadAsStringAsync());
        Assert.True(detailDocument.RootElement.GetProperty("is_default").GetBoolean());
        Assert.False(detailDocument.RootElement.GetProperty("is_locked_by_session").GetBoolean());

        var updateResponse = await SendJsonAsync(
            HttpMethod.Put,
            $"/api/v1/rulesets/{defaultRulesetId}",
            new
            {
                name = "Attempt default update",
                description = "Should be rejected",
                definition = BuildRulesetDefinition(startingCash: 31)
            },
            instructor.AccessToken);
        await AssertDomainRuleViolationAsync(updateResponse);

        var activateResponse = await SendJsonAsync(
            HttpMethod.Post,
            $"/api/v1/rulesets/{defaultRulesetId}/versions/{defaultVersion}/activate",
            body: null,
            instructor.AccessToken);
        await AssertDomainRuleViolationAsync(activateResponse);

        var deleteVersionResponse = await SendJsonAsync(
            HttpMethod.Delete,
            $"/api/v1/rulesets/{defaultRulesetId}/versions/{defaultVersion}",
            body: null,
            instructor.AccessToken);
        await AssertDomainRuleViolationAsync(deleteVersionResponse);

        var deleteResponse = await SendJsonAsync(
            HttpMethod.Delete,
            $"/api/v1/rulesets/{defaultRulesetId}",
            body: null,
            instructor.AccessToken);
        await AssertDomainRuleViolationAsync(deleteResponse);
    }

    [Fact]
    public async Task CustomRuleset_UsedOnlyByCreatedSession_RemainsMutable()
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];
        var instructor = await RegisterAsync($"it_created_guard_{suffix}", "IntegrationInstructorPass!123", "INSTRUCTOR");
        var createdRuleset = await CreateRulesetAsync(instructor.AccessToken, suffix, startingCash: 41);
        await CreateSessionAsync(instructor.AccessToken, suffix, createdRuleset.RulesetVersionId);

        var detailBeforeUpdate = await SendJsonAsync(
            HttpMethod.Get,
            $"/api/v1/rulesets/{createdRuleset.RulesetId}",
            body: null,
            instructor.AccessToken);
        Assert.Equal(HttpStatusCode.OK, detailBeforeUpdate.StatusCode);
        using (var detailDocument = JsonDocument.Parse(await detailBeforeUpdate.Content.ReadAsStringAsync()))
        {
            Assert.False(detailDocument.RootElement.GetProperty("is_default").GetBoolean());
            Assert.False(detailDocument.RootElement.GetProperty("is_locked_by_session").GetBoolean());
        }

        var updateResponse = await SendJsonAsync(
            HttpMethod.Put,
            $"/api/v1/rulesets/{createdRuleset.RulesetId}",
            new
            {
                name = $"Ruleset CREATED Mutable {suffix} V2",
                description = "CREATED sessions do not lock rulesets",
                definition = BuildRulesetDefinition(startingCash: 42)
            },
            instructor.AccessToken);
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var deleteResponse = await SendJsonAsync(
            HttpMethod.Delete,
            $"/api/v1/rulesets/{createdRuleset.RulesetId}",
            body: null,
            instructor.AccessToken);
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
    }

    [Fact]
    public async Task CustomRuleset_UsedByStartedOrEndedSession_IsReadonly()
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];
        var instructor = await RegisterAsync($"it_locked_guard_{suffix}", "IntegrationInstructorPass!123", "INSTRUCTOR");
        var createdRuleset = await CreateRulesetAsync(instructor.AccessToken, suffix, startingCash: 51);
        var createdSession = await CreateSessionAsync(instructor.AccessToken, suffix, createdRuleset.RulesetVersionId);

        await AddPlayersForStartAsync(instructor.AccessToken, createdSession.SessionId, suffix);

        var startResponse = await SendJsonAsync(
            HttpMethod.Post,
            $"/api/v1/sessions/{createdSession.SessionId}/start",
            body: null,
            instructor.AccessToken);
        Assert.Equal(HttpStatusCode.OK, startResponse.StatusCode);

        var detailAfterStart = await SendJsonAsync(
            HttpMethod.Get,
            $"/api/v1/rulesets/{createdRuleset.RulesetId}",
            body: null,
            instructor.AccessToken);
        Assert.Equal(HttpStatusCode.OK, detailAfterStart.StatusCode);
        using (var detailDocument = JsonDocument.Parse(await detailAfterStart.Content.ReadAsStringAsync()))
        {
            Assert.False(detailDocument.RootElement.GetProperty("is_default").GetBoolean());
            Assert.True(detailDocument.RootElement.GetProperty("is_locked_by_session").GetBoolean());
        }

        var listAfterStart = await SendJsonAsync(
            HttpMethod.Get,
            "/api/v1/rulesets",
            body: null,
            instructor.AccessToken);
        Assert.Equal(HttpStatusCode.OK, listAfterStart.StatusCode);
        using (var listDocument = JsonDocument.Parse(await listAfterStart.Content.ReadAsStringAsync()))
        {
            var lockedItem = listDocument.RootElement.GetProperty("items")
                .EnumerateArray()
                .First(item => item.GetProperty("ruleset_id").GetGuid() == createdRuleset.RulesetId);
            Assert.True(lockedItem.GetProperty("is_locked_by_session").GetBoolean());
        }

        var updateResponse = await SendJsonAsync(
            HttpMethod.Put,
            $"/api/v1/rulesets/{createdRuleset.RulesetId}",
            new
            {
                name = $"Ruleset Locked {suffix} V2",
                description = "Should be rejected",
                definition = BuildRulesetDefinition(startingCash: 52)
            },
            instructor.AccessToken);
        await AssertDomainRuleViolationAsync(updateResponse);

        var activateResponse = await SendJsonAsync(
            HttpMethod.Post,
            $"/api/v1/rulesets/{createdRuleset.RulesetId}/versions/{createdRuleset.Version}/activate",
            body: null,
            instructor.AccessToken);
        await AssertDomainRuleViolationAsync(activateResponse);

        var deleteVersionResponse = await SendJsonAsync(
            HttpMethod.Delete,
            $"/api/v1/rulesets/{createdRuleset.RulesetId}/versions/{createdRuleset.Version}",
            body: null,
            instructor.AccessToken);
        await AssertDomainRuleViolationAsync(deleteVersionResponse);

        var deleteResponse = await SendJsonAsync(
            HttpMethod.Delete,
            $"/api/v1/rulesets/{createdRuleset.RulesetId}",
            body: null,
            instructor.AccessToken);
        await AssertDomainRuleViolationAsync(deleteResponse);

        var endResponse = await SendJsonAsync(
            HttpMethod.Post,
            $"/api/v1/sessions/{createdSession.SessionId}/end",
            body: null,
            instructor.AccessToken);
        Assert.Equal(HttpStatusCode.OK, endResponse.StatusCode);

        var detailAfterEnd = await SendJsonAsync(
            HttpMethod.Get,
            $"/api/v1/rulesets/{createdRuleset.RulesetId}",
            body: null,
            instructor.AccessToken);
        Assert.Equal(HttpStatusCode.OK, detailAfterEnd.StatusCode);
        using var endedDetailDocument = JsonDocument.Parse(await detailAfterEnd.Content.ReadAsStringAsync());
        Assert.True(endedDetailDocument.RootElement.GetProperty("is_locked_by_session").GetBoolean());
    }

    [Fact]
    /// <summary>
    /// Memvalidasi bahwa kebijakan password diterapkan pada endpoint registrasi dan
    /// pembuatan player, menolak password yang terlalu pendek dengan error VALIDATION_ERROR.
    /// </summary>
    public async Task PasswordPolicy_IsEnforced_ForRegisterAndCreatePlayer()
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];
        var registerPayload = new RegisterRequest(
            $"it_shortpass_{suffix}",
            "Short1!",
            "PLAYER",
            null);

        var registerResponse = await _client.PostAsJsonAsync("/api/v1/auth/register", registerPayload);
        Assert.Equal(HttpStatusCode.BadRequest, registerResponse.StatusCode);

        var registerError = await registerResponse.Content.ReadFromJsonAsync<ErrorResponse>();
        Assert.NotNull(registerError);
        Assert.Equal("VALIDATION_ERROR", registerError.ErrorCode);

        var instructorUsername = $"it_pwd_instructor_{suffix}";
        const string instructorPassword = "IntegrationPolicyInstructorPass!123";
        var instructorToken = (await RegisterAsync(instructorUsername, instructorPassword, "INSTRUCTOR")).AccessToken;

        var createPlayerResponse = await SendJsonAsync(
            HttpMethod.Post,
            "/api/v1/players",
            new
            {
                display_name = $"Player Policy {suffix}",
                username = $"it_pwd_player_{suffix}",
                password = "Short1!"
            },
            instructorToken);
        Assert.Equal(HttpStatusCode.BadRequest, createPlayerResponse.StatusCode);

        var createPlayerError = await createPlayerResponse.Content.ReadFromJsonAsync<ErrorResponse>();
        Assert.NotNull(createPlayerError);
        Assert.Equal("VALIDATION_ERROR", createPlayerError.ErrorCode);
    }

    /// <summary>
    /// Helper untuk mendaftarkan pengguna baru dan mengembalikan data registrasi.
    /// </summary>
    private async Task<RegisterResponse> RegisterAsync(string username, string password, string role)
    {
        var payload = new RegisterRequest(username, password, role, null);
        var response = await _client.PostAsJsonAsync("/api/v1/auth/register", payload);
        var responseText = await response.Content.ReadAsStringAsync();
        Assert.True(
            response.StatusCode == HttpStatusCode.Created,
            $"Expected Created but got {response.StatusCode}. Body: {responseText}");

        var body = JsonSerializer.Deserialize<RegisterResponse>(responseText);
        Assert.NotNull(body);
        return body;
    }

    /// <summary>
    /// Helper untuk melakukan login dan mengembalikan data token akses.
    /// </summary>
    private async Task<LoginResponse> LoginAsync(string username, string password)
    {
        var payload = new LoginRequest(username, password);
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", payload);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.NotNull(body);
        return body;
    }

    private async Task CreatePlayerAsync(string accessToken, string displayName, string username)
    {
        var response = await SendJsonAsync(
            HttpMethod.Post,
            "/api/v1/players",
            new
            {
                display_name = displayName,
                username,
                password = "IntegrationPlayerPass!123"
            },
            accessToken);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    private async Task<CreateRulesetResponse> CreateRulesetAsync(string accessToken, string suffix, int startingCash)
    {
        var response = await SendJsonAsync(
            HttpMethod.Post,
            "/api/v1/rulesets",
            new
            {
                name = $"Ruleset Lock Guard {suffix}",
                description = "Ruleset lock guard integration test",
                definition = BuildRulesetDefinition(startingCash)
            },
            accessToken);
        var responseText = await response.Content.ReadAsStringAsync();
        Assert.True(
            response.StatusCode == HttpStatusCode.Created,
            $"Expected Created but got {response.StatusCode}. Body: {responseText}");

        var body = JsonSerializer.Deserialize<CreateRulesetResponse>(responseText);
        Assert.NotNull(body);
        return body;
    }

    private async Task<CreateSessionResponse> CreateSessionAsync(string accessToken, string suffix, Guid rulesetVersionId)
    {
        var response = await SendJsonAsync(
            HttpMethod.Post,
            "/api/v1/sessions",
            new
            {
                session_name = $"Ruleset Lock Session {suffix}",
                mode = "PEMULA",
                ruleset_version_id = rulesetVersionId
            },
            accessToken);
        var responseText = await response.Content.ReadAsStringAsync();
        Assert.True(
            response.StatusCode == HttpStatusCode.Created,
            $"Expected Created but got {response.StatusCode}. Body: {responseText}");

        var body = JsonSerializer.Deserialize<CreateSessionResponse>(responseText);
        Assert.NotNull(body);
        return body;
    }

    private async Task AddPlayersForStartAsync(string accessToken, Guid sessionId, string suffix)
    {
        var firstUsername = $"it_lock_player1_{suffix}";
        var secondUsername = $"it_lock_player2_{suffix}";
        await CreatePlayerAsync(accessToken, $"Lock Player 1 {suffix}", firstUsername);
        await CreatePlayerAsync(accessToken, $"Lock Player 2 {suffix}", secondUsername);

        foreach (var assignment in new[]
                 {
                     new { username = firstUsername, player_order_no = 1 },
                     new { username = secondUsername, player_order_no = 2 }
                 })
        {
            var addPlayerResponse = await SendJsonAsync(
                HttpMethod.Post,
                $"/api/v1/sessions/{sessionId}/players",
                assignment,
                accessToken);
            Assert.Equal(HttpStatusCode.OK, addPlayerResponse.StatusCode);
        }
    }

    private static async Task AssertDomainRuleViolationAsync(HttpResponseMessage response)
    {
        var responseText = await response.Content.ReadAsStringAsync();
        Assert.True(
            response.StatusCode == HttpStatusCode.UnprocessableEntity,
            $"Expected UnprocessableEntity but got {response.StatusCode}. Body: {responseText}");

        var error = JsonSerializer.Deserialize<ErrorResponse>(responseText);
        Assert.NotNull(error);
        Assert.Equal("DOMAIN_RULE_VIOLATION", error.ErrorCode);
    }

    /// <summary>
    /// Helper untuk mengirim HTTP request dengan body JSON dan header Bearer token.
    /// </summary>
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

    private static async Task AssertRulesetListIncludesDefaultRowsAsync(HttpResponseMessage response)
    {
        var json = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(json);
        var items = document.RootElement.GetProperty("items").EnumerateArray().ToList();
        var defaultItems = items
            .Where(item => item.TryGetProperty("is_default", out var isDefault) && isDefault.GetBoolean())
            .ToList();

        Assert.Equal(2, defaultItems.Count);
        Assert.Contains(defaultItems, item => string.Equals(item.GetProperty("name").GetString(), "Cashflowpoly Default - Mode Pemula", StringComparison.Ordinal));
        Assert.Contains(defaultItems, item => string.Equals(item.GetProperty("name").GetString(), "Cashflowpoly Default - Mode Mahir", StringComparison.Ordinal));
        Assert.Contains(defaultItems, item => string.Equals(item.GetProperty("mode").GetString(), "PEMULA", StringComparison.Ordinal));
        Assert.Contains(defaultItems, item => string.Equals(item.GetProperty("mode").GetString(), "MAHIR", StringComparison.Ordinal));
        Assert.All(defaultItems, item => Assert.Equal("ACTIVE", item.GetProperty("status").GetString()));
        Assert.All(defaultItems, item => Assert.False(item.GetProperty("is_locked_by_session").GetBoolean()));
    }

    /// <summary>
    /// Helper yang membangun objek konfigurasi ruleset lengkap untuk mode PEMULA
    /// dengan parameter starting cash yang dapat dikustomisasi.
    /// </summary>
    private static RulesetDefinitionDto BuildRulesetDefinition(int startingCash)
    {
        return new RulesetDefinitionDto
        {
            Mode = "PEMULA",
            Settings = new RulesetSettingsDto
            {
                ActionsPerTurn = 2,
                StartingCash = startingCash,
                InitialCoins = startingCash,
                InitialHappiness = 0,
                InitialSaving = 0,
                FinishDay = 25,
                MinPlayers = 2,
                MaxPlayers = 4,
                CashMin = 0,
                MaxIngredientTotal = 6,
                MaxSameIngredient = 3,
                PrimaryNeedMaxPerDay = 1,
                RequirePrimaryBeforeOthers = true,
                DonationMinAmount = 1,
                DonationMaxAmount = 999999,
                GoldTradeAllowBuy = true,
                GoldTradeAllowSell = true,
                LoanEnabled = false,
                InsuranceEnabled = false,
                SavingGoalEnabled = false,
                FreelanceIncome = 1
            },
            PlayerOrdering = new RulesetPlayerOrderingDto
            {
                OrderingCode = "PLAYER_ORDER",
                FridayFeature = "DONATION",
                FridayEnabled = true,
                SaturdayFeature = "GOLD_TRADE",
                SaturdayEnabled = true,
                SundayFeature = "REST",
                SundayEnabled = true
            },
            Actions =
            [
                new RulesetActionDto { ActionId = "BahanMasakan" },
                new RulesetActionDto { ActionId = "JualMasakan" },
                new RulesetActionDto { ActionId = "KerjaLepas" }
            ],
            Ingredients =
            [
                new RulesetIngredientDto
                {
                    Id = "nasi_putih",
                    Nama = "Nasi Putih",
                    HargaBeli = 1
                },
                new RulesetIngredientDto
                {
                    Id = "telur",
                    Nama = "Telur",
                    HargaBeli = 4
                },
                new RulesetIngredientDto
                {
                    Id = "sayur",
                    Nama = "Sayur",
                    HargaBeli = 2
                }
            ],
            Orders =
            {
                new RulesetOrderDto
                {
                    Id = "nasi_goreng",
                    Nama = "nasi goreng",
                    HargaJual = 15,
                    PoinKebahagiaan = 0,
                    Bahan = ["Nasi Putih", "Telur"],
                    CardQty = 5
                }
            },
            Needs =
            {
                new RulesetNeedDto
                {
                    Id = "buku",
                    Nama = "buku",
                    Tipe = "primer",
                    HargaBeli = 2,
                    PoinKebahagiaan = 1
                },
                new RulesetNeedDto
                {
                    Id = "baju",
                    Nama = "baju",
                    Tipe = "primer",
                    HargaBeli = 2,
                    PoinKebahagiaan = 1
                },
                new RulesetNeedDto
                {
                    Id = "sepatu",
                    Nama = "sepatu",
                    Tipe = "primer",
                    HargaBeli = 2,
                    PoinKebahagiaan = 1
                },
                new RulesetNeedDto
                {
                    Id = "tempat_makan",
                    Nama = "tempat makan",
                    Tipe = "primer",
                    HargaBeli = 2,
                    PoinKebahagiaan = 1
                },
                new RulesetNeedDto
                {
                    Id = "alat_tulis",
                    Nama = "alat tulis",
                    Tipe = "primer",
                    HargaBeli = 2,
                    PoinKebahagiaan = 1
                },
                new RulesetNeedDto
                {
                    Id = "boneka",
                    Nama = "boneka",
                    Tipe = "tersier",
                    HargaBeli = 6,
                    PoinKebahagiaan = 3
                },
                new RulesetNeedDto
                {
                    Id = "gameboy", Nama = "gameboy", Tipe = "tersier", HargaBeli = 6, PoinKebahagiaan = 3
                },
                new RulesetNeedDto
                {
                    Id = "hiburan", Nama = "hiburan", Tipe = "tersier", HargaBeli = 6, PoinKebahagiaan = 3
                },
                new RulesetNeedDto
                {
                    Id = "jam", Nama = "jam", Tipe = "tersier", HargaBeli = 6, PoinKebahagiaan = 3
                }
            },
            CollectionMissions =
            [
                new RulesetCollectionMissionDto
                {
                    Id = "misi_boneka",
                    Nama = "boneka",
                    SuccessPoints = 0,
                    FailurePoints = -10,
                    PenaltyPoints = 10,
                    KebutuhanTarget =
                    [
                        new RulesetCollectionMissionRequirementDto
                        {
                            Order = 1,
                            Type = "primer",
                            Value = "buku"
                        },
                        new RulesetCollectionMissionRequirementDto
                        {
                            Order = 2,
                            Type = "tersier",
                            Value = "boneka"
                        }
                    ]
                },
                new RulesetCollectionMissionDto
                {
                    Id = "misi_gameboy", Nama = "gameboy", PenaltyPoints = 10,
                    KebutuhanTarget =
                    [
                        new RulesetCollectionMissionRequirementDto { Order = 1, Type = "primer", Value = "buku" },
                        new RulesetCollectionMissionRequirementDto { Order = 2, Type = "tersier", Value = "gameboy" }
                    ]
                },
                new RulesetCollectionMissionDto
                {
                    Id = "misi_hiburan", Nama = "hiburan", PenaltyPoints = 10,
                    KebutuhanTarget =
                    [
                        new RulesetCollectionMissionRequirementDto { Order = 1, Type = "primer", Value = "buku" },
                        new RulesetCollectionMissionRequirementDto { Order = 2, Type = "tersier", Value = "hiburan" }
                    ]
                },
                new RulesetCollectionMissionDto
                {
                    Id = "misi_jam", Nama = "jam", PenaltyPoints = 10,
                    KebutuhanTarget =
                    [
                        new RulesetCollectionMissionRequirementDto { Order = 1, Type = "primer", Value = "buku" },
                        new RulesetCollectionMissionRequirementDto { Order = 2, Type = "tersier", Value = "jam" }
                    ]
                }
            ],
            FinancialGoals =
            {
                new RulesetFinancialGoalDto
                {
                    Id = "beli_rumah",
                    Nama = "beli rumah",
                    HargaBeli = 20,
                    PoinKebahagiaan = 5
                }
            },
            Narratives =
            [
                new RulesetNarrativeDto
                {
                    Id = "jual_pertama",
                    Nama = "jual_pertama",
                    Teks = ["Narasi integrasi typed"],
                    PrerequisiteAksi =
                    [
                        new RulesetNarrativePrerequisiteDto
                        {
                            Aksi = "JualMasakan",
                            Value = 1
                        }
                    ]
                }
            ],
            DonationRankPoints =
            [
                new RulesetDonationRankPointDto { Rank = 1, Points = 7 },
                new RulesetDonationRankPointDto { Rank = 2, Points = 5 },
                new RulesetDonationRankPointDto { Rank = 3, Points = 2 }
            ],
            GoldPointsByQty =
            [
                new RulesetGoldPointDto { Qty = 1, Points = 3 },
                new RulesetGoldPointDto { Qty = 2, Points = 5 },
                new RulesetGoldPointDto { Qty = 3, Points = 8 },
                new RulesetGoldPointDto { Qty = 4, Points = 12 }
            ],
            PensionRankPoints =
            [
                new RulesetPensionRankPointDto { Rank = 1, Points = 5 },
                new RulesetPensionRankPointDto { Rank = 2, Points = 3 },
                new RulesetPensionRankPointDto { Rank = 3, Points = 1 }
            ],
            TieBreakers =
            [
                new RulesetTieBreakerDto { TieBreakerCode = "tie_breaker_1", TieNumber = 1, CardQty = 1 },
                new RulesetTieBreakerDto { TieBreakerCode = "tie_breaker_2", TieNumber = 2, CardQty = 1 },
                new RulesetTieBreakerDto { TieBreakerCode = "tie_breaker_3", TieNumber = 3, CardQty = 1 },
                new RulesetTieBreakerDto { TieBreakerCode = "tie_breaker_4", TieNumber = 4, CardQty = 1 }
            ]
        };
    }
}
