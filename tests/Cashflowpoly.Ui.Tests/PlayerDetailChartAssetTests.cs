// Fungsi file: Memverifikasi perilaku, lokalisasi, atau tata letak UI melalui PlayerDetailChartAssetTests.
using Xunit;

namespace Cashflowpoly.Ui.Tests;

public sealed class PlayerDetailChartAssetTests
{
    private static readonly string RepoRoot = ResolveRepositoryRoot();

    [Fact]
    public void DetailsView_ShouldPreferExplainedValuesOverCharts()
    {
        var viewPath = Path.Combine(RepoRoot, "src", "Cashflowpoly.Ui", "Views", "Players", "Details.cshtml");
        var view = File.ReadAllText(viewPath);

        Assert.Contains("player-analysis-overview", view);
        Assert.Contains("player-analysis-card__description", view);
        Assert.Contains("player-analysis-card__method", view);
        Assert.DoesNotContain("js-metric-line-chart", view);
        Assert.DoesNotContain("window.cashflowpolyPlayerDetailCharts", view);
        Assert.DoesNotContain("~/js/player-detail-charts.js", view);
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

        throw new DirectoryNotFoundException("Tidak dapat menemukan root repositori (Cashflowpoly.sln).");
    }
}
