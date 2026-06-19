using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Domain;
using Xunit;

namespace Cashflowpoly.Api.Tests;

public sealed class EventDerivedStateCalculatorTests
{
    [Fact]
    public void BuildIngredientInventory_AppliesPurchasesOrderClaimsAndDiscards()
    {
        var playerId = Guid.NewGuid();
        var otherPlayerId = Guid.NewGuid();
        var events = new List<EventDb>
        {
            BuildEvent(playerId, "BahanMasakan", """{"card_id":"flour","amount":2}"""),
            BuildEvent(playerId, "BahanMasakan", """{"card_id":"egg","amount":1}"""),
            BuildEvent(playerId, "JualMasakan", """{"required_ingredient_card_ids":["flour","egg"],"income":8}"""),
            BuildEvent(playerId, "BuangBahanMasakan", """{"card_id":"flour","amount":1}"""),
            BuildEvent(otherPlayerId, "BahanMasakan", """{"card_id":"flour","amount":10}""")
        };

        var inventory = new EventDerivedStateCalculator().BuildIngredientInventory(events, playerId);

        Assert.Equal(0, inventory.Total);
        Assert.Equal(0, inventory.ByCardId["flour"]);
        Assert.Equal(0, inventory.ByCardId["egg"]);
    }

    [Fact]
    public void ComputeSavingBalance_AppliesDepositsWithdrawalsAndGoalCost()
    {
        var playerId = Guid.NewGuid();
        var events = new List<EventDb>
        {
            BuildEvent(playerId, "Menabung", """{"goal_id":"bike","amount":10}"""),
            BuildEvent(playerId, "TarikTabungan", """{"goal_id":"bike","amount":3}"""),
            BuildEvent(playerId, "TujuanFinansial", """{"goal_id":"bike","points":4,"cost":5}"""),
            BuildEvent(playerId, "Menabung", """{"goal_id":"book","amount":99}"""),
            BuildEvent(Guid.NewGuid(), "Menabung", """{"goal_id":"bike","amount":99}""")
        };

        var balance = new EventDerivedStateCalculator().ComputeSavingBalance(events, playerId, "bike");

        Assert.Equal(2, balance);
    }

    private static EventDb BuildEvent(Guid playerId, string actionType, string payload)
    {
        return new EventDb
        {
            EventId = Guid.NewGuid(),
            UserId = playerId,
            ActionType = actionType,
            Payload = payload
        };
    }
}
