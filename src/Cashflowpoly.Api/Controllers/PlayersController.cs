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
[Route("api/players")]
[Authorize]
public sealed class PlayersController : ControllerBase
{
    private readonly PlayerRepository _players;
    private readonly SessionRepository _sessions;

    public PlayersController(PlayerRepository players, SessionRepository sessions)
    {
        _players = players;
        _sessions = sessions;
    }

    [HttpPost]
    [Authorize(Roles = "INSTRUCTOR")]
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

        var playerId = await _players.CreatePlayerAsync(request.DisplayName, instructorUserId, ct);
        return Created($"/api/v1/players/{playerId}", new PlayerResponse(playerId, request.DisplayName));
    }

    [HttpGet]
    [Authorize(Roles = "INSTRUCTOR")]
    public async Task<IActionResult> ListPlayers(CancellationToken ct)
    {
        if (!TryGetCurrentUserId(out var instructorUserId))
        {
            return Unauthorized(ApiErrorHelper.BuildError(HttpContext, "UNAUTHORIZED", "Token user tidak valid"));
        }

        var players = await _players.ListPlayersAsync(instructorUserId, ct);
        var items = players.Select(p => new PlayerResponse(p.PlayerId, p.DisplayName)).ToList();
        return Ok(new PlayerListResponse(items));
    }

    [HttpPost("/api/v1/sessions/{sessionId:guid}/players")]
    [HttpPost("/api/sessions/{sessionId:guid}/players")]
    [Authorize(Roles = "INSTRUCTOR")]
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
        var joinOrder = request.JoinOrder ?? 0;

        await _players.AddPlayerToSessionAsync(sessionId, request.PlayerId, role, joinOrder, ct);
        return Ok();
    }

    private bool TryGetCurrentUserId(out Guid userId)
    {
        var userIdRaw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userIdRaw, out userId);
    }
}
