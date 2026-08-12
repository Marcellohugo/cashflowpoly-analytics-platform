// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui IIngredientInventoryCalculator.
using Cashflowpoly.Api.Data;

namespace Cashflowpoly.Api.Domain;

public interface IIngredientInventoryCalculator
{
    AnalyticsIngredientInventory BuildIngredientInventory(List<EventDb> events);
}
