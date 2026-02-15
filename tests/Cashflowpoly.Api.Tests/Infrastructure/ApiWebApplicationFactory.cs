using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace Cashflowpoly.Api.Tests.Infrastructure;

internal sealed class ApiWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _connectionString;
    private readonly string _jwtSigningKey;

    public ApiWebApplicationFactory(string connectionString, string jwtSigningKey)
    {
        _connectionString = connectionString;
        _jwtSigningKey = jwtSigningKey;
    }

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
                ["AuthBootstrap:SeedDefaultUsers"] = "false",
                ["Auth:AllowPublicInstructorRegistration"] = "true"
            });
        });
    }
}
