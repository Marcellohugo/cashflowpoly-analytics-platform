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

    public AnalyticsSavingGoalMetrics Compute(IEnumerable<EventDb> playerEvents, int? availableGoalCount = null, int initialSaving = 0)
    {
        var events = playerEvents.ToList();
        var savingGoalCostsByGoal = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var savingGoalsAchieved = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        // Mengulangi setiap elemen `playerEvents`; elemen saat ini disimpan sebagai `evt` bertipe `var` untuk diproses oleh badan loop dalam Compute.
        foreach (var evt in events)
        {
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

        // Pertahankan bentuk respons lama tanpa menyiratkan tabungan memesan tujuan tertentu.
        var unallocatedByGoal = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var financialGoalsAttempted = savingGoalsAchieved.Count;
        var financialGoalsAvailableTotal = availableGoalCount is >= 0
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: availableGoalCount dalam Compute.
            ? availableGoalCount
            // Tanpa katalog, hanya tujuan yang sudah dibeli dapat dihitung.
            : savingGoalsAchieved.Count > 0
                ? savingGoalsAchieved.Count
                // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: (int?)null; dalam Compute.
                : (int?)null;
        return new AnalyticsSavingGoalMetrics(
            unallocatedByGoal,
            unallocatedByGoal,
            savingGoalCostsByGoal,
            savingGoalsAchieved,
            unallocatedByGoal,
            EventDerivedStateCalculator.ComputeTotalSavings(events, initialSaving),
            financialGoalsAttempted,
            financialGoalsAvailableTotal,
            savingGoalsAchieved.Count,
            savingGoalCostsByGoal.Values.Sum(),
            0);
    }
}
