// Fungsi file: Menampilkan statistik satu pemain dalam satu mode sesuai lingkup sesi akun.
using System.Net.Http.Json;
using System.Security.Claims;
using Cashflowpoly.Ui.Contracts;
using Cashflowpoly.Ui.Infrastructure;
using Cashflowpoly.Ui.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cashflowpoly.Ui.Controllers;

[Authorize(Roles = AuthConstants.PlayerRole + "," + AuthConstants.InstructorRole)]
[Route("statistics")]
public sealed class PlayerStatisticsController(IHttpClientFactory clientFactory) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index(string? mode, string? status, CancellationToken ct, Guid? playerId = null)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Unauthorized();

        var isInstructor = HttpContext.IsInstructor();
        if (!isInstructor && playerId.HasValue && playerId != userId) return StatusCode(StatusCodes.Status403Forbidden);
        var selectedPlayer = isInstructor ? playerId ?? Guid.Empty : userId;
        var players = new List<PlayerResponse>();
        var selectedMode = mode?.ToUpperInvariant() is "PEMULA" or "MAHIR" ? mode.ToUpperInvariant() : null;
        var selectedStatus = status?.ToUpperInvariant() is "CREATED" or "STARTED" or "ENDED" ? status.ToUpperInvariant() : "ALL";
        var client = clientFactory.CreateClient("Api");
        var sessions = new List<PlayerStatisticsSession>();
        int? total = null;
        string? error = null;
        try
        {
            using var response = await client.GetAsync("api/v1/sessions", ct);
            if (this.HandleUnauthorizedApiResponse(response) is { } unauthorized) return unauthorized;
            var data = response.IsSuccessStatusCode
                ? await response.Content.TryReadFromJsonAsync<SessionListResponse>(cancellationToken: ct) : null;
            if (data?.Items is null)
            {
                error = HttpContext.T("statistics.error.load");
            }
            else
            {
                var eligible = data.Items;
                var participantsComplete = true;
                if (isInstructor)
                {
                    // Hanya peserta sesi milik instruktur ini yang dapat dipilih.
                    var rosters = await SessionRosterLoader.LoadAsync(client, data.Items, false, ct);
                    if (rosters.Any(r => r.Unauthorized))
                    {
                        using var expiredResponse = new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
                        return this.HandleUnauthorizedApiResponse(expiredResponse)!;
                    }
                    var complete = rosters.All(r => r.ParticipantsAvailable);
                    participantsComplete = complete;
                    players = rosters.SelectMany(r => r.Players).GroupBy(p => p.PlayerId)
                        .Select(g => new PlayerResponse(g.Key, g.First().DisplayName))
                        .OrderBy(p => p.DisplayName, StringComparer.CurrentCultureIgnoreCase).ThenBy(p => p.UserId).ToList();
                    if (playerId.HasValue && !players.Any(p => p.UserId == playerId))
                        return complete ? NotFound() : StatusCode(StatusCodes.Status503ServiceUnavailable);
                    selectedPlayer = playerId ?? players.FirstOrDefault()?.UserId ?? Guid.Empty;
                    var joined = rosters.Where(r => r.Players.Any(p => p.PlayerId == selectedPlayer))
                        .Select(r => r.SessionId).ToHashSet();
                    eligible = data.Items.Where(s => joined.Contains(s.SessionId)).ToList();
                    if (!complete) error = HttpContext.T("statistics.error.participants");
                }
                total = participantsComplete ? eligible.Count : null;
                selectedMode ??= eligible.OrderByDescending(s => s.StartedAt ?? s.CreatedAt)
                    .FirstOrDefault(s => s.Mode is "PEMULA" or "MAHIR")?.Mode ?? "PEMULA";
                var selected = eligible.Where(s =>
                    s.Mode == selectedMode &&
                    (selectedStatus == "ALL" || s.Status == selectedStatus))
                    .OrderBy(s => s.StartedAt ?? s.CreatedAt).ThenBy(s => s.SessionId).ToList();
                using var gate = new SemaphoreSlim(4);
                var expired = false;
                var failed = false;
                sessions = (await Task.WhenAll(selected.Select(async session =>
                {
                    // Sesi persiapan belum mempunyai hasil permainan untuk dibandingkan.
                    if (session.Status == "CREATED") return new PlayerStatisticsSession(session, null);
                    await gate.WaitAsync(ct);
                    try
                    {
                        using var result = await client.GetAsync($"api/v1/analytics/sessions/{session.SessionId}/players/{selectedPlayer}/gameplay", ct);
                        if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized) expired = true;
                        var gameplay = result.IsSuccessStatusCode
                            ? await result.Content.TryReadFromJsonAsync<GameplayMetricsResponse>(cancellationToken: ct) : null;
                        if (gameplay?.UserId != selectedPlayer || gameplay.SessionId != session.SessionId || gameplay.ComputedAt is null || gameplay.Economy is null || gameplay.Progress is null || gameplay.Score is null || gameplay.Needs is null)
                            gameplay = null;
                        if (gameplay is null) failed = true;
                        return new PlayerStatisticsSession(session, gameplay);
                    }
                    catch (HttpRequestException) { failed = true; return new PlayerStatisticsSession(session, null); }
                    catch (TaskCanceledException) when (!ct.IsCancellationRequested) { failed = true; return new PlayerStatisticsSession(session, null); }
                    finally { gate.Release(); }
                }))).ToList();
                if (expired)
                {
                    using var unauthorizedResponse = new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
                    return this.HandleUnauthorizedApiResponse(unauthorizedResponse)!;
                }
                if (failed) error = string.Join(" ", new[] { error, HttpContext.T("statistics.error.partial") }.Where(e => e is not null));
            }
        }
        catch (HttpRequestException) { error = HttpContext.T("statistics.error.load"); }
        catch (TaskCanceledException) when (!ct.IsCancellationRequested) { error = HttpContext.T("statistics.error.load"); }

        return View(new PlayerStatisticsViewModel
        {
            PlayerId = selectedPlayer, Mode = selectedMode ?? "PEMULA", Status = selectedStatus,
            AvailablePlayers = players,
            PlayerName = isInstructor ? players.FirstOrDefault(p => p.UserId == selectedPlayer)?.DisplayName
                : User.FindFirst(AuthConstants.DisplayNameClaim)?.Value ?? User.Identity?.Name,
            TotalSessions = total, Sessions = sessions, ErrorMessage = error,
            Charts = PlayerStatisticsChartBuilder.Build(sessions, HttpContext.T)
        });
    }
}
