// Fungsi file: Memastikan API dan databasenya siap sebelum UI dinyatakan siap.
// Mengimpor namespace `Microsoft.Extensions.Diagnostics.HealthChecks` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama
// lengkapnya.
using Microsoft.Extensions.Diagnostics.HealthChecks;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Infrastructure` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Infrastructure;

public sealed class ApiHealthCheck(IHttpClientFactory clientFactory) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        // Parameter `context` bertipe `HealthCheckContext` membawa konteks operasi yang menyediakan data lingkungan pemrosesan saat ini.
        HealthCheckContext context,
        // Parameter `cancellationToken` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan
        // permintaan atau aplikasi berhenti; bila argumen tidak diberikan digunakan nilai literal `default`.
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var response = await clientFactory.CreateClient("ApiHealth")
                .GetAsync("health/ready", cancellationToken);
            return response.IsSuccessStatusCode
                // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: HealthCheckResult.Healthy() dalam CheckHealthAsync.
                ? HealthCheckResult.Healthy()
                // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: HealthCheckResult.Unhealthy($”API readiness mengembalikan HTTP
                // {(int)response.StatusCode}.”); dalam CheckHealthAsync.
                : HealthCheckResult.Unhealthy($"API readiness mengembalikan HTTP {(int)response.StatusCode}.");
        }
        // Menangani exception `Exception` melalui variabel exception dalam CheckHealthAsync.
        catch (Exception exception)
        {
            return HealthCheckResult.Unhealthy("API readiness tidak dapat dijangkau.", exception);
        }
    }
}
