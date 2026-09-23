// Fungsi file: Menangani request MVC dan penyusunan tampilan untuk RulesetsController.
// Mengimpor namespace `System.Net.Http.Json` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Net.Http.Json;
// Mengimpor namespace `System.Text.Json` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.Json;
// Mengimpor namespace `System.Text.Json.Nodes` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.Json.Nodes;
// Mengimpor namespace `Cashflowpoly.Ui.Contracts` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Ui.Contracts;
// Mengimpor namespace `Cashflowpoly.Ui.Domain` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Ui.Domain;
// Mengimpor namespace `Cashflowpoly.Ui.Infrastructure` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Ui.Infrastructure;
// Mengimpor namespace `Cashflowpoly.Ui.Models` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Ui.Models;
// Mengimpor namespace `Microsoft.AspNetCore.Mvc` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.AspNetCore.Mvc;
// Mengimpor namespace `Microsoft.AspNetCore.Mvc.Filters` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.AspNetCore.Mvc.Filters;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Controllers` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Controllers;

// menetapkan pola rute (”rulesets”) untuk pencocokan URL permintaan.
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
        if (!HttpContext.IsInstructor())
        {
            context.Result = RedirectToAction("Index", "Sessions");
            return;
        }

        base.OnActionExecuting(context);
    }

    // mendaftarkan action untuk metode HTTP GET pada rute (””).
    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        try
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
                if (data?.Items is null)
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
                RulesetsAvailable = rulesetErrorMessage is null,
                ErrorMessage = rulesetErrorMessage
            });

        }
        catch (HttpRequestException)
        {
            return View(new RulesetListViewModel { ErrorMessage = HttpContext.T("auth.error.api_unavailable") });
        }
        catch (TaskCanceledException) when (!ct.IsCancellationRequested)
        {
            return View(new RulesetListViewModel { ErrorMessage = HttpContext.T("auth.error.api_unavailable") });
        }
    }

    // mendaftarkan action untuk metode HTTP GET pada rute (”create”).
    [HttpGet("create")]
    public IActionResult Create()
    {
        return View(RulesetFormHelper.BuildDefaultCreateViewModel());
    }

    // mendaftarkan action untuk metode HTTP POST pada rute (”create”).
    [HttpPost("create")]
    // memvalidasi token antiforgery untuk memastikan permintaan formulir membawa token yang sesuai.
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateRulesetViewModel model, CancellationToken ct)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                model.IsEditMode = false;
                model.ErrorMessage = HttpContext.T("rulesets.error.name_required");
                return View(model);
            }

            if (model.Name.Length > 120)
            {
                model.ErrorMessage = HttpContext.T("rulesets.error.name_too_long");
                return View("Create", model);
            }

            JsonNode? configNode;
            try
            {
                configNode = JsonNode.Parse(model.DefinitionJson);
            }
            // Menangani exception `JsonException` melalui variabel dalam Create.
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
        catch (HttpRequestException)
        {
            model.IsEditMode = false;
            model.ErrorMessage = HttpContext.T("auth.error.api_unavailable");
            return View(model);
        }
        catch (Exception ex) when (ex is JsonException or KeyNotFoundException or InvalidOperationException or FormatException or OverflowException)
        {
            model.IsEditMode = false;
            model.ErrorMessage = HttpContext.T("rulesets.error.invalid_definition_json");
            return View(model);
        }
        catch (TaskCanceledException) when (!ct.IsCancellationRequested)
        {
            model.IsEditMode = false;
            model.ErrorMessage = HttpContext.T("auth.error.api_unavailable");
            return View(model);
        }
    }

    // mendaftarkan action untuk metode HTTP GET pada rute (”{rulesetId:guid}/edit”).
    [HttpGet("{rulesetId:guid}/edit")]
    public async Task<IActionResult> Edit(Guid rulesetId, [FromQuery] int? version, CancellationToken ct)
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

        if (data.IsDefault)
        {
            TempData[RulesetInfoTempDataKey] = HttpContext.T("rulesets.readonly_hint");
            return RedirectToAction(nameof(Details), new { rulesetId });
        }

        string definitionJson = SerializeDefinitionConfig(data.Definition);
        if (version.HasValue && version.Value > 0)
        {
            var compResponse = await client.GetAsync($"api/v1/rulesets/{rulesetId}/components?version={version.Value}", ct);
            if (compResponse.IsSuccessStatusCode)
            {
                var compData = await compResponse.Content.TryReadFromJsonAsync<RulesetComponentsResponse>(ct);
                if (compData?.Definition is not null)
                {
                    definitionJson = SerializeDefinitionConfig(compData.Definition);
                }
            }
        }

        return View("Create", new CreateRulesetViewModel
        {
            RulesetId = rulesetId,
            IsEditMode = true,
            Name = data.Name,
            Description = data.Description,
            DefinitionJson = definitionJson
        });
    }

    // mendaftarkan action untuk metode HTTP GET pada rute (”{rulesetId:guid}/version-options”).
    [HttpGet("{rulesetId:guid}/version-options")]
    public async Task<IActionResult> GetVersionOptions(Guid rulesetId, CancellationToken ct)
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
            return StatusCode((int)response.StatusCode);
        }

        var data = await response.Content.TryReadFromJsonAsync<RulesetDetailResponse>(ct);
        if (data is null)
        {
            return NotFound();
        }

        return Json(new
        {
            rulesetId = data.RulesetId,
            name = data.Name,
            isDefault = data.IsDefault,
            isLockedBySession = data.IsLockedBySession,
            latestVersion = data.Version ?? data.Versions.FirstOrDefault()?.Version ?? 1,
            versions = data.Versions.Select(v => new
            {
                version = v.Version,
                status = v.Status,
                createdAt = v.CreatedAt.ToString("yyyy-MM-dd HH:mm"),
                isUsed = v.IsUsed
            }).OrderByDescending(v => v.version)
        });
    }

    // mendaftarkan action untuk metode HTTP POST pada rute (”{rulesetId:guid}/edit”).
    [HttpPost("{rulesetId:guid}/edit")]
    // memvalidasi token antiforgery untuk memastikan permintaan formulir membawa token yang sesuai.
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid rulesetId, CreateRulesetViewModel model, CancellationToken ct)
    {
        try
        {
            model.RulesetId = rulesetId;
            model.IsEditMode = true;

            if (string.IsNullOrWhiteSpace(model.Name))
            {
                model.ErrorMessage = HttpContext.T("rulesets.error.name_required");
                return View("Create", model);
            }

            if (model.Name.Length > 120)
            {
                model.ErrorMessage = HttpContext.T("rulesets.error.name_too_long");
                return View("Create", model);
            }

            JsonNode? configNode;
            try
            {
                configNode = JsonNode.Parse(model.DefinitionJson);
            }
            // Menangani exception `JsonException` melalui variabel dalam Edit.
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

            var created = await response.Content.TryReadFromJsonAsync<CreateRulesetResponse>(ct);
            if (created is not null && created.Version > 0)
            {
                await client.PostAsync($"api/v1/rulesets/{rulesetId}/versions/{created.Version}/activate", null, ct);
            }

            return RedirectToAction(nameof(Details), new { rulesetId });

        }
        catch (HttpRequestException)
        {
            model.ErrorMessage = HttpContext.T("auth.error.api_unavailable");
            return View("Create", model);
        }
        catch (Exception ex) when (ex is JsonException or KeyNotFoundException or InvalidOperationException or FormatException or OverflowException)
        {
            model.ErrorMessage = HttpContext.T("rulesets.error.invalid_definition_json");
            return View("Create", model);
        }
        catch (TaskCanceledException) when (!ct.IsCancellationRequested)
        {
            model.ErrorMessage = HttpContext.T("auth.error.api_unavailable");
            return View("Create", model);
        }
    }

    // mendaftarkan action untuk metode HTTP GET pada rute (”{rulesetId:guid}”).
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
        var displayedVersion = requestedVersion ?? data.Version;
        var componentsPath = displayedVersion.HasValue
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar:
            // $”api/v1/rulesets/{rulesetId}/components?version={requestedVersion.Value}” dalam Details.
            ? $"api/v1/rulesets/{rulesetId}/components?version={displayedVersion.Value}"
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: $”api/v1/rulesets/{rulesetId}/components”; dalam Details.
            : $"api/v1/rulesets/{rulesetId}/components";
        try
        {
            using var componentsResponse = await client.GetAsync(componentsPath, ct);
            unauthorized = this.HandleUnauthorizedApiResponse(componentsResponse);
            if (unauthorized is not null) return unauthorized;

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
                    componentsErrorMessage = HttpContext.T("rulesets.error.invalid_components_response");
            }
        }
        catch (HttpRequestException) { componentsErrorMessage = HttpContext.T("auth.error.api_unavailable"); }
        catch (TaskCanceledException) when (!ct.IsCancellationRequested)
        { componentsErrorMessage = HttpContext.T("auth.error.api_unavailable"); }

        var displayedDefinition = components?.Definition
            ?? (requestedVersion is null || requestedVersion == data.Version ? data.Definition : null);
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
            infoMessages.Add(HttpContext.T("rulesets.info.session_readonly"));
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
            CompatibilityDefinitionJson = BuildCompatibilityConfigElement(displayedDefinition),
            CompatibilityComponentCatalog = BuildCompatibilityComponentCatalog(displayedDefinition),
            ErrorMessage = TempData[RulesetErrorTempDataKey] as string,
            InfoMessage = infoMessages.Count == 0 ? null : string.Join(" ", infoMessages),
            ComponentsErrorMessage = componentsErrorMessage,
            IsReadOnly = isReadOnly,
            IsDefaultCatalogSource = fromDefaultCatalog
        });
    }

    // mendaftarkan action untuk metode HTTP GET pada rute (”default-components/{rulesetVersionId:guid}”).
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

    // mendaftarkan action untuk metode HTTP POST pada rute (”{rulesetId:guid}/versions/{version:int}/activate”).
    [HttpPost("{rulesetId:guid}/versions/{version:int}/activate")]
    // memvalidasi token antiforgery untuk memastikan permintaan formulir membawa token yang sesuai.
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

    // mendaftarkan action untuk metode HTTP POST pada rute (”{rulesetId:guid}/versions/{version:int}/delete”).
    [HttpPost("{rulesetId:guid}/versions/{version:int}/delete")]
    // memvalidasi token antiforgery untuk memastikan permintaan formulir membawa token yang sesuai.
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
            using var remainingRuleset = await client.GetAsync($"api/v1/rulesets/{rulesetId}", ct);
            if (remainingRuleset.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                TempData[RulesetInfoTempDataKey] = HttpContext.T("rulesets.delete_success");
                return RedirectToAction(nameof(Index));
            }

            TempData[RulesetInfoTempDataKey] = HttpContext
                .T("rulesets.delete_version_success")
                .Replace("{version}", $"v{version}");
        }

        return RedirectToAction(nameof(Details), new { rulesetId });
    }

    // mendaftarkan action untuk metode HTTP POST pada rute (”{rulesetId:guid}/delete”).
    [HttpPost("{rulesetId:guid}/delete")]
    // memvalidasi token antiforgery untuk memastikan permintaan formulir membawa token yang sesuai.
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

        TempData[RulesetInfoTempDataKey] = HttpContext.T("rulesets.delete_success");
        return RedirectToAction(nameof(Index));
    }

    // mendaftarkan action untuk metode HTTP POST pada rute (”bulk-delete”).
    [HttpPost("bulk-delete")]
    // memvalidasi token antiforgery untuk memastikan permintaan formulir membawa token yang sesuai.
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
        var lastErrorMessage = string.Empty;

        // Mengulangi setiap elemen `selectedRulesetIds`; elemen saat ini disimpan sebagai `rulesetId` bertipe `var` untuk diproses oleh badan loop dalam
        // BulkDelete.
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
                // Melewati sisa pernyataan pada iterasi saat ini dan melanjutkan ke elemen/iterasi berikutnya dalam BulkDelete.
                continue;
            }

            failedCount++;
            if (string.IsNullOrWhiteSpace(lastErrorMessage))
            {
                lastErrorMessage = await RulesetFormHelper.BuildRulesetApiErrorMessage(
                    response,
                    HttpContext.T("rulesets.error.delete_failed"),
                    ct);
            }
        }

        if (deletedCount > 0 && failedCount == 0)
        {
            TempData[RulesetInfoTempDataKey] = HttpContext
                .T("rulesets.bulk_delete_success")
                .Replace("{count}", deletedCount.ToString());
        }
        else if (deletedCount > 0)
        {
            var msg = HttpContext
                .T("rulesets.bulk_delete_partial")
                .Replace("{success}", deletedCount.ToString())
                .Replace("{failed}", failedCount.ToString());
            if (!string.IsNullOrWhiteSpace(lastErrorMessage))
            {
                msg += $" ({lastErrorMessage})";
            }
            TempData[RulesetErrorTempDataKey] = msg;
        }
        else
        {
            var msg = HttpContext
                .T("rulesets.bulk_delete_failed")
                .Replace("{failed}", failedCount.ToString());
            if (!string.IsNullOrWhiteSpace(lastErrorMessage))
            {
                msg += $" ({lastErrorMessage})";
            }
            TempData[RulesetErrorTempDataKey] = msg;
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task<JsonNode?> EnsureComponentCatalogAsync(JsonNode? configNode, HttpClient client, CancellationToken ct)
    {
        if (configNode is not JsonObject configObject ||
            !RulesetFormHelper.TryResolveMode(configObject, out var mode))
        {
            return configNode;
        }

        var needsAdvancedCatalog = mode == "MAHIR" &&
            (configObject["sharia_loans"] is not JsonArray { Count: > 0 } ||
             configObject["insurance_products"] is not JsonArray { Count: > 0 });
        if (!needsAdvancedCatalog && configObject["component_catalog"] is JsonObject &&
            configObject["actions"] is JsonArray { Count: > 0 })
        {
            return configNode;
        }

        var defaultsResponse = await client.GetAsync($"api/v1/rulesets/components/defaults?mode={Uri.EscapeDataString(mode)}", ct);
        if (!defaultsResponse.IsSuccessStatusCode)
        {
            return configNode;
        }

        var defaultsData = await defaultsResponse.Content.TryReadFromJsonAsync<DefaultRulesetComponentsResponse>(ct);
        var definition = defaultsData?.Items.FirstOrDefault(item =>
            string.Equals(item.Mode, mode, StringComparison.OrdinalIgnoreCase))?.Definition;
        if (definition is null)
        {
            return configNode;
        }

        // Keep all catalogs and action references, including fields the form does not expose.
        var defaults = JsonNode.Parse(RulesetDefinitionMapper.ToConfigJson(definition))!.AsObject();
        foreach (var (key, value) in defaults)
        {
            if (!configObject.ContainsKey(key) || configObject[key] is null)
            {
                configObject[key] = value?.DeepClone();
            }
        }
        if (needsAdvancedCatalog)
        {
            // Switching from Pemula keeps the existing common catalog and custom settings.
            // Only missing Mahir products and their action references come from the target mode.
            foreach (var key in new[] { "sharia_loans", "insurance_products", "life_risks" })
                if (configObject[key] is JsonArray { Count: 0 })
                    configObject[key] = defaults[key]?.DeepClone();
            if (configObject["component_catalog"] is JsonObject catalog &&
                catalog["tujuanFinansial"] is null or JsonArray { Count: 0 })
                catalog["tujuanFinansial"] = defaults["component_catalog"]?["tujuanFinansial"]?.DeepClone();
            if (configObject["actions"] is JsonArray actions)
            {
                var actionIds = actions.Select(action => action?["action_id"]?.GetValue<string>())
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);
                foreach (var action in definition.Actions.Where(action => actionIds.Add(action.ActionId)))
                    actions.Add(JsonSerializer.SerializeToNode(action));
            }
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
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: componentCatalog.Clone() dalam BuildCompatibilityComponentCatalog.
            ? componentCatalog.Clone()
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: null; dalam BuildCompatibilityComponentCatalog.
            : null;
    }

}
