// Fungsi file: Menangani endpoint, validasi akses, dan response HTTP untuk PlayersController.
using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Infrastructure;
using Cashflowpoly.Api.Contracts;
using Cashflowpoly.Api.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Security.Claims;

namespace Cashflowpoly.Api.Controllers;

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
public sealed class PlayersController : ControllerBase
{
    private readonly PlayerRepository _players;
    private readonly SessionRepository _sessions;
    private readonly RulesetRepository _rulesets;
    private readonly UserRepository _users;

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
    public async Task<IActionResult> CreatePlayer([FromBody] CreatePlayerRequest request, CancellationToken ct)
    {
        if (!TryGetCurrentUserId(out _))
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

        if (!PasswordPolicy.IsWithinBcryptLimit(request.Password))
        {
            return BadRequest(ApiErrorHelper.BuildError(
                HttpContext,
                "VALIDATION_ERROR",
                $"Password maksimal {PasswordPolicy.MaxPasswordUtf8Bytes} byte UTF-8",
                new ErrorDetail("password", "OUT_OF_RANGE")));
        }

        AuthenticatedUserDb createdUser;
        try
        {
            createdUser = await _users.CreatePlayerUserAsync(
                username,
                request.Password,
                request.DisplayName.Trim(),
                ct);
        }
        catch (PostgresException exception) when (exception.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            return Conflict(ApiErrorHelper.BuildError(HttpContext, "DUPLICATE", "Username sudah digunakan"));
        }

        return Created($"/api/v1/players/{createdUser.UserId}", new PlayerResponse(createdUser.UserId, createdUser.DisplayName));
    }

    [HttpGet]
    [ProducesResponseType(typeof(PlayerListResponse), StatusCodes.Status200OK)]
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
            players = await _players.ListPlayersAsync(ct);
        }
        else if (string.Equals(role, "PLAYER", StringComparison.OrdinalIgnoreCase))
        {
            var playerUserId = await _users.GetPlayerUserIdAsync(userId, ct);
            if (!playerUserId.HasValue)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    ApiErrorHelper.BuildError(HttpContext, "FORBIDDEN", "Akun PLAYER belum terhubung ke profil pemain"));
            }

            players = await _players.ListPlayersByPlayerScopeAsync(playerUserId.Value, ct);
        }
        else
        {
            return StatusCode(StatusCodes.Status403Forbidden,
                ApiErrorHelper.BuildError(HttpContext, "FORBIDDEN", "Role tidak diizinkan"));
        }

        var items = players.Select(p => new PlayerResponse(p.UserId, p.DisplayName)).ToList();
        return Ok(new PlayerListResponse(items));
    }

    [HttpGet("/api/v1/sessions/{sessionId:guid}/players")]
    [ProducesResponseType(typeof(SessionPlayerListResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListSessionPlayers(Guid sessionId, CancellationToken ct)
    {
        if (!TryGetCurrentUserId(out var userId))
        {
            return Unauthorized(ApiErrorHelper.BuildError(HttpContext, "UNAUTHORIZED", "Token user tidak valid"));
        }

        var role = User.FindFirstValue(ClaimTypes.Role);
        if (string.Equals(role, "INSTRUCTOR", StringComparison.OrdinalIgnoreCase))
        {
            if (await _sessions.GetSessionForInstructorAsync(sessionId, userId, ct) is null)
            {
                return NotFound(ApiErrorHelper.BuildError(HttpContext, "NOT_FOUND", "Session tidak ditemukan"));
            }
        }
        else if (string.Equals(role, "PLAYER", StringComparison.OrdinalIgnoreCase))
        {
            var playerUserId = await _users.GetPlayerUserIdAsync(userId, ct);
            if (!playerUserId.HasValue || !await _players.IsPlayerInSessionAsync(sessionId, playerUserId.Value, ct))
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    ApiErrorHelper.BuildError(HttpContext, "FORBIDDEN", "Pemain tidak terdaftar pada sesi ini"));
            }
        }
        else
        {
            return StatusCode(StatusCodes.Status403Forbidden,
                ApiErrorHelper.BuildError(HttpContext, "FORBIDDEN", "Role tidak diizinkan"));
        }

        var items = (await _players.ListSessionPlayersAsync(sessionId, ct))
            .Select(player => new SessionPlayerResponse(player.UserId, player.DisplayName, player.PlayerOrder))
            .ToList();
        return Ok(new SessionPlayerListResponse(items));
    }

    [HttpPost("/api/v1/sessions/{sessionId:guid}/players")]
    [Authorize(Roles = "INSTRUCTOR")]
    [ProducesResponseType(typeof(AddSessionPlayerResponse), StatusCodes.Status200OK)]
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

        if (!request.UserId.HasValue && string.IsNullOrWhiteSpace(request.Username))
        {
            return BadRequest(ApiErrorHelper.BuildError(
                HttpContext,
                "VALIDATION_ERROR",
                "User ID atau username wajib diisi",
                new ErrorDetail("user_id", "REQUIRED"),
                new ErrorDetail("username", "REQUIRED")));
        }

        if (request.PlayerOrder is <= 0)
        {
            return BadRequest(ApiErrorHelper.BuildError(
                HttpContext,
                "VALIDATION_ERROR",
                "Seat number minimal 1",
                new ErrorDetail("player_order_no", "OUT_OF_RANGE")));
        }

        var activeRulesetVersionId = await _sessions.GetActiveRulesetVersionIdAsync(sessionId, ct);
        if (!activeRulesetVersionId.HasValue)
        {
            return UnprocessableEntity(ApiErrorHelper.BuildError(
                HttpContext,
                "DOMAIN_RULE_VIOLATION",
                "Session belum memiliki ruleset aktif"));
        }

        var activeRulesetVersion = await _rulesets.GetRulesetVersionByIdAsync(activeRulesetVersionId.Value, ct);
        if (activeRulesetVersion?.Definition is null ||
            !string.Equals(activeRulesetVersion.Status, "ACTIVE", StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(activeRulesetVersion.Mode, session.Mode, StringComparison.OrdinalIgnoreCase))
        {
            return UnprocessableEntity(ApiErrorHelper.BuildError(
                HttpContext,
                "DOMAIN_RULE_VIOLATION",
                "Ruleset aktif session tidak valid"));
        }

        var maxPlayers = activeRulesetVersion.Definition.Settings.MaxPlayers;
        if (request.PlayerOrder is > 0 && request.PlayerOrder > maxPlayers)
        {
            return UnprocessableEntity(ApiErrorHelper.BuildError(
                HttpContext,
                "DOMAIN_RULE_VIOLATION",
                $"Seat number maksimal {maxPlayers}"));
        }

        PlayerDb? player = null;
        if (request.UserId.HasValue)
        {
            player = await _players.GetPlayerAsync(request.UserId.Value, ct);
        }
        else if (!string.IsNullOrWhiteSpace(request.Username))
        {
            player = await _players.GetPlayerByUsernameAsync(request.Username.Trim(), ct);
        }

        if (player is null)
        {
            return NotFound(ApiErrorHelper.BuildError(HttpContext, "NOT_FOUND", "Player tidak ditemukan"));
        }

        var userId = player.UserId;
        var alreadyInSession = await _players.IsPlayerInSessionAsync(sessionId, userId, ct);
        if (!alreadyInSession)
        {
            var playersInSession = await _players.CountPlayersInSessionAsync(sessionId, ct);
            if (playersInSession >= maxPlayers)
            {
                return UnprocessableEntity(ApiErrorHelper.BuildError(
                    HttpContext,
                    "DOMAIN_RULE_VIOLATION",
                    $"Sesi maksimal {maxPlayers} pemain"));
            }
        }

        var playerOrder = await _players.AddPlayerToSessionAndAssignPlayerOrderAsync(
            sessionId,
            userId,
            request.PlayerOrder,
            ct);

        return Ok(new AddSessionPlayerResponse(userId, playerOrder));
    }

    private bool TryGetCurrentUserId(out Guid userId)
    {
        var userIdRaw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userIdRaw, out userId);
    }

}
