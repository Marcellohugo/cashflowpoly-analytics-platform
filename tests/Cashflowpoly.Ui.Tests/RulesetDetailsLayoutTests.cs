using Xunit;

namespace Cashflowpoly.Ui.Tests;

public sealed class RulesetDetailsLayoutTests
{
    [Fact]
    public void RulesetDetailsView_ShouldNotRenderComponentCatalogSection()
    {
        var repoRoot = ResolveRepositoryRoot();
        var viewPath = Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "Views", "Rulesets", "Details.cshtml");
        var viewContent = File.ReadAllText(viewPath);

        Assert.DoesNotContain("rulesets.components.title", viewContent, StringComparison.Ordinal);
        Assert.DoesNotContain("rulesets.components.raw_json", viewContent, StringComparison.Ordinal);
        Assert.DoesNotContain("CompatibilityComponentCatalog", viewContent, StringComparison.Ordinal);
        Assert.Contains("rulesets.config_summary", viewContent, StringComparison.Ordinal);
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
