// Fungsi file: Memuat peserta dari sesi yang dapat diakses akun, dengan hasil akhir opsional.
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
        using var gate = new SemaphoreSlim(4);
        return (await Task.WhenAll(sessions.Select(async session =>
        {
            await gate.WaitAsync(ct);
            SessionPlayerListResponse? participants = null;
            AnalyticsSessionResponse? analytics = null;
            var unauthorized = false;
            try
            {
                using var response = await client.GetAsync($"api/v1/sessions/{session.SessionId}/players", ct);
                unauthorized = response.StatusCode == HttpStatusCode.Unauthorized;
                if (response.IsSuccessStatusCode)
                    participants = await response.Content.TryReadFromJsonAsync<SessionPlayerListResponse>(cancellationToken: ct);
                if (participants?.Items is not null && includeResults && session.Status == "ENDED")
                {
                    using var result = await client.GetAsync($"api/v1/analytics/sessions/{session.SessionId}", ct);
                    unauthorized |= result.StatusCode == HttpStatusCode.Unauthorized;
                    if (result.IsSuccessStatusCode)
                        analytics = await result.Content.TryReadFromJsonAsync<AnalyticsSessionResponse>(cancellationToken: ct);
                    if (analytics?.SessionId != session.SessionId || analytics.ByPlayer is null) analytics = null;
                }
            }
            catch (HttpRequestException) { }
            catch (TaskCanceledException) when (!ct.IsCancellationRequested) { }
            finally { gate.Release(); }

            return new PlayerSessionGroupViewModel
            {
                SessionId = session.SessionId, SessionName = session.SessionName,
                Mode = session.Mode, Status = session.Status, CreatedAt = session.CreatedAt,
                StartedAt = session.StartedAt, EndedAt = session.EndedAt,
                ParticipantsAvailable = participants?.Items is not null, Unauthorized = unauthorized,
                ResultsAvailable = analytics is not null,
                Players = (participants?.Items ?? []).OrderBy(p => p.PlayerOrder > 0 ? p.PlayerOrder : int.MaxValue)
                    .ThenBy(p => p.UserId).Select(p =>
                    {
                        var summary = analytics?.ByPlayer?.FirstOrDefault(x => x.UserId == p.UserId);
                        var rank = analytics?.Leaderboard?.FirstOrDefault(x => x.UserId == p.UserId);
                        return new PlayerSessionEntryViewModel
                        {
                            PlayerId = p.UserId, DisplayName = p.DisplayName, PlayerOrder = p.PlayerOrder,
                            FinalRank = rank?.Rank ?? 0,
                            HappinessPointsTotal = rank?.HappinessPointsTotal ?? summary?.HappinessPointsTotal
                        };
                    }).ToList()
            };
        }))).ToList();
    }
}
