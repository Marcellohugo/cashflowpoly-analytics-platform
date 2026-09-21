// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui EventDerivedStateCalculator.
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Domain;

/// <summary>
/// Kalkulator state turunan event yang dipakai validasi domain lanjutan.
/// </summary>
// Mendefinisikan tipe class `EventDerivedStateCalculator` yang mewarisi atau menerapkan `IEventDerivedStateCalculator`; sealed mencegah tipe ini
// diturunkan lagi.
internal sealed class EventDerivedStateCalculator : IEventDerivedStateCalculator
{
    private static readonly EventPayloadReader _payloadReader = new();

    /// <summary>
    /// Membangun inventaris bahan pemain dari event ingredient purchased, order claimed, dan ingredient discarded.
    /// </summary>
    // Mendefinisikan metode `BuildIngredientInventory` dengan hasil bertipe `EventIngredientInventory`. Membangun inventaris bahan pemain dari event
    // ingredient purchased, order claimed, dan ingredient discarded. Masukan: Parameter `events` bertipe `IEnumerable<EventDb>` membawa kumpulan event
    // permainan sebagai sumber riwayat untuk validasi atau perhitungan; Parameter `playerId` bertipe `Guid` membawa nilai pemain identitas.
    public EventIngredientInventory BuildIngredientInventory(IEnumerable<EventDb> events, Guid playerId)
    {
        var inventory = new EventIngredientInventory();

        // Mengulangi setiap elemen `events.Where(e => e.UserId == playerId)`; elemen saat ini disimpan sebagai `evt` bertipe `var` untuk diproses oleh
        // badan loop dalam BuildIngredientInventory.
        foreach (var evt in events.Where(e => e.UserId == playerId).OrderBy(e => e.SequenceNumber))
        {
            var payload = _payloadReader.ReadPayload(evt.Payload);
            if ((GameActionCatalog.Is(evt.ActionType, payload, GameActionCatalog.BahanMasakan) ||
                 GameActionCatalog.Is(evt.ActionType, payload, GameActionCatalog.SetupBahanAwal)) &&
                _payloadReader.TryReadIngredientPurchase(payload, out var cardId, out _))
            {
                inventory.Total += 1;
                inventory.ByCardId[cardId] = inventory.ByCardId.TryGetValue(cardId, out var qty) ? qty + 1 : 1;
            }

            if (GameActionCatalog.Is(evt.ActionType, payload, GameActionCatalog.JualMasakan) &&
                _payloadReader.TryReadOrderClaim(payload, out var requiredCards, out _))
            {
                // Mengulangi setiap elemen `requiredCards`; elemen saat ini disimpan sebagai `card` bertipe `var` untuk diproses oleh badan loop dalam
                // BuildIngredientInventory.
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
    /// Menghitung tabungan bersama milik pemain yang dapat dipakai membeli tujuan mana pun.
    /// </summary>
    public int ComputeSavingBalance(IEnumerable<EventDb> events, Guid playerId, int initialSaving = 0)
    {
        return ComputeTotalSavings(events.Where(e => e.UserId == playerId), initialSaving);
    }

    internal static int ComputeTotalSavings(
        IEnumerable<EventDb> playerEvents, int initialSaving)
    {
        var balance = Math.Max(0, initialSaving);

        // goal_id pada setoran lama hanyalah label, bukan pemesanan atau alokasi dana.
        foreach (var evt in playerEvents)
        {
            var payload = _payloadReader.ReadPayload(evt.Payload);
            if (GameActionCatalog.Is(evt.ActionType, payload, GameActionCatalog.Menabung) &&
                _payloadReader.TryReadSavingDeposit(payload, out _, out var amount))
            {
                balance += amount;
            }

            if (GameActionCatalog.Is(evt.ActionType, payload, GameActionCatalog.SavingDepositWithdrawn) &&
                _payloadReader.TryReadSavingDeposit(payload, out _, out var amountWithdraw))
            {
                balance -= amountWithdraw;
            }

            if (GameActionCatalog.Is(evt.ActionType, payload, GameActionCatalog.TujuanFinansial) &&
                _payloadReader.TryReadSavingGoalAchieved(payload, out _, out _, out var cost))
            {
                balance -= cost;
            }
        }

        return Math.Max(0, balance);
    }
}
