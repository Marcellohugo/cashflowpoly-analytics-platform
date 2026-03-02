// Fungsi file: Repository akses data ruleset dan versi ruleset — CRUD, aktivasi, listing, dan hash konfigurasi.
using System.Security.Cryptography;
using System.Text;
using Cashflowpoly.Api.Models;
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
            select ruleset_id, name, description, instructor_user_id, created_at, created_by
            from rulesets
            where ruleset_id = @rulesetId
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
            select ruleset_id, name, description, instructor_user_id, created_at, created_by
            from rulesets
            where ruleset_id = @rulesetId
              and instructor_user_id = @instructorUserId
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
            select ruleset_version_id, ruleset_id, version, status, config_json::text as config_json, config_hash, created_at, created_by
            from ruleset_versions
            where ruleset_id = @rulesetId
            order by version desc
            limit 1
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        return await conn.QuerySingleOrDefaultAsync<RulesetVersionDb>(new CommandDefinition(sql, new { rulesetId }, cancellationToken: ct));
    }

    /// <summary>
    /// Mengambil versi ACTIVE terbaru dari ruleset tertentu.
    /// </summary>
    public async Task<RulesetVersionDb?> GetLatestActiveVersionAsync(Guid rulesetId, CancellationToken ct)
    {
        const string sql = """
            select ruleset_version_id, ruleset_id, version, status, config_json::text as config_json, config_hash, created_at, created_by
            from ruleset_versions
            where ruleset_id = @rulesetId
              and status = 'ACTIVE'
            order by version desc
            limit 1
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        return await conn.QuerySingleOrDefaultAsync<RulesetVersionDb>(new CommandDefinition(sql, new { rulesetId }, cancellationToken: ct));
    }

    /// <summary>
    /// Mengambil versi spesifik dari ruleset berdasarkan nomor versi.
    /// </summary>
    public async Task<RulesetVersionDb?> GetRulesetVersionAsync(Guid rulesetId, int version, CancellationToken ct)
    {
        const string sql = """
            select ruleset_version_id, ruleset_id, version, status, config_json::text as config_json, config_hash, created_at, created_by
            from ruleset_versions
            where ruleset_id = @rulesetId and version = @version
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        return await conn.QuerySingleOrDefaultAsync<RulesetVersionDb>(new CommandDefinition(sql, new { rulesetId, version }, cancellationToken: ct));
    }

    /// <summary>
    /// Mengambil versi ruleset berdasarkan ruleset_version_id.
    /// </summary>
    public async Task<RulesetVersionDb?> GetRulesetVersionByIdAsync(Guid rulesetVersionId, CancellationToken ct)
    {
        const string sql = """
            select ruleset_version_id, ruleset_id, version, status, config_json::text as config_json, config_hash, created_at, created_by
            from ruleset_versions
            where ruleset_version_id = @rulesetVersionId
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        return await conn.QuerySingleOrDefaultAsync<RulesetVersionDb>(new CommandDefinition(sql, new { rulesetVersionId }, cancellationToken: ct));
    }

    /// <summary>
    /// Membuat ruleset baru beserta versi pertama (v1 ACTIVE) dalam transaksi.
    /// </summary>
    public async Task<(Guid RulesetId, int Version)> CreateRulesetAsync(
        string name,
        string? description,
        Guid instructorUserId,
        string configJson,
        string? createdBy,
        CancellationToken ct)
    {
        var rulesetId = Guid.NewGuid();
        var rulesetVersionId = Guid.NewGuid();
        var createdAt = DateTimeOffset.UtcNow;
        var configHash = ComputeHash(configJson);

        const string insertRuleset = """
            insert into rulesets (ruleset_id, name, description, instructor_user_id, created_at, created_by)
            values (@rulesetId, @name, @description, @instructorUserId, @createdAt, @createdBy)
            """;

        const string insertVersion = """
            insert into ruleset_versions (ruleset_version_id, ruleset_id, version, status, config_json, config_hash, created_at, created_by)
            values (@rulesetVersionId, @rulesetId, 1, 'ACTIVE', @configJson::jsonb, @configHash, @createdAt, @createdBy)
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
            createdBy
        }, tx, cancellationToken: ct);
        await conn.ExecuteAsync(def1);

        var def2 = new CommandDefinition(insertVersion, new
        {
            rulesetVersionId,
            rulesetId,
            configJson,
            configHash,
            createdAt,
            createdBy
        }, tx, cancellationToken: ct);
        await conn.ExecuteAsync(def2);

        await tx.CommitAsync(ct);
        return (rulesetId, 1);
    }

    /// <summary>
    /// Membuat versi baru (DRAFT) untuk ruleset yang sudah ada, opsional memperbarui nama dan deskripsi.
    /// </summary>
    public async Task<int> CreateRulesetVersionAsync(
        Guid rulesetId,
        string? name,
        string? description,
        string configJson,
        string? createdBy,
        CancellationToken ct)
    {
        const string lockRulesetSql = """
            select ruleset_id, name, description, instructor_user_id, created_at, created_by
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
            insert into ruleset_versions (ruleset_version_id, ruleset_id, version, status, config_json, config_hash, created_at, created_by)
            values (@rulesetVersionId, @rulesetId, @version, 'DRAFT', @configJson::jsonb, @configHash, @createdAt, @createdBy)
            """;

        var rulesetVersionId = Guid.NewGuid();
        var createdAt = DateTimeOffset.UtcNow;
        var configHash = ComputeHash(configJson);

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
            configJson,
            configHash,
            createdAt,
            createdBy
        }, tx, cancellationToken: ct);
        await conn.ExecuteAsync(insertDef);

        await tx.CommitAsync(ct);
        return nextVersion;
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
            set status = 'RETIRED'
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
    /// Memeriksa apakah versi ruleset sedang digunakan oleh aktivasi sesi, event, atau snapshot.
    /// </summary>
    public async Task<bool> IsRulesetVersionUsedAsync(Guid rulesetVersionId, CancellationToken ct)
    {
        const string sql = """
            select 1
            from (
                select ruleset_version_id from session_ruleset_activations
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
        const string sql = """
            delete from ruleset_versions
            where ruleset_id = @rulesetId
              and version = @version
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        var affected = await conn.ExecuteAsync(
            new CommandDefinition(sql, new { rulesetId, version }, cancellationToken: ct));
        return affected > 0;
    }

    /// <summary>
    /// Mengambil daftar ruleset milik instruktur tertentu beserta versi terbaru dan status.
    /// </summary>
    public async Task<List<RulesetListItem>> ListRulesetsByInstructorAsync(Guid instructorUserId, CancellationToken ct)
    {
        const string sql = """
            with latest_versions as (
                select
                    rv.ruleset_id,
                    rv.version as latest_version,
                    rv.status as latest_status,
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
                coalesce(v.latest_status, 'DRAFT') as status
            from rulesets r
            left join latest_versions v
                on v.ruleset_id = r.ruleset_id and v.rn = 1
            where r.instructor_user_id = @instructorUserId
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
    public async Task<List<RulesetListItem>> ListRulesetsByPlayerAsync(Guid playerId, CancellationToken ct)
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
                'ACTIVE' as status
            from rulesets r
            left join latest_versions v
                on v.ruleset_id = r.ruleset_id and v.rn = 1
            where exists (
                select 1
                from session_players sp
                join session_ruleset_activations sra on sra.session_id = sp.session_id
                join ruleset_versions rv on rv.ruleset_version_id = sra.ruleset_version_id
                where sp.player_id = @playerId
                  and rv.ruleset_id = r.ruleset_id
            )
            order by r.created_at desc
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        var items = await conn.QueryAsync<RulesetListItem>(
            new CommandDefinition(sql, new { playerId }, cancellationToken: ct));
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
                    rv.config_json,
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
                lv.config_json::text as config_json
            from rulesets r
            join latest_versions lv on lv.ruleset_id = r.ruleset_id and lv.rn = 1
            where r.created_by = 'system-seed-components-v1'
              and lv.config_json ? 'component_catalog'
            order by
                case upper(coalesce(lv.config_json->>'mode', ''))
                    when 'PEMULA' then 1
                    when 'MAHIR' then 2
                    else 3
                end,
                r.name asc
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        var items = await conn.QueryAsync<DefaultRulesetComponentDb>(
            new CommandDefinition(sql, cancellationToken: ct));
        return items.ToList();
    }

    /// <summary>
    /// Mengambil data ruleset jika pemain memiliki akses melalui sesi yang menggunakannya.
    /// </summary>
    public async Task<RulesetDb?> GetRulesetForPlayerAsync(Guid rulesetId, Guid playerId, CancellationToken ct)
    {
        const string sql = """
            select r.ruleset_id, r.name, r.description, r.instructor_user_id, r.created_at, r.created_by
            from rulesets r
            where r.ruleset_id = @rulesetId
              and exists (
                  select 1
                  from session_players sp
                  join session_ruleset_activations sra on sra.session_id = sp.session_id
                  join ruleset_versions rv on rv.ruleset_version_id = sra.ruleset_version_id
                  where sp.player_id = @playerId
                    and rv.ruleset_id = r.ruleset_id
              )
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        return await conn.QuerySingleOrDefaultAsync<RulesetDb>(
            new CommandDefinition(sql, new { rulesetId, playerId }, cancellationToken: ct));
    }

    /// <summary>
    /// Mengambil seluruh versi dari ruleset tertentu diurutkan dari terbaru.
    /// </summary>
    public async Task<List<RulesetVersionDb>> ListRulesetVersionsAsync(Guid rulesetId, CancellationToken ct)
    {
        const string sql = """
            select ruleset_version_id, ruleset_id, version, status, config_json::text as config_json, config_hash, created_at, created_by
            from ruleset_versions
            where ruleset_id = @rulesetId
            order by version desc
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        var items = await conn.QueryAsync<RulesetVersionDb>(new CommandDefinition(sql, new { rulesetId }, cancellationToken: ct));
        return items.ToList();
    }

    /// <summary>
    /// Memeriksa apakah ruleset sedang digunakan oleh aktivasi sesi manapun.
    /// </summary>
    public async Task<bool> IsRulesetUsedAsync(Guid rulesetId, CancellationToken ct)
    {
        const string sql = """
            select 1
            from session_ruleset_activations sra
            join ruleset_versions rv on rv.ruleset_version_id = sra.ruleset_version_id
            where rv.ruleset_id = @rulesetId
            limit 1
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        var result = await conn.ExecuteScalarAsync<int?>(new CommandDefinition(sql, new { rulesetId }, cancellationToken: ct));
        return result.HasValue;
    }

    /// <summary>
    /// Menghapus ruleset beserta seluruh versinya (cascade).
    /// </summary>
    public async Task DeleteRulesetAsync(Guid rulesetId, CancellationToken ct)
    {
        const string sql = """
            delete from rulesets
            where ruleset_id = @rulesetId
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        await conn.ExecuteAsync(new CommandDefinition(sql, new { rulesetId }, cancellationToken: ct));
    }

    /// <summary>
    /// Menghitung SHA-256 hash dari string konfigurasi JSON untuk deteksi perubahan.
    /// </summary>
    private static string ComputeHash(string input)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexStringLower(bytes);
    }
}
