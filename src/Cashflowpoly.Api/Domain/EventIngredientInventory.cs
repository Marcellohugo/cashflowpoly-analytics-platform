// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui EventIngredientInventory.
// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Domain;

/// <summary>
/// Inventaris bahan pemain: total kartu dan jumlah per card ID.
/// </summary>
// Mendefinisikan tipe class `EventIngredientInventory`; sealed mencegah tipe ini diturunkan lagi.
public sealed class EventIngredientInventory
{
    public int Total { get; set; }

    /// <summary>
    /// Pemetaan card ID ke jumlah kartu bahan yang dimiliki.
    /// </summary>
    // Mendefinisikan properti `ByCardId` bertipe `Dictionary<string, int>` untuk nilai berdasarkan kartu identitas; get menyediakan pembacaan nilai;
    // nilai awalnya objek baru dengan tipe mengikuti konteks tujuan dan argumen (StringComparer.OrdinalIgnoreCase).
    public Dictionary<string, int> ByCardId { get; } = new(StringComparer.OrdinalIgnoreCase);
}
