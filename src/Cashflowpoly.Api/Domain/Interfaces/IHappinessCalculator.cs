using Cashflowpoly.Api.Data;

namespace Cashflowpoly.Api.Domain;

internal interface IHappinessCalculator
{
    Dictionary<Guid, AnalyticsHappinessBreakdown> ComputeByPlayer(
        List<EventDb> events,
        List<CashflowProjectionDb> projections,
        RulesetConfig? config);

    AnalyticsHappinessBreakdown ComputeBreakdown(
        List<EventDb> playerEvents,
        double donationPoints,
        double goldPoints,
        double pensionPoints);

    double SumRankAwarded(IEnumerable<EventDb> events, string actionType);

    double SumPointsAwarded(IEnumerable<EventDb> events, string actionType);
}
