using System.Text.Json;
using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Contracts;

namespace Cashflowpoly.Api.Domain;

internal sealed class EventRecordMapper : IEventRecordMapper
{
    public EventRequest ToEventRequest(EventDb record)
    {
        using var document = JsonDocument.Parse(record.Payload);
        var payload = document.RootElement.Clone();

        return new EventRequest(
            record.EventId,
            record.SessionId,
            record.UserId,
            record.ActorType,
            record.Timestamp,
            record.DayIndex,
            record.Weekday,
            record.ActionSlot,
            record.SequenceNumber,
            record.ActionType,
            record.RulesetVersionId,
            payload,
            record.ClientRequestId,
            record.TurnNumber);
    }
}
