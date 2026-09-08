// Fungsi file: Memverifikasi perilaku API, database, atau domain melalui AnalyticsIngredientMealCalculatorTests.
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;
// Mengimpor namespace `Cashflowpoly.Api.Domain` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Domain;
// Mengimpor namespace `Xunit` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Xunit;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Tests` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Tests;

// Mendefinisikan tipe class `AnalyticsIngredientMealCalculatorTests`; sealed mencegah tipe ini diturunkan lagi.
public sealed class AnalyticsIngredientMealCalculatorTests
// Membuka scope tipe AnalyticsIngredientMealCalculatorTests; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `Compute_BuildsIngredientInventoryAndMealOrderMetrics` dengan hasil bertipe `void`; operasi ini menangani compute builds
    // bahan inventory dan meal urutan/pesanan metrics.
    public void Compute_BuildsIngredientInventoryAndMealOrderMetrics()
    // Membuka scope metode Compute_BuildsIngredientInventoryAndMealOrderMetrics; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // Compute_BuildsIngredientInventoryAndMealOrderMetrics.
    {
        // Menyiapkan variabel lokal `playerId` untuk nilai pemain identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var playerId = Guid.NewGuid();
        // Menyiapkan variabel lokal `sessionId` untuk identitas unik sesi permainan yang menjadi batas data operasi ini dengan memanggil `Guid.NewGuid`
        // dengan tanpa argumen. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var sessionId = Guid.NewGuid();
        // Menyiapkan variabel lokal `flourPurchaseId` untuk nilai flour pembelian identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var flourPurchaseId = Guid.NewGuid();
        // Menyiapkan variabel lokal `eggPurchaseId` untuk nilai egg pembelian identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var eggPurchaseId = Guid.NewGuid();
        // Menyiapkan variabel lokal `orderId` untuk nilai urutan/pesanan identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var orderId = Guid.NewGuid();
        // Menyiapkan variabel lokal `events` untuk kumpulan event permainan sebagai sumber riwayat untuk validasi atau perhitungan dengan objek baru
        // bertipe `List<EventDb>` dengan nilai awal sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var events = new List<EventDb>
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // Compute_BuildsIngredientInventoryAndMealOrderMetrics.
        {
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `flourPurchaseId`, `sessionId`, `playerId`, `”BahanMasakan”`,
            // `”””{”card_id”:”flour”,”ingredient_name”:”Flour”,”amount”:3}”””`, `1`, `1` dalam Compute_BuildsIngredientInventoryAndMealOrderMetrics.
            CreateEvent(flourPurchaseId, sessionId, playerId, "BahanMasakan", """{"card_id":"flour","ingredient_name":"Flour","amount":3}""", turn: 1, sequence: 1),
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `eggPurchaseId`, `sessionId`, `playerId`, `”BahanMasakan”`,
            // `”””{”card_id”:”egg”,”ingredient_name”:”Egg”,”amount”:1}”””`, `1`, `2` dalam Compute_BuildsIngredientInventoryAndMealOrderMetrics.
            CreateEvent(eggPurchaseId, sessionId, playerId, "BahanMasakan", """{"card_id":"egg","ingredient_name":"Egg","amount":1}""", turn: 1, sequence: 2),
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `orderId`, `sessionId`, `playerId`, `”JualMasakan”`,
            // `”””{”required_ingredient_card_ids”:[”flour”,”egg”],”income”:12}”””`, `2`, `3` dalam Compute_BuildsIngredientInventoryAndMealOrderMetrics.
            CreateEvent(orderId, sessionId, playerId, "JualMasakan", """{"required_ingredient_card_ids":["flour","egg"],"income":12}""", turn: 2, sequence: 3),
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `Guid.NewGuid()`, `sessionId`, `playerId`, `”BuangBahanMasakan”`,
            // `”””{”card_id”:”flour”,”amount”:1}”””`, `3`, `4` dalam Compute_BuildsIngredientInventoryAndMealOrderMetrics.
            CreateEvent(Guid.NewGuid(), sessionId, playerId, "BuangBahanMasakan", """{"card_id":"flour","amount":1}""", turn: 3, sequence: 4),
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `Guid.NewGuid()`, `sessionId`, `playerId`, `”LewatiOrder”`,
            // `”””{”required_ingredient_card_ids”:[”flour”],”income”:8}”””`, `4`, `5` dalam Compute_BuildsIngredientInventoryAndMealOrderMetrics.
            CreateEvent(Guid.NewGuid(), sessionId, playerId, "LewatiOrder", """{"required_ingredient_card_ids":["flour"],"income":8}""", turn: 4, sequence: 5)
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // Compute_BuildsIngredientInventoryAndMealOrderMetrics.
        };
        // Menyiapkan variabel lokal `projections` untuk proyeksi transaksi arus kas yang diturunkan dari event permainan dengan objek baru bertipe
        // `List<CashflowProjectionDb>` dengan nilai awal sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var projections = new List<CashflowProjectionDb>
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // Compute_BuildsIngredientInventoryAndMealOrderMetrics.
        {
            // Melanjutkan pengolahan dengan memanggil `CreateProjection` dengan `flourPurchaseId`, `sessionId`, `playerId`, `”OUT”`, `3`, `”INGREDIENT”` dalam
            // Compute_BuildsIngredientInventoryAndMealOrderMetrics.
            CreateProjection(flourPurchaseId, sessionId, playerId, "OUT", 3, "INGREDIENT"),
            // Melanjutkan pengolahan dengan memanggil `CreateProjection` dengan `eggPurchaseId`, `sessionId`, `playerId`, `”OUT”`, `1`, `”INGREDIENT”` dalam
            // Compute_BuildsIngredientInventoryAndMealOrderMetrics.
            CreateProjection(eggPurchaseId, sessionId, playerId, "OUT", 1, "INGREDIENT"),
            // Melanjutkan pengolahan dengan memanggil `CreateProjection` dengan `orderId`, `sessionId`, `playerId`, `”IN”`, `12`, `”ORDER_INCOME”` dalam
            // Compute_BuildsIngredientInventoryAndMealOrderMetrics.
            CreateProjection(orderId, sessionId, playerId, "IN", 12, "ORDER_INCOME")
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // Compute_BuildsIngredientInventoryAndMealOrderMetrics.
        };

        // Menyiapkan variabel lokal `metrics` untuk nilai metrics dengan memanggil `new IngredientMealCalculator().Compute` dengan `events`, `projections`.
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var metrics = new IngredientMealCalculator().Compute(events, projections);

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`2`, `metrics.IngredientsCollected`); pengujian
        // gagal jika keduanya berbeda dalam Compute_BuildsIngredientInventoryAndMealOrderMetrics.
        Assert.Equal(2, metrics.IngredientsCollected);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`0`, `metrics.Inventory.Total`); pengujian
        // gagal jika keduanya berbeda dalam Compute_BuildsIngredientInventoryAndMealOrderMetrics.
        Assert.Equal(0, metrics.Inventory.Total);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`0`, `metrics.Inventory.ByCardId[”flour”]`);
        // pengujian gagal jika keduanya berbeda dalam Compute_BuildsIngredientInventoryAndMealOrderMetrics.
        Assert.Equal(0, metrics.Inventory.ByCardId["flour"]);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`0`, `metrics.Inventory.ByCardId[”egg”]`);
        // pengujian gagal jika keduanya berbeda dalam Compute_BuildsIngredientInventoryAndMealOrderMetrics.
        Assert.Equal(0, metrics.Inventory.ByCardId["egg"]);
        // Menjalankan pemeriksaan bahwa `metrics.IngredientTypesHeld.ContainsKey(”Flour”)` bernilai salah; pengujian gagal jika kondisi justru terpenuhi
        // dalam Compute_BuildsIngredientInventoryAndMealOrderMetrics.
        Assert.False(metrics.IngredientTypesHeld.ContainsKey("Flour"));
        // Menjalankan pemeriksaan bahwa `metrics.IngredientTypesHeld.ContainsKey(”Egg”)` bernilai salah; pengujian gagal jika kondisi justru terpenuhi
        // dalam Compute_BuildsIngredientInventoryAndMealOrderMetrics.
        Assert.False(metrics.IngredientTypesHeld.ContainsKey("Egg"));
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`2`, `metrics.IngredientsUsedTotal`); pengujian
        // gagal jika keduanya berbeda dalam Compute_BuildsIngredientInventoryAndMealOrderMetrics.
        Assert.Equal(2, metrics.IngredientsUsedTotal);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`1`, `metrics.IngredientsWasted`); pengujian
        // gagal jika keduanya berbeda dalam Compute_BuildsIngredientInventoryAndMealOrderMetrics.
        Assert.Equal(1, metrics.IngredientsWasted);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`4`, `metrics.IngredientInvestmentTotal`);
        // pengujian gagal jika keduanya berbeda dalam Compute_BuildsIngredientInventoryAndMealOrderMetrics.
        Assert.Equal(4, metrics.IngredientInvestmentTotal);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`new[] { 12 }`,
        // `metrics.MealOrderIncomeValues`); pengujian gagal jika keduanya berbeda dalam Compute_BuildsIngredientInventoryAndMealOrderMetrics.
        Assert.Equal(new[] { 12 }, metrics.MealOrderIncomeValues);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`1`, `metrics.MealOrdersClaimed`); pengujian
        // gagal jika keduanya berbeda dalam Compute_BuildsIngredientInventoryAndMealOrderMetrics.
        Assert.Equal(1, metrics.MealOrdersClaimed);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`12`, `metrics.MealOrderIncomeTotal`);
        // pengujian gagal jika keduanya berbeda dalam Compute_BuildsIngredientInventoryAndMealOrderMetrics.
        Assert.Equal(12, metrics.MealOrderIncomeTotal);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`2`, `metrics.LatestDayIndex`); pengujian gagal
        // jika keduanya berbeda dalam Compute_BuildsIngredientInventoryAndMealOrderMetrics.
        Assert.Equal(2, metrics.LatestDayIndex);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`1.0 / 3.0`,
        // `metrics.MealOrdersPerTurnAverage`); pengujian gagal jika keduanya berbeda dalam Compute_BuildsIngredientInventoryAndMealOrderMetrics.
        Assert.Equal(1.0 / 3.0, metrics.MealOrdersPerTurnAverage);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`4`, `metrics.EssentialIngredientExpenses`);
        // pengujian gagal jika keduanya berbeda dalam Compute_BuildsIngredientInventoryAndMealOrderMetrics.
        Assert.Equal(4, metrics.EssentialIngredientExpenses);
    // Menutup scope metode Compute_BuildsIngredientInventoryAndMealOrderMetrics; bagian berikut berada di luar batas blok tersebut dalam
    // Compute_BuildsIngredientInventoryAndMealOrderMetrics.
    }

    // Mendefinisikan metode `CreateEvent` dengan hasil bertipe `EventDb`; operasi ini menangani create event. Masukan: Parameter `eventId` bertipe
    // `Guid` membawa identitas unik event untuk pencatatan dan pemeriksaan duplikasi; Parameter `sessionId` bertipe `Guid` membawa identitas unik sesi
    // permainan yang menjadi batas data operasi ini; Parameter `playerId` bertipe `Guid` membawa nilai pemain identitas; Parameter `actionType` bertipe
    // `string` membawa nilai aksi jenis; Parameter `payload` bertipe `string` membawa muatan detail event dalam format JSON; Parameter `turn` bertipe
    // `int` membawa giliran pemain yang sedang berlangsung; Parameter `sequence` bertipe `long` membawa nomor urut event yang menentukan urutan
    // pemrosesan riwayat permainan.
    private static EventDb CreateEvent(
        // Parameter `eventId` bertipe `Guid` membawa identitas unik event untuk pencatatan dan pemeriksaan duplikasi.
        Guid eventId,
        // Parameter `sessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini.
        Guid sessionId,
        // Parameter `playerId` bertipe `Guid` membawa nilai pemain identitas.
        Guid playerId,
        // Parameter `actionType` bertipe `string` membawa nilai aksi jenis.
        string actionType,
        // Parameter `payload` bertipe `string` membawa muatan detail event dalam format JSON.
        string payload,
        // Parameter `turn` bertipe `int` membawa giliran pemain yang sedang berlangsung.
        int turn,
        // Parameter `sequence` bertipe `long` membawa nomor urut event yang menentukan urutan pemrosesan riwayat permainan.
        long sequence)
    // Membuka scope metode CreateEvent; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CreateEvent.
    {
        // Mengembalikan objek baru bertipe `EventDb` dengan nilai awal sesuai konstruktornya kepada pemanggil dalam CreateEvent; eksekusi jalur ini selesai
        // setelah nilai hasil ditentukan.
        return new EventDb
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CreateEvent.
        {
            // Memperbarui `EventId` menggunakan `eventId` (identitas unik event untuk pencatatan dan pemeriksaan duplikasi) dalam CreateEvent.
            EventId = eventId,
            // Memperbarui `SessionId` menggunakan `sessionId` (identitas unik sesi permainan yang menjadi batas data operasi ini) dalam CreateEvent.
            SessionId = sessionId,
            // Memperbarui `UserId` menggunakan `playerId` (nilai pemain identitas) dalam CreateEvent.
            UserId = playerId,
            // Memperbarui `ActorType` menggunakan nilai literal `”PLAYER”` dalam CreateEvent.
            ActorType = "PLAYER",
            // Memperbarui `Timestamp` menggunakan objek baru bertipe `DateTimeOffset` dengan argumen (2026, 1, 2, 3, 4, 5, TimeSpan.Zero) dalam CreateEvent.
            Timestamp = new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero),
            // Memperbarui `DayIndex` menggunakan selisih antara `turn` dan `1` dalam CreateEvent.
            DayIndex = turn - 1,
            // Memperbarui `Weekday` menggunakan nilai literal `”MON”` dalam CreateEvent.
            Weekday = "MON",
            // Memperbarui `ActionSlot` menggunakan penjumlahan/penggabungan antara `((turn - 1) % 2)` dan `1` dalam CreateEvent.
            ActionSlot = ((turn - 1) % 2) + 1,
            // Memperbarui `SequenceNumber` menggunakan `sequence` (nomor urut event yang menentukan urutan pemrosesan riwayat permainan) dalam CreateEvent.
            SequenceNumber = sequence,
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

    // Mendefinisikan metode `CreateProjection` dengan hasil bertipe `CashflowProjectionDb`; operasi ini menangani create projection. Masukan: Parameter
    // `eventId` bertipe `Guid` membawa identitas unik event untuk pencatatan dan pemeriksaan duplikasi; Parameter `sessionId` bertipe `Guid` membawa
    // identitas unik sesi permainan yang menjadi batas data operasi ini; Parameter `playerId` bertipe `Guid` membawa nilai pemain identitas; Parameter
    // `direction` bertipe `string` membawa nilai direction; Parameter `amount` bertipe `int` membawa nominal uang atau nilai transaksi yang dipakai
    // dalam operasi; Parameter `category` bertipe `string` membawa nilai category.
    private static CashflowProjectionDb CreateProjection(
        // Parameter `eventId` bertipe `Guid` membawa identitas unik event untuk pencatatan dan pemeriksaan duplikasi.
        Guid eventId,
        // Parameter `sessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini.
        Guid sessionId,
        // Parameter `playerId` bertipe `Guid` membawa nilai pemain identitas.
        Guid playerId,
        // Parameter `direction` bertipe `string` membawa nilai direction.
        string direction,
        // Parameter `amount` bertipe `int` membawa nominal uang atau nilai transaksi yang dipakai dalam operasi.
        int amount,
        // Parameter `category` bertipe `string` membawa nilai category.
        string category)
    // Membuka scope metode CreateProjection; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CreateProjection.
    {
        // Mengembalikan objek baru bertipe `CashflowProjectionDb` dengan nilai awal sesuai konstruktornya kepada pemanggil dalam CreateProjection; eksekusi
        // jalur ini selesai setelah nilai hasil ditentukan.
        return new CashflowProjectionDb
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CreateProjection.
        {
            // Memperbarui `ProjectionId` menggunakan memanggil `Guid.NewGuid` dengan tanpa argumen dalam CreateProjection.
            ProjectionId = Guid.NewGuid(),
            // Memperbarui `SessionId` menggunakan `sessionId` (identitas unik sesi permainan yang menjadi batas data operasi ini) dalam CreateProjection.
            SessionId = sessionId,
            // Memperbarui `UserId` menggunakan `playerId` (nilai pemain identitas) dalam CreateProjection.
            UserId = playerId,
            // Memperbarui `EventPk` menggunakan memanggil `Guid.NewGuid` dengan tanpa argumen dalam CreateProjection.
            EventPk = Guid.NewGuid(),
            // Memperbarui `EventId` menggunakan `eventId` (identitas unik event untuk pencatatan dan pemeriksaan duplikasi) dalam CreateProjection.
            EventId = eventId,
            // Memperbarui `Timestamp` menggunakan objek baru bertipe `DateTimeOffset` dengan argumen (2026, 1, 2, 3, 4, 5, TimeSpan.Zero) dalam
            // CreateProjection.
            Timestamp = new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero),
            // Memperbarui `Direction` menggunakan `direction` (nilai direction) dalam CreateProjection.
            Direction = direction,
            // Memperbarui `Amount` menggunakan `amount` (nominal uang atau nilai transaksi yang dipakai dalam operasi) dalam CreateProjection.
            Amount = amount,
            // Memperbarui `Category` menggunakan `category` (nilai category) dalam CreateProjection.
            Category = category
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam CreateProjection.
        };
    // Menutup scope metode CreateProjection; bagian berikut berada di luar batas blok tersebut dalam CreateProjection.
    }
// Menutup scope tipe AnalyticsIngredientMealCalculatorTests; bagian berikut berada di luar batas blok tersebut.
}
