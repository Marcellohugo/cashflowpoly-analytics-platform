// Fungsi file: Memverifikasi perilaku, lokalisasi, atau tata letak UI melalui StandaloneQuickFlowRemovalTests.
using System.Text.RegularExpressions;
using Xunit;

namespace Cashflowpoly.Ui.Tests;

public sealed class StandaloneQuickFlowRemovalTests
{
    private static readonly string RepoRoot = ResolveRepositoryRoot();
    private static readonly string UiRoot = Path.Combine(RepoRoot, "src", "Cashflowpoly.Ui");

    [Fact]
    public void UiViewsAndStyles_ShouldNotRenderStandaloneQuickFlowGuideShells()
    {
        var filesToScan = Directory
            .EnumerateFiles(Path.Combine(UiRoot, "Views"), "*.*", SearchOption.AllDirectories)
            .Concat(new[] { Path.Combine(UiRoot, "wwwroot", "css", "site.css") });
        var removedSelectors = new[]
        {
            "ruleset-detail-guide-shell",
            "ruleset-detail-guide-title",
            "ruleset-detail-guide",
            "sessions-index-guide-shell",
            "sessions-index-guide-title",
            "sessions-index-guide",
            "players-index-guide-shell",
            "players-index-guide-title",
            "players-index-guide",
            "session-detail-guide-shell",
            "session-detail-guide-title",
            "session-detail-guide",
            "player-detail-guide-shell",
            "player-detail-guide-title",
            "player-detail-guide"
        };

        var violations = new List<string>();
        foreach (var filePath in filesToScan)
        {
            var content = File.ReadAllText(filePath);
            foreach (var selector in removedSelectors)
            {
                if (content.Contains(selector, StringComparison.Ordinal))
                {
                    violations.Add($"{Path.GetRelativePath(RepoRoot, filePath).Replace('\\', '/')}: {selector}");
                }
            }
        }

        Assert.True(
            violations.Count == 0,
            $"Standalone quick-flow guide selectors masih ditemukan:{Environment.NewLine}{string.Join(Environment.NewLine, violations)}");
    }

    [Fact]
    public void UiTextLexicon_ShouldNotExposeAlurCepatCopy()
    {
        var lexiconContent = string.Join(
            Environment.NewLine,
            Directory
                .EnumerateFiles(Path.Combine(UiRoot, "Infrastructure"), "UiTextLexicon*.cs", SearchOption.TopDirectoryOnly)
                .Select(File.ReadAllText));

        Assert.DoesNotMatch(new Regex(@"Alur\s+cepat", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant), lexiconContent);
        Assert.DoesNotContain("Alur Penggunaan Cepat", lexiconContent, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void PlayersIndexView_ShouldFoldQuickFlowContentIntoCollapsedTipsPanel()
    {
        var viewPath = Path.Combine(UiRoot, "Views", "Players", "Index.cshtml");
        var viewContent = File.ReadAllText(viewPath);

        Assert.DoesNotContain("players.index.guide", viewContent);
        Assert.Matches(
            new Regex(@"<details\s+class=""mt-3 data-toggle""\s*>", RegexOptions.CultureInvariant),
            viewContent);
        Assert.Contains("players.tip.validate", viewContent);
        Assert.Contains("players.tip.tracking", viewContent);
        Assert.Contains("players.tip.analysis", viewContent);
    }

    [Fact]
    public void HomeIndexView_ShouldRenderUsageGuideLikeCollapsedTipsPanel()
    {
        var viewPath = Path.Combine(UiRoot, "Views", "Home", "Index.cshtml");
        var viewContent = File.ReadAllText(viewPath);

        Assert.Matches(
            new Regex(
                @"<details\s+class=""mt-3 data-toggle""\s*>[\s\S]*home\.quick_flow\.title[\s\S]*home\.quick_flow\.step5[\s\S]*</details>",
                RegexOptions.CultureInvariant),
            viewContent);
        Assert.DoesNotContain("home.quick_flow.subtitle", viewContent);
    }

    [Fact]
    public void UsageTips_ShouldAlwaysStartCollapsed()
    {
        var viewPaths = new[]
        {
            Path.Combine(UiRoot, "Views", "Home", "Index.cshtml"),
            Path.Combine(UiRoot, "Views", "Sessions", "Index.cshtml"),
            Path.Combine(UiRoot, "Views", "Sessions", "Details.cshtml"),
            Path.Combine(UiRoot, "Views", "Players", "Index.cshtml"),
            Path.Combine(UiRoot, "Views", "Rulesets", "Index.cshtml"),
            Path.Combine(UiRoot, "Views", "Rulesets", "Create.cshtml")
        };

        foreach (var viewPath in viewPaths)
        {
            var viewContent = File.ReadAllText(viewPath);
            Assert.Contains("<details class=\"mt-3 data-toggle\">", viewContent, StringComparison.Ordinal);
            Assert.DoesNotContain("<details class=\"mt-3 data-toggle\" open>", viewContent, StringComparison.Ordinal);
        }

        var layout = File.ReadAllText(Path.Combine(UiRoot, "Views", "Shared", "_Layout.cshtml"));
        var siteScript = File.ReadAllText(Path.Combine(UiRoot, "wwwroot", "js", "site.js"));
        var siteCss = File.ReadAllText(Path.Combine(UiRoot, "wwwroot", "css", "site.css"));
        Assert.Contains("quickstart-shell is-collapsed", layout, StringComparison.Ordinal);
        Assert.Contains("data-quickstart-body hidden", layout, StringComparison.Ordinal);
        Assert.Contains("applyState(true);", siteScript, StringComparison.Ordinal);
        Assert.DoesNotContain("cfp_quickstart_collapsed_", siteScript, StringComparison.Ordinal);
        Assert.Contains(".quickstart-shell.is-collapsed .quickstart-head .subhead", siteCss, StringComparison.Ordinal);
        Assert.Contains(".quickstart-shell.is-collapsed .quickstart-title", siteCss, StringComparison.Ordinal);
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
