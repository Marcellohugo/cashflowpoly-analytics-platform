// Fungsi file: Mengelola endpoint API untuk domain ObservabilityController termasuk validasi request dan respons standar.
using Cashflowpoly.Api.Infrastructure;
using Cashflowpoly.Api.Models;
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
/// Menyatakan peran utama tipe ObservabilityController pada modul ini.
/// </summary>
public sealed class ObservabilityController : ControllerBase
{
    private readonly OperationalMetricsTracker _metricsTracker;

    /// <summary>
    /// Menjalankan fungsi ObservabilityController sebagai bagian dari alur file ini.
    /// </summary>
    public ObservabilityController(OperationalMetricsTracker metricsTracker)
    {
        _metricsTracker = metricsTracker;
    }

    [HttpGet("metrics")]
    [ProducesResponseType(typeof(OperationalMetricsSnapshot), StatusCodes.Status200OK)]
    /// <summary>
    /// Menjalankan fungsi GetOperationalMetrics sebagai bagian dari alur file ini.
    /// </summary>
    public IActionResult GetOperationalMetrics([FromQuery] int top = 20)
    {
        var maxEndpoints = Math.Clamp(top, 1, 200);
        var snapshot = _metricsTracker.Snapshot(maxEndpoints);
        return Ok(snapshot);
    }
}
