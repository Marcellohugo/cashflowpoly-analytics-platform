using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Cashflowpoly.Api.Contracts;
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
            "select count(*) from rulesets where instructor_user_id is null and created_by_user_id is null;");
        Assert.True(seededRulesets >= 2, $"Expected at least 2 default seeded rulesets, found {seededRulesets}.");

        var canonicalSchemaState = await connection.QuerySingleAsync<(
            bool HasParticipants,
            bool HasGameAssets,
            bool HasEventAssetReferences,
            bool HasInventory,
            bool HasGoldHoldings,
            bool HasLoans,
            bool HasInsurances,
            bool HasTriggerConditions,
            bool HasGameSettings,
            bool HasDisplayName,
            bool HasLegacyParticipantAssets,
            bool HasLegacyActionLogs,
            bool HasLegacyInterpreterCommands,
            bool HasLegacyQuestScripts,
            bool HasLegacyNarrativeScripts,
            bool HasLegacyNarrativeAssets,
            bool HasLegacyDonationRankings,
            bool HasLegacyPensionRankings,
            bool HasLegacyPlayersTable,
            bool HasLegacyCatalogTable,
            bool HasConfigJsonColumn,
            bool HasEventInventoryEffectsTable,
            bool HasRulesetCreatedByUserId,
            bool HasRulesetCreatedByVarchar,
            bool HasEventPayloadVersion,
            bool HasArchivedSessionColumn,
            bool HasArchivedRulesetColumn,
            bool HasCitextUsername)>(
            """
            select
                to_regclass('public.session_participants') is not null as HasParticipants,
                to_regclass('public.ruleset_game_assets') is not null as HasGameAssets,
                to_regclass('public.event_asset_references') is not null as HasEventAssetReferences,
                to_regclass('public.session_participant_inventory') is not null as HasInventory,
                to_regclass('public.session_participant_gold_holdings') is not null as HasGoldHoldings,
                to_regclass('public.session_participant_loans') is not null as HasLoans,
                to_regclass('public.session_participant_insurances') is not null as HasInsurances,
                to_regclass('public.ruleset_trigger_conditions') is not null as HasTriggerConditions,
                to_regclass('public.ruleset_game_settings') is not null as HasGameSettings,
                exists (
                    select 1
                    from information_schema.columns
                    where table_schema = 'public'
                      and table_name = 'app_users'
                      and column_name = 'display_name'
                ) as HasDisplayName,
                to_regclass('public.session_participant_assets') is not null as HasLegacyParticipantAssets,
                to_regclass('public.session_action_logs') is not null as HasLegacyActionLogs,
                to_regclass('public.interpreter_commands') is not null as HasLegacyInterpreterCommands,
                to_regclass('public.quest_scripts') is not null as HasLegacyQuestScripts,
                to_regclass('public.narrative_scripts') is not null as HasLegacyNarrativeScripts,
                to_regclass('public.narrative_assets') is not null as HasLegacyNarrativeAssets,
                to_regclass('public.session_donation_event_rankings') is not null as HasLegacyDonationRankings,
                to_regclass('public.session_pension_rankings') is not null as HasLegacyPensionRankings,
                exists (
                    select 1
                    from information_schema.tables
                    where table_schema = 'public'
                      and table_name = 'session_players'
                      and table_type = 'BASE TABLE'
                ) as HasLegacyPlayersTable,
                exists (
                    select 1
                    from information_schema.tables
                    where table_schema = 'public'
                      and table_name = 'ruleset_catalog_items'
                      and table_type = 'BASE TABLE'
                ) as HasLegacyCatalogTable,
                exists (
                    select 1
                    from information_schema.columns
                    where table_schema = 'public'
                      and table_name = 'ruleset_versions'
                      and column_name = 'config_json'
                ) as HasConfigJsonColumn,
                exists (
                    select 1
                    from information_schema.tables
                    where table_schema = 'public'
                      and table_name = 'event_inventory_effects'
                      and table_type = 'BASE TABLE'
                ) as HasEventInventoryEffectsTable,
                exists (
                    select 1
                    from information_schema.columns
                    where table_schema = 'public'
                      and table_name = 'rulesets'
                      and column_name = 'created_by_user_id'
                ) as HasRulesetCreatedByUserId,
                exists (
                    select 1
                    from information_schema.columns
                    where table_schema = 'public'
                      and table_name = 'rulesets'
                      and column_name = 'created_by'
                ) as HasRulesetCreatedByVarchar,
                exists (
                    select 1
                    from information_schema.columns
                    where table_schema = 'public'
                      and table_name = 'events'
                      and column_name = 'payload_version'
                ) as HasEventPayloadVersion,
                exists (
                    select 1
                    from information_schema.columns
                    where table_schema = 'public'
                      and table_name = 'sessions'
                      and column_name = 'is_archived'
                ) as HasArchivedSessionColumn,
                exists (
                    select 1
                    from information_schema.columns
                    where table_schema = 'public'
                      and table_name = 'rulesets'
                      and column_name = 'is_archived'
                ) as HasArchivedRulesetColumn,
                exists (
                    select 1
                    from information_schema.columns
                    where table_schema = 'public'
                      and table_name = 'app_users'
                      and column_name = 'username'
                      and udt_name = 'citext'
                ) as HasCitextUsername
            """);
        Assert.True(canonicalSchemaState.HasParticipants);
        Assert.True(canonicalSchemaState.HasGameAssets);
        Assert.True(canonicalSchemaState.HasEventAssetReferences);
        Assert.True(canonicalSchemaState.HasInventory);
        Assert.True(canonicalSchemaState.HasGoldHoldings);
        Assert.True(canonicalSchemaState.HasLoans);
        Assert.True(canonicalSchemaState.HasInsurances);
        Assert.True(canonicalSchemaState.HasTriggerConditions);
        Assert.True(canonicalSchemaState.HasGameSettings);
        Assert.True(canonicalSchemaState.HasDisplayName);
        Assert.True(canonicalSchemaState.HasCitextUsername);
        Assert.False(canonicalSchemaState.HasLegacyParticipantAssets);
        Assert.False(canonicalSchemaState.HasLegacyActionLogs);
        Assert.False(canonicalSchemaState.HasLegacyInterpreterCommands);
        Assert.False(canonicalSchemaState.HasLegacyQuestScripts);
        Assert.False(canonicalSchemaState.HasLegacyNarrativeScripts);
        Assert.False(canonicalSchemaState.HasLegacyNarrativeAssets);
        Assert.False(canonicalSchemaState.HasLegacyDonationRankings);
        Assert.False(canonicalSchemaState.HasLegacyPensionRankings);
        Assert.False(canonicalSchemaState.HasLegacyPlayersTable);
        Assert.False(canonicalSchemaState.HasLegacyCatalogTable);
        Assert.False(canonicalSchemaState.HasConfigJsonColumn);
        Assert.False(canonicalSchemaState.HasEventInventoryEffectsTable);
        Assert.True(canonicalSchemaState.HasRulesetCreatedByUserId);
        Assert.False(canonicalSchemaState.HasRulesetCreatedByVarchar);
        Assert.True(canonicalSchemaState.HasEventPayloadVersion);
        Assert.True(canonicalSchemaState.HasArchivedSessionColumn);
        Assert.True(canonicalSchemaState.HasArchivedRulesetColumn);

        await AssertHasUniqueIndexAsync(connection, "app_users", ["username"]);
        await AssertHasUniqueIndexAsync(connection, "session_participants", ["session_id", "user_id"]);
        await AssertHasUniqueIndexAsync(connection, "events", ["session_id", "event_id"]);
        await AssertHasUniqueIndexAsync(connection, "events", ["session_id", "client_request_id"]);
    }

    [Fact]
    public async Task ApiStartup_OnPartiallyInitializedDatabase_ReappliesCanonicalSqlSchema()
    {
        await using var database = new PostgreSqlBuilder("postgres:16")
            .WithDatabase("cashflowpoly_partial_boot")
            .WithUsername("cashflowpoly")
            .WithPassword("cashflowpoly")
            .Build();

        await database.StartAsync();

        await using (var setupConnection = new NpgsqlConnection(database.GetConnectionString()))
        {
            await setupConnection.OpenAsync();

            var schemaPath = Path.Combine(AppContext.BaseDirectory, "database", "00_create_schema.sql");
            Assert.True(File.Exists(schemaPath), $"Schema SQL harus tersedia pada path '{schemaPath}'.");

            var schemaSql = await File.ReadAllTextAsync(schemaPath);
            await setupConnection.ExecuteAsync(schemaSql);
            await setupConnection.ExecuteAsync(
                """
                drop table if exists event_asset_references cascade;
                drop table if exists session_participant_gold_holdings cascade;
                drop table if exists session_participant_loans cascade;
                drop table if exists session_participant_insurances cascade;
                drop table if exists session_participant_inventory cascade;
                """);
        }

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

        var canonicalSchemaState = await connection.QuerySingleAsync<(
            bool HasEventAssetReferences,
            bool HasInventory,
            bool HasGoldHoldings,
            bool HasLoans,
            bool HasInsurances)>(
            """
            select
                to_regclass('public.event_asset_references') is not null as HasEventAssetReferences,
                to_regclass('public.session_participant_inventory') is not null as HasInventory,
                to_regclass('public.session_participant_gold_holdings') is not null as HasGoldHoldings,
                to_regclass('public.session_participant_loans') is not null as HasLoans,
                to_regclass('public.session_participant_insurances') is not null as HasInsurances
            """);
        Assert.True(canonicalSchemaState.HasEventAssetReferences);
        Assert.True(canonicalSchemaState.HasInventory);
        Assert.True(canonicalSchemaState.HasGoldHoldings);
        Assert.True(canonicalSchemaState.HasLoans);
        Assert.True(canonicalSchemaState.HasInsurances);
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
