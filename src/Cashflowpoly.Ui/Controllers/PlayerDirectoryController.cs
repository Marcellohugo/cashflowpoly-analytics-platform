using System.Net.Http.Json;
using Cashflowpoly.Ui.Infrastructure;
using Cashflowpoly.Ui.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cashflowpoly.Ui.Controllers;

/// <summary>
/// Controller UI untuk daftar pemain.
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
        if (!HttpContext.Session.IsInstructor())
        {
            return RedirectToAction("Index", "Analytics");
        }

        var client = _clientFactory.CreateClient("Api");
        var response = await client.GetAsync("api/v1/players", ct);
        var unauthorized = this.HandleUnauthorizedApiResponse(response);
        if (unauthorized is not null)
        {
            return unauthorized;
        }

        if (!response.IsSuccessStatusCode)
        {
            return View("~/Views/Players/Index.cshtml", new PlayerDirectoryViewModel
            {
                ErrorMessage = $"Gagal memuat daftar pemain. Status: {(int)response.StatusCode}"
            });
        }

        var data = await response.Content.ReadFromJsonAsync<PlayerListResponseDto>(cancellationToken: ct);
        return View("~/Views/Players/Index.cshtml", new PlayerDirectoryViewModel
        {
            Players = data?.Items ?? new List<PlayerResponseDto>()
        });
    }
}

