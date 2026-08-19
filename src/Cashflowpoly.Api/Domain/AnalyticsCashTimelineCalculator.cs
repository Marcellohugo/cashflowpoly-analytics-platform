// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui AnalyticsCashTimelineCalculator.
using System.Text.Json.Serialization;
using Cashflowpoly.Api.Data;

namespace Cashflowpoly.Api.Domain;

public sealed record AnalyticsTurnAmount(
    [property: JsonPropertyName("day_index")] int DayIndex,
    [property: JsonPropertyName("action_slot")] int ActionSlot,
    [property: JsonPropertyName("sequence_number")] long SequenceNumber,
    [property: JsonPropertyName("amount")] double Amount);

public sealed record AnalyticsTurnNet(
    [property: JsonPropertyName("day_index")] int DayIndex,
    [property: JsonPropertyName("action_slot")] int ActionSlot,
    [property: JsonPropertyName("sequence_number")] long SequenceNumber,
    [property: JsonPropertyName("net")] double Net);

public sealed record AnalyticsTurnCoins(
    [property: JsonPropertyName("day_index")] int DayIndex,
    [property: JsonPropertyName("action_slot")] int ActionSlot,
    [property: JsonPropertyName("sequence_number")] long SequenceNumber,
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

internal sealed class CashTimelineCalculator : ICashTimelineCalculator
{
    public AnalyticsCashTimeline Compute(
        IReadOnlyCollection<EventDb> playerEvents,
        IReadOnlyCollection<CashflowProjectionDb> playerProjections,
        int startingCoins)
    {
        var cashInTotal = playerProjections.Where(p => p.Direction == "IN").Sum(p => (double)p.Amount);
        var cashOutTotal = playerProjections.Where(p => p.Direction == "OUT").Sum(p => (double)p.Amount);
        var coinsNetEndGame = startingCoins + cashInTotal - cashOutTotal;

        var projectionsByEvent = playerProjections
            .GroupBy(projection => projection.EventId)
            .ToDictionary(group => group.Key, group => group.ToList());
        var coinsSpentPerTurn = new List<AnalyticsTurnAmount>();
        var coinsEarnedPerTurn = new List<AnalyticsTurnAmount>();
        var netIncomePerTurn = new List<AnalyticsTurnNet>();
        var coinsProgression = new List<AnalyticsTurnCoins>();
        var runningCoins = (double)startingCoins;
        foreach (var evt in playerEvents.OrderBy(item => item.SequenceNumber))
        {
            if (!projectionsByEvent.TryGetValue(evt.EventId, out var eventProjections))
            {
                continue;
            }

            var spent = eventProjections
                .Where(projection => string.Equals(projection.Direction, "OUT", StringComparison.OrdinalIgnoreCase))
                .Sum(projection => (double)projection.Amount);
            var earned = eventProjections
                .Where(projection => string.Equals(projection.Direction, "IN", StringComparison.OrdinalIgnoreCase))
                .Sum(projection => (double)projection.Amount);

            if (spent > 0)
            {
                coinsSpentPerTurn.Add(new AnalyticsTurnAmount(evt.DayIndex, evt.ActionSlot, evt.SequenceNumber, spent));
            }

            if (earned > 0)
            {
                coinsEarnedPerTurn.Add(new AnalyticsTurnAmount(evt.DayIndex, evt.ActionSlot, evt.SequenceNumber, earned));
            }

            var net = earned - spent;
            runningCoins += net;
            netIncomePerTurn.Add(new AnalyticsTurnNet(evt.DayIndex, evt.ActionSlot, evt.SequenceNumber, net));
            coinsProgression.Add(new AnalyticsTurnCoins(evt.DayIndex, evt.ActionSlot, evt.SequenceNumber, runningCoins));
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
