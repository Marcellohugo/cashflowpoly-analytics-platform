using System.Diagnostics.Metrics;

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
