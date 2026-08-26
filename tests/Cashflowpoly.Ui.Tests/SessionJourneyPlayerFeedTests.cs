// Fungsi file: Memverifikasi perilaku, lokalisasi, atau tata letak UI melalui SessionJourneyPlayerFeedTests.
using Xunit;

namespace Cashflowpoly.Ui.Tests;

public sealed class SessionJourneyPlayerFeedTests
{
    [Fact]
    public void SessionJourneyPartial_ShouldUseReadableMixedActorFeedLayout()
    {
        var repoRoot = ResolveRepositoryRoot();
        var viewPath = Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "Views", "Sessions", "_SessionJourneySection.cshtml");
        var viewContent = File.ReadAllText(viewPath);

        Assert.Contains("session-journey-filter", viewContent);
        Assert.Contains("session-journey-feed", viewContent);
        Assert.Contains("sessions.timeline_mixed_subtitle", viewContent);
        Assert.Contains("sessions.timeline_filter_label", viewContent);
        Assert.DoesNotContain("<span class=\"hint-chip\">@Context.T(\"sessions.timeline_filter_label\")</span>", viewContent, StringComparison.Ordinal);
        Assert.Contains("sessions.timeline_filter_all", viewContent);
        Assert.Contains("sessions.timeline_filter_players", viewContent);
        Assert.Contains("sessions.timeline_filter_system", viewContent);
        Assert.Contains("sessions.timeline_active_players", viewContent);
        Assert.DoesNotContain("session-board-track", viewContent);
        Assert.DoesNotContain("session-journey-legend", viewContent);
    }

    [Fact]
    public void SessionDetails_ShouldStackJourneyInsightsOnMobile()
    {
        var repoRoot = ResolveRepositoryRoot();
        var viewPath = Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "Views", "Sessions", "Details.cshtml");
        var viewContent = File.ReadAllText(viewPath);

        Assert.Contains("grid-template-columns: minmax(0, 1fr);", viewContent);
        Assert.DoesNotContain("grid-template-columns: repeat(2, minmax(0, 1fr));", viewContent);
        Assert.DoesNotContain(".session-journey-insight-card:last-child", viewContent);
    }

    [Fact]
    public void SessionJourneyScript_ShouldRenderPlayerAndImportantSystemEvents()
    {
        var repoRoot = ResolveRepositoryRoot();
        var scriptPath = Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "Views", "Sessions", "_SessionJourneyScript.cshtml");
        var scriptContent = File.ReadAllText(scriptPath);

        Assert.Contains("session-journey-feed", scriptContent);
        Assert.Contains("selectedTimelineFilter", scriptContent);
        Assert.Contains("isImportantSystemEvent", scriptContent);
        Assert.Contains("umumkanjuaradonasi", scriptContent);
        Assert.Contains("actorBucket(item.actorType) === \"SYSTEM\"", scriptContent);
        Assert.Contains("item.actionType !== \"AkhirGiliran\"", scriptContent);
        Assert.Contains("session-journey-feed-item-system", scriptContent);
        Assert.DoesNotContain("renderJourneyBoard", scriptContent);
        Assert.DoesNotContain("session-board-node", scriptContent);
    }

    [Fact]
    public void SessionJourneyScript_ShouldNotRenderSequenceOrdinal()
    {
        var repoRoot = ResolveRepositoryRoot();
        var scriptPath = Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "Views", "Sessions", "_SessionJourneyScript.cshtml");
        var scriptContent = File.ReadAllText(scriptPath);

        Assert.Contains("feedTimeline.map((item) =>", scriptContent);
        Assert.DoesNotContain("displaySequenceNumber", scriptContent);
        Assert.DoesNotContain("seqLabel", scriptContent);
        Assert.DoesNotContain("session-journey-feed-seq", scriptContent);
    }

    [Fact]
    public void SessionJourneyScript_ShouldRenderDayZeroAsGoWithoutIncrementing()
    {
        var repoRoot = ResolveRepositoryRoot();
        var scriptPath = Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "Views", "Sessions", "_SessionJourneyScript.cshtml");
        var scriptContent = File.ReadAllText(scriptPath);

        Assert.Contains("displayDayNumber", scriptContent);
        Assert.Contains("dayPositionLabel", scriptContent);
        Assert.Contains("dayNumber === 0 ? \"GO\"", scriptContent);
        Assert.DoesNotContain("dayIndex + 1", scriptContent);
        Assert.Contains("${dayPositionLabel(item.dayIndex)}", scriptContent);
        Assert.Contains("Math.max(0, toNumber(readValue(item, \"actionSlot\", \"ActionSlot\"), 0))", scriptContent);
    }

    [Fact]
    public void SessionJourneyScript_ShouldKeepEmptyBoardDaySelected()
    {
        var repoRoot = ResolveRepositoryRoot();
        var scriptPath = Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "Views", "Sessions", "_SessionJourneyScript.cshtml");
        var scriptContent = File.ReadAllText(scriptPath);

        Assert.Contains("const selectedDayIsAvailable = selectedTimelineFilter === \"all\"", scriptContent);
        Assert.Contains("selectedDay >= 0 && selectedDay <= finishMarkerDay", scriptContent);
        Assert.Contains("dayEventCountEl.textContent = dayFeedTimeline.length", scriptContent);
    }

    [Fact]
    public void SessionJourneyControls_ShouldExposeSelectionAndDisabledStates()
    {
        var repoRoot = ResolveRepositoryRoot();
        var sectionPath = Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "Views", "Sessions", "_SessionJourneySection.cshtml");
        var scriptPath = Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "Views", "Sessions", "_SessionJourneyScript.cshtml");
        var sectionContent = File.ReadAllText(sectionPath);
        var scriptContent = File.ReadAllText(scriptPath);

        Assert.Contains("aria-pressed=\"true\"", sectionContent);
        Assert.Contains("aria-pressed=\"${selectedTimelineFilter === filterKey ? \"true\" : \"false\"}\"", scriptContent);
        Assert.Contains("tabindex=\"${hasEvents ? \"0\" : \"-1\"}\"", scriptContent);
        Assert.Contains("aria-disabled=\"${hasEvents ? \"false\" : \"true\"}\"", scriptContent);
        Assert.Contains("aria-label=\"${escapeHtml(`${dayLabel} ${d}`)}\"", scriptContent);
        Assert.Contains("aria-label=\"${escapeHtml(finishLabel)}\"", scriptContent);
        Assert.DoesNotContain("aria-label=\"Day ${d}\"", scriptContent);
        Assert.DoesNotContain("aria-label=\"Finish\"", scriptContent);
    }

    [Fact]
    public void SessionJourneyScript_ShouldRequestOnlyEventsAfterTheLastSequence()
    {
        var repoRoot = ResolveRepositoryRoot();
        var scriptPath = Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "Views", "Sessions", "_SessionJourneyScript.cshtml");
        var scriptContent = File.ReadAllText(scriptPath);

        Assert.Contains("let timelineCursor = null;", scriptContent);
        Assert.Contains("encodeURIComponent(timelineCursor)", scriptContent);
        Assert.DoesNotContain("encodeURIComponent(lastSequence)", scriptContent);
        Assert.Contains("response.redirected && response.url.includes(\"/auth/login\")", scriptContent);
    }

    [Fact]
    public void SessionJourneyScript_ShouldUseMappedActionSlotLabelsForPlayerEvents()
    {
        var repoRoot = ResolveRepositoryRoot();
        var scriptPath = Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "Views", "Sessions", "_SessionJourneyScript.cshtml");
        var scriptContent = File.ReadAllText(scriptPath);

        Assert.Contains("actionSlotLabelText", scriptContent);
        Assert.Contains("item.actionSlotLabel", scriptContent);
        Assert.Contains("readValue(item, \"actionSlotLabel\", \"ActionSlotLabel\")", scriptContent);
        Assert.DoesNotContain("? `${dayLabel} ${displayDayNumber(item.dayIndex)} | ${actionSlotLabel} ${item.actionSlot}`", scriptContent);
        Assert.DoesNotContain("? `${dayLabel} ${displayDayNumber(latestItem.dayIndex)} | ${actionSlotLabel} ${latestItem.actionSlot}`", scriptContent);
    }

    [Fact]
    public void SessionJourneyScript_ShouldNestRelatedPlayerEventsInsidePrimaryActionCards()
    {
        var repoRoot = ResolveRepositoryRoot();
        var scriptPath = Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "Views", "Sessions", "_SessionJourneyScript.cshtml");
        var scriptContent = File.ReadAllText(scriptPath);

        Assert.Contains("isPrimaryTimelineItem", scriptContent);
        Assert.Contains("buildFeedTimeline", scriptContent);
        Assert.Contains("relatedEvents", scriptContent);
        Assert.Contains("pendingRelatedEvents", scriptContent);
        Assert.Contains("renderRelatedEvents", scriptContent);
        Assert.Contains("const feedTimeline = buildFeedTimeline(visibleTimeline);", scriptContent);
        Assert.Contains("renderJourneyInsights(feedTimeline);", scriptContent);
        Assert.Contains("renderJourneyFeed(baseTimeline, feedTimeline);", scriptContent);
        Assert.DoesNotContain("renderJourneyFeed(baseTimeline, visibleTimeline);", scriptContent);
    }

    [Fact]
    public void SessionJourneyLexicon_ShouldUseActivityTerminologyForLatestTimelineCard()
    {
        var repoRoot = ResolveRepositoryRoot();
        var lexiconPath = Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "Infrastructure", "UiTextLexicon.Sessions.cs");
        var lexiconContent = File.ReadAllText(lexiconPath);

        Assert.Contains("Hari dan Aktivitas Terakhir", lexiconContent);
        Assert.Contains("Aksi", lexiconContent);
        Assert.Contains("slot aksi", lexiconContent);
        Assert.DoesNotContain("Hari dan Aksi Terakhir", lexiconContent);
        Assert.DoesNotContain("Hari dan Giliran Terakhir", lexiconContent);
        Assert.DoesNotContain("\"Giliran\"", lexiconContent);
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
