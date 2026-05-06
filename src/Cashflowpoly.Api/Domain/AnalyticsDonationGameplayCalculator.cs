// Fungsi file: Menghitung metrik donasi gameplay mentah dan turunan dari histori event.
using System.Text.Json.Serialization;
using Cashflowpoly.Api.Data;
using static Cashflowpoly.Api.Domain.AnalyticsMath;
using static Cashflowpoly.Api.Domain.AnalyticsPayloadReader;

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
    double? DonationRatio,
    double? DonationAggressivenessPercent,
    double? FridayParticipationRate,
    double? DonationCommitmentScore);

internal static class AnalyticsDonationGameplayCalculator
{
    internal static AnalyticsDonationGameplayMetrics Compute(
        IEnumerable<EventDb> playerEvents,
        IEnumerable<EventDb> allEvents,
        double coinsNetEndGame)
    {
        var playerEventsList = playerEvents.ToList();
        var donationByDay = playerEventsList
            .Where(e => e.ActionType == "day.friday.donation")
            .GroupBy(e => e.DayIndex)
            .Select(g => new AnalyticsDonationAmountByDay(
                g.Key,
                g.Sum(e => TryReadAmount(e.Payload, out var amount) ? amount : 0)))
            .OrderBy(item => item.DayIndex)
            .ToList();
        var donationTotal = donationByDay.Sum(item => item.Amount);

        var donationRanks = playerEventsList
            .Where(e => e.ActionType == "donation.rank.awarded")
            .GroupBy(e => e.DayIndex)
            .Select(g =>
            {
                var rank = 0;
                foreach (var evt in g)
                {
                    if (TryReadRankAwarded(evt.Payload, out var awardedRank, out _))
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
        var donationStability = donationAmounts.Count > 0 ? 100 - StdDev(donationAmounts) : (double?)null;
        var donationRatio = SafeRatio(donationTotal, coinsNetEndGame);
        var donationAggressivenessPercent = SafeRatio(donationTotal, coinsNetEndGame, true);
        var totalFridays = allEvents
            .Where(e => string.Equals(e.Weekday, "FRI", StringComparison.OrdinalIgnoreCase))
            .Select(e => e.DayIndex)
            .Distinct()
            .Count();
        var fridayParticipationRate = totalFridays > 0 ? (double)donationByDay.Count / totalFridays : (double?)null;
        var donationCommitmentScore =
            donationStability.HasValue &&
            donationRatio.HasValue &&
            fridayParticipationRate.HasValue
                ? donationStability.Value * donationRatio.Value * fridayParticipationRate.Value
                : (double?)null;

        return new AnalyticsDonationGameplayMetrics(
            donationByDay,
            donationRanks,
            donationTotal,
            playerEventsList.Count(e => e.ActionType == "donation.rank.awarded"),
            donationStabilityStdDeviation,
            donationStability,
            donationRatio,
            donationAggressivenessPercent,
            fridayParticipationRate,
            donationCommitmentScore);
    }
}
