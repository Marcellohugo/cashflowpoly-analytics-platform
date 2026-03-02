// Fungsi file: Menyediakan endpoint manajemen pemain (buat, daftar) dan penugasan pemain ke sesi permainan.
using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Domain;
using Cashflowpoly.Api.Models;
using Cashflowpoly.Api.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace Cashflowpoly.Api.Controllers;

/// <summary>
/// Controller untuk manajemen pemain dan relasi pemain ke sesi.
/// </summary>
[ApiController]
[Route("api/v1/players")]
[Authorize]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status429TooManyRequests)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
/// <summary>
/// Controller manajemen pemain dan relasi pemain-sesi dengan dukungan instructor order dari ruleset.
/// </summary>
public sealed class PlayersController : ControllerBase
{
    private readonly PlayerRepository _players;
    private readonly SessionRepository _sessions;
    private readonly RulesetRepository _rulesets;
    private readonly UserRepository _users;

    /// <summary>
    /// Menginisialisasi controller dengan dependensi repositori pemain, sesi, ruleset, dan user.
    /// </summary>
    public PlayersController(PlayerRepository players, SessionRepository sessions, RulesetRepository rulesets, UserRepository users)
    {
        _players = players;
        _sessions = sessions;
        _rulesets = rulesets;
        _users = users;
    }

    [HttpPost]
    [Authorize(Roles = "INSTRUCTOR")]
    [ProducesResponseType(typeof(PlayerResponse), StatusCodes.Status201Created)]
    /// <summary>
    /// Membuat profil pemain baru beserta akun user PLAYER berdasarkan data yang diberikan instruktur.
    /// </summary>
    /// <param name="request">Data pemain baru: username, password, display name.</param>
    /// <param name="ct">Token pembatalan.</param>
    /// <returns>201 Created dengan ID dan display name pemain baru.</returns>
    public async Task<IActionResult> CreatePlayer([FromBody] CreatePlayerRequest request, CancellationToken ct)
    {
        if (!TryGetCurrentUserId(out var instructorUserId))
        {
            return Unauthorized(ApiErrorHelper.BuildError(HttpContext, "UNAUTHORIZED", "Token user tidak valid"));
        }

        if (string.IsNullOrWhiteSpace(request.DisplayName))
        {
            return BadRequest(ApiErrorHelper.BuildError(HttpContext, "VALIDATION_ERROR", "Nama pemain wajib diisi",
                new ErrorDetail("display_name", "REQUIRED")));
        }

        if (string.IsNullOrWhiteSpace(request.Username))
        {
            return BadRequest(ApiErrorHelper.BuildError(HttpContext, "VALIDATION_ERROR", "Username wajib diisi",
                new ErrorDetail("username", "REQUIRED")));
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(ApiErrorHelper.BuildError(HttpContext, "VALIDATION_ERROR", "Password wajib diisi",
                new ErrorDetail("password", "REQUIRED")));
        }

        var username = request.Username.Trim();
        if (username.Length < 3 || username.Length > 80)
        {
            return BadRequest(ApiErrorHelper.BuildError(HttpContext, "VALIDATION_ERROR", "Username harus 3-80 karakter",
                new ErrorDetail("username", "OUT_OF_RANGE")));
        }

        if (request.Password.Length < PasswordPolicy.MinPasswordLength)
        {
            return BadRequest(ApiErrorHelper.BuildError(
                HttpContext,
                "VALIDATION_ERROR",
                $"Password minimal {PasswordPolicy.MinPasswordLength} karakter",
                new ErrorDetail("password", "OUT_OF_RANGE")));
        }

        var exists = await _users.UsernameExistsAsync(username, ct);
        if (exists)
        {
            return Conflict(ApiErrorHelper.BuildError(HttpContext, "DUPLICATE", "Username sudah digunakan"));
        }

        var createdUser = await _users.CreateUserAsync(username, request.Password, "PLAYER", request.DisplayName.Trim(), ct);
        var updated = await _players.UpdatePlayerProfileAsync(createdUser.UserId, request.DisplayName.Trim(), instructorUserId, ct);
        if (!updated)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiErrorHelper.BuildError(HttpContext, "INTERNAL_ERROR", "Gagal sinkronisasi profil player"));
        }

        return Created($"/api/v1/players/{createdUser.UserId}", new PlayerResponse(createdUser.UserId, request.DisplayName.Trim()));
    }

    [HttpGet]
    [ProducesResponseType(typeof(PlayerListResponse), StatusCodes.Status200OK)]
    /// <summary>
    /// Menampilkan daftar pemain milik instruktur atau pemain yang terkait dengan akun PLAYER yang sedang login.
    /// </summary>
    /// <param name="ct">Token pembatalan.</param>
    /// <returns>200 OK dengan daftar pemain.</returns>
    public async Task<IActionResult> ListPlayers(CancellationToken ct)
    {
        if (!TryGetCurrentUserId(out var userId))
        {
            return Unauthorized(ApiErrorHelper.BuildError(HttpContext, "UNAUTHORIZED", "Token user tidak valid"));
        }

        var role = User.FindFirstValue(ClaimTypes.Role);
        List<PlayerDb> players;

        if (string.Equals(role, "INSTRUCTOR", StringComparison.OrdinalIgnoreCase))
        {
            players = await _players.ListPlayersAsync(userId, ct);
        }
        else if (string.Equals(role, "PLAYER", StringComparison.OrdinalIgnoreCase))
        {
            var linkedPlayerId = await _users.GetLinkedPlayerIdAsync(userId, ct);
            if (!linkedPlayerId.HasValue)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    ApiErrorHelper.BuildError(HttpContext, "FORBIDDEN", "Akun PLAYER belum terhubung ke profil pemain"));
            }

            players = await _players.ListPlayersByPlayerScopeAsync(linkedPlayerId.Value, ct);
        }
        else
        {
            return StatusCode(StatusCodes.Status403Forbidden,
                ApiErrorHelper.BuildError(HttpContext, "FORBIDDEN", "Role tidak diizinkan"));
        }

        var items = players.Select(p => new PlayerResponse(p.PlayerId, p.DisplayName)).ToList();
        return Ok(new PlayerListResponse(items));
    }

    [HttpPost("/api/v1/sessions/{sessionId:guid}/players")]
    [Authorize(Roles = "INSTRUCTOR")]
    [ProducesResponseType(typeof(AddSessionPlayerResponse), StatusCodes.Status200OK)]
    /// <summary>
    /// Menambahkan pemain ke sesi permainan dengan penentuan join order otomatis atau manual berdasarkan ruleset.
    /// </summary>
    /// <param name="sessionId">ID sesi target.</param>
    /// <param name="request">Data penugasan berisi player ID/username, role, dan join order opsional.</param>
    /// <param name="ct">Token pembatalan.</param>
    /// <returns>200 OK dengan player ID dan join order yang ditetapkan.</returns>
    public async Task<IActionResult> AddPlayerToSession(Guid sessionId, [FromBody] AddSessionPlayerRequest request, CancellationToken ct)
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

        if (!request.PlayerId.HasValue && string.IsNullOrWhiteSpace(request.Username))
        {
            return BadRequest(ApiErrorHelper.BuildError(
                HttpContext,
                "VALIDATION_ERROR",
                "Player ID atau username wajib diisi",
                new ErrorDetail("player_id", "REQUIRED"),
                new ErrorDetail("username", "REQUIRED")));
        }

        if (request.JoinOrder is <= 0)
        {
            return BadRequest(ApiErrorHelper.BuildError(
                HttpContext,
                "VALIDATION_ERROR",
                "Join order minimal 1",
                new ErrorDetail("join_order", "OUT_OF_RANGE")));
        }

        if (request.JoinOrder is > SessionRules.MaxPlayersPerSession)
        {
            return UnprocessableEntity(ApiErrorHelper.BuildError(
                HttpContext,
                "DOMAIN_RULE_VIOLATION",
                $"Join order maksimal {SessionRules.MaxPlayersPerSession}"));
        }

        var activeRulesetVersionId = await _sessions.GetActiveRulesetVersionIdAsync(sessionId, ct);
        var requiresInstructorOrder = false;
        var instructorOrderLookup = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        if (activeRulesetVersionId.HasValue)
        {
            var activeRulesetVersion = await _rulesets.GetRulesetVersionByIdAsync(activeRulesetVersionId.Value, ct);
            if (activeRulesetVersion is not null &&
                RulesetConfigParser.TryParse(activeRulesetVersion.ConfigJson, out var activeConfig, out _) &&
                activeConfig?.PlayerOrdering == PlayerOrdering.InstructorOrder)
            {
                requiresInstructorOrder = true;
                instructorOrderLookup = BuildInstructorOrderLookup(activeRulesetVersion.ConfigJson);
            }
        }

        PlayerDb? player = null;
        if (request.PlayerId.HasValue)
        {
            player = await _players.GetPlayerForInstructorAsync(request.PlayerId.Value, instructorUserId, ct);
        }
        else if (!string.IsNullOrWhiteSpace(request.Username))
        {
            player = await _players.GetPlayerForInstructorByUsernameAsync(request.Username.Trim(), instructorUserId, ct);
        }

        if (player is null)
        {
            return NotFound(ApiErrorHelper.BuildError(HttpContext, "NOT_FOUND", "Player tidak ditemukan"));
        }

        var playerId = player.PlayerId;
        var resolvedJoinOrder = request.JoinOrder;
        if (requiresInstructorOrder && !resolvedJoinOrder.HasValue)
        {
            var requestedUsername = string.IsNullOrWhiteSpace(request.Username)
                ? null
                : request.Username.Trim();
            if (string.IsNullOrWhiteSpace(requestedUsername))
            {
                var usernameMap = await _users.GetUsernamesByPlayerIdsAsync(new[] { playerId }, ct);
                requestedUsername = usernameMap.TryGetValue(playerId, out var mappedUsername)
                    ? mappedUsername
                    : null;
            }

            if (!string.IsNullOrWhiteSpace(requestedUsername) &&
                instructorOrderLookup.TryGetValue(requestedUsername.Trim(), out var inferredJoinOrder))
            {
                resolvedJoinOrder = inferredJoinOrder;
            }
        }

        if (requiresInstructorOrder && !resolvedJoinOrder.HasValue)
        {
            return UnprocessableEntity(ApiErrorHelper.BuildError(
                HttpContext,
                "DOMAIN_RULE_VIOLATION",
                "Ruleset mewajibkan join_order atau username yang terdaftar pada slot Player 1-4"));
        }

        var alreadyInSession = await _players.IsPlayerInSessionAsync(sessionId, playerId, ct);
        if (!alreadyInSession)
        {
            var playersInSession = await _players.CountPlayersInSessionAsync(sessionId, ct);
            if (playersInSession >= SessionRules.MaxPlayersPerSession)
            {
                return UnprocessableEntity(ApiErrorHelper.BuildError(
                    HttpContext,
                    "DOMAIN_RULE_VIOLATION",
                    $"Sesi maksimal {SessionRules.MaxPlayersPerSession} pemain"));
            }
        }

        var role = string.IsNullOrWhiteSpace(request.Role)
            ? "PLAYER"
            : request.Role.Trim().ToUpperInvariant();
        if (!string.Equals(role, "PLAYER", StringComparison.Ordinal))
        {
            return BadRequest(ApiErrorHelper.BuildError(
                HttpContext,
                "VALIDATION_ERROR",
                "Role tidak valid",
                new ErrorDetail("role", "INVALID_ENUM")));
        }

        var joinOrder = await _players.AddPlayerToSessionAndAssignJoinOrderAsync(
            sessionId,
            playerId,
            role,
            resolvedJoinOrder,
            ct);

        return Ok(new AddSessionPlayerResponse(playerId, joinOrder));
    }

    /// <summary>
    /// Mencoba mengekstrak user ID dari claim JWT NameIdentifier.
    /// </summary>
    private bool TryGetCurrentUserId(out Guid userId)
    {
        var userIdRaw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userIdRaw, out userId);
    }

    /// <summary>
    /// Membangun lookup username→join order dari konfigurasi JSON field instructor_player_usernames.
    /// </summary>
    private static Dictionary<string, int> BuildInstructorOrderLookup(string configJson)
    {
        var result = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        try
        {
            using var doc = JsonDocument.Parse(configJson);
            var root = doc.RootElement;
            if (!root.TryGetProperty("instructor_player_usernames", out var slotArray) ||
                slotArray.ValueKind != JsonValueKind.Array)
            {
                return result;
            }

            var position = 1;
            foreach (var item in slotArray.EnumerateArray())
            {
                if (position > SessionRules.MaxPlayersPerSession)
                {
                    break;
                }

                if (item.ValueKind == JsonValueKind.String)
                {
                    var username = item.GetString();
                    if (!string.IsNullOrWhiteSpace(username))
                    {
                        var normalized = username.Trim();
                        if (!result.ContainsKey(normalized))
                        {
                            result[normalized] = position;
                        }
                    }
                }

                position++;
            }
        }
        catch (JsonException)
        {
            return new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        }

        return result;
    }
}
