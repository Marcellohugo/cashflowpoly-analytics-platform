using Microsoft.AspNetCore.Mvc.Testing;
using Npgsql;
using Testcontainers.PostgreSql;
using Xunit;

namespace Cashflowpoly.Api.Tests.Infrastructure;

public sealed class ApiIntegrationTestFixture : IAsyncLifetime
{
    private static readonly string JwtSigningKey = "integration-test-signing-key-with-min-32-char";

    private readonly PostgreSqlContainer _dbContainer;
    private ApiWebApplicationFactory? _factory;
    private HttpClient? _client;
    private string? _previousConnectionString;
    private string? _previousJwtSigningKey;
    private string? _previousJwtSectionSigningKey;

    public ApiIntegrationTestFixture()
    {
        _dbContainer = new PostgreSqlBuilder("postgres:16")
            .WithDatabase("cashflowpoly_it")
            .WithUsername("cashflowpoly")
            .WithPassword("cashflowpoly")
            .Build();
    }

    public HttpClient Client => _client ?? throw new InvalidOperationException("HTTP client belum terinisialisasi.");

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        await ApplySchemaAsync(_dbContainer.GetConnectionString());

        _previousConnectionString = Environment.GetEnvironmentVariable("ConnectionStrings__Default");
        _previousJwtSigningKey = Environment.GetEnvironmentVariable("JWT_SIGNING_KEY");
        _previousJwtSectionSigningKey = Environment.GetEnvironmentVariable("Jwt__SigningKey");
        Environment.SetEnvironmentVariable("ConnectionStrings__Default", _dbContainer.GetConnectionString());
        Environment.SetEnvironmentVariable("JWT_SIGNING_KEY", JwtSigningKey);
        Environment.SetEnvironmentVariable("Jwt__SigningKey", JwtSigningKey);

        _factory = new ApiWebApplicationFactory(_dbContainer.GetConnectionString(), JwtSigningKey);
        _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    public async Task DisposeAsync()
    {
        _client?.Dispose();
        _factory?.Dispose();
        Environment.SetEnvironmentVariable("ConnectionStrings__Default", _previousConnectionString);
        Environment.SetEnvironmentVariable("JWT_SIGNING_KEY", _previousJwtSigningKey);
        Environment.SetEnvironmentVariable("Jwt__SigningKey", _previousJwtSectionSigningKey);
        await _dbContainer.DisposeAsync();
    }

    private static async Task ApplySchemaAsync(string connectionString)
    {
        var schemaPath = Path.Combine(AppContext.BaseDirectory, "database", "00_create_schema.sql");
        if (!File.Exists(schemaPath))
        {
            throw new FileNotFoundException($"Schema SQL tidak ditemukan pada path '{schemaPath}'.");
        }

        var schemaSql = await File.ReadAllTextAsync(schemaPath);

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(schemaSql, connection);
        await command.ExecuteNonQueryAsync();
    }
}
