// Fungsi file: Menyediakan endpoint observabilitas API; metrik tersedia dalam format Prometheus di /metrics.
using Cashflowpoly.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cashflowpoly.Api.Controllers;

[ApiController]
[Route("api/v1/observability")]
[Authorize(Roles = "INSTRUCTOR")]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status429TooManyRequests)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
/// <summary>
/// Controller observabilitas yang mengarahkan ke endpoint Prometheus untuk metrik API.
/// </summary>
public sealed class ObservabilityController : ControllerBase
{
    [HttpGet("metrics/summary")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    /// <summary>
    /// Memberikan informasi lokasi endpoint metrik Prometheus.
    /// </summary>
    /// <returns>200 OK dengan pesan lokasi endpoint metrik.</returns>
    public IActionResult GetMetricsSummary()
    {
        return Ok(new { message = "Metrics available at /metrics (Prometheus format)" });
    }
}
