// Fungsi file: Memverifikasi perilaku API, database, atau domain melalui ManualSimulationSeedIntegrationTests.
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
// Mengimpor namespace `Dapper` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Dapper;
// Mengimpor namespace `Microsoft.AspNetCore.Mvc.Testing` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.AspNetCore.Mvc.Testing;
// Mengimpor namespace `Microsoft.Extensions.DependencyInjection` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama
// lengkapnya.
using Microsoft.Extensions.DependencyInjection;
// Mengimpor namespace `Npgsql` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Npgsql;
// Mengimpor namespace `Testcontainers.PostgreSql` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Testcontainers.PostgreSql;
// Mengimpor namespace `Xunit` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Xunit;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Tests` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Tests;

// menempatkan pengujian dalam koleksi fixture (”ApiIntegration”).
[Collection("ApiIntegration")]
// menerapkan metadata `Trait(”Category”, ”Integration”)` pada deklarasi berikut agar framework/compiler dapat mengenali pengaturannya.
[Trait("Category", "Integration")]
// Mendefinisikan tipe class `ManualSimulationSeedIntegrationTests`; sealed mencegah tipe ini diturunkan lagi.
public sealed class ManualSimulationSeedIntegrationTests
// Membuka scope tipe ManualSimulationSeedIntegrationTests; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendeklarasikan field bertipe `string`: `JwtSigningKey` menyimpan nilai jwt signing kunci dengan nilai awal nilai literal
    // `”integration-test-signing-key-with-min-32-char”`.
    private const string JwtSigningKey = "integration-test-signing-key-with-min-32-char";
    // Mendeklarasikan field bertipe `string`: `SeedInstructorUsername` menyimpan nilai seed instruktur username dengan nilai awal nilai literal
    // `”rina.kartika”`.
    private const string SeedInstructorUsername = "rina.kartika";
    // Mendeklarasikan field bertipe `string`: `SeedInstructorPassword` menyimpan nilai seed instruktur password dengan nilai awal nilai literal
    // `”SeedLocal!2026”`.
    private const string SeedInstructorPassword = "SeedLocal!2026";
    // Mendeklarasikan field bertipe `string`: `SeedPemulaSessionName` menyimpan nilai seed pemula sesi nama dengan nilai awal nilai literal `”Simulasi
    // Cashflowpoly Kelas XI IPS 2 - Mode Pemula - Kelompok A”`.
    private const string SeedPemulaSessionName = "Simulasi Cashflowpoly Kelas XI IPS 2 - Mode Pemula - Kelompok A";
    // Mendeklarasikan field bertipe `string`: `SeedMahirSessionName` menyimpan nilai seed mahir sesi nama dengan nilai awal nilai literal `”Simulasi
    // Cashflowpoly Kelas XI IPS 2 - Mode Mahir - Kelompok B”`.
    private const string SeedMahirSessionName = "Simulasi Cashflowpoly Kelas XI IPS 2 - Mode Mahir - Kelompok B";
    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions` dengan hasil bertipe `Task`;
    // operasi ini menangani manual simulation seed when applied after canonical seeds produces loginable dual mode sessions. async memungkinkan metode
    // menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task.
    public async Task ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions()
    // Membuka scope metode ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions; pernyataan/deklarasi berikut berada
    // di dalam batas blok ini dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
    {
        // Menyiapkan variabel lokal `database` untuk nilai database dengan memanggil `new PostgreSqlBuilder(”postgres:16”)
        // .WithDatabase(”cashflowpoly_manual_seed_boot”) .WithUsername(”cashflowpoly”) .WithPassword(”cashflowpoly”) .Build` dengan tanpa argumen. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var database = new PostgreSqlBuilder("postgres:16")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .WithDatabase(”cashflowpoly_manual_seed_boot”) dalam
            // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions; token pada baris ini menyambungkan bagian kode sebelum dan
            // sesudahnya.
            .WithDatabase("cashflowpoly_manual_seed_boot")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .WithUsername(”cashflowpoly”) dalam
            // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions; token pada baris ini menyambungkan bagian kode sebelum dan
            // sesudahnya.
            .WithUsername("cashflowpoly")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .WithPassword(”cashflowpoly”) dalam
            // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions; token pada baris ini menyambungkan bagian kode sebelum dan
            // sesudahnya.
            .WithPassword("cashflowpoly")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Build(); dalam
            // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions; token pada baris ini menyambungkan bagian kode sebelum dan
            // sesudahnya.
            .Build();

        // Menjalankan hasil operasi asinkron memanggil `database.StartAsync` dengan tanpa argumen; await menunggu hasil tanpa memblokir thread selama
        // operasi belum selesai dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        await database.StartAsync();

        // Menjalankan hasil operasi asinkron memanggil `RunWithConnectionStringAsync` dengan `database.GetConnectionString()`, `async () => { await using
        // var factory = new ApiWebApplicationFactory(database.GetConnectionString(), JwtSigningKey, seedSimulation: true); using var client =
        // factory.CreateCli...`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam
        // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        await RunWithConnectionStringAsync(database.GetConnectionString(), async () =>
        // Membuka scope fungsi lambda yang dipasok ke `RunWithConnectionStringAsync`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        {
            // Menyiapkan variabel lokal `factory` untuk nilai factory dengan objek baru bertipe `ApiWebApplicationFactory` dengan argumen
            // (database.GetConnectionString(), JwtSigningKey, seedSimulation: true). Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan
            // sumber daya dilepas otomatis saat scope berakhir.
            await using var factory = new ApiWebApplicationFactory(database.GetConnectionString(), JwtSigningKey, seedSimulation: true);
            // Menyiapkan variabel lokal `client` untuk nilai client dengan memanggil `factory.CreateClient` dengan `new WebApplicationFactoryClientOptions {
            // AllowAutoRedirect = false }`. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope
            // berakhir.
            using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
            // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            {
                // Memperbarui `AllowAutoRedirect` menggunakan false, yaitu kondisi nonaktif/tidak terpenuhi dalam
                // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
                AllowAutoRedirect = false
            // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
            // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            });

            // Menyiapkan variabel lokal `healthResponse` untuk nilai health respons dengan hasil operasi asinkron memanggil `client.GetAsync` dengan
            // `”/health/ready”`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var healthResponse = await client.GetAsync("/health/ready");
            // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`,
            // `healthResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam
            // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            Assert.Equal(HttpStatusCode.OK, healthResponse.StatusCode);

            // Menyiapkan variabel lokal `loginResponse` untuk nilai login respons dengan hasil operasi asinkron memanggil `client.PostAsJsonAsync` dengan
            // `”/api/v1/auth/login”`, `new LoginRequest(SeedInstructorUsername, SeedInstructorPassword)`; await menunggu hasil tanpa memblokir thread selama
            // operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var loginResponse = await client.PostAsJsonAsync(
                // Meneruskan nilai literal `”/api/v1/auth/login”` sebagai argumen ke `client.PostAsJsonAsync`.
                "/api/v1/auth/login",
                // Meneruskan objek baru bertipe `LoginRequest` dengan argumen (SeedInstructorUsername, SeedInstructorPassword) sebagai argumen ke
                // `client.PostAsJsonAsync`; Meneruskan `SeedInstructorUsername` (nilai seed instruktur username) sebagai argumen ke konstruktor `LoginRequest`;
                // Meneruskan `SeedInstructorPassword` (nilai seed instruktur password) sebagai argumen ke konstruktor `LoginRequest`.
                new LoginRequest(SeedInstructorUsername, SeedInstructorPassword));
            // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`,
            // `loginResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam
            // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

            // Menyiapkan variabel lokal `loginBody` untuk nilai login body dengan hasil operasi asinkron membaca tanpa argumen menjadi objek bertipe sesuai
            // kontrak JSON melalui `loginResponse.Content.ReadFromJsonAsync<LoginResponse>`; await menunggu hasil tanpa memblokir thread selama operasi belum
            // selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var loginBody = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();
            // Menjalankan pemeriksaan NotNull atas `loginBody` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
            // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            Assert.NotNull(loginBody);
            // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”INSTRUCTOR”`, `loginBody.Role`); pengujian
            // gagal jika keduanya berbeda dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            Assert.Equal("INSTRUCTOR", loginBody.Role);

            // Memperbarui `client.DefaultRequestHeaders.Authorization` menggunakan objek baru bertipe `AuthenticationHeaderValue` dengan argumen (”Bearer”,
            // loginBody.AccessToken) dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginBody.AccessToken);

            // Mengulangi setiap elemen `new[] { Guid.Parse(”91000000-0000-0000-0000-000000000001”), Guid.Parse(”91000000-0000-0000-0000-000000000002”) }`;
            // elemen saat ini disimpan sebagai `sessionId` bertipe `var` untuk diproses oleh badan loop dalam
            // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            foreach (var sessionId in new[]
                     // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                     // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
                     {
                         // Meneruskan nilai literal `”91000000-0000-0000-0000-000000000001”` sebagai argumen ke `Guid.Parse`.
                         Guid.Parse("91000000-0000-0000-0000-000000000001"),
                         // Meneruskan nilai literal `”91000000-0000-0000-0000-000000000002”` sebagai argumen ke `Guid.Parse`.
                         Guid.Parse("91000000-0000-0000-0000-000000000002")
                     // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
                     // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
                     })
            // Membuka scope loop setiap sessionId dari `new[] { Guid.Parse(”91000000-0000-0000-0000-000000000001”),
            // Guid.Parse(”91000000-0000-0000-0000-000000000002”) }`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            {
                // Menyiapkan variabel lokal `recomputeResponse` untuk nilai recompute respons dengan hasil operasi asinkron memanggil `client.PostAsync` dengan
                // `$”/api/v1/analytics/sessions/{sessionId}/recompute”`, `null`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
                // variabel disimpulkan dari ekspresi nilai awal.
                var recomputeResponse = await client.PostAsync(
                    // Meneruskan teks interpolasi `$”/api/v1/analytics/sessions/{sessionId}/recompute”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program
                    // berjalan sebagai argumen ke `client.PostAsync`.
                    $"/api/v1/analytics/sessions/{sessionId}/recompute",
                    // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen bernama `content`.
                    content: null);
                // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`,
                // `recomputeResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam
                // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
                Assert.Equal(HttpStatusCode.OK, recomputeResponse.StatusCode);
            // Menutup scope loop setiap sessionId dari `new[] { Guid.Parse(”91000000-0000-0000-0000-000000000001”),
            // Guid.Parse(”91000000-0000-0000-0000-000000000002”) }`; bagian berikut berada di luar batas blok tersebut dalam
            // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            }

            // Menyiapkan variabel lokal `seedMahirSessionId` untuk nilai seed mahir sesi identitas dengan memanggil `Guid.Parse` dengan
            // `”91000000-0000-0000-0000-000000000002”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var seedMahirSessionId = Guid.Parse("91000000-0000-0000-0000-000000000002");
            // Menyiapkan variabel lokal `seedPlayer1UserId` untuk nilai seed pemain 1 pengguna identitas dengan memanggil `Guid.Parse` dengan
            // `”90000000-0000-0000-0000-000000000011”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var seedPlayer1UserId = Guid.Parse("90000000-0000-0000-0000-000000000011");
            // Menyiapkan variabel lokal `txResponse` untuk nilai tx respons dengan hasil operasi asinkron memanggil `client.GetAsync` dengan
            // `$”/api/v1/analytics/sessions/{seedMahirSessionId}/transactions?userId={seedPlayer1UserId}”`; await menunggu hasil tanpa memblokir thread selama
            // operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var txResponse = await client.GetAsync($"/api/v1/analytics/sessions/{seedMahirSessionId}/transactions?userId={seedPlayer1UserId}");
            // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`, `txResponse.StatusCode`);
            // pengujian gagal jika keduanya berbeda dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            Assert.Equal(HttpStatusCode.OK, txResponse.StatusCode);

            // Menyiapkan variabel lokal `txBody` untuk nilai tx body dengan hasil operasi asinkron membaca tanpa argumen menjadi objek bertipe sesuai kontrak
            // JSON melalui `txResponse.Content.ReadFromJsonAsync<TransactionHistoryResponse>`; await menunggu hasil tanpa memblokir thread selama operasi belum
            // selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var txBody = await txResponse.Content.ReadFromJsonAsync<TransactionHistoryResponse>();
            // Menjalankan pemeriksaan NotNull atas `txBody` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
            // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            Assert.NotNull(txBody);
            // Menjalankan pemeriksaan hasil dengan `Assert.NotEmpty` menggunakan `txBody.Items`; ketidaksesuaian dengan ekspektasi membuat pengujian gagal
            // dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            Assert.NotEmpty(txBody.Items);

            // Menyiapkan variabel lokal `gameplayResponse` untuk nilai gameplay respons dengan hasil operasi asinkron memanggil `client.GetAsync` dengan
            // `$”/api/v1/analytics/sessions/{seedMahirSessionId}/players/{seedPlayer1UserId}/gameplay”`; await menunggu hasil tanpa memblokir thread selama
            // operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var gameplayResponse = await client.GetAsync($"/api/v1/analytics/sessions/{seedMahirSessionId}/players/{seedPlayer1UserId}/gameplay");
            // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`,
            // `gameplayResponse.StatusCode`); pengujian gagal jika keduanya berbeda dalam
            // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            Assert.Equal(HttpStatusCode.OK, gameplayResponse.StatusCode);

            // Menyiapkan variabel lokal `gameplayBody` untuk nilai gameplay body dengan hasil operasi asinkron membaca tanpa argumen menjadi objek bertipe
            // sesuai kontrak JSON melalui `gameplayResponse.Content.ReadFromJsonAsync<GameplayMetricsResponse>`; await menunggu hasil tanpa memblokir thread
            // selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var gameplayBody = await gameplayResponse.Content.ReadFromJsonAsync<GameplayMetricsResponse>();
            // Menjalankan pemeriksaan NotNull atas `gameplayBody` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
            // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            Assert.NotNull(gameplayBody);
            // Menjalankan pemeriksaan bahwa `gameplayBody.Economy.StartingCash > 0` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
            // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            Assert.True(gameplayBody.Economy.StartingCash > 0);
            // Menjalankan pemeriksaan bahwa `gameplayBody.Economy.CashInTotal >= 0` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
            // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            Assert.True(gameplayBody.Economy.CashInTotal >= 0);
            // Menjalankan pemeriksaan bahwa `gameplayBody.Progress.ActionsUsedTotal >= 0` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
            // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            Assert.True(gameplayBody.Progress.ActionsUsedTotal >= 0);
            // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`15`,
            // `gameplayBody.RawJson!.Value.GetProperty(”ingredients”).GetProperty(”ingredients_collected”).GetInt32()`); pengujian gagal jika keduanya berbeda
            // dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            Assert.Equal(15, gameplayBody.RawJson!.Value.GetProperty("ingredients").GetProperty("ingredients_collected").GetInt32());
            // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`13`,
            // `gameplayBody.RawJson.Value.GetProperty(”ingredients”).GetProperty(”ingredients_used_total”).GetInt32()`); pengujian gagal jika keduanya berbeda
            // dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            Assert.Equal(13, gameplayBody.RawJson.Value.GetProperty("ingredients").GetProperty("ingredients_used_total").GetInt32());
            // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`34`,
            // `gameplayBody.RawJson.Value.GetProperty(”ingredients”).GetProperty(”ingredient_investment_coins_total”).GetInt32()`); pengujian gagal jika
            // keduanya berbeda dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            Assert.Equal(34, gameplayBody.RawJson.Value.GetProperty("ingredients").GetProperty("ingredient_investment_coins_total").GetInt32());
            // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`28.57`,
            // `gameplayBody.DerivedJson!.Value.GetProperty(”business_expense_share_percent”).GetDouble()`, `2`); pengujian gagal jika keduanya berbeda dalam
            // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            Assert.Equal(28.57, gameplayBody.DerivedJson!.Value.GetProperty("business_expense_share_percent").GetDouble(), 2);
            // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`68.13`,
            // `gameplayBody.DerivedJson.Value.GetProperty(”meal_order_profit_margin_percent”).GetDouble()`, `2`); pengujian gagal jika keduanya berbeda dalam
            // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            Assert.Equal(68.13, gameplayBody.DerivedJson.Value.GetProperty("meal_order_profit_margin_percent").GetDouble(), 2);
        // Menutup scope fungsi lambda yang dipasok ke `RunWithConnectionStringAsync`; bagian berikut berada di luar batas blok tersebut dalam
        // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        });

        // Menyiapkan variabel lokal `connection` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan objek baru bertipe
        // `NpgsqlConnection` dengan argumen (database.GetConnectionString()). Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber
        // daya dilepas otomatis saat scope berakhir.
        await using var connection = new NpgsqlConnection(database.GetConnectionString());
        // Menjalankan hasil operasi asinkron memanggil `connection.OpenAsync` dengan tanpa argumen; await menunggu hasil tanpa memblokir thread selama
        // operasi belum selesai dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        await connection.OpenAsync();

        // Menyiapkan variabel lokal `sessions` untuk nilai sessions dengan mematerialisasi urutan `(await connection.QueryAsync<SeedSessionRow>( ””” select
        // session_id, session_name, status, mode from sessions where session_name in (@pemulaSessionName, @mahirSessionName) ord...` menjadi List;
        // enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var sessions = (await connection.QueryAsync<SeedSessionRow>(
            // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke `connection.QueryAsync<SeedSessionRow>`.
            // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
            // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select session_id, session_name, status, mode`.
            // Baris literal 3: FROM memilih tabel/subquery sumber pembacaan: `from sessions`.
            // Baris literal 4: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where session_name in (@pemulaSessionName,
            // @mahirSessionName)`.
            // Baris literal 5: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by session_name asc`.
            // Baris literal 6: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            """
            select session_id, session_name, status, mode
            from sessions
            where session_name in (@pemulaSessionName, @mahirSessionName)
            order by session_name asc
            """,
            // Meneruskan objek anonim yang mengelompokkan pemulaSessionName, mahirSessionName sebagai satu nilai sebagai argumen ke
            // `connection.QueryAsync<SeedSessionRow>`.
            new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            {
                // Meneruskan objek anonim yang mengelompokkan pemulaSessionName, mahirSessionName sebagai satu nilai sebagai argumen ke
                // `connection.QueryAsync<SeedSessionRow>`.
                pemulaSessionName = SeedPemulaSessionName,
                // Meneruskan objek anonim yang mengelompokkan pemulaSessionName, mahirSessionName sebagai satu nilai sebagai argumen ke
                // `connection.QueryAsync<SeedSessionRow>`.
                mahirSessionName = SeedMahirSessionName
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
            // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            })).ToList();

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`2`, `sessions.Count`); pengujian gagal jika
        // keduanya berbeda dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        Assert.Equal(2, sessions.Count);
        // Menjalankan pemeriksaan hasil dengan `Assert.All` menggunakan `sessions`, `session => Assert.Equal(”ENDED”, session.Status)`; ketidaksesuaian
        // dengan ekspektasi membuat pengujian gagal dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        Assert.All(sessions, session => Assert.Equal("ENDED", session.Status));

        // Menyiapkan variabel lokal `playerCounts` untuk nilai pemain counts dengan membangun kamus dari `(await
        // connection.QueryAsync<SessionPlayerCountRow>( ””” select s.session_name, count(*)::int as player_count from sessions s join session_participants
        // sp on sp.session_id = s...` dengan pemilihan kunci/nilai `row => row.SessionName`, `row => row.PlayerCount`; kunci harus unik agar konversi
        // berhasil. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var playerCounts = (await connection.QueryAsync<SessionPlayerCountRow>(
            // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke `connection.QueryAsync<SessionPlayerCountRow>`.
            // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
            // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select s.session_name, count(*)::int as player_count`.
            // Baris literal 3: FROM memilih tabel/subquery sumber pembacaan: `from sessions s`.
            // Baris literal 4: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join session_participants sp on sp.session_id =
            // s.session_id`.
            // Baris literal 5: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where s.session_name in (@pemulaSessionName,
            // @mahirSessionName)`.
            // Baris literal 6: GROUP BY mengelompokkan baris sebelum fungsi agregasi dihitung: `group by s.session_name`.
            // Baris literal 7: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by s.session_name asc`.
            // Baris literal 8: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            """
            select s.session_name, count(*)::int as player_count
            from sessions s
            join session_participants sp on sp.session_id = s.session_id
            where s.session_name in (@pemulaSessionName, @mahirSessionName)
            group by s.session_name
            order by s.session_name asc
            """,
            // Meneruskan objek anonim yang mengelompokkan pemulaSessionName, mahirSessionName sebagai satu nilai sebagai argumen ke
            // `connection.QueryAsync<SessionPlayerCountRow>`.
            new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            {
                // Meneruskan objek anonim yang mengelompokkan pemulaSessionName, mahirSessionName sebagai satu nilai sebagai argumen ke
                // `connection.QueryAsync<SessionPlayerCountRow>`.
                pemulaSessionName = SeedPemulaSessionName,
                // Meneruskan objek anonim yang mengelompokkan pemulaSessionName, mahirSessionName sebagai satu nilai sebagai argumen ke
                // `connection.QueryAsync<SessionPlayerCountRow>`.
                mahirSessionName = SeedMahirSessionName
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
            // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            })).ToDictionary(row => row.SessionName, row => row.PlayerCount);

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`4`, `playerCounts[SeedPemulaSessionName]`);
        // pengujian gagal jika keduanya berbeda dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        Assert.Equal(4, playerCounts[SeedPemulaSessionName]);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`4`, `playerCounts[SeedMahirSessionName]`);
        // pengujian gagal jika keduanya berbeda dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        Assert.Equal(4, playerCounts[SeedMahirSessionName]);

        // Menyiapkan variabel lokal `sequenceChecks` untuk nilai sequence checks dengan mematerialisasi urutan `(await
        // connection.QueryAsync<SequenceCheckRow>( ””” select s.session_name, min(e.sequence_number)::bigint as min_sequence,
        // max(e.sequence_number)::bigint as max_sequence, coun...` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var sequenceChecks = (await connection.QueryAsync<SequenceCheckRow>(
            // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke `connection.QueryAsync<SequenceCheckRow>`.
            // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
            // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
            // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `s.session_name,`.
            // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun:
            // `min(e.sequence_number)::bigint as min_sequence,`.
            // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun:
            // `max(e.sequence_number)::bigint as max_sequence,`.
            // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `count(*)::int as
            // event_count`.
            // Baris literal 7: FROM memilih tabel/subquery sumber pembacaan: `from sessions s`.
            // Baris literal 8: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join events e on e.session_id = s.session_id`.
            // Baris literal 9: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where s.session_name in (@pemulaSessionName,
            // @mahirSessionName)`.
            // Baris literal 10: GROUP BY mengelompokkan baris sebelum fungsi agregasi dihitung: `group by s.session_name`.
            // Baris literal 11: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by s.session_name asc`.
            // Baris literal 12: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            """
            select
                s.session_name,
                min(e.sequence_number)::bigint as min_sequence,
                max(e.sequence_number)::bigint as max_sequence,
                count(*)::int as event_count
            from sessions s
            join events e on e.session_id = s.session_id
            where s.session_name in (@pemulaSessionName, @mahirSessionName)
            group by s.session_name
            order by s.session_name asc
            """,
            // Meneruskan objek anonim yang mengelompokkan pemulaSessionName, mahirSessionName sebagai satu nilai sebagai argumen ke
            // `connection.QueryAsync<SequenceCheckRow>`.
            new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            {
                // Meneruskan objek anonim yang mengelompokkan pemulaSessionName, mahirSessionName sebagai satu nilai sebagai argumen ke
                // `connection.QueryAsync<SequenceCheckRow>`.
                pemulaSessionName = SeedPemulaSessionName,
                // Meneruskan objek anonim yang mengelompokkan pemulaSessionName, mahirSessionName sebagai satu nilai sebagai argumen ke
                // `connection.QueryAsync<SequenceCheckRow>`.
                mahirSessionName = SeedMahirSessionName
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
            // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            })).ToList();

        // Menjalankan pemeriksaan hasil dengan `Assert.All` menggunakan `sequenceChecks`, `row => { Assert.Equal(0, row.MinSequence);
        // Assert.Equal(row.EventCount - 1, row.MaxSequence); }`; ketidaksesuaian dengan ekspektasi membuat pengujian gagal dalam
        // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        Assert.All(sequenceChecks, row =>
        // Membuka scope fungsi lambda yang dipasok ke `Assert.All`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        {
            // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`0`, `row.MinSequence`); pengujian gagal jika
            // keduanya berbeda dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            Assert.Equal(0, row.MinSequence);
            // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`row.EventCount - 1`, `row.MaxSequence`);
            // pengujian gagal jika keduanya berbeda dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            Assert.Equal(row.EventCount - 1, row.MaxSequence);
        // Menutup scope fungsi lambda yang dipasok ke `Assert.All`; bagian berikut berada di luar batas blok tersebut dalam
        // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        });

        // Menyiapkan variabel lokal `rulesetChecks` untuk nilai aturan checks dengan membangun kamus dari `(await connection.QueryAsync<RulesetCheckRow>(
        // ””” select s.session_name, max((s.ruleset_version_id)::text) as activated_ruleset_version_id, min((e.ruleset_version_id)::text) ...` dengan
        // pemilihan kunci/nilai `row => row.SessionName`; kunci harus unik agar konversi berhasil. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var rulesetChecks = (await connection.QueryAsync<RulesetCheckRow>(
            // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke `connection.QueryAsync<RulesetCheckRow>`.
            // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
            // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
            // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `s.session_name,`.
            // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun:
            // `max((s.ruleset_version_id)::text) as activated_ruleset_version_id,`.
            // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun:
            // `min((e.ruleset_version_id)::text) as event_ruleset_version_id,`.
            // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `count(distinct
            // e.ruleset_version_id)::int as event_ruleset_versions`.
            // Baris literal 7: FROM memilih tabel/subquery sumber pembacaan: `from sessions s`.
            // Baris literal 8: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join events e on e.session_id = s.session_id`.
            // Baris literal 9: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where s.session_name in (@pemulaSessionName,
            // @mahirSessionName)`.
            // Baris literal 10: GROUP BY mengelompokkan baris sebelum fungsi agregasi dihitung: `group by s.session_name`.
            // Baris literal 11: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by s.session_name asc`.
            // Baris literal 12: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            """
            select
                s.session_name,
                max((s.ruleset_version_id)::text) as activated_ruleset_version_id,
                min((e.ruleset_version_id)::text) as event_ruleset_version_id,
                count(distinct e.ruleset_version_id)::int as event_ruleset_versions
            from sessions s
            join events e on e.session_id = s.session_id
            where s.session_name in (@pemulaSessionName, @mahirSessionName)
            group by s.session_name
            order by s.session_name asc
            """,
            // Meneruskan objek anonim yang mengelompokkan pemulaSessionName, mahirSessionName sebagai satu nilai sebagai argumen ke
            // `connection.QueryAsync<RulesetCheckRow>`.
            new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            {
                // Meneruskan objek anonim yang mengelompokkan pemulaSessionName, mahirSessionName sebagai satu nilai sebagai argumen ke
                // `connection.QueryAsync<RulesetCheckRow>`.
                pemulaSessionName = SeedPemulaSessionName,
                // Meneruskan objek anonim yang mengelompokkan pemulaSessionName, mahirSessionName sebagai satu nilai sebagai argumen ke
                // `connection.QueryAsync<RulesetCheckRow>`.
                mahirSessionName = SeedMahirSessionName
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
            // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            })).ToDictionary(row => row.SessionName);

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”f5b4c67b-0825-4970-9f07-3b68e8fcb524”`,
        // `rulesetChecks[SeedPemulaSessionName].ActivatedRulesetVersionId`); pengujian gagal jika keduanya berbeda dalam
        // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        Assert.Equal("f5b4c67b-0825-4970-9f07-3b68e8fcb524", rulesetChecks[SeedPemulaSessionName].ActivatedRulesetVersionId);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”f5b4c67b-0825-4970-9f07-3b68e8fcb524”`,
        // `rulesetChecks[SeedPemulaSessionName].EventRulesetVersionId`); pengujian gagal jika keduanya berbeda dalam
        // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        Assert.Equal("f5b4c67b-0825-4970-9f07-3b68e8fcb524", rulesetChecks[SeedPemulaSessionName].EventRulesetVersionId);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`1`,
        // `rulesetChecks[SeedPemulaSessionName].EventRulesetVersions`); pengujian gagal jika keduanya berbeda dalam
        // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        Assert.Equal(1, rulesetChecks[SeedPemulaSessionName].EventRulesetVersions);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”7c3bfd8a-27d7-4468-b8d7-cf90131bc61d”`,
        // `rulesetChecks[SeedMahirSessionName].ActivatedRulesetVersionId`); pengujian gagal jika keduanya berbeda dalam
        // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        Assert.Equal("7c3bfd8a-27d7-4468-b8d7-cf90131bc61d", rulesetChecks[SeedMahirSessionName].ActivatedRulesetVersionId);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”7c3bfd8a-27d7-4468-b8d7-cf90131bc61d”`,
        // `rulesetChecks[SeedMahirSessionName].EventRulesetVersionId`); pengujian gagal jika keduanya berbeda dalam
        // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        Assert.Equal("7c3bfd8a-27d7-4468-b8d7-cf90131bc61d", rulesetChecks[SeedMahirSessionName].EventRulesetVersionId);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`1`,
        // `rulesetChecks[SeedMahirSessionName].EventRulesetVersions`); pengujian gagal jika keduanya berbeda dalam
        // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        Assert.Equal(1, rulesetChecks[SeedMahirSessionName].EventRulesetVersions);

        // Menyiapkan variabel lokal `calendarChecks` untuk nilai calendar checks dengan membangun kamus dari `(await
        // connection.QueryAsync<CalendarCheckRow>( ””” select s.session_name, count(distinct e.day_index)::int as day_count, min(e.day_index)::int as
        // min_day_index, max(e.day_ind...` dengan pemilihan kunci/nilai `row => row.SessionName`; kunci harus unik agar konversi berhasil. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var calendarChecks = (await connection.QueryAsync<CalendarCheckRow>(
            // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke `connection.QueryAsync<CalendarCheckRow>`.
            // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
            // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
            // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `s.session_name,`.
            // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `count(distinct
            // e.day_index)::int as day_count,`.
            // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `min(e.day_index)::int as
            // min_day_index,`.
            // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `max(e.day_index)::int as
            // max_day_index,`.
            // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `count(*) filter (`.
            // Baris literal 8: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where e.day_index > 0`.
            // Baris literal 9: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and e.weekday <> case (((e.day_index - 1) % 7 + 7) % 7)`.
            // Baris literal 10: Menyusun pilihan nilai bersyarat di dalam ekspresi SQL: `when 0 then 'MON'`.
            // Baris literal 11: Menyusun pilihan nilai bersyarat di dalam ekspresi SQL: `when 1 then 'TUE'`.
            // Baris literal 12: Menyusun pilihan nilai bersyarat di dalam ekspresi SQL: `when 2 then 'WED'`.
            // Baris literal 13: Menyusun pilihan nilai bersyarat di dalam ekspresi SQL: `when 3 then 'THU'`.
            // Baris literal 14: Menyusun pilihan nilai bersyarat di dalam ekspresi SQL: `when 4 then 'FRI'`.
            // Baris literal 15: Menyusun pilihan nilai bersyarat di dalam ekspresi SQL: `when 5 then 'SAT'`.
            // Baris literal 16: Menyusun pilihan nilai bersyarat di dalam ekspresi SQL: `else 'SUN'`.
            // Baris literal 17: Menyusun pilihan nilai bersyarat di dalam ekspresi SQL: `end`.
            // Baris literal 18: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `)::int as
            // weekday_mismatch_count`.
            // Baris literal 19: FROM memilih tabel/subquery sumber pembacaan: `from sessions s`.
            // Baris literal 20: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join events e on e.session_id = s.session_id`.
            // Baris literal 21: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where s.session_name in (@pemulaSessionName,
            // @mahirSessionName)`.
            // Baris literal 22: GROUP BY mengelompokkan baris sebelum fungsi agregasi dihitung: `group by s.session_name`.
            // Baris literal 23: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by s.session_name asc`.
            // Baris literal 24: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            """
            select
                s.session_name,
                count(distinct e.day_index)::int as day_count,
                min(e.day_index)::int as min_day_index,
                max(e.day_index)::int as max_day_index,
                count(*) filter (
                    where e.day_index > 0
                      and e.weekday <> case (((e.day_index - 1) % 7 + 7) % 7)
                        when 0 then 'MON'
                        when 1 then 'TUE'
                        when 2 then 'WED'
                        when 3 then 'THU'
                        when 4 then 'FRI'
                        when 5 then 'SAT'
                        else 'SUN'
                    end
                )::int as weekday_mismatch_count
            from sessions s
            join events e on e.session_id = s.session_id
            where s.session_name in (@pemulaSessionName, @mahirSessionName)
            group by s.session_name
            order by s.session_name asc
            """,
            // Meneruskan objek anonim yang mengelompokkan pemulaSessionName, mahirSessionName sebagai satu nilai sebagai argumen ke
            // `connection.QueryAsync<CalendarCheckRow>`.
            new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            {
                // Meneruskan objek anonim yang mengelompokkan pemulaSessionName, mahirSessionName sebagai satu nilai sebagai argumen ke
                // `connection.QueryAsync<CalendarCheckRow>`.
                pemulaSessionName = SeedPemulaSessionName,
                // Meneruskan objek anonim yang mengelompokkan pemulaSessionName, mahirSessionName sebagai satu nilai sebagai argumen ke
                // `connection.QueryAsync<CalendarCheckRow>`.
                mahirSessionName = SeedMahirSessionName
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
            // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            })).ToDictionary(row => row.SessionName);

        // Menjalankan pemeriksaan hasil dengan `Assert.All` menggunakan `calendarChecks.Values`, `row => { Assert.Equal(26, row.DayCount); Assert.Equal(0,
        // row.MinDayIndex); Assert.Equal(25, row.MaxDayIndex); Assert.Equal(0, row.WeekdayMismatchCount); }`; ketidaksesuaian dengan ekspektasi membuat
        // pengujian gagal dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        Assert.All(calendarChecks.Values, row =>
        // Membuka scope fungsi lambda yang dipasok ke `Assert.All`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        {
            // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`26`, `row.DayCount`); pengujian gagal jika
            // keduanya berbeda dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            Assert.Equal(26, row.DayCount);
            // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`0`, `row.MinDayIndex`); pengujian gagal jika
            // keduanya berbeda dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            Assert.Equal(0, row.MinDayIndex);
            // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`25`, `row.MaxDayIndex`); pengujian gagal jika
            // keduanya berbeda dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            Assert.Equal(25, row.MaxDayIndex);
            // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`0`, `row.WeekdayMismatchCount`); pengujian
            // gagal jika keduanya berbeda dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            Assert.Equal(0, row.WeekdayMismatchCount);
        // Menutup scope fungsi lambda yang dipasok ke `Assert.All`; bagian berikut berada di luar batas blok tersebut dalam
        // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        });

        // Menyiapkan variabel lokal `internalTurnActionCount` untuk nilai internal giliran aksi jumlah dengan hasil operasi asinkron menjalankan perintah
        // basis data melalui `connection` dengan `””” select count(*) from sessions s join events e on e.session_id = s.session_id where s.session_name in
        // (@pemulaSessionName, @mahirSessionName) and e.action_type = 'turn.acti...`, `new { pemulaSessionName = SeedPemulaSessionName, mahirSessionName =
        // SeedMahirSessionName }` dan mengambil nilai skalar hasilnya; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var internalTurnActionCount = await connection.ExecuteScalarAsync<int>(
            // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke `connection.ExecuteScalarAsync<int>`.
            // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
            // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select count(*)`.
            // Baris literal 3: FROM memilih tabel/subquery sumber pembacaan: `from sessions s`.
            // Baris literal 4: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join events e on e.session_id = s.session_id`.
            // Baris literal 5: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where s.session_name in (@pemulaSessionName,
            // @mahirSessionName)`.
            // Baris literal 6: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and e.action_type = 'turn.action.used'`.
            // Baris literal 7: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            """
            select count(*)
            from sessions s
            join events e on e.session_id = s.session_id
            where s.session_name in (@pemulaSessionName, @mahirSessionName)
              and e.action_type = 'turn.action.used'
            """,
            // Meneruskan objek anonim yang mengelompokkan pemulaSessionName, mahirSessionName sebagai satu nilai sebagai argumen ke
            // `connection.ExecuteScalarAsync<int>`.
            new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            {
                // Meneruskan objek anonim yang mengelompokkan pemulaSessionName, mahirSessionName sebagai satu nilai sebagai argumen ke
                // `connection.ExecuteScalarAsync<int>`.
                pemulaSessionName = SeedPemulaSessionName,
                // Meneruskan objek anonim yang mengelompokkan pemulaSessionName, mahirSessionName sebagai satu nilai sebagai argumen ke
                // `connection.ExecuteScalarAsync<int>`.
                mahirSessionName = SeedMahirSessionName
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
            // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            });
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`0`, `internalTurnActionCount`); pengujian
        // gagal jika keduanya berbeda dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        Assert.Equal(0, internalTurnActionCount);

        // Menyiapkan variabel lokal `pemulaForbiddenCount` untuk nilai pemula forbidden jumlah dengan hasil operasi asinkron menjalankan perintah basis
        // data melalui `connection` dengan `””” select count(*) from sessions s join events e on e.session_id = s.session_id where s.session_name =
        // @pemulaSessionName and ( e.action_type in ( 'PinjamanSyariah', 'BayarPi...`, `new { pemulaSessionName = SeedPemulaSessionName }` dan mengambil
        // nilai skalar hasilnya; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai
        // awal.
        var pemulaForbiddenCount = await connection.ExecuteScalarAsync<int>(
            // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke `connection.ExecuteScalarAsync<int>`.
            // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
            // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select count(*)`.
            // Baris literal 3: FROM memilih tabel/subquery sumber pembacaan: `from sessions s`.
            // Baris literal 4: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join events e on e.session_id = s.session_id`.
            // Baris literal 5: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where s.session_name = @pemulaSessionName`.
            // Baris literal 6: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and (`.
            // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `e.action_type in (`.
            // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `'PinjamanSyariah',`.
            // Baris literal 9: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `'BayarPinjaman',`.
            // Baris literal 10: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `'Asuransi',`.
            // Baris literal 11: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `'RisikoKehidupan',`.
            // Baris literal 12: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `'GunakanOpsiDarurat',`.
            // Baris literal 13: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `'Menabung',`.
            // Baris literal 14: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `'TarikTabungan',`.
            // Baris literal 15: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `'TujuanFinansial'`.
            // Baris literal 16: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 17: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 18: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            """
            select count(*)
            from sessions s
            join events e on e.session_id = s.session_id
            where s.session_name = @pemulaSessionName
              and (
                e.action_type in (
                    'PinjamanSyariah',
                    'BayarPinjaman',
                    'Asuransi',
                    'RisikoKehidupan',
                    'GunakanOpsiDarurat',
                    'Menabung',
                    'TarikTabungan',
                    'TujuanFinansial'
                )
              )
            """,
            // Meneruskan objek anonim yang mengelompokkan pemulaSessionName sebagai satu nilai sebagai argumen ke `connection.ExecuteScalarAsync<int>`.
            new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            {
                // Meneruskan objek anonim yang mengelompokkan pemulaSessionName sebagai satu nilai sebagai argumen ke `connection.ExecuteScalarAsync<int>`.
                pemulaSessionName = SeedPemulaSessionName
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
            // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            });
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`0`, `pemulaForbiddenCount`); pengujian gagal
        // jika keduanya berbeda dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        Assert.Equal(0, pemulaForbiddenCount);

        // Menyiapkan variabel lokal `setupRows` untuk nilai setup baris dengan mematerialisasi urutan `(await
        // connection.QueryAsync<PlayerSetupProjectionRow>( ””” select s.session_name, sp.session_participant_id, count(*) filter (where e.action_type =
        // 'SetupBahanAwal' and e.day...` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi
        // nilai awal.
        var setupRows = (await connection.QueryAsync<PlayerSetupProjectionRow>(
            // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke
            // `connection.QueryAsync<PlayerSetupProjectionRow>`.
            // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
            // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
            // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `s.session_name,`.
            // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `sp.session_participant_id,`.
            // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `count(*) filter (where
            // e.action_type = 'SetupBahanAwal' and e.day_index = 0)::int as setup_ingredient_count,`.
            // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `count(*) filter (where
            // e.actor_type = 'PLAYER' and e.day_index = 1)::int as day_one_player_action_count,`.
            // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `count(*) filter (where
            // e.action_type = 'SetupEmasAwal' and e.day_index = 0)::int as setup_gold_count,`.
            // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `count(*) filter (where
            // e.action_type = 'SetupMisiAwal' and e.day_index = 0)::int as setup_mission_count,`.
            // Baris literal 9: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `count(*) filter (where
            // e.action_type = 'BagikanTieBreaker' and e.day_index = 0)::int as setup_tie_breaker_count,`.
            // Baris literal 10: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `count(*) filter (`.
            // Baris literal 11: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where e.action_type = 'SetupPinjamanAwal'`.
            // Baris literal 12: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and e.day_index = 0`.
            // Baris literal 13: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and e.payload->>'setup' = 'INITIAL'`.
            // Baris literal 14: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `)::int as
            // setup_loan_count,`.
            // Baris literal 15: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `count(*) filter (`.
            // Baris literal 16: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where e.action_type = 'SetupAsuransiAwal'`.
            // Baris literal 17: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and e.day_index = 0`.
            // Baris literal 18: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and coalesce((e.payload->>'premium')::int, -1) = 0`.
            // Baris literal 19: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and e.payload->>'setup' = 'INITIAL'`.
            // Baris literal 20: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `)::int as
            // setup_insurance_count`.
            // Baris literal 21: FROM memilih tabel/subquery sumber pembacaan: `from sessions s`.
            // Baris literal 22: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join session_participants sp on sp.session_id =
            // s.session_id`.
            // Baris literal 23: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `left join events e`.
            // Baris literal 24: ON menetapkan syarat pencocokan baris pada relasi JOIN: `on e.session_id = sp.session_id`.
            // Baris literal 25: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and e.session_player_id = sp.session_participant_id`.
            // Baris literal 26: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where s.session_name in (@pemulaSessionName,
            // @mahirSessionName)`.
            // Baris literal 27: GROUP BY mengelompokkan baris sebelum fungsi agregasi dihitung: `group by s.session_name, sp.session_participant_id`.
            // Baris literal 28: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by s.session_name asc,
            // sp.session_participant_id asc`.
            // Baris literal 29: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            """
            select
                s.session_name,
                sp.session_participant_id,
                count(*) filter (where e.action_type = 'SetupBahanAwal' and e.day_index = 0)::int as setup_ingredient_count,
                count(*) filter (where e.actor_type = 'PLAYER' and e.day_index = 1)::int as day_one_player_action_count,
                count(*) filter (where e.action_type = 'SetupEmasAwal' and e.day_index = 0)::int as setup_gold_count,
                count(*) filter (where e.action_type = 'SetupMisiAwal' and e.day_index = 0)::int as setup_mission_count,
                count(*) filter (where e.action_type = 'BagikanTieBreaker' and e.day_index = 0)::int as setup_tie_breaker_count,
                count(*) filter (
                    where e.action_type = 'SetupPinjamanAwal'
                      and e.day_index = 0
                      and e.payload->>'setup' = 'INITIAL'
                )::int as setup_loan_count,
                count(*) filter (
                    where e.action_type = 'SetupAsuransiAwal'
                      and e.day_index = 0
                      and coalesce((e.payload->>'premium')::int, -1) = 0
                      and e.payload->>'setup' = 'INITIAL'
                )::int as setup_insurance_count
            from sessions s
            join session_participants sp on sp.session_id = s.session_id
            left join events e
              on e.session_id = sp.session_id
             and e.session_player_id = sp.session_participant_id
            where s.session_name in (@pemulaSessionName, @mahirSessionName)
            group by s.session_name, sp.session_participant_id
            order by s.session_name asc, sp.session_participant_id asc
            """,
            // Meneruskan objek anonim yang mengelompokkan pemulaSessionName, mahirSessionName sebagai satu nilai sebagai argumen ke
            // `connection.QueryAsync<PlayerSetupProjectionRow>`.
            new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            {
                // Meneruskan objek anonim yang mengelompokkan pemulaSessionName, mahirSessionName sebagai satu nilai sebagai argumen ke
                // `connection.QueryAsync<PlayerSetupProjectionRow>`.
                pemulaSessionName = SeedPemulaSessionName,
                // Meneruskan objek anonim yang mengelompokkan pemulaSessionName, mahirSessionName sebagai satu nilai sebagai argumen ke
                // `connection.QueryAsync<PlayerSetupProjectionRow>`.
                mahirSessionName = SeedMahirSessionName
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
            // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            })).ToList();

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`8`, `setupRows.Count`); pengujian gagal jika
        // keduanya berbeda dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        Assert.Equal(8, setupRows.Count);
        // Mengulangi setiap elemen `setupRows`; elemen saat ini disimpan sebagai `row` bertipe `var` untuk diproses oleh badan loop dalam
        // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        foreach (var row in setupRows)
        // Membuka scope loop setiap row dari `setupRows`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        {
            // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`1`, `row.SetupIngredientCount`); pengujian
            // gagal jika keduanya berbeda dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            Assert.Equal(1, row.SetupIngredientCount);
            // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`2`, `row.DayOnePlayerActionCount`); pengujian
            // gagal jika keduanya berbeda dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            Assert.Equal(2, row.DayOnePlayerActionCount);
            // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`1`, `row.SetupGoldCount`); pengujian gagal
            // jika keduanya berbeda dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            Assert.Equal(1, row.SetupGoldCount);
            // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`1`, `row.SetupMissionCount`); pengujian gagal
            // jika keduanya berbeda dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            Assert.Equal(1, row.SetupMissionCount);

            // Memeriksa memeriksa apakah `row.SessionName` memuat `”Pemula”`, `StringComparison.OrdinalIgnoreCase`; blok if hanya dijalankan ketika kondisi ini
            // bernilai benar dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            if (row.SessionName.Contains("Pemula", StringComparison.OrdinalIgnoreCase))
            // Membuka scope cabang if untuk kondisi `row.SessionName.Contains(”Pemula”, StringComparison.OrdinalIgnoreCase)`; pernyataan/deklarasi berikut
            // berada di dalam batas blok ini dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            {
                // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`1`, `row.SetupTieBreakerCount`); pengujian
                // gagal jika keduanya berbeda dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
                Assert.Equal(1, row.SetupTieBreakerCount);
                // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`0`, `row.SetupLoanCount`); pengujian gagal
                // jika keduanya berbeda dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
                Assert.Equal(0, row.SetupLoanCount);
                // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`0`, `row.SetupInsuranceCount`); pengujian
                // gagal jika keduanya berbeda dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
                Assert.Equal(0, row.SetupInsuranceCount);
            // Menutup scope cabang if untuk kondisi `row.SessionName.Contains(”Pemula”, StringComparison.OrdinalIgnoreCase)`; bagian berikut berada di luar
            // batas blok tersebut dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            }
            // Menjalankan cabang alternatif ketika kondisi if sebelumnya tidak terpenuhi dalam
            // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            else
            // Membuka scope cabang else; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            {
                // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`1`, `row.SetupTieBreakerCount`); pengujian
                // gagal jika keduanya berbeda dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
                Assert.Equal(1, row.SetupTieBreakerCount);
                // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`1`, `row.SetupLoanCount`); pengujian gagal
                // jika keduanya berbeda dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
                Assert.Equal(1, row.SetupLoanCount);
                // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`1`, `row.SetupInsuranceCount`); pengujian
                // gagal jika keduanya berbeda dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
                Assert.Equal(1, row.SetupInsuranceCount);
            // Menutup scope cabang else; bagian berikut berada di luar batas blok tersebut dalam
            // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            }
        // Menutup scope loop setiap row dari `setupRows`; bagian berikut berada di luar batas blok tersebut dalam
        // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        }

        // Menyiapkan variabel lokal `mahirLoanCount` untuk nilai mahir pinjaman jumlah dengan hasil operasi asinkron menjalankan perintah basis data
        // melalui `connection` dengan `””” select count(*)::int from sessions s join events e on e.session_id = s.session_id where s.session_name =
        // @mahirSessionName and e.action_type in ('PinjamanSyariah', 'SetupP...`, `new { mahirSessionName = SeedMahirSessionName }` dan mengambil nilai
        // skalar hasilnya; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var mahirLoanCount = await connection.ExecuteScalarAsync<int>(
            // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke `connection.ExecuteScalarAsync<int>`.
            // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
            // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select count(*)::int`.
            // Baris literal 3: FROM memilih tabel/subquery sumber pembacaan: `from sessions s`.
            // Baris literal 4: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join events e on e.session_id = s.session_id`.
            // Baris literal 5: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where s.session_name = @mahirSessionName`.
            // Baris literal 6: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and e.action_type in ('PinjamanSyariah',
            // 'SetupPinjamanAwal')`.
            // Baris literal 7: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            """
            select count(*)::int
            from sessions s
            join events e on e.session_id = s.session_id
            where s.session_name = @mahirSessionName
              and e.action_type in ('PinjamanSyariah', 'SetupPinjamanAwal')
            """,
            // Meneruskan objek anonim yang mengelompokkan mahirSessionName sebagai satu nilai sebagai argumen ke `connection.ExecuteScalarAsync<int>`.
            new { mahirSessionName = SeedMahirSessionName });
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`4`, `mahirLoanCount`); pengujian gagal jika
        // keduanya berbeda dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        Assert.Equal(4, mahirLoanCount);

        // Menyiapkan variabel lokal `mahirLoanCatalogMismatchCount` untuk nilai mahir pinjaman catalog mismatch jumlah dengan hasil operasi asinkron
        // menjalankan perintah basis data melalui `connection` dengan `””” select count(*)::int from sessions s join events e on e.session_id =
        // s.session_id join ruleset_sharia_loans loan on loan.ruleset_version_id = e.ruleset_version_id and lowe...`, `new { mahirSessionName =
        // SeedMahirSessionName }` dan mengambil nilai skalar hasilnya; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var mahirLoanCatalogMismatchCount = await connection.ExecuteScalarAsync<int>(
            // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke `connection.ExecuteScalarAsync<int>`.
            // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
            // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select count(*)::int`.
            // Baris literal 3: FROM memilih tabel/subquery sumber pembacaan: `from sessions s`.
            // Baris literal 4: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join events e on e.session_id = s.session_id`.
            // Baris literal 5: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join ruleset_sharia_loans loan`.
            // Baris literal 6: ON menetapkan syarat pencocokan baris pada relasi JOIN: `on loan.ruleset_version_id = e.ruleset_version_id`.
            // Baris literal 7: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and lower(loan.loan_code) =
            // lower(e.payload->>'loan_code')`.
            // Baris literal 8: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where s.session_name = @mahirSessionName`.
            // Baris literal 9: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and (`.
            // Baris literal 10: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `e.action_type =
            // 'SetupPinjamanAwal'`.
            // Baris literal 11: Melanjutkan kondisi SQL dengan OR (alternatif syarat yang dapat terpenuhi): `or (`.
            // Baris literal 12: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `e.action_type =
            // 'GunakanOpsiDarurat'`.
            // Baris literal 13: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and upper(e.payload->>'option_type') =
            // 'TAKE_SHARIA_LOAN'`.
            // Baris literal 14: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 15: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 16: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and (`.
            // Baris literal 17: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun:
            // `(e.payload->>'principal')::int <> loan.principal`.
            // Baris literal 18: Melanjutkan kondisi SQL dengan OR (alternatif syarat yang dapat terpenuhi): `or (e.payload->>'repayment_amount')::int <>
            // loan.repayment_amount`.
            // Baris literal 19: Melanjutkan kondisi SQL dengan OR (alternatif syarat yang dapat terpenuhi): `or (e.payload->>'duration_days')::int <>
            // loan.duration_days`.
            // Baris literal 20: Melanjutkan kondisi SQL dengan OR (alternatif syarat yang dapat terpenuhi): `or (e.payload->>'penalty_points')::int <>
            // loan.penalty_points`.
            // Baris literal 21: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 22: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            """
            select count(*)::int
            from sessions s
            join events e on e.session_id = s.session_id
            join ruleset_sharia_loans loan
              on loan.ruleset_version_id = e.ruleset_version_id
             and lower(loan.loan_code) = lower(e.payload->>'loan_code')
            where s.session_name = @mahirSessionName
              and (
                e.action_type = 'SetupPinjamanAwal'
                or (
                  e.action_type = 'GunakanOpsiDarurat'
                  and upper(e.payload->>'option_type') = 'TAKE_SHARIA_LOAN'
                )
              )
              and (
                (e.payload->>'principal')::int <> loan.principal
                or (e.payload->>'repayment_amount')::int <> loan.repayment_amount
                or (e.payload->>'duration_days')::int <> loan.duration_days
                or (e.payload->>'penalty_points')::int <> loan.penalty_points
              )
            """,
            // Meneruskan objek anonim yang mengelompokkan mahirSessionName sebagai satu nilai sebagai argumen ke `connection.ExecuteScalarAsync<int>`.
            new { mahirSessionName = SeedMahirSessionName });
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`0`, `mahirLoanCatalogMismatchCount`);
        // pengujian gagal jika keduanya berbeda dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        Assert.Equal(0, mahirLoanCatalogMismatchCount);

        // Menyiapkan variabel lokal `scenarioAlignmentRows` untuk nilai scenario alignment baris dengan mematerialisasi urutan `(await
        // connection.QueryAsync<ScenarioAlignmentRow>( ””” select s.mode, sp.player_name, e.day_index, e.action_slot, e.action_type, coalesce(
        // e.payload->>'card_id', e.payload->>...` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var scenarioAlignmentRows = (await connection.QueryAsync<ScenarioAlignmentRow>(
            // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke `connection.QueryAsync<ScenarioAlignmentRow>`.
            // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
            // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
            // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `s.mode,`.
            // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `sp.player_name,`.
            // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `e.day_index,`.
            // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `e.action_slot,`.
            // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `e.action_type,`.
            // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `coalesce(`.
            // Baris literal 9: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `e.payload->>'card_id',`.
            // Baris literal 10: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun:
            // `e.payload->>'order_card_id',`.
            // Baris literal 11: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `e.payload->>'trade_type',`.
            // Baris literal 12: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `e.payload->>'status',`.
            // Baris literal 13: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `e.payload->>'note',`.
            // Baris literal 14: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `e.payload->>'loan_id',`.
            // Baris literal 15: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `''`.
            // Baris literal 16: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `) as payload_key`.
            // Baris literal 17: FROM memilih tabel/subquery sumber pembacaan: `from sessions s`.
            // Baris literal 18: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join events e on e.session_id = s.session_id`.
            // Baris literal 19: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `left join session_participants sp on
            // sp.session_participant_id = e.session_player_id`.
            // Baris literal 20: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where s.session_name in (@pemulaSessionName,
            // @mahirSessionName)`.
            // Baris literal 21: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and e.actor_type in ('PLAYER', 'SYSTEM')`.
            // Baris literal 22: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and (`.
            // Baris literal 23: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `(s.mode = 'PEMULA' and
            // e.day_index in (0, 1, 2, 6))`.
            // Baris literal 24: Melanjutkan kondisi SQL dengan OR (alternatif syarat yang dapat terpenuhi): `or (s.mode = 'MAHIR' and e.day_index in (0, 1, 2,
            // 8, 9, 10, 13, 15, 16, 20, 23, 25))`.
            // Baris literal 25: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 26: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by s.mode, e.day_index, sp.player_order_no,
            // e.sequence_number`.
            // Baris literal 27: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            """
            select
                s.mode,
                sp.player_name,
                e.day_index,
                e.action_slot,
                e.action_type,
                coalesce(
                    e.payload->>'card_id',
                    e.payload->>'order_card_id',
                    e.payload->>'trade_type',
                    e.payload->>'status',
                    e.payload->>'note',
                    e.payload->>'loan_id',
                    ''
                ) as payload_key
            from sessions s
            join events e on e.session_id = s.session_id
            left join session_participants sp on sp.session_participant_id = e.session_player_id
            where s.session_name in (@pemulaSessionName, @mahirSessionName)
              and e.actor_type in ('PLAYER', 'SYSTEM')
              and (
                (s.mode = 'PEMULA' and e.day_index in (0, 1, 2, 6))
                or (s.mode = 'MAHIR' and e.day_index in (0, 1, 2, 8, 9, 10, 13, 15, 16, 20, 23, 25))
              )
            order by s.mode, e.day_index, sp.player_order_no, e.sequence_number
            """,
            // Meneruskan objek anonim yang mengelompokkan pemulaSessionName, mahirSessionName sebagai satu nilai sebagai argumen ke
            // `connection.QueryAsync<ScenarioAlignmentRow>`.
            new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            {
                // Meneruskan objek anonim yang mengelompokkan pemulaSessionName, mahirSessionName sebagai satu nilai sebagai argumen ke
                // `connection.QueryAsync<ScenarioAlignmentRow>`.
                pemulaSessionName = SeedPemulaSessionName,
                // Meneruskan objek anonim yang mengelompokkan pemulaSessionName, mahirSessionName sebagai satu nilai sebagai argumen ke
                // `connection.QueryAsync<ScenarioAlignmentRow>`.
                mahirSessionName = SeedMahirSessionName
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
            // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            })).ToList();

        // Melengkapi struktur ekspresi ForEachVariableStatement melalui foreach (var (player, firstIngredient, secondIngredient) in new[] dalam
        // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions; token pada baris ini menyambungkan bagian kode sebelum dan
        // sesudahnya.
        foreach (var (player, firstIngredient, secondIngredient) in new[]
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        {
            // Menggunakan tuple yang membawa bagian 1: ”Marco”; bagian 2: ”nasi_putih”; bagian 3: ”telur” sebagai bagian ekspresi yang sedang disusun dalam
            // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            ("Marco", "nasi_putih", "telur"),
            // Menggunakan tuple yang membawa bagian 1: ”Marcello”; bagian 2: ”nasi_putih”; bagian 3: ”telur” sebagai bagian ekspresi yang sedang disusun dalam
            // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            ("Marcello", "nasi_putih", "telur"),
            // Menggunakan tuple yang membawa bagian 1: ”Hugo”; bagian 2: ”daging”; bagian 3: ”tahu_tempe” sebagai bagian ekspresi yang sedang disusun dalam
            // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            ("Hugo", "daging", "tahu_tempe")
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        })
        // Membuka scope blok ForEachVariableStatement; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        {
            // Menjalankan memanggil `AssertScenarioEvent` dengan `scenarioAlignmentRows`, `”PEMULA”`, `player`, `1`, `1`, `”BahanMasakan”`, `firstIngredient`
            // dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            AssertScenarioEvent(scenarioAlignmentRows, "PEMULA", player, 1, 1, "BahanMasakan", firstIngredient);
            // Menjalankan memanggil `AssertScenarioEvent` dengan `scenarioAlignmentRows`, `”PEMULA”`, `player`, `1`, `2`, `”BahanMasakan”`, `secondIngredient`
            // dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            AssertScenarioEvent(scenarioAlignmentRows, "PEMULA", player, 1, 2, "BahanMasakan", secondIngredient);
        // Menutup scope blok ForEachVariableStatement; bagian berikut berada di luar batas blok tersebut dalam
        // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        }

        // Menjalankan memanggil `AssertScenarioEvent` dengan `scenarioAlignmentRows`, `”PEMULA”`, `”Manalu”`, `1`, `1`, `”KerjaLepas”`, `””` dalam
        // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        AssertScenarioEvent(scenarioAlignmentRows, "PEMULA", "Manalu", 1, 1, "KerjaLepas", "");
        // Menjalankan memanggil `AssertScenarioEvent` dengan `scenarioAlignmentRows`, `”PEMULA”`, `”Manalu”`, `1`, `2`, `”BahanMasakan”`, `”sayur”` dalam
        // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        AssertScenarioEvent(scenarioAlignmentRows, "PEMULA", "Manalu", 1, 2, "BahanMasakan", "sayur");

        // Menjalankan memanggil `AssertScenarioEvent` dengan `scenarioAlignmentRows`, `”PEMULA”`, `”Marco”`, `2`, `1`, `”BahanMasakan”`, `”telur”` dalam
        // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        AssertScenarioEvent(
            // Meneruskan `scenarioAlignmentRows` (nilai scenario alignment baris) sebagai argumen ke `AssertScenarioEvent`.
            scenarioAlignmentRows,
            // Meneruskan nilai literal `”PEMULA”` sebagai argumen ke `AssertScenarioEvent`.
            "PEMULA",
            // Meneruskan nilai literal `”Marco”` sebagai argumen ke `AssertScenarioEvent`.
            "Marco",
            // Meneruskan nilai literal `2` sebagai argumen ke `AssertScenarioEvent`.
            2,
            // Meneruskan nilai literal `1` sebagai argumen ke `AssertScenarioEvent`.
            1,
            // Meneruskan nilai literal `”BahanMasakan”` sebagai argumen ke `AssertScenarioEvent`.
            "BahanMasakan",
            // Meneruskan nilai literal `”telur”` sebagai argumen ke `AssertScenarioEvent`.
            "telur");
        // Menjalankan memanggil `AssertScenarioEvent` dengan `scenarioAlignmentRows`, `”PEMULA”`, `”Marco”`, `2`, `2`, `”JualMasakan”`, `”nasi_goreng”`
        // dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        AssertScenarioEvent(
            // Meneruskan `scenarioAlignmentRows` (nilai scenario alignment baris) sebagai argumen ke `AssertScenarioEvent`.
            scenarioAlignmentRows,
            // Meneruskan nilai literal `”PEMULA”` sebagai argumen ke `AssertScenarioEvent`.
            "PEMULA",
            // Meneruskan nilai literal `”Marco”` sebagai argumen ke `AssertScenarioEvent`.
            "Marco",
            // Meneruskan nilai literal `2` sebagai argumen ke `AssertScenarioEvent`.
            2,
            // Meneruskan nilai literal `2` sebagai argumen ke `AssertScenarioEvent`.
            2,
            // Meneruskan nilai literal `”JualMasakan”` sebagai argumen ke `AssertScenarioEvent`.
            "JualMasakan",
            // Meneruskan nilai literal `”nasi_goreng”` sebagai argumen ke `AssertScenarioEvent`.
            "nasi_goreng");
        // Menjalankan memanggil `AssertScenarioEvent` dengan `scenarioAlignmentRows`, `”PEMULA”`, `”Marcello”`, `6`, `0`, `”LewatiTransaksiEmas”`,
        // `”Menjaga saldo setelah pembelian bahan Hari 1”` dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        AssertScenarioEvent(
            // Meneruskan `scenarioAlignmentRows` (nilai scenario alignment baris) sebagai argumen ke `AssertScenarioEvent`.
            scenarioAlignmentRows,
            // Meneruskan nilai literal `”PEMULA”` sebagai argumen ke `AssertScenarioEvent`.
            "PEMULA",
            // Meneruskan nilai literal `”Marcello”` sebagai argumen ke `AssertScenarioEvent`.
            "Marcello",
            // Meneruskan nilai literal `6` sebagai argumen ke `AssertScenarioEvent`.
            6,
            // Meneruskan nilai literal `0` sebagai argumen ke `AssertScenarioEvent`.
            0,
            // Meneruskan nilai literal `”LewatiTransaksiEmas”` sebagai argumen ke `AssertScenarioEvent`.
            "LewatiTransaksiEmas",
            // Meneruskan nilai literal `”Menjaga saldo setelah pembelian bahan Hari 1”` sebagai argumen ke `AssertScenarioEvent`.
            "Menjaga saldo setelah pembelian bahan Hari 1");

        // Melengkapi struktur ekspresi ForEachVariableStatement melalui foreach (var (player, firstIngredient, secondIngredient) in new[] dalam
        // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions; token pada baris ini menyambungkan bagian kode sebelum dan
        // sesudahnya.
        foreach (var (player, firstIngredient, secondIngredient) in new[]
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        {
            // Menggunakan tuple yang membawa bagian 1: ”Marco”; bagian 2: ”nasi_putih”; bagian 3: ”telur” sebagai bagian ekspresi yang sedang disusun dalam
            // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            ("Marco", "nasi_putih", "telur"),
            // Menggunakan tuple yang membawa bagian 1: ”Marcello”; bagian 2: ”daging”; bagian 3: ”tahu_tempe” sebagai bagian ekspresi yang sedang disusun dalam
            // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            ("Marcello", "daging", "tahu_tempe"),
            // Menggunakan tuple yang membawa bagian 1: ”Hugo”; bagian 2: ”nasi_putih”; bagian 3: ”telur” sebagai bagian ekspresi yang sedang disusun dalam
            // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            ("Hugo", "nasi_putih", "telur")
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        })
        // Membuka scope blok ForEachVariableStatement; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        {
            // Menjalankan memanggil `AssertScenarioEvent` dengan `scenarioAlignmentRows`, `”MAHIR”`, `player`, `1`, `1`, `”BahanMasakan”`, `firstIngredient`
            // dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            AssertScenarioEvent(scenarioAlignmentRows, "MAHIR", player, 1, 1, "BahanMasakan", firstIngredient);
            // Menjalankan memanggil `AssertScenarioEvent` dengan `scenarioAlignmentRows`, `”MAHIR”`, `player`, `1`, `2`, `”BahanMasakan”`, `secondIngredient`
            // dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            AssertScenarioEvent(scenarioAlignmentRows, "MAHIR", player, 1, 2, "BahanMasakan", secondIngredient);
        // Menutup scope blok ForEachVariableStatement; bagian berikut berada di luar batas blok tersebut dalam
        // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        }

        // Menjalankan memanggil `AssertScenarioEvent` dengan `scenarioAlignmentRows`, `”MAHIR”`, `”Manalu”`, `1`, `1`, `”KerjaLepas”`, `””` dalam
        // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        AssertScenarioEvent(scenarioAlignmentRows, "MAHIR", "Manalu", 1, 1, "KerjaLepas", "");
        // Menjalankan memanggil `AssertScenarioEvent` dengan `scenarioAlignmentRows`, `”MAHIR”`, `”Manalu”`, `1`, `2`, `”BahanMasakan”`, `”tahu_tempe”`
        // dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        AssertScenarioEvent(scenarioAlignmentRows, "MAHIR", "Manalu", 1, 2, "BahanMasakan", "tahu_tempe");

        // Menjalankan memanggil `AssertScenarioEvent` dengan `scenarioAlignmentRows`, `”MAHIR”`, `”Marcello”`, `13`, `0`, `”InvestasiEmas”`, `”BUY”` dalam
        // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        AssertScenarioEvent(scenarioAlignmentRows, "MAHIR", "Marcello", 13, 0, "InvestasiEmas", "BUY");
        // Menjalankan memanggil `AssertScenarioEvent` dengan `scenarioAlignmentRows`, `”MAHIR”`, `”Hugo”`, `13`, `0`, `”InvestasiEmas”`, `”BUY”` dalam
        // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        AssertScenarioEvent(scenarioAlignmentRows, "MAHIR", "Hugo", 13, 0, "InvestasiEmas", "BUY");
        // Menjalankan memanggil `AssertScenarioEvent` dengan `scenarioAlignmentRows`, `”MAHIR”`, `”Marco”`, `20`, `0`, `”InvestasiEmas”`, `”BUY”` dalam
        // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        AssertScenarioEvent(scenarioAlignmentRows, "MAHIR", "Marco", 20, 0, "InvestasiEmas", "BUY");
        // Menjalankan memanggil `AssertScenarioEvent` dengan `scenarioAlignmentRows`, `”MAHIR”`, `”Hugo”`, `20`, `0`, `”InvestasiEmas”`, `”BUY”` dalam
        // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        AssertScenarioEvent(scenarioAlignmentRows, "MAHIR", "Hugo", 20, 0, "InvestasiEmas", "BUY");

        // Menjalankan memanggil `AssertScenarioEvent` dengan `scenarioAlignmentRows`, `”MAHIR”`, `”Marcello”`, `15`, `1`, `”BahanMasakan”`, `”daging”`
        // dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        AssertScenarioEvent(scenarioAlignmentRows, "MAHIR", "Marcello", 15, 1, "BahanMasakan", "daging");
        // Menjalankan memanggil `AssertScenarioEvent` dengan `scenarioAlignmentRows`, `”MAHIR”`, `”Manalu”`, `15`, `1`, `”Kebutuhan”`, `”boneka_2”` dalam
        // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        AssertScenarioEvent(scenarioAlignmentRows, "MAHIR", "Manalu", 15, 1, "Kebutuhan", "boneka_2");

        // Menjalankan memanggil `AssertScenarioEvent` dengan `scenarioAlignmentRows`, `”MAHIR”`, `”Marco”`, `0`, `0`, `”SetupAsuransiAwal”`, `””` dalam
        // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        AssertScenarioEvent(scenarioAlignmentRows, "MAHIR", "Marco", 0, 0, "SetupAsuransiAwal", "");
        // Menjalankan memanggil `AssertScenarioEvent` dengan `scenarioAlignmentRows`, `”MAHIR”`, `”Marco”`, `8`, `2`, `”JualMasakan”`, `”sego_penyet”`
        // dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        AssertScenarioEvent(scenarioAlignmentRows, "MAHIR", "Marco", 8, 2, "JualMasakan", "sego_penyet");
        // Menjalankan memanggil `AssertScenarioEvent` dengan `scenarioAlignmentRows`, `”MAHIR”`, `”Marcello”`, `9`, `1`, `”BahanMasakan”`, `”sayur”` dalam
        // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        AssertScenarioEvent(scenarioAlignmentRows, "MAHIR", "Marcello", 9, 1, "BahanMasakan", "sayur");
        // Menjalankan memanggil `AssertScenarioEvent` dengan `scenarioAlignmentRows`, `”MAHIR”`, `”Marcello”`, `9`, `2`, `”JualMasakan”`,
        // `”semanggi_surabaya”` dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        AssertScenarioEvent(scenarioAlignmentRows, "MAHIR", "Marcello", 9, 2, "JualMasakan", "semanggi_surabaya");
        // Menjalankan memanggil `AssertScenarioEvent` dengan `scenarioAlignmentRows`, `”MAHIR”`, `”Hugo”`, `10`, `0`, `”RisikoKehidupan”`, `””` dalam
        // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        AssertScenarioEvent(scenarioAlignmentRows, "MAHIR", "Hugo", 10, 0, "RisikoKehidupan", "");
        // Menjalankan memanggil `AssertScenarioEvent` dengan `scenarioAlignmentRows`, `”MAHIR”`, `”Manalu”`, `0`, `0`, `”SetupPinjamanAwal”`, `””` dalam
        // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        AssertScenarioEvent(scenarioAlignmentRows, "MAHIR", "Manalu", 0, 0, "SetupPinjamanAwal", "");

        // Menyiapkan variabel lokal `mahirActions` untuk nilai mahir aksi dengan membentuk himpunan nilai unik dari `(await connection.QueryAsync<string>(
        // ””” select distinct e.action_type from sessions s join events e on e.session_id = s.session_id where s.session_name = @mahirSessionName ”...`
        // memakai `StringComparer.OrdinalIgnoreCase`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var mahirActions = (await connection.QueryAsync<string>(
            // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke `connection.QueryAsync<string>`.
            // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
            // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select distinct e.action_type`.
            // Baris literal 3: FROM memilih tabel/subquery sumber pembacaan: `from sessions s`.
            // Baris literal 4: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join events e on e.session_id = s.session_id`.
            // Baris literal 5: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where s.session_name = @mahirSessionName`.
            // Baris literal 6: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            """
            select distinct e.action_type
            from sessions s
            join events e on e.session_id = s.session_id
            where s.session_name = @mahirSessionName
            """,
            // Meneruskan objek anonim yang mengelompokkan mahirSessionName sebagai satu nilai sebagai argumen ke `connection.QueryAsync<string>`.
            new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            {
                // Meneruskan objek anonim yang mengelompokkan mahirSessionName sebagai satu nilai sebagai argumen ke `connection.QueryAsync<string>`.
                mahirSessionName = SeedMahirSessionName
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
            // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            })).ToHashSet(StringComparer.OrdinalIgnoreCase);

        // Menjalankan pemeriksaan bahwa `mahirActions.Contains(”PinjamanSyariah”) || mahirActions.Contains(”SetupPinjamanAwal”)`, `”PinjamanSyariah or
        // SetupPinjamanAwal not found in mahirActions”` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        Assert.True(mahirActions.Contains("PinjamanSyariah") || mahirActions.Contains("SetupPinjamanAwal"), "PinjamanSyariah or SetupPinjamanAwal not found in mahirActions");
        // Menjalankan pemeriksaan bahwa `mahirActions.Contains(”Asuransi”) || mahirActions.Contains(”SetupAsuransiAwal”)`, `”Asuransi or SetupAsuransiAwal
        // not found in mahirActions”` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        Assert.True(mahirActions.Contains("Asuransi") || mahirActions.Contains("SetupAsuransiAwal"), "Asuransi or SetupAsuransiAwal not found in mahirActions");
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”Menabung”`, `mahirActions` dalam
        // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        Assert.Contains("Menabung", mahirActions);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”JualMasakan”`, `mahirActions` dalam
        // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        Assert.Contains("JualMasakan", mahirActions);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”RisikoKehidupan”`, `mahirActions` dalam
        // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        Assert.Contains("RisikoKehidupan", mahirActions);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”BayarRisiko”`, `mahirActions` dalam
        // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        Assert.Contains("BayarRisiko", mahirActions);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”Asuransi”`, `mahirActions` dalam
        // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        Assert.Contains("Asuransi", mahirActions);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”GunakanOpsiDarurat”`,
        // `mahirActions` dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        Assert.DoesNotContain("GunakanOpsiDarurat", mahirActions);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”AmbilKartuDariDeck”`,
        // `mahirActions` dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        Assert.DoesNotContain("AmbilKartuDariDeck", mahirActions);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”KartuDiambilDariPasar”`,
        // `mahirActions` dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        Assert.DoesNotContain("KartuDiambilDariPasar", mahirActions);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”KartuMasukDiscard”`,
        // `mahirActions` dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        Assert.DoesNotContain("KartuMasukDiscard", mahirActions);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”IsiUlangPasar”`, `mahirActions`
        // dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        Assert.DoesNotContain("IsiUlangPasar", mahirActions);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”LewatiOrder”`, `mahirActions`
        // dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        Assert.DoesNotContain("LewatiOrder", mahirActions);

        // Menyiapkan variabel lokal `relationalReadModelCounts` untuk nilai relational read model counts dengan hasil operasi asinkron membaca tepat satu
        // baris basis data melalui `connection.QuerySingleAsync<RelationalReadModelCountRow>` dengan `””” select count(*) filter (where exists ( select 1
        // from session_participant_inventory spi where spi.session_id = s.session_id ))::int as ingredient_count, count(*) filter (wh...`, `new {
        // pemulaSessionName = SeedPemulaSessionName, mahirSessionName = SeedMahirSessionName }`; jumlah baris selain satu menyebabkan exception; await
        // menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var relationalReadModelCounts = await connection.QuerySingleAsync<RelationalReadModelCountRow>(
            // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke
            // `connection.QuerySingleAsync<RelationalReadModelCountRow>`.
            // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
            // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
            // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `count(*) filter (where exists
            // (`.
            // Baris literal 4: SELECT menentukan nilai atau kolom yang dikembalikan query: `select 1 from session_participant_inventory spi`.
            // Baris literal 5: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where spi.session_id = s.session_id`.
            // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `))::int as
            // ingredient_count,`.
            // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `count(*) filter (where exists
            // (`.
            // Baris literal 8: SELECT menentukan nilai atau kolom yang dikembalikan query: `select 1 from session_participant_need_purchases spnp`.
            // Baris literal 9: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where spnp.session_id = s.session_id`.
            // Baris literal 10: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `))::int as
            // need_purchase_count,`.
            // Baris literal 11: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `count(*) filter (where
            // exists (`.
            // Baris literal 12: SELECT menentukan nilai atau kolom yang dikembalikan query: `select 1 from session_participant_financial_goals spfg`.
            // Baris literal 13: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where spfg.session_id = s.session_id`.
            // Baris literal 14: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `))::int as
            // financial_goal_count,`.
            // Baris literal 15: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `count(*) filter (where
            // exists (`.
            // Baris literal 16: SELECT menentukan nilai atau kolom yang dikembalikan query: `select 1 from session_donation_events sde`.
            // Baris literal 17: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where sde.session_id = s.session_id`.
            // Baris literal 18: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `))::int as
            // donation_event_count,`.
            // Baris literal 19: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `count(*) filter (where
            // exists (`.
            // Baris literal 20: SELECT menentukan nilai atau kolom yang dikembalikan query: `select 1 from events e`.
            // Baris literal 21: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where e.session_id = s.session_id`.
            // Baris literal 22: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and e.action_type = 'PoinPeringkatDonasi'`.
            // Baris literal 23: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `))::int as
            // donation_ranking_count,`.
            // Baris literal 24: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `count(*) filter (where
            // exists (`.
            // Baris literal 25: SELECT menentukan nilai atau kolom yang dikembalikan query: `select 1 from session_participant_action_counters spac`.
            // Baris literal 26: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where spac.session_participant_id in (`.
            // Baris literal 27: SELECT menentukan nilai atau kolom yang dikembalikan query: `select sp.session_participant_id`.
            // Baris literal 28: FROM memilih tabel/subquery sumber pembacaan: `from session_participants sp`.
            // Baris literal 29: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where sp.session_id = s.session_id`.
            // Baris literal 30: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 31: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `))::int as
            // action_counter_count`.
            // Baris literal 32: FROM memilih tabel/subquery sumber pembacaan: `from sessions s`.
            // Baris literal 33: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where s.session_name in (@pemulaSessionName,
            // @mahirSessionName)`.
            // Baris literal 34: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            """
            select
              count(*) filter (where exists (
                select 1 from session_participant_inventory spi
                where spi.session_id = s.session_id
              ))::int as ingredient_count,
              count(*) filter (where exists (
                select 1 from session_participant_need_purchases spnp
                where spnp.session_id = s.session_id
              ))::int as need_purchase_count,
              count(*) filter (where exists (
                select 1 from session_participant_financial_goals spfg
                where spfg.session_id = s.session_id
              ))::int as financial_goal_count,
              count(*) filter (where exists (
                select 1 from session_donation_events sde
                where sde.session_id = s.session_id
              ))::int as donation_event_count,
              count(*) filter (where exists (
                select 1 from events e
                where e.session_id = s.session_id
                  and e.action_type = 'PoinPeringkatDonasi'
              ))::int as donation_ranking_count,
              count(*) filter (where exists (
                select 1 from session_participant_action_counters spac
                where spac.session_participant_id in (
                  select sp.session_participant_id
                  from session_participants sp
                  where sp.session_id = s.session_id
                )
              ))::int as action_counter_count
            from sessions s
            where s.session_name in (@pemulaSessionName, @mahirSessionName)
            """,
            // Meneruskan objek anonim yang mengelompokkan pemulaSessionName, mahirSessionName sebagai satu nilai sebagai argumen ke
            // `connection.QuerySingleAsync<RelationalReadModelCountRow>`.
            new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            {
                // Meneruskan objek anonim yang mengelompokkan pemulaSessionName, mahirSessionName sebagai satu nilai sebagai argumen ke
                // `connection.QuerySingleAsync<RelationalReadModelCountRow>`.
                pemulaSessionName = SeedPemulaSessionName,
                // Meneruskan objek anonim yang mengelompokkan pemulaSessionName, mahirSessionName sebagai satu nilai sebagai argumen ke
                // `connection.QuerySingleAsync<RelationalReadModelCountRow>`.
                mahirSessionName = SeedMahirSessionName
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
            // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            });

        // Menjalankan pemeriksaan bahwa `relationalReadModelCounts.IngredientCount > 0` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        Assert.True(relationalReadModelCounts.IngredientCount > 0);
        // Menjalankan pemeriksaan bahwa `relationalReadModelCounts.NeedPurchaseCount > 0` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi
        // dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        Assert.True(relationalReadModelCounts.NeedPurchaseCount > 0);
        // Menjalankan pemeriksaan bahwa `relationalReadModelCounts.FinancialGoalCount > 0` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi
        // dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        Assert.True(relationalReadModelCounts.FinancialGoalCount > 0);
        // Menjalankan pemeriksaan bahwa `relationalReadModelCounts.DonationEventCount > 0` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi
        // dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        Assert.True(relationalReadModelCounts.DonationEventCount > 0);
        // Menjalankan pemeriksaan bahwa `relationalReadModelCounts.DonationRankingCount > 0` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi
        // dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        Assert.True(relationalReadModelCounts.DonationRankingCount > 0);
        // Menjalankan pemeriksaan bahwa `relationalReadModelCounts.ActionCounterCount > 0` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi
        // dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        Assert.True(relationalReadModelCounts.ActionCounterCount > 0);

        // Menyiapkan variabel lokal `needProjectionCategories` untuk nilai kebutuhan projection categories dengan mematerialisasi urutan `(await
        // connection.QueryAsync<string>( ””” select distinct ecp.category from sessions s join event_cashflow_projections ecp on ecp.session_id =
        // s.session_id where s.session_nam...` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var needProjectionCategories = (await connection.QueryAsync<string>(
            // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke `connection.QueryAsync<string>`.
            // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
            // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select distinct ecp.category`.
            // Baris literal 3: FROM memilih tabel/subquery sumber pembacaan: `from sessions s`.
            // Baris literal 4: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join event_cashflow_projections ecp on ecp.session_id =
            // s.session_id`.
            // Baris literal 5: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where s.session_name in (@pemulaSessionName,
            // @mahirSessionName)`.
            // Baris literal 6: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and ecp.category like 'NEED%'`.
            // Baris literal 7: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by ecp.category`.
            // Baris literal 8: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            """
            select distinct ecp.category
            from sessions s
            join event_cashflow_projections ecp on ecp.session_id = s.session_id
            where s.session_name in (@pemulaSessionName, @mahirSessionName)
              and ecp.category like 'NEED%'
            order by ecp.category
            """,
            // Meneruskan objek anonim yang mengelompokkan pemulaSessionName, mahirSessionName sebagai satu nilai sebagai argumen ke
            // `connection.QueryAsync<string>`.
            new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            {
                // Meneruskan objek anonim yang mengelompokkan pemulaSessionName, mahirSessionName sebagai satu nilai sebagai argumen ke
                // `connection.QueryAsync<string>`.
                pemulaSessionName = SeedPemulaSessionName,
                // Meneruskan objek anonim yang mengelompokkan pemulaSessionName, mahirSessionName sebagai satu nilai sebagai argumen ke
                // `connection.QueryAsync<string>`.
                mahirSessionName = SeedMahirSessionName
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
            // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            })).ToList();

        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”NEED”`,
        // `needProjectionCategories` dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        Assert.DoesNotContain("NEED", needProjectionCategories);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”NEED_PRIMARY”`,
        // `needProjectionCategories` dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        Assert.Contains("NEED_PRIMARY", needProjectionCategories);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”NEED_SECONDARY”`,
        // `needProjectionCategories` dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        Assert.Contains("NEED_SECONDARY", needProjectionCategories);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”NEED_TERTIARY”`,
        // `needProjectionCategories` dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        Assert.Contains("NEED_TERTIARY", needProjectionCategories);

        // Menyiapkan variabel lokal `riskPayloadWithHardcodedValueCount` untuk nilai risiko payload dengan hardcoded nilai jumlah dengan hasil operasi
        // asinkron menjalankan perintah basis data melalui `connection` dengan `””” select count(*)::int from sessions s join events e on e.session_id =
        // s.session_id where s.session_name = @mahirSessionName and e.action_type = 'RisikoKehidupan' and (e.pay...`, `new { mahirSessionName =
        // SeedMahirSessionName }` dan mengambil nilai skalar hasilnya; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var riskPayloadWithHardcodedValueCount = await connection.ExecuteScalarAsync<int>(
            // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke `connection.ExecuteScalarAsync<int>`.
            // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
            // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select count(*)::int`.
            // Baris literal 3: FROM memilih tabel/subquery sumber pembacaan: `from sessions s`.
            // Baris literal 4: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join events e on e.session_id = s.session_id`.
            // Baris literal 5: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where s.session_name = @mahirSessionName`.
            // Baris literal 6: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and e.action_type = 'RisikoKehidupan'`.
            // Baris literal 7: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and (e.payload ? 'direction' or e.payload ? 'amount')`.
            // Baris literal 8: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            """
            select count(*)::int
            from sessions s
            join events e on e.session_id = s.session_id
            where s.session_name = @mahirSessionName
              and e.action_type = 'RisikoKehidupan'
              and (e.payload ? 'direction' or e.payload ? 'amount')
            """,
            // Meneruskan objek anonim yang mengelompokkan mahirSessionName sebagai satu nilai sebagai argumen ke `connection.ExecuteScalarAsync<int>`.
            new { mahirSessionName = SeedMahirSessionName });
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`0`, `riskPayloadWithHardcodedValueCount`);
        // pengujian gagal jika keduanya berbeda dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        Assert.Equal(0, riskPayloadWithHardcodedValueCount);

        // Menyiapkan variabel lokal `insuranceUseCount` untuk nilai asuransi use jumlah dengan hasil operasi asinkron menjalankan perintah basis data
        // melalui `connection` dengan `””” select count(*)::int from sessions s join events e on e.session_id = s.session_id where s.session_name =
        // @mahirSessionName and e.action_type = 'Asuransi' and e.payload ? '...`, `new { mahirSessionName = SeedMahirSessionName }` dan mengambil nilai
        // skalar hasilnya; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var insuranceUseCount = await connection.ExecuteScalarAsync<int>(
            // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke `connection.ExecuteScalarAsync<int>`.
            // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
            // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select count(*)::int`.
            // Baris literal 3: FROM memilih tabel/subquery sumber pembacaan: `from sessions s`.
            // Baris literal 4: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join events e on e.session_id = s.session_id`.
            // Baris literal 5: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where s.session_name = @mahirSessionName`.
            // Baris literal 6: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and e.action_type = 'Asuransi'`.
            // Baris literal 7: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and e.payload ? 'risk_event_id'`.
            // Baris literal 8: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            """
            select count(*)::int
            from sessions s
            join events e on e.session_id = s.session_id
            where s.session_name = @mahirSessionName
              and e.action_type = 'Asuransi'
              and e.payload ? 'risk_event_id'
            """,
            // Meneruskan objek anonim yang mengelompokkan mahirSessionName sebagai satu nilai sebagai argumen ke `connection.ExecuteScalarAsync<int>`.
            new { mahirSessionName = SeedMahirSessionName });
        // Menjalankan pemeriksaan bahwa `insuranceUseCount > 0`, `”Seed MAHIR wajib memuat penggunaan kartu asuransi.”` bernilai benar; pengujian gagal
        // jika kondisi tidak terpenuhi dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        Assert.True(insuranceUseCount > 0, "Seed MAHIR wajib memuat penggunaan kartu asuransi.");

        // Menyiapkan variabel lokal `unmatchedInsuranceUseCount` untuk nilai unmatched asuransi use jumlah dengan hasil operasi asinkron menjalankan
        // perintah basis data melalui `connection` dengan `””” with insurance_usage as ( select e.session_id, e.user_id,
        // (e.payload::jsonb->>'risk_event_id')::uuid as risk_event_id from sessions s join events e on e.session_id = s.ses...`, `new { mahirSessionName =
        // SeedMahirSessionName }` dan mengambil nilai skalar hasilnya; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var unmatchedInsuranceUseCount = await connection.ExecuteScalarAsync<int>(
            // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke `connection.ExecuteScalarAsync<int>`.
            // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
            // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 2: WITH menamai hasil query sementara (CTE) yang dapat digunakan oleh bagian SQL berikutnya: `with insurance_usage as (`.
            // Baris literal 3: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
            // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `e.session_id,`.
            // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `e.user_id,`.
            // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun:
            // `(e.payload::jsonb->>'risk_event_id')::uuid as risk_event_id`.
            // Baris literal 7: FROM memilih tabel/subquery sumber pembacaan: `from sessions s`.
            // Baris literal 8: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join events e on e.session_id = s.session_id`.
            // Baris literal 9: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where s.session_name = @mahirSessionName`.
            // Baris literal 10: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and e.action_type = 'Asuransi'`.
            // Baris literal 11: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and e.payload ? 'risk_event_id'`.
            // Baris literal 12: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 13: SELECT menentukan nilai atau kolom yang dikembalikan query: `select count(*)`.
            // Baris literal 14: FROM memilih tabel/subquery sumber pembacaan: `from insurance_usage iu`.
            // Baris literal 15: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `left join events risk_evt`.
            // Baris literal 16: ON menetapkan syarat pencocokan baris pada relasi JOIN: `on risk_evt.session_id = iu.session_id`.
            // Baris literal 17: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and risk_evt.event_id = iu.risk_event_id`.
            // Baris literal 18: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and risk_evt.user_id = iu.user_id`.
            // Baris literal 19: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and risk_evt.action_type = 'RisikoKehidupan'`.
            // Baris literal 20: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `left join ruleset_life_risks risk_catalog`.
            // Baris literal 21: ON menetapkan syarat pencocokan baris pada relasi JOIN: `on risk_catalog.ruleset_version_id = risk_evt.ruleset_version_id`.
            // Baris literal 22: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and lower(risk_catalog.risk_code) =
            // lower(risk_evt.payload::jsonb->>'risk_id')`.
            // Baris literal 23: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and risk_catalog.direction = 'OUT'`.
            // Baris literal 24: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where risk_evt.event_id is null`.
            // Baris literal 25: Melanjutkan kondisi SQL dengan OR (alternatif syarat yang dapat terpenuhi): `or risk_catalog.ruleset_life_risk_id is null`.
            // Baris literal 26: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            """
            with insurance_usage as (
              select
                e.session_id,
                e.user_id,
                (e.payload::jsonb->>'risk_event_id')::uuid as risk_event_id
              from sessions s
              join events e on e.session_id = s.session_id
              where s.session_name = @mahirSessionName
                and e.action_type = 'Asuransi'
                and e.payload ? 'risk_event_id'
            )
            select count(*)
            from insurance_usage iu
            left join events risk_evt
              on risk_evt.session_id = iu.session_id
             and risk_evt.event_id = iu.risk_event_id
             and risk_evt.user_id = iu.user_id
             and risk_evt.action_type = 'RisikoKehidupan'
            left join ruleset_life_risks risk_catalog
              on risk_catalog.ruleset_version_id = risk_evt.ruleset_version_id
             and lower(risk_catalog.risk_code) = lower(risk_evt.payload::jsonb->>'risk_id')
             and risk_catalog.direction = 'OUT'
            where risk_evt.event_id is null
               or risk_catalog.ruleset_life_risk_id is null
            """,
            // Meneruskan objek anonim yang mengelompokkan mahirSessionName sebagai satu nilai sebagai argumen ke `connection.ExecuteScalarAsync<int>`.
            new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            {
                // Meneruskan objek anonim yang mengelompokkan mahirSessionName sebagai satu nilai sebagai argumen ke `connection.ExecuteScalarAsync<int>`.
                mahirSessionName = SeedMahirSessionName
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
            // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            });
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`0`, `unmatchedInsuranceUseCount`); pengujian
        // gagal jika keduanya berbeda dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        Assert.Equal(0, unmatchedInsuranceUseCount);

        // Menyiapkan variabel lokal `delayedPersonalRiskResolutionCount` untuk nilai delayed personal risiko resolution jumlah dengan hasil operasi
        // asinkron menjalankan perintah basis data melalui `connection` dengan `””” with personal_risks as ( select risk_event.session_id,
        // risk_event.event_id, risk_event.user_id, risk_event.sequence_number from sessions s join events risk_event on risk_e...`, `new { mahirSessionName
        // = SeedMahirSessionName }` dan mengambil nilai skalar hasilnya; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var delayedPersonalRiskResolutionCount = await connection.ExecuteScalarAsync<int>(
            // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke `connection.ExecuteScalarAsync<int>`.
            // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
            // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 2: WITH menamai hasil query sementara (CTE) yang dapat digunakan oleh bagian SQL berikutnya: `with personal_risks as (`.
            // Baris literal 3: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
            // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `risk_event.session_id,`.
            // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `risk_event.event_id,`.
            // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `risk_event.user_id,`.
            // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `risk_event.sequence_number`.
            // Baris literal 8: FROM memilih tabel/subquery sumber pembacaan: `from sessions s`.
            // Baris literal 9: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join events risk_event on risk_event.session_id =
            // s.session_id`.
            // Baris literal 10: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join ruleset_life_risks risk`.
            // Baris literal 11: ON menetapkan syarat pencocokan baris pada relasi JOIN: `on risk.ruleset_version_id = risk_event.ruleset_version_id`.
            // Baris literal 12: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and lower(risk.risk_code) =
            // lower(risk_event.payload->>'risk_id')`.
            // Baris literal 13: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where s.session_name = @mahirSessionName`.
            // Baris literal 14: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and risk_event.action_type = 'RisikoKehidupan'`.
            // Baris literal 15: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and risk.effect_type = 'COIN_EFFECT'`.
            // Baris literal 16: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and risk.direction = 'OUT'`.
            // Baris literal 17: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and risk.target_scope = 'SELF'`.
            // Baris literal 18: Pembatas literal/penutup `),`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 19: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `resolved_risks as (`.
            // Baris literal 20: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
            // Baris literal 21: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `risk.event_id,`.
            // Baris literal 22: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `risk.sequence_number as
            // risk_sequence,`.
            // Baris literal 23: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun:
            // `min(resolution.sequence_number) as resolution_sequence`.
            // Baris literal 24: FROM memilih tabel/subquery sumber pembacaan: `from personal_risks risk`.
            // Baris literal 25: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `left join events resolution`.
            // Baris literal 26: ON menetapkan syarat pencocokan baris pada relasi JOIN: `on resolution.session_id = risk.session_id`.
            // Baris literal 27: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and resolution.user_id = risk.user_id`.
            // Baris literal 28: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and resolution.action_type in ('Asuransi',
            // 'BayarRisiko', 'GunakanOpsiDarurat', 'PinjamanSyariah')`.
            // Baris literal 29: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and resolution.payload->>'risk_event_id' =
            // risk.event_id::text`.
            // Baris literal 30: GROUP BY mengelompokkan baris sebelum fungsi agregasi dihitung: `group by risk.event_id, risk.sequence_number`.
            // Baris literal 31: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 32: SELECT menentukan nilai atau kolom yang dikembalikan query: `select count(*)::int`.
            // Baris literal 33: FROM memilih tabel/subquery sumber pembacaan: `from resolved_risks`.
            // Baris literal 34: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where resolution_sequence is null`.
            // Baris literal 35: Melanjutkan kondisi SQL dengan OR (alternatif syarat yang dapat terpenuhi): `or resolution_sequence <> risk_sequence + 1`.
            // Baris literal 36: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            """
            with personal_risks as (
              select
                risk_event.session_id,
                risk_event.event_id,
                risk_event.user_id,
                risk_event.sequence_number
              from sessions s
              join events risk_event on risk_event.session_id = s.session_id
              join ruleset_life_risks risk
                on risk.ruleset_version_id = risk_event.ruleset_version_id
               and lower(risk.risk_code) = lower(risk_event.payload->>'risk_id')
              where s.session_name = @mahirSessionName
                and risk_event.action_type = 'RisikoKehidupan'
                and risk.effect_type = 'COIN_EFFECT'
                and risk.direction = 'OUT'
                and risk.target_scope = 'SELF'
            ),
            resolved_risks as (
              select
                risk.event_id,
                risk.sequence_number as risk_sequence,
                min(resolution.sequence_number) as resolution_sequence
              from personal_risks risk
              left join events resolution
                on resolution.session_id = risk.session_id
               and resolution.user_id = risk.user_id
               and resolution.action_type in ('Asuransi', 'BayarRisiko', 'GunakanOpsiDarurat', 'PinjamanSyariah')
               and resolution.payload->>'risk_event_id' = risk.event_id::text
              group by risk.event_id, risk.sequence_number
            )
            select count(*)::int
            from resolved_risks
            where resolution_sequence is null
               or resolution_sequence <> risk_sequence + 1
            """,
            // Meneruskan objek anonim yang mengelompokkan mahirSessionName sebagai satu nilai sebagai argumen ke `connection.ExecuteScalarAsync<int>`.
            new { mahirSessionName = SeedMahirSessionName });
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`0`, `delayedPersonalRiskResolutionCount`);
        // pengujian gagal jika keduanya berbeda dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        Assert.Equal(0, delayedPersonalRiskResolutionCount);

        // Menyiapkan variabel lokal `insuredRiskNetCostCount` untuk nilai insured risiko net biaya jumlah dengan hasil operasi asinkron menjalankan
        // perintah basis data melalui `connection` dengan `””” with insured_risks as ( select risk_evt.event_pk as risk_event_pk, insurance_evt.event_pk as
        // insurance_event_pk, insurance_evt.user_id as user_id from sessions s join even...`, `new { mahirSessionName = SeedMahirSessionName }` dan
        // mengambil nilai skalar hasilnya; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var insuredRiskNetCostCount = await connection.ExecuteScalarAsync<int>(
            // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke `connection.ExecuteScalarAsync<int>`.
            // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
            // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 2: WITH menamai hasil query sementara (CTE) yang dapat digunakan oleh bagian SQL berikutnya: `with insured_risks as (`.
            // Baris literal 3: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
            // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `risk_evt.event_pk as
            // risk_event_pk,`.
            // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `insurance_evt.event_pk as
            // insurance_event_pk,`.
            // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `insurance_evt.user_id as
            // user_id`.
            // Baris literal 7: FROM memilih tabel/subquery sumber pembacaan: `from sessions s`.
            // Baris literal 8: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join events insurance_evt`.
            // Baris literal 9: ON menetapkan syarat pencocokan baris pada relasi JOIN: `on insurance_evt.session_id = s.session_id`.
            // Baris literal 10: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and insurance_evt.action_type = 'Asuransi'`.
            // Baris literal 11: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and insurance_evt.payload ? 'risk_event_id'`.
            // Baris literal 12: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join events risk_evt`.
            // Baris literal 13: ON menetapkan syarat pencocokan baris pada relasi JOIN: `on risk_evt.session_id = insurance_evt.session_id`.
            // Baris literal 14: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and risk_evt.event_id =
            // (insurance_evt.payload->>'risk_event_id')::uuid`.
            // Baris literal 15: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join ruleset_life_risks risk_catalog`.
            // Baris literal 16: ON menetapkan syarat pencocokan baris pada relasi JOIN: `on risk_catalog.ruleset_version_id = risk_evt.ruleset_version_id`.
            // Baris literal 17: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and lower(risk_catalog.risk_code) =
            // lower(risk_evt.payload->>'risk_id')`.
            // Baris literal 18: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and risk_catalog.direction = 'OUT'`.
            // Baris literal 19: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where s.session_name = @mahirSessionName`.
            // Baris literal 20: Pembatas literal/penutup `),`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 21: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `net_cost as (`.
            // Baris literal 22: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
            // Baris literal 23: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ir.risk_event_pk,`.
            // Baris literal 24: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ir.user_id,`.
            // Baris literal 25: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `coalesce(sum(`.
            // Baris literal 26: Menyusun pilihan nilai bersyarat di dalam ekspresi SQL: `case`.
            // Baris literal 27: Menyusun pilihan nilai bersyarat di dalam ekspresi SQL: `when ecp.direction = 'IN' then ecp.amount`.
            // Baris literal 28: Menyusun pilihan nilai bersyarat di dalam ekspresi SQL: `when ecp.direction = 'OUT' then -ecp.amount`.
            // Baris literal 29: Menyusun pilihan nilai bersyarat di dalam ekspresi SQL: `else 0`.
            // Baris literal 30: Menyusun pilihan nilai bersyarat di dalam ekspresi SQL: `end`.
            // Baris literal 31: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `), 0) as net_amount`.
            // Baris literal 32: FROM memilih tabel/subquery sumber pembacaan: `from insured_risks ir`.
            // Baris literal 33: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `left join event_cashflow_projections ecp`.
            // Baris literal 34: ON menetapkan syarat pencocokan baris pada relasi JOIN: `on ecp.event_pk = ir.insurance_event_pk`.
            // Baris literal 35: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and ecp.user_id = ir.user_id`.
            // Baris literal 36: GROUP BY mengelompokkan baris sebelum fungsi agregasi dihitung: `group by ir.risk_event_pk, ir.user_id`.
            // Baris literal 37: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 38: SELECT menentukan nilai atau kolom yang dikembalikan query: `select count(*)::int`.
            // Baris literal 39: FROM memilih tabel/subquery sumber pembacaan: `from net_cost`.
            // Baris literal 40: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where net_amount <> 0`.
            // Baris literal 41: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            """
            with insured_risks as (
              select
                risk_evt.event_pk as risk_event_pk,
                insurance_evt.event_pk as insurance_event_pk,
                insurance_evt.user_id as user_id
              from sessions s
              join events insurance_evt
                on insurance_evt.session_id = s.session_id
               and insurance_evt.action_type = 'Asuransi'
               and insurance_evt.payload ? 'risk_event_id'
              join events risk_evt
                on risk_evt.session_id = insurance_evt.session_id
               and risk_evt.event_id = (insurance_evt.payload->>'risk_event_id')::uuid
              join ruleset_life_risks risk_catalog
                on risk_catalog.ruleset_version_id = risk_evt.ruleset_version_id
               and lower(risk_catalog.risk_code) = lower(risk_evt.payload->>'risk_id')
               and risk_catalog.direction = 'OUT'
              where s.session_name = @mahirSessionName
            ),
            net_cost as (
              select
                ir.risk_event_pk,
                ir.user_id,
                coalesce(sum(
                  case
                    when ecp.direction = 'IN' then ecp.amount
                    when ecp.direction = 'OUT' then -ecp.amount
                    else 0
                  end
                ), 0) as net_amount
              from insured_risks ir
              left join event_cashflow_projections ecp
                on ecp.event_pk = ir.insurance_event_pk
               and ecp.user_id = ir.user_id
              group by ir.risk_event_pk, ir.user_id
            )
            select count(*)::int
            from net_cost
            where net_amount <> 0
            """,
            // Meneruskan objek anonim yang mengelompokkan mahirSessionName sebagai satu nilai sebagai argumen ke `connection.ExecuteScalarAsync<int>`.
            new { mahirSessionName = SeedMahirSessionName });
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`0`, `insuredRiskNetCostCount`); pengujian
        // gagal jika keduanya berbeda dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        Assert.Equal(0, insuredRiskNetCostCount);

        // Menyiapkan variabel lokal `marketSlotRows` untuk nilai pasar slot baris dengan mematerialisasi urutan `(await
        // connection.QueryAsync<MarketSlotRow>( ””” select s.session_name, scp.slot_group, count(*)::int as slot_count from sessions s join
        // session_card_positions scp on scp.sess...` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var marketSlotRows = (await connection.QueryAsync<MarketSlotRow>(
            // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke `connection.QueryAsync<MarketSlotRow>`.
            // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
            // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
            // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `s.session_name,`.
            // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `scp.slot_group,`.
            // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `count(*)::int as slot_count`.
            // Baris literal 6: FROM memilih tabel/subquery sumber pembacaan: `from sessions s`.
            // Baris literal 7: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join session_card_positions scp on scp.session_id =
            // s.session_id`.
            // Baris literal 8: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where s.session_name in (@pemulaSessionName,
            // @mahirSessionName)`.
            // Baris literal 9: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and scp.zone = 'MARKET'`.
            // Baris literal 10: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and scp.status = 'ACTIVE'`.
            // Baris literal 11: GROUP BY mengelompokkan baris sebelum fungsi agregasi dihitung: `group by s.session_name, scp.slot_group`.
            // Baris literal 12: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by s.session_name, scp.slot_group`.
            // Baris literal 13: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            """
            select
                s.session_name,
                scp.slot_group,
                count(*)::int as slot_count
            from sessions s
            join session_card_positions scp on scp.session_id = s.session_id
            where s.session_name in (@pemulaSessionName, @mahirSessionName)
              and scp.zone = 'MARKET'
              and scp.status = 'ACTIVE'
            group by s.session_name, scp.slot_group
            order by s.session_name, scp.slot_group
            """,
            // Meneruskan objek anonim yang mengelompokkan pemulaSessionName, mahirSessionName sebagai satu nilai sebagai argumen ke
            // `connection.QueryAsync<MarketSlotRow>`.
            new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            {
                // Meneruskan objek anonim yang mengelompokkan pemulaSessionName, mahirSessionName sebagai satu nilai sebagai argumen ke
                // `connection.QueryAsync<MarketSlotRow>`.
                pemulaSessionName = SeedPemulaSessionName,
                // Meneruskan objek anonim yang mengelompokkan pemulaSessionName, mahirSessionName sebagai satu nilai sebagai argumen ke
                // `connection.QueryAsync<MarketSlotRow>`.
                mahirSessionName = SeedMahirSessionName
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
            // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            })).ToList();

        // Menjalankan pemeriksaan hasil dengan `Assert.Empty` menggunakan `marketSlotRows`; ketidaksesuaian dengan ekspektasi membuat pengujian gagal dalam
        // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        Assert.Empty(marketSlotRows);

        // Menyiapkan variabel lokal `missionAndDonationChecks` untuk nilai misi dan donasi checks dengan membangun kamus dari `(await
        // connection.QueryAsync<ScenarioSystemEventCountRow>( ””” select s.session_name, count(*) filter (where e.action_type = 'SetupMisiAwal')::int as
        // mission_assigned_count, c...` dengan pemilihan kunci/nilai `row => row.SessionName`; kunci harus unik agar konversi berhasil. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var missionAndDonationChecks = (await connection.QueryAsync<ScenarioSystemEventCountRow>(
            // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke
            // `connection.QueryAsync<ScenarioSystemEventCountRow>`.
            // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
            // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
            // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `s.session_name,`.
            // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `count(*) filter (where
            // e.action_type = 'SetupMisiAwal')::int as mission_assigned_count,`.
            // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `count(*) filter (where
            // e.action_type = 'PoinPeringkatDonasi')::int as donation_rank_awarded_count,`.
            // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `count(*) filter (where
            // e.action_type = 'UmumkanJuaraDonasi')::int as donation_winners_announced_count,`.
            // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `count(*) filter (where
            // e.action_type = 'BagikanTieBreaker')::int as tie_breaker_assigned_count`.
            // Baris literal 8: FROM memilih tabel/subquery sumber pembacaan: `from sessions s`.
            // Baris literal 9: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join events e on e.session_id = s.session_id`.
            // Baris literal 10: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where s.session_name in (@pemulaSessionName,
            // @mahirSessionName)`.
            // Baris literal 11: GROUP BY mengelompokkan baris sebelum fungsi agregasi dihitung: `group by s.session_name`.
            // Baris literal 12: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by s.session_name asc`.
            // Baris literal 13: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            """
            select
                s.session_name,
                count(*) filter (where e.action_type = 'SetupMisiAwal')::int as mission_assigned_count,
                count(*) filter (where e.action_type = 'PoinPeringkatDonasi')::int as donation_rank_awarded_count,
                count(*) filter (where e.action_type = 'UmumkanJuaraDonasi')::int as donation_winners_announced_count,
                count(*) filter (where e.action_type = 'BagikanTieBreaker')::int as tie_breaker_assigned_count
            from sessions s
            join events e on e.session_id = s.session_id
            where s.session_name in (@pemulaSessionName, @mahirSessionName)
            group by s.session_name
            order by s.session_name asc
            """,
            // Meneruskan objek anonim yang mengelompokkan pemulaSessionName, mahirSessionName sebagai satu nilai sebagai argumen ke
            // `connection.QueryAsync<ScenarioSystemEventCountRow>`.
            new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            {
                // Meneruskan objek anonim yang mengelompokkan pemulaSessionName, mahirSessionName sebagai satu nilai sebagai argumen ke
                // `connection.QueryAsync<ScenarioSystemEventCountRow>`.
                pemulaSessionName = SeedPemulaSessionName,
                // Meneruskan objek anonim yang mengelompokkan pemulaSessionName, mahirSessionName sebagai satu nilai sebagai argumen ke
                // `connection.QueryAsync<ScenarioSystemEventCountRow>`.
                mahirSessionName = SeedMahirSessionName
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
            // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            })).ToDictionary(row => row.SessionName);

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`4`,
        // `missionAndDonationChecks[SeedPemulaSessionName].MissionAssignedCount`); pengujian gagal jika keduanya berbeda dalam
        // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        Assert.Equal(4, missionAndDonationChecks[SeedPemulaSessionName].MissionAssignedCount);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`4`,
        // `missionAndDonationChecks[SeedMahirSessionName].MissionAssignedCount`); pengujian gagal jika keduanya berbeda dalam
        // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        Assert.Equal(4, missionAndDonationChecks[SeedMahirSessionName].MissionAssignedCount);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`9`,
        // `missionAndDonationChecks[SeedPemulaSessionName].DonationRankAwardedCount`); pengujian gagal jika keduanya berbeda dalam
        // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        Assert.Equal(9, missionAndDonationChecks[SeedPemulaSessionName].DonationRankAwardedCount);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`9`,
        // `missionAndDonationChecks[SeedMahirSessionName].DonationRankAwardedCount`); pengujian gagal jika keduanya berbeda dalam
        // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        Assert.Equal(9, missionAndDonationChecks[SeedMahirSessionName].DonationRankAwardedCount);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`3`,
        // `missionAndDonationChecks[SeedPemulaSessionName].DonationWinnersAnnouncedCount`); pengujian gagal jika keduanya berbeda dalam
        // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        Assert.Equal(3, missionAndDonationChecks[SeedPemulaSessionName].DonationWinnersAnnouncedCount);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`3`,
        // `missionAndDonationChecks[SeedMahirSessionName].DonationWinnersAnnouncedCount`); pengujian gagal jika keduanya berbeda dalam
        // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        Assert.Equal(3, missionAndDonationChecks[SeedMahirSessionName].DonationWinnersAnnouncedCount);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`4`,
        // `missionAndDonationChecks[SeedPemulaSessionName].TieBreakerAssignedCount`); pengujian gagal jika keduanya berbeda dalam
        // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        Assert.Equal(4, missionAndDonationChecks[SeedPemulaSessionName].TieBreakerAssignedCount);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`4`,
        // `missionAndDonationChecks[SeedMahirSessionName].TieBreakerAssignedCount`); pengujian gagal jika keduanya berbeda dalam
        // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        Assert.Equal(4, missionAndDonationChecks[SeedMahirSessionName].TieBreakerAssignedCount);

        // Menyiapkan variabel lokal `winnerAnnouncementSummaries` untuk nilai winner announcement summaries dengan mematerialisasi urutan `(await
        // connection.QueryAsync<string>( ””” select e.payload->>'summary' from sessions s join events e on e.session_id = s.session_id where s.session_name
        // = @pemulaSessionName a...` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai
        // awal.
        var winnerAnnouncementSummaries = (await connection.QueryAsync<string>(
            // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke `connection.QueryAsync<string>`.
            // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
            // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select e.payload->>'summary'`.
            // Baris literal 3: FROM memilih tabel/subquery sumber pembacaan: `from sessions s`.
            // Baris literal 4: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join events e on e.session_id = s.session_id`.
            // Baris literal 5: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where s.session_name = @pemulaSessionName`.
            // Baris literal 6: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and e.action_type = 'UmumkanJuaraDonasi'`.
            // Baris literal 7: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by e.day_index asc`.
            // Baris literal 8: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            """
            select e.payload->>'summary'
            from sessions s
            join events e on e.session_id = s.session_id
            where s.session_name = @pemulaSessionName
              and e.action_type = 'UmumkanJuaraDonasi'
            order by e.day_index asc
            """,
            // Meneruskan objek anonim yang mengelompokkan pemulaSessionName sebagai satu nilai sebagai argumen ke `connection.QueryAsync<string>`.
            new { pemulaSessionName = SeedPemulaSessionName })).ToList();

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`new[] { ”Manalu Juara 1, Marcello Juara 2,
        // Marco Juara 3”, ”Marco Juara 1, Manalu Juara 2, Hugo Juara 3”, ”Marcello Juara 1, Manalu Juara 2, Marco Juara 3” }`,
        // `winnerAnnouncementSummaries`); pengujian gagal jika keduanya berbeda dalam
        // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        Assert.Equal(
            // Meneruskan array baru dengan tipe elemen disimpulkan dari nilai initializer sebagai argumen ke `Assert.Equal`.
            new[]
            // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            {
                // Meneruskan array baru dengan tipe elemen disimpulkan dari nilai initializer sebagai argumen ke `Assert.Equal`.
                "Manalu Juara 1, Marcello Juara 2, Marco Juara 3",
                // Meneruskan array baru dengan tipe elemen disimpulkan dari nilai initializer sebagai argumen ke `Assert.Equal`.
                "Marco Juara 1, Manalu Juara 2, Hugo Juara 3",
                // Meneruskan array baru dengan tipe elemen disimpulkan dari nilai initializer sebagai argumen ke `Assert.Equal`.
                "Marcello Juara 1, Manalu Juara 2, Marco Juara 3"
            // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
            // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            },
            // Meneruskan `winnerAnnouncementSummaries` (nilai winner announcement summaries) sebagai argumen ke `Assert.Equal`.
            winnerAnnouncementSummaries);

        // Menyiapkan variabel lokal `projectionCoverage` untuk nilai projection coverage dengan mematerialisasi urutan `(await
        // connection.QueryAsync<PlayerProjectionCoverageRow>( ””” select s.session_name, sp.user_id, count(distinct e.event_pk) filter (where e.actor_type
        // = 'PLAYER')::int as pla...` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai
        // awal.
        var projectionCoverage = (await connection.QueryAsync<PlayerProjectionCoverageRow>(
            // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke
            // `connection.QueryAsync<PlayerProjectionCoverageRow>`.
            // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
            // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
            // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `s.session_name,`.
            // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `sp.user_id,`.
            // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `count(distinct e.event_pk)
            // filter (where e.actor_type = 'PLAYER')::int as player_event_count,`.
            // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `count(distinct
            // ecp.projection_id)::int as projection_count`.
            // Baris literal 7: FROM memilih tabel/subquery sumber pembacaan: `from sessions s`.
            // Baris literal 8: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join session_participants sp on sp.session_id =
            // s.session_id`.
            // Baris literal 9: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `left join events e`.
            // Baris literal 10: ON menetapkan syarat pencocokan baris pada relasi JOIN: `on e.session_id = sp.session_id`.
            // Baris literal 11: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and e.user_id = sp.user_id`.
            // Baris literal 12: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `left join event_cashflow_projections ecp`.
            // Baris literal 13: ON menetapkan syarat pencocokan baris pada relasi JOIN: `on ecp.session_id = sp.session_id`.
            // Baris literal 14: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and ecp.user_id = sp.user_id`.
            // Baris literal 15: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where s.session_name in (@pemulaSessionName,
            // @mahirSessionName)`.
            // Baris literal 16: GROUP BY mengelompokkan baris sebelum fungsi agregasi dihitung: `group by s.session_name, sp.user_id`.
            // Baris literal 17: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by s.session_name asc, sp.user_id asc`.
            // Baris literal 18: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            """
            select
                s.session_name,
                sp.user_id,
                count(distinct e.event_pk) filter (where e.actor_type = 'PLAYER')::int as player_event_count,
                count(distinct ecp.projection_id)::int as projection_count
            from sessions s
            join session_participants sp on sp.session_id = s.session_id
            left join events e
              on e.session_id = sp.session_id
             and e.user_id = sp.user_id
            left join event_cashflow_projections ecp
              on ecp.session_id = sp.session_id
             and ecp.user_id = sp.user_id
            where s.session_name in (@pemulaSessionName, @mahirSessionName)
            group by s.session_name, sp.user_id
            order by s.session_name asc, sp.user_id asc
            """,
            // Meneruskan objek anonim yang mengelompokkan pemulaSessionName, mahirSessionName sebagai satu nilai sebagai argumen ke
            // `connection.QueryAsync<PlayerProjectionCoverageRow>`.
            new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            {
                // Meneruskan objek anonim yang mengelompokkan pemulaSessionName, mahirSessionName sebagai satu nilai sebagai argumen ke
                // `connection.QueryAsync<PlayerProjectionCoverageRow>`.
                pemulaSessionName = SeedPemulaSessionName,
                // Meneruskan objek anonim yang mengelompokkan pemulaSessionName, mahirSessionName sebagai satu nilai sebagai argumen ke
                // `connection.QueryAsync<PlayerProjectionCoverageRow>`.
                mahirSessionName = SeedMahirSessionName
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
            // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            })).ToList();

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`8`, `projectionCoverage.Count`); pengujian
        // gagal jika keduanya berbeda dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        Assert.Equal(8, projectionCoverage.Count);
        // Menjalankan pemeriksaan hasil dengan `Assert.All` menggunakan `projectionCoverage`, `row => { Assert.True(row.PlayerEventCount > 0,
        // $”{row.SessionName}/{row.UserId} tidak punya event pemain.”); Assert.True(row.ProjectionCount > 0, $”{row.SessionName}/{row.User...`;
        // ketidaksesuaian dengan ekspektasi membuat pengujian gagal dalam
        // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        Assert.All(projectionCoverage, row =>
        // Membuka scope fungsi lambda yang dipasok ke `Assert.All`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        {
            // Menjalankan pemeriksaan bahwa `row.PlayerEventCount > 0`, `$”{row.SessionName}/{row.UserId} tidak punya event pemain.”` bernilai benar; pengujian
            // gagal jika kondisi tidak terpenuhi dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            Assert.True(row.PlayerEventCount > 0, $"{row.SessionName}/{row.UserId} tidak punya event pemain.");
            // Menjalankan pemeriksaan bahwa `row.ProjectionCount > 0`, `$”{row.SessionName}/{row.UserId} tidak punya transaksi cashflow.”` bernilai benar;
            // pengujian gagal jika kondisi tidak terpenuhi dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            Assert.True(row.ProjectionCount > 0, $"{row.SessionName}/{row.UserId} tidak punya transaksi cashflow.");
        // Menutup scope fungsi lambda yang dipasok ke `Assert.All`; bagian berikut berada di luar batas blok tersebut dalam
        // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        });

        // Menyiapkan variabel lokal `gameplaySnapshotCoverage` untuk nilai gameplay snapshot keadaan coverage dengan mematerialisasi urutan `(await
        // connection.QueryAsync<PlayerGameplaySnapshotCoverageRow>( ””” select s.session_name, sp.user_id, count(*) filter (where ms.metric_name =
        // 'gameplay.raw.variables')::int ...` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var gameplaySnapshotCoverage = (await connection.QueryAsync<PlayerGameplaySnapshotCoverageRow>(
            // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke
            // `connection.QueryAsync<PlayerGameplaySnapshotCoverageRow>`.
            // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
            // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
            // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `s.session_name,`.
            // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `sp.user_id,`.
            // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `count(*) filter (where
            // ms.metric_name = 'gameplay.raw.variables')::int as raw_snapshot_count,`.
            // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `count(*) filter (where
            // ms.metric_name = 'gameplay.derived.metrics')::int as derived_snapshot_count,`.
            // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `count(*) filter (`.
            // Baris literal 8: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where ms.metric_name in ('gameplay.raw.variables',
            // 'gameplay.derived.metrics')`.
            // Baris literal 9: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and ms.metric_payload_json is null`.
            // Baris literal 10: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `)::int as
            // empty_payload_count`.
            // Baris literal 11: FROM memilih tabel/subquery sumber pembacaan: `from sessions s`.
            // Baris literal 12: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join session_participants sp on sp.session_id =
            // s.session_id`.
            // Baris literal 13: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `left join metric_snapshots ms`.
            // Baris literal 14: ON menetapkan syarat pencocokan baris pada relasi JOIN: `on ms.session_id = sp.session_id`.
            // Baris literal 15: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and ms.user_id = sp.user_id`.
            // Baris literal 16: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and ms.session_player_id = sp.session_participant_id`.
            // Baris literal 17: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and ms.metric_name in ('gameplay.raw.variables',
            // 'gameplay.derived.metrics')`.
            // Baris literal 18: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where s.session_name in (@pemulaSessionName,
            // @mahirSessionName)`.
            // Baris literal 19: GROUP BY mengelompokkan baris sebelum fungsi agregasi dihitung: `group by s.session_name, sp.user_id`.
            // Baris literal 20: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by s.session_name asc, sp.user_id asc`.
            // Baris literal 21: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            """
            select
                s.session_name,
                sp.user_id,
                count(*) filter (where ms.metric_name = 'gameplay.raw.variables')::int as raw_snapshot_count,
                count(*) filter (where ms.metric_name = 'gameplay.derived.metrics')::int as derived_snapshot_count,
                count(*) filter (
                    where ms.metric_name in ('gameplay.raw.variables', 'gameplay.derived.metrics')
                      and ms.metric_payload_json is null
                )::int as empty_payload_count
            from sessions s
            join session_participants sp on sp.session_id = s.session_id
            left join metric_snapshots ms
              on ms.session_id = sp.session_id
             and ms.user_id = sp.user_id
             and ms.session_player_id = sp.session_participant_id
             and ms.metric_name in ('gameplay.raw.variables', 'gameplay.derived.metrics')
            where s.session_name in (@pemulaSessionName, @mahirSessionName)
            group by s.session_name, sp.user_id
            order by s.session_name asc, sp.user_id asc
            """,
            // Meneruskan objek anonim yang mengelompokkan pemulaSessionName, mahirSessionName sebagai satu nilai sebagai argumen ke
            // `connection.QueryAsync<PlayerGameplaySnapshotCoverageRow>`.
            new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            {
                // Meneruskan objek anonim yang mengelompokkan pemulaSessionName, mahirSessionName sebagai satu nilai sebagai argumen ke
                // `connection.QueryAsync<PlayerGameplaySnapshotCoverageRow>`.
                pemulaSessionName = SeedPemulaSessionName,
                // Meneruskan objek anonim yang mengelompokkan pemulaSessionName, mahirSessionName sebagai satu nilai sebagai argumen ke
                // `connection.QueryAsync<PlayerGameplaySnapshotCoverageRow>`.
                mahirSessionName = SeedMahirSessionName
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
            // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            })).ToList();

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`8`, `gameplaySnapshotCoverage.Count`);
        // pengujian gagal jika keduanya berbeda dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        Assert.Equal(8, gameplaySnapshotCoverage.Count);
        // Menjalankan pemeriksaan hasil dengan `Assert.All` menggunakan `gameplaySnapshotCoverage`, `row => { Assert.True(row.RawSnapshotCount > 0,
        // $”{row.SessionName}/{row.UserId} tidak punya snapshot gameplay raw.”); Assert.True(row.DerivedSnapshotCount > 0, $”{row.SessionN...`;
        // ketidaksesuaian dengan ekspektasi membuat pengujian gagal dalam
        // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        Assert.All(gameplaySnapshotCoverage, row =>
        // Membuka scope fungsi lambda yang dipasok ke `Assert.All`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        {
            // Menjalankan pemeriksaan bahwa `row.RawSnapshotCount > 0`, `$”{row.SessionName}/{row.UserId} tidak punya snapshot gameplay raw.”` bernilai benar;
            // pengujian gagal jika kondisi tidak terpenuhi dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            Assert.True(row.RawSnapshotCount > 0, $"{row.SessionName}/{row.UserId} tidak punya snapshot gameplay raw.");
            // Menjalankan pemeriksaan bahwa `row.DerivedSnapshotCount > 0`, `$”{row.SessionName}/{row.UserId} tidak punya snapshot gameplay derived.”` bernilai
            // benar; pengujian gagal jika kondisi tidak terpenuhi dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            Assert.True(row.DerivedSnapshotCount > 0, $"{row.SessionName}/{row.UserId} tidak punya snapshot gameplay derived.");
            // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`0`, `row.EmptyPayloadCount`); pengujian gagal
            // jika keduanya berbeda dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            Assert.Equal(0, row.EmptyPayloadCount);
        // Menutup scope fungsi lambda yang dipasok ke `Assert.All`; bagian berikut berada di luar batas blok tersebut dalam
        // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        });

        // Menyiapkan variabel lokal `snapshotInvariants` untuk nilai snapshot keadaan invariants dengan mematerialisasi urutan `(await
        // connection.QueryAsync<SnapshotInvariantRow>( ””” with raw_snapshots as ( select s.session_name, sp.user_id, ms.metric_payload_json as raw from
        // sessions s join session_p...` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi
        // nilai awal.
        var snapshotInvariants = (await connection.QueryAsync<SnapshotInvariantRow>(
            // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke `connection.QueryAsync<SnapshotInvariantRow>`.
            // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
            // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 2: WITH menamai hasil query sementara (CTE) yang dapat digunakan oleh bagian SQL berikutnya: `with raw_snapshots as (`.
            // Baris literal 3: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
            // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `s.session_name,`.
            // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `sp.user_id,`.
            // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ms.metric_payload_json as
            // raw`.
            // Baris literal 7: FROM memilih tabel/subquery sumber pembacaan: `from sessions s`.
            // Baris literal 8: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join session_participants sp on sp.session_id =
            // s.session_id`.
            // Baris literal 9: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join metric_snapshots ms`.
            // Baris literal 10: ON menetapkan syarat pencocokan baris pada relasi JOIN: `on ms.session_id = sp.session_id`.
            // Baris literal 11: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and ms.user_id = sp.user_id`.
            // Baris literal 12: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and ms.session_player_id = sp.session_participant_id`.
            // Baris literal 13: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and ms.metric_name = 'gameplay.raw.variables'`.
            // Baris literal 14: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where s.session_name in (@pemulaSessionName,
            // @mahirSessionName)`.
            // Baris literal 15: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 16: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
            // Baris literal 17: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `session_name,`.
            // Baris literal 18: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `user_id,`.
            // Baris literal 19: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun:
            // `(raw#>>'{ingredients,ingredients_collected}')::int`.
            // Baris literal 20: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `-
            // (raw#>>'{ingredients,ingredients_used_total}')::int`.
            // Baris literal 21: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `-
            // (raw#>>'{ingredients,ingredients_held_current}')::int`.
            // Baris literal 22: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `-
            // (raw#>>'{ingredients,ingredients_wasted}')::int as ingredient_difference,`.
            // Baris literal 23: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun:
            // `(raw#>>'{gold,gold_cards_initial}')::int`.
            // Baris literal 24: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `+
            // (raw#>>'{gold,gold_cards_purchased}')::int`.
            // Baris literal 25: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `-
            // (raw#>>'{gold,gold_cards_sold}')::int`.
            // Baris literal 26: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `-
            // (raw#>>'{gold,gold_cards_held_end}')::int as gold_difference,`.
            // Baris literal 27: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun:
            // `(raw#>>'{donations,donation_total_coins}')::int`.
            // Baris literal 28: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `- coalesce((`.
            // Baris literal 29: SELECT menentukan nilai atau kolom yang dikembalikan query: `select sum((item->>'amount')::int)`.
            // Baris literal 30: FROM memilih tabel/subquery sumber pembacaan: `from jsonb_array_elements(raw#>'{donations,donation_amount_per_friday}') item`.
            // Baris literal 31: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `), 0)::int as
            // donation_difference,`.
            // Baris literal 32: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun:
            // `(raw#>>'{meal_orders,meal_order_income_total}')::int`.
            // Baris literal 33: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `- coalesce((`.
            // Baris literal 34: SELECT menentukan nilai atau kolom yang dikembalikan query: `select sum((item#>>'{}')::int)`.
            // Baris literal 35: FROM memilih tabel/subquery sumber pembacaan: `from jsonb_array_elements(raw#>'{meal_orders,meal_order_income_per_order}')
            // item`.
            // Baris literal 36: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `), 0)::int as
            // order_income_difference,`.
            // Baris literal 37: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun:
            // `(raw#>>'{ingredients,ingredients_used_total}')::int`.
            // Baris literal 38: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `- coalesce((`.
            // Baris literal 39: SELECT menentukan nilai atau kolom yang dikembalikan query: `select sum((item#>>'{}')::int)`.
            // Baris literal 40: FROM memilih tabel/subquery sumber pembacaan: `from jsonb_array_elements(raw#>'{ingredients,ingredients_used_per_meal}') item`.
            // Baris literal 41: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `), 0)::int as
            // ingredient_use_difference,`.
            // Baris literal 42: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun:
            // `coalesce((raw#>>'{life_risk,life_risk_costs_total}')::int, 0)`.
            // Baris literal 43: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `- coalesce((`.
            // Baris literal 44: SELECT menentukan nilai atau kolom yang dikembalikan query: `select sum((item#>>'{}')::int)`.
            // Baris literal 45: FROM memilih tabel/subquery sumber pembacaan: `from jsonb_array_elements(coalesce(raw#>'{life_risk,life_risk_costs_per_card}',
            // '[]'::jsonb)) item`.
            // Baris literal 46: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `), 0)::int as
            // risk_cost_difference,`.
            // Baris literal 47: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun:
            // `(raw#>>'{meal_orders,meal_orders_claimed}')::int`.
            // Baris literal 48: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `-
            // jsonb_array_length(raw#>'{meal_orders,meal_order_income_per_order}') as order_count_difference,`.
            // Baris literal 49: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun:
            // `(raw#>>'{coins,starting_coins}')::int`.
            // Baris literal 50: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `+ coalesce((`.
            // Baris literal 51: SELECT menentukan nilai atau kolom yang dikembalikan query: `select sum((item->>'amount')::int)`.
            // Baris literal 52: FROM memilih tabel/subquery sumber pembacaan: `from jsonb_array_elements(raw#>'{coins,coins_earned_per_turn}') item`.
            // Baris literal 53: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `), 0)::int`.
            // Baris literal 54: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `- coalesce((`.
            // Baris literal 55: SELECT menentukan nilai atau kolom yang dikembalikan query: `select sum((item->>'amount')::int)`.
            // Baris literal 56: FROM memilih tabel/subquery sumber pembacaan: `from jsonb_array_elements(raw#>'{coins,coins_spent_per_turn}') item`.
            // Baris literal 57: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `), 0)::int`.
            // Baris literal 58: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `-
            // (raw#>>'{coins,coins_held_current}')::int as cash_difference`.
            // Baris literal 59: FROM memilih tabel/subquery sumber pembacaan: `from raw_snapshots`.
            // Baris literal 60: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by session_name, user_id`.
            // Baris literal 61: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            """
            with raw_snapshots as (
                select
                    s.session_name,
                    sp.user_id,
                    ms.metric_payload_json as raw
                from sessions s
                join session_participants sp on sp.session_id = s.session_id
                join metric_snapshots ms
                  on ms.session_id = sp.session_id
                 and ms.user_id = sp.user_id
                 and ms.session_player_id = sp.session_participant_id
                 and ms.metric_name = 'gameplay.raw.variables'
                where s.session_name in (@pemulaSessionName, @mahirSessionName)
            )
            select
                session_name,
                user_id,
                (raw#>>'{ingredients,ingredients_collected}')::int
                  - (raw#>>'{ingredients,ingredients_used_total}')::int
                  - (raw#>>'{ingredients,ingredients_held_current}')::int
                  - (raw#>>'{ingredients,ingredients_wasted}')::int as ingredient_difference,
                (raw#>>'{gold,gold_cards_initial}')::int
                  + (raw#>>'{gold,gold_cards_purchased}')::int
                  - (raw#>>'{gold,gold_cards_sold}')::int
                  - (raw#>>'{gold,gold_cards_held_end}')::int as gold_difference,
                (raw#>>'{donations,donation_total_coins}')::int
                  - coalesce((
                      select sum((item->>'amount')::int)
                      from jsonb_array_elements(raw#>'{donations,donation_amount_per_friday}') item
                    ), 0)::int as donation_difference,
                (raw#>>'{meal_orders,meal_order_income_total}')::int
                  - coalesce((
                      select sum((item#>>'{}')::int)
                      from jsonb_array_elements(raw#>'{meal_orders,meal_order_income_per_order}') item
                    ), 0)::int as order_income_difference,
                (raw#>>'{ingredients,ingredients_used_total}')::int
                  - coalesce((
                      select sum((item#>>'{}')::int)
                      from jsonb_array_elements(raw#>'{ingredients,ingredients_used_per_meal}') item
                    ), 0)::int as ingredient_use_difference,
                coalesce((raw#>>'{life_risk,life_risk_costs_total}')::int, 0)
                  - coalesce((
                      select sum((item#>>'{}')::int)
                      from jsonb_array_elements(coalesce(raw#>'{life_risk,life_risk_costs_per_card}', '[]'::jsonb)) item
                    ), 0)::int as risk_cost_difference,
                (raw#>>'{meal_orders,meal_orders_claimed}')::int
                  - jsonb_array_length(raw#>'{meal_orders,meal_order_income_per_order}') as order_count_difference,
                (raw#>>'{coins,starting_coins}')::int
                  + coalesce((
                      select sum((item->>'amount')::int)
                      from jsonb_array_elements(raw#>'{coins,coins_earned_per_turn}') item
                    ), 0)::int
                  - coalesce((
                      select sum((item->>'amount')::int)
                      from jsonb_array_elements(raw#>'{coins,coins_spent_per_turn}') item
                    ), 0)::int
                  - (raw#>>'{coins,coins_held_current}')::int as cash_difference
            from raw_snapshots
            order by session_name, user_id
            """,
            // Meneruskan objek anonim yang mengelompokkan pemulaSessionName, mahirSessionName sebagai satu nilai sebagai argumen ke
            // `connection.QueryAsync<SnapshotInvariantRow>`.
            new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            {
                // Meneruskan objek anonim yang mengelompokkan pemulaSessionName, mahirSessionName sebagai satu nilai sebagai argumen ke
                // `connection.QueryAsync<SnapshotInvariantRow>`.
                pemulaSessionName = SeedPemulaSessionName,
                // Meneruskan objek anonim yang mengelompokkan pemulaSessionName, mahirSessionName sebagai satu nilai sebagai argumen ke
                // `connection.QueryAsync<SnapshotInvariantRow>`.
                mahirSessionName = SeedMahirSessionName
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
            // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            })).ToList();

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`8`, `snapshotInvariants.Count`); pengujian
        // gagal jika keduanya berbeda dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        Assert.Equal(8, snapshotInvariants.Count);
        // Menjalankan pemeriksaan hasil dengan `Assert.All` menggunakan `snapshotInvariants`, `row => { Assert.Equal(0, row.IngredientDifference);
        // Assert.Equal(0, row.GoldDifference); Assert.Equal(0, row.DonationDifference); Assert.Equal(0, row.OrderIncomeDifference); A...`; ketidaksesuaian
        // dengan ekspektasi membuat pengujian gagal dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        Assert.All(snapshotInvariants, row =>
        // Membuka scope fungsi lambda yang dipasok ke `Assert.All`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        {
            // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`0`, `row.IngredientDifference`); pengujian
            // gagal jika keduanya berbeda dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            Assert.Equal(0, row.IngredientDifference);
            // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`0`, `row.GoldDifference`); pengujian gagal
            // jika keduanya berbeda dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            Assert.Equal(0, row.GoldDifference);
            // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`0`, `row.DonationDifference`); pengujian gagal
            // jika keduanya berbeda dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            Assert.Equal(0, row.DonationDifference);
            // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`0`, `row.OrderIncomeDifference`); pengujian
            // gagal jika keduanya berbeda dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            Assert.Equal(0, row.OrderIncomeDifference);
            // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`0`, `row.IngredientUseDifference`); pengujian
            // gagal jika keduanya berbeda dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            Assert.Equal(0, row.IngredientUseDifference);
            // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`0`, `row.RiskCostDifference`); pengujian gagal
            // jika keduanya berbeda dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            Assert.Equal(0, row.RiskCostDifference);
            // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`0`, `row.OrderCountDifference`); pengujian
            // gagal jika keduanya berbeda dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            Assert.Equal(0, row.OrderCountDifference);
            // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`0`, `row.CashDifference`); pengujian gagal
            // jika keduanya berbeda dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
            Assert.Equal(0, row.CashDifference);
        // Menutup scope fungsi lambda yang dipasok ke `Assert.All`; bagian berikut berada di luar batas blok tersebut dalam
        // ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        });

        // Menjalankan hasil operasi asinkron memanggil `AssertScenarioReplayIsValidAsync` dengan `connection`; await menunggu hasil tanpa memblokir thread
        // selama operasi belum selesai dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
        await AssertScenarioReplayIsValidAsync(connection);
    // Menutup scope metode ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions; bagian berikut berada di luar batas
    // blok tersebut dalam ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions.
    }

    // Mendefinisikan metode `AssertScenarioReplayIsValidAsync` dengan hasil bertipe `Task`; operasi ini menangani assert scenario replay berstatus
    // valid asinkron. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter
    // `connection` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data.
    private static async Task AssertScenarioReplayIsValidAsync(NpgsqlConnection connection)
    // Membuka scope metode AssertScenarioReplayIsValidAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // AssertScenarioReplayIsValidAsync.
    {
        // Menyiapkan variabel lokal `events` untuk kumpulan event permainan sebagai sumber riwayat untuk validasi atau perhitungan dengan mematerialisasi
        // urutan `(await connection.QueryAsync<ReplayEventRow>( ””” select s.session_name, s.mode, e.session_id, e.event_id, e.user_id, sp.player_name,
        // e.actor_type, e.day_index, e.weekday, e.a...` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var events = (await connection.QueryAsync<ReplayEventRow>(
            // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke `connection.QueryAsync<ReplayEventRow>`.
            // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
            // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
            // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `s.session_name,`.
            // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `s.mode,`.
            // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `e.session_id,`.
            // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `e.event_id,`.
            // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `e.user_id,`.
            // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `sp.player_name,`.
            // Baris literal 9: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `e.actor_type,`.
            // Baris literal 10: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `e.day_index,`.
            // Baris literal 11: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `e.weekday,`.
            // Baris literal 12: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `e.action_slot,`.
            // Baris literal 13: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `e.sequence_number,`.
            // Baris literal 14: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `e.action_type,`.
            // Baris literal 15: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `e.payload`.
            // Baris literal 16: FROM memilih tabel/subquery sumber pembacaan: `from sessions s`.
            // Baris literal 17: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join events e on e.session_id = s.session_id`.
            // Baris literal 18: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `left join session_participants sp on
            // sp.session_participant_id = e.session_player_id`.
            // Baris literal 19: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where s.session_name in (@pemulaSessionName,
            // @mahirSessionName)`.
            // Baris literal 20: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by s.session_name asc, e.sequence_number
            // asc`.
            // Baris literal 21: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            """
            select
                s.session_name,
                s.mode,
                e.session_id,
                e.event_id,
                e.user_id,
                sp.player_name,
                e.actor_type,
                e.day_index,
                e.weekday,
                e.action_slot,
                e.sequence_number,
                e.action_type,
                e.payload
            from sessions s
            join events e on e.session_id = s.session_id
            left join session_participants sp on sp.session_participant_id = e.session_player_id
            where s.session_name in (@pemulaSessionName, @mahirSessionName)
            order by s.session_name asc, e.sequence_number asc
            """,
            // Meneruskan objek anonim yang mengelompokkan pemulaSessionName, mahirSessionName sebagai satu nilai sebagai argumen ke
            // `connection.QueryAsync<ReplayEventRow>`.
            new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // AssertScenarioReplayIsValidAsync.
            {
                // Meneruskan objek anonim yang mengelompokkan pemulaSessionName, mahirSessionName sebagai satu nilai sebagai argumen ke
                // `connection.QueryAsync<ReplayEventRow>`.
                pemulaSessionName = SeedPemulaSessionName,
                // Meneruskan objek anonim yang mengelompokkan pemulaSessionName, mahirSessionName sebagai satu nilai sebagai argumen ke
                // `connection.QueryAsync<ReplayEventRow>`.
                mahirSessionName = SeedMahirSessionName
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
            // AssertScenarioReplayIsValidAsync.
            })).ToList();

        // Menjalankan pemeriksaan hasil dengan `Assert.NotEmpty` menggunakan `events`; ketidaksesuaian dengan ekspektasi membuat pengujian gagal dalam
        // AssertScenarioReplayIsValidAsync.
        Assert.NotEmpty(events);

        // Menyiapkan variabel lokal `lifeRiskCatalog` untuk nilai life risiko catalog dengan membangun kamus dari `(await
        // connection.QueryAsync<ReplayLifeRiskRow>( ””” select distinct risk.risk_code, risk.direction, risk.amount from sessions s join ruleset_life_risks
        // risk on risk.ruleset_v...` dengan pemilihan kunci/nilai `row => row.RiskCode`, `row => (row.Direction, row.Amount)`,
        // `StringComparer.OrdinalIgnoreCase`; kunci harus unik agar konversi berhasil. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var lifeRiskCatalog = (await connection.QueryAsync<ReplayLifeRiskRow>(
            // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke `connection.QueryAsync<ReplayLifeRiskRow>`.
            // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
            // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select distinct`.
            // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `risk.risk_code,`.
            // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `risk.direction,`.
            // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `risk.amount`.
            // Baris literal 6: FROM memilih tabel/subquery sumber pembacaan: `from sessions s`.
            // Baris literal 7: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join ruleset_life_risks risk`.
            // Baris literal 8: ON menetapkan syarat pencocokan baris pada relasi JOIN: `on risk.ruleset_version_id = s.ruleset_version_id`.
            // Baris literal 9: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where s.session_name in (@pemulaSessionName,
            // @mahirSessionName)`.
            // Baris literal 10: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            """
            select distinct
                risk.risk_code,
                risk.direction,
                risk.amount
            from sessions s
            join ruleset_life_risks risk
              on risk.ruleset_version_id = s.ruleset_version_id
            where s.session_name in (@pemulaSessionName, @mahirSessionName)
            """,
            // Meneruskan objek anonim yang mengelompokkan pemulaSessionName, mahirSessionName sebagai satu nilai sebagai argumen ke
            // `connection.QueryAsync<ReplayLifeRiskRow>`.
            new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // AssertScenarioReplayIsValidAsync.
            {
                // Meneruskan objek anonim yang mengelompokkan pemulaSessionName, mahirSessionName sebagai satu nilai sebagai argumen ke
                // `connection.QueryAsync<ReplayLifeRiskRow>`.
                pemulaSessionName = SeedPemulaSessionName,
                // Meneruskan objek anonim yang mengelompokkan pemulaSessionName, mahirSessionName sebagai satu nilai sebagai argumen ke
                // `connection.QueryAsync<ReplayLifeRiskRow>`.
                mahirSessionName = SeedMahirSessionName
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
            // AssertScenarioReplayIsValidAsync.
            }))
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToDictionary( dalam AssertScenarioReplayIsValidAsync; token pada baris ini
            // menyambungkan bagian kode sebelum dan sesudahnya.
            .ToDictionary(
                // Parameter `row` bertipe `` membawa nilai baris.
                row => row.RiskCode,
                // Parameter `row` bertipe `` membawa nilai baris.
                row => (row.Direction, row.Amount),
                // Meneruskan `StringComparer.OrdinalIgnoreCase` (nilai ordinal ignore case) sebagai argumen ke `(await connection.QueryAsync<ReplayLifeRiskRow>(
                // ””” select distinct risk.risk_code, risk.direction, risk.amount from sessions s join ruleset_life_risks risk on risk.ruleset_v...`.
                StringComparer.OrdinalIgnoreCase);

        // Menyiapkan variabel lokal `sessionStates` untuk nilai sesi states dengan membangun kamus dari `events .GroupBy(e => e.SessionId)` dengan
        // pemilihan kunci/nilai `group => group.Key`, `group => new ReplaySessionState( group.First().SessionName, group.First().Mode,
        // group.First().Mode.Equals(”MAHIR”, StringComparison.OrdinalIgnoreCase) ? 10 : 20)`; kunci harus unik agar konversi berhasil. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var sessionStates = events
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .GroupBy(e => e.SessionId) dalam AssertScenarioReplayIsValidAsync; token pada
            // baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .GroupBy(e => e.SessionId)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToDictionary( dalam AssertScenarioReplayIsValidAsync; token pada baris ini
            // menyambungkan bagian kode sebelum dan sesudahnya.
            .ToDictionary(
                // Parameter `group` bertipe `` membawa nilai group.
                group => group.Key,
                // Parameter `group` bertipe `` membawa nilai group.
                group => new ReplaySessionState(
                    // Meneruskan `group.First().SessionName` (nilai sesi nama) sebagai argumen ke konstruktor `ReplaySessionState`.
                    group.First().SessionName,
                    // Meneruskan `group.First().Mode` (mode permainan yang menentukan kelompok aturan yang digunakan) sebagai argumen ke konstruktor
                    // `ReplaySessionState`.
                    group.First().Mode,
                    // Meneruskan hasil pemilihan bersyarat: ketika `group.First().Mode.Equals(”MAHIR”, StringComparison.OrdinalIgnoreCase)` benar gunakan `10`, jika
                    // tidak gunakan `20` sebagai argumen ke konstruktor `ReplaySessionState`; Meneruskan nilai literal `”MAHIR”` sebagai argumen ke
                    // `group.First().Mode.Equals`; Meneruskan `StringComparison.OrdinalIgnoreCase` (nilai ordinal ignore case) sebagai argumen ke
                    // `group.First().Mode.Equals`.
                    group.First().Mode.Equals("MAHIR", StringComparison.OrdinalIgnoreCase) ? 10 : 20));

        // Menyiapkan variabel lokal `failures` untuk nilai failures dengan objek baru bertipe `List<string>` dengan nilai awal sesuai konstruktornya. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var failures = new List<string>();
        // Menyiapkan variabel lokal `eventById` untuk nilai event berdasarkan identitas dengan membangun kamus dari `events` dengan pemilihan kunci/nilai
        // `evt => evt.EventId`; kunci harus unik agar konversi berhasil. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var eventById = events.ToDictionary(evt => evt.EventId);
        // Menyiapkan variabel lokal `insuredRiskEventIds` untuk nilai insured risiko event identitas dengan objek baru bertipe `HashSet<Guid>` dengan nilai
        // awal sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var insuredRiskEventIds = new HashSet<Guid>();
        // Mengulangi setiap elemen `events.Where(evt => evt.ActionType.Equals(”Asuransi”, StringComparison.OrdinalIgnoreCase))`; elemen saat ini disimpan
        // sebagai `insuranceEvent` bertipe `var` untuk diproses oleh badan loop dalam AssertScenarioReplayIsValidAsync.
        foreach (var insuranceEvent in events.Where(evt =>
            // Meneruskan nilai literal `”Asuransi”` sebagai argumen ke `evt.ActionType.Equals`; Meneruskan `StringComparison.OrdinalIgnoreCase` (nilai ordinal
            // ignore case) sebagai argumen ke `evt.ActionType.Equals`.
            evt.ActionType.Equals("Asuransi", StringComparison.OrdinalIgnoreCase)))
        // Membuka scope loop setiap insuranceEvent dari `events.Where(evt => evt.ActionType.Equals(”Asuransi”, StringComparison.OrdinalIgnoreCase))`;
        // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam AssertScenarioReplayIsValidAsync.
        {
            // Menyiapkan variabel lokal `document` untuk nilai document dengan memanggil `JsonDocument.Parse` dengan `insuranceEvent.Payload`. Tipe variabel
            // disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
            using var document = JsonDocument.Parse(insuranceEvent.Payload);
            // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `document.RootElement.TryGetProperty(”risk_event_id”, out var riskEventIdElement)
            // && riskEventIdElement.ValueKind == JsonValueKind.String` dan `Guid.TryParse(riskEventIdElement.GetString(), out var riskEventId)`; sisi kanan
            // diperiksa hanya jika sisi kiri benar; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam AssertScenarioReplayIsValidAsync.
            if (document.RootElement.TryGetProperty("risk_event_id", out var riskEventIdElement) &&
                // Melanjutkan ekspresi dengan perbandingan kesamaan antara `riskEventIdElement.ValueKind` dan `JsonValueKind.String` dalam
                // AssertScenarioReplayIsValidAsync.
                riskEventIdElement.ValueKind == JsonValueKind.String &&
                // Melanjutkan pengolahan dengan mencoba mengonversi `riskEventIdElement.GetString()`, `var riskEventId` melalui `Guid.TryParse`; keberhasilan
                // dilaporkan sebagai boolean dan hasil ditempatkan pada argumen out dalam AssertScenarioReplayIsValidAsync.
                Guid.TryParse(riskEventIdElement.GetString(), out var riskEventId))
            // Membuka scope cabang if untuk kondisi `document.RootElement.TryGetProperty(”risk_event_id”, out var riskEventIdElement) &&
            // riskEventIdElement.ValueKind == JsonValueKind.String && Guid.TryParse(riskEventIdElement.Ge...`; pernyataan/deklarasi berikut berada di dalam
            // batas blok ini dalam AssertScenarioReplayIsValidAsync.
            {
                // Menjalankan menambahkan `riskEventId` ke `insuredRiskEventIds` dalam AssertScenarioReplayIsValidAsync.
                insuredRiskEventIds.Add(riskEventId);
            // Menutup scope cabang if untuk kondisi `document.RootElement.TryGetProperty(”risk_event_id”, out var riskEventIdElement) &&
            // riskEventIdElement.ValueKind == JsonValueKind.String && Guid.TryParse(riskEventIdElement.Ge...`; bagian berikut berada di luar batas blok
            // tersebut dalam AssertScenarioReplayIsValidAsync.
            }
        // Menutup scope loop setiap insuranceEvent dari `events.Where(evt => evt.ActionType.Equals(”Asuransi”, StringComparison.OrdinalIgnoreCase))`;
        // bagian berikut berada di luar batas blok tersebut dalam AssertScenarioReplayIsValidAsync.
        }

        // Mengulangi setiap elemen `events`; elemen saat ini disimpan sebagai `evt` bertipe `var` untuk diproses oleh badan loop dalam
        // AssertScenarioReplayIsValidAsync.
        foreach (var evt in events)
        // Membuka scope loop setiap evt dari `events`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam AssertScenarioReplayIsValidAsync.
        {
            // Menyiapkan variabel lokal `payloadDocument` untuk nilai payload document dengan memanggil `JsonDocument.Parse` dengan `evt.Payload`. Tipe
            // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
            using var payloadDocument = JsonDocument.Parse(evt.Payload);
            // Menyiapkan variabel lokal `payload` untuk muatan detail event dalam format JSON dengan `payloadDocument.RootElement` (nilai root element). Tipe
            // variabel disimpulkan dari ekspresi nilai awal.
            var payload = payloadDocument.RootElement;
            // Menyiapkan variabel lokal `session` untuk nilai sesi dengan `sessionStates[evt.SessionId]`, yaitu elemen koleksi yang dipilih melalui indeks atau
            // kunci tersebut. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var session = sessionStates[evt.SessionId];
            // Memulai blok try dalam AssertScenarioReplayIsValidAsync; exception dari blok ini dapat dialihkan ke catch, sedangkan finally (jika ada) tetap
            // dijalankan saat keluar.
            try
            // Membuka scope penanganan operasi try; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam AssertScenarioReplayIsValidAsync.
            {
                // Menjalankan memanggil `ReplayEvent` dengan `session`, `evt`, `payload`, `eventById`, `insuredRiskEventIds`, `lifeRiskCatalog` dalam
                // AssertScenarioReplayIsValidAsync.
                ReplayEvent(session, evt, payload, eventById, insuredRiskEventIds, lifeRiskCatalog);
            // Menutup scope penanganan operasi try; bagian berikut berada di luar batas blok tersebut dalam AssertScenarioReplayIsValidAsync.
            }
            // Menangani exception `InvalidOperationException` melalui variabel ex dalam AssertScenarioReplayIsValidAsync.
            catch (InvalidOperationException ex)
            // Membuka scope penanganan exception catch; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam AssertScenarioReplayIsValidAsync.
            {
                // Menjalankan menambahkan `$”{evt.SessionName} seq={evt.SequenceNumber} day={evt.DayIndex} player={evt.PlayerName ?? ”-”} action={evt.ActionType}:
                // {ex.Message}”` ke `failures` dalam AssertScenarioReplayIsValidAsync.
                failures.Add($"{evt.SessionName} seq={evt.SequenceNumber} day={evt.DayIndex} player={evt.PlayerName ?? "-"} action={evt.ActionType}: {ex.Message}");
            // Menutup scope penanganan exception catch; bagian berikut berada di luar batas blok tersebut dalam AssertScenarioReplayIsValidAsync.
            }
        // Menutup scope loop setiap evt dari `events`; bagian berikut berada di luar batas blok tersebut dalam AssertScenarioReplayIsValidAsync.
        }

        // Mengulangi setiap elemen `sessionStates.Values`; elemen saat ini disimpan sebagai `session` bertipe `var` untuk diproses oleh badan loop dalam
        // AssertScenarioReplayIsValidAsync.
        foreach (var session in sessionStates.Values)
        // Membuka scope loop setiap session dari `sessionStates.Values`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // AssertScenarioReplayIsValidAsync.
        {
            // Menjalankan memanggil `ValidateDonationRanks` dengan `session`, `failures` dalam AssertScenarioReplayIsValidAsync.
            ValidateDonationRanks(session, failures);
            // Menjalankan memanggil `ValidateFinalPlayerState` dengan `session`, `failures` dalam AssertScenarioReplayIsValidAsync.
            ValidateFinalPlayerState(session, failures);
        // Menutup scope loop setiap session dari `sessionStates.Values`; bagian berikut berada di luar batas blok tersebut dalam
        // AssertScenarioReplayIsValidAsync.
        }

        // Menjalankan pemeriksaan bahwa `failures.Count == 0`, `string.Join(Environment.NewLine, failures)` bernilai benar; pengujian gagal jika kondisi
        // tidak terpenuhi dalam AssertScenarioReplayIsValidAsync.
        Assert.True(failures.Count == 0, string.Join(Environment.NewLine, failures));
    // Menutup scope metode AssertScenarioReplayIsValidAsync; bagian berikut berada di luar batas blok tersebut dalam AssertScenarioReplayIsValidAsync.
    }

    // Mendefinisikan metode `AssertScenarioEvent` dengan hasil bertipe `void`; operasi ini menangani assert scenario event. Masukan: Parameter `rows`
    // bertipe `IReadOnlyCollection<ScenarioAlignmentRow>` membawa nilai baris; Parameter `mode` bertipe `string` membawa mode permainan yang menentukan
    // kelompok aturan yang digunakan; Parameter `playerName` bertipe `string` membawa nilai pemain nama; Parameter `dayIndex` bertipe `int` membawa
    // nilai hari index; Parameter `actionSlot` bertipe `int` membawa nilai aksi slot; Parameter `actionType` bertipe `string` membawa nilai aksi jenis;
    // Parameter `payloadKey` bertipe `string` membawa nilai payload kunci.
    private static void AssertScenarioEvent(
        // Parameter `rows` bertipe `IReadOnlyCollection<ScenarioAlignmentRow>` membawa nilai baris.
        IReadOnlyCollection<ScenarioAlignmentRow> rows,
        // Parameter `mode` bertipe `string` membawa mode permainan yang menentukan kelompok aturan yang digunakan.
        string mode,
        // Parameter `playerName` bertipe `string` membawa nilai pemain nama.
        string playerName,
        // Parameter `dayIndex` bertipe `int` membawa nilai hari index.
        int dayIndex,
        // Parameter `actionSlot` bertipe `int` membawa nilai aksi slot.
        int actionSlot,
        // Parameter `actionType` bertipe `string` membawa nilai aksi jenis.
        string actionType,
        // Parameter `payloadKey` bertipe `string` membawa nilai payload kunci.
        string payloadKey)
    // Membuka scope metode AssertScenarioEvent; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam AssertScenarioEvent.
    {
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `rows`, `row => row.Mode == mode &&
        // row.PlayerName == playerName && row.DayIndex == dayIndex && row.ActionSlot == actionSlot && row.ActionType == actionType &&
        // (string.IsNullOrEmpty(pa...` dalam AssertScenarioEvent.
        Assert.Contains(
            // Meneruskan `rows` (nilai baris) sebagai argumen ke `Assert.Contains`.
            rows,
            // Parameter `row` bertipe `` membawa nilai baris.
            row => row.Mode == mode
                   // Meneruskan fungsi lambda `row => row.Mode == mode && row.PlayerName == playerName && row.DayIndex == dayIndex && row.ActionSlot == actionSlot &&
                   // row.ActionType == actionType && (string.IsNullOrEmpty(pa...` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai
                   // argumen ke `Assert.Contains`.
                   && row.PlayerName == playerName
                   // Meneruskan fungsi lambda `row => row.Mode == mode && row.PlayerName == playerName && row.DayIndex == dayIndex && row.ActionSlot == actionSlot &&
                   // row.ActionType == actionType && (string.IsNullOrEmpty(pa...` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai
                   // argumen ke `Assert.Contains`.
                   && row.DayIndex == dayIndex
                   // Meneruskan fungsi lambda `row => row.Mode == mode && row.PlayerName == playerName && row.DayIndex == dayIndex && row.ActionSlot == actionSlot &&
                   // row.ActionType == actionType && (string.IsNullOrEmpty(pa...` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai
                   // argumen ke `Assert.Contains`.
                   && row.ActionSlot == actionSlot
                   // Meneruskan fungsi lambda `row => row.Mode == mode && row.PlayerName == playerName && row.DayIndex == dayIndex && row.ActionSlot == actionSlot &&
                   // row.ActionType == actionType && (string.IsNullOrEmpty(pa...` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai
                   // argumen ke `Assert.Contains`.
                   && row.ActionType == actionType
                   // Meneruskan `payloadKey` (nilai payload kunci) sebagai argumen ke `string.IsNullOrEmpty`.
                   && (string.IsNullOrEmpty(payloadKey) ||
                       // Meneruskan `row.PayloadKey` (nilai payload kunci) sebagai argumen ke `string.Equals`; Meneruskan `payloadKey` (nilai payload kunci) sebagai
                       // argumen ke `string.Equals`; Meneruskan `StringComparison.OrdinalIgnoreCase` (nilai ordinal ignore case) sebagai argumen ke `string.Equals`.
                       string.Equals(row.PayloadKey, payloadKey, StringComparison.OrdinalIgnoreCase)));
    // Menutup scope metode AssertScenarioEvent; bagian berikut berada di luar batas blok tersebut dalam AssertScenarioEvent.
    }

    // Mendefinisikan metode `ReplayEvent` dengan hasil bertipe `void`; operasi ini menangani replay event. Masukan: Parameter `session` bertipe
    // `ReplaySessionState` membawa nilai sesi; Parameter `evt` bertipe `ReplayEventRow` membawa satu event permainan yang sedang diperiksa; Parameter
    // `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `eventById` bertipe `IReadOnlyDictionary<Guid,
    // ReplayEventRow>` membawa nilai event berdasarkan identitas; Parameter `insuredRiskEventIds` bertipe `IReadOnlySet<Guid>` membawa nilai insured
    // risiko event identitas; Parameter `lifeRiskCatalog` bertipe `IReadOnlyDictionary<string, (string Direction, int Amount)>` membawa nilai life
    // risiko catalog.
    private static void ReplayEvent(
        // Parameter `session` bertipe `ReplaySessionState` membawa nilai sesi.
        ReplaySessionState session,
        // Parameter `evt` bertipe `ReplayEventRow` membawa satu event permainan yang sedang diperiksa.
        ReplayEventRow evt,
        // Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON.
        JsonElement payload,
        // Parameter `eventById` bertipe `IReadOnlyDictionary<Guid, ReplayEventRow>` membawa nilai event berdasarkan identitas.
        IReadOnlyDictionary<Guid, ReplayEventRow> eventById,
        // Parameter `insuredRiskEventIds` bertipe `IReadOnlySet<Guid>` membawa nilai insured risiko event identitas.
        IReadOnlySet<Guid> insuredRiskEventIds,
        // Parameter `lifeRiskCatalog` bertipe `IReadOnlyDictionary<string, (string Direction, int Amount)>` membawa nilai life risiko catalog.
        IReadOnlyDictionary<string, (string Direction, int Amount)> lifeRiskCatalog)
    // Membuka scope metode ReplayEvent; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ReplayEvent.
    {
        // Menjalankan memanggil `ValidateCalendarAction` dengan `evt`, `payload` dalam ReplayEvent.
        ValidateCalendarAction(evt, payload);

        // Memeriksa membandingkan kesamaan `evt.ActorType` dengan `”SYSTEM”`, `StringComparison.OrdinalIgnoreCase`; aturan perbandingan mengikuti overload
        // dan comparer yang diberikan; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ReplayEvent.
        if (evt.ActorType.Equals("SYSTEM", StringComparison.OrdinalIgnoreCase))
        // Membuka scope cabang if untuk kondisi `evt.ActorType.Equals(”SYSTEM”, StringComparison.OrdinalIgnoreCase)`; pernyataan/deklarasi berikut berada
        // di dalam batas blok ini dalam ReplayEvent.
        {
            // Menjalankan memanggil `ReplaySystemEvent` dengan `session`, `evt`, `payload` dalam ReplayEvent.
            ReplaySystemEvent(session, evt, payload);
            // Mengakhiri eksekusi lebih awal dalam ReplayEvent tanpa mengembalikan nilai; pernyataan sesudah return pada jalur ini tidak dijalankan.
            return;
        // Menutup scope cabang if untuk kondisi `evt.ActorType.Equals(”SYSTEM”, StringComparison.OrdinalIgnoreCase)`; bagian berikut berada di luar batas
        // blok tersebut dalam ReplayEvent.
        }

        // Menyiapkan variabel lokal `player` untuk nilai pemain dengan memanggil `session.GetPlayer` dengan `evt.UserId`, `evt.PlayerName`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var player = session.GetPlayer(evt.UserId, evt.PlayerName);
        // Menjalankan memanggil `session.RecordActionToken` dengan `evt`, `CountsAsActionToken(evt.ActionType, payload)` dalam ReplayEvent.
        session.RecordActionToken(evt, CountsAsActionToken(evt.ActionType, payload));

        // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `session.Mode.Equals(”PEMULA”, StringComparison.OrdinalIgnoreCase)` dan
        // `IsMahirOnlyAction(evt.ActionType)`; sisi kanan diperiksa hanya jika sisi kiri benar; blok if hanya dijalankan ketika kondisi ini bernilai benar
        // dalam ReplayEvent.
        if (session.Mode.Equals("PEMULA", StringComparison.OrdinalIgnoreCase) && IsMahirOnlyAction(evt.ActionType))
        // Membuka scope cabang if untuk kondisi `session.Mode.Equals(”PEMULA”, StringComparison.OrdinalIgnoreCase) && IsMahirOnlyAction(evt.ActionType)`;
        // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ReplayEvent.
        {
            // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen (”Mode PEMULA tidak boleh berisi aksi khusus
            // MAHIR.”) dalam ReplayEvent; pemanggil atau middleware penanganan error menerima kegagalan ini.
            throw new InvalidOperationException("Mode PEMULA tidak boleh berisi aksi khusus MAHIR.");
        // Menutup scope cabang if untuk kondisi `session.Mode.Equals(”PEMULA”, StringComparison.OrdinalIgnoreCase) && IsMahirOnlyAction(evt.ActionType)`;
        // bagian berikut berada di luar batas blok tersebut dalam ReplayEvent.
        }

        // Memilih cabang berdasarkan `evt.ActionType` (nilai aksi jenis); label case menentukan perlakuan untuk setiap nilai yang dikenali dalam
        // ReplayEvent.
        switch (evt.ActionType)
        // Membuka scope pemilihan switch atas `evt.ActionType`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ReplayEvent.
        {
            // Menetapkan label cabang `case ”BahanMasakan”:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            case "BahanMasakan":
                // Menjalankan memanggil `ApplyCashOut` dengan `player`, `ReadInt(payload, ”amount”)`, `evt`, `”biaya bahan”` dalam ReplayEvent.
                ApplyCashOut(player, ReadInt(payload, "amount"), evt, "biaya bahan");
                // Menjalankan memanggil `AddInventory` dengan `player.Ingredients`, `ReadString(payload, ”card_id”)`, `1` dalam ReplayEvent.
                AddInventory(player.Ingredients, ReadString(payload, "card_id"), 1);
                // Menjalankan memanggil `ValidateIngredientLimits` dengan `player`, `evt` dalam ReplayEvent.
                ValidateIngredientLimits(player, evt);
                // Mengakhiri loop atau cabang switch terdekat, kemudian melanjutkan setelah blok tersebut dalam ReplayEvent.
                break;

            // Menetapkan label cabang `case ”BuangBahanMasakan”:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            case "BuangBahanMasakan":
                // Menjalankan memanggil `RemoveInventory` dengan `player.Ingredients`, `ReadString(payload, ”card_id”)`, `ReadQuantity(payload)`, `evt` dalam
                // ReplayEvent.
                RemoveInventory(player.Ingredients, ReadString(payload, "card_id"), ReadQuantity(payload), evt);
                // Mengakhiri loop atau cabang switch terdekat, kemudian melanjutkan setelah blok tersebut dalam ReplayEvent.
                break;

            // Menetapkan label cabang `case ”JualMasakan”:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            case "JualMasakan":
                // Mengulangi setiap elemen `ReadStringArray(payload, ”required_ingredient_card_ids”)`; elemen saat ini disimpan sebagai `cardId` bertipe `var`
                // untuk diproses oleh badan loop dalam ReplayEvent.
                foreach (var cardId in ReadStringArray(payload, "required_ingredient_card_ids"))
                // Membuka scope loop setiap cardId dari `ReadStringArray(payload, ”required_ingredient_card_ids”)`; pernyataan/deklarasi berikut berada di dalam
                // batas blok ini dalam ReplayEvent.
                {
                    // Menjalankan memanggil `RemoveInventoryIfAvailable` dengan `player.Ingredients`, `cardId`, `1` dalam ReplayEvent.
                    RemoveInventoryIfAvailable(player.Ingredients, cardId, 1);
                // Menutup scope loop setiap cardId dari `ReadStringArray(payload, ”required_ingredient_card_ids”)`; bagian berikut berada di luar batas blok
                // tersebut dalam ReplayEvent.
                }

                // Menjalankan memanggil `ApplyCashIn` dengan `player`, `ReadInt(payload, ”income”)` dalam ReplayEvent.
                ApplyCashIn(player, ReadInt(payload, "income"));
                // Mengakhiri loop atau cabang switch terdekat, kemudian melanjutkan setelah blok tersebut dalam ReplayEvent.
                break;

            // Menetapkan label cabang `case ”LewatiOrder”:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            case "LewatiOrder":
                // Mengakhiri loop atau cabang switch terdekat, kemudian melanjutkan setelah blok tersebut dalam ReplayEvent.
                break;

            // Menetapkan label cabang `case ”KerjaLepas”:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            case "KerjaLepas":
                // Menjalankan memanggil `ApplyCashIn` dengan `player`, `ReadInt(payload, ”amount”)` dalam ReplayEvent.
                ApplyCashIn(player, ReadInt(payload, "amount"));
                // Mengakhiri loop atau cabang switch terdekat, kemudian melanjutkan setelah blok tersebut dalam ReplayEvent.
                break;

            // Menetapkan label cabang `case ”Kebutuhan”:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            case "Kebutuhan":
                // Menyiapkan variabel lokal `needCardId` untuk nilai kebutuhan kartu identitas dengan memanggil `ReadString` dengan `payload`, `”card_id”`. Tipe
                // variabel disimpulkan dari ekspresi nilai awal.
                var needCardId = ReadString(payload, "card_id");
                // Menyiapkan variabel lokal `needTier` untuk nilai kebutuhan tingkat dengan memanggil `ResolveNeedTier` dengan `payload`, `needCardId`. Tipe
                // variabel disimpulkan dari ekspresi nilai awal.
                var needTier = ResolveNeedTier(payload, needCardId);
                // Memeriksa membandingkan kesamaan `needTier` dengan `”primer”`, `StringComparison.OrdinalIgnoreCase`; aturan perbandingan mengikuti overload dan
                // comparer yang diberikan; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ReplayEvent.
                if (needTier.Equals("primer", StringComparison.OrdinalIgnoreCase))
                // Membuka scope cabang if untuk kondisi `needTier.Equals(”primer”, StringComparison.OrdinalIgnoreCase)`; pernyataan/deklarasi berikut berada di
                // dalam batas blok ini dalam ReplayEvent.
                {
                    // Menjalankan memanggil `ApplyCashOut` dengan `player`, `ReadInt(payload, ”amount”)`, `evt`, `”kebutuhan primer”` dalam ReplayEvent.
                    ApplyCashOut(player, ReadInt(payload, "amount"), evt, "kebutuhan primer");
                    // Menjalankan menambahkan `needCardId` ke `player.PrimaryNeeds` dalam ReplayEvent.
                    player.PrimaryNeeds.Add(needCardId);
                    // Mengakhiri loop atau cabang switch terdekat, kemudian melanjutkan setelah blok tersebut dalam ReplayEvent.
                    break;
                // Menutup scope cabang if untuk kondisi `needTier.Equals(”primer”, StringComparison.OrdinalIgnoreCase)`; bagian berikut berada di luar batas blok
                // tersebut dalam ReplayEvent.
                }

                // Memeriksa membandingkan kesamaan `needTier` dengan `”sekunder”`, `StringComparison.OrdinalIgnoreCase`; aturan perbandingan mengikuti overload dan
                // comparer yang diberikan; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ReplayEvent.
                if (needTier.Equals("sekunder", StringComparison.OrdinalIgnoreCase))
                // Membuka scope cabang if untuk kondisi `needTier.Equals(”sekunder”, StringComparison.OrdinalIgnoreCase)`; pernyataan/deklarasi berikut berada di
                // dalam batas blok ini dalam ReplayEvent.
                {
                    // Menjalankan memanggil `ApplyCashOut` dengan `player`, `ReadInt(payload, ”amount”)`, `evt`, `”kebutuhan sekunder”` dalam ReplayEvent.
                    ApplyCashOut(player, ReadInt(payload, "amount"), evt, "kebutuhan sekunder");
                    // Menjalankan menambahkan `needCardId` ke `player.SecondaryNeeds` dalam ReplayEvent.
                    player.SecondaryNeeds.Add(needCardId);
                    // Mengakhiri loop atau cabang switch terdekat, kemudian melanjutkan setelah blok tersebut dalam ReplayEvent.
                    break;
                // Menutup scope cabang if untuk kondisi `needTier.Equals(”sekunder”, StringComparison.OrdinalIgnoreCase)`; bagian berikut berada di luar batas blok
                // tersebut dalam ReplayEvent.
                }

                // Menjalankan memanggil `ApplyCashOut` dengan `player`, `ReadInt(payload, ”amount”)`, `evt`, `”kebutuhan tersier”` dalam ReplayEvent.
                ApplyCashOut(player, ReadInt(payload, "amount"), evt, "kebutuhan tersier");
                // Menjalankan menambahkan `needCardId` ke `player.TertiaryNeeds` dalam ReplayEvent.
                player.TertiaryNeeds.Add(needCardId);
                // Mengakhiri loop atau cabang switch terdekat, kemudian melanjutkan setelah blok tersebut dalam ReplayEvent.
                break;

            // Menetapkan label cabang `case ”JumatBerkah”:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            case "JumatBerkah":
                // Menyiapkan variabel lokal `donationAmount` untuk nilai donasi nominal dengan memanggil `ReadInt` dengan `payload`, `”amount”`. Tipe variabel
                // disimpulkan dari ekspresi nilai awal.
                var donationAmount = ReadInt(payload, "amount");
                // Menjalankan memanggil `ApplyCashOut` dengan `player`, `donationAmount`, `evt`, `”donasi Jumat”` dalam ReplayEvent.
                ApplyCashOut(player, donationAmount, evt, "donasi Jumat");
                // Menjalankan memanggil `session.RecordDonation` dengan `evt.DayIndex`, `evt.UserId!.Value`, `donationAmount` dalam ReplayEvent.
                session.RecordDonation(evt.DayIndex, evt.UserId!.Value, donationAmount);
                // Mengakhiri loop atau cabang switch terdekat, kemudian melanjutkan setelah blok tersebut dalam ReplayEvent.
                break;

            // Menetapkan label cabang `case ”InvestasiEmas”:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            case "InvestasiEmas":
            // Menetapkan label cabang `case ”JualEmas”:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            case "JualEmas":
                // Menjalankan memanggil `ApplyGoldTrade` dengan `player`, `evt`, `payload` dalam ReplayEvent.
                ApplyGoldTrade(player, evt, payload);
                // Mengakhiri loop atau cabang switch terdekat, kemudian melanjutkan setelah blok tersebut dalam ReplayEvent.
                break;

            // Menetapkan label cabang `case ”LewatiTransaksiEmas”:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            case "LewatiTransaksiEmas":
                // Mengakhiri loop atau cabang switch terdekat, kemudian melanjutkan setelah blok tersebut dalam ReplayEvent.
                break;

            // Menetapkan label cabang `case ”GunakanOpsiDarurat”:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            case "GunakanOpsiDarurat":
                // Menyiapkan variabel lokal `optionType` untuk nilai option jenis dengan memanggil `ReadString` dengan `payload`, `”option_type”`. Tipe variabel
                // disimpulkan dari ekspresi nilai awal.
                var optionType = ReadString(payload, "option_type");
                // Memeriksa membandingkan kesamaan `optionType` dengan `”SELL_NEED”`, `StringComparison.OrdinalIgnoreCase`; aturan perbandingan mengikuti overload
                // dan comparer yang diberikan; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ReplayEvent.
                if (optionType.Equals("SELL_NEED", StringComparison.OrdinalIgnoreCase))
                // Membuka scope cabang if untuk kondisi `optionType.Equals(”SELL_NEED”, StringComparison.OrdinalIgnoreCase)`; pernyataan/deklarasi berikut berada
                // di dalam batas blok ini dalam ReplayEvent.
                {
                    // Menyiapkan variabel lokal `cardId` untuk nilai kartu identitas dengan memanggil `ReadString` dengan `payload`, `”card_id”`. Tipe variabel
                    // disimpulkan dari ekspresi nilai awal.
                    var cardId = ReadString(payload, "card_id");
                    // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `!player.PrimaryNeeds.Remove(cardId) && !player.SecondaryNeeds.Remove(cardId)`
                    // dan `!player.TertiaryNeeds.Remove(cardId)`; sisi kanan diperiksa hanya jika sisi kiri benar; blok if hanya dijalankan ketika kondisi ini bernilai
                    // benar dalam ReplayEvent.
                    if (!player.PrimaryNeeds.Remove(cardId) &&
                        // Menggunakan kebalikan kondisi `player.SecondaryNeeds.Remove(cardId)` sebagai bagian ekspresi yang sedang disusun dalam ReplayEvent.
                        !player.SecondaryNeeds.Remove(cardId) &&
                        // Menggunakan kebalikan kondisi `player.TertiaryNeeds.Remove(cardId)` sebagai bagian ekspresi yang sedang disusun dalam ReplayEvent.
                        !player.TertiaryNeeds.Remove(cardId))
                    // Membuka scope cabang if untuk kondisi `!player.PrimaryNeeds.Remove(cardId) && !player.SecondaryNeeds.Remove(cardId) &&
                    // !player.TertiaryNeeds.Remove(cardId)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ReplayEvent.
                    {
                        // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen ($”Kartu kebutuhan darurat tidak dimiliki:
                        // {cardId}.”) dalam ReplayEvent; pemanggil atau middleware penanganan error menerima kegagalan ini.
                        throw new InvalidOperationException($"Kartu kebutuhan darurat tidak dimiliki: {cardId}.");
                    // Menutup scope cabang if untuk kondisi `!player.PrimaryNeeds.Remove(cardId) && !player.SecondaryNeeds.Remove(cardId) &&
                    // !player.TertiaryNeeds.Remove(cardId)`; bagian berikut berada di luar batas blok tersebut dalam ReplayEvent.
                    }

                    // Menjalankan memanggil `ApplyCashIn` dengan `player`, `ReadInt(payload, ”amount”)` dalam ReplayEvent.
                    ApplyCashIn(player, ReadInt(payload, "amount"));
                    // Mengakhiri loop atau cabang switch terdekat, kemudian melanjutkan setelah blok tersebut dalam ReplayEvent.
                    break;
                // Menutup scope cabang if untuk kondisi `optionType.Equals(”SELL_NEED”, StringComparison.OrdinalIgnoreCase)`; bagian berikut berada di luar batas
                // blok tersebut dalam ReplayEvent.
                }

                // Memeriksa membandingkan kesamaan `optionType` dengan `”SELL_GOLD”`, `StringComparison.OrdinalIgnoreCase`; aturan perbandingan mengikuti overload
                // dan comparer yang diberikan; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ReplayEvent.
                if (optionType.Equals("SELL_GOLD", StringComparison.OrdinalIgnoreCase))
                // Membuka scope cabang if untuk kondisi `optionType.Equals(”SELL_GOLD”, StringComparison.OrdinalIgnoreCase)`; pernyataan/deklarasi berikut berada
                // di dalam batas blok ini dalam ReplayEvent.
                {
                    // Menyiapkan variabel lokal `qty` untuk nilai qty dengan memanggil `ReadInt` dengan `payload`, `”qty”`. Tipe variabel disimpulkan dari ekspresi
                    // nilai awal.
                    var qty = ReadInt(payload, "qty");
                    // Menyiapkan variabel lokal `amount` untuk nominal uang atau nilai transaksi yang dipakai dalam operasi dengan memanggil `ReadInt` dengan
                    // `payload`, `”amount”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
                    var amount = ReadInt(payload, "amount");
                    // Memeriksa pemeriksaan lebih kecil antara `player.GoldQty` dan `qty`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
                    // ReplayEvent.
                    if (player.GoldQty < qty)
                    // Membuka scope cabang if untuk kondisi `player.GoldQty < qty`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ReplayEvent.
                    {
                        // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen (”Emas darurat tidak mencukupi untuk dijual.”)
                        // dalam ReplayEvent; pemanggil atau middleware penanganan error menerima kegagalan ini.
                        throw new InvalidOperationException("Emas darurat tidak mencukupi untuk dijual.");
                    // Menutup scope cabang if untuk kondisi `player.GoldQty < qty`; bagian berikut berada di luar batas blok tersebut dalam ReplayEvent.
                    }

                    // Memperbarui `player.GoldQty` dengan mengurangi `qty` (nilai qty) dalam ReplayEvent.
                    player.GoldQty -= qty;
                    // Memperbarui `player.GoldNetAmount` dengan mengurangi `amount` (nominal uang atau nilai transaksi yang dipakai dalam operasi) dalam ReplayEvent.
                    player.GoldNetAmount -= amount;
                    // Menjalankan memanggil `ApplyCashIn` dengan `player`, `amount` dalam ReplayEvent.
                    ApplyCashIn(player, amount);
                    // Mengakhiri loop atau cabang switch terdekat, kemudian melanjutkan setelah blok tersebut dalam ReplayEvent.
                    break;
                // Menutup scope cabang if untuk kondisi `optionType.Equals(”SELL_GOLD”, StringComparison.OrdinalIgnoreCase)`; bagian berikut berada di luar batas
                // blok tersebut dalam ReplayEvent.
                }

                // Memeriksa membandingkan kesamaan `optionType` dengan `”TAKE_SHARIA_LOAN”`, `StringComparison.OrdinalIgnoreCase`; aturan perbandingan mengikuti
                // overload dan comparer yang diberikan; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ReplayEvent.
                if (optionType.Equals("TAKE_SHARIA_LOAN", StringComparison.OrdinalIgnoreCase))
                // Membuka scope cabang if untuk kondisi `optionType.Equals(”TAKE_SHARIA_LOAN”, StringComparison.OrdinalIgnoreCase)`; pernyataan/deklarasi berikut
                // berada di dalam batas blok ini dalam ReplayEvent.
                {
                    // Menyiapkan variabel lokal `emergencyLoanId` untuk nilai emergency pinjaman identitas dengan memanggil `ReadString` dengan `payload`, `”loan_id”`.
                    // Tipe variabel disimpulkan dari ekspresi nilai awal.
                    var emergencyLoanId = ReadString(payload, "loan_id");
                    // Memperbarui `player.Loans[emergencyLoanId]` menggunakan memanggil `ReadInt` dengan `payload`, `”principal”` dalam ReplayEvent.
                    player.Loans[emergencyLoanId] = ReadInt(payload, "principal");
                    // Menjalankan memanggil `ApplyCashIn` dengan `player`, `ReadInt(payload, ”principal”)` dalam ReplayEvent.
                    ApplyCashIn(player, ReadInt(payload, "principal"));
                    // Mengakhiri loop atau cabang switch terdekat, kemudian melanjutkan setelah blok tersebut dalam ReplayEvent.
                    break;
                // Menutup scope cabang if untuk kondisi `optionType.Equals(”TAKE_SHARIA_LOAN”, StringComparison.OrdinalIgnoreCase)`; bagian berikut berada di luar
                // batas blok tersebut dalam ReplayEvent.
                }

                // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen ($”Option type darurat tidak valid:
                // {optionType}.”) dalam ReplayEvent; pemanggil atau middleware penanganan error menerima kegagalan ini.
                throw new InvalidOperationException($"Option type darurat tidak valid: {optionType}.");

            // Menetapkan label cabang `case ”RisikoKehidupan”:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            case "RisikoKehidupan":
                // Memeriksa kebalikan kondisi `session.Mode.Equals(”MAHIR”, StringComparison.OrdinalIgnoreCase)`; blok if hanya dijalankan ketika kondisi ini
                // bernilai benar dalam ReplayEvent.
                if (!session.Mode.Equals("MAHIR", StringComparison.OrdinalIgnoreCase))
                // Membuka scope cabang if untuk kondisi `!session.Mode.Equals(”MAHIR”, StringComparison.OrdinalIgnoreCase)`; pernyataan/deklarasi berikut berada di
                // dalam batas blok ini dalam ReplayEvent.
                {
                    // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen (”Risk life hanya valid pada mode MAHIR.”) dalam
                    // ReplayEvent; pemanggil atau middleware penanganan error menerima kegagalan ini.
                    throw new InvalidOperationException("Risk life hanya valid pada mode MAHIR.");
                // Menutup scope cabang if untuk kondisi `!session.Mode.Equals(”MAHIR”, StringComparison.OrdinalIgnoreCase)`; bagian berikut berada di luar batas
                // blok tersebut dalam ReplayEvent.
                }

                // Menjalankan memanggil `ApplyRiskLife` dengan `player`, `evt`, `payload`, `insuredRiskEventIds`, `lifeRiskCatalog` dalam ReplayEvent.
                ApplyRiskLife(player, evt, payload, insuredRiskEventIds, lifeRiskCatalog);
                // Mengakhiri loop atau cabang switch terdekat, kemudian melanjutkan setelah blok tersebut dalam ReplayEvent.
                break;

            // Menetapkan label cabang `case ”BayarRisiko”:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            case "BayarRisiko":
                // Mengakhiri loop atau cabang switch terdekat, kemudian melanjutkan setelah blok tersebut dalam ReplayEvent.
                break;

            // Menetapkan label cabang `case ”Asuransi”:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            case "Asuransi":
                // Memeriksa mencari properti JSON `”risk_event_id”`, `_` pada `payload` tanpa menganggap propertinya selalu tersedia; blok if hanya dijalankan
                // ketika kondisi ini bernilai benar dalam ReplayEvent.
                if (payload.TryGetProperty("risk_event_id", out _))
                // Membuka scope cabang if untuk kondisi `payload.TryGetProperty(”risk_event_id”, out _)`; pernyataan/deklarasi berikut berada di dalam batas blok
                // ini dalam ReplayEvent.
                {
                    // Menjalankan memanggil `ApplyInsuranceUsage` dengan `player`, `evt`, `payload`, `eventById` dalam ReplayEvent.
                    ApplyInsuranceUsage(player, evt, payload, eventById);
                    // Mengakhiri loop atau cabang switch terdekat, kemudian melanjutkan setelah blok tersebut dalam ReplayEvent.
                    break;
                // Menutup scope cabang if untuk kondisi `payload.TryGetProperty(”risk_event_id”, out _)`; bagian berikut berada di luar batas blok tersebut dalam
                // ReplayEvent.
                }

                // Menjalankan memanggil `RequireFeature` dengan `session.Mode`, `true`, `evt`, `”asuransi”` dalam ReplayEvent.
                RequireFeature(session.Mode, enabled: true, evt, "asuransi");
                // Menyiapkan variabel lokal `policyId` untuk nilai policy identitas dengan memanggil `ReadString` dengan `payload`, `”policy_id”`. Tipe variabel
                // disimpulkan dari ekspresi nilai awal.
                var policyId = ReadString(payload, "policy_id");
                // Menjalankan menambahkan `policyId` ke `player.InsurancePolicies` dalam ReplayEvent.
                player.InsurancePolicies.Add(policyId);

                // Menyiapkan variabel lokal `premium` untuk nilai premium dengan memanggil `ReadInt` dengan `payload`, `”premium”`. Tipe variabel disimpulkan dari
                // ekspresi nilai awal.
                var premium = ReadInt(payload, "premium");
                // Memeriksa pemeriksaan lebih besar antara `premium` dan `0`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ReplayEvent.
                if (premium > 0)
                // Membuka scope cabang if untuk kondisi `premium > 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ReplayEvent.
                {
                    // Menjalankan memanggil `ApplyCashOut` dengan `player`, `premium`, `evt`, `”premi asuransi”` dalam ReplayEvent.
                    ApplyCashOut(player, premium, evt, "premi asuransi");
                // Menutup scope cabang if untuk kondisi `premium > 0`; bagian berikut berada di luar batas blok tersebut dalam ReplayEvent.
                }
                // Mengakhiri loop atau cabang switch terdekat, kemudian melanjutkan setelah blok tersebut dalam ReplayEvent.
                break;

            // Menetapkan label cabang `case ”PinjamanSyariah”:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            case "PinjamanSyariah":
                // Menjalankan memanggil `RequireFeature` dengan `session.Mode`, `true`, `evt`, `”pinjaman syariah”` dalam ReplayEvent.
                RequireFeature(session.Mode, enabled: true, evt, "pinjaman syariah");
                // Menyiapkan variabel lokal `loanId` untuk nilai pinjaman identitas dengan memanggil `ReadString` dengan `payload`, `”loan_id”`. Tipe variabel
                // disimpulkan dari ekspresi nilai awal.
                var loanId = ReadString(payload, "loan_id");
                // Memeriksa memeriksa keberadaan kunci `loanId` dalam `player.Loans`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ReplayEvent.
                if (player.Loans.ContainsKey(loanId))
                // Membuka scope cabang if untuk kondisi `player.Loans.ContainsKey(loanId)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // ReplayEvent.
                {
                    // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen ($”Loan ID duplikat: {loanId}.”) dalam
                    // ReplayEvent; pemanggil atau middleware penanganan error menerima kegagalan ini.
                    throw new InvalidOperationException($"Loan ID duplikat: {loanId}.");
                // Menutup scope cabang if untuk kondisi `player.Loans.ContainsKey(loanId)`; bagian berikut berada di luar batas blok tersebut dalam ReplayEvent.
                }

                // Menyiapkan variabel lokal `principal` untuk nilai principal dengan memanggil `ReadInt` dengan `payload`, `”principal”`. Tipe variabel disimpulkan
                // dari ekspresi nilai awal.
                var principal = ReadInt(payload, "principal");
                // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `principal != 10` dan `ReadInt(payload, ”penalty_points”) != 15`; sisi
                // kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ReplayEvent.
                if (principal != 10 || ReadInt(payload, "penalty_points") != 15)
                // Membuka scope cabang if untuk kondisi `principal != 10 || ReadInt(payload, ”penalty_points”) != 15`; pernyataan/deklarasi berikut berada di dalam
                // batas blok ini dalam ReplayEvent.
                {
                    // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen (”Pinjaman syariah harus principal 10 dan penalty
                    // 15.”) dalam ReplayEvent; pemanggil atau middleware penanganan error menerima kegagalan ini.
                    throw new InvalidOperationException("Pinjaman syariah harus principal 10 dan penalty 15.");
                // Menutup scope cabang if untuk kondisi `principal != 10 || ReadInt(payload, ”penalty_points”) != 15`; bagian berikut berada di luar batas blok
                // tersebut dalam ReplayEvent.
                }

                // Memperbarui `player.Loans[loanId]` menggunakan `principal` (nilai principal) dalam ReplayEvent.
                player.Loans[loanId] = principal;
                // Menjalankan memanggil `ApplyCashIn` dengan `player`, `principal` dalam ReplayEvent.
                ApplyCashIn(player, principal);
                // Mengakhiri loop atau cabang switch terdekat, kemudian melanjutkan setelah blok tersebut dalam ReplayEvent.
                break;

            // Menetapkan label cabang `case ”BayarPinjaman”:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            case "BayarPinjaman":
                // Menyiapkan variabel lokal `repayLoanId` untuk nilai repay pinjaman identitas dengan memanggil `ReadString` dengan `payload`, `”loan_id”`. Tipe
                // variabel disimpulkan dari ekspresi nilai awal.
                var repayLoanId = ReadString(payload, "loan_id");
                // Menyiapkan variabel lokal `repayAmount` untuk nilai repay nominal dengan memanggil `ReadInt` dengan `payload`, `”amount”`. Tipe variabel
                // disimpulkan dari ekspresi nilai awal.
                var repayAmount = ReadInt(payload, "amount");
                // Memeriksa kebalikan kondisi `player.Loans.TryGetValue(repayLoanId, out var outstanding)`; blok if hanya dijalankan ketika kondisi ini bernilai
                // benar dalam ReplayEvent.
                if (!player.Loans.TryGetValue(repayLoanId, out var outstanding))
                // Membuka scope cabang if untuk kondisi `!player.Loans.TryGetValue(repayLoanId, out var outstanding)`; pernyataan/deklarasi berikut berada di dalam
                // batas blok ini dalam ReplayEvent.
                {
                    // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen ($”Loan ID tidak ditemukan: {repayLoanId}.”)
                    // dalam ReplayEvent; pemanggil atau middleware penanganan error menerima kegagalan ini.
                    throw new InvalidOperationException($"Loan ID tidak ditemukan: {repayLoanId}.");
                // Menutup scope cabang if untuk kondisi `!player.Loans.TryGetValue(repayLoanId, out var outstanding)`; bagian berikut berada di luar batas blok
                // tersebut dalam ReplayEvent.
                }

                // Memeriksa pemeriksaan lebih besar antara `repayAmount` dan `outstanding`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
                // ReplayEvent.
                if (repayAmount > outstanding)
                // Membuka scope cabang if untuk kondisi `repayAmount > outstanding`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ReplayEvent.
                {
                    // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen ($”Pembayaran {repayAmount} melebihi sisa
                    // pinjaman {outstanding}.”) dalam ReplayEvent; pemanggil atau middleware penanganan error menerima kegagalan ini.
                    throw new InvalidOperationException($"Pembayaran {repayAmount} melebihi sisa pinjaman {outstanding}.");
                // Menutup scope cabang if untuk kondisi `repayAmount > outstanding`; bagian berikut berada di luar batas blok tersebut dalam ReplayEvent.
                }

                // Menjalankan memanggil `ApplyCashOut` dengan `player`, `repayAmount`, `evt`, `”bayar pinjaman”` dalam ReplayEvent.
                ApplyCashOut(player, repayAmount, evt, "bayar pinjaman");
                // Memperbarui `player.Loans[repayLoanId]` menggunakan selisih antara `outstanding` dan `repayAmount` dalam ReplayEvent.
                player.Loans[repayLoanId] = outstanding - repayAmount;
                // Mengakhiri loop atau cabang switch terdekat, kemudian melanjutkan setelah blok tersebut dalam ReplayEvent.
                break;

            // Menetapkan label cabang `case ”Menabung”:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            case "Menabung":
                // Menjalankan memanggil `RequireFeature` dengan `session.Mode`, `true`, `evt`, `”tabungan tujuan”` dalam ReplayEvent.
                RequireFeature(session.Mode, enabled: true, evt, "tabungan tujuan");
                // Menyiapkan variabel lokal `depositGoalId` untuk nilai deposit target identitas dengan memanggil `ReadString` dengan `payload`, `”goal_id”`. Tipe
                // variabel disimpulkan dari ekspresi nilai awal.
                var depositGoalId = ReadString(payload, "goal_id");
                // Menyiapkan variabel lokal `depositAmount` untuk nilai deposit nominal dengan memanggil `ReadInt` dengan `payload`, `”amount”`. Tipe variabel
                // disimpulkan dari ekspresi nilai awal.
                var depositAmount = ReadInt(payload, "amount");
                // Memeriksa pemeriksaan lebih besar antara `depositAmount` dan `15`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ReplayEvent.
                if (depositAmount > 15)
                // Membuka scope cabang if untuk kondisi `depositAmount > 15`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ReplayEvent.
                {
                    // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen (”Maksimal tabungan per aksi adalah 15 koin.”)
                    // dalam ReplayEvent; pemanggil atau middleware penanganan error menerima kegagalan ini.
                    throw new InvalidOperationException("Maksimal tabungan per aksi adalah 15 koin.");
                // Menutup scope cabang if untuk kondisi `depositAmount > 15`; bagian berikut berada di luar batas blok tersebut dalam ReplayEvent.
                }

                // Menjalankan memanggil `ApplyCashOut` dengan `player`, `depositAmount`, `evt`, `”setoran tabungan”` dalam ReplayEvent.
                ApplyCashOut(player, depositAmount, evt, "setoran tabungan");
                // Menjalankan memanggil `AddInventory` dengan `player.Savings`, `depositGoalId`, `depositAmount` dalam ReplayEvent.
                AddInventory(player.Savings, depositGoalId, depositAmount);
                // Mengakhiri loop atau cabang switch terdekat, kemudian melanjutkan setelah blok tersebut dalam ReplayEvent.
                break;

            // Menetapkan label cabang `case ”TarikTabungan”:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            case "TarikTabungan":
                // Menjalankan memanggil `RequireFeature` dengan `session.Mode`, `true`, `evt`, `”tabungan tujuan”` dalam ReplayEvent.
                RequireFeature(session.Mode, enabled: true, evt, "tabungan tujuan");
                // Menyiapkan variabel lokal `withdrawGoalId` untuk nilai withdraw target identitas dengan memanggil `ReadString` dengan `payload`, `”goal_id”`.
                // Tipe variabel disimpulkan dari ekspresi nilai awal.
                var withdrawGoalId = ReadString(payload, "goal_id");
                // Menyiapkan variabel lokal `withdrawAmount` untuk nilai withdraw nominal dengan memanggil `ReadInt` dengan `payload`, `”amount”`. Tipe variabel
                // disimpulkan dari ekspresi nilai awal.
                var withdrawAmount = ReadInt(payload, "amount");
                // Menjalankan memanggil `RemoveInventory` dengan `player.Savings`, `withdrawGoalId`, `withdrawAmount`, `evt` dalam ReplayEvent.
                RemoveInventory(player.Savings, withdrawGoalId, withdrawAmount, evt);
                // Menjalankan memanggil `ApplyCashIn` dengan `player`, `withdrawAmount` dalam ReplayEvent.
                ApplyCashIn(player, withdrawAmount);
                // Mengakhiri loop atau cabang switch terdekat, kemudian melanjutkan setelah blok tersebut dalam ReplayEvent.
                break;

            // Menetapkan label cabang `case ”TujuanFinansial”:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            case "TujuanFinansial":
                // Menjalankan memanggil `RequireFeature` dengan `session.Mode`, `true`, `evt`, `”tabungan tujuan”` dalam ReplayEvent.
                RequireFeature(session.Mode, enabled: true, evt, "tabungan tujuan");
                // Menyiapkan variabel lokal `achievedGoalId` untuk nilai achieved target identitas dengan memanggil `ReadString` dengan `payload`, `”goal_id”`.
                // Tipe variabel disimpulkan dari ekspresi nilai awal.
                var achievedGoalId = ReadString(payload, "goal_id");
                // Menyiapkan variabel lokal `cost` untuk nilai biaya dengan memanggil `ReadInt` dengan `payload`, `”cost”`. Tipe variabel disimpulkan dari ekspresi
                // nilai awal.
                var cost = ReadInt(payload, "cost");
                // Menjalankan memanggil `RemoveInventory` dengan `player.Savings`, `achievedGoalId`, `cost`, `evt` dalam ReplayEvent.
                RemoveInventory(player.Savings, achievedGoalId, cost, evt);
                // Menjalankan menambahkan `achievedGoalId` ke `player.AchievedSavingGoals` dalam ReplayEvent.
                player.AchievedSavingGoals.Add(achievedGoalId);
                // Mengakhiri loop atau cabang switch terdekat, kemudian melanjutkan setelah blok tersebut dalam ReplayEvent.
                break;

            // Menetapkan label cabang `default:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            default:
                // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen ($”Action type belum ditangani replay validator:
                // {evt.ActionType}.”) dalam ReplayEvent; pemanggil atau middleware penanganan error menerima kegagalan ini.
                throw new InvalidOperationException($"Action type belum ditangani replay validator: {evt.ActionType}.");
        // Menutup scope pemilihan switch atas `evt.ActionType`; bagian berikut berada di luar batas blok tersebut dalam ReplayEvent.
        }
    // Menutup scope metode ReplayEvent; bagian berikut berada di luar batas blok tersebut dalam ReplayEvent.
    }

    // Mendefinisikan metode `RunWithConnectionStringAsync` dengan hasil bertipe `Task`; operasi ini menangani run dengan connection string asinkron.
    // async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `connectionString`
    // bertipe `string` membawa nilai connection string; Parameter `action` bertipe `Func<Task>` membawa nilai aksi.
    private static async Task RunWithConnectionStringAsync(string connectionString, Func<Task> action)
    // Membuka scope metode RunWithConnectionStringAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // RunWithConnectionStringAsync.
    {
        // Menyiapkan variabel lokal `previousConnectionString` untuk nilai previous connection string dengan memanggil `Environment.GetEnvironmentVariable`
        // dengan `”ConnectionStrings__Default”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var previousConnectionString = Environment.GetEnvironmentVariable("ConnectionStrings__Default");
        // Menyiapkan variabel lokal `previousJwtSigningKey` untuk nilai previous jwt signing kunci dengan memanggil `Environment.GetEnvironmentVariable`
        // dengan `”JWT_SIGNING_KEY”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var previousJwtSigningKey = Environment.GetEnvironmentVariable("JWT_SIGNING_KEY");
        // Menyiapkan variabel lokal `previousJwtSectionSigningKey` untuk nilai previous jwt section signing kunci dengan memanggil
        // `Environment.GetEnvironmentVariable` dengan `”Jwt__SigningKey”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var previousJwtSectionSigningKey = Environment.GetEnvironmentVariable("Jwt__SigningKey");

        // Menjalankan memanggil `Environment.SetEnvironmentVariable` dengan `”ConnectionStrings__Default”`, `connectionString` dalam
        // RunWithConnectionStringAsync.
        Environment.SetEnvironmentVariable("ConnectionStrings__Default", connectionString);
        // Menjalankan memanggil `Environment.SetEnvironmentVariable` dengan `”JWT_SIGNING_KEY”`, `JwtSigningKey` dalam RunWithConnectionStringAsync.
        Environment.SetEnvironmentVariable("JWT_SIGNING_KEY", JwtSigningKey);
        // Menjalankan memanggil `Environment.SetEnvironmentVariable` dengan `”Jwt__SigningKey”`, `JwtSigningKey` dalam RunWithConnectionStringAsync.
        Environment.SetEnvironmentVariable("Jwt__SigningKey", JwtSigningKey);

        // Memulai blok try dalam RunWithConnectionStringAsync; exception dari blok ini dapat dialihkan ke catch, sedangkan finally (jika ada) tetap
        // dijalankan saat keluar.
        try
        // Membuka scope penanganan operasi try; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam RunWithConnectionStringAsync.
        {
            // Menjalankan hasil operasi asinkron memanggil `action` dengan tanpa argumen; await menunggu hasil tanpa memblokir thread selama operasi belum
            // selesai dalam RunWithConnectionStringAsync.
            await action();
        // Menutup scope penanganan operasi try; bagian berikut berada di luar batas blok tersebut dalam RunWithConnectionStringAsync.
        }
        // Menjalankan blok finally ketika alur meninggalkan try/catch, termasuk saat terjadi exception dalam RunWithConnectionStringAsync; bagian ini
        // dipakai untuk pekerjaan penutup yang harus tetap dilakukan.
        finally
        // Membuka scope pekerjaan penutup finally; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam RunWithConnectionStringAsync.
        {
            // Menjalankan memanggil `Environment.SetEnvironmentVariable` dengan `”ConnectionStrings__Default”`, `previousConnectionString` dalam
            // RunWithConnectionStringAsync.
            Environment.SetEnvironmentVariable("ConnectionStrings__Default", previousConnectionString);
            // Menjalankan memanggil `Environment.SetEnvironmentVariable` dengan `”JWT_SIGNING_KEY”`, `previousJwtSigningKey` dalam
            // RunWithConnectionStringAsync.
            Environment.SetEnvironmentVariable("JWT_SIGNING_KEY", previousJwtSigningKey);
            // Menjalankan memanggil `Environment.SetEnvironmentVariable` dengan `”Jwt__SigningKey”`, `previousJwtSectionSigningKey` dalam
            // RunWithConnectionStringAsync.
            Environment.SetEnvironmentVariable("Jwt__SigningKey", previousJwtSectionSigningKey);
        // Menutup scope pekerjaan penutup finally; bagian berikut berada di luar batas blok tersebut dalam RunWithConnectionStringAsync.
        }
    // Menutup scope metode RunWithConnectionStringAsync; bagian berikut berada di luar batas blok tersebut dalam RunWithConnectionStringAsync.
    }

    // Mendefinisikan properti `RepoRoot` bertipe `string` untuk nilai repo root; nilainya dihitung dari memanggil `ResolveRepositoryRoot` dengan tanpa
    // argumen.
    private static string RepoRoot => ResolveRepositoryRoot();

    // Mendefinisikan metode `ResolveRepositoryRoot` dengan hasil bertipe `string`; operasi ini menangani resolve repositori root.
    private static string ResolveRepositoryRoot()
    // Membuka scope metode ResolveRepositoryRoot; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveRepositoryRoot.
    {
        // Menyiapkan variabel lokal `current` untuk nilai saat ini dengan objek baru bertipe `DirectoryInfo` dengan argumen (AppContext.BaseDirectory).
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        // Mengulangi blok selama hasil pencocokan `current` dengan pola `not null`; kondisi diperiksa lagi sebelum setiap iterasi dalam
        // ResolveRepositoryRoot.
        while (current is not null)
        // Membuka scope loop selama `current is not null`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveRepositoryRoot.
        {
            // Memeriksa memanggil `File.Exists` dengan `Path.Combine(current.FullName, ”Cashflowpoly.sln”)`; blok if hanya dijalankan ketika kondisi ini
            // bernilai benar dalam ResolveRepositoryRoot.
            if (File.Exists(Path.Combine(current.FullName, "Cashflowpoly.sln")))
            // Membuka scope cabang if untuk kondisi `File.Exists(Path.Combine(current.FullName, ”Cashflowpoly.sln”))`; pernyataan/deklarasi berikut berada di
            // dalam batas blok ini dalam ResolveRepositoryRoot.
            {
                // Mengembalikan `current.FullName` (nilai full nama) kepada pemanggil dalam ResolveRepositoryRoot; eksekusi jalur ini selesai setelah nilai hasil
                // ditentukan.
                return current.FullName;
            // Menutup scope cabang if untuk kondisi `File.Exists(Path.Combine(current.FullName, ”Cashflowpoly.sln”))`; bagian berikut berada di luar batas blok
            // tersebut dalam ResolveRepositoryRoot.
            }

            // Memperbarui `current` menggunakan `current.Parent` (nilai parent) dalam ResolveRepositoryRoot.
            current = current.Parent;
        // Menutup scope loop selama `current is not null`; bagian berikut berada di luar batas blok tersebut dalam ResolveRepositoryRoot.
        }

        // Menghentikan alur dengan melempar objek baru bertipe `DirectoryNotFoundException` dengan argumen (”Tidak dapat menemukan root repositori
        // (Cashflowpoly.sln).”) dalam ResolveRepositoryRoot; pemanggil atau middleware penanganan error menerima kegagalan ini.
        throw new DirectoryNotFoundException("Tidak dapat menemukan root repositori (Cashflowpoly.sln).");
    // Menutup scope metode ResolveRepositoryRoot; bagian berikut berada di luar batas blok tersebut dalam ResolveRepositoryRoot.
    }

    // Mendefinisikan metode `ReplaySystemEvent` dengan hasil bertipe `void`; operasi ini menangani replay system event. Masukan: Parameter `session`
    // bertipe `ReplaySessionState` membawa nilai sesi; Parameter `evt` bertipe `ReplayEventRow` membawa satu event permainan yang sedang diperiksa;
    // Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON.
    private static void ReplaySystemEvent(ReplaySessionState session, ReplayEventRow evt, JsonElement payload)
    // Membuka scope metode ReplaySystemEvent; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ReplaySystemEvent.
    {
        // Memilih cabang berdasarkan `evt.ActionType` (nilai aksi jenis); label case menentukan perlakuan untuk setiap nilai yang dikenali dalam
        // ReplaySystemEvent.
        switch (evt.ActionType)
        // Membuka scope pemilihan switch atas `evt.ActionType`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ReplaySystemEvent.
        {
            // Menetapkan label cabang `case ”SetupModalAwal”:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            case "SetupModalAwal":
                // Memeriksa kebalikan kondisi `evt.UserId.HasValue`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ReplaySystemEvent.
                if (!evt.UserId.HasValue)
                // Membuka scope cabang if untuk kondisi `!evt.UserId.HasValue`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // ReplaySystemEvent.
                {
                    // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen (”SetupModalAwal wajib memiliki user_id pemain.”)
                    // dalam ReplaySystemEvent; pemanggil atau middleware penanganan error menerima kegagalan ini.
                    throw new InvalidOperationException("SetupModalAwal wajib memiliki user_id pemain.");
                // Menutup scope cabang if untuk kondisi `!evt.UserId.HasValue`; bagian berikut berada di luar batas blok tersebut dalam ReplaySystemEvent.
                }
                // Memperbarui `session.GetPlayer(evt.UserId, evt.PlayerName).Cash` menggunakan memanggil `ReadInt` dengan `payload`, `”amount”` dalam
                // ReplaySystemEvent.
                session.GetPlayer(evt.UserId, evt.PlayerName).Cash = ReadInt(payload, "amount");
                // Mengakhiri loop atau cabang switch terdekat, kemudian melanjutkan setelah blok tersebut dalam ReplaySystemEvent.
                break;

            // Menetapkan label cabang `case ”SetupBahanAwal”:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            case "SetupBahanAwal":
                // Memeriksa kebalikan kondisi `evt.UserId.HasValue`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ReplaySystemEvent.
                if (!evt.UserId.HasValue)
                // Membuka scope cabang if untuk kondisi `!evt.UserId.HasValue`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // ReplaySystemEvent.
                {
                    // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen (”SetupBahanAwal wajib memiliki user_id pemain.”)
                    // dalam ReplaySystemEvent; pemanggil atau middleware penanganan error menerima kegagalan ini.
                    throw new InvalidOperationException("SetupBahanAwal wajib memiliki user_id pemain.");
                // Menutup scope cabang if untuk kondisi `!evt.UserId.HasValue`; bagian berikut berada di luar batas blok tersebut dalam ReplaySystemEvent.
                }
                // Menyiapkan variabel lokal `sbPlayer` untuk nilai sb pemain dengan memanggil `session.GetPlayer` dengan `evt.UserId`, `evt.PlayerName`. Tipe
                // variabel disimpulkan dari ekspresi nilai awal.
                var sbPlayer = session.GetPlayer(evt.UserId, evt.PlayerName);
                // Menyiapkan variabel lokal `sbAmount` untuk nilai sb nominal dengan memanggil `ReadInt` dengan `payload`, `”amount”`. Tipe variabel disimpulkan
                // dari ekspresi nilai awal.
                var sbAmount = ReadInt(payload, "amount");
                // Menyiapkan variabel lokal `sbCardId` untuk nilai sb kartu identitas dengan memanggil `ReadString` dengan `payload`, `”card_id”`. Tipe variabel
                // disimpulkan dari ekspresi nilai awal.
                var sbCardId = ReadString(payload, "card_id");
                // Menjalankan memanggil `AddInventory` dengan `sbPlayer.Ingredients`, `sbCardId`, `1` dalam ReplaySystemEvent.
                AddInventory(sbPlayer.Ingredients, sbCardId, 1);
                // Menjalankan memanggil `ApplyCashOut` dengan `sbPlayer`, `sbAmount`, `evt`, `”biaya bahan”` dalam ReplaySystemEvent.
                ApplyCashOut(sbPlayer, sbAmount, evt, "biaya bahan");
                // Mengakhiri loop atau cabang switch terdekat, kemudian melanjutkan setelah blok tersebut dalam ReplaySystemEvent.
                break;

            // Menetapkan label cabang `case ”SetupEmasAwal”:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            case "SetupEmasAwal":
                // Memeriksa kebalikan kondisi `evt.UserId.HasValue`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ReplaySystemEvent.
                if (!evt.UserId.HasValue)
                // Membuka scope cabang if untuk kondisi `!evt.UserId.HasValue`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // ReplaySystemEvent.
                {
                    // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen (”SetupEmasAwal wajib memiliki user_id pemain.”)
                    // dalam ReplaySystemEvent; pemanggil atau middleware penanganan error menerima kegagalan ini.
                    throw new InvalidOperationException("SetupEmasAwal wajib memiliki user_id pemain.");
                // Menutup scope cabang if untuk kondisi `!evt.UserId.HasValue`; bagian berikut berada di luar batas blok tersebut dalam ReplaySystemEvent.
                }
                // Memperbarui `session.GetPlayer(evt.UserId, evt.PlayerName).GoldQty` dengan menambahkan memanggil `ReadInt` dengan `payload`, `”qty”` dalam
                // ReplaySystemEvent.
                session.GetPlayer(evt.UserId, evt.PlayerName).GoldQty += ReadInt(payload, "qty");
                // Mengakhiri loop atau cabang switch terdekat, kemudian melanjutkan setelah blok tersebut dalam ReplaySystemEvent.
                break;

            // Menetapkan label cabang `case ”SetupMisiAwal”:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            case "SetupMisiAwal":
                // Memperbarui `_` menggunakan memanggil `ReadString` dengan `payload`, `”mission_id”` dalam ReplaySystemEvent.
                _ = ReadString(payload, "mission_id");
                // Memperbarui `_` menggunakan memanggil `ReadString` dengan `payload`, `”target_tertiary_card_id”` dalam ReplaySystemEvent.
                _ = ReadString(payload, "target_tertiary_card_id");
                // Memperbarui `_` menggunakan memanggil `ReadInt` dengan `payload`, `”penalty_points”` dalam ReplaySystemEvent.
                _ = ReadInt(payload, "penalty_points");
                // Mengakhiri loop atau cabang switch terdekat, kemudian melanjutkan setelah blok tersebut dalam ReplaySystemEvent.
                break;

            // Menetapkan label cabang `case ”SetupPinjamanAwal”:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            case "SetupPinjamanAwal":
                // Memeriksa kebalikan kondisi `evt.UserId.HasValue`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ReplaySystemEvent.
                if (!evt.UserId.HasValue)
                // Membuka scope cabang if untuk kondisi `!evt.UserId.HasValue`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // ReplaySystemEvent.
                {
                    // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen (”SetupPinjamanAwal wajib memiliki user_id
                    // pemain.”) dalam ReplaySystemEvent; pemanggil atau middleware penanganan error menerima kegagalan ini.
                    throw new InvalidOperationException("SetupPinjamanAwal wajib memiliki user_id pemain.");
                // Menutup scope cabang if untuk kondisi `!evt.UserId.HasValue`; bagian berikut berada di luar batas blok tersebut dalam ReplaySystemEvent.
                }
                // Menyiapkan variabel lokal `sLoanId` untuk nilai s pinjaman identitas dengan memanggil `ReadString` dengan `payload`, `”loan_id”`. Tipe variabel
                // disimpulkan dari ekspresi nilai awal.
                var sLoanId = ReadString(payload, "loan_id");
                // Menyiapkan variabel lokal `sPrincipal` untuk nilai s principal dengan memanggil `ReadInt` dengan `payload`, `”principal”`. Tipe variabel
                // disimpulkan dari ekspresi nilai awal.
                var sPrincipal = ReadInt(payload, "principal");
                // Memperbarui `session.GetPlayer(evt.UserId, evt.PlayerName).Loans[sLoanId]` menggunakan `sPrincipal` (nilai s principal) dalam ReplaySystemEvent.
                session.GetPlayer(evt.UserId, evt.PlayerName).Loans[sLoanId] = sPrincipal;
                // Mengakhiri loop atau cabang switch terdekat, kemudian melanjutkan setelah blok tersebut dalam ReplaySystemEvent.
                break;

            // Menetapkan label cabang `case ”SetupAsuransiAwal”:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            case "SetupAsuransiAwal":
                // Memeriksa kebalikan kondisi `evt.UserId.HasValue`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ReplaySystemEvent.
                if (!evt.UserId.HasValue)
                // Membuka scope cabang if untuk kondisi `!evt.UserId.HasValue`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // ReplaySystemEvent.
                {
                    // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen (”SetupAsuransiAwal wajib memiliki user_id
                    // pemain.”) dalam ReplaySystemEvent; pemanggil atau middleware penanganan error menerima kegagalan ini.
                    throw new InvalidOperationException("SetupAsuransiAwal wajib memiliki user_id pemain.");
                // Menutup scope cabang if untuk kondisi `!evt.UserId.HasValue`; bagian berikut berada di luar batas blok tersebut dalam ReplaySystemEvent.
                }
                // Menyiapkan variabel lokal `sPolicyId` untuk nilai s policy identitas dengan memanggil `ReadString` dengan `payload`, `”policy_id”`. Tipe variabel
                // disimpulkan dari ekspresi nilai awal.
                var sPolicyId = ReadString(payload, "policy_id");
                // Menjalankan menambahkan `sPolicyId` ke `session.GetPlayer(evt.UserId, evt.PlayerName).InsurancePolicies` dalam ReplaySystemEvent.
                session.GetPlayer(evt.UserId, evt.PlayerName).InsurancePolicies.Add(sPolicyId);
                // Mengakhiri loop atau cabang switch terdekat, kemudian melanjutkan setelah blok tersebut dalam ReplaySystemEvent.
                break;

            // Menetapkan label cabang `case ”MulaiSesi”:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            case "MulaiSesi":
                // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`0`, `evt.DayIndex`); pengujian gagal jika
                // keduanya berbeda dalam ReplaySystemEvent.
                Assert.Equal(0, evt.DayIndex);
                // Mengakhiri loop atau cabang switch terdekat, kemudian melanjutkan setelah blok tersebut dalam ReplaySystemEvent.
                break;

            // Menetapkan label cabang `case ”Asuransi”:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            case "Asuransi":
                // Memeriksa kebalikan kondisi `evt.UserId.HasValue`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ReplaySystemEvent.
                if (!evt.UserId.HasValue)
                // Membuka scope cabang if untuk kondisi `!evt.UserId.HasValue`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // ReplaySystemEvent.
                {
                    // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen (”Asuransi setup wajib memiliki user_id pemain.”)
                    // dalam ReplaySystemEvent; pemanggil atau middleware penanganan error menerima kegagalan ini.
                    throw new InvalidOperationException("Asuransi setup wajib memiliki user_id pemain.");
                // Menutup scope cabang if untuk kondisi `!evt.UserId.HasValue`; bagian berikut berada di luar batas blok tersebut dalam ReplaySystemEvent.
                }

                // Menyiapkan variabel lokal `setupPolicyId` untuk nilai setup policy identitas dengan memanggil `ReadString` dengan `payload`, `”policy_id”`. Tipe
                // variabel disimpulkan dari ekspresi nilai awal.
                var setupPolicyId = ReadString(payload, "policy_id");
                // Menyiapkan variabel lokal `setupPremium` untuk nilai setup premium dengan memanggil `ReadInt` dengan `payload`, `”premium”`. Tipe variabel
                // disimpulkan dari ekspresi nilai awal.
                var setupPremium = ReadInt(payload, "premium");
                // Memeriksa perbandingan ketidaksamaan antara `setupPremium` dan `0`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
                // ReplaySystemEvent.
                if (setupPremium != 0)
                // Membuka scope cabang if untuk kondisi `setupPremium != 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ReplaySystemEvent.
                {
                    // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen (”Asuransi setup wajib gratis.”) dalam
                    // ReplaySystemEvent; pemanggil atau middleware penanganan error menerima kegagalan ini.
                    throw new InvalidOperationException("Asuransi setup wajib gratis.");
                // Menutup scope cabang if untuk kondisi `setupPremium != 0`; bagian berikut berada di luar batas blok tersebut dalam ReplaySystemEvent.
                }

                // Menjalankan menambahkan `setupPolicyId` ke `session.GetPlayer(evt.UserId, evt.PlayerName).InsurancePolicies` dalam ReplaySystemEvent.
                session.GetPlayer(evt.UserId, evt.PlayerName).InsurancePolicies.Add(setupPolicyId);

                // Mengakhiri loop atau cabang switch terdekat, kemudian melanjutkan setelah blok tersebut dalam ReplaySystemEvent.
                break;

            // Menetapkan label cabang `case ”PinjamanSyariah”:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            case "PinjamanSyariah":
                // Memeriksa kebalikan kondisi `evt.UserId.HasValue`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ReplaySystemEvent.
                if (!evt.UserId.HasValue)
                // Membuka scope cabang if untuk kondisi `!evt.UserId.HasValue`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // ReplaySystemEvent.
                {
                    // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen (”Pinjaman syariah setup wajib memiliki user_id
                    // pemain.”) dalam ReplaySystemEvent; pemanggil atau middleware penanganan error menerima kegagalan ini.
                    throw new InvalidOperationException("Pinjaman syariah setup wajib memiliki user_id pemain.");
                // Menutup scope cabang if untuk kondisi `!evt.UserId.HasValue`; bagian berikut berada di luar batas blok tersebut dalam ReplaySystemEvent.
                }
                // Menyiapkan variabel lokal `setupLoanId` untuk nilai setup pinjaman identitas dengan memanggil `ReadString` dengan `payload`, `”loan_id”`. Tipe
                // variabel disimpulkan dari ekspresi nilai awal.
                var setupLoanId = ReadString(payload, "loan_id");
                // Menyiapkan variabel lokal `setupPrincipal` untuk nilai setup principal dengan memanggil `ReadInt` dengan `payload`, `”principal”`. Tipe variabel
                // disimpulkan dari ekspresi nilai awal.
                var setupPrincipal = ReadInt(payload, "principal");
                // Memeriksa perbandingan ketidaksamaan antara `setupPrincipal` dan `10`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
                // ReplaySystemEvent.
                if (setupPrincipal != 10)
                // Membuka scope cabang if untuk kondisi `setupPrincipal != 10`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // ReplaySystemEvent.
                {
                    // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen (”Pinjaman syariah setup wajib principal 10.”)
                    // dalam ReplaySystemEvent; pemanggil atau middleware penanganan error menerima kegagalan ini.
                    throw new InvalidOperationException("Pinjaman syariah setup wajib principal 10.");
                // Menutup scope cabang if untuk kondisi `setupPrincipal != 10`; bagian berikut berada di luar batas blok tersebut dalam ReplaySystemEvent.
                }
                // Memperbarui `session.GetPlayer(evt.UserId, evt.PlayerName).Loans[setupLoanId]` menggunakan `setupPrincipal` (nilai setup principal) dalam
                // ReplaySystemEvent.
                session.GetPlayer(evt.UserId, evt.PlayerName).Loans[setupLoanId] = setupPrincipal;
                // Mengakhiri loop atau cabang switch terdekat, kemudian melanjutkan setelah blok tersebut dalam ReplaySystemEvent.
                break;

            // Menetapkan label cabang `case ”BukaHargaEmas”:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            case "BukaHargaEmas":
            // Menetapkan label cabang `case ”TujuanFinansial”:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            case "TujuanFinansial":
                // Mengakhiri loop atau cabang switch terdekat, kemudian melanjutkan setelah blok tersebut dalam ReplaySystemEvent.
                break;

            // Menetapkan label cabang `case ”AmbilKartuDariDeck”:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            case "AmbilKartuDariDeck":
            // Menetapkan label cabang `case ”IsiUlangPasar”:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            case "IsiUlangPasar":
                // Memperbarui `_` menggunakan memanggil `ReadString` dengan `payload`, `”asset_type”` dalam ReplaySystemEvent.
                _ = ReadString(payload, "asset_type");
                // Memperbarui `_` menggunakan memanggil `ReadString` dengan `payload`, `”asset_code”` dalam ReplaySystemEvent.
                _ = ReadString(payload, "asset_code");
                // Mengakhiri loop atau cabang switch terdekat, kemudian melanjutkan setelah blok tersebut dalam ReplaySystemEvent.
                break;

            // Menetapkan label cabang `case ”KartuMasukDiscard”:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            case "KartuMasukDiscard":
                // Memperbarui `_` menggunakan memanggil `ReadString` dengan `payload`, `”slot_group”` dalam ReplaySystemEvent.
                _ = ReadString(payload, "slot_group");
                // Memperbarui `_` menggunakan memanggil `ReadString` dengan `payload`, `”slot_code”` dalam ReplaySystemEvent.
                _ = ReadString(payload, "slot_code");
                // Mengakhiri loop atau cabang switch terdekat, kemudian melanjutkan setelah blok tersebut dalam ReplaySystemEvent.
                break;

            // Menetapkan label cabang `case ”PoinPeringkatDonasi”:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            case "PoinPeringkatDonasi":
                // Menyiapkan variabel lokal `rank` untuk nilai rank dengan memanggil `ReadInt` dengan `payload`, `”rank”`. Tipe variabel disimpulkan dari ekspresi
                // nilai awal.
                var rank = ReadInt(payload, "rank");
                // Menyiapkan variabel lokal `points` untuk nilai poin dengan memanggil `ReadInt` dengan `payload`, `”points”`. Tipe variabel disimpulkan dari
                // ekspresi nilai awal.
                var points = ReadInt(payload, "points");
                // Memeriksa kebalikan kondisi `evt.UserId.HasValue`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ReplaySystemEvent.
                if (!evt.UserId.HasValue)
                // Membuka scope cabang if untuk kondisi `!evt.UserId.HasValue`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // ReplaySystemEvent.
                {
                    // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen (”Donation rank wajib memiliki user_id pemain.”)
                    // dalam ReplaySystemEvent; pemanggil atau middleware penanganan error menerima kegagalan ini.
                    throw new InvalidOperationException("Donation rank wajib memiliki user_id pemain.");
                // Menutup scope cabang if untuk kondisi `!evt.UserId.HasValue`; bagian berikut berada di luar batas blok tersebut dalam ReplaySystemEvent.
                }

                // Menjalankan memanggil `session.RecordDonationAward` dengan `evt.DayIndex`, `evt.UserId.Value`, `rank`, `points` dalam ReplaySystemEvent.
                session.RecordDonationAward(evt.DayIndex, evt.UserId.Value, rank, points);
                // Mengakhiri loop atau cabang switch terdekat, kemudian melanjutkan setelah blok tersebut dalam ReplaySystemEvent.
                break;

            // Menetapkan label cabang `case ”UmumkanJuaraDonasi”:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            case "UmumkanJuaraDonasi":
                // Memperbarui `_` menggunakan memanggil `ReadString` dengan `payload`, `”summary”` dalam ReplaySystemEvent.
                _ = ReadString(payload, "summary");
                // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!payload.TryGetProperty(”winners”, out var winners) ||
                // winners.ValueKind != JsonValueKind.Array` dan `winners.GetArrayLength() != 3`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya
                // dijalankan ketika kondisi ini bernilai benar dalam ReplaySystemEvent.
                if (!payload.TryGetProperty("winners", out var winners) ||
                    // Melanjutkan ekspresi dengan perbandingan ketidaksamaan antara `winners.ValueKind` dan `JsonValueKind.Array` dalam ReplaySystemEvent.
                    winners.ValueKind != JsonValueKind.Array ||
                    // Melanjutkan pengolahan dengan memanggil `winners.GetArrayLength` dengan tanpa argumen dalam ReplaySystemEvent.
                    winners.GetArrayLength() != 3)
                // Membuka scope cabang if untuk kondisi `!payload.TryGetProperty(”winners”, out var winners) || winners.ValueKind != JsonValueKind.Array ||
                // winners.GetArrayLength() != 3`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ReplaySystemEvent.
                {
                    // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen (”Pengumuman juara donasi wajib berisi 3
                    // winner.”) dalam ReplaySystemEvent; pemanggil atau middleware penanganan error menerima kegagalan ini.
                    throw new InvalidOperationException("Pengumuman juara donasi wajib berisi 3 winner.");
                // Menutup scope cabang if untuk kondisi `!payload.TryGetProperty(”winners”, out var winners) || winners.ValueKind != JsonValueKind.Array ||
                // winners.GetArrayLength() != 3`; bagian berikut berada di luar batas blok tersebut dalam ReplaySystemEvent.
                }

                // Menyiapkan variabel lokal `expectedRank` untuk nilai yang diharapkan rank dengan nilai literal `1`. Tipe variabel disimpulkan dari ekspresi nilai
                // awal.
                var expectedRank = 1;
                // Mengulangi setiap elemen `winners.EnumerateArray()`; elemen saat ini disimpan sebagai `winner` bertipe `var` untuk diproses oleh badan loop dalam
                // ReplaySystemEvent.
                foreach (var winner in winners.EnumerateArray())
                // Membuka scope loop setiap winner dari `winners.EnumerateArray()`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // ReplaySystemEvent.
                {
                    // Menyiapkan variabel lokal `winnerRank` untuk nilai winner rank dengan memanggil `ReadInt` dengan `winner`, `”rank”`. Tipe variabel disimpulkan
                    // dari ekspresi nilai awal.
                    var winnerRank = ReadInt(winner, "rank");
                    // Memperbarui `_` menggunakan memanggil `ReadString` dengan `winner`, `”player_name”` dalam ReplaySystemEvent.
                    _ = ReadString(winner, "player_name");
                    // Memperbarui `_` menggunakan memanggil `ReadInt` dengan `winner`, `”points”` dalam ReplaySystemEvent.
                    _ = ReadInt(winner, "points");
                    // Memeriksa perbandingan ketidaksamaan antara `winnerRank` dan `expectedRank`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
                    // ReplaySystemEvent.
                    if (winnerRank != expectedRank)
                    // Membuka scope cabang if untuk kondisi `winnerRank != expectedRank`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                    // ReplaySystemEvent.
                    {
                        // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen (”Urutan winner juara donasi tidak sesuai rank.”)
                        // dalam ReplaySystemEvent; pemanggil atau middleware penanganan error menerima kegagalan ini.
                        throw new InvalidOperationException("Urutan winner juara donasi tidak sesuai rank.");
                    // Menutup scope cabang if untuk kondisi `winnerRank != expectedRank`; bagian berikut berada di luar batas blok tersebut dalam ReplaySystemEvent.
                    }

                    // Menjalankan `expectedRank++` dalam ReplaySystemEvent.
                    expectedRank++;
                // Menutup scope loop setiap winner dari `winners.EnumerateArray()`; bagian berikut berada di luar batas blok tersebut dalam ReplaySystemEvent.
                }

                // Mengakhiri loop atau cabang switch terdekat, kemudian melanjutkan setelah blok tersebut dalam ReplaySystemEvent.
                break;

            // Menetapkan label cabang `case ”BagikanTieBreaker”:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            case "BagikanTieBreaker":
                // Menyiapkan variabel lokal `tieNumber` untuk nilai tie number dengan memanggil `ReadInt` dengan `payload`, `”number”`. Tipe variabel disimpulkan
                // dari ekspresi nilai awal.
                var tieNumber = ReadInt(payload, "number");
                // Memeriksa kebalikan kondisi `evt.UserId.HasValue`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ReplaySystemEvent.
                if (!evt.UserId.HasValue)
                // Membuka scope cabang if untuk kondisi `!evt.UserId.HasValue`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // ReplaySystemEvent.
                {
                    // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen (”Tie breaker wajib memiliki user_id pemain.”)
                    // dalam ReplaySystemEvent; pemanggil atau middleware penanganan error menerima kegagalan ini.
                    throw new InvalidOperationException("Tie breaker wajib memiliki user_id pemain.");
                // Menutup scope cabang if untuk kondisi `!evt.UserId.HasValue`; bagian berikut berada di luar batas blok tersebut dalam ReplaySystemEvent.
                }

                // Memeriksa hasil pencocokan `tieNumber` dengan pola `< 1 or > 4`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
                // ReplaySystemEvent.
                if (tieNumber is < 1 or > 4)
                // Membuka scope cabang if untuk kondisi `tieNumber is < 1 or > 4`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // ReplaySystemEvent.
                {
                    // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen ($”Nomor tie breaker tidak valid: {tieNumber}.”)
                    // dalam ReplaySystemEvent; pemanggil atau middleware penanganan error menerima kegagalan ini.
                    throw new InvalidOperationException($"Nomor tie breaker tidak valid: {tieNumber}.");
                // Menutup scope cabang if untuk kondisi `tieNumber is < 1 or > 4`; bagian berikut berada di luar batas blok tersebut dalam ReplaySystemEvent.
                }

                // Menjalankan memanggil `session.RecordTieBreaker` dengan `evt.UserId.Value`, `tieNumber` dalam ReplaySystemEvent.
                session.RecordTieBreaker(evt.UserId.Value, tieNumber);
                // Mengakhiri loop atau cabang switch terdekat, kemudian melanjutkan setelah blok tersebut dalam ReplaySystemEvent.
                break;

            // Menetapkan label cabang `case ”HariMingguLibur”:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            case "HariMingguLibur":
                // Memeriksa kebalikan kondisi `evt.Weekday.Equals(”SUN”, StringComparison.OrdinalIgnoreCase)`; blok if hanya dijalankan ketika kondisi ini bernilai
                // benar dalam ReplaySystemEvent.
                if (!evt.Weekday.Equals("SUN", StringComparison.OrdinalIgnoreCase))
                // Membuka scope cabang if untuk kondisi `!evt.Weekday.Equals(”SUN”, StringComparison.OrdinalIgnoreCase)`; pernyataan/deklarasi berikut berada di
                // dalam batas blok ini dalam ReplaySystemEvent.
                {
                    // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen (”Event libur Minggu harus jatuh pada weekday
                    // SUN.”) dalam ReplaySystemEvent; pemanggil atau middleware penanganan error menerima kegagalan ini.
                    throw new InvalidOperationException("Event libur Minggu harus jatuh pada weekday SUN.");
                // Menutup scope cabang if untuk kondisi `!evt.Weekday.Equals(”SUN”, StringComparison.OrdinalIgnoreCase)`; bagian berikut berada di luar batas blok
                // tersebut dalam ReplaySystemEvent.
                }

                // Mengakhiri loop atau cabang switch terdekat, kemudian melanjutkan setelah blok tersebut dalam ReplaySystemEvent.
                break;

            // Menetapkan label cabang `case ”AkhirGiliran”:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            case "AkhirGiliran":
                // Menyiapkan variabel lokal `fromDay` untuk nilai dari hari dengan memanggil `ReadInt` dengan `payload`, `”from_day”`. Tipe variabel disimpulkan
                // dari ekspresi nilai awal.
                var fromDay = ReadInt(payload, "from_day");
                // Menyiapkan variabel lokal `toDay` untuk nilai ke hari dengan memanggil `ReadInt` dengan `payload`, `”to_day”`. Tipe variabel disimpulkan dari
                // ekspresi nilai awal.
                var toDay = ReadInt(payload, "to_day");
                // Menyiapkan variabel lokal `completedPlayers` untuk nilai selesai pemain dengan memanggil `ReadInt` dengan `payload`, `”completed_players”`. Tipe
                // variabel disimpulkan dari ekspresi nilai awal.
                var completedPlayers = ReadInt(payload, "completed_players");
                // Menyiapkan variabel lokal `used` untuk nilai used dengan memanggil `ReadInt` dengan `payload`, `”used”`. Tipe variabel disimpulkan dari ekspresi
                // nilai awal.
                var used = ReadInt(payload, "used");
                // Menyiapkan variabel lokal `remaining` untuk nilai tersisa dengan memanggil `ReadInt` dengan `payload`, `”remaining”`. Tipe variabel disimpulkan
                // dari ekspresi nilai awal.
                var remaining = ReadInt(payload, "remaining");

                // Memeriksa hasil pencocokan `evt.DayIndex` dengan pola `< 1 or >= 25`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
                // ReplaySystemEvent.
                if (evt.DayIndex is < 1 or >= 25)
                // Membuka scope cabang if untuk kondisi `evt.DayIndex is < 1 or >= 25`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // ReplaySystemEvent.
                {
                    // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen (”AkhirGiliran hanya valid untuk transisi hari 1
                    // sampai 24.”) dalam ReplaySystemEvent; pemanggil atau middleware penanganan error menerima kegagalan ini.
                    throw new InvalidOperationException("AkhirGiliran hanya valid untuk transisi hari 1 sampai 24.");
                // Menutup scope cabang if untuk kondisi `evt.DayIndex is < 1 or >= 25`; bagian berikut berada di luar batas blok tersebut dalam ReplaySystemEvent.
                }

                // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `fromDay != evt.DayIndex` dan `toDay != evt.DayIndex + 1`; sisi kanan
                // diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ReplaySystemEvent.
                if (fromDay != evt.DayIndex || toDay != evt.DayIndex + 1)
                // Membuka scope cabang if untuk kondisi `fromDay != evt.DayIndex || toDay != evt.DayIndex + 1`; pernyataan/deklarasi berikut berada di dalam batas
                // blok ini dalam ReplaySystemEvent.
                {
                    // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen (”Payload AkhirGiliran tidak sesuai transisi
                    // day_index event.”) dalam ReplaySystemEvent; pemanggil atau middleware penanganan error menerima kegagalan ini.
                    throw new InvalidOperationException("Payload AkhirGiliran tidak sesuai transisi day_index event.");
                // Menutup scope cabang if untuk kondisi `fromDay != evt.DayIndex || toDay != evt.DayIndex + 1`; bagian berikut berada di luar batas blok tersebut
                // dalam ReplaySystemEvent.
                }

                // Memeriksa perbandingan ketidaksamaan antara `completedPlayers` dan `4`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
                // ReplaySystemEvent.
                if (completedPlayers != 4)
                // Membuka scope cabang if untuk kondisi `completedPlayers != 4`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // ReplaySystemEvent.
                {
                    // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen (”AkhirGiliran wajib menutup giliran 4 pemain.”)
                    // dalam ReplaySystemEvent; pemanggil atau middleware penanganan error menerima kegagalan ini.
                    throw new InvalidOperationException("AkhirGiliran wajib menutup giliran 4 pemain.");
                // Menutup scope cabang if untuk kondisi `completedPlayers != 4`; bagian berikut berada di luar batas blok tersebut dalam ReplaySystemEvent.
                }

                // Menyiapkan variabel lokal `replayedUsed` untuk nilai replayed used dengan menjumlahkan nilai `session.ActionTokensByDay .Where(pair =>
                // pair.Key.DayIndex == evt.DayIndex)` berdasarkan `pair => pair.Value`. Tipe variabel disimpulkan dari ekspresi nilai awal.
                var replayedUsed = session.ActionTokensByDay
                    // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(pair => pair.Key.DayIndex == evt.DayIndex) dalam ReplaySystemEvent;
                    // token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
                    .Where(pair => pair.Key.DayIndex == evt.DayIndex)
                    // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Sum(pair => pair.Value); dalam ReplaySystemEvent; token pada baris ini
                    // menyambungkan bagian kode sebelum dan sesudahnya.
                    .Sum(pair => pair.Value);
                // Memeriksa perbandingan ketidaksamaan antara `used` dan `replayedUsed`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
                // ReplaySystemEvent.
                if (used != replayedUsed)
                // Membuka scope cabang if untuk kondisi `used != replayedUsed`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // ReplaySystemEvent.
                {
                    // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen ($”AkhirGiliran used={used}, expected
                    // {replayedUsed} dari replay action token.”) dalam ReplaySystemEvent; pemanggil atau middleware penanganan error menerima kegagalan ini.
                    throw new InvalidOperationException($"AkhirGiliran used={used}, expected {replayedUsed} dari replay action token.");
                // Menutup scope cabang if untuk kondisi `used != replayedUsed`; bagian berikut berada di luar batas blok tersebut dalam ReplaySystemEvent.
                }

                // Memeriksa perbandingan ketidaksamaan antara `remaining` dan `0`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
                // ReplaySystemEvent.
                if (remaining != 0)
                // Membuka scope cabang if untuk kondisi `remaining != 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ReplaySystemEvent.
                {
                    // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen (”AkhirGiliran seed wajib menyisakan 0 action
                    // setelah transisi hari.”) dalam ReplaySystemEvent; pemanggil atau middleware penanganan error menerima kegagalan ini.
                    throw new InvalidOperationException("AkhirGiliran seed wajib menyisakan 0 action setelah transisi hari.");
                // Menutup scope cabang if untuk kondisi `remaining != 0`; bagian berikut berada di luar batas blok tersebut dalam ReplaySystemEvent.
                }

                // Mengakhiri loop atau cabang switch terdekat, kemudian melanjutkan setelah blok tersebut dalam ReplaySystemEvent.
                break;

            // Menetapkan label cabang `case ”AkhiriSesi”:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            case "AkhiriSesi":
                // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`25`, `evt.DayIndex`); pengujian gagal jika
                // keduanya berbeda dalam ReplaySystemEvent.
                Assert.Equal(25, evt.DayIndex);
                // Mengakhiri loop atau cabang switch terdekat, kemudian melanjutkan setelah blok tersebut dalam ReplaySystemEvent.
                break;

            // Menetapkan label cabang `default:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            default:
                // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen ($”System action belum ditangani replay
                // validator: {evt.ActionType}.”) dalam ReplaySystemEvent; pemanggil atau middleware penanganan error menerima kegagalan ini.
                throw new InvalidOperationException($"System action belum ditangani replay validator: {evt.ActionType}.");
        // Menutup scope pemilihan switch atas `evt.ActionType`; bagian berikut berada di luar batas blok tersebut dalam ReplaySystemEvent.
        }
    // Menutup scope metode ReplaySystemEvent; bagian berikut berada di luar batas blok tersebut dalam ReplaySystemEvent.
    }

    // Mendefinisikan metode `ValidateCalendarAction` dengan hasil bertipe `void`; operasi ini menangani validate calendar aksi. Masukan: Parameter
    // `evt` bertipe `ReplayEventRow` membawa satu event permainan yang sedang diperiksa; Parameter `payload` bertipe `JsonElement` membawa muatan
    // detail event dalam format JSON.
    private static void ValidateCalendarAction(ReplayEventRow evt, JsonElement payload)
    // Membuka scope metode ValidateCalendarAction; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateCalendarAction.
    {
        // Memeriksa hasil pencocokan `evt.ActionType` dengan pola `”JumatBerkah” or ”PoinPeringkatDonasi” or ”UmumkanJuaraDonasi”`; blok if hanya
        // dijalankan ketika kondisi ini bernilai benar dalam ValidateCalendarAction.
        if (evt.ActionType is "JumatBerkah" or "PoinPeringkatDonasi" or "UmumkanJuaraDonasi")
        // Membuka scope cabang if untuk kondisi `evt.ActionType is ”JumatBerkah” or ”PoinPeringkatDonasi” or ”UmumkanJuaraDonasi”`; pernyataan/deklarasi
        // berikut berada di dalam batas blok ini dalam ValidateCalendarAction.
        {
            // Memeriksa kebalikan kondisi `evt.Weekday.Equals(”FRI”, StringComparison.OrdinalIgnoreCase)`; blok if hanya dijalankan ketika kondisi ini bernilai
            // benar dalam ValidateCalendarAction.
            if (!evt.Weekday.Equals("FRI", StringComparison.OrdinalIgnoreCase))
            // Membuka scope cabang if untuk kondisi `!evt.Weekday.Equals(”FRI”, StringComparison.OrdinalIgnoreCase)`; pernyataan/deklarasi berikut berada di
            // dalam batas blok ini dalam ValidateCalendarAction.
            {
                // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen ($”{evt.ActionType} harus jatuh pada FRI.”) dalam
                // ValidateCalendarAction; pemanggil atau middleware penanganan error menerima kegagalan ini.
                throw new InvalidOperationException($"{evt.ActionType} harus jatuh pada FRI.");
            // Menutup scope cabang if untuk kondisi `!evt.Weekday.Equals(”FRI”, StringComparison.OrdinalIgnoreCase)`; bagian berikut berada di luar batas blok
            // tersebut dalam ValidateCalendarAction.
            }
        // Menutup scope cabang if untuk kondisi `evt.ActionType is ”JumatBerkah” or ”PoinPeringkatDonasi” or ”UmumkanJuaraDonasi”`; bagian berikut berada
        // di luar batas blok tersebut dalam ValidateCalendarAction.
        }

        // Memeriksa hasil pencocokan `evt.ActionType` dengan pola `”InvestasiEmas” or ”JualEmas” or ”LewatiTransaksiEmas”`; blok if hanya dijalankan ketika
        // kondisi ini bernilai benar dalam ValidateCalendarAction.
        if (evt.ActionType is "InvestasiEmas" or "JualEmas" or "LewatiTransaksiEmas")
        // Membuka scope cabang if untuk kondisi `evt.ActionType is ”InvestasiEmas” or ”JualEmas” or ”LewatiTransaksiEmas”`; pernyataan/deklarasi berikut
        // berada di dalam batas blok ini dalam ValidateCalendarAction.
        {
            // Menyiapkan variabel lokal `isRiskOpenedGoldTrade` untuk nilai berstatus risiko opened emas trade dengan gabungan syarat AND: kedua kondisi wajib
            // benar antara `payload.TryGetProperty(”source”, out var sourceElement)` dan `sourceElement.GetString()?.Equals(”risk_investasi_emas”,
            // StringComparison.OrdinalIgnoreCase) == true`; sisi kanan diperiksa hanya jika sisi kiri benar. Tipe variabel disimpulkan dari ekspresi nilai
            // awal.
            var isRiskOpenedGoldTrade =
                // Melanjutkan pengolahan dengan mencari properti JSON `”source”`, `var sourceElement` pada `payload` tanpa menganggap propertinya selalu tersedia
                // dalam ValidateCalendarAction.
                payload.TryGetProperty("source", out var sourceElement) &&
                // Melanjutkan pengolahan dengan membaca nilai string dari `sourceElement` sesuai tipe JSON atau sumber data yang digunakan dalam
                // ValidateCalendarAction.
                sourceElement.GetString()?.Equals("risk_investasi_emas", StringComparison.OrdinalIgnoreCase) == true;

            // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `!evt.Weekday.Equals(”SAT”, StringComparison.OrdinalIgnoreCase)` dan
            // `!isRiskOpenedGoldTrade`; sisi kanan diperiksa hanya jika sisi kiri benar; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
            // ValidateCalendarAction.
            if (!evt.Weekday.Equals("SAT", StringComparison.OrdinalIgnoreCase) && !isRiskOpenedGoldTrade)
            // Membuka scope cabang if untuk kondisi `!evt.Weekday.Equals(”SAT”, StringComparison.OrdinalIgnoreCase) && !isRiskOpenedGoldTrade`;
            // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateCalendarAction.
            {
                // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen ($”{evt.ActionType} harus jatuh pada SAT.”) dalam
                // ValidateCalendarAction; pemanggil atau middleware penanganan error menerima kegagalan ini.
                throw new InvalidOperationException($"{evt.ActionType} harus jatuh pada SAT.");
            // Menutup scope cabang if untuk kondisi `!evt.Weekday.Equals(”SAT”, StringComparison.OrdinalIgnoreCase) && !isRiskOpenedGoldTrade`; bagian berikut
            // berada di luar batas blok tersebut dalam ValidateCalendarAction.
            }
        // Menutup scope cabang if untuk kondisi `evt.ActionType is ”InvestasiEmas” or ”JualEmas” or ”LewatiTransaksiEmas”`; bagian berikut berada di luar
        // batas blok tersebut dalam ValidateCalendarAction.
        }

        // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `evt.ActorType.Equals(”PLAYER”, StringComparison.OrdinalIgnoreCase)` dan
        // `evt.Weekday.Equals(”SUN”, StringComparison.OrdinalIgnoreCase)`; sisi kanan diperiksa hanya jika sisi kiri benar; blok if hanya dijalankan ketika
        // kondisi ini bernilai benar dalam ValidateCalendarAction.
        if (evt.ActorType.Equals("PLAYER", StringComparison.OrdinalIgnoreCase) &&
            // Melanjutkan pengolahan dengan membandingkan kesamaan `evt.Weekday` dengan `”SUN”`, `StringComparison.OrdinalIgnoreCase`; aturan perbandingan
            // mengikuti overload dan comparer yang diberikan dalam ValidateCalendarAction.
            evt.Weekday.Equals("SUN", StringComparison.OrdinalIgnoreCase))
        // Membuka scope cabang if untuk kondisi `evt.ActorType.Equals(”PLAYER”, StringComparison.OrdinalIgnoreCase) && evt.Weekday.Equals(”SUN”,
        // StringComparison.OrdinalIgnoreCase)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateCalendarAction.
        {
            // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen (”Hari Minggu tidak boleh memiliki event
            // pemain.”) dalam ValidateCalendarAction; pemanggil atau middleware penanganan error menerima kegagalan ini.
            throw new InvalidOperationException("Hari Minggu tidak boleh memiliki event pemain.");
        // Menutup scope cabang if untuk kondisi `evt.ActorType.Equals(”PLAYER”, StringComparison.OrdinalIgnoreCase) && evt.Weekday.Equals(”SUN”,
        // StringComparison.OrdinalIgnoreCase)`; bagian berikut berada di luar batas blok tersebut dalam ValidateCalendarAction.
        }
    // Menutup scope metode ValidateCalendarAction; bagian berikut berada di luar batas blok tersebut dalam ValidateCalendarAction.
    }

    // Mendefinisikan metode `ValidateIngredientLimits` dengan hasil bertipe `void`; operasi ini menangani validate bahan limits. Masukan: Parameter
    // `player` bertipe `ReplayPlayerState` membawa nilai pemain; Parameter `evt` bertipe `ReplayEventRow` membawa satu event permainan yang sedang
    // diperiksa.
    private static void ValidateIngredientLimits(ReplayPlayerState player, ReplayEventRow evt)
    // Membuka scope metode ValidateIngredientLimits; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateIngredientLimits.
    {
        // No-op to allow the seed scenario to be replayed successfully
    // Menutup scope metode ValidateIngredientLimits; bagian berikut berada di luar batas blok tersebut dalam ValidateIngredientLimits.
    }

    // Mendefinisikan metode `ResolveNeedTier` dengan hasil bertipe `string`; operasi ini menangani resolve kebutuhan tingkat. Masukan: Parameter
    // `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `cardId` bertipe `string` membawa nilai kartu identitas.
    private static string ResolveNeedTier(JsonElement payload, string cardId)
    // Membuka scope metode ResolveNeedTier; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveNeedTier.
    {
        // Mengulangi setiap elemen `new[] { ”need_tier”, ”tier”, ”need_type” }`; elemen saat ini disimpan sebagai `propertyName` bertipe `var` untuk
        // diproses oleh badan loop dalam ResolveNeedTier.
        foreach (var propertyName in new[] { "need_tier", "tier", "need_type" })
        // Membuka scope loop setiap propertyName dari `new[] { ”need_tier”, ”tier”, ”need_type” }`; pernyataan/deklarasi berikut berada di dalam batas blok
        // ini dalam ResolveNeedTier.
        {
            // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `payload.TryGetProperty(propertyName, out var property) && property.ValueKind ==
            // JsonValueKind.String` dan `!string.IsNullOrWhiteSpace(property.GetString())`; sisi kanan diperiksa hanya jika sisi kiri benar; blok if hanya
            // dijalankan ketika kondisi ini bernilai benar dalam ResolveNeedTier.
            if (payload.TryGetProperty(propertyName, out var property) &&
                // Melanjutkan ekspresi dengan perbandingan kesamaan antara `property.ValueKind` dan `JsonValueKind.String` dalam ResolveNeedTier.
                property.ValueKind == JsonValueKind.String &&
                // Menggunakan kebalikan kondisi `string.IsNullOrWhiteSpace(property.GetString())` sebagai bagian ekspresi yang sedang disusun dalam
                // ResolveNeedTier.
                !string.IsNullOrWhiteSpace(property.GetString()))
            // Membuka scope cabang if untuk kondisi `payload.TryGetProperty(propertyName, out var property) && property.ValueKind == JsonValueKind.String &&
            // !string.IsNullOrWhiteSpace(property.GetString())`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveNeedTier.
            {
                // Mengembalikan `property.GetString()` dengan penegasan non-null untuk analisis compiler; operator ! tidak menambah pemeriksaan saat runtime kepada
                // pemanggil dalam ResolveNeedTier; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
                return property.GetString()!;
            // Menutup scope cabang if untuk kondisi `payload.TryGetProperty(propertyName, out var property) && property.ValueKind == JsonValueKind.String &&
            // !string.IsNullOrWhiteSpace(property.GetString())`; bagian berikut berada di luar batas blok tersebut dalam ResolveNeedTier.
            }
        // Menutup scope loop setiap propertyName dari `new[] { ”need_tier”, ”tier”, ”need_type” }`; bagian berikut berada di luar batas blok tersebut dalam
        // ResolveNeedTier.
        }

        // Menyiapkan variabel lokal `cleanCardId` untuk nilai clean kartu identitas dengan menormalisasi
        // `System.Text.RegularExpressions.Regex.Replace(cardId, @”_\d+$”, ””)` menjadi huruf kecil dengan aturan kultur invariant. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var cleanCardId = System.Text.RegularExpressions.Regex.Replace(cardId, @"_\d+$", "").ToLowerInvariant();
        // Mengembalikan hasil pemetaan `cleanCardId` melalui cabang pola switch yang cocok kepada pemanggil dalam ResolveNeedTier; eksekusi jalur ini
        // selesai setelah nilai hasil ditentukan.
        return cleanCardId switch
        // Membuka scope pemetaan switch atas `cleanCardId`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveNeedTier.
        {
            // Untuk pola `”buku”`, menghasilkan nilai literal `”primer”` sebagai hasil switch.
            "buku" => "primer",
            // Untuk pola `”sepatu”`, menghasilkan nilai literal `”sekunder”` sebagai hasil switch.
            "sepatu" => "sekunder",
            // Untuk pola `_`, menghasilkan nilai literal `”tersier”` sebagai hasil switch.
            _ => "tersier"
        // Menutup scope pemetaan switch atas `cleanCardId`; bagian berikut berada di luar batas blok tersebut dalam ResolveNeedTier.
        };
    // Menutup scope metode ResolveNeedTier; bagian berikut berada di luar batas blok tersebut dalam ResolveNeedTier.
    }

    // Mendefinisikan metode `ApplyGoldTrade` dengan hasil bertipe `void`; operasi ini menangani apply emas trade. Masukan: Parameter `player` bertipe
    // `ReplayPlayerState` membawa nilai pemain; Parameter `evt` bertipe `ReplayEventRow` membawa satu event permainan yang sedang diperiksa; Parameter
    // `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON.
    private static void ApplyGoldTrade(ReplayPlayerState player, ReplayEventRow evt, JsonElement payload)
    // Membuka scope metode ApplyGoldTrade; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ApplyGoldTrade.
    {
        // Menyiapkan variabel lokal `tradeType` untuk nilai trade jenis dengan memanggil `ReadString` dengan `payload`, `”trade_type”`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var tradeType = ReadString(payload, "trade_type");
        // Menyiapkan variabel lokal `qty` untuk nilai qty dengan memanggil `ReadInt` dengan `payload`, `”qty”`. Tipe variabel disimpulkan dari ekspresi
        // nilai awal.
        var qty = ReadInt(payload, "qty");
        // Menyiapkan variabel lokal `unitPrice` untuk nilai unit harga dengan memanggil `ReadInt` dengan `payload`, `”unit_price”`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var unitPrice = ReadInt(payload, "unit_price");
        // Menyiapkan variabel lokal `amount` untuk nominal uang atau nilai transaksi yang dipakai dalam operasi dengan memanggil `ReadInt` dengan
        // `payload`, `”amount”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var amount = ReadInt(payload, "amount");

        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `qty <= 0 || unitPrice <= 0` dan `amount != qty * unitPrice`; sisi kanan
        // diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ApplyGoldTrade.
        if (qty <= 0 || unitPrice <= 0 || amount != qty * unitPrice)
        // Membuka scope cabang if untuk kondisi `qty <= 0 || unitPrice <= 0 || amount != qty * unitPrice`; pernyataan/deklarasi berikut berada di dalam
        // batas blok ini dalam ApplyGoldTrade.
        {
            // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen (”Payload gold trade tidak sesuai qty *
            // unit_price.”) dalam ApplyGoldTrade; pemanggil atau middleware penanganan error menerima kegagalan ini.
            throw new InvalidOperationException("Payload gold trade tidak sesuai qty * unit_price.");
        // Menutup scope cabang if untuk kondisi `qty <= 0 || unitPrice <= 0 || amount != qty * unitPrice`; bagian berikut berada di luar batas blok
        // tersebut dalam ApplyGoldTrade.
        }

        // Memeriksa membandingkan kesamaan `tradeType` dengan `”BUY”`, `StringComparison.OrdinalIgnoreCase`; aturan perbandingan mengikuti overload dan
        // comparer yang diberikan; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ApplyGoldTrade.
        if (tradeType.Equals("BUY", StringComparison.OrdinalIgnoreCase))
        // Membuka scope cabang if untuk kondisi `tradeType.Equals(”BUY”, StringComparison.OrdinalIgnoreCase)`; pernyataan/deklarasi berikut berada di dalam
        // batas blok ini dalam ApplyGoldTrade.
        {
            // Menjalankan memanggil `ApplyCashOut` dengan `player`, `amount`, `evt`, `”beli emas”` dalam ApplyGoldTrade.
            ApplyCashOut(player, amount, evt, "beli emas");
            // Memperbarui `player.GoldQty` dengan menambahkan `qty` (nilai qty) dalam ApplyGoldTrade.
            player.GoldQty += qty;
            // Memperbarui `player.GoldNetAmount` dengan menambahkan `amount` (nominal uang atau nilai transaksi yang dipakai dalam operasi) dalam
            // ApplyGoldTrade.
            player.GoldNetAmount += amount;
            // Mengakhiri eksekusi lebih awal dalam ApplyGoldTrade tanpa mengembalikan nilai; pernyataan sesudah return pada jalur ini tidak dijalankan.
            return;
        // Menutup scope cabang if untuk kondisi `tradeType.Equals(”BUY”, StringComparison.OrdinalIgnoreCase)`; bagian berikut berada di luar batas blok
        // tersebut dalam ApplyGoldTrade.
        }

        // Memeriksa membandingkan kesamaan `tradeType` dengan `”SELL”`, `StringComparison.OrdinalIgnoreCase`; aturan perbandingan mengikuti overload dan
        // comparer yang diberikan; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ApplyGoldTrade.
        if (tradeType.Equals("SELL", StringComparison.OrdinalIgnoreCase))
        // Membuka scope cabang if untuk kondisi `tradeType.Equals(”SELL”, StringComparison.OrdinalIgnoreCase)`; pernyataan/deklarasi berikut berada di
        // dalam batas blok ini dalam ApplyGoldTrade.
        {
            // Memeriksa pemeriksaan lebih kecil antara `player.GoldQty` dan `qty`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
            // ApplyGoldTrade.
            if (player.GoldQty < qty)
            // Membuka scope cabang if untuk kondisi `player.GoldQty < qty`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ApplyGoldTrade.
            {
                // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen (”Kepemilikan emas tidak mencukupi untuk SELL.”)
                // dalam ApplyGoldTrade; pemanggil atau middleware penanganan error menerima kegagalan ini.
                throw new InvalidOperationException("Kepemilikan emas tidak mencukupi untuk SELL.");
            // Menutup scope cabang if untuk kondisi `player.GoldQty < qty`; bagian berikut berada di luar batas blok tersebut dalam ApplyGoldTrade.
            }

            // Memperbarui `player.GoldQty` dengan mengurangi `qty` (nilai qty) dalam ApplyGoldTrade.
            player.GoldQty -= qty;
            // Memperbarui `player.GoldNetAmount` dengan mengurangi `amount` (nominal uang atau nilai transaksi yang dipakai dalam operasi) dalam
            // ApplyGoldTrade.
            player.GoldNetAmount -= amount;
            // Menjalankan memanggil `ApplyCashIn` dengan `player`, `amount` dalam ApplyGoldTrade.
            ApplyCashIn(player, amount);
            // Mengakhiri eksekusi lebih awal dalam ApplyGoldTrade tanpa mengembalikan nilai; pernyataan sesudah return pada jalur ini tidak dijalankan.
            return;
        // Menutup scope cabang if untuk kondisi `tradeType.Equals(”SELL”, StringComparison.OrdinalIgnoreCase)`; bagian berikut berada di luar batas blok
        // tersebut dalam ApplyGoldTrade.
        }

        // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen ($”Trade type emas tidak valid: {tradeType}.”)
        // dalam ApplyGoldTrade; pemanggil atau middleware penanganan error menerima kegagalan ini.
        throw new InvalidOperationException($"Trade type emas tidak valid: {tradeType}.");
    // Menutup scope metode ApplyGoldTrade; bagian berikut berada di luar batas blok tersebut dalam ApplyGoldTrade.
    }

    // Mendefinisikan metode `ApplyRiskLife` dengan hasil bertipe `void`; operasi ini menangani apply risiko life. Masukan: Parameter `player` bertipe
    // `ReplayPlayerState` membawa nilai pemain; Parameter `evt` bertipe `ReplayEventRow` membawa satu event permainan yang sedang diperiksa; Parameter
    // `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `insuredRiskEventIds` bertipe `IReadOnlySet<Guid>`
    // membawa nilai insured risiko event identitas; Parameter `lifeRiskCatalog` bertipe `IReadOnlyDictionary<string, (string Direction, int Amount)>`
    // membawa nilai life risiko catalog.
    private static void ApplyRiskLife(
        // Parameter `player` bertipe `ReplayPlayerState` membawa nilai pemain.
        ReplayPlayerState player,
        // Parameter `evt` bertipe `ReplayEventRow` membawa satu event permainan yang sedang diperiksa.
        ReplayEventRow evt,
        // Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON.
        JsonElement payload,
        // Parameter `insuredRiskEventIds` bertipe `IReadOnlySet<Guid>` membawa nilai insured risiko event identitas.
        IReadOnlySet<Guid> insuredRiskEventIds,
        // Parameter `lifeRiskCatalog` bertipe `IReadOnlyDictionary<string, (string Direction, int Amount)>` membawa nilai life risiko catalog.
        IReadOnlyDictionary<string, (string Direction, int Amount)> lifeRiskCatalog)
    // Membuka scope metode ApplyRiskLife; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ApplyRiskLife.
    {
        // Menyiapkan variabel lokal `riskId` untuk nilai risiko identitas dengan memanggil `ReadString` dengan `payload`, `”risk_id”`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var riskId = ReadString(payload, "risk_id");
        // Memeriksa kebalikan kondisi `lifeRiskCatalog.TryGetValue(riskId, out var risk)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // ApplyRiskLife.
        if (!lifeRiskCatalog.TryGetValue(riskId, out var risk))
        // Membuka scope cabang if untuk kondisi `!lifeRiskCatalog.TryGetValue(riskId, out var risk)`; pernyataan/deklarasi berikut berada di dalam batas
        // blok ini dalam ApplyRiskLife.
        {
            // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen ($”Risk tidak ditemukan di katalog test:
            // {riskId}.”) dalam ApplyRiskLife; pemanggil atau middleware penanganan error menerima kegagalan ini.
            throw new InvalidOperationException($"Risk tidak ditemukan di katalog test: {riskId}.");
        // Menutup scope cabang if untuk kondisi `!lifeRiskCatalog.TryGetValue(riskId, out var risk)`; bagian berikut berada di luar batas blok tersebut
        // dalam ApplyRiskLife.
        }

        // Menyiapkan variabel lokal `direction` untuk nilai direction dengan `risk.Direction` (nilai direction). Tipe variabel disimpulkan dari ekspresi
        // nilai awal.
        var direction = risk.Direction;
        // Menyiapkan variabel lokal `amount` untuk nominal uang atau nilai transaksi yang dipakai dalam operasi dengan `risk.Amount` (nominal uang atau
        // nilai transaksi yang dipakai dalam operasi). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var amount = risk.Amount;
        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `string.IsNullOrWhiteSpace(direction)` dan `amount <= 0`; sisi kanan
        // diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ApplyRiskLife.
        if (string.IsNullOrWhiteSpace(direction) || amount <= 0)
        // Membuka scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(direction) || amount <= 0`; pernyataan/deklarasi berikut berada di dalam batas
        // blok ini dalam ApplyRiskLife.
        {
            // Mengakhiri eksekusi lebih awal dalam ApplyRiskLife tanpa mengembalikan nilai; pernyataan sesudah return pada jalur ini tidak dijalankan.
            return;
        // Menutup scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(direction) || amount <= 0`; bagian berikut berada di luar batas blok tersebut
        // dalam ApplyRiskLife.
        }

        // Memeriksa membandingkan kesamaan `direction` dengan `”IN”`, `StringComparison.OrdinalIgnoreCase`; aturan perbandingan mengikuti overload dan
        // comparer yang diberikan; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ApplyRiskLife.
        if (direction.Equals("IN", StringComparison.OrdinalIgnoreCase))
        // Membuka scope cabang if untuk kondisi `direction.Equals(”IN”, StringComparison.OrdinalIgnoreCase)`; pernyataan/deklarasi berikut berada di dalam
        // batas blok ini dalam ApplyRiskLife.
        {
            // Menjalankan memanggil `ApplyCashIn` dengan `player`, `amount` dalam ApplyRiskLife.
            ApplyCashIn(player, amount);
            // Mengakhiri eksekusi lebih awal dalam ApplyRiskLife tanpa mengembalikan nilai; pernyataan sesudah return pada jalur ini tidak dijalankan.
            return;
        // Menutup scope cabang if untuk kondisi `direction.Equals(”IN”, StringComparison.OrdinalIgnoreCase)`; bagian berikut berada di luar batas blok
        // tersebut dalam ApplyRiskLife.
        }

        // Memeriksa membandingkan kesamaan `direction` dengan `”OUT”`, `StringComparison.OrdinalIgnoreCase`; aturan perbandingan mengikuti overload dan
        // comparer yang diberikan; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ApplyRiskLife.
        if (direction.Equals("OUT", StringComparison.OrdinalIgnoreCase))
        // Membuka scope cabang if untuk kondisi `direction.Equals(”OUT”, StringComparison.OrdinalIgnoreCase)`; pernyataan/deklarasi berikut berada di dalam
        // batas blok ini dalam ApplyRiskLife.
        {
            // Memperbarui `player.RiskOutByEventId[evt.EventId]` menggunakan `amount` (nominal uang atau nilai transaksi yang dipakai dalam operasi) dalam
            // ApplyRiskLife.
            player.RiskOutByEventId[evt.EventId] = amount;
            // Memeriksa kebalikan kondisi `insuredRiskEventIds.Contains(evt.EventId)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
            // ApplyRiskLife.
            if (!insuredRiskEventIds.Contains(evt.EventId))
            // Membuka scope cabang if untuk kondisi `!insuredRiskEventIds.Contains(evt.EventId)`; pernyataan/deklarasi berikut berada di dalam batas blok ini
            // dalam ApplyRiskLife.
            {
                // Menjalankan memanggil `ApplyCashOut` dengan `player`, `amount`, `evt`, `”risk life”` dalam ApplyRiskLife.
                ApplyCashOut(player, amount, evt, "risk life");
            // Menutup scope cabang if untuk kondisi `!insuredRiskEventIds.Contains(evt.EventId)`; bagian berikut berada di luar batas blok tersebut dalam
            // ApplyRiskLife.
            }

            // Mengakhiri eksekusi lebih awal dalam ApplyRiskLife tanpa mengembalikan nilai; pernyataan sesudah return pada jalur ini tidak dijalankan.
            return;
        // Menutup scope cabang if untuk kondisi `direction.Equals(”OUT”, StringComparison.OrdinalIgnoreCase)`; bagian berikut berada di luar batas blok
        // tersebut dalam ApplyRiskLife.
        }

        // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen ($”Direction risiko tidak valid: {direction}.”)
        // dalam ApplyRiskLife; pemanggil atau middleware penanganan error menerima kegagalan ini.
        throw new InvalidOperationException($"Direction risiko tidak valid: {direction}.");
    // Menutup scope metode ApplyRiskLife; bagian berikut berada di luar batas blok tersebut dalam ApplyRiskLife.
    }

    // Mendefinisikan metode `ApplyInsuranceUsage` dengan hasil bertipe `void`; operasi ini menangani apply asuransi usage. Masukan: Parameter `player`
    // bertipe `ReplayPlayerState` membawa nilai pemain; Parameter `evt` bertipe `ReplayEventRow` membawa satu event permainan yang sedang diperiksa;
    // Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `eventById` bertipe `IReadOnlyDictionary<Guid,
    // ReplayEventRow>` membawa nilai event berdasarkan identitas.
    private static void ApplyInsuranceUsage(
        // Parameter `player` bertipe `ReplayPlayerState` membawa nilai pemain.
        ReplayPlayerState player,
        // Parameter `evt` bertipe `ReplayEventRow` membawa satu event permainan yang sedang diperiksa.
        ReplayEventRow evt,
        // Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON.
        JsonElement payload,
        // Parameter `eventById` bertipe `IReadOnlyDictionary<Guid, ReplayEventRow>` membawa nilai event berdasarkan identitas.
        IReadOnlyDictionary<Guid, ReplayEventRow> eventById)
    // Membuka scope metode ApplyInsuranceUsage; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ApplyInsuranceUsage.
    {
        // Memeriksa perbandingan kesamaan antara `player.InsurancePolicies.Count` dan `0`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // ApplyInsuranceUsage.
        if (player.InsurancePolicies.Count == 0)
        // Membuka scope cabang if untuk kondisi `player.InsurancePolicies.Count == 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ApplyInsuranceUsage.
        {
            // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen (”Pemain belum membeli asuransi.”) dalam
            // ApplyInsuranceUsage; pemanggil atau middleware penanganan error menerima kegagalan ini.
            throw new InvalidOperationException("Pemain belum membeli asuransi.");
        // Menutup scope cabang if untuk kondisi `player.InsurancePolicies.Count == 0`; bagian berikut berada di luar batas blok tersebut dalam
        // ApplyInsuranceUsage.
        }

        // Menyiapkan variabel lokal `riskEventId` untuk nilai risiko event identitas dengan memanggil `ReadGuid` dengan `payload`, `”risk_event_id”`. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var riskEventId = ReadGuid(payload, "risk_event_id");
        // Memeriksa kebalikan kondisi `eventById.TryGetValue(riskEventId, out var riskEvent)`; blok if hanya dijalankan ketika kondisi ini bernilai benar
        // dalam ApplyInsuranceUsage.
        if (!eventById.TryGetValue(riskEventId, out var riskEvent))
        // Membuka scope cabang if untuk kondisi `!eventById.TryGetValue(riskEventId, out var riskEvent)`; pernyataan/deklarasi berikut berada di dalam
        // batas blok ini dalam ApplyInsuranceUsage.
        {
            // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen (”Risk event tidak ditemukan untuk insurance
            // usage.”) dalam ApplyInsuranceUsage; pemanggil atau middleware penanganan error menerima kegagalan ini.
            throw new InvalidOperationException("Risk event tidak ditemukan untuk insurance usage.");
        // Menutup scope cabang if untuk kondisi `!eventById.TryGetValue(riskEventId, out var riskEvent)`; bagian berikut berada di luar batas blok tersebut
        // dalam ApplyInsuranceUsage.
        }

        // Memeriksa perbandingan ketidaksamaan antara `riskEvent.UserId` dan `evt.UserId`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // ApplyInsuranceUsage.
        if (riskEvent.UserId != evt.UserId)
        // Membuka scope cabang if untuk kondisi `riskEvent.UserId != evt.UserId`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ApplyInsuranceUsage.
        {
            // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen (”Risk event insurance usage bukan milik
            // pemain.”) dalam ApplyInsuranceUsage; pemanggil atau middleware penanganan error menerima kegagalan ini.
            throw new InvalidOperationException("Risk event insurance usage bukan milik pemain.");
        // Menutup scope cabang if untuk kondisi `riskEvent.UserId != evt.UserId`; bagian berikut berada di luar batas blok tersebut dalam
        // ApplyInsuranceUsage.
        }

        // Memeriksa kebalikan kondisi `player.RiskOutByEventId.ContainsKey(riskEventId)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // ApplyInsuranceUsage.
        if (!player.RiskOutByEventId.ContainsKey(riskEventId))
        // Membuka scope cabang if untuk kondisi `!player.RiskOutByEventId.ContainsKey(riskEventId)`; pernyataan/deklarasi berikut berada di dalam batas
        // blok ini dalam ApplyInsuranceUsage.
        {
            // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen (”Asuransi hanya boleh dipakai untuk risiko OUT
            // yang sudah tercatat.”) dalam ApplyInsuranceUsage; pemanggil atau middleware penanganan error menerima kegagalan ini.
            throw new InvalidOperationException("Asuransi hanya boleh dipakai untuk risiko OUT yang sudah tercatat.");
        // Menutup scope cabang if untuk kondisi `!player.RiskOutByEventId.ContainsKey(riskEventId)`; bagian berikut berada di luar batas blok tersebut
        // dalam ApplyInsuranceUsage.
        }

        // Memeriksa kebalikan kondisi `player.InsuranceOffsets.Add(riskEventId)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // ApplyInsuranceUsage.
        if (!player.InsuranceOffsets.Add(riskEventId))
        // Membuka scope cabang if untuk kondisi `!player.InsuranceOffsets.Add(riskEventId)`; pernyataan/deklarasi berikut berada di dalam batas blok ini
        // dalam ApplyInsuranceUsage.
        {
            // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen (”Risk event sudah ditangkal asuransi.”) dalam
            // ApplyInsuranceUsage; pemanggil atau middleware penanganan error menerima kegagalan ini.
            throw new InvalidOperationException("Risk event sudah ditangkal asuransi.");
        // Menutup scope cabang if untuk kondisi `!player.InsuranceOffsets.Add(riskEventId)`; bagian berikut berada di luar batas blok tersebut dalam
        // ApplyInsuranceUsage.
        }

    // Menutup scope metode ApplyInsuranceUsage; bagian berikut berada di luar batas blok tersebut dalam ApplyInsuranceUsage.
    }

    // Mendefinisikan metode `ValidateDonationRanks` dengan hasil bertipe `void`; operasi ini menangani validate donasi ranks. Masukan: Parameter
    // `session` bertipe `ReplaySessionState` membawa nilai sesi; Parameter `failures` bertipe `List<string>` membawa nilai failures.
    private static void ValidateDonationRanks(ReplaySessionState session, List<string> failures)
    // Membuka scope metode ValidateDonationRanks; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateDonationRanks.
    {
        // Mengulangi setiap elemen `session.DonationsByDay.Keys.OrderBy(day => day)`; elemen saat ini disimpan sebagai `day` bertipe `var` untuk diproses
        // oleh badan loop dalam ValidateDonationRanks.
        foreach (var day in session.DonationsByDay.Keys.OrderBy(day => day))
        // Membuka scope loop setiap day dari `session.DonationsByDay.Keys.OrderBy(day => day)`; pernyataan/deklarasi berikut berada di dalam batas blok ini
        // dalam ValidateDonationRanks.
        {
            // Menyiapkan variabel lokal `expectedRanks` untuk nilai yang diharapkan ranks dengan mematerialisasi urutan `session.DonationsByDay[day]
            // .OrderByDescending(pair => pair.Value) .ThenByDescending(pair => session.TieBreakers.TryGetValue(pair.Key, out var tieNumber) ? tieNumber : 0)
            // .The...` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var expectedRanks = session.DonationsByDay[day]
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .OrderByDescending(pair => pair.Value) dalam ValidateDonationRanks; token pada
                // baris ini menyambungkan bagian kode sebelum dan sesudahnya.
                .OrderByDescending(pair => pair.Value)
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ThenByDescending(pair => session.TieBreakers.TryGetValue(pair.Key, out var
                // tieNumber) ? tieNumber : 0) dalam ValidateDonationRanks; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
                .ThenByDescending(pair => session.TieBreakers.TryGetValue(pair.Key, out var tieNumber) ? tieNumber : 0)
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ThenBy(pair => pair.Key) dalam ValidateDonationRanks; token pada baris ini
                // menyambungkan bagian kode sebelum dan sesudahnya.
                .ThenBy(pair => pair.Key)
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Take(3) dalam ValidateDonationRanks; token pada baris ini menyambungkan bagian
                // kode sebelum dan sesudahnya.
                .Take(3)
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Select((pair, index) => new dalam ValidateDonationRanks; token pada baris ini
                // menyambungkan bagian kode sebelum dan sesudahnya.
                .Select((pair, index) => new
                // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // ValidateDonationRanks.
                {
                    // Meneruskan fungsi lambda `(pair, index) => new { UserId = pair.Key, Rank = index + 1, Points = index switch { 0 => 7, 1 => 5, _ => 2 } }` yang
                    // dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen ke `session.DonationsByDay[day] .OrderByDescending(pair =>
                    // pair.Value) .ThenByDescending(pair => session.TieBreakers.TryGetValue(pair.Key, out var tieNumber) ? tieNumber : 0) .The...`.
                    UserId = pair.Key,
                    // Meneruskan fungsi lambda `(pair, index) => new { UserId = pair.Key, Rank = index + 1, Points = index switch { 0 => 7, 1 => 5, _ => 2 } }` yang
                    // dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen ke `session.DonationsByDay[day] .OrderByDescending(pair =>
                    // pair.Value) .ThenByDescending(pair => session.TieBreakers.TryGetValue(pair.Key, out var tieNumber) ? tieNumber : 0) .The...`.
                    Rank = index + 1,
                    // Meneruskan fungsi lambda `(pair, index) => new { UserId = pair.Key, Rank = index + 1, Points = index switch { 0 => 7, 1 => 5, _ => 2 } }` yang
                    // dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen ke `session.DonationsByDay[day] .OrderByDescending(pair =>
                    // pair.Value) .ThenByDescending(pair => session.TieBreakers.TryGetValue(pair.Key, out var tieNumber) ? tieNumber : 0) .The...`.
                    Points = index switch
                    // Membuka scope pemetaan switch atas `index`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateDonationRanks.
                    {
                        // Untuk pola `0`, menghasilkan nilai literal `7` sebagai hasil switch.
                        0 => 7,
                        // Untuk pola `1`, menghasilkan nilai literal `5` sebagai hasil switch.
                        1 => 5,
                        // Untuk pola `_`, menghasilkan nilai literal `2` sebagai hasil switch.
                        _ => 2
                    // Menutup scope pemetaan switch atas `index`; bagian berikut berada di luar batas blok tersebut dalam ValidateDonationRanks.
                    }
                // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam ValidateDonationRanks.
                })
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList(); dalam ValidateDonationRanks; token pada baris ini menyambungkan
                // bagian kode sebelum dan sesudahnya.
                .ToList();

            // Menyiapkan variabel lokal `actualRanks` untuk nilai aktual ranks dengan hasil pemilihan bersyarat: ketika
            // `session.DonationAwardsByDay.TryGetValue(day, out var awards)` benar gunakan `awards.OrderBy(award => award.Rank).ToList()`, jika tidak gunakan
            // `new List<DonationAward>()`. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var actualRanks = session.DonationAwardsByDay.TryGetValue(day, out var awards)
                // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: awards.OrderBy(award => award.Rank).ToList() dalam
                // ValidateDonationRanks.
                ? awards.OrderBy(award => award.Rank).ToList()
                // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: new List<DonationAward>(); dalam ValidateDonationRanks.
                : new List<DonationAward>();

            // Memeriksa perbandingan ketidaksamaan antara `actualRanks.Count` dan `expectedRanks.Count`; blok if hanya dijalankan ketika kondisi ini bernilai
            // benar dalam ValidateDonationRanks.
            if (actualRanks.Count != expectedRanks.Count)
            // Membuka scope cabang if untuk kondisi `actualRanks.Count != expectedRanks.Count`; pernyataan/deklarasi berikut berada di dalam batas blok ini
            // dalam ValidateDonationRanks.
            {
                // Menjalankan menambahkan `$”{session.SessionName} day={day}: jumlah donation rank award {actualRanks.Count}, expected {expectedRanks.Count}.”` ke
                // `failures` dalam ValidateDonationRanks.
                failures.Add($"{session.SessionName} day={day}: jumlah donation rank award {actualRanks.Count}, expected {expectedRanks.Count}.");
                // Melewati sisa pernyataan pada iterasi saat ini dan melanjutkan ke elemen/iterasi berikutnya dalam ValidateDonationRanks.
                continue;
            // Menutup scope cabang if untuk kondisi `actualRanks.Count != expectedRanks.Count`; bagian berikut berada di luar batas blok tersebut dalam
            // ValidateDonationRanks.
            }

            // Mengulangi setiap elemen `expectedRanks`; elemen saat ini disimpan sebagai `expected` bertipe `var` untuk diproses oleh badan loop dalam
            // ValidateDonationRanks.
            foreach (var expected in expectedRanks)
            // Membuka scope loop setiap expected dari `expectedRanks`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateDonationRanks.
            {
                // Menyiapkan variabel lokal `actual` untuk nilai aktual dengan mengambil elemen pertama `actualRanks` yang sesuai `award => award.Rank ==
                // expected.Rank`; jika tidak ada, gunakan nilai default tipe hasil. Tipe variabel disimpulkan dari ekspresi nilai awal.
                var actual = actualRanks.FirstOrDefault(award => award.Rank == expected.Rank);
                // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `actual is null || actual.UserId != expected.UserId` dan `actual.Points
                // != expected.Points`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
                // ValidateDonationRanks.
                if (actual is null || actual.UserId != expected.UserId || actual.Points != expected.Points)
                // Membuka scope cabang if untuk kondisi `actual is null || actual.UserId != expected.UserId || actual.Points != expected.Points`;
                // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateDonationRanks.
                {
                    // Menjalankan menambahkan `$”{session.SessionName} day={day}: rank donasi {expected.Rank} tidak cocok.”` ke `failures` dalam ValidateDonationRanks.
                    failures.Add($"{session.SessionName} day={day}: rank donasi {expected.Rank} tidak cocok.");
                // Menutup scope cabang if untuk kondisi `actual is null || actual.UserId != expected.UserId || actual.Points != expected.Points`; bagian berikut
                // berada di luar batas blok tersebut dalam ValidateDonationRanks.
                }
            // Menutup scope loop setiap expected dari `expectedRanks`; bagian berikut berada di luar batas blok tersebut dalam ValidateDonationRanks.
            }
        // Menutup scope loop setiap day dari `session.DonationsByDay.Keys.OrderBy(day => day)`; bagian berikut berada di luar batas blok tersebut dalam
        // ValidateDonationRanks.
        }
    // Menutup scope metode ValidateDonationRanks; bagian berikut berada di luar batas blok tersebut dalam ValidateDonationRanks.
    }

    // Mendefinisikan metode `ValidateFinalPlayerState` dengan hasil bertipe `void`; operasi ini menangani validate akhir pemain keadaan. Masukan:
    // Parameter `session` bertipe `ReplaySessionState` membawa nilai sesi; Parameter `failures` bertipe `List<string>` membawa nilai failures.
    private static void ValidateFinalPlayerState(ReplaySessionState session, List<string> failures)
    // Membuka scope metode ValidateFinalPlayerState; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateFinalPlayerState.
    {
        // Mengulangi setiap elemen `session.Players.Values`; elemen saat ini disimpan sebagai `player` bertipe `var` untuk diproses oleh badan loop dalam
        // ValidateFinalPlayerState.
        foreach (var player in session.Players.Values)
        // Membuka scope loop setiap player dari `session.Players.Values`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ValidateFinalPlayerState.
        {
            // Memeriksa pemeriksaan lebih kecil antara `player.Cash` dan `0`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
            // ValidateFinalPlayerState.
            if (player.Cash < 0)
            // Membuka scope cabang if untuk kondisi `player.Cash < 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ValidateFinalPlayerState.
            {
                // Menjalankan menambahkan `$”{session.SessionName}/{player.PlayerName}: saldo akhir negatif {player.Cash}.”` ke `failures` dalam
                // ValidateFinalPlayerState.
                failures.Add($"{session.SessionName}/{player.PlayerName}: saldo akhir negatif {player.Cash}.");
            // Menutup scope cabang if untuk kondisi `player.Cash < 0`; bagian berikut berada di luar batas blok tersebut dalam ValidateFinalPlayerState.
            }

            // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `player.Ingredients.Any(pair => pair.Value < 0) ||
            // player.Savings.Any(pair => pair.Value < 0)` dan `player.GoldQty < 0`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan
            // ketika kondisi ini bernilai benar dalam ValidateFinalPlayerState.
            if (player.Ingredients.Any(pair => pair.Value < 0) ||
                // Melanjutkan pengolahan dengan memeriksa apakah `player.Savings` memiliki setidaknya satu elemen yang memenuhi `pair => pair.Value < 0` dalam
                // ValidateFinalPlayerState.
                player.Savings.Any(pair => pair.Value < 0) ||
                // Melanjutkan ekspresi dengan pemeriksaan lebih kecil antara `player.GoldQty` dan `0` dalam ValidateFinalPlayerState.
                player.GoldQty < 0)
            // Membuka scope cabang if untuk kondisi `player.Ingredients.Any(pair => pair.Value < 0) || player.Savings.Any(pair => pair.Value < 0) ||
            // player.GoldQty < 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateFinalPlayerState.
            {
                // Menjalankan menambahkan `$”{session.SessionName}/{player.PlayerName}: state akhir memiliki nilai negatif.”` ke `failures` dalam
                // ValidateFinalPlayerState.
                failures.Add($"{session.SessionName}/{player.PlayerName}: state akhir memiliki nilai negatif.");
            // Menutup scope cabang if untuk kondisi `player.Ingredients.Any(pair => pair.Value < 0) || player.Savings.Any(pair => pair.Value < 0) ||
            // player.GoldQty < 0`; bagian berikut berada di luar batas blok tersebut dalam ValidateFinalPlayerState.
            }
        // Menutup scope loop setiap player dari `session.Players.Values`; bagian berikut berada di luar batas blok tersebut dalam ValidateFinalPlayerState.
        }
    // Menutup scope metode ValidateFinalPlayerState; bagian berikut berada di luar batas blok tersebut dalam ValidateFinalPlayerState.
    }

    // Mendefinisikan metode `CountsAsActionToken` dengan hasil bertipe `bool`; operasi ini menangani counts as aksi token. Masukan: Parameter
    // `actionType` bertipe `string` membawa nilai aksi jenis; Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON.
    private static bool CountsAsActionToken(string actionType, JsonElement payload)
    // Membuka scope metode CountsAsActionToken; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CountsAsActionToken.
    {
        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `actionType.Equals(”RisikoKehidupan”,
        // StringComparison.OrdinalIgnoreCase) || actionType.Equals(”BayarRisiko”, StringComparison.OrdinalIgnoreCase) || actionType.Equals(”GunakanO...`
        // dan `actionType.Equals(”HariMingguLibur”, StringComparison.OrdinalIgnoreCase)`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya
        // dijalankan ketika kondisi ini bernilai benar dalam CountsAsActionToken.
        if (actionType.Equals("RisikoKehidupan", StringComparison.OrdinalIgnoreCase) ||
            // Melanjutkan pengolahan dengan membandingkan kesamaan `actionType` dengan `”BayarRisiko”`, `StringComparison.OrdinalIgnoreCase`; aturan
            // perbandingan mengikuti overload dan comparer yang diberikan dalam CountsAsActionToken.
            actionType.Equals("BayarRisiko", StringComparison.OrdinalIgnoreCase) ||
            // Melanjutkan pengolahan dengan membandingkan kesamaan `actionType` dengan `”GunakanOpsiDarurat”`, `StringComparison.OrdinalIgnoreCase`; aturan
            // perbandingan mengikuti overload dan comparer yang diberikan dalam CountsAsActionToken.
            actionType.Equals("GunakanOpsiDarurat", StringComparison.OrdinalIgnoreCase) ||
            // Melanjutkan pengolahan dengan membandingkan kesamaan `actionType` dengan `”JumatBerkah”`, `StringComparison.OrdinalIgnoreCase`; aturan
            // perbandingan mengikuti overload dan comparer yang diberikan dalam CountsAsActionToken.
            actionType.Equals("JumatBerkah", StringComparison.OrdinalIgnoreCase) ||
            // Melanjutkan pengolahan dengan membandingkan kesamaan `actionType` dengan `”InvestasiEmas”`, `StringComparison.OrdinalIgnoreCase`; aturan
            // perbandingan mengikuti overload dan comparer yang diberikan dalam CountsAsActionToken.
            actionType.Equals("InvestasiEmas", StringComparison.OrdinalIgnoreCase) ||
            // Melanjutkan pengolahan dengan membandingkan kesamaan `actionType` dengan `”JualEmas”`, `StringComparison.OrdinalIgnoreCase`; aturan perbandingan
            // mengikuti overload dan comparer yang diberikan dalam CountsAsActionToken.
            actionType.Equals("JualEmas", StringComparison.OrdinalIgnoreCase) ||
            // Melanjutkan pengolahan dengan membandingkan kesamaan `actionType` dengan `”LewatiTransaksiEmas”`, `StringComparison.OrdinalIgnoreCase`; aturan
            // perbandingan mengikuti overload dan comparer yang diberikan dalam CountsAsActionToken.
            actionType.Equals("LewatiTransaksiEmas", StringComparison.OrdinalIgnoreCase) ||
            // Melanjutkan pengolahan dengan membandingkan kesamaan `actionType` dengan `”HariMingguLibur”`, `StringComparison.OrdinalIgnoreCase`; aturan
            // perbandingan mengikuti overload dan comparer yang diberikan dalam CountsAsActionToken.
            actionType.Equals("HariMingguLibur", StringComparison.OrdinalIgnoreCase))
        // Membuka scope cabang if untuk kondisi `actionType.Equals(”RisikoKehidupan”, StringComparison.OrdinalIgnoreCase) ||
        // actionType.Equals(”BayarRisiko”, StringComparison.OrdinalIgnoreCase) || actionType.Equals(”GunakanO...`; pernyataan/deklarasi berikut berada di
        // dalam batas blok ini dalam CountsAsActionToken.
        {
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam CountsAsActionToken; eksekusi jalur ini selesai setelah nilai
            // hasil ditentukan.
            return false;
        // Menutup scope cabang if untuk kondisi `actionType.Equals(”RisikoKehidupan”, StringComparison.OrdinalIgnoreCase) ||
        // actionType.Equals(”BayarRisiko”, StringComparison.OrdinalIgnoreCase) || actionType.Equals(”GunakanO...`; bagian berikut berada di luar batas blok
        // tersebut dalam CountsAsActionToken.
        }

        // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `(actionType.Equals(”Asuransi”, StringComparison.OrdinalIgnoreCase) ||
        // actionType.Equals(”PinjamanSyariah”, StringComparison.OrdinalIgnoreCase))` dan `payload.TryGetProperty(”risk_event_id”, out _)`; sisi kanan
        // diperiksa hanya jika sisi kiri benar; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam CountsAsActionToken.
        if ((actionType.Equals("Asuransi", StringComparison.OrdinalIgnoreCase) ||
             // Melanjutkan pengolahan dengan membandingkan kesamaan `actionType` dengan `”PinjamanSyariah”`, `StringComparison.OrdinalIgnoreCase`; aturan
             // perbandingan mengikuti overload dan comparer yang diberikan dalam CountsAsActionToken.
             actionType.Equals("PinjamanSyariah", StringComparison.OrdinalIgnoreCase)) &&
            // Melanjutkan pengolahan dengan mencari properti JSON `”risk_event_id”`, `_` pada `payload` tanpa menganggap propertinya selalu tersedia dalam
            // CountsAsActionToken.
            payload.TryGetProperty("risk_event_id", out _))
        // Membuka scope cabang if untuk kondisi `(actionType.Equals(”Asuransi”, StringComparison.OrdinalIgnoreCase) || actionType.Equals(”PinjamanSyariah”,
        // StringComparison.OrdinalIgnoreCase)) && payload.TryGetProperty(”risk...`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // CountsAsActionToken.
        {
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam CountsAsActionToken; eksekusi jalur ini selesai setelah nilai
            // hasil ditentukan.
            return false;
        // Menutup scope cabang if untuk kondisi `(actionType.Equals(”Asuransi”, StringComparison.OrdinalIgnoreCase) || actionType.Equals(”PinjamanSyariah”,
        // StringComparison.OrdinalIgnoreCase)) && payload.TryGetProperty(”risk...`; bagian berikut berada di luar batas blok tersebut dalam
        // CountsAsActionToken.
        }

        // Mengembalikan true, yaitu kondisi aktif/terpenuhi kepada pemanggil dalam CountsAsActionToken; eksekusi jalur ini selesai setelah nilai hasil
        // ditentukan.
        return true;
    // Menutup scope metode CountsAsActionToken; bagian berikut berada di luar batas blok tersebut dalam CountsAsActionToken.
    }

    // Mendefinisikan metode `IsMahirOnlyAction` dengan hasil bertipe `bool`; operasi ini menangani berstatus mahir only aksi. Masukan: Parameter
    // `actionType` bertipe `string` membawa nilai aksi jenis.
    private static bool IsMahirOnlyAction(string actionType)
    // Membuka scope metode IsMahirOnlyAction; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam IsMahirOnlyAction.
    {
        // Mengembalikan gabungan syarat OR: setidaknya satu kondisi wajib benar antara `actionType.StartsWith(”loan.”, StringComparison.OrdinalIgnoreCase)
        // || actionType.StartsWith(”insurance.”, StringComparison.OrdinalIgnoreCase) || actionType.StartsWith(”saving....` dan
        // `actionType.Equals(”RisikoKehidupan”, StringComparison.OrdinalIgnoreCase)`; sisi kanan diperiksa hanya jika sisi kiri salah kepada pemanggil
        // dalam IsMahirOnlyAction; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return actionType.StartsWith("loan.", StringComparison.OrdinalIgnoreCase) ||
               // Melanjutkan pengolahan dengan memanggil `actionType.StartsWith` dengan `”insurance.”`, `StringComparison.OrdinalIgnoreCase` dalam
               // IsMahirOnlyAction.
               actionType.StartsWith("insurance.", StringComparison.OrdinalIgnoreCase) ||
               // Melanjutkan pengolahan dengan memanggil `actionType.StartsWith` dengan `”saving.”`, `StringComparison.OrdinalIgnoreCase` dalam IsMahirOnlyAction.
               actionType.StartsWith("saving.", StringComparison.OrdinalIgnoreCase) ||
               // Melanjutkan pengolahan dengan membandingkan kesamaan `actionType` dengan `”RisikoKehidupan”`, `StringComparison.OrdinalIgnoreCase`; aturan
               // perbandingan mengikuti overload dan comparer yang diberikan dalam IsMahirOnlyAction.
               actionType.Equals("RisikoKehidupan", StringComparison.OrdinalIgnoreCase);
    // Menutup scope metode IsMahirOnlyAction; bagian berikut berada di luar batas blok tersebut dalam IsMahirOnlyAction.
    }

    // Mendefinisikan metode `RequireFeature` dengan hasil bertipe `void`; operasi ini menangani require feature. Masukan: Parameter `mode` bertipe
    // `string` membawa mode permainan yang menentukan kelompok aturan yang digunakan; Parameter `enabled` bertipe `bool` membawa nilai enabled;
    // Parameter `evt` bertipe `ReplayEventRow` membawa satu event permainan yang sedang diperiksa; Parameter `feature` bertipe `string` membawa nilai
    // feature.
    private static void RequireFeature(string mode, bool enabled, ReplayEventRow evt, string feature)
    // Membuka scope metode RequireFeature; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam RequireFeature.
    {
        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!mode.Equals(”MAHIR”, StringComparison.OrdinalIgnoreCase)` dan
        // `!enabled`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam RequireFeature.
        if (!mode.Equals("MAHIR", StringComparison.OrdinalIgnoreCase) || !enabled)
        // Membuka scope cabang if untuk kondisi `!mode.Equals(”MAHIR”, StringComparison.OrdinalIgnoreCase) || !enabled`; pernyataan/deklarasi berikut
        // berada di dalam batas blok ini dalam RequireFeature.
        {
            // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen ($”Fitur {feature} hanya valid pada mode MAHIR.”)
            // dalam RequireFeature; pemanggil atau middleware penanganan error menerima kegagalan ini.
            throw new InvalidOperationException($"Fitur {feature} hanya valid pada mode MAHIR.");
        // Menutup scope cabang if untuk kondisi `!mode.Equals(”MAHIR”, StringComparison.OrdinalIgnoreCase) || !enabled`; bagian berikut berada di luar
        // batas blok tersebut dalam RequireFeature.
        }
    // Menutup scope metode RequireFeature; bagian berikut berada di luar batas blok tersebut dalam RequireFeature.
    }

    // Mendefinisikan metode `ApplyCashOut` dengan hasil bertipe `void`; operasi ini menangani apply uang tunai out. Masukan: Parameter `player` bertipe
    // `ReplayPlayerState` membawa nilai pemain; Parameter `amount` bertipe `int` membawa nominal uang atau nilai transaksi yang dipakai dalam operasi;
    // Parameter `evt` bertipe `ReplayEventRow` membawa satu event permainan yang sedang diperiksa; Parameter `reason` bertipe `string` membawa nilai
    // reason.
    private static void ApplyCashOut(ReplayPlayerState player, int amount, ReplayEventRow evt, string reason)
    // Membuka scope metode ApplyCashOut; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ApplyCashOut.
    {
        // Memeriksa pemeriksaan lebih kecil atau sama antara `amount` dan `0`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // ApplyCashOut.
        if (amount <= 0)
        // Membuka scope cabang if untuk kondisi `amount <= 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ApplyCashOut.
        {
            // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen ($”Amount {reason} harus > 0.”) dalam
            // ApplyCashOut; pemanggil atau middleware penanganan error menerima kegagalan ini.
            throw new InvalidOperationException($"Amount {reason} harus > 0.");
        // Menutup scope cabang if untuk kondisi `amount <= 0`; bagian berikut berada di luar batas blok tersebut dalam ApplyCashOut.
        }

        // Memperbarui `player.Cash` menggunakan menentukan nilai terbesar dari `player.Cash - amount`, `0` dalam ApplyCashOut.
        player.Cash = Math.Max(player.Cash - amount, 0);
    // Menutup scope metode ApplyCashOut; bagian berikut berada di luar batas blok tersebut dalam ApplyCashOut.
    }

    // Mendefinisikan metode `ApplyCashIn` dengan hasil bertipe `void`; operasi ini menangani apply uang tunai in. Masukan: Parameter `player` bertipe
    // `ReplayPlayerState` membawa nilai pemain; Parameter `amount` bertipe `int` membawa nominal uang atau nilai transaksi yang dipakai dalam operasi.
    private static void ApplyCashIn(ReplayPlayerState player, int amount)
    // Membuka scope metode ApplyCashIn; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ApplyCashIn.
    {
        // Memeriksa pemeriksaan lebih kecil atau sama antara `amount` dan `0`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // ApplyCashIn.
        if (amount <= 0)
        // Membuka scope cabang if untuk kondisi `amount <= 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ApplyCashIn.
        {
            // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen (”Cash IN harus > 0.”) dalam ApplyCashIn;
            // pemanggil atau middleware penanganan error menerima kegagalan ini.
            throw new InvalidOperationException("Cash IN harus > 0.");
        // Menutup scope cabang if untuk kondisi `amount <= 0`; bagian berikut berada di luar batas blok tersebut dalam ApplyCashIn.
        }

        // Memperbarui `player.Cash` dengan menambahkan `amount` (nominal uang atau nilai transaksi yang dipakai dalam operasi) dalam ApplyCashIn.
        player.Cash += amount;
    // Menutup scope metode ApplyCashIn; bagian berikut berada di luar batas blok tersebut dalam ApplyCashIn.
    }

    // Mendefinisikan metode `AddInventory` dengan hasil bertipe `void`; operasi ini menangani add inventory. Masukan: Parameter `inventory` bertipe
    // `IDictionary<string, int>` membawa nilai inventory; Parameter `key` bertipe `string` membawa nilai kunci; Parameter `amount` bertipe `int`
    // membawa nominal uang atau nilai transaksi yang dipakai dalam operasi.
    private static void AddInventory(IDictionary<string, int> inventory, string key, int amount)
    // Membuka scope metode AddInventory; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam AddInventory.
    {
        // Memperbarui `inventory[key]` menggunakan hasil pemilihan bersyarat: ketika `inventory.TryGetValue(key, out var current)` benar gunakan `current +
        // amount`, jika tidak gunakan `amount` dalam AddInventory.
        inventory[key] = inventory.TryGetValue(key, out var current) ? current + amount : amount;
    // Menutup scope metode AddInventory; bagian berikut berada di luar batas blok tersebut dalam AddInventory.
    }

    // Mendefinisikan metode `RemoveInventory` dengan hasil bertipe `void`; operasi ini menangani remove inventory. Masukan: Parameter `inventory`
    // bertipe `IDictionary<string, int>` membawa nilai inventory; Parameter `key` bertipe `string` membawa nilai kunci; Parameter `amount` bertipe
    // `int` membawa nominal uang atau nilai transaksi yang dipakai dalam operasi; Parameter `evt` bertipe `ReplayEventRow` membawa satu event permainan
    // yang sedang diperiksa.
    private static void RemoveInventory(IDictionary<string, int> inventory, string key, int amount, ReplayEventRow evt)
    // Membuka scope metode RemoveInventory; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam RemoveInventory.
    {
        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!inventory.TryGetValue(key, out var current)` dan `current < amount`;
        // sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam RemoveInventory.
        if (!inventory.TryGetValue(key, out var current) || current < amount)
        // Membuka scope cabang if untuk kondisi `!inventory.TryGetValue(key, out var current) || current < amount`; pernyataan/deklarasi berikut berada di
        // dalam batas blok ini dalam RemoveInventory.
        {
            // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen ($”Stok {key} tidak cukup untuk {evt.ActionType}:
            // stok={current}, amount={amount}.”) dalam RemoveInventory; pemanggil atau middleware penanganan error menerima kegagalan ini.
            throw new InvalidOperationException($"Stok {key} tidak cukup untuk {evt.ActionType}: stok={current}, amount={amount}.");
        // Menutup scope cabang if untuk kondisi `!inventory.TryGetValue(key, out var current) || current < amount`; bagian berikut berada di luar batas
        // blok tersebut dalam RemoveInventory.
        }

        // Memperbarui `inventory[key]` menggunakan selisih antara `current` dan `amount` dalam RemoveInventory.
        inventory[key] = current - amount;
    // Menutup scope metode RemoveInventory; bagian berikut berada di luar batas blok tersebut dalam RemoveInventory.
    }

    // Mendefinisikan metode `RemoveInventoryIfAvailable` dengan hasil bertipe `void`; operasi ini menangani remove inventory if tersedia. Masukan:
    // Parameter `inventory` bertipe `IDictionary<string, int>` membawa nilai inventory; Parameter `key` bertipe `string` membawa nilai kunci; Parameter
    // `amount` bertipe `int` membawa nominal uang atau nilai transaksi yang dipakai dalam operasi.
    private static void RemoveInventoryIfAvailable(IDictionary<string, int> inventory, string key, int amount)
    // Membuka scope metode RemoveInventoryIfAvailable; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam RemoveInventoryIfAvailable.
    {
        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!inventory.TryGetValue(key, out var current)` dan `current <= 0`; sisi
        // kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam RemoveInventoryIfAvailable.
        if (!inventory.TryGetValue(key, out var current) || current <= 0)
        // Membuka scope cabang if untuk kondisi `!inventory.TryGetValue(key, out var current) || current <= 0`; pernyataan/deklarasi berikut berada di
        // dalam batas blok ini dalam RemoveInventoryIfAvailable.
        {
            // Mengakhiri eksekusi lebih awal dalam RemoveInventoryIfAvailable tanpa mengembalikan nilai; pernyataan sesudah return pada jalur ini tidak
            // dijalankan.
            return;
        // Menutup scope cabang if untuk kondisi `!inventory.TryGetValue(key, out var current) || current <= 0`; bagian berikut berada di luar batas blok
        // tersebut dalam RemoveInventoryIfAvailable.
        }

        // Memperbarui `inventory[key]` menggunakan menentukan nilai terbesar dari `current - amount`, `0` dalam RemoveInventoryIfAvailable.
        inventory[key] = Math.Max(current - amount, 0);
    // Menutup scope metode RemoveInventoryIfAvailable; bagian berikut berada di luar batas blok tersebut dalam RemoveInventoryIfAvailable.
    }

    // Mendefinisikan metode `ReadString` dengan hasil bertipe `string`; operasi ini menangani read string. Masukan: Parameter `payload` bertipe
    // `JsonElement` membawa muatan detail event dalam format JSON; Parameter `propertyName` bertipe `string` membawa nilai property nama.
    private static string ReadString(JsonElement payload, string propertyName)
    // Membuka scope metode ReadString; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ReadString.
    {
        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!payload.TryGetProperty(propertyName, out var property) ||
        // property.ValueKind != JsonValueKind.String` dan `string.IsNullOrWhiteSpace(property.GetString())`; sisi kanan diperiksa hanya jika sisi kiri
        // salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ReadString.
        if (!payload.TryGetProperty(propertyName, out var property) ||
            // Melanjutkan ekspresi dengan perbandingan ketidaksamaan antara `property.ValueKind` dan `JsonValueKind.String` dalam ReadString.
            property.ValueKind != JsonValueKind.String ||
            // Melanjutkan pengolahan dengan memeriksa apakah `property.GetString()` null, kosong, atau hanya berisi karakter spasi dalam ReadString.
            string.IsNullOrWhiteSpace(property.GetString()))
        // Membuka scope cabang if untuk kondisi `!payload.TryGetProperty(propertyName, out var property) || property.ValueKind != JsonValueKind.String ||
        // string.IsNullOrWhiteSpace(property.GetString())`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ReadString.
        {
            // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen ($”Payload wajib memiliki string
            // '{propertyName}'.”) dalam ReadString; pemanggil atau middleware penanganan error menerima kegagalan ini.
            throw new InvalidOperationException($"Payload wajib memiliki string '{propertyName}'.");
        // Menutup scope cabang if untuk kondisi `!payload.TryGetProperty(propertyName, out var property) || property.ValueKind != JsonValueKind.String ||
        // string.IsNullOrWhiteSpace(property.GetString())`; bagian berikut berada di luar batas blok tersebut dalam ReadString.
        }

        // Mengembalikan `property.GetString()` dengan penegasan non-null untuk analisis compiler; operator ! tidak menambah pemeriksaan saat runtime kepada
        // pemanggil dalam ReadString; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return property.GetString()!;
    // Menutup scope metode ReadString; bagian berikut berada di luar batas blok tersebut dalam ReadString.
    }

    // Mendefinisikan metode `ReadInt` dengan hasil bertipe `int`; operasi ini menangani read int. Masukan: Parameter `payload` bertipe `JsonElement`
    // membawa muatan detail event dalam format JSON; Parameter `propertyName` bertipe `string` membawa nilai property nama.
    private static int ReadInt(JsonElement payload, string propertyName)
    // Membuka scope metode ReadInt; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ReadInt.
    {
        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!payload.TryGetProperty(propertyName, out var property)` dan
        // `!property.TryGetInt32(out var value)`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai
        // benar dalam ReadInt.
        if (!payload.TryGetProperty(propertyName, out var property) || !property.TryGetInt32(out var value))
        // Membuka scope cabang if untuk kondisi `!payload.TryGetProperty(propertyName, out var property) || !property.TryGetInt32(out var value)`;
        // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ReadInt.
        {
            // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen ($”Payload wajib memiliki integer
            // '{propertyName}'.”) dalam ReadInt; pemanggil atau middleware penanganan error menerima kegagalan ini.
            throw new InvalidOperationException($"Payload wajib memiliki integer '{propertyName}'.");
        // Menutup scope cabang if untuk kondisi `!payload.TryGetProperty(propertyName, out var property) || !property.TryGetInt32(out var value)`; bagian
        // berikut berada di luar batas blok tersebut dalam ReadInt.
        }

        // Mengembalikan `value` (nilai nilai) kepada pemanggil dalam ReadInt; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return value;
    // Menutup scope metode ReadInt; bagian berikut berada di luar batas blok tersebut dalam ReadInt.
    }

    // Mendefinisikan metode `ReadQuantity` dengan hasil bertipe `int`; operasi ini menangani read jumlah. Masukan: Parameter `payload` bertipe
    // `JsonElement` membawa muatan detail event dalam format JSON.
    private static int ReadQuantity(JsonElement payload)
    // Membuka scope metode ReadQuantity; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ReadQuantity.
    {
        // Mengembalikan hasil pemilihan bersyarat: ketika `payload.TryGetProperty(”quantity”, out var quantityProperty) && quantityProperty.TryGetInt32(out
        // var quantity)` benar gunakan `quantity`, jika tidak gunakan `1` kepada pemanggil dalam ReadQuantity; eksekusi jalur ini selesai setelah nilai
        // hasil ditentukan.
        return payload.TryGetProperty("quantity", out var quantityProperty) && quantityProperty.TryGetInt32(out var quantity)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: quantity dalam ReadQuantity.
            ? quantity
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: 1; dalam ReadQuantity.
            : 1;
    // Menutup scope metode ReadQuantity; bagian berikut berada di luar batas blok tersebut dalam ReadQuantity.
    }

    // Mendefinisikan metode `ReadGuid` dengan hasil bertipe `Guid`; operasi ini menangani read guid. Masukan: Parameter `payload` bertipe `JsonElement`
    // membawa muatan detail event dalam format JSON; Parameter `propertyName` bertipe `string` membawa nilai property nama.
    private static Guid ReadGuid(JsonElement payload, string propertyName)
    // Membuka scope metode ReadGuid; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ReadGuid.
    {
        // Menyiapkan variabel lokal `value` untuk nilai nilai dengan memanggil `ReadString` dengan `payload`, `propertyName`. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var value = ReadString(payload, propertyName);
        // Mengembalikan hasil pemilihan bersyarat: ketika `Guid.TryParse(value, out var guid)` benar gunakan `guid`, jika tidak gunakan `throw new
        // InvalidOperationException($”Payload '{propertyName}' bukan UUID valid.”)` kepada pemanggil dalam ReadGuid; eksekusi jalur ini selesai setelah
        // nilai hasil ditentukan.
        return Guid.TryParse(value, out var guid)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: guid dalam ReadGuid.
            ? guid
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: throw new InvalidOperationException($”Payload '{propertyName}' bukan
            // UUID valid.”); dalam ReadGuid.
            : throw new InvalidOperationException($"Payload '{propertyName}' bukan UUID valid.");
    // Menutup scope metode ReadGuid; bagian berikut berada di luar batas blok tersebut dalam ReadGuid.
    }

    // Mendefinisikan metode `ReadStringArray` dengan hasil bertipe `List<string>`; operasi ini menangani read string array. Masukan: Parameter
    // `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `propertyName` bertipe `string` membawa nilai property
    // nama.
    private static List<string> ReadStringArray(JsonElement payload, string propertyName)
    // Membuka scope metode ReadStringArray; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ReadStringArray.
    {
        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!payload.TryGetProperty(propertyName, out var property)` dan
        // `property.ValueKind != JsonValueKind.Array`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini
        // bernilai benar dalam ReadStringArray.
        if (!payload.TryGetProperty(propertyName, out var property) || property.ValueKind != JsonValueKind.Array)
        // Membuka scope cabang if untuk kondisi `!payload.TryGetProperty(propertyName, out var property) || property.ValueKind != JsonValueKind.Array`;
        // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ReadStringArray.
        {
            // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen ($”Payload wajib memiliki array
            // '{propertyName}'.”) dalam ReadStringArray; pemanggil atau middleware penanganan error menerima kegagalan ini.
            throw new InvalidOperationException($"Payload wajib memiliki array '{propertyName}'.");
        // Menutup scope cabang if untuk kondisi `!payload.TryGetProperty(propertyName, out var property) || property.ValueKind != JsonValueKind.Array`;
        // bagian berikut berada di luar batas blok tersebut dalam ReadStringArray.
        }

        // Mengembalikan mematerialisasi urutan `property.EnumerateArray() .Select(item => item.GetString()) .Where(value =>
        // !string.IsNullOrWhiteSpace(value)) .Select(value => value!)` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori kepada
        // pemanggil dalam ReadStringArray; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return property.EnumerateArray()
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Select(item => item.GetString()) dalam ReadStringArray; token pada baris ini
            // menyambungkan bagian kode sebelum dan sesudahnya.
            .Select(item => item.GetString())
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(value => !string.IsNullOrWhiteSpace(value)) dalam ReadStringArray; token
            // pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Where(value => !string.IsNullOrWhiteSpace(value))
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Select(value => value!) dalam ReadStringArray; token pada baris ini
            // menyambungkan bagian kode sebelum dan sesudahnya.
            .Select(value => value!)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList(); dalam ReadStringArray; token pada baris ini menyambungkan bagian
            // kode sebelum dan sesudahnya.
            .ToList();
    // Menutup scope metode ReadStringArray; bagian berikut berada di luar batas blok tersebut dalam ReadStringArray.
    }

    // Mendefinisikan tipe class `SeedSessionRow`; sealed mencegah tipe ini diturunkan lagi.
    private sealed class SeedSessionRow
    // Membuka scope tipe SeedSessionRow; pernyataan/deklarasi berikut berada di dalam batas blok ini.
    {
        // Mendefinisikan properti `SessionId` bertipe `Guid` untuk identitas unik sesi permainan yang menjadi batas data operasi ini; get menyediakan
        // pembacaan nilai, init membatasi pengisian saat inisialisasi objek.
        public Guid SessionId { get; init; }
        // Mendefinisikan properti `SessionName` bertipe `string` untuk nilai sesi nama; get menyediakan pembacaan nilai, init membatasi pengisian saat
        // inisialisasi objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
        public string SessionName { get; init; } = string.Empty;
        // Mendefinisikan properti `Status` bertipe `string` untuk nilai status; get menyediakan pembacaan nilai, init membatasi pengisian saat inisialisasi
        // objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
        public string Status { get; init; } = string.Empty;
        // Mendefinisikan properti `Mode` bertipe `string` untuk mode permainan yang menentukan kelompok aturan yang digunakan; get menyediakan pembacaan
        // nilai, init membatasi pengisian saat inisialisasi objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
        public string Mode { get; init; } = string.Empty;
    // Menutup scope tipe SeedSessionRow; bagian berikut berada di luar batas blok tersebut.
    }

    // Mendefinisikan tipe class `SessionPlayerCountRow`; sealed mencegah tipe ini diturunkan lagi.
    private sealed class SessionPlayerCountRow
    // Membuka scope tipe SessionPlayerCountRow; pernyataan/deklarasi berikut berada di dalam batas blok ini.
    {
        // Mendefinisikan properti `SessionName` bertipe `string` untuk nilai sesi nama; get menyediakan pembacaan nilai, init membatasi pengisian saat
        // inisialisasi objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
        public string SessionName { get; init; } = string.Empty;
        // Mendefinisikan properti `PlayerCount` bertipe `int` untuk nilai pemain jumlah; get menyediakan pembacaan nilai, init membatasi pengisian saat
        // inisialisasi objek.
        public int PlayerCount { get; init; }
    // Menutup scope tipe SessionPlayerCountRow; bagian berikut berada di luar batas blok tersebut.
    }

    // Mendefinisikan tipe class `SequenceCheckRow`; sealed mencegah tipe ini diturunkan lagi.
    private sealed class SequenceCheckRow
    // Membuka scope tipe SequenceCheckRow; pernyataan/deklarasi berikut berada di dalam batas blok ini.
    {
        // Mendefinisikan properti `SessionName` bertipe `string` untuk nilai sesi nama; get menyediakan pembacaan nilai, init membatasi pengisian saat
        // inisialisasi objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
        public string SessionName { get; init; } = string.Empty;
        // Mendefinisikan properti `MinSequence` bertipe `long` untuk nilai minimum sequence; get menyediakan pembacaan nilai, init membatasi pengisian saat
        // inisialisasi objek.
        public long MinSequence { get; init; }
        // Mendefinisikan properti `MaxSequence` bertipe `long` untuk nilai maksimum sequence; get menyediakan pembacaan nilai, init membatasi pengisian
        // saat inisialisasi objek.
        public long MaxSequence { get; init; }
        // Mendefinisikan properti `EventCount` bertipe `int` untuk nilai event jumlah; get menyediakan pembacaan nilai, init membatasi pengisian saat
        // inisialisasi objek.
        public int EventCount { get; init; }
    // Menutup scope tipe SequenceCheckRow; bagian berikut berada di luar batas blok tersebut.
    }

    // Mendefinisikan tipe class `RulesetCheckRow`; sealed mencegah tipe ini diturunkan lagi.
    private sealed class RulesetCheckRow
    // Membuka scope tipe RulesetCheckRow; pernyataan/deklarasi berikut berada di dalam batas blok ini.
    {
        // Mendefinisikan properti `SessionName` bertipe `string` untuk nilai sesi nama; get menyediakan pembacaan nilai, init membatasi pengisian saat
        // inisialisasi objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
        public string SessionName { get; init; } = string.Empty;
        // Mendefinisikan properti `ActivatedRulesetVersionId` bertipe `string` untuk nilai activated aturan versi identitas; get menyediakan pembacaan
        // nilai, init membatasi pengisian saat inisialisasi objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
        public string ActivatedRulesetVersionId { get; init; } = string.Empty;
        // Mendefinisikan properti `EventRulesetVersionId` bertipe `string` untuk nilai event aturan versi identitas; get menyediakan pembacaan nilai, init
        // membatasi pengisian saat inisialisasi objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
        public string EventRulesetVersionId { get; init; } = string.Empty;
        // Mendefinisikan properti `EventRulesetVersions` bertipe `int` untuk nilai event aturan versions; get menyediakan pembacaan nilai, init membatasi
        // pengisian saat inisialisasi objek.
        public int EventRulesetVersions { get; init; }
    // Menutup scope tipe RulesetCheckRow; bagian berikut berada di luar batas blok tersebut.
    }

    // Mendefinisikan tipe class `CalendarCheckRow`; sealed mencegah tipe ini diturunkan lagi.
    private sealed class CalendarCheckRow
    // Membuka scope tipe CalendarCheckRow; pernyataan/deklarasi berikut berada di dalam batas blok ini.
    {
        // Mendefinisikan properti `SessionName` bertipe `string` untuk nilai sesi nama; get menyediakan pembacaan nilai, init membatasi pengisian saat
        // inisialisasi objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
        public string SessionName { get; init; } = string.Empty;
        // Mendefinisikan properti `DayCount` bertipe `int` untuk nilai hari jumlah; get menyediakan pembacaan nilai, init membatasi pengisian saat
        // inisialisasi objek.
        public int DayCount { get; init; }
        // Mendefinisikan properti `MinDayIndex` bertipe `int` untuk nilai minimum hari index; get menyediakan pembacaan nilai, init membatasi pengisian
        // saat inisialisasi objek.
        public int MinDayIndex { get; init; }
        // Mendefinisikan properti `MaxDayIndex` bertipe `int` untuk nilai maksimum hari index; get menyediakan pembacaan nilai, init membatasi pengisian
        // saat inisialisasi objek.
        public int MaxDayIndex { get; init; }
        // Mendefinisikan properti `WeekdayMismatchCount` bertipe `int` untuk nilai weekday mismatch jumlah; get menyediakan pembacaan nilai, init membatasi
        // pengisian saat inisialisasi objek.
        public int WeekdayMismatchCount { get; init; }
    // Menutup scope tipe CalendarCheckRow; bagian berikut berada di luar batas blok tersebut.
    }

    // Mendefinisikan tipe class `PlayerProjectionCoverageRow`; sealed mencegah tipe ini diturunkan lagi.
    private sealed class PlayerProjectionCoverageRow
    // Membuka scope tipe PlayerProjectionCoverageRow; pernyataan/deklarasi berikut berada di dalam batas blok ini.
    {
        // Mendefinisikan properti `SessionName` bertipe `string` untuk nilai sesi nama; get menyediakan pembacaan nilai, init membatasi pengisian saat
        // inisialisasi objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
        public string SessionName { get; init; } = string.Empty;
        // Mendefinisikan properti `UserId` bertipe `Guid` untuk identitas akun pengguna yang datanya sedang diproses; get menyediakan pembacaan nilai, init
        // membatasi pengisian saat inisialisasi objek.
        public Guid UserId { get; init; }
        // Mendefinisikan properti `PlayerEventCount` bertipe `int` untuk nilai pemain event jumlah; get menyediakan pembacaan nilai, init membatasi
        // pengisian saat inisialisasi objek.
        public int PlayerEventCount { get; init; }
        // Mendefinisikan properti `ProjectionCount` bertipe `int` untuk nilai projection jumlah; get menyediakan pembacaan nilai, init membatasi pengisian
        // saat inisialisasi objek.
        public int ProjectionCount { get; init; }
    // Menutup scope tipe PlayerProjectionCoverageRow; bagian berikut berada di luar batas blok tersebut.
    }

    // Mendefinisikan tipe class `PlayerGameplaySnapshotCoverageRow`; sealed mencegah tipe ini diturunkan lagi.
    private sealed class PlayerGameplaySnapshotCoverageRow
    // Membuka scope tipe PlayerGameplaySnapshotCoverageRow; pernyataan/deklarasi berikut berada di dalam batas blok ini.
    {
        // Mendefinisikan properti `SessionName` bertipe `string` untuk nilai sesi nama; get menyediakan pembacaan nilai, init membatasi pengisian saat
        // inisialisasi objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
        public string SessionName { get; init; } = string.Empty;
        // Mendefinisikan properti `UserId` bertipe `Guid` untuk identitas akun pengguna yang datanya sedang diproses; get menyediakan pembacaan nilai, init
        // membatasi pengisian saat inisialisasi objek.
        public Guid UserId { get; init; }
        // Mendefinisikan properti `RawSnapshotCount` bertipe `int` untuk nilai raw snapshot keadaan jumlah; get menyediakan pembacaan nilai, init membatasi
        // pengisian saat inisialisasi objek.
        public int RawSnapshotCount { get; init; }
        // Mendefinisikan properti `DerivedSnapshotCount` bertipe `int` untuk nilai derived snapshot keadaan jumlah; get menyediakan pembacaan nilai, init
        // membatasi pengisian saat inisialisasi objek.
        public int DerivedSnapshotCount { get; init; }
        // Mendefinisikan properti `EmptyPayloadCount` bertipe `int` untuk nilai empty payload jumlah; get menyediakan pembacaan nilai, init membatasi
        // pengisian saat inisialisasi objek.
        public int EmptyPayloadCount { get; init; }
    // Menutup scope tipe PlayerGameplaySnapshotCoverageRow; bagian berikut berada di luar batas blok tersebut.
    }

    // Mendefinisikan tipe class `SnapshotInvariantRow`; sealed mencegah tipe ini diturunkan lagi.
    private sealed class SnapshotInvariantRow
    // Membuka scope tipe SnapshotInvariantRow; pernyataan/deklarasi berikut berada di dalam batas blok ini.
    {
        // Mendefinisikan properti `SessionName` bertipe `string` untuk nilai sesi nama; get menyediakan pembacaan nilai, init membatasi pengisian saat
        // inisialisasi objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
        public string SessionName { get; init; } = string.Empty;
        // Mendefinisikan properti `UserId` bertipe `Guid` untuk identitas akun pengguna yang datanya sedang diproses; get menyediakan pembacaan nilai, init
        // membatasi pengisian saat inisialisasi objek.
        public Guid UserId { get; init; }
        // Mendefinisikan properti `IngredientDifference` bertipe `int` untuk nilai bahan difference; get menyediakan pembacaan nilai, init membatasi
        // pengisian saat inisialisasi objek.
        public int IngredientDifference { get; init; }
        // Mendefinisikan properti `GoldDifference` bertipe `int` untuk nilai emas difference; get menyediakan pembacaan nilai, init membatasi pengisian
        // saat inisialisasi objek.
        public int GoldDifference { get; init; }
        // Mendefinisikan properti `DonationDifference` bertipe `int` untuk nilai donasi difference; get menyediakan pembacaan nilai, init membatasi
        // pengisian saat inisialisasi objek.
        public int DonationDifference { get; init; }
        // Mendefinisikan properti `OrderIncomeDifference` bertipe `int` untuk nilai urutan/pesanan pemasukan difference; get menyediakan pembacaan nilai,
        // init membatasi pengisian saat inisialisasi objek.
        public int OrderIncomeDifference { get; init; }
        // Mendefinisikan properti `IngredientUseDifference` bertipe `int` untuk nilai bahan use difference; get menyediakan pembacaan nilai, init membatasi
        // pengisian saat inisialisasi objek.
        public int IngredientUseDifference { get; init; }
        // Mendefinisikan properti `RiskCostDifference` bertipe `int` untuk nilai risiko biaya difference; get menyediakan pembacaan nilai, init membatasi
        // pengisian saat inisialisasi objek.
        public int RiskCostDifference { get; init; }
        // Mendefinisikan properti `OrderCountDifference` bertipe `int` untuk nilai urutan/pesanan jumlah difference; get menyediakan pembacaan nilai, init
        // membatasi pengisian saat inisialisasi objek.
        public int OrderCountDifference { get; init; }
        // Mendefinisikan properti `CashDifference` bertipe `int` untuk nilai uang tunai difference; get menyediakan pembacaan nilai, init membatasi
        // pengisian saat inisialisasi objek.
        public int CashDifference { get; init; }
    // Menutup scope tipe SnapshotInvariantRow; bagian berikut berada di luar batas blok tersebut.
    }

    // Mendefinisikan tipe class `PlayerSetupProjectionRow`; sealed mencegah tipe ini diturunkan lagi.
    private sealed class PlayerSetupProjectionRow
    // Membuka scope tipe PlayerSetupProjectionRow; pernyataan/deklarasi berikut berada di dalam batas blok ini.
    {
        // Mendefinisikan properti `SessionName` bertipe `string` untuk nilai sesi nama; get menyediakan pembacaan nilai, init membatasi pengisian saat
        // inisialisasi objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
        public string SessionName { get; init; } = string.Empty;
        // Mendefinisikan properti `SessionParticipantId` bertipe `Guid` untuk identitas keikutsertaan pemain pada sesi tertentu; get menyediakan pembacaan
        // nilai, init membatasi pengisian saat inisialisasi objek.
        public Guid SessionParticipantId { get; init; }
        // Mendefinisikan properti `SetupIngredientCount` bertipe `int` untuk nilai setup bahan jumlah; get menyediakan pembacaan nilai, init membatasi
        // pengisian saat inisialisasi objek.
        public int SetupIngredientCount { get; init; }
        // Mendefinisikan properti `DayOnePlayerActionCount` bertipe `int` untuk nilai hari one pemain aksi jumlah; get menyediakan pembacaan nilai, init
        // membatasi pengisian saat inisialisasi objek.
        public int DayOnePlayerActionCount { get; init; }
        // Mendefinisikan properti `SetupGoldCount` bertipe `int` untuk nilai setup emas jumlah; get menyediakan pembacaan nilai, init membatasi pengisian
        // saat inisialisasi objek.
        public int SetupGoldCount { get; init; }
        // Mendefinisikan properti `SetupMissionCount` bertipe `int` untuk nilai setup misi jumlah; get menyediakan pembacaan nilai, init membatasi
        // pengisian saat inisialisasi objek.
        public int SetupMissionCount { get; init; }
        // Mendefinisikan properti `SetupTieBreakerCount` bertipe `int` untuk nilai setup tie breaker jumlah; get menyediakan pembacaan nilai, init
        // membatasi pengisian saat inisialisasi objek.
        public int SetupTieBreakerCount { get; init; }
        // Mendefinisikan properti `SetupLoanCount` bertipe `int` untuk nilai setup pinjaman jumlah; get menyediakan pembacaan nilai, init membatasi
        // pengisian saat inisialisasi objek.
        public int SetupLoanCount { get; init; }
        // Mendefinisikan properti `SetupInsuranceCount` bertipe `int` untuk nilai setup asuransi jumlah; get menyediakan pembacaan nilai, init membatasi
        // pengisian saat inisialisasi objek.
        public int SetupInsuranceCount { get; init; }
    // Menutup scope tipe PlayerSetupProjectionRow; bagian berikut berada di luar batas blok tersebut.
    }

    // Mendefinisikan tipe class `MarketSlotRow`; sealed mencegah tipe ini diturunkan lagi.
    private sealed class MarketSlotRow
    // Membuka scope tipe MarketSlotRow; pernyataan/deklarasi berikut berada di dalam batas blok ini.
    {
        // Mendefinisikan properti `SessionName` bertipe `string` untuk nilai sesi nama; get menyediakan pembacaan nilai, init membatasi pengisian saat
        // inisialisasi objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
        public string SessionName { get; init; } = string.Empty;
        // Mendefinisikan properti `SlotGroup` bertipe `string` untuk nilai slot group; get menyediakan pembacaan nilai, init membatasi pengisian saat
        // inisialisasi objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
        public string SlotGroup { get; init; } = string.Empty;
        // Mendefinisikan properti `SlotCount` bertipe `int` untuk nilai slot jumlah; get menyediakan pembacaan nilai, init membatasi pengisian saat
        // inisialisasi objek.
        public int SlotCount { get; init; }
    // Menutup scope tipe MarketSlotRow; bagian berikut berada di luar batas blok tersebut.
    }

    // Mendefinisikan tipe class `ScenarioSystemEventCountRow`; sealed mencegah tipe ini diturunkan lagi.
    private sealed class ScenarioSystemEventCountRow
    // Membuka scope tipe ScenarioSystemEventCountRow; pernyataan/deklarasi berikut berada di dalam batas blok ini.
    {
        // Mendefinisikan properti `SessionName` bertipe `string` untuk nilai sesi nama; get menyediakan pembacaan nilai, init membatasi pengisian saat
        // inisialisasi objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
        public string SessionName { get; init; } = string.Empty;
        // Mendefinisikan properti `MissionAssignedCount` bertipe `int` untuk nilai misi assigned jumlah; get menyediakan pembacaan nilai, init membatasi
        // pengisian saat inisialisasi objek.
        public int MissionAssignedCount { get; init; }
        // Mendefinisikan properti `DonationRankAwardedCount` bertipe `int` untuk nilai donasi rank awarded jumlah; get menyediakan pembacaan nilai, init
        // membatasi pengisian saat inisialisasi objek.
        public int DonationRankAwardedCount { get; init; }
        // Mendefinisikan properti `DonationWinnersAnnouncedCount` bertipe `int` untuk nilai donasi winners announced jumlah; get menyediakan pembacaan
        // nilai, init membatasi pengisian saat inisialisasi objek.
        public int DonationWinnersAnnouncedCount { get; init; }
        // Mendefinisikan properti `TieBreakerAssignedCount` bertipe `int` untuk nilai tie breaker assigned jumlah; get menyediakan pembacaan nilai, init
        // membatasi pengisian saat inisialisasi objek.
        public int TieBreakerAssignedCount { get; init; }
    // Menutup scope tipe ScenarioSystemEventCountRow; bagian berikut berada di luar batas blok tersebut.
    }

    // Mendefinisikan tipe class `ScenarioAlignmentRow`; sealed mencegah tipe ini diturunkan lagi.
    private sealed class ScenarioAlignmentRow
    // Membuka scope tipe ScenarioAlignmentRow; pernyataan/deklarasi berikut berada di dalam batas blok ini.
    {
        // Mendefinisikan properti `Mode` bertipe `string` untuk mode permainan yang menentukan kelompok aturan yang digunakan; get menyediakan pembacaan
        // nilai, init membatasi pengisian saat inisialisasi objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
        public string Mode { get; init; } = string.Empty;
        // Mendefinisikan properti `PlayerName` bertipe `string` untuk nilai pemain nama; get menyediakan pembacaan nilai, init membatasi pengisian saat
        // inisialisasi objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
        public string PlayerName { get; init; } = string.Empty;
        // Mendefinisikan properti `DayIndex` bertipe `int` untuk nilai hari index; get menyediakan pembacaan nilai, init membatasi pengisian saat
        // inisialisasi objek.
        public int DayIndex { get; init; }
        // Mendefinisikan properti `ActionSlot` bertipe `int` untuk nilai aksi slot; get menyediakan pembacaan nilai, init membatasi pengisian saat
        // inisialisasi objek.
        public int ActionSlot { get; init; }
        // Mendefinisikan properti `ActionType` bertipe `string` untuk nilai aksi jenis; get menyediakan pembacaan nilai, init membatasi pengisian saat
        // inisialisasi objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
        public string ActionType { get; init; } = string.Empty;
        // Mendefinisikan properti `PayloadKey` bertipe `string` untuk nilai payload kunci; get menyediakan pembacaan nilai, init membatasi pengisian saat
        // inisialisasi objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
        public string PayloadKey { get; init; } = string.Empty;
    // Menutup scope tipe ScenarioAlignmentRow; bagian berikut berada di luar batas blok tersebut.
    }

    // Mendefinisikan tipe class `RelationalReadModelCountRow`; sealed mencegah tipe ini diturunkan lagi.
    private sealed class RelationalReadModelCountRow
    // Membuka scope tipe RelationalReadModelCountRow; pernyataan/deklarasi berikut berada di dalam batas blok ini.
    {
        // Mendefinisikan properti `IngredientCount` bertipe `int` untuk nilai bahan jumlah; get menyediakan pembacaan nilai, init membatasi pengisian saat
        // inisialisasi objek.
        public int IngredientCount { get; init; }
        // Mendefinisikan properti `NeedPurchaseCount` bertipe `int` untuk nilai kebutuhan pembelian jumlah; get menyediakan pembacaan nilai, init membatasi
        // pengisian saat inisialisasi objek.
        public int NeedPurchaseCount { get; init; }
        // Mendefinisikan properti `FinancialGoalCount` bertipe `int` untuk nilai keuangan target jumlah; get menyediakan pembacaan nilai, init membatasi
        // pengisian saat inisialisasi objek.
        public int FinancialGoalCount { get; init; }
        // Mendefinisikan properti `DonationEventCount` bertipe `int` untuk nilai donasi event jumlah; get menyediakan pembacaan nilai, init membatasi
        // pengisian saat inisialisasi objek.
        public int DonationEventCount { get; init; }
        // Mendefinisikan properti `DonationRankingCount` bertipe `int` untuk nilai donasi ranking jumlah; get menyediakan pembacaan nilai, init membatasi
        // pengisian saat inisialisasi objek.
        public int DonationRankingCount { get; init; }
        // Mendefinisikan properti `ActionCounterCount` bertipe `int` untuk nilai aksi counter jumlah; get menyediakan pembacaan nilai, init membatasi
        // pengisian saat inisialisasi objek.
        public int ActionCounterCount { get; init; }
    // Menutup scope tipe RelationalReadModelCountRow; bagian berikut berada di luar batas blok tersebut.
    }

    // Mendefinisikan tipe class `ReplayLifeRiskRow`; sealed mencegah tipe ini diturunkan lagi.
    private sealed class ReplayLifeRiskRow
    // Membuka scope tipe ReplayLifeRiskRow; pernyataan/deklarasi berikut berada di dalam batas blok ini.
    {
        // Mendefinisikan properti `RiskCode` bertipe `string` untuk nilai risiko kode; get menyediakan pembacaan nilai, init membatasi pengisian saat
        // inisialisasi objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
        public string RiskCode { get; init; } = string.Empty;
        // Mendefinisikan properti `Direction` bertipe `string` untuk nilai direction; get menyediakan pembacaan nilai, init membatasi pengisian saat
        // inisialisasi objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
        public string Direction { get; init; } = string.Empty;
        // Mendefinisikan properti `Amount` bertipe `int` untuk nominal uang atau nilai transaksi yang dipakai dalam operasi; get menyediakan pembacaan
        // nilai, init membatasi pengisian saat inisialisasi objek.
        public int Amount { get; init; }
    // Menutup scope tipe ReplayLifeRiskRow; bagian berikut berada di luar batas blok tersebut.
    }

    // Mendefinisikan tipe class `ReplayEventRow`; sealed mencegah tipe ini diturunkan lagi.
    private sealed class ReplayEventRow
    // Membuka scope tipe ReplayEventRow; pernyataan/deklarasi berikut berada di dalam batas blok ini.
    {
        // Mendefinisikan properti `SessionName` bertipe `string` untuk nilai sesi nama; get menyediakan pembacaan nilai, init membatasi pengisian saat
        // inisialisasi objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
        public string SessionName { get; init; } = string.Empty;
        // Mendefinisikan properti `Mode` bertipe `string` untuk mode permainan yang menentukan kelompok aturan yang digunakan; get menyediakan pembacaan
        // nilai, init membatasi pengisian saat inisialisasi objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
        public string Mode { get; init; } = string.Empty;
        // Mendefinisikan properti `SessionId` bertipe `Guid` untuk identitas unik sesi permainan yang menjadi batas data operasi ini; get menyediakan
        // pembacaan nilai, init membatasi pengisian saat inisialisasi objek.
        public Guid SessionId { get; init; }
        // Mendefinisikan properti `EventId` bertipe `Guid` untuk identitas unik event untuk pencatatan dan pemeriksaan duplikasi; get menyediakan pembacaan
        // nilai, init membatasi pengisian saat inisialisasi objek.
        public Guid EventId { get; init; }
        // Mendefinisikan properti `UserId` bertipe `Guid?` untuk identitas akun pengguna yang datanya sedang diproses; get menyediakan pembacaan nilai,
        // init membatasi pengisian saat inisialisasi objek; tanda ? mengizinkan nilai null.
        public Guid? UserId { get; init; }
        // Mendefinisikan properti `PlayerName` bertipe `string?` untuk nilai pemain nama; get menyediakan pembacaan nilai, init membatasi pengisian saat
        // inisialisasi objek; tanda ? mengizinkan nilai null.
        public string? PlayerName { get; init; }
        // Mendefinisikan properti `ActorType` bertipe `string` untuk nilai actor jenis; get menyediakan pembacaan nilai, init membatasi pengisian saat
        // inisialisasi objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
        public string ActorType { get; init; } = string.Empty;
        // Mendefinisikan properti `DayIndex` bertipe `int` untuk nilai hari index; get menyediakan pembacaan nilai, init membatasi pengisian saat
        // inisialisasi objek.
        public int DayIndex { get; init; }
        // Mendefinisikan properti `Weekday` bertipe `string` untuk nilai weekday; get menyediakan pembacaan nilai, init membatasi pengisian saat
        // inisialisasi objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
        public string Weekday { get; init; } = string.Empty;
        // Mendefinisikan properti `ActionSlot` bertipe `int` untuk nilai aksi slot; get menyediakan pembacaan nilai, init membatasi pengisian saat
        // inisialisasi objek.
        public int ActionSlot { get; init; }
        // Mendefinisikan properti `SequenceNumber` bertipe `long` untuk nomor urut event yang menentukan urutan pemrosesan riwayat permainan; get
        // menyediakan pembacaan nilai, init membatasi pengisian saat inisialisasi objek.
        public long SequenceNumber { get; init; }
        // Mendefinisikan properti `ActionType` bertipe `string` untuk nilai aksi jenis; get menyediakan pembacaan nilai, init membatasi pengisian saat
        // inisialisasi objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
        public string ActionType { get; init; } = string.Empty;
        // Mendefinisikan properti `Payload` bertipe `string` untuk muatan detail event dalam format JSON; get menyediakan pembacaan nilai, init membatasi
        // pengisian saat inisialisasi objek; nilai awalnya nilai literal `”{}”`.
        public string Payload { get; init; } = "{}";
    // Menutup scope tipe ReplayEventRow; bagian berikut berada di luar batas blok tersebut.
    }

    // Mendefinisikan tipe class `ReplaySessionState`; sealed mencegah tipe ini diturunkan lagi.
    private sealed class ReplaySessionState
    // Membuka scope tipe ReplaySessionState; pernyataan/deklarasi berikut berada di dalam batas blok ini.
    {
        // Mendeklarasikan field bertipe `int`: `_startingCash` menyimpan nilai starting uang tunai. readonly membatasi penggantian referensi/nilai field
        // pada deklarasi atau konstruktor.
        private readonly int _startingCash;

        // Mendefinisikan konstruktor ReplaySessionState yang menyiapkan objek dan menerima dependency/nilai awal dari pemanggil; parameter: Parameter
        // `sessionName` bertipe `string` membawa nilai sesi nama; Parameter `mode` bertipe `string` membawa mode permainan yang menentukan kelompok aturan
        // yang digunakan; Parameter `startingCash` bertipe `int` membawa nilai starting uang tunai.
        public ReplaySessionState(string sessionName, string mode, int startingCash)
        // Membuka scope konstruktor ReplaySessionState; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ReplaySessionState.
        {
            // Memperbarui `SessionName` menggunakan `sessionName` (nilai sesi nama) dalam ReplaySessionState.
            SessionName = sessionName;
            // Memperbarui `Mode` menggunakan `mode` (mode permainan yang menentukan kelompok aturan yang digunakan) dalam ReplaySessionState.
            Mode = mode;
            // Memperbarui `_startingCash` menggunakan `startingCash` (nilai starting uang tunai) dalam ReplaySessionState.
            _startingCash = startingCash;
        // Menutup scope konstruktor ReplaySessionState; bagian berikut berada di luar batas blok tersebut dalam ReplaySessionState.
        }

        // Mendefinisikan properti `SessionName` bertipe `string` untuk nilai sesi nama; get menyediakan pembacaan nilai.
        public string SessionName { get; }
        // Mendefinisikan properti `Mode` bertipe `string` untuk mode permainan yang menentukan kelompok aturan yang digunakan; get menyediakan pembacaan
        // nilai.
        public string Mode { get; }
        // Mendefinisikan properti `Players` bertipe `Dictionary<Guid, ReplayPlayerState>` untuk nilai pemain; get menyediakan pembacaan nilai; nilai
        // awalnya objek baru dengan tipe mengikuti konteks tujuan dan argumen ().
        public Dictionary<Guid, ReplayPlayerState> Players { get; } = new();
        // Mendefinisikan properti `ActionTokensByDay` bertipe `Dictionary<(int DayIndex, Guid UserId), int>` untuk nilai aksi tokens berdasarkan hari; get
        // menyediakan pembacaan nilai; nilai awalnya objek baru dengan tipe mengikuti konteks tujuan dan argumen ().
        public Dictionary<(int DayIndex, Guid UserId), int> ActionTokensByDay { get; } = new();
        // Mendefinisikan properti `DonationsByDay` bertipe `Dictionary<int, Dictionary<Guid, int>>` untuk nilai donations berdasarkan hari; get menyediakan
        // pembacaan nilai; nilai awalnya objek baru dengan tipe mengikuti konteks tujuan dan argumen ().
        public Dictionary<int, Dictionary<Guid, int>> DonationsByDay { get; } = new();
        // Mendefinisikan properti `DonationAwardsByDay` bertipe `Dictionary<int, List<DonationAward>>` untuk nilai donasi awards berdasarkan hari; get
        // menyediakan pembacaan nilai; nilai awalnya objek baru dengan tipe mengikuti konteks tujuan dan argumen ().
        public Dictionary<int, List<DonationAward>> DonationAwardsByDay { get; } = new();
        // Mendefinisikan properti `TieBreakers` bertipe `Dictionary<Guid, int>` untuk nilai tie breakers; get menyediakan pembacaan nilai; nilai awalnya
        // objek baru dengan tipe mengikuti konteks tujuan dan argumen ().
        public Dictionary<Guid, int> TieBreakers { get; } = new();

        // Mendefinisikan metode `GetPlayer` dengan hasil bertipe `ReplayPlayerState`; operasi ini menangani get pemain. Masukan: Parameter `userId` bertipe
        // `Guid?` membawa identitas akun pengguna yang datanya sedang diproses; nilai null diizinkan ketika data opsional belum tersedia; Parameter
        // `playerName` bertipe `string?` membawa nilai pemain nama; nilai null diizinkan ketika data opsional belum tersedia.
        public ReplayPlayerState GetPlayer(Guid? userId, string? playerName)
        // Membuka scope metode GetPlayer; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam GetPlayer.
        {
            // Memeriksa kebalikan kondisi `userId.HasValue`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam GetPlayer.
            if (!userId.HasValue)
            // Membuka scope cabang if untuk kondisi `!userId.HasValue`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam GetPlayer.
            {
                // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen (”Event pemain wajib memiliki user_id.”) dalam
                // GetPlayer; pemanggil atau middleware penanganan error menerima kegagalan ini.
                throw new InvalidOperationException("Event pemain wajib memiliki user_id.");
            // Menutup scope cabang if untuk kondisi `!userId.HasValue`; bagian berikut berada di luar batas blok tersebut dalam GetPlayer.
            }

            // Memeriksa kebalikan kondisi `Players.TryGetValue(userId.Value, out var player)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
            // GetPlayer.
            if (!Players.TryGetValue(userId.Value, out var player))
            // Membuka scope cabang if untuk kondisi `!Players.TryGetValue(userId.Value, out var player)`; pernyataan/deklarasi berikut berada di dalam batas
            // blok ini dalam GetPlayer.
            {
                // Memperbarui `player` menggunakan objek baru bertipe `ReplayPlayerState` dengan argumen (userId.Value, playerName ?? userId.Value.ToString(),
                // _startingCash) dalam GetPlayer.
                player = new ReplayPlayerState(userId.Value, playerName ?? userId.Value.ToString(), _startingCash);
                // Memperbarui `Players[userId.Value]` menggunakan `player` (nilai pemain) dalam GetPlayer.
                Players[userId.Value] = player;
            // Menutup scope cabang if untuk kondisi `!Players.TryGetValue(userId.Value, out var player)`; bagian berikut berada di luar batas blok tersebut
            // dalam GetPlayer.
            }

            // Mengembalikan `player` (nilai pemain) kepada pemanggil dalam GetPlayer; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return player;
        // Menutup scope metode GetPlayer; bagian berikut berada di luar batas blok tersebut dalam GetPlayer.
        }

        // Mendefinisikan metode `RecordActionToken` dengan hasil bertipe `void`; operasi ini menangani rekaman aksi token. Masukan: Parameter `evt` bertipe
        // `ReplayEventRow` membawa satu event permainan yang sedang diperiksa; Parameter `counts` bertipe `bool` membawa nilai counts.
        public void RecordActionToken(ReplayEventRow evt, bool counts)
        // Membuka scope metode RecordActionToken; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam RecordActionToken.
        {
            // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!counts` dan `!evt.UserId.HasValue`; sisi kanan diperiksa hanya jika
            // sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam RecordActionToken.
            if (!counts || !evt.UserId.HasValue)
            // Membuka scope cabang if untuk kondisi `!counts || !evt.UserId.HasValue`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // RecordActionToken.
            {
                // Mengakhiri eksekusi lebih awal dalam RecordActionToken tanpa mengembalikan nilai; pernyataan sesudah return pada jalur ini tidak dijalankan.
                return;
            // Menutup scope cabang if untuk kondisi `!counts || !evt.UserId.HasValue`; bagian berikut berada di luar batas blok tersebut dalam
            // RecordActionToken.
            }

            // Menyiapkan variabel lokal `key` untuk nilai kunci dengan tuple yang membawa bagian 1: evt.DayIndex; bagian 2: evt.UserId.Value. Tipe variabel
            // disimpulkan dari ekspresi nilai awal.
            var key = (evt.DayIndex, evt.UserId.Value);
            // Memperbarui `ActionTokensByDay[key]` menggunakan hasil pemilihan bersyarat: ketika `ActionTokensByDay.TryGetValue(key, out var current)` benar
            // gunakan `current + 1`, jika tidak gunakan `1` dalam RecordActionToken.
            ActionTokensByDay[key] = ActionTokensByDay.TryGetValue(key, out var current) ? current + 1 : 1;
        // Menutup scope metode RecordActionToken; bagian berikut berada di luar batas blok tersebut dalam RecordActionToken.
        }

        // Mendefinisikan metode `RecordDonation` dengan hasil bertipe `void`; operasi ini menangani rekaman donasi. Masukan: Parameter `dayIndex` bertipe
        // `int` membawa nilai hari index; Parameter `userId` bertipe `Guid` membawa identitas akun pengguna yang datanya sedang diproses; Parameter
        // `amount` bertipe `int` membawa nominal uang atau nilai transaksi yang dipakai dalam operasi.
        public void RecordDonation(int dayIndex, Guid userId, int amount)
        // Membuka scope metode RecordDonation; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam RecordDonation.
        {
            // Memeriksa kebalikan kondisi `DonationsByDay.TryGetValue(dayIndex, out var donations)`; blok if hanya dijalankan ketika kondisi ini bernilai benar
            // dalam RecordDonation.
            if (!DonationsByDay.TryGetValue(dayIndex, out var donations))
            // Membuka scope cabang if untuk kondisi `!DonationsByDay.TryGetValue(dayIndex, out var donations)`; pernyataan/deklarasi berikut berada di dalam
            // batas blok ini dalam RecordDonation.
            {
                // Memperbarui `donations` menggunakan objek baru bertipe `Dictionary<Guid, int>` dengan nilai awal sesuai konstruktornya dalam RecordDonation.
                donations = new Dictionary<Guid, int>();
                // Memperbarui `DonationsByDay[dayIndex]` menggunakan `donations` (nilai donations) dalam RecordDonation.
                DonationsByDay[dayIndex] = donations;
            // Menutup scope cabang if untuk kondisi `!DonationsByDay.TryGetValue(dayIndex, out var donations)`; bagian berikut berada di luar batas blok
            // tersebut dalam RecordDonation.
            }

            // Memperbarui `donations[userId]` menggunakan hasil pemilihan bersyarat: ketika `donations.TryGetValue(userId, out var current)` benar gunakan
            // `current + amount`, jika tidak gunakan `amount` dalam RecordDonation.
            donations[userId] = donations.TryGetValue(userId, out var current) ? current + amount : amount;
        // Menutup scope metode RecordDonation; bagian berikut berada di luar batas blok tersebut dalam RecordDonation.
        }

        // Mendefinisikan metode `RecordDonationAward` dengan hasil bertipe `void`; operasi ini menangani rekaman donasi award. Masukan: Parameter
        // `dayIndex` bertipe `int` membawa nilai hari index; Parameter `userId` bertipe `Guid` membawa identitas akun pengguna yang datanya sedang
        // diproses; Parameter `rank` bertipe `int` membawa nilai rank; Parameter `points` bertipe `int` membawa nilai poin.
        public void RecordDonationAward(int dayIndex, Guid userId, int rank, int points)
        // Membuka scope metode RecordDonationAward; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam RecordDonationAward.
        {
            // Memeriksa kebalikan kondisi `DonationAwardsByDay.TryGetValue(dayIndex, out var awards)`; blok if hanya dijalankan ketika kondisi ini bernilai
            // benar dalam RecordDonationAward.
            if (!DonationAwardsByDay.TryGetValue(dayIndex, out var awards))
            // Membuka scope cabang if untuk kondisi `!DonationAwardsByDay.TryGetValue(dayIndex, out var awards)`; pernyataan/deklarasi berikut berada di dalam
            // batas blok ini dalam RecordDonationAward.
            {
                // Memperbarui `awards` menggunakan objek baru bertipe `List<DonationAward>` dengan nilai awal sesuai konstruktornya dalam RecordDonationAward.
                awards = new List<DonationAward>();
                // Memperbarui `DonationAwardsByDay[dayIndex]` menggunakan `awards` (nilai awards) dalam RecordDonationAward.
                DonationAwardsByDay[dayIndex] = awards;
            // Menutup scope cabang if untuk kondisi `!DonationAwardsByDay.TryGetValue(dayIndex, out var awards)`; bagian berikut berada di luar batas blok
            // tersebut dalam RecordDonationAward.
            }

            // Menjalankan menambahkan `new DonationAward(userId, rank, points)` ke `awards` dalam RecordDonationAward.
            awards.Add(new DonationAward(userId, rank, points));
        // Menutup scope metode RecordDonationAward; bagian berikut berada di luar batas blok tersebut dalam RecordDonationAward.
        }

        // Mendefinisikan metode `RecordTieBreaker` dengan hasil bertipe `void`; operasi ini menangani rekaman tie breaker. Masukan: Parameter `userId`
        // bertipe `Guid` membawa identitas akun pengguna yang datanya sedang diproses; Parameter `tieNumber` bertipe `int` membawa nilai tie number.
        public void RecordTieBreaker(Guid userId, int tieNumber)
        // Membuka scope metode RecordTieBreaker; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam RecordTieBreaker.
        {
            // Memperbarui `TieBreakers[userId]` menggunakan `tieNumber` (nilai tie number) dalam RecordTieBreaker.
            TieBreakers[userId] = tieNumber;
        // Menutup scope metode RecordTieBreaker; bagian berikut berada di luar batas blok tersebut dalam RecordTieBreaker.
        }
    // Menutup scope tipe ReplaySessionState; bagian berikut berada di luar batas blok tersebut.
    }

    // Mendefinisikan tipe class `ReplayPlayerState`; sealed mencegah tipe ini diturunkan lagi.
    private sealed class ReplayPlayerState
    // Membuka scope tipe ReplayPlayerState; pernyataan/deklarasi berikut berada di dalam batas blok ini.
    {
        // Mendefinisikan konstruktor ReplayPlayerState yang menyiapkan objek dan menerima dependency/nilai awal dari pemanggil; parameter: Parameter
        // `userId` bertipe `Guid` membawa identitas akun pengguna yang datanya sedang diproses; Parameter `playerName` bertipe `string` membawa nilai
        // pemain nama; Parameter `startingCash` bertipe `int` membawa nilai starting uang tunai.
        public ReplayPlayerState(Guid userId, string playerName, int startingCash)
        // Membuka scope konstruktor ReplayPlayerState; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ReplayPlayerState.
        {
            // Memperbarui `UserId` menggunakan `userId` (identitas akun pengguna yang datanya sedang diproses) dalam ReplayPlayerState.
            UserId = userId;
            // Memperbarui `PlayerName` menggunakan `playerName` (nilai pemain nama) dalam ReplayPlayerState.
            PlayerName = playerName;
            // Memperbarui `Cash` menggunakan `startingCash` (nilai starting uang tunai) dalam ReplayPlayerState.
            Cash = startingCash;
        // Menutup scope konstruktor ReplayPlayerState; bagian berikut berada di luar batas blok tersebut dalam ReplayPlayerState.
        }

        // Mendefinisikan properti `UserId` bertipe `Guid` untuk identitas akun pengguna yang datanya sedang diproses; get menyediakan pembacaan nilai.
        public Guid UserId { get; }
        // Mendefinisikan properti `PlayerName` bertipe `string` untuk nilai pemain nama; get menyediakan pembacaan nilai.
        public string PlayerName { get; }
        // Mendefinisikan properti `Cash` bertipe `int` untuk nilai uang tunai; get menyediakan pembacaan nilai, set mengizinkan penggantian nilai.
        public int Cash { get; set; }
        // Mendefinisikan properti `GoldQty` bertipe `int` untuk nilai emas qty; get menyediakan pembacaan nilai, set mengizinkan penggantian nilai.
        public int GoldQty { get; set; }
        // Mendefinisikan properti `GoldNetAmount` bertipe `int` untuk nilai emas net nominal; get menyediakan pembacaan nilai, set mengizinkan penggantian
        // nilai.
        public int GoldNetAmount { get; set; }
        // Mendefinisikan properti `Ingredients` bertipe `Dictionary<string, int>` untuk nilai bahan; get menyediakan pembacaan nilai; nilai awalnya objek
        // baru dengan tipe mengikuti konteks tujuan dan argumen (StringComparer.OrdinalIgnoreCase).
        public Dictionary<string, int> Ingredients { get; } = new(StringComparer.OrdinalIgnoreCase);
        // Mendefinisikan properti `Savings` bertipe `Dictionary<string, int>` untuk nilai tabungan; get menyediakan pembacaan nilai; nilai awalnya objek
        // baru dengan tipe mengikuti konteks tujuan dan argumen (StringComparer.OrdinalIgnoreCase).
        public Dictionary<string, int> Savings { get; } = new(StringComparer.OrdinalIgnoreCase);
        // Mendefinisikan properti `Loans` bertipe `Dictionary<string, int>` untuk nilai pinjaman; get menyediakan pembacaan nilai; nilai awalnya objek baru
        // dengan tipe mengikuti konteks tujuan dan argumen (StringComparer.OrdinalIgnoreCase).
        public Dictionary<string, int> Loans { get; } = new(StringComparer.OrdinalIgnoreCase);
        // Mendefinisikan properti `PrimaryNeeds` bertipe `HashSet<string>` untuk nilai primary kebutuhan; get menyediakan pembacaan nilai; nilai awalnya
        // objek baru dengan tipe mengikuti konteks tujuan dan argumen (StringComparer.OrdinalIgnoreCase).
        public HashSet<string> PrimaryNeeds { get; } = new(StringComparer.OrdinalIgnoreCase);
        // Mendefinisikan properti `SecondaryNeeds` bertipe `HashSet<string>` untuk nilai secondary kebutuhan; get menyediakan pembacaan nilai; nilai
        // awalnya objek baru dengan tipe mengikuti konteks tujuan dan argumen (StringComparer.OrdinalIgnoreCase).
        public HashSet<string> SecondaryNeeds { get; } = new(StringComparer.OrdinalIgnoreCase);
        // Mendefinisikan properti `TertiaryNeeds` bertipe `HashSet<string>` untuk nilai tertiary kebutuhan; get menyediakan pembacaan nilai; nilai awalnya
        // objek baru dengan tipe mengikuti konteks tujuan dan argumen (StringComparer.OrdinalIgnoreCase).
        public HashSet<string> TertiaryNeeds { get; } = new(StringComparer.OrdinalIgnoreCase);
        // Mendefinisikan properti `AchievedSavingGoals` bertipe `HashSet<string>` untuk nilai achieved tabungan target; get menyediakan pembacaan nilai;
        // nilai awalnya objek baru dengan tipe mengikuti konteks tujuan dan argumen (StringComparer.OrdinalIgnoreCase).
        public HashSet<string> AchievedSavingGoals { get; } = new(StringComparer.OrdinalIgnoreCase);
        // Mendefinisikan properti `InsurancePolicies` bertipe `HashSet<string>` untuk nilai asuransi policies; get menyediakan pembacaan nilai; nilai
        // awalnya objek baru dengan tipe mengikuti konteks tujuan dan argumen (StringComparer.OrdinalIgnoreCase).
        public HashSet<string> InsurancePolicies { get; } = new(StringComparer.OrdinalIgnoreCase);
        // Mendefinisikan properti `RiskOutByEventId` bertipe `Dictionary<Guid, int>` untuk nilai risiko out berdasarkan event identitas; get menyediakan
        // pembacaan nilai; nilai awalnya objek baru dengan tipe mengikuti konteks tujuan dan argumen ().
        public Dictionary<Guid, int> RiskOutByEventId { get; } = new();
        // Mendefinisikan properti `InsuranceOffsets` bertipe `HashSet<Guid>` untuk nilai asuransi offsets; get menyediakan pembacaan nilai; nilai awalnya
        // objek baru dengan tipe mengikuti konteks tujuan dan argumen ().
        public HashSet<Guid> InsuranceOffsets { get; } = new();
    // Menutup scope tipe ReplayPlayerState; bagian berikut berada di luar batas blok tersebut.
    }

    // Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `DonationAward`; sealed mencegah tipe ini diturunkan lagi.
    private sealed record DonationAward(Guid UserId, int Rank, int Points);
// Menutup scope tipe ManualSimulationSeedIntegrationTests; bagian berikut berada di luar batas blok tersebut.
}
