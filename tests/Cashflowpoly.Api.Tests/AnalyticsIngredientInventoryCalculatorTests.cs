// Fungsi file: Menguji kalkulasi inventaris bahan analitik dari event gameplay pemain.
using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Domain;
using Xunit;

namespace Cashflowpoly.Api.Tests;

public sealed class AnalyticsIngredientInventoryCalculatorTests
{
    [Fact]
    public void BuildIngredientInventory_AppliesPurchaseOrderClaimAndDiscard()
    {
        var events = new List<EventDb>
        {
            BuildEvent("ingredient.purchased", """{"card_id":"flour","amount":2}"""),
            BuildEvent("ingredient.purchased", """{"card_id":"egg","amount":1}"""),
            BuildEvent("order.claimed", """{"required_ingredient_card_ids":["flour","egg"],"income":8}"""),
            BuildEvent("ingredient.discarded", """{"card_id":"flour","amount":1}""")
        };

        var inventory = new IngredientInventoryCalculator().BuildIngredientInventory(events);

        Assert.Equal(0, inventory.Total);
        Assert.Equal(0, inventory.ByCardId["flour"]);
        Assert.Equal(0, inventory.ByCardId["egg"]);
    }

    [Fact]
    public void BuildIngredientInventory_IgnoresMalformedPayloads()
    {
        var events = new List<EventDb>
        {
            BuildEvent("ingredient.purchased", """{"card_id":"flour","amount":2}"""),
            BuildEvent("ingredient.purchased", """{"card_id":"","amount":5}"""),
            BuildEvent("ingredient.discarded", """{"card_id":"unknown","amount":2}""")
        };

        var inventory = new IngredientInventoryCalculator().BuildIngredientInventory(events);

        Assert.Equal(2, inventory.Total);
        Assert.Equal(2, inventory.ByCardId["flour"]);
        Assert.False(inventory.ByCardId.ContainsKey("unknown"));
    }

    private static EventDb BuildEvent(string actionType, string payload)
    {
        return new EventDb
        {
            EventId = Guid.NewGuid(),
            ActionType = actionType,
            Payload = payload
        };
    }
}
