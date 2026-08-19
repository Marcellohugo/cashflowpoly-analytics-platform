// Fungsi file: Memastikan API dan databasenya siap sebelum UI dinyatakan siap.
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Cashflowpoly.Ui.Infrastructure;

public sealed class ApiHealthCheck(IHttpClientFactory clientFactory) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var response = await clientFactory.CreateClient("ApiHealth")
                .GetAsync("health/ready", cancellationToken);
            return response.IsSuccessStatusCode
                ? HealthCheckResult.Healthy()
                : HealthCheckResult.Unhealthy($"API readiness mengembalikan HTTP {(int)response.StatusCode}.");
        }
        catch (Exception exception)
        {
            return HealthCheckResult.Unhealthy("API readiness tidak dapat dijangkau.", exception);
        }
    }
}
