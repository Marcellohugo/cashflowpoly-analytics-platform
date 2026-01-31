using System.Net.Http.Json;
using Cashflowpoly.Ui.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cashflowpoly.Ui.Controllers;

public sealed class AnalyticsController : Controller
{
    private readonly IHttpClientFactory _clientFactory;

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
}
