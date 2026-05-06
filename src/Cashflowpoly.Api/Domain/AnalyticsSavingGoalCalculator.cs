// Fungsi file: Menghitung state tabungan dan ringkasan financial goals dari histori event gameplay.
using Cashflowpoly.Api.Data;
using static Cashflowpoly.Api.Domain.AnalyticsPayloadReader;

namespace Cashflowpoly.Api.Domain;

internal sealed record AnalyticsSavingGoalMetrics(
    IReadOnlyDictionary<string, int> SavingDepositsByGoal,
    IReadOnlyDictionary<string, int> SavingWithdrawalsByGoal,
    IReadOnlyDictionary<string, int> SavingGoalCostsByGoal,
    IReadOnlySet<string> SavingGoalsAchieved,
    IReadOnlyDictionary<string, int> SavingBalancesByGoal,
    int CoinsSaved,
    int FinancialGoalsAttempted,
    int? FinancialGoalsAvailableTotal,
    int FinancialGoalsCompleted,
    int FinancialGoalsCoinsTotalInvested,
    int FinancialGoalsIncompleteCoinsWasted);

internal static class AnalyticsSavingGoalCalculator
{
    internal static AnalyticsSavingGoalMetrics Compute(IEnumerable<EventDb> playerEvents)
    {
        var savingDepositsByGoal = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var savingWithdrawalsByGoal = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var savingGoalCostsByGoal = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var savingGoalsAchieved = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var evt in playerEvents)
        {
            if (evt.ActionType == "saving.deposit.created" &&
                TryReadSavingDeposit(evt.Payload, out var goalId, out var amount))
            {
                savingDepositsByGoal[goalId] = savingDepositsByGoal.TryGetValue(goalId, out var existing)
                    ? existing + amount
                    : amount;
            }

            if (evt.ActionType == "saving.deposit.withdrawn" &&
                TryReadSavingDeposit(evt.Payload, out var withdrawGoalId, out var withdrawAmount))
            {
                savingWithdrawalsByGoal[withdrawGoalId] = savingWithdrawalsByGoal.TryGetValue(withdrawGoalId, out var existing)
                    ? existing + withdrawAmount
                    : withdrawAmount;
            }

            if (evt.ActionType == "saving.goal.achieved" &&
                TryReadSavingGoalAchievedDetailed(evt.Payload, out var achievedGoalId, out _, out var cost))
            {
                savingGoalsAchieved.Add(achievedGoalId);
                savingGoalCostsByGoal[achievedGoalId] = savingGoalCostsByGoal.TryGetValue(achievedGoalId, out var existing)
                    ? existing + cost
                    : cost;
            }
        }

        var savingBalancesByGoal = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        foreach (var goalId in savingDepositsByGoal.Keys
                     .Concat(savingWithdrawalsByGoal.Keys)
                     .Concat(savingGoalCostsByGoal.Keys)
                     .Distinct(StringComparer.OrdinalIgnoreCase))
        {
            var deposits = savingDepositsByGoal.TryGetValue(goalId, out var dep) ? dep : 0;
            var withdraws = savingWithdrawalsByGoal.TryGetValue(goalId, out var wit) ? wit : 0;
            var costs = savingGoalCostsByGoal.TryGetValue(goalId, out var cst) ? cst : 0;
            savingBalancesByGoal[goalId] = Math.Max(0, deposits - withdraws - costs);
        }

        var goalIds = new HashSet<string>(savingDepositsByGoal.Keys, StringComparer.OrdinalIgnoreCase);
        goalIds.UnionWith(savingGoalsAchieved);

        var financialGoalsAttempted = goalIds.Count;
        var financialGoalsAvailableTotal = goalIds.Count > 0 ? goalIds.Count : (int?)null;
        var financialGoalsIncompleteCoinsWasted = savingBalancesByGoal
            .Where(kvp => !savingGoalsAchieved.Contains(kvp.Key))
            .Sum(kvp => kvp.Value);

        return new AnalyticsSavingGoalMetrics(
            savingDepositsByGoal,
            savingWithdrawalsByGoal,
            savingGoalCostsByGoal,
            savingGoalsAchieved,
            savingBalancesByGoal,
            savingBalancesByGoal.Values.Sum(),
            financialGoalsAttempted,
            financialGoalsAvailableTotal,
            savingGoalsAchieved.Count,
            savingDepositsByGoal.Values.Sum(),
            financialGoalsIncompleteCoinsWasted);
    }
}
