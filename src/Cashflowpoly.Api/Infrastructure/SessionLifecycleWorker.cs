// Fungsi file: Mengakhiri sesi yang kehilangan heartbeat dan membersihkan sesi berakhir tanpa aktivitas bermain.
using Cashflowpoly.Api.Data;
using Dapper;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Cashflowpoly.Api.Infrastructure;

public sealed class SessionLifecycleWorker(
    IServiceScopeFactory scopeFactory,
    NpgsqlDataSource dataSource,
    IOptions<SessionLifecycleOptions> options,
    ILogger<SessionLifecycleWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!options.Value.Enabled) return;
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(options.Value.SweepIntervalSeconds));
        do
        {
            try { await SweepAsync(stoppingToken); }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { return; }
            catch (Exception exception) { logger.LogError(exception, "Failed to sweep inactive sessions"); }
        } while (await timer.WaitForNextTickAsync(stoppingToken));
    }

    public async Task SweepAsync(CancellationToken ct)
    {
        // ponytail: legacy empty ENDED sessions are scanned each sweep; move that cleanup to a migration if history becomes large.
        List<Guid> candidates;
        await using (var conn = await dataSource.OpenConnectionAsync(ct))
        {
            candidates = (await conn.QueryAsync<Guid>(new CommandDefinition("""
                select session_id from sessions
                where (status = 'CREATED' and last_activity_at <= clock_timestamp() - @createdTimeout)
                   or (status = 'STARTED' and last_activity_at <= clock_timestamp() - @startedTimeout)
                   or (status = 'ENDED' and not session_has_gameplay(session_id))
                order by last_activity_at
                limit 100
                """, new
                {
                    createdTimeout = TimeSpan.FromSeconds(options.Value.CreatedTimeoutSeconds),
                    startedTimeout = TimeSpan.FromSeconds(options.Value.StartedTimeoutSeconds)
                }, cancellationToken: ct))).ToList();
        }

        using var scope = scopeFactory.CreateScope();
        var state = scope.ServiceProvider.GetRequiredService<SessionStateRepository>();
        foreach (var candidate in candidates)
        {
            try
            {
                var result = await state.EndSessionAsync(candidate, ct,
                    TimeSpan.FromSeconds(options.Value.StartedTimeoutSeconds),
                    TimeSpan.FromSeconds(options.Value.CreatedTimeoutSeconds));
                if (result is not null)
                    logger.LogInformation("Session {SessionId}: {Result} after inactivity/empty-session cleanup", candidate, result);
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested) { throw; }
            catch (Exception exception)
            {
                logger.LogError(exception, "Failed to close inactive session {SessionId}", candidate);
            }
        }
    }
}
