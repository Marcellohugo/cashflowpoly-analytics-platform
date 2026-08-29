// Fungsi file: Memverifikasi perilaku, lokalisasi, atau tata letak UI melalui PlayerDetailStatsLayoutTests.
using Xunit;

namespace Cashflowpoly.Ui.Tests;

public sealed class PlayerDetailStatsLayoutTests
{
    private static readonly string RepoRoot = ResolveRepositoryRoot();
    private static readonly string UiRoot = Path.Combine(RepoRoot, "src", "Cashflowpoly.Ui");

    [Fact]
    public void PlayerDetails_ShouldRenderLayeredPlayerAnalysis()
    {
        var view = File.ReadAllText(Path.Combine(UiRoot, "Views", "Players", "Details.cshtml"));

        Assert.Contains("player-analysis-overview", view, StringComparison.Ordinal);
        Assert.Contains("player-analysis-scorecard", view, StringComparison.Ordinal);
        Assert.Contains("player-analysis-atlas", view, StringComparison.Ordinal);
        Assert.Contains("player-analysis-chapters", view, StringComparison.Ordinal);
        Assert.Contains("player-analysis-card__takeaway", view, StringComparison.Ordinal);
        Assert.Contains("player-analysis-grid", view, StringComparison.Ordinal);
        Assert.Contains("player-evidence-library", view, StringComparison.Ordinal);
        Assert.Contains("player-stats-actions", view, StringComparison.Ordinal);
        Assert.DoesNotContain("player-stats-pillar-grid", view, StringComparison.Ordinal);
    }

    [Fact]
    public void PlayerSummary_ShouldAlwaysShowCollectionMissionInsteadOfActionEfficiency()
    {
        var view = File.ReadAllText(Path.Combine(UiRoot, "Views", "Players", "Details.cshtml"));

        Assert.Contains("player-analysis-scorecard__mission", view, StringComparison.Ordinal);
        Assert.Contains("statSummary.CollectionMissionComplete switch", view, StringComparison.Ordinal);
        Assert.DoesNotContain("actionEfficiencyMetric", view, StringComparison.Ordinal);
    }

    [Fact]
    public void PlayerDetails_ShouldNotRenderStandaloneTransactionHistory()
    {
        var view = File.ReadAllText(Path.Combine(UiRoot, "Views", "Players", "Details.cshtml"));
        var css = File.ReadAllText(Path.Combine(UiRoot, "wwwroot", "css", "site.css"));

        Assert.DoesNotContain("player-transaction-history", view, StringComparison.Ordinal);
        Assert.DoesNotContain("player-transactions-section", view, StringComparison.Ordinal);
        Assert.DoesNotContain("player-transactions-section", css, StringComparison.Ordinal);
        Assert.Contains("id=\"player-statistics-dashboard\"", view, StringComparison.Ordinal);
    }

    [Fact]
    public void PlayerDetails_ShouldRenderThreeMainStatisticSectionsAsAccordions()
    {
        var view = File.ReadAllText(Path.Combine(UiRoot, "Views", "Players", "Details.cshtml"));
        var css = File.ReadAllText(Path.Combine(UiRoot, "wwwroot", "css", "site.css"));

        Assert.Contains("<details id=\"player-statistics-summary\"", view, StringComparison.Ordinal);
        Assert.Contains("<details id=\"player-analysis-atlas\"", view, StringComparison.Ordinal);
        Assert.Contains("<details id=\"player-evidence-library\"", view, StringComparison.Ordinal);
        Assert.Equal(1, CountOccurrences(view, "open aria-labelledby=\"player-"));
        Assert.Contains("<summary class=\"players-fusion-head ruleset-section-head player-stats-hero\"", view, StringComparison.Ordinal);
        Assert.Equal(2, CountOccurrences(view, "<summary class=\"player-analysis-section-head\""));
        Assert.DoesNotContain("players.stats.read_time", view, StringComparison.Ordinal);
        Assert.Equal(3, CountOccurrences(view, " data-player-section-accordion data-accordion-key"));
        Assert.Contains("data-accordion-key=\"summary\"", view, StringComparison.Ordinal);
        Assert.Contains("data-accordion-key=\"analysis\"", view, StringComparison.Ordinal);
        Assert.Contains("data-accordion-key=\"evidence\"", view, StringComparison.Ordinal);
        Assert.Contains("cashflowpoly.player-section-accordions", view, StringComparison.Ordinal);
        Assert.Contains("localStorage.getItem(storageKey)", view, StringComparison.Ordinal);
        Assert.Contains("localStorage.setItem(storageKey", view, StringComparison.Ordinal);
        Assert.Contains(".player-detail-overhaul .player-dashboard-accordion>summary::after", css, StringComparison.Ordinal);
        Assert.Contains(".player-detail-overhaul .player-dashboard-accordion[open]>summary::after", css, StringComparison.Ordinal);
    }

    [Fact]
    public void PlayerDetails_ShouldUseProminentIdentityWithoutPlayerId()
    {
        var view = File.ReadAllText(Path.Combine(UiRoot, "Views", "Players", "Details.cshtml"));
        var css = File.ReadAllText(Path.Combine(UiRoot, "wwwroot", "css", "site.css"));

        Assert.Contains("player-profile-identity", view, StringComparison.Ordinal);
        Assert.Contains("player-profile-avatar", view, StringComparison.Ordinal);
        Assert.Contains("player-profile-turn", view, StringComparison.Ordinal);
        Assert.DoesNotContain("players.analytics_title", view, StringComparison.Ordinal);
        Assert.DoesNotContain("@Model.PlayerId", view, StringComparison.Ordinal);
        Assert.DoesNotContain("Model.GameplayComputedAt", view, StringComparison.Ordinal);
        Assert.Contains(".player-detail-overhaul .player-profile-avatar-shell", css, StringComparison.Ordinal);
    }

    [Fact]
    public void PlayerDetails_ShouldExposeSourceFormulaGuidanceAndRecommendation()
    {
        var view = File.ReadAllText(Path.Combine(UiRoot, "Views", "Players", "Details.cshtml"));
        var lexicon = File.ReadAllText(Path.Combine(UiRoot, "Infrastructure", "UiTextLexicon.Players.cs"));

        Assert.Contains("overallDescriptionKey", view, StringComparison.Ordinal);
        Assert.Contains("player-analysis-card__method", view, StringComparison.Ordinal);
        Assert.Contains("players.support.field.source", view, StringComparison.Ordinal);
        Assert.Contains("players.analysis.source.{analysis.Key}", view, StringComparison.Ordinal);
        Assert.Contains("analysis.FormulaKey", view, StringComparison.Ordinal);
        Assert.Contains("PlayerMetricCollectionHelper.BuildActualCalculation", view, StringComparison.Ordinal);
        Assert.Contains("players.support.field.actual_calculation", view, StringComparison.Ordinal);
        Assert.Contains("primaryMetric.Guidance", view, StringComparison.Ordinal);
        Assert.Contains("primaryMetric.Recommendation", view, StringComparison.Ordinal);
        Assert.True(
            view.IndexOf("players.support.field.source", StringComparison.Ordinal) < view.IndexOf("players.support.field.formula", StringComparison.Ordinal),
            "Data source must appear immediately before the calculation note.");
        Assert.True(
            view.IndexOf("players.support.field.formula", StringComparison.Ordinal) < view.IndexOf("players.support.field.actual_calculation", StringComparison.Ordinal) &&
            view.IndexOf("players.support.field.actual_calculation", StringComparison.Ordinal) < view.IndexOf("players.support.field.recommendation", StringComparison.Ordinal),
            "The numeric substitution must appear directly after the formula and before the recommendation.");
        Assert.Equal(14, CountOccurrences(lexicon, "terms[\"players.analysis.source."));
        Assert.Contains("(\"Sumber data\", \"Data source\")", lexicon, StringComparison.Ordinal);
        Assert.Contains("(\"Cara menghitung\", \"How it is calculated\")", lexicon, StringComparison.Ordinal);
        Assert.Contains("(\"Hitungan dengan angka pemain\", \"Calculation with the player's numbers\")", lexicon, StringComparison.Ordinal);
        Assert.Contains("Koin Tersisa dibanding Koin Awal = Koin Tersisa ÷ Koin Awal × 100%.", lexicon, StringComparison.Ordinal);
        Assert.Contains("jumlah kuadrat setiap (Persentase Pendapatan per Sumber ÷ 100)", lexicon, StringComparison.Ordinal);
        Assert.Contains("Persentase Risiko Selesai tanpa Tindakan Darurat = Risiko Selesai tanpa Tindakan Darurat ÷ Kartu Risiko Kehidupan yang Muncul × 100%.", lexicon, StringComparison.Ordinal);
        Assert.Contains("Skor Komitmen Donasi = Keteraturan Jumlah Donasi × (Porsi Donasi dari Koin Tersisa dan Donasi ÷ 100) × (Persentase Jumat dengan Donasi ÷ 100), dibatasi 0–100.", lexicon, StringComparison.Ordinal);
        Assert.Contains("Poin Kebahagiaan Kartu Kebutuhan + Poin Kebahagiaan Bonus Set Kebutuhan + Poin Kebahagiaan dari Donasi", lexicon, StringComparison.Ordinal);
        Assert.DoesNotContain("tingkat tanpa perlindungan", lexicon, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Konsentrasi Kebutuhan memakai rumus dokumen yang ditampilkan terpisah", lexicon, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void PlayerDetails_ShouldRenderCompleteRawAndDerivedMetricLibrary()
    {
        var view = File.ReadAllText(Path.Combine(UiRoot, "Views", "Players", "Details.cshtml"));
        var controller = File.ReadAllText(Path.Combine(UiRoot, "Controllers", "PlayersController.cs"));
        var model = File.ReadAllText(Path.Combine(UiRoot, "Models", "AnalyticsViewModels.cs"));

        Assert.Contains("GameplayRaw = gameplay?.RawJson", controller, StringComparison.Ordinal);
        Assert.Contains("GameplayDerived = gameplay?.DerivedJson", controller, StringComparison.Ordinal);
        Assert.Contains("JsonElement? GameplayRaw", model, StringComparison.Ordinal);
        Assert.Contains("JsonElement? GameplayDerived", model, StringComparison.Ordinal);
        Assert.Contains("BuildMetricVariableGroups(Model.GameplayRaw", view, StringComparison.Ordinal);
        Assert.Contains("BuildMetricGroups(Model.GameplayDerived", view, StringComparison.Ordinal);
        Assert.Contains("player-analysis-atlas", view, StringComparison.Ordinal);
        Assert.Contains("player-evidence-library", view, StringComparison.Ordinal);
        Assert.Contains("PlayerMetricLabelFormatter.DescribeMetric", view, StringComparison.Ordinal);
        Assert.Contains("players.support.field.recommendation", view, StringComparison.Ordinal);
        Assert.Contains("player-evidence-domain__recommendation", view, StringComparison.Ordinal);
        Assert.Contains("players.support.mode.advanced", view, StringComparison.Ordinal);
        Assert.True(
            view.IndexOf("player-analysis-atlas", StringComparison.Ordinal) < view.IndexOf("player-evidence-library", StringComparison.Ordinal),
            "Gameplay analysis results must appear before physical gameplay variables.");
    }

    [Fact]
    public void PlayerEvidenceLibrary_ShouldOnlyOpenTheFirstDomain()
    {
        var view = File.ReadAllText(Path.Combine(UiRoot, "Views", "Players", "Details.cshtml"));

        Assert.Contains("@(domainIndex == 0 ? \"open\" : null)", view, StringComparison.Ordinal);
        Assert.DoesNotContain("open=\"@(domainIndex == 0)\"", view, StringComparison.Ordinal);
    }

    [Fact]
    public void PlayerEvidenceLibrary_ShouldPresentElevenSourcesInTenNonRedundantGroups()
    {
        var view = File.ReadAllText(Path.Combine(UiRoot, "Views", "Players", "Details.cshtml"));

        foreach (var rawGroup in new[] { "coins", "ingredients", "meal_orders", "needs", "donations", "gold", "pension", "life_risk", "financial_goals", "actions", "turns" })
        {
            Assert.Contains($"GetGroupRows(rawGroupMap, \"{rawGroup}\")", view, StringComparison.Ordinal);
        }

        Assert.Contains("players.details.raw.actions.title", view, StringComparison.Ordinal);
        Assert.Equal(10, CountOccurrences(view, "TitleKey = \"players.details.raw."));
        Assert.DoesNotContain("players.details.raw.turns.title", view, StringComparison.Ordinal);
        Assert.DoesNotContain("domain.Number", view, StringComparison.Ordinal);
        Assert.DoesNotContain("player-evidence-domain__number", view, StringComparison.Ordinal);
        Assert.DoesNotContain("player-evidence-library__summary", view, StringComparison.Ordinal);
        Assert.DoesNotContain("rawMetricCount", view, StringComparison.Ordinal);
        Assert.DoesNotContain("derivedMetricCount", view, StringComparison.Ordinal);
        Assert.DoesNotContain("players.metric_items_suffix", view, StringComparison.Ordinal);
    }

    [Fact]
    public void PlayerEvidenceLibrary_ShouldExcludeRedundantAliasesOnly()
    {
        var view = File.ReadAllText(Path.Combine(UiRoot, "Views", "Players", "Details.cshtml"));
        var css = File.ReadAllText(Path.Combine(UiRoot, "wwwroot", "css", "site.css"));
        var lexicon = File.ReadAllText(Path.Combine(UiRoot, "Infrastructure", "UiTextLexicon.Players.cs"));

        Assert.Contains("BuildMetricVariableGroups", view, StringComparison.Ordinal);
        Assert.DoesNotContain("rawMetricKeysToShow", view, StringComparison.Ordinal);
        Assert.DoesNotContain("rows.RemoveAll", view, StringComparison.Ordinal);
        foreach (var redundantKey in new[]
        {
            "coins_net_end_game",
            "coins_donated",
            "coins_saved",
            "leftover_coins_end_game"
        })
        {
            Assert.Contains(redundantKey, view, StringComparison.Ordinal);
        }
        Assert.Contains("ExcludeRows", view, StringComparison.Ordinal);
        Assert.Contains("List<(string Path, string Value)> BuildActionRows()", view, StringComparison.Ordinal);
        Assert.Contains("ExcludeRows(rows, \"actions_per_turn\", \"action_sequence\", \"action_repetitions_per_turn\")", view, StringComparison.Ordinal);
        Assert.Contains("BuildActionUsageHistoryJson", view, StringComparison.Ordinal);
        Assert.Contains("isCollectionValue", view, StringComparison.Ordinal);
        Assert.Contains("player-metric-card--series", view, StringComparison.Ordinal);
        Assert.Contains("PlayerMetricJsonMapper.BuildCollectionTable", view, StringComparison.Ordinal);
        Assert.Contains("<table>", view, StringComparison.Ordinal);
        Assert.Contains("FormatActionSlot(actionSlot)", view, StringComparison.Ordinal);
        Assert.DoesNotContain("player-metric-series__outside-quota", view, StringComparison.Ordinal);
        Assert.DoesNotContain("players.details.action_slot.outside_quota", view, StringComparison.Ordinal);
        Assert.DoesNotContain("player-metric-series__outside-quota", css, StringComparison.Ordinal);
        Assert.DoesNotContain("players.details.action_slot.outside_quota", lexicon, StringComparison.Ordinal);
        Assert.DoesNotContain("var hasOutsideQuotaRows", view, StringComparison.Ordinal);
        Assert.Contains("var seriesRows = collectionTable.Rows.ToList()", view, StringComparison.Ordinal);
        Assert.Contains("collectionUnits", view, StringComparison.Ordinal);
        Assert.Contains("FormatCollectionCell(row.Path", view, StringComparison.Ordinal);
        Assert.DoesNotContain("FirstOrDefault(seriesRow", view, StringComparison.Ordinal);
        Assert.DoesNotContain("seriesRow[actionSlotColumnIndex] != \"0\"", view, StringComparison.Ordinal);
        Assert.DoesNotContain("zeroBasedDay + 1", view, StringComparison.Ordinal);
        Assert.DoesNotContain("font-family: ui-monospace", css, StringComparison.Ordinal);
        Assert.Contains("(\"Hari\", \"Day\")", lexicon, StringComparison.Ordinal);
        Assert.Contains("(\"Persiapan\", \"Setup\")", lexicon, StringComparison.Ordinal);
    }

    [Fact]
    public void PlayerEvidenceLibrary_ShouldMergeTransactionsChangesAndBalancesIntoOneHistory()
    {
        var view = File.ReadAllText(Path.Combine(UiRoot, "Views", "Players", "Details.cshtml"));
        var lexicon = File.ReadAllText(Path.Combine(UiRoot, "Infrastructure", "UiTextLexicon.Players.cs"));

        Assert.Contains("BuildTransactionHistoryJson", view, StringComparison.Ordinal);
        Assert.Contains("\"coins_spent_per_turn\",", view, StringComparison.Ordinal);
        Assert.Contains("\"coins_earned_per_turn\");", view, StringComparison.Ordinal);
        Assert.Contains("row.Path == \"net_income_per_turn\"", view, StringComparison.Ordinal);
        Assert.Contains("row.Path == \"coins_per_turn_progression\"", view, StringComparison.Ordinal);
        Assert.Contains("\"coins_per_turn_progression\",\n            \"net_income_per_turn\"", view, StringComparison.Ordinal);
        Assert.Contains("result.Add((\"transaction_history\", transactionHistory))", view, StringComparison.Ordinal);
        Assert.Contains("(\"Riwayat Transaksi\", \"Transaction History\")", lexicon, StringComparison.Ordinal);
        Assert.Contains("(\"Koin Masuk\", \"Coins In\")", lexicon, StringComparison.Ordinal);
        Assert.Contains("(\"Koin Keluar\", \"Coins Out\")", lexicon, StringComparison.Ordinal);
        Assert.Contains("(\"Perubahan Koin\", \"Coin Change\")", lexicon, StringComparison.Ordinal);
        Assert.Contains("(\"Saldo setelah Kejadian\", \"Balance after Event\")", lexicon, StringComparison.Ordinal);
        Assert.DoesNotContain("players.raw.transaction_direction", lexicon, StringComparison.Ordinal);
    }

    [Fact]
    public void PlayerEvidenceLibrary_ShouldGroupRepeatedGuidanceByDomain()
    {
        var view = File.ReadAllText(Path.Combine(UiRoot, "Views", "Players", "Details.cshtml"));

        Assert.Contains(".GroupBy(metric => (metric.Guidance, metric.Recommendation))", view, StringComparison.Ordinal);
        Assert.Contains("player-evidence-domain__guide", view, StringComparison.Ordinal);
        Assert.DoesNotContain("player-metric-card__guide", view, StringComparison.Ordinal);
    }

    [Fact]
    public void PlayerAnalysisMap_ShouldRenderThirteenMetricsAndTheirComponents()
    {
        var view = File.ReadAllText(Path.Combine(UiRoot, "Views", "Players", "Details.cshtml"));

        foreach (var componentGroup in new[]
        {
            "cash_growth_components",
            "income_diversification_components",
            "business_expense_share_components",
            "meal_order_profit_margin_components",
            "risk_readiness_components",
            "loan_burden_components",
            "financial_goal_progress_components",
            "income_action_focus_components",
            "ingredient_utilization_components",
            "long_term_action_share_components",
            "need_fulfillment_diversity_components",
            "donation_commitment_components",
            "happiness_points_composition"
        })
        {
            Assert.Contains(componentGroup, view, StringComparison.Ordinal);
        }

        Assert.Equal(13, CountOccurrences(view, "Chapter = \""));
        Assert.Equal(4, CountOccurrences(view, "Items = analysisSections.Where"));
        Assert.Contains("Key = \"happiness-portfolio\"", view, StringComparison.Ordinal);
        Assert.DoesNotContain("analysis.Number", view, StringComparison.Ordinal);
        Assert.DoesNotContain("chapter.Number", view, StringComparison.Ordinal);
        Assert.DoesNotContain("player-analysis-card__number", view, StringComparison.Ordinal);
        Assert.DoesNotContain("player-stat-action__number", view, StringComparison.Ordinal);
        Assert.Contains("player-stat-action__signal", view, StringComparison.Ordinal);
        Assert.Equal(13, CountOccurrences(view, "PrimaryKey = \""));
        Assert.Contains("PrimaryKey = \"cash_growth_percent\"", view, StringComparison.Ordinal);
        Assert.Contains("PrimaryKey = \"risk_readiness_percent\"", view, StringComparison.Ordinal);
        Assert.Contains("GetGroupRows(derivedGroupMap, \"risk_readiness_components\")", view, StringComparison.Ordinal);
        Assert.Contains("PrimaryKey = \"income_action_focus_percent\"", view, StringComparison.Ordinal);
        Assert.Contains("GetGroupRows(derivedGroupMap, \"income_action_focus_components\")", view, StringComparison.Ordinal);
        Assert.Contains("FormulaKey = \"players.support.formula.goal_ambition_index\"", view, StringComparison.Ordinal);
        Assert.Contains("PrimaryKey = \"long_term_action_share_percent\"", view, StringComparison.Ordinal);
        Assert.Contains("GetGroupRows(derivedGroupMap, \"long_term_action_share_components\")", view, StringComparison.Ordinal);
        Assert.Contains("PrimaryKey = \"donation_commitment_score\"", view, StringComparison.Ordinal);
        Assert.Contains("PrimaryKey = \"total_happiness_points\"", view, StringComparison.Ordinal);
        Assert.Contains("row.Path.Equals(analysis.PrimaryKey", view, StringComparison.Ordinal);
    }

    [Fact]
    public void PlayerDetails_ShouldHideAdvancedOnlyContentInBeginnerMode()
    {
        var view = File.ReadAllText(Path.Combine(UiRoot, "Views", "Players", "Details.cshtml"));

        Assert.Equal(2, CountOccurrences(view, ".Where(item => isAdvancedMode || !item.AdvancedOnly)"));
        Assert.Contains(".Where(chapter => chapter.Items.Count > 0)", view, StringComparison.Ordinal);
        Assert.Contains("analysis.AdvancedOnly && !chapter.AdvancedOnly", view, StringComparison.Ordinal);
        Assert.Contains("ExcludeAdvancedRowsInBeginner", view, StringComparison.Ordinal);
        Assert.Contains("players.analysis.happiness_portfolio.beginner.desc", view, StringComparison.Ordinal);
        Assert.Contains("players.support.formula.happiness_portfolio.beginner", view, StringComparison.Ordinal);
        Assert.Contains("players.analysis.source.happiness-portfolio.beginner", view, StringComparison.Ordinal);
        Assert.Contains("players.details.raw.coin_finance.beginner.desc", view, StringComparison.Ordinal);
    }

    [Fact]
    public void PlayerStats_ShouldUseScorecardAnalysisMapEvidenceAndPrioritizedActions()
    {
        var view = File.ReadAllText(Path.Combine(UiRoot, "Views", "Players", "Details.cshtml"));
        var css = File.ReadAllText(Path.Combine(UiRoot, "wwwroot", "css", "site.css"));

        Assert.Contains("player-analysis-overview", view, StringComparison.Ordinal);
        Assert.Contains("player-analysis-scorecard", view, StringComparison.Ordinal);
        Assert.Contains("player-analysis-atlas", view, StringComparison.Ordinal);
        Assert.Contains("player-evidence-library", view, StringComparison.Ordinal);
        Assert.Contains("statSummary.Insights.Take(3)", view, StringComparison.Ordinal);
        Assert.Contains(".player-detail-overhaul .player-analysis-overview {", css, StringComparison.Ordinal);
        Assert.Contains(".player-detail-overhaul .player-analysis-grid {", css, StringComparison.Ordinal);
        Assert.Contains(".player-detail-overhaul .player-evidence-domain>summary {", css, StringComparison.Ordinal);
        Assert.Contains(".player-detail-overhaul .player-stats-actions {", css, StringComparison.Ordinal);
        Assert.Contains(".player-detail-overhaul .player-metric-card-grid {", css, StringComparison.Ordinal);
        Assert.Contains("display: flex;", css, StringComparison.Ordinal);
        Assert.Contains("flex-wrap: wrap;", css, StringComparison.Ordinal);
        Assert.Contains("flex: 1 1 min(250px, 100%);", css, StringComparison.Ordinal);
        Assert.Contains(".player-detail-overhaul .player-metric-card-grid--columns-1", css, StringComparison.Ordinal);
        Assert.Contains(".player-detail-overhaul .player-metric-card-grid--columns-2", css, StringComparison.Ordinal);
        Assert.Contains(".player-detail-overhaul .player-metric-card-grid--columns-3", css, StringComparison.Ordinal);
        Assert.Contains("flex-basis: calc((100% - 1.16rem) / 3);", css, StringComparison.Ordinal);
        Assert.Contains("2 or 4 or 8 => 2", view, StringComparison.Ordinal);
        Assert.Contains("player-metric-card-grid--columns-{scalarGridColumns}", view, StringComparison.Ordinal);
        Assert.Contains("player-analysis-card__metrics--columns-@supportingGridColumns", view, StringComparison.Ordinal);
        Assert.Contains("align-items: stretch;", css, StringComparison.Ordinal);
        Assert.Contains("block-size: 100%;", css, StringComparison.Ordinal);
        Assert.Contains(".player-detail-overhaul .player-metric-card-grid>.player-metric-card--series {", css, StringComparison.Ordinal);
        Assert.Contains("flex: 1 0 100%;", css, StringComparison.Ordinal);
        Assert.Contains("inline-size: 100%;", css, StringComparison.Ordinal);
        Assert.Contains("PlayerMetricCollectionHelper.GetGroupRows(rawGroupMap, \"turns\")", view, StringComparison.Ordinal);
        Assert.Contains("\"coins_per_turn_progression\",", view, StringComparison.Ordinal);
        Assert.Contains("player-metric-card-grid--coin-finance", view, StringComparison.Ordinal);
        Assert.Contains("player-metric-card--coin-overview", view, StringComparison.Ordinal);
        Assert.Contains(".player-detail-overhaul .player-metric-card-grid--coin-finance>.player-metric-card--coin-overview {", css, StringComparison.Ordinal);
        Assert.Contains(".player-metric-card-grid--columns-3.player-metric-card-grid--coin-finance", css, StringComparison.Ordinal);
        Assert.Contains("flex: 1 1 calc((100% - 0.58rem) / 2);", css, StringComparison.Ordinal);
        Assert.Contains("\"meal-orders\" => domain.Rows.OrderBy", view, StringComparison.Ordinal);
        Assert.Contains("\"meal_orders_claimed\" => 0", view, StringComparison.Ordinal);
        Assert.Contains("\"meal_order_income_per_order\" => 1", view, StringComparison.Ordinal);
        Assert.Contains("\"meal_orders_per_turn_average\" => 3", view, StringComparison.Ordinal);
        Assert.Contains("player-metric-card-grid--meal-orders", view, StringComparison.Ordinal);
        Assert.Contains("player-metric-card--meal-orders-primary", view, StringComparison.Ordinal);
        Assert.Contains("player-metric-card--meal-orders-summary", view, StringComparison.Ordinal);
        Assert.Contains(".player-detail-overhaul .player-metric-card-grid--meal-orders>.player-metric-card--meal-orders-primary", css, StringComparison.Ordinal);
        Assert.Contains("\"donations\" => domain.Rows.OrderBy", view, StringComparison.Ordinal);
        Assert.Contains("PlayerMetricJsonMapper.BuildDonationHistoryJson(", view, StringComparison.Ordinal);
        Assert.Contains("ExcludeRows(rows, \"donation_amount_per_friday\", \"donation_rank_per_friday\")", view, StringComparison.Ordinal);
        Assert.Contains("\"donation_history\" => 0", view, StringComparison.Ordinal);
        Assert.Contains("\"donation_happiness_points\" => 3", view, StringComparison.Ordinal);
        Assert.Contains("player-metric-card-grid--donations", view, StringComparison.Ordinal);
        Assert.DoesNotContain(".player-detail-overhaul .player-metric-card-grid--donations>.player-metric-card--series {", css, StringComparison.Ordinal);
        Assert.Contains("\"life-risk\" => domain.Rows.OrderBy", view, StringComparison.Ordinal);
        Assert.Contains("\"life_risk_cards_drawn\" => 0", view, StringComparison.Ordinal);
        Assert.Contains("\"life_risk_costs_per_card\" => 1", view, StringComparison.Ordinal);
        Assert.Contains("\"life_risk_costs_total\" => 2", view, StringComparison.Ordinal);
        Assert.Contains("player-metric-card--featured", view, StringComparison.Ordinal);
        Assert.Contains(".player-detail-overhaul .player-metric-card-grid>.player-metric-card--featured {", css, StringComparison.Ordinal);
        Assert.Contains("\"gold\" => domain.Rows.OrderBy", view, StringComparison.Ordinal);
        Assert.Contains("\"gold_cards_initial\" => 0", view, StringComparison.Ordinal);
        Assert.Contains("\"gold_prices_per_purchase\" => 4", view, StringComparison.Ordinal);
        Assert.Contains("\"gold_investment_net\" => 8", view, StringComparison.Ordinal);
        Assert.Contains("player-metric-card-grid--gold", view, StringComparison.Ordinal);
        Assert.Contains("player-metric-card--gold-overview", view, StringComparison.Ordinal);
        Assert.Contains(".player-detail-overhaul .player-metric-card-grid--gold>.player-metric-card--gold-overview", css, StringComparison.Ordinal);
    }

    [Fact]
    public void PlayerStats_HeaderBadgeAndEvidenceRows_ShouldStayVisuallyAligned()
    {
        var css = File.ReadAllText(Path.Combine(UiRoot, "wwwroot", "css", "site.css"))
            .ReplaceLineEndings("\n");

        Assert.DoesNotContain(".player-detail-overhaul .player-stats-hero::after", css, StringComparison.Ordinal);
        Assert.Contains(".player-detail-overhaul .player-evidence-domain>summary {\n    display: flex;", css, StringComparison.Ordinal);
        Assert.Contains(".player-detail-overhaul .player-evidence-domain__copy {\n    display: grid;\n    flex: 1 1 auto;", css, StringComparison.Ordinal);
    }

    [Fact]
    public void PlayerStats_ShouldAnimateProgressivelyAndRespectReducedMotion()
    {
        var view = File.ReadAllText(Path.Combine(UiRoot, "Views", "Players", "Details.cshtml"));
        var css = File.ReadAllText(Path.Combine(UiRoot, "wwwroot", "css", "site.css"));
        var script = File.ReadAllText(Path.Combine(UiRoot, "wwwroot", "js", "site.js"));

        Assert.Contains("data-player-stats-reveal", view, StringComparison.Ordinal);
        Assert.Contains("player-stats-motion-ready", css, StringComparison.Ordinal);
        Assert.Contains("@keyframes player-stats-shine", css, StringComparison.Ordinal);
        Assert.Contains("@media (prefers-reduced-motion: reduce)", css, StringComparison.Ordinal);
        Assert.Contains("IntersectionObserver", script, StringComparison.Ordinal);
        Assert.Contains("prefers-reduced-motion: reduce", script, StringComparison.Ordinal);
    }

    [Fact]
    public void PlayerStats_ShouldUsePlainLanguageExplanations()
    {
        var view = File.ReadAllText(Path.Combine(UiRoot, "Views", "Players", "Details.cshtml"));
        var lexicon = File.ReadAllText(Path.Combine(UiRoot, "Infrastructure", "UiTextLexicon.Players.cs"));

        Assert.Contains("(\"Kesimpulan cepat\", \"Quick conclusion\")", lexicon, StringComparison.Ordinal);
        Assert.Contains("(\"Cerita di Balik Hasil Pemain\", \"The Story Behind the Player's Result\")", lexicon, StringComparison.Ordinal);
        Assert.Contains("(\"Penjelasan hasil\", \"Result explanation\")", lexicon, StringComparison.Ordinal);
        Assert.DoesNotContain("analysisSections.Count", view, StringComparison.Ordinal);
        Assert.DoesNotContain("players.analysis.metric_suffix", lexicon, StringComparison.Ordinal);
        Assert.Contains("(\"Uang dan usaha\", \"Money and business\")", lexicon, StringComparison.Ordinal);
        Assert.Contains("(\"Berapa koin tersisa dibandingkan koin awal?\", \"How do remaining coins compare with starting coins?\")", lexicon, StringComparison.Ordinal);
        Assert.Contains("(\"Persentase Pengeluaran untuk Membeli Bahan\", \"Ingredient Purchase Share of Spending\")", lexicon, StringComparison.Ordinal);
        Assert.DoesNotContain("apakah dua aksi per giliran digunakan secara beragam", lexicon, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("kecocokannya dengan misi koleksi pribadi", lexicon, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("(\"Prioritas pembahasan\", \"Discussion priorities\")", lexicon, StringComparison.Ordinal);
        Assert.Contains("(\"Kondisi uang\", \"Money condition\")", lexicon, StringComparison.Ordinal);
        Assert.Contains("(\"Pemerataan Kartu Kebutuhan\", \"Need Card Balance\")", lexicon, StringComparison.Ordinal);
        Assert.Contains("(\"Dari mana Poin Kebahagiaan berasal?\", \"Where did Happiness Points come from?\")", lexicon, StringComparison.Ordinal);
        Assert.Contains("(\"Data permainan\", \"Gameplay data\")", lexicon, StringComparison.Ordinal);
        Assert.Contains("(\"Saran\", \"Suggestion\")", lexicon, StringComparison.Ordinal);
        Assert.Contains("players.support.recommendation.savings", lexicon, StringComparison.Ordinal);
        Assert.Contains("players.support.recommendation.debt", lexicon, StringComparison.Ordinal);
        Assert.DoesNotContain("Lihat fungsi metrik", lexicon, StringComparison.Ordinal);
        Assert.DoesNotContain("Pustaka Metrik Pemain", lexicon, StringComparison.Ordinal);
        Assert.DoesNotContain("Data pendukung lengkap", lexicon, StringComparison.Ordinal);
    }

    [Fact]
    public void PlayerDetails_FinancialGoals_ShouldOnlyShowTheThreeEssentialValues()
    {
        var view = File.ReadAllText(Path.Combine(UiRoot, "Views", "Players", "Details.cshtml"));

        Assert.Contains("new[] { \"financial_goals_completed\", \"coins_saved\", \"sharia_loans_outstanding_coins\" }", view, StringComparison.Ordinal);
        Assert.Contains("Rows = financialGoalSummaryRows", view, StringComparison.Ordinal);
        Assert.Contains("Value ?? nullText", view, StringComparison.Ordinal);
        Assert.Contains("domainGuides.Count > 0 && domain.Key != \"financial-goals\"", view, StringComparison.Ordinal);
        Assert.Contains("TryParseMetricNumber(attemptedFinancialGoals, out var attemptedCount)", view, StringComparison.Ordinal);
        Assert.Contains("players.details.financial_goals.attempted_note", view, StringComparison.Ordinal);
        Assert.Contains(".Where(item => isAdvancedMode || !item.AdvancedOnly)", view, StringComparison.Ordinal);
    }

    [Fact]
    public void PlayerDetails_ShouldHideDerivedNeedProfilesFromTheEvidenceLibrary()
    {
        var view = File.ReadAllText(Path.Combine(UiRoot, "Views", "Players", "Details.cshtml"));
        Assert.Contains("ExcludeRows(PlayerMetricCollectionHelper.GetGroupRows(rawGroupMap, \"needs\"), \"need_profile.basic_profile\", \"need_profile.collector_profile\", \"need_profile.specialist_profile\")", view, StringComparison.Ordinal);
        Assert.Contains("Key = \"fulfillment-diversity\", PrimaryKey = \"need_fulfillment_diversity_percent\"", view, StringComparison.Ordinal);
    }

    [Fact]
    public void PlayerDetails_ShouldMergeDailyActionTables()
    {
        var view = File.ReadAllText(Path.Combine(UiRoot, "Views", "Players", "Details.cshtml"));

        Assert.Contains("BuildActionUsageHistoryJson", view, StringComparison.Ordinal);
        Assert.Contains("(\"action_usage_history\", history)", view, StringComparison.Ordinal);
        Assert.Contains("\"actions_per_turn\", \"action_sequence\", \"action_repetitions_per_turn\"", view, StringComparison.Ordinal);
    }

    private static int CountOccurrences(string source, string value) =>
        (source.Length - source.Replace(value, string.Empty, StringComparison.Ordinal).Length) / value.Length;

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
