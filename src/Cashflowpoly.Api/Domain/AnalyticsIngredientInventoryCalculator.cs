// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui AnalyticsIngredientInventoryCalculator.
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Domain;

/// <summary>
/// Kalkulator inventaris bahan analitik dari event pembelian, klaim pesanan, dan discard.
/// </summary>
// Mendefinisikan tipe class `IngredientInventoryCalculator` yang mewarisi atau menerapkan `IIngredientInventoryCalculator`; sealed mencegah tipe
// ini diturunkan lagi.
internal sealed class IngredientInventoryCalculator : IIngredientInventoryCalculator
{
    private static readonly AnalyticsPayloadReader _payloadReader = new();
    /// <summary>
    /// Membangun inventaris bahan berdasarkan event pembelian, klaim pesanan, dan discard.
    /// </summary>
    // Mendefinisikan metode `BuildIngredientInventory` dengan hasil bertipe `AnalyticsIngredientInventory`. Membangun inventaris bahan berdasarkan
    // event pembelian, klaim pesanan, dan discard. Masukan: Parameter `events` bertipe `List<EventDb>` membawa kumpulan event permainan sebagai sumber
    // riwayat untuk validasi atau perhitungan.
    public AnalyticsIngredientInventory BuildIngredientInventory(List<EventDb> events)
    {
        var inventory = new AnalyticsIngredientInventory();

        // Mengulangi setiap elemen `events`; elemen saat ini disimpan sebagai `evt` bertipe `var` untuk diproses oleh badan loop dalam
        // BuildIngredientInventory.
        foreach (var evt in events)
        {
            if ((string.Equals(evt.ActionType, "BahanMasakan", StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(evt.ActionType, "SetupBahanAwal", StringComparison.OrdinalIgnoreCase)) &&
                _payloadReader.TryReadIngredientPurchase(evt.Payload, out var cardId, out var amount))
            {
                inventory.Total += 1;
                inventory.ByCardId[cardId] = inventory.ByCardId.TryGetValue(cardId, out var qty) ? qty + 1 : 1;
            }

            if (evt.ActionType == "JualMasakan" &&
                _payloadReader.TryReadOrderClaim(evt.Payload, out var requiredCards, out _))
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

            if (evt.ActionType == "BuangBahanMasakan" &&
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
