using Xunit;

namespace Cashflowpoly.Api.Tests;

public sealed class DeploymentAssetTests
{
    private static readonly string RepoRoot = ResolveRepositoryRoot();

    [Fact]
    public void DockerCompose_DoesNotMountDatabaseInitScripts()
    {
        var composePath = Path.Combine(RepoRoot, "docker-compose.yml");

        Assert.True(File.Exists(composePath), "docker-compose.yml harus tersedia pada root repositori.");

        var content = File.ReadAllText(composePath);

        Assert.DoesNotContain("/docker-entrypoint-initdb.d", content, StringComparison.Ordinal);
        Assert.DoesNotContain("./database:/docker-entrypoint-initdb.d:ro", content, StringComparison.Ordinal);
    }

    [Fact]
    public void DockerComposeWatch_UsesInlineUiWatcherCommand()
    {
        var composePath = Path.Combine(RepoRoot, "docker-compose.watch.yml");

        Assert.True(File.Exists(composePath), "docker-compose.watch.yml harus tersedia.");

        var content = File.ReadAllText(composePath);

        Assert.Contains("npm ci --no-audit --no-fund", content, StringComparison.Ordinal);
        Assert.Contains("npm run tailwind:watch", content, StringComparison.Ordinal);
        Assert.Contains(
            "dotnet watch --non-interactive --project src/Cashflowpoly.Ui/Cashflowpoly.Ui.csproj run --no-launch-profile",
            content,
            StringComparison.Ordinal);
    }

    [Fact]
    public void InspectionSeedAsset_IsDocumentedForLocalVerification()
    {
        var seedPath = Path.Combine(RepoRoot, "database", "02_seed_full_inspection.sql");
        var readmePath = Path.Combine(RepoRoot, "README.md");

        Assert.True(File.Exists(seedPath), "Seed inspeksi penuh harus tersedia pada folder database.");
        Assert.True(File.Exists(readmePath), "README.md harus tersedia pada root repositori.");

        var readme = File.ReadAllText(readmePath);

        Assert.Contains("02_seed_full_inspection.sql", readme, StringComparison.Ordinal);
        Assert.Contains("mira.hartanto", readme, StringComparison.Ordinal);
        Assert.Contains("MiraAudit!2026", readme, StringComparison.Ordinal);
        Assert.Contains("ulfa.ramadhani", readme, StringComparison.Ordinal);
        Assert.Contains("UlfaAudit!2026", readme, StringComparison.Ordinal);
    }

    private static string ResolveRepositoryRoot()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current is not null)
        {
            if (File.Exists(Path.Combine(current.FullName, "Cashflowpoly.sln")))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new DirectoryNotFoundException("Tidak dapat menemukan root repositori (Cashflowpoly.sln).");
    }
}
