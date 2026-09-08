// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui AnalyticsGoldGameplayCalculator.
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Domain;

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `AnalyticsGoldGameplayMetrics`; sealed mencegah tipe ini diturunkan
// lagi.
public sealed record AnalyticsGoldGameplayMetrics(
    // Parameter `InitialGoldQty` bertipe `int` membawa nilai awal emas qty.
    int InitialGoldQty,
    // Parameter `GoldBuyQty` bertipe `int` membawa nilai emas buy qty.
    int GoldBuyQty,
    // Parameter `GoldSellQty` bertipe `int` membawa nilai emas sell qty.
    int GoldSellQty,
    // Parameter `GoldHeldEnd` bertipe `int` membawa nilai emas held end.
    int GoldHeldEnd,
    // Parameter `GoldPurchasePrices` bertipe `IReadOnlyList<int>` membawa nilai emas pembelian prices.
    IReadOnlyList<int> GoldPurchasePrices,
    // Parameter `GoldSalePrices` bertipe `IReadOnlyList<int>` membawa nilai emas penjualan prices.
    IReadOnlyList<int> GoldSalePrices,
    // Parameter `GoldInvestmentSpent` bertipe `int` membawa nilai emas investment spent.
    int GoldInvestmentSpent,
    // Parameter `GoldInvestmentEarned` bertipe `int` membawa nilai emas investment earned.
    int GoldInvestmentEarned,
    // Parameter `GoldInvestmentNet` bertipe `int` membawa nilai emas investment net.
    int GoldInvestmentNet);

// Mendefinisikan tipe class `GoldGameplayCalculator` yang mewarisi atau menerapkan `IGoldGameplayCalculator`; sealed mencegah tipe ini diturunkan
// lagi.
internal sealed class GoldGameplayCalculator : IGoldGameplayCalculator
// Membuka scope tipe GoldGameplayCalculator; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendeklarasikan field bertipe `AnalyticsPayloadReader`: `_payloadReader` menyimpan nilai payload pembaca dengan nilai awal objek baru dengan tipe
    // mengikuti konteks tujuan dan argumen (). readonly membatasi penggantian referensi/nilai field pada deklarasi atau konstruktor. static membuat
    // field menjadi milik tipe dan dibagikan antar instance.
    private static readonly AnalyticsPayloadReader _payloadReader = new();

    // Mendefinisikan metode `Compute` dengan hasil bertipe `AnalyticsGoldGameplayMetrics`; operasi ini menangani compute. Masukan: Parameter
    // `playerEvents` bertipe `IEnumerable<EventDb>` membawa nilai pemain event.
    public AnalyticsGoldGameplayMetrics Compute(IEnumerable<EventDb> playerEvents)
    // Membuka scope metode Compute; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Compute.
    {
        // Menyiapkan variabel lokal `goldBuyQty` untuk nilai emas buy qty dengan nilai literal `0`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var goldBuyQty = 0;
        // Menyiapkan variabel lokal `goldSellQty` untuk nilai emas sell qty dengan nilai literal `0`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var goldSellQty = 0;
        // Menyiapkan variabel lokal `goldPurchasePrices` untuk nilai emas pembelian prices dengan objek baru bertipe `List<int>` dengan nilai awal sesuai
        // konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var goldPurchasePrices = new List<int>();
        // Menyiapkan variabel lokal `goldSalePrices` untuk nilai emas penjualan prices dengan objek baru bertipe `List<int>` dengan nilai awal sesuai
        // konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var goldSalePrices = new List<int>();
        // Menyiapkan variabel lokal `goldInvestmentSpent` untuk nilai emas investment spent dengan nilai literal `0`. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var goldInvestmentSpent = 0;
        // Menyiapkan variabel lokal `goldInvestmentEarned` untuk nilai emas investment earned dengan nilai literal `0`. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var goldInvestmentEarned = 0;

        // Menyiapkan variabel lokal `initialGoldQty` untuk nilai awal emas qty dengan menjumlahkan nilai `playerEvents .Where(e => e.ActionType ==
        // GameActionCatalog.SetupEmasAwal)` berdasarkan `e => TryReadInt32(e.Payload, ”qty”, out var qty) ? qty : 1`. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var initialGoldQty = playerEvents
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(e => e.ActionType == GameActionCatalog.SetupEmasAwal) dalam Compute;
            // token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Where(e => e.ActionType == GameActionCatalog.SetupEmasAwal)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Sum(e => TryReadInt32(e.Payload, ”qty”, out var qty) ? qty : 1); dalam
            // Compute; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Sum(e => TryReadInt32(e.Payload, "qty", out var qty) ? qty : 1);

        // Mengulangi setiap elemen `playerEvents.Where(e => e.ActionType == GameActionCatalog.InvestasiEmas || e.ActionType == GameActionCatalog.JualEmas
        // || (e.ActionType == GameActionCatalog.RiskEmergencyUsed &...`; elemen saat ini disimpan sebagai `evt` bertipe `var` untuk diproses oleh badan
        // loop dalam Compute.
        foreach (var evt in playerEvents.Where(e =>
                     // Meneruskan fungsi lambda `e => e.ActionType == GameActionCatalog.InvestasiEmas || e.ActionType == GameActionCatalog.JualEmas || (e.ActionType ==
                     // GameActionCatalog.RiskEmergencyUsed && TryReadString(e.P...` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai
                     // argumen ke `playerEvents.Where`.
                     e.ActionType == GameActionCatalog.InvestasiEmas ||
                     // Meneruskan fungsi lambda `e => e.ActionType == GameActionCatalog.InvestasiEmas || e.ActionType == GameActionCatalog.JualEmas || (e.ActionType ==
                     // GameActionCatalog.RiskEmergencyUsed && TryReadString(e.P...` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai
                     // argumen ke `playerEvents.Where`.
                     e.ActionType == GameActionCatalog.JualEmas ||
                     // Meneruskan fungsi lambda `e => e.ActionType == GameActionCatalog.InvestasiEmas || e.ActionType == GameActionCatalog.JualEmas || (e.ActionType ==
                     // GameActionCatalog.RiskEmergencyUsed && TryReadString(e.P...` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai
                     // argumen ke `playerEvents.Where`.
                     (e.ActionType == GameActionCatalog.RiskEmergencyUsed &&
                      // Meneruskan `e.Payload` (muatan detail event dalam format JSON) sebagai argumen ke `TryReadString`; Meneruskan nilai literal `”option_type”`
                      // sebagai argumen ke `TryReadString`; Meneruskan `var optionType` sebagai argumen ke `TryReadString`.
                      TryReadString(e.Payload, "option_type", out var optionType) &&
                      // Meneruskan nilai literal `”SELL_GOLD”` sebagai argumen ke `optionType.Equals`; Meneruskan `StringComparison.OrdinalIgnoreCase` (nilai ordinal
                      // ignore case) sebagai argumen ke `optionType.Equals`.
                      optionType.Equals("SELL_GOLD", StringComparison.OrdinalIgnoreCase))))
        // Membuka scope loop setiap evt dari `playerEvents.Where(e => e.ActionType == GameActionCatalog.InvestasiEmas || e.ActionType ==
        // GameActionCatalog.JualEmas || (e.ActionType == GameActionCatalog.RiskEmergencyUsed &...`; pernyataan/deklarasi berikut berada di dalam batas blok
        // ini dalam Compute.
        {
            // Memeriksa perbandingan kesamaan antara `evt.ActionType` dan `GameActionCatalog.RiskEmergencyUsed`; blok if hanya dijalankan ketika kondisi ini
            // bernilai benar dalam Compute.
            if (evt.ActionType == GameActionCatalog.RiskEmergencyUsed)
            // Membuka scope cabang if untuk kondisi `evt.ActionType == GameActionCatalog.RiskEmergencyUsed`; pernyataan/deklarasi berikut berada di dalam batas
            // blok ini dalam Compute.
            {
                // Memeriksa kebalikan kondisi `TryReadInt32(evt.Payload, ”qty”, out var emergencyQty)`; blok if hanya dijalankan ketika kondisi ini bernilai benar
                // dalam Compute.
                if (!TryReadInt32(evt.Payload, "qty", out var emergencyQty))
                // Membuka scope cabang if untuk kondisi `!TryReadInt32(evt.Payload, ”qty”, out var emergencyQty)`; pernyataan/deklarasi berikut berada di dalam
                // batas blok ini dalam Compute.
                {
                    // Melewati sisa pernyataan pada iterasi saat ini dan melanjutkan ke elemen/iterasi berikutnya dalam Compute.
                    continue;
                // Menutup scope cabang if untuk kondisi `!TryReadInt32(evt.Payload, ”qty”, out var emergencyQty)`; bagian berikut berada di luar batas blok
                // tersebut dalam Compute.
                }

                // Menjalankan memanggil `TryReadInt32` dengan `evt.Payload`, `”unit_price”`, `var emergencyUnitPrice` dalam Compute.
                TryReadInt32(evt.Payload, "unit_price", out var emergencyUnitPrice);
                // Menjalankan memanggil `TryReadInt32` dengan `evt.Payload`, `”amount”`, `var emergencyAmount` dalam Compute.
                TryReadInt32(evt.Payload, "amount", out var emergencyAmount);
                // Memperbarui `goldSellQty` dengan menambahkan `emergencyQty` (nilai emergency qty) dalam Compute.
                goldSellQty += emergencyQty;
                // Memperbarui `goldInvestmentEarned` dengan menambahkan `emergencyAmount` (nilai emergency nominal) dalam Compute.
                goldInvestmentEarned += emergencyAmount;
                // Memeriksa pemeriksaan lebih besar antara `emergencyUnitPrice` dan `0`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam Compute.
                if (emergencyUnitPrice > 0)
                // Membuka scope cabang if untuk kondisi `emergencyUnitPrice > 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Compute.
                {
                    // Menjalankan menambahkan `emergencyUnitPrice` ke `goldSalePrices` dalam Compute.
                    goldSalePrices.Add(emergencyUnitPrice);
                // Menutup scope cabang if untuk kondisi `emergencyUnitPrice > 0`; bagian berikut berada di luar batas blok tersebut dalam Compute.
                }
                // Melewati sisa pernyataan pada iterasi saat ini dan melanjutkan ke elemen/iterasi berikutnya dalam Compute.
                continue;
            // Menutup scope cabang if untuk kondisi `evt.ActionType == GameActionCatalog.RiskEmergencyUsed`; bagian berikut berada di luar batas blok tersebut
            // dalam Compute.
            }

            // Memeriksa kebalikan kondisi `_payloadReader.TryReadGoldTradeDetailed(evt.Payload, out var tradeType, out var qty, out var unitPrice, out var
            // amount)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam Compute.
            if (!_payloadReader.TryReadGoldTradeDetailed(evt.Payload, out var tradeType, out var qty, out var unitPrice, out var amount))
            // Membuka scope cabang if untuk kondisi `!_payloadReader.TryReadGoldTradeDetailed(evt.Payload, out var tradeType, out var qty, out var unitPrice,
            // out var amount)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Compute.
            {
                // Melewati sisa pernyataan pada iterasi saat ini dan melanjutkan ke elemen/iterasi berikutnya dalam Compute.
                continue;
            // Menutup scope cabang if untuk kondisi `!_payloadReader.TryReadGoldTradeDetailed(evt.Payload, out var tradeType, out var qty, out var unitPrice,
            // out var amount)`; bagian berikut berada di luar batas blok tersebut dalam Compute.
            }

            // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `evt.ActionType == GameActionCatalog.InvestasiEmas` dan
            // `string.Equals(tradeType, ”BUY”, StringComparison.OrdinalIgnoreCase)`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan
            // ketika kondisi ini bernilai benar dalam Compute.
            if (evt.ActionType == GameActionCatalog.InvestasiEmas ||
                // Melanjutkan pengolahan dengan membandingkan kesamaan `string` dengan `tradeType`, `”BUY”`, `StringComparison.OrdinalIgnoreCase`; aturan
                // perbandingan mengikuti overload dan comparer yang diberikan dalam Compute.
                string.Equals(tradeType, "BUY", StringComparison.OrdinalIgnoreCase))
            // Membuka scope cabang if untuk kondisi `evt.ActionType == GameActionCatalog.InvestasiEmas || string.Equals(tradeType, ”BUY”,
            // StringComparison.OrdinalIgnoreCase)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Compute.
            {
                // Memperbarui `goldBuyQty` dengan menambahkan `qty` (nilai qty) dalam Compute.
                goldBuyQty += qty;
                // Memperbarui `goldInvestmentSpent` dengan menambahkan `amount` (nominal uang atau nilai transaksi yang dipakai dalam operasi) dalam Compute.
                goldInvestmentSpent += amount;
                // Memeriksa pemeriksaan lebih besar antara `unitPrice` dan `0`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam Compute.
                if (unitPrice > 0)
                // Membuka scope cabang if untuk kondisi `unitPrice > 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Compute.
                {
                    // Menjalankan menambahkan `unitPrice` ke `goldPurchasePrices` dalam Compute.
                    goldPurchasePrices.Add(unitPrice);
                // Menutup scope cabang if untuk kondisi `unitPrice > 0`; bagian berikut berada di luar batas blok tersebut dalam Compute.
                }
            // Menutup scope cabang if untuk kondisi `evt.ActionType == GameActionCatalog.InvestasiEmas || string.Equals(tradeType, ”BUY”,
            // StringComparison.OrdinalIgnoreCase)`; bagian berikut berada di luar batas blok tersebut dalam Compute.
            }
            // Menjalankan cabang alternatif ketika kondisi if sebelumnya tidak terpenuhi dalam Compute.
            else if (evt.ActionType == GameActionCatalog.JualEmas ||
                     // Melanjutkan pengolahan dengan membandingkan kesamaan `string` dengan `tradeType`, `”SELL”`, `StringComparison.OrdinalIgnoreCase`; aturan
                     // perbandingan mengikuti overload dan comparer yang diberikan dalam Compute.
                     string.Equals(tradeType, "SELL", StringComparison.OrdinalIgnoreCase))
            // Membuka scope cabang if untuk kondisi `evt.ActionType == GameActionCatalog.JualEmas || string.Equals(tradeType, ”SELL”,
            // StringComparison.OrdinalIgnoreCase)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Compute.
            {
                // Memperbarui `goldSellQty` dengan menambahkan `qty` (nilai qty) dalam Compute.
                goldSellQty += qty;
                // Memperbarui `goldInvestmentEarned` dengan menambahkan `amount` (nominal uang atau nilai transaksi yang dipakai dalam operasi) dalam Compute.
                goldInvestmentEarned += amount;
                // Memeriksa pemeriksaan lebih besar antara `unitPrice` dan `0`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam Compute.
                if (unitPrice > 0)
                // Membuka scope cabang if untuk kondisi `unitPrice > 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Compute.
                {
                    // Menjalankan menambahkan `unitPrice` ke `goldSalePrices` dalam Compute.
                    goldSalePrices.Add(unitPrice);
                // Menutup scope cabang if untuk kondisi `unitPrice > 0`; bagian berikut berada di luar batas blok tersebut dalam Compute.
                }
            // Menutup scope cabang if untuk kondisi `evt.ActionType == GameActionCatalog.JualEmas || string.Equals(tradeType, ”SELL”,
            // StringComparison.OrdinalIgnoreCase)`; bagian berikut berada di luar batas blok tersebut dalam Compute.
            }
        // Menutup scope loop setiap evt dari `playerEvents.Where(e => e.ActionType == GameActionCatalog.InvestasiEmas || e.ActionType ==
        // GameActionCatalog.JualEmas || (e.ActionType == GameActionCatalog.RiskEmergencyUsed &...`; bagian berikut berada di luar batas blok tersebut dalam
        // Compute.
        }

        // Mengembalikan objek baru bertipe `AnalyticsGoldGameplayMetrics` dengan argumen ( initialGoldQty, goldBuyQty, goldSellQty, initialGoldQty +
        // goldBuyQty - goldSellQty, goldPurchasePrices, goldSalePrices, goldInvestmentSpent, goldInvestmentEar... kepada pemanggil dalam Compute; eksekusi
        // jalur ini selesai setelah nilai hasil ditentukan.
        return new AnalyticsGoldGameplayMetrics(
            // Meneruskan `initialGoldQty` (nilai awal emas qty) sebagai argumen ke konstruktor `AnalyticsGoldGameplayMetrics`.
            initialGoldQty,
            // Meneruskan `goldBuyQty` (nilai emas buy qty) sebagai argumen ke konstruktor `AnalyticsGoldGameplayMetrics`.
            goldBuyQty,
            // Meneruskan `goldSellQty` (nilai emas sell qty) sebagai argumen ke konstruktor `AnalyticsGoldGameplayMetrics`.
            goldSellQty,
            // Meneruskan selisih antara `initialGoldQty + goldBuyQty` dan `goldSellQty` sebagai argumen ke konstruktor `AnalyticsGoldGameplayMetrics`.
            initialGoldQty + goldBuyQty - goldSellQty,
            // Meneruskan `goldPurchasePrices` (nilai emas pembelian prices) sebagai argumen ke konstruktor `AnalyticsGoldGameplayMetrics`.
            goldPurchasePrices,
            // Meneruskan `goldSalePrices` (nilai emas penjualan prices) sebagai argumen ke konstruktor `AnalyticsGoldGameplayMetrics`.
            goldSalePrices,
            // Meneruskan `goldInvestmentSpent` (nilai emas investment spent) sebagai argumen ke konstruktor `AnalyticsGoldGameplayMetrics`.
            goldInvestmentSpent,
            // Meneruskan `goldInvestmentEarned` (nilai emas investment earned) sebagai argumen ke konstruktor `AnalyticsGoldGameplayMetrics`.
            goldInvestmentEarned,
            // Meneruskan selisih antara `goldInvestmentEarned` dan `goldInvestmentSpent` sebagai argumen ke konstruktor `AnalyticsGoldGameplayMetrics`.
            goldInvestmentEarned - goldInvestmentSpent);
    // Menutup scope metode Compute; bagian berikut berada di luar batas blok tersebut dalam Compute.
    }

    // Mendefinisikan metode `TryReadInt32` dengan hasil bertipe `bool`; operasi ini menangani try read int 32. Masukan: Parameter `payload` bertipe
    // `string` membawa muatan detail event dalam format JSON; Parameter `propertyName` bertipe `string` membawa nilai property nama; Parameter `value`
    // bertipe `int` membawa nilai nilai; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    private static bool TryReadInt32(string payload, string propertyName, out int value)
    // Membuka scope metode TryReadInt32; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadInt32.
    {
        // Memperbarui `value` menggunakan nilai literal `0` dalam TryReadInt32.
        value = 0;
        // Memulai blok try dalam TryReadInt32; exception dari blok ini dapat dialihkan ke catch, sedangkan finally (jika ada) tetap dijalankan saat keluar.
        try
        // Membuka scope penanganan operasi try; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadInt32.
        {
            // Menyiapkan variabel lokal `document` untuk nilai document dengan memanggil `System.Text.Json.JsonDocument.Parse` dengan `payload`. Tipe variabel
            // disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
            using var document = System.Text.Json.JsonDocument.Parse(payload);
            // Mengembalikan gabungan syarat AND: kedua kondisi wajib benar antara `document.RootElement.TryGetProperty(propertyName, out var property)` dan
            // `property.TryGetInt32(out value)`; sisi kanan diperiksa hanya jika sisi kiri benar kepada pemanggil dalam TryReadInt32; eksekusi jalur ini
            // selesai setelah nilai hasil ditentukan.
            return document.RootElement.TryGetProperty(propertyName, out var property) && property.TryGetInt32(out value);
        // Menutup scope penanganan operasi try; bagian berikut berada di luar batas blok tersebut dalam TryReadInt32.
        }
        // Menangani exception `System.Text.Json.JsonException` melalui variabel dalam TryReadInt32.
        catch (System.Text.Json.JsonException)
        // Membuka scope penanganan exception catch; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadInt32.
        {
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryReadInt32; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return false;
        // Menutup scope penanganan exception catch; bagian berikut berada di luar batas blok tersebut dalam TryReadInt32.
        }
    // Menutup scope metode TryReadInt32; bagian berikut berada di luar batas blok tersebut dalam TryReadInt32.
    }

    // Mendefinisikan metode `TryReadString` dengan hasil bertipe `bool`; operasi ini menangani try read string. Masukan: Parameter `payload` bertipe
    // `string` membawa muatan detail event dalam format JSON; Parameter `propertyName` bertipe `string` membawa nilai property nama; Parameter `value`
    // bertipe `string` membawa nilai nilai; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    private static bool TryReadString(string payload, string propertyName, out string value)
    // Membuka scope metode TryReadString; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadString.
    {
        // Memperbarui `value` menggunakan `string.Empty`, yaitu nilai kosong bawaan tipe terkait dalam TryReadString.
        value = string.Empty;
        // Memulai blok try dalam TryReadString; exception dari blok ini dapat dialihkan ke catch, sedangkan finally (jika ada) tetap dijalankan saat
        // keluar.
        try
        // Membuka scope penanganan operasi try; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadString.
        {
            // Menyiapkan variabel lokal `document` untuk nilai document dengan memanggil `System.Text.Json.JsonDocument.Parse` dengan `payload`. Tipe variabel
            // disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
            using var document = System.Text.Json.JsonDocument.Parse(payload);
            // Memeriksa kebalikan kondisi `document.RootElement.TryGetProperty(propertyName, out var property)`; blok if hanya dijalankan ketika kondisi ini
            // bernilai benar dalam TryReadString.
            if (!document.RootElement.TryGetProperty(propertyName, out var property))
            // Membuka scope cabang if untuk kondisi `!document.RootElement.TryGetProperty(propertyName, out var property)`; pernyataan/deklarasi berikut berada
            // di dalam batas blok ini dalam TryReadString.
            {
                // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryReadString; eksekusi jalur ini selesai setelah nilai hasil
                // ditentukan.
                return false;
            // Menutup scope cabang if untuk kondisi `!document.RootElement.TryGetProperty(propertyName, out var property)`; bagian berikut berada di luar batas
            // blok tersebut dalam TryReadString.
            }

            // Memperbarui `value` menggunakan `property.GetString()` bila tidak null; jika null gunakan `string.Empty` sebagai nilai pengganti dalam
            // TryReadString.
            value = property.GetString() ?? string.Empty;
            // Mengembalikan pemeriksaan lebih besar antara `value.Length` dan `0` kepada pemanggil dalam TryReadString; eksekusi jalur ini selesai setelah
            // nilai hasil ditentukan.
            return value.Length > 0;
        // Menutup scope penanganan operasi try; bagian berikut berada di luar batas blok tersebut dalam TryReadString.
        }
        // Menangani exception `System.Text.Json.JsonException` melalui variabel dalam TryReadString.
        catch (System.Text.Json.JsonException)
        // Membuka scope penanganan exception catch; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadString.
        {
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryReadString; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return false;
        // Menutup scope penanganan exception catch; bagian berikut berada di luar batas blok tersebut dalam TryReadString.
        }
    // Menutup scope metode TryReadString; bagian berikut berada di luar batas blok tersebut dalam TryReadString.
    }
// Menutup scope tipe GoldGameplayCalculator; bagian berikut berada di luar batas blok tersebut.
}
