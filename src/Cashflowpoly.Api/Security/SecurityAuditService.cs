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
{
    public const string LoginSuccess = "LOGIN_SUCCESS";
    public const string LoginFailed = "LOGIN_FAILED";
    public const string RegisterSuccess = "REGISTER_SUCCESS";
    public const string RegisterDenied = "REGISTER_DENIED";
    public const string AuthChallenge = "AUTH_CHALLENGE";
    public const string AuthForbidden = "AUTH_FORBIDDEN";
    public const string AuthFailed = "AUTH_FAILED";
    public const string RateLimited = "RATE_LIMITED";
    public const string SetupSaved = "SETUP_SAVED";
    public const string SessionStarted = "SESSION_STARTED";
    public const string SessionEnded = "SESSION_ENDED";
    public const string RulesetChanged = "RULESET_CHANGED";
    public const string DemoActivity = "DEMO_ACTIVITY";
    public const string PasswordChanged = "PASSWORD_CHANGED";
    public const string PasswordChangeFailed = "PASSWORD_CHANGE_FAILED";
}

/// <summary>
/// Konstanta string outcome audit keamanan: SUCCESS, FAILURE, DENIED.
/// </summary>
// Mendefinisikan tipe class `SecurityAuditOutcomes`.
public static class SecurityAuditOutcomes
{
    public const string Success = "SUCCESS";
    public const string Failure = "FAILURE";
    public const string Denied = "DENIED";
}

/// <summary>
/// Layanan untuk menyimpan audit event keamanan tanpa mengganggu jalur request utama.
/// </summary>
// Mendefinisikan tipe class `SecurityAuditService`; sealed mencegah tipe ini diturunkan lagi.
public sealed class SecurityAuditService
{
    /// <summary>
    /// Opsi serialisasi JSON web-default untuk mengonversi detail audit menjadi string JSON.
    /// </summary>
    // Mendeklarasikan field bertipe `JsonSerializerOptions`: `JsonOptions` menyimpan nilai JSON options dengan nilai awal objek baru dengan tipe
    // mengikuti konteks tujuan dan argumen (JsonSerializerDefaults.Web). readonly membatasi penggantian referensi/nilai field pada deklarasi atau
    // konstruktor. static membuat field menjadi milik tipe dan dibagikan antar instance.
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly SecurityAuditRepository _repository;
    private readonly ILogger<SecurityAuditService> _logger;

    /// <summary>
    /// Menerima SecurityAuditRepository dan logger untuk pencatatan audit.
    /// </summary>
    // Mendefinisikan konstruktor SecurityAuditService yang menyiapkan objek dan menerima dependency/nilai awal dari pemanggil; parameter: Parameter
    // `repository` bertipe `SecurityAuditRepository` membawa nilai repositori; Parameter `logger` bertipe `ILogger<SecurityAuditService>` membawa
    // pencatat log terstruktur untuk memantau proses dan mendiagnosis kegagalan.
    public SecurityAuditService(SecurityAuditRepository repository, ILogger<SecurityAuditService> logger)
    {
        _repository = repository;
        _logger = logger;
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
        CancellationToken ct, Guid? subjectUserId = null)
    {
        var traceId = ResolveTraceId(context);
        var userIdText = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var username = context.User.FindFirstValue(ClaimTypes.Name);
        var role = context.User.FindFirstValue(ClaimTypes.Role);
        var ipAddress = context.Connection.RemoteIpAddress?.ToString();
        var userAgent = context.Request.Headers.UserAgent.ToString();

        Guid? userId = subjectUserId;
        if (Guid.TryParse(userIdText, out var parsed))
        {
            userId = parsed;
        }

        string? detailJson = null;
        if (details is not null)
        {
            detailJson = JsonSerializer.Serialize(details, JsonOptions);
        }

        var entry = new SecurityAuditLogDb
        {
            SecurityAuditLogId = Guid.NewGuid(),
            OccurredAt = DateTimeOffset.UtcNow,
            TraceId = traceId,
            EventType = eventType,
            Outcome = outcome,
            UserId = userId,
            Username = username,
            Role = role,
            IpAddress = ipAddress,
            UserAgent = string.IsNullOrWhiteSpace(userAgent) ? null : userAgent,
            Method = context.Request.Method,
            Path = context.Request.Path.Value ?? "/",
            StatusCode = statusCode,
            DetailJson = detailJson
        };

        try
        {
            await _repository.InsertAsync(entry, ct);
        }
        // Menangani exception `Exception` melalui variabel ex dalam LogAsync.
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Gagal menyimpan security audit log event_type={EventType} trace_id={TraceId}",
                eventType,
                traceId);
        }
    }

    /// <summary>
    /// Mengambil trace ID dari HttpContext atau Activity.Current; fallback ke "unknown-trace".
    /// </summary>
    // Mendefinisikan metode `ResolveTraceId` dengan hasil bertipe `string`. Mengambil trace ID dari HttpContext atau Activity.Current; fallback ke
    // ”unknown-trace”. Masukan: Parameter `context` bertipe `HttpContext` membawa konteks operasi yang menyediakan data lingkungan pemrosesan saat ini.
    private static string ResolveTraceId(HttpContext context)
    {
        var traceId = context.TraceIdentifier;
        if (!string.IsNullOrWhiteSpace(traceId))
        {
            return traceId;
        }

        var activityTraceId = Activity.Current?.TraceId.ToString();
        return string.IsNullOrWhiteSpace(activityTraceId) ? "unknown-trace" : activityTraceId;
    }
}
