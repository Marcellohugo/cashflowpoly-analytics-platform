// Fungsi file: Menangani request MVC dan penyusunan tampilan untuk PlayerDirectoryController.
using System.Net.Http.Json;
using Cashflowpoly.Ui.Contracts;
using Cashflowpoly.Ui.Infrastructure;
using Cashflowpoly.Ui.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cashflowpoly.Ui.Controllers;

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

        var playerData = await response.Content.TryReadFromJsonAsync<PlayerListResponse>(cancellationToken: ct);
        var players = playerData?.Items ?? new List<PlayerResponse>();

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
            var sessionsData = await sessionsResponse.Content.TryReadFromJsonAsync<SessionListResponse>(cancellationToken: ct);
            var sessions = sessionsData?.Items ?? new List<SessionListItem>();
            var playerNames = players.ToDictionary(player => player.UserId, player => player.DisplayName);
            using var requestGate = new SemaphoreSlim(8);
            var sessionTasks = sessions.Select(async session =>
            {
                await requestGate.WaitAsync(ct);
                try
                {
                    if (!string.Equals(session.Status, "ENDED", StringComparison.OrdinalIgnoreCase))
                    {
                        var activeParticipantsResponse = await client.GetAsync($"api/v1/sessions/{session.SessionId}/players", ct);
                        if (!activeParticipantsResponse.IsSuccessStatusCode)
                        {
                            return (session, participants: (SessionPlayerListResponse?)null, analytics: (AnalyticsSessionResponse?)null);
                        }

                        var activeParticipants = await activeParticipantsResponse.Content.TryReadFromJsonAsync<SessionPlayerListResponse>(cancellationToken: ct);
                        return (session, participants: activeParticipants, analytics: (AnalyticsSessionResponse?)null);
                    }

                    var analyticsResponse = await client.GetAsync($"api/v1/analytics/sessions/{session.SessionId}", ct);
                    if (analyticsResponse.IsSuccessStatusCode)
                    {
                        var analytics = await analyticsResponse.Content.TryReadFromJsonAsync<AnalyticsSessionResponse>(cancellationToken: ct);
                        if (analytics is not null)
                        {
                            var analyticsParticipants = new SessionPlayerListResponse(analytics.ByPlayer
                                .Select(player => new SessionPlayerResponse(
                                    player.UserId,
                                    playerNames.GetValueOrDefault(player.UserId, string.Empty),
                                    player.PlayerOrder))
                                .ToList());
                            return (session, participants: analyticsParticipants, analytics);
                        }
                    }

                    var participantsResponse = await client.GetAsync($"api/v1/sessions/{session.SessionId}/players", ct);
                    if (!participantsResponse.IsSuccessStatusCode)
                    {
                        return (session, participants: (SessionPlayerListResponse?)null, analytics: (AnalyticsSessionResponse?)null);
                    }

                    var participants = await participantsResponse.Content.TryReadFromJsonAsync<SessionPlayerListResponse>(cancellationToken: ct);
                    return (session, participants, analytics: (AnalyticsSessionResponse?)null);
                }
                catch (HttpRequestException)
                {
                    return (session, participants: (SessionPlayerListResponse?)null, analytics: (AnalyticsSessionResponse?)null);
                }
                catch (TaskCanceledException) when (!ct.IsCancellationRequested)
                {
                    return (session, participants: (SessionPlayerListResponse?)null, analytics: (AnalyticsSessionResponse?)null);
                }
                finally
                {
                    requestGate.Release();
                }
            });

            var sessionResults = await Task.WhenAll(sessionTasks);
            if (sessionResults.Any(result =>
                    result.participants is null ||
                    (string.Equals(result.session.Status, "ENDED", StringComparison.OrdinalIgnoreCase) && result.analytics is null)))
            {
                groupError = HttpContext.T("players.error.load_session_details_partial");
            }

            groups = sessionResults
                .Where(x => x.participants is not null && x.participants.Items.Count > 0)
                .Select(x => new PlayerSessionGroupViewModel
                {
                    SessionId = x.session.SessionId,
                    SessionName = x.session.SessionName,
                    Status = x.session.Status,
                    StartedAt = x.session.StartedAt,
                    EndedAt = x.session.EndedAt,
                    Players = x.participants!.Items
                        .OrderBy(p => p.PlayerOrder > 0 ? p.PlayerOrder : int.MaxValue)
                        .ThenBy(p => p.UserId)
                        .Select((p, index) =>
                        {
                            var analytics = x.analytics?.ByPlayer.FirstOrDefault(item => item.UserId == p.UserId);
                            var leaderboard = x.analytics?.Leaderboard?.FirstOrDefault(item => item.UserId == p.UserId);
                            return new PlayerSessionEntryViewModel
                            {
                                PlayerId = p.UserId,
                                PlayerOrder = p.PlayerOrder > 0 ? p.PlayerOrder : index + 1,
                                FinalRank = leaderboard?.Rank ?? 0,
                                DisplayName = string.IsNullOrWhiteSpace(p.DisplayName)
                                    ? $"{HttpContext.T("common.player")} {(p.PlayerOrder > 0 ? p.PlayerOrder : index + 1)}"
                                    : p.DisplayName,
                                CashInTotal = analytics?.CashInTotal ?? 0,
                                CashOutTotal = analytics?.CashOutTotal ?? 0,
                                DonationTotal = analytics?.DonationTotal ?? 0,
                                DonationPointsTotal = analytics?.DonationPointsTotal ?? 0,
                                PensionPointsTotal = analytics?.PensionPointsTotal ?? 0,
                                GoldQty = analytics?.GoldQty ?? 0,
                                HappinessPointsTotal = leaderboard?.HappinessPointsTotal ?? analytics?.HappinessPointsTotal ?? 0
                            };
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
