// Fungsi file: Menangani endpoint, validasi akses, dan response HTTP untuk AnalyticsController.
using Cashflowpoly.Api.Services;
using Cashflowpoly.Api.Contracts;
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
    public async Task<IActionResult> GetTransactions(Guid sessionId, [FromQuery] Guid? userId = null, [FromQuery] string? cursor = null, [FromQuery] int limit = 50, CancellationToken ct = default)
    {
        var (result, status, error) = await _analytics.GetTransactionsAsync(sessionId, userId, cursor, limit, User, ct);
        return status == 200 ? Ok(result) : StatusCode(status, error);
    }

    [HttpGet("sessions/{sessionId:guid}/players/{userId:guid}/gameplay")]
    [ProducesResponseType(typeof(GameplayMetricsResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetGameplayMetrics(Guid sessionId, Guid userId, CancellationToken ct)
    {
        var (result, status, error) = await _analytics.GetGameplayMetricsAsync(sessionId, userId, User, ct);
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
