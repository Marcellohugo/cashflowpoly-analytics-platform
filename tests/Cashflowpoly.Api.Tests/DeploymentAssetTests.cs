// Fungsi file: Memverifikasi perilaku API, database, atau domain melalui DeploymentAssetTests.
using System.Text.Json;
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
    public void Readme_DescribesVersionedSqlMigrations_InsteadOfEfMigrationStartup()
    {
        var readmePath = Path.Combine(RepoRoot, "README.md");

        Assert.True(File.Exists(readmePath), "README.md harus tersedia pada root repositori.");

        var readme = File.ReadAllText(readmePath);

        Assert.DoesNotContain("migration EF", readme, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("baseline SQL lalu migrasi berurutan", readme, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("schema_history", readme, StringComparison.Ordinal);
        Assert.Contains("--migrate-only", readme, StringComparison.Ordinal);
    }

    [Fact]
    public void PostmanAndDocumentation_ShouldMatchCurrentAuthAndApiContract()
    {
        var collection = File.ReadAllText(Path.Combine(RepoRoot, "postman", "Cashflowpoly.postman_collection.json"));
        var environment = File.ReadAllText(Path.Combine(RepoRoot, "postman", "Cashflowpoly.local.postman_environment.json"));
        var readme = File.ReadAllText(Path.Combine(RepoRoot, "README.md"));
        var docsIndex = File.ReadAllText(Path.Combine(RepoRoot, "docs", "README.md"));

        using var collectionJson = JsonDocument.Parse(collection);
        using var environmentJson = JsonDocument.Parse(environment);

        Assert.Contains("26 Agustus 2026", collection, StringComparison.Ordinal);
        Assert.Contains("Retry Session Setup", collection, StringComparison.Ordinal);
        Assert.Contains("?limit=50", collection, StringComparison.Ordinal);
        Assert.DoesNotContain("fromSeq", collection, StringComparison.Ordinal);
        Assert.Contains("Verify Public Instructor Registration Is Rejected", collection, StringComparison.Ordinal);
        Assert.Contains("'Login': [200]", collection, StringComparison.Ordinal);
        Assert.DoesNotContain("'Login': [200, 401]", collection, StringComparison.Ordinal);
        Assert.DoesNotContain("protectedRequests", collection, StringComparison.Ordinal);
        Assert.Contains("List Session Players", collection, StringComparison.Ordinal);
        Assert.Contains("/api/v1/sessions/{{sessionId}}/players", collection, StringComparison.Ordinal);
        Assert.Contains("deniedInstructorUsername", environment, StringComparison.Ordinal);
        Assert.Contains("skema database **`3.0.13`**", readme, StringComparison.Ordinal);
        Assert.DoesNotContain("file:///", docsIndex, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ProductionAssets_ShouldValidateBootstrapAndNotPublishSwagger()
    {
        var prodEnvironment = File.ReadAllText(Path.Combine(RepoRoot, "config", "env", ".env.prod.example"));
        var readinessScript = File.ReadAllText(Path.Combine(RepoRoot, "scripts", "Test-ProductionReadiness.ps1"));
        var nginx = File.ReadAllText(Path.Combine(RepoRoot, "infra", "nginx", "default.conf"));

        Assert.Contains("AUTH_BOOTSTRAP_SEED_DEFAULT_USERS=false", prodEnvironment, StringComparison.Ordinal);
        Assert.Contains("AUTH_BOOTSTRAP_INSTRUCTOR_USERNAME=", prodEnvironment, StringComparison.Ordinal);
        Assert.Contains("Test-BootstrapCredentialPair", readinessScript, StringComparison.Ordinal);
        Assert.Contains("UTF8.GetByteCount", readinessScript, StringComparison.Ordinal);
        Assert.DoesNotContain("location /swagger", nginx, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("^/swagger/", nginx, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ProductionContainers_ShouldBePinnedNonRootAndKeepMetricsPrivate()
    {
        var compose = File.ReadAllText(Path.Combine(RepoRoot, "infra", "docker", "docker-compose.yml"));
        var production = File.ReadAllText(Path.Combine(RepoRoot, "infra", "docker", "docker-compose.prod.yml"));
        var apiDockerfile = File.ReadAllText(Path.Combine(RepoRoot, "src", "Cashflowpoly.Api", "Dockerfile"));
        var uiDockerfile = File.ReadAllText(Path.Combine(RepoRoot, "src", "Cashflowpoly.Ui", "Dockerfile"));
        var nginx = File.ReadAllText(Path.Combine(RepoRoot, "infra", "nginx", "default.conf"));

        Assert.Contains("postgres:16.15", compose, StringComparison.Ordinal);
        Assert.Contains("nginxinc/nginx-unprivileged:1.31.3-alpine", production, StringComparison.Ordinal);
        Assert.Contains("cloudflare/cloudflared:2026.8.1", production, StringComparison.Ordinal);
        Assert.DoesNotContain(":latest", production, StringComparison.Ordinal);
        Assert.Contains("USER app", apiDockerfile, StringComparison.Ordinal);
        Assert.Contains("USER app", uiDockerfile, StringComparison.Ordinal);
        Assert.Contains("chown -R app:app /home/app/.aspnet", apiDockerfile, StringComparison.Ordinal);
        Assert.Contains("chown -R app:app /home/app/.aspnet", uiDockerfile, StringComparison.Ordinal);
        Assert.Contains("mcr.microsoft.com/dotnet/aspnet:10.0.4", apiDockerfile, StringComparison.Ordinal);
        Assert.Contains("mcr.microsoft.com/dotnet/aspnet:10.0.4", uiDockerfile, StringComparison.Ordinal);
        Assert.Contains("driver: ${LOG_DRIVER:-journald}", production, StringComparison.Ordinal);
        Assert.Contains("no-new-privileges:true", production, StringComparison.Ordinal);
        Assert.Contains("location = /metrics", nginx, StringComparison.Ordinal);
        Assert.Contains("return 404;", nginx, StringComparison.Ordinal);
        Assert.Contains("listen 8080;", nginx, StringComparison.Ordinal);
        Assert.Contains("proxy_set_header Host ui;", nginx, StringComparison.Ordinal);
        Assert.Contains("127.0.0.1:80:8080", production, StringComparison.Ordinal);
        Assert.Contains("http://127.0.0.1:8080/health", production, StringComparison.Ordinal);
    }

    [Fact]
    public void DeploymentScript_ShouldLockMigrateRecalculateAndRollbackImagesOnly()
    {
        var script = File.ReadAllText(Path.Combine(RepoRoot, "scripts", "deploy-production.sh"));

        Assert.Contains("flock -n", script, StringComparison.Ordinal);
        Assert.Contains("--migrate-only", script, StringComparison.Ordinal);
        Assert.Contains("--recalculate-analytics", script, StringComparison.Ordinal);
        Assert.Contains("Host: ${DOMAIN_HOST}", script, StringComparison.Ordinal);
        Assert.Contains("rollback_images", script, StringComparison.Ordinal);
        Assert.Contains("PREVIOUS_SHA", script, StringComparison.Ordinal);
        Assert.Contains("COMPOSE_PROJECT_NAME=${COMPOSE_PROJECT_NAME:-cashflowpoly-analytics-platform}", script, StringComparison.Ordinal);
        Assert.DoesNotContain("--project-name cashflowpoly\n", script, StringComparison.Ordinal);
        Assert.Contains("up -d --no-recreate db", script, StringComparison.Ordinal);
        Assert.Contains("up -d --no-build --force-recreate api ui nginx cloudflared", script, StringComparison.Ordinal);
        Assert.Contains("MaxRetentionSec=30day", script, StringComparison.Ordinal);
        Assert.DoesNotContain("pg_dump", script, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("pg_restore", script, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void OperationalLogMigration_ShouldRemoveRejectedPayloadAndUseThirtyDayRetention()
    {
        var migration = File.ReadAllText(Path.Combine(
            RepoRoot,
            "database",
            "migrations",
            "V004__limit_operational_logs.sql"));
        var repository = File.ReadAllText(Path.Combine(RepoRoot, "src", "Cashflowpoly.Api", "Data", "EventRepository.cs"));

        Assert.Contains("status_code integer", migration, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("trace_id varchar(64)", migration, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("raw_payload_json = '{}'::jsonb", migration, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("('validation_logs', 30)", migration, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("('security_audit_logs', 30)", migration, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("'{}'::jsonb", repository, StringComparison.Ordinal);
        Assert.DoesNotContain("rawPayloadJson", repository, StringComparison.Ordinal);

        var retentionWorker = File.ReadAllText(Path.Combine(
            RepoRoot,
            "src",
            "Cashflowpoly.Api",
            "Infrastructure",
            "LogRetentionWorker.cs"));
        Assert.Contains("PeriodicTimer(TimeSpan.FromHours(24))", retentionWorker, StringComparison.Ordinal);
        Assert.Contains("purge_logs_by_retention", retentionWorker, StringComparison.Ordinal);
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
