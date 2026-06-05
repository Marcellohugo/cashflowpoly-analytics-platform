using System.Text.Json;
using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Domain;
using Cashflowpoly.Api.Infrastructure;
using Cashflowpoly.Api.Contracts;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

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
    private readonly SessionStateRepository _state;
    private readonly UserRepository _users;

    public RulesetsController(RulesetRepository rulesets, SessionStateRepository state, UserRepository users)
    {
        _rulesets = rulesets;
        _state = state;
        _users = users;
    }

    [HttpGet("sections")]
    [ProducesResponseType(typeof(RulesetSectionsResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRulesetSections([FromQuery] string? mode, [FromQuery] Guid? rulesetId, CancellationToken ct)
    {
        if (!TryGetCurrentUserId(out var userId))
        {
            return Unauthorized(ApiErrorHelper.BuildError(HttpContext, "UNAUTHORIZED", "Token user tidak valid"));
        }

        if (!TryNormalizeMode(mode, out var normalizedMode, out var modeError))
        {
            return modeError!;
        }

        var role = User.FindFirstValue(ClaimTypes.Role);
        if (!string.Equals(role, "INSTRUCTOR", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(role, "PLAYER", StringComparison.OrdinalIgnoreCase))
        {
            return StatusCode(StatusCodes.Status403Forbidden,
                ApiErrorHelper.BuildError(HttpContext, "FORBIDDEN", "Role tidak diizinkan"));
        }

        var instructorUserId = string.Equals(role, "INSTRUCTOR", StringComparison.OrdinalIgnoreCase)
            ? userId
            : (Guid?)null;
        var ruleset = await _state.GetRulesetSectionAsync(normalizedMode, rulesetId, instructorUserId, ct);
        if (ruleset is null)
        {
            return NotFound(ApiErrorHelper.BuildError(HttpContext, "NOT_FOUND", "Ruleset tidak ditemukan"));
        }

        var catalog = RulesetSectionCatalog.FromConfig(ruleset.ConfigJson);
        return Ok(new RulesetSectionsResponse(
            ruleset.RulesetId,
            ruleset.RulesetVersionId,
            ruleset.Mode,
            catalog.GameConfig,
            catalog.Bahan,
            catalog.Resep,
            catalog.Kebutuhan,
            catalog.TargetKebutuhan,
            catalog.TujuanFinansial,
            catalog.Narasi,
            catalog.Quest));
    }


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
        int nextVersion;
        try
        {
            nextVersion = await _rulesets.CreateRulesetVersionAsync(
                rulesetId,
                request.Name,
                request.Description,
                configJson,
                GetActorName(),
                ct);
        }
        catch (PostgresException ex) when (
            ex.SqlState == PostgresErrorCodes.UniqueViolation &&
            string.Equals(ex.ConstraintName, "ruleset_versions_ruleset_id_config_hash_key", StringComparison.Ordinal))
        {
            return Conflict(ApiErrorHelper.BuildError(
                HttpContext,
                "DUPLICATE",
                "Konfigurasi ruleset tersebut sudah pernah dibuat sebagai versi ruleset ini"));
        }

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

    [HttpDelete("{rulesetId:guid}/versions/{version:int}")]
    [Authorize(Roles = "INSTRUCTOR")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteRulesetVersion(Guid rulesetId, int version, CancellationToken ct)
    {
        if (!TryGetCurrentUserId(out var instructorUserId))
        {
            return Unauthorized(ApiErrorHelper.BuildError(HttpContext, "UNAUTHORIZED", "Token user tidak valid"));
        }

        if (version < 1)
        {
            return BadRequest(ApiErrorHelper.BuildError(
                HttpContext,
                "VALIDATION_ERROR",
                "Path version tidak valid",
                new ErrorDetail("version", "OUT_OF_RANGE")));
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

        var totalVersions = await _rulesets.CountRulesetVersionsAsync(rulesetId, ct);
        if (totalVersions <= 1)
        {
            return UnprocessableEntity(ApiErrorHelper.BuildError(
                HttpContext,
                "DOMAIN_RULE_VIOLATION",
                "Versi terakhir tidak dapat dihapus. Hapus ruleset jika tidak lagi diperlukan."));
        }

        if (string.Equals(selectedVersion.Status, "ACTIVE", StringComparison.OrdinalIgnoreCase))
        {
            return UnprocessableEntity(ApiErrorHelper.BuildError(
                HttpContext,
                "DOMAIN_RULE_VIOLATION",
                "Versi aktif tidak dapat dihapus. Aktifkan versi lain terlebih dahulu."));
        }

        var isUsed = await _rulesets.IsRulesetVersionUsedAsync(selectedVersion.RulesetVersionId, ct);
        if (isUsed)
        {
            return UnprocessableEntity(ApiErrorHelper.BuildError(
                HttpContext,
                "DOMAIN_RULE_VIOLATION",
                "Versi ruleset sudah dipakai pada sesi/event sehingga tidak dapat dihapus."));
        }

        var deleted = await _rulesets.DeleteRulesetVersionAsync(rulesetId, version, ct);
        if (!deleted)
        {
            return NotFound(ApiErrorHelper.BuildError(HttpContext, "NOT_FOUND", "Ruleset version tidak ditemukan"));
        }

        return NoContent();
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
            var playerUserId = await _users.GetPlayerUserIdAsync(userId, ct);
            if (!playerUserId.HasValue)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    ApiErrorHelper.BuildError(HttpContext, "FORBIDDEN", "Akun PLAYER belum terhubung ke profil pemain"));
            }

            items = await _rulesets.ListRulesetsByPlayerAsync(playerUserId.Value, ct);
        }
        else
        {
            return StatusCode(StatusCodes.Status403Forbidden,
                ApiErrorHelper.BuildError(HttpContext, "FORBIDDEN", "Role tidak diizinkan"));
        }

        return Ok(new RulesetListResponse(items));
    }

    [HttpGet("components/defaults")]
    [HttpGet("/api/v1/game-components")]
    [ProducesResponseType(typeof(DefaultRulesetComponentsResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListDefaultRulesetComponents([FromQuery] string? mode, CancellationToken ct)
    {
        if (!TryGetCurrentUserId(out _))
        {
            return Unauthorized(ApiErrorHelper.BuildError(HttpContext, "UNAUTHORIZED", "Token user tidak valid"));
        }

        var role = User.FindFirstValue(ClaimTypes.Role);
        var isInstructor = string.Equals(role, "INSTRUCTOR", StringComparison.OrdinalIgnoreCase);
        var isPlayer = string.Equals(role, "PLAYER", StringComparison.OrdinalIgnoreCase);
        if (!isInstructor && !isPlayer)
        {
            return StatusCode(StatusCodes.Status403Forbidden,
                ApiErrorHelper.BuildError(HttpContext, "FORBIDDEN", "Role tidak diizinkan"));
        }

        string? modeFilter = null;
        if (!string.IsNullOrWhiteSpace(mode))
        {
            modeFilter = mode.Trim().ToUpperInvariant();
            if (!string.Equals(modeFilter, "PEMULA", StringComparison.Ordinal) &&
                !string.Equals(modeFilter, "MAHIR", StringComparison.Ordinal))
            {
                return BadRequest(ApiErrorHelper.BuildError(
                    HttpContext,
                    "VALIDATION_ERROR",
                    "Query mode tidak valid",
                    new ErrorDetail("mode", "INVALID_VALUE")));
            }
        }

        var defaults = await _rulesets.ListDefaultRulesetComponentsAsync(ct);
        var items = new List<DefaultRulesetComponentItem>(defaults.Count);

        foreach (var row in defaults)
        {
            var compatibleConfigJson = await ResolveCompatibleConfigJsonAsync(row.RulesetId, null, row.Mode, row.ConfigJson, ct);
            if (string.IsNullOrWhiteSpace(compatibleConfigJson))
            {
                continue;
            }

            using var doc = JsonDocument.Parse(compatibleConfigJson);
            var root = doc.RootElement;

            var itemMode = row.Mode;
            if (root.TryGetProperty("mode", out var modeProp) && modeProp.ValueKind == JsonValueKind.String)
            {
                itemMode = modeProp.GetString();
            }

            if (!string.IsNullOrWhiteSpace(modeFilter) &&
                !string.Equals(itemMode, modeFilter, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            JsonElement? componentCatalog = null;
            if (root.TryGetProperty("component_catalog", out var componentCatalogProp))
            {
                componentCatalog = componentCatalogProp.Clone();
            }

            items.Add(new DefaultRulesetComponentItem(
                row.RulesetId,
                row.Name,
                row.Description,
                row.RulesetVersionId,
                row.Version,
                itemMode,
                componentCatalog,
                TryBuildSections(compatibleConfigJson)));
        }

        return Ok(new DefaultRulesetComponentsResponse(items));
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
            if (ruleset is null)
            {
                ruleset = await _rulesets.GetDefaultSeedRulesetAsync(rulesetId, ct);
            }
        }
        else if (string.Equals(role, "PLAYER", StringComparison.OrdinalIgnoreCase))
        {
            var playerUserId = await _users.GetPlayerUserIdAsync(userId, ct);
            if (!playerUserId.HasValue)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    ApiErrorHelper.BuildError(HttpContext, "FORBIDDEN", "Akun PLAYER belum terhubung ke profil pemain"));
            }

            ruleset = await _rulesets.GetRulesetForPlayerAsync(rulesetId, playerUserId.Value, ct);
            if (ruleset is null)
            {
                ruleset = await _rulesets.GetDefaultSeedRulesetAsync(rulesetId, ct);
            }
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
        Guid? selectedRulesetVersionId = null;
        int? selectedVersionNumber = null;
        string? selectedMode = null;
        RulesetSectionCatalogResponse? selectedSections = null;
        var latest = versions.FirstOrDefault();
        if (latest is not null)
        {
            var compatibleConfigJson = await ResolveCompatibleConfigJsonAsync(
                rulesetId,
                string.Equals(role, "INSTRUCTOR", StringComparison.OrdinalIgnoreCase) ? userId : null,
                latest.Mode,
                latest.ConfigJson,
                ct);
            if (!string.IsNullOrWhiteSpace(compatibleConfigJson))
            {
                using var doc = JsonDocument.Parse(compatibleConfigJson);
                configJson = doc.RootElement.Clone();
                selectedRulesetVersionId = latest.RulesetVersionId;
                selectedVersionNumber = latest.Version;
                selectedMode = latest.Mode;
                if (doc.RootElement.TryGetProperty("mode", out var modeProp) && modeProp.ValueKind == JsonValueKind.String)
                {
                    selectedMode = modeProp.GetString();
                }

                selectedSections = TryBuildSections(compatibleConfigJson);
            }
        }

        var response = new RulesetDetailResponse(
            ruleset.RulesetId,
            ruleset.Name,
            ruleset.Description,
            versionItems,
            configJson,
            selectedRulesetVersionId,
            selectedVersionNumber,
            selectedMode,
            selectedSections);

        return Ok(response);
    }

    [HttpGet("{rulesetId:guid}/components")]
    [ProducesResponseType(typeof(RulesetComponentsResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRulesetComponents(Guid rulesetId, [FromQuery] int? version, CancellationToken ct)
    {
        if (!TryGetCurrentUserId(out var userId))
        {
            return Unauthorized(ApiErrorHelper.BuildError(HttpContext, "UNAUTHORIZED", "Token user tidak valid"));
        }

        if (version.HasValue && version.Value < 1)
        {
            return BadRequest(ApiErrorHelper.BuildError(
                HttpContext,
                "VALIDATION_ERROR",
                "Query version tidak valid",
                new ErrorDetail("version", "OUT_OF_RANGE")));
        }

        var role = User.FindFirstValue(ClaimTypes.Role);
        RulesetDb? ruleset;
        if (string.Equals(role, "INSTRUCTOR", StringComparison.OrdinalIgnoreCase))
        {
            ruleset = await _rulesets.GetRulesetForInstructorAsync(rulesetId, userId, ct);
            if (ruleset is null)
            {
                ruleset = await _rulesets.GetDefaultSeedRulesetAsync(rulesetId, ct);
            }
        }
        else if (string.Equals(role, "PLAYER", StringComparison.OrdinalIgnoreCase))
        {
            var playerUserId = await _users.GetPlayerUserIdAsync(userId, ct);
            if (!playerUserId.HasValue)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    ApiErrorHelper.BuildError(HttpContext, "FORBIDDEN", "Akun PLAYER belum terhubung ke profil pemain"));
            }

            ruleset = await _rulesets.GetRulesetForPlayerAsync(rulesetId, playerUserId.Value, ct);
            if (ruleset is null)
            {
                ruleset = await _rulesets.GetDefaultSeedRulesetAsync(rulesetId, ct);
            }
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

        RulesetVersionDb? selectedVersion;
        if (version.HasValue)
        {
            selectedVersion = await _rulesets.GetRulesetVersionAsync(rulesetId, version.Value, ct);
        }
        else
        {
            selectedVersion = await _rulesets.GetLatestActiveVersionAsync(rulesetId, ct)
                ?? await _rulesets.GetLatestVersionAsync(rulesetId, ct);
        }

        if (selectedVersion is null)
        {
            return NotFound(ApiErrorHelper.BuildError(HttpContext, "NOT_FOUND", "Ruleset version tidak ditemukan"));
        }

        var compatibleConfigJson = await ResolveCompatibleConfigJsonAsync(
            rulesetId,
            string.Equals(role, "INSTRUCTOR", StringComparison.OrdinalIgnoreCase) ? userId : null,
            selectedVersion.Mode,
            selectedVersion.ConfigJson,
            ct);
        if (string.IsNullOrWhiteSpace(compatibleConfigJson))
        {
            return NotFound(ApiErrorHelper.BuildError(HttpContext, "NOT_FOUND", "Konfigurasi ruleset tidak ditemukan"));
        }

        using var doc = JsonDocument.Parse(compatibleConfigJson);
        var root = doc.RootElement;
        var mode = selectedVersion.Mode;
        if (root.TryGetProperty("mode", out var modeProp) && modeProp.ValueKind == JsonValueKind.String)
        {
            mode = modeProp.GetString();
        }

        JsonElement? componentCatalog = null;
        if (root.TryGetProperty("component_catalog", out var componentsProp))
        {
            componentCatalog = componentsProp.Clone();
        }

        var sections = TryBuildSections(compatibleConfigJson);

        return Ok(new RulesetComponentsResponse(
            rulesetId,
            selectedVersion.RulesetVersionId,
            selectedVersion.Version,
            mode,
            componentCatalog,
            sections));
    }

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

    private async Task<string?> ResolveCompatibleConfigJsonAsync(
        Guid rulesetId,
        Guid? instructorUserId,
        string? mode,
        string? rawConfigJson,
        CancellationToken ct)
    {
        if (!string.IsNullOrWhiteSpace(rawConfigJson) &&
            TryBuildSections(rawConfigJson) is not null)
        {
            return rawConfigJson;
        }

        var normalizedMode = string.IsNullOrWhiteSpace(mode)
            ? "MAHIR"
            : mode.Trim().ToUpperInvariant();
        var section = await _state.GetRulesetSectionAsync(normalizedMode, rulesetId, instructorUserId, ct);
        return section?.ConfigJson;
    }

    private static RulesetSectionCatalogResponse? TryBuildSections(string configJson)
    {
        if (string.IsNullOrWhiteSpace(configJson))
        {
            return null;
        }

        using var document = JsonDocument.Parse(configJson);
        if (!document.RootElement.TryGetProperty("component_catalog", out var componentCatalog) ||
            componentCatalog.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        var requiredKeys = new[]
        {
            "gameConfig",
            "bahan",
            "resep",
            "kebutuhan",
            "targetKebutuhan",
            "tujuanFinansial",
            "narasi",
            "quest"
        };

        foreach (var key in requiredKeys)
        {
            if (!componentCatalog.TryGetProperty(key, out _))
            {
                return null;
            }
        }

        var catalog = RulesetSectionCatalog.FromConfig(configJson);
        return new RulesetSectionCatalogResponse(
            catalog.GameConfig,
            catalog.Bahan,
            catalog.Resep,
            catalog.Kebutuhan,
            catalog.TargetKebutuhan,
            catalog.TujuanFinansial,
            catalog.Narasi,
            catalog.Quest);
    }

    private IActionResult BadRequestError(string field, string issue, string message)
    {
        return BadRequest(ApiErrorHelper.BuildError(HttpContext, "VALIDATION_ERROR", message, new ErrorDetail(field, issue)));
    }

    private bool TryNormalizeMode(string? rawMode, out string mode, out IActionResult? error)
    {
        mode = string.IsNullOrWhiteSpace(rawMode) ? "MAHIR" : rawMode.Trim().ToUpperInvariant();
        error = null;
        if (mode is "PEMULA" or "MAHIR")
        {
            return true;
        }

        error = BadRequestError("mode", "INVALID_ENUM", "Mode tidak valid");
        return false;
    }

}
