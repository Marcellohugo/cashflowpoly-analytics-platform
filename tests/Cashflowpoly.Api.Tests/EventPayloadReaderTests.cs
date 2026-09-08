// Fungsi file: Memverifikasi perilaku API, database, atau domain melalui EventPayloadReaderTests.
// Mengimpor namespace `System.Text.Json` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.Json;
// Mengimpor namespace `Cashflowpoly.Api.Domain` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Domain;
// Mengimpor namespace `Xunit` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Xunit;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Tests` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Tests;

// Mendefinisikan tipe class `EventPayloadReaderTests`; sealed mencegah tipe ini diturunkan lagi.
public sealed class EventPayloadReaderTests
// Membuka scope tipe EventPayloadReaderTests; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `TryReadNeedPurchase_RequiresNonBlankCardId` dengan hasil bertipe `void`; operasi ini menangani try read kebutuhan
    // pembelian requires non blank kartu identitas.
    public void TryReadNeedPurchase_RequiresNonBlankCardId()
    // Membuka scope metode TryReadNeedPurchase_RequiresNonBlankCardId; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // TryReadNeedPurchase_RequiresNonBlankCardId.
    {
        // Menyiapkan variabel lokal `payload` untuk muatan detail event dalam format JSON dengan memanggil `Parse` dengan `”””{”card_id”:”
        // ”,”amount”:4,”points”:2}”””`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var payload = Parse("""{"card_id":" ","amount":4,"points":2}""");

        // Menyiapkan variabel lokal `ok` untuk nilai ok dengan memanggil `new EventPayloadReader().TryReadNeedPurchase` dengan `payload`, `var cardId`,
        // `var amount`, `var points`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var ok = new EventPayloadReader().TryReadNeedPurchase(payload, out var cardId, out var amount, out var points);

        // Menjalankan pemeriksaan bahwa `ok` bernilai salah; pengujian gagal jika kondisi justru terpenuhi dalam
        // TryReadNeedPurchase_RequiresNonBlankCardId.
        Assert.False(ok);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`string.Empty`, `cardId`); pengujian gagal jika
        // keduanya berbeda dalam TryReadNeedPurchase_RequiresNonBlankCardId.
        Assert.Equal(string.Empty, cardId);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`4`, `amount`); pengujian gagal jika keduanya
        // berbeda dalam TryReadNeedPurchase_RequiresNonBlankCardId.
        Assert.Equal(4, amount);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`2`, `points`); pengujian gagal jika keduanya
        // berbeda dalam TryReadNeedPurchase_RequiresNonBlankCardId.
        Assert.Equal(2, points);
    // Menutup scope metode TryReadNeedPurchase_RequiresNonBlankCardId; bagian berikut berada di luar batas blok tersebut dalam
    // TryReadNeedPurchase_RequiresNonBlankCardId.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `TryReadEmergencyOption_ReadsRiskReferenceAndCashflow` dengan hasil bertipe `void`; operasi ini menangani try read
    // emergency option reads risiko reference dan arus kas.
    public void TryReadEmergencyOption_ReadsRiskReferenceAndCashflow()
    // Membuka scope metode TryReadEmergencyOption_ReadsRiskReferenceAndCashflow; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // TryReadEmergencyOption_ReadsRiskReferenceAndCashflow.
    {
        // Menyiapkan variabel lokal `payload` untuk muatan detail event dalam format JSON dengan memanggil `Parse` dengan `””” { ”risk_event_id”:
        // ”risk-123”, ”option_type”: ”SELL_GOLD”, ”amount”: 6 } ”””`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `var payload = Parse(”””`.
        // Baris literal 2: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `{`.
        // Baris literal 3: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”risk_event_id”: ”risk-123”,`.
        // Baris literal 4: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”option_type”: ”SELL_GOLD”,`.
        // Baris literal 5: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”amount”: 6`.
        // Baris literal 6: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `}`.
        // Baris literal 7: Pembatas literal/penutup `”””);`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        var payload = Parse("""
            {
              "risk_event_id": "risk-123",
              "option_type": "SELL_GOLD",
              "amount": 6
            }
            """);

        // Menyiapkan variabel lokal `ok` untuk nilai ok dengan memanggil `new EventPayloadReader().TryReadEmergencyOption` dengan `payload`, `var
        // riskEventId`, `var optionType`, `var direction`, `var amount`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var ok = new EventPayloadReader().TryReadEmergencyOption(
            // Meneruskan `payload` (muatan detail event dalam format JSON) sebagai argumen ke `new EventPayloadReader().TryReadEmergencyOption`.
            payload,
            // Meneruskan `var riskEventId` sebagai argumen ke `new EventPayloadReader().TryReadEmergencyOption`.
            out var riskEventId,
            // Meneruskan `var optionType` sebagai argumen ke `new EventPayloadReader().TryReadEmergencyOption`.
            out var optionType,
            // Meneruskan `var direction` sebagai argumen ke `new EventPayloadReader().TryReadEmergencyOption`.
            out var direction,
            // Meneruskan `var amount` sebagai argumen ke `new EventPayloadReader().TryReadEmergencyOption`.
            out var amount);

        // Menjalankan pemeriksaan bahwa `ok` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // TryReadEmergencyOption_ReadsRiskReferenceAndCashflow.
        Assert.True(ok);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”risk-123”`, `riskEventId`); pengujian gagal
        // jika keduanya berbeda dalam TryReadEmergencyOption_ReadsRiskReferenceAndCashflow.
        Assert.Equal("risk-123", riskEventId);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”SELL_GOLD”`, `optionType`); pengujian gagal
        // jika keduanya berbeda dalam TryReadEmergencyOption_ReadsRiskReferenceAndCashflow.
        Assert.Equal("SELL_GOLD", optionType);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”IN”`, `direction`); pengujian gagal jika
        // keduanya berbeda dalam TryReadEmergencyOption_ReadsRiskReferenceAndCashflow.
        Assert.Equal("IN", direction);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`6`, `amount`); pengujian gagal jika keduanya
        // berbeda dalam TryReadEmergencyOption_ReadsRiskReferenceAndCashflow.
        Assert.Equal(6, amount);
    // Menutup scope metode TryReadEmergencyOption_ReadsRiskReferenceAndCashflow; bagian berikut berada di luar batas blok tersebut dalam
    // TryReadEmergencyOption_ReadsRiskReferenceAndCashflow.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `TryReadOrderClaim_RejectsNonStringCardIds` dengan hasil bertipe `void`; operasi ini menangani try read urutan/pesanan
    // claim rejects non string kartu identitas.
    public void TryReadOrderClaim_RejectsNonStringCardIds()
    // Membuka scope metode TryReadOrderClaim_RejectsNonStringCardIds; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // TryReadOrderClaim_RejectsNonStringCardIds.
    {
        // Menyiapkan variabel lokal `payload` untuk muatan detail event dalam format JSON dengan memanggil `Parse` dengan
        // `”””{”required_ingredient_card_ids”:[”card-1”,7],”income”:10}”””`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var payload = Parse("""{"required_ingredient_card_ids":["card-1",7],"income":10}""");

        // Menyiapkan variabel lokal `ok` untuk nilai ok dengan memanggil `new EventPayloadReader().TryReadOrderClaim` dengan `payload`, `var
        // requiredCards`, `var income`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var ok = new EventPayloadReader().TryReadOrderClaim(payload, out var requiredCards, out var income);

        // Menjalankan pemeriksaan bahwa `ok` bernilai salah; pengujian gagal jika kondisi justru terpenuhi dalam TryReadOrderClaim_RejectsNonStringCardIds.
        Assert.False(ok);
        // Menjalankan pemeriksaan hasil dengan `Assert.Empty` menggunakan `requiredCards`; ketidaksesuaian dengan ekspektasi membuat pengujian gagal dalam
        // TryReadOrderClaim_RejectsNonStringCardIds.
        Assert.Empty(requiredCards);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`10`, `income`); pengujian gagal jika keduanya
        // berbeda dalam TryReadOrderClaim_RejectsNonStringCardIds.
        Assert.Equal(10, income);
    // Menutup scope metode TryReadOrderClaim_RejectsNonStringCardIds; bagian berikut berada di luar batas blok tersebut dalam
    // TryReadOrderClaim_RejectsNonStringCardIds.
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
// Menutup scope tipe EventPayloadReaderTests; bagian berikut berada di luar batas blok tersebut.
}
