// Fungsi file: Menyediakan WebApplicationFactory kustom untuk pengujian integrasi API dengan konfigurasi database dan JWT in-memory.
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace Cashflowpoly.Api.Tests.Infrastructure;

/// <summary>
/// Factory kustom yang menginisialisasi host API untuk pengujian integrasi
/// dengan mengganti connection string dan signing key JWT melalui in-memory configuration.
/// </summary>
internal sealed class ApiWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _connectionString;
    private readonly string _jwtSigningKey;

    /// <summary>
    /// Membuat instance factory dengan connection string database dan signing key JWT
    /// yang akan diinjeksikan ke konfigurasi aplikasi saat pengujian.
    /// </summary>
    public ApiWebApplicationFactory(string connectionString, string jwtSigningKey)
    {
        _connectionString = connectionString;
        _jwtSigningKey = jwtSigningKey;
    }

    /// <summary>
    /// Mengonfigurasi web host dengan environment Development dan menyuntikkan
    /// connection string serta signing key JWT melalui in-memory configuration.
    /// </summary>
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureAppConfiguration((_, configBuilder) =>
        {
            configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Default"] = _connectionString,
                ["Jwt:SigningKey"] = _jwtSigningKey,
                ["JWT_SIGNING_KEY"] = _jwtSigningKey,
                ["AuthBootstrap:SeedDefaultUsers"] = "false"
            });
        });
    }
}
