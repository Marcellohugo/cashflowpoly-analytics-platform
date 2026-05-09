using Cashflowpoly.Api.Services;
using Cashflowpoly.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cashflowpoly.Api.Controllers;

[ApiController]
[Route("api/v1/analytics")]
[Authorize]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status429TooManyRequests)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
public sealed class AnalyticsController : ControllerBase
{
    private readonly IAnalyticsService _analytics;

    public AnalyticsController(IAnalyticsService analytics) => _analytics = analytics;

    [HttpPost("sessions/{sessionId:guid}/recompute")]
    [Authorize(Roles = "INSTRUCTOR")]
    [ProducesResponseType(typeof(AnalyticsSessionResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Recompute(Guid sessionId, CancellationToken ct)
    {
        var (result, status, error) = await _analytics.RecomputeAsync(sessionId, User, ct);
        return status == 200 ? Ok(result) : StatusCode(status, error);
    }

    [HttpGet("sessions/{sessionId:guid}")]
    [ProducesResponseType(typeof(AnalyticsSessionResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSessionAnalytics(Guid sessionId, CancellationToken ct)
    {
        var (result, status, error) = await _analytics.GetSessionAnalyticsAsync(sessionId, User, ct);
        return status == 200 ? Ok(result) : StatusCode(status, error);
    }

    [HttpGet("sessions/{sessionId:guid}/transactions")]
    [ProducesResponseType(typeof(TransactionHistoryResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTransactions(Guid sessionId, [FromQuery] Guid? playerId = null, CancellationToken ct = default)
    {
        var (result, status, error) = await _analytics.GetTransactionsAsync(sessionId, playerId, User, ct);
        return status == 200 ? Ok(result) : StatusCode(status, error);
    }

    [HttpGet("sessions/{sessionId:guid}/players/{playerId:guid}/gameplay")]
    [ProducesResponseType(typeof(GameplayMetricsResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetGameplayMetrics(Guid sessionId, Guid playerId, CancellationToken ct)
    {
        var (result, status, error) = await _analytics.GetGameplayMetricsAsync(sessionId, playerId, User, ct);
        return status == 200 ? Ok(result) : StatusCode(status, error);
    }

    [HttpGet("rulesets/{rulesetId:guid}/summary")]
    [ProducesResponseType(typeof(RulesetAnalyticsSummaryResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRulesetAnalyticsSummary(Guid rulesetId, CancellationToken ct)
    {
        var (result, status, error) = await _analytics.GetRulesetAnalyticsSummaryAsync(rulesetId, User, ct);
        return status == 200 ? Ok(result) : StatusCode(status, error);
    }
}
