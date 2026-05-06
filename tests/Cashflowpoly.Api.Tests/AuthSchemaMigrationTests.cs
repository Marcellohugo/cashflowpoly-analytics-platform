// Fungsi file: Menguji kontrak migrasi skema autentikasi agar bootstrap runtime tetap eksplisit dan aman.
using Cashflowpoly.Api.Data;
using Xunit;

namespace Cashflowpoly.Api.Tests;

public sealed class AuthSchemaMigrationTests
{
    [Fact]
    public void AuthSchemaMigrations_HaveStableUniqueIdsAndSql()
    {
        var migrations = AuthSchemaMigrations.All;

        Assert.NotEmpty(migrations);
        Assert.Equal(migrations.Count, migrations.Select(x => x.Id).Distinct(StringComparer.Ordinal).Count());
        Assert.All(migrations, migration =>
        {
            Assert.Matches(@"^\d{12}_[a-z0-9_]+$", migration.Id);
            Assert.False(string.IsNullOrWhiteSpace(migration.Sql));
        });
    }

    [Fact]
    public void AuthSchemaMigrations_RecordAppliedMigrationsInJournal()
    {
        Assert.Contains("create table if not exists schema_migrations", AuthSchemaMigrator.JournalSql);
        Assert.Contains("migration_id", AuthSchemaMigrator.JournalSql);
        Assert.Contains("applied_at", AuthSchemaMigrator.JournalSql);
    }

    [Fact]
    public void PlayerIdentityBackfill_UsesUserIdAsCanonicalPlayerId()
    {
        var migration = Assert.Single(
            AuthSchemaMigrations.All,
            x => x.Id.EndsWith("_player_identity_backfill", StringComparison.Ordinal));

        Assert.Contains("delete from user_player_links upl", migration.Sql);
        Assert.Contains("where upper(u.role) = 'PLAYER'", migration.Sql);
        Assert.Contains("insert into players (player_id, display_name, instructor_user_id, created_at)", migration.Sql);
        Assert.Contains("select u.user_id, u.username", migration.Sql);
        Assert.Contains("insert into user_player_links (link_id, user_id, player_id, created_at)", migration.Sql);
        Assert.Contains("select gen_random_uuid(), u.user_id, u.user_id", migration.Sql);
    }

    [Fact]
    public void Migrations_DoNotUseDestructiveSchemaDrops()
    {
        var combinedSql = string.Join(Environment.NewLine, AuthSchemaMigrations.All.Select(x => x.Sql));

        Assert.DoesNotContain("drop table", combinedSql, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("drop column", combinedSql, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("truncate table", combinedSql, StringComparison.OrdinalIgnoreCase);
    }
}
