// Fungsi file: Memverifikasi perilaku API, database, atau domain melalui AnalyticsGoldGameplayCalculatorTests.
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
            CreateEvent(playerId, "SetupEmasAwal", """{"asset_code":"gold_card","qty":1,"unit_value":5,"setup":"INITIAL"}"""),
            CreateGoldTrade(playerId, "BUY", qty: 3, unitPrice: 4, amount: 12),
            CreateGoldTrade(playerId, "SELL", qty: 1, unitPrice: 5, amount: 5),
            CreateGoldTrade(playerId, "BUY", qty: 2, unitPrice: 6, amount: 12),
            CreateEvent(playerId, "GunakanOpsiDarurat", """{"option_type":"SELL_GOLD","qty":2,"unit_price":7,"amount":14}"""),
            CreateEvent(playerId, "CatatTransaksi", """{"amount":99}""")
        };

        var metrics = new GoldGameplayCalculator().Compute(events);

        Assert.Equal(1, metrics.InitialGoldQty);
        Assert.Equal(5, metrics.GoldBuyQty);
        Assert.Equal(3, metrics.GoldSellQty);
        Assert.Equal(3, metrics.GoldHeldEnd);
        Assert.Equal(new[] { 4, 6 }, metrics.GoldPurchasePrices);
        Assert.Equal(new[] { 5, 7 }, metrics.GoldSalePrices);
        Assert.Equal(24, metrics.GoldInvestmentSpent);
        Assert.Equal(19, metrics.GoldInvestmentEarned);
        Assert.Equal(-5, metrics.GoldInvestmentNet);
    }

    private static EventDb CreateGoldTrade(Guid playerId, string tradeType, int qty, int unitPrice, int amount)
    {
        return CreateEvent(
            playerId,
            string.Equals(tradeType, "SELL", StringComparison.OrdinalIgnoreCase) ? "JualEmas" : "InvestasiEmas",
            $$"""{"trade_type":"{{tradeType}}","qty":{{qty}},"unit_price":{{unitPrice}},"amount":{{amount}}}""");
    }

    private static EventDb CreateEvent(Guid playerId, string actionType, string payload)
    {
        return new EventDb
        {
            EventId = Guid.NewGuid(),
            SessionId = Guid.NewGuid(),
            UserId = playerId,
            ActorType = "PLAYER",
            Timestamp = new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero),
            DayIndex = 0,
            Weekday = "SAT",
            ActionSlot = 1,
            SequenceNumber = 1,
            ActionType = actionType,
            RulesetVersionId = Guid.NewGuid(),
            Payload = payload
        };
    }
}
