// Fungsi file: Menyediakan komponen infrastruktur API untuk kebutuhan DatabaseHealthCheck.
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Npgsql;

namespace Cashflowpoly.Api.Infrastructure;

/// <summary>
/// Menyatakan peran utama tipe DatabaseHealthCheck pada modul ini.
/// </summary>
internal sealed class DatabaseHealthCheck : IHealthCheck
{
    private readonly NpgsqlDataSource _dataSource;

    /// <summary>
    /// Menjalankan fungsi DatabaseHealthCheck sebagai bagian dari alur file ini.
    /// </summary>
    public DatabaseHealthCheck(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    /// <summary>
    /// Menjalankan fungsi CheckHealthAsync sebagai bagian dari alur file ini.
    /// </summary>
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
