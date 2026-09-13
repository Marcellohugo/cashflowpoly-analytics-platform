// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui AnalyticsIncomeDiversificationCalculator.
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Domain;

public sealed record AnalyticsIncomeDiversificationMetrics(
    // Parameter `FreelanceIncome` bertipe `double` membawa nilai freelance pemasukan.
    double FreelanceIncome,
    // Parameter `MealIncome` bertipe `double` membawa nilai meal pemasukan.
    double MealIncome,
    // Parameter `GoldIncome` bertipe `double` membawa nilai emas pemasukan.
    double GoldIncome,
    // Parameter `ActiveIncomeSourceCount` bertipe `int` membawa nilai aktif pemasukan source jumlah.
    int ActiveIncomeSourceCount,
    // Parameter `IncomeShares` bertipe `IReadOnlyDictionary<string, double>` membawa nilai pemasukan shares.
    IReadOnlyDictionary<string, double> IncomeShares,
    // Parameter `IncomeDiversificationIndex` bertipe `double?` membawa nilai pemasukan diversification index; nilai null diizinkan ketika data opsional
    // belum tersedia.
    double? IncomeDiversificationIndex,
    // Parameter `RequiresIncomeNote` bertipe `bool` membawa nilai requires pemasukan note.
    bool RequiresIncomeNote);

internal sealed class IncomeDiversificationCalculator : IIncomeDiversificationCalculator
{
    private static readonly AnalyticsPayloadReader _payloadReader = new();

    public AnalyticsIncomeDiversificationMetrics Compute(
        // Parameter `playerEvents` bertipe `IReadOnlyCollection<EventDb>` membawa nilai pemain event.
        IReadOnlyCollection<EventDb> playerEvents,
        // Parameter `mealOrderIncomeTotal` bertipe `int` membawa nilai meal urutan/pesanan pemasukan total.
        int mealOrderIncomeTotal,
        // Parameter `goldInvestmentEarned` bertipe `int` membawa nilai emas investment earned.
        int goldInvestmentEarned)
    {
        var freelanceIncome = playerEvents
            .Where(e => e.ActionType == "KerjaLepas")
            .Select(e => _payloadReader.TryReadAmount(e.Payload, out var amount) ? amount : 0)
            .Sum();
        var mealIncome = (double)mealOrderIncomeTotal;
        var goldIncome = (double)goldInvestmentEarned;

        var incomeSourceTotals = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
        {
            ["freelance_income"] = freelanceIncome,
            ["meal_order_income"] = mealIncome,
            ["gold_sale_income"] = goldIncome
        };

        var incomeShares = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
        var diversifiableIncome = incomeSourceTotals.Values.Sum();
        if (diversifiableIncome > 0)
        {
            foreach (var (source, value) in incomeSourceTotals)
            {
                if (value <= 0)
                {
                    // Melewati sisa pernyataan pada iterasi saat ini dan melanjutkan ke elemen/iterasi berikutnya dalam Compute.
                    continue;
                }

                incomeShares[source] = value / diversifiableIncome;
            }
        }

        var activeIncomeSourceCount = incomeShares.Count;
        double? incomeDiversificationIndex = null;
        if (diversifiableIncome > 0)
        {
            if (activeIncomeSourceCount <= 1)
            {
                incomeDiversificationIndex = 0;
            }
            else
            {
                var concentration = incomeShares.Values.Sum(share => share * share);
                var normalizationDenominator = 1 - (1d / activeIncomeSourceCount);
                incomeDiversificationIndex = normalizationDenominator <= 0
                    // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: 0 dalam Compute.
                    ? 0
                    // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: ((1 - concentration) / normalizationDenominator) * 100; dalam Compute.
                    : ((1 - concentration) / normalizationDenominator) * 100;
            }
        }
        return new AnalyticsIncomeDiversificationMetrics(
            freelanceIncome,
            mealIncome,
            goldIncome,
            activeIncomeSourceCount,
            incomeShares,
            incomeDiversificationIndex,
            diversifiableIncome <= 0);
    }
}
