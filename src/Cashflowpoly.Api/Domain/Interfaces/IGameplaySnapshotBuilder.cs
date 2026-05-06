using Cashflowpoly.Api.Data;

namespace Cashflowpoly.Api.Domain;

public interface IGameplaySnapshotBuilder
{
    AnalyticsGameplaySnapshot Build(
        List<EventDb> playerEvents,
        List<CashflowProjectionDb> playerProjections,
        List<EventDb> allEvents,
        RulesetConfig? config,
        AnalyticsHappinessBreakdown happiness);
}
