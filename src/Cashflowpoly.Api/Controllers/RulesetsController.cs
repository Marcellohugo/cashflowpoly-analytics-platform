using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Domain;
using Cashflowpoly.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cashflowpoly.Api.Controllers;

[ApiController]
[Route("api/rulesets")]
public sealed class RulesetsController : ControllerBase
{
    private readonly RulesetRepository _rulesets;

    public RulesetsController(RulesetRepository rulesets)
    {
        _rulesets = rulesets;
    }

    [HttpPost]
    public async Task<IActionResult> CreateRuleset([FromBody] CreateRulesetRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest(ApiErrorHelper.BuildError(HttpContext, "VALIDATION_ERROR", "Field wajib tidak lengkap",
                new ErrorDetail("name", "REQUIRED")));
        }
        if (!RulesetConfigParser.TryParse(request.Config, out _, out var configErrors))
        {
            return BadRequest(ApiErrorHelper.BuildError(HttpContext, "VALIDATION_ERROR", "Konfigurasi ruleset tidak valid", configErrors.ToArray()));
        }

        var configJson = request.Config.GetRawText();
        var created = await _rulesets.CreateRulesetAsync(request.Name, request.Description, configJson, null, ct);

        return Created($"/api/rulesets/{created.RulesetId}", new CreateRulesetResponse(created.RulesetId, created.Version));
    }

    [HttpPut("{rulesetId:guid}")]
    public async Task<IActionResult> UpdateRuleset(Guid rulesetId, [FromBody] UpdateRulesetRequest request, CancellationToken ct)
    {
        var existing = await _rulesets.GetRulesetAsync(rulesetId, ct);
        if (existing is null)
        {
            return NotFound(ApiErrorHelper.BuildError(HttpContext, "NOT_FOUND", "Ruleset tidak ditemukan"));
        }

        if (request.Config is null)
        {
            return BadRequest(ApiErrorHelper.BuildError(HttpContext, "VALIDATION_ERROR", "Config wajib ada",
                new ErrorDetail("config", "REQUIRED")));
        }
        if (!RulesetConfigParser.TryParse(request.Config.Value, out _, out var configErrors))
        {
            return BadRequest(ApiErrorHelper.BuildError(HttpContext, "VALIDATION_ERROR", "Konfigurasi ruleset tidak valid", configErrors.ToArray()));
        }

        var configJson = request.Config.Value.GetRawText();
        var nextVersion = await _rulesets.CreateRulesetVersionAsync(rulesetId, request.Name, request.Description, configJson, null, ct);

        return Ok(new CreateRulesetResponse(rulesetId, nextVersion));
    }

    [HttpGet]
    public async Task<IActionResult> ListRulesets(CancellationToken ct)
    {
        var items = await _rulesets.ListRulesetsAsync(ct);
        return Ok(new RulesetListResponse(items));
    }
}
