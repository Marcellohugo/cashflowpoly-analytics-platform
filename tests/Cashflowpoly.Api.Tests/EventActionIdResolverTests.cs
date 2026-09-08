// Fungsi file: Memverifikasi perilaku API, database, atau domain melalui EventActionIdResolverTests.
// Mengimpor namespace `System.Text.Json` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.Json;
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;
// Mengimpor namespace `Cashflowpoly.Api.Domain` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Domain;
// Mengimpor namespace `Xunit` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Xunit;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Tests` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Tests;

// Mendefinisikan tipe class `EventActionIdResolverTests`; sealed mencegah tipe ini diturunkan lagi.
public sealed class EventActionIdResolverTests
// Membuka scope tipe EventActionIdResolverTests; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // menandai metode sebagai pengujian xUnit yang dijalankan untuk setiap kombinasi data.
    [Theory]
    // menyediakan satu kombinasi masukan pengujian (”BahanMasakan”).
    [InlineData("BahanMasakan")]
    // menyediakan satu kombinasi masukan pengujian (”BuangBahanMasakan”).
    [InlineData("BuangBahanMasakan")]
    // menyediakan satu kombinasi masukan pengujian (”JualMasakan”).
    [InlineData("JualMasakan")]
    // menyediakan satu kombinasi masukan pengujian (”Kebutuhan”).
    [InlineData("Kebutuhan")]
    // menyediakan satu kombinasi masukan pengujian (”KerjaLepas”).
    [InlineData("KerjaLepas")]
    // menyediakan satu kombinasi masukan pengujian (”CatatTransaksi”).
    [InlineData("CatatTransaksi")]
    // menyediakan satu kombinasi masukan pengujian (”Menabung”).
    [InlineData("Menabung")]
    // menyediakan satu kombinasi masukan pengujian (”TarikTabungan”).
    [InlineData("TarikTabungan")]
    // menyediakan satu kombinasi masukan pengujian (”TujuanFinansial”).
    [InlineData("TujuanFinansial")]
    // menyediakan satu kombinasi masukan pengujian (”JumatBerkah”).
    [InlineData("JumatBerkah")]
    // menyediakan satu kombinasi masukan pengujian (”InvestasiEmas”).
    [InlineData("InvestasiEmas")]
    // menyediakan satu kombinasi masukan pengujian (”JualEmas”).
    [InlineData("JualEmas")]
    // menyediakan satu kombinasi masukan pengujian (”LewatiTransaksiEmas”).
    [InlineData("LewatiTransaksiEmas")]
    // menyediakan satu kombinasi masukan pengujian (”HariMingguLibur”).
    [InlineData("HariMingguLibur")]
    // menyediakan satu kombinasi masukan pengujian (”PinjamanSyariah”).
    [InlineData("PinjamanSyariah")]
    // menyediakan satu kombinasi masukan pengujian (”BayarPinjaman”).
    [InlineData("BayarPinjaman")]
    // menyediakan satu kombinasi masukan pengujian (”Asuransi”).
    [InlineData("Asuransi")]
    // menyediakan satu kombinasi masukan pengujian (”RisikoKehidupan”).
    [InlineData("RisikoKehidupan")]
    // menyediakan satu kombinasi masukan pengujian (”BayarRisiko”).
    [InlineData("BayarRisiko")]
    // menyediakan satu kombinasi masukan pengujian (”GunakanOpsiDarurat”).
    [InlineData("GunakanOpsiDarurat")]
    // menyediakan satu kombinasi masukan pengujian (”PoinPeringkatDonasi”).
    [InlineData("PoinPeringkatDonasi")]
    // menyediakan satu kombinasi masukan pengujian (”UmumkanJuaraDonasi”).
    [InlineData("UmumkanJuaraDonasi")]
    // menyediakan satu kombinasi masukan pengujian (”PoinEmas”).
    [InlineData("PoinEmas")]
    // menyediakan satu kombinasi masukan pengujian (”PoinPeringkatPensiun”).
    [InlineData("PoinPeringkatPensiun")]
    // menyediakan satu kombinasi masukan pengujian (”BagikanTieBreaker”).
    [InlineData("BagikanTieBreaker")]
    // menyediakan satu kombinasi masukan pengujian (”MulaiSesi”).
    [InlineData("MulaiSesi")]
    // menyediakan satu kombinasi masukan pengujian (”AkhiriSesi”).
    [InlineData("AkhiriSesi")]
    // menyediakan satu kombinasi masukan pengujian (”AkhirGiliran”).
    [InlineData("AkhirGiliran")]
    // Mendefinisikan metode `Resolve_ReturnsPascalCaseGameActionIdUnchanged` dengan hasil bertipe `void`; operasi ini menangani resolve returns pascal
    // case game aksi identitas unchanged. Masukan: Parameter `actionType` bertipe `string` membawa nilai aksi jenis.
    public void Resolve_ReturnsPascalCaseGameActionIdUnchanged(string actionType)
    // Membuka scope metode Resolve_ReturnsPascalCaseGameActionIdUnchanged; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // Resolve_ReturnsPascalCaseGameActionIdUnchanged.
    {
        // Menyiapkan variabel lokal `actionId` untuk kode aksi yang dipetakan terhadap katalog aturan dengan memanggil `EventActionIdResolver.Resolve`
        // dengan `actionType`, `Parse(”””{}”””)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var actionId = EventActionIdResolver.Resolve(actionType, Parse("""{}"""));

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`actionType`, `actionId`); pengujian gagal jika
        // keduanya berbeda dalam Resolve_ReturnsPascalCaseGameActionIdUnchanged.
        Assert.Equal(actionType, actionId);
    // Menutup scope metode Resolve_ReturnsPascalCaseGameActionIdUnchanged; bagian berikut berada di luar batas blok tersebut dalam
    // Resolve_ReturnsPascalCaseGameActionIdUnchanged.
    }

    // menandai metode sebagai pengujian xUnit yang dijalankan untuk setiap kombinasi data.
    [Theory]
    // menyediakan satu kombinasi masukan pengujian (”ingredient.purchased”).
    [InlineData("ingredient.purchased")]
    // menyediakan satu kombinasi masukan pengujian (”ingredient.discarded”).
    [InlineData("ingredient.discarded")]
    // menyediakan satu kombinasi masukan pengujian (”order.claimed”).
    [InlineData("order.claimed")]
    // menyediakan satu kombinasi masukan pengujian (”order.passed”).
    [InlineData("order.passed")]
    // menyediakan satu kombinasi masukan pengujian (”work.freelance.completed”).
    [InlineData("work.freelance.completed")]
    // menyediakan satu kombinasi masukan pengujian (”transaction.recorded”).
    [InlineData("transaction.recorded")]
    // menyediakan satu kombinasi masukan pengujian (”mission.assigned”).
    [InlineData("mission.assigned")]
    // menyediakan satu kombinasi masukan pengujian (”insurance.multirisk.used”).
    [InlineData("insurance.multirisk.used")]
    // menyediakan satu kombinasi masukan pengujian (”saving.deposit.withdrawn”).
    [InlineData("saving.deposit.withdrawn")]
    // menyediakan satu kombinasi masukan pengujian (”risk.emergency.used”).
    [InlineData("risk.emergency.used")]
    // menyediakan satu kombinasi masukan pengujian (”donation.rank.awarded”).
    [InlineData("donation.rank.awarded")]
    // menyediakan satu kombinasi masukan pengujian (”donation.winners.announced”).
    [InlineData("donation.winners.announced")]
    // menyediakan satu kombinasi masukan pengujian (”gold.points.awarded”).
    [InlineData("gold.points.awarded")]
    // menyediakan satu kombinasi masukan pengujian (”pension.rank.awarded”).
    [InlineData("pension.rank.awarded")]
    // menyediakan satu kombinasi masukan pengujian (”day.saturday.gold_trade”).
    [InlineData("day.saturday.gold_trade")]
    // menyediakan satu kombinasi masukan pengujian (”session.started”).
    [InlineData("session.started")]
    // menyediakan satu kombinasi masukan pengujian (”turn.ended”).
    [InlineData("turn.ended")]
    // menyediakan satu kombinasi masukan pengujian (”BagikanEmasAwal”).
    [InlineData("BagikanEmasAwal")]
    // menyediakan satu kombinasi masukan pengujian (”BagikanMisiKoleksi”).
    [InlineData("BagikanMisiKoleksi")]
    // menyediakan satu kombinasi masukan pengujian (”KartuDiambilDariPasar”).
    [InlineData("KartuDiambilDariPasar")]
    // menyediakan satu kombinasi masukan pengujian (”LewatiOrder”).
    [InlineData("LewatiOrder")]
    // menyediakan satu kombinasi masukan pengujian (”AmbilKartuDariDeck”).
    [InlineData("AmbilKartuDariDeck")]
    // menyediakan satu kombinasi masukan pengujian (”KartuMasukDiscard”).
    [InlineData("KartuMasukDiscard")]
    // menyediakan satu kombinasi masukan pengujian (”IsiUlangPasar”).
    [InlineData("IsiUlangPasar")]
    // Mendefinisikan metode `Resolve_ReturnsNull_ForRemovedTechnicalActionTypes` dengan hasil bertipe `void`; operasi ini menangani resolve returns
    // null untuk removed technical aksi types. Masukan: Parameter `actionType` bertipe `string` membawa nilai aksi jenis.
    public void Resolve_ReturnsNull_ForRemovedTechnicalActionTypes(string actionType)
    // Membuka scope metode Resolve_ReturnsNull_ForRemovedTechnicalActionTypes; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // Resolve_ReturnsNull_ForRemovedTechnicalActionTypes.
    {
        // Menyiapkan variabel lokal `resolved` untuk nilai hasil resolusi dengan memanggil `EventActionIdResolver.Resolve` dengan `actionType`,
        // `Parse(”””{”trade_type”:”SELL”}”””)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var resolved = EventActionIdResolver.Resolve(actionType, Parse("""{"trade_type":"SELL"}"""));

        // Menjalankan pemeriksaan Null atas `resolved` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // Resolve_ReturnsNull_ForRemovedTechnicalActionTypes.
        Assert.Null(resolved);
    // Menutup scope metode Resolve_ReturnsNull_ForRemovedTechnicalActionTypes; bagian berikut berada di luar batas blok tersebut dalam
    // Resolve_ReturnsNull_ForRemovedTechnicalActionTypes.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `Resolve_ReturnsNull_ForUnmappedActionTypes` dengan hasil bertipe `void`; operasi ini menangani resolve returns null untuk
    // unmapped aksi types.
    public void Resolve_ReturnsNull_ForUnmappedActionTypes()
    // Membuka scope metode Resolve_ReturnsNull_ForUnmappedActionTypes; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // Resolve_ReturnsNull_ForUnmappedActionTypes.
    {
        // Menyiapkan variabel lokal `actionId` untuk kode aksi yang dipetakan terhadap katalog aturan dengan memanggil `EventActionIdResolver.Resolve`
        // dengan `”unknown.action.type”`, `Parse(”””{”amount”:5}”””)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var actionId = EventActionIdResolver.Resolve("unknown.action.type", Parse("""{"amount":5}"""));

        // Menjalankan pemeriksaan Null atas `actionId` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // Resolve_ReturnsNull_ForUnmappedActionTypes.
        Assert.Null(actionId);
    // Menutup scope metode Resolve_ReturnsNull_ForUnmappedActionTypes; bagian berikut berada di luar batas blok tersebut dalam
    // Resolve_ReturnsNull_ForUnmappedActionTypes.
    }

    // menandai metode sebagai pengujian xUnit yang dijalankan untuk setiap kombinasi data.
    [Theory]
    // menyediakan satu kombinasi masukan pengujian (”BahanMasakan”).
    [InlineData("BahanMasakan")]
    // menyediakan satu kombinasi masukan pengujian (”JualMasakan”).
    [InlineData("JualMasakan")]
    // menyediakan satu kombinasi masukan pengujian (”Kebutuhan”).
    [InlineData("Kebutuhan")]
    // menyediakan satu kombinasi masukan pengujian (”KerjaLepas”).
    [InlineData("KerjaLepas")]
    // menyediakan satu kombinasi masukan pengujian (”Menabung”).
    [InlineData("Menabung")]
    // menyediakan satu kombinasi masukan pengujian (”BayarPinjaman”).
    [InlineData("BayarPinjaman")]
    // Mendefinisikan metode `SlotPolicy_ConsumesRegularPlayerActions` dengan hasil bertipe `void`; operasi ini menangani slot policy consumes regular
    // pemain aksi. Masukan: Parameter `actionType` bertipe `string` membawa nilai aksi jenis.
    public void SlotPolicy_ConsumesRegularPlayerActions(string actionType)
    // Membuka scope metode SlotPolicy_ConsumesRegularPlayerActions; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // SlotPolicy_ConsumesRegularPlayerActions.
    {
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`PlayerActionSlotPolicy.Consumes`,
        // `GameActionCatalog.GetPlayerActionSlotPolicy(actionType, Parse(”{}”))`); pengujian gagal jika keduanya berbeda dalam
        // SlotPolicy_ConsumesRegularPlayerActions.
        Assert.Equal(PlayerActionSlotPolicy.Consumes, GameActionCatalog.GetPlayerActionSlotPolicy(actionType, Parse("{}")));
    // Menutup scope metode SlotPolicy_ConsumesRegularPlayerActions; bagian berikut berada di luar batas blok tersebut dalam
    // SlotPolicy_ConsumesRegularPlayerActions.
    }

    // menandai metode sebagai pengujian xUnit yang dijalankan untuk setiap kombinasi data.
    [Theory]
    // menyediakan satu kombinasi masukan pengujian (”JumatBerkah”).
    [InlineData("JumatBerkah")]
    // menyediakan satu kombinasi masukan pengujian (”RisikoKehidupan”).
    [InlineData("RisikoKehidupan")]
    // menyediakan satu kombinasi masukan pengujian (”BayarRisiko”).
    [InlineData("BayarRisiko")]
    // menyediakan satu kombinasi masukan pengujian (”GunakanOpsiDarurat”).
    [InlineData("GunakanOpsiDarurat")]
    // menyediakan satu kombinasi masukan pengujian (”InvestasiEmas”).
    [InlineData("InvestasiEmas")]
    // menyediakan satu kombinasi masukan pengujian (”JualEmas”).
    [InlineData("JualEmas")]
    // menyediakan satu kombinasi masukan pengujian (”LewatiTransaksiEmas”).
    [InlineData("LewatiTransaksiEmas")]
    // Mendefinisikan metode `SlotPolicy_DoesNotConsumeRulebookFreeActions` dengan hasil bertipe `void`; operasi ini menangani slot policy does not
    // consume rulebook free aksi. Masukan: Parameter `actionType` bertipe `string` membawa nilai aksi jenis.
    public void SlotPolicy_DoesNotConsumeRulebookFreeActions(string actionType)
    // Membuka scope metode SlotPolicy_DoesNotConsumeRulebookFreeActions; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // SlotPolicy_DoesNotConsumeRulebookFreeActions.
    {
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`PlayerActionSlotPolicy.Free`,
        // `GameActionCatalog.GetPlayerActionSlotPolicy(actionType, Parse(”{}”))`); pengujian gagal jika keduanya berbeda dalam
        // SlotPolicy_DoesNotConsumeRulebookFreeActions.
        Assert.Equal(PlayerActionSlotPolicy.Free, GameActionCatalog.GetPlayerActionSlotPolicy(actionType, Parse("{}")));
    // Menutup scope metode SlotPolicy_DoesNotConsumeRulebookFreeActions; bagian berikut berada di luar batas blok tersebut dalam
    // SlotPolicy_DoesNotConsumeRulebookFreeActions.
    }

    // menandai metode sebagai pengujian xUnit yang dijalankan untuk setiap kombinasi data.
    [Theory]
    // menyediakan satu kombinasi masukan pengujian (”Asuransi”).
    [InlineData("Asuransi")]
    // menyediakan satu kombinasi masukan pengujian (”PinjamanSyariah”).
    [InlineData("PinjamanSyariah")]
    // Mendefinisikan metode `SlotPolicy_DoesNotConsumeRiskResponses` dengan hasil bertipe `void`; operasi ini menangani slot policy does not consume
    // risiko responses. Masukan: Parameter `actionType` bertipe `string` membawa nilai aksi jenis.
    public void SlotPolicy_DoesNotConsumeRiskResponses(string actionType)
    // Membuka scope metode SlotPolicy_DoesNotConsumeRiskResponses; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // SlotPolicy_DoesNotConsumeRiskResponses.
    {
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`PlayerActionSlotPolicy.Free`,
        // `GameActionCatalog.GetPlayerActionSlotPolicy(actionType, Parse(”{\”risk_event_id\”:\”95000000-0000-0000-0000-000000000123\”}”))`); pengujian
        // gagal jika keduanya berbeda dalam SlotPolicy_DoesNotConsumeRiskResponses.
        Assert.Equal(
            // Meneruskan `PlayerActionSlotPolicy.Free` (nilai free) sebagai argumen ke `Assert.Equal`.
            PlayerActionSlotPolicy.Free,
            // Meneruskan memanggil `GameActionCatalog.GetPlayerActionSlotPolicy` dengan `actionType`,
            // `Parse(”{\”risk_event_id\”:\”95000000-0000-0000-0000-000000000123\”}”)` sebagai argumen ke `Assert.Equal`; Meneruskan `actionType` (nilai aksi
            // jenis) sebagai argumen ke `GameActionCatalog.GetPlayerActionSlotPolicy`; Meneruskan memanggil `Parse` dengan
            // `”{\”risk_event_id\”:\”95000000-0000-0000-0000-000000000123\”}”` sebagai argumen ke `GameActionCatalog.GetPlayerActionSlotPolicy`; Meneruskan
            // nilai literal `”{\”risk_event_id\”:\”95000000-0000-0000-0000-000000000123\”}”` sebagai argumen ke `Parse`.
            GameActionCatalog.GetPlayerActionSlotPolicy(actionType, Parse("{\"risk_event_id\":\"95000000-0000-0000-0000-000000000123\"}")));
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`PlayerActionSlotPolicy.Consumes`,
        // `GameActionCatalog.GetPlayerActionSlotPolicy(actionType, Parse(”{}”))`); pengujian gagal jika keduanya berbeda dalam
        // SlotPolicy_DoesNotConsumeRiskResponses.
        Assert.Equal(
            // Meneruskan `PlayerActionSlotPolicy.Consumes` (nilai consumes) sebagai argumen ke `Assert.Equal`.
            PlayerActionSlotPolicy.Consumes,
            // Meneruskan memanggil `GameActionCatalog.GetPlayerActionSlotPolicy` dengan `actionType`, `Parse(”{}”)` sebagai argumen ke `Assert.Equal`;
            // Meneruskan `actionType` (nilai aksi jenis) sebagai argumen ke `GameActionCatalog.GetPlayerActionSlotPolicy`; Meneruskan memanggil `Parse` dengan
            // `”{}”` sebagai argumen ke `GameActionCatalog.GetPlayerActionSlotPolicy`; Meneruskan nilai literal `”{}”` sebagai argumen ke `Parse`.
            GameActionCatalog.GetPlayerActionSlotPolicy(actionType, Parse("{}")));
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`PlayerActionSlotPolicy.Consumes`,
        // `GameActionCatalog.GetPlayerActionSlotPolicy(actionType, Parse(”{\”risk_event_id\”:\”fake\”}”))`); pengujian gagal jika keduanya berbeda dalam
        // SlotPolicy_DoesNotConsumeRiskResponses.
        Assert.Equal(
            // Meneruskan `PlayerActionSlotPolicy.Consumes` (nilai consumes) sebagai argumen ke `Assert.Equal`.
            PlayerActionSlotPolicy.Consumes,
            // Meneruskan memanggil `GameActionCatalog.GetPlayerActionSlotPolicy` dengan `actionType`, `Parse(”{\”risk_event_id\”:\”fake\”}”)` sebagai argumen
            // ke `Assert.Equal`; Meneruskan `actionType` (nilai aksi jenis) sebagai argumen ke `GameActionCatalog.GetPlayerActionSlotPolicy`; Meneruskan
            // memanggil `Parse` dengan `”{\”risk_event_id\”:\”fake\”}”` sebagai argumen ke `GameActionCatalog.GetPlayerActionSlotPolicy`; Meneruskan nilai
            // literal `”{\”risk_event_id\”:\”fake\”}”` sebagai argumen ke `Parse`.
            GameActionCatalog.GetPlayerActionSlotPolicy(actionType, Parse("{\"risk_event_id\":\"fake\"}")));
    // Menutup scope metode SlotPolicy_DoesNotConsumeRiskResponses; bagian berikut berada di luar batas blok tersebut dalam
    // SlotPolicy_DoesNotConsumeRiskResponses.
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
// Menutup scope tipe EventActionIdResolverTests; bagian berikut berada di luar batas blok tersebut.
}
