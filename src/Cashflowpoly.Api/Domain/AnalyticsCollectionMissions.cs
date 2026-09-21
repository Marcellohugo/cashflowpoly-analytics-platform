// Fungsi file: Menilai misi koleksi dari persyaratan ruleset dan riwayat pembelian pemain.
using System.Text.Json;
using Cashflowpoly.Api.Contracts;
using Cashflowpoly.Api.Data;

namespace Cashflowpoly.Api.Domain;

internal static class AnalyticsCollectionMissions
{
    internal sealed record Evaluation(bool Complete, bool SpecificTertiaryAcquired, int PenaltyPoints, int RewardPoints = 0);

    public static List<Evaluation> Evaluate(IEnumerable<EventDb> playerEvents, RulesetConfig? config)
    {
        var events = playerEvents.ToList();
        var reader = new AnalyticsPayloadReader();
        var purchases = events.Where(evt => evt.ActionType == "Kebutuhan")
            .Where(evt => reader.TryReadNeedPurchase(evt.Payload, out _, out _, out _))
            .Select(evt =>
            {
                reader.TryReadNeedPurchase(evt.Payload, out _, out var cardId, out _);
                var need = config?.Needs.FirstOrDefault(item => item.Id.Equals(cardId, StringComparison.OrdinalIgnoreCase));
                var tier = need is null ? NeedTierClassifier.FromPayloadJson(evt.Payload) : ParseTier(need.Tipe);
                var family = string.IsNullOrWhiteSpace(need?.Family)
                    ? System.Text.RegularExpressions.Regex.Replace(cardId, "_[0-9]+$", "")
                    : need.Family;
                return new { CardId = cardId, Tier = tier, Family = family };
            }).ToList();

        var result = new List<Evaluation>();
        foreach (var evt in events.Where(evt => evt.ActionType == GameActionCatalog.SetupMisiAwal))
        {
            if (!reader.TryReadMissionAssigned(evt.Payload, out var missionId, out var target,
                    out var penalty, out var requirePrimary, out var requireSecondary))
            {
                continue;
            }

            var configured = config?.CollectionMissions.FirstOrDefault(item =>
                item.Id.Equals(missionId, StringComparison.OrdinalIgnoreCase));
            var requirements = configured?.KebutuhanTarget;
            if (requirements is null)
            {
                using var payload = JsonDocument.Parse(evt.Payload);
                if (payload.RootElement.TryGetProperty("requirements", out var requirementsJson) &&
                    requirementsJson.ValueKind == JsonValueKind.Array)
                {
                    requirements = requirementsJson.Deserialize<List<RulesetCollectionMissionRequirementDto>>();
                }
            }

            if (requirements is not null)
            {
                bool Matches(RulesetCollectionMissionRequirementDto requirement, string cardId, NeedTier tier, string family)
                    => requirement.Type.ToUpperInvariant() switch
                    {
                        "ASSET" or "NAME" => cardId.Equals(requirement.Value, StringComparison.OrdinalIgnoreCase),
                        "TIER" or "NEED_TIER" => tier != NeedTier.Unknown && tier == ParseTier(requirement.Value),
                        "FAMILY" or "NEED_FAMILY" => family.Equals(requirement.Value, StringComparison.OrdinalIgnoreCase),
                        _ => false
                    };

                var complete = requirements.All(requirement => purchases.Any(need =>
                    Matches(requirement, need.CardId, need.Tier, need.Family)));
                var tertiaryAcquired = requirements.Any(requirement =>
                    purchases.Any(need => need.Tier == NeedTier.Tertiary &&
                        Matches(requirement, need.CardId, need.Tier, need.Family)));
                result.Add(new Evaluation(complete, tertiaryAcquired,
                    configured is null ? penalty : ResolvePenalty(configured), configured?.SuccessPoints ?? 0));
                continue;
            }

            // Older event-only callers retain the explicit legacy target; display names are never inferred.
            var hasTarget = purchases.Any(need => need.Tier == NeedTier.Tertiary &&
                System.Text.RegularExpressions.Regex.Replace(need.CardId, "_[0-9]+$", "")
                    .Equals(target, StringComparison.OrdinalIgnoreCase));
            result.Add(new Evaluation(
                (string.IsNullOrWhiteSpace(target) || hasTarget) &&
                (!requirePrimary || purchases.Any(need => need.Tier == NeedTier.Primary)) &&
                (!requireSecondary || purchases.Any(need => need.Tier == NeedTier.Secondary)),
                !string.IsNullOrWhiteSpace(target) && hasTarget,
                penalty));
        }

        return result;
    }

    private static NeedTier ParseTier(string value) => value.Trim().ToLowerInvariant() switch
    {
        "primary" or "primer" => NeedTier.Primary,
        "secondary" or "sekunder" => NeedTier.Secondary,
        "tertiary" or "tersier" => NeedTier.Tertiary,
        _ => NeedTier.Unknown
    };

    // penaltyPoints is the legacy positive alias; failure_points is its negative form.
    internal static int ResolvePenalty(RulesetCollectionMissionDto mission)
        => mission.PenaltyPoints != 0 ? mission.PenaltyPoints : -mission.FailurePoints;
}
