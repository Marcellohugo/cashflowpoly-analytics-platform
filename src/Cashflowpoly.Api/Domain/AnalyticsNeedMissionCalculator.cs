// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui AnalyticsNeedMissionCalculator.
using Cashflowpoly.Api.Data;
using static Cashflowpoly.Api.Domain.AnalyticsMath;

namespace Cashflowpoly.Api.Domain;

public sealed record AnalyticsNeedMissionMetrics(
    int NeedCardsPurchased,
    int NeedCardsOwnedCurrent,
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
    double? FulfillmentDiversityDocumentFormula,
    int? MissionAchievement);

internal sealed class NeedMissionCalculator : INeedMissionCalculator
{
    private static readonly AnalyticsPayloadReader _payloadReader = new();

    public AnalyticsNeedMissionMetrics Compute(
        IEnumerable<EventDb> playerEvents,
        IEnumerable<CashflowProjectionDb> playerProjections)
    {
        var activeNeeds = new List<NeedCard>();
        var missions = new List<MissionAssignment>();
        var needCardsPurchased = 0;

        foreach (var evt in playerEvents.OrderBy(e => e.SequenceNumber))
        {
            if (evt.ActionType == "Kebutuhan" &&
                _payloadReader.TryReadNeedPurchase(evt.Payload, out _, out var cardId, out _))
            {
                activeNeeds.Add(new NeedCard(cardId, NeedTierClassifier.FromPayloadJson(evt.Payload)));
                needCardsPurchased++;
            }

            if (evt.ActionType == "GunakanOpsiDarurat" &&
                _payloadReader.TryReadSoldNeed(evt.Payload, out var soldNeedCardId))
            {
                var soldIndex = activeNeeds.FindIndex(need =>
                    string.Equals(need.CardId, soldNeedCardId, StringComparison.OrdinalIgnoreCase));
                if (soldIndex >= 0)
                {
                    activeNeeds.RemoveAt(soldIndex);
                }
            }

            if (string.Equals(evt.ActionType, GameActionCatalog.SetupMisiAwal, StringComparison.OrdinalIgnoreCase) &&
                _payloadReader.TryReadMissionAssigned(evt.Payload, out var missionId, out var targetCardId, out var penaltyPoints, out var requirePrimary, out var requireSecondary))
            {
                missions.Add(new MissionAssignment(missionId, targetCardId, penaltyPoints, requirePrimary, requireSecondary));
            }
        }

        var primaryNeeds = activeNeeds.Count(need => need.Tier == NeedTier.Primary);
        var secondaryNeeds = activeNeeds.Count(need => need.Tier == NeedTier.Secondary);
        var tertiaryNeeds = activeNeeds.Count(need => need.Tier == NeedTier.Tertiary);
        var distinctNeedCardIds = activeNeeds
            .Select(need => need.CardId)
            .Where(cardId => !string.IsNullOrWhiteSpace(cardId))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var tertiaryCardIds = activeNeeds
            .Where(need => need.Tier == NeedTier.Tertiary)
            .Select(need => System.Text.RegularExpressions.Regex.Replace(need.CardId, "_[0-9]+$", ""))
            .Where(cardId => !string.IsNullOrWhiteSpace(cardId))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var needCardsOwnedCurrent = primaryNeeds + secondaryNeeds + tertiaryNeeds;
        var hasBasicNeedProfile = primaryNeeds > 0 && secondaryNeeds > 0 && tertiaryNeeds > 0;
        var isCollectorNeedProfile = distinctNeedCardIds.Count >= 4;
        var dominantNeedCount = Math.Max(primaryNeeds, Math.Max(secondaryNeeds, tertiaryNeeds));
        var isSpecialistNeedProfile = needCardsOwnedCurrent > 0 && ((double)dominantNeedCount / needCardsOwnedCurrent) >= 0.7;
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

        var pPrimary = SafeRatio(primaryNeeds, needCardsOwnedCurrent);
        var pSecondary = SafeRatio(secondaryNeeds, needCardsOwnedCurrent);
        var pTertiary = SafeRatio(tertiaryNeeds, needCardsOwnedCurrent);
        var fulfillmentDiversity =
            pPrimary.HasValue && pSecondary.HasValue && pTertiary.HasValue
                ? (1 - (Math.Pow(pPrimary.Value, 2) + Math.Pow(pSecondary.Value, 2) + Math.Pow(pTertiary.Value, 2)))
                    / (1 - (1d / 3))
                : (double?)null;
        var fulfillmentDiversityDocumentFormula = needCardsOwnedCurrent > 0
            ? Math.Sqrt(
                Math.Pow(primaryNeeds, 2) +
                Math.Pow(secondaryNeeds, 2) +
                Math.Pow(tertiaryNeeds, 2)) / needCardsOwnedCurrent
            : (double?)null;
        var missionAchievement = collectionMissionComplete.HasValue
            ? (collectionMissionComplete.Value ? 1 : 0)
            : (int?)null;

        return new AnalyticsNeedMissionMetrics(
            needCardsPurchased,
            needCardsOwnedCurrent,
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
            fulfillmentDiversityDocumentFormula,
            missionAchievement);
    }

    private sealed record MissionAssignment(
        string MissionId,
        string TargetTertiaryCardId,
        int PenaltyPoints,
        bool RequirePrimary,
        bool RequireSecondary);

    private sealed record NeedCard(string CardId, NeedTier Tier);
}
