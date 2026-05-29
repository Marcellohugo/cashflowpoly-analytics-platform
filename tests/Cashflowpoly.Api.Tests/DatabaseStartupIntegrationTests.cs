using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Cashflowpoly.Contracts;
using Cashflowpoly.Api.Tests.Infrastructure;
using Dapper;
using Microsoft.AspNetCore.Mvc.Testing;
using Npgsql;
using Testcontainers.PostgreSql;
using Xunit;

namespace Cashflowpoly.Api.Tests;

[Collection("ApiIntegration")]
[Trait("Category", "Integration")]
public sealed class DatabaseStartupIntegrationTests
{
    private const string JwtSigningKey = "integration-test-signing-key-with-min-32-char";
    private const string InitialMigrationId = "20260509170152_InitialSchema";
    private static readonly Guid InspectionBeginnerRulesetId = Guid.Parse("72000000-0000-0000-0000-000000000001");
    private static readonly Guid InspectionEndedAdvancedSessionId = Guid.Parse("81000000-0000-0000-0000-000000000002");
    private static readonly Guid InspectionPlayerUlfaId = Guid.Parse("22000000-0000-0000-0000-000000000005");
    private const string InspectionInstructorUsername = "mira.hartanto";
    private const string InspectionInstructorPassword = "MiraAudit!2026";
    private const string InspectionPlayerUsername = "ulfa.ramadhani";
    private const string InspectionPlayerPassword = "UlfaAudit!2026";

    [Fact]
    public async Task ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets()
    {
        await using var database = new PostgreSqlBuilder("postgres:16")
            .WithDatabase("cashflowpoly_empty_boot")
            .WithUsername("cashflowpoly")
            .WithPassword("cashflowpoly")
            .Build();

        await database.StartAsync();
        await RunWithConnectionStringAsync(database.GetConnectionString(), async () =>
        {
            await using var factory = new ApiWebApplicationFactory(database.GetConnectionString(), JwtSigningKey);
            using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var response = await client.GetAsync("/health/ready");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        });

        await using var connection = new NpgsqlConnection(database.GetConnectionString());
        await connection.OpenAsync();

        var hasPgcrypto = await connection.ExecuteScalarAsync<bool>("select exists (select 1 from pg_extension where extname = 'pgcrypto');");
        Assert.True(hasPgcrypto);

        var seededRulesets = await connection.ExecuteScalarAsync<int>(
            "select count(*) from rulesets where created_by = 'system-seed-components-v1';");
        Assert.True(seededRulesets >= 2, $"Expected at least 2 default seeded rulesets, found {seededRulesets}.");

        await AssertHasUniqueIndexAsync(connection, "app_users", ["username"]);
        await AssertHasUniqueIndexAsync(connection, "user_player_links", ["user_id"]);
        await AssertHasUniqueIndexAsync(connection, "session_players", ["session_id", "player_id"]);
        await AssertHasUniqueIndexAsync(connection, "events", ["session_id", "event_id"]);
    }

    [Fact]
    public async Task ApiStartup_OnLegacySqlSchemaWithoutMigrationHistory_BootstrapsAndBecomesReady()
    {
        await using var database = new PostgreSqlBuilder("postgres:16")
            .WithDatabase("cashflowpoly_legacy_boot")
            .WithUsername("cashflowpoly")
            .WithPassword("cashflowpoly")
            .Build();

        await database.StartAsync();
        await ApplyLegacySchemaAsync(database.GetConnectionString());
        await RunWithConnectionStringAsync(database.GetConnectionString(), async () =>
        {
            await using var factory = new ApiWebApplicationFactory(database.GetConnectionString(), JwtSigningKey);
            using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var response = await client.GetAsync("/health/ready");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        });

        await using var connection = new NpgsqlConnection(database.GetConnectionString());
        await connection.OpenAsync();

        var initialMigrationPresent = await connection.ExecuteScalarAsync<bool>(
            """
            select exists (
                select 1
                from "__EFMigrationsHistory"
                where "MigrationId" = @migrationId
            );
            """,
            new { migrationId = InitialMigrationId });

        Assert.True(initialMigrationPresent);
    }

    [Fact]
    public async Task ApiStartup_OnEmptyDatabase_SeedsInspectionDatasetThatSupportsLogin_Analytics_And_AuditChecks()
    {
        await using var database = new PostgreSqlBuilder("postgres:16")
            .WithDatabase("cashflowpoly_inspection_boot")
            .WithUsername("cashflowpoly")
            .WithPassword("cashflowpoly")
            .Build();

        await database.StartAsync();

        await RunWithConnectionStringAsync(database.GetConnectionString(), async () =>
        {
            await using var factory = new ApiWebApplicationFactory(database.GetConnectionString(), JwtSigningKey);
            using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var ready = await client.GetAsync("/health/ready");
            Assert.Equal(HttpStatusCode.OK, ready.StatusCode);

            var instructorLogin = await LoginAsync(client, InspectionInstructorUsername, InspectionInstructorPassword);
            var playerLogin = await LoginAsync(client, InspectionPlayerUsername, InspectionPlayerPassword);

            var instructorSessions = await GetAuthorizedAsync<SessionListResponse>(
                client,
                "/api/v1/sessions",
                instructorLogin.AccessToken);
            Assert.NotNull(instructorSessions);
            Assert.True(instructorSessions!.Items.Count >= 3);
            Assert.True(instructorSessions.Items.Count(item => item.Status == "ENDED") >= 2);
            Assert.True(instructorSessions.Items.Count(item => item.Status == "CREATED") >= 1);
            Assert.Contains(instructorSessions.Items, item => item.SessionId == InspectionEndedAdvancedSessionId);

            var instructorRulesets = await GetAuthorizedAsync<RulesetListResponse>(
                client,
                "/api/v1/rulesets",
                instructorLogin.AccessToken);
            Assert.NotNull(instructorRulesets);
            Assert.True(instructorRulesets!.Items.Count >= 2);
            Assert.Contains(instructorRulesets.Items, item => item.Status == "ACTIVE");

            var beginnerRulesetDetail = await GetAuthorizedAsync<RulesetDetailResponse>(
                client,
                $"/api/v1/rulesets/{InspectionBeginnerRulesetId}",
                instructorLogin.AccessToken);
            Assert.NotNull(beginnerRulesetDetail);
            Assert.Contains(beginnerRulesetDetail!.Versions, item => item.Status == "RETIRED");
            Assert.Contains(beginnerRulesetDetail.Versions, item => item.Status == "ACTIVE");

            var sessionAnalytics = await GetAuthorizedAsync<AnalyticsSessionResponse>(
                client,
                $"/api/v1/analytics/sessions/{InspectionEndedAdvancedSessionId}",
                instructorLogin.AccessToken);
            Assert.NotNull(sessionAnalytics);
            Assert.True(sessionAnalytics!.Summary.EventCount >= 20);
            Assert.True(sessionAnalytics.ByPlayer.Count >= 4);
            Assert.Contains(sessionAnalytics.ByPlayer, item => item.PlayerId == InspectionPlayerUlfaId);

            var gameplayMetrics = await GetAuthorizedAsync<GameplayMetricsResponse>(
                client,
                $"/api/v1/analytics/sessions/{InspectionEndedAdvancedSessionId}/players/{InspectionPlayerUlfaId}/gameplay",
                playerLogin.AccessToken);
            Assert.NotNull(gameplayMetrics);
            Assert.True(gameplayMetrics!.Raw.HasValue);
            Assert.True(gameplayMetrics.Derived.HasValue);
            Assert.True(gameplayMetrics.ComputedAt.HasValue);

            var auditLogs = await GetAuthorizedAsync<SecurityAuditLogResponse>(
                client,
                "/api/v1/security/audit-logs?limit=25",
                instructorLogin.AccessToken);
            Assert.NotNull(auditLogs);
            Assert.True(auditLogs!.Items.Count >= 8);
            Assert.Contains(auditLogs.Items, item => item.TraceId.StartsWith("seed-complex-v4-", StringComparison.Ordinal));
        });

        await using var connection = new NpgsqlConnection(database.GetConnectionString());
        await connection.OpenAsync();

        var inspectionUsers = await connection.ExecuteScalarAsync<int>(
            "select count(*) from app_users where username = any(@usernames);",
            new
            {
                usernames = new[]
                {
                    "mira.hartanto",
                    "bayu.prakasa",
                    "sindy.lestari",
                    "arman.wijaya",
                    "nadia.putri",
                    "rangga.maulana",
                    "safira.anindya",
                    "teo.prasetyo",
                    "ulfa.ramadhani",
                    "vina.anggraini",
                    "wahyu.firmansyah",
                    "xenia.kusuma",
                    "yudha.permana",
                    "zara.nuraini",
                    "adit.suryana",
                    "bella.kartika"
                }
            });
        Assert.True(inspectionUsers >= 16);

        var sessionStatusCounts = await connection.QueryAsync<(string Status, int Count)>(
            """
            select status as Status, count(*) as Count
            from sessions
            where session_id = any(@sessionIds)
            group by status
            """,
            new
            {
                sessionIds = new[]
                {
                    Guid.Parse("81000000-0000-0000-0000-000000000001"),
                    InspectionEndedAdvancedSessionId,
                    Guid.Parse("81000000-0000-0000-0000-000000000003"),
                    Guid.Parse("81000000-0000-0000-0000-000000000004"),
                    Guid.Parse("81000000-0000-0000-0000-000000000005"),
                    Guid.Parse("81000000-0000-0000-0000-000000000006"),
                    Guid.Parse("81000000-0000-0000-0000-000000000007"),
                    Guid.Parse("81000000-0000-0000-0000-000000000008")
                }
            });
        var statusMap = sessionStatusCounts.ToDictionary(item => item.Status, item => item.Count, StringComparer.OrdinalIgnoreCase);
        Assert.True(statusMap["CREATED"] >= 2);
        Assert.True(statusMap["STARTED"] >= 2);
        Assert.True(statusMap["ENDED"] >= 4);

        var invalidValidationLogs = await connection.ExecuteScalarAsync<int>(
            """
            select count(*)
            from validation_logs
            where session_id = any(@sessionIds)
              and is_valid = false
            """,
            new
            {
                sessionIds = new[]
                {
                    InspectionEndedAdvancedSessionId,
                    Guid.Parse("81000000-0000-0000-0000-000000000003"),
                    Guid.Parse("81000000-0000-0000-0000-000000000004"),
                    Guid.Parse("81000000-0000-0000-0000-000000000005"),
                    Guid.Parse("81000000-0000-0000-0000-000000000006"),
                    Guid.Parse("81000000-0000-0000-0000-000000000007")
                }
            });
        Assert.True(invalidValidationLogs >= 8);

        var inspectionMetricSnapshots = await connection.ExecuteScalarAsync<int>(
            """
            select count(*)
            from metric_snapshots
            where session_id = any(@sessionIds)
              and metric_name in ('gameplay.raw.variables', 'gameplay.derived.metrics', 'compliance.primary_need.rate', 'cashflow.net.total', 'happiness.points.total')
            """,
            new
            {
                sessionIds = new[]
                {
                    Guid.Parse("81000000-0000-0000-0000-000000000001"),
                    InspectionEndedAdvancedSessionId,
                    Guid.Parse("81000000-0000-0000-0000-000000000003"),
                    Guid.Parse("81000000-0000-0000-0000-000000000004"),
                    Guid.Parse("81000000-0000-0000-0000-000000000005"),
                    Guid.Parse("81000000-0000-0000-0000-000000000006"),
                    Guid.Parse("81000000-0000-0000-0000-000000000007"),
                    Guid.Parse("81000000-0000-0000-0000-000000000008")
                }
            });
        Assert.True(
            inspectionMetricSnapshots >= 60,
            $"Expected at least 60 selected inspection metric snapshots, found {inspectionMetricSnapshots}.");

        var rulesetVariantCoverage = await connection.QuerySingleAsync<(int FridayDisabled, int SaturdayDisabled)>(
            """
            select
                count(*) filter (where coalesce(config_json->'weekday_rules'->'friday'->>'enabled', 'true') = 'false') as FridayDisabled,
                count(*) filter (where coalesce(config_json->'weekday_rules'->'saturday'->>'enabled', 'true') = 'false') as SaturdayDisabled
            from ruleset_versions
            where ruleset_id = any(@rulesetIds)
            """,
            new
            {
                rulesetIds = new[]
                {
                    Guid.Parse("72000000-0000-0000-0000-000000000001"),
                    Guid.Parse("72000000-0000-0000-0000-000000000002"),
                    Guid.Parse("72000000-0000-0000-0000-000000000003"),
                    Guid.Parse("72000000-0000-0000-0000-000000000004"),
                    Guid.Parse("72000000-0000-0000-0000-000000000005")
                }
            });
        Assert.True(rulesetVariantCoverage.FridayDisabled >= 1);
        Assert.True(rulesetVariantCoverage.SaturdayDisabled >= 1);

        var totalInspectionRows = await connection.QuerySingleAsync<(int Sessions, int Events, int Projections, int Metrics, int Audits)>(
            """
            select
                (select count(*) from sessions) as Sessions,
                (select count(*) from events) as Events,
                (select count(*) from event_cashflow_projections) as Projections,
                (select count(*) from metric_snapshots) as Metrics,
                (select count(*) from security_audit_logs where trace_id like 'seed-complex-v4-%') as Audits
            """);
        Assert.True(totalInspectionRows.Sessions >= 8);
        Assert.True(totalInspectionRows.Events >= 195);
        Assert.True(totalInspectionRows.Projections >= 120);
        Assert.True(totalInspectionRows.Metrics >= 220);
        Assert.True(totalInspectionRows.Audits >= 30);

        var distinctActionTypes = await connection.QueryAsync<string>(
            "select distinct action_type from events order by action_type;");
        var actionTypes = distinctActionTypes.ToList();
        Assert.True(actionTypes.Count >= 28, $"Expected at least 28 distinct action types, found {actionTypes.Count}.");
        Assert.Contains("session.created", actionTypes);
        Assert.Contains("session.started", actionTypes);
        Assert.Contains("session.ended", actionTypes);
        Assert.Contains("turn.action.used", actionTypes);
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

    private static async Task ApplyLegacySchemaAsync(string connectionString)
    {
        var schemaPath = Path.Combine(AppContext.BaseDirectory, "database", "00_create_schema.sql");
        var seedPath = Path.Combine(AppContext.BaseDirectory, "database", "01_seed_default_rulesets_components.sql");

        Assert.True(File.Exists(schemaPath), $"Schema SQL tidak ditemukan: {schemaPath}");
        Assert.True(File.Exists(seedPath), $"Seed SQL tidak ditemukan: {seedPath}");

        var schemaSql = await File.ReadAllTextAsync(schemaPath);
        var seedSql = await File.ReadAllTextAsync(seedPath);

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand($"{schemaSql}{Environment.NewLine}{seedSql}", connection);
        await command.ExecuteNonQueryAsync();
    }

    private static async Task<LoginResponse> LoginAsync(HttpClient client, string username, string password)
    {
        var response = await client.PostAsJsonAsync("/api/v1/auth/login", new LoginRequest(username, password));
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.NotNull(body);
        return body!;
    }

    private static async Task<T> GetAuthorizedAsync<T>(HttpClient client, string path, string accessToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, path);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        using var response = await client.SendAsync(request);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<T>();
        Assert.NotNull(body);
        return body!;
    }

    private static async Task AssertHasUniqueIndexAsync(NpgsqlConnection connection, string tableName, string[] columns)
    {
        const string sql = """
            select exists (
                select 1
                from pg_index i
                join pg_class t on t.oid = i.indrelid
                where t.relname = @tableName
                  and i.indisunique
                  and (
                      select array_agg(a.attname order by ord.ordinality)
                      from unnest(i.indkey) with ordinality as ord(attnum, ordinality)
                      join pg_attribute a on a.attrelid = t.oid and a.attnum = ord.attnum
                      where ord.attnum > 0
                  )::text[] = @columns
            );
            """;

        var hasUniqueIndex = await connection.ExecuteScalarAsync<bool>(sql, new
        {
            tableName,
            columns
        });

        Assert.True(
            hasUniqueIndex,
            $"Expected unique index/constraint on {tableName}({string.Join(", ", columns)}), but none was found.");
    }
}
