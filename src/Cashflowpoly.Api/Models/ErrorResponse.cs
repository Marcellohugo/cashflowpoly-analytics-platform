// Fungsi file: Mendefinisikan kontrak data API pada domain ErrorResponse.
using System.Text.Json.Serialization;

namespace Cashflowpoly.Api.Models;

/// <summary>
/// Menyatakan peran utama tipe ErrorDetail pada modul ini.
/// </summary>
public sealed record ErrorDetail(
    [property: JsonPropertyName("field")] string Field,
    [property: JsonPropertyName("issue")] string Issue);

/// <summary>
/// Menyatakan peran utama tipe ErrorResponse pada modul ini.
/// </summary>
public sealed record ErrorResponse(
    [property: JsonPropertyName("error_code")] string ErrorCode,
    [property: JsonPropertyName("message")] string Message,
    [property: JsonPropertyName("details")] List<ErrorDetail> Details,
    [property: JsonPropertyName("trace_id")] string TraceId);
