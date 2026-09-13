// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui IMetricSnapshotBuilder.
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Domain;

public interface IMetricSnapshotBuilder
{
    List<MetricSnapshotDb> BuildMetricSnapshots(
        // Parameter `sessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini.
        Guid sessionId,
        // Parameter `playerId` bertipe `Guid?` membawa nilai pemain identitas; nilai null diizinkan ketika data opsional belum tersedia.
        Guid? playerId,
        // Parameter `rulesetVersionId` bertipe `Guid` membawa identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang tepat.
        Guid rulesetVersionId,
        // Parameter `computedAt` bertipe `DateTimeOffset` membawa nilai computed at.
        DateTimeOffset computedAt,
        // Parameter `metrics` bertipe `Dictionary<string, (double? Numeric, string? Json)>` membawa nilai metrics.
        Dictionary<string, (double? Numeric, string? Json)> metrics);
}
