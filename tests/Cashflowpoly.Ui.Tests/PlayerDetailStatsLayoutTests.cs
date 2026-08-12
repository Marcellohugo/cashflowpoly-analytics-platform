// Fungsi file: Memverifikasi perilaku, lokalisasi, atau tata letak UI melalui PlayerDetailStatsLayoutTests.
using Xunit;

namespace Cashflowpoly.Ui.Tests;

public sealed class PlayerDetailStatsLayoutTests
{
    private static readonly string RepoRoot = ResolveRepositoryRoot();
    private static readonly string UiRoot = Path.Combine(RepoRoot, "src", "Cashflowpoly.Ui");

    [Fact]
    public void PlayerDetails_ShouldRenderFocusedInstructorSummary()
    {
        var view = File.ReadAllText(Path.Combine(UiRoot, "Views", "Players", "Details.cshtml"));

        Assert.Contains("player-stats-verdict", view, StringComparison.Ordinal);
        Assert.Contains("player-stats-pillar-grid", view, StringComparison.Ordinal);
        Assert.Contains("player-stat-pillar--money", view, StringComparison.Ordinal);
        Assert.Contains("player-stat-pillar--behavior", view, StringComparison.Ordinal);
        Assert.Contains("player-stat-pillar--happiness", view, StringComparison.Ordinal);
        Assert.Contains("player-stats-actions", view, StringComparison.Ordinal);
        Assert.DoesNotContain("players.stats.happiness_breakdown", view, StringComparison.Ordinal);
        Assert.DoesNotContain("player-stat-score-breakdown", view, StringComparison.Ordinal);
        Assert.DoesNotContain("player-stats-technical", view, StringComparison.Ordinal);
        Assert.DoesNotContain("player-stats-tablist", view, StringComparison.Ordinal);
        Assert.DoesNotContain("data-player-stat-tab", view, StringComparison.Ordinal);
    }

    [Fact]
    public void PlayerDetails_ShouldRenderCashflowJourneyBeforeInstructorEvaluation()
    {
        var view = File.ReadAllText(Path.Combine(UiRoot, "Views", "Players", "Details.cshtml"));

        var cashflowJourneyIndex = view.IndexOf("id=\"player-transaction-history\"", StringComparison.Ordinal);
        var instructorEvaluationIndex = view.IndexOf("id=\"player-statistics-dashboard\"", StringComparison.Ordinal);

        Assert.NotEqual(-1, cashflowJourneyIndex);
        Assert.NotEqual(-1, instructorEvaluationIndex);
        Assert.True(cashflowJourneyIndex < instructorEvaluationIndex);
    }

    [Fact]
    public void PlayerDetails_ShouldRenderCashflowJourneyAsAccordion()
    {
        var view = File.ReadAllText(Path.Combine(UiRoot, "Views", "Players", "Details.cshtml"));
        var css = File.ReadAllText(Path.Combine(UiRoot, "wwwroot", "css", "site.css"));

        Assert.Contains("<details id=\"player-transaction-history\"", view, StringComparison.Ordinal);
        Assert.Contains("player-transactions-section data-toggle", view, StringComparison.Ordinal);
        Assert.DoesNotContain("player-transactions-section data-toggle\" open", view, StringComparison.Ordinal);
        Assert.Contains("<summary class=\"players-fusion-head ruleset-section-head\">", view, StringComparison.Ordinal);
        Assert.Contains(".player-transactions-section.data-toggle > summary > .ruleset-section-title::after", css, StringComparison.Ordinal);
    }

    [Fact]
    public void PlayerDetails_ShouldUseProminentIdentityWithoutPlayerId()
    {
        var view = File.ReadAllText(Path.Combine(UiRoot, "Views", "Players", "Details.cshtml"));
        var css = File.ReadAllText(Path.Combine(UiRoot, "wwwroot", "css", "site.css"));

        Assert.Contains("player-profile-identity", view, StringComparison.Ordinal);
        Assert.Contains("player-profile-avatar", view, StringComparison.Ordinal);
        Assert.Contains("player-profile-turn", view, StringComparison.Ordinal);
        Assert.DoesNotContain("@Model.PlayerId", view, StringComparison.Ordinal);
        Assert.Contains(".player-detail-overhaul .player-profile-avatar-shell", css, StringComparison.Ordinal);
    }

    [Fact]
    public void PlayerDetails_ShouldExplainWhatEveryMainNumberMeans()
    {
        var view = File.ReadAllText(Path.Combine(UiRoot, "Views", "Players", "Details.cshtml"));

        Assert.Contains("player-stat-pillar__summary", view, StringComparison.Ordinal);
        Assert.Contains("overallDescriptionKey", view, StringComparison.Ordinal);
        Assert.Contains("financeMeaningKey", view, StringComparison.Ordinal);
        Assert.Contains("behaviorMeaningKey", view, StringComparison.Ordinal);
        Assert.Contains("happinessMeaningKey", view, StringComparison.Ordinal);
        Assert.Equal(3, CountOccurrences(view, "player-stat-pillar__recommendation"));
        Assert.Contains("financeRecommendationKey", view, StringComparison.Ordinal);
        Assert.Contains("behaviorRecommendationKey", view, StringComparison.Ordinal);
        Assert.Contains("happinessRecommendationKey", view, StringComparison.Ordinal);
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
        Assert.Contains("BuildMetricGroups(Model.GameplayRaw", view, StringComparison.Ordinal);
        Assert.Contains("BuildMetricGroups(Model.GameplayDerived", view, StringComparison.Ordinal);
        Assert.Contains("player-metric-library", view, StringComparison.Ordinal);
        Assert.Contains("PlayerMetricLabelFormatter.DescribeMetric", view, StringComparison.Ordinal);
        Assert.Contains("players.support.field.recommendation", view, StringComparison.Ordinal);
        Assert.Contains("player-metric-card__recommendation", view, StringComparison.Ordinal);
        Assert.Contains("players.support.mode.advanced", view, StringComparison.Ordinal);
        Assert.DoesNotContain("players.stats.technical", view, StringComparison.Ordinal);
        Assert.DoesNotContain("players.stats.library.notes", view, StringComparison.Ordinal);
        Assert.DoesNotContain("GetGroupRows(derivedGroupMap, \"notes\")", view, StringComparison.Ordinal);
        Assert.True(
            view.IndexOf("Key = \"derived\"", StringComparison.Ordinal) < view.IndexOf("Key = \"raw\"", StringComparison.Ordinal),
            "Gameplay analysis results must appear before physical gameplay variables.");
    }

    [Fact]
    public void PlayerMetricLibrary_ShouldOnlyOpenTheFirstDomainInEachCollection()
    {
        var view = File.ReadAllText(Path.Combine(UiRoot, "Views", "Players", "Details.cshtml"));

        Assert.Contains("@(domainIndex == 0 ? \"open\" : null)", view, StringComparison.Ordinal);
        Assert.DoesNotContain("open=\"@(domainIndex == 0)\"", view, StringComparison.Ordinal);
    }

    [Fact]
    public void PlayerMetricLibrary_ShouldExcludeActionTokenUsageVariables()
    {
        var view = File.ReadAllText(Path.Combine(UiRoot, "Views", "Players", "Details.cshtml"));

        foreach (var rawGroup in new[] { "coins", "ingredients", "meal_orders", "needs", "donations", "gold", "pension", "life_risk", "financial_goals" })
        {
            Assert.Contains($"GetGroupRows(rawGroupMap, \"{rawGroup}\")", view, StringComparison.Ordinal);
        }

        Assert.DoesNotContain("GetGroupRows(rawGroupMap, \"actions\")", view, StringComparison.Ordinal);
        Assert.DoesNotContain("GetGroupRows(rawGroupMap, \"turns\")", view, StringComparison.Ordinal);
        Assert.DoesNotContain("players.details.raw.actions.title", view, StringComparison.Ordinal);
        Assert.DoesNotContain("metric.actions_used", view, StringComparison.Ordinal);

        foreach (var derivedGroup in new[] { "financial", "strategy", "behavior", "flourishing" })
        {
            Assert.Contains($"Key = \"{derivedGroup}\"", view, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void PlayerMetricLibrary_ShouldUseACuratedSetOfNonDuplicatePhysicalVariables()
    {
        var view = File.ReadAllText(Path.Combine(UiRoot, "Views", "Players", "Details.cshtml"));

        Assert.Contains("rawMetricKeysToShow", view, StringComparison.Ordinal);
        Assert.Contains("rows.RemoveAll", view, StringComparison.Ordinal);
        Assert.Contains("!rawMetricKeysToShow.Contains(rootKey)", view, StringComparison.Ordinal);
        foreach (var importantKey in new[]
        {
            "starting_coins",
            "ingredients_used_total",
            "meal_order_income_total",
            "donation_total_coins",
            "gold_investment_coins_earned",
            "pension_fund_total",
            "life_risks_accepted",
            "financial_goals_completed",
            "sharia_loans_unpaid_end"
        })
        {
            Assert.Contains($"\"{importantKey}\"", view, StringComparison.Ordinal);
        }

        foreach (var excludedKey in new[]
        {
            "cash_in_total",
            "coins_net_end_game",
            "ingredient_types_held",
            "meal_orders_claimed",
            "need_profile",
            "donation_rank_per_friday",
            "gold_prices_per_purchase",
            "pension_fund_rank_per_game",
            "life_risk_costs_per_card",
            "financial_goals_coins_per_goal",
            "day_when_first_risk_hit"
        })
        {
            Assert.DoesNotContain($"\"{excludedKey}\"", view, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void PlayerMetricLibrary_ShouldExcludeTechnicalTimelinesAndRepeatedAnalysisComponents()
    {
        var view = File.ReadAllText(Path.Combine(UiRoot, "Views", "Players", "Details.cshtml"));

        foreach (var technicalKey in new[]
        {
            "latest_event_action_slot",
            "action_slot_timeline",
            "coins_spent_per_turn",
            "coins_earned_per_turn",
            "meal_order_income_per_order",
            "donation_amount_per_friday"
        })
        {
            Assert.DoesNotContain($"\"{technicalKey}\"", view, StringComparison.Ordinal);
        }

        foreach (var duplicateAnalysisKey in new[]
        {
            "growth_pattern_ratio",
            "debt_ratio",
            "sharia_loans_outstanding_coins"
        })
        {
            Assert.Contains($"\"{duplicateAnalysisKey}\"", view, StringComparison.Ordinal);
        }

        Assert.DoesNotContain("Key = \"context\"", view, StringComparison.Ordinal);
        foreach (var componentGroup in new[]
        {
            "income_diversification_components",
            "expense_management_components",
            "risk_appetite_components",
            "goal_setting_components",
            "fulfillment_diversity_components",
            "donation_commitment_components",
            "happiness_portfolio"
        })
        {
            Assert.DoesNotContain(componentGroup, view, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void PlayerStats_ShouldUseOneVerdictThreePillarsAndPrioritizedActions()
    {
        var view = File.ReadAllText(Path.Combine(UiRoot, "Views", "Players", "Details.cshtml"));
        var css = File.ReadAllText(Path.Combine(UiRoot, "wwwroot", "css", "site.css"));

        Assert.Contains("player-stats-verdict", view, StringComparison.Ordinal);
        Assert.Equal(3, CountOccurrences(view, "<article class=\"player-stat-pillar"));
        Assert.Contains("Math.Min(3, statSummary.Insights.Count)", view, StringComparison.Ordinal);
        Assert.DoesNotContain("KeyMetrics", view, StringComparison.Ordinal);
        Assert.DoesNotContain("player-stats-primary-grid", view, StringComparison.Ordinal);
        Assert.Contains(".player-detail-overhaul .player-stats-verdict {", css, StringComparison.Ordinal);
        Assert.Contains(".player-detail-overhaul .player-stats-pillar-grid {", css, StringComparison.Ordinal);
        Assert.Contains(".player-detail-overhaul .player-stats-actions {", css, StringComparison.Ordinal);
        Assert.Contains(".player-detail-overhaul .player-metric-card-grid {", css, StringComparison.Ordinal);
        Assert.Contains("grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));", css, StringComparison.Ordinal);
        Assert.DoesNotContain(".player-detail-overhaul .player-stats-disclosure", css, StringComparison.Ordinal);
    }

    [Fact]
    public void PlayerStats_ShouldUsePlainLanguageExplanations()
    {
        var lexicon = File.ReadAllText(Path.Combine(UiRoot, "Infrastructure", "UiTextLexicon.Players.cs"));

        Assert.Contains("(\"Kesimpulan cepat\", \"Quick conclusion\")", lexicon, StringComparison.Ordinal);
        Assert.Contains("(\"Tiga pilar hasil pemain\", \"Three player-result pillars\")", lexicon, StringComparison.Ordinal);
        Assert.Contains("(\"Prioritas pembahasan\", \"Discussion priorities\")", lexicon, StringComparison.Ordinal);
        Assert.Contains("(\"Kondisi uang\", \"Money condition\")", lexicon, StringComparison.Ordinal);
        Assert.Contains("(\"Kepatuhan Kebutuhan Primer\", \"Primary-Need Compliance\")", lexicon, StringComparison.Ordinal);
        Assert.Contains("(\"Pembentuk Poin Kebahagiaan\", \"Happiness Score composition\")", lexicon, StringComparison.Ordinal);
        Assert.Contains("(\"Pustaka Data Pemain\", \"Player Data Library\")", lexicon, StringComparison.Ordinal);
        Assert.Contains("(\"Pertimbangan\", \"Consideration\")", lexicon, StringComparison.Ordinal);
        Assert.Contains("players.support.recommendation.savings", lexicon, StringComparison.Ordinal);
        Assert.Contains("players.support.recommendation.debt", lexicon, StringComparison.Ordinal);
        Assert.DoesNotContain("Lihat fungsi metrik", lexicon, StringComparison.Ordinal);
        Assert.DoesNotContain("Pustaka Metrik Pemain", lexicon, StringComparison.Ordinal);
        Assert.DoesNotContain("Data pendukung lengkap", lexicon, StringComparison.Ordinal);
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
