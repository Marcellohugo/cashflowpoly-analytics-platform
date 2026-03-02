// Fungsi file: Helper kebijakan rate limiting — menentukan limit dan partition key berdasarkan path dan identitas pengguna.
using System.Security.Claims;

namespace Cashflowpoly.Api.Security;

/// <summary>
/// Menyediakan logika partisi dan batas rate-limit berdasarkan jalur API dan identitas klien.
/// </summary>
internal static class RateLimitPolicyHelper
{
    private const int IngestPermitLimit = 120;
    private const int DefaultPermitLimit = 60;

    /// <summary>
    /// Mengembalikan batas permit: 120 untuk path ingest event, 60 untuk path lainnya.
    /// </summary>
    internal static int ResolvePermitLimit(PathString path)
    {
        return IsIngestPath(path) ? IngestPermitLimit : DefaultPermitLimit;
    }

    /// <summary>
    /// Membangun partition key format "scope:client" untuk sliding-window rate limiter.
    /// </summary>
    internal static string BuildPartitionKey(HttpContext context)
    {
        var scope = IsIngestPath(context.Request.Path) ? "ingest" : "default";
        var client = ResolveClientKey(context);
        return $"{scope}:{client}";
    }

    /// <summary>
    /// Memeriksa apakah path merupakan endpoint ingest event (/api/v1/events).
    /// </summary>
    private static bool IsIngestPath(PathString path)
    {
        return path.StartsWithSegments("/api/v1/events", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Mengambil identitas klien: user ID dari claim JWT jika ada, atau alamat IP remote.
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
