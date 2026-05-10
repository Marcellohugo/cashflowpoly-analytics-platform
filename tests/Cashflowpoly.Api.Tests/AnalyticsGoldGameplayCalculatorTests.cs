using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Domain;
using Xunit;

namespace Cashflowpoly.Api.Tests;

public sealed class AnalyticsGoldGameplayCalculatorTests
{
    [Fact]
    public void Compute_SummarizesBuySellQtyPricesAndNetInvestment()
    {
        var playerId = Guid.NewGuid();
        var events = new List<EventDb>
        {
            CreateGoldTrade(playerId, "BUY", qty: 3, unitPrice: 4, amount: 12),
            CreateGoldTrade(playerId, "SELL", qty: 1, unitPrice: 5, amount: 5),
            CreateGoldTrade(playerId, "BUY", qty: 2, unitPrice: 6, amount: 12),
            CreateEvent(playerId, "transaction.recorded", """{"amount":99}""")
        };

        var metrics = new GoldGameplayCalculator().Compute(events);

        Assert.Equal(5, metrics.GoldBuyQty);
        Assert.Equal(1, metrics.GoldSellQty);
        Assert.Equal(4, metrics.GoldHeldEnd);
        Assert.Equal(new[] { 4, 6 }, metrics.GoldPurchasePrices);
        Assert.Equal(new[] { 5 }, metrics.GoldSalePrices);
        Assert.Equal(24, metrics.GoldInvestmentSpent);
        Assert.Equal(5, metrics.GoldInvestmentEarned);
        Assert.Equal(-19, metrics.GoldInvestmentNet);
    }

    private static EventDb CreateGoldTrade(Guid playerId, string tradeType, int qty, int unitPrice, int amount)
    {
        return CreateEvent(
            playerId,
            "day.saturday.gold_trade",
            $$"""{"trade_type":"{{tradeType}}","qty":{{qty}},"unit_price":{{unitPrice}},"amount":{{amount}}}""");
    }

    private static EventDb CreateEvent(Guid playerId, string actionType, string payload)
    {
        return new EventDb
        {
            EventId = Guid.NewGuid(),
            SessionId = Guid.NewGuid(),
            PlayerId = playerId,
            ActorType = "PLAYER",
            Timestamp = new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero),
            DayIndex = 0,
            Weekday = "SAT",
            TurnNumber = 1,
            SequenceNumber = 1,
            ActionType = actionType,
            RulesetVersionId = Guid.NewGuid(),
            Payload = payload
        };
    }
}
