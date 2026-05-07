// Fungsi file: Menguji kalkulasi state turunan event seperti inventaris bahan dan saldo tabungan.
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
            BuildEvent(playerId, "ingredient.purchased", """{"card_id":"flour","amount":2}"""),
            BuildEvent(playerId, "ingredient.purchased", """{"card_id":"egg","amount":1}"""),
            BuildEvent(playerId, "order.claimed", """{"required_ingredient_card_ids":["flour","egg"],"income":8}"""),
            BuildEvent(playerId, "ingredient.discarded", """{"card_id":"flour","amount":1}"""),
            BuildEvent(otherPlayerId, "ingredient.purchased", """{"card_id":"flour","amount":10}""")
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
            BuildEvent(playerId, "saving.deposit.created", """{"goal_id":"bike","amount":10}"""),
            BuildEvent(playerId, "saving.deposit.withdrawn", """{"goal_id":"bike","amount":3}"""),
            BuildEvent(playerId, "saving.goal.achieved", """{"goal_id":"bike","points":4,"cost":5}"""),
            BuildEvent(playerId, "saving.deposit.created", """{"goal_id":"book","amount":99}"""),
            BuildEvent(Guid.NewGuid(), "saving.deposit.created", """{"goal_id":"bike","amount":99}""")
        };

        var balance = new EventDerivedStateCalculator().ComputeSavingBalance(events, playerId, "bike");

        Assert.Equal(2, balance);
    }

    private static EventDb BuildEvent(Guid playerId, string actionType, string payload)
    {
        return new EventDb
        {
            EventId = Guid.NewGuid(),
            PlayerId = playerId,
            ActionType = actionType,
            Payload = payload
        };
    }
}
