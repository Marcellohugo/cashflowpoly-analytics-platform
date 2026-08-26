// Fungsi file: Menerapkan migrasi SQL berurutan dan seed idempoten secara aman.
using System.Security.Cryptography;
using System.Text;
using Dapper;
using Npgsql;

namespace Cashflowpoly.Api.Infrastructure;

internal static class DatabaseInitialization
{
    private const string BaselineName = "canonical_relational_baseline";
    private const string BaselineVersion = "3.0.13";
    private const long MigrationLockKey = 43465348504;

    public static async Task InitializeAsync(
        IServiceProvider services,
        bool applyChanges,
        CancellationToken cancellationToken)
    {
        using var scope = services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseInitialization");
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var dataSource = scope.ServiceProvider.GetRequiredService<NpgsqlDataSource>();
        var migrations = ResolveMigrations(configuration);

        if (!applyChanges)
        {
            await VerifyAppliedMigrationsAsync(dataSource, migrations, cancellationToken);
            logger.LogInformation("Database migration history verified; no schema changes were applied");
            return;
        }

        await ApplyMigrationsAsync(dataSource, migrations, logger, configuration, cancellationToken);
        await dataSource.ReloadTypesAsync(cancellationToken);
    }

    private static async Task ApplyMigrationsAsync(
        NpgsqlDataSource dataSource,
        IReadOnlyList<SqlMigration> migrations,
        ILogger logger,
        IConfiguration configuration,
        CancellationToken cancellationToken)
    {
        await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);
        await connection.ExecuteAsync(new CommandDefinition(
            "select pg_advisory_lock(@lockKey);",
            new { lockKey = MigrationLockKey },
            cancellationToken: cancellationToken));

        try
        {
            var databaseState = await connection.QuerySingleAsync<DatabaseState>(new CommandDefinition(
                "select to_regclass('public.app_users') is not null as HasApplicationSchema, to_regclass('public.schema_history') is not null as HasHistory;",
                cancellationToken: cancellationToken));

            if (!databaseState.HasApplicationSchema)
            {
                var baselinePath = ResolveSqlFilePath("00_create_schema.sql", configuration);
                var baselineSql = await File.ReadAllTextAsync(baselinePath, cancellationToken);
                await connection.ExecuteAsync(new CommandDefinition(baselineSql, cancellationToken: cancellationToken));
                logger.LogInformation("Applied canonical database baseline from {BaselinePath}", baselinePath);
            }
            else if (!databaseState.HasHistory)
            {
                await VerifyCanonicalBaselineAsync(connection, cancellationToken);
            }

            await EnsureHistoryTableAsync(connection, cancellationToken);
            await RecordOrVerifyBaselineAsync(connection, configuration, cancellationToken);

            foreach (var migration in migrations)
            {
                var applied = await connection.QuerySingleOrDefaultAsync<AppliedMigration>(new CommandDefinition(
                    "select version, name, checksum from schema_history where version = @version;",
                    new { migration.Version },
                    cancellationToken: cancellationToken));

                if (applied is not null)
                {
                    EnsureChecksumMatches(applied, migration);
                    continue;
                }

                await using var transaction = await connection.BeginTransactionAsync(cancellationToken);
                await connection.ExecuteAsync(new CommandDefinition(
                    migration.Sql,
                    transaction: transaction,
                    cancellationToken: cancellationToken));
                await connection.ExecuteAsync(new CommandDefinition(
                    "insert into schema_history (version, name, checksum) values (@Version, @Name, @Checksum);",
                    new { migration.Version, migration.Name, migration.Checksum },
                    transaction: transaction,
                    cancellationToken: cancellationToken));
                await transaction.CommitAsync(cancellationToken);
                logger.LogInformation("Applied database migration V{Version}: {Name}", migration.Version, migration.Name);
            }

            await SeedSqlFileAsync(connection, logger, "01_seed_default_rulesets_components.sql", configuration, cancellationToken);
            if (configuration.GetValue<bool>("DatabaseMigrations:SeedSimulation"))
            {
                await SeedSqlFileAsync(connection, logger, "02_seed_simulation_sessions_events.sql", configuration, cancellationToken);
                await connection.ExecuteAsync(new CommandDefinition(
                    "update app_users set is_demo = true where lower(username::text) in ('rina.kartika', 'marco', 'marcello', 'hugo', 'manalu');",
                    cancellationToken: cancellationToken));
            }
        }
        catch
        {
            try
            {
                await connection.ExecuteAsync(new CommandDefinition("rollback;", cancellationToken: cancellationToken));
            }
            catch (PostgresException)
            {
                // Menutup koneksi tetap melepaskan advisory lock jika transaksi gagal sebelum rollback.
            }

            throw;
        }
        finally
        {
            if (connection.FullState == System.Data.ConnectionState.Open)
            {
                await connection.ExecuteAsync(new CommandDefinition(
                    "select pg_advisory_unlock(@lockKey);",
                    new { lockKey = MigrationLockKey },
                    cancellationToken: cancellationToken));
            }
        }
    }

    private static async Task VerifyAppliedMigrationsAsync(
        NpgsqlDataSource dataSource,
        IReadOnlyList<SqlMigration> migrations,
        CancellationToken cancellationToken)
    {
        await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);
        var hasHistory = await connection.ExecuteScalarAsync<bool>(new CommandDefinition(
            "select to_regclass('public.schema_history') is not null;",
            cancellationToken: cancellationToken));
        if (!hasHistory)
        {
            throw new InvalidOperationException("Database belum dimigrasikan. Jalankan aplikasi dengan --migrate-only sebelum menyalakan API.");
        }

        var applied = (await connection.QueryAsync<AppliedMigration>(new CommandDefinition(
            "select version, name, checksum from schema_history order by version;",
            cancellationToken: cancellationToken))).ToDictionary(item => item.Version);

        foreach (var migration in migrations)
        {
            if (!applied.TryGetValue(migration.Version, out var existing))
            {
                throw new InvalidOperationException($"Migrasi V{migration.Version} belum diterapkan. Jalankan --migrate-only.");
            }

            EnsureChecksumMatches(existing, migration);
        }
    }

    private static async Task VerifyCanonicalBaselineAsync(NpgsqlConnection connection, CancellationToken cancellationToken)
    {
        var hasBaselineRegistry = await connection.ExecuteScalarAsync<bool>(new CommandDefinition(
            "select to_regclass('public.schema_baseline_versions') is not null;",
            cancellationToken: cancellationToken));
        if (!hasBaselineRegistry)
        {
            throw new InvalidOperationException("Database lama tidak mempunyai penanda baseline resmi; migrasi dihentikan agar schema tidak ditebak.");
        }

        await connection.ExecuteAsync(new CommandDefinition(
            "select assert_schema_baseline(@baselineName, @baselineVersion);",
            new { baselineName = BaselineName, baselineVersion = BaselineVersion },
            cancellationToken: cancellationToken));
    }

    private static async Task EnsureHistoryTableAsync(NpgsqlConnection connection, CancellationToken cancellationToken)
    {
        const string sql = """
            create table if not exists schema_history (
              version integer not null,
              name varchar(160) not null,
              checksum char(64) not null,
              applied_at timestamptz not null default now(),
              constraint pk_schema_history primary key (version),
              constraint ck_schema_history_version check (version > 0),
              constraint ck_schema_history_name check (nullif(btrim(name), '') is not null),
              constraint ck_schema_history_checksum check (checksum ~ '^[0-9a-f]{64}$')
            );
            """;
        await connection.ExecuteAsync(new CommandDefinition(sql, cancellationToken: cancellationToken));
    }

    private static async Task RecordOrVerifyBaselineAsync(
        NpgsqlConnection connection,
        IConfiguration configuration,
        CancellationToken cancellationToken)
    {
        var baselinePath = ResolveSqlFilePath("00_create_schema.sql", configuration);
        var baseline = CreateMigrationDescriptor(1, BaselineName, baselinePath, includeSql: false);
        var applied = await connection.QuerySingleOrDefaultAsync<AppliedMigration>(new CommandDefinition(
            "select version, name, checksum from schema_history where version = 1;",
            cancellationToken: cancellationToken));

        if (applied is not null)
        {
            EnsureChecksumMatches(applied, baseline);
            return;
        }

        await connection.ExecuteAsync(new CommandDefinition(
            "insert into schema_history (version, name, checksum) values (1, @name, @checksum);",
            new { name = BaselineName, checksum = baseline.Checksum },
            cancellationToken: cancellationToken));
    }

    private static async Task SeedSqlFileAsync(
        NpgsqlConnection connection,
        ILogger logger,
        string fileName,
        IConfiguration configuration,
        CancellationToken cancellationToken)
    {
        var seedPath = ResolveSqlFilePath(fileName, configuration);
        var seedSql = await File.ReadAllTextAsync(seedPath, cancellationToken);
        seedSql = seedSql.Replace("create extension if not exists pgcrypto;", string.Empty, StringComparison.OrdinalIgnoreCase);
        await connection.ExecuteAsync(new CommandDefinition(seedSql, cancellationToken: cancellationToken));
        logger.LogInformation("Applied idempotent seed from {SeedPath}", seedPath);
    }

    private static IReadOnlyList<SqlMigration> ResolveMigrations(IConfiguration configuration)
    {
        var directory = ResolveSqlDirectory("migrations", configuration);
        return Directory.GetFiles(directory, "V*__*.sql")
            .Select(path =>
            {
                var fileName = Path.GetFileName(path);
                var separator = fileName.IndexOf("__", StringComparison.Ordinal);
                if (separator < 2 || !int.TryParse(fileName.AsSpan(1, separator - 1), out var version) || version <= 1)
                {
                    throw new InvalidOperationException($"Nama migrasi '{fileName}' tidak valid. Gunakan VNNN__nama.sql dengan versi di atas 1.");
                }

                var name = Path.GetFileNameWithoutExtension(fileName)[(separator + 2)..].Replace('_', ' ');
                return CreateMigrationDescriptor(version, name, path, includeSql: true);
            })
            .OrderBy(migration => migration.Version)
            .ToArray();
    }

    private static string ResolveSqlDirectory(string directoryName, IConfiguration configuration)
    {
        var markerPath = ResolveSqlFilePath(Path.Combine(directoryName, ".keep"), configuration, requireFile: false);
        var directory = Path.GetDirectoryName(markerPath)!;
        if (!Directory.Exists(directory))
        {
            throw new DirectoryNotFoundException($"Direktori migrasi SQL '{directoryName}' tidak ditemukan.");
        }

        return directory;
    }

    private static string ResolveSqlFilePath(
        string fileName,
        IConfiguration configuration,
        bool requireFile = true)
    {
        var candidateRoots = new List<string>();
        var configuredDirectory = configuration["DatabaseBootstrap:SqlDirectory"];
        if (!string.IsNullOrWhiteSpace(configuredDirectory))
        {
            candidateRoots.Add(configuredDirectory);
        }

        candidateRoots.AddRange(
        [
            Path.Combine(AppContext.BaseDirectory, "artifacts", "runtime-sql"),
            Path.Combine(Directory.GetCurrentDirectory(), "artifacts", "runtime-sql"),
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "artifacts", "runtime-sql"),
            Path.Combine(Directory.GetCurrentDirectory(), "..", "artifacts", "runtime-sql"),
            Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "artifacts", "runtime-sql"),
            Path.Combine(AppContext.BaseDirectory, "database"),
            Path.Combine(Directory.GetCurrentDirectory(), "database"),
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "database"),
            Path.Combine(Directory.GetCurrentDirectory(), "..", "database"),
            Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "database")
        ]);

        foreach (var root in candidateRoots)
        {
            var fullPath = Path.GetFullPath(Path.Combine(root, fileName));
            if (requireFile ? File.Exists(fullPath) : Directory.Exists(Path.GetDirectoryName(fullPath)))
            {
                return fullPath;
            }
        }

        throw new FileNotFoundException($"Aset SQL '{fileName}' tidak ditemukan.");
    }

    private static string ComputeChecksum(byte[] bytes) => Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();

    private static SqlMigration CreateMigrationDescriptor(
        int version,
        string name,
        string path,
        bool includeSql)
    {
        var sql = File.ReadAllText(path);
        var normalized = sql.ReplaceLineEndings("\n");
        var checksums = new HashSet<string>(StringComparer.Ordinal)
        {
            ComputeChecksum(File.ReadAllBytes(path)),
            ComputeChecksum(Encoding.UTF8.GetBytes(normalized)),
            ComputeChecksum(Encoding.UTF8.GetBytes(normalized.Replace("\n", "\r\n", StringComparison.Ordinal)))
        };

        var canonicalChecksum = ComputeChecksum(Encoding.UTF8.GetBytes(normalized));
        return new SqlMigration(
            version,
            name,
            canonicalChecksum,
            includeSql ? sql : string.Empty,
            checksums);
    }

    private static void EnsureChecksumMatches(AppliedMigration applied, SqlMigration expected)
    {
        if (!string.Equals(applied.Name, expected.Name, StringComparison.Ordinal) ||
            !expected.CompatibleChecksums.Contains(applied.Checksum))
        {
            throw new InvalidOperationException(
                $"Checksum migrasi V{expected.Version} berbeda. Migrasi yang sudah diterapkan tidak boleh diedit.");
        }
    }

    private sealed record SqlMigration(
        int Version,
        string Name,
        string Checksum,
        string Sql,
        IReadOnlySet<string> CompatibleChecksums);
    private sealed record AppliedMigration(int Version, string Name, string Checksum);
    private sealed record DatabaseState(bool HasApplicationSchema, bool HasHistory);
}
