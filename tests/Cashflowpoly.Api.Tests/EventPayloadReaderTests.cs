// Fungsi file: Menguji parser payload event gameplay yang dipakai validasi domain dan proyeksi cashflow.
using System.Text.Json;
using Cashflowpoly.Api.Domain;
using Xunit;

namespace Cashflowpoly.Api.Tests;

public sealed class EventPayloadReaderTests
{
    [Fact]
    public void TryReadNeedPurchase_RequiresNonBlankCardId()
    {
        var payload = Parse("""{"card_id":" ","amount":4,"points":2}""");

        var ok = EventPayloadReader.TryReadNeedPurchase(payload, out var cardId, out var amount, out var points);

        Assert.False(ok);
        Assert.Equal(string.Empty, cardId);
        Assert.Equal(4, amount);
        Assert.Equal(2, points);
    }

    [Fact]
    public void TryReadEmergencyOption_ReadsRiskReferenceAndCashflow()
    {
        var payload = Parse("""
            {
              "risk_event_id": "risk-123",
              "option_type": "SELL_GOLD",
              "direction": "IN",
              "amount": 6
            }
            """);

        var ok = EventPayloadReader.TryReadEmergencyOption(
            payload,
            out var riskEventId,
            out var optionType,
            out var direction,
            out var amount);

        Assert.True(ok);
        Assert.Equal("risk-123", riskEventId);
        Assert.Equal("SELL_GOLD", optionType);
        Assert.Equal("IN", direction);
        Assert.Equal(6, amount);
    }

    [Fact]
    public void TryReadOrderClaim_RejectsNonStringCardIds()
    {
        var payload = Parse("""{"required_ingredient_card_ids":["card-1",7],"income":10}""");

        var ok = EventPayloadReader.TryReadOrderClaim(payload, out var requiredCards, out var income);

        Assert.False(ok);
        Assert.Empty(requiredCards);
        Assert.Equal(10, income);
    }

    private static JsonElement Parse(string json)
    {
        using var document = JsonDocument.Parse(json);
        return document.RootElement.Clone();
    }
}
