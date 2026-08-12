// Fungsi file: Menyediakan dukungan infrastruktur API melalui DatabaseInitialization.
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
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var dataSource = scope.ServiceProvider.GetRequiredService<NpgsqlDataSource>();

        await EnsureSqlSchemaAsync(dataSource, logger, configuration, cancellationToken);
        await SeedSqlFileAsync(
            dataSource,
            logger,
            "01_seed_default_rulesets_components.sql",
            "Default ruleset seed ensured",
            stripPgcryptoExtension: true,
            configuration,
            cancellationToken);
    }

    private static async Task EnsureSqlSchemaAsync(
        NpgsqlDataSource dataSource,
        ILogger logger,
        IConfiguration configuration,
        CancellationToken cancellationToken)
    {
        var schemaPath = ResolveSqlFilePath("00_create_schema.sql", configuration);
        var schemaSql = await File.ReadAllTextAsync(schemaPath, cancellationToken);

        await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);
        try
        {
            await connection.ExecuteAsync(new CommandDefinition(schemaSql, cancellationToken: cancellationToken));
        }
        catch (PostgresException ex) when (RequiresSchemaReset(ex))
        {
            throw new InvalidOperationException(
                $"Removed database baseline detected while ensuring '{schemaPath}'. Reset database terlebih dahulu lalu jalankan startup ulang. Detail: {ex.MessageText}",
                ex);
        }

        await dataSource.ReloadTypesAsync(cancellationToken);
        logger.LogInformation("Canonical SQL schema ensured from {SchemaPath}", schemaPath);
    }

    private static async Task SeedSqlFileAsync(
        NpgsqlDataSource dataSource,
        ILogger logger,
        string fileName,
        string successLogMessage,
        bool stripPgcryptoExtension,
        IConfiguration configuration,
        CancellationToken cancellationToken)
    {
        var seedPath = ResolveSqlFilePath(fileName, configuration);

        var seedSql = await File.ReadAllTextAsync(seedPath, cancellationToken);
        if (stripPgcryptoExtension)
        {
            seedSql = seedSql.Replace("create extension if not exists pgcrypto;", string.Empty, StringComparison.OrdinalIgnoreCase);
        }

        await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);
        await connection.ExecuteAsync(new CommandDefinition(seedSql, cancellationToken: cancellationToken));
        logger.LogInformation("{Message} from {SeedPath}", successLogMessage, seedPath);
    }

    private static string ResolveSqlFilePath(string fileName, IConfiguration configuration)
    {
        var candidatePaths = new List<string>();
        var configuredDirectory = configuration["DatabaseBootstrap:SqlDirectory"];
        if (!string.IsNullOrWhiteSpace(configuredDirectory))
        {
            candidatePaths.Add(Path.Combine(configuredDirectory, fileName));
        }

        candidatePaths.AddRange(
        [
            Path.Combine(AppContext.BaseDirectory, "artifacts", "runtime-sql", fileName),
            Path.Combine(Directory.GetCurrentDirectory(), "artifacts", "runtime-sql", fileName),
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "artifacts", "runtime-sql", fileName),
            Path.Combine(Directory.GetCurrentDirectory(), "..", "artifacts", "runtime-sql", fileName),
            Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "artifacts", "runtime-sql", fileName),
            Path.Combine(AppContext.BaseDirectory, "database", fileName),
            Path.Combine(Directory.GetCurrentDirectory(), "database", fileName),
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "database", fileName),
            Path.Combine(Directory.GetCurrentDirectory(), "..", "database", fileName),
            Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "database", fileName)
        ]);

        foreach (var candidate in candidatePaths)
        {
            var fullPath = Path.GetFullPath(candidate);
            if (File.Exists(fullPath))
            {
                return fullPath;
            }
        }

        throw new FileNotFoundException($"File SQL bootstrap '{fileName}' tidak ditemukan.");
    }

    private static bool RequiresSchemaReset(PostgresException ex)
    {
        return ex.MessageText.Contains("reset required", StringComparison.OrdinalIgnoreCase) ||
               ex.MessageText.Contains("removed schema detected", StringComparison.OrdinalIgnoreCase);
    }
}
