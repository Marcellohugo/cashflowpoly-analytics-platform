// Fungsi file: Memverifikasi perilaku, lokalisasi, atau tata letak UI melalui PlayerStatSummaryBuilderTests.
using Cashflowpoly.Ui.Contracts;
using Cashflowpoly.Ui.Infrastructure;
using Cashflowpoly.Ui.Models;
using Xunit;

namespace Cashflowpoly.Ui.Tests;

public sealed class PlayerStatSummaryBuilderTests
{
    [Fact]
    public void Build_AddsRiskInsightWhenCashflowIsNegative()
    {
        var summary = PlayerStatSummaryBuilder.Build(
            BuildGameplay(cashflowNetTotal: -12, happinessPointsTotal: 30, primaryNeedRate: 0.9, hasUnpaidLoan: false),
            null,
            null,
            Translate);

        Assert.Contains(summary.Insights, item => item.Key == "cashflow_negative" && item.Tone == "warning");
    }

    [Fact]
    public void Build_AddsRiskInsightWhenLoanIsUnpaid()
    {
        var summary = PlayerStatSummaryBuilder.Build(
            BuildGameplay(cashflowNetTotal: 8, happinessPointsTotal: 30, primaryNeedRate: 0.9, hasUnpaidLoan: true),
            null,
            null,
            Translate);

        Assert.Contains(summary.Insights, item => item.Key == "loan_unpaid" && item.Tone == "danger");
    }

    [Fact]
    public void Build_DoesNotRepeatPositivePillarsAsDiscussionPriorities()
    {
        var summary = PlayerStatSummaryBuilder.Build(
            BuildGameplay(cashflowNetTotal: 8, happinessPointsTotal: 72, primaryNeedRate: 0.9, hasUnpaidLoan: false),
            null,
            null,
            Translate);

        Assert.Single(summary.Insights);
        Assert.Equal("stable_profile", summary.Insights[0].Key);
    }

    [Fact]
    public void Build_AddsWarningInsightWhenPrimaryNeedComplianceIsLow()
    {
        var summary = PlayerStatSummaryBuilder.Build(
            BuildGameplay(cashflowNetTotal: 8, happinessPointsTotal: 30, primaryNeedRate: 0.55, hasUnpaidLoan: false),
            null,
            null,
            Translate);

        Assert.Contains(summary.Insights, item => item.Key == "primary_need_low" && item.Tone == "warning");
    }

    [Fact]
    public void Build_PrioritizesRisksWithoutPositiveDuplicates()
    {
        var summary = PlayerStatSummaryBuilder.Build(
            BuildGameplay(cashflowNetTotal: 8, happinessPointsTotal: 72, primaryNeedRate: 0.55, hasUnpaidLoan: true),
            null,
            null,
            Translate);

        Assert.Equal(2, summary.Insights.Count);
        Assert.Equal("loan_unpaid", summary.Insights[0].Key);
        Assert.Contains(summary.Insights, item => item.Key == "primary_need_low");
        Assert.DoesNotContain(summary.Insights, item => item.Tone == "positive");
    }

    [Fact]
    public void Build_PrefersTheAuthoritativePlayerAnalyticsSummary()
    {
        var summary = PlayerStatSummaryBuilder.Build(
            BuildGameplay(cashflowNetTotal: 0, happinessPointsTotal: 0, primaryNeedRate: 0, hasUnpaidLoan: false),
            BuildAnalyticsSummary(cashIn: 50, cashOut: 20, happiness: 72, primaryNeedRate: 0.9, hasUnpaidLoan: false),
            null,
            Translate);

        Assert.Contains(summary.Insights, item => item.Key == "stable_profile");
        Assert.DoesNotContain(summary.Insights, item => item.Key == "happiness_low");
        Assert.DoesNotContain(summary.Insights, item => item.Key == "primary_need_low");
    }

    private static AnalyticsByPlayerItem BuildAnalyticsSummary(
        double cashIn,
        double cashOut,
        double happiness,
        double primaryNeedRate,
        bool hasUnpaidLoan)
    {
        return new AnalyticsByPlayerItem(
            Guid.NewGuid(), 1, cashIn, cashOut, 0, 0, 0, 0, 0,
            primaryNeedRate, 0, happiness, 0, 0, 0, 0, 0, 0, 0,
            hasUnpaidLoan ? 4 : 0, hasUnpaidLoan);
    }

    private static GameplayMetricsResponse BuildGameplay(
        double cashflowNetTotal,
        double happinessPointsTotal,
        double primaryNeedRate,
        bool hasUnpaidLoan)
    {
        return new GameplayMetricsResponse(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTimeOffset.UtcNow,
            new GameplayEconomyMetrics(20, 50, 50 - cashflowNetTotal, cashflowNetTotal, 6),
            new GameplayProgressMetrics(3, 2, 5, 11),
            new GameplayScoreMetrics(happinessPointsTotal, 12, 2, 6, 4, 3, 5, 0, hasUnpaidLoan ? 4 : 0, hasUnpaidLoan),
            new GameplayComplianceMetrics(primaryNeedRate, primaryNeedRate < 0.75 ? 2 : 0));
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
