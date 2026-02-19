using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
public sealed class PlayersController : ControllerBase
{
    private readonly PlayerRepository _players;
    private readonly SessionRepository _sessions;
    private readonly UserRepository _users;

    public PlayersController(PlayerRepository players, SessionRepository sessions, UserRepository users)
    {
        _players = players;
        _sessions = sessions;
        _users = users;
    }

    [HttpPost]
    [Authorize(Roles = "INSTRUCTOR")]
    [ProducesResponseType(typeof(PlayerResponse), StatusCodes.Status201Created)]
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

        if (request.Password.Length < 6)
        {
            return BadRequest(ApiErrorHelper.BuildError(HttpContext, "VALIDATION_ERROR", "Password minimal 6 karakter",
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

        var player = await _players.GetPlayerForInstructorAsync(request.PlayerId, instructorUserId, ct);
        if (player is null)
        {
            return NotFound(ApiErrorHelper.BuildError(HttpContext, "NOT_FOUND", "Player tidak ditemukan"));
        }

        var role = string.IsNullOrWhiteSpace(request.Role) ? "PLAYER" : request.Role;
        var joinOrder = await _players.AddPlayerToSessionAndAssignJoinOrderByPlayerIdAsync(
            sessionId,
            request.PlayerId,
            role,
            ct);

        return Ok(new AddSessionPlayerResponse(request.PlayerId, joinOrder));
    }

    private bool TryGetCurrentUserId(out Guid userId)
    {
        var userIdRaw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userIdRaw, out userId);
    }
}
