// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui IGameplaySnapshotBuilder.
using Cashflowpoly.Api.Data;

namespace Cashflowpoly.Api.Domain;

public interface IGameplaySnapshotBuilder
{
    AnalyticsGameplaySnapshot Build(
        List<EventDb> playerEvents,
        List<CashflowProjectionDb> playerProjections,
        List<EventDb> allEvents,
        RulesetConfig? config,
        AnalyticsHappinessBreakdown happiness,
        SessionFinalScoreDb? finalScore = null,
        int? pensionRank = null,
        string? playerAlias = null,
        bool sessionEnded = false);
}
