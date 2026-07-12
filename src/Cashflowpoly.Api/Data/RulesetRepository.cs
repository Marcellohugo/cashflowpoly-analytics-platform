using System.Security.Cryptography;
using System.Data.Common;
using System.Text;
using System.Text.Json;
using Cashflowpoly.Api.Domain;
using Cashflowpoly.Api.Contracts;
using Dapper;
using Npgsql;

namespace Cashflowpoly.Api.Data;

/// <summary>
/// Repository untuk ruleset dan versi ruleset.
/// </summary>
public sealed class RulesetRepository
{
    private readonly NpgsqlDataSource _dataSource;

    /// <summary>
    /// Menerima NpgsqlDataSource untuk koneksi ke tabel rulesets dan ruleset_versions.
    /// </summary>
    public RulesetRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    /// <summary>
    /// Mengambil data ruleset berdasarkan ruleset_id.
    /// </summary>
    public async Task<RulesetDb?> GetRulesetAsync(Guid rulesetId, CancellationToken ct)
    {
        const string sql = """
            select ruleset_id, name, description, instructor_user_id, is_archived, archived_at, created_at, created_by_user_id
            from rulesets
            where ruleset_id = @rulesetId
              and not is_archived
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        return await conn.QuerySingleOrDefaultAsync<RulesetDb>(new CommandDefinition(sql, new { rulesetId }, cancellationToken: ct));
    }

    /// <summary>
    /// Mengambil data ruleset yang dimiliki instruktur tertentu.
    /// </summary>
    public async Task<RulesetDb?> GetRulesetForInstructorAsync(Guid rulesetId, Guid instructorUserId, CancellationToken ct)
    {
        const string sql = """
            select ruleset_id, name, description, instructor_user_id, is_archived, archived_at, created_at, created_by_user_id
            from rulesets
            where ruleset_id = @rulesetId
              and instructor_user_id = @instructorUserId
              and not is_archived
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        return await conn.QuerySingleOrDefaultAsync<RulesetDb>(
            new CommandDefinition(sql, new { rulesetId, instructorUserId }, cancellationToken: ct));
    }

    /// <summary>
    /// Mengambil versi terbaru (nomor tertinggi) dari ruleset tertentu.
    /// </summary>
    public async Task<RulesetVersionDb?> GetLatestVersionAsync(Guid rulesetId, CancellationToken ct)
    {
        const string sql = """
            select ruleset_version_id, ruleset_id, version, status, mode, config_hash, created_at, created_by_user_id
            from ruleset_versions
            where ruleset_id = @rulesetId
            order by version desc
            limit 1
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        var version = await conn.QuerySingleOrDefaultAsync<RulesetVersionDb>(new CommandDefinition(sql, new { rulesetId }, cancellationToken: ct));
        if (version is not null)
        {
            version.Definition = await ReadRulesetDefinitionAsync(conn, version.RulesetVersionId, ct);
        }

        return version;
    }

    /// <summary>
    /// Mengambil versi ACTIVE terbaru dari ruleset tertentu.
    /// </summary>
    public async Task<RulesetVersionDb?> GetLatestActiveVersionAsync(Guid rulesetId, CancellationToken ct)
    {
        const string sql = """
            select ruleset_version_id, ruleset_id, version, status, mode, config_hash, created_at, created_by_user_id
            from ruleset_versions
            where ruleset_id = @rulesetId
              and status = 'ACTIVE'
            order by version desc
            limit 1
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        var version = await conn.QuerySingleOrDefaultAsync<RulesetVersionDb>(new CommandDefinition(sql, new { rulesetId }, cancellationToken: ct));
        if (version is not null)
        {
            version.Definition = await ReadRulesetDefinitionAsync(conn, version.RulesetVersionId, ct);
        }

        return version;
    }

    /// <summary>
    /// Mengambil versi spesifik dari ruleset berdasarkan nomor versi.
    /// </summary>
    public async Task<RulesetVersionDb?> GetRulesetVersionAsync(Guid rulesetId, int version, CancellationToken ct)
    {
        const string sql = """
            select ruleset_version_id, ruleset_id, version, status, mode, config_hash, created_at, created_by_user_id
            from ruleset_versions
            where ruleset_id = @rulesetId and version = @version
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        var row = await conn.QuerySingleOrDefaultAsync<RulesetVersionDb>(new CommandDefinition(sql, new { rulesetId, version }, cancellationToken: ct));
        if (row is not null)
        {
            row.Definition = await ReadRulesetDefinitionAsync(conn, row.RulesetVersionId, ct);
        }

        return row;
    }

    /// <summary>
    /// Mengambil versi ruleset berdasarkan ruleset_version_id.
    /// </summary>
    public async Task<RulesetVersionDb?> GetRulesetVersionByIdAsync(Guid rulesetVersionId, CancellationToken ct)
    {
        const string sql = """
            select ruleset_version_id, ruleset_id, version, status, mode, config_hash, created_at, created_by_user_id
            from ruleset_versions
            where ruleset_version_id = @rulesetVersionId
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        var row = await conn.QuerySingleOrDefaultAsync<RulesetVersionDb>(new CommandDefinition(sql, new { rulesetVersionId }, cancellationToken: ct));
        if (row is not null)
        {
            row.Definition = await ReadRulesetDefinitionAsync(conn, row.RulesetVersionId, ct);
        }

        return row;
    }

    /// <summary>
    /// Membuat ruleset baru beserta versi pertama (v1 ACTIVE) dalam transaksi.
    /// </summary>
    public async Task<(Guid RulesetId, Guid RulesetVersionId, int Version)> CreateRulesetAsync(
        string name,
        string? description,
        Guid instructorUserId,
        RulesetDefinitionDto definition,
        Guid? createdByUserId,
        CancellationToken ct)
    {
        var rulesetId = Guid.NewGuid();
        var rulesetVersionId = Guid.NewGuid();
        var createdAt = DateTimeOffset.UtcNow;
        var configHash = ComputeHash(definition);
        var mode = ResolveMode(definition);

        const string insertRuleset = """
            insert into rulesets (ruleset_id, name, description, instructor_user_id, created_at, created_by_user_id)
            values (@rulesetId, @name, @description, @instructorUserId, @createdAt, @CreatedByUserId)
            """;

        const string insertVersion = """
            insert into ruleset_versions (ruleset_version_id, ruleset_id, version, status, mode, config_hash, created_at, created_by_user_id)
            values (@rulesetVersionId, @rulesetId, 1, 'ACTIVE', @mode, @configHash, @createdAt, @CreatedByUserId)
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        await using var tx = await conn.BeginTransactionAsync(ct);
        var def1 = new CommandDefinition(insertRuleset, new
        {
            rulesetId,
            name,
            description,
            instructorUserId,
            createdAt,
            CreatedByUserId = createdByUserId
        }, tx, cancellationToken: ct);
        await conn.ExecuteAsync(def1);

        var def2 = new CommandDefinition(insertVersion, new
        {
            rulesetVersionId,
            rulesetId,
            mode,
            configHash,
            createdAt,
            CreatedByUserId = createdByUserId
        }, tx, cancellationToken: ct);
        await conn.ExecuteAsync(def2);
        await WriteRulesetDefinitionAsync(conn, tx, rulesetVersionId, definition, ct);

        await tx.CommitAsync(ct);
        return (rulesetId, rulesetVersionId, 1);
    }

    /// <summary>
    /// Membuat versi baru (DRAFT) untuk ruleset yang sudah ada, opsional memperbarui nama dan deskripsi.
    /// </summary>
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

        const string nextVersionSql = """
            select coalesce(max(version), 0) + 1
            from ruleset_versions
            where ruleset_id = @rulesetId
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

        var nextVersion = await conn.ExecuteScalarAsync<int>(
            new CommandDefinition(nextVersionSql, new { rulesetId }, tx, cancellationToken: ct));

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
    {
        const string targetSql = """
            select 1
            from ruleset_versions
            where ruleset_id = @rulesetId and version = @version
            for update
            """;

        const string retireSql = """
            update ruleset_versions
            set status = 'ARCHIVED'
            where ruleset_id = @rulesetId
              and status = 'ACTIVE'
              and version <> @version
            """;

        const string activateSql = """
            update ruleset_versions
            set status = 'ACTIVE'
            where ruleset_id = @rulesetId
              and version = @version
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        await using var tx = await conn.BeginTransactionAsync(ct);

        var exists = await conn.ExecuteScalarAsync<int?>(
            new CommandDefinition(targetSql, new { rulesetId, version }, tx, cancellationToken: ct));
        if (!exists.HasValue)
        {
            await tx.RollbackAsync(ct);
            return false;
        }

        await conn.ExecuteAsync(new CommandDefinition(retireSql, new { rulesetId, version }, tx, cancellationToken: ct));
        await conn.ExecuteAsync(new CommandDefinition(activateSql, new { rulesetId, version }, tx, cancellationToken: ct));
        await tx.CommitAsync(ct);
        return true;
    }

    /// <summary>
    /// Menghitung jumlah versi yang dimiliki ruleset tertentu.
    /// </summary>
    public async Task<int> CountRulesetVersionsAsync(Guid rulesetId, CancellationToken ct)
    {
        const string sql = """
            select count(*)
            from ruleset_versions
            where ruleset_id = @rulesetId
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        return await conn.ExecuteScalarAsync<int>(
            new CommandDefinition(sql, new { rulesetId }, cancellationToken: ct));
    }

    /// <summary>
    /// Memeriksa apakah versi ruleset sedang digunakan oleh sesi, event, atau snapshot.
    /// </summary>
    public async Task<bool> IsRulesetVersionUsedAsync(Guid rulesetVersionId, CancellationToken ct)
    {
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

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        var result = await conn.ExecuteScalarAsync<int?>(
            new CommandDefinition(sql, new { rulesetVersionId }, cancellationToken: ct));
        return result.HasValue;
    }

    /// <summary>
    /// Menghapus versi spesifik dari ruleset.
    /// </summary>
    public async Task<bool> DeleteRulesetVersionAsync(Guid rulesetId, int version, CancellationToken ct)
    {
        const string selectVersionSql = """
            select ruleset_version_id
            from ruleset_versions
            where ruleset_id = @rulesetId
              and version = @version
            for update
            """;

        const string purgeSql = """
            select purge_ruleset_version_content(@rulesetVersionId)
            """;

        const string deleteVersionSql = """
            delete from ruleset_versions
            where ruleset_version_id = @rulesetVersionId
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        await using var tx = await conn.BeginTransactionAsync(ct);

        var rulesetVersionId = await conn.QuerySingleOrDefaultAsync<Guid?>(
            new CommandDefinition(selectVersionSql, new { rulesetId, version }, tx, cancellationToken: ct));
        if (!rulesetVersionId.HasValue)
        {
            await tx.RollbackAsync(ct);
            return false;
        }

        await conn.ExecuteAsync(
            new CommandDefinition(purgeSql, new { rulesetVersionId = rulesetVersionId.Value }, tx, cancellationToken: ct));

        var affected = await conn.ExecuteAsync(
            new CommandDefinition(deleteVersionSql, new { rulesetVersionId = rulesetVersionId.Value }, tx, cancellationToken: ct));

        await tx.CommitAsync(ct);
        return affected > 0;
    }

    /// <summary>
    /// Mengambil daftar ruleset milik instruktur tertentu beserta versi terbaru dan status pemakaian sesi.
    /// Status bernilai 'ACTIVE' jika ruleset pernah diaktifkan di sesi, 'DRAFT' jika belum.
    /// </summary>
    public async Task<List<RulesetListItem>> ListRulesetsByInstructorAsync(Guid instructorUserId, CancellationToken ct)
    {
        const string sql = """
            with latest_versions as (
                select
                    rv.ruleset_id,
                    rv.version as latest_version,
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
                      and s_lock.status in ('STARTED', 'ENDED')
                ) as is_locked_by_session
            from rulesets r
            left join latest_versions v
                on v.ruleset_id = r.ruleset_id and v.rn = 1
            where r.instructor_user_id = @instructorUserId
              and not r.is_archived
            order by r.created_at desc
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        var items = await conn.QueryAsync<RulesetListItem>(
            new CommandDefinition(sql, new { instructorUserId }, cancellationToken: ct));
        return items.ToList();
    }

    /// <summary>
    /// Mengambil daftar ruleset yang digunakan dalam sesi yang diikuti pemain.
    /// </summary>
    public async Task<List<RulesetListItem>> ListRulesetsByPlayerAsync(Guid userId, CancellationToken ct)
    {
        const string sql = """
            with latest_versions as (
                select
                    rv.ruleset_id,
                    rv.version as latest_version,
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
                      and s_lock.status in ('STARTED', 'ENDED')
                ) as is_locked_by_session
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

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        var items = await conn.QueryAsync<RulesetListItem>(
            new CommandDefinition(sql, new { userId }, cancellationToken: ct));
        return items.ToList();
    }

    /// <summary>
    /// Mengambil daftar ruleset default sistem untuk ditampilkan bersama daftar ruleset workspace.
    /// </summary>
    public async Task<List<RulesetListItem>> ListDefaultRulesetsAsync(CancellationToken ct)
    {
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
                      and s_lock.status in ('STARTED', 'ENDED')
                ) as is_locked_by_session
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

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        var items = await conn.QueryAsync<RulesetListItem>(
            new CommandDefinition(sql, cancellationToken: ct));
        return items.ToList();
    }

    /// <summary>
    /// Mengambil komponen ruleset default yang ditandai system-seed beserta config terbaru.
    /// </summary>
    public async Task<List<DefaultRulesetComponentDb>> ListDefaultRulesetComponentsAsync(CancellationToken ct)
    {
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

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        var items = await conn.QueryAsync<DefaultRulesetComponentDb>(
            new CommandDefinition(sql, cancellationToken: ct));
        var list = items.ToList();
        foreach (var item in list)
        {
            item.Definition = await ReadRulesetDefinitionAsync(conn, item.RulesetVersionId, ct);
        }

        return list;
    }

    /// <summary>
    /// Mengambil data ruleset jika pemain memiliki akses melalui sesi yang menggunakannya.
    /// </summary>
    public async Task<RulesetDb?> GetRulesetForPlayerAsync(Guid rulesetId, Guid userId, CancellationToken ct)
    {
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

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        return await conn.QuerySingleOrDefaultAsync<RulesetDb>(
            new CommandDefinition(sql, new { rulesetId, userId }, cancellationToken: ct));
    }

    /// <summary>
    /// Mengambil seluruh versi dari ruleset tertentu diurutkan dari terbaru.
    /// </summary>
    public async Task<List<RulesetVersionDb>> ListRulesetVersionsAsync(Guid rulesetId, CancellationToken ct)
    {
        const string sql = """
            select ruleset_version_id, ruleset_id, version, status, mode, config_hash, created_at, created_by_user_id
            from ruleset_versions
            where ruleset_id = @rulesetId
            order by version desc
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        var items = await conn.QueryAsync<RulesetVersionDb>(new CommandDefinition(sql, new { rulesetId }, cancellationToken: ct));
        var list = items.ToList();
        foreach (var item in list)
        {
            item.Definition = await ReadRulesetDefinitionAsync(conn, item.RulesetVersionId, ct);
        }

        return list;
    }

    /// <summary>
    /// Mengambil ruleset jika merupakan seed default sistem tanpa owner instruktur.
    /// </summary>
    public async Task<RulesetDb?> GetDefaultSeedRulesetAsync(Guid rulesetId, CancellationToken ct)
    {
        const string sql = """
            select ruleset_id, name, description, instructor_user_id, is_archived, archived_at, created_at, created_by_user_id
            from rulesets
            where ruleset_id = @rulesetId
              and instructor_user_id is null
              and not is_archived
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        return await conn.QuerySingleOrDefaultAsync<RulesetDb>(new CommandDefinition(sql, new { rulesetId }, cancellationToken: ct));
    }

    /// <summary>
    /// Memeriksa apakah ruleset sedang digunakan oleh sesi manapun.
    /// </summary>
    public async Task<bool> IsRulesetUsedAsync(Guid rulesetId, CancellationToken ct)
    {
        const string sql = """
            select 1
            from sessions s
            join ruleset_versions rv on rv.ruleset_version_id = s.ruleset_version_id
            where rv.ruleset_id = @rulesetId
            limit 1
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        var result = await conn.ExecuteScalarAsync<int?>(new CommandDefinition(sql, new { rulesetId }, cancellationToken: ct));
        return result.HasValue;
    }

    /// <summary>
    /// Memeriksa apakah ruleset dikunci karena dipakai sesi yang sudah dimulai atau selesai.
    /// </summary>
    public async Task<bool> IsRulesetLockedBySessionAsync(Guid rulesetId, CancellationToken ct)
    {
        const string sql = """
            select 1
            from sessions s
            join ruleset_versions rv on rv.ruleset_version_id = s.ruleset_version_id
            where rv.ruleset_id = @rulesetId
              and s.status in ('STARTED', 'ENDED')
            limit 1
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        var result = await conn.ExecuteScalarAsync<int?>(new CommandDefinition(sql, new { rulesetId }, cancellationToken: ct));
        return result.HasValue;
    }

    /// <summary>
    /// Mengarsipkan ruleset agar histori dan versi tetap dapat diaudit.
    /// </summary>
    public async Task DeleteRulesetAsync(Guid rulesetId, CancellationToken ct)
    {
        const string sql = """
            update rulesets
            set is_archived = true,
                archived_at = now()
            where ruleset_id = @rulesetId
              and not is_archived
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        await conn.ExecuteAsync(new CommandDefinition(sql, new { rulesetId }, cancellationToken: ct));
    }

    public async Task<List<RulesetActionDto>> ListRulesetActionsAsync(Guid rulesetVersionId, CancellationToken ct)
    {
        const string sql = """
            select
                action_id as ActionId
            from ruleset_actions
            where ruleset_version_id = @rulesetVersionId
              and is_active
            order by sort_order asc, action_id asc
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        var items = await conn.QueryAsync<RulesetActionDto>(
            new CommandDefinition(sql, new { rulesetVersionId }, cancellationToken: ct));
        return items.ToList();
    }

    public async Task<RulesetDefinitionDto?> GetRulesetDefinitionAsync(Guid rulesetVersionId, CancellationToken ct)
    {
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        return await ReadRulesetDefinitionAsync(conn, rulesetVersionId, ct);
    }

    private static async Task<Guid> InsertGameAssetAsync(
        NpgsqlConnection conn,
        DbTransaction tx,
        Guid rulesetVersionId,
        string assetType,
        string assetCode,
        string displayName,
        int sortOrder,
        CancellationToken ct)
    {
        var assetId = Guid.NewGuid();
        await conn.ExecuteAsync(new CommandDefinition(
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
                '{}'::jsonb,
                now(),
                now()
            )
            """,
            new { assetId, rulesetVersionId, assetType, assetCode, displayName, sortOrder },
            tx,
            cancellationToken: ct));
        return assetId;
    }

    private async Task WriteRulesetDefinitionAsync(
        NpgsqlConnection conn,
        DbTransaction tx,
        Guid rulesetVersionId,
        RulesetDefinitionDto definition,
        CancellationToken ct)
    {
        var settings = definition.Settings ?? new RulesetSettingsDto();
        var playerOrdering = definition.PlayerOrdering ?? new RulesetPlayerOrderingDto();

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

        await conn.ExecuteAsync(
            new CommandDefinition(
                insertSettingsSql,
                new
                {
                    RulesetVersionId = rulesetVersionId,
                    settings.StartingCash,
                    InitialHappiness = settings.InitialHappiness,
                    InitialSaving = settings.InitialSaving,
                    settings.ActionsPerTurn,
                    settings.FinishDay,
                    settings.MinPlayers,
                    settings.MaxPlayers,
                    settings.CashMin,
                    settings.MaxIngredientTotal,
                    settings.MaxSameIngredient,
                    settings.PrimaryNeedMaxPerDay,
                    settings.RequirePrimaryBeforeOthers,
                    settings.DonationMinAmount,
                    settings.DonationMaxAmount,
                    settings.GoldTradeAllowBuy,
                    settings.GoldTradeAllowSell,
                    settings.LoanEnabled,
                    settings.InsuranceEnabled,
                    settings.SavingGoalEnabled,
                    settings.FreelanceIncome
                },
                tx,
                cancellationToken: ct));

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

        await conn.ExecuteAsync(
            new CommandDefinition(
                insertOrderingSql,
                new
                {
                    RulesetPlayerOrderingRuleId = Guid.NewGuid(),
                    RulesetVersionId = rulesetVersionId,
                    SortOrder = 10,
                    OrderingCode = playerOrdering.OrderingCode,
                    WeekdayCode = (string?)null,
                    FeatureCode = (string?)null,
                    IsEnabled = true
                },
                tx,
                cancellationToken: ct));

        foreach (var weekdayRule in new[]
                 {
                     new { SortOrder = 20, WeekdayCode = "FRI", FeatureCode = playerOrdering.FridayFeature, IsEnabled = playerOrdering.FridayEnabled },
                     new { SortOrder = 30, WeekdayCode = "SAT", FeatureCode = playerOrdering.SaturdayFeature, IsEnabled = playerOrdering.SaturdayEnabled },
                     new { SortOrder = 40, WeekdayCode = "SUN", FeatureCode = playerOrdering.SundayFeature, IsEnabled = playerOrdering.SundayEnabled }
                 })
        {
            await conn.ExecuteAsync(
                new CommandDefinition(
                    insertOrderingSql,
                    new
                    {
                        RulesetPlayerOrderingRuleId = Guid.NewGuid(),
                        RulesetVersionId = rulesetVersionId,
                        weekdayRule.SortOrder,
                        OrderingCode = (string?)null,
                        weekdayRule.WeekdayCode,
                        weekdayRule.FeatureCode,
                        weekdayRule.IsEnabled
                    },
                    tx,
                    cancellationToken: ct));
        }

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

        var actionIds = definition.Actions
            .Select(item => item.ActionId)
            .Where(item => !string.IsNullOrWhiteSpace(item))
            .Append("CatatTransaksi")
            .Append("JumatBerkah")
            .Append("LewatiTransaksiEmas")
            .Append("HariMingguLibur")
            .Append("MulaiSesi")
            .Append("SetupBahanAwal")
            .Append("SetupEmasAwal")
            .Append("SetupMisiAwal")
            .Append("SetupPinjamanAwal")
            .Append("SetupAsuransiAwal")
            .Append("BagikanTieBreaker")
            .Append("AkhirGiliran")
            .Append("AkhiriSesi")
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        for (var index = 0; index < actionIds.Count; index++)
        {
            await conn.ExecuteAsync(
                new CommandDefinition(
                    insertActionSql,
                    new
                    {
                        RulesetActionId = Guid.NewGuid(),
                        RulesetVersionId = rulesetVersionId,
                        ActionId = actionIds[index],
                        BehaviorId = actionIds[index],
                        SortOrder = index + 1
                    },
                    tx,
                    cancellationToken: ct));
        }

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
                null,
                true,
                '{}',
                now()
            )
            """;

        var ingredientAssetIds = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);
        for (var index = 0; index < definition.Ingredients.Count; index++)
        {
            var ingredient = definition.Ingredients[index];
            var assetId = await InsertGameAssetAsync(
                conn,
                tx,
                rulesetVersionId,
                "INGREDIENT",
                ingredient.Id,
                ingredient.Nama,
                index + 1,
                ct);
            ingredientAssetIds[ingredient.Id] = assetId;
            ingredientAssetIds[ingredient.Nama] = assetId;
            await conn.ExecuteAsync(
                new CommandDefinition(
                    insertIngredientSql,
                    new
                    {
                        RulesetIngredientId = Guid.NewGuid(),
                        RulesetVersionId = rulesetVersionId,
                        RulesetGameAssetId = assetId,
                        IngredientCode = ingredient.Id,
                        ItemName = ingredient.Nama,
                        DisplayName = ingredient.Nama,
                        PurchasePrice = ingredient.HargaBeli,
                        SortOrder = index + 1,
                        PayloadJson = "{}"
                    },
                    tx,
                    cancellationToken: ct));
        }

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
                null,
                true,
                '{}',
                now()
            )
            """;

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

        foreach (var orderEntry in definition.Orders.Select((item, index) => new { Item = item, Index = index }))
        {
            var orderId = Guid.NewGuid();
            var orderAssetId = await InsertGameAssetAsync(
                conn,
                tx,
                rulesetVersionId,
                "ORDER",
                orderEntry.Item.Id,
                orderEntry.Item.Nama,
                orderEntry.Index + 1,
                ct);
            await conn.ExecuteAsync(
                new CommandDefinition(
                    insertOrderSql,
                    new
                    {
                        RulesetOrderId = orderId,
                        RulesetVersionId = rulesetVersionId,
                        RulesetGameAssetId = orderAssetId,
                        OrderCode = orderEntry.Item.Id,
                        ItemName = orderEntry.Item.Nama,
                        SellPrice = orderEntry.Item.HargaJual,
                        HappinessPoints = orderEntry.Item.PoinKebahagiaan,
                        SortOrder = orderEntry.Index + 1
                    },
                    tx,
                    cancellationToken: ct));

            foreach (var requirement in orderEntry.Item.Bahan.Select((name, requirementIndex) => new { Name = name, RequirementIndex = requirementIndex }))
            {
                if (!ingredientAssetIds.TryGetValue(requirement.Name, out var requiredAssetId))
                {
                    throw new InvalidOperationException($"Ingredient requirement '{requirement.Name}' tidak ditemukan dalam ruleset.");
                }

                await conn.ExecuteAsync(
                    new CommandDefinition(
                        insertOrderRequirementSql,
                        new
                        {
                            RulesetOrderRequirementId = Guid.NewGuid(),
                            RulesetVersionId = rulesetVersionId,
                            RulesetOrderId = orderId,
                            RequirementOrder = requirement.RequirementIndex + 1,
                            RequiredAssetId = requiredAssetId,
                            QtyRequired = 1
                        },
                        tx,
                        cancellationToken: ct));
            }
        }

        const string insertNeedSql = """
            insert into ruleset_needs (
                ruleset_need_id,
                ruleset_version_id,
                ruleset_game_asset_id,
                need_code,
                item_name,
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
                @NeedTier,
                @PurchasePrice,
                @HappinessPoints,
                @SortOrder,
                null,
                true,
                '{}',
                now()
            )
            """;

        var needAssetIds = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);
        for (var index = 0; index < definition.Needs.Count; index++)
        {
            var need = definition.Needs[index];
            var assetId = await InsertGameAssetAsync(
                conn,
                tx,
                rulesetVersionId,
                "NEED",
                need.Id,
                need.Nama,
                index + 1,
                ct);
            needAssetIds[need.Id] = assetId;
            needAssetIds[need.Nama] = assetId;
            await conn.ExecuteAsync(
                new CommandDefinition(
                    insertNeedSql,
                    new
                    {
                        RulesetNeedId = Guid.NewGuid(),
                        RulesetVersionId = rulesetVersionId,
                        RulesetGameAssetId = assetId,
                        NeedCode = need.Id,
                        ItemName = need.Nama,
                        NeedTier = need.Tipe,
                        PurchasePrice = need.HargaBeli,
                        HappinessPoints = need.PoinKebahagiaan,
                        SortOrder = index + 1
                    },
                    tx,
                    cancellationToken: ct));
        }

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

        for (var index = 0; index < definition.NeedSetBonuses.Count; index++)
        {
            var bonus = definition.NeedSetBonuses[index];
            await conn.ExecuteAsync(
                new CommandDefinition(
                    insertNeedSetBonusSql,
                    new
                    {
                        RulesetNeedSetBonusId = Guid.NewGuid(),
                        RulesetVersionId = rulesetVersionId,
                        bonus.PatternCode,
                        bonus.RequiredCount,
                        bonus.Points,
                        SortOrder = index + 1
                    },
                    tx,
                    cancellationToken: ct));
        }

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

        const string insertMissionRequirementSql = """
            insert into ruleset_collection_mission_requirements (
                ruleset_collection_mission_requirement_id,
                ruleset_version_id,
                ruleset_collection_mission_id,
                requirement_order,
                requirement_type,
                required_asset_id,
                required_need_tier,
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
                null,
                '{}',
                now()
            )
            """;

        foreach (var missionEntry in definition.CollectionMissions.Select((item, index) => new { Item = item, Index = index }))
        {
            var missionId = Guid.NewGuid();
            await conn.ExecuteAsync(
                new CommandDefinition(
                    insertMissionSql,
                    new
                    {
                        RulesetCollectionMissionId = missionId,
                        RulesetVersionId = rulesetVersionId,
                        MissionCode = missionEntry.Item.Id,
                        ItemName = missionEntry.Item.Nama,
                        missionEntry.Item.SuccessPoints,
                        missionEntry.Item.FailurePoints,
                        missionEntry.Item.PenaltyPoints,
                        SortOrder = missionEntry.Index + 1
                    },
                    tx,
                    cancellationToken: ct));

            foreach (var requirement in missionEntry.Item.KebutuhanTarget)
            {
                var isTier = string.Equals(requirement.Type, "TIER", StringComparison.OrdinalIgnoreCase)
                             || string.Equals(requirement.Type, "NEED_TIER", StringComparison.OrdinalIgnoreCase);
                Guid? requiredAssetId = null;
                if (!isTier)
                {
                    if (!needAssetIds.TryGetValue(requirement.Value, out var resolvedAssetId))
                    {
                        throw new InvalidOperationException($"Need requirement '{requirement.Value}' tidak ditemukan dalam ruleset.");
                    }

                    requiredAssetId = resolvedAssetId;
                }

                await conn.ExecuteAsync(
                    new CommandDefinition(
                        insertMissionRequirementSql,
                        new
                        {
                            RulesetCollectionMissionRequirementId = Guid.NewGuid(),
                            RulesetVersionId = rulesetVersionId,
                            RulesetCollectionMissionId = missionId,
                            RequirementOrder = requirement.Order,
                            RequirementType = isTier ? "NEED_TIER" : "ASSET",
                            RequiredAssetId = requiredAssetId,
                            RequiredNeedTier = isTier ? requirement.Value.ToLowerInvariant() : null
                        },
                        tx,
                        cancellationToken: ct));
            }
        }

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
                null,
                true,
                '{}',
                now()
            )
            """;

        for (var index = 0; index < definition.FinancialGoals.Count; index++)
        {
            var goal = definition.FinancialGoals[index];
            await conn.ExecuteAsync(
                new CommandDefinition(
                    insertFinancialGoalSql,
                    new
                    {
                        RulesetFinancialGoalId = Guid.NewGuid(),
                        RulesetVersionId = rulesetVersionId,
                        GoalCode = goal.Id,
                        ItemName = goal.Nama,
                        PurchasePrice = goal.HargaBeli,
                        HappinessPoints = goal.PoinKebahagiaan,
                        SortOrder = index + 1
                    },
                    tx,
                    cancellationToken: ct));
        }

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

        foreach (var narrativeEntry in definition.Narratives.Select((item, index) => new { Item = item, Index = index }))
        {
            var narrativeId = Guid.NewGuid();
            await conn.ExecuteAsync(
                new CommandDefinition(
                    insertNarrativeSql,
                    new
                    {
                        RulesetNarrativeId = narrativeId,
                        RulesetVersionId = rulesetVersionId,
                        NarrativeCode = narrativeEntry.Item.Id,
                        ItemName = narrativeEntry.Item.Nama,
                        SortOrder = narrativeEntry.Index + 1
                    },
                    tx,
                    cancellationToken: ct));

            await conn.ExecuteAsync(
                new CommandDefinition(
                    insertNarrativeSceneSql,
                    new
                    {
                        RulesetNarrativeSceneId = Guid.NewGuid(),
                        RulesetVersionId = rulesetVersionId,
                        RulesetNarrativeId = narrativeId,
                        SceneCode = $"{narrativeEntry.Item.Id}_scene_1",
                        SceneOrder = 1,
                        TextLines = JsonSerializer.Serialize(narrativeEntry.Item.Teks)
                    },
                    tx,
                    cancellationToken: ct));

            foreach (var prerequisite in narrativeEntry.Item.PrerequisiteAksi.Select((item, prerequisiteIndex) => new { Item = item, Index = prerequisiteIndex }))
            {
                await conn.ExecuteAsync(
                    new CommandDefinition(
                        insertTriggerConditionSql,
                        new
                        {
                            RulesetTriggerConditionId = Guid.NewGuid(),
                            RulesetVersionId = rulesetVersionId,
                            RulesetNarrativeId = narrativeId,
                            ActionId = prerequisite.Item.Aksi,
                            ThresholdNumeric = Math.Max(1, prerequisite.Item.Value),
                            SortOrder = prerequisite.Index + 1
                        },
                        tx,
                        cancellationToken: ct));
            }
        }

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

        for (var index = 0; index < definition.DonationRankPoints.Count; index++)
        {
            var point = definition.DonationRankPoints[index];
            await conn.ExecuteAsync(
                    new CommandDefinition(
                    insertRankPointSql,
                    new
                    {
                        RulesetRankPointId = Guid.NewGuid(),
                        RulesetVersionId = rulesetVersionId,
                        RankType = "DONATION",
                        RankNo = point.Rank,
                        point.Points,
                        SortOrder = index + 1
                    },
                    tx,
                    cancellationToken: ct));
        }

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

        Guid? goldAssetId = null;
        if (definition.GoldPointsByQty.Count > 0)
        {
            goldAssetId = await InsertGameAssetAsync(
                conn,
                tx,
                rulesetVersionId,
                "GOLD",
                "gold_card",
                "Emas",
                1,
                ct);
        }

        for (var index = 0; index < definition.GoldPointsByQty.Count; index++)
        {
            var point = definition.GoldPointsByQty[index];
            await conn.ExecuteAsync(
                new CommandDefinition(
                    insertGoldPointSql,
                    new
                    {
                        RulesetGoldAssetId = Guid.NewGuid(),
                        RulesetVersionId = rulesetVersionId,
                        RulesetGameAssetId = goldAssetId!.Value,
                        AssetCode = "gold_card",
                        Quantity = point.Qty,
                        point.Points,
                        SortOrder = index + 1
                    },
                    tx,
                    cancellationToken: ct));
        }

        for (var index = 0; index < definition.PensionRankPoints.Count; index++)
        {
            var point = definition.PensionRankPoints[index];
            await conn.ExecuteAsync(
                new CommandDefinition(
                    insertRankPointSql,
                    new
                    {
                        RulesetRankPointId = Guid.NewGuid(),
                        RulesetVersionId = rulesetVersionId,
                        RankType = "PENSION",
                        RankNo = point.Rank,
                        point.Points,
                        SortOrder = index + 1
                    },
                    tx,
                    cancellationToken: ct));
        }

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

        for (var index = 0; index < definition.GoldPrices.Count; index++)
        {
            var price = definition.GoldPrices[index];
            var assetId = await InsertGameAssetAsync(
                conn,
                tx,
                rulesetVersionId,
                "GOLD_PRICE",
                price.PriceCode,
                $"Harga Emas {price.Qty}",
                index + 1,
                ct);
            await conn.ExecuteAsync(
                new CommandDefinition(
                    insertGoldPriceSql,
                    new
                    {
                        RulesetGoldPriceId = Guid.NewGuid(),
                        RulesetVersionId = rulesetVersionId,
                        RulesetGameAssetId = assetId,
                        price.PriceCode,
                        Quantity = price.Qty,
                        price.UnitPrice,
                        SortOrder = index + 1,
                        price.CardQty
                    },
                    tx,
                    cancellationToken: ct));
        }

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

        for (var index = 0; index < definition.TieBreakers.Count; index++)
        {
            var tieBreaker = definition.TieBreakers[index];
            var assetId = await InsertGameAssetAsync(
                conn,
                tx,
                rulesetVersionId,
                "TIE_BREAKER",
                tieBreaker.TieBreakerCode,
                $"Tie Breaker {tieBreaker.TieNumber}",
                index + 1,
                ct);
            await conn.ExecuteAsync(
                new CommandDefinition(
                    insertTieBreakerSql,
                    new
                    {
                        RulesetTieBreakerId = Guid.NewGuid(),
                        RulesetVersionId = rulesetVersionId,
                        RulesetGameAssetId = assetId,
                        tieBreaker.TieBreakerCode,
                        tieBreaker.TieNumber,
                        SortOrder = index + 1,
                        tieBreaker.CardQty
                    },
                    tx,
                    cancellationToken: ct));
        }

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

        for (var index = 0; index < definition.ShariaLoans.Count; index++)
        {
            var loan = definition.ShariaLoans[index];
            await conn.ExecuteAsync(
                new CommandDefinition(
                    insertShariaLoanSql,
                    new
                    {
                        RulesetShariaLoanId = Guid.NewGuid(),
                        RulesetVersionId = rulesetVersionId,
                        loan.LoanCode,
                        loan.ItemName,
                        loan.Principal,
                        loan.RepaymentAmount,
                        loan.DurationDays,
                        loan.PenaltyPoints,
                        SortOrder = index + 1,
                        loan.CardQty
                    },
                    tx,
                    cancellationToken: ct));
        }

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

        for (var index = 0; index < definition.InsuranceProducts.Count; index++)
        {
            var product = definition.InsuranceProducts[index];
            await conn.ExecuteAsync(
                new CommandDefinition(
                    insertInsuranceProductSql,
                    new
                    {
                        RulesetInsuranceProductId = Guid.NewGuid(),
                        RulesetVersionId = rulesetVersionId,
                        product.ProductCode,
                        product.ItemName,
                        product.Premium,
                        product.UsageLimit,
                        SortOrder = index + 1,
                        product.CardQty
                    },
                    tx,
                    cancellationToken: ct));
        }

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
                @SortOrder,
                @CardQty,
                '{}',
                now()
            )
            """;

        for (var index = 0; index < definition.LifeRisks.Count; index++)
        {
            var risk = definition.LifeRisks[index];
            var assetId = await InsertGameAssetAsync(
                conn,
                tx,
                rulesetVersionId,
                "RISK",
                risk.RiskCode,
                risk.ItemName,
                index + 1,
                ct);
            await conn.ExecuteAsync(
                new CommandDefinition(
                    insertLifeRiskSql,
                    new
                    {
                        RulesetLifeRiskId = Guid.NewGuid(),
                        RulesetVersionId = rulesetVersionId,
                        RulesetGameAssetId = assetId,
                        risk.RiskCode,
                        risk.ItemName,
                        risk.EffectType,
                        Direction = string.IsNullOrWhiteSpace(risk.Direction) ? null : risk.Direction,
                        risk.Amount,
                        SortOrder = index + 1,
                        risk.CardQty
                    },
                    tx,
                    cancellationToken: ct));
        }
    }

    private async Task<RulesetDefinitionDto?> ReadRulesetDefinitionAsync(
        NpgsqlConnection conn,
        Guid rulesetVersionId,
        CancellationToken ct)
    {
        const string versionSql = """
            select mode
            from ruleset_versions
            where ruleset_version_id = @rulesetVersionId
            """;

        var mode = await conn.ExecuteScalarAsync<string?>(
            new CommandDefinition(versionSql, new { rulesetVersionId }, cancellationToken: ct));
        if (string.IsNullOrWhiteSpace(mode))
        {
            return null;
        }

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

        var settings = await conn.QuerySingleOrDefaultAsync<RulesetSettingsRow>(
            new CommandDefinition(settingsSql, new { rulesetVersionId }, cancellationToken: ct));
        if (settings is null)
        {
            return null;
        }

        var orderingRules = (await conn.QueryAsync<PlayerOrderingRuleRow>(
            new CommandDefinition(
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
                new { rulesetVersionId },
                cancellationToken: ct))).ToList();

        var instructorUsernames = new List<string>();

        var actions = (await conn.QueryAsync<RulesetActionDto>(
            new CommandDefinition(
                """
                select action_id as ActionId
                from ruleset_actions
                where ruleset_version_id = @rulesetVersionId
                  and is_active
                order by sort_order asc, action_id asc
                """,
                new { rulesetVersionId },
                cancellationToken: ct))).ToList();

        var ingredients = (await conn.QueryAsync<RulesetIngredientDto>(
            new CommandDefinition(
                """
                select
                    ingredient_code as Id,
                    display_name as Nama,
                    purchase_price as HargaBeli
                from ruleset_ingredients
                where ruleset_version_id = @rulesetVersionId
                  and is_active
                order by sort_order asc, ingredient_code asc
                """,
                new { rulesetVersionId },
                cancellationToken: ct))).ToList();

        var orderRows = (await conn.QueryAsync<OrderRow>(
            new CommandDefinition(
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
                new { rulesetVersionId },
                cancellationToken: ct))).ToList();

        var orderRequirementLookup = (await conn.QueryAsync<OrderRequirementRow>(
            new CommandDefinition(
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
                new { rulesetVersionId },
                cancellationToken: ct)))
            .GroupBy(row => row.RulesetOrderId)
            .ToDictionary(
                group => group.Key,
                group => group
                    .OrderBy(item => item.RequirementOrder)
                    .SelectMany(item => Enumerable.Repeat(item.IngredientValue, item.QtyRequired))
                    .ToList());

        var orders = orderRows.Select(row => new RulesetOrderDto
        {
            Id = row.Id,
            Nama = row.Nama,
            HargaJual = row.HargaJual,
            PoinKebahagiaan = row.PoinKebahagiaan,
            Bahan = orderRequirementLookup.TryGetValue(row.RulesetOrderId, out var requirements)
                ? requirements
                : [],
            CardQty = row.CardQty
        }).ToList();

        var needs = (await conn.QueryAsync<RulesetNeedDto>(
            new CommandDefinition(
                """
                select
                    need_code as Id,
                    item_name as Nama,
                    need_tier as Tipe,
                    purchase_price as HargaBeli,
                    happiness_points as PoinKebahagiaan
                from ruleset_needs
                where ruleset_version_id = @rulesetVersionId
                  and is_active
                order by sort_order asc, need_code asc
                """,
                new { rulesetVersionId },
                cancellationToken: ct))).ToList();

        var needSetBonuses = (await conn.QueryAsync<RulesetNeedSetBonusDto>(
            new CommandDefinition(
                """
                select
                    pattern_code as PatternCode,
                    required_count as RequiredCount,
                    points as Points
                from ruleset_need_set_bonuses
                where ruleset_version_id = @rulesetVersionId
                order by sort_order asc, pattern_code asc
                """,
                new { rulesetVersionId },
                cancellationToken: ct))).ToList();

        var missionRows = (await conn.QueryAsync<CollectionMissionRow>(
            new CommandDefinition(
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
                new { rulesetVersionId },
                cancellationToken: ct))).ToList();

        var missionRequirementLookup = (await conn.QueryAsync<CollectionMissionRequirementRow>(
            new CommandDefinition(
                """
                select
                    requirement.ruleset_collection_mission_id as RulesetCollectionMissionId,
                    requirement.requirement_order as RequirementOrder,
                    case
                      when requirement.requirement_type = 'NEED_TIER' then 'TIER'
                      else 'NAME'
                    end as Type,
                    coalesce(requirement.required_need_tier, asset.asset_code) as Value
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
                new { rulesetVersionId },
                cancellationToken: ct)))
            .GroupBy(row => row.RulesetCollectionMissionId)
            .ToDictionary(
                group => group.Key,
                group => group
                    .OrderBy(item => item.RequirementOrder)
                    .Select(item => new RulesetCollectionMissionRequirementDto
                    {
                        Order = item.RequirementOrder,
                        Type = item.Type,
                        Value = item.Value
                    })
                    .ToList());

        var missions = missionRows.Select(row => new RulesetCollectionMissionDto
        {
            Id = row.Id,
            Nama = row.Nama,
            SuccessPoints = row.SuccessPoints,
            FailurePoints = row.FailurePoints,
            PenaltyPoints = row.PenaltyPoints,
            KebutuhanTarget = missionRequirementLookup.TryGetValue(row.RulesetCollectionMissionId, out var requirements)
                ? requirements
                : []
        }).ToList();

        var financialGoals = (await conn.QueryAsync<RulesetFinancialGoalDto>(
            new CommandDefinition(
                """
                select
                    goal_code as Id,
                    item_name as Nama,
                    purchase_price as HargaBeli,
                    happiness_points as PoinKebahagiaan
                from ruleset_financial_goals
                where ruleset_version_id = @rulesetVersionId
                  and is_active
                order by sort_order asc, goal_code asc
                """,
                new { rulesetVersionId },
                cancellationToken: ct))).ToList();

        var narrativeRows = (await conn.QueryAsync<NarrativeRow>(
            new CommandDefinition(
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
                new { rulesetVersionId },
                cancellationToken: ct))).ToList();

        var prerequisiteLookup = (await conn.QueryAsync<NarrativePrerequisiteRow>(
            new CommandDefinition(
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
                new { rulesetVersionId },
                cancellationToken: ct)))
            .GroupBy(row => row.RulesetNarrativeId)
            .ToDictionary(
                group => group.Key,
                group => group
                    .OrderBy(item => item.SortOrder)
                    .Select(item => new RulesetNarrativePrerequisiteDto
                    {
                        Aksi = item.Aksi,
                        Value = item.Value
                    })
                    .ToList());

        var narratives = narrativeRows.Select(row => new RulesetNarrativeDto
        {
            Id = row.Id,
            Nama = row.Nama,
            Teks = ParseStringArray(row.TextLinesJson),
            PrerequisiteAksi = prerequisiteLookup.TryGetValue(row.RulesetNarrativeId, out var prerequisites)
                ? prerequisites
                : []
        }).ToList();

        var donationRankPoints = (await conn.QueryAsync<RulesetDonationRankPointDto>(
            new CommandDefinition(
                """
                select rank_no as Rank, points as Points
                from ruleset_rank_points
                where ruleset_version_id = @rulesetVersionId
                  and rank_type = 'DONATION'
                order by sort_order asc, rank_no asc
                """,
                new { rulesetVersionId },
                cancellationToken: ct))).ToList();

        var goldPointRows = (await conn.QueryAsync<RulesetGoldPointDto>(
            new CommandDefinition(
                """
                select quantity as Qty, points as Points
                from ruleset_gold_assets
                where ruleset_version_id = @rulesetVersionId
                  and is_active
                order by sort_order asc, quantity asc
                """,
                new { rulesetVersionId },
                cancellationToken: ct))).ToList();

        var pensionRankPoints = (await conn.QueryAsync<RulesetPensionRankPointDto>(
            new CommandDefinition(
                """
                select rank_no as Rank, points as Points
                from ruleset_rank_points
                where ruleset_version_id = @rulesetVersionId
                  and rank_type = 'PENSION'
                order by sort_order asc, rank_no asc
                """,
                new { rulesetVersionId },
                cancellationToken: ct))).ToList();

        var goldPrices = (await conn.QueryAsync<RulesetGoldPriceDto>(
            new CommandDefinition(
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
                new { rulesetVersionId },
                cancellationToken: ct))).ToList();

        var tieBreakers = (await conn.QueryAsync<RulesetTieBreakerDto>(
            new CommandDefinition(
                """
                select
                    tie_breaker_code as TieBreakerCode,
                    tie_number as TieNumber,
                    card_qty as CardQty
                from ruleset_tie_breakers
                where ruleset_version_id = @rulesetVersionId
                order by sort_order asc, tie_number asc
                """,
                new { rulesetVersionId },
                cancellationToken: ct))).ToList();

        var shariaLoans = (await conn.QueryAsync<RulesetShariaLoanDto>(
            new CommandDefinition(
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
                new { rulesetVersionId },
                cancellationToken: ct))).ToList();

        var insuranceProducts = (await conn.QueryAsync<RulesetInsuranceProductDto>(
            new CommandDefinition(
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
                new { rulesetVersionId },
                cancellationToken: ct))).ToList();

        var lifeRisks = (await conn.QueryAsync<RulesetLifeRiskDto>(
            new CommandDefinition(
                """
                select
                    risk_code as RiskCode,
                    item_name as ItemName,
                    effect_type as EffectType,
                    coalesce(direction, '') as Direction,
                    amount as Amount,
                    card_qty as CardQty
                from ruleset_life_risks
                where ruleset_version_id = @rulesetVersionId
                order by sort_order asc, risk_code asc
                """,
                new { rulesetVersionId },
                cancellationToken: ct))).ToList();

        return new RulesetDefinitionDto
        {
            Mode = mode,
            Settings = new RulesetSettingsDto
            {
                ActionsPerTurn = settings.ActionsPerTurn,
                StartingCash = settings.StartingCash,
                InitialCoins = settings.StartingCash,
                InitialHappiness = settings.InitialHappiness,
                InitialSaving = settings.InitialSaving,
                FinishDay = settings.FinishDay,
                MinPlayers = settings.MinPlayers,
                MaxPlayers = settings.MaxPlayers,
                CashMin = settings.CashMin,
                MaxIngredientTotal = settings.MaxIngredientTotal,
                MaxSameIngredient = settings.MaxSameIngredient,
                PrimaryNeedMaxPerDay = settings.PrimaryNeedMaxPerDay,
                RequirePrimaryBeforeOthers = settings.RequirePrimaryBeforeOthers,
                DonationMinAmount = settings.DonationMinAmount,
                DonationMaxAmount = settings.DonationMaxAmount,
                GoldTradeAllowBuy = settings.GoldTradeAllowBuy,
                GoldTradeAllowSell = settings.GoldTradeAllowSell,
                LoanEnabled = settings.LoanEnabled,
                InsuranceEnabled = settings.InsuranceEnabled,
                SavingGoalEnabled = settings.SavingGoalEnabled,
                FreelanceIncome = settings.FreelanceIncome
            },
            PlayerOrdering = BuildPlayerOrdering(orderingRules),
            Actions = actions,
            Ingredients = ingredients,
            Orders = orders,
            Needs = needs,
            NeedSetBonuses = needSetBonuses,
            CollectionMissions = missions,
            FinancialGoals = financialGoals,
            Narratives = narratives,
            DonationRankPoints = donationRankPoints,
            GoldPointsByQty = goldPointRows,
            GoldPrices = goldPrices,
            PensionRankPoints = pensionRankPoints,
            TieBreakers = tieBreakers,
            ShariaLoans = shariaLoans,
            InsuranceProducts = insuranceProducts,
            LifeRisks = lifeRisks
        };
    }

    private static RulesetPlayerOrderingDto BuildPlayerOrdering(
        IReadOnlyCollection<PlayerOrderingRuleRow> rules)
    {
        var orderingCode = rules
            .Where(rule => !string.IsNullOrWhiteSpace(rule.OrderingCode))
            .OrderBy(rule => rule.SortOrder)
            .Select(rule => rule.OrderingCode!)
            .FirstOrDefault() ?? "PLAYER_ORDER";

        var friday = rules.FirstOrDefault(rule => string.Equals(rule.WeekdayCode, "FRI", StringComparison.OrdinalIgnoreCase));
        var saturday = rules.FirstOrDefault(rule => string.Equals(rule.WeekdayCode, "SAT", StringComparison.OrdinalIgnoreCase));
        var sunday = rules.FirstOrDefault(rule => string.Equals(rule.WeekdayCode, "SUN", StringComparison.OrdinalIgnoreCase));

        return new RulesetPlayerOrderingDto
        {
            OrderingCode = orderingCode,
            FridayFeature = friday?.FeatureCode ?? "DONATION",
            FridayEnabled = friday?.IsEnabled ?? true,
            SaturdayFeature = saturday?.FeatureCode ?? "GOLD_TRADE",
            SaturdayEnabled = saturday?.IsEnabled ?? true,
            SundayFeature = sunday?.FeatureCode ?? "REST",
            SundayEnabled = sunday?.IsEnabled ?? true
        };
    }

    private static string SerializeJsonElement(JsonElement element)
    {
        if (element.ValueKind is JsonValueKind.Undefined or JsonValueKind.Null)
        {
            return "{}";
        }

        return element.GetRawText();
    }

    private static JsonElement ParseJsonElement(string? json)
    {
        using var document = JsonDocument.Parse(string.IsNullOrWhiteSpace(json) ? "{}" : json);
        return document.RootElement.Clone();
    }

    private static List<string> ParseStringArray(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return [];
        }

        try
        {
            using var document = JsonDocument.Parse(json);
            if (document.RootElement.ValueKind != JsonValueKind.Array)
            {
                return [];
            }

            return document.RootElement
                .EnumerateArray()
                .Where(item => item.ValueKind == JsonValueKind.String)
                .Select(item => item.GetString())
                .Where(item => !string.IsNullOrWhiteSpace(item))
                .Select(item => item!)
                .ToList();
        }
        catch (JsonException)
        {
            return [];
        }
    }

    /// <summary>
    /// Menghitung SHA-256 hash dari definisi ruleset untuk deteksi perubahan.
    /// </summary>
    private static string ComputeHash(RulesetDefinitionDto definition)
    {
        var input = JsonSerializer.Serialize(definition);
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexStringLower(bytes);
    }

    private static string ResolveMode(RulesetDefinitionDto definition)
    {
        if (!RulesetRuntimeMapper.TryBuildConfig(definition, out var config, out _) ||
            config is null ||
            string.IsNullOrWhiteSpace(config.Mode))
        {
            throw new InvalidOperationException("Definition ruleset harus memiliki mode yang valid.");
        }

        return config.Mode.ToUpperInvariant();
    }

    private sealed class RulesetSettingsRow
    {
        public int StartingCash { get; init; }
        public int InitialHappiness { get; init; }
        public int InitialSaving { get; init; }
        public int ActionsPerTurn { get; init; }
        public int FinishDay { get; init; }
        public int MinPlayers { get; init; }
        public int MaxPlayers { get; init; }
        public int CashMin { get; init; }
        public int MaxIngredientTotal { get; init; }
        public int MaxSameIngredient { get; init; }
        public int PrimaryNeedMaxPerDay { get; init; }
        public bool RequirePrimaryBeforeOthers { get; init; }
        public int DonationMinAmount { get; init; }
        public int DonationMaxAmount { get; init; }
        public bool GoldTradeAllowBuy { get; init; }
        public bool GoldTradeAllowSell { get; init; }
        public bool LoanEnabled { get; init; }
        public bool InsuranceEnabled { get; init; }
        public bool SavingGoalEnabled { get; init; }
        public int FreelanceIncome { get; init; }
    }

    private sealed class PlayerOrderingRuleRow
    {
        public string? OrderingCode { get; init; }
        public string? WeekdayCode { get; init; }
        public string? FeatureCode { get; init; }
        public bool IsEnabled { get; init; }
        public int SortOrder { get; init; }
    }

    private sealed class OrderRow
    {
        public Guid RulesetOrderId { get; init; }
        public string Id { get; init; } = string.Empty;
        public string Nama { get; init; } = string.Empty;
        public int HargaJual { get; init; }
        public int PoinKebahagiaan { get; init; }
        public int SortOrder { get; init; }
        public int? CardQty { get; init; }
    }

    private sealed class OrderRequirementRow
    {
        public Guid RulesetOrderId { get; init; }
        public int RequirementOrder { get; init; }
        public string IngredientValue { get; init; } = string.Empty;
        public int QtyRequired { get; init; }
    }

    private sealed class CollectionMissionRow
    {
        public Guid RulesetCollectionMissionId { get; init; }
        public string Id { get; init; } = string.Empty;
        public string Nama { get; init; } = string.Empty;
        public int SuccessPoints { get; init; }
        public int FailurePoints { get; init; }
        public int PenaltyPoints { get; init; }
        public int SortOrder { get; init; }
    }

    private sealed class CollectionMissionRequirementRow
    {
        public Guid RulesetCollectionMissionId { get; init; }
        public int RequirementOrder { get; init; }
        public string Type { get; init; } = string.Empty;
        public string Value { get; init; } = string.Empty;
    }

    private sealed class NarrativeRow
    {
        public Guid RulesetNarrativeId { get; init; }
        public string Id { get; init; } = string.Empty;
        public string Nama { get; init; } = string.Empty;
        public string? TextLinesJson { get; init; }
        public int SortOrder { get; init; }
    }

    private sealed class NarrativePrerequisiteRow
    {
        public Guid RulesetNarrativeId { get; init; }
        public string Aksi { get; init; } = string.Empty;
        public int Value { get; init; }
        public int SortOrder { get; init; }
    }

}
