using System.Net.Http.Json;
using Cashflowpoly.Ui.Infrastructure;
using Cashflowpoly.Ui.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cashflowpoly.Ui.Controllers;

/// <summary>
/// Controller UI untuk detail pemain pada sesi.
/// </summary>
[Route("sessions/{sessionId:guid}/players")]
public sealed class PlayersController : Controller
{
    private readonly IHttpClientFactory _clientFactory;

    public PlayersController(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    [HttpGet("{playerId:guid}")]
    public async Task<IActionResult> Details(Guid sessionId, Guid playerId, CancellationToken ct)
    {
        var client = _clientFactory.CreateClient("Api");
        var playerDisplayName = await ResolvePlayerDisplayNameAsync(client, playerId, ct);
        var analyticsResponse = await client.GetAsync($"api/v1/analytics/sessions/{sessionId}", ct);
        var unauthorized = this.HandleUnauthorizedApiResponse(analyticsResponse);
        if (unauthorized is not null)
        {
            return unauthorized;
        }

        if (!analyticsResponse.IsSuccessStatusCode)
        {
            var error = await analyticsResponse.Content.ReadFromJsonAsync<ApiErrorResponseDto>(cancellationToken: ct);
            return View(new PlayerDetailViewModel
            {
                SessionId = sessionId,
                PlayerId = playerId,
                PlayerDisplayName = playerDisplayName,
                ErrorMessage = error?.Message ?? HttpContext
                    .T("players.error.load_session_analytics_failed")
                    .Replace("{status}", ((int)analyticsResponse.StatusCode).ToString())
            });
        }

        var analytics = await analyticsResponse.Content.ReadFromJsonAsync<AnalyticsSessionResponseDto>(cancellationToken: ct);
        var summary = analytics?.ByPlayer.FirstOrDefault(p => p.PlayerId == playerId);

        var txResponse = await client.GetAsync($"api/v1/analytics/sessions/{sessionId}/transactions?playerId={playerId}", ct);
        unauthorized = this.HandleUnauthorizedApiResponse(txResponse);
        if (unauthorized is not null)
        {
            return unauthorized;
        }

        if (!txResponse.IsSuccessStatusCode)
        {
            var error = await txResponse.Content.ReadFromJsonAsync<ApiErrorResponseDto>(cancellationToken: ct);
            return View(new PlayerDetailViewModel
            {
                SessionId = sessionId,
                PlayerId = playerId,
                PlayerDisplayName = playerDisplayName,
                Summary = summary,
                ErrorMessage = error?.Message ?? HttpContext
                    .T("players.error.load_transactions_failed")
                    .Replace("{status}", ((int)txResponse.StatusCode).ToString())
            });
        }

        var tx = await txResponse.Content.ReadFromJsonAsync<TransactionHistoryResponseDto>(cancellationToken: ct);
        string? gameplayError = null;
        GameplayMetricsResponseDto? gameplay = null;
        var gameplayResponse = await client.GetAsync($"api/v1/analytics/sessions/{sessionId}/players/{playerId}/gameplay", ct);
        unauthorized = this.HandleUnauthorizedApiResponse(gameplayResponse);
        if (unauthorized is not null)
        {
            return unauthorized;
        }

        if (gameplayResponse.IsSuccessStatusCode)
        {
            gameplay = await gameplayResponse.Content.ReadFromJsonAsync<GameplayMetricsResponseDto>(cancellationToken: ct);
        }
        else
        {
            var error = await gameplayResponse.Content.ReadFromJsonAsync<ApiErrorResponseDto>(cancellationToken: ct);
            gameplayError = error?.Message ?? HttpContext
                .T("players.error.load_gameplay_failed")
                .Replace("{status}", ((int)gameplayResponse.StatusCode).ToString());
        }

        return View(new PlayerDetailViewModel
        {
            SessionId = sessionId,
            PlayerId = playerId,
            PlayerDisplayName = playerDisplayName,
            Summary = summary,
            Transactions = tx?.Items ?? new List<TransactionHistoryItemDto>(),
            GameplayRaw = gameplay?.Raw,
            GameplayDerived = gameplay?.Derived,
            GameplayComputedAt = gameplay?.ComputedAt,
            GameplayErrorMessage = gameplayError
        });
    }

    private static async Task<string?> ResolvePlayerDisplayNameAsync(HttpClient client, Guid playerId, CancellationToken ct)
    {
        var response = await client.GetAsync("api/v1/players", ct);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var players = await response.Content.ReadFromJsonAsync<PlayerListResponseDto>(cancellationToken: ct);
        return players?.Items.FirstOrDefault(item => item.PlayerId == playerId)?.DisplayName;
    }
}

