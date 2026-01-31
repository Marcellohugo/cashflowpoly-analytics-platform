using System.Security.Cryptography;
using System.Text;
using Cashflowpoly.Api.Models;
using Dapper;
using Npgsql;

namespace Cashflowpoly.Api.Data;

internal sealed class RulesetRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public RulesetRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<RulesetDb?> GetRulesetAsync(Guid rulesetId, CancellationToken ct)
    {
        const string sql = """
            select ruleset_id, name, description, is_archived, created_at, created_by
            from rulesets
            where ruleset_id = @rulesetId
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        return await conn.QuerySingleOrDefaultAsync<RulesetDb>(new CommandDefinition(sql, new { rulesetId }, cancellationToken: ct));
    }

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

    public async Task<(Guid RulesetId, int Version)> CreateRulesetAsync(
        string name,
        string? description,
        string configJson,
        string? createdBy,
        CancellationToken ct)
    {
        var rulesetId = Guid.NewGuid();
        var rulesetVersionId = Guid.NewGuid();
        var createdAt = DateTimeOffset.UtcNow;
        var configHash = ComputeHash(configJson);

        const string insertRuleset = """
            insert into rulesets (ruleset_id, name, description, is_archived, created_at, created_by)
            values (@rulesetId, @name, @description, false, @createdAt, @createdBy)
            """;

        const string insertVersion = """
            insert into ruleset_versions (ruleset_version_id, ruleset_id, version, status, config_json, config_hash, created_at, created_by)
            values (@rulesetVersionId, @rulesetId, 1, 'ACTIVE', @configJson::jsonb, @configHash, @createdAt, @createdBy)
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        await using var tx = await conn.BeginTransactionAsync(ct);
        var def1 = new CommandDefinition(insertRuleset, new { rulesetId, name, description, createdAt, createdBy }, tx, cancellationToken: ct);
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

    public async Task<int> CreateRulesetVersionAsync(
        Guid rulesetId,
        string? name,
        string? description,
        string configJson,
        string? createdBy,
        CancellationToken ct)
    {
        var latest = await GetLatestVersionAsync(rulesetId, ct);
        var nextVersion = (latest?.Version ?? 0) + 1;
        var rulesetVersionId = Guid.NewGuid();
        var createdAt = DateTimeOffset.UtcNow;
        var configHash = ComputeHash(configJson);

        const string updateRuleset = """
            update rulesets
            set name = @name,
                description = @description
            where ruleset_id = @rulesetId
            """;

        const string retireActive = """
            update ruleset_versions
            set status = 'RETIRED'
            where ruleset_id = @rulesetId and status = 'ACTIVE'
            """;

        const string insertVersion = """
            insert into ruleset_versions (ruleset_version_id, ruleset_id, version, status, config_json, config_hash, created_at, created_by)
            values (@rulesetVersionId, @rulesetId, @version, 'ACTIVE', @configJson::jsonb, @configHash, @createdAt, @createdBy)
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        await using var tx = await conn.BeginTransactionAsync(ct);

        if (name is not null || description is not null)
        {
            var existing = await GetRulesetAsync(rulesetId, ct);
            var updateDef = new CommandDefinition(updateRuleset, new
            {
                rulesetId,
                name = name ?? existing?.Name,
                description = description ?? existing?.Description
            }, tx, cancellationToken: ct);
            await conn.ExecuteAsync(updateDef);
        }

        var retireDef = new CommandDefinition(retireActive, new { rulesetId }, tx, cancellationToken: ct);
        await conn.ExecuteAsync(retireDef);

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

    public async Task<List<RulesetListItem>> ListRulesetsAsync(CancellationToken ct)
    {
        const string sql = """
            select r.ruleset_id, r.name, coalesce(v.latest_version, 0) as latest_version
            from rulesets r
            left join (
                select ruleset_id, max(version) as latest_version
                from ruleset_versions
                group by ruleset_id
            ) v on v.ruleset_id = r.ruleset_id
            order by r.created_at desc
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        var items = await conn.QueryAsync<RulesetListItem>(new CommandDefinition(sql, cancellationToken: ct));
        return items.ToList();
    }

    private static string ComputeHash(string input)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
        var sb = new StringBuilder(bytes.Length * 2);
        foreach (var b in bytes)
        {
            sb.Append(b.ToString("x2"));
        }

        return sb.ToString();
    }
}
