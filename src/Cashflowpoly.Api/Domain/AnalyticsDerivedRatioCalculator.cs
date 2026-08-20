// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui AnalyticsDerivedRatioCalculator.
using Cashflowpoly.Api.Data;
using static Cashflowpoly.Api.Domain.AnalyticsMath;

namespace Cashflowpoly.Api.Domain;

public sealed record AnalyticsDerivedRatioMetrics(
    double? ExpenseEfficiency,
    double? BusinessProfitMargin,
    double? GoalAttemptRate,
    double? GoalInvestmentRate,
    double? GoalAmbitionIndex,
    double? GoalSettingAmbition,
    double? MealOrderSuccessRate,
    double? PlanningHorizon,
    double? PlanningHorizonPercent,
    int SavingsActionCount,
    int FinancialGoalActionCount,
    int InsurancePremiumActionCount,
    double? PrimaryNeedShare,
    double? SecondaryNeedShare,
    double? TertiaryNeedShare,
    double? GrowthPatternRatio,
    double? RiskAppetiteScoreNormalized);

internal sealed class DerivedRatioCalculator : IDerivedRatioCalculator
{
    public AnalyticsDerivedRatioMetrics Compute(
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
        var goalInvestmentRate = SafeRatio(
            savingGoalMetrics.FinancialGoalsCoinsTotalInvested,
            coinsNetEndGame + savingGoalMetrics.CoinsSaved);
        var goalAmbitionIndex = goalAttemptRate.HasValue && goalInvestmentRate.HasValue
            ? Clamp(((goalAttemptRate.Value + goalInvestmentRate.Value) / 2) * 100, 0, 100)
            : (double?)null;
        var goalInvestmentToNetWorth = SafeRatio(
            savingGoalMetrics.FinancialGoalsCoinsTotalInvested,
            coinsNetEndGame);
        var goalSettingAmbition = goalInvestmentToNetWorth.HasValue
            ? savingGoalMetrics.FinancialGoalsAttempted + goalInvestmentToNetWorth.Value * 100
            : (double?)null;

        var mealOrdersAttempted = mealOrdersClaimed + mealOrdersPassed;
        var mealOrderSuccessRate = SafeRatio(mealOrdersClaimed, mealOrdersAttempted, true);
        var savingsActionCount = playerEvents.Count(e =>
            e.ActorType == "PLAYER" && e.ActionType == GameActionCatalog.Menabung);
        var financialGoalActionCount = playerEvents.Count(e =>
            e.ActorType == "PLAYER" && e.ActionType == GameActionCatalog.TujuanFinansial);
        var insurancePremiumActionCount = playerEvents.Count(e =>
            e.ActorType == "PLAYER" &&
            e.ActionType == GameActionCatalog.Asuransi &&
            e.Payload.Contains("\"premium\"", StringComparison.OrdinalIgnoreCase));
        var longTermActionCount = savingsActionCount + financialGoalActionCount + insurancePremiumActionCount;
        var planningHorizon = SafeRatio(longTermActionCount, actionEventCount);
        var planningHorizonPercent = planningHorizon.HasValue ? planningHorizon.Value * 100 : (double?)null;

        var totalNeeds = needMissionMetrics.NeedCardsOwnedCurrent;
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
            goalAmbitionIndex,
            goalSettingAmbition,
            mealOrderSuccessRate,
            planningHorizon,
            planningHorizonPercent,
            savingsActionCount,
            financialGoalActionCount,
            insurancePremiumActionCount,
            primaryNeedShare,
            secondaryNeedShare,
            tertiaryNeedShare,
            growthPatternRatio,
            riskAppetiteScoreNormalized);
    }
}
