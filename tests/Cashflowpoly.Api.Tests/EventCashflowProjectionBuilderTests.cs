// Fungsi file: Memverifikasi perilaku API, database, atau domain melalui EventCashflowProjectionBuilderTests.
// Mengimpor namespace `System.Text.Json` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.Json;
// Mengimpor namespace `Cashflowpoly.Api.Domain` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Domain;
// Mengimpor namespace `Cashflowpoly.Api.Contracts` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Contracts;
// Mengimpor namespace `Xunit` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Xunit;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Tests` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Tests;

// Mendefinisikan tipe class `EventCashflowProjectionBuilderTests`; sealed mencegah tipe ini diturunkan lagi.
public sealed class EventCashflowProjectionBuilderTests
// Membuka scope tipe EventCashflowProjectionBuilderTests; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `TryBuild_ReturnsFalse_WhenEventHasNoPlayer` dengan hasil bertipe `void`; operasi ini menangani try build returns false
    // when event memiliki no pemain.
    public void TryBuild_ReturnsFalse_WhenEventHasNoPlayer()
    // Membuka scope metode TryBuild_ReturnsFalse_WhenEventHasNoPlayer; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // TryBuild_ReturnsFalse_WhenEventHasNoPlayer.
    {
        // Menyiapkan variabel lokal `request` untuk data masukan permintaan yang akan divalidasi atau diteruskan ke layanan dengan memanggil `BuildRequest`
        // dengan `false`, `”JumatBerkah”`, `”””{”amount”:5}”””`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var request = BuildRequest(
            // Meneruskan false, yaitu kondisi nonaktif/tidak terpenuhi sebagai argumen bernama `includePlayer`.
            includePlayer: false,
            // Meneruskan nilai literal `”JumatBerkah”` sebagai argumen bernama `actionType`.
            actionType: "JumatBerkah",
            // Meneruskan nilai literal `”””{”amount”:5}”””` sebagai argumen bernama `payloadJson`.
            payloadJson: """{"amount":5}""");

        // Menyiapkan variabel lokal `ok` untuk nilai ok dengan memanggil `new EventCashflowProjectionBuilder().TryBuild` dengan `request`,
        // `DateTimeOffset.Parse(”2026-01-02T03:04:05Z”)`, `Guid.NewGuid()`, `var projection`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var ok = new EventCashflowProjectionBuilder().TryBuild(
            // Meneruskan `request` (data masukan permintaan yang akan divalidasi atau diteruskan ke layanan) sebagai argumen ke `new
            // EventCashflowProjectionBuilder().TryBuild`.
            request,
            // Meneruskan memanggil `DateTimeOffset.Parse` dengan `”2026-01-02T03:04:05Z”` sebagai argumen ke `new EventCashflowProjectionBuilder().TryBuild`;
            // Meneruskan nilai literal `”2026-01-02T03:04:05Z”` sebagai argumen ke `DateTimeOffset.Parse`.
            DateTimeOffset.Parse("2026-01-02T03:04:05Z"),
            // Meneruskan memanggil `Guid.NewGuid` dengan tanpa argumen sebagai argumen ke `new EventCashflowProjectionBuilder().TryBuild`.
            Guid.NewGuid(),
            // Meneruskan `var projection` sebagai argumen ke `new EventCashflowProjectionBuilder().TryBuild`.
            out var projection);

        // Menjalankan pemeriksaan bahwa `ok` bernilai salah; pengujian gagal jika kondisi justru terpenuhi dalam
        // TryBuild_ReturnsFalse_WhenEventHasNoPlayer.
        Assert.False(ok);
        // Menjalankan pemeriksaan Null atas `projection` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // TryBuild_ReturnsFalse_WhenEventHasNoPlayer.
        Assert.Null(projection);
    // Menutup scope metode TryBuild_ReturnsFalse_WhenEventHasNoPlayer; bagian berikut berada di luar batas blok tersebut dalam
    // TryBuild_ReturnsFalse_WhenEventHasNoPlayer.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `TryBuild_GoldSellCreatesIncomingGoldTradeProjection` dengan hasil bertipe `void`; operasi ini menangani try build emas
    // sell creates incoming emas trade projection.
    public void TryBuild_GoldSellCreatesIncomingGoldTradeProjection()
    // Membuka scope metode TryBuild_GoldSellCreatesIncomingGoldTradeProjection; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // TryBuild_GoldSellCreatesIncomingGoldTradeProjection.
    {
        // Menyiapkan variabel lokal `playerId` untuk nilai pemain identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var playerId = Guid.NewGuid();
        // Menyiapkan variabel lokal `sessionId` untuk identitas unik sesi permainan yang menjadi batas data operasi ini dengan memanggil `Guid.NewGuid`
        // dengan tanpa argumen. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var sessionId = Guid.NewGuid();
        // Menyiapkan variabel lokal `eventId` untuk identitas unik event untuk pencatatan dan pemeriksaan duplikasi dengan memanggil `Guid.NewGuid` dengan
        // tanpa argumen. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var eventId = Guid.NewGuid();
        // Menyiapkan variabel lokal `eventPk` untuk nilai event pk dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var eventPk = Guid.NewGuid();
        // Menyiapkan variabel lokal `timestamp` untuk waktu kejadian yang menjaga urutan kronologis data dengan memanggil `DateTimeOffset.Parse` dengan
        // `”2026-01-02T03:04:05Z”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var timestamp = DateTimeOffset.Parse("2026-01-02T03:04:05Z");
        // Menyiapkan variabel lokal `request` untuk data masukan permintaan yang akan divalidasi atau diteruskan ke layanan dengan memanggil `BuildRequest`
        // dengan `sessionId`, `playerId`, `eventId`, `”JualEmas”`, `”””{”trade_type”:”SELL”,”qty”:2,”unit_price”:6,”amount”:12}”””`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var request = BuildRequest(
            // Meneruskan `sessionId` (identitas unik sesi permainan yang menjadi batas data operasi ini) sebagai argumen bernama `sessionId`.
            sessionId: sessionId,
            // Meneruskan `playerId` (nilai pemain identitas) sebagai argumen bernama `playerId`.
            playerId: playerId,
            // Meneruskan `eventId` (identitas unik event untuk pencatatan dan pemeriksaan duplikasi) sebagai argumen bernama `eventId`.
            eventId: eventId,
            // Meneruskan nilai literal `”JualEmas”` sebagai argumen bernama `actionType`.
            actionType: "JualEmas",
            // Meneruskan nilai literal `”””{”trade_type”:”SELL”,”qty”:2,”unit_price”:6,”amount”:12}”””` sebagai argumen bernama `payloadJson`.
            payloadJson: """{"trade_type":"SELL","qty":2,"unit_price":6,"amount":12}""");

        // Menyiapkan variabel lokal `ok` untuk nilai ok dengan memanggil `new EventCashflowProjectionBuilder().TryBuild` dengan `request`, `timestamp`,
        // `eventPk`, `var projection`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var ok = new EventCashflowProjectionBuilder().TryBuild(request, timestamp, eventPk, out var projection);

        // Menjalankan pemeriksaan bahwa `ok` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // TryBuild_GoldSellCreatesIncomingGoldTradeProjection.
        Assert.True(ok);
        // Menjalankan pemeriksaan NotNull atas `projection` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // TryBuild_GoldSellCreatesIncomingGoldTradeProjection.
        Assert.NotNull(projection);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`sessionId`, `projection.SessionId`); pengujian
        // gagal jika keduanya berbeda dalam TryBuild_GoldSellCreatesIncomingGoldTradeProjection.
        Assert.Equal(sessionId, projection.SessionId);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`playerId`, `projection.UserId`); pengujian
        // gagal jika keduanya berbeda dalam TryBuild_GoldSellCreatesIncomingGoldTradeProjection.
        Assert.Equal(playerId, projection.UserId);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`eventPk`, `projection.EventPk`); pengujian
        // gagal jika keduanya berbeda dalam TryBuild_GoldSellCreatesIncomingGoldTradeProjection.
        Assert.Equal(eventPk, projection.EventPk);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`eventId`, `projection.EventId`); pengujian
        // gagal jika keduanya berbeda dalam TryBuild_GoldSellCreatesIncomingGoldTradeProjection.
        Assert.Equal(eventId, projection.EventId);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`timestamp`, `projection.Timestamp`); pengujian
        // gagal jika keduanya berbeda dalam TryBuild_GoldSellCreatesIncomingGoldTradeProjection.
        Assert.Equal(timestamp, projection.Timestamp);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”IN”`, `projection.Direction`); pengujian
        // gagal jika keduanya berbeda dalam TryBuild_GoldSellCreatesIncomingGoldTradeProjection.
        Assert.Equal("IN", projection.Direction);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`12`, `projection.Amount`); pengujian gagal
        // jika keduanya berbeda dalam TryBuild_GoldSellCreatesIncomingGoldTradeProjection.
        Assert.Equal(12, projection.Amount);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”GOLD_TRADE”`, `projection.Category`);
        // pengujian gagal jika keduanya berbeda dalam TryBuild_GoldSellCreatesIncomingGoldTradeProjection.
        Assert.Equal("GOLD_TRADE", projection.Category);
    // Menutup scope metode TryBuild_GoldSellCreatesIncomingGoldTradeProjection; bagian berikut berada di luar batas blok tersebut dalam
    // TryBuild_GoldSellCreatesIncomingGoldTradeProjection.
    }

    // menandai metode sebagai pengujian xUnit yang dijalankan untuk setiap kombinasi data.
    [Theory]
    // menyediakan satu kombinasi masukan pengujian (”InvestasiEmas”, ”OUT”).
    [InlineData("InvestasiEmas", "OUT")]
    // menyediakan satu kombinasi masukan pengujian (”JualEmas”, ”IN”).
    [InlineData("JualEmas", "IN")]
    // Mendefinisikan metode `TryBuild_GoldGameActionsUseFixedCashflowDirection` dengan hasil bertipe `void`; operasi ini menangani try build emas game
    // aksi use fixed arus kas direction. Masukan: Parameter `actionType` bertipe `string` membawa nilai aksi jenis; Parameter `expectedDirection`
    // bertipe `string` membawa nilai yang diharapkan direction.
    public void TryBuild_GoldGameActionsUseFixedCashflowDirection(string actionType, string expectedDirection)
    // Membuka scope metode TryBuild_GoldGameActionsUseFixedCashflowDirection; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // TryBuild_GoldGameActionsUseFixedCashflowDirection.
    {
        // Menyiapkan variabel lokal `request` untuk data masukan permintaan yang akan divalidasi atau diteruskan ke layanan dengan memanggil `BuildRequest`
        // dengan `actionType`, `”””{”qty”:2,”unit_price”:6,”amount”:12}”””`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var request = BuildRequest(
            // Meneruskan `actionType` (nilai aksi jenis) sebagai argumen bernama `actionType`.
            actionType: actionType,
            // Meneruskan nilai literal `”””{”qty”:2,”unit_price”:6,”amount”:12}”””` sebagai argumen bernama `payloadJson`.
            payloadJson: """{"qty":2,"unit_price":6,"amount":12}""");

        // Menyiapkan variabel lokal `ok` untuk nilai ok dengan memanggil `new EventCashflowProjectionBuilder().TryBuild` dengan `request`,
        // `DateTimeOffset.Parse(”2026-01-02T03:04:05Z”)`, `Guid.NewGuid()`, `var projection`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var ok = new EventCashflowProjectionBuilder().TryBuild(
            // Meneruskan `request` (data masukan permintaan yang akan divalidasi atau diteruskan ke layanan) sebagai argumen ke `new
            // EventCashflowProjectionBuilder().TryBuild`.
            request,
            // Meneruskan memanggil `DateTimeOffset.Parse` dengan `”2026-01-02T03:04:05Z”` sebagai argumen ke `new EventCashflowProjectionBuilder().TryBuild`;
            // Meneruskan nilai literal `”2026-01-02T03:04:05Z”` sebagai argumen ke `DateTimeOffset.Parse`.
            DateTimeOffset.Parse("2026-01-02T03:04:05Z"),
            // Meneruskan memanggil `Guid.NewGuid` dengan tanpa argumen sebagai argumen ke `new EventCashflowProjectionBuilder().TryBuild`.
            Guid.NewGuid(),
            // Meneruskan `var projection` sebagai argumen ke `new EventCashflowProjectionBuilder().TryBuild`.
            out var projection);

        // Menjalankan pemeriksaan bahwa `ok` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // TryBuild_GoldGameActionsUseFixedCashflowDirection.
        Assert.True(ok);
        // Menjalankan pemeriksaan NotNull atas `projection` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // TryBuild_GoldGameActionsUseFixedCashflowDirection.
        Assert.NotNull(projection);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`expectedDirection`, `projection.Direction`);
        // pengujian gagal jika keduanya berbeda dalam TryBuild_GoldGameActionsUseFixedCashflowDirection.
        Assert.Equal(expectedDirection, projection.Direction);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`12`, `projection.Amount`); pengujian gagal
        // jika keduanya berbeda dalam TryBuild_GoldGameActionsUseFixedCashflowDirection.
        Assert.Equal(12, projection.Amount);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”GOLD_TRADE”`, `projection.Category`);
        // pengujian gagal jika keduanya berbeda dalam TryBuild_GoldGameActionsUseFixedCashflowDirection.
        Assert.Equal("GOLD_TRADE", projection.Category);
    // Menutup scope metode TryBuild_GoldGameActionsUseFixedCashflowDirection; bagian berikut berada di luar batas blok tersebut dalam
    // TryBuild_GoldGameActionsUseFixedCashflowDirection.
    }

    // menandai metode sebagai pengujian xUnit yang dijalankan untuk setiap kombinasi data.
    [Theory]
    // menyediakan satu kombinasi masukan pengujian (”BahanMasakan”, ”OUT”, 4, ”INGREDIENT”, ”””{”card_id”:”telur”,”amount”:4}”””).
    [InlineData("BahanMasakan", "OUT", 4, "INGREDIENT", """{"card_id":"telur","amount":4}""")]
    // menyediakan satu kombinasi masukan pengujian (”JualMasakan”, ”IN”, 15, ”ORDER”,
    // ”””{”order_card_id”:”nasi_goreng”,”required_ingredient_card_ids”:[”nasi_putih”,”telur”],”income”:15}”””).
    [InlineData("JualMasakan", "IN", 15, "ORDER", """{"order_card_id":"nasi_goreng","required_ingredient_card_ids":["nasi_putih","telur"],"income":15}""")]
    // menyediakan satu kombinasi masukan pengujian (”JumatBerkah”, ”OUT”, 5, ”DONATION”, ”””{”amount”:5}”””).
    [InlineData("JumatBerkah", "OUT", 5, "DONATION", """{"amount":5}""")]
    // Mendefinisikan metode `TryBuild_GameActionIdsCreateCashflowProjection` dengan hasil bertipe `void`; operasi ini menangani try build game aksi
    // identitas create arus kas projection. Masukan: Parameter `actionType` bertipe `string` membawa nilai aksi jenis; Parameter `expectedDirection`
    // bertipe `string` membawa nilai yang diharapkan direction; Parameter `expectedAmount` bertipe `int` membawa nilai yang diharapkan nominal;
    // Parameter `expectedCategory` bertipe `string` membawa nilai yang diharapkan category; Parameter `payloadJson` bertipe `string` membawa nilai
    // payload JSON.
    public void TryBuild_GameActionIdsCreateCashflowProjection(
        // Parameter `actionType` bertipe `string` membawa nilai aksi jenis.
        string actionType,
        // Parameter `expectedDirection` bertipe `string` membawa nilai yang diharapkan direction.
        string expectedDirection,
        // Parameter `expectedAmount` bertipe `int` membawa nilai yang diharapkan nominal.
        int expectedAmount,
        // Parameter `expectedCategory` bertipe `string` membawa nilai yang diharapkan category.
        string expectedCategory,
        // Parameter `payloadJson` bertipe `string` membawa nilai payload JSON.
        string payloadJson)
    // Membuka scope metode TryBuild_GameActionIdsCreateCashflowProjection; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // TryBuild_GameActionIdsCreateCashflowProjection.
    {
        // Menyiapkan variabel lokal `request` untuk data masukan permintaan yang akan divalidasi atau diteruskan ke layanan dengan memanggil `BuildRequest`
        // dengan `actionType`, `payloadJson`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var request = BuildRequest(actionType: actionType, payloadJson: payloadJson);

        // Menyiapkan variabel lokal `ok` untuk nilai ok dengan memanggil `new EventCashflowProjectionBuilder().TryBuild` dengan `request`,
        // `DateTimeOffset.Parse(”2026-01-02T03:04:05Z”)`, `Guid.NewGuid()`, `var projection`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var ok = new EventCashflowProjectionBuilder().TryBuild(
            // Meneruskan `request` (data masukan permintaan yang akan divalidasi atau diteruskan ke layanan) sebagai argumen ke `new
            // EventCashflowProjectionBuilder().TryBuild`.
            request,
            // Meneruskan memanggil `DateTimeOffset.Parse` dengan `”2026-01-02T03:04:05Z”` sebagai argumen ke `new EventCashflowProjectionBuilder().TryBuild`;
            // Meneruskan nilai literal `”2026-01-02T03:04:05Z”` sebagai argumen ke `DateTimeOffset.Parse`.
            DateTimeOffset.Parse("2026-01-02T03:04:05Z"),
            // Meneruskan memanggil `Guid.NewGuid` dengan tanpa argumen sebagai argumen ke `new EventCashflowProjectionBuilder().TryBuild`.
            Guid.NewGuid(),
            // Meneruskan `var projection` sebagai argumen ke `new EventCashflowProjectionBuilder().TryBuild`.
            out var projection);

        // Menjalankan pemeriksaan bahwa `ok` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // TryBuild_GameActionIdsCreateCashflowProjection.
        Assert.True(ok);
        // Menjalankan pemeriksaan NotNull atas `projection` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // TryBuild_GameActionIdsCreateCashflowProjection.
        Assert.NotNull(projection);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`expectedDirection`, `projection.Direction`);
        // pengujian gagal jika keduanya berbeda dalam TryBuild_GameActionIdsCreateCashflowProjection.
        Assert.Equal(expectedDirection, projection.Direction);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`expectedAmount`, `projection.Amount`);
        // pengujian gagal jika keduanya berbeda dalam TryBuild_GameActionIdsCreateCashflowProjection.
        Assert.Equal(expectedAmount, projection.Amount);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`expectedCategory`, `projection.Category`);
        // pengujian gagal jika keduanya berbeda dalam TryBuild_GameActionIdsCreateCashflowProjection.
        Assert.Equal(expectedCategory, projection.Category);
    // Menutup scope metode TryBuild_GameActionIdsCreateCashflowProjection; bagian berikut berada di luar batas blok tersebut dalam
    // TryBuild_GameActionIdsCreateCashflowProjection.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `TryBuild_TransactionRoundsAmountAndUppercasesDirection` dengan hasil bertipe `void`; operasi ini menangani try build
    // transaction rounds nominal dan uppercases direction.
    public void TryBuild_TransactionRoundsAmountAndUppercasesDirection()
    // Membuka scope metode TryBuild_TransactionRoundsAmountAndUppercasesDirection; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // TryBuild_TransactionRoundsAmountAndUppercasesDirection.
    {
        // Menyiapkan variabel lokal `request` untuk data masukan permintaan yang akan divalidasi atau diteruskan ke layanan dengan memanggil `BuildRequest`
        // dengan `”CatatTransaksi”`, `””” { ”direction”: ”out”, ”amount”: 7.6, ”category”: ”CUSTOM”, ”counterparty”: ”BANK” } ”””`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var request = BuildRequest(
            // Meneruskan nilai literal `”CatatTransaksi”` sebagai argumen bernama `actionType`.
            actionType: "CatatTransaksi",
            // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen bernama `payloadJson`.
            // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
            // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `payloadJson: ”””`.
            // Baris literal 2: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `{`.
            // Baris literal 3: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”direction”: ”out”,`.
            // Baris literal 4: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”amount”: 7.6,`.
            // Baris literal 5: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”category”: ”CUSTOM”,`.
            // Baris literal 6: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”counterparty”: ”BANK”`.
            // Baris literal 7: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `}`.
            // Baris literal 8: Pembatas literal/penutup `”””);`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            payloadJson: """
                {
                  "direction": "out",
                  "amount": 7.6,
                  "category": "CUSTOM",
                  "counterparty": "BANK"
                }
                """);

        // Menyiapkan variabel lokal `ok` untuk nilai ok dengan memanggil `new EventCashflowProjectionBuilder().TryBuild` dengan `request`,
        // `DateTimeOffset.Parse(”2026-01-02T03:04:05Z”)`, `Guid.NewGuid()`, `var projection`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var ok = new EventCashflowProjectionBuilder().TryBuild(
            // Meneruskan `request` (data masukan permintaan yang akan divalidasi atau diteruskan ke layanan) sebagai argumen ke `new
            // EventCashflowProjectionBuilder().TryBuild`.
            request,
            // Meneruskan memanggil `DateTimeOffset.Parse` dengan `”2026-01-02T03:04:05Z”` sebagai argumen ke `new EventCashflowProjectionBuilder().TryBuild`;
            // Meneruskan nilai literal `”2026-01-02T03:04:05Z”` sebagai argumen ke `DateTimeOffset.Parse`.
            DateTimeOffset.Parse("2026-01-02T03:04:05Z"),
            // Meneruskan memanggil `Guid.NewGuid` dengan tanpa argumen sebagai argumen ke `new EventCashflowProjectionBuilder().TryBuild`.
            Guid.NewGuid(),
            // Meneruskan `var projection` sebagai argumen ke `new EventCashflowProjectionBuilder().TryBuild`.
            out var projection);

        // Menjalankan pemeriksaan bahwa `ok` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // TryBuild_TransactionRoundsAmountAndUppercasesDirection.
        Assert.True(ok);
        // Menjalankan pemeriksaan NotNull atas `projection` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // TryBuild_TransactionRoundsAmountAndUppercasesDirection.
        Assert.NotNull(projection);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”OUT”`, `projection.Direction`); pengujian
        // gagal jika keduanya berbeda dalam TryBuild_TransactionRoundsAmountAndUppercasesDirection.
        Assert.Equal("OUT", projection.Direction);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`8`, `projection.Amount`); pengujian gagal jika
        // keduanya berbeda dalam TryBuild_TransactionRoundsAmountAndUppercasesDirection.
        Assert.Equal(8, projection.Amount);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”CUSTOM”`, `projection.Category`); pengujian
        // gagal jika keduanya berbeda dalam TryBuild_TransactionRoundsAmountAndUppercasesDirection.
        Assert.Equal("CUSTOM", projection.Category);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”BANK”`, `projection.Counterparty`); pengujian
        // gagal jika keduanya berbeda dalam TryBuild_TransactionRoundsAmountAndUppercasesDirection.
        Assert.Equal("BANK", projection.Counterparty);
    // Menutup scope metode TryBuild_TransactionRoundsAmountAndUppercasesDirection; bagian berikut berada di luar batas blok tersebut dalam
    // TryBuild_TransactionRoundsAmountAndUppercasesDirection.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `TryBuild_EmergencyLoanUsesCatalogPrincipalInsteadOfClientCashflowFields` dengan hasil bertipe `void`; operasi ini
    // menangani try build emergency pinjaman uses catalog principal instead of client arus kas fields.
    public void TryBuild_EmergencyLoanUsesCatalogPrincipalInsteadOfClientCashflowFields()
    // Membuka scope metode TryBuild_EmergencyLoanUsesCatalogPrincipalInsteadOfClientCashflowFields; pernyataan/deklarasi berikut berada di dalam batas
    // blok ini dalam TryBuild_EmergencyLoanUsesCatalogPrincipalInsteadOfClientCashflowFields.
    {
        // Menyiapkan variabel lokal `request` untuk data masukan permintaan yang akan divalidasi atau diteruskan ke layanan dengan memanggil `BuildRequest`
        // dengan `”GunakanOpsiDarurat”`, `””” { ”risk_event_id”:”00000000-0000-0000-0000-000000000001”, ”option_type”:”TAKE_SHARIA_LOAN”, ”principal”:10,
        // ”direction”:”OUT”, ”amount”:100 } ”””`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var request = BuildRequest(
            // Meneruskan nilai literal `”GunakanOpsiDarurat”` sebagai argumen bernama `actionType`.
            actionType: "GunakanOpsiDarurat",
            // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen bernama `payloadJson`.
            // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
            // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `payloadJson: ”””`.
            // Baris literal 2: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `{`.
            // Baris literal 3: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis:
            // `”risk_event_id”:”00000000-0000-0000-0000-000000000001”,`.
            // Baris literal 4: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”option_type”:”TAKE_SHARIA_LOAN”,`.
            // Baris literal 5: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”principal”:10,`.
            // Baris literal 6: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”direction”:”OUT”,`.
            // Baris literal 7: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”amount”:100`.
            // Baris literal 8: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `}`.
            // Baris literal 9: Pembatas literal/penutup `”””);`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            payloadJson: """
                {
                  "risk_event_id":"00000000-0000-0000-0000-000000000001",
                  "option_type":"TAKE_SHARIA_LOAN",
                  "principal":10,
                  "direction":"OUT",
                  "amount":100
                }
                """);

        // Menyiapkan variabel lokal `ok` untuk nilai ok dengan memanggil `new EventCashflowProjectionBuilder().TryBuild` dengan `request`,
        // `DateTimeOffset.Parse(”2026-01-02T03:04:05Z”)`, `Guid.NewGuid()`, `var projection`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var ok = new EventCashflowProjectionBuilder().TryBuild(
            // Meneruskan `request` (data masukan permintaan yang akan divalidasi atau diteruskan ke layanan) sebagai argumen ke `new
            // EventCashflowProjectionBuilder().TryBuild`.
            request,
            // Meneruskan memanggil `DateTimeOffset.Parse` dengan `”2026-01-02T03:04:05Z”` sebagai argumen ke `new EventCashflowProjectionBuilder().TryBuild`;
            // Meneruskan nilai literal `”2026-01-02T03:04:05Z”` sebagai argumen ke `DateTimeOffset.Parse`.
            DateTimeOffset.Parse("2026-01-02T03:04:05Z"),
            // Meneruskan memanggil `Guid.NewGuid` dengan tanpa argumen sebagai argumen ke `new EventCashflowProjectionBuilder().TryBuild`.
            Guid.NewGuid(),
            // Meneruskan `var projection` sebagai argumen ke `new EventCashflowProjectionBuilder().TryBuild`.
            out var projection);

        // Menjalankan pemeriksaan bahwa `ok` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // TryBuild_EmergencyLoanUsesCatalogPrincipalInsteadOfClientCashflowFields.
        Assert.True(ok);
        // Menjalankan pemeriksaan NotNull atas `projection` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // TryBuild_EmergencyLoanUsesCatalogPrincipalInsteadOfClientCashflowFields.
        Assert.NotNull(projection);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”IN”`, `projection.Direction`); pengujian
        // gagal jika keduanya berbeda dalam TryBuild_EmergencyLoanUsesCatalogPrincipalInsteadOfClientCashflowFields.
        Assert.Equal("IN", projection.Direction);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`10`, `projection.Amount`); pengujian gagal
        // jika keduanya berbeda dalam TryBuild_EmergencyLoanUsesCatalogPrincipalInsteadOfClientCashflowFields.
        Assert.Equal(10, projection.Amount);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”EMERGENCY_OPTION”`, `projection.Category`);
        // pengujian gagal jika keduanya berbeda dalam TryBuild_EmergencyLoanUsesCatalogPrincipalInsteadOfClientCashflowFields.
        Assert.Equal("EMERGENCY_OPTION", projection.Category);
    // Menutup scope metode TryBuild_EmergencyLoanUsesCatalogPrincipalInsteadOfClientCashflowFields; bagian berikut berada di luar batas blok tersebut
    // dalam TryBuild_EmergencyLoanUsesCatalogPrincipalInsteadOfClientCashflowFields.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `TryBuild_EmergencyOptionType_IsCaseInsensitive` dengan hasil bertipe `void`; operasi ini menangani try build emergency
    // option jenis berstatus case insensitive.
    public void TryBuild_EmergencyOptionType_IsCaseInsensitive()
    // Membuka scope metode TryBuild_EmergencyOptionType_IsCaseInsensitive; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // TryBuild_EmergencyOptionType_IsCaseInsensitive.
    {
        // Menyiapkan variabel lokal `request` untuk data masukan permintaan yang akan divalidasi atau diteruskan ke layanan dengan memanggil `BuildRequest`
        // dengan `”GunakanOpsiDarurat”`, `””” { ”risk_event_id”:”00000000-0000-0000-0000-000000000001”, ”option_type”:”sell_need”, ”amount”:2 } ”””`. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var request = BuildRequest(
            // Meneruskan nilai literal `”GunakanOpsiDarurat”` sebagai argumen bernama `actionType`.
            actionType: "GunakanOpsiDarurat",
            // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen bernama `payloadJson`.
            // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
            // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `payloadJson: ”””`.
            // Baris literal 2: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `{`.
            // Baris literal 3: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis:
            // `”risk_event_id”:”00000000-0000-0000-0000-000000000001”,`.
            // Baris literal 4: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”option_type”:”sell_need”,`.
            // Baris literal 5: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”amount”:2`.
            // Baris literal 6: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `}`.
            // Baris literal 7: Pembatas literal/penutup `”””);`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            payloadJson: """
                {
                  "risk_event_id":"00000000-0000-0000-0000-000000000001",
                  "option_type":"sell_need",
                  "amount":2
                }
                """);

        // Menyiapkan variabel lokal `ok` untuk nilai ok dengan memanggil `new EventCashflowProjectionBuilder().TryBuild` dengan `request`,
        // `DateTimeOffset.Parse(”2026-01-02T03:04:05Z”)`, `Guid.NewGuid()`, `var projection`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var ok = new EventCashflowProjectionBuilder().TryBuild(
            // Meneruskan `request` (data masukan permintaan yang akan divalidasi atau diteruskan ke layanan) sebagai argumen ke `new
            // EventCashflowProjectionBuilder().TryBuild`.
            request,
            // Meneruskan memanggil `DateTimeOffset.Parse` dengan `”2026-01-02T03:04:05Z”` sebagai argumen ke `new EventCashflowProjectionBuilder().TryBuild`;
            // Meneruskan nilai literal `”2026-01-02T03:04:05Z”` sebagai argumen ke `DateTimeOffset.Parse`.
            DateTimeOffset.Parse("2026-01-02T03:04:05Z"),
            // Meneruskan memanggil `Guid.NewGuid` dengan tanpa argumen sebagai argumen ke `new EventCashflowProjectionBuilder().TryBuild`.
            Guid.NewGuid(),
            // Meneruskan `var projection` sebagai argumen ke `new EventCashflowProjectionBuilder().TryBuild`.
            out var projection);

        // Menjalankan pemeriksaan bahwa `ok` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // TryBuild_EmergencyOptionType_IsCaseInsensitive.
        Assert.True(ok);
        // Menjalankan pemeriksaan NotNull atas `projection` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // TryBuild_EmergencyOptionType_IsCaseInsensitive.
        Assert.NotNull(projection);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”IN”`, `projection.Direction`); pengujian
        // gagal jika keduanya berbeda dalam TryBuild_EmergencyOptionType_IsCaseInsensitive.
        Assert.Equal("IN", projection.Direction);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`2`, `projection.Amount`); pengujian gagal jika
        // keduanya berbeda dalam TryBuild_EmergencyOptionType_IsCaseInsensitive.
        Assert.Equal(2, projection.Amount);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”EMERGENCY_OPTION”`, `projection.Category`);
        // pengujian gagal jika keduanya berbeda dalam TryBuild_EmergencyOptionType_IsCaseInsensitive.
        Assert.Equal("EMERGENCY_OPTION", projection.Category);
    // Menutup scope metode TryBuild_EmergencyOptionType_IsCaseInsensitive; bagian berikut berada di luar batas blok tersebut dalam
    // TryBuild_EmergencyOptionType_IsCaseInsensitive.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `TryBuild_RemovedEmergencyInsuranceOptionDoesNotCreateCashflow` dengan hasil bertipe `void`; operasi ini menangani try
    // build removed emergency asuransi option does not create arus kas.
    public void TryBuild_RemovedEmergencyInsuranceOptionDoesNotCreateCashflow()
    // Membuka scope metode TryBuild_RemovedEmergencyInsuranceOptionDoesNotCreateCashflow; pernyataan/deklarasi berikut berada di dalam batas blok ini
    // dalam TryBuild_RemovedEmergencyInsuranceOptionDoesNotCreateCashflow.
    {
        // Menyiapkan variabel lokal `request` untuk data masukan permintaan yang akan divalidasi atau diteruskan ke layanan dengan memanggil `BuildRequest`
        // dengan `”GunakanOpsiDarurat”`, `””” { ”risk_event_id”:”00000000-0000-0000-0000-000000000001”, ”option_type”:”USE_INSURANCE”, ”amount”:10 } ”””`.
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var request = BuildRequest(
            // Meneruskan nilai literal `”GunakanOpsiDarurat”` sebagai argumen bernama `actionType`.
            actionType: "GunakanOpsiDarurat",
            // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen bernama `payloadJson`.
            // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
            // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `payloadJson: ”””`.
            // Baris literal 2: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `{`.
            // Baris literal 3: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis:
            // `”risk_event_id”:”00000000-0000-0000-0000-000000000001”,`.
            // Baris literal 4: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”option_type”:”USE_INSURANCE”,`.
            // Baris literal 5: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”amount”:10`.
            // Baris literal 6: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `}`.
            // Baris literal 7: Pembatas literal/penutup `”””);`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            payloadJson: """
                {
                  "risk_event_id":"00000000-0000-0000-0000-000000000001",
                  "option_type":"USE_INSURANCE",
                  "amount":10
                }
                """);

        // Menyiapkan variabel lokal `ok` untuk nilai ok dengan memanggil `new EventCashflowProjectionBuilder().TryBuild` dengan `request`,
        // `DateTimeOffset.Parse(”2026-01-02T03:04:05Z”)`, `Guid.NewGuid()`, `var projection`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var ok = new EventCashflowProjectionBuilder().TryBuild(
            // Meneruskan `request` (data masukan permintaan yang akan divalidasi atau diteruskan ke layanan) sebagai argumen ke `new
            // EventCashflowProjectionBuilder().TryBuild`.
            request,
            // Meneruskan memanggil `DateTimeOffset.Parse` dengan `”2026-01-02T03:04:05Z”` sebagai argumen ke `new EventCashflowProjectionBuilder().TryBuild`;
            // Meneruskan nilai literal `”2026-01-02T03:04:05Z”` sebagai argumen ke `DateTimeOffset.Parse`.
            DateTimeOffset.Parse("2026-01-02T03:04:05Z"),
            // Meneruskan memanggil `Guid.NewGuid` dengan tanpa argumen sebagai argumen ke `new EventCashflowProjectionBuilder().TryBuild`.
            Guid.NewGuid(),
            // Meneruskan `var projection` sebagai argumen ke `new EventCashflowProjectionBuilder().TryBuild`.
            out var projection);

        // Menjalankan pemeriksaan bahwa `ok` bernilai salah; pengujian gagal jika kondisi justru terpenuhi dalam
        // TryBuild_RemovedEmergencyInsuranceOptionDoesNotCreateCashflow.
        Assert.False(ok);
        // Menjalankan pemeriksaan Null atas `projection` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // TryBuild_RemovedEmergencyInsuranceOptionDoesNotCreateCashflow.
        Assert.Null(projection);
    // Menutup scope metode TryBuild_RemovedEmergencyInsuranceOptionDoesNotCreateCashflow; bagian berikut berada di luar batas blok tersebut dalam
    // TryBuild_RemovedEmergencyInsuranceOptionDoesNotCreateCashflow.
    }

    // Mendefinisikan metode `BuildRequest` dengan hasil bertipe `EventRequest`; operasi ini menangani build permintaan. Masukan: Parameter `actionType`
    // bertipe `string` membawa nilai aksi jenis; Parameter `payloadJson` bertipe `string` membawa nilai payload JSON; Parameter `sessionId` bertipe
    // `Guid?` membawa identitas unik sesi permainan yang menjadi batas data operasi ini; nilai null diizinkan ketika data opsional belum tersedia; bila
    // argumen tidak diberikan digunakan null, yaitu penanda tidak ada nilai; Parameter `eventId` bertipe `Guid?` membawa identitas unik event untuk
    // pencatatan dan pemeriksaan duplikasi; nilai null diizinkan ketika data opsional belum tersedia; bila argumen tidak diberikan digunakan null,
    // yaitu penanda tidak ada nilai; Parameter `playerId` bertipe `Guid?` membawa nilai pemain identitas; nilai null diizinkan ketika data opsional
    // belum tersedia; bila argumen tidak diberikan digunakan null, yaitu penanda tidak ada nilai; Parameter `includePlayer` bertipe `bool` membawa
    // nilai include pemain; bila argumen tidak diberikan digunakan true, yaitu kondisi aktif/terpenuhi.
    private static EventRequest BuildRequest(
        // Parameter `actionType` bertipe `string` membawa nilai aksi jenis.
        string actionType,
        // Parameter `payloadJson` bertipe `string` membawa nilai payload JSON.
        string payloadJson,
        // Parameter `sessionId` bertipe `Guid?` membawa identitas unik sesi permainan yang menjadi batas data operasi ini; nilai null diizinkan ketika data
        // opsional belum tersedia; bila argumen tidak diberikan digunakan null, yaitu penanda tidak ada nilai.
        Guid? sessionId = null,
        // Parameter `eventId` bertipe `Guid?` membawa identitas unik event untuk pencatatan dan pemeriksaan duplikasi; nilai null diizinkan ketika data
        // opsional belum tersedia; bila argumen tidak diberikan digunakan null, yaitu penanda tidak ada nilai.
        Guid? eventId = null,
        // Parameter `playerId` bertipe `Guid?` membawa nilai pemain identitas; nilai null diizinkan ketika data opsional belum tersedia; bila argumen tidak
        // diberikan digunakan null, yaitu penanda tidak ada nilai.
        Guid? playerId = null,
        // Parameter `includePlayer` bertipe `bool` membawa nilai include pemain; bila argumen tidak diberikan digunakan true, yaitu kondisi
        // aktif/terpenuhi.
        bool includePlayer = true)
    // Membuka scope metode BuildRequest; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildRequest.
    {
        // Mengembalikan objek baru bertipe `EventRequest` dengan argumen ( eventId ?? Guid.NewGuid(), sessionId ?? Guid.NewGuid(), includePlayer ? playerId
        // ?? Guid.NewGuid() : null, ”PLAYER”, DateTimeOffset.Parse(”2026-01-02T03:04:05... kepada pemanggil dalam BuildRequest; eksekusi jalur ini selesai
        // setelah nilai hasil ditentukan.
        return new EventRequest(
            // Meneruskan `eventId` bila tidak null; jika null gunakan `Guid.NewGuid()` sebagai nilai pengganti sebagai argumen ke konstruktor `EventRequest`.
            eventId ?? Guid.NewGuid(),
            // Meneruskan `sessionId` bila tidak null; jika null gunakan `Guid.NewGuid()` sebagai nilai pengganti sebagai argumen ke konstruktor `EventRequest`.
            sessionId ?? Guid.NewGuid(),
            // Meneruskan hasil pemilihan bersyarat: ketika `includePlayer` benar gunakan `playerId ?? Guid.NewGuid()`, jika tidak gunakan `null` sebagai
            // argumen ke konstruktor `EventRequest`.
            includePlayer ? playerId ?? Guid.NewGuid() : null,
            // Meneruskan nilai literal `”PLAYER”` sebagai argumen ke konstruktor `EventRequest`.
            "PLAYER",
            // Meneruskan memanggil `DateTimeOffset.Parse` dengan `”2026-01-02T03:04:05Z”` sebagai argumen ke konstruktor `EventRequest`; Meneruskan nilai
            // literal `”2026-01-02T03:04:05Z”` sebagai argumen ke `DateTimeOffset.Parse`.
            DateTimeOffset.Parse("2026-01-02T03:04:05Z"),
            // Meneruskan nilai literal `0` sebagai argumen ke konstruktor `EventRequest`.
            0,
            // Meneruskan nilai literal `”MON”` sebagai argumen ke konstruktor `EventRequest`.
            "MON",
            // Meneruskan nilai literal `1` sebagai argumen ke konstruktor `EventRequest`.
            1,
            // Meneruskan nilai literal `1` sebagai argumen ke konstruktor `EventRequest`.
            1,
            // Meneruskan `actionType` (nilai aksi jenis) sebagai argumen ke konstruktor `EventRequest`.
            actionType,
            // Meneruskan memanggil `Guid.NewGuid` dengan tanpa argumen sebagai argumen ke konstruktor `EventRequest`.
            Guid.NewGuid(),
            // Meneruskan memanggil `Parse` dengan `payloadJson` sebagai argumen ke konstruktor `EventRequest`; Meneruskan `payloadJson` (nilai payload JSON)
            // sebagai argumen ke `Parse`.
            Parse(payloadJson),
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen ke konstruktor `EventRequest`.
            null);
    // Menutup scope metode BuildRequest; bagian berikut berada di luar batas blok tersebut dalam BuildRequest.
    }

    // Mendefinisikan metode `Parse` dengan hasil bertipe `JsonElement`; operasi ini menangani parse. Masukan: Parameter `json` bertipe `string` membawa
    // nilai JSON.
    private static JsonElement Parse(string json)
    // Membuka scope metode Parse; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Parse.
    {
        // Menyiapkan variabel lokal `document` untuk nilai document dengan memanggil `JsonDocument.Parse` dengan `json`. Tipe variabel disimpulkan dari
        // ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var document = JsonDocument.Parse(json);
        // Mengembalikan membuat salinan `document.RootElement` agar hasil dapat digunakan terpisah dari objek sumber kepada pemanggil dalam Parse; eksekusi
        // jalur ini selesai setelah nilai hasil ditentukan.
        return document.RootElement.Clone();
    // Menutup scope metode Parse; bagian berikut berada di luar batas blok tersebut dalam Parse.
    }
// Menutup scope tipe EventCashflowProjectionBuilderTests; bagian berikut berada di luar batas blok tersebut.
}
