// Fungsi file: Mengelola pemetaan dan akses PostgreSQL untuk SessionRepository.
using Dapper;
using Npgsql;

namespace Cashflowpoly.Api.Data;

/// <summary>
/// Repository untuk data sesi dan ruleset yang dikunci per sesi.
/// </summary>
public sealed class SessionRepository
{
    private readonly NpgsqlDataSource _dataSource;

    /// <summary>
    /// Menerima NpgsqlDataSource untuk koneksi ke database sesi.
    /// </summary>
    public SessionRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    /// <summary>
    /// Mengambil data sesi berdasarkan session_id.
    /// </summary>
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
    public async Task<Guid> CreateSessionAsync(
        string sessionName,
        string mode,
        Guid rulesetVersionId,
        Guid instructorUserId,
        string? createdBy,
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
