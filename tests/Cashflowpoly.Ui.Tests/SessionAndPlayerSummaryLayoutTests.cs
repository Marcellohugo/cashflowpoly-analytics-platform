using Xunit;

namespace Cashflowpoly.Ui.Tests;

public sealed class SessionAndPlayerSummaryLayoutTests
{
    private static readonly string RepoRoot = ResolveRepositoryRoot();
    private static readonly string UiRoot = Path.Combine(RepoRoot, "src", "Cashflowpoly.Ui");

    [Fact]
    public void SessionDetails_ShouldNotRenderCategoryChampionSection()
    {
        var sessionDetailView = File.ReadAllText(Path.Combine(UiRoot, "Views", "Sessions", "Details.cshtml"));

        Assert.DoesNotContain("sessions.champion.title", sessionDetailView, StringComparison.Ordinal);
        Assert.DoesNotContain("sessions.champion.donation", sessionDetailView, StringComparison.Ordinal);
        Assert.DoesNotContain("sessions.champion.pension", sessionDetailView, StringComparison.Ordinal);
        Assert.DoesNotContain("winner-card", sessionDetailView, StringComparison.Ordinal);
        Assert.DoesNotContain("donationChampions", sessionDetailView, StringComparison.Ordinal);
        Assert.DoesNotContain("pensionChampions", sessionDetailView, StringComparison.Ordinal);
    }

    [Fact]
    public void PlayerIndexSummary_ShouldOnlyShowTotalPlayersCount()
    {
        var playerIndexView = File.ReadAllText(Path.Combine(UiRoot, "Views", "Players", "Index.cshtml"));

        Assert.Contains("ruleset-section-head-row", playerIndexView, StringComparison.Ordinal);
        Assert.Contains("players.total_players", playerIndexView, StringComparison.Ordinal);
        Assert.Contains("@Model.Players.Count", playerIndexView, StringComparison.Ordinal);
        Assert.DoesNotContain("players-stats-grid", playerIndexView, StringComparison.Ordinal);
        Assert.DoesNotContain("players.index.players_with_data", playerIndexView, StringComparison.Ordinal);
        Assert.DoesNotContain("players.index.sessions_with_data", playerIndexView, StringComparison.Ordinal);
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
