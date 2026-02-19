// Fungsi file: Menguji perilaku dan kontrak komponen pada domain ApiWebApplicationFactory.
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace Cashflowpoly.Api.Tests.Infrastructure;

/// <summary>
/// Menyatakan peran utama tipe ApiWebApplicationFactory pada modul ini.
/// </summary>
internal sealed class ApiWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _connectionString;
    private readonly string _jwtSigningKey;

    /// <summary>
    /// Menjalankan fungsi ApiWebApplicationFactory sebagai bagian dari alur file ini.
    /// </summary>
    public ApiWebApplicationFactory(string connectionString, string jwtSigningKey)
    {
        _connectionString = connectionString;
        _jwtSigningKey = jwtSigningKey;
    }

    /// <summary>
    /// Menjalankan fungsi ConfigureWebHost sebagai bagian dari alur file ini.
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
