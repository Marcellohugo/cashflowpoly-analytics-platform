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
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var setup = await BuildInitialModelAsync(ct);
        if (setup.Result is not null)
        {
            return setup.Result;
        }

        var model = setup.Model;
        if (!model.IsInstructor && model.Sessions.Count > 0)
        {
            var preferredSession = model.Sessions
                .OrderByDescending(item => string.Equals(item.Status, "STARTED", StringComparison.OrdinalIgnoreCase))
                .ThenByDescending(item => item.StartedAt ?? item.CreatedAt)
                .First();

            model.SessionId = preferredSession.SessionId.ToString();
            var loadResult = await PopulateAnalyticsResultAsync(model, preferredSession.SessionId, ct);
            if (loadResult is not null)
            {
                return loadResult;
            }
        }

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Index(AnalyticsSearchViewModel model, CancellationToken ct)
    {
        var setup = await BuildInitialModelAsync(ct);
        if (setup.Result is not null)
        {
            return setup.Result;
        }

        var viewModel = setup.Model;
        viewModel.SessionId = (model.SessionId ?? string.Empty).Trim();

        if (!Guid.TryParse(viewModel.SessionId, out var sessionId))
        {
            viewModel.ErrorMessage = "Session ID tidak valid.";
            return View(viewModel);
        }

        if (!viewModel.IsInstructor &&
            viewModel.Sessions.Count > 0 &&
            viewModel.Sessions.All(item => item.SessionId != sessionId))
        {
            viewModel.ErrorMessage = "Session tidak tersedia untuk akun player ini.";
            return View(viewModel);
        }

        var loadResult = await PopulateAnalyticsResultAsync(viewModel, sessionId, ct);
        if (loadResult is not null)
        {
            return loadResult;
        }

        return View(viewModel);
    }

    private async Task<(AnalyticsSearchViewModel Model, IActionResult? Result)> BuildInitialModelAsync(CancellationToken ct)
    {
        var model = new AnalyticsSearchViewModel
        {
            IsInstructor = HttpContext.Session.IsInstructor()
        };

        var client = _clientFactory.CreateClient("Api");
        var sessionResponse = await client.GetAsync("api/v1/sessions", ct);
        var unauthorized = this.HandleUnauthorizedApiResponse(sessionResponse);
        if (unauthorized is not null)
        {
            return (model, unauthorized);
        }

        if (!sessionResponse.IsSuccessStatusCode)
        {
            model.SessionLookupErrorMessage = $"Gagal memuat daftar sesi. Status: {(int)sessionResponse.StatusCode}";
        }
        else
        {
            var sessions = await sessionResponse.Content.ReadFromJsonAsync<SessionListResponseDto>(cancellationToken: ct);
            model.Sessions = (sessions?.Items ?? new List<SessionListItemDto>())
                .OrderByDescending(item => string.Equals(item.Status, "STARTED", StringComparison.OrdinalIgnoreCase))
                .ThenByDescending(item => item.StartedAt ?? item.CreatedAt)
                .ToList();
        }

        return (model, null);
    }

    private async Task<IActionResult?> PopulateAnalyticsResultAsync(
        AnalyticsSearchViewModel model,
        Guid sessionId,
        CancellationToken ct)
    {
        model.SessionId = sessionId.ToString();
        model.RulesetResult = null;
        model.RulesetErrorMessage = null;

        var client = _clientFactory.CreateClient("Api");
        var response = await client.GetAsync($"api/v1/analytics/sessions/{sessionId}", ct);
        var unauthorized = this.HandleUnauthorizedApiResponse(response);
        if (unauthorized is not null)
        {
            return unauthorized;
        }

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadFromJsonAsync<ApiErrorResponseDto>(cancellationToken: ct);
            model.ErrorMessage = error?.Message ?? $"Gagal memuat analitika. Status: {(int)response.StatusCode}";
            model.Result = null;
            return null;
        }

        var result = await response.Content.ReadFromJsonAsync<AnalyticsSessionResponseDto>(cancellationToken: ct);
        model.Result = result;

        if (result?.RulesetId is Guid rulesetId)
        {
            var loadRulesetResult = await PopulateRulesetSummaryAsync(model, rulesetId, ct);
            if (loadRulesetResult is not null)
            {
                return loadRulesetResult;
            }
        }

        return null;
    }

    private async Task<IActionResult?> PopulateRulesetSummaryAsync(
        AnalyticsSearchViewModel model,
        Guid rulesetId,
        CancellationToken ct)
    {
        var client = _clientFactory.CreateClient("Api");
        var response = await client.GetAsync($"api/v1/analytics/rulesets/{rulesetId}/summary", ct);
        var unauthorized = this.HandleUnauthorizedApiResponse(response);
        if (unauthorized is not null)
        {
            return unauthorized;
        }

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadFromJsonAsync<ApiErrorResponseDto>(cancellationToken: ct);
            model.RulesetErrorMessage = error?.Message ?? $"Gagal memuat analitika ruleset. Status: {(int)response.StatusCode}";
            model.RulesetResult = null;
            return null;
        }

        var result = await response.Content.ReadFromJsonAsync<RulesetAnalyticsSummaryResponseDto>(cancellationToken: ct);
        model.RulesetResult = result;
        return null;
    }
}

