// Fungsi file: Menguji builder offset cashflow untuk klaim asuransi risiko.
using System.Text.Json;
using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Domain;
using Cashflowpoly.Contracts;
using Xunit;

namespace Cashflowpoly.Api.Tests;

public sealed class EventInsuranceOffsetBuilderTests
{
    [Fact]
    public void TryBuild_CreatesInsuranceClaimProjectionForOutgoingRisk()
    {
        var sessionId = Guid.NewGuid();
        var playerId = Guid.NewGuid();
        var eventId = Guid.NewGuid();
        var eventPk = Guid.NewGuid();
        var riskEventId = Guid.NewGuid();
        var timestamp = DateTimeOffset.Parse("2026-01-02T03:04:05Z");
        var request = BuildRequest(
            sessionId,
            playerId,
            eventId,
            $$"""{"risk_event_id":"{{riskEventId}}"}""");
        var riskEvent = new EventDb
        {
            EventId = riskEventId,
            SessionId = sessionId,
            PlayerId = playerId,
            ActionType = "risk.life.drawn",
            Payload = """{"risk_id":"risk-a","direction":"OUT","amount":7}"""
        };

        var ok = EventInsuranceOffsetBuilder.TryBuild(request, timestamp, eventPk, riskEvent, out var projection);

        Assert.True(ok);
        Assert.NotNull(projection);
        Assert.Equal(sessionId, projection.SessionId);
        Assert.Equal(playerId, projection.PlayerId);
        Assert.Equal(eventPk, projection.EventPk);
        Assert.Equal(eventId, projection.EventId);
        Assert.Equal(timestamp, projection.Timestamp);
        Assert.Equal("IN", projection.Direction);
        Assert.Equal(7, projection.Amount);
        Assert.Equal("INSURANCE_CLAIM", projection.Category);
        Assert.Equal("BANK", projection.Counterparty);
        Assert.Equal(riskEventId.ToString(), projection.Reference);
        Assert.Equal("Offset risiko via asuransi", projection.Note);
    }

    private static EventRequest BuildRequest(Guid sessionId, Guid playerId, Guid eventId, string payloadJson)
    {
        return new EventRequest(
            eventId,
            sessionId,
            playerId,
            "PLAYER",
            DateTimeOffset.Parse("2026-01-02T03:04:05Z"),
            0,
            "MON",
            1,
            1,
            "insurance.multirisk.used",
            Guid.NewGuid(),
            Parse(payloadJson),
            null);
    }

    private static JsonElement Parse(string json)
    {
        using var document = JsonDocument.Parse(json);
        return document.RootElement.Clone();
    }
}
