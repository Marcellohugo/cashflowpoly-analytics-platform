// Fungsi file: Memverifikasi perilaku API, database, atau domain melalui AnalyticsGoldGameplayCalculatorTests.
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;
// Mengimpor namespace `Cashflowpoly.Api.Domain` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Domain;
// Mengimpor namespace `Xunit` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Xunit;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Tests` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Tests;

// Mendefinisikan tipe class `AnalyticsGoldGameplayCalculatorTests`; sealed mencegah tipe ini diturunkan lagi.
public sealed class AnalyticsGoldGameplayCalculatorTests
// Membuka scope tipe AnalyticsGoldGameplayCalculatorTests; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `Compute_SummarizesBuySellQtyPricesAndNetInvestment` dengan hasil bertipe `void`; operasi ini menangani compute summarizes
    // buy sell qty prices dan net investment.
    public void Compute_SummarizesBuySellQtyPricesAndNetInvestment()
    // Membuka scope metode Compute_SummarizesBuySellQtyPricesAndNetInvestment; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // Compute_SummarizesBuySellQtyPricesAndNetInvestment.
    {
        // Menyiapkan variabel lokal `playerId` untuk nilai pemain identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var playerId = Guid.NewGuid();
        // Menyiapkan variabel lokal `events` untuk kumpulan event permainan sebagai sumber riwayat untuk validasi atau perhitungan dengan objek baru
        // bertipe `List<EventDb>` dengan nilai awal sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var events = new List<EventDb>
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // Compute_SummarizesBuySellQtyPricesAndNetInvestment.
        {
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `playerId`, `”SetupEmasAwal”`,
            // `”””{”asset_code”:”gold_card”,”qty”:1,”unit_value”:5,”setup”:”INITIAL”}”””` dalam Compute_SummarizesBuySellQtyPricesAndNetInvestment.
            CreateEvent(playerId, "SetupEmasAwal", """{"asset_code":"gold_card","qty":1,"unit_value":5,"setup":"INITIAL"}"""),
            // Melanjutkan pengolahan dengan memanggil `CreateGoldTrade` dengan `playerId`, `”BUY”`, `3`, `4`, `12` dalam
            // Compute_SummarizesBuySellQtyPricesAndNetInvestment.
            CreateGoldTrade(playerId, "BUY", qty: 3, unitPrice: 4, amount: 12),
            // Melanjutkan pengolahan dengan memanggil `CreateGoldTrade` dengan `playerId`, `”SELL”`, `1`, `5`, `5` dalam
            // Compute_SummarizesBuySellQtyPricesAndNetInvestment.
            CreateGoldTrade(playerId, "SELL", qty: 1, unitPrice: 5, amount: 5),
            // Melanjutkan pengolahan dengan memanggil `CreateGoldTrade` dengan `playerId`, `”BUY”`, `2`, `6`, `12` dalam
            // Compute_SummarizesBuySellQtyPricesAndNetInvestment.
            CreateGoldTrade(playerId, "BUY", qty: 2, unitPrice: 6, amount: 12),
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `playerId`, `”GunakanOpsiDarurat”`,
            // `”””{”option_type”:”SELL_GOLD”,”qty”:2,”unit_price”:7,”amount”:14}”””` dalam Compute_SummarizesBuySellQtyPricesAndNetInvestment.
            CreateEvent(playerId, "GunakanOpsiDarurat", """{"option_type":"SELL_GOLD","qty":2,"unit_price":7,"amount":14}"""),
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `playerId`, `”CatatTransaksi”`, `”””{”amount”:99}”””` dalam
            // Compute_SummarizesBuySellQtyPricesAndNetInvestment.
            CreateEvent(playerId, "CatatTransaksi", """{"amount":99}""")
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // Compute_SummarizesBuySellQtyPricesAndNetInvestment.
        };

        // Menyiapkan variabel lokal `metrics` untuk nilai metrics dengan memanggil `new GoldGameplayCalculator().Compute` dengan `events`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var metrics = new GoldGameplayCalculator().Compute(events);

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`1`, `metrics.InitialGoldQty`); pengujian gagal
        // jika keduanya berbeda dalam Compute_SummarizesBuySellQtyPricesAndNetInvestment.
        Assert.Equal(1, metrics.InitialGoldQty);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`5`, `metrics.GoldBuyQty`); pengujian gagal
        // jika keduanya berbeda dalam Compute_SummarizesBuySellQtyPricesAndNetInvestment.
        Assert.Equal(5, metrics.GoldBuyQty);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`3`, `metrics.GoldSellQty`); pengujian gagal
        // jika keduanya berbeda dalam Compute_SummarizesBuySellQtyPricesAndNetInvestment.
        Assert.Equal(3, metrics.GoldSellQty);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`3`, `metrics.GoldHeldEnd`); pengujian gagal
        // jika keduanya berbeda dalam Compute_SummarizesBuySellQtyPricesAndNetInvestment.
        Assert.Equal(3, metrics.GoldHeldEnd);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`new[] { 4, 6 }`,
        // `metrics.GoldPurchasePrices`); pengujian gagal jika keduanya berbeda dalam Compute_SummarizesBuySellQtyPricesAndNetInvestment.
        Assert.Equal(new[] { 4, 6 }, metrics.GoldPurchasePrices);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`new[] { 5, 7 }`, `metrics.GoldSalePrices`);
        // pengujian gagal jika keduanya berbeda dalam Compute_SummarizesBuySellQtyPricesAndNetInvestment.
        Assert.Equal(new[] { 5, 7 }, metrics.GoldSalePrices);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`24`, `metrics.GoldInvestmentSpent`); pengujian
        // gagal jika keduanya berbeda dalam Compute_SummarizesBuySellQtyPricesAndNetInvestment.
        Assert.Equal(24, metrics.GoldInvestmentSpent);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`19`, `metrics.GoldInvestmentEarned`);
        // pengujian gagal jika keduanya berbeda dalam Compute_SummarizesBuySellQtyPricesAndNetInvestment.
        Assert.Equal(19, metrics.GoldInvestmentEarned);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`-5`, `metrics.GoldInvestmentNet`); pengujian
        // gagal jika keduanya berbeda dalam Compute_SummarizesBuySellQtyPricesAndNetInvestment.
        Assert.Equal(-5, metrics.GoldInvestmentNet);
    // Menutup scope metode Compute_SummarizesBuySellQtyPricesAndNetInvestment; bagian berikut berada di luar batas blok tersebut dalam
    // Compute_SummarizesBuySellQtyPricesAndNetInvestment.
    }

    // Mendefinisikan metode `CreateGoldTrade` dengan hasil bertipe `EventDb`; operasi ini menangani create emas trade. Masukan: Parameter `playerId`
    // bertipe `Guid` membawa nilai pemain identitas; Parameter `tradeType` bertipe `string` membawa nilai trade jenis; Parameter `qty` bertipe `int`
    // membawa nilai qty; Parameter `unitPrice` bertipe `int` membawa nilai unit harga; Parameter `amount` bertipe `int` membawa nominal uang atau nilai
    // transaksi yang dipakai dalam operasi.
    private static EventDb CreateGoldTrade(Guid playerId, string tradeType, int qty, int unitPrice, int amount)
    // Membuka scope metode CreateGoldTrade; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CreateGoldTrade.
    {
        // Mengembalikan memanggil `CreateEvent` dengan `playerId`, `string.Equals(tradeType, ”SELL”, StringComparison.OrdinalIgnoreCase) ? ”JualEmas” :
        // ”InvestasiEmas”`, `$$”””{”trade_type”:”{{tradeType}}”,”qty”:{{qty}},”unit_price”:{{unitPrice}},”amount”:{{amount}}}”””` kepada pemanggil dalam
        // CreateGoldTrade; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return CreateEvent(
            // Meneruskan `playerId` (nilai pemain identitas) sebagai argumen ke `CreateEvent`.
            playerId,
            // Meneruskan hasil pemilihan bersyarat: ketika `string.Equals(tradeType, ”SELL”, StringComparison.OrdinalIgnoreCase)` benar gunakan `”JualEmas”`,
            // jika tidak gunakan `”InvestasiEmas”` sebagai argumen ke `CreateEvent`; Meneruskan `tradeType` (nilai trade jenis) sebagai argumen ke
            // `string.Equals`; Meneruskan nilai literal `”SELL”` sebagai argumen ke `string.Equals`; Meneruskan `StringComparison.OrdinalIgnoreCase` (nilai
            // ordinal ignore case) sebagai argumen ke `string.Equals`.
            string.Equals(tradeType, "SELL", StringComparison.OrdinalIgnoreCase) ? "JualEmas" : "InvestasiEmas",
            // Meneruskan teks interpolasi `$$”””{”trade_type”:”{{tradeType}}”,”qty”:{{qty}},”unit_price”:{{unitPrice}},”amount”:{{amount}}}”””`; nilai ekspresi
            // di dalam kurung kurawal disisipkan saat program berjalan sebagai argumen ke `CreateEvent`.
            $$"""{"trade_type":"{{tradeType}}","qty":{{qty}},"unit_price":{{unitPrice}},"amount":{{amount}}}""");
    // Menutup scope metode CreateGoldTrade; bagian berikut berada di luar batas blok tersebut dalam CreateGoldTrade.
    }

    // Mendefinisikan metode `CreateEvent` dengan hasil bertipe `EventDb`; operasi ini menangani create event. Masukan: Parameter `playerId` bertipe
    // `Guid` membawa nilai pemain identitas; Parameter `actionType` bertipe `string` membawa nilai aksi jenis; Parameter `payload` bertipe `string`
    // membawa muatan detail event dalam format JSON.
    private static EventDb CreateEvent(Guid playerId, string actionType, string payload)
    // Membuka scope metode CreateEvent; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CreateEvent.
    {
        // Mengembalikan objek baru bertipe `EventDb` dengan nilai awal sesuai konstruktornya kepada pemanggil dalam CreateEvent; eksekusi jalur ini selesai
        // setelah nilai hasil ditentukan.
        return new EventDb
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CreateEvent.
        {
            // Memperbarui `EventId` menggunakan memanggil `Guid.NewGuid` dengan tanpa argumen dalam CreateEvent.
            EventId = Guid.NewGuid(),
            // Memperbarui `SessionId` menggunakan memanggil `Guid.NewGuid` dengan tanpa argumen dalam CreateEvent.
            SessionId = Guid.NewGuid(),
            // Memperbarui `UserId` menggunakan `playerId` (nilai pemain identitas) dalam CreateEvent.
            UserId = playerId,
            // Memperbarui `ActorType` menggunakan nilai literal `”PLAYER”` dalam CreateEvent.
            ActorType = "PLAYER",
            // Memperbarui `Timestamp` menggunakan objek baru bertipe `DateTimeOffset` dengan argumen (2026, 1, 2, 3, 4, 5, TimeSpan.Zero) dalam CreateEvent.
            Timestamp = new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero),
            // Memperbarui `DayIndex` menggunakan nilai literal `0` dalam CreateEvent.
            DayIndex = 0,
            // Memperbarui `Weekday` menggunakan nilai literal `”SAT”` dalam CreateEvent.
            Weekday = "SAT",
            // Memperbarui `ActionSlot` menggunakan nilai literal `1` dalam CreateEvent.
            ActionSlot = 1,
            // Memperbarui `SequenceNumber` menggunakan nilai literal `1` dalam CreateEvent.
            SequenceNumber = 1,
            // Memperbarui `ActionType` menggunakan `actionType` (nilai aksi jenis) dalam CreateEvent.
            ActionType = actionType,
            // Memperbarui `RulesetVersionId` menggunakan memanggil `Guid.NewGuid` dengan tanpa argumen dalam CreateEvent.
            RulesetVersionId = Guid.NewGuid(),
            // Memperbarui `Payload` menggunakan `payload` (muatan detail event dalam format JSON) dalam CreateEvent.
            Payload = payload
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam CreateEvent.
        };
    // Menutup scope metode CreateEvent; bagian berikut berada di luar batas blok tersebut dalam CreateEvent.
    }
// Menutup scope tipe AnalyticsGoldGameplayCalculatorTests; bagian berikut berada di luar batas blok tersebut.
}
