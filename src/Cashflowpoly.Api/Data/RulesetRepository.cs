// Fungsi file: Mengelola pemetaan dan akses PostgreSQL untuk RulesetRepository.
// Mengimpor namespace `System.Security.Cryptography` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Security.Cryptography;
// Mengimpor namespace `System.Data.Common` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Data.Common;
// Mengimpor namespace `System.Text` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text;
// Mengimpor namespace `System.Text.Json` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.Json;
// Mengimpor namespace `Cashflowpoly.Api.Domain` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Domain;
// Mengimpor namespace `Cashflowpoly.Api.Contracts` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Contracts;
// Mengimpor namespace `Dapper` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Dapper;
// Mengimpor namespace `Npgsql` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Npgsql;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Data` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Data;

/// <summary>
/// Repository untuk ruleset dan versi ruleset.
/// </summary>
// Mendefinisikan tipe class `RulesetRepository`; sealed mencegah tipe ini diturunkan lagi.
public sealed class RulesetRepository
// Membuka scope tipe RulesetRepository; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendeklarasikan field bertipe `NpgsqlDataSource`: `_dataSource` menyimpan sumber koneksi PostgreSQL yang mengelola pembuatan dan penggunaan ulang
    // koneksi. readonly membatasi penggantian referensi/nilai field pada deklarasi atau konstruktor.
    private readonly NpgsqlDataSource _dataSource;

    /// <summary>
    /// Menerima NpgsqlDataSource untuk koneksi ke tabel rulesets dan ruleset_versions.
    /// </summary>
    // Mendefinisikan konstruktor RulesetRepository yang menyiapkan objek dan menerima dependency/nilai awal dari pemanggil; parameter: Parameter
    // `dataSource` bertipe `NpgsqlDataSource` membawa sumber koneksi PostgreSQL yang mengelola pembuatan dan penggunaan ulang koneksi.
    public RulesetRepository(NpgsqlDataSource dataSource)
    // Membuka scope konstruktor RulesetRepository; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam RulesetRepository.
    {
        // Memperbarui `_dataSource` menggunakan `dataSource` (sumber koneksi PostgreSQL yang mengelola pembuatan dan penggunaan ulang koneksi) dalam
        // RulesetRepository.
        _dataSource = dataSource;
    // Menutup scope konstruktor RulesetRepository; bagian berikut berada di luar batas blok tersebut dalam RulesetRepository.
    }

    /// <summary>
    /// Mengambil data ruleset berdasarkan ruleset_id.
    /// </summary>
    // Mendefinisikan metode `GetRulesetAsync` dengan hasil bertipe `Task<RulesetDb?>`. Mengambil data ruleset berdasarkan ruleset_id. async
    // memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `rulesetId` bertipe `Guid`
    // membawa identitas kumpulan aturan permainan; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan
    // ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<RulesetDb?> GetRulesetAsync(Guid rulesetId, CancellationToken ct)
    // Membuka scope metode GetRulesetAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam GetRulesetAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai
        // adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string sql = ”””`.
        // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select ruleset_id, name, description, instructor_user_id,
        // is_archived, archived_at, created_at, created_by_user_id`.
        // Baris literal 3: FROM memilih tabel/subquery sumber pembacaan: `from rulesets`.
        // Baris literal 4: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where ruleset_id = @rulesetId`.
        // Baris literal 5: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and not is_archived`.
        // Baris literal 6: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string sql = """
            select ruleset_id, name, description, instructor_user_id, is_archived, archived_at, created_at, created_by_user_id
            from rulesets
            where ruleset_id = @rulesetId
              and not is_archived
            """;

        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Mengembalikan hasil operasi asinkron membaca satu hasil basis data melalui `conn.QuerySingleOrDefaultAsync<RulesetDb>` dengan `new
        // CommandDefinition(sql, new { rulesetId }, cancellationToken: ct)`; nilai default menunjukkan tidak ada baris hasil; await menunggu hasil tanpa
        // memblokir thread selama operasi belum selesai kepada pemanggil dalam GetRulesetAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return await conn.QuerySingleOrDefaultAsync<RulesetDb>(new CommandDefinition(sql, new { rulesetId }, cancellationToken: ct));
    // Menutup scope metode GetRulesetAsync; bagian berikut berada di luar batas blok tersebut dalam GetRulesetAsync.
    }

    /// <summary>
    /// Mengambil data ruleset yang dimiliki instruktur tertentu.
    /// </summary>
    // Mendefinisikan metode `GetRulesetForInstructorAsync` dengan hasil bertipe `Task<RulesetDb?>`. Mengambil data ruleset yang dimiliki instruktur
    // tertentu. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `rulesetId`
    // bertipe `Guid` membawa identitas kumpulan aturan permainan; Parameter `instructorUserId` bertipe `Guid` membawa identitas instruktur pemilik sesi
    // atau aturan; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan
    // permintaan atau aplikasi berhenti.
    public async Task<RulesetDb?> GetRulesetForInstructorAsync(Guid rulesetId, Guid instructorUserId, CancellationToken ct)
    // Membuka scope metode GetRulesetForInstructorAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // GetRulesetForInstructorAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai
        // adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string sql = ”””`.
        // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select ruleset_id, name, description, instructor_user_id,
        // is_archived, archived_at, created_at, created_by_user_id`.
        // Baris literal 3: FROM memilih tabel/subquery sumber pembacaan: `from rulesets`.
        // Baris literal 4: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where ruleset_id = @rulesetId`.
        // Baris literal 5: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and instructor_user_id = @instructorUserId`.
        // Baris literal 6: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and not is_archived`.
        // Baris literal 7: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string sql = """
            select ruleset_id, name, description, instructor_user_id, is_archived, archived_at, created_at, created_by_user_id
            from rulesets
            where ruleset_id = @rulesetId
              and instructor_user_id = @instructorUserId
              and not is_archived
            """;

        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Mengembalikan hasil operasi asinkron membaca satu hasil basis data melalui `conn.QuerySingleOrDefaultAsync<RulesetDb>` dengan `new
        // CommandDefinition(sql, new { rulesetId, instructorUserId }, cancellationToken: ct)`; nilai default menunjukkan tidak ada baris hasil; await
        // menunggu hasil tanpa memblokir thread selama operasi belum selesai kepada pemanggil dalam GetRulesetForInstructorAsync; eksekusi jalur ini
        // selesai setelah nilai hasil ditentukan.
        return await conn.QuerySingleOrDefaultAsync<RulesetDb>(
            // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen (sql, new { rulesetId, instructorUserId }, cancellationToken: ct) sebagai
            // argumen ke `conn.QuerySingleOrDefaultAsync<RulesetDb>`; Meneruskan `sql` (nilai SQL) sebagai argumen ke konstruktor `CommandDefinition`;
            // Meneruskan objek anonim yang mengelompokkan rulesetId, instructorUserId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`;
            // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
            // bernama `cancellationToken`.
            new CommandDefinition(sql, new { rulesetId, instructorUserId }, cancellationToken: ct));
    // Menutup scope metode GetRulesetForInstructorAsync; bagian berikut berada di luar batas blok tersebut dalam GetRulesetForInstructorAsync.
    }

    /// <summary>
    /// Mengambil ruleset yang boleh dipakai untuk membuat sesi: ruleset bawaan
    /// atau ruleset milik instruktur yang sedang masuk.
    /// </summary>
    // Mendefinisikan metode `GetRulesetForSessionAsync` dengan hasil bertipe `Task<RulesetDb?>`. Mengambil ruleset yang boleh dipakai untuk membuat
    // sesi: ruleset bawaan atau ruleset milik instruktur yang sedang masuk. async memungkinkan metode menunggu operasi I/O dengan await dan
    // mengembalikan penyelesaian melalui Task. Masukan: Parameter `rulesetId` bertipe `Guid` membawa identitas kumpulan aturan permainan; Parameter
    // `instructorUserId` bertipe `Guid` membawa identitas instruktur pemilik sesi atau aturan; Parameter `ct` bertipe `CancellationToken` membawa
    // sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<RulesetDb?> GetRulesetForSessionAsync(Guid rulesetId, Guid instructorUserId, CancellationToken ct)
    // Membuka scope metode GetRulesetForSessionAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam GetRulesetForSessionAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai
        // adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string sql = ”””`.
        // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select ruleset_id, name, description, instructor_user_id,
        // is_archived, archived_at, created_at, created_by_user_id`.
        // Baris literal 3: FROM memilih tabel/subquery sumber pembacaan: `from rulesets`.
        // Baris literal 4: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where ruleset_id = @rulesetId`.
        // Baris literal 5: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and (instructor_user_id is null or instructor_user_id =
        // @instructorUserId)`.
        // Baris literal 6: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and not is_archived`.
        // Baris literal 7: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string sql = """
            select ruleset_id, name, description, instructor_user_id, is_archived, archived_at, created_at, created_by_user_id
            from rulesets
            where ruleset_id = @rulesetId
              and (instructor_user_id is null or instructor_user_id = @instructorUserId)
              and not is_archived
            """;

        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Mengembalikan hasil operasi asinkron membaca satu hasil basis data melalui `conn.QuerySingleOrDefaultAsync<RulesetDb>` dengan `new
        // CommandDefinition(sql, new { rulesetId, instructorUserId }, cancellationToken: ct)`; nilai default menunjukkan tidak ada baris hasil; await
        // menunggu hasil tanpa memblokir thread selama operasi belum selesai kepada pemanggil dalam GetRulesetForSessionAsync; eksekusi jalur ini selesai
        // setelah nilai hasil ditentukan.
        return await conn.QuerySingleOrDefaultAsync<RulesetDb>(
            // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen (sql, new { rulesetId, instructorUserId }, cancellationToken: ct) sebagai
            // argumen ke `conn.QuerySingleOrDefaultAsync<RulesetDb>`; Meneruskan `sql` (nilai SQL) sebagai argumen ke konstruktor `CommandDefinition`;
            // Meneruskan objek anonim yang mengelompokkan rulesetId, instructorUserId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`;
            // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
            // bernama `cancellationToken`.
            new CommandDefinition(sql, new { rulesetId, instructorUserId }, cancellationToken: ct));
    // Menutup scope metode GetRulesetForSessionAsync; bagian berikut berada di luar batas blok tersebut dalam GetRulesetForSessionAsync.
    }

    /// <summary>
    /// Mengambil versi terbaru (nomor tertinggi) dari ruleset tertentu.
    /// </summary>
    // Mendefinisikan metode `GetLatestVersionAsync` dengan hasil bertipe `Task<RulesetVersionDb?>`. Mengambil versi terbaru (nomor tertinggi) dari
    // ruleset tertentu. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter
    // `rulesetId` bertipe `Guid` membawa identitas kumpulan aturan permainan; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar
    // operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<RulesetVersionDb?> GetLatestVersionAsync(Guid rulesetId, CancellationToken ct)
    // Membuka scope metode GetLatestVersionAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam GetLatestVersionAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai
        // adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string sql = ”””`.
        // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select ruleset_version_id, ruleset_id, version, status, mode,
        // config_hash, created_at, created_by_user_id`.
        // Baris literal 3: FROM memilih tabel/subquery sumber pembacaan: `from ruleset_versions`.
        // Baris literal 4: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where ruleset_id = @rulesetId`.
        // Baris literal 5: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by version desc`.
        // Baris literal 6: LIMIT membatasi jumlah baris yang dikembalikan query: `limit 1`.
        // Baris literal 7: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string sql = """
            select ruleset_version_id, ruleset_id, version, status, mode, config_hash, created_at, created_by_user_id
            from ruleset_versions
            where ruleset_id = @rulesetId
            order by version desc
            limit 1
            """;

        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Menyiapkan variabel lokal `version` untuk nomor versi yang dipakai untuk konsistensi data atau konfigurasi dengan hasil operasi asinkron membaca
        // satu hasil basis data melalui `conn.QuerySingleOrDefaultAsync<RulesetVersionDb>` dengan `new CommandDefinition(sql, new { rulesetId },
        // cancellationToken: ct)`; nilai default menunjukkan tidak ada baris hasil; await menunggu hasil tanpa memblokir thread selama operasi belum
        // selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var version = await conn.QuerySingleOrDefaultAsync<RulesetVersionDb>(new CommandDefinition(sql, new { rulesetId }, cancellationToken: ct));
        // Memeriksa hasil pencocokan `version` dengan pola `not null`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // GetLatestVersionAsync.
        if (version is not null)
        // Membuka scope cabang if untuk kondisi `version is not null`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // GetLatestVersionAsync.
        {
            // Memperbarui `version.Definition` menggunakan hasil operasi asinkron memanggil `ReadRulesetDefinitionAsync` dengan `conn`,
            // `version.RulesetVersionId`, `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam GetLatestVersionAsync.
            version.Definition = await ReadRulesetDefinitionAsync(conn, version.RulesetVersionId, ct);
        // Menutup scope cabang if untuk kondisi `version is not null`; bagian berikut berada di luar batas blok tersebut dalam GetLatestVersionAsync.
        }

        // Mengembalikan `version` (nomor versi yang dipakai untuk konsistensi data atau konfigurasi) kepada pemanggil dalam GetLatestVersionAsync; eksekusi
        // jalur ini selesai setelah nilai hasil ditentukan.
        return version;
    // Menutup scope metode GetLatestVersionAsync; bagian berikut berada di luar batas blok tersebut dalam GetLatestVersionAsync.
    }

    /// <summary>
    /// Mengambil versi ACTIVE terbaru dari ruleset tertentu.
    /// </summary>
    // Mendefinisikan metode `GetLatestActiveVersionAsync` dengan hasil bertipe `Task<RulesetVersionDb?>`. Mengambil versi ACTIVE terbaru dari ruleset
    // tertentu. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `rulesetId`
    // bertipe `Guid` membawa identitas kumpulan aturan permainan; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi
    // dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<RulesetVersionDb?> GetLatestActiveVersionAsync(Guid rulesetId, CancellationToken ct)
    // Membuka scope metode GetLatestActiveVersionAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam GetLatestActiveVersionAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai
        // adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string sql = ”””`.
        // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select ruleset_version_id, ruleset_id, version, status, mode,
        // config_hash, created_at, created_by_user_id`.
        // Baris literal 3: FROM memilih tabel/subquery sumber pembacaan: `from ruleset_versions`.
        // Baris literal 4: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where ruleset_id = @rulesetId`.
        // Baris literal 5: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and status = 'ACTIVE'`.
        // Baris literal 6: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by version desc`.
        // Baris literal 7: LIMIT membatasi jumlah baris yang dikembalikan query: `limit 1`.
        // Baris literal 8: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string sql = """
            select ruleset_version_id, ruleset_id, version, status, mode, config_hash, created_at, created_by_user_id
            from ruleset_versions
            where ruleset_id = @rulesetId
              and status = 'ACTIVE'
            order by version desc
            limit 1
            """;

        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Menyiapkan variabel lokal `version` untuk nomor versi yang dipakai untuk konsistensi data atau konfigurasi dengan hasil operasi asinkron membaca
        // satu hasil basis data melalui `conn.QuerySingleOrDefaultAsync<RulesetVersionDb>` dengan `new CommandDefinition(sql, new { rulesetId },
        // cancellationToken: ct)`; nilai default menunjukkan tidak ada baris hasil; await menunggu hasil tanpa memblokir thread selama operasi belum
        // selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var version = await conn.QuerySingleOrDefaultAsync<RulesetVersionDb>(new CommandDefinition(sql, new { rulesetId }, cancellationToken: ct));
        // Memeriksa hasil pencocokan `version` dengan pola `not null`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // GetLatestActiveVersionAsync.
        if (version is not null)
        // Membuka scope cabang if untuk kondisi `version is not null`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // GetLatestActiveVersionAsync.
        {
            // Memperbarui `version.Definition` menggunakan hasil operasi asinkron memanggil `ReadRulesetDefinitionAsync` dengan `conn`,
            // `version.RulesetVersionId`, `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam GetLatestActiveVersionAsync.
            version.Definition = await ReadRulesetDefinitionAsync(conn, version.RulesetVersionId, ct);
        // Menutup scope cabang if untuk kondisi `version is not null`; bagian berikut berada di luar batas blok tersebut dalam GetLatestActiveVersionAsync.
        }

        // Mengembalikan `version` (nomor versi yang dipakai untuk konsistensi data atau konfigurasi) kepada pemanggil dalam GetLatestActiveVersionAsync;
        // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return version;
    // Menutup scope metode GetLatestActiveVersionAsync; bagian berikut berada di luar batas blok tersebut dalam GetLatestActiveVersionAsync.
    }

    /// <summary>
    /// Mengambil versi spesifik dari ruleset berdasarkan nomor versi.
    /// </summary>
    // Mendefinisikan metode `GetRulesetVersionAsync` dengan hasil bertipe `Task<RulesetVersionDb?>`. Mengambil versi spesifik dari ruleset berdasarkan
    // nomor versi. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter
    // `rulesetId` bertipe `Guid` membawa identitas kumpulan aturan permainan; Parameter `version` bertipe `int` membawa nomor versi yang dipakai untuk
    // konsistensi data atau konfigurasi; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika
    // pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<RulesetVersionDb?> GetRulesetVersionAsync(Guid rulesetId, int version, CancellationToken ct)
    // Membuka scope metode GetRulesetVersionAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam GetRulesetVersionAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai
        // adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string sql = ”””`.
        // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select ruleset_version_id, ruleset_id, version, status, mode,
        // config_hash, created_at, created_by_user_id`.
        // Baris literal 3: FROM memilih tabel/subquery sumber pembacaan: `from ruleset_versions`.
        // Baris literal 4: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where ruleset_id = @rulesetId and version = @version`.
        // Baris literal 5: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string sql = """
            select ruleset_version_id, ruleset_id, version, status, mode, config_hash, created_at, created_by_user_id
            from ruleset_versions
            where ruleset_id = @rulesetId and version = @version
            """;

        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Menyiapkan variabel lokal `row` untuk nilai baris dengan hasil operasi asinkron membaca satu hasil basis data melalui
        // `conn.QuerySingleOrDefaultAsync<RulesetVersionDb>` dengan `new CommandDefinition(sql, new { rulesetId, version }, cancellationToken: ct)`; nilai
        // default menunjukkan tidak ada baris hasil; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var row = await conn.QuerySingleOrDefaultAsync<RulesetVersionDb>(new CommandDefinition(sql, new { rulesetId, version }, cancellationToken: ct));
        // Memeriksa hasil pencocokan `row` dengan pola `not null`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam GetRulesetVersionAsync.
        if (row is not null)
        // Membuka scope cabang if untuk kondisi `row is not null`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // GetRulesetVersionAsync.
        {
            // Memperbarui `row.Definition` menggunakan hasil operasi asinkron memanggil `ReadRulesetDefinitionAsync` dengan `conn`, `row.RulesetVersionId`,
            // `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam GetRulesetVersionAsync.
            row.Definition = await ReadRulesetDefinitionAsync(conn, row.RulesetVersionId, ct);
        // Menutup scope cabang if untuk kondisi `row is not null`; bagian berikut berada di luar batas blok tersebut dalam GetRulesetVersionAsync.
        }

        // Mengembalikan `row` (nilai baris) kepada pemanggil dalam GetRulesetVersionAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return row;
    // Menutup scope metode GetRulesetVersionAsync; bagian berikut berada di luar batas blok tersebut dalam GetRulesetVersionAsync.
    }

    /// <summary>
    /// Mengambil versi ruleset berdasarkan ruleset_version_id.
    /// </summary>
    // Mendefinisikan metode `GetRulesetVersionByIdAsync` dengan hasil bertipe `Task<RulesetVersionDb?>`. Mengambil versi ruleset berdasarkan
    // ruleset_version_id. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter
    // `rulesetVersionId` bertipe `Guid` membawa identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang tepat; Parameter `ct`
    // bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi
    // berhenti.
    public async Task<RulesetVersionDb?> GetRulesetVersionByIdAsync(Guid rulesetVersionId, CancellationToken ct)
    // Membuka scope metode GetRulesetVersionByIdAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam GetRulesetVersionByIdAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai
        // adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string sql = ”””`.
        // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select ruleset_version_id, ruleset_id, version, status, mode,
        // config_hash, created_at, created_by_user_id`.
        // Baris literal 3: FROM memilih tabel/subquery sumber pembacaan: `from ruleset_versions`.
        // Baris literal 4: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where ruleset_version_id = @rulesetVersionId`.
        // Baris literal 5: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string sql = """
            select ruleset_version_id, ruleset_id, version, status, mode, config_hash, created_at, created_by_user_id
            from ruleset_versions
            where ruleset_version_id = @rulesetVersionId
            """;

        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Menyiapkan variabel lokal `row` untuk nilai baris dengan hasil operasi asinkron membaca satu hasil basis data melalui
        // `conn.QuerySingleOrDefaultAsync<RulesetVersionDb>` dengan `new CommandDefinition(sql, new { rulesetVersionId }, cancellationToken: ct)`; nilai
        // default menunjukkan tidak ada baris hasil; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var row = await conn.QuerySingleOrDefaultAsync<RulesetVersionDb>(new CommandDefinition(sql, new { rulesetVersionId }, cancellationToken: ct));
        // Memeriksa hasil pencocokan `row` dengan pola `not null`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // GetRulesetVersionByIdAsync.
        if (row is not null)
        // Membuka scope cabang if untuk kondisi `row is not null`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // GetRulesetVersionByIdAsync.
        {
            // Memperbarui `row.Definition` menggunakan hasil operasi asinkron memanggil `ReadRulesetDefinitionAsync` dengan `conn`, `row.RulesetVersionId`,
            // `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam GetRulesetVersionByIdAsync.
            row.Definition = await ReadRulesetDefinitionAsync(conn, row.RulesetVersionId, ct);
        // Menutup scope cabang if untuk kondisi `row is not null`; bagian berikut berada di luar batas blok tersebut dalam GetRulesetVersionByIdAsync.
        }

        // Mengembalikan `row` (nilai baris) kepada pemanggil dalam GetRulesetVersionByIdAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return row;
    // Menutup scope metode GetRulesetVersionByIdAsync; bagian berikut berada di luar batas blok tersebut dalam GetRulesetVersionByIdAsync.
    }

    /// <summary>
    /// Membuat ruleset baru beserta versi pertama (v1 ACTIVE) dalam transaksi.
    /// </summary>
    // Mendefinisikan metode `CreateRulesetAsync` dengan hasil bertipe `Task<(Guid RulesetId, Guid RulesetVersionId, int Version)>`. Membuat ruleset
    // baru beserta versi pertama (v1 ACTIVE) dalam transaksi. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan
    // penyelesaian melalui Task. Masukan: Parameter `name` bertipe `string` membawa nilai nama; Parameter `description` bertipe `string?` membawa nilai
    // description; nilai null diizinkan ketika data opsional belum tersedia; Parameter `instructorUserId` bertipe `Guid` membawa identitas instruktur
    // pemilik sesi atau aturan; Parameter `definition` bertipe `RulesetDefinitionDto` membawa definisi terstruktur komponen serta parameter aturan
    // permainan; Parameter `createdByUserId` bertipe `Guid?` membawa nilai created berdasarkan pengguna identitas; nilai null diizinkan ketika data
    // opsional belum tersedia; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil
    // membatalkan permintaan atau aplikasi berhenti.
    public async Task<(Guid RulesetId, Guid RulesetVersionId, int Version)> CreateRulesetAsync(
        // Parameter `name` bertipe `string` membawa nilai nama.
        string name,
        // Parameter `description` bertipe `string?` membawa nilai description; nilai null diizinkan ketika data opsional belum tersedia.
        string? description,
        // Parameter `instructorUserId` bertipe `Guid` membawa identitas instruktur pemilik sesi atau aturan.
        Guid instructorUserId,
        // Parameter `definition` bertipe `RulesetDefinitionDto` membawa definisi terstruktur komponen serta parameter aturan permainan.
        RulesetDefinitionDto definition,
        // Parameter `createdByUserId` bertipe `Guid?` membawa nilai created berdasarkan pengguna identitas; nilai null diizinkan ketika data opsional belum
        // tersedia.
        Guid? createdByUserId,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    // Membuka scope metode CreateRulesetAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CreateRulesetAsync.
    {
        // Menyiapkan variabel lokal `rulesetId` untuk identitas kumpulan aturan permainan dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var rulesetId = Guid.NewGuid();
        // Menyiapkan variabel lokal `rulesetVersionId` untuk identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang tepat dengan
        // memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var rulesetVersionId = Guid.NewGuid();
        // Menyiapkan variabel lokal `createdAt` untuk nilai created at dengan `DateTimeOffset.UtcNow`, yaitu waktu UTC saat operasi dilakukan. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var createdAt = DateTimeOffset.UtcNow;
        // Menyiapkan variabel lokal `configHash` untuk nilai konfigurasi hash dengan memanggil `ComputeHash` dengan `definition`. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var configHash = ComputeHash(definition);
        // Menyiapkan variabel lokal `mode` untuk mode permainan yang menentukan kelompok aturan yang digunakan dengan memanggil `ResolveMode` dengan
        // `definition`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var mode = ResolveMode(definition);

        // Menyiapkan variabel lokal `insertRuleset` untuk nilai insert aturan dengan literal multiline yang dirinci pada komentar di dekat deklarasinya.
        // Tipe yang dipakai adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string insertRuleset =
        // ”””`.
        // Baris literal 2: INSERT INTO menetapkan tabel dan kolom tujuan penambahan rekaman: `insert into rulesets (ruleset_id, name, description,
        // instructor_user_id, created_at, created_by_user_id)`.
        // Baris literal 3: VALUES menyediakan nilai baris baru sesuai urutan kolom INSERT; placeholder @ diikat ke parameter perintah: `values (@rulesetId,
        // @name, @description, @instructorUserId, @createdAt, @CreatedByUserId)`.
        // Baris literal 4: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string insertRuleset = """
            insert into rulesets (ruleset_id, name, description, instructor_user_id, created_at, created_by_user_id)
            values (@rulesetId, @name, @description, @instructorUserId, @createdAt, @CreatedByUserId)
            """;

        // Menyiapkan variabel lokal `insertVersion` untuk nilai insert versi dengan literal multiline yang dirinci pada komentar di dekat deklarasinya.
        // Tipe yang dipakai adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string insertVersion =
        // ”””`.
        // Baris literal 2: INSERT INTO menetapkan tabel dan kolom tujuan penambahan rekaman: `insert into ruleset_versions (ruleset_version_id, ruleset_id,
        // version, status, mode, config_hash, created_at, created_by_user_id)`.
        // Baris literal 3: VALUES menyediakan nilai baris baru sesuai urutan kolom INSERT; placeholder @ diikat ke parameter perintah: `values
        // (@rulesetVersionId, @rulesetId, 1, 'ACTIVE', @mode, @configHash, @createdAt, @CreatedByUserId)`.
        // Baris literal 4: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string insertVersion = """
            insert into ruleset_versions (ruleset_version_id, ruleset_id, version, status, mode, config_hash, created_at, created_by_user_id)
            values (@rulesetVersionId, @rulesetId, 1, 'ACTIVE', @mode, @configHash, @createdAt, @CreatedByUserId)
            """;

        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Menyiapkan variabel lokal `tx` untuk transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan dengan hasil operasi asinkron
        // memulai transaksi pada `conn` menggunakan `ct` agar perubahan terkait dapat diselesaikan bersama; await menunggu hasil tanpa memblokir thread
        // selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope
        // berakhir.
        await using var tx = await conn.BeginTransactionAsync(ct);
        // Menyiapkan variabel lokal `def1` untuk nilai def 1 dengan objek baru bertipe `CommandDefinition` dengan argumen (insertRuleset, new { rulesetId,
        // name, description, instructorUserId, createdAt, CreatedByUserId = createdByUserId }, tx, cancellationToken: ct). Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var def1 = new CommandDefinition(insertRuleset, new
        // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // CreateRulesetAsync.
        {
            // Meneruskan objek anonim yang mengelompokkan rulesetId, name, description, instructorUserId, createdAt, CreatedByUserId sebagai satu nilai sebagai
            // argumen ke konstruktor `CommandDefinition`.
            rulesetId,
            // Meneruskan objek anonim yang mengelompokkan rulesetId, name, description, instructorUserId, createdAt, CreatedByUserId sebagai satu nilai sebagai
            // argumen ke konstruktor `CommandDefinition`.
            name,
            // Meneruskan objek anonim yang mengelompokkan rulesetId, name, description, instructorUserId, createdAt, CreatedByUserId sebagai satu nilai sebagai
            // argumen ke konstruktor `CommandDefinition`.
            description,
            // Meneruskan objek anonim yang mengelompokkan rulesetId, name, description, instructorUserId, createdAt, CreatedByUserId sebagai satu nilai sebagai
            // argumen ke konstruktor `CommandDefinition`.
            instructorUserId,
            // Meneruskan objek anonim yang mengelompokkan rulesetId, name, description, instructorUserId, createdAt, CreatedByUserId sebagai satu nilai sebagai
            // argumen ke konstruktor `CommandDefinition`.
            createdAt,
            // Meneruskan objek anonim yang mengelompokkan rulesetId, name, description, instructorUserId, createdAt, CreatedByUserId sebagai satu nilai sebagai
            // argumen ke konstruktor `CommandDefinition`.
            CreatedByUserId = createdByUserId
        // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam CreateRulesetAsync.
        }, tx, cancellationToken: ct);
        // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `def1`; nilai hasil menunjukkan jumlah baris yang
        // terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam CreateRulesetAsync.
        await conn.ExecuteAsync(def1);

        // Menyiapkan variabel lokal `def2` untuk nilai def 2 dengan objek baru bertipe `CommandDefinition` dengan argumen (insertVersion, new {
        // rulesetVersionId, rulesetId, mode, configHash, createdAt, CreatedByUserId = createdByUserId }, tx, cancellationToken: ct). Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var def2 = new CommandDefinition(insertVersion, new
        // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // CreateRulesetAsync.
        {
            // Meneruskan objek anonim yang mengelompokkan rulesetVersionId, rulesetId, mode, configHash, createdAt, CreatedByUserId sebagai satu nilai sebagai
            // argumen ke konstruktor `CommandDefinition`.
            rulesetVersionId,
            // Meneruskan objek anonim yang mengelompokkan rulesetVersionId, rulesetId, mode, configHash, createdAt, CreatedByUserId sebagai satu nilai sebagai
            // argumen ke konstruktor `CommandDefinition`.
            rulesetId,
            // Meneruskan objek anonim yang mengelompokkan rulesetVersionId, rulesetId, mode, configHash, createdAt, CreatedByUserId sebagai satu nilai sebagai
            // argumen ke konstruktor `CommandDefinition`.
            mode,
            // Meneruskan objek anonim yang mengelompokkan rulesetVersionId, rulesetId, mode, configHash, createdAt, CreatedByUserId sebagai satu nilai sebagai
            // argumen ke konstruktor `CommandDefinition`.
            configHash,
            // Meneruskan objek anonim yang mengelompokkan rulesetVersionId, rulesetId, mode, configHash, createdAt, CreatedByUserId sebagai satu nilai sebagai
            // argumen ke konstruktor `CommandDefinition`.
            createdAt,
            // Meneruskan objek anonim yang mengelompokkan rulesetVersionId, rulesetId, mode, configHash, createdAt, CreatedByUserId sebagai satu nilai sebagai
            // argumen ke konstruktor `CommandDefinition`.
            CreatedByUserId = createdByUserId
        // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam CreateRulesetAsync.
        }, tx, cancellationToken: ct);
        // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `def2`; nilai hasil menunjukkan jumlah baris yang
        // terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam CreateRulesetAsync.
        await conn.ExecuteAsync(def2);
        // Menjalankan hasil operasi asinkron memanggil `WriteRulesetDefinitionAsync` dengan `conn`, `tx`, `rulesetVersionId`, `definition`, `ct`; await
        // menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam CreateRulesetAsync.
        await WriteRulesetDefinitionAsync(conn, tx, rulesetVersionId, definition, ct);

        // Menjalankan hasil operasi asinkron mengesahkan transaksi `tx` sehingga perubahan yang terkumpul menjadi permanen; await menunggu hasil tanpa
        // memblokir thread selama operasi belum selesai dalam CreateRulesetAsync.
        await tx.CommitAsync(ct);
        // Mengembalikan tuple yang membawa bagian 1: rulesetId; bagian 2: rulesetVersionId; bagian 3: 1 kepada pemanggil dalam CreateRulesetAsync; eksekusi
        // jalur ini selesai setelah nilai hasil ditentukan.
        return (rulesetId, rulesetVersionId, 1);
    // Menutup scope metode CreateRulesetAsync; bagian berikut berada di luar batas blok tersebut dalam CreateRulesetAsync.
    }

    /// <summary>
    /// Membuat versi baru (DRAFT) untuk ruleset yang sudah ada, opsional memperbarui nama dan deskripsi.
    /// </summary>
    // Mendefinisikan metode `CreateRulesetVersionAsync` dengan hasil bertipe `Task<(Guid RulesetVersionId, int Version)>`. Membuat versi baru (DRAFT)
    // untuk ruleset yang sudah ada, opsional memperbarui nama dan deskripsi. async memungkinkan metode menunggu operasi I/O dengan await dan
    // mengembalikan penyelesaian melalui Task. Masukan: Parameter `rulesetId` bertipe `Guid` membawa identitas kumpulan aturan permainan; Parameter
    // `name` bertipe `string?` membawa nilai nama; nilai null diizinkan ketika data opsional belum tersedia; Parameter `description` bertipe `string?`
    // membawa nilai description; nilai null diizinkan ketika data opsional belum tersedia; Parameter `definition` bertipe `RulesetDefinitionDto`
    // membawa definisi terstruktur komponen serta parameter aturan permainan; Parameter `createdByUserId` bertipe `Guid?` membawa nilai created
    // berdasarkan pengguna identitas; nilai null diizinkan ketika data opsional belum tersedia; Parameter `ct` bertipe `CancellationToken` membawa
    // sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<(Guid RulesetVersionId, int Version)> CreateRulesetVersionAsync(
        Guid rulesetId,
        string? name,
        string? description,
        RulesetDefinitionDto definition,
        Guid? createdByUserId,
        CancellationToken ct)
    {
        const string lockRulesetSql = """
            select ruleset_id, name, description, instructor_user_id, is_archived, archived_at, created_at, created_by_user_id
            from rulesets
            where ruleset_id = @rulesetId
            for update
            """;

        const string latestVersionSql = """
            select ruleset_version_id, version, config_hash
            from ruleset_versions
            where ruleset_id = @rulesetId
            order by version desc
            limit 1
            """;

        const string updateRuleset = """
            update rulesets
            set name = @name,
                description = @description
            where ruleset_id = @rulesetId
            """;

        const string insertVersion = """
            insert into ruleset_versions (ruleset_version_id, ruleset_id, version, status, mode, config_hash, created_at, created_by_user_id)
            values (@rulesetVersionId, @rulesetId, @version, 'DRAFT', @mode, @configHash, @createdAt, @CreatedByUserId)
            """;

        var rulesetVersionId = Guid.NewGuid();
        var createdAt = DateTimeOffset.UtcNow;
        var configHash = ComputeHash(definition);
        var mode = ResolveMode(definition);

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        await using var tx = await conn.BeginTransactionAsync(ct);

        var lockedRuleset = await conn.QuerySingleOrDefaultAsync<RulesetDb>(
            new CommandDefinition(lockRulesetSql, new { rulesetId }, tx, cancellationToken: ct));
        if (lockedRuleset is null)
        {
            throw new InvalidOperationException("Ruleset tidak ditemukan.");
        }

        var latestVersion = await conn.QuerySingleOrDefaultAsync<RulesetVersionDb>(
            new CommandDefinition(latestVersionSql, new { rulesetId }, tx, cancellationToken: ct));

        if (name is not null || description is not null)
        {
            var updateDef = new CommandDefinition(updateRuleset, new
            {
                rulesetId,
                name = name ?? lockedRuleset.Name,
                description = description ?? lockedRuleset.Description
            }, tx, cancellationToken: ct);
            await conn.ExecuteAsync(updateDef);
        }

        // The database normalizes catalog order; compare the persisted definition on form round trips too.
        var unchanged = latestVersion?.ConfigHash == configHash;
        if (!unchanged && latestVersion is not null)
        {
            var persistedDefinition = await ReadRulesetDefinitionAsync(conn, latestVersion.RulesetVersionId, ct);
            unchanged = persistedDefinition is not null && ComputeHash(persistedDefinition) == configHash;
        }
        if (unchanged && latestVersion is not null)
        {
            await tx.CommitAsync(ct);
            return (latestVersion.RulesetVersionId, latestVersion.Version);
        }

        var nextVersion = (latestVersion?.Version ?? 0) + 1;
        var insertDef = new CommandDefinition(insertVersion, new
        {
            rulesetVersionId,
            rulesetId,
            version = nextVersion,
            mode,
            configHash,
            createdAt,
            CreatedByUserId = createdByUserId
        }, tx, cancellationToken: ct);
        await conn.ExecuteAsync(insertDef);
        await WriteRulesetDefinitionAsync(conn, tx, rulesetVersionId, definition, ct);

        await tx.CommitAsync(ct);
        return (rulesetVersionId, nextVersion);
    }

    /// <summary>
    /// Mengaktifkan versi ruleset: retire versi ACTIVE sebelumnya dan set versi target menjadi ACTIVE.
    /// </summary>
    public async Task<bool> ActivateRulesetVersionAsync(Guid rulesetId, int version, CancellationToken ct)
    // Membuka scope metode ActivateRulesetVersionAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ActivateRulesetVersionAsync.
    {
        // Menyiapkan variabel lokal `targetSql` untuk nilai target SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang
        // dipakai adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string targetSql =
        // ”””`.
        // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select 1`.
        // Baris literal 3: FROM memilih tabel/subquery sumber pembacaan: `from ruleset_versions`.
        // Baris literal 4: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where ruleset_id = @rulesetId and version = @version`.
        // Baris literal 5: FOR UPDATE mengunci baris hasil selama transaksi agar perubahan bersamaan tidak menimpa keadaan yang dibaca: `for update`.
        // Baris literal 6: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string targetSql = """
            select 1
            from ruleset_versions
            where ruleset_id = @rulesetId and version = @version
            for update
            """;

        // Menyiapkan variabel lokal `retireSql` untuk nilai retire SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang
        // dipakai adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string retireSql =
        // ”””`.
        // Baris literal 2: UPDATE memilih tabel yang akan diperbarui; kolom dan batas baris ditentukan oleh SET/WHERE: `update ruleset_versions`.
        // Baris literal 3: SET menetapkan nilai kolom yang diperbarui oleh UPDATE: `set status = 'ARCHIVED'`.
        // Baris literal 4: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where ruleset_id = @rulesetId`.
        // Baris literal 5: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and status = 'ACTIVE'`.
        // Baris literal 6: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and version <> @version`.
        // Baris literal 7: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string retireSql = """
            update ruleset_versions
            set status = 'ARCHIVED'
            where ruleset_id = @rulesetId
              and status = 'ACTIVE'
              and version <> @version
            """;

        // Menyiapkan variabel lokal `activateSql` untuk nilai activate SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe
        // yang dipakai adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string activateSql =
        // ”””`.
        // Baris literal 2: UPDATE memilih tabel yang akan diperbarui; kolom dan batas baris ditentukan oleh SET/WHERE: `update ruleset_versions`.
        // Baris literal 3: SET menetapkan nilai kolom yang diperbarui oleh UPDATE: `set status = 'ACTIVE'`.
        // Baris literal 4: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where ruleset_id = @rulesetId`.
        // Baris literal 5: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and version = @version`.
        // Baris literal 6: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string activateSql = """
            update ruleset_versions
            set status = 'ACTIVE'
            where ruleset_id = @rulesetId
              and version = @version
            """;

        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Menyiapkan variabel lokal `tx` untuk transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan dengan hasil operasi asinkron
        // memulai transaksi pada `conn` menggunakan `ct` agar perubahan terkait dapat diselesaikan bersama; await menunggu hasil tanpa memblokir thread
        // selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope
        // berakhir.
        await using var tx = await conn.BeginTransactionAsync(ct);

        // Menyiapkan variabel lokal `exists` untuk nilai exists dengan hasil operasi asinkron menjalankan perintah basis data melalui `conn` dengan `new
        // CommandDefinition(targetSql, new { rulesetId, version }, tx, cancellationToken: ct)` dan mengambil nilai skalar hasilnya; await menunggu hasil
        // tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var exists = await conn.ExecuteScalarAsync<int?>(
            // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen (targetSql, new { rulesetId, version }, tx, cancellationToken: ct) sebagai
            // argumen ke `conn.ExecuteScalarAsync<int?>`; Meneruskan `targetSql` (nilai target SQL) sebagai argumen ke konstruktor `CommandDefinition`;
            // Meneruskan objek anonim yang mengelompokkan rulesetId, version sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`; Meneruskan
            // `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`; Meneruskan
            // `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen bernama
            // `cancellationToken`.
            new CommandDefinition(targetSql, new { rulesetId, version }, tx, cancellationToken: ct));
        // Memeriksa kebalikan kondisi `exists.HasValue`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ActivateRulesetVersionAsync.
        if (!exists.HasValue)
        // Membuka scope cabang if untuk kondisi `!exists.HasValue`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ActivateRulesetVersionAsync.
        {
            // Menjalankan hasil operasi asinkron membatalkan perubahan yang belum disahkan pada transaksi `tx`; await menunggu hasil tanpa memblokir thread
            // selama operasi belum selesai dalam ActivateRulesetVersionAsync.
            await tx.RollbackAsync(ct);
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam ActivateRulesetVersionAsync; eksekusi jalur ini selesai
            // setelah nilai hasil ditentukan.
            return false;
        // Menutup scope cabang if untuk kondisi `!exists.HasValue`; bagian berikut berada di luar batas blok tersebut dalam ActivateRulesetVersionAsync.
        }

        // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition(retireSql, new { rulesetId, version
        // }, tx, cancellationToken: ct)`; nilai hasil menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi
        // belum selesai dalam ActivateRulesetVersionAsync.
        await conn.ExecuteAsync(new CommandDefinition(retireSql, new { rulesetId, version }, tx, cancellationToken: ct));
        // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition(activateSql, new { rulesetId,
        // version }, tx, cancellationToken: ct)`; nilai hasil menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama
        // operasi belum selesai dalam ActivateRulesetVersionAsync.
        await conn.ExecuteAsync(new CommandDefinition(activateSql, new { rulesetId, version }, tx, cancellationToken: ct));
        // Menjalankan hasil operasi asinkron mengesahkan transaksi `tx` sehingga perubahan yang terkumpul menjadi permanen; await menunggu hasil tanpa
        // memblokir thread selama operasi belum selesai dalam ActivateRulesetVersionAsync.
        await tx.CommitAsync(ct);
        // Mengembalikan true, yaitu kondisi aktif/terpenuhi kepada pemanggil dalam ActivateRulesetVersionAsync; eksekusi jalur ini selesai setelah nilai
        // hasil ditentukan.
        return true;
    // Menutup scope metode ActivateRulesetVersionAsync; bagian berikut berada di luar batas blok tersebut dalam ActivateRulesetVersionAsync.
    }

    /// <summary>
    /// Menghitung jumlah versi yang dimiliki ruleset tertentu.
    /// </summary>
    // Mendefinisikan metode `CountRulesetVersionsAsync` dengan hasil bertipe `Task<int>`. Menghitung jumlah versi yang dimiliki ruleset tertentu. async
    // memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `rulesetId` bertipe `Guid`
    // membawa identitas kumpulan aturan permainan; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan
    // ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<int> CountRulesetVersionsAsync(Guid rulesetId, CancellationToken ct)
    // Membuka scope metode CountRulesetVersionsAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CountRulesetVersionsAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai
        // adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string sql = ”””`.
        // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select count(*)`.
        // Baris literal 3: FROM memilih tabel/subquery sumber pembacaan: `from ruleset_versions`.
        // Baris literal 4: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where ruleset_id = @rulesetId`.
        // Baris literal 5: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string sql = """
            select count(*)
            from ruleset_versions
            where ruleset_id = @rulesetId
            """;

        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Mengembalikan hasil operasi asinkron menjalankan perintah basis data melalui `conn` dengan `new CommandDefinition(sql, new { rulesetId },
        // cancellationToken: ct)` dan mengambil nilai skalar hasilnya; await menunggu hasil tanpa memblokir thread selama operasi belum selesai kepada
        // pemanggil dalam CountRulesetVersionsAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return await conn.ExecuteScalarAsync<int>(
            // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen (sql, new { rulesetId }, cancellationToken: ct) sebagai argumen ke
            // `conn.ExecuteScalarAsync<int>`; Meneruskan `sql` (nilai SQL) sebagai argumen ke konstruktor `CommandDefinition`; Meneruskan objek anonim yang
            // mengelompokkan rulesetId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`; Meneruskan `ct` (sinyal pembatalan agar operasi
            // dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen bernama `cancellationToken`.
            new CommandDefinition(sql, new { rulesetId }, cancellationToken: ct));
    // Menutup scope metode CountRulesetVersionsAsync; bagian berikut berada di luar batas blok tersebut dalam CountRulesetVersionsAsync.
    }

    /// <summary>
    /// Memeriksa apakah versi ruleset sedang digunakan oleh sesi, event, atau snapshot.
    /// </summary>
    // Mendefinisikan metode `IsRulesetVersionUsedAsync` dengan hasil bertipe `Task<bool>`. Memeriksa apakah versi ruleset sedang digunakan oleh sesi,
    // event, atau snapshot. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter
    // `rulesetVersionId` bertipe `Guid` membawa identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang tepat; Parameter `ct`
    // bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi
    // berhenti.
    public async Task<bool> IsRulesetVersionUsedAsync(Guid rulesetVersionId, CancellationToken ct)
    // Membuka scope metode IsRulesetVersionUsedAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam IsRulesetVersionUsedAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai
        // adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string sql = ”””`.
        // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select 1`.
        // Baris literal 3: FROM memilih tabel/subquery sumber pembacaan: `from (`.
        // Baris literal 4: SELECT menentukan nilai atau kolom yang dikembalikan query: `select ruleset_version_id from sessions`.
        // Baris literal 5: UNION menggabungkan hasil SELECT; penanda ALL mempertahankan duplikat bila digunakan: `union all`.
        // Baris literal 6: SELECT menentukan nilai atau kolom yang dikembalikan query: `select ruleset_version_id from events`.
        // Baris literal 7: UNION menggabungkan hasil SELECT; penanda ALL mempertahankan duplikat bila digunakan: `union all`.
        // Baris literal 8: SELECT menentukan nilai atau kolom yang dikembalikan query: `select ruleset_version_id from metric_snapshots`.
        // Baris literal 9: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `) refs`.
        // Baris literal 10: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where refs.ruleset_version_id = @rulesetVersionId`.
        // Baris literal 11: LIMIT membatasi jumlah baris yang dikembalikan query: `limit 1`.
        // Baris literal 12: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string sql = """
            select 1
            from (
                select ruleset_version_id from sessions
                union all
                select ruleset_version_id from events
                union all
                select ruleset_version_id from metric_snapshots
            ) refs
            where refs.ruleset_version_id = @rulesetVersionId
            limit 1
            """;

        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Menyiapkan variabel lokal `result` untuk nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya dengan hasil operasi asinkron menjalankan
        // perintah basis data melalui `conn` dengan `new CommandDefinition(sql, new { rulesetVersionId }, cancellationToken: ct)` dan mengambil nilai
        // skalar hasilnya; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var result = await conn.ExecuteScalarAsync<int?>(
            // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen (sql, new { rulesetVersionId }, cancellationToken: ct) sebagai argumen ke
            // `conn.ExecuteScalarAsync<int?>`; Meneruskan `sql` (nilai SQL) sebagai argumen ke konstruktor `CommandDefinition`; Meneruskan objek anonim yang
            // mengelompokkan rulesetVersionId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`; Meneruskan `ct` (sinyal pembatalan agar
            // operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen bernama `cancellationToken`.
            new CommandDefinition(sql, new { rulesetVersionId }, cancellationToken: ct));
        // Mengembalikan `result.HasValue`, yaitu penanda bahwa nilai nullable tidak kosong kepada pemanggil dalam IsRulesetVersionUsedAsync; eksekusi jalur
        // ini selesai setelah nilai hasil ditentukan.
        return result.HasValue;
    // Menutup scope metode IsRulesetVersionUsedAsync; bagian berikut berada di luar batas blok tersebut dalam IsRulesetVersionUsedAsync.
    }

    /// <summary>
    /// Menghapus versi spesifik dari ruleset.
    /// </summary>
    // Mendefinisikan metode `DeleteRulesetVersionAsync` dengan hasil bertipe `Task<bool>`. Menghapus versi spesifik dari ruleset. async memungkinkan
    // metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `rulesetId` bertipe `Guid` membawa
    // identitas kumpulan aturan permainan; Parameter `version` bertipe `int` membawa nomor versi yang dipakai untuk konsistensi data atau konfigurasi;
    // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
    // aplikasi berhenti.
    public async Task<bool> DeleteRulesetVersionAsync(Guid rulesetId, int version, CancellationToken ct)
    // Membuka scope metode DeleteRulesetVersionAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam DeleteRulesetVersionAsync.
    {
        // Menyiapkan variabel lokal `selectVersionSql` untuk nilai select versi SQL dengan literal multiline yang dirinci pada komentar di dekat
        // deklarasinya. Tipe yang dipakai adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string selectVersionSql
        // = ”””`.
        // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select ruleset_version_id`.
        // Baris literal 3: FROM memilih tabel/subquery sumber pembacaan: `from ruleset_versions`.
        // Baris literal 4: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where ruleset_id = @rulesetId`.
        // Baris literal 5: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and version = @version`.
        // Baris literal 6: FOR UPDATE mengunci baris hasil selama transaksi agar perubahan bersamaan tidak menimpa keadaan yang dibaca: `for update`.
        // Baris literal 7: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string selectVersionSql = """
            select ruleset_version_id
            from ruleset_versions
            where ruleset_id = @rulesetId
              and version = @version
            for update
            """;

        // Menyiapkan variabel lokal `purgeSql` untuk nilai purge SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang
        // dipakai adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string purgeSql = ”””`.
        // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select purge_ruleset_version_content(@rulesetVersionId)`.
        // Baris literal 3: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string purgeSql = """
            select purge_ruleset_version_content(@rulesetVersionId)
            """;

        // Menyiapkan variabel lokal `deleteVersionSql` untuk nilai delete versi SQL dengan literal multiline yang dirinci pada komentar di dekat
        // deklarasinya. Tipe yang dipakai adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string deleteVersionSql
        // = ”””`.
        // Baris literal 2: DELETE menghapus baris pada tabel tujuan sesuai pembatas query: `delete from ruleset_versions`.
        // Baris literal 3: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where ruleset_version_id = @rulesetVersionId`.
        // Baris literal 4: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string deleteVersionSql = """
            delete from ruleset_versions
            where ruleset_version_id = @rulesetVersionId
            """;

        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Menyiapkan variabel lokal `tx` untuk transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan dengan hasil operasi asinkron
        // memulai transaksi pada `conn` menggunakan `ct` agar perubahan terkait dapat diselesaikan bersama; await menunggu hasil tanpa memblokir thread
        // selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope
        // berakhir.
        await using var tx = await conn.BeginTransactionAsync(ct);

        // Menyiapkan variabel lokal `rulesetVersionId` untuk identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang tepat dengan hasil
        // operasi asinkron membaca satu hasil basis data melalui `conn.QuerySingleOrDefaultAsync<Guid?>` dengan `new CommandDefinition(selectVersionSql,
        // new { rulesetId, version }, tx, cancellationToken: ct)`; nilai default menunjukkan tidak ada baris hasil; await menunggu hasil tanpa memblokir
        // thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var rulesetVersionId = await conn.QuerySingleOrDefaultAsync<Guid?>(
            // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen (selectVersionSql, new { rulesetId, version }, tx, cancellationToken: ct)
            // sebagai argumen ke `conn.QuerySingleOrDefaultAsync<Guid?>`; Meneruskan `selectVersionSql` (nilai select versi SQL) sebagai argumen ke konstruktor
            // `CommandDefinition`; Meneruskan objek anonim yang mengelompokkan rulesetId, version sebagai satu nilai sebagai argumen ke konstruktor
            // `CommandDefinition`; Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor
            // `CommandDefinition`; Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi
            // berhenti) sebagai argumen bernama `cancellationToken`.
            new CommandDefinition(selectVersionSql, new { rulesetId, version }, tx, cancellationToken: ct));
        // Memeriksa kebalikan kondisi `rulesetVersionId.HasValue`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // DeleteRulesetVersionAsync.
        if (!rulesetVersionId.HasValue)
        // Membuka scope cabang if untuk kondisi `!rulesetVersionId.HasValue`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // DeleteRulesetVersionAsync.
        {
            // Menjalankan hasil operasi asinkron membatalkan perubahan yang belum disahkan pada transaksi `tx`; await menunggu hasil tanpa memblokir thread
            // selama operasi belum selesai dalam DeleteRulesetVersionAsync.
            await tx.RollbackAsync(ct);
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam DeleteRulesetVersionAsync; eksekusi jalur ini selesai setelah
            // nilai hasil ditentukan.
            return false;
        // Menutup scope cabang if untuk kondisi `!rulesetVersionId.HasValue`; bagian berikut berada di luar batas blok tersebut dalam
        // DeleteRulesetVersionAsync.
        }

        // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition(purgeSql, new { rulesetVersionId =
        // rulesetVersionId.Value }, tx, cancellationToken: ct)`; nilai hasil menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa
        // memblokir thread selama operasi belum selesai dalam DeleteRulesetVersionAsync.
        await conn.ExecuteAsync(
            // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen (purgeSql, new { rulesetVersionId = rulesetVersionId.Value }, tx,
            // cancellationToken: ct) sebagai argumen ke `conn.ExecuteAsync`; Meneruskan `purgeSql` (nilai purge SQL) sebagai argumen ke konstruktor
            // `CommandDefinition`; Meneruskan objek anonim yang mengelompokkan rulesetVersionId sebagai satu nilai sebagai argumen ke konstruktor
            // `CommandDefinition`; Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor
            // `CommandDefinition`; Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi
            // berhenti) sebagai argumen bernama `cancellationToken`.
            new CommandDefinition(purgeSql, new { rulesetVersionId = rulesetVersionId.Value }, tx, cancellationToken: ct));

        // Menyiapkan variabel lokal `affected` untuk nilai affected dengan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new
        // CommandDefinition(deleteVersionSql, new { rulesetVersionId = rulesetVersionId.Value }, tx, cancellationToken: ct)`; nilai hasil menunjukkan
        // jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi
        // nilai awal.
        var affected = await conn.ExecuteAsync(
            // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen (deleteVersionSql, new { rulesetVersionId = rulesetVersionId.Value }, tx,
            // cancellationToken: ct) sebagai argumen ke `conn.ExecuteAsync`; Meneruskan `deleteVersionSql` (nilai delete versi SQL) sebagai argumen ke
            // konstruktor `CommandDefinition`; Meneruskan objek anonim yang mengelompokkan rulesetVersionId sebagai satu nilai sebagai argumen ke konstruktor
            // `CommandDefinition`; Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor
            // `CommandDefinition`; Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi
            // berhenti) sebagai argumen bernama `cancellationToken`.
            new CommandDefinition(deleteVersionSql, new { rulesetVersionId = rulesetVersionId.Value }, tx, cancellationToken: ct));

        // Menjalankan hasil operasi asinkron mengesahkan transaksi `tx` sehingga perubahan yang terkumpul menjadi permanen; await menunggu hasil tanpa
        // memblokir thread selama operasi belum selesai dalam DeleteRulesetVersionAsync.
        await tx.CommitAsync(ct);
        // Mengembalikan pemeriksaan lebih besar antara `affected` dan `0` kepada pemanggil dalam DeleteRulesetVersionAsync; eksekusi jalur ini selesai
        // setelah nilai hasil ditentukan.
        return affected > 0;
    // Menutup scope metode DeleteRulesetVersionAsync; bagian berikut berada di luar batas blok tersebut dalam DeleteRulesetVersionAsync.
    }

    /// <summary>
    /// Mengambil daftar ruleset milik instruktur tertentu beserta versi terbaru dan status pemakaian sesi.
    /// Status bernilai 'ACTIVE' jika ruleset pernah diaktifkan di sesi, 'DRAFT' jika belum.
    /// </summary>
    // Mendefinisikan metode `ListRulesetsByInstructorAsync` dengan hasil bertipe `Task<List<RulesetListItem>>`. Mengambil daftar ruleset milik
    // instruktur tertentu beserta versi terbaru dan status pemakaian sesi. Status bernilai 'ACTIVE' jika ruleset pernah diaktifkan di sesi, 'DRAFT'
    // jika belum. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter
    // `instructorUserId` bertipe `Guid` membawa identitas instruktur pemilik sesi atau aturan; Parameter `ct` bertipe `CancellationToken` membawa
    // sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<List<RulesetListItem>> ListRulesetsByInstructorAsync(Guid instructorUserId, CancellationToken ct)
    // Membuka scope metode ListRulesetsByInstructorAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // ListRulesetsByInstructorAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai
        // adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string sql = ”””`.
        // Baris literal 2: WITH menamai hasil query sementara (CTE) yang dapat digunakan oleh bagian SQL berikutnya: `with latest_versions as (`.
        // Baris literal 3: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
        // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `rv.ruleset_id,`.
        // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `rv.version as
        // latest_version,`.
        // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `rv.mode,`.
        // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `row_number() over (`.
        // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `partition by rv.ruleset_id`.
        // Baris literal 9: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by rv.version desc`.
        // Baris literal 10: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `) as rn`.
        // Baris literal 11: FROM memilih tabel/subquery sumber pembacaan: `from ruleset_versions rv`.
        // Baris literal 12: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 13: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
        // Baris literal 14: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `r.ruleset_id,`.
        // Baris literal 15: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `r.name,`.
        // Baris literal 16: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `coalesce(v.latest_version,
        // 0) as latest_version,`.
        // Baris literal 17: Menyusun pilihan nilai bersyarat di dalam ekspresi SQL: `case when exists (`.
        // Baris literal 18: SELECT menentukan nilai atau kolom yang dikembalikan query: `select 1`.
        // Baris literal 19: FROM memilih tabel/subquery sumber pembacaan: `from ruleset_versions rv2`.
        // Baris literal 20: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join sessions s on s.ruleset_version_id =
        // rv2.ruleset_version_id`.
        // Baris literal 21: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where rv2.ruleset_id = r.ruleset_id`.
        // Baris literal 22: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `) then 'ACTIVE' else 'DRAFT'
        // end as status,`.
        // Baris literal 23: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `false as is_default,`.
        // Baris literal 24: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `exists (`.
        // Baris literal 25: SELECT menentukan nilai atau kolom yang dikembalikan query: `select 1`.
        // Baris literal 26: FROM memilih tabel/subquery sumber pembacaan: `from ruleset_versions rv_lock`.
        // Baris literal 27: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join sessions s_lock on s_lock.ruleset_version_id =
        // rv_lock.ruleset_version_id`.
        // Baris literal 28: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where rv_lock.ruleset_id = r.ruleset_id`.
        // Baris literal 30: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `) as is_locked_by_session,`.
        // Baris literal 31: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `v.mode`.
        // Baris literal 32: FROM memilih tabel/subquery sumber pembacaan: `from rulesets r`.
        // Baris literal 33: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `left join latest_versions v`.
        // Baris literal 34: ON menetapkan syarat pencocokan baris pada relasi JOIN: `on v.ruleset_id = r.ruleset_id and v.rn = 1`.
        // Baris literal 35: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where r.instructor_user_id = @instructorUserId`.
        // Baris literal 36: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and not r.is_archived`.
        // Baris literal 37: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by r.created_at desc`.
        // Baris literal 38: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string sql = """
            with latest_versions as (
                select
                    rv.ruleset_id,
                    rv.version as latest_version,
                    rv.mode,
                    row_number() over (
                        partition by rv.ruleset_id
                        order by rv.version desc
                    ) as rn
                from ruleset_versions rv
            )
            select
                r.ruleset_id,
                r.name,
                coalesce(v.latest_version, 0) as latest_version,
                case when exists (
                    select 1
                    from ruleset_versions rv2
                    join sessions s on s.ruleset_version_id = rv2.ruleset_version_id
                    where rv2.ruleset_id = r.ruleset_id
                ) then 'ACTIVE' else 'DRAFT' end as status,
                false as is_default,
                exists (
                    select 1
                    from ruleset_versions rv_lock
                    join sessions s_lock on s_lock.ruleset_version_id = rv_lock.ruleset_version_id
                    where rv_lock.ruleset_id = r.ruleset_id
                ) as is_locked_by_session,
                v.mode
            from rulesets r
            left join latest_versions v
                on v.ruleset_id = r.ruleset_id and v.rn = 1
            where r.instructor_user_id = @instructorUserId
              and not r.is_archived
            order by r.created_at desc
            """;

        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Menyiapkan variabel lokal `items` untuk nilai elemen dengan hasil operasi asinkron menjalankan query baca melalui `conn` dengan `new
        // CommandDefinition(sql, new { instructorUserId }, cancellationToken: ct)` dan memetakan baris hasil ke tipe yang diminta; await menunggu hasil
        // tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var items = await conn.QueryAsync<RulesetListItem>(
            // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen (sql, new { instructorUserId }, cancellationToken: ct) sebagai argumen ke
            // `conn.QueryAsync<RulesetListItem>`; Meneruskan `sql` (nilai SQL) sebagai argumen ke konstruktor `CommandDefinition`; Meneruskan objek anonim yang
            // mengelompokkan instructorUserId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`; Meneruskan `ct` (sinyal pembatalan agar
            // operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen bernama `cancellationToken`.
            new CommandDefinition(sql, new { instructorUserId }, cancellationToken: ct));
        // Mengembalikan mematerialisasi urutan `items` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori kepada pemanggil dalam
        // ListRulesetsByInstructorAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return items.ToList();
    // Menutup scope metode ListRulesetsByInstructorAsync; bagian berikut berada di luar batas blok tersebut dalam ListRulesetsByInstructorAsync.
    }

    /// <summary>
    /// Mengambil daftar ruleset yang digunakan dalam sesi yang diikuti pemain.
    /// </summary>
    // Mendefinisikan metode `ListRulesetsByPlayerAsync` dengan hasil bertipe `Task<List<RulesetListItem>>`. Mengambil daftar ruleset yang digunakan
    // dalam sesi yang diikuti pemain. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan:
    // Parameter `userId` bertipe `Guid` membawa identitas akun pengguna yang datanya sedang diproses; Parameter `ct` bertipe `CancellationToken`
    // membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<List<RulesetListItem>> ListRulesetsByPlayerAsync(Guid userId, CancellationToken ct)
    // Membuka scope metode ListRulesetsByPlayerAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ListRulesetsByPlayerAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai
        // adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string sql = ”””`.
        // Baris literal 2: WITH menamai hasil query sementara (CTE) yang dapat digunakan oleh bagian SQL berikutnya: `with latest_versions as (`.
        // Baris literal 3: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
        // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `rv.ruleset_id,`.
        // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `rv.version as
        // latest_version,`.
        // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `rv.mode,`.
        // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `row_number() over (`.
        // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `partition by rv.ruleset_id`.
        // Baris literal 9: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by rv.version desc`.
        // Baris literal 10: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `) as rn`.
        // Baris literal 11: FROM memilih tabel/subquery sumber pembacaan: `from ruleset_versions rv`.
        // Baris literal 12: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 13: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
        // Baris literal 14: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `r.ruleset_id,`.
        // Baris literal 15: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `r.name,`.
        // Baris literal 16: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `coalesce(v.latest_version,
        // 0) as latest_version,`.
        // Baris literal 17: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `'ACTIVE' as status,`.
        // Baris literal 18: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `false as is_default,`.
        // Baris literal 19: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `exists (`.
        // Baris literal 20: SELECT menentukan nilai atau kolom yang dikembalikan query: `select 1`.
        // Baris literal 21: FROM memilih tabel/subquery sumber pembacaan: `from ruleset_versions rv_lock`.
        // Baris literal 22: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join sessions s_lock on s_lock.ruleset_version_id =
        // rv_lock.ruleset_version_id`.
        // Baris literal 23: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where rv_lock.ruleset_id = r.ruleset_id`.
        // Baris literal 25: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `) as is_locked_by_session,`.
        // Baris literal 26: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `v.mode`.
        // Baris literal 27: FROM memilih tabel/subquery sumber pembacaan: `from rulesets r`.
        // Baris literal 28: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `left join latest_versions v`.
        // Baris literal 29: ON menetapkan syarat pencocokan baris pada relasi JOIN: `on v.ruleset_id = r.ruleset_id and v.rn = 1`.
        // Baris literal 30: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where exists (`.
        // Baris literal 31: SELECT menentukan nilai atau kolom yang dikembalikan query: `select 1`.
        // Baris literal 32: FROM memilih tabel/subquery sumber pembacaan: `from session_participants sp`.
        // Baris literal 33: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join sessions s on s.session_id = sp.session_id`.
        // Baris literal 34: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join ruleset_versions rv on rv.ruleset_version_id =
        // s.ruleset_version_id`.
        // Baris literal 35: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where sp.user_id = @userId`.
        // Baris literal 36: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and rv.ruleset_id = r.ruleset_id`.
        // Baris literal 37: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 38: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and not r.is_archived`.
        // Baris literal 39: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by r.created_at desc`.
        // Baris literal 40: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string sql = """
            with latest_versions as (
                select
                    rv.ruleset_id,
                    rv.version as latest_version,
                    rv.mode,
                    row_number() over (
                        partition by rv.ruleset_id
                        order by rv.version desc
                    ) as rn
                from ruleset_versions rv
            )
            select
                r.ruleset_id,
                r.name,
                coalesce(v.latest_version, 0) as latest_version,
                'ACTIVE' as status,
                false as is_default,
                exists (
                    select 1
                    from ruleset_versions rv_lock
                    join sessions s_lock on s_lock.ruleset_version_id = rv_lock.ruleset_version_id
                    where rv_lock.ruleset_id = r.ruleset_id
                ) as is_locked_by_session,
                v.mode
            from rulesets r
            left join latest_versions v
                on v.ruleset_id = r.ruleset_id and v.rn = 1
            where exists (
                select 1
                from session_participants sp
                join sessions s on s.session_id = sp.session_id
                join ruleset_versions rv on rv.ruleset_version_id = s.ruleset_version_id
                where sp.user_id = @userId
                  and rv.ruleset_id = r.ruleset_id
            )
              and not r.is_archived
            order by r.created_at desc
            """;

        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Menyiapkan variabel lokal `items` untuk nilai elemen dengan hasil operasi asinkron menjalankan query baca melalui `conn` dengan `new
        // CommandDefinition(sql, new { userId }, cancellationToken: ct)` dan memetakan baris hasil ke tipe yang diminta; await menunggu hasil tanpa
        // memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var items = await conn.QueryAsync<RulesetListItem>(
            // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen (sql, new { userId }, cancellationToken: ct) sebagai argumen ke
            // `conn.QueryAsync<RulesetListItem>`; Meneruskan `sql` (nilai SQL) sebagai argumen ke konstruktor `CommandDefinition`; Meneruskan objek anonim yang
            // mengelompokkan userId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`; Meneruskan `ct` (sinyal pembatalan agar operasi
            // dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen bernama `cancellationToken`.
            new CommandDefinition(sql, new { userId }, cancellationToken: ct));
        // Mengembalikan mematerialisasi urutan `items` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori kepada pemanggil dalam
        // ListRulesetsByPlayerAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return items.ToList();
    // Menutup scope metode ListRulesetsByPlayerAsync; bagian berikut berada di luar batas blok tersebut dalam ListRulesetsByPlayerAsync.
    }

    /// <summary>
    /// Mengambil daftar ruleset default sistem untuk ditampilkan bersama daftar ruleset workspace.
    /// </summary>
    // Mendefinisikan metode `ListDefaultRulesetsAsync` dengan hasil bertipe `Task<List<RulesetListItem>>`. Mengambil daftar ruleset default sistem
    // untuk ditampilkan bersama daftar ruleset workspace. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian
    // melalui Task. Masukan: Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil
    // membatalkan permintaan atau aplikasi berhenti.
    public async Task<List<RulesetListItem>> ListDefaultRulesetsAsync(CancellationToken ct)
    // Membuka scope metode ListDefaultRulesetsAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ListDefaultRulesetsAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai
        // adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string sql = ”””`.
        // Baris literal 2: WITH menamai hasil query sementara (CTE) yang dapat digunakan oleh bagian SQL berikutnya: `with latest_versions as (`.
        // Baris literal 3: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
        // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `rv.ruleset_id,`.
        // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `rv.version as
        // latest_version,`.
        // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `rv.status,`.
        // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `rv.mode,`.
        // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `row_number() over (`.
        // Baris literal 9: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `partition by rv.ruleset_id`.
        // Baris literal 10: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by`.
        // Baris literal 11: Menyusun pilihan nilai bersyarat di dalam ekspresi SQL: `case when rv.status = 'ACTIVE' then 0 else 1 end,`.
        // Baris literal 12: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `rv.version desc`.
        // Baris literal 13: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `) as rn`.
        // Baris literal 14: FROM memilih tabel/subquery sumber pembacaan: `from ruleset_versions rv`.
        // Baris literal 15: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 16: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
        // Baris literal 17: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `r.ruleset_id,`.
        // Baris literal 18: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `r.name,`.
        // Baris literal 19: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `coalesce(v.latest_version,
        // 0) as latest_version,`.
        // Baris literal 20: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `coalesce(v.status, 'ACTIVE')
        // as status,`.
        // Baris literal 21: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `true as is_default,`.
        // Baris literal 22: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `exists (`.
        // Baris literal 23: SELECT menentukan nilai atau kolom yang dikembalikan query: `select 1`.
        // Baris literal 24: FROM memilih tabel/subquery sumber pembacaan: `from ruleset_versions rv_lock`.
        // Baris literal 25: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join sessions s_lock on s_lock.ruleset_version_id =
        // rv_lock.ruleset_version_id`.
        // Baris literal 26: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where rv_lock.ruleset_id = r.ruleset_id`.
        // Baris literal 28: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `) as is_locked_by_session,`.
        // Baris literal 29: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `v.mode`.
        // Baris literal 30: FROM memilih tabel/subquery sumber pembacaan: `from rulesets r`.
        // Baris literal 31: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `left join latest_versions v`.
        // Baris literal 32: ON menetapkan syarat pencocokan baris pada relasi JOIN: `on v.ruleset_id = r.ruleset_id and v.rn = 1`.
        // Baris literal 33: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where r.instructor_user_id is null`.
        // Baris literal 34: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and not r.is_archived`.
        // Baris literal 35: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by`.
        // Baris literal 36: Menyusun pilihan nilai bersyarat di dalam ekspresi SQL: `case upper(coalesce(v.mode, ''))`.
        // Baris literal 37: Menyusun pilihan nilai bersyarat di dalam ekspresi SQL: `when 'PEMULA' then 1`.
        // Baris literal 38: Menyusun pilihan nilai bersyarat di dalam ekspresi SQL: `when 'MAHIR' then 2`.
        // Baris literal 39: Menyusun pilihan nilai bersyarat di dalam ekspresi SQL: `else 3`.
        // Baris literal 40: Menyusun pilihan nilai bersyarat di dalam ekspresi SQL: `end,`.
        // Baris literal 41: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `r.name asc`.
        // Baris literal 42: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string sql = """
            with latest_versions as (
                select
                    rv.ruleset_id,
                    rv.version as latest_version,
                    rv.status,
                    rv.mode,
                    row_number() over (
                        partition by rv.ruleset_id
                        order by
                            case when rv.status = 'ACTIVE' then 0 else 1 end,
                            rv.version desc
                    ) as rn
                from ruleset_versions rv
            )
            select
                r.ruleset_id,
                r.name,
                coalesce(v.latest_version, 0) as latest_version,
                coalesce(v.status, 'ACTIVE') as status,
                true as is_default,
                exists (
                    select 1
                    from ruleset_versions rv_lock
                    join sessions s_lock on s_lock.ruleset_version_id = rv_lock.ruleset_version_id
                    where rv_lock.ruleset_id = r.ruleset_id
                ) as is_locked_by_session,
                v.mode
            from rulesets r
            left join latest_versions v
                on v.ruleset_id = r.ruleset_id and v.rn = 1
            where r.instructor_user_id is null
              and not r.is_archived
            order by
                case upper(coalesce(v.mode, ''))
                    when 'PEMULA' then 1
                    when 'MAHIR' then 2
                    else 3
                end,
                r.name asc
            """;

        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Menyiapkan variabel lokal `items` untuk nilai elemen dengan hasil operasi asinkron menjalankan query baca melalui `conn` dengan `new
        // CommandDefinition(sql, cancellationToken: ct)` dan memetakan baris hasil ke tipe yang diminta; await menunggu hasil tanpa memblokir thread selama
        // operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var items = await conn.QueryAsync<RulesetListItem>(
            // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen (sql, cancellationToken: ct) sebagai argumen ke
            // `conn.QueryAsync<RulesetListItem>`; Meneruskan `sql` (nilai SQL) sebagai argumen ke konstruktor `CommandDefinition`; Meneruskan `ct` (sinyal
            // pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen bernama
            // `cancellationToken`.
            new CommandDefinition(sql, cancellationToken: ct));
        // Mengembalikan mematerialisasi urutan `items` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori kepada pemanggil dalam
        // ListDefaultRulesetsAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return items.ToList();
    // Menutup scope metode ListDefaultRulesetsAsync; bagian berikut berada di luar batas blok tersebut dalam ListDefaultRulesetsAsync.
    }

    /// <summary>
    /// Mengambil komponen ruleset default yang ditandai system-seed beserta config terbaru.
    /// </summary>
    // Mendefinisikan metode `ListDefaultRulesetComponentsAsync` dengan hasil bertipe `Task<List<DefaultRulesetComponentDb>>`. Mengambil komponen
    // ruleset default yang ditandai system-seed beserta config terbaru. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan
    // penyelesaian melalui Task. Masukan: Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika
    // pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<List<DefaultRulesetComponentDb>> ListDefaultRulesetComponentsAsync(CancellationToken ct)
    // Membuka scope metode ListDefaultRulesetComponentsAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // ListDefaultRulesetComponentsAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai
        // adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string sql = ”””`.
        // Baris literal 2: WITH menamai hasil query sementara (CTE) yang dapat digunakan oleh bagian SQL berikutnya: `with latest_versions as (`.
        // Baris literal 3: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
        // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `rv.ruleset_id,`.
        // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `rv.ruleset_version_id,`.
        // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `rv.version,`.
        // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `rv.mode,`.
        // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `row_number() over (`.
        // Baris literal 9: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `partition by rv.ruleset_id`.
        // Baris literal 10: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by`.
        // Baris literal 11: Menyusun pilihan nilai bersyarat di dalam ekspresi SQL: `case when rv.status = 'ACTIVE' then 0 else 1 end,`.
        // Baris literal 12: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `rv.version desc`.
        // Baris literal 13: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `) as rn`.
        // Baris literal 14: FROM memilih tabel/subquery sumber pembacaan: `from ruleset_versions rv`.
        // Baris literal 15: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 16: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
        // Baris literal 17: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `r.ruleset_id,`.
        // Baris literal 18: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `r.name,`.
        // Baris literal 19: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `r.description,`.
        // Baris literal 20: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `lv.ruleset_version_id,`.
        // Baris literal 21: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `lv.version,`.
        // Baris literal 22: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `lv.mode`.
        // Baris literal 23: FROM memilih tabel/subquery sumber pembacaan: `from rulesets r`.
        // Baris literal 24: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join latest_versions lv on lv.ruleset_id = r.ruleset_id and
        // lv.rn = 1`.
        // Baris literal 25: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where r.instructor_user_id is null`.
        // Baris literal 26: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and not r.is_archived`.
        // Baris literal 27: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by`.
        // Baris literal 28: Menyusun pilihan nilai bersyarat di dalam ekspresi SQL: `case upper(coalesce(lv.mode, ''))`.
        // Baris literal 29: Menyusun pilihan nilai bersyarat di dalam ekspresi SQL: `when 'PEMULA' then 1`.
        // Baris literal 30: Menyusun pilihan nilai bersyarat di dalam ekspresi SQL: `when 'MAHIR' then 2`.
        // Baris literal 31: Menyusun pilihan nilai bersyarat di dalam ekspresi SQL: `else 3`.
        // Baris literal 32: Menyusun pilihan nilai bersyarat di dalam ekspresi SQL: `end,`.
        // Baris literal 33: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `r.name asc`.
        // Baris literal 34: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string sql = """
            with latest_versions as (
                select
                    rv.ruleset_id,
                    rv.ruleset_version_id,
                    rv.version,
                    rv.mode,
                    row_number() over (
                        partition by rv.ruleset_id
                        order by
                            case when rv.status = 'ACTIVE' then 0 else 1 end,
                            rv.version desc
                    ) as rn
                from ruleset_versions rv
            )
            select
                r.ruleset_id,
                r.name,
                r.description,
                lv.ruleset_version_id,
                lv.version,
                lv.mode
            from rulesets r
            join latest_versions lv on lv.ruleset_id = r.ruleset_id and lv.rn = 1
            where r.instructor_user_id is null
              and not r.is_archived
            order by
                case upper(coalesce(lv.mode, ''))
                    when 'PEMULA' then 1
                    when 'MAHIR' then 2
                    else 3
                end,
                r.name asc
            """;

        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Menyiapkan variabel lokal `items` untuk nilai elemen dengan hasil operasi asinkron menjalankan query baca melalui `conn` dengan `new
        // CommandDefinition(sql, cancellationToken: ct)` dan memetakan baris hasil ke tipe yang diminta; await menunggu hasil tanpa memblokir thread selama
        // operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var items = await conn.QueryAsync<DefaultRulesetComponentDb>(
            // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen (sql, cancellationToken: ct) sebagai argumen ke
            // `conn.QueryAsync<DefaultRulesetComponentDb>`; Meneruskan `sql` (nilai SQL) sebagai argumen ke konstruktor `CommandDefinition`; Meneruskan `ct`
            // (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen bernama
            // `cancellationToken`.
            new CommandDefinition(sql, cancellationToken: ct));
        // Menyiapkan variabel lokal `list` untuk nilai daftar dengan mematerialisasi urutan `items` menjadi List; enumerasi dijalankan dan hasilnya
        // disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var list = items.ToList();
        // Mengulangi setiap elemen `list`; elemen saat ini disimpan sebagai `item` bertipe `var` untuk diproses oleh badan loop dalam
        // ListDefaultRulesetComponentsAsync.
        foreach (var item in list)
        // Membuka scope loop setiap item dari `list`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ListDefaultRulesetComponentsAsync.
        {
            // Memperbarui `item.Definition` menggunakan hasil operasi asinkron memanggil `ReadRulesetDefinitionAsync` dengan `conn`, `item.RulesetVersionId`,
            // `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam ListDefaultRulesetComponentsAsync.
            item.Definition = await ReadRulesetDefinitionAsync(conn, item.RulesetVersionId, ct);
        // Menutup scope loop setiap item dari `list`; bagian berikut berada di luar batas blok tersebut dalam ListDefaultRulesetComponentsAsync.
        }

        // Mengembalikan `list` (nilai daftar) kepada pemanggil dalam ListDefaultRulesetComponentsAsync; eksekusi jalur ini selesai setelah nilai hasil
        // ditentukan.
        return list;
    // Menutup scope metode ListDefaultRulesetComponentsAsync; bagian berikut berada di luar batas blok tersebut dalam
    // ListDefaultRulesetComponentsAsync.
    }

    /// <summary>
    /// Mengambil data ruleset jika pemain memiliki akses melalui sesi yang menggunakannya.
    /// </summary>
    // Mendefinisikan metode `GetRulesetForPlayerAsync` dengan hasil bertipe `Task<RulesetDb?>`. Mengambil data ruleset jika pemain memiliki akses
    // melalui sesi yang menggunakannya. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task.
    // Masukan: Parameter `rulesetId` bertipe `Guid` membawa identitas kumpulan aturan permainan; Parameter `userId` bertipe `Guid` membawa identitas
    // akun pengguna yang datanya sedang diproses; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan
    // ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<RulesetDb?> GetRulesetForPlayerAsync(Guid rulesetId, Guid userId, CancellationToken ct)
    // Membuka scope metode GetRulesetForPlayerAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam GetRulesetForPlayerAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai
        // adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string sql = ”””`.
        // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select r.ruleset_id, r.name, r.description, r.instructor_user_id,
        // r.is_archived, r.archived_at, r.created_at, r.created_by_user_id`.
        // Baris literal 3: FROM memilih tabel/subquery sumber pembacaan: `from rulesets r`.
        // Baris literal 4: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where r.ruleset_id = @rulesetId`.
        // Baris literal 5: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and exists (`.
        // Baris literal 6: SELECT menentukan nilai atau kolom yang dikembalikan query: `select 1`.
        // Baris literal 7: FROM memilih tabel/subquery sumber pembacaan: `from session_participants sp`.
        // Baris literal 8: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join sessions s on s.session_id = sp.session_id`.
        // Baris literal 9: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join ruleset_versions rv on rv.ruleset_version_id =
        // s.ruleset_version_id`.
        // Baris literal 10: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where sp.user_id = @userId`.
        // Baris literal 11: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and rv.ruleset_id = r.ruleset_id`.
        // Baris literal 12: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 13: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string sql = """
            select r.ruleset_id, r.name, r.description, r.instructor_user_id, r.is_archived, r.archived_at, r.created_at, r.created_by_user_id
            from rulesets r
            where r.ruleset_id = @rulesetId
              and exists (
                  select 1
                  from session_participants sp
                  join sessions s on s.session_id = sp.session_id
                  join ruleset_versions rv on rv.ruleset_version_id = s.ruleset_version_id
                  where sp.user_id = @userId
                    and rv.ruleset_id = r.ruleset_id
              )
            """;

        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Mengembalikan hasil operasi asinkron membaca satu hasil basis data melalui `conn.QuerySingleOrDefaultAsync<RulesetDb>` dengan `new
        // CommandDefinition(sql, new { rulesetId, userId }, cancellationToken: ct)`; nilai default menunjukkan tidak ada baris hasil; await menunggu hasil
        // tanpa memblokir thread selama operasi belum selesai kepada pemanggil dalam GetRulesetForPlayerAsync; eksekusi jalur ini selesai setelah nilai
        // hasil ditentukan.
        return await conn.QuerySingleOrDefaultAsync<RulesetDb>(
            // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen (sql, new { rulesetId, userId }, cancellationToken: ct) sebagai argumen ke
            // `conn.QuerySingleOrDefaultAsync<RulesetDb>`; Meneruskan `sql` (nilai SQL) sebagai argumen ke konstruktor `CommandDefinition`; Meneruskan objek
            // anonim yang mengelompokkan rulesetId, userId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`; Meneruskan `ct` (sinyal
            // pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen bernama
            // `cancellationToken`.
            new CommandDefinition(sql, new { rulesetId, userId }, cancellationToken: ct));
    // Menutup scope metode GetRulesetForPlayerAsync; bagian berikut berada di luar batas blok tersebut dalam GetRulesetForPlayerAsync.
    }

    /// <summary>
    /// Mengambil seluruh versi dari ruleset tertentu diurutkan dari terbaru.
    /// </summary>
    // Mendefinisikan metode `ListRulesetVersionsAsync` dengan hasil bertipe `Task<List<RulesetVersionDb>>`. Mengambil seluruh versi dari ruleset
    // tertentu diurutkan dari terbaru. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task.
    // Masukan: Parameter `rulesetId` bertipe `Guid` membawa identitas kumpulan aturan permainan; Parameter `ct` bertipe `CancellationToken` membawa
    // sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<List<RulesetVersionDb>> ListRulesetVersionsAsync(Guid rulesetId, CancellationToken ct)
    // Membuka scope metode ListRulesetVersionsAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ListRulesetVersionsAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai
        // adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string sql = ”””`.
        // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select ruleset_version_id, ruleset_id, version, status, mode,
        // config_hash, created_at, created_by_user_id`.
        // Baris literal 3: FROM memilih tabel/subquery sumber pembacaan: `from ruleset_versions`.
        // Baris literal 4: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where ruleset_id = @rulesetId`.
        // Baris literal 5: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by version desc`.
        // Baris literal 6: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string sql = """
            select ruleset_version_id, ruleset_id, version, status, mode, config_hash, created_at, created_by_user_id
            from ruleset_versions
            where ruleset_id = @rulesetId
            order by version desc
            """;

        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Menyiapkan variabel lokal `items` untuk nilai elemen dengan hasil operasi asinkron menjalankan query baca melalui `conn` dengan `new
        // CommandDefinition(sql, new { rulesetId }, cancellationToken: ct)` dan memetakan baris hasil ke tipe yang diminta; await menunggu hasil tanpa
        // memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var items = await conn.QueryAsync<RulesetVersionDb>(new CommandDefinition(sql, new { rulesetId }, cancellationToken: ct));
        // Menyiapkan variabel lokal `list` untuk nilai daftar dengan mematerialisasi urutan `items` menjadi List; enumerasi dijalankan dan hasilnya
        // disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var list = items.ToList();
        // Mengulangi setiap elemen `list`; elemen saat ini disimpan sebagai `item` bertipe `var` untuk diproses oleh badan loop dalam
        // ListRulesetVersionsAsync.
        foreach (var item in list)
        // Membuka scope loop setiap item dari `list`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ListRulesetVersionsAsync.
        {
            // Memperbarui `item.Definition` menggunakan hasil operasi asinkron memanggil `ReadRulesetDefinitionAsync` dengan `conn`, `item.RulesetVersionId`,
            // `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam ListRulesetVersionsAsync.
            item.Definition = await ReadRulesetDefinitionAsync(conn, item.RulesetVersionId, ct);
        // Menutup scope loop setiap item dari `list`; bagian berikut berada di luar batas blok tersebut dalam ListRulesetVersionsAsync.
        }

        // Mengembalikan `list` (nilai daftar) kepada pemanggil dalam ListRulesetVersionsAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return list;
    // Menutup scope metode ListRulesetVersionsAsync; bagian berikut berada di luar batas blok tersebut dalam ListRulesetVersionsAsync.
    }

    /// <summary>
    /// Mengambil ruleset jika merupakan seed default sistem tanpa owner instruktur.
    /// </summary>
    // Mendefinisikan metode `GetDefaultSeedRulesetAsync` dengan hasil bertipe `Task<RulesetDb?>`. Mengambil ruleset jika merupakan seed default sistem
    // tanpa owner instruktur. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan:
    // Parameter `rulesetId` bertipe `Guid` membawa identitas kumpulan aturan permainan; Parameter `ct` bertipe `CancellationToken` membawa sinyal
    // pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<RulesetDb?> GetDefaultSeedRulesetAsync(Guid rulesetId, CancellationToken ct)
    // Membuka scope metode GetDefaultSeedRulesetAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam GetDefaultSeedRulesetAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai
        // adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string sql = ”””`.
        // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select ruleset_id, name, description, instructor_user_id,
        // is_archived, archived_at, created_at, created_by_user_id`.
        // Baris literal 3: FROM memilih tabel/subquery sumber pembacaan: `from rulesets`.
        // Baris literal 4: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where ruleset_id = @rulesetId`.
        // Baris literal 5: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and instructor_user_id is null`.
        // Baris literal 6: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and not is_archived`.
        // Baris literal 7: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string sql = """
            select ruleset_id, name, description, instructor_user_id, is_archived, archived_at, created_at, created_by_user_id
            from rulesets
            where ruleset_id = @rulesetId
              and instructor_user_id is null
              and not is_archived
            """;

        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Mengembalikan hasil operasi asinkron membaca satu hasil basis data melalui `conn.QuerySingleOrDefaultAsync<RulesetDb>` dengan `new
        // CommandDefinition(sql, new { rulesetId }, cancellationToken: ct)`; nilai default menunjukkan tidak ada baris hasil; await menunggu hasil tanpa
        // memblokir thread selama operasi belum selesai kepada pemanggil dalam GetDefaultSeedRulesetAsync; eksekusi jalur ini selesai setelah nilai hasil
        // ditentukan.
        return await conn.QuerySingleOrDefaultAsync<RulesetDb>(new CommandDefinition(sql, new { rulesetId }, cancellationToken: ct));
    // Menutup scope metode GetDefaultSeedRulesetAsync; bagian berikut berada di luar batas blok tersebut dalam GetDefaultSeedRulesetAsync.
    }

    /// <summary>
    /// Memeriksa apakah ruleset sedang digunakan oleh sesi manapun.
    /// </summary>
    // Mendefinisikan metode `IsRulesetUsedAsync` dengan hasil bertipe `Task<bool>`. Memeriksa apakah ruleset sedang digunakan oleh sesi manapun. async
    // memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `rulesetId` bertipe `Guid`
    // membawa identitas kumpulan aturan permainan; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan
    // ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<bool> IsRulesetUsedAsync(Guid rulesetId, CancellationToken ct)
    // Membuka scope metode IsRulesetUsedAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam IsRulesetUsedAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai
        // adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string sql = ”””`.
        // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select 1`.
        // Baris literal 3: FROM memilih tabel/subquery sumber pembacaan: `from sessions s`.
        // Baris literal 4: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join ruleset_versions rv on rv.ruleset_version_id =
        // s.ruleset_version_id`.
        // Baris literal 5: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where rv.ruleset_id = @rulesetId`.
        // Baris literal 6: LIMIT membatasi jumlah baris yang dikembalikan query: `limit 1`.
        // Baris literal 7: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string sql = """
            select 1
            from sessions s
            join ruleset_versions rv on rv.ruleset_version_id = s.ruleset_version_id
            where rv.ruleset_id = @rulesetId
            limit 1
            """;

        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Menyiapkan variabel lokal `result` untuk nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya dengan hasil operasi asinkron menjalankan
        // perintah basis data melalui `conn` dengan `new CommandDefinition(sql, new { rulesetId }, cancellationToken: ct)` dan mengambil nilai skalar
        // hasilnya; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var result = await conn.ExecuteScalarAsync<int?>(new CommandDefinition(sql, new { rulesetId }, cancellationToken: ct));
        // Mengembalikan `result.HasValue`, yaitu penanda bahwa nilai nullable tidak kosong kepada pemanggil dalam IsRulesetUsedAsync; eksekusi jalur ini
        // selesai setelah nilai hasil ditentukan.
        return result.HasValue;
    // Menutup scope metode IsRulesetUsedAsync; bagian berikut berada di luar batas blok tersebut dalam IsRulesetUsedAsync.
    }

    /// <summary>
    /// Memeriksa apakah ruleset dikunci karena dipakai sesi sejak dibuat, termasuk histori sesi.
    /// </summary>
    // Mendefinisikan metode `IsRulesetLockedBySessionAsync` dengan hasil bertipe `Task<bool>`. Memeriksa apakah ruleset dikunci karena dipakai sesi
    // sejak dibuat, termasuk histori sesi. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task.
    // Masukan: Parameter `rulesetId` bertipe `Guid` membawa identitas kumpulan aturan permainan; Parameter `ct` bertipe `CancellationToken` membawa
    // sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<bool> IsRulesetLockedBySessionAsync(Guid rulesetId, CancellationToken ct)
    // Membuka scope metode IsRulesetLockedBySessionAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // IsRulesetLockedBySessionAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai
        // adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string sql = ”””`.
        // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select 1`.
        // Baris literal 3: FROM memilih tabel/subquery sumber pembacaan: `from sessions s`.
        // Baris literal 4: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join ruleset_versions rv on rv.ruleset_version_id =
        // s.ruleset_version_id`.
        // Baris literal 5: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where rv.ruleset_id = @rulesetId`.
        // Baris literal 7: LIMIT membatasi jumlah baris yang dikembalikan query: `limit 1`.
        // Baris literal 8: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string sql = """
            select 1
            from sessions s
            join ruleset_versions rv on rv.ruleset_version_id = s.ruleset_version_id
            where rv.ruleset_id = @rulesetId
            limit 1
            """;

        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Menyiapkan variabel lokal `result` untuk nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya dengan hasil operasi asinkron menjalankan
        // perintah basis data melalui `conn` dengan `new CommandDefinition(sql, new { rulesetId }, cancellationToken: ct)` dan mengambil nilai skalar
        // hasilnya; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var result = await conn.ExecuteScalarAsync<int?>(new CommandDefinition(sql, new { rulesetId }, cancellationToken: ct));
        // Mengembalikan `result.HasValue`, yaitu penanda bahwa nilai nullable tidak kosong kepada pemanggil dalam IsRulesetLockedBySessionAsync; eksekusi
        // jalur ini selesai setelah nilai hasil ditentukan.
        return result.HasValue;
    // Menutup scope metode IsRulesetLockedBySessionAsync; bagian berikut berada di luar batas blok tersebut dalam IsRulesetLockedBySessionAsync.
    }

    /// <summary>
    /// Mengarsipkan ruleset agar histori dan versi tetap dapat diaudit.
    /// </summary>
    // Mendefinisikan metode `DeleteRulesetAsync` dengan hasil bertipe `Task`. Mengarsipkan ruleset agar histori dan versi tetap dapat diaudit. async
    // memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `rulesetId` bertipe `Guid`
    // membawa identitas kumpulan aturan permainan; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan
    // ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task DeleteRulesetAsync(Guid rulesetId, CancellationToken ct)
    // Membuka scope metode DeleteRulesetAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam DeleteRulesetAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai
        // adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string sql = ”””`.
        // Baris literal 2: UPDATE memilih tabel yang akan diperbarui; kolom dan batas baris ditentukan oleh SET/WHERE: `update rulesets`.
        // Baris literal 3: SET menetapkan nilai kolom yang diperbarui oleh UPDATE: `set is_archived = true,`.
        // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `archived_at = now()`.
        // Baris literal 5: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where ruleset_id = @rulesetId`.
        // Baris literal 6: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and not is_archived`.
        // Baris literal 7: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string sql = """
            update rulesets
            set is_archived = true,
                archived_at = now()
            where ruleset_id = @rulesetId
              and not is_archived
            """;

        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition(sql, new { rulesetId },
        // cancellationToken: ct)`; nilai hasil menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum
        // selesai dalam DeleteRulesetAsync.
        await conn.ExecuteAsync(new CommandDefinition(sql, new { rulesetId }, cancellationToken: ct));
    // Menutup scope metode DeleteRulesetAsync; bagian berikut berada di luar batas blok tersebut dalam DeleteRulesetAsync.
    }

    // Mendefinisikan metode `ListRulesetActionsAsync` dengan hasil bertipe `Task<List<RulesetActionDto>>`; operasi ini menangani daftar aturan aksi
    // asinkron. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter
    // `rulesetVersionId` bertipe `Guid` membawa identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang tepat; Parameter `ct`
    // bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi
    // berhenti.
    public async Task<List<RulesetActionDto>> ListRulesetActionsAsync(Guid rulesetVersionId, CancellationToken ct)
    // Membuka scope metode ListRulesetActionsAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ListRulesetActionsAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai
        // adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string sql = ”””`.
        // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
        // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `action_id as ActionId`.
        // Baris literal 4: FROM memilih tabel/subquery sumber pembacaan: `from ruleset_actions`.
        // Baris literal 5: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where ruleset_version_id = @rulesetVersionId`.
        // Baris literal 6: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and is_active`.
        // Baris literal 7: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by sort_order asc, action_id asc`.
        // Baris literal 8: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string sql = """
            select
                action_id as ActionId
            from ruleset_actions
            where ruleset_version_id = @rulesetVersionId
              and is_active
            order by sort_order asc, action_id asc
            """;

        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Menyiapkan variabel lokal `items` untuk nilai elemen dengan hasil operasi asinkron menjalankan query baca melalui `conn` dengan `new
        // CommandDefinition(sql, new { rulesetVersionId }, cancellationToken: ct)` dan memetakan baris hasil ke tipe yang diminta; await menunggu hasil
        // tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var items = await conn.QueryAsync<RulesetActionDto>(
            // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen (sql, new { rulesetVersionId }, cancellationToken: ct) sebagai argumen ke
            // `conn.QueryAsync<RulesetActionDto>`; Meneruskan `sql` (nilai SQL) sebagai argumen ke konstruktor `CommandDefinition`; Meneruskan objek anonim
            // yang mengelompokkan rulesetVersionId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`; Meneruskan `ct` (sinyal pembatalan
            // agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen bernama `cancellationToken`.
            new CommandDefinition(sql, new { rulesetVersionId }, cancellationToken: ct));
        // Mengembalikan mematerialisasi urutan `items` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori kepada pemanggil dalam
        // ListRulesetActionsAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return items.ToList();
    // Menutup scope metode ListRulesetActionsAsync; bagian berikut berada di luar batas blok tersebut dalam ListRulesetActionsAsync.
    }

    // Mendefinisikan metode `GetRulesetDefinitionAsync` dengan hasil bertipe `Task<RulesetDefinitionDto?>`; operasi ini menangani get aturan definisi
    // asinkron. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter
    // `rulesetVersionId` bertipe `Guid` membawa identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang tepat; Parameter `ct`
    // bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi
    // berhenti.
    public async Task<RulesetDefinitionDto?> GetRulesetDefinitionAsync(Guid rulesetVersionId, CancellationToken ct)
    // Membuka scope metode GetRulesetDefinitionAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam GetRulesetDefinitionAsync.
    {
        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Mengembalikan hasil operasi asinkron memanggil `ReadRulesetDefinitionAsync` dengan `conn`, `rulesetVersionId`, `ct`; await menunggu hasil tanpa
        // memblokir thread selama operasi belum selesai kepada pemanggil dalam GetRulesetDefinitionAsync; eksekusi jalur ini selesai setelah nilai hasil
        // ditentukan.
        return await ReadRulesetDefinitionAsync(conn, rulesetVersionId, ct);
    // Menutup scope metode GetRulesetDefinitionAsync; bagian berikut berada di luar batas blok tersebut dalam GetRulesetDefinitionAsync.
    }

    // Mendefinisikan metode `InsertGameAssetAsync` dengan hasil bertipe `Task<Guid>`; operasi ini menangani insert game aset asinkron. async
    // memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `conn` bertipe
    // `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data; Parameter `tx` bertipe `DbTransaction`
    // membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan; Parameter `rulesetVersionId` bertipe `Guid` membawa identitas
    // versi aturan sehingga perhitungan memakai konfigurasi aturan yang tepat; Parameter `assetType` bertipe `string` membawa nilai aset jenis;
    // Parameter `assetCode` bertipe `string` membawa nilai aset kode; Parameter `displayName` bertipe `string` membawa nilai display nama; Parameter
    // `sortOrder` bertipe `int` membawa nilai sort urutan/pesanan; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi
    // dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    private static async Task<Guid> InsertGameAssetAsync(
        // Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data.
        NpgsqlConnection conn,
        // Parameter `tx` bertipe `DbTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan.
        DbTransaction tx,
        // Parameter `rulesetVersionId` bertipe `Guid` membawa identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang tepat.
        Guid rulesetVersionId,
        // Parameter `assetType` bertipe `string` membawa nilai aset jenis.
        string assetType,
        // Parameter `assetCode` bertipe `string` membawa nilai aset kode.
        string assetCode,
        // Parameter `displayName` bertipe `string` membawa nilai display nama.
        string displayName,
        // Parameter `sortOrder` bertipe `int` membawa nilai sort urutan/pesanan.
        int sortOrder,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    // Membuka scope metode InsertGameAssetAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam InsertGameAssetAsync.
    {
        // Menyiapkan variabel lokal `assetId` untuk nilai aset identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var assetId = Guid.NewGuid();
        // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition( ””” insert into
        // ruleset_game_assets ( ruleset_game_asset_id, ruleset_version_id, asset_type, asset_code, display_name, sort_order, is_active, metadata_j...`;
        // nilai hasil menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam
        // InsertGameAssetAsync.
        await conn.ExecuteAsync(new CommandDefinition(
            // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke konstruktor `CommandDefinition`.
            // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
            // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 2: INSERT INTO menetapkan tabel dan kolom tujuan penambahan rekaman: `insert into ruleset_game_assets (`.
            // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_game_asset_id,`.
            // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_version_id,`.
            // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `asset_type,`.
            // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `asset_code,`.
            // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `display_name,`.
            // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `sort_order,`.
            // Baris literal 9: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `is_active,`.
            // Baris literal 10: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `metadata_json,`.
            // Baris literal 11: CREATE mendefinisikan objek basis data yang diperlukan proses ini: `created_at,`.
            // Baris literal 12: UPDATE memilih tabel yang akan diperbarui; kolom dan batas baris ditentukan oleh SET/WHERE: `updated_at`.
            // Baris literal 13: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 14: VALUES menyediakan nilai baris baru sesuai urutan kolom INSERT; placeholder @ diikat ke parameter perintah: `values (`.
            // Baris literal 15: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `@assetId,`.
            // Baris literal 16: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `@rulesetVersionId,`.
            // Baris literal 17: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `@assetType,`.
            // Baris literal 18: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `@assetCode,`.
            // Baris literal 19: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `@displayName,`.
            // Baris literal 20: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `@sortOrder,`.
            // Baris literal 21: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `true,`.
            // Baris literal 22: Menyusun pilihan nilai bersyarat di dalam ekspresi SQL: `case when @assetType = 'GOLD' then '{”card_qty”:20}'::jsonb else
            // '{}'::jsonb end,`.
            // Baris literal 23: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `now(),`.
            // Baris literal 24: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `now()`.
            // Baris literal 25: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 26: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            """
            insert into ruleset_game_assets (
                ruleset_game_asset_id,
                ruleset_version_id,
                asset_type,
                asset_code,
                display_name,
                sort_order,
                is_active,
                metadata_json,
                created_at,
                updated_at
            )
            values (
                @assetId,
                @rulesetVersionId,
                @assetType,
                @assetCode,
                @displayName,
                @sortOrder,
                true,
                case when @assetType = 'GOLD' then '{"card_qty":20}'::jsonb else '{}'::jsonb end,
                now(),
                now()
            )
            """,
            // Meneruskan objek anonim yang mengelompokkan assetId, rulesetVersionId, assetType, assetCode, displayName, sortOrder sebagai satu nilai sebagai
            // argumen ke konstruktor `CommandDefinition`.
            new { assetId, rulesetVersionId, assetType, assetCode, displayName, sortOrder },
            // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
            tx,
            // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
            // bernama `cancellationToken`.
            cancellationToken: ct));
        // Mengembalikan `assetId` (nilai aset identitas) kepada pemanggil dalam InsertGameAssetAsync; eksekusi jalur ini selesai setelah nilai hasil
        // ditentukan.
        return assetId;
    // Menutup scope metode InsertGameAssetAsync; bagian berikut berada di luar batas blok tersebut dalam InsertGameAssetAsync.
    }

    // Mendefinisikan metode `WriteRulesetDefinitionAsync` dengan hasil bertipe `Task`; operasi ini menangani write aturan definisi asinkron. async
    // memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `conn` bertipe
    // `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data; Parameter `tx` bertipe `DbTransaction`
    // membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan; Parameter `rulesetVersionId` bertipe `Guid` membawa identitas
    // versi aturan sehingga perhitungan memakai konfigurasi aturan yang tepat; Parameter `definition` bertipe `RulesetDefinitionDto` membawa definisi
    // terstruktur komponen serta parameter aturan permainan; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat
    // dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    private async Task WriteRulesetDefinitionAsync(
        // Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data.
        NpgsqlConnection conn,
        // Parameter `tx` bertipe `DbTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan.
        DbTransaction tx,
        // Parameter `rulesetVersionId` bertipe `Guid` membawa identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang tepat.
        Guid rulesetVersionId,
        // Parameter `definition` bertipe `RulesetDefinitionDto` membawa definisi terstruktur komponen serta parameter aturan permainan.
        RulesetDefinitionDto definition,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    // Membuka scope metode WriteRulesetDefinitionAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam WriteRulesetDefinitionAsync.
    {
        // Menyiapkan variabel lokal `settings` untuk nilai settings dengan `definition.Settings` bila tidak null; jika null gunakan `new
        // RulesetSettingsDto()` sebagai nilai pengganti. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var settings = definition.Settings ?? new RulesetSettingsDto();
        // Menyiapkan variabel lokal `playerOrdering` untuk nilai pemain ordering dengan `definition.PlayerOrdering` bila tidak null; jika null gunakan `new
        // RulesetPlayerOrderingDto()` sebagai nilai pengganti. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var playerOrdering = definition.PlayerOrdering ?? new RulesetPlayerOrderingDto();

        // Menyiapkan variabel lokal `insertSettingsSql` untuk nilai insert settings SQL dengan literal multiline yang dirinci pada komentar di dekat
        // deklarasinya. Tipe yang dipakai adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string
        // insertSettingsSql = ”””`.
        // Baris literal 2: INSERT INTO menetapkan tabel dan kolom tujuan penambahan rekaman: `insert into ruleset_game_settings (`.
        // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_version_id,`.
        // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `starting_cash,`.
        // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `starting_happiness,`.
        // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `starting_saving,`.
        // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `actions_per_turn,`.
        // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `finish_day,`.
        // Baris literal 9: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `min_players,`.
        // Baris literal 10: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `max_players,`.
        // Baris literal 11: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `cash_min,`.
        // Baris literal 12: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `max_ingredient_total,`.
        // Baris literal 13: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `max_same_ingredient,`.
        // Baris literal 14: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `primary_need_max_per_day,`.
        // Baris literal 15: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun:
        // `require_primary_before_others,`.
        // Baris literal 16: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `donation_min_amount,`.
        // Baris literal 17: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `donation_max_amount,`.
        // Baris literal 18: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `gold_trade_allow_buy,`.
        // Baris literal 19: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `gold_trade_allow_sell,`.
        // Baris literal 20: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `loan_enabled,`.
        // Baris literal 21: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `insurance_enabled,`.
        // Baris literal 22: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `saving_goal_enabled,`.
        // Baris literal 23: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `freelance_income`.
        // Baris literal 24: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 25: VALUES menyediakan nilai baris baru sesuai urutan kolom INSERT; placeholder @ diikat ke parameter perintah: `values (`.
        // Baris literal 26: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@RulesetVersionId,`.
        // Baris literal 27: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@StartingCash,`.
        // Baris literal 28: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@InitialHappiness,`.
        // Baris literal 29: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@InitialSaving,`.
        // Baris literal 30: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@ActionsPerTurn,`.
        // Baris literal 31: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@FinishDay,`.
        // Baris literal 32: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@MinPlayers,`.
        // Baris literal 33: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@MaxPlayers,`.
        // Baris literal 34: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@CashMin,`.
        // Baris literal 35: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@MaxIngredientTotal,`.
        // Baris literal 36: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@MaxSameIngredient,`.
        // Baris literal 37: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@PrimaryNeedMaxPerDay,`.
        // Baris literal 38: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@RequirePrimaryBeforeOthers,`.
        // Baris literal 39: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@DonationMinAmount,`.
        // Baris literal 40: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@DonationMaxAmount,`.
        // Baris literal 41: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@GoldTradeAllowBuy,`.
        // Baris literal 42: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@GoldTradeAllowSell,`.
        // Baris literal 43: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@LoanEnabled,`.
        // Baris literal 44: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@InsuranceEnabled,`.
        // Baris literal 45: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@SavingGoalEnabled,`.
        // Baris literal 46: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@FreelanceIncome`.
        // Baris literal 47: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 48: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string insertSettingsSql = """
            insert into ruleset_game_settings (
                ruleset_version_id,
                starting_cash,
                starting_happiness,
                starting_saving,
                actions_per_turn,
                finish_day,
                min_players,
                max_players,
                cash_min,
                max_ingredient_total,
                max_same_ingredient,
                primary_need_max_per_day,
                require_primary_before_others,
                donation_min_amount,
                donation_max_amount,
                gold_trade_allow_buy,
                gold_trade_allow_sell,
                loan_enabled,
                insurance_enabled,
                saving_goal_enabled,
                freelance_income
            )
            values (
                @RulesetVersionId,
                @StartingCash,
                @InitialHappiness,
                @InitialSaving,
                @ActionsPerTurn,
                @FinishDay,
                @MinPlayers,
                @MaxPlayers,
                @CashMin,
                @MaxIngredientTotal,
                @MaxSameIngredient,
                @PrimaryNeedMaxPerDay,
                @RequirePrimaryBeforeOthers,
                @DonationMinAmount,
                @DonationMaxAmount,
                @GoldTradeAllowBuy,
                @GoldTradeAllowSell,
                @LoanEnabled,
                @InsuranceEnabled,
                @SavingGoalEnabled,
                @FreelanceIncome
            )
            """;

        // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition( insertSettingsSql, new {
        // RulesetVersionId = rulesetVersionId, settings.StartingCash, InitialHappiness = settings.InitialHappiness, InitialSaving = setti...`; nilai hasil
        // menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam
        // WriteRulesetDefinitionAsync.
        await conn.ExecuteAsync(
            // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen ( insertSettingsSql, new { RulesetVersionId = rulesetVersionId,
            // settings.StartingCash, InitialHappiness = settings.InitialHappiness, InitialSaving = settings.In... sebagai argumen ke `conn.ExecuteAsync`.
            new CommandDefinition(
                // Meneruskan `insertSettingsSql` (nilai insert settings SQL) sebagai argumen ke konstruktor `CommandDefinition`.
                insertSettingsSql,
                // Meneruskan objek anonim yang mengelompokkan RulesetVersionId, settings.StartingCash, InitialHappiness, InitialSaving, settings.ActionsPerTurn,
                // settings.FinishDay, settings.MinPlayers, settings.MaxPlayers, settings.CashMin, settings.MaxIngredientTotal, settings.MaxSameIngredient,
                // settings.PrimaryNeedMaxPerDay, settings.RequirePrimaryBeforeOthers, settings.DonationMinAmount, settings.DonationMaxAmount,
                // settings.GoldTradeAllowBuy, settings.GoldTradeAllowSell, settings.LoanEnabled, settings.InsuranceEnabled, settings.SavingGoalEnabled,
                // settings.FreelanceIncome sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                new
                // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // WriteRulesetDefinitionAsync.
                {
                    // Meneruskan objek anonim yang mengelompokkan RulesetVersionId, settings.StartingCash, InitialHappiness, InitialSaving, settings.ActionsPerTurn,
                    // settings.FinishDay, settings.MinPlayers, settings.MaxPlayers, settings.CashMin, settings.MaxIngredientTotal, settings.MaxSameIngredient,
                    // settings.PrimaryNeedMaxPerDay, settings.RequirePrimaryBeforeOthers, settings.DonationMinAmount, settings.DonationMaxAmount,
                    // settings.GoldTradeAllowBuy, settings.GoldTradeAllowSell, settings.LoanEnabled, settings.InsuranceEnabled, settings.SavingGoalEnabled,
                    // settings.FreelanceIncome sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                    RulesetVersionId = rulesetVersionId,
                    // Meneruskan objek anonim yang mengelompokkan RulesetVersionId, settings.StartingCash, InitialHappiness, InitialSaving, settings.ActionsPerTurn,
                    // settings.FinishDay, settings.MinPlayers, settings.MaxPlayers, settings.CashMin, settings.MaxIngredientTotal, settings.MaxSameIngredient,
                    // settings.PrimaryNeedMaxPerDay, settings.RequirePrimaryBeforeOthers, settings.DonationMinAmount, settings.DonationMaxAmount,
                    // settings.GoldTradeAllowBuy, settings.GoldTradeAllowSell, settings.LoanEnabled, settings.InsuranceEnabled, settings.SavingGoalEnabled,
                    // settings.FreelanceIncome sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                    settings.StartingCash,
                    // Meneruskan objek anonim yang mengelompokkan RulesetVersionId, settings.StartingCash, InitialHappiness, InitialSaving, settings.ActionsPerTurn,
                    // settings.FinishDay, settings.MinPlayers, settings.MaxPlayers, settings.CashMin, settings.MaxIngredientTotal, settings.MaxSameIngredient,
                    // settings.PrimaryNeedMaxPerDay, settings.RequirePrimaryBeforeOthers, settings.DonationMinAmount, settings.DonationMaxAmount,
                    // settings.GoldTradeAllowBuy, settings.GoldTradeAllowSell, settings.LoanEnabled, settings.InsuranceEnabled, settings.SavingGoalEnabled,
                    // settings.FreelanceIncome sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                    InitialHappiness = settings.InitialHappiness,
                    // Meneruskan objek anonim yang mengelompokkan RulesetVersionId, settings.StartingCash, InitialHappiness, InitialSaving, settings.ActionsPerTurn,
                    // settings.FinishDay, settings.MinPlayers, settings.MaxPlayers, settings.CashMin, settings.MaxIngredientTotal, settings.MaxSameIngredient,
                    // settings.PrimaryNeedMaxPerDay, settings.RequirePrimaryBeforeOthers, settings.DonationMinAmount, settings.DonationMaxAmount,
                    // settings.GoldTradeAllowBuy, settings.GoldTradeAllowSell, settings.LoanEnabled, settings.InsuranceEnabled, settings.SavingGoalEnabled,
                    // settings.FreelanceIncome sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                    InitialSaving = settings.InitialSaving,
                    // Meneruskan objek anonim yang mengelompokkan RulesetVersionId, settings.StartingCash, InitialHappiness, InitialSaving, settings.ActionsPerTurn,
                    // settings.FinishDay, settings.MinPlayers, settings.MaxPlayers, settings.CashMin, settings.MaxIngredientTotal, settings.MaxSameIngredient,
                    // settings.PrimaryNeedMaxPerDay, settings.RequirePrimaryBeforeOthers, settings.DonationMinAmount, settings.DonationMaxAmount,
                    // settings.GoldTradeAllowBuy, settings.GoldTradeAllowSell, settings.LoanEnabled, settings.InsuranceEnabled, settings.SavingGoalEnabled,
                    // settings.FreelanceIncome sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                    settings.ActionsPerTurn,
                    // Meneruskan objek anonim yang mengelompokkan RulesetVersionId, settings.StartingCash, InitialHappiness, InitialSaving, settings.ActionsPerTurn,
                    // settings.FinishDay, settings.MinPlayers, settings.MaxPlayers, settings.CashMin, settings.MaxIngredientTotal, settings.MaxSameIngredient,
                    // settings.PrimaryNeedMaxPerDay, settings.RequirePrimaryBeforeOthers, settings.DonationMinAmount, settings.DonationMaxAmount,
                    // settings.GoldTradeAllowBuy, settings.GoldTradeAllowSell, settings.LoanEnabled, settings.InsuranceEnabled, settings.SavingGoalEnabled,
                    // settings.FreelanceIncome sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                    settings.FinishDay,
                    // Meneruskan objek anonim yang mengelompokkan RulesetVersionId, settings.StartingCash, InitialHappiness, InitialSaving, settings.ActionsPerTurn,
                    // settings.FinishDay, settings.MinPlayers, settings.MaxPlayers, settings.CashMin, settings.MaxIngredientTotal, settings.MaxSameIngredient,
                    // settings.PrimaryNeedMaxPerDay, settings.RequirePrimaryBeforeOthers, settings.DonationMinAmount, settings.DonationMaxAmount,
                    // settings.GoldTradeAllowBuy, settings.GoldTradeAllowSell, settings.LoanEnabled, settings.InsuranceEnabled, settings.SavingGoalEnabled,
                    // settings.FreelanceIncome sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                    settings.MinPlayers,
                    // Meneruskan objek anonim yang mengelompokkan RulesetVersionId, settings.StartingCash, InitialHappiness, InitialSaving, settings.ActionsPerTurn,
                    // settings.FinishDay, settings.MinPlayers, settings.MaxPlayers, settings.CashMin, settings.MaxIngredientTotal, settings.MaxSameIngredient,
                    // settings.PrimaryNeedMaxPerDay, settings.RequirePrimaryBeforeOthers, settings.DonationMinAmount, settings.DonationMaxAmount,
                    // settings.GoldTradeAllowBuy, settings.GoldTradeAllowSell, settings.LoanEnabled, settings.InsuranceEnabled, settings.SavingGoalEnabled,
                    // settings.FreelanceIncome sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                    settings.MaxPlayers,
                    // Meneruskan objek anonim yang mengelompokkan RulesetVersionId, settings.StartingCash, InitialHappiness, InitialSaving, settings.ActionsPerTurn,
                    // settings.FinishDay, settings.MinPlayers, settings.MaxPlayers, settings.CashMin, settings.MaxIngredientTotal, settings.MaxSameIngredient,
                    // settings.PrimaryNeedMaxPerDay, settings.RequirePrimaryBeforeOthers, settings.DonationMinAmount, settings.DonationMaxAmount,
                    // settings.GoldTradeAllowBuy, settings.GoldTradeAllowSell, settings.LoanEnabled, settings.InsuranceEnabled, settings.SavingGoalEnabled,
                    // settings.FreelanceIncome sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                    settings.CashMin,
                    // Meneruskan objek anonim yang mengelompokkan RulesetVersionId, settings.StartingCash, InitialHappiness, InitialSaving, settings.ActionsPerTurn,
                    // settings.FinishDay, settings.MinPlayers, settings.MaxPlayers, settings.CashMin, settings.MaxIngredientTotal, settings.MaxSameIngredient,
                    // settings.PrimaryNeedMaxPerDay, settings.RequirePrimaryBeforeOthers, settings.DonationMinAmount, settings.DonationMaxAmount,
                    // settings.GoldTradeAllowBuy, settings.GoldTradeAllowSell, settings.LoanEnabled, settings.InsuranceEnabled, settings.SavingGoalEnabled,
                    // settings.FreelanceIncome sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                    settings.MaxIngredientTotal,
                    // Meneruskan objek anonim yang mengelompokkan RulesetVersionId, settings.StartingCash, InitialHappiness, InitialSaving, settings.ActionsPerTurn,
                    // settings.FinishDay, settings.MinPlayers, settings.MaxPlayers, settings.CashMin, settings.MaxIngredientTotal, settings.MaxSameIngredient,
                    // settings.PrimaryNeedMaxPerDay, settings.RequirePrimaryBeforeOthers, settings.DonationMinAmount, settings.DonationMaxAmount,
                    // settings.GoldTradeAllowBuy, settings.GoldTradeAllowSell, settings.LoanEnabled, settings.InsuranceEnabled, settings.SavingGoalEnabled,
                    // settings.FreelanceIncome sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                    settings.MaxSameIngredient,
                    // Meneruskan objek anonim yang mengelompokkan RulesetVersionId, settings.StartingCash, InitialHappiness, InitialSaving, settings.ActionsPerTurn,
                    // settings.FinishDay, settings.MinPlayers, settings.MaxPlayers, settings.CashMin, settings.MaxIngredientTotal, settings.MaxSameIngredient,
                    // settings.PrimaryNeedMaxPerDay, settings.RequirePrimaryBeforeOthers, settings.DonationMinAmount, settings.DonationMaxAmount,
                    // settings.GoldTradeAllowBuy, settings.GoldTradeAllowSell, settings.LoanEnabled, settings.InsuranceEnabled, settings.SavingGoalEnabled,
                    // settings.FreelanceIncome sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                    settings.PrimaryNeedMaxPerDay,
                    // Meneruskan objek anonim yang mengelompokkan RulesetVersionId, settings.StartingCash, InitialHappiness, InitialSaving, settings.ActionsPerTurn,
                    // settings.FinishDay, settings.MinPlayers, settings.MaxPlayers, settings.CashMin, settings.MaxIngredientTotal, settings.MaxSameIngredient,
                    // settings.PrimaryNeedMaxPerDay, settings.RequirePrimaryBeforeOthers, settings.DonationMinAmount, settings.DonationMaxAmount,
                    // settings.GoldTradeAllowBuy, settings.GoldTradeAllowSell, settings.LoanEnabled, settings.InsuranceEnabled, settings.SavingGoalEnabled,
                    // settings.FreelanceIncome sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                    settings.RequirePrimaryBeforeOthers,
                    // Meneruskan objek anonim yang mengelompokkan RulesetVersionId, settings.StartingCash, InitialHappiness, InitialSaving, settings.ActionsPerTurn,
                    // settings.FinishDay, settings.MinPlayers, settings.MaxPlayers, settings.CashMin, settings.MaxIngredientTotal, settings.MaxSameIngredient,
                    // settings.PrimaryNeedMaxPerDay, settings.RequirePrimaryBeforeOthers, settings.DonationMinAmount, settings.DonationMaxAmount,
                    // settings.GoldTradeAllowBuy, settings.GoldTradeAllowSell, settings.LoanEnabled, settings.InsuranceEnabled, settings.SavingGoalEnabled,
                    // settings.FreelanceIncome sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                    settings.DonationMinAmount,
                    // Meneruskan objek anonim yang mengelompokkan RulesetVersionId, settings.StartingCash, InitialHappiness, InitialSaving, settings.ActionsPerTurn,
                    // settings.FinishDay, settings.MinPlayers, settings.MaxPlayers, settings.CashMin, settings.MaxIngredientTotal, settings.MaxSameIngredient,
                    // settings.PrimaryNeedMaxPerDay, settings.RequirePrimaryBeforeOthers, settings.DonationMinAmount, settings.DonationMaxAmount,
                    // settings.GoldTradeAllowBuy, settings.GoldTradeAllowSell, settings.LoanEnabled, settings.InsuranceEnabled, settings.SavingGoalEnabled,
                    // settings.FreelanceIncome sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                    settings.DonationMaxAmount,
                    // Meneruskan objek anonim yang mengelompokkan RulesetVersionId, settings.StartingCash, InitialHappiness, InitialSaving, settings.ActionsPerTurn,
                    // settings.FinishDay, settings.MinPlayers, settings.MaxPlayers, settings.CashMin, settings.MaxIngredientTotal, settings.MaxSameIngredient,
                    // settings.PrimaryNeedMaxPerDay, settings.RequirePrimaryBeforeOthers, settings.DonationMinAmount, settings.DonationMaxAmount,
                    // settings.GoldTradeAllowBuy, settings.GoldTradeAllowSell, settings.LoanEnabled, settings.InsuranceEnabled, settings.SavingGoalEnabled,
                    // settings.FreelanceIncome sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                    settings.GoldTradeAllowBuy,
                    // Meneruskan objek anonim yang mengelompokkan RulesetVersionId, settings.StartingCash, InitialHappiness, InitialSaving, settings.ActionsPerTurn,
                    // settings.FinishDay, settings.MinPlayers, settings.MaxPlayers, settings.CashMin, settings.MaxIngredientTotal, settings.MaxSameIngredient,
                    // settings.PrimaryNeedMaxPerDay, settings.RequirePrimaryBeforeOthers, settings.DonationMinAmount, settings.DonationMaxAmount,
                    // settings.GoldTradeAllowBuy, settings.GoldTradeAllowSell, settings.LoanEnabled, settings.InsuranceEnabled, settings.SavingGoalEnabled,
                    // settings.FreelanceIncome sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                    settings.GoldTradeAllowSell,
                    // Meneruskan objek anonim yang mengelompokkan RulesetVersionId, settings.StartingCash, InitialHappiness, InitialSaving, settings.ActionsPerTurn,
                    // settings.FinishDay, settings.MinPlayers, settings.MaxPlayers, settings.CashMin, settings.MaxIngredientTotal, settings.MaxSameIngredient,
                    // settings.PrimaryNeedMaxPerDay, settings.RequirePrimaryBeforeOthers, settings.DonationMinAmount, settings.DonationMaxAmount,
                    // settings.GoldTradeAllowBuy, settings.GoldTradeAllowSell, settings.LoanEnabled, settings.InsuranceEnabled, settings.SavingGoalEnabled,
                    // settings.FreelanceIncome sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                    settings.LoanEnabled,
                    // Meneruskan objek anonim yang mengelompokkan RulesetVersionId, settings.StartingCash, InitialHappiness, InitialSaving, settings.ActionsPerTurn,
                    // settings.FinishDay, settings.MinPlayers, settings.MaxPlayers, settings.CashMin, settings.MaxIngredientTotal, settings.MaxSameIngredient,
                    // settings.PrimaryNeedMaxPerDay, settings.RequirePrimaryBeforeOthers, settings.DonationMinAmount, settings.DonationMaxAmount,
                    // settings.GoldTradeAllowBuy, settings.GoldTradeAllowSell, settings.LoanEnabled, settings.InsuranceEnabled, settings.SavingGoalEnabled,
                    // settings.FreelanceIncome sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                    settings.InsuranceEnabled,
                    // Meneruskan objek anonim yang mengelompokkan RulesetVersionId, settings.StartingCash, InitialHappiness, InitialSaving, settings.ActionsPerTurn,
                    // settings.FinishDay, settings.MinPlayers, settings.MaxPlayers, settings.CashMin, settings.MaxIngredientTotal, settings.MaxSameIngredient,
                    // settings.PrimaryNeedMaxPerDay, settings.RequirePrimaryBeforeOthers, settings.DonationMinAmount, settings.DonationMaxAmount,
                    // settings.GoldTradeAllowBuy, settings.GoldTradeAllowSell, settings.LoanEnabled, settings.InsuranceEnabled, settings.SavingGoalEnabled,
                    // settings.FreelanceIncome sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                    settings.SavingGoalEnabled,
                    // Meneruskan objek anonim yang mengelompokkan RulesetVersionId, settings.StartingCash, InitialHappiness, InitialSaving, settings.ActionsPerTurn,
                    // settings.FinishDay, settings.MinPlayers, settings.MaxPlayers, settings.CashMin, settings.MaxIngredientTotal, settings.MaxSameIngredient,
                    // settings.PrimaryNeedMaxPerDay, settings.RequirePrimaryBeforeOthers, settings.DonationMinAmount, settings.DonationMaxAmount,
                    // settings.GoldTradeAllowBuy, settings.GoldTradeAllowSell, settings.LoanEnabled, settings.InsuranceEnabled, settings.SavingGoalEnabled,
                    // settings.FreelanceIncome sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                    settings.FreelanceIncome
                // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
                // WriteRulesetDefinitionAsync.
                },
                // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
                tx,
                // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                // bernama `cancellationToken`.
                cancellationToken: ct));

        // Menyiapkan variabel lokal `insertOrderingSql` untuk nilai insert ordering SQL dengan literal multiline yang dirinci pada komentar di dekat
        // deklarasinya. Tipe yang dipakai adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string
        // insertOrderingSql = ”””`.
        // Baris literal 2: INSERT INTO menetapkan tabel dan kolom tujuan penambahan rekaman: `insert into ruleset_player_ordering_rules (`.
        // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun:
        // `ruleset_player_ordering_rule_id,`.
        // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_version_id,`.
        // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `sort_order,`.
        // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ordering_code,`.
        // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `weekday_code,`.
        // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `feature_code,`.
        // Baris literal 9: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `is_enabled,`.
        // Baris literal 10: CREATE mendefinisikan objek basis data yang diperlukan proses ini: `created_at`.
        // Baris literal 11: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 12: VALUES menyediakan nilai baris baru sesuai urutan kolom INSERT; placeholder @ diikat ke parameter perintah: `values (`.
        // Baris literal 13: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@RulesetPlayerOrderingRuleId,`.
        // Baris literal 14: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@RulesetVersionId,`.
        // Baris literal 15: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@SortOrder,`.
        // Baris literal 16: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@OrderingCode,`.
        // Baris literal 17: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@WeekdayCode,`.
        // Baris literal 18: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@FeatureCode,`.
        // Baris literal 19: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@IsEnabled,`.
        // Baris literal 20: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `now()`.
        // Baris literal 21: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 22: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string insertOrderingSql = """
            insert into ruleset_player_ordering_rules (
                ruleset_player_ordering_rule_id,
                ruleset_version_id,
                sort_order,
                ordering_code,
                weekday_code,
                feature_code,
                is_enabled,
                created_at
            )
            values (
                @RulesetPlayerOrderingRuleId,
                @RulesetVersionId,
                @SortOrder,
                @OrderingCode,
                @WeekdayCode,
                @FeatureCode,
                @IsEnabled,
                now()
            )
            """;

        // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition( insertOrderingSql, new {
        // RulesetPlayerOrderingRuleId = Guid.NewGuid(), RulesetVersionId = rulesetVersionId, SortOrder = 10, OrderingCode = playerOrderin...`; nilai hasil
        // menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam
        // WriteRulesetDefinitionAsync.
        await conn.ExecuteAsync(
            // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen ( insertOrderingSql, new { RulesetPlayerOrderingRuleId = Guid.NewGuid(),
            // RulesetVersionId = rulesetVersionId, SortOrder = 10, OrderingCode = playerOrdering.Orde... sebagai argumen ke `conn.ExecuteAsync`.
            new CommandDefinition(
                // Meneruskan `insertOrderingSql` (nilai insert ordering SQL) sebagai argumen ke konstruktor `CommandDefinition`.
                insertOrderingSql,
                // Meneruskan objek anonim yang mengelompokkan RulesetPlayerOrderingRuleId, RulesetVersionId, SortOrder, OrderingCode, WeekdayCode, FeatureCode,
                // IsEnabled sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                new
                // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // WriteRulesetDefinitionAsync.
                {
                    // Meneruskan objek anonim yang mengelompokkan RulesetPlayerOrderingRuleId, RulesetVersionId, SortOrder, OrderingCode, WeekdayCode, FeatureCode,
                    // IsEnabled sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                    RulesetPlayerOrderingRuleId = Guid.NewGuid(),
                    // Meneruskan objek anonim yang mengelompokkan RulesetPlayerOrderingRuleId, RulesetVersionId, SortOrder, OrderingCode, WeekdayCode, FeatureCode,
                    // IsEnabled sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                    RulesetVersionId = rulesetVersionId,
                    // Meneruskan objek anonim yang mengelompokkan RulesetPlayerOrderingRuleId, RulesetVersionId, SortOrder, OrderingCode, WeekdayCode, FeatureCode,
                    // IsEnabled sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                    SortOrder = 10,
                    // Meneruskan objek anonim yang mengelompokkan RulesetPlayerOrderingRuleId, RulesetVersionId, SortOrder, OrderingCode, WeekdayCode, FeatureCode,
                    // IsEnabled sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                    OrderingCode = playerOrdering.OrderingCode,
                    // Meneruskan objek anonim yang mengelompokkan RulesetPlayerOrderingRuleId, RulesetVersionId, SortOrder, OrderingCode, WeekdayCode, FeatureCode,
                    // IsEnabled sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                    WeekdayCode = (string?)null,
                    // Meneruskan objek anonim yang mengelompokkan RulesetPlayerOrderingRuleId, RulesetVersionId, SortOrder, OrderingCode, WeekdayCode, FeatureCode,
                    // IsEnabled sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                    FeatureCode = (string?)null,
                    // Meneruskan objek anonim yang mengelompokkan RulesetPlayerOrderingRuleId, RulesetVersionId, SortOrder, OrderingCode, WeekdayCode, FeatureCode,
                    // IsEnabled sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                    IsEnabled = true
                // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
                // WriteRulesetDefinitionAsync.
                },
                // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
                tx,
                // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                // bernama `cancellationToken`.
                cancellationToken: ct));

        // Mengulangi setiap elemen `new[] { new { SortOrder = 20, WeekdayCode = ”FRI”, FeatureCode = playerOrdering.FridayFeature, IsEnabled =
        // playerOrdering.FridayEnabled }, new { SortOrder = 30, WeekdayCode = ...`; elemen saat ini disimpan sebagai `weekdayRule` bertipe `var` untuk
        // diproses oleh badan loop dalam WriteRulesetDefinitionAsync.
        foreach (var weekdayRule in new[]
                 // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                 // WriteRulesetDefinitionAsync.
                 {
                     // Menggunakan objek anonim yang mengelompokkan SortOrder, WeekdayCode, FeatureCode, IsEnabled sebagai satu nilai sebagai bagian ekspresi yang
                     // sedang disusun dalam WriteRulesetDefinitionAsync.
                     new { SortOrder = 20, WeekdayCode = "FRI", FeatureCode = playerOrdering.FridayFeature, IsEnabled = playerOrdering.FridayEnabled },
                     // Menggunakan objek anonim yang mengelompokkan SortOrder, WeekdayCode, FeatureCode, IsEnabled sebagai satu nilai sebagai bagian ekspresi yang
                     // sedang disusun dalam WriteRulesetDefinitionAsync.
                     new { SortOrder = 30, WeekdayCode = "SAT", FeatureCode = playerOrdering.SaturdayFeature, IsEnabled = playerOrdering.SaturdayEnabled },
                     // Menggunakan objek anonim yang mengelompokkan SortOrder, WeekdayCode, FeatureCode, IsEnabled sebagai satu nilai sebagai bagian ekspresi yang
                     // sedang disusun dalam WriteRulesetDefinitionAsync.
                     new { SortOrder = 40, WeekdayCode = "SUN", FeatureCode = playerOrdering.SundayFeature, IsEnabled = playerOrdering.SundayEnabled }
                 // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam WriteRulesetDefinitionAsync.
                 })
        // Membuka scope loop setiap weekdayRule dari `new[] { new { SortOrder = 20, WeekdayCode = ”FRI”, FeatureCode = playerOrdering.FridayFeature,
        // IsEnabled = playerOrdering.FridayEnabled }, new { SortOrder = 30, WeekdayCode = ...`; pernyataan/deklarasi berikut berada di dalam batas blok ini
        // dalam WriteRulesetDefinitionAsync.
        {
            // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition( insertOrderingSql, new {
            // RulesetPlayerOrderingRuleId = Guid.NewGuid(), RulesetVersionId = rulesetVersionId, weekdayRule.SortOrder, OrderingCode = (strin...`; nilai hasil
            // menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam
            // WriteRulesetDefinitionAsync.
            await conn.ExecuteAsync(
                // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen ( insertOrderingSql, new { RulesetPlayerOrderingRuleId = Guid.NewGuid(),
                // RulesetVersionId = rulesetVersionId, weekdayRule.SortOrder, OrderingCode = (string?)nul... sebagai argumen ke `conn.ExecuteAsync`.
                new CommandDefinition(
                    // Meneruskan `insertOrderingSql` (nilai insert ordering SQL) sebagai argumen ke konstruktor `CommandDefinition`.
                    insertOrderingSql,
                    // Meneruskan objek anonim yang mengelompokkan RulesetPlayerOrderingRuleId, RulesetVersionId, weekdayRule.SortOrder, OrderingCode,
                    // weekdayRule.WeekdayCode, weekdayRule.FeatureCode, weekdayRule.IsEnabled sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                    new
                    // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                    // WriteRulesetDefinitionAsync.
                    {
                        // Meneruskan objek anonim yang mengelompokkan RulesetPlayerOrderingRuleId, RulesetVersionId, weekdayRule.SortOrder, OrderingCode,
                        // weekdayRule.WeekdayCode, weekdayRule.FeatureCode, weekdayRule.IsEnabled sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        RulesetPlayerOrderingRuleId = Guid.NewGuid(),
                        // Meneruskan objek anonim yang mengelompokkan RulesetPlayerOrderingRuleId, RulesetVersionId, weekdayRule.SortOrder, OrderingCode,
                        // weekdayRule.WeekdayCode, weekdayRule.FeatureCode, weekdayRule.IsEnabled sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        RulesetVersionId = rulesetVersionId,
                        // Meneruskan objek anonim yang mengelompokkan RulesetPlayerOrderingRuleId, RulesetVersionId, weekdayRule.SortOrder, OrderingCode,
                        // weekdayRule.WeekdayCode, weekdayRule.FeatureCode, weekdayRule.IsEnabled sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        weekdayRule.SortOrder,
                        // Meneruskan objek anonim yang mengelompokkan RulesetPlayerOrderingRuleId, RulesetVersionId, weekdayRule.SortOrder, OrderingCode,
                        // weekdayRule.WeekdayCode, weekdayRule.FeatureCode, weekdayRule.IsEnabled sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        OrderingCode = (string?)null,
                        // Meneruskan objek anonim yang mengelompokkan RulesetPlayerOrderingRuleId, RulesetVersionId, weekdayRule.SortOrder, OrderingCode,
                        // weekdayRule.WeekdayCode, weekdayRule.FeatureCode, weekdayRule.IsEnabled sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        weekdayRule.WeekdayCode,
                        // Meneruskan objek anonim yang mengelompokkan RulesetPlayerOrderingRuleId, RulesetVersionId, weekdayRule.SortOrder, OrderingCode,
                        // weekdayRule.WeekdayCode, weekdayRule.FeatureCode, weekdayRule.IsEnabled sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        weekdayRule.FeatureCode,
                        // Meneruskan objek anonim yang mengelompokkan RulesetPlayerOrderingRuleId, RulesetVersionId, weekdayRule.SortOrder, OrderingCode,
                        // weekdayRule.WeekdayCode, weekdayRule.FeatureCode, weekdayRule.IsEnabled sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        weekdayRule.IsEnabled
                    // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
                    // WriteRulesetDefinitionAsync.
                    },
                    // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
                    tx,
                    // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                    // bernama `cancellationToken`.
                    cancellationToken: ct));
        // Menutup scope loop setiap weekdayRule dari `new[] { new { SortOrder = 20, WeekdayCode = ”FRI”, FeatureCode = playerOrdering.FridayFeature,
        // IsEnabled = playerOrdering.FridayEnabled }, new { SortOrder = 30, WeekdayCode = ...`; bagian berikut berada di luar batas blok tersebut dalam
        // WriteRulesetDefinitionAsync.
        }

        // Menyiapkan variabel lokal `insertActionSql` untuk nilai insert aksi SQL dengan literal multiline yang dirinci pada komentar di dekat
        // deklarasinya. Tipe yang dipakai adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string insertActionSql
        // = ”””`.
        // Baris literal 2: INSERT INTO menetapkan tabel dan kolom tujuan penambahan rekaman: `insert into ruleset_actions (`.
        // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_action_id,`.
        // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_version_id,`.
        // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `action_id,`.
        // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `behavior_id,`.
        // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `sort_order,`.
        // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `is_active,`.
        // Baris literal 9: CREATE mendefinisikan objek basis data yang diperlukan proses ini: `created_at`.
        // Baris literal 10: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 11: VALUES menyediakan nilai baris baru sesuai urutan kolom INSERT; placeholder @ diikat ke parameter perintah: `values (`.
        // Baris literal 12: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@RulesetActionId,`.
        // Baris literal 13: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@RulesetVersionId,`.
        // Baris literal 14: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@ActionId,`.
        // Baris literal 15: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@BehaviorId,`.
        // Baris literal 16: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@SortOrder,`.
        // Baris literal 17: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `true,`.
        // Baris literal 18: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `now()`.
        // Baris literal 19: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 20: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string insertActionSql = """
            insert into ruleset_actions (
                ruleset_action_id,
                ruleset_version_id,
                action_id,
                behavior_id,
                sort_order,
                is_active,
                created_at
            )
            values (
                @RulesetActionId,
                @RulesetVersionId,
                @ActionId,
                @BehaviorId,
                @SortOrder,
                true,
                now()
            )
            """;

        // Menyiapkan variabel lokal `actionIds` untuk nilai aksi identitas dengan mematerialisasi urutan `definition.Actions .Select(item => item.ActionId)
        // .Where(item => !string.IsNullOrWhiteSpace(item)) .Append(”CatatTransaksi”) .Append(”JumatBerkah”) .Append(”LewatiTransaksiEma...` menjadi List;
        // enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var actionIds = definition.Actions
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Select(item => item.ActionId) dalam WriteRulesetDefinitionAsync; token pada
            // baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Select(item => item.ActionId)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(item => !string.IsNullOrWhiteSpace(item)) dalam
            // WriteRulesetDefinitionAsync; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Where(item => !string.IsNullOrWhiteSpace(item))
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Append(”CatatTransaksi”) dalam WriteRulesetDefinitionAsync; token pada baris
            // ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Append("CatatTransaksi")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Append(”JumatBerkah”) dalam WriteRulesetDefinitionAsync; token pada baris ini
            // menyambungkan bagian kode sebelum dan sesudahnya.
            .Append("JumatBerkah")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Append(”LewatiTransaksiEmas”) dalam WriteRulesetDefinitionAsync; token pada
            // baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Append("LewatiTransaksiEmas")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Append(”HariMingguLibur”) dalam WriteRulesetDefinitionAsync; token pada baris
            // ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Append("HariMingguLibur")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Append(”MulaiSesi”) dalam WriteRulesetDefinitionAsync; token pada baris ini
            // menyambungkan bagian kode sebelum dan sesudahnya.
            .Append("MulaiSesi")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Append(”SetupBahanAwal”) dalam WriteRulesetDefinitionAsync; token pada baris
            // ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Append("SetupBahanAwal")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Append(”SetupEmasAwal”) dalam WriteRulesetDefinitionAsync; token pada baris
            // ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Append("SetupEmasAwal")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Append(”SetupMisiAwal”) dalam WriteRulesetDefinitionAsync; token pada baris
            // ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Append("SetupMisiAwal")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Append(”SetupPinjamanAwal”) dalam WriteRulesetDefinitionAsync; token pada
            // baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Append("SetupPinjamanAwal")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Append(”SetupAsuransiAwal”) dalam WriteRulesetDefinitionAsync; token pada
            // baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Append("SetupAsuransiAwal")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Append(”BagikanTieBreaker”) dalam WriteRulesetDefinitionAsync; token pada
            // baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Append("BagikanTieBreaker")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Append(”AkhirGiliran”) dalam WriteRulesetDefinitionAsync; token pada baris ini
            // menyambungkan bagian kode sebelum dan sesudahnya.
            .Append("AkhirGiliran")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Append(”AkhiriSesi”) dalam WriteRulesetDefinitionAsync; token pada baris ini
            // menyambungkan bagian kode sebelum dan sesudahnya.
            .Append("AkhiriSesi")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Distinct(StringComparer.OrdinalIgnoreCase) dalam WriteRulesetDefinitionAsync;
            // token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Distinct(StringComparer.OrdinalIgnoreCase)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList(); dalam WriteRulesetDefinitionAsync; token pada baris ini
            // menyambungkan bagian kode sebelum dan sesudahnya.
            .ToList();

        // Memulai loop dengan inisialisasi `var index = 0`, berjalan selama `index < actionIds.Count`, lalu memperbarui pencacah melalui `index++` dalam
        // WriteRulesetDefinitionAsync.
        for (var index = 0; index < actionIds.Count; index++)
        // Membuka scope loop dengan syarat `index < actionIds.Count`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // WriteRulesetDefinitionAsync.
        {
            // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition( insertActionSql, new {
            // RulesetActionId = Guid.NewGuid(), RulesetVersionId = rulesetVersionId, ActionId = actionIds[index], BehaviorId = actionIds[index]...`; nilai
            // hasil menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam
            // WriteRulesetDefinitionAsync.
            await conn.ExecuteAsync(
                // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen ( insertActionSql, new { RulesetActionId = Guid.NewGuid(), RulesetVersionId =
                // rulesetVersionId, ActionId = actionIds[index], BehaviorId = actionIds[index], Sort... sebagai argumen ke `conn.ExecuteAsync`.
                new CommandDefinition(
                    // Meneruskan `insertActionSql` (nilai insert aksi SQL) sebagai argumen ke konstruktor `CommandDefinition`.
                    insertActionSql,
                    // Meneruskan objek anonim yang mengelompokkan RulesetActionId, RulesetVersionId, ActionId, BehaviorId, SortOrder sebagai satu nilai sebagai argumen
                    // ke konstruktor `CommandDefinition`.
                    new
                    // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                    // WriteRulesetDefinitionAsync.
                    {
                        // Meneruskan objek anonim yang mengelompokkan RulesetActionId, RulesetVersionId, ActionId, BehaviorId, SortOrder sebagai satu nilai sebagai argumen
                        // ke konstruktor `CommandDefinition`.
                        RulesetActionId = Guid.NewGuid(),
                        // Meneruskan objek anonim yang mengelompokkan RulesetActionId, RulesetVersionId, ActionId, BehaviorId, SortOrder sebagai satu nilai sebagai argumen
                        // ke konstruktor `CommandDefinition`.
                        RulesetVersionId = rulesetVersionId,
                        // Meneruskan `index` (nilai index) sebagai argumen ke konstruktor `CommandDefinition`.
                        ActionId = actionIds[index],
                        // Meneruskan `index` (nilai index) sebagai argumen ke konstruktor `CommandDefinition`.
                        BehaviorId = actionIds[index],
                        // Meneruskan objek anonim yang mengelompokkan RulesetActionId, RulesetVersionId, ActionId, BehaviorId, SortOrder sebagai satu nilai sebagai argumen
                        // ke konstruktor `CommandDefinition`.
                        SortOrder = index + 1
                    // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
                    // WriteRulesetDefinitionAsync.
                    },
                    // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
                    tx,
                    // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                    // bernama `cancellationToken`.
                    cancellationToken: ct));
        // Menutup scope loop dengan syarat `index < actionIds.Count`; bagian berikut berada di luar batas blok tersebut dalam WriteRulesetDefinitionAsync.
        }

        // Menyiapkan variabel lokal `insertIngredientSql` untuk nilai insert bahan SQL dengan literal multiline yang dirinci pada komentar di dekat
        // deklarasinya. Tipe yang dipakai adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string
        // insertIngredientSql = ”””`.
        // Baris literal 2: INSERT INTO menetapkan tabel dan kolom tujuan penambahan rekaman: `insert into ruleset_ingredients (`.
        // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_ingredient_id,`.
        // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_version_id,`.
        // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_game_asset_id,`.
        // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ingredient_code,`.
        // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `item_name,`.
        // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `display_name,`.
        // Baris literal 9: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `purchase_price,`.
        // Baris literal 10: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `sort_order,`.
        // Baris literal 11: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `card_qty,`.
        // Baris literal 12: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `is_active,`.
        // Baris literal 13: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `payload_json,`.
        // Baris literal 14: CREATE mendefinisikan objek basis data yang diperlukan proses ini: `created_at`.
        // Baris literal 15: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 16: VALUES menyediakan nilai baris baru sesuai urutan kolom INSERT; placeholder @ diikat ke parameter perintah: `values (`.
        // Baris literal 17: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@RulesetIngredientId,`.
        // Baris literal 18: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@RulesetVersionId,`.
        // Baris literal 19: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@RulesetGameAssetId,`.
        // Baris literal 20: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@IngredientCode,`.
        // Baris literal 21: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@ItemName,`.
        // Baris literal 22: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@DisplayName,`.
        // Baris literal 23: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@PurchasePrice,`.
        // Baris literal 24: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@SortOrder,`.
        // Baris literal 25: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@CardQty,`.
        // Baris literal 26: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `true,`.
        // Baris literal 27: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `'{}',`.
        // Baris literal 28: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `now()`.
        // Baris literal 29: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 30: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string insertIngredientSql = """
            insert into ruleset_ingredients (
                ruleset_ingredient_id,
                ruleset_version_id,
                ruleset_game_asset_id,
                ingredient_code,
                item_name,
                display_name,
                purchase_price,
                sort_order,
                card_qty,
                is_active,
                payload_json,
                created_at
            )
            values (
                @RulesetIngredientId,
                @RulesetVersionId,
                @RulesetGameAssetId,
                @IngredientCode,
                @ItemName,
                @DisplayName,
                @PurchasePrice,
                @SortOrder,
                @CardQty,
                true,
                '{}',
                now()
            )
            """;

        // Menyiapkan variabel lokal `ingredientAssetIds` untuk nilai bahan aset identitas dengan objek baru bertipe `Dictionary<string, Guid>` dengan
        // argumen (StringComparer.OrdinalIgnoreCase). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var ingredientAssetIds = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);
        // Memulai loop dengan inisialisasi `var index = 0`, berjalan selama `index < definition.Ingredients.Count`, lalu memperbarui pencacah melalui
        // `index++` dalam WriteRulesetDefinitionAsync.
        for (var index = 0; index < definition.Ingredients.Count; index++)
        // Membuka scope loop dengan syarat `index < definition.Ingredients.Count`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // WriteRulesetDefinitionAsync.
        {
            // Menyiapkan variabel lokal `ingredient` untuk nilai bahan dengan `definition.Ingredients[index]`, yaitu elemen koleksi yang dipilih melalui indeks
            // atau kunci tersebut. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var ingredient = definition.Ingredients[index];
            // Menyiapkan variabel lokal `assetId` untuk nilai aset identitas dengan hasil operasi asinkron memanggil `InsertGameAssetAsync` dengan `conn`,
            // `tx`, `rulesetVersionId`, `”INGREDIENT”`, `ingredient.Id`, `ingredient.Nama`, `index + 1`, `ct`; await menunggu hasil tanpa memblokir thread
            // selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var assetId = await InsertGameAssetAsync(
                // Meneruskan `conn` (koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data) sebagai argumen ke `InsertGameAssetAsync`.
                conn,
                // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke `InsertGameAssetAsync`.
                tx,
                // Meneruskan `rulesetVersionId` (identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang tepat) sebagai argumen ke
                // `InsertGameAssetAsync`.
                rulesetVersionId,
                // Meneruskan nilai literal `”INGREDIENT”` sebagai argumen ke `InsertGameAssetAsync`.
                "INGREDIENT",
                // Meneruskan `ingredient.Id` (nilai identitas) sebagai argumen ke `InsertGameAssetAsync`.
                ingredient.Id,
                // Meneruskan `ingredient.Nama` (nilai nama) sebagai argumen ke `InsertGameAssetAsync`.
                ingredient.Nama,
                // Meneruskan penjumlahan/penggabungan antara `index` dan `1` sebagai argumen ke `InsertGameAssetAsync`.
                index + 1,
                // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                // ke `InsertGameAssetAsync`.
                ct);
            // Memperbarui `ingredientAssetIds[ingredient.Id]` menggunakan `assetId` (nilai aset identitas) dalam WriteRulesetDefinitionAsync.
            ingredientAssetIds[ingredient.Id] = assetId;
            // Memperbarui `ingredientAssetIds[ingredient.Nama]` menggunakan `assetId` (nilai aset identitas) dalam WriteRulesetDefinitionAsync.
            ingredientAssetIds[ingredient.Nama] = assetId;
            // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition( insertIngredientSql, new {
            // RulesetIngredientId = Guid.NewGuid(), RulesetVersionId = rulesetVersionId, RulesetGameAssetId = assetId, IngredientCode = ing...`; nilai hasil
            // menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam
            // WriteRulesetDefinitionAsync.
            await conn.ExecuteAsync(
                // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen ( insertIngredientSql, new { RulesetIngredientId = Guid.NewGuid(),
                // RulesetVersionId = rulesetVersionId, RulesetGameAssetId = assetId, IngredientCode = ingredien... sebagai argumen ke `conn.ExecuteAsync`.
                new CommandDefinition(
                    // Meneruskan `insertIngredientSql` (nilai insert bahan SQL) sebagai argumen ke konstruktor `CommandDefinition`.
                    insertIngredientSql,
                    // Meneruskan objek anonim yang mengelompokkan RulesetIngredientId, RulesetVersionId, RulesetGameAssetId, IngredientCode, ItemName, DisplayName,
                    // PurchasePrice, SortOrder, CardQty, PayloadJson sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                    new
                    // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                    // WriteRulesetDefinitionAsync.
                    {
                        // Meneruskan objek anonim yang mengelompokkan RulesetIngredientId, RulesetVersionId, RulesetGameAssetId, IngredientCode, ItemName, DisplayName,
                        // PurchasePrice, SortOrder, CardQty, PayloadJson sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        RulesetIngredientId = Guid.NewGuid(),
                        // Meneruskan objek anonim yang mengelompokkan RulesetIngredientId, RulesetVersionId, RulesetGameAssetId, IngredientCode, ItemName, DisplayName,
                        // PurchasePrice, SortOrder, CardQty, PayloadJson sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        RulesetVersionId = rulesetVersionId,
                        // Meneruskan objek anonim yang mengelompokkan RulesetIngredientId, RulesetVersionId, RulesetGameAssetId, IngredientCode, ItemName, DisplayName,
                        // PurchasePrice, SortOrder, CardQty, PayloadJson sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        RulesetGameAssetId = assetId,
                        // Meneruskan objek anonim yang mengelompokkan RulesetIngredientId, RulesetVersionId, RulesetGameAssetId, IngredientCode, ItemName, DisplayName,
                        // PurchasePrice, SortOrder, CardQty, PayloadJson sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        IngredientCode = ingredient.Id,
                        // Meneruskan objek anonim yang mengelompokkan RulesetIngredientId, RulesetVersionId, RulesetGameAssetId, IngredientCode, ItemName, DisplayName,
                        // PurchasePrice, SortOrder, CardQty, PayloadJson sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        ItemName = ingredient.Nama,
                        // Meneruskan objek anonim yang mengelompokkan RulesetIngredientId, RulesetVersionId, RulesetGameAssetId, IngredientCode, ItemName, DisplayName,
                        // PurchasePrice, SortOrder, CardQty, PayloadJson sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        DisplayName = ingredient.Nama,
                        // Meneruskan objek anonim yang mengelompokkan RulesetIngredientId, RulesetVersionId, RulesetGameAssetId, IngredientCode, ItemName, DisplayName,
                        // PurchasePrice, SortOrder, CardQty, PayloadJson sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        PurchasePrice = ingredient.HargaBeli,
                        // Meneruskan objek anonim yang mengelompokkan RulesetIngredientId, RulesetVersionId, RulesetGameAssetId, IngredientCode, ItemName, DisplayName,
                        // PurchasePrice, SortOrder, CardQty, PayloadJson sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        SortOrder = index + 1,
                        // Meneruskan objek anonim yang mengelompokkan RulesetIngredientId, RulesetVersionId, RulesetGameAssetId, IngredientCode, ItemName, DisplayName,
                        // PurchasePrice, SortOrder, CardQty, PayloadJson sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        CardQty = ingredient.CardQty ?? 5,
                        // Meneruskan objek anonim yang mengelompokkan RulesetIngredientId, RulesetVersionId, RulesetGameAssetId, IngredientCode, ItemName, DisplayName,
                        // PurchasePrice, SortOrder, CardQty, PayloadJson sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        PayloadJson = "{}"
                    // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
                    // WriteRulesetDefinitionAsync.
                    },
                    // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
                    tx,
                    // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                    // bernama `cancellationToken`.
                    cancellationToken: ct));
        // Menutup scope loop dengan syarat `index < definition.Ingredients.Count`; bagian berikut berada di luar batas blok tersebut dalam
        // WriteRulesetDefinitionAsync.
        }

        // Menyiapkan variabel lokal `insertOrderSql` untuk nilai insert urutan/pesanan SQL dengan literal multiline yang dirinci pada komentar di dekat
        // deklarasinya. Tipe yang dipakai adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string insertOrderSql =
        // ”””`.
        // Baris literal 2: INSERT INTO menetapkan tabel dan kolom tujuan penambahan rekaman: `insert into ruleset_orders (`.
        // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_order_id,`.
        // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_version_id,`.
        // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_game_asset_id,`.
        // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `order_code,`.
        // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `item_name,`.
        // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `sell_price,`.
        // Baris literal 9: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `happiness_points,`.
        // Baris literal 10: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `sort_order,`.
        // Baris literal 11: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `card_qty,`.
        // Baris literal 12: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `is_active,`.
        // Baris literal 13: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `payload_json,`.
        // Baris literal 14: CREATE mendefinisikan objek basis data yang diperlukan proses ini: `created_at`.
        // Baris literal 15: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 16: VALUES menyediakan nilai baris baru sesuai urutan kolom INSERT; placeholder @ diikat ke parameter perintah: `values (`.
        // Baris literal 17: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@RulesetOrderId,`.
        // Baris literal 18: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@RulesetVersionId,`.
        // Baris literal 19: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@RulesetGameAssetId,`.
        // Baris literal 20: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@OrderCode,`.
        // Baris literal 21: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@ItemName,`.
        // Baris literal 22: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@SellPrice,`.
        // Baris literal 23: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@HappinessPoints,`.
        // Baris literal 24: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@SortOrder,`.
        // Baris literal 25: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@CardQty,`.
        // Baris literal 26: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `true,`.
        // Baris literal 27: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `'{}',`.
        // Baris literal 28: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `now()`.
        // Baris literal 29: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 30: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string insertOrderSql = """
            insert into ruleset_orders (
                ruleset_order_id,
                ruleset_version_id,
                ruleset_game_asset_id,
                order_code,
                item_name,
                sell_price,
                happiness_points,
                sort_order,
                card_qty,
                is_active,
                payload_json,
                created_at
            )
            values (
                @RulesetOrderId,
                @RulesetVersionId,
                @RulesetGameAssetId,
                @OrderCode,
                @ItemName,
                @SellPrice,
                @HappinessPoints,
                @SortOrder,
                @CardQty,
                true,
                '{}',
                now()
            )
            """;

        // Menyiapkan variabel lokal `insertOrderRequirementSql` untuk nilai insert urutan/pesanan requirement SQL dengan literal multiline yang dirinci
        // pada komentar di dekat deklarasinya. Tipe yang dipakai adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string
        // insertOrderRequirementSql = ”””`.
        // Baris literal 2: INSERT INTO menetapkan tabel dan kolom tujuan penambahan rekaman: `insert into ruleset_order_requirements (`.
        // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun:
        // `ruleset_order_requirement_id,`.
        // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_version_id,`.
        // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_order_id,`.
        // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `requirement_order,`.
        // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `required_asset_id,`.
        // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `qty_required,`.
        // Baris literal 9: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `payload_json,`.
        // Baris literal 10: CREATE mendefinisikan objek basis data yang diperlukan proses ini: `created_at`.
        // Baris literal 11: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 12: VALUES menyediakan nilai baris baru sesuai urutan kolom INSERT; placeholder @ diikat ke parameter perintah: `values (`.
        // Baris literal 13: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@RulesetOrderRequirementId,`.
        // Baris literal 14: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@RulesetVersionId,`.
        // Baris literal 15: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@RulesetOrderId,`.
        // Baris literal 16: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@RequirementOrder,`.
        // Baris literal 17: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@RequiredAssetId,`.
        // Baris literal 18: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@QtyRequired,`.
        // Baris literal 19: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `'{}',`.
        // Baris literal 20: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `now()`.
        // Baris literal 21: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 22: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string insertOrderRequirementSql = """
            insert into ruleset_order_requirements (
                ruleset_order_requirement_id,
                ruleset_version_id,
                ruleset_order_id,
                requirement_order,
                required_asset_id,
                qty_required,
                payload_json,
                created_at
            )
            values (
                @RulesetOrderRequirementId,
                @RulesetVersionId,
                @RulesetOrderId,
                @RequirementOrder,
                @RequiredAssetId,
                @QtyRequired,
                '{}',
                now()
            )
            """;

        // Mengulangi setiap elemen `definition.Orders.Select((item, index) => new { Item = item, Index = index })`; elemen saat ini disimpan sebagai
        // `orderEntry` bertipe `var` untuk diproses oleh badan loop dalam WriteRulesetDefinitionAsync.
        foreach (var orderEntry in definition.Orders.Select((item, index) => new { Item = item, Index = index }))
        // Membuka scope loop setiap orderEntry dari `definition.Orders.Select((item, index) => new { Item = item, Index = index })`; pernyataan/deklarasi
        // berikut berada di dalam batas blok ini dalam WriteRulesetDefinitionAsync.
        {
            // Menyiapkan variabel lokal `orderId` untuk nilai urutan/pesanan identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel
            // disimpulkan dari ekspresi nilai awal.
            var orderId = Guid.NewGuid();
            // Menyiapkan variabel lokal `orderAssetId` untuk nilai urutan/pesanan aset identitas dengan hasil operasi asinkron memanggil `InsertGameAssetAsync`
            // dengan `conn`, `tx`, `rulesetVersionId`, `”ORDER”`, `orderEntry.Item.Id`, `orderEntry.Item.Nama`, `orderEntry.Index + 1`, `ct`; await menunggu
            // hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var orderAssetId = await InsertGameAssetAsync(
                // Meneruskan `conn` (koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data) sebagai argumen ke `InsertGameAssetAsync`.
                conn,
                // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke `InsertGameAssetAsync`.
                tx,
                // Meneruskan `rulesetVersionId` (identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang tepat) sebagai argumen ke
                // `InsertGameAssetAsync`.
                rulesetVersionId,
                // Meneruskan nilai literal `”ORDER”` sebagai argumen ke `InsertGameAssetAsync`.
                "ORDER",
                // Meneruskan `orderEntry.Item.Id` (nilai identitas) sebagai argumen ke `InsertGameAssetAsync`.
                orderEntry.Item.Id,
                // Meneruskan `orderEntry.Item.Nama` (nilai nama) sebagai argumen ke `InsertGameAssetAsync`.
                orderEntry.Item.Nama,
                // Meneruskan penjumlahan/penggabungan antara `orderEntry.Index` dan `1` sebagai argumen ke `InsertGameAssetAsync`.
                orderEntry.Index + 1,
                // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                // ke `InsertGameAssetAsync`.
                ct);
            // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition( insertOrderSql, new {
            // RulesetOrderId = orderId, RulesetVersionId = rulesetVersionId, RulesetGameAssetId = orderAssetId, OrderCode = orderEntry.Item.Id, ...`; nilai
            // hasil menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam
            // WriteRulesetDefinitionAsync.
            await conn.ExecuteAsync(
                // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen ( insertOrderSql, new { RulesetOrderId = orderId, RulesetVersionId =
                // rulesetVersionId, RulesetGameAssetId = orderAssetId, OrderCode = orderEntry.Item.Id, ItemNa... sebagai argumen ke `conn.ExecuteAsync`.
                new CommandDefinition(
                    // Meneruskan `insertOrderSql` (nilai insert urutan/pesanan SQL) sebagai argumen ke konstruktor `CommandDefinition`.
                    insertOrderSql,
                    // Meneruskan objek anonim yang mengelompokkan RulesetOrderId, RulesetVersionId, RulesetGameAssetId, OrderCode, ItemName, SellPrice,
                    // HappinessPoints, SortOrder, orderEntry.Item.CardQty sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                    new
                    // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                    // WriteRulesetDefinitionAsync.
                    {
                        // Meneruskan objek anonim yang mengelompokkan RulesetOrderId, RulesetVersionId, RulesetGameAssetId, OrderCode, ItemName, SellPrice,
                        // HappinessPoints, SortOrder, orderEntry.Item.CardQty sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        RulesetOrderId = orderId,
                        // Meneruskan objek anonim yang mengelompokkan RulesetOrderId, RulesetVersionId, RulesetGameAssetId, OrderCode, ItemName, SellPrice,
                        // HappinessPoints, SortOrder, orderEntry.Item.CardQty sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        RulesetVersionId = rulesetVersionId,
                        // Meneruskan objek anonim yang mengelompokkan RulesetOrderId, RulesetVersionId, RulesetGameAssetId, OrderCode, ItemName, SellPrice,
                        // HappinessPoints, SortOrder, orderEntry.Item.CardQty sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        RulesetGameAssetId = orderAssetId,
                        // Meneruskan objek anonim yang mengelompokkan RulesetOrderId, RulesetVersionId, RulesetGameAssetId, OrderCode, ItemName, SellPrice,
                        // HappinessPoints, SortOrder, orderEntry.Item.CardQty sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        OrderCode = orderEntry.Item.Id,
                        // Meneruskan objek anonim yang mengelompokkan RulesetOrderId, RulesetVersionId, RulesetGameAssetId, OrderCode, ItemName, SellPrice,
                        // HappinessPoints, SortOrder, orderEntry.Item.CardQty sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        ItemName = orderEntry.Item.Nama,
                        // Meneruskan objek anonim yang mengelompokkan RulesetOrderId, RulesetVersionId, RulesetGameAssetId, OrderCode, ItemName, SellPrice,
                        // HappinessPoints, SortOrder, orderEntry.Item.CardQty sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        SellPrice = orderEntry.Item.HargaJual,
                        // Meneruskan objek anonim yang mengelompokkan RulesetOrderId, RulesetVersionId, RulesetGameAssetId, OrderCode, ItemName, SellPrice,
                        // HappinessPoints, SortOrder, orderEntry.Item.CardQty sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        HappinessPoints = orderEntry.Item.PoinKebahagiaan,
                        // Meneruskan objek anonim yang mengelompokkan RulesetOrderId, RulesetVersionId, RulesetGameAssetId, OrderCode, ItemName, SellPrice,
                        // HappinessPoints, SortOrder, orderEntry.Item.CardQty sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        SortOrder = orderEntry.Index + 1,
                        // Meneruskan objek anonim yang mengelompokkan RulesetOrderId, RulesetVersionId, RulesetGameAssetId, OrderCode, ItemName, SellPrice,
                        // HappinessPoints, SortOrder, orderEntry.Item.CardQty sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        orderEntry.Item.CardQty
                    // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
                    // WriteRulesetDefinitionAsync.
                    },
                    // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
                    tx,
                    // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                    // bernama `cancellationToken`.
                    cancellationToken: ct));

            // Mengulangi setiap elemen `orderEntry.Item.Bahan.Select((name, requirementIndex) => new { Name = name, RequirementIndex = requirementIndex })`;
            // elemen saat ini disimpan sebagai `requirement` bertipe `var` untuk diproses oleh badan loop dalam WriteRulesetDefinitionAsync.
            foreach (var requirement in orderEntry.Item.Bahan.Select((name, requirementIndex) => new { Name = name, RequirementIndex = requirementIndex }))
            // Membuka scope loop setiap requirement dari `orderEntry.Item.Bahan.Select((name, requirementIndex) => new { Name = name, RequirementIndex =
            // requirementIndex })`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam WriteRulesetDefinitionAsync.
            {
                // Memeriksa kebalikan kondisi `ingredientAssetIds.TryGetValue(requirement.Name, out var requiredAssetId)`; blok if hanya dijalankan ketika kondisi
                // ini bernilai benar dalam WriteRulesetDefinitionAsync.
                if (!ingredientAssetIds.TryGetValue(requirement.Name, out var requiredAssetId))
                // Membuka scope cabang if untuk kondisi `!ingredientAssetIds.TryGetValue(requirement.Name, out var requiredAssetId)`; pernyataan/deklarasi berikut
                // berada di dalam batas blok ini dalam WriteRulesetDefinitionAsync.
                {
                    // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen ($”Ingredient requirement '{requirement.Name}'
                    // tidak ditemukan dalam ruleset.”) dalam WriteRulesetDefinitionAsync; pemanggil atau middleware penanganan error menerima kegagalan ini.
                    throw new InvalidOperationException($"Ingredient requirement '{requirement.Name}' tidak ditemukan dalam ruleset.");
                // Menutup scope cabang if untuk kondisi `!ingredientAssetIds.TryGetValue(requirement.Name, out var requiredAssetId)`; bagian berikut berada di luar
                // batas blok tersebut dalam WriteRulesetDefinitionAsync.
                }

                // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition( insertOrderRequirementSql, new {
                // RulesetOrderRequirementId = Guid.NewGuid(), RulesetVersionId = rulesetVersionId, RulesetOrderId = orderId, RequirementO...`; nilai hasil
                // menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam
                // WriteRulesetDefinitionAsync.
                await conn.ExecuteAsync(
                    // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen ( insertOrderRequirementSql, new { RulesetOrderRequirementId = Guid.NewGuid(),
                    // RulesetVersionId = rulesetVersionId, RulesetOrderId = orderId, RequirementOrder =... sebagai argumen ke `conn.ExecuteAsync`.
                    new CommandDefinition(
                        // Meneruskan `insertOrderRequirementSql` (nilai insert urutan/pesanan requirement SQL) sebagai argumen ke konstruktor `CommandDefinition`.
                        insertOrderRequirementSql,
                        // Meneruskan objek anonim yang mengelompokkan RulesetOrderRequirementId, RulesetVersionId, RulesetOrderId, RequirementOrder, RequiredAssetId,
                        // QtyRequired sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        new
                        // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                        // WriteRulesetDefinitionAsync.
                        {
                            // Meneruskan objek anonim yang mengelompokkan RulesetOrderRequirementId, RulesetVersionId, RulesetOrderId, RequirementOrder, RequiredAssetId,
                            // QtyRequired sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                            RulesetOrderRequirementId = Guid.NewGuid(),
                            // Meneruskan objek anonim yang mengelompokkan RulesetOrderRequirementId, RulesetVersionId, RulesetOrderId, RequirementOrder, RequiredAssetId,
                            // QtyRequired sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                            RulesetVersionId = rulesetVersionId,
                            // Meneruskan objek anonim yang mengelompokkan RulesetOrderRequirementId, RulesetVersionId, RulesetOrderId, RequirementOrder, RequiredAssetId,
                            // QtyRequired sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                            RulesetOrderId = orderId,
                            // Meneruskan objek anonim yang mengelompokkan RulesetOrderRequirementId, RulesetVersionId, RulesetOrderId, RequirementOrder, RequiredAssetId,
                            // QtyRequired sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                            RequirementOrder = requirement.RequirementIndex + 1,
                            // Meneruskan objek anonim yang mengelompokkan RulesetOrderRequirementId, RulesetVersionId, RulesetOrderId, RequirementOrder, RequiredAssetId,
                            // QtyRequired sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                            RequiredAssetId = requiredAssetId,
                            // Meneruskan objek anonim yang mengelompokkan RulesetOrderRequirementId, RulesetVersionId, RulesetOrderId, RequirementOrder, RequiredAssetId,
                            // QtyRequired sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                            QtyRequired = 1
                        // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
                        // WriteRulesetDefinitionAsync.
                        },
                        // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
                        tx,
                        // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                        // bernama `cancellationToken`.
                        cancellationToken: ct));
            // Menutup scope loop setiap requirement dari `orderEntry.Item.Bahan.Select((name, requirementIndex) => new { Name = name, RequirementIndex =
            // requirementIndex })`; bagian berikut berada di luar batas blok tersebut dalam WriteRulesetDefinitionAsync.
            }
        // Menutup scope loop setiap orderEntry dari `definition.Orders.Select((item, index) => new { Item = item, Index = index })`; bagian berikut berada
        // di luar batas blok tersebut dalam WriteRulesetDefinitionAsync.
        }

        // Menyiapkan variabel lokal `insertNeedSql` untuk nilai insert kebutuhan SQL dengan literal multiline yang dirinci pada komentar di dekat
        // deklarasinya. Tipe yang dipakai adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string insertNeedSql =
        // ”””`.
        // Baris literal 2: INSERT INTO menetapkan tabel dan kolom tujuan penambahan rekaman: `insert into ruleset_needs (`.
        // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_need_id,`.
        // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_version_id,`.
        // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_game_asset_id,`.
        // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `need_code,`.
        // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `item_name,`.
        // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `need_family_code,`.
        // Baris literal 9: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `need_tier,`.
        // Baris literal 10: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `purchase_price,`.
        // Baris literal 11: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `happiness_points,`.
        // Baris literal 12: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `sort_order,`.
        // Baris literal 13: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `card_qty,`.
        // Baris literal 14: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `is_active,`.
        // Baris literal 15: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `payload_json,`.
        // Baris literal 16: CREATE mendefinisikan objek basis data yang diperlukan proses ini: `created_at`.
        // Baris literal 17: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 18: VALUES menyediakan nilai baris baru sesuai urutan kolom INSERT; placeholder @ diikat ke parameter perintah: `values (`.
        // Baris literal 19: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@RulesetNeedId,`.
        // Baris literal 20: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@RulesetVersionId,`.
        // Baris literal 21: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@RulesetGameAssetId,`.
        // Baris literal 22: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@NeedCode,`.
        // Baris literal 23: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@ItemName,`.
        // Baris literal 24: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@NeedFamilyCode,`.
        // Baris literal 25: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@NeedTier,`.
        // Baris literal 26: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@PurchasePrice,`.
        // Baris literal 27: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@HappinessPoints,`.
        // Baris literal 28: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@SortOrder,`.
        // Baris literal 29: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@CardQty,`.
        // Baris literal 30: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `true,`.
        // Baris literal 31: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `'{}',`.
        // Baris literal 32: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `now()`.
        // Baris literal 33: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 34: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string insertNeedSql = """
            insert into ruleset_needs (
                ruleset_need_id,
                ruleset_version_id,
                ruleset_game_asset_id,
                need_code,
                item_name,
                need_family_code,
                need_tier,
                purchase_price,
                happiness_points,
                sort_order,
                card_qty,
                is_active,
                payload_json,
                created_at
            )
            values (
                @RulesetNeedId,
                @RulesetVersionId,
                @RulesetGameAssetId,
                @NeedCode,
                @ItemName,
                @NeedFamilyCode,
                @NeedTier,
                @PurchasePrice,
                @HappinessPoints,
                @SortOrder,
                @CardQty,
                true,
                '{}',
                now()
            )
            """;

        // Menyiapkan variabel lokal `needAssetIds` untuk nilai kebutuhan aset identitas dengan objek baru bertipe `Dictionary<string, Guid>` dengan argumen
        // (StringComparer.OrdinalIgnoreCase). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var needAssetIds = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);
        // Memulai loop dengan inisialisasi `var index = 0`, berjalan selama `index < definition.Needs.Count`, lalu memperbarui pencacah melalui `index++`
        // dalam WriteRulesetDefinitionAsync.
        for (var index = 0; index < definition.Needs.Count; index++)
        // Membuka scope loop dengan syarat `index < definition.Needs.Count`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // WriteRulesetDefinitionAsync.
        {
            // Menyiapkan variabel lokal `need` untuk nilai kebutuhan dengan `definition.Needs[index]`, yaitu elemen koleksi yang dipilih melalui indeks atau
            // kunci tersebut. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var need = definition.Needs[index];
            // Menyiapkan variabel lokal `assetId` untuk nilai aset identitas dengan hasil operasi asinkron memanggil `InsertGameAssetAsync` dengan `conn`,
            // `tx`, `rulesetVersionId`, `”NEED”`, `need.Id`, `need.Nama`, `index + 1`, `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum
            // selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var assetId = await InsertGameAssetAsync(
                // Meneruskan `conn` (koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data) sebagai argumen ke `InsertGameAssetAsync`.
                conn,
                // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke `InsertGameAssetAsync`.
                tx,
                // Meneruskan `rulesetVersionId` (identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang tepat) sebagai argumen ke
                // `InsertGameAssetAsync`.
                rulesetVersionId,
                // Meneruskan nilai literal `”NEED”` sebagai argumen ke `InsertGameAssetAsync`.
                "NEED",
                // Meneruskan `need.Id` (nilai identitas) sebagai argumen ke `InsertGameAssetAsync`.
                need.Id,
                // Meneruskan `need.Nama` (nilai nama) sebagai argumen ke `InsertGameAssetAsync`.
                need.Nama,
                // Meneruskan penjumlahan/penggabungan antara `index` dan `1` sebagai argumen ke `InsertGameAssetAsync`.
                index + 1,
                // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                // ke `InsertGameAssetAsync`.
                ct);
            // Memperbarui `needAssetIds[need.Id]` menggunakan `assetId` (nilai aset identitas) dalam WriteRulesetDefinitionAsync.
            needAssetIds[need.Id] = assetId;
            // Memperbarui `needAssetIds[need.Nama]` menggunakan `assetId` (nilai aset identitas) dalam WriteRulesetDefinitionAsync.
            needAssetIds[need.Nama] = assetId;
            // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition( insertNeedSql, new { RulesetNeedId
            // = Guid.NewGuid(), RulesetVersionId = rulesetVersionId, RulesetGameAssetId = assetId, NeedCode = need.Id, ItemName = n...`; nilai hasil
            // menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam
            // WriteRulesetDefinitionAsync.
            await conn.ExecuteAsync(
                // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen ( insertNeedSql, new { RulesetNeedId = Guid.NewGuid(), RulesetVersionId =
                // rulesetVersionId, RulesetGameAssetId = assetId, NeedCode = need.Id, ItemName = need.Na... sebagai argumen ke `conn.ExecuteAsync`.
                new CommandDefinition(
                    // Meneruskan `insertNeedSql` (nilai insert kebutuhan SQL) sebagai argumen ke konstruktor `CommandDefinition`.
                    insertNeedSql,
                    // Meneruskan objek anonim yang mengelompokkan RulesetNeedId, RulesetVersionId, RulesetGameAssetId, NeedCode, ItemName, NeedFamilyCode, NeedTier,
                    // PurchasePrice, HappinessPoints, SortOrder, CardQty sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                    new
                    // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                    // WriteRulesetDefinitionAsync.
                    {
                        // Meneruskan objek anonim yang mengelompokkan RulesetNeedId, RulesetVersionId, RulesetGameAssetId, NeedCode, ItemName, NeedFamilyCode, NeedTier,
                        // PurchasePrice, HappinessPoints, SortOrder, CardQty sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        RulesetNeedId = Guid.NewGuid(),
                        // Meneruskan objek anonim yang mengelompokkan RulesetNeedId, RulesetVersionId, RulesetGameAssetId, NeedCode, ItemName, NeedFamilyCode, NeedTier,
                        // PurchasePrice, HappinessPoints, SortOrder, CardQty sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        RulesetVersionId = rulesetVersionId,
                        // Meneruskan objek anonim yang mengelompokkan RulesetNeedId, RulesetVersionId, RulesetGameAssetId, NeedCode, ItemName, NeedFamilyCode, NeedTier,
                        // PurchasePrice, HappinessPoints, SortOrder, CardQty sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        RulesetGameAssetId = assetId,
                        // Meneruskan objek anonim yang mengelompokkan RulesetNeedId, RulesetVersionId, RulesetGameAssetId, NeedCode, ItemName, NeedFamilyCode, NeedTier,
                        // PurchasePrice, HappinessPoints, SortOrder, CardQty sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        NeedCode = need.Id,
                        // Meneruskan objek anonim yang mengelompokkan RulesetNeedId, RulesetVersionId, RulesetGameAssetId, NeedCode, ItemName, NeedFamilyCode, NeedTier,
                        // PurchasePrice, HappinessPoints, SortOrder, CardQty sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        ItemName = need.Nama,
                        // Meneruskan `need.Family` (nilai kelompok) sebagai argumen ke `string.IsNullOrWhiteSpace`.
                        NeedFamilyCode = string.IsNullOrWhiteSpace(need.Family) ? need.Id : need.Family,
                        // Meneruskan objek anonim yang mengelompokkan RulesetNeedId, RulesetVersionId, RulesetGameAssetId, NeedCode, ItemName, NeedFamilyCode, NeedTier,
                        // PurchasePrice, HappinessPoints, SortOrder, CardQty sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        NeedTier = need.Tipe,
                        // Meneruskan objek anonim yang mengelompokkan RulesetNeedId, RulesetVersionId, RulesetGameAssetId, NeedCode, ItemName, NeedFamilyCode, NeedTier,
                        // PurchasePrice, HappinessPoints, SortOrder, CardQty sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        PurchasePrice = need.HargaBeli,
                        // Meneruskan objek anonim yang mengelompokkan RulesetNeedId, RulesetVersionId, RulesetGameAssetId, NeedCode, ItemName, NeedFamilyCode, NeedTier,
                        // PurchasePrice, HappinessPoints, SortOrder, CardQty sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        HappinessPoints = need.PoinKebahagiaan,
                        // Meneruskan objek anonim yang mengelompokkan RulesetNeedId, RulesetVersionId, RulesetGameAssetId, NeedCode, ItemName, NeedFamilyCode, NeedTier,
                        // PurchasePrice, HappinessPoints, SortOrder, CardQty sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        SortOrder = index + 1,
                        // Meneruskan objek anonim yang mengelompokkan RulesetNeedId, RulesetVersionId, RulesetGameAssetId, NeedCode, ItemName, NeedFamilyCode, NeedTier,
                        // PurchasePrice, HappinessPoints, SortOrder, CardQty sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        CardQty = need.CardQty ?? 1
                    // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
                    // WriteRulesetDefinitionAsync.
                    },
                    // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
                    tx,
                    // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                    // bernama `cancellationToken`.
                    cancellationToken: ct));
        // Menutup scope loop dengan syarat `index < definition.Needs.Count`; bagian berikut berada di luar batas blok tersebut dalam
        // WriteRulesetDefinitionAsync.
        }

        // Menyiapkan variabel lokal `insertNeedSetBonusSql` untuk nilai insert kebutuhan set bonus SQL dengan literal multiline yang dirinci pada komentar
        // di dekat deklarasinya. Tipe yang dipakai adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string
        // insertNeedSetBonusSql = ”””`.
        // Baris literal 2: INSERT INTO menetapkan tabel dan kolom tujuan penambahan rekaman: `insert into ruleset_need_set_bonuses (`.
        // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_need_set_bonus_id,`.
        // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_version_id,`.
        // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `pattern_code,`.
        // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `required_count,`.
        // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `points,`.
        // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `sort_order,`.
        // Baris literal 9: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `payload_json,`.
        // Baris literal 10: CREATE mendefinisikan objek basis data yang diperlukan proses ini: `created_at`.
        // Baris literal 11: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 12: VALUES menyediakan nilai baris baru sesuai urutan kolom INSERT; placeholder @ diikat ke parameter perintah: `values (`.
        // Baris literal 13: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@RulesetNeedSetBonusId,`.
        // Baris literal 14: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@RulesetVersionId,`.
        // Baris literal 15: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@PatternCode,`.
        // Baris literal 16: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@RequiredCount,`.
        // Baris literal 17: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@Points,`.
        // Baris literal 18: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@SortOrder,`.
        // Baris literal 19: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `'{}',`.
        // Baris literal 20: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `now()`.
        // Baris literal 21: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 22: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string insertNeedSetBonusSql = """
            insert into ruleset_need_set_bonuses (
                ruleset_need_set_bonus_id,
                ruleset_version_id,
                pattern_code,
                required_count,
                points,
                sort_order,
                payload_json,
                created_at
            )
            values (
                @RulesetNeedSetBonusId,
                @RulesetVersionId,
                @PatternCode,
                @RequiredCount,
                @Points,
                @SortOrder,
                '{}',
                now()
            )
            """;

        // Memulai loop dengan inisialisasi `var index = 0`, berjalan selama `index < definition.NeedSetBonuses.Count`, lalu memperbarui pencacah melalui
        // `index++` dalam WriteRulesetDefinitionAsync.
        for (var index = 0; index < definition.NeedSetBonuses.Count; index++)
        // Membuka scope loop dengan syarat `index < definition.NeedSetBonuses.Count`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // WriteRulesetDefinitionAsync.
        {
            // Menyiapkan variabel lokal `bonus` untuk nilai bonus dengan `definition.NeedSetBonuses[index]`, yaitu elemen koleksi yang dipilih melalui indeks
            // atau kunci tersebut. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var bonus = definition.NeedSetBonuses[index];
            // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition( insertNeedSetBonusSql, new {
            // RulesetNeedSetBonusId = Guid.NewGuid(), RulesetVersionId = rulesetVersionId, bonus.PatternCode, bonus.RequiredCount, bonus....`; nilai hasil
            // menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam
            // WriteRulesetDefinitionAsync.
            await conn.ExecuteAsync(
                // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen ( insertNeedSetBonusSql, new { RulesetNeedSetBonusId = Guid.NewGuid(),
                // RulesetVersionId = rulesetVersionId, bonus.PatternCode, bonus.RequiredCount, bonus.Points... sebagai argumen ke `conn.ExecuteAsync`.
                new CommandDefinition(
                    // Meneruskan `insertNeedSetBonusSql` (nilai insert kebutuhan set bonus SQL) sebagai argumen ke konstruktor `CommandDefinition`.
                    insertNeedSetBonusSql,
                    // Meneruskan objek anonim yang mengelompokkan RulesetNeedSetBonusId, RulesetVersionId, bonus.PatternCode, bonus.RequiredCount, bonus.Points,
                    // SortOrder sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                    new
                    // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                    // WriteRulesetDefinitionAsync.
                    {
                        // Meneruskan objek anonim yang mengelompokkan RulesetNeedSetBonusId, RulesetVersionId, bonus.PatternCode, bonus.RequiredCount, bonus.Points,
                        // SortOrder sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        RulesetNeedSetBonusId = Guid.NewGuid(),
                        // Meneruskan objek anonim yang mengelompokkan RulesetNeedSetBonusId, RulesetVersionId, bonus.PatternCode, bonus.RequiredCount, bonus.Points,
                        // SortOrder sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        RulesetVersionId = rulesetVersionId,
                        // Meneruskan objek anonim yang mengelompokkan RulesetNeedSetBonusId, RulesetVersionId, bonus.PatternCode, bonus.RequiredCount, bonus.Points,
                        // SortOrder sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        bonus.PatternCode,
                        // Meneruskan objek anonim yang mengelompokkan RulesetNeedSetBonusId, RulesetVersionId, bonus.PatternCode, bonus.RequiredCount, bonus.Points,
                        // SortOrder sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        bonus.RequiredCount,
                        // Meneruskan objek anonim yang mengelompokkan RulesetNeedSetBonusId, RulesetVersionId, bonus.PatternCode, bonus.RequiredCount, bonus.Points,
                        // SortOrder sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        bonus.Points,
                        // Meneruskan objek anonim yang mengelompokkan RulesetNeedSetBonusId, RulesetVersionId, bonus.PatternCode, bonus.RequiredCount, bonus.Points,
                        // SortOrder sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        SortOrder = index + 1
                    // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
                    // WriteRulesetDefinitionAsync.
                    },
                    // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
                    tx,
                    // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                    // bernama `cancellationToken`.
                    cancellationToken: ct));
        // Menutup scope loop dengan syarat `index < definition.NeedSetBonuses.Count`; bagian berikut berada di luar batas blok tersebut dalam
        // WriteRulesetDefinitionAsync.
        }

        // Menyiapkan variabel lokal `insertMissionSql` untuk nilai insert misi SQL dengan literal multiline yang dirinci pada komentar di dekat
        // deklarasinya. Tipe yang dipakai adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string insertMissionSql
        // = ”””`.
        // Baris literal 2: INSERT INTO menetapkan tabel dan kolom tujuan penambahan rekaman: `insert into ruleset_collection_missions (`.
        // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun:
        // `ruleset_collection_mission_id,`.
        // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_version_id,`.
        // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `mission_code,`.
        // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `item_name,`.
        // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `success_points,`.
        // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `failure_points,`.
        // Baris literal 9: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `penalty_points,`.
        // Baris literal 10: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `sort_order,`.
        // Baris literal 11: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `card_qty,`.
        // Baris literal 12: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `is_active,`.
        // Baris literal 13: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `payload_json,`.
        // Baris literal 14: CREATE mendefinisikan objek basis data yang diperlukan proses ini: `created_at`.
        // Baris literal 15: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 16: VALUES menyediakan nilai baris baru sesuai urutan kolom INSERT; placeholder @ diikat ke parameter perintah: `values (`.
        // Baris literal 17: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@RulesetCollectionMissionId,`.
        // Baris literal 18: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@RulesetVersionId,`.
        // Baris literal 19: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@MissionCode,`.
        // Baris literal 20: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@ItemName,`.
        // Baris literal 21: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@SuccessPoints,`.
        // Baris literal 22: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@FailurePoints,`.
        // Baris literal 23: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@PenaltyPoints,`.
        // Baris literal 24: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@SortOrder,`.
        // Baris literal 25: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `null,`.
        // Baris literal 26: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `true,`.
        // Baris literal 27: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `'{}',`.
        // Baris literal 28: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `now()`.
        // Baris literal 29: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 30: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string insertMissionSql = """
            insert into ruleset_collection_missions (
                ruleset_collection_mission_id,
                ruleset_version_id,
                mission_code,
                item_name,
                success_points,
                failure_points,
                penalty_points,
                sort_order,
                card_qty,
                is_active,
                payload_json,
                created_at
            )
            values (
                @RulesetCollectionMissionId,
                @RulesetVersionId,
                @MissionCode,
                @ItemName,
                @SuccessPoints,
                @FailurePoints,
                @PenaltyPoints,
                @SortOrder,
                null,
                true,
                '{}',
                now()
            )
            """;

        // Menyiapkan variabel lokal `insertMissionRequirementSql` untuk nilai insert misi requirement SQL dengan literal multiline yang dirinci pada
        // komentar di dekat deklarasinya. Tipe yang dipakai adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string
        // insertMissionRequirementSql = ”””`.
        // Baris literal 2: INSERT INTO menetapkan tabel dan kolom tujuan penambahan rekaman: `insert into ruleset_collection_mission_requirements (`.
        // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun:
        // `ruleset_collection_mission_requirement_id,`.
        // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_version_id,`.
        // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun:
        // `ruleset_collection_mission_id,`.
        // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `requirement_order,`.
        // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `requirement_type,`.
        // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `required_asset_id,`.
        // Baris literal 9: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `required_need_tier,`.
        // Baris literal 10: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `required_need_family_code,`.
        // Baris literal 11: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `qty_required,`.
        // Baris literal 12: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `payload_json,`.
        // Baris literal 13: CREATE mendefinisikan objek basis data yang diperlukan proses ini: `created_at`.
        // Baris literal 14: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 15: VALUES menyediakan nilai baris baru sesuai urutan kolom INSERT; placeholder @ diikat ke parameter perintah: `values (`.
        // Baris literal 16: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@RulesetCollectionMissionRequirementId,`.
        // Baris literal 17: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@RulesetVersionId,`.
        // Baris literal 18: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@RulesetCollectionMissionId,`.
        // Baris literal 19: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@RequirementOrder,`.
        // Baris literal 20: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@RequirementType,`.
        // Baris literal 21: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@RequiredAssetId,`.
        // Baris literal 22: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@RequiredNeedTier,`.
        // Baris literal 23: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@RequiredNeedFamilyCode,`.
        // Baris literal 24: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `null,`.
        // Baris literal 25: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `'{}',`.
        // Baris literal 26: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `now()`.
        // Baris literal 27: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 28: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string insertMissionRequirementSql = """
            insert into ruleset_collection_mission_requirements (
                ruleset_collection_mission_requirement_id,
                ruleset_version_id,
                ruleset_collection_mission_id,
                requirement_order,
                requirement_type,
                required_asset_id,
                required_need_tier,
                required_need_family_code,
                qty_required,
                payload_json,
                created_at
            )
            values (
                @RulesetCollectionMissionRequirementId,
                @RulesetVersionId,
                @RulesetCollectionMissionId,
                @RequirementOrder,
                @RequirementType,
                @RequiredAssetId,
                @RequiredNeedTier,
                @RequiredNeedFamilyCode,
                null,
                '{}',
                now()
            )
            """;

        // Mengulangi setiap elemen `definition.CollectionMissions.Select((item, index) => new { Item = item, Index = index })`; elemen saat ini disimpan
        // sebagai `missionEntry` bertipe `var` untuk diproses oleh badan loop dalam WriteRulesetDefinitionAsync.
        foreach (var missionEntry in definition.CollectionMissions.Select((item, index) => new { Item = item, Index = index }))
        // Membuka scope loop setiap missionEntry dari `definition.CollectionMissions.Select((item, index) => new { Item = item, Index = index })`;
        // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam WriteRulesetDefinitionAsync.
        {
            // Menyiapkan variabel lokal `missionId` untuk identitas misi koleksi yang ditugaskan dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe
            // variabel disimpulkan dari ekspresi nilai awal.
            var missionId = Guid.NewGuid();
            // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition( insertMissionSql, new {
            // RulesetCollectionMissionId = missionId, RulesetVersionId = rulesetVersionId, MissionCode = missionEntry.Item.Id, ItemName = miss...`; nilai hasil
            // menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam
            // WriteRulesetDefinitionAsync.
            await conn.ExecuteAsync(
                // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen ( insertMissionSql, new { RulesetCollectionMissionId = missionId,
                // RulesetVersionId = rulesetVersionId, MissionCode = missionEntry.Item.Id, ItemName = missionEnt... sebagai argumen ke `conn.ExecuteAsync`.
                new CommandDefinition(
                    // Meneruskan `insertMissionSql` (nilai insert misi SQL) sebagai argumen ke konstruktor `CommandDefinition`.
                    insertMissionSql,
                    // Meneruskan objek anonim yang mengelompokkan RulesetCollectionMissionId, RulesetVersionId, MissionCode, ItemName, missionEntry.Item.SuccessPoints,
                    // missionEntry.Item.FailurePoints, missionEntry.Item.PenaltyPoints, SortOrder sebagai satu nilai sebagai argumen ke konstruktor
                    // `CommandDefinition`.
                    new
                    // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                    // WriteRulesetDefinitionAsync.
                    {
                        // Meneruskan objek anonim yang mengelompokkan RulesetCollectionMissionId, RulesetVersionId, MissionCode, ItemName, missionEntry.Item.SuccessPoints,
                        // missionEntry.Item.FailurePoints, missionEntry.Item.PenaltyPoints, SortOrder sebagai satu nilai sebagai argumen ke konstruktor
                        // `CommandDefinition`.
                        RulesetCollectionMissionId = missionId,
                        // Meneruskan objek anonim yang mengelompokkan RulesetCollectionMissionId, RulesetVersionId, MissionCode, ItemName, missionEntry.Item.SuccessPoints,
                        // missionEntry.Item.FailurePoints, missionEntry.Item.PenaltyPoints, SortOrder sebagai satu nilai sebagai argumen ke konstruktor
                        // `CommandDefinition`.
                        RulesetVersionId = rulesetVersionId,
                        // Meneruskan objek anonim yang mengelompokkan RulesetCollectionMissionId, RulesetVersionId, MissionCode, ItemName, missionEntry.Item.SuccessPoints,
                        // missionEntry.Item.FailurePoints, missionEntry.Item.PenaltyPoints, SortOrder sebagai satu nilai sebagai argumen ke konstruktor
                        // `CommandDefinition`.
                        MissionCode = missionEntry.Item.Id,
                        // Meneruskan objek anonim yang mengelompokkan RulesetCollectionMissionId, RulesetVersionId, MissionCode, ItemName, missionEntry.Item.SuccessPoints,
                        // missionEntry.Item.FailurePoints, missionEntry.Item.PenaltyPoints, SortOrder sebagai satu nilai sebagai argumen ke konstruktor
                        // `CommandDefinition`.
                        ItemName = missionEntry.Item.Nama,
                        // Meneruskan objek anonim yang mengelompokkan RulesetCollectionMissionId, RulesetVersionId, MissionCode, ItemName, missionEntry.Item.SuccessPoints,
                        // missionEntry.Item.FailurePoints, missionEntry.Item.PenaltyPoints, SortOrder sebagai satu nilai sebagai argumen ke konstruktor
                        // `CommandDefinition`.
                        missionEntry.Item.SuccessPoints,
                        // Meneruskan objek anonim yang mengelompokkan RulesetCollectionMissionId, RulesetVersionId, MissionCode, ItemName, missionEntry.Item.SuccessPoints,
                        // missionEntry.Item.FailurePoints, missionEntry.Item.PenaltyPoints, SortOrder sebagai satu nilai sebagai argumen ke konstruktor
                        // `CommandDefinition`.
                        missionEntry.Item.FailurePoints,
                        // Meneruskan objek anonim yang mengelompokkan RulesetCollectionMissionId, RulesetVersionId, MissionCode, ItemName, missionEntry.Item.SuccessPoints,
                        // missionEntry.Item.FailurePoints, missionEntry.Item.PenaltyPoints, SortOrder sebagai satu nilai sebagai argumen ke konstruktor
                        // `CommandDefinition`.
                        missionEntry.Item.PenaltyPoints,
                        // Meneruskan objek anonim yang mengelompokkan RulesetCollectionMissionId, RulesetVersionId, MissionCode, ItemName, missionEntry.Item.SuccessPoints,
                        // missionEntry.Item.FailurePoints, missionEntry.Item.PenaltyPoints, SortOrder sebagai satu nilai sebagai argumen ke konstruktor
                        // `CommandDefinition`.
                        SortOrder = missionEntry.Index + 1
                    // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
                    // WriteRulesetDefinitionAsync.
                    },
                    // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
                    tx,
                    // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                    // bernama `cancellationToken`.
                    cancellationToken: ct));

            // Mengulangi setiap elemen `missionEntry.Item.KebutuhanTarget`; elemen saat ini disimpan sebagai `requirement` bertipe `var` untuk diproses oleh
            // badan loop dalam WriteRulesetDefinitionAsync.
            foreach (var requirement in missionEntry.Item.KebutuhanTarget)
            // Membuka scope loop setiap requirement dari `missionEntry.Item.KebutuhanTarget`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // WriteRulesetDefinitionAsync.
            {
                // Menyiapkan variabel lokal `isTier` untuk nilai berstatus tingkat dengan gabungan syarat OR: setidaknya satu kondisi wajib benar antara
                // `string.Equals(requirement.Type, ”TIER”, StringComparison.OrdinalIgnoreCase)` dan `string.Equals(requirement.Type, ”NEED_TIER”,
                // StringComparison.OrdinalIgnoreCase)`; sisi kanan diperiksa hanya jika sisi kiri salah. Tipe variabel disimpulkan dari ekspresi nilai awal.
                var isTier = string.Equals(requirement.Type, "TIER", StringComparison.OrdinalIgnoreCase)
                             // Melengkapi struktur ekspresi LogicalOrExpression melalui || string.Equals(requirement.Type, ”NEED_TIER”, StringComparison.OrdinalIgnoreCase);
                             // dalam WriteRulesetDefinitionAsync; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
                             || string.Equals(requirement.Type, "NEED_TIER", StringComparison.OrdinalIgnoreCase);
                // Menyiapkan variabel lokal `isFamily` untuk nilai berstatus kelompok dengan gabungan syarat OR: setidaknya satu kondisi wajib benar antara
                // `string.Equals(requirement.Type, ”FAMILY”, StringComparison.OrdinalIgnoreCase)` dan `string.Equals(requirement.Type, ”NEED_FAMILY”,
                // StringComparison.OrdinalIgnoreCase)`; sisi kanan diperiksa hanya jika sisi kiri salah. Tipe variabel disimpulkan dari ekspresi nilai awal.
                var isFamily = string.Equals(requirement.Type, "FAMILY", StringComparison.OrdinalIgnoreCase)
                               // Melengkapi struktur ekspresi LogicalOrExpression melalui || string.Equals(requirement.Type, ”NEED_FAMILY”, StringComparison.OrdinalIgnoreCase);
                               // dalam WriteRulesetDefinitionAsync; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
                               || string.Equals(requirement.Type, "NEED_FAMILY", StringComparison.OrdinalIgnoreCase);
                // Menyiapkan variabel lokal `requiredAssetId` untuk nilai required aset identitas dengan null, yaitu penanda tidak ada nilai. Tipe yang dipakai
                // adalah `Guid?`.
                Guid? requiredAssetId = null;
                // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `!isTier` dan `!isFamily`; sisi kanan diperiksa hanya jika sisi kiri benar; blok
                // if hanya dijalankan ketika kondisi ini bernilai benar dalam WriteRulesetDefinitionAsync.
                if (!isTier && !isFamily)
                // Membuka scope cabang if untuk kondisi `!isTier && !isFamily`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // WriteRulesetDefinitionAsync.
                {
                    // Memeriksa kebalikan kondisi `needAssetIds.TryGetValue(requirement.Value, out var resolvedAssetId)`; blok if hanya dijalankan ketika kondisi ini
                    // bernilai benar dalam WriteRulesetDefinitionAsync.
                    if (!needAssetIds.TryGetValue(requirement.Value, out var resolvedAssetId))
                    // Membuka scope cabang if untuk kondisi `!needAssetIds.TryGetValue(requirement.Value, out var resolvedAssetId)`; pernyataan/deklarasi berikut
                    // berada di dalam batas blok ini dalam WriteRulesetDefinitionAsync.
                    {
                        // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen ($”Need requirement '{requirement.Value}' tidak
                        // ditemukan dalam ruleset.”) dalam WriteRulesetDefinitionAsync; pemanggil atau middleware penanganan error menerima kegagalan ini.
                        throw new InvalidOperationException($"Need requirement '{requirement.Value}' tidak ditemukan dalam ruleset.");
                    // Menutup scope cabang if untuk kondisi `!needAssetIds.TryGetValue(requirement.Value, out var resolvedAssetId)`; bagian berikut berada di luar
                    // batas blok tersebut dalam WriteRulesetDefinitionAsync.
                    }

                    // Memperbarui `requiredAssetId` menggunakan `resolvedAssetId` (nilai hasil resolusi aset identitas) dalam WriteRulesetDefinitionAsync.
                    requiredAssetId = resolvedAssetId;
                // Menutup scope cabang if untuk kondisi `!isTier && !isFamily`; bagian berikut berada di luar batas blok tersebut dalam
                // WriteRulesetDefinitionAsync.
                }

                // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition( insertMissionRequirementSql, new {
                // RulesetCollectionMissionRequirementId = Guid.NewGuid(), RulesetVersionId = rulesetVersionId, RulesetCollectionMission...`; nilai hasil
                // menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam
                // WriteRulesetDefinitionAsync.
                await conn.ExecuteAsync(
                    // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen ( insertMissionRequirementSql, new { RulesetCollectionMissionRequirementId =
                    // Guid.NewGuid(), RulesetVersionId = rulesetVersionId, RulesetCollectionMissionId = m... sebagai argumen ke `conn.ExecuteAsync`.
                    new CommandDefinition(
                        // Meneruskan `insertMissionRequirementSql` (nilai insert misi requirement SQL) sebagai argumen ke konstruktor `CommandDefinition`.
                        insertMissionRequirementSql,
                        // Meneruskan objek anonim yang mengelompokkan RulesetCollectionMissionRequirementId, RulesetVersionId, RulesetCollectionMissionId,
                        // RequirementOrder, RequirementType, RequiredAssetId, RequiredNeedTier, RequiredNeedFamilyCode sebagai satu nilai sebagai argumen ke konstruktor
                        // `CommandDefinition`.
                        new
                        // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                        // WriteRulesetDefinitionAsync.
                        {
                            // Meneruskan objek anonim yang mengelompokkan RulesetCollectionMissionRequirementId, RulesetVersionId, RulesetCollectionMissionId,
                            // RequirementOrder, RequirementType, RequiredAssetId, RequiredNeedTier, RequiredNeedFamilyCode sebagai satu nilai sebagai argumen ke konstruktor
                            // `CommandDefinition`.
                            RulesetCollectionMissionRequirementId = Guid.NewGuid(),
                            // Meneruskan objek anonim yang mengelompokkan RulesetCollectionMissionRequirementId, RulesetVersionId, RulesetCollectionMissionId,
                            // RequirementOrder, RequirementType, RequiredAssetId, RequiredNeedTier, RequiredNeedFamilyCode sebagai satu nilai sebagai argumen ke konstruktor
                            // `CommandDefinition`.
                            RulesetVersionId = rulesetVersionId,
                            // Meneruskan objek anonim yang mengelompokkan RulesetCollectionMissionRequirementId, RulesetVersionId, RulesetCollectionMissionId,
                            // RequirementOrder, RequirementType, RequiredAssetId, RequiredNeedTier, RequiredNeedFamilyCode sebagai satu nilai sebagai argumen ke konstruktor
                            // `CommandDefinition`.
                            RulesetCollectionMissionId = missionId,
                            // Meneruskan objek anonim yang mengelompokkan RulesetCollectionMissionRequirementId, RulesetVersionId, RulesetCollectionMissionId,
                            // RequirementOrder, RequirementType, RequiredAssetId, RequiredNeedTier, RequiredNeedFamilyCode sebagai satu nilai sebagai argumen ke konstruktor
                            // `CommandDefinition`.
                            RequirementOrder = requirement.Order,
                            // Meneruskan objek anonim yang mengelompokkan RulesetCollectionMissionRequirementId, RulesetVersionId, RulesetCollectionMissionId,
                            // RequirementOrder, RequirementType, RequiredAssetId, RequiredNeedTier, RequiredNeedFamilyCode sebagai satu nilai sebagai argumen ke konstruktor
                            // `CommandDefinition`.
                            RequirementType = isTier ? "NEED_TIER" : isFamily ? "NEED_FAMILY" : "ASSET",
                            // Meneruskan objek anonim yang mengelompokkan RulesetCollectionMissionRequirementId, RulesetVersionId, RulesetCollectionMissionId,
                            // RequirementOrder, RequirementType, RequiredAssetId, RequiredNeedTier, RequiredNeedFamilyCode sebagai satu nilai sebagai argumen ke konstruktor
                            // `CommandDefinition`.
                            RequiredAssetId = requiredAssetId,
                            // Meneruskan objek anonim yang mengelompokkan RulesetCollectionMissionRequirementId, RulesetVersionId, RulesetCollectionMissionId,
                            // RequirementOrder, RequirementType, RequiredAssetId, RequiredNeedTier, RequiredNeedFamilyCode sebagai satu nilai sebagai argumen ke konstruktor
                            // `CommandDefinition`.
                            RequiredNeedTier = isTier ? requirement.Value.ToLowerInvariant() : null,
                            // Meneruskan objek anonim yang mengelompokkan RulesetCollectionMissionRequirementId, RulesetVersionId, RulesetCollectionMissionId,
                            // RequirementOrder, RequirementType, RequiredAssetId, RequiredNeedTier, RequiredNeedFamilyCode sebagai satu nilai sebagai argumen ke konstruktor
                            // `CommandDefinition`.
                            RequiredNeedFamilyCode = isFamily ? requirement.Value : null
                        // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
                        // WriteRulesetDefinitionAsync.
                        },
                        // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
                        tx,
                        // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                        // bernama `cancellationToken`.
                        cancellationToken: ct));
            // Menutup scope loop setiap requirement dari `missionEntry.Item.KebutuhanTarget`; bagian berikut berada di luar batas blok tersebut dalam
            // WriteRulesetDefinitionAsync.
            }
        // Menutup scope loop setiap missionEntry dari `definition.CollectionMissions.Select((item, index) => new { Item = item, Index = index })`; bagian
        // berikut berada di luar batas blok tersebut dalam WriteRulesetDefinitionAsync.
        }

        // Menyiapkan variabel lokal `insertFinancialGoalSql` untuk nilai insert keuangan target SQL dengan literal multiline yang dirinci pada komentar di
        // dekat deklarasinya. Tipe yang dipakai adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string
        // insertFinancialGoalSql = ”””`.
        // Baris literal 2: INSERT INTO menetapkan tabel dan kolom tujuan penambahan rekaman: `insert into ruleset_financial_goals (`.
        // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_financial_goal_id,`.
        // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_version_id,`.
        // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `goal_code,`.
        // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `item_name,`.
        // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `purchase_price,`.
        // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `happiness_points,`.
        // Baris literal 9: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `sort_order,`.
        // Baris literal 10: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `card_qty,`.
        // Baris literal 11: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `is_active,`.
        // Baris literal 12: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `payload_json,`.
        // Baris literal 13: CREATE mendefinisikan objek basis data yang diperlukan proses ini: `created_at`.
        // Baris literal 14: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 15: VALUES menyediakan nilai baris baru sesuai urutan kolom INSERT; placeholder @ diikat ke parameter perintah: `values (`.
        // Baris literal 16: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@RulesetFinancialGoalId,`.
        // Baris literal 17: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@RulesetVersionId,`.
        // Baris literal 18: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@GoalCode,`.
        // Baris literal 19: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@ItemName,`.
        // Baris literal 20: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@PurchasePrice,`.
        // Baris literal 21: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@HappinessPoints,`.
        // Baris literal 22: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@SortOrder,`.
        // Baris literal 23: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@CardQty,`.
        // Baris literal 24: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `true,`.
        // Baris literal 25: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `'{}',`.
        // Baris literal 26: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `now()`.
        // Baris literal 27: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 28: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string insertFinancialGoalSql = """
            insert into ruleset_financial_goals (
                ruleset_financial_goal_id,
                ruleset_version_id,
                goal_code,
                item_name,
                purchase_price,
                happiness_points,
                sort_order,
                card_qty,
                is_active,
                payload_json,
                created_at
            )
            values (
                @RulesetFinancialGoalId,
                @RulesetVersionId,
                @GoalCode,
                @ItemName,
                @PurchasePrice,
                @HappinessPoints,
                @SortOrder,
                @CardQty,
                true,
                '{}',
                now()
            )
            """;

        // Memulai loop dengan inisialisasi `var index = 0`, berjalan selama `index < definition.FinancialGoals.Count`, lalu memperbarui pencacah melalui
        // `index++` dalam WriteRulesetDefinitionAsync.
        for (var index = 0; index < definition.FinancialGoals.Count; index++)
        // Membuka scope loop dengan syarat `index < definition.FinancialGoals.Count`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // WriteRulesetDefinitionAsync.
        {
            // Menyiapkan variabel lokal `goal` untuk nilai target dengan `definition.FinancialGoals[index]`, yaitu elemen koleksi yang dipilih melalui indeks
            // atau kunci tersebut. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var goal = definition.FinancialGoals[index];
            // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition( insertFinancialGoalSql, new {
            // RulesetFinancialGoalId = Guid.NewGuid(), RulesetVersionId = rulesetVersionId, GoalCode = goal.Id, ItemName = goal.Nama, Pu...`; nilai hasil
            // menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam
            // WriteRulesetDefinitionAsync.
            await conn.ExecuteAsync(
                // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen ( insertFinancialGoalSql, new { RulesetFinancialGoalId = Guid.NewGuid(),
                // RulesetVersionId = rulesetVersionId, GoalCode = goal.Id, ItemName = goal.Nama, Purchase... sebagai argumen ke `conn.ExecuteAsync`.
                new CommandDefinition(
                    // Meneruskan `insertFinancialGoalSql` (nilai insert keuangan target SQL) sebagai argumen ke konstruktor `CommandDefinition`.
                    insertFinancialGoalSql,
                    // Meneruskan objek anonim yang mengelompokkan RulesetFinancialGoalId, RulesetVersionId, GoalCode, ItemName, PurchasePrice, HappinessPoints,
                    // SortOrder, CardQty sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                    new
                    // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                    // WriteRulesetDefinitionAsync.
                    {
                        // Meneruskan objek anonim yang mengelompokkan RulesetFinancialGoalId, RulesetVersionId, GoalCode, ItemName, PurchasePrice, HappinessPoints,
                        // SortOrder, CardQty sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        RulesetFinancialGoalId = Guid.NewGuid(),
                        // Meneruskan objek anonim yang mengelompokkan RulesetFinancialGoalId, RulesetVersionId, GoalCode, ItemName, PurchasePrice, HappinessPoints,
                        // SortOrder, CardQty sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        RulesetVersionId = rulesetVersionId,
                        // Meneruskan objek anonim yang mengelompokkan RulesetFinancialGoalId, RulesetVersionId, GoalCode, ItemName, PurchasePrice, HappinessPoints,
                        // SortOrder, CardQty sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        GoalCode = goal.Id,
                        // Meneruskan objek anonim yang mengelompokkan RulesetFinancialGoalId, RulesetVersionId, GoalCode, ItemName, PurchasePrice, HappinessPoints,
                        // SortOrder, CardQty sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        ItemName = goal.Nama,
                        // Meneruskan objek anonim yang mengelompokkan RulesetFinancialGoalId, RulesetVersionId, GoalCode, ItemName, PurchasePrice, HappinessPoints,
                        // SortOrder, CardQty sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        PurchasePrice = goal.HargaBeli,
                        // Meneruskan objek anonim yang mengelompokkan RulesetFinancialGoalId, RulesetVersionId, GoalCode, ItemName, PurchasePrice, HappinessPoints,
                        // SortOrder, CardQty sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        HappinessPoints = goal.PoinKebahagiaan,
                        // Meneruskan objek anonim yang mengelompokkan RulesetFinancialGoalId, RulesetVersionId, GoalCode, ItemName, PurchasePrice, HappinessPoints,
                        // SortOrder, CardQty sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        SortOrder = index + 1,
                        // Meneruskan objek anonim yang mengelompokkan RulesetFinancialGoalId, RulesetVersionId, GoalCode, ItemName, PurchasePrice, HappinessPoints,
                        // SortOrder, CardQty sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        CardQty = goal.CardQty ?? 1
                    // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
                    // WriteRulesetDefinitionAsync.
                    },
                    // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
                    tx,
                    // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                    // bernama `cancellationToken`.
                    cancellationToken: ct));
        // Menutup scope loop dengan syarat `index < definition.FinancialGoals.Count`; bagian berikut berada di luar batas blok tersebut dalam
        // WriteRulesetDefinitionAsync.
        }

        // Menyiapkan variabel lokal `insertNarrativeSql` untuk nilai insert narrative SQL dengan literal multiline yang dirinci pada komentar di dekat
        // deklarasinya. Tipe yang dipakai adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string
        // insertNarrativeSql = ”””`.
        // Baris literal 2: INSERT INTO menetapkan tabel dan kolom tujuan penambahan rekaman: `insert into ruleset_narratives (`.
        // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_narrative_id,`.
        // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_version_id,`.
        // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `narrative_code,`.
        // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `item_name,`.
        // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `sort_order,`.
        // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `repeatable,`.
        // Baris literal 9: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `cooldown_turns,`.
        // Baris literal 10: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `is_active,`.
        // Baris literal 11: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `payload_json,`.
        // Baris literal 12: CREATE mendefinisikan objek basis data yang diperlukan proses ini: `created_at`.
        // Baris literal 13: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 14: VALUES menyediakan nilai baris baru sesuai urutan kolom INSERT; placeholder @ diikat ke parameter perintah: `values (`.
        // Baris literal 15: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@RulesetNarrativeId,`.
        // Baris literal 16: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@RulesetVersionId,`.
        // Baris literal 17: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@NarrativeCode,`.
        // Baris literal 18: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@ItemName,`.
        // Baris literal 19: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@SortOrder,`.
        // Baris literal 20: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `false,`.
        // Baris literal 21: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `null,`.
        // Baris literal 22: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `true,`.
        // Baris literal 23: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `'{}',`.
        // Baris literal 24: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `now()`.
        // Baris literal 25: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 26: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string insertNarrativeSql = """
            insert into ruleset_narratives (
                ruleset_narrative_id,
                ruleset_version_id,
                narrative_code,
                item_name,
                sort_order,
                repeatable,
                cooldown_turns,
                is_active,
                payload_json,
                created_at
            )
            values (
                @RulesetNarrativeId,
                @RulesetVersionId,
                @NarrativeCode,
                @ItemName,
                @SortOrder,
                false,
                null,
                true,
                '{}',
                now()
            )
            """;

        // Menyiapkan variabel lokal `insertNarrativeSceneSql` untuk nilai insert narrative scene SQL dengan literal multiline yang dirinci pada komentar di
        // dekat deklarasinya. Tipe yang dipakai adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string
        // insertNarrativeSceneSql = ”””`.
        // Baris literal 2: INSERT INTO menetapkan tabel dan kolom tujuan penambahan rekaman: `insert into ruleset_narrative_scenes (`.
        // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_narrative_scene_id,`.
        // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_version_id,`.
        // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_narrative_id,`.
        // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `scene_code,`.
        // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `scene_order,`.
        // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `text_lines,`.
        // Baris literal 9: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `media_json,`.
        // Baris literal 10: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `payload_json,`.
        // Baris literal 11: CREATE mendefinisikan objek basis data yang diperlukan proses ini: `created_at`.
        // Baris literal 12: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 13: VALUES menyediakan nilai baris baru sesuai urutan kolom INSERT; placeholder @ diikat ke parameter perintah: `values (`.
        // Baris literal 14: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@RulesetNarrativeSceneId,`.
        // Baris literal 15: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@RulesetVersionId,`.
        // Baris literal 16: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@RulesetNarrativeId,`.
        // Baris literal 17: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@SceneCode,`.
        // Baris literal 18: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@SceneOrder,`.
        // Baris literal 19: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@TextLines::jsonb,`.
        // Baris literal 20: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `'{}',`.
        // Baris literal 21: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `'{}',`.
        // Baris literal 22: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `now()`.
        // Baris literal 23: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 24: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string insertNarrativeSceneSql = """
            insert into ruleset_narrative_scenes (
                ruleset_narrative_scene_id,
                ruleset_version_id,
                ruleset_narrative_id,
                scene_code,
                scene_order,
                text_lines,
                media_json,
                payload_json,
                created_at
            )
            values (
                @RulesetNarrativeSceneId,
                @RulesetVersionId,
                @RulesetNarrativeId,
                @SceneCode,
                @SceneOrder,
                @TextLines::jsonb,
                '{}',
                '{}',
                now()
            )
            """;

        // Menyiapkan variabel lokal `insertTriggerConditionSql` untuk nilai insert trigger condition SQL dengan literal multiline yang dirinci pada
        // komentar di dekat deklarasinya. Tipe yang dipakai adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string
        // insertTriggerConditionSql = ”””`.
        // Baris literal 2: INSERT INTO menetapkan tabel dan kolom tujuan penambahan rekaman: `insert into ruleset_trigger_conditions (`.
        // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun:
        // `ruleset_trigger_condition_id,`.
        // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_version_id,`.
        // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `trigger_owner_type,`.
        // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_narrative_id,`.
        // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_action_id,`.
        // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `reference_asset_id,`.
        // Baris literal 9: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `operator,`.
        // Baris literal 10: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `threshold_numeric,`.
        // Baris literal 11: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `sort_order,`.
        // Baris literal 12: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `condition_json,`.
        // Baris literal 13: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `is_active,`.
        // Baris literal 14: CREATE mendefinisikan objek basis data yang diperlukan proses ini: `created_at`.
        // Baris literal 15: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 16: VALUES menyediakan nilai baris baru sesuai urutan kolom INSERT; placeholder @ diikat ke parameter perintah: `values (`.
        // Baris literal 17: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@RulesetTriggerConditionId,`.
        // Baris literal 18: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@RulesetVersionId,`.
        // Baris literal 19: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `'NARRATIVE',`.
        // Baris literal 20: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@RulesetNarrativeId,`.
        // Baris literal 21: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `(`.
        // Baris literal 22: SELECT menentukan nilai atau kolom yang dikembalikan query: `select ra.ruleset_action_id`.
        // Baris literal 23: FROM memilih tabel/subquery sumber pembacaan: `from ruleset_actions ra`.
        // Baris literal 24: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where ra.ruleset_version_id = @RulesetVersionId`.
        // Baris literal 25: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and lower(ra.action_id) = lower(@ActionId)`.
        // Baris literal 26: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and ra.is_active`.
        // Baris literal 27: LIMIT membatasi jumlah baris yang dikembalikan query: `limit 1`.
        // Baris literal 28: Pembatas literal/penutup `),`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 29: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `null,`.
        // Baris literal 30: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `'COUNT_GTE',`.
        // Baris literal 31: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@ThresholdNumeric,`.
        // Baris literal 32: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@SortOrder,`.
        // Baris literal 33: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `'{}'::jsonb,`.
        // Baris literal 34: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `true,`.
        // Baris literal 35: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `now()`.
        // Baris literal 36: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 37: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string insertTriggerConditionSql = """
            insert into ruleset_trigger_conditions (
                ruleset_trigger_condition_id,
                ruleset_version_id,
                trigger_owner_type,
                ruleset_narrative_id,
                ruleset_action_id,
                reference_asset_id,
                operator,
                threshold_numeric,
                sort_order,
                condition_json,
                is_active,
                created_at
            )
            values (
                @RulesetTriggerConditionId,
                @RulesetVersionId,
                'NARRATIVE',
                @RulesetNarrativeId,
                (
                    select ra.ruleset_action_id
                    from ruleset_actions ra
                    where ra.ruleset_version_id = @RulesetVersionId
                      and lower(ra.action_id) = lower(@ActionId)
                      and ra.is_active
                    limit 1
                ),
                null,
                'COUNT_GTE',
                @ThresholdNumeric,
                @SortOrder,
                '{}'::jsonb,
                true,
                now()
            )
            """;

        // Mengulangi setiap elemen `definition.Narratives.Select((item, index) => new { Item = item, Index = index })`; elemen saat ini disimpan sebagai
        // `narrativeEntry` bertipe `var` untuk diproses oleh badan loop dalam WriteRulesetDefinitionAsync.
        foreach (var narrativeEntry in definition.Narratives.Select((item, index) => new { Item = item, Index = index }))
        // Membuka scope loop setiap narrativeEntry dari `definition.Narratives.Select((item, index) => new { Item = item, Index = index })`;
        // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam WriteRulesetDefinitionAsync.
        {
            // Menyiapkan variabel lokal `narrativeId` untuk nilai narrative identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel
            // disimpulkan dari ekspresi nilai awal.
            var narrativeId = Guid.NewGuid();
            // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition( insertNarrativeSql, new {
            // RulesetNarrativeId = narrativeId, RulesetVersionId = rulesetVersionId, NarrativeCode = narrativeEntry.Item.Id, ItemName = narr...`; nilai hasil
            // menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam
            // WriteRulesetDefinitionAsync.
            await conn.ExecuteAsync(
                // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen ( insertNarrativeSql, new { RulesetNarrativeId = narrativeId, RulesetVersionId =
                // rulesetVersionId, NarrativeCode = narrativeEntry.Item.Id, ItemName = narrativeE... sebagai argumen ke `conn.ExecuteAsync`.
                new CommandDefinition(
                    // Meneruskan `insertNarrativeSql` (nilai insert narrative SQL) sebagai argumen ke konstruktor `CommandDefinition`.
                    insertNarrativeSql,
                    // Meneruskan objek anonim yang mengelompokkan RulesetNarrativeId, RulesetVersionId, NarrativeCode, ItemName, SortOrder sebagai satu nilai sebagai
                    // argumen ke konstruktor `CommandDefinition`.
                    new
                    // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                    // WriteRulesetDefinitionAsync.
                    {
                        // Meneruskan objek anonim yang mengelompokkan RulesetNarrativeId, RulesetVersionId, NarrativeCode, ItemName, SortOrder sebagai satu nilai sebagai
                        // argumen ke konstruktor `CommandDefinition`.
                        RulesetNarrativeId = narrativeId,
                        // Meneruskan objek anonim yang mengelompokkan RulesetNarrativeId, RulesetVersionId, NarrativeCode, ItemName, SortOrder sebagai satu nilai sebagai
                        // argumen ke konstruktor `CommandDefinition`.
                        RulesetVersionId = rulesetVersionId,
                        // Meneruskan objek anonim yang mengelompokkan RulesetNarrativeId, RulesetVersionId, NarrativeCode, ItemName, SortOrder sebagai satu nilai sebagai
                        // argumen ke konstruktor `CommandDefinition`.
                        NarrativeCode = narrativeEntry.Item.Id,
                        // Meneruskan objek anonim yang mengelompokkan RulesetNarrativeId, RulesetVersionId, NarrativeCode, ItemName, SortOrder sebagai satu nilai sebagai
                        // argumen ke konstruktor `CommandDefinition`.
                        ItemName = narrativeEntry.Item.Nama,
                        // Meneruskan objek anonim yang mengelompokkan RulesetNarrativeId, RulesetVersionId, NarrativeCode, ItemName, SortOrder sebagai satu nilai sebagai
                        // argumen ke konstruktor `CommandDefinition`.
                        SortOrder = narrativeEntry.Index + 1
                    // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
                    // WriteRulesetDefinitionAsync.
                    },
                    // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
                    tx,
                    // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                    // bernama `cancellationToken`.
                    cancellationToken: ct));

            // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition( insertNarrativeSceneSql, new {
            // RulesetNarrativeSceneId = Guid.NewGuid(), RulesetVersionId = rulesetVersionId, RulesetNarrativeId = narrativeId, SceneCod...`; nilai hasil
            // menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam
            // WriteRulesetDefinitionAsync.
            await conn.ExecuteAsync(
                // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen ( insertNarrativeSceneSql, new { RulesetNarrativeSceneId = Guid.NewGuid(),
                // RulesetVersionId = rulesetVersionId, RulesetNarrativeId = narrativeId, SceneCode = $”... sebagai argumen ke `conn.ExecuteAsync`.
                new CommandDefinition(
                    // Meneruskan `insertNarrativeSceneSql` (nilai insert narrative scene SQL) sebagai argumen ke konstruktor `CommandDefinition`.
                    insertNarrativeSceneSql,
                    // Meneruskan objek anonim yang mengelompokkan RulesetNarrativeSceneId, RulesetVersionId, RulesetNarrativeId, SceneCode, SceneOrder, TextLines
                    // sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                    new
                    // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                    // WriteRulesetDefinitionAsync.
                    {
                        // Meneruskan objek anonim yang mengelompokkan RulesetNarrativeSceneId, RulesetVersionId, RulesetNarrativeId, SceneCode, SceneOrder, TextLines
                        // sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        RulesetNarrativeSceneId = Guid.NewGuid(),
                        // Meneruskan objek anonim yang mengelompokkan RulesetNarrativeSceneId, RulesetVersionId, RulesetNarrativeId, SceneCode, SceneOrder, TextLines
                        // sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        RulesetVersionId = rulesetVersionId,
                        // Meneruskan objek anonim yang mengelompokkan RulesetNarrativeSceneId, RulesetVersionId, RulesetNarrativeId, SceneCode, SceneOrder, TextLines
                        // sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        RulesetNarrativeId = narrativeId,
                        // Meneruskan objek anonim yang mengelompokkan RulesetNarrativeSceneId, RulesetVersionId, RulesetNarrativeId, SceneCode, SceneOrder, TextLines
                        // sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        SceneCode = $"{narrativeEntry.Item.Id}_scene_1",
                        // Meneruskan objek anonim yang mengelompokkan RulesetNarrativeSceneId, RulesetVersionId, RulesetNarrativeId, SceneCode, SceneOrder, TextLines
                        // sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        SceneOrder = 1,
                        // Meneruskan `narrativeEntry.Item.Teks` (nilai teks) sebagai argumen ke `JsonSerializer.Serialize`.
                        TextLines = JsonSerializer.Serialize(narrativeEntry.Item.Teks)
                    // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
                    // WriteRulesetDefinitionAsync.
                    },
                    // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
                    tx,
                    // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                    // bernama `cancellationToken`.
                    cancellationToken: ct));

            // Mengulangi setiap elemen `narrativeEntry.Item.PrerequisiteAksi.Select((item, prerequisiteIndex) => new { Item = item, Index = prerequisiteIndex
            // })`; elemen saat ini disimpan sebagai `prerequisite` bertipe `var` untuk diproses oleh badan loop dalam WriteRulesetDefinitionAsync.
            foreach (var prerequisite in narrativeEntry.Item.PrerequisiteAksi.Select((item, prerequisiteIndex) => new { Item = item, Index = prerequisiteIndex }))
            // Membuka scope loop setiap prerequisite dari `narrativeEntry.Item.PrerequisiteAksi.Select((item, prerequisiteIndex) => new { Item = item, Index =
            // prerequisiteIndex })`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam WriteRulesetDefinitionAsync.
            {
                // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition( insertTriggerConditionSql, new {
                // RulesetTriggerConditionId = Guid.NewGuid(), RulesetVersionId = rulesetVersionId, RulesetNarrativeId = narrativeId, Acti...`; nilai hasil
                // menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam
                // WriteRulesetDefinitionAsync.
                await conn.ExecuteAsync(
                    // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen ( insertTriggerConditionSql, new { RulesetTriggerConditionId = Guid.NewGuid(),
                    // RulesetVersionId = rulesetVersionId, RulesetNarrativeId = narrativeId, ActionId =... sebagai argumen ke `conn.ExecuteAsync`.
                    new CommandDefinition(
                        // Meneruskan `insertTriggerConditionSql` (nilai insert trigger condition SQL) sebagai argumen ke konstruktor `CommandDefinition`.
                        insertTriggerConditionSql,
                        // Meneruskan objek anonim yang mengelompokkan RulesetTriggerConditionId, RulesetVersionId, RulesetNarrativeId, ActionId, ThresholdNumeric,
                        // SortOrder sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        new
                        // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                        // WriteRulesetDefinitionAsync.
                        {
                            // Meneruskan objek anonim yang mengelompokkan RulesetTriggerConditionId, RulesetVersionId, RulesetNarrativeId, ActionId, ThresholdNumeric,
                            // SortOrder sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                            RulesetTriggerConditionId = Guid.NewGuid(),
                            // Meneruskan objek anonim yang mengelompokkan RulesetTriggerConditionId, RulesetVersionId, RulesetNarrativeId, ActionId, ThresholdNumeric,
                            // SortOrder sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                            RulesetVersionId = rulesetVersionId,
                            // Meneruskan objek anonim yang mengelompokkan RulesetTriggerConditionId, RulesetVersionId, RulesetNarrativeId, ActionId, ThresholdNumeric,
                            // SortOrder sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                            RulesetNarrativeId = narrativeId,
                            // Meneruskan objek anonim yang mengelompokkan RulesetTriggerConditionId, RulesetVersionId, RulesetNarrativeId, ActionId, ThresholdNumeric,
                            // SortOrder sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                            ActionId = prerequisite.Item.Aksi,
                            // Meneruskan nilai literal `1` sebagai argumen ke `Math.Max`; Meneruskan `prerequisite.Item.Value`, yaitu nilai yang dibungkus objek/nullable
                            // sebagai argumen ke `Math.Max`.
                            ThresholdNumeric = Math.Max(1, prerequisite.Item.Value),
                            // Meneruskan objek anonim yang mengelompokkan RulesetTriggerConditionId, RulesetVersionId, RulesetNarrativeId, ActionId, ThresholdNumeric,
                            // SortOrder sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                            SortOrder = prerequisite.Index + 1
                        // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
                        // WriteRulesetDefinitionAsync.
                        },
                        // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
                        tx,
                        // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                        // bernama `cancellationToken`.
                        cancellationToken: ct));
            // Menutup scope loop setiap prerequisite dari `narrativeEntry.Item.PrerequisiteAksi.Select((item, prerequisiteIndex) => new { Item = item, Index =
            // prerequisiteIndex })`; bagian berikut berada di luar batas blok tersebut dalam WriteRulesetDefinitionAsync.
            }
        // Menutup scope loop setiap narrativeEntry dari `definition.Narratives.Select((item, index) => new { Item = item, Index = index })`; bagian berikut
        // berada di luar batas blok tersebut dalam WriteRulesetDefinitionAsync.
        }

        // Menyiapkan variabel lokal `insertRankPointSql` untuk nilai insert rank point SQL dengan literal multiline yang dirinci pada komentar di dekat
        // deklarasinya. Tipe yang dipakai adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string
        // insertRankPointSql = ”””`.
        // Baris literal 2: INSERT INTO menetapkan tabel dan kolom tujuan penambahan rekaman: `insert into ruleset_rank_points (`.
        // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_rank_point_id,`.
        // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_version_id,`.
        // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `rank_type,`.
        // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `rank_no,`.
        // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `points,`.
        // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `sort_order,`.
        // Baris literal 9: CREATE mendefinisikan objek basis data yang diperlukan proses ini: `created_at`.
        // Baris literal 10: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 11: VALUES menyediakan nilai baris baru sesuai urutan kolom INSERT; placeholder @ diikat ke parameter perintah: `values (`.
        // Baris literal 12: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@RulesetRankPointId,`.
        // Baris literal 13: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@RulesetVersionId,`.
        // Baris literal 14: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@RankType,`.
        // Baris literal 15: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@RankNo,`.
        // Baris literal 16: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@Points,`.
        // Baris literal 17: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@SortOrder,`.
        // Baris literal 18: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `now()`.
        // Baris literal 19: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 20: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string insertRankPointSql = """
            insert into ruleset_rank_points (
                ruleset_rank_point_id,
                ruleset_version_id,
                rank_type,
                rank_no,
                points,
                sort_order,
                created_at
            )
            values (
                @RulesetRankPointId,
                @RulesetVersionId,
                @RankType,
                @RankNo,
                @Points,
                @SortOrder,
                now()
            )
            """;

        // Memulai loop dengan inisialisasi `var index = 0`, berjalan selama `index < definition.DonationRankPoints.Count`, lalu memperbarui pencacah
        // melalui `index++` dalam WriteRulesetDefinitionAsync.
        for (var index = 0; index < definition.DonationRankPoints.Count; index++)
        // Membuka scope loop dengan syarat `index < definition.DonationRankPoints.Count`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // WriteRulesetDefinitionAsync.
        {
            // Menyiapkan variabel lokal `point` untuk nilai point dengan `definition.DonationRankPoints[index]`, yaitu elemen koleksi yang dipilih melalui
            // indeks atau kunci tersebut. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var point = definition.DonationRankPoints[index];
            // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition( insertRankPointSql, new {
            // RulesetRankPointId = Guid.NewGuid(), RulesetVersionId = rulesetVersionId, RankType = ”DONATION”, RankNo = point.Rank, point.Po...`; nilai hasil
            // menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam
            // WriteRulesetDefinitionAsync.
            await conn.ExecuteAsync(
                    // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen ( insertRankPointSql, new { RulesetRankPointId = Guid.NewGuid(),
                    // RulesetVersionId = rulesetVersionId, RankType = ”DONATION”, RankNo = point.Rank, point.Points, ... sebagai argumen ke `conn.ExecuteAsync`.
                    new CommandDefinition(
                    // Meneruskan `insertRankPointSql` (nilai insert rank point SQL) sebagai argumen ke konstruktor `CommandDefinition`.
                    insertRankPointSql,
                    // Meneruskan objek anonim yang mengelompokkan RulesetRankPointId, RulesetVersionId, RankType, RankNo, point.Points, SortOrder sebagai satu nilai
                    // sebagai argumen ke konstruktor `CommandDefinition`.
                    new
                    // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                    // WriteRulesetDefinitionAsync.
                    {
                        // Meneruskan objek anonim yang mengelompokkan RulesetRankPointId, RulesetVersionId, RankType, RankNo, point.Points, SortOrder sebagai satu nilai
                        // sebagai argumen ke konstruktor `CommandDefinition`.
                        RulesetRankPointId = Guid.NewGuid(),
                        // Meneruskan objek anonim yang mengelompokkan RulesetRankPointId, RulesetVersionId, RankType, RankNo, point.Points, SortOrder sebagai satu nilai
                        // sebagai argumen ke konstruktor `CommandDefinition`.
                        RulesetVersionId = rulesetVersionId,
                        // Meneruskan objek anonim yang mengelompokkan RulesetRankPointId, RulesetVersionId, RankType, RankNo, point.Points, SortOrder sebagai satu nilai
                        // sebagai argumen ke konstruktor `CommandDefinition`.
                        RankType = "DONATION",
                        // Meneruskan objek anonim yang mengelompokkan RulesetRankPointId, RulesetVersionId, RankType, RankNo, point.Points, SortOrder sebagai satu nilai
                        // sebagai argumen ke konstruktor `CommandDefinition`.
                        RankNo = point.Rank,
                        // Meneruskan objek anonim yang mengelompokkan RulesetRankPointId, RulesetVersionId, RankType, RankNo, point.Points, SortOrder sebagai satu nilai
                        // sebagai argumen ke konstruktor `CommandDefinition`.
                        point.Points,
                        // Meneruskan objek anonim yang mengelompokkan RulesetRankPointId, RulesetVersionId, RankType, RankNo, point.Points, SortOrder sebagai satu nilai
                        // sebagai argumen ke konstruktor `CommandDefinition`.
                        SortOrder = index + 1
                    // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
                    // WriteRulesetDefinitionAsync.
                    },
                    // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
                    tx,
                    // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                    // bernama `cancellationToken`.
                    cancellationToken: ct));
        // Menutup scope loop dengan syarat `index < definition.DonationRankPoints.Count`; bagian berikut berada di luar batas blok tersebut dalam
        // WriteRulesetDefinitionAsync.
        }

        // Menyiapkan variabel lokal `insertGoldPointSql` untuk nilai insert emas point SQL dengan literal multiline yang dirinci pada komentar di dekat
        // deklarasinya. Tipe yang dipakai adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string
        // insertGoldPointSql = ”””`.
        // Baris literal 2: INSERT INTO menetapkan tabel dan kolom tujuan penambahan rekaman: `insert into ruleset_gold_assets (`.
        // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_gold_asset_id,`.
        // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_version_id,`.
        // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_game_asset_id,`.
        // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `asset_code,`.
        // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `quantity,`.
        // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `points,`.
        // Baris literal 9: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `sort_order,`.
        // Baris literal 10: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `card_qty,`.
        // Baris literal 11: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `is_active,`.
        // Baris literal 12: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `payload_json,`.
        // Baris literal 13: CREATE mendefinisikan objek basis data yang diperlukan proses ini: `created_at`.
        // Baris literal 14: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 15: VALUES menyediakan nilai baris baru sesuai urutan kolom INSERT; placeholder @ diikat ke parameter perintah: `values (`.
        // Baris literal 16: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@RulesetGoldAssetId,`.
        // Baris literal 17: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@RulesetVersionId,`.
        // Baris literal 18: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@RulesetGameAssetId,`.
        // Baris literal 19: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@AssetCode,`.
        // Baris literal 20: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@Quantity,`.
        // Baris literal 21: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@Points,`.
        // Baris literal 22: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@SortOrder,`.
        // Baris literal 23: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `null,`.
        // Baris literal 24: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `true,`.
        // Baris literal 25: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `'{}',`.
        // Baris literal 26: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `now()`.
        // Baris literal 27: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 28: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string insertGoldPointSql = """
            insert into ruleset_gold_assets (
                ruleset_gold_asset_id,
                ruleset_version_id,
                ruleset_game_asset_id,
                asset_code,
                quantity,
                points,
                sort_order,
                card_qty,
                is_active,
                payload_json,
                created_at
            )
            values (
                @RulesetGoldAssetId,
                @RulesetVersionId,
                @RulesetGameAssetId,
                @AssetCode,
                @Quantity,
                @Points,
                @SortOrder,
                null,
                true,
                '{}',
                now()
            )
            """;

        // Menyiapkan variabel lokal `goldAssetId` untuk nilai emas aset identitas dengan null, yaitu penanda tidak ada nilai. Tipe yang dipakai adalah
        // `Guid?`.
        Guid? goldAssetId = null;
        // Memeriksa pemeriksaan lebih besar antara `definition.GoldPointsByQty.Count` dan `0`; blok if hanya dijalankan ketika kondisi ini bernilai benar
        // dalam WriteRulesetDefinitionAsync.
        if (definition.GoldPointsByQty.Count > 0)
        // Membuka scope cabang if untuk kondisi `definition.GoldPointsByQty.Count > 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // WriteRulesetDefinitionAsync.
        {
            // Memperbarui `goldAssetId` menggunakan hasil operasi asinkron memanggil `InsertGameAssetAsync` dengan `conn`, `tx`, `rulesetVersionId`, `”GOLD”`,
            // `”gold_card”`, `”Emas”`, `1`, `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam WriteRulesetDefinitionAsync.
            goldAssetId = await InsertGameAssetAsync(
                // Meneruskan `conn` (koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data) sebagai argumen ke `InsertGameAssetAsync`.
                conn,
                // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke `InsertGameAssetAsync`.
                tx,
                // Meneruskan `rulesetVersionId` (identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang tepat) sebagai argumen ke
                // `InsertGameAssetAsync`.
                rulesetVersionId,
                // Meneruskan nilai literal `”GOLD”` sebagai argumen ke `InsertGameAssetAsync`.
                "GOLD",
                // Meneruskan nilai literal `”gold_card”` sebagai argumen ke `InsertGameAssetAsync`.
                "gold_card",
                // Meneruskan nilai literal `”Emas”` sebagai argumen ke `InsertGameAssetAsync`.
                "Emas",
                // Meneruskan nilai literal `1` sebagai argumen ke `InsertGameAssetAsync`.
                1,
                // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                // ke `InsertGameAssetAsync`.
                ct);
        // Menutup scope cabang if untuk kondisi `definition.GoldPointsByQty.Count > 0`; bagian berikut berada di luar batas blok tersebut dalam
        // WriteRulesetDefinitionAsync.
        }

        // Memulai loop dengan inisialisasi `var index = 0`, berjalan selama `index < definition.GoldPointsByQty.Count`, lalu memperbarui pencacah melalui
        // `index++` dalam WriteRulesetDefinitionAsync.
        for (var index = 0; index < definition.GoldPointsByQty.Count; index++)
        // Membuka scope loop dengan syarat `index < definition.GoldPointsByQty.Count`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // WriteRulesetDefinitionAsync.
        {
            // Menyiapkan variabel lokal `point` untuk nilai point dengan `definition.GoldPointsByQty[index]`, yaitu elemen koleksi yang dipilih melalui indeks
            // atau kunci tersebut. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var point = definition.GoldPointsByQty[index];
            // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition( insertGoldPointSql, new {
            // RulesetGoldAssetId = Guid.NewGuid(), RulesetVersionId = rulesetVersionId, RulesetGameAssetId = goldAssetId!.Value, AssetCode =...`; nilai hasil
            // menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam
            // WriteRulesetDefinitionAsync.
            await conn.ExecuteAsync(
                // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen ( insertGoldPointSql, new { RulesetGoldAssetId = Guid.NewGuid(),
                // RulesetVersionId = rulesetVersionId, RulesetGameAssetId = goldAssetId!.Value, AssetCode = ”gold... sebagai argumen ke `conn.ExecuteAsync`.
                new CommandDefinition(
                    // Meneruskan `insertGoldPointSql` (nilai insert emas point SQL) sebagai argumen ke konstruktor `CommandDefinition`.
                    insertGoldPointSql,
                    // Meneruskan objek anonim yang mengelompokkan RulesetGoldAssetId, RulesetVersionId, RulesetGameAssetId, AssetCode, Quantity, point.Points,
                    // SortOrder sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                    new
                    // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                    // WriteRulesetDefinitionAsync.
                    {
                        // Meneruskan objek anonim yang mengelompokkan RulesetGoldAssetId, RulesetVersionId, RulesetGameAssetId, AssetCode, Quantity, point.Points,
                        // SortOrder sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        RulesetGoldAssetId = Guid.NewGuid(),
                        // Meneruskan objek anonim yang mengelompokkan RulesetGoldAssetId, RulesetVersionId, RulesetGameAssetId, AssetCode, Quantity, point.Points,
                        // SortOrder sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        RulesetVersionId = rulesetVersionId,
                        // Meneruskan objek anonim yang mengelompokkan RulesetGoldAssetId, RulesetVersionId, RulesetGameAssetId, AssetCode, Quantity, point.Points,
                        // SortOrder sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        RulesetGameAssetId = goldAssetId!.Value,
                        // Meneruskan objek anonim yang mengelompokkan RulesetGoldAssetId, RulesetVersionId, RulesetGameAssetId, AssetCode, Quantity, point.Points,
                        // SortOrder sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        AssetCode = "gold_card",
                        // Meneruskan objek anonim yang mengelompokkan RulesetGoldAssetId, RulesetVersionId, RulesetGameAssetId, AssetCode, Quantity, point.Points,
                        // SortOrder sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        Quantity = point.Qty,
                        // Meneruskan objek anonim yang mengelompokkan RulesetGoldAssetId, RulesetVersionId, RulesetGameAssetId, AssetCode, Quantity, point.Points,
                        // SortOrder sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        point.Points,
                        // Meneruskan objek anonim yang mengelompokkan RulesetGoldAssetId, RulesetVersionId, RulesetGameAssetId, AssetCode, Quantity, point.Points,
                        // SortOrder sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        SortOrder = index + 1
                    // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
                    // WriteRulesetDefinitionAsync.
                    },
                    // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
                    tx,
                    // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                    // bernama `cancellationToken`.
                    cancellationToken: ct));
        // Menutup scope loop dengan syarat `index < definition.GoldPointsByQty.Count`; bagian berikut berada di luar batas blok tersebut dalam
        // WriteRulesetDefinitionAsync.
        }

        // Memulai loop dengan inisialisasi `var index = 0`, berjalan selama `index < definition.PensionRankPoints.Count`, lalu memperbarui pencacah melalui
        // `index++` dalam WriteRulesetDefinitionAsync.
        for (var index = 0; index < definition.PensionRankPoints.Count; index++)
        // Membuka scope loop dengan syarat `index < definition.PensionRankPoints.Count`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // WriteRulesetDefinitionAsync.
        {
            // Menyiapkan variabel lokal `point` untuk nilai point dengan `definition.PensionRankPoints[index]`, yaitu elemen koleksi yang dipilih melalui
            // indeks atau kunci tersebut. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var point = definition.PensionRankPoints[index];
            // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition( insertRankPointSql, new {
            // RulesetRankPointId = Guid.NewGuid(), RulesetVersionId = rulesetVersionId, RankType = ”PENSION”, RankNo = point.Rank, point.Poi...`; nilai hasil
            // menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam
            // WriteRulesetDefinitionAsync.
            await conn.ExecuteAsync(
                // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen ( insertRankPointSql, new { RulesetRankPointId = Guid.NewGuid(),
                // RulesetVersionId = rulesetVersionId, RankType = ”PENSION”, RankNo = point.Rank, point.Points, S... sebagai argumen ke `conn.ExecuteAsync`.
                new CommandDefinition(
                    // Meneruskan `insertRankPointSql` (nilai insert rank point SQL) sebagai argumen ke konstruktor `CommandDefinition`.
                    insertRankPointSql,
                    // Meneruskan objek anonim yang mengelompokkan RulesetRankPointId, RulesetVersionId, RankType, RankNo, point.Points, SortOrder sebagai satu nilai
                    // sebagai argumen ke konstruktor `CommandDefinition`.
                    new
                    // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                    // WriteRulesetDefinitionAsync.
                    {
                        // Meneruskan objek anonim yang mengelompokkan RulesetRankPointId, RulesetVersionId, RankType, RankNo, point.Points, SortOrder sebagai satu nilai
                        // sebagai argumen ke konstruktor `CommandDefinition`.
                        RulesetRankPointId = Guid.NewGuid(),
                        // Meneruskan objek anonim yang mengelompokkan RulesetRankPointId, RulesetVersionId, RankType, RankNo, point.Points, SortOrder sebagai satu nilai
                        // sebagai argumen ke konstruktor `CommandDefinition`.
                        RulesetVersionId = rulesetVersionId,
                        // Meneruskan objek anonim yang mengelompokkan RulesetRankPointId, RulesetVersionId, RankType, RankNo, point.Points, SortOrder sebagai satu nilai
                        // sebagai argumen ke konstruktor `CommandDefinition`.
                        RankType = "PENSION",
                        // Meneruskan objek anonim yang mengelompokkan RulesetRankPointId, RulesetVersionId, RankType, RankNo, point.Points, SortOrder sebagai satu nilai
                        // sebagai argumen ke konstruktor `CommandDefinition`.
                        RankNo = point.Rank,
                        // Meneruskan objek anonim yang mengelompokkan RulesetRankPointId, RulesetVersionId, RankType, RankNo, point.Points, SortOrder sebagai satu nilai
                        // sebagai argumen ke konstruktor `CommandDefinition`.
                        point.Points,
                        // Meneruskan objek anonim yang mengelompokkan RulesetRankPointId, RulesetVersionId, RankType, RankNo, point.Points, SortOrder sebagai satu nilai
                        // sebagai argumen ke konstruktor `CommandDefinition`.
                        SortOrder = index + 1
                    // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
                    // WriteRulesetDefinitionAsync.
                    },
                    // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
                    tx,
                    // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                    // bernama `cancellationToken`.
                    cancellationToken: ct));
        // Menutup scope loop dengan syarat `index < definition.PensionRankPoints.Count`; bagian berikut berada di luar batas blok tersebut dalam
        // WriteRulesetDefinitionAsync.
        }

        // Menyiapkan variabel lokal `insertGoldPriceSql` untuk nilai insert emas harga SQL dengan literal multiline yang dirinci pada komentar di dekat
        // deklarasinya. Tipe yang dipakai adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string
        // insertGoldPriceSql = ”””`.
        // Baris literal 2: INSERT INTO menetapkan tabel dan kolom tujuan penambahan rekaman: `insert into ruleset_gold_prices (`.
        // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_gold_price_id,`.
        // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_version_id,`.
        // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_game_asset_id,`.
        // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `price_code,`.
        // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `quantity,`.
        // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `unit_price,`.
        // Baris literal 9: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `sort_order,`.
        // Baris literal 10: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `card_qty,`.
        // Baris literal 11: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `is_active,`.
        // Baris literal 12: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `payload_json,`.
        // Baris literal 13: CREATE mendefinisikan objek basis data yang diperlukan proses ini: `created_at`.
        // Baris literal 14: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 15: VALUES menyediakan nilai baris baru sesuai urutan kolom INSERT; placeholder @ diikat ke parameter perintah: `values (`.
        // Baris literal 16: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@RulesetGoldPriceId,`.
        // Baris literal 17: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@RulesetVersionId,`.
        // Baris literal 18: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@RulesetGameAssetId,`.
        // Baris literal 19: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@PriceCode,`.
        // Baris literal 20: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@Quantity,`.
        // Baris literal 21: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@UnitPrice,`.
        // Baris literal 22: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@SortOrder,`.
        // Baris literal 23: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@CardQty,`.
        // Baris literal 24: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `true,`.
        // Baris literal 25: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `'{}',`.
        // Baris literal 26: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `now()`.
        // Baris literal 27: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 28: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string insertGoldPriceSql = """
            insert into ruleset_gold_prices (
                ruleset_gold_price_id,
                ruleset_version_id,
                ruleset_game_asset_id,
                price_code,
                quantity,
                unit_price,
                sort_order,
                card_qty,
                is_active,
                payload_json,
                created_at
            )
            values (
                @RulesetGoldPriceId,
                @RulesetVersionId,
                @RulesetGameAssetId,
                @PriceCode,
                @Quantity,
                @UnitPrice,
                @SortOrder,
                @CardQty,
                true,
                '{}',
                now()
            )
            """;

        // Memulai loop dengan inisialisasi `var index = 0`, berjalan selama `index < definition.GoldPrices.Count`, lalu memperbarui pencacah melalui
        // `index++` dalam WriteRulesetDefinitionAsync.
        for (var index = 0; index < definition.GoldPrices.Count; index++)
        // Membuka scope loop dengan syarat `index < definition.GoldPrices.Count`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // WriteRulesetDefinitionAsync.
        {
            // Menyiapkan variabel lokal `price` untuk nilai harga dengan `definition.GoldPrices[index]`, yaitu elemen koleksi yang dipilih melalui indeks atau
            // kunci tersebut. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var price = definition.GoldPrices[index];
            // Menyiapkan variabel lokal `assetId` untuk nilai aset identitas dengan hasil operasi asinkron memanggil `InsertGameAssetAsync` dengan `conn`,
            // `tx`, `rulesetVersionId`, `”GOLD_PRICE”`, `price.PriceCode`, `$”Harga Emas {price.Qty}”`, `index + 1`, `ct`; await menunggu hasil tanpa memblokir
            // thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var assetId = await InsertGameAssetAsync(
                // Meneruskan `conn` (koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data) sebagai argumen ke `InsertGameAssetAsync`.
                conn,
                // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke `InsertGameAssetAsync`.
                tx,
                // Meneruskan `rulesetVersionId` (identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang tepat) sebagai argumen ke
                // `InsertGameAssetAsync`.
                rulesetVersionId,
                // Meneruskan nilai literal `”GOLD_PRICE”` sebagai argumen ke `InsertGameAssetAsync`.
                "GOLD_PRICE",
                // Meneruskan `price.PriceCode` (nilai harga kode) sebagai argumen ke `InsertGameAssetAsync`.
                price.PriceCode,
                // Meneruskan teks interpolasi `$”Harga Emas {price.Qty}”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan sebagai argumen
                // ke `InsertGameAssetAsync`.
                $"Harga Emas {price.Qty}",
                // Meneruskan penjumlahan/penggabungan antara `index` dan `1` sebagai argumen ke `InsertGameAssetAsync`.
                index + 1,
                // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                // ke `InsertGameAssetAsync`.
                ct);
            // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition( insertGoldPriceSql, new {
            // RulesetGoldPriceId = Guid.NewGuid(), RulesetVersionId = rulesetVersionId, RulesetGameAssetId = assetId, price.PriceCode, Quant...`; nilai hasil
            // menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam
            // WriteRulesetDefinitionAsync.
            await conn.ExecuteAsync(
                // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen ( insertGoldPriceSql, new { RulesetGoldPriceId = Guid.NewGuid(),
                // RulesetVersionId = rulesetVersionId, RulesetGameAssetId = assetId, price.PriceCode, Quantity = ... sebagai argumen ke `conn.ExecuteAsync`.
                new CommandDefinition(
                    // Meneruskan `insertGoldPriceSql` (nilai insert emas harga SQL) sebagai argumen ke konstruktor `CommandDefinition`.
                    insertGoldPriceSql,
                    // Meneruskan objek anonim yang mengelompokkan RulesetGoldPriceId, RulesetVersionId, RulesetGameAssetId, price.PriceCode, Quantity, price.UnitPrice,
                    // SortOrder, price.CardQty sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                    new
                    // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                    // WriteRulesetDefinitionAsync.
                    {
                        // Meneruskan objek anonim yang mengelompokkan RulesetGoldPriceId, RulesetVersionId, RulesetGameAssetId, price.PriceCode, Quantity, price.UnitPrice,
                        // SortOrder, price.CardQty sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        RulesetGoldPriceId = Guid.NewGuid(),
                        // Meneruskan objek anonim yang mengelompokkan RulesetGoldPriceId, RulesetVersionId, RulesetGameAssetId, price.PriceCode, Quantity, price.UnitPrice,
                        // SortOrder, price.CardQty sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        RulesetVersionId = rulesetVersionId,
                        // Meneruskan objek anonim yang mengelompokkan RulesetGoldPriceId, RulesetVersionId, RulesetGameAssetId, price.PriceCode, Quantity, price.UnitPrice,
                        // SortOrder, price.CardQty sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        RulesetGameAssetId = assetId,
                        // Meneruskan objek anonim yang mengelompokkan RulesetGoldPriceId, RulesetVersionId, RulesetGameAssetId, price.PriceCode, Quantity, price.UnitPrice,
                        // SortOrder, price.CardQty sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        price.PriceCode,
                        // Meneruskan objek anonim yang mengelompokkan RulesetGoldPriceId, RulesetVersionId, RulesetGameAssetId, price.PriceCode, Quantity, price.UnitPrice,
                        // SortOrder, price.CardQty sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        Quantity = price.Qty,
                        // Meneruskan objek anonim yang mengelompokkan RulesetGoldPriceId, RulesetVersionId, RulesetGameAssetId, price.PriceCode, Quantity, price.UnitPrice,
                        // SortOrder, price.CardQty sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        price.UnitPrice,
                        // Meneruskan objek anonim yang mengelompokkan RulesetGoldPriceId, RulesetVersionId, RulesetGameAssetId, price.PriceCode, Quantity, price.UnitPrice,
                        // SortOrder, price.CardQty sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        SortOrder = index + 1,
                        // Meneruskan objek anonim yang mengelompokkan RulesetGoldPriceId, RulesetVersionId, RulesetGameAssetId, price.PriceCode, Quantity, price.UnitPrice,
                        // SortOrder, price.CardQty sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        price.CardQty
                    // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
                    // WriteRulesetDefinitionAsync.
                    },
                    // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
                    tx,
                    // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                    // bernama `cancellationToken`.
                    cancellationToken: ct));
        // Menutup scope loop dengan syarat `index < definition.GoldPrices.Count`; bagian berikut berada di luar batas blok tersebut dalam
        // WriteRulesetDefinitionAsync.
        }

        // Menyiapkan variabel lokal `insertTieBreakerSql` untuk nilai insert tie breaker SQL dengan literal multiline yang dirinci pada komentar di dekat
        // deklarasinya. Tipe yang dipakai adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string
        // insertTieBreakerSql = ”””`.
        // Baris literal 2: INSERT INTO menetapkan tabel dan kolom tujuan penambahan rekaman: `insert into ruleset_tie_breakers (`.
        // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_tie_breaker_id,`.
        // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_version_id,`.
        // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_game_asset_id,`.
        // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `tie_breaker_code,`.
        // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `tie_number,`.
        // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `sort_order,`.
        // Baris literal 9: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `card_qty,`.
        // Baris literal 10: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `payload_json,`.
        // Baris literal 11: CREATE mendefinisikan objek basis data yang diperlukan proses ini: `created_at`.
        // Baris literal 12: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 13: VALUES menyediakan nilai baris baru sesuai urutan kolom INSERT; placeholder @ diikat ke parameter perintah: `values (`.
        // Baris literal 14: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@RulesetTieBreakerId,`.
        // Baris literal 15: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@RulesetVersionId,`.
        // Baris literal 16: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@RulesetGameAssetId,`.
        // Baris literal 17: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@TieBreakerCode,`.
        // Baris literal 18: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@TieNumber,`.
        // Baris literal 19: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@SortOrder,`.
        // Baris literal 20: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@CardQty,`.
        // Baris literal 21: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `'{}',`.
        // Baris literal 22: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `now()`.
        // Baris literal 23: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 24: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string insertTieBreakerSql = """
            insert into ruleset_tie_breakers (
                ruleset_tie_breaker_id,
                ruleset_version_id,
                ruleset_game_asset_id,
                tie_breaker_code,
                tie_number,
                sort_order,
                card_qty,
                payload_json,
                created_at
            )
            values (
                @RulesetTieBreakerId,
                @RulesetVersionId,
                @RulesetGameAssetId,
                @TieBreakerCode,
                @TieNumber,
                @SortOrder,
                @CardQty,
                '{}',
                now()
            )
            """;

        // Memulai loop dengan inisialisasi `var index = 0`, berjalan selama `index < definition.TieBreakers.Count`, lalu memperbarui pencacah melalui
        // `index++` dalam WriteRulesetDefinitionAsync.
        for (var index = 0; index < definition.TieBreakers.Count; index++)
        // Membuka scope loop dengan syarat `index < definition.TieBreakers.Count`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // WriteRulesetDefinitionAsync.
        {
            // Menyiapkan variabel lokal `tieBreaker` untuk nilai tie breaker dengan `definition.TieBreakers[index]`, yaitu elemen koleksi yang dipilih melalui
            // indeks atau kunci tersebut. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var tieBreaker = definition.TieBreakers[index];
            // Menyiapkan variabel lokal `assetId` untuk nilai aset identitas dengan hasil operasi asinkron memanggil `InsertGameAssetAsync` dengan `conn`,
            // `tx`, `rulesetVersionId`, `”TIE_BREAKER”`, `tieBreaker.TieBreakerCode`, `$”Tie Breaker {tieBreaker.TieNumber}”`, `index + 1`, `ct`; await
            // menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var assetId = await InsertGameAssetAsync(
                // Meneruskan `conn` (koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data) sebagai argumen ke `InsertGameAssetAsync`.
                conn,
                // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke `InsertGameAssetAsync`.
                tx,
                // Meneruskan `rulesetVersionId` (identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang tepat) sebagai argumen ke
                // `InsertGameAssetAsync`.
                rulesetVersionId,
                // Meneruskan nilai literal `”TIE_BREAKER”` sebagai argumen ke `InsertGameAssetAsync`.
                "TIE_BREAKER",
                // Meneruskan `tieBreaker.TieBreakerCode` (kode kartu penentu urutan saat nilai pemain sama) sebagai argumen ke `InsertGameAssetAsync`.
                tieBreaker.TieBreakerCode,
                // Meneruskan teks interpolasi `$”Tie Breaker {tieBreaker.TieNumber}”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan
                // sebagai argumen ke `InsertGameAssetAsync`.
                $"Tie Breaker {tieBreaker.TieNumber}",
                // Meneruskan penjumlahan/penggabungan antara `index` dan `1` sebagai argumen ke `InsertGameAssetAsync`.
                index + 1,
                // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                // ke `InsertGameAssetAsync`.
                ct);
            // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition( insertTieBreakerSql, new {
            // RulesetTieBreakerId = Guid.NewGuid(), RulesetVersionId = rulesetVersionId, RulesetGameAssetId = assetId, tieBreaker.TieBreake...`; nilai hasil
            // menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam
            // WriteRulesetDefinitionAsync.
            await conn.ExecuteAsync(
                // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen ( insertTieBreakerSql, new { RulesetTieBreakerId = Guid.NewGuid(),
                // RulesetVersionId = rulesetVersionId, RulesetGameAssetId = assetId, tieBreaker.TieBreakerCode,... sebagai argumen ke `conn.ExecuteAsync`.
                new CommandDefinition(
                    // Meneruskan `insertTieBreakerSql` (nilai insert tie breaker SQL) sebagai argumen ke konstruktor `CommandDefinition`.
                    insertTieBreakerSql,
                    // Meneruskan objek anonim yang mengelompokkan RulesetTieBreakerId, RulesetVersionId, RulesetGameAssetId, tieBreaker.TieBreakerCode,
                    // tieBreaker.TieNumber, SortOrder, tieBreaker.CardQty sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                    new
                    // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                    // WriteRulesetDefinitionAsync.
                    {
                        // Meneruskan objek anonim yang mengelompokkan RulesetTieBreakerId, RulesetVersionId, RulesetGameAssetId, tieBreaker.TieBreakerCode,
                        // tieBreaker.TieNumber, SortOrder, tieBreaker.CardQty sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        RulesetTieBreakerId = Guid.NewGuid(),
                        // Meneruskan objek anonim yang mengelompokkan RulesetTieBreakerId, RulesetVersionId, RulesetGameAssetId, tieBreaker.TieBreakerCode,
                        // tieBreaker.TieNumber, SortOrder, tieBreaker.CardQty sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        RulesetVersionId = rulesetVersionId,
                        // Meneruskan objek anonim yang mengelompokkan RulesetTieBreakerId, RulesetVersionId, RulesetGameAssetId, tieBreaker.TieBreakerCode,
                        // tieBreaker.TieNumber, SortOrder, tieBreaker.CardQty sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        RulesetGameAssetId = assetId,
                        // Meneruskan objek anonim yang mengelompokkan RulesetTieBreakerId, RulesetVersionId, RulesetGameAssetId, tieBreaker.TieBreakerCode,
                        // tieBreaker.TieNumber, SortOrder, tieBreaker.CardQty sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        tieBreaker.TieBreakerCode,
                        // Meneruskan objek anonim yang mengelompokkan RulesetTieBreakerId, RulesetVersionId, RulesetGameAssetId, tieBreaker.TieBreakerCode,
                        // tieBreaker.TieNumber, SortOrder, tieBreaker.CardQty sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        tieBreaker.TieNumber,
                        // Meneruskan objek anonim yang mengelompokkan RulesetTieBreakerId, RulesetVersionId, RulesetGameAssetId, tieBreaker.TieBreakerCode,
                        // tieBreaker.TieNumber, SortOrder, tieBreaker.CardQty sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        SortOrder = index + 1,
                        // Meneruskan objek anonim yang mengelompokkan RulesetTieBreakerId, RulesetVersionId, RulesetGameAssetId, tieBreaker.TieBreakerCode,
                        // tieBreaker.TieNumber, SortOrder, tieBreaker.CardQty sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        tieBreaker.CardQty
                    // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
                    // WriteRulesetDefinitionAsync.
                    },
                    // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
                    tx,
                    // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                    // bernama `cancellationToken`.
                    cancellationToken: ct));
        // Menutup scope loop dengan syarat `index < definition.TieBreakers.Count`; bagian berikut berada di luar batas blok tersebut dalam
        // WriteRulesetDefinitionAsync.
        }

        // Menyiapkan variabel lokal `insertShariaLoanSql` untuk nilai insert sharia pinjaman SQL dengan literal multiline yang dirinci pada komentar di
        // dekat deklarasinya. Tipe yang dipakai adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string
        // insertShariaLoanSql = ”””`.
        // Baris literal 2: INSERT INTO menetapkan tabel dan kolom tujuan penambahan rekaman: `insert into ruleset_sharia_loans (`.
        // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_sharia_loan_id,`.
        // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_version_id,`.
        // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `loan_code,`.
        // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `item_name,`.
        // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `principal,`.
        // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `repayment_amount,`.
        // Baris literal 9: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `duration_days,`.
        // Baris literal 10: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `penalty_points,`.
        // Baris literal 11: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `sort_order,`.
        // Baris literal 12: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `card_qty,`.
        // Baris literal 13: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `payload_json,`.
        // Baris literal 14: CREATE mendefinisikan objek basis data yang diperlukan proses ini: `created_at`.
        // Baris literal 15: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 16: VALUES menyediakan nilai baris baru sesuai urutan kolom INSERT; placeholder @ diikat ke parameter perintah: `values (`.
        // Baris literal 17: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@RulesetShariaLoanId,`.
        // Baris literal 18: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@RulesetVersionId,`.
        // Baris literal 19: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@LoanCode,`.
        // Baris literal 20: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@ItemName,`.
        // Baris literal 21: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@Principal,`.
        // Baris literal 22: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@RepaymentAmount,`.
        // Baris literal 23: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@DurationDays,`.
        // Baris literal 24: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@PenaltyPoints,`.
        // Baris literal 25: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@SortOrder,`.
        // Baris literal 26: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@CardQty,`.
        // Baris literal 27: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `'{}',`.
        // Baris literal 28: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `now()`.
        // Baris literal 29: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 30: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string insertShariaLoanSql = """
            insert into ruleset_sharia_loans (
                ruleset_sharia_loan_id,
                ruleset_version_id,
                loan_code,
                item_name,
                principal,
                repayment_amount,
                duration_days,
                penalty_points,
                sort_order,
                card_qty,
                payload_json,
                created_at
            )
            values (
                @RulesetShariaLoanId,
                @RulesetVersionId,
                @LoanCode,
                @ItemName,
                @Principal,
                @RepaymentAmount,
                @DurationDays,
                @PenaltyPoints,
                @SortOrder,
                @CardQty,
                '{}',
                now()
            )
            """;

        // Memulai loop dengan inisialisasi `var index = 0`, berjalan selama `index < definition.ShariaLoans.Count`, lalu memperbarui pencacah melalui
        // `index++` dalam WriteRulesetDefinitionAsync.
        for (var index = 0; index < definition.ShariaLoans.Count; index++)
        // Membuka scope loop dengan syarat `index < definition.ShariaLoans.Count`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // WriteRulesetDefinitionAsync.
        {
            // Menyiapkan variabel lokal `loan` untuk nilai pinjaman dengan `definition.ShariaLoans[index]`, yaitu elemen koleksi yang dipilih melalui indeks
            // atau kunci tersebut. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var loan = definition.ShariaLoans[index];
            // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition( insertShariaLoanSql, new {
            // RulesetShariaLoanId = Guid.NewGuid(), RulesetVersionId = rulesetVersionId, loan.LoanCode, loan.ItemName, loan.Principal, loan...`; nilai hasil
            // menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam
            // WriteRulesetDefinitionAsync.
            await conn.ExecuteAsync(
                // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen ( insertShariaLoanSql, new { RulesetShariaLoanId = Guid.NewGuid(),
                // RulesetVersionId = rulesetVersionId, loan.LoanCode, loan.ItemName, loan.Principal, loan.Repay... sebagai argumen ke `conn.ExecuteAsync`.
                new CommandDefinition(
                    // Meneruskan `insertShariaLoanSql` (nilai insert sharia pinjaman SQL) sebagai argumen ke konstruktor `CommandDefinition`.
                    insertShariaLoanSql,
                    // Meneruskan objek anonim yang mengelompokkan RulesetShariaLoanId, RulesetVersionId, loan.LoanCode, loan.ItemName, loan.Principal,
                    // loan.RepaymentAmount, loan.DurationDays, loan.PenaltyPoints, SortOrder, loan.CardQty sebagai satu nilai sebagai argumen ke konstruktor
                    // `CommandDefinition`.
                    new
                    // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                    // WriteRulesetDefinitionAsync.
                    {
                        // Meneruskan objek anonim yang mengelompokkan RulesetShariaLoanId, RulesetVersionId, loan.LoanCode, loan.ItemName, loan.Principal,
                        // loan.RepaymentAmount, loan.DurationDays, loan.PenaltyPoints, SortOrder, loan.CardQty sebagai satu nilai sebagai argumen ke konstruktor
                        // `CommandDefinition`.
                        RulesetShariaLoanId = Guid.NewGuid(),
                        // Meneruskan objek anonim yang mengelompokkan RulesetShariaLoanId, RulesetVersionId, loan.LoanCode, loan.ItemName, loan.Principal,
                        // loan.RepaymentAmount, loan.DurationDays, loan.PenaltyPoints, SortOrder, loan.CardQty sebagai satu nilai sebagai argumen ke konstruktor
                        // `CommandDefinition`.
                        RulesetVersionId = rulesetVersionId,
                        // Meneruskan objek anonim yang mengelompokkan RulesetShariaLoanId, RulesetVersionId, loan.LoanCode, loan.ItemName, loan.Principal,
                        // loan.RepaymentAmount, loan.DurationDays, loan.PenaltyPoints, SortOrder, loan.CardQty sebagai satu nilai sebagai argumen ke konstruktor
                        // `CommandDefinition`.
                        loan.LoanCode,
                        // Meneruskan objek anonim yang mengelompokkan RulesetShariaLoanId, RulesetVersionId, loan.LoanCode, loan.ItemName, loan.Principal,
                        // loan.RepaymentAmount, loan.DurationDays, loan.PenaltyPoints, SortOrder, loan.CardQty sebagai satu nilai sebagai argumen ke konstruktor
                        // `CommandDefinition`.
                        loan.ItemName,
                        // Meneruskan objek anonim yang mengelompokkan RulesetShariaLoanId, RulesetVersionId, loan.LoanCode, loan.ItemName, loan.Principal,
                        // loan.RepaymentAmount, loan.DurationDays, loan.PenaltyPoints, SortOrder, loan.CardQty sebagai satu nilai sebagai argumen ke konstruktor
                        // `CommandDefinition`.
                        loan.Principal,
                        // Meneruskan objek anonim yang mengelompokkan RulesetShariaLoanId, RulesetVersionId, loan.LoanCode, loan.ItemName, loan.Principal,
                        // loan.RepaymentAmount, loan.DurationDays, loan.PenaltyPoints, SortOrder, loan.CardQty sebagai satu nilai sebagai argumen ke konstruktor
                        // `CommandDefinition`.
                        loan.RepaymentAmount,
                        // Meneruskan objek anonim yang mengelompokkan RulesetShariaLoanId, RulesetVersionId, loan.LoanCode, loan.ItemName, loan.Principal,
                        // loan.RepaymentAmount, loan.DurationDays, loan.PenaltyPoints, SortOrder, loan.CardQty sebagai satu nilai sebagai argumen ke konstruktor
                        // `CommandDefinition`.
                        loan.DurationDays,
                        // Meneruskan objek anonim yang mengelompokkan RulesetShariaLoanId, RulesetVersionId, loan.LoanCode, loan.ItemName, loan.Principal,
                        // loan.RepaymentAmount, loan.DurationDays, loan.PenaltyPoints, SortOrder, loan.CardQty sebagai satu nilai sebagai argumen ke konstruktor
                        // `CommandDefinition`.
                        loan.PenaltyPoints,
                        // Meneruskan objek anonim yang mengelompokkan RulesetShariaLoanId, RulesetVersionId, loan.LoanCode, loan.ItemName, loan.Principal,
                        // loan.RepaymentAmount, loan.DurationDays, loan.PenaltyPoints, SortOrder, loan.CardQty sebagai satu nilai sebagai argumen ke konstruktor
                        // `CommandDefinition`.
                        SortOrder = index + 1,
                        // Meneruskan objek anonim yang mengelompokkan RulesetShariaLoanId, RulesetVersionId, loan.LoanCode, loan.ItemName, loan.Principal,
                        // loan.RepaymentAmount, loan.DurationDays, loan.PenaltyPoints, SortOrder, loan.CardQty sebagai satu nilai sebagai argumen ke konstruktor
                        // `CommandDefinition`.
                        loan.CardQty
                    // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
                    // WriteRulesetDefinitionAsync.
                    },
                    // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
                    tx,
                    // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                    // bernama `cancellationToken`.
                    cancellationToken: ct));
        // Menutup scope loop dengan syarat `index < definition.ShariaLoans.Count`; bagian berikut berada di luar batas blok tersebut dalam
        // WriteRulesetDefinitionAsync.
        }

        // Menyiapkan variabel lokal `insertInsuranceProductSql` untuk nilai insert asuransi product SQL dengan literal multiline yang dirinci pada komentar
        // di dekat deklarasinya. Tipe yang dipakai adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string
        // insertInsuranceProductSql = ”””`.
        // Baris literal 2: INSERT INTO menetapkan tabel dan kolom tujuan penambahan rekaman: `insert into ruleset_insurance_products (`.
        // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun:
        // `ruleset_insurance_product_id,`.
        // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_version_id,`.
        // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `product_code,`.
        // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `item_name,`.
        // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `premium,`.
        // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `usage_limit,`.
        // Baris literal 9: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `sort_order,`.
        // Baris literal 10: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `card_qty,`.
        // Baris literal 11: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `payload_json,`.
        // Baris literal 12: CREATE mendefinisikan objek basis data yang diperlukan proses ini: `created_at`.
        // Baris literal 13: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 14: VALUES menyediakan nilai baris baru sesuai urutan kolom INSERT; placeholder @ diikat ke parameter perintah: `values (`.
        // Baris literal 15: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@RulesetInsuranceProductId,`.
        // Baris literal 16: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@RulesetVersionId,`.
        // Baris literal 17: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@ProductCode,`.
        // Baris literal 18: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@ItemName,`.
        // Baris literal 19: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@Premium,`.
        // Baris literal 20: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@UsageLimit,`.
        // Baris literal 21: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@SortOrder,`.
        // Baris literal 22: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@CardQty,`.
        // Baris literal 23: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `'{}',`.
        // Baris literal 24: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `now()`.
        // Baris literal 25: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 26: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string insertInsuranceProductSql = """
            insert into ruleset_insurance_products (
                ruleset_insurance_product_id,
                ruleset_version_id,
                product_code,
                item_name,
                premium,
                usage_limit,
                sort_order,
                card_qty,
                payload_json,
                created_at
            )
            values (
                @RulesetInsuranceProductId,
                @RulesetVersionId,
                @ProductCode,
                @ItemName,
                @Premium,
                @UsageLimit,
                @SortOrder,
                @CardQty,
                '{}',
                now()
            )
            """;

        // Memulai loop dengan inisialisasi `var index = 0`, berjalan selama `index < definition.InsuranceProducts.Count`, lalu memperbarui pencacah melalui
        // `index++` dalam WriteRulesetDefinitionAsync.
        for (var index = 0; index < definition.InsuranceProducts.Count; index++)
        // Membuka scope loop dengan syarat `index < definition.InsuranceProducts.Count`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // WriteRulesetDefinitionAsync.
        {
            // Menyiapkan variabel lokal `product` untuk nilai product dengan `definition.InsuranceProducts[index]`, yaitu elemen koleksi yang dipilih melalui
            // indeks atau kunci tersebut. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var product = definition.InsuranceProducts[index];
            // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition( insertInsuranceProductSql, new {
            // RulesetInsuranceProductId = Guid.NewGuid(), RulesetVersionId = rulesetVersionId, product.ProductCode, product.ItemName,...`; nilai hasil
            // menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam
            // WriteRulesetDefinitionAsync.
            await conn.ExecuteAsync(
                // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen ( insertInsuranceProductSql, new { RulesetInsuranceProductId = Guid.NewGuid(),
                // RulesetVersionId = rulesetVersionId, product.ProductCode, product.ItemName, produ... sebagai argumen ke `conn.ExecuteAsync`.
                new CommandDefinition(
                    // Meneruskan `insertInsuranceProductSql` (nilai insert asuransi product SQL) sebagai argumen ke konstruktor `CommandDefinition`.
                    insertInsuranceProductSql,
                    // Meneruskan objek anonim yang mengelompokkan RulesetInsuranceProductId, RulesetVersionId, product.ProductCode, product.ItemName, product.Premium,
                    // product.UsageLimit, SortOrder, product.CardQty sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                    new
                    // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                    // WriteRulesetDefinitionAsync.
                    {
                        // Meneruskan objek anonim yang mengelompokkan RulesetInsuranceProductId, RulesetVersionId, product.ProductCode, product.ItemName, product.Premium,
                        // product.UsageLimit, SortOrder, product.CardQty sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        RulesetInsuranceProductId = Guid.NewGuid(),
                        // Meneruskan objek anonim yang mengelompokkan RulesetInsuranceProductId, RulesetVersionId, product.ProductCode, product.ItemName, product.Premium,
                        // product.UsageLimit, SortOrder, product.CardQty sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        RulesetVersionId = rulesetVersionId,
                        // Meneruskan objek anonim yang mengelompokkan RulesetInsuranceProductId, RulesetVersionId, product.ProductCode, product.ItemName, product.Premium,
                        // product.UsageLimit, SortOrder, product.CardQty sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        product.ProductCode,
                        // Meneruskan objek anonim yang mengelompokkan RulesetInsuranceProductId, RulesetVersionId, product.ProductCode, product.ItemName, product.Premium,
                        // product.UsageLimit, SortOrder, product.CardQty sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        product.ItemName,
                        // Meneruskan objek anonim yang mengelompokkan RulesetInsuranceProductId, RulesetVersionId, product.ProductCode, product.ItemName, product.Premium,
                        // product.UsageLimit, SortOrder, product.CardQty sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        product.Premium,
                        // Meneruskan objek anonim yang mengelompokkan RulesetInsuranceProductId, RulesetVersionId, product.ProductCode, product.ItemName, product.Premium,
                        // product.UsageLimit, SortOrder, product.CardQty sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        product.UsageLimit,
                        // Meneruskan objek anonim yang mengelompokkan RulesetInsuranceProductId, RulesetVersionId, product.ProductCode, product.ItemName, product.Premium,
                        // product.UsageLimit, SortOrder, product.CardQty sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        SortOrder = index + 1,
                        // Meneruskan objek anonim yang mengelompokkan RulesetInsuranceProductId, RulesetVersionId, product.ProductCode, product.ItemName, product.Premium,
                        // product.UsageLimit, SortOrder, product.CardQty sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        product.CardQty
                    // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
                    // WriteRulesetDefinitionAsync.
                    },
                    // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
                    tx,
                    // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                    // bernama `cancellationToken`.
                    cancellationToken: ct));
        // Menutup scope loop dengan syarat `index < definition.InsuranceProducts.Count`; bagian berikut berada di luar batas blok tersebut dalam
        // WriteRulesetDefinitionAsync.
        }

        // Menyiapkan variabel lokal `insertLifeRiskSql` untuk nilai insert life risiko SQL dengan literal multiline yang dirinci pada komentar di dekat
        // deklarasinya. Tipe yang dipakai adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string
        // insertLifeRiskSql = ”””`.
        // Baris literal 2: INSERT INTO menetapkan tabel dan kolom tujuan penambahan rekaman: `insert into ruleset_life_risks (`.
        // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_life_risk_id,`.
        // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_version_id,`.
        // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_game_asset_id,`.
        // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `risk_code,`.
        // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `item_name,`.
        // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `effect_type,`.
        // Baris literal 9: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `direction,`.
        // Baris literal 10: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `amount,`.
        // Baris literal 11: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `duration_days,`.
        // Baris literal 12: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `target_scope,`.
        // Baris literal 13: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `sort_order,`.
        // Baris literal 14: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `card_qty,`.
        // Baris literal 15: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `payload_json,`.
        // Baris literal 16: CREATE mendefinisikan objek basis data yang diperlukan proses ini: `created_at`.
        // Baris literal 17: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 18: VALUES menyediakan nilai baris baru sesuai urutan kolom INSERT; placeholder @ diikat ke parameter perintah: `values (`.
        // Baris literal 19: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@RulesetLifeRiskId,`.
        // Baris literal 20: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@RulesetVersionId,`.
        // Baris literal 21: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@RulesetGameAssetId,`.
        // Baris literal 22: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@RiskCode,`.
        // Baris literal 23: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@ItemName,`.
        // Baris literal 24: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@EffectType,`.
        // Baris literal 25: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@Direction,`.
        // Baris literal 26: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@Amount,`.
        // Baris literal 27: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@DurationDays,`.
        // Baris literal 28: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@TargetScope,`.
        // Baris literal 29: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@SortOrder,`.
        // Baris literal 30: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@CardQty,`.
        // Baris literal 31: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `jsonb_strip_nulls(jsonb_build_object('value_delta', @ValueDelta)),`.
        // Baris literal 32: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `now()`.
        // Baris literal 33: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 34: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string insertLifeRiskSql = """
            insert into ruleset_life_risks (
                ruleset_life_risk_id,
                ruleset_version_id,
                ruleset_game_asset_id,
                risk_code,
                item_name,
                effect_type,
                direction,
                amount,
                duration_days,
                target_scope,
                sort_order,
                card_qty,
                payload_json,
                created_at
            )
            values (
                @RulesetLifeRiskId,
                @RulesetVersionId,
                @RulesetGameAssetId,
                @RiskCode,
                @ItemName,
                @EffectType,
                @Direction,
                @Amount,
                @DurationDays,
                @TargetScope,
                @SortOrder,
                @CardQty,
                jsonb_strip_nulls(jsonb_build_object('value_delta', @ValueDelta)),
                now()
            )
            """;

        // Memulai loop dengan inisialisasi `var index = 0`, berjalan selama `index < definition.LifeRisks.Count`, lalu memperbarui pencacah melalui
        // `index++` dalam WriteRulesetDefinitionAsync.
        for (var index = 0; index < definition.LifeRisks.Count; index++)
        // Membuka scope loop dengan syarat `index < definition.LifeRisks.Count`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // WriteRulesetDefinitionAsync.
        {
            // Menyiapkan variabel lokal `risk` untuk nilai risiko dengan `definition.LifeRisks[index]`, yaitu elemen koleksi yang dipilih melalui indeks atau
            // kunci tersebut. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var risk = definition.LifeRisks[index];
            // Menyiapkan variabel lokal `assetId` untuk nilai aset identitas dengan hasil operasi asinkron memanggil `InsertGameAssetAsync` dengan `conn`,
            // `tx`, `rulesetVersionId`, `”RISK”`, `risk.RiskCode`, `risk.ItemName`, `index + 1`, `ct`; await menunggu hasil tanpa memblokir thread selama
            // operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var assetId = await InsertGameAssetAsync(
                // Meneruskan `conn` (koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data) sebagai argumen ke `InsertGameAssetAsync`.
                conn,
                // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke `InsertGameAssetAsync`.
                tx,
                // Meneruskan `rulesetVersionId` (identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang tepat) sebagai argumen ke
                // `InsertGameAssetAsync`.
                rulesetVersionId,
                // Meneruskan nilai literal `”RISK”` sebagai argumen ke `InsertGameAssetAsync`.
                "RISK",
                // Meneruskan `risk.RiskCode` (nilai risiko kode) sebagai argumen ke `InsertGameAssetAsync`.
                risk.RiskCode,
                // Meneruskan `risk.ItemName` (nilai elemen nama) sebagai argumen ke `InsertGameAssetAsync`.
                risk.ItemName,
                // Meneruskan penjumlahan/penggabungan antara `index` dan `1` sebagai argumen ke `InsertGameAssetAsync`.
                index + 1,
                // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                // ke `InsertGameAssetAsync`.
                ct);
            // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition( insertLifeRiskSql, new {
            // RulesetLifeRiskId = Guid.NewGuid(), RulesetVersionId = rulesetVersionId, RulesetGameAssetId = assetId, risk.RiskCode, risk.Item...`; nilai hasil
            // menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam
            // WriteRulesetDefinitionAsync.
            await conn.ExecuteAsync(
                // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen ( insertLifeRiskSql, new { RulesetLifeRiskId = Guid.NewGuid(), RulesetVersionId
                // = rulesetVersionId, RulesetGameAssetId = assetId, risk.RiskCode, risk.ItemName, ... sebagai argumen ke `conn.ExecuteAsync`.
                new CommandDefinition(
                    // Meneruskan `insertLifeRiskSql` (nilai insert life risiko SQL) sebagai argumen ke konstruktor `CommandDefinition`.
                    insertLifeRiskSql,
                    // Meneruskan objek anonim yang mengelompokkan RulesetLifeRiskId, RulesetVersionId, RulesetGameAssetId, risk.RiskCode, risk.ItemName,
                    // risk.EffectType, Direction, risk.Amount, DurationDays, TargetScope, risk.ValueDelta, SortOrder, risk.CardQty sebagai satu nilai sebagai argumen
                    // ke konstruktor `CommandDefinition`.
                    new
                    // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                    // WriteRulesetDefinitionAsync.
                    {
                        // Meneruskan objek anonim yang mengelompokkan RulesetLifeRiskId, RulesetVersionId, RulesetGameAssetId, risk.RiskCode, risk.ItemName,
                        // risk.EffectType, Direction, risk.Amount, DurationDays, TargetScope, risk.ValueDelta, SortOrder, risk.CardQty sebagai satu nilai sebagai argumen
                        // ke konstruktor `CommandDefinition`.
                        RulesetLifeRiskId = Guid.NewGuid(),
                        // Meneruskan objek anonim yang mengelompokkan RulesetLifeRiskId, RulesetVersionId, RulesetGameAssetId, risk.RiskCode, risk.ItemName,
                        // risk.EffectType, Direction, risk.Amount, DurationDays, TargetScope, risk.ValueDelta, SortOrder, risk.CardQty sebagai satu nilai sebagai argumen
                        // ke konstruktor `CommandDefinition`.
                        RulesetVersionId = rulesetVersionId,
                        // Meneruskan objek anonim yang mengelompokkan RulesetLifeRiskId, RulesetVersionId, RulesetGameAssetId, risk.RiskCode, risk.ItemName,
                        // risk.EffectType, Direction, risk.Amount, DurationDays, TargetScope, risk.ValueDelta, SortOrder, risk.CardQty sebagai satu nilai sebagai argumen
                        // ke konstruktor `CommandDefinition`.
                        RulesetGameAssetId = assetId,
                        // Meneruskan objek anonim yang mengelompokkan RulesetLifeRiskId, RulesetVersionId, RulesetGameAssetId, risk.RiskCode, risk.ItemName,
                        // risk.EffectType, Direction, risk.Amount, DurationDays, TargetScope, risk.ValueDelta, SortOrder, risk.CardQty sebagai satu nilai sebagai argumen
                        // ke konstruktor `CommandDefinition`.
                        risk.RiskCode,
                        // Meneruskan objek anonim yang mengelompokkan RulesetLifeRiskId, RulesetVersionId, RulesetGameAssetId, risk.RiskCode, risk.ItemName,
                        // risk.EffectType, Direction, risk.Amount, DurationDays, TargetScope, risk.ValueDelta, SortOrder, risk.CardQty sebagai satu nilai sebagai argumen
                        // ke konstruktor `CommandDefinition`.
                        risk.ItemName,
                        // Meneruskan objek anonim yang mengelompokkan RulesetLifeRiskId, RulesetVersionId, RulesetGameAssetId, risk.RiskCode, risk.ItemName,
                        // risk.EffectType, Direction, risk.Amount, DurationDays, TargetScope, risk.ValueDelta, SortOrder, risk.CardQty sebagai satu nilai sebagai argumen
                        // ke konstruktor `CommandDefinition`.
                        risk.EffectType,
                        // Meneruskan `risk.Direction` (nilai direction) sebagai argumen ke `string.IsNullOrWhiteSpace`.
                        Direction = string.IsNullOrWhiteSpace(risk.Direction) ? null : risk.Direction,
                        // Meneruskan objek anonim yang mengelompokkan RulesetLifeRiskId, RulesetVersionId, RulesetGameAssetId, risk.RiskCode, risk.ItemName,
                        // risk.EffectType, Direction, risk.Amount, DurationDays, TargetScope, risk.ValueDelta, SortOrder, risk.CardQty sebagai satu nilai sebagai argumen
                        // ke konstruktor `CommandDefinition`.
                        risk.Amount,
                        // Meneruskan nilai literal `1` sebagai argumen ke `risk.DurationDays.GetValueOrDefault`.
                        DurationDays = risk.DurationDays.GetValueOrDefault(1),
                        // Meneruskan `risk.TargetScope` (nilai target cakupan) sebagai argumen ke `string.IsNullOrWhiteSpace`.
                        TargetScope = string.IsNullOrWhiteSpace(risk.TargetScope) ? "SELF" : risk.TargetScope,
                        // Meneruskan objek anonim yang mengelompokkan RulesetLifeRiskId, RulesetVersionId, RulesetGameAssetId, risk.RiskCode, risk.ItemName,
                        // risk.EffectType, Direction, risk.Amount, DurationDays, TargetScope, risk.ValueDelta, SortOrder, risk.CardQty sebagai satu nilai sebagai argumen
                        // ke konstruktor `CommandDefinition`.
                        risk.ValueDelta,
                        // Meneruskan objek anonim yang mengelompokkan RulesetLifeRiskId, RulesetVersionId, RulesetGameAssetId, risk.RiskCode, risk.ItemName,
                        // risk.EffectType, Direction, risk.Amount, DurationDays, TargetScope, risk.ValueDelta, SortOrder, risk.CardQty sebagai satu nilai sebagai argumen
                        // ke konstruktor `CommandDefinition`.
                        SortOrder = index + 1,
                        // Meneruskan objek anonim yang mengelompokkan RulesetLifeRiskId, RulesetVersionId, RulesetGameAssetId, risk.RiskCode, risk.ItemName,
                        // risk.EffectType, Direction, risk.Amount, DurationDays, TargetScope, risk.ValueDelta, SortOrder, risk.CardQty sebagai satu nilai sebagai argumen
                        // ke konstruktor `CommandDefinition`.
                        risk.CardQty
                    // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
                    // WriteRulesetDefinitionAsync.
                    },
                    // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
                    tx,
                    // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                    // bernama `cancellationToken`.
                    cancellationToken: ct));
        // Menutup scope loop dengan syarat `index < definition.LifeRisks.Count`; bagian berikut berada di luar batas blok tersebut dalam
        // WriteRulesetDefinitionAsync.
        }
    // Menutup scope metode WriteRulesetDefinitionAsync; bagian berikut berada di luar batas blok tersebut dalam WriteRulesetDefinitionAsync.
    }

    // Mendefinisikan metode `ReadRulesetDefinitionAsync` dengan hasil bertipe `Task<RulesetDefinitionDto?>`; operasi ini menangani read aturan definisi
    // asinkron. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `conn`
    // bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data; Parameter `rulesetVersionId` bertipe
    // `Guid` membawa identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang tepat; Parameter `ct` bertipe `CancellationToken`
    // membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    private async Task<RulesetDefinitionDto?> ReadRulesetDefinitionAsync(
        // Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data.
        NpgsqlConnection conn,
        // Parameter `rulesetVersionId` bertipe `Guid` membawa identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang tepat.
        Guid rulesetVersionId,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    // Membuka scope metode ReadRulesetDefinitionAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ReadRulesetDefinitionAsync.
    {
        // Menyiapkan variabel lokal `versionSql` untuk nilai versi SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang
        // dipakai adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string versionSql =
        // ”””`.
        // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select mode`.
        // Baris literal 3: FROM memilih tabel/subquery sumber pembacaan: `from ruleset_versions`.
        // Baris literal 4: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where ruleset_version_id = @rulesetVersionId`.
        // Baris literal 5: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string versionSql = """
            select mode
            from ruleset_versions
            where ruleset_version_id = @rulesetVersionId
            """;

        // Menyiapkan variabel lokal `mode` untuk mode permainan yang menentukan kelompok aturan yang digunakan dengan hasil operasi asinkron menjalankan
        // perintah basis data melalui `conn` dengan `new CommandDefinition(versionSql, new { rulesetVersionId }, cancellationToken: ct)` dan mengambil
        // nilai skalar hasilnya; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai
        // awal.
        var mode = await conn.ExecuteScalarAsync<string?>(
            // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen (versionSql, new { rulesetVersionId }, cancellationToken: ct) sebagai argumen ke
            // `conn.ExecuteScalarAsync<string?>`; Meneruskan `versionSql` (nilai versi SQL) sebagai argumen ke konstruktor `CommandDefinition`; Meneruskan
            // objek anonim yang mengelompokkan rulesetVersionId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`; Meneruskan `ct` (sinyal
            // pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen bernama
            // `cancellationToken`.
            new CommandDefinition(versionSql, new { rulesetVersionId }, cancellationToken: ct));
        // Memeriksa memeriksa apakah `mode` null, kosong, atau hanya berisi karakter spasi; blok if hanya dijalankan ketika kondisi ini bernilai benar
        // dalam ReadRulesetDefinitionAsync.
        if (string.IsNullOrWhiteSpace(mode))
        // Membuka scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(mode)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ReadRulesetDefinitionAsync.
        {
            // Mengembalikan null, yaitu penanda tidak ada nilai kepada pemanggil dalam ReadRulesetDefinitionAsync; eksekusi jalur ini selesai setelah nilai
            // hasil ditentukan.
            return null;
        // Menutup scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(mode)`; bagian berikut berada di luar batas blok tersebut dalam
        // ReadRulesetDefinitionAsync.
        }

        // Menyiapkan variabel lokal `settingsSql` untuk nilai settings SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe
        // yang dipakai adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string settingsSql =
        // ”””`.
        // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
        // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `starting_cash as
        // StartingCash,`.
        // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `starting_happiness as
        // InitialHappiness,`.
        // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `starting_saving as
        // InitialSaving,`.
        // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `actions_per_turn as
        // ActionsPerTurn,`.
        // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `finish_day as FinishDay,`.
        // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `min_players as MinPlayers,`.
        // Baris literal 9: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `max_players as MaxPlayers,`.
        // Baris literal 10: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `cash_min as CashMin,`.
        // Baris literal 11: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `max_ingredient_total as
        // MaxIngredientTotal,`.
        // Baris literal 12: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `max_same_ingredient as
        // MaxSameIngredient,`.
        // Baris literal 13: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `primary_need_max_per_day as
        // PrimaryNeedMaxPerDay,`.
        // Baris literal 14: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun:
        // `require_primary_before_others as RequirePrimaryBeforeOthers,`.
        // Baris literal 15: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `donation_min_amount as
        // DonationMinAmount,`.
        // Baris literal 16: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `donation_max_amount as
        // DonationMaxAmount,`.
        // Baris literal 17: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `gold_trade_allow_buy as
        // GoldTradeAllowBuy,`.
        // Baris literal 18: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `gold_trade_allow_sell as
        // GoldTradeAllowSell,`.
        // Baris literal 19: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `loan_enabled as
        // LoanEnabled,`.
        // Baris literal 20: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `insurance_enabled as
        // InsuranceEnabled,`.
        // Baris literal 21: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `saving_goal_enabled as
        // SavingGoalEnabled,`.
        // Baris literal 22: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `freelance_income as
        // FreelanceIncome`.
        // Baris literal 23: FROM memilih tabel/subquery sumber pembacaan: `from ruleset_game_settings`.
        // Baris literal 24: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where ruleset_version_id = @rulesetVersionId`.
        // Baris literal 25: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string settingsSql = """
            select
                starting_cash as StartingCash,
                starting_happiness as InitialHappiness,
                starting_saving as InitialSaving,
                actions_per_turn as ActionsPerTurn,
                finish_day as FinishDay,
                min_players as MinPlayers,
                max_players as MaxPlayers,
                cash_min as CashMin,
                max_ingredient_total as MaxIngredientTotal,
                max_same_ingredient as MaxSameIngredient,
                primary_need_max_per_day as PrimaryNeedMaxPerDay,
                require_primary_before_others as RequirePrimaryBeforeOthers,
                donation_min_amount as DonationMinAmount,
                donation_max_amount as DonationMaxAmount,
                gold_trade_allow_buy as GoldTradeAllowBuy,
                gold_trade_allow_sell as GoldTradeAllowSell,
                loan_enabled as LoanEnabled,
                insurance_enabled as InsuranceEnabled,
                saving_goal_enabled as SavingGoalEnabled,
                freelance_income as FreelanceIncome
            from ruleset_game_settings
            where ruleset_version_id = @rulesetVersionId
            """;

        // Menyiapkan variabel lokal `settings` untuk nilai settings dengan hasil operasi asinkron membaca satu hasil basis data melalui
        // `conn.QuerySingleOrDefaultAsync<RulesetSettingsRow>` dengan `new CommandDefinition(settingsSql, new { rulesetVersionId }, cancellationToken:
        // ct)`; nilai default menunjukkan tidak ada baris hasil; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var settings = await conn.QuerySingleOrDefaultAsync<RulesetSettingsRow>(
            // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen (settingsSql, new { rulesetVersionId }, cancellationToken: ct) sebagai argumen
            // ke `conn.QuerySingleOrDefaultAsync<RulesetSettingsRow>`; Meneruskan `settingsSql` (nilai settings SQL) sebagai argumen ke konstruktor
            // `CommandDefinition`; Meneruskan objek anonim yang mengelompokkan rulesetVersionId sebagai satu nilai sebagai argumen ke konstruktor
            // `CommandDefinition`; Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi
            // berhenti) sebagai argumen bernama `cancellationToken`.
            new CommandDefinition(settingsSql, new { rulesetVersionId }, cancellationToken: ct));
        // Memeriksa hasil pencocokan `settings` dengan pola `null`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // ReadRulesetDefinitionAsync.
        if (settings is null)
        // Membuka scope cabang if untuk kondisi `settings is null`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ReadRulesetDefinitionAsync.
        {
            // Mengembalikan null, yaitu penanda tidak ada nilai kepada pemanggil dalam ReadRulesetDefinitionAsync; eksekusi jalur ini selesai setelah nilai
            // hasil ditentukan.
            return null;
        // Menutup scope cabang if untuk kondisi `settings is null`; bagian berikut berada di luar batas blok tersebut dalam ReadRulesetDefinitionAsync.
        }

        // Menyiapkan variabel lokal `orderingRules` untuk nilai ordering rules dengan mematerialisasi urutan `(await
        // conn.QueryAsync<PlayerOrderingRuleRow>( new CommandDefinition( ””” select ordering_code as OrderingCode, weekday_code as WeekdayCode,
        // feature_code as FeatureCode, is_en...` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var orderingRules = (await conn.QueryAsync<PlayerOrderingRuleRow>(
            // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen ( ””” select ordering_code as OrderingCode, weekday_code as WeekdayCode,
            // feature_code as FeatureCode, is_enabled as IsEnabled, sort_order as SortOrder from rule... sebagai argumen ke
            // `conn.QueryAsync<PlayerOrderingRuleRow>`.
            new CommandDefinition(
                // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke konstruktor `CommandDefinition`.
                // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
                // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
                // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
                // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ordering_code as
                // OrderingCode,`.
                // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `weekday_code as
                // WeekdayCode,`.
                // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `feature_code as
                // FeatureCode,`.
                // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `is_enabled as IsEnabled,`.
                // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `sort_order as SortOrder`.
                // Baris literal 8: FROM memilih tabel/subquery sumber pembacaan: `from ruleset_player_ordering_rules`.
                // Baris literal 9: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where ruleset_version_id = @rulesetVersionId`.
                // Baris literal 10: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by sort_order asc`.
                // Baris literal 11: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
                """
                select
                    ordering_code as OrderingCode,
                    weekday_code as WeekdayCode,
                    feature_code as FeatureCode,
                    is_enabled as IsEnabled,
                    sort_order as SortOrder
                from ruleset_player_ordering_rules
                where ruleset_version_id = @rulesetVersionId
                order by sort_order asc
                """,
                // Meneruskan objek anonim yang mengelompokkan rulesetVersionId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                new { rulesetVersionId },
                // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                // bernama `cancellationToken`.
                cancellationToken: ct))).ToList();

        // Menyiapkan variabel lokal `instructorUsernames` untuk nilai instruktur usernames dengan objek baru bertipe `List<string>` dengan nilai awal
        // sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var instructorUsernames = new List<string>();

        // Menyiapkan variabel lokal `actions` untuk nilai aksi dengan mematerialisasi urutan `(await conn.QueryAsync<RulesetActionDto>( new
        // CommandDefinition( ””” select action_id as ActionId from ruleset_actions where ruleset_version_id = @rulesetVersionId and is_acti...` menjadi
        // List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var actions = (await conn.QueryAsync<RulesetActionDto>(
            // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen ( ””” select action_id as ActionId from ruleset_actions where ruleset_version_id
            // = @rulesetVersionId and is_active order by sort_order asc, action_id asc ”””, n... sebagai argumen ke `conn.QueryAsync<RulesetActionDto>`.
            new CommandDefinition(
                // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke konstruktor `CommandDefinition`.
                // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
                // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
                // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select action_id as ActionId`.
                // Baris literal 3: FROM memilih tabel/subquery sumber pembacaan: `from ruleset_actions`.
                // Baris literal 4: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where ruleset_version_id = @rulesetVersionId`.
                // Baris literal 5: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and is_active`.
                // Baris literal 6: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by sort_order asc, action_id asc`.
                // Baris literal 7: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
                """
                select action_id as ActionId
                from ruleset_actions
                where ruleset_version_id = @rulesetVersionId
                  and is_active
                order by sort_order asc, action_id asc
                """,
                // Meneruskan objek anonim yang mengelompokkan rulesetVersionId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                new { rulesetVersionId },
                // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                // bernama `cancellationToken`.
                cancellationToken: ct))).ToList();

        // Menyiapkan variabel lokal `ingredients` untuk nilai bahan dengan mematerialisasi urutan `(await conn.QueryAsync<RulesetIngredientDto>( new
        // CommandDefinition( ””” select ingredient_code as Id, display_name as Nama, purchase_price as HargaBeli, card_qty as CardQty f...` menjadi List;
        // enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var ingredients = (await conn.QueryAsync<RulesetIngredientDto>(
            // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen ( ””” select ingredient_code as Id, display_name as Nama, purchase_price as
            // HargaBeli, card_qty as CardQty from ruleset_ingredients where ruleset_version_id = @... sebagai argumen ke
            // `conn.QueryAsync<RulesetIngredientDto>`.
            new CommandDefinition(
                // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke konstruktor `CommandDefinition`.
                // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
                // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
                // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
                // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ingredient_code as Id,`.
                // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `display_name as Nama,`.
                // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `purchase_price as
                // HargaBeli,`.
                // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `card_qty as CardQty`.
                // Baris literal 7: FROM memilih tabel/subquery sumber pembacaan: `from ruleset_ingredients`.
                // Baris literal 8: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where ruleset_version_id = @rulesetVersionId`.
                // Baris literal 9: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and is_active`.
                // Baris literal 10: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by sort_order asc, ingredient_code asc`.
                // Baris literal 11: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
                """
                select
                    ingredient_code as Id,
                    display_name as Nama,
                    purchase_price as HargaBeli,
                    card_qty as CardQty
                from ruleset_ingredients
                where ruleset_version_id = @rulesetVersionId
                  and is_active
                order by sort_order asc, ingredient_code asc
                """,
                // Meneruskan objek anonim yang mengelompokkan rulesetVersionId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                new { rulesetVersionId },
                // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                // bernama `cancellationToken`.
                cancellationToken: ct))).ToList();

        // Menyiapkan variabel lokal `orderRows` untuk nilai urutan/pesanan baris dengan mematerialisasi urutan `(await conn.QueryAsync<OrderRow>( new
        // CommandDefinition( ””” select ruleset_order_id as RulesetOrderId, order_code as Id, item_name as Nama, sell_price as HargaJual, happiness...`
        // menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var orderRows = (await conn.QueryAsync<OrderRow>(
            // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen ( ””” select ruleset_order_id as RulesetOrderId, order_code as Id, item_name as
            // Nama, sell_price as HargaJual, happiness_points as PoinKebahagiaan, sort_order a... sebagai argumen ke `conn.QueryAsync<OrderRow>`.
            new CommandDefinition(
                // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke konstruktor `CommandDefinition`.
                // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
                // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
                // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
                // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_order_id as
                // RulesetOrderId,`.
                // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `order_code as Id,`.
                // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `item_name as Nama,`.
                // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `sell_price as HargaJual,`.
                // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `happiness_points as
                // PoinKebahagiaan,`.
                // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `sort_order as SortOrder,`.
                // Baris literal 9: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `card_qty as CardQty`.
                // Baris literal 10: FROM memilih tabel/subquery sumber pembacaan: `from ruleset_orders`.
                // Baris literal 11: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where ruleset_version_id = @rulesetVersionId`.
                // Baris literal 12: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and is_active`.
                // Baris literal 13: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by sort_order asc, order_code asc`.
                // Baris literal 14: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
                """
                select
                    ruleset_order_id as RulesetOrderId,
                    order_code as Id,
                    item_name as Nama,
                    sell_price as HargaJual,
                    happiness_points as PoinKebahagiaan,
                    sort_order as SortOrder,
                    card_qty as CardQty
                from ruleset_orders
                where ruleset_version_id = @rulesetVersionId
                  and is_active
                order by sort_order asc, order_code asc
                """,
                // Meneruskan objek anonim yang mengelompokkan rulesetVersionId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                new { rulesetVersionId },
                // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                // bernama `cancellationToken`.
                cancellationToken: ct))).ToList();

        // Menyiapkan variabel lokal `orderRequirementLookup` untuk nilai urutan/pesanan requirement lookup dengan membangun kamus dari `(await
        // conn.QueryAsync<OrderRequirementRow>( new CommandDefinition( ””” select requirement.ruleset_order_id as RulesetOrderId,
        // requirement.requirement_order as RequirementOrde...` dengan pemilihan kunci/nilai `group => group.Key`, `group => group .OrderBy(item =>
        // item.RequirementOrder) .SelectMany(item => Enumerable.Repeat(item.IngredientValue, item.QtyRequired)) .ToList()`; kunci harus unik agar konversi
        // berhasil. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var orderRequirementLookup = (await conn.QueryAsync<OrderRequirementRow>(
            // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen ( ””” select requirement.ruleset_order_id as RulesetOrderId,
            // requirement.requirement_order as RequirementOrder, asset.display_name as IngredientValue, requireme... sebagai argumen ke
            // `conn.QueryAsync<OrderRequirementRow>`.
            new CommandDefinition(
                // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke konstruktor `CommandDefinition`.
                // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
                // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
                // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
                // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `requirement.ruleset_order_id
                // as RulesetOrderId,`.
                // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `requirement.requirement_order
                // as RequirementOrder,`.
                // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `asset.display_name as
                // IngredientValue,`.
                // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `requirement.qty_required as
                // QtyRequired`.
                // Baris literal 7: FROM memilih tabel/subquery sumber pembacaan: `from ruleset_order_requirements requirement`.
                // Baris literal 8: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join ruleset_game_assets asset`.
                // Baris literal 9: ON menetapkan syarat pencocokan baris pada relasi JOIN: `on asset.ruleset_game_asset_id = requirement.required_asset_id`.
                // Baris literal 10: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where requirement.ruleset_order_id in (`.
                // Baris literal 11: SELECT menentukan nilai atau kolom yang dikembalikan query: `select ruleset_order_id`.
                // Baris literal 12: FROM memilih tabel/subquery sumber pembacaan: `from ruleset_orders`.
                // Baris literal 13: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where ruleset_version_id = @rulesetVersionId`.
                // Baris literal 14: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
                // Baris literal 15: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by requirement_order asc`.
                // Baris literal 16: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
                """
                select
                    requirement.ruleset_order_id as RulesetOrderId,
                    requirement.requirement_order as RequirementOrder,
                    asset.display_name as IngredientValue,
                    requirement.qty_required as QtyRequired
                from ruleset_order_requirements requirement
                join ruleset_game_assets asset
                  on asset.ruleset_game_asset_id = requirement.required_asset_id
                where requirement.ruleset_order_id in (
                    select ruleset_order_id
                    from ruleset_orders
                    where ruleset_version_id = @rulesetVersionId
                )
                order by requirement_order asc
                """,
                // Meneruskan objek anonim yang mengelompokkan rulesetVersionId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                new { rulesetVersionId },
                // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                // bernama `cancellationToken`.
                cancellationToken: ct)))
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .GroupBy(row => row.RulesetOrderId) dalam ReadRulesetDefinitionAsync; token
            // pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .GroupBy(row => row.RulesetOrderId)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToDictionary( dalam ReadRulesetDefinitionAsync; token pada baris ini
            // menyambungkan bagian kode sebelum dan sesudahnya.
            .ToDictionary(
                // Parameter `group` bertipe `` membawa nilai group.
                group => group.Key,
                // Parameter `group` bertipe `` membawa nilai group.
                group => group
                    // Meneruskan fungsi lambda `item => item.RequirementOrder` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen ke
                    // `group .OrderBy`.
                    .OrderBy(item => item.RequirementOrder)
                    // Meneruskan fungsi lambda `item => Enumerable.Repeat(item.IngredientValue, item.QtyRequired)` yang dijalankan oleh operasi pemanggil untuk
                    // memproses setiap masukan sebagai argumen ke `group .OrderBy(item => item.RequirementOrder) .SelectMany`; Meneruskan `item.IngredientValue` (nilai
                    // bahan nilai) sebagai argumen ke `Enumerable.Repeat`; Meneruskan `item.QtyRequired` (nilai qty required) sebagai argumen ke `Enumerable.Repeat`.
                    .SelectMany(item => Enumerable.Repeat(item.IngredientValue, item.QtyRequired))
                    // Meneruskan fungsi lambda `group => group .OrderBy(item => item.RequirementOrder) .SelectMany(item => Enumerable.Repeat(item.IngredientValue,
                    // item.QtyRequired)) .ToList()` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen ke `(await
                    // conn.QueryAsync<OrderRequirementRow>( new CommandDefinition( ””” select requirement.ruleset_order_id as RulesetOrderId,
                    // requirement.requirement_order as RequirementOrde...`.
                    .ToList());

        // Menyiapkan variabel lokal `orders` untuk nilai pesanan dengan mematerialisasi urutan `orderRows.Select(row => new RulesetOrderDto { Id = row.Id,
        // Nama = row.Nama, HargaJual = row.HargaJual, PoinKebahagiaan = row.PoinKebahagiaan, Bahan = orderRequirementLookup.Tr...` menjadi List; enumerasi
        // dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var orders = orderRows.Select(row => new RulesetOrderDto
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ReadRulesetDefinitionAsync.
        {
            // Memperbarui `Id` menggunakan `row.Id` (nilai identitas) dalam ReadRulesetDefinitionAsync.
            Id = row.Id,
            // Memperbarui `Nama` menggunakan `row.Nama` (nilai nama) dalam ReadRulesetDefinitionAsync.
            Nama = row.Nama,
            // Memperbarui `HargaJual` menggunakan `row.HargaJual` (nilai harga jual) dalam ReadRulesetDefinitionAsync.
            HargaJual = row.HargaJual,
            // Memperbarui `PoinKebahagiaan` menggunakan `row.PoinKebahagiaan` (nilai poin kebahagiaan) dalam ReadRulesetDefinitionAsync.
            PoinKebahagiaan = row.PoinKebahagiaan,
            // Memperbarui `Bahan` menggunakan hasil pemilihan bersyarat: ketika `orderRequirementLookup.TryGetValue(row.RulesetOrderId, out var requirements)`
            // benar gunakan `requirements`, jika tidak gunakan `[]` dalam ReadRulesetDefinitionAsync.
            Bahan = orderRequirementLookup.TryGetValue(row.RulesetOrderId, out var requirements)
                // Meneruskan fungsi lambda `row => new RulesetOrderDto { Id = row.Id, Nama = row.Nama, HargaJual = row.HargaJual, PoinKebahagiaan =
                // row.PoinKebahagiaan, Bahan = orderRequirementLookup.TryGetValue(row.Rul...` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan
                // sebagai argumen ke `orderRows.Select`.
                ? requirements
                // Meneruskan fungsi lambda `row => new RulesetOrderDto { Id = row.Id, Nama = row.Nama, HargaJual = row.HargaJual, PoinKebahagiaan =
                // row.PoinKebahagiaan, Bahan = orderRequirementLookup.TryGetValue(row.Rul...` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan
                // sebagai argumen ke `orderRows.Select`.
                : [],
            // Memperbarui `CardQty` menggunakan `row.CardQty` (nilai kartu qty) dalam ReadRulesetDefinitionAsync.
            CardQty = row.CardQty
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam ReadRulesetDefinitionAsync.
        }).ToList();

        // Menyiapkan variabel lokal `needs` untuk nilai kebutuhan dengan mematerialisasi urutan `(await conn.QueryAsync<RulesetNeedDto>( new
        // CommandDefinition( ””” select need_code as Id, item_name as Nama, need_family_code as Family, need_tier as Tipe, purchase_price as ...` menjadi
        // List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var needs = (await conn.QueryAsync<RulesetNeedDto>(
            // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen ( ””” select need_code as Id, item_name as Nama, need_family_code as Family,
            // need_tier as Tipe, purchase_price as HargaBeli, happiness_points as PoinKebahagiaan... sebagai argumen ke `conn.QueryAsync<RulesetNeedDto>`.
            new CommandDefinition(
                // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke konstruktor `CommandDefinition`.
                // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
                // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
                // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
                // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `need_code as Id,`.
                // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `item_name as Nama,`.
                // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `need_family_code as Family,`.
                // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `need_tier as Tipe,`.
                // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `purchase_price as
                // HargaBeli,`.
                // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `happiness_points as
                // PoinKebahagiaan,`.
                // Baris literal 9: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `card_qty as CardQty`.
                // Baris literal 10: FROM memilih tabel/subquery sumber pembacaan: `from ruleset_needs`.
                // Baris literal 11: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where ruleset_version_id = @rulesetVersionId`.
                // Baris literal 12: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and is_active`.
                // Baris literal 13: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by sort_order asc, need_code asc`.
                // Baris literal 14: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
                """
                select
                    need_code as Id,
                    item_name as Nama,
                    need_family_code as Family,
                    need_tier as Tipe,
                    purchase_price as HargaBeli,
                    happiness_points as PoinKebahagiaan,
                    card_qty as CardQty
                from ruleset_needs
                where ruleset_version_id = @rulesetVersionId
                  and is_active
                order by sort_order asc, need_code asc
                """,
                // Meneruskan objek anonim yang mengelompokkan rulesetVersionId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                new { rulesetVersionId },
                // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                // bernama `cancellationToken`.
                cancellationToken: ct))).ToList();

        // Menyiapkan variabel lokal `needSetBonuses` untuk nilai kebutuhan set bonuses dengan mematerialisasi urutan `(await
        // conn.QueryAsync<RulesetNeedSetBonusDto>( new CommandDefinition( ””” select pattern_code as PatternCode, required_count as RequiredCount, points
        // as Points from ruleset_n...` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai
        // awal.
        var needSetBonuses = (await conn.QueryAsync<RulesetNeedSetBonusDto>(
            // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen ( ””” select pattern_code as PatternCode, required_count as RequiredCount,
            // points as Points from ruleset_need_set_bonuses where ruleset_version_id = @rulesetVer... sebagai argumen ke
            // `conn.QueryAsync<RulesetNeedSetBonusDto>`.
            new CommandDefinition(
                // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke konstruktor `CommandDefinition`.
                // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
                // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
                // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
                // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `pattern_code as
                // PatternCode,`.
                // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `required_count as
                // RequiredCount,`.
                // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `points as Points`.
                // Baris literal 6: FROM memilih tabel/subquery sumber pembacaan: `from ruleset_need_set_bonuses`.
                // Baris literal 7: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where ruleset_version_id = @rulesetVersionId`.
                // Baris literal 8: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by sort_order asc, pattern_code asc`.
                // Baris literal 9: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
                """
                select
                    pattern_code as PatternCode,
                    required_count as RequiredCount,
                    points as Points
                from ruleset_need_set_bonuses
                where ruleset_version_id = @rulesetVersionId
                order by sort_order asc, pattern_code asc
                """,
                // Meneruskan objek anonim yang mengelompokkan rulesetVersionId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                new { rulesetVersionId },
                // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                // bernama `cancellationToken`.
                cancellationToken: ct))).ToList();

        // Menyiapkan variabel lokal `missionRows` untuk nilai misi baris dengan mematerialisasi urutan `(await conn.QueryAsync<CollectionMissionRow>( new
        // CommandDefinition( ””” select ruleset_collection_mission_id as RulesetCollectionMissionId, mission_code as Id, item_name as N...` menjadi List;
        // enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var missionRows = (await conn.QueryAsync<CollectionMissionRow>(
            // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen ( ””” select ruleset_collection_mission_id as RulesetCollectionMissionId,
            // mission_code as Id, item_name as Nama, success_points as SuccessPoints, failure_points... sebagai argumen ke
            // `conn.QueryAsync<CollectionMissionRow>`.
            new CommandDefinition(
                // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke konstruktor `CommandDefinition`.
                // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
                // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
                // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
                // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_collection_mission_id
                // as RulesetCollectionMissionId,`.
                // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `mission_code as Id,`.
                // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `item_name as Nama,`.
                // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `success_points as
                // SuccessPoints,`.
                // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `failure_points as
                // FailurePoints,`.
                // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `penalty_points as
                // PenaltyPoints,`.
                // Baris literal 9: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `sort_order as SortOrder`.
                // Baris literal 10: FROM memilih tabel/subquery sumber pembacaan: `from ruleset_collection_missions`.
                // Baris literal 11: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where ruleset_version_id = @rulesetVersionId`.
                // Baris literal 12: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and is_active`.
                // Baris literal 13: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by sort_order asc, mission_code asc`.
                // Baris literal 14: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
                """
                select
                    ruleset_collection_mission_id as RulesetCollectionMissionId,
                    mission_code as Id,
                    item_name as Nama,
                    success_points as SuccessPoints,
                    failure_points as FailurePoints,
                    penalty_points as PenaltyPoints,
                    sort_order as SortOrder
                from ruleset_collection_missions
                where ruleset_version_id = @rulesetVersionId
                  and is_active
                order by sort_order asc, mission_code asc
                """,
                // Meneruskan objek anonim yang mengelompokkan rulesetVersionId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                new { rulesetVersionId },
                // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                // bernama `cancellationToken`.
                cancellationToken: ct))).ToList();

        // Menyiapkan variabel lokal `missionRequirementLookup` untuk nilai misi requirement lookup dengan membangun kamus dari `(await
        // conn.QueryAsync<CollectionMissionRequirementRow>( new CommandDefinition( ””” select requirement.ruleset_collection_mission_id as
        // RulesetCollectionMissionId, requirement...` dengan pemilihan kunci/nilai `group => group.Key`, `group => group .OrderBy(item =>
        // item.RequirementOrder) .Select(item => new RulesetCollectionMissionRequirementDto { Order = item.RequirementOrder, Type = item.Type, Value =
        // it...`; kunci harus unik agar konversi berhasil. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var missionRequirementLookup = (await conn.QueryAsync<CollectionMissionRequirementRow>(
            // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen ( ””” select requirement.ruleset_collection_mission_id as
            // RulesetCollectionMissionId, requirement.requirement_order as RequirementOrder, case when requirement.r... sebagai argumen ke
            // `conn.QueryAsync<CollectionMissionRequirementRow>`.
            new CommandDefinition(
                // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke konstruktor `CommandDefinition`.
                // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
                // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
                // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
                // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun:
                // `requirement.ruleset_collection_mission_id as RulesetCollectionMissionId,`.
                // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `requirement.requirement_order
                // as RequirementOrder,`.
                // Baris literal 5: Menyusun pilihan nilai bersyarat di dalam ekspresi SQL: `case`.
                // Baris literal 6: Menyusun pilihan nilai bersyarat di dalam ekspresi SQL: `when requirement.requirement_type = 'NEED_TIER' then 'TIER'`.
                // Baris literal 7: Menyusun pilihan nilai bersyarat di dalam ekspresi SQL: `when requirement.requirement_type = 'NEED_FAMILY' then 'FAMILY'`.
                // Baris literal 8: Menyusun pilihan nilai bersyarat di dalam ekspresi SQL: `else 'NAME'`.
                // Baris literal 9: Menyusun pilihan nilai bersyarat di dalam ekspresi SQL: `end as Type,`.
                // Baris literal 10: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun:
                // `coalesce(requirement.required_need_tier, requirement.required_need_family_code, asset.asset_code) as Value`.
                // Baris literal 11: FROM memilih tabel/subquery sumber pembacaan: `from ruleset_collection_mission_requirements requirement`.
                // Baris literal 12: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `left join ruleset_game_assets asset`.
                // Baris literal 13: ON menetapkan syarat pencocokan baris pada relasi JOIN: `on asset.ruleset_game_asset_id = requirement.required_asset_id`.
                // Baris literal 14: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where requirement.ruleset_collection_mission_id in (`.
                // Baris literal 15: SELECT menentukan nilai atau kolom yang dikembalikan query: `select ruleset_collection_mission_id`.
                // Baris literal 16: FROM memilih tabel/subquery sumber pembacaan: `from ruleset_collection_missions`.
                // Baris literal 17: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where ruleset_version_id = @rulesetVersionId`.
                // Baris literal 18: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
                // Baris literal 19: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by requirement_order asc`.
                // Baris literal 20: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
                """
                select
                    requirement.ruleset_collection_mission_id as RulesetCollectionMissionId,
                    requirement.requirement_order as RequirementOrder,
                    case
                      when requirement.requirement_type = 'NEED_TIER' then 'TIER'
                      when requirement.requirement_type = 'NEED_FAMILY' then 'FAMILY'
                      else 'NAME'
                    end as Type,
                    coalesce(requirement.required_need_tier, requirement.required_need_family_code, asset.asset_code) as Value
                from ruleset_collection_mission_requirements requirement
                left join ruleset_game_assets asset
                  on asset.ruleset_game_asset_id = requirement.required_asset_id
                where requirement.ruleset_collection_mission_id in (
                    select ruleset_collection_mission_id
                    from ruleset_collection_missions
                    where ruleset_version_id = @rulesetVersionId
                )
                order by requirement_order asc
                """,
                // Meneruskan objek anonim yang mengelompokkan rulesetVersionId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                new { rulesetVersionId },
                // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                // bernama `cancellationToken`.
                cancellationToken: ct)))
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .GroupBy(row => row.RulesetCollectionMissionId) dalam
            // ReadRulesetDefinitionAsync; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .GroupBy(row => row.RulesetCollectionMissionId)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToDictionary( dalam ReadRulesetDefinitionAsync; token pada baris ini
            // menyambungkan bagian kode sebelum dan sesudahnya.
            .ToDictionary(
                // Parameter `group` bertipe `` membawa nilai group.
                group => group.Key,
                // Parameter `group` bertipe `` membawa nilai group.
                group => group
                    // Meneruskan fungsi lambda `item => item.RequirementOrder` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen ke
                    // `group .OrderBy`.
                    .OrderBy(item => item.RequirementOrder)
                    // Meneruskan fungsi lambda `item => new RulesetCollectionMissionRequirementDto { Order = item.RequirementOrder, Type = item.Type, Value =
                    // item.Value }` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen ke `group .OrderBy(item =>
                    // item.RequirementOrder) .Select`.
                    .Select(item => new RulesetCollectionMissionRequirementDto
                    // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                    // ReadRulesetDefinitionAsync.
                    {
                        // Memperbarui `Order` menggunakan `item.RequirementOrder` (nilai requirement urutan/pesanan) dalam ReadRulesetDefinitionAsync.
                        Order = item.RequirementOrder,
                        // Memperbarui `Type` menggunakan `item.Type` (nilai jenis) dalam ReadRulesetDefinitionAsync.
                        Type = item.Type,
                        // Memperbarui `Value` menggunakan `item.Value`, yaitu nilai yang dibungkus objek/nullable dalam ReadRulesetDefinitionAsync.
                        Value = item.Value
                    // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam ReadRulesetDefinitionAsync.
                    })
                    // Meneruskan fungsi lambda `group => group .OrderBy(item => item.RequirementOrder) .Select(item => new RulesetCollectionMissionRequirementDto {
                    // Order = item.RequirementOrder, Type = item.Type, Value = it...` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai
                    // argumen ke `(await conn.QueryAsync<CollectionMissionRequirementRow>( new CommandDefinition( ””” select requirement.ruleset_collection_mission_id
                    // as RulesetCollectionMissionId, requirement...`.
                    .ToList());

        // Menyiapkan variabel lokal `missions` untuk nilai misi dengan mematerialisasi urutan `missionRows.Select(row => new RulesetCollectionMissionDto {
        // Id = row.Id, Nama = row.Nama, SuccessPoints = row.SuccessPoints, FailurePoints = row.FailurePoints, PenaltyPoints =...` menjadi List; enumerasi
        // dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var missions = missionRows.Select(row => new RulesetCollectionMissionDto
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ReadRulesetDefinitionAsync.
        {
            // Memperbarui `Id` menggunakan `row.Id` (nilai identitas) dalam ReadRulesetDefinitionAsync.
            Id = row.Id,
            // Memperbarui `Nama` menggunakan `row.Nama` (nilai nama) dalam ReadRulesetDefinitionAsync.
            Nama = row.Nama,
            // Memperbarui `SuccessPoints` menggunakan `row.SuccessPoints` (nilai success poin) dalam ReadRulesetDefinitionAsync.
            SuccessPoints = row.SuccessPoints,
            // Memperbarui `FailurePoints` menggunakan `row.FailurePoints` (nilai failure poin) dalam ReadRulesetDefinitionAsync.
            FailurePoints = row.FailurePoints,
            // Memperbarui `PenaltyPoints` menggunakan `row.PenaltyPoints` (nilai penalti poin) dalam ReadRulesetDefinitionAsync.
            PenaltyPoints = row.PenaltyPoints,
            // Memperbarui `KebutuhanTarget` menggunakan hasil pemilihan bersyarat: ketika `missionRequirementLookup.TryGetValue(row.RulesetCollectionMissionId,
            // out var requirements)` benar gunakan `requirements`, jika tidak gunakan `[]` dalam ReadRulesetDefinitionAsync.
            KebutuhanTarget = missionRequirementLookup.TryGetValue(row.RulesetCollectionMissionId, out var requirements)
                // Meneruskan fungsi lambda `row => new RulesetCollectionMissionDto { Id = row.Id, Nama = row.Nama, SuccessPoints = row.SuccessPoints, FailurePoints
                // = row.FailurePoints, PenaltyPoints = row.PenaltyPoints,...` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen
                // ke `missionRows.Select`.
                ? requirements
                // Meneruskan fungsi lambda `row => new RulesetCollectionMissionDto { Id = row.Id, Nama = row.Nama, SuccessPoints = row.SuccessPoints, FailurePoints
                // = row.FailurePoints, PenaltyPoints = row.PenaltyPoints,...` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen
                // ke `missionRows.Select`.
                : []
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam ReadRulesetDefinitionAsync.
        }).ToList();

        // Menyiapkan variabel lokal `financialGoals` untuk nilai keuangan target dengan mematerialisasi urutan `(await
        // conn.QueryAsync<RulesetFinancialGoalDto>( new CommandDefinition( ””” select goal_code as Id, item_name as Nama, purchase_price as HargaBeli,
        // happiness_points as PoinKeb...` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi
        // nilai awal.
        var financialGoals = (await conn.QueryAsync<RulesetFinancialGoalDto>(
            // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen ( ””” select goal_code as Id, item_name as Nama, purchase_price as HargaBeli,
            // happiness_points as PoinKebahagiaan, card_qty as CardQty from ruleset_financial_go... sebagai argumen ke
            // `conn.QueryAsync<RulesetFinancialGoalDto>`.
            new CommandDefinition(
                // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke konstruktor `CommandDefinition`.
                // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
                // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
                // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
                // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `goal_code as Id,`.
                // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `item_name as Nama,`.
                // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `purchase_price as
                // HargaBeli,`.
                // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `happiness_points as
                // PoinKebahagiaan,`.
                // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `card_qty as CardQty`.
                // Baris literal 8: FROM memilih tabel/subquery sumber pembacaan: `from ruleset_financial_goals`.
                // Baris literal 9: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where ruleset_version_id = @rulesetVersionId`.
                // Baris literal 10: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and is_active`.
                // Baris literal 11: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by sort_order asc, goal_code asc`.
                // Baris literal 12: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
                """
                select
                    goal_code as Id,
                    item_name as Nama,
                    purchase_price as HargaBeli,
                    happiness_points as PoinKebahagiaan,
                    card_qty as CardQty
                from ruleset_financial_goals
                where ruleset_version_id = @rulesetVersionId
                  and is_active
                order by sort_order asc, goal_code asc
                """,
                // Meneruskan objek anonim yang mengelompokkan rulesetVersionId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                new { rulesetVersionId },
                // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                // bernama `cancellationToken`.
                cancellationToken: ct))).ToList();

        // Menyiapkan variabel lokal `narrativeRows` untuk nilai narrative baris dengan mematerialisasi urutan `(await conn.QueryAsync<NarrativeRow>( new
        // CommandDefinition( ””” select rn.ruleset_narrative_id as RulesetNarrativeId, rn.narrative_code as Id, rn.item_name as Nama, coalesce(...` menjadi
        // List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var narrativeRows = (await conn.QueryAsync<NarrativeRow>(
            // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen ( ””” select rn.ruleset_narrative_id as RulesetNarrativeId, rn.narrative_code as
            // Id, rn.item_name as Nama, coalesce(scene.text_lines::text, '[]') as TextLinesJs... sebagai argumen ke `conn.QueryAsync<NarrativeRow>`.
            new CommandDefinition(
                // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke konstruktor `CommandDefinition`.
                // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
                // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
                // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
                // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `rn.ruleset_narrative_id as
                // RulesetNarrativeId,`.
                // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `rn.narrative_code as Id,`.
                // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `rn.item_name as Nama,`.
                // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun:
                // `coalesce(scene.text_lines::text, '[]') as TextLinesJson,`.
                // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `rn.sort_order as SortOrder`.
                // Baris literal 8: FROM memilih tabel/subquery sumber pembacaan: `from ruleset_narratives rn`.
                // Baris literal 9: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `left join lateral (`.
                // Baris literal 10: SELECT menentukan nilai atau kolom yang dikembalikan query: `select rns.text_lines`.
                // Baris literal 11: FROM memilih tabel/subquery sumber pembacaan: `from ruleset_narrative_scenes rns`.
                // Baris literal 12: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where rns.ruleset_version_id = rn.ruleset_version_id`.
                // Baris literal 13: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and rns.ruleset_narrative_id = rn.ruleset_narrative_id`.
                // Baris literal 14: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by rns.scene_order asc`.
                // Baris literal 15: LIMIT membatasi jumlah baris yang dikembalikan query: `limit 1`.
                // Baris literal 16: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `) scene on true`.
                // Baris literal 17: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where rn.ruleset_version_id = @rulesetVersionId`.
                // Baris literal 18: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and rn.is_active`.
                // Baris literal 19: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by rn.sort_order asc, rn.narrative_code
                // asc`.
                // Baris literal 20: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
                """
                select
                    rn.ruleset_narrative_id as RulesetNarrativeId,
                    rn.narrative_code as Id,
                    rn.item_name as Nama,
                    coalesce(scene.text_lines::text, '[]') as TextLinesJson,
                    rn.sort_order as SortOrder
                from ruleset_narratives rn
                left join lateral (
                    select rns.text_lines
                    from ruleset_narrative_scenes rns
                    where rns.ruleset_version_id = rn.ruleset_version_id
                      and rns.ruleset_narrative_id = rn.ruleset_narrative_id
                    order by rns.scene_order asc
                    limit 1
                ) scene on true
                where rn.ruleset_version_id = @rulesetVersionId
                  and rn.is_active
                order by rn.sort_order asc, rn.narrative_code asc
                """,
                // Meneruskan objek anonim yang mengelompokkan rulesetVersionId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                new { rulesetVersionId },
                // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                // bernama `cancellationToken`.
                cancellationToken: ct))).ToList();

        // Menyiapkan variabel lokal `prerequisiteLookup` untuk nilai prerequisite lookup dengan membangun kamus dari `(await
        // conn.QueryAsync<NarrativePrerequisiteRow>( new CommandDefinition( ””” select rtc.ruleset_narrative_id as RulesetNarrativeId, ra.action_id as
        // Aksi, rtc.threshold_numeric...` dengan pemilihan kunci/nilai `group => group.Key`, `group => group .OrderBy(item => item.SortOrder) .Select(item
        // => new RulesetNarrativePrerequisiteDto { Aksi = item.Aksi, Value = item.Value }) .ToList()`; kunci harus unik agar konversi berhasil. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var prerequisiteLookup = (await conn.QueryAsync<NarrativePrerequisiteRow>(
            // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen ( ””” select rtc.ruleset_narrative_id as RulesetNarrativeId, ra.action_id as
            // Aksi, rtc.threshold_numeric as Value, rtc.sort_order as SortOrder from ruleset_trig... sebagai argumen ke
            // `conn.QueryAsync<NarrativePrerequisiteRow>`.
            new CommandDefinition(
                // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke konstruktor `CommandDefinition`.
                // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
                // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
                // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
                // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `rtc.ruleset_narrative_id as
                // RulesetNarrativeId,`.
                // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ra.action_id as Aksi,`.
                // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `rtc.threshold_numeric as
                // Value,`.
                // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `rtc.sort_order as SortOrder`.
                // Baris literal 7: FROM memilih tabel/subquery sumber pembacaan: `from ruleset_trigger_conditions rtc`.
                // Baris literal 8: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join ruleset_actions ra`.
                // Baris literal 9: ON menetapkan syarat pencocokan baris pada relasi JOIN: `on ra.ruleset_version_id = rtc.ruleset_version_id`.
                // Baris literal 10: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and ra.ruleset_action_id = rtc.ruleset_action_id`.
                // Baris literal 11: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where rtc.trigger_owner_type = 'NARRATIVE'`.
                // Baris literal 12: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and rtc.is_active`.
                // Baris literal 13: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and rtc.ruleset_narrative_id in (`.
                // Baris literal 14: SELECT menentukan nilai atau kolom yang dikembalikan query: `select ruleset_narrative_id`.
                // Baris literal 15: FROM memilih tabel/subquery sumber pembacaan: `from ruleset_narratives`.
                // Baris literal 16: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where ruleset_version_id = @rulesetVersionId`.
                // Baris literal 17: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
                // Baris literal 18: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by rtc.sort_order asc`.
                // Baris literal 19: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
                """
                select
                    rtc.ruleset_narrative_id as RulesetNarrativeId,
                    ra.action_id as Aksi,
                    rtc.threshold_numeric as Value,
                    rtc.sort_order as SortOrder
                from ruleset_trigger_conditions rtc
                join ruleset_actions ra
                  on ra.ruleset_version_id = rtc.ruleset_version_id
                 and ra.ruleset_action_id = rtc.ruleset_action_id
                where rtc.trigger_owner_type = 'NARRATIVE'
                  and rtc.is_active
                  and rtc.ruleset_narrative_id in (
                    select ruleset_narrative_id
                    from ruleset_narratives
                    where ruleset_version_id = @rulesetVersionId
                )
                order by rtc.sort_order asc
                """,
                // Meneruskan objek anonim yang mengelompokkan rulesetVersionId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                new { rulesetVersionId },
                // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                // bernama `cancellationToken`.
                cancellationToken: ct)))
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .GroupBy(row => row.RulesetNarrativeId) dalam ReadRulesetDefinitionAsync; token
            // pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .GroupBy(row => row.RulesetNarrativeId)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToDictionary( dalam ReadRulesetDefinitionAsync; token pada baris ini
            // menyambungkan bagian kode sebelum dan sesudahnya.
            .ToDictionary(
                // Parameter `group` bertipe `` membawa nilai group.
                group => group.Key,
                // Parameter `group` bertipe `` membawa nilai group.
                group => group
                    // Meneruskan fungsi lambda `item => item.SortOrder` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen ke `group
                    // .OrderBy`.
                    .OrderBy(item => item.SortOrder)
                    // Meneruskan fungsi lambda `item => new RulesetNarrativePrerequisiteDto { Aksi = item.Aksi, Value = item.Value }` yang dijalankan oleh operasi
                    // pemanggil untuk memproses setiap masukan sebagai argumen ke `group .OrderBy(item => item.SortOrder) .Select`.
                    .Select(item => new RulesetNarrativePrerequisiteDto
                    // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                    // ReadRulesetDefinitionAsync.
                    {
                        // Memperbarui `Aksi` menggunakan `item.Aksi` (nilai aksi) dalam ReadRulesetDefinitionAsync.
                        Aksi = item.Aksi,
                        // Memperbarui `Value` menggunakan `item.Value`, yaitu nilai yang dibungkus objek/nullable dalam ReadRulesetDefinitionAsync.
                        Value = item.Value
                    // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam ReadRulesetDefinitionAsync.
                    })
                    // Meneruskan fungsi lambda `group => group .OrderBy(item => item.SortOrder) .Select(item => new RulesetNarrativePrerequisiteDto { Aksi = item.Aksi,
                    // Value = item.Value }) .ToList()` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen ke `(await
                    // conn.QueryAsync<NarrativePrerequisiteRow>( new CommandDefinition( ””” select rtc.ruleset_narrative_id as RulesetNarrativeId, ra.action_id as
                    // Aksi, rtc.threshold_numeric...`.
                    .ToList());

        // Menyiapkan variabel lokal `narratives` untuk nilai narratives dengan mematerialisasi urutan `narrativeRows.Select(row => new RulesetNarrativeDto
        // { Id = row.Id, Nama = row.Nama, Teks = ParseStringArray(row.TextLinesJson), PrerequisiteAksi = prerequisiteLookup.TryGetVal...` menjadi List;
        // enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var narratives = narrativeRows.Select(row => new RulesetNarrativeDto
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ReadRulesetDefinitionAsync.
        {
            // Memperbarui `Id` menggunakan `row.Id` (nilai identitas) dalam ReadRulesetDefinitionAsync.
            Id = row.Id,
            // Memperbarui `Nama` menggunakan `row.Nama` (nilai nama) dalam ReadRulesetDefinitionAsync.
            Nama = row.Nama,
            // Memperbarui `Teks` menggunakan memanggil `ParseStringArray` dengan `row.TextLinesJson` dalam ReadRulesetDefinitionAsync.
            Teks = ParseStringArray(row.TextLinesJson),
            // Memperbarui `PrerequisiteAksi` menggunakan hasil pemilihan bersyarat: ketika `prerequisiteLookup.TryGetValue(row.RulesetNarrativeId, out var
            // prerequisites)` benar gunakan `prerequisites`, jika tidak gunakan `[]` dalam ReadRulesetDefinitionAsync.
            PrerequisiteAksi = prerequisiteLookup.TryGetValue(row.RulesetNarrativeId, out var prerequisites)
                // Meneruskan fungsi lambda `row => new RulesetNarrativeDto { Id = row.Id, Nama = row.Nama, Teks = ParseStringArray(row.TextLinesJson),
                // PrerequisiteAksi = prerequisiteLookup.TryGetValue(row.RulesetNarrati...` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan
                // sebagai argumen ke `narrativeRows.Select`.
                ? prerequisites
                // Meneruskan fungsi lambda `row => new RulesetNarrativeDto { Id = row.Id, Nama = row.Nama, Teks = ParseStringArray(row.TextLinesJson),
                // PrerequisiteAksi = prerequisiteLookup.TryGetValue(row.RulesetNarrati...` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan
                // sebagai argumen ke `narrativeRows.Select`.
                : []
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam ReadRulesetDefinitionAsync.
        }).ToList();

        // Menyiapkan variabel lokal `donationRankPoints` untuk nilai donasi rank poin dengan mematerialisasi urutan `(await
        // conn.QueryAsync<RulesetDonationRankPointDto>( new CommandDefinition( ””” select rank_no as Rank, points as Points from ruleset_rank_points where
        // ruleset_version_id = @r...` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai
        // awal.
        var donationRankPoints = (await conn.QueryAsync<RulesetDonationRankPointDto>(
            // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen ( ””” select rank_no as Rank, points as Points from ruleset_rank_points where
            // ruleset_version_id = @rulesetVersionId and rank_type = 'DONATION' order by sort_or... sebagai argumen ke
            // `conn.QueryAsync<RulesetDonationRankPointDto>`.
            new CommandDefinition(
                // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke konstruktor `CommandDefinition`.
                // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
                // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
                // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select rank_no as Rank, points as Points`.
                // Baris literal 3: FROM memilih tabel/subquery sumber pembacaan: `from ruleset_rank_points`.
                // Baris literal 4: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where ruleset_version_id = @rulesetVersionId`.
                // Baris literal 5: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and rank_type = 'DONATION'`.
                // Baris literal 6: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by sort_order asc, rank_no asc`.
                // Baris literal 7: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
                """
                select rank_no as Rank, points as Points
                from ruleset_rank_points
                where ruleset_version_id = @rulesetVersionId
                  and rank_type = 'DONATION'
                order by sort_order asc, rank_no asc
                """,
                // Meneruskan objek anonim yang mengelompokkan rulesetVersionId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                new { rulesetVersionId },
                // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                // bernama `cancellationToken`.
                cancellationToken: ct))).ToList();

        // Menyiapkan variabel lokal `goldPointRows` untuk nilai emas point baris dengan mematerialisasi urutan `(await
        // conn.QueryAsync<RulesetGoldPointDto>( new CommandDefinition( ””” select quantity as Qty, points as Points from ruleset_gold_assets where
        // ruleset_version_id = @rulesetVe...` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var goldPointRows = (await conn.QueryAsync<RulesetGoldPointDto>(
            // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen ( ””” select quantity as Qty, points as Points from ruleset_gold_assets where
            // ruleset_version_id = @rulesetVersionId and is_active order by sort_order asc, quan... sebagai argumen ke `conn.QueryAsync<RulesetGoldPointDto>`.
            new CommandDefinition(
                // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke konstruktor `CommandDefinition`.
                // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
                // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
                // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select quantity as Qty, points as Points`.
                // Baris literal 3: FROM memilih tabel/subquery sumber pembacaan: `from ruleset_gold_assets`.
                // Baris literal 4: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where ruleset_version_id = @rulesetVersionId`.
                // Baris literal 5: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and is_active`.
                // Baris literal 6: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by sort_order asc, quantity asc`.
                // Baris literal 7: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
                """
                select quantity as Qty, points as Points
                from ruleset_gold_assets
                where ruleset_version_id = @rulesetVersionId
                  and is_active
                order by sort_order asc, quantity asc
                """,
                // Meneruskan objek anonim yang mengelompokkan rulesetVersionId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                new { rulesetVersionId },
                // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                // bernama `cancellationToken`.
                cancellationToken: ct))).ToList();

        // Menyiapkan variabel lokal `pensionRankPoints` untuk nilai pension rank poin dengan mematerialisasi urutan `(await
        // conn.QueryAsync<RulesetPensionRankPointDto>( new CommandDefinition( ””” select rank_no as Rank, points as Points from ruleset_rank_points where
        // ruleset_version_id = @ru...` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai
        // awal.
        var pensionRankPoints = (await conn.QueryAsync<RulesetPensionRankPointDto>(
            // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen ( ””” select rank_no as Rank, points as Points from ruleset_rank_points where
            // ruleset_version_id = @rulesetVersionId and rank_type = 'PENSION' order by sort_ord... sebagai argumen ke
            // `conn.QueryAsync<RulesetPensionRankPointDto>`.
            new CommandDefinition(
                // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke konstruktor `CommandDefinition`.
                // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
                // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
                // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select rank_no as Rank, points as Points`.
                // Baris literal 3: FROM memilih tabel/subquery sumber pembacaan: `from ruleset_rank_points`.
                // Baris literal 4: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where ruleset_version_id = @rulesetVersionId`.
                // Baris literal 5: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and rank_type = 'PENSION'`.
                // Baris literal 6: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by sort_order asc, rank_no asc`.
                // Baris literal 7: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
                """
                select rank_no as Rank, points as Points
                from ruleset_rank_points
                where ruleset_version_id = @rulesetVersionId
                  and rank_type = 'PENSION'
                order by sort_order asc, rank_no asc
                """,
                // Meneruskan objek anonim yang mengelompokkan rulesetVersionId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                new { rulesetVersionId },
                // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                // bernama `cancellationToken`.
                cancellationToken: ct))).ToList();

        // Menyiapkan variabel lokal `goldPrices` untuk nilai emas prices dengan mematerialisasi urutan `(await conn.QueryAsync<RulesetGoldPriceDto>( new
        // CommandDefinition( ””” select price_code as PriceCode, quantity as Qty, unit_price as UnitPrice, card_qty as CardQty from rule...` menjadi List;
        // enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var goldPrices = (await conn.QueryAsync<RulesetGoldPriceDto>(
            // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen ( ””” select price_code as PriceCode, quantity as Qty, unit_price as UnitPrice,
            // card_qty as CardQty from ruleset_gold_prices where ruleset_version_id = @ruleset... sebagai argumen ke `conn.QueryAsync<RulesetGoldPriceDto>`.
            new CommandDefinition(
                // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke konstruktor `CommandDefinition`.
                // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
                // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
                // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
                // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `price_code as PriceCode,`.
                // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `quantity as Qty,`.
                // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `unit_price as UnitPrice,`.
                // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `card_qty as CardQty`.
                // Baris literal 7: FROM memilih tabel/subquery sumber pembacaan: `from ruleset_gold_prices`.
                // Baris literal 8: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where ruleset_version_id = @rulesetVersionId`.
                // Baris literal 9: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and is_active`.
                // Baris literal 10: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by sort_order asc, price_code asc`.
                // Baris literal 11: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
                """
                select
                    price_code as PriceCode,
                    quantity as Qty,
                    unit_price as UnitPrice,
                    card_qty as CardQty
                from ruleset_gold_prices
                where ruleset_version_id = @rulesetVersionId
                  and is_active
                order by sort_order asc, price_code asc
                """,
                // Meneruskan objek anonim yang mengelompokkan rulesetVersionId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                new { rulesetVersionId },
                // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                // bernama `cancellationToken`.
                cancellationToken: ct))).ToList();

        // Menyiapkan variabel lokal `tieBreakers` untuk nilai tie breakers dengan mematerialisasi urutan `(await conn.QueryAsync<RulesetTieBreakerDto>( new
        // CommandDefinition( ””” select tie_breaker_code as TieBreakerCode, tie_number as TieNumber, card_qty as CardQty from ruleset_t...` menjadi List;
        // enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var tieBreakers = (await conn.QueryAsync<RulesetTieBreakerDto>(
            // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen ( ””” select tie_breaker_code as TieBreakerCode, tie_number as TieNumber,
            // card_qty as CardQty from ruleset_tie_breakers where ruleset_version_id = @rulesetVersi... sebagai argumen ke
            // `conn.QueryAsync<RulesetTieBreakerDto>`.
            new CommandDefinition(
                // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke konstruktor `CommandDefinition`.
                // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
                // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
                // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
                // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `tie_breaker_code as
                // TieBreakerCode,`.
                // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `tie_number as TieNumber,`.
                // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `card_qty as CardQty`.
                // Baris literal 6: FROM memilih tabel/subquery sumber pembacaan: `from ruleset_tie_breakers`.
                // Baris literal 7: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where ruleset_version_id = @rulesetVersionId`.
                // Baris literal 8: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by sort_order asc, tie_number asc`.
                // Baris literal 9: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
                """
                select
                    tie_breaker_code as TieBreakerCode,
                    tie_number as TieNumber,
                    card_qty as CardQty
                from ruleset_tie_breakers
                where ruleset_version_id = @rulesetVersionId
                order by sort_order asc, tie_number asc
                """,
                // Meneruskan objek anonim yang mengelompokkan rulesetVersionId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                new { rulesetVersionId },
                // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                // bernama `cancellationToken`.
                cancellationToken: ct))).ToList();

        // Menyiapkan variabel lokal `shariaLoans` untuk nilai sharia pinjaman dengan mematerialisasi urutan `(await conn.QueryAsync<RulesetShariaLoanDto>(
        // new CommandDefinition( ””” select loan_code as LoanCode, item_name as ItemName, principal as Principal, repayment_amount as Repay...` menjadi
        // List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var shariaLoans = (await conn.QueryAsync<RulesetShariaLoanDto>(
            // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen ( ””” select loan_code as LoanCode, item_name as ItemName, principal as
            // Principal, repayment_amount as RepaymentAmount, duration_days as DurationDays, penalty_p... sebagai argumen ke
            // `conn.QueryAsync<RulesetShariaLoanDto>`.
            new CommandDefinition(
                // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke konstruktor `CommandDefinition`.
                // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
                // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
                // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
                // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `loan_code as LoanCode,`.
                // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `item_name as ItemName,`.
                // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `principal as Principal,`.
                // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `repayment_amount as
                // RepaymentAmount,`.
                // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `duration_days as
                // DurationDays,`.
                // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `penalty_points as
                // PenaltyPoints,`.
                // Baris literal 9: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `card_qty as CardQty`.
                // Baris literal 10: FROM memilih tabel/subquery sumber pembacaan: `from ruleset_sharia_loans`.
                // Baris literal 11: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where ruleset_version_id = @rulesetVersionId`.
                // Baris literal 12: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by sort_order asc, loan_code asc`.
                // Baris literal 13: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
                """
                select
                    loan_code as LoanCode,
                    item_name as ItemName,
                    principal as Principal,
                    repayment_amount as RepaymentAmount,
                    duration_days as DurationDays,
                    penalty_points as PenaltyPoints,
                    card_qty as CardQty
                from ruleset_sharia_loans
                where ruleset_version_id = @rulesetVersionId
                order by sort_order asc, loan_code asc
                """,
                // Meneruskan objek anonim yang mengelompokkan rulesetVersionId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                new { rulesetVersionId },
                // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                // bernama `cancellationToken`.
                cancellationToken: ct))).ToList();

        // Menyiapkan variabel lokal `insuranceProducts` untuk nilai asuransi products dengan mematerialisasi urutan `(await
        // conn.QueryAsync<RulesetInsuranceProductDto>( new CommandDefinition( ””” select product_code as ProductCode, item_name as ItemName, premium as
        // Premium, usage_limit as Us...` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi
        // nilai awal.
        var insuranceProducts = (await conn.QueryAsync<RulesetInsuranceProductDto>(
            // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen ( ””” select product_code as ProductCode, item_name as ItemName, premium as
            // Premium, usage_limit as UsageLimit, card_qty as CardQty from ruleset_insurance_produ... sebagai argumen ke
            // `conn.QueryAsync<RulesetInsuranceProductDto>`.
            new CommandDefinition(
                // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke konstruktor `CommandDefinition`.
                // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
                // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
                // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
                // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `product_code as
                // ProductCode,`.
                // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `item_name as ItemName,`.
                // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `premium as Premium,`.
                // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `usage_limit as UsageLimit,`.
                // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `card_qty as CardQty`.
                // Baris literal 8: FROM memilih tabel/subquery sumber pembacaan: `from ruleset_insurance_products`.
                // Baris literal 9: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where ruleset_version_id = @rulesetVersionId`.
                // Baris literal 10: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by sort_order asc, product_code asc`.
                // Baris literal 11: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
                """
                select
                    product_code as ProductCode,
                    item_name as ItemName,
                    premium as Premium,
                    usage_limit as UsageLimit,
                    card_qty as CardQty
                from ruleset_insurance_products
                where ruleset_version_id = @rulesetVersionId
                order by sort_order asc, product_code asc
                """,
                // Meneruskan objek anonim yang mengelompokkan rulesetVersionId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                new { rulesetVersionId },
                // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                // bernama `cancellationToken`.
                cancellationToken: ct))).ToList();

        // Menyiapkan variabel lokal `lifeRisks` untuk nilai life risks dengan mematerialisasi urutan `(await conn.QueryAsync<RulesetLifeRiskDto>( new
        // CommandDefinition( ””” select risk_code as RiskCode, item_name as ItemName, effect_type as EffectType, coalesce(direction, '') ...` menjadi List;
        // enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var lifeRisks = (await conn.QueryAsync<RulesetLifeRiskDto>(
            // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen ( ””” select risk_code as RiskCode, item_name as ItemName, effect_type as
            // EffectType, coalesce(direction, '') as Direction, amount as Amount, target_scope as Ta... sebagai argumen ke
            // `conn.QueryAsync<RulesetLifeRiskDto>`.
            new CommandDefinition(
                // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke konstruktor `CommandDefinition`.
                // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
                // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
                // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
                // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `risk_code as RiskCode,`.
                // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `item_name as ItemName,`.
                // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `effect_type as EffectType,`.
                // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `coalesce(direction, '') as
                // Direction,`.
                // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `amount as Amount,`.
                // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `target_scope as
                // TargetScope,`.
                // Baris literal 9: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `nullif(payload_json ->>
                // 'value_delta', '')::int as ValueDelta,`.
                // Baris literal 10: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `duration_days as
                // DurationDays,`.
                // Baris literal 11: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `card_qty as CardQty`.
                // Baris literal 12: FROM memilih tabel/subquery sumber pembacaan: `from ruleset_life_risks`.
                // Baris literal 13: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where ruleset_version_id = @rulesetVersionId`.
                // Baris literal 14: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by sort_order asc, risk_code asc`.
                // Baris literal 15: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
                """
                select
                    risk_code as RiskCode,
                    item_name as ItemName,
                    effect_type as EffectType,
                    coalesce(direction, '') as Direction,
                    amount as Amount,
                    target_scope as TargetScope,
                    nullif(payload_json ->> 'value_delta', '')::int as ValueDelta,
                    duration_days as DurationDays,
                    card_qty as CardQty
                from ruleset_life_risks
                where ruleset_version_id = @rulesetVersionId
                order by sort_order asc, risk_code asc
                """,
                // Meneruskan objek anonim yang mengelompokkan rulesetVersionId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                new { rulesetVersionId },
                // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                // bernama `cancellationToken`.
                cancellationToken: ct))).ToList();

        // Mengembalikan objek baru bertipe `RulesetDefinitionDto` dengan nilai awal sesuai konstruktornya kepada pemanggil dalam
        // ReadRulesetDefinitionAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return new RulesetDefinitionDto
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ReadRulesetDefinitionAsync.
        {
            // Memperbarui `Mode` menggunakan `mode` (mode permainan yang menentukan kelompok aturan yang digunakan) dalam ReadRulesetDefinitionAsync.
            Mode = mode,
            // Memperbarui `Settings` menggunakan objek baru bertipe `RulesetSettingsDto` dengan nilai awal sesuai konstruktornya dalam
            // ReadRulesetDefinitionAsync.
            Settings = new RulesetSettingsDto
            // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ReadRulesetDefinitionAsync.
            {
                // Memperbarui `ActionsPerTurn` menggunakan `settings.ActionsPerTurn` (nilai aksi per giliran) dalam ReadRulesetDefinitionAsync.
                ActionsPerTurn = settings.ActionsPerTurn,
                // Memperbarui `StartingCash` menggunakan `settings.StartingCash` (nilai starting uang tunai) dalam ReadRulesetDefinitionAsync.
                StartingCash = settings.StartingCash,
                // Memperbarui `InitialCoins` menggunakan `settings.StartingCash` (nilai starting uang tunai) dalam ReadRulesetDefinitionAsync.
                InitialCoins = settings.StartingCash,
                // Memperbarui `InitialHappiness` menggunakan `settings.InitialHappiness` (nilai awal kebahagiaan) dalam ReadRulesetDefinitionAsync.
                InitialHappiness = settings.InitialHappiness,
                // Memperbarui `InitialSaving` menggunakan `settings.InitialSaving` (nilai awal tabungan) dalam ReadRulesetDefinitionAsync.
                InitialSaving = settings.InitialSaving,
                // Memperbarui `FinishDay` menggunakan `settings.FinishDay` (nilai finish hari) dalam ReadRulesetDefinitionAsync.
                FinishDay = settings.FinishDay,
                // Memperbarui `MinPlayers` menggunakan `settings.MinPlayers` (nilai minimum pemain) dalam ReadRulesetDefinitionAsync.
                MinPlayers = settings.MinPlayers,
                // Memperbarui `MaxPlayers` menggunakan `settings.MaxPlayers` (nilai maksimum pemain) dalam ReadRulesetDefinitionAsync.
                MaxPlayers = settings.MaxPlayers,
                // Memperbarui `CashMin` menggunakan `settings.CashMin` (nilai uang tunai minimum) dalam ReadRulesetDefinitionAsync.
                CashMin = settings.CashMin,
                // Memperbarui `MaxIngredientTotal` menggunakan `settings.MaxIngredientTotal` (nilai maksimum bahan total) dalam ReadRulesetDefinitionAsync.
                MaxIngredientTotal = settings.MaxIngredientTotal,
                // Memperbarui `MaxSameIngredient` menggunakan `settings.MaxSameIngredient` (nilai maksimum same bahan) dalam ReadRulesetDefinitionAsync.
                MaxSameIngredient = settings.MaxSameIngredient,
                // Memperbarui `PrimaryNeedMaxPerDay` menggunakan `settings.PrimaryNeedMaxPerDay` (nilai primary kebutuhan maksimum per hari) dalam
                // ReadRulesetDefinitionAsync.
                PrimaryNeedMaxPerDay = settings.PrimaryNeedMaxPerDay,
                // Memperbarui `RequirePrimaryBeforeOthers` menggunakan `settings.RequirePrimaryBeforeOthers` (nilai require primary before others) dalam
                // ReadRulesetDefinitionAsync.
                RequirePrimaryBeforeOthers = settings.RequirePrimaryBeforeOthers,
                // Memperbarui `DonationMinAmount` menggunakan `settings.DonationMinAmount` (nilai donasi minimum nominal) dalam ReadRulesetDefinitionAsync.
                DonationMinAmount = settings.DonationMinAmount,
                // Memperbarui `DonationMaxAmount` menggunakan `settings.DonationMaxAmount` (nilai donasi maksimum nominal) dalam ReadRulesetDefinitionAsync.
                DonationMaxAmount = settings.DonationMaxAmount,
                // Memperbarui `GoldTradeAllowBuy` menggunakan `settings.GoldTradeAllowBuy` (nilai emas trade allow buy) dalam ReadRulesetDefinitionAsync.
                GoldTradeAllowBuy = settings.GoldTradeAllowBuy,
                // Memperbarui `GoldTradeAllowSell` menggunakan `settings.GoldTradeAllowSell` (nilai emas trade allow sell) dalam ReadRulesetDefinitionAsync.
                GoldTradeAllowSell = settings.GoldTradeAllowSell,
                // Memperbarui `LoanEnabled` menggunakan `settings.LoanEnabled` (nilai pinjaman enabled) dalam ReadRulesetDefinitionAsync.
                LoanEnabled = settings.LoanEnabled,
                // Memperbarui `InsuranceEnabled` menggunakan `settings.InsuranceEnabled` (nilai asuransi enabled) dalam ReadRulesetDefinitionAsync.
                InsuranceEnabled = settings.InsuranceEnabled,
                // Memperbarui `SavingGoalEnabled` menggunakan `settings.SavingGoalEnabled` (nilai tabungan target enabled) dalam ReadRulesetDefinitionAsync.
                SavingGoalEnabled = settings.SavingGoalEnabled,
                // Memperbarui `FreelanceIncome` menggunakan `settings.FreelanceIncome` (nilai freelance pemasukan) dalam ReadRulesetDefinitionAsync.
                FreelanceIncome = settings.FreelanceIncome
            // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam ReadRulesetDefinitionAsync.
            },
            // Memperbarui `PlayerOrdering` menggunakan memanggil `BuildPlayerOrdering` dengan `orderingRules` dalam ReadRulesetDefinitionAsync.
            PlayerOrdering = BuildPlayerOrdering(orderingRules),
            // Memperbarui `Actions` menggunakan `actions` (nilai aksi) dalam ReadRulesetDefinitionAsync.
            Actions = actions,
            // Memperbarui `Ingredients` menggunakan `ingredients` (nilai bahan) dalam ReadRulesetDefinitionAsync.
            Ingredients = ingredients,
            // Memperbarui `Orders` menggunakan `orders` (nilai pesanan) dalam ReadRulesetDefinitionAsync.
            Orders = orders,
            // Memperbarui `Needs` menggunakan `needs` (nilai kebutuhan) dalam ReadRulesetDefinitionAsync.
            Needs = needs,
            // Memperbarui `NeedSetBonuses` menggunakan `needSetBonuses` (nilai kebutuhan set bonuses) dalam ReadRulesetDefinitionAsync.
            NeedSetBonuses = needSetBonuses,
            // Memperbarui `CollectionMissions` menggunakan `missions` (nilai misi) dalam ReadRulesetDefinitionAsync.
            CollectionMissions = missions,
            // Memperbarui `FinancialGoals` menggunakan `financialGoals` (nilai keuangan target) dalam ReadRulesetDefinitionAsync.
            FinancialGoals = financialGoals,
            // Memperbarui `Narratives` menggunakan `narratives` (nilai narratives) dalam ReadRulesetDefinitionAsync.
            Narratives = narratives,
            // Memperbarui `DonationRankPoints` menggunakan `donationRankPoints` (nilai donasi rank poin) dalam ReadRulesetDefinitionAsync.
            DonationRankPoints = donationRankPoints,
            // Memperbarui `GoldPointsByQty` menggunakan `goldPointRows` (nilai emas point baris) dalam ReadRulesetDefinitionAsync.
            GoldPointsByQty = goldPointRows,
            // Memperbarui `GoldPrices` menggunakan `goldPrices` (nilai emas prices) dalam ReadRulesetDefinitionAsync.
            GoldPrices = goldPrices,
            // Memperbarui `PensionRankPoints` menggunakan `pensionRankPoints` (nilai pension rank poin) dalam ReadRulesetDefinitionAsync.
            PensionRankPoints = pensionRankPoints,
            // Memperbarui `TieBreakers` menggunakan `tieBreakers` (nilai tie breakers) dalam ReadRulesetDefinitionAsync.
            TieBreakers = tieBreakers,
            // Memperbarui `ShariaLoans` menggunakan `shariaLoans` (nilai sharia pinjaman) dalam ReadRulesetDefinitionAsync.
            ShariaLoans = shariaLoans,
            // Memperbarui `InsuranceProducts` menggunakan `insuranceProducts` (nilai asuransi products) dalam ReadRulesetDefinitionAsync.
            InsuranceProducts = insuranceProducts,
            // Memperbarui `LifeRisks` menggunakan `lifeRisks` (nilai life risks) dalam ReadRulesetDefinitionAsync.
            LifeRisks = lifeRisks
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam ReadRulesetDefinitionAsync.
        };
    // Menutup scope metode ReadRulesetDefinitionAsync; bagian berikut berada di luar batas blok tersebut dalam ReadRulesetDefinitionAsync.
    }

    // Mendefinisikan metode `BuildPlayerOrdering` dengan hasil bertipe `RulesetPlayerOrderingDto`; operasi ini menangani build pemain ordering.
    // Masukan: Parameter `rules` bertipe `IReadOnlyCollection<PlayerOrderingRuleRow>` membawa nilai rules.
    private static RulesetPlayerOrderingDto BuildPlayerOrdering(
        // Parameter `rules` bertipe `IReadOnlyCollection<PlayerOrderingRuleRow>` membawa nilai rules.
        IReadOnlyCollection<PlayerOrderingRuleRow> rules)
    // Membuka scope metode BuildPlayerOrdering; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildPlayerOrdering.
    {
        // Menyiapkan variabel lokal `orderingCode` untuk nilai ordering kode dengan `rules .Where(rule => !string.IsNullOrWhiteSpace(rule.OrderingCode))
        // .OrderBy(rule => rule.SortOrder) .Select(rule => rule.OrderingCode!) .FirstOrDefault()` bila tidak null; jika null gunakan `”PLAYER_ORDER”`
        // sebagai nilai pengganti. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var orderingCode = rules
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(rule => !string.IsNullOrWhiteSpace(rule.OrderingCode)) dalam
            // BuildPlayerOrdering; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Where(rule => !string.IsNullOrWhiteSpace(rule.OrderingCode))
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .OrderBy(rule => rule.SortOrder) dalam BuildPlayerOrdering; token pada baris
            // ini menyambungkan bagian kode sebelum dan sesudahnya.
            .OrderBy(rule => rule.SortOrder)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Select(rule => rule.OrderingCode!) dalam BuildPlayerOrdering; token pada baris
            // ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Select(rule => rule.OrderingCode!)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .FirstOrDefault() ?? ”PLAYER_ORDER”; dalam BuildPlayerOrdering; token pada
            // baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .FirstOrDefault() ?? "PLAYER_ORDER";

        // Menyiapkan variabel lokal `friday` untuk nilai friday dengan mengambil elemen pertama `rules` yang sesuai `rule =>
        // string.Equals(rule.WeekdayCode, ”FRI”, StringComparison.OrdinalIgnoreCase)`; jika tidak ada, gunakan nilai default tipe hasil. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var friday = rules.FirstOrDefault(rule => string.Equals(rule.WeekdayCode, "FRI", StringComparison.OrdinalIgnoreCase));
        // Menyiapkan variabel lokal `saturday` untuk nilai saturday dengan mengambil elemen pertama `rules` yang sesuai `rule =>
        // string.Equals(rule.WeekdayCode, ”SAT”, StringComparison.OrdinalIgnoreCase)`; jika tidak ada, gunakan nilai default tipe hasil. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var saturday = rules.FirstOrDefault(rule => string.Equals(rule.WeekdayCode, "SAT", StringComparison.OrdinalIgnoreCase));
        // Menyiapkan variabel lokal `sunday` untuk nilai sunday dengan mengambil elemen pertama `rules` yang sesuai `rule =>
        // string.Equals(rule.WeekdayCode, ”SUN”, StringComparison.OrdinalIgnoreCase)`; jika tidak ada, gunakan nilai default tipe hasil. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var sunday = rules.FirstOrDefault(rule => string.Equals(rule.WeekdayCode, "SUN", StringComparison.OrdinalIgnoreCase));

        // Mengembalikan objek baru bertipe `RulesetPlayerOrderingDto` dengan nilai awal sesuai konstruktornya kepada pemanggil dalam BuildPlayerOrdering;
        // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return new RulesetPlayerOrderingDto
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildPlayerOrdering.
        {
            // Memperbarui `OrderingCode` menggunakan `orderingCode` (nilai ordering kode) dalam BuildPlayerOrdering.
            OrderingCode = orderingCode,
            // Memperbarui `FridayFeature` menggunakan `friday?.FeatureCode` bila tidak null; jika null gunakan `”DONATION”` sebagai nilai pengganti dalam
            // BuildPlayerOrdering.
            FridayFeature = friday?.FeatureCode ?? "DONATION",
            // Memperbarui `FridayEnabled` menggunakan `friday?.IsEnabled` bila tidak null; jika null gunakan `true` sebagai nilai pengganti dalam
            // BuildPlayerOrdering.
            FridayEnabled = friday?.IsEnabled ?? true,
            // Memperbarui `SaturdayFeature` menggunakan `saturday?.FeatureCode` bila tidak null; jika null gunakan `”GOLD_TRADE”` sebagai nilai pengganti dalam
            // BuildPlayerOrdering.
            SaturdayFeature = saturday?.FeatureCode ?? "GOLD_TRADE",
            // Memperbarui `SaturdayEnabled` menggunakan `saturday?.IsEnabled` bila tidak null; jika null gunakan `true` sebagai nilai pengganti dalam
            // BuildPlayerOrdering.
            SaturdayEnabled = saturday?.IsEnabled ?? true,
            // Memperbarui `SundayFeature` menggunakan `sunday?.FeatureCode` bila tidak null; jika null gunakan `”REST”` sebagai nilai pengganti dalam
            // BuildPlayerOrdering.
            SundayFeature = sunday?.FeatureCode ?? "REST",
            // Memperbarui `SundayEnabled` menggunakan `sunday?.IsEnabled` bila tidak null; jika null gunakan `true` sebagai nilai pengganti dalam
            // BuildPlayerOrdering.
            SundayEnabled = sunday?.IsEnabled ?? true
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam BuildPlayerOrdering.
        };
    // Menutup scope metode BuildPlayerOrdering; bagian berikut berada di luar batas blok tersebut dalam BuildPlayerOrdering.
    }

    // Mendefinisikan metode `SerializeJsonElement` dengan hasil bertipe `string`; operasi ini menangani serialize JSON element. Masukan: Parameter
    // `element` bertipe `JsonElement` membawa nilai element.
    private static string SerializeJsonElement(JsonElement element)
    // Membuka scope metode SerializeJsonElement; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam SerializeJsonElement.
    {
        // Memeriksa hasil pencocokan `element.ValueKind` dengan pola `JsonValueKind.Undefined or JsonValueKind.Null`; blok if hanya dijalankan ketika
        // kondisi ini bernilai benar dalam SerializeJsonElement.
        if (element.ValueKind is JsonValueKind.Undefined or JsonValueKind.Null)
        // Membuka scope cabang if untuk kondisi `element.ValueKind is JsonValueKind.Undefined or JsonValueKind.Null`; pernyataan/deklarasi berikut berada
        // di dalam batas blok ini dalam SerializeJsonElement.
        {
            // Mengembalikan nilai literal `”{}”` kepada pemanggil dalam SerializeJsonElement; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return "{}";
        // Menutup scope cabang if untuk kondisi `element.ValueKind is JsonValueKind.Undefined or JsonValueKind.Null`; bagian berikut berada di luar batas
        // blok tersebut dalam SerializeJsonElement.
        }

        // Mengembalikan mengambil representasi JSON mentah dari `element` untuk disimpan atau diteruskan kepada pemanggil dalam SerializeJsonElement;
        // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return element.GetRawText();
    // Menutup scope metode SerializeJsonElement; bagian berikut berada di luar batas blok tersebut dalam SerializeJsonElement.
    }

    // Mendefinisikan metode `ParseJsonElement` dengan hasil bertipe `JsonElement`; operasi ini menangani parse JSON element. Masukan: Parameter `json`
    // bertipe `string?` membawa nilai JSON; nilai null diizinkan ketika data opsional belum tersedia.
    private static JsonElement ParseJsonElement(string? json)
    // Membuka scope metode ParseJsonElement; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ParseJsonElement.
    {
        // Menyiapkan variabel lokal `document` untuk nilai document dengan memanggil `JsonDocument.Parse` dengan `string.IsNullOrWhiteSpace(json) ? ”{}” :
        // json`. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var document = JsonDocument.Parse(string.IsNullOrWhiteSpace(json) ? "{}" : json);
        // Mengembalikan membuat salinan `document.RootElement` agar hasil dapat digunakan terpisah dari objek sumber kepada pemanggil dalam
        // ParseJsonElement; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return document.RootElement.Clone();
    // Menutup scope metode ParseJsonElement; bagian berikut berada di luar batas blok tersebut dalam ParseJsonElement.
    }

    // Mendefinisikan metode `ParseStringArray` dengan hasil bertipe `List<string>`; operasi ini menangani parse string array. Masukan: Parameter `json`
    // bertipe `string?` membawa nilai JSON; nilai null diizinkan ketika data opsional belum tersedia.
    private static List<string> ParseStringArray(string? json)
    // Membuka scope metode ParseStringArray; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ParseStringArray.
    {
        // Memeriksa memeriksa apakah `json` null, kosong, atau hanya berisi karakter spasi; blok if hanya dijalankan ketika kondisi ini bernilai benar
        // dalam ParseStringArray.
        if (string.IsNullOrWhiteSpace(json))
        // Membuka scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(json)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ParseStringArray.
        {
            // Mengembalikan koleksi kosong dengan tipe mengikuti konteks tujuan kepada pemanggil dalam ParseStringArray; eksekusi jalur ini selesai setelah
            // nilai hasil ditentukan.
            return [];
        // Menutup scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(json)`; bagian berikut berada di luar batas blok tersebut dalam
        // ParseStringArray.
        }

        // Memulai blok try dalam ParseStringArray; exception dari blok ini dapat dialihkan ke catch, sedangkan finally (jika ada) tetap dijalankan saat
        // keluar.
        try
        // Membuka scope penanganan operasi try; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ParseStringArray.
        {
            // Menyiapkan variabel lokal `document` untuk nilai document dengan memanggil `JsonDocument.Parse` dengan `json`. Tipe variabel disimpulkan dari
            // ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
            using var document = JsonDocument.Parse(json);
            // Memeriksa perbandingan ketidaksamaan antara `document.RootElement.ValueKind` dan `JsonValueKind.Array`; blok if hanya dijalankan ketika kondisi
            // ini bernilai benar dalam ParseStringArray.
            if (document.RootElement.ValueKind != JsonValueKind.Array)
            // Membuka scope cabang if untuk kondisi `document.RootElement.ValueKind != JsonValueKind.Array`; pernyataan/deklarasi berikut berada di dalam batas
            // blok ini dalam ParseStringArray.
            {
                // Mengembalikan koleksi kosong dengan tipe mengikuti konteks tujuan kepada pemanggil dalam ParseStringArray; eksekusi jalur ini selesai setelah
                // nilai hasil ditentukan.
                return [];
            // Menutup scope cabang if untuk kondisi `document.RootElement.ValueKind != JsonValueKind.Array`; bagian berikut berada di luar batas blok tersebut
            // dalam ParseStringArray.
            }

            // Mengembalikan mematerialisasi urutan `document.RootElement .EnumerateArray() .Where(item => item.ValueKind == JsonValueKind.String) .Select(item
            // => item.GetString()) .Where(item => !string.IsNullOrWhiteSpace(item)...` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori
            // kepada pemanggil dalam ParseStringArray; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return document.RootElement
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .EnumerateArray() dalam ParseStringArray; token pada baris ini menyambungkan
                // bagian kode sebelum dan sesudahnya.
                .EnumerateArray()
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(item => item.ValueKind == JsonValueKind.String) dalam ParseStringArray;
                // token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
                .Where(item => item.ValueKind == JsonValueKind.String)
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Select(item => item.GetString()) dalam ParseStringArray; token pada baris ini
                // menyambungkan bagian kode sebelum dan sesudahnya.
                .Select(item => item.GetString())
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(item => !string.IsNullOrWhiteSpace(item)) dalam ParseStringArray; token
                // pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
                .Where(item => !string.IsNullOrWhiteSpace(item))
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Select(item => item!) dalam ParseStringArray; token pada baris ini
                // menyambungkan bagian kode sebelum dan sesudahnya.
                .Select(item => item!)
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList(); dalam ParseStringArray; token pada baris ini menyambungkan bagian
                // kode sebelum dan sesudahnya.
                .ToList();
        // Menutup scope penanganan operasi try; bagian berikut berada di luar batas blok tersebut dalam ParseStringArray.
        }
        // Menangani exception `JsonException` melalui variabel dalam ParseStringArray.
        catch (JsonException)
        // Membuka scope penanganan exception catch; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ParseStringArray.
        {
            // Mengembalikan koleksi kosong dengan tipe mengikuti konteks tujuan kepada pemanggil dalam ParseStringArray; eksekusi jalur ini selesai setelah
            // nilai hasil ditentukan.
            return [];
        // Menutup scope penanganan exception catch; bagian berikut berada di luar batas blok tersebut dalam ParseStringArray.
        }
    // Menutup scope metode ParseStringArray; bagian berikut berada di luar batas blok tersebut dalam ParseStringArray.
    }

    /// <summary>
    /// Menghitung SHA-256 hash dari definisi ruleset untuk deteksi perubahan.
    /// </summary>
    // Mendefinisikan metode `ComputeHash` dengan hasil bertipe `string`. Menghitung SHA-256 hash dari definisi ruleset untuk deteksi perubahan.
    // Masukan: Parameter `definition` bertipe `RulesetDefinitionDto` membawa definisi terstruktur komponen serta parameter aturan permainan.
    private static string ComputeHash(RulesetDefinitionDto definition)
    // Membuka scope metode ComputeHash; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ComputeHash.
    {
        // Menyiapkan variabel lokal `input` untuk nilai input dengan menserialisasi `definition` menjadi JSON melalui `JsonSerializer.Serialize`. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var input = JsonSerializer.Serialize(definition);
        // Menyiapkan variabel lokal `bytes` untuk nilai bytes dengan memanggil `SHA256.HashData` dengan `Encoding.UTF8.GetBytes(input)`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        // Mengembalikan memanggil `Convert.ToHexStringLower` dengan `bytes` kepada pemanggil dalam ComputeHash; eksekusi jalur ini selesai setelah nilai
        // hasil ditentukan.
        return Convert.ToHexStringLower(bytes);
    // Menutup scope metode ComputeHash; bagian berikut berada di luar batas blok tersebut dalam ComputeHash.
    }

    // Mendefinisikan metode `ResolveMode` dengan hasil bertipe `string`; operasi ini menangani resolve mode. Masukan: Parameter `definition` bertipe
    // `RulesetDefinitionDto` membawa definisi terstruktur komponen serta parameter aturan permainan.
    private static string ResolveMode(RulesetDefinitionDto definition)
    // Membuka scope metode ResolveMode; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveMode.
    {
        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!RulesetRuntimeMapper.TryBuildConfig(definition, out var config, out _)
        // || config is null` dan `string.IsNullOrWhiteSpace(config.Mode)`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika
        // kondisi ini bernilai benar dalam ResolveMode.
        if (!RulesetRuntimeMapper.TryBuildConfig(definition, out var config, out _) ||
            // Menggunakan `config` (konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan) sebagai bagian ekspresi yang sedang disusun dalam
            // ResolveMode.
            config is null ||
            // Melanjutkan pengolahan dengan memeriksa apakah `config.Mode` null, kosong, atau hanya berisi karakter spasi dalam ResolveMode.
            string.IsNullOrWhiteSpace(config.Mode))
        // Membuka scope cabang if untuk kondisi `!RulesetRuntimeMapper.TryBuildConfig(definition, out var config, out _) || config is null ||
        // string.IsNullOrWhiteSpace(config.Mode)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveMode.
        {
            // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen (”Definition ruleset harus memiliki mode yang
            // valid.”) dalam ResolveMode; pemanggil atau middleware penanganan error menerima kegagalan ini.
            throw new InvalidOperationException("Definition ruleset harus memiliki mode yang valid.");
        // Menutup scope cabang if untuk kondisi `!RulesetRuntimeMapper.TryBuildConfig(definition, out var config, out _) || config is null ||
        // string.IsNullOrWhiteSpace(config.Mode)`; bagian berikut berada di luar batas blok tersebut dalam ResolveMode.
        }

        // Mengembalikan menormalisasi `config.Mode` menjadi huruf besar dengan aturan kultur invariant kepada pemanggil dalam ResolveMode; eksekusi jalur
        // ini selesai setelah nilai hasil ditentukan.
        return config.Mode.ToUpperInvariant();
    // Menutup scope metode ResolveMode; bagian berikut berada di luar batas blok tersebut dalam ResolveMode.
    }

    // Mendefinisikan tipe class `RulesetSettingsRow`; sealed mencegah tipe ini diturunkan lagi.
    private sealed class RulesetSettingsRow
    // Membuka scope tipe RulesetSettingsRow; pernyataan/deklarasi berikut berada di dalam batas blok ini.
    {
        // Mendefinisikan properti `StartingCash` bertipe `int` untuk nilai starting uang tunai; get menyediakan pembacaan nilai, init membatasi pengisian
        // saat inisialisasi objek.
        public int StartingCash { get; init; }
        // Mendefinisikan properti `InitialHappiness` bertipe `int` untuk nilai awal kebahagiaan; get menyediakan pembacaan nilai, init membatasi pengisian
        // saat inisialisasi objek.
        public int InitialHappiness { get; init; }
        // Mendefinisikan properti `InitialSaving` bertipe `int` untuk nilai awal tabungan; get menyediakan pembacaan nilai, init membatasi pengisian saat
        // inisialisasi objek.
        public int InitialSaving { get; init; }
        // Mendefinisikan properti `ActionsPerTurn` bertipe `int` untuk nilai aksi per giliran; get menyediakan pembacaan nilai, init membatasi pengisian
        // saat inisialisasi objek.
        public int ActionsPerTurn { get; init; }
        // Mendefinisikan properti `FinishDay` bertipe `int` untuk nilai finish hari; get menyediakan pembacaan nilai, init membatasi pengisian saat
        // inisialisasi objek.
        public int FinishDay { get; init; }
        // Mendefinisikan properti `MinPlayers` bertipe `int` untuk nilai minimum pemain; get menyediakan pembacaan nilai, init membatasi pengisian saat
        // inisialisasi objek.
        public int MinPlayers { get; init; }
        // Mendefinisikan properti `MaxPlayers` bertipe `int` untuk nilai maksimum pemain; get menyediakan pembacaan nilai, init membatasi pengisian saat
        // inisialisasi objek.
        public int MaxPlayers { get; init; }
        // Mendefinisikan properti `CashMin` bertipe `int` untuk nilai uang tunai minimum; get menyediakan pembacaan nilai, init membatasi pengisian saat
        // inisialisasi objek.
        public int CashMin { get; init; }
        // Mendefinisikan properti `MaxIngredientTotal` bertipe `int` untuk nilai maksimum bahan total; get menyediakan pembacaan nilai, init membatasi
        // pengisian saat inisialisasi objek.
        public int MaxIngredientTotal { get; init; }
        // Mendefinisikan properti `MaxSameIngredient` bertipe `int` untuk nilai maksimum same bahan; get menyediakan pembacaan nilai, init membatasi
        // pengisian saat inisialisasi objek.
        public int MaxSameIngredient { get; init; }
        // Mendefinisikan properti `PrimaryNeedMaxPerDay` bertipe `int?` untuk nilai primary kebutuhan maksimum per hari; get menyediakan pembacaan nilai,
        // init membatasi pengisian saat inisialisasi objek; tanda ? mengizinkan nilai null.
        public int? PrimaryNeedMaxPerDay { get; init; }
        // Mendefinisikan properti `RequirePrimaryBeforeOthers` bertipe `bool` untuk nilai require primary before others; get menyediakan pembacaan nilai,
        // init membatasi pengisian saat inisialisasi objek.
        public bool RequirePrimaryBeforeOthers { get; init; }
        // Mendefinisikan properti `DonationMinAmount` bertipe `int` untuk nilai donasi minimum nominal; get menyediakan pembacaan nilai, init membatasi
        // pengisian saat inisialisasi objek.
        public int DonationMinAmount { get; init; }
        // Mendefinisikan properti `DonationMaxAmount` bertipe `int` untuk nilai donasi maksimum nominal; get menyediakan pembacaan nilai, init membatasi
        // pengisian saat inisialisasi objek.
        public int DonationMaxAmount { get; init; }
        // Mendefinisikan properti `GoldTradeAllowBuy` bertipe `bool` untuk nilai emas trade allow buy; get menyediakan pembacaan nilai, init membatasi
        // pengisian saat inisialisasi objek.
        public bool GoldTradeAllowBuy { get; init; }
        // Mendefinisikan properti `GoldTradeAllowSell` bertipe `bool` untuk nilai emas trade allow sell; get menyediakan pembacaan nilai, init membatasi
        // pengisian saat inisialisasi objek.
        public bool GoldTradeAllowSell { get; init; }
        // Mendefinisikan properti `LoanEnabled` bertipe `bool` untuk nilai pinjaman enabled; get menyediakan pembacaan nilai, init membatasi pengisian saat
        // inisialisasi objek.
        public bool LoanEnabled { get; init; }
        // Mendefinisikan properti `InsuranceEnabled` bertipe `bool` untuk nilai asuransi enabled; get menyediakan pembacaan nilai, init membatasi pengisian
        // saat inisialisasi objek.
        public bool InsuranceEnabled { get; init; }
        // Mendefinisikan properti `SavingGoalEnabled` bertipe `bool` untuk nilai tabungan target enabled; get menyediakan pembacaan nilai, init membatasi
        // pengisian saat inisialisasi objek.
        public bool SavingGoalEnabled { get; init; }
        // Mendefinisikan properti `FreelanceIncome` bertipe `int` untuk nilai freelance pemasukan; get menyediakan pembacaan nilai, init membatasi
        // pengisian saat inisialisasi objek.
        public int FreelanceIncome { get; init; }
    // Menutup scope tipe RulesetSettingsRow; bagian berikut berada di luar batas blok tersebut.
    }

    // Mendefinisikan tipe class `PlayerOrderingRuleRow`; sealed mencegah tipe ini diturunkan lagi.
    private sealed class PlayerOrderingRuleRow
    // Membuka scope tipe PlayerOrderingRuleRow; pernyataan/deklarasi berikut berada di dalam batas blok ini.
    {
        // Mendefinisikan properti `OrderingCode` bertipe `string?` untuk nilai ordering kode; get menyediakan pembacaan nilai, init membatasi pengisian
        // saat inisialisasi objek; tanda ? mengizinkan nilai null.
        public string? OrderingCode { get; init; }
        // Mendefinisikan properti `WeekdayCode` bertipe `string?` untuk nilai weekday kode; get menyediakan pembacaan nilai, init membatasi pengisian saat
        // inisialisasi objek; tanda ? mengizinkan nilai null.
        public string? WeekdayCode { get; init; }
        // Mendefinisikan properti `FeatureCode` bertipe `string?` untuk nilai feature kode; get menyediakan pembacaan nilai, init membatasi pengisian saat
        // inisialisasi objek; tanda ? mengizinkan nilai null.
        public string? FeatureCode { get; init; }
        // Mendefinisikan properti `IsEnabled` bertipe `bool` untuk nilai berstatus enabled; get menyediakan pembacaan nilai, init membatasi pengisian saat
        // inisialisasi objek.
        public bool IsEnabled { get; init; }
        // Mendefinisikan properti `SortOrder` bertipe `int` untuk nilai sort urutan/pesanan; get menyediakan pembacaan nilai, init membatasi pengisian saat
        // inisialisasi objek.
        public int SortOrder { get; init; }
    // Menutup scope tipe PlayerOrderingRuleRow; bagian berikut berada di luar batas blok tersebut.
    }

    // Mendefinisikan tipe class `OrderRow`; sealed mencegah tipe ini diturunkan lagi.
    private sealed class OrderRow
    // Membuka scope tipe OrderRow; pernyataan/deklarasi berikut berada di dalam batas blok ini.
    {
        // Mendefinisikan properti `RulesetOrderId` bertipe `Guid` untuk nilai aturan urutan/pesanan identitas; get menyediakan pembacaan nilai, init
        // membatasi pengisian saat inisialisasi objek.
        public Guid RulesetOrderId { get; init; }
        // Mendefinisikan properti `Id` bertipe `string` untuk nilai identitas; get menyediakan pembacaan nilai, init membatasi pengisian saat inisialisasi
        // objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
        public string Id { get; init; } = string.Empty;
        // Mendefinisikan properti `Nama` bertipe `string` untuk nilai nama; get menyediakan pembacaan nilai, init membatasi pengisian saat inisialisasi
        // objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
        public string Nama { get; init; } = string.Empty;
        // Mendefinisikan properti `HargaJual` bertipe `int` untuk nilai harga jual; get menyediakan pembacaan nilai, init membatasi pengisian saat
        // inisialisasi objek.
        public int HargaJual { get; init; }
        // Mendefinisikan properti `PoinKebahagiaan` bertipe `int` untuk nilai poin kebahagiaan; get menyediakan pembacaan nilai, init membatasi pengisian
        // saat inisialisasi objek.
        public int PoinKebahagiaan { get; init; }
        // Mendefinisikan properti `SortOrder` bertipe `int` untuk nilai sort urutan/pesanan; get menyediakan pembacaan nilai, init membatasi pengisian saat
        // inisialisasi objek.
        public int SortOrder { get; init; }
        // Mendefinisikan properti `CardQty` bertipe `int?` untuk nilai kartu qty; get menyediakan pembacaan nilai, init membatasi pengisian saat
        // inisialisasi objek; tanda ? mengizinkan nilai null.
        public int? CardQty { get; init; }
    // Menutup scope tipe OrderRow; bagian berikut berada di luar batas blok tersebut.
    }

    // Mendefinisikan tipe class `OrderRequirementRow`; sealed mencegah tipe ini diturunkan lagi.
    private sealed class OrderRequirementRow
    // Membuka scope tipe OrderRequirementRow; pernyataan/deklarasi berikut berada di dalam batas blok ini.
    {
        // Mendefinisikan properti `RulesetOrderId` bertipe `Guid` untuk nilai aturan urutan/pesanan identitas; get menyediakan pembacaan nilai, init
        // membatasi pengisian saat inisialisasi objek.
        public Guid RulesetOrderId { get; init; }
        // Mendefinisikan properti `RequirementOrder` bertipe `int` untuk nilai requirement urutan/pesanan; get menyediakan pembacaan nilai, init membatasi
        // pengisian saat inisialisasi objek.
        public int RequirementOrder { get; init; }
        // Mendefinisikan properti `IngredientValue` bertipe `string` untuk nilai bahan nilai; get menyediakan pembacaan nilai, init membatasi pengisian
        // saat inisialisasi objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
        public string IngredientValue { get; init; } = string.Empty;
        // Mendefinisikan properti `QtyRequired` bertipe `int` untuk nilai qty required; get menyediakan pembacaan nilai, init membatasi pengisian saat
        // inisialisasi objek.
        public int QtyRequired { get; init; }
    // Menutup scope tipe OrderRequirementRow; bagian berikut berada di luar batas blok tersebut.
    }

    // Mendefinisikan tipe class `CollectionMissionRow`; sealed mencegah tipe ini diturunkan lagi.
    private sealed class CollectionMissionRow
    // Membuka scope tipe CollectionMissionRow; pernyataan/deklarasi berikut berada di dalam batas blok ini.
    {
        // Mendefinisikan properti `RulesetCollectionMissionId` bertipe `Guid` untuk nilai aturan collection misi identitas; get menyediakan pembacaan
        // nilai, init membatasi pengisian saat inisialisasi objek.
        public Guid RulesetCollectionMissionId { get; init; }
        // Mendefinisikan properti `Id` bertipe `string` untuk nilai identitas; get menyediakan pembacaan nilai, init membatasi pengisian saat inisialisasi
        // objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
        public string Id { get; init; } = string.Empty;
        // Mendefinisikan properti `Nama` bertipe `string` untuk nilai nama; get menyediakan pembacaan nilai, init membatasi pengisian saat inisialisasi
        // objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
        public string Nama { get; init; } = string.Empty;
        // Mendefinisikan properti `SuccessPoints` bertipe `int` untuk nilai success poin; get menyediakan pembacaan nilai, init membatasi pengisian saat
        // inisialisasi objek.
        public int SuccessPoints { get; init; }
        // Mendefinisikan properti `FailurePoints` bertipe `int` untuk nilai failure poin; get menyediakan pembacaan nilai, init membatasi pengisian saat
        // inisialisasi objek.
        public int FailurePoints { get; init; }
        // Mendefinisikan properti `PenaltyPoints` bertipe `int` untuk nilai penalti poin; get menyediakan pembacaan nilai, init membatasi pengisian saat
        // inisialisasi objek.
        public int PenaltyPoints { get; init; }
        // Mendefinisikan properti `SortOrder` bertipe `int` untuk nilai sort urutan/pesanan; get menyediakan pembacaan nilai, init membatasi pengisian saat
        // inisialisasi objek.
        public int SortOrder { get; init; }
    // Menutup scope tipe CollectionMissionRow; bagian berikut berada di luar batas blok tersebut.
    }

    // Mendefinisikan tipe class `CollectionMissionRequirementRow`; sealed mencegah tipe ini diturunkan lagi.
    private sealed class CollectionMissionRequirementRow
    // Membuka scope tipe CollectionMissionRequirementRow; pernyataan/deklarasi berikut berada di dalam batas blok ini.
    {
        // Mendefinisikan properti `RulesetCollectionMissionId` bertipe `Guid` untuk nilai aturan collection misi identitas; get menyediakan pembacaan
        // nilai, init membatasi pengisian saat inisialisasi objek.
        public Guid RulesetCollectionMissionId { get; init; }
        // Mendefinisikan properti `RequirementOrder` bertipe `int` untuk nilai requirement urutan/pesanan; get menyediakan pembacaan nilai, init membatasi
        // pengisian saat inisialisasi objek.
        public int RequirementOrder { get; init; }
        // Mendefinisikan properti `Type` bertipe `string` untuk nilai jenis; get menyediakan pembacaan nilai, init membatasi pengisian saat inisialisasi
        // objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
        public string Type { get; init; } = string.Empty;
        // Mendefinisikan properti `Value` bertipe `string` untuk nilai nilai; get menyediakan pembacaan nilai, init membatasi pengisian saat inisialisasi
        // objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
        public string Value { get; init; } = string.Empty;
    // Menutup scope tipe CollectionMissionRequirementRow; bagian berikut berada di luar batas blok tersebut.
    }

    // Mendefinisikan tipe class `NarrativeRow`; sealed mencegah tipe ini diturunkan lagi.
    private sealed class NarrativeRow
    // Membuka scope tipe NarrativeRow; pernyataan/deklarasi berikut berada di dalam batas blok ini.
    {
        // Mendefinisikan properti `RulesetNarrativeId` bertipe `Guid` untuk nilai aturan narrative identitas; get menyediakan pembacaan nilai, init
        // membatasi pengisian saat inisialisasi objek.
        public Guid RulesetNarrativeId { get; init; }
        // Mendefinisikan properti `Id` bertipe `string` untuk nilai identitas; get menyediakan pembacaan nilai, init membatasi pengisian saat inisialisasi
        // objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
        public string Id { get; init; } = string.Empty;
        // Mendefinisikan properti `Nama` bertipe `string` untuk nilai nama; get menyediakan pembacaan nilai, init membatasi pengisian saat inisialisasi
        // objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
        public string Nama { get; init; } = string.Empty;
        // Mendefinisikan properti `TextLinesJson` bertipe `string?` untuk nilai text lines JSON; get menyediakan pembacaan nilai, init membatasi pengisian
        // saat inisialisasi objek; tanda ? mengizinkan nilai null.
        public string? TextLinesJson { get; init; }
        // Mendefinisikan properti `SortOrder` bertipe `int` untuk nilai sort urutan/pesanan; get menyediakan pembacaan nilai, init membatasi pengisian saat
        // inisialisasi objek.
        public int SortOrder { get; init; }
    // Menutup scope tipe NarrativeRow; bagian berikut berada di luar batas blok tersebut.
    }

    // Mendefinisikan tipe class `NarrativePrerequisiteRow`; sealed mencegah tipe ini diturunkan lagi.
    private sealed class NarrativePrerequisiteRow
    // Membuka scope tipe NarrativePrerequisiteRow; pernyataan/deklarasi berikut berada di dalam batas blok ini.
    {
        // Mendefinisikan properti `RulesetNarrativeId` bertipe `Guid` untuk nilai aturan narrative identitas; get menyediakan pembacaan nilai, init
        // membatasi pengisian saat inisialisasi objek.
        public Guid RulesetNarrativeId { get; init; }
        // Mendefinisikan properti `Aksi` bertipe `string` untuk nilai aksi; get menyediakan pembacaan nilai, init membatasi pengisian saat inisialisasi
        // objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
        public string Aksi { get; init; } = string.Empty;
        // Mendefinisikan properti `Value` bertipe `int` untuk nilai nilai; get menyediakan pembacaan nilai, init membatasi pengisian saat inisialisasi
        // objek.
        public int Value { get; init; }
        // Mendefinisikan properti `SortOrder` bertipe `int` untuk nilai sort urutan/pesanan; get menyediakan pembacaan nilai, init membatasi pengisian saat
        // inisialisasi objek.
        public int SortOrder { get; init; }
    // Menutup scope tipe NarrativePrerequisiteRow; bagian berikut berada di luar batas blok tersebut.
    }

// Menutup scope tipe RulesetRepository; bagian berikut berada di luar batas blok tersebut.
}
