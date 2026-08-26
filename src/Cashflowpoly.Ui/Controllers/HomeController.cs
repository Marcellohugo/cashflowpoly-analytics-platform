// Fungsi file: Menangani request MVC dan penyusunan tampilan untuk HomeController.
using System.Diagnostics;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Security;
using Cashflowpoly.Ui.Contracts;
using Cashflowpoly.Ui.Infrastructure;
using Cashflowpoly.Ui.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Cashflowpoly.Ui.Controllers;

public class HomeController : Controller
{
    private static readonly TimeSpan RealtimeStatsCacheDuration = TimeSpan.FromSeconds(20);

    private readonly IHttpClientFactory _clientFactory;
    private readonly IMemoryCache _memoryCache;
    private readonly IConfiguration _configuration;

    public HomeController(
        IHttpClientFactory clientFactory,
        IMemoryCache memoryCache,
        IConfiguration configuration)
    {
        _clientFactory = clientFactory;
        _memoryCache = memoryCache;
        _configuration = configuration;
    }

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var statsResult = await GetRealtimeStatsInternal(ct);
        if (statsResult.UnauthorizedResult is not null)
        {
            return statsResult.UnauthorizedResult;
        }

        var model = statsResult.Model;

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> RealtimeStats(CancellationToken ct)
    {
        var statsResult = await GetRealtimeStatsInternal(ct);
        if (statsResult.UnauthorizedResult is not null)
        {
            return statsResult.UnauthorizedResult;
        }

        var model = statsResult.Model;
        return Json(new
        {
            activeSessions = model.ActiveSessions,
            totalSessions = model.TotalSessions,
            totalPlayers = model.TotalPlayers,
            totalRulesets = model.TotalRulesets,
            lastSyncedAt = model.LastSyncedAt,
            errorMessage = model.ErrorMessage
        });
    }

    public IActionResult Rulebook()
    {
        var requestPath = HttpContext.Request.Path.Value?.TrimEnd('/');
        if (string.Equals(requestPath, "/Home/Rulebook", StringComparison.OrdinalIgnoreCase))
        {
            return Redirect("/rulebook");
        }

        var language = UiText.NormalizeLanguage(HttpContext.Session.GetString(AuthConstants.SessionLanguageKey));
        return View("Privacy", model: RulebookContent.Build(language));
    }

    public IActionResult Privacy()
    {
        return View("PrivacyPolicy");
    }

    public IActionResult Terms()
    {
        return View();
    }

    [HttpGet("/robots.txt")]
    public IActionResult Robots()
    {
        var baseUrl = SiteUrlResolver.ResolveBaseUrl(_configuration, Request);
        var content = $"User-agent: *\nAllow: /rulebook\nAllow: /privacy\nAllow: /terms\nDisallow: /auth/\nDisallow: /sessions/\nDisallow: /players/\nDisallow: /rulesets/\nSitemap: {baseUrl}/sitemap.xml\n";
        return Content(content, "text/plain");
    }

    [HttpGet("/sitemap.xml")]
    public IActionResult Sitemap()
    {
        var baseUrl = SecurityElement.Escape(SiteUrlResolver.ResolveBaseUrl(_configuration, Request));
        var content = $"<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n" +
            $"<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">\n" +
            $"  <url><loc>{baseUrl}/</loc></url>\n" +
            $"  <url><loc>{baseUrl}/rulebook</loc></url>\n" +
            $"</urlset>\n";
        return Content(content, "application/xml");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    private async Task<(HomeIndexViewModel Model, IActionResult? UnauthorizedResult)> GetRealtimeStatsInternal(CancellationToken ct)
    {
        var cacheKey = BuildRealtimeStatsCacheKey();
        if (_memoryCache.TryGetValue(cacheKey, out HomeIndexViewModel? cachedModel) && cachedModel is not null)
        {
            return (cachedModel, null);
        }

        var client = _clientFactory.CreateClient("Api");

        var sessionsTask = client.GetAsync("api/v1/sessions", ct);
        var playersTask = client.GetAsync("api/v1/players", ct);
        var rulesetsTask = client.GetAsync("api/v1/rulesets", ct);
        await Task.WhenAll(sessionsTask, playersTask, rulesetsTask);

        var sessionsResponse = sessionsTask.Result;
        var playersResponse = playersTask.Result;
        var rulesetsResponse = rulesetsTask.Result;

        var unauthorized = this.HandleUnauthorizedApiResponse(sessionsResponse)
                          ?? this.HandleUnauthorizedApiResponse(playersResponse)
                          ?? this.HandleUnauthorizedApiResponse(rulesetsResponse);
        if (unauthorized is not null)
        {
            return (new HomeIndexViewModel(), unauthorized);
        }

        var errorMessages = new List<string>();
        var sessions = new List<SessionListItem>();
        var players = new List<PlayerResponse>();
        var rulesets = new List<RulesetListItem>();

        if (sessionsResponse.IsSuccessStatusCode)
        {
            var data = await sessionsResponse.Content.TryReadFromJsonAsync<SessionListResponse>(cancellationToken: ct);
            sessions = data?.Items ?? new List<SessionListItem>();
        }
        else
        {
            errorMessages.Add($"sessions:{(int)sessionsResponse.StatusCode}");
        }

        if (playersResponse.IsSuccessStatusCode)
        {
            var data = await playersResponse.Content.TryReadFromJsonAsync<PlayerListResponse>(cancellationToken: ct);
            players = data?.Items ?? new List<PlayerResponse>();
        }
        else
        {
            errorMessages.Add($"players:{(int)playersResponse.StatusCode}");
        }

        if (rulesetsResponse.IsSuccessStatusCode)
        {
            var data = await rulesetsResponse.Content.TryReadFromJsonAsync<RulesetListResponse>(cancellationToken: ct);
            rulesets = data?.Items ?? new List<RulesetListItem>();
        }
        else
        {
            errorMessages.Add($"rulesets:{(int)rulesetsResponse.StatusCode}");
        }

        var model = new HomeIndexViewModel
        {
            TotalSessions = sessions.Count,
            ActiveSessions = sessions.Count(s => string.Equals(s.Status, "STARTED", StringComparison.OrdinalIgnoreCase)),
            TotalPlayers = players.Count,
            TotalRulesets = rulesets.Count(r => string.Equals(r.Status, "ACTIVE", StringComparison.OrdinalIgnoreCase)),
            LastSyncedAt = DateTimeOffset.UtcNow,
            ErrorMessage = errorMessages.Count == 0
                ? null
                : HttpContext.T("home.error.partial_realtime_failed")
                    .Replace("{details}", string.Join(", ", errorMessages))
        };

        if (errorMessages.Count == 0)
        {
            _memoryCache.Set(cacheKey, model, RealtimeStatsCacheDuration);
        }

        return (model, null);
    }

    private string BuildRealtimeStatsCacheKey()
    {
        var sessionId = HttpContext.Session.Id;
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var role = User.FindFirst(ClaimTypes.Role)?.Value;
        var sessionScope = string.IsNullOrWhiteSpace(sessionId) ? "anonymous-session" : sessionId;
        var userScope = string.IsNullOrWhiteSpace(userId) ? "anonymous-user" : userId.Trim();
        var roleScope = string.IsNullOrWhiteSpace(role) ? "unknown-role" : role.Trim().ToUpperInvariant();
        return $"home:realtime:{sessionScope}:{userScope}:{roleScope}";
    }
}
