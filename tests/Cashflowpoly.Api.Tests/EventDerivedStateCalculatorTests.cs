// Fungsi file: Memverifikasi perilaku API, database, atau domain melalui EventDerivedStateCalculatorTests.
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;
// Mengimpor namespace `Cashflowpoly.Api.Domain` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Domain;
// Mengimpor namespace `Xunit` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Xunit;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Tests` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Tests;

// Mendefinisikan tipe class `EventDerivedStateCalculatorTests`; sealed mencegah tipe ini diturunkan lagi.
public sealed class EventDerivedStateCalculatorTests
// Membuka scope tipe EventDerivedStateCalculatorTests; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `BuildIngredientInventory_AppliesPurchasesOrderClaimsAndDiscards` dengan hasil bertipe `void`; operasi ini menangani build
    // bahan inventory applies purchases urutan/pesanan claims dan discards.
    public void BuildIngredientInventory_AppliesPurchasesOrderClaimsAndDiscards()
    // Membuka scope metode BuildIngredientInventory_AppliesPurchasesOrderClaimsAndDiscards; pernyataan/deklarasi berikut berada di dalam batas blok ini
    // dalam BuildIngredientInventory_AppliesPurchasesOrderClaimsAndDiscards.
    {
        // Menyiapkan variabel lokal `playerId` untuk nilai pemain identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var playerId = Guid.NewGuid();
        // Menyiapkan variabel lokal `otherPlayerId` untuk nilai other pemain identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var otherPlayerId = Guid.NewGuid();
        // Menyiapkan variabel lokal `events` untuk kumpulan event permainan sebagai sumber riwayat untuk validasi atau perhitungan dengan objek baru
        // bertipe `List<EventDb>` dengan nilai awal sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var events = new List<EventDb>
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // BuildIngredientInventory_AppliesPurchasesOrderClaimsAndDiscards.
        {
            // Melanjutkan pengolahan dengan memanggil `BuildEvent` dengan `playerId`, `”BahanMasakan”`, `”””{”card_id”:”flour”,”amount”:2}”””` dalam
            // BuildIngredientInventory_AppliesPurchasesOrderClaimsAndDiscards.
            BuildEvent(playerId, "BahanMasakan", """{"card_id":"flour","amount":2}"""),
            // Melanjutkan pengolahan dengan memanggil `BuildEvent` dengan `playerId`, `”BahanMasakan”`, `”””{”card_id”:”egg”,”amount”:1}”””` dalam
            // BuildIngredientInventory_AppliesPurchasesOrderClaimsAndDiscards.
            BuildEvent(playerId, "BahanMasakan", """{"card_id":"egg","amount":1}"""),
            // Melanjutkan pengolahan dengan memanggil `BuildEvent` dengan `playerId`, `”JualMasakan”`,
            // `”””{”required_ingredient_card_ids”:[”flour”,”egg”],”income”:8}”””` dalam BuildIngredientInventory_AppliesPurchasesOrderClaimsAndDiscards.
            BuildEvent(playerId, "JualMasakan", """{"required_ingredient_card_ids":["flour","egg"],"income":8}"""),
            // Melanjutkan pengolahan dengan memanggil `BuildEvent` dengan `playerId`, `”BuangBahanMasakan”`, `”””{”card_id”:”flour”,”amount”:1}”””` dalam
            // BuildIngredientInventory_AppliesPurchasesOrderClaimsAndDiscards.
            BuildEvent(playerId, "BuangBahanMasakan", """{"card_id":"flour","amount":1}"""),
            // Melanjutkan pengolahan dengan memanggil `BuildEvent` dengan `otherPlayerId`, `”BahanMasakan”`, `”””{”card_id”:”flour”,”amount”:10}”””` dalam
            // BuildIngredientInventory_AppliesPurchasesOrderClaimsAndDiscards.
            BuildEvent(otherPlayerId, "BahanMasakan", """{"card_id":"flour","amount":10}""")
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // BuildIngredientInventory_AppliesPurchasesOrderClaimsAndDiscards.
        };

        // Menyiapkan variabel lokal `inventory` untuk nilai inventory dengan memanggil `new EventDerivedStateCalculator().BuildIngredientInventory` dengan
        // `events`, `playerId`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var inventory = new EventDerivedStateCalculator().BuildIngredientInventory(events, playerId);

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`0`, `inventory.Total`); pengujian gagal jika
        // keduanya berbeda dalam BuildIngredientInventory_AppliesPurchasesOrderClaimsAndDiscards.
        Assert.Equal(0, inventory.Total);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`0`, `inventory.ByCardId[”flour”]`); pengujian
        // gagal jika keduanya berbeda dalam BuildIngredientInventory_AppliesPurchasesOrderClaimsAndDiscards.
        Assert.Equal(0, inventory.ByCardId["flour"]);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`0`, `inventory.ByCardId[”egg”]`); pengujian
        // gagal jika keduanya berbeda dalam BuildIngredientInventory_AppliesPurchasesOrderClaimsAndDiscards.
        Assert.Equal(0, inventory.ByCardId["egg"]);
    // Menutup scope metode BuildIngredientInventory_AppliesPurchasesOrderClaimsAndDiscards; bagian berikut berada di luar batas blok tersebut dalam
    // BuildIngredientInventory_AppliesPurchasesOrderClaimsAndDiscards.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `ComputeSavingBalance_AppliesDepositsWithdrawalsAndGoalCost` dengan hasil bertipe `void`; operasi ini menangani compute
    // tabungan saldo applies deposits withdrawals dan target biaya.
    public void ComputeSavingBalance_AppliesDepositsWithdrawalsAndGoalCost()
    // Membuka scope metode ComputeSavingBalance_AppliesDepositsWithdrawalsAndGoalCost; pernyataan/deklarasi berikut berada di dalam batas blok ini
    // dalam ComputeSavingBalance_AppliesDepositsWithdrawalsAndGoalCost.
    {
        // Menyiapkan variabel lokal `playerId` untuk nilai pemain identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var playerId = Guid.NewGuid();
        // Menyiapkan variabel lokal `events` untuk kumpulan event permainan sebagai sumber riwayat untuk validasi atau perhitungan dengan objek baru
        // bertipe `List<EventDb>` dengan nilai awal sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var events = new List<EventDb>
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ComputeSavingBalance_AppliesDepositsWithdrawalsAndGoalCost.
        {
            // Melanjutkan pengolahan dengan memanggil `BuildEvent` dengan `playerId`, `”Menabung”`, `”””{”goal_id”:”bike”,”amount”:10}”””` dalam
            // ComputeSavingBalance_AppliesDepositsWithdrawalsAndGoalCost.
            BuildEvent(playerId, "Menabung", """{"goal_id":"bike","amount":10}"""),
            // Melanjutkan pengolahan dengan memanggil `BuildEvent` dengan `playerId`, `”TarikTabungan”`, `”””{”goal_id”:”bike”,”amount”:3}”””` dalam
            // ComputeSavingBalance_AppliesDepositsWithdrawalsAndGoalCost.
            BuildEvent(playerId, "TarikTabungan", """{"goal_id":"bike","amount":3}"""),
            // Melanjutkan pengolahan dengan memanggil `BuildEvent` dengan `playerId`, `”TujuanFinansial”`, `”””{”goal_id”:”bike”,”points”:4,”cost”:5}”””` dalam
            // ComputeSavingBalance_AppliesDepositsWithdrawalsAndGoalCost.
            BuildEvent(playerId, "TujuanFinansial", """{"goal_id":"bike","points":4,"cost":5}"""),
            // Melanjutkan pengolahan dengan memanggil `BuildEvent` dengan `playerId`, `”Menabung”`, `”””{”goal_id”:”book”,”amount”:99}”””` dalam
            // ComputeSavingBalance_AppliesDepositsWithdrawalsAndGoalCost.
            BuildEvent(playerId, "Menabung", """{"goal_id":"book","amount":99}"""),
            // Melanjutkan pengolahan dengan memanggil `BuildEvent` dengan `Guid.NewGuid()`, `”Menabung”`, `”””{”goal_id”:”bike”,”amount”:99}”””` dalam
            // ComputeSavingBalance_AppliesDepositsWithdrawalsAndGoalCost.
            BuildEvent(Guid.NewGuid(), "Menabung", """{"goal_id":"bike","amount":99}""")
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // ComputeSavingBalance_AppliesDepositsWithdrawalsAndGoalCost.
        };

        // Menyiapkan variabel lokal `balance` untuk saldo uang pemain pada keadaan yang sedang diproses dengan memanggil `new
        // EventDerivedStateCalculator().ComputeSavingBalance` dengan `events`, `playerId`, `”bike”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var balance = new EventDerivedStateCalculator().ComputeSavingBalance(events, playerId, "bike");

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`2`, `balance`); pengujian gagal jika keduanya
        // berbeda dalam ComputeSavingBalance_AppliesDepositsWithdrawalsAndGoalCost.
        Assert.Equal(2, balance);
    // Menutup scope metode ComputeSavingBalance_AppliesDepositsWithdrawalsAndGoalCost; bagian berikut berada di luar batas blok tersebut dalam
    // ComputeSavingBalance_AppliesDepositsWithdrawalsAndGoalCost.
    }

    // Mendefinisikan metode `BuildEvent` dengan hasil bertipe `EventDb`; operasi ini menangani build event. Masukan: Parameter `playerId` bertipe
    // `Guid` membawa nilai pemain identitas; Parameter `actionType` bertipe `string` membawa nilai aksi jenis; Parameter `payload` bertipe `string`
    // membawa muatan detail event dalam format JSON.
    private static EventDb BuildEvent(Guid playerId, string actionType, string payload)
    // Membuka scope metode BuildEvent; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildEvent.
    {
        // Mengembalikan objek baru bertipe `EventDb` dengan nilai awal sesuai konstruktornya kepada pemanggil dalam BuildEvent; eksekusi jalur ini selesai
        // setelah nilai hasil ditentukan.
        return new EventDb
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildEvent.
        {
            // Memperbarui `EventId` menggunakan memanggil `Guid.NewGuid` dengan tanpa argumen dalam BuildEvent.
            EventId = Guid.NewGuid(),
            // Memperbarui `UserId` menggunakan `playerId` (nilai pemain identitas) dalam BuildEvent.
            UserId = playerId,
            // Memperbarui `ActionType` menggunakan `actionType` (nilai aksi jenis) dalam BuildEvent.
            ActionType = actionType,
            // Memperbarui `Payload` menggunakan `payload` (muatan detail event dalam format JSON) dalam BuildEvent.
            Payload = payload
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam BuildEvent.
        };
    // Menutup scope metode BuildEvent; bagian berikut berada di luar batas blok tersebut dalam BuildEvent.
    }
// Menutup scope tipe EventDerivedStateCalculatorTests; bagian berikut berada di luar batas blok tersebut.
}
