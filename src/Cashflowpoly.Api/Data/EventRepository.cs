// Fungsi file: Mengelola pemetaan dan akses PostgreSQL untuk EventRepository.
// Mengimpor namespace `Dapper` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Dapper;
// Mengimpor namespace `Npgsql` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Npgsql;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Data` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Data;

/// <summary>
/// Repository untuk event, log validasi, dan proyeksi arus kas.
/// </summary>
// Mendefinisikan tipe class `EventRepository`; sealed mencegah tipe ini diturunkan lagi.
public sealed class EventRepository
// Membuka scope tipe EventRepository; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendeklarasikan field bertipe `NpgsqlDataSource`: `_dataSource` menyimpan sumber koneksi PostgreSQL yang mengelola pembuatan dan penggunaan ulang
    // koneksi. readonly membatasi penggantian referensi/nilai field pada deklarasi atau konstruktor.
    private readonly NpgsqlDataSource _dataSource;

    // Mendeklarasikan field bertipe `string`: `EventSelectColumns` menyimpan nilai event select columns dengan nilai awal literal multiline yang
    // dirinci pada komentar di dekat deklarasinya.
    // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
    // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `private const string
    // EventSelectColumns = ”””`.
    // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
    // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `e.event_pk,`.
    // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `e.event_id,`.
    // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `e.session_id,`.
    // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `e.session_player_id,`.
    // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `e.user_id,`.
    // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `e.actor_type,`.
    // Baris literal 9: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `e.timestamp,`.
    // Baris literal 10: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `e.day_index,`.
    // Baris literal 11: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `e.weekday,`.
    // Baris literal 12: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `e.turn_number,`.
    // Baris literal 13: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `e.action_slot,`.
    // Baris literal 14: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `e.sequence_number,`.
    // Baris literal 15: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `e.ruleset_action_id,`.
    // Baris literal 16: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ra.action_id,`.
    // Baris literal 17: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `e.action_type,`.
    // Baris literal 18: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `e.ruleset_version_id,`.
    // Baris literal 19: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `e.payload_version,`.
    // Baris literal 20: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `coalesce(e.payload::text,
    // '{}') as payload,`.
    // Baris literal 21: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `e.received_at,`.
    // Baris literal 22: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `e.client_request_id`.
    // Baris literal 23: FROM memilih tabel/subquery sumber pembacaan: `from events e`.
    // Baris literal 24: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join ruleset_actions ra`.
    // Baris literal 25: ON menetapkan syarat pencocokan baris pada relasi JOIN: `on ra.ruleset_version_id = e.ruleset_version_id`.
    // Baris literal 26: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and ra.ruleset_action_id = e.ruleset_action_id`.
    // Baris literal 27: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
    private const string EventSelectColumns = """
        select
            e.event_pk,
            e.event_id,
            e.session_id,
            e.session_player_id,
            e.user_id,
            e.actor_type,
            e.timestamp,
            e.day_index,
            e.weekday,
            e.turn_number,
            e.action_slot,
            e.sequence_number,
            e.ruleset_action_id,
            ra.action_id,
            e.action_type,
            e.ruleset_version_id,
            e.payload_version,
            coalesce(e.payload::text, '{}') as payload,
            e.received_at,
            e.client_request_id
        from events e
        join ruleset_actions ra
          on ra.ruleset_version_id = e.ruleset_version_id
         and ra.ruleset_action_id = e.ruleset_action_id
        """;

    /// <summary>
    /// Menerima NpgsqlDataSource untuk koneksi ke tabel events, validation_logs, dan cashflow_projections.
    /// </summary>
    // Mendefinisikan konstruktor EventRepository yang menyiapkan objek dan menerima dependency/nilai awal dari pemanggil; parameter: Parameter
    // `dataSource` bertipe `NpgsqlDataSource` membawa sumber koneksi PostgreSQL yang mengelola pembuatan dan penggunaan ulang koneksi.
    public EventRepository(NpgsqlDataSource dataSource)
    // Membuka scope konstruktor EventRepository; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam EventRepository.
    {
        // Memperbarui `_dataSource` menggunakan `dataSource` (sumber koneksi PostgreSQL yang mengelola pembuatan dan penggunaan ulang koneksi) dalam
        // EventRepository.
        _dataSource = dataSource;
    // Menutup scope konstruktor EventRepository; bagian berikut berada di luar batas blok tersebut dalam EventRepository.
    }

    /// <summary>
    /// Memeriksa apakah event_id sudah ada di database (untuk idempotency).
    /// </summary>
    // Mendefinisikan metode `EventIdExistsAsync` dengan hasil bertipe `Task<bool>`. Memeriksa apakah event_id sudah ada di database (untuk
    // idempotency). async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter
    // `sessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini; Parameter `eventId` bertipe `Guid` membawa
    // identitas unik event untuk pencatatan dan pemeriksaan duplikasi; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar
    // operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<bool> EventIdExistsAsync(Guid sessionId, Guid eventId, CancellationToken ct)
    // Membuka scope metode EventIdExistsAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam EventIdExistsAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai
        // adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string sql = ”””`.
        // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select 1`.
        // Baris literal 3: FROM memilih tabel/subquery sumber pembacaan: `from events`.
        // Baris literal 4: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where session_id = @sessionId and event_id = @eventId`.
        // Baris literal 5: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string sql = """
            select 1
            from events
            where session_id = @sessionId and event_id = @eventId
            """;

        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Menyiapkan variabel lokal `result` untuk nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya dengan hasil operasi asinkron menjalankan
        // perintah basis data melalui `conn` dengan `new CommandDefinition(sql, new { sessionId, eventId }, cancellationToken: ct)` dan mengambil nilai
        // skalar hasilnya; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var result = await conn.ExecuteScalarAsync<int?>(new CommandDefinition(sql, new { sessionId, eventId }, cancellationToken: ct));
        // Mengembalikan `result.HasValue`, yaitu penanda bahwa nilai nullable tidak kosong kepada pemanggil dalam EventIdExistsAsync; eksekusi jalur ini
        // selesai setelah nilai hasil ditentukan.
        return result.HasValue;
    // Menutup scope metode EventIdExistsAsync; bagian berikut berada di luar batas blok tersebut dalam EventIdExistsAsync.
    }

    /// <summary>
    /// Memeriksa apakah sequence_number sudah digunakan dalam satu sesi.
    /// </summary>
    // Mendefinisikan metode `SequenceNumberExistsAsync` dengan hasil bertipe `Task<bool>`. Memeriksa apakah sequence_number sudah digunakan dalam satu
    // sesi. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `sessionId`
    // bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini; Parameter `sequenceNumber` bertipe `long` membawa nomor
    // urut event yang menentukan urutan pemrosesan riwayat permainan; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi
    // dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<bool> SequenceNumberExistsAsync(Guid sessionId, long sequenceNumber, CancellationToken ct)
    // Membuka scope metode SequenceNumberExistsAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam SequenceNumberExistsAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai
        // adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string sql = ”””`.
        // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select 1`.
        // Baris literal 3: FROM memilih tabel/subquery sumber pembacaan: `from events`.
        // Baris literal 4: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where session_id = @sessionId and sequence_number =
        // @sequenceNumber`.
        // Baris literal 5: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string sql = """
            select 1
            from events
            where session_id = @sessionId and sequence_number = @sequenceNumber
            """;

        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Menyiapkan variabel lokal `result` untuk nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya dengan hasil operasi asinkron menjalankan
        // perintah basis data melalui `conn` dengan `new CommandDefinition(sql, new { sessionId, sequenceNumber }, cancellationToken: ct)` dan mengambil
        // nilai skalar hasilnya; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai
        // awal.
        var result = await conn.ExecuteScalarAsync<int?>(new CommandDefinition(sql, new { sessionId, sequenceNumber }, cancellationToken: ct));
        // Mengembalikan `result.HasValue`, yaitu penanda bahwa nilai nullable tidak kosong kepada pemanggil dalam SequenceNumberExistsAsync; eksekusi jalur
        // ini selesai setelah nilai hasil ditentukan.
        return result.HasValue;
    // Menutup scope metode SequenceNumberExistsAsync; bagian berikut berada di luar batas blok tersebut dalam SequenceNumberExistsAsync.
    }

    /// <summary>
    /// Mengambil sequence_number tertinggi pada satu sesi untuk penentuan urutan event berikutnya.
    /// </summary>
    // Mendefinisikan metode `GetMaxSequenceNumberAsync` dengan hasil bertipe `Task<long?>`. Mengambil sequence_number tertinggi pada satu sesi untuk
    // penentuan urutan event berikutnya. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task.
    // Masukan: Parameter `sessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini; Parameter `ct` bertipe
    // `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<long?> GetMaxSequenceNumberAsync(Guid sessionId, CancellationToken ct)
    // Membuka scope metode GetMaxSequenceNumberAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam GetMaxSequenceNumberAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai
        // adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string sql = ”””`.
        // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select max(sequence_number)`.
        // Baris literal 3: FROM memilih tabel/subquery sumber pembacaan: `from events`.
        // Baris literal 4: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where session_id = @sessionId`.
        // Baris literal 5: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string sql = """
            select max(sequence_number)
            from events
            where session_id = @sessionId
            """;

        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Mengembalikan hasil operasi asinkron menjalankan perintah basis data melalui `conn` dengan `new CommandDefinition(sql, new { sessionId },
        // cancellationToken: ct)` dan mengambil nilai skalar hasilnya; await menunggu hasil tanpa memblokir thread selama operasi belum selesai kepada
        // pemanggil dalam GetMaxSequenceNumberAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return await conn.ExecuteScalarAsync<long?>(new CommandDefinition(sql, new { sessionId }, cancellationToken: ct));
    // Menutup scope metode GetMaxSequenceNumberAsync; bagian berikut berada di luar batas blok tersebut dalam GetMaxSequenceNumberAsync.
    }

    /// <summary>
    /// SQL INSERT untuk tabel events, digunakan oleh kedua overload InsertEventAsync.
    /// </summary>
    // Mendeklarasikan field bertipe `string`: `InsertEventSql` menyimpan nilai insert event SQL dengan nilai awal literal multiline yang dirinci pada
    // komentar di dekat deklarasinya.
    // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
    // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `private const string
    // InsertEventSql = ”””`.
    // Baris literal 2: INSERT INTO menetapkan tabel dan kolom tujuan penambahan rekaman: `insert into events (`.
    // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `event_pk,`.
    // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `event_id,`.
    // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `session_id,`.
    // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `session_player_id,`.
    // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `user_id,`.
    // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `actor_type,`.
    // Baris literal 9: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `timestamp,`.
    // Baris literal 10: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `day_index,`.
    // Baris literal 11: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `weekday,`.
    // Baris literal 12: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `turn_number,`.
    // Baris literal 13: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `action_slot,`.
    // Baris literal 14: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `sequence_number,`.
    // Baris literal 15: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_action_id,`.
    // Baris literal 16: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `action_type,`.
    // Baris literal 17: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_version_id,`.
    // Baris literal 18: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `payload_version,`.
    // Baris literal 19: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `payload,`.
    // Baris literal 20: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `received_at,`.
    // Baris literal 21: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `client_request_id`.
    // Baris literal 22: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
    // Baris literal 23: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
    // Baris literal 24: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
    // SQL: `@EventPk,`.
    // Baris literal 25: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
    // SQL: `@EventId,`.
    // Baris literal 26: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
    // SQL: `@SessionId,`.
    // Baris literal 27: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
    // SQL: `@SessionPlayerId,`.
    // Baris literal 28: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
    // SQL: `@UserId,`.
    // Baris literal 29: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
    // SQL: `@ActorType,`.
    // Baris literal 30: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
    // SQL: `@Timestamp,`.
    // Baris literal 31: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
    // SQL: `@DayIndex,`.
    // Baris literal 32: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
    // SQL: `@Weekday,`.
    // Baris literal 33: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
    // SQL: `@TurnNumber,`.
    // Baris literal 34: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
    // SQL: `@ActionSlot,`.
    // Baris literal 35: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
    // SQL: `@SequenceNumber,`.
    // Baris literal 36: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ra.ruleset_action_id,`.
    // Baris literal 37: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
    // SQL: `@ActionType,`.
    // Baris literal 38: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
    // SQL: `@RulesetVersionId,`.
    // Baris literal 39: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
    // SQL: `@PayloadVersion,`.
    // Baris literal 40: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
    // SQL: `@Payload::jsonb,`.
    // Baris literal 41: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
    // SQL: `@ReceivedAt,`.
    // Baris literal 42: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
    // SQL: `@ClientRequestId`.
    // Baris literal 43: FROM memilih tabel/subquery sumber pembacaan: `from ruleset_actions ra`.
    // Baris literal 44: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where ra.ruleset_version_id = @RulesetVersionId`.
    // Baris literal 45: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and lower(ra.action_id) = lower(@ActionId)`.
    // Baris literal 46: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and ra.is_active`.
    // Baris literal 47: LIMIT membatasi jumlah baris yang dikembalikan query: `limit 1`.
    // Baris literal 48: RETURNING mengembalikan kolom dari baris yang baru ditambahkan/diubah: `returning ruleset_action_id`.
    // Baris literal 49: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
    private const string InsertEventSql = """
        insert into events (
            event_pk,
            event_id,
            session_id,
            session_player_id,
            user_id,
            actor_type,
            timestamp,
            day_index,
            weekday,
            turn_number,
            action_slot,
            sequence_number,
            ruleset_action_id,
            action_type,
            ruleset_version_id,
            payload_version,
            payload,
            received_at,
            client_request_id
        )
        select
            @EventPk,
            @EventId,
            @SessionId,
            @SessionPlayerId,
            @UserId,
            @ActorType,
            @Timestamp,
            @DayIndex,
            @Weekday,
            @TurnNumber,
            @ActionSlot,
            @SequenceNumber,
            ra.ruleset_action_id,
            @ActionType,
            @RulesetVersionId,
            @PayloadVersion,
            @Payload::jsonb,
            @ReceivedAt,
            @ClientRequestId
        from ruleset_actions ra
        where ra.ruleset_version_id = @RulesetVersionId
          and lower(ra.action_id) = lower(@ActionId)
          and ra.is_active
        limit 1
        returning ruleset_action_id
        """;

    // Mendefinisikan metode `InsertEventAsync` dengan hasil bertipe `Task`; operasi ini menangani insert event asinkron. async memungkinkan metode
    // menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `record` bertipe `EventDb` membawa nilai
    // rekaman; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan
    // permintaan atau aplikasi berhenti.
    public async Task InsertEventAsync(EventDb record, CancellationToken ct)
    // Membuka scope metode InsertEventAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam InsertEventAsync.
    {
        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Menyiapkan variabel lokal `rulesetActionId` untuk nilai aturan aksi identitas dengan hasil operasi asinkron membaca satu hasil basis data melalui
        // `conn.QuerySingleOrDefaultAsync<Guid?>` dengan `new CommandDefinition(InsertEventSql, BuildEventParameters(record), cancellationToken: ct)`;
        // nilai default menunjukkan tidak ada baris hasil; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var rulesetActionId = await conn.QuerySingleOrDefaultAsync<Guid?>(
            // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen (InsertEventSql, BuildEventParameters(record), cancellationToken: ct) sebagai
            // argumen ke `conn.QuerySingleOrDefaultAsync<Guid?>`; Meneruskan `InsertEventSql` (nilai insert event SQL) sebagai argumen ke konstruktor
            // `CommandDefinition`; Meneruskan memanggil `BuildEventParameters` dengan `record` sebagai argumen ke konstruktor `CommandDefinition`; Meneruskan
            // `record` (nilai rekaman) sebagai argumen ke `BuildEventParameters`; Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika
            // pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen bernama `cancellationToken`.
            new CommandDefinition(InsertEventSql, BuildEventParameters(record), cancellationToken: ct));
        // Memperbarui `record.RulesetActionId` menggunakan memanggil `EnsureActionResolved` dengan `rulesetActionId`, `record` dalam InsertEventAsync.
        record.RulesetActionId = EnsureActionResolved(rulesetActionId, record);
    // Menutup scope metode InsertEventAsync; bagian berikut berada di luar batas blok tersebut dalam InsertEventAsync.
    }

    /// <summary>
    /// Menyisipkan event menggunakan koneksi dan transaksi yang sudah ada (untuk atomisitas dengan proyeksi).
    /// </summary>
    // Mendefinisikan metode `InsertEventAsync` dengan hasil bertipe `Task`. Menyisipkan event menggunakan koneksi dan transaksi yang sudah ada (untuk
    // atomisitas dengan proyeksi). async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan:
    // Parameter `record` bertipe `EventDb` membawa nilai rekaman; Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim
    // perintah dan membaca hasil basis data; Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan
    // sebagai satu kesatuan; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil
    // membatalkan permintaan atau aplikasi berhenti.
    internal async Task InsertEventAsync(EventDb record, NpgsqlConnection conn, NpgsqlTransaction tx, CancellationToken ct)
    // Membuka scope metode InsertEventAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam InsertEventAsync.
    {
        // Menyiapkan variabel lokal `rulesetActionId` untuk nilai aturan aksi identitas dengan hasil operasi asinkron membaca satu hasil basis data melalui
        // `conn.QuerySingleOrDefaultAsync<Guid?>` dengan `new CommandDefinition(InsertEventSql, BuildEventParameters(record), tx, cancellationToken: ct)`;
        // nilai default menunjukkan tidak ada baris hasil; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var rulesetActionId = await conn.QuerySingleOrDefaultAsync<Guid?>(
            // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen (InsertEventSql, BuildEventParameters(record), tx, cancellationToken: ct)
            // sebagai argumen ke `conn.QuerySingleOrDefaultAsync<Guid?>`; Meneruskan `InsertEventSql` (nilai insert event SQL) sebagai argumen ke konstruktor
            // `CommandDefinition`; Meneruskan memanggil `BuildEventParameters` dengan `record` sebagai argumen ke konstruktor `CommandDefinition`; Meneruskan
            // `record` (nilai rekaman) sebagai argumen ke `BuildEventParameters`; Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai
            // satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`; Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika
            // pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen bernama `cancellationToken`.
            new CommandDefinition(InsertEventSql, BuildEventParameters(record), tx, cancellationToken: ct));
        // Memperbarui `record.RulesetActionId` menggunakan memanggil `EnsureActionResolved` dengan `rulesetActionId`, `record` dalam InsertEventAsync.
        record.RulesetActionId = EnsureActionResolved(rulesetActionId, record);
    // Menutup scope metode InsertEventAsync; bagian berikut berada di luar batas blok tersebut dalam InsertEventAsync.
    }

    // Mendefinisikan metode `InsertEventAssetReferencesAsync` dengan hasil bertipe `Task`; operasi ini menangani insert event aset references asinkron.
    // async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `record` bertipe
    // `EventDb` membawa nilai rekaman; Parameter `references` bertipe `IReadOnlyCollection<EventAssetReferenceInput>` membawa nilai references;
    // Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data; Parameter `tx`
    // bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan; Parameter `ct` bertipe
    // `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    internal async Task InsertEventAssetReferencesAsync(
        // Parameter `record` bertipe `EventDb` membawa nilai rekaman.
        EventDb record,
        // Parameter `references` bertipe `IReadOnlyCollection<EventAssetReferenceInput>` membawa nilai references.
        IReadOnlyCollection<EventAssetReferenceInput> references,
        // Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data.
        NpgsqlConnection conn,
        // Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan.
        NpgsqlTransaction tx,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    // Membuka scope metode InsertEventAssetReferencesAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // InsertEventAssetReferencesAsync.
    {
        // Memeriksa perbandingan kesamaan antara `references.Count` dan `0`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // InsertEventAssetReferencesAsync.
        if (references.Count == 0)
        // Membuka scope cabang if untuk kondisi `references.Count == 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // InsertEventAssetReferencesAsync.
        {
            // Mengakhiri eksekusi lebih awal dalam InsertEventAssetReferencesAsync tanpa mengembalikan nilai; pernyataan sesudah return pada jalur ini tidak
            // dijalankan.
            return;
        // Menutup scope cabang if untuk kondisi `references.Count == 0`; bagian berikut berada di luar batas blok tersebut dalam
        // InsertEventAssetReferencesAsync.
        }

        // Menyiapkan variabel lokal `resolveSql` untuk nilai resolve SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe
        // yang dipakai adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string resolveSql =
        // ”””`.
        // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select asset.ruleset_game_asset_id`.
        // Baris literal 3: FROM memilih tabel/subquery sumber pembacaan: `from ruleset_game_assets asset`.
        // Baris literal 4: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where asset.ruleset_version_id = @rulesetVersionId`.
        // Baris literal 5: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and asset.asset_type = @assetType`.
        // Baris literal 6: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and (`.
        // Baris literal 7: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `lower(asset.asset_code) = lower(@assetCode)`.
        // Baris literal 8: Melanjutkan kondisi SQL dengan OR (alternatif syarat yang dapat terpenuhi): `or lower(asset.display_name) = lower(@assetCode)`.
        // Baris literal 9: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 10: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and asset.is_active`.
        // Baris literal 11: LIMIT membatasi jumlah baris yang dikembalikan query: `limit 1`.
        // Baris literal 12: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string resolveSql = """
            select asset.ruleset_game_asset_id
            from ruleset_game_assets asset
            where asset.ruleset_version_id = @rulesetVersionId
              and asset.asset_type = @assetType
              and (
                lower(asset.asset_code) = lower(@assetCode)
                or lower(asset.display_name) = lower(@assetCode)
              )
              and asset.is_active
            limit 1
            """;

        // Menyiapkan variabel lokal `insertSql` untuk nilai insert SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang
        // dipakai adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string insertSql =
        // ”””`.
        // Baris literal 2: INSERT INTO menetapkan tabel dan kolom tujuan penambahan rekaman: `insert into event_asset_references (`.
        // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `event_asset_reference_id,`.
        // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `session_id,`.
        // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `event_id,`.
        // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_version_id,`.
        // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_game_asset_id,`.
        // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `reference_role,`.
        // Baris literal 9: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `payload_path,`.
        // Baris literal 10: CREATE mendefinisikan objek basis data yang diperlukan proses ini: `created_at`.
        // Baris literal 11: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 12: VALUES menyediakan nilai baris baru sesuai urutan kolom INSERT; placeholder @ diikat ke parameter perintah: `values (`.
        // Baris literal 13: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@referenceId,`.
        // Baris literal 14: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@sessionId,`.
        // Baris literal 15: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@eventId,`.
        // Baris literal 16: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@rulesetVersionId,`.
        // Baris literal 17: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@rulesetGameAssetId,`.
        // Baris literal 18: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@referenceRole,`.
        // Baris literal 19: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@payloadPath,`.
        // Baris literal 20: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `now()`.
        // Baris literal 21: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 22: ON CONFLICT menentukan penanganan saat INSERT bertabrakan dengan kunci unik yang sudah ada: `on conflict (session_id, event_id,
        // ruleset_game_asset_id, reference_role) do nothing`.
        // Baris literal 23: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string insertSql = """
            insert into event_asset_references (
                event_asset_reference_id,
                session_id,
                event_id,
                ruleset_version_id,
                ruleset_game_asset_id,
                reference_role,
                payload_path,
                created_at
            )
            values (
                @referenceId,
                @sessionId,
                @eventId,
                @rulesetVersionId,
                @rulesetGameAssetId,
                @referenceRole,
                @payloadPath,
                now()
            )
            on conflict (session_id, event_id, ruleset_game_asset_id, reference_role) do nothing
            """;

        // Mengulangi setiap elemen `references .GroupBy(reference => new { AssetType = reference.AssetType.Trim().ToUpperInvariant(), AssetCode =
        // reference.AssetCode.Trim().ToUpperInvariant(), ReferenceRole = re...`; elemen saat ini disimpan sebagai `reference` bertipe `var` untuk diproses
        // oleh badan loop dalam InsertEventAssetReferencesAsync.
        foreach (var reference in references
                     // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .GroupBy(reference => new dalam InsertEventAssetReferencesAsync; token pada
                     // baris ini menyambungkan bagian kode sebelum dan sesudahnya.
                     .GroupBy(reference => new
                     // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                     // InsertEventAssetReferencesAsync.
                     {
                         // Meneruskan fungsi lambda `reference => new { AssetType = reference.AssetType.Trim().ToUpperInvariant(), AssetCode =
                         // reference.AssetCode.Trim().ToUpperInvariant(), ReferenceRole = reference.ReferenceRol...` yang dijalankan oleh operasi pemanggil untuk memproses
                         // setiap masukan sebagai argumen ke `references .GroupBy`.
                         AssetType = reference.AssetType.Trim().ToUpperInvariant(),
                         // Meneruskan fungsi lambda `reference => new { AssetType = reference.AssetType.Trim().ToUpperInvariant(), AssetCode =
                         // reference.AssetCode.Trim().ToUpperInvariant(), ReferenceRole = reference.ReferenceRol...` yang dijalankan oleh operasi pemanggil untuk memproses
                         // setiap masukan sebagai argumen ke `references .GroupBy`.
                         AssetCode = reference.AssetCode.Trim().ToUpperInvariant(),
                         // Meneruskan fungsi lambda `reference => new { AssetType = reference.AssetType.Trim().ToUpperInvariant(), AssetCode =
                         // reference.AssetCode.Trim().ToUpperInvariant(), ReferenceRole = reference.ReferenceRol...` yang dijalankan oleh operasi pemanggil untuk memproses
                         // setiap masukan sebagai argumen ke `references .GroupBy`.
                         ReferenceRole = reference.ReferenceRole.Trim().ToUpperInvariant()
                     // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
                     // InsertEventAssetReferencesAsync.
                     })
                     // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Select(group => group.First())) dalam InsertEventAssetReferencesAsync; token
                     // pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
                     .Select(group => group.First()))
        // Membuka scope loop setiap reference dari `references .GroupBy(reference => new { AssetType = reference.AssetType.Trim().ToUpperInvariant(),
        // AssetCode = reference.AssetCode.Trim().ToUpperInvariant(), ReferenceRole = re...`; pernyataan/deklarasi berikut berada di dalam batas blok ini
        // dalam InsertEventAssetReferencesAsync.
        {
            // Menyiapkan variabel lokal `rulesetGameAssetId` untuk nilai aturan game aset identitas dengan hasil operasi asinkron membaca satu hasil basis data
            // melalui `conn.QuerySingleOrDefaultAsync<Guid?>` dengan `new CommandDefinition( resolveSql, new { rulesetVersionId = record.RulesetVersionId,
            // reference.AssetType, reference.AssetCode }, tx, cancellationToken: ct)`; nilai default menunjukkan tidak ada baris hasil; await menunggu hasil
            // tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var rulesetGameAssetId = await conn.QuerySingleOrDefaultAsync<Guid?>(
                // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen ( resolveSql, new { rulesetVersionId = record.RulesetVersionId,
                // reference.AssetType, reference.AssetCode }, tx, cancellationToken: ct) sebagai argumen ke `conn.QuerySingleOrDefaultAsync<Guid?>`.
                new CommandDefinition(
                    // Meneruskan `resolveSql` (nilai resolve SQL) sebagai argumen ke konstruktor `CommandDefinition`.
                    resolveSql,
                    // Meneruskan objek anonim yang mengelompokkan rulesetVersionId, reference.AssetType, reference.AssetCode sebagai satu nilai sebagai argumen ke
                    // konstruktor `CommandDefinition`.
                    new
                    // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                    // InsertEventAssetReferencesAsync.
                    {
                        // Meneruskan objek anonim yang mengelompokkan rulesetVersionId, reference.AssetType, reference.AssetCode sebagai satu nilai sebagai argumen ke
                        // konstruktor `CommandDefinition`.
                        rulesetVersionId = record.RulesetVersionId,
                        // Meneruskan objek anonim yang mengelompokkan rulesetVersionId, reference.AssetType, reference.AssetCode sebagai satu nilai sebagai argumen ke
                        // konstruktor `CommandDefinition`.
                        reference.AssetType,
                        // Meneruskan objek anonim yang mengelompokkan rulesetVersionId, reference.AssetType, reference.AssetCode sebagai satu nilai sebagai argumen ke
                        // konstruktor `CommandDefinition`.
                        reference.AssetCode
                    // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
                    // InsertEventAssetReferencesAsync.
                    },
                    // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
                    tx,
                    // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                    // bernama `cancellationToken`.
                    cancellationToken: ct));

            // Memeriksa kebalikan kondisi `rulesetGameAssetId.HasValue`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
            // InsertEventAssetReferencesAsync.
            if (!rulesetGameAssetId.HasValue)
            // Membuka scope cabang if untuk kondisi `!rulesetGameAssetId.HasValue`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // InsertEventAssetReferencesAsync.
            {
                // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen ( $”Asset
                // '{reference.AssetType}/{reference.AssetCode}' tidak ditemukan pada ruleset event.”) dalam InsertEventAssetReferencesAsync; pemanggil atau
                // middleware penanganan error menerima kegagalan ini.
                throw new InvalidOperationException(
                    // Meneruskan teks interpolasi `$”Asset '{reference.AssetType}/{reference.AssetCode}' tidak ditemukan pada ruleset event.”`; nilai ekspresi di dalam
                    // kurung kurawal disisipkan saat program berjalan sebagai argumen ke konstruktor `InvalidOperationException`.
                    $"Asset '{reference.AssetType}/{reference.AssetCode}' tidak ditemukan pada ruleset event.");
            // Menutup scope cabang if untuk kondisi `!rulesetGameAssetId.HasValue`; bagian berikut berada di luar batas blok tersebut dalam
            // InsertEventAssetReferencesAsync.
            }

            // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition( insertSql, new { referenceId =
            // Guid.NewGuid(), sessionId = record.SessionId, eventId = record.EventId, rulesetVersionId = record.RulesetVersionId, rules...`; nilai hasil
            // menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam
            // InsertEventAssetReferencesAsync.
            await conn.ExecuteAsync(new CommandDefinition(
                // Meneruskan `insertSql` (nilai insert SQL) sebagai argumen ke konstruktor `CommandDefinition`.
                insertSql,
                // Meneruskan objek anonim yang mengelompokkan referenceId, sessionId, eventId, rulesetVersionId, rulesetGameAssetId, reference.ReferenceRole,
                // reference.PayloadPath sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                new
                // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // InsertEventAssetReferencesAsync.
                {
                    // Meneruskan objek anonim yang mengelompokkan referenceId, sessionId, eventId, rulesetVersionId, rulesetGameAssetId, reference.ReferenceRole,
                    // reference.PayloadPath sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                    referenceId = Guid.NewGuid(),
                    // Meneruskan objek anonim yang mengelompokkan referenceId, sessionId, eventId, rulesetVersionId, rulesetGameAssetId, reference.ReferenceRole,
                    // reference.PayloadPath sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                    sessionId = record.SessionId,
                    // Meneruskan objek anonim yang mengelompokkan referenceId, sessionId, eventId, rulesetVersionId, rulesetGameAssetId, reference.ReferenceRole,
                    // reference.PayloadPath sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                    eventId = record.EventId,
                    // Meneruskan objek anonim yang mengelompokkan referenceId, sessionId, eventId, rulesetVersionId, rulesetGameAssetId, reference.ReferenceRole,
                    // reference.PayloadPath sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                    rulesetVersionId = record.RulesetVersionId,
                    // Meneruskan objek anonim yang mengelompokkan referenceId, sessionId, eventId, rulesetVersionId, rulesetGameAssetId, reference.ReferenceRole,
                    // reference.PayloadPath sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                    rulesetGameAssetId = rulesetGameAssetId.Value,
                    // Meneruskan objek anonim yang mengelompokkan referenceId, sessionId, eventId, rulesetVersionId, rulesetGameAssetId, reference.ReferenceRole,
                    // reference.PayloadPath sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                    reference.ReferenceRole,
                    // Meneruskan objek anonim yang mengelompokkan referenceId, sessionId, eventId, rulesetVersionId, rulesetGameAssetId, reference.ReferenceRole,
                    // reference.PayloadPath sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                    reference.PayloadPath
                // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
                // InsertEventAssetReferencesAsync.
                },
                // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
                tx,
                // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                // bernama `cancellationToken`.
                cancellationToken: ct));
        // Menutup scope loop setiap reference dari `references .GroupBy(reference => new { AssetType = reference.AssetType.Trim().ToUpperInvariant(),
        // AssetCode = reference.AssetCode.Trim().ToUpperInvariant(), ReferenceRole = re...`; bagian berikut berada di luar batas blok tersebut dalam
        // InsertEventAssetReferencesAsync.
        }
    // Menutup scope metode InsertEventAssetReferencesAsync; bagian berikut berada di luar batas blok tersebut dalam InsertEventAssetReferencesAsync.
    }

    /// <summary>
    /// Menyimpan catatan validasi gagal. Event valid hanya masuk stream events.
    /// </summary>
    // Mendefinisikan metode `InsertValidationLogAsync` dengan hasil bertipe `Task`. Menyimpan catatan validasi gagal. Event valid hanya masuk stream
    // events. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `sessionId`
    // bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini; Parameter `eventId` bertipe `Guid` membawa identitas
    // unik event untuk pencatatan dan pemeriksaan duplikasi; Parameter `rulesetVersionId` bertipe `Guid?` membawa identitas versi aturan sehingga
    // perhitungan memakai konfigurasi aturan yang tepat; nilai null diizinkan ketika data opsional belum tersedia; Parameter `errorCode` bertipe
    // `string?` membawa nilai kesalahan kode; nilai null diizinkan ketika data opsional belum tersedia; Parameter `errorMessage` bertipe `string?`
    // membawa nilai kesalahan pesan; nilai null diizinkan ketika data opsional belum tersedia; Parameter `statusCode` bertipe `int` membawa kode status
    // hasil HTTP yang mengomunikasikan keberhasilan atau kegagalan; Parameter `traceId` bertipe `string` membawa identitas penelusuran yang
    // menghubungkan respons, log, dan permintaan yang sama; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat
    // dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task InsertValidationLogAsync(
        // Parameter `sessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini.
        Guid sessionId,
        // Parameter `eventId` bertipe `Guid` membawa identitas unik event untuk pencatatan dan pemeriksaan duplikasi.
        Guid eventId,
        // Parameter `rulesetVersionId` bertipe `Guid?` membawa identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang tepat; nilai
        // null diizinkan ketika data opsional belum tersedia.
        Guid? rulesetVersionId,
        // Parameter `errorCode` bertipe `string?` membawa nilai kesalahan kode; nilai null diizinkan ketika data opsional belum tersedia.
        string? errorCode,
        // Parameter `errorMessage` bertipe `string?` membawa nilai kesalahan pesan; nilai null diizinkan ketika data opsional belum tersedia.
        string? errorMessage,
        // Parameter `statusCode` bertipe `int` membawa kode status hasil HTTP yang mengomunikasikan keberhasilan atau kegagalan.
        int statusCode,
        // Parameter `traceId` bertipe `string` membawa identitas penelusuran yang menghubungkan respons, log, dan permintaan yang sama.
        string traceId,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    // Membuka scope metode InsertValidationLogAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam InsertValidationLogAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai
        // adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string sql = ”””`.
        // Baris literal 2: INSERT INTO menetapkan tabel dan kolom tujuan penambahan rekaman: `insert into validation_logs (`.
        // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `validation_log_id,`.
        // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `session_id,`.
        // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_version_id,`.
        // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `event_id,`.
        // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `raw_payload_json,`.
        // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `error_code,`.
        // Baris literal 9: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `error_message,`.
        // Baris literal 10: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `status_code,`.
        // Baris literal 11: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `trace_id,`.
        // Baris literal 12: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `details_json,`.
        // Baris literal 13: CREATE mendefinisikan objek basis data yang diperlukan proses ini: `created_at`.
        // Baris literal 14: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 15: VALUES menyediakan nilai baris baru sesuai urutan kolom INSERT; placeholder @ diikat ke parameter perintah: `values (`.
        // Baris literal 16: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@validationLogId,`.
        // Baris literal 17: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@sessionId,`.
        // Baris literal 18: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@rulesetVersionId,`.
        // Baris literal 19: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@eventId,`.
        // Baris literal 20: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `'{}'::jsonb,`.
        // Baris literal 21: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@errorCode,`.
        // Baris literal 22: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@errorMessage,`.
        // Baris literal 23: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@statusCode,`.
        // Baris literal 24: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@traceId,`.
        // Baris literal 25: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `'{}'::jsonb,`.
        // Baris literal 26: CREATE mendefinisikan objek basis data yang diperlukan proses ini: `@createdAt`.
        // Baris literal 27: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 28: ON CONFLICT menentukan penanganan saat INSERT bertabrakan dengan kunci unik yang sudah ada: `on conflict (session_id, event_id)
        // do nothing`.
        // Baris literal 29: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string sql = """
            insert into validation_logs (
                validation_log_id,
                session_id,
                ruleset_version_id,
                event_id,
                raw_payload_json,
                error_code,
                error_message,
                status_code,
                trace_id,
                details_json,
                created_at
            )
            values (
                @validationLogId,
                @sessionId,
                @rulesetVersionId,
                @eventId,
                '{}'::jsonb,
                @errorCode,
                @errorMessage,
                @statusCode,
                @traceId,
                '{}'::jsonb,
                @createdAt
            )
            on conflict (session_id, event_id) do nothing
            """;

        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition(sql, new { validationLogId =
        // Guid.NewGuid(), sessionId, eventId, rulesetVersionId, errorCode, errorMessage, statusCode, traceId = string.IsNullOrWhiteSpa...`; nilai hasil
        // menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam
        // InsertValidationLogAsync.
        await conn.ExecuteAsync(new CommandDefinition(sql, new
        // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // InsertValidationLogAsync.
        {
            // Meneruskan objek anonim yang mengelompokkan validationLogId, sessionId, eventId, rulesetVersionId, errorCode, errorMessage, statusCode, traceId,
            // createdAt sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
            validationLogId = Guid.NewGuid(),
            // Meneruskan objek anonim yang mengelompokkan validationLogId, sessionId, eventId, rulesetVersionId, errorCode, errorMessage, statusCode, traceId,
            // createdAt sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
            sessionId,
            // Meneruskan objek anonim yang mengelompokkan validationLogId, sessionId, eventId, rulesetVersionId, errorCode, errorMessage, statusCode, traceId,
            // createdAt sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
            eventId,
            // Meneruskan objek anonim yang mengelompokkan validationLogId, sessionId, eventId, rulesetVersionId, errorCode, errorMessage, statusCode, traceId,
            // createdAt sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
            rulesetVersionId,
            // Meneruskan objek anonim yang mengelompokkan validationLogId, sessionId, eventId, rulesetVersionId, errorCode, errorMessage, statusCode, traceId,
            // createdAt sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
            errorCode,
            // Meneruskan objek anonim yang mengelompokkan validationLogId, sessionId, eventId, rulesetVersionId, errorCode, errorMessage, statusCode, traceId,
            // createdAt sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
            errorMessage,
            // Meneruskan objek anonim yang mengelompokkan validationLogId, sessionId, eventId, rulesetVersionId, errorCode, errorMessage, statusCode, traceId,
            // createdAt sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
            statusCode,
            // Meneruskan `traceId` (identitas penelusuran yang menghubungkan respons, log, dan permintaan yang sama) sebagai argumen ke
            // `string.IsNullOrWhiteSpace`.
            traceId = string.IsNullOrWhiteSpace(traceId) ? "unknown" : traceId,
            // Meneruskan objek anonim yang mengelompokkan validationLogId, sessionId, eventId, rulesetVersionId, errorCode, errorMessage, statusCode, traceId,
            // createdAt sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
            createdAt = DateTimeOffset.UtcNow
        // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam InsertValidationLogAsync.
        }, cancellationToken: ct));
    // Menutup scope metode InsertValidationLogAsync; bagian berikut berada di luar batas blok tersebut dalam InsertValidationLogAsync.
    }

    /// <summary>
    /// SQL INSERT untuk tabel event_cashflow_projections, digunakan oleh kedua overload.
    /// </summary>
    // Mendeklarasikan field bertipe `string`: `InsertProjectionSql` menyimpan nilai insert projection SQL dengan nilai awal literal multiline yang
    // dirinci pada komentar di dekat deklarasinya.
    // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
    // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `private const string
    // InsertProjectionSql = ”””`.
    // Baris literal 2: INSERT INTO menetapkan tabel dan kolom tujuan penambahan rekaman: `insert into event_cashflow_projections (`.
    // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `projection_id,`.
    // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `session_id,`.
    // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `user_id,`.
    // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `event_pk,`.
    // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `event_id,`.
    // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `projection_order,`.
    // Baris literal 9: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `timestamp,`.
    // Baris literal 10: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `direction,`.
    // Baris literal 11: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `amount,`.
    // Baris literal 12: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `category,`.
    // Baris literal 13: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `counterparty,`.
    // Baris literal 14: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `reference,`.
    // Baris literal 15: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `note`.
    // Baris literal 16: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
    // Baris literal 17: VALUES menyediakan nilai baris baru sesuai urutan kolom INSERT; placeholder @ diikat ke parameter perintah: `values (`.
    // Baris literal 18: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
    // SQL: `@ProjectionId,`.
    // Baris literal 19: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
    // SQL: `@SessionId,`.
    // Baris literal 20: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
    // SQL: `@UserId,`.
    // Baris literal 21: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
    // SQL: `@EventPk,`.
    // Baris literal 22: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
    // SQL: `@EventId,`.
    // Baris literal 23: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
    // SQL: `@ProjectionOrder,`.
    // Baris literal 24: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
    // SQL: `@Timestamp,`.
    // Baris literal 25: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
    // SQL: `@Direction,`.
    // Baris literal 26: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
    // SQL: `@Amount,`.
    // Baris literal 27: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
    // SQL: `@Category,`.
    // Baris literal 28: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
    // SQL: `@Counterparty,`.
    // Baris literal 29: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
    // SQL: `@Reference,`.
    // Baris literal 30: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
    // SQL: `@Note`.
    // Baris literal 31: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
    // Baris literal 32: ON CONFLICT menentukan penanganan saat INSERT bertabrakan dengan kunci unik yang sudah ada: `on conflict (session_id, event_id,
    // projection_order) do nothing`.
    // Baris literal 33: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
    private const string InsertProjectionSql = """
        insert into event_cashflow_projections (
            projection_id,
            session_id,
            user_id,
            event_pk,
            event_id,
            projection_order,
            timestamp,
            direction,
            amount,
            category,
            counterparty,
            reference,
            note
        )
        values (
            @ProjectionId,
            @SessionId,
            @UserId,
            @EventPk,
            @EventId,
            @ProjectionOrder,
            @Timestamp,
            @Direction,
            @Amount,
            @Category,
            @Counterparty,
            @Reference,
            @Note
        )
        on conflict (session_id, event_id, projection_order) do nothing
        """;

    /// <summary>
    /// Menyimpan proyeksi arus kas (cashflow projection) yang dihasilkan dari satu event.
    /// </summary>
    // Mendefinisikan metode `InsertCashflowProjectionAsync` dengan hasil bertipe `Task`. Menyimpan proyeksi arus kas (cashflow projection) yang
    // dihasilkan dari satu event. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan:
    // Parameter `projection` bertipe `CashflowProjectionDb` membawa nilai projection; Parameter `ct` bertipe `CancellationToken` membawa sinyal
    // pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task InsertCashflowProjectionAsync(CashflowProjectionDb projection, CancellationToken ct)
    // Membuka scope metode InsertCashflowProjectionAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // InsertCashflowProjectionAsync.
    {
        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition(InsertProjectionSql, projection,
        // cancellationToken: ct)`; nilai hasil menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum
        // selesai dalam InsertCashflowProjectionAsync.
        await conn.ExecuteAsync(new CommandDefinition(InsertProjectionSql, projection, cancellationToken: ct));
    // Menutup scope metode InsertCashflowProjectionAsync; bagian berikut berada di luar batas blok tersebut dalam InsertCashflowProjectionAsync.
    }

    /// <summary>
    /// Menyisipkan proyeksi arus kas menggunakan koneksi dan transaksi yang sudah ada.
    /// </summary>
    // Mendefinisikan metode `InsertCashflowProjectionAsync` dengan hasil bertipe `Task`. Menyisipkan proyeksi arus kas menggunakan koneksi dan
    // transaksi yang sudah ada. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan:
    // Parameter `projection` bertipe `CashflowProjectionDb` membawa nilai projection; Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi
    // PostgreSQL untuk mengirim perintah dan membaca hasil basis data; Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang
    // menggabungkan perubahan sebagai satu kesatuan; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan
    // ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    internal async Task InsertCashflowProjectionAsync(CashflowProjectionDb projection, NpgsqlConnection conn, NpgsqlTransaction tx, CancellationToken ct)
    // Membuka scope metode InsertCashflowProjectionAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // InsertCashflowProjectionAsync.
    {
        // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition(InsertProjectionSql, projection,
        // tx, cancellationToken: ct)`; nilai hasil menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi
        // belum selesai dalam InsertCashflowProjectionAsync.
        await conn.ExecuteAsync(new CommandDefinition(InsertProjectionSql, projection, tx, cancellationToken: ct));
    // Menutup scope metode InsertCashflowProjectionAsync; bagian berikut berada di luar batas blok tersebut dalam InsertCashflowProjectionAsync.
    }

    /// <summary>
    /// Membuka koneksi database untuk digunakan dengan transaksi eksternal.
    /// </summary>
    // Mendefinisikan metode `OpenConnectionAsync` dengan hasil bertipe `Task<NpgsqlConnection>`. Membuka koneksi database untuk digunakan dengan
    // transaksi eksternal. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter
    // `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi
    // berhenti.
    internal async Task<NpgsqlConnection> OpenConnectionAsync(CancellationToken ct)
    // Membuka scope metode OpenConnectionAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam OpenConnectionAsync.
    {
        // Mengembalikan hasil operasi asinkron membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir
        // thread selama operasi belum selesai kepada pemanggil dalam OpenConnectionAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return await _dataSource.OpenConnectionAsync(ct);
    // Menutup scope metode OpenConnectionAsync; bagian berikut berada di luar batas blok tersebut dalam OpenConnectionAsync.
    }

    // Mendefinisikan metode `ResolveSessionParticipantIdAsync` dengan hasil bertipe `Task<Guid?>`; operasi ini menangani resolve sesi participant
    // identitas asinkron. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter
    // `sessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini; Parameter `userId` bertipe `Guid` membawa
    // identitas akun pengguna yang datanya sedang diproses; Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim
    // perintah dan membaca hasil basis data; Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan
    // sebagai satu kesatuan; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil
    // membatalkan permintaan atau aplikasi berhenti.
    internal async Task<Guid?> ResolveSessionParticipantIdAsync(
        // Parameter `sessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini.
        Guid sessionId,
        // Parameter `userId` bertipe `Guid` membawa identitas akun pengguna yang datanya sedang diproses.
        Guid userId,
        // Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data.
        NpgsqlConnection conn,
        // Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan.
        NpgsqlTransaction tx,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    // Membuka scope metode ResolveSessionParticipantIdAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // ResolveSessionParticipantIdAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai
        // adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string sql = ”””`.
        // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select session_participant_id`.
        // Baris literal 3: FROM memilih tabel/subquery sumber pembacaan: `from session_participants`.
        // Baris literal 4: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where session_id = @sessionId`.
        // Baris literal 5: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and user_id = @userId`.
        // Baris literal 6: LIMIT membatasi jumlah baris yang dikembalikan query: `limit 1`.
        // Baris literal 7: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string sql = """
            select session_participant_id
            from session_participants
            where session_id = @sessionId
              and user_id = @userId
            limit 1
            """;

        // Mengembalikan hasil operasi asinkron membaca satu hasil basis data melalui `conn.QuerySingleOrDefaultAsync<Guid?>` dengan `new
        // CommandDefinition(sql, new { sessionId, userId }, tx, cancellationToken: ct)`; nilai default menunjukkan tidak ada baris hasil; await menunggu
        // hasil tanpa memblokir thread selama operasi belum selesai kepada pemanggil dalam ResolveSessionParticipantIdAsync; eksekusi jalur ini selesai
        // setelah nilai hasil ditentukan.
        return await conn.QuerySingleOrDefaultAsync<Guid?>(
            // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen (sql, new { sessionId, userId }, tx, cancellationToken: ct) sebagai argumen ke
            // `conn.QuerySingleOrDefaultAsync<Guid?>`; Meneruskan `sql` (nilai SQL) sebagai argumen ke konstruktor `CommandDefinition`; Meneruskan objek anonim
            // yang mengelompokkan sessionId, userId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`; Meneruskan `tx` (transaksi basis
            // data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`; Meneruskan `ct` (sinyal pembatalan
            // agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen bernama `cancellationToken`.
            new CommandDefinition(sql, new { sessionId, userId }, tx, cancellationToken: ct));
    // Menutup scope metode ResolveSessionParticipantIdAsync; bagian berikut berada di luar batas blok tersebut dalam ResolveSessionParticipantIdAsync.
    }

    /// <summary>
    /// Mengambil seluruh proyeksi arus kas dalam satu sesi.
    /// </summary>
    // Mendefinisikan metode `GetCashflowProjectionsAsync` dengan hasil bertipe `Task<List<CashflowProjectionDb>>`. Mengambil seluruh proyeksi arus kas
    // dalam satu sesi. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter
    // `sessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini; Parameter `ct` bertipe `CancellationToken`
    // membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<List<CashflowProjectionDb>> GetCashflowProjectionsAsync(Guid sessionId, CancellationToken ct)
    // Membuka scope metode GetCashflowProjectionsAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam GetCashflowProjectionsAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai
        // adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string sql = ”””`.
        // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select projection_id,`.
        // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `session_id,`.
        // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `user_id,`.
        // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `event_pk,`.
        // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `event_id,`.
        // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `projection_order,`.
        // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `timestamp,`.
        // Baris literal 9: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `direction,`.
        // Baris literal 10: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `amount,`.
        // Baris literal 11: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `category,`.
        // Baris literal 12: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `counterparty,`.
        // Baris literal 13: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `reference,`.
        // Baris literal 14: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `note`.
        // Baris literal 15: FROM memilih tabel/subquery sumber pembacaan: `from event_cashflow_projections`.
        // Baris literal 16: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where session_id = @sessionId`.
        // Baris literal 17: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string sql = """
            select projection_id,
                   session_id,
                   user_id,
                   event_pk,
                   event_id,
                   projection_order,
                   timestamp,
                   direction,
                   amount,
                   category,
                   counterparty,
                   reference,
                   note
            from event_cashflow_projections
            where session_id = @sessionId
            """;

        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Menyiapkan variabel lokal `items` untuk nilai elemen dengan hasil operasi asinkron menjalankan query baca melalui `conn` dengan `new
        // CommandDefinition(sql, new { sessionId }, cancellationToken: ct)` dan memetakan baris hasil ke tipe yang diminta; await menunggu hasil tanpa
        // memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var items = await conn.QueryAsync<CashflowProjectionDb>(new CommandDefinition(sql, new { sessionId }, cancellationToken: ct));
        // Mengembalikan mematerialisasi urutan `items` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori kepada pemanggil dalam
        // GetCashflowProjectionsAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return items.ToList();
    // Menutup scope metode GetCashflowProjectionsAsync; bagian berikut berada di luar batas blok tersebut dalam GetCashflowProjectionsAsync.
    }

    /// <summary>
    /// Mengambil event setelah sequence number cursor dengan batas jumlah tertentu.
    /// </summary>
    // Mendefinisikan metode `GetEventsBySessionAsync` dengan hasil bertipe `Task<List<EventDb>>`. Mengambil event setelah sequence number cursor dengan
    // batas jumlah tertentu. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan:
    // Parameter `sessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini; Parameter `afterSequence` bertipe
    // `long` membawa nilai after sequence; Parameter `limit` bertipe `int` membawa batas jumlah hasil yang diminta pada satu operasi; Parameter `ct`
    // bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi
    // berhenti.
    public async Task<List<EventDb>> GetEventsBySessionAsync(Guid sessionId, long afterSequence, int limit, CancellationToken ct)
    // Membuka scope metode GetEventsBySessionAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam GetEventsBySessionAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan penjumlahan/penggabungan antara `EventSelectColumns` dan `””” where session_id =
        // @sessionId and sequence_number > @afterSequence order by sequence_number limit @limit ”””`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `var sql = EventSelectColumns
        // + ”””`.
        // Baris literal 2: Pembatas literal/penutup ``; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 3: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where session_id = @sessionId`.
        // Baris literal 4: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and sequence_number > @afterSequence`.
        // Baris literal 5: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by sequence_number`.
        // Baris literal 6: LIMIT membatasi jumlah baris yang dikembalikan query: `limit @limit`.
        // Baris literal 7: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        var sql = EventSelectColumns + """

            where session_id = @sessionId
              and sequence_number > @afterSequence
            order by sequence_number
            limit @limit
            """;

        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Menyiapkan variabel lokal `items` untuk nilai elemen dengan hasil operasi asinkron menjalankan query baca melalui `conn` dengan `new
        // CommandDefinition(sql, new { sessionId, afterSequence, limit }, cancellationToken: ct)` dan memetakan baris hasil ke tipe yang diminta; await
        // menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var items = await conn.QueryAsync<EventDb>(new CommandDefinition(sql, new { sessionId, afterSequence, limit }, cancellationToken: ct));
        // Mengembalikan mematerialisasi urutan `items` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori kepada pemanggil dalam
        // GetEventsBySessionAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return items.ToList();
    // Menutup scope metode GetEventsBySessionAsync; bagian berikut berada di luar batas blok tersebut dalam GetEventsBySessionAsync.
    }

    // Mendefinisikan metode `GetCashflowProjectionPageAsync` dengan hasil bertipe `Task<List<CashflowProjectionDb>>`; operasi ini menangani get arus
    // kas projection page asinkron. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan:
    // Parameter `sessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini; Parameter `userId` bertipe
    // `Guid?` membawa identitas akun pengguna yang datanya sedang diproses; nilai null diizinkan ketika data opsional belum tersedia; Parameter
    // `afterTimestamp` bertipe `DateTimeOffset?` membawa nilai after timestamp; nilai null diizinkan ketika data opsional belum tersedia; Parameter
    // `afterTransactionId` bertipe `Guid?` membawa nilai after transaction identitas; nilai null diizinkan ketika data opsional belum tersedia;
    // Parameter `limit` bertipe `int` membawa batas jumlah hasil yang diminta pada satu operasi; Parameter `ct` bertipe `CancellationToken` membawa
    // sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<List<CashflowProjectionDb>> GetCashflowProjectionPageAsync(
        // Parameter `sessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini.
        Guid sessionId,
        // Parameter `userId` bertipe `Guid?` membawa identitas akun pengguna yang datanya sedang diproses; nilai null diizinkan ketika data opsional belum
        // tersedia.
        Guid? userId,
        // Parameter `afterTimestamp` bertipe `DateTimeOffset?` membawa nilai after timestamp; nilai null diizinkan ketika data opsional belum tersedia.
        DateTimeOffset? afterTimestamp,
        // Parameter `afterTransactionId` bertipe `Guid?` membawa nilai after transaction identitas; nilai null diizinkan ketika data opsional belum
        // tersedia.
        Guid? afterTransactionId,
        // Parameter `limit` bertipe `int` membawa batas jumlah hasil yang diminta pada satu operasi.
        int limit,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    // Membuka scope metode GetCashflowProjectionPageAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // GetCashflowProjectionPageAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai
        // adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string sql = ”””`.
        // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select projection_id,`.
        // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `session_id,`.
        // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `user_id,`.
        // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `event_pk,`.
        // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `event_id,`.
        // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `projection_order,`.
        // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `timestamp,`.
        // Baris literal 9: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `direction,`.
        // Baris literal 10: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `amount,`.
        // Baris literal 11: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `category,`.
        // Baris literal 12: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `counterparty,`.
        // Baris literal 13: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `reference,`.
        // Baris literal 14: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `note`.
        // Baris literal 15: FROM memilih tabel/subquery sumber pembacaan: `from event_cashflow_projections`.
        // Baris literal 16: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where session_id = @sessionId`.
        // Baris literal 17: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and (@userId is null or user_id = @userId)`.
        // Baris literal 18: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and (@afterTimestamp is null or (timestamp,
        // projection_id) > (@afterTimestamp, @afterTransactionId))`.
        // Baris literal 19: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by timestamp, projection_id`.
        // Baris literal 20: LIMIT membatasi jumlah baris yang dikembalikan query: `limit @limit`.
        // Baris literal 21: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string sql = """
            select projection_id,
                   session_id,
                   user_id,
                   event_pk,
                   event_id,
                   projection_order,
                   timestamp,
                   direction,
                   amount,
                   category,
                   counterparty,
                   reference,
                   note
            from event_cashflow_projections
            where session_id = @sessionId
              and (@userId is null or user_id = @userId)
              and (@afterTimestamp is null or (timestamp, projection_id) > (@afterTimestamp, @afterTransactionId))
            order by timestamp, projection_id
            limit @limit
            """;

        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Menyiapkan variabel lokal `items` untuk nilai elemen dengan hasil operasi asinkron menjalankan query baca melalui `conn` dengan `new
        // CommandDefinition( sql, new { sessionId, userId, afterTimestamp, afterTransactionId, limit }, cancellationToken: ct)` dan memetakan baris hasil
        // ke tipe yang diminta; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai
        // awal.
        var items = await conn.QueryAsync<CashflowProjectionDb>(new CommandDefinition(
            // Meneruskan `sql` (nilai SQL) sebagai argumen ke konstruktor `CommandDefinition`.
            sql,
            // Meneruskan objek anonim yang mengelompokkan sessionId, userId, afterTimestamp, afterTransactionId, limit sebagai satu nilai sebagai argumen ke
            // konstruktor `CommandDefinition`.
            new { sessionId, userId, afterTimestamp, afterTransactionId, limit },
            // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
            // bernama `cancellationToken`.
            cancellationToken: ct));
        // Mengembalikan mematerialisasi urutan `items` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori kepada pemanggil dalam
        // GetCashflowProjectionPageAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return items.ToList();
    // Menutup scope metode GetCashflowProjectionPageAsync; bagian berikut berada di luar batas blok tersebut dalam GetCashflowProjectionPageAsync.
    }

    /// <summary>
    /// Mengambil seluruh event pada satu sesi, diurutkan berdasarkan sequence_number.
    /// </summary>
    // Mendefinisikan metode `GetAllEventsBySessionAsync` dengan hasil bertipe `Task<List<EventDb>>`. Mengambil seluruh event pada satu sesi, diurutkan
    // berdasarkan sequence_number. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan:
    // Parameter `sessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini; Parameter `ct` bertipe
    // `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<List<EventDb>> GetAllEventsBySessionAsync(Guid sessionId, CancellationToken ct)
    // Membuka scope metode GetAllEventsBySessionAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam GetAllEventsBySessionAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan penjumlahan/penggabungan antara `EventSelectColumns` dan `””” where session_id =
        // @sessionId order by sequence_number ”””`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `var sql = EventSelectColumns
        // + ”””`.
        // Baris literal 2: Pembatas literal/penutup ``; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 3: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where session_id = @sessionId`.
        // Baris literal 4: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by sequence_number`.
        // Baris literal 5: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        var sql = EventSelectColumns + """

            where session_id = @sessionId
            order by sequence_number
            """;

        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Menyiapkan variabel lokal `items` untuk nilai elemen dengan hasil operasi asinkron menjalankan query baca melalui `conn` dengan `new
        // CommandDefinition(sql, new { sessionId }, cancellationToken: ct)` dan memetakan baris hasil ke tipe yang diminta; await menunggu hasil tanpa
        // memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var items = await conn.QueryAsync<EventDb>(new CommandDefinition(sql, new { sessionId }, cancellationToken: ct));
        // Mengembalikan mematerialisasi urutan `items` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori kepada pemanggil dalam
        // GetAllEventsBySessionAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return items.ToList();
    // Menutup scope metode GetAllEventsBySessionAsync; bagian berikut berada di luar batas blok tersebut dalam GetAllEventsBySessionAsync.
    }

    /// <summary>
    /// Mengambil satu event berdasarkan session_id dan event_id.
    /// </summary>
    // Mendefinisikan metode `GetEventByIdAsync` dengan hasil bertipe `Task<EventDb?>`. Mengambil satu event berdasarkan session_id dan event_id. async
    // memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `sessionId` bertipe `Guid`
    // membawa identitas unik sesi permainan yang menjadi batas data operasi ini; Parameter `eventId` bertipe `Guid` membawa identitas unik event untuk
    // pencatatan dan pemeriksaan duplikasi; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika
    // pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<EventDb?> GetEventByIdAsync(Guid sessionId, Guid eventId, CancellationToken ct)
    // Membuka scope metode GetEventByIdAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam GetEventByIdAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan penjumlahan/penggabungan antara `EventSelectColumns` dan `””” where session_id =
        // @sessionId and event_id = @eventId limit 1 ”””`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `var sql = EventSelectColumns
        // + ”””`.
        // Baris literal 2: Pembatas literal/penutup ``; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 3: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where session_id = @sessionId`.
        // Baris literal 4: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and event_id = @eventId`.
        // Baris literal 5: LIMIT membatasi jumlah baris yang dikembalikan query: `limit 1`.
        // Baris literal 6: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        var sql = EventSelectColumns + """

            where session_id = @sessionId
              and event_id = @eventId
            limit 1
            """;

        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Mengembalikan hasil operasi asinkron membaca satu hasil basis data melalui `conn.QuerySingleOrDefaultAsync<EventDb>` dengan `new
        // CommandDefinition(sql, new { sessionId, eventId }, cancellationToken: ct)`; nilai default menunjukkan tidak ada baris hasil; await menunggu hasil
        // tanpa memblokir thread selama operasi belum selesai kepada pemanggil dalam GetEventByIdAsync; eksekusi jalur ini selesai setelah nilai hasil
        // ditentukan.
        return await conn.QuerySingleOrDefaultAsync<EventDb>(
            // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen (sql, new { sessionId, eventId }, cancellationToken: ct) sebagai argumen ke
            // `conn.QuerySingleOrDefaultAsync<EventDb>`; Meneruskan `sql` (nilai SQL) sebagai argumen ke konstruktor `CommandDefinition`; Meneruskan objek
            // anonim yang mengelompokkan sessionId, eventId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`; Meneruskan `ct` (sinyal
            // pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen bernama
            // `cancellationToken`.
            new CommandDefinition(sql, new { sessionId, eventId }, cancellationToken: ct));
    // Menutup scope metode GetEventByIdAsync; bagian berikut berada di luar batas blok tersebut dalam GetEventByIdAsync.
    }

    // Mendefinisikan metode `IsRiskResolvedAsync` dengan hasil bertipe `Task<bool>`; operasi ini menangani berstatus risiko hasil resolusi asinkron.
    // async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `sessionId` bertipe
    // `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini; Parameter `riskEventId` bertipe `Guid` membawa nilai risiko
    // event identitas; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan
    // permintaan atau aplikasi berhenti.
    internal async Task<bool> IsRiskResolvedAsync(Guid sessionId, Guid riskEventId, CancellationToken ct)
    // Membuka scope metode IsRiskResolvedAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam IsRiskResolvedAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai
        // adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string sql = ”””`.
        // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select exists (`.
        // Baris literal 3: SELECT menentukan nilai atau kolom yang dikembalikan query: `select 1`.
        // Baris literal 4: FROM memilih tabel/subquery sumber pembacaan: `from event_cashflow_projections`.
        // Baris literal 5: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where session_id = @sessionId`.
        // Baris literal 6: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and category = 'RISK_LIFE'`.
        // Baris literal 7: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and (event_id = @riskEventId or reference =
        // @riskEventId::text)`.
        // Baris literal 8: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 9: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string sql = """
            select exists (
                select 1
                from event_cashflow_projections
                where session_id = @sessionId
                  and category = 'RISK_LIFE'
                  and (event_id = @riskEventId or reference = @riskEventId::text)
            )
            """;

        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Mengembalikan hasil operasi asinkron menjalankan perintah basis data melalui `conn` dengan `new CommandDefinition(sql, new { sessionId,
        // riskEventId }, cancellationToken: ct)` dan mengambil nilai skalar hasilnya; await menunggu hasil tanpa memblokir thread selama operasi belum
        // selesai kepada pemanggil dalam IsRiskResolvedAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return await conn.ExecuteScalarAsync<bool>(
            // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen (sql, new { sessionId, riskEventId }, cancellationToken: ct) sebagai argumen ke
            // `conn.ExecuteScalarAsync<bool>`; Meneruskan `sql` (nilai SQL) sebagai argumen ke konstruktor `CommandDefinition`; Meneruskan objek anonim yang
            // mengelompokkan sessionId, riskEventId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`; Meneruskan `ct` (sinyal pembatalan
            // agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen bernama `cancellationToken`.
            new CommandDefinition(sql, new { sessionId, riskEventId }, cancellationToken: ct));
    // Menutup scope metode IsRiskResolvedAsync; bagian berikut berada di luar batas blok tersebut dalam IsRiskResolvedAsync.
    }

    // Mendefinisikan metode `HasPendingLifeRiskAsync` dengan hasil bertipe `Task<bool>`; operasi ini menangani memiliki tertunda life risiko asinkron.
    // async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `sessionId` bertipe
    // `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini; Parameter `userId` bertipe `Guid?` membawa identitas akun
    // pengguna yang datanya sedang diproses; nilai null diizinkan ketika data opsional belum tersedia; Parameter `ct` bertipe `CancellationToken`
    // membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    internal async Task<bool> HasPendingLifeRiskAsync(Guid sessionId, Guid? userId, CancellationToken ct)
    // Membuka scope metode HasPendingLifeRiskAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam HasPendingLifeRiskAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai
        // adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string sql = ”””`.
        // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select exists (`.
        // Baris literal 3: SELECT menentukan nilai atau kolom yang dikembalikan query: `select 1`.
        // Baris literal 4: FROM memilih tabel/subquery sumber pembacaan: `from events risk_event`.
        // Baris literal 5: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join ruleset_life_risks risk`.
        // Baris literal 6: ON menetapkan syarat pencocokan baris pada relasi JOIN: `on risk.ruleset_version_id = risk_event.ruleset_version_id`.
        // Baris literal 7: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and lower(risk.risk_code) = lower(risk_event.payload ->>
        // 'risk_id')`.
        // Baris literal 8: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where risk_event.session_id = @sessionId`.
        // Baris literal 9: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and (@userId is null or risk_event.user_id = @userId)`.
        // Baris literal 10: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and risk_event.action_type = 'RisikoKehidupan'`.
        // Baris literal 11: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and risk.effect_type = 'COIN_EFFECT'`.
        // Baris literal 12: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and risk.direction = 'OUT'`.
        // Baris literal 13: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and not exists (`.
        // Baris literal 14: SELECT menentukan nilai atau kolom yang dikembalikan query: `select 1`.
        // Baris literal 15: FROM memilih tabel/subquery sumber pembacaan: `from event_cashflow_projections projection`.
        // Baris literal 16: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where projection.session_id = risk_event.session_id`.
        // Baris literal 17: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and projection.category = 'RISK_LIFE'`.
        // Baris literal 18: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and projection.direction = 'OUT'`.
        // Baris literal 19: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and projection.reference = risk_event.event_id::text`.
        // Baris literal 20: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 21: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 22: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string sql = """
            select exists (
                select 1
                from events risk_event
                join ruleset_life_risks risk
                  on risk.ruleset_version_id = risk_event.ruleset_version_id
                 and lower(risk.risk_code) = lower(risk_event.payload ->> 'risk_id')
                where risk_event.session_id = @sessionId
                  and (@userId is null or risk_event.user_id = @userId)
                  and risk_event.action_type = 'RisikoKehidupan'
                  and risk.effect_type = 'COIN_EFFECT'
                  and risk.direction = 'OUT'
                  and not exists (
                      select 1
                      from event_cashflow_projections projection
                      where projection.session_id = risk_event.session_id
                        and projection.category = 'RISK_LIFE'
                        and projection.direction = 'OUT'
                        and projection.reference = risk_event.event_id::text
                  )
            )
            """;

        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Mengembalikan hasil operasi asinkron menjalankan perintah basis data melalui `conn` dengan `new CommandDefinition(sql, new { sessionId, userId },
        // cancellationToken: ct)` dan mengambil nilai skalar hasilnya; await menunggu hasil tanpa memblokir thread selama operasi belum selesai kepada
        // pemanggil dalam HasPendingLifeRiskAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return await conn.ExecuteScalarAsync<bool>(
            // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen (sql, new { sessionId, userId }, cancellationToken: ct) sebagai argumen ke
            // `conn.ExecuteScalarAsync<bool>`; Meneruskan `sql` (nilai SQL) sebagai argumen ke konstruktor `CommandDefinition`; Meneruskan objek anonim yang
            // mengelompokkan sessionId, userId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`; Meneruskan `ct` (sinyal pembatalan agar
            // operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen bernama `cancellationToken`.
            new CommandDefinition(sql, new { sessionId, userId }, cancellationToken: ct));
    // Menutup scope metode HasPendingLifeRiskAsync; bagian berikut berada di luar batas blok tersebut dalam HasPendingLifeRiskAsync.
    }

    // Mendefinisikan metode `GetOwnedNeedSaleAmountAsync` dengan hasil bertipe `Task<int?>`; operasi ini menangani get dimiliki kebutuhan penjualan
    // nominal asinkron. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter
    // `sessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini; Parameter `userId` bertipe `Guid` membawa
    // identitas akun pengguna yang datanya sedang diproses; Parameter `cardId` bertipe `string` membawa nilai kartu identitas; Parameter `ct` bertipe
    // `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    internal async Task<int?> GetOwnedNeedSaleAmountAsync(
        // Parameter `sessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini.
        Guid sessionId,
        // Parameter `userId` bertipe `Guid` membawa identitas akun pengguna yang datanya sedang diproses.
        Guid userId,
        // Parameter `cardId` bertipe `string` membawa nilai kartu identitas.
        string cardId,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    // Membuka scope metode GetOwnedNeedSaleAmountAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam GetOwnedNeedSaleAmountAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai
        // adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string sql = ”””`.
        // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select floor(spnp.paid_amount / 2.0)::int`.
        // Baris literal 3: FROM memilih tabel/subquery sumber pembacaan: `from session_participant_need_purchases spnp`.
        // Baris literal 4: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join session_participants sp on sp.session_participant_id =
        // spnp.session_participant_id`.
        // Baris literal 5: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join ruleset_needs rn on rn.ruleset_need_id =
        // spnp.ruleset_need_id`.
        // Baris literal 6: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where sp.session_id = @sessionId`.
        // Baris literal 7: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and sp.user_id = @userId`.
        // Baris literal 8: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and lower(rn.need_code) = lower(@cardId)`.
        // Baris literal 9: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and not spnp.is_sold`.
        // Baris literal 10: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by spnp.purchased_at_day, spnp.sort_order`.
        // Baris literal 11: LIMIT membatasi jumlah baris yang dikembalikan query: `limit 1`.
        // Baris literal 12: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string sql = """
            select floor(spnp.paid_amount / 2.0)::int
            from session_participant_need_purchases spnp
            join session_participants sp on sp.session_participant_id = spnp.session_participant_id
            join ruleset_needs rn on rn.ruleset_need_id = spnp.ruleset_need_id
            where sp.session_id = @sessionId
              and sp.user_id = @userId
              and lower(rn.need_code) = lower(@cardId)
              and not spnp.is_sold
            order by spnp.purchased_at_day, spnp.sort_order
            limit 1
            """;

        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Mengembalikan hasil operasi asinkron menjalankan perintah basis data melalui `conn` dengan `new CommandDefinition(sql, new { sessionId, userId,
        // cardId }, cancellationToken: ct)` dan mengambil nilai skalar hasilnya; await menunggu hasil tanpa memblokir thread selama operasi belum selesai
        // kepada pemanggil dalam GetOwnedNeedSaleAmountAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return await conn.ExecuteScalarAsync<int?>(
            // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen (sql, new { sessionId, userId, cardId }, cancellationToken: ct) sebagai argumen
            // ke `conn.ExecuteScalarAsync<int?>`; Meneruskan `sql` (nilai SQL) sebagai argumen ke konstruktor `CommandDefinition`; Meneruskan objek anonim yang
            // mengelompokkan sessionId, userId, cardId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`; Meneruskan `ct` (sinyal
            // pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen bernama
            // `cancellationToken`.
            new CommandDefinition(sql, new { sessionId, userId, cardId }, cancellationToken: ct));
    // Menutup scope metode GetOwnedNeedSaleAmountAsync; bagian berikut berada di luar batas blok tersebut dalam GetOwnedNeedSaleAmountAsync.
    }

    // Mendefinisikan metode `GetGoldQuantityAsync` dengan hasil bertipe `Task<int>`; operasi ini menangani get emas jumlah asinkron. async memungkinkan
    // metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `sessionId` bertipe `Guid` membawa
    // identitas unik sesi permainan yang menjadi batas data operasi ini; Parameter `userId` bertipe `Guid` membawa identitas akun pengguna yang datanya
    // sedang diproses; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan
    // permintaan atau aplikasi berhenti.
    internal async Task<int> GetGoldQuantityAsync(Guid sessionId, Guid userId, CancellationToken ct)
    // Membuka scope metode GetGoldQuantityAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam GetGoldQuantityAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai
        // adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string sql = ”””`.
        // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select coalesce(sum(spgh.quantity), 0)::int`.
        // Baris literal 3: FROM memilih tabel/subquery sumber pembacaan: `from session_participant_gold_holdings spgh`.
        // Baris literal 4: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join session_participants sp on sp.session_participant_id =
        // spgh.session_participant_id`.
        // Baris literal 5: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where sp.session_id = @sessionId`.
        // Baris literal 6: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and sp.user_id = @userId`.
        // Baris literal 7: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string sql = """
            select coalesce(sum(spgh.quantity), 0)::int
            from session_participant_gold_holdings spgh
            join session_participants sp on sp.session_participant_id = spgh.session_participant_id
            where sp.session_id = @sessionId
              and sp.user_id = @userId
            """;

        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Mengembalikan hasil operasi asinkron menjalankan perintah basis data melalui `conn` dengan `new CommandDefinition(sql, new { sessionId, userId },
        // cancellationToken: ct)` dan mengambil nilai skalar hasilnya; await menunggu hasil tanpa memblokir thread selama operasi belum selesai kepada
        // pemanggil dalam GetGoldQuantityAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return await conn.ExecuteScalarAsync<int>(
            // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen (sql, new { sessionId, userId }, cancellationToken: ct) sebagai argumen ke
            // `conn.ExecuteScalarAsync<int>`; Meneruskan `sql` (nilai SQL) sebagai argumen ke konstruktor `CommandDefinition`; Meneruskan objek anonim yang
            // mengelompokkan sessionId, userId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`; Meneruskan `ct` (sinyal pembatalan agar
            // operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen bernama `cancellationToken`.
            new CommandDefinition(sql, new { sessionId, userId }, cancellationToken: ct));
    // Menutup scope metode GetGoldQuantityAsync; bagian berikut berada di luar batas blok tersebut dalam GetGoldQuantityAsync.
    }

    // Mendefinisikan metode `HasActiveInsuranceAsync` dengan hasil bertipe `Task<bool>`; operasi ini menangani memiliki aktif asuransi asinkron. async
    // memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `sessionId` bertipe `Guid`
    // membawa identitas unik sesi permainan yang menjadi batas data operasi ini; Parameter `userId` bertipe `Guid` membawa identitas akun pengguna yang
    // datanya sedang diproses; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil
    // membatalkan permintaan atau aplikasi berhenti.
    internal async Task<bool> HasActiveInsuranceAsync(Guid sessionId, Guid userId, CancellationToken ct)
    // Membuka scope metode HasActiveInsuranceAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam HasActiveInsuranceAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai
        // adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string sql = ”””`.
        // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select exists (`.
        // Baris literal 3: SELECT menentukan nilai atau kolom yang dikembalikan query: `select 1`.
        // Baris literal 4: FROM memilih tabel/subquery sumber pembacaan: `from session_participant_insurances spi`.
        // Baris literal 5: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join session_participants sp on sp.session_participant_id =
        // spi.session_participant_id`.
        // Baris literal 6: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where sp.session_id = @sessionId`.
        // Baris literal 7: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and sp.user_id = @userId`.
        // Baris literal 8: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and spi.status = 'ACTIVE'`.
        // Baris literal 9: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and spi.remaining_uses > 0`.
        // Baris literal 10: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 11: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string sql = """
            select exists (
                select 1
                from session_participant_insurances spi
                join session_participants sp on sp.session_participant_id = spi.session_participant_id
                where sp.session_id = @sessionId
                  and sp.user_id = @userId
                  and spi.status = 'ACTIVE'
                  and spi.remaining_uses > 0
            )
            """;

        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Mengembalikan hasil operasi asinkron menjalankan perintah basis data melalui `conn` dengan `new CommandDefinition(sql, new { sessionId, userId },
        // cancellationToken: ct)` dan mengambil nilai skalar hasilnya; await menunggu hasil tanpa memblokir thread selama operasi belum selesai kepada
        // pemanggil dalam HasActiveInsuranceAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return await conn.ExecuteScalarAsync<bool>(
            // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen (sql, new { sessionId, userId }, cancellationToken: ct) sebagai argumen ke
            // `conn.ExecuteScalarAsync<bool>`; Meneruskan `sql` (nilai SQL) sebagai argumen ke konstruktor `CommandDefinition`; Meneruskan objek anonim yang
            // mengelompokkan sessionId, userId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`; Meneruskan `ct` (sinyal pembatalan agar
            // operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen bernama `cancellationToken`.
            new CommandDefinition(sql, new { sessionId, userId }, cancellationToken: ct));
    // Menutup scope metode HasActiveInsuranceAsync; bagian berikut berada di luar batas blok tersebut dalam HasActiveInsuranceAsync.
    }

    // Mendefinisikan metode `GetActiveLoanOutstandingAsync` dengan hasil bertipe `Task<int?>`; operasi ini menangani get aktif pinjaman belum dilunasi
    // asinkron. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `sessionId`
    // bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini; Parameter `userId` bertipe `Guid` membawa identitas
    // akun pengguna yang datanya sedang diproses; Parameter `loanInstanceId` bertipe `string` membawa nilai pinjaman instance identitas; Parameter `ct`
    // bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi
    // berhenti.
    internal async Task<int?> GetActiveLoanOutstandingAsync(
        // Parameter `sessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini.
        Guid sessionId,
        // Parameter `userId` bertipe `Guid` membawa identitas akun pengguna yang datanya sedang diproses.
        Guid userId,
        // Parameter `loanInstanceId` bertipe `string` membawa nilai pinjaman instance identitas.
        string loanInstanceId,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    // Membuka scope metode GetActiveLoanOutstandingAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // GetActiveLoanOutstandingAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai
        // adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string sql = ”””`.
        // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select spl.outstanding_amount`.
        // Baris literal 3: FROM memilih tabel/subquery sumber pembacaan: `from session_participant_loans spl`.
        // Baris literal 4: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join session_participants sp on sp.session_participant_id =
        // spl.session_participant_id`.
        // Baris literal 5: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where sp.session_id = @sessionId`.
        // Baris literal 6: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and sp.user_id = @userId`.
        // Baris literal 7: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and lower(spl.loan_instance_id) =
        // lower(@loanInstanceId)`.
        // Baris literal 8: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and spl.status = 'ACTIVE'`.
        // Baris literal 9: LIMIT membatasi jumlah baris yang dikembalikan query: `limit 1`.
        // Baris literal 10: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string sql = """
            select spl.outstanding_amount
            from session_participant_loans spl
            join session_participants sp on sp.session_participant_id = spl.session_participant_id
            where sp.session_id = @sessionId
              and sp.user_id = @userId
              and lower(spl.loan_instance_id) = lower(@loanInstanceId)
              and spl.status = 'ACTIVE'
            limit 1
            """;

        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Mengembalikan hasil operasi asinkron menjalankan perintah basis data melalui `conn` dengan `new CommandDefinition(sql, new { sessionId, userId,
        // loanInstanceId }, cancellationToken: ct)` dan mengambil nilai skalar hasilnya; await menunggu hasil tanpa memblokir thread selama operasi belum
        // selesai kepada pemanggil dalam GetActiveLoanOutstandingAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return await conn.ExecuteScalarAsync<int?>(
            // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen (sql, new { sessionId, userId, loanInstanceId }, cancellationToken: ct) sebagai
            // argumen ke `conn.ExecuteScalarAsync<int?>`; Meneruskan `sql` (nilai SQL) sebagai argumen ke konstruktor `CommandDefinition`; Meneruskan objek
            // anonim yang mengelompokkan sessionId, userId, loanInstanceId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`; Meneruskan
            // `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen bernama
            // `cancellationToken`.
            new CommandDefinition(sql, new { sessionId, userId, loanInstanceId }, cancellationToken: ct));
    // Menutup scope metode GetActiveLoanOutstandingAsync; bagian berikut berada di luar batas blok tersebut dalam GetActiveLoanOutstandingAsync.
    }

    // Mendefinisikan metode `BuildEventParameters` dengan hasil bertipe `object`; operasi ini menangani build event parameters. Masukan: Parameter
    // `record` bertipe `EventDb` membawa nilai rekaman.
    private static object BuildEventParameters(EventDb record)
    // Membuka scope metode BuildEventParameters; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildEventParameters.
    {
        // Mengembalikan objek anonim yang mengelompokkan record.EventPk, record.EventId, record.SessionId, record.SessionPlayerId, record.UserId,
        // record.ActorType, record.Timestamp, record.DayIndex, record.Weekday, record.TurnNumber, record.ActionSlot, record.SequenceNumber, ActionId,
        // ActionType, record.RulesetVersionId, PayloadVersion, record.Payload, record.ReceivedAt, record.ClientRequestId sebagai satu nilai kepada
        // pemanggil dalam BuildEventParameters; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return new
        // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // BuildEventParameters.
        {
            // Menggunakan `record` (nilai rekaman) sebagai bagian ekspresi yang sedang disusun dalam BuildEventParameters.
            record.EventPk,
            // Menggunakan `record` (nilai rekaman) sebagai bagian ekspresi yang sedang disusun dalam BuildEventParameters.
            record.EventId,
            // Menggunakan `record` (nilai rekaman) sebagai bagian ekspresi yang sedang disusun dalam BuildEventParameters.
            record.SessionId,
            // Menggunakan `record` (nilai rekaman) sebagai bagian ekspresi yang sedang disusun dalam BuildEventParameters.
            record.SessionPlayerId,
            // Menggunakan `record` (nilai rekaman) sebagai bagian ekspresi yang sedang disusun dalam BuildEventParameters.
            record.UserId,
            // Menggunakan `record` (nilai rekaman) sebagai bagian ekspresi yang sedang disusun dalam BuildEventParameters.
            record.ActorType,
            // Menggunakan `record` (nilai rekaman) sebagai bagian ekspresi yang sedang disusun dalam BuildEventParameters.
            record.Timestamp,
            // Menggunakan `record` (nilai rekaman) sebagai bagian ekspresi yang sedang disusun dalam BuildEventParameters.
            record.DayIndex,
            // Menggunakan `record` (nilai rekaman) sebagai bagian ekspresi yang sedang disusun dalam BuildEventParameters.
            record.Weekday,
            // Menggunakan `record` (nilai rekaman) sebagai bagian ekspresi yang sedang disusun dalam BuildEventParameters.
            record.TurnNumber,
            // Menggunakan `record` (nilai rekaman) sebagai bagian ekspresi yang sedang disusun dalam BuildEventParameters.
            record.ActionSlot,
            // Menggunakan `record` (nilai rekaman) sebagai bagian ekspresi yang sedang disusun dalam BuildEventParameters.
            record.SequenceNumber,
            // Menggunakan `ActionId` (kode aksi yang dipetakan terhadap katalog aturan) sebagai bagian ekspresi yang sedang disusun dalam BuildEventParameters.
            ActionId = ResolveActionId(record),
            // Menggunakan `ActionType` (nilai aksi jenis) sebagai bagian ekspresi yang sedang disusun dalam BuildEventParameters.
            ActionType = ResolveActionId(record) ?? record.ActionType,
            // Menggunakan `record` (nilai rekaman) sebagai bagian ekspresi yang sedang disusun dalam BuildEventParameters.
            record.RulesetVersionId,
            // Menggunakan `PayloadVersion` (nilai payload versi) sebagai bagian ekspresi yang sedang disusun dalam BuildEventParameters.
            PayloadVersion = string.IsNullOrWhiteSpace(record.PayloadVersion) ? "1.0" : record.PayloadVersion,
            // Menggunakan `record` (nilai rekaman) sebagai bagian ekspresi yang sedang disusun dalam BuildEventParameters.
            record.Payload,
            // Menggunakan `record` (nilai rekaman) sebagai bagian ekspresi yang sedang disusun dalam BuildEventParameters.
            record.ReceivedAt,
            // Menggunakan `record` (nilai rekaman) sebagai bagian ekspresi yang sedang disusun dalam BuildEventParameters.
            record.ClientRequestId
        // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam BuildEventParameters.
        };
    // Menutup scope metode BuildEventParameters; bagian berikut berada di luar batas blok tersebut dalam BuildEventParameters.
    }

    // Mendefinisikan metode `EnsureActionResolved` dengan hasil bertipe `Guid`; operasi ini menangani ensure aksi hasil resolusi. Masukan: Parameter
    // `rulesetActionId` bertipe `Guid?` membawa nilai aturan aksi identitas; nilai null diizinkan ketika data opsional belum tersedia; Parameter
    // `record` bertipe `EventDb` membawa nilai rekaman.
    private static Guid EnsureActionResolved(Guid? rulesetActionId, EventDb record)
    // Membuka scope metode EnsureActionResolved; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam EnsureActionResolved.
    {
        // Memeriksa kebalikan kondisi `rulesetActionId.HasValue`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam EnsureActionResolved.
        if (!rulesetActionId.HasValue)
        // Membuka scope cabang if untuk kondisi `!rulesetActionId.HasValue`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // EnsureActionResolved.
        {
            // Menyiapkan variabel lokal `actionId` untuk kode aksi yang dipetakan terhadap katalog aturan dengan hasil pemilihan bersyarat: ketika
            // `string.IsNullOrWhiteSpace(record.ActionId)` benar gunakan `EventActionIdResolver.Resolve(record.ActionType, record.Payload)`, jika tidak gunakan
            // `record.ActionId`. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var actionId = string.IsNullOrWhiteSpace(record.ActionId)
                // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: EventActionIdResolver.Resolve(record.ActionType, record.Payload)
                // dalam EnsureActionResolved.
                ? EventActionIdResolver.Resolve(record.ActionType, record.Payload)
                // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: record.ActionId; dalam EnsureActionResolved.
                : record.ActionId;
            // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen ( $”Action '{actionId}' tidak aktif atau tidak
            // terdaftar pada ruleset event.”) dalam EnsureActionResolved; pemanggil atau middleware penanganan error menerima kegagalan ini.
            throw new InvalidOperationException(
                // Meneruskan teks interpolasi `$”Action '{actionId}' tidak aktif atau tidak terdaftar pada ruleset event.”`; nilai ekspresi di dalam kurung kurawal
                // disisipkan saat program berjalan sebagai argumen ke konstruktor `InvalidOperationException`.
                $"Action '{actionId}' tidak aktif atau tidak terdaftar pada ruleset event.");
        // Menutup scope cabang if untuk kondisi `!rulesetActionId.HasValue`; bagian berikut berada di luar batas blok tersebut dalam EnsureActionResolved.
        }

        // Mengembalikan `rulesetActionId.Value`, yaitu nilai yang dibungkus objek/nullable kepada pemanggil dalam EnsureActionResolved; eksekusi jalur ini
        // selesai setelah nilai hasil ditentukan.
        return rulesetActionId.Value;
    // Menutup scope metode EnsureActionResolved; bagian berikut berada di luar batas blok tersebut dalam EnsureActionResolved.
    }

    // Mendefinisikan metode `ResolveActionId` dengan hasil bertipe `string?`; operasi ini menangani resolve aksi identitas. Masukan: Parameter `record`
    // bertipe `EventDb` membawa nilai rekaman.
    private static string? ResolveActionId(EventDb record)
    // Membuka scope metode ResolveActionId; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveActionId.
    {
        // Menyiapkan variabel lokal `actionId` untuk kode aksi yang dipetakan terhadap katalog aturan dengan hasil pemilihan bersyarat: ketika
        // `string.IsNullOrWhiteSpace(record.ActionId)` benar gunakan `EventActionIdResolver.Resolve(record.ActionType, record.Payload)`, jika tidak gunakan
        // `record.ActionId`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var actionId = string.IsNullOrWhiteSpace(record.ActionId)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: EventActionIdResolver.Resolve(record.ActionType, record.Payload)
            // dalam ResolveActionId.
            ? EventActionIdResolver.Resolve(record.ActionType, record.Payload)
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: record.ActionId; dalam ResolveActionId.
            : record.ActionId;

        // Memeriksa kebalikan kondisi `string.IsNullOrWhiteSpace(actionId)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // ResolveActionId.
        if (!string.IsNullOrWhiteSpace(actionId))
        // Membuka scope cabang if untuk kondisi `!string.IsNullOrWhiteSpace(actionId)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ResolveActionId.
        {
            // Memperbarui `record.ActionId` menggunakan `actionId` (kode aksi yang dipetakan terhadap katalog aturan) dalam ResolveActionId.
            record.ActionId = actionId;
            // Memperbarui `record.ActionType` menggunakan `actionId` (kode aksi yang dipetakan terhadap katalog aturan) dalam ResolveActionId.
            record.ActionType = actionId;
        // Menutup scope cabang if untuk kondisi `!string.IsNullOrWhiteSpace(actionId)`; bagian berikut berada di luar batas blok tersebut dalam
        // ResolveActionId.
        }

        // Mengembalikan `actionId` (kode aksi yang dipetakan terhadap katalog aturan) kepada pemanggil dalam ResolveActionId; eksekusi jalur ini selesai
        // setelah nilai hasil ditentukan.
        return actionId;
    // Menutup scope metode ResolveActionId; bagian berikut berada di luar batas blok tersebut dalam ResolveActionId.
    }
// Menutup scope tipe EventRepository; bagian berikut berada di luar batas blok tersebut.
}

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `EventAssetReferenceInput`; sealed mencegah tipe ini diturunkan lagi.
internal sealed record EventAssetReferenceInput(
    // Parameter `AssetType` bertipe `string` membawa nilai aset jenis.
    string AssetType,
    // Parameter `AssetCode` bertipe `string` membawa nilai aset kode.
    string AssetCode,
    // Parameter `ReferenceRole` bertipe `string` membawa nilai reference role.
    string ReferenceRole,
    // Parameter `PayloadPath` bertipe `string` membawa nilai payload path.
    string PayloadPath);
