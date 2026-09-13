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
{
    private readonly NpgsqlDataSource _dataSource;

    /// <summary>
    /// Menerima NpgsqlDataSource untuk koneksi ke database sesi.
    /// </summary>
    // Mendefinisikan konstruktor SessionRepository yang menyiapkan objek dan menerima dependency/nilai awal dari pemanggil; parameter: Parameter
    // `dataSource` bertipe `NpgsqlDataSource` membawa sumber koneksi PostgreSQL yang mengelola pembuatan dan penggunaan ulang koneksi.
    public SessionRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    /// <summary>
    /// Mengambil data sesi berdasarkan session_id.
    /// </summary>
    // Mendefinisikan metode `GetSessionAsync` dengan hasil bertipe `Task<SessionDb?>`. Mengambil data sesi berdasarkan session_id. async memungkinkan
    // metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `sessionId` bertipe `Guid` membawa
    // identitas unik sesi permainan yang menjadi batas data operasi ini; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar
    // operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<SessionDb?> GetSessionAsync(Guid sessionId, CancellationToken ct)
    {
        const string sql = """
            select session_id, session_name, mode, status, started_at, ended_at, instructor_user_id, ruleset_version_id, is_archived, archived_at, created_at
            from sessions
            where session_id = @sessionId
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        return await conn.QuerySingleOrDefaultAsync<SessionDb>(new CommandDefinition(sql, new { sessionId }, cancellationToken: ct));
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
    {
        const string sql = """
            select session_id, session_name, mode, status, started_at, ended_at, instructor_user_id, ruleset_version_id, is_archived, archived_at, created_at
            from sessions
            where session_id = @sessionId
              and instructor_user_id = @instructorUserId
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        return await conn.QuerySingleOrDefaultAsync<SessionDb>(
            new CommandDefinition(sql, new { sessionId, instructorUserId }, cancellationToken: ct));
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
    {
        var sessionId = Guid.NewGuid();
        var createdAt = DateTimeOffset.UtcNow;

        const string insertSession = """
            insert into sessions (session_id, session_name, mode, status, started_at, ended_at, instructor_user_id, ruleset_version_id, created_at)
            values (@sessionId, @sessionName, @mode, 'CREATED', null, null, @instructorUserId, @rulesetVersionId, @createdAt)
            """;

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

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        await using var tx = await conn.BeginTransactionAsync(ct);

        var def1 = new CommandDefinition(insertSession, new { sessionId, sessionName, mode, instructorUserId, rulesetVersionId, createdAt }, tx, cancellationToken: ct);
        await conn.ExecuteAsync(def1);

        await conn.ExecuteAsync(new CommandDefinition(
            insertState,
            new { sessionId, rulesetVersionId, createdAt },
            tx,
            cancellationToken: ct));

        await tx.CommitAsync(ct);
        return sessionId;
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
    {
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

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        var rows = await conn.ExecuteAsync(new CommandDefinition(sql, new { sessionId, status, startedAt, endedAt }, cancellationToken: ct));
        return rows > 0;
    }

    /// <summary>
    /// Mengambil ruleset_version_id yang dikunci pada sesi.
    /// </summary>
    // Mendefinisikan metode `GetActiveRulesetVersionIdAsync` dengan hasil bertipe `Task<Guid?>`. Mengambil ruleset_version_id yang dikunci pada sesi.
    // async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `sessionId` bertipe
    // `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini; Parameter `ct` bertipe `CancellationToken` membawa sinyal
    // pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<Guid?> GetActiveRulesetVersionIdAsync(Guid sessionId, CancellationToken ct)
    {
        const string sql = """
            select ruleset_version_id
            from sessions
            where session_id = @sessionId
            limit 1
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        return await conn.QuerySingleOrDefaultAsync<Guid?>(new CommandDefinition(sql, new { sessionId }, cancellationToken: ct));
    }

    /// <summary>
    /// Mengambil hari yang sedang aktif dan batas akhir sesi.
    /// </summary>
    // Mendefinisikan metode `GetProgressAsync` dengan hasil bertipe `Task<SessionProgressDb?>`. Mengambil hari yang sedang aktif dan batas akhir sesi.
    // async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `sessionId` bertipe
    // `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini; Parameter `ct` bertipe `CancellationToken` membawa sinyal
    // pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<SessionProgressDb?> GetProgressAsync(Guid sessionId, CancellationToken ct)
    {
        const string sql = """
            select day, finish_day as FinishDay
            from session_states
            where session_id = @sessionId
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        return await conn.QuerySingleOrDefaultAsync<SessionProgressDb>(
            new CommandDefinition(sql, new { sessionId }, cancellationToken: ct));
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
    {
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

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        var scores = await conn.QueryAsync<SessionFinalScoreDb>(
            new CommandDefinition(sql, new { sessionId }, cancellationToken: ct));
        return scores.ToList();
    }

    /// <summary>
    /// Mengambil semua sesi diurutkan terbaru.
    /// </summary>
    // Mendefinisikan metode `ListSessionsAsync` dengan hasil bertipe `Task<List<SessionDb>>`. Mengambil semua sesi diurutkan terbaru. async
    // memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `ct` bertipe
    // `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<List<SessionDb>> ListSessionsAsync(CancellationToken ct)
    {
        const string sql = """
            select session_id, session_name, mode, status, started_at, ended_at, instructor_user_id, ruleset_version_id, is_archived, archived_at, created_at
            from sessions
            where not is_archived
            order by created_at desc
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        var items = await conn.QueryAsync<SessionDb>(new CommandDefinition(sql, cancellationToken: ct));
        return items.ToList();
    }

    /// <summary>
    /// Mengambil seluruh sesi, termasuk yang diarsipkan, untuk pekerjaan pemeliharaan terkontrol.
    /// </summary>
    // Mendefinisikan metode `ListAllSessionsForMaintenanceAsync` dengan hasil bertipe `Task<List<SessionDb>>`. Mengambil seluruh sesi, termasuk yang
    // diarsipkan, untuk pekerjaan pemeliharaan terkontrol. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian
    // melalui Task. Masukan: Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil
    // membatalkan permintaan atau aplikasi berhenti.
    public async Task<List<SessionDb>> ListAllSessionsForMaintenanceAsync(CancellationToken ct)
    {
        const string sql = """
            select session_id, session_name, mode, status, started_at, ended_at, instructor_user_id, ruleset_version_id, is_archived, archived_at, created_at
            from sessions
            order by created_at
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        var items = await conn.QueryAsync<SessionDb>(new CommandDefinition(sql, cancellationToken: ct));
        return items.ToList();
    }

    /// <summary>
    /// Mengambil daftar sesi milik instruktur tertentu.
    /// </summary>
    // Mendefinisikan metode `ListSessionsByInstructorAsync` dengan hasil bertipe `Task<List<SessionDb>>`. Mengambil daftar sesi milik instruktur
    // tertentu. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter
    // `instructorUserId` bertipe `Guid` membawa identitas instruktur pemilik sesi atau aturan; Parameter `ct` bertipe `CancellationToken` membawa
    // sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<List<SessionDb>> ListSessionsByInstructorAsync(Guid instructorUserId, CancellationToken ct)
    {
        const string sql = """
            select session_id, session_name, mode, status, started_at, ended_at, instructor_user_id, ruleset_version_id, is_archived, archived_at, created_at
            from sessions
            where instructor_user_id = @instructorUserId
              and not is_archived
            order by created_at desc
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        var items = await conn.QueryAsync<SessionDb>(new CommandDefinition(sql, new { instructorUserId }, cancellationToken: ct));
        return items.ToList();
    }

    /// <summary>
    /// Mengambil daftar sesi yang diikuti pemain tertentu.
    /// </summary>
    // Mendefinisikan metode `ListSessionsByPlayerAsync` dengan hasil bertipe `Task<List<SessionDb>>`. Mengambil daftar sesi yang diikuti pemain
    // tertentu. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `userId`
    // bertipe `Guid` membawa identitas akun pengguna yang datanya sedang diproses; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan
    // agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<List<SessionDb>> ListSessionsByPlayerAsync(Guid userId, CancellationToken ct)
    {
        const string sql = """
            select distinct s.session_id, s.session_name, s.mode, s.status, s.started_at, s.ended_at, s.instructor_user_id, s.ruleset_version_id, s.is_archived, s.archived_at, s.created_at
            from sessions s
            join session_participants sp on sp.session_id = s.session_id
            where sp.user_id = @userId
              and not s.is_archived
            order by s.created_at desc
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        var items = await conn.QueryAsync<SessionDb>(new CommandDefinition(sql, new { userId }, cancellationToken: ct));
        return items.ToList();
    }

}
