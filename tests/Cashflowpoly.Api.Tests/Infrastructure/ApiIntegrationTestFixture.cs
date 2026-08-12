// Fungsi file: Memverifikasi perilaku API, database, atau domain melalui ApiIntegrationTestFixture.
using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.PostgreSql;
using Xunit;

namespace Cashflowpoly.Api.Tests.Infrastructure;

/// <summary>
/// Fixture bersama untuk pengujian integrasi API yang mengelola lifecycle
/// container PostgreSQL kosong dan pembuatan HttpClient agar startup API
/// mengeksekusi migrasi aslinya sendiri.
/// </summary>
public sealed class ApiIntegrationTestFixture : IAsyncLifetime
{
    private static readonly string JwtSigningKey = "integration-test-signing-key-with-min-32-char";

    private readonly PostgreSqlContainer _dbContainer;
    private ApiWebApplicationFactory? _factory;
    private HttpClient? _client;
    private string? _previousConnectionString;
    private string? _previousJwtSigningKey;
    private string? _previousJwtSectionSigningKey;

    /// <summary>
    /// Membuat instance fixture dan mengonfigurasi container PostgreSQL
    /// dengan kredensial dan nama database untuk pengujian integrasi.
    /// </summary>
    public ApiIntegrationTestFixture()
    {
        _dbContainer = new PostgreSqlBuilder("postgres:16")
            .WithDatabase("cashflowpoly_it")
            .WithUsername("cashflowpoly")
            .WithPassword("cashflowpoly")
            .Build();
    }

    /// <summary>
    /// Menyediakan HttpClient yang telah terkonfigurasi untuk pengujian integrasi.
    /// Melempar exception jika belum diinisialisasi.
    /// </summary>
    public HttpClient Client => _client ?? throw new InvalidOperationException("HTTP client belum terinisialisasi.");

    /// <summary>
    /// Menjalankan container PostgreSQL kosong, mengatur environment variable,
    /// dan membuat HttpClient melalui WebApplicationFactory.
    /// </summary>
    public async ValueTask InitializeAsync()
    {
        await _dbContainer.StartAsync();

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

    /// <summary>
    /// Membersihkan resource: menutup HttpClient, factory, mengembalikan
    /// environment variable, dan menghentikan container PostgreSQL.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        _client?.Dispose();
        _factory?.Dispose();
        Environment.SetEnvironmentVariable("ConnectionStrings__Default", _previousConnectionString);
        Environment.SetEnvironmentVariable("JWT_SIGNING_KEY", _previousJwtSigningKey);
        Environment.SetEnvironmentVariable("Jwt__SigningKey", _previousJwtSectionSigningKey);
        await _dbContainer.DisposeAsync();
    }
}
