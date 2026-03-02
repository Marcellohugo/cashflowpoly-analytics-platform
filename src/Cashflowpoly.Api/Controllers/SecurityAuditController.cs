// Fungsi file: Menyediakan endpoint untuk instruktur melihat log audit keamanan dengan filter event type dan user.
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
/// Controller audit keamanan yang menyediakan akses log audit untuk instruktur.
/// </summary>
public sealed class SecurityAuditController : ControllerBase
{
    private readonly SecurityAuditRepository _securityAudit;

    /// <summary>
    /// Menginisialisasi controller dengan repositori audit keamanan.
    /// </summary>
    public SecurityAuditController(SecurityAuditRepository securityAudit)
    {
        _securityAudit = securityAudit;
    }

    [HttpGet("audit-logs")]
    [ProducesResponseType(typeof(SecurityAuditLogResponse), StatusCodes.Status200OK)]
    /// <summary>
    /// Mengambil daftar log audit keamanan terbaru dengan opsi filter berdasarkan event type dan user ID.
    /// </summary>
    /// <param name="limit">Jumlah maksimum log (1-500, default 100).</param>
    /// <param name="eventType">Filter jenis event audit (opsional).</param>
    /// <param name="userId">Filter berdasarkan user ID (opsional).</param>
    /// <param name="ct">Token pembatalan.</param>
    /// <returns>200 OK dengan daftar log audit keamanan.</returns>
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
    /// Mem-parse string JSON menjadi JsonElement, mengembalikan null jika string kosong atau tidak valid.
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
