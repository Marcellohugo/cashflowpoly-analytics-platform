// Fungsi file: Model inventaris bahan untuk kalkulasi analitik pemain.
namespace Cashflowpoly.Api.Domain;

/// <summary>
/// Inventaris bahan: total kartu dan jumlah per card ID.
/// </summary>
internal sealed class AnalyticsIngredientInventory
{
    internal int Total { get; set; }

    /// <summary>
    /// Pemetaan card ID ke jumlah kartu bahan yang dimiliki.
    /// </summary>
    internal Dictionary<string, int> ByCardId { get; } = new(StringComparer.OrdinalIgnoreCase);
}
