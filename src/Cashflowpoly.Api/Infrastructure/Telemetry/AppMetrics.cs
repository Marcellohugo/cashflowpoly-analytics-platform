// Fungsi file: Menyediakan dukungan infrastruktur API melalui AppMetrics.
// Mengimpor namespace `System.Diagnostics.Metrics` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Diagnostics.Metrics;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Infrastructure.Telemetry` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Infrastructure.Telemetry;

public static class AppMetrics
{
    public const string MeterName = "Cashflowpoly.Api";

    private static readonly Meter Meter = new(MeterName, "1.0");

    public static readonly Counter<long> RequestsTotal =
        Meter.CreateCounter<long>("http_requests_total", "requests", "Total HTTP requests processed");

    public static readonly Histogram<double> RequestDurationMs =
        Meter.CreateHistogram<double>("http_request_duration_ms", "ms", "HTTP request duration in milliseconds");

    public static readonly Counter<long> RequestErrorsTotal =
        Meter.CreateCounter<long>("http_request_errors_total", "requests", "Total HTTP requests that returned 4xx or 5xx");
}
