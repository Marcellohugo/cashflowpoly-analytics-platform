using System.Text.RegularExpressions;
using Xunit;

namespace Cashflowpoly.Ui.Tests;

public sealed class RulesetIndexLayoutTests
{
    [Fact]
    public void RulesetIndexView_ShouldUseTipsPanelWithoutSeparateGuideShell()
    {
        var repoRoot = ResolveRepositoryRoot();
        var viewPath = Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "Views", "Rulesets", "Index.cshtml");
        var viewContent = File.ReadAllText(viewPath);

        Assert.DoesNotContain("ruleset-index-guide-shell", viewContent);
        Assert.Contains("ruleset-section-title", viewContent);
        Assert.Contains("ruleset-section-subtitle", viewContent);
        Assert.Matches(
            new Regex(@"<details\s+class=""mt-3 data-toggle""\s+open>", RegexOptions.CultureInvariant),
            viewContent);
        Assert.Contains("rulesets.tips.title", viewContent);
        Assert.Contains("rulesets.tip.create", viewContent);
        Assert.Contains("rulesets.tip.versioning", viewContent);
        Assert.Contains("rulesets.tip.safety", viewContent);
        Assert.DoesNotContain("rulesets.default_components.title", viewContent);
        Assert.DoesNotContain("DefaultComponentItems", viewContent);
    }

    [Fact]
    public void RulesetIndexView_ShouldRenderDefaultRowsAsReadonlyInMainTable()
    {
        var repoRoot = ResolveRepositoryRoot();
        var viewPath = Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "Views", "Rulesets", "Index.cshtml");
        var viewContent = File.ReadAllText(viewPath);

        Assert.Contains("item.IsDefault", viewContent, StringComparison.Ordinal);
        Assert.Contains("item.IsLockedBySession", viewContent, StringComparison.Ordinal);
        Assert.Contains("source = item.IsDefault ? DefaultCatalogSource : null", viewContent, StringComparison.Ordinal);
        Assert.Contains("!item.IsDefault && !item.IsLockedBySession", viewContent, StringComparison.Ordinal);
        Assert.DoesNotContain("asp-route-rulesetVersionId", viewContent, StringComparison.Ordinal);
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
