// Fungsi file: Memverifikasi perilaku, lokalisasi, atau tata letak UI melalui PlayerStatSummaryBuilderTests.
using System.Text.Json;
using Cashflowpoly.Ui.Contracts;
using Cashflowpoly.Ui.Infrastructure;
using Cashflowpoly.Ui.Models;
using Xunit;

namespace Cashflowpoly.Ui.Tests;

public sealed class PlayerStatSummaryBuilderTests
{
    [Theory]
    [InlineData("{\"needs\":{\"collection_mission_complete\":true}}", true)]
    [InlineData("{\"needs\":{\"collection_mission_complete\":false}}", false)]
    [InlineData("{\"needs\":{\"collection_mission_complete\":null}}", null)]
    [InlineData("{\"needs\":{\"collection_mission_complete\":\"true\"}}", null)]
    [InlineData("{\"needs\":{}}", null)]
    [InlineData("{\"needs\":null}", null)]
    [InlineData("{}", null)]
    [InlineData("null", null)]
    public void Build_UsesExplicitMissionResultWithoutTreatingMissingDataAsIncomplete(string rawJson, bool? expected)
    {
        var gameplay = BuildGameplay(8, 30, 0.9, false) with
        {
            RawJson = JsonSerializer.Deserialize<JsonElement>(rawJson)
        };

        var summary = PlayerStatSummaryBuilder.Build(gameplay, null, null, Translate);

        Assert.Equal(expected, summary.CollectionMissionComplete);
    }

    [Fact]
    public void Build_MissingGameplayHasUnknownMissionResult()
    {
        var summary = PlayerStatSummaryBuilder.Build(null, null, null, Translate);

        Assert.Null(summary.CollectionMissionComplete);
    }

    [Fact]
    public void Build_AddsRiskInsightWhenCashflowIsNegative()
    {
        var summary = PlayerStatSummaryBuilder.Build(
            BuildGameplay(cashflowNetTotal: -12, happinessPointsTotal: 30, fulfillmentDiversity: 0.9, hasUnpaidLoan: false),
            null,
            null,
            Translate);

        Assert.Contains(summary.Insights, item => item.Key == "cashflow_negative" && item.Tone == "warning");
    }

    [Fact]
    public void Build_AddsRiskInsightWhenLoanIsUnpaid()
    {
        var summary = PlayerStatSummaryBuilder.Build(
            BuildGameplay(cashflowNetTotal: 8, happinessPointsTotal: 30, fulfillmentDiversity: 0.9, hasUnpaidLoan: true),
            null,
            null,
            Translate);

        Assert.Contains(summary.Insights, item => item.Key == "loan_unpaid" && item.Tone == "danger");
    }

    [Fact]
    public void Build_DoesNotRepeatPositivePillarsAsDiscussionPriorities()
    {
        var summary = PlayerStatSummaryBuilder.Build(
            BuildGameplay(cashflowNetTotal: 8, happinessPointsTotal: 72, fulfillmentDiversity: 0.9, hasUnpaidLoan: false),
            null,
            null,
            Translate);

        Assert.Single(summary.Insights);
        Assert.Equal("stable_profile", summary.Insights[0].Key);
    }

    [Fact]
    public void Build_AddsWarningInsightWhenNeedDiversityIsLow()
    {
        var summary = PlayerStatSummaryBuilder.Build(
            BuildGameplay(cashflowNetTotal: 8, happinessPointsTotal: 30, fulfillmentDiversity: 0.35, hasUnpaidLoan: false),
            null,
            null,
            Translate);

        Assert.Contains(summary.Insights, item => item.Key == "need_diversity_low" && item.Tone == "warning");
    }

    [Fact]
    public void Build_ExplainsWhenNeedBalanceCannotBeCalculated()
    {
        var summary = PlayerStatSummaryBuilder.Build(
            BuildGameplay(cashflowNetTotal: 8, happinessPointsTotal: 30, fulfillmentDiversity: 0, hasUnpaidLoan: false, needCardsOwned: 0),
            null,
            null,
            Translate);

        Assert.Contains(summary.Insights, item => item.Key == "need_cards_missing" && item.Tone == "warning");
        Assert.DoesNotContain(summary.Insights, item => item.Key == "need_diversity_low");
    }

    [Fact]
    public void Build_PrioritizesRisksWithoutPositiveDuplicates()
    {
        var summary = PlayerStatSummaryBuilder.Build(
            BuildGameplay(cashflowNetTotal: 8, happinessPointsTotal: 72, fulfillmentDiversity: 0.35, hasUnpaidLoan: true),
            null,
            null,
            Translate);

        Assert.Equal(2, summary.Insights.Count);
        Assert.Equal("loan_unpaid", summary.Insights[0].Key);
        Assert.Contains(summary.Insights, item => item.Key == "need_diversity_low");
        Assert.DoesNotContain(summary.Insights, item => item.Tone == "positive");
    }

    [Fact]
    public void Build_PrefersTheAuthoritativePlayerAnalyticsSummary()
    {
        var summary = PlayerStatSummaryBuilder.Build(
            BuildGameplay(cashflowNetTotal: 0, happinessPointsTotal: 0, fulfillmentDiversity: 0, hasUnpaidLoan: false),
            BuildAnalyticsSummary(cashIn: 50, cashOut: 20, happiness: 72, fulfillmentDiversity: 0.9, hasUnpaidLoan: false),
            null,
            Translate);

        Assert.Contains(summary.Insights, item => item.Key == "stable_profile");
        Assert.DoesNotContain(summary.Insights, item => item.Key == "happiness_low");
        Assert.DoesNotContain(summary.Insights, item => item.Key == "need_diversity_low");
    }

    private static AnalyticsByPlayerItem BuildAnalyticsSummary(
        double cashIn,
        double cashOut,
        double happiness,
        double fulfillmentDiversity,
        bool hasUnpaidLoan)
    {
        return new AnalyticsByPlayerItem(
            Guid.NewGuid(), 1, cashIn, cashOut, 0, 0, 0, 0, 0,
            fulfillmentDiversity, happiness, 0, 0, 0, 0, 0, 0, 0,
            hasUnpaidLoan ? 4 : 0, hasUnpaidLoan);
    }

    private static GameplayMetricsResponse BuildGameplay(
        double cashflowNetTotal,
        double happinessPointsTotal,
        double fulfillmentDiversity,
        bool hasUnpaidLoan,
        int? needCardsOwned = null)
    {
        JsonElement? rawJson = needCardsOwned.HasValue
            ? JsonSerializer.SerializeToElement(new
            {
                needs = new { need_cards_owned_current = needCardsOwned.Value }
            })
            : null;

        return new GameplayMetricsResponse(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTimeOffset.UtcNow,
            new GameplayEconomyMetrics(20, 50, 50 - cashflowNetTotal, cashflowNetTotal, 6),
            new GameplayProgressMetrics(3, 2, 5, 11),
            new GameplayScoreMetrics(happinessPointsTotal, 12, 2, 6, 4, 3, 5, 0, hasUnpaidLoan ? 4 : 0, hasUnpaidLoan),
            new GameplayNeedMetrics(fulfillmentDiversity),
            rawJson);
    }

    private static string Translate(string key)
    {
        return key switch
        {
            "players.stats.clear" => "Clear",
            "players.stats.unpaid" => "Unpaid",
            _ => key
        };
    }
}
