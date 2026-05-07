// Fungsi file: Menguji kalkulasi ingredient dan meal-order gameplay dari event pemain.
using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Domain;
using Xunit;

namespace Cashflowpoly.Api.Tests;

public sealed class AnalyticsIngredientMealCalculatorTests
{
    [Fact]
    public void Compute_BuildsIngredientInventoryAndMealOrderMetrics()
    {
        var playerId = Guid.NewGuid();
        var sessionId = Guid.NewGuid();
        var flourPurchaseId = Guid.NewGuid();
        var eggPurchaseId = Guid.NewGuid();
        var orderId = Guid.NewGuid();
        var events = new List<EventDb>
        {
            CreateEvent(flourPurchaseId, sessionId, playerId, "ingredient.purchased", """{"card_id":"flour","ingredient_name":"Flour","amount":3}""", turn: 1, sequence: 1),
            CreateEvent(eggPurchaseId, sessionId, playerId, "ingredient.purchased", """{"card_id":"egg","ingredient_name":"Egg","amount":1}""", turn: 1, sequence: 2),
            CreateEvent(orderId, sessionId, playerId, "order.claimed", """{"required_ingredient_card_ids":["flour","egg"],"income":12}""", turn: 2, sequence: 3),
            CreateEvent(Guid.NewGuid(), sessionId, playerId, "ingredient.discarded", """{"card_id":"flour","amount":1}""", turn: 3, sequence: 4),
            CreateEvent(Guid.NewGuid(), sessionId, playerId, "order.passed", """{"required_ingredient_card_ids":["flour"],"income":8}""", turn: 4, sequence: 5)
        };
        var projections = new List<CashflowProjectionDb>
        {
            CreateProjection(flourPurchaseId, sessionId, playerId, "OUT", 3, "INGREDIENT"),
            CreateProjection(eggPurchaseId, sessionId, playerId, "OUT", 1, "INGREDIENT"),
            CreateProjection(orderId, sessionId, playerId, "IN", 12, "ORDER_INCOME")
        };

        var metrics = new IngredientMealCalculator().Compute(events, projections);

        Assert.Equal(4, metrics.IngredientsCollected);
        Assert.Equal(1, metrics.Inventory.Total);
        Assert.Equal(1, metrics.Inventory.ByCardId["flour"]);
        Assert.Equal(0, metrics.Inventory.ByCardId["egg"]);
        Assert.Equal(1, metrics.IngredientTypesHeld["Flour"]);
        Assert.False(metrics.IngredientTypesHeld.ContainsKey("Egg"));
        Assert.Equal(2, metrics.IngredientsUsedTotal);
        Assert.Equal(1, metrics.IngredientsWasted);
        Assert.Equal(4, metrics.IngredientInvestmentTotal);
        Assert.Equal(new[] { 12 }, metrics.MealOrderIncomeValues);
        Assert.Equal(1, metrics.MealOrdersClaimed);
        Assert.Equal(1, metrics.MealOrdersPassed);
        Assert.Equal(12, metrics.MealOrderIncomeTotal);
        Assert.Equal(4, metrics.MaxTurnNumber);
        Assert.Equal(0.25, metrics.MealOrdersPerTurnAverage);
        Assert.Equal(4, metrics.EssentialIngredientExpenses);
    }

    private static EventDb CreateEvent(
        Guid eventId,
        Guid sessionId,
        Guid playerId,
        string actionType,
        string payload,
        int turn,
        long sequence)
    {
        return new EventDb
        {
            EventId = eventId,
            SessionId = sessionId,
            PlayerId = playerId,
            ActorType = "PLAYER",
            Timestamp = new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero),
            DayIndex = 0,
            Weekday = "MON",
            TurnNumber = turn,
            SequenceNumber = sequence,
            ActionType = actionType,
            RulesetVersionId = Guid.NewGuid(),
            Payload = payload
        };
    }

    private static CashflowProjectionDb CreateProjection(
        Guid eventId,
        Guid sessionId,
        Guid playerId,
        string direction,
        int amount,
        string category)
    {
        return new CashflowProjectionDb
        {
            ProjectionId = Guid.NewGuid(),
            SessionId = sessionId,
            PlayerId = playerId,
            EventPk = Guid.NewGuid(),
            EventId = eventId,
            Timestamp = new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero),
            Direction = direction,
            Amount = amount,
            Category = category
        };
    }
}
