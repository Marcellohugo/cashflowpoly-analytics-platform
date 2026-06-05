using Dapper;
using Cashflowpoly.Api.Data;
using Npgsql;

namespace Cashflowpoly.Api.Infrastructure;

internal static class DatabaseInitialization
{
    public static async Task InitializeAsync(IServiceProvider services, CancellationToken cancellationToken)
    {
        using var scope = services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseInitialization");
        var dataSource = scope.ServiceProvider.GetRequiredService<NpgsqlDataSource>();

        await EnsureSqlSchemaAsync(dataSource, logger, cancellationToken);
        await SeedSqlFileAsync(
            dataSource,
            logger,
            Path.Combine("database", "01_seed_default_rulesets_components.sql"),
            "Default ruleset seed ensured",
            stripPgcryptoExtension: true,
            cancellationToken);
    }

    private static async Task EnsureSqlSchemaAsync(
        NpgsqlDataSource dataSource,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        var schemaPath = ResolveDatabaseFilePath("00_create_schema.sql");
        var schemaSql = await File.ReadAllTextAsync(schemaPath, cancellationToken);

        await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);
        await connection.ExecuteAsync(new CommandDefinition(schemaSql, cancellationToken: cancellationToken));
        logger.LogInformation("Canonical SQL schema ensured from {SchemaPath}", schemaPath);
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

    private static string ResolveDatabaseFilePath(string fileName)
    {
        var candidatePaths = new[]
        {
            Path.Combine(AppContext.BaseDirectory, "database", fileName),
            Path.Combine(Directory.GetCurrentDirectory(), "database", fileName),
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "database", fileName),
            Path.Combine(Directory.GetCurrentDirectory(), "..", "database", fileName)
        };

        foreach (var candidate in candidatePaths)
        {
            var fullPath = Path.GetFullPath(candidate);
            if (File.Exists(fullPath))
            {
                return fullPath;
            }
        }

        throw new FileNotFoundException($"File database '{fileName}' tidak ditemukan.");
    }
}
