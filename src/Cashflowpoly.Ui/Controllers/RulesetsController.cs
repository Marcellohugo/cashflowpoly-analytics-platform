// Fungsi file: Mengelola alur halaman UI untuk domain RulesetsController termasuk komunikasi ke API backend.
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
    private const string RulesetInfoTempDataKey = "ruleset_info";
    private const string DefaultCatalogSource = "default-catalog";

    /// <summary>
    /// Menjalankan fungsi RulesetsController sebagai bagian dari alur file ini.
    /// </summary>
    public RulesetsController(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    [HttpGet("")]
    /// <summary>
    /// Menjalankan fungsi Index sebagai bagian dari alur file ini.
    /// </summary>
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        ViewData[RulesetErrorTempDataKey] = TempData[RulesetErrorTempDataKey] as string;
        ViewData[RulesetInfoTempDataKey] = TempData[RulesetInfoTempDataKey] as string;

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
                ErrorMessage = HttpContext
                    .T("rulesets.error.load_list_failed")
                    .Replace("{status}", ((int)response.StatusCode).ToString())
            });
        }

        var data = await TryReadJsonAsync<RulesetListResponseDto>(response.Content, ct);
        if (data is null)
        {
            return View(new RulesetListViewModel
            {
                ErrorMessage = HttpContext.T("rulesets.error.invalid_list_response")
            });
        }

        var defaultComponentItems = new List<DefaultRulesetComponentItemDto>();
        string? defaultComponentsErrorMessage = null;
        var defaultsResponse = await client.GetAsync("api/v1/rulesets/components/defaults", ct);
        unauthorized = this.HandleUnauthorizedApiResponse(defaultsResponse);
        if (unauthorized is not null)
        {
            return unauthorized;
        }

        if (!defaultsResponse.IsSuccessStatusCode)
        {
            defaultComponentsErrorMessage = HttpContext
                .T("rulesets.error.load_default_components_failed")
                .Replace("{status}", ((int)defaultsResponse.StatusCode).ToString());
        }
        else
        {
            var defaultsData = await TryReadJsonAsync<DefaultRulesetComponentsResponseDto>(defaultsResponse.Content, ct);
            if (defaultsData is null)
            {
                defaultComponentsErrorMessage = HttpContext.T("rulesets.error.invalid_default_components_response");
            }
            else
            {
                defaultComponentItems = defaultsData.Items ?? new List<DefaultRulesetComponentItemDto>();
            }
        }

        return View(new RulesetListViewModel
        {
            Items = data.Items ?? new List<RulesetListItemDto>(),
            DefaultComponentItems = defaultComponentItems,
            DefaultComponentsErrorMessage = defaultComponentsErrorMessage
        });
    }

    [HttpGet("create")]
    /// <summary>
    /// Menjalankan fungsi Create sebagai bagian dari alur file ini.
    /// </summary>
    public IActionResult Create()
    {
        if (!HttpContext.Session.IsInstructor())
        {
            return RedirectToAction(nameof(Index));
        }

        return View(BuildDefaultCreateViewModel());
    }

    [HttpPost("create")]
    /// <summary>
    /// Menjalankan fungsi Create sebagai bagian dari alur file ini.
    /// </summary>
    public async Task<IActionResult> Create(CreateRulesetViewModel model, CancellationToken ct)
    {
        if (!HttpContext.Session.IsInstructor())
        {
            return RedirectToAction(nameof(Index));
        }

        if (string.IsNullOrWhiteSpace(model.Name))
        {
            model.IsEditMode = false;
            model.ErrorMessage = HttpContext.T("rulesets.error.name_required");
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
            model.ErrorMessage = HttpContext.T("rulesets.error.invalid_config_json");
            return View(model);
        }

        var client = _clientFactory.CreateClient("Api");
        configNode = await EnsureComponentCatalogAsync(configNode, client, ct);
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
            model.ErrorMessage = error?.Message ?? HttpContext
                .T("rulesets.error.create_failed")
                .Replace("{status}", ((int)response.StatusCode).ToString());
            return View(model);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("{rulesetId:guid}/edit")]
    /// <summary>
    /// Menjalankan fungsi Edit sebagai bagian dari alur file ini.
    /// </summary>
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
                ErrorMessage = error?.Message ?? HttpContext
                    .T("rulesets.error.load_for_edit_failed")
                    .Replace("{status}", ((int)response.StatusCode).ToString())
            });
        }

        var data = await TryReadJsonAsync<RulesetDetailResponseDto>(response.Content, ct);
        if (data is null)
        {
            return View("Create", new CreateRulesetViewModel
            {
                RulesetId = rulesetId,
                IsEditMode = true,
                ErrorMessage = HttpContext.T("rulesets.error.invalid_detail_response")
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
    /// <summary>
    /// Menjalankan fungsi Edit sebagai bagian dari alur file ini.
    /// </summary>
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
            model.ErrorMessage = HttpContext.T("rulesets.error.name_required");
            return View("Create", model);
        }

        JsonNode? configNode;
        try
        {
            configNode = JsonNode.Parse(model.ConfigJson);
        }
        catch (JsonException)
        {
            model.ErrorMessage = HttpContext.T("rulesets.error.invalid_config_json");
            return View("Create", model);
        }

        var client = _clientFactory.CreateClient("Api");
        configNode = await EnsureComponentCatalogAsync(configNode, client, ct);
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
            model.ErrorMessage = error?.Message ?? HttpContext
                .T("rulesets.error.update_failed")
                .Replace("{status}", ((int)response.StatusCode).ToString());
            return View("Create", model);
        }

        return RedirectToAction(nameof(Details), new { rulesetId });
    }

    [HttpGet("{rulesetId:guid}")]
    /// <summary>
    /// Menjalankan fungsi Details sebagai bagian dari alur file ini.
    /// </summary>
    public async Task<IActionResult> Details(Guid rulesetId, int? version, string? source, Guid? defaultRulesetVersionId, CancellationToken ct)
    {
        var fromDefaultCatalog = string.Equals(source, DefaultCatalogSource, StringComparison.OrdinalIgnoreCase);
        var requestedVersion = version.HasValue && version.Value > 0 ? version : null;
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
            if (fromDefaultCatalog)
            {
                var defaultsResponse = await client.GetAsync("api/v1/rulesets/components/defaults", ct);
                unauthorized = this.HandleUnauthorizedApiResponse(defaultsResponse);
                if (unauthorized is not null)
                {
                    return unauthorized;
                }

                if (defaultsResponse.IsSuccessStatusCode)
                {
                    var defaultsData = await TryReadJsonAsync<DefaultRulesetComponentsResponseDto>(defaultsResponse.Content, ct);
                    var fallbackItem = defaultsData?.Items?.FirstOrDefault(item =>
                        item.RulesetId == rulesetId &&
                        (requestedVersion is null || item.Version == requestedVersion.Value) &&
                        (!defaultRulesetVersionId.HasValue || item.RulesetVersionId == defaultRulesetVersionId.Value));
                    if (fallbackItem is not null)
                    {
                        return View(new RulesetDetailViewModel
                        {
                            Ruleset = new RulesetDetailResponseDto(
                                fallbackItem.RulesetId,
                                fallbackItem.Name,
                                fallbackItem.Description,
                                new List<RulesetVersionItemDto>(),
                                null),
                            Components = new RulesetComponentsResponseDto(
                                fallbackItem.RulesetId,
                                fallbackItem.RulesetVersionId,
                                fallbackItem.Version,
                                fallbackItem.Mode,
                                fallbackItem.ComponentCatalog),
                            InfoMessage = HttpContext.T("rulesets.info.default_catalog_readonly"),
                            IsReadOnly = true,
                            IsDefaultCatalogSource = true
                        });
                    }
                }
            }

            return View(new RulesetDetailViewModel
            {
                ErrorMessage = error?.Message ?? HttpContext
                    .T("rulesets.error.load_detail_failed")
                    .Replace("{status}", ((int)response.StatusCode).ToString())
            });
        }

        var data = await TryReadJsonAsync<RulesetDetailResponseDto>(response.Content, ct);
        if (data is null)
        {
            return View(new RulesetDetailViewModel
            {
                ErrorMessage = HttpContext.T("rulesets.error.invalid_detail_response")
            });
        }

        RulesetComponentsResponseDto? components = null;
        string? componentsErrorMessage = null;
        var componentsPath = requestedVersion.HasValue
            ? $"api/v1/rulesets/{rulesetId}/components?version={requestedVersion.Value}"
            : $"api/v1/rulesets/{rulesetId}/components";
        var componentsResponse = await client.GetAsync(componentsPath, ct);
        unauthorized = this.HandleUnauthorizedApiResponse(componentsResponse);
        if (unauthorized is not null)
        {
            return unauthorized;
        }

        if (!componentsResponse.IsSuccessStatusCode)
        {
            componentsErrorMessage = HttpContext
                .T("rulesets.error.load_components_failed")
                .Replace("{status}", ((int)componentsResponse.StatusCode).ToString());
        }
        else
        {
            components = await TryReadJsonAsync<RulesetComponentsResponseDto>(componentsResponse.Content, ct);
            if (components is null)
            {
                componentsErrorMessage = HttpContext.T("rulesets.error.invalid_components_response");
            }
        }

        var tempInfo = TempData[RulesetInfoTempDataKey] as string;
        var infoMessages = new List<string>();
        if (!string.IsNullOrWhiteSpace(tempInfo))
        {
            infoMessages.Add(tempInfo);
        }

        if (fromDefaultCatalog)
        {
            infoMessages.Add(HttpContext.T("rulesets.info.default_catalog_readonly"));
        }

        if (requestedVersion.HasValue)
        {
            infoMessages.Add(HttpContext
                .T("rulesets.info.viewing_version")
                .Replace("{version}", $"v{requestedVersion.Value}"));
        }

        return View(new RulesetDetailViewModel
        {
            Ruleset = data,
            Components = components,
            ErrorMessage = TempData[RulesetErrorTempDataKey] as string,
            InfoMessage = infoMessages.Count == 0 ? null : string.Join(" ", infoMessages),
            ComponentsErrorMessage = componentsErrorMessage,
            IsReadOnly = fromDefaultCatalog,
            IsDefaultCatalogSource = fromDefaultCatalog
        });
    }

    [HttpGet("default-components/{rulesetVersionId:guid}")]
    /// <summary>
    /// Membuka rincian komponen default dengan resolusi versi agar tautan tetap stabil.
    /// </summary>
    public async Task<IActionResult> DefaultComponentDetails(Guid rulesetVersionId, CancellationToken ct)
    {
        var client = _clientFactory.CreateClient("Api");
        var defaultsResponse = await client.GetAsync("api/v1/rulesets/components/defaults", ct);
        var unauthorized = this.HandleUnauthorizedApiResponse(defaultsResponse);
        if (unauthorized is not null)
        {
            return unauthorized;
        }

        if (!defaultsResponse.IsSuccessStatusCode)
        {
            TempData[RulesetErrorTempDataKey] = HttpContext
                .T("rulesets.error.load_default_components_failed")
                .Replace("{status}", ((int)defaultsResponse.StatusCode).ToString());
            return RedirectToAction(nameof(Index));
        }

        var defaultsData = await TryReadJsonAsync<DefaultRulesetComponentsResponseDto>(defaultsResponse.Content, ct);
        if (defaultsData is null)
        {
            TempData[RulesetErrorTempDataKey] = HttpContext.T("rulesets.error.invalid_default_components_response");
            return RedirectToAction(nameof(Index));
        }

        var selectedItem = defaultsData.Items.FirstOrDefault(item => item.RulesetVersionId == rulesetVersionId);
        if (selectedItem is null)
        {
            TempData[RulesetErrorTempDataKey] = HttpContext.T("rulesets.error.default_component_not_found");
            return RedirectToAction(nameof(Index));
        }

        return RedirectToAction(nameof(Details), new
        {
            rulesetId = selectedItem.RulesetId,
            version = selectedItem.Version,
            source = DefaultCatalogSource,
            defaultRulesetVersionId = selectedItem.RulesetVersionId
        });
    }

    [HttpPost("{rulesetId:guid}/versions/{version:int}/activate")]
    /// <summary>
    /// Menjalankan fungsi ActivateVersion sebagai bagian dari alur file ini.
    /// </summary>
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
                HttpContext.T("rulesets.error.activate_version_failed"),
                ct);
        }

        return RedirectToAction(nameof(Details), new { rulesetId });
    }

    [HttpPost("{rulesetId:guid}/versions/{version:int}/delete")]
    /// <summary>
    /// Menjalankan fungsi DeleteVersion sebagai bagian dari alur file ini.
    /// </summary>
    public async Task<IActionResult> DeleteVersion(Guid rulesetId, int version, CancellationToken ct)
    {
        if (!HttpContext.Session.IsInstructor())
        {
            return RedirectToAction(nameof(Details), new { rulesetId });
        }

        var client = _clientFactory.CreateClient("Api");
        var response = await client.DeleteAsync($"api/v1/rulesets/{rulesetId}/versions/{version}", ct);
        var unauthorized = this.HandleUnauthorizedApiResponse(response);
        if (unauthorized is not null)
        {
            return unauthorized;
        }

        if (!response.IsSuccessStatusCode && response.StatusCode != System.Net.HttpStatusCode.NoContent)
        {
            TempData[RulesetErrorTempDataKey] = await BuildRulesetApiErrorMessage(
                response,
                HttpContext.T("rulesets.error.delete_version_failed"),
                ct);
        }
        else
        {
            TempData[RulesetInfoTempDataKey] = HttpContext
                .T("rulesets.delete_version_success")
                .Replace("{version}", $"v{version}");
        }

        return RedirectToAction(nameof(Details), new { rulesetId });
    }

    [HttpPost("{rulesetId:guid}/delete")]
    /// <summary>
    /// Menjalankan fungsi Delete sebagai bagian dari alur file ini.
    /// </summary>
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
                HttpContext.T("rulesets.error.delete_failed"),
                ct);
            return RedirectToAction(nameof(Details), new { rulesetId });
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost("bulk-delete")]
    /// <summary>
    /// Menjalankan fungsi BulkDelete sebagai bagian dari alur file ini.
    /// </summary>
    public async Task<IActionResult> BulkDelete([FromForm(Name = "rulesetIds")] List<Guid>? rulesetIds, CancellationToken ct)
    {
        if (!HttpContext.Session.IsInstructor())
        {
            return RedirectToAction(nameof(Index));
        }

        var selectedRulesetIds = (rulesetIds ?? []).Distinct().ToList();
        if (selectedRulesetIds.Count == 0)
        {
            TempData[RulesetErrorTempDataKey] = HttpContext.T("rulesets.error.bulk_delete_empty");
            return RedirectToAction(nameof(Index));
        }

        var client = _clientFactory.CreateClient("Api");
        var deletedCount = 0;
        var failedCount = 0;

        foreach (var rulesetId in selectedRulesetIds)
        {
            var response = await client.DeleteAsync($"api/v1/rulesets/{rulesetId}", ct);
            var unauthorized = this.HandleUnauthorizedApiResponse(response);
            if (unauthorized is not null)
            {
                return unauthorized;
            }

            if (response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                deletedCount++;
                continue;
            }

            failedCount++;
        }

        if (deletedCount > 0 && failedCount == 0)
        {
            TempData[RulesetInfoTempDataKey] = HttpContext
                .T("rulesets.bulk_delete_success")
                .Replace("{count}", deletedCount.ToString());
        }
        else if (deletedCount > 0)
        {
            TempData[RulesetErrorTempDataKey] = HttpContext
                .T("rulesets.bulk_delete_partial")
                .Replace("{success}", deletedCount.ToString())
                .Replace("{failed}", failedCount.ToString());
        }
        else
        {
            TempData[RulesetErrorTempDataKey] = HttpContext
                .T("rulesets.bulk_delete_failed")
                .Replace("{failed}", failedCount.ToString());
        }

        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Menjalankan fungsi BuildDefaultCreateViewModel sebagai bagian dari alur file ini.
    /// </summary>
    private static CreateRulesetViewModel BuildDefaultCreateViewModel()
    {
        return new CreateRulesetViewModel
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
        };
    }

    /// <summary>
    /// Menambahkan component_catalog default berdasarkan mode bila konfigurasi belum memilikinya.
    /// </summary>
    private async Task<JsonNode?> EnsureComponentCatalogAsync(JsonNode? configNode, HttpClient client, CancellationToken ct)
    {
        if (configNode is not JsonObject configObject)
        {
            return configNode;
        }

        if (configObject.TryGetPropertyValue("component_catalog", out var existingCatalog) && existingCatalog is not null)
        {
            return configNode;
        }

        if (!TryResolveMode(configObject, out var mode))
        {
            return configNode;
        }

        var defaultsResponse = await client.GetAsync($"api/v1/rulesets/components/defaults?mode={Uri.EscapeDataString(mode)}", ct);
        if (!defaultsResponse.IsSuccessStatusCode)
        {
            return configNode;
        }

        var defaultsData = await TryReadJsonAsync<DefaultRulesetComponentsResponseDto>(defaultsResponse.Content, ct);
        if (defaultsData?.Items is null || defaultsData.Items.Count == 0)
        {
            return configNode;
        }

        var selectedCatalog = defaultsData.Items
            .FirstOrDefault(item =>
                string.Equals(item.Mode, mode, StringComparison.OrdinalIgnoreCase) &&
                item.ComponentCatalog.HasValue)
            ?.ComponentCatalog
            ?? defaultsData.Items.FirstOrDefault(item => item.ComponentCatalog.HasValue)?.ComponentCatalog;

        if (!selectedCatalog.HasValue)
        {
            return configNode;
        }

        try
        {
            configObject["component_catalog"] = JsonNode.Parse(selectedCatalog.Value.GetRawText());
        }
        catch (JsonException)
        {
            return configNode;
        }

        return configNode;
    }

    /// <summary>
    /// Menentukan mode ruleset (PEMULA/MAHIR) dari konfigurasi JSON.
    /// </summary>
    private static bool TryResolveMode(JsonObject configObject, out string mode)
    {
        mode = string.Empty;
        if (!configObject.TryGetPropertyValue("mode", out var modeNode))
        {
            return false;
        }

        if (modeNode is not JsonValue modeValue || !modeValue.TryGetValue<string>(out var rawMode))
        {
            return false;
        }

        var modeText = rawMode?.Trim().ToUpperInvariant();
        if (!string.Equals(modeText, "PEMULA", StringComparison.Ordinal) &&
            !string.Equals(modeText, "MAHIR", StringComparison.Ordinal))
        {
            return false;
        }

        mode = modeText!;
        return true;
    }

    /// <summary>
    /// Menjalankan fungsi BuildRulesetApiErrorMessage sebagai bagian dari alur file ini.
    /// </summary>
    private static async Task<string> BuildRulesetApiErrorMessage(HttpResponseMessage response, string prefix, CancellationToken ct)
    {
        var error = await TryReadJsonAsync<ApiErrorResponseDto>(response.Content, ct);

        return error?.Message ?? $"{prefix}. Status: {(int)response.StatusCode}";
    }

    /// <summary>
    /// Menjalankan fungsi SerializeIndentedJson sebagai bagian dari alur file ini.
    /// </summary>
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

    /// <summary>
    /// Membaca JSON dari konten HTTP dengan fallback default saat payload tidak kompatibel.
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

