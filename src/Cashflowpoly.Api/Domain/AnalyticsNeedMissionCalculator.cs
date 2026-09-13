// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui AnalyticsNeedMissionCalculator.
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;
// Mengimpor namespace `Cashflowpoly.Api.Domain.AnalyticsMath` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using static Cashflowpoly.Api.Domain.AnalyticsMath;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Domain;

public sealed record AnalyticsNeedMissionMetrics(
    // Parameter `NeedCardsPurchased` bertipe `int` membawa nilai kebutuhan kartu dibeli.
    int NeedCardsPurchased,
    // Parameter `NeedCardsOwnedCurrent` bertipe `int` membawa nilai kebutuhan kartu dimiliki saat ini.
    int NeedCardsOwnedCurrent,
    // Parameter `PrimaryNeeds` bertipe `int` membawa nilai primary kebutuhan.
    int PrimaryNeeds,
    // Parameter `SecondaryNeeds` bertipe `int` membawa nilai secondary kebutuhan.
    int SecondaryNeeds,
    // Parameter `TertiaryNeeds` bertipe `int` membawa nilai tertiary kebutuhan.
    int TertiaryNeeds,
    // Parameter `HasBasicNeedProfile` bertipe `bool` membawa nilai memiliki basic kebutuhan profile.
    bool HasBasicNeedProfile,
    // Parameter `IsCollectorNeedProfile` bertipe `bool` membawa nilai berstatus collector kebutuhan profile.
    bool IsCollectorNeedProfile,
    // Parameter `IsSpecialistNeedProfile` bertipe `bool` membawa nilai berstatus specialist kebutuhan profile.
    bool IsSpecialistNeedProfile,
    // Parameter `SpecificTertiaryAcquired` bertipe `bool?` membawa nilai specific tertiary acquired; nilai null diizinkan ketika data opsional belum
    // tersedia.
    bool? SpecificTertiaryAcquired,
    // Parameter `CollectionMissionComplete` bertipe `bool?` membawa nilai collection misi complete; nilai null diizinkan ketika data opsional belum
    // tersedia.
    bool? CollectionMissionComplete,
    // Parameter `NeedCoinsSpent` bertipe `int` membawa nilai kebutuhan coins spent.
    int NeedCoinsSpent,
    // Parameter `FulfillmentDiversity` bertipe `double?` membawa tingkat keberagaman kategori kebutuhan yang telah dipenuhi; nilai null diizinkan
    // ketika data opsional belum tersedia.
    double? FulfillmentDiversity,
    // Parameter `FulfillmentDiversityDocumentFormula` bertipe `double?` membawa nilai pemenuhan keberagaman document formula; nilai null diizinkan
    // ketika data opsional belum tersedia.
    double? FulfillmentDiversityDocumentFormula,
    // Parameter `MissionAchievement` bertipe `int?` membawa nilai misi achievement; nilai null diizinkan ketika data opsional belum tersedia.
    int? MissionAchievement);

internal sealed class NeedMissionCalculator : INeedMissionCalculator
{
    private static readonly AnalyticsPayloadReader _payloadReader = new();

    public AnalyticsNeedMissionMetrics Compute(
        // Parameter `playerEvents` bertipe `IEnumerable<EventDb>` membawa nilai pemain event.
        IEnumerable<EventDb> playerEvents,
        // Parameter `playerProjections` bertipe `IEnumerable<CashflowProjectionDb>` membawa nilai pemain projections.
        IEnumerable<CashflowProjectionDb> playerProjections)
    {
        var activeNeeds = new List<NeedCard>();
        var purchasedNeeds = new List<NeedCard>();
        var missions = new List<MissionAssignment>();
        var needCardsPurchased = 0;

        // Mengulangi setiap elemen `playerEvents.OrderBy(e => e.SequenceNumber)`; elemen saat ini disimpan sebagai `evt` bertipe `var` untuk diproses oleh
        // badan loop dalam Compute.
        foreach (var evt in playerEvents.OrderBy(e => e.SequenceNumber))
        {
            if (evt.ActionType == "Kebutuhan" &&
                _payloadReader.TryReadNeedPurchase(evt.Payload, out _, out var cardId, out _))
            {
                var purchasedNeed = new NeedCard(cardId, NeedTierClassifier.FromPayloadJson(evt.Payload));
                activeNeeds.Add(purchasedNeed);
                purchasedNeeds.Add(purchasedNeed);
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
        var purchasedPrimaryNeeds = purchasedNeeds.Count(need => need.Tier == NeedTier.Primary);
        var purchasedSecondaryNeeds = purchasedNeeds.Count(need => need.Tier == NeedTier.Secondary);
        var purchasedTertiaryCardIds = purchasedNeeds
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
                                                        purchasedTertiaryCardIds.Contains(m.TargetTertiaryCardId));

            var hasPrimary = purchasedPrimaryNeeds > 0;
            var hasSecondary = purchasedSecondaryNeeds > 0;
            collectionMissionComplete = missions.All(m =>
            {
                var hasTarget = string.IsNullOrWhiteSpace(m.TargetTertiaryCardId) ||
                                purchasedTertiaryCardIds.Contains(m.TargetTertiaryCardId);
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
                // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: (1 - (Math.Pow(pPrimary.Value, 2) + Math.Pow(pSecondary.Value, 2) +
                // Math.Pow(pTertiary.Value, 2))) dalam Compute.
                ? (1 - (Math.Pow(pPrimary.Value, 2) + Math.Pow(pSecondary.Value, 2) + Math.Pow(pTertiary.Value, 2)))
                    / (1 - (1d / 3))
                // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: (double?)null; dalam Compute.
                : (double?)null;
        var fulfillmentDiversityDocumentFormula = needCardsOwnedCurrent > 0
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: Math.Sqrt( dalam Compute.
            ? Math.Sqrt(
                Math.Pow(primaryNeeds, 2) +
                Math.Pow(secondaryNeeds, 2) +
                Math.Pow(tertiaryNeeds, 2)) / needCardsOwnedCurrent
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: (double?)null; dalam Compute.
            : (double?)null;
        var missionAchievement = collectionMissionComplete.HasValue
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: (collectionMissionComplete.Value ? 1 : 0) dalam Compute.
            ? (collectionMissionComplete.Value ? 1 : 0)
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: (int?)null; dalam Compute.
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
        // Parameter `MissionId` bertipe `string` membawa identitas misi koleksi yang ditugaskan.
        string MissionId,
        // Parameter `TargetTertiaryCardId` bertipe `string` membawa nilai target tertiary kartu identitas.
        string TargetTertiaryCardId,
        // Parameter `PenaltyPoints` bertipe `int` membawa nilai penalti poin.
        int PenaltyPoints,
        // Parameter `RequirePrimary` bertipe `bool` membawa nilai require primary.
        bool RequirePrimary,
        // Parameter `RequireSecondary` bertipe `bool` membawa nilai require secondary.
        bool RequireSecondary);

    private sealed record NeedCard(string CardId, NeedTier Tier);
}
