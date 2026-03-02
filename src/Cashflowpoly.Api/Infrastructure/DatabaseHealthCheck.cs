// Fungsi file: Implementasi health check yang memverifikasi konektivitas ke database PostgreSQL untuk readiness probe.
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Npgsql;

namespace Cashflowpoly.Api.Infrastructure;

/// <summary>
/// Health check yang menguji koneksi ke database PostgreSQL dengan menjalankan query sederhana.
/// </summary>
internal sealed class DatabaseHealthCheck : IHealthCheck
{
    private readonly NpgsqlDataSource _dataSource;

    /// <summary>
    /// Membuat instance <see cref="DatabaseHealthCheck"/> dengan data source PostgreSQL.
    /// </summary>
    /// <param name="dataSource">Data source Npgsql untuk membuka koneksi database.</param>
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
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
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
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Database unreachable", ex);
        }
    }
}
