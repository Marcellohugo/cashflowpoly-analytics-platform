// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui AnalyticsMetricSnapshotBuilder.
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Domain;

/// <summary>
/// Builder snapshot metrik yang siap disimpan ke tabel metric_snapshots.
/// </summary>
// Mendefinisikan tipe class `MetricSnapshotBuilder` yang mewarisi atau menerapkan `IMetricSnapshotBuilder`; sealed mencegah tipe ini diturunkan
// lagi.
internal sealed class MetricSnapshotBuilder : IMetricSnapshotBuilder
// Membuka scope tipe MetricSnapshotBuilder; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    /// <summary>
    /// Mengonversi dictionary metrik menjadi daftar record MetricSnapshotDb siap disimpan.
    /// </summary>
    // Mendefinisikan metode `BuildMetricSnapshots` dengan hasil bertipe `List<MetricSnapshotDb>`. Mengonversi dictionary metrik menjadi daftar record
    // MetricSnapshotDb siap disimpan. Masukan: Parameter `sessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data
    // operasi ini; Parameter `playerId` bertipe `Guid?` membawa nilai pemain identitas; nilai null diizinkan ketika data opsional belum tersedia;
    // Parameter `rulesetVersionId` bertipe `Guid` membawa identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang tepat; Parameter
    // `computedAt` bertipe `DateTimeOffset` membawa nilai computed at; Parameter `metrics` bertipe `Dictionary<string, (double? Numeric, string?
    // Json)>` membawa nilai metrics.
    public List<MetricSnapshotDb> BuildMetricSnapshots(
        // Parameter `sessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini.
        Guid sessionId,
        // Parameter `playerId` bertipe `Guid?` membawa nilai pemain identitas; nilai null diizinkan ketika data opsional belum tersedia.
        Guid? playerId,
        // Parameter `rulesetVersionId` bertipe `Guid` membawa identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang tepat.
        Guid rulesetVersionId,
        // Parameter `computedAt` bertipe `DateTimeOffset` membawa nilai computed at.
        DateTimeOffset computedAt,
        // Parameter `metrics` bertipe `Dictionary<string, (double? Numeric, string? Json)>` membawa nilai metrics.
        Dictionary<string, (double? Numeric, string? Json)> metrics)
    // Membuka scope metode BuildMetricSnapshots; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildMetricSnapshots.
    {
        // Menyiapkan variabel lokal `list` untuk nilai daftar dengan objek baru bertipe `List<MetricSnapshotDb>` dengan nilai awal sesuai konstruktornya.
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var list = new List<MetricSnapshotDb>();
        // Mengulangi setiap elemen `metrics`; elemen saat ini disimpan sebagai `item` bertipe `var` untuk diproses oleh badan loop dalam
        // BuildMetricSnapshots.
        foreach (var item in metrics)
        // Membuka scope loop setiap item dari `metrics`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildMetricSnapshots.
        {
            // Menjalankan menambahkan `new MetricSnapshotDb { MetricSnapshotId = Guid.NewGuid(), SessionId = sessionId, UserId = playerId, ComputedAt =
            // computedAt, MetricName = item.Key, MetricValueNumeric = item.V...` ke `list` dalam BuildMetricSnapshots.
            list.Add(new MetricSnapshotDb
            // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // BuildMetricSnapshots.
            {
                // Memperbarui `MetricSnapshotId` menggunakan memanggil `Guid.NewGuid` dengan tanpa argumen dalam BuildMetricSnapshots.
                MetricSnapshotId = Guid.NewGuid(),
                // Memperbarui `SessionId` menggunakan `sessionId` (identitas unik sesi permainan yang menjadi batas data operasi ini) dalam BuildMetricSnapshots.
                SessionId = sessionId,
                // Memperbarui `UserId` menggunakan `playerId` (nilai pemain identitas) dalam BuildMetricSnapshots.
                UserId = playerId,
                // Memperbarui `ComputedAt` menggunakan `computedAt` (nilai computed at) dalam BuildMetricSnapshots.
                ComputedAt = computedAt,
                // Memperbarui `MetricName` menggunakan `item.Key` (nilai kunci) dalam BuildMetricSnapshots.
                MetricName = item.Key,
                // Memperbarui `MetricValueNumeric` menggunakan `item.Value.Numeric` (nilai numerik) dalam BuildMetricSnapshots.
                MetricValueNumeric = item.Value.Numeric,
                // Memperbarui `MetricValueJson` menggunakan `item.Value.Json` (nilai JSON) dalam BuildMetricSnapshots.
                MetricValueJson = item.Value.Json,
                // Memperbarui `RulesetVersionId` menggunakan `rulesetVersionId` (identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang tepat)
                // dalam BuildMetricSnapshots.
                RulesetVersionId = rulesetVersionId
            // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam BuildMetricSnapshots.
            });
        // Menutup scope loop setiap item dari `metrics`; bagian berikut berada di luar batas blok tersebut dalam BuildMetricSnapshots.
        }

        // Mengembalikan `list` (nilai daftar) kepada pemanggil dalam BuildMetricSnapshots; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return list;
    // Menutup scope metode BuildMetricSnapshots; bagian berikut berada di luar batas blok tersebut dalam BuildMetricSnapshots.
    }
// Menutup scope tipe MetricSnapshotBuilder; bagian berikut berada di luar batas blok tersebut.
}
