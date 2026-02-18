using System.Text.Json;
using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Domain;
using Cashflowpoly.Api.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cashflowpoly.Api.Controllers;

[ApiController]
[Route("api/v1/rulesets")]
[Authorize]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status429TooManyRequests)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
public sealed class RulesetsController : ControllerBase
{
    private readonly RulesetRepository _rulesets;
    private readonly UserRepository _users;

    public RulesetsController(RulesetRepository rulesets, UserRepository users)
    {
        _rulesets = rulesets;
        _users = users;
    }

    /// <summary>
    /// Membuat ruleset baru beserta versi awal.
    /// </summary>

    [HttpPost]
    [Authorize(Roles = "INSTRUCTOR")]
    [ProducesResponseType(typeof(CreateRulesetResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateRuleset([FromBody] CreateRulesetRequest request, CancellationToken ct)
    {
        if (!TryGetCurrentUserId(out var instructorUserId))
        {
            return Unauthorized(ApiErrorHelper.BuildError(HttpContext, "UNAUTHORIZED", "Token user tidak valid"));
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest(ApiErrorHelper.BuildError(HttpContext, "VALIDATION_ERROR", "Field wajib tidak lengkap",
                new ErrorDetail("name", "REQUIRED")));
        }
        if (!RulesetConfigParser.TryParse(request.Config, out _, out var configErrors))
        {
            return BadRequest(ApiErrorHelper.BuildError(HttpContext, "VALIDATION_ERROR", "Konfigurasi ruleset tidak valid", configErrors.ToArray()));
        }

        var configJson = request.Config.GetRawText();
        var created = await _rulesets.CreateRulesetAsync(
            request.Name,
            request.Description,
            instructorUserId,
            configJson,
            GetActorName(),
            ct);

        return Created($"/api/v1/rulesets/{created.RulesetId}", new CreateRulesetResponse(created.RulesetId, created.Version));
    }

    [HttpPut("{rulesetId:guid}")]
    [Authorize(Roles = "INSTRUCTOR")]
    [ProducesResponseType(typeof(CreateRulesetResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateRuleset(Guid rulesetId, [FromBody] UpdateRulesetRequest request, CancellationToken ct)
    {
        if (!TryGetCurrentUserId(out var instructorUserId))
        {
            return Unauthorized(ApiErrorHelper.BuildError(HttpContext, "UNAUTHORIZED", "Token user tidak valid"));
        }

        var existing = await _rulesets.GetRulesetForInstructorAsync(rulesetId, instructorUserId, ct);
        if (existing is null)
        {
            return NotFound(ApiErrorHelper.BuildError(HttpContext, "NOT_FOUND", "Ruleset tidak ditemukan"));
        }

        if (request.Config is null)
        {
            return BadRequest(ApiErrorHelper.BuildError(HttpContext, "VALIDATION_ERROR", "Config wajib ada",
                new ErrorDetail("config", "REQUIRED")));
        }
        if (!RulesetConfigParser.TryParse(request.Config.Value, out _, out var configErrors))
        {
            return BadRequest(ApiErrorHelper.BuildError(HttpContext, "VALIDATION_ERROR", "Konfigurasi ruleset tidak valid", configErrors.ToArray()));
        }

        var configJson = request.Config.Value.GetRawText();
        var nextVersion = await _rulesets.CreateRulesetVersionAsync(
            rulesetId,
            request.Name,
            request.Description,
            configJson,
            GetActorName(),
            ct);

        return Ok(new CreateRulesetResponse(rulesetId, nextVersion));
    }

    [HttpPost("{rulesetId:guid}/versions/{version:int}/activate")]
    [Authorize(Roles = "INSTRUCTOR")]
    [ProducesResponseType(typeof(CreateRulesetResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> ActivateRulesetVersion(Guid rulesetId, int version, CancellationToken ct)
    {
        if (!TryGetCurrentUserId(out var instructorUserId))
        {
            return Unauthorized(ApiErrorHelper.BuildError(HttpContext, "UNAUTHORIZED", "Token user tidak valid"));
        }

        var ruleset = await _rulesets.GetRulesetForInstructorAsync(rulesetId, instructorUserId, ct);
        if (ruleset is null)
        {
            return NotFound(ApiErrorHelper.BuildError(HttpContext, "NOT_FOUND", "Ruleset tidak ditemukan"));
        }

        var selectedVersion = await _rulesets.GetRulesetVersionAsync(rulesetId, version, ct);
        if (selectedVersion is null)
        {
            return NotFound(ApiErrorHelper.BuildError(HttpContext, "NOT_FOUND", "Ruleset version tidak ditemukan"));
        }

        if (!RulesetConfigParser.TryParse(selectedVersion.ConfigJson, out _, out var configErrors))
        {
            return BadRequest(ApiErrorHelper.BuildError(
                HttpContext,
                "VALIDATION_ERROR",
                "Konfigurasi ruleset tidak valid",
                configErrors.ToArray()));
        }

        await _rulesets.ActivateRulesetVersionAsync(rulesetId, version, ct);
        return Ok(new CreateRulesetResponse(rulesetId, version));
    }

    [HttpGet]
    [ProducesResponseType(typeof(RulesetListResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListRulesets(CancellationToken ct)
    {
        if (!TryGetCurrentUserId(out var userId))
        {
            return Unauthorized(ApiErrorHelper.BuildError(HttpContext, "UNAUTHORIZED", "Token user tidak valid"));
        }

        var role = User.FindFirstValue(ClaimTypes.Role);
        List<RulesetListItem> items;

        if (string.Equals(role, "INSTRUCTOR", StringComparison.OrdinalIgnoreCase))
        {
            items = await _rulesets.ListRulesetsByInstructorAsync(userId, ct);
        }
        else if (string.Equals(role, "PLAYER", StringComparison.OrdinalIgnoreCase))
        {
            var linkedPlayerId = await _users.GetLinkedPlayerIdAsync(userId, ct);
            if (!linkedPlayerId.HasValue)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    ApiErrorHelper.BuildError(HttpContext, "FORBIDDEN", "Akun PLAYER belum terhubung ke profil pemain"));
            }

            items = await _rulesets.ListRulesetsByPlayerAsync(linkedPlayerId.Value, ct);
        }
        else
        {
            return StatusCode(StatusCodes.Status403Forbidden,
                ApiErrorHelper.BuildError(HttpContext, "FORBIDDEN", "Role tidak diizinkan"));
        }

        return Ok(new RulesetListResponse(items));
    }

    [HttpGet("{rulesetId:guid}")]
    [ProducesResponseType(typeof(RulesetDetailResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRulesetDetail(Guid rulesetId, CancellationToken ct)
    {
        if (!TryGetCurrentUserId(out var userId))
        {
            return Unauthorized(ApiErrorHelper.BuildError(HttpContext, "UNAUTHORIZED", "Token user tidak valid"));
        }

        var role = User.FindFirstValue(ClaimTypes.Role);
        RulesetDb? ruleset;
        if (string.Equals(role, "INSTRUCTOR", StringComparison.OrdinalIgnoreCase))
        {
            ruleset = await _rulesets.GetRulesetForInstructorAsync(rulesetId, userId, ct);
        }
        else if (string.Equals(role, "PLAYER", StringComparison.OrdinalIgnoreCase))
        {
            var linkedPlayerId = await _users.GetLinkedPlayerIdAsync(userId, ct);
            if (!linkedPlayerId.HasValue)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    ApiErrorHelper.BuildError(HttpContext, "FORBIDDEN", "Akun PLAYER belum terhubung ke profil pemain"));
            }

            ruleset = await _rulesets.GetRulesetForPlayerAsync(rulesetId, linkedPlayerId.Value, ct);
        }
        else
        {
            return StatusCode(StatusCodes.Status403Forbidden,
                ApiErrorHelper.BuildError(HttpContext, "FORBIDDEN", "Role tidak diizinkan"));
        }

        if (ruleset is null)
        {
            return NotFound(ApiErrorHelper.BuildError(HttpContext, "NOT_FOUND", "Ruleset tidak ditemukan"));
        }

        var versions = await _rulesets.ListRulesetVersionsAsync(rulesetId, ct);
        var versionItems = versions.Select(v => new RulesetVersionItem(
            v.RulesetVersionId,
            v.Version,
            v.Status,
            v.CreatedAt)).ToList();

        JsonElement? configJson = null;
        var latest = versions.FirstOrDefault();
        if (latest is not null)
        {
            using var doc = JsonDocument.Parse(latest.ConfigJson);
            configJson = doc.RootElement.Clone();
        }

        var response = new RulesetDetailResponse(
            ruleset.RulesetId,
            ruleset.Name,
            ruleset.Description,
            ruleset.IsArchived,
            versionItems,
            configJson);

        return Ok(response);
    }

    /// <summary>
    /// Mengarsipkan ruleset (soft archive).
    /// </summary>
    [HttpPost("{rulesetId:guid}/archive")]
    [Authorize(Roles = "INSTRUCTOR")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ArchiveRuleset(Guid rulesetId, CancellationToken ct)
    {
        if (!TryGetCurrentUserId(out var instructorUserId))
        {
            return Unauthorized(ApiErrorHelper.BuildError(HttpContext, "UNAUTHORIZED", "Token user tidak valid"));
        }

        var ruleset = await _rulesets.GetRulesetForInstructorAsync(rulesetId, instructorUserId, ct);
        if (ruleset is null)
        {
            return NotFound(ApiErrorHelper.BuildError(HttpContext, "NOT_FOUND", "Ruleset tidak ditemukan"));
        }

        await _rulesets.SetArchiveAsync(rulesetId, true, ct);
        return Ok();
    }

    /// <summary>
    /// Menghapus ruleset jika belum pernah dipakai pada sesi.
    /// </summary>
    [HttpDelete("{rulesetId:guid}")]
    [Authorize(Roles = "INSTRUCTOR")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteRuleset(Guid rulesetId, CancellationToken ct)
    {
        if (!TryGetCurrentUserId(out var instructorUserId))
        {
            return Unauthorized(ApiErrorHelper.BuildError(HttpContext, "UNAUTHORIZED", "Token user tidak valid"));
        }

        var ruleset = await _rulesets.GetRulesetForInstructorAsync(rulesetId, instructorUserId, ct);
        if (ruleset is null)
        {
            return NotFound(ApiErrorHelper.BuildError(HttpContext, "NOT_FOUND", "Ruleset tidak ditemukan"));
        }

        var inUse = await _rulesets.IsRulesetUsedAsync(rulesetId, ct);
        if (inUse)
        {
            return UnprocessableEntity(ApiErrorHelper.BuildError(HttpContext, "DOMAIN_RULE_VIOLATION", "Ruleset sudah dipakai sesi"));
        }

        await _rulesets.DeleteRulesetAsync(rulesetId, ct);
        return NoContent();
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
