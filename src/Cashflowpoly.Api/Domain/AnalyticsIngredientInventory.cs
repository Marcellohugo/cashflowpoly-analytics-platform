// Fungsi file: Model inventaris bahan untuk kalkulasi analitik pemain.
namespace Cashflowpoly.Api.Domain;

/// <summary>
/// Inventaris bahan: total kartu dan jumlah per card ID.
/// </summary>
public sealed class AnalyticsIngredientInventory
{
    public int Total { get; set; }

    /// <summary>
    /// Pemetaan card ID ke jumlah kartu bahan yang dimiliki.
    /// </summary>
    public Dictionary<string, int> ByCardId { get; } = new(StringComparer.OrdinalIgnoreCase);
}
