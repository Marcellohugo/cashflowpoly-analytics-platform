// Fungsi file: Memverifikasi perilaku API, database, atau domain melalui AuthRbacRulesetIntegrationTests.
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
/// Kelas pengujian integrasi yang memvalidasi alur registrasi, login, pembatasan akses RBAC,
/// serta operasi CRUD dan versioning ruleset secara end-to-end melalui API.
/// </summary>
// Mendefinisikan tipe class `AuthRbacRulesetIntegrationTests`; sealed mencegah tipe ini diturunkan lagi.
public sealed class AuthRbacRulesetIntegrationTests
// Membuka scope tipe AuthRbacRulesetIntegrationTests; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendeklarasikan field bertipe `HttpClient`: `_client` menyimpan nilai client. readonly membatasi penggantian referensi/nilai field pada deklarasi
    // atau konstruktor.
    private readonly HttpClient _client;

    [Fact]
    public async Task PlayerDirectory_InMySessions_OnlyIncludesDistinctParticipantsOfCurrentInstructor()
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];
        const string password = "IntegrationScopePass!123";
        var owner = await RegisterAsync($"scope_owner_{suffix}", password, "INSTRUCTOR");
        var other = await RegisterAsync($"scope_other_{suffix}", password, "INSTRUCTOR");
        var joined = await RegisterAsync($"scope_joined_{suffix}", password, "PLAYER");
        var elsewhere = await RegisterAsync($"scope_elsewhere_{suffix}", password, "PLAYER");
        var unjoined = await RegisterAsync($"scope_unjoined_{suffix}", password, "PLAYER");

        async Task<List<PlayerResponse>> ListAsync(string token, bool scoped)
        {
            using var response = await SendJsonAsync(HttpMethod.Get,
                scoped ? "/api/v1/players?inMySessions=true" : "/api/v1/players", null, token);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            return (await response.Content.ReadFromJsonAsync<PlayerListResponse>())!.Items;
        }

        Assert.Empty(await ListAsync(owner.AccessToken, true));
        var ruleset = await CreateRulesetAsync(owner.AccessToken, suffix, 10);
        var otherRuleset = await CreateRulesetAsync(other.AccessToken, $"other_{suffix}", 10);
        var first = await CreateSessionAsync(owner.AccessToken, $"first_{suffix}", ruleset.RulesetVersionId);
        var second = await CreateSessionAsync(owner.AccessToken, $"second_{suffix}", ruleset.RulesetVersionId);
        var otherSession = await CreateSessionAsync(other.AccessToken, $"other_{suffix}", otherRuleset.RulesetVersionId);
        Assert.Empty(await ListAsync(owner.AccessToken, true));

        foreach (var (sessionId, playerId, token) in new[]
                 {
                     (first.SessionId, joined.UserId, owner.AccessToken),
                     (second.SessionId, joined.UserId, owner.AccessToken),
                     (otherSession.SessionId, elsewhere.UserId, other.AccessToken)
                 })
        {
            using var response = await SendJsonAsync(HttpMethod.Post, $"/api/v1/sessions/{sessionId}/players",
                new { user_id = playerId, player_order_no = 1 }, token);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        Assert.Equal(joined.UserId, Assert.Single(await ListAsync(owner.AccessToken, true)).UserId);
        Assert.Equal(elsewhere.UserId, Assert.Single(await ListAsync(other.AccessToken, true)).UserId);
        var general = await ListAsync(owner.AccessToken, false);
        Assert.Contains(general, player => player.UserId == unjoined.UserId);
        Assert.Contains(general, player => player.UserId == elsewhere.UserId);
        Assert.Contains(general, player => player.UserId == joined.UserId);

        // A PLAYER cannot use the optional filter to see unrelated accounts.
        Assert.Equal(joined.UserId, Assert.Single(await ListAsync(joined.AccessToken, true)).UserId);
    }

    /// <summary>
    /// Menginisialisasi instance pengujian dengan HttpClient dari fixture integrasi bersama.
    /// </summary>
    // Mendefinisikan konstruktor AuthRbacRulesetIntegrationTests yang menyiapkan objek dan menerima dependency/nilai awal dari pemanggil; parameter:
    // Parameter `fixture` bertipe `ApiIntegrationTestFixture` membawa nilai fixture.
    public AuthRbacRulesetIntegrationTests(ApiIntegrationTestFixture fixture)
    // Membuka scope konstruktor AuthRbacRulesetIntegrationTests; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // AuthRbacRulesetIntegrationTests.
    {
        // Memperbarui `_client` menggunakan `fixture.Client` (nilai client) dalam AuthRbacRulesetIntegrationTests.
        _client = fixture.Client;
    // Menutup scope konstruktor AuthRbacRulesetIntegrationTests; bagian berikut berada di luar batas blok tersebut dalam
    // AuthRbacRulesetIntegrationTests.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    /// <summary>
    /// Memvalidasi alur lengkap: registrasi INSTRUCTOR/PLAYER, login, pembatasan akses tanpa token,
    /// CRUD ruleset dengan RBAC, aktivasi versi, penghapusan versi draft, dan pembuatan sesi permainan.
    /// </summary>
    // Mendefinisikan metode `Auth_Rbac_And_RulesetFlow_Work_EndToEnd` dengan hasil bertipe `Task`; operasi ini menangani auth rbac dan aturan flow work
    // end ke end. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task.
    public async Task Auth_Rbac_And_RulesetFlow_Work_EndToEnd()
    // Membuka scope metode Auth_Rbac_And_RulesetFlow_Work_EndToEnd; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
    {
        // Menyiapkan variabel lokal `suffix` untuk nilai suffix dengan `Guid.NewGuid().ToString(”N”)[..8]`, yaitu elemen koleksi yang dipilih melalui
        // indeks atau kunci tersebut. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var suffix = Guid.NewGuid().ToString("N")[..8];
        // Menyiapkan variabel lokal `instructorUsername` untuk nilai instruktur username dengan teks interpolasi `$”it_instructor_{suffix}”`; nilai
        // ekspresi di dalam kurung kurawal disisipkan saat program berjalan. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var instructorUsername = $"it_instructor_{suffix}";
        // Menyiapkan variabel lokal `playerUsername` untuk nilai pemain username dengan teks interpolasi `$”it_player_{suffix}”`; nilai ekspresi di dalam
        // kurung kurawal disisipkan saat program berjalan. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var playerUsername = $"it_player_{suffix}";
        // Menyiapkan variabel lokal `unrelatedPlayerUsername` untuk nilai unrelated pemain username dengan teks interpolasi
        // `$”it_player_unrelated_{suffix}”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var unrelatedPlayerUsername = $"it_player_unrelated_{suffix}";
        // Menyiapkan variabel lokal `instructorPassword` untuk nilai instruktur password dengan nilai literal `”IntegrationInstructorPass!123”`. Tipe yang
        // dipakai adalah `string`.
        const string instructorPassword = "IntegrationInstructorPass!123";
        // Menyiapkan variabel lokal `playerPassword` untuk nilai pemain password dengan nilai literal `”IntegrationPlayerPass!123”`. Tipe yang dipakai
        // adalah `string`.
        const string playerPassword = "IntegrationPlayerPass!123";

        // Menyiapkan variabel lokal `instructorRegister` untuk nilai instruktur register dengan hasil operasi asinkron memanggil `RegisterAsync` dengan
        // `instructorUsername`, `instructorPassword`, `”INSTRUCTOR”`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var instructorRegister = await RegisterAsync(instructorUsername, instructorPassword, "INSTRUCTOR");
        // Menyiapkan variabel lokal `playerRegister` untuk nilai pemain register dengan hasil operasi asinkron memanggil `RegisterAsync` dengan
        // `playerUsername`, `playerPassword`, `”PLAYER”`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var playerRegister = await RegisterAsync(playerUsername, playerPassword, "PLAYER");
        // Menyiapkan variabel lokal `unrelatedPlayerRegister` untuk nilai unrelated pemain register dengan hasil operasi asinkron memanggil `RegisterAsync`
        // dengan `unrelatedPlayerUsername`, `playerPassword`, `”PLAYER”`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var unrelatedPlayerRegister = await RegisterAsync(unrelatedPlayerUsername, playerPassword, "PLAYER");

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”INSTRUCTOR”`, `instructorRegister.Role`);
        // pengujian gagal jika keduanya berbeda dalam Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        Assert.Equal("INSTRUCTOR", instructorRegister.Role);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”PLAYER”`, `playerRegister.Role`); pengujian
        // gagal jika keduanya berbeda dalam Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        Assert.Equal("PLAYER", playerRegister.Role);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”PLAYER”`, `unrelatedPlayerRegister.Role`);
        // pengujian gagal jika keduanya berbeda dalam Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        Assert.Equal("PLAYER", unrelatedPlayerRegister.Role);

        // Menyiapkan variabel lokal `instructorLogin` untuk nilai instruktur login dengan hasil operasi asinkron memanggil `LoginAsync` dengan
        // `instructorUsername`, `instructorPassword`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var instructorLogin = await LoginAsync(instructorUsername, instructorPassword);
        // Menyiapkan variabel lokal `playerLogin` untuk nilai pemain login dengan hasil operasi asinkron memanggil `LoginAsync` dengan `playerUsername`,
        // `playerPassword`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var playerLogin = await LoginAsync(playerUsername, playerPassword);
        // Menyiapkan variabel lokal `unrelatedPlayerLogin` untuk nilai unrelated pemain login dengan hasil operasi asinkron memanggil `LoginAsync` dengan
        // `unrelatedPlayerUsername`, `playerPassword`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var unrelatedPlayerLogin = await LoginAsync(unrelatedPlayerUsername, playerPassword);

        // Menjalankan pemeriksaan bahwa `string.IsNullOrWhiteSpace(instructorLogin.AccessToken)` bernilai salah; pengujian gagal jika kondisi justru
        // terpenuhi dalam Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        Assert.False(string.IsNullOrWhiteSpace(instructorLogin.AccessToken));
        // Menjalankan pemeriksaan bahwa `string.IsNullOrWhiteSpace(playerLogin.AccessToken)` bernilai salah; pengujian gagal jika kondisi justru terpenuhi
        // dalam Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        Assert.False(string.IsNullOrWhiteSpace(playerLogin.AccessToken));

        // Menyiapkan variabel lokal `withoutToken` untuk nilai tanpa token dengan hasil operasi asinkron memanggil `_client.GetAsync` dengan
        // `”/api/v1/sessions”`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai
        // awal.
        var withoutToken = await _client.GetAsync("/api/v1/sessions");
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.Unauthorized`,
        // `withoutToken.StatusCode`); pengujian gagal jika keduanya berbeda dalam Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        Assert.Equal(HttpStatusCode.Unauthorized, withoutToken.StatusCode);

        // Menyiapkan variabel lokal `instructorRulesetsBeforeCreate` untuk nilai instruktur aturan before create dengan hasil operasi asinkron memanggil
        // `SendJsonAsync` dengan `HttpMethod.Get`, `”/api/v1/rulesets”`, `null`, `instructorLogin.AccessToken`; await menunggu hasil tanpa memblokir thread
        // selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var instructorRulesetsBeforeCreate = await SendJsonAsync(
            // Meneruskan `HttpMethod.Get` (nilai get) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Get,
            // Meneruskan nilai literal `”/api/v1/rulesets”` sebagai argumen ke `SendJsonAsync`.
            "/api/v1/rulesets",
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen bernama `body`.
            body: null,
            // Meneruskan `instructorLogin.AccessToken` (nilai akses token) sebagai argumen ke `SendJsonAsync`.
            instructorLogin.AccessToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`,
        // `instructorRulesetsBeforeCreate.StatusCode`); pengujian gagal jika keduanya berbeda dalam Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        Assert.Equal(HttpStatusCode.OK, instructorRulesetsBeforeCreate.StatusCode);
        // Menjalankan hasil operasi asinkron memanggil `AssertRulesetListIncludesDefaultRowsAsync` dengan `instructorRulesetsBeforeCreate`; await menunggu
        // hasil tanpa memblokir thread selama operasi belum selesai dalam Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        await AssertRulesetListIncludesDefaultRowsAsync(instructorRulesetsBeforeCreate);

        // Menyiapkan variabel lokal `playerRulesetsBeforeSession` untuk nilai pemain aturan before sesi dengan hasil operasi asinkron memanggil
        // `SendJsonAsync` dengan `HttpMethod.Get`, `”/api/v1/rulesets”`, `null`, `playerLogin.AccessToken`; await menunggu hasil tanpa memblokir thread
        // selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var playerRulesetsBeforeSession = await SendJsonAsync(
            // Meneruskan `HttpMethod.Get` (nilai get) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Get,
            // Meneruskan nilai literal `”/api/v1/rulesets”` sebagai argumen ke `SendJsonAsync`.
            "/api/v1/rulesets",
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen bernama `body`.
            body: null,
            // Meneruskan `playerLogin.AccessToken` (nilai akses token) sebagai argumen ke `SendJsonAsync`.
            playerLogin.AccessToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`,
        // `playerRulesetsBeforeSession.StatusCode`); pengujian gagal jika keduanya berbeda dalam Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        Assert.Equal(HttpStatusCode.OK, playerRulesetsBeforeSession.StatusCode);
        // Menjalankan hasil operasi asinkron memanggil `AssertRulesetListIncludesDefaultRowsAsync` dengan `playerRulesetsBeforeSession`; await menunggu
        // hasil tanpa memblokir thread selama operasi belum selesai dalam Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        await AssertRulesetListIncludesDefaultRowsAsync(playerRulesetsBeforeSession);

        // Menyiapkan variabel lokal `defaultComponentsByInstructor` untuk nilai bawaan komponen berdasarkan instruktur dengan hasil operasi asinkron
        // memanggil `SendJsonAsync` dengan `HttpMethod.Get`, `”/api/v1/rulesets/components/defaults”`, `null`, `instructorLogin.AccessToken`; await
        // menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var defaultComponentsByInstructor = await SendJsonAsync(
            // Meneruskan `HttpMethod.Get` (nilai get) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Get,
            // Meneruskan nilai literal `”/api/v1/rulesets/components/defaults”` sebagai argumen ke `SendJsonAsync`.
            "/api/v1/rulesets/components/defaults",
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen bernama `body`.
            body: null,
            // Meneruskan `instructorLogin.AccessToken` (nilai akses token) sebagai argumen ke `SendJsonAsync`.
            instructorLogin.AccessToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`,
        // `defaultComponentsByInstructor.StatusCode`); pengujian gagal jika keduanya berbeda dalam Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        Assert.Equal(HttpStatusCode.OK, defaultComponentsByInstructor.StatusCode);

        // Menyiapkan variabel lokal `defaultComponentsInstructorPayload` untuk nilai bawaan komponen instruktur payload dengan hasil operasi asinkron
        // membaca tanpa argumen menjadi objek bertipe sesuai kontrak JSON melalui
        // `defaultComponentsByInstructor.Content.ReadFromJsonAsync<DefaultRulesetComponentsResponse>`; await menunggu hasil tanpa memblokir thread selama
        // operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var defaultComponentsInstructorPayload =
            // Menggunakan hasil operasi asinkron membaca tanpa argumen menjadi objek bertipe sesuai kontrak JSON melalui
            // `defaultComponentsByInstructor.Content.ReadFromJsonAsync<DefaultRulesetComponentsResponse>`; await menunggu hasil tanpa memblokir thread selama
            // operasi belum selesai sebagai bagian ekspresi yang sedang disusun dalam Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
            await defaultComponentsByInstructor.Content.ReadFromJsonAsync<DefaultRulesetComponentsResponse>();
        // Menjalankan pemeriksaan NotNull atas `defaultComponentsInstructorPayload` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        Assert.NotNull(defaultComponentsInstructorPayload);
        // Menjalankan pemeriksaan NotNull atas `defaultComponentsInstructorPayload.Items` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        Assert.NotNull(defaultComponentsInstructorPayload.Items);
        // Memeriksa pemeriksaan lebih besar antara `defaultComponentsInstructorPayload.Items.Count` dan `0`; blok if hanya dijalankan ketika kondisi ini
        // bernilai benar dalam Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        if (defaultComponentsInstructorPayload.Items.Count > 0)
        // Membuka scope cabang if untuk kondisi `defaultComponentsInstructorPayload.Items.Count > 0`; pernyataan/deklarasi berikut berada di dalam batas
        // blok ini dalam Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        {
            // Menyiapkan variabel lokal `defaultRuleset` untuk nilai bawaan aturan dengan `defaultComponentsInstructorPayload.Items[0]`, yaitu elemen koleksi
            // yang dipilih melalui indeks atau kunci tersebut. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var defaultRuleset = defaultComponentsInstructorPayload.Items[0];
            // Menyiapkan variabel lokal `defaultRulesetDetailByInstructor` untuk nilai bawaan aturan detail berdasarkan instruktur dengan hasil operasi
            // asinkron memanggil `SendJsonAsync` dengan `HttpMethod.Get`, `$”/api/v1/rulesets/{defaultRuleset.RulesetId}”`, `null`,
            // `instructorLogin.AccessToken`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi
            // nilai awal.
            var defaultRulesetDetailByInstructor = await SendJsonAsync(
                // Meneruskan `HttpMethod.Get` (nilai get) sebagai argumen ke `SendJsonAsync`.
                HttpMethod.Get,
                // Meneruskan teks interpolasi `$”/api/v1/rulesets/{defaultRuleset.RulesetId}”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program
                // berjalan sebagai argumen ke `SendJsonAsync`.
                $"/api/v1/rulesets/{defaultRuleset.RulesetId}",
                // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen bernama `body`.
                body: null,
                // Meneruskan `instructorLogin.AccessToken` (nilai akses token) sebagai argumen ke `SendJsonAsync`.
                instructorLogin.AccessToken);
            // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`,
            // `defaultRulesetDetailByInstructor.StatusCode`); pengujian gagal jika keduanya berbeda dalam Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
            Assert.Equal(HttpStatusCode.OK, defaultRulesetDetailByInstructor.StatusCode);

            // Menyiapkan variabel lokal `defaultRulesetComponentsByInstructor` untuk nilai bawaan aturan komponen berdasarkan instruktur dengan hasil operasi
            // asinkron memanggil `SendJsonAsync` dengan `HttpMethod.Get`, `$”/api/v1/rulesets/{defaultRuleset.RulesetId}/components”`, `null`,
            // `instructorLogin.AccessToken`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi
            // nilai awal.
            var defaultRulesetComponentsByInstructor = await SendJsonAsync(
                // Meneruskan `HttpMethod.Get` (nilai get) sebagai argumen ke `SendJsonAsync`.
                HttpMethod.Get,
                // Meneruskan teks interpolasi `$”/api/v1/rulesets/{defaultRuleset.RulesetId}/components”`; nilai ekspresi di dalam kurung kurawal disisipkan saat
                // program berjalan sebagai argumen ke `SendJsonAsync`.
                $"/api/v1/rulesets/{defaultRuleset.RulesetId}/components",
                // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen bernama `body`.
                body: null,
                // Meneruskan `instructorLogin.AccessToken` (nilai akses token) sebagai argumen ke `SendJsonAsync`.
                instructorLogin.AccessToken);
            // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`,
            // `defaultRulesetComponentsByInstructor.StatusCode`); pengujian gagal jika keduanya berbeda dalam Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
            Assert.Equal(HttpStatusCode.OK, defaultRulesetComponentsByInstructor.StatusCode);
        // Menutup scope cabang if untuk kondisi `defaultComponentsInstructorPayload.Items.Count > 0`; bagian berikut berada di luar batas blok tersebut
        // dalam Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        }

        // Menyiapkan variabel lokal `defaultComponentsByPlayer` untuk nilai bawaan komponen berdasarkan pemain dengan hasil operasi asinkron memanggil
        // `SendJsonAsync` dengan `HttpMethod.Get`, `”/api/v1/rulesets/components/defaults”`, `null`, `playerLogin.AccessToken`; await menunggu hasil tanpa
        // memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var defaultComponentsByPlayer = await SendJsonAsync(
            // Meneruskan `HttpMethod.Get` (nilai get) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Get,
            // Meneruskan nilai literal `”/api/v1/rulesets/components/defaults”` sebagai argumen ke `SendJsonAsync`.
            "/api/v1/rulesets/components/defaults",
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen bernama `body`.
            body: null,
            // Meneruskan `playerLogin.AccessToken` (nilai akses token) sebagai argumen ke `SendJsonAsync`.
            playerLogin.AccessToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`,
        // `defaultComponentsByPlayer.StatusCode`); pengujian gagal jika keduanya berbeda dalam Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        Assert.Equal(HttpStatusCode.OK, defaultComponentsByPlayer.StatusCode);

        // Menyiapkan variabel lokal `gameComponentsPemula` untuk nilai game komponen pemula dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Get`, `”/api/v1/game-components?mode=PEMULA”`, `null`, `instructorLogin.AccessToken`; await menunggu hasil tanpa memblokir thread
        // selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var gameComponentsPemula = await SendJsonAsync(
            // Meneruskan `HttpMethod.Get` (nilai get) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Get,
            // Meneruskan nilai literal `”/api/v1/game-components?mode=PEMULA”` sebagai argumen ke `SendJsonAsync`.
            "/api/v1/game-components?mode=PEMULA",
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen bernama `body`.
            body: null,
            // Meneruskan `instructorLogin.AccessToken` (nilai akses token) sebagai argumen ke `SendJsonAsync`.
            instructorLogin.AccessToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`,
        // `gameComponentsPemula.StatusCode`); pengujian gagal jika keduanya berbeda dalam Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        Assert.Equal(HttpStatusCode.OK, gameComponentsPemula.StatusCode);

        // Menyiapkan variabel lokal `gameComponentsPemulaPayload` untuk nilai game komponen pemula payload dengan hasil operasi asinkron membaca tanpa
        // argumen menjadi objek bertipe sesuai kontrak JSON melalui `gameComponentsPemula.Content.ReadFromJsonAsync<DefaultRulesetComponentsResponse>`;
        // await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var gameComponentsPemulaPayload =
            // Menggunakan hasil operasi asinkron membaca tanpa argumen menjadi objek bertipe sesuai kontrak JSON melalui
            // `gameComponentsPemula.Content.ReadFromJsonAsync<DefaultRulesetComponentsResponse>`; await menunggu hasil tanpa memblokir thread selama operasi
            // belum selesai sebagai bagian ekspresi yang sedang disusun dalam Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
            await gameComponentsPemula.Content.ReadFromJsonAsync<DefaultRulesetComponentsResponse>();
        // Menjalankan pemeriksaan NotNull atas `gameComponentsPemulaPayload` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        Assert.NotNull(gameComponentsPemulaPayload);
        // Menjalankan pemeriksaan NotNull atas `gameComponentsPemulaPayload.Items` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        Assert.NotNull(gameComponentsPemulaPayload.Items);
        // Menjalankan pemeriksaan hasil dengan `Assert.All` menggunakan `gameComponentsPemulaPayload.Items`, `item => Assert.Equal(”PEMULA”, (item.Mode ??
        // string.Empty).ToUpperInvariant())`; ketidaksesuaian dengan ekspektasi membuat pengujian gagal dalam Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        Assert.All(gameComponentsPemulaPayload.Items, item =>
            // Meneruskan nilai literal `”PEMULA”` sebagai argumen ke `Assert.Equal`; Meneruskan menormalisasi `(item.Mode ?? string.Empty)` menjadi huruf besar
            // dengan aturan kultur invariant sebagai argumen ke `Assert.Equal`.
            Assert.Equal("PEMULA", (item.Mode ?? string.Empty).ToUpperInvariant()));

        // Menyiapkan variabel lokal `gameComponentsInvalidMode` untuk nilai game komponen invalid mode dengan hasil operasi asinkron memanggil
        // `SendJsonAsync` dengan `HttpMethod.Get`, `”/api/v1/game-components?mode=INVALID”`, `null`, `instructorLogin.AccessToken`; await menunggu hasil
        // tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var gameComponentsInvalidMode = await SendJsonAsync(
            // Meneruskan `HttpMethod.Get` (nilai get) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Get,
            // Meneruskan nilai literal `”/api/v1/game-components?mode=INVALID”` sebagai argumen ke `SendJsonAsync`.
            "/api/v1/game-components?mode=INVALID",
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen bernama `body`.
            body: null,
            // Meneruskan `instructorLogin.AccessToken` (nilai akses token) sebagai argumen ke `SendJsonAsync`.
            instructorLogin.AccessToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.BadRequest`,
        // `gameComponentsInvalidMode.StatusCode`); pengujian gagal jika keduanya berbeda dalam Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        Assert.Equal(HttpStatusCode.BadRequest, gameComponentsInvalidMode.StatusCode);

        // Menyiapkan variabel lokal `createRulesetPayload` untuk nilai create aturan payload dengan objek anonim yang mengelompokkan name, description,
        // definition sebagai satu nilai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var createRulesetPayload = new
        // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        {
            // Menggunakan `name` (nilai nama) sebagai bagian ekspresi yang sedang disusun dalam Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
            name = $"Ruleset IT {suffix}",
            // Menggunakan `description` (nilai description) sebagai bagian ekspresi yang sedang disusun dalam Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
            description = "Integration test ruleset",
            // Menggunakan `definition` (definisi terstruktur komponen serta parameter aturan permainan) sebagai bagian ekspresi yang sedang disusun dalam
            // Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
            definition = BuildRulesetDefinition(startingCash: 20)
        // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
        // Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        };

        // Menyiapkan variabel lokal `playerCreateRuleset` untuk nilai pemain create aturan dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Post`, `”/api/v1/rulesets”`, `createRulesetPayload`, `playerLogin.AccessToken`; await menunggu hasil tanpa memblokir thread selama
        // operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var playerCreateRuleset = await SendJsonAsync(
            // Meneruskan `HttpMethod.Post` (nilai post) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Post,
            // Meneruskan nilai literal `”/api/v1/rulesets”` sebagai argumen ke `SendJsonAsync`.
            "/api/v1/rulesets",
            // Meneruskan `createRulesetPayload` (nilai create aturan payload) sebagai argumen ke `SendJsonAsync`.
            createRulesetPayload,
            // Meneruskan `playerLogin.AccessToken` (nilai akses token) sebagai argumen ke `SendJsonAsync`.
            playerLogin.AccessToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.Forbidden`,
        // `playerCreateRuleset.StatusCode`); pengujian gagal jika keduanya berbeda dalam Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        Assert.Equal(HttpStatusCode.Forbidden, playerCreateRuleset.StatusCode);

        // Menyiapkan variabel lokal `instructorCreateRuleset` untuk nilai instruktur create aturan dengan hasil operasi asinkron memanggil `SendJsonAsync`
        // dengan `HttpMethod.Post`, `”/api/v1/rulesets”`, `createRulesetPayload`, `instructorLogin.AccessToken`; await menunggu hasil tanpa memblokir
        // thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var instructorCreateRuleset = await SendJsonAsync(
            // Meneruskan `HttpMethod.Post` (nilai post) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Post,
            // Meneruskan nilai literal `”/api/v1/rulesets”` sebagai argumen ke `SendJsonAsync`.
            "/api/v1/rulesets",
            // Meneruskan `createRulesetPayload` (nilai create aturan payload) sebagai argumen ke `SendJsonAsync`.
            createRulesetPayload,
            // Meneruskan `instructorLogin.AccessToken` (nilai akses token) sebagai argumen ke `SendJsonAsync`.
            instructorLogin.AccessToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.Created`,
        // `instructorCreateRuleset.StatusCode`); pengujian gagal jika keduanya berbeda dalam Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        Assert.Equal(HttpStatusCode.Created, instructorCreateRuleset.StatusCode);

        // Menyiapkan variabel lokal `createdRuleset` untuk nilai created aturan dengan hasil operasi asinkron membaca tanpa argumen menjadi objek bertipe
        // sesuai kontrak JSON melalui `instructorCreateRuleset.Content.ReadFromJsonAsync<CreateRulesetResponse>`; await menunggu hasil tanpa memblokir
        // thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var createdRuleset = await instructorCreateRuleset.Content.ReadFromJsonAsync<CreateRulesetResponse>();
        // Menjalankan pemeriksaan NotNull atas `createdRuleset` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        Assert.NotNull(createdRuleset);
        // Menjalankan pemeriksaan hasil dengan `Assert.NotEqual` menggunakan `Guid.Empty`, `createdRuleset.RulesetId`; ketidaksesuaian dengan ekspektasi
        // membuat pengujian gagal dalam Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        Assert.NotEqual(Guid.Empty, createdRuleset.RulesetId);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`1`, `createdRuleset.Version`); pengujian gagal
        // jika keduanya berbeda dalam Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        Assert.Equal(1, createdRuleset.Version);

        // Menyiapkan variabel lokal `getComponentsResponse` untuk nilai get komponen respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Get`, `$”/api/v1/rulesets/{createdRuleset.RulesetId}/components”`, `null`, `instructorLogin.AccessToken`; await menunggu hasil tanpa
        // memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var getComponentsResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Get` (nilai get) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Get,
            // Meneruskan teks interpolasi `$”/api/v1/rulesets/{createdRuleset.RulesetId}/components”`; nilai ekspresi di dalam kurung kurawal disisipkan saat
            // program berjalan sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/rulesets/{createdRuleset.RulesetId}/components",
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen bernama `body`.
            body: null,
            // Meneruskan `instructorLogin.AccessToken` (nilai akses token) sebagai argumen ke `SendJsonAsync`.
            instructorLogin.AccessToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`,
        // `getComponentsResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        Assert.Equal(HttpStatusCode.OK, getComponentsResponse.StatusCode);

        // Menyiapkan variabel lokal `components` untuk nilai komponen dengan hasil operasi asinkron membaca tanpa argumen menjadi objek bertipe sesuai
        // kontrak JSON melalui `getComponentsResponse.Content.ReadFromJsonAsync<RulesetComponentsResponse>`; await menunggu hasil tanpa memblokir thread
        // selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var components = await getComponentsResponse.Content.ReadFromJsonAsync<RulesetComponentsResponse>();
        // Menjalankan pemeriksaan NotNull atas `components` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        Assert.NotNull(components);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`createdRuleset.RulesetId`,
        // `components.RulesetId`); pengujian gagal jika keduanya berbeda dalam Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        Assert.Equal(createdRuleset.RulesetId, components.RulesetId);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`1`, `components.Version`); pengujian gagal
        // jika keduanya berbeda dalam Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        Assert.Equal(1, components.Version);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”PEMULA”`, `components.Mode`); pengujian gagal
        // jika keduanya berbeda dalam Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        Assert.Equal("PEMULA", components.Mode);
        // Menjalankan pemeriksaan NotNull atas `components.Definition` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        Assert.NotNull(components.Definition);
        // Menjalankan pemeriksaan hasil dengan `Assert.NotEmpty` menggunakan `components.Definition!.Actions`; ketidaksesuaian dengan ekspektasi membuat
        // pengujian gagal dalam Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        Assert.NotEmpty(components.Definition!.Actions);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `components.Definition.Narratives.SelectMany(item => item.PrerequisiteAksi)`, `item => item.Aksi == ”JualMasakan” && item.Value == 1` dalam
        // Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        Assert.Contains(
            // Meneruskan meratakan hasil koleksi bertingkat dari `components.Definition.Narratives` melalui `item => item.PrerequisiteAksi` menjadi satu urutan
            // sebagai argumen ke `Assert.Contains`; Meneruskan fungsi lambda `item => item.PrerequisiteAksi` yang dijalankan oleh operasi pemanggil untuk
            // memproses setiap masukan sebagai argumen ke `components.Definition.Narratives.SelectMany`.
            components.Definition.Narratives.SelectMany(item => item.PrerequisiteAksi),
            // Parameter `item` bertipe `` membawa nilai elemen.
            item => item.Aksi == "JualMasakan" && item.Value == 1);

        // Menyiapkan variabel lokal `duplicateConfigUpdatePayload` untuk nilai duplicate konfigurasi update payload dengan objek anonim yang mengelompokkan
        // name, description, definition sebagai satu nilai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var duplicateConfigUpdatePayload = new
        // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        {
            // Menggunakan `name` (nilai nama) sebagai bagian ekspresi yang sedang disusun dalam Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
            name = $"Ruleset IT {suffix} duplicate",
            // Menggunakan `description` (nilai description) sebagai bagian ekspresi yang sedang disusun dalam Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
            description = "Integration test duplicate config",
            // Menggunakan `definition` (definisi terstruktur komponen serta parameter aturan permainan) sebagai bagian ekspresi yang sedang disusun dalam
            // Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
            definition = BuildRulesetDefinition(startingCash: 20)
        // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
        // Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        };

        // Menyiapkan variabel lokal `duplicateConfigUpdate` untuk nilai duplicate konfigurasi update dengan hasil operasi asinkron memanggil
        // `SendJsonAsync` dengan `HttpMethod.Put`, `$”/api/v1/rulesets/{createdRuleset.RulesetId}”`, `duplicateConfigUpdatePayload`,
        // `instructorLogin.AccessToken`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi
        // nilai awal.
        var duplicateConfigUpdate = await SendJsonAsync(
            // Meneruskan `HttpMethod.Put` (nilai put) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Put,
            // Meneruskan teks interpolasi `$”/api/v1/rulesets/{createdRuleset.RulesetId}”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program
            // berjalan sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/rulesets/{createdRuleset.RulesetId}",
            // Meneruskan `duplicateConfigUpdatePayload` (nilai duplicate konfigurasi update payload) sebagai argumen ke `SendJsonAsync`.
            duplicateConfigUpdatePayload,
            // Meneruskan `instructorLogin.AccessToken` (nilai akses token) sebagai argumen ke `SendJsonAsync`.
            instructorLogin.AccessToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.Conflict`,
        // `duplicateConfigUpdate.StatusCode`); pengujian gagal jika keduanya berbeda dalam Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        Assert.Equal(HttpStatusCode.Conflict, duplicateConfigUpdate.StatusCode);

        // Menyiapkan variabel lokal `updateRulesetPayload` untuk nilai update aturan payload dengan objek anonim yang mengelompokkan name, description,
        // definition sebagai satu nilai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var updateRulesetPayload = new
        // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        {
            // Menggunakan `name` (nilai nama) sebagai bagian ekspresi yang sedang disusun dalam Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
            name = $"Ruleset IT {suffix} V2",
            // Menggunakan `description` (nilai description) sebagai bagian ekspresi yang sedang disusun dalam Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
            description = "Integration test ruleset v2",
            // Menggunakan `definition` (definisi terstruktur komponen serta parameter aturan permainan) sebagai bagian ekspresi yang sedang disusun dalam
            // Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
            definition = BuildRulesetDefinition(startingCash: 21)
        // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
        // Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        };

        // Menyiapkan variabel lokal `instructorUpdateRuleset` untuk nilai instruktur update aturan dengan hasil operasi asinkron memanggil `SendJsonAsync`
        // dengan `HttpMethod.Put`, `$”/api/v1/rulesets/{createdRuleset.RulesetId}”`, `updateRulesetPayload`, `instructorLogin.AccessToken`; await menunggu
        // hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var instructorUpdateRuleset = await SendJsonAsync(
            // Meneruskan `HttpMethod.Put` (nilai put) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Put,
            // Meneruskan teks interpolasi `$”/api/v1/rulesets/{createdRuleset.RulesetId}”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program
            // berjalan sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/rulesets/{createdRuleset.RulesetId}",
            // Meneruskan `updateRulesetPayload` (nilai update aturan payload) sebagai argumen ke `SendJsonAsync`.
            updateRulesetPayload,
            // Meneruskan `instructorLogin.AccessToken` (nilai akses token) sebagai argumen ke `SendJsonAsync`.
            instructorLogin.AccessToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`,
        // `instructorUpdateRuleset.StatusCode`); pengujian gagal jika keduanya berbeda dalam Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        Assert.Equal(HttpStatusCode.OK, instructorUpdateRuleset.StatusCode);

        // Menyiapkan variabel lokal `updatedRuleset` untuk nilai updated aturan dengan hasil operasi asinkron membaca tanpa argumen menjadi objek bertipe
        // sesuai kontrak JSON melalui `instructorUpdateRuleset.Content.ReadFromJsonAsync<CreateRulesetResponse>`; await menunggu hasil tanpa memblokir
        // thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var updatedRuleset = await instructorUpdateRuleset.Content.ReadFromJsonAsync<CreateRulesetResponse>();
        // Menjalankan pemeriksaan NotNull atas `updatedRuleset` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        Assert.NotNull(updatedRuleset);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`createdRuleset.RulesetId`,
        // `updatedRuleset.RulesetId`); pengujian gagal jika keduanya berbeda dalam Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        Assert.Equal(createdRuleset.RulesetId, updatedRuleset.RulesetId);
        // Menjalankan pemeriksaan bahwa `updatedRuleset.Version >= 2` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        Assert.True(updatedRuleset.Version >= 2);

        // Menyiapkan variabel lokal `playerActivateVersion` untuk nilai pemain activate versi dengan hasil operasi asinkron memanggil `SendJsonAsync`
        // dengan `HttpMethod.Post`, `$”/api/v1/rulesets/{createdRuleset.RulesetId}/versions/{updatedRuleset.Version}/activate”`, `null`,
        // `playerLogin.AccessToken`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi
        // nilai awal.
        var playerActivateVersion = await SendJsonAsync(
            // Meneruskan `HttpMethod.Post` (nilai post) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Post,
            // Meneruskan teks interpolasi `$”/api/v1/rulesets/{createdRuleset.RulesetId}/versions/{updatedRuleset.Version}/activate”`; nilai ekspresi di dalam
            // kurung kurawal disisipkan saat program berjalan sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/rulesets/{createdRuleset.RulesetId}/versions/{updatedRuleset.Version}/activate",
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen bernama `body`.
            body: null,
            // Meneruskan `playerLogin.AccessToken` (nilai akses token) sebagai argumen ke `SendJsonAsync`.
            playerLogin.AccessToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.Forbidden`,
        // `playerActivateVersion.StatusCode`); pengujian gagal jika keduanya berbeda dalam Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        Assert.Equal(HttpStatusCode.Forbidden, playerActivateVersion.StatusCode);

        // Menyiapkan variabel lokal `instructorActivateVersion` untuk nilai instruktur activate versi dengan hasil operasi asinkron memanggil
        // `SendJsonAsync` dengan `HttpMethod.Post`, `$”/api/v1/rulesets/{createdRuleset.RulesetId}/versions/{updatedRuleset.Version}/activate”`, `null`,
        // `instructorLogin.AccessToken`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi
        // nilai awal.
        var instructorActivateVersion = await SendJsonAsync(
            // Meneruskan `HttpMethod.Post` (nilai post) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Post,
            // Meneruskan teks interpolasi `$”/api/v1/rulesets/{createdRuleset.RulesetId}/versions/{updatedRuleset.Version}/activate”`; nilai ekspresi di dalam
            // kurung kurawal disisipkan saat program berjalan sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/rulesets/{createdRuleset.RulesetId}/versions/{updatedRuleset.Version}/activate",
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen bernama `body`.
            body: null,
            // Meneruskan `instructorLogin.AccessToken` (nilai akses token) sebagai argumen ke `SendJsonAsync`.
            instructorLogin.AccessToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`,
        // `instructorActivateVersion.StatusCode`); pengujian gagal jika keduanya berbeda dalam Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        Assert.Equal(HttpStatusCode.OK, instructorActivateVersion.StatusCode);

        // Menyiapkan variabel lokal `updateRulesetPayloadV3` untuk nilai update aturan payload 3 dengan objek anonim yang mengelompokkan name, description,
        // definition sebagai satu nilai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var updateRulesetPayloadV3 = new
        // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        {
            // Menggunakan `name` (nilai nama) sebagai bagian ekspresi yang sedang disusun dalam Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
            name = $"Ruleset IT {suffix} V3",
            // Menggunakan `description` (nilai description) sebagai bagian ekspresi yang sedang disusun dalam Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
            description = "Integration test ruleset v3",
            // Menggunakan `definition` (definisi terstruktur komponen serta parameter aturan permainan) sebagai bagian ekspresi yang sedang disusun dalam
            // Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
            definition = BuildRulesetDefinition(startingCash: 22)
        // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
        // Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        };

        // Menyiapkan variabel lokal `instructorUpdateRulesetV3` untuk nilai instruktur update aturan 3 dengan hasil operasi asinkron memanggil
        // `SendJsonAsync` dengan `HttpMethod.Put`, `$”/api/v1/rulesets/{createdRuleset.RulesetId}”`, `updateRulesetPayloadV3`,
        // `instructorLogin.AccessToken`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi
        // nilai awal.
        var instructorUpdateRulesetV3 = await SendJsonAsync(
            // Meneruskan `HttpMethod.Put` (nilai put) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Put,
            // Meneruskan teks interpolasi `$”/api/v1/rulesets/{createdRuleset.RulesetId}”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program
            // berjalan sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/rulesets/{createdRuleset.RulesetId}",
            // Meneruskan `updateRulesetPayloadV3` (nilai update aturan payload 3) sebagai argumen ke `SendJsonAsync`.
            updateRulesetPayloadV3,
            // Meneruskan `instructorLogin.AccessToken` (nilai akses token) sebagai argumen ke `SendJsonAsync`.
            instructorLogin.AccessToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`,
        // `instructorUpdateRulesetV3.StatusCode`); pengujian gagal jika keduanya berbeda dalam Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        Assert.Equal(HttpStatusCode.OK, instructorUpdateRulesetV3.StatusCode);

        // Menyiapkan variabel lokal `updatedRulesetV3` untuk nilai updated aturan 3 dengan hasil operasi asinkron membaca tanpa argumen menjadi objek
        // bertipe sesuai kontrak JSON melalui `instructorUpdateRulesetV3.Content.ReadFromJsonAsync<CreateRulesetResponse>`; await menunggu hasil tanpa
        // memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var updatedRulesetV3 = await instructorUpdateRulesetV3.Content.ReadFromJsonAsync<CreateRulesetResponse>();
        // Menjalankan pemeriksaan NotNull atas `updatedRulesetV3` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        Assert.NotNull(updatedRulesetV3);
        // Menjalankan pemeriksaan bahwa `updatedRulesetV3.Version > updatedRuleset.Version` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi
        // dalam Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        Assert.True(updatedRulesetV3.Version > updatedRuleset.Version);

        // Menyiapkan variabel lokal `playerDeleteVersion` untuk nilai pemain delete versi dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Delete`, `$”/api/v1/rulesets/{createdRuleset.RulesetId}/versions/{updatedRulesetV3.Version}”`, `null`, `playerLogin.AccessToken`;
        // await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var playerDeleteVersion = await SendJsonAsync(
            // Meneruskan `HttpMethod.Delete` (nilai delete) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Delete,
            // Meneruskan teks interpolasi `$”/api/v1/rulesets/{createdRuleset.RulesetId}/versions/{updatedRulesetV3.Version}”`; nilai ekspresi di dalam kurung
            // kurawal disisipkan saat program berjalan sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/rulesets/{createdRuleset.RulesetId}/versions/{updatedRulesetV3.Version}",
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen bernama `body`.
            body: null,
            // Meneruskan `playerLogin.AccessToken` (nilai akses token) sebagai argumen ke `SendJsonAsync`.
            playerLogin.AccessToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.Forbidden`,
        // `playerDeleteVersion.StatusCode`); pengujian gagal jika keduanya berbeda dalam Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        Assert.Equal(HttpStatusCode.Forbidden, playerDeleteVersion.StatusCode);

        // Menyiapkan variabel lokal `instructorDeleteActiveVersion` untuk nilai instruktur delete aktif versi dengan hasil operasi asinkron memanggil
        // `SendJsonAsync` dengan `HttpMethod.Delete`, `$”/api/v1/rulesets/{createdRuleset.RulesetId}/versions/{updatedRuleset.Version}”`, `null`,
        // `instructorLogin.AccessToken`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi
        // nilai awal.
        var instructorDeleteActiveVersion = await SendJsonAsync(
            // Meneruskan `HttpMethod.Delete` (nilai delete) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Delete,
            // Meneruskan teks interpolasi `$”/api/v1/rulesets/{createdRuleset.RulesetId}/versions/{updatedRuleset.Version}”`; nilai ekspresi di dalam kurung
            // kurawal disisipkan saat program berjalan sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/rulesets/{createdRuleset.RulesetId}/versions/{updatedRuleset.Version}",
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen bernama `body`.
            body: null,
            // Meneruskan `instructorLogin.AccessToken` (nilai akses token) sebagai argumen ke `SendJsonAsync`.
            instructorLogin.AccessToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.UnprocessableEntity`,
        // `instructorDeleteActiveVersion.StatusCode`); pengujian gagal jika keduanya berbeda dalam Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        Assert.Equal(HttpStatusCode.UnprocessableEntity, instructorDeleteActiveVersion.StatusCode);

        // Menyiapkan variabel lokal `instructorDeleteDraftVersion` untuk nilai instruktur delete draft versi dengan hasil operasi asinkron memanggil
        // `SendJsonAsync` dengan `HttpMethod.Delete`, `$”/api/v1/rulesets/{createdRuleset.RulesetId}/versions/{updatedRulesetV3.Version}”`, `null`,
        // `instructorLogin.AccessToken`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi
        // nilai awal.
        var instructorDeleteDraftVersion = await SendJsonAsync(
            // Meneruskan `HttpMethod.Delete` (nilai delete) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Delete,
            // Meneruskan teks interpolasi `$”/api/v1/rulesets/{createdRuleset.RulesetId}/versions/{updatedRulesetV3.Version}”`; nilai ekspresi di dalam kurung
            // kurawal disisipkan saat program berjalan sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/rulesets/{createdRuleset.RulesetId}/versions/{updatedRulesetV3.Version}",
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen bernama `body`.
            body: null,
            // Meneruskan `instructorLogin.AccessToken` (nilai akses token) sebagai argumen ke `SendJsonAsync`.
            instructorLogin.AccessToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.NoContent`,
        // `instructorDeleteDraftVersion.StatusCode`); pengujian gagal jika keduanya berbeda dalam Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        Assert.Equal(HttpStatusCode.NoContent, instructorDeleteDraftVersion.StatusCode);

        // Menyiapkan variabel lokal `rulesetDetailAfterDeleteVersion` untuk nilai aturan detail after delete versi dengan hasil operasi asinkron memanggil
        // `SendJsonAsync` dengan `HttpMethod.Get`, `$”/api/v1/rulesets/{createdRuleset.RulesetId}”`, `null`, `instructorLogin.AccessToken`; await menunggu
        // hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var rulesetDetailAfterDeleteVersion = await SendJsonAsync(
            // Meneruskan `HttpMethod.Get` (nilai get) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Get,
            // Meneruskan teks interpolasi `$”/api/v1/rulesets/{createdRuleset.RulesetId}”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program
            // berjalan sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/rulesets/{createdRuleset.RulesetId}",
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen bernama `body`.
            body: null,
            // Meneruskan `instructorLogin.AccessToken` (nilai akses token) sebagai argumen ke `SendJsonAsync`.
            instructorLogin.AccessToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`,
        // `rulesetDetailAfterDeleteVersion.StatusCode`); pengujian gagal jika keduanya berbeda dalam Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        Assert.Equal(HttpStatusCode.OK, rulesetDetailAfterDeleteVersion.StatusCode);

        // Menyiapkan variabel lokal `rulesetDetailPayload` untuk nilai aturan detail payload dengan hasil operasi asinkron membaca tanpa argumen menjadi
        // objek bertipe sesuai kontrak JSON melalui `rulesetDetailAfterDeleteVersion.Content.ReadFromJsonAsync<RulesetDetailResponse>`; await menunggu
        // hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var rulesetDetailPayload = await rulesetDetailAfterDeleteVersion.Content.ReadFromJsonAsync<RulesetDetailResponse>();
        // Menjalankan pemeriksaan NotNull atas `rulesetDetailPayload` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        Assert.NotNull(rulesetDetailPayload);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `rulesetDetailPayload.Versions`, `v
        // => v.Version == updatedRulesetV3.Version` dalam Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        Assert.DoesNotContain(rulesetDetailPayload.Versions, v => v.Version == updatedRulesetV3.Version);

        // Menyiapkan variabel lokal `createSessionPayload` untuk nilai create sesi payload dengan objek anonim yang mengelompokkan session_name, mode,
        // ruleset_version_id sebagai satu nilai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var createSessionPayload = new
        // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        {
            // Menggunakan `session_name` (nilai sesi nama) sebagai bagian ekspresi yang sedang disusun dalam Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
            session_name = $"Session IT {suffix}",
            // Menggunakan `mode` (mode permainan yang menentukan kelompok aturan yang digunakan) sebagai bagian ekspresi yang sedang disusun dalam
            // Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
            mode = "PEMULA",
            // Menggunakan `ruleset_version_id` (nilai aturan versi identitas) sebagai bagian ekspresi yang sedang disusun dalam
            // Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
            ruleset_version_id = updatedRuleset.RulesetVersionId
        // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
        // Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        };

        // Menyiapkan variabel lokal `playerCreateSession` untuk nilai pemain create sesi dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Post`, `”/api/v1/sessions”`, `createSessionPayload`, `playerLogin.AccessToken`; await menunggu hasil tanpa memblokir thread selama
        // operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var playerCreateSession = await SendJsonAsync(
            // Meneruskan `HttpMethod.Post` (nilai post) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Post,
            // Meneruskan nilai literal `”/api/v1/sessions”` sebagai argumen ke `SendJsonAsync`.
            "/api/v1/sessions",
            // Meneruskan `createSessionPayload` (nilai create sesi payload) sebagai argumen ke `SendJsonAsync`.
            createSessionPayload,
            // Meneruskan `playerLogin.AccessToken` (nilai akses token) sebagai argumen ke `SendJsonAsync`.
            playerLogin.AccessToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.Forbidden`,
        // `playerCreateSession.StatusCode`); pengujian gagal jika keduanya berbeda dalam Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        Assert.Equal(HttpStatusCode.Forbidden, playerCreateSession.StatusCode);

        // Menyiapkan variabel lokal `instructorCreateSession` untuk nilai instruktur create sesi dengan hasil operasi asinkron memanggil `SendJsonAsync`
        // dengan `HttpMethod.Post`, `”/api/v1/sessions”`, `createSessionPayload`, `instructorLogin.AccessToken`; await menunggu hasil tanpa memblokir
        // thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var instructorCreateSession = await SendJsonAsync(
            // Meneruskan `HttpMethod.Post` (nilai post) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Post,
            // Meneruskan nilai literal `”/api/v1/sessions”` sebagai argumen ke `SendJsonAsync`.
            "/api/v1/sessions",
            // Meneruskan `createSessionPayload` (nilai create sesi payload) sebagai argumen ke `SendJsonAsync`.
            createSessionPayload,
            // Meneruskan `instructorLogin.AccessToken` (nilai akses token) sebagai argumen ke `SendJsonAsync`.
            instructorLogin.AccessToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.Created`,
        // `instructorCreateSession.StatusCode`); pengujian gagal jika keduanya berbeda dalam Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        Assert.Equal(HttpStatusCode.Created, instructorCreateSession.StatusCode);

        // Menyiapkan variabel lokal `createdSession` untuk nilai created sesi dengan hasil operasi asinkron membaca tanpa argumen menjadi objek bertipe
        // sesuai kontrak JSON melalui `instructorCreateSession.Content.ReadFromJsonAsync<CreateSessionResponse>`; await menunggu hasil tanpa memblokir
        // thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var createdSession = await instructorCreateSession.Content.ReadFromJsonAsync<CreateSessionResponse>();
        // Menjalankan pemeriksaan NotNull atas `createdSession` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        Assert.NotNull(createdSession);
        // Menjalankan pemeriksaan hasil dengan `Assert.NotEqual` menggunakan `Guid.Empty`, `createdSession.SessionId`; ketidaksesuaian dengan ekspektasi
        // membuat pengujian gagal dalam Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        Assert.NotEqual(Guid.Empty, createdSession.SessionId);

        // Menyiapkan variabel lokal `extraPlayerOneUsername` untuk nilai extra pemain one username dengan teks interpolasi `$”it_player_extra1_{suffix}”`;
        // nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var extraPlayerOneUsername = $"it_player_extra1_{suffix}";
        // Menyiapkan variabel lokal `extraPlayerTwoUsername` untuk nilai extra pemain two username dengan teks interpolasi `$”it_player_extra2_{suffix}”`;
        // nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var extraPlayerTwoUsername = $"it_player_extra2_{suffix}";
        // Menjalankan hasil operasi asinkron memanggil `CreatePlayerAsync` dengan `instructorLogin.AccessToken`, `$”Player Extra 1 {suffix}”`,
        // `extraPlayerOneUsername`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        await CreatePlayerAsync(instructorLogin.AccessToken, $"Player Extra 1 {suffix}", extraPlayerOneUsername);
        // Menjalankan hasil operasi asinkron memanggil `CreatePlayerAsync` dengan `instructorLogin.AccessToken`, `$”Player Extra 2 {suffix}”`,
        // `extraPlayerTwoUsername`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        await CreatePlayerAsync(instructorLogin.AccessToken, $"Player Extra 2 {suffix}", extraPlayerTwoUsername);

        // Mengulangi setiap elemen `new[] { new { username = playerUsername, player_order_no = 1 }, new { username = extraPlayerOneUsername,
        // player_order_no = 2 }, new { username = extraPlayerTwoUsername, player...`; elemen saat ini disimpan sebagai `assignment` bertipe `var` untuk
        // diproses oleh badan loop dalam Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        foreach (var assignment in new[]
                 // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                 // Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
                 {
                     // Menggunakan objek anonim yang mengelompokkan username, player_order_no sebagai satu nilai sebagai bagian ekspresi yang sedang disusun dalam
                     // Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
                     new { username = playerUsername, player_order_no = 1 },
                     // Menggunakan objek anonim yang mengelompokkan username, player_order_no sebagai satu nilai sebagai bagian ekspresi yang sedang disusun dalam
                     // Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
                     new { username = extraPlayerOneUsername, player_order_no = 2 },
                     // Menggunakan objek anonim yang mengelompokkan username, player_order_no sebagai satu nilai sebagai bagian ekspresi yang sedang disusun dalam
                     // Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
                     new { username = extraPlayerTwoUsername, player_order_no = 3 }
                 // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
                 // Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
                 })
        // Membuka scope loop setiap assignment dari `new[] { new { username = playerUsername, player_order_no = 1 }, new { username =
        // extraPlayerOneUsername, player_order_no = 2 }, new { username = extraPlayerTwoUsername, player...`; pernyataan/deklarasi berikut berada di dalam
        // batas blok ini dalam Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        {
            // Menyiapkan variabel lokal `addPlayerResponse` untuk nilai add pemain respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
            // `HttpMethod.Post`, `$”/api/v1/sessions/{createdSession.SessionId}/players”`, `assignment`, `instructorLogin.AccessToken`; await menunggu hasil
            // tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var addPlayerResponse = await SendJsonAsync(
                // Meneruskan `HttpMethod.Post` (nilai post) sebagai argumen ke `SendJsonAsync`.
                HttpMethod.Post,
                // Meneruskan teks interpolasi `$”/api/v1/sessions/{createdSession.SessionId}/players”`; nilai ekspresi di dalam kurung kurawal disisipkan saat
                // program berjalan sebagai argumen ke `SendJsonAsync`.
                $"/api/v1/sessions/{createdSession.SessionId}/players",
                // Meneruskan `assignment` (nilai assignment) sebagai argumen ke `SendJsonAsync`.
                assignment,
                // Meneruskan `instructorLogin.AccessToken` (nilai akses token) sebagai argumen ke `SendJsonAsync`.
                instructorLogin.AccessToken);
            // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`,
            // `addPlayerResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
            Assert.Equal(HttpStatusCode.OK, addPlayerResponse.StatusCode);
        // Menutup scope loop setiap assignment dari `new[] { new { username = playerUsername, player_order_no = 1 }, new { username =
        // extraPlayerOneUsername, player_order_no = 2 }, new { username = extraPlayerTwoUsername, player...`; bagian berikut berada di luar batas blok
        // tersebut dalam Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        }

        // Menyiapkan variabel lokal `sessionPlayersResponse` untuk nilai sesi pemain respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Get`, `$”/api/v1/sessions/{createdSession.SessionId}/players”`, `null`, `playerLogin.AccessToken`; await menunggu hasil tanpa
        // memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var sessionPlayersResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Get` (nilai get) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Get,
            // Meneruskan teks interpolasi `$”/api/v1/sessions/{createdSession.SessionId}/players”`; nilai ekspresi di dalam kurung kurawal disisipkan saat
            // program berjalan sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/sessions/{createdSession.SessionId}/players",
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen bernama `body`.
            body: null,
            // Meneruskan `playerLogin.AccessToken` (nilai akses token) sebagai argumen ke `SendJsonAsync`.
            playerLogin.AccessToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`,
        // `sessionPlayersResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        Assert.Equal(HttpStatusCode.OK, sessionPlayersResponse.StatusCode);

        // Menyiapkan variabel lokal `sessionPlayers` untuk nilai sesi pemain dengan hasil operasi asinkron membaca tanpa argumen menjadi objek bertipe
        // sesuai kontrak JSON melalui `sessionPlayersResponse.Content.ReadFromJsonAsync<SessionPlayerListResponse>`; await menunggu hasil tanpa memblokir
        // thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var sessionPlayers = await sessionPlayersResponse.Content.ReadFromJsonAsync<SessionPlayerListResponse>();
        // Menjalankan pemeriksaan NotNull atas `sessionPlayers` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        Assert.NotNull(sessionPlayers);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`3`, `sessionPlayers.Items.Count`); pengujian
        // gagal jika keduanya berbeda dalam Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        Assert.Equal(3, sessionPlayers.Items.Count);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`new[] { 1, 2, 3 }`,
        // `sessionPlayers.Items.Select(item => item.PlayerOrder)`); pengujian gagal jika keduanya berbeda dalam Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        Assert.Equal(new[] { 1, 2, 3 }, sessionPlayers.Items.Select(item => item.PlayerOrder));
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `sessionPlayers.Items`, `item =>
        // item.DisplayName.Contains(suffix, StringComparison.Ordinal)` dalam Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        Assert.Contains(sessionPlayers.Items, item => item.DisplayName.Contains(suffix, StringComparison.Ordinal));

        // Menyiapkan variabel lokal `unrelatedSessionPlayersResponse` untuk nilai unrelated sesi pemain respons dengan hasil operasi asinkron memanggil
        // `SendJsonAsync` dengan `HttpMethod.Get`, `$”/api/v1/sessions/{createdSession.SessionId}/players”`, `null`, `unrelatedPlayerLogin.AccessToken`;
        // await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var unrelatedSessionPlayersResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Get` (nilai get) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Get,
            // Meneruskan teks interpolasi `$”/api/v1/sessions/{createdSession.SessionId}/players”`; nilai ekspresi di dalam kurung kurawal disisipkan saat
            // program berjalan sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/sessions/{createdSession.SessionId}/players",
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen bernama `body`.
            body: null,
            // Meneruskan `unrelatedPlayerLogin.AccessToken` (nilai akses token) sebagai argumen ke `SendJsonAsync`.
            unrelatedPlayerLogin.AccessToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.Forbidden`,
        // `unrelatedSessionPlayersResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        Assert.Equal(HttpStatusCode.Forbidden, unrelatedSessionPlayersResponse.StatusCode);

        // Menyiapkan variabel lokal `playerStartSession` untuk nilai pemain start sesi dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Post`, `$”/api/v1/sessions/{createdSession.SessionId}/start”`, `null`, `playerLogin.AccessToken`; await menunggu hasil tanpa
        // memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var playerStartSession = await SendJsonAsync(
            // Meneruskan `HttpMethod.Post` (nilai post) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Post,
            // Meneruskan teks interpolasi `$”/api/v1/sessions/{createdSession.SessionId}/start”`; nilai ekspresi di dalam kurung kurawal disisipkan saat
            // program berjalan sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/sessions/{createdSession.SessionId}/start",
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen bernama `body`.
            body: null,
            // Meneruskan `playerLogin.AccessToken` (nilai akses token) sebagai argumen ke `SendJsonAsync`.
            playerLogin.AccessToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.Forbidden`,
        // `playerStartSession.StatusCode`); pengujian gagal jika keduanya berbeda dalam Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        Assert.Equal(HttpStatusCode.Forbidden, playerStartSession.StatusCode);

        // Menyiapkan variabel lokal `saveSetupResponse` untuk nilai save setup respons dengan hasil operasi asinkron memanggil
        // `SessionSetupTestHelper.SaveAsync` dengan `_client`, `instructorLogin.AccessToken`, `createdSession.SessionId`,
        // `BuildRulesetDefinition(startingCash: 21)`, `TestContext.Current.CancellationToken`; await menunggu hasil tanpa memblokir thread selama operasi
        // belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var saveSetupResponse = await SessionSetupTestHelper.SaveAsync(
            // Meneruskan `_client` (nilai client) sebagai argumen ke `SessionSetupTestHelper.SaveAsync`.
            _client,
            // Meneruskan `instructorLogin.AccessToken` (nilai akses token) sebagai argumen ke `SessionSetupTestHelper.SaveAsync`.
            instructorLogin.AccessToken,
            // Meneruskan `createdSession.SessionId` (identitas unik sesi permainan yang menjadi batas data operasi ini) sebagai argumen ke
            // `SessionSetupTestHelper.SaveAsync`.
            createdSession.SessionId,
            // Meneruskan memanggil `BuildRulesetDefinition` dengan `21` sebagai argumen ke `SessionSetupTestHelper.SaveAsync`; Meneruskan nilai literal `21`
            // sebagai argumen bernama `startingCash`.
            BuildRulesetDefinition(startingCash: 21),
            // Meneruskan `TestContext.Current.CancellationToken` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
            // aplikasi berhenti) sebagai argumen ke `SessionSetupTestHelper.SaveAsync`.
            TestContext.Current.CancellationToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.Created`,
        // `saveSetupResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        Assert.Equal(HttpStatusCode.Created, saveSetupResponse.StatusCode);

        // Menyiapkan variabel lokal `instructorStartSession` untuk nilai instruktur start sesi dengan hasil operasi asinkron memanggil `SendJsonAsync`
        // dengan `HttpMethod.Post`, `$”/api/v1/sessions/{createdSession.SessionId}/start”`, `null`, `instructorLogin.AccessToken`; await menunggu hasil
        // tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var instructorStartSession = await SendJsonAsync(
            // Meneruskan `HttpMethod.Post` (nilai post) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Post,
            // Meneruskan teks interpolasi `$”/api/v1/sessions/{createdSession.SessionId}/start”`; nilai ekspresi di dalam kurung kurawal disisipkan saat
            // program berjalan sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/sessions/{createdSession.SessionId}/start",
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen bernama `body`.
            body: null,
            // Meneruskan `instructorLogin.AccessToken` (nilai akses token) sebagai argumen ke `SendJsonAsync`.
            instructorLogin.AccessToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`,
        // `instructorStartSession.StatusCode`); pengujian gagal jika keduanya berbeda dalam Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        Assert.Equal(HttpStatusCode.OK, instructorStartSession.StatusCode);

        // Menyiapkan variabel lokal `startResponse` untuk nilai start respons dengan hasil operasi asinkron membaca tanpa argumen menjadi objek bertipe
        // sesuai kontrak JSON melalui `instructorStartSession.Content.ReadFromJsonAsync<SessionStatusResponse>`; await menunggu hasil tanpa memblokir
        // thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var startResponse = await instructorStartSession.Content.ReadFromJsonAsync<SessionStatusResponse>();
        // Menjalankan pemeriksaan NotNull atas `startResponse` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        Assert.NotNull(startResponse);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”STARTED”`, `startResponse.Status`); pengujian
        // gagal jika keduanya berbeda dalam Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
        Assert.Equal("STARTED", startResponse.Status);
    // Menutup scope metode Auth_Rbac_And_RulesetFlow_Work_EndToEnd; bagian berikut berada di luar batas blok tersebut dalam
    // Auth_Rbac_And_RulesetFlow_Work_EndToEnd.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `DefaultRulesets_AreReadonly_AndExposeLockMetadata` dengan hasil bertipe `Task`; operasi ini menangani bawaan aturan are
    // readonly dan expose lock metadata. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task.
    public async Task DefaultRulesets_AreReadonly_AndExposeLockMetadata()
    // Membuka scope metode DefaultRulesets_AreReadonly_AndExposeLockMetadata; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // DefaultRulesets_AreReadonly_AndExposeLockMetadata.
    {
        // Menyiapkan variabel lokal `suffix` untuk nilai suffix dengan `Guid.NewGuid().ToString(”N”)[..8]`, yaitu elemen koleksi yang dipilih melalui
        // indeks atau kunci tersebut. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var suffix = Guid.NewGuid().ToString("N")[..8];
        // Menyiapkan variabel lokal `instructor` untuk nilai instruktur dengan hasil operasi asinkron memanggil `RegisterAsync` dengan
        // `$”it_default_guard_{suffix}”`, `”IntegrationInstructorPass!123”`, `”INSTRUCTOR”`; await menunggu hasil tanpa memblokir thread selama operasi
        // belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var instructor = await RegisterAsync($"it_default_guard_{suffix}", "IntegrationInstructorPass!123", "INSTRUCTOR");

        // Menyiapkan variabel lokal `listResponse` untuk nilai daftar respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Get`, `”/api/v1/rulesets”`, `null`, `instructor.AccessToken`; await menunggu hasil tanpa memblokir thread selama operasi belum
        // selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var listResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Get` (nilai get) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Get,
            // Meneruskan nilai literal `”/api/v1/rulesets”` sebagai argumen ke `SendJsonAsync`.
            "/api/v1/rulesets",
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen bernama `body`.
            body: null,
            // Meneruskan `instructor.AccessToken` (nilai akses token) sebagai argumen ke `SendJsonAsync`.
            instructor.AccessToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`,
        // `listResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam DefaultRulesets_AreReadonly_AndExposeLockMetadata.
        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);

        // Menyiapkan variabel lokal `listDocument` untuk nilai daftar document dengan memanggil `JsonDocument.Parse` dengan `await
        // listResponse.Content.ReadAsStringAsync()`. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat
        // scope berakhir.
        using var listDocument = JsonDocument.Parse(await listResponse.Content.ReadAsStringAsync());
        // Menyiapkan variabel lokal `defaultItem` untuk nilai bawaan elemen dengan mengambil elemen pertama `listDocument.RootElement.GetProperty(”items”)
        // .EnumerateArray()` sesuai `item => item.GetProperty(”is_default”).GetBoolean()`; urutan tanpa kecocokan menyebabkan exception. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var defaultItem = listDocument.RootElement.GetProperty("items")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .EnumerateArray() dalam DefaultRulesets_AreReadonly_AndExposeLockMetadata;
            // token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .EnumerateArray()
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .First(item => item.GetProperty(”is_default”).GetBoolean()); dalam
            // DefaultRulesets_AreReadonly_AndExposeLockMetadata; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .First(item => item.GetProperty("is_default").GetBoolean());
        // Menjalankan pemeriksaan bahwa `defaultItem.GetProperty(”is_locked_by_session”).GetBoolean()` bernilai salah; pengujian gagal jika kondisi justru
        // terpenuhi dalam DefaultRulesets_AreReadonly_AndExposeLockMetadata.
        Assert.False(defaultItem.GetProperty("is_locked_by_session").GetBoolean());

        // Menyiapkan variabel lokal `defaultRulesetId` untuk nilai bawaan aturan identitas dengan memanggil `defaultItem.GetProperty(”ruleset_id”).GetGuid`
        // dengan tanpa argumen. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var defaultRulesetId = defaultItem.GetProperty("ruleset_id").GetGuid();
        // Menyiapkan variabel lokal `defaultVersion` untuk nilai bawaan versi dengan memanggil `defaultItem.GetProperty(”latest_version”).GetInt32` dengan
        // tanpa argumen. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var defaultVersion = defaultItem.GetProperty("latest_version").GetInt32();

        // Menyiapkan variabel lokal `detailResponse` untuk nilai detail respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Get`, `$”/api/v1/rulesets/{defaultRulesetId}”`, `null`, `instructor.AccessToken`; await menunggu hasil tanpa memblokir thread selama
        // operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var detailResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Get` (nilai get) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Get,
            // Meneruskan teks interpolasi `$”/api/v1/rulesets/{defaultRulesetId}”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan
            // sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/rulesets/{defaultRulesetId}",
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen bernama `body`.
            body: null,
            // Meneruskan `instructor.AccessToken` (nilai akses token) sebagai argumen ke `SendJsonAsync`.
            instructor.AccessToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`,
        // `detailResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam DefaultRulesets_AreReadonly_AndExposeLockMetadata.
        Assert.Equal(HttpStatusCode.OK, detailResponse.StatusCode);

        // Menyiapkan variabel lokal `detailDocument` untuk nilai detail document dengan memanggil `JsonDocument.Parse` dengan `await
        // detailResponse.Content.ReadAsStringAsync()`. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis
        // saat scope berakhir.
        using var detailDocument = JsonDocument.Parse(await detailResponse.Content.ReadAsStringAsync());
        // Menjalankan pemeriksaan bahwa `detailDocument.RootElement.GetProperty(”is_default”).GetBoolean()` bernilai benar; pengujian gagal jika kondisi
        // tidak terpenuhi dalam DefaultRulesets_AreReadonly_AndExposeLockMetadata.
        Assert.True(detailDocument.RootElement.GetProperty("is_default").GetBoolean());
        // Menjalankan pemeriksaan bahwa `detailDocument.RootElement.GetProperty(”is_locked_by_session”).GetBoolean()` bernilai salah; pengujian gagal jika
        // kondisi justru terpenuhi dalam DefaultRulesets_AreReadonly_AndExposeLockMetadata.
        Assert.False(detailDocument.RootElement.GetProperty("is_locked_by_session").GetBoolean());

        // Menyiapkan variabel lokal `updateResponse` untuk nilai update respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Put`, `$”/api/v1/rulesets/{defaultRulesetId}”`, `new { name = ”Attempt default update”, description = ”Should be rejected”,
        // definition = BuildRulesetDefinition(startingCash: 31) }`, `instructor.AccessToken`; await menunggu hasil tanpa memblokir thread selama operasi
        // belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var updateResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Put` (nilai put) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Put,
            // Meneruskan teks interpolasi `$”/api/v1/rulesets/{defaultRulesetId}”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan
            // sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/rulesets/{defaultRulesetId}",
            // Meneruskan objek anonim yang mengelompokkan name, description, definition sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // DefaultRulesets_AreReadonly_AndExposeLockMetadata.
            {
                // Meneruskan objek anonim yang mengelompokkan name, description, definition sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                name = "Attempt default update",
                // Meneruskan objek anonim yang mengelompokkan name, description, definition sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                description = "Should be rejected",
                // Meneruskan nilai literal `31` sebagai argumen bernama `startingCash`.
                definition = BuildRulesetDefinition(startingCash: 31)
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
            // DefaultRulesets_AreReadonly_AndExposeLockMetadata.
            },
            // Meneruskan `instructor.AccessToken` (nilai akses token) sebagai argumen ke `SendJsonAsync`.
            instructor.AccessToken);
        // Menjalankan hasil operasi asinkron memanggil `AssertDomainRuleViolationAsync` dengan `updateResponse`; await menunggu hasil tanpa memblokir
        // thread selama operasi belum selesai dalam DefaultRulesets_AreReadonly_AndExposeLockMetadata.
        await AssertDomainRuleViolationAsync(updateResponse);

        // Menyiapkan variabel lokal `activateResponse` untuk nilai activate respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Post`, `$”/api/v1/rulesets/{defaultRulesetId}/versions/{defaultVersion}/activate”`, `null`, `instructor.AccessToken`; await menunggu
        // hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var activateResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Post` (nilai post) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Post,
            // Meneruskan teks interpolasi `$”/api/v1/rulesets/{defaultRulesetId}/versions/{defaultVersion}/activate”`; nilai ekspresi di dalam kurung kurawal
            // disisipkan saat program berjalan sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/rulesets/{defaultRulesetId}/versions/{defaultVersion}/activate",
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen bernama `body`.
            body: null,
            // Meneruskan `instructor.AccessToken` (nilai akses token) sebagai argumen ke `SendJsonAsync`.
            instructor.AccessToken);
        // Menjalankan hasil operasi asinkron memanggil `AssertDomainRuleViolationAsync` dengan `activateResponse`; await menunggu hasil tanpa memblokir
        // thread selama operasi belum selesai dalam DefaultRulesets_AreReadonly_AndExposeLockMetadata.
        await AssertDomainRuleViolationAsync(activateResponse);

        // Menyiapkan variabel lokal `deleteVersionResponse` untuk nilai delete versi respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Delete`, `$”/api/v1/rulesets/{defaultRulesetId}/versions/{defaultVersion}”`, `null`, `instructor.AccessToken`; await menunggu hasil
        // tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var deleteVersionResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Delete` (nilai delete) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Delete,
            // Meneruskan teks interpolasi `$”/api/v1/rulesets/{defaultRulesetId}/versions/{defaultVersion}”`; nilai ekspresi di dalam kurung kurawal disisipkan
            // saat program berjalan sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/rulesets/{defaultRulesetId}/versions/{defaultVersion}",
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen bernama `body`.
            body: null,
            // Meneruskan `instructor.AccessToken` (nilai akses token) sebagai argumen ke `SendJsonAsync`.
            instructor.AccessToken);
        // Menjalankan hasil operasi asinkron memanggil `AssertDomainRuleViolationAsync` dengan `deleteVersionResponse`; await menunggu hasil tanpa
        // memblokir thread selama operasi belum selesai dalam DefaultRulesets_AreReadonly_AndExposeLockMetadata.
        await AssertDomainRuleViolationAsync(deleteVersionResponse);

        // Menyiapkan variabel lokal `deleteResponse` untuk nilai delete respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Delete`, `$”/api/v1/rulesets/{defaultRulesetId}”`, `null`, `instructor.AccessToken`; await menunggu hasil tanpa memblokir thread
        // selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var deleteResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Delete` (nilai delete) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Delete,
            // Meneruskan teks interpolasi `$”/api/v1/rulesets/{defaultRulesetId}”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan
            // sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/rulesets/{defaultRulesetId}",
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen bernama `body`.
            body: null,
            // Meneruskan `instructor.AccessToken` (nilai akses token) sebagai argumen ke `SendJsonAsync`.
            instructor.AccessToken);
        // Menjalankan hasil operasi asinkron memanggil `AssertDomainRuleViolationAsync` dengan `deleteResponse`; await menunggu hasil tanpa memblokir
        // thread selama operasi belum selesai dalam DefaultRulesets_AreReadonly_AndExposeLockMetadata.
        await AssertDomainRuleViolationAsync(deleteResponse);
    // Menutup scope metode DefaultRulesets_AreReadonly_AndExposeLockMetadata; bagian berikut berada di luar batas blok tersebut dalam
    // DefaultRulesets_AreReadonly_AndExposeLockMetadata.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `CustomRuleset_UsedOnlyByCreatedSession_RemainsMutable` dengan hasil bertipe `Task`; operasi ini menangani custom aturan
    // used only berdasarkan created sesi remains mutable. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian
    // melalui Task.
    public async Task CustomRuleset_UsedOnlyByCreatedSession_RemainsMutable()
    // Membuka scope metode CustomRuleset_UsedOnlyByCreatedSession_RemainsMutable; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // CustomRuleset_UsedOnlyByCreatedSession_RemainsMutable.
    {
        // Menyiapkan variabel lokal `suffix` untuk nilai suffix dengan `Guid.NewGuid().ToString(”N”)[..8]`, yaitu elemen koleksi yang dipilih melalui
        // indeks atau kunci tersebut. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var suffix = Guid.NewGuid().ToString("N")[..8];
        // Menyiapkan variabel lokal `instructor` untuk nilai instruktur dengan hasil operasi asinkron memanggil `RegisterAsync` dengan
        // `$”it_created_guard_{suffix}”`, `”IntegrationInstructorPass!123”`, `”INSTRUCTOR”`; await menunggu hasil tanpa memblokir thread selama operasi
        // belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var instructor = await RegisterAsync($"it_created_guard_{suffix}", "IntegrationInstructorPass!123", "INSTRUCTOR");
        // Menyiapkan variabel lokal `createdRuleset` untuk nilai created aturan dengan hasil operasi asinkron memanggil `CreateRulesetAsync` dengan
        // `instructor.AccessToken`, `suffix`, `41`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var createdRuleset = await CreateRulesetAsync(instructor.AccessToken, suffix, startingCash: 41);
        // Menjalankan hasil operasi asinkron memanggil `CreateSessionAsync` dengan `instructor.AccessToken`, `suffix`, `createdRuleset.RulesetVersionId`;
        // await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam CustomRuleset_UsedOnlyByCreatedSession_RemainsMutable.
        await CreateSessionAsync(instructor.AccessToken, suffix, createdRuleset.RulesetVersionId);

        // Menyiapkan variabel lokal `detailBeforeUpdate` untuk nilai detail before update dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Get`, `$”/api/v1/rulesets/{createdRuleset.RulesetId}”`, `null`, `instructor.AccessToken`; await menunggu hasil tanpa memblokir thread
        // selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var detailBeforeUpdate = await SendJsonAsync(
            // Meneruskan `HttpMethod.Get` (nilai get) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Get,
            // Meneruskan teks interpolasi `$”/api/v1/rulesets/{createdRuleset.RulesetId}”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program
            // berjalan sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/rulesets/{createdRuleset.RulesetId}",
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen bernama `body`.
            body: null,
            // Meneruskan `instructor.AccessToken` (nilai akses token) sebagai argumen ke `SendJsonAsync`.
            instructor.AccessToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`,
        // `detailBeforeUpdate.StatusCode`); pengujian gagal jika keduanya berbeda dalam CustomRuleset_UsedOnlyByCreatedSession_RemainsMutable.
        Assert.Equal(HttpStatusCode.OK, detailBeforeUpdate.StatusCode);
        // Membatasi masa pakai `var detailDocument = JsonDocument.Parse(await detailBeforeUpdate.Content.ReadAsStringAsync())` pada blok using; sumber daya
        // dilepas ketika blok berakhir melalui Dispose.
        using (var detailDocument = JsonDocument.Parse(await detailBeforeUpdate.Content.ReadAsStringAsync()))
        // Membuka scope scope pemakaian sumber daya using; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // CustomRuleset_UsedOnlyByCreatedSession_RemainsMutable.
        {
            // Menjalankan pemeriksaan bahwa `detailDocument.RootElement.GetProperty(”is_default”).GetBoolean()` bernilai salah; pengujian gagal jika kondisi
            // justru terpenuhi dalam CustomRuleset_UsedOnlyByCreatedSession_RemainsMutable.
            Assert.False(detailDocument.RootElement.GetProperty("is_default").GetBoolean());
            // Menjalankan pemeriksaan bahwa `detailDocument.RootElement.GetProperty(”is_locked_by_session”).GetBoolean()` bernilai salah; pengujian gagal jika
            // kondisi justru terpenuhi dalam CustomRuleset_UsedOnlyByCreatedSession_RemainsMutable.
            Assert.False(detailDocument.RootElement.GetProperty("is_locked_by_session").GetBoolean());
        // Menutup scope scope pemakaian sumber daya using; bagian berikut berada di luar batas blok tersebut dalam
        // CustomRuleset_UsedOnlyByCreatedSession_RemainsMutable.
        }

        // Menyiapkan variabel lokal `updateResponse` untuk nilai update respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Put`, `$”/api/v1/rulesets/{createdRuleset.RulesetId}”`, `new { name = $”Ruleset CREATED Mutable {suffix} V2”, description = ”CREATED
        // sessions do not lock rulesets”, definition = BuildRulesetDefinition(startingCash: 42) }`, `instructor.AccessToken`; await menunggu hasil tanpa
        // memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var updateResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Put` (nilai put) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Put,
            // Meneruskan teks interpolasi `$”/api/v1/rulesets/{createdRuleset.RulesetId}”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program
            // berjalan sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/rulesets/{createdRuleset.RulesetId}",
            // Meneruskan objek anonim yang mengelompokkan name, description, definition sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // CustomRuleset_UsedOnlyByCreatedSession_RemainsMutable.
            {
                // Meneruskan objek anonim yang mengelompokkan name, description, definition sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                name = $"Ruleset CREATED Mutable {suffix} V2",
                // Meneruskan objek anonim yang mengelompokkan name, description, definition sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                description = "CREATED sessions do not lock rulesets",
                // Meneruskan nilai literal `42` sebagai argumen bernama `startingCash`.
                definition = BuildRulesetDefinition(startingCash: 42)
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
            // CustomRuleset_UsedOnlyByCreatedSession_RemainsMutable.
            },
            // Meneruskan `instructor.AccessToken` (nilai akses token) sebagai argumen ke `SendJsonAsync`.
            instructor.AccessToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`,
        // `updateResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam CustomRuleset_UsedOnlyByCreatedSession_RemainsMutable.
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        // Menyiapkan variabel lokal `deleteResponse` untuk nilai delete respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Delete`, `$”/api/v1/rulesets/{createdRuleset.RulesetId}”`, `null`, `instructor.AccessToken`; await menunggu hasil tanpa memblokir
        // thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var deleteResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Delete` (nilai delete) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Delete,
            // Meneruskan teks interpolasi `$”/api/v1/rulesets/{createdRuleset.RulesetId}”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program
            // berjalan sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/rulesets/{createdRuleset.RulesetId}",
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen bernama `body`.
            body: null,
            // Meneruskan `instructor.AccessToken` (nilai akses token) sebagai argumen ke `SendJsonAsync`.
            instructor.AccessToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.NoContent`,
        // `deleteResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam CustomRuleset_UsedOnlyByCreatedSession_RemainsMutable.
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
    // Menutup scope metode CustomRuleset_UsedOnlyByCreatedSession_RemainsMutable; bagian berikut berada di luar batas blok tersebut dalam
    // CustomRuleset_UsedOnlyByCreatedSession_RemainsMutable.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `CustomRuleset_UsedByStartedOrEndedSession_IsReadonly` dengan hasil bertipe `Task`; operasi ini menangani custom aturan
    // used berdasarkan started atau ended sesi berstatus readonly. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan
    // penyelesaian melalui Task.
    public async Task CustomRuleset_UsedByStartedOrEndedSession_IsReadonly()
    // Membuka scope metode CustomRuleset_UsedByStartedOrEndedSession_IsReadonly; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // CustomRuleset_UsedByStartedOrEndedSession_IsReadonly.
    {
        // Menyiapkan variabel lokal `suffix` untuk nilai suffix dengan `Guid.NewGuid().ToString(”N”)[..8]`, yaitu elemen koleksi yang dipilih melalui
        // indeks atau kunci tersebut. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var suffix = Guid.NewGuid().ToString("N")[..8];
        // Menyiapkan variabel lokal `instructor` untuk nilai instruktur dengan hasil operasi asinkron memanggil `RegisterAsync` dengan
        // `$”it_locked_guard_{suffix}”`, `”IntegrationInstructorPass!123”`, `”INSTRUCTOR”`; await menunggu hasil tanpa memblokir thread selama operasi
        // belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var instructor = await RegisterAsync($"it_locked_guard_{suffix}", "IntegrationInstructorPass!123", "INSTRUCTOR");
        // Menyiapkan variabel lokal `createdRuleset` untuk nilai created aturan dengan hasil operasi asinkron memanggil `CreateRulesetAsync` dengan
        // `instructor.AccessToken`, `suffix`, `51`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var createdRuleset = await CreateRulesetAsync(instructor.AccessToken, suffix, startingCash: 51);
        // Menyiapkan variabel lokal `createdSession` untuk nilai created sesi dengan hasil operasi asinkron memanggil `CreateSessionAsync` dengan
        // `instructor.AccessToken`, `suffix`, `createdRuleset.RulesetVersionId`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai.
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var createdSession = await CreateSessionAsync(instructor.AccessToken, suffix, createdRuleset.RulesetVersionId);

        // Menjalankan hasil operasi asinkron memanggil `AddPlayersForStartAsync` dengan `instructor.AccessToken`, `createdSession.SessionId`, `suffix`;
        // await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam CustomRuleset_UsedByStartedOrEndedSession_IsReadonly.
        await AddPlayersForStartAsync(instructor.AccessToken, createdSession.SessionId, suffix);

        // Menyiapkan variabel lokal `saveSetupResponse` untuk nilai save setup respons dengan hasil operasi asinkron memanggil
        // `SessionSetupTestHelper.SaveAsync` dengan `_client`, `instructor.AccessToken`, `createdSession.SessionId`, `BuildRulesetDefinition(startingCash:
        // 51)`, `TestContext.Current.CancellationToken`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel
        // disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var saveSetupResponse = await SessionSetupTestHelper.SaveAsync(
            // Meneruskan `_client` (nilai client) sebagai argumen ke `SessionSetupTestHelper.SaveAsync`.
            _client,
            // Meneruskan `instructor.AccessToken` (nilai akses token) sebagai argumen ke `SessionSetupTestHelper.SaveAsync`.
            instructor.AccessToken,
            // Meneruskan `createdSession.SessionId` (identitas unik sesi permainan yang menjadi batas data operasi ini) sebagai argumen ke
            // `SessionSetupTestHelper.SaveAsync`.
            createdSession.SessionId,
            // Meneruskan memanggil `BuildRulesetDefinition` dengan `51` sebagai argumen ke `SessionSetupTestHelper.SaveAsync`; Meneruskan nilai literal `51`
            // sebagai argumen bernama `startingCash`.
            BuildRulesetDefinition(startingCash: 51),
            // Meneruskan `TestContext.Current.CancellationToken` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
            // aplikasi berhenti) sebagai argumen ke `SessionSetupTestHelper.SaveAsync`.
            TestContext.Current.CancellationToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.Created`,
        // `saveSetupResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam CustomRuleset_UsedByStartedOrEndedSession_IsReadonly.
        Assert.Equal(HttpStatusCode.Created, saveSetupResponse.StatusCode);

        // Menyiapkan variabel lokal `startResponse` untuk nilai start respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Post`, `$”/api/v1/sessions/{createdSession.SessionId}/start”`, `null`, `instructor.AccessToken`; await menunggu hasil tanpa memblokir
        // thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var startResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Post` (nilai post) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Post,
            // Meneruskan teks interpolasi `$”/api/v1/sessions/{createdSession.SessionId}/start”`; nilai ekspresi di dalam kurung kurawal disisipkan saat
            // program berjalan sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/sessions/{createdSession.SessionId}/start",
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen bernama `body`.
            body: null,
            // Meneruskan `instructor.AccessToken` (nilai akses token) sebagai argumen ke `SendJsonAsync`.
            instructor.AccessToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`,
        // `startResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam CustomRuleset_UsedByStartedOrEndedSession_IsReadonly.
        Assert.Equal(HttpStatusCode.OK, startResponse.StatusCode);

        // Menyiapkan variabel lokal `detailAfterStart` untuk nilai detail after start dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Get`, `$”/api/v1/rulesets/{createdRuleset.RulesetId}”`, `null`, `instructor.AccessToken`; await menunggu hasil tanpa memblokir thread
        // selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var detailAfterStart = await SendJsonAsync(
            // Meneruskan `HttpMethod.Get` (nilai get) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Get,
            // Meneruskan teks interpolasi `$”/api/v1/rulesets/{createdRuleset.RulesetId}”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program
            // berjalan sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/rulesets/{createdRuleset.RulesetId}",
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen bernama `body`.
            body: null,
            // Meneruskan `instructor.AccessToken` (nilai akses token) sebagai argumen ke `SendJsonAsync`.
            instructor.AccessToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`,
        // `detailAfterStart.StatusCode`); pengujian gagal jika keduanya berbeda dalam CustomRuleset_UsedByStartedOrEndedSession_IsReadonly.
        Assert.Equal(HttpStatusCode.OK, detailAfterStart.StatusCode);
        // Membatasi masa pakai `var detailDocument = JsonDocument.Parse(await detailAfterStart.Content.ReadAsStringAsync())` pada blok using; sumber daya
        // dilepas ketika blok berakhir melalui Dispose.
        using (var detailDocument = JsonDocument.Parse(await detailAfterStart.Content.ReadAsStringAsync()))
        // Membuka scope scope pemakaian sumber daya using; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // CustomRuleset_UsedByStartedOrEndedSession_IsReadonly.
        {
            // Menjalankan pemeriksaan bahwa `detailDocument.RootElement.GetProperty(”is_default”).GetBoolean()` bernilai salah; pengujian gagal jika kondisi
            // justru terpenuhi dalam CustomRuleset_UsedByStartedOrEndedSession_IsReadonly.
            Assert.False(detailDocument.RootElement.GetProperty("is_default").GetBoolean());
            // Menjalankan pemeriksaan bahwa `detailDocument.RootElement.GetProperty(”is_locked_by_session”).GetBoolean()` bernilai benar; pengujian gagal jika
            // kondisi tidak terpenuhi dalam CustomRuleset_UsedByStartedOrEndedSession_IsReadonly.
            Assert.True(detailDocument.RootElement.GetProperty("is_locked_by_session").GetBoolean());
        // Menutup scope scope pemakaian sumber daya using; bagian berikut berada di luar batas blok tersebut dalam
        // CustomRuleset_UsedByStartedOrEndedSession_IsReadonly.
        }

        // Menyiapkan variabel lokal `listAfterStart` untuk nilai daftar after start dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Get`, `”/api/v1/rulesets”`, `null`, `instructor.AccessToken`; await menunggu hasil tanpa memblokir thread selama operasi belum
        // selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var listAfterStart = await SendJsonAsync(
            // Meneruskan `HttpMethod.Get` (nilai get) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Get,
            // Meneruskan nilai literal `”/api/v1/rulesets”` sebagai argumen ke `SendJsonAsync`.
            "/api/v1/rulesets",
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen bernama `body`.
            body: null,
            // Meneruskan `instructor.AccessToken` (nilai akses token) sebagai argumen ke `SendJsonAsync`.
            instructor.AccessToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`,
        // `listAfterStart.StatusCode`); pengujian gagal jika keduanya berbeda dalam CustomRuleset_UsedByStartedOrEndedSession_IsReadonly.
        Assert.Equal(HttpStatusCode.OK, listAfterStart.StatusCode);
        // Membatasi masa pakai `var listDocument = JsonDocument.Parse(await listAfterStart.Content.ReadAsStringAsync())` pada blok using; sumber daya
        // dilepas ketika blok berakhir melalui Dispose.
        using (var listDocument = JsonDocument.Parse(await listAfterStart.Content.ReadAsStringAsync()))
        // Membuka scope scope pemakaian sumber daya using; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // CustomRuleset_UsedByStartedOrEndedSession_IsReadonly.
        {
            // Menyiapkan variabel lokal `lockedItem` untuk nilai locked elemen dengan mengambil elemen pertama `listDocument.RootElement.GetProperty(”items”)
            // .EnumerateArray()` sesuai `item => item.GetProperty(”ruleset_id”).GetGuid() == createdRuleset.RulesetId`; urutan tanpa kecocokan menyebabkan
            // exception. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var lockedItem = listDocument.RootElement.GetProperty("items")
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .EnumerateArray() dalam CustomRuleset_UsedByStartedOrEndedSession_IsReadonly;
                // token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
                .EnumerateArray()
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .First(item => item.GetProperty(”ruleset_id”).GetGuid() ==
                // createdRuleset.RulesetId); dalam CustomRuleset_UsedByStartedOrEndedSession_IsReadonly; token pada baris ini menyambungkan bagian kode sebelum dan
                // sesudahnya.
                .First(item => item.GetProperty("ruleset_id").GetGuid() == createdRuleset.RulesetId);
            // Menjalankan pemeriksaan bahwa `lockedItem.GetProperty(”is_locked_by_session”).GetBoolean()` bernilai benar; pengujian gagal jika kondisi tidak
            // terpenuhi dalam CustomRuleset_UsedByStartedOrEndedSession_IsReadonly.
            Assert.True(lockedItem.GetProperty("is_locked_by_session").GetBoolean());
        // Menutup scope scope pemakaian sumber daya using; bagian berikut berada di luar batas blok tersebut dalam
        // CustomRuleset_UsedByStartedOrEndedSession_IsReadonly.
        }

        // Menyiapkan variabel lokal `updateResponse` untuk nilai update respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Put`, `$”/api/v1/rulesets/{createdRuleset.RulesetId}”`, `new { name = $”Ruleset Locked {suffix} V2”, description = ”Should be
        // rejected”, definition = BuildRulesetDefinition(startingCash: 52) }`, `instructor.AccessToken`; await menunggu hasil tanpa memblokir thread selama
        // operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var updateResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Put` (nilai put) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Put,
            // Meneruskan teks interpolasi `$”/api/v1/rulesets/{createdRuleset.RulesetId}”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program
            // berjalan sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/rulesets/{createdRuleset.RulesetId}",
            // Meneruskan objek anonim yang mengelompokkan name, description, definition sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // CustomRuleset_UsedByStartedOrEndedSession_IsReadonly.
            {
                // Meneruskan objek anonim yang mengelompokkan name, description, definition sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                name = $"Ruleset Locked {suffix} V2",
                // Meneruskan objek anonim yang mengelompokkan name, description, definition sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                description = "Should be rejected",
                // Meneruskan nilai literal `52` sebagai argumen bernama `startingCash`.
                definition = BuildRulesetDefinition(startingCash: 52)
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
            // CustomRuleset_UsedByStartedOrEndedSession_IsReadonly.
            },
            // Meneruskan `instructor.AccessToken` (nilai akses token) sebagai argumen ke `SendJsonAsync`.
            instructor.AccessToken);
        // Menjalankan hasil operasi asinkron memanggil `AssertDomainRuleViolationAsync` dengan `updateResponse`; await menunggu hasil tanpa memblokir
        // thread selama operasi belum selesai dalam CustomRuleset_UsedByStartedOrEndedSession_IsReadonly.
        await AssertDomainRuleViolationAsync(updateResponse);

        // Menyiapkan variabel lokal `activateResponse` untuk nilai activate respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Post`, `$”/api/v1/rulesets/{createdRuleset.RulesetId}/versions/{createdRuleset.Version}/activate”`, `null`, `instructor.AccessToken`;
        // await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var activateResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Post` (nilai post) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Post,
            // Meneruskan teks interpolasi `$”/api/v1/rulesets/{createdRuleset.RulesetId}/versions/{createdRuleset.Version}/activate”`; nilai ekspresi di dalam
            // kurung kurawal disisipkan saat program berjalan sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/rulesets/{createdRuleset.RulesetId}/versions/{createdRuleset.Version}/activate",
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen bernama `body`.
            body: null,
            // Meneruskan `instructor.AccessToken` (nilai akses token) sebagai argumen ke `SendJsonAsync`.
            instructor.AccessToken);
        // Menjalankan hasil operasi asinkron memanggil `AssertDomainRuleViolationAsync` dengan `activateResponse`; await menunggu hasil tanpa memblokir
        // thread selama operasi belum selesai dalam CustomRuleset_UsedByStartedOrEndedSession_IsReadonly.
        await AssertDomainRuleViolationAsync(activateResponse);

        // Menyiapkan variabel lokal `deleteVersionResponse` untuk nilai delete versi respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Delete`, `$”/api/v1/rulesets/{createdRuleset.RulesetId}/versions/{createdRuleset.Version}”`, `null`, `instructor.AccessToken`; await
        // menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var deleteVersionResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Delete` (nilai delete) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Delete,
            // Meneruskan teks interpolasi `$”/api/v1/rulesets/{createdRuleset.RulesetId}/versions/{createdRuleset.Version}”`; nilai ekspresi di dalam kurung
            // kurawal disisipkan saat program berjalan sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/rulesets/{createdRuleset.RulesetId}/versions/{createdRuleset.Version}",
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen bernama `body`.
            body: null,
            // Meneruskan `instructor.AccessToken` (nilai akses token) sebagai argumen ke `SendJsonAsync`.
            instructor.AccessToken);
        // Menjalankan hasil operasi asinkron memanggil `AssertDomainRuleViolationAsync` dengan `deleteVersionResponse`; await menunggu hasil tanpa
        // memblokir thread selama operasi belum selesai dalam CustomRuleset_UsedByStartedOrEndedSession_IsReadonly.
        await AssertDomainRuleViolationAsync(deleteVersionResponse);

        // Menyiapkan variabel lokal `deleteResponse` untuk nilai delete respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Delete`, `$”/api/v1/rulesets/{createdRuleset.RulesetId}”`, `null`, `instructor.AccessToken`; await menunggu hasil tanpa memblokir
        // thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var deleteResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Delete` (nilai delete) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Delete,
            // Meneruskan teks interpolasi `$”/api/v1/rulesets/{createdRuleset.RulesetId}”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program
            // berjalan sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/rulesets/{createdRuleset.RulesetId}",
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen bernama `body`.
            body: null,
            // Meneruskan `instructor.AccessToken` (nilai akses token) sebagai argumen ke `SendJsonAsync`.
            instructor.AccessToken);
        // Menjalankan hasil operasi asinkron memanggil `AssertDomainRuleViolationAsync` dengan `deleteResponse`; await menunggu hasil tanpa memblokir
        // thread selama operasi belum selesai dalam CustomRuleset_UsedByStartedOrEndedSession_IsReadonly.
        await AssertDomainRuleViolationAsync(deleteResponse);

        // Menyiapkan variabel lokal `endResponse` untuk nilai end respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan `HttpMethod.Post`,
        // `$”/api/v1/sessions/{createdSession.SessionId}/end”`, `null`, `instructor.AccessToken`; await menunggu hasil tanpa memblokir thread selama
        // operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var endResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Post` (nilai post) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Post,
            // Meneruskan teks interpolasi `$”/api/v1/sessions/{createdSession.SessionId}/end”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program
            // berjalan sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/sessions/{createdSession.SessionId}/end",
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen bernama `body`.
            body: null,
            // Meneruskan `instructor.AccessToken` (nilai akses token) sebagai argumen ke `SendJsonAsync`.
            instructor.AccessToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`, `endResponse.StatusCode`);
        // pengujian gagal jika keduanya berbeda dalam CustomRuleset_UsedByStartedOrEndedSession_IsReadonly.
        Assert.Equal(HttpStatusCode.OK, endResponse.StatusCode);

        // Menyiapkan variabel lokal `detailAfterEnd` untuk nilai detail after end dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Get`, `$”/api/v1/rulesets/{createdRuleset.RulesetId}”`, `null`, `instructor.AccessToken`; await menunggu hasil tanpa memblokir thread
        // selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var detailAfterEnd = await SendJsonAsync(
            // Meneruskan `HttpMethod.Get` (nilai get) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Get,
            // Meneruskan teks interpolasi `$”/api/v1/rulesets/{createdRuleset.RulesetId}”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program
            // berjalan sebagai argumen ke `SendJsonAsync`.
            $"/api/v1/rulesets/{createdRuleset.RulesetId}",
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen bernama `body`.
            body: null,
            // Meneruskan `instructor.AccessToken` (nilai akses token) sebagai argumen ke `SendJsonAsync`.
            instructor.AccessToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`,
        // `detailAfterEnd.StatusCode`); pengujian gagal jika keduanya berbeda dalam CustomRuleset_UsedByStartedOrEndedSession_IsReadonly.
        Assert.Equal(HttpStatusCode.OK, detailAfterEnd.StatusCode);
        // Menyiapkan variabel lokal `endedDetailDocument` untuk nilai ended detail document dengan memanggil `JsonDocument.Parse` dengan `await
        // detailAfterEnd.Content.ReadAsStringAsync()`. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis
        // saat scope berakhir.
        using var endedDetailDocument = JsonDocument.Parse(await detailAfterEnd.Content.ReadAsStringAsync());
        // Menjalankan pemeriksaan bahwa `endedDetailDocument.RootElement.GetProperty(”is_locked_by_session”).GetBoolean()` bernilai benar; pengujian gagal
        // jika kondisi tidak terpenuhi dalam CustomRuleset_UsedByStartedOrEndedSession_IsReadonly.
        Assert.True(endedDetailDocument.RootElement.GetProperty("is_locked_by_session").GetBoolean());
    // Menutup scope metode CustomRuleset_UsedByStartedOrEndedSession_IsReadonly; bagian berikut berada di luar batas blok tersebut dalam
    // CustomRuleset_UsedByStartedOrEndedSession_IsReadonly.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    /// <summary>
    /// Memvalidasi bahwa kebijakan password diterapkan pada endpoint registrasi dan
    /// pembuatan player, menolak password yang terlalu pendek dengan error VALIDATION_ERROR.
    /// </summary>
    // Mendefinisikan metode `PasswordPolicy_IsEnforced_ForRegisterAndCreatePlayer` dengan hasil bertipe `Task`; operasi ini menangani password policy
    // berstatus enforced untuk register dan create pemain. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian
    // melalui Task.
    public async Task PasswordPolicy_IsEnforced_ForRegisterAndCreatePlayer()
    // Membuka scope metode PasswordPolicy_IsEnforced_ForRegisterAndCreatePlayer; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // PasswordPolicy_IsEnforced_ForRegisterAndCreatePlayer.
    {
        // Menyiapkan variabel lokal `suffix` untuk nilai suffix dengan `Guid.NewGuid().ToString(”N”)[..8]`, yaitu elemen koleksi yang dipilih melalui
        // indeks atau kunci tersebut. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var suffix = Guid.NewGuid().ToString("N")[..8];
        // Menyiapkan variabel lokal `registerPayload` untuk nilai register payload dengan objek baru bertipe `RegisterRequest` dengan argumen (
        // $”it_shortpass_{suffix}”, ”Short1!”, ”PLAYER”, null). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var registerPayload = new RegisterRequest(
            // Meneruskan teks interpolasi `$”it_shortpass_{suffix}”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan sebagai argumen
            // ke konstruktor `RegisterRequest`.
            $"it_shortpass_{suffix}",
            // Meneruskan nilai literal `”Short1!”` sebagai argumen ke konstruktor `RegisterRequest`.
            "Short1!",
            // Meneruskan nilai literal `”PLAYER”` sebagai argumen ke konstruktor `RegisterRequest`.
            "PLAYER",
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen ke konstruktor `RegisterRequest`.
            null);

        // Menyiapkan variabel lokal `registerResponse` untuk nilai register respons dengan hasil operasi asinkron memanggil `_client.PostAsJsonAsync`
        // dengan `”/api/v1/auth/register”`, `registerPayload`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var registerResponse = await _client.PostAsJsonAsync("/api/v1/auth/register", registerPayload);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.BadRequest`,
        // `registerResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam PasswordPolicy_IsEnforced_ForRegisterAndCreatePlayer.
        Assert.Equal(HttpStatusCode.BadRequest, registerResponse.StatusCode);

        // Menyiapkan variabel lokal `registerError` untuk nilai register kesalahan dengan hasil operasi asinkron membaca tanpa argumen menjadi objek
        // bertipe sesuai kontrak JSON melalui `registerResponse.Content.ReadFromJsonAsync<ErrorResponse>`; await menunggu hasil tanpa memblokir thread
        // selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var registerError = await registerResponse.Content.ReadFromJsonAsync<ErrorResponse>();
        // Menjalankan pemeriksaan NotNull atas `registerError` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // PasswordPolicy_IsEnforced_ForRegisterAndCreatePlayer.
        Assert.NotNull(registerError);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”VALIDATION_ERROR”`,
        // `registerError.ErrorCode`); pengujian gagal jika keduanya berbeda dalam PasswordPolicy_IsEnforced_ForRegisterAndCreatePlayer.
        Assert.Equal("VALIDATION_ERROR", registerError.ErrorCode);

        // Menyiapkan variabel lokal `oversizedRegisterResponse` untuk nilai oversized register respons dengan hasil operasi asinkron memanggil
        // `_client.PostAsJsonAsync` dengan `”/api/v1/auth/register”`, `new RegisterRequest($”it_longpass_{suffix}”, new string('a', 73), ”PLAYER”, null)`;
        // await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var oversizedRegisterResponse = await _client.PostAsJsonAsync(
            // Meneruskan nilai literal `”/api/v1/auth/register”` sebagai argumen ke `_client.PostAsJsonAsync`.
            "/api/v1/auth/register",
            // Meneruskan objek baru bertipe `RegisterRequest` dengan argumen ($”it_longpass_{suffix}”, new string('a', 73), ”PLAYER”, null) sebagai argumen ke
            // `_client.PostAsJsonAsync`; Meneruskan teks interpolasi `$”it_longpass_{suffix}”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program
            // berjalan sebagai argumen ke konstruktor `RegisterRequest`; Meneruskan objek baru bertipe `string` dengan argumen ('a', 73) sebagai argumen ke
            // konstruktor `RegisterRequest`; Meneruskan nilai literal `'a'` sebagai argumen ke konstruktor `string`; Meneruskan nilai literal `73` sebagai
            // argumen ke konstruktor `string`; Meneruskan nilai literal `”PLAYER”` sebagai argumen ke konstruktor `RegisterRequest`; Meneruskan null, yaitu
            // penanda tidak ada nilai sebagai argumen ke konstruktor `RegisterRequest`.
            new RegisterRequest($"it_longpass_{suffix}", new string('a', 73), "PLAYER", null));
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.BadRequest`,
        // `oversizedRegisterResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam PasswordPolicy_IsEnforced_ForRegisterAndCreatePlayer.
        Assert.Equal(HttpStatusCode.BadRequest, oversizedRegisterResponse.StatusCode);

        // Menyiapkan variabel lokal `oversizedLoginResponse` untuk nilai oversized login respons dengan hasil operasi asinkron memanggil
        // `_client.PostAsJsonAsync` dengan `”/api/v1/auth/login”`, `new LoginRequest($”it_longpass_{suffix}”, new string('a', 73))`; await menunggu hasil
        // tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var oversizedLoginResponse = await _client.PostAsJsonAsync(
            // Meneruskan nilai literal `”/api/v1/auth/login”` sebagai argumen ke `_client.PostAsJsonAsync`.
            "/api/v1/auth/login",
            // Meneruskan objek baru bertipe `LoginRequest` dengan argumen ($”it_longpass_{suffix}”, new string('a', 73)) sebagai argumen ke
            // `_client.PostAsJsonAsync`; Meneruskan teks interpolasi `$”it_longpass_{suffix}”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program
            // berjalan sebagai argumen ke konstruktor `LoginRequest`; Meneruskan objek baru bertipe `string` dengan argumen ('a', 73) sebagai argumen ke
            // konstruktor `LoginRequest`; Meneruskan nilai literal `'a'` sebagai argumen ke konstruktor `string`; Meneruskan nilai literal `73` sebagai argumen
            // ke konstruktor `string`.
            new LoginRequest($"it_longpass_{suffix}", new string('a', 73)));
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.BadRequest`,
        // `oversizedLoginResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam PasswordPolicy_IsEnforced_ForRegisterAndCreatePlayer.
        Assert.Equal(HttpStatusCode.BadRequest, oversizedLoginResponse.StatusCode);

        // Menyiapkan variabel lokal `instructorUsername` untuk nilai instruktur username dengan teks interpolasi `$”it_pwd_instructor_{suffix}”`; nilai
        // ekspresi di dalam kurung kurawal disisipkan saat program berjalan. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var instructorUsername = $"it_pwd_instructor_{suffix}";
        // Menyiapkan variabel lokal `instructorPassword` untuk nilai instruktur password dengan nilai literal `”IntegrationPolicyInstructorPass!123”`. Tipe
        // yang dipakai adalah `string`.
        const string instructorPassword = "IntegrationPolicyInstructorPass!123";
        // Menyiapkan variabel lokal `instructorToken` untuk nilai instruktur token dengan `(await RegisterAsync(instructorUsername, instructorPassword,
        // ”INSTRUCTOR”)).AccessToken` (nilai akses token). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var instructorToken = (await RegisterAsync(instructorUsername, instructorPassword, "INSTRUCTOR")).AccessToken;

        // Menyiapkan variabel lokal `createPlayerResponse` untuk nilai create pemain respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
        // `HttpMethod.Post`, `”/api/v1/players”`, `new { display_name = $”Player Policy {suffix}”, username = $”it_pwd_player_{suffix}”, password =
        // ”Short1!” }`, `instructorToken`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var createPlayerResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Post` (nilai post) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Post,
            // Meneruskan nilai literal `”/api/v1/players”` sebagai argumen ke `SendJsonAsync`.
            "/api/v1/players",
            // Meneruskan objek anonim yang mengelompokkan display_name, username, password sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // PasswordPolicy_IsEnforced_ForRegisterAndCreatePlayer.
            {
                // Meneruskan objek anonim yang mengelompokkan display_name, username, password sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                display_name = $"Player Policy {suffix}",
                // Meneruskan objek anonim yang mengelompokkan display_name, username, password sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                username = $"it_pwd_player_{suffix}",
                // Meneruskan objek anonim yang mengelompokkan display_name, username, password sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                password = "Short1!"
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
            // PasswordPolicy_IsEnforced_ForRegisterAndCreatePlayer.
            },
            // Meneruskan `instructorToken` (nilai instruktur token) sebagai argumen ke `SendJsonAsync`.
            instructorToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.BadRequest`,
        // `createPlayerResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam PasswordPolicy_IsEnforced_ForRegisterAndCreatePlayer.
        Assert.Equal(HttpStatusCode.BadRequest, createPlayerResponse.StatusCode);

        // Menyiapkan variabel lokal `createPlayerError` untuk nilai create pemain kesalahan dengan hasil operasi asinkron membaca tanpa argumen menjadi
        // objek bertipe sesuai kontrak JSON melalui `createPlayerResponse.Content.ReadFromJsonAsync<ErrorResponse>`; await menunggu hasil tanpa memblokir
        // thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var createPlayerError = await createPlayerResponse.Content.ReadFromJsonAsync<ErrorResponse>();
        // Menjalankan pemeriksaan NotNull atas `createPlayerError` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // PasswordPolicy_IsEnforced_ForRegisterAndCreatePlayer.
        Assert.NotNull(createPlayerError);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”VALIDATION_ERROR”`,
        // `createPlayerError.ErrorCode`); pengujian gagal jika keduanya berbeda dalam PasswordPolicy_IsEnforced_ForRegisterAndCreatePlayer.
        Assert.Equal("VALIDATION_ERROR", createPlayerError.ErrorCode);

        // Menyiapkan variabel lokal `oversizedCreatePlayerResponse` untuk nilai oversized create pemain respons dengan hasil operasi asinkron memanggil
        // `SendJsonAsync` dengan `HttpMethod.Post`, `”/api/v1/players”`, `new { display_name = $”Long Password Player {suffix}”, username =
        // $”it_long_player_{suffix}”, password = new string('a', 73) }`, `instructorToken`; await menunggu hasil tanpa memblokir thread selama operasi
        // belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var oversizedCreatePlayerResponse = await SendJsonAsync(
            // Meneruskan `HttpMethod.Post` (nilai post) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Post,
            // Meneruskan nilai literal `”/api/v1/players”` sebagai argumen ke `SendJsonAsync`.
            "/api/v1/players",
            // Meneruskan objek anonim yang mengelompokkan display_name, username, password sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // PasswordPolicy_IsEnforced_ForRegisterAndCreatePlayer.
            {
                // Meneruskan objek anonim yang mengelompokkan display_name, username, password sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                display_name = $"Long Password Player {suffix}",
                // Meneruskan objek anonim yang mengelompokkan display_name, username, password sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                username = $"it_long_player_{suffix}",
                // Meneruskan nilai literal `'a'` sebagai argumen ke konstruktor `string`; Meneruskan nilai literal `73` sebagai argumen ke konstruktor `string`.
                password = new string('a', 73)
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
            // PasswordPolicy_IsEnforced_ForRegisterAndCreatePlayer.
            },
            // Meneruskan `instructorToken` (nilai instruktur token) sebagai argumen ke `SendJsonAsync`.
            instructorToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.BadRequest`,
        // `oversizedCreatePlayerResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam PasswordPolicy_IsEnforced_ForRegisterAndCreatePlayer.
        Assert.Equal(HttpStatusCode.BadRequest, oversizedCreatePlayerResponse.StatusCode);
    // Menutup scope metode PasswordPolicy_IsEnforced_ForRegisterAndCreatePlayer; bagian berikut berada di luar batas blok tersebut dalam
    // PasswordPolicy_IsEnforced_ForRegisterAndCreatePlayer.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `CreatePlayer_ConcurrentDuplicate_ReturnsCreatedAndConflict` dengan hasil bertipe `Task`; operasi ini menangani create
    // pemain concurrent duplicate returns created dan conflict. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan
    // penyelesaian melalui Task.
    public async Task CreatePlayer_ConcurrentDuplicate_ReturnsCreatedAndConflict()
    // Membuka scope metode CreatePlayer_ConcurrentDuplicate_ReturnsCreatedAndConflict; pernyataan/deklarasi berikut berada di dalam batas blok ini
    // dalam CreatePlayer_ConcurrentDuplicate_ReturnsCreatedAndConflict.
    {
        // Menyiapkan variabel lokal `suffix` untuk nilai suffix dengan `Guid.NewGuid().ToString(”N”)[..8]`, yaitu elemen koleksi yang dipilih melalui
        // indeks atau kunci tersebut. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var suffix = Guid.NewGuid().ToString("N")[..8];
        // Menyiapkan variabel lokal `instructor` untuk nilai instruktur dengan hasil operasi asinkron memanggil `RegisterAsync` dengan
        // `$”it_race_instructor_{suffix}”`, `”IntegrationRaceInstructorPass!123”`, `”INSTRUCTOR”`; await menunggu hasil tanpa memblokir thread selama
        // operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var instructor = await RegisterAsync(
            // Meneruskan teks interpolasi `$”it_race_instructor_{suffix}”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan sebagai
            // argumen ke `RegisterAsync`.
            $"it_race_instructor_{suffix}",
            // Meneruskan nilai literal `”IntegrationRaceInstructorPass!123”` sebagai argumen ke `RegisterAsync`.
            "IntegrationRaceInstructorPass!123",
            // Meneruskan nilai literal `”INSTRUCTOR”` sebagai argumen ke `RegisterAsync`.
            "INSTRUCTOR");
        // Menyiapkan variabel lokal `payload` untuk muatan detail event dalam format JSON dengan objek anonim yang mengelompokkan display_name, username,
        // password sebagai satu nilai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var payload = new
        // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // CreatePlayer_ConcurrentDuplicate_ReturnsCreatedAndConflict.
        {
            // Menggunakan `display_name` (nilai display nama) sebagai bagian ekspresi yang sedang disusun dalam
            // CreatePlayer_ConcurrentDuplicate_ReturnsCreatedAndConflict.
            display_name = $"Race Player {suffix}",
            // Menggunakan `username` (nama akun yang dipakai saat autentikasi) sebagai bagian ekspresi yang sedang disusun dalam
            // CreatePlayer_ConcurrentDuplicate_ReturnsCreatedAndConflict.
            username = $"it_race_player_{suffix}",
            // Menggunakan `password` (kata sandi masukan yang diperiksa sesuai kebijakan autentikasi) sebagai bagian ekspresi yang sedang disusun dalam
            // CreatePlayer_ConcurrentDuplicate_ReturnsCreatedAndConflict.
            password = "IntegrationRacePlayerPass!123"
        // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
        // CreatePlayer_ConcurrentDuplicate_ReturnsCreatedAndConflict.
        };

        // Menyiapkan variabel lokal `responses` untuk nilai responses dengan hasil operasi asinkron memanggil `Task.WhenAll` dengan
        // `SendJsonAsync(HttpMethod.Post, ”/api/v1/players”, payload, instructor.AccessToken)`, `SendJsonAsync(HttpMethod.Post, ”/api/v1/players”, payload,
        // instructor.AccessToken)`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai
        // awal.
        var responses = await Task.WhenAll(
            // Meneruskan memanggil `SendJsonAsync` dengan `HttpMethod.Post`, `”/api/v1/players”`, `payload`, `instructor.AccessToken` sebagai argumen ke
            // `Task.WhenAll`; Meneruskan `HttpMethod.Post` (nilai post) sebagai argumen ke `SendJsonAsync`; Meneruskan nilai literal `”/api/v1/players”`
            // sebagai argumen ke `SendJsonAsync`; Meneruskan `payload` (muatan detail event dalam format JSON) sebagai argumen ke `SendJsonAsync`; Meneruskan
            // `instructor.AccessToken` (nilai akses token) sebagai argumen ke `SendJsonAsync`.
            SendJsonAsync(HttpMethod.Post, "/api/v1/players", payload, instructor.AccessToken),
            // Meneruskan memanggil `SendJsonAsync` dengan `HttpMethod.Post`, `”/api/v1/players”`, `payload`, `instructor.AccessToken` sebagai argumen ke
            // `Task.WhenAll`; Meneruskan `HttpMethod.Post` (nilai post) sebagai argumen ke `SendJsonAsync`; Meneruskan nilai literal `”/api/v1/players”`
            // sebagai argumen ke `SendJsonAsync`; Meneruskan `payload` (muatan detail event dalam format JSON) sebagai argumen ke `SendJsonAsync`; Meneruskan
            // `instructor.AccessToken` (nilai akses token) sebagai argumen ke `SendJsonAsync`.
            SendJsonAsync(HttpMethod.Post, "/api/v1/players", payload, instructor.AccessToken));

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `responses`, `response =>
        // response.StatusCode == HttpStatusCode.Created` dalam CreatePlayer_ConcurrentDuplicate_ReturnsCreatedAndConflict.
        Assert.Contains(responses, response => response.StatusCode == HttpStatusCode.Created);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `responses`, `response =>
        // response.StatusCode == HttpStatusCode.Conflict` dalam CreatePlayer_ConcurrentDuplicate_ReturnsCreatedAndConflict.
        Assert.Contains(responses, response => response.StatusCode == HttpStatusCode.Conflict);
    // Menutup scope metode CreatePlayer_ConcurrentDuplicate_ReturnsCreatedAndConflict; bagian berikut berada di luar batas blok tersebut dalam
    // CreatePlayer_ConcurrentDuplicate_ReturnsCreatedAndConflict.
    }

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
        // Menyiapkan variabel lokal `responseText` untuk nilai respons text dengan hasil operasi asinkron memanggil `response.Content.ReadAsStringAsync`
        // dengan tanpa argumen; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai
        // awal.
        var responseText = await response.Content.ReadAsStringAsync();
        // Menjalankan pemeriksaan bahwa `response.StatusCode == HttpStatusCode.Created`, `$”Expected Created but got {response.StatusCode}. Body:
        // {responseText}”` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam RegisterAsync.
        Assert.True(
            // Meneruskan perbandingan kesamaan antara `response.StatusCode` dan `HttpStatusCode.Created` sebagai argumen ke `Assert.True`.
            response.StatusCode == HttpStatusCode.Created,
            // Meneruskan teks interpolasi `$”Expected Created but got {response.StatusCode}. Body: {responseText}”`; nilai ekspresi di dalam kurung kurawal
            // disisipkan saat program berjalan sebagai argumen ke `Assert.True`.
            $"Expected Created but got {response.StatusCode}. Body: {responseText}");

        // Menyiapkan variabel lokal `body` untuk nilai body dengan membaca `responseText` menjadi objek bertipe sesuai kontrak JSON melalui
        // `JsonSerializer.Deserialize<RegisterResponse>`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var body = JsonSerializer.Deserialize<RegisterResponse>(responseText);
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

    // Mendefinisikan metode `CreatePlayerAsync` dengan hasil bertipe `Task`; operasi ini menangani create pemain asinkron. async memungkinkan metode
    // menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `accessToken` bertipe `string` membawa nilai
    // akses token; Parameter `displayName` bertipe `string` membawa nilai display nama; Parameter `username` bertipe `string` membawa nama akun yang
    // dipakai saat autentikasi.
    private async Task CreatePlayerAsync(string accessToken, string displayName, string username)
    // Membuka scope metode CreatePlayerAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CreatePlayerAsync.
    {
        // Menyiapkan variabel lokal `response` untuk hasil respons yang akan dibaca atau dikirim kepada pemanggil dengan hasil operasi asinkron memanggil
        // `SendJsonAsync` dengan `HttpMethod.Post`, `”/api/v1/players”`, `new { display_name = displayName, username, password =
        // ”IntegrationPlayerPass!123” }`, `accessToken`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var response = await SendJsonAsync(
            // Meneruskan `HttpMethod.Post` (nilai post) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Post,
            // Meneruskan nilai literal `”/api/v1/players”` sebagai argumen ke `SendJsonAsync`.
            "/api/v1/players",
            // Meneruskan objek anonim yang mengelompokkan display_name, username, password sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // CreatePlayerAsync.
            {
                // Meneruskan objek anonim yang mengelompokkan display_name, username, password sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                display_name = displayName,
                // Meneruskan objek anonim yang mengelompokkan display_name, username, password sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                username,
                // Meneruskan objek anonim yang mengelompokkan display_name, username, password sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                password = "IntegrationPlayerPass!123"
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam CreatePlayerAsync.
            },
            // Meneruskan `accessToken` (nilai akses token) sebagai argumen ke `SendJsonAsync`.
            accessToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.Created`,
        // `response.StatusCode`); pengujian gagal jika keduanya berbeda dalam CreatePlayerAsync.
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    // Menutup scope metode CreatePlayerAsync; bagian berikut berada di luar batas blok tersebut dalam CreatePlayerAsync.
    }

    // Mendefinisikan metode `CreateRulesetAsync` dengan hasil bertipe `Task<CreateRulesetResponse>`; operasi ini menangani create aturan asinkron.
    // async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `accessToken` bertipe
    // `string` membawa nilai akses token; Parameter `suffix` bertipe `string` membawa nilai suffix; Parameter `startingCash` bertipe `int` membawa
    // nilai starting uang tunai.
    private async Task<CreateRulesetResponse> CreateRulesetAsync(string accessToken, string suffix, int startingCash)
    // Membuka scope metode CreateRulesetAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CreateRulesetAsync.
    {
        // Menyiapkan variabel lokal `response` untuk hasil respons yang akan dibaca atau dikirim kepada pemanggil dengan hasil operasi asinkron memanggil
        // `SendJsonAsync` dengan `HttpMethod.Post`, `”/api/v1/rulesets”`, `new { name = $”Ruleset Lock Guard {suffix}”, description = ”Ruleset lock guard
        // integration test”, definition = BuildRulesetDefinition(startingCash) }`, `accessToken`; await menunggu hasil tanpa memblokir thread selama
        // operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var response = await SendJsonAsync(
            // Meneruskan `HttpMethod.Post` (nilai post) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Post,
            // Meneruskan nilai literal `”/api/v1/rulesets”` sebagai argumen ke `SendJsonAsync`.
            "/api/v1/rulesets",
            // Meneruskan objek anonim yang mengelompokkan name, description, definition sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // CreateRulesetAsync.
            {
                // Meneruskan objek anonim yang mengelompokkan name, description, definition sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                name = $"Ruleset Lock Guard {suffix}",
                // Meneruskan objek anonim yang mengelompokkan name, description, definition sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                description = "Ruleset lock guard integration test",
                // Meneruskan `startingCash` (nilai starting uang tunai) sebagai argumen ke `BuildRulesetDefinition`.
                definition = BuildRulesetDefinition(startingCash)
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam CreateRulesetAsync.
            },
            // Meneruskan `accessToken` (nilai akses token) sebagai argumen ke `SendJsonAsync`.
            accessToken);
        // Menyiapkan variabel lokal `responseText` untuk nilai respons text dengan hasil operasi asinkron memanggil `response.Content.ReadAsStringAsync`
        // dengan tanpa argumen; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai
        // awal.
        var responseText = await response.Content.ReadAsStringAsync();
        // Menjalankan pemeriksaan bahwa `response.StatusCode == HttpStatusCode.Created`, `$”Expected Created but got {response.StatusCode}. Body:
        // {responseText}”` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam CreateRulesetAsync.
        Assert.True(
            // Meneruskan perbandingan kesamaan antara `response.StatusCode` dan `HttpStatusCode.Created` sebagai argumen ke `Assert.True`.
            response.StatusCode == HttpStatusCode.Created,
            // Meneruskan teks interpolasi `$”Expected Created but got {response.StatusCode}. Body: {responseText}”`; nilai ekspresi di dalam kurung kurawal
            // disisipkan saat program berjalan sebagai argumen ke `Assert.True`.
            $"Expected Created but got {response.StatusCode}. Body: {responseText}");

        // Menyiapkan variabel lokal `body` untuk nilai body dengan membaca `responseText` menjadi objek bertipe sesuai kontrak JSON melalui
        // `JsonSerializer.Deserialize<CreateRulesetResponse>`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var body = JsonSerializer.Deserialize<CreateRulesetResponse>(responseText);
        // Menjalankan pemeriksaan NotNull atas `body` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam CreateRulesetAsync.
        Assert.NotNull(body);
        // Mengembalikan `body` (nilai body) kepada pemanggil dalam CreateRulesetAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return body;
    // Menutup scope metode CreateRulesetAsync; bagian berikut berada di luar batas blok tersebut dalam CreateRulesetAsync.
    }

    // Mendefinisikan metode `CreateSessionAsync` dengan hasil bertipe `Task<CreateSessionResponse>`; operasi ini menangani create sesi asinkron. async
    // memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `accessToken` bertipe
    // `string` membawa nilai akses token; Parameter `suffix` bertipe `string` membawa nilai suffix; Parameter `rulesetVersionId` bertipe `Guid` membawa
    // identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang tepat.
    private async Task<CreateSessionResponse> CreateSessionAsync(string accessToken, string suffix, Guid rulesetVersionId)
    // Membuka scope metode CreateSessionAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CreateSessionAsync.
    {
        // Menyiapkan variabel lokal `response` untuk hasil respons yang akan dibaca atau dikirim kepada pemanggil dengan hasil operasi asinkron memanggil
        // `SendJsonAsync` dengan `HttpMethod.Post`, `”/api/v1/sessions”`, `new { session_name = $”Ruleset Lock Session {suffix}”, mode = ”PEMULA”,
        // ruleset_version_id = rulesetVersionId }`, `accessToken`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var response = await SendJsonAsync(
            // Meneruskan `HttpMethod.Post` (nilai post) sebagai argumen ke `SendJsonAsync`.
            HttpMethod.Post,
            // Meneruskan nilai literal `”/api/v1/sessions”` sebagai argumen ke `SendJsonAsync`.
            "/api/v1/sessions",
            // Meneruskan objek anonim yang mengelompokkan session_name, mode, ruleset_version_id sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
            new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // CreateSessionAsync.
            {
                // Meneruskan objek anonim yang mengelompokkan session_name, mode, ruleset_version_id sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                session_name = $"Ruleset Lock Session {suffix}",
                // Meneruskan objek anonim yang mengelompokkan session_name, mode, ruleset_version_id sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                mode = "PEMULA",
                // Meneruskan objek anonim yang mengelompokkan session_name, mode, ruleset_version_id sebagai satu nilai sebagai argumen ke `SendJsonAsync`.
                ruleset_version_id = rulesetVersionId
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam CreateSessionAsync.
            },
            // Meneruskan `accessToken` (nilai akses token) sebagai argumen ke `SendJsonAsync`.
            accessToken);
        // Menyiapkan variabel lokal `responseText` untuk nilai respons text dengan hasil operasi asinkron memanggil `response.Content.ReadAsStringAsync`
        // dengan tanpa argumen; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai
        // awal.
        var responseText = await response.Content.ReadAsStringAsync();
        // Menjalankan pemeriksaan bahwa `response.StatusCode == HttpStatusCode.Created`, `$”Expected Created but got {response.StatusCode}. Body:
        // {responseText}”` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam CreateSessionAsync.
        Assert.True(
            // Meneruskan perbandingan kesamaan antara `response.StatusCode` dan `HttpStatusCode.Created` sebagai argumen ke `Assert.True`.
            response.StatusCode == HttpStatusCode.Created,
            // Meneruskan teks interpolasi `$”Expected Created but got {response.StatusCode}. Body: {responseText}”`; nilai ekspresi di dalam kurung kurawal
            // disisipkan saat program berjalan sebagai argumen ke `Assert.True`.
            $"Expected Created but got {response.StatusCode}. Body: {responseText}");

        // Menyiapkan variabel lokal `body` untuk nilai body dengan membaca `responseText` menjadi objek bertipe sesuai kontrak JSON melalui
        // `JsonSerializer.Deserialize<CreateSessionResponse>`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var body = JsonSerializer.Deserialize<CreateSessionResponse>(responseText);
        // Menjalankan pemeriksaan NotNull atas `body` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam CreateSessionAsync.
        Assert.NotNull(body);
        // Mengembalikan `body` (nilai body) kepada pemanggil dalam CreateSessionAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return body;
    // Menutup scope metode CreateSessionAsync; bagian berikut berada di luar batas blok tersebut dalam CreateSessionAsync.
    }

    // Mendefinisikan metode `AddPlayersForStartAsync` dengan hasil bertipe `Task`; operasi ini menangani add pemain untuk start asinkron. async
    // memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `accessToken` bertipe
    // `string` membawa nilai akses token; Parameter `sessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi
    // ini; Parameter `suffix` bertipe `string` membawa nilai suffix.
    private async Task AddPlayersForStartAsync(string accessToken, Guid sessionId, string suffix)
    // Membuka scope metode AddPlayersForStartAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam AddPlayersForStartAsync.
    {
        // Menyiapkan variabel lokal `firstUsername` untuk nilai first username dengan teks interpolasi `$”it_lock_player1_{suffix}”`; nilai ekspresi di
        // dalam kurung kurawal disisipkan saat program berjalan. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var firstUsername = $"it_lock_player1_{suffix}";
        // Menyiapkan variabel lokal `secondUsername` untuk nilai second username dengan teks interpolasi `$”it_lock_player2_{suffix}”`; nilai ekspresi di
        // dalam kurung kurawal disisipkan saat program berjalan. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var secondUsername = $"it_lock_player2_{suffix}";
        // Menjalankan hasil operasi asinkron memanggil `CreatePlayerAsync` dengan `accessToken`, `$”Lock Player 1 {suffix}”`, `firstUsername`; await
        // menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam AddPlayersForStartAsync.
        await CreatePlayerAsync(accessToken, $"Lock Player 1 {suffix}", firstUsername);
        // Menjalankan hasil operasi asinkron memanggil `CreatePlayerAsync` dengan `accessToken`, `$”Lock Player 2 {suffix}”`, `secondUsername`; await
        // menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam AddPlayersForStartAsync.
        await CreatePlayerAsync(accessToken, $"Lock Player 2 {suffix}", secondUsername);

        // Mengulangi setiap elemen `new[] { new { username = firstUsername, player_order_no = 1 }, new { username = secondUsername, player_order_no = 2 }
        // }`; elemen saat ini disimpan sebagai `assignment` bertipe `var` untuk diproses oleh badan loop dalam AddPlayersForStartAsync.
        foreach (var assignment in new[]
                 // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                 // AddPlayersForStartAsync.
                 {
                     // Menggunakan objek anonim yang mengelompokkan username, player_order_no sebagai satu nilai sebagai bagian ekspresi yang sedang disusun dalam
                     // AddPlayersForStartAsync.
                     new { username = firstUsername, player_order_no = 1 },
                     // Menggunakan objek anonim yang mengelompokkan username, player_order_no sebagai satu nilai sebagai bagian ekspresi yang sedang disusun dalam
                     // AddPlayersForStartAsync.
                     new { username = secondUsername, player_order_no = 2 }
                 // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam AddPlayersForStartAsync.
                 })
        // Membuka scope loop setiap assignment dari `new[] { new { username = firstUsername, player_order_no = 1 }, new { username = secondUsername,
        // player_order_no = 2 } }`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam AddPlayersForStartAsync.
        {
            // Menyiapkan variabel lokal `addPlayerResponse` untuk nilai add pemain respons dengan hasil operasi asinkron memanggil `SendJsonAsync` dengan
            // `HttpMethod.Post`, `$”/api/v1/sessions/{sessionId}/players”`, `assignment`, `accessToken`; await menunggu hasil tanpa memblokir thread selama
            // operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var addPlayerResponse = await SendJsonAsync(
                // Meneruskan `HttpMethod.Post` (nilai post) sebagai argumen ke `SendJsonAsync`.
                HttpMethod.Post,
                // Meneruskan teks interpolasi `$”/api/v1/sessions/{sessionId}/players”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan
                // sebagai argumen ke `SendJsonAsync`.
                $"/api/v1/sessions/{sessionId}/players",
                // Meneruskan `assignment` (nilai assignment) sebagai argumen ke `SendJsonAsync`.
                assignment,
                // Meneruskan `accessToken` (nilai akses token) sebagai argumen ke `SendJsonAsync`.
                accessToken);
            // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`,
            // `addPlayerResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam AddPlayersForStartAsync.
            Assert.Equal(HttpStatusCode.OK, addPlayerResponse.StatusCode);
        // Menutup scope loop setiap assignment dari `new[] { new { username = firstUsername, player_order_no = 1 }, new { username = secondUsername,
        // player_order_no = 2 } }`; bagian berikut berada di luar batas blok tersebut dalam AddPlayersForStartAsync.
        }
    // Menutup scope metode AddPlayersForStartAsync; bagian berikut berada di luar batas blok tersebut dalam AddPlayersForStartAsync.
    }

    // Mendefinisikan metode `AssertDomainRuleViolationAsync` dengan hasil bertipe `Task`; operasi ini menangani assert domain rule violation asinkron.
    // async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `response` bertipe
    // `HttpResponseMessage` membawa hasil respons yang akan dibaca atau dikirim kepada pemanggil.
    private static async Task AssertDomainRuleViolationAsync(HttpResponseMessage response)
    // Membuka scope metode AssertDomainRuleViolationAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // AssertDomainRuleViolationAsync.
    {
        // Menyiapkan variabel lokal `responseText` untuk nilai respons text dengan hasil operasi asinkron memanggil `response.Content.ReadAsStringAsync`
        // dengan tanpa argumen; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai
        // awal.
        var responseText = await response.Content.ReadAsStringAsync();
        // Menjalankan pemeriksaan bahwa `response.StatusCode == HttpStatusCode.UnprocessableEntity`, `$”Expected UnprocessableEntity but got
        // {response.StatusCode}. Body: {responseText}”` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam AssertDomainRuleViolationAsync.
        Assert.True(
            // Meneruskan perbandingan kesamaan antara `response.StatusCode` dan `HttpStatusCode.UnprocessableEntity` sebagai argumen ke `Assert.True`.
            response.StatusCode == HttpStatusCode.UnprocessableEntity,
            // Meneruskan teks interpolasi `$”Expected UnprocessableEntity but got {response.StatusCode}. Body: {responseText}”`; nilai ekspresi di dalam kurung
            // kurawal disisipkan saat program berjalan sebagai argumen ke `Assert.True`.
            $"Expected UnprocessableEntity but got {response.StatusCode}. Body: {responseText}");

        // Menyiapkan variabel lokal `error` untuk informasi kesalahan yang dikembalikan atau dicatat ketika operasi tidak berhasil dengan membaca
        // `responseText` menjadi objek bertipe sesuai kontrak JSON melalui `JsonSerializer.Deserialize<ErrorResponse>`. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var error = JsonSerializer.Deserialize<ErrorResponse>(responseText);
        // Menjalankan pemeriksaan NotNull atas `error` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam AssertDomainRuleViolationAsync.
        Assert.NotNull(error);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”DOMAIN_RULE_VIOLATION”`, `error.ErrorCode`);
        // pengujian gagal jika keduanya berbeda dalam AssertDomainRuleViolationAsync.
        Assert.Equal("DOMAIN_RULE_VIOLATION", error.ErrorCode);
    // Menutup scope metode AssertDomainRuleViolationAsync; bagian berikut berada di luar batas blok tersebut dalam AssertDomainRuleViolationAsync.
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

    // Mendefinisikan metode `AssertRulesetListIncludesDefaultRowsAsync` dengan hasil bertipe `Task`; operasi ini menangani assert aturan daftar
    // includes bawaan baris asinkron. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan:
    // Parameter `response` bertipe `HttpResponseMessage` membawa hasil respons yang akan dibaca atau dikirim kepada pemanggil.
    private static async Task AssertRulesetListIncludesDefaultRowsAsync(HttpResponseMessage response)
    // Membuka scope metode AssertRulesetListIncludesDefaultRowsAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // AssertRulesetListIncludesDefaultRowsAsync.
    {
        // Menyiapkan variabel lokal `json` untuk nilai JSON dengan hasil operasi asinkron memanggil `response.Content.ReadAsStringAsync` dengan tanpa
        // argumen; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var json = await response.Content.ReadAsStringAsync();
        // Menyiapkan variabel lokal `document` untuk nilai document dengan memanggil `JsonDocument.Parse` dengan `json`. Tipe variabel disimpulkan dari
        // ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var document = JsonDocument.Parse(json);
        // Menyiapkan variabel lokal `items` untuk nilai elemen dengan mematerialisasi urutan `document.RootElement.GetProperty(”items”).EnumerateArray()`
        // menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var items = document.RootElement.GetProperty("items").EnumerateArray().ToList();
        // Menyiapkan variabel lokal `defaultItems` untuk nilai bawaan elemen dengan mematerialisasi urutan `items .Where(item =>
        // item.TryGetProperty(”is_default”, out var isDefault) && isDefault.GetBoolean())` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam
        // memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var defaultItems = items
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(item => item.TryGetProperty(”is_default”, out var isDefault) &&
            // isDefault.GetBoolean()) dalam AssertRulesetListIncludesDefaultRowsAsync; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Where(item => item.TryGetProperty("is_default", out var isDefault) && isDefault.GetBoolean())
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList(); dalam AssertRulesetListIncludesDefaultRowsAsync; token pada baris
            // ini menyambungkan bagian kode sebelum dan sesudahnya.
            .ToList();

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`2`, `defaultItems.Count`); pengujian gagal
        // jika keduanya berbeda dalam AssertRulesetListIncludesDefaultRowsAsync.
        Assert.Equal(2, defaultItems.Count);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `defaultItems`, `item =>
        // string.Equals(item.GetProperty(”name”).GetString(), ”Cashflowpoly Default - Mode Pemula”, StringComparison.Ordinal)` dalam
        // AssertRulesetListIncludesDefaultRowsAsync.
        Assert.Contains(defaultItems, item => string.Equals(item.GetProperty("name").GetString(), "Cashflowpoly Default - Mode Pemula", StringComparison.Ordinal));
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `defaultItems`, `item =>
        // string.Equals(item.GetProperty(”name”).GetString(), ”Cashflowpoly Default - Mode Mahir”, StringComparison.Ordinal)` dalam
        // AssertRulesetListIncludesDefaultRowsAsync.
        Assert.Contains(defaultItems, item => string.Equals(item.GetProperty("name").GetString(), "Cashflowpoly Default - Mode Mahir", StringComparison.Ordinal));
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `defaultItems`, `item =>
        // string.Equals(item.GetProperty(”mode”).GetString(), ”PEMULA”, StringComparison.Ordinal)` dalam AssertRulesetListIncludesDefaultRowsAsync.
        Assert.Contains(defaultItems, item => string.Equals(item.GetProperty("mode").GetString(), "PEMULA", StringComparison.Ordinal));
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `defaultItems`, `item =>
        // string.Equals(item.GetProperty(”mode”).GetString(), ”MAHIR”, StringComparison.Ordinal)` dalam AssertRulesetListIncludesDefaultRowsAsync.
        Assert.Contains(defaultItems, item => string.Equals(item.GetProperty("mode").GetString(), "MAHIR", StringComparison.Ordinal));
        // Menjalankan pemeriksaan hasil dengan `Assert.All` menggunakan `defaultItems`, `item => Assert.Equal(”ACTIVE”,
        // item.GetProperty(”status”).GetString())`; ketidaksesuaian dengan ekspektasi membuat pengujian gagal dalam
        // AssertRulesetListIncludesDefaultRowsAsync.
        Assert.All(defaultItems, item => Assert.Equal("ACTIVE", item.GetProperty("status").GetString()));
        // Menjalankan pemeriksaan hasil dengan `Assert.All` menggunakan `defaultItems`, `item => Assert.True( item.TryGetProperty(”is_locked_by_session”,
        // out var locked) && locked.ValueKind is JsonValueKind.True or JsonValueKind.False)`; ketidaksesuaian dengan ekspektasi membuat pengujian gagal
        // dalam AssertRulesetListIncludesDefaultRowsAsync.
        Assert.All(defaultItems, item => Assert.True(
            // Meneruskan gabungan syarat AND: kedua kondisi wajib benar antara `item.TryGetProperty(”is_locked_by_session”, out var locked)` dan
            // `locked.ValueKind is JsonValueKind.True or JsonValueKind.False`; sisi kanan diperiksa hanya jika sisi kiri benar sebagai argumen ke
            // `Assert.True`; Meneruskan nilai literal `”is_locked_by_session”` sebagai argumen ke `item.TryGetProperty`; Meneruskan `var locked` sebagai
            // argumen ke `item.TryGetProperty`.
            item.TryGetProperty("is_locked_by_session", out var locked) &&
            // Meneruskan gabungan syarat AND: kedua kondisi wajib benar antara `item.TryGetProperty(”is_locked_by_session”, out var locked)` dan
            // `locked.ValueKind is JsonValueKind.True or JsonValueKind.False`; sisi kanan diperiksa hanya jika sisi kiri benar sebagai argumen ke
            // `Assert.True`.
            locked.ValueKind is JsonValueKind.True or JsonValueKind.False));
    // Menutup scope metode AssertRulesetListIncludesDefaultRowsAsync; bagian berikut berada di luar batas blok tersebut dalam
    // AssertRulesetListIncludesDefaultRowsAsync.
    }

    /// <summary>
    /// Helper yang membangun objek konfigurasi ruleset lengkap untuk mode PEMULA
    /// dengan parameter starting cash yang dapat dikustomisasi.
    /// </summary>
    // Mendefinisikan metode `BuildRulesetDefinition` dengan hasil bertipe `RulesetDefinitionDto`. Helper yang membangun objek konfigurasi ruleset
    // lengkap untuk mode PEMULA dengan parameter starting cash yang dapat dikustomisasi. Masukan: Parameter `startingCash` bertipe `int` membawa nilai
    // starting uang tunai.
    private static RulesetDefinitionDto BuildRulesetDefinition(int startingCash)
    // Membuka scope metode BuildRulesetDefinition; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildRulesetDefinition.
    {
        // Mengembalikan objek baru bertipe `RulesetDefinitionDto` dengan nilai awal sesuai konstruktornya kepada pemanggil dalam BuildRulesetDefinition;
        // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return new RulesetDefinitionDto
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // BuildRulesetDefinition.
        {
            // Memperbarui `Mode` menggunakan nilai literal `”PEMULA”` dalam BuildRulesetDefinition.
            Mode = "PEMULA",
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
                // Memperbarui `LoanEnabled` menggunakan false, yaitu kondisi nonaktif/tidak terpenuhi dalam BuildRulesetDefinition.
                LoanEnabled = false,
                // Memperbarui `InsuranceEnabled` menggunakan false, yaitu kondisi nonaktif/tidak terpenuhi dalam BuildRulesetDefinition.
                InsuranceEnabled = false,
                // Memperbarui `SavingGoalEnabled` menggunakan false, yaitu kondisi nonaktif/tidak terpenuhi dalam BuildRulesetDefinition.
                SavingGoalEnabled = false,
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
                // Memperbarui `OrderingCode` menggunakan nilai literal `”PLAYER_ORDER”` dalam BuildRulesetDefinition.
                OrderingCode = "PLAYER_ORDER",
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
            // ”JualMasakan” }, new RulesetActionDto { ActionId = ”KerjaLepas” } dalam BuildRulesetDefinition.
            Actions =
            // Menggunakan koleksi berisi new RulesetActionDto { ActionId = ”BahanMasakan” }, new RulesetActionDto { ActionId = ”JualMasakan” }, new
            // RulesetActionDto { ActionId = ”KerjaLepas” } sebagai bagian ekspresi yang sedang disusun dalam BuildRulesetDefinition.
            [
                // Menggunakan objek baru bertipe `RulesetActionDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun dalam
                // BuildRulesetDefinition.
                new RulesetActionDto { ActionId = "BahanMasakan" },
                // Menggunakan objek baru bertipe `RulesetActionDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun dalam
                // BuildRulesetDefinition.
                new RulesetActionDto { ActionId = "JualMasakan" },
                // Menggunakan objek baru bertipe `RulesetActionDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun dalam
                // BuildRulesetDefinition.
                new RulesetActionDto { ActionId = "KerjaLepas" }
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
                new RulesetIngredientDto
                // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // BuildRulesetDefinition.
                {
                    // Memperbarui `Id` menggunakan nilai literal `”nasi_putih”` dalam BuildRulesetDefinition.
                    Id = "nasi_putih",
                    // Memperbarui `Nama` menggunakan nilai literal `”Nasi Putih”` dalam BuildRulesetDefinition.
                    Nama = "Nasi Putih",
                    // Memperbarui `HargaBeli` menggunakan nilai literal `1` dalam BuildRulesetDefinition.
                    HargaBeli = 1
                // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam BuildRulesetDefinition.
                },
                // Menggunakan objek baru bertipe `RulesetIngredientDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun dalam
                // BuildRulesetDefinition.
                new RulesetIngredientDto
                // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // BuildRulesetDefinition.
                {
                    // Memperbarui `Id` menggunakan nilai literal `”telur”` dalam BuildRulesetDefinition.
                    Id = "telur",
                    // Memperbarui `Nama` menggunakan nilai literal `”Telur”` dalam BuildRulesetDefinition.
                    Nama = "Telur",
                    // Memperbarui `HargaBeli` menggunakan nilai literal `4` dalam BuildRulesetDefinition.
                    HargaBeli = 4
                // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam BuildRulesetDefinition.
                },
                // Menggunakan objek baru bertipe `RulesetIngredientDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun dalam
                // BuildRulesetDefinition.
                new RulesetIngredientDto
                // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // BuildRulesetDefinition.
                {
                    // Memperbarui `Id` menggunakan nilai literal `”sayur”` dalam BuildRulesetDefinition.
                    Id = "sayur",
                    // Memperbarui `Nama` menggunakan nilai literal `”Sayur”` dalam BuildRulesetDefinition.
                    Nama = "Sayur",
                    // Memperbarui `HargaBeli` menggunakan nilai literal `2` dalam BuildRulesetDefinition.
                    HargaBeli = 2
                // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam BuildRulesetDefinition.
                }
            // Menandai akhir daftar elemen atau indeks koleksi dalam BuildRulesetDefinition; pasangan kurung siku mengelompokkan nilai sebagai satu struktur.
            ],
            // Memperbarui `Orders` menggunakan `{ new RulesetOrderDto { Id = ”nasi_goreng”, Nama = ”nasi goreng”, HargaJual = 15, PoinKebahagiaan = 0, Bahan =
            // [”Nasi Putih”, ”Telur”], CardQty = 5 } }` dalam BuildRulesetDefinition.
            Orders =
            // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // BuildRulesetDefinition.
            {
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
                    // Memperbarui `HargaJual` menggunakan nilai literal `15` dalam BuildRulesetDefinition.
                    HargaJual = 15,
                    // Memperbarui `PoinKebahagiaan` menggunakan nilai literal `0` dalam BuildRulesetDefinition.
                    PoinKebahagiaan = 0,
                    // Memperbarui `Bahan` menggunakan koleksi berisi ”Nasi Putih”, ”Telur” dalam BuildRulesetDefinition.
                    Bahan = ["Nasi Putih", "Telur"],
                    // Memperbarui `CardQty` menggunakan nilai literal `5` dalam BuildRulesetDefinition.
                    CardQty = 5
                // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam BuildRulesetDefinition.
                }
            // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam BuildRulesetDefinition.
            },
            // Memperbarui `Needs` menggunakan `{ new RulesetNeedDto { Id = ”buku”, Nama = ”buku”, Tipe = ”primer”, HargaBeli = 2, PoinKebahagiaan = 1 }, new
            // RulesetNeedDto { Id = ”baju”, Nama = ”baju”, Tipe = ”primer”, Har...` dalam BuildRulesetDefinition.
            Needs =
            // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // BuildRulesetDefinition.
            {
                // Menggunakan objek baru bertipe `RulesetNeedDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun dalam
                // BuildRulesetDefinition.
                new RulesetNeedDto
                // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // BuildRulesetDefinition.
                {
                    // Memperbarui `Id` menggunakan nilai literal `”buku”` dalam BuildRulesetDefinition.
                    Id = "buku",
                    // Memperbarui `Nama` menggunakan nilai literal `”buku”` dalam BuildRulesetDefinition.
                    Nama = "buku",
                    // Memperbarui `Tipe` menggunakan nilai literal `”primer”` dalam BuildRulesetDefinition.
                    Tipe = "primer",
                    // Memperbarui `HargaBeli` menggunakan nilai literal `2` dalam BuildRulesetDefinition.
                    HargaBeli = 2,
                    // Memperbarui `PoinKebahagiaan` menggunakan nilai literal `1` dalam BuildRulesetDefinition.
                    PoinKebahagiaan = 1
                // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam BuildRulesetDefinition.
                },
                // Menggunakan objek baru bertipe `RulesetNeedDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun dalam
                // BuildRulesetDefinition.
                new RulesetNeedDto
                // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // BuildRulesetDefinition.
                {
                    // Memperbarui `Id` menggunakan nilai literal `”baju”` dalam BuildRulesetDefinition.
                    Id = "baju",
                    // Memperbarui `Nama` menggunakan nilai literal `”baju”` dalam BuildRulesetDefinition.
                    Nama = "baju",
                    // Memperbarui `Tipe` menggunakan nilai literal `”primer”` dalam BuildRulesetDefinition.
                    Tipe = "primer",
                    // Memperbarui `HargaBeli` menggunakan nilai literal `2` dalam BuildRulesetDefinition.
                    HargaBeli = 2,
                    // Memperbarui `PoinKebahagiaan` menggunakan nilai literal `1` dalam BuildRulesetDefinition.
                    PoinKebahagiaan = 1
                // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam BuildRulesetDefinition.
                },
                // Menggunakan objek baru bertipe `RulesetNeedDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun dalam
                // BuildRulesetDefinition.
                new RulesetNeedDto
                // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // BuildRulesetDefinition.
                {
                    // Memperbarui `Id` menggunakan nilai literal `”sepatu”` dalam BuildRulesetDefinition.
                    Id = "sepatu",
                    // Memperbarui `Nama` menggunakan nilai literal `”sepatu”` dalam BuildRulesetDefinition.
                    Nama = "sepatu",
                    // Memperbarui `Tipe` menggunakan nilai literal `”primer”` dalam BuildRulesetDefinition.
                    Tipe = "primer",
                    // Memperbarui `HargaBeli` menggunakan nilai literal `2` dalam BuildRulesetDefinition.
                    HargaBeli = 2,
                    // Memperbarui `PoinKebahagiaan` menggunakan nilai literal `1` dalam BuildRulesetDefinition.
                    PoinKebahagiaan = 1
                // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam BuildRulesetDefinition.
                },
                // Menggunakan objek baru bertipe `RulesetNeedDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun dalam
                // BuildRulesetDefinition.
                new RulesetNeedDto
                // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // BuildRulesetDefinition.
                {
                    // Memperbarui `Id` menggunakan nilai literal `”tempat_makan”` dalam BuildRulesetDefinition.
                    Id = "tempat_makan",
                    // Memperbarui `Nama` menggunakan nilai literal `”tempat makan”` dalam BuildRulesetDefinition.
                    Nama = "tempat makan",
                    // Memperbarui `Tipe` menggunakan nilai literal `”primer”` dalam BuildRulesetDefinition.
                    Tipe = "primer",
                    // Memperbarui `HargaBeli` menggunakan nilai literal `2` dalam BuildRulesetDefinition.
                    HargaBeli = 2,
                    // Memperbarui `PoinKebahagiaan` menggunakan nilai literal `1` dalam BuildRulesetDefinition.
                    PoinKebahagiaan = 1
                // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam BuildRulesetDefinition.
                },
                // Menggunakan objek baru bertipe `RulesetNeedDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun dalam
                // BuildRulesetDefinition.
                new RulesetNeedDto
                // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // BuildRulesetDefinition.
                {
                    // Memperbarui `Id` menggunakan nilai literal `”alat_tulis”` dalam BuildRulesetDefinition.
                    Id = "alat_tulis",
                    // Memperbarui `Nama` menggunakan nilai literal `”alat tulis”` dalam BuildRulesetDefinition.
                    Nama = "alat tulis",
                    // Memperbarui `Tipe` menggunakan nilai literal `”primer”` dalam BuildRulesetDefinition.
                    Tipe = "primer",
                    // Memperbarui `HargaBeli` menggunakan nilai literal `2` dalam BuildRulesetDefinition.
                    HargaBeli = 2,
                    // Memperbarui `PoinKebahagiaan` menggunakan nilai literal `1` dalam BuildRulesetDefinition.
                    PoinKebahagiaan = 1
                // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam BuildRulesetDefinition.
                },
                // Menggunakan objek baru bertipe `RulesetNeedDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun dalam
                // BuildRulesetDefinition.
                new RulesetNeedDto
                // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // BuildRulesetDefinition.
                {
                    // Memperbarui `Id` menggunakan nilai literal `”boneka”` dalam BuildRulesetDefinition.
                    Id = "boneka",
                    // Memperbarui `Nama` menggunakan nilai literal `”boneka”` dalam BuildRulesetDefinition.
                    Nama = "boneka",
                    // Memperbarui `Tipe` menggunakan nilai literal `”tersier”` dalam BuildRulesetDefinition.
                    Tipe = "tersier",
                    // Memperbarui `HargaBeli` menggunakan nilai literal `6` dalam BuildRulesetDefinition.
                    HargaBeli = 6,
                    // Memperbarui `PoinKebahagiaan` menggunakan nilai literal `3` dalam BuildRulesetDefinition.
                    PoinKebahagiaan = 3
                // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam BuildRulesetDefinition.
                },
                // Menggunakan objek baru bertipe `RulesetNeedDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun dalam
                // BuildRulesetDefinition.
                new RulesetNeedDto
                // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // BuildRulesetDefinition.
                {
                    // Memperbarui `Id` menggunakan nilai literal `”gameboy”` dalam BuildRulesetDefinition.
                    Id = "gameboy", Nama = "gameboy", Tipe = "tersier", HargaBeli = 6, PoinKebahagiaan = 3
                // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam BuildRulesetDefinition.
                },
                // Menggunakan objek baru bertipe `RulesetNeedDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun dalam
                // BuildRulesetDefinition.
                new RulesetNeedDto
                // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // BuildRulesetDefinition.
                {
                    // Memperbarui `Id` menggunakan nilai literal `”hiburan”` dalam BuildRulesetDefinition.
                    Id = "hiburan", Nama = "hiburan", Tipe = "tersier", HargaBeli = 6, PoinKebahagiaan = 3
                // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam BuildRulesetDefinition.
                },
                // Menggunakan objek baru bertipe `RulesetNeedDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun dalam
                // BuildRulesetDefinition.
                new RulesetNeedDto
                // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // BuildRulesetDefinition.
                {
                    // Memperbarui `Id` menggunakan nilai literal `”jam”` dalam BuildRulesetDefinition.
                    Id = "jam", Nama = "jam", Tipe = "tersier", HargaBeli = 6, PoinKebahagiaan = 3
                // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam BuildRulesetDefinition.
                }
            // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam BuildRulesetDefinition.
            },
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
                        new RulesetCollectionMissionRequirementDto
                        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                        // BuildRulesetDefinition.
                        {
                            // Memperbarui `Order` menggunakan nilai literal `1` dalam BuildRulesetDefinition.
                            Order = 1,
                            // Memperbarui `Type` menggunakan nilai literal `”primer”` dalam BuildRulesetDefinition.
                            Type = "primer",
                            // Memperbarui `Value` menggunakan nilai literal `”buku”` dalam BuildRulesetDefinition.
                            Value = "buku"
                        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam BuildRulesetDefinition.
                        },
                        // Menggunakan objek baru bertipe `RulesetCollectionMissionRequirementDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang
                        // sedang disusun dalam BuildRulesetDefinition.
                        new RulesetCollectionMissionRequirementDto
                        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                        // BuildRulesetDefinition.
                        {
                            // Memperbarui `Order` menggunakan nilai literal `2` dalam BuildRulesetDefinition.
                            Order = 2,
                            // Memperbarui `Type` menggunakan nilai literal `”tersier”` dalam BuildRulesetDefinition.
                            Type = "tersier",
                            // Memperbarui `Value` menggunakan nilai literal `”boneka”` dalam BuildRulesetDefinition.
                            Value = "boneka"
                        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam BuildRulesetDefinition.
                        }
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
            // Memperbarui `FinancialGoals` menggunakan `{ new RulesetFinancialGoalDto { Id = ”beli_rumah”, Nama = ”beli rumah”, HargaBeli = 20, PoinKebahagiaan
            // = 5 } }` dalam BuildRulesetDefinition.
            FinancialGoals =
            // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // BuildRulesetDefinition.
            {
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
            // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam BuildRulesetDefinition.
            },
            // Memperbarui `Narratives` menggunakan koleksi berisi new RulesetNarrativeDto { Id = ”jual_pertama”, Nam... dalam BuildRulesetDefinition.
            Narratives =
            // Menggunakan koleksi berisi new RulesetNarrativeDto { Id = ”jual_pertama”, Nam... sebagai bagian ekspresi yang sedang disusun dalam
            // BuildRulesetDefinition.
            [
                // Menggunakan objek baru bertipe `RulesetNarrativeDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun dalam
                // BuildRulesetDefinition.
                new RulesetNarrativeDto
                // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // BuildRulesetDefinition.
                {
                    // Memperbarui `Id` menggunakan nilai literal `”jual_pertama”` dalam BuildRulesetDefinition.
                    Id = "jual_pertama",
                    // Memperbarui `Nama` menggunakan nilai literal `”jual_pertama”` dalam BuildRulesetDefinition.
                    Nama = "jual_pertama",
                    // Memperbarui `Teks` menggunakan koleksi berisi ”Narasi integrasi typed” dalam BuildRulesetDefinition.
                    Teks = ["Narasi integrasi typed"],
                    // Memperbarui `PrerequisiteAksi` menggunakan koleksi berisi new RulesetNarrativePrerequisiteDto { Aksi = ”Jual... dalam BuildRulesetDefinition.
                    PrerequisiteAksi =
                    // Menggunakan koleksi berisi new RulesetNarrativePrerequisiteDto { Aksi = ”Jual... sebagai bagian ekspresi yang sedang disusun dalam
                    // BuildRulesetDefinition.
                    [
                        // Menggunakan objek baru bertipe `RulesetNarrativePrerequisiteDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang
                        // disusun dalam BuildRulesetDefinition.
                        new RulesetNarrativePrerequisiteDto
                        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                        // BuildRulesetDefinition.
                        {
                            // Memperbarui `Aksi` menggunakan nilai literal `”JualMasakan”` dalam BuildRulesetDefinition.
                            Aksi = "JualMasakan",
                            // Memperbarui `Value` menggunakan nilai literal `1` dalam BuildRulesetDefinition.
                            Value = 1
                        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam BuildRulesetDefinition.
                        }
                    // Menandai akhir daftar elemen atau indeks koleksi dalam BuildRulesetDefinition; pasangan kurung siku mengelompokkan nilai sebagai satu struktur.
                    ]
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
            ]
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam BuildRulesetDefinition.
        };
    // Menutup scope metode BuildRulesetDefinition; bagian berikut berada di luar batas blok tersebut dalam BuildRulesetDefinition.
    }
// Menutup scope tipe AuthRbacRulesetIntegrationTests; bagian berikut berada di luar batas blok tersebut.
}
