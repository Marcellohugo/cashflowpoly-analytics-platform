// Fungsi file: Menerapkan kontrol keamanan aplikasi melalui RateLimitPolicyHelper.
// Mengimpor namespace `System.Security.Claims` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Security.Claims;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Security` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Security;

/// <summary>
/// Menyediakan logika partisi dan batas rate-limit berdasarkan jalur API dan identitas klien.
/// </summary>
// Mendefinisikan tipe class `RateLimitPolicyHelper`.
internal static class RateLimitPolicyHelper
{
    private const int AuthPermitLimit = 30;
    private const int IngestPermitLimit = 340;
    private const int DefaultPermitLimit = 400;

    /// <summary>
    /// Mengembalikan batas permit per kelompok endpoint agar autentikasi tetap ketat tanpa
    /// memutus halaman analitika yang secara wajar memuat beberapa sumber data sekaligus.
    /// </summary>
    // Mendefinisikan metode `ResolvePermitLimit` dengan hasil bertipe `int`. Mengembalikan batas permit per kelompok endpoint agar autentikasi tetap
    // ketat tanpa memutus halaman analitika yang secara wajar memuat beberapa sumber data sekaligus. Masukan: Parameter `path` bertipe `PathString`
    // membawa nilai path.
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
    // Mendefinisikan metode `BuildPartitionKey` dengan hasil bertipe `string`. Membangun partition key format ”scope:client” untuk fixed-window rate
    // limiter. Masukan: Parameter `context` bertipe `HttpContext` membawa konteks operasi yang menyediakan data lingkungan pemrosesan saat ini.
    internal static string BuildPartitionKey(HttpContext context)
    {
        var scope = IsAuthPath(context.Request.Path)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: ”auth” dalam BuildPartitionKey.
            ? "auth"
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: IsIngestPath(context.Request.Path) ? ”ingest” : ”default”; dalam
            // BuildPartitionKey.
            : IsIngestPath(context.Request.Path) ? "ingest" : "default";
        var client = ResolveClientKey(context);
        return $"{scope}:{client}";
    }

    /// <summary>
    /// Memeriksa apakah path merupakan endpoint ingest event (/api/v1/events).
    /// </summary>
    // Mendefinisikan metode `IsIngestPath` dengan hasil bertipe `bool`. Memeriksa apakah path merupakan endpoint ingest event (/api/v1/events).
    // Masukan: Parameter `path` bertipe `PathString` membawa nilai path.
    private static bool IsIngestPath(PathString path)
    {
        return path.StartsWithSegments("/api/v1/events", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Memisahkan endpoint login dan registrasi agar percobaan autentikasi tidak berbagi
    /// kuota longgar milik pembacaan analitika.
    /// </summary>
    // Mendefinisikan metode `IsAuthPath` dengan hasil bertipe `bool`. Memisahkan endpoint login dan registrasi agar percobaan autentikasi tidak berbagi
    // kuota longgar milik pembacaan analitika. Masukan: Parameter `path` bertipe `PathString` membawa nilai path.
    private static bool IsAuthPath(PathString path)
    {
        return path.StartsWithSegments("/api/v1/auth", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Mengambil identitas klien: user ID dari claim JWT jika ada, atau alamat IP remote.
    /// </summary>
    // Mendefinisikan metode `ResolveClientKey` dengan hasil bertipe `string`. Mengambil identitas klien: user ID dari claim JWT jika ada, atau alamat
    // IP remote. Masukan: Parameter `context` bertipe `HttpContext` membawa konteks operasi yang menyediakan data lingkungan pemrosesan saat ini.
    private static string ResolveClientKey(HttpContext context)
    {
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: context.User.FindFirstValue(”sub”); dalam ResolveClientKey.
            ?? context.User.FindFirstValue("sub");

        if (!string.IsNullOrWhiteSpace(userId))
        {
            return $"user:{userId}";
        }

        var remoteIp = context.Connection.RemoteIpAddress?.ToString();
        return $"ip:{(string.IsNullOrWhiteSpace(remoteIp) ? "unknown" : remoteIp)}";
    }
}
