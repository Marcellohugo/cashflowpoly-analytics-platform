// Fungsi file: Menghitung timeline kas gameplay dari proyeksi cashflow dan turn event.
using System.Text.Json.Serialization;
using Cashflowpoly.Api.Data;

namespace Cashflowpoly.Api.Domain;

public sealed record AnalyticsTurnAmount(
    [property: JsonPropertyName("turn_number")] int TurnNumber,
    [property: JsonPropertyName("amount")] double Amount);

public sealed record AnalyticsTurnNet(
    [property: JsonPropertyName("turn_number")] int TurnNumber,
    [property: JsonPropertyName("net")] double Net);

public sealed record AnalyticsTurnCoins(
    [property: JsonPropertyName("turn_number")] int TurnNumber,
    [property: JsonPropertyName("coins")] double Coins);

public sealed record AnalyticsCashTimeline(
    int StartingCoins,
    double CashInTotal,
    double CashOutTotal,
    double CoinsNetEndGame,
    double CoinsHeldCurrent,
    IReadOnlyList<AnalyticsTurnAmount> CoinsSpentPerTurn,
    IReadOnlyList<AnalyticsTurnAmount> CoinsEarnedPerTurn,
    IReadOnlyList<AnalyticsTurnNet> NetIncomePerTurn,
    IReadOnlyList<AnalyticsTurnCoins> CoinsProgression);

internal static class AnalyticsCashTimelineCalculator
{
    internal static AnalyticsCashTimeline Compute(
        IReadOnlyCollection<EventDb> playerEvents,
        IReadOnlyCollection<CashflowProjectionDb> playerProjections,
        int startingCoins)
    {
        var cashInTotal = playerProjections.Where(p => p.Direction == "IN").Sum(p => (double)p.Amount);
        var cashOutTotal = playerProjections.Where(p => p.Direction == "OUT").Sum(p => (double)p.Amount);
        var coinsNetEndGame = startingCoins + cashInTotal - cashOutTotal;

        var eventById = playerEvents
            .GroupBy(e => e.EventId)
            .ToDictionary(g => g.Key, g => g.First());

        var spentByTurn = new Dictionary<int, double>();
        var earnedByTurn = new Dictionary<int, double>();

        foreach (var projection in playerProjections)
        {
            if (!eventById.TryGetValue(projection.EventId, out var evt))
            {
                continue;
            }

            var turn = evt.TurnNumber;
            if (string.Equals(projection.Direction, "OUT", StringComparison.OrdinalIgnoreCase))
            {
                spentByTurn[turn] = spentByTurn.TryGetValue(turn, out var existing)
                    ? existing + projection.Amount
                    : projection.Amount;
            }
            else if (string.Equals(projection.Direction, "IN", StringComparison.OrdinalIgnoreCase))
            {
                earnedByTurn[turn] = earnedByTurn.TryGetValue(turn, out var existing)
                    ? existing + projection.Amount
                    : projection.Amount;
            }
        }

        var coinsSpentPerTurn = spentByTurn
            .OrderBy(k => k.Key)
            .Select(k => new AnalyticsTurnAmount(k.Key, k.Value))
            .ToList();
        var coinsEarnedPerTurn = earnedByTurn
            .OrderBy(k => k.Key)
            .Select(k => new AnalyticsTurnAmount(k.Key, k.Value))
            .ToList();

        var netIncomePerTurn = new List<AnalyticsTurnNet>();
        var coinsProgression = new List<AnalyticsTurnCoins>();
        var runningCoins = (double)startingCoins;
        foreach (var turn in spentByTurn.Keys.Union(earnedByTurn.Keys).OrderBy(t => t))
        {
            var spent = spentByTurn.TryGetValue(turn, out var spentAmount) ? spentAmount : 0;
            var earned = earnedByTurn.TryGetValue(turn, out var earnedAmount) ? earnedAmount : 0;
            var net = earned - spent;
            runningCoins += net;
            netIncomePerTurn.Add(new AnalyticsTurnNet(turn, net));
            coinsProgression.Add(new AnalyticsTurnCoins(turn, runningCoins));
        }

        return new AnalyticsCashTimeline(
            startingCoins,
            cashInTotal,
            cashOutTotal,
            coinsNetEndGame,
            coinsNetEndGame,
            coinsSpentPerTurn,
            coinsEarnedPerTurn,
            netIncomePerTurn,
            coinsProgression);
    }
}
