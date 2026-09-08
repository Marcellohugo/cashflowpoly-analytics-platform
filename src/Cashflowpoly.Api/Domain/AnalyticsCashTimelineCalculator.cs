// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui AnalyticsCashTimelineCalculator.
// Mengimpor namespace `System.Text.Json.Serialization` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.Json.Serialization;
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Domain;

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `AnalyticsTurnAmount`; sealed mencegah tipe ini diturunkan lagi.
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

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `AnalyticsTurnNet`; sealed mencegah tipe ini diturunkan lagi.
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

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `AnalyticsTurnCoins`; sealed mencegah tipe ini diturunkan lagi.
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

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `AnalyticsCashTimeline`; sealed mencegah tipe ini diturunkan lagi.
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

// Mendefinisikan tipe class `CashTimelineCalculator` yang mewarisi atau menerapkan `ICashTimelineCalculator`; sealed mencegah tipe ini diturunkan
// lagi.
internal sealed class CashTimelineCalculator : ICashTimelineCalculator
// Membuka scope tipe CashTimelineCalculator; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan metode `Compute` dengan hasil bertipe `AnalyticsCashTimeline`; operasi ini menangani compute. Masukan: Parameter `sessionEvents`
    // bertipe `IReadOnlyCollection<EventDb>` membawa nilai sesi event; Parameter `playerProjections` bertipe
    // `IReadOnlyCollection<CashflowProjectionDb>` membawa nilai pemain projections; Parameter `startingCoins` bertipe `int` membawa nilai starting
    // coins.
    public AnalyticsCashTimeline Compute(
        // Parameter `sessionEvents` bertipe `IReadOnlyCollection<EventDb>` membawa nilai sesi event.
        IReadOnlyCollection<EventDb> sessionEvents,
        // Parameter `playerProjections` bertipe `IReadOnlyCollection<CashflowProjectionDb>` membawa nilai pemain projections.
        IReadOnlyCollection<CashflowProjectionDb> playerProjections,
        // Parameter `startingCoins` bertipe `int` membawa nilai starting coins.
        int startingCoins)
    // Membuka scope metode Compute; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Compute.
    {
        // Menyiapkan variabel lokal `eventIds` untuk nilai event identitas dengan membentuk himpunan nilai unik dari `sessionEvents.Select(item =>
        // item.EventId)` memakai tanpa argumen. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var eventIds = sessionEvents.Select(item => item.EventId).ToHashSet();
        // Menyiapkan variabel lokal `reconciledProjections` untuk nilai reconciled projections dengan mematerialisasi urutan `playerProjections
        // .Where(projection => eventIds.Contains(projection.EventId))` menjadi array dengan elemen hasil saat ini. Tipe variabel disimpulkan dari ekspresi
        // nilai awal.
        var reconciledProjections = playerProjections
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(projection => eventIds.Contains(projection.EventId)) dalam Compute;
            // token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Where(projection => eventIds.Contains(projection.EventId))
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToArray(); dalam Compute; token pada baris ini menyambungkan bagian kode
            // sebelum dan sesudahnya.
            .ToArray();
        // Menyiapkan variabel lokal `cashInTotal` untuk jumlah seluruh pemasukan arus kas dengan menjumlahkan nilai `reconciledProjections.Where(p =>
        // p.Direction == ”IN”)` berdasarkan `p => (double)p.Amount`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var cashInTotal = reconciledProjections.Where(p => p.Direction == "IN").Sum(p => (double)p.Amount);
        // Menyiapkan variabel lokal `cashOutTotal` untuk jumlah seluruh pengeluaran arus kas dengan menjumlahkan nilai `reconciledProjections.Where(p =>
        // p.Direction == ”OUT”)` berdasarkan `p => (double)p.Amount`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var cashOutTotal = reconciledProjections.Where(p => p.Direction == "OUT").Sum(p => (double)p.Amount);
        // Menyiapkan variabel lokal `coinsNetEndGame` untuk nilai coins net end game dengan selisih antara `startingCoins + cashInTotal` dan
        // `cashOutTotal`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var coinsNetEndGame = startingCoins + cashInTotal - cashOutTotal;

        // Menyiapkan variabel lokal `projectionsByEvent` untuk nilai projections berdasarkan event dengan membangun kamus dari `reconciledProjections
        // .GroupBy(projection => projection.EventId)` dengan pemilihan kunci/nilai `group => group.Key`, `group => group.ToList()`; kunci harus unik agar
        // konversi berhasil. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var projectionsByEvent = reconciledProjections
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .GroupBy(projection => projection.EventId) dalam Compute; token pada baris ini
            // menyambungkan bagian kode sebelum dan sesudahnya.
            .GroupBy(projection => projection.EventId)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToDictionary(group => group.Key, group => group.ToList()); dalam Compute;
            // token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .ToDictionary(group => group.Key, group => group.ToList());
        // Menyiapkan variabel lokal `coinsSpentPerTurn` untuk nilai coins spent per giliran dengan objek baru bertipe `List<AnalyticsTurnAmount>` dengan
        // nilai awal sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var coinsSpentPerTurn = new List<AnalyticsTurnAmount>();
        // Menyiapkan variabel lokal `coinsEarnedPerTurn` untuk nilai coins earned per giliran dengan objek baru bertipe `List<AnalyticsTurnAmount>` dengan
        // nilai awal sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var coinsEarnedPerTurn = new List<AnalyticsTurnAmount>();
        // Menyiapkan variabel lokal `netIncomePerTurn` untuk nilai net pemasukan per giliran dengan objek baru bertipe `List<AnalyticsTurnNet>` dengan
        // nilai awal sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var netIncomePerTurn = new List<AnalyticsTurnNet>();
        // Menyiapkan variabel lokal `coinsProgression` untuk nilai coins progression dengan objek baru bertipe `List<AnalyticsTurnCoins>` dengan nilai awal
        // sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var coinsProgression = new List<AnalyticsTurnCoins>();
        // Menyiapkan variabel lokal `runningCoins` untuk nilai running coins dengan hasil konversi `startingCoins` menjadi tipe `double`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var runningCoins = (double)startingCoins;
        // Mengulangi setiap elemen `sessionEvents.OrderBy(item => item.SequenceNumber)`; elemen saat ini disimpan sebagai `evt` bertipe `var` untuk
        // diproses oleh badan loop dalam Compute.
        foreach (var evt in sessionEvents.OrderBy(item => item.SequenceNumber))
        // Membuka scope loop setiap evt dari `sessionEvents.OrderBy(item => item.SequenceNumber)`; pernyataan/deklarasi berikut berada di dalam batas blok
        // ini dalam Compute.
        {
            // Memeriksa kebalikan kondisi `projectionsByEvent.TryGetValue(evt.EventId, out var eventProjections)`; blok if hanya dijalankan ketika kondisi ini
            // bernilai benar dalam Compute.
            if (!projectionsByEvent.TryGetValue(evt.EventId, out var eventProjections))
            // Membuka scope cabang if untuk kondisi `!projectionsByEvent.TryGetValue(evt.EventId, out var eventProjections)`; pernyataan/deklarasi berikut
            // berada di dalam batas blok ini dalam Compute.
            {
                // Melewati sisa pernyataan pada iterasi saat ini dan melanjutkan ke elemen/iterasi berikutnya dalam Compute.
                continue;
            // Menutup scope cabang if untuk kondisi `!projectionsByEvent.TryGetValue(evt.EventId, out var eventProjections)`; bagian berikut berada di luar
            // batas blok tersebut dalam Compute.
            }

            // Menyiapkan variabel lokal `spent` untuk nilai spent dengan menjumlahkan nilai `eventProjections .Where(projection =>
            // string.Equals(projection.Direction, ”OUT”, StringComparison.OrdinalIgnoreCase))` berdasarkan `projection => (double)projection.Amount`. Tipe
            // variabel disimpulkan dari ekspresi nilai awal.
            var spent = eventProjections
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(projection => string.Equals(projection.Direction, ”OUT”,
                // StringComparison.OrdinalIgnoreCase)) dalam Compute; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
                .Where(projection => string.Equals(projection.Direction, "OUT", StringComparison.OrdinalIgnoreCase))
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Sum(projection => (double)projection.Amount); dalam Compute; token pada baris
                // ini menyambungkan bagian kode sebelum dan sesudahnya.
                .Sum(projection => (double)projection.Amount);
            // Menyiapkan variabel lokal `earned` untuk nilai earned dengan menjumlahkan nilai `eventProjections .Where(projection =>
            // string.Equals(projection.Direction, ”IN”, StringComparison.OrdinalIgnoreCase))` berdasarkan `projection => (double)projection.Amount`. Tipe
            // variabel disimpulkan dari ekspresi nilai awal.
            var earned = eventProjections
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(projection => string.Equals(projection.Direction, ”IN”,
                // StringComparison.OrdinalIgnoreCase)) dalam Compute; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
                .Where(projection => string.Equals(projection.Direction, "IN", StringComparison.OrdinalIgnoreCase))
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Sum(projection => (double)projection.Amount); dalam Compute; token pada baris
                // ini menyambungkan bagian kode sebelum dan sesudahnya.
                .Sum(projection => (double)projection.Amount);
            // Menyiapkan variabel lokal `cashflowCategory` untuk nilai arus kas category dengan memanggil `string.Join` dengan `'|'`, `eventProjections
            // .Select(projection => projection.Category) .Where(category => !string.IsNullOrWhiteSpace(category)) .Distinct(StringComparer.OrdinalIgnoreCase)`.
            // Tipe variabel disimpulkan dari ekspresi nilai awal.
            var cashflowCategory = string.Join(
                // Meneruskan nilai literal `'|'` sebagai argumen ke `string.Join`.
                '|',
                // Meneruskan menghapus hasil duplikat dari `eventProjections .Select(projection => projection.Category) .Where(category =>
                // !string.IsNullOrWhiteSpace(category))` memakai `StringComparer.OrdinalIgnoreCase` sebagai argumen ke `string.Join`.
                eventProjections
                    // Meneruskan fungsi lambda `projection => projection.Category` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai
                    // argumen ke `eventProjections .Select`.
                    .Select(projection => string.Equals(evt.ActionType, GameActionCatalog.SetupBahanAwal, StringComparison.OrdinalIgnoreCase)
                        && string.Equals(projection.Category, "INGREDIENT", StringComparison.OrdinalIgnoreCase)
                            ? "INITIAL_INGREDIENT"
                            : projection.Category)
                    // Meneruskan fungsi lambda `category => !string.IsNullOrWhiteSpace(category)` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan
                    // sebagai argumen ke `eventProjections .Select(projection => projection.Category) .Where`; Meneruskan `category` (nilai category) sebagai argumen
                    // ke `string.IsNullOrWhiteSpace`.
                    .Where(category => !string.IsNullOrWhiteSpace(category))
                    // Meneruskan `StringComparer.OrdinalIgnoreCase` (nilai ordinal ignore case) sebagai argumen ke `eventProjections .Select(projection =>
                    // projection.Category) .Where(category => !string.IsNullOrWhiteSpace(category)) .Distinct`.
                    .Distinct(StringComparer.OrdinalIgnoreCase));

            // Memeriksa pemeriksaan lebih besar antara `spent` dan `0`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam Compute.
            if (spent > 0)
            // Membuka scope cabang if untuk kondisi `spent > 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Compute.
            {
                // Menjalankan menambahkan `new AnalyticsTurnAmount(evt.DayIndex, evt.ActionSlot, evt.SequenceNumber, cashflowCategory, spent)` ke
                // `coinsSpentPerTurn` dalam Compute.
                coinsSpentPerTurn.Add(new AnalyticsTurnAmount(evt.DayIndex, evt.ActionSlot, evt.SequenceNumber, cashflowCategory, spent));
            // Menutup scope cabang if untuk kondisi `spent > 0`; bagian berikut berada di luar batas blok tersebut dalam Compute.
            }

            // Memeriksa pemeriksaan lebih besar antara `earned` dan `0`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam Compute.
            if (earned > 0)
            // Membuka scope cabang if untuk kondisi `earned > 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Compute.
            {
                // Menjalankan menambahkan `new AnalyticsTurnAmount(evt.DayIndex, evt.ActionSlot, evt.SequenceNumber, cashflowCategory, earned)` ke
                // `coinsEarnedPerTurn` dalam Compute.
                coinsEarnedPerTurn.Add(new AnalyticsTurnAmount(evt.DayIndex, evt.ActionSlot, evt.SequenceNumber, cashflowCategory, earned));
            // Menutup scope cabang if untuk kondisi `earned > 0`; bagian berikut berada di luar batas blok tersebut dalam Compute.
            }

            // Menyiapkan variabel lokal `net` untuk nilai net dengan selisih antara `earned` dan `spent`. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var net = earned - spent;
            // Memperbarui `runningCoins` dengan menambahkan `net` (nilai net) dalam Compute.
            runningCoins += net;
            // Menjalankan menambahkan `new AnalyticsTurnNet(evt.DayIndex, evt.ActionSlot, evt.SequenceNumber, cashflowCategory, net)` ke `netIncomePerTurn`
            // dalam Compute.
            netIncomePerTurn.Add(new AnalyticsTurnNet(evt.DayIndex, evt.ActionSlot, evt.SequenceNumber, cashflowCategory, net));
            // Menjalankan menambahkan `new AnalyticsTurnCoins(evt.DayIndex, evt.ActionSlot, evt.SequenceNumber, cashflowCategory, runningCoins)` ke
            // `coinsProgression` dalam Compute.
            coinsProgression.Add(new AnalyticsTurnCoins(evt.DayIndex, evt.ActionSlot, evt.SequenceNumber, cashflowCategory, runningCoins));
        // Menutup scope loop setiap evt dari `sessionEvents.OrderBy(item => item.SequenceNumber)`; bagian berikut berada di luar batas blok tersebut dalam
        // Compute.
        }

        // Mengembalikan objek baru bertipe `AnalyticsCashTimeline` dengan argumen ( startingCoins, cashInTotal, cashOutTotal, coinsNetEndGame,
        // coinsNetEndGame, coinsSpentPerTurn, coinsEarnedPerTurn, netIncomePerTurn, coinsProgression) kepada pemanggil dalam Compute; eksekusi jalur ini
        // selesai setelah nilai hasil ditentukan.
        return new AnalyticsCashTimeline(
            // Meneruskan `startingCoins` (nilai starting coins) sebagai argumen ke konstruktor `AnalyticsCashTimeline`.
            startingCoins,
            // Meneruskan `cashInTotal` (jumlah seluruh pemasukan arus kas) sebagai argumen ke konstruktor `AnalyticsCashTimeline`.
            cashInTotal,
            // Meneruskan `cashOutTotal` (jumlah seluruh pengeluaran arus kas) sebagai argumen ke konstruktor `AnalyticsCashTimeline`.
            cashOutTotal,
            // Meneruskan `coinsNetEndGame` (nilai coins net end game) sebagai argumen ke konstruktor `AnalyticsCashTimeline`.
            coinsNetEndGame,
            // Meneruskan `coinsNetEndGame` (nilai coins net end game) sebagai argumen ke konstruktor `AnalyticsCashTimeline`.
            coinsNetEndGame,
            // Meneruskan `coinsSpentPerTurn` (nilai coins spent per giliran) sebagai argumen ke konstruktor `AnalyticsCashTimeline`.
            coinsSpentPerTurn,
            // Meneruskan `coinsEarnedPerTurn` (nilai coins earned per giliran) sebagai argumen ke konstruktor `AnalyticsCashTimeline`.
            coinsEarnedPerTurn,
            // Meneruskan `netIncomePerTurn` (nilai net pemasukan per giliran) sebagai argumen ke konstruktor `AnalyticsCashTimeline`.
            netIncomePerTurn,
            // Meneruskan `coinsProgression` (nilai coins progression) sebagai argumen ke konstruktor `AnalyticsCashTimeline`.
            coinsProgression);
    // Menutup scope metode Compute; bagian berikut berada di luar batas blok tersebut dalam Compute.
    }
// Menutup scope tipe CashTimelineCalculator; bagian berikut berada di luar batas blok tersebut.
}
