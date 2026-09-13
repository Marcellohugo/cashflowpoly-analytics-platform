// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui AnalyticsCashTimelineCalculator.
// Mengimpor namespace `System.Text.Json.Serialization` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.Json.Serialization;
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Domain;

public sealed record AnalyticsTurnAmount(
    // Parameter `DayIndex` bertipe `int` membawa nilai hari index; memetakan nama properti JSON menjadi (”day_index”).
    [property: JsonPropertyName("day_index")] int DayIndex,
    // Parameter `ActionSlot` bertipe `int` membawa nilai aksi slot; memetakan nama properti JSON menjadi (”action_slot”).
    [property: JsonPropertyName("action_slot")] int ActionSlot,
    // Parameter `SequenceNumber` bertipe `long` membawa nomor urut event yang menentukan urutan pemrosesan riwayat permainan; memetakan nama properti
    // JSON menjadi (”sequence_number”).
    [property: JsonPropertyName("sequence_number")] long SequenceNumber,
    // Parameter `CashflowCategory` bertipe `string` membawa nilai arus kas category; memetakan nama properti JSON menjadi (”cashflow_category”).
    [property: JsonPropertyName("cashflow_category")] string CashflowCategory,
    // Parameter `Amount` bertipe `double` membawa nominal uang atau nilai transaksi yang dipakai dalam operasi; memetakan nama properti JSON menjadi
    // (”amount”).
    [property: JsonPropertyName("amount")] double Amount);

public sealed record AnalyticsTurnNet(
    // Parameter `DayIndex` bertipe `int` membawa nilai hari index; memetakan nama properti JSON menjadi (”day_index”).
    [property: JsonPropertyName("day_index")] int DayIndex,
    // Parameter `ActionSlot` bertipe `int` membawa nilai aksi slot; memetakan nama properti JSON menjadi (”action_slot”).
    [property: JsonPropertyName("action_slot")] int ActionSlot,
    // Parameter `SequenceNumber` bertipe `long` membawa nomor urut event yang menentukan urutan pemrosesan riwayat permainan; memetakan nama properti
    // JSON menjadi (”sequence_number”).
    [property: JsonPropertyName("sequence_number")] long SequenceNumber,
    // Parameter `CashflowCategory` bertipe `string` membawa nilai arus kas category; memetakan nama properti JSON menjadi (”cashflow_category”).
    [property: JsonPropertyName("cashflow_category")] string CashflowCategory,
    // Parameter `Net` bertipe `double` membawa nilai net; memetakan nama properti JSON menjadi (”net”).
    [property: JsonPropertyName("net")] double Net);

public sealed record AnalyticsTurnCoins(
    // Parameter `DayIndex` bertipe `int` membawa nilai hari index; memetakan nama properti JSON menjadi (”day_index”).
    [property: JsonPropertyName("day_index")] int DayIndex,
    // Parameter `ActionSlot` bertipe `int` membawa nilai aksi slot; memetakan nama properti JSON menjadi (”action_slot”).
    [property: JsonPropertyName("action_slot")] int ActionSlot,
    // Parameter `SequenceNumber` bertipe `long` membawa nomor urut event yang menentukan urutan pemrosesan riwayat permainan; memetakan nama properti
    // JSON menjadi (”sequence_number”).
    [property: JsonPropertyName("sequence_number")] long SequenceNumber,
    // Parameter `CashflowCategory` bertipe `string` membawa nilai arus kas category; memetakan nama properti JSON menjadi (”cashflow_category”).
    [property: JsonPropertyName("cashflow_category")] string CashflowCategory,
    // Parameter `Coins` bertipe `double` membawa nilai coins; memetakan nama properti JSON menjadi (”coins”).
    [property: JsonPropertyName("coins")] double Coins);

public sealed record AnalyticsCashTimeline(
    // Parameter `StartingCoins` bertipe `int` membawa nilai starting coins.
    int StartingCoins,
    // Parameter `CashInTotal` bertipe `double` membawa jumlah seluruh pemasukan arus kas.
    double CashInTotal,
    // Parameter `CashOutTotal` bertipe `double` membawa jumlah seluruh pengeluaran arus kas.
    double CashOutTotal,
    // Parameter `CoinsNetEndGame` bertipe `double` membawa nilai coins net end game.
    double CoinsNetEndGame,
    // Parameter `CoinsHeldCurrent` bertipe `double` membawa nilai coins held saat ini.
    double CoinsHeldCurrent,
    // Parameter `CoinsSpentPerTurn` bertipe `IReadOnlyList<AnalyticsTurnAmount>` membawa nilai coins spent per giliran.
    IReadOnlyList<AnalyticsTurnAmount> CoinsSpentPerTurn,
    // Parameter `CoinsEarnedPerTurn` bertipe `IReadOnlyList<AnalyticsTurnAmount>` membawa nilai coins earned per giliran.
    IReadOnlyList<AnalyticsTurnAmount> CoinsEarnedPerTurn,
    // Parameter `NetIncomePerTurn` bertipe `IReadOnlyList<AnalyticsTurnNet>` membawa nilai net pemasukan per giliran.
    IReadOnlyList<AnalyticsTurnNet> NetIncomePerTurn,
    // Parameter `CoinsProgression` bertipe `IReadOnlyList<AnalyticsTurnCoins>` membawa nilai coins progression.
    IReadOnlyList<AnalyticsTurnCoins> CoinsProgression);

internal sealed class CashTimelineCalculator : ICashTimelineCalculator
{
    public AnalyticsCashTimeline Compute(
        // Parameter `sessionEvents` bertipe `IReadOnlyCollection<EventDb>` membawa nilai sesi event.
        IReadOnlyCollection<EventDb> sessionEvents,
        // Parameter `playerProjections` bertipe `IReadOnlyCollection<CashflowProjectionDb>` membawa nilai pemain projections.
        IReadOnlyCollection<CashflowProjectionDb> playerProjections,
        // Parameter `startingCoins` bertipe `int` membawa nilai starting coins.
        int startingCoins)
    {
        var eventIds = sessionEvents.Select(item => item.EventId).ToHashSet();
        var reconciledProjections = playerProjections
            .Where(projection => eventIds.Contains(projection.EventId))
            .ToArray();
        var cashInTotal = reconciledProjections.Where(p => p.Direction == "IN").Sum(p => (double)p.Amount);
        var cashOutTotal = reconciledProjections.Where(p => p.Direction == "OUT").Sum(p => (double)p.Amount);
        var coinsNetEndGame = startingCoins + cashInTotal - cashOutTotal;

        var projectionsByEvent = reconciledProjections
            .GroupBy(projection => projection.EventId)
            .ToDictionary(group => group.Key, group => group.ToList());
        var coinsSpentPerTurn = new List<AnalyticsTurnAmount>();
        var coinsEarnedPerTurn = new List<AnalyticsTurnAmount>();
        var netIncomePerTurn = new List<AnalyticsTurnNet>();
        var coinsProgression = new List<AnalyticsTurnCoins>();
        var runningCoins = (double)startingCoins;
        // Mengulangi setiap elemen `sessionEvents.OrderBy(item => item.SequenceNumber)`; elemen saat ini disimpan sebagai `evt` bertipe `var` untuk
        // diproses oleh badan loop dalam Compute.
        foreach (var evt in sessionEvents.OrderBy(item => item.SequenceNumber))
        {
            if (!projectionsByEvent.TryGetValue(evt.EventId, out var eventProjections))
            {
                // Melewati sisa pernyataan pada iterasi saat ini dan melanjutkan ke elemen/iterasi berikutnya dalam Compute.
                continue;
            }

            var spent = eventProjections
                .Where(projection => string.Equals(projection.Direction, "OUT", StringComparison.OrdinalIgnoreCase))
                .Sum(projection => (double)projection.Amount);
            var earned = eventProjections
                .Where(projection => string.Equals(projection.Direction, "IN", StringComparison.OrdinalIgnoreCase))
                .Sum(projection => (double)projection.Amount);
            var cashflowCategory = string.Join(
                '|',
                eventProjections
                    .Select(projection => string.Equals(evt.ActionType, GameActionCatalog.SetupBahanAwal, StringComparison.OrdinalIgnoreCase)
                        && string.Equals(projection.Category, "INGREDIENT", StringComparison.OrdinalIgnoreCase)
                            ? "INITIAL_INGREDIENT"
                            : projection.Category)
                    .Where(category => !string.IsNullOrWhiteSpace(category))
                    .Distinct(StringComparer.OrdinalIgnoreCase));

            if (spent > 0)
            {
                coinsSpentPerTurn.Add(new AnalyticsTurnAmount(evt.DayIndex, evt.ActionSlot, evt.SequenceNumber, cashflowCategory, spent));
            }

            if (earned > 0)
            {
                coinsEarnedPerTurn.Add(new AnalyticsTurnAmount(evt.DayIndex, evt.ActionSlot, evt.SequenceNumber, cashflowCategory, earned));
            }

            var net = earned - spent;
            runningCoins += net;
            netIncomePerTurn.Add(new AnalyticsTurnNet(evt.DayIndex, evt.ActionSlot, evt.SequenceNumber, cashflowCategory, net));
            coinsProgression.Add(new AnalyticsTurnCoins(evt.DayIndex, evt.ActionSlot, evt.SequenceNumber, cashflowCategory, runningCoins));
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
