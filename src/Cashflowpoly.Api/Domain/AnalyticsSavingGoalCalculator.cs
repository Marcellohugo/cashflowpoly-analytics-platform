// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui AnalyticsSavingGoalCalculator.
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Domain;

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `AnalyticsSavingGoalMetrics`; sealed mencegah tipe ini diturunkan lagi.
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

// Mendefinisikan tipe class `SavingGoalCalculator` yang mewarisi atau menerapkan `ISavingGoalCalculator`; sealed mencegah tipe ini diturunkan lagi.
internal sealed class SavingGoalCalculator : ISavingGoalCalculator
// Membuka scope tipe SavingGoalCalculator; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendeklarasikan field bertipe `AnalyticsPayloadReader`: `_payloadReader` menyimpan nilai payload pembaca dengan nilai awal objek baru dengan tipe
    // mengikuti konteks tujuan dan argumen (). readonly membatasi penggantian referensi/nilai field pada deklarasi atau konstruktor. static membuat
    // field menjadi milik tipe dan dibagikan antar instance.
    private static readonly AnalyticsPayloadReader _payloadReader = new();

    // Mendefinisikan metode `Compute` dengan hasil bertipe `AnalyticsSavingGoalMetrics`; operasi ini menangani compute. Masukan: Parameter
    // `playerEvents` bertipe `IEnumerable<EventDb>` membawa nilai pemain event; Parameter `availableGoalCount` bertipe `int?` membawa nilai tersedia
    // target jumlah; nilai null diizinkan ketika data opsional belum tersedia; bila argumen tidak diberikan digunakan null, yaitu penanda tidak ada
    // nilai.
    public AnalyticsSavingGoalMetrics Compute(IEnumerable<EventDb> playerEvents, int? availableGoalCount = null)
    // Membuka scope metode Compute; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Compute.
    {
        // Menyiapkan variabel lokal `savingDepositsByGoal` untuk nilai tabungan deposits berdasarkan target dengan objek baru bertipe `Dictionary<string,
        // int>` dengan argumen (StringComparer.OrdinalIgnoreCase). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var savingDepositsByGoal = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        // Menyiapkan variabel lokal `savingWithdrawalsByGoal` untuk nilai tabungan withdrawals berdasarkan target dengan objek baru bertipe
        // `Dictionary<string, int>` dengan argumen (StringComparer.OrdinalIgnoreCase). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var savingWithdrawalsByGoal = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        // Menyiapkan variabel lokal `savingGoalCostsByGoal` untuk nilai tabungan target costs berdasarkan target dengan objek baru bertipe
        // `Dictionary<string, int>` dengan argumen (StringComparer.OrdinalIgnoreCase). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var savingGoalCostsByGoal = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        // Menyiapkan variabel lokal `savingGoalsAchieved` untuk nilai tabungan target achieved dengan objek baru bertipe `HashSet<string>` dengan argumen
        // (StringComparer.OrdinalIgnoreCase). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var savingGoalsAchieved = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        // Mengulangi setiap elemen `playerEvents`; elemen saat ini disimpan sebagai `evt` bertipe `var` untuk diproses oleh badan loop dalam Compute.
        foreach (var evt in playerEvents)
        // Membuka scope loop setiap evt dari `playerEvents`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Compute.
        {
            // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `evt.ActionType == ”Menabung”` dan
            // `_payloadReader.TryReadSavingDeposit(evt.Payload, out var goalId, out var amount)`; sisi kanan diperiksa hanya jika sisi kiri benar; blok if
            // hanya dijalankan ketika kondisi ini bernilai benar dalam Compute.
            if (evt.ActionType == "Menabung" &&
                // Melanjutkan pengolahan dengan memanggil `_payloadReader.TryReadSavingDeposit` dengan `evt.Payload`, `var goalId`, `var amount` dalam Compute.
                _payloadReader.TryReadSavingDeposit(evt.Payload, out var goalId, out var amount))
            // Membuka scope cabang if untuk kondisi `evt.ActionType == ”Menabung” && _payloadReader.TryReadSavingDeposit(evt.Payload, out var goalId, out var
            // amount)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Compute.
            {
                // Memperbarui `savingDepositsByGoal[goalId]` menggunakan hasil pemilihan bersyarat: ketika `savingDepositsByGoal.TryGetValue(goalId, out var
                // existing)` benar gunakan `existing + amount`, jika tidak gunakan `amount` dalam Compute.
                savingDepositsByGoal[goalId] = savingDepositsByGoal.TryGetValue(goalId, out var existing)
                    // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: existing + amount dalam Compute.
                    ? existing + amount
                    // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: amount; dalam Compute.
                    : amount;
            // Menutup scope cabang if untuk kondisi `evt.ActionType == ”Menabung” && _payloadReader.TryReadSavingDeposit(evt.Payload, out var goalId, out var
            // amount)`; bagian berikut berada di luar batas blok tersebut dalam Compute.
            }

            // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `evt.ActionType == ”TarikTabungan”` dan
            // `_payloadReader.TryReadSavingDeposit(evt.Payload, out var withdrawGoalId, out var withdrawAmount)`; sisi kanan diperiksa hanya jika sisi kiri
            // benar; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam Compute.
            if (evt.ActionType == "TarikTabungan" &&
                // Melanjutkan pengolahan dengan memanggil `_payloadReader.TryReadSavingDeposit` dengan `evt.Payload`, `var withdrawGoalId`, `var withdrawAmount`
                // dalam Compute.
                _payloadReader.TryReadSavingDeposit(evt.Payload, out var withdrawGoalId, out var withdrawAmount))
            // Membuka scope cabang if untuk kondisi `evt.ActionType == ”TarikTabungan” && _payloadReader.TryReadSavingDeposit(evt.Payload, out var
            // withdrawGoalId, out var withdrawAmount)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Compute.
            {
                // Memperbarui `savingWithdrawalsByGoal[withdrawGoalId]` menggunakan hasil pemilihan bersyarat: ketika
                // `savingWithdrawalsByGoal.TryGetValue(withdrawGoalId, out var existing)` benar gunakan `existing + withdrawAmount`, jika tidak gunakan
                // `withdrawAmount` dalam Compute.
                savingWithdrawalsByGoal[withdrawGoalId] = savingWithdrawalsByGoal.TryGetValue(withdrawGoalId, out var existing)
                    // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: existing + withdrawAmount dalam Compute.
                    ? existing + withdrawAmount
                    // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: withdrawAmount; dalam Compute.
                    : withdrawAmount;
            // Menutup scope cabang if untuk kondisi `evt.ActionType == ”TarikTabungan” && _payloadReader.TryReadSavingDeposit(evt.Payload, out var
            // withdrawGoalId, out var withdrawAmount)`; bagian berikut berada di luar batas blok tersebut dalam Compute.
            }

            // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `evt.ActionType == ”TujuanFinansial”` dan
            // `_payloadReader.TryReadSavingGoalAchievedDetailed(evt.Payload, out var achievedGoalId, out _, out var cost)`; sisi kanan diperiksa hanya jika
            // sisi kiri benar; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam Compute.
            if (evt.ActionType == "TujuanFinansial" &&
                // Melanjutkan pengolahan dengan memanggil `_payloadReader.TryReadSavingGoalAchievedDetailed` dengan `evt.Payload`, `var achievedGoalId`, `_`, `var
                // cost` dalam Compute.
                _payloadReader.TryReadSavingGoalAchievedDetailed(evt.Payload, out var achievedGoalId, out _, out var cost))
            // Membuka scope cabang if untuk kondisi `evt.ActionType == ”TujuanFinansial” && _payloadReader.TryReadSavingGoalAchievedDetailed(evt.Payload, out
            // var achievedGoalId, out _, out var cost)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Compute.
            {
                // Menjalankan menambahkan `achievedGoalId` ke `savingGoalsAchieved` dalam Compute.
                savingGoalsAchieved.Add(achievedGoalId);
                // Memperbarui `savingGoalCostsByGoal[achievedGoalId]` menggunakan hasil pemilihan bersyarat: ketika
                // `savingGoalCostsByGoal.TryGetValue(achievedGoalId, out var existing)` benar gunakan `existing + cost`, jika tidak gunakan `cost` dalam Compute.
                savingGoalCostsByGoal[achievedGoalId] = savingGoalCostsByGoal.TryGetValue(achievedGoalId, out var existing)
                    // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: existing + cost dalam Compute.
                    ? existing + cost
                    // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: cost; dalam Compute.
                    : cost;
            // Menutup scope cabang if untuk kondisi `evt.ActionType == ”TujuanFinansial” && _payloadReader.TryReadSavingGoalAchievedDetailed(evt.Payload, out
            // var achievedGoalId, out _, out var cost)`; bagian berikut berada di luar batas blok tersebut dalam Compute.
            }
        // Menutup scope loop setiap evt dari `playerEvents`; bagian berikut berada di luar batas blok tersebut dalam Compute.
        }

        // Menyiapkan variabel lokal `savingBalancesByGoal` untuk nilai tabungan balances berdasarkan target dengan objek baru bertipe `Dictionary<string,
        // int>` dengan argumen (StringComparer.OrdinalIgnoreCase). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var savingBalancesByGoal = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        // Mengulangi setiap elemen `savingDepositsByGoal.Keys .Concat(savingWithdrawalsByGoal.Keys) .Concat(savingGoalCostsByGoal.Keys)
        // .Distinct(StringComparer.OrdinalIgnoreCase)`; elemen saat ini disimpan sebagai `goalId` bertipe `var` untuk diproses oleh badan loop dalam
        // Compute.
        foreach (var goalId in savingDepositsByGoal.Keys
                     // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Concat(savingWithdrawalsByGoal.Keys) dalam Compute; token pada baris ini
                     // menyambungkan bagian kode sebelum dan sesudahnya.
                     .Concat(savingWithdrawalsByGoal.Keys)
                     // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Concat(savingGoalCostsByGoal.Keys) dalam Compute; token pada baris ini
                     // menyambungkan bagian kode sebelum dan sesudahnya.
                     .Concat(savingGoalCostsByGoal.Keys)
                     // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Distinct(StringComparer.OrdinalIgnoreCase)) dalam Compute; token pada baris
                     // ini menyambungkan bagian kode sebelum dan sesudahnya.
                     .Distinct(StringComparer.OrdinalIgnoreCase))
        // Membuka scope loop setiap goalId dari `savingDepositsByGoal.Keys .Concat(savingWithdrawalsByGoal.Keys) .Concat(savingGoalCostsByGoal.Keys)
        // .Distinct(StringComparer.OrdinalIgnoreCase)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Compute.
        {
            // Menyiapkan variabel lokal `deposits` untuk nilai deposits dengan hasil pemilihan bersyarat: ketika `savingDepositsByGoal.TryGetValue(goalId, out
            // var dep)` benar gunakan `dep`, jika tidak gunakan `0`. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var deposits = savingDepositsByGoal.TryGetValue(goalId, out var dep) ? dep : 0;
            // Menyiapkan variabel lokal `withdraws` untuk nilai withdraws dengan hasil pemilihan bersyarat: ketika `savingWithdrawalsByGoal.TryGetValue(goalId,
            // out var wit)` benar gunakan `wit`, jika tidak gunakan `0`. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var withdraws = savingWithdrawalsByGoal.TryGetValue(goalId, out var wit) ? wit : 0;
            // Menyiapkan variabel lokal `costs` untuk nilai costs dengan hasil pemilihan bersyarat: ketika `savingGoalCostsByGoal.TryGetValue(goalId, out var
            // cst)` benar gunakan `cst`, jika tidak gunakan `0`. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var costs = savingGoalCostsByGoal.TryGetValue(goalId, out var cst) ? cst : 0;
            // Memperbarui `savingBalancesByGoal[goalId]` menggunakan menentukan nilai terbesar dari `0`, `deposits - withdraws - costs` dalam Compute.
            savingBalancesByGoal[goalId] = Math.Max(0, deposits - withdraws - costs);
        // Menutup scope loop setiap goalId dari `savingDepositsByGoal.Keys .Concat(savingWithdrawalsByGoal.Keys) .Concat(savingGoalCostsByGoal.Keys)
        // .Distinct(StringComparer.OrdinalIgnoreCase)`; bagian berikut berada di luar batas blok tersebut dalam Compute.
        }

        // Menyiapkan variabel lokal `goalIds` untuk nilai target identitas dengan objek baru bertipe `HashSet<string>` dengan argumen
        // (savingDepositsByGoal.Keys, StringComparer.OrdinalIgnoreCase). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var goalIds = new HashSet<string>(savingDepositsByGoal.Keys, StringComparer.OrdinalIgnoreCase);
        // Menjalankan memanggil `goalIds.UnionWith` dengan `savingGoalsAchieved` dalam Compute.
        goalIds.UnionWith(savingGoalsAchieved);

        // Menyiapkan variabel lokal `financialGoalsAttempted` untuk nilai keuangan target attempted dengan `goalIds.Count`, yaitu jumlah elemen atau
        // panjang data. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var financialGoalsAttempted = goalIds.Count;
        // Menyiapkan variabel lokal `financialGoalsAvailableTotal` untuk nilai keuangan target tersedia total dengan hasil pemilihan bersyarat: ketika
        // `availableGoalCount is > 0` benar gunakan `availableGoalCount`, jika tidak gunakan `goalIds.Count > 0 ? goalIds.Count : (int?)null`. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var financialGoalsAvailableTotal = availableGoalCount is > 0
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: availableGoalCount dalam Compute.
            ? availableGoalCount
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: goalIds.Count > 0 dalam Compute.
            : goalIds.Count > 0
                // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: goalIds.Count dalam Compute.
                ? goalIds.Count
                // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: (int?)null; dalam Compute.
                : (int?)null;
        // Menyiapkan variabel lokal `financialGoalsIncompleteCoinsWasted` untuk nilai keuangan target incomplete coins wasted dengan menjumlahkan nilai
        // `savingBalancesByGoal .Where(kvp => !savingGoalsAchieved.Contains(kvp.Key))` berdasarkan `kvp => kvp.Value`. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var financialGoalsIncompleteCoinsWasted = savingBalancesByGoal
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(kvp => !savingGoalsAchieved.Contains(kvp.Key)) dalam Compute; token pada
            // baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Where(kvp => !savingGoalsAchieved.Contains(kvp.Key))
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Sum(kvp => kvp.Value); dalam Compute; token pada baris ini menyambungkan
            // bagian kode sebelum dan sesudahnya.
            .Sum(kvp => kvp.Value);

        // Mengembalikan objek baru bertipe `AnalyticsSavingGoalMetrics` dengan argumen ( savingDepositsByGoal, savingWithdrawalsByGoal,
        // savingGoalCostsByGoal, savingGoalsAchieved, savingBalancesByGoal, savingBalancesByGoal.Values.Sum(), financialG... kepada pemanggil dalam
        // Compute; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return new AnalyticsSavingGoalMetrics(
            // Meneruskan `savingDepositsByGoal` (nilai tabungan deposits berdasarkan target) sebagai argumen ke konstruktor `AnalyticsSavingGoalMetrics`.
            savingDepositsByGoal,
            // Meneruskan `savingWithdrawalsByGoal` (nilai tabungan withdrawals berdasarkan target) sebagai argumen ke konstruktor `AnalyticsSavingGoalMetrics`.
            savingWithdrawalsByGoal,
            // Meneruskan `savingGoalCostsByGoal` (nilai tabungan target costs berdasarkan target) sebagai argumen ke konstruktor `AnalyticsSavingGoalMetrics`.
            savingGoalCostsByGoal,
            // Meneruskan `savingGoalsAchieved` (nilai tabungan target achieved) sebagai argumen ke konstruktor `AnalyticsSavingGoalMetrics`.
            savingGoalsAchieved,
            // Meneruskan `savingBalancesByGoal` (nilai tabungan balances berdasarkan target) sebagai argumen ke konstruktor `AnalyticsSavingGoalMetrics`.
            savingBalancesByGoal,
            // Meneruskan menjumlahkan nilai `savingBalancesByGoal.Values` sebagai argumen ke konstruktor `AnalyticsSavingGoalMetrics`.
            savingBalancesByGoal.Values.Sum(),
            // Meneruskan `financialGoalsAttempted` (nilai keuangan target attempted) sebagai argumen ke konstruktor `AnalyticsSavingGoalMetrics`.
            financialGoalsAttempted,
            // Meneruskan `financialGoalsAvailableTotal` (nilai keuangan target tersedia total) sebagai argumen ke konstruktor `AnalyticsSavingGoalMetrics`.
            financialGoalsAvailableTotal,
            // Meneruskan `savingGoalsAchieved.Count`, yaitu jumlah elemen atau panjang data sebagai argumen ke konstruktor `AnalyticsSavingGoalMetrics`.
            savingGoalsAchieved.Count,
            // Meneruskan menjumlahkan nilai `savingDepositsByGoal.Values` sebagai argumen ke konstruktor `AnalyticsSavingGoalMetrics`.
            savingDepositsByGoal.Values.Sum(),
            // Meneruskan `financialGoalsIncompleteCoinsWasted` (nilai keuangan target incomplete coins wasted) sebagai argumen ke konstruktor
            // `AnalyticsSavingGoalMetrics`.
            financialGoalsIncompleteCoinsWasted);
    // Menutup scope metode Compute; bagian berikut berada di luar batas blok tersebut dalam Compute.
    }
// Menutup scope tipe SavingGoalCalculator; bagian berikut berada di luar batas blok tersebut.
}
