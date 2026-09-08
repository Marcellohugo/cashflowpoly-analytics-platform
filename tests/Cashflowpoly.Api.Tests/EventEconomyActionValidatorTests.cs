// Fungsi file: Memverifikasi perilaku API, database, atau domain melalui EventEconomyActionValidatorTests.
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

// Mendefinisikan tipe class `EventEconomyActionValidatorTests`; sealed mencegah tipe ini diturunkan lagi.
public sealed class EventEconomyActionValidatorTests
// Membuka scope tipe EventEconomyActionValidatorTests; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `TryValidate_TransactionOut_ReturnsOutgoingAmountForBalanceCheck` dengan hasil bertipe `void`; operasi ini menangani try
    // validate transaction out returns outgoing nominal untuk saldo check.
    public void TryValidate_TransactionOut_ReturnsOutgoingAmountForBalanceCheck()
    // Membuka scope metode TryValidate_TransactionOut_ReturnsOutgoingAmountForBalanceCheck; pernyataan/deklarasi berikut berada di dalam batas blok ini
    // dalam TryValidate_TransactionOut_ReturnsOutgoingAmountForBalanceCheck.
    {
        // Menyiapkan variabel lokal `request` untuk data masukan permintaan yang akan divalidasi atau diteruskan ke layanan dengan memanggil
        // `CreateRequest` dengan `”CatatTransaksi”`, `”””{”direction”:”OUT”,”amount”:6,”category”:”CUSTOM”,”counterparty”:”BANK”}”””`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var request = CreateRequest("CatatTransaksi", """{"direction":"OUT","amount":6,"category":"CUSTOM","counterparty":"BANK"}""");

        // Menyiapkan variabel lokal `handled` untuk nilai handled dengan memanggil `new EventEconomyActionValidator().TryValidate` dengan `request`,
        // `CreateConfig()`, `Array.Empty<EventDb>()`, `var result`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var handled = new EventEconomyActionValidator().TryValidate(request, CreateConfig(), Array.Empty<EventDb>(), out var result);

        // Menjalankan pemeriksaan bahwa `handled` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // TryValidate_TransactionOut_ReturnsOutgoingAmountForBalanceCheck.
        Assert.True(handled);
        // Menjalankan pemeriksaan bahwa `result.Validation.IsValid` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // TryValidate_TransactionOut_ReturnsOutgoingAmountForBalanceCheck.
        Assert.True(result.Validation.IsValid);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`6`, `result.OutgoingAmount`); pengujian gagal
        // jika keduanya berbeda dalam TryValidate_TransactionOut_ReturnsOutgoingAmountForBalanceCheck.
        Assert.Equal(6, result.OutgoingAmount);
    // Menutup scope metode TryValidate_TransactionOut_ReturnsOutgoingAmountForBalanceCheck; bagian berikut berada di luar batas blok tersebut dalam
    // TryValidate_TransactionOut_ReturnsOutgoingAmountForBalanceCheck.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `TryValidate_DonationRejectsWrongWeekday` dengan hasil bertipe `void`; operasi ini menangani try validate donasi rejects
    // wrong weekday.
    public void TryValidate_DonationRejectsWrongWeekday()
    // Membuka scope metode TryValidate_DonationRejectsWrongWeekday; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // TryValidate_DonationRejectsWrongWeekday.
    {
        // Menyiapkan variabel lokal `request` untuk data masukan permintaan yang akan divalidasi atau diteruskan ke layanan dengan memanggil
        // `CreateRequest` dengan `”JumatBerkah”`, `”””{”amount”:3}”””`, `”MON”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var request = CreateRequest("JumatBerkah", """{"amount":3}""", weekday: "MON");

        // Menyiapkan variabel lokal `handled` untuk nilai handled dengan memanggil `new EventEconomyActionValidator().TryValidate` dengan `request`,
        // `CreateConfig()`, `Array.Empty<EventDb>()`, `var result`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var handled = new EventEconomyActionValidator().TryValidate(request, CreateConfig(), Array.Empty<EventDb>(), out var result);

        // Menjalankan pemeriksaan bahwa `handled` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // TryValidate_DonationRejectsWrongWeekday.
        Assert.True(handled);
        // Menjalankan pemeriksaan bahwa `result.Validation.IsValid` bernilai salah; pengujian gagal jika kondisi justru terpenuhi dalam
        // TryValidate_DonationRejectsWrongWeekday.
        Assert.False(result.Validation.IsValid);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`StatusCodes.Status400BadRequest`,
        // `result.Validation.StatusCode`); pengujian gagal jika keduanya berbeda dalam TryValidate_DonationRejectsWrongWeekday.
        Assert.Equal(StatusCodes.Status400BadRequest, result.Validation.StatusCode);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `result.Validation.Details`, `detail =>
        // detail.Field == ”weekday” && detail.Issue == ”INVALID_VALUE”` dalam TryValidate_DonationRejectsWrongWeekday.
        Assert.Contains(result.Validation.Details, detail => detail.Field == "weekday" && detail.Issue == "INVALID_VALUE");
    // Menutup scope metode TryValidate_DonationRejectsWrongWeekday; bagian berikut berada di luar batas blok tersebut dalam
    // TryValidate_DonationRejectsWrongWeekday.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `TryValidate_DonationRejectsSecondSubmissionOnSameFriday` dengan hasil bertipe `void`; operasi ini menangani try validate
    // donasi rejects second submission on same friday.
    public void TryValidate_DonationRejectsSecondSubmissionOnSameFriday()
    // Membuka scope metode TryValidate_DonationRejectsSecondSubmissionOnSameFriday; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // TryValidate_DonationRejectsSecondSubmissionOnSameFriday.
    {
        // Menyiapkan variabel lokal `playerId` untuk nilai pemain identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var playerId = Guid.NewGuid();
        // Menyiapkan variabel lokal `request` untuk data masukan permintaan yang akan divalidasi atau diteruskan ke layanan dengan memanggil
        // `CreateRequest` dengan `”JumatBerkah”`, `”””{”amount”:3}”””`, `playerId`, `”FRI”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var request = CreateRequest("JumatBerkah", """{"amount":3}""", playerId, weekday: "FRI");
        // Menyiapkan variabel lokal `history` untuk nilai history dengan array baru dengan tipe elemen disimpulkan dari nilai initializer. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var history = new[] { CreateEvent(playerId, "JumatBerkah", """{"amount":2}""", sessionId: request.SessionId) };

        // Menjalankan memanggil `new EventEconomyActionValidator().TryValidate` dengan `request`, `CreateConfig()`, `history`, `var result` dalam
        // TryValidate_DonationRejectsSecondSubmissionOnSameFriday.
        new EventEconomyActionValidator().TryValidate(request, CreateConfig(), history, out var result);

        // Menjalankan pemeriksaan bahwa `result.Validation.IsValid` bernilai salah; pengujian gagal jika kondisi justru terpenuhi dalam
        // TryValidate_DonationRejectsSecondSubmissionOnSameFriday.
        Assert.False(result.Validation.IsValid);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”DONATION_ALREADY_SUBMITTED”`,
        // `result.Validation.ErrorCode`); pengujian gagal jika keduanya berbeda dalam TryValidate_DonationRejectsSecondSubmissionOnSameFriday.
        Assert.Equal("DONATION_ALREADY_SUBMITTED", result.Validation.ErrorCode);
    // Menutup scope metode TryValidate_DonationRejectsSecondSubmissionOnSameFriday; bagian berikut berada di luar batas blok tersebut dalam
    // TryValidate_DonationRejectsSecondSubmissionOnSameFriday.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `TryValidate_GoldSellLeavesInventoryCheckToRelationalHolding` dengan hasil bertipe `void`; operasi ini menangani try
    // validate emas sell leaves inventory check ke relational holding.
    public void TryValidate_GoldSellLeavesInventoryCheckToRelationalHolding()
    // Membuka scope metode TryValidate_GoldSellLeavesInventoryCheckToRelationalHolding; pernyataan/deklarasi berikut berada di dalam batas blok ini
    // dalam TryValidate_GoldSellLeavesInventoryCheckToRelationalHolding.
    {
        // Menyiapkan variabel lokal `playerId` untuk nilai pemain identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var playerId = Guid.NewGuid();
        // Menyiapkan variabel lokal `request` untuk data masukan permintaan yang akan divalidasi atau diteruskan ke layanan dengan memanggil
        // `CreateRequest` dengan `”JualEmas”`, `”””{”trade_type”:”SELL”,”qty”:2,”unit_price”:5,”amount”:10}”””`, `playerId`, `”SAT”`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var request = CreateRequest(
            // Meneruskan nilai literal `”JualEmas”` sebagai argumen ke `CreateRequest`.
            "JualEmas",
            // Meneruskan nilai literal `”””{”trade_type”:”SELL”,”qty”:2,”unit_price”:5,”amount”:10}”””` sebagai argumen ke `CreateRequest`.
            """{"trade_type":"SELL","qty":2,"unit_price":5,"amount":10}""",
            // Meneruskan `playerId` (nilai pemain identitas) sebagai argumen ke `CreateRequest`.
            playerId,
            // Meneruskan nilai literal `”SAT”` sebagai argumen bernama `weekday`.
            weekday: "SAT");
        // Menyiapkan variabel lokal `history` untuk nilai history dengan array baru dengan tipe elemen disimpulkan dari nilai initializer. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var history = new[]
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // TryValidate_GoldSellLeavesInventoryCheckToRelationalHolding.
        {
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `playerId`, `”BukaHargaEmas”`, `”””{”gold_price”:5}”””`, `request.SessionId` dalam
            // TryValidate_GoldSellLeavesInventoryCheckToRelationalHolding.
            CreateEvent(playerId, "BukaHargaEmas", """{"gold_price":5}""", sessionId: request.SessionId)
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // TryValidate_GoldSellLeavesInventoryCheckToRelationalHolding.
        };

        // Menyiapkan variabel lokal `handled` untuk nilai handled dengan memanggil `new EventEconomyActionValidator().TryValidate` dengan `request`,
        // `CreateConfig()`, `history`, `var result`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var handled = new EventEconomyActionValidator().TryValidate(request, CreateConfig(), history, out var result);

        // Menjalankan pemeriksaan bahwa `handled` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // TryValidate_GoldSellLeavesInventoryCheckToRelationalHolding.
        Assert.True(handled);
        // Menjalankan pemeriksaan bahwa `result.Validation.IsValid` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // TryValidate_GoldSellLeavesInventoryCheckToRelationalHolding.
        Assert.True(result.Validation.IsValid);
    // Menutup scope metode TryValidate_GoldSellLeavesInventoryCheckToRelationalHolding; bagian berikut berada di luar batas blok tersebut dalam
    // TryValidate_GoldSellLeavesInventoryCheckToRelationalHolding.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `TryValidate_GoldTradeAllowsNonSaturdayWhenTriggeredByLifeRisk` dengan hasil bertipe `void`; operasi ini menangani try
    // validate emas trade allows non saturday when triggered berdasarkan life risiko.
    public void TryValidate_GoldTradeAllowsNonSaturdayWhenTriggeredByLifeRisk()
    // Membuka scope metode TryValidate_GoldTradeAllowsNonSaturdayWhenTriggeredByLifeRisk; pernyataan/deklarasi berikut berada di dalam batas blok ini
    // dalam TryValidate_GoldTradeAllowsNonSaturdayWhenTriggeredByLifeRisk.
    {
        // Menyiapkan variabel lokal `riskEventId` untuk nilai risiko event identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var riskEventId = Guid.NewGuid();
        // Menyiapkan variabel lokal `request` untuk data masukan permintaan yang akan divalidasi atau diteruskan ke layanan dengan memanggil
        // `CreateRequest` dengan `”InvestasiEmas”`, `$$”””{”trade_type”:”BUY”,”qty”:1,”unit_price”:5,”amount”:5,”risk_event_id”:”{{riskEventId}}”}”””`,
        // `”MON”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var request = CreateRequest(
            // Meneruskan nilai literal `”InvestasiEmas”` sebagai argumen ke `CreateRequest`.
            "InvestasiEmas",
            // Meneruskan teks interpolasi `$$”””{”trade_type”:”BUY”,”qty”:1,”unit_price”:5,”amount”:5,”risk_event_id”:”{{riskEventId}}”}”””`; nilai ekspresi di
            // dalam kurung kurawal disisipkan saat program berjalan sebagai argumen ke `CreateRequest`.
            $$"""{"trade_type":"BUY","qty":1,"unit_price":5,"amount":5,"risk_event_id":"{{riskEventId}}"}""",
            // Meneruskan nilai literal `”MON”` sebagai argumen bernama `weekday`.
            weekday: "MON");
        // Menyiapkan variabel lokal `history` untuk nilai history dengan array baru dengan tipe elemen disimpulkan dari nilai initializer. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var history = new[]
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // TryValidate_GoldTradeAllowsNonSaturdayWhenTriggeredByLifeRisk.
        {
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `request.UserId!.Value`, `”BukaHargaEmas”`, `”””{”gold_price”:5}”””`,
            // `request.SessionId` dalam TryValidate_GoldTradeAllowsNonSaturdayWhenTriggeredByLifeRisk.
            CreateEvent(request.UserId!.Value, "BukaHargaEmas", """{"gold_price":5}""", sessionId: request.SessionId),
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `request.UserId!.Value`, `”RisikoKehidupan”`, `”””{”risk_id”:”risk_gold”}”””`,
            // `riskEventId`, `request.SessionId` dalam TryValidate_GoldTradeAllowsNonSaturdayWhenTriggeredByLifeRisk.
            CreateEvent(
                // Meneruskan `request.UserId!.Value`, yaitu nilai yang dibungkus objek/nullable sebagai argumen ke `CreateEvent`.
                request.UserId!.Value,
                // Meneruskan nilai literal `”RisikoKehidupan”` sebagai argumen ke `CreateEvent`.
                "RisikoKehidupan",
                // Meneruskan nilai literal `”””{”risk_id”:”risk_gold”}”””` sebagai argumen ke `CreateEvent`.
                """{"risk_id":"risk_gold"}""",
                // Meneruskan `riskEventId` (nilai risiko event identitas) sebagai argumen ke `CreateEvent`.
                riskEventId,
                // Meneruskan `request.SessionId` (identitas unik sesi permainan yang menjadi batas data operasi ini) sebagai argumen ke `CreateEvent`.
                request.SessionId)
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // TryValidate_GoldTradeAllowsNonSaturdayWhenTriggeredByLifeRisk.
        };

        // Menyiapkan variabel lokal `handled` untuk nilai handled dengan memanggil `new EventEconomyActionValidator().TryValidate` dengan `request`,
        // `CreateConfig()`, `history`, `var result`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var handled = new EventEconomyActionValidator().TryValidate(request, CreateConfig(), history, out var result);

        // Menjalankan pemeriksaan bahwa `handled` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // TryValidate_GoldTradeAllowsNonSaturdayWhenTriggeredByLifeRisk.
        Assert.True(handled);
        // Menjalankan pemeriksaan bahwa `result.Validation.IsValid` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // TryValidate_GoldTradeAllowsNonSaturdayWhenTriggeredByLifeRisk.
        Assert.True(result.Validation.IsValid);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`5`, `result.OutgoingAmount`); pengujian gagal
        // jika keduanya berbeda dalam TryValidate_GoldTradeAllowsNonSaturdayWhenTriggeredByLifeRisk.
        Assert.Equal(5, result.OutgoingAmount);
    // Menutup scope metode TryValidate_GoldTradeAllowsNonSaturdayWhenTriggeredByLifeRisk; bagian berikut berada di luar batas blok tersebut dalam
    // TryValidate_GoldTradeAllowsNonSaturdayWhenTriggeredByLifeRisk.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `TryValidate_GoldTradeRejectsPriceFromPreviousSaturday` dengan hasil bertipe `void`; operasi ini menangani try validate
    // emas trade rejects harga dari previous saturday.
    public void TryValidate_GoldTradeRejectsPriceFromPreviousSaturday()
    // Membuka scope metode TryValidate_GoldTradeRejectsPriceFromPreviousSaturday; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // TryValidate_GoldTradeRejectsPriceFromPreviousSaturday.
    {
        // Menyiapkan variabel lokal `playerId` untuk nilai pemain identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var playerId = Guid.NewGuid();
        // Menyiapkan variabel lokal `request` untuk data masukan permintaan yang akan divalidasi atau diteruskan ke layanan dengan memanggil
        // `CreateRequest` dengan `”InvestasiEmas”`, `”””{”trade_type”:”BUY”,”qty”:1,”unit_price”:5,”amount”:5}”””`, `playerId`, `”SAT”`, `13`. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var request = CreateRequest(
            // Meneruskan nilai literal `”InvestasiEmas”` sebagai argumen ke `CreateRequest`.
            "InvestasiEmas",
            // Meneruskan nilai literal `”””{”trade_type”:”BUY”,”qty”:1,”unit_price”:5,”amount”:5}”””` sebagai argumen ke `CreateRequest`.
            """{"trade_type":"BUY","qty":1,"unit_price":5,"amount":5}""",
            // Meneruskan `playerId` (nilai pemain identitas) sebagai argumen ke `CreateRequest`.
            playerId,
            // Meneruskan nilai literal `”SAT”` sebagai argumen bernama `weekday`.
            weekday: "SAT",
            // Meneruskan nilai literal `13` sebagai argumen bernama `dayIndex`.
            dayIndex: 13);
        // Menyiapkan variabel lokal `history` untuk nilai history dengan array baru dengan tipe elemen disimpulkan dari nilai initializer. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var history = new[]
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // TryValidate_GoldTradeRejectsPriceFromPreviousSaturday.
        {
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `playerId`, `”BukaHargaEmas”`, `”””{”gold_price”:5}”””`, `request.SessionId`, `6`
            // dalam TryValidate_GoldTradeRejectsPriceFromPreviousSaturday.
            CreateEvent(playerId, "BukaHargaEmas", """{"gold_price":5}""", sessionId: request.SessionId, dayIndex: 6)
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // TryValidate_GoldTradeRejectsPriceFromPreviousSaturday.
        };

        // Menjalankan memanggil `new EventEconomyActionValidator().TryValidate` dengan `request`, `CreateConfig()`, `history`, `var result` dalam
        // TryValidate_GoldTradeRejectsPriceFromPreviousSaturday.
        new EventEconomyActionValidator().TryValidate(request, CreateConfig(), history, out var result);

        // Menjalankan pemeriksaan bahwa `result.Validation.IsValid` bernilai salah; pengujian gagal jika kondisi justru terpenuhi dalam
        // TryValidate_GoldTradeRejectsPriceFromPreviousSaturday.
        Assert.False(result.Validation.IsValid);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`StatusCodes.Status422UnprocessableEntity`,
        // `result.Validation.StatusCode`); pengujian gagal jika keduanya berbeda dalam TryValidate_GoldTradeRejectsPriceFromPreviousSaturday.
        Assert.Equal(StatusCodes.Status422UnprocessableEntity, result.Validation.StatusCode);
    // Menutup scope metode TryValidate_GoldTradeRejectsPriceFromPreviousSaturday; bagian berikut berada di luar batas blok tersebut dalam
    // TryValidate_GoldTradeRejectsPriceFromPreviousSaturday.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `TryValidate_GoldTradeRejectsFakeRiskReferenceOutsideSaturday` dengan hasil bertipe `void`; operasi ini menangani try
    // validate emas trade rejects fake risiko reference outside saturday.
    public void TryValidate_GoldTradeRejectsFakeRiskReferenceOutsideSaturday()
    // Membuka scope metode TryValidate_GoldTradeRejectsFakeRiskReferenceOutsideSaturday; pernyataan/deklarasi berikut berada di dalam batas blok ini
    // dalam TryValidate_GoldTradeRejectsFakeRiskReferenceOutsideSaturday.
    {
        // Menyiapkan variabel lokal `request` untuk data masukan permintaan yang akan divalidasi atau diteruskan ke layanan dengan memanggil
        // `CreateRequest` dengan `”InvestasiEmas”`, `$$”””{”trade_type”:”BUY”,”qty”:1,”unit_price”:5,”amount”:5,”risk_event_id”:”{{Guid.NewGuid()}}”}”””`,
        // `”MON”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var request = CreateRequest(
            // Meneruskan nilai literal `”InvestasiEmas”` sebagai argumen ke `CreateRequest`.
            "InvestasiEmas",
            // Meneruskan teks interpolasi `$$”””{”trade_type”:”BUY”,”qty”:1,”unit_price”:5,”amount”:5,”risk_event_id”:”{{Guid.NewGuid()}}”}”””`; nilai ekspresi
            // di dalam kurung kurawal disisipkan saat program berjalan sebagai argumen ke `CreateRequest`.
            $$"""{"trade_type":"BUY","qty":1,"unit_price":5,"amount":5,"risk_event_id":"{{Guid.NewGuid()}}"}""",
            // Meneruskan nilai literal `”MON”` sebagai argumen bernama `weekday`.
            weekday: "MON");

        // Menjalankan memanggil `new EventEconomyActionValidator().TryValidate` dengan `request`, `CreateConfig()`, `Array.Empty<EventDb>()`, `var result`
        // dalam TryValidate_GoldTradeRejectsFakeRiskReferenceOutsideSaturday.
        new EventEconomyActionValidator().TryValidate(request, CreateConfig(), Array.Empty<EventDb>(), out var result);

        // Menjalankan pemeriksaan bahwa `result.Validation.IsValid` bernilai salah; pengujian gagal jika kondisi justru terpenuhi dalam
        // TryValidate_GoldTradeRejectsFakeRiskReferenceOutsideSaturday.
        Assert.False(result.Validation.IsValid);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`StatusCodes.Status400BadRequest`,
        // `result.Validation.StatusCode`); pengujian gagal jika keduanya berbeda dalam TryValidate_GoldTradeRejectsFakeRiskReferenceOutsideSaturday.
        Assert.Equal(StatusCodes.Status400BadRequest, result.Validation.StatusCode);
    // Menutup scope metode TryValidate_GoldTradeRejectsFakeRiskReferenceOutsideSaturday; bagian berikut berada di luar batas blok tersebut dalam
    // TryValidate_GoldTradeRejectsFakeRiskReferenceOutsideSaturday.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `TryValidate_GoldPriceRejectsValueOutsideRulesetDeck` dengan hasil bertipe `void`; operasi ini menangani try validate emas
    // harga rejects nilai outside aturan deck.
    public void TryValidate_GoldPriceRejectsValueOutsideRulesetDeck()
    // Membuka scope metode TryValidate_GoldPriceRejectsValueOutsideRulesetDeck; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // TryValidate_GoldPriceRejectsValueOutsideRulesetDeck.
    {
        // Menyiapkan variabel lokal `request` untuk data masukan permintaan yang akan divalidasi atau diteruskan ke layanan dengan memanggil
        // `CreateRequest` dengan `”BukaHargaEmas”`, `”””{”gold_price”:99}”””`, `”SAT”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var request = CreateRequest("BukaHargaEmas", """{"gold_price":99}""", weekday: "SAT");

        // Menjalankan memanggil `new EventEconomyActionValidator().TryValidate` dengan `request`, `CreateConfig()`, `[]`, `var result` dalam
        // TryValidate_GoldPriceRejectsValueOutsideRulesetDeck.
        new EventEconomyActionValidator().TryValidate(request, CreateConfig(), [], out var result);

        // Menjalankan pemeriksaan bahwa `result.Validation.IsValid` bernilai salah; pengujian gagal jika kondisi justru terpenuhi dalam
        // TryValidate_GoldPriceRejectsValueOutsideRulesetDeck.
        Assert.False(result.Validation.IsValid);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”Kartu Harga Emas”`,
        // `result.Validation.Message` dalam TryValidate_GoldPriceRejectsValueOutsideRulesetDeck.
        Assert.Contains("Kartu Harga Emas", result.Validation.Message);
    // Menutup scope metode TryValidate_GoldPriceRejectsValueOutsideRulesetDeck; bagian berikut berada di luar batas blok tersebut dalam
    // TryValidate_GoldPriceRejectsValueOutsideRulesetDeck.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `TryValidate_GoldBuyRejectsWhenTwentyPhysicalCardsAreHeld` dengan hasil bertipe `void`; operasi ini menangani try validate
    // emas buy rejects when twenty physical kartu are held.
    public void TryValidate_GoldBuyRejectsWhenTwentyPhysicalCardsAreHeld()
    // Membuka scope metode TryValidate_GoldBuyRejectsWhenTwentyPhysicalCardsAreHeld; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // TryValidate_GoldBuyRejectsWhenTwentyPhysicalCardsAreHeld.
    {
        // Menyiapkan variabel lokal `playerId` untuk nilai pemain identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var playerId = Guid.NewGuid();
        // Menyiapkan variabel lokal `request` untuk data masukan permintaan yang akan divalidasi atau diteruskan ke layanan dengan memanggil
        // `CreateRequest` dengan `”InvestasiEmas”`, `”””{”trade_type”:”BUY”,”qty”:1,”unit_price”:5,”amount”:5}”””`, `playerId`, `”SAT”`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var request = CreateRequest(
            // Meneruskan nilai literal `”InvestasiEmas”` sebagai argumen ke `CreateRequest`.
            "InvestasiEmas",
            // Meneruskan nilai literal `”””{”trade_type”:”BUY”,”qty”:1,”unit_price”:5,”amount”:5}”””` sebagai argumen ke `CreateRequest`.
            """{"trade_type":"BUY","qty":1,"unit_price":5,"amount":5}""",
            // Meneruskan `playerId` (nilai pemain identitas) sebagai argumen ke `CreateRequest`.
            playerId,
            // Meneruskan nilai literal `”SAT”` sebagai argumen bernama `weekday`.
            weekday: "SAT");
        // Menyiapkan variabel lokal `history` untuk nilai history dengan mematerialisasi urutan `Enumerable.Range(0, 20) .Select(_ =>
        // CreateEvent(Guid.NewGuid(), ”SetupEmasAwal”, ”””{”qty”:1}”””, sessionId: request.SessionId)) .Append(CreateEvent(playerId, ”BukaHargaEmas”...`
        // menjadi array dengan elemen hasil saat ini. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var history = Enumerable.Range(0, 20)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Select(_ => CreateEvent(Guid.NewGuid(), ”SetupEmasAwal”, ”””{”qty”:1}”””,
            // sessionId: request.SessionId)) dalam TryValidate_GoldBuyRejectsWhenTwentyPhysicalCardsAreHeld; token pada baris ini menyambungkan bagian kode
            // sebelum dan sesudahnya.
            .Select(_ => CreateEvent(Guid.NewGuid(), "SetupEmasAwal", """{"qty":1}""", sessionId: request.SessionId))
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Append(CreateEvent(playerId, ”BukaHargaEmas”, ”””{”gold_price”:5}”””,
            // sessionId: request.SessionId)) dalam TryValidate_GoldBuyRejectsWhenTwentyPhysicalCardsAreHeld; token pada baris ini menyambungkan bagian kode
            // sebelum dan sesudahnya.
            .Append(CreateEvent(playerId, "BukaHargaEmas", """{"gold_price":5}""", sessionId: request.SessionId))
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToArray(); dalam TryValidate_GoldBuyRejectsWhenTwentyPhysicalCardsAreHeld;
            // token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .ToArray();

        // Menjalankan memanggil `new EventEconomyActionValidator().TryValidate` dengan `request`, `CreateConfig()`, `history`, `var result` dalam
        // TryValidate_GoldBuyRejectsWhenTwentyPhysicalCardsAreHeld.
        new EventEconomyActionValidator().TryValidate(request, CreateConfig(), history, out var result);

        // Menjalankan pemeriksaan bahwa `result.Validation.IsValid` bernilai salah; pengujian gagal jika kondisi justru terpenuhi dalam
        // TryValidate_GoldBuyRejectsWhenTwentyPhysicalCardsAreHeld.
        Assert.False(result.Validation.IsValid);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”Stok fisik”`,
        // `result.Validation.Message` dalam TryValidate_GoldBuyRejectsWhenTwentyPhysicalCardsAreHeld.
        Assert.Contains("Stok fisik", result.Validation.Message);
    // Menutup scope metode TryValidate_GoldBuyRejectsWhenTwentyPhysicalCardsAreHeld; bagian berikut berada di luar batas blok tersebut dalam
    // TryValidate_GoldBuyRejectsWhenTwentyPhysicalCardsAreHeld.
    }

    // Mendefinisikan metode `CreateRequest` dengan hasil bertipe `EventRequest`; operasi ini menangani create permintaan. Masukan: Parameter
    // `actionType` bertipe `string` membawa nilai aksi jenis; Parameter `payloadJson` bertipe `string` membawa nilai payload JSON; Parameter `playerId`
    // bertipe `Guid?` membawa nilai pemain identitas; nilai null diizinkan ketika data opsional belum tersedia; bila argumen tidak diberikan digunakan
    // null, yaitu penanda tidak ada nilai; Parameter `weekday` bertipe `string` membawa nilai weekday; bila argumen tidak diberikan digunakan nilai
    // literal `”MON”`; Parameter `dayIndex` bertipe `int` membawa nilai hari index; bila argumen tidak diberikan digunakan nilai literal `0`.
    private static EventRequest CreateRequest(
        // Parameter `actionType` bertipe `string` membawa nilai aksi jenis.
        string actionType,
        // Parameter `payloadJson` bertipe `string` membawa nilai payload JSON.
        string payloadJson,
        // Parameter `playerId` bertipe `Guid?` membawa nilai pemain identitas; nilai null diizinkan ketika data opsional belum tersedia; bila argumen tidak
        // diberikan digunakan null, yaitu penanda tidak ada nilai.
        Guid? playerId = null,
        // Parameter `weekday` bertipe `string` membawa nilai weekday; bila argumen tidak diberikan digunakan nilai literal `”MON”`.
        string weekday = "MON",
        // Parameter `dayIndex` bertipe `int` membawa nilai hari index; bila argumen tidak diberikan digunakan nilai literal `0`.
        int dayIndex = 0)
    // Membuka scope metode CreateRequest; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CreateRequest.
    {
        // Menyiapkan variabel lokal `document` untuk nilai document dengan memanggil `JsonDocument.Parse` dengan `payloadJson`. Tipe variabel disimpulkan
        // dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var document = JsonDocument.Parse(payloadJson);
        // Mengembalikan objek baru bertipe `EventRequest` dengan argumen ( Guid.NewGuid(), Guid.NewGuid(), playerId ?? Guid.NewGuid(), ”PLAYER”, new
        // DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero), dayIndex, weekday, 1, 0, actionT... kepada pemanggil dalam CreateRequest; eksekusi jalur ini
        // selesai setelah nilai hasil ditentukan.
        return new EventRequest(
            // Meneruskan memanggil `Guid.NewGuid` dengan tanpa argumen sebagai argumen ke konstruktor `EventRequest`.
            Guid.NewGuid(),
            // Meneruskan memanggil `Guid.NewGuid` dengan tanpa argumen sebagai argumen ke konstruktor `EventRequest`.
            Guid.NewGuid(),
            // Meneruskan `playerId` bila tidak null; jika null gunakan `Guid.NewGuid()` sebagai nilai pengganti sebagai argumen ke konstruktor `EventRequest`.
            playerId ?? Guid.NewGuid(),
            // Meneruskan nilai literal `”PLAYER”` sebagai argumen ke konstruktor `EventRequest`.
            "PLAYER",
            // Meneruskan objek baru bertipe `DateTimeOffset` dengan argumen (2026, 1, 2, 3, 4, 5, TimeSpan.Zero) sebagai argumen ke konstruktor `EventRequest`;
            // Meneruskan nilai literal `2026` sebagai argumen ke konstruktor `DateTimeOffset`; Meneruskan nilai literal `1` sebagai argumen ke konstruktor
            // `DateTimeOffset`; Meneruskan nilai literal `2` sebagai argumen ke konstruktor `DateTimeOffset`; Meneruskan nilai literal `3` sebagai argumen ke
            // konstruktor `DateTimeOffset`; Meneruskan nilai literal `4` sebagai argumen ke konstruktor `DateTimeOffset`; Meneruskan nilai literal `5` sebagai
            // argumen ke konstruktor `DateTimeOffset`; Meneruskan `TimeSpan.Zero` (nilai zero) sebagai argumen ke konstruktor `DateTimeOffset`.
            new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero),
            // Meneruskan `dayIndex` (nilai hari index) sebagai argumen ke konstruktor `EventRequest`.
            dayIndex,
            // Meneruskan `weekday` (nilai weekday) sebagai argumen ke konstruktor `EventRequest`.
            weekday,
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

    // Mendefinisikan metode `CreateEvent` dengan hasil bertipe `EventDb`; operasi ini menangani create event. Masukan: Parameter `playerId` bertipe
    // `Guid` membawa nilai pemain identitas; Parameter `actionType` bertipe `string` membawa nilai aksi jenis; Parameter `payload` bertipe `string`
    // membawa muatan detail event dalam format JSON; Parameter `eventId` bertipe `Guid?` membawa identitas unik event untuk pencatatan dan pemeriksaan
    // duplikasi; nilai null diizinkan ketika data opsional belum tersedia; bila argumen tidak diberikan digunakan null, yaitu penanda tidak ada nilai;
    // Parameter `sessionId` bertipe `Guid?` membawa identitas unik sesi permainan yang menjadi batas data operasi ini; nilai null diizinkan ketika data
    // opsional belum tersedia; bila argumen tidak diberikan digunakan null, yaitu penanda tidak ada nilai; Parameter `dayIndex` bertipe `int` membawa
    // nilai hari index; bila argumen tidak diberikan digunakan nilai literal `0`.
    private static EventDb CreateEvent(
        // Parameter `playerId` bertipe `Guid` membawa nilai pemain identitas.
        Guid playerId,
        // Parameter `actionType` bertipe `string` membawa nilai aksi jenis.
        string actionType,
        // Parameter `payload` bertipe `string` membawa muatan detail event dalam format JSON.
        string payload,
        // Parameter `eventId` bertipe `Guid?` membawa identitas unik event untuk pencatatan dan pemeriksaan duplikasi; nilai null diizinkan ketika data
        // opsional belum tersedia; bila argumen tidak diberikan digunakan null, yaitu penanda tidak ada nilai.
        Guid? eventId = null,
        // Parameter `sessionId` bertipe `Guid?` membawa identitas unik sesi permainan yang menjadi batas data operasi ini; nilai null diizinkan ketika data
        // opsional belum tersedia; bila argumen tidak diberikan digunakan null, yaitu penanda tidak ada nilai.
        Guid? sessionId = null,
        // Parameter `dayIndex` bertipe `int` membawa nilai hari index; bila argumen tidak diberikan digunakan nilai literal `0`.
        int dayIndex = 0)
    // Membuka scope metode CreateEvent; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CreateEvent.
    {
        // Mengembalikan objek baru bertipe `EventDb` dengan nilai awal sesuai konstruktornya kepada pemanggil dalam CreateEvent; eksekusi jalur ini selesai
        // setelah nilai hasil ditentukan.
        return new EventDb
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CreateEvent.
        {
            // Memperbarui `EventId` menggunakan `eventId` bila tidak null; jika null gunakan `Guid.NewGuid()` sebagai nilai pengganti dalam CreateEvent.
            EventId = eventId ?? Guid.NewGuid(),
            // Memperbarui `SessionId` menggunakan `sessionId` bila tidak null; jika null gunakan `Guid.NewGuid()` sebagai nilai pengganti dalam CreateEvent.
            SessionId = sessionId ?? Guid.NewGuid(),
            // Memperbarui `UserId` menggunakan `playerId` (nilai pemain identitas) dalam CreateEvent.
            UserId = playerId,
            // Memperbarui `ActorType` menggunakan nilai literal `”PLAYER”` dalam CreateEvent.
            ActorType = "PLAYER",
            // Memperbarui `Timestamp` menggunakan objek baru bertipe `DateTimeOffset` dengan argumen (2026, 1, 2, 3, 4, 5, TimeSpan.Zero) dalam CreateEvent.
            Timestamp = new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero),
            // Memperbarui `DayIndex` menggunakan `dayIndex` (nilai hari index) dalam CreateEvent.
            DayIndex = dayIndex,
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

    // Mendefinisikan metode `CreateConfig` dengan hasil bertipe `RulesetConfig`; operasi ini menangani create konfigurasi.
    private static RulesetConfig CreateConfig()
    // Membuka scope metode CreateConfig; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CreateConfig.
    {
        // Mengembalikan objek baru bertipe `RulesetConfig` dengan argumen ( ”PEMULA”, ActionsPerTurn: 3, StartingCash: 20, PlayerOrdering.PlayerOrder,
        // CashMin: 0, MaxIngredientTotal: 10, MaxSameIngredient: 5, PrimaryNeedMaxPerDay: 1, ... kepada pemanggil dalam CreateConfig; eksekusi jalur ini
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
            // Meneruskan nilai literal `10` sebagai argumen bernama `MaxIngredientTotal`.
            MaxIngredientTotal: 10,
            // Meneruskan nilai literal `5` sebagai argumen bernama `MaxSameIngredient`.
            MaxSameIngredient: 5,
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
            // Memperbarui `GoldPrices` menggunakan koleksi berisi new RulesetGoldPriceDto { PriceCode = ”harga_5”, Q..., new RulesetGoldPriceDto { PriceCode =
            // ”harga_6”, Q... dalam CreateConfig.
            GoldPrices =
            // Menggunakan koleksi berisi new RulesetGoldPriceDto { PriceCode = ”harga_5”, Q..., new RulesetGoldPriceDto { PriceCode = ”harga_6”, Q... sebagai
            // bagian ekspresi yang sedang disusun dalam CreateConfig.
            [
                // Menggunakan objek baru bertipe `RulesetGoldPriceDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun dalam
                // CreateConfig.
                new RulesetGoldPriceDto { PriceCode = "harga_5", Qty = 1, UnitPrice = 5, CardQty = 1 },
                // Menggunakan objek baru bertipe `RulesetGoldPriceDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun dalam
                // CreateConfig.
                new RulesetGoldPriceDto { PriceCode = "harga_6", Qty = 1, UnitPrice = 6, CardQty = 1 }
            // Menandai akhir daftar elemen atau indeks koleksi dalam CreateConfig; pasangan kurung siku mengelompokkan nilai sebagai satu struktur.
            ],
            // Memperbarui `LifeRisks` menggunakan koleksi berisi new RulesetLifeRiskDto { RiskCode = ”risk_gold”, I... dalam CreateConfig.
            LifeRisks =
            // Menggunakan koleksi berisi new RulesetLifeRiskDto { RiskCode = ”risk_gold”, I... sebagai bagian ekspresi yang sedang disusun dalam CreateConfig.
            [
                // Menggunakan objek baru bertipe `RulesetLifeRiskDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun dalam
                // CreateConfig.
                new RulesetLifeRiskDto
                // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CreateConfig.
                {
                    // Memperbarui `RiskCode` menggunakan nilai literal `”risk_gold”` dalam CreateConfig.
                    RiskCode = "risk_gold",
                    // Memperbarui `ItemName` menggunakan nilai literal `”Gold trade”` dalam CreateConfig.
                    ItemName = "Gold trade",
                    // Memperbarui `EffectType` menggunakan nilai literal `”GOLD_TRADE”` dalam CreateConfig.
                    EffectType = "GOLD_TRADE",
                    // Memperbarui `Direction` menggunakan nilai literal `”IN”` dalam CreateConfig.
                    Direction = "IN",
                    // Memperbarui `Amount` menggunakan nilai literal `0` dalam CreateConfig.
                    Amount = 0,
                    // Memperbarui `DurationDays` menggunakan nilai literal `1` dalam CreateConfig.
                    DurationDays = 1
                // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam CreateConfig.
                }
            // Menandai akhir daftar elemen atau indeks koleksi dalam CreateConfig; pasangan kurung siku mengelompokkan nilai sebagai satu struktur.
            ]
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam CreateConfig.
        };
    // Menutup scope metode CreateConfig; bagian berikut berada di luar batas blok tersebut dalam CreateConfig.
    }
// Menutup scope tipe EventEconomyActionValidatorTests; bagian berikut berada di luar batas blok tersebut.
}
