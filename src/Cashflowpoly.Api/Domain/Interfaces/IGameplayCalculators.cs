using Cashflowpoly.Api.Data;

namespace Cashflowpoly.Api.Domain;

internal interface ICashTimelineCalculator
{
    AnalyticsCashTimeline Compute(
        IReadOnlyCollection<EventDb> playerEvents,
        IReadOnlyCollection<CashflowProjectionDb> playerProjections,
        int startingCoins);
}

internal interface IDonationGameplayCalculator
{
    AnalyticsDonationGameplayMetrics Compute(
        IEnumerable<EventDb> playerEvents,
        IEnumerable<EventDb> allEvents,
        double coinsNetEndGame);
}

internal interface ISavingGoalCalculator
{
    AnalyticsSavingGoalMetrics Compute(IEnumerable<EventDb> playerEvents);
}

internal interface IIngredientMealCalculator
{
    AnalyticsIngredientMealMetrics Compute(
        IReadOnlyCollection<EventDb> playerEvents,
        IReadOnlyCollection<CashflowProjectionDb> playerProjections);
}

internal interface IGoldGameplayCalculator
{
    AnalyticsGoldGameplayMetrics Compute(IEnumerable<EventDb> playerEvents);
}

internal interface INeedMissionCalculator
{
    AnalyticsNeedMissionMetrics Compute(
        IEnumerable<EventDb> playerEvents,
        IEnumerable<CashflowProjectionDb> playerProjections);
}

internal interface IRiskLoanCalculator
{
    AnalyticsRiskLoanMetrics Compute(
        IReadOnlyCollection<EventDb> playerEvents,
        IReadOnlyCollection<CashflowProjectionDb> playerProjections,
        int startingCoins,
        double coinsNetEndGame,
        double totalIncome);
}

internal interface IActionUsageCalculator
{
    AnalyticsActionUsageMetrics Compute(
        IReadOnlyCollection<EventDb> playerEvents,
        IReadOnlyCollection<CashflowProjectionDb> playerProjections,
        int maxTurnNumber,
        int actionsPerTurn);
}

internal interface IIncomeDiversificationCalculator
{
    AnalyticsIncomeDiversificationMetrics Compute(
        IReadOnlyCollection<EventDb> playerEvents,
        IReadOnlyCollection<CashflowProjectionDb> playerProjections,
        double totalIncome,
        int mealOrderIncomeTotal,
        int goldInvestmentEarned);
}

internal interface IDerivedRatioCalculator
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
