// Fungsi file: Mengelola pemetaan dan akses PostgreSQL untuk SecurityAuditRepository.
// Mengimpor namespace `Dapper` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Dapper;
// Mengimpor namespace `Npgsql` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Npgsql;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Data` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Data;

/// <summary>
/// Repository untuk menyimpan dan membaca audit log keamanan.
/// </summary>
// Mendefinisikan tipe class `SecurityAuditRepository`; sealed mencegah tipe ini diturunkan lagi.
public sealed class SecurityAuditRepository
{
    private readonly NpgsqlDataSource _dataSource;

    /// <summary>
    /// Menerima NpgsqlDataSource untuk koneksi ke tabel security_audit_logs.
    /// </summary>
    // Mendefinisikan konstruktor SecurityAuditRepository yang menyiapkan objek dan menerima dependency/nilai awal dari pemanggil; parameter: Parameter
    // `dataSource` bertipe `NpgsqlDataSource` membawa sumber koneksi PostgreSQL yang mengelola pembuatan dan penggunaan ulang koneksi.
    public SecurityAuditRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    /// <summary>
    /// Menyisipkan satu record audit log keamanan ke database.
    /// </summary>
    // Mendefinisikan metode `InsertAsync` dengan hasil bertipe `Task`. Menyisipkan satu record audit log keamanan ke database. async memungkinkan
    // metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `log` bertipe `SecurityAuditLogDb`
    // membawa nilai log; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil
    // membatalkan permintaan atau aplikasi berhenti.
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
    /// Mengambil daftar audit log terbaru dengan filter opsional event_type dan user_id.
    /// </summary>
    // Mendefinisikan metode `ListRecentAsync` dengan hasil bertipe `Task<List<SecurityAuditLogDb>>`. Mengambil daftar audit log terbaru dengan filter
    // opsional event_type dan user_id. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task.
    // Masukan: Parameter `limit` bertipe `int` membawa batas jumlah hasil yang diminta pada satu operasi; Parameter `eventType` bertipe `string?`
    // membawa jenis aktivitas yang menentukan aturan validasi dan proyeksi event; nilai null diizinkan ketika data opsional belum tersedia; Parameter
    // `userId` bertipe `Guid?` membawa identitas akun pengguna yang datanya sedang diproses; nilai null diizinkan ketika data opsional belum tersedia;
    // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
    // aplikasi berhenti.
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
                detail_json::text as detail_json
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
