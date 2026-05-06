// Fungsi file: Menghitung metrik ingredient dan meal-order gameplay dari event pemain.
using Cashflowpoly.Api.Data;
using static Cashflowpoly.Api.Domain.AnalyticsPayloadReader;

namespace Cashflowpoly.Api.Domain;

internal sealed record AnalyticsIngredientMealMetrics(
    AnalyticsIngredientInventory Inventory,
    IReadOnlyDictionary<string, int> IngredientTypesHeld,
    int IngredientsCollected,
    int IngredientsUsedTotal,
    int IngredientsWasted,
    int IngredientInvestmentTotal,
    IReadOnlyList<int> MealOrderIncomeValues,
    int MealOrdersClaimed,
    int MealOrdersPassed,
    int MealOrderIncomeTotal,
    int MaxTurnNumber,
    double MealOrdersPerTurnAverage,
    double EssentialIngredientExpenses);

internal static class AnalyticsIngredientMealCalculator
{
    internal static AnalyticsIngredientMealMetrics Compute(
        IReadOnlyCollection<EventDb> playerEvents,
        IReadOnlyCollection<CashflowProjectionDb> playerProjections)
    {
        var ingredientPurchaseMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var ingredientsCollected = 0;
        foreach (var evt in playerEvents.Where(e => e.ActionType == "ingredient.purchased"))
        {
            if (TryReadIngredientPurchaseDetailed(evt.Payload, out var cardId, out var ingredientName, out var amount))
            {
                ingredientsCollected += amount;
                if (!string.IsNullOrWhiteSpace(cardId) && !string.IsNullOrWhiteSpace(ingredientName))
                {
                    ingredientPurchaseMap[cardId] = ingredientName;
                }
            }
            else if (TryReadIngredientPurchase(evt.Payload, out var fallbackCardId, out var fallbackAmount))
            {
                ingredientsCollected += fallbackAmount;
                if (!string.IsNullOrWhiteSpace(fallbackCardId) && !ingredientPurchaseMap.ContainsKey(fallbackCardId))
                {
                    ingredientPurchaseMap[fallbackCardId] = fallbackCardId;
                }
            }
        }

        var inventory = AnalyticsIngredientInventoryCalculator.BuildIngredientInventory(playerEvents.ToList());
        var ingredientTypesHeld = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        foreach (var (cardId, qty) in inventory.ByCardId)
        {
            if (qty <= 0)
            {
                continue;
            }

            var name = ingredientPurchaseMap.TryGetValue(cardId, out var ingredientName) ? ingredientName : cardId;
            ingredientTypesHeld[name] = ingredientTypesHeld.TryGetValue(name, out var existing) ? existing + qty : qty;
        }

        var ingredientsUsedTotal = playerEvents
            .Where(e => e.ActionType == "order.claimed")
            .Select(e => TryReadOrderClaim(e.Payload, out var cards, out _) ? cards.Count : 0)
            .Sum();

        var ingredientsWasted = playerEvents
            .Where(e => e.ActionType == "ingredient.discarded")
            .Select(e => TryReadIngredientPurchase(e.Payload, out _, out var amount) ? amount : 0)
            .Sum();

        var ingredientInvestmentTotal = playerProjections
            .Where(p => p.Category == "INGREDIENT" && p.Direction == "OUT")
            .Sum(p => p.Amount);

        var mealOrderIncomeValues = new List<int>();
        foreach (var evt in playerEvents.Where(e => e.ActionType == "order.claimed"))
        {
            if (TryReadOrderClaim(evt.Payload, out _, out var income))
            {
                mealOrderIncomeValues.Add(income);
            }
        }

        var mealOrdersClaimed = mealOrderIncomeValues.Count;
        var mealOrdersPassed = playerEvents.Count(e => e.ActionType == "order.passed");
        var mealOrderIncomeTotal = mealOrderIncomeValues.Sum();
        var maxTurnNumber = playerEvents.Count == 0 ? 0 : playerEvents.Max(e => e.TurnNumber);
        var mealOrdersPerTurnAverage = maxTurnNumber > 0 ? (double)mealOrdersClaimed / maxTurnNumber : 0;
        var essentialIngredientExpenses = ComputeEssentialIngredientExpenses(playerEvents);

        return new AnalyticsIngredientMealMetrics(
            inventory,
            ingredientTypesHeld,
            ingredientsCollected,
            ingredientsUsedTotal,
            ingredientsWasted,
            ingredientInvestmentTotal,
            mealOrderIncomeValues,
            mealOrdersClaimed,
            mealOrdersPassed,
            mealOrderIncomeTotal,
            maxTurnNumber,
            mealOrdersPerTurnAverage,
            essentialIngredientExpenses);
    }

    private static double ComputeEssentialIngredientExpenses(IEnumerable<EventDb> playerEvents)
    {
        var purchaseCostByCardId = new Dictionary<string, Queue<double>>(StringComparer.OrdinalIgnoreCase);
        var essentialIngredientExpenses = 0d;

        foreach (var evt in playerEvents.OrderBy(e => e.SequenceNumber))
        {
            if (evt.ActionType == "ingredient.purchased" &&
                TryReadIngredientPurchase(evt.Payload, out var purchasedCardId, out var purchaseAmount) &&
                !string.IsNullOrWhiteSpace(purchasedCardId))
            {
                if (!purchaseCostByCardId.TryGetValue(purchasedCardId, out var queue))
                {
                    queue = new Queue<double>();
                    purchaseCostByCardId[purchasedCardId] = queue;
                }

                queue.Enqueue(Math.Max(0, purchaseAmount));
            }

            if (evt.ActionType == "order.claimed" &&
                TryReadOrderClaim(evt.Payload, out var requiredCards, out _))
            {
                foreach (var requiredCard in requiredCards)
                {
                    if (!purchaseCostByCardId.TryGetValue(requiredCard, out var queue) || queue.Count == 0)
                    {
                        essentialIngredientExpenses += 1;
                        continue;
                    }

                    essentialIngredientExpenses += queue.Dequeue();
                }
            }
        }

        return essentialIngredientExpenses;
    }
}
