// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui AnalyticsDonationGameplayCalculator.
// Mengimpor namespace `System.Text.Json.Serialization` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.Json.Serialization;
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;
// Mengimpor namespace `Cashflowpoly.Api.Domain.AnalyticsMath` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using static Cashflowpoly.Api.Domain.AnalyticsMath;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Domain;

public sealed record AnalyticsDonationAmountByDay(
    // Parameter `DayIndex` bertipe `int` membawa nilai hari index; memetakan nama properti JSON menjadi (”day_index”).
    [property: JsonPropertyName("day_index")] int DayIndex,
    // Parameter `Amount` bertipe `double` membawa nominal uang atau nilai transaksi yang dipakai dalam operasi; memetakan nama properti JSON menjadi
    // (”amount”).
    [property: JsonPropertyName("amount")] double Amount);

public sealed record AnalyticsDonationRankByDay(
    // Parameter `DayIndex` bertipe `int` membawa nilai hari index; memetakan nama properti JSON menjadi (”day_index”).
    [property: JsonPropertyName("day_index")] int DayIndex,
    // Parameter `Rank` bertipe `int?` membawa nilai rank; nilai null diizinkan ketika data opsional belum tersedia; memetakan nama properti JSON
    // menjadi (”rank”).
    [property: JsonPropertyName("rank")] int? Rank);

public sealed record AnalyticsDonationGameplayMetrics(
    // Parameter `DonationAmountPerFriday` bertipe `IReadOnlyList<AnalyticsDonationAmountByDay>` membawa nilai donasi nominal per friday.
    IReadOnlyList<AnalyticsDonationAmountByDay> DonationAmountPerFriday,
    // Parameter `DonationRankPerFriday` bertipe `IReadOnlyList<AnalyticsDonationRankByDay>` membawa nilai donasi rank per friday.
    IReadOnlyList<AnalyticsDonationRankByDay> DonationRankPerFriday,
    // Parameter `DonationTotalCoins` bertipe `double` membawa nilai donasi total coins.
    double DonationTotalCoins,
    // Parameter `DonationChampionCardsEarned` bertipe `int` membawa nilai donasi champion kartu earned.
    int DonationChampionCardsEarned,
    // Parameter `DonationStabilityStdDeviation` bertipe `double?` membawa nilai donasi stability std deviation; nilai null diizinkan ketika data
    // opsional belum tersedia.
    double? DonationStabilityStdDeviation,
    // Parameter `DonationStability` bertipe `double?` membawa nilai donasi stability; nilai null diizinkan ketika data opsional belum tersedia.
    double? DonationStability,
    // Parameter `DonationStabilityIndex` bertipe `double?` membawa nilai donasi stability index; nilai null diizinkan ketika data opsional belum
    // tersedia.
    double? DonationStabilityIndex,
    // Parameter `DonationRatio` bertipe `double?` membawa nilai donasi ratio; nilai null diizinkan ketika data opsional belum tersedia.
    double? DonationRatio,
    // Parameter `DonationAggressivenessPercent` bertipe `double?` membawa nilai donasi aggressiveness percent; nilai null diizinkan ketika data
    // opsional belum tersedia.
    double? DonationAggressivenessPercent,
    // Parameter `FridayParticipationRate` bertipe `double?` membawa nilai friday participation rate; nilai null diizinkan ketika data opsional belum
    // tersedia.
    double? FridayParticipationRate,
    // Parameter `DonationCommitmentScore` bertipe `double?` membawa nilai donasi commitment skor; nilai null diizinkan ketika data opsional belum
    // tersedia.
    double? DonationCommitmentScore);

internal sealed class DonationGameplayCalculator : IDonationGameplayCalculator
{
    private static readonly AnalyticsPayloadReader _payloadReader = new();

    public AnalyticsDonationGameplayMetrics Compute(
        // Parameter `playerEvents` bertipe `IEnumerable<EventDb>` membawa nilai pemain event.
        IEnumerable<EventDb> playerEvents,
        // Parameter `allEvents` bertipe `IEnumerable<EventDb>` membawa nilai all event.
        IEnumerable<EventDb> allEvents,
        // Parameter `coinsNetEndGame` bertipe `double` membawa nilai coins net end game.
        double coinsNetEndGame)
    {
        var playerEventsList = playerEvents.ToList();
        var allEventsList = allEvents.ToList();
        var donationByDay = playerEventsList
            .Where(e => e.ActionType == "JumatBerkah")
            .GroupBy(e => e.DayIndex)
            .Select(g => new AnalyticsDonationAmountByDay(
                g.Key,
                g.Sum(e => _payloadReader.TryReadAmount(e.Payload, out var amount) ? amount : 0)))
            .OrderBy(item => item.DayIndex)
            .ToList();
        var donationTotal = donationByDay.Sum(item => item.Amount);

        var playerId = playerEventsList.FirstOrDefault(e => e.UserId.HasValue)?.UserId;
        var awardedRanksByDay = playerEventsList
            .Where(e => e.ActionType == GameActionCatalog.DonationRankAwarded)
            .Select(e => new
            {
                e.DayIndex,
                Rank = _payloadReader.TryReadRankAwarded(e.Payload, out var rank, out _) ? rank : 0
            })
            .Where(item => item.Rank > 0)
            .GroupBy(item => item.DayIndex)
            .ToDictionary(group => group.Key, group => group.First().Rank);
        var tieBreakers = allEventsList
            .Where(e => e.UserId.HasValue && e.ActionType == GameActionCatalog.TieBreakerAssigned)
            .GroupBy(e => e.UserId!.Value)
            .ToDictionary(
                // Parameter `group` bertipe `` membawa nilai group.
                group => group.Key,
                // Parameter `group` bertipe `` membawa nilai group.
                group => _payloadReader.TryReadTieBreaker(group.OrderBy(e => e.SequenceNumber).Last().Payload, out var number)
                    ? number
                    : 0);
        var donationRanks = allEventsList
            .Where(e => e.UserId.HasValue && e.ActionType == "JumatBerkah")
            .GroupBy(e => e.DayIndex)
            .Select(group => new
            {
                DayIndex = group.Key,
                RankedPlayers = group
                    .GroupBy(e => e.UserId!.Value)
                    .Select(playerGroup => new
                    {
                        PlayerId = playerGroup.Key,
                        Amount = playerGroup.Sum(e => _payloadReader.TryReadAmount(e.Payload, out var amount) ? amount : 0),
                        TieBreaker = tieBreakers.GetValueOrDefault(playerGroup.Key)
                    })
                    .OrderByDescending(item => item.Amount)
                    .ThenByDescending(item => item.TieBreaker)
                    .ToList()
            })
            .Where(group => playerId.HasValue && group.RankedPlayers.Any(item => item.PlayerId == playerId.Value))
            .Select(group => new AnalyticsDonationRankByDay(
                group.DayIndex,
                awardedRanksByDay.GetValueOrDefault(
                    group.DayIndex,
                    group.RankedPlayers.FindIndex(item => item.PlayerId == playerId!.Value) + 1)))
            .OrderBy(item => item.DayIndex)
            .ToList();

        var donationAmounts = donationByDay.Select(d => d.Amount).ToList();
        var donationStabilityStdDeviation = donationAmounts.Count > 0 ? StdDev(donationAmounts) : (double?)null;
        var averageDonation = donationAmounts.Count > 0 ? donationAmounts.Average() : 0;
        var donationStability = donationStabilityStdDeviation.HasValue
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: Clamp(100 - donationStabilityStdDeviation.Value, 0, 100) dalam
            // Compute.
            ? Clamp(100 - donationStabilityStdDeviation.Value, 0, 100)
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: (double?)null; dalam Compute.
            : (double?)null;
        var donationStabilityIndex = averageDonation > 0
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: Clamp((1 - (StdDev(donationAmounts) / averageDonation)) * 100, 0,
            // 100) dalam Compute.
            ? Clamp((1 - (StdDev(donationAmounts) / averageDonation)) * 100, 0, 100)
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: (double?)null; dalam Compute.
            : (double?)null;
        var donationRatio = SafeRatio(donationTotal, coinsNetEndGame);
        var donationAggressivenessPercent = SafeRatio(donationTotal, coinsNetEndGame, true);
        var totalFridays = allEventsList
            .Where(e => string.Equals(e.Weekday, "FRI", StringComparison.OrdinalIgnoreCase))
            .Select(e => e.DayIndex)
            .Distinct()
            .Count();
        var fridayParticipationRate = totalFridays > 0 ? (double)donationByDay.Count / totalFridays : (double?)null;
        var donationCommitmentScore =
            donationStabilityIndex.HasValue &&
            donationRatio.HasValue &&
            fridayParticipationRate.HasValue
                // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: Clamp(donationStabilityIndex.Value * donationRatio.Value *
                // fridayParticipationRate.Value, 0, 100) dalam Compute.
                ? Clamp(donationStabilityIndex.Value * donationRatio.Value * fridayParticipationRate.Value, 0, 100)
                // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: (double?)null; dalam Compute.
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
