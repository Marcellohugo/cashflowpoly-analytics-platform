// Fungsi file: Memverifikasi perilaku API, database, atau domain melalui SessionStateApiIntegrationTests.
// Mengimpor namespace `System.Net` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Net;
// Mengimpor namespace `System.Net.Http.Headers` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Net.Http.Headers;
// Mengimpor namespace `System.Net.Http.Json` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Net.Http.Json;
// Mengimpor namespace `System.Text.Json` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.Json;
// Mengimpor namespace `Cashflowpoly.Api.Tests.Infrastructure` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Tests.Infrastructure;
// Mengimpor namespace `Cashflowpoly.Api.Contracts` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Contracts;
// Mengimpor namespace `Xunit` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Xunit;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Tests` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Tests;

// menempatkan pengujian dalam koleksi fixture (”ApiIntegration”).
[Collection("ApiIntegration")]
// menerapkan metadata `Trait(”Category”, ”Integration”)` pada deklarasi berikut agar framework/compiler dapat mengenali pengaturannya.
[Trait("Category", "Integration")]
// Mendefinisikan tipe class `SessionStateApiIntegrationTests`; sealed mencegah tipe ini diturunkan lagi.
public sealed class SessionStateApiIntegrationTests
// Membuka scope tipe SessionStateApiIntegrationTests; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendeklarasikan field bertipe `HttpClient`: `_client` menyimpan nilai client. readonly membatasi penggantian referensi/nilai field pada deklarasi
    // atau konstruktor.
    private readonly HttpClient _client;

    // Mendefinisikan konstruktor SessionStateApiIntegrationTests yang menyiapkan objek dan menerima dependency/nilai awal dari pemanggil; parameter:
    // Parameter `fixture` bertipe `ApiIntegrationTestFixture` membawa nilai fixture.
    public SessionStateApiIntegrationTests(ApiIntegrationTestFixture fixture)
    // Membuka scope konstruktor SessionStateApiIntegrationTests; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // SessionStateApiIntegrationTests.
    {
        // Memperbarui `_client` menggunakan `fixture.Client` (nilai client) dalam SessionStateApiIntegrationTests.
        _client = fixture.Client;
    // Menutup scope konstruktor SessionStateApiIntegrationTests; bagian berikut berada di luar batas blok tersebut dalam
    // SessionStateApiIntegrationTests.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `RulesetSections_ReturnsUnityFriendlyCatalogWithoutExtraDataWrapper` dengan hasil bertipe `Task`; operasi ini menangani
    // aturan sections returns unity friendly catalog tanpa extra data wrapper. async memungkinkan metode menunggu operasi I/O dengan await dan
    // mengembalikan penyelesaian melalui Task.
    public async Task RulesetSections_ReturnsUnityFriendlyCatalogWithoutExtraDataWrapper()
    // Membuka scope metode RulesetSections_ReturnsUnityFriendlyCatalogWithoutExtraDataWrapper; pernyataan/deklarasi berikut berada di dalam batas blok
    // ini dalam RulesetSections_ReturnsUnityFriendlyCatalogWithoutExtraDataWrapper.
    {
        // Menyiapkan variabel lokal `token` untuk token yang diteruskan pada operasi terkait dengan hasil operasi asinkron memanggil
        // `RegisterInstructorAndGetTokenAsync` dengan tanpa argumen; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var token = await RegisterInstructorAndGetTokenAsync();

        // Menyiapkan variabel lokal `response` untuk hasil respons yang akan dibaca atau dikirim kepada pemanggil dengan hasil operasi asinkron memanggil
        // `SendJsonAsync` dengan `HttpMethod.Get`, `”/api/v1/rulesets/sections?mode=MAHIR”`, `null`, `token`; await menunggu hasil tanpa memblokir thread
        // selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope
        // berakhir.
        using var response = await SendJsonAsync(HttpMethod.Get, "/api/v1/rulesets/sections?mode=MAHIR", null, token);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`, `response.StatusCode`);
        // pengujian gagal jika keduanya berbeda dalam RulesetSections_ReturnsUnityFriendlyCatalogWithoutExtraDataWrapper.
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Menyiapkan variabel lokal `body` untuk nilai body dengan hasil operasi asinkron memanggil `ReadJsonAsync` dengan `response`; await menunggu hasil
        // tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas
        // otomatis saat scope berakhir.
        using var body = await ReadJsonAsync(response);
        // Menyiapkan variabel lokal `root` untuk nilai root dengan `body.RootElement` (nilai root element). Tipe variabel disimpulkan dari ekspresi nilai
        // awal.
        var root = body.RootElement;

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”MAHIR”`,
        // `root.GetProperty(”mode”).GetString()`); pengujian gagal jika keduanya berbeda dalam
        // RulesetSections_ReturnsUnityFriendlyCatalogWithoutExtraDataWrapper.
        Assert.Equal("MAHIR", root.GetProperty("mode").GetString());
        // Menjalankan pemeriksaan hasil dengan `Assert.NotEqual` menggunakan `Guid.Empty`, `root.GetProperty(”ruleset_id”).GetGuid()`; ketidaksesuaian
        // dengan ekspektasi membuat pengujian gagal dalam RulesetSections_ReturnsUnityFriendlyCatalogWithoutExtraDataWrapper.
        Assert.NotEqual(Guid.Empty, root.GetProperty("ruleset_id").GetGuid());
        // Menjalankan pemeriksaan hasil dengan `Assert.NotEqual` menggunakan `Guid.Empty`, `root.GetProperty(”ruleset_version_id”).GetGuid()`;
        // ketidaksesuaian dengan ekspektasi membuat pengujian gagal dalam RulesetSections_ReturnsUnityFriendlyCatalogWithoutExtraDataWrapper.
        Assert.NotEqual(Guid.Empty, root.GetProperty("ruleset_version_id").GetGuid());
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`10`,
        // `root.GetProperty(”gameConfig”).GetProperty(”initialCoins”).GetInt32()`); pengujian gagal jika keduanya berbeda dalam
        // RulesetSections_ReturnsUnityFriendlyCatalogWithoutExtraDataWrapper.
        Assert.Equal(10, root.GetProperty("gameConfig").GetProperty("initialCoins").GetInt32());
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`2`,
        // `root.GetProperty(”gameConfig”).GetProperty(”actionsPerTurn”).GetInt32()`); pengujian gagal jika keduanya berbeda dalam
        // RulesetSections_ReturnsUnityFriendlyCatalogWithoutExtraDataWrapper.
        Assert.Equal(2, root.GetProperty("gameConfig").GetProperty("actionsPerTurn").GetInt32());
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`25`,
        // `root.GetProperty(”gameConfig”).GetProperty(”finishDay”).GetInt32()`); pengujian gagal jika keduanya berbeda dalam
        // RulesetSections_ReturnsUnityFriendlyCatalogWithoutExtraDataWrapper.
        Assert.Equal(25, root.GetProperty("gameConfig").GetProperty("finishDay").GetInt32());

        // Menyiapkan variabel lokal `rulesetId` untuk identitas kumpulan aturan permainan dengan memanggil `root.GetProperty(”ruleset_id”).GetGuid` dengan
        // tanpa argumen. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var rulesetId = root.GetProperty("ruleset_id").GetGuid();
        // Menyiapkan variabel lokal `detailResponse` untuk nilai detail respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Get`, `$”/api/v1/rulesets/{rulesetId}”`, `null`, `token`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai.
        // Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var detailResponse = await SendJsonAsync(HttpMethod.Get, $"/api/v1/rulesets/{rulesetId}", null, token);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`,
        // `detailResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam RulesetSections_ReturnsUnityFriendlyCatalogWithoutExtraDataWrapper.
        Assert.Equal(HttpStatusCode.OK, detailResponse.StatusCode);
        // Menyiapkan variabel lokal `detailBody` untuk nilai detail body dengan hasil operasi asinkron memanggil `ReadJsonAsync` dengan `detailResponse`;
        // await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan
        // sumber daya dilepas otomatis saat scope berakhir.
        using var detailBody = await ReadJsonAsync(detailResponse);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”MAHIR”`,
        // `detailBody.RootElement.GetProperty(”mode”).GetString()`); pengujian gagal jika keduanya berbeda dalam
        // RulesetSections_ReturnsUnityFriendlyCatalogWithoutExtraDataWrapper.
        Assert.Equal("MAHIR", detailBody.RootElement.GetProperty("mode").GetString());
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui
        // Assert.Equal(`root.GetProperty(”ruleset_version_id”).GetGuid()`, `detailBody.RootElement.GetProperty(”ruleset_version_id”).GetGuid()`); pengujian
        // gagal jika keduanya berbeda dalam RulesetSections_ReturnsUnityFriendlyCatalogWithoutExtraDataWrapper.
        Assert.Equal(root.GetProperty("ruleset_version_id").GetGuid(), detailBody.RootElement.GetProperty("ruleset_version_id").GetGuid());
        // Menjalankan pemeriksaan bahwa `detailBody.RootElement.TryGetProperty(”config_json”, out _)` bernilai salah; pengujian gagal jika kondisi justru
        // terpenuhi dalam RulesetSections_ReturnsUnityFriendlyCatalogWithoutExtraDataWrapper.
        Assert.False(detailBody.RootElement.TryGetProperty("config_json", out _));
        // Menyiapkan variabel lokal `definition` untuk definisi terstruktur komponen serta parameter aturan permainan dengan memanggil
        // `detailBody.RootElement.GetProperty` dengan `”definition”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var definition = detailBody.RootElement.GetProperty("definition");
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`10`,
        // `definition.GetProperty(”settings”).GetProperty(”initial_coins”).GetInt32()`); pengujian gagal jika keduanya berbeda dalam
        // RulesetSections_ReturnsUnityFriendlyCatalogWithoutExtraDataWrapper.
        Assert.Equal(10, definition.GetProperty("settings").GetProperty("initial_coins").GetInt32());
        // Menjalankan pemeriksaan bahwa `definition.GetProperty(”ingredients”).GetArrayLength() > 0` bernilai benar; pengujian gagal jika kondisi tidak
        // terpenuhi dalam RulesetSections_ReturnsUnityFriendlyCatalogWithoutExtraDataWrapper.
        Assert.True(definition.GetProperty("ingredients").GetArrayLength() > 0);
        // Menjalankan pemeriksaan bahwa `definition.GetProperty(”orders”).GetArrayLength() > 0` bernilai benar; pengujian gagal jika kondisi tidak
        // terpenuhi dalam RulesetSections_ReturnsUnityFriendlyCatalogWithoutExtraDataWrapper.
        Assert.True(definition.GetProperty("orders").GetArrayLength() > 0);
        // Menjalankan pemeriksaan bahwa `definition.TryGetProperty(”quests”, out _)` bernilai salah; pengujian gagal jika kondisi justru terpenuhi dalam
        // RulesetSections_ReturnsUnityFriendlyCatalogWithoutExtraDataWrapper.
        Assert.False(definition.TryGetProperty("quests", out _));

        // Menyiapkan variabel lokal `componentsResponse` untuk nilai komponen respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Get`, `$”/api/v1/rulesets/{rulesetId}/components”`, `null`, `token`; await menunggu hasil tanpa memblokir thread selama operasi belum
        // selesai. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var componentsResponse = await SendJsonAsync(HttpMethod.Get, $"/api/v1/rulesets/{rulesetId}/components", null, token);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`,
        // `componentsResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam RulesetSections_ReturnsUnityFriendlyCatalogWithoutExtraDataWrapper.
        Assert.Equal(HttpStatusCode.OK, componentsResponse.StatusCode);
        // Menyiapkan variabel lokal `componentsBody` untuk nilai komponen body dengan hasil operasi asinkron memanggil `ReadJsonAsync` dengan
        // `componentsResponse`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai
        // awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var componentsBody = await ReadJsonAsync(componentsResponse);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”MAHIR”`,
        // `componentsBody.RootElement.GetProperty(”mode”).GetString()`); pengujian gagal jika keduanya berbeda dalam
        // RulesetSections_ReturnsUnityFriendlyCatalogWithoutExtraDataWrapper.
        Assert.Equal("MAHIR", componentsBody.RootElement.GetProperty("mode").GetString());
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui
        // Assert.Equal(`root.GetProperty(”ruleset_version_id”).GetGuid()`, `componentsBody.RootElement.GetProperty(”ruleset_version_id”).GetGuid()`);
        // pengujian gagal jika keduanya berbeda dalam RulesetSections_ReturnsUnityFriendlyCatalogWithoutExtraDataWrapper.
        Assert.Equal(root.GetProperty("ruleset_version_id").GetGuid(), componentsBody.RootElement.GetProperty("ruleset_version_id").GetGuid());
        // Menyiapkan variabel lokal `componentsDefinition` untuk nilai komponen definisi dengan memanggil `componentsBody.RootElement.GetProperty` dengan
        // `”definition”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var componentsDefinition = componentsBody.RootElement.GetProperty("definition");
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`10`,
        // `componentsDefinition.GetProperty(”settings”).GetProperty(”initial_coins”).GetInt32()`); pengujian gagal jika keduanya berbeda dalam
        // RulesetSections_ReturnsUnityFriendlyCatalogWithoutExtraDataWrapper.
        Assert.Equal(10, componentsDefinition.GetProperty("settings").GetProperty("initial_coins").GetInt32());
        // Menjalankan pemeriksaan bahwa `componentsDefinition.TryGetProperty(”quests”, out _)` bernilai salah; pengujian gagal jika kondisi justru
        // terpenuhi dalam RulesetSections_ReturnsUnityFriendlyCatalogWithoutExtraDataWrapper.
        Assert.False(componentsDefinition.TryGetProperty("quests", out _));

        // Menyiapkan variabel lokal `defaultsResponse` untuk nilai defaults respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Get`, `”/api/v1/rulesets/components/defaults?mode=MAHIR”`, `null`, `token`; await menunggu hasil tanpa memblokir thread selama
        // operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var defaultsResponse = await SendJsonAsync(HttpMethod.Get, "/api/v1/rulesets/components/defaults?mode=MAHIR", null, token);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`,
        // `defaultsResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam RulesetSections_ReturnsUnityFriendlyCatalogWithoutExtraDataWrapper.
        Assert.Equal(HttpStatusCode.OK, defaultsResponse.StatusCode);
        // Menyiapkan variabel lokal `defaultsBody` untuk nilai defaults body dengan hasil operasi asinkron memanggil `ReadJsonAsync` dengan
        // `defaultsResponse`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal;
        // using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var defaultsBody = await ReadJsonAsync(defaultsResponse);
        // Menyiapkan variabel lokal `defaultItems` untuk nilai bawaan elemen dengan mematerialisasi urutan
        // `defaultsBody.RootElement.GetProperty(”items”).EnumerateArray()` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var defaultItems = defaultsBody.RootElement.GetProperty("items").EnumerateArray().ToList();
        // Menyiapkan variabel lokal `selectedDefault` untuk nilai selected bawaan dengan pemeriksaan hasil dengan `Assert.Single` menggunakan
        // `defaultItems`, `item => item.GetProperty(”ruleset_id”).GetGuid() == rulesetId`; ketidaksesuaian dengan ekspektasi membuat pengujian gagal. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var selectedDefault = Assert.Single(
            // Meneruskan `defaultItems` (nilai bawaan elemen) sebagai argumen ke `Assert.Single`.
            defaultItems,
            // Parameter `item` bertipe `` membawa nilai elemen.
            item => item.GetProperty("ruleset_id").GetGuid() == rulesetId);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui
        // Assert.Equal(`root.GetProperty(”ruleset_version_id”).GetGuid()`, `selectedDefault.GetProperty(”ruleset_version_id”).GetGuid()`); pengujian gagal
        // jika keduanya berbeda dalam RulesetSections_ReturnsUnityFriendlyCatalogWithoutExtraDataWrapper.
        Assert.Equal(root.GetProperty("ruleset_version_id").GetGuid(), selectedDefault.GetProperty("ruleset_version_id").GetGuid());
        // Menyiapkan variabel lokal `defaultDefinition` untuk nilai bawaan definisi dengan memanggil `selectedDefault.GetProperty` dengan `”definition”`.
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var defaultDefinition = selectedDefault.GetProperty("definition");
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`10`,
        // `defaultDefinition.GetProperty(”settings”).GetProperty(”initial_coins”).GetInt32()`); pengujian gagal jika keduanya berbeda dalam
        // RulesetSections_ReturnsUnityFriendlyCatalogWithoutExtraDataWrapper.
        Assert.Equal(10, defaultDefinition.GetProperty("settings").GetProperty("initial_coins").GetInt32());
        // Menjalankan pemeriksaan bahwa `defaultDefinition.GetProperty(”collection_missions”).GetArrayLength() > 0` bernilai benar; pengujian gagal jika
        // kondisi tidak terpenuhi dalam RulesetSections_ReturnsUnityFriendlyCatalogWithoutExtraDataWrapper.
        Assert.True(defaultDefinition.GetProperty("collection_missions").GetArrayLength() > 0);

        // Menyiapkan variabel lokal `bahan` untuk nilai bahan dengan mematerialisasi urutan `root.GetProperty(”bahan”).EnumerateArray()` menjadi List;
        // enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var bahan = root.GetProperty("bahan").EnumerateArray().ToList();
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`5`, `bahan.Count`); pengujian gagal jika
        // keduanya berbeda dalam RulesetSections_ReturnsUnityFriendlyCatalogWithoutExtraDataWrapper.
        Assert.Equal(5, bahan.Count);
        // Menyiapkan variabel lokal `nasi` untuk nilai nasi dengan pemeriksaan hasil dengan `Assert.Single` menggunakan `bahan`, `item =>
        // item.GetProperty(”nama”).GetString() == ”Nasi Putih”`; ketidaksesuaian dengan ekspektasi membuat pengujian gagal. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var nasi = Assert.Single(bahan, item => item.GetProperty("nama").GetString() == "Nasi Putih");
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`1`,
        // `nasi.GetProperty(”hargaBeli”).GetInt32()`); pengujian gagal jika keduanya berbeda dalam
        // RulesetSections_ReturnsUnityFriendlyCatalogWithoutExtraDataWrapper.
        Assert.Equal(1, nasi.GetProperty("hargaBeli").GetInt32());

        // Menyiapkan variabel lokal `resep` untuk nilai resep dengan mematerialisasi urutan `root.GetProperty(”resep”).EnumerateArray()` menjadi List;
        // enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var resep = root.GetProperty("resep").EnumerateArray().ToList();
        // Menyiapkan variabel lokal `nasiGoreng` untuk nilai nasi goreng dengan pemeriksaan hasil dengan `Assert.Single` menggunakan `resep`, `item =>
        // item.GetProperty(”nama”).GetString() == ”nasi goreng”`; ketidaksesuaian dengan ekspektasi membuat pengujian gagal. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var nasiGoreng = Assert.Single(resep, item => item.GetProperty("nama").GetString() == "nasi goreng");
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`15`,
        // `nasiGoreng.GetProperty(”hargaJual”).GetInt32()`); pengujian gagal jika keduanya berbeda dalam
        // RulesetSections_ReturnsUnityFriendlyCatalogWithoutExtraDataWrapper.
        Assert.Equal(15, nasiGoreng.GetProperty("hargaJual").GetInt32());
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`0`,
        // `nasiGoreng.GetProperty(”poinKebahagiaan”).GetInt32()`); pengujian gagal jika keduanya berbeda dalam
        // RulesetSections_ReturnsUnityFriendlyCatalogWithoutExtraDataWrapper.
        Assert.Equal(0, nasiGoreng.GetProperty("poinKebahagiaan").GetInt32());
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`new[] { ”Nasi Putih”, ”Telur” }`,
        // `nasiGoreng.GetProperty(”bahan”).EnumerateArray().Select(item => item.GetString()).ToArray()`); pengujian gagal jika keduanya berbeda dalam
        // RulesetSections_ReturnsUnityFriendlyCatalogWithoutExtraDataWrapper.
        Assert.Equal(
            // Meneruskan array baru dengan tipe elemen disimpulkan dari nilai initializer sebagai argumen ke `Assert.Equal`.
            new[] { "Nasi Putih", "Telur" },
            // Meneruskan mematerialisasi urutan `nasiGoreng.GetProperty(”bahan”).EnumerateArray().Select(item => item.GetString())` menjadi array dengan elemen
            // hasil saat ini sebagai argumen ke `Assert.Equal`; Meneruskan nilai literal `”bahan”` sebagai argumen ke `nasiGoreng.GetProperty`; Meneruskan
            // fungsi lambda `item => item.GetString()` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen ke
            // `nasiGoreng.GetProperty(”bahan”).EnumerateArray().Select`.
            nasiGoreng.GetProperty("bahan").EnumerateArray().Select(item => item.GetString()).ToArray());

        // Menyiapkan variabel lokal `target` untuk nilai target dengan pemeriksaan hasil dengan `Assert.Single` menggunakan
        // `root.GetProperty(”targetKebutuhan”).EnumerateArray()`, `item => item.GetProperty(”id”).GetString() == ”misi_boneka”`; ketidaksesuaian dengan
        // ekspektasi membuat pengujian gagal. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var target = Assert.Single(
            // Meneruskan menelusuri elemen array JSON `root.GetProperty(”targetKebutuhan”)` sebagai argumen ke `Assert.Single`; Meneruskan nilai literal
            // `”targetKebutuhan”` sebagai argumen ke `root.GetProperty`.
            root.GetProperty("targetKebutuhan").EnumerateArray(),
            // Parameter `item` bertipe `` membawa nilai elemen.
            item => item.GetProperty("id").GetString() == "misi_boneka");
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”boneka”`,
        // `target.GetProperty(”nama”).GetString()`); pengujian gagal jika keduanya berbeda dalam
        // RulesetSections_ReturnsUnityFriendlyCatalogWithoutExtraDataWrapper.
        Assert.Equal("boneka", target.GetProperty("nama").GetString());
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`3`,
        // `target.GetProperty(”kebutuhanTarget”).GetArrayLength()`); pengujian gagal jika keduanya berbeda dalam
        // RulesetSections_ReturnsUnityFriendlyCatalogWithoutExtraDataWrapper.
        Assert.Equal(3, target.GetProperty("kebutuhanTarget").GetArrayLength());
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`10`,
        // `Math.Abs(target.GetProperty(”penaltyPoints”).GetInt32())`); pengujian gagal jika keduanya berbeda dalam
        // RulesetSections_ReturnsUnityFriendlyCatalogWithoutExtraDataWrapper.
        Assert.Equal(10, Math.Abs(target.GetProperty("penaltyPoints").GetInt32()));

        // Menyiapkan variabel lokal `narasi` untuk nilai narasi dengan pemeriksaan hasil dengan `Assert.Single` menggunakan
        // `root.GetProperty(”narasi”).EnumerateArray()`, `item => item.GetProperty(”nama”).GetString() == ”jual_pertama”`; ketidaksesuaian dengan
        // ekspektasi membuat pengujian gagal. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var narasi = Assert.Single(
            // Meneruskan menelusuri elemen array JSON `root.GetProperty(”narasi”)` sebagai argumen ke `Assert.Single`; Meneruskan nilai literal `”narasi”`
            // sebagai argumen ke `root.GetProperty`.
            root.GetProperty("narasi").EnumerateArray(),
            // Parameter `item` bertipe `` membawa nilai elemen.
            item => item.GetProperty("nama").GetString() == "jual_pertama");
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”JualMasakan”`,
        // `narasi.GetProperty(”prerequisiteAksi”)[0].GetProperty(”aksi”).GetString()`); pengujian gagal jika keduanya berbeda dalam
        // RulesetSections_ReturnsUnityFriendlyCatalogWithoutExtraDataWrapper.
        Assert.Equal("JualMasakan", narasi.GetProperty("prerequisiteAksi")[0].GetProperty("aksi").GetString());
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`1`,
        // `narasi.GetProperty(”prerequisiteAksi”)[0].GetProperty(”value”).GetInt32()`); pengujian gagal jika keduanya berbeda dalam
        // RulesetSections_ReturnsUnityFriendlyCatalogWithoutExtraDataWrapper.
        Assert.Equal(1, narasi.GetProperty("prerequisiteAksi")[0].GetProperty("value").GetInt32());

        // Menjalankan pemeriksaan bahwa `root.TryGetProperty(”quest”, out _)` bernilai salah; pengujian gagal jika kondisi justru terpenuhi dalam
        // RulesetSections_ReturnsUnityFriendlyCatalogWithoutExtraDataWrapper.
        Assert.False(root.TryGetProperty("quest", out _));
    // Menutup scope metode RulesetSections_ReturnsUnityFriendlyCatalogWithoutExtraDataWrapper; bagian berikut berada di luar batas blok tersebut dalam
    // RulesetSections_ReturnsUnityFriendlyCatalogWithoutExtraDataWrapper.
    }

    // menandai metode sebagai pengujian xUnit yang dijalankan untuk setiap kombinasi data.
    [Theory]
    // menyediakan satu kombinasi masukan pengujian (2).
    [InlineData(2)]
    // menyediakan satu kombinasi masukan pengujian (3).
    [InlineData(3)]
    // menyediakan satu kombinasi masukan pengujian (4).
    [InlineData(4)]
    // Mendefinisikan metode `CreateSession_InitializesStartedStateForSupportedPlayerCounts` dengan hasil bertipe `Task`; operasi ini menangani create
    // sesi initializes started keadaan untuk supported pemain counts. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan
    // penyelesaian melalui Task. Masukan: Parameter `playerCount` bertipe `int` membawa nilai pemain jumlah.
    public async Task CreateSession_InitializesStartedStateForSupportedPlayerCounts(int playerCount)
    // Membuka scope metode CreateSession_InitializesStartedStateForSupportedPlayerCounts; pernyataan/deklarasi berikut berada di dalam batas blok ini
    // dalam CreateSession_InitializesStartedStateForSupportedPlayerCounts.
    {
        // Menyiapkan variabel lokal `token` untuk token yang diteruskan pada operasi terkait dengan hasil operasi asinkron memanggil
        // `RegisterInstructorAndGetTokenAsync` dengan tanpa argumen; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var token = await RegisterInstructorAndGetTokenAsync();
        // Menyiapkan variabel lokal `names` untuk nilai nama dengan mematerialisasi urutan `Enumerable.Range(1, playerCount).Select(index => $”P{index}”)`
        // menjadi array dengan elemen hasil saat ini. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var names = Enumerable.Range(1, playerCount).Select(index => $"P{index}").ToArray();
        // Menyiapkan variabel lokal `started` untuk nilai started dengan hasil operasi asinkron memanggil `CreateStartedSessionAsync` dengan `token`,
        // `names`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var started = await CreateStartedSessionAsync(token, names);
        // Menjalankan memanggil `AssertInitialState` dengan `started.State`, `playerCount`, `names` dalam
        // CreateSession_InitializesStartedStateForSupportedPlayerCounts.
        AssertInitialState(started.State, playerCount, names);

        // Menyiapkan variabel lokal `eventsResponse` untuk nilai event respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Get`, `$”/api/v1/sessions/{started.SessionId}/events?limit=100”`, `null`, `token`; await menunggu hasil tanpa memblokir thread selama
        // operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var eventsResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Get` (nilai get) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Get,
            // Meneruskan teks interpolasi `$”/api/v1/sessions/{started.SessionId}/events?limit=100”`; nilai ekspresi di dalam kurung kurawal disisipkan saat
            // program berjalan sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/sessions/{started.SessionId}/events?limit=100",
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen ke `SendJsonAsync`.
            null,
            // Meneruskan `token` (token yang diteruskan pada operasi terkait) sebagai argumen ke `SendJsonAsync`.
            token);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`,
        // `eventsResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam CreateSession_InitializesStartedStateForSupportedPlayerCounts.
        Assert.Equal(HttpStatusCode.OK, eventsResponse.StatusCode);
        // Menyiapkan variabel lokal `eventsBody` untuk nilai event body dengan hasil operasi asinkron memanggil `ReadJsonAsync` dengan `eventsResponse`;
        // await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan
        // sumber daya dilepas otomatis saat scope berakhir.
        using var eventsBody = await ReadJsonAsync(eventsResponse);
        // Menyiapkan variabel lokal `setupEvents` untuk nilai setup event dengan mematerialisasi urutan
        // `eventsBody.RootElement.GetProperty(”items”).EnumerateArray()` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var setupEvents = eventsBody.RootElement.GetProperty("items").EnumerateArray().ToList();
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`1 + (playerCount * 6)`, `setupEvents.Count`);
        // pengujian gagal jika keduanya berbeda dalam CreateSession_InitializesStartedStateForSupportedPlayerCounts.
        Assert.Equal(1 + (playerCount * 6), setupEvents.Count);
        // Menjalankan pemeriksaan hasil dengan `Assert.Single` menggunakan `setupEvents`, `item => item.GetProperty(”action_type”).GetString() ==
        // ”MulaiSesi”`; ketidaksesuaian dengan ekspektasi membuat pengujian gagal dalam CreateSession_InitializesStartedStateForSupportedPlayerCounts.
        Assert.Single(setupEvents, item => item.GetProperty("action_type").GetString() == "MulaiSesi");
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `setupEvents`, `item =>
        // item.GetProperty(”action_type”).GetString() is ”AmbilKartuDariDeck” or ”IsiUlangPasar” or ”KartuMasukDiscard”` dalam
        // CreateSession_InitializesStartedStateForSupportedPlayerCounts.
        Assert.DoesNotContain(setupEvents, item =>
            // Meneruskan nilai literal `”action_type”` sebagai argumen ke `item.GetProperty`.
            item.GetProperty("action_type").GetString() is "AmbilKartuDariDeck" or "IsiUlangPasar" or "KartuMasukDiscard");

        // Menyiapkan variabel lokal `tieNumbers` untuk nilai tie numbers dengan mematerialisasi urutan `setupEvents .Where(item =>
        // item.GetProperty(”action_type”).GetString() == ”BagikanTieBreaker”) .Select(item => item.GetProperty(”payload”).GetProperty(”number”).GetInt32())
        // .O...` menjadi array dengan elemen hasil saat ini. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var tieNumbers = setupEvents
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(item => item.GetProperty(”action_type”).GetString() ==
            // ”BagikanTieBreaker”) dalam CreateSession_InitializesStartedStateForSupportedPlayerCounts; token pada baris ini menyambungkan bagian kode sebelum
            // dan sesudahnya.
            .Where(item => item.GetProperty("action_type").GetString() == "BagikanTieBreaker")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Select(item => item.GetProperty(”payload”).GetProperty(”number”).GetInt32())
            // dalam CreateSession_InitializesStartedStateForSupportedPlayerCounts; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Select(item => item.GetProperty("payload").GetProperty("number").GetInt32())
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .OrderBy(number => number) dalam
            // CreateSession_InitializesStartedStateForSupportedPlayerCounts; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .OrderBy(number => number)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToArray(); dalam
            // CreateSession_InitializesStartedStateForSupportedPlayerCounts; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .ToArray();
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`Enumerable.Range(1, playerCount)`,
        // `tieNumbers`); pengujian gagal jika keduanya berbeda dalam CreateSession_InitializesStartedStateForSupportedPlayerCounts.
        Assert.Equal(Enumerable.Range(1, playerCount), tieNumbers);
        // Menyiapkan variabel lokal `turnOrderByUserId` untuk nilai giliran urutan/pesanan berdasarkan pengguna identitas dengan membangun kamus dari
        // `started.State.GetProperty(”players”) .EnumerateArray()` dengan pemilihan kunci/nilai `item => item.GetProperty(”user_id”).GetGuid()`, `item =>
        // item.GetProperty(”player_order_no”).GetInt32()`; kunci harus unik agar konversi berhasil. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var turnOrderByUserId = started.State.GetProperty("players")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .EnumerateArray() dalam
            // CreateSession_InitializesStartedStateForSupportedPlayerCounts; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .EnumerateArray()
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToDictionary( dalam
            // CreateSession_InitializesStartedStateForSupportedPlayerCounts; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .ToDictionary(
                // Parameter `item` bertipe `` membawa nilai elemen.
                item => item.GetProperty("user_id").GetGuid(),
                // Parameter `item` bertipe `` membawa nilai elemen.
                item => item.GetProperty("player_order_no").GetInt32());
        // Menjalankan pemeriksaan hasil dengan `Assert.All` menggunakan `setupEvents.Where(item => item.GetProperty(”action_type”).GetString() ==
        // ”BagikanTieBreaker”)`, `item => Assert.Equal( item.GetProperty(”payload”).GetProperty(”number”).GetInt32(),
        // turnOrderByUserId[item.GetProperty(”user_id”).GetGuid()])`; ketidaksesuaian dengan ekspektasi membuat pengujian gagal dalam
        // CreateSession_InitializesStartedStateForSupportedPlayerCounts.
        Assert.All(
            // Meneruskan menyaring elemen `setupEvents` dengan predikat `item => item.GetProperty(”action_type”).GetString() == ”BagikanTieBreaker”`; hanya
            // elemen yang memenuhi kondisi diteruskan sebagai argumen ke `Assert.All`; Meneruskan fungsi lambda `item =>
            // item.GetProperty(”action_type”).GetString() == ”BagikanTieBreaker”` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai
            // argumen ke `setupEvents.Where`; Meneruskan nilai literal `”action_type”` sebagai argumen ke `item.GetProperty`.
            setupEvents.Where(item => item.GetProperty("action_type").GetString() == "BagikanTieBreaker"),
            // Parameter `item` bertipe `` membawa nilai elemen.
            item => Assert.Equal(
                // Meneruskan memanggil `item.GetProperty(”payload”).GetProperty(”number”).GetInt32` dengan tanpa argumen sebagai argumen ke `Assert.Equal`;
                // Meneruskan nilai literal `”payload”` sebagai argumen ke `item.GetProperty`; Meneruskan nilai literal `”number”` sebagai argumen ke
                // `item.GetProperty(”payload”).GetProperty`.
                item.GetProperty("payload").GetProperty("number").GetInt32(),
                // Meneruskan `turnOrderByUserId[item.GetProperty(”user_id”).GetGuid()]`, yaitu elemen koleksi yang dipilih melalui indeks atau kunci tersebut
                // sebagai argumen ke `Assert.Equal`; Meneruskan memanggil `item.GetProperty(”user_id”).GetGuid` dengan tanpa argumen sebagai argumen ke
                // `Assert.Equal`; Meneruskan nilai literal `”user_id”` sebagai argumen ke `item.GetProperty`.
                turnOrderByUserId[item.GetProperty("user_id").GetGuid()]));
        // Menyiapkan variabel lokal `missionIds` untuk nilai misi identitas dengan mematerialisasi urutan `setupEvents .Where(item =>
        // item.GetProperty(”action_type”).GetString() == ”SetupMisiAwal”) .Select(item =>
        // item.GetProperty(”payload”).GetProperty(”mission_id”).GetString())` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var missionIds = setupEvents
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(item => item.GetProperty(”action_type”).GetString() == ”SetupMisiAwal”)
            // dalam CreateSession_InitializesStartedStateForSupportedPlayerCounts; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Where(item => item.GetProperty("action_type").GetString() == "SetupMisiAwal")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Select(item =>
            // item.GetProperty(”payload”).GetProperty(”mission_id”).GetString()) dalam CreateSession_InitializesStartedStateForSupportedPlayerCounts; token
            // pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Select(item => item.GetProperty("payload").GetProperty("mission_id").GetString())
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList(); dalam CreateSession_InitializesStartedStateForSupportedPlayerCounts;
            // token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .ToList();
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`playerCount`,
        // `missionIds.Distinct(StringComparer.OrdinalIgnoreCase).Count()`); pengujian gagal jika keduanya berbeda dalam
        // CreateSession_InitializesStartedStateForSupportedPlayerCounts.
        Assert.Equal(playerCount, missionIds.Distinct(StringComparer.OrdinalIgnoreCase).Count());

        // Menyiapkan variabel lokal `getResponse` untuk nilai get respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan `HttpMethod.Get`,
        // `$”/api/v1/sessions/{started.SessionId}/state”`, `null`, `token`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var getResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Get` (nilai get) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Get,
            // Meneruskan teks interpolasi `$”/api/v1/sessions/{started.SessionId}/state”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program
            // berjalan sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/sessions/{started.SessionId}/state",
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen ke `SendJsonAsync`.
            null,
            // Meneruskan `token` (token yang diteruskan pada operasi terkait) sebagai argumen ke `SendJsonAsync`.
            token);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`, `getResponse.StatusCode`);
        // pengujian gagal jika keduanya berbeda dalam CreateSession_InitializesStartedStateForSupportedPlayerCounts.
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        // Menyiapkan variabel lokal `getBody` untuk nilai get body dengan hasil operasi asinkron memanggil `ReadJsonAsync` dengan `getResponse`; await
        // menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber
        // daya dilepas otomatis saat scope berakhir.
        using var getBody = await ReadJsonAsync(getResponse);
        // Menjalankan memanggil `AssertInitialState` dengan `getBody.RootElement`, `playerCount`, `names` dalam
        // CreateSession_InitializesStartedStateForSupportedPlayerCounts.
        AssertInitialState(getBody.RootElement, playerCount, names);
    // Menutup scope metode CreateSession_InitializesStartedStateForSupportedPlayerCounts; bagian berikut berada di luar batas blok tersebut dalam
    // CreateSession_InitializesStartedStateForSupportedPlayerCounts.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `PutState_ReturnsGone_AndDoesNotMutateState` dengan hasil bertipe `Task`; operasi ini menangani put keadaan returns gone
    // dan does not mutate keadaan. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task.
    public async Task PutState_ReturnsGone_AndDoesNotMutateState()
    // Membuka scope metode PutState_ReturnsGone_AndDoesNotMutateState; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // PutState_ReturnsGone_AndDoesNotMutateState.
    {
        // Menyiapkan variabel lokal `token` untuk token yang diteruskan pada operasi terkait dengan hasil operasi asinkron memanggil
        // `RegisterInstructorAndGetTokenAsync` dengan tanpa argumen; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var token = await RegisterInstructorAndGetTokenAsync();
        // Menyiapkan variabel lokal `started` untuk nilai started dengan hasil operasi asinkron memanggil `CreateStartedSessionAsync` dengan `token`,
        // `[”Doni”, ”Rani”, ”Bimo”]`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi
        // nilai awal.
        var started = await CreateStartedSessionAsync(token, ["Doni", "Rani", "Bimo"]);
        // Menyiapkan variabel lokal `sessionId` untuk identitas unik sesi permainan yang menjadi batas data operasi ini dengan `started.SessionId`
        // (identitas unik sesi permainan yang menjadi batas data operasi ini). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var sessionId = started.SessionId;
        // Menyiapkan variabel lokal `players` untuk nilai pemain dengan mematerialisasi urutan `started.State.GetProperty(”players”).EnumerateArray()`
        // menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var players = started.State.GetProperty("players").EnumerateArray().ToList();
        // Menyiapkan variabel lokal `firstPlayerId` untuk nilai first pemain identitas dengan memanggil
        // `players[0].GetProperty(”session_player_id”).GetGuid` dengan tanpa argumen. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var firstPlayerId = players[0].GetProperty("session_player_id").GetGuid();
        // Menyiapkan variabel lokal `secondPlayerId` untuk nilai second pemain identitas dengan memanggil
        // `players[1].GetProperty(”session_player_id”).GetGuid` dengan tanpa argumen. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var secondPlayerId = players[1].GetProperty("session_player_id").GetGuid();
        // Menyiapkan variabel lokal `thirdPlayerId` untuk nilai third pemain identitas dengan memanggil
        // `players[2].GetProperty(”session_player_id”).GetGuid` dengan tanpa argumen. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var thirdPlayerId = players[2].GetProperty("session_player_id").GetGuid();

        // Menyiapkan variabel lokal `updatedState` untuk nilai updated keadaan dengan memanggil `BuildStatePayload` dengan `1`, `firstPlayerId`,
        // `secondPlayerId`, `thirdPlayerId`, `”Nasi Putih”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var updatedState = BuildStatePayload(
            // Meneruskan nilai literal `1` sebagai argumen bernama `stateVersion`.
            stateVersion: 1,
            // Meneruskan `firstPlayerId` (nilai first pemain identitas) sebagai argumen ke `BuildStatePayload`.
            firstPlayerId,
            // Meneruskan `secondPlayerId` (nilai second pemain identitas) sebagai argumen ke `BuildStatePayload`.
            secondPlayerId,
            // Meneruskan `thirdPlayerId` (nilai third pemain identitas) sebagai argumen ke `BuildStatePayload`.
            thirdPlayerId,
            // Meneruskan nilai literal `”Nasi Putih”` sebagai argumen bernama `firstBahanNama`.
            firstBahanNama: "Nasi Putih");
        // Menyiapkan variabel lokal `updateResponse` untuk nilai update respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Put`, `$”/api/v1/sessions/{sessionId}/state”`, `updatedState`, `token`; await menunggu hasil tanpa memblokir thread selama operasi
        // belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var updateResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Put` (nilai put) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Put,
            // Meneruskan teks interpolasi `$”/api/v1/sessions/{sessionId}/state”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan
            // sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/sessions/{sessionId}/state",
            // Meneruskan `updatedState` (nilai updated keadaan) sebagai argumen ke `SendJsonAsync`.
            updatedState,
            // Meneruskan `token` (token yang diteruskan pada operasi terkait) sebagai argumen ke `SendJsonAsync`.
            token);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.Gone`,
        // `updateResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam PutState_ReturnsGone_AndDoesNotMutateState.
        Assert.Equal(HttpStatusCode.Gone, updateResponse.StatusCode);

        // Menyiapkan variabel lokal `errorBody` untuk nilai kesalahan body dengan hasil operasi asinkron memanggil `ReadJsonAsync` dengan `updateResponse`;
        // await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan
        // sumber daya dilepas otomatis saat scope berakhir.
        using var errorBody = await ReadJsonAsync(updateResponse);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”STATE_WRITE_DISABLED”`,
        // `errorBody.RootElement.GetProperty(”error_code”).GetString()`); pengujian gagal jika keduanya berbeda dalam
        // PutState_ReturnsGone_AndDoesNotMutateState.
        Assert.Equal("STATE_WRITE_DISABLED", errorBody.RootElement.GetProperty("error_code").GetString());

        // Menyiapkan variabel lokal `getResponse` untuk nilai get respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan `HttpMethod.Get`,
        // `$”/api/v1/sessions/{sessionId}/state”`, `null`, `token`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel
        // disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var getResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Get` (nilai get) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Get,
            // Meneruskan teks interpolasi `$”/api/v1/sessions/{sessionId}/state”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan
            // sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/sessions/{sessionId}/state",
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen ke `SendJsonAsync`.
            null,
            // Meneruskan `token` (token yang diteruskan pada operasi terkait) sebagai argumen ke `SendJsonAsync`.
            token);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`, `getResponse.StatusCode`);
        // pengujian gagal jika keduanya berbeda dalam PutState_ReturnsGone_AndDoesNotMutateState.
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        // Menyiapkan variabel lokal `getBody` untuk nilai get body dengan hasil operasi asinkron memanggil `ReadJsonAsync` dengan `getResponse`; await
        // menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber
        // daya dilepas otomatis saat scope berakhir.
        using var getBody = await ReadJsonAsync(getResponse);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`1`,
        // `getBody.RootElement.GetProperty(”state_version”).GetInt64()`); pengujian gagal jika keduanya berbeda dalam
        // PutState_ReturnsGone_AndDoesNotMutateState.
        Assert.Equal(1, getBody.RootElement.GetProperty("state_version").GetInt64());
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`players.Select(item =>
        // item.GetProperty(”name”).GetString())`, `getBody.RootElement.GetProperty(”players”).EnumerateArray().Select(item =>
        // item.GetProperty(”name”).GetString())`); pengujian gagal jika keduanya berbeda dalam PutState_ReturnsGone_AndDoesNotMutateState.
        Assert.Equal(
            // Meneruskan memetakan setiap elemen `players` melalui `item => item.GetProperty(”name”).GetString()` menjadi bentuk hasil yang dibutuhkan sebagai
            // argumen ke `Assert.Equal`; Meneruskan fungsi lambda `item => item.GetProperty(”name”).GetString()` yang dijalankan oleh operasi pemanggil untuk
            // memproses setiap masukan sebagai argumen ke `players.Select`; Meneruskan nilai literal `”name”` sebagai argumen ke `item.GetProperty`.
            players.Select(item => item.GetProperty("name").GetString()),
            // Meneruskan memetakan setiap elemen `getBody.RootElement.GetProperty(”players”).EnumerateArray()` melalui `item =>
            // item.GetProperty(”name”).GetString()` menjadi bentuk hasil yang dibutuhkan sebagai argumen ke `Assert.Equal`; Meneruskan nilai literal
            // `”players”` sebagai argumen ke `getBody.RootElement.GetProperty`; Meneruskan fungsi lambda `item => item.GetProperty(”name”).GetString()` yang
            // dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen ke
            // `getBody.RootElement.GetProperty(”players”).EnumerateArray().Select`; Meneruskan nilai literal `”name”` sebagai argumen ke `item.GetProperty`.
            getBody.RootElement.GetProperty("players").EnumerateArray().Select(item => item.GetProperty("name").GetString()));
    // Menutup scope metode PutState_ReturnsGone_AndDoesNotMutateState; bagian berikut berada di luar batas blok tersebut dalam
    // PutState_ReturnsGone_AndDoesNotMutateState.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `SetupEndpoints_RequireSetupAllowRetryAndLockLatestRevisionOnStart` dengan hasil bertipe `Task`; operasi ini menangani
    // setup endpoints require setup allow retry dan lock latest revision on start. async memungkinkan metode menunggu operasi I/O dengan await dan
    // mengembalikan penyelesaian melalui Task.
    public async Task SetupEndpoints_RequireSetupAllowRetryAndLockLatestRevisionOnStart()
    // Membuka scope metode SetupEndpoints_RequireSetupAllowRetryAndLockLatestRevisionOnStart; pernyataan/deklarasi berikut berada di dalam batas blok
    // ini dalam SetupEndpoints_RequireSetupAllowRetryAndLockLatestRevisionOnStart.
    {
        // Menyiapkan variabel lokal `token` untuk token yang diteruskan pada operasi terkait dengan hasil operasi asinkron memanggil
        // `RegisterInstructorAndGetTokenAsync` dengan tanpa argumen; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var token = await RegisterInstructorAndGetTokenAsync();
        // Menyiapkan variabel lokal `prepared` untuk nilai prepared dengan hasil operasi asinkron memanggil `CreateSessionWithPlayersAsync` dengan `token`,
        // `[”A”, ”B”]`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var prepared = await CreateSessionWithPlayersAsync(token, ["A", "B"]);

        // Menyiapkan variabel lokal `startWithoutSetupResponse` untuk nilai start tanpa setup respons dengan hasil operasi asinkron memanggil
        // `SendJsonAsync` dengan `HttpMethod.Post`, `$”/api/v1/sessions/{prepared.SessionId}/start”`, `null`, `token`; await menunggu hasil tanpa memblokir
        // thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope
        // berakhir.
        using var startWithoutSetupResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Post` (nilai post) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Post,
            // Meneruskan teks interpolasi `$”/api/v1/sessions/{prepared.SessionId}/start”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program
            // berjalan sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/sessions/{prepared.SessionId}/start",
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen ke `SendJsonAsync`.
            null,
            // Meneruskan `token` (token yang diteruskan pada operasi terkait) sebagai argumen ke `SendJsonAsync`.
            token);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.UnprocessableEntity`,
        // `startWithoutSetupResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam
        // SetupEndpoints_RequireSetupAllowRetryAndLockLatestRevisionOnStart.
        Assert.Equal(HttpStatusCode.UnprocessableEntity, startWithoutSetupResponse.StatusCode);
        // Menyiapkan variabel lokal `startError` untuk nilai start kesalahan dengan hasil operasi asinkron membaca tanpa argumen menjadi objek bertipe
        // sesuai kontrak JSON melalui `startWithoutSetupResponse.Content.ReadFromJsonAsync<ErrorResponse>`; await menunggu hasil tanpa memblokir thread
        // selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var startError = await startWithoutSetupResponse.Content.ReadFromJsonAsync<ErrorResponse>();
        // Menjalankan pemeriksaan NotNull atas `startError` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // SetupEndpoints_RequireSetupAllowRetryAndLockLatestRevisionOnStart.
        Assert.NotNull(startError);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”SETUP_REQUIRED”`, `startError.ErrorCode`);
        // pengujian gagal jika keduanya berbeda dalam SetupEndpoints_RequireSetupAllowRetryAndLockLatestRevisionOnStart.
        Assert.Equal("SETUP_REQUIRED", startError.ErrorCode);

        // Menyiapkan variabel lokal `firstSaveResponse` untuk nilai first save respons dengan hasil operasi asinkron memanggil
        // `SessionSetupTestHelper.SaveAsync` dengan `_client`, `token`, `prepared.SessionId`, `prepared.Definition`,
        // `TestContext.Current.CancellationToken`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari
        // ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var firstSaveResponse = await SessionSetupTestHelper.SaveAsync(
            // Meneruskan `_client` (nilai client) sebagai argumen ke `SessionSetupTestHelper.SaveAsync`.
            _client,
            // Meneruskan `token` (token yang diteruskan pada operasi terkait) sebagai argumen ke `SessionSetupTestHelper.SaveAsync`.
            token,
            // Meneruskan `prepared.SessionId` (identitas unik sesi permainan yang menjadi batas data operasi ini) sebagai argumen ke
            // `SessionSetupTestHelper.SaveAsync`.
            prepared.SessionId,
            // Meneruskan `prepared.Definition` (definisi terstruktur komponen serta parameter aturan permainan) sebagai argumen ke
            // `SessionSetupTestHelper.SaveAsync`.
            prepared.Definition,
            // Meneruskan `TestContext.Current.CancellationToken` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
            // aplikasi berhenti) sebagai argumen ke `SessionSetupTestHelper.SaveAsync`.
            TestContext.Current.CancellationToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.Created`,
        // `firstSaveResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam SetupEndpoints_RequireSetupAllowRetryAndLockLatestRevisionOnStart.
        Assert.Equal(HttpStatusCode.Created, firstSaveResponse.StatusCode);

        // Menyiapkan variabel lokal `setupBody` untuk nilai setup body dengan hasil operasi asinkron memanggil `ReadJsonAsync` dengan `firstSaveResponse`;
        // await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan
        // sumber daya dilepas otomatis saat scope berakhir.
        using var setupBody = await ReadJsonAsync(firstSaveResponse);
        // Menyiapkan variabel lokal `validateRequest` untuk nilai validate permintaan dengan objek baru bertipe `SessionSetupRequest` dengan argumen (
        // setupBody.RootElement.GetProperty(”client_request_id”).GetString()!,
        // setupBody.RootElement.GetProperty(”players”).Deserialize<List<SessionPlayerSetupRequest>>.... Tipe variabel disimpulkan dari ekspresi nilai awal.
        var validateRequest = new SessionSetupRequest(
            // Meneruskan `setupBody.RootElement.GetProperty(”client_request_id”).GetString()` dengan penegasan non-null untuk analisis compiler; operator !
            // tidak menambah pemeriksaan saat runtime sebagai argumen ke konstruktor `SessionSetupRequest`; Meneruskan nilai literal `”client_request_id”`
            // sebagai argumen ke `setupBody.RootElement.GetProperty`.
            setupBody.RootElement.GetProperty("client_request_id").GetString()!,
            // Meneruskan `setupBody.RootElement.GetProperty(”players”).Deserialize<List<SessionPlayerSetupRequest>>()` dengan penegasan non-null untuk analisis
            // compiler; operator ! tidak menambah pemeriksaan saat runtime sebagai argumen ke konstruktor `SessionSetupRequest`; Meneruskan nilai literal
            // `”players”` sebagai argumen ke `setupBody.RootElement.GetProperty`.
            setupBody.RootElement.GetProperty("players").Deserialize<List<SessionPlayerSetupRequest>>()!);
        // Menyiapkan variabel lokal `validateResponse` untuk nilai validate respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Post`, `$”/api/v1/sessions/{prepared.SessionId}/setup/validate”`, `validateRequest`, `token`; await menunggu hasil tanpa memblokir
        // thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope
        // berakhir.
        using var validateResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Post` (nilai post) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Post,
            // Meneruskan teks interpolasi `$”/api/v1/sessions/{prepared.SessionId}/setup/validate”`; nilai ekspresi di dalam kurung kurawal disisipkan saat
            // program berjalan sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/sessions/{prepared.SessionId}/setup/validate",
            // Meneruskan `validateRequest` (nilai validate permintaan) sebagai argumen ke `SendJsonAsync`.
            validateRequest,
            // Meneruskan `token` (token yang diteruskan pada operasi terkait) sebagai argumen ke `SendJsonAsync`.
            token);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`,
        // `validateResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam SetupEndpoints_RequireSetupAllowRetryAndLockLatestRevisionOnStart.
        Assert.Equal(HttpStatusCode.OK, validateResponse.StatusCode);

        // Menyiapkan variabel lokal `retryResponse` untuk nilai retry respons dengan hasil operasi asinkron memanggil `SessionSetupTestHelper.SaveAsync`
        // dengan `_client`, `token`, `prepared.SessionId`, `prepared.Definition`, `TestContext.Current.CancellationToken`; await menunggu hasil tanpa
        // memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis
        // saat scope berakhir.
        using var retryResponse = await SessionSetupTestHelper.SaveAsync(
            // Meneruskan `_client` (nilai client) sebagai argumen ke `SessionSetupTestHelper.SaveAsync`.
            _client,
            // Meneruskan `token` (token yang diteruskan pada operasi terkait) sebagai argumen ke `SessionSetupTestHelper.SaveAsync`.
            token,
            // Meneruskan `prepared.SessionId` (identitas unik sesi permainan yang menjadi batas data operasi ini) sebagai argumen ke
            // `SessionSetupTestHelper.SaveAsync`.
            prepared.SessionId,
            // Meneruskan `prepared.Definition` (definisi terstruktur komponen serta parameter aturan permainan) sebagai argumen ke
            // `SessionSetupTestHelper.SaveAsync`.
            prepared.Definition,
            // Meneruskan `TestContext.Current.CancellationToken` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
            // aplikasi berhenti) sebagai argumen ke `SessionSetupTestHelper.SaveAsync`.
            TestContext.Current.CancellationToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`,
        // `retryResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam SetupEndpoints_RequireSetupAllowRetryAndLockLatestRevisionOnStart.
        Assert.Equal(HttpStatusCode.OK, retryResponse.StatusCode);

        // Menyiapkan variabel lokal `revisedPlayers` untuk nilai revised pemain dengan mematerialisasi urutan `validateRequest.Players` menjadi List;
        // enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var revisedPlayers = validateRequest.Players.ToList();
        // Memperbarui `(revisedPlayers[0], revisedPlayers[1])` menggunakan tuple yang membawa bagian 1: revisedPlayers[0] with { TieBreakerCode =
        // revisedPlayers[1].TieBreakerCode }; bagian 2: revisedPlayers[1] with { TieBreakerCode = revisedPlayers[0].TieBreakerCode } dalam
        // SetupEndpoints_RequireSetupAllowRetryAndLockLatestRevisionOnStart.
        (revisedPlayers[0], revisedPlayers[1]) = (
            // Meneruskan `revisedPlayers[0] with { TieBreakerCode = revisedPlayers[1].TieBreakerCode }` sebagai argumen ke
            // `SetupEndpoints_RequireSetupAllowRetryAndLockLatestRevisionOnStart`; Meneruskan nilai literal `0` sebagai argumen ke
            // `SetupEndpoints_RequireSetupAllowRetryAndLockLatestRevisionOnStart`; Meneruskan nilai literal `1` sebagai argumen ke
            // `SetupEndpoints_RequireSetupAllowRetryAndLockLatestRevisionOnStart`.
            revisedPlayers[0] with { TieBreakerCode = revisedPlayers[1].TieBreakerCode },
            // Meneruskan `revisedPlayers[1] with { TieBreakerCode = revisedPlayers[0].TieBreakerCode }` sebagai argumen ke
            // `SetupEndpoints_RequireSetupAllowRetryAndLockLatestRevisionOnStart`; Meneruskan nilai literal `1` sebagai argumen ke
            // `SetupEndpoints_RequireSetupAllowRetryAndLockLatestRevisionOnStart`; Meneruskan nilai literal `0` sebagai argumen ke
            // `SetupEndpoints_RequireSetupAllowRetryAndLockLatestRevisionOnStart`.
            revisedPlayers[1] with { TieBreakerCode = revisedPlayers[0].TieBreakerCode });
        // Menyiapkan variabel lokal `revisionResponse` untuk nilai revision respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Post`, `$”/api/v1/sessions/{prepared.SessionId}/setup”`, `new SessionSetupRequest($”test-setup-revision-{prepared.SessionId:N}”,
        // revisedPlayers)`, `token`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi
        // nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var revisionResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Post` (nilai post) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Post,
            // Meneruskan teks interpolasi `$”/api/v1/sessions/{prepared.SessionId}/setup”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program
            // berjalan sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/sessions/{prepared.SessionId}/setup",
            // Meneruskan objek baru bertipe `SessionSetupRequest` dengan argumen ($”test-setup-revision-{prepared.SessionId:N}”, revisedPlayers) sebagai
            // argumen ke `SendJsonAsync`; Meneruskan teks interpolasi `$”test-setup-revision-{prepared.SessionId:N}”`; nilai ekspresi di dalam kurung kurawal
            // disisipkan saat program berjalan sebagai argumen ke konstruktor `SessionSetupRequest`; Meneruskan `revisedPlayers` (nilai revised pemain) sebagai
            // argumen ke konstruktor `SessionSetupRequest`.
            new SessionSetupRequest($"test-setup-revision-{prepared.SessionId:N}", revisedPlayers),
            // Meneruskan `token` (token yang diteruskan pada operasi terkait) sebagai argumen ke `SendJsonAsync`.
            token);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.Created`,
        // `revisionResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam SetupEndpoints_RequireSetupAllowRetryAndLockLatestRevisionOnStart.
        Assert.Equal(HttpStatusCode.Created, revisionResponse.StatusCode);
        // Menyiapkan variabel lokal `revision` untuk nomor revisi data untuk membedakan versi penyimpanan dengan hasil operasi asinkron membaca tanpa
        // argumen menjadi objek bertipe sesuai kontrak JSON melalui `revisionResponse.Content.ReadFromJsonAsync<SessionSetupResponse>`; await menunggu
        // hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var revision = await revisionResponse.Content.ReadFromJsonAsync<SessionSetupResponse>();
        // Menjalankan pemeriksaan NotNull atas `revision` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // SetupEndpoints_RequireSetupAllowRetryAndLockLatestRevisionOnStart.
        Assert.NotNull(revision);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`2`, `revision.Revision`); pengujian gagal jika
        // keduanya berbeda dalam SetupEndpoints_RequireSetupAllowRetryAndLockLatestRevisionOnStart.
        Assert.Equal(2, revision.Revision);

        // Menyiapkan variabel lokal `extraPlayerResponse` untuk nilai extra pemain respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Post`, `”/api/v1/players”`, `new CreatePlayerRequest(”Roster Locked”, $”locked_{Guid.NewGuid():N}”, ”SessionStatePlayerPass!123”)`,
        // `token`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal; using
        // memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var extraPlayerResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Post` (nilai post) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Post,
            // Meneruskan nilai literal `”/api/v1/players”` sebagai argumen ke `SendJsonAsync`.
            "/api/v1/players",
            // Meneruskan objek baru bertipe `CreatePlayerRequest` dengan argumen (”Roster Locked”, $”locked_{Guid.NewGuid():N}”, ”SessionStatePlayerPass!123”)
            // sebagai argumen ke `SendJsonAsync`; Meneruskan nilai literal `”Roster Locked”` sebagai argumen ke konstruktor `CreatePlayerRequest`; Meneruskan
            // teks interpolasi `$”locked_{Guid.NewGuid():N}”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan sebagai argumen ke
            // konstruktor `CreatePlayerRequest`; Meneruskan nilai literal `”SessionStatePlayerPass!123”` sebagai argumen ke konstruktor `CreatePlayerRequest`.
            new CreatePlayerRequest("Roster Locked", $"locked_{Guid.NewGuid():N}", "SessionStatePlayerPass!123"),
            // Meneruskan `token` (token yang diteruskan pada operasi terkait) sebagai argumen ke `SendJsonAsync`.
            token);
        // Menyiapkan variabel lokal `extraPlayer` untuk nilai extra pemain dengan hasil operasi asinkron membaca tanpa argumen menjadi objek bertipe sesuai
        // kontrak JSON melalui `extraPlayerResponse.Content.ReadFromJsonAsync<PlayerResponse>`; await menunggu hasil tanpa memblokir thread selama operasi
        // belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var extraPlayer = await extraPlayerResponse.Content.ReadFromJsonAsync<PlayerResponse>();
        // Menjalankan pemeriksaan NotNull atas `extraPlayer` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // SetupEndpoints_RequireSetupAllowRetryAndLockLatestRevisionOnStart.
        Assert.NotNull(extraPlayer);
        // Menyiapkan variabel lokal `addAfterSetupResponse` untuk nilai add after setup respons dengan hasil operasi asinkron memanggil `SendJsonAsync`
        // dengan `HttpMethod.Post`, `$”/api/v1/sessions/{prepared.SessionId}/players”`, `new AddSessionPlayerRequest(extraPlayer.UserId, null, 3)`,
        // `token`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal; using
        // memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var addAfterSetupResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Post` (nilai post) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Post,
            // Meneruskan teks interpolasi `$”/api/v1/sessions/{prepared.SessionId}/players”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program
            // berjalan sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/sessions/{prepared.SessionId}/players",
            // Meneruskan objek baru bertipe `AddSessionPlayerRequest` dengan argumen (extraPlayer.UserId, null, 3) sebagai argumen ke `SendJsonAsync`;
            // Meneruskan `extraPlayer.UserId` (identitas akun pengguna yang datanya sedang diproses) sebagai argumen ke konstruktor `AddSessionPlayerRequest`;
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen ke konstruktor `AddSessionPlayerRequest`; Meneruskan nilai literal `3` sebagai
            // argumen ke konstruktor `AddSessionPlayerRequest`.
            new AddSessionPlayerRequest(extraPlayer.UserId, null, 3),
            // Meneruskan `token` (token yang diteruskan pada operasi terkait) sebagai argumen ke `SendJsonAsync`.
            token);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.Conflict`,
        // `addAfterSetupResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam
        // SetupEndpoints_RequireSetupAllowRetryAndLockLatestRevisionOnStart.
        Assert.Equal(HttpStatusCode.Conflict, addAfterSetupResponse.StatusCode);

        // Menyiapkan variabel lokal `getSetupResponse` untuk nilai get setup respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Get`, `$”/api/v1/sessions/{prepared.SessionId}/setup”`, `null`, `token`; await menunggu hasil tanpa memblokir thread selama operasi
        // belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var getSetupResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Get` (nilai get) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Get,
            // Meneruskan teks interpolasi `$”/api/v1/sessions/{prepared.SessionId}/setup”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program
            // berjalan sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/sessions/{prepared.SessionId}/setup",
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen ke `SendJsonAsync`.
            null,
            // Meneruskan `token` (token yang diteruskan pada operasi terkait) sebagai argumen ke `SendJsonAsync`.
            token);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`,
        // `getSetupResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam SetupEndpoints_RequireSetupAllowRetryAndLockLatestRevisionOnStart.
        Assert.Equal(HttpStatusCode.OK, getSetupResponse.StatusCode);
        // Menyiapkan variabel lokal `storedSetup` untuk nilai stored setup dengan hasil operasi asinkron membaca tanpa argumen menjadi objek bertipe sesuai
        // kontrak JSON melalui `getSetupResponse.Content.ReadFromJsonAsync<SessionSetupResponse>`; await menunggu hasil tanpa memblokir thread selama
        // operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var storedSetup = await getSetupResponse.Content.ReadFromJsonAsync<SessionSetupResponse>();
        // Menjalankan pemeriksaan NotNull atas `storedSetup` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // SetupEndpoints_RequireSetupAllowRetryAndLockLatestRevisionOnStart.
        Assert.NotNull(storedSetup);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`2`, `storedSetup.Revision`); pengujian gagal
        // jika keduanya berbeda dalam SetupEndpoints_RequireSetupAllowRetryAndLockLatestRevisionOnStart.
        Assert.Equal(2, storedSetup.Revision);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”EDITABLE”`, `storedSetup.SetupStatus`);
        // pengujian gagal jika keduanya berbeda dalam SetupEndpoints_RequireSetupAllowRetryAndLockLatestRevisionOnStart.
        Assert.Equal("EDITABLE", storedSetup.SetupStatus);
        // Menjalankan pemeriksaan Null atas `storedSetup.LockedAt` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // SetupEndpoints_RequireSetupAllowRetryAndLockLatestRevisionOnStart.
        Assert.Null(storedSetup.LockedAt);

        // Menyiapkan variabel lokal `startResponse` untuk nilai start respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Post`, `$”/api/v1/sessions/{prepared.SessionId}/start”`, `null`, `token`; await menunggu hasil tanpa memblokir thread selama operasi
        // belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var startResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Post` (nilai post) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Post,
            // Meneruskan teks interpolasi `$”/api/v1/sessions/{prepared.SessionId}/start”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program
            // berjalan sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/sessions/{prepared.SessionId}/start",
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen ke `SendJsonAsync`.
            null,
            // Meneruskan `token` (token yang diteruskan pada operasi terkait) sebagai argumen ke `SendJsonAsync`.
            token);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`,
        // `startResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam SetupEndpoints_RequireSetupAllowRetryAndLockLatestRevisionOnStart.
        Assert.Equal(HttpStatusCode.OK, startResponse.StatusCode);

        // Menyiapkan variabel lokal `lockedSetupResponse` untuk nilai locked setup respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Get`, `$”/api/v1/sessions/{prepared.SessionId}/setup”`, `null`, `token`; await menunggu hasil tanpa memblokir thread selama operasi
        // belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var lockedSetupResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Get` (nilai get) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Get,
            // Meneruskan teks interpolasi `$”/api/v1/sessions/{prepared.SessionId}/setup”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program
            // berjalan sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/sessions/{prepared.SessionId}/setup",
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen ke `SendJsonAsync`.
            null,
            // Meneruskan `token` (token yang diteruskan pada operasi terkait) sebagai argumen ke `SendJsonAsync`.
            token);
        // Menyiapkan variabel lokal `lockedSetup` untuk nilai locked setup dengan hasil operasi asinkron membaca tanpa argumen menjadi objek bertipe sesuai
        // kontrak JSON melalui `lockedSetupResponse.Content.ReadFromJsonAsync<SessionSetupResponse>`; await menunggu hasil tanpa memblokir thread selama
        // operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var lockedSetup = await lockedSetupResponse.Content.ReadFromJsonAsync<SessionSetupResponse>();
        // Menjalankan pemeriksaan NotNull atas `lockedSetup` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // SetupEndpoints_RequireSetupAllowRetryAndLockLatestRevisionOnStart.
        Assert.NotNull(lockedSetup);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”LOCKED”`, `lockedSetup.SetupStatus`);
        // pengujian gagal jika keduanya berbeda dalam SetupEndpoints_RequireSetupAllowRetryAndLockLatestRevisionOnStart.
        Assert.Equal("LOCKED", lockedSetup.SetupStatus);
        // Menjalankan pemeriksaan NotNull atas `lockedSetup.LockedAt` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // SetupEndpoints_RequireSetupAllowRetryAndLockLatestRevisionOnStart.
        Assert.NotNull(lockedSetup.LockedAt);
    // Menutup scope metode SetupEndpoints_RequireSetupAllowRetryAndLockLatestRevisionOnStart; bagian berikut berada di luar batas blok tersebut dalam
    // SetupEndpoints_RequireSetupAllowRetryAndLockLatestRevisionOnStart.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `SessionStateEndpoints_RejectLegacyBootstrapInvalidPlayerCountAndWrongOwner` dengan hasil bertipe `Task`; operasi ini
    // menangani sesi keadaan endpoints reject legacy bootstrap invalid pemain jumlah dan wrong owner. async memungkinkan metode menunggu operasi I/O
    // dengan await dan mengembalikan penyelesaian melalui Task.
    public async Task SessionStateEndpoints_RejectLegacyBootstrapInvalidPlayerCountAndWrongOwner()
    // Membuka scope metode SessionStateEndpoints_RejectLegacyBootstrapInvalidPlayerCountAndWrongOwner; pernyataan/deklarasi berikut berada di dalam
    // batas blok ini dalam SessionStateEndpoints_RejectLegacyBootstrapInvalidPlayerCountAndWrongOwner.
    {
        // Menyiapkan variabel lokal `ownerToken` untuk nilai owner token dengan hasil operasi asinkron memanggil `RegisterInstructorAndGetTokenAsync`
        // dengan tanpa argumen; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai
        // awal.
        var ownerToken = await RegisterInstructorAndGetTokenAsync();
        // Menyiapkan variabel lokal `otherInstructorToken` untuk nilai other instruktur token dengan hasil operasi asinkron memanggil
        // `RegisterInstructorAndGetTokenAsync` dengan tanpa argumen; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var otherInstructorToken = await RegisterInstructorAndGetTokenAsync();
        // Menyiapkan variabel lokal `ruleset` untuk nilai aturan dengan hasil operasi asinkron memanggil `GetDefaultRulesetAsync` dengan `”MAHIR”`,
        // `ownerToken`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var ruleset = await GetDefaultRulesetAsync("MAHIR", ownerToken);

        // Menyiapkan variabel lokal `legacyBootstrapResponse` untuk nilai legacy bootstrap respons dengan hasil operasi asinkron memanggil `SendJsonAsync`
        // dengan `HttpMethod.Post`, `”/api/v1/sessions”`, `new { session_name = $”Legacy Bootstrap {Guid.NewGuid():N}”, mode = ”MAHIR”, ruleset_version_id
        // = ruleset.RulesetVersionId, player_names = new[] { ”A” } }`, `ownerToken`; await menunggu hasil tanpa memblokir thread selama operasi belum
        // selesai. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var legacyBootstrapResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Post` (nilai post) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Post,
            // Meneruskan nilai literal `”/api/v1/sessions”` sebagai argumen ke `SendJsonAsync`.
            "/api/v1/sessions",
            // Meneruskan objek anonim yang mengelompokkan session_name, mode, ruleset_version_id, player_names sebagai satu nilai sebagai argumen ke
            // `SendJsonAsync`.
            new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // SessionStateEndpoints_RejectLegacyBootstrapInvalidPlayerCountAndWrongOwner.
            {
                // Meneruskan objek anonim yang mengelompokkan session_name, mode, ruleset_version_id, player_names sebagai satu nilai sebagai argumen ke
                // `SendJsonAsync`.
                session_name = $"Legacy Bootstrap {Guid.NewGuid():N}",
                // Meneruskan objek anonim yang mengelompokkan session_name, mode, ruleset_version_id, player_names sebagai satu nilai sebagai argumen ke
                // `SendJsonAsync`.
                mode = "MAHIR",
                // Meneruskan objek anonim yang mengelompokkan session_name, mode, ruleset_version_id, player_names sebagai satu nilai sebagai argumen ke
                // `SendJsonAsync`.
                ruleset_version_id = ruleset.RulesetVersionId,
                // Meneruskan objek anonim yang mengelompokkan session_name, mode, ruleset_version_id, player_names sebagai satu nilai sebagai argumen ke
                // `SendJsonAsync`.
                player_names = new[] { "A" }
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
            // SessionStateEndpoints_RejectLegacyBootstrapInvalidPlayerCountAndWrongOwner.
            },
            // Meneruskan `ownerToken` (nilai owner token) sebagai argumen ke `SendJsonAsync`.
            ownerToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.BadRequest`,
        // `legacyBootstrapResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam
        // SessionStateEndpoints_RejectLegacyBootstrapInvalidPlayerCountAndWrongOwner.
        Assert.Equal(HttpStatusCode.BadRequest, legacyBootstrapResponse.StatusCode);
        // Menyiapkan variabel lokal `legacyError` untuk nilai legacy kesalahan dengan hasil operasi asinkron membaca tanpa argumen menjadi objek bertipe
        // sesuai kontrak JSON melalui `legacyBootstrapResponse.Content.ReadFromJsonAsync<ErrorResponse>`; await menunggu hasil tanpa memblokir thread
        // selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var legacyError = await legacyBootstrapResponse.Content.ReadFromJsonAsync<ErrorResponse>();
        // Menjalankan pemeriksaan NotNull atas `legacyError` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // SessionStateEndpoints_RejectLegacyBootstrapInvalidPlayerCountAndWrongOwner.
        Assert.NotNull(legacyError);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `legacyError.Details`, `detail =>
        // detail.Field == ”player_names” && detail.Issue == ”NOT_ALLOWED”` dalam
        // SessionStateEndpoints_RejectLegacyBootstrapInvalidPlayerCountAndWrongOwner.
        Assert.Contains(legacyError.Details, detail => detail.Field == "player_names" && detail.Issue == "NOT_ALLOWED");

        // Menyiapkan variabel lokal `lowCountSessionResponse` untuk nilai low jumlah sesi respons dengan hasil operasi asinkron memanggil `SendJsonAsync`
        // dengan `HttpMethod.Post`, `”/api/v1/sessions”`, `new { session_name = $”Invalid Low Count {Guid.NewGuid():N}”, mode = ”MAHIR”, ruleset_version_id
        // = ruleset.RulesetVersionId }`, `ownerToken`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan
        // dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var lowCountSessionResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Post` (nilai post) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Post,
            // Meneruskan nilai literal `”/api/v1/sessions”` sebagai argumen ke `SendJsonAsync`.
            "/api/v1/sessions",
            // Meneruskan objek anonim yang mengelompokkan session_name, mode, ruleset_version_id sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // SessionStateEndpoints_RejectLegacyBootstrapInvalidPlayerCountAndWrongOwner.
            {
                // Meneruskan objek anonim yang mengelompokkan session_name, mode, ruleset_version_id sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                session_name = $"Invalid Low Count {Guid.NewGuid():N}",
                // Meneruskan objek anonim yang mengelompokkan session_name, mode, ruleset_version_id sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                mode = "MAHIR",
                // Meneruskan objek anonim yang mengelompokkan session_name, mode, ruleset_version_id sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                ruleset_version_id = ruleset.RulesetVersionId
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
            // SessionStateEndpoints_RejectLegacyBootstrapInvalidPlayerCountAndWrongOwner.
            },
            // Meneruskan `ownerToken` (nilai owner token) sebagai argumen ke `SendJsonAsync`.
            ownerToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.Created`,
        // `lowCountSessionResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam
        // SessionStateEndpoints_RejectLegacyBootstrapInvalidPlayerCountAndWrongOwner.
        Assert.Equal(HttpStatusCode.Created, lowCountSessionResponse.StatusCode);
        // Menyiapkan variabel lokal `lowCountSession` untuk nilai low jumlah sesi dengan hasil operasi asinkron membaca tanpa argumen menjadi objek bertipe
        // sesuai kontrak JSON melalui `lowCountSessionResponse.Content.ReadFromJsonAsync<CreateSessionResponse>`; await menunggu hasil tanpa memblokir
        // thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var lowCountSession = await lowCountSessionResponse.Content.ReadFromJsonAsync<CreateSessionResponse>();
        // Menjalankan pemeriksaan NotNull atas `lowCountSession` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // SessionStateEndpoints_RejectLegacyBootstrapInvalidPlayerCountAndWrongOwner.
        Assert.NotNull(lowCountSession);

        // Menyiapkan variabel lokal `createPlayerResponse` untuk nilai create pemain respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Post`, `”/api/v1/players”`, `new CreatePlayerRequest(”Only Player”, $”only_player_{Guid.NewGuid():N}”, ”OnlyPlayerPass!123”)`,
        // `ownerToken`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal; using
        // memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var createPlayerResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Post` (nilai post) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Post,
            // Meneruskan nilai literal `”/api/v1/players”` sebagai argumen ke `SendJsonAsync`.
            "/api/v1/players",
            // Meneruskan objek baru bertipe `CreatePlayerRequest` dengan argumen (”Only Player”, $”only_player_{Guid.NewGuid():N}”, ”OnlyPlayerPass!123”)
            // sebagai argumen ke `SendJsonAsync`; Meneruskan nilai literal `”Only Player”` sebagai argumen ke konstruktor `CreatePlayerRequest`; Meneruskan
            // teks interpolasi `$”only_player_{Guid.NewGuid():N}”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan sebagai argumen ke
            // konstruktor `CreatePlayerRequest`; Meneruskan nilai literal `”OnlyPlayerPass!123”` sebagai argumen ke konstruktor `CreatePlayerRequest`.
            new CreatePlayerRequest("Only Player", $"only_player_{Guid.NewGuid():N}", "OnlyPlayerPass!123"),
            // Meneruskan `ownerToken` (nilai owner token) sebagai argumen ke `SendJsonAsync`.
            ownerToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.Created`,
        // `createPlayerResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam
        // SessionStateEndpoints_RejectLegacyBootstrapInvalidPlayerCountAndWrongOwner.
        Assert.Equal(HttpStatusCode.Created, createPlayerResponse.StatusCode);
        // Menyiapkan variabel lokal `onlyPlayer` untuk nilai only pemain dengan hasil operasi asinkron membaca tanpa argumen menjadi objek bertipe sesuai
        // kontrak JSON melalui `createPlayerResponse.Content.ReadFromJsonAsync<PlayerResponse>`; await menunggu hasil tanpa memblokir thread selama operasi
        // belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var onlyPlayer = await createPlayerResponse.Content.ReadFromJsonAsync<PlayerResponse>();
        // Menjalankan pemeriksaan NotNull atas `onlyPlayer` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // SessionStateEndpoints_RejectLegacyBootstrapInvalidPlayerCountAndWrongOwner.
        Assert.NotNull(onlyPlayer);

        // Menyiapkan variabel lokal `addOnlyPlayerResponse` untuk nilai add only pemain respons dengan hasil operasi asinkron memanggil `SendJsonAsync`
        // dengan `HttpMethod.Post`, `$”/api/v1/sessions/{lowCountSession.SessionId}/players”`, `new AddSessionPlayerRequest(onlyPlayer.UserId, null, 1)`,
        // `ownerToken`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal; using
        // memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var addOnlyPlayerResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Post` (nilai post) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Post,
            // Meneruskan teks interpolasi `$”/api/v1/sessions/{lowCountSession.SessionId}/players”`; nilai ekspresi di dalam kurung kurawal disisipkan saat
            // program berjalan sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/sessions/{lowCountSession.SessionId}/players",
            // Meneruskan objek baru bertipe `AddSessionPlayerRequest` dengan argumen (onlyPlayer.UserId, null, 1) sebagai argumen ke `SendJsonAsync`;
            // Meneruskan `onlyPlayer.UserId` (identitas akun pengguna yang datanya sedang diproses) sebagai argumen ke konstruktor `AddSessionPlayerRequest`;
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen ke konstruktor `AddSessionPlayerRequest`; Meneruskan nilai literal `1` sebagai
            // argumen ke konstruktor `AddSessionPlayerRequest`.
            new AddSessionPlayerRequest(onlyPlayer.UserId, null, 1),
            // Meneruskan `ownerToken` (nilai owner token) sebagai argumen ke `SendJsonAsync`.
            ownerToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`,
        // `addOnlyPlayerResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam
        // SessionStateEndpoints_RejectLegacyBootstrapInvalidPlayerCountAndWrongOwner.
        Assert.Equal(HttpStatusCode.OK, addOnlyPlayerResponse.StatusCode);

        // Menyiapkan variabel lokal `lowCountStartResponse` untuk nilai low jumlah start respons dengan hasil operasi asinkron memanggil `SendJsonAsync`
        // dengan `HttpMethod.Post`, `$”/api/v1/sessions/{lowCountSession.SessionId}/start”`, `null`, `ownerToken`; await menunggu hasil tanpa memblokir
        // thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope
        // berakhir.
        using var lowCountStartResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Post` (nilai post) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Post,
            // Meneruskan teks interpolasi `$”/api/v1/sessions/{lowCountSession.SessionId}/start”`; nilai ekspresi di dalam kurung kurawal disisipkan saat
            // program berjalan sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/sessions/{lowCountSession.SessionId}/start",
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen ke `SendJsonAsync`.
            null,
            // Meneruskan `ownerToken` (nilai owner token) sebagai argumen ke `SendJsonAsync`.
            ownerToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.UnprocessableEntity`,
        // `lowCountStartResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam
        // SessionStateEndpoints_RejectLegacyBootstrapInvalidPlayerCountAndWrongOwner.
        Assert.Equal(HttpStatusCode.UnprocessableEntity, lowCountStartResponse.StatusCode);

        // Menyiapkan variabel lokal `started` untuk nilai started dengan hasil operasi asinkron memanggil `CreateStartedSessionAsync` dengan `ownerToken`,
        // `[”Doni”, ”Rani”, ”Bimo”]`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi
        // nilai awal.
        var started = await CreateStartedSessionAsync(ownerToken, ["Doni", "Rani", "Bimo"]);
        // Menyiapkan variabel lokal `sessionId` untuk identitas unik sesi permainan yang menjadi batas data operasi ini dengan `started.SessionId`
        // (identitas unik sesi permainan yang menjadi batas data operasi ini). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var sessionId = started.SessionId;
        // Menyiapkan variabel lokal `players` untuk nilai pemain dengan mematerialisasi urutan `started.State.GetProperty(”players”).EnumerateArray()`
        // menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var players = started.State.GetProperty("players").EnumerateArray().ToList();
        // Menyiapkan variabel lokal `firstPlayerId` untuk nilai first pemain identitas dengan memanggil
        // `players[0].GetProperty(”session_player_id”).GetGuid` dengan tanpa argumen. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var firstPlayerId = players[0].GetProperty("session_player_id").GetGuid();
        // Menyiapkan variabel lokal `secondPlayerId` untuk nilai second pemain identitas dengan memanggil
        // `players[1].GetProperty(”session_player_id”).GetGuid` dengan tanpa argumen. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var secondPlayerId = players[1].GetProperty("session_player_id").GetGuid();
        // Menyiapkan variabel lokal `thirdPlayerId` untuk nilai third pemain identitas dengan memanggil
        // `players[2].GetProperty(”session_player_id”).GetGuid` dengan tanpa argumen. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var thirdPlayerId = players[2].GetProperty("session_player_id").GetGuid();

        // Menyiapkan variabel lokal `wrongOwnerResponse` untuk nilai wrong owner respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Get`, `$”/api/v1/sessions/{sessionId}/state”`, `null`, `otherInstructorToken`; await menunggu hasil tanpa memblokir thread selama
        // operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var wrongOwnerResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Get` (nilai get) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Get,
            // Meneruskan teks interpolasi `$”/api/v1/sessions/{sessionId}/state”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan
            // sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/sessions/{sessionId}/state",
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen ke `SendJsonAsync`.
            null,
            // Meneruskan `otherInstructorToken` (nilai other instruktur token) sebagai argumen ke `SendJsonAsync`.
            otherInstructorToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.NotFound`,
        // `wrongOwnerResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam
        // SessionStateEndpoints_RejectLegacyBootstrapInvalidPlayerCountAndWrongOwner.
        Assert.Equal(HttpStatusCode.NotFound, wrongOwnerResponse.StatusCode);

        // Menyiapkan variabel lokal `negativeCoinsResponse` untuk nilai negative coins respons dengan hasil operasi asinkron memanggil `SendJsonAsync`
        // dengan `HttpMethod.Put`, `$”/api/v1/sessions/{sessionId}/state”`, `BuildStatePayload( stateVersion: 1, firstPlayerId, secondPlayerId,
        // thirdPlayerId, firstBahanNama: ”Nasi Putih”, firstCoins: -1)`, `ownerToken`; await menunggu hasil tanpa memblokir thread selama operasi belum
        // selesai. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var negativeCoinsResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Put` (nilai put) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Put,
            // Meneruskan teks interpolasi `$”/api/v1/sessions/{sessionId}/state”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan
            // sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/sessions/{sessionId}/state",
            // Meneruskan memanggil `BuildStatePayload` dengan `1`, `firstPlayerId`, `secondPlayerId`, `thirdPlayerId`, `”Nasi Putih”`, `-1` sebagai argumen ke
            // `SendJsonAsync`.
            BuildStatePayload(
                // Meneruskan nilai literal `1` sebagai argumen bernama `stateVersion`.
                stateVersion: 1,
                // Meneruskan `firstPlayerId` (nilai first pemain identitas) sebagai argumen ke `BuildStatePayload`.
                firstPlayerId,
                // Meneruskan `secondPlayerId` (nilai second pemain identitas) sebagai argumen ke `BuildStatePayload`.
                secondPlayerId,
                // Meneruskan `thirdPlayerId` (nilai third pemain identitas) sebagai argumen ke `BuildStatePayload`.
                thirdPlayerId,
            // Meneruskan nilai literal `”Nasi Putih”` sebagai argumen bernama `firstBahanNama`.
            firstBahanNama: "Nasi Putih",
            // Meneruskan `-1` sebagai argumen bernama `firstCoins`.
            firstCoins: -1),
            // Meneruskan `ownerToken` (nilai owner token) sebagai argumen ke `SendJsonAsync`.
            ownerToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.Gone`,
        // `negativeCoinsResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam
        // SessionStateEndpoints_RejectLegacyBootstrapInvalidPlayerCountAndWrongOwner.
        Assert.Equal(HttpStatusCode.Gone, negativeCoinsResponse.StatusCode);
    // Menutup scope metode SessionStateEndpoints_RejectLegacyBootstrapInvalidPlayerCountAndWrongOwner; bagian berikut berada di luar batas blok
    // tersebut dalam SessionStateEndpoints_RejectLegacyBootstrapInvalidPlayerCountAndWrongOwner.
    }

    // Mendefinisikan metode `BuildStatePayload` dengan hasil bertipe `object`; operasi ini menangani build keadaan payload. Masukan: Parameter
    // `stateVersion` bertipe `long` membawa nilai keadaan versi; Parameter `firstPlayerId` bertipe `Guid` membawa nilai first pemain identitas;
    // Parameter `secondPlayerId` bertipe `Guid` membawa nilai second pemain identitas; Parameter `thirdPlayerId` bertipe `Guid` membawa nilai third
    // pemain identitas; Parameter `firstBahanNama` bertipe `string` membawa nilai first bahan nama; Parameter `firstCoins` bertipe `int` membawa nilai
    // first coins; bila argumen tidak diberikan digunakan nilai literal `17`.
    private static object BuildStatePayload(
        // Parameter `stateVersion` bertipe `long` membawa nilai keadaan versi.
        long stateVersion,
        // Parameter `firstPlayerId` bertipe `Guid` membawa nilai first pemain identitas.
        Guid firstPlayerId,
        // Parameter `secondPlayerId` bertipe `Guid` membawa nilai second pemain identitas.
        Guid secondPlayerId,
        // Parameter `thirdPlayerId` bertipe `Guid` membawa nilai third pemain identitas.
        Guid thirdPlayerId,
        // Parameter `firstBahanNama` bertipe `string` membawa nilai first bahan nama.
        string firstBahanNama,
        // Parameter `firstCoins` bertipe `int` membawa nilai first coins; bila argumen tidak diberikan digunakan nilai literal `17`.
        int firstCoins = 17)
    // Membuka scope metode BuildStatePayload; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildStatePayload.
    {
        // Mengembalikan objek anonim yang mengelompokkan state_version, day, turn, action_slots_left, finish_day, is_game_over, ui_state, players,
        // donationEvents, last_action sebagai satu nilai kepada pemanggil dalam BuildStatePayload; eksekusi jalur ini selesai setelah nilai hasil
        // ditentukan.
        return new
        // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // BuildStatePayload.
        {
            // Menggunakan `state_version` (nilai keadaan versi) sebagai bagian ekspresi yang sedang disusun dalam BuildStatePayload.
            state_version = stateVersion,
            // Menggunakan `day` (nomor hari permainan yang menjadi konteks aktivitas) sebagai bagian ekspresi yang sedang disusun dalam BuildStatePayload.
            day = 2,
            // Menggunakan `turn` (giliran pemain yang sedang berlangsung) sebagai bagian ekspresi yang sedang disusun dalam BuildStatePayload.
            turn = 2,
            // Menggunakan `action_slots_left` (nilai aksi slots left) sebagai bagian ekspresi yang sedang disusun dalam BuildStatePayload.
            action_slots_left = 1,
            // Menggunakan `finish_day` (nilai finish hari) sebagai bagian ekspresi yang sedang disusun dalam BuildStatePayload.
            finish_day = 25,
            // Menggunakan `is_game_over` (nilai berstatus game over) sebagai bagian ekspresi yang sedang disusun dalam BuildStatePayload.
            is_game_over = false,
            // Menggunakan `ui_state` (nilai ui keadaan) sebagai bagian ekspresi yang sedang disusun dalam BuildStatePayload.
            ui_state = new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // BuildStatePayload.
            {
                // Menggunakan `SavingText` (nilai tabungan text) sebagai bagian ekspresi yang sedang disusun dalam BuildStatePayload.
                SavingText = "5",
                // Menggunakan `JumatBerkah` (nilai jumat berkah) sebagai bagian ekspresi yang sedang disusun dalam BuildStatePayload.
                JumatBerkah = false,
                // Menggunakan `kebutuhanSelected` (nilai kebutuhan selected) sebagai bagian ekspresi yang sedang disusun dalam BuildStatePayload.
                kebutuhanSelected = "buku"
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam BuildStatePayload.
            },
            // Menggunakan `players` (nilai pemain) sebagai bagian ekspresi yang sedang disusun dalam BuildStatePayload.
            players = new object[]
            // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildStatePayload.
            {
                // Menggunakan objek anonim yang mengelompokkan session_player_id, player_order_no, name, coins, happiness, saving, bahan, kebutuhan,
                // tujuanFinansial, targetKebutuhan, actionCounters, totalDonasi sebagai satu nilai sebagai bagian ekspresi yang sedang disusun dalam
                // BuildStatePayload.
                new
                // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // BuildStatePayload.
                {
                    // Menggunakan `session_player_id` (nilai sesi pemain identitas) sebagai bagian ekspresi yang sedang disusun dalam BuildStatePayload.
                    session_player_id = firstPlayerId,
                    // Menggunakan `player_order_no` (nilai pemain urutan/pesanan no) sebagai bagian ekspresi yang sedang disusun dalam BuildStatePayload.
                    player_order_no = 1,
                    // Menggunakan `name` (nilai nama) sebagai bagian ekspresi yang sedang disusun dalam BuildStatePayload.
                    name = "Doni",
                    // Menggunakan `coins` (nilai coins) sebagai bagian ekspresi yang sedang disusun dalam BuildStatePayload.
                    coins = firstCoins,
                    // Menggunakan `happiness` (nilai kebahagiaan) sebagai bagian ekspresi yang sedang disusun dalam BuildStatePayload.
                    happiness = 3,
                    // Menggunakan `saving` (nilai tabungan) sebagai bagian ekspresi yang sedang disusun dalam BuildStatePayload.
                    saving = 5,
                    // Menggunakan `bahan` (nilai bahan) sebagai bagian ekspresi yang sedang disusun dalam BuildStatePayload.
                    bahan = new[] { new { nama = firstBahanNama, jumlah = 2 } },
                    // Menggunakan `kebutuhan` (nilai kebutuhan) sebagai bagian ekspresi yang sedang disusun dalam BuildStatePayload.
                    kebutuhan = new[] { new { nama = "buku", tipe = "primer" } },
                    // Menggunakan `tujuanFinansial` (nilai tujuan finansial) sebagai bagian ekspresi yang sedang disusun dalam BuildStatePayload.
                    tujuanFinansial = new[]
                    // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildStatePayload.
                    {
                        // Menggunakan objek anonim yang mengelompokkan nama, current_amount, target_amount, status, purchased_at_day sebagai satu nilai sebagai bagian
                        // ekspresi yang sedang disusun dalam BuildStatePayload.
                        new
                        // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                        // BuildStatePayload.
                        {
                            // Menggunakan `nama` (nilai nama) sebagai bagian ekspresi yang sedang disusun dalam BuildStatePayload.
                            nama = "beli rumah",
                            // Menggunakan `current_amount` (nilai saat ini nominal) sebagai bagian ekspresi yang sedang disusun dalam BuildStatePayload.
                            current_amount = 12,
                            // Menggunakan `target_amount` (nilai target nominal) sebagai bagian ekspresi yang sedang disusun dalam BuildStatePayload.
                            target_amount = 20,
                            // Menggunakan `status` (nilai status) sebagai bagian ekspresi yang sedang disusun dalam BuildStatePayload.
                            status = "COMPLETED",
                            // Menggunakan `purchased_at_day` (nilai dibeli at hari) sebagai bagian ekspresi yang sedang disusun dalam BuildStatePayload.
                            purchased_at_day = 2
                        // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam BuildStatePayload.
                        }
                    // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam BuildStatePayload.
                    },
                    // Menggunakan `targetKebutuhan` (nilai target kebutuhan) sebagai bagian ekspresi yang sedang disusun dalam BuildStatePayload.
                    targetKebutuhan = new[]
                    // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildStatePayload.
                    {
                        // Menggunakan objek anonim yang mengelompokkan id, is_completed, is_failed, reward_applied sebagai satu nilai sebagai bagian ekspresi yang sedang
                        // disusun dalam BuildStatePayload.
                        new
                        // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                        // BuildStatePayload.
                        {
                            // Menggunakan `id` (nilai identitas) sebagai bagian ekspresi yang sedang disusun dalam BuildStatePayload.
                            id = "misi_boneka",
                            // Menggunakan `is_completed` (nilai berstatus selesai) sebagai bagian ekspresi yang sedang disusun dalam BuildStatePayload.
                            is_completed = false,
                            // Menggunakan `is_failed` (nilai berstatus failed) sebagai bagian ekspresi yang sedang disusun dalam BuildStatePayload.
                            is_failed = false,
                            // Menggunakan `reward_applied` (nilai reward applied) sebagai bagian ekspresi yang sedang disusun dalam BuildStatePayload.
                            reward_applied = false
                        // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam BuildStatePayload.
                        }
                    // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam BuildStatePayload.
                    },
                    // Menggunakan `actionCounters` (nilai aksi counters) sebagai bagian ekspresi yang sedang disusun dalam BuildStatePayload.
                    actionCounters = new[] { new { aksi = "JualMasakan", count = 1 } },
                    // Menggunakan `totalDonasi` (nilai total donasi) sebagai bagian ekspresi yang sedang disusun dalam BuildStatePayload.
                    totalDonasi = 4
                // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam BuildStatePayload.
                },
                // Melanjutkan pengolahan dengan memanggil `BuildEmptyPlayer` dengan `secondPlayerId`, `2`, `”Rani”` dalam BuildStatePayload.
                BuildEmptyPlayer(secondPlayerId, 2, "Rani"),
                // Melanjutkan pengolahan dengan memanggil `BuildEmptyPlayer` dengan `thirdPlayerId`, `3`, `”Bimo”` dalam BuildStatePayload.
                BuildEmptyPlayer(thirdPlayerId, 3, "Bimo")
            // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam BuildStatePayload.
            },
            // Menggunakan `donationEvents` (nilai donasi event) sebagai bagian ekspresi yang sedang disusun dalam BuildStatePayload.
            donationEvents = new[]
            // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildStatePayload.
            {
                // Menggunakan objek anonim yang mengelompokkan event_ke, day, rankings sebagai satu nilai sebagai bagian ekspresi yang sedang disusun dalam
                // BuildStatePayload.
                new
                // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // BuildStatePayload.
                {
                    // Menggunakan `event_ke` (nilai event ke) sebagai bagian ekspresi yang sedang disusun dalam BuildStatePayload.
                    event_ke = 1,
                    // Menggunakan `day` (nomor hari permainan yang menjadi konteks aktivitas) sebagai bagian ekspresi yang sedang disusun dalam BuildStatePayload.
                    day = 5,
                    // Menggunakan `rankings` (nilai rankings) sebagai bagian ekspresi yang sedang disusun dalam BuildStatePayload.
                    rankings = new[]
                    // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildStatePayload.
                    {
                        // Menggunakan objek anonim yang mengelompokkan rank, session_player_id, total_donasi sebagai satu nilai sebagai bagian ekspresi yang sedang disusun
                        // dalam BuildStatePayload.
                        new { rank = 1, session_player_id = firstPlayerId, total_donasi = 4 },
                        // Menggunakan objek anonim yang mengelompokkan rank, session_player_id, total_donasi sebagai satu nilai sebagai bagian ekspresi yang sedang disusun
                        // dalam BuildStatePayload.
                        new { rank = 2, session_player_id = secondPlayerId, total_donasi = 0 },
                        // Menggunakan objek anonim yang mengelompokkan rank, session_player_id, total_donasi sebagai satu nilai sebagai bagian ekspresi yang sedang disusun
                        // dalam BuildStatePayload.
                        new { rank = 3, session_player_id = thirdPlayerId, total_donasi = 0 }
                    // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam BuildStatePayload.
                    }
                // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam BuildStatePayload.
                }
            // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam BuildStatePayload.
            },
            // Menggunakan `last_action` (nilai last aksi) sebagai bagian ekspresi yang sedang disusun dalam BuildStatePayload.
            last_action = new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // BuildStatePayload.
            {
                // Menggunakan `aksi` (nilai aksi) sebagai bagian ekspresi yang sedang disusun dalam BuildStatePayload.
                aksi = "JualMasakan",
                // Menggunakan `player_order_no` (nilai pemain urutan/pesanan no) sebagai bagian ekspresi yang sedang disusun dalam BuildStatePayload.
                player_order_no = 1,
                // Menggunakan `payload` (muatan detail event dalam format JSON) sebagai bagian ekspresi yang sedang disusun dalam BuildStatePayload.
                payload = new { resep = "nasi goreng" }
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam BuildStatePayload.
            }
        // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam BuildStatePayload.
        };
    // Menutup scope metode BuildStatePayload; bagian berikut berada di luar batas blok tersebut dalam BuildStatePayload.
    }

    // Mendefinisikan metode `BuildEmptyPlayer` dengan hasil bertipe `object`; operasi ini menangani build empty pemain. Masukan: Parameter
    // `sessionPlayerId` bertipe `Guid` membawa identitas keikutsertaan pemain pada sesi tertentu; Parameter `playerIndex` bertipe `int` membawa nilai
    // pemain index; Parameter `name` bertipe `string` membawa nilai nama.
    private static object BuildEmptyPlayer(Guid sessionPlayerId, int playerIndex, string name)
    // Membuka scope metode BuildEmptyPlayer; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildEmptyPlayer.
    {
        // Mengembalikan objek anonim yang mengelompokkan session_player_id, player_order_no, name, coins, happiness, saving, bahan, kebutuhan,
        // tujuanFinansial, targetKebutuhan, actionCounters, totalDonasi sebagai satu nilai kepada pemanggil dalam BuildEmptyPlayer; eksekusi jalur ini
        // selesai setelah nilai hasil ditentukan.
        return new
        // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // BuildEmptyPlayer.
        {
            // Menggunakan `session_player_id` (nilai sesi pemain identitas) sebagai bagian ekspresi yang sedang disusun dalam BuildEmptyPlayer.
            session_player_id = sessionPlayerId,
            // Menggunakan `player_order_no` (nilai pemain urutan/pesanan no) sebagai bagian ekspresi yang sedang disusun dalam BuildEmptyPlayer.
            player_order_no = playerIndex,
            // Menggunakan `name` (nilai nama) sebagai bagian ekspresi yang sedang disusun dalam BuildEmptyPlayer.
            name,
            // Menggunakan `coins` (nilai coins) sebagai bagian ekspresi yang sedang disusun dalam BuildEmptyPlayer.
            coins = 10,
            // Menggunakan `happiness` (nilai kebahagiaan) sebagai bagian ekspresi yang sedang disusun dalam BuildEmptyPlayer.
            happiness = 0,
            // Menggunakan `saving` (nilai tabungan) sebagai bagian ekspresi yang sedang disusun dalam BuildEmptyPlayer.
            saving = 0,
            // Menggunakan `bahan` (nilai bahan) sebagai bagian ekspresi yang sedang disusun dalam BuildEmptyPlayer.
            bahan = Array.Empty<object>(),
            // Menggunakan `kebutuhan` (nilai kebutuhan) sebagai bagian ekspresi yang sedang disusun dalam BuildEmptyPlayer.
            kebutuhan = Array.Empty<object>(),
            // Menggunakan `tujuanFinansial` (nilai tujuan finansial) sebagai bagian ekspresi yang sedang disusun dalam BuildEmptyPlayer.
            tujuanFinansial = Array.Empty<object>(),
            // Menggunakan `targetKebutuhan` (nilai target kebutuhan) sebagai bagian ekspresi yang sedang disusun dalam BuildEmptyPlayer.
            targetKebutuhan = Array.Empty<object>(),
            // Menggunakan `actionCounters` (nilai aksi counters) sebagai bagian ekspresi yang sedang disusun dalam BuildEmptyPlayer.
            actionCounters = Array.Empty<object>(),
            // Menggunakan `totalDonasi` (nilai total donasi) sebagai bagian ekspresi yang sedang disusun dalam BuildEmptyPlayer.
            totalDonasi = 0
        // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam BuildEmptyPlayer.
        };
    // Menutup scope metode BuildEmptyPlayer; bagian berikut berada di luar batas blok tersebut dalam BuildEmptyPlayer.
    }

    // Mendefinisikan metode `AssertInitialState` dengan hasil bertipe `void`; operasi ini menangani assert awal keadaan. Masukan: Parameter `state`
    // bertipe `JsonElement` membawa keadaan permainan yang menjadi sumber atau hasil pembaruan; Parameter `playerCount` bertipe `int` membawa nilai
    // pemain jumlah; Parameter `names` bertipe `string[]` membawa nilai nama.
    private static void AssertInitialState(JsonElement state, int playerCount, string[] names)
    // Membuka scope metode AssertInitialState; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam AssertInitialState.
    {
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`1`,
        // `state.GetProperty(”state_version”).GetInt64()`); pengujian gagal jika keduanya berbeda dalam AssertInitialState.
        Assert.Equal(1, state.GetProperty("state_version").GetInt64());
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`1 + (playerCount * 6)`,
        // `state.GetProperty(”next_sequence_number”).GetInt64()`); pengujian gagal jika keduanya berbeda dalam AssertInitialState.
        Assert.Equal(1 + (playerCount * 6), state.GetProperty("next_sequence_number").GetInt64());
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`1`, `state.GetProperty(”day”).GetInt32()`);
        // pengujian gagal jika keduanya berbeda dalam AssertInitialState.
        Assert.Equal(1, state.GetProperty("day").GetInt32());
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`1`, `state.GetProperty(”turn”).GetInt32()`);
        // pengujian gagal jika keduanya berbeda dalam AssertInitialState.
        Assert.Equal(1, state.GetProperty("turn").GetInt32());
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`2`,
        // `state.GetProperty(”action_slots_left”).GetInt32()`); pengujian gagal jika keduanya berbeda dalam AssertInitialState.
        Assert.Equal(2, state.GetProperty("action_slots_left").GetInt32());
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`25`,
        // `state.GetProperty(”finish_day”).GetInt32()`); pengujian gagal jika keduanya berbeda dalam AssertInitialState.
        Assert.Equal(25, state.GetProperty("finish_day").GetInt32());
        // Menjalankan pemeriksaan bahwa `state.GetProperty(”is_game_over”).GetBoolean()` bernilai salah; pengujian gagal jika kondisi justru terpenuhi
        // dalam AssertInitialState.
        Assert.False(state.GetProperty("is_game_over").GetBoolean());

        // Menyiapkan variabel lokal `players` untuk nilai pemain dengan mematerialisasi urutan `state.GetProperty(”players”).EnumerateArray()` menjadi
        // List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var players = state.GetProperty("players").EnumerateArray().ToList();
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`playerCount`, `players.Count`); pengujian
        // gagal jika keduanya berbeda dalam AssertInitialState.
        Assert.Equal(playerCount, players.Count);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`names.OrderBy(name => name,
        // StringComparer.Ordinal)`, `players.Select(item => item.GetProperty(”name”).GetString()!).OrderBy(name => name, StringComparer.Ordinal)`);
        // pengujian gagal jika keduanya berbeda dalam AssertInitialState.
        Assert.Equal(
            // Meneruskan mengurutkan `names` secara menaik berdasarkan `name => name`, `StringComparer.Ordinal` sebagai argumen ke `Assert.Equal`; Meneruskan
            // fungsi lambda `name => name` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen ke `names.OrderBy`; Meneruskan
            // `StringComparer.Ordinal` (nilai ordinal) sebagai argumen ke `names.OrderBy`.
            names.OrderBy(name => name, StringComparer.Ordinal),
            // Meneruskan mengurutkan `players.Select(item => item.GetProperty(”name”).GetString()!)` secara menaik berdasarkan `name => name`,
            // `StringComparer.Ordinal` sebagai argumen ke `Assert.Equal`; Meneruskan fungsi lambda `item => item.GetProperty(”name”).GetString()!` yang
            // dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen ke `players.Select`; Meneruskan nilai literal `”name”` sebagai
            // argumen ke `item.GetProperty`; Meneruskan fungsi lambda `name => name` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan
            // sebagai argumen ke `players.Select(item => item.GetProperty(”name”).GetString()!).OrderBy`; Meneruskan `StringComparer.Ordinal` (nilai ordinal)
            // sebagai argumen ke `players.Select(item => item.GetProperty(”name”).GetString()!).OrderBy`.
            players.Select(item => item.GetProperty("name").GetString()!).OrderBy(name => name, StringComparer.Ordinal));
        // Menyiapkan variabel lokal `expectedMissionIds` untuk nilai yang diharapkan misi identitas dengan objek baru bertipe `HashSet<string>` dengan
        // argumen (StringComparer.Ordinal). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var expectedMissionIds = new HashSet<string>(StringComparer.Ordinal)
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam AssertInitialState.
        {
            // Menggunakan nilai literal `”misi_boneka”` sebagai bagian ekspresi yang sedang disusun dalam AssertInitialState.
            "misi_boneka",
            // Menggunakan nilai literal `”misi_gameboy”` sebagai bagian ekspresi yang sedang disusun dalam AssertInitialState.
            "misi_gameboy",
            // Menggunakan nilai literal `”misi_hiburan”` sebagai bagian ekspresi yang sedang disusun dalam AssertInitialState.
            "misi_hiburan",
            // Menggunakan nilai literal `”misi_jam”` sebagai bagian ekspresi yang sedang disusun dalam AssertInitialState.
            "misi_jam"
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam AssertInitialState.
        };
        // Menyiapkan variabel lokal `ingredientPrices` untuk nilai bahan prices dengan objek baru bertipe `Dictionary<string, int>` dengan argumen
        // (StringComparer.Ordinal). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var ingredientPrices = new Dictionary<string, int>(StringComparer.Ordinal)
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam AssertInitialState.
        {
            // Memperbarui `[”Nasi Putih”]` menggunakan nilai literal `1` dalam AssertInitialState.
            ["Nasi Putih"] = 1,
            // Memperbarui `[”Sayur”]` menggunakan nilai literal `2` dalam AssertInitialState.
            ["Sayur"] = 2,
            // Memperbarui `[”Tahu Tempe”]` menggunakan nilai literal `3` dalam AssertInitialState.
            ["Tahu Tempe"] = 3,
            // Memperbarui `[”Telur”]` menggunakan nilai literal `4` dalam AssertInitialState.
            ["Telur"] = 4,
            // Memperbarui `[”Daging”]` menggunakan nilai literal `5` dalam AssertInitialState.
            ["Daging"] = 5
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam AssertInitialState.
        };
        // Memulai loop dengan inisialisasi `var i = 0`, berjalan selama `i < playerCount`, lalu memperbarui pencacah melalui `i++` dalam
        // AssertInitialState.
        for (var i = 0; i < playerCount; i++)
        // Membuka scope loop dengan syarat `i < playerCount`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam AssertInitialState.
        {
            // Menjalankan pemeriksaan hasil dengan `Assert.NotEqual` menggunakan `Guid.Empty`, `players[i].GetProperty(”session_player_id”).GetGuid()`;
            // ketidaksesuaian dengan ekspektasi membuat pengujian gagal dalam AssertInitialState.
            Assert.NotEqual(Guid.Empty, players[i].GetProperty("session_player_id").GetGuid());
            // Menjalankan pemeriksaan hasil dengan `Assert.NotEqual` menggunakan `Guid.Empty`, `players[i].GetProperty(”user_id”).GetGuid()`; ketidaksesuaian
            // dengan ekspektasi membuat pengujian gagal dalam AssertInitialState.
            Assert.NotEqual(Guid.Empty, players[i].GetProperty("user_id").GetGuid());
            // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`i + 1`,
            // `players[i].GetProperty(”player_order_no”).GetInt32()`); pengujian gagal jika keduanya berbeda dalam AssertInitialState.
            Assert.Equal(i + 1, players[i].GetProperty("player_order_no").GetInt32());
            // Menyiapkan variabel lokal `initialIngredient` untuk nilai awal bahan dengan pemeriksaan hasil dengan `Assert.Single` menggunakan
            // `players[i].GetProperty(”bahan”).EnumerateArray()`; ketidaksesuaian dengan ekspektasi membuat pengujian gagal. Tipe variabel disimpulkan dari
            // ekspresi nilai awal.
            var initialIngredient = Assert.Single(players[i].GetProperty("bahan").EnumerateArray());
            // Menyiapkan variabel lokal `ingredientName` untuk nilai bahan nama dengan `initialIngredient.GetProperty(”nama”).GetString()` dengan penegasan
            // non-null untuk analisis compiler; operator ! tidak menambah pemeriksaan saat runtime. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var ingredientName = initialIngredient.GetProperty("nama").GetString()!;
            // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`1`,
            // `initialIngredient.GetProperty(”jumlah”).GetInt32()`); pengujian gagal jika keduanya berbeda dalam AssertInitialState.
            Assert.Equal(1, initialIngredient.GetProperty("jumlah").GetInt32());
            // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`20 - ingredientPrices[ingredientName]`,
            // `players[i].GetProperty(”coins”).GetInt32()`); pengujian gagal jika keduanya berbeda dalam AssertInitialState.
            Assert.Equal(20 - ingredientPrices[ingredientName], players[i].GetProperty("coins").GetInt32());
            // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`0`,
            // `players[i].GetProperty(”happiness”).GetInt32()`); pengujian gagal jika keduanya berbeda dalam AssertInitialState.
            Assert.Equal(0, players[i].GetProperty("happiness").GetInt32());
            // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`0`,
            // `players[i].GetProperty(”saving”).GetInt32()`); pengujian gagal jika keduanya berbeda dalam AssertInitialState.
            Assert.Equal(0, players[i].GetProperty("saving").GetInt32());
            // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`0`,
            // `players[i].GetProperty(”actionCounters”).GetArrayLength()`); pengujian gagal jika keduanya berbeda dalam AssertInitialState.
            Assert.Equal(0, players[i].GetProperty("actionCounters").GetArrayLength());
            // Menyiapkan variabel lokal `targetKebutuhan` untuk nilai target kebutuhan dengan mematerialisasi urutan
            // `players[i].GetProperty(”targetKebutuhan”).EnumerateArray()` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel
            // disimpulkan dari ekspresi nilai awal.
            var targetKebutuhan = players[i].GetProperty("targetKebutuhan").EnumerateArray().ToList();
            // Menyiapkan variabel lokal `mission` untuk nilai misi dengan pemeriksaan hasil dengan `Assert.Single` menggunakan `targetKebutuhan`;
            // ketidaksesuaian dengan ekspektasi membuat pengujian gagal. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var mission = Assert.Single(targetKebutuhan);
            // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `mission.GetProperty(”id”).GetString()!`,
            // `expectedMissionIds` dalam AssertInitialState.
            Assert.Contains(mission.GetProperty("id").GetString()!, expectedMissionIds);
            // Menjalankan pemeriksaan bahwa `players[i].TryGetProperty(”questProgress”, out _)` bernilai salah; pengujian gagal jika kondisi justru terpenuhi
            // dalam AssertInitialState.
            Assert.False(players[i].TryGetProperty("questProgress", out _));
            // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`0`,
            // `players[i].GetProperty(”totalDonasi”).GetInt32()`); pengujian gagal jika keduanya berbeda dalam AssertInitialState.
            Assert.Equal(0, players[i].GetProperty("totalDonasi").GetInt32());
        // Menutup scope loop dengan syarat `i < playerCount`; bagian berikut berada di luar batas blok tersebut dalam AssertInitialState.
        }
    // Menutup scope metode AssertInitialState; bagian berikut berada di luar batas blok tersebut dalam AssertInitialState.
    }

    // Mendefinisikan metode `RegisterInstructorAndGetTokenAsync` dengan hasil bertipe `Task<string>`; operasi ini menangani register instruktur dan get
    // token asinkron. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task.
    private async Task<string> RegisterInstructorAndGetTokenAsync()
    // Membuka scope metode RegisterInstructorAndGetTokenAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // RegisterInstructorAndGetTokenAsync.
    {
        // Menyiapkan variabel lokal `suffix` untuk nilai suffix dengan `Guid.NewGuid().ToString(”N”)[..8]`, yaitu elemen koleksi yang dipilih melalui
        // indeks atau kunci tersebut. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var suffix = Guid.NewGuid().ToString("N")[..8];
        // Menyiapkan variabel lokal `response` untuk hasil respons yang akan dibaca atau dikirim kepada pemanggil dengan hasil operasi asinkron memanggil
        // `_client.PostAsJsonAsync` dengan `”/api/v1/auth/register”`, `new RegisterRequest($”session_instructor_{suffix}”, ”SessionInstructorPass!123”,
        // ”INSTRUCTOR”, null)`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai
        // awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var response = await _client.PostAsJsonAsync(
            // Meneruskan nilai literal `”/api/v1/auth/register”` sebagai argumen ke `_client.PostAsJsonAsync`.
            "/api/v1/auth/register",
            // Meneruskan objek baru bertipe `RegisterRequest` dengan argumen ($”session_instructor_{suffix}”, ”SessionInstructorPass!123”, ”INSTRUCTOR”, null)
            // sebagai argumen ke `_client.PostAsJsonAsync`; Meneruskan teks interpolasi `$”session_instructor_{suffix}”`; nilai ekspresi di dalam kurung
            // kurawal disisipkan saat program berjalan sebagai argumen ke konstruktor `RegisterRequest`; Meneruskan nilai literal `”SessionInstructorPass!123”`
            // sebagai argumen ke konstruktor `RegisterRequest`; Meneruskan nilai literal `”INSTRUCTOR”` sebagai argumen ke konstruktor `RegisterRequest`;
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen ke konstruktor `RegisterRequest`.
            new RegisterRequest($"session_instructor_{suffix}", "SessionInstructorPass!123", "INSTRUCTOR", null));
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.Created`,
        // `response.StatusCode`); pengujian gagal jika keduanya berbeda dalam RegisterInstructorAndGetTokenAsync.
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        // Menyiapkan variabel lokal `body` untuk nilai body dengan hasil operasi asinkron membaca tanpa argumen menjadi objek bertipe sesuai kontrak JSON
        // melalui `response.Content.ReadFromJsonAsync<RegisterResponse>`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var body = await response.Content.ReadFromJsonAsync<RegisterResponse>();
        // Menjalankan pemeriksaan NotNull atas `body` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam RegisterInstructorAndGetTokenAsync.
        Assert.NotNull(body);
        // Mengembalikan `body.AccessToken` (nilai akses token) kepada pemanggil dalam RegisterInstructorAndGetTokenAsync; eksekusi jalur ini selesai
        // setelah nilai hasil ditentukan.
        return body.AccessToken;
    // Menutup scope metode RegisterInstructorAndGetTokenAsync; bagian berikut berada di luar batas blok tersebut dalam
    // RegisterInstructorAndGetTokenAsync.
    }

    // Mendefinisikan metode `GetDefaultRulesetAsync` dengan hasil bertipe `Task<(Guid RulesetVersionId, RulesetDefinitionDto Definition)>`; operasi ini
    // menangani get bawaan aturan asinkron. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task.
    // Masukan: Parameter `mode` bertipe `string` membawa mode permainan yang menentukan kelompok aturan yang digunakan; Parameter `accessToken` bertipe
    // `string` membawa nilai akses token.
    private async Task<(Guid RulesetVersionId, RulesetDefinitionDto Definition)> GetDefaultRulesetAsync(
        // Parameter `mode` bertipe `string` membawa mode permainan yang menentukan kelompok aturan yang digunakan.
        string mode,
        // Parameter `accessToken` bertipe `string` membawa nilai akses token.
        string accessToken)
    // Membuka scope metode GetDefaultRulesetAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam GetDefaultRulesetAsync.
    {
        // Menyiapkan variabel lokal `response` untuk hasil respons yang akan dibaca atau dikirim kepada pemanggil dengan hasil operasi asinkron memanggil
        // `SendJsonAsync` dengan `HttpMethod.Get`, `$”/api/v1/rulesets/components/defaults?mode={mode}”`, `null`, `accessToken`; await menunggu hasil tanpa
        // memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis
        // saat scope berakhir.
        using var response = await SendJsonAsync(
            // Meneruskan `HttpMethod.Get` (nilai get) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Get,
            // Meneruskan teks interpolasi `$”/api/v1/rulesets/components/defaults?mode={mode}”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program
            // berjalan sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/rulesets/components/defaults?mode={mode}",
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen ke `SendJsonAsync`.
            null,
            // Meneruskan `accessToken` (nilai akses token) sebagai argumen ke `SendJsonAsync`.
            accessToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`, `response.StatusCode`);
        // pengujian gagal jika keduanya berbeda dalam GetDefaultRulesetAsync.
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Menyiapkan variabel lokal `body` untuk nilai body dengan hasil operasi asinkron membaca tanpa argumen menjadi objek bertipe sesuai kontrak JSON
        // melalui `response.Content.ReadFromJsonAsync<DefaultRulesetComponentsResponse>`; await menunggu hasil tanpa memblokir thread selama operasi belum
        // selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var body = await response.Content.ReadFromJsonAsync<DefaultRulesetComponentsResponse>();
        // Menjalankan pemeriksaan NotNull atas `body` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam GetDefaultRulesetAsync.
        Assert.NotNull(body);
        // Menyiapkan variabel lokal `selected` untuk nilai selected dengan pemeriksaan hasil dengan `Assert.Single` menggunakan `body.Items`, `item =>
        // string.Equals(item.Mode, mode, StringComparison.OrdinalIgnoreCase)`; ketidaksesuaian dengan ekspektasi membuat pengujian gagal. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var selected = Assert.Single(body.Items, item => string.Equals(item.Mode, mode, StringComparison.OrdinalIgnoreCase));
        // Menjalankan pemeriksaan NotNull atas `selected.Definition` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // GetDefaultRulesetAsync.
        Assert.NotNull(selected.Definition);
        // Mengembalikan tuple yang membawa bagian 1: selected.RulesetVersionId; bagian 2: selected.Definition! kepada pemanggil dalam
        // GetDefaultRulesetAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return (selected.RulesetVersionId, selected.Definition!);
    // Menutup scope metode GetDefaultRulesetAsync; bagian berikut berada di luar batas blok tersebut dalam GetDefaultRulesetAsync.
    }

    // Mendefinisikan metode `CreateStartedSessionAsync` dengan hasil bertipe `Task<(Guid SessionId, JsonElement State)>`; operasi ini menangani create
    // started sesi asinkron. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan:
    // Parameter `accessToken` bertipe `string` membawa nilai akses token; Parameter `playerNames` bertipe `string[]` membawa nilai pemain nama.
    private async Task<(Guid SessionId, JsonElement State)> CreateStartedSessionAsync(
        // Parameter `accessToken` bertipe `string` membawa nilai akses token.
        string accessToken,
        // Parameter `playerNames` bertipe `string[]` membawa nilai pemain nama.
        string[] playerNames)
    // Membuka scope metode CreateStartedSessionAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CreateStartedSessionAsync.
    {
        // Menyiapkan variabel lokal `prepared` untuk nilai prepared dengan hasil operasi asinkron memanggil `CreateSessionWithPlayersAsync` dengan
        // `accessToken`, `playerNames`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi
        // nilai awal.
        var prepared = await CreateSessionWithPlayersAsync(accessToken, playerNames);
        // Menyiapkan variabel lokal `saveSetupResponse` untuk nilai save setup respons dengan hasil operasi asinkron memanggil
        // `SessionSetupTestHelper.SaveAsync` dengan `_client`, `accessToken`, `prepared.SessionId`, `prepared.Definition`,
        // `TestContext.Current.CancellationToken`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari
        // ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var saveSetupResponse = await SessionSetupTestHelper.SaveAsync(
            // Meneruskan `_client` (nilai client) sebagai argumen ke `SessionSetupTestHelper.SaveAsync`.
            _client,
            // Meneruskan `accessToken` (nilai akses token) sebagai argumen ke `SessionSetupTestHelper.SaveAsync`.
            accessToken,
            // Meneruskan `prepared.SessionId` (identitas unik sesi permainan yang menjadi batas data operasi ini) sebagai argumen ke
            // `SessionSetupTestHelper.SaveAsync`.
            prepared.SessionId,
            // Meneruskan `prepared.Definition` (definisi terstruktur komponen serta parameter aturan permainan) sebagai argumen ke
            // `SessionSetupTestHelper.SaveAsync`.
            prepared.Definition,
            // Meneruskan `TestContext.Current.CancellationToken` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
            // aplikasi berhenti) sebagai argumen ke `SessionSetupTestHelper.SaveAsync`.
            TestContext.Current.CancellationToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.Created`,
        // `saveSetupResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam CreateStartedSessionAsync.
        Assert.Equal(HttpStatusCode.Created, saveSetupResponse.StatusCode);

        // Menyiapkan variabel lokal `startResponse` untuk nilai start respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Post`, `$”/api/v1/sessions/{prepared.SessionId}/start”`, `null`, `accessToken`; await menunggu hasil tanpa memblokir thread selama
        // operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var startResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Post` (nilai post) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Post,
            // Meneruskan teks interpolasi `$”/api/v1/sessions/{prepared.SessionId}/start”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program
            // berjalan sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/sessions/{prepared.SessionId}/start",
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen ke `SendJsonAsync`.
            null,
            // Meneruskan `accessToken` (nilai akses token) sebagai argumen ke `SendJsonAsync`.
            accessToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`,
        // `startResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam CreateStartedSessionAsync.
        Assert.Equal(HttpStatusCode.OK, startResponse.StatusCode);

        // Menyiapkan variabel lokal `stateResponse` untuk nilai keadaan respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Get`, `$”/api/v1/sessions/{prepared.SessionId}/state”`, `null`, `accessToken`; await menunggu hasil tanpa memblokir thread selama
        // operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var stateResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Get` (nilai get) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Get,
            // Meneruskan teks interpolasi `$”/api/v1/sessions/{prepared.SessionId}/state”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program
            // berjalan sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/sessions/{prepared.SessionId}/state",
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen ke `SendJsonAsync`.
            null,
            // Meneruskan `accessToken` (nilai akses token) sebagai argumen ke `SendJsonAsync`.
            accessToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`,
        // `stateResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam CreateStartedSessionAsync.
        Assert.Equal(HttpStatusCode.OK, stateResponse.StatusCode);
        // Menyiapkan variabel lokal `state` untuk keadaan permainan yang menjadi sumber atau hasil pembaruan dengan hasil operasi asinkron memanggil
        // `ReadJsonAsync` dengan `stateResponse`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari
        // ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var state = await ReadJsonAsync(stateResponse);
        // Mengembalikan tuple yang membawa bagian 1: prepared.SessionId; bagian 2: state.RootElement.Clone() kepada pemanggil dalam
        // CreateStartedSessionAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return (prepared.SessionId, state.RootElement.Clone());
    // Menutup scope metode CreateStartedSessionAsync; bagian berikut berada di luar batas blok tersebut dalam CreateStartedSessionAsync.
    }

    // Mendefinisikan metode `CreateSessionWithPlayersAsync` dengan hasil bertipe `Task<(Guid SessionId, RulesetDefinitionDto Definition)>`; operasi ini
    // menangani create sesi dengan pemain asinkron. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui
    // Task. Masukan: Parameter `accessToken` bertipe `string` membawa nilai akses token; Parameter `playerNames` bertipe `string[]` membawa nilai
    // pemain nama.
    private async Task<(Guid SessionId, RulesetDefinitionDto Definition)> CreateSessionWithPlayersAsync(
        // Parameter `accessToken` bertipe `string` membawa nilai akses token.
        string accessToken,
        // Parameter `playerNames` bertipe `string[]` membawa nilai pemain nama.
        string[] playerNames)
    // Membuka scope metode CreateSessionWithPlayersAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // CreateSessionWithPlayersAsync.
    {
        // Menyiapkan variabel lokal `ruleset` untuk nilai aturan dengan hasil operasi asinkron memanggil `GetDefaultRulesetAsync` dengan `”MAHIR”`,
        // `accessToken`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var ruleset = await GetDefaultRulesetAsync("MAHIR", accessToken);
        // Menyiapkan variabel lokal `createSessionResponse` untuk nilai create sesi respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Post`, `”/api/v1/sessions”`, `new CreateSessionRequest($”Session State {Guid.NewGuid():N}”, ”MAHIR”, ruleset.RulesetVersionId)`,
        // `accessToken`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal;
        // using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var createSessionResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Post` (nilai post) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Post,
            // Meneruskan nilai literal `”/api/v1/sessions”` sebagai argumen ke `SendJsonAsync`.
            "/api/v1/sessions",
            // Meneruskan objek baru bertipe `CreateSessionRequest` dengan argumen ($”Session State {Guid.NewGuid():N}”, ”MAHIR”, ruleset.RulesetVersionId)
            // sebagai argumen ke `SendJsonAsync`; Meneruskan teks interpolasi `$”Session State {Guid.NewGuid():N}”`; nilai ekspresi di dalam kurung kurawal
            // disisipkan saat program berjalan sebagai argumen ke konstruktor `CreateSessionRequest`; Meneruskan nilai literal `”MAHIR”` sebagai argumen ke
            // konstruktor `CreateSessionRequest`; Meneruskan `ruleset.RulesetVersionId` (identitas versi aturan sehingga perhitungan memakai konfigurasi aturan
            // yang tepat) sebagai argumen ke konstruktor `CreateSessionRequest`.
            new CreateSessionRequest($"Session State {Guid.NewGuid():N}", "MAHIR", ruleset.RulesetVersionId),
            // Meneruskan `accessToken` (nilai akses token) sebagai argumen ke `SendJsonAsync`.
            accessToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.Created`,
        // `createSessionResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam CreateSessionWithPlayersAsync.
        Assert.Equal(HttpStatusCode.Created, createSessionResponse.StatusCode);
        // Menyiapkan variabel lokal `session` untuk nilai sesi dengan hasil operasi asinkron membaca tanpa argumen menjadi objek bertipe sesuai kontrak
        // JSON melalui `createSessionResponse.Content.ReadFromJsonAsync<CreateSessionResponse>`; await menunggu hasil tanpa memblokir thread selama operasi
        // belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var session = await createSessionResponse.Content.ReadFromJsonAsync<CreateSessionResponse>();
        // Menjalankan pemeriksaan NotNull atas `session` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam CreateSessionWithPlayersAsync.
        Assert.NotNull(session);

        // Memulai loop dengan inisialisasi `var index = 0`, berjalan selama `index < playerNames.Length`, lalu memperbarui pencacah melalui `index++` dalam
        // CreateSessionWithPlayersAsync.
        for (var index = 0; index < playerNames.Length; index++)
        // Membuka scope loop dengan syarat `index < playerNames.Length`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // CreateSessionWithPlayersAsync.
        {
            // Menyiapkan variabel lokal `createPlayerResponse` untuk nilai create pemain respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
            // `HttpMethod.Post`, `”/api/v1/players”`, `new CreatePlayerRequest( playerNames[index], $”state_player_{Guid.NewGuid():N}”,
            // ”SessionStatePlayerPass!123”)`, `accessToken`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel
            // disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
            using var createPlayerResponse = await SendJsonAsync(
                // Meneruskan `HttpMethod.Post` (nilai post) sebagai argumen ke `SendJsonAsync`.
                HttpMethod.Post,
                // Meneruskan nilai literal `”/api/v1/players”` sebagai argumen ke `SendJsonAsync`.
                "/api/v1/players",
                // Meneruskan objek baru bertipe `CreatePlayerRequest` dengan argumen ( playerNames[index], $”state_player_{Guid.NewGuid():N}”,
                // ”SessionStatePlayerPass!123”) sebagai argumen ke `SendJsonAsync`.
                new CreatePlayerRequest(
                    // Meneruskan `playerNames[index]`, yaitu elemen koleksi yang dipilih melalui indeks atau kunci tersebut sebagai argumen ke konstruktor
                    // `CreatePlayerRequest`; Meneruskan `index` (nilai index) sebagai argumen ke konstruktor `CreatePlayerRequest`.
                    playerNames[index],
                    // Meneruskan teks interpolasi `$”state_player_{Guid.NewGuid():N}”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan sebagai
                    // argumen ke konstruktor `CreatePlayerRequest`.
                    $"state_player_{Guid.NewGuid():N}",
                    // Meneruskan nilai literal `”SessionStatePlayerPass!123”` sebagai argumen ke konstruktor `CreatePlayerRequest`.
                    "SessionStatePlayerPass!123"),
                // Meneruskan `accessToken` (nilai akses token) sebagai argumen ke `SendJsonAsync`.
                accessToken);
            // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.Created`,
            // `createPlayerResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam CreateSessionWithPlayersAsync.
            Assert.Equal(HttpStatusCode.Created, createPlayerResponse.StatusCode);
            // Menyiapkan variabel lokal `player` untuk nilai pemain dengan hasil operasi asinkron membaca tanpa argumen menjadi objek bertipe sesuai kontrak
            // JSON melalui `createPlayerResponse.Content.ReadFromJsonAsync<PlayerResponse>`; await menunggu hasil tanpa memblokir thread selama operasi belum
            // selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var player = await createPlayerResponse.Content.ReadFromJsonAsync<PlayerResponse>();
            // Menjalankan pemeriksaan NotNull atas `player` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam CreateSessionWithPlayersAsync.
            Assert.NotNull(player);

            // Menyiapkan variabel lokal `addPlayerResponse` untuk nilai add pemain respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
            // `HttpMethod.Post`, `$”/api/v1/sessions/{session.SessionId}/players”`, `new AddSessionPlayerRequest(player.UserId, null, index + 1)`,
            // `accessToken`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal;
            // using memastikan sumber daya dilepas otomatis saat scope berakhir.
            using var addPlayerResponse = await SendJsonAsync(
                // Meneruskan `HttpMethod.Post` (nilai post) sebagai argumen ke `SendJsonAsync`.
                HttpMethod.Post,
                // Meneruskan teks interpolasi `$”/api/v1/sessions/{session.SessionId}/players”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program
                // berjalan sebagai argumen ke `SendJsonAsync`.
                $"/api/v1/sessions/{session.SessionId}/players",
                // Meneruskan objek baru bertipe `AddSessionPlayerRequest` dengan argumen (player.UserId, null, index + 1) sebagai argumen ke `SendJsonAsync`;
                // Meneruskan `player.UserId` (identitas akun pengguna yang datanya sedang diproses) sebagai argumen ke konstruktor `AddSessionPlayerRequest`;
                // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen ke konstruktor `AddSessionPlayerRequest`; Meneruskan penjumlahan/penggabungan
                // antara `index` dan `1` sebagai argumen ke konstruktor `AddSessionPlayerRequest`.
                new AddSessionPlayerRequest(player.UserId, null, index + 1),
                // Meneruskan `accessToken` (nilai akses token) sebagai argumen ke `SendJsonAsync`.
                accessToken);
            // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`,
            // `addPlayerResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam CreateSessionWithPlayersAsync.
            Assert.Equal(HttpStatusCode.OK, addPlayerResponse.StatusCode);
        // Menutup scope loop dengan syarat `index < playerNames.Length`; bagian berikut berada di luar batas blok tersebut dalam
        // CreateSessionWithPlayersAsync.
        }

        // Mengembalikan tuple yang membawa bagian 1: session.SessionId; bagian 2: ruleset.Definition kepada pemanggil dalam CreateSessionWithPlayersAsync;
        // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return (session.SessionId, ruleset.Definition);
    // Menutup scope metode CreateSessionWithPlayersAsync; bagian berikut berada di luar batas blok tersebut dalam CreateSessionWithPlayersAsync.
    }

    // Mendefinisikan metode `SendJsonAsync` dengan hasil bertipe `Task<HttpResponseMessage>`; operasi ini menangani send JSON asinkron. async
    // memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `method` bertipe
    // `HttpMethod` membawa nilai method; Parameter `path` bertipe `string` membawa nilai path; Parameter `body` bertipe `object?` membawa nilai body;
    // nilai null diizinkan ketika data opsional belum tersedia; Parameter `accessToken` bertipe `string` membawa nilai akses token.
    private async Task<HttpResponseMessage> SendJsonAsync(
        // Parameter `method` bertipe `HttpMethod` membawa nilai method.
        HttpMethod method,
        // Parameter `path` bertipe `string` membawa nilai path.
        string path,
        // Parameter `body` bertipe `object?` membawa nilai body; nilai null diizinkan ketika data opsional belum tersedia.
        object? body,
        // Parameter `accessToken` bertipe `string` membawa nilai akses token.
        string accessToken)
    // Membuka scope metode SendJsonAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam SendJsonAsync.
    {
        // Menyiapkan variabel lokal `request` untuk data masukan permintaan yang akan divalidasi atau diteruskan ke layanan dengan objek baru bertipe
        // `HttpRequestMessage` dengan argumen (method, path). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var request = new HttpRequestMessage(method, path);
        // Memperbarui `request.Headers.Authorization` menggunakan objek baru bertipe `AuthenticationHeaderValue` dengan argumen (”Bearer”, accessToken)
        // dalam SendJsonAsync.
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        // Memeriksa hasil pencocokan `body` dengan pola `not null`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam SendJsonAsync.
        if (body is not null)
        // Membuka scope cabang if untuk kondisi `body is not null`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam SendJsonAsync.
        {
            // Memperbarui `request.Content` menggunakan memanggil `JsonContent.Create` dengan `body` dalam SendJsonAsync.
            request.Content = JsonContent.Create(body);
        // Menutup scope cabang if untuk kondisi `body is not null`; bagian berikut berada di luar batas blok tersebut dalam SendJsonAsync.
        }

        // Mengembalikan hasil operasi asinkron memanggil `_client.SendAsync` dengan `request`; await menunggu hasil tanpa memblokir thread selama operasi
        // belum selesai kepada pemanggil dalam SendJsonAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return await _client.SendAsync(request);
    // Menutup scope metode SendJsonAsync; bagian berikut berada di luar batas blok tersebut dalam SendJsonAsync.
    }

    // Mendefinisikan metode `ReadJsonAsync` dengan hasil bertipe `Task<JsonDocument>`; operasi ini menangani read JSON asinkron. async memungkinkan
    // metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `response` bertipe `HttpResponseMessage`
    // membawa hasil respons yang akan dibaca atau dikirim kepada pemanggil.
    private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)
    // Membuka scope metode ReadJsonAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ReadJsonAsync.
    {
        // Menyiapkan variabel lokal `stream` untuk nilai stream dengan hasil operasi asinkron memanggil `response.Content.ReadAsStreamAsync` dengan tanpa
        // argumen; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal; using
        // memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var stream = await response.Content.ReadAsStreamAsync();
        // Mengembalikan hasil operasi asinkron memanggil `JsonDocument.ParseAsync` dengan `stream`; await menunggu hasil tanpa memblokir thread selama
        // operasi belum selesai kepada pemanggil dalam ReadJsonAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return await JsonDocument.ParseAsync(stream);
    // Menutup scope metode ReadJsonAsync; bagian berikut berada di luar batas blok tersebut dalam ReadJsonAsync.
    }
// Menutup scope tipe SessionStateApiIntegrationTests; bagian berikut berada di luar batas blok tersebut.
}
