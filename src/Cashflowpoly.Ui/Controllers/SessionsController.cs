// Fungsi file: Menangani permintaan HTTP untuk halaman sesi permainan, termasuk daftar sesi, detail analitik, timeline event, dan manajemen aktivasi ruleset pada sesi.
using System.Net.Http.Json;
using Cashflowpoly.Ui.Infrastructure;
using Cashflowpoly.Ui.Models;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Cashflowpoly.Ui.Controllers;

/// <summary>
/// Controller MVC yang mengelola tampilan dan interaksi halaman sesi permainan,
/// termasuk daftar sesi, detail analitik per sesi, timeline event, dan aktivasi ruleset.
/// </summary>
[Route("sessions")]
public sealed class SessionsController : Controller
{
    private readonly IHttpClientFactory _clientFactory;

    /// <summary>
    /// Menginisialisasi controller sesi dengan factory HTTP client untuk komunikasi ke API backend.
    /// </summary>
    /// <param name="clientFactory">Factory untuk membuat instance <see cref="HttpClient"/> ke API backend.</param>
    public SessionsController(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    [HttpGet("")]
    /// <summary>
    /// Menampilkan halaman daftar semua sesi permainan yang tersedia.
    /// </summary>
    /// <param name="ct">Token pembatalan untuk membatalkan permintaan.</param>
    /// <returns>View berisi daftar sesi permainan atau pesan kesalahan jika gagal memuat data.</returns>
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

        var data = await response.Content.TryReadFromJsonAsync<SessionListResponseDto>(cancellationToken: ct);
        return View(new SessionListViewModel
        {
            Items = data?.Items ?? new List<SessionListItemDto>()
        });
    }

    [HttpGet("{sessionId:guid}")]
    /// <summary>
    /// Menampilkan halaman detail analitik untuk satu sesi permainan tertentu.
    /// </summary>
    /// <param name="sessionId">Identifier unik sesi permainan.</param>
    /// <param name="ct">Token pembatalan untuk membatalkan permintaan.</param>
    /// <returns>View berisi detail analitik sesi, termasuk statistik pemain dan timeline event.</returns>
    public async Task<IActionResult> Details(Guid sessionId, CancellationToken ct)
    {
        var detail = await BuildSessionDetailViewModel(sessionId, ct);
        return detail.Result ?? View(detail.Model);
    }

    [HttpGet("{sessionId:guid}/timeline")]
    /// <summary>
    /// Mengambil data timeline event sesi dalam format JSON untuk pembaruan dinamis di halaman detail.
    /// </summary>
    /// <param name="sessionId">Identifier unik sesi permainan.</param>
    /// <param name="fromSeq">Nomor urut event awal untuk paginasi (default: 0).</param>
    /// <param name="limit">Jumlah maksimum event yang diambil (default: 300, maks: 1000).</param>
    /// <param name="ct">Token pembatalan untuk membatalkan permintaan.</param>
    /// <returns>Objek JSON berisi daftar event timeline dan metadata sinkronisasi.</returns>
    public async Task<IActionResult> Timeline(
        Guid sessionId,
        [FromQuery] long fromSeq = 0,
        [FromQuery] int limit = 300,
        CancellationToken ct = default)
    {
        var normalizedLimit = Math.Clamp(limit, 1, 1000);
        var client = _clientFactory.CreateClient("Api");
        var response = await client.GetAsync($"api/v1/sessions/{sessionId}/events?fromSeq={fromSeq}&limit={normalizedLimit}", ct);
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

        var data = await response.Content.TryReadFromJsonAsync<EventsBySessionResponseDto>(cancellationToken: ct);
        var language = UiText.NormalizeLanguage(HttpContext.Session.GetString(AuthConstants.SessionLanguageKey));
        var timeline = SessionTimelineMapper.MapTimeline(data?.Events, language);
        var playerDisplayNames = await LoadPlayerDisplayNameMapAsync(client, ct);
        SessionTimelineMapper.ApplyPlayerDisplayNames(timeline, playerDisplayNames);

        return Json(new
        {
            timeline,
            errorMessage = (string?)null,
            lastSyncedAt = DateTimeOffset.UtcNow
        });
    }

    [HttpGet("{sessionId:guid}/ruleset")]
    /// <summary>
    /// Menampilkan formulir pemilihan ruleset untuk diaktifkan pada sesi tertentu (hanya instruktur).
    /// </summary>
    /// <param name="sessionId">Identifier unik sesi permainan.</param>
    /// <param name="ct">Token pembatalan untuk membatalkan permintaan.</param>
    /// <returns>View berisi daftar ruleset yang tersedia untuk dipilih.</returns>
    public async Task<IActionResult> Ruleset(Guid sessionId, CancellationToken ct)
    {
        if (!HttpContext.Session.IsInstructor())
        {
            return RedirectToAction(nameof(Details), new { sessionId });
        }

        var client = _clientFactory.CreateClient("Api");
        var rulesetOptions = await LoadRulesetOptionsAsync(client, ct);
        if (rulesetOptions.UnauthorizedResult is not null)
        {
            return rulesetOptions.UnauthorizedResult;
        }
        return View(new SessionRulesetViewModel
        {
            SessionId = sessionId,
            Rulesets = rulesetOptions.Rulesets,
            ErrorMessage = rulesetOptions.ErrorMessage
        });
    }

    [HttpPost("{sessionId:guid}/ruleset")]
    /// <summary>
    /// Memproses pengiriman formulir aktivasi ruleset pada sesi tertentu (hanya instruktur).
    /// </summary>
    /// <param name="sessionId">Identifier unik sesi permainan.</param>
    /// <param name="model">ViewModel berisi ID ruleset dan versi yang dipilih.</param>
    /// <param name="ct">Token pembatalan untuk membatalkan permintaan.</param>
    /// <returns>Redirect ke halaman detail sesi jika berhasil, atau formulir dengan pesan kesalahan.</returns>
    public async Task<IActionResult> Ruleset(Guid sessionId, SessionRulesetViewModel model, CancellationToken ct)
    {
        if (!HttpContext.Session.IsInstructor())
        {
            return RedirectToAction(nameof(Details), new { sessionId });
        }

        model.SessionId = sessionId;

        if (model.SelectedRulesetId is null || model.SelectedVersion is null)
        {
            return await BuildRulesetFormErrorResult(
                sessionId,
                model,
                HttpContext.T("sessions.error.ruleset_version_required"),
                ct);
        }

        var client = _clientFactory.CreateClient("Api");
        var payload = new
        {
            ruleset_id = model.SelectedRulesetId,
            version = model.SelectedVersion
        };

        var response = await client.PostAsJsonAsync($"api/v1/sessions/{sessionId}/ruleset/activate", payload, ct);
        var unauthorized = this.HandleUnauthorizedApiResponse(response);
        if (unauthorized is not null)
        {
            return unauthorized;
        }

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.TryReadFromJsonAsync<ApiErrorResponseDto>(cancellationToken: ct);
            return await BuildRulesetFormErrorResult(
                sessionId,
                model,
                error?.Message ?? HttpContext
                    .T("sessions.error.activate_ruleset_failed")
                    .Replace("{status}", ((int)response.StatusCode).ToString()),
                ct);
        }

        return RedirectToAction(nameof(Details), new { sessionId });
    }

    /// <summary>
    /// Menyusun kembali view formulir ruleset dengan pesan kesalahan dan memuat ulang daftar ruleset yang tersedia.
    /// </summary>
    /// <param name="sessionId">Identifier unik sesi permainan.</param>
    /// <param name="model">ViewModel formulir yang dikembalikan dengan pesan kesalahan.</param>
    /// <param name="errorMessage">Pesan kesalahan yang akan ditampilkan ke pengguna.</param>
    /// <param name="ct">Token pembatalan untuk membatalkan permintaan.</param>
    /// <returns>View formulir ruleset dengan pesan kesalahan.</returns>
    private async Task<IActionResult> BuildRulesetFormErrorResult(
        Guid sessionId,
        SessionRulesetViewModel model,
        string errorMessage,
        CancellationToken ct)
    {
        model.SessionId = sessionId;
        model.ErrorMessage = errorMessage;

        var client = _clientFactory.CreateClient("Api");
        var rulesetOptions = await LoadRulesetOptionsAsync(client, ct);
        if (rulesetOptions.UnauthorizedResult is not null)
        {
            return rulesetOptions.UnauthorizedResult;
        }

        model.Rulesets = rulesetOptions.Rulesets;
        if (!string.IsNullOrWhiteSpace(rulesetOptions.ErrorMessage))
        {
            model.ErrorMessage = string.IsNullOrWhiteSpace(model.ErrorMessage)
                ? rulesetOptions.ErrorMessage
                : $"{model.ErrorMessage} {rulesetOptions.ErrorMessage}";
        }

        return View(model);
    }

    /// <summary>
    /// Membangun ViewModel detail sesi dengan mengambil data analitik, status sesi,
    /// daftar pemain, dan timeline event secara paralel dari API backend.
    /// </summary>
    /// <param name="sessionId">Identifier unik sesi permainan.</param>
    /// <param name="ct">Token pembatalan untuk membatalkan permintaan.</param>
    /// <param name="overrideErrorMessage">Pesan kesalahan opsional yang menggantikan pesan dari API.</param>
    /// <returns>Tuple berisi ViewModel detail sesi dan hasil redirect jika terjadi unauthorized.</returns>
    private async Task<(SessionDetailViewModel Model, IActionResult? Result)> BuildSessionDetailViewModel(
        Guid sessionId,
        CancellationToken ct,
        string? overrideErrorMessage = null)
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
            var error = await response.Content.TryReadFromJsonAsync<ApiErrorResponseDto>(cancellationToken: ct);
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
                ErrorMessage = overrideErrorMessage ?? error?.Message ?? HttpContext
                    .T("sessions.error.load_detail_failed")
                    .Replace("{status}", ((int)response.StatusCode).ToString())
            }, null);
        }

        var analytics = await response.Content.TryReadFromJsonAsync<AnalyticsSessionResponseDto>(cancellationToken: ct);
        var playerDisplayNames = await playerDisplayNamesTask;
        var (timeline, timelineError) = await timelineTask;
        SessionTimelineMapper.ApplyPlayerDisplayNames(timeline, playerDisplayNames);

        return (new SessionDetailViewModel
        {
            SessionId = sessionId,
            SessionStatus = sessionStatus,
            Analytics = analytics,
            Timeline = timeline,
            TimelineErrorMessage = timelineError,
            PlayerDisplayNames = playerDisplayNames,
            ErrorMessage = overrideErrorMessage
        }, null);
    }

    /// <summary>
    /// Mengambil status sesi permainan (misalnya STARTED, ENDED) dari daftar sesi di API.
    /// </summary>
    /// <param name="client">HTTP client untuk komunikasi ke API backend.</param>
    /// <param name="sessionId">Identifier unik sesi permainan.</param>
    /// <param name="ct">Token pembatalan untuk membatalkan permintaan.</param>
    /// <returns>String status sesi, atau null jika gagal ditemukan.</returns>
    private static async Task<string?> GetSessionStatusAsync(HttpClient client, Guid sessionId, CancellationToken ct)
    {
        var sessionResponse = await client.GetAsync("api/v1/sessions", ct);
        if (!sessionResponse.IsSuccessStatusCode)
        {
            return null;
        }

        var data = await sessionResponse.Content.TryReadFromJsonAsync<SessionListResponseDto>(cancellationToken: ct);
        return data?.Items.FirstOrDefault(x => x.SessionId == sessionId)?.Status;
    }

    /// <summary>
    /// Memuat daftar ruleset yang tersedia dari API untuk ditampilkan sebagai opsi pilihan.
    /// </summary>
    /// <param name="client">HTTP client untuk komunikasi ke API backend.</param>
    /// <param name="ct">Token pembatalan untuk membatalkan permintaan.</param>
    /// <returns>Tuple berisi daftar ruleset, hasil unauthorized jika ada, dan pesan kesalahan opsional.</returns>
    private async Task<(List<RulesetListItemDto> Rulesets, IActionResult? UnauthorizedResult, string? ErrorMessage)> LoadRulesetOptionsAsync(
        HttpClient client,
        CancellationToken ct)
    {
        var response = await client.GetAsync("api/v1/rulesets", ct);
        var unauthorized = this.HandleUnauthorizedApiResponse(response);
        if (unauthorized is not null)
        {
            return (new List<RulesetListItemDto>(), unauthorized, null);
        }

        if (!response.IsSuccessStatusCode)
        {
            return (
                new List<RulesetListItemDto>(),
                null,
                HttpContext.T("sessions.error.load_rulesets_failed")
                    .Replace("{status}", ((int)response.StatusCode).ToString()));
        }

        var data = await response.Content.TryReadFromJsonAsync<RulesetListResponseDto>(cancellationToken: ct);
        return (data?.Items ?? new List<RulesetListItemDto>(), null, null);
    }

    /// <summary>
    /// Memuat peta nama tampilan pemain (PlayerId ke DisplayName) dari API untuk resolusi identitas pada timeline.
    /// </summary>
    /// <param name="client">HTTP client untuk komunikasi ke API backend.</param>
    /// <param name="ct">Token pembatalan untuk membatalkan permintaan.</param>
    /// <returns>Dictionary yang memetakan PlayerId ke nama tampilan pemain.</returns>
    private static async Task<Dictionary<Guid, string>> LoadPlayerDisplayNameMapAsync(
        HttpClient client,
        CancellationToken ct)
    {
        var response = await client.GetAsync("api/v1/players", ct);
        if (!response.IsSuccessStatusCode)
        {
            return new Dictionary<Guid, string>();
        }

        var data = await response.Content.TryReadFromJsonAsync<PlayerListResponseDto>(cancellationToken: ct);
        return (data?.Items ?? new List<PlayerResponseDto>())
            .Where(item => !string.IsNullOrWhiteSpace(item.DisplayName))
            .GroupBy(item => item.PlayerId)
            .ToDictionary(group => group.Key, group => group.First().DisplayName);
    }

    /// <summary>
    /// Memuat dan memetakan event timeline sesi dari API menjadi daftar ViewModel yang siap ditampilkan.
    /// </summary>
    /// <param name="client">HTTP client untuk komunikasi ke API backend.</param>
    /// <param name="sessionId">Identifier unik sesi permainan.</param>
    /// <param name="language">Kode bahasa untuk lokalisasi label event.</param>
    /// <param name="ct">Token pembatalan untuk membatalkan permintaan.</param>
    /// <returns>Tuple berisi daftar event timeline dan pesan kesalahan opsional.</returns>
    private async Task<(List<SessionTimelineEventViewModel> Timeline, string? ErrorMessage)> LoadTimelineAsync(
        HttpClient client,
        Guid sessionId,
        string language,
        CancellationToken ct)
    {
        var response = await client.GetAsync($"api/v1/sessions/{sessionId}/events?fromSeq=0&limit=1000", ct);
        if (!response.IsSuccessStatusCode)
        {
            return (
                new List<SessionTimelineEventViewModel>(),
                HttpContext.T("sessions.error.load_timeline_failed")
                    .Replace("{status}", ((int)response.StatusCode).ToString()));
        }

        var data = await response.Content.TryReadFromJsonAsync<EventsBySessionResponseDto>(cancellationToken: ct);
        if (data?.Events is null || data.Events.Count == 0)
        {
            return (new List<SessionTimelineEventViewModel>(), null);
        }

        var timeline = SessionTimelineMapper.MapTimeline(data.Events, language);
        return (timeline, null);
    }

}

