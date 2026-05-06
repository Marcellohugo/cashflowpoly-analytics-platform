// Fungsi file: Menguji kalkulator skor dan ringkasan analitik sesi.
using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Domain;
using Xunit;

namespace Cashflowpoly.Api.Tests;

/// <summary>
/// Kelas pengujian unit untuk kalkulasi skor dan ringkasan analitik murni.
/// </summary>
public sealed class AnalyticsScoreCalculatorTests
{
    [Fact]
    /// <summary>
    /// Memvalidasi ringkasan sesi menjumlahkan cashflow masuk, keluar, net, event, dan pelanggaran.
    /// </summary>
    public void BuildSummary_ComputesCashflowTotalsAndEventCount()
    {
        var events = new List<EventDb>
        {
            new() { EventId = Guid.NewGuid() },
            new() { EventId = Guid.NewGuid() }
        };
        var projections = new List<CashflowProjectionDb>
        {
            new() { Direction = "IN", Amount = 12 },
            new() { Direction = "OUT", Amount = 5 },
            new() { Direction = "IN", Amount = 3 }
        };

        var summary = AnalyticsScoreCalculator.BuildSummary(events, projections, rulesViolationsCount: 4);

        Assert.Equal(2, summary.EventCount);
        Assert.Equal(15, summary.CashInTotal);
        Assert.Equal(5, summary.CashOutTotal);
        Assert.Equal(10, summary.CashflowNetTotal);
        Assert.Equal(4, summary.RulesViolationsCount);
    }

    [Fact]
    /// <summary>
    /// Memvalidasi skor pembelajaran memakai bobot cashflow, compliance, dan happiness.
    /// </summary>
    public void ComputeLearningPerformanceScore_UsesWeightedComponents()
    {
        var score = AnalyticsScoreCalculator.ComputeLearningPerformanceScore(
            cashInTotal: 8,
            cashOutTotal: 3,
            happinessPointsTotal: 10,
            complianceRate: 0.8);

        Assert.Equal(70.5, score);
    }

    [Fact]
    /// <summary>
    /// Memvalidasi rata-rata nullable mengabaikan nilai null dan membulatkan dua digit.
    /// </summary>
    public void AverageNullable_IgnoresNullAndRounds()
    {
        var value = AnalyticsScoreCalculator.AverageNullable(new double?[] { 10, null, 11.555 });

        Assert.Equal(10.78, value);
    }
}
