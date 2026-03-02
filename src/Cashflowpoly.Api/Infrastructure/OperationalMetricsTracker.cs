// Fungsi file: Mengumpulkan dan menyimpan metrik operasional request API secara in-memory per endpoint, termasuk jumlah request, error rate, dan durasi rata-rata/P95.
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Routing;

namespace Cashflowpoly.Api.Infrastructure;

/// <summary>
/// Kolektor metrik operasional request API (in-memory) untuk dashboard observability.
/// </summary>
public sealed class OperationalMetricsTracker
{
    private readonly ConcurrentDictionary<string, EndpointMetricsAccumulator> _endpoints =
        new(StringComparer.Ordinal);

    private long _totalRequests;
    private long _totalErrors;

    public void Record(HttpContext context, double durationMs)
    {
        var method = context.Request.Method.ToUpperInvariant();
        var routePattern = ResolveRoutePattern(context);
        var key = $"{method} {routePattern}";
        var statusCode = context.Response.StatusCode;
        var isError = statusCode >= StatusCodes.Status400BadRequest;

        var endpoint = _endpoints.GetOrAdd(key, _ => new EndpointMetricsAccumulator(method, routePattern));
        endpoint.Record(durationMs, statusCode, isError);

        Interlocked.Increment(ref _totalRequests);
        if (isError)
        {
            Interlocked.Increment(ref _totalErrors);
        }
    }

    /// <summary>
    /// Menghasilkan snapshot metrik operasional saat ini dengan daftar endpoint teratas.
    /// </summary>
    /// <param name="maxEndpoints">Jumlah maksimum endpoint yang ditampilkan dalam snapshot.</param>
    /// <returns>Snapshot metrik operasional berisi total request, error rate, dan detail per endpoint.</returns>
    public OperationalMetricsSnapshot Snapshot(int maxEndpoints)
    {
        var endpoints = _endpoints.Values
            .Select(endpoint => endpoint.Snapshot())
            .OrderByDescending(endpoint => endpoint.RequestCount)
            .Take(Math.Max(1, maxEndpoints))
            .ToList();

        var totalRequests = Interlocked.Read(ref _totalRequests);
        var totalErrors = Interlocked.Read(ref _totalErrors);
        var errorRate = totalRequests == 0 ? 0 : (totalErrors * 100d) / totalRequests;

        return new OperationalMetricsSnapshot(
            DateTimeOffset.UtcNow,
            totalRequests,
            totalErrors,
            Math.Round(errorRate, 2),
            endpoints);
    }

    /// <summary>
    /// Menentukan route pattern endpoint dari HttpContext, fallback ke path mentah jika tidak ada.
    /// </summary>
    private static string ResolveRoutePattern(HttpContext context)
    {
        if (context.GetEndpoint() is RouteEndpoint routeEndpoint &&
            !string.IsNullOrWhiteSpace(routeEndpoint.RoutePattern.RawText))
        {
            return routeEndpoint.RoutePattern.RawText;
        }

        if (context.Request.Path.HasValue)
        {
            return context.Request.Path.Value!;
        }

        return "/";
    }

    /// <summary>
    /// Akumulator thread-safe yang merekam metrik per endpoint: jumlah request, error, durasi, dan sampel.
    /// </summary>
    private sealed class EndpointMetricsAccumulator
    {
        private const int DurationSamplesLimit = 4096;

        /// <summary>
        /// Objek pengunci untuk sinkronisasi akses ke data metrik.
        /// </summary>
        private readonly object _gate = new();
        /// <summary>
        /// Antrian sampel durasi terbatas untuk perhitungan persentil.
        /// </summary>
        private readonly Queue<double> _durationSamples = new();

        private readonly string _method;
        private readonly string _routePattern;
        private long _requestCount;
        private long _errorCount;
        private double _durationSumMs;
        private int _lastStatusCode;
        private DateTimeOffset _lastSeenAt;

        /// <summary>
        /// Membuat akumulator metrik baru untuk satu endpoint tertentu.
        /// </summary>
        /// <param name="method">HTTP method (GET, POST, dll.).</param>
        /// <param name="routePattern">Pola route endpoint.</param>
        public EndpointMetricsAccumulator(string method, string routePattern)
        {
            _method = method;
            _routePattern = routePattern;
        }

        public void Record(double durationMs, int statusCode, bool isError)
        {
            lock (_gate)
            {
                _requestCount++;
                if (isError)
                {
                    _errorCount++;
                }

                _durationSumMs += durationMs;
                _lastStatusCode = statusCode;
                _lastSeenAt = DateTimeOffset.UtcNow;

                _durationSamples.Enqueue(durationMs);
                if (_durationSamples.Count > DurationSamplesLimit)
                {
                    _durationSamples.Dequeue();
                }
            }
        }

        /// <summary>
        /// Menghasilkan snapshot metrik saat ini untuk endpoint ini.
        /// </summary>
        /// <returns>Metrik endpoint mencakup jumlah request, error rate, durasi rata-rata, dan P95.</returns>
        public EndpointOperationalMetric Snapshot()
        {
            lock (_gate)
            {
                var requestCount = _requestCount;
                var errorCount = _errorCount;
                var errorRate = requestCount == 0 ? 0 : (errorCount * 100d) / requestCount;
                var averageDurationMs = requestCount == 0 ? 0 : _durationSumMs / requestCount;
                var p95DurationMs = ComputePercentile(_durationSamples, 95);

                return new EndpointOperationalMetric(
                    _method,
                    _routePattern,
                    requestCount,
                    errorCount,
                    Math.Round(errorRate, 2),
                    Math.Round(averageDurationMs, 2),
                    p95DurationMs,
                    _lastStatusCode,
                    _lastSeenAt);
            }
        }

        /// <summary>
        /// Menghitung nilai persentil dari kumpulan sampel durasi.
        /// </summary>
        /// <param name="values">Kumpulan sampel durasi.</param>
        /// <param name="percentile">Persentil yang diinginkan (1-100).</param>
        /// <returns>Nilai persentil yang dihitung, 0 jika tidak ada data.</returns>
        private static double ComputePercentile(IEnumerable<double> values, int percentile)
        {
            var ordered = values.OrderBy(value => value).ToArray();
            if (ordered.Length == 0)
            {
                return 0;
            }

            var clampedPercentile = Math.Clamp(percentile, 1, 100);
            var index = (int)Math.Ceiling((clampedPercentile / 100d) * ordered.Length) - 1;
            index = Math.Clamp(index, 0, ordered.Length - 1);
            return Math.Round(ordered[index], 2);
        }
    }
}

/// <summary>
/// Record snapshot metrik operasional secara keseluruhan pada titik waktu tertentu.
/// </summary>
public sealed record OperationalMetricsSnapshot(
    DateTimeOffset GeneratedAt,
    long TotalRequests,
    long TotalErrors,
    double ErrorRatePercent,
    List<EndpointOperationalMetric> Endpoints);

/// <summary>
/// Record metrik operasional untuk satu endpoint: request count, error rate, durasi, dan status terakhir.
/// </summary>
public sealed record EndpointOperationalMetric(
    string Method,
    string RoutePattern,
    long RequestCount,
    long ErrorCount,
    double ErrorRatePercent,
    double AverageDurationMs,
    double P95DurationMs,
    int LastStatusCode,
    DateTimeOffset LastSeenAt);
