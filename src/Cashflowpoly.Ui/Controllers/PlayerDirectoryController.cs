using System.Net.Http.Json;
using Cashflowpoly.Ui.Infrastructure;
using Cashflowpoly.Ui.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cashflowpoly.Ui.Controllers;

/// <summary>
/// Controller UI untuk daftar pemain.
/// </summary>
[Route("players")]
public sealed class PlayerDirectoryController : Controller
{
    private readonly IHttpClientFactory _clientFactory;

    public PlayerDirectoryController(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        if (!HttpContext.Session.IsInstructor())
        {
            return RedirectToAction("Index", "Analytics");
        }

        var client = _clientFactory.CreateClient("Api");
        var response = await client.GetAsync("api/v1/players", ct);
        var unauthorized = this.HandleUnauthorizedApiResponse(response);
        if (unauthorized is not null)
        {
            return unauthorized;
        }

        if (!response.IsSuccessStatusCode)
        {
            return View("~/Views/Players/Index.cshtml", new PlayerDirectoryViewModel
            {
                ErrorMessage = $"Gagal memuat daftar pemain. Status: {(int)response.StatusCode}"
            });
        }

        var playerData = await response.Content.ReadFromJsonAsync<PlayerListResponseDto>(cancellationToken: ct);
        var players = playerData?.Items ?? new List<PlayerResponseDto>();

        var sessionsResponse = await client.GetAsync("api/v1/sessions", ct);
        unauthorized = this.HandleUnauthorizedApiResponse(sessionsResponse);
        if (unauthorized is not null)
        {
            return unauthorized;
        }

        var groups = new List<PlayerSessionGroupViewModel>();
        string? groupError = null;

        if (sessionsResponse.IsSuccessStatusCode)
        {
            var sessionsData = await sessionsResponse.Content.ReadFromJsonAsync<SessionListResponseDto>(cancellationToken: ct);
            var sessions = sessionsData?.Items ?? new List<SessionListItemDto>();
            var playerMap = players.ToDictionary(x => x.PlayerId, x => x.DisplayName);

            var analyticsTasks = sessions.Select(async session =>
            {
                var analyticsResponse = await client.GetAsync($"api/v1/analytics/sessions/{session.SessionId}", ct);
                if (!analyticsResponse.IsSuccessStatusCode)
                {
                    return (session, analytics: (AnalyticsSessionResponseDto?)null);
                }

                var analytics = await analyticsResponse.Content.ReadFromJsonAsync<AnalyticsSessionResponseDto>(cancellationToken: ct);
                return (session, analytics);
            });

            var analyticsResults = await Task.WhenAll(analyticsTasks);
            groups = analyticsResults
                .Where(x => x.analytics is not null && x.analytics.ByPlayer.Count > 0)
                .Select(x => new PlayerSessionGroupViewModel
                {
                    SessionId = x.session.SessionId,
                    SessionName = x.session.SessionName,
                    Status = x.session.Status,
                    StartedAt = x.session.StartedAt,
                    EndedAt = x.session.EndedAt,
                    Players = x.analytics!.ByPlayer
                        .OrderBy(p => p.JoinOrder > 0 ? p.JoinOrder : int.MaxValue)
                        .ThenBy(p => playerMap.TryGetValue(p.PlayerId, out var name) ? name : p.PlayerId.ToString())
                        .Select(p => new PlayerSessionEntryViewModel
                        {
                            PlayerId = p.PlayerId,
                            JoinOrder = p.JoinOrder,
                            DisplayName = playerMap.TryGetValue(p.PlayerId, out var displayName) ? displayName : p.PlayerId.ToString(),
                            CashInTotal = p.CashInTotal,
                            CashOutTotal = p.CashOutTotal,
                            DonationTotal = p.DonationTotal,
                            GoldQty = p.GoldQty,
                            HappinessPointsTotal = p.HappinessPointsTotal
                        })
                        .ToList()
                })
                .OrderByDescending(x => x.StartedAt ?? x.EndedAt ?? DateTimeOffset.MinValue)
                .ThenBy(x => x.SessionName)
                .ToList();
        }
        else
        {
            groupError = $"Gagal memuat daftar sesi untuk grouping pemain. Status: {(int)sessionsResponse.StatusCode}";
        }

        return View("~/Views/Players/Index.cshtml", new PlayerDirectoryViewModel
        {
            Players = players,
            SessionGroups = groups,
            ErrorMessage = groupError
        });
    }
}

