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
{
    private readonly NpgsqlDataSource _dataSource;

    /// <summary>
    /// Membuat instance <see cref="DatabaseHealthCheck"/> dengan data source PostgreSQL.
    /// </summary>
    /// <param name="dataSource">Data source Npgsql untuk membuka koneksi database.</param>
    // Mendefinisikan konstruktor DatabaseHealthCheck yang menyiapkan objek dan menerima dependency/nilai awal dari pemanggil; parameter: Parameter
    // `dataSource` bertipe `NpgsqlDataSource` membawa sumber koneksi PostgreSQL yang mengelola pembuatan dan penggunaan ulang koneksi.
    public DatabaseHealthCheck(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
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
    {
        try
        {
            await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
            await using var command = connection.CreateCommand();
            command.CommandText = "select 1";
            _ = await command.ExecuteScalarAsync(cancellationToken);
            return HealthCheckResult.Healthy("Database reachable");
        }
        // Menangani exception `Exception` melalui variabel ex dalam CheckHealthAsync.
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Database unreachable", ex);
        }
    }
}
