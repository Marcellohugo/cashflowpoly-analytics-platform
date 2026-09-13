// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui AnalyticsIngredientMealCalculator.
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Domain;

public sealed record AnalyticsIngredientMealMetrics(
    // Parameter `Inventory` bertipe `AnalyticsIngredientInventory` membawa nilai inventory.
    AnalyticsIngredientInventory Inventory,
    // Parameter `IngredientTypesHeld` bertipe `IReadOnlyDictionary<string, int>` membawa nilai bahan types held.
    IReadOnlyDictionary<string, int> IngredientTypesHeld,
    // Parameter `IngredientsCollected` bertipe `int` membawa nilai bahan collected.
    int IngredientsCollected,
    // Parameter `IngredientsUsedPerMeal` bertipe `IReadOnlyList<int>` membawa nilai bahan used per meal.
    IReadOnlyList<int> IngredientsUsedPerMeal,
    // Parameter `IngredientsUsedTotal` bertipe `int` membawa nilai bahan used total.
    int IngredientsUsedTotal,
    // Parameter `IngredientsWasted` bertipe `int` membawa nilai bahan wasted.
    int IngredientsWasted,
    // Parameter `IngredientInvestmentTotal` bertipe `int` membawa nilai bahan investment total.
    int IngredientInvestmentTotal,
    // Parameter `MealOrderIncomeValues` bertipe `IReadOnlyList<int>` membawa nilai meal urutan/pesanan pemasukan nilai.
    IReadOnlyList<int> MealOrderIncomeValues,
    // Parameter `MealOrdersClaimed` bertipe `int` membawa nilai meal pesanan claimed.
    int MealOrdersClaimed,
    // Parameter `MealOrderIncomeTotal` bertipe `int` membawa nilai meal urutan/pesanan pemasukan total.
    int MealOrderIncomeTotal,
    // Parameter `LatestDayIndex` bertipe `int` membawa nilai latest hari index.
    int LatestDayIndex,
    // Parameter `MealOrdersPerTurnAverage` bertipe `double` membawa nilai meal pesanan per giliran rata-rata.
    double MealOrdersPerTurnAverage,
    // Parameter `EssentialIngredientExpenses` bertipe `double` membawa nilai essential bahan expenses.
    double EssentialIngredientExpenses);

internal sealed class IngredientMealCalculator : IIngredientMealCalculator
{
    private static readonly HashSet<string> _retiredActionTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "LewatiOrder",
        "AmbilKartuDariDeck",
        "KartuMasukDiscard",
        "IsiUlangPasar"
    };
    private static readonly AnalyticsPayloadReader _payloadReader = new();
    private static readonly IngredientInventoryCalculator _inventoryCalculator = new();

    public AnalyticsIngredientMealMetrics Compute(
        // Parameter `playerEvents` bertipe `IReadOnlyCollection<EventDb>` membawa nilai pemain event.
        IReadOnlyCollection<EventDb> playerEvents,
        // Parameter `playerProjections` bertipe `IReadOnlyCollection<CashflowProjectionDb>` membawa nilai pemain projections.
        IReadOnlyCollection<CashflowProjectionDb> playerProjections)
    {
        playerEvents = playerEvents.Where(e => !_retiredActionTypes.Contains(e.ActionType)).ToArray();

        var ingredientPurchaseMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var ingredientsCollected = 0;
        // Mengulangi setiap elemen `playerEvents.Where(e => string.Equals(e.ActionType, ”BahanMasakan”, StringComparison.OrdinalIgnoreCase) ||
        // string.Equals(e.ActionType, ”SetupBahanAwal”, StringComparison.Ordin...`; elemen saat ini disimpan sebagai `evt` bertipe `var` untuk diproses
        // oleh badan loop dalam Compute.
        foreach (var evt in playerEvents.Where(e => string.Equals(e.ActionType, "BahanMasakan", StringComparison.OrdinalIgnoreCase) || string.Equals(e.ActionType, "SetupBahanAwal", StringComparison.OrdinalIgnoreCase)))
        {
            if (_payloadReader.TryReadIngredientPurchaseDetailed(evt.Payload, out var cardId, out var ingredientName, out var amount))
            {
                ingredientsCollected += 1;
                if (!string.IsNullOrWhiteSpace(cardId) && !string.IsNullOrWhiteSpace(ingredientName))
                {
                    ingredientPurchaseMap[cardId] = ingredientName;
                }
            }
            else if (_payloadReader.TryReadIngredientPurchase(evt.Payload, out var fallbackCardId, out var fallbackAmount))
            {
                ingredientsCollected += 1;
                if (!string.IsNullOrWhiteSpace(fallbackCardId) && !ingredientPurchaseMap.ContainsKey(fallbackCardId))
                {
                    ingredientPurchaseMap[fallbackCardId] = fallbackCardId;
                }
            }
        }

        var inventory = _inventoryCalculator.BuildIngredientInventory(playerEvents.ToList());
        var ingredientTypesHeld = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        foreach (var (cardId, qty) in inventory.ByCardId)
        {
            if (qty <= 0)
            {
                // Melewati sisa pernyataan pada iterasi saat ini dan melanjutkan ke elemen/iterasi berikutnya dalam Compute.
                continue;
            }

            var name = ingredientPurchaseMap.TryGetValue(cardId, out var ingredientName) ? ingredientName : cardId;
            ingredientTypesHeld[name] = ingredientTypesHeld.TryGetValue(name, out var existing) ? existing + qty : qty;
        }

        var ingredientsUsedPerMeal = playerEvents
            .Where(e => e.ActionType == "JualMasakan")
            .Select(e => _payloadReader.TryReadOrderClaim(e.Payload, out var cards, out _) ? cards.Count : 0)
            .ToList();
        var ingredientsUsedTotal = ingredientsUsedPerMeal.Sum();

        var ingredientsWasted = playerEvents
            .Where(e => e.ActionType == "BuangBahanMasakan")
            .Select(e => _payloadReader.TryReadIngredientPurchase(e.Payload, out _, out var amount) ? amount : 0)
            .Sum();

        var ingredientInvestmentTotal = playerProjections
            .Where(p => p.Category == "INGREDIENT" && p.Direction == "OUT")
            .Sum(p => p.Amount);

        var mealOrderIncomeValues = new List<int>();
        // Mengulangi setiap elemen `playerEvents.Where(e => e.ActionType == ”JualMasakan”)`; elemen saat ini disimpan sebagai `evt` bertipe `var` untuk
        // diproses oleh badan loop dalam Compute.
        foreach (var evt in playerEvents.Where(e => e.ActionType == "JualMasakan"))
        {
            if (_payloadReader.TryReadOrderClaim(evt.Payload, out _, out var income))
            {
                mealOrderIncomeValues.Add(income);
            }
        }

        var mealOrdersClaimed = mealOrderIncomeValues.Count;
        var mealOrderIncomeTotal = mealOrderIncomeValues.Sum();
        var latestDayIndex = playerEvents.Count == 0 ? -1 : playerEvents.Max(e => e.DayIndex);
        var eventPayloadReader = new EventPayloadReader();
        var playedTurnCount = playerEvents
            .Where(e => string.Equals(e.ActorType, "PLAYER", StringComparison.OrdinalIgnoreCase) &&
                        GameActionCatalog.GetPlayerActionSlotPolicy(
                            e.ActionType,
                            eventPayloadReader.ReadPayload(string.IsNullOrWhiteSpace(e.Payload) ? "{}" : e.Payload)) == PlayerActionSlotPolicy.Consumes)
            .Select(e => e.DayIndex)
            .Distinct()
            .Count();
        var mealOrdersPerTurnAverage = playedTurnCount > 0 ? (double)mealOrdersClaimed / playedTurnCount : 0;
        var essentialIngredientExpenses = ComputeEssentialIngredientExpenses(playerEvents);

        return new AnalyticsIngredientMealMetrics(
            inventory,
            ingredientTypesHeld,
            ingredientsCollected,
            ingredientsUsedPerMeal,
            ingredientsUsedTotal,
            ingredientsWasted,
            ingredientInvestmentTotal,
            mealOrderIncomeValues,
            mealOrdersClaimed,
            mealOrderIncomeTotal,
            latestDayIndex,
            mealOrdersPerTurnAverage,
            essentialIngredientExpenses);
    }

    private double ComputeEssentialIngredientExpenses(IEnumerable<EventDb> playerEvents)
    {
        var purchaseCostByCardId = new Dictionary<string, Queue<double>>(StringComparer.OrdinalIgnoreCase);
        var essentialIngredientExpenses = 0d;

        // Mengulangi setiap elemen `playerEvents.OrderBy(e => e.SequenceNumber)`; elemen saat ini disimpan sebagai `evt` bertipe `var` untuk diproses oleh
        // badan loop dalam ComputeEssentialIngredientExpenses.
        foreach (var evt in playerEvents.OrderBy(e => e.SequenceNumber))
        {
            if ((string.Equals(evt.ActionType, "BahanMasakan", StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(evt.ActionType, "SetupBahanAwal", StringComparison.OrdinalIgnoreCase)) &&
                _payloadReader.TryReadIngredientPurchase(evt.Payload, out var purchasedCardId, out var purchaseAmount) &&
                !string.IsNullOrWhiteSpace(purchasedCardId))
            {
                if (!purchaseCostByCardId.TryGetValue(purchasedCardId, out var queue))
                {
                    queue = new Queue<double>();
                    purchaseCostByCardId[purchasedCardId] = queue;
                }

                queue.Enqueue(Math.Max(0, purchaseAmount));
            }

            if (evt.ActionType == "JualMasakan" &&
                _payloadReader.TryReadOrderClaim(evt.Payload, out var requiredCards, out _))
            {
                // Mengulangi setiap elemen `requiredCards`; elemen saat ini disimpan sebagai `requiredCard` bertipe `var` untuk diproses oleh badan loop dalam
                // ComputeEssentialIngredientExpenses.
                foreach (var requiredCard in requiredCards)
                {
                    if (!purchaseCostByCardId.TryGetValue(requiredCard, out var queue) || queue.Count == 0)
                    {
                        essentialIngredientExpenses += 1;
                        // Melewati sisa pernyataan pada iterasi saat ini dan melanjutkan ke elemen/iterasi berikutnya dalam ComputeEssentialIngredientExpenses.
                        continue;
                    }

                    essentialIngredientExpenses += queue.Dequeue();
                }
            }
        }

        return essentialIngredientExpenses;
    }
}
