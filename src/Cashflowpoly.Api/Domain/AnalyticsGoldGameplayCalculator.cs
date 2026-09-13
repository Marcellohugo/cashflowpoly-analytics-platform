// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui AnalyticsGoldGameplayCalculator.
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Domain;

public sealed record AnalyticsGoldGameplayMetrics(
    // Parameter `InitialGoldQty` bertipe `int` membawa nilai awal emas qty.
    int InitialGoldQty,
    // Parameter `GoldBuyQty` bertipe `int` membawa nilai emas buy qty.
    int GoldBuyQty,
    // Parameter `GoldSellQty` bertipe `int` membawa nilai emas sell qty.
    int GoldSellQty,
    // Parameter `GoldHeldEnd` bertipe `int` membawa nilai emas held end.
    int GoldHeldEnd,
    // Parameter `GoldPurchasePrices` bertipe `IReadOnlyList<int>` membawa nilai emas pembelian prices.
    IReadOnlyList<int> GoldPurchasePrices,
    // Parameter `GoldSalePrices` bertipe `IReadOnlyList<int>` membawa nilai emas penjualan prices.
    IReadOnlyList<int> GoldSalePrices,
    // Parameter `GoldInvestmentSpent` bertipe `int` membawa nilai emas investment spent.
    int GoldInvestmentSpent,
    // Parameter `GoldInvestmentEarned` bertipe `int` membawa nilai emas investment earned.
    int GoldInvestmentEarned,
    // Parameter `GoldInvestmentNet` bertipe `int` membawa nilai emas investment net.
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

        // Mengulangi setiap elemen `playerEvents.Where(e => e.ActionType == GameActionCatalog.InvestasiEmas || e.ActionType == GameActionCatalog.JualEmas
        // || (e.ActionType == GameActionCatalog.RiskEmergencyUsed &...`; elemen saat ini disimpan sebagai `evt` bertipe `var` untuk diproses oleh badan
        // loop dalam Compute.
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
                    // Melewati sisa pernyataan pada iterasi saat ini dan melanjutkan ke elemen/iterasi berikutnya dalam Compute.
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
                // Melewati sisa pernyataan pada iterasi saat ini dan melanjutkan ke elemen/iterasi berikutnya dalam Compute.
                continue;
            }

            if (!_payloadReader.TryReadGoldTradeDetailed(evt.Payload, out var tradeType, out var qty, out var unitPrice, out var amount))
            {
                // Melewati sisa pernyataan pada iterasi saat ini dan melanjutkan ke elemen/iterasi berikutnya dalam Compute.
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
            initialGoldQty,
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
        // Menangani exception `System.Text.Json.JsonException` melalui variabel dalam TryReadInt32.
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
        // Menangani exception `System.Text.Json.JsonException` melalui variabel dalam TryReadString.
        catch (System.Text.Json.JsonException)
        {
            return false;
        }
    }
}
