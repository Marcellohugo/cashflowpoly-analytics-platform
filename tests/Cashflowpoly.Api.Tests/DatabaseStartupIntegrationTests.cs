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
            "select count(*) from rulesets where created_by = 'system-seed-relational-v2';");
        Assert.True(seededRulesets >= 2, $"Expected at least 2 default seeded rulesets, found {seededRulesets}.");

        var canonicalSchemaState = await connection.QuerySingleAsync<(bool HasCatalogItems, bool HasCatalogRequirements, bool HasPlayerAssets, bool HasDonationEvents, bool HasDisplayName, bool HasPlayersTable, bool HasUserPlayerLinksTable)>(
            """
            select
                to_regclass('public.ruleset_catalog_items') is not null as HasCatalogItems,
                to_regclass('public.ruleset_catalog_item_requirements') is not null as HasCatalogRequirements,
                to_regclass('public.session_player_assets') is not null as HasPlayerAssets,
                to_regclass('public.session_donation_events') is not null as HasDonationEvents,
                exists (
                    select 1
                    from information_schema.columns
                    where table_schema = 'public'
                      and table_name = 'app_users'
                      and column_name = 'display_name'
                ) as HasDisplayName,
                to_regclass('public.players') is not null as HasPlayersTable,
                to_regclass('public.user_player_links') is not null as HasUserPlayerLinksTable
            """);
        Assert.True(canonicalSchemaState.HasCatalogItems);
        Assert.True(canonicalSchemaState.HasCatalogRequirements);
        Assert.True(canonicalSchemaState.HasPlayerAssets);
        Assert.True(canonicalSchemaState.HasDonationEvents);
        Assert.True(canonicalSchemaState.HasDisplayName);
        Assert.False(canonicalSchemaState.HasPlayersTable);
        Assert.False(canonicalSchemaState.HasUserPlayerLinksTable);

        await AssertHasUniqueIndexAsync(connection, "app_users", ["username"]);
        await AssertHasUniqueIndexAsync(connection, "session_players", ["session_id", "user_id"]);
        await AssertHasUniqueIndexAsync(connection, "events", ["session_id", "event_id"]);
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
                drop table if exists session_donation_events cascade;
                drop table if exists session_player_assets cascade;
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

        var canonicalSchemaState = await connection.QuerySingleAsync<(bool HasPlayerAssets, bool HasDonationEvents)>(
            """
            select
                to_regclass('public.session_player_assets') is not null as HasPlayerAssets,
                to_regclass('public.session_donation_events') is not null as HasDonationEvents
            """);
        Assert.True(canonicalSchemaState.HasPlayerAssets);
        Assert.True(canonicalSchemaState.HasDonationEvents);
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
