// Fungsi file: Menguji kalkulasi tabungan dan financial goals dari histori event gameplay.
using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Domain;
using Xunit;

namespace Cashflowpoly.Api.Tests;

public sealed class AnalyticsSavingGoalCalculatorTests
{
    [Fact]
    public void Compute_BuildsGoalBalancesAndFinancialGoalSummary()
    {
        var playerId = Guid.NewGuid();
        var events = new List<EventDb>
        {
            CreateEvent(playerId, "saving.deposit.created", """{"goal_id":"goal-a","amount":20}"""),
            CreateEvent(playerId, "saving.deposit.created", """{"goal_id":"goal-a","amount":10}"""),
            CreateEvent(playerId, "saving.deposit.withdrawn", """{"goal_id":"goal-a","amount":5}"""),
            CreateEvent(playerId, "saving.goal.achieved", """{"goal_id":"goal-a","points":10,"cost":12}"""),
            CreateEvent(playerId, "saving.deposit.created", """{"goal_id":"goal-b","amount":7}"""),
            CreateEvent(playerId, "saving.deposit.withdrawn", """{"goal_id":"goal-b","amount":3}"""),
            CreateEvent(playerId, "saving.goal.achieved", """{"goal_id":"goal-c","points":5,"cost":3}""")
        };

        var result = AnalyticsSavingGoalCalculator.Compute(events);

        Assert.Equal(30, result.SavingDepositsByGoal["goal-a"]);
        Assert.Equal(7, result.SavingDepositsByGoal["goal-b"]);
        Assert.Equal(5, result.SavingWithdrawalsByGoal["goal-a"]);
        Assert.Equal(3, result.SavingWithdrawalsByGoal["goal-b"]);
        Assert.Equal(12, result.SavingGoalCostsByGoal["goal-a"]);
        Assert.Equal(3, result.SavingGoalCostsByGoal["goal-c"]);
        Assert.Contains("goal-a", result.SavingGoalsAchieved);
        Assert.Contains("goal-c", result.SavingGoalsAchieved);
        Assert.Equal(13, result.SavingBalancesByGoal["goal-a"]);
        Assert.Equal(4, result.SavingBalancesByGoal["goal-b"]);
        Assert.Equal(0, result.SavingBalancesByGoal["goal-c"]);
        Assert.Equal(17, result.CoinsSaved);
        Assert.Equal(3, result.FinancialGoalsAttempted);
        Assert.Equal(3, result.FinancialGoalsAvailableTotal);
        Assert.Equal(2, result.FinancialGoalsCompleted);
        Assert.Equal(37, result.FinancialGoalsCoinsTotalInvested);
        Assert.Equal(4, result.FinancialGoalsIncompleteCoinsWasted);
    }

    [Fact]
    public void Compute_HandlesMissingGoalActivity()
    {
        var result = AnalyticsSavingGoalCalculator.Compute(Array.Empty<EventDb>());

        Assert.Empty(result.SavingDepositsByGoal);
        Assert.Empty(result.SavingBalancesByGoal);
        Assert.Equal(0, result.CoinsSaved);
        Assert.Equal(0, result.FinancialGoalsAttempted);
        Assert.Null(result.FinancialGoalsAvailableTotal);
        Assert.Equal(0, result.FinancialGoalsCompleted);
        Assert.Equal(0, result.FinancialGoalsCoinsTotalInvested);
        Assert.Equal(0, result.FinancialGoalsIncompleteCoinsWasted);
    }

    private static EventDb CreateEvent(Guid playerId, string actionType, string payload)
    {
        return new EventDb
        {
            EventId = Guid.NewGuid(),
            SessionId = Guid.NewGuid(),
            PlayerId = playerId,
            ActorType = "PLAYER",
            Timestamp = new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero),
            DayIndex = 0,
            Weekday = "MON",
            TurnNumber = 1,
            SequenceNumber = 1,
            ActionType = actionType,
            RulesetVersionId = Guid.NewGuid(),
            Payload = payload
        };
    }
}
