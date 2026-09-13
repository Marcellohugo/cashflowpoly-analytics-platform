// Fungsi file: Menangani endpoint, validasi akses, dan response HTTP untuk RulesetsController.
// Mengimpor namespace `System.Text.Json` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.Json;
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;
// Mengimpor namespace `Cashflowpoly.Api.Domain` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Domain;
// Mengimpor namespace `Cashflowpoly.Api.Infrastructure` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Infrastructure;
// Mengimpor namespace `Cashflowpoly.Api.Contracts` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Contracts;
// Mengimpor namespace `System.Security.Claims` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Security.Claims;
// Mengimpor namespace `Microsoft.AspNetCore.Authorization` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.AspNetCore.Authorization;
// Mengimpor namespace `Microsoft.AspNetCore.Mvc` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.AspNetCore.Mvc;
// Mengimpor namespace `Npgsql` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Npgsql;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Controllers` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Controllers;

// mengaktifkan perilaku API controller, termasuk inferensi binding dan respons otomatis atas model tidak valid.
[ApiController]
// menetapkan pola rute (”api/v1/rulesets”) untuk pencocokan URL permintaan.
[Route("api/v1/rulesets")]
// mewajibkan otorisasi pengguna sesuai kebijakan autentikasi aplikasi.
[Authorize]
// menerapkan metadata `ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)` pada deklarasi berikut agar framework/compiler
// dapat mengenali pengaturannya.
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
// menerapkan metadata `ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)` pada deklarasi berikut agar
// framework/compiler dapat mengenali pengaturannya.
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
// menerapkan metadata `ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)` pada deklarasi berikut agar framework/compiler
// dapat mengenali pengaturannya.
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
// menerapkan metadata `ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)` pada deklarasi berikut agar framework/compiler
// dapat mengenali pengaturannya.
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
// menerapkan metadata `ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)` pada deklarasi berikut agar framework/compiler
// dapat mengenali pengaturannya.
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
// menerapkan metadata `ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)` pada deklarasi berikut agar
// framework/compiler dapat mengenali pengaturannya.
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
// menerapkan metadata `ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status429TooManyRequests)` pada deklarasi berikut agar
// framework/compiler dapat mengenali pengaturannya.
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status429TooManyRequests)]
// menerapkan metadata `ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)` pada deklarasi berikut agar
// framework/compiler dapat mengenali pengaturannya.
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

    // mendaftarkan action untuk metode HTTP GET pada rute (”sections”).
    [HttpGet("sections")]
    // menerapkan metadata `ProducesResponseType(typeof(RulesetSectionsResponse), StatusCodes.Status200OK)` pada deklarasi berikut agar
    // framework/compiler dapat mengenali pengaturannya.
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
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: userId dalam GetRulesetSections.
            ? userId
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: (Guid?)null; dalam GetRulesetSections.
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

    // mendaftarkan action untuk metode HTTP POST pada rute controller saat ini.
    [HttpPost]
    // mewajibkan otorisasi pengguna dengan ketentuan (Roles = ”INSTRUCTOR”).
    [Authorize(Roles = "INSTRUCTOR")]
    // menerapkan metadata `ProducesResponseType(typeof(CreateRulesetResponse), StatusCodes.Status201Created)` pada deklarasi berikut agar
    // framework/compiler dapat mengenali pengaturannya.
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

    // mendaftarkan action untuk metode HTTP PUT pada rute (”{rulesetId:guid}”).
    [HttpPut("{rulesetId:guid}")]
    // mewajibkan otorisasi pengguna dengan ketentuan (Roles = ”INSTRUCTOR”).
    [Authorize(Roles = "INSTRUCTOR")]
    // menerapkan metadata `ProducesResponseType(typeof(CreateRulesetResponse), StatusCodes.Status200OK)` pada deklarasi berikut agar framework/compiler
    // dapat mengenali pengaturannya.
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
        // Menangani exception `PostgresException` melalui variabel ex hanya jika filter `ex.SqlState == PostgresErrorCodes.UniqueViolation &&
        // IsRulesetConfigHashUniqueViolation(ex)` terpenuhi dalam UpdateRuleset.
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

    // mendaftarkan action untuk metode HTTP POST pada rute (”{rulesetId:guid}/versions/{version:int}/activate”).
    [HttpPost("{rulesetId:guid}/versions/{version:int}/activate")]
    // mewajibkan otorisasi pengguna dengan ketentuan (Roles = ”INSTRUCTOR”).
    [Authorize(Roles = "INSTRUCTOR")]
    // menerapkan metadata `ProducesResponseType(typeof(CreateRulesetResponse), StatusCodes.Status200OK)` pada deklarasi berikut agar framework/compiler
    // dapat mengenali pengaturannya.
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

        if (selectedVersion.Definition is { } activatingDefinition &&
            ValidateMechanicAvailability(activatingDefinition) is { } mechanicError)
        {
            return mechanicError;
        }
        await _rulesets.ActivateRulesetVersionAsync(rulesetId, version, ct);
        return Ok(new CreateRulesetResponse(rulesetId, selectedVersion.RulesetVersionId, version));
    }

    // mendaftarkan action untuk metode HTTP DELETE pada rute (”{rulesetId:guid}/versions/{version:int}”).
    [HttpDelete("{rulesetId:guid}/versions/{version:int}")]
    // mewajibkan otorisasi pengguna dengan ketentuan (Roles = ”INSTRUCTOR”).
    [Authorize(Roles = "INSTRUCTOR")]
    // menerapkan metadata `ProducesResponseType(StatusCodes.Status204NoContent)` pada deklarasi berikut agar framework/compiler dapat mengenali
    // pengaturannya.
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

    // mendaftarkan action untuk metode HTTP GET pada rute controller saat ini.
    [HttpGet]
    // menerapkan metadata `ProducesResponseType(typeof(RulesetListResponse), StatusCodes.Status200OK)` pada deklarasi berikut agar framework/compiler
    // dapat mengenali pengaturannya.
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

    // mendaftarkan action untuk metode HTTP GET pada rute (”components/defaults”).
    [HttpGet("components/defaults")]
    // mendaftarkan action untuk metode HTTP GET pada rute (”/api/v1/game-components”).
    [HttpGet("/api/v1/game-components")]
    // menerapkan metadata `ProducesResponseType(typeof(DefaultRulesetComponentsResponse), StatusCodes.Status200OK)` pada deklarasi berikut agar
    // framework/compiler dapat mengenali pengaturannya.
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

        // Mengulangi setiap elemen `defaults`; elemen saat ini disimpan sebagai `row` bertipe `var` untuk diproses oleh badan loop dalam
        // ListDefaultRulesetComponents.
        foreach (var row in defaults)
        {
            if (row.Definition is null)
            {
                // Melewati sisa pernyataan pada iterasi saat ini dan melanjutkan ke elemen/iterasi berikutnya dalam ListDefaultRulesetComponents.
                continue;
            }

            var itemMode = string.IsNullOrWhiteSpace(row.Definition.Mode)
                // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: row.Mode dalam ListDefaultRulesetComponents.
                ? row.Mode
                // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: row.Definition.Mode; dalam ListDefaultRulesetComponents.
                : row.Definition.Mode;

            if (!string.IsNullOrWhiteSpace(modeFilter) &&
                !string.Equals(itemMode, modeFilter, StringComparison.OrdinalIgnoreCase))
            {
                // Melewati sisa pernyataan pada iterasi saat ini dan melanjutkan ke elemen/iterasi berikutnya dalam ListDefaultRulesetComponents.
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

    // mendaftarkan action untuk metode HTTP GET pada rute (”{rulesetId:guid}”).
    [HttpGet("{rulesetId:guid}")]
    // menerapkan metadata `ProducesResponseType(typeof(RulesetDetailResponse), StatusCodes.Status200OK)` pada deklarasi berikut agar framework/compiler
    // dapat mengenali pengaturannya.
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

    // mendaftarkan action untuk metode HTTP GET pada rute (”{rulesetId:guid}/components”).
    [HttpGet("{rulesetId:guid}/components")]
    // menerapkan metadata `ProducesResponseType(typeof(RulesetComponentsResponse), StatusCodes.Status200OK)` pada deklarasi berikut agar
    // framework/compiler dapat mengenali pengaturannya.
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
                // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: await _rulesets.GetLatestVersionAsync(rulesetId, ct); dalam
                // GetRulesetComponents.
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

    // mendaftarkan action untuk metode HTTP DELETE pada rute (”{rulesetId:guid}”).
    [HttpDelete("{rulesetId:guid}")]
    // mewajibkan otorisasi pengguna dengan ketentuan (Roles = ”INSTRUCTOR”).
    [Authorize(Roles = "INSTRUCTOR")]
    // menerapkan metadata `ProducesResponseType(StatusCodes.Status204NoContent)` pada deklarasi berikut agar framework/compiler dapat mengenali
    // pengaturannya.
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
        // Parameter `rulesetId` bertipe `Guid` membawa identitas kumpulan aturan permainan.
        Guid rulesetId,
        // Parameter `instructorUserId` bertipe `Guid` membawa identitas instruktur pemilik sesi atau aturan.
        Guid instructorUserId,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
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
                "Ruleset sudah terhubung ke sesi sehingga hanya dapat dilihat, termasuk saat sesi belum dimulai. Buat ruleset baru untuk perubahan aturan.")));
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
        // Parameter `explicitDefinition` bertipe `RulesetDefinitionDto?` membawa nilai explicit definisi; nilai null diizinkan ketika data opsional belum
        // tersedia.
        RulesetDefinitionDto? explicitDefinition,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
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

        var actionIds = definition.Actions.Select(action => action.ActionId).ToHashSet(StringComparer.OrdinalIgnoreCase);
        if (definition.Narratives.SelectMany(narrative => narrative.PrerequisiteAksi)
            .Any(prerequisite => !actionIds.Contains(prerequisite.Aksi)))
        {
            return (null, BadRequest(ApiErrorHelper.BuildError(HttpContext, "VALIDATION_ERROR",
                "Prasyarat narasi harus merujuk aksi yang tersedia pada set aturan.",
                new ErrorDetail("definition.narratives.prerequisite_aksi", "UNKNOWN_ACTION"))));
        }

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

        return (definition, ValidateMechanicAvailability(definition));
    }

    private IActionResult? ValidateMechanicAvailability(RulesetDefinitionDto definition)
    {
        var settings = definition.Settings;
        var ordering = definition.PlayerOrdering;
        var advanced = string.Equals(definition.Mode, "MAHIR", StringComparison.OrdinalIgnoreCase);
        if (!ordering.FridayEnabled || !ordering.SaturdayEnabled || !ordering.SundayEnabled ||
            !settings.GoldTradeAllowBuy || !settings.GoldTradeAllowSell ||
            settings.LoanEnabled != advanced || settings.InsuranceEnabled != advanced ||
            settings.SavingGoalEnabled != advanced)
        {
            return UnprocessableEntity(ApiErrorHelper.BuildError(HttpContext, "DOMAIN_RULE_VIOLATION",
                "Pengubahan mekanik permainan belum tersedia (Coming soon). Gunakan mekanik bawaan sesuai mode."));
        }
        return null;
    }

    private static RulesetDefinitionDto CloneDefinition(
        // Parameter `source` bertipe `RulesetDefinitionDto` membawa nilai source.
        RulesetDefinitionDto source,
        // Parameter `actions` bertipe `IReadOnlyCollection<RulesetActionDto>` membawa nilai aksi.
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
