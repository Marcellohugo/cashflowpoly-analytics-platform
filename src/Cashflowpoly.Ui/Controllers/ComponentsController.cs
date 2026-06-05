using Cashflowpoly.Ui.Contracts;
using Cashflowpoly.Ui.Infrastructure;
using Cashflowpoly.Ui.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cashflowpoly.Ui.Controllers;

[Route("components")]
public sealed class ComponentsController : Controller
{
    private readonly IHttpClientFactory _clientFactory;
    private const string RulesetErrorTempDataKey = "ruleset_error";

    public ComponentsController(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var flashError = TempData[RulesetErrorTempDataKey] as string;
        var client = _clientFactory.CreateClient("Api");
        var response = await client.GetAsync("api/v1/rulesets/components/defaults", ct);
        var unauthorized = this.HandleUnauthorizedApiResponse(response);
        if (unauthorized is not null)
        {
            return unauthorized;
        }

        if (!response.IsSuccessStatusCode)
        {
            return View(new ComponentCatalogListViewModel
            {
                ErrorMessage = flashError ?? HttpContext
                    .T("rulesets.error.load_default_components_failed")
                    .Replace("{status}", ((int)response.StatusCode).ToString())
            });
        }

        var data = await response.Content.TryReadFromJsonAsync<DefaultRulesetComponentsResponse>(ct);
        if (data is null)
        {
            return View(new ComponentCatalogListViewModel
            {
                ErrorMessage = flashError ?? HttpContext.T("rulesets.error.invalid_default_components_response")
            });
        }

        return View(new ComponentCatalogListViewModel
        {
            Items = data.Items ?? new List<DefaultRulesetComponentItem>(),
            ErrorMessage = flashError
        });
    }
}
