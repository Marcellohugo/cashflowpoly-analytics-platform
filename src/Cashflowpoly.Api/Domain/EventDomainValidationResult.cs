using Cashflowpoly.Contracts;
using Microsoft.AspNetCore.Http;

namespace Cashflowpoly.Api.Domain;

public sealed record EventDomainValidationResult(
    bool IsValid,
    int StatusCode,
    string? ErrorCode,
    string? Message,
    IReadOnlyList<ErrorDetail> Details)
{
    public static readonly EventDomainValidationResult Valid =
        new(true, StatusCodes.Status200OK, null, null, Array.Empty<ErrorDetail>());

    public static EventDomainValidationResult Fail(
        int statusCode,
        string errorCode,
        string message,
        params ErrorDetail[] details)
    {
        return new EventDomainValidationResult(false, statusCode, errorCode, message, details);
    }
}
