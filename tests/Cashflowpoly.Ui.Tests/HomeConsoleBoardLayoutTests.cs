using Xunit;
using System.Text.RegularExpressions;

namespace Cashflowpoly.Ui.Tests;

public sealed class HomeConsoleBoardLayoutTests
{
    [Fact]
    public void HomeIndexView_ShouldUseConsoleBoardHeroStructure()
    {
        var repoRoot = ResolveRepositoryRoot();
        var viewPath = Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "Views", "Home", "Index.cshtml");
        var viewContent = File.ReadAllText(viewPath);

        Assert.Contains("home-console-intro", viewContent);
        Assert.Contains("home-console-header", viewContent);
        Assert.DoesNotContain("home-console-board", viewContent);
        Assert.DoesNotContain("home-console-status", viewContent);
        Assert.DoesNotContain("home-console-timeblock", viewContent);
        Assert.DoesNotContain("mt-6 grid gap-3 text-sm text-slate-600 sm:grid-cols-2", viewContent);
        Assert.DoesNotContain("home.realtime", viewContent);
        Assert.DoesNotContain("home-live-time", viewContent);
        Assert.DoesNotContain("home-last-synced", viewContent);
        Assert.DoesNotContain("home-progress-bar", viewContent);
        Assert.DoesNotContain("h-2 w-full rounded-full bg-slate-100", viewContent);
        Assert.Contains("home-total-sessions", viewContent);
        Assert.Contains("home-active-sessions", viewContent);
        Assert.Contains("home-total-players", viewContent);
        Assert.Contains("home-total-rulesets", viewContent);
        Assert.Contains("home-realtime-error", viewContent);
    }

    [Fact]
    public void SiteCss_ShouldNotClampHomeHeroToNarrowColumnWidths()
    {
        var repoRoot = ResolveRepositoryRoot();
        var cssPath = Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "wwwroot", "css", "site.css");
        var cssContent = File.ReadAllText(cssPath);

        Assert.DoesNotMatch(new Regex(@"\.home-console-copy\s*\{[^}]*max-width:\s*44rem;", RegexOptions.Singleline), cssContent);
        Assert.DoesNotMatch(new Regex(@"\.home-console-subhead\s*\{[^}]*max-width:\s*38rem;", RegexOptions.Singleline), cssContent);
        Assert.DoesNotMatch(new Regex(@"\.home-console-progress-shell\s*\{[^}]*max-width:\s*48rem;", RegexOptions.Singleline), cssContent);
        Assert.DoesNotMatch(new Regex(@"\.home-console-metrics\s*\{[^}]*max-width:\s*52rem;", RegexOptions.Singleline), cssContent);
        Assert.DoesNotMatch(new Regex(@"\.home-console-intro\s+\.headline\s*\{[^}]*max-width:\s*9ch;", RegexOptions.Singleline), cssContent);
        Assert.DoesNotMatch(new Regex(@"\.home-console-intro\s+\.headline\s*\{[^}]*max-width:\s*15ch;", RegexOptions.Singleline), cssContent);
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
