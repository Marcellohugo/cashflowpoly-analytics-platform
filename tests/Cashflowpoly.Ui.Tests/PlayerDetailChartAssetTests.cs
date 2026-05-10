using Xunit;

namespace Cashflowpoly.Ui.Tests;

public sealed class PlayerDetailChartAssetTests
{
    private static readonly string RepoRoot = ResolveRepositoryRoot();

    [Fact]
    public void DetailsView_UsesExternalChartScriptWithLocalizedConfig()
    {
        var viewPath = Path.Combine(RepoRoot, "src", "Cashflowpoly.Ui", "Views", "Players", "Details.cshtml");
        var scriptPath = Path.Combine(RepoRoot, "src", "Cashflowpoly.Ui", "wwwroot", "js", "player-detail-charts.js");

        var view = File.ReadAllText(viewPath);

        Assert.Contains("window.cashflowpolyPlayerDetailCharts", view);
        Assert.Contains("~/js/player-detail-charts.js", view);
        Assert.DoesNotContain("const drawChart = (svg, payload)", view);
        Assert.True(File.Exists(scriptPath), "Asset player-detail-charts.js harus tersedia di wwwroot/js.");

        var script = File.ReadAllText(scriptPath);
        Assert.Contains("const drawChart = (svg, payload)", script);
        Assert.Contains("window.cashflowpolyPlayerDetailCharts", script);
        Assert.DoesNotContain("Context.T(", script);
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
