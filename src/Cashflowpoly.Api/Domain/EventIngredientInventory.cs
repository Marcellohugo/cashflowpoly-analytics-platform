// Fungsi file: Model state inventaris bahan yang dihitung dari rangkaian event gameplay.
namespace Cashflowpoly.Api.Domain;

/// <summary>
/// Inventaris bahan pemain: total kartu dan jumlah per card ID.
/// </summary>
public sealed class EventIngredientInventory
{
    public int Total { get; set; }

    /// <summary>
    /// Pemetaan card ID ke jumlah kartu bahan yang dimiliki.
    /// </summary>
    public Dictionary<string, int> ByCardId { get; } = new(StringComparer.OrdinalIgnoreCase);
}
