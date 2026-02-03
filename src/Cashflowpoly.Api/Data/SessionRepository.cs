using Dapper;
using Npgsql;

namespace Cashflowpoly.Api.Data;

/// <summary>
/// Repository untuk data sesi dan aktivasi ruleset per sesi.
/// </summary>
public sealed class SessionRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public SessionRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<SessionDb?> GetSessionAsync(Guid sessionId, CancellationToken ct)
    {
        const string sql = """
            select session_id, session_name, mode, status, started_at, ended_at, created_at
            from sessions
            where session_id = @sessionId
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        return await conn.QuerySingleOrDefaultAsync<SessionDb>(new CommandDefinition(sql, new { sessionId }, cancellationToken: ct));
    }

    public async Task<Guid> CreateSessionAsync(
        string sessionName,
        string mode,
        Guid rulesetVersionId,
        string? createdBy,
        CancellationToken ct)
    {
        var sessionId = Guid.NewGuid();
        var createdAt = DateTimeOffset.UtcNow;

        const string insertSession = """
            insert into sessions (session_id, session_name, mode, status, started_at, ended_at, created_at)
            values (@sessionId, @sessionName, @mode, 'CREATED', null, null, @createdAt)
            """;

        const string insertActivation = """
            insert into session_ruleset_activations (activation_id, session_id, ruleset_version_id, activated_at, activated_by)
            values (@activationId, @sessionId, @rulesetVersionId, @activatedAt, @activatedBy)
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        await using var tx = await conn.BeginTransactionAsync(ct);

        var def1 = new CommandDefinition(insertSession, new { sessionId, sessionName, mode, createdAt }, tx, cancellationToken: ct);
        await conn.ExecuteAsync(def1);

        var def2 = new CommandDefinition(insertActivation, new
        {
            activationId = Guid.NewGuid(),
            sessionId,
            rulesetVersionId,
            activatedAt = createdAt,
            activatedBy = createdBy
        }, tx, cancellationToken: ct);
        await conn.ExecuteAsync(def2);

        await tx.CommitAsync(ct);
        return sessionId;
    }

    public async Task<bool> UpdateStatusAsync(Guid sessionId, string status, DateTimeOffset? startedAt, DateTimeOffset? endedAt, CancellationToken ct)
    {
        const string sql = """
            update sessions
            set status = @status,
                started_at = @startedAt,
                ended_at = @endedAt
            where session_id = @sessionId
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        var rows = await conn.ExecuteAsync(new CommandDefinition(sql, new { sessionId, status, startedAt, endedAt }, cancellationToken: ct));
        return rows > 0;
    }

    public async Task<Guid?> GetActiveRulesetVersionIdAsync(Guid sessionId, CancellationToken ct)
    {
        const string sql = """
            select ruleset_version_id
            from session_ruleset_activations
            where session_id = @sessionId
            order by activated_at desc
            limit 1
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        return await conn.QuerySingleOrDefaultAsync<Guid?>(new CommandDefinition(sql, new { sessionId }, cancellationToken: ct));
    }

    public async Task<List<SessionDb>> ListSessionsAsync(CancellationToken ct)
    {
        const string sql = """
            select session_id, session_name, mode, status, started_at, ended_at, created_at
            from sessions
            order by created_at desc
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        var items = await conn.QueryAsync<SessionDb>(new CommandDefinition(sql, cancellationToken: ct));
        return items.ToList();
    }

    public async Task ActivateRulesetAsync(Guid sessionId, Guid rulesetVersionId, string? activatedBy, CancellationToken ct)
    {
        const string sql = """
            insert into session_ruleset_activations (activation_id, session_id, ruleset_version_id, activated_at, activated_by)
            values (@activationId, @sessionId, @rulesetVersionId, @activatedAt, @activatedBy)
            """;

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        await conn.ExecuteAsync(new CommandDefinition(sql, new
        {
            activationId = Guid.NewGuid(),
            sessionId,
            rulesetVersionId,
            activatedAt = DateTimeOffset.UtcNow,
            activatedBy
        }, cancellationToken: ct));
    }
}
