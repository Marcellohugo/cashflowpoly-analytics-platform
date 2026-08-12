// Fungsi file: Mendefinisikan kontrak pertukaran data UI dengan API untuk ErrorResponse.
using System.Text.Json.Serialization;

namespace Cashflowpoly.Ui.Contracts;

public sealed record ErrorDetail(
    [property: JsonPropertyName("field")] string Field,
    [property: JsonPropertyName("issue")] string Issue);

public sealed record ErrorResponse(
    [property: JsonPropertyName("error_code")] string ErrorCode,
    [property: JsonPropertyName("message")] string Message,
    [property: JsonPropertyName("details")] List<ErrorDetail> Details,
    [property: JsonPropertyName("trace_id")] string TraceId);
