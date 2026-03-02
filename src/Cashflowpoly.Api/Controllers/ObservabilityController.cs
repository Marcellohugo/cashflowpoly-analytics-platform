// Fungsi file: Menyediakan endpoint observabilitas untuk menampilkan metrik operasional API kepada instruktur.
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
/// Controller observabilitas yang menyediakan snapshot metrik operasional API untuk dashboard instruktur.
/// </summary>
public sealed class ObservabilityController : ControllerBase
{
    private readonly OperationalMetricsTracker _metricsTracker;

    /// <summary>
    /// Menginisialisasi controller dengan tracker metrik operasional.
    /// </summary>
    public ObservabilityController(OperationalMetricsTracker metricsTracker)
    {
        _metricsTracker = metricsTracker;
    }

    [HttpGet("metrics")]
    [ProducesResponseType(typeof(OperationalMetricsSnapshot), StatusCodes.Status200OK)]
    /// <summary>
    /// Mengambil snapshot metrik operasional dengan jumlah endpoint teratas yang dapat dikonfigurasi.
    /// </summary>
    /// <param name="top">Jumlah endpoint teratas yang ditampilkan (default 20, maks 200).</param>
    /// <returns>200 OK dengan snapshot metrik operasional.</returns>
    public IActionResult GetOperationalMetrics([FromQuery] int top = 20)
    {
        var maxEndpoints = Math.Clamp(top, 1, 200);
        var snapshot = _metricsTracker.Snapshot(maxEndpoints);
        return Ok(snapshot);
    }
}
