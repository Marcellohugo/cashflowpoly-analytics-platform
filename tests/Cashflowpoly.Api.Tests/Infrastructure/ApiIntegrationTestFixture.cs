// Fungsi file: Memverifikasi perilaku API, database, atau domain melalui ApiIntegrationTestFixture.
// Mengimpor namespace `Microsoft.AspNetCore.Mvc.Testing` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.AspNetCore.Mvc.Testing;
// Mengimpor namespace `Testcontainers.PostgreSql` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Testcontainers.PostgreSql;
// Mengimpor namespace `Xunit` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Xunit;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Tests.Infrastructure` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Tests.Infrastructure;

/// <summary>
/// Fixture bersama untuk pengujian integrasi API yang mengelola lifecycle
/// container PostgreSQL kosong dan pembuatan HttpClient agar startup API
/// mengeksekusi migrasi aslinya sendiri.
/// </summary>
// Mendefinisikan tipe class `ApiIntegrationTestFixture` yang mewarisi atau menerapkan `IAsyncLifetime`; sealed mencegah tipe ini diturunkan lagi.
public sealed class ApiIntegrationTestFixture : IAsyncLifetime
// Membuka scope tipe ApiIntegrationTestFixture; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendeklarasikan field bertipe `string`: `JwtSigningKey` menyimpan nilai jwt signing kunci dengan nilai awal nilai literal
    // `”integration-test-signing-key-with-min-32-char”`. readonly membatasi penggantian referensi/nilai field pada deklarasi atau konstruktor. static
    // membuat field menjadi milik tipe dan dibagikan antar instance.
    private static readonly string JwtSigningKey = "integration-test-signing-key-with-min-32-char";

    // Mendeklarasikan field bertipe `PostgreSqlContainer`: `_dbContainer` menyimpan nilai basis data container. readonly membatasi penggantian
    // referensi/nilai field pada deklarasi atau konstruktor.
    private readonly PostgreSqlContainer _dbContainer;
    // Mendeklarasikan field bertipe `ApiWebApplicationFactory?`: `_factory` menyimpan nilai factory.
    private ApiWebApplicationFactory? _factory;
    // Mendeklarasikan field bertipe `HttpClient?`: `_client` menyimpan nilai client.
    private HttpClient? _client;
    // Mendeklarasikan field bertipe `string?`: `_previousConnectionString` menyimpan nilai previous connection string.
    private string? _previousConnectionString;
    // Mendeklarasikan field bertipe `string?`: `_previousJwtSigningKey` menyimpan nilai previous jwt signing kunci.
    private string? _previousJwtSigningKey;
    // Mendeklarasikan field bertipe `string?`: `_previousJwtSectionSigningKey` menyimpan nilai previous jwt section signing kunci.
    private string? _previousJwtSectionSigningKey;

    /// <summary>
    /// Membuat instance fixture dan mengonfigurasi container PostgreSQL
    /// dengan kredensial dan nama database untuk pengujian integrasi.
    /// </summary>
    // Mendefinisikan konstruktor ApiIntegrationTestFixture yang menyiapkan objek dan menerima dependency/nilai awal dari pemanggil; konstruktor ini
    // tidak memerlukan argumen.
    public ApiIntegrationTestFixture()
    // Membuka scope konstruktor ApiIntegrationTestFixture; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ApiIntegrationTestFixture.
    {
        // Memperbarui `_dbContainer` menggunakan memanggil `new PostgreSqlBuilder(”postgres:16”) .WithDatabase(”cashflowpoly_it”)
        // .WithUsername(”cashflowpoly”) .WithPassword(”cashflowpoly”) .Build` dengan tanpa argumen dalam ApiIntegrationTestFixture.
        _dbContainer = new PostgreSqlBuilder("postgres:16")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .WithDatabase(”cashflowpoly_it”) dalam ApiIntegrationTestFixture; token pada
            // baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .WithDatabase("cashflowpoly_it")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .WithUsername(”cashflowpoly”) dalam ApiIntegrationTestFixture; token pada baris
            // ini menyambungkan bagian kode sebelum dan sesudahnya.
            .WithUsername("cashflowpoly")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .WithPassword(”cashflowpoly”) dalam ApiIntegrationTestFixture; token pada baris
            // ini menyambungkan bagian kode sebelum dan sesudahnya.
            .WithPassword("cashflowpoly")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Build(); dalam ApiIntegrationTestFixture; token pada baris ini menyambungkan
            // bagian kode sebelum dan sesudahnya.
            .Build();
    // Menutup scope konstruktor ApiIntegrationTestFixture; bagian berikut berada di luar batas blok tersebut dalam ApiIntegrationTestFixture.
    }

    /// <summary>
    /// Menyediakan HttpClient yang telah terkonfigurasi untuk pengujian integrasi.
    /// Melempar exception jika belum diinisialisasi.
    /// </summary>
    // Mendefinisikan properti `Client` bertipe `HttpClient` untuk nilai client; nilainya dihitung dari `_client` bila tidak null; jika null gunakan
    // `throw new InvalidOperationException(”HTTP client belum terinisialisasi.”)` sebagai nilai pengganti.
    public HttpClient Client => _client ?? throw new InvalidOperationException("HTTP client belum terinisialisasi.");

    /// <summary>
    /// Menjalankan container PostgreSQL kosong, mengatur environment variable,
    /// dan membuat HttpClient melalui WebApplicationFactory.
    /// </summary>
    // Mendefinisikan metode `InitializeAsync` dengan hasil bertipe `ValueTask`. Menjalankan container PostgreSQL kosong, mengatur environment variable,
    // dan membuat HttpClient melalui WebApplicationFactory. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian
    // melalui Task.
    public async ValueTask InitializeAsync()
    // Membuka scope metode InitializeAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam InitializeAsync.
    {
        // Menjalankan hasil operasi asinkron memanggil `_dbContainer.StartAsync` dengan tanpa argumen; await menunggu hasil tanpa memblokir thread selama
        // operasi belum selesai dalam InitializeAsync.
        await _dbContainer.StartAsync();

        // Memperbarui `_previousConnectionString` menggunakan memanggil `Environment.GetEnvironmentVariable` dengan `”ConnectionStrings__Default”` dalam
        // InitializeAsync.
        _previousConnectionString = Environment.GetEnvironmentVariable("ConnectionStrings__Default");
        // Memperbarui `_previousJwtSigningKey` menggunakan memanggil `Environment.GetEnvironmentVariable` dengan `”JWT_SIGNING_KEY”` dalam InitializeAsync.
        _previousJwtSigningKey = Environment.GetEnvironmentVariable("JWT_SIGNING_KEY");
        // Memperbarui `_previousJwtSectionSigningKey` menggunakan memanggil `Environment.GetEnvironmentVariable` dengan `”Jwt__SigningKey”` dalam
        // InitializeAsync.
        _previousJwtSectionSigningKey = Environment.GetEnvironmentVariable("Jwt__SigningKey");
        // Menjalankan memanggil `Environment.SetEnvironmentVariable` dengan `”ConnectionStrings__Default”`, `_dbContainer.GetConnectionString()` dalam
        // InitializeAsync.
        Environment.SetEnvironmentVariable("ConnectionStrings__Default", _dbContainer.GetConnectionString());
        // Menjalankan memanggil `Environment.SetEnvironmentVariable` dengan `”JWT_SIGNING_KEY”`, `JwtSigningKey` dalam InitializeAsync.
        Environment.SetEnvironmentVariable("JWT_SIGNING_KEY", JwtSigningKey);
        // Menjalankan memanggil `Environment.SetEnvironmentVariable` dengan `”Jwt__SigningKey”`, `JwtSigningKey` dalam InitializeAsync.
        Environment.SetEnvironmentVariable("Jwt__SigningKey", JwtSigningKey);

        // Memperbarui `_factory` menggunakan objek baru bertipe `ApiWebApplicationFactory` dengan argumen (_dbContainer.GetConnectionString(),
        // JwtSigningKey) dalam InitializeAsync.
        _factory = new ApiWebApplicationFactory(_dbContainer.GetConnectionString(), JwtSigningKey);
        // Memperbarui `_client` menggunakan memanggil `_factory.CreateClient` dengan `new WebApplicationFactoryClientOptions { AllowAutoRedirect = false }`
        // dalam InitializeAsync.
        _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam InitializeAsync.
        {
            // Memperbarui `AllowAutoRedirect` menggunakan false, yaitu kondisi nonaktif/tidak terpenuhi dalam InitializeAsync.
            AllowAutoRedirect = false
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam InitializeAsync.
        });
    // Menutup scope metode InitializeAsync; bagian berikut berada di luar batas blok tersebut dalam InitializeAsync.
    }

    /// <summary>
    /// Membersihkan resource: menutup HttpClient, factory, mengembalikan
    /// environment variable, dan menghentikan container PostgreSQL.
    /// </summary>
    // Mendefinisikan metode `DisposeAsync` dengan hasil bertipe `ValueTask`. Membersihkan resource: menutup HttpClient, factory, mengembalikan
    // environment variable, dan menghentikan container PostgreSQL. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan
    // penyelesaian melalui Task.
    public async ValueTask DisposeAsync()
    // Membuka scope metode DisposeAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam DisposeAsync.
    {
        // Menjalankan `_client?.Dispose()`; akses setelah ?. hanya dilakukan bila penerimanya tidak null dalam DisposeAsync.
        _client?.Dispose();
        // Menjalankan `_factory?.Dispose()`; akses setelah ?. hanya dilakukan bila penerimanya tidak null dalam DisposeAsync.
        _factory?.Dispose();
        // Menjalankan memanggil `Environment.SetEnvironmentVariable` dengan `”ConnectionStrings__Default”`, `_previousConnectionString` dalam DisposeAsync.
        Environment.SetEnvironmentVariable("ConnectionStrings__Default", _previousConnectionString);
        // Menjalankan memanggil `Environment.SetEnvironmentVariable` dengan `”JWT_SIGNING_KEY”`, `_previousJwtSigningKey` dalam DisposeAsync.
        Environment.SetEnvironmentVariable("JWT_SIGNING_KEY", _previousJwtSigningKey);
        // Menjalankan memanggil `Environment.SetEnvironmentVariable` dengan `”Jwt__SigningKey”`, `_previousJwtSectionSigningKey` dalam DisposeAsync.
        Environment.SetEnvironmentVariable("Jwt__SigningKey", _previousJwtSectionSigningKey);
        // Menjalankan hasil operasi asinkron melepaskan sumber daya milik `_dbContainer` setelah selesai digunakan; await menunggu hasil tanpa memblokir
        // thread selama operasi belum selesai dalam DisposeAsync.
        await _dbContainer.DisposeAsync();
    // Menutup scope metode DisposeAsync; bagian berikut berada di luar batas blok tersebut dalam DisposeAsync.
    }
// Menutup scope tipe ApiIntegrationTestFixture; bagian berikut berada di luar batas blok tersebut.
}
