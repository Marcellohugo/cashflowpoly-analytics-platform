using System.Security.Claims;
using Cashflowpoly.Contracts;

namespace Cashflowpoly.Api.Services;

public interface IEventIngestionService
{
    Task<(EventStoredResponse? Result, int StatusCode, ErrorResponse? Error)> IngestEventAsync(
        EventRequest request, ClaimsPrincipal user, CancellationToken ct);

    Task<(EventBatchResponse? Result, int StatusCode, ErrorResponse? Error)> IngestBatchAsync(
        EventBatchRequest request, ClaimsPrincipal user, CancellationToken ct);

    Task<(EventsBySessionResponse? Result, int StatusCode, ErrorResponse? Error)> GetEventsBySessionAsync(
        Guid sessionId, ClaimsPrincipal user, long fromSeq, int limit, CancellationToken ct);
}
