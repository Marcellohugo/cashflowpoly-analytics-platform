// Fungsi file: Memuat roster lintas sesi dalam satu permintaan tanpa menghapus sesi saat data tidak tersedia.
using System.Net;
using System.Net.Http.Json;
using Cashflowpoly.Ui.Contracts;
using Cashflowpoly.Ui.Models;

namespace Cashflowpoly.Ui.Infrastructure;

public static class SessionRosterLoader
{
    public static async Task<List<PlayerSessionGroupViewModel>> LoadAsync(
        HttpClient client, IReadOnlyList<SessionListItem> sessions, bool includeResults, CancellationToken ct)
    {
        if (sessions.Count == 0) return [];
        SessionRostersResponse? data = null;
        var unauthorized = false;
        try
        {
            using var response = await client.GetAsync($"api/v1/analytics/session-rosters?includeResults={includeResults.ToString().ToLowerInvariant()}", ct);
            unauthorized = response.StatusCode == HttpStatusCode.Unauthorized;
            if (response.IsSuccessStatusCode)
                data = await response.Content.TryReadFromJsonAsync<SessionRostersResponse>(cancellationToken: ct);
        }
        catch (HttpRequestException) { }
        catch (TaskCanceledException) when (!ct.IsCancellationRequested) { }
        var rosters = data?.Items?.GroupBy(r => r.SessionId).Where(g => g.Count() == 1)
            .ToDictionary(g => g.Key, g => g.Single());
        return sessions.Select(session =>
        {
            var roster = rosters?.GetValueOrDefault(session.SessionId);
            return new PlayerSessionGroupViewModel
            {
                SessionId = session.SessionId, SessionName = session.SessionName,
                Mode = session.Mode, Status = session.Status, CreatedAt = session.CreatedAt,
                StartedAt = session.StartedAt, EndedAt = session.EndedAt,
                ParticipantsAvailable = roster?.Players is not null, Unauthorized = unauthorized,
                ResultsAvailable = roster?.ResultsAvailable == true,
                Players = (roster?.Players ?? []).OrderBy(p => p.PlayerOrder > 0 ? p.PlayerOrder : int.MaxValue)
                    .ThenBy(p => p.UserId).Select(p => new PlayerSessionEntryViewModel
                    {
                        PlayerId = p.UserId, DisplayName = p.DisplayName, PlayerOrder = p.PlayerOrder,
                        FinalRank = p.FinalRank ?? 0, HappinessPointsTotal = p.HappinessPointsTotal
                    }).ToList()
            };
        }).ToList();
    }
}
