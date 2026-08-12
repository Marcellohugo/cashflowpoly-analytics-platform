// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui EventDerivedStateCalculator.
using Cashflowpoly.Api.Data;

namespace Cashflowpoly.Api.Domain;

/// <summary>
/// Kalkulator state turunan event yang dipakai validasi domain lanjutan.
/// </summary>
internal sealed class EventDerivedStateCalculator : IEventDerivedStateCalculator
{
    private static readonly EventPayloadReader _payloadReader = new();

    /// <summary>
    /// Membangun inventaris bahan pemain dari event ingredient purchased, order claimed, dan ingredient discarded.
    /// </summary>
    public EventIngredientInventory BuildIngredientInventory(IEnumerable<EventDb> events, Guid playerId)
    {
        var inventory = new EventIngredientInventory();

        foreach (var evt in events.Where(e => e.UserId == playerId))
        {
            var payload = _payloadReader.ReadPayload(evt.Payload);
            if (GameActionCatalog.Is(evt.ActionType, payload, GameActionCatalog.BahanMasakan) &&
                _payloadReader.TryReadIngredientPurchase(payload, out var cardId, out _))
            {
                inventory.Total += 1;
                inventory.ByCardId[cardId] = inventory.ByCardId.TryGetValue(cardId, out var qty) ? qty + 1 : 1;
            }

            if (GameActionCatalog.Is(evt.ActionType, payload, GameActionCatalog.JualMasakan) &&
                _payloadReader.TryReadOrderClaim(payload, out var requiredCards, out _))
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

            if (GameActionCatalog.Is(evt.ActionType, payload, GameActionCatalog.IngredientDiscarded) &&
                _payloadReader.TryReadIngredientPurchase(payload, out var discardCardId, out var discardAmount) &&
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
    public int ComputeSavingBalance(IEnumerable<EventDb> events, Guid playerId, string goalId)
    {
        var balance = 0;

        foreach (var evt in events.Where(e => e.UserId == playerId))
        {
            var payload = _payloadReader.ReadPayload(evt.Payload);
            if (GameActionCatalog.Is(evt.ActionType, payload, GameActionCatalog.Menabung) &&
                _payloadReader.TryReadSavingDeposit(payload, out var existingGoalId, out var amount) &&
                string.Equals(existingGoalId, goalId, StringComparison.OrdinalIgnoreCase))
            {
                balance += amount;
            }

            if (GameActionCatalog.Is(evt.ActionType, payload, GameActionCatalog.SavingDepositWithdrawn) &&
                _payloadReader.TryReadSavingDeposit(payload, out var withdrawGoalId, out var amountWithdraw) &&
                string.Equals(withdrawGoalId, goalId, StringComparison.OrdinalIgnoreCase))
            {
                balance -= amountWithdraw;
            }

            if (GameActionCatalog.Is(evt.ActionType, payload, GameActionCatalog.TujuanFinansial) &&
                _payloadReader.TryReadSavingGoalAchieved(payload, out var achievedGoalId, out _, out var cost) &&
                string.Equals(achievedGoalId, goalId, StringComparison.OrdinalIgnoreCase))
            {
                balance -= cost;
            }
        }

        return balance;
    }
}
