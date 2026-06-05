using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Cashflowpoly.Api.Tests.Infrastructure;
using Cashflowpoly.Api.Contracts;
using Dapper;
using Microsoft.AspNetCore.Mvc.Testing;
using Npgsql;
using Testcontainers.PostgreSql;
using Xunit;

namespace Cashflowpoly.Api.Tests;

[Collection("ApiIntegration")]
[Trait("Category", "Integration")]
public sealed class ManualSimulationSeedIntegrationTests
{
    private const string JwtSigningKey = "integration-test-signing-key-with-min-32-char";
    private const string SeedInstructorUsername = "seed.instructor";
    private const string SeedInstructorPassword = "SeedLocal!2026";

    [Fact]
    public async Task ManualSimulationSeed_WhenAppliedAfterCanonicalSeeds_ProducesLoginableDualModeSessions()
    {
        await using var database = new PostgreSqlBuilder("postgres:16")
            .WithDatabase("cashflowpoly_manual_seed_boot")
            .WithUsername("cashflowpoly")
            .WithPassword("cashflowpoly")
            .Build();

        await database.StartAsync();

        await using (var setupConnection = new NpgsqlConnection(database.GetConnectionString()))
        {
            await setupConnection.OpenAsync();
            await setupConnection.ExecuteAsync(await File.ReadAllTextAsync(Path.Combine(RepoRoot, "database", "00_create_schema.sql")));
            await setupConnection.ExecuteAsync(await File.ReadAllTextAsync(Path.Combine(RepoRoot, "database", "01_seed_default_rulesets_components.sql")));
            await setupConnection.ExecuteAsync(await File.ReadAllTextAsync(Path.Combine(RepoRoot, "database", "02_seed_simulation_sessions_events.sql")));
        }

        await RunWithConnectionStringAsync(database.GetConnectionString(), async () =>
        {
            await using var factory = new ApiWebApplicationFactory(database.GetConnectionString(), JwtSigningKey);
            using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var healthResponse = await client.GetAsync("/health/ready");
            Assert.Equal(HttpStatusCode.OK, healthResponse.StatusCode);

            var loginResponse = await client.PostAsJsonAsync(
                "/api/v1/auth/login",
                new LoginRequest(SeedInstructorUsername, SeedInstructorPassword));
            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

            var loginBody = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();
            Assert.NotNull(loginBody);
            Assert.Equal("INSTRUCTOR", loginBody.Role);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginBody.AccessToken);

            var seedMahirSessionId = Guid.Parse("91000000-0000-0000-0000-000000000002");
            var seedPlayer1UserId = Guid.Parse("90000000-0000-0000-0000-000000000011");
            var txResponse = await client.GetAsync($"/api/v1/analytics/sessions/{seedMahirSessionId}/transactions?userId={seedPlayer1UserId}");
            Assert.Equal(HttpStatusCode.OK, txResponse.StatusCode);

            var txBody = await txResponse.Content.ReadFromJsonAsync<TransactionHistoryResponse>();
            Assert.NotNull(txBody);
            Assert.NotEmpty(txBody.Items);

            var gameplayResponse = await client.GetAsync($"/api/v1/analytics/sessions/{seedMahirSessionId}/players/{seedPlayer1UserId}/gameplay");
            Assert.Equal(HttpStatusCode.OK, gameplayResponse.StatusCode);

            var gameplayBody = await gameplayResponse.Content.ReadFromJsonAsync<GameplayMetricsResponse>();
            Assert.NotNull(gameplayBody);
            Assert.NotNull(gameplayBody.Raw);
            Assert.NotNull(gameplayBody.Derived);
        });

        await using var connection = new NpgsqlConnection(database.GetConnectionString());
        await connection.OpenAsync();

        var sessions = (await connection.QueryAsync<SeedSessionRow>(
            """
            select session_id, session_name, status, mode
            from sessions
            where session_name in ('[SEED] Simulasi Pemula', '[SEED] Simulasi Mahir')
            order by session_name asc
            """)).ToList();

        Assert.Equal(2, sessions.Count);
        Assert.All(sessions, session => Assert.Equal("ENDED", session.Status));

        var playerCounts = (await connection.QueryAsync<SessionPlayerCountRow>(
            """
            select s.session_name, count(*)::int as player_count
            from sessions s
            join session_players sp on sp.session_id = s.session_id
            where s.session_name in ('[SEED] Simulasi Pemula', '[SEED] Simulasi Mahir')
            group by s.session_name
            order by s.session_name asc
            """)).ToDictionary(row => row.SessionName, row => row.PlayerCount);

        Assert.Equal(3, playerCounts["[SEED] Simulasi Pemula"]);
        Assert.Equal(3, playerCounts["[SEED] Simulasi Mahir"]);

        var sequenceChecks = (await connection.QueryAsync<SequenceCheckRow>(
            """
            select
                s.session_name,
                min(e.sequence_number)::bigint as min_sequence,
                max(e.sequence_number)::bigint as max_sequence,
                count(*)::int as event_count
            from sessions s
            join events e on e.session_id = s.session_id
            where s.session_name in ('[SEED] Simulasi Pemula', '[SEED] Simulasi Mahir')
            group by s.session_name
            order by s.session_name asc
            """)).ToList();

        Assert.All(sequenceChecks, row =>
        {
            Assert.Equal(0, row.MinSequence);
            Assert.Equal(row.EventCount - 1, row.MaxSequence);
        });

        var rulesetChecks = (await connection.QueryAsync<RulesetCheckRow>(
            """
            select
                s.session_name,
                max((sra.ruleset_version_id)::text) as activated_ruleset_version_id,
                min((e.ruleset_version_id)::text) as event_ruleset_version_id,
                count(distinct e.ruleset_version_id)::int as event_ruleset_versions
            from sessions s
            join session_ruleset_activations sra on sra.session_id = s.session_id
            join events e on e.session_id = s.session_id
            where s.session_name in ('[SEED] Simulasi Pemula', '[SEED] Simulasi Mahir')
            group by s.session_name
            order by s.session_name asc
            """)).ToDictionary(row => row.SessionName);

        Assert.Equal("f5b4c67b-0825-4970-9f07-3b68e8fcb524", rulesetChecks["[SEED] Simulasi Pemula"].ActivatedRulesetVersionId);
        Assert.Equal("f5b4c67b-0825-4970-9f07-3b68e8fcb524", rulesetChecks["[SEED] Simulasi Pemula"].EventRulesetVersionId);
        Assert.Equal(1, rulesetChecks["[SEED] Simulasi Pemula"].EventRulesetVersions);
        Assert.Equal("7c3bfd8a-27d7-4468-b8d7-cf90131bc61d", rulesetChecks["[SEED] Simulasi Mahir"].ActivatedRulesetVersionId);
        Assert.Equal("7c3bfd8a-27d7-4468-b8d7-cf90131bc61d", rulesetChecks["[SEED] Simulasi Mahir"].EventRulesetVersionId);
        Assert.Equal(1, rulesetChecks["[SEED] Simulasi Mahir"].EventRulesetVersions);

        var calendarChecks = (await connection.QueryAsync<CalendarCheckRow>(
            """
            select
                s.session_name,
                count(distinct e.day_index)::int as day_count,
                min(e.day_index)::int as min_day_index,
                max(e.day_index)::int as max_day_index,
                count(*) filter (
                    where e.weekday <> case (((e.day_index - 1) % 7 + 7) % 7)
                        when 0 then 'MON'
                        when 1 then 'TUE'
                        when 2 then 'WED'
                        when 3 then 'THU'
                        when 4 then 'FRI'
                        when 5 then 'SAT'
                        else 'SUN'
                    end
                )::int as weekday_mismatch_count
            from sessions s
            join events e on e.session_id = s.session_id
            where s.session_name in ('[SEED] Simulasi Pemula', '[SEED] Simulasi Mahir')
            group by s.session_name
            order by s.session_name asc
            """)).ToDictionary(row => row.SessionName);

        Assert.All(calendarChecks.Values, row =>
        {
            Assert.Equal(25, row.DayCount);
            Assert.Equal(1, row.MinDayIndex);
            Assert.Equal(25, row.MaxDayIndex);
            Assert.Equal(0, row.WeekdayMismatchCount);
        });

        var internalTurnActionCount = await connection.ExecuteScalarAsync<int>(
            """
            select count(*)
            from sessions s
            join events e on e.session_id = s.session_id
            where s.session_name in ('[SEED] Simulasi Pemula', '[SEED] Simulasi Mahir')
              and e.action_type = 'turn.action.used'
            """);
        Assert.Equal(0, internalTurnActionCount);

        var pemulaForbiddenCount = await connection.ExecuteScalarAsync<int>(
            """
            select count(*)
            from sessions s
            join events e on e.session_id = s.session_id
            where s.session_name = '[SEED] Simulasi Pemula'
              and (
                e.action_type like 'loan.%'
                or e.action_type like 'insurance.%'
                or e.action_type like 'saving.%'
                or e.action_type = 'risk.life.drawn'
              )
            """);
        Assert.Equal(0, pemulaForbiddenCount);

        var mahirActions = (await connection.QueryAsync<string>(
            """
            select distinct e.action_type
            from sessions s
            join events e on e.session_id = s.session_id
            where s.session_name = '[SEED] Simulasi Mahir'
            """)).ToHashSet(StringComparer.OrdinalIgnoreCase);

        Assert.Contains("loan.syariah.taken", mahirActions);
        Assert.Contains("insurance.multirisk.purchased", mahirActions);
        Assert.Contains("saving.deposit.created", mahirActions);
        Assert.Contains("order.claimed", mahirActions);
        Assert.Contains("risk.life.drawn", mahirActions);
        Assert.Contains("insurance.multirisk.used", mahirActions);

        var unmatchedInsuranceUseCount = await connection.ExecuteScalarAsync<int>(
            """
            with insurance_usage as (
              select
                e.session_id,
                e.user_id,
                (e.payload->>'risk_event_id')::uuid as risk_event_id
              from sessions s
              join events e on e.session_id = s.session_id
              where s.session_name = '[SEED] Simulasi Mahir'
                and e.action_type = 'insurance.multirisk.used'
            )
            select count(*)
            from insurance_usage iu
            left join events risk_evt
              on risk_evt.session_id = iu.session_id
             and risk_evt.event_id = iu.risk_event_id
             and risk_evt.user_id = iu.user_id
             and risk_evt.action_type = 'risk.life.drawn'
             and upper(coalesce(risk_evt.payload->>'direction', '')) = 'OUT'
            where risk_evt.event_id is null
            """);
        Assert.Equal(0, unmatchedInsuranceUseCount);

        var projectionCoverage = (await connection.QueryAsync<PlayerProjectionCoverageRow>(
            """
            select
                s.session_name,
                sp.user_id,
                count(distinct e.event_pk) filter (where e.actor_type = 'PLAYER')::int as player_event_count,
                count(distinct ecp.projection_id)::int as projection_count
            from sessions s
            join session_players sp on sp.session_id = s.session_id
            left join events e
              on e.session_id = sp.session_id
             and e.user_id = sp.user_id
            left join event_cashflow_projections ecp
              on ecp.session_id = sp.session_id
             and ecp.user_id = sp.user_id
            where s.session_name in ('[SEED] Simulasi Pemula', '[SEED] Simulasi Mahir')
            group by s.session_name, sp.user_id
            order by s.session_name asc, sp.user_id asc
            """)).ToList();

        Assert.Equal(6, projectionCoverage.Count);
        Assert.All(projectionCoverage, row =>
        {
            Assert.True(row.PlayerEventCount > 0, $"{row.SessionName}/{row.UserId} tidak punya event pemain.");
            Assert.True(row.ProjectionCount > 0, $"{row.SessionName}/{row.UserId} tidak punya transaksi cashflow.");
        });

        var gameplaySnapshotCoverage = (await connection.QueryAsync<PlayerGameplaySnapshotCoverageRow>(
            """
            select
                s.session_name,
                sp.user_id,
                count(*) filter (where ms.metric_name = 'gameplay.raw.variables')::int as raw_snapshot_count,
                count(*) filter (where ms.metric_name = 'gameplay.derived.metrics')::int as derived_snapshot_count,
                count(*) filter (
                    where ms.metric_name in ('gameplay.raw.variables', 'gameplay.derived.metrics')
                      and ms.metric_payload_json is null
                )::int as empty_payload_count
            from sessions s
            join session_players sp on sp.session_id = s.session_id
            left join metric_snapshots ms
              on ms.session_id = sp.session_id
             and ms.user_id = sp.user_id
             and ms.session_player_id = sp.session_player_id
             and ms.metric_name in ('gameplay.raw.variables', 'gameplay.derived.metrics')
            where s.session_name in ('[SEED] Simulasi Pemula', '[SEED] Simulasi Mahir')
            group by s.session_name, sp.user_id
            order by s.session_name asc, sp.user_id asc
            """)).ToList();

        Assert.Equal(6, gameplaySnapshotCoverage.Count);
        Assert.All(gameplaySnapshotCoverage, row =>
        {
            Assert.True(row.RawSnapshotCount > 0, $"{row.SessionName}/{row.UserId} tidak punya snapshot gameplay raw.");
            Assert.True(row.DerivedSnapshotCount > 0, $"{row.SessionName}/{row.UserId} tidak punya snapshot gameplay derived.");
            Assert.Equal(0, row.EmptyPayloadCount);
        });
    }

    private static async Task RunWithConnectionStringAsync(string connectionString, Func<Task> action)
    {
        var previousConnectionString = Environment.GetEnvironmentVariable("ConnectionStrings__Default");
        var previousJwtSigningKey = Environment.GetEnvironmentVariable("JWT_SIGNING_KEY");
        var previousJwtSectionSigningKey = Environment.GetEnvironmentVariable("Jwt__SigningKey");

        Environment.SetEnvironmentVariable("ConnectionStrings__Default", connectionString);
        Environment.SetEnvironmentVariable("JWT_SIGNING_KEY", JwtSigningKey);
        Environment.SetEnvironmentVariable("Jwt__SigningKey", JwtSigningKey);

        try
        {
            await action();
        }
        finally
        {
            Environment.SetEnvironmentVariable("ConnectionStrings__Default", previousConnectionString);
            Environment.SetEnvironmentVariable("JWT_SIGNING_KEY", previousJwtSigningKey);
            Environment.SetEnvironmentVariable("Jwt__SigningKey", previousJwtSectionSigningKey);
        }
    }

    private static string RepoRoot => ResolveRepositoryRoot();

    private static string ResolveRepositoryRoot()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current is not null)
        {
            if (File.Exists(Path.Combine(current.FullName, "Cashflowpoly.sln")))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new DirectoryNotFoundException("Tidak dapat menemukan root repositori (Cashflowpoly.sln).");
    }

    private sealed class SeedSessionRow
    {
        public Guid SessionId { get; init; }
        public string SessionName { get; init; } = string.Empty;
        public string Status { get; init; } = string.Empty;
        public string Mode { get; init; } = string.Empty;
    }

    private sealed class SessionPlayerCountRow
    {
        public string SessionName { get; init; } = string.Empty;
        public int PlayerCount { get; init; }
    }

    private sealed class SequenceCheckRow
    {
        public string SessionName { get; init; } = string.Empty;
        public long MinSequence { get; init; }
        public long MaxSequence { get; init; }
        public int EventCount { get; init; }
    }

    private sealed class RulesetCheckRow
    {
        public string SessionName { get; init; } = string.Empty;
        public string ActivatedRulesetVersionId { get; init; } = string.Empty;
        public string EventRulesetVersionId { get; init; } = string.Empty;
        public int EventRulesetVersions { get; init; }
    }

    private sealed class CalendarCheckRow
    {
        public string SessionName { get; init; } = string.Empty;
        public int DayCount { get; init; }
        public int MinDayIndex { get; init; }
        public int MaxDayIndex { get; init; }
        public int WeekdayMismatchCount { get; init; }
    }

    private sealed class PlayerProjectionCoverageRow
    {
        public string SessionName { get; init; } = string.Empty;
        public Guid UserId { get; init; }
        public int PlayerEventCount { get; init; }
        public int ProjectionCount { get; init; }
    }

    private sealed class PlayerGameplaySnapshotCoverageRow
    {
        public string SessionName { get; init; } = string.Empty;
        public Guid UserId { get; init; }
        public int RawSnapshotCount { get; init; }
        public int DerivedSnapshotCount { get; init; }
        public int EmptyPayloadCount { get; init; }
    }
}
