// Fungsi file: Menerapkan migrasi SQL berurutan dan seed idempoten secara aman.
// Mengimpor namespace `System.Security.Cryptography` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Security.Cryptography;
// Mengimpor namespace `System.Text` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text;
// Mengimpor namespace `Dapper` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Dapper;
// Mengimpor namespace `Npgsql` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Npgsql;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Infrastructure` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Infrastructure;

internal static class DatabaseInitialization
{
    private const string BaselineName = "canonical_relational_baseline";
    private const string BaselineVersion = "3.0.13";
    private const long MigrationLockKey = 43465348504;
    private const int MaintenanceCommandTimeoutSeconds = 300;

    public static async Task InitializeAsync(
        // Parameter `services` bertipe `IServiceProvider` membawa nilai services.
        IServiceProvider services,
        // Parameter `applyChanges` bertipe `bool` membawa nilai apply changes.
        bool applyChanges,
        // Parameter `cancellationToken` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan
        // permintaan atau aplikasi berhenti.
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
        // Parameter `dataSource` bertipe `NpgsqlDataSource` membawa sumber koneksi PostgreSQL yang mengelola pembuatan dan penggunaan ulang koneksi.
        NpgsqlDataSource dataSource,
        // Parameter `migrations` bertipe `IReadOnlyList<SqlMigration>` membawa nilai migrations.
        IReadOnlyList<SqlMigration> migrations,
        // Parameter `logger` bertipe `ILogger` membawa pencatat log terstruktur untuk memantau proses dan mendiagnosis kegagalan.
        ILogger logger,
        // Parameter `configuration` bertipe `IConfiguration` membawa konfigurasi aplikasi yang menyediakan nilai pengaturan dari sumber terdaftar.
        IConfiguration configuration,
        // Parameter `cancellationToken` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan
        // permintaan atau aplikasi berhenti.
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
                await connection.ExecuteAsync(new CommandDefinition(baselineSql, commandTimeout: MaintenanceCommandTimeoutSeconds, cancellationToken: cancellationToken));
                logger.LogInformation("Applied canonical database baseline from {BaselinePath}", baselinePath);
            }
            else if (!databaseState.HasHistory)
            {
                await VerifyCanonicalBaselineAsync(connection, cancellationToken);
            }

            await EnsureHistoryTableAsync(connection, cancellationToken);
            await RecordOrVerifyBaselineAsync(connection, configuration, cancellationToken);

            // Mengulangi setiap elemen `migrations`; elemen saat ini disimpan sebagai `migration` bertipe `var` untuk diproses oleh badan loop dalam
            // ApplyMigrationsAsync.
            foreach (var migration in migrations)
            {
                var applied = await connection.QuerySingleOrDefaultAsync<AppliedMigration>(new CommandDefinition(
                    "select version, name, checksum from schema_history where version = @version;",
                    new { migration.Version },
                    cancellationToken: cancellationToken));

                if (applied is not null)
                {
                    EnsureChecksumMatches(applied, migration);
                    // Melewati sisa pernyataan pada iterasi saat ini dan melanjutkan ke elemen/iterasi berikutnya dalam ApplyMigrationsAsync.
                    continue;
                }

                await using var transaction = await connection.BeginTransactionAsync(cancellationToken);
                await connection.ExecuteAsync(new CommandDefinition(
                    migration.Sql,
                    transaction: transaction,
                    commandTimeout: MaintenanceCommandTimeoutSeconds,
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
            }
        }
        // Menangani exception yang muncul dari blok try sebelumnya dalam ApplyMigrationsAsync.
        catch
        {
            try
            {
                await connection.ExecuteAsync(new CommandDefinition("rollback;", cancellationToken: cancellationToken));
            }
            // Menangani exception `PostgresException` melalui variabel dalam ApplyMigrationsAsync.
            catch (PostgresException)
            {
                // Menutup koneksi tetap melepaskan advisory lock jika transaksi gagal sebelum rollback.
            // Menutup scope penanganan exception catch; bagian berikut berada di luar batas blok tersebut dalam ApplyMigrationsAsync.
            }

            // Melempar ulang exception yang sedang ditangani sambil mempertahankan jejak asal kegagalannya.
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
        // Parameter `dataSource` bertipe `NpgsqlDataSource` membawa sumber koneksi PostgreSQL yang mengelola pembuatan dan penggunaan ulang koneksi.
        NpgsqlDataSource dataSource,
        // Parameter `migrations` bertipe `IReadOnlyList<SqlMigration>` membawa nilai migrations.
        IReadOnlyList<SqlMigration> migrations,
        // Parameter `cancellationToken` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan
        // permintaan atau aplikasi berhenti.
        CancellationToken cancellationToken)
    {
        await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);
        var hasHistory = await connection.ExecuteScalarAsync<bool>(new CommandDefinition(
            "select to_regclass('public.schema_history') is not null;",
            cancellationToken: cancellationToken));
        if (!hasHistory)
        {
            // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen (”Database belum dimigrasikan. Jalankan aplikasi
            // dengan --migrate-only sebelum menyalakan API.”) dalam VerifyAppliedMigrationsAsync; pemanggil atau middleware penanganan error menerima kegagalan
            // ini.
            throw new InvalidOperationException("Database belum dimigrasikan. Jalankan aplikasi dengan --migrate-only sebelum menyalakan API.");
        }

        var applied = (await connection.QueryAsync<AppliedMigration>(new CommandDefinition(
            "select version, name, checksum from schema_history order by version;",
            cancellationToken: cancellationToken))).ToDictionary(item => item.Version);

        // Mengulangi setiap elemen `migrations`; elemen saat ini disimpan sebagai `migration` bertipe `var` untuk diproses oleh badan loop dalam
        // VerifyAppliedMigrationsAsync.
        foreach (var migration in migrations)
        {
            if (!applied.TryGetValue(migration.Version, out var existing))
            {
                // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen ($”Migrasi V{migration.Version} belum diterapkan.
                // Jalankan --migrate-only.”) dalam VerifyAppliedMigrationsAsync; pemanggil atau middleware penanganan error menerima kegagalan ini.
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
            // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen (”Database lama tidak mempunyai penanda baseline
            // resmi; migrasi dihentikan agar schema tidak ditebak.”) dalam VerifyCanonicalBaselineAsync; pemanggil atau middleware penanganan error menerima
            // kegagalan ini.
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
        // Parameter `connection` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data.
        NpgsqlConnection connection,
        // Parameter `configuration` bertipe `IConfiguration` membawa konfigurasi aplikasi yang menyediakan nilai pengaturan dari sumber terdaftar.
        IConfiguration configuration,
        // Parameter `cancellationToken` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan
        // permintaan atau aplikasi berhenti.
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
        // Parameter `connection` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data.
        NpgsqlConnection connection,
        // Parameter `logger` bertipe `ILogger` membawa pencatat log terstruktur untuk memantau proses dan mendiagnosis kegagalan.
        ILogger logger,
        // Parameter `fileName` bertipe `string` membawa nilai file nama.
        string fileName,
        // Parameter `configuration` bertipe `IConfiguration` membawa konfigurasi aplikasi yang menyediakan nilai pengaturan dari sumber terdaftar.
        IConfiguration configuration,
        // Parameter `cancellationToken` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan
        // permintaan atau aplikasi berhenti.
        CancellationToken cancellationToken)
    {
        var seedPath = ResolveSqlFilePath(fileName, configuration);
        var seedSql = await File.ReadAllTextAsync(seedPath, cancellationToken);
        seedSql = seedSql.Replace("create extension if not exists pgcrypto;", string.Empty, StringComparison.OrdinalIgnoreCase);
        await connection.ExecuteAsync(new CommandDefinition(seedSql, commandTimeout: MaintenanceCommandTimeoutSeconds, cancellationToken: cancellationToken));
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
                    // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen ($”Nama migrasi '{fileName}' tidak valid. Gunakan
                    // VNNN__nama.sql dengan versi di atas 1.”) dalam ResolveMigrations; pemanggil atau middleware penanganan error menerima kegagalan ini.
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
            // Menghentikan alur dengan melempar objek baru bertipe `DirectoryNotFoundException` dengan argumen ($”Direktori migrasi SQL '{directoryName}' tidak
            // ditemukan.”) dalam ResolveSqlDirectory; pemanggil atau middleware penanganan error menerima kegagalan ini.
            throw new DirectoryNotFoundException($"Direktori migrasi SQL '{directoryName}' tidak ditemukan.");
        }

        return directory;
    }

    private static string ResolveSqlFilePath(
        // Parameter `fileName` bertipe `string` membawa nilai file nama.
        string fileName,
        // Parameter `configuration` bertipe `IConfiguration` membawa konfigurasi aplikasi yang menyediakan nilai pengaturan dari sumber terdaftar.
        IConfiguration configuration,
        // Parameter `requireFile` bertipe `bool` membawa nilai require file; bila argumen tidak diberikan digunakan true, yaitu kondisi aktif/terpenuhi.
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

        // Mengulangi setiap elemen `candidateRoots`; elemen saat ini disimpan sebagai `root` bertipe `var` untuk diproses oleh badan loop dalam
        // ResolveSqlFilePath.
        foreach (var root in candidateRoots)
        {
            var fullPath = Path.GetFullPath(Path.Combine(root, fileName));
            if (requireFile ? File.Exists(fullPath) : Directory.Exists(Path.GetDirectoryName(fullPath)))
            {
                return fullPath;
            }
        }

        // Menghentikan alur dengan melempar objek baru bertipe `FileNotFoundException` dengan argumen ($”Aset SQL '{fileName}' tidak ditemukan.”) dalam
        // ResolveSqlFilePath; pemanggil atau middleware penanganan error menerima kegagalan ini.
        throw new FileNotFoundException($"Aset SQL '{fileName}' tidak ditemukan.");
    }

    private static string ComputeChecksum(byte[] bytes) => Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();

    private static SqlMigration CreateMigrationDescriptor(
        // Parameter `version` bertipe `int` membawa nomor versi yang dipakai untuk konsistensi data atau konfigurasi.
        int version,
        // Parameter `name` bertipe `string` membawa nilai nama.
        string name,
        // Parameter `path` bertipe `string` membawa nilai path.
        string path,
        // Parameter `includeSql` bertipe `bool` membawa nilai include SQL.
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
            // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen ( $”Checksum migrasi V{expected.Version} berbeda.
            // Migrasi yang sudah diterapkan tidak boleh diedit.”) dalam EnsureChecksumMatches; pemanggil atau middleware penanganan error menerima kegagalan
            // ini.
            throw new InvalidOperationException(
                $"Checksum migrasi V{expected.Version} berbeda. Migrasi yang sudah diterapkan tidak boleh diedit.");
        }
    }

    private sealed record SqlMigration(
        // Parameter `Version` bertipe `int` membawa nomor versi yang dipakai untuk konsistensi data atau konfigurasi.
        int Version,
        // Parameter `Name` bertipe `string` membawa nilai nama.
        string Name,
        // Parameter `Checksum` bertipe `string` membawa nilai checksum.
        string Checksum,
        // Parameter `Sql` bertipe `string` membawa nilai SQL.
        string Sql,
        // Parameter `CompatibleChecksums` bertipe `IReadOnlySet<string>` membawa nilai compatible checksums.
        IReadOnlySet<string> CompatibleChecksums);
    private sealed record AppliedMigration(int Version, string Name, string Checksum);
    private sealed record DatabaseState(bool HasApplicationSchema, bool HasHistory);
}
