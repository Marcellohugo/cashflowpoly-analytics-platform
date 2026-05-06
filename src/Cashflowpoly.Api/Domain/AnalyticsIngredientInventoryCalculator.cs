// Fungsi file: Menghitung inventaris bahan pemain dari event analitik gameplay.
using Cashflowpoly.Api.Data;
using static Cashflowpoly.Api.Domain.AnalyticsPayloadReader;

namespace Cashflowpoly.Api.Domain;

/// <summary>
/// Kalkulator inventaris bahan analitik dari event pembelian, klaim pesanan, dan discard.
/// </summary>
internal static class AnalyticsIngredientInventoryCalculator
{
    /// <summary>
    /// Membangun inventaris bahan berdasarkan event pembelian, klaim pesanan, dan discard.
    /// </summary>
    internal static AnalyticsIngredientInventory BuildIngredientInventory(List<EventDb> events)
    {
        var inventory = new AnalyticsIngredientInventory();

        foreach (var evt in events)
        {
            if (evt.ActionType == "ingredient.purchased" &&
                TryReadIngredientPurchase(evt.Payload, out var cardId, out var amount))
            {
                inventory.Total += amount;
                inventory.ByCardId[cardId] = inventory.ByCardId.TryGetValue(cardId, out var qty) ? qty + amount : amount;
            }

            if (evt.ActionType == "order.claimed" &&
                TryReadOrderClaim(evt.Payload, out var requiredCards, out _))
            {
                foreach (var card in requiredCards)
                {
                    if (inventory.ByCardId.TryGetValue(card, out var qty) && qty > 0)
                    {
                        inventory.ByCardId[card] = qty - 1;
                        inventory.Total = Math.Max(0, inventory.Total - 1);
                    }
                }
            }

            if (evt.ActionType == "ingredient.discarded" &&
                TryReadIngredientPurchase(evt.Payload, out var discardCardId, out var discardAmount) &&
                inventory.ByCardId.TryGetValue(discardCardId, out var discardQty))
            {
                var newQty = Math.Max(0, discardQty - discardAmount);
                inventory.ByCardId[discardCardId] = newQty;
                inventory.Total = Math.Max(0, inventory.Total - discardAmount);
            }
        }

        return inventory;
    }
}
