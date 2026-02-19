// Fungsi file: Menyediakan komponen keamanan aplikasi untuk domain SecurityAuditService (JWT, audit, atau rate limiting).
using System.Diagnostics;
using System.Security.Claims;
using System.Text.Json;
using Cashflowpoly.Api.Data;

namespace Cashflowpoly.Api.Security;

/// <summary>
/// Menyatakan peran utama tipe SecurityAuditEventTypes pada modul ini.
/// </summary>
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
}

/// <summary>
/// Menyatakan peran utama tipe SecurityAuditOutcomes pada modul ini.
/// </summary>
public static class SecurityAuditOutcomes
{
    public const string Success = "SUCCESS";
    public const string Failure = "FAILURE";
    public const string Denied = "DENIED";
}

/// <summary>
/// Layanan untuk menyimpan audit event keamanan tanpa mengganggu jalur request utama.
/// </summary>
public sealed class SecurityAuditService
{
    /// <summary>
    /// Menjalankan fungsi new sebagai bagian dari alur file ini.
    /// </summary>
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly SecurityAuditRepository _repository;
    private readonly ILogger<SecurityAuditService> _logger;

    /// <summary>
    /// Menjalankan fungsi SecurityAuditService sebagai bagian dari alur file ini.
    /// </summary>
    public SecurityAuditService(SecurityAuditRepository repository, ILogger<SecurityAuditService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Menjalankan fungsi LogAsync sebagai bagian dari alur file ini.
    /// </summary>
    public async Task LogAsync(
        HttpContext context,
        string eventType,
        string outcome,
        int statusCode,
        object? details,
        CancellationToken ct)
    {
        var traceId = ResolveTraceId(context);
        var userIdText = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var username = context.User.FindFirstValue(ClaimTypes.Name);
        var role = context.User.FindFirstValue(ClaimTypes.Role);
        var ipAddress = context.Connection.RemoteIpAddress?.ToString();
        var userAgent = context.Request.Headers.UserAgent.ToString();

        Guid? userId = null;
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
    /// Menjalankan fungsi ResolveTraceId sebagai bagian dari alur file ini.
    /// </summary>
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
