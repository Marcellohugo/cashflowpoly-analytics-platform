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
// Membuka scope tipe SecurityAuditRepository; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendeklarasikan field bertipe `NpgsqlDataSource`: `_dataSource` menyimpan sumber koneksi PostgreSQL yang mengelola pembuatan dan penggunaan ulang
    // koneksi. readonly membatasi penggantian referensi/nilai field pada deklarasi atau konstruktor.
    private readonly NpgsqlDataSource _dataSource;

    /// <summary>
    /// Menerima NpgsqlDataSource untuk koneksi ke tabel security_audit_logs.
    /// </summary>
    // Mendefinisikan konstruktor SecurityAuditRepository yang menyiapkan objek dan menerima dependency/nilai awal dari pemanggil; parameter: Parameter
    // `dataSource` bertipe `NpgsqlDataSource` membawa sumber koneksi PostgreSQL yang mengelola pembuatan dan penggunaan ulang koneksi.
    public SecurityAuditRepository(NpgsqlDataSource dataSource)
    // Membuka scope konstruktor SecurityAuditRepository; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam SecurityAuditRepository.
    {
        // Memperbarui `_dataSource` menggunakan `dataSource` (sumber koneksi PostgreSQL yang mengelola pembuatan dan penggunaan ulang koneksi) dalam
        // SecurityAuditRepository.
        _dataSource = dataSource;
    // Menutup scope konstruktor SecurityAuditRepository; bagian berikut berada di luar batas blok tersebut dalam SecurityAuditRepository.
    }

    /// <summary>
    /// Menyisipkan satu record audit log keamanan ke database.
    /// </summary>
    // Mendefinisikan metode `InsertAsync` dengan hasil bertipe `Task`. Menyisipkan satu record audit log keamanan ke database. async memungkinkan
    // metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `log` bertipe `SecurityAuditLogDb`
    // membawa nilai log; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil
    // membatalkan permintaan atau aplikasi berhenti.
    public async Task InsertAsync(SecurityAuditLogDb log, CancellationToken ct)
    // Membuka scope metode InsertAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam InsertAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai
        // adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string sql = ”””`.
        // Baris literal 2: INSERT INTO menetapkan tabel dan kolom tujuan penambahan rekaman: `insert into security_audit_logs (`.
        // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `security_audit_log_id,`.
        // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `occurred_at,`.
        // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `trace_id,`.
        // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `event_type,`.
        // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `outcome,`.
        // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `user_id,`.
        // Baris literal 9: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `username,`.
        // Baris literal 10: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `role,`.
        // Baris literal 11: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ip_address,`.
        // Baris literal 12: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `user_agent,`.
        // Baris literal 13: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `method,`.
        // Baris literal 14: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `path,`.
        // Baris literal 15: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `status_code,`.
        // Baris literal 16: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `detail_json`.
        // Baris literal 17: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 18: VALUES menyediakan nilai baris baru sesuai urutan kolom INSERT; placeholder @ diikat ke parameter perintah: `values (`.
        // Baris literal 19: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@SecurityAuditLogId,`.
        // Baris literal 20: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@OccurredAt,`.
        // Baris literal 21: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@TraceId,`.
        // Baris literal 22: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@EventType,`.
        // Baris literal 23: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@Outcome,`.
        // Baris literal 24: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@UserId,`.
        // Baris literal 25: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@Username,`.
        // Baris literal 26: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@Role,`.
        // Baris literal 27: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@IpAddress,`.
        // Baris literal 28: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@UserAgent,`.
        // Baris literal 29: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@Method,`.
        // Baris literal 30: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@Path,`.
        // Baris literal 31: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@StatusCode,`.
        // Baris literal 32: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@DetailJson::jsonb`.
        // Baris literal 33: Pembatas literal/penutup `);`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 34: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
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

        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition(sql, log, cancellationToken: ct)`;
        // nilai hasil menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam
        // InsertAsync.
        await conn.ExecuteAsync(new CommandDefinition(sql, log, cancellationToken: ct));
    // Menutup scope metode InsertAsync; bagian berikut berada di luar batas blok tersebut dalam InsertAsync.
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
    // Membuka scope metode ListRecentAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ListRecentAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai
        // adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string sql = ”””`.
        // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
        // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `security_audit_log_id,`.
        // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `occurred_at,`.
        // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `trace_id,`.
        // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `event_type,`.
        // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `outcome,`.
        // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `user_id,`.
        // Baris literal 9: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `username,`.
        // Baris literal 10: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `role,`.
        // Baris literal 11: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ip_address,`.
        // Baris literal 12: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `user_agent,`.
        // Baris literal 13: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `method,`.
        // Baris literal 14: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `path,`.
        // Baris literal 15: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `status_code,`.
        // Baris literal 16: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `detail_json::text as
        // detail_json`.
        // Baris literal 17: FROM memilih tabel/subquery sumber pembacaan: `from security_audit_logs`.
        // Baris literal 18: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where (@eventType is null or event_type = @eventType)`.
        // Baris literal 19: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and (@userId is null or user_id = @userId)`.
        // Baris literal 20: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by occurred_at desc`.
        // Baris literal 21: LIMIT membatasi jumlah baris yang dikembalikan query: `limit @limit;`.
        // Baris literal 22: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
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

        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Menyiapkan variabel lokal `items` untuk nilai elemen dengan hasil operasi asinkron menjalankan query baca melalui `conn` dengan `new
        // CommandDefinition( sql, new { limit, eventType, userId }, cancellationToken: ct)` dan memetakan baris hasil ke tipe yang diminta; await menunggu
        // hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var items = await conn.QueryAsync<SecurityAuditLogDb>(
            // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen ( sql, new { limit, eventType, userId }, cancellationToken: ct) sebagai argumen
            // ke `conn.QueryAsync<SecurityAuditLogDb>`.
            new CommandDefinition(
                // Meneruskan `sql` (nilai SQL) sebagai argumen ke konstruktor `CommandDefinition`.
                sql,
                // Meneruskan objek anonim yang mengelompokkan limit, eventType, userId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                new
                // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ListRecentAsync.
                {
                    // Meneruskan objek anonim yang mengelompokkan limit, eventType, userId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                    limit,
                    // Meneruskan objek anonim yang mengelompokkan limit, eventType, userId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                    eventType,
                    // Meneruskan objek anonim yang mengelompokkan limit, eventType, userId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                    userId
                // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam ListRecentAsync.
                },
                // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                // bernama `cancellationToken`.
                cancellationToken: ct));
        // Mengembalikan mematerialisasi urutan `items` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori kepada pemanggil dalam
        // ListRecentAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return items.ToList();
    // Menutup scope metode ListRecentAsync; bagian berikut berada di luar batas blok tersebut dalam ListRecentAsync.
    }
// Menutup scope tipe SecurityAuditRepository; bagian berikut berada di luar batas blok tersebut.
}
