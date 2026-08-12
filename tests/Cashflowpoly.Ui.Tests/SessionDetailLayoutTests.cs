// Fungsi file: Memverifikasi perilaku, lokalisasi, atau tata letak UI melalui SessionDetailLayoutTests.
using Xunit;

namespace Cashflowpoly.Ui.Tests;

public sealed class SessionDetailLayoutTests
{
    [Fact]
    public void LayoutAndSessionDetailView_ShouldKeepPageStyleOutOfMainFlow()
    {
        var repoRoot = ResolveRepositoryRoot();
        var layoutPath = Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "Views", "Shared", "_Layout.cshtml");
        var sessionDetailPath = Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "Views", "Sessions", "Details.cshtml");

        var layoutContent = File.ReadAllText(layoutPath);
        var sessionDetailContent = File.ReadAllText(sessionDetailPath);

        Assert.Contains("""@await RenderSectionAsync("Head", required: false)""", layoutContent);
        Assert.Contains("@section Head", sessionDetailContent);
        Assert.DoesNotContain(Environment.NewLine + "<style>", sessionDetailContent, StringComparison.Ordinal);
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
