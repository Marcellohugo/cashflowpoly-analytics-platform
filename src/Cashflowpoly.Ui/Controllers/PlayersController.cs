using System.Net.Http.Json;
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
        var analyticsResponse = await client.GetAsync($"api/analytics/sessions/{sessionId}", ct);
        if (!analyticsResponse.IsSuccessStatusCode)
        {
            var error = await analyticsResponse.Content.ReadFromJsonAsync<ApiErrorResponseDto>(cancellationToken: ct);
            return View(new PlayerDetailViewModel
            {
                SessionId = sessionId,
                PlayerId = playerId,
                ErrorMessage = error?.Message ?? $"Gagal memuat analitika sesi. Status: {(int)analyticsResponse.StatusCode}"
            });
        }

        var analytics = await analyticsResponse.Content.ReadFromJsonAsync<AnalyticsSessionResponseDto>(cancellationToken: ct);
        var summary = analytics?.ByPlayer.FirstOrDefault(p => p.PlayerId == playerId);

        var txResponse = await client.GetAsync($"api/analytics/sessions/{sessionId}/transactions?playerId={playerId}", ct);
        if (!txResponse.IsSuccessStatusCode)
        {
            var error = await txResponse.Content.ReadFromJsonAsync<ApiErrorResponseDto>(cancellationToken: ct);
            return View(new PlayerDetailViewModel
            {
                SessionId = sessionId,
                PlayerId = playerId,
                Summary = summary,
                ErrorMessage = error?.Message ?? $"Gagal memuat transaksi. Status: {(int)txResponse.StatusCode}"
            });
        }

        var tx = await txResponse.Content.ReadFromJsonAsync<TransactionHistoryResponseDto>(cancellationToken: ct);
        string? gameplayError = null;
        GameplayMetricsResponseDto? gameplay = null;
        var gameplayResponse = await client.GetAsync($"api/analytics/sessions/{sessionId}/players/{playerId}/gameplay", ct);
        if (gameplayResponse.IsSuccessStatusCode)
        {
            gameplay = await gameplayResponse.Content.ReadFromJsonAsync<GameplayMetricsResponseDto>(cancellationToken: ct);
        }
        else
        {
            var error = await gameplayResponse.Content.ReadFromJsonAsync<ApiErrorResponseDto>(cancellationToken: ct);
            gameplayError = error?.Message ?? $"Gagal memuat metrik gameplay. Status: {(int)gameplayResponse.StatusCode}";
        }

        return View(new PlayerDetailViewModel
        {
            SessionId = sessionId,
            PlayerId = playerId,
            Summary = summary,
            Transactions = tx?.Items ?? new List<TransactionHistoryItemDto>(),
            GameplayRaw = gameplay?.Raw,
            GameplayDerived = gameplay?.Derived,
            GameplayComputedAt = gameplay?.ComputedAt,
            GameplayErrorMessage = gameplayError
        });
    }
}
