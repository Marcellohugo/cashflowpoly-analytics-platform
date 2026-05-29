using Dapper;
using Cashflowpoly.Api.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Cashflowpoly.Api.Infrastructure;

internal static class DatabaseInitialization
{
    private const string InitialMigrationId = "20260509170152_InitialSchema";
    private const string SchemaParityMigrationId = "20260517164422_SchemaParityAndDefaultSeed";
    private const string EfProductVersion = "9.0.4";

    public static async Task InitializeAsync(IServiceProvider services, CancellationToken cancellationToken)
    {
        using var scope = services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseInitialization");
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var dataSource = scope.ServiceProvider.GetRequiredService<NpgsqlDataSource>();

        await BaselineLegacySchemaAsync(dataSource, logger, cancellationToken);
        await db.Database.MigrateAsync(cancellationToken);
        await SeedSqlFileAsync(
            dataSource,
            logger,
            Path.Combine("database", "01_seed_default_rulesets_components.sql"),
            "Default ruleset seed ensured",
            stripPgcryptoExtension: true,
            cancellationToken);
        await SeedSqlFileAsync(
            dataSource,
            logger,
            Path.Combine("database", "02_seed_full_inspection.sql"),
            "Inspection seed ensured",
            stripPgcryptoExtension: false,
            cancellationToken);
    }

    private static async Task BaselineLegacySchemaAsync(
        NpgsqlDataSource dataSource,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        const string ensureHistorySql = """
            create table if not exists "__EFMigrationsHistory" (
              "MigrationId" character varying(150) not null,
              "ProductVersion" character varying(32) not null,
              constraint "PK___EFMigrationsHistory" primary key ("MigrationId")
            );
            """;

        const string insertInitialMigrationSql = """
            with schema_state as (
                select
                    to_regclass('public.app_users') is not null as has_app_users,
                    to_regclass('public.rulesets') is not null as has_rulesets,
                    to_regclass('public.user_player_links') is not null as has_user_player_links
            )
            insert into "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
            select @migrationId, @productVersion
            from schema_state
            where has_app_users and has_rulesets and has_user_player_links
              and not exists (
                  select 1
                  from "__EFMigrationsHistory"
                  where "MigrationId" = @migrationId
              );
            """;

        const string insertParityMigrationSql = """
            with schema_state as (
                select
                    to_regclass('public.app_users') is not null as has_app_users,
                    to_regclass('public.rulesets') is not null as has_rulesets,
                    to_regclass('public.user_player_links') is not null as has_user_player_links,
                    exists (select 1 from pg_extension where extname = 'pgcrypto') as has_pgcrypto,
                    exists (
                        select 1
                        from pg_indexes
                        where schemaname = 'public'
                          and tablename = 'app_users'
                          and indexname = 'app_users_username_key'
                    ) as has_username_unique,
                    exists (
                        select 1
                        from pg_indexes
                        where schemaname = 'public'
                          and tablename = 'session_players'
                          and indexname = 'session_players_session_id_player_id_key'
                    ) as has_session_player_unique,
                    exists (
                        select 1
                        from pg_indexes
                        where schemaname = 'public'
                          and tablename = 'events'
                          and indexname = 'events_session_id_event_id_key'
                    ) as has_event_unique,
                    exists (
                        select 1
                        from pg_indexes
                        where schemaname = 'public'
                          and tablename = 'ruleset_versions'
                          and indexname = 'ruleset_versions_ruleset_id_version_key'
                    ) as has_ruleset_version_unique
            )
            insert into "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
            select @migrationId, @productVersion
            from schema_state
            where has_app_users
              and has_rulesets
              and has_user_player_links
              and has_pgcrypto
              and has_username_unique
              and has_session_player_unique
              and has_event_unique
              and has_ruleset_version_unique
              and not exists (
                  select 1
                  from "__EFMigrationsHistory"
                  where "MigrationId" = @migrationId
              );
            """;

        await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);
        await connection.ExecuteAsync(new CommandDefinition(ensureHistorySql, cancellationToken: cancellationToken));

        var initialInserted = await connection.ExecuteAsync(new CommandDefinition(
            insertInitialMigrationSql,
            new { migrationId = InitialMigrationId, productVersion = EfProductVersion },
            cancellationToken: cancellationToken));

        var parityInserted = await connection.ExecuteAsync(new CommandDefinition(
            insertParityMigrationSql,
            new { migrationId = SchemaParityMigrationId, productVersion = EfProductVersion },
            cancellationToken: cancellationToken));

        if (initialInserted > 0 || parityInserted > 0)
        {
            logger.LogInformation(
                "Legacy SQL schema detected. Baseline migration history inserted. initial={InitialInserted} parity={ParityInserted}",
                initialInserted,
                parityInserted);
        }
    }

    private static async Task SeedSqlFileAsync(
        NpgsqlDataSource dataSource,
        ILogger logger,
        string relativePath,
        string successLogMessage,
        bool stripPgcryptoExtension,
        CancellationToken cancellationToken)
    {
        var seedPath = Path.Combine(AppContext.BaseDirectory, relativePath);
        if (!File.Exists(seedPath))
        {
            throw new FileNotFoundException($"Seed SQL tidak ditemukan pada path '{seedPath}'.");
        }

        var seedSql = await File.ReadAllTextAsync(seedPath, cancellationToken);
        if (stripPgcryptoExtension)
        {
            seedSql = seedSql.Replace("create extension if not exists pgcrypto;", string.Empty, StringComparison.OrdinalIgnoreCase);
        }

        await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);
        await connection.ExecuteAsync(new CommandDefinition(seedSql, cancellationToken: cancellationToken));
        logger.LogInformation("{Message} from {SeedPath}", successLogMessage, seedPath);
    }
}
