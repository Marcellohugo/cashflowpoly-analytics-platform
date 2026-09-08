// Fungsi file: Memverifikasi perilaku API, database, atau domain melalui EventAnalyticsIntegrationTests.
// Mengimpor namespace `System.Net` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Net;
// Mengimpor namespace `System.Net.Http.Headers` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Net.Http.Headers;
// Mengimpor namespace `System.Net.Http.Json` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Net.Http.Json;
// Mengimpor namespace `System.Text.Json` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.Json;
// Mengimpor namespace `Cashflowpoly.Api.Contracts` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Contracts;
// Mengimpor namespace `Cashflowpoly.Api.Tests.Infrastructure` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Tests.Infrastructure;
// Mengimpor namespace `Xunit` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Xunit;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Tests` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Tests;

// menempatkan pengujian dalam koleksi fixture (”ApiIntegration”).
[Collection("ApiIntegration")]
// menerapkan metadata `Trait(”Category”, ”Integration”)` pada deklarasi berikut agar framework/compiler dapat mengenali pengaturannya.
[Trait("Category", "Integration")]
/// <summary>
/// Kelas pengujian integrasi yang memvalidasi alur lengkap ingest event gameplay,
/// perhitungan analitik sesi, riwayat transaksi, pengurutan pemain, dan pembatasan akses.
/// </summary>
// Mendefinisikan tipe class `EventAnalyticsIntegrationTests`; sealed mencegah tipe ini diturunkan lagi.
public sealed class EventAnalyticsIntegrationTests
// Membuka scope tipe EventAnalyticsIntegrationTests; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendeklarasikan field bertipe `HttpClient`: `_client` menyimpan nilai client. readonly membatasi penggantian referensi/nilai field pada deklarasi
    // atau konstruktor.
    private readonly HttpClient _client;

    /// <summary>
    /// Menginisialisasi instance pengujian dengan HttpClient dari fixture integrasi bersama.
    /// </summary>
    // Mendefinisikan konstruktor EventAnalyticsIntegrationTests yang menyiapkan objek dan menerima dependency/nilai awal dari pemanggil; parameter:
    // Parameter `fixture` bertipe `ApiIntegrationTestFixture` membawa nilai fixture.
    public EventAnalyticsIntegrationTests(ApiIntegrationTestFixture fixture)
    // Membuka scope konstruktor EventAnalyticsIntegrationTests; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // EventAnalyticsIntegrationTests.
    {
        // Memperbarui `_client` menggunakan `fixture.Client` (nilai client) dalam EventAnalyticsIntegrationTests.
        _client = fixture.Client;
    // Menutup scope konstruktor EventAnalyticsIntegrationTests; bagian berikut berada di luar batas blok tersebut dalam EventAnalyticsIntegrationTests.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    /// <summary>
    /// Memvalidasi alur lengkap: ingest dua event transaksi lalu memverifikasi hasil
    /// analitik sesi (cash in/out/net), data per pemain, riwayat transaksi, dan recompute.
    /// </summary>
    // Mendefinisikan metode `IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData` dengan hasil bertipe `Task`; operasi ini menangani ingest
    // event then analytics dan transactions return yang diharapkan data. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan
    // penyelesaian melalui Task.
    public async Task IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData()
    // Membuka scope metode IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData; pernyataan/deklarasi berikut berada di dalam batas blok ini
    // dalam IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
    {
        // Menyiapkan variabel lokal `suffix` untuk nilai suffix dengan `Guid.NewGuid().ToString(”N”)[..8]`, yaitu elemen koleksi yang dipilih melalui
        // indeks atau kunci tersebut. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var suffix = Guid.NewGuid().ToString("N")[..8];
        // Menyiapkan variabel lokal `instructorUsername` untuk nilai instruktur username dengan teks interpolasi `$”it_evt_instructor_{suffix}”`; nilai
        // ekspresi di dalam kurung kurawal disisipkan saat program berjalan. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var instructorUsername = $"it_evt_instructor_{suffix}";
        // Menyiapkan variabel lokal `instructorPassword` untuk nilai instruktur password dengan nilai literal `”IntegrationEventInstructorPass!123”`. Tipe
        // yang dipakai adalah `string`.
        const string instructorPassword = "IntegrationEventInstructorPass!123";
        // Menyiapkan variabel lokal `playerUsername` untuk nilai pemain username dengan teks interpolasi `$”it_evt_player_{suffix}”`; nilai ekspresi di
        // dalam kurung kurawal disisipkan saat program berjalan. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var playerUsername = $"it_evt_player_{suffix}";
        // Menyiapkan variabel lokal `playerPassword` untuk nilai pemain password dengan nilai literal `”IntegrationEventPlayerPass!123”`. Tipe yang dipakai
        // adalah `string`.
        const string playerPassword = "IntegrationEventPlayerPass!123";

        // Menyiapkan variabel lokal `instructorToken` untuk nilai instruktur token dengan `(await RegisterAsync(instructorUsername, instructorPassword,
        // ”INSTRUCTOR”)).AccessToken` (nilai akses token). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var instructorToken = (await RegisterAsync(instructorUsername, instructorPassword, "INSTRUCTOR")).AccessToken;

        // Menyiapkan variabel lokal `definition` untuk definisi terstruktur komponen serta parameter aturan permainan dengan memanggil
        // `BuildRulesetDefinition` dengan `20`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var definition = BuildRulesetDefinition(startingCash: 20);
        // Menyiapkan variabel lokal `createRulesetPayload` untuk nilai create aturan payload dengan objek anonim yang mengelompokkan name, description,
        // definition sebagai satu nilai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var createRulesetPayload = new
        // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
        {
            // Menggunakan `name` (nilai nama) sebagai bagian ekspresi yang sedang disusun dalam IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
            name = $"Ruleset Event IT {suffix}",
            // Menggunakan `description` (nilai description) sebagai bagian ekspresi yang sedang disusun dalam
            // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
            description = "Integration event analytics",
            // Menggunakan `definition` (definisi terstruktur komponen serta parameter aturan permainan) sebagai bagian ekspresi yang sedang disusun dalam
            // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
            definition
        // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
        // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
        };

        // Menyiapkan variabel lokal `createRulesetResponse` untuk nilai create aturan respons dengan hasil operasi asinkron memanggil `SendJsonAsync`
        // dengan `HttpMethod.Post`, `”/api/v1/rulesets”`, `createRulesetPayload`, `instructorToken`; await menunggu hasil tanpa memblokir thread selama
        // operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var createRulesetResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Post` (nilai post) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Post,
            // Meneruskan nilai literal `”/api/v1/rulesets”` sebagai argumen ke `SendJsonAsync`.
            "/api/v1/rulesets",
            // Meneruskan `createRulesetPayload` (nilai create aturan payload) sebagai argumen ke `SendJsonAsync`.
            createRulesetPayload,
            // Meneruskan `instructorToken` (nilai instruktur token) sebagai argumen ke `SendJsonAsync`.
            instructorToken);
        // Menjalankan pemeriksaan bahwa `createRulesetResponse.StatusCode == HttpStatusCode.Created`, `await
        // createRulesetResponse.Content.ReadAsStringAsync()` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
        Assert.True(
            // Meneruskan perbandingan kesamaan antara `createRulesetResponse.StatusCode` dan `HttpStatusCode.Created` sebagai argumen ke `Assert.True`.
            createRulesetResponse.StatusCode == HttpStatusCode.Created,
            // Meneruskan hasil operasi asinkron memanggil `createRulesetResponse.Content.ReadAsStringAsync` dengan tanpa argumen; await menunggu hasil tanpa
            // memblokir thread selama operasi belum selesai sebagai argumen ke `Assert.True`.
            await createRulesetResponse.Content.ReadAsStringAsync());

        // Menyiapkan variabel lokal `createdRuleset` untuk nilai created aturan dengan hasil operasi asinkron membaca tanpa argumen menjadi objek bertipe
        // sesuai kontrak JSON melalui `createRulesetResponse.Content.ReadFromJsonAsync<CreateRulesetResponse>`; await menunggu hasil tanpa memblokir thread
        // selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var createdRuleset = await createRulesetResponse.Content.ReadFromJsonAsync<CreateRulesetResponse>();
        // Menjalankan pemeriksaan NotNull atas `createdRuleset` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
        Assert.NotNull(createdRuleset);

        // Menyiapkan variabel lokal `updateRulesetPayload` untuk nilai update aturan payload dengan objek anonim yang mengelompokkan name, description,
        // definition sebagai satu nilai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var updateRulesetPayload = new
        // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
        {
            // Menggunakan `name` (nilai nama) sebagai bagian ekspresi yang sedang disusun dalam IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
            name = $"Ruleset Event IT {suffix} V2",
            // Menggunakan `description` (nilai description) sebagai bagian ekspresi yang sedang disusun dalam
            // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
            description = "Integration event analytics v2",
            // Menggunakan `definition` (definisi terstruktur komponen serta parameter aturan permainan) sebagai bagian ekspresi yang sedang disusun dalam
            // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
            definition = BuildRulesetDefinition(startingCash: 21)
        // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
        // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
        };
        // Menyiapkan variabel lokal `updateRulesetResponse` untuk nilai update aturan respons dengan hasil operasi asinkron memanggil `SendJsonAsync`
        // dengan `HttpMethod.Put`, `$”/api/v1/rulesets/{createdRuleset.RulesetId}”`, `updateRulesetPayload`, `instructorToken`; await menunggu hasil tanpa
        // memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var updateRulesetResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Put` (nilai put) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Put,
            // Meneruskan teks interpolasi `$”/api/v1/rulesets/{createdRuleset.RulesetId}”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program
            // berjalan sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/rulesets/{createdRuleset.RulesetId}",
            // Meneruskan `updateRulesetPayload` (nilai update aturan payload) sebagai argumen ke `SendJsonAsync`.
            updateRulesetPayload,
            // Meneruskan `instructorToken` (nilai instruktur token) sebagai argumen ke `SendJsonAsync`.
            instructorToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`,
        // `updateRulesetResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
        Assert.Equal(HttpStatusCode.OK, updateRulesetResponse.StatusCode);

        // Menyiapkan variabel lokal `updatedRuleset` untuk nilai updated aturan dengan hasil operasi asinkron membaca tanpa argumen menjadi objek bertipe
        // sesuai kontrak JSON melalui `updateRulesetResponse.Content.ReadFromJsonAsync<CreateRulesetResponse>`; await menunggu hasil tanpa memblokir thread
        // selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var updatedRuleset = await updateRulesetResponse.Content.ReadFromJsonAsync<CreateRulesetResponse>();
        // Menjalankan pemeriksaan NotNull atas `updatedRuleset` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
        Assert.NotNull(updatedRuleset);

        // Menyiapkan variabel lokal `activateRulesetResponse` untuk nilai activate aturan respons dengan hasil operasi asinkron memanggil `SendJsonAsync`
        // dengan `HttpMethod.Post`, `$”/api/v1/rulesets/{createdRuleset.RulesetId}/versions/{updatedRuleset.Version}/activate”`, `null`, `instructorToken`;
        // await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var activateRulesetResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Post` (nilai post) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Post,
            // Meneruskan teks interpolasi `$”/api/v1/rulesets/{createdRuleset.RulesetId}/versions/{updatedRuleset.Version}/activate”`; nilai ekspresi di dalam
            // kurung kurawal disisipkan saat program berjalan sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/rulesets/{createdRuleset.RulesetId}/versions/{updatedRuleset.Version}/activate",
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen bernama `body`.
            body: null,
            // Meneruskan `instructorToken` (nilai instruktur token) sebagai argumen ke `SendJsonAsync`.
            instructorToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`,
        // `activateRulesetResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
        Assert.Equal(HttpStatusCode.OK, activateRulesetResponse.StatusCode);

        // Menyiapkan variabel lokal `createSessionPayload` untuk nilai create sesi payload dengan objek anonim yang mengelompokkan session_name, mode,
        // ruleset_version_id sebagai satu nilai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var createSessionPayload = new
        // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
        {
            // Menggunakan `session_name` (nilai sesi nama) sebagai bagian ekspresi yang sedang disusun dalam
            // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
            session_name = $"Session Event IT {suffix}",
            // Menggunakan `mode` (mode permainan yang menentukan kelompok aturan yang digunakan) sebagai bagian ekspresi yang sedang disusun dalam
            // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
            mode = "PEMULA",
            // Menggunakan `ruleset_version_id` (nilai aturan versi identitas) sebagai bagian ekspresi yang sedang disusun dalam
            // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
            ruleset_version_id = updatedRuleset.RulesetVersionId
        // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
        // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
        };

        // Menyiapkan variabel lokal `createSessionResponse` untuk nilai create sesi respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Post`, `”/api/v1/sessions”`, `createSessionPayload`, `instructorToken`; await menunggu hasil tanpa memblokir thread selama operasi
        // belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var createSessionResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Post` (nilai post) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Post,
            // Meneruskan nilai literal `”/api/v1/sessions”` sebagai argumen ke `SendJsonAsync`.
            "/api/v1/sessions",
            // Meneruskan `createSessionPayload` (nilai create sesi payload) sebagai argumen ke `SendJsonAsync`.
            createSessionPayload,
            // Meneruskan `instructorToken` (nilai instruktur token) sebagai argumen ke `SendJsonAsync`.
            instructorToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.Created`,
        // `createSessionResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
        Assert.Equal(HttpStatusCode.Created, createSessionResponse.StatusCode);

        // Menyiapkan variabel lokal `createdSession` untuk nilai created sesi dengan hasil operasi asinkron membaca tanpa argumen menjadi objek bertipe
        // sesuai kontrak JSON melalui `createSessionResponse.Content.ReadFromJsonAsync<CreateSessionResponse>`; await menunggu hasil tanpa memblokir thread
        // selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var createdSession = await createSessionResponse.Content.ReadFromJsonAsync<CreateSessionResponse>();
        // Menjalankan pemeriksaan NotNull atas `createdSession` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
        Assert.NotNull(createdSession);

        // Menyiapkan variabel lokal `createPlayerPayload` untuk nilai create pemain payload dengan objek anonim yang mengelompokkan display_name, username,
        // password sebagai satu nilai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var createPlayerPayload = new
        // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
        {
            // Menggunakan `display_name` (nilai display nama) sebagai bagian ekspresi yang sedang disusun dalam
            // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
            display_name = $"Player Event {suffix}",
            // Menggunakan `username` (nama akun yang dipakai saat autentikasi) sebagai bagian ekspresi yang sedang disusun dalam
            // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
            username = playerUsername,
            // Menggunakan `password` (kata sandi masukan yang diperiksa sesuai kebijakan autentikasi) sebagai bagian ekspresi yang sedang disusun dalam
            // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
            password = playerPassword
        // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
        // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
        };

        // Menyiapkan variabel lokal `createPlayerResponse` untuk nilai create pemain respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Post`, `”/api/v1/players”`, `createPlayerPayload`, `instructorToken`; await menunggu hasil tanpa memblokir thread selama operasi
        // belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var createPlayerResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Post` (nilai post) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Post,
            // Meneruskan nilai literal `”/api/v1/players”` sebagai argumen ke `SendJsonAsync`.
            "/api/v1/players",
            // Meneruskan `createPlayerPayload` (nilai create pemain payload) sebagai argumen ke `SendJsonAsync`.
            createPlayerPayload,
            // Meneruskan `instructorToken` (nilai instruktur token) sebagai argumen ke `SendJsonAsync`.
            instructorToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.Created`,
        // `createPlayerResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
        Assert.Equal(HttpStatusCode.Created, createPlayerResponse.StatusCode);

        // Menyiapkan variabel lokal `createdPlayerBody` untuk nilai created pemain body dengan hasil operasi asinkron memanggil `ReadJsonAsync` dengan
        // `createPlayerResponse`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai
        // awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var createdPlayerBody = await ReadJsonAsync(createPlayerResponse);
        // Menyiapkan variabel lokal `createdUserId` untuk nilai created pengguna identitas dengan memanggil
        // `createdPlayerBody.RootElement.GetProperty(”user_id”).GetGuid` dengan tanpa argumen. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var createdUserId = createdPlayerBody.RootElement.GetProperty("user_id").GetGuid();
        // Menjalankan pemeriksaan hasil dengan `Assert.NotEqual` menggunakan `Guid.Empty`, `createdUserId`; ketidaksesuaian dengan ekspektasi membuat
        // pengujian gagal dalam IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
        Assert.NotEqual(Guid.Empty, createdUserId);

        // Menyiapkan variabel lokal `addPlayerPayload` untuk nilai add pemain payload dengan objek anonim yang mengelompokkan user_id, player_order_no
        // sebagai satu nilai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var addPlayerPayload = new
        // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
        {
            // Menggunakan `user_id` (nilai pengguna identitas) sebagai bagian ekspresi yang sedang disusun dalam
            // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
            user_id = createdUserId,
            // Menggunakan `player_order_no` (nilai pemain urutan/pesanan no) sebagai bagian ekspresi yang sedang disusun dalam
            // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
            player_order_no = 1
        // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
        // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
        };

        // Menyiapkan variabel lokal `addPlayerResponse` untuk nilai add pemain respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Post`, `$”/api/v1/sessions/{createdSession.SessionId}/players”`, `addPlayerPayload`, `instructorToken`; await menunggu hasil tanpa
        // memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var addPlayerResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Post` (nilai post) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Post,
            // Meneruskan teks interpolasi `$”/api/v1/sessions/{createdSession.SessionId}/players”`; nilai ekspresi di dalam kurung kurawal disisipkan saat
            // program berjalan sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/sessions/{createdSession.SessionId}/players",
            // Meneruskan `addPlayerPayload` (nilai add pemain payload) sebagai argumen ke `SendJsonAsync`.
            addPlayerPayload,
            // Meneruskan `instructorToken` (nilai instruktur token) sebagai argumen ke `SendJsonAsync`.
            instructorToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`,
        // `addPlayerResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
        Assert.Equal(HttpStatusCode.OK, addPlayerResponse.StatusCode);

        // Memulai loop dengan inisialisasi `var i = 2`, berjalan selama `i <= 3`, lalu memperbarui pencacah melalui `i++` dalam
        // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
        for (var i = 2; i <= 3; i++)
        // Membuka scope loop dengan syarat `i <= 3`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
        {
            // Menyiapkan variabel lokal `extraPlayerResponse` untuk nilai extra pemain respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
            // `HttpMethod.Post`, `”/api/v1/players”`, `new { display_name = $”Player Event Extra {i} {suffix}”, username = $”it_evt_player_extra_{i}_{suffix}”,
            // password = ”IntegrationEventExtraPlayerPass!123” }`, `instructorToken`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai.
            // Tipe variabel disimpulkan dari ekspresi nilai awal.
            var extraPlayerResponse = await SendJsonAsync(
                // Meneruskan `HttpMethod.Post` (nilai post) sebagai argumen ke `SendJsonAsync`.
                HttpMethod.Post,
                // Meneruskan nilai literal `”/api/v1/players”` sebagai argumen ke `SendJsonAsync`.
                "/api/v1/players",
                // Meneruskan objek anonim yang mengelompokkan display_name, username, password sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                new
                // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
                {
                    // Meneruskan objek anonim yang mengelompokkan display_name, username, password sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                    display_name = $"Player Event Extra {i} {suffix}",
                    // Meneruskan objek anonim yang mengelompokkan display_name, username, password sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                    username = $"it_evt_player_extra_{i}_{suffix}",
                    // Meneruskan objek anonim yang mengelompokkan display_name, username, password sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                    password = "IntegrationEventExtraPlayerPass!123"
                // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
                // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
                },
                // Meneruskan `instructorToken` (nilai instruktur token) sebagai argumen ke `SendJsonAsync`.
                instructorToken);
            // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.Created`,
            // `extraPlayerResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
            Assert.Equal(HttpStatusCode.Created, extraPlayerResponse.StatusCode);

            // Menyiapkan variabel lokal `extraPlayer` untuk nilai extra pemain dengan hasil operasi asinkron membaca tanpa argumen menjadi objek bertipe sesuai
            // kontrak JSON melalui `extraPlayerResponse.Content.ReadFromJsonAsync<PlayerResponse>`; await menunggu hasil tanpa memblokir thread selama operasi
            // belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var extraPlayer = await extraPlayerResponse.Content.ReadFromJsonAsync<PlayerResponse>();
            // Menjalankan pemeriksaan NotNull atas `extraPlayer` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
            // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
            Assert.NotNull(extraPlayer);

            // Menyiapkan variabel lokal `addExtraPlayerResponse` untuk nilai add extra pemain respons dengan hasil operasi asinkron memanggil `SendJsonAsync`
            // dengan `HttpMethod.Post`, `$”/api/v1/sessions/{createdSession.SessionId}/players”`, `new { user_id = extraPlayer.UserId, player_order_no = i }`,
            // `instructorToken`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var addExtraPlayerResponse = await SendJsonAsync(
                // Meneruskan `HttpMethod.Post` (nilai post) sebagai argumen ke `SendJsonAsync`.
                HttpMethod.Post,
                // Meneruskan teks interpolasi `$”/api/v1/sessions/{createdSession.SessionId}/players”`; nilai ekspresi di dalam kurung kurawal disisipkan saat
                // program berjalan sebagai argumen ke `SendJsonAsync`.
                $"/api/v1/sessions/{createdSession.SessionId}/players",
                // Meneruskan objek anonim yang mengelompokkan user_id, player_order_no sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                new
                // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
                {
                    // Meneruskan objek anonim yang mengelompokkan user_id, player_order_no sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                    user_id = extraPlayer.UserId,
                    // Meneruskan objek anonim yang mengelompokkan user_id, player_order_no sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                    player_order_no = i
                // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
                // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
                },
                // Meneruskan `instructorToken` (nilai instruktur token) sebagai argumen ke `SendJsonAsync`.
                instructorToken);
            // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`,
            // `addExtraPlayerResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
            Assert.Equal(HttpStatusCode.OK, addExtraPlayerResponse.StatusCode);
        // Menutup scope loop dengan syarat `i <= 3`; bagian berikut berada di luar batas blok tersebut dalam
        // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
        }

        // Menyiapkan variabel lokal `saveSetupResponse` untuk nilai save setup respons dengan hasil operasi asinkron memanggil
        // `SessionSetupTestHelper.SaveAsync` dengan `_client`, `instructorToken`, `createdSession.SessionId`, `definition`,
        // `TestContext.Current.CancellationToken`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari
        // ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var saveSetupResponse = await SessionSetupTestHelper.SaveAsync(
            // Meneruskan `_client` (nilai client) sebagai argumen ke `SessionSetupTestHelper.SaveAsync`.
            _client,
            // Meneruskan `instructorToken` (nilai instruktur token) sebagai argumen ke `SessionSetupTestHelper.SaveAsync`.
            instructorToken,
            // Meneruskan `createdSession.SessionId` (identitas unik sesi permainan yang menjadi batas data operasi ini) sebagai argumen ke
            // `SessionSetupTestHelper.SaveAsync`.
            createdSession.SessionId,
            // Meneruskan `definition` (definisi terstruktur komponen serta parameter aturan permainan) sebagai argumen ke `SessionSetupTestHelper.SaveAsync`.
            definition,
            // Meneruskan `TestContext.Current.CancellationToken` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
            // aplikasi berhenti) sebagai argumen ke `SessionSetupTestHelper.SaveAsync`.
            TestContext.Current.CancellationToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.Created`,
        // `saveSetupResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
        Assert.Equal(HttpStatusCode.Created, saveSetupResponse.StatusCode);

        // Menyiapkan variabel lokal `startSessionResponse` untuk nilai start sesi respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Post`, `$”/api/v1/sessions/{createdSession.SessionId}/start”`, `null`, `instructorToken`; await menunggu hasil tanpa memblokir thread
        // selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var startSessionResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Post` (nilai post) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Post,
            // Meneruskan teks interpolasi `$”/api/v1/sessions/{createdSession.SessionId}/start”`; nilai ekspresi di dalam kurung kurawal disisipkan saat
            // program berjalan sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/sessions/{createdSession.SessionId}/start",
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen bernama `body`.
            body: null,
            // Meneruskan `instructorToken` (nilai instruktur token) sebagai argumen ke `SendJsonAsync`.
            instructorToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`,
        // `startSessionResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
        Assert.Equal(HttpStatusCode.OK, startSessionResponse.StatusCode);

        // Menyiapkan variabel lokal `rulesetDetailResponse` untuk nilai aturan detail respons dengan hasil operasi asinkron memanggil `SendJsonAsync`
        // dengan `HttpMethod.Get`, `$”/api/v1/rulesets/{createdRuleset.RulesetId}”`, `null`, `instructorToken`; await menunggu hasil tanpa memblokir thread
        // selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var rulesetDetailResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Get` (nilai get) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Get,
            // Meneruskan teks interpolasi `$”/api/v1/rulesets/{createdRuleset.RulesetId}”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program
            // berjalan sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/rulesets/{createdRuleset.RulesetId}",
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen bernama `body`.
            body: null,
            // Meneruskan `instructorToken` (nilai instruktur token) sebagai argumen ke `SendJsonAsync`.
            instructorToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`,
        // `rulesetDetailResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
        Assert.Equal(HttpStatusCode.OK, rulesetDetailResponse.StatusCode);

        // Menyiapkan variabel lokal `rulesetDetail` untuk nilai aturan detail dengan hasil operasi asinkron membaca tanpa argumen menjadi objek bertipe
        // sesuai kontrak JSON melalui `rulesetDetailResponse.Content.ReadFromJsonAsync<RulesetDetailResponse>`; await menunggu hasil tanpa memblokir thread
        // selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var rulesetDetail = await rulesetDetailResponse.Content.ReadFromJsonAsync<RulesetDetailResponse>();
        // Menjalankan pemeriksaan NotNull atas `rulesetDetail` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
        Assert.NotNull(rulesetDetail);

        // Menyiapkan variabel lokal `activeVersion` untuk nilai aktif versi dengan mengambil elemen pertama `rulesetDetail.Versions .Where(v =>
        // string.Equals(v.Status, ”ACTIVE”, StringComparison.OrdinalIgnoreCase)) .OrderByDescending(v => v.Version)` sesuai tanpa argumen; urutan tanpa
        // kecocokan menyebabkan exception. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var activeVersion = rulesetDetail.Versions
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(v => string.Equals(v.Status, ”ACTIVE”,
            // StringComparison.OrdinalIgnoreCase)) dalam IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData; token pada baris ini menyambungkan
            // bagian kode sebelum dan sesudahnya.
            .Where(v => string.Equals(v.Status, "ACTIVE", StringComparison.OrdinalIgnoreCase))
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .OrderByDescending(v => v.Version) dalam
            // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .OrderByDescending(v => v.Version)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .First(); dalam IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData;
            // token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .First();

        // Menyiapkan variabel lokal `nextSequence` untuk nilai next sequence dengan hasil operasi asinkron memanggil `GetNextSequenceNumberAsync` dengan
        // `createdSession.SessionId`, `instructorToken`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var nextSequence = await GetNextSequenceNumberAsync(createdSession.SessionId, instructorToken);
        // Menyiapkan variabel lokal `setupEventsResponse` untuk nilai setup event respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Get`, `$”/api/v1/sessions/{createdSession.SessionId}/events?limit=100”`, `null`, `instructorToken`; await menunggu hasil tanpa
        // memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis
        // saat scope berakhir.
        using var setupEventsResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Get` (nilai get) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Get,
            // Meneruskan teks interpolasi `$”/api/v1/sessions/{createdSession.SessionId}/events?limit=100”`; nilai ekspresi di dalam kurung kurawal disisipkan
            // saat program berjalan sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/sessions/{createdSession.SessionId}/events?limit=100",
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen ke `SendJsonAsync`.
            null,
            // Meneruskan `instructorToken` (nilai instruktur token) sebagai argumen ke `SendJsonAsync`.
            instructorToken);
        // Menyiapkan variabel lokal `setupEventsBody` untuk nilai setup event body dengan hasil operasi asinkron memanggil `ReadJsonAsync` dengan
        // `setupEventsResponse`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai
        // awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var setupEventsBody = await ReadJsonAsync(setupEventsResponse);
        // Menyiapkan variabel lokal `setupEvents` untuk nilai setup event dengan mematerialisasi urutan `setupEventsBody.RootElement.GetProperty(”items”)
        // .EnumerateArray() .Select(item => item.Clone())` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var setupEvents = setupEventsBody.RootElement.GetProperty("items")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .EnumerateArray() dalam
            // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .EnumerateArray()
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Select(item => item.Clone()) dalam
            // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Select(item => item.Clone())
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList(); dalam IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData;
            // token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .ToList();
        // Menyiapkan variabel lokal `actingUserId` untuk nilai acting pengguna identitas dengan memanggil `setupEvents .Single(item =>
        // item.GetProperty(”action_type”).GetString() == ”BagikanTieBreaker” && item.GetProperty(”payload”).GetProperty(”number”).GetInt32() == 1)
        // .GetProper...` dengan tanpa argumen. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var actingUserId = setupEvents
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Single(item => dalam
            // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Single(item =>
                // Meneruskan nilai literal `”action_type”` sebagai argumen ke `item.GetProperty`.
                item.GetProperty("action_type").GetString() == "BagikanTieBreaker" &&
                // Meneruskan nilai literal `”payload”` sebagai argumen ke `item.GetProperty`; Meneruskan nilai literal `”number”` sebagai argumen ke
                // `item.GetProperty(”payload”).GetProperty`.
                item.GetProperty("payload").GetProperty("number").GetInt32() == 1)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .GetProperty(”user_id”) dalam
            // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .GetProperty("user_id")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .GetGuid(); dalam
            // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .GetGuid();
        // Menyiapkan variabel lokal `setupIngredientEvents` untuk nilai setup bahan event dengan mematerialisasi urutan `setupEvents .Where(item =>
        // item.GetProperty(”action_type”).GetString() == ”SetupBahanAwal”)` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var setupIngredientEvents = setupEvents
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(item => item.GetProperty(”action_type”).GetString() == ”SetupBahanAwal”)
            // dalam IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Where(item => item.GetProperty("action_type").GetString() == "SetupBahanAwal")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList(); dalam IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData;
            // token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .ToList();
        // Menyiapkan variabel lokal `setupIngredientCost` untuk nilai setup bahan biaya dengan menjumlahkan nilai `setupIngredientEvents` berdasarkan `item
        // => item.GetProperty(”payload”).GetProperty(”amount”).GetInt32()`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var setupIngredientCost = setupIngredientEvents.Sum(item =>
            // Meneruskan nilai literal `”payload”` sebagai argumen ke `item.GetProperty`; Meneruskan nilai literal `”amount”` sebagai argumen ke
            // `item.GetProperty(”payload”).GetProperty`.
            item.GetProperty("payload").GetProperty("amount").GetInt32());
        // Menyiapkan variabel lokal `playerSetupIngredientCost` untuk nilai pemain setup bahan biaya dengan memanggil `setupIngredientEvents .Single(item
        // => item.GetProperty(”user_id”).GetGuid() == actingUserId) .GetProperty(”payload”) .GetProperty(”amount”) .GetInt32` dengan tanpa argumen. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var playerSetupIngredientCost = setupIngredientEvents
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Single(item => item.GetProperty(”user_id”).GetGuid() == actingUserId) dalam
            // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Single(item => item.GetProperty("user_id").GetGuid() == actingUserId)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .GetProperty(”payload”) dalam
            // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .GetProperty("payload")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .GetProperty(”amount”) dalam
            // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .GetProperty("amount")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .GetInt32(); dalam
            // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .GetInt32();
        // Menyiapkan variabel lokal `now` untuk nilai now dengan `DateTimeOffset.UtcNow`, yaitu waktu UTC saat operasi dilakukan. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var now = DateTimeOffset.UtcNow;
        // Menyiapkan variabel lokal `event1Payload` untuk nilai event 1 payload dengan objek anonim yang mengelompokkan event_id, session_id, user_id,
        // actor_type, turn_number, timestamp, day_index, weekday, action_slot, sequence_number, action_type, ruleset_version_id, payload sebagai satu
        // nilai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var event1Payload = new
        // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
        {
            // Menggunakan `event_id` (nilai event identitas) sebagai bagian ekspresi yang sedang disusun dalam
            // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
            event_id = Guid.NewGuid(),
            // Menggunakan `session_id` (nilai sesi identitas) sebagai bagian ekspresi yang sedang disusun dalam
            // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
            session_id = createdSession.SessionId,
            // Menggunakan `user_id` (nilai pengguna identitas) sebagai bagian ekspresi yang sedang disusun dalam
            // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
            user_id = actingUserId,
            // Menggunakan `actor_type` (nilai actor jenis) sebagai bagian ekspresi yang sedang disusun dalam
            // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
            actor_type = "PLAYER",
            // Menggunakan `turn_number` (nilai giliran number) sebagai bagian ekspresi yang sedang disusun dalam
            // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
            turn_number = 1,
            // Menggunakan `timestamp` (waktu kejadian yang menjaga urutan kronologis data) sebagai bagian ekspresi yang sedang disusun dalam
            // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
            timestamp = now.ToString("O"),
            // Menggunakan `day_index` (nilai hari index) sebagai bagian ekspresi yang sedang disusun dalam
            // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
            day_index = 1,
            // Menggunakan `weekday` (nilai weekday) sebagai bagian ekspresi yang sedang disusun dalam
            // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
            weekday = "MON",
            // Menggunakan `action_slot` (nilai aksi slot) sebagai bagian ekspresi yang sedang disusun dalam
            // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
            action_slot = 1,
            // Menggunakan `sequence_number` (nilai sequence number) sebagai bagian ekspresi yang sedang disusun dalam
            // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
            sequence_number = nextSequence,
            // Menggunakan `action_type` (nilai aksi jenis) sebagai bagian ekspresi yang sedang disusun dalam
            // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
            action_type = "KerjaLepas",
            // Menggunakan `ruleset_version_id` (nilai aturan versi identitas) sebagai bagian ekspresi yang sedang disusun dalam
            // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
            ruleset_version_id = activeVersion.RulesetVersionId,
            // Menggunakan `payload` (muatan detail event dalam format JSON) sebagai bagian ekspresi yang sedang disusun dalam
            // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
            payload = new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
            {
                // Menggunakan `amount` (nominal uang atau nilai transaksi yang dipakai dalam operasi) sebagai bagian ekspresi yang sedang disusun dalam
                // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
                amount = 1
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
            // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
            }
        // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
        // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
        };
        // Menyiapkan variabel lokal `event1Response` untuk nilai event 1 respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Post`, `”/api/v1/events”`, `event1Payload`, `instructorToken`; await menunggu hasil tanpa memblokir thread selama operasi belum
        // selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var event1Response = await SendJsonAsync(HttpMethod.Post, "/api/v1/events", event1Payload, instructorToken);
        // Menjalankan pemeriksaan bahwa `event1Response.StatusCode == HttpStatusCode.Created`, `await event1Response.Content.ReadAsStringAsync()` bernilai
        // benar; pengujian gagal jika kondisi tidak terpenuhi dalam IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
        Assert.True(
            // Meneruskan perbandingan kesamaan antara `event1Response.StatusCode` dan `HttpStatusCode.Created` sebagai argumen ke `Assert.True`.
            event1Response.StatusCode == HttpStatusCode.Created,
            // Meneruskan hasil operasi asinkron memanggil `event1Response.Content.ReadAsStringAsync` dengan tanpa argumen; await menunggu hasil tanpa memblokir
            // thread selama operasi belum selesai sebagai argumen ke `Assert.True`.
            await event1Response.Content.ReadAsStringAsync());

        // Menyiapkan variabel lokal `event2Payload` untuk nilai event 2 payload dengan objek anonim yang mengelompokkan event_id, session_id, user_id,
        // actor_type, turn_number, timestamp, day_index, weekday, action_slot, sequence_number, action_type, ruleset_version_id, payload sebagai satu
        // nilai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var event2Payload = new
        // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
        {
            // Menggunakan `event_id` (nilai event identitas) sebagai bagian ekspresi yang sedang disusun dalam
            // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
            event_id = Guid.NewGuid(),
            // Menggunakan `session_id` (nilai sesi identitas) sebagai bagian ekspresi yang sedang disusun dalam
            // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
            session_id = createdSession.SessionId,
            // Menggunakan `user_id` (nilai pengguna identitas) sebagai bagian ekspresi yang sedang disusun dalam
            // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
            user_id = actingUserId,
            // Menggunakan `actor_type` (nilai actor jenis) sebagai bagian ekspresi yang sedang disusun dalam
            // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
            actor_type = "PLAYER",
            // Menggunakan `turn_number` (nilai giliran number) sebagai bagian ekspresi yang sedang disusun dalam
            // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
            turn_number = 1,
            // Menggunakan `timestamp` (waktu kejadian yang menjaga urutan kronologis data) sebagai bagian ekspresi yang sedang disusun dalam
            // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
            timestamp = now.AddSeconds(1).ToString("O"),
            // Menggunakan `day_index` (nilai hari index) sebagai bagian ekspresi yang sedang disusun dalam
            // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
            day_index = 1,
            // Menggunakan `weekday` (nilai weekday) sebagai bagian ekspresi yang sedang disusun dalam
            // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
            weekday = "MON",
            // Menggunakan `action_slot` (nilai aksi slot) sebagai bagian ekspresi yang sedang disusun dalam
            // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
            action_slot = 2,
            // Menggunakan `sequence_number` (nilai sequence number) sebagai bagian ekspresi yang sedang disusun dalam
            // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
            sequence_number = nextSequence + 1,
            // Menggunakan `action_type` (nilai aksi jenis) sebagai bagian ekspresi yang sedang disusun dalam
            // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
            action_type = "Kebutuhan",
            // Menggunakan `ruleset_version_id` (nilai aturan versi identitas) sebagai bagian ekspresi yang sedang disusun dalam
            // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
            ruleset_version_id = activeVersion.RulesetVersionId,
            // Menggunakan `payload` (muatan detail event dalam format JSON) sebagai bagian ekspresi yang sedang disusun dalam
            // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
            payload = new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
            {
                // Menggunakan `card_id` (nilai kartu identitas) sebagai bagian ekspresi yang sedang disusun dalam
                // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
                card_id = "buku",
                // Menggunakan `amount` (nominal uang atau nilai transaksi yang dipakai dalam operasi) sebagai bagian ekspresi yang sedang disusun dalam
                // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
                amount = 3,
                // Menggunakan `points` (nilai poin) sebagai bagian ekspresi yang sedang disusun dalam
                // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
                points = 1,
                // Menggunakan `need_tier` (nilai kebutuhan tingkat) sebagai bagian ekspresi yang sedang disusun dalam
                // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
                need_tier = "primer"
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
            // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
            }
        // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
        // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
        };
        // Menyiapkan variabel lokal `event2Response` untuk nilai event 2 respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Post`, `”/api/v1/events”`, `event2Payload`, `instructorToken`; await menunggu hasil tanpa memblokir thread selama operasi belum
        // selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var event2Response = await SendJsonAsync(HttpMethod.Post, "/api/v1/events", event2Payload, instructorToken);
        // Menjalankan pemeriksaan bahwa `event2Response.StatusCode == HttpStatusCode.Created`, `await event2Response.Content.ReadAsStringAsync()` bernilai
        // benar; pengujian gagal jika kondisi tidak terpenuhi dalam IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
        Assert.True(
            // Meneruskan perbandingan kesamaan antara `event2Response.StatusCode` dan `HttpStatusCode.Created` sebagai argumen ke `Assert.True`.
            event2Response.StatusCode == HttpStatusCode.Created,
            // Meneruskan hasil operasi asinkron memanggil `event2Response.Content.ReadAsStringAsync` dengan tanpa argumen; await menunggu hasil tanpa memblokir
            // thread selama operasi belum selesai sebagai argumen ke `Assert.True`.
            await event2Response.Content.ReadAsStringAsync());

        // Menyiapkan variabel lokal `analyticsResponse` untuk nilai analytics respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Get`, `$”/api/v1/analytics/sessions/{createdSession.SessionId}”`, `null`, `instructorToken`; await menunggu hasil tanpa memblokir
        // thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var analyticsResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Get` (nilai get) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Get,
            // Meneruskan teks interpolasi `$”/api/v1/analytics/sessions/{createdSession.SessionId}”`; nilai ekspresi di dalam kurung kurawal disisipkan saat
            // program berjalan sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/analytics/sessions/{createdSession.SessionId}",
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen bernama `body`.
            body: null,
            // Meneruskan `instructorToken` (nilai instruktur token) sebagai argumen ke `SendJsonAsync`.
            instructorToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`,
        // `analyticsResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
        Assert.Equal(HttpStatusCode.OK, analyticsResponse.StatusCode);

        // Menyiapkan variabel lokal `analyticsBody` untuk nilai analytics body dengan hasil operasi asinkron memanggil `ReadJsonAsync` dengan
        // `analyticsResponse`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai
        // awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var analyticsBody = await ReadJsonAsync(analyticsResponse);
        // Menyiapkan variabel lokal `analyticsRoot` untuk nilai analytics root dengan `analyticsBody.RootElement` (nilai root element). Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var analyticsRoot = analyticsBody.RootElement;
        // Menyiapkan variabel lokal `summary` untuk nilai summary dengan memanggil `analyticsRoot.GetProperty` dengan `”summary”`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var summary = analyticsRoot.GetProperty("summary");
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`(int)(nextSequence + 2)`,
        // `summary.GetProperty(”event_count”).GetInt32()`); pengujian gagal jika keduanya berbeda dalam
        // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
        Assert.Equal((int)(nextSequence + 2), summary.GetProperty("event_count").GetInt32());
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`1d`,
        // `summary.GetProperty(”cash_in_total”).GetDouble()`, `6`); pengujian gagal jika keduanya berbeda dalam
        // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
        Assert.Equal(1d, summary.GetProperty("cash_in_total").GetDouble(), 6);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`setupIngredientCost + 3d`,
        // `summary.GetProperty(”cash_out_total”).GetDouble()`, `6`); pengujian gagal jika keduanya berbeda dalam
        // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
        Assert.Equal(setupIngredientCost + 3d, summary.GetProperty("cash_out_total").GetDouble(), 6);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`-(setupIngredientCost + 2d)`,
        // `summary.GetProperty(”cashflow_net_total”).GetDouble()`, `6`); pengujian gagal jika keduanya berbeda dalam
        // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
        Assert.Equal(-(setupIngredientCost + 2d), summary.GetProperty("cashflow_net_total").GetDouble(), 6);

        // Menyiapkan variabel lokal `byPlayer` untuk nilai berdasarkan pemain dengan mengambil tepat satu elemen `analyticsRoot.GetProperty(”by_player”)
        // .EnumerateArray()` sesuai `item => item.GetProperty(”user_id”).GetGuid() == actingUserId`; jumlah kecocokan selain satu menyebabkan exception.
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var byPlayer = analyticsRoot.GetProperty("by_player")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .EnumerateArray() dalam
            // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .EnumerateArray()
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Single(item => item.GetProperty(”user_id”).GetGuid() == actingUserId); dalam
            // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Single(item => item.GetProperty("user_id").GetGuid() == actingUserId);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`1d`,
        // `byPlayer.GetProperty(”cash_in_total”).GetDouble()`, `6`); pengujian gagal jika keduanya berbeda dalam
        // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
        Assert.Equal(1d, byPlayer.GetProperty("cash_in_total").GetDouble(), 6);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`playerSetupIngredientCost + 3d`,
        // `byPlayer.GetProperty(”cash_out_total”).GetDouble()`, `6`); pengujian gagal jika keduanya berbeda dalam
        // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
        Assert.Equal(playerSetupIngredientCost + 3d, byPlayer.GetProperty("cash_out_total").GetDouble(), 6);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`0`,
        // `byPlayer.GetProperty(”orders_completed_count”).GetInt32()`); pengujian gagal jika keduanya berbeda dalam
        // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
        Assert.Equal(0, byPlayer.GetProperty("orders_completed_count").GetInt32());
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`1`,
        // `byPlayer.GetProperty(”inventory_ingredient_total”).GetInt32()`); pengujian gagal jika keduanya berbeda dalam
        // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
        Assert.Equal(1, byPlayer.GetProperty("inventory_ingredient_total").GetInt32());
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`2`,
        // `byPlayer.GetProperty(”actions_used_total”).GetInt32()`); pengujian gagal jika keduanya berbeda dalam
        // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
        Assert.Equal(2, byPlayer.GetProperty("actions_used_total").GetInt32());
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`0d`,
        // `byPlayer.GetProperty(”fulfillment_diversity”).GetDouble()`, `6`); pengujian gagal jika keduanya berbeda dalam
        // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
        Assert.Equal(0d, byPlayer.GetProperty("fulfillment_diversity").GetDouble(), 6);
        // Menjalankan pemeriksaan bahwa `byPlayer.TryGetProperty(”rules_violations_count”, out _)` bernilai salah; pengujian gagal jika kondisi justru
        // terpenuhi dalam IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
        Assert.False(byPlayer.TryGetProperty("rules_violations_count", out _));

        // Menyiapkan variabel lokal `transactionsResponse` untuk nilai transactions respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Get`, `$”/api/v1/analytics/sessions/{createdSession.SessionId}/transactions?userId={actingUserId}”`, `null`, `instructorToken`; await
        // menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var transactionsResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Get` (nilai get) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Get,
            // Meneruskan teks interpolasi `$”/api/v1/analytics/sessions/{createdSession.SessionId}/transactions?userId={actingUserId}”`; nilai ekspresi di
            // dalam kurung kurawal disisipkan saat program berjalan sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/analytics/sessions/{createdSession.SessionId}/transactions?userId={actingUserId}",
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen bernama `body`.
            body: null,
            // Meneruskan `instructorToken` (nilai instruktur token) sebagai argumen ke `SendJsonAsync`.
            instructorToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`,
        // `transactionsResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
        Assert.Equal(HttpStatusCode.OK, transactionsResponse.StatusCode);

        // Menyiapkan variabel lokal `transactions` untuk nilai transactions dengan hasil operasi asinkron membaca tanpa argumen menjadi objek bertipe
        // sesuai kontrak JSON melalui `transactionsResponse.Content.ReadFromJsonAsync<TransactionHistoryResponse>`; await menunggu hasil tanpa memblokir
        // thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var transactions = await transactionsResponse.Content.ReadFromJsonAsync<TransactionHistoryResponse>();
        // Menjalankan pemeriksaan NotNull atas `transactions` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
        Assert.NotNull(transactions);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`3`, `transactions.Items.Count`); pengujian
        // gagal jika keduanya berbeda dalam IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
        Assert.Equal(3, transactions.Items.Count);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`playerSetupIngredientCost`,
        // `transactions.Items[0].Amount`, `6`); pengujian gagal jika keduanya berbeda dalam IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
        Assert.Equal(playerSetupIngredientCost, transactions.Items[0].Amount, 6);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`1d`, `transactions.Items[1].Amount`, `6`);
        // pengujian gagal jika keduanya berbeda dalam IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
        Assert.Equal(1d, transactions.Items[1].Amount, 6);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`3d`, `transactions.Items[2].Amount`, `6`);
        // pengujian gagal jika keduanya berbeda dalam IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
        Assert.Equal(3d, transactions.Items[2].Amount, 6);

        // Menyiapkan variabel lokal `recomputeResponse` untuk nilai recompute respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Post`, `$”/api/v1/analytics/sessions/{createdSession.SessionId}/recompute”`, `null`, `instructorToken`; await menunggu hasil tanpa
        // memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var recomputeResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Post` (nilai post) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Post,
            // Meneruskan teks interpolasi `$”/api/v1/analytics/sessions/{createdSession.SessionId}/recompute”`; nilai ekspresi di dalam kurung kurawal
            // disisipkan saat program berjalan sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/analytics/sessions/{createdSession.SessionId}/recompute",
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen bernama `body`.
            body: null,
            // Meneruskan `instructorToken` (nilai instruktur token) sebagai argumen ke `SendJsonAsync`.
            instructorToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`,
        // `recomputeResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
        Assert.Equal(HttpStatusCode.OK, recomputeResponse.StatusCode);
    // Menutup scope metode IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData; bagian berikut berada di luar batas blok tersebut dalam
    // IngestEvents_Then_AnalyticsAndTransactions_ReturnExpectedData.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    /// <summary>
    /// Memvalidasi bahwa urutan aksi dan analitika sesi yang sudah dimulai
    /// mengikuti hasil Tie Breaker yang dikirim IDN, bukan urutan pendaftaran awal.
    /// </summary>
    // Mendefinisikan metode `StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder` dengan hasil bertipe `Task`; operasi ini menangani started sesi
    // uses submitted tie breaker as pemain giliran urutan/pesanan. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan
    // penyelesaian melalui Task.
    public async Task StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder()
    // Membuka scope metode StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder.
    {
        // Menyiapkan variabel lokal `suffix` untuk nilai suffix dengan `Guid.NewGuid().ToString(”N”)[..8]`, yaitu elemen koleksi yang dipilih melalui
        // indeks atau kunci tersebut. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var suffix = Guid.NewGuid().ToString("N")[..8];
        // Menyiapkan variabel lokal `instructorUsername` untuk nilai instruktur username dengan teks interpolasi `$”it_evt_order_instructor_{suffix}”`;
        // nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var instructorUsername = $"it_evt_order_instructor_{suffix}";
        // Menyiapkan variabel lokal `instructorPassword` untuk nilai instruktur password dengan nilai literal `”IntegrationOrderInstructorPass!123”`. Tipe
        // yang dipakai adalah `string`.
        const string instructorPassword = "IntegrationOrderInstructorPass!123";

        // Menyiapkan variabel lokal `instructorToken` untuk nilai instruktur token dengan `(await RegisterAsync(instructorUsername, instructorPassword,
        // ”INSTRUCTOR”)).AccessToken` (nilai akses token). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var instructorToken = (await RegisterAsync(instructorUsername, instructorPassword, "INSTRUCTOR")).AccessToken;

        // Menyiapkan variabel lokal `definition` untuk definisi terstruktur komponen serta parameter aturan permainan dengan memanggil
        // `BuildRulesetDefinition` dengan `20`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var definition = BuildRulesetDefinition(startingCash: 20);
        // Menyiapkan variabel lokal `createRulesetPayload` untuk nilai create aturan payload dengan objek anonim yang mengelompokkan name, description,
        // definition sebagai satu nilai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var createRulesetPayload = new
        // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder.
        {
            // Menggunakan `name` (nilai nama) sebagai bagian ekspresi yang sedang disusun dalam StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder.
            name = $"Ruleset Order IT {suffix}",
            // Menggunakan `description` (nilai description) sebagai bagian ekspresi yang sedang disusun dalam
            // StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder.
            description = "Integration player order assignment",
            // Menggunakan `definition` (definisi terstruktur komponen serta parameter aturan permainan) sebagai bagian ekspresi yang sedang disusun dalam
            // StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder.
            definition
        // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
        // StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder.
        };

        // Menyiapkan variabel lokal `createRulesetResponse` untuk nilai create aturan respons dengan hasil operasi asinkron memanggil `SendJsonAsync`
        // dengan `HttpMethod.Post`, `”/api/v1/rulesets”`, `createRulesetPayload`, `instructorToken`; await menunggu hasil tanpa memblokir thread selama
        // operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var createRulesetResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Post` (nilai post) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Post,
            // Meneruskan nilai literal `”/api/v1/rulesets”` sebagai argumen ke `SendJsonAsync`.
            "/api/v1/rulesets",
            // Meneruskan `createRulesetPayload` (nilai create aturan payload) sebagai argumen ke `SendJsonAsync`.
            createRulesetPayload,
            // Meneruskan `instructorToken` (nilai instruktur token) sebagai argumen ke `SendJsonAsync`.
            instructorToken);
        // Menjalankan pemeriksaan bahwa `createRulesetResponse.StatusCode == HttpStatusCode.Created`, `await
        // createRulesetResponse.Content.ReadAsStringAsync()` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder.
        Assert.True(
            // Meneruskan perbandingan kesamaan antara `createRulesetResponse.StatusCode` dan `HttpStatusCode.Created` sebagai argumen ke `Assert.True`.
            createRulesetResponse.StatusCode == HttpStatusCode.Created,
            // Meneruskan hasil operasi asinkron memanggil `createRulesetResponse.Content.ReadAsStringAsync` dengan tanpa argumen; await menunggu hasil tanpa
            // memblokir thread selama operasi belum selesai sebagai argumen ke `Assert.True`.
            await createRulesetResponse.Content.ReadAsStringAsync());

        // Menyiapkan variabel lokal `createdRuleset` untuk nilai created aturan dengan hasil operasi asinkron membaca tanpa argumen menjadi objek bertipe
        // sesuai kontrak JSON melalui `createRulesetResponse.Content.ReadFromJsonAsync<CreateRulesetResponse>`; await menunggu hasil tanpa memblokir thread
        // selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var createdRuleset = await createRulesetResponse.Content.ReadFromJsonAsync<CreateRulesetResponse>();
        // Menjalankan pemeriksaan NotNull atas `createdRuleset` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder.
        Assert.NotNull(createdRuleset);

        // Menyiapkan variabel lokal `createSessionPayload` untuk nilai create sesi payload dengan objek anonim yang mengelompokkan session_name, mode,
        // ruleset_version_id sebagai satu nilai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var createSessionPayload = new
        // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder.
        {
            // Menggunakan `session_name` (nilai sesi nama) sebagai bagian ekspresi yang sedang disusun dalam
            // StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder.
            session_name = $"Session Order IT {suffix}",
            // Menggunakan `mode` (mode permainan yang menentukan kelompok aturan yang digunakan) sebagai bagian ekspresi yang sedang disusun dalam
            // StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder.
            mode = "PEMULA",
            // Menggunakan `ruleset_version_id` (nilai aturan versi identitas) sebagai bagian ekspresi yang sedang disusun dalam
            // StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder.
            ruleset_version_id = createdRuleset.RulesetVersionId
        // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
        // StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder.
        };

        // Menyiapkan variabel lokal `createSessionResponse` untuk nilai create sesi respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Post`, `”/api/v1/sessions”`, `createSessionPayload`, `instructorToken`; await menunggu hasil tanpa memblokir thread selama operasi
        // belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var createSessionResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Post` (nilai post) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Post,
            // Meneruskan nilai literal `”/api/v1/sessions”` sebagai argumen ke `SendJsonAsync`.
            "/api/v1/sessions",
            // Meneruskan `createSessionPayload` (nilai create sesi payload) sebagai argumen ke `SendJsonAsync`.
            createSessionPayload,
            // Meneruskan `instructorToken` (nilai instruktur token) sebagai argumen ke `SendJsonAsync`.
            instructorToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.Created`,
        // `createSessionResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder.
        Assert.Equal(HttpStatusCode.Created, createSessionResponse.StatusCode);

        // Menyiapkan variabel lokal `createdSession` untuk nilai created sesi dengan hasil operasi asinkron membaca tanpa argumen menjadi objek bertipe
        // sesuai kontrak JSON melalui `createSessionResponse.Content.ReadFromJsonAsync<CreateSessionResponse>`; await menunggu hasil tanpa memblokir thread
        // selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var createdSession = await createSessionResponse.Content.ReadFromJsonAsync<CreateSessionResponse>();
        // Menjalankan pemeriksaan NotNull atas `createdSession` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder.
        Assert.NotNull(createdSession);

        // Menyiapkan variabel lokal `players` untuk nilai pemain dengan objek baru bertipe `List<PlayerResponse>` dengan nilai awal sesuai konstruktornya.
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var players = new List<PlayerResponse>();
        // Memulai loop dengan inisialisasi `var i = 1`, berjalan selama `i <= 3`, lalu memperbarui pencacah melalui `i++` dalam
        // StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder.
        for (var i = 1; i <= 3; i++)
        // Membuka scope loop dengan syarat `i <= 3`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder.
        {
            // Menyiapkan variabel lokal `createPlayerPayload` untuk nilai create pemain payload dengan objek anonim yang mengelompokkan display_name, username,
            // password sebagai satu nilai. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var createPlayerPayload = new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder.
            {
                // Menggunakan `display_name` (nilai display nama) sebagai bagian ekspresi yang sedang disusun dalam
                // StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder.
                display_name = $"Player Order {i} {suffix}",
                // Menggunakan `username` (nama akun yang dipakai saat autentikasi) sebagai bagian ekspresi yang sedang disusun dalam
                // StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder.
                username = $"it_evt_order_player_{i}_{suffix}",
                // Menggunakan `password` (kata sandi masukan yang diperiksa sesuai kebijakan autentikasi) sebagai bagian ekspresi yang sedang disusun dalam
                // StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder.
                password = "IntegrationOrderPlayerPass!123"
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
            // StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder.
            };

            // Menyiapkan variabel lokal `createPlayerResponse` untuk nilai create pemain respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
            // `HttpMethod.Post`, `”/api/v1/players”`, `createPlayerPayload`, `instructorToken`; await menunggu hasil tanpa memblokir thread selama operasi
            // belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var createPlayerResponse = await SendJsonAsync(
                // Meneruskan `HttpMethod.Post` (nilai post) sebagai argumen ke `SendJsonAsync`.
                HttpMethod.Post,
                // Meneruskan nilai literal `”/api/v1/players”` sebagai argumen ke `SendJsonAsync`.
                "/api/v1/players",
                // Meneruskan `createPlayerPayload` (nilai create pemain payload) sebagai argumen ke `SendJsonAsync`.
                createPlayerPayload,
                // Meneruskan `instructorToken` (nilai instruktur token) sebagai argumen ke `SendJsonAsync`.
                instructorToken);
            // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.Created`,
            // `createPlayerResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder.
            Assert.Equal(HttpStatusCode.Created, createPlayerResponse.StatusCode);

            // Menyiapkan variabel lokal `createdPlayer` untuk nilai created pemain dengan hasil operasi asinkron membaca tanpa argumen menjadi objek bertipe
            // sesuai kontrak JSON melalui `createPlayerResponse.Content.ReadFromJsonAsync<PlayerResponse>`; await menunggu hasil tanpa memblokir thread selama
            // operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var createdPlayer = await createPlayerResponse.Content.ReadFromJsonAsync<PlayerResponse>();
            // Menjalankan pemeriksaan NotNull atas `createdPlayer` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
            // StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder.
            Assert.NotNull(createdPlayer);
            // Menjalankan menambahkan `createdPlayer` ke `players` dalam StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder.
            players.Add(createdPlayer);

            // Menyiapkan variabel lokal `addPlayerPayload` untuk nilai add pemain payload dengan objek anonim yang mengelompokkan user_id, player_order_no
            // sebagai satu nilai. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var addPlayerPayload = new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder.
            {
                // Menggunakan `user_id` (nilai pengguna identitas) sebagai bagian ekspresi yang sedang disusun dalam
                // StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder.
                user_id = createdPlayer.UserId,
                // Menggunakan `player_order_no` (nilai pemain urutan/pesanan no) sebagai bagian ekspresi yang sedang disusun dalam
                // StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder.
                player_order_no = i
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
            // StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder.
            };

            // Menyiapkan variabel lokal `addPlayerResponse` untuk nilai add pemain respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
            // `HttpMethod.Post`, `$”/api/v1/sessions/{createdSession.SessionId}/players”`, `addPlayerPayload`, `instructorToken`; await menunggu hasil tanpa
            // memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var addPlayerResponse = await SendJsonAsync(
                // Meneruskan `HttpMethod.Post` (nilai post) sebagai argumen ke `SendJsonAsync`.
                HttpMethod.Post,
                // Meneruskan teks interpolasi `$”/api/v1/sessions/{createdSession.SessionId}/players”`; nilai ekspresi di dalam kurung kurawal disisipkan saat
                // program berjalan sebagai argumen ke `SendJsonAsync`.
                $"/api/v1/sessions/{createdSession.SessionId}/players",
                // Meneruskan `addPlayerPayload` (nilai add pemain payload) sebagai argumen ke `SendJsonAsync`.
                addPlayerPayload,
                // Meneruskan `instructorToken` (nilai instruktur token) sebagai argumen ke `SendJsonAsync`.
                instructorToken);
            // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`,
            // `addPlayerResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder.
            Assert.Equal(HttpStatusCode.OK, addPlayerResponse.StatusCode);
        // Menutup scope loop dengan syarat `i <= 3`; bagian berikut berada di luar batas blok tersebut dalam
        // StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder.
        }

        // Menyiapkan variabel lokal `saveSetupResponse` untuk nilai save setup respons dengan hasil operasi asinkron memanggil
        // `SessionSetupTestHelper.SaveAsync` dengan `_client`, `instructorToken`, `createdSession.SessionId`, `definition`,
        // `TestContext.Current.CancellationToken`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari
        // ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var saveSetupResponse = await SessionSetupTestHelper.SaveAsync(
            // Meneruskan `_client` (nilai client) sebagai argumen ke `SessionSetupTestHelper.SaveAsync`.
            _client,
            // Meneruskan `instructorToken` (nilai instruktur token) sebagai argumen ke `SessionSetupTestHelper.SaveAsync`.
            instructorToken,
            // Meneruskan `createdSession.SessionId` (identitas unik sesi permainan yang menjadi batas data operasi ini) sebagai argumen ke
            // `SessionSetupTestHelper.SaveAsync`.
            createdSession.SessionId,
            // Meneruskan `definition` (definisi terstruktur komponen serta parameter aturan permainan) sebagai argumen ke `SessionSetupTestHelper.SaveAsync`.
            definition,
            // Meneruskan `TestContext.Current.CancellationToken` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
            // aplikasi berhenti) sebagai argumen ke `SessionSetupTestHelper.SaveAsync`.
            TestContext.Current.CancellationToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.Created`,
        // `saveSetupResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder.
        Assert.Equal(HttpStatusCode.Created, saveSetupResponse.StatusCode);

        // Menyiapkan variabel lokal `startSessionResponse` untuk nilai start sesi respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Post`, `$”/api/v1/sessions/{createdSession.SessionId}/start”`, `null`, `instructorToken`; await menunggu hasil tanpa memblokir thread
        // selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var startSessionResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Post` (nilai post) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Post,
            // Meneruskan teks interpolasi `$”/api/v1/sessions/{createdSession.SessionId}/start”`; nilai ekspresi di dalam kurung kurawal disisipkan saat
            // program berjalan sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/sessions/{createdSession.SessionId}/start",
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen bernama `body`.
            body: null,
            // Meneruskan `instructorToken` (nilai instruktur token) sebagai argumen ke `SendJsonAsync`.
            instructorToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`,
        // `startSessionResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder.
        Assert.Equal(HttpStatusCode.OK, startSessionResponse.StatusCode);

        // Menyiapkan variabel lokal `rulesetDetailResponse` untuk nilai aturan detail respons dengan hasil operasi asinkron memanggil `SendJsonAsync`
        // dengan `HttpMethod.Get`, `$”/api/v1/rulesets/{createdRuleset.RulesetId}”`, `null`, `instructorToken`; await menunggu hasil tanpa memblokir thread
        // selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var rulesetDetailResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Get` (nilai get) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Get,
            // Meneruskan teks interpolasi `$”/api/v1/rulesets/{createdRuleset.RulesetId}”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program
            // berjalan sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/rulesets/{createdRuleset.RulesetId}",
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen bernama `body`.
            body: null,
            // Meneruskan `instructorToken` (nilai instruktur token) sebagai argumen ke `SendJsonAsync`.
            instructorToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`,
        // `rulesetDetailResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder.
        Assert.Equal(HttpStatusCode.OK, rulesetDetailResponse.StatusCode);

        // Menyiapkan variabel lokal `rulesetDetail` untuk nilai aturan detail dengan hasil operasi asinkron membaca tanpa argumen menjadi objek bertipe
        // sesuai kontrak JSON melalui `rulesetDetailResponse.Content.ReadFromJsonAsync<RulesetDetailResponse>`; await menunggu hasil tanpa memblokir thread
        // selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var rulesetDetail = await rulesetDetailResponse.Content.ReadFromJsonAsync<RulesetDetailResponse>();
        // Menjalankan pemeriksaan NotNull atas `rulesetDetail` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder.
        Assert.NotNull(rulesetDetail);

        // Menyiapkan variabel lokal `activeVersion` untuk nilai aktif versi dengan mengambil elemen pertama `rulesetDetail.Versions .Where(v =>
        // string.Equals(v.Status, ”ACTIVE”, StringComparison.OrdinalIgnoreCase)) .OrderByDescending(v => v.Version)` sesuai tanpa argumen; urutan tanpa
        // kecocokan menyebabkan exception. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var activeVersion = rulesetDetail.Versions
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(v => string.Equals(v.Status, ”ACTIVE”,
            // StringComparison.OrdinalIgnoreCase)) dalam StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder; token pada baris ini menyambungkan bagian
            // kode sebelum dan sesudahnya.
            .Where(v => string.Equals(v.Status, "ACTIVE", StringComparison.OrdinalIgnoreCase))
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .OrderByDescending(v => v.Version) dalam
            // StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .OrderByDescending(v => v.Version)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .First(); dalam StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder; token
            // pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .First();

        // Menyiapkan variabel lokal `setupEventsResponse` untuk nilai setup event respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Get`, `$”/api/v1/sessions/{createdSession.SessionId}/events?limit=100”`, `null`, `instructorToken`; await menunggu hasil tanpa
        // memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis
        // saat scope berakhir.
        using var setupEventsResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Get` (nilai get) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Get,
            // Meneruskan teks interpolasi `$”/api/v1/sessions/{createdSession.SessionId}/events?limit=100”`; nilai ekspresi di dalam kurung kurawal disisipkan
            // saat program berjalan sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/sessions/{createdSession.SessionId}/events?limit=100",
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen ke `SendJsonAsync`.
            null,
            // Meneruskan `instructorToken` (nilai instruktur token) sebagai argumen ke `SendJsonAsync`.
            instructorToken);
        // Menyiapkan variabel lokal `setupEventsBody` untuk nilai setup event body dengan hasil operasi asinkron memanggil `ReadJsonAsync` dengan
        // `setupEventsResponse`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai
        // awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var setupEventsBody = await ReadJsonAsync(setupEventsResponse);
        // Menyiapkan variabel lokal `playerOrderByUserId` untuk nilai pemain urutan/pesanan berdasarkan pengguna identitas dengan membangun kamus dari
        // `setupEventsBody.RootElement.GetProperty(”items”) .EnumerateArray() .Where(item => item.GetProperty(”action_type”).GetString() ==
        // ”BagikanTieBreaker”)` dengan pemilihan kunci/nilai `item => item.GetProperty(”user_id”).GetGuid()`, `item =>
        // item.GetProperty(”payload”).GetProperty(”number”).GetInt32()`; kunci harus unik agar konversi berhasil. Tipe variabel disimpulkan dari ekspresi
        // nilai awal.
        var playerOrderByUserId = setupEventsBody.RootElement.GetProperty("items")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .EnumerateArray() dalam
            // StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .EnumerateArray()
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(item => item.GetProperty(”action_type”).GetString() ==
            // ”BagikanTieBreaker”) dalam StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder; token pada baris ini menyambungkan bagian kode sebelum dan
            // sesudahnya.
            .Where(item => item.GetProperty("action_type").GetString() == "BagikanTieBreaker")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToDictionary( dalam StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder;
            // token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .ToDictionary(
                // Parameter `item` bertipe `` membawa nilai elemen.
                item => item.GetProperty("user_id").GetGuid(),
                // Parameter `item` bertipe `` membawa nilai elemen.
                item => item.GetProperty("payload").GetProperty("number").GetInt32());
        // Menyiapkan variabel lokal `playersInTurnOrder` untuk nilai pemain in giliran urutan/pesanan dengan mematerialisasi urutan `players.OrderBy(player
        // => playerOrderByUserId[player.UserId])` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var playersInTurnOrder = players.OrderBy(player => playerOrderByUserId[player.UserId]).ToList();
        // Menyiapkan variabel lokal `now` untuk nilai now dengan `DateTimeOffset.UtcNow`, yaitu waktu UTC saat operasi dilakukan. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var now = DateTimeOffset.UtcNow;
        // Menyiapkan variabel lokal `sequence` untuk nomor urut event yang menentukan urutan pemrosesan riwayat permainan dengan hasil operasi asinkron
        // memanggil `GetNextSequenceNumberAsync` dengan `createdSession.SessionId`, `instructorToken`; await menunggu hasil tanpa memblokir thread selama
        // operasi belum selesai. Tipe yang dipakai adalah `long`.
        long sequence = await GetNextSequenceNumberAsync(createdSession.SessionId, instructorToken);
        // Mengulangi setiap elemen `playersInTurnOrder`; elemen saat ini disimpan sebagai `player` bertipe `var` untuk diproses oleh badan loop dalam
        // StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder.
        foreach (var player in playersInTurnOrder)
        // Membuka scope loop setiap player dari `playersInTurnOrder`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder.
        {
            // Memulai loop dengan inisialisasi `var slot = 1`, berjalan selama `slot <= 2`, lalu memperbarui pencacah melalui `slot++` dalam
            // StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder.
            for (var slot = 1; slot <= 2; slot++)
            // Membuka scope loop dengan syarat `slot <= 2`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder.
            {
                // Menyiapkan variabel lokal `eventPayload` untuk nilai event payload dengan objek anonim yang mengelompokkan event_id, session_id, user_id,
                // actor_type, turn_number, timestamp, day_index, weekday, action_slot, sequence_number, action_type, ruleset_version_id, payload sebagai satu
                // nilai. Tipe variabel disimpulkan dari ekspresi nilai awal.
                var eventPayload = new
                // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder.
                {
                    // Menggunakan `event_id` (nilai event identitas) sebagai bagian ekspresi yang sedang disusun dalam
                    // StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder.
                    event_id = Guid.NewGuid(),
                    // Menggunakan `session_id` (nilai sesi identitas) sebagai bagian ekspresi yang sedang disusun dalam
                    // StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder.
                    session_id = createdSession.SessionId,
                    // Menggunakan `user_id` (nilai pengguna identitas) sebagai bagian ekspresi yang sedang disusun dalam
                    // StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder.
                    user_id = player.UserId,
                    // Menggunakan `actor_type` (nilai actor jenis) sebagai bagian ekspresi yang sedang disusun dalam
                    // StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder.
                    actor_type = "PLAYER",
                    // Menggunakan `turn_number` (nilai giliran number) sebagai bagian ekspresi yang sedang disusun dalam
                    // StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder.
                    turn_number = playerOrderByUserId[player.UserId],
                    // Menggunakan `timestamp` (waktu kejadian yang menjaga urutan kronologis data) sebagai bagian ekspresi yang sedang disusun dalam
                    // StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder.
                    timestamp = now.AddSeconds(sequence).ToString("O"),
                    // Menggunakan `day_index` (nilai hari index) sebagai bagian ekspresi yang sedang disusun dalam
                    // StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder.
                    day_index = 1,
                    // Menggunakan `weekday` (nilai weekday) sebagai bagian ekspresi yang sedang disusun dalam StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder.
                    weekday = "MON",
                    // Menggunakan `action_slot` (nilai aksi slot) sebagai bagian ekspresi yang sedang disusun dalam
                    // StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder.
                    action_slot = slot,
                    // Menggunakan `sequence_number` (nilai sequence number) sebagai bagian ekspresi yang sedang disusun dalam
                    // StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder.
                    sequence_number = sequence,
                    // Menggunakan `action_type` (nilai aksi jenis) sebagai bagian ekspresi yang sedang disusun dalam
                    // StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder.
                    action_type = "KerjaLepas",
                    // Menggunakan `ruleset_version_id` (nilai aturan versi identitas) sebagai bagian ekspresi yang sedang disusun dalam
                    // StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder.
                    ruleset_version_id = activeVersion.RulesetVersionId,
                    // Menggunakan `payload` (muatan detail event dalam format JSON) sebagai bagian ekspresi yang sedang disusun dalam
                    // StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder.
                    payload = new
                    // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                    // StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder.
                    {
                        // Menggunakan `amount` (nominal uang atau nilai transaksi yang dipakai dalam operasi) sebagai bagian ekspresi yang sedang disusun dalam
                        // StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder.
                        amount = 1
                    // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
                    // StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder.
                    }
                // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
                // StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder.
                };

                // Menyiapkan variabel lokal `eventResponse` untuk nilai event respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
                // `HttpMethod.Post`, `”/api/v1/events”`, `eventPayload`, `instructorToken`; await menunggu hasil tanpa memblokir thread selama operasi belum
                // selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
                var eventResponse = await SendJsonAsync(HttpMethod.Post, "/api/v1/events", eventPayload, instructorToken);
                // Menjalankan pemeriksaan bahwa `eventResponse.StatusCode == HttpStatusCode.Created`, `await eventResponse.Content.ReadAsStringAsync()` bernilai
                // benar; pengujian gagal jika kondisi tidak terpenuhi dalam StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder.
                Assert.True(
                    // Meneruskan perbandingan kesamaan antara `eventResponse.StatusCode` dan `HttpStatusCode.Created` sebagai argumen ke `Assert.True`.
                    eventResponse.StatusCode == HttpStatusCode.Created,
                    // Meneruskan hasil operasi asinkron memanggil `eventResponse.Content.ReadAsStringAsync` dengan tanpa argumen; await menunggu hasil tanpa memblokir
                    // thread selama operasi belum selesai sebagai argumen ke `Assert.True`.
                    await eventResponse.Content.ReadAsStringAsync());
                // Memperbarui `sequence` dengan menambahkan nilai literal `1` dalam StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder.
                sequence += 1;
            // Menutup scope loop dengan syarat `slot <= 2`; bagian berikut berada di luar batas blok tersebut dalam
            // StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder.
            }
        // Menutup scope loop setiap player dari `playersInTurnOrder`; bagian berikut berada di luar batas blok tersebut dalam
        // StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder.
        }

        // Menyiapkan variabel lokal `analyticsResponse` untuk nilai analytics respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Get`, `$”/api/v1/analytics/sessions/{createdSession.SessionId}”`, `null`, `instructorToken`; await menunggu hasil tanpa memblokir
        // thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var analyticsResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Get` (nilai get) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Get,
            // Meneruskan teks interpolasi `$”/api/v1/analytics/sessions/{createdSession.SessionId}”`; nilai ekspresi di dalam kurung kurawal disisipkan saat
            // program berjalan sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/analytics/sessions/{createdSession.SessionId}",
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen bernama `body`.
            body: null,
            // Meneruskan `instructorToken` (nilai instruktur token) sebagai argumen ke `SendJsonAsync`.
            instructorToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`,
        // `analyticsResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder.
        Assert.Equal(HttpStatusCode.OK, analyticsResponse.StatusCode);

        // Menyiapkan variabel lokal `analytics` untuk nilai analytics dengan hasil operasi asinkron membaca tanpa argumen menjadi objek bertipe sesuai
        // kontrak JSON melalui `analyticsResponse.Content.ReadFromJsonAsync<AnalyticsSessionResponse>`; await menunggu hasil tanpa memblokir thread selama
        // operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var analytics = await analyticsResponse.Content.ReadFromJsonAsync<AnalyticsSessionResponse>();
        // Menjalankan pemeriksaan NotNull atas `analytics` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder.
        Assert.NotNull(analytics);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`3`, `analytics.ByPlayer.Count`); pengujian
        // gagal jika keduanya berbeda dalam StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder.
        Assert.Equal(3, analytics.ByPlayer.Count);

        // Menyiapkan variabel lokal `expectedPlayerOrder` untuk nilai yang diharapkan pemain urutan/pesanan dengan mematerialisasi urutan
        // `playersInTurnOrder.Select(item => item.UserId)` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var expectedPlayerOrder = playersInTurnOrder.Select(item => item.UserId).ToList();
        // Menyiapkan variabel lokal `actualPlayerOrder` untuk nilai aktual pemain urutan/pesanan dengan mematerialisasi urutan
        // `analytics.ByPlayer.Select(item => item.UserId)` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var actualPlayerOrder = analytics.ByPlayer.Select(item => item.UserId).ToList();
        // Menyiapkan variabel lokal `actualPlayerOrders` untuk nilai aktual pemain pesanan dengan mematerialisasi urutan `analytics.ByPlayer.Select(item =>
        // item.PlayerOrder)` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var actualPlayerOrders = analytics.ByPlayer.Select(item => item.PlayerOrder).ToList();
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`expectedPlayerOrder`, `actualPlayerOrder`);
        // pengujian gagal jika keduanya berbeda dalam StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder.
        Assert.Equal(expectedPlayerOrder, actualPlayerOrder);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`new[] { 1, 2, 3 }`, `actualPlayerOrders`);
        // pengujian gagal jika keduanya berbeda dalam StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder.
        Assert.Equal(new[] { 1, 2, 3 }, actualPlayerOrders);
    // Menutup scope metode StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder; bagian berikut berada di luar batas blok tersebut dalam
    // StartedSession_UsesSubmittedTieBreakerAsPlayerTurnOrder.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    /// <summary>
    /// Memvalidasi bahwa penambahan pemain via username tanpa player_order_no tetap
    /// mengikuti urutan assignment sesi.
    /// </summary>
    // Mendefinisikan metode `AddPlayersByUsername_WithoutExplicitOrder_UsesAssignmentOrder` dengan hasil bertipe `Task`; operasi ini menangani add
    // pemain berdasarkan username tanpa explicit urutan/pesanan uses assignment urutan/pesanan. async memungkinkan metode menunggu operasi I/O dengan
    // await dan mengembalikan penyelesaian melalui Task.
    public async Task AddPlayersByUsername_WithoutExplicitOrder_UsesAssignmentOrder()
    // Membuka scope metode AddPlayersByUsername_WithoutExplicitOrder_UsesAssignmentOrder; pernyataan/deklarasi berikut berada di dalam batas blok ini
    // dalam AddPlayersByUsername_WithoutExplicitOrder_UsesAssignmentOrder.
    {
        // Menyiapkan variabel lokal `suffix` untuk nilai suffix dengan `Guid.NewGuid().ToString(”N”)[..8]`, yaitu elemen koleksi yang dipilih melalui
        // indeks atau kunci tersebut. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var suffix = Guid.NewGuid().ToString("N")[..8];
        // Menyiapkan variabel lokal `instructorUsername` untuk nilai instruktur username dengan teks interpolasi
        // `$”it_evt_assignment_order_instructor_{suffix}”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var instructorUsername = $"it_evt_assignment_order_instructor_{suffix}";
        // Menyiapkan variabel lokal `instructorPassword` untuk nilai instruktur password dengan nilai literal
        // `”IntegrationAssignmentOrderInstructorPass!123”`. Tipe yang dipakai adalah `string`.
        const string instructorPassword = "IntegrationAssignmentOrderInstructorPass!123";
        // Menyiapkan variabel lokal `instructorToken` untuk nilai instruktur token dengan `(await RegisterAsync(instructorUsername, instructorPassword,
        // ”INSTRUCTOR”)).AccessToken` (nilai akses token). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var instructorToken = (await RegisterAsync(instructorUsername, instructorPassword, "INSTRUCTOR")).AccessToken;
        // Menyiapkan variabel lokal `firstPlayerUsername` untuk nilai first pemain username dengan teks interpolasi
        // `$”it_evt_assignment_order_player_1_{suffix}”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var firstPlayerUsername = $"it_evt_assignment_order_player_1_{suffix}";
        // Menyiapkan variabel lokal `secondPlayerUsername` untuk nilai second pemain username dengan teks interpolasi
        // `$”it_evt_assignment_order_player_2_{suffix}”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var secondPlayerUsername = $"it_evt_assignment_order_player_2_{suffix}";
        // Menyiapkan variabel lokal `thirdPlayerUsername` untuk nilai third pemain username dengan teks interpolasi
        // `$”it_evt_assignment_order_player_3_{suffix}”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var thirdPlayerUsername = $"it_evt_assignment_order_player_3_{suffix}";

        // Menyiapkan variabel lokal `definition` untuk definisi terstruktur komponen serta parameter aturan permainan dengan memanggil
        // `BuildRulesetDefinition` dengan `20`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var definition = BuildRulesetDefinition(startingCash: 20);
        // Menyiapkan variabel lokal `createRulesetPayload` untuk nilai create aturan payload dengan objek anonim yang mengelompokkan name, description,
        // definition sebagai satu nilai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var createRulesetPayload = new
        // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // AddPlayersByUsername_WithoutExplicitOrder_UsesAssignmentOrder.
        {
            // Menggunakan `name` (nilai nama) sebagai bagian ekspresi yang sedang disusun dalam AddPlayersByUsername_WithoutExplicitOrder_UsesAssignmentOrder.
            name = $"Ruleset Assignment Order IT {suffix}",
            // Menggunakan `description` (nilai description) sebagai bagian ekspresi yang sedang disusun dalam
            // AddPlayersByUsername_WithoutExplicitOrder_UsesAssignmentOrder.
            description = "Integration assignment order by username",
            // Menggunakan `definition` (definisi terstruktur komponen serta parameter aturan permainan) sebagai bagian ekspresi yang sedang disusun dalam
            // AddPlayersByUsername_WithoutExplicitOrder_UsesAssignmentOrder.
            definition
        // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
        // AddPlayersByUsername_WithoutExplicitOrder_UsesAssignmentOrder.
        };

        // Menyiapkan variabel lokal `createRulesetResponse` untuk nilai create aturan respons dengan hasil operasi asinkron memanggil `SendJsonAsync`
        // dengan `HttpMethod.Post`, `”/api/v1/rulesets”`, `createRulesetPayload`, `instructorToken`; await menunggu hasil tanpa memblokir thread selama
        // operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var createRulesetResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Post` (nilai post) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Post,
            // Meneruskan nilai literal `”/api/v1/rulesets”` sebagai argumen ke `SendJsonAsync`.
            "/api/v1/rulesets",
            // Meneruskan `createRulesetPayload` (nilai create aturan payload) sebagai argumen ke `SendJsonAsync`.
            createRulesetPayload,
            // Meneruskan `instructorToken` (nilai instruktur token) sebagai argumen ke `SendJsonAsync`.
            instructorToken);
        // Menjalankan pemeriksaan bahwa `createRulesetResponse.StatusCode == HttpStatusCode.Created`, `await
        // createRulesetResponse.Content.ReadAsStringAsync()` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // AddPlayersByUsername_WithoutExplicitOrder_UsesAssignmentOrder.
        Assert.True(
            // Meneruskan perbandingan kesamaan antara `createRulesetResponse.StatusCode` dan `HttpStatusCode.Created` sebagai argumen ke `Assert.True`.
            createRulesetResponse.StatusCode == HttpStatusCode.Created,
            // Meneruskan hasil operasi asinkron memanggil `createRulesetResponse.Content.ReadAsStringAsync` dengan tanpa argumen; await menunggu hasil tanpa
            // memblokir thread selama operasi belum selesai sebagai argumen ke `Assert.True`.
            await createRulesetResponse.Content.ReadAsStringAsync());

        // Menyiapkan variabel lokal `createdRuleset` untuk nilai created aturan dengan hasil operasi asinkron membaca tanpa argumen menjadi objek bertipe
        // sesuai kontrak JSON melalui `createRulesetResponse.Content.ReadFromJsonAsync<CreateRulesetResponse>`; await menunggu hasil tanpa memblokir thread
        // selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var createdRuleset = await createRulesetResponse.Content.ReadFromJsonAsync<CreateRulesetResponse>();
        // Menjalankan pemeriksaan NotNull atas `createdRuleset` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // AddPlayersByUsername_WithoutExplicitOrder_UsesAssignmentOrder.
        Assert.NotNull(createdRuleset);

        // Menyiapkan variabel lokal `createSessionResponse` untuk nilai create sesi respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Post`, `”/api/v1/sessions”`, `new { session_name = $”Session Assignment Order IT {suffix}”, mode = ”PEMULA”, ruleset_version_id =
        // createdRuleset.RulesetVersionId }`, `instructorToken`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var createSessionResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Post` (nilai post) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Post,
            // Meneruskan nilai literal `”/api/v1/sessions”` sebagai argumen ke `SendJsonAsync`.
            "/api/v1/sessions",
            // Meneruskan objek anonim yang mengelompokkan session_name, mode, ruleset_version_id sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // AddPlayersByUsername_WithoutExplicitOrder_UsesAssignmentOrder.
            {
                // Meneruskan objek anonim yang mengelompokkan session_name, mode, ruleset_version_id sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                session_name = $"Session Assignment Order IT {suffix}",
                // Meneruskan objek anonim yang mengelompokkan session_name, mode, ruleset_version_id sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                mode = "PEMULA",
                // Meneruskan objek anonim yang mengelompokkan session_name, mode, ruleset_version_id sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                ruleset_version_id = createdRuleset.RulesetVersionId
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
            // AddPlayersByUsername_WithoutExplicitOrder_UsesAssignmentOrder.
            },
            // Meneruskan `instructorToken` (nilai instruktur token) sebagai argumen ke `SendJsonAsync`.
            instructorToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.Created`,
        // `createSessionResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam AddPlayersByUsername_WithoutExplicitOrder_UsesAssignmentOrder.
        Assert.Equal(HttpStatusCode.Created, createSessionResponse.StatusCode);

        // Menyiapkan variabel lokal `createdSession` untuk nilai created sesi dengan hasil operasi asinkron membaca tanpa argumen menjadi objek bertipe
        // sesuai kontrak JSON melalui `createSessionResponse.Content.ReadFromJsonAsync<CreateSessionResponse>`; await menunggu hasil tanpa memblokir thread
        // selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var createdSession = await createSessionResponse.Content.ReadFromJsonAsync<CreateSessionResponse>();
        // Menjalankan pemeriksaan NotNull atas `createdSession` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // AddPlayersByUsername_WithoutExplicitOrder_UsesAssignmentOrder.
        Assert.NotNull(createdSession);

        // Menyiapkan variabel lokal `players` untuk nilai pemain dengan objek baru bertipe `List<(string Username, Guid UserId)>` dengan nilai awal sesuai
        // konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var players = new List<(string Username, Guid UserId)>();
        // Memulai loop dengan inisialisasi `var i = 1`, berjalan selama `i <= 3`, lalu memperbarui pencacah melalui `i++` dalam
        // AddPlayersByUsername_WithoutExplicitOrder_UsesAssignmentOrder.
        for (var i = 1; i <= 3; i++)
        // Membuka scope loop dengan syarat `i <= 3`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // AddPlayersByUsername_WithoutExplicitOrder_UsesAssignmentOrder.
        {
            // Menyiapkan variabel lokal `username` untuk nama akun yang dipakai saat autentikasi dengan hasil pemetaan `i` melalui cabang pola switch yang
            // cocok. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var username = i switch
            // Membuka scope pemetaan switch atas `i`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // AddPlayersByUsername_WithoutExplicitOrder_UsesAssignmentOrder.
            {
                // Untuk pola `1`, menghasilkan `firstPlayerUsername` (nilai first pemain username) sebagai hasil switch.
                1 => firstPlayerUsername,
                // Untuk pola `2`, menghasilkan `secondPlayerUsername` (nilai second pemain username) sebagai hasil switch.
                2 => secondPlayerUsername,
                // Untuk pola `_`, menghasilkan `thirdPlayerUsername` (nilai third pemain username) sebagai hasil switch.
                _ => thirdPlayerUsername
            // Menutup scope pemetaan switch atas `i`; bagian berikut berada di luar batas blok tersebut dalam
            // AddPlayersByUsername_WithoutExplicitOrder_UsesAssignmentOrder.
            };
            // Menyiapkan variabel lokal `createPlayerResponse` untuk nilai create pemain respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
            // `HttpMethod.Post`, `”/api/v1/players”`, `new { display_name = $”Player Assignment Order {i} {suffix}”, username, password =
            // ”IntegrationAssignmentOrderPlayerPass!123” }`, `instructorToken`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
            // variabel disimpulkan dari ekspresi nilai awal.
            var createPlayerResponse = await SendJsonAsync(
                // Meneruskan `HttpMethod.Post` (nilai post) sebagai argumen ke `SendJsonAsync`.
                HttpMethod.Post,
                // Meneruskan nilai literal `”/api/v1/players”` sebagai argumen ke `SendJsonAsync`.
                "/api/v1/players",
                // Meneruskan objek anonim yang mengelompokkan display_name, username, password sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                new
                // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // AddPlayersByUsername_WithoutExplicitOrder_UsesAssignmentOrder.
                {
                    // Meneruskan objek anonim yang mengelompokkan display_name, username, password sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                    display_name = $"Player Assignment Order {i} {suffix}",
                    // Meneruskan objek anonim yang mengelompokkan display_name, username, password sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                    username,
                    // Meneruskan objek anonim yang mengelompokkan display_name, username, password sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                    password = "IntegrationAssignmentOrderPlayerPass!123"
                // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
                // AddPlayersByUsername_WithoutExplicitOrder_UsesAssignmentOrder.
                },
                // Meneruskan `instructorToken` (nilai instruktur token) sebagai argumen ke `SendJsonAsync`.
                instructorToken);
            // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.Created`,
            // `createPlayerResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam AddPlayersByUsername_WithoutExplicitOrder_UsesAssignmentOrder.
            Assert.Equal(HttpStatusCode.Created, createPlayerResponse.StatusCode);

            // Menyiapkan variabel lokal `createdPlayer` untuk nilai created pemain dengan hasil operasi asinkron membaca tanpa argumen menjadi objek bertipe
            // sesuai kontrak JSON melalui `createPlayerResponse.Content.ReadFromJsonAsync<PlayerResponse>`; await menunggu hasil tanpa memblokir thread selama
            // operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var createdPlayer = await createPlayerResponse.Content.ReadFromJsonAsync<PlayerResponse>();
            // Menjalankan pemeriksaan NotNull atas `createdPlayer` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
            // AddPlayersByUsername_WithoutExplicitOrder_UsesAssignmentOrder.
            Assert.NotNull(createdPlayer);
            // Menjalankan menambahkan `(username, createdPlayer.UserId)` ke `players` dalam AddPlayersByUsername_WithoutExplicitOrder_UsesAssignmentOrder.
            players.Add((username, createdPlayer.UserId));
        // Menutup scope loop dengan syarat `i <= 3`; bagian berikut berada di luar batas blok tersebut dalam
        // AddPlayersByUsername_WithoutExplicitOrder_UsesAssignmentOrder.
        }

        // Menyiapkan variabel lokal `firstPlayer` untuk nilai first pemain dengan `players[0]`, yaitu elemen koleksi yang dipilih melalui indeks atau kunci
        // tersebut. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var firstPlayer = players[0];
        // Menyiapkan variabel lokal `secondPlayer` untuk nilai second pemain dengan `players[1]`, yaitu elemen koleksi yang dipilih melalui indeks atau
        // kunci tersebut. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var secondPlayer = players[1];
        // Menyiapkan variabel lokal `thirdPlayer` untuk nilai third pemain dengan `players[2]`, yaitu elemen koleksi yang dipilih melalui indeks atau kunci
        // tersebut. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var thirdPlayer = players[2];

        // Menyiapkan variabel lokal `addFirst` untuk nilai add first dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan `HttpMethod.Post`,
        // `$”/api/v1/sessions/{createdSession.SessionId}/players”`, `new { username = firstPlayer.Username }`, `instructorToken`; await menunggu hasil
        // tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var addFirst = await SendJsonAsync(
            // Meneruskan `HttpMethod.Post` (nilai post) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Post,
            // Meneruskan teks interpolasi `$”/api/v1/sessions/{createdSession.SessionId}/players”`; nilai ekspresi di dalam kurung kurawal disisipkan saat
            // program berjalan sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/sessions/{createdSession.SessionId}/players",
            // Meneruskan objek anonim yang mengelompokkan username sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            new { username = firstPlayer.Username },
            // Meneruskan `instructorToken` (nilai instruktur token) sebagai argumen ke `SendJsonAsync`.
            instructorToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`, `addFirst.StatusCode`);
        // pengujian gagal jika keduanya berbeda dalam AddPlayersByUsername_WithoutExplicitOrder_UsesAssignmentOrder.
        Assert.Equal(HttpStatusCode.OK, addFirst.StatusCode);
        // Menyiapkan variabel lokal `addedFirst` untuk nilai added first dengan hasil operasi asinkron membaca tanpa argumen menjadi objek bertipe sesuai
        // kontrak JSON melalui `addFirst.Content.ReadFromJsonAsync<AddSessionPlayerResponse>`; await menunggu hasil tanpa memblokir thread selama operasi
        // belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var addedFirst = await addFirst.Content.ReadFromJsonAsync<AddSessionPlayerResponse>();
        // Menjalankan pemeriksaan NotNull atas `addedFirst` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // AddPlayersByUsername_WithoutExplicitOrder_UsesAssignmentOrder.
        Assert.NotNull(addedFirst);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`1`, `addedFirst.PlayerOrder`); pengujian gagal
        // jika keduanya berbeda dalam AddPlayersByUsername_WithoutExplicitOrder_UsesAssignmentOrder.
        Assert.Equal(1, addedFirst.PlayerOrder);

        // Menyiapkan variabel lokal `addSecond` untuk nilai add second dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan `HttpMethod.Post`,
        // `$”/api/v1/sessions/{createdSession.SessionId}/players”`, `new { username = secondPlayer.Username }`, `instructorToken`; await menunggu hasil
        // tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var addSecond = await SendJsonAsync(
            // Meneruskan `HttpMethod.Post` (nilai post) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Post,
            // Meneruskan teks interpolasi `$”/api/v1/sessions/{createdSession.SessionId}/players”`; nilai ekspresi di dalam kurung kurawal disisipkan saat
            // program berjalan sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/sessions/{createdSession.SessionId}/players",
            // Meneruskan objek anonim yang mengelompokkan username sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            new { username = secondPlayer.Username },
            // Meneruskan `instructorToken` (nilai instruktur token) sebagai argumen ke `SendJsonAsync`.
            instructorToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`, `addSecond.StatusCode`);
        // pengujian gagal jika keduanya berbeda dalam AddPlayersByUsername_WithoutExplicitOrder_UsesAssignmentOrder.
        Assert.Equal(HttpStatusCode.OK, addSecond.StatusCode);
        // Menyiapkan variabel lokal `addedSecond` untuk nilai added second dengan hasil operasi asinkron membaca tanpa argumen menjadi objek bertipe sesuai
        // kontrak JSON melalui `addSecond.Content.ReadFromJsonAsync<AddSessionPlayerResponse>`; await menunggu hasil tanpa memblokir thread selama operasi
        // belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var addedSecond = await addSecond.Content.ReadFromJsonAsync<AddSessionPlayerResponse>();
        // Menjalankan pemeriksaan NotNull atas `addedSecond` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // AddPlayersByUsername_WithoutExplicitOrder_UsesAssignmentOrder.
        Assert.NotNull(addedSecond);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`2`, `addedSecond.PlayerOrder`); pengujian
        // gagal jika keduanya berbeda dalam AddPlayersByUsername_WithoutExplicitOrder_UsesAssignmentOrder.
        Assert.Equal(2, addedSecond.PlayerOrder);

        // Menyiapkan variabel lokal `addThird` untuk nilai add third dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan `HttpMethod.Post`,
        // `$”/api/v1/sessions/{createdSession.SessionId}/players”`, `new { username = thirdPlayer.Username }`, `instructorToken`; await menunggu hasil
        // tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var addThird = await SendJsonAsync(
            // Meneruskan `HttpMethod.Post` (nilai post) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Post,
            // Meneruskan teks interpolasi `$”/api/v1/sessions/{createdSession.SessionId}/players”`; nilai ekspresi di dalam kurung kurawal disisipkan saat
            // program berjalan sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/sessions/{createdSession.SessionId}/players",
            // Meneruskan objek anonim yang mengelompokkan username sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            new { username = thirdPlayer.Username },
            // Meneruskan `instructorToken` (nilai instruktur token) sebagai argumen ke `SendJsonAsync`.
            instructorToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`, `addThird.StatusCode`);
        // pengujian gagal jika keduanya berbeda dalam AddPlayersByUsername_WithoutExplicitOrder_UsesAssignmentOrder.
        Assert.Equal(HttpStatusCode.OK, addThird.StatusCode);
        // Menyiapkan variabel lokal `addedThird` untuk nilai added third dengan hasil operasi asinkron membaca tanpa argumen menjadi objek bertipe sesuai
        // kontrak JSON melalui `addThird.Content.ReadFromJsonAsync<AddSessionPlayerResponse>`; await menunggu hasil tanpa memblokir thread selama operasi
        // belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var addedThird = await addThird.Content.ReadFromJsonAsync<AddSessionPlayerResponse>();
        // Menjalankan pemeriksaan NotNull atas `addedThird` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // AddPlayersByUsername_WithoutExplicitOrder_UsesAssignmentOrder.
        Assert.NotNull(addedThird);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`3`, `addedThird.PlayerOrder`); pengujian gagal
        // jika keduanya berbeda dalam AddPlayersByUsername_WithoutExplicitOrder_UsesAssignmentOrder.
        Assert.Equal(3, addedThird.PlayerOrder);

        // Menyiapkan variabel lokal `analyticsResponse` untuk nilai analytics respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Get`, `$”/api/v1/analytics/sessions/{createdSession.SessionId}”`, `null`, `instructorToken`; await menunggu hasil tanpa memblokir
        // thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var analyticsResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Get` (nilai get) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Get,
            // Meneruskan teks interpolasi `$”/api/v1/analytics/sessions/{createdSession.SessionId}”`; nilai ekspresi di dalam kurung kurawal disisipkan saat
            // program berjalan sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/analytics/sessions/{createdSession.SessionId}",
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen bernama `body`.
            body: null,
            // Meneruskan `instructorToken` (nilai instruktur token) sebagai argumen ke `SendJsonAsync`.
            instructorToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`,
        // `analyticsResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam AddPlayersByUsername_WithoutExplicitOrder_UsesAssignmentOrder.
        Assert.Equal(HttpStatusCode.OK, analyticsResponse.StatusCode);

        // Menyiapkan variabel lokal `analytics` untuk nilai analytics dengan hasil operasi asinkron membaca tanpa argumen menjadi objek bertipe sesuai
        // kontrak JSON melalui `analyticsResponse.Content.ReadFromJsonAsync<AnalyticsSessionResponse>`; await menunggu hasil tanpa memblokir thread selama
        // operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var analytics = await analyticsResponse.Content.ReadFromJsonAsync<AnalyticsSessionResponse>();
        // Menjalankan pemeriksaan NotNull atas `analytics` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // AddPlayersByUsername_WithoutExplicitOrder_UsesAssignmentOrder.
        Assert.NotNull(analytics);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`3`, `analytics.ByPlayer.Count`); pengujian
        // gagal jika keduanya berbeda dalam AddPlayersByUsername_WithoutExplicitOrder_UsesAssignmentOrder.
        Assert.Equal(3, analytics.ByPlayer.Count);

        // Menyiapkan variabel lokal `expectedPlayerOrder` untuk nilai yang diharapkan pemain urutan/pesanan dengan array baru dengan tipe elemen
        // disimpulkan dari nilai initializer. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var expectedPlayerOrder = new[] { firstPlayer.UserId, secondPlayer.UserId, thirdPlayer.UserId };
        // Menyiapkan variabel lokal `actualPlayerOrder` untuk nilai aktual pemain urutan/pesanan dengan mematerialisasi urutan
        // `analytics.ByPlayer.Select(item => item.UserId)` menjadi array dengan elemen hasil saat ini. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var actualPlayerOrder = analytics.ByPlayer.Select(item => item.UserId).ToArray();
        // Menyiapkan variabel lokal `actualPlayerOrders` untuk nilai aktual pemain pesanan dengan mematerialisasi urutan `analytics.ByPlayer.Select(item =>
        // item.PlayerOrder)` menjadi array dengan elemen hasil saat ini. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var actualPlayerOrders = analytics.ByPlayer.Select(item => item.PlayerOrder).ToArray();

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`expectedPlayerOrder`, `actualPlayerOrder`);
        // pengujian gagal jika keduanya berbeda dalam AddPlayersByUsername_WithoutExplicitOrder_UsesAssignmentOrder.
        Assert.Equal(expectedPlayerOrder, actualPlayerOrder);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`new[] { 1, 2, 3 }`, `actualPlayerOrders`);
        // pengujian gagal jika keduanya berbeda dalam AddPlayersByUsername_WithoutExplicitOrder_UsesAssignmentOrder.
        Assert.Equal(new[] { 1, 2, 3 }, actualPlayerOrders);
    // Menutup scope metode AddPlayersByUsername_WithoutExplicitOrder_UsesAssignmentOrder; bagian berikut berada di luar batas blok tersebut dalam
    // AddPlayersByUsername_WithoutExplicitOrder_UsesAssignmentOrder.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    /// <summary>
    /// Memvalidasi bahwa penambahan pemain kelima ke sesi ditolak dengan error
    /// DOMAIN_RULE_VIOLATION karena melebihi batas maksimal 4 pemain per sesi.
    /// </summary>
    // Mendefinisikan metode `AddPlayerToSession_FifthPlayer_IsRejectedWithDomainRuleViolation` dengan hasil bertipe `Task`; operasi ini menangani add
    // pemain ke sesi fifth pemain berstatus rejected dengan domain rule violation. async memungkinkan metode menunggu operasi I/O dengan await dan
    // mengembalikan penyelesaian melalui Task.
    public async Task AddPlayerToSession_FifthPlayer_IsRejectedWithDomainRuleViolation()
    // Membuka scope metode AddPlayerToSession_FifthPlayer_IsRejectedWithDomainRuleViolation; pernyataan/deklarasi berikut berada di dalam batas blok
    // ini dalam AddPlayerToSession_FifthPlayer_IsRejectedWithDomainRuleViolation.
    {
        // Menyiapkan variabel lokal `suffix` untuk nilai suffix dengan `Guid.NewGuid().ToString(”N”)[..8]`, yaitu elemen koleksi yang dipilih melalui
        // indeks atau kunci tersebut. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var suffix = Guid.NewGuid().ToString("N")[..8];
        // Menyiapkan variabel lokal `instructorUsername` untuk nilai instruktur username dengan teks interpolasi `$”it_evt_limit_instructor_{suffix}”`;
        // nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var instructorUsername = $"it_evt_limit_instructor_{suffix}";
        // Menyiapkan variabel lokal `instructorPassword` untuk nilai instruktur password dengan nilai literal `”IntegrationLimitInstructorPass!123”`. Tipe
        // yang dipakai adalah `string`.
        const string instructorPassword = "IntegrationLimitInstructorPass!123";
        // Menyiapkan variabel lokal `instructorToken` untuk nilai instruktur token dengan `(await RegisterAsync(instructorUsername, instructorPassword,
        // ”INSTRUCTOR”)).AccessToken` (nilai akses token). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var instructorToken = (await RegisterAsync(instructorUsername, instructorPassword, "INSTRUCTOR")).AccessToken;

        // Menyiapkan variabel lokal `definition` untuk definisi terstruktur komponen serta parameter aturan permainan dengan memanggil
        // `BuildRulesetDefinition` dengan `20`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var definition = BuildRulesetDefinition(startingCash: 20);
        // Menyiapkan variabel lokal `createRulesetPayload` untuk nilai create aturan payload dengan objek anonim yang mengelompokkan name, description,
        // definition sebagai satu nilai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var createRulesetPayload = new
        // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // AddPlayerToSession_FifthPlayer_IsRejectedWithDomainRuleViolation.
        {
            // Menggunakan `name` (nilai nama) sebagai bagian ekspresi yang sedang disusun dalam
            // AddPlayerToSession_FifthPlayer_IsRejectedWithDomainRuleViolation.
            name = $"Ruleset Limit IT {suffix}",
            // Menggunakan `description` (nilai description) sebagai bagian ekspresi yang sedang disusun dalam
            // AddPlayerToSession_FifthPlayer_IsRejectedWithDomainRuleViolation.
            description = "Integration max player per session",
            // Menggunakan `definition` (definisi terstruktur komponen serta parameter aturan permainan) sebagai bagian ekspresi yang sedang disusun dalam
            // AddPlayerToSession_FifthPlayer_IsRejectedWithDomainRuleViolation.
            definition
        // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
        // AddPlayerToSession_FifthPlayer_IsRejectedWithDomainRuleViolation.
        };

        // Menyiapkan variabel lokal `createRulesetResponse` untuk nilai create aturan respons dengan hasil operasi asinkron memanggil `SendJsonAsync`
        // dengan `HttpMethod.Post`, `”/api/v1/rulesets”`, `createRulesetPayload`, `instructorToken`; await menunggu hasil tanpa memblokir thread selama
        // operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var createRulesetResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Post` (nilai post) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Post,
            // Meneruskan nilai literal `”/api/v1/rulesets”` sebagai argumen ke `SendJsonAsync`.
            "/api/v1/rulesets",
            // Meneruskan `createRulesetPayload` (nilai create aturan payload) sebagai argumen ke `SendJsonAsync`.
            createRulesetPayload,
            // Meneruskan `instructorToken` (nilai instruktur token) sebagai argumen ke `SendJsonAsync`.
            instructorToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.Created`,
        // `createRulesetResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam
        // AddPlayerToSession_FifthPlayer_IsRejectedWithDomainRuleViolation.
        Assert.Equal(HttpStatusCode.Created, createRulesetResponse.StatusCode);

        // Menyiapkan variabel lokal `createdRuleset` untuk nilai created aturan dengan hasil operasi asinkron membaca tanpa argumen menjadi objek bertipe
        // sesuai kontrak JSON melalui `createRulesetResponse.Content.ReadFromJsonAsync<CreateRulesetResponse>`; await menunggu hasil tanpa memblokir thread
        // selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var createdRuleset = await createRulesetResponse.Content.ReadFromJsonAsync<CreateRulesetResponse>();
        // Menjalankan pemeriksaan NotNull atas `createdRuleset` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // AddPlayerToSession_FifthPlayer_IsRejectedWithDomainRuleViolation.
        Assert.NotNull(createdRuleset);

        // Menyiapkan variabel lokal `createSessionPayload` untuk nilai create sesi payload dengan objek anonim yang mengelompokkan session_name, mode,
        // ruleset_version_id sebagai satu nilai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var createSessionPayload = new
        // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // AddPlayerToSession_FifthPlayer_IsRejectedWithDomainRuleViolation.
        {
            // Menggunakan `session_name` (nilai sesi nama) sebagai bagian ekspresi yang sedang disusun dalam
            // AddPlayerToSession_FifthPlayer_IsRejectedWithDomainRuleViolation.
            session_name = $"Session Limit IT {suffix}",
            // Menggunakan `mode` (mode permainan yang menentukan kelompok aturan yang digunakan) sebagai bagian ekspresi yang sedang disusun dalam
            // AddPlayerToSession_FifthPlayer_IsRejectedWithDomainRuleViolation.
            mode = "PEMULA",
            // Menggunakan `ruleset_version_id` (nilai aturan versi identitas) sebagai bagian ekspresi yang sedang disusun dalam
            // AddPlayerToSession_FifthPlayer_IsRejectedWithDomainRuleViolation.
            ruleset_version_id = createdRuleset.RulesetVersionId
        // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
        // AddPlayerToSession_FifthPlayer_IsRejectedWithDomainRuleViolation.
        };

        // Menyiapkan variabel lokal `createSessionResponse` untuk nilai create sesi respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Post`, `”/api/v1/sessions”`, `createSessionPayload`, `instructorToken`; await menunggu hasil tanpa memblokir thread selama operasi
        // belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var createSessionResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Post` (nilai post) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Post,
            // Meneruskan nilai literal `”/api/v1/sessions”` sebagai argumen ke `SendJsonAsync`.
            "/api/v1/sessions",
            // Meneruskan `createSessionPayload` (nilai create sesi payload) sebagai argumen ke `SendJsonAsync`.
            createSessionPayload,
            // Meneruskan `instructorToken` (nilai instruktur token) sebagai argumen ke `SendJsonAsync`.
            instructorToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.Created`,
        // `createSessionResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam
        // AddPlayerToSession_FifthPlayer_IsRejectedWithDomainRuleViolation.
        Assert.Equal(HttpStatusCode.Created, createSessionResponse.StatusCode);

        // Menyiapkan variabel lokal `createdSession` untuk nilai created sesi dengan hasil operasi asinkron membaca tanpa argumen menjadi objek bertipe
        // sesuai kontrak JSON melalui `createSessionResponse.Content.ReadFromJsonAsync<CreateSessionResponse>`; await menunggu hasil tanpa memblokir thread
        // selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var createdSession = await createSessionResponse.Content.ReadFromJsonAsync<CreateSessionResponse>();
        // Menjalankan pemeriksaan NotNull atas `createdSession` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // AddPlayerToSession_FifthPlayer_IsRejectedWithDomainRuleViolation.
        Assert.NotNull(createdSession);

        // Memulai loop dengan inisialisasi `var i = 1`, berjalan selama `i <= 4`, lalu memperbarui pencacah melalui `i++` dalam
        // AddPlayerToSession_FifthPlayer_IsRejectedWithDomainRuleViolation.
        for (var i = 1; i <= 4; i++)
        // Membuka scope loop dengan syarat `i <= 4`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // AddPlayerToSession_FifthPlayer_IsRejectedWithDomainRuleViolation.
        {
            // Menyiapkan variabel lokal `createPlayerPayload` untuk nilai create pemain payload dengan objek anonim yang mengelompokkan display_name, username,
            // password sebagai satu nilai. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var createPlayerPayload = new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // AddPlayerToSession_FifthPlayer_IsRejectedWithDomainRuleViolation.
            {
                // Menggunakan `display_name` (nilai display nama) sebagai bagian ekspresi yang sedang disusun dalam
                // AddPlayerToSession_FifthPlayer_IsRejectedWithDomainRuleViolation.
                display_name = $"Player Limit {i} {suffix}",
                // Menggunakan `username` (nama akun yang dipakai saat autentikasi) sebagai bagian ekspresi yang sedang disusun dalam
                // AddPlayerToSession_FifthPlayer_IsRejectedWithDomainRuleViolation.
                username = $"it_evt_limit_player_{i}_{suffix}",
                // Menggunakan `password` (kata sandi masukan yang diperiksa sesuai kebijakan autentikasi) sebagai bagian ekspresi yang sedang disusun dalam
                // AddPlayerToSession_FifthPlayer_IsRejectedWithDomainRuleViolation.
                password = "IntegrationLimitPlayerPass!123"
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
            // AddPlayerToSession_FifthPlayer_IsRejectedWithDomainRuleViolation.
            };

            // Menyiapkan variabel lokal `createPlayerResponse` untuk nilai create pemain respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
            // `HttpMethod.Post`, `”/api/v1/players”`, `createPlayerPayload`, `instructorToken`; await menunggu hasil tanpa memblokir thread selama operasi
            // belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var createPlayerResponse = await SendJsonAsync(
                // Meneruskan `HttpMethod.Post` (nilai post) sebagai argumen ke `SendJsonAsync`.
                HttpMethod.Post,
                // Meneruskan nilai literal `”/api/v1/players”` sebagai argumen ke `SendJsonAsync`.
                "/api/v1/players",
                // Meneruskan `createPlayerPayload` (nilai create pemain payload) sebagai argumen ke `SendJsonAsync`.
                createPlayerPayload,
                // Meneruskan `instructorToken` (nilai instruktur token) sebagai argumen ke `SendJsonAsync`.
                instructorToken);
            // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.Created`,
            // `createPlayerResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam AddPlayerToSession_FifthPlayer_IsRejectedWithDomainRuleViolation.
            Assert.Equal(HttpStatusCode.Created, createPlayerResponse.StatusCode);

            // Menyiapkan variabel lokal `createdPlayer` untuk nilai created pemain dengan hasil operasi asinkron membaca tanpa argumen menjadi objek bertipe
            // sesuai kontrak JSON melalui `createPlayerResponse.Content.ReadFromJsonAsync<PlayerResponse>`; await menunggu hasil tanpa memblokir thread selama
            // operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var createdPlayer = await createPlayerResponse.Content.ReadFromJsonAsync<PlayerResponse>();
            // Menjalankan pemeriksaan NotNull atas `createdPlayer` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
            // AddPlayerToSession_FifthPlayer_IsRejectedWithDomainRuleViolation.
            Assert.NotNull(createdPlayer);

            // Menyiapkan variabel lokal `addPlayerResponse` untuk nilai add pemain respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
            // `HttpMethod.Post`, `$”/api/v1/sessions/{createdSession.SessionId}/players”`, `new { user_id = createdPlayer.UserId, player_order_no = i }`,
            // `instructorToken`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var addPlayerResponse = await SendJsonAsync(
                // Meneruskan `HttpMethod.Post` (nilai post) sebagai argumen ke `SendJsonAsync`.
                HttpMethod.Post,
                // Meneruskan teks interpolasi `$”/api/v1/sessions/{createdSession.SessionId}/players”`; nilai ekspresi di dalam kurung kurawal disisipkan saat
                // program berjalan sebagai argumen ke `SendJsonAsync`.
                $"/api/v1/sessions/{createdSession.SessionId}/players",
                // Meneruskan objek anonim yang mengelompokkan user_id, player_order_no sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                new
                // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // AddPlayerToSession_FifthPlayer_IsRejectedWithDomainRuleViolation.
                {
                    // Meneruskan objek anonim yang mengelompokkan user_id, player_order_no sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                    user_id = createdPlayer.UserId,
                    // Meneruskan objek anonim yang mengelompokkan user_id, player_order_no sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                    player_order_no = i
                // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
                // AddPlayerToSession_FifthPlayer_IsRejectedWithDomainRuleViolation.
                },
                // Meneruskan `instructorToken` (nilai instruktur token) sebagai argumen ke `SendJsonAsync`.
                instructorToken);
            // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`,
            // `addPlayerResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam AddPlayerToSession_FifthPlayer_IsRejectedWithDomainRuleViolation.
            Assert.Equal(HttpStatusCode.OK, addPlayerResponse.StatusCode);
        // Menutup scope loop dengan syarat `i <= 4`; bagian berikut berada di luar batas blok tersebut dalam
        // AddPlayerToSession_FifthPlayer_IsRejectedWithDomainRuleViolation.
        }

        // Menyiapkan variabel lokal `createFifthPlayerResponse` untuk nilai create fifth pemain respons dengan hasil operasi asinkron memanggil
        // `SendJsonAsync` dengan `HttpMethod.Post`, `”/api/v1/players”`, `new { display_name = $”Player Limit 5 {suffix}”, username =
        // $”it_evt_limit_player_5_{suffix}”, password = ”IntegrationLimitPlayerPass!123” }`, `instructorToken`; await menunggu hasil tanpa memblokir thread
        // selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var createFifthPlayerResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Post` (nilai post) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Post,
            // Meneruskan nilai literal `”/api/v1/players”` sebagai argumen ke `SendJsonAsync`.
            "/api/v1/players",
            // Meneruskan objek anonim yang mengelompokkan display_name, username, password sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // AddPlayerToSession_FifthPlayer_IsRejectedWithDomainRuleViolation.
            {
                // Meneruskan objek anonim yang mengelompokkan display_name, username, password sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                display_name = $"Player Limit 5 {suffix}",
                // Meneruskan objek anonim yang mengelompokkan display_name, username, password sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                username = $"it_evt_limit_player_5_{suffix}",
                // Meneruskan objek anonim yang mengelompokkan display_name, username, password sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                password = "IntegrationLimitPlayerPass!123"
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
            // AddPlayerToSession_FifthPlayer_IsRejectedWithDomainRuleViolation.
            },
            // Meneruskan `instructorToken` (nilai instruktur token) sebagai argumen ke `SendJsonAsync`.
            instructorToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.Created`,
        // `createFifthPlayerResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam
        // AddPlayerToSession_FifthPlayer_IsRejectedWithDomainRuleViolation.
        Assert.Equal(HttpStatusCode.Created, createFifthPlayerResponse.StatusCode);

        // Menyiapkan variabel lokal `fifthPlayer` untuk nilai fifth pemain dengan hasil operasi asinkron membaca tanpa argumen menjadi objek bertipe sesuai
        // kontrak JSON melalui `createFifthPlayerResponse.Content.ReadFromJsonAsync<PlayerResponse>`; await menunggu hasil tanpa memblokir thread selama
        // operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var fifthPlayer = await createFifthPlayerResponse.Content.ReadFromJsonAsync<PlayerResponse>();
        // Menjalankan pemeriksaan NotNull atas `fifthPlayer` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // AddPlayerToSession_FifthPlayer_IsRejectedWithDomainRuleViolation.
        Assert.NotNull(fifthPlayer);

        // Menyiapkan variabel lokal `addFifthPlayerResponse` untuk nilai add fifth pemain respons dengan hasil operasi asinkron memanggil `SendJsonAsync`
        // dengan `HttpMethod.Post`, `$”/api/v1/sessions/{createdSession.SessionId}/players”`, `new { user_id = fifthPlayer.UserId, player_order_no = 5 }`,
        // `instructorToken`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var addFifthPlayerResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Post` (nilai post) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Post,
            // Meneruskan teks interpolasi `$”/api/v1/sessions/{createdSession.SessionId}/players”`; nilai ekspresi di dalam kurung kurawal disisipkan saat
            // program berjalan sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/sessions/{createdSession.SessionId}/players",
            // Meneruskan objek anonim yang mengelompokkan user_id, player_order_no sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // AddPlayerToSession_FifthPlayer_IsRejectedWithDomainRuleViolation.
            {
                // Meneruskan objek anonim yang mengelompokkan user_id, player_order_no sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                user_id = fifthPlayer.UserId,
                // Meneruskan objek anonim yang mengelompokkan user_id, player_order_no sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                player_order_no = 5
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
            // AddPlayerToSession_FifthPlayer_IsRejectedWithDomainRuleViolation.
            },
            // Meneruskan `instructorToken` (nilai instruktur token) sebagai argumen ke `SendJsonAsync`.
            instructorToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.UnprocessableEntity`,
        // `addFifthPlayerResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam
        // AddPlayerToSession_FifthPlayer_IsRejectedWithDomainRuleViolation.
        Assert.Equal(HttpStatusCode.UnprocessableEntity, addFifthPlayerResponse.StatusCode);

        // Menyiapkan variabel lokal `addFifthPlayerError` untuk nilai add fifth pemain kesalahan dengan hasil operasi asinkron membaca tanpa argumen
        // menjadi objek bertipe sesuai kontrak JSON melalui `addFifthPlayerResponse.Content.ReadFromJsonAsync<ErrorResponse>`; await menunggu hasil tanpa
        // memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var addFifthPlayerError = await addFifthPlayerResponse.Content.ReadFromJsonAsync<ErrorResponse>();
        // Menjalankan pemeriksaan NotNull atas `addFifthPlayerError` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // AddPlayerToSession_FifthPlayer_IsRejectedWithDomainRuleViolation.
        Assert.NotNull(addFifthPlayerError);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”DOMAIN_RULE_VIOLATION”`,
        // `addFifthPlayerError.ErrorCode`); pengujian gagal jika keduanya berbeda dalam AddPlayerToSession_FifthPlayer_IsRejectedWithDomainRuleViolation.
        Assert.Equal("DOMAIN_RULE_VIOLATION", addFifthPlayerError.ErrorCode);

        // Menyiapkan variabel lokal `saveSetupResponse` untuk nilai save setup respons dengan hasil operasi asinkron memanggil
        // `SessionSetupTestHelper.SaveAsync` dengan `_client`, `instructorToken`, `createdSession.SessionId`, `definition`,
        // `TestContext.Current.CancellationToken`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari
        // ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var saveSetupResponse = await SessionSetupTestHelper.SaveAsync(
            // Meneruskan `_client` (nilai client) sebagai argumen ke `SessionSetupTestHelper.SaveAsync`.
            _client,
            // Meneruskan `instructorToken` (nilai instruktur token) sebagai argumen ke `SessionSetupTestHelper.SaveAsync`.
            instructorToken,
            // Meneruskan `createdSession.SessionId` (identitas unik sesi permainan yang menjadi batas data operasi ini) sebagai argumen ke
            // `SessionSetupTestHelper.SaveAsync`.
            createdSession.SessionId,
            // Meneruskan `definition` (definisi terstruktur komponen serta parameter aturan permainan) sebagai argumen ke `SessionSetupTestHelper.SaveAsync`.
            definition,
            // Meneruskan `TestContext.Current.CancellationToken` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
            // aplikasi berhenti) sebagai argumen ke `SessionSetupTestHelper.SaveAsync`.
            TestContext.Current.CancellationToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.Created`,
        // `saveSetupResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam AddPlayerToSession_FifthPlayer_IsRejectedWithDomainRuleViolation.
        Assert.Equal(HttpStatusCode.Created, saveSetupResponse.StatusCode);

        // Menyiapkan variabel lokal `startSessionResponse` untuk nilai start sesi respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Post`, `$”/api/v1/sessions/{createdSession.SessionId}/start”`, `null`, `instructorToken`; await menunggu hasil tanpa memblokir thread
        // selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var startSessionResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Post` (nilai post) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Post,
            // Meneruskan teks interpolasi `$”/api/v1/sessions/{createdSession.SessionId}/start”`; nilai ekspresi di dalam kurung kurawal disisipkan saat
            // program berjalan sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/sessions/{createdSession.SessionId}/start",
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen bernama `body`.
            body: null,
            // Meneruskan `instructorToken` (nilai instruktur token) sebagai argumen ke `SendJsonAsync`.
            instructorToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`,
        // `startSessionResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam AddPlayerToSession_FifthPlayer_IsRejectedWithDomainRuleViolation.
        Assert.Equal(HttpStatusCode.OK, startSessionResponse.StatusCode);
    // Menutup scope metode AddPlayerToSession_FifthPlayer_IsRejectedWithDomainRuleViolation; bagian berikut berada di luar batas blok tersebut dalam
    // AddPlayerToSession_FifthPlayer_IsRejectedWithDomainRuleViolation.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    /// <summary>
    /// Memvalidasi bahwa path API tanpa versi (/api/...) mengembalikan 404 Not Found
    /// dan path versioned (/api/v1/...) berfungsi dengan benar.
    /// </summary>
    // Mendefinisikan metode `UnversionedApiRoute_ReturnsNotFound_AndV1AuthWorks` dengan hasil bertipe `Task`; operasi ini menangani unversioned api
    // route returns not found dan 1 auth works. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui
    // Task.
    public async Task UnversionedApiRoute_ReturnsNotFound_AndV1AuthWorks()
    // Membuka scope metode UnversionedApiRoute_ReturnsNotFound_AndV1AuthWorks; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // UnversionedApiRoute_ReturnsNotFound_AndV1AuthWorks.
    {
        // Menyiapkan variabel lokal `suffix` untuk nilai suffix dengan `Guid.NewGuid().ToString(”N”)[..8]`, yaitu elemen koleksi yang dipilih melalui
        // indeks atau kunci tersebut. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var suffix = Guid.NewGuid().ToString("N")[..8];
        // Menyiapkan variabel lokal `username` untuk nama akun yang dipakai saat autentikasi dengan teks interpolasi `$”it_unversioned_{suffix}”`; nilai
        // ekspresi di dalam kurung kurawal disisipkan saat program berjalan. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var username = $"it_unversioned_{suffix}";
        // Menyiapkan variabel lokal `password` untuk kata sandi masukan yang diperiksa sesuai kebijakan autentikasi dengan nilai literal
        // `”UnversionedRoutePass!123”`. Tipe yang dipakai adalah `string`.
        const string password = "UnversionedRoutePass!123";

        // Menyiapkan variabel lokal `registerPayload` untuk nilai register payload dengan objek baru bertipe `RegisterRequest` dengan argumen (username,
        // password, ”INSTRUCTOR”, null). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var registerPayload = new RegisterRequest(username, password, "INSTRUCTOR", null);
        // Menyiapkan variabel lokal `unversionedRegisterResponse` untuk nilai unversioned register respons dengan hasil operasi asinkron memanggil
        // `_client.PostAsJsonAsync` dengan `”/api/auth/register”`, `registerPayload`; await menunggu hasil tanpa memblokir thread selama operasi belum
        // selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var unversionedRegisterResponse = await _client.PostAsJsonAsync("/api/auth/register", registerPayload);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.NotFound`,
        // `unversionedRegisterResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam UnversionedApiRoute_ReturnsNotFound_AndV1AuthWorks.
        Assert.Equal(HttpStatusCode.NotFound, unversionedRegisterResponse.StatusCode);

        // Menyiapkan variabel lokal `registerResponse` untuk nilai register respons dengan hasil operasi asinkron memanggil `_client.PostAsJsonAsync`
        // dengan `”/api/v1/auth/register”`, `registerPayload`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var registerResponse = await _client.PostAsJsonAsync("/api/v1/auth/register", registerPayload);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.Created`,
        // `registerResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam UnversionedApiRoute_ReturnsNotFound_AndV1AuthWorks.
        Assert.Equal(HttpStatusCode.Created, registerResponse.StatusCode);

        // Menyiapkan variabel lokal `loginPayload` untuk nilai login payload dengan objek baru bertipe `LoginRequest` dengan argumen (username, password).
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var loginPayload = new LoginRequest(username, password);
        // Menyiapkan variabel lokal `loginResponse` untuk nilai login respons dengan hasil operasi asinkron memanggil `_client.PostAsJsonAsync` dengan
        // `”/api/v1/auth/login”`, `loginPayload`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var loginResponse = await _client.PostAsJsonAsync("/api/v1/auth/login", loginPayload);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`,
        // `loginResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam UnversionedApiRoute_ReturnsNotFound_AndV1AuthWorks.
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        // Menyiapkan variabel lokal `login` untuk nilai login dengan hasil operasi asinkron membaca tanpa argumen menjadi objek bertipe sesuai kontrak JSON
        // melalui `loginResponse.Content.ReadFromJsonAsync<LoginResponse>`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var login = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();
        // Menjalankan pemeriksaan NotNull atas `login` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // UnversionedApiRoute_ReturnsNotFound_AndV1AuthWorks.
        Assert.NotNull(login);
        // Menjalankan pemeriksaan bahwa `string.IsNullOrWhiteSpace(login.AccessToken)` bernilai salah; pengujian gagal jika kondisi justru terpenuhi dalam
        // UnversionedApiRoute_ReturnsNotFound_AndV1AuthWorks.
        Assert.False(string.IsNullOrWhiteSpace(login.AccessToken));

        // Menyiapkan variabel lokal `unversionedSessionsResponse` untuk nilai unversioned sessions respons dengan hasil operasi asinkron memanggil
        // `SendJsonAsync` dengan `HttpMethod.Get`, `”/api/sessions”`, `null`, `login.AccessToken`; await menunggu hasil tanpa memblokir thread selama
        // operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var unversionedSessionsResponse = await SendJsonAsync(HttpMethod.Get, "/api/sessions", null, login.AccessToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.NotFound`,
        // `unversionedSessionsResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam UnversionedApiRoute_ReturnsNotFound_AndV1AuthWorks.
        Assert.Equal(HttpStatusCode.NotFound, unversionedSessionsResponse.StatusCode);

        // Menyiapkan variabel lokal `sessionsResponse` untuk nilai sessions respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Get`, `”/api/v1/sessions”`, `null`, `login.AccessToken`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai.
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var sessionsResponse = await SendJsonAsync(HttpMethod.Get, "/api/v1/sessions", null, login.AccessToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`,
        // `sessionsResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam UnversionedApiRoute_ReturnsNotFound_AndV1AuthWorks.
        Assert.Equal(HttpStatusCode.OK, sessionsResponse.StatusCode);
    // Menutup scope metode UnversionedApiRoute_ReturnsNotFound_AndV1AuthWorks; bagian berikut berada di luar batas blok tersebut dalam
    // UnversionedApiRoute_ReturnsNotFound_AndV1AuthWorks.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    /// <summary>
    /// Memvalidasi bahwa ingest event dengan payload tidak valid (amount bukan angka,
    /// day_index negatif) dan query parameter tidak valid mengembalikan 400 Bad Request.
    /// </summary>
    // Mendefinisikan metode `IngestEvent_InvalidPayloadAndQuery_ReturnsBadRequest` dengan hasil bertipe `Task`; operasi ini menangani ingest event
    // invalid payload dan query returns bad permintaan. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian
    // melalui Task.
    public async Task IngestEvent_InvalidPayloadAndQuery_ReturnsBadRequest()
    // Membuka scope metode IngestEvent_InvalidPayloadAndQuery_ReturnsBadRequest; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // IngestEvent_InvalidPayloadAndQuery_ReturnsBadRequest.
    {
        // Menyiapkan variabel lokal `suffix` untuk nilai suffix dengan `Guid.NewGuid().ToString(”N”)[..8]`, yaitu elemen koleksi yang dipilih melalui
        // indeks atau kunci tersebut. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var suffix = Guid.NewGuid().ToString("N")[..8];
        // Menyiapkan variabel lokal `instructorUsername` untuk nilai instruktur username dengan teks interpolasi `$”it_evt_invalid_instr_{suffix}”`; nilai
        // ekspresi di dalam kurung kurawal disisipkan saat program berjalan. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var instructorUsername = $"it_evt_invalid_instr_{suffix}";
        // Menyiapkan variabel lokal `instructorPassword` untuk nilai instruktur password dengan nilai literal `”IntegrationInvalidInstructorPass!123”`.
        // Tipe yang dipakai adalah `string`.
        const string instructorPassword = "IntegrationInvalidInstructorPass!123";
        // Menyiapkan variabel lokal `instructorToken` untuk nilai instruktur token dengan `(await RegisterAsync(instructorUsername, instructorPassword,
        // ”INSTRUCTOR”)).AccessToken` (nilai akses token). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var instructorToken = (await RegisterAsync(instructorUsername, instructorPassword, "INSTRUCTOR")).AccessToken;
        // Menyiapkan variabel lokal `setup` untuk nilai setup dengan hasil operasi asinkron memanggil `CreateReadySessionAsync` dengan `instructorToken`,
        // `suffix`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var setup = await CreateReadySessionAsync(instructorToken, suffix);

        // Menyiapkan variabel lokal `now` untuk nilai now dengan `DateTimeOffset.UtcNow`, yaitu waktu UTC saat operasi dilakukan. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var now = DateTimeOffset.UtcNow;
        // Menyiapkan variabel lokal `invalidAmountPayload` untuk nilai invalid nominal payload dengan objek anonim yang mengelompokkan event_id,
        // session_id, user_id, actor_type, timestamp, day_index, weekday, action_slot, turn_number, sequence_number, action_type, ruleset_version_id,
        // payload sebagai satu nilai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var invalidAmountPayload = new
        // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // IngestEvent_InvalidPayloadAndQuery_ReturnsBadRequest.
        {
            // Menggunakan `event_id` (nilai event identitas) sebagai bagian ekspresi yang sedang disusun dalam
            // IngestEvent_InvalidPayloadAndQuery_ReturnsBadRequest.
            event_id = Guid.NewGuid(),
            // Menggunakan `session_id` (nilai sesi identitas) sebagai bagian ekspresi yang sedang disusun dalam
            // IngestEvent_InvalidPayloadAndQuery_ReturnsBadRequest.
            session_id = setup.SessionId,
            // Menggunakan `user_id` (nilai pengguna identitas) sebagai bagian ekspresi yang sedang disusun dalam
            // IngestEvent_InvalidPayloadAndQuery_ReturnsBadRequest.
            user_id = setup.ActingUserId,
            // Menggunakan `actor_type` (nilai actor jenis) sebagai bagian ekspresi yang sedang disusun dalam
            // IngestEvent_InvalidPayloadAndQuery_ReturnsBadRequest.
            actor_type = "SYSTEM",
            // Menggunakan `timestamp` (waktu kejadian yang menjaga urutan kronologis data) sebagai bagian ekspresi yang sedang disusun dalam
            // IngestEvent_InvalidPayloadAndQuery_ReturnsBadRequest.
            timestamp = now.ToString("O"),
            // Menggunakan `day_index` (nilai hari index) sebagai bagian ekspresi yang sedang disusun dalam
            // IngestEvent_InvalidPayloadAndQuery_ReturnsBadRequest.
            day_index = 1,
            // Menggunakan `weekday` (nilai weekday) sebagai bagian ekspresi yang sedang disusun dalam IngestEvent_InvalidPayloadAndQuery_ReturnsBadRequest.
            weekday = "MON",
            // Menggunakan `action_slot` (nilai aksi slot) sebagai bagian ekspresi yang sedang disusun dalam
            // IngestEvent_InvalidPayloadAndQuery_ReturnsBadRequest.
            action_slot = 0,
            // Menggunakan `turn_number` (nilai giliran number) sebagai bagian ekspresi yang sedang disusun dalam
            // IngestEvent_InvalidPayloadAndQuery_ReturnsBadRequest.
            turn_number = 0,
            // Menggunakan `sequence_number` (nilai sequence number) sebagai bagian ekspresi yang sedang disusun dalam
            // IngestEvent_InvalidPayloadAndQuery_ReturnsBadRequest.
            sequence_number = setup.NextSequenceNumber,
            // Menggunakan `action_type` (nilai aksi jenis) sebagai bagian ekspresi yang sedang disusun dalam
            // IngestEvent_InvalidPayloadAndQuery_ReturnsBadRequest.
            action_type = "CatatTransaksi",
            // Menggunakan `ruleset_version_id` (nilai aturan versi identitas) sebagai bagian ekspresi yang sedang disusun dalam
            // IngestEvent_InvalidPayloadAndQuery_ReturnsBadRequest.
            ruleset_version_id = setup.RulesetVersionId,
            // Menggunakan `payload` (muatan detail event dalam format JSON) sebagai bagian ekspresi yang sedang disusun dalam
            // IngestEvent_InvalidPayloadAndQuery_ReturnsBadRequest.
            payload = new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // IngestEvent_InvalidPayloadAndQuery_ReturnsBadRequest.
            {
                // Menggunakan `direction` (nilai direction) sebagai bagian ekspresi yang sedang disusun dalam IngestEvent_InvalidPayloadAndQuery_ReturnsBadRequest.
                direction = "IN",
                // Menggunakan `amount` (nominal uang atau nilai transaksi yang dipakai dalam operasi) sebagai bagian ekspresi yang sedang disusun dalam
                // IngestEvent_InvalidPayloadAndQuery_ReturnsBadRequest.
                amount = "invalid-number",
                // Menggunakan `category` (nilai category) sebagai bagian ekspresi yang sedang disusun dalam IngestEvent_InvalidPayloadAndQuery_ReturnsBadRequest.
                category = "NEED_PRIMARY"
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
            // IngestEvent_InvalidPayloadAndQuery_ReturnsBadRequest.
            }
        // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
        // IngestEvent_InvalidPayloadAndQuery_ReturnsBadRequest.
        };

        // Menyiapkan variabel lokal `invalidAmountResponse` untuk nilai invalid nominal respons dengan hasil operasi asinkron memanggil `SendJsonAsync`
        // dengan `HttpMethod.Post`, `”/api/v1/events”`, `invalidAmountPayload`, `instructorToken`; await menunggu hasil tanpa memblokir thread selama
        // operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var invalidAmountResponse = await SendJsonAsync(HttpMethod.Post, "/api/v1/events", invalidAmountPayload, instructorToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.BadRequest`,
        // `invalidAmountResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam IngestEvent_InvalidPayloadAndQuery_ReturnsBadRequest.
        Assert.Equal(HttpStatusCode.BadRequest, invalidAmountResponse.StatusCode);

        // Menyiapkan variabel lokal `invalidDayIndexPayload` untuk nilai invalid hari index payload dengan objek anonim yang mengelompokkan event_id,
        // session_id, user_id, actor_type, timestamp, day_index, weekday, action_slot, turn_number, sequence_number, action_type, ruleset_version_id,
        // payload sebagai satu nilai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var invalidDayIndexPayload = new
        // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // IngestEvent_InvalidPayloadAndQuery_ReturnsBadRequest.
        {
            // Menggunakan `event_id` (nilai event identitas) sebagai bagian ekspresi yang sedang disusun dalam
            // IngestEvent_InvalidPayloadAndQuery_ReturnsBadRequest.
            event_id = Guid.NewGuid(),
            // Menggunakan `session_id` (nilai sesi identitas) sebagai bagian ekspresi yang sedang disusun dalam
            // IngestEvent_InvalidPayloadAndQuery_ReturnsBadRequest.
            session_id = setup.SessionId,
            // Menggunakan `user_id` (nilai pengguna identitas) sebagai bagian ekspresi yang sedang disusun dalam
            // IngestEvent_InvalidPayloadAndQuery_ReturnsBadRequest.
            user_id = setup.ActingUserId,
            // Menggunakan `actor_type` (nilai actor jenis) sebagai bagian ekspresi yang sedang disusun dalam
            // IngestEvent_InvalidPayloadAndQuery_ReturnsBadRequest.
            actor_type = "SYSTEM",
            // Menggunakan `timestamp` (waktu kejadian yang menjaga urutan kronologis data) sebagai bagian ekspresi yang sedang disusun dalam
            // IngestEvent_InvalidPayloadAndQuery_ReturnsBadRequest.
            timestamp = now.AddSeconds(1).ToString("O"),
            // Menggunakan `day_index` (nilai hari index) sebagai bagian ekspresi yang sedang disusun dalam
            // IngestEvent_InvalidPayloadAndQuery_ReturnsBadRequest.
            day_index = -1,
            // Menggunakan `weekday` (nilai weekday) sebagai bagian ekspresi yang sedang disusun dalam IngestEvent_InvalidPayloadAndQuery_ReturnsBadRequest.
            weekday = "MON",
            // Menggunakan `action_slot` (nilai aksi slot) sebagai bagian ekspresi yang sedang disusun dalam
            // IngestEvent_InvalidPayloadAndQuery_ReturnsBadRequest.
            action_slot = 0,
            // Menggunakan `turn_number` (nilai giliran number) sebagai bagian ekspresi yang sedang disusun dalam
            // IngestEvent_InvalidPayloadAndQuery_ReturnsBadRequest.
            turn_number = 0,
            // Menggunakan `sequence_number` (nilai sequence number) sebagai bagian ekspresi yang sedang disusun dalam
            // IngestEvent_InvalidPayloadAndQuery_ReturnsBadRequest.
            sequence_number = setup.NextSequenceNumber,
            // Menggunakan `action_type` (nilai aksi jenis) sebagai bagian ekspresi yang sedang disusun dalam
            // IngestEvent_InvalidPayloadAndQuery_ReturnsBadRequest.
            action_type = "CatatTransaksi",
            // Menggunakan `ruleset_version_id` (nilai aturan versi identitas) sebagai bagian ekspresi yang sedang disusun dalam
            // IngestEvent_InvalidPayloadAndQuery_ReturnsBadRequest.
            ruleset_version_id = setup.RulesetVersionId,
            // Menggunakan `payload` (muatan detail event dalam format JSON) sebagai bagian ekspresi yang sedang disusun dalam
            // IngestEvent_InvalidPayloadAndQuery_ReturnsBadRequest.
            payload = new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // IngestEvent_InvalidPayloadAndQuery_ReturnsBadRequest.
            {
                // Menggunakan `direction` (nilai direction) sebagai bagian ekspresi yang sedang disusun dalam IngestEvent_InvalidPayloadAndQuery_ReturnsBadRequest.
                direction = "IN",
                // Menggunakan `amount` (nominal uang atau nilai transaksi yang dipakai dalam operasi) sebagai bagian ekspresi yang sedang disusun dalam
                // IngestEvent_InvalidPayloadAndQuery_ReturnsBadRequest.
                amount = 1,
                // Menggunakan `category` (nilai category) sebagai bagian ekspresi yang sedang disusun dalam IngestEvent_InvalidPayloadAndQuery_ReturnsBadRequest.
                category = "NEED_PRIMARY"
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
            // IngestEvent_InvalidPayloadAndQuery_ReturnsBadRequest.
            }
        // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
        // IngestEvent_InvalidPayloadAndQuery_ReturnsBadRequest.
        };

        // Menyiapkan variabel lokal `invalidDayResponse` untuk nilai invalid hari respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Post`, `”/api/v1/events”`, `invalidDayIndexPayload`, `instructorToken`; await menunggu hasil tanpa memblokir thread selama operasi
        // belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var invalidDayResponse = await SendJsonAsync(HttpMethod.Post, "/api/v1/events", invalidDayIndexPayload, instructorToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.BadRequest`,
        // `invalidDayResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam IngestEvent_InvalidPayloadAndQuery_ReturnsBadRequest.
        Assert.Equal(HttpStatusCode.BadRequest, invalidDayResponse.StatusCode);

        // Menyiapkan variabel lokal `futureDayResponse` untuk nilai future hari respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Post`, `”/api/v1/events”`, `new { event_id = Guid.NewGuid(), session_id = setup.SessionId, user_id = setup.ActingUserId, actor_type =
        // ”PLAYER”, timestamp = now.AddSeconds(2), day_index = 2, weekday = ”TU...`, `instructorToken`; await menunggu hasil tanpa memblokir thread selama
        // operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var futureDayResponse = await SendJsonAsync(HttpMethod.Post, "/api/v1/events", new
        // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // IngestEvent_InvalidPayloadAndQuery_ReturnsBadRequest.
        {
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            event_id = Guid.NewGuid(),
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            session_id = setup.SessionId,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            user_id = setup.ActingUserId,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            actor_type = "PLAYER",
            // Meneruskan nilai literal `2` sebagai argumen ke `now.AddSeconds`.
            timestamp = now.AddSeconds(2),
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            day_index = 2,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            weekday = "TUE",
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            turn_number = 1,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            action_slot = 1,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            sequence_number = setup.NextSequenceNumber,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            action_type = "KerjaLepas",
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            ruleset_version_id = setup.RulesetVersionId,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            payload = new { amount = 1 }
        // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
        // IngestEvent_InvalidPayloadAndQuery_ReturnsBadRequest.
        }, instructorToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.UnprocessableEntity`,
        // `futureDayResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam IngestEvent_InvalidPayloadAndQuery_ReturnsBadRequest.
        Assert.Equal(HttpStatusCode.UnprocessableEntity, futureDayResponse.StatusCode);

        // Menyiapkan variabel lokal `arbitraryTransactionResponse` untuk nilai arbitrary transaction respons dengan hasil operasi asinkron memanggil
        // `SendJsonAsync` dengan `HttpMethod.Post`, `”/api/v1/events”`, `new { event_id = Guid.NewGuid(), session_id = setup.SessionId, user_id =
        // setup.ActingUserId, actor_type = ”PLAYER”, timestamp = now.AddSeconds(3), day_index = 1, weekday = ”MO...`, `instructorToken`; await menunggu
        // hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya
        // dilepas otomatis saat scope berakhir.
        using var arbitraryTransactionResponse = await SendJsonAsync(HttpMethod.Post, "/api/v1/events", new
        // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // IngestEvent_InvalidPayloadAndQuery_ReturnsBadRequest.
        {
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            event_id = Guid.NewGuid(),
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            session_id = setup.SessionId,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            user_id = setup.ActingUserId,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            actor_type = "PLAYER",
            // Meneruskan nilai literal `3` sebagai argumen ke `now.AddSeconds`.
            timestamp = now.AddSeconds(3),
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            day_index = 1,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            weekday = "MON",
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            turn_number = 1,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            action_slot = 1,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            sequence_number = setup.NextSequenceNumber,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            action_type = "CatatTransaksi",
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            ruleset_version_id = setup.RulesetVersionId,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            payload = new { direction = "IN", amount = 999, category = "CUSTOM" }
        // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
        // IngestEvent_InvalidPayloadAndQuery_ReturnsBadRequest.
        }, instructorToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.BadRequest`,
        // `arbitraryTransactionResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam IngestEvent_InvalidPayloadAndQuery_ReturnsBadRequest.
        Assert.Equal(HttpStatusCode.BadRequest, arbitraryTransactionResponse.StatusCode);

        // Menyiapkan variabel lokal `invalidQueryResponse` untuk nilai invalid query respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Get`, `$”/api/v1/sessions/{setup.SessionId}/events?limit=-1”`, `null`, `instructorToken`; await menunggu hasil tanpa memblokir thread
        // selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var invalidQueryResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Get` (nilai get) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Get,
            // Meneruskan teks interpolasi `$”/api/v1/sessions/{setup.SessionId}/events?limit=-1”`; nilai ekspresi di dalam kurung kurawal disisipkan saat
            // program berjalan sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/sessions/{setup.SessionId}/events?limit=-1",
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen ke `SendJsonAsync`.
            null,
            // Meneruskan `instructorToken` (nilai instruktur token) sebagai argumen ke `SendJsonAsync`.
            instructorToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.BadRequest`,
        // `invalidQueryResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam IngestEvent_InvalidPayloadAndQuery_ReturnsBadRequest.
        Assert.Equal(HttpStatusCode.BadRequest, invalidQueryResponse.StatusCode);
    // Menutup scope metode IngestEvent_InvalidPayloadAndQuery_ReturnsBadRequest; bagian berikut berada di luar batas blok tersebut dalam
    // IngestEvent_InvalidPayloadAndQuery_ReturnsBadRequest.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `FridayDonations_RemainSealedUntilEveryPlayerSubmits` dengan hasil bertipe `Task`; operasi ini menangani friday donations
    // remain sealed until every pemain submits. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui
    // Task.
    public async Task FridayDonations_RemainSealedUntilEveryPlayerSubmits()
    // Membuka scope metode FridayDonations_RemainSealedUntilEveryPlayerSubmits; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // FridayDonations_RemainSealedUntilEveryPlayerSubmits.
    {
        // Menyiapkan variabel lokal `suffix` untuk nilai suffix dengan `Guid.NewGuid().ToString(”N”)[..8]`, yaitu elemen koleksi yang dipilih melalui
        // indeks atau kunci tersebut. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var suffix = Guid.NewGuid().ToString("N")[..8];
        // Menyiapkan variabel lokal `instructorToken` untuk nilai instruktur token dengan `(await RegisterAsync( $”it_donation_instructor_{suffix}”,
        // ”IntegrationDonationInstructorPass!123”, ”INSTRUCTOR”)).AccessToken` (nilai akses token). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var instructorToken = (await RegisterAsync(
            // Meneruskan teks interpolasi `$”it_donation_instructor_{suffix}”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan sebagai
            // argumen ke `RegisterAsync`.
            $"it_donation_instructor_{suffix}",
            // Meneruskan nilai literal `”IntegrationDonationInstructorPass!123”` sebagai argumen ke `RegisterAsync`.
            "IntegrationDonationInstructorPass!123",
            // Meneruskan nilai literal `”INSTRUCTOR”` sebagai argumen ke `RegisterAsync`.
            "INSTRUCTOR")).AccessToken;
        // Menyiapkan variabel lokal `setup` untuk nilai setup dengan hasil operasi asinkron memanggil `CreateReadySessionAsync` dengan `instructorToken`,
        // `$”donation_{suffix}”`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai
        // awal.
        var setup = await CreateReadySessionAsync(instructorToken, $"donation_{suffix}");
        // Menyiapkan variabel lokal `donationSequence` untuk nilai donasi sequence dengan hasil operasi asinkron memanggil `AdvanceSessionToDayAsync`
        // dengan `setup`, `instructorToken`, `5`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var donationSequence = await AdvanceSessionToDayAsync(setup, instructorToken, targetDay: 5);

        // Mendefinisikan fungsi lokal GetEventsAsync dengan hasil `Task<List<JsonElement>>`; fungsi ini dipakai oleh alur di dalam scope yang sama.
        async Task<List<JsonElement>> GetEventsAsync()
        // Membuka scope fungsi lokal GetEventsAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam GetEventsAsync.
        {
            // Menyiapkan variabel lokal `response` untuk hasil respons yang akan dibaca atau dikirim kepada pemanggil dengan hasil operasi asinkron memanggil
            // `SendJsonAsync` dengan `HttpMethod.Get`, `$”/api/v1/sessions/{setup.SessionId}/events?limit=100”`, `null`, `instructorToken`; await menunggu
            // hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya
            // dilepas otomatis saat scope berakhir.
            using var response = await SendJsonAsync(
                // Meneruskan `HttpMethod.Get` (nilai get) sebagai argumen ke `SendJsonAsync`.
                HttpMethod.Get,
                // Meneruskan teks interpolasi `$”/api/v1/sessions/{setup.SessionId}/events?limit=100”`; nilai ekspresi di dalam kurung kurawal disisipkan saat
                // program berjalan sebagai argumen ke `SendJsonAsync`.
                $"/api/v1/sessions/{setup.SessionId}/events?limit=100",
                // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen ke `SendJsonAsync`.
                null,
                // Meneruskan `instructorToken` (nilai instruktur token) sebagai argumen ke `SendJsonAsync`.
                instructorToken);
            // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`, `response.StatusCode`);
            // pengujian gagal jika keduanya berbeda dalam GetEventsAsync.
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            // Menyiapkan variabel lokal `body` untuk nilai body dengan hasil operasi asinkron memanggil `ReadJsonAsync` dengan `response`; await menunggu hasil
            // tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas
            // otomatis saat scope berakhir.
            using var body = await ReadJsonAsync(response);
            // Mengembalikan mematerialisasi urutan `body.RootElement.GetProperty(”items”) .EnumerateArray() .Select(item => item.Clone())` menjadi List;
            // enumerasi dijalankan dan hasilnya disimpan dalam memori kepada pemanggil dalam GetEventsAsync; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return body.RootElement.GetProperty("items")
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .EnumerateArray() dalam GetEventsAsync; token pada baris ini menyambungkan
                // bagian kode sebelum dan sesudahnya.
                .EnumerateArray()
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Select(item => item.Clone()) dalam GetEventsAsync; token pada baris ini
                // menyambungkan bagian kode sebelum dan sesudahnya.
                .Select(item => item.Clone())
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList(); dalam GetEventsAsync; token pada baris ini menyambungkan bagian kode
                // sebelum dan sesudahnya.
                .ToList();
        // Menutup scope fungsi lokal GetEventsAsync; bagian berikut berada di luar batas blok tersebut dalam GetEventsAsync.
        }

        // Menyiapkan variabel lokal `setupEvents` untuk nilai setup event dengan hasil operasi asinkron memanggil `GetEventsAsync` dengan tanpa argumen;
        // await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var setupEvents = await GetEventsAsync();
        // Menyiapkan variabel lokal `players` untuk nilai pemain dengan mematerialisasi urutan `setupEvents .Where(item =>
        // item.GetProperty(”action_type”).GetString() == ”BagikanTieBreaker”) .OrderBy(item =>
        // item.GetProperty(”payload”).GetProperty(”number”).GetInt32()) ....` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var players = setupEvents
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(item => item.GetProperty(”action_type”).GetString() ==
            // ”BagikanTieBreaker”) dalam FridayDonations_RemainSealedUntilEveryPlayerSubmits; token pada baris ini menyambungkan bagian kode sebelum dan
            // sesudahnya.
            .Where(item => item.GetProperty("action_type").GetString() == "BagikanTieBreaker")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .OrderBy(item => item.GetProperty(”payload”).GetProperty(”number”).GetInt32())
            // dalam FridayDonations_RemainSealedUntilEveryPlayerSubmits; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .OrderBy(item => item.GetProperty("payload").GetProperty("number").GetInt32())
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Select(item => ( dalam FridayDonations_RemainSealedUntilEveryPlayerSubmits;
            // token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Select(item => (
                // Meneruskan memanggil `item.GetProperty(”user_id”).GetGuid` dengan tanpa argumen sebagai argumen bernama `UserId`; Meneruskan nilai literal
                // `”user_id”` sebagai argumen ke `item.GetProperty`.
                UserId: item.GetProperty("user_id").GetGuid(),
                // Meneruskan memanggil `item.GetProperty(”payload”).GetProperty(”number”).GetInt32` dengan tanpa argumen sebagai argumen bernama `Turn`; Meneruskan
                // nilai literal `”payload”` sebagai argumen ke `item.GetProperty`; Meneruskan nilai literal `”number”` sebagai argumen ke
                // `item.GetProperty(”payload”).GetProperty`.
                Turn: item.GetProperty("payload").GetProperty("number").GetInt32()))
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList(); dalam FridayDonations_RemainSealedUntilEveryPlayerSubmits; token
            // pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .ToList();
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`3`, `players.Count`); pengujian gagal jika
        // keduanya berbeda dalam FridayDonations_RemainSealedUntilEveryPlayerSubmits.
        Assert.Equal(3, players.Count);

        // Memulai loop dengan inisialisasi `var index = 0`, berjalan selama `index < players.Count`, lalu memperbarui pencacah melalui `index++` dalam
        // FridayDonations_RemainSealedUntilEveryPlayerSubmits.
        for (var index = 0; index < players.Count; index++)
        // Membuka scope loop dengan syarat `index < players.Count`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // FridayDonations_RemainSealedUntilEveryPlayerSubmits.
        {
            // Menyiapkan variabel lokal `player` untuk nilai pemain dengan `players[index]`, yaitu elemen koleksi yang dipilih melalui indeks atau kunci
            // tersebut. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var player = players[index];
            // Menyiapkan variabel lokal `donationResponse` untuk nilai donasi respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
            // `HttpMethod.Post`, `”/api/v1/events”`, `new { event_id = Guid.NewGuid(), session_id = setup.SessionId, user_id = player.UserId, actor_type =
            // ”PLAYER”, timestamp = DateTimeOffset.UtcNow.AddSeconds(index), day_index =...`, `instructorToken`; await menunggu hasil tanpa memblokir thread
            // selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope
            // berakhir.
            using var donationResponse = await SendJsonAsync(HttpMethod.Post, "/api/v1/events", new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // FridayDonations_RemainSealedUntilEveryPlayerSubmits.
            {
                // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                event_id = Guid.NewGuid(),
                // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                session_id = setup.SessionId,
                // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                user_id = player.UserId,
                // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                actor_type = "PLAYER",
                // Meneruskan `index` (nilai index) sebagai argumen ke `DateTimeOffset.UtcNow.AddSeconds`.
                timestamp = DateTimeOffset.UtcNow.AddSeconds(index),
                // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                day_index = 5,
                // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                weekday = "FRI",
                // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                turn_number = player.Turn,
                // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                action_slot = 0,
                // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                sequence_number = donationSequence + index,
                // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                action_type = "JumatBerkah",
                // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                ruleset_version_id = setup.RulesetVersionId,
                // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                payload = new { amount = index + 1 }
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
            // FridayDonations_RemainSealedUntilEveryPlayerSubmits.
            }, instructorToken);
            // Menjalankan pemeriksaan bahwa `donationResponse.StatusCode == HttpStatusCode.Created`, `await donationResponse.Content.ReadAsStringAsync()`
            // bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam FridayDonations_RemainSealedUntilEveryPlayerSubmits.
            Assert.True(
                // Meneruskan perbandingan kesamaan antara `donationResponse.StatusCode` dan `HttpStatusCode.Created` sebagai argumen ke `Assert.True`.
                donationResponse.StatusCode == HttpStatusCode.Created,
                // Meneruskan hasil operasi asinkron memanggil `donationResponse.Content.ReadAsStringAsync` dengan tanpa argumen; await menunggu hasil tanpa
                // memblokir thread selama operasi belum selesai sebagai argumen ke `Assert.True`.
                await donationResponse.Content.ReadAsStringAsync());

            // Menyiapkan variabel lokal `donationEvents` untuk nilai donasi event dengan mematerialisasi urutan `(await GetEventsAsync()) .Where(item =>
            // item.GetProperty(”action_type”).GetString() == ”JumatBerkah”)` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe
            // variabel disimpulkan dari ekspresi nilai awal.
            var donationEvents = (await GetEventsAsync())
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(item => item.GetProperty(”action_type”).GetString() == ”JumatBerkah”)
                // dalam FridayDonations_RemainSealedUntilEveryPlayerSubmits; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
                .Where(item => item.GetProperty("action_type").GetString() == "JumatBerkah")
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList(); dalam FridayDonations_RemainSealedUntilEveryPlayerSubmits; token
                // pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
                .ToList();
            // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`index + 1`, `donationEvents.Count`); pengujian
            // gagal jika keduanya berbeda dalam FridayDonations_RemainSealedUntilEveryPlayerSubmits.
            Assert.Equal(index + 1, donationEvents.Count);
            // Memeriksa pemeriksaan lebih kecil antara `index` dan `players.Count - 1`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
            // FridayDonations_RemainSealedUntilEveryPlayerSubmits.
            if (index < players.Count - 1)
            // Membuka scope cabang if untuk kondisi `index < players.Count - 1`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // FridayDonations_RemainSealedUntilEveryPlayerSubmits.
            {
                // Menjalankan pemeriksaan hasil dengan `Assert.All` menggunakan `donationEvents`, `item => { var payload = item.GetProperty(”payload”);
                // Assert.Equal(”SEALED”, payload.GetProperty(”status”).GetString()); Assert.False(payload.TryGetProperty(”amount”, out _)); ...`; ketidaksesuaian
                // dengan ekspektasi membuat pengujian gagal dalam FridayDonations_RemainSealedUntilEveryPlayerSubmits.
                Assert.All(donationEvents, item =>
                // Membuka scope fungsi lambda yang dipasok ke `Assert.All`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // FridayDonations_RemainSealedUntilEveryPlayerSubmits.
                {
                    // Menyiapkan variabel lokal `payload` untuk muatan detail event dalam format JSON dengan memanggil `item.GetProperty` dengan `”payload”`. Tipe
                    // variabel disimpulkan dari ekspresi nilai awal.
                    var payload = item.GetProperty("payload");
                    // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”SEALED”`,
                    // `payload.GetProperty(”status”).GetString()`); pengujian gagal jika keduanya berbeda dalam FridayDonations_RemainSealedUntilEveryPlayerSubmits.
                    Assert.Equal("SEALED", payload.GetProperty("status").GetString());
                    // Menjalankan pemeriksaan bahwa `payload.TryGetProperty(”amount”, out _)` bernilai salah; pengujian gagal jika kondisi justru terpenuhi dalam
                    // FridayDonations_RemainSealedUntilEveryPlayerSubmits.
                    Assert.False(payload.TryGetProperty("amount", out _));
                // Menutup scope fungsi lambda yang dipasok ke `Assert.All`; bagian berikut berada di luar batas blok tersebut dalam
                // FridayDonations_RemainSealedUntilEveryPlayerSubmits.
                });
            // Menutup scope cabang if untuk kondisi `index < players.Count - 1`; bagian berikut berada di luar batas blok tersebut dalam
            // FridayDonations_RemainSealedUntilEveryPlayerSubmits.
            }
            // Menjalankan cabang alternatif ketika kondisi if sebelumnya tidak terpenuhi dalam FridayDonations_RemainSealedUntilEveryPlayerSubmits.
            else
            // Membuka scope cabang else; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam FridayDonations_RemainSealedUntilEveryPlayerSubmits.
            {
                // Menjalankan pemeriksaan hasil dengan `Assert.All` menggunakan `donationEvents`, `item => { var payload = item.GetProperty(”payload”);
                // Assert.True(payload.GetProperty(”amount”).GetInt32() > 0); Assert.False(payload.TryGetProperty(”status”, out _)); }`; ketidaksesuaian dengan
                // ekspektasi membuat pengujian gagal dalam FridayDonations_RemainSealedUntilEveryPlayerSubmits.
                Assert.All(donationEvents, item =>
                // Membuka scope fungsi lambda yang dipasok ke `Assert.All`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // FridayDonations_RemainSealedUntilEveryPlayerSubmits.
                {
                    // Menyiapkan variabel lokal `payload` untuk muatan detail event dalam format JSON dengan memanggil `item.GetProperty` dengan `”payload”`. Tipe
                    // variabel disimpulkan dari ekspresi nilai awal.
                    var payload = item.GetProperty("payload");
                    // Menjalankan pemeriksaan bahwa `payload.GetProperty(”amount”).GetInt32() > 0` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
                    // FridayDonations_RemainSealedUntilEveryPlayerSubmits.
                    Assert.True(payload.GetProperty("amount").GetInt32() > 0);
                    // Menjalankan pemeriksaan bahwa `payload.TryGetProperty(”status”, out _)` bernilai salah; pengujian gagal jika kondisi justru terpenuhi dalam
                    // FridayDonations_RemainSealedUntilEveryPlayerSubmits.
                    Assert.False(payload.TryGetProperty("status", out _));
                // Menutup scope fungsi lambda yang dipasok ke `Assert.All`; bagian berikut berada di luar batas blok tersebut dalam
                // FridayDonations_RemainSealedUntilEveryPlayerSubmits.
                });
            // Menutup scope cabang else; bagian berikut berada di luar batas blok tersebut dalam FridayDonations_RemainSealedUntilEveryPlayerSubmits.
            }
        // Menutup scope loop dengan syarat `index < players.Count`; bagian berikut berada di luar batas blok tersebut dalam
        // FridayDonations_RemainSealedUntilEveryPlayerSubmits.
        }
    // Menutup scope metode FridayDonations_RemainSealedUntilEveryPlayerSubmits; bagian berikut berada di luar batas blok tersebut dalam
    // FridayDonations_RemainSealedUntilEveryPlayerSubmits.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `VirtualMarketActions_AreRejected_AndPhysicalCardActionHasNoRefillPayload` dengan hasil bertipe `Task`; operasi ini
    // menangani virtual pasar aksi are rejected dan physical kartu aksi memiliki no refill payload. async memungkinkan metode menunggu operasi I/O
    // dengan await dan mengembalikan penyelesaian melalui Task.
    public async Task VirtualMarketActions_AreRejected_AndPhysicalCardActionHasNoRefillPayload()
    // Membuka scope metode VirtualMarketActions_AreRejected_AndPhysicalCardActionHasNoRefillPayload; pernyataan/deklarasi berikut berada di dalam batas
    // blok ini dalam VirtualMarketActions_AreRejected_AndPhysicalCardActionHasNoRefillPayload.
    {
        // Menyiapkan variabel lokal `suffix` untuk nilai suffix dengan teks interpolasi `$”physical_cards_{Guid.NewGuid():N}”`; nilai ekspresi di dalam
        // kurung kurawal disisipkan saat program berjalan. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var suffix = $"physical_cards_{Guid.NewGuid():N}";
        // Menyiapkan variabel lokal `instructorToken` untuk nilai instruktur token dengan `(await RegisterAsync(
        // $”it_market_instructor_{Guid.NewGuid():N}”, ”IntegrationMarketInstructorPass!123”, ”INSTRUCTOR”)).AccessToken` (nilai akses token). Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var instructorToken = (await RegisterAsync(
            // Meneruskan teks interpolasi `$”it_market_instructor_{Guid.NewGuid():N}”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan
            // sebagai argumen ke `RegisterAsync`.
            $"it_market_instructor_{Guid.NewGuid():N}",
            // Meneruskan nilai literal `”IntegrationMarketInstructorPass!123”` sebagai argumen ke `RegisterAsync`.
            "IntegrationMarketInstructorPass!123",
            // Meneruskan nilai literal `”INSTRUCTOR”` sebagai argumen ke `RegisterAsync`.
            "INSTRUCTOR")).AccessToken;
        // Menyiapkan variabel lokal `setup` untuk nilai setup dengan hasil operasi asinkron memanggil `CreateReadySessionAsync` dengan `instructorToken`,
        // `suffix`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var setup = await CreateReadySessionAsync(instructorToken, suffix);
        // Menyiapkan variabel lokal `now` untuk nilai now dengan `DateTimeOffset.UtcNow`, yaitu waktu UTC saat operasi dilakukan. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var now = DateTimeOffset.UtcNow;

        // Menyiapkan variabel lokal `virtualMarketResponse` untuk nilai virtual pasar respons dengan hasil operasi asinkron memanggil `SendJsonAsync`
        // dengan `HttpMethod.Post`, `”/api/v1/events”`, `new { event_id = Guid.NewGuid(), session_id = setup.SessionId, user_id = (Guid?)null, actor_type =
        // ”SYSTEM”, timestamp = now.AddSeconds(1), day_index = 1, weekday = ”MON”, tur...`, `instructorToken`; await menunggu hasil tanpa memblokir thread
        // selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope
        // berakhir.
        using var virtualMarketResponse = await SendJsonAsync(HttpMethod.Post, "/api/v1/events", new
        // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // VirtualMarketActions_AreRejected_AndPhysicalCardActionHasNoRefillPayload.
        {
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            event_id = Guid.NewGuid(),
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            session_id = setup.SessionId,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            user_id = (Guid?)null,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            actor_type = "SYSTEM",
            // Meneruskan nilai literal `1` sebagai argumen ke `now.AddSeconds`.
            timestamp = now.AddSeconds(1),
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            day_index = 1,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            weekday = "MON",
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            turn_number = 0,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            action_slot = 0,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            sequence_number = setup.NextSequenceNumber,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            action_type = "IsiUlangPasar",
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            ruleset_version_id = setup.RulesetVersionId,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            payload = new { }
        // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
        // VirtualMarketActions_AreRejected_AndPhysicalCardActionHasNoRefillPayload.
        }, instructorToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.UnprocessableEntity`,
        // `virtualMarketResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam
        // VirtualMarketActions_AreRejected_AndPhysicalCardActionHasNoRefillPayload.
        Assert.Equal(HttpStatusCode.UnprocessableEntity, virtualMarketResponse.StatusCode);

        // Menyiapkan variabel lokal `skippedOrderResponse` untuk nilai skipped urutan/pesanan respons dengan hasil operasi asinkron memanggil
        // `SendJsonAsync` dengan `HttpMethod.Post`, `”/api/v1/events”`, `new { event_id = Guid.NewGuid(), session_id = setup.SessionId, user_id =
        // setup.ActingUserId, actor_type = ”PLAYER”, timestamp = now.AddMilliseconds(500), day_index = 1, weekda...`, `instructorToken`; await menunggu
        // hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya
        // dilepas otomatis saat scope berakhir.
        using var skippedOrderResponse = await SendJsonAsync(HttpMethod.Post, "/api/v1/events", new
        // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // VirtualMarketActions_AreRejected_AndPhysicalCardActionHasNoRefillPayload.
        {
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            event_id = Guid.NewGuid(),
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            session_id = setup.SessionId,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            user_id = setup.ActingUserId,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            actor_type = "PLAYER",
            // Meneruskan nilai literal `500` sebagai argumen ke `now.AddMilliseconds`.
            timestamp = now.AddMilliseconds(500),
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            day_index = 1,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            weekday = "MON",
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            turn_number = 1,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            action_slot = 1,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            sequence_number = setup.NextSequenceNumber,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            action_type = "LewatiOrder",
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            ruleset_version_id = setup.RulesetVersionId,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            payload = new { }
        // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
        // VirtualMarketActions_AreRejected_AndPhysicalCardActionHasNoRefillPayload.
        }, instructorToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.UnprocessableEntity`,
        // `skippedOrderResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam
        // VirtualMarketActions_AreRejected_AndPhysicalCardActionHasNoRefillPayload.
        Assert.Equal(HttpStatusCode.UnprocessableEntity, skippedOrderResponse.StatusCode);

        // Menyiapkan variabel lokal `purchaseEventId` untuk nilai pembelian event identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var purchaseEventId = Guid.NewGuid();
        // Menyiapkan variabel lokal `purchaseResponse` untuk nilai pembelian respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Post`, `”/api/v1/events”`, `new { event_id = purchaseEventId, session_id = setup.SessionId, user_id = setup.ActingUserId, actor_type
        // = ”PLAYER”, timestamp = now.AddSeconds(1), day_index = 1, weekday = ”M...`, `instructorToken`; await menunggu hasil tanpa memblokir thread selama
        // operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var purchaseResponse = await SendJsonAsync(HttpMethod.Post, "/api/v1/events", new
        // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // VirtualMarketActions_AreRejected_AndPhysicalCardActionHasNoRefillPayload.
        {
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            event_id = purchaseEventId,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            session_id = setup.SessionId,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            user_id = setup.ActingUserId,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            actor_type = "PLAYER",
            // Meneruskan nilai literal `1` sebagai argumen ke `now.AddSeconds`.
            timestamp = now.AddSeconds(1),
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            day_index = 1,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            weekday = "MON",
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            turn_number = 1,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            action_slot = 1,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            sequence_number = setup.NextSequenceNumber,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            action_type = "BahanMasakan",
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            ruleset_version_id = setup.RulesetVersionId,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            payload = new { card_id = "nasi_putih", amount = 1 }
        // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
        // VirtualMarketActions_AreRejected_AndPhysicalCardActionHasNoRefillPayload.
        }, instructorToken);
        // Menjalankan pemeriksaan bahwa `purchaseResponse.StatusCode == HttpStatusCode.Created`, `await purchaseResponse.Content.ReadAsStringAsync()`
        // bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam VirtualMarketActions_AreRejected_AndPhysicalCardActionHasNoRefillPayload.
        Assert.True(purchaseResponse.StatusCode == HttpStatusCode.Created, await purchaseResponse.Content.ReadAsStringAsync());

        // Menyiapkan variabel lokal `eventsResponse` untuk nilai event respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Get`, `$”/api/v1/sessions/{setup.SessionId}/events?limit=100”`, `null`, `instructorToken`; await menunggu hasil tanpa memblokir
        // thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope
        // berakhir.
        using var eventsResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Get` (nilai get) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Get,
            // Meneruskan teks interpolasi `$”/api/v1/sessions/{setup.SessionId}/events?limit=100”`; nilai ekspresi di dalam kurung kurawal disisipkan saat
            // program berjalan sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/sessions/{setup.SessionId}/events?limit=100",
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen ke `SendJsonAsync`.
            null,
            // Meneruskan `instructorToken` (nilai instruktur token) sebagai argumen ke `SendJsonAsync`.
            instructorToken);
        // Menyiapkan variabel lokal `eventsBody` untuk nilai event body dengan hasil operasi asinkron memanggil `ReadJsonAsync` dengan `eventsResponse`;
        // await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan
        // sumber daya dilepas otomatis saat scope berakhir.
        using var eventsBody = await ReadJsonAsync(eventsResponse);
        // Menyiapkan variabel lokal `purchaseEvent` untuk nilai pembelian event dengan pemeriksaan hasil dengan `Assert.Single` menggunakan
        // `eventsBody.RootElement.GetProperty(”items”).EnumerateArray()`, `item => item.GetProperty(”event_id”).GetGuid() == purchaseEventId`;
        // ketidaksesuaian dengan ekspektasi membuat pengujian gagal. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var purchaseEvent = Assert.Single(
            // Meneruskan menelusuri elemen array JSON `eventsBody.RootElement.GetProperty(”items”)` sebagai argumen ke `Assert.Single`; Meneruskan nilai
            // literal `”items”` sebagai argumen ke `eventsBody.RootElement.GetProperty`.
            eventsBody.RootElement.GetProperty("items").EnumerateArray(),
            // Parameter `item` bertipe `` membawa nilai elemen.
            item => item.GetProperty("event_id").GetGuid() == purchaseEventId);
        // Menjalankan pemeriksaan bahwa `purchaseEvent.GetProperty(”payload”).TryGetProperty(”market_refills”, out _)` bernilai salah; pengujian gagal jika
        // kondisi justru terpenuhi dalam VirtualMarketActions_AreRejected_AndPhysicalCardActionHasNoRefillPayload.
        Assert.False(purchaseEvent.GetProperty("payload").TryGetProperty("market_refills", out _));
    // Menutup scope metode VirtualMarketActions_AreRejected_AndPhysicalCardActionHasNoRefillPayload; bagian berikut berada di luar batas blok tersebut
    // dalam VirtualMarketActions_AreRejected_AndPhysicalCardActionHasNoRefillPayload.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `MahirRisk_WithInsufficientCash_IsResolvedByCatalogEmergencyLoan` dengan hasil bertipe `Task`; operasi ini menangani mahir
    // risiko dengan insufficient uang tunai berstatus hasil resolusi berdasarkan catalog emergency pinjaman. async memungkinkan metode menunggu operasi
    // I/O dengan await dan mengembalikan penyelesaian melalui Task.
    public async Task MahirRisk_WithInsufficientCash_IsResolvedByCatalogEmergencyLoan()
    // Membuka scope metode MahirRisk_WithInsufficientCash_IsResolvedByCatalogEmergencyLoan; pernyataan/deklarasi berikut berada di dalam batas blok ini
    // dalam MahirRisk_WithInsufficientCash_IsResolvedByCatalogEmergencyLoan.
    {
        // Menyiapkan variabel lokal `suffix` untuk nilai suffix dengan `Guid.NewGuid().ToString(”N”)[..8]`, yaitu elemen koleksi yang dipilih melalui
        // indeks atau kunci tersebut. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var suffix = Guid.NewGuid().ToString("N")[..8];
        // Menyiapkan variabel lokal `instructorToken` untuk nilai instruktur token dengan `(await RegisterAsync( $”it_risk_instructor_{suffix}”,
        // ”IntegrationRiskInstructorPass!123”, ”INSTRUCTOR”)).AccessToken` (nilai akses token). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var instructorToken = (await RegisterAsync(
            // Meneruskan teks interpolasi `$”it_risk_instructor_{suffix}”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan sebagai
            // argumen ke `RegisterAsync`.
            $"it_risk_instructor_{suffix}",
            // Meneruskan nilai literal `”IntegrationRiskInstructorPass!123”` sebagai argumen ke `RegisterAsync`.
            "IntegrationRiskInstructorPass!123",
            // Meneruskan nilai literal `”INSTRUCTOR”` sebagai argumen ke `RegisterAsync`.
            "INSTRUCTOR")).AccessToken;
        // Menyiapkan variabel lokal `setup` untuk nilai setup dengan hasil operasi asinkron memanggil `CreateReadySessionAsync` dengan `instructorToken`,
        // `$”risk_{suffix}”`, `BuildRulesetDefinition(startingCash: 5, mode: ”MAHIR”, riskAmount: 20)`; await menunggu hasil tanpa memblokir thread selama
        // operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var setup = await CreateReadySessionAsync(
            // Meneruskan `instructorToken` (nilai instruktur token) sebagai argumen ke `CreateReadySessionAsync`.
            instructorToken,
            // Meneruskan teks interpolasi `$”risk_{suffix}”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan sebagai argumen ke
            // `CreateReadySessionAsync`.
            $"risk_{suffix}",
            // Meneruskan memanggil `BuildRulesetDefinition` dengan `5`, `”MAHIR”`, `20` sebagai argumen ke `CreateReadySessionAsync`; Meneruskan nilai literal
            // `5` sebagai argumen bernama `startingCash`; Meneruskan nilai literal `”MAHIR”` sebagai argumen bernama `mode`; Meneruskan nilai literal `20`
            // sebagai argumen bernama `riskAmount`.
            BuildRulesetDefinition(startingCash: 5, mode: "MAHIR", riskAmount: 20));
        // Menyiapkan variabel lokal `orderEventId` untuk nilai urutan/pesanan event identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var orderEventId = Guid.NewGuid();
        // Menyiapkan variabel lokal `riskEventId` untuk nilai risiko event identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var riskEventId = Guid.NewGuid();
        // Menyiapkan variabel lokal `now` untuk nilai now dengan `DateTimeOffset.UtcNow`, yaitu waktu UTC saat operasi dilakukan. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var now = DateTimeOffset.UtcNow;
        // Menyiapkan variabel lokal `sequence` untuk nomor urut event yang menentukan urutan pemrosesan riwayat permainan dengan `setup.NextSequenceNumber`
        // (nilai next sequence number). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var sequence = setup.NextSequenceNumber;

        // Menyiapkan variabel lokal `orderResponse` untuk nilai urutan/pesanan respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Post`, `”/api/v1/events”`, `new { event_id = orderEventId, session_id = setup.SessionId, user_id = setup.ActingUserId, actor_type =
        // ”PLAYER”, timestamp = now, day_index = 1, weekday = ”MON”, turn_number ...`, `instructorToken`; await menunggu hasil tanpa memblokir thread
        // selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var orderResponse = await SendJsonAsync(HttpMethod.Post, "/api/v1/events", new
        // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // MahirRisk_WithInsufficientCash_IsResolvedByCatalogEmergencyLoan.
        {
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            event_id = orderEventId,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            session_id = setup.SessionId,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            user_id = setup.ActingUserId,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            actor_type = "PLAYER",
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            timestamp = now,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            day_index = 1,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            weekday = "MON",
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            turn_number = 1,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            action_slot = 1,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            sequence_number = sequence,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            action_type = "JualMasakan",
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            ruleset_version_id = setup.RulesetVersionId,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            payload = new { order_card_id = "nasi_goreng" }
        // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
        // MahirRisk_WithInsufficientCash_IsResolvedByCatalogEmergencyLoan.
        }, instructorToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.Created`,
        // `orderResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam MahirRisk_WithInsufficientCash_IsResolvedByCatalogEmergencyLoan.
        Assert.Equal(HttpStatusCode.Created, orderResponse.StatusCode);

        // Menyiapkan variabel lokal `riskResponse` untuk nilai risiko respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Post`, `”/api/v1/events”`, `new { event_id = riskEventId, session_id = setup.SessionId, user_id = setup.ActingUserId, actor_type =
        // ”PLAYER”, timestamp = now.AddSeconds(1), day_index = 1, weekday = ”MON”,...`, `instructorToken`; await menunggu hasil tanpa memblokir thread
        // selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var riskResponse = await SendJsonAsync(HttpMethod.Post, "/api/v1/events", new
        // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // MahirRisk_WithInsufficientCash_IsResolvedByCatalogEmergencyLoan.
        {
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            event_id = riskEventId,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            session_id = setup.SessionId,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            user_id = setup.ActingUserId,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            actor_type = "PLAYER",
            // Meneruskan nilai literal `1` sebagai argumen ke `now.AddSeconds`.
            timestamp = now.AddSeconds(1),
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            day_index = 1,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            weekday = "MON",
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            turn_number = 1,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            action_slot = 0,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            sequence_number = sequence + 1,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            action_type = "RisikoKehidupan",
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            ruleset_version_id = setup.RulesetVersionId,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            payload = new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // MahirRisk_WithInsufficientCash_IsResolvedByCatalogEmergencyLoan.
            {
                // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                risk_id = "risk_cost_4",
                // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                source_order_event_id = orderEventId
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
            // MahirRisk_WithInsufficientCash_IsResolvedByCatalogEmergencyLoan.
            }
        // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
        // MahirRisk_WithInsufficientCash_IsResolvedByCatalogEmergencyLoan.
        }, instructorToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.Created`,
        // `riskResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam MahirRisk_WithInsufficientCash_IsResolvedByCatalogEmergencyLoan.
        Assert.Equal(HttpStatusCode.Created, riskResponse.StatusCode);

        // Menyiapkan variabel lokal `unrelatedActionResponse` untuk nilai unrelated aksi respons dengan hasil operasi asinkron memanggil `SendJsonAsync`
        // dengan `HttpMethod.Post`, `”/api/v1/events”`, `new { event_id = Guid.NewGuid(), session_id = setup.SessionId, user_id = setup.ActingUserId,
        // actor_type = ”PLAYER”, timestamp = now.AddSeconds(2), day_index = 1, weekday = ”MO...`, `instructorToken`; await menunggu hasil tanpa memblokir
        // thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var unrelatedActionResponse = await SendJsonAsync(HttpMethod.Post, "/api/v1/events", new
        // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // MahirRisk_WithInsufficientCash_IsResolvedByCatalogEmergencyLoan.
        {
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            event_id = Guid.NewGuid(),
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            session_id = setup.SessionId,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            user_id = setup.ActingUserId,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            actor_type = "PLAYER",
            // Meneruskan nilai literal `2` sebagai argumen ke `now.AddSeconds`.
            timestamp = now.AddSeconds(2),
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            day_index = 1,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            weekday = "MON",
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            turn_number = 1,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            action_slot = 2,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            sequence_number = sequence + 2,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            action_type = "KerjaLepas",
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            ruleset_version_id = setup.RulesetVersionId,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            payload = new { amount = 1 }
        // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
        // MahirRisk_WithInsufficientCash_IsResolvedByCatalogEmergencyLoan.
        }, instructorToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.UnprocessableEntity`,
        // `unrelatedActionResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam
        // MahirRisk_WithInsufficientCash_IsResolvedByCatalogEmergencyLoan.
        Assert.Equal(HttpStatusCode.UnprocessableEntity, unrelatedActionResponse.StatusCode);
        // Membatasi masa pakai `var unrelatedActionBody = await ReadJsonAsync(unrelatedActionResponse)` pada blok using; sumber daya dilepas ketika blok
        // berakhir melalui Dispose.
        using (var unrelatedActionBody = await ReadJsonAsync(unrelatedActionResponse))
        // Membuka scope scope pemakaian sumber daya using; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // MahirRisk_WithInsufficientCash_IsResolvedByCatalogEmergencyLoan.
        {
            // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”Selesaikan risiko pengeluaran pemain”`,
            // `unrelatedActionBody.RootElement.GetProperty(”message”).GetString()`, `StringComparison.Ordinal` dalam
            // MahirRisk_WithInsufficientCash_IsResolvedByCatalogEmergencyLoan.
            Assert.Contains(
                // Meneruskan nilai literal `”Selesaikan risiko pengeluaran pemain”` sebagai argumen ke `Assert.Contains`.
                "Selesaikan risiko pengeluaran pemain",
                // Meneruskan membaca nilai string dari `unrelatedActionBody.RootElement.GetProperty(”message”)` sesuai tipe JSON atau sumber data yang digunakan
                // sebagai argumen ke `Assert.Contains`; Meneruskan nilai literal `”message”` sebagai argumen ke `unrelatedActionBody.RootElement.GetProperty`.
                unrelatedActionBody.RootElement.GetProperty("message").GetString(),
                // Meneruskan `StringComparison.Ordinal` (nilai ordinal) sebagai argumen ke `Assert.Contains`.
                StringComparison.Ordinal);
        // Menutup scope scope pemakaian sumber daya using; bagian berikut berada di luar batas blok tersebut dalam
        // MahirRisk_WithInsufficientCash_IsResolvedByCatalogEmergencyLoan.
        }

        // Menyiapkan variabel lokal `pendingTurnEnd` untuk nilai tertunda giliran end dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Post`, `”/api/v1/events”`, `new { event_id = Guid.NewGuid(), session_id = setup.SessionId, user_id = (Guid?)null, actor_type =
        // ”SYSTEM”, timestamp = now.AddSeconds(2), day_index = 1, weekday = ”MON”, tur...`, `instructorToken`; await menunggu hasil tanpa memblokir thread
        // selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var pendingTurnEnd = await SendJsonAsync(HttpMethod.Post, "/api/v1/events", new
        // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // MahirRisk_WithInsufficientCash_IsResolvedByCatalogEmergencyLoan.
        {
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            event_id = Guid.NewGuid(),
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            session_id = setup.SessionId,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            user_id = (Guid?)null,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            actor_type = "SYSTEM",
            // Meneruskan nilai literal `2` sebagai argumen ke `now.AddSeconds`.
            timestamp = now.AddSeconds(2),
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            day_index = 1,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            weekday = "MON",
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            turn_number = 0,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            action_slot = 0,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            sequence_number = sequence + 2,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            action_type = "AkhirGiliran",
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            ruleset_version_id = setup.RulesetVersionId,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            payload = new { }
        // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
        // MahirRisk_WithInsufficientCash_IsResolvedByCatalogEmergencyLoan.
        }, instructorToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.UnprocessableEntity`,
        // `pendingTurnEnd.StatusCode`); pengujian gagal jika keduanya berbeda dalam MahirRisk_WithInsufficientCash_IsResolvedByCatalogEmergencyLoan.
        Assert.Equal(HttpStatusCode.UnprocessableEntity, pendingTurnEnd.StatusCode);

        // Menyiapkan variabel lokal `emergencyResponse` untuk nilai emergency respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Post`, `”/api/v1/events”`, `new { event_id = Guid.NewGuid(), session_id = setup.SessionId, user_id = setup.ActingUserId, actor_type =
        // ”PLAYER”, timestamp = now.AddSeconds(2), day_index = 1, weekday = ”MO...`, `instructorToken`; await menunggu hasil tanpa memblokir thread selama
        // operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var emergencyResponse = await SendJsonAsync(HttpMethod.Post, "/api/v1/events", new
        // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // MahirRisk_WithInsufficientCash_IsResolvedByCatalogEmergencyLoan.
        {
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            event_id = Guid.NewGuid(),
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            session_id = setup.SessionId,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            user_id = setup.ActingUserId,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            actor_type = "PLAYER",
            // Meneruskan nilai literal `2` sebagai argumen ke `now.AddSeconds`.
            timestamp = now.AddSeconds(2),
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            day_index = 1,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            weekday = "MON",
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            turn_number = 1,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            action_slot = 0,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            sequence_number = sequence + 2,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            action_type = "GunakanOpsiDarurat",
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            ruleset_version_id = setup.RulesetVersionId,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            payload = new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // MahirRisk_WithInsufficientCash_IsResolvedByCatalogEmergencyLoan.
            {
                // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                risk_event_id = riskEventId,
                // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                option_type = "TAKE_SHARIA_LOAN",
                // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                loan_code = "loan_syariah_10",
                // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                direction = "IN",
                // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                amount = 100
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
            // MahirRisk_WithInsufficientCash_IsResolvedByCatalogEmergencyLoan.
            }
        // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
        // MahirRisk_WithInsufficientCash_IsResolvedByCatalogEmergencyLoan.
        }, instructorToken);
        // Menjalankan pemeriksaan bahwa `emergencyResponse.StatusCode == HttpStatusCode.Created`, `await emergencyResponse.Content.ReadAsStringAsync()`
        // bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam MahirRisk_WithInsufficientCash_IsResolvedByCatalogEmergencyLoan.
        Assert.True(emergencyResponse.StatusCode == HttpStatusCode.Created, await emergencyResponse.Content.ReadAsStringAsync());

        // Menyiapkan variabel lokal `duplicateLoanResponse` untuk nilai duplicate pinjaman respons dengan hasil operasi asinkron memanggil `SendJsonAsync`
        // dengan `HttpMethod.Post`, `”/api/v1/events”`, `new { event_id = Guid.NewGuid(), session_id = setup.SessionId, user_id = setup.ActingUserId,
        // actor_type = ”PLAYER”, timestamp = now.AddSeconds(3), day_index = 1, weekday = ”MO...`, `instructorToken`; await menunggu hasil tanpa memblokir
        // thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var duplicateLoanResponse = await SendJsonAsync(HttpMethod.Post, "/api/v1/events", new
        // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // MahirRisk_WithInsufficientCash_IsResolvedByCatalogEmergencyLoan.
        {
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            event_id = Guid.NewGuid(),
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            session_id = setup.SessionId,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            user_id = setup.ActingUserId,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            actor_type = "PLAYER",
            // Meneruskan nilai literal `3` sebagai argumen ke `now.AddSeconds`.
            timestamp = now.AddSeconds(3),
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            day_index = 1,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            weekday = "MON",
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            turn_number = 1,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            action_slot = 2,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            sequence_number = sequence + 3,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            action_type = "PinjamanSyariah",
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            ruleset_version_id = setup.RulesetVersionId,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            payload = new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // MahirRisk_WithInsufficientCash_IsResolvedByCatalogEmergencyLoan.
            {
                // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                loan_id = "loan_syariah_10",
                // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                loan_code = "loan_syariah_10",
                // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                principal = 10,
                // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                repayment_amount = 10,
                // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                duration_days = 1,
                // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                penalty_points = 10
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
            // MahirRisk_WithInsufficientCash_IsResolvedByCatalogEmergencyLoan.
            }
        // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
        // MahirRisk_WithInsufficientCash_IsResolvedByCatalogEmergencyLoan.
        }, instructorToken);
        // Menjalankan pemeriksaan bahwa `duplicateLoanResponse.StatusCode == HttpStatusCode.Created`, `await
        // duplicateLoanResponse.Content.ReadAsStringAsync()` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // MahirRisk_WithInsufficientCash_IsResolvedByCatalogEmergencyLoan.
        Assert.True(
            // Meneruskan perbandingan kesamaan antara `duplicateLoanResponse.StatusCode` dan `HttpStatusCode.Created` sebagai argumen ke `Assert.True`.
            duplicateLoanResponse.StatusCode == HttpStatusCode.Created,
            // Meneruskan hasil operasi asinkron memanggil `duplicateLoanResponse.Content.ReadAsStringAsync` dengan tanpa argumen; await menunggu hasil tanpa
            // memblokir thread selama operasi belum selesai sebagai argumen ke `Assert.True`.
            await duplicateLoanResponse.Content.ReadAsStringAsync());

        // Menyiapkan variabel lokal `resolvedTurnEnd` untuk nilai hasil resolusi giliran end dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Post`, `”/api/v1/events”`, `new { event_id = Guid.NewGuid(), session_id = setup.SessionId, user_id = (Guid?)null, actor_type =
        // ”SYSTEM”, timestamp = now.AddSeconds(4), day_index = 1, weekday = ”MON”, tur...`, `instructorToken`; await menunggu hasil tanpa memblokir thread
        // selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var resolvedTurnEnd = await SendJsonAsync(HttpMethod.Post, "/api/v1/events", new
        // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // MahirRisk_WithInsufficientCash_IsResolvedByCatalogEmergencyLoan.
        {
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            event_id = Guid.NewGuid(),
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            session_id = setup.SessionId,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            user_id = (Guid?)null,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            actor_type = "SYSTEM",
            // Meneruskan nilai literal `4` sebagai argumen ke `now.AddSeconds`.
            timestamp = now.AddSeconds(4),
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            day_index = 1,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            weekday = "MON",
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            turn_number = 0,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            action_slot = 0,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            sequence_number = sequence + 4,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            action_type = "AkhirGiliran",
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            ruleset_version_id = setup.RulesetVersionId,
            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            payload = new { }
        // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
        // MahirRisk_WithInsufficientCash_IsResolvedByCatalogEmergencyLoan.
        }, instructorToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.UnprocessableEntity`,
        // `resolvedTurnEnd.StatusCode`); pengujian gagal jika keduanya berbeda dalam MahirRisk_WithInsufficientCash_IsResolvedByCatalogEmergencyLoan.
        Assert.Equal(HttpStatusCode.UnprocessableEntity, resolvedTurnEnd.StatusCode);

        // Menyiapkan variabel lokal `transactionsResponse` untuk nilai transactions respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Get`, `$”/api/v1/analytics/sessions/{setup.SessionId}/transactions?userId={setup.ActingUserId}”`, `null`, `instructorToken`; await
        // menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var transactionsResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Get` (nilai get) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Get,
            // Meneruskan teks interpolasi `$”/api/v1/analytics/sessions/{setup.SessionId}/transactions?userId={setup.ActingUserId}”`; nilai ekspresi di dalam
            // kurung kurawal disisipkan saat program berjalan sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/analytics/sessions/{setup.SessionId}/transactions?userId={setup.ActingUserId}",
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen ke `SendJsonAsync`.
            null,
            // Meneruskan `instructorToken` (nilai instruktur token) sebagai argumen ke `SendJsonAsync`.
            instructorToken);
        // Menyiapkan variabel lokal `transactions` untuk nilai transactions dengan hasil operasi asinkron membaca tanpa argumen menjadi objek bertipe
        // sesuai kontrak JSON melalui `transactionsResponse.Content.ReadFromJsonAsync<TransactionHistoryResponse>`; await menunggu hasil tanpa memblokir
        // thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var transactions = await transactionsResponse.Content.ReadFromJsonAsync<TransactionHistoryResponse>();
        // Menjalankan pemeriksaan NotNull atas `transactions` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // MahirRisk_WithInsufficientCash_IsResolvedByCatalogEmergencyLoan.
        Assert.NotNull(transactions);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `transactions.Items`, `item =>
        // item.Category == ”EMERGENCY_OPTION” && item.Direction == ”IN” && item.Amount == 10` dalam
        // MahirRisk_WithInsufficientCash_IsResolvedByCatalogEmergencyLoan.
        Assert.Contains(transactions.Items, item => item.Category == "EMERGENCY_OPTION" && item.Direction == "IN" && item.Amount == 10);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `transactions.Items`, `item =>
        // item.Category == ”RISK_LIFE” && item.Direction == ”OUT” && item.Amount == 20` dalam
        // MahirRisk_WithInsufficientCash_IsResolvedByCatalogEmergencyLoan.
        Assert.Contains(transactions.Items, item => item.Category == "RISK_LIFE" && item.Direction == "OUT" && item.Amount == 20);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `transactions.Items`, `item =>
        // item.Amount == 100` dalam MahirRisk_WithInsufficientCash_IsResolvedByCatalogEmergencyLoan.
        Assert.DoesNotContain(transactions.Items, item => item.Amount == 100);
    // Menutup scope metode MahirRisk_WithInsufficientCash_IsResolvedByCatalogEmergencyLoan; bagian berikut berada di luar batas blok tersebut dalam
    // MahirRisk_WithInsufficientCash_IsResolvedByCatalogEmergencyLoan.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `MahirInsurance_CannotBeUsedAfterPolicyBecomesInactive` dengan hasil bertipe `Task`; operasi ini menangani mahir asuransi
    // cannot be used after policy becomes inactive. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui
    // Task.
    public async Task MahirInsurance_CannotBeUsedAfterPolicyBecomesInactive()
    // Membuka scope metode MahirInsurance_CannotBeUsedAfterPolicyBecomesInactive; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // MahirInsurance_CannotBeUsedAfterPolicyBecomesInactive.
    {
        // Menyiapkan variabel lokal `suffix` untuk nilai suffix dengan `Guid.NewGuid().ToString(”N”)[..8]`, yaitu elemen koleksi yang dipilih melalui
        // indeks atau kunci tersebut. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var suffix = Guid.NewGuid().ToString("N")[..8];
        // Menyiapkan variabel lokal `instructorToken` untuk nilai instruktur token dengan `(await RegisterAsync( $”it_insurance_instructor_{suffix}”,
        // ”IntegrationInsuranceInstructorPass!123”, ”INSTRUCTOR”)).AccessToken` (nilai akses token). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var instructorToken = (await RegisterAsync(
            // Meneruskan teks interpolasi `$”it_insurance_instructor_{suffix}”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan
            // sebagai argumen ke `RegisterAsync`.
            $"it_insurance_instructor_{suffix}",
            // Meneruskan nilai literal `”IntegrationInsuranceInstructorPass!123”` sebagai argumen ke `RegisterAsync`.
            "IntegrationInsuranceInstructorPass!123",
            // Meneruskan nilai literal `”INSTRUCTOR”` sebagai argumen ke `RegisterAsync`.
            "INSTRUCTOR")).AccessToken;
        // Menyiapkan variabel lokal `setup` untuk nilai setup dengan hasil operasi asinkron memanggil `CreateReadySessionAsync` dengan `instructorToken`,
        // `$”insurance_{suffix}”`, `BuildRulesetDefinition(startingCash: 15, mode: ”MAHIR”, riskAmount: 10)`; await menunggu hasil tanpa memblokir thread
        // selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var setup = await CreateReadySessionAsync(
            // Meneruskan `instructorToken` (nilai instruktur token) sebagai argumen ke `CreateReadySessionAsync`.
            instructorToken,
            // Meneruskan teks interpolasi `$”insurance_{suffix}”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan sebagai argumen ke
            // `CreateReadySessionAsync`.
            $"insurance_{suffix}",
            // Meneruskan memanggil `BuildRulesetDefinition` dengan `15`, `”MAHIR”`, `10` sebagai argumen ke `CreateReadySessionAsync`; Meneruskan nilai literal
            // `15` sebagai argumen bernama `startingCash`; Meneruskan nilai literal `”MAHIR”` sebagai argumen bernama `mode`; Meneruskan nilai literal `10`
            // sebagai argumen bernama `riskAmount`.
            BuildRulesetDefinition(startingCash: 15, mode: "MAHIR", riskAmount: 10));
        // Menyiapkan variabel lokal `sequence` untuk nomor urut event yang menentukan urutan pemrosesan riwayat permainan dengan selisih antara
        // `setup.NextSequenceNumber` dan `1`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var sequence = setup.NextSequenceNumber - 1;
        // Menyiapkan variabel lokal `now` untuk nilai now dengan `DateTimeOffset.UtcNow`, yaitu waktu UTC saat operasi dilakukan. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var now = DateTimeOffset.UtcNow;

        // Mendefinisikan fungsi lokal SendEventAsync dengan hasil `Task<HttpResponseMessage>`; fungsi ini dipakai oleh alur di dalam scope yang sama.
        async Task<HttpResponseMessage> SendEventAsync(
            // Parameter `actionType` bertipe `string` membawa nilai aksi jenis.
            string actionType,
            // Parameter `dayIndex` bertipe `int` membawa nilai hari index.
            int dayIndex,
            // Parameter `actionSlot` bertipe `int` membawa nilai aksi slot.
            int actionSlot,
            // Parameter `payload` bertipe `object` membawa muatan detail event dalam format JSON.
            object payload,
            // Parameter `eventId` bertipe `Guid?` membawa identitas unik event untuk pencatatan dan pemeriksaan duplikasi; nilai null diizinkan ketika data
            // opsional belum tersedia; bila argumen tidak diberikan digunakan null, yaitu penanda tidak ada nilai.
            Guid? eventId = null)
        // Membuka scope fungsi lokal SendEventAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam SendEventAsync.
        {
            // Menjalankan `sequence++` dalam SendEventAsync.
            sequence++;
            // Mengembalikan hasil operasi asinkron memanggil `SendJsonAsync` dengan `HttpMethod.Post`, `”/api/v1/events”`, `new { event_id = eventId ??
            // Guid.NewGuid(), session_id = setup.SessionId, user_id = setup.ActingUserId, actor_type = ”PLAYER”, timestamp = now.AddSeconds(sequence),
            // day_index ...`, `instructorToken`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai kepada pemanggil dalam
            // SendEventAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return await SendJsonAsync(HttpMethod.Post, "/api/v1/events", new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam SendEventAsync.
            {
                // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                event_id = eventId ?? Guid.NewGuid(),
                // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                session_id = setup.SessionId,
                // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                user_id = setup.ActingUserId,
                // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                actor_type = "PLAYER",
                // Meneruskan `sequence` (nomor urut event yang menentukan urutan pemrosesan riwayat permainan) sebagai argumen ke `now.AddSeconds`.
                timestamp = now.AddSeconds(sequence),
                // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                day_index = dayIndex,
                // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                weekday = "MON",
                // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                turn_number = 1,
                // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                action_slot = actionSlot,
                // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                sequence_number = sequence,
                // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                action_type = actionType,
                // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                ruleset_version_id = setup.RulesetVersionId,
                // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                payload
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam SendEventAsync.
            }, instructorToken);
        // Menutup scope fungsi lokal SendEventAsync; bagian berikut berada di luar batas blok tersebut dalam SendEventAsync.
        }

        // Menyiapkan variabel lokal `firstOrderId` untuk nilai first urutan/pesanan identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var firstOrderId = Guid.NewGuid();
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.Created`, `(await
        // SendEventAsync( ”JualMasakan”, 1, 1, new { order_card_id = ”nasi_goreng” }, firstOrderId)).StatusCode`); pengujian gagal jika keduanya berbeda
        // dalam MahirInsurance_CannotBeUsedAfterPolicyBecomesInactive.
        Assert.Equal(HttpStatusCode.Created, (await SendEventAsync(
            // Meneruskan nilai literal `”JualMasakan”` sebagai argumen ke `SendEventAsync`; Meneruskan nilai literal `1` sebagai argumen ke `SendEventAsync`;
            // Meneruskan nilai literal `1` sebagai argumen ke `SendEventAsync`; Meneruskan objek anonim yang mengelompokkan order_card_id sebagai satu nilai
            // sebagai argumen ke `SendEventAsync`; Meneruskan `firstOrderId` (nilai first urutan/pesanan identitas) sebagai argumen ke `SendEventAsync`.
            "JualMasakan", 1, 1, new { order_card_id = "nasi_goreng" }, firstOrderId)).StatusCode);
        // Menyiapkan variabel lokal `firstRiskId` untuk nilai first risiko identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var firstRiskId = Guid.NewGuid();
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.Created`, `(await
        // SendEventAsync( ”RisikoKehidupan”, 1, 0, new { risk_id = ”risk_cost_4”, source_order_event_id = firstOrderId }, firstRiskId)).StatusCode`);
        // pengujian gagal jika keduanya berbeda dalam MahirInsurance_CannotBeUsedAfterPolicyBecomesInactive.
        Assert.Equal(HttpStatusCode.Created, (await SendEventAsync(
            // Meneruskan nilai literal `”RisikoKehidupan”` sebagai argumen ke `SendEventAsync`.
            "RisikoKehidupan",
            // Meneruskan nilai literal `1` sebagai argumen ke `SendEventAsync`.
            1,
            // Meneruskan nilai literal `0` sebagai argumen ke `SendEventAsync`.
            0,
            // Meneruskan objek anonim yang mengelompokkan risk_id, source_order_event_id sebagai satu nilai sebagai argumen ke `SendEventAsync`.
            new { risk_id = "risk_cost_4", source_order_event_id = firstOrderId },
            // Meneruskan `firstRiskId` (nilai first risiko identitas) sebagai argumen ke `SendEventAsync`.
            firstRiskId)).StatusCode);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.Created`, `(await
        // SendEventAsync( ”Asuransi”, 1, 0, new { risk_event_id = firstRiskId })).StatusCode`); pengujian gagal jika keduanya berbeda dalam
        // MahirInsurance_CannotBeUsedAfterPolicyBecomesInactive.
        Assert.Equal(HttpStatusCode.Created, (await SendEventAsync(
            // Meneruskan nilai literal `”Asuransi”` sebagai argumen ke `SendEventAsync`.
            "Asuransi",
            // Meneruskan nilai literal `1` sebagai argumen ke `SendEventAsync`.
            1,
            // Meneruskan nilai literal `0` sebagai argumen ke `SendEventAsync`.
            0,
            // Meneruskan objek anonim yang mengelompokkan risk_event_id sebagai satu nilai sebagai argumen ke `SendEventAsync`.
            new { risk_event_id = firstRiskId })).StatusCode);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.Created`, `(await
        // SendEventAsync( ”KerjaLepas”, 1, 2, new { amount = 1 })).StatusCode`); pengujian gagal jika keduanya berbeda dalam
        // MahirInsurance_CannotBeUsedAfterPolicyBecomesInactive.
        Assert.Equal(HttpStatusCode.Created, (await SendEventAsync(
            // Meneruskan nilai literal `”KerjaLepas”` sebagai argumen ke `SendEventAsync`; Meneruskan nilai literal `1` sebagai argumen ke `SendEventAsync`;
            // Meneruskan nilai literal `2` sebagai argumen ke `SendEventAsync`; Meneruskan objek anonim yang mengelompokkan amount sebagai satu nilai sebagai
            // argumen ke `SendEventAsync`.
            "KerjaLepas", 1, 2, new { amount = 1 })).StatusCode);

        // Menyiapkan variabel lokal `secondUse` untuk nilai second use dengan hasil operasi asinkron memanggil `SendEventAsync` dengan `”Asuransi”`, `1`,
        // `0`, `new { risk_event_id = firstRiskId }`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var secondUse = await SendEventAsync("Asuransi", 1, 0, new { risk_event_id = firstRiskId });
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.UnprocessableEntity`,
        // `secondUse.StatusCode`); pengujian gagal jika keduanya berbeda dalam MahirInsurance_CannotBeUsedAfterPolicyBecomesInactive.
        Assert.Equal(HttpStatusCode.UnprocessableEntity, secondUse.StatusCode);

        // Menyiapkan variabel lokal `transactionsResponse` untuk nilai transactions respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Get`, `$”/api/v1/analytics/sessions/{setup.SessionId}/transactions?userId={setup.ActingUserId}”`, `null`, `instructorToken`; await
        // menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var transactionsResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Get` (nilai get) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Get,
            // Meneruskan teks interpolasi `$”/api/v1/analytics/sessions/{setup.SessionId}/transactions?userId={setup.ActingUserId}”`; nilai ekspresi di dalam
            // kurung kurawal disisipkan saat program berjalan sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/analytics/sessions/{setup.SessionId}/transactions?userId={setup.ActingUserId}",
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen ke `SendJsonAsync`.
            null,
            // Meneruskan `instructorToken` (nilai instruktur token) sebagai argumen ke `SendJsonAsync`.
            instructorToken);
        // Menyiapkan variabel lokal `transactions` untuk nilai transactions dengan hasil operasi asinkron membaca tanpa argumen menjadi objek bertipe
        // sesuai kontrak JSON melalui `transactionsResponse.Content.ReadFromJsonAsync<TransactionHistoryResponse>`; await menunggu hasil tanpa memblokir
        // thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var transactions = await transactionsResponse.Content.ReadFromJsonAsync<TransactionHistoryResponse>();
        // Menjalankan pemeriksaan NotNull atas `transactions` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // MahirInsurance_CannotBeUsedAfterPolicyBecomesInactive.
        Assert.NotNull(transactions);
        // Menjalankan pemeriksaan hasil dengan `Assert.Single` menggunakan `transactions.Items`, `item => item.Category == ”INSURANCE_OFFSET” &&
        // item.Amount == 10`; ketidaksesuaian dengan ekspektasi membuat pengujian gagal dalam MahirInsurance_CannotBeUsedAfterPolicyBecomesInactive.
        Assert.Single(transactions.Items, item => item.Category == "INSURANCE_OFFSET" && item.Amount == 10);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `transactions.Items`, `item =>
        // item.Category == ”EMERGENCY_OPTION” && item.Amount == 10` dalam MahirInsurance_CannotBeUsedAfterPolicyBecomesInactive.
        Assert.DoesNotContain(transactions.Items, item => item.Category == "EMERGENCY_OPTION" && item.Amount == 10);
    // Menutup scope metode MahirInsurance_CannotBeUsedAfterPolicyBecomesInactive; bagian berikut berada di luar batas blok tersebut dalam
    // MahirInsurance_CannotBeUsedAfterPolicyBecomesInactive.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `MahirGoldSell_UsesRelationalHoldingIncludingInitialGold` dengan hasil bertipe `Task`; operasi ini menangani mahir emas
    // sell uses relational holding including awal emas. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian
    // melalui Task.
    public async Task MahirGoldSell_UsesRelationalHoldingIncludingInitialGold()
    // Membuka scope metode MahirGoldSell_UsesRelationalHoldingIncludingInitialGold; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // MahirGoldSell_UsesRelationalHoldingIncludingInitialGold.
    {
        // Menyiapkan variabel lokal `suffix` untuk nilai suffix dengan `Guid.NewGuid().ToString(”N”)[..8]`, yaitu elemen koleksi yang dipilih melalui
        // indeks atau kunci tersebut. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var suffix = Guid.NewGuid().ToString("N")[..8];
        // Menyiapkan variabel lokal `instructorToken` untuk nilai instruktur token dengan `(await RegisterAsync( $”it_gold_instructor_{suffix}”,
        // ”IntegrationGoldInstructorPass!123”, ”INSTRUCTOR”)).AccessToken` (nilai akses token). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var instructorToken = (await RegisterAsync(
            // Meneruskan teks interpolasi `$”it_gold_instructor_{suffix}”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan sebagai
            // argumen ke `RegisterAsync`.
            $"it_gold_instructor_{suffix}",
            // Meneruskan nilai literal `”IntegrationGoldInstructorPass!123”` sebagai argumen ke `RegisterAsync`.
            "IntegrationGoldInstructorPass!123",
            // Meneruskan nilai literal `”INSTRUCTOR”` sebagai argumen ke `RegisterAsync`.
            "INSTRUCTOR")).AccessToken;
        // Menyiapkan variabel lokal `setup` untuk nilai setup dengan hasil operasi asinkron memanggil `CreateReadySessionAsync` dengan `instructorToken`,
        // `$”gold_{suffix}”`, `BuildRulesetDefinition(startingCash: 10, mode: ”MAHIR”)`; await menunggu hasil tanpa memblokir thread selama operasi belum
        // selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var setup = await CreateReadySessionAsync(
            // Meneruskan `instructorToken` (nilai instruktur token) sebagai argumen ke `CreateReadySessionAsync`.
            instructorToken,
            // Meneruskan teks interpolasi `$”gold_{suffix}”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan sebagai argumen ke
            // `CreateReadySessionAsync`.
            $"gold_{suffix}",
            // Meneruskan memanggil `BuildRulesetDefinition` dengan `10`, `”MAHIR”` sebagai argumen ke `CreateReadySessionAsync`; Meneruskan nilai literal `10`
            // sebagai argumen bernama `startingCash`; Meneruskan nilai literal `”MAHIR”` sebagai argumen bernama `mode`.
            BuildRulesetDefinition(startingCash: 10, mode: "MAHIR"));
        // Menyiapkan variabel lokal `now` untuk nilai now dengan `DateTimeOffset.UtcNow`, yaitu waktu UTC saat operasi dilakukan. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var now = DateTimeOffset.UtcNow;
        // Menyiapkan variabel lokal `nextSequence` untuk nilai next sequence dengan hasil operasi asinkron memanggil `AdvanceSessionToDayAsync` dengan
        // `setup`, `instructorToken`, `6`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var nextSequence = await AdvanceSessionToDayAsync(setup, instructorToken, targetDay: 6);

        // Mendefinisikan fungsi lokal SendEventAsync dengan hasil `Task<HttpResponseMessage>`; fungsi ini dipakai oleh alur di dalam scope yang sama.
        async Task<HttpResponseMessage> SendEventAsync(
            // Parameter `actionType` bertipe `string` membawa nilai aksi jenis.
            string actionType,
            // Parameter `actorType` bertipe `string` membawa nilai actor jenis.
            string actorType,
            // Parameter `userId` bertipe `Guid?` membawa identitas akun pengguna yang datanya sedang diproses; nilai null diizinkan ketika data opsional belum
            // tersedia.
            Guid? userId,
            // Parameter `sequence` bertipe `long` membawa nomor urut event yang menentukan urutan pemrosesan riwayat permainan.
            long sequence,
            // Parameter `payload` bertipe `object` membawa muatan detail event dalam format JSON.
            object payload)
        // Membuka scope fungsi lokal SendEventAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam SendEventAsync.
        {
            // Mengembalikan hasil operasi asinkron memanggil `SendJsonAsync` dengan `HttpMethod.Post`, `”/api/v1/events”`, `new { event_id = Guid.NewGuid(),
            // session_id = setup.SessionId, user_id = userId, actor_type = actorType, timestamp = now.AddSeconds(sequence), day_index = 6, weekday = ”SAT”,
            // ...`, `instructorToken`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai kepada pemanggil dalam SendEventAsync; eksekusi
            // jalur ini selesai setelah nilai hasil ditentukan.
            return await SendJsonAsync(HttpMethod.Post, "/api/v1/events", new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam SendEventAsync.
            {
                // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                event_id = Guid.NewGuid(),
                // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                session_id = setup.SessionId,
                // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                user_id = userId,
                // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                actor_type = actorType,
                // Meneruskan `sequence` (nomor urut event yang menentukan urutan pemrosesan riwayat permainan) sebagai argumen ke `now.AddSeconds`.
                timestamp = now.AddSeconds(sequence),
                // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                day_index = 6,
                // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                weekday = "SAT",
                // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                turn_number = actorType == "SYSTEM" ? 0 : 1,
                // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                action_slot = 0,
                // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                sequence_number = sequence,
                // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                action_type = actionType,
                // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                ruleset_version_id = setup.RulesetVersionId,
                // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                payload
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam SendEventAsync.
            }, instructorToken);
        // Menutup scope fungsi lokal SendEventAsync; bagian berikut berada di luar batas blok tersebut dalam SendEventAsync.
        }

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.Created`, `(await
        // SendEventAsync( ”BukaHargaEmas”, ”SYSTEM”, null, nextSequence, new { gold_price = 6 })).StatusCode`); pengujian gagal jika keduanya berbeda dalam
        // MahirGoldSell_UsesRelationalHoldingIncludingInitialGold.
        Assert.Equal(HttpStatusCode.Created, (await SendEventAsync(
            // Meneruskan nilai literal `”BukaHargaEmas”` sebagai argumen ke `SendEventAsync`; Meneruskan nilai literal `”SYSTEM”` sebagai argumen ke
            // `SendEventAsync`; Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen ke `SendEventAsync`; Meneruskan `nextSequence` (nilai next
            // sequence) sebagai argumen ke `SendEventAsync`; Meneruskan objek anonim yang mengelompokkan gold_price sebagai satu nilai sebagai argumen ke
            // `SendEventAsync`.
            "BukaHargaEmas", "SYSTEM", null, nextSequence, new { gold_price = 6 })).StatusCode);
        // Menyiapkan variabel lokal `firstSell` untuk nilai first sell dengan hasil operasi asinkron memanggil `SendEventAsync` dengan `”JualEmas”`,
        // `”PLAYER”`, `setup.ActingUserId`, `nextSequence + 1`, `new { trade_type = ”SELL”, qty = 1, unit_price = 6, amount = 6, asset_code = ”gold_card”
        // }`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var firstSell = await SendEventAsync(
            // Meneruskan nilai literal `”JualEmas”` sebagai argumen ke `SendEventAsync`; Meneruskan nilai literal `”PLAYER”` sebagai argumen ke
            // `SendEventAsync`; Meneruskan `setup.ActingUserId` (nilai acting pengguna identitas) sebagai argumen ke `SendEventAsync`; Meneruskan
            // penjumlahan/penggabungan antara `nextSequence` dan `1` sebagai argumen ke `SendEventAsync`; Meneruskan objek anonim yang mengelompokkan
            // trade_type, qty, unit_price, amount, asset_code sebagai satu nilai sebagai argumen ke `SendEventAsync`.
            "JualEmas", "PLAYER", setup.ActingUserId, nextSequence + 1, new { trade_type = "SELL", qty = 1, unit_price = 6, amount = 6, asset_code = "gold_card" });
        // Menjalankan pemeriksaan bahwa `firstSell.StatusCode == HttpStatusCode.Created`, `await firstSell.Content.ReadAsStringAsync()` bernilai benar;
        // pengujian gagal jika kondisi tidak terpenuhi dalam MahirGoldSell_UsesRelationalHoldingIncludingInitialGold.
        Assert.True(firstSell.StatusCode == HttpStatusCode.Created, await firstSell.Content.ReadAsStringAsync());

        // Menyiapkan variabel lokal `secondSell` untuk nilai second sell dengan hasil operasi asinkron memanggil `SendEventAsync` dengan `”JualEmas”`,
        // `”PLAYER”`, `setup.ActingUserId`, `nextSequence + 2`, `new { trade_type = ”SELL”, qty = 1, unit_price = 6, amount = 6, asset_code = ”gold_card”
        // }`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var secondSell = await SendEventAsync(
            // Meneruskan nilai literal `”JualEmas”` sebagai argumen ke `SendEventAsync`; Meneruskan nilai literal `”PLAYER”` sebagai argumen ke
            // `SendEventAsync`; Meneruskan `setup.ActingUserId` (nilai acting pengguna identitas) sebagai argumen ke `SendEventAsync`; Meneruskan
            // penjumlahan/penggabungan antara `nextSequence` dan `2` sebagai argumen ke `SendEventAsync`; Meneruskan objek anonim yang mengelompokkan
            // trade_type, qty, unit_price, amount, asset_code sebagai satu nilai sebagai argumen ke `SendEventAsync`.
            "JualEmas", "PLAYER", setup.ActingUserId, nextSequence + 2, new { trade_type = "SELL", qty = 1, unit_price = 6, amount = 6, asset_code = "gold_card" });
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.UnprocessableEntity`,
        // `secondSell.StatusCode`); pengujian gagal jika keduanya berbeda dalam MahirGoldSell_UsesRelationalHoldingIncludingInitialGold.
        Assert.Equal(HttpStatusCode.UnprocessableEntity, secondSell.StatusCode);
    // Menutup scope metode MahirGoldSell_UsesRelationalHoldingIncludingInitialGold; bagian berikut berada di luar batas blok tersebut dalam
    // MahirGoldSell_UsesRelationalHoldingIncludingInitialGold.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `Player_SeesOnlyOwnMissionUntilSessionEnds` dengan hasil bertipe `Task`; operasi ini menangani pemain sees only own misi
    // until sesi ends. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task.
    public async Task Player_SeesOnlyOwnMissionUntilSessionEnds()
    // Membuka scope metode Player_SeesOnlyOwnMissionUntilSessionEnds; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // Player_SeesOnlyOwnMissionUntilSessionEnds.
    {
        // Menyiapkan variabel lokal `suffix` untuk nilai suffix dengan `$”mission_{Guid.NewGuid():N}”[..17]`, yaitu elemen koleksi yang dipilih melalui
        // indeks atau kunci tersebut. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var suffix = $"mission_{Guid.NewGuid():N}"[..17];
        // Menyiapkan variabel lokal `instructorToken` untuk nilai instruktur token dengan `(await RegisterAsync(
        // $”it_mission_instructor_{Guid.NewGuid():N}”, ”IntegrationMissionInstructorPass!123”, ”INSTRUCTOR”)).AccessToken` (nilai akses token). Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var instructorToken = (await RegisterAsync(
            // Meneruskan teks interpolasi `$”it_mission_instructor_{Guid.NewGuid():N}”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program
            // berjalan sebagai argumen ke `RegisterAsync`.
            $"it_mission_instructor_{Guid.NewGuid():N}",
            // Meneruskan nilai literal `”IntegrationMissionInstructorPass!123”` sebagai argumen ke `RegisterAsync`.
            "IntegrationMissionInstructorPass!123",
            // Meneruskan nilai literal `”INSTRUCTOR”` sebagai argumen ke `RegisterAsync`.
            "INSTRUCTOR")).AccessToken;
        // Menyiapkan variabel lokal `setup` untuk nilai setup dengan hasil operasi asinkron memanggil `CreateReadySessionAsync` dengan `instructorToken`,
        // `suffix`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var setup = await CreateReadySessionAsync(instructorToken, suffix);
        // Menyiapkan variabel lokal `playerToken` untuk nilai pemain token dengan `(await LoginAsync( $”it_evt_invalid_player_{suffix}”,
        // ”IntegrationInvalidPlayerPass!123”)).AccessToken` (nilai akses token). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var playerToken = (await LoginAsync(
            // Meneruskan teks interpolasi `$”it_evt_invalid_player_{suffix}”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan sebagai
            // argumen ke `LoginAsync`.
            $"it_evt_invalid_player_{suffix}",
            // Meneruskan nilai literal `”IntegrationInvalidPlayerPass!123”` sebagai argumen ke `LoginAsync`.
            "IntegrationInvalidPlayerPass!123")).AccessToken;

        // Mendefinisikan fungsi lokal ReadMissionsAsync dengan hasil `Task<List<JsonElement>>`; fungsi ini dipakai oleh alur di dalam scope yang sama.
        async Task<List<JsonElement>> ReadMissionsAsync(string token)
        // Membuka scope fungsi lokal ReadMissionsAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ReadMissionsAsync.
        {
            // Menyiapkan variabel lokal `response` untuk hasil respons yang akan dibaca atau dikirim kepada pemanggil dengan hasil operasi asinkron memanggil
            // `SendJsonAsync` dengan `HttpMethod.Get`, `$”/api/v1/sessions/{setup.SessionId}/events?limit=100”`, `null`, `token`; await menunggu hasil tanpa
            // memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis
            // saat scope berakhir.
            using var response = await SendJsonAsync(
                // Meneruskan `HttpMethod.Get` (nilai get) sebagai argumen ke `SendJsonAsync`.
                HttpMethod.Get,
                // Meneruskan teks interpolasi `$”/api/v1/sessions/{setup.SessionId}/events?limit=100”`; nilai ekspresi di dalam kurung kurawal disisipkan saat
                // program berjalan sebagai argumen ke `SendJsonAsync`.
                $"/api/v1/sessions/{setup.SessionId}/events?limit=100",
                // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen ke `SendJsonAsync`.
                null,
                // Meneruskan `token` (token yang diteruskan pada operasi terkait) sebagai argumen ke `SendJsonAsync`.
                token);
            // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`, `response.StatusCode`);
            // pengujian gagal jika keduanya berbeda dalam ReadMissionsAsync.
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            // Menyiapkan variabel lokal `body` untuk nilai body dengan hasil operasi asinkron memanggil `ReadJsonAsync` dengan `response`; await menunggu hasil
            // tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas
            // otomatis saat scope berakhir.
            using var body = await ReadJsonAsync(response);
            // Mengembalikan mematerialisasi urutan `body.RootElement.GetProperty(”items”) .EnumerateArray() .Where(item =>
            // item.GetProperty(”action_type”).GetString() == ”SetupMisiAwal”) .Select(item => item.Clone())` menjadi List; enumerasi dijalankan dan hasilnya
            // disimpan dalam memori kepada pemanggil dalam ReadMissionsAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return body.RootElement.GetProperty("items")
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .EnumerateArray() dalam ReadMissionsAsync; token pada baris ini menyambungkan
                // bagian kode sebelum dan sesudahnya.
                .EnumerateArray()
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(item => item.GetProperty(”action_type”).GetString() == ”SetupMisiAwal”)
                // dalam ReadMissionsAsync; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
                .Where(item => item.GetProperty("action_type").GetString() == "SetupMisiAwal")
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Select(item => item.Clone()) dalam ReadMissionsAsync; token pada baris ini
                // menyambungkan bagian kode sebelum dan sesudahnya.
                .Select(item => item.Clone())
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList(); dalam ReadMissionsAsync; token pada baris ini menyambungkan bagian
                // kode sebelum dan sesudahnya.
                .ToList();
        // Menutup scope fungsi lokal ReadMissionsAsync; bagian berikut berada di luar batas blok tersebut dalam ReadMissionsAsync.
        }

        // Menyiapkan variabel lokal `playerMissions` untuk nilai pemain misi dengan hasil operasi asinkron memanggil `ReadMissionsAsync` dengan
        // `playerToken`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var playerMissions = await ReadMissionsAsync(playerToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`3`, `playerMissions.Count`); pengujian gagal
        // jika keduanya berbeda dalam Player_SeesOnlyOwnMissionUntilSessionEnds.
        Assert.Equal(3, playerMissions.Count);
        // Menyiapkan variabel lokal `ownMission` untuk nilai own misi dengan pemeriksaan hasil dengan `Assert.Single` menggunakan `playerMissions`, `item
        // => item.GetProperty(”user_id”).GetGuid() == setup.UserId`; ketidaksesuaian dengan ekspektasi membuat pengujian gagal. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var ownMission = Assert.Single(
            // Meneruskan `playerMissions` (nilai pemain misi) sebagai argumen ke `Assert.Single`.
            playerMissions,
            // Parameter `item` bertipe `` membawa nilai elemen.
            item => item.GetProperty("user_id").GetGuid() == setup.UserId);
        // Menjalankan pemeriksaan bahwa `ownMission.GetProperty(”payload”).TryGetProperty(”mission_id”, out _)` bernilai benar; pengujian gagal jika
        // kondisi tidak terpenuhi dalam Player_SeesOnlyOwnMissionUntilSessionEnds.
        Assert.True(ownMission.GetProperty("payload").TryGetProperty("mission_id", out _));
        // Menjalankan pemeriksaan hasil dengan `Assert.All` menggunakan `playerMissions.Where(item => item.GetProperty(”user_id”).GetGuid() !=
        // setup.UserId)`, `item => { var payload = item.GetProperty(”payload”); Assert.Equal(”HIDDEN”, payload.GetProperty(”status”).GetString());
        // Assert.False(payload.TryGetProperty(”mission_id”, out _...`; ketidaksesuaian dengan ekspektasi membuat pengujian gagal dalam
        // Player_SeesOnlyOwnMissionUntilSessionEnds.
        Assert.All(
            // Meneruskan menyaring elemen `playerMissions` dengan predikat `item => item.GetProperty(”user_id”).GetGuid() != setup.UserId`; hanya elemen yang
            // memenuhi kondisi diteruskan sebagai argumen ke `Assert.All`; Meneruskan fungsi lambda `item => item.GetProperty(”user_id”).GetGuid() !=
            // setup.UserId` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen ke `playerMissions.Where`; Meneruskan nilai
            // literal `”user_id”` sebagai argumen ke `item.GetProperty`.
            playerMissions.Where(item => item.GetProperty("user_id").GetGuid() != setup.UserId),
            // Parameter `item` bertipe `` membawa nilai elemen.
            item =>
            // Membuka scope fungsi lambda yang dipasok ke `Assert.All`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // Player_SeesOnlyOwnMissionUntilSessionEnds.
            {
                // Menyiapkan variabel lokal `payload` untuk muatan detail event dalam format JSON dengan memanggil `item.GetProperty` dengan `”payload”`. Tipe
                // variabel disimpulkan dari ekspresi nilai awal.
                var payload = item.GetProperty("payload");
                // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”HIDDEN”`,
                // `payload.GetProperty(”status”).GetString()`); pengujian gagal jika keduanya berbeda dalam Player_SeesOnlyOwnMissionUntilSessionEnds.
                Assert.Equal("HIDDEN", payload.GetProperty("status").GetString());
                // Menjalankan pemeriksaan bahwa `payload.TryGetProperty(”mission_id”, out _)` bernilai salah; pengujian gagal jika kondisi justru terpenuhi dalam
                // Player_SeesOnlyOwnMissionUntilSessionEnds.
                Assert.False(payload.TryGetProperty("mission_id", out _));
            // Menutup scope fungsi lambda yang dipasok ke `Assert.All`; bagian berikut berada di luar batas blok tersebut dalam
            // Player_SeesOnlyOwnMissionUntilSessionEnds.
            });

        // Menjalankan pemeriksaan hasil dengan `Assert.All` menggunakan `await ReadMissionsAsync(instructorToken)`, `item =>
        // Assert.True(item.GetProperty(”payload”).TryGetProperty(”mission_id”, out _))`; ketidaksesuaian dengan ekspektasi membuat pengujian gagal dalam
        // Player_SeesOnlyOwnMissionUntilSessionEnds.
        Assert.All(
            // Meneruskan hasil operasi asinkron memanggil `ReadMissionsAsync` dengan `instructorToken`; await menunggu hasil tanpa memblokir thread selama
            // operasi belum selesai sebagai argumen ke `Assert.All`; Meneruskan `instructorToken` (nilai instruktur token) sebagai argumen ke
            // `ReadMissionsAsync`.
            await ReadMissionsAsync(instructorToken),
            // Parameter `item` bertipe `` membawa nilai elemen.
            item => Assert.True(item.GetProperty("payload").TryGetProperty("mission_id", out _)));

        // Menyiapkan variabel lokal `endResponse` untuk nilai end respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan `HttpMethod.Post`,
        // `$”/api/v1/sessions/{setup.SessionId}/end”`, `null`, `instructorToken`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai.
        // Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var endResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Post` (nilai post) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Post,
            // Meneruskan teks interpolasi `$”/api/v1/sessions/{setup.SessionId}/end”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan
            // sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/sessions/{setup.SessionId}/end",
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen ke `SendJsonAsync`.
            null,
            // Meneruskan `instructorToken` (nilai instruktur token) sebagai argumen ke `SendJsonAsync`.
            instructorToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`, `endResponse.StatusCode`);
        // pengujian gagal jika keduanya berbeda dalam Player_SeesOnlyOwnMissionUntilSessionEnds.
        Assert.Equal(HttpStatusCode.OK, endResponse.StatusCode);
        // Menjalankan pemeriksaan hasil dengan `Assert.All` menggunakan `await ReadMissionsAsync(playerToken)`, `item =>
        // Assert.True(item.GetProperty(”payload”).TryGetProperty(”mission_id”, out _))`; ketidaksesuaian dengan ekspektasi membuat pengujian gagal dalam
        // Player_SeesOnlyOwnMissionUntilSessionEnds.
        Assert.All(
            // Meneruskan hasil operasi asinkron memanggil `ReadMissionsAsync` dengan `playerToken`; await menunggu hasil tanpa memblokir thread selama operasi
            // belum selesai sebagai argumen ke `Assert.All`; Meneruskan `playerToken` (nilai pemain token) sebagai argumen ke `ReadMissionsAsync`.
            await ReadMissionsAsync(playerToken),
            // Parameter `item` bertipe `` membawa nilai elemen.
            item => Assert.True(item.GetProperty("payload").TryGetProperty("mission_id", out _)));
    // Menutup scope metode Player_SeesOnlyOwnMissionUntilSessionEnds; bagian berikut berada di luar batas blok tersebut dalam
    // Player_SeesOnlyOwnMissionUntilSessionEnds.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    /// <summary>
    /// Memvalidasi bahwa PLAYER tidak dapat mengakses event atau mengirim event ke sesi
    /// yang bukan miliknya, dan menerima error FORBIDDEN.
    /// </summary>
    // Mendefinisikan metode `Player_CannotAccessOrIngestEvents_OnForeignSession` dengan hasil bertipe `Task`; operasi ini menangani pemain cannot akses
    // atau ingest event on foreign sesi. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task.
    public async Task Player_CannotAccessOrIngestEvents_OnForeignSession()
    // Membuka scope metode Player_CannotAccessOrIngestEvents_OnForeignSession; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // Player_CannotAccessOrIngestEvents_OnForeignSession.
    {
        // Menyiapkan variabel lokal `suffix` untuk nilai suffix dengan `Guid.NewGuid().ToString(”N”)[..8]`, yaitu elemen koleksi yang dipilih melalui
        // indeks atau kunci tersebut. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var suffix = Guid.NewGuid().ToString("N")[..8];
        // Menyiapkan variabel lokal `instructorAUsername` untuk nilai instruktur a username dengan teks interpolasi
        // `$”it_evt_scope_instructor_a_{suffix}”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var instructorAUsername = $"it_evt_scope_instructor_a_{suffix}";
        // Menyiapkan variabel lokal `instructorBUsername` untuk nilai instruktur b username dengan teks interpolasi
        // `$”it_evt_scope_instructor_b_{suffix}”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var instructorBUsername = $"it_evt_scope_instructor_b_{suffix}";
        // Menyiapkan variabel lokal `instructorPassword` untuk nilai instruktur password dengan nilai literal `”IntegrationScopeInstructorPass!123”`. Tipe
        // yang dipakai adalah `string`.
        const string instructorPassword = "IntegrationScopeInstructorPass!123";
        // Menyiapkan variabel lokal `playerPassword` untuk nilai pemain password dengan nilai literal `”IntegrationInvalidPlayerPass!123”`. Tipe yang
        // dipakai adalah `string`.
        const string playerPassword = "IntegrationInvalidPlayerPass!123";
        // Menyiapkan variabel lokal `ownScopeSuffix` untuk nilai own cakupan suffix dengan teks interpolasi `$”scope_own_{suffix}”`; nilai ekspresi di
        // dalam kurung kurawal disisipkan saat program berjalan. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var ownScopeSuffix = $"scope_own_{suffix}";
        // Menyiapkan variabel lokal `foreignScopeSuffix` untuk nilai foreign cakupan suffix dengan teks interpolasi `$”scope_foreign_{suffix}”`; nilai
        // ekspresi di dalam kurung kurawal disisipkan saat program berjalan. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var foreignScopeSuffix = $"scope_foreign_{suffix}";

        // Menyiapkan variabel lokal `instructorAToken` untuk nilai instruktur a token dengan `(await RegisterAsync(instructorAUsername, instructorPassword,
        // ”INSTRUCTOR”)).AccessToken` (nilai akses token). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var instructorAToken = (await RegisterAsync(instructorAUsername, instructorPassword, "INSTRUCTOR")).AccessToken;
        // Menyiapkan variabel lokal `instructorBToken` untuk nilai instruktur b token dengan `(await RegisterAsync(instructorBUsername, instructorPassword,
        // ”INSTRUCTOR”)).AccessToken` (nilai akses token). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var instructorBToken = (await RegisterAsync(instructorBUsername, instructorPassword, "INSTRUCTOR")).AccessToken;

        // Menyiapkan variabel lokal `ownSession` untuk nilai own sesi dengan hasil operasi asinkron memanggil `CreateReadySessionAsync` dengan
        // `instructorAToken`, `ownScopeSuffix`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var ownSession = await CreateReadySessionAsync(instructorAToken, ownScopeSuffix);
        // Menyiapkan variabel lokal `foreignSession` untuk nilai foreign sesi dengan hasil operasi asinkron memanggil `CreateReadySessionAsync` dengan
        // `instructorBToken`, `foreignScopeSuffix`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var foreignSession = await CreateReadySessionAsync(instructorBToken, foreignScopeSuffix);

        // Menyiapkan variabel lokal `playerUsername` untuk nilai pemain username dengan teks interpolasi `$”it_evt_invalid_player_{ownScopeSuffix}”`; nilai
        // ekspresi di dalam kurung kurawal disisipkan saat program berjalan. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var playerUsername = $"it_evt_invalid_player_{ownScopeSuffix}";
        // Menyiapkan variabel lokal `playerToken` untuk nilai pemain token dengan `(await LoginAsync(playerUsername, playerPassword)).AccessToken` (nilai
        // akses token). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var playerToken = (await LoginAsync(playerUsername, playerPassword)).AccessToken;

        // Menyiapkan variabel lokal `ownEventsResponse` untuk nilai own event respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Get`, `$”/api/v1/sessions/{ownSession.SessionId}/events?limit=10”`, `null`, `playerToken`; await menunggu hasil tanpa memblokir
        // thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var ownEventsResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Get` (nilai get) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Get,
            // Meneruskan teks interpolasi `$”/api/v1/sessions/{ownSession.SessionId}/events?limit=10”`; nilai ekspresi di dalam kurung kurawal disisipkan saat
            // program berjalan sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/sessions/{ownSession.SessionId}/events?limit=10",
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen ke `SendJsonAsync`.
            null,
            // Meneruskan `playerToken` (nilai pemain token) sebagai argumen ke `SendJsonAsync`.
            playerToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`,
        // `ownEventsResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam Player_CannotAccessOrIngestEvents_OnForeignSession.
        Assert.Equal(HttpStatusCode.OK, ownEventsResponse.StatusCode);

        // Menyiapkan variabel lokal `foreignEventsResponse` untuk nilai foreign event respons dengan hasil operasi asinkron memanggil `SendJsonAsync`
        // dengan `HttpMethod.Get`, `$”/api/v1/sessions/{foreignSession.SessionId}/events?limit=10”`, `null`, `playerToken`; await menunggu hasil tanpa
        // memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var foreignEventsResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Get` (nilai get) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Get,
            // Meneruskan teks interpolasi `$”/api/v1/sessions/{foreignSession.SessionId}/events?limit=10”`; nilai ekspresi di dalam kurung kurawal disisipkan
            // saat program berjalan sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/sessions/{foreignSession.SessionId}/events?limit=10",
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen ke `SendJsonAsync`.
            null,
            // Meneruskan `playerToken` (nilai pemain token) sebagai argumen ke `SendJsonAsync`.
            playerToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.Forbidden`,
        // `foreignEventsResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam Player_CannotAccessOrIngestEvents_OnForeignSession.
        Assert.Equal(HttpStatusCode.Forbidden, foreignEventsResponse.StatusCode);

        // Menyiapkan variabel lokal `foreignEventsError` untuk nilai foreign event kesalahan dengan hasil operasi asinkron membaca tanpa argumen menjadi
        // objek bertipe sesuai kontrak JSON melalui `foreignEventsResponse.Content.ReadFromJsonAsync<ErrorResponse>`; await menunggu hasil tanpa memblokir
        // thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var foreignEventsError = await foreignEventsResponse.Content.ReadFromJsonAsync<ErrorResponse>();
        // Menjalankan pemeriksaan NotNull atas `foreignEventsError` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // Player_CannotAccessOrIngestEvents_OnForeignSession.
        Assert.NotNull(foreignEventsError);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”FORBIDDEN”`, `foreignEventsError.ErrorCode`);
        // pengujian gagal jika keduanya berbeda dalam Player_CannotAccessOrIngestEvents_OnForeignSession.
        Assert.Equal("FORBIDDEN", foreignEventsError.ErrorCode);

        // Menyiapkan variabel lokal `now` untuk nilai now dengan `DateTimeOffset.UtcNow`, yaitu waktu UTC saat operasi dilakukan. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var now = DateTimeOffset.UtcNow;
        // Menyiapkan variabel lokal `foreignSystemEventPayload` untuk nilai foreign system event payload dengan objek anonim yang mengelompokkan event_id,
        // session_id, user_id, actor_type, timestamp, day_index, weekday, action_slot, sequence_number, action_type, ruleset_version_id, payload sebagai
        // satu nilai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var foreignSystemEventPayload = new
        // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // Player_CannotAccessOrIngestEvents_OnForeignSession.
        {
            // Menggunakan `event_id` (nilai event identitas) sebagai bagian ekspresi yang sedang disusun dalam
            // Player_CannotAccessOrIngestEvents_OnForeignSession.
            event_id = Guid.NewGuid(),
            // Menggunakan `session_id` (nilai sesi identitas) sebagai bagian ekspresi yang sedang disusun dalam
            // Player_CannotAccessOrIngestEvents_OnForeignSession.
            session_id = foreignSession.SessionId,
            // Menggunakan `user_id` (nilai pengguna identitas) sebagai bagian ekspresi yang sedang disusun dalam
            // Player_CannotAccessOrIngestEvents_OnForeignSession.
            user_id = (Guid?)null,
            // Menggunakan `actor_type` (nilai actor jenis) sebagai bagian ekspresi yang sedang disusun dalam
            // Player_CannotAccessOrIngestEvents_OnForeignSession.
            actor_type = "SYSTEM",
            // Menggunakan `timestamp` (waktu kejadian yang menjaga urutan kronologis data) sebagai bagian ekspresi yang sedang disusun dalam
            // Player_CannotAccessOrIngestEvents_OnForeignSession.
            timestamp = now.ToString("O"),
            // Menggunakan `day_index` (nilai hari index) sebagai bagian ekspresi yang sedang disusun dalam Player_CannotAccessOrIngestEvents_OnForeignSession.
            day_index = 0,
            // Menggunakan `weekday` (nilai weekday) sebagai bagian ekspresi yang sedang disusun dalam Player_CannotAccessOrIngestEvents_OnForeignSession.
            weekday = "MON",
            // Menggunakan `action_slot` (nilai aksi slot) sebagai bagian ekspresi yang sedang disusun dalam Player_CannotAccessOrIngestEvents_OnForeignSession.
            action_slot = 1,
            // Menggunakan `sequence_number` (nilai sequence number) sebagai bagian ekspresi yang sedang disusun dalam
            // Player_CannotAccessOrIngestEvents_OnForeignSession.
            sequence_number = 1,
            // Menggunakan `action_type` (nilai aksi jenis) sebagai bagian ekspresi yang sedang disusun dalam
            // Player_CannotAccessOrIngestEvents_OnForeignSession.
            action_type = "CatatTransaksi",
            // Menggunakan `ruleset_version_id` (nilai aturan versi identitas) sebagai bagian ekspresi yang sedang disusun dalam
            // Player_CannotAccessOrIngestEvents_OnForeignSession.
            ruleset_version_id = foreignSession.RulesetVersionId,
            // Menggunakan `payload` (muatan detail event dalam format JSON) sebagai bagian ekspresi yang sedang disusun dalam
            // Player_CannotAccessOrIngestEvents_OnForeignSession.
            payload = new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // Player_CannotAccessOrIngestEvents_OnForeignSession.
            {
                // Menggunakan `direction` (nilai direction) sebagai bagian ekspresi yang sedang disusun dalam Player_CannotAccessOrIngestEvents_OnForeignSession.
                direction = "IN",
                // Menggunakan `amount` (nominal uang atau nilai transaksi yang dipakai dalam operasi) sebagai bagian ekspresi yang sedang disusun dalam
                // Player_CannotAccessOrIngestEvents_OnForeignSession.
                amount = 1,
                // Menggunakan `category` (nilai category) sebagai bagian ekspresi yang sedang disusun dalam Player_CannotAccessOrIngestEvents_OnForeignSession.
                category = "NEED_PRIMARY",
                // Menggunakan `counterparty` (nilai counterparty) sebagai bagian ekspresi yang sedang disusun dalam
                // Player_CannotAccessOrIngestEvents_OnForeignSession.
                counterparty = "BANK"
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
            // Player_CannotAccessOrIngestEvents_OnForeignSession.
            }
        // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
        // Player_CannotAccessOrIngestEvents_OnForeignSession.
        };

        // Menyiapkan variabel lokal `foreignIngestResponse` untuk nilai foreign ingest respons dengan hasil operasi asinkron memanggil `SendJsonAsync`
        // dengan `HttpMethod.Post`, `”/api/v1/events”`, `foreignSystemEventPayload`, `playerToken`; await menunggu hasil tanpa memblokir thread selama
        // operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var foreignIngestResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Post` (nilai post) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Post,
            // Meneruskan nilai literal `”/api/v1/events”` sebagai argumen ke `SendJsonAsync`.
            "/api/v1/events",
            // Meneruskan `foreignSystemEventPayload` (nilai foreign system event payload) sebagai argumen ke `SendJsonAsync`.
            foreignSystemEventPayload,
            // Meneruskan `playerToken` (nilai pemain token) sebagai argumen ke `SendJsonAsync`.
            playerToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.Forbidden`,
        // `foreignIngestResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam Player_CannotAccessOrIngestEvents_OnForeignSession.
        Assert.Equal(HttpStatusCode.Forbidden, foreignIngestResponse.StatusCode);

        // Menyiapkan variabel lokal `foreignIngestError` untuk nilai foreign ingest kesalahan dengan hasil operasi asinkron membaca tanpa argumen menjadi
        // objek bertipe sesuai kontrak JSON melalui `foreignIngestResponse.Content.ReadFromJsonAsync<ErrorResponse>`; await menunggu hasil tanpa memblokir
        // thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var foreignIngestError = await foreignIngestResponse.Content.ReadFromJsonAsync<ErrorResponse>();
        // Menjalankan pemeriksaan NotNull atas `foreignIngestError` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // Player_CannotAccessOrIngestEvents_OnForeignSession.
        Assert.NotNull(foreignIngestError);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”FORBIDDEN”`, `foreignIngestError.ErrorCode`);
        // pengujian gagal jika keduanya berbeda dalam Player_CannotAccessOrIngestEvents_OnForeignSession.
        Assert.Equal("FORBIDDEN", foreignIngestError.ErrorCode);
    // Menutup scope metode Player_CannotAccessOrIngestEvents_OnForeignSession; bagian berikut berada di luar batas blok tersebut dalam
    // Player_CannotAccessOrIngestEvents_OnForeignSession.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    /// <summary>
    /// Memvalidasi bahwa penambahan pemain dengan role tidak valid (bukan PLAYER/OBSERVER)
    /// ditolak dengan error VALIDATION_ERROR dan detail field "role" INVALID_ENUM.
    /// </summary>
    // Mendefinisikan metode `AddPlayerToSession_ExtraRoleField_IsIgnoredByParticipantContract` dengan hasil bertipe `Task`; operasi ini menangani add
    // pemain ke sesi extra role field berstatus ignored berdasarkan participant contract. async memungkinkan metode menunggu operasi I/O dengan await
    // dan mengembalikan penyelesaian melalui Task.
    public async Task AddPlayerToSession_ExtraRoleField_IsIgnoredByParticipantContract()
    // Membuka scope metode AddPlayerToSession_ExtraRoleField_IsIgnoredByParticipantContract; pernyataan/deklarasi berikut berada di dalam batas blok
    // ini dalam AddPlayerToSession_ExtraRoleField_IsIgnoredByParticipantContract.
    {
        // Menyiapkan variabel lokal `suffix` untuk nilai suffix dengan `Guid.NewGuid().ToString(”N”)[..8]`, yaitu elemen koleksi yang dipilih melalui
        // indeks atau kunci tersebut. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var suffix = Guid.NewGuid().ToString("N")[..8];
        // Menyiapkan variabel lokal `instructorUsername` untuk nilai instruktur username dengan teks interpolasi
        // `$”it_evt_role_guard_instructor_{suffix}”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var instructorUsername = $"it_evt_role_guard_instructor_{suffix}";
        // Menyiapkan variabel lokal `instructorPassword` untuk nilai instruktur password dengan nilai literal `”IntegrationRoleGuardInstructorPass!123”`.
        // Tipe yang dipakai adalah `string`.
        const string instructorPassword = "IntegrationRoleGuardInstructorPass!123";
        // Menyiapkan variabel lokal `instructorToken` untuk nilai instruktur token dengan `(await RegisterAsync(instructorUsername, instructorPassword,
        // ”INSTRUCTOR”)).AccessToken` (nilai akses token). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var instructorToken = (await RegisterAsync(instructorUsername, instructorPassword, "INSTRUCTOR")).AccessToken;

        // Menyiapkan variabel lokal `setup` untuk nilai setup dengan hasil operasi asinkron memanggil `CreateReadySessionAsync` dengan `instructorToken`,
        // `$”role_guard_{suffix}”`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai
        // awal.
        var setup = await CreateReadySessionAsync(instructorToken, $"role_guard_{suffix}");
        // Menyiapkan variabel lokal `createPlayerResponse` untuk nilai create pemain respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Post`, `”/api/v1/players”`, `new { display_name = $”Player Role Guard {suffix}”, username = $”it_evt_role_guard_player_{suffix}”,
        // password = ”IntegrationRoleGuardPlayerPass!123” }`, `instructorToken`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai.
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var createPlayerResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Post` (nilai post) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Post,
            // Meneruskan nilai literal `”/api/v1/players”` sebagai argumen ke `SendJsonAsync`.
            "/api/v1/players",
            // Meneruskan objek anonim yang mengelompokkan display_name, username, password sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // AddPlayerToSession_ExtraRoleField_IsIgnoredByParticipantContract.
            {
                // Meneruskan objek anonim yang mengelompokkan display_name, username, password sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                display_name = $"Player Role Guard {suffix}",
                // Meneruskan objek anonim yang mengelompokkan display_name, username, password sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                username = $"it_evt_role_guard_player_{suffix}",
                // Meneruskan objek anonim yang mengelompokkan display_name, username, password sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                password = "IntegrationRoleGuardPlayerPass!123"
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
            // AddPlayerToSession_ExtraRoleField_IsIgnoredByParticipantContract.
            },
            // Meneruskan `instructorToken` (nilai instruktur token) sebagai argumen ke `SendJsonAsync`.
            instructorToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.Created`,
        // `createPlayerResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam AddPlayerToSession_ExtraRoleField_IsIgnoredByParticipantContract.
        Assert.Equal(HttpStatusCode.Created, createPlayerResponse.StatusCode);

        // Menyiapkan variabel lokal `createdPlayer` untuk nilai created pemain dengan hasil operasi asinkron membaca tanpa argumen menjadi objek bertipe
        // sesuai kontrak JSON melalui `createPlayerResponse.Content.ReadFromJsonAsync<PlayerResponse>`; await menunggu hasil tanpa memblokir thread selama
        // operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var createdPlayer = await createPlayerResponse.Content.ReadFromJsonAsync<PlayerResponse>();
        // Menjalankan pemeriksaan NotNull atas `createdPlayer` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // AddPlayerToSession_ExtraRoleField_IsIgnoredByParticipantContract.
        Assert.NotNull(createdPlayer);

        // Menyiapkan variabel lokal `addWithInvalidRoleResponse` untuk nilai add dengan invalid role respons dengan hasil operasi asinkron memanggil
        // `SendJsonAsync` dengan `HttpMethod.Post`, `$”/api/v1/sessions/{setup.SessionId}/players”`, `new { user_id = createdPlayer.UserId, role = ”ADMIN”
        // }`, `instructorToken`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai
        // awal.
        var addWithInvalidRoleResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Post` (nilai post) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Post,
            // Meneruskan teks interpolasi `$”/api/v1/sessions/{setup.SessionId}/players”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program
            // berjalan sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/sessions/{setup.SessionId}/players",
            // Meneruskan objek anonim yang mengelompokkan user_id, role sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // AddPlayerToSession_ExtraRoleField_IsIgnoredByParticipantContract.
            {
                // Meneruskan objek anonim yang mengelompokkan user_id, role sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                user_id = createdPlayer.UserId,
                // Meneruskan objek anonim yang mengelompokkan user_id, role sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                role = "ADMIN"
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
            // AddPlayerToSession_ExtraRoleField_IsIgnoredByParticipantContract.
            },
            // Meneruskan `instructorToken` (nilai instruktur token) sebagai argumen ke `SendJsonAsync`.
            instructorToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.Conflict`,
        // `addWithInvalidRoleResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam
        // AddPlayerToSession_ExtraRoleField_IsIgnoredByParticipantContract.
        Assert.Equal(HttpStatusCode.Conflict, addWithInvalidRoleResponse.StatusCode);

        // Menyiapkan variabel lokal `error` untuk informasi kesalahan yang dikembalikan atau dicatat ketika operasi tidak berhasil dengan hasil operasi
        // asinkron membaca tanpa argumen menjadi objek bertipe sesuai kontrak JSON melalui
        // `addWithInvalidRoleResponse.Content.ReadFromJsonAsync<ErrorResponse>`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai.
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var error = await addWithInvalidRoleResponse.Content.ReadFromJsonAsync<ErrorResponse>();
        // Menjalankan pemeriksaan NotNull atas `error` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // AddPlayerToSession_ExtraRoleField_IsIgnoredByParticipantContract.
        Assert.NotNull(error);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”SESSION_ROSTER_LOCKED”`, `error.ErrorCode`);
        // pengujian gagal jika keduanya berbeda dalam AddPlayerToSession_ExtraRoleField_IsIgnoredByParticipantContract.
        Assert.Equal("SESSION_ROSTER_LOCKED", error.ErrorCode);
    // Menutup scope metode AddPlayerToSession_ExtraRoleField_IsIgnoredByParticipantContract; bagian berikut berada di luar batas blok tersebut dalam
    // AddPlayerToSession_ExtraRoleField_IsIgnoredByParticipantContract.
    }

    /// <summary>
    /// Helper yang membuat ruleset, sesi, player, dan menjalankan sesi hingga siap
    /// untuk menerima event, lalu mengembalikan ID sesi, player, dan versi ruleset aktif.
    /// </summary>
    // Mendefinisikan metode `CreateReadySessionAsync` dengan hasil bertipe `Task<(Guid SessionId, Guid UserId, Guid ActingUserId, Guid
    // RulesetVersionId, long NextSequenceNumber)>`. Helper yang membuat ruleset, sesi, player, dan menjalankan sesi hingga siap untuk menerima event,
    // lalu mengembalikan ID sesi, player, dan versi ruleset aktif. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan
    // penyelesaian melalui Task. Masukan: Parameter `instructorToken` bertipe `string` membawa nilai instruktur token; Parameter `suffix` bertipe
    // `string` membawa nilai suffix; Parameter `definition` bertipe `RulesetDefinitionDto?` membawa definisi terstruktur komponen serta parameter
    // aturan permainan; nilai null diizinkan ketika data opsional belum tersedia; bila argumen tidak diberikan digunakan null, yaitu penanda tidak ada
    // nilai.
    private async Task<(Guid SessionId, Guid UserId, Guid ActingUserId, Guid RulesetVersionId, long NextSequenceNumber)> CreateReadySessionAsync(
        // Parameter `instructorToken` bertipe `string` membawa nilai instruktur token.
        string instructorToken,
        // Parameter `suffix` bertipe `string` membawa nilai suffix.
        string suffix,
        // Parameter `definition` bertipe `RulesetDefinitionDto?` membawa definisi terstruktur komponen serta parameter aturan permainan; nilai null
        // diizinkan ketika data opsional belum tersedia; bila argumen tidak diberikan digunakan null, yaitu penanda tidak ada nilai.
        RulesetDefinitionDto? definition = null)
    // Membuka scope metode CreateReadySessionAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CreateReadySessionAsync.
    {
        // Memperbarui `definition` hanya jika nilainya null, menggunakan memanggil `BuildRulesetDefinition` dengan `50` dalam CreateReadySessionAsync.
        definition ??= BuildRulesetDefinition(startingCash: 50);
        // Menyiapkan variabel lokal `createRulesetPayload` untuk nilai create aturan payload dengan objek anonim yang mengelompokkan name, description,
        // definition sebagai satu nilai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var createRulesetPayload = new
        // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // CreateReadySessionAsync.
        {
            // Menggunakan `name` (nilai nama) sebagai bagian ekspresi yang sedang disusun dalam CreateReadySessionAsync.
            name = $"Ruleset Invalid IT {suffix}",
            // Menggunakan `description` (nilai description) sebagai bagian ekspresi yang sedang disusun dalam CreateReadySessionAsync.
            description = "Integration invalid event validation",
            // Menggunakan `definition` (definisi terstruktur komponen serta parameter aturan permainan) sebagai bagian ekspresi yang sedang disusun dalam
            // CreateReadySessionAsync.
            definition
        // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam CreateReadySessionAsync.
        };

        // Menyiapkan variabel lokal `createRulesetResponse` untuk nilai create aturan respons dengan hasil operasi asinkron memanggil `SendJsonAsync`
        // dengan `HttpMethod.Post`, `”/api/v1/rulesets”`, `createRulesetPayload`, `instructorToken`; await menunggu hasil tanpa memblokir thread selama
        // operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var createRulesetResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Post` (nilai post) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Post,
            // Meneruskan nilai literal `”/api/v1/rulesets”` sebagai argumen ke `SendJsonAsync`.
            "/api/v1/rulesets",
            // Meneruskan `createRulesetPayload` (nilai create aturan payload) sebagai argumen ke `SendJsonAsync`.
            createRulesetPayload,
            // Meneruskan `instructorToken` (nilai instruktur token) sebagai argumen ke `SendJsonAsync`.
            instructorToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.Created`,
        // `createRulesetResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam CreateReadySessionAsync.
        Assert.Equal(HttpStatusCode.Created, createRulesetResponse.StatusCode);

        // Menyiapkan variabel lokal `createdRuleset` untuk nilai created aturan dengan hasil operasi asinkron membaca tanpa argumen menjadi objek bertipe
        // sesuai kontrak JSON melalui `createRulesetResponse.Content.ReadFromJsonAsync<CreateRulesetResponse>`; await menunggu hasil tanpa memblokir thread
        // selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var createdRuleset = await createRulesetResponse.Content.ReadFromJsonAsync<CreateRulesetResponse>();
        // Menjalankan pemeriksaan NotNull atas `createdRuleset` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam CreateReadySessionAsync.
        Assert.NotNull(createdRuleset);

        // Menyiapkan variabel lokal `createSessionPayload` untuk nilai create sesi payload dengan objek anonim yang mengelompokkan session_name, mode,
        // ruleset_version_id sebagai satu nilai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var createSessionPayload = new
        // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // CreateReadySessionAsync.
        {
            // Menggunakan `session_name` (nilai sesi nama) sebagai bagian ekspresi yang sedang disusun dalam CreateReadySessionAsync.
            session_name = $"Session Invalid IT {suffix}",
            // Menggunakan `mode` (mode permainan yang menentukan kelompok aturan yang digunakan) sebagai bagian ekspresi yang sedang disusun dalam
            // CreateReadySessionAsync.
            mode = definition.Mode,
            // Menggunakan `ruleset_version_id` (nilai aturan versi identitas) sebagai bagian ekspresi yang sedang disusun dalam CreateReadySessionAsync.
            ruleset_version_id = createdRuleset.RulesetVersionId
        // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam CreateReadySessionAsync.
        };

        // Menyiapkan variabel lokal `createSessionResponse` untuk nilai create sesi respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Post`, `”/api/v1/sessions”`, `createSessionPayload`, `instructorToken`; await menunggu hasil tanpa memblokir thread selama operasi
        // belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var createSessionResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Post` (nilai post) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Post,
            // Meneruskan nilai literal `”/api/v1/sessions”` sebagai argumen ke `SendJsonAsync`.
            "/api/v1/sessions",
            // Meneruskan `createSessionPayload` (nilai create sesi payload) sebagai argumen ke `SendJsonAsync`.
            createSessionPayload,
            // Meneruskan `instructorToken` (nilai instruktur token) sebagai argumen ke `SendJsonAsync`.
            instructorToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.Created`,
        // `createSessionResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam CreateReadySessionAsync.
        Assert.Equal(HttpStatusCode.Created, createSessionResponse.StatusCode);

        // Menyiapkan variabel lokal `createdSession` untuk nilai created sesi dengan hasil operasi asinkron membaca tanpa argumen menjadi objek bertipe
        // sesuai kontrak JSON melalui `createSessionResponse.Content.ReadFromJsonAsync<CreateSessionResponse>`; await menunggu hasil tanpa memblokir thread
        // selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var createdSession = await createSessionResponse.Content.ReadFromJsonAsync<CreateSessionResponse>();
        // Menjalankan pemeriksaan NotNull atas `createdSession` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam CreateReadySessionAsync.
        Assert.NotNull(createdSession);

        // Menyiapkan variabel lokal `createPlayerPayload` untuk nilai create pemain payload dengan objek anonim yang mengelompokkan display_name, username,
        // password sebagai satu nilai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var createPlayerPayload = new
        // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // CreateReadySessionAsync.
        {
            // Menggunakan `display_name` (nilai display nama) sebagai bagian ekspresi yang sedang disusun dalam CreateReadySessionAsync.
            display_name = $"Player Invalid {suffix}",
            // Menggunakan `username` (nama akun yang dipakai saat autentikasi) sebagai bagian ekspresi yang sedang disusun dalam CreateReadySessionAsync.
            username = $"it_evt_invalid_player_{suffix}",
            // Menggunakan `password` (kata sandi masukan yang diperiksa sesuai kebijakan autentikasi) sebagai bagian ekspresi yang sedang disusun dalam
            // CreateReadySessionAsync.
            password = "IntegrationInvalidPlayerPass!123"
        // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam CreateReadySessionAsync.
        };

        // Menyiapkan variabel lokal `createPlayerResponse` untuk nilai create pemain respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Post`, `”/api/v1/players”`, `createPlayerPayload`, `instructorToken`; await menunggu hasil tanpa memblokir thread selama operasi
        // belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var createPlayerResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Post` (nilai post) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Post,
            // Meneruskan nilai literal `”/api/v1/players”` sebagai argumen ke `SendJsonAsync`.
            "/api/v1/players",
            // Meneruskan `createPlayerPayload` (nilai create pemain payload) sebagai argumen ke `SendJsonAsync`.
            createPlayerPayload,
            // Meneruskan `instructorToken` (nilai instruktur token) sebagai argumen ke `SendJsonAsync`.
            instructorToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.Created`,
        // `createPlayerResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam CreateReadySessionAsync.
        Assert.Equal(HttpStatusCode.Created, createPlayerResponse.StatusCode);

        // Menyiapkan variabel lokal `createdPlayer` untuk nilai created pemain dengan hasil operasi asinkron membaca tanpa argumen menjadi objek bertipe
        // sesuai kontrak JSON melalui `createPlayerResponse.Content.ReadFromJsonAsync<PlayerResponse>`; await menunggu hasil tanpa memblokir thread selama
        // operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var createdPlayer = await createPlayerResponse.Content.ReadFromJsonAsync<PlayerResponse>();
        // Menjalankan pemeriksaan NotNull atas `createdPlayer` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam CreateReadySessionAsync.
        Assert.NotNull(createdPlayer);

        // Menyiapkan variabel lokal `addPlayerPayload` untuk nilai add pemain payload dengan objek anonim yang mengelompokkan user_id, player_order_no
        // sebagai satu nilai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var addPlayerPayload = new
        // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // CreateReadySessionAsync.
        {
            // Menggunakan `user_id` (nilai pengguna identitas) sebagai bagian ekspresi yang sedang disusun dalam CreateReadySessionAsync.
            user_id = createdPlayer.UserId,
            // Menggunakan `player_order_no` (nilai pemain urutan/pesanan no) sebagai bagian ekspresi yang sedang disusun dalam CreateReadySessionAsync.
            player_order_no = 1
        // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam CreateReadySessionAsync.
        };

        // Menyiapkan variabel lokal `addPlayerResponse` untuk nilai add pemain respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Post`, `$”/api/v1/sessions/{createdSession.SessionId}/players”`, `addPlayerPayload`, `instructorToken`; await menunggu hasil tanpa
        // memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var addPlayerResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Post` (nilai post) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Post,
            // Meneruskan teks interpolasi `$”/api/v1/sessions/{createdSession.SessionId}/players”`; nilai ekspresi di dalam kurung kurawal disisipkan saat
            // program berjalan sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/sessions/{createdSession.SessionId}/players",
            // Meneruskan `addPlayerPayload` (nilai add pemain payload) sebagai argumen ke `SendJsonAsync`.
            addPlayerPayload,
            // Meneruskan `instructorToken` (nilai instruktur token) sebagai argumen ke `SendJsonAsync`.
            instructorToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`,
        // `addPlayerResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam CreateReadySessionAsync.
        Assert.Equal(HttpStatusCode.OK, addPlayerResponse.StatusCode);

        // Memulai loop dengan inisialisasi `var i = 2`, berjalan selama `i <= 3`, lalu memperbarui pencacah melalui `i++` dalam CreateReadySessionAsync.
        for (var i = 2; i <= 3; i++)
        // Membuka scope loop dengan syarat `i <= 3`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CreateReadySessionAsync.
        {
            // Menyiapkan variabel lokal `extraPlayerResponse` untuk nilai extra pemain respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
            // `HttpMethod.Post`, `”/api/v1/players”`, `new { display_name = $”Player Invalid Extra {i} {suffix}”, username =
            // $”it_evt_invalid_player_extra_{i}_{suffix}”, password = ”IntegrationInvalidExtraPlayerPass!123” }`, `instructorToken`; await menunggu hasil tanpa
            // memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var extraPlayerResponse = await SendJsonAsync(
                // Meneruskan `HttpMethod.Post` (nilai post) sebagai argumen ke `SendJsonAsync`.
                HttpMethod.Post,
                // Meneruskan nilai literal `”/api/v1/players”` sebagai argumen ke `SendJsonAsync`.
                "/api/v1/players",
                // Meneruskan objek anonim yang mengelompokkan display_name, username, password sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                new
                // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // CreateReadySessionAsync.
                {
                    // Meneruskan objek anonim yang mengelompokkan display_name, username, password sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                    display_name = $"Player Invalid Extra {i} {suffix}",
                    // Meneruskan objek anonim yang mengelompokkan display_name, username, password sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                    username = $"it_evt_invalid_player_extra_{i}_{suffix}",
                    // Meneruskan objek anonim yang mengelompokkan display_name, username, password sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                    password = "IntegrationInvalidExtraPlayerPass!123"
                // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam CreateReadySessionAsync.
                },
                // Meneruskan `instructorToken` (nilai instruktur token) sebagai argumen ke `SendJsonAsync`.
                instructorToken);
            // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.Created`,
            // `extraPlayerResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam CreateReadySessionAsync.
            Assert.Equal(HttpStatusCode.Created, extraPlayerResponse.StatusCode);

            // Menyiapkan variabel lokal `extraPlayer` untuk nilai extra pemain dengan hasil operasi asinkron membaca tanpa argumen menjadi objek bertipe sesuai
            // kontrak JSON melalui `extraPlayerResponse.Content.ReadFromJsonAsync<PlayerResponse>`; await menunggu hasil tanpa memblokir thread selama operasi
            // belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var extraPlayer = await extraPlayerResponse.Content.ReadFromJsonAsync<PlayerResponse>();
            // Menjalankan pemeriksaan NotNull atas `extraPlayer` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam CreateReadySessionAsync.
            Assert.NotNull(extraPlayer);

            // Menyiapkan variabel lokal `addExtraPlayerResponse` untuk nilai add extra pemain respons dengan hasil operasi asinkron memanggil `SendJsonAsync`
            // dengan `HttpMethod.Post`, `$”/api/v1/sessions/{createdSession.SessionId}/players”`, `new { user_id = extraPlayer.UserId, player_order_no = i }`,
            // `instructorToken`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var addExtraPlayerResponse = await SendJsonAsync(
                // Meneruskan `HttpMethod.Post` (nilai post) sebagai argumen ke `SendJsonAsync`.
                HttpMethod.Post,
                // Meneruskan teks interpolasi `$”/api/v1/sessions/{createdSession.SessionId}/players”`; nilai ekspresi di dalam kurung kurawal disisipkan saat
                // program berjalan sebagai argumen ke `SendJsonAsync`.
                $"/api/v1/sessions/{createdSession.SessionId}/players",
                // Meneruskan objek anonim yang mengelompokkan user_id, player_order_no sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                new
                // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // CreateReadySessionAsync.
                {
                    // Meneruskan objek anonim yang mengelompokkan user_id, player_order_no sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                    user_id = extraPlayer.UserId,
                    // Meneruskan objek anonim yang mengelompokkan user_id, player_order_no sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                    player_order_no = i
                // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam CreateReadySessionAsync.
                },
                // Meneruskan `instructorToken` (nilai instruktur token) sebagai argumen ke `SendJsonAsync`.
                instructorToken);
            // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`,
            // `addExtraPlayerResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam CreateReadySessionAsync.
            Assert.Equal(HttpStatusCode.OK, addExtraPlayerResponse.StatusCode);
        // Menutup scope loop dengan syarat `i <= 3`; bagian berikut berada di luar batas blok tersebut dalam CreateReadySessionAsync.
        }

        // Menyiapkan variabel lokal `saveSetupResponse` untuk nilai save setup respons dengan hasil operasi asinkron memanggil
        // `SessionSetupTestHelper.SaveAsync` dengan `_client`, `instructorToken`, `createdSession.SessionId`, `definition`,
        // `TestContext.Current.CancellationToken`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari
        // ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var saveSetupResponse = await SessionSetupTestHelper.SaveAsync(
            // Meneruskan `_client` (nilai client) sebagai argumen ke `SessionSetupTestHelper.SaveAsync`.
            _client,
            // Meneruskan `instructorToken` (nilai instruktur token) sebagai argumen ke `SessionSetupTestHelper.SaveAsync`.
            instructorToken,
            // Meneruskan `createdSession.SessionId` (identitas unik sesi permainan yang menjadi batas data operasi ini) sebagai argumen ke
            // `SessionSetupTestHelper.SaveAsync`.
            createdSession.SessionId,
            // Meneruskan `definition` (definisi terstruktur komponen serta parameter aturan permainan) sebagai argumen ke `SessionSetupTestHelper.SaveAsync`.
            definition,
            // Meneruskan `TestContext.Current.CancellationToken` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
            // aplikasi berhenti) sebagai argumen ke `SessionSetupTestHelper.SaveAsync`.
            TestContext.Current.CancellationToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.Created`,
        // `saveSetupResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam CreateReadySessionAsync.
        Assert.Equal(HttpStatusCode.Created, saveSetupResponse.StatusCode);

        // Menyiapkan variabel lokal `startSessionResponse` untuk nilai start sesi respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Post`, `$”/api/v1/sessions/{createdSession.SessionId}/start”`, `null`, `instructorToken`; await menunggu hasil tanpa memblokir thread
        // selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var startSessionResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Post` (nilai post) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Post,
            // Meneruskan teks interpolasi `$”/api/v1/sessions/{createdSession.SessionId}/start”`; nilai ekspresi di dalam kurung kurawal disisipkan saat
            // program berjalan sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/sessions/{createdSession.SessionId}/start",
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen bernama `body`.
            body: null,
            // Meneruskan `instructorToken` (nilai instruktur token) sebagai argumen ke `SendJsonAsync`.
            instructorToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`,
        // `startSessionResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam CreateReadySessionAsync.
        Assert.Equal(HttpStatusCode.OK, startSessionResponse.StatusCode);

        // Menyiapkan variabel lokal `rulesetDetailResponse` untuk nilai aturan detail respons dengan hasil operasi asinkron memanggil `SendJsonAsync`
        // dengan `HttpMethod.Get`, `$”/api/v1/rulesets/{createdRuleset.RulesetId}”`, `null`, `instructorToken`; await menunggu hasil tanpa memblokir thread
        // selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var rulesetDetailResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Get` (nilai get) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Get,
            // Meneruskan teks interpolasi `$”/api/v1/rulesets/{createdRuleset.RulesetId}”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program
            // berjalan sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/rulesets/{createdRuleset.RulesetId}",
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen bernama `body`.
            body: null,
            // Meneruskan `instructorToken` (nilai instruktur token) sebagai argumen ke `SendJsonAsync`.
            instructorToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`,
        // `rulesetDetailResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam CreateReadySessionAsync.
        Assert.Equal(HttpStatusCode.OK, rulesetDetailResponse.StatusCode);

        // Menyiapkan variabel lokal `rulesetDetail` untuk nilai aturan detail dengan hasil operasi asinkron membaca tanpa argumen menjadi objek bertipe
        // sesuai kontrak JSON melalui `rulesetDetailResponse.Content.ReadFromJsonAsync<RulesetDetailResponse>`; await menunggu hasil tanpa memblokir thread
        // selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var rulesetDetail = await rulesetDetailResponse.Content.ReadFromJsonAsync<RulesetDetailResponse>();
        // Menjalankan pemeriksaan NotNull atas `rulesetDetail` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam CreateReadySessionAsync.
        Assert.NotNull(rulesetDetail);
        // Menyiapkan variabel lokal `activeVersion` untuk nilai aktif versi dengan mengambil elemen pertama `rulesetDetail.Versions .Where(v =>
        // string.Equals(v.Status, ”ACTIVE”, StringComparison.OrdinalIgnoreCase)) .OrderByDescending(v => v.Version)` sesuai tanpa argumen; urutan tanpa
        // kecocokan menyebabkan exception. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var activeVersion = rulesetDetail.Versions
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(v => string.Equals(v.Status, ”ACTIVE”,
            // StringComparison.OrdinalIgnoreCase)) dalam CreateReadySessionAsync; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Where(v => string.Equals(v.Status, "ACTIVE", StringComparison.OrdinalIgnoreCase))
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .OrderByDescending(v => v.Version) dalam CreateReadySessionAsync; token pada
            // baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .OrderByDescending(v => v.Version)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .First(); dalam CreateReadySessionAsync; token pada baris ini menyambungkan
            // bagian kode sebelum dan sesudahnya.
            .First();

        // Menyiapkan variabel lokal `setupEventsResponse` untuk nilai setup event respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Get`, `$”/api/v1/sessions/{createdSession.SessionId}/events?limit=100”`, `null`, `instructorToken`; await menunggu hasil tanpa
        // memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis
        // saat scope berakhir.
        using var setupEventsResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Get` (nilai get) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Get,
            // Meneruskan teks interpolasi `$”/api/v1/sessions/{createdSession.SessionId}/events?limit=100”`; nilai ekspresi di dalam kurung kurawal disisipkan
            // saat program berjalan sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/sessions/{createdSession.SessionId}/events?limit=100",
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen ke `SendJsonAsync`.
            null,
            // Meneruskan `instructorToken` (nilai instruktur token) sebagai argumen ke `SendJsonAsync`.
            instructorToken);
        // Menyiapkan variabel lokal `setupEventsBody` untuk nilai setup event body dengan hasil operasi asinkron memanggil `ReadJsonAsync` dengan
        // `setupEventsResponse`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai
        // awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var setupEventsBody = await ReadJsonAsync(setupEventsResponse);
        // Menyiapkan variabel lokal `actingUserId` untuk nilai acting pengguna identitas dengan memanggil `setupEventsBody.RootElement.GetProperty(”items”)
        // .EnumerateArray() .Single(item => item.GetProperty(”action_type”).GetString() == ”BagikanTieBreaker” && item.GetProperty(”payl...` dengan tanpa
        // argumen. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var actingUserId = setupEventsBody.RootElement.GetProperty("items")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .EnumerateArray() dalam CreateReadySessionAsync; token pada baris ini
            // menyambungkan bagian kode sebelum dan sesudahnya.
            .EnumerateArray()
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Single(item => dalam CreateReadySessionAsync; token pada baris ini
            // menyambungkan bagian kode sebelum dan sesudahnya.
            .Single(item =>
                // Meneruskan nilai literal `”action_type”` sebagai argumen ke `item.GetProperty`.
                item.GetProperty("action_type").GetString() == "BagikanTieBreaker" &&
                // Meneruskan nilai literal `”payload”` sebagai argumen ke `item.GetProperty`; Meneruskan nilai literal `”number”` sebagai argumen ke
                // `item.GetProperty(”payload”).GetProperty`.
                item.GetProperty("payload").GetProperty("number").GetInt32() == 1)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .GetProperty(”user_id”) dalam CreateReadySessionAsync; token pada baris ini
            // menyambungkan bagian kode sebelum dan sesudahnya.
            .GetProperty("user_id")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .GetGuid(); dalam CreateReadySessionAsync; token pada baris ini menyambungkan
            // bagian kode sebelum dan sesudahnya.
            .GetGuid();
        // Menyiapkan variabel lokal `nextSequenceNumber` untuk nilai next sequence number dengan hasil operasi asinkron memanggil
        // `GetNextSequenceNumberAsync` dengan `createdSession.SessionId`, `instructorToken`; await menunggu hasil tanpa memblokir thread selama operasi
        // belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var nextSequenceNumber = await GetNextSequenceNumberAsync(createdSession.SessionId, instructorToken);
        // Mengembalikan tuple yang membawa bagian 1: createdSession.SessionId; bagian 2: createdPlayer.UserId; bagian 3: actingUserId; bagian 4:
        // activeVersion.RulesetVersionId; bagian 5: nextSequenceNumber kepada pemanggil dalam CreateReadySessionAsync; eksekusi jalur ini selesai setelah
        // nilai hasil ditentukan.
        return (createdSession.SessionId, createdPlayer.UserId, actingUserId, activeVersion.RulesetVersionId, nextSequenceNumber);
    // Menutup scope metode CreateReadySessionAsync; bagian berikut berada di luar batas blok tersebut dalam CreateReadySessionAsync.
    }

    // Mendefinisikan metode `GetNextSequenceNumberAsync` dengan hasil bertipe `Task<long>`; operasi ini menangani get next sequence number asinkron.
    // async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `sessionId` bertipe
    // `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini; Parameter `accessToken` bertipe `string` membawa nilai akses
    // token.
    private async Task<long> GetNextSequenceNumberAsync(Guid sessionId, string accessToken)
    // Membuka scope metode GetNextSequenceNumberAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam GetNextSequenceNumberAsync.
    {
        // Menyiapkan variabel lokal `response` untuk hasil respons yang akan dibaca atau dikirim kepada pemanggil dengan hasil operasi asinkron memanggil
        // `SendJsonAsync` dengan `HttpMethod.Get`, `$”/api/v1/sessions/{sessionId}/state”`, `null`, `accessToken`; await menunggu hasil tanpa memblokir
        // thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope
        // berakhir.
        using var response = await SendJsonAsync(
            // Meneruskan `HttpMethod.Get` (nilai get) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Get,
            // Meneruskan teks interpolasi `$”/api/v1/sessions/{sessionId}/state”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan
            // sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/sessions/{sessionId}/state",
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen ke `SendJsonAsync`.
            null,
            // Meneruskan `accessToken` (nilai akses token) sebagai argumen ke `SendJsonAsync`.
            accessToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`, `response.StatusCode`);
        // pengujian gagal jika keduanya berbeda dalam GetNextSequenceNumberAsync.
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        // Menyiapkan variabel lokal `body` untuk nilai body dengan hasil operasi asinkron memanggil `ReadJsonAsync` dengan `response`; await menunggu hasil
        // tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas
        // otomatis saat scope berakhir.
        using var body = await ReadJsonAsync(response);
        // Mengembalikan memanggil `body.RootElement.GetProperty(”next_sequence_number”).GetInt64` dengan tanpa argumen kepada pemanggil dalam
        // GetNextSequenceNumberAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return body.RootElement.GetProperty("next_sequence_number").GetInt64();
    // Menutup scope metode GetNextSequenceNumberAsync; bagian berikut berada di luar batas blok tersebut dalam GetNextSequenceNumberAsync.
    }

    // Mendefinisikan metode `AdvanceSessionToDayAsync` dengan hasil bertipe `Task<long>`; operasi ini menangani advance sesi ke hari asinkron. async
    // memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `setup` bertipe `(Guid
    // SessionId, Guid UserId, Guid ActingUserId, Guid RulesetVersionId, long NextSequenceNumber)` membawa nilai setup; Parameter `instructorToken`
    // bertipe `string` membawa nilai instruktur token; Parameter `targetDay` bertipe `int` membawa nilai target hari.
    private async Task<long> AdvanceSessionToDayAsync(
        // Parameter `setup` bertipe `(Guid SessionId, Guid UserId, Guid ActingUserId, Guid RulesetVersionId, long NextSequenceNumber)` membawa nilai setup.
        (Guid SessionId, Guid UserId, Guid ActingUserId, Guid RulesetVersionId, long NextSequenceNumber) setup,
        // Parameter `instructorToken` bertipe `string` membawa nilai instruktur token.
        string instructorToken,
        // Parameter `targetDay` bertipe `int` membawa nilai target hari.
        int targetDay)
    // Membuka scope metode AdvanceSessionToDayAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam AdvanceSessionToDayAsync.
    {
        // Menyiapkan variabel lokal `eventsResponse` untuk nilai event respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Get`, `$”/api/v1/sessions/{setup.SessionId}/events?limit=100”`, `null`, `instructorToken`; await menunggu hasil tanpa memblokir
        // thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope
        // berakhir.
        using var eventsResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Get` (nilai get) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Get,
            // Meneruskan teks interpolasi `$”/api/v1/sessions/{setup.SessionId}/events?limit=100”`; nilai ekspresi di dalam kurung kurawal disisipkan saat
            // program berjalan sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/sessions/{setup.SessionId}/events?limit=100",
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen ke `SendJsonAsync`.
            null,
            // Meneruskan `instructorToken` (nilai instruktur token) sebagai argumen ke `SendJsonAsync`.
            instructorToken);
        // Menyiapkan variabel lokal `eventsBody` untuk nilai event body dengan hasil operasi asinkron memanggil `ReadJsonAsync` dengan `eventsResponse`;
        // await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan
        // sumber daya dilepas otomatis saat scope berakhir.
        using var eventsBody = await ReadJsonAsync(eventsResponse);
        // Menyiapkan variabel lokal `players` untuk nilai pemain dengan mematerialisasi urutan `eventsBody.RootElement.GetProperty(”items”)
        // .EnumerateArray() .Where(item => item.GetProperty(”action_type”).GetString() == ”BagikanTieBreaker”) .Select(item => new { UserId =...` menjadi
        // List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var players = eventsBody.RootElement.GetProperty("items")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .EnumerateArray() dalam AdvanceSessionToDayAsync; token pada baris ini
            // menyambungkan bagian kode sebelum dan sesudahnya.
            .EnumerateArray()
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(item => item.GetProperty(”action_type”).GetString() ==
            // ”BagikanTieBreaker”) dalam AdvanceSessionToDayAsync; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Where(item => item.GetProperty("action_type").GetString() == "BagikanTieBreaker")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Select(item => new dalam AdvanceSessionToDayAsync; token pada baris ini
            // menyambungkan bagian kode sebelum dan sesudahnya.
            .Select(item => new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // AdvanceSessionToDayAsync.
            {
                // Meneruskan nilai literal `”user_id”` sebagai argumen ke `item.GetProperty`.
                UserId = item.GetProperty("user_id").GetGuid(),
                // Meneruskan nilai literal `”payload”` sebagai argumen ke `item.GetProperty`; Meneruskan nilai literal `”number”` sebagai argumen ke
                // `item.GetProperty(”payload”).GetProperty`.
                TurnNumber = item.GetProperty("payload").GetProperty("number").GetInt32()
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam AdvanceSessionToDayAsync.
            })
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .OrderBy(item => item.TurnNumber) dalam AdvanceSessionToDayAsync; token pada
            // baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .OrderBy(item => item.TurnNumber)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList(); dalam AdvanceSessionToDayAsync; token pada baris ini menyambungkan
            // bagian kode sebelum dan sesudahnya.
            .ToList();

        // Menyiapkan variabel lokal `sequence` untuk nomor urut event yang menentukan urutan pemrosesan riwayat permainan dengan `setup.NextSequenceNumber`
        // (nilai next sequence number). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var sequence = setup.NextSequenceNumber;
        // Menyiapkan variabel lokal `now` untuk nilai now dengan `DateTimeOffset.UtcNow`, yaitu waktu UTC saat operasi dilakukan. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var now = DateTimeOffset.UtcNow;
        // Memulai loop dengan inisialisasi `var day = 1`, berjalan selama `day < targetDay`, lalu memperbarui pencacah melalui `day++` dalam
        // AdvanceSessionToDayAsync.
        for (var day = 1; day < targetDay; day++)
        // Membuka scope loop dengan syarat `day < targetDay`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam AdvanceSessionToDayAsync.
        {
            // Menyiapkan variabel lokal `weekday` untuk nilai weekday dengan memanggil `ResolveWeekday` dengan `day`. Tipe variabel disimpulkan dari ekspresi
            // nilai awal.
            var weekday = ResolveWeekday(day);
            // Memeriksa hasil pencocokan `weekday` dengan pola `”MON” or ”TUE” or ”WED” or ”THU”`; blok if hanya dijalankan ketika kondisi ini bernilai benar
            // dalam AdvanceSessionToDayAsync.
            if (weekday is "MON" or "TUE" or "WED" or "THU")
            // Membuka scope cabang if untuk kondisi `weekday is ”MON” or ”TUE” or ”WED” or ”THU”`; pernyataan/deklarasi berikut berada di dalam batas blok ini
            // dalam AdvanceSessionToDayAsync.
            {
                // Mengulangi setiap elemen `players`; elemen saat ini disimpan sebagai `player` bertipe `var` untuk diproses oleh badan loop dalam
                // AdvanceSessionToDayAsync.
                foreach (var player in players)
                // Membuka scope loop setiap player dari `players`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam AdvanceSessionToDayAsync.
                {
                    // Memulai loop dengan inisialisasi `var slot = 1`, berjalan selama `slot <= 2`, lalu memperbarui pencacah melalui `slot++` dalam
                    // AdvanceSessionToDayAsync.
                    for (var slot = 1; slot <= 2; slot++)
                    // Membuka scope loop dengan syarat `slot <= 2`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam AdvanceSessionToDayAsync.
                    {
                        // Menyiapkan variabel lokal `actionResponse` untuk nilai aksi respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
                        // `HttpMethod.Post`, `”/api/v1/events”`, `new { event_id = Guid.NewGuid(), session_id = setup.SessionId, user_id = player.UserId, actor_type =
                        // ”PLAYER”, timestamp = now.AddSeconds(sequence), day_index = day, weekday, ...`, `instructorToken`; await menunggu hasil tanpa memblokir thread
                        // selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope
                        // berakhir.
                        using var actionResponse = await SendJsonAsync(HttpMethod.Post, "/api/v1/events", new
                        // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                        // AdvanceSessionToDayAsync.
                        {
                            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                            event_id = Guid.NewGuid(),
                            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                            session_id = setup.SessionId,
                            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                            user_id = player.UserId,
                            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                            actor_type = "PLAYER",
                            // Meneruskan `sequence` (nomor urut event yang menentukan urutan pemrosesan riwayat permainan) sebagai argumen ke `now.AddSeconds`.
                            timestamp = now.AddSeconds(sequence),
                            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                            day_index = day,
                            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                            weekday,
                            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                            turn_number = player.TurnNumber,
                            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                            action_slot = slot,
                            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                            sequence_number = sequence++,
                            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                            action_type = "KerjaLepas",
                            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                            ruleset_version_id = setup.RulesetVersionId,
                            // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                            // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                            payload = new { amount = 1 }
                        // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam AdvanceSessionToDayAsync.
                        }, instructorToken);
                        // Menjalankan pemeriksaan bahwa `actionResponse.StatusCode == HttpStatusCode.Created`, `await actionResponse.Content.ReadAsStringAsync()` bernilai
                        // benar; pengujian gagal jika kondisi tidak terpenuhi dalam AdvanceSessionToDayAsync.
                        Assert.True(actionResponse.StatusCode == HttpStatusCode.Created, await actionResponse.Content.ReadAsStringAsync());
                    // Menutup scope loop dengan syarat `slot <= 2`; bagian berikut berada di luar batas blok tersebut dalam AdvanceSessionToDayAsync.
                    }
                // Menutup scope loop setiap player dari `players`; bagian berikut berada di luar batas blok tersebut dalam AdvanceSessionToDayAsync.
                }
            // Menutup scope cabang if untuk kondisi `weekday is ”MON” or ”TUE” or ”WED” or ”THU”`; bagian berikut berada di luar batas blok tersebut dalam
            // AdvanceSessionToDayAsync.
            }
            // Menjalankan cabang alternatif ketika kondisi if sebelumnya tidak terpenuhi dalam AdvanceSessionToDayAsync.
            else if (weekday == "FRI")
            // Membuka scope cabang if untuk kondisi `weekday == ”FRI”`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // AdvanceSessionToDayAsync.
            {
                // Mengulangi setiap elemen `players`; elemen saat ini disimpan sebagai `player` bertipe `var` untuk diproses oleh badan loop dalam
                // AdvanceSessionToDayAsync.
                foreach (var player in players)
                // Membuka scope loop setiap player dari `players`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam AdvanceSessionToDayAsync.
                {
                    // Menyiapkan variabel lokal `donationResponse` untuk nilai donasi respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
                    // `HttpMethod.Post`, `”/api/v1/events”`, `new { event_id = Guid.NewGuid(), session_id = setup.SessionId, user_id = player.UserId, actor_type =
                    // ”PLAYER”, timestamp = now.AddSeconds(sequence), day_index = day, weekday, ...`, `instructorToken`; await menunggu hasil tanpa memblokir thread
                    // selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope
                    // berakhir.
                    using var donationResponse = await SendJsonAsync(HttpMethod.Post, "/api/v1/events", new
                    // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                    // AdvanceSessionToDayAsync.
                    {
                        // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                        // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                        event_id = Guid.NewGuid(),
                        // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                        // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                        session_id = setup.SessionId,
                        // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                        // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                        user_id = player.UserId,
                        // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                        // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                        actor_type = "PLAYER",
                        // Meneruskan `sequence` (nomor urut event yang menentukan urutan pemrosesan riwayat permainan) sebagai argumen ke `now.AddSeconds`.
                        timestamp = now.AddSeconds(sequence),
                        // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                        // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                        day_index = day,
                        // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                        // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                        weekday,
                        // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                        // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                        turn_number = player.TurnNumber,
                        // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                        // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                        action_slot = 0,
                        // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                        // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                        sequence_number = sequence++,
                        // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                        // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                        action_type = "JumatBerkah",
                        // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                        // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                        ruleset_version_id = setup.RulesetVersionId,
                        // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                        // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                        payload = new { amount = 1 }
                    // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam AdvanceSessionToDayAsync.
                    }, instructorToken);
                    // Menjalankan pemeriksaan bahwa `donationResponse.StatusCode == HttpStatusCode.Created`, `await donationResponse.Content.ReadAsStringAsync()`
                    // bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam AdvanceSessionToDayAsync.
                    Assert.True(donationResponse.StatusCode == HttpStatusCode.Created, await donationResponse.Content.ReadAsStringAsync());
                // Menutup scope loop setiap player dari `players`; bagian berikut berada di luar batas blok tersebut dalam AdvanceSessionToDayAsync.
                }
            // Menutup scope cabang if untuk kondisi `weekday == ”FRI”`; bagian berikut berada di luar batas blok tersebut dalam AdvanceSessionToDayAsync.
            }

            // Menyiapkan variabel lokal `endTurnResponse` untuk nilai end giliran respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
            // `HttpMethod.Post`, `”/api/v1/events”`, `new { event_id = Guid.NewGuid(), session_id = setup.SessionId, user_id = (Guid?)null, actor_type =
            // ”SYSTEM”, timestamp = now.AddSeconds(sequence), day_index = day, weekday, tu...`, `instructorToken`; await menunggu hasil tanpa memblokir thread
            // selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope
            // berakhir.
            using var endTurnResponse = await SendJsonAsync(HttpMethod.Post, "/api/v1/events", new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // AdvanceSessionToDayAsync.
            {
                // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                event_id = Guid.NewGuid(),
                // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                session_id = setup.SessionId,
                // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                user_id = (Guid?)null,
                // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                actor_type = "SYSTEM",
                // Meneruskan `sequence` (nomor urut event yang menentukan urutan pemrosesan riwayat permainan) sebagai argumen ke `now.AddSeconds`.
                timestamp = now.AddSeconds(sequence),
                // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                day_index = day,
                // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                weekday,
                // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                turn_number = 0,
                // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                action_slot = 0,
                // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                sequence_number = sequence++,
                // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                action_type = "AkhirGiliran",
                // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                ruleset_version_id = setup.RulesetVersionId,
                // Meneruskan objek anonim yang mengelompokkan event_id, session_id, user_id, actor_type, timestamp, day_index, weekday, turn_number, action_slot,
                // sequence_number, action_type, ruleset_version_id, payload sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                payload = new { }
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam AdvanceSessionToDayAsync.
            }, instructorToken);
            // Menjalankan pemeriksaan bahwa `endTurnResponse.StatusCode == HttpStatusCode.Created`, `await endTurnResponse.Content.ReadAsStringAsync()`
            // bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam AdvanceSessionToDayAsync.
            Assert.True(endTurnResponse.StatusCode == HttpStatusCode.Created, await endTurnResponse.Content.ReadAsStringAsync());
        // Menutup scope loop dengan syarat `day < targetDay`; bagian berikut berada di luar batas blok tersebut dalam AdvanceSessionToDayAsync.
        }

        // Mengembalikan `sequence` (nomor urut event yang menentukan urutan pemrosesan riwayat permainan) kepada pemanggil dalam AdvanceSessionToDayAsync;
        // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return sequence;
    // Menutup scope metode AdvanceSessionToDayAsync; bagian berikut berada di luar batas blok tersebut dalam AdvanceSessionToDayAsync.
    }

    // Mendefinisikan metode `ResolveWeekday` dengan hasil bertipe `string`; operasi ini menangani resolve weekday. Masukan: Parameter `day` bertipe
    // `int` membawa nomor hari permainan yang menjadi konteks aktivitas. Nilai hasil langsung berasal dari hasil pemetaan `(((day - 1) % 7 + 7) % 7)`
    // melalui cabang pola switch yang cocok.
    private static string ResolveWeekday(int day)
        // Melengkapi struktur ekspresi ArrowExpressionClause melalui => (((day - 1) % 7 + 7) % 7) switch dalam ResolveWeekday; token pada baris ini
        // menyambungkan bagian kode sebelum dan sesudahnya.
        => (((day - 1) % 7 + 7) % 7) switch
        // Membuka scope pemetaan switch atas `(((day - 1) % 7 + 7) % 7)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveWeekday.
        {
            // Untuk pola `0`, menghasilkan nilai literal `”MON”` sebagai hasil switch.
            0 => "MON",
            // Untuk pola `1`, menghasilkan nilai literal `”TUE”` sebagai hasil switch.
            1 => "TUE",
            // Untuk pola `2`, menghasilkan nilai literal `”WED”` sebagai hasil switch.
            2 => "WED",
            // Untuk pola `3`, menghasilkan nilai literal `”THU”` sebagai hasil switch.
            3 => "THU",
            // Untuk pola `4`, menghasilkan nilai literal `”FRI”` sebagai hasil switch.
            4 => "FRI",
            // Untuk pola `5`, menghasilkan nilai literal `”SAT”` sebagai hasil switch.
            5 => "SAT",
            // Untuk pola `_`, menghasilkan nilai literal `”SUN”` sebagai hasil switch.
            _ => "SUN"
        // Menutup scope pemetaan switch atas `(((day - 1) % 7 + 7) % 7)`; bagian berikut berada di luar batas blok tersebut dalam ResolveWeekday.
        };

    /// <summary>
    /// Helper untuk mendaftarkan pengguna baru dan mengembalikan data registrasi.
    /// </summary>
    // Mendefinisikan metode `RegisterAsync` dengan hasil bertipe `Task<RegisterResponse>`. Helper untuk mendaftarkan pengguna baru dan mengembalikan
    // data registrasi. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter
    // `username` bertipe `string` membawa nama akun yang dipakai saat autentikasi; Parameter `password` bertipe `string` membawa kata sandi masukan
    // yang diperiksa sesuai kebijakan autentikasi; Parameter `role` bertipe `string` membawa peran pengguna yang menentukan hak akses.
    private async Task<RegisterResponse> RegisterAsync(string username, string password, string role)
    // Membuka scope metode RegisterAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam RegisterAsync.
    {
        // Menyiapkan variabel lokal `payload` untuk muatan detail event dalam format JSON dengan objek baru bertipe `RegisterRequest` dengan argumen
        // (username, password, role, null). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var payload = new RegisterRequest(username, password, role, null);
        // Menyiapkan variabel lokal `response` untuk hasil respons yang akan dibaca atau dikirim kepada pemanggil dengan hasil operasi asinkron memanggil
        // `_client.PostAsJsonAsync` dengan `”/api/v1/auth/register”`, `payload`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai.
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var response = await _client.PostAsJsonAsync("/api/v1/auth/register", payload);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.Created`,
        // `response.StatusCode`); pengujian gagal jika keduanya berbeda dalam RegisterAsync.
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        // Menyiapkan variabel lokal `body` untuk nilai body dengan hasil operasi asinkron membaca tanpa argumen menjadi objek bertipe sesuai kontrak JSON
        // melalui `response.Content.ReadFromJsonAsync<RegisterResponse>`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var body = await response.Content.ReadFromJsonAsync<RegisterResponse>();
        // Menjalankan pemeriksaan NotNull atas `body` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam RegisterAsync.
        Assert.NotNull(body);
        // Mengembalikan `body` (nilai body) kepada pemanggil dalam RegisterAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return body;
    // Menutup scope metode RegisterAsync; bagian berikut berada di luar batas blok tersebut dalam RegisterAsync.
    }

    /// <summary>
    /// Helper untuk melakukan login dan mengembalikan data token akses.
    /// </summary>
    // Mendefinisikan metode `LoginAsync` dengan hasil bertipe `Task<LoginResponse>`. Helper untuk melakukan login dan mengembalikan data token akses.
    // async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `username` bertipe
    // `string` membawa nama akun yang dipakai saat autentikasi; Parameter `password` bertipe `string` membawa kata sandi masukan yang diperiksa sesuai
    // kebijakan autentikasi.
    private async Task<LoginResponse> LoginAsync(string username, string password)
    // Membuka scope metode LoginAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam LoginAsync.
    {
        // Menyiapkan variabel lokal `payload` untuk muatan detail event dalam format JSON dengan objek baru bertipe `LoginRequest` dengan argumen
        // (username, password). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var payload = new LoginRequest(username, password);
        // Menyiapkan variabel lokal `response` untuk hasil respons yang akan dibaca atau dikirim kepada pemanggil dengan hasil operasi asinkron memanggil
        // `_client.PostAsJsonAsync` dengan `”/api/v1/auth/login”`, `payload`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai.
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", payload);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`, `response.StatusCode`);
        // pengujian gagal jika keduanya berbeda dalam LoginAsync.
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Menyiapkan variabel lokal `body` untuk nilai body dengan hasil operasi asinkron membaca tanpa argumen menjadi objek bertipe sesuai kontrak JSON
        // melalui `response.Content.ReadFromJsonAsync<LoginResponse>`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var body = await response.Content.ReadFromJsonAsync<LoginResponse>();
        // Menjalankan pemeriksaan NotNull atas `body` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam LoginAsync.
        Assert.NotNull(body);
        // Mengembalikan `body` (nilai body) kepada pemanggil dalam LoginAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return body;
    // Menutup scope metode LoginAsync; bagian berikut berada di luar batas blok tersebut dalam LoginAsync.
    }

    /// <summary>
    /// Helper untuk mengirim HTTP request dengan body JSON dan header Bearer token.
    /// </summary>
    // Mendefinisikan metode `SendJsonAsync` dengan hasil bertipe `Task<HttpResponseMessage>`. Helper untuk mengirim HTTP request dengan body JSON dan
    // header Bearer token. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter
    // `method` bertipe `HttpMethod` membawa nilai method; Parameter `path` bertipe `string` membawa nilai path; Parameter `body` bertipe `object?`
    // membawa nilai body; nilai null diizinkan ketika data opsional belum tersedia; Parameter `accessToken` bertipe `string` membawa nilai akses token.
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

    /// <summary>
    /// Helper yang membangun definition ruleset lengkap untuk mode PEMULA
    /// dengan parameter starting cash dan opsional pengaturan urutan pemain.
    /// </summary>
    // Mendefinisikan metode `BuildRulesetDefinition` dengan hasil bertipe `RulesetDefinitionDto`. Helper yang membangun definition ruleset lengkap
    // untuk mode PEMULA dengan parameter starting cash dan opsional pengaturan urutan pemain. Masukan: Parameter `startingCash` bertipe `int` membawa
    // nilai starting uang tunai; Parameter `playerOrdering` bertipe `string` membawa nilai pemain ordering; bila argumen tidak diberikan digunakan
    // nilai literal `”PLAYER_ORDER”`; Parameter `mode` bertipe `string` membawa mode permainan yang menentukan kelompok aturan yang digunakan; bila
    // argumen tidak diberikan digunakan nilai literal `”PEMULA”`; Parameter `riskAmount` bertipe `int` membawa nilai risiko nominal; bila argumen tidak
    // diberikan digunakan nilai literal `4`.
    private static RulesetDefinitionDto BuildRulesetDefinition(
        // Parameter `startingCash` bertipe `int` membawa nilai starting uang tunai.
        int startingCash,
        // Parameter `playerOrdering` bertipe `string` membawa nilai pemain ordering; bila argumen tidak diberikan digunakan nilai literal `”PLAYER_ORDER”`.
        string playerOrdering = "PLAYER_ORDER",
        // Parameter `mode` bertipe `string` membawa mode permainan yang menentukan kelompok aturan yang digunakan; bila argumen tidak diberikan digunakan
        // nilai literal `”PEMULA”`.
        string mode = "PEMULA",
        // Parameter `riskAmount` bertipe `int` membawa nilai risiko nominal; bila argumen tidak diberikan digunakan nilai literal `4`.
        int riskAmount = 4)
    // Membuka scope metode BuildRulesetDefinition; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildRulesetDefinition.
    {
        // Mengembalikan objek baru bertipe `RulesetDefinitionDto` dengan nilai awal sesuai konstruktornya kepada pemanggil dalam BuildRulesetDefinition;
        // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return new RulesetDefinitionDto
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // BuildRulesetDefinition.
        {
            // Memperbarui `Mode` menggunakan `mode` (mode permainan yang menentukan kelompok aturan yang digunakan) dalam BuildRulesetDefinition.
            Mode = mode,
            // Memperbarui `Settings` menggunakan objek baru bertipe `RulesetSettingsDto` dengan nilai awal sesuai konstruktornya dalam BuildRulesetDefinition.
            Settings = new RulesetSettingsDto
            // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // BuildRulesetDefinition.
            {
                // Memperbarui `ActionsPerTurn` menggunakan nilai literal `2` dalam BuildRulesetDefinition.
                ActionsPerTurn = 2,
                // Memperbarui `StartingCash` menggunakan `startingCash` (nilai starting uang tunai) dalam BuildRulesetDefinition.
                StartingCash = startingCash,
                // Memperbarui `InitialCoins` menggunakan `startingCash` (nilai starting uang tunai) dalam BuildRulesetDefinition.
                InitialCoins = startingCash,
                // Memperbarui `InitialHappiness` menggunakan nilai literal `0` dalam BuildRulesetDefinition.
                InitialHappiness = 0,
                // Memperbarui `InitialSaving` menggunakan nilai literal `0` dalam BuildRulesetDefinition.
                InitialSaving = 0,
                // Memperbarui `FinishDay` menggunakan nilai literal `25` dalam BuildRulesetDefinition.
                FinishDay = 25,
                // Memperbarui `MinPlayers` menggunakan nilai literal `2` dalam BuildRulesetDefinition.
                MinPlayers = 2,
                // Memperbarui `MaxPlayers` menggunakan nilai literal `4` dalam BuildRulesetDefinition.
                MaxPlayers = 4,
                // Memperbarui `CashMin` menggunakan nilai literal `0` dalam BuildRulesetDefinition.
                CashMin = 0,
                // Memperbarui `MaxIngredientTotal` menggunakan nilai literal `6` dalam BuildRulesetDefinition.
                MaxIngredientTotal = 6,
                // Memperbarui `MaxSameIngredient` menggunakan nilai literal `3` dalam BuildRulesetDefinition.
                MaxSameIngredient = 3,
                // Memperbarui `PrimaryNeedMaxPerDay` menggunakan nilai literal `1` dalam BuildRulesetDefinition.
                PrimaryNeedMaxPerDay = 1,
                // Memperbarui `RequirePrimaryBeforeOthers` menggunakan true, yaitu kondisi aktif/terpenuhi dalam BuildRulesetDefinition.
                RequirePrimaryBeforeOthers = true,
                // Memperbarui `DonationMinAmount` menggunakan nilai literal `1` dalam BuildRulesetDefinition.
                DonationMinAmount = 1,
                // Memperbarui `DonationMaxAmount` menggunakan nilai literal `999999` dalam BuildRulesetDefinition.
                DonationMaxAmount = 999999,
                // Memperbarui `GoldTradeAllowBuy` menggunakan true, yaitu kondisi aktif/terpenuhi dalam BuildRulesetDefinition.
                GoldTradeAllowBuy = true,
                // Memperbarui `GoldTradeAllowSell` menggunakan true, yaitu kondisi aktif/terpenuhi dalam BuildRulesetDefinition.
                GoldTradeAllowSell = true,
                // Memperbarui `LoanEnabled` menggunakan perbandingan kesamaan antara `mode` dan `”MAHIR”` dalam BuildRulesetDefinition.
                LoanEnabled = mode == "MAHIR",
                // Memperbarui `InsuranceEnabled` menggunakan perbandingan kesamaan antara `mode` dan `”MAHIR”` dalam BuildRulesetDefinition.
                InsuranceEnabled = mode == "MAHIR",
                // Memperbarui `SavingGoalEnabled` menggunakan perbandingan kesamaan antara `mode` dan `”MAHIR”` dalam BuildRulesetDefinition.
                SavingGoalEnabled = mode == "MAHIR",
                // Memperbarui `FreelanceIncome` menggunakan nilai literal `1` dalam BuildRulesetDefinition.
                FreelanceIncome = 1
            // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam BuildRulesetDefinition.
            },
            // Memperbarui `PlayerOrdering` menggunakan objek baru bertipe `RulesetPlayerOrderingDto` dengan nilai awal sesuai konstruktornya dalam
            // BuildRulesetDefinition.
            PlayerOrdering = new RulesetPlayerOrderingDto
            // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // BuildRulesetDefinition.
            {
                // Memperbarui `OrderingCode` menggunakan `playerOrdering` (nilai pemain ordering) dalam BuildRulesetDefinition.
                OrderingCode = playerOrdering,
                // Memperbarui `FridayFeature` menggunakan nilai literal `”DONATION”` dalam BuildRulesetDefinition.
                FridayFeature = "DONATION",
                // Memperbarui `FridayEnabled` menggunakan true, yaitu kondisi aktif/terpenuhi dalam BuildRulesetDefinition.
                FridayEnabled = true,
                // Memperbarui `SaturdayFeature` menggunakan nilai literal `”GOLD_TRADE”` dalam BuildRulesetDefinition.
                SaturdayFeature = "GOLD_TRADE",
                // Memperbarui `SaturdayEnabled` menggunakan true, yaitu kondisi aktif/terpenuhi dalam BuildRulesetDefinition.
                SaturdayEnabled = true,
                // Memperbarui `SundayFeature` menggunakan nilai literal `”REST”` dalam BuildRulesetDefinition.
                SundayFeature = "REST",
                // Memperbarui `SundayEnabled` menggunakan true, yaitu kondisi aktif/terpenuhi dalam BuildRulesetDefinition.
                SundayEnabled = true
            // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam BuildRulesetDefinition.
            },
            // Memperbarui `Actions` menggunakan koleksi berisi new RulesetActionDto { ActionId = ”BahanMasakan” }, new RulesetActionDto { ActionId =
            // ”JualMasakan” }, new RulesetActionDto { ActionId = ”Kebutuhan” }, new RulesetActionDto { ActionId = ”KerjaLepas” }, new RulesetActionDto {
            // ActionId = ”RisikoKehidupan..., new RulesetActionDto { ActionId = ”GunakanOpsiDaru..., new RulesetActionDto { ActionId = ”PinjamanSyariah..., new
            // RulesetActionDto { ActionId = ”BayarPinjaman” ..., new RulesetActionDto { ActionId = ”Asuransi” }, new RulesetActionDto { ActionId =
            // ”BayarRisiko” }, new RulesetActionDto { ActionId = ”BukaHargaEmas” ..., new RulesetActionDto { ActionId = ”InvestasiEmas” ..., new
            // RulesetActionDto { ActionId = ”JualEmas” } dalam BuildRulesetDefinition.
            Actions =
            // Menggunakan koleksi berisi new RulesetActionDto { ActionId = ”BahanMasakan” }, new RulesetActionDto { ActionId = ”JualMasakan” }, new
            // RulesetActionDto { ActionId = ”Kebutuhan” }, new RulesetActionDto { ActionId = ”KerjaLepas” }, new RulesetActionDto { ActionId =
            // ”RisikoKehidupan..., new RulesetActionDto { ActionId = ”GunakanOpsiDaru..., new RulesetActionDto { ActionId = ”PinjamanSyariah..., new
            // RulesetActionDto { ActionId = ”BayarPinjaman” ..., new RulesetActionDto { ActionId = ”Asuransi” }, new RulesetActionDto { ActionId =
            // ”BayarRisiko” }, new RulesetActionDto { ActionId = ”BukaHargaEmas” ..., new RulesetActionDto { ActionId = ”InvestasiEmas” ..., new
            // RulesetActionDto { ActionId = ”JualEmas” } sebagai bagian ekspresi yang sedang disusun dalam BuildRulesetDefinition.
            [
                // Menggunakan objek baru bertipe `RulesetActionDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun dalam
                // BuildRulesetDefinition.
                new RulesetActionDto { ActionId = "BahanMasakan" },
                // Menggunakan objek baru bertipe `RulesetActionDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun dalam
                // BuildRulesetDefinition.
                new RulesetActionDto { ActionId = "JualMasakan" },
                // Menggunakan objek baru bertipe `RulesetActionDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun dalam
                // BuildRulesetDefinition.
                new RulesetActionDto { ActionId = "Kebutuhan" },
                // Menggunakan objek baru bertipe `RulesetActionDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun dalam
                // BuildRulesetDefinition.
                new RulesetActionDto { ActionId = "KerjaLepas" },
                // Menggunakan objek baru bertipe `RulesetActionDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun dalam
                // BuildRulesetDefinition.
                new RulesetActionDto { ActionId = "RisikoKehidupan" },
                // Menggunakan objek baru bertipe `RulesetActionDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun dalam
                // BuildRulesetDefinition.
                new RulesetActionDto { ActionId = "GunakanOpsiDarurat" },
                // Menggunakan objek baru bertipe `RulesetActionDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun dalam
                // BuildRulesetDefinition.
                new RulesetActionDto { ActionId = "PinjamanSyariah" },
                // Menggunakan objek baru bertipe `RulesetActionDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun dalam
                // BuildRulesetDefinition.
                new RulesetActionDto { ActionId = "BayarPinjaman" },
                // Menggunakan objek baru bertipe `RulesetActionDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun dalam
                // BuildRulesetDefinition.
                new RulesetActionDto { ActionId = "Asuransi" },
                // Menggunakan objek baru bertipe `RulesetActionDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun dalam
                // BuildRulesetDefinition.
                new RulesetActionDto { ActionId = "BayarRisiko" },
                // Menggunakan objek baru bertipe `RulesetActionDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun dalam
                // BuildRulesetDefinition.
                new RulesetActionDto { ActionId = "BukaHargaEmas" },
                // Menggunakan objek baru bertipe `RulesetActionDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun dalam
                // BuildRulesetDefinition.
                new RulesetActionDto { ActionId = "InvestasiEmas" },
                // Menggunakan objek baru bertipe `RulesetActionDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun dalam
                // BuildRulesetDefinition.
                new RulesetActionDto { ActionId = "JualEmas" }
            // Menandai akhir daftar elemen atau indeks koleksi dalam BuildRulesetDefinition; pasangan kurung siku mengelompokkan nilai sebagai satu struktur.
            ],
            // Memperbarui `GoldPrices` menggunakan koleksi berisi new RulesetGoldPriceDto { PriceCode = ”gold_price_..., new RulesetGoldPriceDto { PriceCode =
            // ”gold_price_... dalam BuildRulesetDefinition.
            GoldPrices =
            // Menggunakan koleksi berisi new RulesetGoldPriceDto { PriceCode = ”gold_price_..., new RulesetGoldPriceDto { PriceCode = ”gold_price_... sebagai
            // bagian ekspresi yang sedang disusun dalam BuildRulesetDefinition.
            [
                // Menggunakan objek baru bertipe `RulesetGoldPriceDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun dalam
                // BuildRulesetDefinition.
                new RulesetGoldPriceDto { PriceCode = "gold_price_5", Qty = 1, UnitPrice = 5, CardQty = 3 },
                // Menggunakan objek baru bertipe `RulesetGoldPriceDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun dalam
                // BuildRulesetDefinition.
                new RulesetGoldPriceDto { PriceCode = "gold_price_6", Qty = 1, UnitPrice = 6, CardQty = 3 }
            // Menandai akhir daftar elemen atau indeks koleksi dalam BuildRulesetDefinition; pasangan kurung siku mengelompokkan nilai sebagai satu struktur.
            ],
            // Memperbarui `Ingredients` menggunakan koleksi berisi new RulesetIngredientDto { Id = ”nasi_putih”, Nama..., new RulesetIngredientDto { Id =
            // ”telur”, Nama = ”T..., new RulesetIngredientDto { Id = ”sayur”, Nama = ”S... dalam BuildRulesetDefinition.
            Ingredients =
            // Menggunakan koleksi berisi new RulesetIngredientDto { Id = ”nasi_putih”, Nama..., new RulesetIngredientDto { Id = ”telur”, Nama = ”T..., new
            // RulesetIngredientDto { Id = ”sayur”, Nama = ”S... sebagai bagian ekspresi yang sedang disusun dalam BuildRulesetDefinition.
            [
                // Menggunakan objek baru bertipe `RulesetIngredientDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun dalam
                // BuildRulesetDefinition.
                new RulesetIngredientDto { Id = "nasi_putih", Nama = "Nasi Putih", HargaBeli = 1 },
                // Menggunakan objek baru bertipe `RulesetIngredientDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun dalam
                // BuildRulesetDefinition.
                new RulesetIngredientDto { Id = "telur", Nama = "Telur", HargaBeli = 4 },
                // Menggunakan objek baru bertipe `RulesetIngredientDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun dalam
                // BuildRulesetDefinition.
                new RulesetIngredientDto { Id = "sayur", Nama = "Sayur", HargaBeli = 2 }
            // Menandai akhir daftar elemen atau indeks koleksi dalam BuildRulesetDefinition; pasangan kurung siku mengelompokkan nilai sebagai satu struktur.
            ],
            // Memperbarui `Orders` menggunakan koleksi berisi new RulesetOrderDto { Id = ”nasi_goreng”, Nama = ”... dalam BuildRulesetDefinition.
            Orders =
            // Menggunakan koleksi berisi new RulesetOrderDto { Id = ”nasi_goreng”, Nama = ”... sebagai bagian ekspresi yang sedang disusun dalam
            // BuildRulesetDefinition.
            [
                // Menggunakan objek baru bertipe `RulesetOrderDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun dalam
                // BuildRulesetDefinition.
                new RulesetOrderDto
                // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // BuildRulesetDefinition.
                {
                    // Memperbarui `Id` menggunakan nilai literal `”nasi_goreng”` dalam BuildRulesetDefinition.
                    Id = "nasi_goreng",
                    // Memperbarui `Nama` menggunakan nilai literal `”nasi goreng”` dalam BuildRulesetDefinition.
                    Nama = "nasi goreng",
                    // Memperbarui `HargaJual` menggunakan hasil pemilihan bersyarat: ketika `mode == ”MAHIR”` benar gunakan `1`, jika tidak gunakan `15` dalam
                    // BuildRulesetDefinition.
                    HargaJual = mode == "MAHIR" ? 1 : 15,
                    // Memperbarui `PoinKebahagiaan` menggunakan nilai literal `0` dalam BuildRulesetDefinition.
                    PoinKebahagiaan = 0,
                    // Memperbarui `Bahan` menggunakan hasil pemilihan bersyarat: ketika `mode == ”MAHIR”` benar gunakan `[]`, jika tidak gunakan `[”Nasi Putih”,
                    // ”Telur”]` dalam BuildRulesetDefinition.
                    Bahan = mode == "MAHIR" ? [] : ["Nasi Putih", "Telur"],
                    // Memperbarui `CardQty` menggunakan nilai literal `6` dalam BuildRulesetDefinition.
                    CardQty = 6
                // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam BuildRulesetDefinition.
                }
            // Menandai akhir daftar elemen atau indeks koleksi dalam BuildRulesetDefinition; pasangan kurung siku mengelompokkan nilai sebagai satu struktur.
            ],
            // Memperbarui `Needs` menggunakan koleksi berisi new RulesetNeedDto { Id = ”buku”, Nama = ”buku”, T..., new RulesetNeedDto { Id = ”baju”, Nama =
            // ”baju”, T..., new RulesetNeedDto { Id = ”sepatu”, Nama = ”sepatu..., new RulesetNeedDto { Id = ”tempat_makan”, Nama = ”..., new RulesetNeedDto {
            // Id = ”alat_tulis”, Nama = ”al..., new RulesetNeedDto { Id = ”boneka”, Nama = ”boneka..., new RulesetNeedDto { Id = ”gameboy”, Nama = ”gameb...,
            // new RulesetNeedDto { Id = ”hiburan”, Nama = ”hibur..., new RulesetNeedDto { Id = ”jam”, Nama = ”jam”, Tip... dalam BuildRulesetDefinition.
            Needs =
            // Menggunakan koleksi berisi new RulesetNeedDto { Id = ”buku”, Nama = ”buku”, T..., new RulesetNeedDto { Id = ”baju”, Nama = ”baju”, T..., new
            // RulesetNeedDto { Id = ”sepatu”, Nama = ”sepatu..., new RulesetNeedDto { Id = ”tempat_makan”, Nama = ”..., new RulesetNeedDto { Id = ”alat_tulis”,
            // Nama = ”al..., new RulesetNeedDto { Id = ”boneka”, Nama = ”boneka..., new RulesetNeedDto { Id = ”gameboy”, Nama = ”gameb..., new RulesetNeedDto {
            // Id = ”hiburan”, Nama = ”hibur..., new RulesetNeedDto { Id = ”jam”, Nama = ”jam”, Tip... sebagai bagian ekspresi yang sedang disusun dalam
            // BuildRulesetDefinition.
            [
                // Menggunakan objek baru bertipe `RulesetNeedDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun dalam
                // BuildRulesetDefinition.
                new RulesetNeedDto { Id = "buku", Nama = "buku", Tipe = "primer", HargaBeli = 3, PoinKebahagiaan = 1 },
                // Menggunakan objek baru bertipe `RulesetNeedDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun dalam
                // BuildRulesetDefinition.
                new RulesetNeedDto { Id = "baju", Nama = "baju", Tipe = "primer", HargaBeli = 3, PoinKebahagiaan = 1 },
                // Menggunakan objek baru bertipe `RulesetNeedDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun dalam
                // BuildRulesetDefinition.
                new RulesetNeedDto { Id = "sepatu", Nama = "sepatu", Tipe = "primer", HargaBeli = 3, PoinKebahagiaan = 1 },
                // Menggunakan objek baru bertipe `RulesetNeedDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun dalam
                // BuildRulesetDefinition.
                new RulesetNeedDto { Id = "tempat_makan", Nama = "tempat makan", Tipe = "primer", HargaBeli = 3, PoinKebahagiaan = 1 },
                // Menggunakan objek baru bertipe `RulesetNeedDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun dalam
                // BuildRulesetDefinition.
                new RulesetNeedDto { Id = "alat_tulis", Nama = "alat tulis", Tipe = "primer", HargaBeli = 3, PoinKebahagiaan = 1 },
                // Menggunakan objek baru bertipe `RulesetNeedDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun dalam
                // BuildRulesetDefinition.
                new RulesetNeedDto { Id = "boneka", Nama = "boneka", Tipe = "tersier", HargaBeli = 6, PoinKebahagiaan = 3 },
                // Menggunakan objek baru bertipe `RulesetNeedDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun dalam
                // BuildRulesetDefinition.
                new RulesetNeedDto { Id = "gameboy", Nama = "gameboy", Tipe = "tersier", HargaBeli = 6, PoinKebahagiaan = 3 },
                // Menggunakan objek baru bertipe `RulesetNeedDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun dalam
                // BuildRulesetDefinition.
                new RulesetNeedDto { Id = "hiburan", Nama = "hiburan", Tipe = "tersier", HargaBeli = 6, PoinKebahagiaan = 3 },
                // Menggunakan objek baru bertipe `RulesetNeedDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun dalam
                // BuildRulesetDefinition.
                new RulesetNeedDto { Id = "jam", Nama = "jam", Tipe = "tersier", HargaBeli = 6, PoinKebahagiaan = 3 }
            // Menandai akhir daftar elemen atau indeks koleksi dalam BuildRulesetDefinition; pasangan kurung siku mengelompokkan nilai sebagai satu struktur.
            ],
            // Memperbarui `CollectionMissions` menggunakan koleksi berisi new RulesetCollectionMissionDto { Id = ”misi_bonek..., new
            // RulesetCollectionMissionDto { Id = ”misi_gameb..., new RulesetCollectionMissionDto { Id = ”misi_hibur..., new RulesetCollectionMissionDto { Id =
            // ”misi_jam”,... dalam BuildRulesetDefinition.
            CollectionMissions =
            // Menggunakan koleksi berisi new RulesetCollectionMissionDto { Id = ”misi_bonek..., new RulesetCollectionMissionDto { Id = ”misi_gameb..., new
            // RulesetCollectionMissionDto { Id = ”misi_hibur..., new RulesetCollectionMissionDto { Id = ”misi_jam”,... sebagai bagian ekspresi yang sedang
            // disusun dalam BuildRulesetDefinition.
            [
                // Menggunakan objek baru bertipe `RulesetCollectionMissionDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun
                // dalam BuildRulesetDefinition.
                new RulesetCollectionMissionDto
                // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // BuildRulesetDefinition.
                {
                    // Memperbarui `Id` menggunakan nilai literal `”misi_boneka”` dalam BuildRulesetDefinition.
                    Id = "misi_boneka",
                    // Memperbarui `Nama` menggunakan nilai literal `”boneka”` dalam BuildRulesetDefinition.
                    Nama = "boneka",
                    // Memperbarui `SuccessPoints` menggunakan nilai literal `0` dalam BuildRulesetDefinition.
                    SuccessPoints = 0,
                    // Memperbarui `FailurePoints` menggunakan `-10` dalam BuildRulesetDefinition.
                    FailurePoints = -10,
                    // Memperbarui `PenaltyPoints` menggunakan nilai literal `10` dalam BuildRulesetDefinition.
                    PenaltyPoints = 10,
                    // Memperbarui `KebutuhanTarget` menggunakan koleksi berisi new RulesetCollectionMissionRequirementDto { Order..., new
                    // RulesetCollectionMissionRequirementDto { Order... dalam BuildRulesetDefinition.
                    KebutuhanTarget =
                    // Menggunakan koleksi berisi new RulesetCollectionMissionRequirementDto { Order..., new RulesetCollectionMissionRequirementDto { Order... sebagai
                    // bagian ekspresi yang sedang disusun dalam BuildRulesetDefinition.
                    [
                        // Menggunakan objek baru bertipe `RulesetCollectionMissionRequirementDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang
                        // sedang disusun dalam BuildRulesetDefinition.
                        new RulesetCollectionMissionRequirementDto { Order = 1, Type = "primer", Value = "buku" },
                        // Menggunakan objek baru bertipe `RulesetCollectionMissionRequirementDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang
                        // sedang disusun dalam BuildRulesetDefinition.
                        new RulesetCollectionMissionRequirementDto { Order = 2, Type = "tersier", Value = "boneka" }
                    // Menandai akhir daftar elemen atau indeks koleksi dalam BuildRulesetDefinition; pasangan kurung siku mengelompokkan nilai sebagai satu struktur.
                    ]
                // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam BuildRulesetDefinition.
                },
                // Menggunakan objek baru bertipe `RulesetCollectionMissionDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun
                // dalam BuildRulesetDefinition.
                new RulesetCollectionMissionDto
                // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // BuildRulesetDefinition.
                {
                    // Memperbarui `Id` menggunakan nilai literal `”misi_gameboy”` dalam BuildRulesetDefinition.
                    Id = "misi_gameboy", Nama = "gameboy", PenaltyPoints = 10,
                    // Memperbarui `KebutuhanTarget` menggunakan koleksi berisi new RulesetCollectionMissionRequirementDto { Order..., new
                    // RulesetCollectionMissionRequirementDto { Order... dalam BuildRulesetDefinition.
                    KebutuhanTarget =
                    // Menggunakan koleksi berisi new RulesetCollectionMissionRequirementDto { Order..., new RulesetCollectionMissionRequirementDto { Order... sebagai
                    // bagian ekspresi yang sedang disusun dalam BuildRulesetDefinition.
                    [
                        // Menggunakan objek baru bertipe `RulesetCollectionMissionRequirementDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang
                        // sedang disusun dalam BuildRulesetDefinition.
                        new RulesetCollectionMissionRequirementDto { Order = 1, Type = "primer", Value = "buku" },
                        // Menggunakan objek baru bertipe `RulesetCollectionMissionRequirementDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang
                        // sedang disusun dalam BuildRulesetDefinition.
                        new RulesetCollectionMissionRequirementDto { Order = 2, Type = "tersier", Value = "gameboy" }
                    // Menandai akhir daftar elemen atau indeks koleksi dalam BuildRulesetDefinition; pasangan kurung siku mengelompokkan nilai sebagai satu struktur.
                    ]
                // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam BuildRulesetDefinition.
                },
                // Menggunakan objek baru bertipe `RulesetCollectionMissionDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun
                // dalam BuildRulesetDefinition.
                new RulesetCollectionMissionDto
                // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // BuildRulesetDefinition.
                {
                    // Memperbarui `Id` menggunakan nilai literal `”misi_hiburan”` dalam BuildRulesetDefinition.
                    Id = "misi_hiburan", Nama = "hiburan", PenaltyPoints = 10,
                    // Memperbarui `KebutuhanTarget` menggunakan koleksi berisi new RulesetCollectionMissionRequirementDto { Order..., new
                    // RulesetCollectionMissionRequirementDto { Order... dalam BuildRulesetDefinition.
                    KebutuhanTarget =
                    // Menggunakan koleksi berisi new RulesetCollectionMissionRequirementDto { Order..., new RulesetCollectionMissionRequirementDto { Order... sebagai
                    // bagian ekspresi yang sedang disusun dalam BuildRulesetDefinition.
                    [
                        // Menggunakan objek baru bertipe `RulesetCollectionMissionRequirementDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang
                        // sedang disusun dalam BuildRulesetDefinition.
                        new RulesetCollectionMissionRequirementDto { Order = 1, Type = "primer", Value = "buku" },
                        // Menggunakan objek baru bertipe `RulesetCollectionMissionRequirementDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang
                        // sedang disusun dalam BuildRulesetDefinition.
                        new RulesetCollectionMissionRequirementDto { Order = 2, Type = "tersier", Value = "hiburan" }
                    // Menandai akhir daftar elemen atau indeks koleksi dalam BuildRulesetDefinition; pasangan kurung siku mengelompokkan nilai sebagai satu struktur.
                    ]
                // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam BuildRulesetDefinition.
                },
                // Menggunakan objek baru bertipe `RulesetCollectionMissionDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun
                // dalam BuildRulesetDefinition.
                new RulesetCollectionMissionDto
                // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // BuildRulesetDefinition.
                {
                    // Memperbarui `Id` menggunakan nilai literal `”misi_jam”` dalam BuildRulesetDefinition.
                    Id = "misi_jam", Nama = "jam", PenaltyPoints = 10,
                    // Memperbarui `KebutuhanTarget` menggunakan koleksi berisi new RulesetCollectionMissionRequirementDto { Order..., new
                    // RulesetCollectionMissionRequirementDto { Order... dalam BuildRulesetDefinition.
                    KebutuhanTarget =
                    // Menggunakan koleksi berisi new RulesetCollectionMissionRequirementDto { Order..., new RulesetCollectionMissionRequirementDto { Order... sebagai
                    // bagian ekspresi yang sedang disusun dalam BuildRulesetDefinition.
                    [
                        // Menggunakan objek baru bertipe `RulesetCollectionMissionRequirementDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang
                        // sedang disusun dalam BuildRulesetDefinition.
                        new RulesetCollectionMissionRequirementDto { Order = 1, Type = "primer", Value = "buku" },
                        // Menggunakan objek baru bertipe `RulesetCollectionMissionRequirementDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang
                        // sedang disusun dalam BuildRulesetDefinition.
                        new RulesetCollectionMissionRequirementDto { Order = 2, Type = "tersier", Value = "jam" }
                    // Menandai akhir daftar elemen atau indeks koleksi dalam BuildRulesetDefinition; pasangan kurung siku mengelompokkan nilai sebagai satu struktur.
                    ]
                // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam BuildRulesetDefinition.
                }
            // Menandai akhir daftar elemen atau indeks koleksi dalam BuildRulesetDefinition; pasangan kurung siku mengelompokkan nilai sebagai satu struktur.
            ],
            // Memperbarui `FinancialGoals` menggunakan koleksi berisi new RulesetFinancialGoalDto { Id = ”beli_rumah”, N... dalam BuildRulesetDefinition.
            FinancialGoals =
            // Menggunakan koleksi berisi new RulesetFinancialGoalDto { Id = ”beli_rumah”, N... sebagai bagian ekspresi yang sedang disusun dalam
            // BuildRulesetDefinition.
            [
                // Menggunakan objek baru bertipe `RulesetFinancialGoalDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun
                // dalam BuildRulesetDefinition.
                new RulesetFinancialGoalDto
                // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // BuildRulesetDefinition.
                {
                    // Memperbarui `Id` menggunakan nilai literal `”beli_rumah”` dalam BuildRulesetDefinition.
                    Id = "beli_rumah",
                    // Memperbarui `Nama` menggunakan nilai literal `”beli rumah”` dalam BuildRulesetDefinition.
                    Nama = "beli rumah",
                    // Memperbarui `HargaBeli` menggunakan nilai literal `20` dalam BuildRulesetDefinition.
                    HargaBeli = 20,
                    // Memperbarui `PoinKebahagiaan` menggunakan nilai literal `5` dalam BuildRulesetDefinition.
                    PoinKebahagiaan = 5
                // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam BuildRulesetDefinition.
                }
            // Menandai akhir daftar elemen atau indeks koleksi dalam BuildRulesetDefinition; pasangan kurung siku mengelompokkan nilai sebagai satu struktur.
            ],
            // Memperbarui `DonationRankPoints` menggunakan koleksi berisi new RulesetDonationRankPointDto { Rank = 1, Points..., new
            // RulesetDonationRankPointDto { Rank = 2, Points..., new RulesetDonationRankPointDto { Rank = 3, Points... dalam BuildRulesetDefinition.
            DonationRankPoints =
            // Menggunakan koleksi berisi new RulesetDonationRankPointDto { Rank = 1, Points..., new RulesetDonationRankPointDto { Rank = 2, Points..., new
            // RulesetDonationRankPointDto { Rank = 3, Points... sebagai bagian ekspresi yang sedang disusun dalam BuildRulesetDefinition.
            [
                // Menggunakan objek baru bertipe `RulesetDonationRankPointDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun
                // dalam BuildRulesetDefinition.
                new RulesetDonationRankPointDto { Rank = 1, Points = 7 },
                // Menggunakan objek baru bertipe `RulesetDonationRankPointDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun
                // dalam BuildRulesetDefinition.
                new RulesetDonationRankPointDto { Rank = 2, Points = 5 },
                // Menggunakan objek baru bertipe `RulesetDonationRankPointDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun
                // dalam BuildRulesetDefinition.
                new RulesetDonationRankPointDto { Rank = 3, Points = 2 }
            // Menandai akhir daftar elemen atau indeks koleksi dalam BuildRulesetDefinition; pasangan kurung siku mengelompokkan nilai sebagai satu struktur.
            ],
            // Memperbarui `GoldPointsByQty` menggunakan koleksi berisi new RulesetGoldPointDto { Qty = 1, Points = 3 }, new RulesetGoldPointDto { Qty = 2,
            // Points = 5 }, new RulesetGoldPointDto { Qty = 3, Points = 8 }, new RulesetGoldPointDto { Qty = 4, Points = 12 } dalam BuildRulesetDefinition.
            GoldPointsByQty =
            // Menggunakan koleksi berisi new RulesetGoldPointDto { Qty = 1, Points = 3 }, new RulesetGoldPointDto { Qty = 2, Points = 5 }, new
            // RulesetGoldPointDto { Qty = 3, Points = 8 }, new RulesetGoldPointDto { Qty = 4, Points = 12 } sebagai bagian ekspresi yang sedang disusun dalam
            // BuildRulesetDefinition.
            [
                // Menggunakan objek baru bertipe `RulesetGoldPointDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun dalam
                // BuildRulesetDefinition.
                new RulesetGoldPointDto { Qty = 1, Points = 3 },
                // Menggunakan objek baru bertipe `RulesetGoldPointDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun dalam
                // BuildRulesetDefinition.
                new RulesetGoldPointDto { Qty = 2, Points = 5 },
                // Menggunakan objek baru bertipe `RulesetGoldPointDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun dalam
                // BuildRulesetDefinition.
                new RulesetGoldPointDto { Qty = 3, Points = 8 },
                // Menggunakan objek baru bertipe `RulesetGoldPointDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun dalam
                // BuildRulesetDefinition.
                new RulesetGoldPointDto { Qty = 4, Points = 12 }
            // Menandai akhir daftar elemen atau indeks koleksi dalam BuildRulesetDefinition; pasangan kurung siku mengelompokkan nilai sebagai satu struktur.
            ],
            // Memperbarui `PensionRankPoints` menggunakan koleksi berisi new RulesetPensionRankPointDto { Rank = 1, Points ..., new RulesetPensionRankPointDto
            // { Rank = 2, Points ..., new RulesetPensionRankPointDto { Rank = 3, Points ... dalam BuildRulesetDefinition.
            PensionRankPoints =
            // Menggunakan koleksi berisi new RulesetPensionRankPointDto { Rank = 1, Points ..., new RulesetPensionRankPointDto { Rank = 2, Points ..., new
            // RulesetPensionRankPointDto { Rank = 3, Points ... sebagai bagian ekspresi yang sedang disusun dalam BuildRulesetDefinition.
            [
                // Menggunakan objek baru bertipe `RulesetPensionRankPointDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun
                // dalam BuildRulesetDefinition.
                new RulesetPensionRankPointDto { Rank = 1, Points = 5 },
                // Menggunakan objek baru bertipe `RulesetPensionRankPointDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun
                // dalam BuildRulesetDefinition.
                new RulesetPensionRankPointDto { Rank = 2, Points = 3 },
                // Menggunakan objek baru bertipe `RulesetPensionRankPointDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun
                // dalam BuildRulesetDefinition.
                new RulesetPensionRankPointDto { Rank = 3, Points = 1 }
            // Menandai akhir daftar elemen atau indeks koleksi dalam BuildRulesetDefinition; pasangan kurung siku mengelompokkan nilai sebagai satu struktur.
            ],
            // Memperbarui `TieBreakers` menggunakan koleksi berisi new RulesetTieBreakerDto { TieBreakerCode = ”tie_b..., new RulesetTieBreakerDto {
            // TieBreakerCode = ”tie_b..., new RulesetTieBreakerDto { TieBreakerCode = ”tie_b..., new RulesetTieBreakerDto { TieBreakerCode = ”tie_b... dalam
            // BuildRulesetDefinition.
            TieBreakers =
            // Menggunakan koleksi berisi new RulesetTieBreakerDto { TieBreakerCode = ”tie_b..., new RulesetTieBreakerDto { TieBreakerCode = ”tie_b..., new
            // RulesetTieBreakerDto { TieBreakerCode = ”tie_b..., new RulesetTieBreakerDto { TieBreakerCode = ”tie_b... sebagai bagian ekspresi yang sedang
            // disusun dalam BuildRulesetDefinition.
            [
                // Menggunakan objek baru bertipe `RulesetTieBreakerDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun dalam
                // BuildRulesetDefinition.
                new RulesetTieBreakerDto { TieBreakerCode = "tie_breaker_1", TieNumber = 1, CardQty = 1 },
                // Menggunakan objek baru bertipe `RulesetTieBreakerDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun dalam
                // BuildRulesetDefinition.
                new RulesetTieBreakerDto { TieBreakerCode = "tie_breaker_2", TieNumber = 2, CardQty = 1 },
                // Menggunakan objek baru bertipe `RulesetTieBreakerDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun dalam
                // BuildRulesetDefinition.
                new RulesetTieBreakerDto { TieBreakerCode = "tie_breaker_3", TieNumber = 3, CardQty = 1 },
                // Menggunakan objek baru bertipe `RulesetTieBreakerDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun dalam
                // BuildRulesetDefinition.
                new RulesetTieBreakerDto { TieBreakerCode = "tie_breaker_4", TieNumber = 4, CardQty = 1 }
            // Menandai akhir daftar elemen atau indeks koleksi dalam BuildRulesetDefinition; pasangan kurung siku mengelompokkan nilai sebagai satu struktur.
            ],
            // Memperbarui `ShariaLoans` menggunakan koleksi berisi new RulesetShariaLoanDto { LoanCode = ”loan_syaria... dalam BuildRulesetDefinition.
            ShariaLoans =
            // Menggunakan koleksi berisi new RulesetShariaLoanDto { LoanCode = ”loan_syaria... sebagai bagian ekspresi yang sedang disusun dalam
            // BuildRulesetDefinition.
            [
                // Menggunakan objek baru bertipe `RulesetShariaLoanDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun dalam
                // BuildRulesetDefinition.
                new RulesetShariaLoanDto
                // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // BuildRulesetDefinition.
                {
                    // Memperbarui `LoanCode` menggunakan nilai literal `”loan_syariah_10”` dalam BuildRulesetDefinition.
                    LoanCode = "loan_syariah_10",
                    // Memperbarui `ItemName` menggunakan nilai literal `”Pinjaman Syariah 10”` dalam BuildRulesetDefinition.
                    ItemName = "Pinjaman Syariah 10",
                    // Memperbarui `Principal` menggunakan nilai literal `10` dalam BuildRulesetDefinition.
                    Principal = 10,
                    // Memperbarui `RepaymentAmount` menggunakan nilai literal `10` dalam BuildRulesetDefinition.
                    RepaymentAmount = 10,
                    // Memperbarui `DurationDays` menggunakan nilai literal `1` dalam BuildRulesetDefinition.
                    DurationDays = 1,
                    // Memperbarui `PenaltyPoints` menggunakan nilai literal `10` dalam BuildRulesetDefinition.
                    PenaltyPoints = 10,
                    // Memperbarui `CardQty` menggunakan nilai literal `8` dalam BuildRulesetDefinition.
                    CardQty = 8
                // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam BuildRulesetDefinition.
                }
            // Menandai akhir daftar elemen atau indeks koleksi dalam BuildRulesetDefinition; pasangan kurung siku mengelompokkan nilai sebagai satu struktur.
            ],
            // Memperbarui `InsuranceProducts` menggunakan koleksi berisi new RulesetInsuranceProductDto { ProductCode = ”mu... dalam BuildRulesetDefinition.
            InsuranceProducts =
            // Menggunakan koleksi berisi new RulesetInsuranceProductDto { ProductCode = ”mu... sebagai bagian ekspresi yang sedang disusun dalam
            // BuildRulesetDefinition.
            [
                // Menggunakan objek baru bertipe `RulesetInsuranceProductDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun
                // dalam BuildRulesetDefinition.
                new RulesetInsuranceProductDto
                // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // BuildRulesetDefinition.
                {
                    // Memperbarui `ProductCode` menggunakan nilai literal `”multirisk_basic”` dalam BuildRulesetDefinition.
                    ProductCode = "multirisk_basic",
                    // Memperbarui `ItemName` menggunakan nilai literal `”Asuransi”` dalam BuildRulesetDefinition.
                    ItemName = "Asuransi",
                    // Memperbarui `Premium` menggunakan nilai literal `1` dalam BuildRulesetDefinition.
                    Premium = 1,
                    // Memperbarui `UsageLimit` menggunakan nilai literal `1` dalam BuildRulesetDefinition.
                    UsageLimit = 1,
                    // Memperbarui `CardQty` menggunakan nilai literal `4` dalam BuildRulesetDefinition.
                    CardQty = 4
                // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam BuildRulesetDefinition.
                }
            // Menandai akhir daftar elemen atau indeks koleksi dalam BuildRulesetDefinition; pasangan kurung siku mengelompokkan nilai sebagai satu struktur.
            ],
            // Memperbarui `LifeRisks` menggunakan koleksi berisi new RulesetLifeRiskDto { RiskCode = ”risk_cost_4”,... dalam BuildRulesetDefinition.
            LifeRisks =
            // Menggunakan koleksi berisi new RulesetLifeRiskDto { RiskCode = ”risk_cost_4”,... sebagai bagian ekspresi yang sedang disusun dalam
            // BuildRulesetDefinition.
            [
                // Menggunakan objek baru bertipe `RulesetLifeRiskDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun dalam
                // BuildRulesetDefinition.
                new RulesetLifeRiskDto
                // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // BuildRulesetDefinition.
                {
                    // Memperbarui `RiskCode` menggunakan nilai literal `”risk_cost_4”` dalam BuildRulesetDefinition.
                    RiskCode = "risk_cost_4",
                    // Memperbarui `ItemName` menggunakan nilai literal `”Biaya 4”` dalam BuildRulesetDefinition.
                    ItemName = "Biaya 4",
                    // Memperbarui `EffectType` menggunakan nilai literal `”COIN_EFFECT”` dalam BuildRulesetDefinition.
                    EffectType = "COIN_EFFECT",
                    // Memperbarui `Direction` menggunakan nilai literal `”OUT”` dalam BuildRulesetDefinition.
                    Direction = "OUT",
                    // Memperbarui `Amount` menggunakan `riskAmount` (nilai risiko nominal) dalam BuildRulesetDefinition.
                    Amount = riskAmount,
                    // Memperbarui `TargetScope` menggunakan nilai literal `”SELF”` dalam BuildRulesetDefinition.
                    TargetScope = "SELF",
                    // Memperbarui `DurationDays` menggunakan nilai literal `1` dalam BuildRulesetDefinition.
                    DurationDays = 1,
                    // Memperbarui `CardQty` menggunakan nilai literal `4` dalam BuildRulesetDefinition.
                    CardQty = 4
                // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam BuildRulesetDefinition.
                }
            // Menandai akhir daftar elemen atau indeks koleksi dalam BuildRulesetDefinition; pasangan kurung siku mengelompokkan nilai sebagai satu struktur.
            ]
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam BuildRulesetDefinition.
        };
    // Menutup scope metode BuildRulesetDefinition; bagian berikut berada di luar batas blok tersebut dalam BuildRulesetDefinition.
    }
// Menutup scope tipe EventAnalyticsIntegrationTests; bagian berikut berada di luar batas blok tersebut.
}
