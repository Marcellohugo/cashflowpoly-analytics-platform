// Fungsi file: Menyediakan komponen keamanan aplikasi untuk domain RateLimitPolicyHelper (JWT, audit, atau rate limiting).
using System.Security.Claims;

namespace Cashflowpoly.Api.Security;

/// <summary>
/// Menyatakan peran utama tipe RateLimitPolicyHelper pada modul ini.
/// </summary>
internal static class RateLimitPolicyHelper
{
    private const int IngestPermitLimit = 120;
    private const int DefaultPermitLimit = 60;

    /// <summary>
    /// Menjalankan fungsi ResolvePermitLimit sebagai bagian dari alur file ini.
    /// </summary>
    internal static int ResolvePermitLimit(PathString path)
    {
        return IsIngestPath(path) ? IngestPermitLimit : DefaultPermitLimit;
    }

    /// <summary>
    /// Menjalankan fungsi BuildPartitionKey sebagai bagian dari alur file ini.
    /// </summary>
    internal static string BuildPartitionKey(HttpContext context)
    {
        var scope = IsIngestPath(context.Request.Path) ? "ingest" : "default";
        var client = ResolveClientKey(context);
        return $"{scope}:{client}";
    }

    /// <summary>
    /// Menjalankan fungsi IsIngestPath sebagai bagian dari alur file ini.
    /// </summary>
    private static bool IsIngestPath(PathString path)
    {
        return path.StartsWithSegments("/api/v1/events", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Menjalankan fungsi ResolveClientKey sebagai bagian dari alur file ini.
    /// </summary>
    private static string ResolveClientKey(HttpContext context)
    {
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? context.User.FindFirstValue("sub");

        if (!string.IsNullOrWhiteSpace(userId))
        {
            return $"user:{userId}";
        }

        var remoteIp = context.Connection.RemoteIpAddress?.ToString();
        return $"ip:{(string.IsNullOrWhiteSpace(remoteIp) ? "unknown" : remoteIp)}";
    }
}
