// Fungsi file: Menguji kalkulasi rasio derived gameplay yang menggabungkan goal, aksi, kebutuhan, dan profit.
using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Domain;
using Xunit;

namespace Cashflowpoly.Api.Tests;

public sealed class AnalyticsDerivedRatioCalculatorTests
{
    [Fact]
    public void Compute_CalculatesDerivedRatiosAndNormalizedRiskScore()
    {
        var savingGoalMetrics = new AnalyticsSavingGoalMetrics(
            new Dictionary<string, int>(),
            new Dictionary<string, int>(),
            new Dictionary<string, int>(),
            new HashSet<string>(),
            new Dictionary<string, int>(),
            CoinsSaved: 0,
            FinancialGoalsAttempted: 2,
            FinancialGoalsAvailableTotal: 4,
            FinancialGoalsCompleted: 1,
            FinancialGoalsCoinsTotalInvested: 20,
            FinancialGoalsIncompleteCoinsWasted: 5);
        var needMissionMetrics = new AnalyticsNeedMissionMetrics(
            NeedCardsPurchased: 4,
            PrimaryNeeds: 1,
            SecondaryNeeds: 1,
            TertiaryNeeds: 2,
            HasBasicNeedProfile: true,
            IsCollectorNeedProfile: false,
            IsSpecialistNeedProfile: false,
            SpecificTertiaryAcquired: null,
            CollectionMissionComplete: null,
            NeedCoinsSpent: 0,
            FulfillmentDiversity: 80,
            MissionAchievement: 1);
        var playerEvents = new List<EventDb>
        {
            CreateEvent("insurance.multirisk.purchased")
        };

        var metrics = AnalyticsDerivedRatioCalculator.Compute(
            essentialIngredientExpenses: 5,
            totalExpenses: 20,
            mealOrderIncomeTotal: 30,
            ingredientInvestmentTotal: 10,
            savingGoalMetrics,
            coinsNetEndGame: 40,
            playerEvents,
            actionEventCount: 5,
            mealOrdersClaimed: 3,
            mealOrdersPassed: 1,
            needMissionMetrics,
            startingCoins: 20,
            riskAppetiteScore: 120);

        Assert.Equal(25, metrics.ExpenseEfficiency);
        Assert.Equal(66.666667, metrics.BusinessProfitMargin!.Value, precision: 6);
        Assert.Equal(0.5, metrics.GoalAttemptRate);
        Assert.Equal(0.5, metrics.GoalInvestmentRate);
        Assert.Equal(50, metrics.GoalSettingAmbition);
        Assert.Equal(75, metrics.MealOrderSuccessRate);
        Assert.Equal(4.6, metrics.PlanningHorizon!.Value, precision: 6);
        Assert.Equal(460, metrics.PlanningHorizonPercent!.Value, precision: 6);
        Assert.Equal(0.25, metrics.PrimaryNeedShare);
        Assert.Equal(0.25, metrics.SecondaryNeedShare);
        Assert.Equal(0.5, metrics.TertiaryNeedShare);
        Assert.Equal(2, metrics.GrowthPatternRatio);
        Assert.Equal(100, metrics.RiskAppetiteScoreNormalized);
    }

    private static EventDb CreateEvent(string actionType)
    {
        return new EventDb
        {
            EventId = Guid.NewGuid(),
            SessionId = Guid.NewGuid(),
            PlayerId = Guid.NewGuid(),
            ActorType = "PLAYER",
            Timestamp = new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero),
            DayIndex = 0,
            Weekday = "MON",
            TurnNumber = 1,
            SequenceNumber = 1,
            ActionType = actionType,
            RulesetVersionId = Guid.NewGuid(),
            Payload = "{}"
        };
    }
}
