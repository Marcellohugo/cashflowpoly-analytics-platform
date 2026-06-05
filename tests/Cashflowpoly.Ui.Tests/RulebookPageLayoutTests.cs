using Xunit;

namespace Cashflowpoly.Ui.Tests;

public sealed class RulebookPageLayoutTests
{
    [Fact]
    public void ProgramAndLayout_ShouldExposeRulebookAsCanonicalRootRoute()
    {
        var repoRoot = ResolveRepositoryRoot();
        var programPath = Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "Program.cs");
        var controllerPath = Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "Controllers", "HomeController.cs");
        var layoutPath = Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "Views", "Shared", "_Layout.cshtml");
        var homePath = Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "Views", "Home", "Index.cshtml");

        var programContent = File.ReadAllText(programPath);
        var controllerContent = File.ReadAllText(controllerPath);
        var layoutContent = File.ReadAllText(layoutPath);
        var homeContent = File.ReadAllText(homePath);

        Assert.Contains("name: \"rulebook\"", programContent, StringComparison.Ordinal);
        Assert.Contains("pattern: \"rulebook\"", programContent, StringComparison.Ordinal);
        Assert.Contains("Redirect(\"/rulebook\")", controllerContent, StringComparison.Ordinal);
        Assert.Contains("href=\"/rulebook\"", layoutContent, StringComparison.Ordinal);
        Assert.Contains("href=\"/rulebook#rulebook-copyright\"", layoutContent, StringComparison.Ordinal);
        Assert.Contains("href=\"/rulebook\"", homeContent, StringComparison.Ordinal);
        Assert.DoesNotContain("asp-controller=\"Home\" asp-action=\"Rulebook\"", layoutContent, StringComparison.Ordinal);
        Assert.DoesNotContain("asp-controller=\"Home\" asp-action=\"Rulebook\"", homeContent, StringComparison.Ordinal);
    }

    [Fact]
    public void RulebookView_ShouldUseStructuredReadableLayout()
    {
        var repoRoot = ResolveRepositoryRoot();
        var viewPath = Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "Views", "Home", "Privacy.cshtml");
        var viewContent = File.ReadAllText(viewPath);

        Assert.DoesNotContain("<style>", viewContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("rulebook-page-grid", viewContent, StringComparison.Ordinal);
        Assert.DoesNotContain("rulebook-sidebar", viewContent, StringComparison.Ordinal);
        Assert.DoesNotContain("Daftar Isi Aturan", viewContent, StringComparison.Ordinal);
        Assert.DoesNotContain("rulebook-page-chip", viewContent, StringComparison.Ordinal);
        Assert.DoesNotContain("Urutan A-M mengikuti buku aturan", viewContent, StringComparison.Ordinal);
        Assert.Contains("rulebook-flow-map", viewContent, StringComparison.Ordinal);
        Assert.Contains("rulebook-guide-grid", viewContent, StringComparison.Ordinal);
        Assert.Contains("rulebook-visual-stack", viewContent, StringComparison.Ordinal);
        Assert.Contains("rulebook-reference-grid", viewContent, StringComparison.Ordinal);
        Assert.Contains("rulebook-score-grid", viewContent, StringComparison.Ordinal);
        Assert.Contains("rulebook-anchor-target", viewContent, StringComparison.Ordinal);
        Assert.Contains("Panduan ringkas untuk mulai main", viewContent, StringComparison.Ordinal);
        Assert.Contains("Referensi cepat saat bermain", viewContent, StringComparison.Ordinal);
        Assert.DoesNotContain("Gambar tetap besar", viewContent, StringComparison.Ordinal);
        Assert.DoesNotContain("Aturan mengacu PDF", viewContent, StringComparison.Ordinal);
        Assert.DoesNotContain("Metrik ke aturan", viewContent, StringComparison.Ordinal);
        Assert.DoesNotContain("Apa yang dilihat di analitika", viewContent, StringComparison.Ordinal);
        Assert.DoesNotContain("rulebook-reference-table", viewContent, StringComparison.Ordinal);
        Assert.DoesNotContain("rulebook-chapter-grid", viewContent, StringComparison.Ordinal);
        Assert.DoesNotContain("rulebook-section-block", viewContent, StringComparison.Ordinal);
        Assert.DoesNotContain("rulebook-scoring-table", viewContent, StringComparison.Ordinal);
        Assert.DoesNotContain("rulebook-mode-flow-grid", viewContent, StringComparison.Ordinal);
        Assert.DoesNotContain("@(index + 3).ToString", viewContent, StringComparison.Ordinal);
        Assert.DoesNotContain("@(sections.Count + 3).ToString", viewContent, StringComparison.Ordinal);
    }

    [Fact]
    public void SiteCss_ShouldGiveRulebookResponsiveWideScreenLayout()
    {
        var repoRoot = ResolveRepositoryRoot();
        var cssPath = Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "wwwroot", "css", "site.css");
        var cssContent = File.ReadAllText(cssPath);

        Assert.Contains(".rulebook-page-grid", cssContent, StringComparison.Ordinal);
        Assert.Contains("grid-template-columns: minmax(0, 1fr);", cssContent, StringComparison.Ordinal);
        Assert.Contains(".rulebook-flow-map", cssContent, StringComparison.Ordinal);
        Assert.Contains(".rulebook-guide-grid", cssContent, StringComparison.Ordinal);
        Assert.Contains(".rulebook-guide-card", cssContent, StringComparison.Ordinal);
        Assert.Contains(".rulebook-visual-stack", cssContent, StringComparison.Ordinal);
        Assert.Contains(".rulebook-visual-panel", cssContent, StringComparison.Ordinal);
        Assert.Contains(".rulebook-reference-grid", cssContent, StringComparison.Ordinal);
        Assert.Contains(".rulebook-reference-card", cssContent, StringComparison.Ordinal);
        Assert.Contains(".rulebook-score-grid", cssContent, StringComparison.Ordinal);
        Assert.Contains(".rulebook-anchor-target", cssContent, StringComparison.Ordinal);
        Assert.Contains(".rulebook-anchor-target:target", cssContent, StringComparison.Ordinal);
        Assert.Contains("list-style: disc;", cssContent, StringComparison.Ordinal);
        Assert.Contains("list-style-position: outside;", cssContent, StringComparison.Ordinal);
        Assert.Contains(".rulebook-list li::marker", cssContent, StringComparison.Ordinal);
        Assert.Contains("scroll-margin-top", cssContent, StringComparison.Ordinal);
        Assert.DoesNotContain("grid-template-columns: repeat(auto-fit, minmax(min(100%, 21rem), 1fr));", cssContent, StringComparison.Ordinal);
        Assert.DoesNotContain("grid-template-columns: repeat(3, minmax(22rem, 1fr));", cssContent, StringComparison.Ordinal);
        Assert.Contains("aspect-ratio: 16 / 10;", cssContent, StringComparison.Ordinal);
        Assert.Contains("object-fit: contain;", cssContent, StringComparison.Ordinal);
        Assert.DoesNotContain(".rulebook-page-chip", cssContent, StringComparison.Ordinal);
        Assert.DoesNotContain(".rulebook-sidebar", cssContent, StringComparison.Ordinal);
        Assert.DoesNotContain(".rulebook-outline-list", cssContent, StringComparison.Ordinal);
        Assert.DoesNotContain(".rulebook-reference-table", cssContent, StringComparison.Ordinal);
        Assert.DoesNotContain(".rulebook-scoring-table", cssContent, StringComparison.Ordinal);
        Assert.DoesNotContain(".rulebook-chapter-grid", cssContent, StringComparison.Ordinal);
        Assert.DoesNotContain(".rulebook-mode-flow-grid", cssContent, StringComparison.Ordinal);
        Assert.Contains("@media (min-width: 1600px)", cssContent, StringComparison.Ordinal);
        Assert.Contains("@media (max-width: 900px)", cssContent, StringComparison.Ordinal);
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
