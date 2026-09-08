// Fungsi file: Memverifikasi perilaku API, database, atau domain melalui ObservabilitySecurityIntegrationTests.
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
/// Kelas pengujian integrasi yang memvalidasi bahwa endpoint observability summary
/// dan security audit logs menerapkan pembatasan akses berbasis role dengan benar.
/// </summary>
// Mendefinisikan tipe class `ObservabilitySecurityIntegrationTests`; sealed mencegah tipe ini diturunkan lagi.
public sealed class ObservabilitySecurityIntegrationTests
// Membuka scope tipe ObservabilitySecurityIntegrationTests; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendeklarasikan field bertipe `HttpClient`: `_client` menyimpan nilai client. readonly membatasi penggantian referensi/nilai field pada deklarasi
    // atau konstruktor.
    private readonly HttpClient _client;

    /// <summary>
    /// Menginisialisasi instance pengujian dengan HttpClient dari fixture integrasi bersama.
    /// </summary>
    // Mendefinisikan konstruktor ObservabilitySecurityIntegrationTests yang menyiapkan objek dan menerima dependency/nilai awal dari pemanggil;
    // parameter: Parameter `fixture` bertipe `ApiIntegrationTestFixture` membawa nilai fixture.
    public ObservabilitySecurityIntegrationTests(ApiIntegrationTestFixture fixture)
    // Membuka scope konstruktor ObservabilitySecurityIntegrationTests; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // ObservabilitySecurityIntegrationTests.
    {
        // Memperbarui `_client` menggunakan `fixture.Client` (nilai client) dalam ObservabilitySecurityIntegrationTests.
        _client = fixture.Client;
    // Menutup scope konstruktor ObservabilitySecurityIntegrationTests; bagian berikut berada di luar batas blok tersebut dalam
    // ObservabilitySecurityIntegrationTests.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    /// <summary>
    /// Memvalidasi bahwa INSTRUCTOR dapat mengakses observability summary dan audit logs,
    /// sedangkan PLAYER ditolak dengan status 403 Forbidden pada kedua endpoint.
    /// </summary>
    // Mendefinisikan metode `Observability_And_SecurityAudit_RespectRoleAccess` dengan hasil bertipe `Task`; operasi ini menangani observability dan
    // security audit respect role akses. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task.
    public async Task Observability_And_SecurityAudit_RespectRoleAccess()
    // Membuka scope metode Observability_And_SecurityAudit_RespectRoleAccess; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // Observability_And_SecurityAudit_RespectRoleAccess.
    {
        // Menyiapkan variabel lokal `suffix` untuk nilai suffix dengan `Guid.NewGuid().ToString(”N”)[..8]`, yaitu elemen koleksi yang dipilih melalui
        // indeks atau kunci tersebut. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var suffix = Guid.NewGuid().ToString("N")[..8];
        // Menyiapkan variabel lokal `instructorUsername` untuk nilai instruktur username dengan teks interpolasi `$”it_obs_instr_{suffix}”`; nilai ekspresi
        // di dalam kurung kurawal disisipkan saat program berjalan. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var instructorUsername = $"it_obs_instr_{suffix}";
        // Menyiapkan variabel lokal `playerUsername` untuk nilai pemain username dengan teks interpolasi `$”it_obs_player_{suffix}”`; nilai ekspresi di
        // dalam kurung kurawal disisipkan saat program berjalan. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var playerUsername = $"it_obs_player_{suffix}";
        // Menyiapkan variabel lokal `instructorPassword` untuk nilai instruktur password dengan nilai literal `”IntegrationInstructorPass!123”`. Tipe yang
        // dipakai adalah `string`.
        const string instructorPassword = "IntegrationInstructorPass!123";
        // Menyiapkan variabel lokal `playerPassword` untuk nilai pemain password dengan nilai literal `”IntegrationPlayerPass!123”`. Tipe yang dipakai
        // adalah `string`.
        const string playerPassword = "IntegrationPlayerPass!123";

        // Menjalankan hasil operasi asinkron memanggil `RegisterAsync` dengan `instructorUsername`, `instructorPassword`, `”INSTRUCTOR”`; await menunggu
        // hasil tanpa memblokir thread selama operasi belum selesai dalam Observability_And_SecurityAudit_RespectRoleAccess.
        await RegisterAsync(instructorUsername, instructorPassword, "INSTRUCTOR");
        // Menjalankan hasil operasi asinkron memanggil `RegisterAsync` dengan `playerUsername`, `playerPassword`, `”PLAYER”`; await menunggu hasil tanpa
        // memblokir thread selama operasi belum selesai dalam Observability_And_SecurityAudit_RespectRoleAccess.
        await RegisterAsync(playerUsername, playerPassword, "PLAYER");

        // Menyiapkan variabel lokal `instructorLogin` untuk nilai instruktur login dengan hasil operasi asinkron memanggil `LoginAsync` dengan
        // `instructorUsername`, `instructorPassword`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var instructorLogin = await LoginAsync(instructorUsername, instructorPassword);
        // Menyiapkan variabel lokal `playerLogin` untuk nilai pemain login dengan hasil operasi asinkron memanggil `LoginAsync` dengan `playerUsername`,
        // `playerPassword`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var playerLogin = await LoginAsync(playerUsername, playerPassword);

        // Menyiapkan variabel lokal `instructorObservability` untuk nilai instruktur observability dengan hasil operasi asinkron memanggil `SendAsync`
        // dengan `HttpMethod.Get`, `”/api/v1/observability/metrics/summary”`, `instructorLogin.AccessToken`; await menunggu hasil tanpa memblokir thread
        // selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var instructorObservability = await SendAsync(
            // Meneruskan `HttpMethod.Get` (nilai get) sebagai argumen ke `SendAsync`.
            HttpMethod.Get,
            // Meneruskan nilai literal `”/api/v1/observability/metrics/summary”` sebagai argumen ke `SendAsync`.
            "/api/v1/observability/metrics/summary",
            // Meneruskan `instructorLogin.AccessToken` (nilai akses token) sebagai argumen ke `SendAsync`.
            instructorLogin.AccessToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`,
        // `instructorObservability.StatusCode`); pengujian gagal jika keduanya berbeda dalam Observability_And_SecurityAudit_RespectRoleAccess.
        Assert.Equal(HttpStatusCode.OK, instructorObservability.StatusCode);
        // Menjalankan hasil operasi asinkron memanggil `AssertJsonHasPropertyAsync` dengan `instructorObservability`, `”message”`; await menunggu hasil
        // tanpa memblokir thread selama operasi belum selesai dalam Observability_And_SecurityAudit_RespectRoleAccess.
        await AssertJsonHasPropertyAsync(instructorObservability, "message");

        // Menyiapkan variabel lokal `playerObservability` untuk nilai pemain observability dengan hasil operasi asinkron memanggil `SendAsync` dengan
        // `HttpMethod.Get`, `”/api/v1/observability/metrics/summary”`, `playerLogin.AccessToken`; await menunggu hasil tanpa memblokir thread selama
        // operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var playerObservability = await SendAsync(
            // Meneruskan `HttpMethod.Get` (nilai get) sebagai argumen ke `SendAsync`.
            HttpMethod.Get,
            // Meneruskan nilai literal `”/api/v1/observability/metrics/summary”` sebagai argumen ke `SendAsync`.
            "/api/v1/observability/metrics/summary",
            // Meneruskan `playerLogin.AccessToken` (nilai akses token) sebagai argumen ke `SendAsync`.
            playerLogin.AccessToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.Forbidden`,
        // `playerObservability.StatusCode`); pengujian gagal jika keduanya berbeda dalam Observability_And_SecurityAudit_RespectRoleAccess.
        Assert.Equal(HttpStatusCode.Forbidden, playerObservability.StatusCode);

        // Menyiapkan variabel lokal `instructorAudit` untuk nilai instruktur audit dengan hasil operasi asinkron memanggil `SendAsync` dengan
        // `HttpMethod.Get`, `”/api/v1/security/audit-logs?limit=10”`, `instructorLogin.AccessToken`; await menunggu hasil tanpa memblokir thread selama
        // operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var instructorAudit = await SendAsync(
            // Meneruskan `HttpMethod.Get` (nilai get) sebagai argumen ke `SendAsync`.
            HttpMethod.Get,
            // Meneruskan nilai literal `”/api/v1/security/audit-logs?limit=10”` sebagai argumen ke `SendAsync`.
            "/api/v1/security/audit-logs?limit=10",
            // Meneruskan `instructorLogin.AccessToken` (nilai akses token) sebagai argumen ke `SendAsync`.
            instructorLogin.AccessToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`,
        // `instructorAudit.StatusCode`); pengujian gagal jika keduanya berbeda dalam Observability_And_SecurityAudit_RespectRoleAccess.
        Assert.Equal(HttpStatusCode.OK, instructorAudit.StatusCode);
        // Menjalankan hasil operasi asinkron memanggil `AssertJsonHasPropertyAsync` dengan `instructorAudit`, `”items”`; await menunggu hasil tanpa
        // memblokir thread selama operasi belum selesai dalam Observability_And_SecurityAudit_RespectRoleAccess.
        await AssertJsonHasPropertyAsync(instructorAudit, "items");

        // Menyiapkan variabel lokal `playerAudit` untuk nilai pemain audit dengan hasil operasi asinkron memanggil `SendAsync` dengan `HttpMethod.Get`,
        // `”/api/v1/security/audit-logs?limit=10”`, `playerLogin.AccessToken`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai.
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var playerAudit = await SendAsync(
            // Meneruskan `HttpMethod.Get` (nilai get) sebagai argumen ke `SendAsync`.
            HttpMethod.Get,
            // Meneruskan nilai literal `”/api/v1/security/audit-logs?limit=10”` sebagai argumen ke `SendAsync`.
            "/api/v1/security/audit-logs?limit=10",
            // Meneruskan `playerLogin.AccessToken` (nilai akses token) sebagai argumen ke `SendAsync`.
            playerLogin.AccessToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.Forbidden`,
        // `playerAudit.StatusCode`); pengujian gagal jika keduanya berbeda dalam Observability_And_SecurityAudit_RespectRoleAccess.
        Assert.Equal(HttpStatusCode.Forbidden, playerAudit.StatusCode);
    // Menutup scope metode Observability_And_SecurityAudit_RespectRoleAccess; bagian berikut berada di luar batas blok tersebut dalam
    // Observability_And_SecurityAudit_RespectRoleAccess.
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
    /// Helper untuk mengirim HTTP request dengan header Bearer token.
    /// </summary>
    // Mendefinisikan metode `SendAsync` dengan hasil bertipe `Task<HttpResponseMessage>`. Helper untuk mengirim HTTP request dengan header Bearer
    // token. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `method`
    // bertipe `HttpMethod` membawa nilai method; Parameter `path` bertipe `string` membawa nilai path; Parameter `accessToken` bertipe `string` membawa
    // nilai akses token.
    private async Task<HttpResponseMessage> SendAsync(HttpMethod method, string path, string accessToken)
    // Membuka scope metode SendAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam SendAsync.
    {
        // Menyiapkan variabel lokal `request` untuk data masukan permintaan yang akan divalidasi atau diteruskan ke layanan dengan objek baru bertipe
        // `HttpRequestMessage` dengan argumen (method, path). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var request = new HttpRequestMessage(method, path);
        // Memperbarui `request.Headers.Authorization` menggunakan objek baru bertipe `AuthenticationHeaderValue` dengan argumen (”Bearer”, accessToken)
        // dalam SendAsync.
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        // Mengembalikan hasil operasi asinkron memanggil `_client.SendAsync` dengan `request`; await menunggu hasil tanpa memblokir thread selama operasi
        // belum selesai kepada pemanggil dalam SendAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return await _client.SendAsync(request);
    // Menutup scope metode SendAsync; bagian berikut berada di luar batas blok tersebut dalam SendAsync.
    }

    /// <summary>
    /// Helper yang memvalidasi bahwa body respons JSON mengandung properti tertentu.
    /// </summary>
    // Mendefinisikan metode `AssertJsonHasPropertyAsync` dengan hasil bertipe `Task`. Helper yang memvalidasi bahwa body respons JSON mengandung
    // properti tertentu. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter
    // `response` bertipe `HttpResponseMessage` membawa hasil respons yang akan dibaca atau dikirim kepada pemanggil; Parameter `propertyName` bertipe
    // `string` membawa nilai property nama.
    private static async Task AssertJsonHasPropertyAsync(HttpResponseMessage response, string propertyName)
    // Membuka scope metode AssertJsonHasPropertyAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam AssertJsonHasPropertyAsync.
    {
        // Menyiapkan variabel lokal `raw` untuk nilai raw dengan hasil operasi asinkron memanggil `response.Content.ReadAsStringAsync` dengan tanpa
        // argumen; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var raw = await response.Content.ReadAsStringAsync();
        // Menyiapkan variabel lokal `document` untuk nilai document dengan memanggil `JsonDocument.Parse` dengan `raw`. Tipe variabel disimpulkan dari
        // ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var document = JsonDocument.Parse(raw);
        // Menyiapkan variabel lokal `hasProperty` untuk nilai memiliki property dengan mencari properti JSON `propertyName`, `_` pada
        // `document.RootElement` tanpa menganggap propertinya selalu tersedia. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var hasProperty = document.RootElement.TryGetProperty(propertyName, out _);
        // Menjalankan pemeriksaan bahwa `hasProperty`, `$”Respons tidak memiliki properti '{propertyName}'. Body: {raw}”` bernilai benar; pengujian gagal
        // jika kondisi tidak terpenuhi dalam AssertJsonHasPropertyAsync.
        Assert.True(hasProperty, $"Respons tidak memiliki properti '{propertyName}'. Body: {raw}");
    // Menutup scope metode AssertJsonHasPropertyAsync; bagian berikut berada di luar batas blok tersebut dalam AssertJsonHasPropertyAsync.
    }
// Menutup scope tipe ObservabilitySecurityIntegrationTests; bagian berikut berada di luar batas blok tersebut.
}
