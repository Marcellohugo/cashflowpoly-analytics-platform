// Fungsi file: Memverifikasi target beban rilis dengan 100 akun, 20 sesi aktif, dan 20 klien bersamaan.
// Mengimpor namespace `System.Collections.Concurrent` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Collections.Concurrent;
// Mengimpor namespace `System.Diagnostics` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Diagnostics;
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
// Mengimpor namespace `Dapper` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Dapper;
// Mengimpor namespace `Microsoft.AspNetCore.Mvc.Testing` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.AspNetCore.Mvc.Testing;
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
// menerapkan metadata `Trait(”Category”, ”Performance”)` pada deklarasi berikut agar framework/compiler dapat mengenali pengaturannya.
[Trait("Category", "Performance")]
// Mendefinisikan tipe class `ReleasePerformanceIntegrationTests`; sealed mencegah tipe ini diturunkan lagi.
public sealed class ReleasePerformanceIntegrationTests
// Membuka scope tipe ReleasePerformanceIntegrationTests; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendeklarasikan field bertipe `string`: `JwtSigningKey` menyimpan nilai jwt signing kunci dengan nilai awal nilai literal
    // `”release-performance-signing-key-minimum-32-chars”`.
    private const string JwtSigningKey = "release-performance-signing-key-minimum-32-chars";
    // Mendeklarasikan field bertipe `string`: `SeedPassword` menyimpan nilai seed password dengan nilai awal nilai literal `”SeedLocal!2026”`.
    private const string SeedPassword = "SeedLocal!2026";
    // Mendeklarasikan field bertipe `int`: `EventsPerSession` menyimpan nilai event per sesi dengan nilai awal nilai literal `2_000`.
    private const int EventsPerSession = 2_000;
    // Mendeklarasikan field bertipe `Guid`: `ReleaseRulesetVersionId` menyimpan nilai release aturan versi identitas dengan nilai awal memanggil
    // `Guid.Parse` dengan `”f5b4c67b-0825-4970-9f07-3b68e8fcb524”`. readonly membatasi penggantian referensi/nilai field pada deklarasi atau
    // konstruktor. static membuat field menjadi milik tipe dan dibagikan antar instance.
    private static readonly Guid ReleaseRulesetVersionId = Guid.Parse("f5b4c67b-0825-4970-9f07-3b68e8fcb524");
    // Mendeklarasikan field bertipe `ITestOutputHelper`: `_output` menyimpan nilai output. readonly membatasi penggantian referensi/nilai field pada
    // deklarasi atau konstruktor.
    private readonly ITestOutputHelper _output;

    // Mendefinisikan konstruktor ReleasePerformanceIntegrationTests yang menyiapkan objek dan menerima dependency/nilai awal dari pemanggil; parameter:
    // Parameter `output` bertipe `ITestOutputHelper` membawa nilai output.
    public ReleasePerformanceIntegrationTests(ITestOutputHelper output)
    // Membuka scope konstruktor ReleasePerformanceIntegrationTests; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // ReleasePerformanceIntegrationTests.
    {
        // Memperbarui `_output` menggunakan `output` (nilai output) dalam ReleasePerformanceIntegrationTests.
        _output = output;
    // Menutup scope konstruktor ReleasePerformanceIntegrationTests; bagian berikut berada di luar batas blok tersebut dalam
    // ReleasePerformanceIntegrationTests.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `ReleaseDataset_MeetsConcurrentP95TargetsAtTwoThousandEventsPerSession` dengan hasil bertipe `Task`; operasi ini menangani
    // release dataset meets concurrent 95 targets at two thousand event per sesi. async memungkinkan metode menunggu operasi I/O dengan await dan
    // mengembalikan penyelesaian melalui Task.
    public async Task ReleaseDataset_MeetsConcurrentP95TargetsAtTwoThousandEventsPerSession()
    // Membuka scope metode ReleaseDataset_MeetsConcurrentP95TargetsAtTwoThousandEventsPerSession; pernyataan/deklarasi berikut berada di dalam batas
    // blok ini dalam ReleaseDataset_MeetsConcurrentP95TargetsAtTwoThousandEventsPerSession.
    {
        // Menyiapkan variabel lokal `database` untuk nilai database dengan memanggil `new PostgreSqlBuilder(”postgres:16”)
        // .WithDatabase(”cashflowpoly_release_performance”) .WithUsername(”cashflowpoly”) .WithPassword(”cashflowpoly”) .Build` dengan tanpa argumen. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var database = new PostgreSqlBuilder("postgres:16")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .WithDatabase(”cashflowpoly_release_performance”) dalam
            // ReleaseDataset_MeetsConcurrentP95TargetsAtTwoThousandEventsPerSession; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .WithDatabase("cashflowpoly_release_performance")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .WithUsername(”cashflowpoly”) dalam
            // ReleaseDataset_MeetsConcurrentP95TargetsAtTwoThousandEventsPerSession; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .WithUsername("cashflowpoly")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .WithPassword(”cashflowpoly”) dalam
            // ReleaseDataset_MeetsConcurrentP95TargetsAtTwoThousandEventsPerSession; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .WithPassword("cashflowpoly")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Build(); dalam
            // ReleaseDataset_MeetsConcurrentP95TargetsAtTwoThousandEventsPerSession; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Build();
        // Menjalankan hasil operasi asinkron memanggil `database.StartAsync` dengan tanpa argumen; await menunggu hasil tanpa memblokir thread selama
        // operasi belum selesai dalam ReleaseDataset_MeetsConcurrentP95TargetsAtTwoThousandEventsPerSession.
        await database.StartAsync();

        // Menyiapkan variabel lokal `previousConnection` untuk nilai previous connection dengan memanggil `Environment.GetEnvironmentVariable` dengan
        // `”ConnectionStrings__Default”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var previousConnection = Environment.GetEnvironmentVariable("ConnectionStrings__Default");
        // Menyiapkan variabel lokal `previousSigningKey` untuk nilai previous signing kunci dengan memanggil `Environment.GetEnvironmentVariable` dengan
        // `”JWT_SIGNING_KEY”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var previousSigningKey = Environment.GetEnvironmentVariable("JWT_SIGNING_KEY");
        // Menjalankan memanggil `Environment.SetEnvironmentVariable` dengan `”ConnectionStrings__Default”`, `database.GetConnectionString()` dalam
        // ReleaseDataset_MeetsConcurrentP95TargetsAtTwoThousandEventsPerSession.
        Environment.SetEnvironmentVariable("ConnectionStrings__Default", database.GetConnectionString());
        // Menjalankan memanggil `Environment.SetEnvironmentVariable` dengan `”JWT_SIGNING_KEY”`, `JwtSigningKey` dalam
        // ReleaseDataset_MeetsConcurrentP95TargetsAtTwoThousandEventsPerSession.
        Environment.SetEnvironmentVariable("JWT_SIGNING_KEY", JwtSigningKey);

        // Memulai blok try dalam ReleaseDataset_MeetsConcurrentP95TargetsAtTwoThousandEventsPerSession; exception dari blok ini dapat dialihkan ke catch,
        // sedangkan finally (jika ada) tetap dijalankan saat keluar.
        try
        // Membuka scope penanganan operasi try; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ReleaseDataset_MeetsConcurrentP95TargetsAtTwoThousandEventsPerSession.
        {
            // Menyiapkan variabel lokal `factory` untuk nilai factory dengan objek baru bertipe `ApiWebApplicationFactory` dengan argumen
            // (database.GetConnectionString(), JwtSigningKey, seedSimulation: true). Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan
            // sumber daya dilepas otomatis saat scope berakhir.
            await using var factory = new ApiWebApplicationFactory(database.GetConnectionString(), JwtSigningKey, seedSimulation: true);
            // Menyiapkan variabel lokal `bootstrapClient` untuk nilai bootstrap client dengan memanggil `factory.CreateClient` dengan `new
            // WebApplicationFactoryClientOptions { AllowAutoRedirect = false }`. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber
            // daya dilepas otomatis saat scope berakhir.
            using var bootstrapClient = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
            // Menjalankan pemeriksaan bahwa `(await bootstrapClient.GetAsync(”/health/ready”)).IsSuccessStatusCode` bernilai benar; pengujian gagal jika
            // kondisi tidak terpenuhi dalam ReleaseDataset_MeetsConcurrentP95TargetsAtTwoThousandEventsPerSession.
            Assert.True((await bootstrapClient.GetAsync("/health/ready")).IsSuccessStatusCode);

            // Menyiapkan variabel lokal `performanceScopes` untuk nilai performa scopes dengan hasil operasi asinkron memanggil `PrepareReleaseDatasetAsync`
            // dengan `database.GetConnectionString()`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari
            // ekspresi nilai awal.
            var performanceScopes = await PrepareReleaseDatasetAsync(database.GetConnectionString());
            // Menyiapkan variabel lokal `tokens` untuk nilai tokens dengan hasil operasi asinkron memanggil `LoginConcurrentUsersAsync` dengan `factory`; await
            // menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var tokens = await LoginConcurrentUsersAsync(factory);
            // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`20`, `tokens.Count`); pengujian gagal jika
            // keduanya berbeda dalam ReleaseDataset_MeetsConcurrentP95TargetsAtTwoThousandEventsPerSession.
            Assert.Equal(20, tokens.Count);
            // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`20`, `performanceScopes.Count`); pengujian
            // gagal jika keduanya berbeda dalam ReleaseDataset_MeetsConcurrentP95TargetsAtTwoThousandEventsPerSession.
            Assert.Equal(20, performanceScopes.Count);
            // Membatasi masa pakai `var accessClient = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false })` pada blok
            // using; sumber daya dilepas ketika blok berakhir melalui Dispose.
            using (var accessClient = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false }))
            // Membuka scope scope pemakaian sumber daya using; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ReleaseDataset_MeetsConcurrentP95TargetsAtTwoThousandEventsPerSession.
            {
                // Memperbarui `accessClient.DefaultRequestHeaders.Authorization` menggunakan objek baru bertipe `AuthenticationHeaderValue` dengan argumen
                // (”Bearer”, tokens[0]) dalam ReleaseDataset_MeetsConcurrentP95TargetsAtTwoThousandEventsPerSession.
                accessClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokens[0]);
                // Menyiapkan variabel lokal `sessionList` untuk nilai sesi daftar dengan hasil operasi asinkron memanggil
                // `accessClient.GetFromJsonAsync<SessionListResponse>` dengan `”/api/v1/sessions”`; await menunggu hasil tanpa memblokir thread selama operasi
                // belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
                var sessionList = await accessClient.GetFromJsonAsync<SessionListResponse>("/api/v1/sessions");
                // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
                // `Assert.IsType<SessionListResponse>(sessionList).Items`, `item => item.SessionId == performanceScopes[0].SessionId` dalam
                // ReleaseDataset_MeetsConcurrentP95TargetsAtTwoThousandEventsPerSession.
                Assert.Contains(Assert.IsType<SessionListResponse>(sessionList).Items, item => item.SessionId == performanceScopes[0].SessionId);
                // Menyiapkan variabel lokal `analyticsAccess` untuk nilai analytics akses dengan hasil operasi asinkron memanggil `accessClient.GetAsync` dengan
                // `$”/api/v1/analytics/sessions/{performanceScopes[0].SessionId}”`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
                // variabel disimpulkan dari ekspresi nilai awal.
                var analyticsAccess = await accessClient.GetAsync(
                    // Meneruskan teks interpolasi `$”/api/v1/analytics/sessions/{performanceScopes[0].SessionId}”`; nilai ekspresi di dalam kurung kurawal disisipkan
                    // saat program berjalan sebagai argumen ke `accessClient.GetAsync`; Meneruskan nilai literal `0` sebagai argumen ke `accessClient.GetAsync`.
                    $"/api/v1/analytics/sessions/{performanceScopes[0].SessionId}");
                // Menjalankan pemeriksaan bahwa `analyticsAccess.IsSuccessStatusCode`, `$”Preflight analitik gagal: {(int)analyticsAccess.StatusCode} {await
                // analyticsAccess.Content.ReadAsStringAsync()}”` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
                // ReleaseDataset_MeetsConcurrentP95TargetsAtTwoThousandEventsPerSession.
                Assert.True(
                    // Meneruskan `analyticsAccess.IsSuccessStatusCode` (nilai berstatus success status kode) sebagai argumen ke `Assert.True`.
                    analyticsAccess.IsSuccessStatusCode,
                    // Meneruskan teks interpolasi `$”Preflight analitik gagal: {(int)analyticsAccess.StatusCode} {await analyticsAccess.Content.ReadAsStringAsync()}”`;
                    // nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan sebagai argumen ke `Assert.True`.
                    $"Preflight analitik gagal: {(int)analyticsAccess.StatusCode} {await analyticsAccess.Content.ReadAsStringAsync()}");
                // Menyiapkan variabel lokal `analytics` untuk nilai analytics dengan pemeriksaan hasil dengan `Assert.IsType<AnalyticsSessionResponse>` menggunakan
                // `await analyticsAccess.Content.ReadFromJsonAsync<AnalyticsSessionResponse>()`; ketidaksesuaian dengan ekspektasi membuat pengujian gagal. Tipe
                // variabel disimpulkan dari ekspresi nilai awal.
                var analytics = Assert.IsType<AnalyticsSessionResponse>(
                    // Meneruskan hasil operasi asinkron membaca tanpa argumen menjadi objek bertipe sesuai kontrak JSON melalui
                    // `analyticsAccess.Content.ReadFromJsonAsync<AnalyticsSessionResponse>`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai
                    // sebagai argumen ke `Assert.IsType<AnalyticsSessionResponse>`.
                    await analyticsAccess.Content.ReadFromJsonAsync<AnalyticsSessionResponse>());
                // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`EventsPerSession`,
                // `analytics.Summary.EventCount`); pengujian gagal jika keduanya berbeda dalam
                // ReleaseDataset_MeetsConcurrentP95TargetsAtTwoThousandEventsPerSession.
                Assert.Equal(EventsPerSession, analytics.Summary.EventCount);
                // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`1_000d`, `analytics.Summary.CashInTotal`);
                // pengujian gagal jika keduanya berbeda dalam ReleaseDataset_MeetsConcurrentP95TargetsAtTwoThousandEventsPerSession.
                Assert.Equal(1_000d, analytics.Summary.CashInTotal);
                // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`1_000d`, `analytics.Summary.CashOutTotal`);
                // pengujian gagal jika keduanya berbeda dalam ReleaseDataset_MeetsConcurrentP95TargetsAtTwoThousandEventsPerSession.
                Assert.Equal(1_000d, analytics.Summary.CashOutTotal);
                // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`2`, `analytics.ByPlayer.Count`); pengujian
                // gagal jika keduanya berbeda dalam ReleaseDataset_MeetsConcurrentP95TargetsAtTwoThousandEventsPerSession.
                Assert.Equal(2, analytics.ByPlayer.Count);
                // Menjalankan pemeriksaan hasil dengan `Assert.All` menggunakan `analytics.ByPlayer`, `player => { Assert.Equal(500d, player.CashInTotal);
                // Assert.Equal(500d, player.CashOutTotal); Assert.Equal(16, player.InventoryIngredientTotal); }`; ketidaksesuaian dengan ekspektasi membuat
                // pengujian gagal dalam ReleaseDataset_MeetsConcurrentP95TargetsAtTwoThousandEventsPerSession.
                Assert.All(analytics.ByPlayer, player =>
                // Membuka scope fungsi lambda yang dipasok ke `Assert.All`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // ReleaseDataset_MeetsConcurrentP95TargetsAtTwoThousandEventsPerSession.
                {
                    // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`500d`, `player.CashInTotal`); pengujian gagal
                    // jika keduanya berbeda dalam ReleaseDataset_MeetsConcurrentP95TargetsAtTwoThousandEventsPerSession.
                    Assert.Equal(500d, player.CashInTotal);
                    // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`500d`, `player.CashOutTotal`); pengujian gagal
                    // jika keduanya berbeda dalam ReleaseDataset_MeetsConcurrentP95TargetsAtTwoThousandEventsPerSession.
                    Assert.Equal(500d, player.CashOutTotal);
                    // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`16`, `player.InventoryIngredientTotal`);
                    // pengujian gagal jika keduanya berbeda dalam ReleaseDataset_MeetsConcurrentP95TargetsAtTwoThousandEventsPerSession.
                    Assert.Equal(16, player.InventoryIngredientTotal);
                // Menutup scope fungsi lambda yang dipasok ke `Assert.All`; bagian berikut berada di luar batas blok tersebut dalam
                // ReleaseDataset_MeetsConcurrentP95TargetsAtTwoThousandEventsPerSession.
                });
            // Menutup scope scope pemakaian sumber daya using; bagian berikut berada di luar batas blok tersebut dalam
            // ReleaseDataset_MeetsConcurrentP95TargetsAtTwoThousandEventsPerSession.
            }

            // Menyiapkan variabel lokal `analyticsP95` untuk nilai analytics 95 dengan hasil operasi asinkron memanggil `MeasureP95Async` dengan `factory`,
            // `tokens`, `performanceScopes .Select(scope => $”/api/v1/analytics/sessions/{scope.SessionId}”) .ToArray()`, `10`; await menunggu hasil tanpa
            // memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var analyticsP95 = await MeasureP95Async(
                // Meneruskan `factory` (nilai factory) sebagai argumen ke `MeasureP95Async`.
                factory,
                // Meneruskan `tokens` (nilai tokens) sebagai argumen ke `MeasureP95Async`.
                tokens,
                // Meneruskan mematerialisasi urutan `performanceScopes .Select(scope => $”/api/v1/analytics/sessions/{scope.SessionId}”)` menjadi array dengan
                // elemen hasil saat ini sebagai argumen ke `MeasureP95Async`.
                performanceScopes
                    // Meneruskan fungsi lambda `scope => $”/api/v1/analytics/sessions/{scope.SessionId}”` yang dijalankan oleh operasi pemanggil untuk memproses setiap
                    // masukan sebagai argumen ke `performanceScopes .Select`.
                    .Select(scope => $"/api/v1/analytics/sessions/{scope.SessionId}")
                    // Meneruskan mematerialisasi urutan `performanceScopes .Select(scope => $”/api/v1/analytics/sessions/{scope.SessionId}”)` menjadi array dengan
                    // elemen hasil saat ini sebagai argumen ke `MeasureP95Async`.
                    .ToArray(),
                // Meneruskan nilai literal `10` sebagai argumen bernama `requestsPerUser`.
                requestsPerUser: 10);
            // Menyiapkan variabel lokal `eventIngestionP95` untuk nilai event ingestion 95 dengan hasil operasi asinkron memanggil
            // `MeasureEventIngestionP95Async` dengan `factory`, `tokens`, `performanceScopes`, `10`; await menunggu hasil tanpa memblokir thread selama operasi
            // belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var eventIngestionP95 = await MeasureEventIngestionP95Async(
                // Meneruskan `factory` (nilai factory) sebagai argumen ke `MeasureEventIngestionP95Async`.
                factory,
                // Meneruskan `tokens` (nilai tokens) sebagai argumen ke `MeasureEventIngestionP95Async`.
                tokens,
                // Meneruskan `performanceScopes` (nilai performa scopes) sebagai argumen ke `MeasureEventIngestionP95Async`.
                performanceScopes,
                // Meneruskan nilai literal `10` sebagai argumen bernama `requestsPerUser`.
                requestsPerUser: 10);

            // Menjalankan memanggil `_output.WriteLine` dengan `”Beban: {0} sesi x {1} event beserta proyeksi (64 aksi pemain, 1936 transaksi sistem terkait
            // pemain); 20 klien x 10 permintaan per endpoint. POST CatatTransaksi dengan pemain....`, `performanceScopes.Count`, `EventsPerSession`,
            // `eventIngestionP95`, `analyticsP95` dalam ReleaseDataset_MeetsConcurrentP95TargetsAtTwoThousandEventsPerSession.
            _output.WriteLine(
                // Meneruskan nilai literal `”Beban: {0} sesi x {1} event beserta proyeksi (64 aksi pemain, 1936 transaksi sistem terkait pemain); 20 klien x 10
                // permintaan per endpoint. POST CatatTransaksi dengan pemain....` sebagai argumen ke `_output.WriteLine`.
                "Beban: {0} sesi x {1} event beserta proyeksi (64 aksi pemain, 1936 transaksi sistem terkait pemain); 20 klien x 10 permintaan per endpoint. POST CatatTransaksi dengan pemain. POST /api/v1/events P95={2:F1} ms; GET /api/v1/analytics/sessions/{{sessionId}} P95={3:F1} ms.",
                // Meneruskan `performanceScopes.Count`, yaitu jumlah elemen atau panjang data sebagai argumen ke `_output.WriteLine`.
                performanceScopes.Count,
                // Meneruskan `EventsPerSession` (nilai event per sesi) sebagai argumen ke `_output.WriteLine`.
                EventsPerSession,
                // Meneruskan `eventIngestionP95` (nilai event ingestion 95) sebagai argumen ke `_output.WriteLine`.
                eventIngestionP95,
                // Meneruskan `analyticsP95` (nilai analytics 95) sebagai argumen ke `_output.WriteLine`.
                analyticsP95);

            // Menjalankan pemeriksaan bahwa `eventIngestionP95 <= 500`, `$”POST /api/v1/events p95 {eventIngestionP95:F1} ms, target <= 500 ms.”` bernilai
            // benar; pengujian gagal jika kondisi tidak terpenuhi dalam ReleaseDataset_MeetsConcurrentP95TargetsAtTwoThousandEventsPerSession.
            Assert.True(eventIngestionP95 <= 500, $"POST /api/v1/events p95 {eventIngestionP95:F1} ms, target <= 500 ms.");
            // Menjalankan pemeriksaan bahwa `analyticsP95 <= 1_500`, `$”GET /api/v1/analytics/sessions/{{sessionId}} p95 {analyticsP95:F1} ms, target <= 1.500
            // ms.”` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam ReleaseDataset_MeetsConcurrentP95TargetsAtTwoThousandEventsPerSession.
            Assert.True(analyticsP95 <= 1_500, $"GET /api/v1/analytics/sessions/{{sessionId}} p95 {analyticsP95:F1} ms, target <= 1.500 ms.");
        // Menutup scope penanganan operasi try; bagian berikut berada di luar batas blok tersebut dalam
        // ReleaseDataset_MeetsConcurrentP95TargetsAtTwoThousandEventsPerSession.
        }
        // Menjalankan blok finally ketika alur meninggalkan try/catch, termasuk saat terjadi exception dalam
        // ReleaseDataset_MeetsConcurrentP95TargetsAtTwoThousandEventsPerSession; bagian ini dipakai untuk pekerjaan penutup yang harus tetap dilakukan.
        finally
        // Membuka scope pekerjaan penutup finally; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ReleaseDataset_MeetsConcurrentP95TargetsAtTwoThousandEventsPerSession.
        {
            // Menjalankan memanggil `Environment.SetEnvironmentVariable` dengan `”ConnectionStrings__Default”`, `previousConnection` dalam
            // ReleaseDataset_MeetsConcurrentP95TargetsAtTwoThousandEventsPerSession.
            Environment.SetEnvironmentVariable("ConnectionStrings__Default", previousConnection);
            // Menjalankan memanggil `Environment.SetEnvironmentVariable` dengan `”JWT_SIGNING_KEY”`, `previousSigningKey` dalam
            // ReleaseDataset_MeetsConcurrentP95TargetsAtTwoThousandEventsPerSession.
            Environment.SetEnvironmentVariable("JWT_SIGNING_KEY", previousSigningKey);
        // Menutup scope pekerjaan penutup finally; bagian berikut berada di luar batas blok tersebut dalam
        // ReleaseDataset_MeetsConcurrentP95TargetsAtTwoThousandEventsPerSession.
        }
    // Menutup scope metode ReleaseDataset_MeetsConcurrentP95TargetsAtTwoThousandEventsPerSession; bagian berikut berada di luar batas blok tersebut
    // dalam ReleaseDataset_MeetsConcurrentP95TargetsAtTwoThousandEventsPerSession.
    }

    // Mendefinisikan metode `PrepareReleaseDatasetAsync` dengan hasil bertipe `Task<IReadOnlyList<PerformanceScope>>`; operasi ini menangani prepare
    // release dataset asinkron. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan:
    // Parameter `connectionString` bertipe `string` membawa nilai connection string.
    private static async Task<IReadOnlyList<PerformanceScope>> PrepareReleaseDatasetAsync(string connectionString)
    // Membuka scope metode PrepareReleaseDatasetAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam PrepareReleaseDatasetAsync.
    {
        // Menyiapkan variabel lokal `connection` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan objek baru bertipe
        // `NpgsqlConnection` dengan argumen (connectionString). Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas
        // otomatis saat scope berakhir.
        await using var connection = new NpgsqlConnection(connectionString);
        // Menjalankan hasil operasi asinkron memanggil `connection.OpenAsync` dengan tanpa argumen; await menunggu hasil tanpa memblokir thread selama
        // operasi belum selesai dalam PrepareReleaseDatasetAsync.
        await connection.OpenAsync();
        // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `connection` menggunakan `””” insert into app_users (user_id, username,
        // display_name, password_hash, role, is_active, is_demo) select gen_random_uuid(), 'perf_user_' || lpad(number::text, 3, '0'), 'Per...`, `180`;
        // nilai hasil menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam
        // PrepareReleaseDatasetAsync.
        await connection.ExecuteAsync(
            // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke `connection.ExecuteAsync`.
            // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
            // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 2: INSERT INTO menetapkan tabel dan kolom tujuan penambahan rekaman: `insert into app_users (user_id, username, display_name,
            // password_hash, role, is_active, is_demo)`.
            // Baris literal 3: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
            // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `gen_random_uuid(),`.
            // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `'perf_user_' ||
            // lpad(number::text, 3, '0'),`.
            // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `'Performance User ' ||
            // number,`.
            // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `source.password_hash,`.
            // Baris literal 8: Menyusun pilihan nilai bersyarat di dalam ekspresi SQL: `case when number <= 20 then 'INSTRUCTOR' else 'PLAYER' end,`.
            // Baris literal 9: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `true,`.
            // Baris literal 10: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `true`.
            // Baris literal 11: FROM memilih tabel/subquery sumber pembacaan: `from generate_series(1, 95) as number`.
            // Baris literal 12: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `cross join lateral (`.
            // Baris literal 13: SELECT menentukan nilai atau kolom yang dikembalikan query: `select password_hash`.
            // Baris literal 14: FROM memilih tabel/subquery sumber pembacaan: `from app_users`.
            // Baris literal 15: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where lower(username::text) = 'rina.kartika'`.
            // Baris literal 16: LIMIT membatasi jumlah baris yang dikembalikan query: `limit 1`.
            // Baris literal 17: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `) source`.
            // Baris literal 18: ON CONFLICT menentukan penanganan saat INSERT bertabrakan dengan kunci unik yang sudah ada: `on conflict (username) do
            // nothing;`.
            // Baris literal 19: Pembatas literal/penutup ``; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 20: CREATE mendefinisikan objek basis data yang diperlukan proses ini: `create temporary table performance_session_scope (`.
            // Baris literal 21: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `number int primary key,`.
            // Baris literal 22: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `session_id uuid not null
            // unique`.
            // Baris literal 23: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `) on commit drop;`.
            // Baris literal 24: Pembatas literal/penutup ``; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 25: INSERT INTO menetapkan tabel dan kolom tujuan penambahan rekaman: `insert into performance_session_scope (number, session_id)`.
            // Baris literal 26: SELECT menentukan nilai atau kolom yang dikembalikan query: `select number, gen_random_uuid()`.
            // Baris literal 27: FROM memilih tabel/subquery sumber pembacaan: `from generate_series(1, 20) as number;`.
            // Baris literal 28: Pembatas literal/penutup ``; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 29: INSERT INTO menetapkan tabel dan kolom tujuan penambahan rekaman: `insert into sessions (`.
            // Baris literal 30: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `session_id,`.
            // Baris literal 31: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `session_name,`.
            // Baris literal 32: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_version_id,`.
            // Baris literal 33: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `mode,`.
            // Baris literal 34: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `status,`.
            // Baris literal 35: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `player_count,`.
            // Baris literal 36: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `started_at,`.
            // Baris literal 37: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `instructor_user_id`.
            // Baris literal 38: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 39: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
            // Baris literal 40: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `scope.session_id,`.
            // Baris literal 41: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `'Performance Active Session
            // ' || scope.number,`.
            // Baris literal 42: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun:
            // `'f5b4c67b-0825-4970-9f07-3b68e8fcb524'::uuid,`.
            // Baris literal 43: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `'PEMULA',`.
            // Baris literal 44: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `'CREATED',`.
            // Baris literal 45: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `0,`.
            // Baris literal 46: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `null,`.
            // Baris literal 47: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `(`.
            // Baris literal 48: SELECT menentukan nilai atau kolom yang dikembalikan query: `select user_id`.
            // Baris literal 49: FROM memilih tabel/subquery sumber pembacaan: `from app_users`.
            // Baris literal 50: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where lower(username::text) = 'perf_user_' ||
            // lpad(scope.number::text, 3, '0')`.
            // Baris literal 51: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 52: FROM memilih tabel/subquery sumber pembacaan: `from performance_session_scope scope;`.
            // Baris literal 53: Pembatas literal/penutup ``; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 54: INSERT INTO menetapkan tabel dan kolom tujuan penambahan rekaman: `insert into session_participants (`.
            // Baris literal 55: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `session_id,`.
            // Baris literal 56: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `user_id,`.
            // Baris literal 57: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `player_order_no,`.
            // Baris literal 58: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `player_name`.
            // Baris literal 59: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 60: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
            // Baris literal 61: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `scope.session_id,`.
            // Baris literal 62: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `participant.user_id,`.
            // Baris literal 63: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun:
            // `participant.player_order_no,`.
            // Baris literal 64: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `participant.display_name`.
            // Baris literal 65: FROM memilih tabel/subquery sumber pembacaan: `from performance_session_scope scope`.
            // Baris literal 66: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `cross join lateral (`.
            // Baris literal 67: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
            // Baris literal 68: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `user_id,`.
            // Baris literal 69: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `display_name,`.
            // Baris literal 70: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `row_number() over (order by
            // username)::int as player_order_no`.
            // Baris literal 71: FROM memilih tabel/subquery sumber pembacaan: `from app_users`.
            // Baris literal 72: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where lower(username::text) in (`.
            // Baris literal 73: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `'perf_user_' || lpad((20 +
            // ((scope.number - 1) * 2) + 1)::text, 3, '0'),`.
            // Baris literal 74: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `'perf_user_' || lpad((20 +
            // ((scope.number - 1) * 2) + 2)::text, 3, '0')`.
            // Baris literal 75: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 76: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `) participant;`.
            // Baris literal 77: Pembatas literal/penutup ``; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 78: UPDATE memilih tabel yang akan diperbarui; kolom dan batas baris ditentukan oleh SET/WHERE: `update sessions session`.
            // Baris literal 79: SET menetapkan nilai kolom yang diperbarui oleh UPDATE: `set status = 'STARTED', player_count = 2, started_at = now()`.
            // Baris literal 80: FROM memilih tabel/subquery sumber pembacaan: `from performance_session_scope scope`.
            // Baris literal 81: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where session.session_id = scope.session_id;`.
            // Baris literal 82: Pembatas literal/penutup ``; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 83: INSERT INTO menetapkan tabel dan kolom tujuan penambahan rekaman: `insert into session_states (`.
            // Baris literal 84: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `session_id,`.
            // Baris literal 85: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `day,`.
            // Baris literal 86: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `weekday,`.
            // Baris literal 87: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `turn_number,`.
            // Baris literal 88: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `action_slot,`.
            // Baris literal 89: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `current_session_player_id,`.
            // Baris literal 90: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `current_action_slot,`.
            // Baris literal 91: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `action_slots_left,`.
            // Baris literal 92: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `finish_day,`.
            // Baris literal 93: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `phase,`.
            // Baris literal 94: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `is_game_over,`.
            // Baris literal 95: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `state_version,`.
            // Baris literal 96: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ui_state_json,`.
            // Baris literal 97: CREATE mendefinisikan objek basis data yang diperlukan proses ini: `created_at,`.
            // Baris literal 98: UPDATE memilih tabel yang akan diperbarui; kolom dan batas baris ditentukan oleh SET/WHERE: `updated_at`.
            // Baris literal 99: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 100: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
            // Baris literal 101: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `scope.session_id,`.
            // Baris literal 102: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `25,`.
            // Baris literal 103: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `'THU',`.
            // Baris literal 104: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `0,`.
            // Baris literal 105: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `0,`.
            // Baris literal 106: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `null,`.
            // Baris literal 107: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `1,`.
            // Baris literal 108: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun:
            // `settings.actions_per_turn,`.
            // Baris literal 109: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `settings.finish_day,`.
            // Baris literal 110: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `'PLAYER_TURN',`.
            // Baris literal 111: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `false,`.
            // Baris literal 112: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `1,`.
            // Baris literal 113: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `'{}'::jsonb,`.
            // Baris literal 114: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `now(),`.
            // Baris literal 115: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `now()`.
            // Baris literal 116: FROM memilih tabel/subquery sumber pembacaan: `from performance_session_scope scope`.
            // Baris literal 117: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join ruleset_game_settings settings`.
            // Baris literal 118: ON menetapkan syarat pencocokan baris pada relasi JOIN: `on settings.ruleset_version_id =
            // 'f5b4c67b-0825-4970-9f07-3b68e8fcb524'::uuid;`.
            // Baris literal 119: Pembatas literal/penutup ``; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 120: INSERT INTO menetapkan tabel dan kolom tujuan penambahan rekaman: `insert into events (`.
            // Baris literal 121: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `event_pk,`.
            // Baris literal 122: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `event_id,`.
            // Baris literal 123: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `session_id,`.
            // Baris literal 124: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `session_player_id,`.
            // Baris literal 125: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `user_id,`.
            // Baris literal 126: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `actor_type,`.
            // Baris literal 127: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `”timestamp”,`.
            // Baris literal 128: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `day_index,`.
            // Baris literal 129: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `weekday,`.
            // Baris literal 130: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `turn_number,`.
            // Baris literal 131: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `action_slot,`.
            // Baris literal 132: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `sequence_number,`.
            // Baris literal 133: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_action_id,`.
            // Baris literal 134: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `action_type,`.
            // Baris literal 135: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_version_id,`.
            // Baris literal 136: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `payload_version,`.
            // Baris literal 137: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `payload,`.
            // Baris literal 138: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `received_at,`.
            // Baris literal 139: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `client_request_id`.
            // Baris literal 140: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 141: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
            // Baris literal 142: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `gen_random_uuid(),`.
            // Baris literal 143: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `gen_random_uuid(),`.
            // Baris literal 144: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `scope.session_id,`.
            // Baris literal 145: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun:
            // `participant.session_participant_id,`.
            // Baris literal 146: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `participant.user_id,`.
            // Baris literal 147: Menyusun pilihan nilai bersyarat di dalam ekspresi SQL: `case when event_number <= 64 then 'PLAYER' else 'SYSTEM' end,`.
            // Baris literal 148: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `now() + make_interval(secs
            // => event_number::double precision / 1000),`.
            // Baris literal 149: Menyusun pilihan nilai bersyarat di dalam ekspresi SQL: `case when event_number <= 64`.
            // Baris literal 150: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `then ((event_number - 1) /
            // 16) * 7 + ((event_number - 1) / 4) % 4 + 1`.
            // Baris literal 151: Menyusun pilihan nilai bersyarat di dalam ekspresi SQL: `else 25 end,`.
            // Baris literal 152: Menyusun pilihan nilai bersyarat di dalam ekspresi SQL: `case when event_number <= 64`.
            // Baris literal 153: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `then
            // (array['MON','TUE','WED','THU'])[((event_number - 1) / 4) % 4 + 1]`.
            // Baris literal 154: Menyusun pilihan nilai bersyarat di dalam ekspresi SQL: `else 'THU' end,`.
            // Baris literal 155: Menyusun pilihan nilai bersyarat di dalam ekspresi SQL: `case when event_number <= 64 then participant.player_order_no else 0
            // end,`.
            // Baris literal 156: Menyusun pilihan nilai bersyarat di dalam ekspresi SQL: `case when event_number <= 64 then (event_number - 1) % 2 + 1 else 0
            // end,`.
            // Baris literal 157: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `event_number - 1,`.
            // Baris literal 158: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `action.ruleset_action_id,`.
            // Baris literal 159: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `action.action_id,`.
            // Baris literal 160: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun:
            // `'f5b4c67b-0825-4970-9f07-3b68e8fcb524'::uuid,`.
            // Baris literal 161: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `'1.0',`.
            // Baris literal 162: Menyusun pilihan nilai bersyarat di dalam ekspresi SQL: `case when action.action_id = 'KerjaLepas'`.
            // Baris literal 163: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `then '{”amount”:1}'::jsonb`.
            // Baris literal 164: Menyusun pilihan nilai bersyarat di dalam ekspresi SQL: `when action.action_id = 'BahanMasakan'`.
            // Baris literal 165: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `then
            // '{”card_id”:”nasi_putih”,”ingredient_name”:”Nasi Putih”,”amount”:1}'::jsonb`.
            // Baris literal 166: Menyusun pilihan nilai bersyarat di dalam ekspresi SQL: `else jsonb_build_object('direction', case when event_number % 2 = 1
            // then 'IN' else 'OUT' end,`.
            // Baris literal 167: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `'amount', 1, 'category',
            // 'PERFORMANCE', 'counterparty', 'BANK')`.
            // Baris literal 168: Menyusun pilihan nilai bersyarat di dalam ekspresi SQL: `end,`.
            // Baris literal 169: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `now(),`.
            // Baris literal 170: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `null`.
            // Baris literal 171: FROM memilih tabel/subquery sumber pembacaan: `from performance_session_scope scope`.
            // Baris literal 172: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `cross join generate_series(1, 2000) event_number`.
            // Baris literal 173: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join session_participants participant`.
            // Baris literal 174: ON menetapkan syarat pencocokan baris pada relasi JOIN: `on participant.session_id = scope.session_id`.
            // Baris literal 175: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and participant.player_order_no = ((event_number - 1) %
            // 4) / 2 + 1`.
            // Baris literal 176: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join ruleset_actions action`.
            // Baris literal 177: ON menetapkan syarat pencocokan baris pada relasi JOIN: `on action.ruleset_version_id =
            // 'f5b4c67b-0825-4970-9f07-3b68e8fcb524'::uuid`.
            // Baris literal 178: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and action.action_id = case when event_number > 64 then
            // 'CatatTransaksi'`.
            // Baris literal 179: Menyusun pilihan nilai bersyarat di dalam ekspresi SQL: `when event_number % 2 = 1 then 'KerjaLepas' else 'BahanMasakan'
            // end;`.
            // Baris literal 180: Pembatas literal/penutup ``; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 181: INSERT INTO menetapkan tabel dan kolom tujuan penambahan rekaman: `insert into event_cashflow_projections (`.
            // Baris literal 182: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `session_id, user_id,
            // event_pk, event_id, ”timestamp”, direction, amount, category`.
            // Baris literal 183: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 184: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
            // Baris literal 185: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `event.session_id,
            // event.user_id, event.event_pk, event.event_id, event.”timestamp”,`.
            // Baris literal 186: Menyusun pilihan nilai bersyarat di dalam ekspresi SQL: `case when event.sequence_number % 2 = 0 then 'IN' else 'OUT' end,`.
            // Baris literal 187: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `1,`.
            // Baris literal 188: Menyusun pilihan nilai bersyarat di dalam ekspresi SQL: `case when event.action_type = 'KerjaLepas' then 'FREELANCE'`.
            // Baris literal 189: Menyusun pilihan nilai bersyarat di dalam ekspresi SQL: `when event.action_type = 'BahanMasakan' then 'INGREDIENT' else
            // 'PERFORMANCE' end`.
            // Baris literal 190: FROM memilih tabel/subquery sumber pembacaan: `from events event`.
            // Baris literal 191: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join performance_session_scope scope on scope.session_id =
            // event.session_id;`.
            // Baris literal 192: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `”””, commandTimeout:
            // 180);`.
            """
            insert into app_users (user_id, username, display_name, password_hash, role, is_active, is_demo)
            select
                gen_random_uuid(),
                'perf_user_' || lpad(number::text, 3, '0'),
                'Performance User ' || number,
                source.password_hash,
                case when number <= 20 then 'INSTRUCTOR' else 'PLAYER' end,
                true,
                true
            from generate_series(1, 95) as number
            cross join lateral (
                select password_hash
                from app_users
                where lower(username::text) = 'rina.kartika'
                limit 1
            ) source
            on conflict (username) do nothing;

            create temporary table performance_session_scope (
                number int primary key,
                session_id uuid not null unique
            ) on commit drop;

            insert into performance_session_scope (number, session_id)
            select number, gen_random_uuid()
            from generate_series(1, 20) as number;

            insert into sessions (
                session_id,
                session_name,
                ruleset_version_id,
                mode,
                status,
                player_count,
                started_at,
                instructor_user_id
            )
            select
                scope.session_id,
                'Performance Active Session ' || scope.number,
                'f5b4c67b-0825-4970-9f07-3b68e8fcb524'::uuid,
                'PEMULA',
                'CREATED',
                0,
                null,
                (
                    select user_id
                    from app_users
                    where lower(username::text) = 'perf_user_' || lpad(scope.number::text, 3, '0')
                )
            from performance_session_scope scope;

            insert into session_participants (
                session_id,
                user_id,
                player_order_no,
                player_name
            )
            select
                scope.session_id,
                participant.user_id,
                participant.player_order_no,
                participant.display_name
            from performance_session_scope scope
            cross join lateral (
                select
                    user_id,
                    display_name,
                    row_number() over (order by username)::int as player_order_no
                from app_users
                where lower(username::text) in (
                    'perf_user_' || lpad((20 + ((scope.number - 1) * 2) + 1)::text, 3, '0'),
                    'perf_user_' || lpad((20 + ((scope.number - 1) * 2) + 2)::text, 3, '0')
                )
            ) participant;

            update sessions session
            set status = 'STARTED', player_count = 2, started_at = now()
            from performance_session_scope scope
            where session.session_id = scope.session_id;

            insert into session_states (
                session_id,
                day,
                weekday,
                turn_number,
                action_slot,
                current_session_player_id,
                current_action_slot,
                action_slots_left,
                finish_day,
                phase,
                is_game_over,
                state_version,
                ui_state_json,
                created_at,
                updated_at
            )
            select
                scope.session_id,
                25,
                'THU',
                0,
                0,
                null,
                1,
                settings.actions_per_turn,
                settings.finish_day,
                'PLAYER_TURN',
                false,
                1,
                '{}'::jsonb,
                now(),
                now()
            from performance_session_scope scope
            join ruleset_game_settings settings
              on settings.ruleset_version_id = 'f5b4c67b-0825-4970-9f07-3b68e8fcb524'::uuid;

            insert into events (
                event_pk,
                event_id,
                session_id,
                session_player_id,
                user_id,
                actor_type,
                "timestamp",
                day_index,
                weekday,
                turn_number,
                action_slot,
                sequence_number,
                ruleset_action_id,
                action_type,
                ruleset_version_id,
                payload_version,
                payload,
                received_at,
                client_request_id
            )
            select
                gen_random_uuid(),
                gen_random_uuid(),
                scope.session_id,
                participant.session_participant_id,
                participant.user_id,
                case when event_number <= 64 then 'PLAYER' else 'SYSTEM' end,
                now() + make_interval(secs => event_number::double precision / 1000),
                case when event_number <= 64
                    then ((event_number - 1) / 16) * 7 + ((event_number - 1) / 4) % 4 + 1
                    else 25 end,
                case when event_number <= 64
                    then (array['MON','TUE','WED','THU'])[((event_number - 1) / 4) % 4 + 1]
                    else 'THU' end,
                case when event_number <= 64 then participant.player_order_no else 0 end,
                case when event_number <= 64 then (event_number - 1) % 2 + 1 else 0 end,
                event_number - 1,
                action.ruleset_action_id,
                action.action_id,
                'f5b4c67b-0825-4970-9f07-3b68e8fcb524'::uuid,
                '1.0',
                case when action.action_id = 'KerjaLepas'
                    then '{"amount":1}'::jsonb
                    when action.action_id = 'BahanMasakan'
                    then '{"card_id":"nasi_putih","ingredient_name":"Nasi Putih","amount":1}'::jsonb
                    else jsonb_build_object('direction', case when event_number % 2 = 1 then 'IN' else 'OUT' end,
                        'amount', 1, 'category', 'PERFORMANCE', 'counterparty', 'BANK')
                end,
                now(),
                null
            from performance_session_scope scope
            cross join generate_series(1, 2000) event_number
            join session_participants participant
              on participant.session_id = scope.session_id
             and participant.player_order_no = ((event_number - 1) % 4) / 2 + 1
            join ruleset_actions action
              on action.ruleset_version_id = 'f5b4c67b-0825-4970-9f07-3b68e8fcb524'::uuid
             and action.action_id = case when event_number > 64 then 'CatatTransaksi'
                 when event_number % 2 = 1 then 'KerjaLepas' else 'BahanMasakan' end;

            insert into event_cashflow_projections (
                session_id, user_id, event_pk, event_id, "timestamp", direction, amount, category
            )
            select
                event.session_id, event.user_id, event.event_pk, event.event_id, event."timestamp",
                case when event.sequence_number % 2 = 0 then 'IN' else 'OUT' end,
                1,
                case when event.action_type = 'KerjaLepas' then 'FREELANCE'
                     when event.action_type = 'BahanMasakan' then 'INGREDIENT' else 'PERFORMANCE' end
            from events event
            join performance_session_scope scope on scope.session_id = event.session_id;
            """, commandTimeout: 180);

        // Menyiapkan variabel lokal `accountCount` untuk nilai account jumlah dengan hasil operasi asinkron menjalankan perintah basis data melalui
        // `connection` dengan `”select count(*) from app_users;”` dan mengambil nilai skalar hasilnya; await menunggu hasil tanpa memblokir thread selama
        // operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var accountCount = await connection.ExecuteScalarAsync<int>("select count(*) from app_users;");
        // Menyiapkan variabel lokal `activeSessionCount` untuk nilai aktif sesi jumlah dengan hasil operasi asinkron menjalankan perintah basis data
        // melalui `connection` dengan `”select count(*) from sessions where status = 'STARTED';”` dan mengambil nilai skalar hasilnya; await menunggu hasil
        // tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var activeSessionCount = await connection.ExecuteScalarAsync<int>("select count(*) from sessions where status = 'STARTED';");
        // Menyiapkan variabel lokal `minimumEventCount` untuk nilai minimum event jumlah dengan hasil operasi asinkron menjalankan perintah basis data
        // melalui `connection` dengan `”select min(event_count) from (select count(*)::int event_count from events where session_id in (select session_id
        // from sessions where session_name like 'Performance Active Se...` dan mengambil nilai skalar hasilnya; await menunggu hasil tanpa memblokir thread
        // selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var minimumEventCount = await connection.ExecuteScalarAsync<int>(
            // Meneruskan nilai literal `”select min(event_count) from (select count(*)::int event_count from events where session_id in (select session_id from
            // sessions where session_name like 'Performance Active Se...` sebagai argumen ke `connection.ExecuteScalarAsync<int>`.
            "select min(event_count) from (select count(*)::int event_count from events where session_id in (select session_id from sessions where session_name like 'Performance Active Session %') group by session_id) counts;");
        // Menjalankan pemeriksaan bahwa `accountCount >= 100`, `$”Dataset hanya memiliki {accountCount} akun.”` bernilai benar; pengujian gagal jika
        // kondisi tidak terpenuhi dalam PrepareReleaseDatasetAsync.
        Assert.True(accountCount >= 100, $"Dataset hanya memiliki {accountCount} akun.");
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`20`, `activeSessionCount`); pengujian gagal
        // jika keduanya berbeda dalam PrepareReleaseDatasetAsync.
        Assert.Equal(20, activeSessionCount);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`EventsPerSession`, `minimumEventCount`);
        // pengujian gagal jika keduanya berbeda dalam PrepareReleaseDatasetAsync.
        Assert.Equal(EventsPerSession, minimumEventCount);

        // Menyiapkan variabel lokal `scopes` untuk nilai scopes dengan hasil operasi asinkron menjalankan query baca melalui `connection` dengan `”””
        // select session.session_id, participant.user_id as player_user_id, session.instructor_user_id as owner_user_id from sessions session join
        // session_participants participant o...` dan memetakan baris hasil ke tipe yang diminta; await menunggu hasil tanpa memblokir thread selama operasi
        // belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var scopes = await connection.QueryAsync<PerformanceScope>(
            // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke `connection.QueryAsync<PerformanceScope>`.
            // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
            // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
            // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `session.session_id,`.
            // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `participant.user_id as
            // player_user_id,`.
            // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `session.instructor_user_id as
            // owner_user_id`.
            // Baris literal 6: FROM memilih tabel/subquery sumber pembacaan: `from sessions session`.
            // Baris literal 7: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join session_participants participant on
            // participant.session_id = session.session_id`.
            // Baris literal 8: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join app_users player on player.user_id =
            // participant.user_id`.
            // Baris literal 9: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join app_users owner on owner.user_id =
            // session.instructor_user_id`.
            // Baris literal 10: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where session.session_name like 'Performance Active
            // Session %'`.
            // Baris literal 11: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and lower(player.username::text) = 'perf_user_' ||
            // lpad(`.
            // Baris literal 12: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `(20 +
            // ((substring(session.session_name from '[0-9]+$')::int - 1) * 2) + 1)::text,`.
            // Baris literal 13: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `3,`.
            // Baris literal 14: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `'0'`.
            // Baris literal 15: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 16: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by lower(owner.username::text)`.
            // Baris literal 17: Pembatas literal/penutup `”””);`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            """
            select
                session.session_id,
                participant.user_id as player_user_id,
                session.instructor_user_id as owner_user_id
            from sessions session
            join session_participants participant on participant.session_id = session.session_id
            join app_users player on player.user_id = participant.user_id
            join app_users owner on owner.user_id = session.instructor_user_id
            where session.session_name like 'Performance Active Session %'
              and lower(player.username::text) = 'perf_user_' || lpad(
                  (20 + ((substring(session.session_name from '[0-9]+$')::int - 1) * 2) + 1)::text,
                  3,
                  '0'
              )
            order by lower(owner.username::text)
            """);
        // Mengembalikan mematerialisasi urutan `scopes` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori kepada pemanggil dalam
        // PrepareReleaseDatasetAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return scopes.ToList();
    // Menutup scope metode PrepareReleaseDatasetAsync; bagian berikut berada di luar batas blok tersebut dalam PrepareReleaseDatasetAsync.
    }

    // Mendefinisikan metode `LoginConcurrentUsersAsync` dengan hasil bertipe `Task<IReadOnlyList<string>>`; operasi ini menangani login concurrent
    // pengguna asinkron. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter
    // `factory` bertipe `WebApplicationFactory<Program>` membawa nilai factory.
    private static async Task<IReadOnlyList<string>> LoginConcurrentUsersAsync(WebApplicationFactory<Program> factory)
    // Membuka scope metode LoginConcurrentUsersAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam LoginConcurrentUsersAsync.
    {
        // Menyiapkan variabel lokal `loginTasks` untuk nilai login tasks dengan memetakan setiap elemen `Enumerable.Range(1, 20)` melalui `async number =>
        // { return await LoginAsync(factory, $”perf_user_{number:000}”); }` menjadi bentuk hasil yang dibutuhkan. Tipe variabel disimpulkan dari ekspresi
        // nilai awal.
        var loginTasks = Enumerable.Range(1, 20).Select(async number =>
        // Membuka scope fungsi lambda yang dipasok ke `Enumerable.Range(1, 20).Select`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // LoginConcurrentUsersAsync.
        {
            // Mengembalikan hasil operasi asinkron memanggil `LoginAsync` dengan `factory`, `$”perf_user_{number:000}”`; await menunggu hasil tanpa memblokir
            // thread selama operasi belum selesai kepada pemanggil dalam LoginConcurrentUsersAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return await LoginAsync(factory, $"perf_user_{number:000}");
        // Menutup scope fungsi lambda yang dipasok ke `Enumerable.Range(1, 20).Select`; bagian berikut berada di luar batas blok tersebut dalam
        // LoginConcurrentUsersAsync.
        });

        // Mengembalikan hasil operasi asinkron memanggil `Task.WhenAll` dengan `loginTasks`; await menunggu hasil tanpa memblokir thread selama operasi
        // belum selesai kepada pemanggil dalam LoginConcurrentUsersAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return await Task.WhenAll(loginTasks);
    // Menutup scope metode LoginConcurrentUsersAsync; bagian berikut berada di luar batas blok tersebut dalam LoginConcurrentUsersAsync.
    }

    // Mendefinisikan metode `LoginAsync` dengan hasil bertipe `Task<string>`; operasi ini menangani login asinkron. async memungkinkan metode menunggu
    // operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `factory` bertipe `WebApplicationFactory<Program>`
    // membawa nilai factory; Parameter `username` bertipe `string` membawa nama akun yang dipakai saat autentikasi.
    private static async Task<string> LoginAsync(WebApplicationFactory<Program> factory, string username)
    // Membuka scope metode LoginAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam LoginAsync.
    {
        // Mengembalikan `(await LoginDetailsAsync(factory, username)).AccessToken` (nilai akses token) kepada pemanggil dalam LoginAsync; eksekusi jalur
        // ini selesai setelah nilai hasil ditentukan.
        return (await LoginDetailsAsync(factory, username)).AccessToken;
    // Menutup scope metode LoginAsync; bagian berikut berada di luar batas blok tersebut dalam LoginAsync.
    }

    // Mendefinisikan metode `LoginDetailsAsync` dengan hasil bertipe `Task<LoginResponse>`; operasi ini menangani login rincian asinkron. async
    // memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `factory` bertipe
    // `WebApplicationFactory<Program>` membawa nilai factory; Parameter `username` bertipe `string` membawa nama akun yang dipakai saat autentikasi.
    private static async Task<LoginResponse> LoginDetailsAsync(WebApplicationFactory<Program> factory, string username)
    // Membuka scope metode LoginDetailsAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam LoginDetailsAsync.
    {
        // Menyiapkan variabel lokal `client` untuk nilai client dengan memanggil `factory.CreateClient` dengan `new WebApplicationFactoryClientOptions {
        // AllowAutoRedirect = false }`. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope
        // berakhir.
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        // Menyiapkan variabel lokal `response` untuk hasil respons yang akan dibaca atau dikirim kepada pemanggil dengan hasil operasi asinkron memanggil
        // `client.PostAsJsonAsync` dengan `”/api/v1/auth/login”`, `new LoginRequest(username, SeedPassword)`; await menunggu hasil tanpa memblokir thread
        // selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var response = await client.PostAsJsonAsync(
            // Meneruskan nilai literal `”/api/v1/auth/login”` sebagai argumen ke `client.PostAsJsonAsync`.
            "/api/v1/auth/login",
            // Meneruskan objek baru bertipe `LoginRequest` dengan argumen (username, SeedPassword) sebagai argumen ke `client.PostAsJsonAsync`; Meneruskan
            // `username` (nama akun yang dipakai saat autentikasi) sebagai argumen ke konstruktor `LoginRequest`; Meneruskan `SeedPassword` (nilai seed
            // password) sebagai argumen ke konstruktor `LoginRequest`.
            new LoginRequest(username, SeedPassword));
        // Menjalankan memanggil `response.EnsureSuccessStatusCode` dengan tanpa argumen dalam LoginDetailsAsync.
        response.EnsureSuccessStatusCode();
        // Menyiapkan variabel lokal `login` untuk nilai login dengan hasil operasi asinkron membaca tanpa argumen menjadi objek bertipe sesuai kontrak JSON
        // melalui `response.Content.ReadFromJsonAsync<LoginResponse>`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var login = await response.Content.ReadFromJsonAsync<LoginResponse>();
        // Mengembalikan pemeriksaan hasil dengan `Assert.IsType<LoginResponse>` menggunakan `login`; ketidaksesuaian dengan ekspektasi membuat pengujian
        // gagal kepada pemanggil dalam LoginDetailsAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return Assert.IsType<LoginResponse>(login);
    // Menutup scope metode LoginDetailsAsync; bagian berikut berada di luar batas blok tersebut dalam LoginDetailsAsync.
    }

    // Mendefinisikan metode `MeasureP95Async` dengan hasil bertipe `Task<double>`; operasi ini menangani measure 95 asinkron. async memungkinkan metode
    // menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `factory` bertipe
    // `WebApplicationFactory<Program>` membawa nilai factory; Parameter `tokens` bertipe `IReadOnlyList<string>` membawa nilai tokens; Parameter
    // `paths` bertipe `IReadOnlyList<string>` membawa nilai paths; Parameter `requestsPerUser` bertipe `int` membawa nilai requests per pengguna.
    private static async Task<double> MeasureP95Async(
        // Parameter `factory` bertipe `WebApplicationFactory<Program>` membawa nilai factory.
        WebApplicationFactory<Program> factory,
        // Parameter `tokens` bertipe `IReadOnlyList<string>` membawa nilai tokens.
        IReadOnlyList<string> tokens,
        // Parameter `paths` bertipe `IReadOnlyList<string>` membawa nilai paths.
        IReadOnlyList<string> paths,
        // Parameter `requestsPerUser` bertipe `int` membawa nilai requests per pengguna.
        int requestsPerUser)
    // Membuka scope metode MeasureP95Async; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam MeasureP95Async.
    {
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`tokens.Count`, `paths.Count`); pengujian gagal
        // jika keduanya berbeda dalam MeasureP95Async.
        Assert.Equal(tokens.Count, paths.Count);
        // Menyiapkan variabel lokal `durations` untuk nilai durations dengan objek baru bertipe `ConcurrentBag<double>` dengan nilai awal sesuai
        // konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var durations = new ConcurrentBag<double>();
        // Menyiapkan variabel lokal `clients` untuk nilai clients dengan mematerialisasi urutan `tokens.Select(_ => factory.CreateClient(new
        // WebApplicationFactoryClientOptions { AllowAutoRedirect = false }))` menjadi array dengan elemen hasil saat ini. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var clients = tokens.Select(_ => factory.CreateClient(new WebApplicationFactoryClientOptions
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam MeasureP95Async.
        {
            // Memperbarui `AllowAutoRedirect` menggunakan false, yaitu kondisi nonaktif/tidak terpenuhi dalam MeasureP95Async.
            AllowAutoRedirect = false
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam MeasureP95Async.
        })).ToArray();

        // Memulai blok try dalam MeasureP95Async; exception dari blok ini dapat dialihkan ke catch, sedangkan finally (jika ada) tetap dijalankan saat
        // keluar.
        try
        // Membuka scope penanganan operasi try; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam MeasureP95Async.
        {
            // Menyiapkan variabel lokal `tasks` untuk nilai tasks dengan memetakan setiap elemen `tokens` melalui `(token, index) => Task.Run(async () => { for
            // (var requestIndex = 0; requestIndex < requestsPerUser; requestIndex += 1) { var path = paths[index]; using var request = new HttpR...` menjadi
            // bentuk hasil yang dibutuhkan. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var tasks = tokens.Select((token, index) => Task.Run(async () =>
            // Membuka scope fungsi lambda yang dipasok ke `Task.Run`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam MeasureP95Async.
            {
                // Memulai loop dengan inisialisasi `var requestIndex = 0`, berjalan selama `requestIndex < requestsPerUser`, lalu memperbarui pencacah melalui
                // `requestIndex += 1` dalam MeasureP95Async.
                for (var requestIndex = 0; requestIndex < requestsPerUser; requestIndex += 1)
                // Membuka scope loop dengan syarat `requestIndex < requestsPerUser`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // MeasureP95Async.
                {
                    // Menyiapkan variabel lokal `path` untuk nilai path dengan `paths[index]`, yaitu elemen koleksi yang dipilih melalui indeks atau kunci tersebut.
                    // Tipe variabel disimpulkan dari ekspresi nilai awal.
                    var path = paths[index];
                    // Menyiapkan variabel lokal `request` untuk data masukan permintaan yang akan divalidasi atau diteruskan ke layanan dengan objek baru bertipe
                    // `HttpRequestMessage` dengan argumen (HttpMethod.Get, path). Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya
                    // dilepas otomatis saat scope berakhir.
                    using var request = new HttpRequestMessage(HttpMethod.Get, path);
                    // Memperbarui `request.Headers.Authorization` menggunakan objek baru bertipe `AuthenticationHeaderValue` dengan argumen (”Bearer”, token) dalam
                    // MeasureP95Async.
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    // Menyiapkan variabel lokal `started` untuk nilai started dengan memanggil `Stopwatch.GetTimestamp` dengan tanpa argumen. Tipe variabel disimpulkan
                    // dari ekspresi nilai awal.
                    var started = Stopwatch.GetTimestamp();
                    // Menyiapkan variabel lokal `response` untuk hasil respons yang akan dibaca atau dikirim kepada pemanggil dengan hasil operasi asinkron memanggil
                    // `clients[index].SendAsync` dengan `request`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan
                    // dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
                    using var response = await clients[index].SendAsync(request);
                    // Menjalankan menambahkan `Stopwatch.GetElapsedTime(started).TotalMilliseconds` ke `durations` dalam MeasureP95Async.
                    durations.Add(Stopwatch.GetElapsedTime(started).TotalMilliseconds);
                    // Memeriksa kebalikan kondisi `response.IsSuccessStatusCode`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam MeasureP95Async.
                    if (!response.IsSuccessStatusCode)
                    // Membuka scope cabang if untuk kondisi `!response.IsSuccessStatusCode`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                    // MeasureP95Async.
                    {
                        // Menyiapkan variabel lokal `body` untuk nilai body dengan hasil operasi asinkron memanggil `response.Content.ReadAsStringAsync` dengan tanpa
                        // argumen; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
                        var body = await response.Content.ReadAsStringAsync();
                        // Menghentikan alur dengan melempar objek baru bertipe `HttpRequestException` dengan argumen ($”GET {path} menghasilkan {(int)response.StatusCode}:
                        // {body}”) dalam MeasureP95Async; pemanggil atau middleware penanganan error menerima kegagalan ini.
                        throw new HttpRequestException($"GET {path} menghasilkan {(int)response.StatusCode}: {body}");
                    // Menutup scope cabang if untuk kondisi `!response.IsSuccessStatusCode`; bagian berikut berada di luar batas blok tersebut dalam MeasureP95Async.
                    }
                // Menutup scope loop dengan syarat `requestIndex < requestsPerUser`; bagian berikut berada di luar batas blok tersebut dalam MeasureP95Async.
                }
            // Menutup scope fungsi lambda yang dipasok ke `Task.Run`; bagian berikut berada di luar batas blok tersebut dalam MeasureP95Async.
            }));
            // Menjalankan hasil operasi asinkron memanggil `Task.WhenAll` dengan `tasks`; await menunggu hasil tanpa memblokir thread selama operasi belum
            // selesai dalam MeasureP95Async.
            await Task.WhenAll(tasks);
        // Menutup scope penanganan operasi try; bagian berikut berada di luar batas blok tersebut dalam MeasureP95Async.
        }
        // Menjalankan blok finally ketika alur meninggalkan try/catch, termasuk saat terjadi exception dalam MeasureP95Async; bagian ini dipakai untuk
        // pekerjaan penutup yang harus tetap dilakukan.
        finally
        // Membuka scope pekerjaan penutup finally; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam MeasureP95Async.
        {
            // Mengulangi setiap elemen `clients`; elemen saat ini disimpan sebagai `client` bertipe `var` untuk diproses oleh badan loop dalam MeasureP95Async.
            foreach (var client in clients)
            // Membuka scope loop setiap client dari `clients`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam MeasureP95Async.
            {
                // Menjalankan melepaskan sumber daya milik `client` setelah selesai digunakan dalam MeasureP95Async.
                client.Dispose();
            // Menutup scope loop setiap client dari `clients`; bagian berikut berada di luar batas blok tersebut dalam MeasureP95Async.
            }
        // Menutup scope pekerjaan penutup finally; bagian berikut berada di luar batas blok tersebut dalam MeasureP95Async.
        }

        // Menyiapkan variabel lokal `ordered` untuk nilai ordered dengan mematerialisasi urutan `durations.Order()` menjadi array dengan elemen hasil saat
        // ini. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var ordered = durations.Order().ToArray();
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`tokens.Count * requestsPerUser`,
        // `ordered.Length`); pengujian gagal jika keduanya berbeda dalam MeasureP95Async.
        Assert.Equal(tokens.Count * requestsPerUser, ordered.Length);
        // Menyiapkan variabel lokal `p95Index` untuk nilai p 95 index dengan menentukan nilai terbesar dari `0`, `(int)Math.Ceiling(ordered.Length * 0.95)
        // - 1`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var p95Index = Math.Max(0, (int)Math.Ceiling(ordered.Length * 0.95) - 1);
        // Mengembalikan `ordered[p95Index]`, yaitu elemen koleksi yang dipilih melalui indeks atau kunci tersebut kepada pemanggil dalam MeasureP95Async;
        // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return ordered[p95Index];
    // Menutup scope metode MeasureP95Async; bagian berikut berada di luar batas blok tersebut dalam MeasureP95Async.
    }

    // Mendefinisikan metode `MeasureEventIngestionP95Async` dengan hasil bertipe `Task<double>`; operasi ini menangani measure event ingestion 95
    // asinkron. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `factory`
    // bertipe `WebApplicationFactory<Program>` membawa nilai factory; Parameter `tokens` bertipe `IReadOnlyList<string>` membawa nilai tokens;
    // Parameter `scopes` bertipe `IReadOnlyList<PerformanceScope>` membawa nilai scopes; Parameter `requestsPerUser` bertipe `int` membawa nilai
    // requests per pengguna.
    private static async Task<double> MeasureEventIngestionP95Async(
        // Parameter `factory` bertipe `WebApplicationFactory<Program>` membawa nilai factory.
        WebApplicationFactory<Program> factory,
        // Parameter `tokens` bertipe `IReadOnlyList<string>` membawa nilai tokens.
        IReadOnlyList<string> tokens,
        // Parameter `scopes` bertipe `IReadOnlyList<PerformanceScope>` membawa nilai scopes.
        IReadOnlyList<PerformanceScope> scopes,
        // Parameter `requestsPerUser` bertipe `int` membawa nilai requests per pengguna.
        int requestsPerUser)
    // Membuka scope metode MeasureEventIngestionP95Async; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // MeasureEventIngestionP95Async.
    {
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`tokens.Count`, `scopes.Count`); pengujian
        // gagal jika keduanya berbeda dalam MeasureEventIngestionP95Async.
        Assert.Equal(tokens.Count, scopes.Count);
        // Menyiapkan variabel lokal `durations` untuk nilai durations dengan objek baru bertipe `ConcurrentBag<double>` dengan nilai awal sesuai
        // konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var durations = new ConcurrentBag<double>();
        // Menyiapkan variabel lokal `clients` untuk nilai clients dengan mematerialisasi urutan `tokens.Select(_ => factory.CreateClient(new
        // WebApplicationFactoryClientOptions { AllowAutoRedirect = false }))` menjadi array dengan elemen hasil saat ini. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var clients = tokens.Select(_ => factory.CreateClient(new WebApplicationFactoryClientOptions
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // MeasureEventIngestionP95Async.
        {
            // Memperbarui `AllowAutoRedirect` menggunakan false, yaitu kondisi nonaktif/tidak terpenuhi dalam MeasureEventIngestionP95Async.
            AllowAutoRedirect = false
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam MeasureEventIngestionP95Async.
        })).ToArray();

        // Memulai blok try dalam MeasureEventIngestionP95Async; exception dari blok ini dapat dialihkan ke catch, sedangkan finally (jika ada) tetap
        // dijalankan saat keluar.
        try
        // Membuka scope penanganan operasi try; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam MeasureEventIngestionP95Async.
        {
            // Menyiapkan variabel lokal `tasks` untuk nilai tasks dengan memetakan setiap elemen `tokens` melalui `(token, index) => Task.Run(async () => {
            // clients[index].DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(”Bearer”, token); for (var requestIndex = 0; reques...`
            // menjadi bentuk hasil yang dibutuhkan. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var tasks = tokens.Select((token, index) => Task.Run(async () =>
            // Membuka scope fungsi lambda yang dipasok ke `Task.Run`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // MeasureEventIngestionP95Async.
            {
                // Memperbarui `clients[index].DefaultRequestHeaders.Authorization` menggunakan objek baru bertipe `AuthenticationHeaderValue` dengan argumen
                // (”Bearer”, token) dalam MeasureEventIngestionP95Async.
                clients[index].DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                // Memulai loop dengan inisialisasi `var requestIndex = 0`, berjalan selama `requestIndex < requestsPerUser`, lalu memperbarui pencacah melalui
                // `requestIndex += 1` dalam MeasureEventIngestionP95Async.
                for (var requestIndex = 0; requestIndex < requestsPerUser; requestIndex += 1)
                // Membuka scope loop dengan syarat `requestIndex < requestsPerUser`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // MeasureEventIngestionP95Async.
                {
                    // Menyiapkan variabel lokal `request` untuk data masukan permintaan yang akan divalidasi atau diteruskan ke layanan dengan objek baru bertipe
                    // `EventRequest` dengan argumen ( Guid.NewGuid(), scopes[index].SessionId, scopes[index].PlayerUserId, ”SYSTEM”,
                    // DateTimeOffset.UtcNow.AddMilliseconds(requestIndex), 25, ”THU”, 0, EventsPerSes.... Tipe variabel disimpulkan dari ekspresi nilai awal.
                    var request = new EventRequest(
                        // Meneruskan memanggil `Guid.NewGuid` dengan tanpa argumen sebagai argumen ke konstruktor `EventRequest`.
                        Guid.NewGuid(),
                        // Meneruskan `scopes[index].SessionId` (identitas unik sesi permainan yang menjadi batas data operasi ini) sebagai argumen ke konstruktor
                        // `EventRequest`; Meneruskan `index` (nilai index) sebagai argumen ke konstruktor `EventRequest`.
                        scopes[index].SessionId,
                        // Meneruskan `scopes[index].PlayerUserId` (nilai pemain pengguna identitas) sebagai argumen ke konstruktor `EventRequest`; Meneruskan `index`
                        // (nilai index) sebagai argumen ke konstruktor `EventRequest`.
                        scopes[index].PlayerUserId,
                        // Meneruskan nilai literal `”SYSTEM”` sebagai argumen ke konstruktor `EventRequest`.
                        "SYSTEM",
                        // Meneruskan memanggil `DateTimeOffset.UtcNow.AddMilliseconds` dengan `requestIndex` sebagai argumen ke konstruktor `EventRequest`; Meneruskan
                        // `requestIndex` (nilai permintaan index) sebagai argumen ke `DateTimeOffset.UtcNow.AddMilliseconds`.
                        DateTimeOffset.UtcNow.AddMilliseconds(requestIndex),
                        // Meneruskan nilai literal `25` sebagai argumen ke konstruktor `EventRequest`.
                        25,
                        // Meneruskan nilai literal `”THU”` sebagai argumen ke konstruktor `EventRequest`.
                        "THU",
                        // Meneruskan nilai literal `0` sebagai argumen ke konstruktor `EventRequest`.
                        0,
                        // Meneruskan penjumlahan/penggabungan antara `EventsPerSession` dan `requestIndex` sebagai argumen ke konstruktor `EventRequest`.
                        EventsPerSession + requestIndex,
                        // Meneruskan nilai literal `”CatatTransaksi”` sebagai argumen ke konstruktor `EventRequest`.
                        "CatatTransaksi",
                        // Meneruskan `ReleaseRulesetVersionId` (nilai release aturan versi identitas) sebagai argumen ke konstruktor `EventRequest`.
                        ReleaseRulesetVersionId,
                        // Meneruskan memanggil `JsonSerializer.SerializeToElement` dengan `new { direction = requestIndex % 2 == 0 ? ”IN” : ”OUT”, amount = 1, category =
                        // ”PERFORMANCE”, counterparty = ”BANK” }` sebagai argumen ke konstruktor `EventRequest`; Meneruskan objek anonim yang mengelompokkan direction,
                        // amount, category, counterparty sebagai satu nilai sebagai argumen ke `JsonSerializer.SerializeToElement`.
                        JsonSerializer.SerializeToElement(new
                        // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                        // MeasureEventIngestionP95Async.
                        {
                            // Meneruskan objek anonim yang mengelompokkan direction, amount, category, counterparty sebagai satu nilai sebagai argumen ke
                            // `JsonSerializer.SerializeToElement`.
                            direction = requestIndex % 2 == 0 ? "IN" : "OUT",
                            // Meneruskan objek anonim yang mengelompokkan direction, amount, category, counterparty sebagai satu nilai sebagai argumen ke
                            // `JsonSerializer.SerializeToElement`.
                            amount = 1,
                            // Meneruskan objek anonim yang mengelompokkan direction, amount, category, counterparty sebagai satu nilai sebagai argumen ke
                            // `JsonSerializer.SerializeToElement`.
                            category = "PERFORMANCE",
                            // Meneruskan objek anonim yang mengelompokkan direction, amount, category, counterparty sebagai satu nilai sebagai argumen ke
                            // `JsonSerializer.SerializeToElement`.
                            counterparty = "BANK"
                        // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
                        // MeasureEventIngestionP95Async.
                        }),
                        // Meneruskan teks interpolasi `$”perf-{index}-{requestIndex}-{Guid.NewGuid():N}”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program
                        // berjalan sebagai argumen ke konstruktor `EventRequest`.
                        $"perf-{index}-{requestIndex}-{Guid.NewGuid():N}",
                        // Meneruskan nilai literal `0` sebagai argumen ke konstruktor `EventRequest`.
                        0);

                    // Menyiapkan variabel lokal `started` untuk nilai started dengan memanggil `Stopwatch.GetTimestamp` dengan tanpa argumen. Tipe variabel disimpulkan
                    // dari ekspresi nilai awal.
                    var started = Stopwatch.GetTimestamp();
                    // Menyiapkan variabel lokal `response` untuk hasil respons yang akan dibaca atau dikirim kepada pemanggil dengan hasil operasi asinkron memanggil
                    // `clients[index].PostAsJsonAsync` dengan `”/api/v1/events”`, `request`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai.
                    // Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
                    using var response = await clients[index].PostAsJsonAsync("/api/v1/events", request);
                    // Menjalankan menambahkan `Stopwatch.GetElapsedTime(started).TotalMilliseconds` ke `durations` dalam MeasureEventIngestionP95Async.
                    durations.Add(Stopwatch.GetElapsedTime(started).TotalMilliseconds);
                    // Memeriksa perbandingan ketidaksamaan antara `response.StatusCode` dan `System.Net.HttpStatusCode.Created`; blok if hanya dijalankan ketika
                    // kondisi ini bernilai benar dalam MeasureEventIngestionP95Async.
                    if (response.StatusCode != System.Net.HttpStatusCode.Created)
                    // Membuka scope cabang if untuk kondisi `response.StatusCode != System.Net.HttpStatusCode.Created`; pernyataan/deklarasi berikut berada di dalam
                    // batas blok ini dalam MeasureEventIngestionP95Async.
                    {
                        // Menyiapkan variabel lokal `body` untuk nilai body dengan hasil operasi asinkron memanggil `response.Content.ReadAsStringAsync` dengan tanpa
                        // argumen; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
                        var body = await response.Content.ReadAsStringAsync();
                        // Menghentikan alur dengan melempar objek baru bertipe `HttpRequestException` dengan argumen ($”POST /api/v1/events menghasilkan
                        // {(int)response.StatusCode}: {body}”) dalam MeasureEventIngestionP95Async; pemanggil atau middleware penanganan error menerima kegagalan ini.
                        throw new HttpRequestException($"POST /api/v1/events menghasilkan {(int)response.StatusCode}: {body}");
                    // Menutup scope cabang if untuk kondisi `response.StatusCode != System.Net.HttpStatusCode.Created`; bagian berikut berada di luar batas blok
                    // tersebut dalam MeasureEventIngestionP95Async.
                    }
                // Menutup scope loop dengan syarat `requestIndex < requestsPerUser`; bagian berikut berada di luar batas blok tersebut dalam
                // MeasureEventIngestionP95Async.
                }
            // Menutup scope fungsi lambda yang dipasok ke `Task.Run`; bagian berikut berada di luar batas blok tersebut dalam MeasureEventIngestionP95Async.
            }));
            // Menjalankan hasil operasi asinkron memanggil `Task.WhenAll` dengan `tasks`; await menunggu hasil tanpa memblokir thread selama operasi belum
            // selesai dalam MeasureEventIngestionP95Async.
            await Task.WhenAll(tasks);
        // Menutup scope penanganan operasi try; bagian berikut berada di luar batas blok tersebut dalam MeasureEventIngestionP95Async.
        }
        // Menjalankan blok finally ketika alur meninggalkan try/catch, termasuk saat terjadi exception dalam MeasureEventIngestionP95Async; bagian ini
        // dipakai untuk pekerjaan penutup yang harus tetap dilakukan.
        finally
        // Membuka scope pekerjaan penutup finally; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam MeasureEventIngestionP95Async.
        {
            // Mengulangi setiap elemen `clients`; elemen saat ini disimpan sebagai `client` bertipe `var` untuk diproses oleh badan loop dalam
            // MeasureEventIngestionP95Async.
            foreach (var client in clients)
            // Membuka scope loop setiap client dari `clients`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam MeasureEventIngestionP95Async.
            {
                // Menjalankan melepaskan sumber daya milik `client` setelah selesai digunakan dalam MeasureEventIngestionP95Async.
                client.Dispose();
            // Menutup scope loop setiap client dari `clients`; bagian berikut berada di luar batas blok tersebut dalam MeasureEventIngestionP95Async.
            }
        // Menutup scope pekerjaan penutup finally; bagian berikut berada di luar batas blok tersebut dalam MeasureEventIngestionP95Async.
        }

        // Menyiapkan variabel lokal `ordered` untuk nilai ordered dengan mematerialisasi urutan `durations.Order()` menjadi array dengan elemen hasil saat
        // ini. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var ordered = durations.Order().ToArray();
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`tokens.Count * requestsPerUser`,
        // `ordered.Length`); pengujian gagal jika keduanya berbeda dalam MeasureEventIngestionP95Async.
        Assert.Equal(tokens.Count * requestsPerUser, ordered.Length);
        // Menyiapkan variabel lokal `p95Index` untuk nilai p 95 index dengan menentukan nilai terbesar dari `0`, `(int)Math.Ceiling(ordered.Length * 0.95)
        // - 1`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var p95Index = Math.Max(0, (int)Math.Ceiling(ordered.Length * 0.95) - 1);
        // Mengembalikan `ordered[p95Index]`, yaitu elemen koleksi yang dipilih melalui indeks atau kunci tersebut kepada pemanggil dalam
        // MeasureEventIngestionP95Async; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return ordered[p95Index];
    // Menutup scope metode MeasureEventIngestionP95Async; bagian berikut berada di luar batas blok tersebut dalam MeasureEventIngestionP95Async.
    }

    // Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `PerformanceScope`; sealed mencegah tipe ini diturunkan lagi.
    private sealed record PerformanceScope(Guid SessionId, Guid PlayerUserId, Guid OwnerUserId);
// Menutup scope tipe ReleasePerformanceIntegrationTests; bagian berikut berada di luar batas blok tersebut.
}
