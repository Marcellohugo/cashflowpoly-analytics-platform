using System.Net.Http.Json;
using Cashflowpoly.Ui.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cashflowpoly.Ui.Controllers;

/// <summary>
/// Controller UI untuk daftar dan pembuatan pemain.
/// </summary>
[Route("players")]
public sealed class PlayerDirectoryController : Controller
{
    private readonly IHttpClientFactory _clientFactory;

    public PlayerDirectoryController(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var client = _clientFactory.CreateClient("Api");
        var response = await client.GetAsync("api/players", ct);
        if (!response.IsSuccessStatusCode)
        {
            return View(new PlayerDirectoryViewModel
            {
                ErrorMessage = $"Gagal memuat daftar pemain. Status: {(int)response.StatusCode}"
            });
        }

        var data = await response.Content.ReadFromJsonAsync<PlayerListResponseDto>(cancellationToken: ct);
        return View(new PlayerDirectoryViewModel
        {
            Players = data?.Items ?? new List<PlayerResponseDto>()
        });
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(PlayerDirectoryViewModel model, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(model.DisplayName))
        {
            model.ErrorMessage = "Nama pemain wajib diisi.";
            return View("Index", model);
        }

        var client = _clientFactory.CreateClient("Api");
        var payload = new { display_name = model.DisplayName };
        var response = await client.PostAsJsonAsync("api/players", payload, ct);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadFromJsonAsync<ApiErrorResponseDto>(cancellationToken: ct);
            model.ErrorMessage = error?.Message ?? $"Gagal membuat pemain. Status: {(int)response.StatusCode}";
            return View("Index", model);
        }

        return RedirectToAction(nameof(Index));
    }
}
