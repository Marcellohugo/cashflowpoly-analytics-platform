using System.Net.Http.Json;
using Cashflowpoly.Ui.Infrastructure;
using Cashflowpoly.Ui.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cashflowpoly.Ui.Controllers;

public sealed class AnalyticsController : Controller
{
    private readonly IHttpClientFactory _clientFactory;

    /// <summary>
    /// Controller UI untuk pencarian analitika sesi.
    /// </summary>
    public AnalyticsController(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(new AnalyticsSearchViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Index(AnalyticsSearchViewModel model, CancellationToken ct)
    {
        if (!Guid.TryParse(model.SessionId, out var sessionId))
        {
            model.ErrorMessage = "Session ID tidak valid.";
            return View(model);
        }

        var client = _clientFactory.CreateClient("Api");
        var response = await client.GetAsync($"api/analytics/sessions/{sessionId}", ct);
        var unauthorized = this.HandleUnauthorizedApiResponse(response);
        if (unauthorized is not null)
        {
            return unauthorized;
        }

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadFromJsonAsync<ApiErrorResponseDto>(cancellationToken: ct);
            model.ErrorMessage = error?.Message ?? $"Gagal memuat analitika. Status: {(int)response.StatusCode}";
            return View(model);
        }

        var result = await response.Content.ReadFromJsonAsync<AnalyticsSessionResponseDto>(cancellationToken: ct);
        model.Result = result;
        return View(model);
    }

    [HttpGet("/analytics/rulesets/{rulesetId:guid}")]
    public async Task<IActionResult> Ruleset(Guid rulesetId, CancellationToken ct)
    {
        var client = _clientFactory.CreateClient("Api");
        var response = await client.GetAsync($"api/analytics/rulesets/{rulesetId}/summary", ct);
        var unauthorized = this.HandleUnauthorizedApiResponse(response);
        if (unauthorized is not null)
        {
            return unauthorized;
        }

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadFromJsonAsync<ApiErrorResponseDto>(cancellationToken: ct);
            return View(new RulesetAnalyticsViewModel
            {
                RulesetId = rulesetId.ToString(),
                ErrorMessage = error?.Message ?? $"Gagal memuat analitika ruleset. Status: {(int)response.StatusCode}"
            });
        }

        var result = await response.Content.ReadFromJsonAsync<RulesetAnalyticsSummaryResponseDto>(cancellationToken: ct);
        return View(new RulesetAnalyticsViewModel
        {
            RulesetId = rulesetId.ToString(),
            Result = result
        });
    }
}
