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
// Membuka scope tipe RateLimitPolicyHelper; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendeklarasikan field bertipe `int`: `AuthPermitLimit` menyimpan nilai auth permit limit dengan nilai awal nilai literal `10`.
    private const int AuthPermitLimit = 10;
    // Mendeklarasikan field bertipe `int`: `IngestPermitLimit` menyimpan nilai ingest permit limit dengan nilai awal nilai literal `240`.
    private const int IngestPermitLimit = 240;
    // Mendeklarasikan field bertipe `int`: `DefaultPermitLimit` menyimpan nilai bawaan permit limit dengan nilai awal nilai literal `300`.
    private const int DefaultPermitLimit = 300;

    /// <summary>
    /// Mengembalikan batas permit per kelompok endpoint agar autentikasi tetap ketat tanpa
    /// memutus halaman analitika yang secara wajar memuat beberapa sumber data sekaligus.
    /// </summary>
    // Mendefinisikan metode `ResolvePermitLimit` dengan hasil bertipe `int`. Mengembalikan batas permit per kelompok endpoint agar autentikasi tetap
    // ketat tanpa memutus halaman analitika yang secara wajar memuat beberapa sumber data sekaligus. Masukan: Parameter `path` bertipe `PathString`
    // membawa nilai path.
    internal static int ResolvePermitLimit(PathString path)
    // Membuka scope metode ResolvePermitLimit; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolvePermitLimit.
    {
        // Memeriksa memanggil `IsAuthPath` dengan `path`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ResolvePermitLimit.
        if (IsAuthPath(path))
        // Membuka scope cabang if untuk kondisi `IsAuthPath(path)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolvePermitLimit.
        {
            // Mengembalikan `AuthPermitLimit` (nilai auth permit limit) kepada pemanggil dalam ResolvePermitLimit; eksekusi jalur ini selesai setelah nilai
            // hasil ditentukan.
            return AuthPermitLimit;
        // Menutup scope cabang if untuk kondisi `IsAuthPath(path)`; bagian berikut berada di luar batas blok tersebut dalam ResolvePermitLimit.
        }

        // Mengembalikan hasil pemilihan bersyarat: ketika `IsIngestPath(path)` benar gunakan `IngestPermitLimit`, jika tidak gunakan `DefaultPermitLimit`
        // kepada pemanggil dalam ResolvePermitLimit; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return IsIngestPath(path) ? IngestPermitLimit : DefaultPermitLimit;
    // Menutup scope metode ResolvePermitLimit; bagian berikut berada di luar batas blok tersebut dalam ResolvePermitLimit.
    }

    /// <summary>
    /// Membangun partition key format "scope:client" untuk fixed-window rate limiter.
    /// </summary>
    // Mendefinisikan metode `BuildPartitionKey` dengan hasil bertipe `string`. Membangun partition key format ”scope:client” untuk fixed-window rate
    // limiter. Masukan: Parameter `context` bertipe `HttpContext` membawa konteks operasi yang menyediakan data lingkungan pemrosesan saat ini.
    internal static string BuildPartitionKey(HttpContext context)
    // Membuka scope metode BuildPartitionKey; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildPartitionKey.
    {
        // Menyiapkan variabel lokal `scope` untuk nilai cakupan dengan hasil pemilihan bersyarat: ketika `IsAuthPath(context.Request.Path)` benar gunakan
        // `”auth”`, jika tidak gunakan `IsIngestPath(context.Request.Path) ? ”ingest” : ”default”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var scope = IsAuthPath(context.Request.Path)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: ”auth” dalam BuildPartitionKey.
            ? "auth"
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: IsIngestPath(context.Request.Path) ? ”ingest” : ”default”; dalam
            // BuildPartitionKey.
            : IsIngestPath(context.Request.Path) ? "ingest" : "default";
        // Menyiapkan variabel lokal `client` untuk nilai client dengan memanggil `ResolveClientKey` dengan `context`. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var client = ResolveClientKey(context);
        // Mengembalikan teks interpolasi `$”{scope}:{client}”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan kepada pemanggil
        // dalam BuildPartitionKey; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return $"{scope}:{client}";
    // Menutup scope metode BuildPartitionKey; bagian berikut berada di luar batas blok tersebut dalam BuildPartitionKey.
    }

    /// <summary>
    /// Memeriksa apakah path merupakan endpoint ingest event (/api/v1/events).
    /// </summary>
    // Mendefinisikan metode `IsIngestPath` dengan hasil bertipe `bool`. Memeriksa apakah path merupakan endpoint ingest event (/api/v1/events).
    // Masukan: Parameter `path` bertipe `PathString` membawa nilai path.
    private static bool IsIngestPath(PathString path)
    // Membuka scope metode IsIngestPath; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam IsIngestPath.
    {
        // Mengembalikan memanggil `path.StartsWithSegments` dengan `”/api/v1/events”`, `StringComparison.OrdinalIgnoreCase` kepada pemanggil dalam
        // IsIngestPath; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return path.StartsWithSegments("/api/v1/events", StringComparison.OrdinalIgnoreCase);
    // Menutup scope metode IsIngestPath; bagian berikut berada di luar batas blok tersebut dalam IsIngestPath.
    }

    /// <summary>
    /// Memisahkan endpoint login dan registrasi agar percobaan autentikasi tidak berbagi
    /// kuota longgar milik pembacaan analitika.
    /// </summary>
    // Mendefinisikan metode `IsAuthPath` dengan hasil bertipe `bool`. Memisahkan endpoint login dan registrasi agar percobaan autentikasi tidak berbagi
    // kuota longgar milik pembacaan analitika. Masukan: Parameter `path` bertipe `PathString` membawa nilai path.
    private static bool IsAuthPath(PathString path)
    // Membuka scope metode IsAuthPath; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam IsAuthPath.
    {
        // Mengembalikan memanggil `path.StartsWithSegments` dengan `”/api/v1/auth”`, `StringComparison.OrdinalIgnoreCase` kepada pemanggil dalam
        // IsAuthPath; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return path.StartsWithSegments("/api/v1/auth", StringComparison.OrdinalIgnoreCase);
    // Menutup scope metode IsAuthPath; bagian berikut berada di luar batas blok tersebut dalam IsAuthPath.
    }

    /// <summary>
    /// Mengambil identitas klien: user ID dari claim JWT jika ada, atau alamat IP remote.
    /// </summary>
    // Mendefinisikan metode `ResolveClientKey` dengan hasil bertipe `string`. Mengambil identitas klien: user ID dari claim JWT jika ada, atau alamat
    // IP remote. Masukan: Parameter `context` bertipe `HttpContext` membawa konteks operasi yang menyediakan data lingkungan pemrosesan saat ini.
    private static string ResolveClientKey(HttpContext context)
    // Membuka scope metode ResolveClientKey; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveClientKey.
    {
        // Menyiapkan variabel lokal `userId` untuk identitas akun pengguna yang datanya sedang diproses dengan
        // `context.User.FindFirstValue(ClaimTypes.NameIdentifier)` bila tidak null; jika null gunakan `context.User.FindFirstValue(”sub”)` sebagai nilai
        // pengganti. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: context.User.FindFirstValue(”sub”); dalam ResolveClientKey.
            ?? context.User.FindFirstValue("sub");

        // Memeriksa kebalikan kondisi `string.IsNullOrWhiteSpace(userId)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // ResolveClientKey.
        if (!string.IsNullOrWhiteSpace(userId))
        // Membuka scope cabang if untuk kondisi `!string.IsNullOrWhiteSpace(userId)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ResolveClientKey.
        {
            // Mengembalikan teks interpolasi `$”user:{userId}”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan kepada pemanggil dalam
            // ResolveClientKey; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return $"user:{userId}";
        // Menutup scope cabang if untuk kondisi `!string.IsNullOrWhiteSpace(userId)`; bagian berikut berada di luar batas blok tersebut dalam
        // ResolveClientKey.
        }

        // Menyiapkan variabel lokal `remoteIp` untuk nilai remote ip dengan `context.Connection.RemoteIpAddress?.ToString()`; akses setelah ?. hanya
        // dilakukan bila penerimanya tidak null. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var remoteIp = context.Connection.RemoteIpAddress?.ToString();
        // Mengembalikan teks interpolasi `$”ip:{(string.IsNullOrWhiteSpace(remoteIp) ? ”unknown” : remoteIp)}”`; nilai ekspresi di dalam kurung kurawal
        // disisipkan saat program berjalan kepada pemanggil dalam ResolveClientKey; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return $"ip:{(string.IsNullOrWhiteSpace(remoteIp) ? "unknown" : remoteIp)}";
    // Menutup scope metode ResolveClientKey; bagian berikut berada di luar batas blok tersebut dalam ResolveClientKey.
    }
// Menutup scope tipe RateLimitPolicyHelper; bagian berikut berada di luar batas blok tersebut.
}
