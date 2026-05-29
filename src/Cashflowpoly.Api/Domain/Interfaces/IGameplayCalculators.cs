using Cashflowpoly.Api.Data;

namespace Cashflowpoly.Api.Domain;

public interface ICashTimelineCalculator
{
    AnalyticsCashTimeline Compute(
        IReadOnlyCollection<EventDb> playerEvents,
        IReadOnlyCollection<CashflowProjectionDb> playerProjections,
        int startingCoins);
}

public interface IDonationGameplayCalculator
{
    AnalyticsDonationGameplayMetrics Compute(
        IEnumerable<EventDb> playerEvents,
        IEnumerable<EventDb> allEvents,
        double coinsNetEndGame);
}

public interface ISavingGoalCalculator
{
    AnalyticsSavingGoalMetrics Compute(IEnumerable<EventDb> playerEvents);
}

public interface IIngredientMealCalculator
{
    AnalyticsIngredientMealMetrics Compute(
        IReadOnlyCollection<EventDb> playerEvents,
        IReadOnlyCollection<CashflowProjectionDb> playerProjections);
}

public interface IGoldGameplayCalculator
{
    AnalyticsGoldGameplayMetrics Compute(IEnumerable<EventDb> playerEvents);
}

public interface INeedMissionCalculator
{
    AnalyticsNeedMissionMetrics Compute(
        IEnumerable<EventDb> playerEvents,
        IEnumerable<CashflowProjectionDb> playerProjections);
}

public interface IRiskLoanCalculator
{
    AnalyticsRiskLoanMetrics Compute(
        IReadOnlyCollection<EventDb> playerEvents,
        IReadOnlyCollection<CashflowProjectionDb> playerProjections,
        int startingCoins,
        double coinsNetEndGame,
        double totalIncome);
}

public interface IActionUsageCalculator
{
    AnalyticsActionUsageMetrics Compute(
        IReadOnlyCollection<EventDb> playerEvents,
        IReadOnlyCollection<CashflowProjectionDb> playerProjections,
        int maxTurnNumber,
        int actionsPerTurn);
}

public interface IIncomeDiversificationCalculator
{
    AnalyticsIncomeDiversificationMetrics Compute(
        IReadOnlyCollection<EventDb> playerEvents,
        IReadOnlyCollection<CashflowProjectionDb> playerProjections,
        double totalIncome,
        int mealOrderIncomeTotal,
        int goldInvestmentEarned);
}

public interface IDerivedRatioCalculator
{
    AnalyticsDerivedRatioMetrics Compute(
        double essentialIngredientExpenses,
        double totalExpenses,
        int mealOrderIncomeTotal,
        int ingredientInvestmentTotal,
        AnalyticsSavingGoalMetrics savingGoalMetrics,
        double coinsNetEndGame,
        IReadOnlyCollection<EventDb> playerEvents,
        int actionEventCount,
        int mealOrdersClaimed,
        int mealOrdersPassed,
        AnalyticsNeedMissionMetrics needMissionMetrics,
        int startingCoins,
        double? riskAppetiteScore);
}
