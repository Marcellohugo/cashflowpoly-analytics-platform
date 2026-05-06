using Cashflowpoly.Api.Data;

namespace Cashflowpoly.Api.Domain;

internal interface IIngredientInventoryCalculator
{
    AnalyticsIngredientInventory BuildIngredientInventory(List<EventDb> events);
}
