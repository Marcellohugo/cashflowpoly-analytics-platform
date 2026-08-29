// Fungsi file: Memverifikasi perilaku, lokalisasi, atau tata letak UI melalui RulesetDetailsLayoutTests.
using System.Text.RegularExpressions;
using Xunit;

namespace Cashflowpoly.Ui.Tests;

public sealed class RulesetDetailsLayoutTests
{
    [Fact]
    public void RulesetModeCallout_ShouldKeepBadgeOnOneLineWhileDescriptionCanShrink()
    {
        var css = File.ReadAllText(Path.Combine(ResolveRepositoryRoot(), "src", "Cashflowpoly.Ui", "wwwroot", "css", "tailwind.input.css"));
        var badgeRule = Regex.Match(css, @"\.ruleset-mode-badge,\s*\.ruleset-default-badge\s*\{(?<body>[^}]+)\}");
        var descriptionRule = Regex.Match(css, @"\.ruleset-mode-callout p\s*\{(?<body>[^}]+)\}");

        Assert.True(badgeRule.Success);
        Assert.Contains("flex: 0 0 auto;", badgeRule.Groups["body"].Value, StringComparison.Ordinal);
        Assert.Contains("min-width: max-content;", badgeRule.Groups["body"].Value, StringComparison.Ordinal);
        Assert.Contains("white-space: nowrap;", badgeRule.Groups["body"].Value, StringComparison.Ordinal);
        Assert.True(descriptionRule.Success);
        Assert.Contains("min-width: 0;", descriptionRule.Groups["body"].Value, StringComparison.Ordinal);
        Assert.Contains("flex: 1 1 auto;", descriptionRule.Groups["body"].Value, StringComparison.Ordinal);
    }

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
