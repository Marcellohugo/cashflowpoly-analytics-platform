// Fungsi file: Memverifikasi perilaku API, database, atau domain melalui EventIngredientOrderValidatorTests.
// Mengimpor namespace `System.Text.Json` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.Json;
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;
// Mengimpor namespace `Cashflowpoly.Api.Domain` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Domain;
// Mengimpor namespace `Cashflowpoly.Api.Contracts` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Contracts;
// Mengimpor namespace `Microsoft.AspNetCore.Http` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.AspNetCore.Http;
// Mengimpor namespace `Xunit` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Xunit;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Tests` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Tests;

// Mendefinisikan tipe class `EventIngredientOrderValidatorTests`; sealed mencegah tipe ini diturunkan lagi.
public sealed class EventIngredientOrderValidatorTests
// Membuka scope tipe EventIngredientOrderValidatorTests; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `TryValidate_ReturnsFalseForUnhandledAction` dengan hasil bertipe `void`; operasi ini menangani try validate returns false
    // untuk unhandled aksi.
    public void TryValidate_ReturnsFalseForUnhandledAction()
    // Membuka scope metode TryValidate_ReturnsFalseForUnhandledAction; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // TryValidate_ReturnsFalseForUnhandledAction.
    {
        // Menyiapkan variabel lokal `request` untuk data masukan permintaan yang akan divalidasi atau diteruskan ke layanan dengan memanggil
        // `CreateRequest` dengan `”CatatTransaksi”`, `”””{”amount”:1}”””`, `Guid.NewGuid()`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var request = CreateRequest("CatatTransaksi", """{"amount":1}""", Guid.NewGuid());

        // Menyiapkan variabel lokal `handled` untuk nilai handled dengan memanggil `new EventIngredientOrderValidator().TryValidate` dengan `request`,
        // `CreateConfig()`, `Array.Empty<EventDb>()`, `var result`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var handled = new EventIngredientOrderValidator().TryValidate(request, CreateConfig(), Array.Empty<EventDb>(), out var result);

        // Menjalankan pemeriksaan bahwa `handled` bernilai salah; pengujian gagal jika kondisi justru terpenuhi dalam
        // TryValidate_ReturnsFalseForUnhandledAction.
        Assert.False(handled);
        // Menjalankan pemeriksaan bahwa `result.Validation.IsValid` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // TryValidate_ReturnsFalseForUnhandledAction.
        Assert.True(result.Validation.IsValid);
        // Menjalankan pemeriksaan Null atas `result.OutgoingAmount` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // TryValidate_ReturnsFalseForUnhandledAction.
        Assert.Null(result.OutgoingAmount);
    // Menutup scope metode TryValidate_ReturnsFalseForUnhandledAction; bagian berikut berada di luar batas blok tersebut dalam
    // TryValidate_ReturnsFalseForUnhandledAction.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `TryValidatePurchase_RejectsIngredientTotalLimit` dengan hasil bertipe `void`; operasi ini menangani try validate pembelian
    // rejects bahan total limit.
    public void TryValidatePurchase_RejectsIngredientTotalLimit()
    // Membuka scope metode TryValidatePurchase_RejectsIngredientTotalLimit; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // TryValidatePurchase_RejectsIngredientTotalLimit.
    {
        // Menyiapkan variabel lokal `playerId` untuk nilai pemain identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var playerId = Guid.NewGuid();
        // Menyiapkan variabel lokal `request` untuk data masukan permintaan yang akan divalidasi atau diteruskan ke layanan dengan memanggil
        // `CreateRequest` dengan `”BahanMasakan”`, `”””{”card_id”:”flour”,”amount”:2}”””`, `playerId`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var request = CreateRequest("BahanMasakan", """{"card_id":"flour","amount":2}""", playerId);
        // Menyiapkan variabel lokal `history` untuk nilai history dengan array baru dengan tipe elemen disimpulkan dari nilai initializer. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var history = new[]
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // TryValidatePurchase_RejectsIngredientTotalLimit.
        {
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `”BahanMasakan”`, `”””{”card_id”:”egg”,”amount”:2}”””`, `playerId` dalam
            // TryValidatePurchase_RejectsIngredientTotalLimit.
            CreateEvent("BahanMasakan", """{"card_id":"egg","amount":2}""", playerId),
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `”BahanMasakan”`, `”””{”card_id”:”egg”,”amount”:2}”””`, `playerId` dalam
            // TryValidatePurchase_RejectsIngredientTotalLimit.
            CreateEvent("BahanMasakan", """{"card_id":"egg","amount":2}""", playerId),
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `”BahanMasakan”`, `”””{”card_id”:”egg”,”amount”:2}”””`, `playerId` dalam
            // TryValidatePurchase_RejectsIngredientTotalLimit.
            CreateEvent("BahanMasakan", """{"card_id":"egg","amount":2}""", playerId)
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // TryValidatePurchase_RejectsIngredientTotalLimit.
        };

        // Menyiapkan variabel lokal `handled` untuk nilai handled dengan memanggil `new EventIngredientOrderValidator().TryValidate` dengan `request`,
        // `CreateConfig(maxIngredientTotal: 3)`, `history`, `var result`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var handled = new EventIngredientOrderValidator().TryValidate(request, CreateConfig(maxIngredientTotal: 3), history, out var result);

        // Menjalankan pemeriksaan bahwa `handled` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // TryValidatePurchase_RejectsIngredientTotalLimit.
        Assert.True(handled);
        // Menjalankan pemeriksaan bahwa `result.Validation.IsValid` bernilai salah; pengujian gagal jika kondisi justru terpenuhi dalam
        // TryValidatePurchase_RejectsIngredientTotalLimit.
        Assert.False(result.Validation.IsValid);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`StatusCodes.Status422UnprocessableEntity`,
        // `result.Validation.StatusCode`); pengujian gagal jika keduanya berbeda dalam TryValidatePurchase_RejectsIngredientTotalLimit.
        Assert.Equal(StatusCodes.Status422UnprocessableEntity, result.Validation.StatusCode);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”Total kartu bahan melebihi batas ruleset”`,
        // `result.Validation.Message`); pengujian gagal jika keduanya berbeda dalam TryValidatePurchase_RejectsIngredientTotalLimit.
        Assert.Equal("Total kartu bahan melebihi batas ruleset", result.Validation.Message);
        // Menjalankan pemeriksaan Null atas `result.OutgoingAmount` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // TryValidatePurchase_RejectsIngredientTotalLimit.
        Assert.Null(result.OutgoingAmount);
    // Menutup scope metode TryValidatePurchase_RejectsIngredientTotalLimit; bagian berikut berada di luar batas blok tersebut dalam
    // TryValidatePurchase_RejectsIngredientTotalLimit.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `TryValidateDiscard_RejectsDiscardAboveStock` dengan hasil bertipe `void`; operasi ini menangani try validate discard
    // rejects discard above stock.
    public void TryValidateDiscard_RejectsDiscardAboveStock()
    // Membuka scope metode TryValidateDiscard_RejectsDiscardAboveStock; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // TryValidateDiscard_RejectsDiscardAboveStock.
    {
        // Menyiapkan variabel lokal `playerId` untuk nilai pemain identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var playerId = Guid.NewGuid();
        // Menyiapkan variabel lokal `request` untuk data masukan permintaan yang akan divalidasi atau diteruskan ke layanan dengan memanggil
        // `CreateRequest` dengan `”BuangBahanMasakan”`, `”””{”card_id”:”flour”,”amount”:2}”””`, `playerId`. Tipe variabel disimpulkan dari ekspresi nilai
        // awal.
        var request = CreateRequest("BuangBahanMasakan", """{"card_id":"flour","amount":2}""", playerId);
        // Menyiapkan variabel lokal `history` untuk nilai history dengan array baru dengan tipe elemen disimpulkan dari nilai initializer. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var history = new[]
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // TryValidateDiscard_RejectsDiscardAboveStock.
        {
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `”BahanMasakan”`, `”””{”card_id”:”flour”,”amount”:2}”””`, `playerId` dalam
            // TryValidateDiscard_RejectsDiscardAboveStock.
            CreateEvent("BahanMasakan", """{"card_id":"flour","amount":2}""", playerId)
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // TryValidateDiscard_RejectsDiscardAboveStock.
        };

        // Menyiapkan variabel lokal `handled` untuk nilai handled dengan memanggil `new EventIngredientOrderValidator().TryValidate` dengan `request`,
        // `CreateConfig()`, `history`, `var result`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var handled = new EventIngredientOrderValidator().TryValidate(request, CreateConfig(), history, out var result);

        // Menjalankan pemeriksaan bahwa `handled` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // TryValidateDiscard_RejectsDiscardAboveStock.
        Assert.True(handled);
        // Menjalankan pemeriksaan bahwa `result.Validation.IsValid` bernilai salah; pengujian gagal jika kondisi justru terpenuhi dalam
        // TryValidateDiscard_RejectsDiscardAboveStock.
        Assert.False(result.Validation.IsValid);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”Jumlah discard melebihi stok bahan”`,
        // `result.Validation.Message`); pengujian gagal jika keduanya berbeda dalam TryValidateDiscard_RejectsDiscardAboveStock.
        Assert.Equal("Jumlah discard melebihi stok bahan", result.Validation.Message);
    // Menutup scope metode TryValidateDiscard_RejectsDiscardAboveStock; bagian berikut berada di luar batas blok tersebut dalam
    // TryValidateDiscard_RejectsDiscardAboveStock.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `TryValidateOrderClaim_ReturnsValidWhenInventoryCoversRequiredCards` dengan hasil bertipe `void`; operasi ini menangani try
    // validate urutan/pesanan claim returns valid when inventory covers required kartu.
    public void TryValidateOrderClaim_ReturnsValidWhenInventoryCoversRequiredCards()
    // Membuka scope metode TryValidateOrderClaim_ReturnsValidWhenInventoryCoversRequiredCards; pernyataan/deklarasi berikut berada di dalam batas blok
    // ini dalam TryValidateOrderClaim_ReturnsValidWhenInventoryCoversRequiredCards.
    {
        // Menyiapkan variabel lokal `playerId` untuk nilai pemain identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var playerId = Guid.NewGuid();
        // Menyiapkan variabel lokal `request` untuk data masukan permintaan yang akan divalidasi atau diteruskan ke layanan dengan memanggil
        // `CreateRequest` dengan `”JualMasakan”`, `”””{”order_card_id”:”bread”}”””`, `playerId`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var request = CreateRequest("JualMasakan", """{"order_card_id":"bread"}""", playerId);
        // Menyiapkan variabel lokal `history` untuk nilai history dengan array baru dengan tipe elemen disimpulkan dari nilai initializer. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var history = new[]
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // TryValidateOrderClaim_ReturnsValidWhenInventoryCoversRequiredCards.
        {
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `”BahanMasakan”`, `”””{”card_id”:”flour”,”amount”:2}”””`, `playerId` dalam
            // TryValidateOrderClaim_ReturnsValidWhenInventoryCoversRequiredCards.
            CreateEvent("BahanMasakan", """{"card_id":"flour","amount":2}""", playerId),
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `”BahanMasakan”`, `”””{”card_id”:”egg”,”amount”:2}”””`, `playerId` dalam
            // TryValidateOrderClaim_ReturnsValidWhenInventoryCoversRequiredCards.
            CreateEvent("BahanMasakan", """{"card_id":"egg","amount":2}""", playerId)
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // TryValidateOrderClaim_ReturnsValidWhenInventoryCoversRequiredCards.
        };

        // Menyiapkan variabel lokal `handled` untuk nilai handled dengan memanggil `new EventIngredientOrderValidator().TryValidate` dengan `request`,
        // `CreateConfig()`, `history`, `var result`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var handled = new EventIngredientOrderValidator().TryValidate(request, CreateConfig(), history, out var result);

        // Menjalankan pemeriksaan bahwa `handled` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // TryValidateOrderClaim_ReturnsValidWhenInventoryCoversRequiredCards.
        Assert.True(handled);
        // Menjalankan pemeriksaan bahwa `result.Validation.IsValid` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // TryValidateOrderClaim_ReturnsValidWhenInventoryCoversRequiredCards.
        Assert.True(result.Validation.IsValid);
        // Menjalankan pemeriksaan Null atas `result.OutgoingAmount` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // TryValidateOrderClaim_ReturnsValidWhenInventoryCoversRequiredCards.
        Assert.Null(result.OutgoingAmount);
    // Menutup scope metode TryValidateOrderClaim_ReturnsValidWhenInventoryCoversRequiredCards; bagian berikut berada di luar batas blok tersebut dalam
    // TryValidateOrderClaim_ReturnsValidWhenInventoryCoversRequiredCards.
    }

    // Mendefinisikan metode `CreateRequest` dengan hasil bertipe `EventRequest`; operasi ini menangani create permintaan. Masukan: Parameter
    // `actionType` bertipe `string` membawa nilai aksi jenis; Parameter `payloadJson` bertipe `string` membawa nilai payload JSON; Parameter `playerId`
    // bertipe `Guid?` membawa nilai pemain identitas; nilai null diizinkan ketika data opsional belum tersedia.
    private static EventRequest CreateRequest(string actionType, string payloadJson, Guid? playerId)
    // Membuka scope metode CreateRequest; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CreateRequest.
    {
        // Menyiapkan variabel lokal `document` untuk nilai document dengan memanggil `JsonDocument.Parse` dengan `payloadJson`. Tipe variabel disimpulkan
        // dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var document = JsonDocument.Parse(payloadJson);
        // Mengembalikan objek baru bertipe `EventRequest` dengan argumen ( Guid.NewGuid(), Guid.NewGuid(), playerId, ”PLAYER”, new DateTimeOffset(2026, 1,
        // 2, 3, 4, 5, TimeSpan.Zero), 0, ”MON”, 1, 0, actionType, Guid.NewGuid(), docume... kepada pemanggil dalam CreateRequest; eksekusi jalur ini
        // selesai setelah nilai hasil ditentukan.
        return new EventRequest(
            // Meneruskan memanggil `Guid.NewGuid` dengan tanpa argumen sebagai argumen ke konstruktor `EventRequest`.
            Guid.NewGuid(),
            // Meneruskan memanggil `Guid.NewGuid` dengan tanpa argumen sebagai argumen ke konstruktor `EventRequest`.
            Guid.NewGuid(),
            // Meneruskan `playerId` (nilai pemain identitas) sebagai argumen ke konstruktor `EventRequest`.
            playerId,
            // Meneruskan nilai literal `”PLAYER”` sebagai argumen ke konstruktor `EventRequest`.
            "PLAYER",
            // Meneruskan objek baru bertipe `DateTimeOffset` dengan argumen (2026, 1, 2, 3, 4, 5, TimeSpan.Zero) sebagai argumen ke konstruktor `EventRequest`;
            // Meneruskan nilai literal `2026` sebagai argumen ke konstruktor `DateTimeOffset`; Meneruskan nilai literal `1` sebagai argumen ke konstruktor
            // `DateTimeOffset`; Meneruskan nilai literal `2` sebagai argumen ke konstruktor `DateTimeOffset`; Meneruskan nilai literal `3` sebagai argumen ke
            // konstruktor `DateTimeOffset`; Meneruskan nilai literal `4` sebagai argumen ke konstruktor `DateTimeOffset`; Meneruskan nilai literal `5` sebagai
            // argumen ke konstruktor `DateTimeOffset`; Meneruskan `TimeSpan.Zero` (nilai zero) sebagai argumen ke konstruktor `DateTimeOffset`.
            new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero),
            // Meneruskan nilai literal `0` sebagai argumen ke konstruktor `EventRequest`.
            0,
            // Meneruskan nilai literal `”MON”` sebagai argumen ke konstruktor `EventRequest`.
            "MON",
            // Meneruskan nilai literal `1` sebagai argumen ke konstruktor `EventRequest`.
            1,
            // Meneruskan nilai literal `0` sebagai argumen ke konstruktor `EventRequest`.
            0,
            // Meneruskan `actionType` (nilai aksi jenis) sebagai argumen ke konstruktor `EventRequest`.
            actionType,
            // Meneruskan memanggil `Guid.NewGuid` dengan tanpa argumen sebagai argumen ke konstruktor `EventRequest`.
            Guid.NewGuid(),
            // Meneruskan membuat salinan `document.RootElement` agar hasil dapat digunakan terpisah dari objek sumber sebagai argumen ke konstruktor
            // `EventRequest`.
            document.RootElement.Clone(),
            // Meneruskan nilai literal `”client-123”` sebagai argumen ke konstruktor `EventRequest`.
            "client-123");
    // Menutup scope metode CreateRequest; bagian berikut berada di luar batas blok tersebut dalam CreateRequest.
    }

    // Mendefinisikan metode `CreateEvent` dengan hasil bertipe `EventDb`; operasi ini menangani create event. Masukan: Parameter `actionType` bertipe
    // `string` membawa nilai aksi jenis; Parameter `payloadJson` bertipe `string` membawa nilai payload JSON; Parameter `playerId` bertipe `Guid`
    // membawa nilai pemain identitas.
    private static EventDb CreateEvent(string actionType, string payloadJson, Guid playerId)
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
            // Memperbarui `Weekday` menggunakan nilai literal `”MON”` dalam CreateEvent.
            Weekday = "MON",
            // Memperbarui `ActionSlot` menggunakan nilai literal `1` dalam CreateEvent.
            ActionSlot = 1,
            // Memperbarui `SequenceNumber` menggunakan nilai literal `1` dalam CreateEvent.
            SequenceNumber = 1,
            // Memperbarui `ActionType` menggunakan `actionType` (nilai aksi jenis) dalam CreateEvent.
            ActionType = actionType,
            // Memperbarui `RulesetVersionId` menggunakan memanggil `Guid.NewGuid` dengan tanpa argumen dalam CreateEvent.
            RulesetVersionId = Guid.NewGuid(),
            // Memperbarui `Payload` menggunakan `payloadJson` (nilai payload JSON) dalam CreateEvent.
            Payload = payloadJson
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam CreateEvent.
        };
    // Menutup scope metode CreateEvent; bagian berikut berada di luar batas blok tersebut dalam CreateEvent.
    }

    // Mendefinisikan metode `CreateConfig` dengan hasil bertipe `RulesetConfig`; operasi ini menangani create konfigurasi. Masukan: Parameter
    // `maxIngredientTotal` bertipe `int` membawa nilai maksimum bahan total; bila argumen tidak diberikan digunakan nilai literal `10`; Parameter
    // `maxSameIngredient` bertipe `int` membawa nilai maksimum same bahan; bila argumen tidak diberikan digunakan nilai literal `5`.
    private static RulesetConfig CreateConfig(int maxIngredientTotal = 10, int maxSameIngredient = 5)
    // Membuka scope metode CreateConfig; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CreateConfig.
    {
        // Mengembalikan objek baru bertipe `RulesetConfig` dengan argumen ( ”PEMULA”, ActionsPerTurn: 3, StartingCash: 20, PlayerOrdering.PlayerOrder,
        // CashMin: 0, maxIngredientTotal, maxSameIngredient, PrimaryNeedMaxPerDay: 1, Require... kepada pemanggil dalam CreateConfig; eksekusi jalur ini
        // selesai setelah nilai hasil ditentukan.
        return new RulesetConfig(
            // Meneruskan nilai literal `”PEMULA”` sebagai argumen ke konstruktor `RulesetConfig`.
            "PEMULA",
            // Meneruskan nilai literal `3` sebagai argumen bernama `ActionsPerTurn`.
            ActionsPerTurn: 3,
            // Meneruskan nilai literal `20` sebagai argumen bernama `StartingCash`.
            StartingCash: 20,
            // Meneruskan `PlayerOrdering.PlayerOrder` (nomor urut pemain untuk menentukan urutan tindakan) sebagai argumen ke konstruktor `RulesetConfig`.
            PlayerOrdering.PlayerOrder,
            // Meneruskan nilai literal `0` sebagai argumen bernama `CashMin`.
            CashMin: 0,
            // Meneruskan `maxIngredientTotal` (nilai maksimum bahan total) sebagai argumen ke konstruktor `RulesetConfig`.
            maxIngredientTotal,
            // Meneruskan `maxSameIngredient` (nilai maksimum same bahan) sebagai argumen ke konstruktor `RulesetConfig`.
            maxSameIngredient,
            // Meneruskan nilai literal `1` sebagai argumen bernama `PrimaryNeedMaxPerDay`.
            PrimaryNeedMaxPerDay: 1,
            // Meneruskan true, yaitu kondisi aktif/terpenuhi sebagai argumen bernama `RequirePrimaryBeforeOthers`.
            RequirePrimaryBeforeOthers: true,
            // Meneruskan true, yaitu kondisi aktif/terpenuhi sebagai argumen bernama `FridayEnabled`.
            FridayEnabled: true,
            // Meneruskan true, yaitu kondisi aktif/terpenuhi sebagai argumen bernama `SaturdayEnabled`.
            SaturdayEnabled: true,
            // Meneruskan true, yaitu kondisi aktif/terpenuhi sebagai argumen bernama `SundayEnabled`.
            SundayEnabled: true,
            // Meneruskan nilai literal `1` sebagai argumen bernama `DonationMin`.
            DonationMin: 1,
            // Meneruskan nilai literal `10` sebagai argumen bernama `DonationMax`.
            DonationMax: 10,
            // Meneruskan true, yaitu kondisi aktif/terpenuhi sebagai argumen bernama `GoldAllowBuy`.
            GoldAllowBuy: true,
            // Meneruskan true, yaitu kondisi aktif/terpenuhi sebagai argumen bernama `GoldAllowSell`.
            GoldAllowSell: true,
            // Meneruskan false, yaitu kondisi nonaktif/tidak terpenuhi sebagai argumen bernama `LoanEnabled`.
            LoanEnabled: false,
            // Meneruskan false, yaitu kondisi nonaktif/tidak terpenuhi sebagai argumen bernama `InsuranceEnabled`.
            InsuranceEnabled: false,
            // Meneruskan false, yaitu kondisi nonaktif/tidak terpenuhi sebagai argumen bernama `SavingGoalEnabled`.
            SavingGoalEnabled: false,
            // Meneruskan nilai literal `5` sebagai argumen bernama `FreelanceIncome`.
            FreelanceIncome: 5,
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen bernama `Scoring`.
            Scoring: null)
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CreateConfig.
        {
            // Memperbarui `Ingredients` menggunakan objek baru bertipe `List<RulesetIngredientDto>` dengan nilai awal sesuai konstruktornya dalam CreateConfig.
            Ingredients = new List<RulesetIngredientDto>
            // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CreateConfig.
            {
                // Menggunakan objek baru dengan tipe mengikuti konteks tujuan dan argumen () sebagai bagian ekspresi yang sedang disusun dalam CreateConfig.
                new() { Id = "flour", Nama = "Flour", HargaBeli = 2 },
                // Menggunakan objek baru dengan tipe mengikuti konteks tujuan dan argumen () sebagai bagian ekspresi yang sedang disusun dalam CreateConfig.
                new() { Id = "egg", Nama = "Egg", HargaBeli = 2 }
            // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam CreateConfig.
            },
            // Memperbarui `Orders` menggunakan objek baru bertipe `List<RulesetOrderDto>` dengan nilai awal sesuai konstruktornya dalam CreateConfig.
            Orders = new List<RulesetOrderDto>
            // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CreateConfig.
            {
                // Menggunakan objek baru dengan tipe mengikuti konteks tujuan dan argumen () sebagai bagian ekspresi yang sedang disusun dalam CreateConfig.
                new() { Id = "bread", Nama = "Bread", HargaJual = 8, Bahan = new List<string> { "Flour", "Egg" } }
            // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam CreateConfig.
            }
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam CreateConfig.
        };
    // Menutup scope metode CreateConfig; bagian berikut berada di luar batas blok tersebut dalam CreateConfig.
    }
// Menutup scope tipe EventIngredientOrderValidatorTests; bagian berikut berada di luar batas blok tersebut.
}
