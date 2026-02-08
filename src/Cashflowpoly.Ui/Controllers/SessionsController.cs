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

    public SessionsController(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        if (!HttpContext.Session.IsInstructor())
        {
            return RedirectToAction("Index", "Analytics");
        }

        var client = _clientFactory.CreateClient("Api");
        var response = await client.GetAsync("api/sessions", ct);
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
        var client = _clientFactory.CreateClient("Api");
        var response = await client.GetAsync($"api/analytics/sessions/{sessionId}", ct);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadFromJsonAsync<ApiErrorResponseDto>(cancellationToken: ct);
            return View(new SessionDetailViewModel
            {
                SessionId = sessionId,
                ErrorMessage = error?.Message ?? $"Gagal memuat detail sesi. Status: {(int)response.StatusCode}"
            });
        }

        var analytics = await response.Content.ReadFromJsonAsync<AnalyticsSessionResponseDto>(cancellationToken: ct);
        return View(new SessionDetailViewModel
        {
            SessionId = sessionId,
            Analytics = analytics
        });
    }

    [HttpGet("{sessionId:guid}/ruleset")]
    public async Task<IActionResult> Ruleset(Guid sessionId, CancellationToken ct)
    {
        if (!HttpContext.Session.IsInstructor())
        {
            return RedirectToAction(nameof(Details), new { sessionId });
        }

        var client = _clientFactory.CreateClient("Api");
        var response = await client.GetAsync("api/rulesets", ct);
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

        var response = await client.PostAsJsonAsync($"api/sessions/{sessionId}/ruleset/activate", payload, ct);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadFromJsonAsync<ApiErrorResponseDto>(cancellationToken: ct);
            model.ErrorMessage = error?.Message ?? $"Gagal aktivasi ruleset. Status: {(int)response.StatusCode}";
            return View(model);
        }

        return RedirectToAction(nameof(Details), new { sessionId });
    }
}
