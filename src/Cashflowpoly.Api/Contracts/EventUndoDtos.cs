// Fungsi file: Mendefinisikan permintaan undo, hasil idempoten, dan riwayat audit pembatalan event.
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cashflowpoly.Api.Contracts;

public sealed class UndoEventRequest
{
    [Required, MaxLength(120), JsonPropertyName("client_request_id")]
    public string ClientRequestId { get; init; } = string.Empty;

    [Range(1, long.MaxValue), JsonPropertyName("expected_state_version")]
    public long ExpectedStateVersion { get; init; }

    [Required, MaxLength(500), JsonPropertyName("reason")]
    public string Reason { get; init; } = string.Empty;
}

public sealed record EventUndoResponse(
    [property: JsonPropertyName("undo_id")] Guid UndoId,
    [property: JsonPropertyName("event_id")] Guid EventId,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("state_version")] long StateVersion,
    [property: JsonPropertyName("next_sequence_number")] long NextSequenceNumber);

public sealed record EventUndoAuditItem(
    [property: JsonPropertyName("undo_id")] Guid UndoId,
    [property: JsonPropertyName("event_id")] Guid EventId,
    [property: JsonPropertyName("instructor_user_id")] Guid InstructorUserId,
    [property: JsonPropertyName("client_request_id")] string ClientRequestId,
    [property: JsonPropertyName("reason")] string Reason,
    [property: JsonPropertyName("created_at")] DateTimeOffset CreatedAt,
    [property: JsonPropertyName("original_event")] JsonElement OriginalEvent,
    [property: JsonPropertyName("original_cashflows")] JsonElement OriginalCashflows,
    [property: JsonPropertyName("original_asset_references")] JsonElement OriginalAssetReferences);

public sealed record EventUndoAuditResponse(
    [property: JsonPropertyName("items")] List<EventUndoAuditItem> Items,
    [property: JsonPropertyName("next_cursor")] string? NextCursor,
    [property: JsonPropertyName("has_more")] bool HasMore);
