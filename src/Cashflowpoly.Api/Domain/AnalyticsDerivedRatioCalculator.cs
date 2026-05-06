// Fungsi file: Menghitung rasio derived gameplay lintas goal, aksi, kebutuhan, profit, dan risiko.
using Cashflowpoly.Api.Data;
using static Cashflowpoly.Api.Domain.AnalyticsMath;

namespace Cashflowpoly.Api.Domain;

internal sealed record AnalyticsDerivedRatioMetrics(
    double? ExpenseEfficiency,
    double? BusinessProfitMargin,
    double? GoalAttemptRate,
    double? GoalInvestmentRate,
    double? GoalSettingAmbition,
    double? MealOrderSuccessRate,
    double? PlanningHorizon,
    double? PlanningHorizonPercent,
    double? PrimaryNeedShare,
    double? SecondaryNeedShare,
    double? TertiaryNeedShare,
    double? GrowthPatternRatio,
    double? RiskAppetiteScoreNormalized);

internal static class AnalyticsDerivedRatioCalculator
{
    internal static AnalyticsDerivedRatioMetrics Compute(
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
        double? riskAppetiteScore)
    {
        var expenseEfficiency = SafeRatio(essentialIngredientExpenses, totalExpenses, true);
        var businessProfitMargin = SafeRatio(mealOrderIncomeTotal - ingredientInvestmentTotal, mealOrderIncomeTotal, true);
        var goalAttemptRate = savingGoalMetrics.FinancialGoalsAvailableTotal.HasValue &&
                              savingGoalMetrics.FinancialGoalsAvailableTotal.Value > 0
            ? SafeRatio(savingGoalMetrics.FinancialGoalsAttempted, savingGoalMetrics.FinancialGoalsAvailableTotal.Value)
            : (double?)null;
        var goalInvestmentRate = SafeRatio(savingGoalMetrics.FinancialGoalsCoinsTotalInvested, coinsNetEndGame);
        var goalSettingAmbition = goalAttemptRate.HasValue && goalInvestmentRate.HasValue
            ? ((goalAttemptRate.Value * 0.4) + (goalInvestmentRate.Value * 0.6)) * 100
            : (double?)null;

        var mealOrdersAttempted = mealOrdersClaimed + mealOrdersPassed;
        var mealOrderSuccessRate = SafeRatio(mealOrdersClaimed, mealOrdersAttempted, true);
        var planningHorizon = SafeRatio(
            savingGoalMetrics.FinancialGoalsCoinsTotalInvested +
            savingGoalMetrics.FinancialGoalsAttempted +
            playerEvents.Count(e => e.ActionType == "insurance.multirisk.purchased"),
            actionEventCount);
        var planningHorizonPercent = planningHorizon.HasValue ? planningHorizon.Value * 100 : (double?)null;

        var totalNeeds = needMissionMetrics.NeedCardsPurchased;
        var primaryNeedShare = SafeRatio(needMissionMetrics.PrimaryNeeds, totalNeeds);
        var secondaryNeedShare = SafeRatio(needMissionMetrics.SecondaryNeeds, totalNeeds);
        var tertiaryNeedShare = SafeRatio(needMissionMetrics.TertiaryNeeds, totalNeeds);

        var growthPatternRatio = SafeRatio(coinsNetEndGame, startingCoins);
        var riskAppetiteScoreNormalized = riskAppetiteScore.HasValue
            ? Clamp(riskAppetiteScore.Value, 0, 100)
            : (double?)null;

        return new AnalyticsDerivedRatioMetrics(
            expenseEfficiency,
            businessProfitMargin,
            goalAttemptRate,
            goalInvestmentRate,
            goalSettingAmbition,
            mealOrderSuccessRate,
            planningHorizon,
            planningHorizonPercent,
            primaryNeedShare,
            secondaryNeedShare,
            tertiaryNeedShare,
            growthPatternRatio,
            riskAppetiteScoreNormalized);
    }
}
