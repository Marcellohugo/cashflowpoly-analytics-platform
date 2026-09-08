// Fungsi file: Memverifikasi perilaku API, database, atau domain melalui ApiWebApplicationFactory.
// Mengimpor namespace `Microsoft.AspNetCore.Hosting` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.AspNetCore.Hosting;
// Mengimpor namespace `Microsoft.AspNetCore.Mvc.Testing` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.AspNetCore.Mvc.Testing;
// Mengimpor namespace `Microsoft.Extensions.Configuration` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.Extensions.Configuration;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Tests.Infrastructure` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Tests.Infrastructure;

/// <summary>
/// Factory kustom yang menginisialisasi host API untuk pengujian integrasi
/// dengan mengganti connection string dan signing key JWT melalui in-memory configuration.
/// </summary>
// Mendefinisikan tipe class `ApiWebApplicationFactory` yang mewarisi atau menerapkan `WebApplicationFactory<Program>`; sealed mencegah tipe ini
// diturunkan lagi.
internal sealed class ApiWebApplicationFactory : WebApplicationFactory<Program>
// Membuka scope tipe ApiWebApplicationFactory; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendeklarasikan field bertipe `string`: `_connectionString` menyimpan nilai connection string. readonly membatasi penggantian referensi/nilai
    // field pada deklarasi atau konstruktor.
    private readonly string _connectionString;
    // Mendeklarasikan field bertipe `string`: `_jwtSigningKey` menyimpan nilai jwt signing kunci. readonly membatasi penggantian referensi/nilai field
    // pada deklarasi atau konstruktor.
    private readonly string _jwtSigningKey;
    // Mendeklarasikan field bertipe `bool`: `_seedSimulation` menyimpan nilai seed simulation. readonly membatasi penggantian referensi/nilai field
    // pada deklarasi atau konstruktor.
    private readonly bool _seedSimulation;

    /// <summary>
    /// Membuat instance factory dengan connection string database dan signing key JWT
    /// yang akan diinjeksikan ke konfigurasi aplikasi saat pengujian.
    /// </summary>
    // Mendefinisikan konstruktor ApiWebApplicationFactory yang menyiapkan objek dan menerima dependency/nilai awal dari pemanggil; parameter: Parameter
    // `connectionString` bertipe `string` membawa nilai connection string; Parameter `jwtSigningKey` bertipe `string` membawa nilai jwt signing kunci;
    // Parameter `seedSimulation` bertipe `bool` membawa nilai seed simulation; bila argumen tidak diberikan digunakan false, yaitu kondisi
    // nonaktif/tidak terpenuhi.
    public ApiWebApplicationFactory(string connectionString, string jwtSigningKey, bool seedSimulation = false)
    // Membuka scope konstruktor ApiWebApplicationFactory; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ApiWebApplicationFactory.
    {
        // Memperbarui `_connectionString` menggunakan `connectionString` (nilai connection string) dalam ApiWebApplicationFactory.
        _connectionString = connectionString;
        // Memperbarui `_jwtSigningKey` menggunakan `jwtSigningKey` (nilai jwt signing kunci) dalam ApiWebApplicationFactory.
        _jwtSigningKey = jwtSigningKey;
        // Memperbarui `_seedSimulation` menggunakan `seedSimulation` (nilai seed simulation) dalam ApiWebApplicationFactory.
        _seedSimulation = seedSimulation;
    // Menutup scope konstruktor ApiWebApplicationFactory; bagian berikut berada di luar batas blok tersebut dalam ApiWebApplicationFactory.
    }

    /// <summary>
    /// Mengonfigurasi web host dengan environment Testing dan menyuntikkan
    /// connection string serta signing key JWT melalui in-memory configuration.
    /// </summary>
    // Mendefinisikan metode `ConfigureWebHost` dengan hasil bertipe `void`. Mengonfigurasi web host dengan environment Testing dan menyuntikkan
    // connection string serta signing key JWT melalui in-memory configuration. Masukan: Parameter `builder` bertipe `IWebHostBuilder` membawa nilai
    // pembentuk.
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    // Membuka scope metode ConfigureWebHost; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ConfigureWebHost.
    {
        // Menjalankan memanggil `builder.UseEnvironment` dengan `”Testing”` dalam ConfigureWebHost.
        builder.UseEnvironment("Testing");
        // Menjalankan memanggil `builder.ConfigureAppConfiguration` dengan `(_, configBuilder) => { configBuilder.AddInMemoryCollection(new
        // Dictionary<string, string?> { [”ConnectionStrings:Default”] = _connectionString, [”Jwt:SigningKey”] = _jwtSigni...` dalam ConfigureWebHost.
        builder.ConfigureAppConfiguration((_, configBuilder) =>
        // Membuka scope fungsi lambda yang dipasok ke `builder.ConfigureAppConfiguration`; pernyataan/deklarasi berikut berada di dalam batas blok ini
        // dalam ConfigureWebHost.
        {
            // Menjalankan memanggil `configBuilder.AddInMemoryCollection` dengan `new Dictionary<string, string?> { [”ConnectionStrings:Default”] =
            // _connectionString, [”Jwt:SigningKey”] = _jwtSigningKey, [”JWT_SIGNING_KEY”] = _jwtSigningKey, [”AuthBootstrap...` dalam ConfigureWebHost.
            configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ConfigureWebHost.
            {
                // Memperbarui `[”ConnectionStrings:Default”]` menggunakan `_connectionString` (nilai connection string) dalam ConfigureWebHost.
                ["ConnectionStrings:Default"] = _connectionString,
                // Memperbarui `[”Jwt:SigningKey”]` menggunakan `_jwtSigningKey` (nilai jwt signing kunci) dalam ConfigureWebHost.
                ["Jwt:SigningKey"] = _jwtSigningKey,
                // Memperbarui `[”JWT_SIGNING_KEY”]` menggunakan `_jwtSigningKey` (nilai jwt signing kunci) dalam ConfigureWebHost.
                ["JWT_SIGNING_KEY"] = _jwtSigningKey,
                // Memperbarui `[”AuthBootstrap:SeedDefaultUsers”]` menggunakan nilai literal `”false”` dalam ConfigureWebHost.
                ["AuthBootstrap:SeedDefaultUsers"] = "false",
                // Memperbarui `[”Auth:AllowPublicInstructorRegistration”]` menggunakan nilai literal `”true”` dalam ConfigureWebHost.
                ["Auth:AllowPublicInstructorRegistration"] = "true",
                // Memperbarui `[”DatabaseMigrations:SeedSimulation”]` menggunakan mengubah `_seedSimulation` menjadi teks dalam ConfigureWebHost.
                ["DatabaseMigrations:SeedSimulation"] = _seedSimulation.ToString()
            // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam ConfigureWebHost.
            });
        // Menutup scope fungsi lambda yang dipasok ke `builder.ConfigureAppConfiguration`; bagian berikut berada di luar batas blok tersebut dalam
        // ConfigureWebHost.
        });
    // Menutup scope metode ConfigureWebHost; bagian berikut berada di luar batas blok tersebut dalam ConfigureWebHost.
    }
// Menutup scope tipe ApiWebApplicationFactory; bagian berikut berada di luar batas blok tersebut.
}
