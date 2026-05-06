// Fungsi file: Menghitung metrik emas gameplay dari histori event pemain.
using Cashflowpoly.Api.Data;
using static Cashflowpoly.Api.Domain.AnalyticsPayloadReader;

namespace Cashflowpoly.Api.Domain;

internal sealed record AnalyticsGoldGameplayMetrics(
    int GoldBuyQty,
    int GoldSellQty,
    int GoldHeldEnd,
    IReadOnlyList<int> GoldPurchasePrices,
    IReadOnlyList<int> GoldSalePrices,
    int GoldInvestmentSpent,
    int GoldInvestmentEarned,
    int GoldInvestmentNet);

internal static class AnalyticsGoldGameplayCalculator
{
    internal static AnalyticsGoldGameplayMetrics Compute(IEnumerable<EventDb> playerEvents)
    {
        var goldBuyQty = 0;
        var goldSellQty = 0;
        var goldPurchasePrices = new List<int>();
        var goldSalePrices = new List<int>();
        var goldInvestmentSpent = 0;
        var goldInvestmentEarned = 0;

        foreach (var evt in playerEvents.Where(e => e.ActionType == "day.saturday.gold_trade"))
        {
            if (!TryReadGoldTradeDetailed(evt.Payload, out var tradeType, out var qty, out var unitPrice, out var amount))
            {
                continue;
            }

            if (string.Equals(tradeType, "BUY", StringComparison.OrdinalIgnoreCase))
            {
                goldBuyQty += qty;
                goldInvestmentSpent += amount;
                if (unitPrice > 0)
                {
                    goldPurchasePrices.Add(unitPrice);
                }
            }
            else if (string.Equals(tradeType, "SELL", StringComparison.OrdinalIgnoreCase))
            {
                goldSellQty += qty;
                goldInvestmentEarned += amount;
                if (unitPrice > 0)
                {
                    goldSalePrices.Add(unitPrice);
                }
            }
        }

        return new AnalyticsGoldGameplayMetrics(
            goldBuyQty,
            goldSellQty,
            goldBuyQty - goldSellQty,
            goldPurchasePrices,
            goldSalePrices,
            goldInvestmentSpent,
            goldInvestmentEarned,
            goldInvestmentEarned - goldInvestmentSpent);
    }
}
