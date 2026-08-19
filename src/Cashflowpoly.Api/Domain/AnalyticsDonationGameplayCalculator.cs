// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui AnalyticsDonationGameplayCalculator.
using System.Text.Json.Serialization;
using Cashflowpoly.Api.Data;
using static Cashflowpoly.Api.Domain.AnalyticsMath;

namespace Cashflowpoly.Api.Domain;

public sealed record AnalyticsDonationAmountByDay(
    [property: JsonPropertyName("day_index")] int DayIndex,
    [property: JsonPropertyName("amount")] double Amount);

public sealed record AnalyticsDonationRankByDay(
    [property: JsonPropertyName("day_index")] int DayIndex,
    [property: JsonPropertyName("rank")] int? Rank);

public sealed record AnalyticsDonationGameplayMetrics(
    IReadOnlyList<AnalyticsDonationAmountByDay> DonationAmountPerFriday,
    IReadOnlyList<AnalyticsDonationRankByDay> DonationRankPerFriday,
    double DonationTotalCoins,
    int DonationChampionCardsEarned,
    double? DonationStabilityStdDeviation,
    double? DonationStability,
    double? DonationStabilityIndex,
    double? DonationRatio,
    double? DonationAggressivenessPercent,
    double? FridayParticipationRate,
    double? DonationCommitmentScore);

internal sealed class DonationGameplayCalculator : IDonationGameplayCalculator
{
    private static readonly AnalyticsPayloadReader _payloadReader = new();

    public AnalyticsDonationGameplayMetrics Compute(
        IEnumerable<EventDb> playerEvents,
        IEnumerable<EventDb> allEvents,
        double coinsNetEndGame)
    {
        var playerEventsList = playerEvents.ToList();
        var donationByDay = playerEventsList
            .Where(e => e.ActionType == "JumatBerkah")
            .GroupBy(e => e.DayIndex)
            .Select(g => new AnalyticsDonationAmountByDay(
                g.Key,
                g.Sum(e => _payloadReader.TryReadAmount(e.Payload, out var amount) ? amount : 0)))
            .OrderBy(item => item.DayIndex)
            .ToList();
        var donationTotal = donationByDay.Sum(item => item.Amount);

        var donationRanks = playerEventsList
            .Where(e => e.ActionType == "PoinPeringkatDonasi")
            .GroupBy(e => e.DayIndex)
            .Select(g =>
            {
                var rank = 0;
                foreach (var evt in g)
                {
                    if (_payloadReader.TryReadRankAwarded(evt.Payload, out var awardedRank, out _))
                    {
                        rank = awardedRank;
                        break;
                    }
                }

                return new AnalyticsDonationRankByDay(g.Key, rank == 0 ? null : rank);
            })
            .OrderBy(item => item.DayIndex)
            .ToList();

        var donationAmounts = donationByDay.Select(d => d.Amount).ToList();
        var donationStabilityStdDeviation = donationAmounts.Count > 0 ? StdDev(donationAmounts) : (double?)null;
        var averageDonation = donationAmounts.Count > 0 ? donationAmounts.Average() : 0;
        var donationStability = donationStabilityStdDeviation.HasValue
            ? Clamp(100 - donationStabilityStdDeviation.Value, 0, 100)
            : (double?)null;
        var donationStabilityIndex = averageDonation > 0
            ? Clamp((1 - (StdDev(donationAmounts) / averageDonation)) * 100, 0, 100)
            : (double?)null;
        var donationRatio = SafeRatio(donationTotal, coinsNetEndGame);
        var donationAggressivenessPercent = SafeRatio(donationTotal, coinsNetEndGame, true);
        var totalFridays = allEvents
            .Where(e => string.Equals(e.Weekday, "FRI", StringComparison.OrdinalIgnoreCase))
            .Select(e => e.DayIndex)
            .Distinct()
            .Count();
        var fridayParticipationRate = totalFridays > 0 ? (double)donationByDay.Count / totalFridays : (double?)null;
        var donationCommitmentScore =
            donationStabilityIndex.HasValue &&
            donationRatio.HasValue &&
            fridayParticipationRate.HasValue
                ? Clamp(donationStabilityIndex.Value * donationRatio.Value * fridayParticipationRate.Value, 0, 100)
                : (double?)null;

        return new AnalyticsDonationGameplayMetrics(
            donationByDay,
            donationRanks,
            donationTotal,
            playerEventsList.Count(e => e.ActionType == "PoinPeringkatDonasi"),
            donationStabilityStdDeviation,
            donationStability,
            donationStabilityIndex,
            donationRatio,
            donationAggressivenessPercent,
            fridayParticipationRate,
            donationCommitmentScore);
    }
}
