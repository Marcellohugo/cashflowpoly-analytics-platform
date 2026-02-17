using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Cashflowpoly.Api.Models;
using Cashflowpoly.Api.Tests.Infrastructure;
using Xunit;

namespace Cashflowpoly.Api.Tests;

[Collection("ApiIntegration")]
[Trait("Category", "Integration")]
public sealed class ObservabilitySecurityIntegrationTests
{
    private readonly HttpClient _client;

    public ObservabilitySecurityIntegrationTests(ApiIntegrationTestFixture fixture)
    {
        _client = fixture.Client;
    }

    [Fact]
    public async Task Observability_And_SecurityAudit_RespectRoleAccess()
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];
        var instructorUsername = $"it_obs_instr_{suffix}";
        var playerUsername = $"it_obs_player_{suffix}";
        const string instructorPassword = "IntegrationInstructorPass!123";
        const string playerPassword = "IntegrationPlayerPass!123";

        await RegisterAsync(instructorUsername, instructorPassword, "INSTRUCTOR");
        await RegisterAsync(playerUsername, playerPassword, "PLAYER");

        var instructorLogin = await LoginAsync(instructorUsername, instructorPassword);
        var playerLogin = await LoginAsync(playerUsername, playerPassword);

        var instructorObservability = await SendAsync(
            HttpMethod.Get,
            "/api/v1/observability/metrics?top=5",
            instructorLogin.AccessToken);
        Assert.Equal(HttpStatusCode.OK, instructorObservability.StatusCode);
        await AssertJsonHasPropertyAsync(instructorObservability, "totalRequests");

        var playerObservability = await SendAsync(
            HttpMethod.Get,
            "/api/v1/observability/metrics?top=5",
            playerLogin.AccessToken);
        Assert.Equal(HttpStatusCode.Forbidden, playerObservability.StatusCode);

        var instructorAudit = await SendAsync(
            HttpMethod.Get,
            "/api/v1/security/audit-logs?limit=10",
            instructorLogin.AccessToken);
        Assert.Equal(HttpStatusCode.OK, instructorAudit.StatusCode);
        await AssertJsonHasPropertyAsync(instructorAudit, "items");

        var playerAudit = await SendAsync(
            HttpMethod.Get,
            "/api/v1/security/audit-logs?limit=10",
            playerLogin.AccessToken);
        Assert.Equal(HttpStatusCode.Forbidden, playerAudit.StatusCode);
    }

    private async Task<RegisterResponse> RegisterAsync(string username, string password, string role)
    {
        var payload = new RegisterRequest(username, password, role, null);
        var response = await _client.PostAsJsonAsync("/api/v1/auth/register", payload);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<RegisterResponse>();
        Assert.NotNull(body);
        return body;
    }

    private async Task<LoginResponse> LoginAsync(string username, string password)
    {
        var payload = new LoginRequest(username, password);
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", payload);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.NotNull(body);
        return body;
    }

    private async Task<HttpResponseMessage> SendAsync(HttpMethod method, string path, string accessToken)
    {
        var request = new HttpRequestMessage(method, path);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        return await _client.SendAsync(request);
    }

    private static async Task AssertJsonHasPropertyAsync(HttpResponseMessage response, string propertyName)
    {
        var raw = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(raw);
        var hasProperty = document.RootElement.TryGetProperty(propertyName, out _);
        Assert.True(hasProperty, $"Respons tidak memiliki properti '{propertyName}'. Body: {raw}");
    }
}
