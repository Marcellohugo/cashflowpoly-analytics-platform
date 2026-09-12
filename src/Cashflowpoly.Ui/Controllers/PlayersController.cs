// Fungsi file: Memuat analitika pemain dengan memverifikasi peserta sesi dan snapshot sumbernya.
using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using Cashflowpoly.Ui.Contracts;
using Cashflowpoly.Ui.Infrastructure;
using Cashflowpoly.Ui.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cashflowpoly.Ui.Controllers;

[Route("sessions/{sessionId:guid}/players")]
public sealed class PlayersController(IHttpClientFactory clientFactory) : Controller
{
    [HttpGet("{playerId:guid}")]
    public async Task<IActionResult> Details(Guid sessionId, Guid playerId, CancellationToken ct)
    {
        if (!HttpContext.IsInstructor() &&
            (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var currentUserId) || currentUserId != playerId))
            return StatusCode(StatusCodes.Status403Forbidden);

        var client = clientFactory.CreateClient("Api");
        AnalyticsByPlayerItem? summary = null;
        GameplayMetricsResponse? gameplay = null;
        string? playerDisplayName = null;
        string? errorMessage = null;
        string? gameplayError = null;
        try
        {
            using var rosterResponse = await client.GetAsync($"api/v1/sessions/{sessionId}/players", ct);
            if (this.HandleUnauthorizedApiResponse(rosterResponse) is { } rosterUnauthorized) return rosterUnauthorized;
            if (rosterResponse.StatusCode is HttpStatusCode.Forbidden or HttpStatusCode.NotFound)
                return StatusCode((int)rosterResponse.StatusCode);
            var roster = rosterResponse.IsSuccessStatusCode
                ? await rosterResponse.Content.TryReadFromJsonAsync<SessionPlayerListResponse>(ct) : null;
            var participant = roster?.Items?.FirstOrDefault(p => p.UserId == playerId);
            if (roster?.Items is not null && participant is null) return NotFound();
            playerDisplayName = participant?.DisplayName;

            using var analyticsResponse = await client.GetAsync($"api/v1/analytics/sessions/{sessionId}", ct);
            if (this.HandleUnauthorizedApiResponse(analyticsResponse) is { } unauthorized) return unauthorized;
            if (analyticsResponse.StatusCode is HttpStatusCode.Forbidden or HttpStatusCode.NotFound)
                return StatusCode((int)analyticsResponse.StatusCode);
            var analytics = analyticsResponse.IsSuccessStatusCode
                ? await analyticsResponse.Content.TryReadFromJsonAsync<AnalyticsSessionResponse>(ct) : null;
            if (analytics?.SessionId == sessionId)
                summary = analytics.ByPlayer?.FirstOrDefault(p => p.UserId == playerId);
            if (summary is null)
                errorMessage = HttpContext.T("players.error.load_session_analytics_failed")
                    .Replace("{status}", ((int)analyticsResponse.StatusCode).ToString());

            // Gameplay supplies the evidence table and opening balance from the same snapshot.
            // A second paginated transaction feed could silently omit entries or disagree with it.
            using var gameplayResponse = await client.GetAsync($"api/v1/analytics/sessions/{sessionId}/players/{playerId}/gameplay", ct);
            if (this.HandleUnauthorizedApiResponse(gameplayResponse) is { } gameplayUnauthorized) return gameplayUnauthorized;
            if (gameplayResponse.StatusCode is HttpStatusCode.Forbidden or HttpStatusCode.NotFound)
                return StatusCode((int)gameplayResponse.StatusCode);
            if (gameplayResponse.IsSuccessStatusCode)
                gameplay = await gameplayResponse.Content.TryReadFromJsonAsync<GameplayMetricsResponse>(ct);
            if (gameplay?.SessionId != sessionId || gameplay.UserId != playerId || gameplay.ComputedAt is null || gameplay.Economy is null || gameplay.Progress is null || gameplay.Score is null || gameplay.Needs is null)
                gameplay = null;
            if (gameplay is null)
                gameplayError = HttpContext.T("players.error.load_gameplay_failed")
                    .Replace("{status}", ((int)gameplayResponse.StatusCode).ToString());
        }
        catch (HttpRequestException) { errorMessage = HttpContext.T("auth.error.api_unavailable"); }
        catch (TaskCanceledException) when (!ct.IsCancellationRequested) { errorMessage = HttpContext.T("auth.error.api_unavailable"); }

        return View(new PlayerDetailViewModel
        {
            SessionId = sessionId, PlayerId = playerId, PlayerDisplayName = playerDisplayName,
            Summary = summary, Gameplay = gameplay,
            StatSummary = PlayerStatSummaryBuilder.Build(gameplay, summary, HttpContext.T),
            GameplayRaw = gameplay?.RawJson, GameplayDerived = gameplay?.DerivedJson,
            GameplayComputedAt = gameplay?.ComputedAt,
            GameplayErrorMessage = gameplayError, ErrorMessage = errorMessage
        });
    }
}
