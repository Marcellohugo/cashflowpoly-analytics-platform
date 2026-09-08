// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui AnalyticsActionUsageCalculator.
// Mengimpor namespace `System.Text.Json.Serialization` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.Json.Serialization;
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;
// Mengimpor namespace `Cashflowpoly.Api.Domain.AnalyticsMath` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using static Cashflowpoly.Api.Domain.AnalyticsMath;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Domain;

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `AnalyticsActionUsageMetrics`; sealed mencegah tipe ini diturunkan
// lagi.
public sealed record AnalyticsActionUsageMetrics(
    // Parameter `ActionSequences` bertipe `IReadOnlyList<AnalyticsActionSequence>` membawa nilai aksi sequences.
    IReadOnlyList<AnalyticsActionSequence> ActionSequences,
    // Parameter `ActionRepetitions` bertipe `IReadOnlyList<AnalyticsActionRepetition>` membawa nilai aksi repetitions.
    IReadOnlyList<AnalyticsActionRepetition> ActionRepetitions,
    // Parameter `ActionSlotTimeline` bertipe `IReadOnlyList<AnalyticsActionSlot>` membawa nilai aksi slot timeline.
    IReadOnlyList<AnalyticsActionSlot> ActionSlotTimeline,
    // Parameter `LatestDayIndex` bertipe `int?` membawa nilai latest hari index; nilai null diizinkan ketika data opsional belum tersedia.
    int? LatestDayIndex,
    // Parameter `LatestActionSlot` bertipe `int?` membawa nilai latest aksi slot; nilai null diizinkan ketika data opsional belum tersedia.
    int? LatestActionSlot,
    // Parameter `ActionEventCount` bertipe `int` membawa nilai aksi event jumlah.
    int ActionEventCount,
    // Parameter `IncomeActions` bertipe `int` membawa nilai pemasukan aksi.
    int IncomeActions,
    // Parameter `ActionEfficiency` bertipe `double?` membawa nilai aksi efficiency; nilai null diizinkan ketika data opsional belum tersedia.
    double? ActionEfficiency,
    // Parameter `ActionEfficiencyPercent` bertipe `double?` membawa nilai aksi efficiency percent; nilai null diizinkan ketika data opsional belum
    // tersedia.
    double? ActionEfficiencyPercent,
    // Parameter `ActionDiversityAverage` bertipe `double` membawa nilai aksi keberagaman rata-rata.
    double ActionDiversityAverage);

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `AnalyticsActionSequence`; sealed mencegah tipe ini diturunkan lagi.
public sealed record AnalyticsActionSequence(
    // Parameter `DayIndex` bertipe `int` membawa nilai hari index; memetakan nama properti JSON menjadi (”day_index”).
    [property: JsonPropertyName("day_index")] int DayIndex,
    // Parameter `Actions` bertipe `IReadOnlyList<string>` membawa nilai aksi; memetakan nama properti JSON menjadi (”actions”).
    [property: JsonPropertyName("actions")] IReadOnlyList<string> Actions);

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `AnalyticsActionRepetition`; sealed mencegah tipe ini diturunkan lagi.
public sealed record AnalyticsActionRepetition(
    // Parameter `DayIndex` bertipe `int` membawa nilai hari index; memetakan nama properti JSON menjadi (”day_index”).
    [property: JsonPropertyName("day_index")] int DayIndex,
    // Parameter `TotalActions` bertipe `int` membawa nilai total aksi; memetakan nama properti JSON menjadi (”total_actions”).
    [property: JsonPropertyName("total_actions")] int TotalActions,
    // Parameter `DistinctActions` bertipe `int` membawa nilai distinct aksi; memetakan nama properti JSON menjadi (”distinct_actions”).
    [property: JsonPropertyName("distinct_actions")] int DistinctActions,
    // Parameter `RepeatedActions` bertipe `int` membawa nilai repeated aksi; memetakan nama properti JSON menjadi (”repeated_actions”).
    [property: JsonPropertyName("repeated_actions")] int RepeatedActions,
    // Parameter `DiversityScore` bertipe `double` membawa nilai keberagaman skor; memetakan nama properti JSON menjadi (”diversity_score”).
    [property: JsonPropertyName("diversity_score")] double DiversityScore);

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `AnalyticsActionSlot`; sealed mencegah tipe ini diturunkan lagi.
public sealed record AnalyticsActionSlot(
    // Parameter `DayIndex` bertipe `int` membawa nilai hari index; memetakan nama properti JSON menjadi (”day_index”).
    [property: JsonPropertyName("day_index")] int DayIndex,
    // Parameter `ActionSlot` bertipe `int` membawa nilai aksi slot; memetakan nama properti JSON menjadi (”action_slot”).
    [property: JsonPropertyName("action_slot")] int ActionSlot,
    // Parameter `ActionIndex` bertipe `int` membawa nilai aksi index; memetakan nama properti JSON menjadi (”action_index”).
    [property: JsonPropertyName("action_index")] int ActionIndex,
    // Parameter `ActionType` bertipe `string` membawa nilai aksi jenis; memetakan nama properti JSON menjadi (”action_type”).
    [property: JsonPropertyName("action_type")] string ActionType,
    // Parameter `SequenceNumber` bertipe `long` membawa nomor urut event yang menentukan urutan pemrosesan riwayat permainan; mengatur pengabaian
    // properti JSON sesuai saat serialisasi/deserialisasi.
    [property: JsonIgnore] long SequenceNumber);

// Mendefinisikan tipe class `ActionUsageCalculator` yang mewarisi atau menerapkan `IActionUsageCalculator`; sealed mencegah tipe ini diturunkan
// lagi.
internal sealed class ActionUsageCalculator : IActionUsageCalculator
// Membuka scope tipe ActionUsageCalculator; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendeklarasikan field bertipe `EventPayloadReader`: `_payloadReader` menyimpan nilai payload pembaca dengan nilai awal objek baru dengan tipe
    // mengikuti konteks tujuan dan argumen (). readonly membatasi penggantian referensi/nilai field pada deklarasi atau konstruktor. static membuat
    // field menjadi milik tipe dan dibagikan antar instance.
    private static readonly EventPayloadReader _payloadReader = new();

    // Mendefinisikan metode `Compute` dengan hasil bertipe `AnalyticsActionUsageMetrics`; operasi ini menangani compute. Masukan: Parameter
    // `playerEvents` bertipe `IReadOnlyCollection<EventDb>` membawa nilai pemain event; Parameter `playerProjections` bertipe
    // `IReadOnlyCollection<CashflowProjectionDb>` membawa nilai pemain projections; Parameter `latestDayIndex` bertipe `int` membawa nilai latest hari
    // index; Parameter `actionsPerTurn` bertipe `int` membawa nilai aksi per giliran.
    public AnalyticsActionUsageMetrics Compute(
        // Parameter `playerEvents` bertipe `IReadOnlyCollection<EventDb>` membawa nilai pemain event.
        IReadOnlyCollection<EventDb> playerEvents,
        // Parameter `playerProjections` bertipe `IReadOnlyCollection<CashflowProjectionDb>` membawa nilai pemain projections.
        IReadOnlyCollection<CashflowProjectionDb> playerProjections,
        // Parameter `latestDayIndex` bertipe `int` membawa nilai latest hari index.
        int latestDayIndex,
        // Parameter `actionsPerTurn` bertipe `int` membawa nilai aksi per giliran.
        int actionsPerTurn)
    // Membuka scope metode Compute; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Compute.
    {
        // Menyiapkan variabel lokal `actionEvents` untuk nilai aksi event dengan mematerialisasi urutan `playerEvents .Where(e =>
        // string.Equals(e.ActorType, ”PLAYER”, StringComparison.OrdinalIgnoreCase) && GameActionCatalog.GetPlayerActionSlotPolicy( e.ActionType,
        // _payloadReader....` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var actionEvents = playerEvents
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(e => string.Equals(e.ActorType, ”PLAYER”,
            // StringComparison.OrdinalIgnoreCase) && dalam Compute; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Where(e => string.Equals(e.ActorType, "PLAYER", StringComparison.OrdinalIgnoreCase) &&
                        // Meneruskan fungsi lambda `e => string.Equals(e.ActorType, ”PLAYER”, StringComparison.OrdinalIgnoreCase) &&
                        // GameActionCatalog.GetPlayerActionSlotPolicy( e.ActionType, _payloadReader.ReadPayload(string.I...` yang dijalankan oleh operasi pemanggil untuk
                        // memproses setiap masukan sebagai argumen ke `playerEvents .Where`.
                        GameActionCatalog.GetPlayerActionSlotPolicy(
                            // Meneruskan `e.ActionType` (nilai aksi jenis) sebagai argumen ke `GameActionCatalog.GetPlayerActionSlotPolicy`.
                            e.ActionType,
                            // Meneruskan memanggil `_payloadReader.ReadPayload` dengan `string.IsNullOrWhiteSpace(e.Payload) ? ”{}” : e.Payload` sebagai argumen ke
                            // `GameActionCatalog.GetPlayerActionSlotPolicy`; Meneruskan hasil pemilihan bersyarat: ketika `string.IsNullOrWhiteSpace(e.Payload)` benar gunakan
                            // `”{}”`, jika tidak gunakan `e.Payload` sebagai argumen ke `_payloadReader.ReadPayload`; Meneruskan `e.Payload` (muatan detail event dalam format
                            // JSON) sebagai argumen ke `string.IsNullOrWhiteSpace`.
                            _payloadReader.ReadPayload(string.IsNullOrWhiteSpace(e.Payload) ? "{}" : e.Payload)) == PlayerActionSlotPolicy.Consumes)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .OrderBy(e => e.SequenceNumber) dalam Compute; token pada baris ini
            // menyambungkan bagian kode sebelum dan sesudahnya.
            .OrderBy(e => e.SequenceNumber)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList(); dalam Compute; token pada baris ini menyambungkan bagian kode
            // sebelum dan sesudahnya.
            .ToList();

        // Menyiapkan variabel lokal `actionSequences` untuk nilai aksi sequences dengan mematerialisasi urutan `actionEvents .GroupBy(e => e.DayIndex)
        // .OrderBy(g => g.Key) .Select(g => new AnalyticsActionSequence( g.Key, g.OrderBy(e => e.SequenceNumber).Select(e => e.ActionType).ToList(...`
        // menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var actionSequences = actionEvents
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .GroupBy(e => e.DayIndex) dalam Compute; token pada baris ini menyambungkan
            // bagian kode sebelum dan sesudahnya.
            .GroupBy(e => e.DayIndex)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .OrderBy(g => g.Key) dalam Compute; token pada baris ini menyambungkan bagian
            // kode sebelum dan sesudahnya.
            .OrderBy(g => g.Key)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Select(g => new AnalyticsActionSequence( dalam Compute; token pada baris ini
            // menyambungkan bagian kode sebelum dan sesudahnya.
            .Select(g => new AnalyticsActionSequence(
                // Meneruskan `g.Key` (nilai kunci) sebagai argumen ke konstruktor `AnalyticsActionSequence`.
                g.Key,
                // Meneruskan mematerialisasi urutan `g.OrderBy(e => e.SequenceNumber).Select(e => e.ActionType)` menjadi List; enumerasi dijalankan dan hasilnya
                // disimpan dalam memori sebagai argumen ke konstruktor `AnalyticsActionSequence`; Meneruskan fungsi lambda `e => e.SequenceNumber` yang dijalankan
                // oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen ke `g.OrderBy`; Meneruskan fungsi lambda `e => e.ActionType` yang
                // dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen ke `g.OrderBy(e => e.SequenceNumber).Select`.
                g.OrderBy(e => e.SequenceNumber).Select(e => e.ActionType).ToList()))
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList(); dalam Compute; token pada baris ini menyambungkan bagian kode
            // sebelum dan sesudahnya.
            .ToList();

        // Menyiapkan variabel lokal `actionRepetitions` untuk nilai aksi repetitions dengan mematerialisasi urutan `actionEvents .GroupBy(e => e.DayIndex)
        // .OrderBy(g => g.Key) .Select(g => { var distinctActions = g.Select(e => e.ActionType).Distinct(StringComparer.OrdinalIgnoreCase).Count()...`
        // menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var actionRepetitions = actionEvents
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .GroupBy(e => e.DayIndex) dalam Compute; token pada baris ini menyambungkan
            // bagian kode sebelum dan sesudahnya.
            .GroupBy(e => e.DayIndex)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .OrderBy(g => g.Key) dalam Compute; token pada baris ini menyambungkan bagian
            // kode sebelum dan sesudahnya.
            .OrderBy(g => g.Key)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Select(g => dalam Compute; token pada baris ini menyambungkan bagian kode
            // sebelum dan sesudahnya.
            .Select(g =>
            // Membuka scope fungsi lambda yang dipasok ke `actionEvents .GroupBy(e => e.DayIndex) .OrderBy(g => g.Key) .Select`; pernyataan/deklarasi berikut
            // berada di dalam batas blok ini dalam Compute.
            {
                // Menyiapkan variabel lokal `distinctActions` untuk nilai distinct aksi dengan memanggil `g.Select(e =>
                // e.ActionType).Distinct(StringComparer.OrdinalIgnoreCase).Count` dengan tanpa argumen. Tipe variabel disimpulkan dari ekspresi nilai awal.
                var distinctActions = g.Select(e => e.ActionType).Distinct(StringComparer.OrdinalIgnoreCase).Count();
                // Menyiapkan variabel lokal `totalActions` untuk nilai total aksi dengan memanggil `g.Count` dengan tanpa argumen. Tipe variabel disimpulkan dari
                // ekspresi nilai awal.
                var totalActions = g.Count();
                // Menyiapkan variabel lokal `repeatedActions` untuk nilai repeated aksi dengan menentukan nilai terbesar dari `0`, `totalActions -
                // distinctActions`. Tipe variabel disimpulkan dari ekspresi nilai awal.
                var repeatedActions = Math.Max(0, totalActions - distinctActions);
                // Menyiapkan variabel lokal `diversityScore` untuk nilai keberagaman skor dengan hasil pemilihan bersyarat: ketika `actionsPerTurn > 0` benar
                // gunakan `Math.Min(1, (double)distinctActions / actionsPerTurn)`, jika tidak gunakan `0`. Tipe variabel disimpulkan dari ekspresi nilai awal.
                var diversityScore = actionsPerTurn > 0
                    // Meneruskan nilai literal `1` sebagai argumen ke `Math.Min`; Meneruskan pembagian antara `(double)distinctActions` dan `actionsPerTurn` sebagai
                    // argumen ke `Math.Min`.
                    ? Math.Min(1, (double)distinctActions / actionsPerTurn)
                    // Meneruskan fungsi lambda `g => { var distinctActions = g.Select(e => e.ActionType).Distinct(StringComparer.OrdinalIgnoreCase).Count(); var
                    // totalActions = g.Count(); var repeatedActions = Math.Max(0, to...` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai
                    // argumen ke `actionEvents .GroupBy(e => e.DayIndex) .OrderBy(g => g.Key) .Select`.
                    : 0;
                // Mengembalikan objek baru bertipe `AnalyticsActionRepetition` dengan argumen ( g.Key, totalActions, distinctActions, repeatedActions,
                // diversityScore) kepada pemanggil dalam Compute; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
                return new AnalyticsActionRepetition(
                    // Meneruskan `g.Key` (nilai kunci) sebagai argumen ke konstruktor `AnalyticsActionRepetition`.
                    g.Key,
                    // Meneruskan `totalActions` (nilai total aksi) sebagai argumen ke konstruktor `AnalyticsActionRepetition`.
                    totalActions,
                    // Meneruskan `distinctActions` (nilai distinct aksi) sebagai argumen ke konstruktor `AnalyticsActionRepetition`.
                    distinctActions,
                    // Meneruskan `repeatedActions` (nilai repeated aksi) sebagai argumen ke konstruktor `AnalyticsActionRepetition`.
                    repeatedActions,
                    // Meneruskan `diversityScore` (nilai keberagaman skor) sebagai argumen ke konstruktor `AnalyticsActionRepetition`.
                    diversityScore);
            // Menutup scope fungsi lambda yang dipasok ke `actionEvents .GroupBy(e => e.DayIndex) .OrderBy(g => g.Key) .Select`; bagian berikut berada di luar
            // batas blok tersebut dalam Compute.
            })
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList(); dalam Compute; token pada baris ini menyambungkan bagian kode
            // sebelum dan sesudahnya.
            .ToList();

        // Menyiapkan variabel lokal `actionSlotTimeline` untuk nilai aksi slot timeline dengan mematerialisasi urutan `actionEvents .GroupBy(e =>
        // e.DayIndex) .OrderBy(g => g.Key) .SelectMany(g => g .OrderBy(e => e.SequenceNumber) .Select((e, index) => new AnalyticsActionSlot( e.DayIndex,
        // e.Act...` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var actionSlotTimeline = actionEvents
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .GroupBy(e => e.DayIndex) dalam Compute; token pada baris ini menyambungkan
            // bagian kode sebelum dan sesudahnya.
            .GroupBy(e => e.DayIndex)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .OrderBy(g => g.Key) dalam Compute; token pada baris ini menyambungkan bagian
            // kode sebelum dan sesudahnya.
            .OrderBy(g => g.Key)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .SelectMany(g => g dalam Compute; token pada baris ini menyambungkan bagian
            // kode sebelum dan sesudahnya.
            .SelectMany(g => g
                // Meneruskan fungsi lambda `e => e.SequenceNumber` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen ke `g
                // .OrderBy`.
                .OrderBy(e => e.SequenceNumber)
                // Meneruskan fungsi lambda `(e, index) => new AnalyticsActionSlot( e.DayIndex, e.ActionSlot, index + 1, e.ActionType, e.SequenceNumber)` yang
                // dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen ke `g .OrderBy(e => e.SequenceNumber) .Select`.
                .Select((e, index) => new AnalyticsActionSlot(
                    // Meneruskan `e.DayIndex` (nilai hari index) sebagai argumen ke konstruktor `AnalyticsActionSlot`.
                    e.DayIndex,
                    // Meneruskan `e.ActionSlot` (nilai aksi slot) sebagai argumen ke konstruktor `AnalyticsActionSlot`.
                    e.ActionSlot,
                    // Meneruskan penjumlahan/penggabungan antara `index` dan `1` sebagai argumen ke konstruktor `AnalyticsActionSlot`.
                    index + 1,
                    // Meneruskan `e.ActionType` (nilai aksi jenis) sebagai argumen ke konstruktor `AnalyticsActionSlot`.
                    e.ActionType,
                    // Meneruskan `e.SequenceNumber` (nomor urut event yang menentukan urutan pemrosesan riwayat permainan) sebagai argumen ke konstruktor
                    // `AnalyticsActionSlot`.
                    e.SequenceNumber)))
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList(); dalam Compute; token pada baris ini menyambungkan bagian kode
            // sebelum dan sesudahnya.
            .ToList();
        // Menyiapkan variabel lokal `latestActionSlot` untuk nilai latest aksi slot dengan mengambil elemen pertama `actionSlotTimeline
        // .OrderByDescending(item => item.SequenceNumber) .Select(item => (int?)item.ActionSlot)`; jika tidak ada, gunakan nilai default tipe hasil. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var latestActionSlot = actionSlotTimeline
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .OrderByDescending(item => item.SequenceNumber) dalam Compute; token pada baris
            // ini menyambungkan bagian kode sebelum dan sesudahnya.
            .OrderByDescending(item => item.SequenceNumber)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Select(item => (int?)item.ActionSlot) dalam Compute; token pada baris ini
            // menyambungkan bagian kode sebelum dan sesudahnya.
            .Select(item => (int?)item.ActionSlot)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .FirstOrDefault(); dalam Compute; token pada baris ini menyambungkan bagian
            // kode sebelum dan sesudahnya.
            .FirstOrDefault();

        // Menyiapkan variabel lokal `incomeActionEventIds` untuk nilai pemasukan aksi event identitas dengan membentuk himpunan nilai unik dari
        // `playerProjections .Where(p => p.Direction == ”IN”) .Select(p => p.EventId)` memakai tanpa argumen. Tipe variabel disimpulkan dari ekspresi nilai
        // awal.
        var incomeActionEventIds = playerProjections
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(p => p.Direction == ”IN”) dalam Compute; token pada baris ini
            // menyambungkan bagian kode sebelum dan sesudahnya.
            .Where(p => p.Direction == "IN")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Select(p => p.EventId) dalam Compute; token pada baris ini menyambungkan
            // bagian kode sebelum dan sesudahnya.
            .Select(p => p.EventId)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToHashSet(); dalam Compute; token pada baris ini menyambungkan bagian kode
            // sebelum dan sesudahnya.
            .ToHashSet();
        // Menyiapkan variabel lokal `incomeActions` untuk nilai pemasukan aksi dengan memanggil `actionEvents.Count` dengan `e =>
        // incomeActionEventIds.Contains(e.EventId)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var incomeActions = actionEvents.Count(e =>
            e.ActionType is GameActionCatalog.KerjaLepas or GameActionCatalog.JualMasakan &&
            incomeActionEventIds.Contains(e.EventId));
        // Menyiapkan variabel lokal `actionEfficiency` untuk nilai aksi efficiency dengan memanggil `SafeRatio` dengan `incomeActions`,
        // `actionEvents.Count`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var actionEfficiency = SafeRatio(incomeActions, actionEvents.Count);
        // Menyiapkan variabel lokal `actionDiversityAverage` untuk nilai aksi keberagaman rata-rata dengan hasil pemilihan bersyarat: ketika
        // `actionRepetitions.Count > 0` benar gunakan `actionRepetitions.Average(item => item.DiversityScore)`, jika tidak gunakan `0`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var actionDiversityAverage = actionRepetitions.Count > 0
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: actionRepetitions.Average(item => item.DiversityScore) dalam Compute.
            ? actionRepetitions.Average(item => item.DiversityScore)
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: 0; dalam Compute.
            : 0;

        // Mengembalikan objek baru bertipe `AnalyticsActionUsageMetrics` dengan argumen ( actionSequences, actionRepetitions, actionSlotTimeline,
        // actionEvents.Count == 0 ? null : actionEvents.Max(e => e.DayIndex), latestActionSlot, actionEvents.Cou... kepada pemanggil dalam Compute;
        // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return new AnalyticsActionUsageMetrics(
            // Meneruskan `actionSequences` (nilai aksi sequences) sebagai argumen ke konstruktor `AnalyticsActionUsageMetrics`.
            actionSequences,
            // Meneruskan `actionRepetitions` (nilai aksi repetitions) sebagai argumen ke konstruktor `AnalyticsActionUsageMetrics`.
            actionRepetitions,
            // Meneruskan `actionSlotTimeline` (nilai aksi slot timeline) sebagai argumen ke konstruktor `AnalyticsActionUsageMetrics`.
            actionSlotTimeline,
            // Meneruskan hasil pemilihan bersyarat: ketika `actionEvents.Count == 0` benar gunakan `null`, jika tidak gunakan `actionEvents.Max(e =>
            // e.DayIndex)` sebagai argumen ke konstruktor `AnalyticsActionUsageMetrics`; Meneruskan fungsi lambda `e => e.DayIndex` yang dijalankan oleh
            // operasi pemanggil untuk memproses setiap masukan sebagai argumen ke `actionEvents.Max`.
            actionEvents.Count == 0 ? null : actionEvents.Max(e => e.DayIndex),
            // Meneruskan `latestActionSlot` (nilai latest aksi slot) sebagai argumen ke konstruktor `AnalyticsActionUsageMetrics`.
            latestActionSlot,
            // Meneruskan `actionEvents.Count`, yaitu jumlah elemen atau panjang data sebagai argumen ke konstruktor `AnalyticsActionUsageMetrics`.
            actionEvents.Count,
            // Meneruskan `incomeActions` (nilai pemasukan aksi) sebagai argumen ke konstruktor `AnalyticsActionUsageMetrics`.
            incomeActions,
            // Meneruskan `actionEfficiency` (nilai aksi efficiency) sebagai argumen ke konstruktor `AnalyticsActionUsageMetrics`.
            actionEfficiency,
            // Meneruskan hasil pemilihan bersyarat: ketika `actionEfficiency.HasValue` benar gunakan `actionEfficiency.Value * 100`, jika tidak gunakan `null`
            // sebagai argumen ke konstruktor `AnalyticsActionUsageMetrics`.
            actionEfficiency.HasValue ? actionEfficiency.Value * 100 : null,
            // Meneruskan `actionDiversityAverage` (nilai aksi keberagaman rata-rata) sebagai argumen ke konstruktor `AnalyticsActionUsageMetrics`.
            actionDiversityAverage);
    // Menutup scope metode Compute; bagian berikut berada di luar batas blok tersebut dalam Compute.
    }
// Menutup scope tipe ActionUsageCalculator; bagian berikut berada di luar batas blok tersebut.
}
