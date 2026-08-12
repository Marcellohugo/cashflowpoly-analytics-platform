// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui AnalyticsGoldGameplayCalculator.
using Cashflowpoly.Api.Data;

namespace Cashflowpoly.Api.Domain;

public sealed record AnalyticsGoldGameplayMetrics(
    int GoldBuyQty,
    int GoldSellQty,
    int GoldHeldEnd,
    IReadOnlyList<int> GoldPurchasePrices,
    IReadOnlyList<int> GoldSalePrices,
    int GoldInvestmentSpent,
    int GoldInvestmentEarned,
    int GoldInvestmentNet);

internal sealed class GoldGameplayCalculator : IGoldGameplayCalculator
{
    private static readonly AnalyticsPayloadReader _payloadReader = new();

    public AnalyticsGoldGameplayMetrics Compute(IEnumerable<EventDb> playerEvents)
    {
        var goldBuyQty = 0;
        var goldSellQty = 0;
        var goldPurchasePrices = new List<int>();
        var goldSalePrices = new List<int>();
        var goldInvestmentSpent = 0;
        var goldInvestmentEarned = 0;

        var initialGoldQty = playerEvents
            .Where(e => e.ActionType == GameActionCatalog.SetupEmasAwal)
            .Sum(e => TryReadInt32(e.Payload, "qty", out var qty) ? qty : 1);

        foreach (var evt in playerEvents.Where(e =>
                     e.ActionType == GameActionCatalog.InvestasiEmas ||
                     e.ActionType == GameActionCatalog.JualEmas ||
                     (e.ActionType == GameActionCatalog.RiskEmergencyUsed &&
                      TryReadString(e.Payload, "option_type", out var optionType) &&
                      optionType.Equals("SELL_GOLD", StringComparison.OrdinalIgnoreCase))))
        {
            if (evt.ActionType == GameActionCatalog.RiskEmergencyUsed)
            {
                if (!TryReadInt32(evt.Payload, "qty", out var emergencyQty))
                {
                    continue;
                }

                TryReadInt32(evt.Payload, "unit_price", out var emergencyUnitPrice);
                TryReadInt32(evt.Payload, "amount", out var emergencyAmount);
                goldSellQty += emergencyQty;
                goldInvestmentEarned += emergencyAmount;
                if (emergencyUnitPrice > 0)
                {
                    goldSalePrices.Add(emergencyUnitPrice);
                }
                continue;
            }

            if (!_payloadReader.TryReadGoldTradeDetailed(evt.Payload, out var tradeType, out var qty, out var unitPrice, out var amount))
            {
                continue;
            }

            if (evt.ActionType == GameActionCatalog.InvestasiEmas ||
                string.Equals(tradeType, "BUY", StringComparison.OrdinalIgnoreCase))
            {
                goldBuyQty += qty;
                goldInvestmentSpent += amount;
                if (unitPrice > 0)
                {
                    goldPurchasePrices.Add(unitPrice);
                }
            }
            else if (evt.ActionType == GameActionCatalog.JualEmas ||
                     string.Equals(tradeType, "SELL", StringComparison.OrdinalIgnoreCase))
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
            initialGoldQty + goldBuyQty - goldSellQty,
            goldPurchasePrices,
            goldSalePrices,
            goldInvestmentSpent,
            goldInvestmentEarned,
            goldInvestmentEarned - goldInvestmentSpent);
    }

    private static bool TryReadInt32(string payload, string propertyName, out int value)
    {
        value = 0;
        try
        {
            using var document = System.Text.Json.JsonDocument.Parse(payload);
            return document.RootElement.TryGetProperty(propertyName, out var property) && property.TryGetInt32(out value);
        }
        catch (System.Text.Json.JsonException)
        {
            return false;
        }
    }

    private static bool TryReadString(string payload, string propertyName, out string value)
    {
        value = string.Empty;
        try
        {
            using var document = System.Text.Json.JsonDocument.Parse(payload);
            if (!document.RootElement.TryGetProperty(propertyName, out var property))
            {
                return false;
            }

            value = property.GetString() ?? string.Empty;
            return value.Length > 0;
        }
        catch (System.Text.Json.JsonException)
        {
            return false;
        }
    }
}
