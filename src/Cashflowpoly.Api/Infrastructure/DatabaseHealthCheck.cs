// Fungsi file: Menyediakan dukungan infrastruktur API melalui DatabaseHealthCheck.
// Mengimpor namespace `Microsoft.Extensions.Diagnostics.HealthChecks` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama
// lengkapnya.
using Microsoft.Extensions.Diagnostics.HealthChecks;
// Mengimpor namespace `Npgsql` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Npgsql;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Infrastructure` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Infrastructure;

/// <summary>
/// Health check yang menguji koneksi ke database PostgreSQL dengan menjalankan query sederhana.
/// </summary>
// Mendefinisikan tipe class `DatabaseHealthCheck` yang mewarisi atau menerapkan `IHealthCheck`; sealed mencegah tipe ini diturunkan lagi.
internal sealed class DatabaseHealthCheck : IHealthCheck
// Membuka scope tipe DatabaseHealthCheck; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendeklarasikan field bertipe `NpgsqlDataSource`: `_dataSource` menyimpan sumber koneksi PostgreSQL yang mengelola pembuatan dan penggunaan ulang
    // koneksi. readonly membatasi penggantian referensi/nilai field pada deklarasi atau konstruktor.
    private readonly NpgsqlDataSource _dataSource;

    /// <summary>
    /// Membuat instance <see cref="DatabaseHealthCheck"/> dengan data source PostgreSQL.
    /// </summary>
    /// <param name="dataSource">Data source Npgsql untuk membuka koneksi database.</param>
    // Mendefinisikan konstruktor DatabaseHealthCheck yang menyiapkan objek dan menerima dependency/nilai awal dari pemanggil; parameter: Parameter
    // `dataSource` bertipe `NpgsqlDataSource` membawa sumber koneksi PostgreSQL yang mengelola pembuatan dan penggunaan ulang koneksi.
    public DatabaseHealthCheck(NpgsqlDataSource dataSource)
    // Membuka scope konstruktor DatabaseHealthCheck; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam DatabaseHealthCheck.
    {
        // Memperbarui `_dataSource` menggunakan `dataSource` (sumber koneksi PostgreSQL yang mengelola pembuatan dan penggunaan ulang koneksi) dalam
        // DatabaseHealthCheck.
        _dataSource = dataSource;
    // Menutup scope konstruktor DatabaseHealthCheck; bagian berikut berada di luar batas blok tersebut dalam DatabaseHealthCheck.
    }

    /// <summary>
    /// Mengeksekusi query <c>SELECT 1</c> ke database untuk memastikan koneksi tersedia.
    /// </summary>
    /// <param name="context">Konteks health check dari framework.</param>
    /// <param name="cancellationToken">Token pembatalan operasi.</param>
    /// <returns>Healthy jika database terjangkau, Unhealthy jika koneksi gagal.</returns>
    // Mendefinisikan metode `CheckHealthAsync` dengan hasil bertipe `Task<HealthCheckResult>`. Mengeksekusi query SELECT 1 ke database untuk memastikan
    // koneksi tersedia. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter
    // `context` bertipe `HealthCheckContext` membawa konteks operasi yang menyediakan data lingkungan pemrosesan saat ini; Parameter
    // `cancellationToken` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan
    // atau aplikasi berhenti; bila argumen tidak diberikan digunakan nilai literal `default`.
    public async Task<HealthCheckResult> CheckHealthAsync(
        // Parameter `context` bertipe `HealthCheckContext` membawa konteks operasi yang menyediakan data lingkungan pemrosesan saat ini.
        HealthCheckContext context,
        // Parameter `cancellationToken` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan
        // permintaan atau aplikasi berhenti; bila argumen tidak diberikan digunakan nilai literal `default`.
        CancellationToken cancellationToken = default)
    // Membuka scope metode CheckHealthAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CheckHealthAsync.
    {
        // Memulai blok try dalam CheckHealthAsync; exception dari blok ini dapat dialihkan ke catch, sedangkan finally (jika ada) tetap dijalankan saat
        // keluar.
        try
        // Membuka scope penanganan operasi try; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CheckHealthAsync.
        {
            // Menyiapkan variabel lokal `connection` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi
            // asinkron membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `cancellationToken`; await menunggu hasil tanpa memblokir thread selama
            // operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
            await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
            // Menyiapkan variabel lokal `command` untuk nilai command dengan memanggil `connection.CreateCommand` dengan tanpa argumen. Tipe variabel
            // disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
            await using var command = connection.CreateCommand();
            // Memperbarui `command.CommandText` menggunakan nilai literal `”select 1”` dalam CheckHealthAsync.
            command.CommandText = "select 1";
            // Memperbarui `_` menggunakan hasil operasi asinkron menjalankan perintah basis data melalui `command` dengan `cancellationToken` dan mengambil
            // nilai skalar hasilnya; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam CheckHealthAsync.
            _ = await command.ExecuteScalarAsync(cancellationToken);
            // Mengembalikan memanggil `HealthCheckResult.Healthy` dengan `”Database reachable”` kepada pemanggil dalam CheckHealthAsync; eksekusi jalur ini
            // selesai setelah nilai hasil ditentukan.
            return HealthCheckResult.Healthy("Database reachable");
        // Menutup scope penanganan operasi try; bagian berikut berada di luar batas blok tersebut dalam CheckHealthAsync.
        }
        // Menangani exception `Exception` melalui variabel ex dalam CheckHealthAsync.
        catch (Exception ex)
        // Membuka scope penanganan exception catch; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CheckHealthAsync.
        {
            // Mengembalikan memanggil `HealthCheckResult.Unhealthy` dengan `”Database unreachable”`, `ex` kepada pemanggil dalam CheckHealthAsync; eksekusi
            // jalur ini selesai setelah nilai hasil ditentukan.
            return HealthCheckResult.Unhealthy("Database unreachable", ex);
        // Menutup scope penanganan exception catch; bagian berikut berada di luar batas blok tersebut dalam CheckHealthAsync.
        }
    // Menutup scope metode CheckHealthAsync; bagian berikut berada di luar batas blok tersebut dalam CheckHealthAsync.
    }
// Menutup scope tipe DatabaseHealthCheck; bagian berikut berada di luar batas blok tersebut.
}
