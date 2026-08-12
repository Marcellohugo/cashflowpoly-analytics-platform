// Fungsi file: Memverifikasi perilaku API, database, atau domain melalui DeploymentAssetTests.
using Xunit;

namespace Cashflowpoly.Api.Tests;

public sealed class DeploymentAssetTests
{
    private static readonly string RepoRoot = ResolveRepositoryRoot();

    [Fact]
    public void DockerCompose_DoesNotMountDatabaseInitScripts()
    {
        var composePath = Path.Combine(RepoRoot, "infra", "docker", "docker-compose.yml");

        Assert.True(File.Exists(composePath), "docker-compose.yml harus tersedia pada infra/docker.");

        var content = File.ReadAllText(composePath);

        Assert.DoesNotContain("/docker-entrypoint-initdb.d", content, StringComparison.Ordinal);
        Assert.DoesNotContain("./database:/docker-entrypoint-initdb.d:ro", content, StringComparison.Ordinal);
    }

    [Fact]
    public void DockerComposeWatch_UsesInlineUiWatcherCommand()
    {
        var composePath = Path.Combine(RepoRoot, "infra", "docker", "docker-compose.watch.yml");

        Assert.True(File.Exists(composePath), "docker-compose.watch.yml harus tersedia pada infra/docker.");

        var content = File.ReadAllText(composePath);

        Assert.Contains("npm ci --no-audit --no-fund", content, StringComparison.Ordinal);
        Assert.Contains("npm run tailwind:watch", content, StringComparison.Ordinal);
        Assert.Contains(
            "dotnet watch --non-interactive --project src/Cashflowpoly.Ui/Cashflowpoly.Ui.csproj run --no-launch-profile",
            content,
            StringComparison.Ordinal);
    }

    [Fact]
    public void ApiAndUiProjects_DoNotReferenceSharedContractsProject()
    {
        var apiProjectPath = Path.Combine(RepoRoot, "src", "Cashflowpoly.Api", "Cashflowpoly.Api.csproj");
        var uiProjectPath = Path.Combine(RepoRoot, "src", "Cashflowpoly.Ui", "Cashflowpoly.Ui.csproj");
        var apiDockerfilePath = Path.Combine(RepoRoot, "src", "Cashflowpoly.Api", "Dockerfile");
        var uiDockerfilePath = Path.Combine(RepoRoot, "src", "Cashflowpoly.Ui", "Dockerfile");
        var contractsProjectPath = Path.Combine(RepoRoot, "src", "Cashflowpoly.Contracts", "Cashflowpoly.Contracts.csproj");

        Assert.True(File.Exists(apiProjectPath), "Project API harus tersedia.");
        Assert.True(File.Exists(uiProjectPath), "Project UI harus tersedia.");
        Assert.True(File.Exists(apiDockerfilePath), "Dockerfile API harus tersedia.");
        Assert.True(File.Exists(uiDockerfilePath), "Dockerfile UI harus tersedia.");

        var apiProject = File.ReadAllText(apiProjectPath);
        var uiProject = File.ReadAllText(uiProjectPath);
        var apiDockerfile = File.ReadAllText(apiDockerfilePath);
        var uiDockerfile = File.ReadAllText(uiDockerfilePath);

        Assert.False(File.Exists(contractsProjectPath), "Project Contracts terpisah tidak digunakan lagi.");
        Assert.DoesNotContain("Cashflowpoly.Contracts", apiProject, StringComparison.Ordinal);
        Assert.DoesNotContain("Cashflowpoly.Contracts", uiProject, StringComparison.Ordinal);
        Assert.DoesNotContain("Cashflowpoly.Contracts", apiDockerfile, StringComparison.Ordinal);
        Assert.DoesNotContain("Cashflowpoly.Contracts", uiDockerfile, StringComparison.Ordinal);
    }

    [Fact]
    public void Readme_DoesNotReferenceRemovedInspectionSeedOrDemoCredentials()
    {
        var readmePath = Path.Combine(RepoRoot, "README.md");

        Assert.True(File.Exists(readmePath), "README.md harus tersedia pada root repositori.");

        var readme = File.ReadAllText(readmePath);

        Assert.DoesNotContain("02_seed_full_inspection.sql", readme, StringComparison.Ordinal);
        Assert.DoesNotContain("mira.hartanto", readme, StringComparison.Ordinal);
        Assert.DoesNotContain("MiraAudit!2026", readme, StringComparison.Ordinal);
        Assert.DoesNotContain("ulfa.ramadhani", readme, StringComparison.Ordinal);
        Assert.DoesNotContain("UlfaAudit!2026", readme, StringComparison.Ordinal);
    }

    [Fact]
    public void Readme_DescribesSqlBootstrap_InsteadOfEfMigrationStartup()
    {
        var readmePath = Path.Combine(RepoRoot, "README.md");

        Assert.True(File.Exists(readmePath), "README.md harus tersedia pada root repositori.");

        var readme = File.ReadAllText(readmePath);

        Assert.DoesNotContain("migration EF", readme, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("bootstrap schema SQL kanonik", readme, StringComparison.OrdinalIgnoreCase);
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
