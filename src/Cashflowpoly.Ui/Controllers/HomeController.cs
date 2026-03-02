// Fungsi file: Menangani permintaan HTTP untuk halaman beranda, termasuk statistik realtime, halaman rulebook, dan penanganan error.
using System.Diagnostics;
using Cashflowpoly.Ui.Infrastructure;
using Cashflowpoly.Ui.Models;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Cashflowpoly.Ui.Controllers;

/// <summary>
/// Controller MVC yang mengelola halaman beranda aplikasi, menampilkan statistik realtime
/// (jumlah sesi, pemain, ruleset), halaman rulebook, dan halaman error.
/// </summary>
public class HomeController : Controller
{
    /// <summary>
    /// Durasi cache statistik realtime beranda (20 detik) untuk mengurangi beban panggilan API.
    /// </summary>
    private static readonly TimeSpan RealtimeStatsCacheDuration = TimeSpan.FromSeconds(20);

    private readonly IHttpClientFactory _clientFactory;
    private readonly IMemoryCache _memoryCache;

    /// <summary>
    /// Menginisialisasi controller beranda dengan factory HTTP client dan cache memori.
    /// </summary>
    /// <param name="clientFactory">Factory untuk membuat instance <see cref="HttpClient"/> ke API backend.</param>
    /// <param name="memoryCache">Cache memori untuk menyimpan statistik realtime sementara.</param>
    public HomeController(IHttpClientFactory clientFactory, IMemoryCache memoryCache)
    {
        _clientFactory = clientFactory;
        _memoryCache = memoryCache;
    }

    /// <summary>
    /// Menampilkan halaman beranda dengan statistik realtime jumlah sesi, pemain, dan ruleset.
    /// </summary>
    /// <param name="ct">Token pembatalan untuk membatalkan permintaan.</param>
    /// <returns>View beranda dengan data statistik realtime.</returns>
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
    /// Mengembalikan statistik realtime dalam format JSON untuk pembaruan dinamis di halaman beranda.
    /// </summary>
    /// <param name="ct">Token pembatalan untuk membatalkan permintaan.</param>
    /// <returns>Objek JSON berisi jumlah sesi aktif, total sesi, pemain, ruleset, dan waktu sinkronisasi.</returns>
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
    /// Menampilkan halaman panduan aturan permainan (rulebook) sesuai bahasa yang dipilih pengguna.
    /// </summary>
    /// <returns>View berisi konten rulebook dalam bahasa yang sesuai.</returns>
    public IActionResult Rulebook()
    {
        var language = UiText.NormalizeLanguage(HttpContext.Session.GetString(AuthConstants.SessionLanguageKey));
        return View("Privacy", model: RulebookContent.Build(language));
    }

    /// <summary>
    /// Mengarahkan halaman Privacy ke halaman Rulebook sebagai pengganti.
    /// </summary>
    /// <returns>Redirect ke action Rulebook.</returns>
    public IActionResult Privacy()
    {
        return RedirectToAction(nameof(Rulebook));
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    /// <summary>
    /// Menampilkan halaman error generik dengan informasi request ID untuk pelacakan.
    /// </summary>
    /// <returns>View error dengan ID permintaan saat ini.</returns>
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    /// <summary>
    /// Mengambil statistik realtime dari cache atau dari API backend secara paralel (sesi, pemain, ruleset).
    /// </summary>
    /// <param name="ct">Token pembatalan untuk membatalkan permintaan.</param>
    /// <returns>Tuple berisi ViewModel statistik beranda dan hasil unauthorized jika ada.</returns>
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
            var data = await sessionsResponse.Content.TryReadFromJsonAsync<SessionListResponseDto>(cancellationToken: ct);
            sessions = data?.Items ?? new List<SessionListItemDto>();
        }
        else
        {
            errorMessages.Add($"sessions:{(int)sessionsResponse.StatusCode}");
        }

        if (playersResponse.IsSuccessStatusCode)
        {
            var data = await playersResponse.Content.TryReadFromJsonAsync<PlayerListResponseDto>(cancellationToken: ct);
            players = data?.Items ?? new List<PlayerResponseDto>();
        }
        else
        {
            errorMessages.Add($"players:{(int)playersResponse.StatusCode}");
        }

        if (rulesetsResponse.IsSuccessStatusCode)
        {
            var data = await rulesetsResponse.Content.TryReadFromJsonAsync<RulesetListResponseDto>(cancellationToken: ct);
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

        if (errorMessages.Count == 0)
        {
            _memoryCache.Set(cacheKey, model, RealtimeStatsCacheDuration);
        }

        return (model, null);
    }

    /// <summary>
    /// Membangun kunci cache unik berdasarkan ID sesi HTTP, pengguna, dan peran untuk isolasi data statistik.
    /// </summary>
    /// <returns>String kunci cache yang unik per konteks pengguna.</returns>
    private string BuildRealtimeStatsCacheKey()
    {
        var sessionId = HttpContext.Session.Id;
        var userId = HttpContext.Session.GetString(AuthConstants.SessionUserIdKey);
        var role = HttpContext.Session.GetString(AuthConstants.SessionRoleKey);
        var sessionScope = string.IsNullOrWhiteSpace(sessionId) ? "anonymous-session" : sessionId;
        var userScope = string.IsNullOrWhiteSpace(userId) ? "anonymous-user" : userId.Trim();
        var roleScope = string.IsNullOrWhiteSpace(role) ? "unknown-role" : role.Trim().ToUpperInvariant();
        return $"home:realtime:{sessionScope}:{userScope}:{roleScope}";
    }
}
