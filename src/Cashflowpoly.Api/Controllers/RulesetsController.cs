// Fungsi file: Menangani endpoint, validasi akses, dan response HTTP untuk RulesetsController.
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

        if (ruleset.Definition is null)
        {
            return NotFound(ApiErrorHelper.BuildError(HttpContext, "NOT_FOUND", "Ruleset tidak memiliki definisi relasional yang lengkap"));
        }

        var catalog = RulesetSectionCatalog.FromDefinition(ruleset.Definition);
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
            catalog.Narasi));
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

        var prepared = await PrepareDefinitionForWriteAsync(request.Definition, ct);
        if (prepared.Error is not null)
        {
            return prepared.Error;
        }

        var created = await _rulesets.CreateRulesetAsync(
            request.Name,
            request.Description,
            instructorUserId,
            prepared.Definition!,
            instructorUserId,
            ct);

        return Created(
            $"/api/v1/rulesets/{created.RulesetId}",
            new CreateRulesetResponse(created.RulesetId, created.RulesetVersionId, created.Version));
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

        var mutableRuleset = await GetMutableInstructorRulesetAsync(rulesetId, instructorUserId, ct);
        if (mutableRuleset.Error is not null)
        {
            return mutableRuleset.Error;
        }

        var prepared = await PrepareDefinitionForWriteAsync(request.Definition, ct);
        if (prepared.Error is not null)
        {
            return prepared.Error;
        }

        (Guid RulesetVersionId, int Version) createdVersion;
        try
        {
            createdVersion = await _rulesets.CreateRulesetVersionAsync(
                rulesetId,
                request.Name,
                request.Description,
                prepared.Definition!,
                instructorUserId,
                ct);
        }
        catch (PostgresException ex) when (
            ex.SqlState == PostgresErrorCodes.UniqueViolation &&
            IsRulesetConfigHashUniqueViolation(ex))
        {
            return Conflict(ApiErrorHelper.BuildError(
                HttpContext,
                "DUPLICATE",
                "Konfigurasi ruleset tersebut sudah pernah dibuat sebagai versi ruleset ini"));
        }

        return Ok(new CreateRulesetResponse(rulesetId, createdVersion.RulesetVersionId, createdVersion.Version));
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

        var mutableRuleset = await GetMutableInstructorRulesetAsync(rulesetId, instructorUserId, ct);
        if (mutableRuleset.Error is not null)
        {
            return mutableRuleset.Error;
        }

        var selectedVersion = await _rulesets.GetRulesetVersionAsync(rulesetId, version, ct);
        if (selectedVersion is null)
        {
            return NotFound(ApiErrorHelper.BuildError(HttpContext, "NOT_FOUND", "Ruleset version tidak ditemukan"));
        }

        List<ErrorDetail> configErrors;
        if (selectedVersion.Definition is null)
        {
            configErrors = [new ErrorDetail("definition", "REQUIRED")];
        }
        else if (!RulesetRuntimeMapper.TryBuildConfig(selectedVersion.Definition, out _, out configErrors))
        {
            return BadRequest(ApiErrorHelper.BuildError(
                HttpContext,
                "VALIDATION_ERROR",
                "Definition ruleset tidak valid",
                configErrors.ToArray()));
        }

        await _rulesets.ActivateRulesetVersionAsync(rulesetId, version, ct);
        return Ok(new CreateRulesetResponse(rulesetId, selectedVersion.RulesetVersionId, version));
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

        var mutableRuleset = await GetMutableInstructorRulesetAsync(rulesetId, instructorUserId, ct);
        if (mutableRuleset.Error is not null)
        {
            return mutableRuleset.Error;
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

        var defaults = await _rulesets.ListDefaultRulesetsAsync(ct);
        items = defaults
            .Concat(items)
            .GroupBy(item => item.RulesetId)
            .Select(group => group.First())
            .ToList();

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
            if (row.Definition is null)
            {
                continue;
            }

            var itemMode = string.IsNullOrWhiteSpace(row.Definition.Mode)
                ? row.Mode
                : row.Definition.Mode;

            if (!string.IsNullOrWhiteSpace(modeFilter) &&
                !string.Equals(itemMode, modeFilter, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            items.Add(new DefaultRulesetComponentItem(
                row.RulesetId,
                row.Name,
                row.Description,
                row.RulesetVersionId,
                row.Version,
                itemMode,
                CloneDefinition(
                    row.Definition,
                    row.Definition.Actions)));
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

        Guid? selectedRulesetVersionId = null;
        int? selectedVersionNumber = null;
        string? selectedMode = null;
        RulesetDefinitionDto? selectedDefinition = null;
        var latest = versions.FirstOrDefault();
        if (latest?.Definition is not null)
        {
            selectedRulesetVersionId = latest.RulesetVersionId;
            selectedVersionNumber = latest.Version;
            selectedMode = latest.Mode;
            selectedDefinition = CloneDefinition(
                latest.Definition,
                latest.Definition.Actions);
        }

        var isDefault = ruleset.InstructorUserId is null;
        var isLockedBySession = await _rulesets.IsRulesetLockedBySessionAsync(rulesetId, ct);
        var response = new RulesetDetailResponse(
            ruleset.RulesetId,
            ruleset.Name,
            ruleset.Description,
            versionItems,
            selectedRulesetVersionId,
            selectedVersionNumber,
            selectedMode,
            selectedDefinition,
            isDefault,
            isLockedBySession);

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

        if (selectedVersion.Definition is null)
        {
            return NotFound(ApiErrorHelper.BuildError(HttpContext, "NOT_FOUND", "Definition ruleset tidak ditemukan"));
        }

        var definition = CloneDefinition(
            selectedVersion.Definition,
            selectedVersion.Definition.Actions);

        return Ok(new RulesetComponentsResponse(
            rulesetId,
            selectedVersion.RulesetVersionId,
            selectedVersion.Version,
            selectedVersion.Mode,
            definition));
    }

    private static bool IsRulesetConfigHashUniqueViolation(PostgresException ex)
    {
        return string.Equals(ex.ConstraintName, "uq_ruleset_versions_ruleset_config_hash", StringComparison.Ordinal) ||
               string.Equals(ex.ConstraintName, "ruleset_versions_ruleset_id_config_hash_key", StringComparison.Ordinal);
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

        var mutableRuleset = await GetMutableInstructorRulesetAsync(rulesetId, instructorUserId, ct);
        if (mutableRuleset.Error is not null)
        {
            return mutableRuleset.Error;
        }

        await _rulesets.DeleteRulesetAsync(rulesetId, ct);
        return NoContent();
    }

    private async Task<(RulesetDb? Ruleset, IActionResult? Error)> GetMutableInstructorRulesetAsync(
        Guid rulesetId,
        Guid instructorUserId,
        CancellationToken ct)
    {
        var ruleset = await _rulesets.GetRulesetAsync(rulesetId, ct);
        if (ruleset is null)
        {
            return (null, NotFound(ApiErrorHelper.BuildError(HttpContext, "NOT_FOUND", "Ruleset tidak ditemukan")));
        }

        if (ruleset.InstructorUserId is null)
        {
            return (null, UnprocessableEntity(ApiErrorHelper.BuildError(
                HttpContext,
                "DOMAIN_RULE_VIOLATION",
                "Ruleset default sistem hanya dapat dilihat atau dijadikan dasar membuat ruleset baru.")));
        }

        if (ruleset.InstructorUserId.Value != instructorUserId)
        {
            return (null, NotFound(ApiErrorHelper.BuildError(HttpContext, "NOT_FOUND", "Ruleset tidak ditemukan")));
        }

        var lockedBySession = await _rulesets.IsRulesetLockedBySessionAsync(rulesetId, ct);
        if (lockedBySession)
        {
            return (null, UnprocessableEntity(ApiErrorHelper.BuildError(
                HttpContext,
                "DOMAIN_RULE_VIOLATION",
                "Ruleset sudah dipakai pada sesi yang berjalan atau selesai sehingga hanya dapat dilihat.")));
        }

        return (ruleset, null);
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

    private async Task<(RulesetDefinitionDto? Definition, IActionResult? Error)> PrepareDefinitionForWriteAsync(
        RulesetDefinitionDto? explicitDefinition,
        CancellationToken ct)
    {
        if (explicitDefinition is null)
        {
            return (
                null,
                BadRequest(ApiErrorHelper.BuildError(
                    HttpContext,
                    "VALIDATION_ERROR",
                    "Definition ruleset wajib ada",
                    new ErrorDetail("definition", "REQUIRED"))));
        }

        var definition = explicitDefinition;

        if (!RulesetRuntimeMapper.TryBuildConfig(definition, out _, out var configErrors))
        {
            return (
                null,
                BadRequest(ApiErrorHelper.BuildError(
                    HttpContext,
                    "VALIDATION_ERROR",
                    "Definition ruleset tidak valid",
                    configErrors.ToArray())));
        }

        return (definition, null);
    }

    private static RulesetDefinitionDto CloneDefinition(
        RulesetDefinitionDto source,
        IReadOnlyCollection<RulesetActionDto> actions)
    {
        return new RulesetDefinitionDto
        {
            Mode = source.Mode,
            Settings = source.Settings,
            PlayerOrdering = source.PlayerOrdering,
            Actions = actions.ToList(),
            Ingredients = source.Ingredients,
            Orders = source.Orders,
            Needs = source.Needs,
            NeedSetBonuses = source.NeedSetBonuses,
            CollectionMissions = source.CollectionMissions,
            FinancialGoals = source.FinancialGoals,
            Narratives = source.Narratives,
            DonationRankPoints = source.DonationRankPoints,
            GoldPointsByQty = source.GoldPointsByQty,
            GoldPrices = source.GoldPrices,
            PensionRankPoints = source.PensionRankPoints,
            TieBreakers = source.TieBreakers,
            ShariaLoans = source.ShariaLoans,
            InsuranceProducts = source.InsuranceProducts,
            LifeRisks = source.LifeRisks
        };
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
