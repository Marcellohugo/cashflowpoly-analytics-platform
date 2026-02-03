using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cashflowpoly.Api.Controllers;

/// <summary>
/// Controller untuk manajemen pemain dan relasi pemain ke sesi.
/// </summary>
[ApiController]
[Route("api/players")]
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
    public async Task<IActionResult> CreatePlayer([FromBody] CreatePlayerRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.DisplayName))
        {
            return BadRequest(ApiErrorHelper.BuildError(HttpContext, "VALIDATION_ERROR", "Nama pemain wajib diisi",
                new ErrorDetail("display_name", "REQUIRED")));
        }

        var playerId = await _players.CreatePlayerAsync(request.DisplayName, ct);
        return Created($"/api/players/{playerId}", new PlayerResponse(playerId, request.DisplayName));
    }

    [HttpGet]
    public async Task<IActionResult> ListPlayers(CancellationToken ct)
    {
        var players = await _players.ListPlayersAsync(ct);
        var items = players.Select(p => new PlayerResponse(p.PlayerId, p.DisplayName)).ToList();
        return Ok(new PlayerListResponse(items));
    }

    [HttpPost("/api/sessions/{sessionId:guid}/players")]
    public async Task<IActionResult> AddPlayerToSession(Guid sessionId, [FromBody] AddSessionPlayerRequest request, CancellationToken ct)
    {
        var session = await _sessions.GetSessionAsync(sessionId, ct);
        if (session is null)
        {
            return NotFound(ApiErrorHelper.BuildError(HttpContext, "NOT_FOUND", "Session tidak ditemukan"));
        }

        var player = await _players.GetPlayerAsync(request.PlayerId, ct);
        if (player is null)
        {
            return NotFound(ApiErrorHelper.BuildError(HttpContext, "NOT_FOUND", "Player tidak ditemukan"));
        }

        var role = string.IsNullOrWhiteSpace(request.Role) ? "PLAYER" : request.Role;
        var joinOrder = request.JoinOrder ?? 0;

        await _players.AddPlayerToSessionAsync(sessionId, request.PlayerId, role, joinOrder, ct);
        return Ok();
    }
}
