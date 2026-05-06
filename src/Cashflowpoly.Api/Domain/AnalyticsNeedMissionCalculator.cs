// Fungsi file: Menghitung metrik kebutuhan dan misi gameplay dari event pemain.
using Cashflowpoly.Api.Data;
using static Cashflowpoly.Api.Domain.AnalyticsPayloadReader;
using static Cashflowpoly.Api.Domain.AnalyticsMath;

namespace Cashflowpoly.Api.Domain;

internal sealed record AnalyticsNeedMissionMetrics(
    int NeedCardsPurchased,
    int PrimaryNeeds,
    int SecondaryNeeds,
    int TertiaryNeeds,
    bool HasBasicNeedProfile,
    bool IsCollectorNeedProfile,
    bool IsSpecialistNeedProfile,
    bool? SpecificTertiaryAcquired,
    bool? CollectionMissionComplete,
    int NeedCoinsSpent,
    double? FulfillmentDiversity,
    int? MissionAchievement);

internal static class AnalyticsNeedMissionCalculator
{
    internal static AnalyticsNeedMissionMetrics Compute(
        IEnumerable<EventDb> playerEvents,
        IEnumerable<CashflowProjectionDb> playerProjections)
    {
        var primaryNeeds = 0;
        var secondaryNeeds = 0;
        var tertiaryNeeds = 0;
        var distinctNeedCardIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var tertiaryCardIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var missions = new List<MissionAssignment>();

        foreach (var evt in playerEvents)
        {
            if (evt.ActionType == "need.primary.purchased" &&
                TryReadNeedPurchase(evt.Payload, out _, out var primaryCardId, out _))
            {
                primaryNeeds += 1;
                if (!string.IsNullOrWhiteSpace(primaryCardId))
                {
                    distinctNeedCardIds.Add(primaryCardId);
                }
            }

            if (evt.ActionType == "need.secondary.purchased" &&
                TryReadNeedPurchase(evt.Payload, out _, out var secondaryCardId, out _))
            {
                secondaryNeeds += 1;
                if (!string.IsNullOrWhiteSpace(secondaryCardId))
                {
                    distinctNeedCardIds.Add(secondaryCardId);
                }
            }

            if (evt.ActionType == "need.tertiary.purchased" &&
                TryReadNeedPurchase(evt.Payload, out _, out var cardId, out _))
            {
                tertiaryNeeds += 1;
                if (!string.IsNullOrWhiteSpace(cardId))
                {
                    tertiaryCardIds.Add(cardId);
                }
            }

            if (evt.ActionType == "mission.assigned" &&
                TryReadMissionAssigned(evt.Payload, out var missionId, out var targetCardId, out var penaltyPoints, out var requirePrimary, out var requireSecondary))
            {
                missions.Add(new MissionAssignment(missionId, targetCardId, penaltyPoints, requirePrimary, requireSecondary));
            }
        }

        var needCardsPurchased = primaryNeeds + secondaryNeeds + tertiaryNeeds;
        var hasBasicNeedProfile = primaryNeeds > 0 && secondaryNeeds > 0 && tertiaryNeeds > 0;
        var isCollectorNeedProfile = distinctNeedCardIds.Count >= 4;
        var dominantNeedCount = Math.Max(primaryNeeds, Math.Max(secondaryNeeds, tertiaryNeeds));
        var isSpecialistNeedProfile = needCardsPurchased > 0 && ((double)dominantNeedCount / needCardsPurchased) >= 0.7;
        var needCoinsSpent = playerProjections
            .Where(p => p.Direction == "OUT" &&
                        (p.Category == "NEED_PRIMARY" || p.Category == "NEED_SECONDARY" || p.Category == "NEED_TERTIARY"))
            .Sum(p => p.Amount);

        bool? specificTertiaryAcquired = null;
        bool? collectionMissionComplete = null;
        if (missions.Count > 0)
        {
            specificTertiaryAcquired = missions.Any(m => !string.IsNullOrWhiteSpace(m.TargetTertiaryCardId) &&
                                                        tertiaryCardIds.Contains(m.TargetTertiaryCardId));

            var hasPrimary = primaryNeeds > 0;
            var hasSecondary = secondaryNeeds > 0;
            collectionMissionComplete = missions.All(m =>
            {
                var hasTarget = string.IsNullOrWhiteSpace(m.TargetTertiaryCardId) ||
                                tertiaryCardIds.Contains(m.TargetTertiaryCardId);
                var requirePrimary = !m.RequirePrimary || hasPrimary;
                var requireSecondary = !m.RequireSecondary || hasSecondary;
                return hasTarget && requirePrimary && requireSecondary;
            });
        }

        var pPrimary = SafeRatio(primaryNeeds, needCardsPurchased);
        var pSecondary = SafeRatio(secondaryNeeds, needCardsPurchased);
        var pTertiary = SafeRatio(tertiaryNeeds, needCardsPurchased);
        var fulfillmentDiversity =
            pPrimary.HasValue && pSecondary.HasValue && pTertiary.HasValue
                ? (1 - (Math.Pow(pPrimary.Value, 2) + Math.Pow(pSecondary.Value, 2) + Math.Pow(pTertiary.Value, 2)))
                    / (1 - (1d / 3))
                : (double?)null;
        var missionAchievement = collectionMissionComplete.HasValue
            ? (collectionMissionComplete.Value ? 1 : 0)
            : (int?)null;

        return new AnalyticsNeedMissionMetrics(
            needCardsPurchased,
            primaryNeeds,
            secondaryNeeds,
            tertiaryNeeds,
            hasBasicNeedProfile,
            isCollectorNeedProfile,
            isSpecialistNeedProfile,
            specificTertiaryAcquired,
            collectionMissionComplete,
            needCoinsSpent,
            fulfillmentDiversity,
            missionAchievement);
    }

    private sealed record MissionAssignment(
        string MissionId,
        string TargetTertiaryCardId,
        int PenaltyPoints,
        bool RequirePrimary,
        bool RequireSecondary);
}
