// Fungsi file: Menyediakan undo aktivitas terakhir dan audit pembatalan bagi instruktur pemilik sesi.
using System.Security.Claims;
using Cashflowpoly.Api.Contracts;
using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cashflowpoly.Api.Controllers;

[ApiController]
[Route("api/v1/sessions/{sessionId:guid}")]
[Authorize(Roles = "INSTRUCTOR")]
public sealed class EventUndoController(EventUndoRepository repository) : ControllerBase
{
    [HttpPost("events/{eventId:guid}/undo")]
    [ProducesResponseType(typeof(EventUndoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(EventUndoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Undo(Guid sessionId, Guid eventId, UndoEventRequest request, CancellationToken ct)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Unauthorized(ApiErrorHelper.BuildError(HttpContext, "UNAUTHORIZED", "Token pengguna tidak valid"));
        var result = await repository.UndoAsync(sessionId, eventId, userId, request, ct);
        return result.Response is not null ? StatusCode(result.StatusCode, result.Response)
            : StatusCode(result.StatusCode, ApiErrorHelper.BuildError(HttpContext, result.ErrorCode!, result.Message!));
    }

    [HttpGet("event-undos")]
    [ProducesResponseType(typeof(EventUndoAuditResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> List(Guid sessionId, string? cursor = null, int limit = 50, CancellationToken ct = default)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Unauthorized(ApiErrorHelper.BuildError(HttpContext, "UNAUTHORIZED", "Token pengguna tidak valid"));
        if (limit is < 1 or > 100 || !OpaqueCursor.TryDecodeTransaction(cursor, out var timestamp, out var afterId))
            return BadRequest(ApiErrorHelper.BuildError(HttpContext, "VALIDATION_ERROR", "Cursor atau batas riwayat undo tidak valid"));
        var result = await repository.ListAsync(sessionId, userId, string.IsNullOrWhiteSpace(cursor) ? null : timestamp, afterId, limit, ct);
        return result is null ? NotFound(ApiErrorHelper.BuildError(HttpContext, "NOT_FOUND", "Sesi tidak ditemukan")) : Ok(result);
    }
}
