using System.Net.Http.Json;
using Cashflowpoly.Ui.Infrastructure;
using Cashflowpoly.Ui.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cashflowpoly.Ui.Controllers;

/// <summary>
/// Controller UI untuk halaman sesi.
/// </summary>
[Route("sessions")]
public sealed class SessionsController : Controller
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly IWebHostEnvironment _environment;

    public SessionsController(IHttpClientFactory clientFactory, IWebHostEnvironment environment)
    {
        _clientFactory = clientFactory;
        _environment = environment;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        if (!HttpContext.Session.IsInstructor())
        {
            return RedirectToAction("Index", "Analytics");
        }

        var client = _clientFactory.CreateClient("Api");
        var response = await client.GetAsync("api/v1/sessions", ct);
        var unauthorized = this.HandleUnauthorizedApiResponse(response);
        if (unauthorized is not null)
        {
            return unauthorized;
        }

        if (!response.IsSuccessStatusCode)
        {
            return View(new SessionListViewModel
            {
                ErrorMessage = $"Gagal mengambil daftar sesi. Status: {(int)response.StatusCode}"
            });
        }

        var data = await response.Content.ReadFromJsonAsync<SessionListResponseDto>(cancellationToken: ct);
        return View(new SessionListViewModel
        {
            Items = data?.Items ?? new List<SessionListItemDto>()
        });
    }

    [HttpGet("{sessionId:guid}")]
    public async Task<IActionResult> Details(Guid sessionId, CancellationToken ct)
    {
        var detail = await BuildSessionDetailViewModel(sessionId, ct);
        return detail.Result ?? View(detail.Model);
    }

    [HttpPost("{sessionId:guid}/end")]
    public async Task<IActionResult> End(Guid sessionId, CancellationToken ct)
    {
        if (!_environment.IsDevelopment() || !HttpContext.Session.IsInstructor())
        {
            return RedirectToAction(nameof(Details), new { sessionId });
        }

        var client = _clientFactory.CreateClient("Api");
        var response = await client.PostAsync($"api/v1/sessions/{sessionId}/end", null, ct);
        var unauthorized = this.HandleUnauthorizedApiResponse(response);
        if (unauthorized is not null)
        {
            return unauthorized;
        }

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction(nameof(Details), new { sessionId });
        }

        var apiError = await response.Content.ReadFromJsonAsync<ApiErrorResponseDto>(cancellationToken: ct);
        var detail = await BuildSessionDetailViewModel(
            sessionId,
            ct,
            apiError?.Message ?? $"Gagal menyelesaikan sesi. Status: {(int)response.StatusCode}");
        return detail.Result ?? View("Details", detail.Model);
    }

    [HttpGet("{sessionId:guid}/ruleset")]
    public async Task<IActionResult> Ruleset(Guid sessionId, CancellationToken ct)
    {
        if (!HttpContext.Session.IsInstructor())
        {
            return RedirectToAction(nameof(Details), new { sessionId });
        }

        var client = _clientFactory.CreateClient("Api");
        var response = await client.GetAsync("api/v1/rulesets", ct);
        var unauthorized = this.HandleUnauthorizedApiResponse(response);
        if (unauthorized is not null)
        {
            return unauthorized;
        }

        if (!response.IsSuccessStatusCode)
        {
            return View(new SessionRulesetViewModel
            {
                SessionId = sessionId,
                ErrorMessage = $"Gagal memuat ruleset. Status: {(int)response.StatusCode}"
            });
        }

        var data = await response.Content.ReadFromJsonAsync<RulesetListResponseDto>(cancellationToken: ct);
        return View(new SessionRulesetViewModel
        {
            SessionId = sessionId,
            Rulesets = data?.Items ?? new List<RulesetListItemDto>()
        });
    }

    [HttpPost("{sessionId:guid}/ruleset")]
    public async Task<IActionResult> Ruleset(Guid sessionId, SessionRulesetViewModel model, CancellationToken ct)
    {
        if (!HttpContext.Session.IsInstructor())
        {
            return RedirectToAction(nameof(Details), new { sessionId });
        }

        if (model.SelectedRulesetId is null || model.SelectedVersion is null)
        {
            model.ErrorMessage = "Ruleset dan versi wajib dipilih.";
            return View(model);
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
            var error = await response.Content.ReadFromJsonAsync<ApiErrorResponseDto>(cancellationToken: ct);
            model.ErrorMessage = error?.Message ?? $"Gagal aktivasi ruleset. Status: {(int)response.StatusCode}";
            return View(model);
        }

        return RedirectToAction(nameof(Details), new { sessionId });
    }

    private async Task<(SessionDetailViewModel Model, IActionResult? Result)> BuildSessionDetailViewModel(
        Guid sessionId,
        CancellationToken ct,
        string? overrideErrorMessage = null)
    {
        var client = _clientFactory.CreateClient("Api");
        var response = await client.GetAsync($"api/v1/analytics/sessions/{sessionId}", ct);
        var unauthorized = this.HandleUnauthorizedApiResponse(response);
        if (unauthorized is not null)
        {
            return (new SessionDetailViewModel
            {
                SessionId = sessionId,
                IsDevelopment = _environment.IsDevelopment()
            }, unauthorized);
        }

        var sessionStatus = await GetSessionStatusAsync(client, sessionId, ct);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadFromJsonAsync<ApiErrorResponseDto>(cancellationToken: ct);
            return (new SessionDetailViewModel
            {
                SessionId = sessionId,
                SessionStatus = sessionStatus,
                IsDevelopment = _environment.IsDevelopment(),
                ErrorMessage = overrideErrorMessage ?? error?.Message ?? $"Gagal memuat detail sesi. Status: {(int)response.StatusCode}"
            }, null);
        }

        var analytics = await response.Content.ReadFromJsonAsync<AnalyticsSessionResponseDto>(cancellationToken: ct);
        return (new SessionDetailViewModel
        {
            SessionId = sessionId,
            SessionStatus = sessionStatus,
            IsDevelopment = _environment.IsDevelopment(),
            Analytics = analytics,
            ErrorMessage = overrideErrorMessage
        }, null);
    }

    private static async Task<string?> GetSessionStatusAsync(HttpClient client, Guid sessionId, CancellationToken ct)
    {
        var sessionResponse = await client.GetAsync("api/v1/sessions", ct);
        if (!sessionResponse.IsSuccessStatusCode)
        {
            return null;
        }

        var data = await sessionResponse.Content.ReadFromJsonAsync<SessionListResponseDto>(cancellationToken: ct);
        return data?.Items.FirstOrDefault(x => x.SessionId == sessionId)?.Status;
    }
}

