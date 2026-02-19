// Fungsi file: Mengelola alur halaman UI untuk domain AnalyticsController termasuk komunikasi ke API backend.
using System.Net.Http.Json;
using System.Text.Json;
using Cashflowpoly.Ui.Infrastructure;
using Cashflowpoly.Ui.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cashflowpoly.Ui.Controllers;

/// <summary>
/// Menyatakan peran utama tipe AnalyticsController pada modul ini.
/// </summary>
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
    /// <summary>
    /// Menjalankan fungsi Index sebagai bagian dari alur file ini.
    /// </summary>
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
    /// <summary>
    /// Menjalankan fungsi Index sebagai bagian dari alur file ini.
    /// </summary>
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
            viewModel.ErrorMessage = HttpContext.T("analytics.error.invalid_session_id");
            return View(viewModel);
        }

        if (!viewModel.IsInstructor &&
            viewModel.Sessions.Count > 0 &&
            viewModel.Sessions.All(item => item.SessionId != sessionId))
        {
            viewModel.ErrorMessage = HttpContext.T("analytics.error.session_not_available_for_player");
            return View(viewModel);
        }

        var loadResult = await PopulateAnalyticsResultAsync(viewModel, sessionId, ct);
        if (loadResult is not null)
        {
            return loadResult;
        }

        return View(viewModel);
    }

    /// <summary>
    /// Menjalankan fungsi BuildInitialModelAsync sebagai bagian dari alur file ini.
    /// </summary>
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
            model.SessionLookupErrorMessage = HttpContext
                .T("analytics.error.load_sessions_failed")
                .Replace("{status}", ((int)sessionResponse.StatusCode).ToString());
        }
        else
        {
            var sessions = await TryReadJsonAsync<SessionListResponseDto>(sessionResponse.Content, ct);
            if (sessions is null)
            {
                model.SessionLookupErrorMessage = HttpContext.T("analytics.error.invalid_sessions_response");
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

    /// <summary>
    /// Menjalankan fungsi PopulateAnalyticsResultAsync sebagai bagian dari alur file ini.
    /// </summary>
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
            model.ErrorMessage = error?.Message ?? HttpContext
                .T("analytics.error.load_session_analytics_failed")
                .Replace("{status}", ((int)response.StatusCode).ToString());
            model.Result = null;
            return null;
        }

        var result = await TryReadJsonAsync<AnalyticsSessionResponseDto>(response.Content, ct);
        if (result is null)
        {
            model.ErrorMessage = HttpContext.T("analytics.error.invalid_session_analytics_response");
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

    /// <summary>
    /// Menjalankan fungsi PopulateRulesetSummaryAsync sebagai bagian dari alur file ini.
    /// </summary>
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
            model.RulesetErrorMessage = error?.Message ?? HttpContext
                .T("analytics.error.load_ruleset_analytics_failed")
                .Replace("{status}", ((int)response.StatusCode).ToString());
            model.RulesetResult = null;
            return null;
        }

        var result = await TryReadJsonAsync<RulesetAnalyticsSummaryResponseDto>(response.Content, ct);
        if (result is null)
        {
            model.RulesetErrorMessage = HttpContext.T("analytics.error.invalid_ruleset_analytics_response");
            model.RulesetResult = null;
            return null;
        }

        model.RulesetResult = result;
        return null;
    }

    /// <summary>
    /// Membaca JSON dari respons HTTP dengan fallback aman saat format tidak valid.
    /// </summary>
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

