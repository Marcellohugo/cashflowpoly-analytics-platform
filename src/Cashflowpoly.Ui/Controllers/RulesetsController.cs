using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using Cashflowpoly.Ui.Infrastructure;
using Cashflowpoly.Ui.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cashflowpoly.Ui.Controllers;

/// <summary>
/// Controller UI untuk manajemen ruleset.
/// </summary>
[Route("rulesets")]
public sealed class RulesetsController : Controller
{
    private readonly IHttpClientFactory _clientFactory;
    private const string RulesetErrorTempDataKey = "ruleset_error";

    public RulesetsController(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var client = _clientFactory.CreateClient("Api");
        var response = await client.GetAsync("api/v1/rulesets", ct);
        var unauthorized = this.HandleUnauthorizedApiResponse(response);
        if (unauthorized is not null)
        {
            return unauthorized;
        }

        if (!response.IsSuccessStatusCode)
        {
            return View(new RulesetListViewModel
            {
                ErrorMessage = $"Gagal mengambil ruleset. Status: {(int)response.StatusCode}"
            });
        }

        var data = await TryReadJsonAsync<RulesetListResponseDto>(response.Content, ct);
        if (data is null)
        {
            return View(new RulesetListViewModel
            {
                ErrorMessage = "Respon daftar ruleset tidak valid."
            });
        }

        return View(new RulesetListViewModel
        {
            Items = data?.Items ?? new List<RulesetListItemDto>()
        });
    }

    [HttpGet("create")]
    public IActionResult Create()
    {
        if (!HttpContext.Session.IsInstructor())
        {
            return RedirectToAction(nameof(Index));
        }

        return View(new CreateRulesetViewModel
        {
            IsEditMode = false,
            ConfigJson = """
            {
              "mode": "PEMULA",
              "actions_per_turn": 2,
              "starting_cash": 20,
              "weekday_rules": {
                "friday": { "feature": "DONATION", "enabled": true },
                "saturday": { "feature": "GOLD_TRADE", "enabled": true },
                "sunday": { "feature": "REST", "enabled": true }
              },
              "constraints": {
                "cash_min": 0,
                "max_ingredient_total": 6,
                "max_same_ingredient": 3,
                "primary_need_max_per_day": 1,
                "require_primary_before_others": true
              },
              "donation": { "min_amount": 1, "max_amount": 999999 },
              "gold_trade": { "allow_buy": true, "allow_sell": true },
              "advanced": {
                "loan": { "enabled": false },
                "insurance": { "enabled": false },
                "saving_goal": { "enabled": false }
              },
              "freelance": { "income": 1 },
              "scoring": {
                "donation_rank_points": [
                  { "rank": 1, "points": 7 },
                  { "rank": 2, "points": 5 },
                  { "rank": 3, "points": 2 }
                ],
                "gold_points_by_qty": [
                  { "qty": 1, "points": 3 },
                  { "qty": 2, "points": 5 },
                  { "qty": 3, "points": 8 },
                  { "qty": 4, "points": 12 }
                ],
                "pension_rank_points": [
                  { "rank": 1, "points": 5 },
                  { "rank": 2, "points": 3 },
                  { "rank": 3, "points": 1 }
                ]
              }
            }
            """
        });
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(CreateRulesetViewModel model, CancellationToken ct)
    {
        if (!HttpContext.Session.IsInstructor())
        {
            return RedirectToAction(nameof(Index));
        }

        if (string.IsNullOrWhiteSpace(model.Name))
        {
            model.IsEditMode = false;
            model.ErrorMessage = "Nama ruleset wajib diisi.";
            return View(model);
        }

        JsonNode? configNode;
        try
        {
            configNode = JsonNode.Parse(model.ConfigJson);
        }
        catch (JsonException)
        {
            model.IsEditMode = false;
            model.ErrorMessage = "Config JSON tidak valid.";
            return View(model);
        }

        var client = _clientFactory.CreateClient("Api");
        var payload = new
        {
            name = model.Name,
            description = model.Description,
            config = configNode
        };

        var response = await client.PostAsJsonAsync("api/v1/rulesets", payload, ct);
        var unauthorized = this.HandleUnauthorizedApiResponse(response);
        if (unauthorized is not null)
        {
            return unauthorized;
        }

        if (!response.IsSuccessStatusCode)
        {
            var error = await TryReadJsonAsync<ApiErrorResponseDto>(response.Content, ct);
            model.IsEditMode = false;
            model.ErrorMessage = error?.Message ?? $"Gagal membuat ruleset. Status: {(int)response.StatusCode}";
            return View(model);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("{rulesetId:guid}/edit")]
    public async Task<IActionResult> Edit(Guid rulesetId, CancellationToken ct)
    {
        if (!HttpContext.Session.IsInstructor())
        {
            return RedirectToAction(nameof(Index));
        }

        var client = _clientFactory.CreateClient("Api");
        var response = await client.GetAsync($"api/v1/rulesets/{rulesetId}", ct);
        var unauthorized = this.HandleUnauthorizedApiResponse(response);
        if (unauthorized is not null)
        {
            return unauthorized;
        }

        if (!response.IsSuccessStatusCode)
        {
            var error = await TryReadJsonAsync<ApiErrorResponseDto>(response.Content, ct);
            return View("Create", new CreateRulesetViewModel
            {
                RulesetId = rulesetId,
                IsEditMode = true,
                ErrorMessage = error?.Message ?? $"Gagal memuat ruleset untuk diedit. Status: {(int)response.StatusCode}"
            });
        }

        var data = await TryReadJsonAsync<RulesetDetailResponseDto>(response.Content, ct);
        if (data is null)
        {
            return View("Create", new CreateRulesetViewModel
            {
                RulesetId = rulesetId,
                IsEditMode = true,
                ErrorMessage = "Respon detail ruleset tidak valid."
            });
        }

        return View("Create", new CreateRulesetViewModel
        {
            RulesetId = rulesetId,
            IsEditMode = true,
            Name = data.Name,
            Description = data.Description,
            ConfigJson = SerializeIndentedJson(data.ConfigJson)
        });
    }

    [HttpPost("{rulesetId:guid}/edit")]
    public async Task<IActionResult> Edit(Guid rulesetId, CreateRulesetViewModel model, CancellationToken ct)
    {
        if (!HttpContext.Session.IsInstructor())
        {
            return RedirectToAction(nameof(Index));
        }

        model.RulesetId = rulesetId;
        model.IsEditMode = true;

        if (string.IsNullOrWhiteSpace(model.Name))
        {
            model.ErrorMessage = "Nama ruleset wajib diisi.";
            return View("Create", model);
        }

        JsonNode? configNode;
        try
        {
            configNode = JsonNode.Parse(model.ConfigJson);
        }
        catch (JsonException)
        {
            model.ErrorMessage = "Config JSON tidak valid.";
            return View("Create", model);
        }

        var client = _clientFactory.CreateClient("Api");
        var payload = new
        {
            name = model.Name,
            description = model.Description,
            config = configNode
        };

        var response = await client.PutAsJsonAsync($"api/v1/rulesets/{rulesetId}", payload, ct);
        var unauthorized = this.HandleUnauthorizedApiResponse(response);
        if (unauthorized is not null)
        {
            return unauthorized;
        }

        if (!response.IsSuccessStatusCode)
        {
            var error = await TryReadJsonAsync<ApiErrorResponseDto>(response.Content, ct);
            model.ErrorMessage = error?.Message ?? $"Gagal mengubah ruleset. Status: {(int)response.StatusCode}";
            return View("Create", model);
        }

        return RedirectToAction(nameof(Details), new { rulesetId });
    }

    [HttpGet("{rulesetId:guid}")]
    public async Task<IActionResult> Details(Guid rulesetId, CancellationToken ct)
    {
        var client = _clientFactory.CreateClient("Api");
        var response = await client.GetAsync($"api/v1/rulesets/{rulesetId}", ct);
        var unauthorized = this.HandleUnauthorizedApiResponse(response);
        if (unauthorized is not null)
        {
            return unauthorized;
        }

        if (!response.IsSuccessStatusCode)
        {
            var error = await TryReadJsonAsync<ApiErrorResponseDto>(response.Content, ct);
            return View(new RulesetDetailViewModel
            {
                ErrorMessage = error?.Message ?? $"Gagal memuat ruleset. Status: {(int)response.StatusCode}"
            });
        }

        var data = await TryReadJsonAsync<RulesetDetailResponseDto>(response.Content, ct);
        if (data is null)
        {
            return View(new RulesetDetailViewModel
            {
                ErrorMessage = "Respon detail ruleset tidak valid."
            });
        }

        return View(new RulesetDetailViewModel
        {
            Ruleset = data,
            ErrorMessage = TempData[RulesetErrorTempDataKey] as string
        });
    }

    [HttpPost("{rulesetId:guid}/versions/{version:int}/activate")]
    public async Task<IActionResult> ActivateVersion(Guid rulesetId, int version, CancellationToken ct)
    {
        if (!HttpContext.Session.IsInstructor())
        {
            return RedirectToAction(nameof(Details), new { rulesetId });
        }

        var client = _clientFactory.CreateClient("Api");
        var response = await client.PostAsync($"api/v1/rulesets/{rulesetId}/versions/{version}/activate", null, ct);
        var unauthorized = this.HandleUnauthorizedApiResponse(response);
        if (unauthorized is not null)
        {
            return unauthorized;
        }

        if (!response.IsSuccessStatusCode)
        {
            TempData[RulesetErrorTempDataKey] = await BuildRulesetApiErrorMessage(
                response,
                "Gagal mengaktifkan versi ruleset",
                ct);
        }

        return RedirectToAction(nameof(Details), new { rulesetId });
    }

    [HttpPost("{rulesetId:guid}/delete")]
    public async Task<IActionResult> Delete(Guid rulesetId, CancellationToken ct)
    {
        if (!HttpContext.Session.IsInstructor())
        {
            return RedirectToAction(nameof(Details), new { rulesetId });
        }

        var client = _clientFactory.CreateClient("Api");
        var response = await client.DeleteAsync($"api/v1/rulesets/{rulesetId}", ct);
        var unauthorized = this.HandleUnauthorizedApiResponse(response);
        if (unauthorized is not null)
        {
            return unauthorized;
        }

        if (!response.IsSuccessStatusCode && response.StatusCode != System.Net.HttpStatusCode.NoContent)
        {
            TempData[RulesetErrorTempDataKey] = await BuildRulesetApiErrorMessage(
                response,
                "Gagal hapus ruleset",
                ct);
            return RedirectToAction(nameof(Details), new { rulesetId });
        }

        return RedirectToAction(nameof(Index));
    }

    private static async Task<string> BuildRulesetApiErrorMessage(HttpResponseMessage response, string prefix, CancellationToken ct)
    {
        var error = await TryReadJsonAsync<ApiErrorResponseDto>(response.Content, ct);

        return error?.Message ?? $"{prefix}. Status: {(int)response.StatusCode}";
    }

    private static string SerializeIndentedJson(JsonElement? configJson)
    {
        if (!configJson.HasValue)
        {
            return "{}";
        }

        try
        {
            return JsonSerializer.Serialize(configJson.Value, new JsonSerializerOptions
            {
                WriteIndented = true
            });
        }
        catch (JsonException)
        {
            return "{}";
        }
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

