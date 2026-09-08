// Fungsi file: Menyediakan transformasi, lokalisasi, atau koneksi UI melalui PlayerMetricChartPayloadBuilder.
// Mengimpor namespace `System.Globalization` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Globalization;
// Mengimpor namespace `System.Text.Json` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.Json;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Infrastructure` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Infrastructure;

/// <summary>
/// Builder payload chart metrik pemain yang dapat diuji tanpa Razor/browser.
/// </summary>
// Mendefinisikan tipe class `PlayerMetricChartPayloadBuilder`.
public static class PlayerMetricChartPayloadBuilder
// Membuka scope tipe PlayerMetricChartPayloadBuilder; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    /// <summary>
    /// Membangun label sumber data untuk path metrik.
    /// </summary>
    // Mendefinisikan metode `BuildMetricSourceLabel` dengan hasil bertipe `string`. Membangun label sumber data untuk path metrik. Masukan: Parameter
    // `metricPath` bertipe `string?` membawa nilai metric path; nilai null diizinkan ketika data opsional belum tersedia; Parameter `isRawDomain`
    // bertipe `bool` membawa nilai berstatus raw domain; Parameter `translate` bertipe `Func<string, string>` membawa nilai translate.
    public static string BuildMetricSourceLabel(string? metricPath, bool isRawDomain, Func<string, string> translate)
    // Membuka scope metode BuildMetricSourceLabel; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildMetricSourceLabel.
    {
        // Menyiapkan variabel lokal `root` untuk nilai root dengan hasil pemilihan bersyarat: ketika `isRawDomain` benar gunakan
        // `translate(”players.details.raw_title”)`, jika tidak gunakan `translate(”players.details.derived_title”)`. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var root = isRawDomain
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: translate(”players.details.raw_title”) dalam BuildMetricSourceLabel.
            ? translate("players.details.raw_title")
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: translate(”players.details.derived_title”); dalam
            // BuildMetricSourceLabel.
            : translate("players.details.derived_title");
        // Menyiapkan variabel lokal `normalized` untuk nilai normalized dengan membersihkan karakter tepi pada `(metricPath ?? string.Empty).Trim()`
        // memakai `'.'`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var normalized = (metricPath ?? string.Empty).Trim().Trim('.');
        // Memeriksa memeriksa apakah `normalized` null, kosong, atau hanya berisi karakter spasi; blok if hanya dijalankan ketika kondisi ini bernilai
        // benar dalam BuildMetricSourceLabel.
        if (string.IsNullOrWhiteSpace(normalized))
        // Membuka scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(normalized)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // BuildMetricSourceLabel.
        {
            // Mengembalikan `root` (nilai root) kepada pemanggil dalam BuildMetricSourceLabel; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return root;
        // Menutup scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(normalized)`; bagian berikut berada di luar batas blok tersebut dalam
        // BuildMetricSourceLabel.
        }

        // Mengembalikan teks interpolasi `$”{root} -> {PlayerMetricLabelFormatter.FormatMetricPathLabel(normalized, translate)}”`; nilai ekspresi di dalam
        // kurung kurawal disisipkan saat program berjalan kepada pemanggil dalam BuildMetricSourceLabel; eksekusi jalur ini selesai setelah nilai hasil
        // ditentukan.
        return $"{root} -> {PlayerMetricLabelFormatter.FormatMetricPathLabel(normalized, translate)}";
    // Menutup scope metode BuildMetricSourceLabel; bagian berikut berada di luar batas blok tersebut dalam BuildMetricSourceLabel.
    }

    /// <summary>
    /// Membangun chart batang ringkasan gabungan dari rows numerik.
    /// </summary>
    // Mendefinisikan metode `BuildMergedRowChart` dengan hasil bertipe `(string Title, string Json)?`. Membangun chart batang ringkasan gabungan dari
    // rows numerik. Masukan: Parameter `domainTitle` bertipe `string` membawa nilai domain title; Parameter `rows` bertipe `IEnumerable<(string Path,
    // string Value)>` membawa nilai baris; Parameter `isRawDomain` bertipe `bool` membawa nilai berstatus raw domain; Parameter `translate` bertipe
    // `Func<string, string>` membawa nilai translate.
    public static (string Title, string Json)? BuildMergedRowChart(
        // Parameter `domainTitle` bertipe `string` membawa nilai domain title.
        string domainTitle,
        // Parameter `rows` bertipe `IEnumerable<(string Path, string Value)>` membawa nilai baris.
        IEnumerable<(string Path, string Value)> rows,
        // Parameter `isRawDomain` bertipe `bool` membawa nilai berstatus raw domain.
        bool isRawDomain,
        // Parameter `translate` bertipe `Func<string, string>` membawa nilai translate.
        Func<string, string> translate)
    // Membuka scope metode BuildMergedRowChart; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildMergedRowChart.
    {
        // Menyiapkan variabel lokal `candidateRows` untuk nilai candidate baris dengan mematerialisasi urutan `rows .Where(row =>
        // !string.IsNullOrWhiteSpace(row.Path))` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var candidateRows = rows
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(row => !string.IsNullOrWhiteSpace(row.Path)) dalam BuildMergedRowChart;
            // token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Where(row => !string.IsNullOrWhiteSpace(row.Path))
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList(); dalam BuildMergedRowChart; token pada baris ini menyambungkan bagian
            // kode sebelum dan sesudahnya.
            .ToList();
        // Menyiapkan variabel lokal `numericRows` untuk nilai numerik baris dengan mematerialisasi urutan `candidateRows .Where(row =>
        // PlayerMetricLabelFormatter.TryParseMetricNumber(row.Value, out _))` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var numericRows = candidateRows
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(row => PlayerMetricLabelFormatter.TryParseMetricNumber(row.Value, out
            // _)) dalam BuildMergedRowChart; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Where(row => PlayerMetricLabelFormatter.TryParseMetricNumber(row.Value, out _))
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList(); dalam BuildMergedRowChart; token pada baris ini menyambungkan bagian
            // kode sebelum dan sesudahnya.
            .ToList();

        // Menyiapkan variabel lokal `summaryRows` untuk nilai summary baris dengan memanggil `SelectSummaryRows` dengan `numericRows`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var summaryRows = SelectSummaryRows(numericRows);
        // Menyiapkan variabel lokal `useZeroFallback` untuk nilai use zero fallback dengan false, yaitu kondisi nonaktif/tidak terpenuhi. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var useZeroFallback = false;
        // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `summaryRows.Count == 0` dan `numericRows.Count > 0`; sisi kanan diperiksa hanya
        // jika sisi kiri benar; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam BuildMergedRowChart.
        if (summaryRows.Count == 0 && numericRows.Count > 0)
        // Membuka scope cabang if untuk kondisi `summaryRows.Count == 0 && numericRows.Count > 0`; pernyataan/deklarasi berikut berada di dalam batas blok
        // ini dalam BuildMergedRowChart.
        {
            // Memperbarui `summaryRows` menggunakan mematerialisasi urutan `numericRows.Take(8)` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam
            // memori dalam BuildMergedRowChart.
            summaryRows = numericRows.Take(8).ToList();
        // Menutup scope cabang if untuk kondisi `summaryRows.Count == 0 && numericRows.Count > 0`; bagian berikut berada di luar batas blok tersebut dalam
        // BuildMergedRowChart.
        }

        // Memeriksa perbandingan kesamaan antara `summaryRows.Count` dan `0`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // BuildMergedRowChart.
        if (summaryRows.Count == 0)
        // Membuka scope cabang if untuk kondisi `summaryRows.Count == 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // BuildMergedRowChart.
        {
            // Memperbarui `summaryRows` menggunakan memanggil `SelectSummaryRows` dengan `candidateRows` dalam BuildMergedRowChart.
            summaryRows = SelectSummaryRows(candidateRows);
            // Memperbarui `useZeroFallback` menggunakan pemeriksaan lebih besar antara `summaryRows.Count` dan `0` dalam BuildMergedRowChart.
            useZeroFallback = summaryRows.Count > 0;
        // Menutup scope cabang if untuk kondisi `summaryRows.Count == 0`; bagian berikut berada di luar batas blok tersebut dalam BuildMergedRowChart.
        }

        // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `summaryRows.Count == 0` dan `candidateRows.Count > 0`; sisi kanan diperiksa
        // hanya jika sisi kiri benar; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam BuildMergedRowChart.
        if (summaryRows.Count == 0 && candidateRows.Count > 0)
        // Membuka scope cabang if untuk kondisi `summaryRows.Count == 0 && candidateRows.Count > 0`; pernyataan/deklarasi berikut berada di dalam batas
        // blok ini dalam BuildMergedRowChart.
        {
            // Memperbarui `summaryRows` menggunakan mematerialisasi urutan `candidateRows.Take(8)` menjadi List; enumerasi dijalankan dan hasilnya disimpan
            // dalam memori dalam BuildMergedRowChart.
            summaryRows = candidateRows.Take(8).ToList();
            // Memperbarui `useZeroFallback` menggunakan true, yaitu kondisi aktif/terpenuhi dalam BuildMergedRowChart.
            useZeroFallback = true;
        // Menutup scope cabang if untuk kondisi `summaryRows.Count == 0 && candidateRows.Count > 0`; bagian berikut berada di luar batas blok tersebut
        // dalam BuildMergedRowChart.
        }

        // Menyiapkan variabel lokal `points` untuk nilai poin dengan memanggil `BuildChartPoints` dengan `summaryRows`, `isRawDomain`, `translate`,
        // `useZeroFallback`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var points = BuildChartPoints(summaryRows, isRawDomain, translate, useZeroFallback);
        // Memeriksa perbandingan kesamaan antara `points.Count` dan `0`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // BuildMergedRowChart.
        if (points.Count == 0)
        // Membuka scope cabang if untuk kondisi `points.Count == 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildMergedRowChart.
        {
            // Memperbarui `points` menggunakan koleksi berisi ( string.Empty, translate(”players.details.metric_... dalam BuildMergedRowChart.
            points =
            // Menggunakan koleksi berisi ( string.Empty, translate(”players.details.metric_... sebagai bagian ekspresi yang sedang disusun dalam
            // BuildMergedRowChart.
            [
                // Menggunakan tuple yang membawa bagian 1: string.Empty; bagian 2: translate(”players.details.metric_fallback”); bagian 3: 0d; bagian 4:
                // BuildSummaryFallbackDetail(isRawDomain, translate) sebagai bagian ekspresi yang sedang disusun dalam BuildMergedRowChart.
                (
                    // Meneruskan `string.Empty`, yaitu nilai kosong bawaan tipe terkait sebagai argumen ke `BuildMergedRowChart`.
                    string.Empty,
                    // Meneruskan memanggil `translate` dengan `”players.details.metric_fallback”` sebagai argumen ke `BuildMergedRowChart`; Meneruskan nilai literal
                    // `”players.details.metric_fallback”` sebagai argumen ke `translate`.
                    translate("players.details.metric_fallback"),
                    // Meneruskan nilai literal `0d` sebagai argumen ke `BuildMergedRowChart`.
                    0d,
                    // Meneruskan memanggil `BuildSummaryFallbackDetail` dengan `isRawDomain`, `translate` sebagai argumen ke `BuildMergedRowChart`; Meneruskan
                    // `isRawDomain` (nilai berstatus raw domain) sebagai argumen ke `BuildSummaryFallbackDetail`; Meneruskan `translate` (nilai translate) sebagai
                    // argumen ke `BuildSummaryFallbackDetail`.
                    BuildSummaryFallbackDetail(isRawDomain, translate))
            // Menandai akhir daftar elemen atau indeks koleksi dalam BuildMergedRowChart; pasangan kurung siku mengelompokkan nilai sebagai satu struktur.
            ];
        // Menutup scope cabang if untuk kondisi `points.Count == 0`; bagian berikut berada di luar batas blok tersebut dalam BuildMergedRowChart.
        }

        // Menyiapkan variabel lokal `maxPoints` untuk nilai maksimum poin dengan nilai literal `36`. Tipe yang dipakai adalah `int`.
        const int maxPoints = 36;
        // Memeriksa pemeriksaan lebih besar antara `points.Count` dan `maxPoints`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // BuildMergedRowChart.
        if (points.Count > maxPoints)
        // Membuka scope cabang if untuk kondisi `points.Count > maxPoints`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // BuildMergedRowChart.
        {
            // Memperbarui `points` menggunakan mematerialisasi urutan `points.Take(maxPoints)` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam
            // memori dalam BuildMergedRowChart.
            points = points.Take(maxPoints).ToList();
        // Menutup scope cabang if untuk kondisi `points.Count > maxPoints`; bagian berikut berada di luar batas blok tersebut dalam BuildMergedRowChart.
        }

        // Menyiapkan variabel lokal `payload` untuk muatan detail event dalam format JSON dengan objek anonim yang mengelompokkan chartType, labels, keys,
        // formulas, detailLabel, detailFallback, series sebagai satu nilai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var payload = new
        // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // BuildMergedRowChart.
        {
            // Menggunakan `chartType` (nilai chart jenis) sebagai bagian ekspresi yang sedang disusun dalam BuildMergedRowChart.
            chartType = "bar",
            // Menggunakan `labels` (nilai labels) sebagai bagian ekspresi yang sedang disusun dalam BuildMergedRowChart.
            labels = points.Select(point => point.Label).ToList(),
            // Menggunakan `keys` (nilai kunci) sebagai bagian ekspresi yang sedang disusun dalam BuildMergedRowChart.
            keys = points.Select(point => point.Path).ToList(),
            // Menggunakan `formulas` (nilai formulas) sebagai bagian ekspresi yang sedang disusun dalam BuildMergedRowChart.
            formulas = points.Select(point => point.Formula).ToList(),
            // Menggunakan `detailLabel` (nilai detail label) sebagai bagian ekspresi yang sedang disusun dalam BuildMergedRowChart.
            detailLabel = translate("players.details.source_calc_label"),
            // Menggunakan `detailFallback` (nilai detail fallback) sebagai bagian ekspresi yang sedang disusun dalam BuildMergedRowChart.
            detailFallback = isRawDomain
                // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: translate(”players.details.source_raw_summary”) dalam
                // BuildMergedRowChart.
                ? translate("players.details.source_raw_summary")
                // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: translate(”players.details.source_derived_summary”), dalam
                // BuildMergedRowChart.
                : translate("players.details.source_derived_summary"),
            // Menggunakan `series` (nilai series) sebagai bagian ekspresi yang sedang disusun dalam BuildMergedRowChart.
            series = new[]
            // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildMergedRowChart.
            {
                // Menggunakan objek anonim yang mengelompokkan name, values sebagai satu nilai sebagai bagian ekspresi yang sedang disusun dalam
                // BuildMergedRowChart.
                new
                // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // BuildMergedRowChart.
                {
                    // Menggunakan `name` (nilai nama) sebagai bagian ekspresi yang sedang disusun dalam BuildMergedRowChart.
                    name = translate("common.value"),
                    // Menggunakan `values` (nilai nilai) sebagai bagian ekspresi yang sedang disusun dalam BuildMergedRowChart.
                    values = points.Select(point => (double?)point.Value).ToList()
                // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam BuildMergedRowChart.
                }
            // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam BuildMergedRowChart.
            }
        // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam BuildMergedRowChart.
        };

        // Mengembalikan tuple yang membawa bagian 1: $”{domainTitle}: {translate(”players.details.combined_snapshot”)}”; bagian 2:
        // JsonSerializer.Serialize(payload) kepada pemanggil dalam BuildMergedRowChart; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return ($"{domainTitle}: {translate("players.details.combined_snapshot")}", JsonSerializer.Serialize(payload));
    // Menutup scope metode BuildMergedRowChart; bagian berikut berada di luar batas blok tersebut dalam BuildMergedRowChart.
    }

    // Mendefinisikan metode `SelectSummaryRows` dengan hasil bertipe `List<(string Path, string Value)>`; operasi ini menangani select summary baris.
    // Masukan: Parameter `rows` bertipe `IEnumerable<(string Path, string Value)>` membawa nilai baris.
    private static List<(string Path, string Value)> SelectSummaryRows(IEnumerable<(string Path, string Value)> rows)
    // Membuka scope metode SelectSummaryRows; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam SelectSummaryRows.
    {
        // Menyiapkan variabel lokal `preferredRows` untuk nilai preferred baris dengan mematerialisasi urutan `rows .Where(row =>
        // PlayerMetricLabelFormatter.IsPreferredCombinedSummaryPath(row.Path))` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var preferredRows = rows
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(row =>
            // PlayerMetricLabelFormatter.IsPreferredCombinedSummaryPath(row.Path)) dalam SelectSummaryRows; token pada baris ini menyambungkan bagian kode
            // sebelum dan sesudahnya.
            .Where(row => PlayerMetricLabelFormatter.IsPreferredCombinedSummaryPath(row.Path))
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList(); dalam SelectSummaryRows; token pada baris ini menyambungkan bagian
            // kode sebelum dan sesudahnya.
            .ToList();
        // Memeriksa pemeriksaan lebih besar antara `preferredRows.Count` dan `0`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // SelectSummaryRows.
        if (preferredRows.Count > 0)
        // Membuka scope cabang if untuk kondisi `preferredRows.Count > 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // SelectSummaryRows.
        {
            // Mengembalikan `preferredRows` (nilai preferred baris) kepada pemanggil dalam SelectSummaryRows; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return preferredRows;
        // Menutup scope cabang if untuk kondisi `preferredRows.Count > 0`; bagian berikut berada di luar batas blok tersebut dalam SelectSummaryRows.
        }

        // Mengembalikan mematerialisasi urutan `rows .Where(row => PlayerMetricLabelFormatter.IsFallbackCombinedSummaryPath(row.Path))` menjadi List;
        // enumerasi dijalankan dan hasilnya disimpan dalam memori kepada pemanggil dalam SelectSummaryRows; eksekusi jalur ini selesai setelah nilai hasil
        // ditentukan.
        return rows
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(row =>
            // PlayerMetricLabelFormatter.IsFallbackCombinedSummaryPath(row.Path)) dalam SelectSummaryRows; token pada baris ini menyambungkan bagian kode
            // sebelum dan sesudahnya.
            .Where(row => PlayerMetricLabelFormatter.IsFallbackCombinedSummaryPath(row.Path))
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList(); dalam SelectSummaryRows; token pada baris ini menyambungkan bagian
            // kode sebelum dan sesudahnya.
            .ToList();
    // Menutup scope metode SelectSummaryRows; bagian berikut berada di luar batas blok tersebut dalam SelectSummaryRows.
    }

    // Mendefinisikan metode `BuildChartPoints` dengan hasil bertipe `List<(string Path, string Label, double Value, string Formula)>`; operasi ini
    // menangani build chart poin. Masukan: Parameter `rows` bertipe `IEnumerable<(string Path, string Value)>` membawa nilai baris; Parameter
    // `isRawDomain` bertipe `bool` membawa nilai berstatus raw domain; Parameter `translate` bertipe `Func<string, string>` membawa nilai translate;
    // Parameter `allowZeroFallback` bertipe `bool` membawa nilai allow zero fallback.
    private static List<(string Path, string Label, double Value, string Formula)> BuildChartPoints(
        // Parameter `rows` bertipe `IEnumerable<(string Path, string Value)>` membawa nilai baris.
        IEnumerable<(string Path, string Value)> rows,
        // Parameter `isRawDomain` bertipe `bool` membawa nilai berstatus raw domain.
        bool isRawDomain,
        // Parameter `translate` bertipe `Func<string, string>` membawa nilai translate.
        Func<string, string> translate,
        // Parameter `allowZeroFallback` bertipe `bool` membawa nilai allow zero fallback.
        bool allowZeroFallback)
    // Membuka scope metode BuildChartPoints; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildChartPoints.
    {
        // Menyiapkan variabel lokal `points` untuk nilai poin dengan objek baru bertipe `List<(string Path, string Label, double Value, string Formula)>`
        // dengan nilai awal sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var points = new List<(string Path, string Label, double Value, string Formula)>();
        // Menyiapkan variabel lokal `labelUsage` untuk nilai label usage dengan objek baru bertipe `Dictionary<string, int>` dengan argumen
        // (StringComparer.OrdinalIgnoreCase). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var labelUsage = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        // Mengulangi setiap elemen `rows`; elemen saat ini disimpan sebagai `row` bertipe `var` untuk diproses oleh badan loop dalam BuildChartPoints.
        foreach (var row in rows)
        // Membuka scope loop setiap row dari `rows`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildChartPoints.
        {
            // Menyiapkan variabel lokal `hasNumericValue` untuk nilai memiliki numerik nilai dengan memanggil `PlayerMetricLabelFormatter.TryParseMetricNumber`
            // dengan `row.Value`, `var value`. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var hasNumericValue = PlayerMetricLabelFormatter.TryParseMetricNumber(row.Value, out var value);
            // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `!hasNumericValue` dan `!allowZeroFallback`; sisi kanan diperiksa hanya jika sisi
            // kiri benar; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam BuildChartPoints.
            if (!hasNumericValue && !allowZeroFallback)
            // Membuka scope cabang if untuk kondisi `!hasNumericValue && !allowZeroFallback`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // BuildChartPoints.
            {
                // Melewati sisa pernyataan pada iterasi saat ini dan melanjutkan ke elemen/iterasi berikutnya dalam BuildChartPoints.
                continue;
            // Menutup scope cabang if untuk kondisi `!hasNumericValue && !allowZeroFallback`; bagian berikut berada di luar batas blok tersebut dalam
            // BuildChartPoints.
            }

            // Menyiapkan variabel lokal `resolvedValue` untuk nilai hasil resolusi nilai dengan hasil pemilihan bersyarat: ketika `hasNumericValue` benar
            // gunakan `value`, jika tidak gunakan `0d`. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var resolvedValue = hasNumericValue ? value : 0d;
            // Menyiapkan variabel lokal `baseLabel` untuk nilai base label dengan membersihkan karakter tepi pada
            // `PlayerMetricLabelFormatter.FormatMetricPathLabel(row.Path, translate)` memakai tanpa argumen. Tipe variabel disimpulkan dari ekspresi nilai
            // awal.
            var baseLabel = PlayerMetricLabelFormatter.FormatMetricPathLabel(row.Path, translate).Trim();
            // Memeriksa memeriksa apakah `baseLabel` null, kosong, atau hanya berisi karakter spasi; blok if hanya dijalankan ketika kondisi ini bernilai benar
            // dalam BuildChartPoints.
            if (string.IsNullOrWhiteSpace(baseLabel))
            // Membuka scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(baseLabel)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // BuildChartPoints.
            {
                // Memperbarui `baseLabel` menggunakan memanggil `translate` dengan `”players.details.metric_fallback”` dalam BuildChartPoints.
                baseLabel = translate("players.details.metric_fallback");
            // Menutup scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(baseLabel)`; bagian berikut berada di luar batas blok tersebut dalam
            // BuildChartPoints.
            }

            // Memeriksa kebalikan kondisi `labelUsage.TryGetValue(baseLabel, out var usageCount)`; blok if hanya dijalankan ketika kondisi ini bernilai benar
            // dalam BuildChartPoints.
            if (!labelUsage.TryGetValue(baseLabel, out var usageCount))
            // Membuka scope cabang if untuk kondisi `!labelUsage.TryGetValue(baseLabel, out var usageCount)`; pernyataan/deklarasi berikut berada di dalam
            // batas blok ini dalam BuildChartPoints.
            {
                // Memperbarui `labelUsage[baseLabel]` menggunakan nilai literal `1` dalam BuildChartPoints.
                labelUsage[baseLabel] = 1;
                // Menjalankan menambahkan `(row.Path, baseLabel, resolvedValue, BuildFormulaHint(row.Path, isRawDomain, translate))` ke `points` dalam
                // BuildChartPoints.
                points.Add((row.Path, baseLabel, resolvedValue, BuildFormulaHint(row.Path, isRawDomain, translate)));
                // Melewati sisa pernyataan pada iterasi saat ini dan melanjutkan ke elemen/iterasi berikutnya dalam BuildChartPoints.
                continue;
            // Menutup scope cabang if untuk kondisi `!labelUsage.TryGetValue(baseLabel, out var usageCount)`; bagian berikut berada di luar batas blok tersebut
            // dalam BuildChartPoints.
            }

            // Memperbarui `usageCount` dengan menambahkan nilai literal `1` dalam BuildChartPoints.
            usageCount += 1;
            // Memperbarui `labelUsage[baseLabel]` menggunakan `usageCount` (nilai usage jumlah) dalam BuildChartPoints.
            labelUsage[baseLabel] = usageCount;
            // Menjalankan menambahkan `(row.Path, $”{baseLabel} ({usageCount})”, resolvedValue, BuildFormulaHint(row.Path, isRawDomain, translate))` ke
            // `points` dalam BuildChartPoints.
            points.Add((row.Path, $"{baseLabel} ({usageCount})", resolvedValue, BuildFormulaHint(row.Path, isRawDomain, translate)));
        // Menutup scope loop setiap row dari `rows`; bagian berikut berada di luar batas blok tersebut dalam BuildChartPoints.
        }

        // Mengembalikan `points` (nilai poin) kepada pemanggil dalam BuildChartPoints; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return points;
    // Menutup scope metode BuildChartPoints; bagian berikut berada di luar batas blok tersebut dalam BuildChartPoints.
    }

    /// <summary>
    /// Membangun semua chart line dari properti object group yang renderable.
    /// </summary>
    // Mendefinisikan metode `BuildLineChartsForGroup` dengan hasil bertipe `List<(string Title, string Json)>`. Membangun semua chart line dari
    // properti object group yang renderable. Masukan: Parameter `groupKey` bertipe `string` membawa nilai group kunci; Parameter `groupElement` bertipe
    // `JsonElement` membawa nilai group element; Parameter `isRawDomain` bertipe `bool` membawa nilai berstatus raw domain; Parameter `translate`
    // bertipe `Func<string, string>` membawa nilai translate.
    public static List<(string Title, string Json)> BuildLineChartsForGroup(
        // Parameter `groupKey` bertipe `string` membawa nilai group kunci.
        string groupKey,
        // Parameter `groupElement` bertipe `JsonElement` membawa nilai group element.
        JsonElement groupElement,
        // Parameter `isRawDomain` bertipe `bool` membawa nilai berstatus raw domain.
        bool isRawDomain,
        // Parameter `translate` bertipe `Func<string, string>` membawa nilai translate.
        Func<string, string> translate)
    // Membuka scope metode BuildLineChartsForGroup; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildLineChartsForGroup.
    {
        // Menyiapkan variabel lokal `charts` untuk nilai charts dengan objek baru bertipe `List<(string Title, string Json)>` dengan nilai awal sesuai
        // konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var charts = new List<(string Title, string Json)>();
        // Memeriksa perbandingan ketidaksamaan antara `groupElement.ValueKind` dan `JsonValueKind.Object`; blok if hanya dijalankan ketika kondisi ini
        // bernilai benar dalam BuildLineChartsForGroup.
        if (groupElement.ValueKind != JsonValueKind.Object)
        // Membuka scope cabang if untuk kondisi `groupElement.ValueKind != JsonValueKind.Object`; pernyataan/deklarasi berikut berada di dalam batas blok
        // ini dalam BuildLineChartsForGroup.
        {
            // Mengembalikan `charts` (nilai charts) kepada pemanggil dalam BuildLineChartsForGroup; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return charts;
        // Menutup scope cabang if untuk kondisi `groupElement.ValueKind != JsonValueKind.Object`; bagian berikut berada di luar batas blok tersebut dalam
        // BuildLineChartsForGroup.
        }

        // Mengulangi setiap elemen `groupElement.EnumerateObject()`; elemen saat ini disimpan sebagai `property` bertipe `var` untuk diproses oleh badan
        // loop dalam BuildLineChartsForGroup.
        foreach (var property in groupElement.EnumerateObject())
        // Membuka scope loop setiap property dari `groupElement.EnumerateObject()`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // BuildLineChartsForGroup.
        {
            // Menyiapkan variabel lokal `chart` untuk nilai chart dengan memanggil `BuildLineChartPayload` dengan `$”{groupKey}.{property.Name}”`,
            // `property.Value`, `isRawDomain`, `translate`. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var chart = BuildLineChartPayload($"{groupKey}.{property.Name}", property.Value, isRawDomain, translate);
            // Memeriksa `chart.HasValue`, yaitu penanda bahwa nilai nullable tidak kosong; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
            // BuildLineChartsForGroup.
            if (chart.HasValue)
            // Membuka scope cabang if untuk kondisi `chart.HasValue`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // BuildLineChartsForGroup.
            {
                // Menjalankan menambahkan `chart.Value` ke `charts` dalam BuildLineChartsForGroup.
                charts.Add(chart.Value);
            // Menutup scope cabang if untuk kondisi `chart.HasValue`; bagian berikut berada di luar batas blok tersebut dalam BuildLineChartsForGroup.
            }
        // Menutup scope loop setiap property dari `groupElement.EnumerateObject()`; bagian berikut berada di luar batas blok tersebut dalam
        // BuildLineChartsForGroup.
        }

        // Mengembalikan `charts` (nilai charts) kepada pemanggil dalam BuildLineChartsForGroup; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return charts;
    // Menutup scope metode BuildLineChartsForGroup; bagian berikut berada di luar batas blok tersebut dalam BuildLineChartsForGroup.
    }

    /// <summary>
    /// Membangun payload chart line/bar dari JsonElement array atau object numerik.
    /// </summary>
    // Mendefinisikan metode `BuildLineChartPayload` dengan hasil bertipe `(string Title, string Json)?`. Membangun payload chart line/bar dari
    // JsonElement array atau object numerik. Masukan: Parameter `path` bertipe `string` membawa nilai path; Parameter `element` bertipe `JsonElement`
    // membawa nilai element; Parameter `isRawDomain` bertipe `bool` membawa nilai berstatus raw domain; Parameter `translate` bertipe `Func<string,
    // string>` membawa nilai translate.
    public static (string Title, string Json)? BuildLineChartPayload(
        // Parameter `path` bertipe `string` membawa nilai path.
        string path,
        // Parameter `element` bertipe `JsonElement` membawa nilai element.
        JsonElement element,
        // Parameter `isRawDomain` bertipe `bool` membawa nilai berstatus raw domain.
        bool isRawDomain,
        // Parameter `translate` bertipe `Func<string, string>` membawa nilai translate.
        Func<string, string> translate)
    // Membuka scope metode BuildLineChartPayload; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildLineChartPayload.
    {
        // Menyiapkan variabel lokal `labels` untuk nilai labels dengan objek baru bertipe `List<string>` dengan nilai awal sesuai konstruktornya. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var labels = new List<string>();
        // Menyiapkan variabel lokal `series` untuk nilai series dengan objek baru bertipe `Dictionary<string, List<double?>>` dengan argumen
        // (StringComparer.OrdinalIgnoreCase). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var series = new Dictionary<string, List<double?>>(StringComparer.OrdinalIgnoreCase);

        // Mendefinisikan fungsi lokal AppendLabel dengan hasil `void`; fungsi ini dipakai oleh alur di dalam scope yang sama.
        void AppendLabel(string label)
        // Membuka scope fungsi lokal AppendLabel; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam AppendLabel.
        {
            // Menjalankan menambahkan `label` ke `labels` dalam AppendLabel.
            labels.Add(label);
            // Mengulangi setiap elemen `series.Values`; elemen saat ini disimpan sebagai `values` bertipe `var` untuk diproses oleh badan loop dalam
            // AppendLabel.
            foreach (var values in series.Values)
            // Membuka scope loop setiap values dari `series.Values`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam AppendLabel.
            {
                // Menjalankan menambahkan `null` ke `values` dalam AppendLabel.
                values.Add(null);
            // Menutup scope loop setiap values dari `series.Values`; bagian berikut berada di luar batas blok tersebut dalam AppendLabel.
            }
        // Menutup scope fungsi lokal AppendLabel; bagian berikut berada di luar batas blok tersebut dalam AppendLabel.
        }

        // Mendefinisikan fungsi lokal SetSeriesValue dengan hasil `void`; fungsi ini dipakai oleh alur di dalam scope yang sama.
        void SetSeriesValue(string seriesName, double value)
        // Membuka scope fungsi lokal SetSeriesValue; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam SetSeriesValue.
        {
            // Memeriksa kebalikan kondisi `series.TryGetValue(seriesName, out var values)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
            // SetSeriesValue.
            if (!series.TryGetValue(seriesName, out var values))
            // Membuka scope cabang if untuk kondisi `!series.TryGetValue(seriesName, out var values)`; pernyataan/deklarasi berikut berada di dalam batas blok
            // ini dalam SetSeriesValue.
            {
                // Memperbarui `values` menggunakan mematerialisasi urutan `Enumerable.Repeat<double?>(null, labels.Count)` menjadi List; enumerasi dijalankan dan
                // hasilnya disimpan dalam memori dalam SetSeriesValue.
                values = Enumerable.Repeat<double?>(null, labels.Count).ToList();
                // Memperbarui `series[seriesName]` menggunakan `values` (nilai nilai) dalam SetSeriesValue.
                series[seriesName] = values;
            // Menutup scope cabang if untuk kondisi `!series.TryGetValue(seriesName, out var values)`; bagian berikut berada di luar batas blok tersebut dalam
            // SetSeriesValue.
            }

            // Memperbarui `values[labels.Count - 1]` menggunakan `value` (nilai nilai) dalam SetSeriesValue.
            values[labels.Count - 1] = value;
        // Menutup scope fungsi lokal SetSeriesValue; bagian berikut berada di luar batas blok tersebut dalam SetSeriesValue.
        }

        // Memeriksa perbandingan kesamaan antara `element.ValueKind` dan `JsonValueKind.Array`; blok if hanya dijalankan ketika kondisi ini bernilai benar
        // dalam BuildLineChartPayload.
        if (element.ValueKind == JsonValueKind.Array)
        // Membuka scope cabang if untuk kondisi `element.ValueKind == JsonValueKind.Array`; pernyataan/deklarasi berikut berada di dalam batas blok ini
        // dalam BuildLineChartPayload.
        {
            // Menyiapkan variabel lokal `items` untuk nilai elemen dengan mematerialisasi urutan `element.EnumerateArray()` menjadi List; enumerasi dijalankan
            // dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var items = element.EnumerateArray().ToList();
            // Memeriksa perbandingan kesamaan antara `items.Count` dan `0`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
            // BuildLineChartPayload.
            if (items.Count == 0)
            // Membuka scope cabang if untuk kondisi `items.Count == 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // BuildLineChartPayload.
            {
                // Mengembalikan null, yaitu penanda tidak ada nilai kepada pemanggil dalam BuildLineChartPayload; eksekusi jalur ini selesai setelah nilai hasil
                // ditentukan.
                return null;
            // Menutup scope cabang if untuk kondisi `items.Count == 0`; bagian berikut berada di luar batas blok tersebut dalam BuildLineChartPayload.
            }

            // Memeriksa memeriksa apakah seluruh elemen `items` memenuhi `item => PlayerMetricJsonMapper.TryGetNumericValue(item, out _)`; koleksi kosong
            // menghasilkan true; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam BuildLineChartPayload.
            if (items.All(item => PlayerMetricJsonMapper.TryGetNumericValue(item, out _)))
            // Membuka scope cabang if untuk kondisi `items.All(item => PlayerMetricJsonMapper.TryGetNumericValue(item, out _))`; pernyataan/deklarasi berikut
            // berada di dalam batas blok ini dalam BuildLineChartPayload.
            {
                // Memulai loop dengan inisialisasi `var index = 0`, berjalan selama `index < items.Count`, lalu memperbarui pencacah melalui `index++` dalam
                // BuildLineChartPayload.
                for (var index = 0; index < items.Count; index++)
                // Membuka scope loop dengan syarat `index < items.Count`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildLineChartPayload.
                {
                    // Menjalankan memanggil `AppendLabel` dengan `(index + 1).ToString()` dalam BuildLineChartPayload.
                    AppendLabel((index + 1).ToString());
                    // Memeriksa memanggil `PlayerMetricJsonMapper.TryGetNumericValue` dengan `items[index]`, `var numericValue`; blok if hanya dijalankan ketika
                    // kondisi ini bernilai benar dalam BuildLineChartPayload.
                    if (PlayerMetricJsonMapper.TryGetNumericValue(items[index], out var numericValue))
                    // Membuka scope cabang if untuk kondisi `PlayerMetricJsonMapper.TryGetNumericValue(items[index], out var numericValue)`; pernyataan/deklarasi
                    // berikut berada di dalam batas blok ini dalam BuildLineChartPayload.
                    {
                        // Menjalankan memanggil `SetSeriesValue` dengan `”value”`, `numericValue` dalam BuildLineChartPayload.
                        SetSeriesValue("value", numericValue);
                    // Menutup scope cabang if untuk kondisi `PlayerMetricJsonMapper.TryGetNumericValue(items[index], out var numericValue)`; bagian berikut berada di
                    // luar batas blok tersebut dalam BuildLineChartPayload.
                    }
                // Menutup scope loop dengan syarat `index < items.Count`; bagian berikut berada di luar batas blok tersebut dalam BuildLineChartPayload.
                }
            // Menutup scope cabang if untuk kondisi `items.All(item => PlayerMetricJsonMapper.TryGetNumericValue(item, out _))`; bagian berikut berada di luar
            // batas blok tersebut dalam BuildLineChartPayload.
            }
            // Menjalankan cabang alternatif ketika kondisi if sebelumnya tidak terpenuhi dalam BuildLineChartPayload.
            else if (items.All(item => item.ValueKind == JsonValueKind.Object))
            // Membuka scope cabang if untuk kondisi `items.All(item => item.ValueKind == JsonValueKind.Object)`; pernyataan/deklarasi berikut berada di dalam
            // batas blok ini dalam BuildLineChartPayload.
            {
                // Menjalankan memanggil `BuildSeriesFromObjectArray` dengan `items`, `labels`, `series`, `AppendLabel`, `SetSeriesValue`, `translate` dalam
                // BuildLineChartPayload.
                BuildSeriesFromObjectArray(items, labels, series, AppendLabel, SetSeriesValue, translate);
            // Menutup scope cabang if untuk kondisi `items.All(item => item.ValueKind == JsonValueKind.Object)`; bagian berikut berada di luar batas blok
            // tersebut dalam BuildLineChartPayload.
            }
            // Menjalankan cabang alternatif ketika kondisi if sebelumnya tidak terpenuhi dalam BuildLineChartPayload.
            else
            // Membuka scope cabang else; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildLineChartPayload.
            {
                // Mengembalikan null, yaitu penanda tidak ada nilai kepada pemanggil dalam BuildLineChartPayload; eksekusi jalur ini selesai setelah nilai hasil
                // ditentukan.
                return null;
            // Menutup scope cabang else; bagian berikut berada di luar batas blok tersebut dalam BuildLineChartPayload.
            }
        // Menutup scope cabang if untuk kondisi `element.ValueKind == JsonValueKind.Array`; bagian berikut berada di luar batas blok tersebut dalam
        // BuildLineChartPayload.
        }
        // Menjalankan cabang alternatif ketika kondisi if sebelumnya tidak terpenuhi dalam BuildLineChartPayload.
        else if (element.ValueKind == JsonValueKind.Object &&
                 // Melanjutkan pengolahan dengan memeriksa apakah `path` memuat `”_per_”`, `StringComparison.OrdinalIgnoreCase` dalam BuildLineChartPayload.
                 path.Contains("_per_", StringComparison.OrdinalIgnoreCase))
        // Membuka scope cabang if untuk kondisi `element.ValueKind == JsonValueKind.Object && path.Contains(”_per_”, StringComparison.OrdinalIgnoreCase)`;
        // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildLineChartPayload.
        {
            // Mengulangi setiap elemen `element.EnumerateObject()`; elemen saat ini disimpan sebagai `property` bertipe `var` untuk diproses oleh badan loop
            // dalam BuildLineChartPayload.
            foreach (var property in element.EnumerateObject())
            // Membuka scope loop setiap property dari `element.EnumerateObject()`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // BuildLineChartPayload.
            {
                // Memeriksa kebalikan kondisi `PlayerMetricJsonMapper.TryGetNumericValue(property.Value, out var numericValue)`; blok if hanya dijalankan ketika
                // kondisi ini bernilai benar dalam BuildLineChartPayload.
                if (!PlayerMetricJsonMapper.TryGetNumericValue(property.Value, out var numericValue))
                // Membuka scope cabang if untuk kondisi `!PlayerMetricJsonMapper.TryGetNumericValue(property.Value, out var numericValue)`; pernyataan/deklarasi
                // berikut berada di dalam batas blok ini dalam BuildLineChartPayload.
                {
                    // Melewati sisa pernyataan pada iterasi saat ini dan melanjutkan ke elemen/iterasi berikutnya dalam BuildLineChartPayload.
                    continue;
                // Menutup scope cabang if untuk kondisi `!PlayerMetricJsonMapper.TryGetNumericValue(property.Value, out var numericValue)`; bagian berikut berada
                // di luar batas blok tersebut dalam BuildLineChartPayload.
                }

                // Menjalankan memanggil `AppendLabel` dengan `PlayerMetricLabelFormatter.HumanizeMetricKey(property.Name, translate)` dalam BuildLineChartPayload.
                AppendLabel(PlayerMetricLabelFormatter.HumanizeMetricKey(property.Name, translate));
                // Menjalankan memanggil `SetSeriesValue` dengan `”value”`, `numericValue` dalam BuildLineChartPayload.
                SetSeriesValue("value", numericValue);
            // Menutup scope loop setiap property dari `element.EnumerateObject()`; bagian berikut berada di luar batas blok tersebut dalam
            // BuildLineChartPayload.
            }
        // Menutup scope cabang if untuk kondisi `element.ValueKind == JsonValueKind.Object && path.Contains(”_per_”, StringComparison.OrdinalIgnoreCase)`;
        // bagian berikut berada di luar batas blok tersebut dalam BuildLineChartPayload.
        }
        // Menjalankan cabang alternatif ketika kondisi if sebelumnya tidak terpenuhi dalam BuildLineChartPayload.
        else
        // Membuka scope cabang else; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildLineChartPayload.
        {
            // Mengembalikan null, yaitu penanda tidak ada nilai kepada pemanggil dalam BuildLineChartPayload; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return null;
        // Menutup scope cabang else; bagian berikut berada di luar batas blok tersebut dalam BuildLineChartPayload.
        }

        // Menyiapkan variabel lokal `activeSeries` untuk nilai aktif series dengan mematerialisasi urutan `series .Where(item => item.Value.Any(value =>
        // value.HasValue)) .Select(item => new { name = PlayerMetricLabelFormatter.HumanizeMetricKey(item.Key, translate), values = item.Va...` menjadi
        // List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var activeSeries = series
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(item => item.Value.Any(value => value.HasValue)) dalam
            // BuildLineChartPayload; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Where(item => item.Value.Any(value => value.HasValue))
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Select(item => new dalam BuildLineChartPayload; token pada baris ini
            // menyambungkan bagian kode sebelum dan sesudahnya.
            .Select(item => new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // BuildLineChartPayload.
            {
                // Meneruskan `item.Key` (nilai kunci) sebagai argumen ke `PlayerMetricLabelFormatter.HumanizeMetricKey`; Meneruskan `translate` (nilai translate)
                // sebagai argumen ke `PlayerMetricLabelFormatter.HumanizeMetricKey`.
                name = PlayerMetricLabelFormatter.HumanizeMetricKey(item.Key, translate),
                // Meneruskan fungsi lambda `item => new { name = PlayerMetricLabelFormatter.HumanizeMetricKey(item.Key, translate), values = item.Value }` yang
                // dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen ke `series .Where(item => item.Value.Any(value =>
                // value.HasValue)) .Select`.
                values = item.Value
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam BuildLineChartPayload.
            })
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList(); dalam BuildLineChartPayload; token pada baris ini menyambungkan
            // bagian kode sebelum dan sesudahnya.
            .ToList();
        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `labels.Count == 0` dan `activeSeries.Count == 0`; sisi kanan diperiksa
        // hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam BuildLineChartPayload.
        if (labels.Count == 0 || activeSeries.Count == 0)
        // Membuka scope cabang if untuk kondisi `labels.Count == 0 || activeSeries.Count == 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini
        // dalam BuildLineChartPayload.
        {
            // Mengembalikan null, yaitu penanda tidak ada nilai kepada pemanggil dalam BuildLineChartPayload; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return null;
        // Menutup scope cabang if untuk kondisi `labels.Count == 0 || activeSeries.Count == 0`; bagian berikut berada di luar batas blok tersebut dalam
        // BuildLineChartPayload.
        }

        // Menyiapkan variabel lokal `pathParts` untuk nilai path parts dengan memanggil `path.Split` dengan `'.'`, `StringSplitOptions.RemoveEmptyEntries`.
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var pathParts = path.Split('.', StringSplitOptions.RemoveEmptyEntries);
        // Menyiapkan variabel lokal `groupLabel` untuk nilai group label dengan hasil pemilihan bersyarat: ketika `pathParts.Length > 0` benar gunakan
        // `PlayerMetricLabelFormatter.HumanizeMetricKey(pathParts[0], translate)`, jika tidak gunakan `translate(”players.details.series”)`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var groupLabel = pathParts.Length > 0
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: PlayerMetricLabelFormatter.HumanizeMetricKey(pathParts[0], translate)
            // dalam BuildLineChartPayload.
            ? PlayerMetricLabelFormatter.HumanizeMetricKey(pathParts[0], translate)
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: translate(”players.details.series”); dalam BuildLineChartPayload.
            : translate("players.details.series");
        // Menyiapkan variabel lokal `metricLabel` untuk nilai metric label dengan hasil pemilihan bersyarat: ketika `pathParts.Length > 1` benar gunakan
        // `PlayerMetricLabelFormatter.HumanizeMetricKey(pathParts[^1], translate)`, jika tidak gunakan `PlayerMetricLabelFormatter.HumanizeMetricKey(path,
        // translate)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var metricLabel = pathParts.Length > 1
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: PlayerMetricLabelFormatter.HumanizeMetricKey(pathParts[^1],
            // translate) dalam BuildLineChartPayload.
            ? PlayerMetricLabelFormatter.HumanizeMetricKey(pathParts[^1], translate)
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: PlayerMetricLabelFormatter.HumanizeMetricKey(path, translate); dalam
            // BuildLineChartPayload.
            : PlayerMetricLabelFormatter.HumanizeMetricKey(path, translate);
        // Menyiapkan variabel lokal `title` untuk nilai title dengan teks interpolasi `$”{groupLabel}: {metricLabel}”`; nilai ekspresi di dalam kurung
        // kurawal disisipkan saat program berjalan. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var title = $"{groupLabel}: {metricLabel}";
        // Menyiapkan variabel lokal `sourcePath` untuk nilai source path dengan memanggil `BuildMetricSourceLabel` dengan `path`, `isRawDomain`,
        // `translate`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var sourcePath = BuildMetricSourceLabel(path, isRawDomain, translate);
        // Menyiapkan variabel lokal `payload` untuk muatan detail event dalam format JSON dengan objek anonim yang mengelompokkan labels, series,
        // detailLabel, detailFallback sebagai satu nilai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var payload = new
        // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // BuildLineChartPayload.
        {
            // Menggunakan `labels` (nilai labels) sebagai bagian ekspresi yang sedang disusun dalam BuildLineChartPayload.
            labels,
            // Menggunakan `series` (nilai series) sebagai bagian ekspresi yang sedang disusun dalam BuildLineChartPayload.
            series = activeSeries,
            // Menggunakan `detailLabel` (nilai detail label) sebagai bagian ekspresi yang sedang disusun dalam BuildLineChartPayload.
            detailLabel = translate("players.details.source_calc_label"),
            // Menggunakan `detailFallback` (nilai detail fallback) sebagai bagian ekspresi yang sedang disusun dalam BuildLineChartPayload.
            detailFallback = isRawDomain
                // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: string.Format( dalam BuildLineChartPayload.
                ? string.Format(
                    // Meneruskan `CultureInfo.CurrentCulture` (nilai saat ini culture) sebagai argumen ke `string.Format`.
                    CultureInfo.CurrentCulture,
                    // Meneruskan memanggil `translate` dengan `”players.details.source_line_raw_template”` sebagai argumen ke `string.Format`; Meneruskan nilai literal
                    // `”players.details.source_line_raw_template”` sebagai argumen ke `translate`.
                    translate("players.details.source_line_raw_template"),
                    // Meneruskan `sourcePath` (nilai source path) sebagai argumen ke `string.Format`.
                    sourcePath)
                // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: string.Format( dalam BuildLineChartPayload.
                : string.Format(
                    // Meneruskan `CultureInfo.CurrentCulture` (nilai saat ini culture) sebagai argumen ke `string.Format`.
                    CultureInfo.CurrentCulture,
                    // Meneruskan memanggil `translate` dengan `”players.details.source_line_derived_template”` sebagai argumen ke `string.Format`; Meneruskan nilai
                    // literal `”players.details.source_line_derived_template”` sebagai argumen ke `translate`.
                    translate("players.details.source_line_derived_template"),
                    // Meneruskan `sourcePath` (nilai source path) sebagai argumen ke `string.Format`.
                    sourcePath)
        // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam BuildLineChartPayload.
        };

        // Mengembalikan tuple yang membawa bagian 1: title; bagian 2: JsonSerializer.Serialize(payload) kepada pemanggil dalam BuildLineChartPayload;
        // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return (title, JsonSerializer.Serialize(payload));
    // Menutup scope metode BuildLineChartPayload; bagian berikut berada di luar batas blok tersebut dalam BuildLineChartPayload.
    }

    // Mendefinisikan metode `BuildFormulaHint` dengan hasil bertipe `string`; operasi ini menangani build formula hint. Masukan: Parameter `metricPath`
    // bertipe `string` membawa nilai metric path; Parameter `isRawDomain` bertipe `bool` membawa nilai berstatus raw domain; Parameter `translate`
    // bertipe `Func<string, string>` membawa nilai translate.
    private static string BuildFormulaHint(string metricPath, bool isRawDomain, Func<string, string> translate)
    // Membuka scope metode BuildFormulaHint; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildFormulaHint.
    {
        // Menyiapkan variabel lokal `key` untuk nilai kunci dengan menormalisasi `(metricPath ?? string.Empty)` menjadi huruf kecil dengan aturan kultur
        // invariant. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var key = (metricPath ?? string.Empty).ToLowerInvariant();
        // Menyiapkan variabel lokal `sourcePath` untuk nilai source path dengan memanggil `BuildMetricSourceLabel` dengan `metricPath`, `isRawDomain`,
        // `translate`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var sourcePath = BuildMetricSourceLabel(metricPath, isRawDomain, translate);

        // Mendefinisikan fungsi lokal WithSource dengan hasil `string`; fungsi ini dipakai oleh alur di dalam scope yang sama.
        string WithSource(string formulaKey)
        // Membuka scope fungsi lokal WithSource; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam WithSource.
        {
            // Mengembalikan memanggil `string.Format` dengan `CultureInfo.CurrentCulture`, `translate(”players.details.source_calc_template”)`, `sourcePath`,
            // `translate(formulaKey)` kepada pemanggil dalam WithSource; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return string.Format(
                // Meneruskan `CultureInfo.CurrentCulture` (nilai saat ini culture) sebagai argumen ke `string.Format`.
                CultureInfo.CurrentCulture,
                // Meneruskan memanggil `translate` dengan `”players.details.source_calc_template”` sebagai argumen ke `string.Format`; Meneruskan nilai literal
                // `”players.details.source_calc_template”` sebagai argumen ke `translate`.
                translate("players.details.source_calc_template"),
                // Meneruskan `sourcePath` (nilai source path) sebagai argumen ke `string.Format`.
                sourcePath,
                // Meneruskan memanggil `translate` dengan `formulaKey` sebagai argumen ke `string.Format`; Meneruskan `formulaKey` (nilai formula kunci) sebagai
                // argumen ke `translate`.
                translate(formulaKey));
        // Menutup scope fungsi lokal WithSource; bagian berikut berada di luar batas blok tersebut dalam WithSource.
        }

        // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `isRawDomain` dan `(key.Contains(”coins_net_end_game”) ||
        // key.Contains(”coins_net_end”))`; sisi kanan diperiksa hanya jika sisi kiri benar; blok if hanya dijalankan ketika kondisi ini bernilai benar
        // dalam BuildFormulaHint.
        if (isRawDomain &&
            // Menggunakan gabungan syarat OR: setidaknya satu kondisi wajib benar antara `key.Contains(”coins_net_end_game”)` dan
            // `key.Contains(”coins_net_end”)`; sisi kanan diperiksa hanya jika sisi kiri salah sebagai bagian ekspresi yang sedang disusun dalam
            // BuildFormulaHint.
            (key.Contains("coins_net_end_game") || key.Contains("coins_net_end")))
        // Membuka scope cabang if untuk kondisi `isRawDomain && (key.Contains(”coins_net_end_game”) || key.Contains(”coins_net_end”))`;
        // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildFormulaHint.
        {
            // Mengembalikan memanggil `WithSource` dengan `”players.details.formula.coins_net_end”` kepada pemanggil dalam BuildFormulaHint; eksekusi jalur ini
            // selesai setelah nilai hasil ditentukan.
            return WithSource("players.details.formula.coins_net_end");
        // Menutup scope cabang if untuk kondisi `isRawDomain && (key.Contains(”coins_net_end_game”) || key.Contains(”coins_net_end”))`; bagian berikut
        // berada di luar batas blok tersebut dalam BuildFormulaHint.
        }

        // Memeriksa `isRawDomain` (nilai berstatus raw domain); blok if hanya dijalankan ketika kondisi ini bernilai benar dalam BuildFormulaHint.
        if (isRawDomain)
        // Membuka scope cabang if untuk kondisi `isRawDomain`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildFormulaHint.
        {
            // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `key.Contains(”cash_in”)` dan `key.Contains(”earned”)`; sisi kanan
            // diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam BuildFormulaHint.
            if (key.Contains("cash_in") || key.Contains("earned"))
            // Membuka scope cabang if untuk kondisi `key.Contains(”cash_in”) || key.Contains(”earned”)`; pernyataan/deklarasi berikut berada di dalam batas
            // blok ini dalam BuildFormulaHint.
            {
                // Mengembalikan memanggil `WithSource` dengan `”players.details.formula.raw.cash_in_sum”` kepada pemanggil dalam BuildFormulaHint; eksekusi jalur
                // ini selesai setelah nilai hasil ditentukan.
                return WithSource("players.details.formula.raw.cash_in_sum");
            // Menutup scope cabang if untuk kondisi `key.Contains(”cash_in”) || key.Contains(”earned”)`; bagian berikut berada di luar batas blok tersebut
            // dalam BuildFormulaHint.
            }

            // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `key.Contains(”cash_out”)` dan `key.Contains(”spent”)`; sisi kanan
            // diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam BuildFormulaHint.
            if (key.Contains("cash_out") || key.Contains("spent"))
            // Membuka scope cabang if untuk kondisi `key.Contains(”cash_out”) || key.Contains(”spent”)`; pernyataan/deklarasi berikut berada di dalam batas
            // blok ini dalam BuildFormulaHint.
            {
                // Mengembalikan memanggil `WithSource` dengan `”players.details.formula.raw.cash_out_sum”` kepada pemanggil dalam BuildFormulaHint; eksekusi jalur
                // ini selesai setelah nilai hasil ditentukan.
                return WithSource("players.details.formula.raw.cash_out_sum");
            // Menutup scope cabang if untuk kondisi `key.Contains(”cash_out”) || key.Contains(”spent”)`; bagian berikut berada di luar batas blok tersebut
            // dalam BuildFormulaHint.
            }

            // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `key.Contains(”per_turn”)` dan `key.Contains(”progression”)`; sisi kanan
            // diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam BuildFormulaHint.
            if (key.Contains("per_turn") || key.Contains("progression"))
            // Membuka scope cabang if untuk kondisi `key.Contains(”per_turn”) || key.Contains(”progression”)`; pernyataan/deklarasi berikut berada di dalam
            // batas blok ini dalam BuildFormulaHint.
            {
                // Mengembalikan memanggil `WithSource` dengan `”players.details.formula.raw.per_turn_aggregate”` kepada pemanggil dalam BuildFormulaHint; eksekusi
                // jalur ini selesai setelah nilai hasil ditentukan.
                return WithSource("players.details.formula.raw.per_turn_aggregate");
            // Menutup scope cabang if untuk kondisi `key.Contains(”per_turn”) || key.Contains(”progression”)`; bagian berikut berada di luar batas blok
            // tersebut dalam BuildFormulaHint.
            }

            // Mengembalikan memanggil `string.Format` dengan `CultureInfo.CurrentCulture`, `translate(”players.details.source_line_raw_template”)`,
            // `sourcePath` kepada pemanggil dalam BuildFormulaHint; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return string.Format(
                // Meneruskan `CultureInfo.CurrentCulture` (nilai saat ini culture) sebagai argumen ke `string.Format`.
                CultureInfo.CurrentCulture,
                // Meneruskan memanggil `translate` dengan `”players.details.source_line_raw_template”` sebagai argumen ke `string.Format`; Meneruskan nilai literal
                // `”players.details.source_line_raw_template”` sebagai argumen ke `translate`.
                translate("players.details.source_line_raw_template"),
                // Meneruskan `sourcePath` (nilai source path) sebagai argumen ke `string.Format`.
                sourcePath);
        // Menutup scope cabang if untuk kondisi `isRawDomain`; bagian berikut berada di luar batas blok tersebut dalam BuildFormulaHint.
        }

        // Memeriksa memeriksa apakah `key` memuat `”happiness_points”`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam BuildFormulaHint.
        if (key.Contains("happiness_points"))
        // Membuka scope cabang if untuk kondisi `key.Contains(”happiness_points”)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // BuildFormulaHint.
        {
            // Mengembalikan memanggil `WithSource` dengan `”players.details.formula.derived.happiness_points”` kepada pemanggil dalam BuildFormulaHint;
            // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return WithSource("players.details.formula.derived.happiness_points");
        // Menutup scope cabang if untuk kondisi `key.Contains(”happiness_points”)`; bagian berikut berada di luar batas blok tersebut dalam
        // BuildFormulaHint.
        }

        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `key.Contains(”cashflow_net”) || key.Contains(”net_cashflow”)` dan
        // `key.Contains(”coins_net”)`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // BuildFormulaHint.
        if (key.Contains("cashflow_net") || key.Contains("net_cashflow") || key.Contains("coins_net"))
        // Membuka scope cabang if untuk kondisi `key.Contains(”cashflow_net”) || key.Contains(”net_cashflow”) || key.Contains(”coins_net”)`;
        // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildFormulaHint.
        {
            // Mengembalikan memanggil `WithSource` dengan `”players.details.formula.derived.net_cashflow”` kepada pemanggil dalam BuildFormulaHint; eksekusi
            // jalur ini selesai setelah nilai hasil ditentukan.
            return WithSource("players.details.formula.derived.net_cashflow");
        // Menutup scope cabang if untuk kondisi `key.Contains(”cashflow_net”) || key.Contains(”net_cashflow”) || key.Contains(”coins_net”)`; bagian berikut
        // berada di luar batas blok tersebut dalam BuildFormulaHint.
        }

        // Memeriksa memeriksa apakah `key` memuat `”donation_points”`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam BuildFormulaHint.
        if (key.Contains("donation_points"))
        // Membuka scope cabang if untuk kondisi `key.Contains(”donation_points”)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // BuildFormulaHint.
        {
            // Mengembalikan memanggil `WithSource` dengan `”players.details.formula.derived.donation_points”` kepada pemanggil dalam BuildFormulaHint; eksekusi
            // jalur ini selesai setelah nilai hasil ditentukan.
            return WithSource("players.details.formula.derived.donation_points");
        // Menutup scope cabang if untuk kondisi `key.Contains(”donation_points”)`; bagian berikut berada di luar batas blok tersebut dalam
        // BuildFormulaHint.
        }

        // Memeriksa memeriksa apakah `key` memuat `”pension_points”`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam BuildFormulaHint.
        if (key.Contains("pension_points"))
        // Membuka scope cabang if untuk kondisi `key.Contains(”pension_points”)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // BuildFormulaHint.
        {
            // Mengembalikan memanggil `WithSource` dengan `”players.details.formula.derived.pension_points”` kepada pemanggil dalam BuildFormulaHint; eksekusi
            // jalur ini selesai setelah nilai hasil ditentukan.
            return WithSource("players.details.formula.derived.pension_points");
        // Menutup scope cabang if untuk kondisi `key.Contains(”pension_points”)`; bagian berikut berada di luar batas blok tersebut dalam BuildFormulaHint.
        }

        // Memeriksa memeriksa apakah `key` memuat `”saving_goal_points”`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // BuildFormulaHint.
        if (key.Contains("saving_goal_points"))
        // Membuka scope cabang if untuk kondisi `key.Contains(”saving_goal_points”)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // BuildFormulaHint.
        {
            // Mengembalikan memanggil `WithSource` dengan `”players.details.formula.derived.saving_goal_points”` kepada pemanggil dalam BuildFormulaHint;
            // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return WithSource("players.details.formula.derived.saving_goal_points");
        // Menutup scope cabang if untuk kondisi `key.Contains(”saving_goal_points”)`; bagian berikut berada di luar batas blok tersebut dalam
        // BuildFormulaHint.
        }

        // Memeriksa memeriksa apakah `key` memuat `”need_points”`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam BuildFormulaHint.
        if (key.Contains("need_points"))
        // Membuka scope cabang if untuk kondisi `key.Contains(”need_points”)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // BuildFormulaHint.
        {
            // Mengembalikan memanggil `WithSource` dengan `”players.details.formula.derived.need_points”` kepada pemanggil dalam BuildFormulaHint; eksekusi
            // jalur ini selesai setelah nilai hasil ditentukan.
            return WithSource("players.details.formula.derived.need_points");
        // Menutup scope cabang if untuk kondisi `key.Contains(”need_points”)`; bagian berikut berada di luar batas blok tersebut dalam BuildFormulaHint.
        }

        // Memeriksa memeriksa apakah `key` memuat `”penalty”`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam BuildFormulaHint.
        if (key.Contains("penalty"))
        // Membuka scope cabang if untuk kondisi `key.Contains(”penalty”)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // BuildFormulaHint.
        {
            // Mengembalikan memanggil `WithSource` dengan `”players.details.formula.derived.penalty”` kepada pemanggil dalam BuildFormulaHint; eksekusi jalur
            // ini selesai setelah nilai hasil ditentukan.
            return WithSource("players.details.formula.derived.penalty");
        // Menutup scope cabang if untuk kondisi `key.Contains(”penalty”)`; bagian berikut berada di luar batas blok tersebut dalam BuildFormulaHint.
        }

        // Memeriksa memeriksa apakah `key` memuat `”points”`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam BuildFormulaHint.
        if (key.Contains("points"))
        // Membuka scope cabang if untuk kondisi `key.Contains(”points”)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // BuildFormulaHint.
        {
            // Mengembalikan memanggil `WithSource` dengan `”players.details.formula.derived.points_weight”` kepada pemanggil dalam BuildFormulaHint; eksekusi
            // jalur ini selesai setelah nilai hasil ditentukan.
            return WithSource("players.details.formula.derived.points_weight");
        // Menutup scope cabang if untuk kondisi `key.Contains(”points”)`; bagian berikut berada di luar batas blok tersebut dalam BuildFormulaHint.
        }

        // Mengembalikan memanggil `WithSource` dengan `”players.details.formula.derived.default”` kepada pemanggil dalam BuildFormulaHint; eksekusi jalur
        // ini selesai setelah nilai hasil ditentukan.
        return WithSource("players.details.formula.derived.default");
    // Menutup scope metode BuildFormulaHint; bagian berikut berada di luar batas blok tersebut dalam BuildFormulaHint.
    }

    // Mendefinisikan metode `BuildSummaryFallbackDetail` dengan hasil bertipe `string`; operasi ini menangani build summary fallback detail. Masukan:
    // Parameter `isRawDomain` bertipe `bool` membawa nilai berstatus raw domain; Parameter `translate` bertipe `Func<string, string>` membawa nilai
    // translate.
    private static string BuildSummaryFallbackDetail(bool isRawDomain, Func<string, string> translate)
    // Membuka scope metode BuildSummaryFallbackDetail; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildSummaryFallbackDetail.
    {
        // Mengembalikan hasil pemilihan bersyarat: ketika `isRawDomain` benar gunakan `translate(”players.details.source_raw_summary”)`, jika tidak gunakan
        // `translate(”players.details.source_derived_summary”)` kepada pemanggil dalam BuildSummaryFallbackDetail; eksekusi jalur ini selesai setelah nilai
        // hasil ditentukan.
        return isRawDomain
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: translate(”players.details.source_raw_summary”) dalam
            // BuildSummaryFallbackDetail.
            ? translate("players.details.source_raw_summary")
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: translate(”players.details.source_derived_summary”); dalam
            // BuildSummaryFallbackDetail.
            : translate("players.details.source_derived_summary");
    // Menutup scope metode BuildSummaryFallbackDetail; bagian berikut berada di luar batas blok tersebut dalam BuildSummaryFallbackDetail.
    }

    // Mendefinisikan metode `BuildSeriesFromObjectArray` dengan hasil bertipe `void`; operasi ini menangani build series dari object array. Masukan:
    // Parameter `items` bertipe `List<JsonElement>` membawa nilai elemen; Parameter `labels` bertipe `List<string>` membawa nilai labels; Parameter
    // `series` bertipe `Dictionary<string, List<double?>>` membawa nilai series; Parameter `appendLabel` bertipe `Action<string>` membawa nilai append
    // label; Parameter `setSeriesValue` bertipe `Action<string, double>` membawa nilai set series nilai; Parameter `translate` bertipe `Func<string,
    // string>` membawa nilai translate.
    private static void BuildSeriesFromObjectArray(
        // Parameter `items` bertipe `List<JsonElement>` membawa nilai elemen.
        List<JsonElement> items,
        // Parameter `labels` bertipe `List<string>` membawa nilai labels.
        List<string> labels,
        // Parameter `series` bertipe `Dictionary<string, List<double?>>` membawa nilai series.
        Dictionary<string, List<double?>> series,
        // Parameter `appendLabel` bertipe `Action<string>` membawa nilai append label.
        Action<string> appendLabel,
        // Parameter `setSeriesValue` bertipe `Action<string, double>` membawa nilai set series nilai.
        Action<string, double> setSeriesValue,
        // Parameter `translate` bertipe `Func<string, string>` membawa nilai translate.
        Func<string, string> translate)
    // Membuka scope metode BuildSeriesFromObjectArray; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildSeriesFromObjectArray.
    {
        // Menyiapkan variabel lokal `xAxisCandidates` untuk nilai x axis candidates dengan array baru dengan tipe elemen disimpulkan dari nilai
        // initializer. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var xAxisCandidates = new[]
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // BuildSeriesFromObjectArray.
        {
            // Menggunakan nilai literal `”action_slot”` sebagai bagian ekspresi yang sedang disusun dalam BuildSeriesFromObjectArray.
            "action_slot",
            // Menggunakan nilai literal `”day_index”` sebagai bagian ekspresi yang sedang disusun dalam BuildSeriesFromObjectArray.
            "day_index",
            // Menggunakan nilai literal `”friday_index”` sebagai bagian ekspresi yang sedang disusun dalam BuildSeriesFromObjectArray.
            "friday_index",
            // Menggunakan nilai literal `”order_index”` sebagai bagian ekspresi yang sedang disusun dalam BuildSeriesFromObjectArray.
            "order_index",
            // Menggunakan nilai literal `”card_index”` sebagai bagian ekspresi yang sedang disusun dalam BuildSeriesFromObjectArray.
            "card_index",
            // Menggunakan nilai literal `”goal_index”` sebagai bagian ekspresi yang sedang disusun dalam BuildSeriesFromObjectArray.
            "goal_index",
            // Menggunakan nilai literal `”sequence_number”` sebagai bagian ekspresi yang sedang disusun dalam BuildSeriesFromObjectArray.
            "sequence_number",
            // Menggunakan nilai literal `”index”` sebagai bagian ekspresi yang sedang disusun dalam BuildSeriesFromObjectArray.
            "index"
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam BuildSeriesFromObjectArray.
        };

        // Menyiapkan variabel lokal `xKey` untuk nilai x kunci dengan null, yaitu penanda tidak ada nilai. Tipe yang dipakai adalah `string?`.
        string? xKey = null;
        // Mengulangi setiap elemen `xAxisCandidates`; elemen saat ini disimpan sebagai `candidate` bertipe `var` untuk diproses oleh badan loop dalam
        // BuildSeriesFromObjectArray.
        foreach (var candidate in xAxisCandidates)
        // Membuka scope loop setiap candidate dari `xAxisCandidates`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // BuildSeriesFromObjectArray.
        {
            // Memeriksa memeriksa apakah `items` memiliki setidaknya satu elemen yang memenuhi `item => item.TryGetProperty(candidate, out _)`; blok if hanya
            // dijalankan ketika kondisi ini bernilai benar dalam BuildSeriesFromObjectArray.
            if (items.Any(item => item.TryGetProperty(candidate, out _)))
            // Membuka scope cabang if untuk kondisi `items.Any(item => item.TryGetProperty(candidate, out _))`; pernyataan/deklarasi berikut berada di dalam
            // batas blok ini dalam BuildSeriesFromObjectArray.
            {
                // Memperbarui `xKey` menggunakan `candidate` (nilai candidate) dalam BuildSeriesFromObjectArray.
                xKey = candidate;
                // Mengakhiri loop atau cabang switch terdekat, kemudian melanjutkan setelah blok tersebut dalam BuildSeriesFromObjectArray.
                break;
            // Menutup scope cabang if untuk kondisi `items.Any(item => item.TryGetProperty(candidate, out _))`; bagian berikut berada di luar batas blok
            // tersebut dalam BuildSeriesFromObjectArray.
            }
        // Menutup scope loop setiap candidate dari `xAxisCandidates`; bagian berikut berada di luar batas blok tersebut dalam BuildSeriesFromObjectArray.
        }

        // Memeriksa hasil pencocokan `xKey` dengan pola `null`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // BuildSeriesFromObjectArray.
        if (xKey is null)
        // Membuka scope cabang if untuk kondisi `xKey is null`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // BuildSeriesFromObjectArray.
        {
            // Memperbarui `xKey` menggunakan mengambil elemen pertama `items .SelectMany(item => item.EnumerateObject().Select(prop => prop.Name))` yang sesuai
            // `name => name.EndsWith(”_number”, StringComparison.OrdinalIgnoreCase) || name.EndsWith(”_index”, StringComparison.OrdinalIgnoreCase)`; jika tidak
            // ada, gunakan nilai default tipe hasil dalam BuildSeriesFromObjectArray.
            xKey = items
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .SelectMany(item => item.EnumerateObject().Select(prop => prop.Name)) dalam
                // BuildSeriesFromObjectArray; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
                .SelectMany(item => item.EnumerateObject().Select(prop => prop.Name))
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .FirstOrDefault(name => dalam BuildSeriesFromObjectArray; token pada baris ini
                // menyambungkan bagian kode sebelum dan sesudahnya.
                .FirstOrDefault(name =>
                    // Meneruskan nilai literal `”_number”` sebagai argumen ke `name.EndsWith`; Meneruskan `StringComparison.OrdinalIgnoreCase` (nilai ordinal ignore
                    // case) sebagai argumen ke `name.EndsWith`.
                    name.EndsWith("_number", StringComparison.OrdinalIgnoreCase) ||
                    // Meneruskan nilai literal `”_index”` sebagai argumen ke `name.EndsWith`; Meneruskan `StringComparison.OrdinalIgnoreCase` (nilai ordinal ignore
                    // case) sebagai argumen ke `name.EndsWith`.
                    name.EndsWith("_index", StringComparison.OrdinalIgnoreCase));
        // Menutup scope cabang if untuk kondisi `xKey is null`; bagian berikut berada di luar batas blok tersebut dalam BuildSeriesFromObjectArray.
        }

        // Memulai loop dengan inisialisasi `var index = 0`, berjalan selama `index < items.Count`, lalu memperbarui pencacah melalui `index++` dalam
        // BuildSeriesFromObjectArray.
        for (var index = 0; index < items.Count; index++)
        // Membuka scope loop dengan syarat `index < items.Count`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // BuildSeriesFromObjectArray.
        {
            // Menyiapkan variabel lokal `item` untuk nilai elemen dengan `items[index]`, yaitu elemen koleksi yang dipilih melalui indeks atau kunci tersebut.
            // Tipe variabel disimpulkan dari ekspresi nilai awal.
            var item = items[index];
            // Menyiapkan variabel lokal `label` untuk nilai label dengan mengubah `(index + 1)` menjadi teks. Tipe variabel disimpulkan dari ekspresi nilai
            // awal.
            var label = (index + 1).ToString();
            // Menyiapkan variabel lokal `rowValues` untuk nilai baris nilai dengan objek baru bertipe `Dictionary<string, double>` dengan argumen
            // (StringComparer.OrdinalIgnoreCase). Tipe variabel disimpulkan dari ekspresi nilai awal.
            var rowValues = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
            // Mengulangi setiap elemen `item.EnumerateObject()`; elemen saat ini disimpan sebagai `property` bertipe `var` untuk diproses oleh badan loop dalam
            // BuildSeriesFromObjectArray.
            foreach (var property in item.EnumerateObject())
            // Membuka scope loop setiap property dari `item.EnumerateObject()`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // BuildSeriesFromObjectArray.
            {
                // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `!string.IsNullOrWhiteSpace(xKey)` dan `property.Name.Equals(xKey,
                // StringComparison.OrdinalIgnoreCase)`; sisi kanan diperiksa hanya jika sisi kiri benar; blok if hanya dijalankan ketika kondisi ini bernilai benar
                // dalam BuildSeriesFromObjectArray.
                if (!string.IsNullOrWhiteSpace(xKey) &&
                    // Melanjutkan pengolahan dengan membandingkan kesamaan `property.Name` dengan `xKey`, `StringComparison.OrdinalIgnoreCase`; aturan perbandingan
                    // mengikuti overload dan comparer yang diberikan dalam BuildSeriesFromObjectArray.
                    property.Name.Equals(xKey, StringComparison.OrdinalIgnoreCase))
                // Membuka scope cabang if untuk kondisi `!string.IsNullOrWhiteSpace(xKey) && property.Name.Equals(xKey, StringComparison.OrdinalIgnoreCase)`;
                // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildSeriesFromObjectArray.
                {
                    // Memperbarui `label` menggunakan memanggil `PlayerMetricJsonMapper.FormatJsonLeafValue` dengan `property.Value`, `translate(”state.true”)`,
                    // `translate(”state.false”)`, `translate(”state.null”)` dalam BuildSeriesFromObjectArray.
                    label = PlayerMetricJsonMapper.FormatJsonLeafValue(
                        // Meneruskan `property.Value`, yaitu nilai yang dibungkus objek/nullable sebagai argumen ke `PlayerMetricJsonMapper.FormatJsonLeafValue`.
                        property.Value,
                        // Meneruskan memanggil `translate` dengan `”state.true”` sebagai argumen ke `PlayerMetricJsonMapper.FormatJsonLeafValue`; Meneruskan nilai literal
                        // `”state.true”` sebagai argumen ke `translate`.
                        translate("state.true"),
                        // Meneruskan memanggil `translate` dengan `”state.false”` sebagai argumen ke `PlayerMetricJsonMapper.FormatJsonLeafValue`; Meneruskan nilai literal
                        // `”state.false”` sebagai argumen ke `translate`.
                        translate("state.false"),
                        // Meneruskan memanggil `translate` dengan `”state.null”` sebagai argumen ke `PlayerMetricJsonMapper.FormatJsonLeafValue`; Meneruskan nilai literal
                        // `”state.null”` sebagai argumen ke `translate`.
                        translate("state.null"));
                    // Melewati sisa pernyataan pada iterasi saat ini dan melanjutkan ke elemen/iterasi berikutnya dalam BuildSeriesFromObjectArray.
                    continue;
                // Menutup scope cabang if untuk kondisi `!string.IsNullOrWhiteSpace(xKey) && property.Name.Equals(xKey, StringComparison.OrdinalIgnoreCase)`;
                // bagian berikut berada di luar batas blok tersebut dalam BuildSeriesFromObjectArray.
                }

                // Memeriksa memanggil `PlayerMetricJsonMapper.TryGetNumericValue` dengan `property.Value`, `var numericValue`; blok if hanya dijalankan ketika
                // kondisi ini bernilai benar dalam BuildSeriesFromObjectArray.
                if (PlayerMetricJsonMapper.TryGetNumericValue(property.Value, out var numericValue))
                // Membuka scope cabang if untuk kondisi `PlayerMetricJsonMapper.TryGetNumericValue(property.Value, out var numericValue)`; pernyataan/deklarasi
                // berikut berada di dalam batas blok ini dalam BuildSeriesFromObjectArray.
                {
                    // Memperbarui `rowValues[property.Name]` menggunakan `numericValue` (nilai numerik nilai) dalam BuildSeriesFromObjectArray.
                    rowValues[property.Name] = numericValue;
                    // Melewati sisa pernyataan pada iterasi saat ini dan melanjutkan ke elemen/iterasi berikutnya dalam BuildSeriesFromObjectArray.
                    continue;
                // Menutup scope cabang if untuk kondisi `PlayerMetricJsonMapper.TryGetNumericValue(property.Value, out var numericValue)`; bagian berikut berada di
                // luar batas blok tersebut dalam BuildSeriesFromObjectArray.
                }

                // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `property.Value.ValueKind == JsonValueKind.Array` dan
                // `property.Name.Equals(”actions”, StringComparison.OrdinalIgnoreCase)`; sisi kanan diperiksa hanya jika sisi kiri benar; blok if hanya dijalankan
                // ketika kondisi ini bernilai benar dalam BuildSeriesFromObjectArray.
                if (property.Value.ValueKind == JsonValueKind.Array &&
                    // Melanjutkan pengolahan dengan membandingkan kesamaan `property.Name` dengan `”actions”`, `StringComparison.OrdinalIgnoreCase`; aturan
                    // perbandingan mengikuti overload dan comparer yang diberikan dalam BuildSeriesFromObjectArray.
                    property.Name.Equals("actions", StringComparison.OrdinalIgnoreCase))
                // Membuka scope cabang if untuk kondisi `property.Value.ValueKind == JsonValueKind.Array && property.Name.Equals(”actions”,
                // StringComparison.OrdinalIgnoreCase)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildSeriesFromObjectArray.
                {
                    // Memperbarui `rowValues[”actions_count”]` menggunakan memanggil `property.Value.GetArrayLength` dengan tanpa argumen dalam
                    // BuildSeriesFromObjectArray.
                    rowValues["actions_count"] = property.Value.GetArrayLength();
                // Menutup scope cabang if untuk kondisi `property.Value.ValueKind == JsonValueKind.Array && property.Name.Equals(”actions”,
                // StringComparison.OrdinalIgnoreCase)`; bagian berikut berada di luar batas blok tersebut dalam BuildSeriesFromObjectArray.
                }
            // Menutup scope loop setiap property dari `item.EnumerateObject()`; bagian berikut berada di luar batas blok tersebut dalam
            // BuildSeriesFromObjectArray.
            }

            // Menjalankan memanggil `appendLabel` dengan `label` dalam BuildSeriesFromObjectArray.
            appendLabel(label);
            // Mengulangi setiap elemen `rowValues`; elemen saat ini disimpan sebagai `rowValue` bertipe `var` untuk diproses oleh badan loop dalam
            // BuildSeriesFromObjectArray.
            foreach (var rowValue in rowValues)
            // Membuka scope loop setiap rowValue dari `rowValues`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // BuildSeriesFromObjectArray.
            {
                // Menjalankan memanggil `setSeriesValue` dengan `rowValue.Key`, `rowValue.Value` dalam BuildSeriesFromObjectArray.
                setSeriesValue(rowValue.Key, rowValue.Value);
            // Menutup scope loop setiap rowValue dari `rowValues`; bagian berikut berada di luar batas blok tersebut dalam BuildSeriesFromObjectArray.
            }
        // Menutup scope loop dengan syarat `index < items.Count`; bagian berikut berada di luar batas blok tersebut dalam BuildSeriesFromObjectArray.
        }
    // Menutup scope metode BuildSeriesFromObjectArray; bagian berikut berada di luar batas blok tersebut dalam BuildSeriesFromObjectArray.
    }
// Menutup scope tipe PlayerMetricChartPayloadBuilder; bagian berikut berada di luar batas blok tersebut.
}
