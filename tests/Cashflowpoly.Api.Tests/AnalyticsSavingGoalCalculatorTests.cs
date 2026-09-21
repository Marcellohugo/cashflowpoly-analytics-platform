// Fungsi file: Memverifikasi tabungan bersama dan pencapaian tujuan berdasarkan pembelian.
using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Domain;
using Xunit;

namespace Cashflowpoly.Api.Tests;

public sealed class AnalyticsSavingGoalCalculatorTests
{
    [Fact]
    public void Compute_ReportsPurchasesAndRetainsUnspentSharedSavings()
    {
        var playerId = Guid.NewGuid();
        var events = new[]
        {
            CreateEvent(playerId, "Menabung", """{"goal_id":"goal-a","amount":20}"""),
            CreateEvent(playerId, "Menabung", """{"amount":10}"""),
            CreateEvent(playerId, "TarikTabungan", """{"goal_id":"goal-a","amount":5}"""),
            CreateEvent(playerId, "TujuanFinansial", """{"goal_id":"goal-a","points":10,"cost":12}"""),
            CreateEvent(playerId, "Menabung", """{"goal_id":"goal-b","amount":7}"""),
            CreateEvent(playerId, "TarikTabungan", """{"amount":3}"""),
            CreateEvent(playerId, "TujuanFinansial", """{"goal_id":"goal-c","points":5,"cost":3}""")
        };

        var result = new SavingGoalCalculator().Compute(events, availableGoalCount: 4, initialSaving: 2);

        Assert.Empty(result.SavingDepositsByGoal);
        Assert.Empty(result.SavingWithdrawalsByGoal);
        Assert.Empty(result.SavingBalancesByGoal);
        Assert.Equal(12, result.SavingGoalCostsByGoal["goal-a"]);
        Assert.Equal(3, result.SavingGoalCostsByGoal["goal-c"]);
        Assert.Contains("goal-a", result.SavingGoalsAchieved);
        Assert.Contains("goal-c", result.SavingGoalsAchieved);
        Assert.DoesNotContain("goal-b", result.SavingGoalsAchieved);
        Assert.Equal(16, result.CoinsSaved);
        Assert.Equal(2, result.FinancialGoalsAttempted);
        Assert.Equal(4, result.FinancialGoalsAvailableTotal);
        Assert.Equal(2, result.FinancialGoalsCompleted);
        Assert.Equal(15, result.FinancialGoalsCoinsTotalInvested);
        Assert.Equal(0, result.FinancialGoalsIncompleteCoinsWasted);
    }

    [Fact]
    public void Compute_DepositLabelDoesNotReserveOrAttemptAGoal()
    {
        var result = new SavingGoalCalculator().Compute(new[]
        {
            CreateEvent(Guid.NewGuid(), "Menabung", """{"goal_id":"goal-a","amount":20}""")
        }, initialSaving: 5);

        Assert.Equal(25, result.CoinsSaved);
        Assert.Empty(result.SavingBalancesByGoal);
        Assert.Equal(0, result.FinancialGoalsAttempted);
        Assert.Equal(0, result.FinancialGoalsCompleted);
        Assert.Equal(0, result.FinancialGoalsCoinsTotalInvested);
        Assert.Equal(0, result.FinancialGoalsIncompleteCoinsWasted);
        Assert.Null(result.FinancialGoalsAvailableTotal);
    }

    [Fact]
    public void Compute_HandlesMissingGoalActivity()
    {
        var result = new SavingGoalCalculator().Compute(Array.Empty<EventDb>());

        Assert.Empty(result.SavingDepositsByGoal);
        Assert.Empty(result.SavingBalancesByGoal);
        Assert.Equal(0, result.CoinsSaved);
        Assert.Equal(0, result.FinancialGoalsAttempted);
        Assert.Null(result.FinancialGoalsAvailableTotal);
        Assert.Equal(0, result.FinancialGoalsCompleted);
        Assert.Equal(0, result.FinancialGoalsCoinsTotalInvested);
        Assert.Equal(0, result.FinancialGoalsIncompleteCoinsWasted);
    }

    private static EventDb CreateEvent(Guid playerId, string actionType, string payload) => new()
    {
        EventId = Guid.NewGuid(),
        UserId = playerId,
        ActionType = actionType,
        Payload = payload
    };
}