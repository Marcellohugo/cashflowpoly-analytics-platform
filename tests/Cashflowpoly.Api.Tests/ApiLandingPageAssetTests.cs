// Fungsi file: Memverifikasi perilaku API, database, atau domain melalui ApiLandingPageAssetTests.
using Xunit;

namespace Cashflowpoly.Api.Tests;

public sealed class ApiLandingPageAssetTests
{
    private static readonly string RepoRoot = ResolveRepositoryRoot();

    [Fact]
    public void ApiLandingPage_DoesNotAdvertiseSwaggerLinks()
    {
        var indexPath = Path.Combine(RepoRoot, "src", "Cashflowpoly.Api", "wwwroot", "index.html");

        Assert.True(File.Exists(indexPath), "Landing page API harus tersedia di wwwroot/index.html.");

        var content = File.ReadAllText(indexPath);

        Assert.DoesNotContain("href=\"/swagger\"", content, StringComparison.Ordinal);
        Assert.DoesNotContain("href=\"/swagger/v1/swagger.json\"", content, StringComparison.Ordinal);
        Assert.DoesNotContain("cta_swagger", content, StringComparison.Ordinal);
        Assert.DoesNotContain("cta_openapi", content, StringComparison.Ordinal);
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
