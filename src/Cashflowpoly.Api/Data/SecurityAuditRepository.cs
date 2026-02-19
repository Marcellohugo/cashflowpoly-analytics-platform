// Fungsi file: Menyediakan akses data PostgreSQL untuk domain SecurityAuditRepository melalui query dan command terenkapsulasi.
using Dapper;
using Npgsql;

namespace Cashflowpoly.Api.Data;

/// <summary>
/// Repository untuk menyimpan dan membaca audit log keamanan.
/// </summary>
public sealed class SecurityAuditRepository
{
    private readonly NpgsqlDataSource _dataSource;

    /// <summary>
    /// Menjalankan fungsi SecurityAuditRepository sebagai bagian dari alur file ini.
    /// </summary>
    public SecurityAuditRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    /// <summary>
    /// Menjalankan fungsi InsertAsync sebagai bagian dari alur file ini.
    /// </summary>
    public async Task InsertAsync(SecurityAuditLogDb log, CancellationToken ct)
    {
        const string sql = """
            insert into security_audit_logs (
                security_audit_log_id,
                occurred_at,
                trace_id,
                event_type,
                outcome,
                user_id,
                username,
                role,
                ip_address,
                user_agent,
                method,
                path,
                status_code,
                detail_json
            )
            values (
                @SecurityAuditLogId,
                @OccurredAt,
                @TraceId,
                @EventType,
                @Outcome,
                @UserId,
                @Username,
                @Role,
                @IpAddress,
                @UserAgent,
                @Method,
                @Path,
                @StatusCode,
                @DetailJson::jsonb
            );
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        await conn.ExecuteAsync(new CommandDefinition(sql, log, cancellationToken: ct));
    }

    /// <summary>
    /// Menjalankan fungsi ListRecentAsync sebagai bagian dari alur file ini.
    /// </summary>
    public async Task<List<SecurityAuditLogDb>> ListRecentAsync(int limit, string? eventType, Guid? userId, CancellationToken ct)
    {
        const string sql = """
            select
                security_audit_log_id,
                occurred_at,
                trace_id,
                event_type,
                outcome,
                user_id,
                username,
                role,
                ip_address,
                user_agent,
                method,
                path,
                status_code,
                detail_json
            from security_audit_logs
            where (@eventType is null or event_type = @eventType)
              and (@userId is null or user_id = @userId)
            order by occurred_at desc
            limit @limit;
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        var items = await conn.QueryAsync<SecurityAuditLogDb>(
            new CommandDefinition(
                sql,
                new
                {
                    limit,
                    eventType,
                    userId
                },
                cancellationToken: ct));
        return items.ToList();
    }
}
