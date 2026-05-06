// Fungsi file: Menguji builder proyeksi arus kas dari event gameplay tanpa akses database.
using System.Text.Json;
using Cashflowpoly.Api.Domain;
using Cashflowpoly.Contracts;
using Xunit;

namespace Cashflowpoly.Api.Tests;

public sealed class EventCashflowProjectionBuilderTests
{
    [Fact]
    public void TryBuild_ReturnsFalse_WhenEventHasNoPlayer()
    {
        var request = BuildRequest(
            includePlayer: false,
            actionType: "day.friday.donation",
            payloadJson: """{"amount":5}""");

        var ok = EventCashflowProjectionBuilder.TryBuild(
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
            actionType: "day.saturday.gold_trade",
            payloadJson: """{"trade_type":"SELL","qty":2,"unit_price":6,"amount":12}""");

        var ok = EventCashflowProjectionBuilder.TryBuild(request, timestamp, eventPk, out var projection);

        Assert.True(ok);
        Assert.NotNull(projection);
        Assert.Equal(sessionId, projection.SessionId);
        Assert.Equal(playerId, projection.PlayerId);
        Assert.Equal(eventPk, projection.EventPk);
        Assert.Equal(eventId, projection.EventId);
        Assert.Equal(timestamp, projection.Timestamp);
        Assert.Equal("IN", projection.Direction);
        Assert.Equal(12, projection.Amount);
        Assert.Equal("GOLD_TRADE", projection.Category);
    }

    [Fact]
    public void TryBuild_TransactionRoundsAmountAndUppercasesDirection()
    {
        var request = BuildRequest(
            actionType: "transaction.recorded",
            payloadJson: """
                {
                  "direction": "out",
                  "amount": 7.6,
                  "category": "CUSTOM",
                  "counterparty": "BANK"
                }
                """);

        var ok = EventCashflowProjectionBuilder.TryBuild(
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
