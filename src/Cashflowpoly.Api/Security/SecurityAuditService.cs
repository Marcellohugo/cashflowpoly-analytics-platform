// Fungsi file: Menerapkan kontrol keamanan aplikasi melalui SecurityAuditService.
// Mengimpor namespace `System.Diagnostics` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Diagnostics;
// Mengimpor namespace `System.Security.Claims` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Security.Claims;
// Mengimpor namespace `System.Text.Json` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.Json;
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Security` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Security;

/// <summary>
/// Konstanta string tipe event untuk audit keamanan (login, register, challenge, dsb.).
/// </summary>
// Mendefinisikan tipe class `SecurityAuditEventTypes`.
public static class SecurityAuditEventTypes
// Membuka scope tipe SecurityAuditEventTypes; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendeklarasikan field bertipe `string`: `LoginSuccess` menyimpan nilai login success dengan nilai awal nilai literal `”LOGIN_SUCCESS”`.
    public const string LoginSuccess = "LOGIN_SUCCESS";
    // Mendeklarasikan field bertipe `string`: `LoginFailed` menyimpan nilai login failed dengan nilai awal nilai literal `”LOGIN_FAILED”`.
    public const string LoginFailed = "LOGIN_FAILED";
    // Mendeklarasikan field bertipe `string`: `RegisterSuccess` menyimpan nilai register success dengan nilai awal nilai literal `”REGISTER_SUCCESS”`.
    public const string RegisterSuccess = "REGISTER_SUCCESS";
    // Mendeklarasikan field bertipe `string`: `RegisterDenied` menyimpan nilai register denied dengan nilai awal nilai literal `”REGISTER_DENIED”`.
    public const string RegisterDenied = "REGISTER_DENIED";
    // Mendeklarasikan field bertipe `string`: `AuthChallenge` menyimpan nilai auth challenge dengan nilai awal nilai literal `”AUTH_CHALLENGE”`.
    public const string AuthChallenge = "AUTH_CHALLENGE";
    // Mendeklarasikan field bertipe `string`: `AuthForbidden` menyimpan nilai auth forbidden dengan nilai awal nilai literal `”AUTH_FORBIDDEN”`.
    public const string AuthForbidden = "AUTH_FORBIDDEN";
    // Mendeklarasikan field bertipe `string`: `AuthFailed` menyimpan nilai auth failed dengan nilai awal nilai literal `”AUTH_FAILED”`.
    public const string AuthFailed = "AUTH_FAILED";
    // Mendeklarasikan field bertipe `string`: `RateLimited` menyimpan nilai rate limited dengan nilai awal nilai literal `”RATE_LIMITED”`.
    public const string RateLimited = "RATE_LIMITED";
    // Mendeklarasikan field bertipe `string`: `SetupSaved` menyimpan nilai setup saved dengan nilai awal nilai literal `”SETUP_SAVED”`.
    public const string SetupSaved = "SETUP_SAVED";
    // Mendeklarasikan field bertipe `string`: `SessionStarted` menyimpan nilai sesi started dengan nilai awal nilai literal `”SESSION_STARTED”`.
    public const string SessionStarted = "SESSION_STARTED";
    // Mendeklarasikan field bertipe `string`: `SessionEnded` menyimpan nilai sesi ended dengan nilai awal nilai literal `”SESSION_ENDED”`.
    public const string SessionEnded = "SESSION_ENDED";
    // Mendeklarasikan field bertipe `string`: `RulesetChanged` menyimpan nilai aturan changed dengan nilai awal nilai literal `”RULESET_CHANGED”`.
    public const string RulesetChanged = "RULESET_CHANGED";
    // Mendeklarasikan field bertipe `string`: `DemoActivity` menyimpan nilai demo activity dengan nilai awal nilai literal `”DEMO_ACTIVITY”`.
    public const string DemoActivity = "DEMO_ACTIVITY";
// Menutup scope tipe SecurityAuditEventTypes; bagian berikut berada di luar batas blok tersebut.
}

/// <summary>
/// Konstanta string outcome audit keamanan: SUCCESS, FAILURE, DENIED.
/// </summary>
// Mendefinisikan tipe class `SecurityAuditOutcomes`.
public static class SecurityAuditOutcomes
// Membuka scope tipe SecurityAuditOutcomes; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendeklarasikan field bertipe `string`: `Success` menyimpan nilai success dengan nilai awal nilai literal `”SUCCESS”`.
    public const string Success = "SUCCESS";
    // Mendeklarasikan field bertipe `string`: `Failure` menyimpan nilai failure dengan nilai awal nilai literal `”FAILURE”`.
    public const string Failure = "FAILURE";
    // Mendeklarasikan field bertipe `string`: `Denied` menyimpan nilai denied dengan nilai awal nilai literal `”DENIED”`.
    public const string Denied = "DENIED";
// Menutup scope tipe SecurityAuditOutcomes; bagian berikut berada di luar batas blok tersebut.
}

/// <summary>
/// Layanan untuk menyimpan audit event keamanan tanpa mengganggu jalur request utama.
/// </summary>
// Mendefinisikan tipe class `SecurityAuditService`; sealed mencegah tipe ini diturunkan lagi.
public sealed class SecurityAuditService
// Membuka scope tipe SecurityAuditService; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    /// <summary>
    /// Opsi serialisasi JSON web-default untuk mengonversi detail audit menjadi string JSON.
    /// </summary>
    // Mendeklarasikan field bertipe `JsonSerializerOptions`: `JsonOptions` menyimpan nilai JSON options dengan nilai awal objek baru dengan tipe
    // mengikuti konteks tujuan dan argumen (JsonSerializerDefaults.Web). readonly membatasi penggantian referensi/nilai field pada deklarasi atau
    // konstruktor. static membuat field menjadi milik tipe dan dibagikan antar instance.
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    // Mendeklarasikan field bertipe `SecurityAuditRepository`: `_repository` menyimpan nilai repositori. readonly membatasi penggantian referensi/nilai
    // field pada deklarasi atau konstruktor.
    private readonly SecurityAuditRepository _repository;
    // Mendeklarasikan field bertipe `ILogger<SecurityAuditService>`: `_logger` menyimpan pencatat log terstruktur untuk memantau proses dan
    // mendiagnosis kegagalan. readonly membatasi penggantian referensi/nilai field pada deklarasi atau konstruktor.
    private readonly ILogger<SecurityAuditService> _logger;

    /// <summary>
    /// Menerima SecurityAuditRepository dan logger untuk pencatatan audit.
    /// </summary>
    // Mendefinisikan konstruktor SecurityAuditService yang menyiapkan objek dan menerima dependency/nilai awal dari pemanggil; parameter: Parameter
    // `repository` bertipe `SecurityAuditRepository` membawa nilai repositori; Parameter `logger` bertipe `ILogger<SecurityAuditService>` membawa
    // pencatat log terstruktur untuk memantau proses dan mendiagnosis kegagalan.
    public SecurityAuditService(SecurityAuditRepository repository, ILogger<SecurityAuditService> logger)
    // Membuka scope konstruktor SecurityAuditService; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam SecurityAuditService.
    {
        // Memperbarui `_repository` menggunakan `repository` (nilai repositori) dalam SecurityAuditService.
        _repository = repository;
        // Memperbarui `_logger` menggunakan `logger` (pencatat log terstruktur untuk memantau proses dan mendiagnosis kegagalan) dalam
        // SecurityAuditService.
        _logger = logger;
    // Menutup scope konstruktor SecurityAuditService; bagian berikut berada di luar batas blok tersebut dalam SecurityAuditService.
    }

    /// <summary>
    /// Menyimpan satu entri audit keamanan; kegagalan ditelan agar tidak mengganggu request utama.
    /// </summary>
    // Mendefinisikan metode `LogAsync` dengan hasil bertipe `Task`. Menyimpan satu entri audit keamanan; kegagalan ditelan agar tidak mengganggu
    // request utama. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter
    // `context` bertipe `HttpContext` membawa konteks operasi yang menyediakan data lingkungan pemrosesan saat ini; Parameter `eventType` bertipe
    // `string` membawa jenis aktivitas yang menentukan aturan validasi dan proyeksi event; Parameter `outcome` bertipe `string` membawa nilai hasil;
    // Parameter `statusCode` bertipe `int` membawa kode status hasil HTTP yang mengomunikasikan keberhasilan atau kegagalan; Parameter `details`
    // bertipe `object?` membawa nilai rincian; nilai null diizinkan ketika data opsional belum tersedia; Parameter `ct` bertipe `CancellationToken`
    // membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task LogAsync(
        // Parameter `context` bertipe `HttpContext` membawa konteks operasi yang menyediakan data lingkungan pemrosesan saat ini.
        HttpContext context,
        // Parameter `eventType` bertipe `string` membawa jenis aktivitas yang menentukan aturan validasi dan proyeksi event.
        string eventType,
        // Parameter `outcome` bertipe `string` membawa nilai hasil.
        string outcome,
        // Parameter `statusCode` bertipe `int` membawa kode status hasil HTTP yang mengomunikasikan keberhasilan atau kegagalan.
        int statusCode,
        // Parameter `details` bertipe `object?` membawa nilai rincian; nilai null diizinkan ketika data opsional belum tersedia.
        object? details,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    // Membuka scope metode LogAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam LogAsync.
    {
        // Menyiapkan variabel lokal `traceId` untuk identitas penelusuran yang menghubungkan respons, log, dan permintaan yang sama dengan memanggil
        // `ResolveTraceId` dengan `context`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var traceId = ResolveTraceId(context);
        // Menyiapkan variabel lokal `userIdText` untuk nilai pengguna identitas text dengan memanggil `context.User.FindFirstValue` dengan
        // `ClaimTypes.NameIdentifier`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var userIdText = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        // Menyiapkan variabel lokal `username` untuk nama akun yang dipakai saat autentikasi dengan memanggil `context.User.FindFirstValue` dengan
        // `ClaimTypes.Name`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var username = context.User.FindFirstValue(ClaimTypes.Name);
        // Menyiapkan variabel lokal `role` untuk peran pengguna yang menentukan hak akses dengan memanggil `context.User.FindFirstValue` dengan
        // `ClaimTypes.Role`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var role = context.User.FindFirstValue(ClaimTypes.Role);
        // Menyiapkan variabel lokal `ipAddress` untuk nilai ip address dengan `context.Connection.RemoteIpAddress?.ToString()`; akses setelah ?. hanya
        // dilakukan bila penerimanya tidak null. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var ipAddress = context.Connection.RemoteIpAddress?.ToString();
        // Menyiapkan variabel lokal `userAgent` untuk nilai pengguna agent dengan mengubah `context.Request.Headers.UserAgent` menjadi teks. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var userAgent = context.Request.Headers.UserAgent.ToString();

        // Menyiapkan variabel lokal `userId` untuk identitas akun pengguna yang datanya sedang diproses dengan null, yaitu penanda tidak ada nilai. Tipe
        // yang dipakai adalah `Guid?`.
        Guid? userId = null;
        // Memeriksa mencoba mengonversi `userIdText`, `var parsed` melalui `Guid.TryParse`; keberhasilan dilaporkan sebagai boolean dan hasil ditempatkan
        // pada argumen out; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam LogAsync.
        if (Guid.TryParse(userIdText, out var parsed))
        // Membuka scope cabang if untuk kondisi `Guid.TryParse(userIdText, out var parsed)`; pernyataan/deklarasi berikut berada di dalam batas blok ini
        // dalam LogAsync.
        {
            // Memperbarui `userId` menggunakan `parsed` (nilai parsed) dalam LogAsync.
            userId = parsed;
        // Menutup scope cabang if untuk kondisi `Guid.TryParse(userIdText, out var parsed)`; bagian berikut berada di luar batas blok tersebut dalam
        // LogAsync.
        }

        // Menyiapkan variabel lokal `detailJson` untuk nilai detail JSON dengan null, yaitu penanda tidak ada nilai. Tipe yang dipakai adalah `string?`.
        string? detailJson = null;
        // Memeriksa hasil pencocokan `details` dengan pola `not null`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam LogAsync.
        if (details is not null)
        // Membuka scope cabang if untuk kondisi `details is not null`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam LogAsync.
        {
            // Memperbarui `detailJson` menggunakan menserialisasi `details`, `JsonOptions` menjadi JSON melalui `JsonSerializer.Serialize` dalam LogAsync.
            detailJson = JsonSerializer.Serialize(details, JsonOptions);
        // Menutup scope cabang if untuk kondisi `details is not null`; bagian berikut berada di luar batas blok tersebut dalam LogAsync.
        }

        // Menyiapkan variabel lokal `entry` untuk nilai entry dengan objek baru bertipe `SecurityAuditLogDb` dengan nilai awal sesuai konstruktornya. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var entry = new SecurityAuditLogDb
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam LogAsync.
        {
            // Memperbarui `SecurityAuditLogId` menggunakan memanggil `Guid.NewGuid` dengan tanpa argumen dalam LogAsync.
            SecurityAuditLogId = Guid.NewGuid(),
            // Memperbarui `OccurredAt` menggunakan `DateTimeOffset.UtcNow`, yaitu waktu UTC saat operasi dilakukan dalam LogAsync.
            OccurredAt = DateTimeOffset.UtcNow,
            // Memperbarui `TraceId` menggunakan `traceId` (identitas penelusuran yang menghubungkan respons, log, dan permintaan yang sama) dalam LogAsync.
            TraceId = traceId,
            // Memperbarui `EventType` menggunakan `eventType` (jenis aktivitas yang menentukan aturan validasi dan proyeksi event) dalam LogAsync.
            EventType = eventType,
            // Memperbarui `Outcome` menggunakan `outcome` (nilai hasil) dalam LogAsync.
            Outcome = outcome,
            // Memperbarui `UserId` menggunakan `userId` (identitas akun pengguna yang datanya sedang diproses) dalam LogAsync.
            UserId = userId,
            // Memperbarui `Username` menggunakan `username` (nama akun yang dipakai saat autentikasi) dalam LogAsync.
            Username = username,
            // Memperbarui `Role` menggunakan `role` (peran pengguna yang menentukan hak akses) dalam LogAsync.
            Role = role,
            // Memperbarui `IpAddress` menggunakan `ipAddress` (nilai ip address) dalam LogAsync.
            IpAddress = ipAddress,
            // Memperbarui `UserAgent` menggunakan hasil pemilihan bersyarat: ketika `string.IsNullOrWhiteSpace(userAgent)` benar gunakan `null`, jika tidak
            // gunakan `userAgent` dalam LogAsync.
            UserAgent = string.IsNullOrWhiteSpace(userAgent) ? null : userAgent,
            // Memperbarui `Method` menggunakan `context.Request.Method` (nilai method) dalam LogAsync.
            Method = context.Request.Method,
            // Memperbarui `Path` menggunakan `context.Request.Path.Value` bila tidak null; jika null gunakan `”/”` sebagai nilai pengganti dalam LogAsync.
            Path = context.Request.Path.Value ?? "/",
            // Memperbarui `StatusCode` menggunakan `statusCode` (kode status hasil HTTP yang mengomunikasikan keberhasilan atau kegagalan) dalam LogAsync.
            StatusCode = statusCode,
            // Memperbarui `DetailJson` menggunakan `detailJson` (nilai detail JSON) dalam LogAsync.
            DetailJson = detailJson
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam LogAsync.
        };

        // Memulai blok try dalam LogAsync; exception dari blok ini dapat dialihkan ke catch, sedangkan finally (jika ada) tetap dijalankan saat keluar.
        try
        // Membuka scope penanganan operasi try; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam LogAsync.
        {
            // Menjalankan hasil operasi asinkron memanggil `_repository.InsertAsync` dengan `entry`, `ct`; await menunggu hasil tanpa memblokir thread selama
            // operasi belum selesai dalam LogAsync.
            await _repository.InsertAsync(entry, ct);
        // Menutup scope penanganan operasi try; bagian berikut berada di luar batas blok tersebut dalam LogAsync.
        }
        // Menangani exception `Exception` melalui variabel ex dalam LogAsync.
        catch (Exception ex)
        // Membuka scope penanganan exception catch; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam LogAsync.
        {
            // Menjalankan mencatat log tingkat Error melalui `_logger` dengan pesan dan data `ex`, `”Gagal menyimpan security audit log event_type={EventType}
            // trace_id={TraceId}”`, `eventType`, `traceId` dalam LogAsync.
            _logger.LogError(
                // Meneruskan `ex` (nilai ex) sebagai argumen ke `_logger.LogError`.
                ex,
                // Meneruskan nilai literal `”Gagal menyimpan security audit log event_type={EventType} trace_id={TraceId}”` sebagai argumen ke `_logger.LogError`.
                "Gagal menyimpan security audit log event_type={EventType} trace_id={TraceId}",
                // Meneruskan `eventType` (jenis aktivitas yang menentukan aturan validasi dan proyeksi event) sebagai argumen ke `_logger.LogError`.
                eventType,
                // Meneruskan `traceId` (identitas penelusuran yang menghubungkan respons, log, dan permintaan yang sama) sebagai argumen ke `_logger.LogError`.
                traceId);
        // Menutup scope penanganan exception catch; bagian berikut berada di luar batas blok tersebut dalam LogAsync.
        }
    // Menutup scope metode LogAsync; bagian berikut berada di luar batas blok tersebut dalam LogAsync.
    }

    /// <summary>
    /// Mengambil trace ID dari HttpContext atau Activity.Current; fallback ke "unknown-trace".
    /// </summary>
    // Mendefinisikan metode `ResolveTraceId` dengan hasil bertipe `string`. Mengambil trace ID dari HttpContext atau Activity.Current; fallback ke
    // ”unknown-trace”. Masukan: Parameter `context` bertipe `HttpContext` membawa konteks operasi yang menyediakan data lingkungan pemrosesan saat ini.
    private static string ResolveTraceId(HttpContext context)
    // Membuka scope metode ResolveTraceId; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveTraceId.
    {
        // Menyiapkan variabel lokal `traceId` untuk identitas penelusuran yang menghubungkan respons, log, dan permintaan yang sama dengan
        // `context.TraceIdentifier` (nilai trace identifier). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var traceId = context.TraceIdentifier;
        // Memeriksa kebalikan kondisi `string.IsNullOrWhiteSpace(traceId)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // ResolveTraceId.
        if (!string.IsNullOrWhiteSpace(traceId))
        // Membuka scope cabang if untuk kondisi `!string.IsNullOrWhiteSpace(traceId)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ResolveTraceId.
        {
            // Mengembalikan `traceId` (identitas penelusuran yang menghubungkan respons, log, dan permintaan yang sama) kepada pemanggil dalam ResolveTraceId;
            // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return traceId;
        // Menutup scope cabang if untuk kondisi `!string.IsNullOrWhiteSpace(traceId)`; bagian berikut berada di luar batas blok tersebut dalam
        // ResolveTraceId.
        }

        // Menyiapkan variabel lokal `activityTraceId` untuk nilai activity trace identitas dengan `Activity.Current?.TraceId.ToString()`; akses setelah ?.
        // hanya dilakukan bila penerimanya tidak null. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var activityTraceId = Activity.Current?.TraceId.ToString();
        // Mengembalikan hasil pemilihan bersyarat: ketika `string.IsNullOrWhiteSpace(activityTraceId)` benar gunakan `”unknown-trace”`, jika tidak gunakan
        // `activityTraceId` kepada pemanggil dalam ResolveTraceId; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return string.IsNullOrWhiteSpace(activityTraceId) ? "unknown-trace" : activityTraceId;
    // Menutup scope metode ResolveTraceId; bagian berikut berada di luar batas blok tersebut dalam ResolveTraceId.
    }
// Menutup scope tipe SecurityAuditService; bagian berikut berada di luar batas blok tersebut.
}
