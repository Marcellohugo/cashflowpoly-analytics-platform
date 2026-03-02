// Fungsi file: Menangani permintaan HTTP untuk halaman direktori pemain, menampilkan daftar pemain yang dikelompokkan berdasarkan sesi beserta statistik analitik masing-masing.
using System.Net.Http.Json;
using Cashflowpoly.Ui.Infrastructure;
using Cashflowpoly.Ui.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cashflowpoly.Ui.Controllers;

/// <summary>
/// Controller MVC yang mengelola halaman direktori pemain lintas sesi,
/// menampilkan daftar semua pemain beserta ringkasan statistik per sesi (hanya instruktur).
/// </summary>
[Route("players")]
public sealed class PlayerDirectoryController : Controller
{
    private readonly IHttpClientFactory _clientFactory;

    /// <summary>
    /// Menginisialisasi controller direktori pemain dengan factory HTTP client untuk komunikasi ke API backend.
    /// </summary>
    /// <param name="clientFactory">Factory untuk membuat instance <see cref="HttpClient"/> ke API backend.</param>
    public PlayerDirectoryController(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    [HttpGet("")]
    /// <summary>
    /// Menampilkan halaman direktori pemain dengan daftar pemain yang dikelompokkan berdasarkan sesi
    /// dan dilengkapi statistik analitik (hanya instruktur).
    /// </summary>
    /// <param name="ct">Token pembatalan untuk membatalkan permintaan.</param>
    /// <returns>View berisi daftar pemain per sesi dengan statistik, atau redirect jika bukan instruktur.</returns>
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        if (!HttpContext.Session.IsInstructor())
        {
            return RedirectToAction("Index", "Sessions");
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
                ErrorMessage = HttpContext
                    .T("players.error.load_player_directory_failed")
                    .Replace("{status}", ((int)response.StatusCode).ToString())
            });
        }

        var playerData = await response.Content.TryReadFromJsonAsync<PlayerListResponseDto>(cancellationToken: ct);
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
            var sessionsData = await sessionsResponse.Content.TryReadFromJsonAsync<SessionListResponseDto>(cancellationToken: ct);
            var sessions = sessionsData?.Items ?? new List<SessionListItemDto>();
            var playerMap = players.ToDictionary(x => x.PlayerId, x => x.DisplayName);

            var analyticsTasks = sessions.Select(async session =>
            {
                var analyticsResponse = await client.GetAsync($"api/v1/analytics/sessions/{session.SessionId}", ct);
                if (!analyticsResponse.IsSuccessStatusCode)
                {
                    return (session, analytics: (AnalyticsSessionResponseDto?)null);
                }

                var analytics = await analyticsResponse.Content.TryReadFromJsonAsync<AnalyticsSessionResponseDto>(cancellationToken: ct);
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
                        .ThenBy(p => p.PlayerId)
                        .Select((p, index) => new PlayerSessionEntryViewModel
                        {
                            PlayerId = p.PlayerId,
                            JoinOrder = p.JoinOrder > 0 ? p.JoinOrder : index + 1,
                            DisplayName = playerMap.TryGetValue(p.PlayerId, out var displayName) ? displayName : p.PlayerId.ToString(),
                            CashInTotal = p.CashInTotal,
                            CashOutTotal = p.CashOutTotal,
                            DonationTotal = p.DonationTotal,
                            DonationPointsTotal = p.DonationPointsTotal,
                            PensionPointsTotal = p.PensionPointsTotal,
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
            groupError = HttpContext
                .T("players.error.load_sessions_grouping_failed")
                .Replace("{status}", ((int)sessionsResponse.StatusCode).ToString());
        }

        return View("~/Views/Players/Index.cshtml", new PlayerDirectoryViewModel
        {
            Players = players,
            SessionGroups = groups,
            ErrorMessage = groupError
        });
    }
}

