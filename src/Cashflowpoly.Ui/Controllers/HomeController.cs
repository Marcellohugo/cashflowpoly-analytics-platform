// Fungsi file: Mengelola alur halaman UI untuk domain HomeController termasuk komunikasi ke API backend.
using System.Diagnostics;
using Cashflowpoly.Ui.Infrastructure;
using Cashflowpoly.Ui.Models;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Cashflowpoly.Ui.Controllers;

/// <summary>
/// Menyatakan peran utama tipe HomeController pada modul ini.
/// </summary>
public class HomeController : Controller
{
    /// <summary>
    /// Menjalankan fungsi FromSeconds sebagai bagian dari alur file ini.
    /// </summary>
    private static readonly TimeSpan RealtimeStatsCacheDuration = TimeSpan.FromSeconds(20);

    private readonly IHttpClientFactory _clientFactory;
    private readonly IMemoryCache _memoryCache;

    /// <summary>
    /// Menjalankan fungsi HomeController sebagai bagian dari alur file ini.
    /// </summary>
    public HomeController(IHttpClientFactory clientFactory, IMemoryCache memoryCache)
    {
        _clientFactory = clientFactory;
        _memoryCache = memoryCache;
    }

    /// <summary>
    /// Menjalankan fungsi Index sebagai bagian dari alur file ini.
    /// </summary>
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
    /// <summary>
    /// Menjalankan fungsi RealtimeStats sebagai bagian dari alur file ini.
    /// </summary>
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

    /// <summary>
    /// Menjalankan fungsi Rulebook sebagai bagian dari alur file ini.
    /// </summary>
    public IActionResult Rulebook()
    {
        var language = UiText.NormalizeLanguage(HttpContext.Session.GetString(AuthConstants.SessionLanguageKey));
        return View("Privacy", model: RulebookContent.Build(language));
    }

    /// <summary>
    /// Menjalankan fungsi Privacy sebagai bagian dari alur file ini.
    /// </summary>
    public IActionResult Privacy()
    {
        return RedirectToAction(nameof(Rulebook));
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    /// <summary>
    /// Menjalankan fungsi Error sebagai bagian dari alur file ini.
    /// </summary>
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    /// <summary>
    /// Menjalankan fungsi GetRealtimeStatsInternal sebagai bagian dari alur file ini.
    /// </summary>
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
        var sessions = new List<SessionListItemDto>();
        var players = new List<PlayerResponseDto>();
        var rulesets = new List<RulesetListItemDto>();

        if (sessionsResponse.IsSuccessStatusCode)
        {
            var data = await sessionsResponse.Content.ReadFromJsonAsync<SessionListResponseDto>(cancellationToken: ct);
            sessions = data?.Items ?? new List<SessionListItemDto>();
        }
        else
        {
            errorMessages.Add($"sessions:{(int)sessionsResponse.StatusCode}");
        }

        if (playersResponse.IsSuccessStatusCode)
        {
            var data = await playersResponse.Content.ReadFromJsonAsync<PlayerListResponseDto>(cancellationToken: ct);
            players = data?.Items ?? new List<PlayerResponseDto>();
        }
        else
        {
            errorMessages.Add($"players:{(int)playersResponse.StatusCode}");
        }

        if (rulesetsResponse.IsSuccessStatusCode)
        {
            var data = await rulesetsResponse.Content.ReadFromJsonAsync<RulesetListResponseDto>(cancellationToken: ct);
            rulesets = data?.Items ?? new List<RulesetListItemDto>();
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
            TotalRulesets = rulesets.Count,
            LastSyncedAt = DateTimeOffset.UtcNow,
            ErrorMessage = errorMessages.Count == 0
                ? null
                : HttpContext.T("home.error.partial_realtime_failed")
                    .Replace("{details}", string.Join(", ", errorMessages))
        };

        _memoryCache.Set(cacheKey, model, RealtimeStatsCacheDuration);

        return (model, null);
    }

    /// <summary>
    /// Menjalankan fungsi BuildRealtimeStatsCacheKey sebagai bagian dari alur file ini.
    /// </summary>
    private string BuildRealtimeStatsCacheKey()
    {
        var sessionId = HttpContext.Session.Id;
        return string.IsNullOrWhiteSpace(sessionId)
            ? "home:realtime:anonymous"
            : $"home:realtime:{sessionId}";
    }
}
