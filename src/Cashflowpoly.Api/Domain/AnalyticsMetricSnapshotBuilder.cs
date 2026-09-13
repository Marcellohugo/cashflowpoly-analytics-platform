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
    {
        var list = new List<MetricSnapshotDb>();
        // Mengulangi setiap elemen `metrics`; elemen saat ini disimpan sebagai `item` bertipe `var` untuk diproses oleh badan loop dalam
        // BuildMetricSnapshots.
        foreach (var item in metrics)
        {
            list.Add(new MetricSnapshotDb
            {
                MetricSnapshotId = Guid.NewGuid(),
                SessionId = sessionId,
                UserId = playerId,
                ComputedAt = computedAt,
                MetricName = item.Key,
                MetricValueNumeric = item.Value.Numeric,
                MetricValueJson = item.Value.Json,
                RulesetVersionId = rulesetVersionId
            });
        }

        return list;
    }
}
