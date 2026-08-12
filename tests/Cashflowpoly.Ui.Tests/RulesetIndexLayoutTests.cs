// Fungsi file: Memverifikasi perilaku, lokalisasi, atau tata letak UI melalui RulesetIndexLayoutTests.
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
            new Regex(@"<details\s+class=""mt-3 data-toggle""\s*>", RegexOptions.CultureInvariant),
            viewContent);
        Assert.Contains("rulesets.tips.title", viewContent);
        Assert.DoesNotContain("rulesets.index.tips_subtitle", viewContent);
        Assert.DoesNotContain("ux-legend", viewContent);
        Assert.DoesNotContain("rulesets.legend.title", viewContent);
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

    [Fact]
    public void RulesetIndexView_ShouldHideTechnicalRulesetIdColumn()
    {
        var repoRoot = ResolveRepositoryRoot();
        var viewContent = File.ReadAllText(Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "Views", "Rulesets", "Index.cshtml"));

        Assert.Contains("showMutationActions ? 7 : 5", viewContent, StringComparison.Ordinal);
        Assert.DoesNotContain("<th class=\"px-4 py-3\">@Context.T(\"common.ruleset_id\")</th>", viewContent, StringComparison.Ordinal);
        Assert.DoesNotContain("data-label=\"@Context.T(\"common.ruleset_id\")\"", viewContent, StringComparison.Ordinal);
    }

    [Fact]
    public void RulesetViews_ShouldVisuallyDistinguishBeginnerAndAdvancedModes()
    {
        var repoRoot = ResolveRepositoryRoot();
        var rulesetViewsRoot = Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "Views", "Rulesets");
        var indexView = File.ReadAllText(Path.Combine(rulesetViewsRoot, "Index.cshtml"));
        var createView = File.ReadAllText(Path.Combine(rulesetViewsRoot, "Create.cshtml"));
        var detailView = File.ReadAllText(Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "Views", "Shared", "_RulesetDetailContent.cshtml"));
        var css = File.ReadAllText(Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "wwwroot", "css", "tailwind.input.css"));

        Assert.Contains("item.Mode", indexView, StringComparison.Ordinal);
        Assert.Contains("ruleset-mode-row--@modeCss", indexView, StringComparison.Ordinal);
        Assert.Contains("ruleset-default-badge", indexView, StringComparison.Ordinal);
        Assert.Contains("name=\"cfg-mode\"", createView, StringComparison.Ordinal);
        Assert.Contains("data-ruleset-mode=\"pemula\"", createView, StringComparison.Ordinal);
        Assert.DoesNotContain("<select id=\"cfg-mode\"", createView, StringComparison.Ordinal);
        Assert.Contains("ruleset-mode-surface--@modeCss", detailView, StringComparison.Ordinal);
        Assert.Contains(".ruleset-mode-badge--beginner", css, StringComparison.Ordinal);
        Assert.Contains(".ruleset-mode-badge--advanced", css, StringComparison.Ordinal);
        Assert.Contains("background: #dbeafe;", css, StringComparison.Ordinal);
        Assert.Contains("color: #1d4ed8;", css, StringComparison.Ordinal);
    }

    [Fact]
    public void RulesetViews_ShouldExposeAdvancedFeaturesOnlyForAdvancedMode()
    {
        var repoRoot = ResolveRepositoryRoot();
        var rulesetViewsRoot = Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "Views", "Rulesets");
        var createView = File.ReadAllText(Path.Combine(rulesetViewsRoot, "Create.cshtml"));
        var detailView = File.ReadAllText(Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "Views", "Shared", "_RulesetDetailContent.cshtml"));

        Assert.Contains("data-ruleset-advanced-features hidden", createView, StringComparison.Ordinal);
        Assert.Contains("setAdvancedFeatureAvailability", createView, StringComparison.Ordinal);
        Assert.Contains("input.disabled = !isAdvanced", createView, StringComparison.Ordinal);
        Assert.Contains("getMode() === \"MAHIR\" && toBool(\"cfg-adv-loan\")", createView, StringComparison.Ordinal);
        Assert.Contains("@if (isAdvancedMode)", detailView, StringComparison.Ordinal);
        Assert.Contains("ruleset-advanced-feature-grid--active", detailView, StringComparison.Ordinal);
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
