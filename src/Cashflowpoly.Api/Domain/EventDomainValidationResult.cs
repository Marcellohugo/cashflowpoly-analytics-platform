// Fungsi file: Merepresentasikan hasil validasi domain event sebelum dikonversi menjadi ErrorResponse API.
using Cashflowpoly.Contracts;
using Microsoft.AspNetCore.Http;

namespace Cashflowpoly.Api.Domain;

internal sealed record EventDomainValidationResult(
    bool IsValid,
    int StatusCode,
    string? ErrorCode,
    string? Message,
    IReadOnlyList<ErrorDetail> Details)
{
    internal static readonly EventDomainValidationResult Valid =
        new(true, StatusCodes.Status200OK, null, null, Array.Empty<ErrorDetail>());

    internal static EventDomainValidationResult Fail(
        int statusCode,
        string errorCode,
        string message,
        params ErrorDetail[] details)
    {
        return new EventDomainValidationResult(false, statusCode, errorCode, message, details);
    }
}
