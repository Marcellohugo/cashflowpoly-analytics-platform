// Fungsi file: Menerapkan kontrol keamanan aplikasi melalui RateLimitPolicyHelper.
using System.Security.Claims;

namespace Cashflowpoly.Api.Security;

/// <summary>
/// Menyediakan logika partisi dan batas rate-limit berdasarkan jalur API dan identitas klien.
/// </summary>
internal static class RateLimitPolicyHelper
{
    private const int AuthPermitLimit = 10;
    private const int IngestPermitLimit = 240;
    private const int DefaultPermitLimit = 300;

    /// <summary>
    /// Mengembalikan batas permit per kelompok endpoint agar autentikasi tetap ketat tanpa
    /// memutus halaman analitika yang secara wajar memuat beberapa sumber data sekaligus.
    /// </summary>
    internal static int ResolvePermitLimit(PathString path)
    {
        if (IsAuthPath(path))
        {
            return AuthPermitLimit;
        }

        return IsIngestPath(path) ? IngestPermitLimit : DefaultPermitLimit;
    }

    /// <summary>
    /// Membangun partition key format "scope:client" untuk fixed-window rate limiter.
    /// </summary>
    internal static string BuildPartitionKey(HttpContext context)
    {
        var scope = IsAuthPath(context.Request.Path)
            ? "auth"
            : IsIngestPath(context.Request.Path) ? "ingest" : "default";
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
    /// Memisahkan endpoint login dan registrasi agar percobaan autentikasi tidak berbagi
    /// kuota longgar milik pembacaan analitika.
    /// </summary>
    private static bool IsAuthPath(PathString path)
    {
        return path.StartsWithSegments("/api/v1/auth", StringComparison.OrdinalIgnoreCase);
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
