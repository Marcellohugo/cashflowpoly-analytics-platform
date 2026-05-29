using Cashflowpoly.Api.Services;
using Cashflowpoly.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cashflowpoly.Api.Controllers;

[ApiController]
[Route("api/v1")]
[Authorize]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status429TooManyRequests)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
public sealed class EventsController : ControllerBase
{
    private readonly IEventIngestionService _ingestion;

    public EventsController(IEventIngestionService ingestion) => _ingestion = ingestion;

    [HttpPost("events")]
    [ProducesResponseType(typeof(EventStoredResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateEvent([FromBody] EventRequest request, CancellationToken ct)
    {
        var (result, status, error) = await _ingestion.IngestEventAsync(request, User, ct);
        return status == StatusCodes.Status201Created ? StatusCode(StatusCodes.Status201Created, result) : StatusCode(status, error);
    }

    [HttpPost("events/batch")]
    [ProducesResponseType(typeof(EventBatchResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateEventsBatch([FromBody] EventBatchRequest request, CancellationToken ct)
    {
        var (result, status, error) = await _ingestion.IngestBatchAsync(request, User, ct);
        return status == 200 ? Ok(result) : StatusCode(status, error);
    }

    [HttpGet("sessions/{sessionId:guid}/events")]
    [ProducesResponseType(typeof(EventsBySessionResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEventsBySession(Guid sessionId, [FromQuery] long fromSeq = 0, [FromQuery] int limit = 200, CancellationToken ct = default)
    {
        var (result, status, error) = await _ingestion.GetEventsBySessionAsync(sessionId, User, fromSeq, limit, ct);
        return status == 200 ? Ok(result) : StatusCode(status, error);
    }
}
