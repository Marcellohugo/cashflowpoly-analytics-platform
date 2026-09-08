// Fungsi file: Memverifikasi perilaku API, database, atau domain melalui AnalyticsMetricSnapshotBuilderTests.
// Mengimpor namespace `Cashflowpoly.Api.Domain` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Domain;
// Mengimpor namespace `Xunit` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Xunit;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Tests` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Tests;

// Mendefinisikan tipe class `AnalyticsMetricSnapshotBuilderTests`; sealed mencegah tipe ini diturunkan lagi.
public sealed class AnalyticsMetricSnapshotBuilderTests
// Membuka scope tipe AnalyticsMetricSnapshotBuilderTests; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `BuildMetricSnapshots_MapsMetricValuesToSnapshotRows` dengan hasil bertipe `void`; operasi ini menangani build metric
    // snapshots maps metric nilai ke snapshot keadaan baris.
    public void BuildMetricSnapshots_MapsMetricValuesToSnapshotRows()
    // Membuka scope metode BuildMetricSnapshots_MapsMetricValuesToSnapshotRows; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // BuildMetricSnapshots_MapsMetricValuesToSnapshotRows.
    {
        // Menyiapkan variabel lokal `sessionId` untuk identitas unik sesi permainan yang menjadi batas data operasi ini dengan memanggil `Guid.NewGuid`
        // dengan tanpa argumen. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var sessionId = Guid.NewGuid();
        // Menyiapkan variabel lokal `playerId` untuk nilai pemain identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var playerId = Guid.NewGuid();
        // Menyiapkan variabel lokal `rulesetVersionId` untuk identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang tepat dengan
        // memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var rulesetVersionId = Guid.NewGuid();
        // Menyiapkan variabel lokal `computedAt` untuk nilai computed at dengan memanggil `DateTimeOffset.Parse` dengan `”2026-05-04T01:02:03Z”`. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var computedAt = DateTimeOffset.Parse("2026-05-04T01:02:03Z");
        // Menyiapkan variabel lokal `metrics` untuk nilai metrics dengan objek baru bertipe `Dictionary<string, (double? Numeric, string? Json)>` dengan
        // nilai awal sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var metrics = new Dictionary<string, (double? Numeric, string? Json)>
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // BuildMetricSnapshots_MapsMetricValuesToSnapshotRows.
        {
            // Memperbarui `[”cashflow.net.total”]` menggunakan tuple yang membawa bagian 1: 7; bagian 2: null dalam
            // BuildMetricSnapshots_MapsMetricValuesToSnapshotRows.
            ["cashflow.net.total"] = (7, null),
            // Memperbarui `[”gameplay.raw.variables”]` menggunakan tuple yang membawa bagian 1: null; bagian 2: ”””{”coins”:20}””” dalam
            // BuildMetricSnapshots_MapsMetricValuesToSnapshotRows.
            ["gameplay.raw.variables"] = (null, """{"coins":20}""")
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // BuildMetricSnapshots_MapsMetricValuesToSnapshotRows.
        };

        // Menyiapkan variabel lokal `snapshots` untuk nilai snapshots dengan memanggil `new MetricSnapshotBuilder().BuildMetricSnapshots` dengan
        // `sessionId`, `playerId`, `rulesetVersionId`, `computedAt`, `metrics`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var snapshots = new MetricSnapshotBuilder().BuildMetricSnapshots(
            // Meneruskan `sessionId` (identitas unik sesi permainan yang menjadi batas data operasi ini) sebagai argumen ke `new
            // MetricSnapshotBuilder().BuildMetricSnapshots`.
            sessionId,
            // Meneruskan `playerId` (nilai pemain identitas) sebagai argumen ke `new MetricSnapshotBuilder().BuildMetricSnapshots`.
            playerId,
            // Meneruskan `rulesetVersionId` (identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang tepat) sebagai argumen ke `new
            // MetricSnapshotBuilder().BuildMetricSnapshots`.
            rulesetVersionId,
            // Meneruskan `computedAt` (nilai computed at) sebagai argumen ke `new MetricSnapshotBuilder().BuildMetricSnapshots`.
            computedAt,
            // Meneruskan `metrics` (nilai metrics) sebagai argumen ke `new MetricSnapshotBuilder().BuildMetricSnapshots`.
            metrics);

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`2`, `snapshots.Count`); pengujian gagal jika
        // keduanya berbeda dalam BuildMetricSnapshots_MapsMetricValuesToSnapshotRows.
        Assert.Equal(2, snapshots.Count);
        // Menjalankan pemeriksaan hasil dengan `Assert.All` menggunakan `snapshots`, `snapshot => { Assert.NotEqual(Guid.Empty, snapshot.MetricSnapshotId);
        // Assert.Equal(sessionId, snapshot.SessionId); Assert.Equal(playerId, snapshot.UserId); Assert.Equal(rulese...`; ketidaksesuaian dengan ekspektasi
        // membuat pengujian gagal dalam BuildMetricSnapshots_MapsMetricValuesToSnapshotRows.
        Assert.All(snapshots, snapshot =>
        // Membuka scope fungsi lambda yang dipasok ke `Assert.All`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // BuildMetricSnapshots_MapsMetricValuesToSnapshotRows.
        {
            // Menjalankan pemeriksaan hasil dengan `Assert.NotEqual` menggunakan `Guid.Empty`, `snapshot.MetricSnapshotId`; ketidaksesuaian dengan ekspektasi
            // membuat pengujian gagal dalam BuildMetricSnapshots_MapsMetricValuesToSnapshotRows.
            Assert.NotEqual(Guid.Empty, snapshot.MetricSnapshotId);
            // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`sessionId`, `snapshot.SessionId`); pengujian
            // gagal jika keduanya berbeda dalam BuildMetricSnapshots_MapsMetricValuesToSnapshotRows.
            Assert.Equal(sessionId, snapshot.SessionId);
            // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`playerId`, `snapshot.UserId`); pengujian gagal
            // jika keduanya berbeda dalam BuildMetricSnapshots_MapsMetricValuesToSnapshotRows.
            Assert.Equal(playerId, snapshot.UserId);
            // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`rulesetVersionId`,
            // `snapshot.RulesetVersionId`); pengujian gagal jika keduanya berbeda dalam BuildMetricSnapshots_MapsMetricValuesToSnapshotRows.
            Assert.Equal(rulesetVersionId, snapshot.RulesetVersionId);
            // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`computedAt`, `snapshot.ComputedAt`); pengujian
            // gagal jika keduanya berbeda dalam BuildMetricSnapshots_MapsMetricValuesToSnapshotRows.
            Assert.Equal(computedAt, snapshot.ComputedAt);
        // Menutup scope fungsi lambda yang dipasok ke `Assert.All`; bagian berikut berada di luar batas blok tersebut dalam
        // BuildMetricSnapshots_MapsMetricValuesToSnapshotRows.
        });

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `snapshots`, `snapshot =>
        // snapshot.MetricName == ”cashflow.net.total” && snapshot.MetricValueNumeric == 7 && snapshot.MetricValueJson is null` dalam
        // BuildMetricSnapshots_MapsMetricValuesToSnapshotRows.
        Assert.Contains(snapshots, snapshot =>
            // Meneruskan fungsi lambda `snapshot => snapshot.MetricName == ”cashflow.net.total” && snapshot.MetricValueNumeric == 7 && snapshot.MetricValueJson
            // is null` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen ke `Assert.Contains`.
            snapshot.MetricName == "cashflow.net.total" &&
            // Meneruskan fungsi lambda `snapshot => snapshot.MetricName == ”cashflow.net.total” && snapshot.MetricValueNumeric == 7 && snapshot.MetricValueJson
            // is null` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen ke `Assert.Contains`.
            snapshot.MetricValueNumeric == 7 &&
            // Meneruskan fungsi lambda `snapshot => snapshot.MetricName == ”cashflow.net.total” && snapshot.MetricValueNumeric == 7 && snapshot.MetricValueJson
            // is null` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen ke `Assert.Contains`.
            snapshot.MetricValueJson is null);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `snapshots`, `snapshot =>
        // snapshot.MetricName == ”gameplay.raw.variables” && snapshot.MetricValueNumeric is null && snapshot.MetricValueJson == ”””{”coins”:20}”””` dalam
        // BuildMetricSnapshots_MapsMetricValuesToSnapshotRows.
        Assert.Contains(snapshots, snapshot =>
            // Meneruskan fungsi lambda `snapshot => snapshot.MetricName == ”gameplay.raw.variables” && snapshot.MetricValueNumeric is null &&
            // snapshot.MetricValueJson == ”””{”coins”:20}”””` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen ke
            // `Assert.Contains`.
            snapshot.MetricName == "gameplay.raw.variables" &&
            // Meneruskan fungsi lambda `snapshot => snapshot.MetricName == ”gameplay.raw.variables” && snapshot.MetricValueNumeric is null &&
            // snapshot.MetricValueJson == ”””{”coins”:20}”””` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen ke
            // `Assert.Contains`.
            snapshot.MetricValueNumeric is null &&
            // Meneruskan fungsi lambda `snapshot => snapshot.MetricName == ”gameplay.raw.variables” && snapshot.MetricValueNumeric is null &&
            // snapshot.MetricValueJson == ”””{”coins”:20}”””` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen ke
            // `Assert.Contains`.
            snapshot.MetricValueJson == """{"coins":20}""");
    // Menutup scope metode BuildMetricSnapshots_MapsMetricValuesToSnapshotRows; bagian berikut berada di luar batas blok tersebut dalam
    // BuildMetricSnapshots_MapsMetricValuesToSnapshotRows.
    }
// Menutup scope tipe AnalyticsMetricSnapshotBuilderTests; bagian berikut berada di luar batas blok tersebut.
}
