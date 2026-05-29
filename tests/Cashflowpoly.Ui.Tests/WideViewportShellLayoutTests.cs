using Xunit;

namespace Cashflowpoly.Ui.Tests;

public sealed class WideViewportShellLayoutTests
{
    [Fact]
    public void SiteCss_ShouldUseSharedWideShellWidthForNavPageAndFooter()
    {
        var repoRoot = ResolveRepositoryRoot();
        var cssPath = Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "wwwroot", "css", "site.css");
        var cssContent = File.ReadAllText(cssPath);

        Assert.Contains("--shell-max-width:", cssContent);
        Assert.Contains("--shell-gutter:", cssContent);
        Assert.Contains(".nav-shell,", cssContent);
        Assert.Contains(".page-shell,", cssContent);
        Assert.Contains("body > footer .footer-shell {", cssContent);
        Assert.Contains("width: min(var(--shell-max-width), calc(100% - (var(--shell-gutter) * 2)));", cssContent);
        Assert.DoesNotContain("width: min(1280px, calc(100% - 2rem));", cssContent);
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
