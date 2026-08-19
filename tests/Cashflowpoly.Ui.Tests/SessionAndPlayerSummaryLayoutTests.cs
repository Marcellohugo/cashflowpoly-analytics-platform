// Fungsi file: Memverifikasi perilaku, lokalisasi, atau tata letak UI melalui SessionAndPlayerSummaryLayoutTests.
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
    public void SessionDetails_PlayerScoresShouldUsePlayersAsColumns()
    {
        var sessionDetailView = File.ReadAllText(Path.Combine(UiRoot, "Views", "Sessions", "Details.cshtml"));
        var css = File.ReadAllText(Path.Combine(UiRoot, "wwwroot", "css", "site.css"));

        Assert.Contains("happiness-score-table", sessionDetailView, StringComparison.Ordinal);
        Assert.Contains("happinessScoreRows", sessionDetailView, StringComparison.Ordinal);
        Assert.Contains("item.NeedPointsTotal", sessionDetailView, StringComparison.Ordinal);
        Assert.Contains("-item.MissionPenaltyTotal", sessionDetailView, StringComparison.Ordinal);
        Assert.Contains("-item.LoanPenaltyTotal", sessionDetailView, StringComparison.Ordinal);
        Assert.Contains("item.HappinessPointsTotal", sessionDetailView, StringComparison.Ordinal);
        Assert.Contains("@Context.T(\"sessions.view_analytics\")", sessionDetailView, StringComparison.Ordinal);
        Assert.Contains("value == 0d ? \"0\"", sessionDetailView, StringComparison.Ordinal);
        Assert.Contains("happiness-score-label-column", sessionDetailView, StringComparison.Ordinal);
        Assert.Contains("happiness-score-player-column", sessionDetailView, StringComparison.Ordinal);
        Assert.Contains(".happiness-score-label-column {", css, StringComparison.Ordinal);
        Assert.Contains("width: 12rem;", css, StringComparison.Ordinal);
        Assert.DoesNotContain(".happiness-score-label {" + Environment.NewLine + "    position: sticky;", css, StringComparison.Ordinal);
        Assert.Contains("happiness-score-desktop", sessionDetailView, StringComparison.Ordinal);
        Assert.Contains("happiness-score-mobile-card", sessionDetailView, StringComparison.Ordinal);
        Assert.Contains("scoreRow.Values[playerIndex]", sessionDetailView, StringComparison.Ordinal);
        Assert.Contains(".happiness-score-desktop {", css, StringComparison.Ordinal);
        Assert.Contains(".happiness-score-mobile-row", css, StringComparison.Ordinal);
        Assert.Contains("@media (max-width: 900px)", css, StringComparison.Ordinal);
        Assert.Contains("grid-template-columns: repeat(2, minmax(0, 1fr));", css, StringComparison.Ordinal);
    }

    [Fact]
    public void SessionDetails_ActiveRulesetCard_ShouldUseTwoColumnLayout()
    {
        var sessionDetailView = File.ReadAllText(Path.Combine(UiRoot, "Views", "Sessions", "Details.cshtml"));
        var css = File.ReadAllText(Path.Combine(UiRoot, "wwwroot", "css", "site.css"));

        Assert.Contains("session-ruleset-card", sessionDetailView, StringComparison.Ordinal);
        Assert.Contains("session-ruleset-content", sessionDetailView, StringComparison.Ordinal);
        Assert.Contains("session-ruleset-action", sessionDetailView, StringComparison.Ordinal);
        Assert.Contains(".session-ruleset-card {", css, StringComparison.Ordinal);
        Assert.Contains("justify-content: space-between;", css, StringComparison.Ordinal);
        Assert.Contains(".session-ruleset-action {", css, StringComparison.Ordinal);
    }

    [Fact]
    public void SessionDetails_ActiveRulesetCard_ShouldOpenModalPopupWithDarkBackdropAndCloseButton()
    {
        var sessionDetailView = File.ReadAllText(Path.Combine(UiRoot, "Views", "Sessions", "Details.cshtml"));
        var sharedRulesetDetail = File.ReadAllText(Path.Combine(UiRoot, "Views", "Shared", "_RulesetDetailContent.cshtml"));
        var css = File.ReadAllText(Path.Combine(UiRoot, "wwwroot", "css", "site.css"));

        Assert.Contains("id=\"openRulesetModalBtn\"", sessionDetailView, StringComparison.Ordinal);
        Assert.Contains("id=\"rulesetModal\"", sessionDetailView, StringComparison.Ordinal);
        Assert.Contains("class=\"ruleset-modal-backdrop ruleset-modal--@modalModeCss\"", sessionDetailView, StringComparison.Ordinal);
        Assert.Contains("class=\"ruleset-modal-overlay\"", sessionDetailView, StringComparison.Ordinal);
        Assert.Contains("id=\"closeRulesetModalBtn\"", sessionDetailView, StringComparison.Ordinal);
        Assert.Contains("class=\"ruleset-modal-close\"", sessionDetailView, StringComparison.Ordinal);
        Assert.Contains(".ruleset-modal-backdrop", css, StringComparison.Ordinal);
        Assert.Contains(".ruleset-modal-overlay", css, StringComparison.Ordinal);
        Assert.Contains(".ruleset-modal-close", css, StringComparison.Ordinal);
        Assert.Contains(".ruleset-modal-header .font-mono", css, StringComparison.Ordinal);
        Assert.Contains("overflow-wrap: anywhere;", css, StringComparison.Ordinal);
        Assert.Contains(".ruleset-modal-overlay,", css, StringComparison.Ordinal);
        Assert.Contains("document.body.appendChild(modal)", sessionDetailView, StringComparison.Ordinal);
        Assert.Contains("if (e.key === 'Tab')", sessionDetailView, StringComparison.Ordinal);
        Assert.Contains("_RulesetDetailContent", sessionDetailView, StringComparison.Ordinal);
        Assert.Contains("rulesets.config_unavailable", sharedRulesetDetail, StringComparison.Ordinal);
        Assert.DoesNotContain("?? 20", sessionDetailView, StringComparison.Ordinal);
        Assert.Contains("ruleset-modal--@modalModeCss", sessionDetailView, StringComparison.Ordinal);
        Assert.Contains("data-ruleset-mode=\"@modalModeCss\"", sessionDetailView, StringComparison.Ordinal);
        Assert.Contains("ruleset-mode-badge--@modeCss", sharedRulesetDetail, StringComparison.Ordinal);
        Assert.Contains("rulesetDetail?.Mode ?? rulesetDetail?.Definition?.Mode", sessionDetailView, StringComparison.Ordinal);
        Assert.Contains("rulesets.default_description_advanced", sharedRulesetDetail, StringComparison.Ordinal);
        Assert.Contains("rulesets.default_description_beginner", sharedRulesetDetail, StringComparison.Ordinal);
        Assert.DoesNotContain("@modalRulesetId", sessionDetailView, StringComparison.Ordinal);
        Assert.DoesNotContain("@activeRulesetId", sessionDetailView, StringComparison.Ordinal);
        Assert.DoesNotContain("ruleset-modal-stat-card--mode", sessionDetailView, StringComparison.Ordinal);
        Assert.Contains(".ruleset-modal--beginner", css, StringComparison.Ordinal);
        Assert.Contains(".ruleset-modal--advanced", css, StringComparison.Ordinal);
    }

    [Fact]
    public void RulesetDetailsPageAndSessionPopup_ShouldUseTheSameCompleteContentPartial()
    {
        var rulesetDetailView = File.ReadAllText(Path.Combine(UiRoot, "Views", "Rulesets", "Details.cshtml"));
        var sessionDetailView = File.ReadAllText(Path.Combine(UiRoot, "Views", "Sessions", "Details.cshtml"));
        var sharedContent = File.ReadAllText(Path.Combine(UiRoot, "Views", "Shared", "_RulesetDetailContent.cshtml"));

        Assert.Contains("_RulesetDetailContent", rulesetDetailView, StringComparison.Ordinal);
        Assert.Contains("_RulesetDetailContent", sessionDetailView, StringComparison.Ordinal);
        Assert.Contains("rulesets.version_history", sharedContent, StringComparison.Ordinal);
        Assert.Contains("rulesets.config_summary", sharedContent, StringComparison.Ordinal);
        Assert.Contains("rulesets.form.core_setup", sharedContent, StringComparison.Ordinal);
        Assert.Contains("rulesets.form.weekday_features", sharedContent, StringComparison.Ordinal);
        Assert.Contains("rulesets.form.constraints", sharedContent, StringComparison.Ordinal);
        Assert.Contains("rulesets.form.economy_and_donation", sharedContent, StringComparison.Ordinal);
    }

    [Fact]
    public void PlayerDirectory_ShouldBeVisibleButOnlyOwnDetailCanBeOpened()
    {
        var layout = File.ReadAllText(Path.Combine(UiRoot, "Views", "Shared", "_Layout.cshtml"));
        var directoryController = File.ReadAllText(Path.Combine(UiRoot, "Controllers", "PlayerDirectoryController.cs"));
        var playersController = File.ReadAllText(Path.Combine(UiRoot, "Controllers", "PlayersController.cs"));
        var directoryView = File.ReadAllText(Path.Combine(UiRoot, "Views", "Players", "Index.cshtml"));

        Assert.Contains("data-nav=\"players\"", layout, StringComparison.Ordinal);
        Assert.DoesNotContain("return RedirectToAction(\"Index\", \"Sessions\")", directoryController, StringComparison.Ordinal);
        Assert.Contains("currentUserId != playerId", playersController, StringComparison.Ordinal);
        Assert.Contains("StatusCodes.Status403Forbidden", playersController, StringComparison.Ordinal);
        Assert.Contains("api/v1/sessions/{session.SessionId}/players", directoryController, StringComparison.Ordinal);
        Assert.Contains("foreach (var session in Model.SessionGroups)", directoryView, StringComparison.Ordinal);
        Assert.Contains("isInstructor || (hasCurrentPlayerId && player.PlayerId == currentPlayerId)", directoryView, StringComparison.Ordinal);
        Assert.Contains("players.index.detail_unavailable", directoryView, StringComparison.Ordinal);
    }

    [Fact]
    public void VisibleLexicon_ShouldAvoidIntegrationTermsAndUsePlayerAnalyticsWording()
    {
        var infrastructurePath = Path.Combine(UiRoot, "Infrastructure");
        var lexicon = string.Join(
            Environment.NewLine,
            Directory.GetFiles(infrastructurePath, "UiTextLexicon*.cs").Select(File.ReadAllText));

        Assert.DoesNotContain("IDN", lexicon, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(" API", lexicon, StringComparison.Ordinal);
        Assert.Contains("Analitika Pemain pada Sesi", lexicon, StringComparison.Ordinal);
        Assert.Contains("Visual papan mengikuti urutan aktivitas permainan terbaru secara otomatis.", lexicon, StringComparison.Ordinal);
    }

    [Fact]
    public void SessionDetails_PlayerSummaryShouldUseOnlyThePlayerAnalyticsRow()
    {
        var view = File.ReadAllText(Path.Combine(UiRoot, "Views", "Sessions", "Details.cshtml"));

        Assert.Contains("currentPlayerSummary", view, StringComparison.Ordinal);
        Assert.Contains("sessions.detail.player_summary_title", view, StringComparison.Ordinal);
        Assert.Contains("sessions.detail.player_scores_title", view, StringComparison.Ordinal);
        Assert.Contains("currentPlayerSummary.CashInTotal", view, StringComparison.Ordinal);
        Assert.Contains("currentPlayerSummary.HappinessPointsTotal", view, StringComparison.Ordinal);
        Assert.Contains("currentPlayerSummary.FulfillmentDiversity", view, StringComparison.Ordinal);
        Assert.DoesNotContain("font-mono\">@Model.SessionId", view, StringComparison.Ordinal);

        var lexicon = File.ReadAllText(Path.Combine(UiRoot, "Infrastructure", "UiTextLexicon.Players.cs"));
        Assert.Contains("(\"Selisih Uang Masuk dan Keluar\", \"Income and Spending Difference\")", lexicon, StringComparison.Ordinal);
    }

    [Fact]
    public void SessionDetails_SummaryCardsShouldUseAvailableResponsiveGridsForBothRoles()
    {
        var view = File.ReadAllText(Path.Combine(UiRoot, "Views", "Sessions", "Details.cshtml"));

        Assert.Contains("class=\"mt-3 ruleset-stats\"", view, StringComparison.Ordinal);
        Assert.Contains("class=\"mt-3 session-stats-grid\"", view, StringComparison.Ordinal);
        Assert.DoesNotContain("@Context.T(\"metric.violations\")", view, StringComparison.Ordinal);
        Assert.DoesNotContain("RulesViolationsCount", view, StringComparison.Ordinal);
        Assert.DoesNotContain("md:grid-cols-5", view, StringComparison.Ordinal);
        Assert.DoesNotContain("sm:grid-cols-2 xl:grid-cols-3", view, StringComparison.Ordinal);
    }

    [Fact]
    public void PlayerIndexSummary_ShouldOnlyShowTotalPlayersCount()
    {
        var playerIndexView = File.ReadAllText(Path.Combine(UiRoot, "Views", "Players", "Index.cshtml"));
        var sessionIndexView = File.ReadAllText(Path.Combine(UiRoot, "Views", "Sessions", "Index.cshtml"));

        Assert.Contains("ruleset-section-head-row", playerIndexView, StringComparison.Ordinal);
        Assert.Contains("players.total_players", playerIndexView, StringComparison.Ordinal);
        Assert.Contains("@Model.Players.Count", playerIndexView, StringComparison.Ordinal);
        Assert.DoesNotContain("players-stats-grid", playerIndexView, StringComparison.Ordinal);
        Assert.DoesNotContain("players.index.players_with_data", playerIndexView, StringComparison.Ordinal);
        Assert.DoesNotContain("players.index.sessions_with_data", playerIndexView, StringComparison.Ordinal);
        Assert.DoesNotContain("ux-legend", playerIndexView, StringComparison.Ordinal);
        Assert.DoesNotContain("ux-legend", sessionIndexView, StringComparison.Ordinal);
        Assert.DoesNotContain("sessions.legend.title", playerIndexView, StringComparison.Ordinal);
        Assert.DoesNotContain("sessions.legend.title", sessionIndexView, StringComparison.Ordinal);
        Assert.DoesNotContain(".players-index-shell .ruleset-index-count-block", File.ReadAllText(Path.Combine(UiRoot, "wwwroot", "css", "site.css")), StringComparison.Ordinal);
    }

    [Fact]
    public void PlayerIndexGroupedTable_ShouldReplaceDonationAndPensionRanksWithPlayerRank()
    {
        var playerIndexView = File.ReadAllText(Path.Combine(UiRoot, "Views", "Players", "Index.cshtml"));
        var playerDirectoryController = File.ReadAllText(Path.Combine(UiRoot, "Controllers", "PlayerDirectoryController.cs"));

        Assert.Contains("common.rank", playerIndexView, StringComparison.Ordinal);
        Assert.Contains("player.FinalRank", playerIndexView, StringComparison.Ordinal);
        Assert.DoesNotContain("playerRankLookup", playerIndexView, StringComparison.Ordinal);
        Assert.Contains("x.analytics?.Leaderboard", playerDirectoryController, StringComparison.Ordinal);
        Assert.Contains("leaderboard?.HappinessPointsTotal", playerDirectoryController, StringComparison.Ordinal);
        Assert.Contains("winner-row", playerIndexView, StringComparison.Ordinal);
        Assert.DoesNotContain("players.rank", playerIndexView, StringComparison.Ordinal);
        Assert.DoesNotContain("players.winner_badge", playerIndexView, StringComparison.Ordinal);
        Assert.DoesNotContain("winner-chip", playerIndexView, StringComparison.Ordinal);
        Assert.DoesNotContain("players.champion.donation_short", playerIndexView, StringComparison.Ordinal);
        Assert.DoesNotContain("players.champion.pension_short", playerIndexView, StringComparison.Ordinal);
        Assert.DoesNotContain("donationChampionLookup", playerIndexView, StringComparison.Ordinal);
        Assert.DoesNotContain("pensionChampionLookup", playerIndexView, StringComparison.Ordinal);
    }

    [Fact]
    public void PlayerIndexTable_ShouldNotRenderPlayerIdOrNetCashflowColumns()
    {
        var playerIndexView = File.ReadAllText(Path.Combine(UiRoot, "Views", "Players", "Index.cshtml"));
        var playerDirectoryController = File.ReadAllText(Path.Combine(UiRoot, "Controllers", "PlayerDirectoryController.cs"));

        Assert.DoesNotContain("common.player_id", playerIndexView, StringComparison.Ordinal);
        Assert.DoesNotContain("common.session_id", playerIndexView, StringComparison.Ordinal);
        Assert.DoesNotContain("data-label=\"@Context.T(\\\"common.player_id\\\")\"", playerIndexView, StringComparison.Ordinal);
        Assert.DoesNotContain("metric.net_cashflow", playerIndexView, StringComparison.Ordinal);
        Assert.DoesNotContain("data-label=\"@Context.T(\\\"metric.net_cashflow\\\")\"", playerIndexView, StringComparison.Ordinal);
        Assert.DoesNotContain("p.UserId.ToString()", playerDirectoryController, StringComparison.Ordinal);
    }

    [Fact]
    public void PlayerIndexGroupedTable_ShouldUseStableReadableColumns()
    {
        var playerIndexView = File.ReadAllText(Path.Combine(UiRoot, "Views", "Players", "Index.cshtml"));
        var css = File.ReadAllText(Path.Combine(UiRoot, "wwwroot", "css", "site.css"));

        Assert.Contains("mobile-card-table players-session-table", playerIndexView, StringComparison.Ordinal);
        Assert.Contains("players-session-col-order", playerIndexView, StringComparison.Ordinal);
        Assert.Contains("players-session-col-rank", playerIndexView, StringComparison.Ordinal);
        Assert.Contains("players-session-col-score", playerIndexView, StringComparison.Ordinal);
        Assert.Contains("players-session-col-action", playerIndexView, StringComparison.Ordinal);
        Assert.Contains("players.index.happiness_points", playerIndexView, StringComparison.Ordinal);
        Assert.Contains(".players-session-table {", css, StringComparison.Ordinal);
        Assert.Contains("table-layout: fixed !important;", css, StringComparison.Ordinal);
        Assert.Contains(".players-session-table .players-session-col-action", css, StringComparison.Ordinal);
        Assert.Contains("width: 9rem;", css, StringComparison.Ordinal);
        Assert.Contains(".table-wrap table.mobile-card-table.players-session-table", css, StringComparison.Ordinal);

        var tableHeader = playerIndexView[
            playerIndexView.IndexOf("<thead class=\"table-head\">", playerIndexView.IndexOf("players-session-table", StringComparison.Ordinal), StringComparison.Ordinal)..];
        tableHeader = tableHeader[..tableHeader.IndexOf("</thead>", StringComparison.Ordinal)];
        var expectedHeaders = new[]
        {
            "common.turn_order",
            "common.rank",
            "common.name",
            "players.index.happiness_points",
            "players.index.analytics_column"
        };
        var previousHeaderIndex = -1;
        foreach (var header in expectedHeaders)
        {
            var headerIndex = tableHeader.IndexOf(header, StringComparison.Ordinal);
            Assert.True(headerIndex > previousHeaderIndex, $"Kolom {header} harus muncul pada urutan tetap.");
            previousHeaderIndex = headerIndex;
        }
        Assert.DoesNotContain("@if (isInstructor)", tableHeader, StringComparison.Ordinal);
    }

    [Fact]
    public void PlayerIndex_ShouldRevealPlayerRowsOnlyAfterSessionEnds()
    {
        var playerIndexView = File.ReadAllText(Path.Combine(UiRoot, "Views", "Players", "Index.cshtml"));
        var directoryController = File.ReadAllText(Path.Combine(UiRoot, "Controllers", "PlayerDirectoryController.cs"));

        Assert.Contains("var isSessionEnded = string.Equals(session.Status, \"ENDED\"", playerIndexView, StringComparison.Ordinal);
        Assert.Contains("@if (!isSessionEnded)", playerIndexView, StringComparison.Ordinal);
        Assert.Contains("colspan=\"5\"", playerIndexView, StringComparison.Ordinal);
        Assert.Contains("players.index.results_pending", playerIndexView, StringComparison.Ordinal);
        Assert.Contains("!string.Equals(session.Status, \"ENDED\"", directoryController, StringComparison.Ordinal);
        Assert.True(
            directoryController.IndexOf("!string.Equals(session.Status, \"ENDED\"", StringComparison.Ordinal) <
            directoryController.IndexOf("api/v1/analytics/sessions/{session.SessionId}", StringComparison.Ordinal),
            "Analitika sesi aktif tidak boleh diminta sebelum pemeriksaan status selesai.");
    }

    [Fact]
    public void PlayerIndex_ShouldUseOneCalmPaletteWithGoldReservedForWinner()
    {
        var css = File.ReadAllText(Path.Combine(UiRoot, "wwwroot", "css", "site.css"));

        Assert.Contains("body:has(.players-index-shell)", css, StringComparison.Ordinal);
        Assert.Contains("--session-accent: #2f6973;", css, StringComparison.Ordinal);
        Assert.Contains(".players-session-rank.rank-1", css, StringComparison.Ordinal);
        Assert.DoesNotContain(".players-session-card:nth-child", css, StringComparison.Ordinal);
    }

    [Fact]
    public void SessionDetails_PlayerHeadersShouldUseTurnColorClasses()
    {
        var sessionDetailView = File.ReadAllText(Path.Combine(UiRoot, "Views", "Sessions", "Details.cshtml"));
        var css = File.ReadAllText(Path.Combine(UiRoot, "wwwroot", "css", "site.css"));

        Assert.Contains("happiness-score-player--turn-@turnOrder", sessionDetailView, StringComparison.Ordinal);
        Assert.Contains(".happiness-score-player--turn-1", css, StringComparison.Ordinal);
        Assert.Contains("background: #e7f1fb;", css, StringComparison.Ordinal);
        Assert.Contains(".happiness-score-player--turn-2", css, StringComparison.Ordinal);
        Assert.Contains("background: #fff4df;", css, StringComparison.Ordinal);
        Assert.Contains(".happiness-score-player--turn-3", css, StringComparison.Ordinal);
        Assert.Contains("background: #e8f7ec;", css, StringComparison.Ordinal);
        Assert.Contains(".happiness-score-player--turn-4", css, StringComparison.Ordinal);
        Assert.Contains("background: #f1edfb;", css, StringComparison.Ordinal);
        Assert.DoesNotContain(".happiness-score-player:nth-child", css, StringComparison.Ordinal);
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
