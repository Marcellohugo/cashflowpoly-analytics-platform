// Fungsi file: Mengelola pemetaan dan akses PostgreSQL untuk SessionRepository.
// Mengimpor namespace `Dapper` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Dapper;
// Mengimpor namespace `Npgsql` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Npgsql;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Data` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Data;

/// <summary>
/// Repository untuk data sesi dan ruleset yang dikunci per sesi.
/// </summary>
// Mendefinisikan tipe class `SessionRepository`; sealed mencegah tipe ini diturunkan lagi.
public sealed class SessionRepository
// Membuka scope tipe SessionRepository; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendeklarasikan field bertipe `NpgsqlDataSource`: `_dataSource` menyimpan sumber koneksi PostgreSQL yang mengelola pembuatan dan penggunaan ulang
    // koneksi. readonly membatasi penggantian referensi/nilai field pada deklarasi atau konstruktor.
    private readonly NpgsqlDataSource _dataSource;

    /// <summary>
    /// Menerima NpgsqlDataSource untuk koneksi ke database sesi.
    /// </summary>
    // Mendefinisikan konstruktor SessionRepository yang menyiapkan objek dan menerima dependency/nilai awal dari pemanggil; parameter: Parameter
    // `dataSource` bertipe `NpgsqlDataSource` membawa sumber koneksi PostgreSQL yang mengelola pembuatan dan penggunaan ulang koneksi.
    public SessionRepository(NpgsqlDataSource dataSource)
    // Membuka scope konstruktor SessionRepository; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam SessionRepository.
    {
        // Memperbarui `_dataSource` menggunakan `dataSource` (sumber koneksi PostgreSQL yang mengelola pembuatan dan penggunaan ulang koneksi) dalam
        // SessionRepository.
        _dataSource = dataSource;
    // Menutup scope konstruktor SessionRepository; bagian berikut berada di luar batas blok tersebut dalam SessionRepository.
    }

    /// <summary>
    /// Mengambil data sesi berdasarkan session_id.
    /// </summary>
    // Mendefinisikan metode `GetSessionAsync` dengan hasil bertipe `Task<SessionDb?>`. Mengambil data sesi berdasarkan session_id. async memungkinkan
    // metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `sessionId` bertipe `Guid` membawa
    // identitas unik sesi permainan yang menjadi batas data operasi ini; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar
    // operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<SessionDb?> GetSessionAsync(Guid sessionId, CancellationToken ct)
    // Membuka scope metode GetSessionAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam GetSessionAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai
        // adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string sql = ”””`.
        // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select session_id, session_name, mode, status, started_at,
        // ended_at, instructor_user_id, ruleset_version_id, is_archived, archived_at, created_at`.
        // Baris literal 3: FROM memilih tabel/subquery sumber pembacaan: `from sessions`.
        // Baris literal 4: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where session_id = @sessionId`.
        // Baris literal 5: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string sql = """
            select session_id, session_name, mode, status, started_at, ended_at, instructor_user_id, ruleset_version_id, is_archived, archived_at, created_at
            from sessions
            where session_id = @sessionId
            """;

        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Mengembalikan hasil operasi asinkron membaca satu hasil basis data melalui `conn.QuerySingleOrDefaultAsync<SessionDb>` dengan `new
        // CommandDefinition(sql, new { sessionId }, cancellationToken: ct)`; nilai default menunjukkan tidak ada baris hasil; await menunggu hasil tanpa
        // memblokir thread selama operasi belum selesai kepada pemanggil dalam GetSessionAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return await conn.QuerySingleOrDefaultAsync<SessionDb>(new CommandDefinition(sql, new { sessionId }, cancellationToken: ct));
    // Menutup scope metode GetSessionAsync; bagian berikut berada di luar batas blok tersebut dalam GetSessionAsync.
    }

    /// <summary>
    /// Mengambil data sesi yang dimiliki instruktur tertentu.
    /// </summary>
    // Mendefinisikan metode `GetSessionForInstructorAsync` dengan hasil bertipe `Task<SessionDb?>`. Mengambil data sesi yang dimiliki instruktur
    // tertentu. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `sessionId`
    // bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini; Parameter `instructorUserId` bertipe `Guid` membawa
    // identitas instruktur pemilik sesi atau aturan; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan
    // ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<SessionDb?> GetSessionForInstructorAsync(Guid sessionId, Guid instructorUserId, CancellationToken ct)
    // Membuka scope metode GetSessionForInstructorAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // GetSessionForInstructorAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai
        // adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string sql = ”””`.
        // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select session_id, session_name, mode, status, started_at,
        // ended_at, instructor_user_id, ruleset_version_id, is_archived, archived_at, created_at`.
        // Baris literal 3: FROM memilih tabel/subquery sumber pembacaan: `from sessions`.
        // Baris literal 4: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where session_id = @sessionId`.
        // Baris literal 5: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and instructor_user_id = @instructorUserId`.
        // Baris literal 6: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string sql = """
            select session_id, session_name, mode, status, started_at, ended_at, instructor_user_id, ruleset_version_id, is_archived, archived_at, created_at
            from sessions
            where session_id = @sessionId
              and instructor_user_id = @instructorUserId
            """;

        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Mengembalikan hasil operasi asinkron membaca satu hasil basis data melalui `conn.QuerySingleOrDefaultAsync<SessionDb>` dengan `new
        // CommandDefinition(sql, new { sessionId, instructorUserId }, cancellationToken: ct)`; nilai default menunjukkan tidak ada baris hasil; await
        // menunggu hasil tanpa memblokir thread selama operasi belum selesai kepada pemanggil dalam GetSessionForInstructorAsync; eksekusi jalur ini
        // selesai setelah nilai hasil ditentukan.
        return await conn.QuerySingleOrDefaultAsync<SessionDb>(
            // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen (sql, new { sessionId, instructorUserId }, cancellationToken: ct) sebagai
            // argumen ke `conn.QuerySingleOrDefaultAsync<SessionDb>`; Meneruskan `sql` (nilai SQL) sebagai argumen ke konstruktor `CommandDefinition`;
            // Meneruskan objek anonim yang mengelompokkan sessionId, instructorUserId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`;
            // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
            // bernama `cancellationToken`.
            new CommandDefinition(sql, new { sessionId, instructorUserId }, cancellationToken: ct));
    // Menutup scope metode GetSessionForInstructorAsync; bagian berikut berada di luar batas blok tersebut dalam GetSessionForInstructorAsync.
    }

    /// <summary>
    /// Membuat sesi baru dengan satu ruleset_version_id yang terkunci di baris sessions.
    /// </summary>
    // Mendefinisikan metode `CreateSessionAsync` dengan hasil bertipe `Task<Guid>`. Membuat sesi baru dengan satu ruleset_version_id yang terkunci di
    // baris sessions. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter
    // `sessionName` bertipe `string` membawa nilai sesi nama; Parameter `mode` bertipe `string` membawa mode permainan yang menentukan kelompok aturan
    // yang digunakan; Parameter `rulesetVersionId` bertipe `Guid` membawa identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang
    // tepat; Parameter `instructorUserId` bertipe `Guid` membawa identitas instruktur pemilik sesi atau aturan; Parameter `createdBy` bertipe `string?`
    // membawa nilai created berdasarkan; nilai null diizinkan ketika data opsional belum tersedia; Parameter `ct` bertipe `CancellationToken` membawa
    // sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<Guid> CreateSessionAsync(
        // Parameter `sessionName` bertipe `string` membawa nilai sesi nama.
        string sessionName,
        // Parameter `mode` bertipe `string` membawa mode permainan yang menentukan kelompok aturan yang digunakan.
        string mode,
        // Parameter `rulesetVersionId` bertipe `Guid` membawa identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang tepat.
        Guid rulesetVersionId,
        // Parameter `instructorUserId` bertipe `Guid` membawa identitas instruktur pemilik sesi atau aturan.
        Guid instructorUserId,
        // Parameter `createdBy` bertipe `string?` membawa nilai created berdasarkan; nilai null diizinkan ketika data opsional belum tersedia.
        string? createdBy,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    // Membuka scope metode CreateSessionAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CreateSessionAsync.
    {
        // Menyiapkan variabel lokal `sessionId` untuk identitas unik sesi permainan yang menjadi batas data operasi ini dengan memanggil `Guid.NewGuid`
        // dengan tanpa argumen. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var sessionId = Guid.NewGuid();
        // Menyiapkan variabel lokal `createdAt` untuk nilai created at dengan `DateTimeOffset.UtcNow`, yaitu waktu UTC saat operasi dilakukan. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var createdAt = DateTimeOffset.UtcNow;

        // Menyiapkan variabel lokal `insertSession` untuk nilai insert sesi dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe
        // yang dipakai adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string insertSession =
        // ”””`.
        // Baris literal 2: INSERT INTO menetapkan tabel dan kolom tujuan penambahan rekaman: `insert into sessions (session_id, session_name, mode, status,
        // started_at, ended_at, instructor_user_id, ruleset_version_id, created_at)`.
        // Baris literal 3: VALUES menyediakan nilai baris baru sesuai urutan kolom INSERT; placeholder @ diikat ke parameter perintah: `values (@sessionId,
        // @sessionName, @mode, 'CREATED', null, null, @instructorUserId, @rulesetVersionId, @createdAt)`.
        // Baris literal 4: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string insertSession = """
            insert into sessions (session_id, session_name, mode, status, started_at, ended_at, instructor_user_id, ruleset_version_id, created_at)
            values (@sessionId, @sessionName, @mode, 'CREATED', null, null, @instructorUserId, @rulesetVersionId, @createdAt)
            """;

        // Menyiapkan variabel lokal `insertState` untuk nilai insert keadaan dengan literal multiline yang dirinci pada komentar di dekat deklarasinya.
        // Tipe yang dipakai adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string insertState =
        // ”””`.
        // Baris literal 2: INSERT INTO menetapkan tabel dan kolom tujuan penambahan rekaman: `insert into session_states (`.
        // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `session_id,`.
        // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `day,`.
        // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `weekday,`.
        // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `turn_number,`.
        // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `action_slot,`.
        // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `current_session_player_id,`.
        // Baris literal 9: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `current_action_slot,`.
        // Baris literal 10: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `action_slots_left,`.
        // Baris literal 11: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `finish_day,`.
        // Baris literal 12: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `phase,`.
        // Baris literal 13: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `is_game_over,`.
        // Baris literal 14: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `state_version,`.
        // Baris literal 15: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ui_state_json,`.
        // Baris literal 16: CREATE mendefinisikan objek basis data yang diperlukan proses ini: `created_at,`.
        // Baris literal 17: UPDATE memilih tabel yang akan diperbarui; kolom dan batas baris ditentukan oleh SET/WHERE: `updated_at`.
        // Baris literal 18: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 19: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
        // Baris literal 20: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@sessionId,`.
        // Baris literal 21: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `1,`.
        // Baris literal 22: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `'MON',`.
        // Baris literal 23: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `0,`.
        // Baris literal 24: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `1,`.
        // Baris literal 25: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `null,`.
        // Baris literal 26: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `1,`.
        // Baris literal 27: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `rgs.actions_per_turn,`.
        // Baris literal 28: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `rgs.finish_day,`.
        // Baris literal 29: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `'SETUP',`.
        // Baris literal 30: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `false,`.
        // Baris literal 31: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `1,`.
        // Baris literal 32: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `'{}'::jsonb,`.
        // Baris literal 33: CREATE mendefinisikan objek basis data yang diperlukan proses ini: `@createdAt,`.
        // Baris literal 34: CREATE mendefinisikan objek basis data yang diperlukan proses ini: `@createdAt`.
        // Baris literal 35: FROM memilih tabel/subquery sumber pembacaan: `from ruleset_game_settings rgs`.
        // Baris literal 36: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where rgs.ruleset_version_id = @rulesetVersionId`.
        // Baris literal 37: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string insertState = """
            insert into session_states (
                session_id,
                day,
                weekday,
                turn_number,
                action_slot,
                current_session_player_id,
                current_action_slot,
                action_slots_left,
                finish_day,
                phase,
                is_game_over,
                state_version,
                ui_state_json,
                created_at,
                updated_at
            )
            select
                @sessionId,
                1,
                'MON',
                0,
                1,
                null,
                1,
                rgs.actions_per_turn,
                rgs.finish_day,
                'SETUP',
                false,
                1,
                '{}'::jsonb,
                @createdAt,
                @createdAt
            from ruleset_game_settings rgs
            where rgs.ruleset_version_id = @rulesetVersionId
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

        // Menyiapkan variabel lokal `def1` untuk nilai def 1 dengan objek baru bertipe `CommandDefinition` dengan argumen (insertSession, new { sessionId,
        // sessionName, mode, instructorUserId, rulesetVersionId, createdAt }, tx, cancellationToken: ct). Tipe variabel disimpulkan dari ekspresi nilai
        // awal.
        var def1 = new CommandDefinition(insertSession, new { sessionId, sessionName, mode, instructorUserId, rulesetVersionId, createdAt }, tx, cancellationToken: ct);
        // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `def1`; nilai hasil menunjukkan jumlah baris yang
        // terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam CreateSessionAsync.
        await conn.ExecuteAsync(def1);

        // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition( insertState, new { sessionId,
        // rulesetVersionId, createdAt }, tx, cancellationToken: ct)`; nilai hasil menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa
        // memblokir thread selama operasi belum selesai dalam CreateSessionAsync.
        await conn.ExecuteAsync(new CommandDefinition(
            // Meneruskan `insertState` (nilai insert keadaan) sebagai argumen ke konstruktor `CommandDefinition`.
            insertState,
            // Meneruskan objek anonim yang mengelompokkan sessionId, rulesetVersionId, createdAt sebagai satu nilai sebagai argumen ke konstruktor
            // `CommandDefinition`.
            new { sessionId, rulesetVersionId, createdAt },
            // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
            tx,
            // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
            // bernama `cancellationToken`.
            cancellationToken: ct));

        // Menjalankan hasil operasi asinkron mengesahkan transaksi `tx` sehingga perubahan yang terkumpul menjadi permanen; await menunggu hasil tanpa
        // memblokir thread selama operasi belum selesai dalam CreateSessionAsync.
        await tx.CommitAsync(ct);
        // Mengembalikan `sessionId` (identitas unik sesi permainan yang menjadi batas data operasi ini) kepada pemanggil dalam CreateSessionAsync; eksekusi
        // jalur ini selesai setelah nilai hasil ditentukan.
        return sessionId;
    // Menutup scope metode CreateSessionAsync; bagian berikut berada di luar batas blok tersebut dalam CreateSessionAsync.
    }

    /// <summary>
    /// Memperbarui status sesi beserta waktu mulai/selesai.
    /// </summary>
    // Mendefinisikan metode `UpdateStatusAsync` dengan hasil bertipe `Task<bool>`. Memperbarui status sesi beserta waktu mulai/selesai. async
    // memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `sessionId` bertipe `Guid`
    // membawa identitas unik sesi permainan yang menjadi batas data operasi ini; Parameter `status` bertipe `string` membawa nilai status; Parameter
    // `startedAt` bertipe `DateTimeOffset?` membawa nilai started at; nilai null diizinkan ketika data opsional belum tersedia; Parameter `endedAt`
    // bertipe `DateTimeOffset?` membawa nilai ended at; nilai null diizinkan ketika data opsional belum tersedia; Parameter `ct` bertipe
    // `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<bool> UpdateStatusAsync(Guid sessionId, string status, DateTimeOffset? startedAt, DateTimeOffset? endedAt, CancellationToken ct)
    // Membuka scope metode UpdateStatusAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam UpdateStatusAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai
        // adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string sql = ”””`.
        // Baris literal 2: UPDATE memilih tabel yang akan diperbarui; kolom dan batas baris ditentukan oleh SET/WHERE: `update sessions`.
        // Baris literal 3: SET menetapkan nilai kolom yang diperbarui oleh UPDATE: `set status = @status,`.
        // Baris literal 4: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `started_at = @startedAt,`.
        // Baris literal 5: Menyusun pilihan nilai bersyarat di dalam ekspresi SQL: `ended_at = @endedAt`.
        // Baris literal 6: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where session_id = @sessionId;`.
        // Baris literal 7: Pembatas literal/penutup ``; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 8: UPDATE memilih tabel yang akan diperbarui; kolom dan batas baris ditentukan oleh SET/WHERE: `update session_states`.
        // Baris literal 9: SET menetapkan nilai kolom yang diperbarui oleh UPDATE: `set phase = case`.
        // Baris literal 10: Menyusun pilihan nilai bersyarat di dalam ekspresi SQL: `when @status = 'STARTED' then 'PLAYER_TURN'`.
        // Baris literal 11: Menyusun pilihan nilai bersyarat di dalam ekspresi SQL: `when @status = 'ENDED' then 'GAME_END'`.
        // Baris literal 12: Menyusun pilihan nilai bersyarat di dalam ekspresi SQL: `else phase`.
        // Baris literal 13: Menyusun pilihan nilai bersyarat di dalam ekspresi SQL: `end,`.
        // Baris literal 14: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `is_game_over = case when @status = 'ENDED' then true else is_game_over end,`.
        // Baris literal 15: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `current_session_player_id =
        // case`.
        // Baris literal 16: Menyusun pilihan nilai bersyarat di dalam ekspresi SQL: `when @status = 'STARTED' then coalesce(`.
        // Baris literal 17: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `current_session_player_id,`.
        // Baris literal 18: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `(`.
        // Baris literal 19: SELECT menentukan nilai atau kolom yang dikembalikan query: `select sp.session_participant_id`.
        // Baris literal 20: FROM memilih tabel/subquery sumber pembacaan: `from session_participants sp`.
        // Baris literal 21: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where sp.session_id = @sessionId`.
        // Baris literal 22: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by sp.player_order_no asc`.
        // Baris literal 23: LIMIT membatasi jumlah baris yang dikembalikan query: `limit 1`.
        // Baris literal 24: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 25: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 26: Menyusun pilihan nilai bersyarat di dalam ekspresi SQL: `when @status = 'ENDED' then null`.
        // Baris literal 27: Menyusun pilihan nilai bersyarat di dalam ekspresi SQL: `else current_session_player_id`.
        // Baris literal 28: Menyusun pilihan nilai bersyarat di dalam ekspresi SQL: `end,`.
        // Baris literal 29: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `state_version =
        // state_version + 1,`.
        // Baris literal 30: UPDATE memilih tabel yang akan diperbarui; kolom dan batas baris ditentukan oleh SET/WHERE: `updated_at = now()`.
        // Baris literal 31: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where session_id = @sessionId`.
        // Baris literal 32: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string sql = """
            update sessions
            set status = @status,
                started_at = @startedAt,
                ended_at = @endedAt
            where session_id = @sessionId;

            update session_states
            set phase = case
                    when @status = 'STARTED' then 'PLAYER_TURN'
                    when @status = 'ENDED' then 'GAME_END'
                    else phase
                end,
                is_game_over = case when @status = 'ENDED' then true else is_game_over end,
                current_session_player_id = case
                    when @status = 'STARTED' then coalesce(
                        current_session_player_id,
                        (
                            select sp.session_participant_id
                            from session_participants sp
                            where sp.session_id = @sessionId
                            order by sp.player_order_no asc
                            limit 1
                        )
                    )
                    when @status = 'ENDED' then null
                    else current_session_player_id
                end,
                state_version = state_version + 1,
                updated_at = now()
            where session_id = @sessionId
            """;

        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Menyiapkan variabel lokal `rows` untuk nilai baris dengan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new
        // CommandDefinition(sql, new { sessionId, status, startedAt, endedAt }, cancellationToken: ct)`; nilai hasil menunjukkan jumlah baris yang
        // terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var rows = await conn.ExecuteAsync(new CommandDefinition(sql, new { sessionId, status, startedAt, endedAt }, cancellationToken: ct));
        // Mengembalikan pemeriksaan lebih besar antara `rows` dan `0` kepada pemanggil dalam UpdateStatusAsync; eksekusi jalur ini selesai setelah nilai
        // hasil ditentukan.
        return rows > 0;
    // Menutup scope metode UpdateStatusAsync; bagian berikut berada di luar batas blok tersebut dalam UpdateStatusAsync.
    }

    /// <summary>
    /// Mengambil ruleset_version_id yang dikunci pada sesi.
    /// </summary>
    // Mendefinisikan metode `GetActiveRulesetVersionIdAsync` dengan hasil bertipe `Task<Guid?>`. Mengambil ruleset_version_id yang dikunci pada sesi.
    // async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `sessionId` bertipe
    // `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini; Parameter `ct` bertipe `CancellationToken` membawa sinyal
    // pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<Guid?> GetActiveRulesetVersionIdAsync(Guid sessionId, CancellationToken ct)
    // Membuka scope metode GetActiveRulesetVersionIdAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // GetActiveRulesetVersionIdAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai
        // adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string sql = ”””`.
        // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select ruleset_version_id`.
        // Baris literal 3: FROM memilih tabel/subquery sumber pembacaan: `from sessions`.
        // Baris literal 4: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where session_id = @sessionId`.
        // Baris literal 5: LIMIT membatasi jumlah baris yang dikembalikan query: `limit 1`.
        // Baris literal 6: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string sql = """
            select ruleset_version_id
            from sessions
            where session_id = @sessionId
            limit 1
            """;

        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Mengembalikan hasil operasi asinkron membaca satu hasil basis data melalui `conn.QuerySingleOrDefaultAsync<Guid?>` dengan `new
        // CommandDefinition(sql, new { sessionId }, cancellationToken: ct)`; nilai default menunjukkan tidak ada baris hasil; await menunggu hasil tanpa
        // memblokir thread selama operasi belum selesai kepada pemanggil dalam GetActiveRulesetVersionIdAsync; eksekusi jalur ini selesai setelah nilai
        // hasil ditentukan.
        return await conn.QuerySingleOrDefaultAsync<Guid?>(new CommandDefinition(sql, new { sessionId }, cancellationToken: ct));
    // Menutup scope metode GetActiveRulesetVersionIdAsync; bagian berikut berada di luar batas blok tersebut dalam GetActiveRulesetVersionIdAsync.
    }

    /// <summary>
    /// Mengambil hari yang sedang aktif dan batas akhir sesi.
    /// </summary>
    // Mendefinisikan metode `GetProgressAsync` dengan hasil bertipe `Task<SessionProgressDb?>`. Mengambil hari yang sedang aktif dan batas akhir sesi.
    // async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `sessionId` bertipe
    // `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini; Parameter `ct` bertipe `CancellationToken` membawa sinyal
    // pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<SessionProgressDb?> GetProgressAsync(Guid sessionId, CancellationToken ct)
    // Membuka scope metode GetProgressAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam GetProgressAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai
        // adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string sql = ”””`.
        // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select day, finish_day as FinishDay`.
        // Baris literal 3: FROM memilih tabel/subquery sumber pembacaan: `from session_states`.
        // Baris literal 4: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where session_id = @sessionId`.
        // Baris literal 5: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string sql = """
            select day, finish_day as FinishDay
            from session_states
            where session_id = @sessionId
            """;

        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Mengembalikan hasil operasi asinkron membaca satu hasil basis data melalui `conn.QuerySingleOrDefaultAsync<SessionProgressDb>` dengan `new
        // CommandDefinition(sql, new { sessionId }, cancellationToken: ct)`; nilai default menunjukkan tidak ada baris hasil; await menunggu hasil tanpa
        // memblokir thread selama operasi belum selesai kepada pemanggil dalam GetProgressAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return await conn.QuerySingleOrDefaultAsync<SessionProgressDb>(
            // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen (sql, new { sessionId }, cancellationToken: ct) sebagai argumen ke
            // `conn.QuerySingleOrDefaultAsync<SessionProgressDb>`; Meneruskan `sql` (nilai SQL) sebagai argumen ke konstruktor `CommandDefinition`; Meneruskan
            // objek anonim yang mengelompokkan sessionId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`; Meneruskan `ct` (sinyal
            // pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen bernama
            // `cancellationToken`.
            new CommandDefinition(sql, new { sessionId }, cancellationToken: ct));
    // Menutup scope metode GetProgressAsync; bagian berikut berada di luar batas blok tersebut dalam GetProgressAsync.
    }

    /// <summary>
    /// Mengambil skor dan delapan komponen final yang telah dibekukan ketika sesi berakhir.
    /// </summary>
    // Mendefinisikan metode `GetFinalScoresAsync` dengan hasil bertipe `Task<List<SessionFinalScoreDb>>`. Mengambil skor dan delapan komponen final
    // yang telah dibekukan ketika sesi berakhir. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui
    // Task. Masukan: Parameter `sessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini; Parameter `ct`
    // bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi
    // berhenti.
    public async Task<List<SessionFinalScoreDb>> GetFinalScoresAsync(Guid sessionId, CancellationToken ct)
    // Membuka scope metode GetFinalScoresAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam GetFinalScoresAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai
        // adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string sql = ”””`.
        // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
        // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `sp.user_id as UserId,`.
        // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `sp.player_order_no as
        // PlayerOrder,`.
        // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `fs.rank_no as Rank,`.
        // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `fs.total_points as
        // TotalPoints,`.
        // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `coalesce(sum(fsc.points)
        // filter (where fsc.component_code = 'NEED_POINTS'), 0) as NeedPoints,`.
        // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `coalesce(sum(fsc.points)
        // filter (where fsc.component_code = 'NEED_SET_BONUS'), 0) as NeedSetBonusPoints,`.
        // Baris literal 9: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `coalesce(sum(fsc.points)
        // filter (where fsc.component_code = 'DONATION'), 0) as DonationPoints,`.
        // Baris literal 10: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `coalesce(sum(fsc.points)
        // filter (where fsc.component_code = 'GOLD'), 0) as GoldPoints,`.
        // Baris literal 11: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `coalesce(sum(fsc.points)
        // filter (where fsc.component_code = 'PENSION'), 0) as PensionPoints,`.
        // Baris literal 12: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `coalesce(sum(fsc.points)
        // filter (where fsc.component_code = 'SAVING_GOAL'), 0) as SavingGoalPoints,`.
        // Baris literal 13: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `greatest(0,
        // -coalesce(sum(fsc.points) filter (where fsc.component_code = 'MISSION_PENALTY'), 0)) as MissionPenaltyPoints,`.
        // Baris literal 14: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `greatest(0,
        // -coalesce(sum(fsc.points) filter (where fsc.component_code = 'LOAN_PENALTY'), 0)) as LoanPenaltyPoints,`.
        // Baris literal 15: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `fs.has_unpaid_loan as
        // HasUnpaidLoan`.
        // Baris literal 16: FROM memilih tabel/subquery sumber pembacaan: `from session_final_scores fs`.
        // Baris literal 17: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join session_participants sp`.
        // Baris literal 18: ON menetapkan syarat pencocokan baris pada relasi JOIN: `on sp.session_id = fs.session_id`.
        // Baris literal 19: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and sp.session_participant_id =
        // fs.session_participant_id`.
        // Baris literal 20: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `left join session_final_score_components fsc`.
        // Baris literal 21: ON menetapkan syarat pencocokan baris pada relasi JOIN: `on fsc.session_id = fs.session_id`.
        // Baris literal 22: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and fsc.session_final_score_id =
        // fs.session_final_score_id`.
        // Baris literal 23: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where fs.session_id = @sessionId`.
        // Baris literal 24: GROUP BY mengelompokkan baris sebelum fungsi agregasi dihitung: `group by`.
        // Baris literal 25: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `sp.user_id,`.
        // Baris literal 26: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `sp.player_order_no,`.
        // Baris literal 27: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `fs.rank_no,`.
        // Baris literal 28: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `fs.total_points,`.
        // Baris literal 29: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `fs.has_unpaid_loan`.
        // Baris literal 30: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by fs.rank_no, sp.player_order_no`.
        // Baris literal 31: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string sql = """
            select
                sp.user_id as UserId,
                sp.player_order_no as PlayerOrder,
                fs.rank_no as Rank,
                fs.total_points as TotalPoints,
                coalesce(sum(fsc.points) filter (where fsc.component_code = 'NEED_POINTS'), 0) as NeedPoints,
                coalesce(sum(fsc.points) filter (where fsc.component_code = 'NEED_SET_BONUS'), 0) as NeedSetBonusPoints,
                coalesce(sum(fsc.points) filter (where fsc.component_code = 'DONATION'), 0) as DonationPoints,
                coalesce(sum(fsc.points) filter (where fsc.component_code = 'GOLD'), 0) as GoldPoints,
                coalesce(sum(fsc.points) filter (where fsc.component_code = 'PENSION'), 0) as PensionPoints,
                coalesce(sum(fsc.points) filter (where fsc.component_code = 'SAVING_GOAL'), 0) as SavingGoalPoints,
                greatest(0, -coalesce(sum(fsc.points) filter (where fsc.component_code = 'MISSION_PENALTY'), 0)) as MissionPenaltyPoints,
                greatest(0, -coalesce(sum(fsc.points) filter (where fsc.component_code = 'LOAN_PENALTY'), 0)) as LoanPenaltyPoints,
                fs.has_unpaid_loan as HasUnpaidLoan
            from session_final_scores fs
            join session_participants sp
              on sp.session_id = fs.session_id
             and sp.session_participant_id = fs.session_participant_id
            left join session_final_score_components fsc
              on fsc.session_id = fs.session_id
             and fsc.session_final_score_id = fs.session_final_score_id
            where fs.session_id = @sessionId
            group by
                sp.user_id,
                sp.player_order_no,
                fs.rank_no,
                fs.total_points,
                fs.has_unpaid_loan
            order by fs.rank_no, sp.player_order_no
            """;

        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Menyiapkan variabel lokal `scores` untuk nilai skor dengan hasil operasi asinkron menjalankan query baca melalui `conn` dengan `new
        // CommandDefinition(sql, new { sessionId }, cancellationToken: ct)` dan memetakan baris hasil ke tipe yang diminta; await menunggu hasil tanpa
        // memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var scores = await conn.QueryAsync<SessionFinalScoreDb>(
            // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen (sql, new { sessionId }, cancellationToken: ct) sebagai argumen ke
            // `conn.QueryAsync<SessionFinalScoreDb>`; Meneruskan `sql` (nilai SQL) sebagai argumen ke konstruktor `CommandDefinition`; Meneruskan objek anonim
            // yang mengelompokkan sessionId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`; Meneruskan `ct` (sinyal pembatalan agar
            // operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen bernama `cancellationToken`.
            new CommandDefinition(sql, new { sessionId }, cancellationToken: ct));
        // Mengembalikan mematerialisasi urutan `scores` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori kepada pemanggil dalam
        // GetFinalScoresAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return scores.ToList();
    // Menutup scope metode GetFinalScoresAsync; bagian berikut berada di luar batas blok tersebut dalam GetFinalScoresAsync.
    }

    /// <summary>
    /// Mengambil semua sesi diurutkan terbaru.
    /// </summary>
    // Mendefinisikan metode `ListSessionsAsync` dengan hasil bertipe `Task<List<SessionDb>>`. Mengambil semua sesi diurutkan terbaru. async
    // memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `ct` bertipe
    // `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<List<SessionDb>> ListSessionsAsync(CancellationToken ct)
    // Membuka scope metode ListSessionsAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ListSessionsAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai
        // adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string sql = ”””`.
        // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select session_id, session_name, mode, status, started_at,
        // ended_at, instructor_user_id, ruleset_version_id, is_archived, archived_at, created_at`.
        // Baris literal 3: FROM memilih tabel/subquery sumber pembacaan: `from sessions`.
        // Baris literal 4: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where not is_archived`.
        // Baris literal 5: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by created_at desc`.
        // Baris literal 6: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string sql = """
            select session_id, session_name, mode, status, started_at, ended_at, instructor_user_id, ruleset_version_id, is_archived, archived_at, created_at
            from sessions
            where not is_archived
            order by created_at desc
            """;

        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Menyiapkan variabel lokal `items` untuk nilai elemen dengan hasil operasi asinkron menjalankan query baca melalui `conn` dengan `new
        // CommandDefinition(sql, cancellationToken: ct)` dan memetakan baris hasil ke tipe yang diminta; await menunggu hasil tanpa memblokir thread selama
        // operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var items = await conn.QueryAsync<SessionDb>(new CommandDefinition(sql, cancellationToken: ct));
        // Mengembalikan mematerialisasi urutan `items` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori kepada pemanggil dalam
        // ListSessionsAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return items.ToList();
    // Menutup scope metode ListSessionsAsync; bagian berikut berada di luar batas blok tersebut dalam ListSessionsAsync.
    }

    /// <summary>
    /// Mengambil seluruh sesi, termasuk yang diarsipkan, untuk pekerjaan pemeliharaan terkontrol.
    /// </summary>
    // Mendefinisikan metode `ListAllSessionsForMaintenanceAsync` dengan hasil bertipe `Task<List<SessionDb>>`. Mengambil seluruh sesi, termasuk yang
    // diarsipkan, untuk pekerjaan pemeliharaan terkontrol. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian
    // melalui Task. Masukan: Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil
    // membatalkan permintaan atau aplikasi berhenti.
    public async Task<List<SessionDb>> ListAllSessionsForMaintenanceAsync(CancellationToken ct)
    // Membuka scope metode ListAllSessionsForMaintenanceAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // ListAllSessionsForMaintenanceAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai
        // adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string sql = ”””`.
        // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select session_id, session_name, mode, status, started_at,
        // ended_at, instructor_user_id, ruleset_version_id, is_archived, archived_at, created_at`.
        // Baris literal 3: FROM memilih tabel/subquery sumber pembacaan: `from sessions`.
        // Baris literal 4: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by created_at`.
        // Baris literal 5: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string sql = """
            select session_id, session_name, mode, status, started_at, ended_at, instructor_user_id, ruleset_version_id, is_archived, archived_at, created_at
            from sessions
            order by created_at
            """;

        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Menyiapkan variabel lokal `items` untuk nilai elemen dengan hasil operasi asinkron menjalankan query baca melalui `conn` dengan `new
        // CommandDefinition(sql, cancellationToken: ct)` dan memetakan baris hasil ke tipe yang diminta; await menunggu hasil tanpa memblokir thread selama
        // operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var items = await conn.QueryAsync<SessionDb>(new CommandDefinition(sql, cancellationToken: ct));
        // Mengembalikan mematerialisasi urutan `items` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori kepada pemanggil dalam
        // ListAllSessionsForMaintenanceAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return items.ToList();
    // Menutup scope metode ListAllSessionsForMaintenanceAsync; bagian berikut berada di luar batas blok tersebut dalam
    // ListAllSessionsForMaintenanceAsync.
    }

    /// <summary>
    /// Mengambil daftar sesi milik instruktur tertentu.
    /// </summary>
    // Mendefinisikan metode `ListSessionsByInstructorAsync` dengan hasil bertipe `Task<List<SessionDb>>`. Mengambil daftar sesi milik instruktur
    // tertentu. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter
    // `instructorUserId` bertipe `Guid` membawa identitas instruktur pemilik sesi atau aturan; Parameter `ct` bertipe `CancellationToken` membawa
    // sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<List<SessionDb>> ListSessionsByInstructorAsync(Guid instructorUserId, CancellationToken ct)
    // Membuka scope metode ListSessionsByInstructorAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // ListSessionsByInstructorAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai
        // adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string sql = ”””`.
        // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select session_id, session_name, mode, status, started_at,
        // ended_at, instructor_user_id, ruleset_version_id, is_archived, archived_at, created_at`.
        // Baris literal 3: FROM memilih tabel/subquery sumber pembacaan: `from sessions`.
        // Baris literal 4: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where instructor_user_id = @instructorUserId`.
        // Baris literal 5: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and not is_archived`.
        // Baris literal 6: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by created_at desc`.
        // Baris literal 7: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string sql = """
            select session_id, session_name, mode, status, started_at, ended_at, instructor_user_id, ruleset_version_id, is_archived, archived_at, created_at
            from sessions
            where instructor_user_id = @instructorUserId
              and not is_archived
            order by created_at desc
            """;

        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Menyiapkan variabel lokal `items` untuk nilai elemen dengan hasil operasi asinkron menjalankan query baca melalui `conn` dengan `new
        // CommandDefinition(sql, new { instructorUserId }, cancellationToken: ct)` dan memetakan baris hasil ke tipe yang diminta; await menunggu hasil
        // tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var items = await conn.QueryAsync<SessionDb>(new CommandDefinition(sql, new { instructorUserId }, cancellationToken: ct));
        // Mengembalikan mematerialisasi urutan `items` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori kepada pemanggil dalam
        // ListSessionsByInstructorAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return items.ToList();
    // Menutup scope metode ListSessionsByInstructorAsync; bagian berikut berada di luar batas blok tersebut dalam ListSessionsByInstructorAsync.
    }

    /// <summary>
    /// Mengambil daftar sesi yang diikuti pemain tertentu.
    /// </summary>
    // Mendefinisikan metode `ListSessionsByPlayerAsync` dengan hasil bertipe `Task<List<SessionDb>>`. Mengambil daftar sesi yang diikuti pemain
    // tertentu. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `userId`
    // bertipe `Guid` membawa identitas akun pengguna yang datanya sedang diproses; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan
    // agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<List<SessionDb>> ListSessionsByPlayerAsync(Guid userId, CancellationToken ct)
    // Membuka scope metode ListSessionsByPlayerAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ListSessionsByPlayerAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai
        // adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string sql = ”””`.
        // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select distinct s.session_id, s.session_name, s.mode, s.status,
        // s.started_at, s.ended_at, s.instructor_user_id, s.ruleset_version_id, s.is_archived, s.archived_at, s.created_at`.
        // Baris literal 3: FROM memilih tabel/subquery sumber pembacaan: `from sessions s`.
        // Baris literal 4: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join session_participants sp on sp.session_id =
        // s.session_id`.
        // Baris literal 5: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where sp.user_id = @userId`.
        // Baris literal 6: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and not s.is_archived`.
        // Baris literal 7: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by s.created_at desc`.
        // Baris literal 8: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string sql = """
            select distinct s.session_id, s.session_name, s.mode, s.status, s.started_at, s.ended_at, s.instructor_user_id, s.ruleset_version_id, s.is_archived, s.archived_at, s.created_at
            from sessions s
            join session_participants sp on sp.session_id = s.session_id
            where sp.user_id = @userId
              and not s.is_archived
            order by s.created_at desc
            """;

        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Menyiapkan variabel lokal `items` untuk nilai elemen dengan hasil operasi asinkron menjalankan query baca melalui `conn` dengan `new
        // CommandDefinition(sql, new { userId }, cancellationToken: ct)` dan memetakan baris hasil ke tipe yang diminta; await menunggu hasil tanpa
        // memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var items = await conn.QueryAsync<SessionDb>(new CommandDefinition(sql, new { userId }, cancellationToken: ct));
        // Mengembalikan mematerialisasi urutan `items` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori kepada pemanggil dalam
        // ListSessionsByPlayerAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return items.ToList();
    // Menutup scope metode ListSessionsByPlayerAsync; bagian berikut berada di luar batas blok tersebut dalam ListSessionsByPlayerAsync.
    }

// Menutup scope tipe SessionRepository; bagian berikut berada di luar batas blok tersebut.
}
