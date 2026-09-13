// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui AnalyticsSavingGoalCalculator.
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Domain;

public sealed record AnalyticsSavingGoalMetrics(
    // Parameter `SavingDepositsByGoal` bertipe `IReadOnlyDictionary<string, int>` membawa nilai tabungan deposits berdasarkan target.
    IReadOnlyDictionary<string, int> SavingDepositsByGoal,
    // Parameter `SavingWithdrawalsByGoal` bertipe `IReadOnlyDictionary<string, int>` membawa nilai tabungan withdrawals berdasarkan target.
    IReadOnlyDictionary<string, int> SavingWithdrawalsByGoal,
    // Parameter `SavingGoalCostsByGoal` bertipe `IReadOnlyDictionary<string, int>` membawa nilai tabungan target costs berdasarkan target.
    IReadOnlyDictionary<string, int> SavingGoalCostsByGoal,
    // Parameter `SavingGoalsAchieved` bertipe `IReadOnlySet<string>` membawa nilai tabungan target achieved.
    IReadOnlySet<string> SavingGoalsAchieved,
    // Parameter `SavingBalancesByGoal` bertipe `IReadOnlyDictionary<string, int>` membawa nilai tabungan balances berdasarkan target.
    IReadOnlyDictionary<string, int> SavingBalancesByGoal,
    // Parameter `CoinsSaved` bertipe `int` membawa nilai coins saved.
    int CoinsSaved,
    // Parameter `FinancialGoalsAttempted` bertipe `int` membawa nilai keuangan target attempted.
    int FinancialGoalsAttempted,
    // Parameter `FinancialGoalsAvailableTotal` bertipe `int?` membawa nilai keuangan target tersedia total; nilai null diizinkan ketika data opsional
    // belum tersedia.
    int? FinancialGoalsAvailableTotal,
    // Parameter `FinancialGoalsCompleted` bertipe `int` membawa nilai keuangan target selesai.
    int FinancialGoalsCompleted,
    // Parameter `FinancialGoalsCoinsTotalInvested` bertipe `int` membawa nilai keuangan target coins total invested.
    int FinancialGoalsCoinsTotalInvested,
    // Parameter `FinancialGoalsIncompleteCoinsWasted` bertipe `int` membawa nilai keuangan target incomplete coins wasted.
    int FinancialGoalsIncompleteCoinsWasted);

internal sealed class SavingGoalCalculator : ISavingGoalCalculator
{
    private static readonly AnalyticsPayloadReader _payloadReader = new();

    public AnalyticsSavingGoalMetrics Compute(IEnumerable<EventDb> playerEvents, int? availableGoalCount = null)
    {
        var savingDepositsByGoal = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var savingWithdrawalsByGoal = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var savingGoalCostsByGoal = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var savingGoalsAchieved = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        // Mengulangi setiap elemen `playerEvents`; elemen saat ini disimpan sebagai `evt` bertipe `var` untuk diproses oleh badan loop dalam Compute.
        foreach (var evt in playerEvents)
        {
            if (evt.ActionType == "Menabung" &&
                _payloadReader.TryReadSavingDeposit(evt.Payload, out var goalId, out var amount))
            {
                savingDepositsByGoal[goalId] = savingDepositsByGoal.TryGetValue(goalId, out var existing)
                    // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: existing + amount dalam Compute.
                    ? existing + amount
                    // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: amount; dalam Compute.
                    : amount;
            }

            if (evt.ActionType == "TarikTabungan" &&
                _payloadReader.TryReadSavingDeposit(evt.Payload, out var withdrawGoalId, out var withdrawAmount))
            {
                savingWithdrawalsByGoal[withdrawGoalId] = savingWithdrawalsByGoal.TryGetValue(withdrawGoalId, out var existing)
                    // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: existing + withdrawAmount dalam Compute.
                    ? existing + withdrawAmount
                    // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: withdrawAmount; dalam Compute.
                    : withdrawAmount;
            }

            if (evt.ActionType == "TujuanFinansial" &&
                _payloadReader.TryReadSavingGoalAchievedDetailed(evt.Payload, out var achievedGoalId, out _, out var cost))
            {
                savingGoalsAchieved.Add(achievedGoalId);
                savingGoalCostsByGoal[achievedGoalId] = savingGoalCostsByGoal.TryGetValue(achievedGoalId, out var existing)
                    // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: existing + cost dalam Compute.
                    ? existing + cost
                    // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: cost; dalam Compute.
                    : cost;
            }
        }

        var savingBalancesByGoal = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        // Mengulangi setiap elemen `savingDepositsByGoal.Keys .Concat(savingWithdrawalsByGoal.Keys) .Concat(savingGoalCostsByGoal.Keys)
        // .Distinct(StringComparer.OrdinalIgnoreCase)`; elemen saat ini disimpan sebagai `goalId` bertipe `var` untuk diproses oleh badan loop dalam
        // Compute.
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
        var financialGoalsAvailableTotal = availableGoalCount is > 0
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: availableGoalCount dalam Compute.
            ? availableGoalCount
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: goalIds.Count > 0 dalam Compute.
            : goalIds.Count > 0
                // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: goalIds.Count dalam Compute.
                ? goalIds.Count
                // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: (int?)null; dalam Compute.
                : (int?)null;
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
