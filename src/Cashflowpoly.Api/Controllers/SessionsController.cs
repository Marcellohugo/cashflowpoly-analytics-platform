using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Domain;
using Cashflowpoly.Api.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cashflowpoly.Api.Controllers;

[ApiController]
[Route("api/v1/sessions")]
[Route("api/sessions")]
[Authorize]
public sealed class SessionsController : ControllerBase
{
    private readonly RulesetRepository _rulesets;
    private readonly SessionRepository _sessions;

    public SessionsController(RulesetRepository rulesets, SessionRepository sessions)
    {
        _rulesets = rulesets;
        _sessions = sessions;
    }

    /// <summary>
    /// Mengembalikan daftar sesi yang ada.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> ListSessions(CancellationToken ct)
    {
        if (!TryGetCurrentUserId(out var instructorUserId))
        {
            return Unauthorized(ApiErrorHelper.BuildError(HttpContext, "UNAUTHORIZED", "Token user tidak valid"));
        }

        var sessions = await _sessions.ListSessionsByInstructorAsync(instructorUserId, ct);
        var items = sessions.Select(s => new SessionListItem(
            s.SessionId,
            s.SessionName,
            s.Mode,
            s.Status,
            s.CreatedAt,
            s.StartedAt,
            s.EndedAt)).ToList();

        return Ok(new SessionListResponse(items));
    }

    [HttpPost]
    [Authorize(Roles = "INSTRUCTOR")]
    public async Task<IActionResult> CreateSession([FromBody] CreateSessionRequest request, CancellationToken ct)
    {
        if (!TryGetCurrentUserId(out var instructorUserId))
        {
            return Unauthorized(ApiErrorHelper.BuildError(HttpContext, "UNAUTHORIZED", "Token user tidak valid"));
        }

        if (string.IsNullOrWhiteSpace(request.SessionName))
        {
            return BadRequest(ApiErrorHelper.BuildError(HttpContext, "VALIDATION_ERROR", "Field wajib tidak lengkap",
                new ErrorDetail("session_name", "REQUIRED")));
        }

        if (!string.Equals(request.Mode, "PEMULA", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(request.Mode, "MAHIR", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(ApiErrorHelper.BuildError(HttpContext, "VALIDATION_ERROR", "Mode tidak valid",
                new ErrorDetail("mode", "INVALID_ENUM")));
        }

        var ruleset = await _rulesets.GetRulesetForInstructorAsync(request.RulesetId, instructorUserId, ct);
        if (ruleset is null)
        {
            return NotFound(ApiErrorHelper.BuildError(HttpContext, "NOT_FOUND", "Ruleset tidak ditemukan"));
        }

        var latestVersion = await _rulesets.GetLatestActiveVersionAsync(request.RulesetId, ct);
        if (latestVersion is null)
        {
            return NotFound(ApiErrorHelper.BuildError(HttpContext, "NOT_FOUND", "Ruleset belum memiliki versi ACTIVE"));
        }

        var sessionId = await _sessions.CreateSessionAsync(
            request.SessionName,
            request.Mode.ToUpperInvariant(),
            latestVersion.RulesetVersionId,
            instructorUserId,
            GetActorName(),
            ct);

        return Created($"/api/v1/sessions/{sessionId}", new CreateSessionResponse(sessionId));
    }

    [HttpPost("{sessionId:guid}/start")]
    [Authorize(Roles = "INSTRUCTOR")]
    public async Task<IActionResult> StartSession(Guid sessionId, CancellationToken ct)
    {
        if (!TryGetCurrentUserId(out var instructorUserId))
        {
            return Unauthorized(ApiErrorHelper.BuildError(HttpContext, "UNAUTHORIZED", "Token user tidak valid"));
        }

        var session = await _sessions.GetSessionForInstructorAsync(sessionId, instructorUserId, ct);
        if (session is null)
        {
            return NotFound(ApiErrorHelper.BuildError(HttpContext, "NOT_FOUND", "Session tidak ditemukan"));
        }

        if (!string.Equals(session.Status, "CREATED", StringComparison.OrdinalIgnoreCase))
        {
            return UnprocessableEntity(ApiErrorHelper.BuildError(HttpContext, "DOMAIN_RULE_VIOLATION", "Status sesi tidak valid"));
        }

        var startedAt = DateTimeOffset.UtcNow;
        await _sessions.UpdateStatusAsync(sessionId, "STARTED", startedAt, session.EndedAt, ct);

        return Ok(new SessionStatusResponse("STARTED"));
    }

    [HttpPost("{sessionId:guid}/end")]
    [Authorize(Roles = "INSTRUCTOR")]
    public async Task<IActionResult> EndSession(Guid sessionId, CancellationToken ct)
    {
        if (!TryGetCurrentUserId(out var instructorUserId))
        {
            return Unauthorized(ApiErrorHelper.BuildError(HttpContext, "UNAUTHORIZED", "Token user tidak valid"));
        }

        var session = await _sessions.GetSessionForInstructorAsync(sessionId, instructorUserId, ct);
        if (session is null)
        {
            return NotFound(ApiErrorHelper.BuildError(HttpContext, "NOT_FOUND", "Session tidak ditemukan"));
        }

        if (!string.Equals(session.Status, "STARTED", StringComparison.OrdinalIgnoreCase))
        {
            return UnprocessableEntity(ApiErrorHelper.BuildError(HttpContext, "DOMAIN_RULE_VIOLATION", "Status sesi tidak valid"));
        }

        var endedAt = DateTimeOffset.UtcNow;
        await _sessions.UpdateStatusAsync(sessionId, "ENDED", session.StartedAt, endedAt, ct);

        return Ok(new SessionStatusResponse("ENDED"));
    }

    [HttpPost("{sessionId:guid}/ruleset/activate")]
    [Authorize(Roles = "INSTRUCTOR")]
    public async Task<IActionResult> ActivateRuleset(Guid sessionId, [FromBody] ActivateRulesetRequest request, CancellationToken ct)
    {
        if (!TryGetCurrentUserId(out var instructorUserId))
        {
            return Unauthorized(ApiErrorHelper.BuildError(HttpContext, "UNAUTHORIZED", "Token user tidak valid"));
        }

        var session = await _sessions.GetSessionForInstructorAsync(sessionId, instructorUserId, ct);
        if (session is null)
        {
            return NotFound(ApiErrorHelper.BuildError(HttpContext, "NOT_FOUND", "Session tidak ditemukan"));
        }

        if (string.Equals(session.Status, "ENDED", StringComparison.OrdinalIgnoreCase))
        {
            return UnprocessableEntity(ApiErrorHelper.BuildError(HttpContext, "DOMAIN_RULE_VIOLATION", "Session sudah berakhir"));
        }

        var ruleset = await _rulesets.GetRulesetForInstructorAsync(request.RulesetId, instructorUserId, ct);
        if (ruleset is null)
        {
            return NotFound(ApiErrorHelper.BuildError(HttpContext, "NOT_FOUND", "Ruleset tidak ditemukan"));
        }

        var rulesetVersion = await _rulesets.GetRulesetVersionAsync(request.RulesetId, request.Version, ct);
        if (rulesetVersion is null)
        {
            return NotFound(ApiErrorHelper.BuildError(HttpContext, "NOT_FOUND", "Ruleset version tidak ditemukan"));
        }

        if (!string.Equals(rulesetVersion.Status, "ACTIVE", StringComparison.OrdinalIgnoreCase))
        {
            return UnprocessableEntity(ApiErrorHelper.BuildError(
                HttpContext,
                "DOMAIN_RULE_VIOLATION",
                "Ruleset version harus ACTIVE sebelum dipakai sesi"));
        }

        if (!RulesetConfigParser.TryParse(rulesetVersion.ConfigJson, out _, out var errors))
        {
            return BadRequest(ApiErrorHelper.BuildError(HttpContext, "VALIDATION_ERROR", "Konfigurasi ruleset tidak valid", errors.ToArray()));
        }

        await _sessions.ActivateRulesetAsync(sessionId, rulesetVersion.RulesetVersionId, GetActorName(), ct);

        return Ok(new ActivateRulesetResponse(sessionId, rulesetVersion.RulesetVersionId));
    }

    private string? GetActorName()
    {
        return User.FindFirstValue(ClaimTypes.Name) ??
               User.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    private bool TryGetCurrentUserId(out Guid userId)
    {
        var userIdRaw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userIdRaw, out userId);
    }
}
