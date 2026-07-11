using System.Text.Json;
using Cashflowpoly.Api.Domain;
using Cashflowpoly.Api.Contracts;
using Xunit;

namespace Cashflowpoly.Api.Tests;

public sealed class EventCashflowProjectionBuilderTests
{
    [Fact]
    public void TryBuild_ReturnsFalse_WhenEventHasNoPlayer()
    {
        var request = BuildRequest(
            includePlayer: false,
            actionType: "JumatBerkah",
            payloadJson: """{"amount":5}""");

        var ok = new EventCashflowProjectionBuilder().TryBuild(
            request,
            DateTimeOffset.Parse("2026-01-02T03:04:05Z"),
            Guid.NewGuid(),
            out var projection);

        Assert.False(ok);
        Assert.Null(projection);
    }

    [Fact]
    public void TryBuild_GoldSellCreatesIncomingGoldTradeProjection()
    {
        var playerId = Guid.NewGuid();
        var sessionId = Guid.NewGuid();
        var eventId = Guid.NewGuid();
        var eventPk = Guid.NewGuid();
        var timestamp = DateTimeOffset.Parse("2026-01-02T03:04:05Z");
        var request = BuildRequest(
            sessionId: sessionId,
            playerId: playerId,
            eventId: eventId,
            actionType: "JualEmas",
            payloadJson: """{"trade_type":"SELL","qty":2,"unit_price":6,"amount":12}""");

        var ok = new EventCashflowProjectionBuilder().TryBuild(request, timestamp, eventPk, out var projection);

        Assert.True(ok);
        Assert.NotNull(projection);
        Assert.Equal(sessionId, projection.SessionId);
        Assert.Equal(playerId, projection.UserId);
        Assert.Equal(eventPk, projection.EventPk);
        Assert.Equal(eventId, projection.EventId);
        Assert.Equal(timestamp, projection.Timestamp);
        Assert.Equal("IN", projection.Direction);
        Assert.Equal(12, projection.Amount);
        Assert.Equal("GOLD_TRADE", projection.Category);
    }

    [Theory]
    [InlineData("InvestasiEmas", "OUT")]
    [InlineData("JualEmas", "IN")]
    public void TryBuild_GoldGameActionsUseFixedCashflowDirection(string actionType, string expectedDirection)
    {
        var request = BuildRequest(
            actionType: actionType,
            payloadJson: """{"qty":2,"unit_price":6,"amount":12}""");

        var ok = new EventCashflowProjectionBuilder().TryBuild(
            request,
            DateTimeOffset.Parse("2026-01-02T03:04:05Z"),
            Guid.NewGuid(),
            out var projection);

        Assert.True(ok);
        Assert.NotNull(projection);
        Assert.Equal(expectedDirection, projection.Direction);
        Assert.Equal(12, projection.Amount);
        Assert.Equal("GOLD_TRADE", projection.Category);
    }

    [Theory]
    [InlineData("BahanMasakan", "OUT", 4, "INGREDIENT", """{"card_id":"telur","amount":4}""")]
    [InlineData("JualMasakan", "IN", 15, "ORDER", """{"order_card_id":"nasi_goreng","required_ingredient_card_ids":["nasi_putih","telur"],"income":15}""")]
    [InlineData("JumatBerkah", "OUT", 5, "DONATION", """{"amount":5}""")]
    public void TryBuild_GameActionIdsCreateCashflowProjection(
        string actionType,
        string expectedDirection,
        int expectedAmount,
        string expectedCategory,
        string payloadJson)
    {
        var request = BuildRequest(actionType: actionType, payloadJson: payloadJson);

        var ok = new EventCashflowProjectionBuilder().TryBuild(
            request,
            DateTimeOffset.Parse("2026-01-02T03:04:05Z"),
            Guid.NewGuid(),
            out var projection);

        Assert.True(ok);
        Assert.NotNull(projection);
        Assert.Equal(expectedDirection, projection.Direction);
        Assert.Equal(expectedAmount, projection.Amount);
        Assert.Equal(expectedCategory, projection.Category);
    }

    [Fact]
    public void TryBuild_TransactionRoundsAmountAndUppercasesDirection()
    {
        var request = BuildRequest(
            actionType: "CatatTransaksi",
            payloadJson: """
                {
                  "direction": "out",
                  "amount": 7.6,
                  "category": "CUSTOM",
                  "counterparty": "BANK"
                }
                """);

        var ok = new EventCashflowProjectionBuilder().TryBuild(
            request,
            DateTimeOffset.Parse("2026-01-02T03:04:05Z"),
            Guid.NewGuid(),
            out var projection);

        Assert.True(ok);
        Assert.NotNull(projection);
        Assert.Equal("OUT", projection.Direction);
        Assert.Equal(8, projection.Amount);
        Assert.Equal("CUSTOM", projection.Category);
        Assert.Equal("BANK", projection.Counterparty);
    }

    [Fact]
    public void TryBuild_EmergencyLoanUsesCatalogPrincipalInsteadOfClientCashflowFields()
    {
        var request = BuildRequest(
            actionType: "GunakanOpsiDarurat",
            payloadJson: """
                {
                  "risk_event_id":"00000000-0000-0000-0000-000000000001",
                  "option_type":"TAKE_SHARIA_LOAN",
                  "principal":10,
                  "direction":"OUT",
                  "amount":100
                }
                """);

        var ok = new EventCashflowProjectionBuilder().TryBuild(
            request,
            DateTimeOffset.Parse("2026-01-02T03:04:05Z"),
            Guid.NewGuid(),
            out var projection);

        Assert.True(ok);
        Assert.NotNull(projection);
        Assert.Equal("IN", projection.Direction);
        Assert.Equal(10, projection.Amount);
        Assert.Equal("EMERGENCY_OPTION", projection.Category);
    }

    private static EventRequest BuildRequest(
        string actionType,
        string payloadJson,
        Guid? sessionId = null,
        Guid? eventId = null,
        Guid? playerId = null,
        bool includePlayer = true)
    {
        return new EventRequest(
            eventId ?? Guid.NewGuid(),
            sessionId ?? Guid.NewGuid(),
            includePlayer ? playerId ?? Guid.NewGuid() : null,
            "PLAYER",
            DateTimeOffset.Parse("2026-01-02T03:04:05Z"),
            0,
            "MON",
            1,
            1,
            actionType,
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
