// Fungsi file: Menghapus log yang melewati kebijakan retensi database secara berkala.
// Mengimpor namespace `Dapper` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Dapper;
// Mengimpor namespace `Npgsql` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Npgsql;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Infrastructure` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
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
        // Mengulangi blok selama hasil operasi asinkron memanggil `timer.WaitForNextTickAsync` dengan `stoppingToken`; await menunggu hasil tanpa memblokir
        // thread selama operasi belum selesai; kondisi diperiksa lagi sebelum setiap iterasi dalam ExecuteAsync.
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
        // Menangani exception `OperationCanceledException` melalui variabel hanya jika filter `cancellationToken.IsCancellationRequested` terpenuhi dalam
        // PurgeAsync.
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            // Shutdown normal aplikasi.
        // Menutup scope penanganan exception catch; bagian berikut berada di luar batas blok tersebut dalam PurgeAsync.
        }
        // Menangani exception `Exception` melalui variabel exception dalam PurgeAsync.
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to purge expired operational logs");
        }
    }
}
