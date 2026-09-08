// Fungsi file: Mengelola pemetaan dan akses PostgreSQL untuk MetricsRepository.
// Mengimpor namespace `Dapper` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Dapper;
// Mengimpor namespace `Npgsql` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Npgsql;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Data` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Data;

/// <summary>
/// Repository untuk penyimpanan snapshot metrik.
/// </summary>
// Mendefinisikan tipe class `MetricsRepository`; sealed mencegah tipe ini diturunkan lagi.
public sealed class MetricsRepository
// Membuka scope tipe MetricsRepository; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendeklarasikan field bertipe `NpgsqlDataSource`: `_dataSource` menyimpan sumber koneksi PostgreSQL yang mengelola pembuatan dan penggunaan ulang
    // koneksi. readonly membatasi penggantian referensi/nilai field pada deklarasi atau konstruktor.
    private readonly NpgsqlDataSource _dataSource;

    /// <summary>
    /// Menerima NpgsqlDataSource untuk koneksi ke tabel metric_snapshots.
    /// </summary>
    // Mendefinisikan konstruktor MetricsRepository yang menyiapkan objek dan menerima dependency/nilai awal dari pemanggil; parameter: Parameter
    // `dataSource` bertipe `NpgsqlDataSource` membawa sumber koneksi PostgreSQL yang mengelola pembuatan dan penggunaan ulang koneksi.
    public MetricsRepository(NpgsqlDataSource dataSource)
    // Membuka scope konstruktor MetricsRepository; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam MetricsRepository.
    {
        // Memperbarui `_dataSource` menggunakan `dataSource` (sumber koneksi PostgreSQL yang mengelola pembuatan dan penggunaan ulang koneksi) dalam
        // MetricsRepository.
        _dataSource = dataSource;
    // Menutup scope konstruktor MetricsRepository; bagian berikut berada di luar batas blok tersebut dalam MetricsRepository.
    }

    /// <summary>
    /// Menyisipkan batch snapshot metrik ke database.
    /// </summary>
    // Mendefinisikan metode `InsertSnapshotsAsync` dengan hasil bertipe `Task`. Menyisipkan batch snapshot metrik ke database. async memungkinkan
    // metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `snapshots` bertipe
    // `IEnumerable<MetricSnapshotDb>` membawa nilai snapshots; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat
    // dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task InsertSnapshotsAsync(IEnumerable<MetricSnapshotDb> snapshots, CancellationToken ct)
    // Membuka scope metode InsertSnapshotsAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam InsertSnapshotsAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai
        // adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string sql = ”””`.
        // Baris literal 2: INSERT INTO menetapkan tabel dan kolom tujuan penambahan rekaman: `insert into metric_snapshots (`.
        // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `metric_snapshot_id,`.
        // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `session_id,`.
        // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `user_id,`.
        // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `session_player_id,`.
        // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `computed_at,`.
        // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `metric_name,`.
        // Baris literal 9: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `metric_value_numeric,`.
        // Baris literal 10: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `metric_payload_json,`.
        // Baris literal 11: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_version_id,`.
        // Baris literal 12: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `last_event_id`.
        // Baris literal 13: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 14: VALUES menyediakan nilai baris baru sesuai urutan kolom INSERT; placeholder @ diikat ke parameter perintah: `values (`.
        // Baris literal 15: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@MetricSnapshotId,`.
        // Baris literal 16: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@SessionId,`.
        // Baris literal 17: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@UserId,`.
        // Baris literal 18: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@SessionPlayerId,`.
        // Baris literal 19: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@ComputedAt,`.
        // Baris literal 20: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@MetricName,`.
        // Baris literal 21: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@MetricValueNumeric,`.
        // Baris literal 22: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@MetricValueJson::jsonb,`.
        // Baris literal 23: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@RulesetVersionId,`.
        // Baris literal 24: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@LastEventId`.
        // Baris literal 25: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 26: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string sql = """
            insert into metric_snapshots (
                metric_snapshot_id,
                session_id,
                user_id,
                session_player_id,
                computed_at,
                metric_name,
                metric_value_numeric,
                metric_payload_json,
                ruleset_version_id,
                last_event_id
            )
            values (
                @MetricSnapshotId,
                @SessionId,
                @UserId,
                @SessionPlayerId,
                @ComputedAt,
                @MetricName,
                @MetricValueNumeric,
                @MetricValueJson::jsonb,
                @RulesetVersionId,
                @LastEventId
            )
            """;

        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition(sql, snapshots, cancellationToken:
        // ct)`; nilai hasil menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam
        // InsertSnapshotsAsync.
        await conn.ExecuteAsync(new CommandDefinition(sql, snapshots, cancellationToken: ct));
    // Menutup scope metode InsertSnapshotsAsync; bagian berikut berada di luar batas blok tersebut dalam InsertSnapshotsAsync.
    }

    /// <summary>
    /// Mengambil snapshot gameplay JSON terbaru (variabel mentah dan metrik turunan) per pemain.
    /// </summary>
    // Mendefinisikan metode `GetLatestGameplaySnapshotsAsync` dengan hasil bertipe `Task<List<MetricSnapshotJsonDb>>`. Mengambil snapshot gameplay JSON
    // terbaru (variabel mentah dan metrik turunan) per pemain. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan
    // penyelesaian melalui Task. Masukan: Parameter `sessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi
    // ini; Parameter `userId` bertipe `Guid` membawa identitas akun pengguna yang datanya sedang diproses; Parameter `ct` bertipe `CancellationToken`
    // membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<List<MetricSnapshotJsonDb>> GetLatestGameplaySnapshotsAsync(Guid sessionId, Guid userId, CancellationToken ct)
    // Membuka scope metode GetLatestGameplaySnapshotsAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // GetLatestGameplaySnapshotsAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai
        // adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string sql = ”””`.
        // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select distinct on (metric_name)`.
        // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `metric_name,`.
        // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `metric_payload_json::text as
        // metric_value_json,`.
        // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `computed_at`.
        // Baris literal 6: FROM memilih tabel/subquery sumber pembacaan: `from metric_snapshots`.
        // Baris literal 7: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where session_id = @sessionId`.
        // Baris literal 8: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and user_id = @userId`.
        // Baris literal 9: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and metric_name = any(@metricNames)`.
        // Baris literal 10: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by metric_name, computed_at desc`.
        // Baris literal 11: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string sql = """
            select distinct on (metric_name)
                   metric_name,
                   metric_payload_json::text as metric_value_json,
                   computed_at
            from metric_snapshots
            where session_id = @sessionId
              and user_id = @userId
              and metric_name = any(@metricNames)
            order by metric_name, computed_at desc
            """;

        // Menyiapkan variabel lokal `metricNames` untuk nilai metric nama dengan array baru dengan tipe elemen disimpulkan dari nilai initializer. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var metricNames = new[] { "gameplay.raw.variables", "gameplay.derived.metrics" };
        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Menyiapkan variabel lokal `items` untuk nilai elemen dengan hasil operasi asinkron menjalankan query baca melalui `conn` dengan `new
        // CommandDefinition(sql, new { sessionId, userId, metricNames }, cancellationToken: ct)` dan memetakan baris hasil ke tipe yang diminta; await
        // menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var items = await conn.QueryAsync<MetricSnapshotJsonDb>(
            // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen (sql, new { sessionId, userId, metricNames }, cancellationToken: ct) sebagai
            // argumen ke `conn.QueryAsync<MetricSnapshotJsonDb>`; Meneruskan `sql` (nilai SQL) sebagai argumen ke konstruktor `CommandDefinition`; Meneruskan
            // objek anonim yang mengelompokkan sessionId, userId, metricNames sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`; Meneruskan
            // `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen bernama
            // `cancellationToken`.
            new CommandDefinition(sql, new { sessionId, userId, metricNames }, cancellationToken: ct));
        // Mengembalikan mematerialisasi urutan `items` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori kepada pemanggil dalam
        // GetLatestGameplaySnapshotsAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return items.ToList();
    // Menutup scope metode GetLatestGameplaySnapshotsAsync; bagian berikut berada di luar batas blok tersebut dalam GetLatestGameplaySnapshotsAsync.
    }

    // Mendefinisikan metode `GetLatestMetricValuesAsync` dengan hasil bertipe `Task<List<MetricSnapshotValueDb>>`; operasi ini menangani get latest
    // metric nilai asinkron. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan:
    // Parameter `sessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini; Parameter `userId` bertipe `Guid`
    // membawa identitas akun pengguna yang datanya sedang diproses; Parameter `metricNames` bertipe `IReadOnlyCollection<string>` membawa nilai metric
    // nama; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan
    // atau aplikasi berhenti.
    public async Task<List<MetricSnapshotValueDb>> GetLatestMetricValuesAsync(Guid sessionId, Guid userId, IReadOnlyCollection<string> metricNames, CancellationToken ct)
    // Membuka scope metode GetLatestMetricValuesAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam GetLatestMetricValuesAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai
        // adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string sql = ”””`.
        // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select distinct on (metric_name)`.
        // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `metric_name,`.
        // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `metric_value_numeric,`.
        // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `computed_at`.
        // Baris literal 6: FROM memilih tabel/subquery sumber pembacaan: `from metric_snapshots`.
        // Baris literal 7: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where session_id = @sessionId`.
        // Baris literal 8: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and user_id = @userId`.
        // Baris literal 9: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and metric_name = any(@metricNames)`.
        // Baris literal 10: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by metric_name, computed_at desc`.
        // Baris literal 11: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string sql = """
            select distinct on (metric_name)
                   metric_name,
                   metric_value_numeric,
                   computed_at
            from metric_snapshots
            where session_id = @sessionId
              and user_id = @userId
              and metric_name = any(@metricNames)
            order by metric_name, computed_at desc
            """;

        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Menyiapkan variabel lokal `items` untuk nilai elemen dengan hasil operasi asinkron menjalankan query baca melalui `conn` dengan `new
        // CommandDefinition(sql, new { sessionId, userId, metricNames = metricNames.ToArray() }, cancellationToken: ct)` dan memetakan baris hasil ke tipe
        // yang diminta; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var items = await conn.QueryAsync<MetricSnapshotValueDb>(
            // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen (sql, new { sessionId, userId, metricNames = metricNames.ToArray() },
            // cancellationToken: ct) sebagai argumen ke `conn.QueryAsync<MetricSnapshotValueDb>`; Meneruskan `sql` (nilai SQL) sebagai argumen ke konstruktor
            // `CommandDefinition`; Meneruskan objek anonim yang mengelompokkan sessionId, userId, metricNames sebagai satu nilai sebagai argumen ke konstruktor
            // `CommandDefinition`; Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi
            // berhenti) sebagai argumen bernama `cancellationToken`.
            new CommandDefinition(sql, new { sessionId, userId, metricNames = metricNames.ToArray() }, cancellationToken: ct));
        // Mengembalikan mematerialisasi urutan `items` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori kepada pemanggil dalam
        // GetLatestMetricValuesAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return items.ToList();
    // Menutup scope metode GetLatestMetricValuesAsync; bagian berikut berada di luar batas blok tersebut dalam GetLatestMetricValuesAsync.
    }

    /// <summary>
    /// Mengambil nilai numerik terbaru dari metrik tertentu per pemain.
    /// </summary>
    // Mendefinisikan metode `GetLatestMetricNumericAsync` dengan hasil bertipe `Task<double?>`. Mengambil nilai numerik terbaru dari metrik tertentu
    // per pemain. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter
    // `sessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini; Parameter `userId` bertipe `Guid` membawa
    // identitas akun pengguna yang datanya sedang diproses; Parameter `metricName` bertipe `string` membawa nilai metric nama; Parameter `ct` bertipe
    // `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<double?> GetLatestMetricNumericAsync(Guid sessionId, Guid userId, string metricName, CancellationToken ct)
    // Membuka scope metode GetLatestMetricNumericAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam GetLatestMetricNumericAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai
        // adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string sql = ”””`.
        // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select metric_value_numeric`.
        // Baris literal 3: FROM memilih tabel/subquery sumber pembacaan: `from metric_snapshots`.
        // Baris literal 4: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where session_id = @sessionId`.
        // Baris literal 5: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and user_id = @userId`.
        // Baris literal 6: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and metric_name = @metricName`.
        // Baris literal 7: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and metric_value_numeric is not null`.
        // Baris literal 8: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by computed_at desc`.
        // Baris literal 9: LIMIT membatasi jumlah baris yang dikembalikan query: `limit 1`.
        // Baris literal 10: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string sql = """
            select metric_value_numeric
            from metric_snapshots
            where session_id = @sessionId
              and user_id = @userId
              and metric_name = @metricName
              and metric_value_numeric is not null
            order by computed_at desc
            limit 1
            """;

        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Mengembalikan hasil operasi asinkron menjalankan perintah basis data melalui `conn` dengan `new CommandDefinition(sql, new { sessionId, userId,
        // metricName }, cancellationToken: ct)` dan mengambil nilai skalar hasilnya; await menunggu hasil tanpa memblokir thread selama operasi belum
        // selesai kepada pemanggil dalam GetLatestMetricNumericAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return await conn.ExecuteScalarAsync<double?>(
            // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen (sql, new { sessionId, userId, metricName }, cancellationToken: ct) sebagai
            // argumen ke `conn.ExecuteScalarAsync<double?>`; Meneruskan `sql` (nilai SQL) sebagai argumen ke konstruktor `CommandDefinition`; Meneruskan objek
            // anonim yang mengelompokkan sessionId, userId, metricName sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`; Meneruskan `ct`
            // (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen bernama
            // `cancellationToken`.
            new CommandDefinition(sql, new { sessionId, userId, metricName }, cancellationToken: ct));
    // Menutup scope metode GetLatestMetricNumericAsync; bagian berikut berada di luar batas blok tersebut dalam GetLatestMetricNumericAsync.
    }
// Menutup scope tipe MetricsRepository; bagian berikut berada di luar batas blok tersebut.
}
