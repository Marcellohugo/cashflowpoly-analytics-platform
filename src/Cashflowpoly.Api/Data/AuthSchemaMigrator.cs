// Fungsi file: Menjalankan migrasi skema autentikasi dengan pencatatan journal dan validasi checksum.
using System.Security.Cryptography;
using System.Text;
using Dapper;
using Npgsql;

namespace Cashflowpoly.Api.Data;

/// <summary>
/// Runner migrasi database sederhana untuk SQL autentikasi yang masih perlu berjalan saat startup.
/// </summary>
public static class AuthSchemaMigrator
{
    /// <summary>
    /// SQL untuk membuat journal migrasi agar bootstrap tidak lagi berupa DDL anonim.
    /// </summary>
    public const string JournalSql = """
        create table if not exists schema_migrations (
          migration_id varchar(120) primary key,
          sql_sha256 varchar(64) not null,
          applied_at timestamptz not null default now()
        );
        """;

    private const string AdvisoryLockSql = """
        select pg_advisory_xact_lock(hashtext('cashflowpoly_auth_schema_migrations')::bigint);
        """;

    private const string ReadMigrationHashSql = """
        select sql_sha256
        from schema_migrations
        where migration_id = @migrationId;
        """;

    private const string RecordMigrationSql = """
        insert into schema_migrations (migration_id, sql_sha256, applied_at)
        values (@migrationId, @sqlSha256, now())
        on conflict (migration_id) do nothing;
        """;

    /// <summary>
    /// Menerapkan migrasi yang belum tercatat dengan lock transaksi untuk mencegah race saat multi-instance startup.
    /// </summary>
    public static async Task ApplyAsync(
        NpgsqlConnection connection,
        IReadOnlyList<DatabaseMigration> migrations,
        CancellationToken cancellationToken)
    {
        ValidateMigrations(migrations);

        await connection.ExecuteAsync(new CommandDefinition(JournalSql, cancellationToken: cancellationToken));

        foreach (var migration in migrations)
        {
            var sqlSha256 = ComputeSha256(migration.Sql);

            await using var transaction = await connection.BeginTransactionAsync(cancellationToken);
            await connection.ExecuteAsync(new CommandDefinition(
                AdvisoryLockSql,
                transaction: transaction,
                cancellationToken: cancellationToken));

            var appliedHash = await connection.ExecuteScalarAsync<string?>(new CommandDefinition(
                ReadMigrationHashSql,
                new { migrationId = migration.Id },
                transaction,
                cancellationToken: cancellationToken));

            if (!string.IsNullOrWhiteSpace(appliedHash))
            {
                if (!string.Equals(appliedHash, sqlSha256, StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        $"Migrasi database '{migration.Id}' sudah tercatat dengan checksum berbeda.");
                }

                await transaction.CommitAsync(cancellationToken);
                continue;
            }

            await connection.ExecuteAsync(new CommandDefinition(
                migration.Sql,
                transaction: transaction,
                cancellationToken: cancellationToken));

            await connection.ExecuteAsync(new CommandDefinition(
                RecordMigrationSql,
                new { migrationId = migration.Id, sqlSha256 },
                transaction,
                cancellationToken: cancellationToken));

            await transaction.CommitAsync(cancellationToken);
        }
    }

    private static void ValidateMigrations(IReadOnlyList<DatabaseMigration> migrations)
    {
        var seenIds = new HashSet<string>(StringComparer.Ordinal);

        foreach (var migration in migrations)
        {
            if (string.IsNullOrWhiteSpace(migration.Id))
            {
                throw new InvalidOperationException("ID migrasi database tidak boleh kosong.");
            }

            if (string.IsNullOrWhiteSpace(migration.Sql))
            {
                throw new InvalidOperationException($"SQL migrasi database '{migration.Id}' tidak boleh kosong.");
            }

            if (!seenIds.Add(migration.Id))
            {
                throw new InvalidOperationException($"ID migrasi database '{migration.Id}' terduplikasi.");
            }
        }
    }

    private static string ComputeSha256(string value)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(value));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}
