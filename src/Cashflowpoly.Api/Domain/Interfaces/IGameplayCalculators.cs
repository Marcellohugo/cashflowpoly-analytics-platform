// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui IGameplayCalculators.
using Cashflowpoly.Api.Data;

namespace Cashflowpoly.Api.Domain;

public interface ICashTimelineCalculator
{
    AnalyticsCashTimeline Compute(
        IReadOnlyCollection<EventDb> sessionEvents,
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
    AnalyticsSavingGoalMetrics Compute(IEnumerable<EventDb> playerEvents, int? availableGoalCount = null);
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
        double totalIncome,
        IReadOnlyCollection<Cashflowpoly.Api.Contracts.RulesetLifeRiskDto>? lifeRisks = null);
}

public interface IActionUsageCalculator
{
    AnalyticsActionUsageMetrics Compute(
        IReadOnlyCollection<EventDb> playerEvents,
        IReadOnlyCollection<CashflowProjectionDb> playerProjections,
        int latestDayIndex,
        int actionsPerTurn);
}

public interface IIncomeDiversificationCalculator
{
    AnalyticsIncomeDiversificationMetrics Compute(
        IReadOnlyCollection<EventDb> playerEvents,
        int mealOrderIncomeTotal,
        int goldInvestmentEarned);
}
