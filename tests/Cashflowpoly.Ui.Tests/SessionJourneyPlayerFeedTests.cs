using System.Text.RegularExpressions;
using Xunit;

namespace Cashflowpoly.Ui.Tests;

public sealed class SessionJourneyPlayerFeedTests
{
    [Fact]
    public void SessionJourneyPartial_ShouldUsePlayerOnlyFilterableFeedLayout()
    {
        var repoRoot = ResolveRepositoryRoot();
        var viewPath = Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "Views", "Sessions", "_SessionJourneySection.cshtml");
        var viewContent = File.ReadAllText(viewPath);

        Assert.Contains("session-journey-filter", viewContent);
        Assert.Contains("session-journey-feed", viewContent);
        Assert.Contains("sessions.timeline_player_subtitle", viewContent);
        Assert.Contains("sessions.timeline_filter_label", viewContent);
        Assert.Contains("sessions.timeline_filter_all", viewContent);
        Assert.Contains("sessions.timeline_active_players", viewContent);
        Assert.DoesNotContain("session-board-track", viewContent);
        Assert.DoesNotContain("session-journey-legend", viewContent);
        Assert.DoesNotContain("sessions.actor.system", viewContent);
        Assert.DoesNotContain("sessions.actor.instructor", viewContent);
    }

    [Fact]
    public void SessionJourneyScript_ShouldFilterToPlayerEventsBeforeRenderingFeed()
    {
        var repoRoot = ResolveRepositoryRoot();
        var scriptPath = Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "Views", "Sessions", "_SessionJourneyScript.cshtml");
        var scriptContent = File.ReadAllText(scriptPath);

        Assert.Contains("session-journey-feed", scriptContent);
        Assert.Contains("selectedPlayerKey", scriptContent);
        Assert.Contains("isVisiblePlayerEvent", scriptContent);
        Assert.Contains("actorBucket(item.actorType) === \"PLAYER\"", scriptContent);
        Assert.Contains("item.actionType !== \"turn.action.used\"", scriptContent);
        Assert.DoesNotContain("renderJourneyBoard", scriptContent);
        Assert.DoesNotContain("session-board-node", scriptContent);
    }

    [Fact]
    public void SessionJourneyScript_ShouldRenderVisibleOrdinalInsteadOfRawSessionSequence()
    {
        var repoRoot = ResolveRepositoryRoot();
        var scriptPath = Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "Views", "Sessions", "_SessionJourneyScript.cshtml");
        var scriptContent = File.ReadAllText(scriptPath);

        Assert.Contains("displaySequenceNumber", scriptContent);
        Assert.Matches(
            new Regex(@"visibleTimeline\.map\(\(item,\s*index\)\s*=>", RegexOptions.Singleline),
            scriptContent);
        Assert.Contains("${displaySequenceNumber}", scriptContent);
    }

    [Fact]
    public void SessionJourneyScript_ShouldRenderOneBasedDayIndexWithoutIncrementing()
    {
        var repoRoot = ResolveRepositoryRoot();
        var scriptPath = Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "Views", "Sessions", "_SessionJourneyScript.cshtml");
        var scriptContent = File.ReadAllText(scriptPath);

        Assert.Contains("displayDayNumber", scriptContent);
        Assert.DoesNotContain("dayIndex + 1", scriptContent);
        Assert.Contains("${displayDayNumber(item.dayIndex)}", scriptContent);
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
