// Fungsi file: Menghapus log yang melewati kebijakan retensi database secara berkala.
using Dapper;
using Npgsql;

namespace Cashflowpoly.Api.Infrastructure;

internal sealed class LogRetentionWorker : BackgroundService
{
    private readonly NpgsqlDataSource _dataSource;
    private readonly ILogger<LogRetentionWorker> _logger;

    public LogRetentionWorker(NpgsqlDataSource dataSource, ILogger<LogRetentionWorker> logger)
    {
        _dataSource = dataSource;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await PurgeAsync(stoppingToken);
        using var timer = new PeriodicTimer(TimeSpan.FromHours(24));
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await PurgeAsync(stoppingToken);
        }
    }

    private async Task PurgeAsync(CancellationToken cancellationToken)
    {
        try
        {
            await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
            var deleted = await connection.ExecuteScalarAsync<int>(new CommandDefinition(
                "select coalesce(sum(purge_logs_by_retention(table_name)), 0)::int from log_retention_policies;",
                cancellationToken: cancellationToken));
            if (deleted > 0)
            {
                _logger.LogInformation("Purged {DeletedRows} expired operational log rows", deleted);
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            // Shutdown normal aplikasi.
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to purge expired operational logs");
        }
    }
}
