// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui AnalyticsSessionMetricCalculator.
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Domain;

/// <summary>
/// Kalkulator murni untuk metric snapshot level sesi.
/// </summary>
// Mendefinisikan tipe class `SessionMetricCalculator` yang mewarisi atau menerapkan `ISessionMetricCalculator`; sealed mencegah tipe ini diturunkan
// lagi.
internal sealed class SessionMetricCalculator : ISessionMetricCalculator
// Membuka scope tipe SessionMetricCalculator; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendeklarasikan field bertipe `AnalyticsPayloadReader`: `_payloadReader` menyimpan nilai payload pembaca dengan nilai awal objek baru dengan tipe
    // mengikuti konteks tujuan dan argumen (). readonly membatasi penggantian referensi/nilai field pada deklarasi atau konstruktor. static membuat
    // field menjadi milik tipe dan dibagikan antar instance.
    private static readonly AnalyticsPayloadReader _payloadReader = new();
    /// <summary>
    /// Menghitung metrik agregat level sesi: cashflow total, donasi, dan happiness.
    /// </summary>
    // Mendefinisikan metode `ComputeSessionMetrics` dengan hasil bertipe `Dictionary<string, (double? Numeric, string? Json)>`. Menghitung metrik
    // agregat level sesi: cashflow total, donasi, dan happiness. Masukan: Parameter `events` bertipe `List<EventDb>` membawa kumpulan event permainan
    // sebagai sumber riwayat untuk validasi atau perhitungan; Parameter `projections` bertipe `List<CashflowProjectionDb>` membawa proyeksi transaksi
    // arus kas yang diturunkan dari event permainan; Parameter `happinessByPlayer` bertipe `Dictionary<Guid, AnalyticsHappinessBreakdown>` membawa
    // nilai kebahagiaan berdasarkan pemain.
    public Dictionary<string, (double? Numeric, string? Json)> ComputeSessionMetrics(
        // Parameter `events` bertipe `List<EventDb>` membawa kumpulan event permainan sebagai sumber riwayat untuk validasi atau perhitungan.
        List<EventDb> events,
        // Parameter `projections` bertipe `List<CashflowProjectionDb>` membawa proyeksi transaksi arus kas yang diturunkan dari event permainan.
        List<CashflowProjectionDb> projections,
        // Parameter `happinessByPlayer` bertipe `Dictionary<Guid, AnalyticsHappinessBreakdown>` membawa nilai kebahagiaan berdasarkan pemain.
        Dictionary<Guid, AnalyticsHappinessBreakdown> happinessByPlayer)
    // Membuka scope metode ComputeSessionMetrics; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ComputeSessionMetrics.
    {
        // Menyiapkan variabel lokal `metrics` untuk nilai metrics dengan objek baru bertipe `Dictionary<string, (double? Numeric, string? Json)>` dengan
        // nilai awal sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var metrics = new Dictionary<string, (double? Numeric, string? Json)>();

        // Menyiapkan variabel lokal `cashIn` untuk nilai uang tunai in dengan menjumlahkan nilai `projections.Where(p => p.Direction == ”IN”)` berdasarkan
        // `p => (double)p.Amount`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var cashIn = projections.Where(p => p.Direction == "IN").Sum(p => (double)p.Amount);
        // Menyiapkan variabel lokal `cashOut` untuk nilai uang tunai out dengan menjumlahkan nilai `projections.Where(p => p.Direction == ”OUT”)`
        // berdasarkan `p => (double)p.Amount`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var cashOut = projections.Where(p => p.Direction == "OUT").Sum(p => (double)p.Amount);
        // Memperbarui `metrics[”cashflow.in.total”]` menggunakan tuple yang membawa bagian 1: cashIn; bagian 2: null dalam ComputeSessionMetrics.
        metrics["cashflow.in.total"] = (cashIn, null);
        // Memperbarui `metrics[”cashflow.out.total”]` menggunakan tuple yang membawa bagian 1: cashOut; bagian 2: null dalam ComputeSessionMetrics.
        metrics["cashflow.out.total"] = (cashOut, null);
        // Memperbarui `metrics[”cashflow.net.total”]` menggunakan tuple yang membawa bagian 1: cashIn - cashOut; bagian 2: null dalam
        // ComputeSessionMetrics.
        metrics["cashflow.net.total"] = (cashIn - cashOut, null);

        // Menyiapkan variabel lokal `donationTotal` untuk nilai donasi total dengan menjumlahkan nilai `events.Where(e => e.ActionType == ”JumatBerkah”)
        // .Select(e => _payloadReader.TryReadAmount(e.Payload, out var amount) ? amount : 0)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var donationTotal = events.Where(e => e.ActionType == "JumatBerkah")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Select(e => _payloadReader.TryReadAmount(e.Payload, out var amount) ? amount :
            // 0) dalam ComputeSessionMetrics; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Select(e => _payloadReader.TryReadAmount(e.Payload, out var amount) ? amount : 0)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Sum(); dalam ComputeSessionMetrics; token pada baris ini menyambungkan bagian
            // kode sebelum dan sesudahnya.
            .Sum();
        // Memperbarui `metrics[”donation.total”]` menggunakan tuple yang membawa bagian 1: donationTotal; bagian 2: null dalam ComputeSessionMetrics.
        metrics["donation.total"] = (donationTotal, null);

        // Menyiapkan variabel lokal `happinessTotal` untuk nilai kebahagiaan total dengan menjumlahkan nilai `happinessByPlayer.Values` berdasarkan `item
        // => item.Total`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var happinessTotal = happinessByPlayer.Values.Sum(item => item.Total);
        // Memperbarui `metrics[”happiness.points.total”]` menggunakan tuple yang membawa bagian 1: happinessTotal; bagian 2: null dalam
        // ComputeSessionMetrics.
        metrics["happiness.points.total"] = (happinessTotal, null);

        // Mengembalikan `metrics` (nilai metrics) kepada pemanggil dalam ComputeSessionMetrics; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return metrics;
    // Menutup scope metode ComputeSessionMetrics; bagian berikut berada di luar batas blok tersebut dalam ComputeSessionMetrics.
    }
// Menutup scope tipe SessionMetricCalculator; bagian berikut berada di luar batas blok tersebut.
}
