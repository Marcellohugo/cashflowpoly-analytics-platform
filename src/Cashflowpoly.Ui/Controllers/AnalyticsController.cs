using System.Net.Http.Json;
using System.Text.Json;
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
            var sessions = await TryReadJsonAsync<SessionListResponseDto>(sessionResponse.Content, ct);
            if (sessions is null)
            {
                model.SessionLookupErrorMessage = "Respon daftar sesi tidak valid.";
                return (model, null);
            }

            model.Sessions = (sessions?.Items ?? new List<SessionListItemDto>())
                .OrderByDescending(item => string.Equals(item.Status, "STARTED", StringComparison.OrdinalIgnoreCase))
                .ThenByDescending(item => item.StartedAt ?? item.CreatedAt)
                .ToList();
        }

        var playersResponse = await client.GetAsync("api/v1/players", ct);
        unauthorized = this.HandleUnauthorizedApiResponse(playersResponse);
        if (unauthorized is not null)
        {
            return (model, unauthorized);
        }

        if (playersResponse.IsSuccessStatusCode)
        {
            var players = await TryReadJsonAsync<PlayerListResponseDto>(playersResponse.Content, ct);
            model.PlayerDisplayNames = (players?.Items ?? new List<PlayerResponseDto>())
                .Where(item => !string.IsNullOrWhiteSpace(item.DisplayName))
                .GroupBy(item => item.PlayerId)
                .ToDictionary(group => group.Key, group => group.First().DisplayName);
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
            var error = await TryReadJsonAsync<ApiErrorResponseDto>(response.Content, ct);
            model.ErrorMessage = error?.Message ?? $"Gagal memuat analitika. Status: {(int)response.StatusCode}";
            model.Result = null;
            return null;
        }

        var result = await TryReadJsonAsync<AnalyticsSessionResponseDto>(response.Content, ct);
        if (result is null)
        {
            model.ErrorMessage = "Respon analitika sesi tidak valid.";
            model.Result = null;
            return null;
        }

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
            var error = await TryReadJsonAsync<ApiErrorResponseDto>(response.Content, ct);
            model.RulesetErrorMessage = error?.Message ?? $"Gagal memuat analitika ruleset. Status: {(int)response.StatusCode}";
            model.RulesetResult = null;
            return null;
        }

        var result = await TryReadJsonAsync<RulesetAnalyticsSummaryResponseDto>(response.Content, ct);
        if (result is null)
        {
            model.RulesetErrorMessage = "Respon analitika ruleset tidak valid.";
            model.RulesetResult = null;
            return null;
        }

        model.RulesetResult = result;
        return null;
    }

    private static async Task<T?> TryReadJsonAsync<T>(HttpContent content, CancellationToken ct)
    {
        try
        {
            return await content.ReadFromJsonAsync<T>(cancellationToken: ct);
        }
        catch (JsonException)
        {
            return default;
        }
        catch (NotSupportedException)
        {
            return default;
        }
    }
}

