// Fungsi file: Memverifikasi perilaku, lokalisasi, atau tata letak UI melalui RulesetFormHelperTests.
// Mengimpor namespace `System.Net` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Net;
// Mengimpor namespace `System.Text` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text;
// Mengimpor namespace `System.Text.Json` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.Json;
// Mengimpor namespace `System.Text.Json.Nodes` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.Json.Nodes;
// Mengimpor namespace `Cashflowpoly.Ui.Infrastructure` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Ui.Infrastructure;
// Mengimpor namespace `Xunit` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Xunit;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Tests` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Tests;

// Mendefinisikan tipe class `RulesetFormHelperTests`; sealed mencegah tipe ini diturunkan lagi.
public sealed class RulesetFormHelperTests
// Membuka scope tipe RulesetFormHelperTests; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `BuildDefaultCreateViewModel_ReturnsParsableBeginnerConfig` dengan hasil bertipe `void`; operasi ini menangani build bawaan
    // create view model returns parsable beginner konfigurasi.
    public void BuildDefaultCreateViewModel_ReturnsParsableBeginnerConfig()
    // Membuka scope metode BuildDefaultCreateViewModel_ReturnsParsableBeginnerConfig; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // BuildDefaultCreateViewModel_ReturnsParsableBeginnerConfig.
    {
        // Menyiapkan variabel lokal `model` untuk nilai model dengan memanggil `RulesetFormHelper.BuildDefaultCreateViewModel` dengan tanpa argumen. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var model = RulesetFormHelper.BuildDefaultCreateViewModel();

        // Menjalankan pemeriksaan bahwa `model.IsEditMode` bernilai salah; pengujian gagal jika kondisi justru terpenuhi dalam
        // BuildDefaultCreateViewModel_ReturnsParsableBeginnerConfig.
        Assert.False(model.IsEditMode);
        // Menyiapkan variabel lokal `document` untuk nilai document dengan memanggil `JsonDocument.Parse` dengan `model.DefinitionJson`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var document = JsonDocument.Parse(model.DefinitionJson);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”PEMULA”`,
        // `document.RootElement.GetProperty(”mode”).GetString()`); pengujian gagal jika keduanya berbeda dalam
        // BuildDefaultCreateViewModel_ReturnsParsableBeginnerConfig.
        Assert.Equal("PEMULA", document.RootElement.GetProperty("mode").GetString());
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`20`,
        // `document.RootElement.GetProperty(”starting_cash”).GetInt32()`); pengujian gagal jika keduanya berbeda dalam
        // BuildDefaultCreateViewModel_ReturnsParsableBeginnerConfig.
        Assert.Equal(20, document.RootElement.GetProperty("starting_cash").GetInt32());
    // Menutup scope metode BuildDefaultCreateViewModel_ReturnsParsableBeginnerConfig; bagian berikut berada di luar batas blok tersebut dalam
    // BuildDefaultCreateViewModel_ReturnsParsableBeginnerConfig.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `TryResolveMode_ReturnsUppercaseModeForValidConfig` dengan hasil bertipe `void`; operasi ini menangani try resolve mode
    // returns uppercase mode untuk valid konfigurasi.
    public void TryResolveMode_ReturnsUppercaseModeForValidConfig()
    // Membuka scope metode TryResolveMode_ReturnsUppercaseModeForValidConfig; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // TryResolveMode_ReturnsUppercaseModeForValidConfig.
    {
        // Menyiapkan variabel lokal `config` untuk konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan dengan memanggil
        // `JsonNode.Parse(”””{”mode”:”mahir”}”””)!.AsObject` dengan tanpa argumen. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var config = JsonNode.Parse("""{"mode":"mahir"}""")!.AsObject();

        // Menyiapkan variabel lokal `ok` untuk nilai ok dengan memanggil `RulesetFormHelper.TryResolveMode` dengan `config`, `var mode`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var ok = RulesetFormHelper.TryResolveMode(config, out var mode);

        // Menjalankan pemeriksaan bahwa `ok` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // TryResolveMode_ReturnsUppercaseModeForValidConfig.
        Assert.True(ok);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”MAHIR”`, `mode`); pengujian gagal jika
        // keduanya berbeda dalam TryResolveMode_ReturnsUppercaseModeForValidConfig.
        Assert.Equal("MAHIR", mode);
    // Menutup scope metode TryResolveMode_ReturnsUppercaseModeForValidConfig; bagian berikut berada di luar batas blok tersebut dalam
    // TryResolveMode_ReturnsUppercaseModeForValidConfig.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `SerializeIndentedJson_FormatsValidJsonAndFallsBackForNull` dengan hasil bertipe `void`; operasi ini menangani serialize
    // indented JSON formats valid JSON dan falls back untuk null.
    public void SerializeIndentedJson_FormatsValidJsonAndFallsBackForNull()
    // Membuka scope metode SerializeIndentedJson_FormatsValidJsonAndFallsBackForNull; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // SerializeIndentedJson_FormatsValidJsonAndFallsBackForNull.
    {
        // Menyiapkan variabel lokal `document` untuk nilai document dengan memanggil `JsonDocument.Parse` dengan
        // `”””{”mode”:”PEMULA”,”starting_cash”:20}”””`. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis
        // saat scope berakhir.
        using var document = JsonDocument.Parse("""{"mode":"PEMULA","starting_cash":20}""");

        // Menyiapkan variabel lokal `formatted` untuk nilai formatted dengan memanggil `RulesetFormHelper.SerializeIndentedJson` dengan
        // `document.RootElement`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var formatted = RulesetFormHelper.SerializeIndentedJson(document.RootElement);

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `Environment.NewLine`, `formatted` dalam
        // SerializeIndentedJson_FormatsValidJsonAndFallsBackForNull.
        Assert.Contains(Environment.NewLine, formatted);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”{}”`,
        // `RulesetFormHelper.SerializeIndentedJson(null)`); pengujian gagal jika keduanya berbeda dalam
        // SerializeIndentedJson_FormatsValidJsonAndFallsBackForNull.
        Assert.Equal("{}", RulesetFormHelper.SerializeIndentedJson(null));
    // Menutup scope metode SerializeIndentedJson_FormatsValidJsonAndFallsBackForNull; bagian berikut berada di luar batas blok tersebut dalam
    // SerializeIndentedJson_FormatsValidJsonAndFallsBackForNull.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `BuildRulesetApiErrorMessage_UsesApiErrorMessageWhenAvailable` dengan hasil bertipe `Task`; operasi ini menangani build
    // aturan api kesalahan pesan uses api kesalahan pesan when tersedia. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan
    // penyelesaian melalui Task.
    public async Task BuildRulesetApiErrorMessage_UsesApiErrorMessageWhenAvailable()
    // Membuka scope metode BuildRulesetApiErrorMessage_UsesApiErrorMessageWhenAvailable; pernyataan/deklarasi berikut berada di dalam batas blok ini
    // dalam BuildRulesetApiErrorMessage_UsesApiErrorMessageWhenAvailable.
    {
        // Menyiapkan variabel lokal `response` untuk hasil respons yang akan dibaca atau dikirim kepada pemanggil dengan objek baru bertipe
        // `HttpResponseMessage` dengan argumen (HttpStatusCode.UnprocessableEntity). Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan
        // sumber daya dilepas otomatis saat scope berakhir.
        using var response = new HttpResponseMessage(HttpStatusCode.UnprocessableEntity)
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // BuildRulesetApiErrorMessage_UsesApiErrorMessageWhenAvailable.
        {
            // Memperbarui `Content` menggunakan objek baru bertipe `StringContent` dengan argumen (”””{”message”:”Config tidak valid”}”””, Encoding.UTF8,
            // ”application/json”) dalam BuildRulesetApiErrorMessage_UsesApiErrorMessageWhenAvailable.
            Content = new StringContent("""{"message":"Config tidak valid"}""", Encoding.UTF8, "application/json")
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // BuildRulesetApiErrorMessage_UsesApiErrorMessageWhenAvailable.
        };

        // Menyiapkan variabel lokal `message` untuk nilai pesan dengan hasil operasi asinkron memanggil `RulesetFormHelper.BuildRulesetApiErrorMessage`
        // dengan `response`, `”Fallback”`, `CancellationToken.None`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var message = await RulesetFormHelper.BuildRulesetApiErrorMessage(response, "Fallback", CancellationToken.None);

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”Config tidak valid”`, `message`); pengujian
        // gagal jika keduanya berbeda dalam BuildRulesetApiErrorMessage_UsesApiErrorMessageWhenAvailable.
        Assert.Equal("Config tidak valid", message);
    // Menutup scope metode BuildRulesetApiErrorMessage_UsesApiErrorMessageWhenAvailable; bagian berikut berada di luar batas blok tersebut dalam
    // BuildRulesetApiErrorMessage_UsesApiErrorMessageWhenAvailable.
    }
// Menutup scope tipe RulesetFormHelperTests; bagian berikut berada di luar batas blok tersebut.
}
