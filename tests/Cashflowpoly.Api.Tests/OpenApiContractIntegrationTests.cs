// Fungsi file: Memastikan dokumen OpenAPI runtime tetap sesuai dengan kontrak publik API v1.
using System.Net;
using System.Text.Json;
using Cashflowpoly.Api.Tests.Infrastructure;
using Xunit;

namespace Cashflowpoly.Api.Tests;

[Collection("ApiIntegration")]
[Trait("Category", "Contract")]
public sealed class OpenApiContractIntegrationTests
{
    private readonly HttpClient _client;

    public OpenApiContractIntegrationTests(ApiIntegrationTestFixture fixture)
    {
        _client = fixture.Client;
    }

    [Fact]
    public async Task SwaggerJson_ContainsCurrentSetupPaginationAndErrorContracts()
    {
        var response = await _client.GetAsync("/swagger/v1/swagger.json");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        await using var stream = await response.Content.ReadAsStreamAsync();
        using var document = await JsonDocument.ParseAsync(stream);
        var root = document.RootElement;

        Assert.StartsWith("3.0.", root.GetProperty("openapi").GetString());
        var paths = root.GetProperty("paths");
        Assert.True(paths.TryGetProperty("/api/v1/sessions/{sessionId}/setup/validate", out _));
        Assert.True(paths.TryGetProperty("/api/v1/sessions/{sessionId}/setup", out var setupPath));
        Assert.True(setupPath.TryGetProperty("get", out _));
        Assert.True(setupPath.TryGetProperty("post", out _));

        var eventsOperation = paths
            .GetProperty("/api/v1/sessions/{sessionId}/events")
            .GetProperty("get");
        AssertHasQueryParameter(eventsOperation, "cursor");
        AssertHasQueryParameter(eventsOperation, "limit");

        var transactionsOperation = paths
            .GetProperty("/api/v1/analytics/sessions/{sessionId}/transactions")
            .GetProperty("get");
        AssertHasQueryParameter(transactionsOperation, "cursor");
        AssertHasQueryParameter(transactionsOperation, "limit");

        var unauthorizedResponse = await _client.GetAsync("/api/v1/sessions");
        Assert.Equal(HttpStatusCode.Unauthorized, unauthorizedResponse.StatusCode);
        using var errorDocument = JsonDocument.Parse(await unauthorizedResponse.Content.ReadAsStringAsync());
        var errorProperties = errorDocument.RootElement;
        Assert.True(errorProperties.TryGetProperty("error_code", out _));
        Assert.True(errorProperties.TryGetProperty("message", out _));
        Assert.True(errorProperties.TryGetProperty("details", out _));
        Assert.True(errorProperties.TryGetProperty("trace_id", out _));
    }

    private static void AssertHasQueryParameter(JsonElement operation, string parameterName)
    {
        var found = operation.GetProperty("parameters")
            .EnumerateArray()
            .Any(parameter =>
                parameter.GetProperty("in").GetString() == "query" &&
                parameter.GetProperty("name").GetString() == parameterName);

        Assert.True(found, $"Parameter query '{parameterName}' tidak ditemukan di OpenAPI.");
    }
}
