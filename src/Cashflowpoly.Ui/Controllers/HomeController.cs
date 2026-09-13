// Fungsi file: Menangani request MVC dan penyusunan tampilan untuk HomeController.
// Mengimpor namespace `System.Diagnostics` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Diagnostics;
// Mengimpor namespace `System.Net.Http.Json` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Net.Http.Json;
// Mengimpor namespace `System.Security.Claims` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Security.Claims;
// Mengimpor namespace `System.Security` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Security;
// Mengimpor namespace `Cashflowpoly.Ui.Contracts` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Ui.Contracts;
// Mengimpor namespace `Cashflowpoly.Ui.Infrastructure` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Ui.Infrastructure;
// Mengimpor namespace `Cashflowpoly.Ui.Models` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Ui.Models;
// Mengimpor namespace `Microsoft.AspNetCore.Mvc` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.AspNetCore.Mvc;
// Mengimpor namespace `Microsoft.Extensions.Caching.Memory` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.Extensions.Caching.Memory;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Controllers` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Controllers;

public class HomeController : Controller
{
    private static readonly TimeSpan RealtimeStatsCacheDuration = TimeSpan.FromSeconds(20);

    private readonly IHttpClientFactory _clientFactory;
    private readonly IMemoryCache _memoryCache;
    private readonly IConfiguration _configuration;

    public HomeController(
        // Parameter `clientFactory` bertipe `IHttpClientFactory` membawa nilai client factory.
        IHttpClientFactory clientFactory,
        // Parameter `memoryCache` bertipe `IMemoryCache` membawa nilai memory cache.
        IMemoryCache memoryCache,
        // Parameter `configuration` bertipe `IConfiguration` membawa konfigurasi aplikasi yang menyediakan nilai pengaturan dari sumber terdaftar.
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

    // mendaftarkan action untuk metode HTTP GET pada rute controller saat ini.
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

    // mendaftarkan action untuk metode HTTP GET pada rute (”/robots.txt”).
    [HttpGet("/robots.txt")]
    public IActionResult Robots()
    {
        var baseUrl = SiteUrlResolver.ResolveBaseUrl(_configuration, Request);
        var content = $"User-agent: *\nAllow: /rulebook\nAllow: /privacy\nAllow: /terms\nDisallow: /auth/\nDisallow: /sessions/\nDisallow: /players/\nDisallow: /rulesets/\nSitemap: {baseUrl}/sitemap.xml\n";
        return Content(content, "text/plain");
    }

    // mendaftarkan action untuk metode HTTP GET pada rute (”/sitemap.xml”).
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

    // menerapkan metadata `ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)` pada deklarasi berikut agar
    // framework/compiler dapat mengenali pengaturannya.
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
        var errors = new System.Collections.Concurrent.ConcurrentBag<string>();
        var expired = false;
        async Task<T?> LoadAsync<T>(string path) where T : class
        {
            try
            {
                using var response = await client.GetAsync(path, ct);
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized) expired = true;
                var value = response.IsSuccessStatusCode
                    ? await response.Content.TryReadFromJsonAsync<T>(cancellationToken: ct) : null;
                if (value is null) errors.Add(path.Split('?')[0].Split('/').Last());
                return value;
            }
            catch (HttpRequestException) { errors.Add(path.Split('?')[0].Split('/').Last()); return null; }
            catch (TaskCanceledException) when (!ct.IsCancellationRequested)
            { errors.Add(path.Split('?')[0].Split('/').Last()); return null; }
        }

        var sessionsTask = LoadAsync<SessionListResponse>("api/v1/sessions");
        var playersTask = LoadAsync<PlayerListResponse>(HttpContext.IsInstructor()
            ? "api/v1/players?inMySessions=true" : "api/v1/players");
        var rulesetsTask = LoadAsync<RulesetListResponse>("api/v1/rulesets");
        await Task.WhenAll(sessionsTask, playersTask, rulesetsTask);
        if (expired)
        {
            using var response = new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
            return (new HomeIndexViewModel(), this.HandleUnauthorizedApiResponse(response));
        }

        var sessions = sessionsTask.Result?.Items;
        var players = playersTask.Result?.Items;
        var rulesets = rulesetsTask.Result?.Items;
        if (sessions is null && sessionsTask.Result is not null) errors.Add("sessions");
        if (players is null && playersTask.Result is not null) errors.Add("players");
        if (rulesets is null && rulesetsTask.Result is not null) errors.Add("rulesets");
        var model = new HomeIndexViewModel
        {
            TotalSessions = sessions?.Count,
            ActiveSessions = sessions?.Count(s => string.Equals(s.Status, "STARTED", StringComparison.OrdinalIgnoreCase)),
            // The player-list endpoint includes the account itself even before it joins a session.
            TotalPlayers = !HttpContext.IsInstructor() && sessions is null ? null
                : sessions?.Count == 0 ? 0 : players?.Select(p => p.UserId).Distinct().Count(),
            TotalRulesets = rulesets?.Count(r => string.Equals(r.Status, "ACTIVE", StringComparison.OrdinalIgnoreCase)),
            LastSyncedAt = DateTimeOffset.UtcNow,
            ErrorMessage = errors.IsEmpty ? null : HttpContext.T("home.error.partial_realtime_failed")
                .Replace("{details}", string.Join(", ", errors.Distinct().Order()))
        };
        if (errors.IsEmpty) _memoryCache.Set(cacheKey, model, RealtimeStatsCacheDuration);
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
