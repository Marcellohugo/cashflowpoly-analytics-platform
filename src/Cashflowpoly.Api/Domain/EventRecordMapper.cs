// Fungsi file: Memetakan baris event database menjadi DTO event API yang siap dikembalikan ke klien.
using System.Text.Json;
using Cashflowpoly.Api.Data;
using Cashflowpoly.Contracts;

namespace Cashflowpoly.Api.Domain;

internal static class EventRecordMapper
{
    internal static EventRequest ToEventRequest(EventDb record)
    {
        using var document = JsonDocument.Parse(record.Payload);
        var payload = document.RootElement.Clone();

        return new EventRequest(
            record.EventId,
            record.SessionId,
            record.PlayerId,
            record.ActorType,
            record.Timestamp,
            record.DayIndex,
            record.Weekday,
            record.TurnNumber,
            record.SequenceNumber,
            record.ActionType,
            record.RulesetVersionId,
            payload,
            record.ClientRequestId);
    }
}
