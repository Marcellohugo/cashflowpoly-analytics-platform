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
{
    private readonly NpgsqlDataSource _dataSource;

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
    {
        _dataSource = dataSource;
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
    {
        const string sql = """
            select 1
            from events
            where session_id = @sessionId and event_id = @eventId
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        var result = await conn.ExecuteScalarAsync<int?>(new CommandDefinition(sql, new { sessionId, eventId }, cancellationToken: ct));
        return result.HasValue;
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
    {
        const string sql = """
            select 1
            from events
            where session_id = @sessionId and sequence_number = @sequenceNumber
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        var result = await conn.ExecuteScalarAsync<int?>(new CommandDefinition(sql, new { sessionId, sequenceNumber }, cancellationToken: ct));
        return result.HasValue;
    }

    /// <summary>
    /// Mengambil sequence_number tertinggi pada satu sesi untuk penentuan urutan event berikutnya.
    /// </summary>
    // Mendefinisikan metode `GetMaxSequenceNumberAsync` dengan hasil bertipe `Task<long?>`. Mengambil sequence_number tertinggi pada satu sesi untuk
    // penentuan urutan event berikutnya. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task.
    // Masukan: Parameter `sessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini; Parameter `ct` bertipe
    // `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<long?> GetMaxSequenceNumberAsync(Guid sessionId, CancellationToken ct)
    {
        const string sql = """
            select max(sequence_number)
            from events
            where session_id = @sessionId
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        return await conn.ExecuteScalarAsync<long?>(new CommandDefinition(sql, new { sessionId }, cancellationToken: ct));
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

    public async Task InsertEventAsync(EventDb record, CancellationToken ct)
    {
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        var rulesetActionId = await conn.QuerySingleOrDefaultAsync<Guid?>(
            new CommandDefinition(InsertEventSql, BuildEventParameters(record), cancellationToken: ct));
        record.RulesetActionId = EnsureActionResolved(rulesetActionId, record);
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
    {
        var rulesetActionId = await conn.QuerySingleOrDefaultAsync<Guid?>(
            new CommandDefinition(InsertEventSql, BuildEventParameters(record), tx, cancellationToken: ct));
        record.RulesetActionId = EnsureActionResolved(rulesetActionId, record);
    }

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
    {
        if (references.Count == 0)
        {
            return;
        }

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
                     .GroupBy(reference => new
                     {
                         AssetType = reference.AssetType.Trim().ToUpperInvariant(),
                         AssetCode = reference.AssetCode.Trim().ToUpperInvariant(),
                         ReferenceRole = reference.ReferenceRole.Trim().ToUpperInvariant()
                     })
                     .Select(group => group.First()))
        {
            var rulesetGameAssetId = await conn.QuerySingleOrDefaultAsync<Guid?>(
                new CommandDefinition(
                    resolveSql,
                    new
                    {
                        rulesetVersionId = record.RulesetVersionId,
                        reference.AssetType,
                        reference.AssetCode
                    },
                    tx,
                    cancellationToken: ct));

            if (!rulesetGameAssetId.HasValue)
            {
                // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen ( $”Asset
                // '{reference.AssetType}/{reference.AssetCode}' tidak ditemukan pada ruleset event.”) dalam InsertEventAssetReferencesAsync; pemanggil atau
                // middleware penanganan error menerima kegagalan ini.
                throw new InvalidOperationException(
                    $"Asset '{reference.AssetType}/{reference.AssetCode}' tidak ditemukan pada ruleset event.");
            }

            await conn.ExecuteAsync(new CommandDefinition(
                insertSql,
                new
                {
                    referenceId = Guid.NewGuid(),
                    sessionId = record.SessionId,
                    eventId = record.EventId,
                    rulesetVersionId = record.RulesetVersionId,
                    rulesetGameAssetId = rulesetGameAssetId.Value,
                    reference.ReferenceRole,
                    reference.PayloadPath
                },
                tx,
                cancellationToken: ct));
        }
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
    {
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

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        await conn.ExecuteAsync(new CommandDefinition(sql, new
        {
            validationLogId = Guid.NewGuid(),
            sessionId,
            eventId,
            rulesetVersionId,
            errorCode,
            errorMessage,
            statusCode,
            traceId = string.IsNullOrWhiteSpace(traceId) ? "unknown" : traceId,
            createdAt = DateTimeOffset.UtcNow
        }, cancellationToken: ct));
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
    {
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        await conn.ExecuteAsync(new CommandDefinition(InsertProjectionSql, projection, cancellationToken: ct));
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
    {
        await conn.ExecuteAsync(new CommandDefinition(InsertProjectionSql, projection, tx, cancellationToken: ct));
    }

    /// <summary>
    /// Membuka koneksi database untuk digunakan dengan transaksi eksternal.
    /// </summary>
    // Mendefinisikan metode `OpenConnectionAsync` dengan hasil bertipe `Task<NpgsqlConnection>`. Membuka koneksi database untuk digunakan dengan
    // transaksi eksternal. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter
    // `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi
    // berhenti.
    internal async Task<NpgsqlConnection> OpenConnectionAsync(CancellationToken ct)
    {
        return await _dataSource.OpenConnectionAsync(ct);
    }

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
    {
        const string sql = """
            select session_participant_id
            from session_participants
            where session_id = @sessionId
              and user_id = @userId
            limit 1
            """;

        return await conn.QuerySingleOrDefaultAsync<Guid?>(
            new CommandDefinition(sql, new { sessionId, userId }, tx, cancellationToken: ct));
    }

    /// <summary>
    /// Mengambil seluruh proyeksi arus kas dalam satu sesi.
    /// </summary>
    // Mendefinisikan metode `GetCashflowProjectionsAsync` dengan hasil bertipe `Task<List<CashflowProjectionDb>>`. Mengambil seluruh proyeksi arus kas
    // dalam satu sesi. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter
    // `sessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini; Parameter `ct` bertipe `CancellationToken`
    // membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<List<CashflowProjectionDb>> GetCashflowProjectionsAsync(Guid sessionId, CancellationToken ct)
    {
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

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        var items = await conn.QueryAsync<CashflowProjectionDb>(new CommandDefinition(sql, new { sessionId }, cancellationToken: ct));
        return items.ToList();
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
    {
        var sql = EventSelectColumns + """

            where session_id = @sessionId
              and sequence_number > @afterSequence
            order by sequence_number
            limit @limit
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        var items = await conn.QueryAsync<EventDb>(new CommandDefinition(sql, new { sessionId, afterSequence, limit }, cancellationToken: ct));
        return items.ToList();
    }

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
    {
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

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        var items = await conn.QueryAsync<CashflowProjectionDb>(new CommandDefinition(
            sql,
            new { sessionId, userId, afterTimestamp, afterTransactionId, limit },
            cancellationToken: ct));
        return items.ToList();
    }

    /// <summary>
    /// Mengambil seluruh event pada satu sesi, diurutkan berdasarkan sequence_number.
    /// </summary>
    // Mendefinisikan metode `GetAllEventsBySessionAsync` dengan hasil bertipe `Task<List<EventDb>>`. Mengambil seluruh event pada satu sesi, diurutkan
    // berdasarkan sequence_number. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan:
    // Parameter `sessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini; Parameter `ct` bertipe
    // `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<List<EventDb>> GetAllEventsBySessionAsync(Guid sessionId, CancellationToken ct)
    {
        var sql = EventSelectColumns + """

            where session_id = @sessionId
            order by sequence_number
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        var items = await conn.QueryAsync<EventDb>(new CommandDefinition(sql, new { sessionId }, cancellationToken: ct));
        return items.ToList();
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
    {
        var sql = EventSelectColumns + """

            where session_id = @sessionId
              and event_id = @eventId
            limit 1
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        return await conn.QuerySingleOrDefaultAsync<EventDb>(
            new CommandDefinition(sql, new { sessionId, eventId }, cancellationToken: ct));
    }

    internal async Task<bool> IsRiskResolvedAsync(Guid sessionId, Guid riskEventId, CancellationToken ct)
    {
        const string sql = """
            select exists (
                select 1
                from event_cashflow_projections
                where session_id = @sessionId
                  and category = 'RISK_LIFE'
                  and (event_id = @riskEventId or reference = @riskEventId::text)
            )
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        return await conn.ExecuteScalarAsync<bool>(
            new CommandDefinition(sql, new { sessionId, riskEventId }, cancellationToken: ct));
    }

    internal async Task<bool> HasPendingLifeRiskAsync(Guid sessionId, Guid? userId, CancellationToken ct)
    {
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

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        return await conn.ExecuteScalarAsync<bool>(
            new CommandDefinition(sql, new { sessionId, userId }, cancellationToken: ct));
    }

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
    {
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

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        return await conn.ExecuteScalarAsync<int?>(
            new CommandDefinition(sql, new { sessionId, userId, cardId }, cancellationToken: ct));
    }

    internal async Task<int> GetGoldQuantityAsync(Guid sessionId, Guid userId, CancellationToken ct)
    {
        const string sql = """
            select coalesce(sum(spgh.quantity), 0)::int
            from session_participant_gold_holdings spgh
            join session_participants sp on sp.session_participant_id = spgh.session_participant_id
            where sp.session_id = @sessionId
              and sp.user_id = @userId
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        return await conn.ExecuteScalarAsync<int>(
            new CommandDefinition(sql, new { sessionId, userId }, cancellationToken: ct));
    }

    internal async Task<bool> HasActiveInsuranceAsync(Guid sessionId, Guid userId, CancellationToken ct)
    {
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

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        return await conn.ExecuteScalarAsync<bool>(
            new CommandDefinition(sql, new { sessionId, userId }, cancellationToken: ct));
    }

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
    {
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

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        return await conn.ExecuteScalarAsync<int?>(
            new CommandDefinition(sql, new { sessionId, userId, loanInstanceId }, cancellationToken: ct));
    }

    private static object BuildEventParameters(EventDb record)
    {
        return new
        {
            record.EventPk,
            record.EventId,
            record.SessionId,
            record.SessionPlayerId,
            record.UserId,
            record.ActorType,
            record.Timestamp,
            record.DayIndex,
            record.Weekday,
            record.TurnNumber,
            record.ActionSlot,
            record.SequenceNumber,
            ActionId = ResolveActionId(record),
            ActionType = ResolveActionId(record) ?? record.ActionType,
            record.RulesetVersionId,
            PayloadVersion = string.IsNullOrWhiteSpace(record.PayloadVersion) ? "1.0" : record.PayloadVersion,
            record.Payload,
            record.ReceivedAt,
            record.ClientRequestId
        };
    }

    private static Guid EnsureActionResolved(Guid? rulesetActionId, EventDb record)
    {
        if (!rulesetActionId.HasValue)
        {
            var actionId = string.IsNullOrWhiteSpace(record.ActionId)
                // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: EventActionIdResolver.Resolve(record.ActionType, record.Payload)
                // dalam EnsureActionResolved.
                ? EventActionIdResolver.Resolve(record.ActionType, record.Payload)
                // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: record.ActionId; dalam EnsureActionResolved.
                : record.ActionId;
            // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen ( $”Action '{actionId}' tidak aktif atau tidak
            // terdaftar pada ruleset event.”) dalam EnsureActionResolved; pemanggil atau middleware penanganan error menerima kegagalan ini.
            throw new InvalidOperationException(
                $"Action '{actionId}' tidak aktif atau tidak terdaftar pada ruleset event.");
        }

        return rulesetActionId.Value;
    }

    private static string? ResolveActionId(EventDb record)
    {
        var actionId = string.IsNullOrWhiteSpace(record.ActionId)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: EventActionIdResolver.Resolve(record.ActionType, record.Payload)
            // dalam ResolveActionId.
            ? EventActionIdResolver.Resolve(record.ActionType, record.Payload)
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: record.ActionId; dalam ResolveActionId.
            : record.ActionId;

        if (!string.IsNullOrWhiteSpace(actionId))
        {
            record.ActionId = actionId;
            record.ActionType = actionId;
        }

        return actionId;
    }
}

internal sealed record EventAssetReferenceInput(
    // Parameter `AssetType` bertipe `string` membawa nilai aset jenis.
    string AssetType,
    // Parameter `AssetCode` bertipe `string` membawa nilai aset kode.
    string AssetCode,
    // Parameter `ReferenceRole` bertipe `string` membawa nilai reference role.
    string ReferenceRole,
    // Parameter `PayloadPath` bertipe `string` membawa nilai payload path.
    string PayloadPath);
