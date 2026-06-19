using Xunit;

namespace Cashflowpoly.Ui.Tests;

public sealed class PlayerDetailStatsLayoutTests
{
    private static readonly string RepoRoot = ResolveRepositoryRoot();
    private static readonly string UiRoot = Path.Combine(RepoRoot, "src", "Cashflowpoly.Ui");

    [Fact]
    public void PlayerDetails_ShouldRenderStatisticsAsTabbedInstructorDashboard()
    {
        var view = File.ReadAllText(Path.Combine(UiRoot, "Views", "Players", "Details.cshtml"));

        Assert.Contains("player-stats-tablist", view, StringComparison.Ordinal);
        Assert.Contains("data-player-stat-tab=\"summary\"", view, StringComparison.Ordinal);
        Assert.Contains("data-player-stat-tab=\"finance\"", view, StringComparison.Ordinal);
        Assert.Contains("data-player-stat-tab=\"behavior\"", view, StringComparison.Ordinal);
        Assert.Contains("data-player-stat-tab=\"happiness\"", view, StringComparison.Ordinal);
        Assert.Contains("data-player-stat-tab=\"technical\"", view, StringComparison.Ordinal);
        Assert.Contains("player-stat-technical-raw", view, StringComparison.Ordinal);
        Assert.Contains("player-stat-technical-derived", view, StringComparison.Ordinal);
    }

    [Fact]
    public void PlayerDetails_ShouldKeepRawAndDerivedChartsOutOfPrimarySections()
    {
        var view = File.ReadAllText(Path.Combine(UiRoot, "Views", "Players", "Details.cshtml"));

        Assert.DoesNotContain("id=\"physical-variables\"", view, StringComparison.Ordinal);
        Assert.DoesNotContain("id=\"derived-metrics\"", view, StringComparison.Ordinal);
        Assert.Contains("id=\"technical-drilldown\"", view, StringComparison.Ordinal);
    }

    [Fact]
    public void PlayerStatsRows_ShouldUseHorizontalScrollWhenCardsOverflow()
    {
        var css = File.ReadAllText(Path.Combine(UiRoot, "wwwroot", "css", "site.css"));

        Assert.Contains(".player-detail-overhaul .player-stats-key-grid,", css, StringComparison.Ordinal);
        Assert.Contains(".player-detail-overhaul .player-stats-mini-grid,", css, StringComparison.Ordinal);
        Assert.Contains(".player-detail-overhaul .player-stats-insights {", css, StringComparison.Ordinal);
        Assert.Contains("overflow-x: auto;", css, StringComparison.Ordinal);
        Assert.Contains("inline-size: 100%;", css, StringComparison.Ordinal);
        Assert.Contains("max-inline-size: 100%;", css, StringComparison.Ordinal);
        Assert.Contains("min-inline-size: 0;", css, StringComparison.Ordinal);
        Assert.Contains("scrollbar-gutter: stable both-edges;", css, StringComparison.Ordinal);
        Assert.Contains(".player-detail-overhaul .player-stats-panels {", css, StringComparison.Ordinal);
        Assert.Contains(".player-detail-overhaul .player-stats-tab-panel {", css, StringComparison.Ordinal);
        Assert.Contains(".player-detail-overhaul .player-stats-key-grid .player-stat-kpi", css, StringComparison.Ordinal);
        Assert.Contains(".player-detail-overhaul .player-stats-mini-grid .stat-card", css, StringComparison.Ordinal);
        Assert.Contains(".player-detail-overhaul .player-stats-insights .player-stat-insight", css, StringComparison.Ordinal);
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
