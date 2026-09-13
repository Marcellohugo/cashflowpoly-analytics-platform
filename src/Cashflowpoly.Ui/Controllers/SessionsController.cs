// Fungsi file: Menangani request MVC dan penyusunan tampilan untuk SessionsController.
// Mengimpor namespace `System.Net` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Net;
// Mengimpor namespace `System.Net.Http.Json` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Net.Http.Json;
// Mengimpor namespace `Cashflowpoly.Ui.Contracts` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Ui.Contracts;
// Mengimpor namespace `Cashflowpoly.Ui.Domain` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Ui.Domain;
// Mengimpor namespace `Cashflowpoly.Ui.Infrastructure` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Ui.Infrastructure;
// Mengimpor namespace `Cashflowpoly.Ui.Models` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Ui.Models;
// Mengimpor namespace `Microsoft.AspNetCore.Mvc` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.AspNetCore.Mvc;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Controllers` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Controllers;

// menetapkan pola rute (”sessions”) untuk pencocokan URL permintaan.
[Route("sessions")]
public sealed class SessionsController : Controller
{
    private readonly IHttpClientFactory _clientFactory;

    public SessionsController(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    // mendaftarkan action untuk metode HTTP GET pada rute (””).
    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        try
        {
            var client = _clientFactory.CreateClient("Api");
            var response = await client.GetAsync("api/v1/sessions", ct);
            var unauthorized = this.HandleUnauthorizedApiResponse(response);
            if (unauthorized is not null)
            {
                return unauthorized;
            }

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    return View(new SessionListViewModel
                    {
                        ErrorMessage = HttpContext.T("sessions.error.too_many_requests")
                    });
                }

                return View(new SessionListViewModel
                {
                    ErrorMessage = HttpContext
                        .T("sessions.error.load_sessions_failed")
                        .Replace("{status}", ((int)response.StatusCode).ToString())
                });
            }

            var data = await response.Content.TryReadFromJsonAsync<SessionListResponse>(cancellationToken: ct);
            var items = data?.Items ?? [];
            var groups = await SessionRosterLoader.LoadAsync(client, items, true, ct);
            if (groups.Any(g => g.Unauthorized))
            {
                using var expired = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                return this.HandleUnauthorizedApiResponse(expired)!;
            }
            var participantsComplete = groups.All(g => g.ParticipantsAvailable);
            return View(new SessionListViewModel
            {
                Items = items, SessionGroups = groups, SessionsAvailable = data?.Items is not null,
                MonitoredPlayers = data?.Items is not null && participantsComplete
                    ? groups.SelectMany(g => g.Players).Select(p => p.PlayerId).Distinct().Count() : null,
                ErrorMessage = data?.Items is null || groups.Any(g => !g.ParticipantsAvailable || (g.Status == "ENDED" && !g.ResultsAvailable))
                    ? HttpContext.T("players.error.load_session_details_partial") : null
            });
        }
        catch (HttpRequestException) { return View(new SessionListViewModel { ErrorMessage = HttpContext.T("auth.error.api_unavailable") }); }
        catch (TaskCanceledException) when (!ct.IsCancellationRequested)
        { return View(new SessionListViewModel { ErrorMessage = HttpContext.T("auth.error.api_unavailable") }); }
    }

    [HttpGet("{sessionId:guid}")]
    public async Task<IActionResult> Details(Guid sessionId, CancellationToken ct)
    {
        try
        {
            var detail = await BuildSessionDetailViewModel(sessionId, ct);
            return detail.Result ?? View(detail.Model);
        }
        catch (HttpRequestException)
        { return View(new SessionDetailViewModel { SessionId = sessionId, ErrorMessage = HttpContext.T("auth.error.api_unavailable") }); }
        catch (TaskCanceledException) when (!ct.IsCancellationRequested)
        { return View(new SessionDetailViewModel { SessionId = sessionId, ErrorMessage = HttpContext.T("auth.error.api_unavailable") }); }
    }

    [HttpGet("{sessionId:guid}/timeline")]
    public async Task<IActionResult> Timeline(
        // Parameter `sessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini.
        Guid sessionId,
        // Parameter `cursor` bertipe `string?` membawa penanda halaman untuk melanjutkan pembacaan setelah elemen sebelumnya; nilai null diizinkan ketika
        // data opsional belum tersedia; bila argumen tidak diberikan digunakan null, yaitu penanda tidak ada nilai; mengambil nilai parameter dari query
        // string URL.
        [FromQuery] string? cursor = null,
        // Parameter `limit` bertipe `int` membawa batas jumlah hasil yang diminta pada satu operasi; bila argumen tidak diberikan digunakan nilai literal
        // `100`; mengambil nilai parameter dari query string URL.
        [FromQuery] int limit = 100,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti; bila argumen tidak diberikan digunakan nilai literal `default`.
        CancellationToken ct = default)
    {
        var normalizedLimit = Math.Clamp(limit, 1, 100);
        var client = _clientFactory.CreateClient("Api");
        var cursorQuery = string.IsNullOrWhiteSpace(cursor) ? string.Empty : $"&cursor={Uri.EscapeDataString(cursor)}";
        var response = await client.GetAsync($"api/v1/sessions/{sessionId}/events?limit={normalizedLimit}{cursorQuery}", ct);
        var unauthorized = this.HandleUnauthorizedApiResponse(response);
        if (unauthorized is not null)
        {
            return unauthorized;
        }

        if (!response.IsSuccessStatusCode)
        {
            return Json(new
            {
                timeline = Array.Empty<SessionTimelineEventViewModel>(),
                errorMessage = HttpContext
                    .T("sessions.error.load_timeline_failed")
                    .Replace("{status}", ((int)response.StatusCode).ToString()),
                lastSyncedAt = DateTimeOffset.UtcNow
            });
        }

        var data = await response.Content.TryReadFromJsonAsync<EventsBySessionResponse>(cancellationToken: ct);
        if (data?.Items is null || (data.HasMore && (string.IsNullOrWhiteSpace(data.NextCursor) || data.NextCursor == cursor)))
            return Json(new
            {
                timeline = Array.Empty<SessionTimelineEventViewModel>(),
                errorMessage = HttpContext.T("sessions.error.timeline_incomplete"),
                lastSyncedAt = DateTimeOffset.UtcNow
            });
        var language = UiText.NormalizeLanguage(HttpContext.Session.GetString(AuthConstants.SessionLanguageKey));
        var timeline = SessionTimelineMapper.MapTimeline(data?.Items, language);
        var playerDisplayNames = await LoadPlayerDisplayNameMapAsync(client, sessionId, ct);
        SessionTimelineMapper.ApplyPlayerDisplayNames(timeline, playerDisplayNames);

        return Json(new
        {
            timeline,
            nextCursor = data?.NextCursor,
            hasMore = data?.HasMore ?? false,
            errorMessage = (string?)null,
            lastSyncedAt = DateTimeOffset.UtcNow
        });
    }

    private async Task<(SessionDetailViewModel Model, IActionResult? Result)> BuildSessionDetailViewModel(
        // Parameter `sessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini.
        Guid sessionId,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    {
        var client = _clientFactory.CreateClient("Api");
        var response = await client.GetAsync($"api/v1/analytics/sessions/{sessionId}", ct);
        var unauthorized = this.HandleUnauthorizedApiResponse(response);
        var language = UiText.NormalizeLanguage(HttpContext.Session.GetString(AuthConstants.SessionLanguageKey));
        if (unauthorized is not null)
        {
            return (new SessionDetailViewModel
            {
                SessionId = sessionId
            }, unauthorized);
        }

        var sessionStatus = await GetSessionStatusAsync(client, sessionId, ct);
        var playerDisplayNamesTask = LoadPlayerDisplayNameMapAsync(client, sessionId, ct);
        var timelineTask = LoadTimelineAsync(client, sessionId, language, ct);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.TryReadFromJsonAsync<ErrorResponse>(cancellationToken: ct);
            var fallbackPlayerDisplayNames = await playerDisplayNamesTask;
            var (fallbackTimeline, fallbackTimelineError) = await timelineTask;
            SessionTimelineMapper.ApplyPlayerDisplayNames(fallbackTimeline, fallbackPlayerDisplayNames);

            return (new SessionDetailViewModel
            {
                SessionId = sessionId,
                SessionStatus = sessionStatus,
                Timeline = fallbackTimeline,
                TimelineErrorMessage = fallbackTimelineError,
                PlayerDisplayNames = fallbackPlayerDisplayNames,
                ErrorMessage = error?.Message ?? HttpContext
                    .T("sessions.error.load_detail_failed")
                    .Replace("{status}", ((int)response.StatusCode).ToString())
            }, null);
        }

        var analytics = await response.Content.TryReadFromJsonAsync<AnalyticsSessionResponse>(cancellationToken: ct);
        if (analytics?.SessionId != sessionId || analytics.Summary is null || analytics.ByPlayer is null)
            analytics = null;
        var playerDisplayNames = await playerDisplayNamesTask;
        var (timeline, timelineError) = await timelineTask;
        SessionTimelineMapper.ApplyPlayerDisplayNames(timeline, playerDisplayNames);
        var activeRulesetDetail = analytics?.RulesetId is Guid rulesetId
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: await LoadActiveRulesetDetailAsync(client, rulesetId, analytics.RulesetVersionId, ct) dalam
            // BuildSessionDetailViewModel.
            ? await LoadActiveRulesetDetailAsync(client, rulesetId, analytics.RulesetVersionId, ct)
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: null; dalam BuildSessionDetailViewModel.
            : null;
        var activeRulesetViewModel = activeRulesetDetail is null
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: null dalam BuildSessionDetailViewModel.
            ? null
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: new RulesetDetailViewModel dalam BuildSessionDetailViewModel.
            : new RulesetDetailViewModel
            {
                Ruleset = activeRulesetDetail,
                CompatibilityDefinitionJson = RulesetDefinitionMapper.ToConfigElement(activeRulesetDetail.Definition),
                IsReadOnly = true,
                IsDefaultCatalogSource = activeRulesetDetail.IsDefault
            };

        return (new SessionDetailViewModel
        {
            SessionId = sessionId,
            SessionStatus = sessionStatus,
            Analytics = analytics,
            ActiveRulesetDetail = activeRulesetViewModel,
            Timeline = timeline,
            TimelineErrorMessage = timelineError,
            PlayerDisplayNames = playerDisplayNames,
            ErrorMessage = analytics is null ? HttpContext.T("sessions.error.invalid_analytics") : null
        }, null);
    }

    private static async Task<RulesetDetailResponse?> LoadActiveRulesetDetailAsync(
        // Parameter `client` bertipe `HttpClient` membawa nilai client.
        HttpClient client,
        // Parameter `rulesetId` bertipe `Guid` membawa identitas kumpulan aturan permainan.
        Guid rulesetId,
        Guid? versionId,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    {
        var response = await client.GetAsync($"api/v1/rulesets/{rulesetId}", ct);
        if (response.IsSuccessStatusCode)
        {
            var detail = await response.Content.TryReadFromJsonAsync<RulesetDetailResponse>(cancellationToken: ct);
            if (detail is not null && versionId.HasValue)
            {
                if (detail.RulesetVersionId == versionId) return detail;
                var version = detail.Versions?.FirstOrDefault(v => v.RulesetVersionId == versionId);
                if (version is null) return null;
                using var componentsResponse = await client.GetAsync($"api/v1/rulesets/{rulesetId}/components?version={version.Version}", ct);
                var components = componentsResponse.IsSuccessStatusCode
                    ? await componentsResponse.Content.TryReadFromJsonAsync<RulesetComponentsResponse>(ct) : null;
                return components?.RulesetVersionId == versionId
                    ? detail with { RulesetVersionId = versionId, Version = components.Version, Mode = components.Mode, Definition = components.Definition }
                    : null;
            }
        }

        var defaultsResponse = await client.GetAsync("api/v1/rulesets/components/defaults", ct);
        if (defaultsResponse.IsSuccessStatusCode)
        {
            var defaultsData = await defaultsResponse.Content.TryReadFromJsonAsync<DefaultRulesetComponentsResponse>(cancellationToken: ct);
            var fallbackItem = defaultsData?.Items?.FirstOrDefault(item => item.RulesetId == rulesetId && item.RulesetVersionId == versionId);
            if (fallbackItem is not null)
            {
                return new RulesetDetailResponse(
                    fallbackItem.RulesetId,
                    fallbackItem.Name,
                    fallbackItem.Description,
                    new List<RulesetVersionItem>
                    {
                        new(fallbackItem.RulesetVersionId, fallbackItem.Version, "ACTIVE", DateTimeOffset.UtcNow)
                    },
                    fallbackItem.RulesetVersionId,
                    fallbackItem.Version,
                    fallbackItem.Mode,
                    fallbackItem.Definition,
                    IsDefault: true,
                    IsLockedBySession: true
                // Menutup daftar argumen atau parameter konstruktor `RulesetDetailResponse`; nilai pada baris sebelumnya melengkapi kontrak pemanggilan/deklarasi
                // ini dalam LoadActiveRulesetDetailAsync.
                );
            }
        }

        return null;
    }

    private static async Task<string?> GetSessionStatusAsync(HttpClient client, Guid sessionId, CancellationToken ct)
    {
        var sessionResponse = await client.GetAsync("api/v1/sessions", ct);
        if (!sessionResponse.IsSuccessStatusCode)
        {
            return null;
        }

        var data = await sessionResponse.Content.TryReadFromJsonAsync<SessionListResponse>(cancellationToken: ct);
        return data?.Items?.FirstOrDefault(x => x.SessionId == sessionId)?.Status;
    }

    private static async Task<Dictionary<Guid, string>> LoadPlayerDisplayNameMapAsync(
        // Parameter `client` bertipe `HttpClient` membawa nilai client.
        HttpClient client,
        Guid sessionId,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    {
        var response = await client.GetAsync($"api/v1/sessions/{sessionId}/players", ct);
        if (!response.IsSuccessStatusCode)
        {
            return new Dictionary<Guid, string>();
        }

        var data = await response.Content.TryReadFromJsonAsync<SessionPlayerListResponse>(cancellationToken: ct);
        return (data?.Items ?? new List<SessionPlayerResponse>())
            .Where(item => !string.IsNullOrWhiteSpace(item.DisplayName))
            .GroupBy(item => item.UserId)
            .ToDictionary(group => group.Key, group => group.First().DisplayName);
    }

    private async Task<(List<SessionTimelineEventViewModel> Timeline, string? ErrorMessage)> LoadTimelineAsync(
        HttpClient client, Guid sessionId, string language, CancellationToken ct)
    {
        var events = new List<EventRequest>();
        var cursors = new HashSet<string>(StringComparer.Ordinal);
        string? cursor = null;
        do
        {
            var cursorQuery = string.IsNullOrWhiteSpace(cursor) ? string.Empty : $"&cursor={Uri.EscapeDataString(cursor)}";
            using var response = await client.GetAsync($"api/v1/sessions/{sessionId}/events?limit=100{cursorQuery}", ct);
            if (!response.IsSuccessStatusCode)
                return ([], HttpContext.T("sessions.error.load_timeline_failed")
                    .Replace("{status}", ((int)response.StatusCode).ToString()));

            var page = await response.Content.TryReadFromJsonAsync<EventsBySessionResponse>(ct);
            if (page?.Items is null || (page.HasMore && (string.IsNullOrWhiteSpace(page.NextCursor) || !cursors.Add(page.NextCursor))))
                return ([], HttpContext.T("sessions.error.timeline_incomplete"));
            events.AddRange(page.Items);
            cursor = page.HasMore ? page.NextCursor : null;
        }
        while (!string.IsNullOrWhiteSpace(cursor));
        return (SessionTimelineMapper.MapTimeline(events, language), null);
    }
}
