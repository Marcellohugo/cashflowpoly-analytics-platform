using Cashflowpoly.Api.Data;

namespace Cashflowpoly.Api.Domain;

/// <summary>
/// Kalkulator inventaris bahan analitik dari event pembelian, klaim pesanan, dan discard.
/// </summary>
internal sealed class IngredientInventoryCalculator : IIngredientInventoryCalculator
{
    private static readonly AnalyticsPayloadReader _payloadReader = new();
    /// <summary>
    /// Membangun inventaris bahan berdasarkan event pembelian, klaim pesanan, dan discard.
    /// </summary>
    public AnalyticsIngredientInventory BuildIngredientInventory(List<EventDb> events)
    {
        var inventory = new AnalyticsIngredientInventory();

        foreach (var evt in events)
        {
            if (evt.ActionType == "ingredient.purchased" &&
                _payloadReader.TryReadIngredientPurchase(evt.Payload, out var cardId, out var amount))
            {
                inventory.Total += amount;
                inventory.ByCardId[cardId] = inventory.ByCardId.TryGetValue(cardId, out var qty) ? qty + amount : amount;
            }

            if (evt.ActionType == "order.claimed" &&
                _payloadReader.TryReadOrderClaim(evt.Payload, out var requiredCards, out _))
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
                _payloadReader.TryReadIngredientPurchase(evt.Payload, out var discardCardId, out var discardAmount) &&
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
