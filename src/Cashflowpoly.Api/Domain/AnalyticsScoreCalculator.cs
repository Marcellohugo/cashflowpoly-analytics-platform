// Fungsi file: Menghitung ringkasan sesi dan skor performa analitik.
using Cashflowpoly.Api.Data;
using Cashflowpoly.Contracts;

namespace Cashflowpoly.Api.Domain;

/// <summary>
/// Kalkulator murni untuk ringkasan sesi dan skor performa analitik.
/// </summary>
internal sealed class ScoreCalculator : IScoreCalculator
{
    /// <summary>
    /// Membangun ringkasan analitik sesi dari event, proyeksi cashflow, dan jumlah pelanggaran aturan.
    /// </summary>
    public AnalyticsSessionSummary BuildSummary(
        List<EventDb> events,
        List<CashflowProjectionDb> projections,
        int rulesViolationsCount)
    {
        var cashInTotal = projections.Where(p => p.Direction == "IN").Sum(p => (double)p.Amount);
        var cashOutTotal = projections.Where(p => p.Direction == "OUT").Sum(p => (double)p.Amount);
        var cashflowNetTotal = cashInTotal - cashOutTotal;
        return new AnalyticsSessionSummary(events.Count, cashInTotal, cashOutTotal, cashflowNetTotal, rulesViolationsCount);
    }

    /// <summary>
    /// Menghitung skor performa pembelajaran berdasarkan cashflow, compliance, dan happiness dengan bobot tertimbang.
    /// </summary>
    public double? ComputeLearningPerformanceScore(
        double cashInTotal,
        double cashOutTotal,
        double happinessPointsTotal,
        double? complianceRate)
    {
        var cashflowComponent = AnalyticsMath.Clamp(50 + 5 * (cashInTotal - cashOutTotal), 0, 100);
        var complianceComponent = complianceRate.HasValue ? AnalyticsMath.Clamp(complianceRate.Value * 100, 0, 100) : (double?)null;
        var happinessComponent = AnalyticsMath.Clamp(5 * happinessPointsTotal, 0, 100);

        return WeightedAverage(new (double? value, double weight)[]
        {
            (cashflowComponent, 0.40),
            (complianceComponent, 0.35),
            (happinessComponent, 0.25)
        });
    }

    /// <summary>
    /// Menghitung skor performa misi berdasarkan penalti misi dan penalti pinjaman.
    /// </summary>
    public double ComputeMissionPerformanceScore(double missionPenaltyTotal, double loanPenaltyTotal)
    {
        var missionPenaltyComponent = AnalyticsMath.Clamp(missionPenaltyTotal + loanPenaltyTotal, 0, 100);
        return AnalyticsMath.Clamp(100 - missionPenaltyComponent, 0, 100);
    }

    /// <summary>
    /// Menghitung rata-rata dari nilai-nilai nullable, mengabaikan null.
    /// </summary>
    public double? AverageNullable(IEnumerable<double?> values)
    {
        var nonNull = values.Where(value => value.HasValue).Select(value => value!.Value).ToList();
        if (nonNull.Count == 0)
        {
            return null;
        }

        return Math.Round(nonNull.Average(), 2);
    }

    /// <summary>
    /// Menghitung rata-rata tertimbang dari komponen yang memiliki nilai, mengabaikan komponen null.
    /// </summary>
    private double? WeightedAverage(IEnumerable<(double? value, double weight)> components)
    {
        var active = components.Where(item => item.value.HasValue).ToList();
        if (active.Count == 0)
        {
            return null;
        }

        var weightTotal = active.Sum(item => item.weight);
        if (weightTotal <= 0)
        {
            return null;
        }

        var weightedScore = active.Sum(item => item.value!.Value * item.weight) / weightTotal;
        return Math.Round(weightedScore, 2);
    }
}
