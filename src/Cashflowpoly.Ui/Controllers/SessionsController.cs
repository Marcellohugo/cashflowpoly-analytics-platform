// Fungsi file: Menangani request MVC dan penyusunan tampilan untuk SessionsController.
using System.Net;
using System.Net.Http.Json;
using Cashflowpoly.Ui.Contracts;
using Cashflowpoly.Ui.Domain;
using Cashflowpoly.Ui.Infrastructure;
using Cashflowpoly.Ui.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cashflowpoly.Ui.Controllers;

[Route("sessions")]
public sealed class SessionsController : Controller
{
    private readonly IHttpClientFactory _clientFactory;

    public SessionsController(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken ct)
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
        return View(new SessionListViewModel
        {
            Items = data?.Items ?? new List<SessionListItem>()
        });
    }

    [HttpGet("{sessionId:guid}")]
    public async Task<IActionResult> Details(Guid sessionId, CancellationToken ct)
    {
        var detail = await BuildSessionDetailViewModel(sessionId, ct);
        return detail.Result ?? View(detail.Model);
    }

    [HttpGet("{sessionId:guid}/timeline")]
    public async Task<IActionResult> Timeline(
        Guid sessionId,
        [FromQuery] string? cursor = null,
        [FromQuery] int limit = 100,
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
        var language = UiText.NormalizeLanguage(HttpContext.Session.GetString(AuthConstants.SessionLanguageKey));
        var timeline = SessionTimelineMapper.MapTimeline(data?.Items, language);
        var playerDisplayNames = await LoadPlayerDisplayNameMapAsync(client, ct);
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
        Guid sessionId,
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
        var playerDisplayNamesTask = LoadPlayerDisplayNameMapAsync(client, ct);
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
        var playerDisplayNames = await playerDisplayNamesTask;
        var (timeline, timelineError) = await timelineTask;
        SessionTimelineMapper.ApplyPlayerDisplayNames(timeline, playerDisplayNames);
        var activeRulesetDetail = analytics?.RulesetId is Guid rulesetId
            ? await LoadActiveRulesetDetailAsync(client, rulesetId, ct)
            : null;
        var activeRulesetViewModel = activeRulesetDetail is null
            ? null
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
            ErrorMessage = null
        }, null);
    }

    private static async Task<RulesetDetailResponse?> LoadActiveRulesetDetailAsync(
        HttpClient client,
        Guid rulesetId,
        CancellationToken ct)
    {
        var response = await client.GetAsync($"api/v1/rulesets/{rulesetId}", ct);
        if (response.IsSuccessStatusCode)
        {
            var detail = await response.Content.TryReadFromJsonAsync<RulesetDetailResponse>(cancellationToken: ct);
            if (detail is not null)
            {
                return detail;
            }
        }

        var defaultsResponse = await client.GetAsync("api/v1/rulesets/components/defaults", ct);
        if (defaultsResponse.IsSuccessStatusCode)
        {
            var defaultsData = await defaultsResponse.Content.TryReadFromJsonAsync<DefaultRulesetComponentsResponse>(cancellationToken: ct);
            var fallbackItem = defaultsData?.Items?.FirstOrDefault(item => item.RulesetId == rulesetId);
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
        return data?.Items.FirstOrDefault(x => x.SessionId == sessionId)?.Status;
    }

    private static async Task<Dictionary<Guid, string>> LoadPlayerDisplayNameMapAsync(
        HttpClient client,
        CancellationToken ct)
    {
        var response = await client.GetAsync("api/v1/players", ct);
        if (!response.IsSuccessStatusCode)
        {
            return new Dictionary<Guid, string>();
        }

        var data = await response.Content.TryReadFromJsonAsync<PlayerListResponse>(cancellationToken: ct);
        return (data?.Items ?? new List<PlayerResponse>())
            .Where(item => !string.IsNullOrWhiteSpace(item.DisplayName))
            .GroupBy(item => item.UserId)
            .ToDictionary(group => group.Key, group => group.First().DisplayName);
    }

    private async Task<(List<SessionTimelineEventViewModel> Timeline, string? ErrorMessage)> LoadTimelineAsync(
        HttpClient client,
        Guid sessionId,
        string language,
        CancellationToken ct)
    {
        var events = new List<EventRequest>();
        string? cursor = null;
        do
        {
            var cursorQuery = string.IsNullOrWhiteSpace(cursor) ? string.Empty : $"&cursor={Uri.EscapeDataString(cursor)}";
            var response = await client.GetAsync($"api/v1/sessions/{sessionId}/events?limit=100{cursorQuery}", ct);
            if (!response.IsSuccessStatusCode)
            {
                return (
                    new List<SessionTimelineEventViewModel>(),
                    HttpContext.T("sessions.error.load_timeline_failed")
                        .Replace("{status}", ((int)response.StatusCode).ToString()));
            }

            var page = await response.Content.TryReadFromJsonAsync<EventsBySessionResponse>(cancellationToken: ct);
            if (page is null)
            {
                break;
            }

            events.AddRange(page.Items);
            cursor = page.HasMore ? page.NextCursor : null;
        }
        while (!string.IsNullOrWhiteSpace(cursor));

        if (events.Count == 0)
        {
            return (new List<SessionTimelineEventViewModel>(), null);
        }

        var timeline = SessionTimelineMapper.MapTimeline(events, language);
        return (timeline, null);
    }

}
