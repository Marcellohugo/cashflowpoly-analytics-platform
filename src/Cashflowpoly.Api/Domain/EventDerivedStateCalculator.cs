// Fungsi file: Menghitung state turunan pemain dari event gameplay historis.
using Cashflowpoly.Api.Data;

namespace Cashflowpoly.Api.Domain;

/// <summary>
/// Kalkulator state turunan event yang dipakai validasi domain lanjutan.
/// </summary>
internal static class EventDerivedStateCalculator
{
    /// <summary>
    /// Membangun inventaris bahan pemain dari event ingredient purchased, order claimed, dan ingredient discarded.
    /// </summary>
    internal static EventIngredientInventory BuildIngredientInventory(IEnumerable<EventDb> events, Guid playerId)
    {
        var inventory = new EventIngredientInventory();

        foreach (var evt in events.Where(e => e.PlayerId == playerId))
        {
            if (evt.ActionType == "ingredient.purchased" &&
                EventPayloadReader.TryReadIngredientPurchase(EventPayloadReader.ReadPayload(evt.Payload), out var cardId, out var amount))
            {
                inventory.Total += amount;
                inventory.ByCardId[cardId] = inventory.ByCardId.TryGetValue(cardId, out var qty) ? qty + amount : amount;
            }

            if (evt.ActionType == "order.claimed" &&
                EventPayloadReader.TryReadOrderClaim(EventPayloadReader.ReadPayload(evt.Payload), out var requiredCards, out _))
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
                EventPayloadReader.TryReadIngredientPurchase(EventPayloadReader.ReadPayload(evt.Payload), out var discardCardId, out var discardAmount) &&
                inventory.ByCardId.TryGetValue(discardCardId, out var discardQty))
            {
                var newQty = Math.Max(0, discardQty - discardAmount);
                inventory.ByCardId[discardCardId] = newQty;
                inventory.Total = Math.Max(0, inventory.Total - discardAmount);
            }
        }

        return inventory;
    }

    /// <summary>
    /// Menghitung saldo tabungan pemain untuk goal tertentu dari deposit, penarikan, dan pencapaian goal.
    /// </summary>
    internal static int ComputeSavingBalance(IEnumerable<EventDb> events, Guid playerId, string goalId)
    {
        var balance = 0;

        foreach (var evt in events.Where(e => e.PlayerId == playerId))
        {
            if (evt.ActionType == "saving.deposit.created" &&
                EventPayloadReader.TryReadSavingDeposit(EventPayloadReader.ReadPayload(evt.Payload), out var existingGoalId, out var amount) &&
                string.Equals(existingGoalId, goalId, StringComparison.OrdinalIgnoreCase))
            {
                balance += amount;
            }

            if (evt.ActionType == "saving.deposit.withdrawn" &&
                EventPayloadReader.TryReadSavingDeposit(EventPayloadReader.ReadPayload(evt.Payload), out var withdrawGoalId, out var amountWithdraw) &&
                string.Equals(withdrawGoalId, goalId, StringComparison.OrdinalIgnoreCase))
            {
                balance -= amountWithdraw;
            }

            if (evt.ActionType == "saving.goal.achieved" &&
                EventPayloadReader.TryReadSavingGoalAchieved(EventPayloadReader.ReadPayload(evt.Payload), out var achievedGoalId, out _, out var cost) &&
                string.Equals(achievedGoalId, goalId, StringComparison.OrdinalIgnoreCase))
            {
                balance -= cost;
            }
        }

        return balance;
    }
}
