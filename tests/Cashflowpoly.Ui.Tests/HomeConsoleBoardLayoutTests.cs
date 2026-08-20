// Fungsi file: Memverifikasi perilaku, lokalisasi, atau tata letak UI melalui HomeConsoleBoardLayoutTests.
using Xunit;
using System.Text.RegularExpressions;

namespace Cashflowpoly.Ui.Tests;

public sealed class HomeCommandCenterLayoutTests
{
    [Fact]
    public void HomeIndexView_ShouldUseCommandCenterHeroStructure()
    {
        var repoRoot = ResolveRepositoryRoot();
        var viewPath = Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "Views", "Home", "Index.cshtml");
        var viewContent = File.ReadAllText(viewPath);

        Assert.Contains("home-command-main", viewContent);
        Assert.Contains("<section class=\"home-hero", viewContent);
        Assert.Contains("home-command-intro", viewContent);
        Assert.Contains("home-live-card", viewContent);
        Assert.Contains("home-session-ring", viewContent);
        Assert.Contains("home-stat-grid", viewContent);
        Assert.Contains("home-guide-shell", viewContent);
        Assert.Contains("<details class=\"home-guide-shell mt-6 data-toggle\">", viewContent);
        Assert.Contains("<details class=\"home-guide-shell mt-6 player-guide data-toggle\">", viewContent);
        Assert.DoesNotContain("home.player_flow", viewContent);
        Assert.DoesNotContain("<details class=\"home-guide-shell mt-6 data-toggle\" open", viewContent);
        Assert.Contains("activeSessionPercent", viewContent);
        Assert.DoesNotContain("home-console-", viewContent);
        Assert.DoesNotContain("section-shell home-hero", viewContent);
        Assert.DoesNotContain("home-command-footer", viewContent);
        Assert.DoesNotContain("home.auto_refresh_note", viewContent);
        Assert.DoesNotContain("section-shell mt-6 player-guide", viewContent);
        Assert.Contains("home-total-sessions", viewContent);
        Assert.Contains("home-active-sessions", viewContent);
        Assert.Contains("home-active-total-sessions", viewContent);
        Assert.DoesNotContain("home-active-percentage", viewContent);
        Assert.DoesNotContain("home-live-summary", viewContent);
        Assert.DoesNotContain("home-stat-mark", viewContent);
        Assert.Contains("home-total-players", viewContent);
        Assert.Contains("home-total-rulesets", viewContent);
        Assert.Contains("home-realtime-error", viewContent);
        Assert.Contains("sessionRingEl.style.setProperty", viewContent);
        Assert.Single(Regex.Matches(viewContent, "id=\"home-active-sessions\""));
    }

    [Fact]
    public void SiteCss_ShouldDefineResponsiveCommandCenterCards()
    {
        var repoRoot = ResolveRepositoryRoot();
        var cssPath = Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "wwwroot", "css", "site.css");
        var cssContent = File.ReadAllText(cssPath);

        Assert.Contains(".home-command-main", cssContent);
        Assert.Contains(".home-session-ring", cssContent);
        Assert.Contains("conic-gradient", cssContent);
        Assert.Contains(".home-stat-card-rulesets", cssContent);
        Assert.Contains(".home-guide-summary-copy", cssContent);
        Assert.Matches(new Regex(@"\.home-guide-shell\s*\{[\s\S]*?background:", RegexOptions.Singleline), cssContent);
        Assert.Contains(".home-guide-shell[open]", cssContent);
        Assert.Contains("grid-template-areas:", cssContent);
        Assert.Contains("\"live-header live-ring\"", cssContent);
        Assert.True(
            cssContent.LastIndexOf("@media (max-width: 1024px)", StringComparison.Ordinal) >
            cssContent.IndexOf(".home-command-main {", cssContent.IndexOf(".home-hero {", StringComparison.Ordinal), StringComparison.Ordinal),
            "Aturan responsif beranda harus didefinisikan setelah aturan dasarnya agar tidak tertimpa cascade CSS.");
        Assert.Matches(new Regex(@"@media \(max-width: 640px\)[\s\S]*?\.home-stat-grid\s*\{\s*grid-template-columns:\s*1fr;", RegexOptions.Singleline), cssContent);
        Assert.DoesNotContain(".home-console-", cssContent);
        Assert.DoesNotContain(".home-command-footer", cssContent);
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
