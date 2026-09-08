// Fungsi file: Memverifikasi perilaku API, database, atau domain melalui AnalyticsIngredientInventoryCalculatorTests.
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;
// Mengimpor namespace `Cashflowpoly.Api.Domain` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Domain;
// Mengimpor namespace `Xunit` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Xunit;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Tests` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Tests;

// Mendefinisikan tipe class `AnalyticsIngredientInventoryCalculatorTests`; sealed mencegah tipe ini diturunkan lagi.
public sealed class AnalyticsIngredientInventoryCalculatorTests
// Membuka scope tipe AnalyticsIngredientInventoryCalculatorTests; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `BuildIngredientInventory_CountsSetupPriceAsOneCard` dengan hasil bertipe `void`; operasi ini menangani build bahan
    // inventory counts setup harga as one kartu.
    public void BuildIngredientInventory_CountsSetupPriceAsOneCard()
    // Membuka scope metode BuildIngredientInventory_CountsSetupPriceAsOneCard; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // BuildIngredientInventory_CountsSetupPriceAsOneCard.
    {
        // Menyiapkan variabel lokal `inventory` untuk nilai inventory dengan memanggil `new IngredientInventoryCalculator().BuildIngredientInventory`
        // dengan `[ BuildEvent(”SetupBahanAwal”, ”””{”card_id”:”meat”,”amount”:5,”setup”:”INITIAL”}”””) ]`. Tipe variabel disimpulkan dari ekspresi nilai
        // awal.
        var inventory = new IngredientInventoryCalculator().BuildIngredientInventory(
        // Meneruskan koleksi berisi BuildEvent(”SetupBahanAwal”, ”””{”card_id”:”meat”,... sebagai argumen ke `new
        // IngredientInventoryCalculator().BuildIngredientInventory`.
        [
            // Meneruskan nilai literal `”SetupBahanAwal”` sebagai argumen ke `BuildEvent`; Meneruskan nilai literal
            // `”””{”card_id”:”meat”,”amount”:5,”setup”:”INITIAL”}”””` sebagai argumen ke `BuildEvent`.
            BuildEvent("SetupBahanAwal", """{"card_id":"meat","amount":5,"setup":"INITIAL"}""")
        // Meneruskan koleksi berisi BuildEvent(”SetupBahanAwal”, ”””{”card_id”:”meat”,... sebagai argumen ke `new
        // IngredientInventoryCalculator().BuildIngredientInventory`.
        ]);

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`1`, `inventory.Total`); pengujian gagal jika
        // keduanya berbeda dalam BuildIngredientInventory_CountsSetupPriceAsOneCard.
        Assert.Equal(1, inventory.Total);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`1`, `inventory.ByCardId[”meat”]`); pengujian
        // gagal jika keduanya berbeda dalam BuildIngredientInventory_CountsSetupPriceAsOneCard.
        Assert.Equal(1, inventory.ByCardId["meat"]);
    // Menutup scope metode BuildIngredientInventory_CountsSetupPriceAsOneCard; bagian berikut berada di luar batas blok tersebut dalam
    // BuildIngredientInventory_CountsSetupPriceAsOneCard.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `BuildIngredientInventory_AppliesPurchaseOrderClaimAndDiscard` dengan hasil bertipe `void`; operasi ini menangani build
    // bahan inventory applies pembelian urutan/pesanan claim dan discard.
    public void BuildIngredientInventory_AppliesPurchaseOrderClaimAndDiscard()
    // Membuka scope metode BuildIngredientInventory_AppliesPurchaseOrderClaimAndDiscard; pernyataan/deklarasi berikut berada di dalam batas blok ini
    // dalam BuildIngredientInventory_AppliesPurchaseOrderClaimAndDiscard.
    {
        // Menyiapkan variabel lokal `events` untuk kumpulan event permainan sebagai sumber riwayat untuk validasi atau perhitungan dengan objek baru
        // bertipe `List<EventDb>` dengan nilai awal sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var events = new List<EventDb>
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // BuildIngredientInventory_AppliesPurchaseOrderClaimAndDiscard.
        {
            // Melanjutkan pengolahan dengan memanggil `BuildEvent` dengan `”BahanMasakan”`, `”””{”card_id”:”flour”,”amount”:2}”””` dalam
            // BuildIngredientInventory_AppliesPurchaseOrderClaimAndDiscard.
            BuildEvent("BahanMasakan", """{"card_id":"flour","amount":2}"""),
            // Melanjutkan pengolahan dengan memanggil `BuildEvent` dengan `”BahanMasakan”`, `”””{”card_id”:”egg”,”amount”:1}”””` dalam
            // BuildIngredientInventory_AppliesPurchaseOrderClaimAndDiscard.
            BuildEvent("BahanMasakan", """{"card_id":"egg","amount":1}"""),
            // Melanjutkan pengolahan dengan memanggil `BuildEvent` dengan `”JualMasakan”`, `”””{”required_ingredient_card_ids”:[”flour”,”egg”],”income”:8}”””`
            // dalam BuildIngredientInventory_AppliesPurchaseOrderClaimAndDiscard.
            BuildEvent("JualMasakan", """{"required_ingredient_card_ids":["flour","egg"],"income":8}"""),
            // Melanjutkan pengolahan dengan memanggil `BuildEvent` dengan `”BuangBahanMasakan”`, `”””{”card_id”:”flour”,”amount”:1}”””` dalam
            // BuildIngredientInventory_AppliesPurchaseOrderClaimAndDiscard.
            BuildEvent("BuangBahanMasakan", """{"card_id":"flour","amount":1}""")
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // BuildIngredientInventory_AppliesPurchaseOrderClaimAndDiscard.
        };

        // Menyiapkan variabel lokal `inventory` untuk nilai inventory dengan memanggil `new IngredientInventoryCalculator().BuildIngredientInventory`
        // dengan `events`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var inventory = new IngredientInventoryCalculator().BuildIngredientInventory(events);

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`0`, `inventory.Total`); pengujian gagal jika
        // keduanya berbeda dalam BuildIngredientInventory_AppliesPurchaseOrderClaimAndDiscard.
        Assert.Equal(0, inventory.Total);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`0`, `inventory.ByCardId[”flour”]`); pengujian
        // gagal jika keduanya berbeda dalam BuildIngredientInventory_AppliesPurchaseOrderClaimAndDiscard.
        Assert.Equal(0, inventory.ByCardId["flour"]);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`0`, `inventory.ByCardId[”egg”]`); pengujian
        // gagal jika keduanya berbeda dalam BuildIngredientInventory_AppliesPurchaseOrderClaimAndDiscard.
        Assert.Equal(0, inventory.ByCardId["egg"]);
    // Menutup scope metode BuildIngredientInventory_AppliesPurchaseOrderClaimAndDiscard; bagian berikut berada di luar batas blok tersebut dalam
    // BuildIngredientInventory_AppliesPurchaseOrderClaimAndDiscard.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `BuildIngredientInventory_IgnoresMalformedPayloads` dengan hasil bertipe `void`; operasi ini menangani build bahan
    // inventory ignores malformed payloads.
    public void BuildIngredientInventory_IgnoresMalformedPayloads()
    // Membuka scope metode BuildIngredientInventory_IgnoresMalformedPayloads; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // BuildIngredientInventory_IgnoresMalformedPayloads.
    {
        // Menyiapkan variabel lokal `events` untuk kumpulan event permainan sebagai sumber riwayat untuk validasi atau perhitungan dengan objek baru
        // bertipe `List<EventDb>` dengan nilai awal sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var events = new List<EventDb>
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // BuildIngredientInventory_IgnoresMalformedPayloads.
        {
            // Melanjutkan pengolahan dengan memanggil `BuildEvent` dengan `”BahanMasakan”`, `”””{”card_id”:”flour”,”amount”:2}”””` dalam
            // BuildIngredientInventory_IgnoresMalformedPayloads.
            BuildEvent("BahanMasakan", """{"card_id":"flour","amount":2}"""),
            // Melanjutkan pengolahan dengan memanggil `BuildEvent` dengan `”BahanMasakan”`, `”””{”card_id”:””,”amount”:5}”””` dalam
            // BuildIngredientInventory_IgnoresMalformedPayloads.
            BuildEvent("BahanMasakan", """{"card_id":"","amount":5}"""),
            // Melanjutkan pengolahan dengan memanggil `BuildEvent` dengan `”BuangBahanMasakan”`, `”””{”card_id”:”unknown”,”amount”:2}”””` dalam
            // BuildIngredientInventory_IgnoresMalformedPayloads.
            BuildEvent("BuangBahanMasakan", """{"card_id":"unknown","amount":2}""")
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // BuildIngredientInventory_IgnoresMalformedPayloads.
        };

        // Menyiapkan variabel lokal `inventory` untuk nilai inventory dengan memanggil `new IngredientInventoryCalculator().BuildIngredientInventory`
        // dengan `events`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var inventory = new IngredientInventoryCalculator().BuildIngredientInventory(events);

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`1`, `inventory.Total`); pengujian gagal jika
        // keduanya berbeda dalam BuildIngredientInventory_IgnoresMalformedPayloads.
        Assert.Equal(1, inventory.Total);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`1`, `inventory.ByCardId[”flour”]`); pengujian
        // gagal jika keduanya berbeda dalam BuildIngredientInventory_IgnoresMalformedPayloads.
        Assert.Equal(1, inventory.ByCardId["flour"]);
        // Menjalankan pemeriksaan bahwa `inventory.ByCardId.ContainsKey(”unknown”)` bernilai salah; pengujian gagal jika kondisi justru terpenuhi dalam
        // BuildIngredientInventory_IgnoresMalformedPayloads.
        Assert.False(inventory.ByCardId.ContainsKey("unknown"));
    // Menutup scope metode BuildIngredientInventory_IgnoresMalformedPayloads; bagian berikut berada di luar batas blok tersebut dalam
    // BuildIngredientInventory_IgnoresMalformedPayloads.
    }

    // Mendefinisikan metode `BuildEvent` dengan hasil bertipe `EventDb`; operasi ini menangani build event. Masukan: Parameter `actionType` bertipe
    // `string` membawa nilai aksi jenis; Parameter `payload` bertipe `string` membawa muatan detail event dalam format JSON.
    private static EventDb BuildEvent(string actionType, string payload)
    // Membuka scope metode BuildEvent; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildEvent.
    {
        // Mengembalikan objek baru bertipe `EventDb` dengan nilai awal sesuai konstruktornya kepada pemanggil dalam BuildEvent; eksekusi jalur ini selesai
        // setelah nilai hasil ditentukan.
        return new EventDb
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildEvent.
        {
            // Memperbarui `EventId` menggunakan memanggil `Guid.NewGuid` dengan tanpa argumen dalam BuildEvent.
            EventId = Guid.NewGuid(),
            // Memperbarui `ActionType` menggunakan `actionType` (nilai aksi jenis) dalam BuildEvent.
            ActionType = actionType,
            // Memperbarui `Payload` menggunakan `payload` (muatan detail event dalam format JSON) dalam BuildEvent.
            Payload = payload
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam BuildEvent.
        };
    // Menutup scope metode BuildEvent; bagian berikut berada di luar batas blok tersebut dalam BuildEvent.
    }
// Menutup scope tipe AnalyticsIngredientInventoryCalculatorTests; bagian berikut berada di luar batas blok tersebut.
}
