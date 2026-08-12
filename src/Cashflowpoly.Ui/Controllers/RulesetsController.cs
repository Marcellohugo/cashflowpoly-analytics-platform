// Fungsi file: Menangani request MVC dan penyusunan tampilan untuk RulesetsController.
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using Cashflowpoly.Ui.Contracts;
using Cashflowpoly.Ui.Domain;
using Cashflowpoly.Ui.Infrastructure;
using Cashflowpoly.Ui.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Cashflowpoly.Ui.Controllers;

[Route("rulesets")]
public sealed class RulesetsController : Controller
{
    private readonly IHttpClientFactory _clientFactory;
    private const string RulesetErrorTempDataKey = "ruleset_error";
    private const string RulesetInfoTempDataKey = "ruleset_info";
    private const string DefaultCatalogSource = "default-catalog";

    public RulesetsController(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!HttpContext.Session.IsInstructor())
        {
            context.Result = RedirectToAction("Index", "Sessions");
            return;
        }

        base.OnActionExecuting(context);
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var flashError = TempData[RulesetErrorTempDataKey] as string;
        ViewData[RulesetErrorTempDataKey] = flashError;
        ViewData[RulesetInfoTempDataKey] = TempData[RulesetInfoTempDataKey] as string;

        var client = _clientFactory.CreateClient("Api");
        var response = await client.GetAsync("api/v1/rulesets", ct);
        var unauthorized = this.HandleUnauthorizedApiResponse(response);
        if (unauthorized is not null)
        {
            return unauthorized;
        }

        var rulesetItems = new List<RulesetListItem>();
        string? rulesetErrorMessage = null;
        if (!response.IsSuccessStatusCode)
        {
            rulesetErrorMessage = HttpContext
                .T("rulesets.error.load_list_failed")
                .Replace("{status}", ((int)response.StatusCode).ToString());
        }
        else
        {
            var data = await response.Content.TryReadFromJsonAsync<RulesetListResponse>(ct);
            if (data is null)
            {
                rulesetErrorMessage = HttpContext.T("rulesets.error.invalid_list_response");
            }
            else
            {
                rulesetItems = data.Items ?? new List<RulesetListItem>();
            }
        }

        return View(new RulesetListViewModel
        {
            Items = rulesetItems,
            ErrorMessage = rulesetErrorMessage
        });
    }

    [HttpGet("create")]
    public IActionResult Create()
    {
        return View(RulesetFormHelper.BuildDefaultCreateViewModel());
    }

    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateRulesetViewModel model, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(model.Name))
        {
            model.IsEditMode = false;
            model.ErrorMessage = HttpContext.T("rulesets.error.name_required");
            return View(model);
        }

        JsonNode? configNode;
        try
        {
            configNode = JsonNode.Parse(model.DefinitionJson);
        }
        catch (JsonException)
        {
            model.IsEditMode = false;
            model.ErrorMessage = HttpContext.T("rulesets.error.invalid_definition_json");
            return View(model);
        }

        var client = _clientFactory.CreateClient("Api");
        configNode = await EnsureComponentCatalogAsync(configNode, client, ct);
        var definition = RulesetDefinitionMapper.FromConfigJson(configNode?.ToJsonString() ?? "{}");
        var payload = new
        {
            name = model.Name,
            description = model.Description,
            definition
        };

        var response = await client.PostAsJsonAsync("api/v1/rulesets", payload, ct);
        var unauthorized = this.HandleUnauthorizedApiResponse(response);
        if (unauthorized is not null)
        {
            return unauthorized;
        }

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.TryReadFromJsonAsync<ErrorResponse>(ct);
            model.IsEditMode = false;
            model.ErrorMessage = error?.Message ?? HttpContext
                .T("rulesets.error.create_failed")
                .Replace("{status}", ((int)response.StatusCode).ToString());
            return View(model);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("{rulesetId:guid}/edit")]
    public async Task<IActionResult> Edit(Guid rulesetId, CancellationToken ct)
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
            var error = await response.Content.TryReadFromJsonAsync<ErrorResponse>(ct);
            return View("Create", new CreateRulesetViewModel
            {
                RulesetId = rulesetId,
                IsEditMode = true,
                ErrorMessage = error?.Message ?? HttpContext
                    .T("rulesets.error.load_for_edit_failed")
                    .Replace("{status}", ((int)response.StatusCode).ToString())
            });
        }

        var data = await response.Content.TryReadFromJsonAsync<RulesetDetailResponse>(ct);
        if (data is null)
        {
            return View("Create", new CreateRulesetViewModel
            {
                RulesetId = rulesetId,
                IsEditMode = true,
                ErrorMessage = HttpContext.T("rulesets.error.invalid_detail_response")
            });
        }

        if (data.IsDefault || data.IsLockedBySession)
        {
            TempData[RulesetInfoTempDataKey] = HttpContext.T("rulesets.readonly_hint");
            return RedirectToAction(nameof(Details), new { rulesetId });
        }

        return View("Create", new CreateRulesetViewModel
        {
            RulesetId = rulesetId,
            IsEditMode = true,
            Name = data.Name,
            Description = data.Description,
            DefinitionJson = SerializeDefinitionConfig(data.Definition)
        });
    }

    [HttpPost("{rulesetId:guid}/edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid rulesetId, CreateRulesetViewModel model, CancellationToken ct)
    {
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
            configNode = JsonNode.Parse(model.DefinitionJson);
        }
        catch (JsonException)
        {
            model.ErrorMessage = HttpContext.T("rulesets.error.invalid_definition_json");
            return View("Create", model);
        }

        var client = _clientFactory.CreateClient("Api");
        configNode = await EnsureComponentCatalogAsync(configNode, client, ct);
        var definition = RulesetDefinitionMapper.FromConfigJson(configNode?.ToJsonString() ?? "{}");
        var payload = new
        {
            name = model.Name,
            description = model.Description,
            definition
        };

        var response = await client.PutAsJsonAsync($"api/v1/rulesets/{rulesetId}", payload, ct);
        var unauthorized = this.HandleUnauthorizedApiResponse(response);
        if (unauthorized is not null)
        {
            return unauthorized;
        }

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.TryReadFromJsonAsync<ErrorResponse>(ct);
            model.ErrorMessage = error?.Message ?? HttpContext
                .T("rulesets.error.update_failed")
                .Replace("{status}", ((int)response.StatusCode).ToString());
            return View("Create", model);
        }

        return RedirectToAction(nameof(Details), new { rulesetId });
    }

    [HttpGet("{rulesetId:guid}")]
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
            var error = await response.Content.TryReadFromJsonAsync<ErrorResponse>(ct);
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
                    var defaultsData = await defaultsResponse.Content.TryReadFromJsonAsync<DefaultRulesetComponentsResponse>(ct);
                    var fallbackItem = defaultsData?.Items?.FirstOrDefault(item =>
                        item.RulesetId == rulesetId &&
                        (requestedVersion is null || item.Version == requestedVersion.Value) &&
                        (!defaultRulesetVersionId.HasValue || item.RulesetVersionId == defaultRulesetVersionId.Value));
                    if (fallbackItem is not null)
                    {
                        return View(new RulesetDetailViewModel
                        {
                            Ruleset = new RulesetDetailResponse(
                                fallbackItem.RulesetId,
                                fallbackItem.Name,
                                fallbackItem.Description,
                                new List<RulesetVersionItem>(),
                                fallbackItem.RulesetVersionId,
                                fallbackItem.Version,
                                fallbackItem.Mode,
                                fallbackItem.Definition),
                            Components = new RulesetComponentsResponse(
                                fallbackItem.RulesetId,
                                fallbackItem.RulesetVersionId,
                                fallbackItem.Version,
                                fallbackItem.Mode,
                                fallbackItem.Definition),
                            CompatibilityDefinitionJson = BuildCompatibilityConfigElement(fallbackItem.Definition),
                            CompatibilityComponentCatalog = BuildCompatibilityComponentCatalog(fallbackItem.Definition),
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

        var data = await response.Content.TryReadFromJsonAsync<RulesetDetailResponse>(ct);
        if (data is null)
        {
            return View(new RulesetDetailViewModel
            {
                ErrorMessage = HttpContext.T("rulesets.error.invalid_detail_response")
            });
        }

        RulesetComponentsResponse? components = null;
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
            components = await componentsResponse.Content.TryReadFromJsonAsync<RulesetComponentsResponse>(ct);
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

        var isReadOnly = fromDefaultCatalog || data.IsDefault || data.IsLockedBySession;
        if (fromDefaultCatalog || data.IsDefault)
        {
            infoMessages.Add(HttpContext.T("rulesets.info.default_catalog_readonly"));
        }
        else if (data.IsLockedBySession)
        {
            infoMessages.Add(HttpContext.T("rulesets.readonly_hint"));
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
            CompatibilityDefinitionJson = BuildCompatibilityConfigElement(data.Definition),
            CompatibilityComponentCatalog = BuildCompatibilityComponentCatalog(components?.Definition ?? data.Definition),
            ErrorMessage = TempData[RulesetErrorTempDataKey] as string,
            InfoMessage = infoMessages.Count == 0 ? null : string.Join(" ", infoMessages),
            ComponentsErrorMessage = componentsErrorMessage,
            IsReadOnly = isReadOnly,
            IsDefaultCatalogSource = fromDefaultCatalog
        });
    }

    [HttpGet("default-components/{rulesetVersionId:guid}")]
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

        var defaultsData = await defaultsResponse.Content.TryReadFromJsonAsync<DefaultRulesetComponentsResponse>(ct);
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
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ActivateVersion(Guid rulesetId, int version, CancellationToken ct)
    {
        var client = _clientFactory.CreateClient("Api");
        var response = await client.PostAsync($"api/v1/rulesets/{rulesetId}/versions/{version}/activate", null, ct);
        var unauthorized = this.HandleUnauthorizedApiResponse(response);
        if (unauthorized is not null)
        {
            return unauthorized;
        }

        if (!response.IsSuccessStatusCode)
        {
            TempData[RulesetErrorTempDataKey] = await RulesetFormHelper.BuildRulesetApiErrorMessage(
                response,
                HttpContext.T("rulesets.error.activate_version_failed"),
                ct);
        }

        return RedirectToAction(nameof(Details), new { rulesetId });
    }

    [HttpPost("{rulesetId:guid}/versions/{version:int}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteVersion(Guid rulesetId, int version, CancellationToken ct)
    {
        var client = _clientFactory.CreateClient("Api");
        var response = await client.DeleteAsync($"api/v1/rulesets/{rulesetId}/versions/{version}", ct);
        var unauthorized = this.HandleUnauthorizedApiResponse(response);
        if (unauthorized is not null)
        {
            return unauthorized;
        }

        if (!response.IsSuccessStatusCode)
        {
            TempData[RulesetErrorTempDataKey] = await RulesetFormHelper.BuildRulesetApiErrorMessage(
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
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid rulesetId, CancellationToken ct)
    {
        var client = _clientFactory.CreateClient("Api");
        var response = await client.DeleteAsync($"api/v1/rulesets/{rulesetId}", ct);
        var unauthorized = this.HandleUnauthorizedApiResponse(response);
        if (unauthorized is not null)
        {
            return unauthorized;
        }

        if (!response.IsSuccessStatusCode)
        {
            TempData[RulesetErrorTempDataKey] = await RulesetFormHelper.BuildRulesetApiErrorMessage(
                response,
                HttpContext.T("rulesets.error.delete_failed"),
                ct);
            return RedirectToAction(nameof(Details), new { rulesetId });
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost("bulk-delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> BulkDelete([FromForm(Name = "rulesetIds")] List<Guid>? rulesetIds, CancellationToken ct)
    {
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

            if (response.IsSuccessStatusCode)
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

        if (!RulesetFormHelper.TryResolveMode(configObject, out var mode))
        {
            return configNode;
        }

        var defaultsResponse = await client.GetAsync($"api/v1/rulesets/components/defaults?mode={Uri.EscapeDataString(mode)}", ct);
        if (!defaultsResponse.IsSuccessStatusCode)
        {
            return configNode;
        }

        var defaultsData = await defaultsResponse.Content.TryReadFromJsonAsync<DefaultRulesetComponentsResponse>(ct);
        if (defaultsData?.Items is null || defaultsData.Items.Count == 0)
        {
            return configNode;
        }

        JsonElement? selectedCatalog = null;
        foreach (var item in defaultsData.Items)
        {
            var catalog = BuildCompatibilityComponentCatalog(item.Definition);
            if (catalog.HasValue && string.Equals(item.Mode, mode, StringComparison.OrdinalIgnoreCase))
            {
                selectedCatalog = catalog;
                break;
            }

            if (!selectedCatalog.HasValue && catalog.HasValue)
            {
                selectedCatalog = catalog;
            }
        }

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

    private static string SerializeDefinitionConfig(RulesetDefinitionDto? definition)
    {
        var element = BuildCompatibilityConfigElement(definition);
        return RulesetFormHelper.SerializeIndentedJson(element);
    }

    private static JsonElement? BuildCompatibilityConfigElement(RulesetDefinitionDto? definition)
        => RulesetDefinitionMapper.ToConfigElement(definition);

    private static JsonElement? BuildCompatibilityComponentCatalog(RulesetDefinitionDto? definition)
    {
        var config = BuildCompatibilityConfigElement(definition);
        if (!config.HasValue || config.Value.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        return config.Value.TryGetProperty("component_catalog", out var componentCatalog)
            ? componentCatalog.Clone()
            : null;
    }

}

