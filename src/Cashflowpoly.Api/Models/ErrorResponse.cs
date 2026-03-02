// Fungsi file: Mendefinisikan model respons error standar API, termasuk detail per-field dan trace ID untuk penelusuran masalah.
using System.Text.Json.Serialization;

namespace Cashflowpoly.Api.Models;

/// <summary>
/// Detail kesalahan validasi pada satu field tertentu, berisi nama field dan jenis masalahnya.
/// </summary>
public sealed record ErrorDetail(
    [property: JsonPropertyName("field")] string Field,
    [property: JsonPropertyName("issue")] string Issue);

/// <summary>
/// Respons error standar API yang mencakup kode error, pesan, daftar detail, dan trace ID.
/// </summary>
public sealed record ErrorResponse(
    [property: JsonPropertyName("error_code")] string ErrorCode,
    [property: JsonPropertyName("message")] string Message,
    [property: JsonPropertyName("details")] List<ErrorDetail> Details,
    [property: JsonPropertyName("trace_id")] string TraceId);
