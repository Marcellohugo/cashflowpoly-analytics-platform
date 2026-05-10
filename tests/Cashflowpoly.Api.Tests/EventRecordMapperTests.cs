using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Domain;
using Xunit;

namespace Cashflowpoly.Api.Tests;

public sealed class EventRecordMapperTests
{
    [Fact]
    public void ToEventRequest_CopiesStoredFieldsAndClonesPayload()
    {
        var eventId = Guid.NewGuid();
        var sessionId = Guid.NewGuid();
        var playerId = Guid.NewGuid();
        var rulesetVersionId = Guid.NewGuid();
        var timestamp = new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero);
        var record = new EventDb
        {
            EventId = eventId,
            SessionId = sessionId,
            PlayerId = playerId,
            ActorType = "player",
            Timestamp = timestamp,
            DayIndex = 7,
            Weekday = "Friday",
            TurnNumber = 3,
            SequenceNumber = 12,
            ActionType = "transaction.recorded",
            RulesetVersionId = rulesetVersionId,
            Payload = """{"amount":5,"category":"PAYCHECK"}""",
            ClientRequestId = "client-123"
        };

        var request = new EventRecordMapper().ToEventRequest(record);

        Assert.Equal(eventId, request.EventId);
        Assert.Equal(sessionId, request.SessionId);
        Assert.Equal(playerId, request.PlayerId);
        Assert.Equal("player", request.ActorType);
        Assert.Equal(timestamp, request.Timestamp);
        Assert.Equal(7, request.DayIndex);
        Assert.Equal("Friday", request.Weekday);
        Assert.Equal(3, request.TurnNumber);
        Assert.Equal(12, request.SequenceNumber);
        Assert.Equal("transaction.recorded", request.ActionType);
        Assert.Equal(rulesetVersionId, request.RulesetVersionId);
        Assert.Equal("client-123", request.ClientRequestId);
        Assert.Equal(5, request.Payload.GetProperty("amount").GetInt32());
        Assert.Equal("PAYCHECK", request.Payload.GetProperty("category").GetString());
    }
}
