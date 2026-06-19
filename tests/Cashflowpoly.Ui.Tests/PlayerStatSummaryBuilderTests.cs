using Cashflowpoly.Ui.Contracts;
using Cashflowpoly.Ui.Infrastructure;
using Cashflowpoly.Ui.Models;
using Xunit;

namespace Cashflowpoly.Ui.Tests;

public sealed class PlayerStatSummaryBuilderTests
{
    [Fact]
    public void Build_CreatesPriorityKeyMetricsForInstructorReview()
    {
        var gameplay = BuildGameplay(
            cashflowNetTotal: 18,
            happinessPointsTotal: 42,
            primaryNeedRate: 0.82,
            hasUnpaidLoan: false);
        var journey = new PlayerCashflowJourneyStatsViewModel
        {
            EndingCash = 38,
            TotalCashIn = 60,
            TotalCashOut = 42,
            NetCashflow = 18,
            TransactionCount = 7
        };

        var summary = PlayerStatSummaryBuilder.Build(gameplay, journey, Translate);

        Assert.True(summary.KeyMetrics.Count >= 8);
        Assert.Contains(summary.KeyMetrics, item => item.Key == "ending_cash" && item.Value == "38");
        Assert.Contains(summary.KeyMetrics, item => item.Key == "net_cashflow" && item.Value == "18");
        Assert.Contains(summary.KeyMetrics, item => item.Key == "happiness_points" && item.Value == "42");
        Assert.Contains(summary.KeyMetrics, item => item.Key == "loan_status" && item.Value == "Clear");
    }

    [Fact]
    public void Build_AddsRiskInsightWhenCashflowIsNegative()
    {
        var summary = PlayerStatSummaryBuilder.Build(
            BuildGameplay(cashflowNetTotal: -12, happinessPointsTotal: 30, primaryNeedRate: 0.9, hasUnpaidLoan: false),
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
            Translate);

        Assert.Contains(summary.Insights, item => item.Key == "loan_unpaid" && item.Tone == "danger");
    }

    [Fact]
    public void Build_AddsPositiveInsightWhenHappinessIsStrong()
    {
        var summary = PlayerStatSummaryBuilder.Build(
            BuildGameplay(cashflowNetTotal: 8, happinessPointsTotal: 72, primaryNeedRate: 0.9, hasUnpaidLoan: false),
            null,
            Translate);

        Assert.Contains(summary.Insights, item => item.Key == "happiness_strong" && item.Tone == "positive");
    }

    [Fact]
    public void Build_AddsWarningInsightWhenPrimaryNeedComplianceIsLow()
    {
        var summary = PlayerStatSummaryBuilder.Build(
            BuildGameplay(cashflowNetTotal: 8, happinessPointsTotal: 30, primaryNeedRate: 0.55, hasUnpaidLoan: false),
            null,
            Translate);

        Assert.Contains(summary.Insights, item => item.Key == "primary_need_low" && item.Tone == "warning");
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
