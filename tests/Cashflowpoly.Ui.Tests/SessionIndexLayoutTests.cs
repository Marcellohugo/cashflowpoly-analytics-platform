// Fungsi file: Memverifikasi perilaku, lokalisasi, atau tata letak UI melalui SessionIndexLayoutTests.
using System.Text.RegularExpressions;
using Xunit;

namespace Cashflowpoly.Ui.Tests;

public sealed class SessionIndexLayoutTests
{
    [Fact]
    public void SiteCss_ShouldRenderSessionSummaryCardsWithResponsiveColumns()
    {
        var repoRoot = ResolveRepositoryRoot();
        var cssPath = Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "wwwroot", "css", "site.css");
        var cssContent = File.ReadAllText(cssPath);

        Assert.Matches(
            new Regex(
                @"\.session-stats-grid\s*\{[^}]*grid-template-columns:\s*1fr\s*!important;",
                RegexOptions.Singleline),
            cssContent);
        Assert.Matches(
            new Regex(
                @"@media\s*\(min-width:\s*640px\)\s*\{\s*\.session-stats-grid\s*\{[^}]*grid-template-columns:\s*repeat\(2,\s*minmax\(0,\s*1fr\)\)\s*!important;",
                RegexOptions.Singleline),
            cssContent);
        Assert.Matches(
            new Regex(
                @"@media\s*\(min-width:\s*1024px\)\s*\{\s*\.session-stats-grid\s*\{[^}]*grid-template-columns:\s*repeat\(3,\s*minmax\(0,\s*1fr\)\)\s*!important;",
                RegexOptions.Singleline),
            cssContent);
        Assert.DoesNotMatch(
            new Regex(@"\.session-total-card\s*\{[^}]*grid-column:\s*1\s*/\s*-1", RegexOptions.Singleline),
            cssContent);
        Assert.Matches(
            new Regex(@"\.session-total-card\s*\{[^}]*grid-column:\s*auto\s*!important;", RegexOptions.Singleline),
            cssContent);
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
