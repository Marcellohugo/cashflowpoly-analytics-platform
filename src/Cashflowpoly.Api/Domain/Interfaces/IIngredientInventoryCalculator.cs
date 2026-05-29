using Cashflowpoly.Api.Data;

namespace Cashflowpoly.Api.Domain;

public interface IIngredientInventoryCalculator
{
    AnalyticsIngredientInventory BuildIngredientInventory(List<EventDb> events);
}
