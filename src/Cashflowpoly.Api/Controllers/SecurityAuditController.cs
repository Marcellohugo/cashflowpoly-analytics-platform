// Fungsi file: Mengelola endpoint API untuk domain SecurityAuditController termasuk validasi request dan respons standar.
using System.Text.Json;
using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cashflowpoly.Api.Controllers;

[ApiController]
[Route("api/v1/security")]
[Authorize(Roles = "INSTRUCTOR")]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status429TooManyRequests)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
/// <summary>
/// Menyatakan peran utama tipe SecurityAuditController pada modul ini.
/// </summary>
public sealed class SecurityAuditController : ControllerBase
{
    private readonly SecurityAuditRepository _securityAudit;

    /// <summary>
    /// Menjalankan fungsi SecurityAuditController sebagai bagian dari alur file ini.
    /// </summary>
    public SecurityAuditController(SecurityAuditRepository securityAudit)
    {
        _securityAudit = securityAudit;
    }

    [HttpGet("audit-logs")]
    [ProducesResponseType(typeof(SecurityAuditLogResponse), StatusCodes.Status200OK)]
    /// <summary>
    /// Menjalankan fungsi GetAuditLogs sebagai bagian dari alur file ini.
    /// </summary>
    public async Task<IActionResult> GetAuditLogs(
        [FromQuery] int limit = 100,
        [FromQuery] string? eventType = null,
        [FromQuery] Guid? userId = null,
        CancellationToken ct = default)
    {
        var clampedLimit = Math.Clamp(limit, 1, 500);
        var normalizedEventType = string.IsNullOrWhiteSpace(eventType)
            ? null
            : eventType.Trim().ToUpperInvariant();

        var logs = await _securityAudit.ListRecentAsync(clampedLimit, normalizedEventType, userId, ct);
        var items = logs
            .Select(log => new SecurityAuditLogItem(
                log.SecurityAuditLogId,
                log.OccurredAt,
                log.TraceId,
                log.EventType,
                log.Outcome,
                log.UserId,
                log.Username,
                log.Role,
                log.IpAddress,
                log.UserAgent,
                log.Method,
                log.Path,
                log.StatusCode,
                ParseJsonElement(log.DetailJson)))
            .ToList();

        return Ok(new SecurityAuditLogResponse(items));
    }

    /// <summary>
    /// Menjalankan fungsi ParseJsonElement sebagai bagian dari alur file ini.
    /// </summary>
    private static JsonElement? ParseJsonElement(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        using var document = JsonDocument.Parse(json);
        return document.RootElement.Clone();
    }
}
