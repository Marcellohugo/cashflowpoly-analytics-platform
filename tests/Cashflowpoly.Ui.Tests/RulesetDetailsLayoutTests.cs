// Fungsi file: Memverifikasi perilaku, lokalisasi, atau tata letak UI melalui RulesetDetailsLayoutTests.
using Xunit;

namespace Cashflowpoly.Ui.Tests;

public sealed class RulesetDetailsLayoutTests
{
    [Fact]
    public void RulesetDetailsView_ShouldNotRenderComponentCatalogSection()
    {
        var repoRoot = ResolveRepositoryRoot();
        var viewPath = Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "Views", "Shared", "_RulesetDetailContent.cshtml");
        var viewContent = File.ReadAllText(viewPath);

        Assert.DoesNotContain("rulesets.components.title", viewContent, StringComparison.Ordinal);
        Assert.DoesNotContain("rulesets.components.raw_json", viewContent, StringComparison.Ordinal);
        Assert.DoesNotContain("CompatibilityComponentCatalog", viewContent, StringComparison.Ordinal);
        Assert.Contains("rulesets.config_summary", viewContent, StringComparison.Ordinal);
        Assert.DoesNotContain("ruleset-detail-id-label", viewContent, StringComparison.Ordinal);
        Assert.DoesNotContain("id-chip", viewContent, StringComparison.Ordinal);
        Assert.DoesNotContain("ruleset-overview-grid", viewContent, StringComparison.Ordinal);
        Assert.Contains("displayDescription", viewContent, StringComparison.Ordinal);
        Assert.Contains("rulesets.default_description_advanced", viewContent, StringComparison.Ordinal);
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

        throw new InvalidOperationException("Repository root tidak ditemukan.");
    }
}
