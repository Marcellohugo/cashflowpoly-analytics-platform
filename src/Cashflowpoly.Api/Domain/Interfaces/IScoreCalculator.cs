using Cashflowpoly.Api.Data;
using Cashflowpoly.Contracts;

namespace Cashflowpoly.Api.Domain;

internal interface IScoreCalculator
{
    AnalyticsSessionSummary BuildSummary(
        List<EventDb> events,
        List<CashflowProjectionDb> projections,
        int rulesViolationsCount);

    double? ComputeLearningPerformanceScore(
        double cashInTotal,
        double cashOutTotal,
        double happinessPointsTotal,
        double? complianceRate);

    double ComputeMissionPerformanceScore(double missionPenaltyTotal, double loanPenaltyTotal);

    double? AverageNullable(IEnumerable<double?> values);
}
