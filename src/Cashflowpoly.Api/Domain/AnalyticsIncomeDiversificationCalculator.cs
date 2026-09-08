// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui AnalyticsIncomeDiversificationCalculator.
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Domain;

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `AnalyticsIncomeDiversificationMetrics`; sealed mencegah tipe ini
// diturunkan lagi.
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

// Mendefinisikan tipe class `IncomeDiversificationCalculator` yang mewarisi atau menerapkan `IIncomeDiversificationCalculator`; sealed mencegah
// tipe ini diturunkan lagi.
internal sealed class IncomeDiversificationCalculator : IIncomeDiversificationCalculator
// Membuka scope tipe IncomeDiversificationCalculator; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendeklarasikan field bertipe `AnalyticsPayloadReader`: `_payloadReader` menyimpan nilai payload pembaca dengan nilai awal objek baru dengan tipe
    // mengikuti konteks tujuan dan argumen (). readonly membatasi penggantian referensi/nilai field pada deklarasi atau konstruktor. static membuat
    // field menjadi milik tipe dan dibagikan antar instance.
    private static readonly AnalyticsPayloadReader _payloadReader = new();

    // Mendefinisikan metode `Compute` dengan hasil bertipe `AnalyticsIncomeDiversificationMetrics`; operasi ini menangani compute. Masukan: Parameter
    // `playerEvents` bertipe `IReadOnlyCollection<EventDb>` membawa nilai pemain event; Parameter `mealOrderIncomeTotal` bertipe `int` membawa nilai
    // meal urutan/pesanan pemasukan total; Parameter `goldInvestmentEarned` bertipe `int` membawa nilai emas investment earned.
    public AnalyticsIncomeDiversificationMetrics Compute(
        // Parameter `playerEvents` bertipe `IReadOnlyCollection<EventDb>` membawa nilai pemain event.
        IReadOnlyCollection<EventDb> playerEvents,
        // Parameter `mealOrderIncomeTotal` bertipe `int` membawa nilai meal urutan/pesanan pemasukan total.
        int mealOrderIncomeTotal,
        // Parameter `goldInvestmentEarned` bertipe `int` membawa nilai emas investment earned.
        int goldInvestmentEarned)
    // Membuka scope metode Compute; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Compute.
    {
        // Menyiapkan variabel lokal `freelanceIncome` untuk nilai freelance pemasukan dengan menjumlahkan nilai `playerEvents .Where(e => e.ActionType ==
        // ”KerjaLepas”) .Select(e => _payloadReader.TryReadAmount(e.Payload, out var amount) ? amount : 0)`. Tipe variabel disimpulkan dari ekspresi nilai
        // awal.
        var freelanceIncome = playerEvents
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(e => e.ActionType == ”KerjaLepas”) dalam Compute; token pada baris ini
            // menyambungkan bagian kode sebelum dan sesudahnya.
            .Where(e => e.ActionType == "KerjaLepas")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Select(e => _payloadReader.TryReadAmount(e.Payload, out var amount) ? amount :
            // 0) dalam Compute; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Select(e => _payloadReader.TryReadAmount(e.Payload, out var amount) ? amount : 0)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Sum(); dalam Compute; token pada baris ini menyambungkan bagian kode sebelum
            // dan sesudahnya.
            .Sum();
        // Menyiapkan variabel lokal `mealIncome` untuk nilai meal pemasukan dengan hasil konversi `mealOrderIncomeTotal` menjadi tipe `double`. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var mealIncome = (double)mealOrderIncomeTotal;
        // Menyiapkan variabel lokal `goldIncome` untuk nilai emas pemasukan dengan hasil konversi `goldInvestmentEarned` menjadi tipe `double`. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var goldIncome = (double)goldInvestmentEarned;

        // Menyiapkan variabel lokal `incomeSourceTotals` untuk nilai pemasukan source totals dengan objek baru bertipe `Dictionary<string, double>` dengan
        // argumen (StringComparer.OrdinalIgnoreCase). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var incomeSourceTotals = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Compute.
        {
            // Memperbarui `[”freelance_income”]` menggunakan `freelanceIncome` (nilai freelance pemasukan) dalam Compute.
            ["freelance_income"] = freelanceIncome,
            // Memperbarui `[”meal_order_income”]` menggunakan `mealIncome` (nilai meal pemasukan) dalam Compute.
            ["meal_order_income"] = mealIncome,
            // Memperbarui `[”gold_sale_income”]` menggunakan `goldIncome` (nilai emas pemasukan) dalam Compute.
            ["gold_sale_income"] = goldIncome
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam Compute.
        };

        // Menyiapkan variabel lokal `incomeShares` untuk nilai pemasukan shares dengan objek baru bertipe `Dictionary<string, double>` dengan argumen
        // (StringComparer.OrdinalIgnoreCase). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var incomeShares = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
        // Menyiapkan variabel lokal `diversifiableIncome` untuk nilai diversifiable pemasukan dengan menjumlahkan nilai `incomeSourceTotals.Values`. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var diversifiableIncome = incomeSourceTotals.Values.Sum();
        // Memeriksa pemeriksaan lebih besar antara `diversifiableIncome` dan `0`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam Compute.
        if (diversifiableIncome > 0)
        // Membuka scope cabang if untuk kondisi `diversifiableIncome > 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Compute.
        {
            // Melengkapi struktur ekspresi ForEachVariableStatement melalui foreach (var (source, value) in incomeSourceTotals) dalam Compute; token pada baris
            // ini menyambungkan bagian kode sebelum dan sesudahnya.
            foreach (var (source, value) in incomeSourceTotals)
            // Membuka scope blok ForEachVariableStatement; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Compute.
            {
                // Memeriksa pemeriksaan lebih kecil atau sama antara `value` dan `0`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam Compute.
                if (value <= 0)
                // Membuka scope cabang if untuk kondisi `value <= 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Compute.
                {
                    // Melewati sisa pernyataan pada iterasi saat ini dan melanjutkan ke elemen/iterasi berikutnya dalam Compute.
                    continue;
                // Menutup scope cabang if untuk kondisi `value <= 0`; bagian berikut berada di luar batas blok tersebut dalam Compute.
                }

                // Memperbarui `incomeShares[source]` menggunakan pembagian antara `value` dan `diversifiableIncome` dalam Compute.
                incomeShares[source] = value / diversifiableIncome;
            // Menutup scope blok ForEachVariableStatement; bagian berikut berada di luar batas blok tersebut dalam Compute.
            }
        // Menutup scope cabang if untuk kondisi `diversifiableIncome > 0`; bagian berikut berada di luar batas blok tersebut dalam Compute.
        }

        // Menyiapkan variabel lokal `activeIncomeSourceCount` untuk nilai aktif pemasukan source jumlah dengan `incomeShares.Count`, yaitu jumlah elemen
        // atau panjang data. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var activeIncomeSourceCount = incomeShares.Count;
        // Menyiapkan variabel lokal `incomeDiversificationIndex` untuk nilai pemasukan diversification index dengan null, yaitu penanda tidak ada nilai.
        // Tipe yang dipakai adalah `double?`.
        double? incomeDiversificationIndex = null;
        // Memeriksa pemeriksaan lebih besar antara `diversifiableIncome` dan `0`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam Compute.
        if (diversifiableIncome > 0)
        // Membuka scope cabang if untuk kondisi `diversifiableIncome > 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Compute.
        {
            // Memeriksa pemeriksaan lebih kecil atau sama antara `activeIncomeSourceCount` dan `1`; blok if hanya dijalankan ketika kondisi ini bernilai benar
            // dalam Compute.
            if (activeIncomeSourceCount <= 1)
            // Membuka scope cabang if untuk kondisi `activeIncomeSourceCount <= 1`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Compute.
            {
                // Memperbarui `incomeDiversificationIndex` menggunakan nilai literal `0` dalam Compute.
                incomeDiversificationIndex = 0;
            // Menutup scope cabang if untuk kondisi `activeIncomeSourceCount <= 1`; bagian berikut berada di luar batas blok tersebut dalam Compute.
            }
            // Menjalankan cabang alternatif ketika kondisi if sebelumnya tidak terpenuhi dalam Compute.
            else
            // Membuka scope cabang else; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Compute.
            {
                // Menyiapkan variabel lokal `concentration` untuk nilai concentration dengan menjumlahkan nilai `incomeShares.Values` berdasarkan `share => share *
                // share`. Tipe variabel disimpulkan dari ekspresi nilai awal.
                var concentration = incomeShares.Values.Sum(share => share * share);
                // Menyiapkan variabel lokal `normalizationDenominator` untuk nilai normalization denominator dengan selisih antara `1` dan `(1d /
                // activeIncomeSourceCount)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
                var normalizationDenominator = 1 - (1d / activeIncomeSourceCount);
                // Memperbarui `incomeDiversificationIndex` menggunakan hasil pemilihan bersyarat: ketika `normalizationDenominator <= 0` benar gunakan `0`, jika
                // tidak gunakan `((1 - concentration) / normalizationDenominator) * 100` dalam Compute.
                incomeDiversificationIndex = normalizationDenominator <= 0
                    // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: 0 dalam Compute.
                    ? 0
                    // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: ((1 - concentration) / normalizationDenominator) * 100; dalam Compute.
                    : ((1 - concentration) / normalizationDenominator) * 100;
            // Menutup scope cabang else; bagian berikut berada di luar batas blok tersebut dalam Compute.
            }
        // Menutup scope cabang if untuk kondisi `diversifiableIncome > 0`; bagian berikut berada di luar batas blok tersebut dalam Compute.
        }
        // Mengembalikan objek baru bertipe `AnalyticsIncomeDiversificationMetrics` dengan argumen ( freelanceIncome, mealIncome, goldIncome,
        // activeIncomeSourceCount, incomeShares, incomeDiversificationIndex, diversifiableIncome <= 0) kepada pemanggil dalam Compute; eksekusi jalur ini
        // selesai setelah nilai hasil ditentukan.
        return new AnalyticsIncomeDiversificationMetrics(
            // Meneruskan `freelanceIncome` (nilai freelance pemasukan) sebagai argumen ke konstruktor `AnalyticsIncomeDiversificationMetrics`.
            freelanceIncome,
            // Meneruskan `mealIncome` (nilai meal pemasukan) sebagai argumen ke konstruktor `AnalyticsIncomeDiversificationMetrics`.
            mealIncome,
            // Meneruskan `goldIncome` (nilai emas pemasukan) sebagai argumen ke konstruktor `AnalyticsIncomeDiversificationMetrics`.
            goldIncome,
            // Meneruskan `activeIncomeSourceCount` (nilai aktif pemasukan source jumlah) sebagai argumen ke konstruktor
            // `AnalyticsIncomeDiversificationMetrics`.
            activeIncomeSourceCount,
            // Meneruskan `incomeShares` (nilai pemasukan shares) sebagai argumen ke konstruktor `AnalyticsIncomeDiversificationMetrics`.
            incomeShares,
            // Meneruskan `incomeDiversificationIndex` (nilai pemasukan diversification index) sebagai argumen ke konstruktor
            // `AnalyticsIncomeDiversificationMetrics`.
            incomeDiversificationIndex,
            // Meneruskan pemeriksaan lebih kecil atau sama antara `diversifiableIncome` dan `0` sebagai argumen ke konstruktor
            // `AnalyticsIncomeDiversificationMetrics`.
            diversifiableIncome <= 0);
    // Menutup scope metode Compute; bagian berikut berada di luar batas blok tersebut dalam Compute.
    }
// Menutup scope tipe IncomeDiversificationCalculator; bagian berikut berada di luar batas blok tersebut.
}
