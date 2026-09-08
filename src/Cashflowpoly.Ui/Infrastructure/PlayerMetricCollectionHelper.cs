// Fungsi file: Menyediakan transformasi, lokalisasi, atau koneksi UI melalui PlayerMetricCollectionHelper.
// Mengimpor namespace `System.Globalization` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Globalization;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Infrastructure` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Infrastructure;

/// <summary>
/// Helper murni untuk menggabungkan dan memfilter baris serta chart metrik.
/// </summary>
// Mendefinisikan tipe class `PlayerMetricCollectionHelper`.
public static class PlayerMetricCollectionHelper
// Membuka scope tipe PlayerMetricCollectionHelper; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    /// <summary>
    /// Menggabungkan baris berdasarkan path unik, mempertahankan kemunculan pertama.
    /// </summary>
    // Mendefinisikan metode `MergeUniqueRows` dengan hasil bertipe `List<(string Path, string Value)>`. Menggabungkan baris berdasarkan path unik,
    // mempertahankan kemunculan pertama. Masukan: Parameter `rowSets` bertipe `IEnumerable<(string Path, string Value)>[]` membawa nilai baris sets.
    public static List<(string Path, string Value)> MergeUniqueRows(params IEnumerable<(string Path, string Value)>[] rowSets)
    // Membuka scope metode MergeUniqueRows; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam MergeUniqueRows.
    {
        // Menyiapkan variabel lokal `result` untuk nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya dengan objek baru bertipe `List<(string
        // Path, string Value)>` dengan nilai awal sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var result = new List<(string Path, string Value)>();
        // Menyiapkan variabel lokal `seen` untuk nilai seen dengan objek baru bertipe `HashSet<string>` dengan argumen (StringComparer.OrdinalIgnoreCase).
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        // Mengulangi setiap elemen `rowSets`; elemen saat ini disimpan sebagai `rowSet` bertipe `var` untuk diproses oleh badan loop dalam MergeUniqueRows.
        foreach (var rowSet in rowSets)
        // Membuka scope loop setiap rowSet dari `rowSets`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam MergeUniqueRows.
        {
            // Mengulangi setiap elemen `rowSet`; elemen saat ini disimpan sebagai `row` bertipe `var` untuk diproses oleh badan loop dalam MergeUniqueRows.
            foreach (var row in rowSet)
            // Membuka scope loop setiap row dari `rowSet`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam MergeUniqueRows.
            {
                // Memeriksa memeriksa apakah `row.Path` null, kosong, atau hanya berisi karakter spasi; blok if hanya dijalankan ketika kondisi ini bernilai benar
                // dalam MergeUniqueRows.
                if (string.IsNullOrWhiteSpace(row.Path))
                // Membuka scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(row.Path)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // MergeUniqueRows.
                {
                    // Melewati sisa pernyataan pada iterasi saat ini dan melanjutkan ke elemen/iterasi berikutnya dalam MergeUniqueRows.
                    continue;
                // Menutup scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(row.Path)`; bagian berikut berada di luar batas blok tersebut dalam
                // MergeUniqueRows.
                }

                // Memeriksa menambahkan `row.Path` ke `seen`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam MergeUniqueRows.
                if (seen.Add(row.Path))
                // Membuka scope cabang if untuk kondisi `seen.Add(row.Path)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam MergeUniqueRows.
                {
                    // Menjalankan menambahkan `row` ke `result` dalam MergeUniqueRows.
                    result.Add(row);
                // Menutup scope cabang if untuk kondisi `seen.Add(row.Path)`; bagian berikut berada di luar batas blok tersebut dalam MergeUniqueRows.
                }
            // Menutup scope loop setiap row dari `rowSet`; bagian berikut berada di luar batas blok tersebut dalam MergeUniqueRows.
            }
        // Menutup scope loop setiap rowSet dari `rowSets`; bagian berikut berada di luar batas blok tersebut dalam MergeUniqueRows.
        }

        // Mengembalikan `result` (nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya) kepada pemanggil dalam MergeUniqueRows; eksekusi jalur
        // ini selesai setelah nilai hasil ditentukan.
        return result;
    // Menutup scope metode MergeUniqueRows; bagian berikut berada di luar batas blok tersebut dalam MergeUniqueRows.
    }

    /// <summary>
    /// Mengambil baris dari beberapa grup sesuai urutan key yang diminta.
    /// </summary>
    // Mendefinisikan metode `GetGroupRows` dengan hasil bertipe `List<(string Path, string Value)>`. Mengambil baris dari beberapa grup sesuai urutan
    // key yang diminta. Masukan: Parameter `source` bertipe `Dictionary<string, List<(string Path, string Value)>>` membawa nilai source; Parameter
    // `keys` bertipe `string[]` membawa nilai kunci.
    public static List<(string Path, string Value)> GetGroupRows(
        // Parameter `source` bertipe `Dictionary<string, List<(string Path, string Value)>>` membawa nilai source.
        Dictionary<string, List<(string Path, string Value)>> source,
        // Parameter `keys` bertipe `string[]` membawa nilai kunci.
        params string[] keys)
    // Membuka scope metode GetGroupRows; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam GetGroupRows.
    {
        // Menyiapkan variabel lokal `rows` untuk nilai baris dengan objek baru bertipe `List<(string Path, string Value)>` dengan nilai awal sesuai
        // konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var rows = new List<(string Path, string Value)>();
        // Mengulangi setiap elemen `keys`; elemen saat ini disimpan sebagai `key` bertipe `var` untuk diproses oleh badan loop dalam GetGroupRows.
        foreach (var key in keys)
        // Membuka scope loop setiap key dari `keys`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam GetGroupRows.
        {
            // Memeriksa kebalikan kondisi `source.TryGetValue(key, out var groupRows)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
            // GetGroupRows.
            if (!source.TryGetValue(key, out var groupRows))
            // Membuka scope cabang if untuk kondisi `!source.TryGetValue(key, out var groupRows)`; pernyataan/deklarasi berikut berada di dalam batas blok ini
            // dalam GetGroupRows.
            {
                // Melewati sisa pernyataan pada iterasi saat ini dan melanjutkan ke elemen/iterasi berikutnya dalam GetGroupRows.
                continue;
            // Menutup scope cabang if untuk kondisi `!source.TryGetValue(key, out var groupRows)`; bagian berikut berada di luar batas blok tersebut dalam
            // GetGroupRows.
            }

            // Menjalankan menambahkan seluruh elemen `groupRows` ke `rows` dalam GetGroupRows.
            rows.AddRange(groupRows);
        // Menutup scope loop setiap key dari `keys`; bagian berikut berada di luar batas blok tersebut dalam GetGroupRows.
        }

        // Mengembalikan `rows` (nilai baris) kepada pemanggil dalam GetGroupRows; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return rows;
    // Menutup scope metode GetGroupRows; bagian berikut berada di luar batas blok tersebut dalam GetGroupRows.
    }

    /// <summary>
    /// Memfilter baris yang path-nya mengandung salah satu keyword.
    /// </summary>
    // Mendefinisikan metode `FilterRowsByKeywords` dengan hasil bertipe `List<(string Path, string Value)>`. Memfilter baris yang path-nya mengandung
    // salah satu keyword. Masukan: Parameter `rows` bertipe `IEnumerable<(string Path, string Value)>` membawa nilai baris; Parameter `keywords`
    // bertipe `string[]` membawa nilai keywords.
    public static List<(string Path, string Value)> FilterRowsByKeywords(
        // Parameter `rows` bertipe `IEnumerable<(string Path, string Value)>` membawa nilai baris.
        IEnumerable<(string Path, string Value)> rows,
        // Parameter `keywords` bertipe `string[]` membawa nilai keywords.
        params string[] keywords)
    // Membuka scope metode FilterRowsByKeywords; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam FilterRowsByKeywords.
    {
        // Mengembalikan mematerialisasi urutan `rows .Where(row => keywords.Any(keyword => row.Path.Contains(keyword,
        // StringComparison.OrdinalIgnoreCase)))` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori kepada pemanggil dalam
        // FilterRowsByKeywords; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return rows
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(row => keywords.Any(keyword => row.Path.Contains(keyword,
            // StringComparison.OrdinalIgnoreCase))) dalam FilterRowsByKeywords; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Where(row => keywords.Any(keyword => row.Path.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList(); dalam FilterRowsByKeywords; token pada baris ini menyambungkan
            // bagian kode sebelum dan sesudahnya.
            .ToList();
    // Menutup scope metode FilterRowsByKeywords; bagian berikut berada di luar batas blok tersebut dalam FilterRowsByKeywords.
    }

    /// <summary>
    /// Menggabungkan chart berdasarkan title unik, mempertahankan kemunculan pertama.
    /// </summary>
    // Mendefinisikan metode `MergeUniqueCharts` dengan hasil bertipe `List<(string Title, string Json)>`. Menggabungkan chart berdasarkan title unik,
    // mempertahankan kemunculan pertama. Masukan: Parameter `chartSets` bertipe `IEnumerable<(string Title, string Json)>[]` membawa nilai chart sets.
    public static List<(string Title, string Json)> MergeUniqueCharts(params IEnumerable<(string Title, string Json)>[] chartSets)
    // Membuka scope metode MergeUniqueCharts; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam MergeUniqueCharts.
    {
        // Menyiapkan variabel lokal `result` untuk nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya dengan objek baru bertipe `List<(string
        // Title, string Json)>` dengan nilai awal sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var result = new List<(string Title, string Json)>();
        // Menyiapkan variabel lokal `seen` untuk nilai seen dengan objek baru bertipe `HashSet<string>` dengan argumen (StringComparer.OrdinalIgnoreCase).
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        // Mengulangi setiap elemen `chartSets`; elemen saat ini disimpan sebagai `chartSet` bertipe `var` untuk diproses oleh badan loop dalam
        // MergeUniqueCharts.
        foreach (var chartSet in chartSets)
        // Membuka scope loop setiap chartSet dari `chartSets`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam MergeUniqueCharts.
        {
            // Mengulangi setiap elemen `chartSet`; elemen saat ini disimpan sebagai `chart` bertipe `var` untuk diproses oleh badan loop dalam
            // MergeUniqueCharts.
            foreach (var chart in chartSet)
            // Membuka scope loop setiap chart dari `chartSet`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam MergeUniqueCharts.
            {
                // Memeriksa memeriksa apakah `chart.Title` null, kosong, atau hanya berisi karakter spasi; blok if hanya dijalankan ketika kondisi ini bernilai
                // benar dalam MergeUniqueCharts.
                if (string.IsNullOrWhiteSpace(chart.Title))
                // Membuka scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(chart.Title)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // MergeUniqueCharts.
                {
                    // Melewati sisa pernyataan pada iterasi saat ini dan melanjutkan ke elemen/iterasi berikutnya dalam MergeUniqueCharts.
                    continue;
                // Menutup scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(chart.Title)`; bagian berikut berada di luar batas blok tersebut dalam
                // MergeUniqueCharts.
                }

                // Memeriksa menambahkan `chart.Title` ke `seen`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam MergeUniqueCharts.
                if (seen.Add(chart.Title))
                // Membuka scope cabang if untuk kondisi `seen.Add(chart.Title)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // MergeUniqueCharts.
                {
                    // Menjalankan menambahkan `chart` ke `result` dalam MergeUniqueCharts.
                    result.Add(chart);
                // Menutup scope cabang if untuk kondisi `seen.Add(chart.Title)`; bagian berikut berada di luar batas blok tersebut dalam MergeUniqueCharts.
                }
            // Menutup scope loop setiap chart dari `chartSet`; bagian berikut berada di luar batas blok tersebut dalam MergeUniqueCharts.
            }
        // Menutup scope loop setiap chartSet dari `chartSets`; bagian berikut berada di luar batas blok tersebut dalam MergeUniqueCharts.
        }

        // Mengembalikan `result` (nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya) kepada pemanggil dalam MergeUniqueCharts; eksekusi jalur
        // ini selesai setelah nilai hasil ditentukan.
        return result;
    // Menutup scope metode MergeUniqueCharts; bagian berikut berada di luar batas blok tersebut dalam MergeUniqueCharts.
    }

    /// <summary>
    /// Memfilter chart yang title-nya mengandung salah satu keyword.
    /// </summary>
    // Mendefinisikan metode `FilterChartsByKeywords` dengan hasil bertipe `List<(string Title, string Json)>`. Memfilter chart yang title-nya
    // mengandung salah satu keyword. Masukan: Parameter `charts` bertipe `IEnumerable<(string Title, string Json)>` membawa nilai charts; Parameter
    // `keywords` bertipe `string[]` membawa nilai keywords.
    public static List<(string Title, string Json)> FilterChartsByKeywords(
        // Parameter `charts` bertipe `IEnumerable<(string Title, string Json)>` membawa nilai charts.
        IEnumerable<(string Title, string Json)> charts,
        // Parameter `keywords` bertipe `string[]` membawa nilai keywords.
        params string[] keywords)
    // Membuka scope metode FilterChartsByKeywords; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam FilterChartsByKeywords.
    {
        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `keywords is null` dan `keywords.Length == 0`; sisi kanan diperiksa
        // hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam FilterChartsByKeywords.
        if (keywords is null || keywords.Length == 0)
        // Membuka scope cabang if untuk kondisi `keywords is null || keywords.Length == 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini
        // dalam FilterChartsByKeywords.
        {
            // Mengembalikan mematerialisasi urutan `charts` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori kepada pemanggil dalam
            // FilterChartsByKeywords; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return charts.ToList();
        // Menutup scope cabang if untuk kondisi `keywords is null || keywords.Length == 0`; bagian berikut berada di luar batas blok tersebut dalam
        // FilterChartsByKeywords.
        }

        // Mengembalikan mematerialisasi urutan `charts .Where(chart => keywords.Any(keyword => chart.Title.Contains(keyword,
        // StringComparison.OrdinalIgnoreCase)))` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori kepada pemanggil dalam
        // FilterChartsByKeywords; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return charts
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(chart => keywords.Any(keyword => chart.Title.Contains(keyword,
            // StringComparison.OrdinalIgnoreCase))) dalam FilterChartsByKeywords; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Where(chart => keywords.Any(keyword => chart.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList(); dalam FilterChartsByKeywords; token pada baris ini menyambungkan
            // bagian kode sebelum dan sesudahnya.
            .ToList();
    // Menutup scope metode FilterChartsByKeywords; bagian berikut berada di luar batas blok tersebut dalam FilterChartsByKeywords.
    }

    /// <summary>
    /// Mengambil chart dari beberapa grup sesuai urutan key yang diminta.
    /// </summary>
    // Mendefinisikan metode `GetGroupCharts` dengan hasil bertipe `List<(string Title, string Json)>`. Mengambil chart dari beberapa grup sesuai urutan
    // key yang diminta. Masukan: Parameter `source` bertipe `Dictionary<string, List<(string Title, string Json)>>` membawa nilai source; Parameter
    // `keys` bertipe `string[]` membawa nilai kunci.
    public static List<(string Title, string Json)> GetGroupCharts(
        // Parameter `source` bertipe `Dictionary<string, List<(string Title, string Json)>>` membawa nilai source.
        Dictionary<string, List<(string Title, string Json)>> source,
        // Parameter `keys` bertipe `string[]` membawa nilai kunci.
        params string[] keys)
    // Membuka scope metode GetGroupCharts; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam GetGroupCharts.
    {
        // Menyiapkan variabel lokal `charts` untuk nilai charts dengan objek baru bertipe `List<(string Title, string Json)>` dengan nilai awal sesuai
        // konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var charts = new List<(string Title, string Json)>();
        // Mengulangi setiap elemen `keys`; elemen saat ini disimpan sebagai `key` bertipe `var` untuk diproses oleh badan loop dalam GetGroupCharts.
        foreach (var key in keys)
        // Membuka scope loop setiap key dari `keys`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam GetGroupCharts.
        {
            // Memeriksa kebalikan kondisi `source.TryGetValue(key, out var groupCharts)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
            // GetGroupCharts.
            if (!source.TryGetValue(key, out var groupCharts))
            // Membuka scope cabang if untuk kondisi `!source.TryGetValue(key, out var groupCharts)`; pernyataan/deklarasi berikut berada di dalam batas blok
            // ini dalam GetGroupCharts.
            {
                // Melewati sisa pernyataan pada iterasi saat ini dan melanjutkan ke elemen/iterasi berikutnya dalam GetGroupCharts.
                continue;
            // Menutup scope cabang if untuk kondisi `!source.TryGetValue(key, out var groupCharts)`; bagian berikut berada di luar batas blok tersebut dalam
            // GetGroupCharts.
            }

            // Menjalankan menambahkan seluruh elemen `groupCharts` ke `charts` dalam GetGroupCharts.
            charts.AddRange(groupCharts);
        // Menutup scope loop setiap key dari `keys`; bagian berikut berada di luar batas blok tersebut dalam GetGroupCharts.
        }

        // Mengembalikan `charts` (nilai charts) kepada pemanggil dalam GetGroupCharts; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return charts;
    // Menutup scope metode GetGroupCharts; bagian berikut berada di luar batas blok tersebut dalam GetGroupCharts.
    }

    /// <summary>
    /// Mengganti variabel rumus analitik dengan angka pemain yang benar-benar digunakan.
    /// </summary>
    // Mendefinisikan metode `BuildActualCalculation` dengan hasil bertipe `string`. Mengganti variabel rumus analitik dengan angka pemain yang
    // benar-benar digunakan. Masukan: Parameter `analysisKey` bertipe `string` membawa nilai analysis kunci; Parameter `rows` bertipe
    // `IEnumerable<(string Path, string Value)>` membawa nilai baris; Parameter `resultValue` bertipe `string` membawa nilai hasil nilai; Parameter
    // `resultUnit` bertipe `string` membawa nilai hasil unit; Parameter `unavailableText` bertipe `string` membawa nilai unavailable text; Parameter
    // `culture` bertipe `CultureInfo` membawa nilai culture.
    public static string BuildActualCalculation(
        // Parameter `analysisKey` bertipe `string` membawa nilai analysis kunci.
        string analysisKey,
        // Parameter `rows` bertipe `IEnumerable<(string Path, string Value)>` membawa nilai baris.
        IEnumerable<(string Path, string Value)> rows,
        // Parameter `resultValue` bertipe `string` membawa nilai hasil nilai.
        string resultValue,
        // Parameter `resultUnit` bertipe `string` membawa nilai hasil unit.
        string resultUnit,
        // Parameter `unavailableText` bertipe `string` membawa nilai unavailable text.
        string unavailableText,
        // Parameter `culture` bertipe `CultureInfo` membawa nilai culture.
        CultureInfo culture)
    // Membuka scope metode BuildActualCalculation; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildActualCalculation.
    {
        // Menyiapkan variabel lokal `values` untuk nilai nilai dengan membangun kamus dari `rows .Where(row => !string.IsNullOrWhiteSpace(row.Path))
        // .GroupBy(row => row.Path, StringComparer.OrdinalIgnoreCase)` dengan pemilihan kunci/nilai `group => group.Key`, `group => group.First().Value`,
        // `StringComparer.OrdinalIgnoreCase`; kunci harus unik agar konversi berhasil. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var values = rows
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(row => !string.IsNullOrWhiteSpace(row.Path)) dalam
            // BuildActualCalculation; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Where(row => !string.IsNullOrWhiteSpace(row.Path))
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .GroupBy(row => row.Path, StringComparer.OrdinalIgnoreCase) dalam
            // BuildActualCalculation; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .GroupBy(row => row.Path, StringComparer.OrdinalIgnoreCase)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToDictionary(group => group.Key, group => group.First().Value,
            // StringComparer.OrdinalIgnoreCase); dalam BuildActualCalculation; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .ToDictionary(group => group.Key, group => group.First().Value, StringComparer.OrdinalIgnoreCase);

        // Memeriksa membandingkan kesamaan `string` dengan `resultValue`, `unavailableText`, `StringComparison.OrdinalIgnoreCase`; aturan perbandingan
        // mengikuti overload dan comparer yang diberikan; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam BuildActualCalculation.
        if (string.Equals(resultValue, unavailableText, StringComparison.OrdinalIgnoreCase))
        // Membuka scope cabang if untuk kondisi `string.Equals(resultValue, unavailableText, StringComparison.OrdinalIgnoreCase)`; pernyataan/deklarasi
        // berikut berada di dalam batas blok ini dalam BuildActualCalculation.
        {
            // Mengembalikan `unavailableText` (nilai unavailable text) kepada pemanggil dalam BuildActualCalculation; eksekusi jalur ini selesai setelah nilai
            // hasil ditentukan.
            return unavailableText;
        // Menutup scope cabang if untuk kondisi `string.Equals(resultValue, unavailableText, StringComparison.OrdinalIgnoreCase)`; bagian berikut berada di
        // luar batas blok tersebut dalam BuildActualCalculation.
        }

        // Mendefinisikan fungsi lokal Value dengan hasil `string`; fungsi ini dipakai oleh alur di dalam scope yang sama.
        string Value(string path, double scale = 1)
        // Membuka scope fungsi lokal Value; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Value.
        {
            // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!values.TryGetValue(path, out var rawValue)` dan
            // `!double.TryParse(rawValue, NumberStyles.Float, CultureInfo.InvariantCulture, out var numericValue)`; sisi kanan diperiksa hanya jika sisi kiri
            // salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam Value.
            if (!values.TryGetValue(path, out var rawValue) ||
                // Menggunakan kebalikan kondisi `double.TryParse(rawValue, NumberStyles.Float, CultureInfo.InvariantCulture, out var numericValue)` sebagai bagian
                // ekspresi yang sedang disusun dalam Value.
                !double.TryParse(rawValue, NumberStyles.Float, CultureInfo.InvariantCulture, out var numericValue))
            // Membuka scope cabang if untuk kondisi `!values.TryGetValue(path, out var rawValue) || !double.TryParse(rawValue, NumberStyles.Float,
            // CultureInfo.InvariantCulture, out var numericValue)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Value.
            {
                // Mengembalikan `unavailableText` (nilai unavailable text) kepada pemanggil dalam Value; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
                return unavailableText;
            // Menutup scope cabang if untuk kondisi `!values.TryGetValue(path, out var rawValue) || !double.TryParse(rawValue, NumberStyles.Float,
            // CultureInfo.InvariantCulture, out var numericValue)`; bagian berikut berada di luar batas blok tersebut dalam Value.
            }

            // Mengembalikan memanggil `FormatCalculationNumber` dengan `numericValue * scale`, `culture` kepada pemanggil dalam Value; eksekusi jalur ini
            // selesai setelah nilai hasil ditentukan.
            return FormatCalculationNumber(numericValue * scale, culture);
        // Menutup scope fungsi lokal Value; bagian berikut berada di luar batas blok tersebut dalam Value.
        }

        // Menyiapkan variabel lokal `result` untuk nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya dengan hasil pemilihan bersyarat: ketika
        // `string.IsNullOrWhiteSpace(resultUnit) || string.Equals(resultValue, unavailableText, StringComparison.OrdinalIgnoreCase)` benar gunakan
        // `resultValue`, jika tidak gunakan `resultUnit == ”%” ? $”{resultValue}{resultUnit}” : $”{resultValue} {resultUnit}”`. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var result = string.IsNullOrWhiteSpace(resultUnit) ||
                     // Melanjutkan pengolahan dengan membandingkan kesamaan `string` dengan `resultValue`, `unavailableText`, `StringComparison.OrdinalIgnoreCase`;
                     // aturan perbandingan mengikuti overload dan comparer yang diberikan dalam BuildActualCalculation.
                     string.Equals(resultValue, unavailableText, StringComparison.OrdinalIgnoreCase)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: resultValue dalam BuildActualCalculation.
            ? resultValue
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: resultUnit == ”%” dalam BuildActualCalculation.
            : resultUnit == "%"
                // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: $”{resultValue}{resultUnit}” dalam BuildActualCalculation.
                ? $"{resultValue}{resultUnit}"
                // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: $”{resultValue} {resultUnit}”; dalam BuildActualCalculation.
                : $"{resultValue} {resultUnit}";
        // Menyiapkan variabel lokal `incomeShares` untuk nilai pemasukan shares dengan mematerialisasi urutan `values .Where(item =>
        // item.Key.StartsWith(”income_shares.”, StringComparison.OrdinalIgnoreCase)) .OrderBy(item => item.Key, StringComparer.OrdinalIgnoreCase)
        // .Select(item => d...` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
        if (analysisKey == "goal-ambition") return result.Trim();
        if (analysisKey == "income-diversification" &&
            values.TryGetValue("active_income_source_count", out var sourceCount) &&
            int.TryParse(sourceCount, out var count) && count == 1)
            return result;

        var incomeShares = values
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(item => item.Key.StartsWith(”income_shares.”,
            // StringComparison.OrdinalIgnoreCase)) dalam BuildActualCalculation; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Where(item => item.Key.StartsWith("income_shares.", StringComparison.OrdinalIgnoreCase))
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .OrderBy(item => item.Key, StringComparer.OrdinalIgnoreCase) dalam
            // BuildActualCalculation; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .OrderBy(item => item.Key, StringComparer.OrdinalIgnoreCase)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Select(item => double.TryParse(item.Value, NumberStyles.Float,
            // CultureInfo.InvariantCulture, out var share) dalam BuildActualCalculation; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Select(item => double.TryParse(item.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var share)
                // Meneruskan perkalian antara `share` dan `100` sebagai argumen ke `FormatCalculationNumber`; Meneruskan `culture` (nilai culture) sebagai argumen
                // ke `FormatCalculationNumber`.
                ? $"({FormatCalculationNumber(share * 100, culture)} ÷ 100)²"
                // Meneruskan fungsi lambda `item => double.TryParse(item.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var share) ?
                // $”({FormatCalculationNumber(share * 100, culture)} ÷ 100)²” : $”({unavai...` yang dijalankan oleh operasi pemanggil untuk memproses setiap
                // masukan sebagai argumen ke `values .Where(item => item.Key.StartsWith(”income_shares.”, StringComparison.OrdinalIgnoreCase)) .OrderBy(item =>
                // item.Key, StringComparer.OrdinalIgnoreCase) .Select`.
                : $"({unavailableText} ÷ 100)²")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList(); dalam BuildActualCalculation; token pada baris ini menyambungkan
            // bagian kode sebelum dan sesudahnya.
            .ToList();

        // Menyiapkan variabel lokal `expression` untuk nilai expression dengan hasil pemetaan `analysisKey` melalui cabang pola switch yang cocok. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var expression = analysisKey switch
        // Membuka scope pemetaan switch atas `analysisKey`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildActualCalculation.
        {
            // Untuk pola `”net-worth”`, menghasilkan teks interpolasi `$”{Value(”coins_net_end_game”)} ÷ {Value(”starting_coins”)} × 100%”`; nilai ekspresi di
            // dalam kurung kurawal disisipkan saat program berjalan sebagai hasil switch.
            "net-worth" => $"({Value("coins_net_end_game")} − {Value("starting_coins")}) ÷ {Value("starting_coins")} × 100%",
            // Untuk pola `”income-diversification”`, menghasilkan teks interpolasi `$”[1 − ({(incomeShares.Count > 0 ? string.Join(” + ”, incomeShares) :
            // unavailableText)})] ÷ [1 − (1 ÷ {Value(”active_income_source_count”)})] × 100%”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program
            // berjalan sebagai hasil switch.
            "income-diversification" =>
                // Menggunakan teks interpolasi `$”[1 − ({(incomeShares.Count > 0 ? string.Join(” + ”, incomeShares) : unavailableText)})] ÷ [1 − (1 ÷
                // {Value(”active_income_source_count”)})] × 100%”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan sebagai bagian ekspresi
                // yang sedang disusun dalam BuildActualCalculation.
                $"[1 − ({(incomeShares.Count > 0 ? string.Join(" + ", incomeShares) : unavailableText)})] ÷ [1 − (1 ÷ {Value("active_income_source_count")})] × 100%",
            // Untuk pola `”expense-efficiency”`, menghasilkan teks interpolasi `$”{Value(”ingredient_investment_coins_total”)} ÷ {Value(”total_cash_out”)} ×
            // 100%”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan sebagai hasil switch.
            "expense-efficiency" => $"{Value("ingredient_investment_coins_total")} ÷ {Value("total_cash_out")} × 100%",
            // Untuk pola `”business-margin”`, menghasilkan teks interpolasi `$”({Value(”meal_order_income_total”)} − {Value(”ingredient_cost_used”)}) ÷
            // {Value(”meal_order_income_total”)} × 100%”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan sebagai hasil switch.
            "business-margin" =>
                // Menggunakan teks interpolasi `$”({Value(”meal_order_income_total”)} − {Value(”ingredient_cost_used”)}) ÷ {Value(”meal_order_income_total”)} ×
                // 100%”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan sebagai bagian ekspresi yang sedang disusun dalam
                // BuildActualCalculation.
                $"({Value("meal_order_income_total")} − {Value("ingredient_cost_used")}) ÷ {Value("meal_order_income_total")} × 100%",
            // Untuk pola `”risk-appetite”`, menghasilkan teks interpolasi `$”{Value(”risks_resolved_without_emergency”)} ÷ {Value(”life_risk_cards_drawn”)} ×
            // 100%”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan sebagai hasil switch.
            "risk-appetite" =>
                // Menggunakan teks interpolasi `$”{Value(”risks_resolved_without_emergency”)} ÷ {Value(”life_risk_cards_drawn”)} × 100%”`; nilai ekspresi di dalam
                // kurung kurawal disisipkan saat program berjalan sebagai bagian ekspresi yang sedang disusun dalam BuildActualCalculation.
                $"{Value("risks_resolved_without_emergency")} ÷ {Value("life_risk_cards_drawn")} × 100%",
            // Untuk pola `”debt-discipline”`, menghasilkan teks interpolasi `$”{Value(”outstanding_loan”)} ÷ ({Value(”outstanding_loan”)} +
            // {Value(”liquid_assets”)}) × 100%”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan sebagai hasil switch.
            "debt-discipline" =>
                // Menggunakan teks interpolasi `$”{Value(”outstanding_loan”)} ÷ ({Value(”outstanding_loan”)} + {Value(”liquid_assets”)}) × 100%”`; nilai ekspresi
                // di dalam kurung kurawal disisipkan saat program berjalan sebagai bagian ekspresi yang sedang disusun dalam BuildActualCalculation.
                $"{Value("outstanding_loan")} ÷ ({Value("outstanding_loan")} + {Value("liquid_assets")}) × 100%",
            // Untuk pola `”goal-ambition”`, menghasilkan teks interpolasi `$”{Value(”coins_committed_to_goals”)} ÷ {Value(”attempted_goal_target_total”)} ×
            // 100%”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan sebagai hasil switch.
            // Untuk pola `”action-efficiency”`, menghasilkan teks interpolasi `$”{Value(”income_main_actions”)} ÷ {Value(”total_main_actions”)} × 100%”`; nilai
            // ekspresi di dalam kurung kurawal disisipkan saat program berjalan sebagai hasil switch.
            "action-efficiency" =>
                // Menggunakan teks interpolasi `$”{Value(”income_main_actions”)} ÷ {Value(”total_main_actions”)} × 100%”`; nilai ekspresi di dalam kurung kurawal
                // disisipkan saat program berjalan sebagai bagian ekspresi yang sedang disusun dalam BuildActualCalculation.
                $"{Value("income_main_actions")} ÷ {Value("total_main_actions")} × 100%",
            // Untuk pola `”meal-success”`, menghasilkan teks interpolasi `$”{Value(”ingredients_used_in_completed_orders”)} ÷ {Value(”ingredients_collected”)}
            // × 100%”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan sebagai hasil switch.
            "meal-success" =>
                // Menggunakan teks interpolasi `$”{Value(”ingredients_used_in_completed_orders”)} ÷ {Value(”ingredients_collected”)} × 100%”`; nilai ekspresi di
                // dalam kurung kurawal disisipkan saat program berjalan sebagai bagian ekspresi yang sedang disusun dalam BuildActualCalculation.
                $"{Value("ingredients_used_in_completed_orders")} ÷ {Value("ingredients_collected")} × 100%",
            // Untuk pola `”planning-horizon”`, menghasilkan teks interpolasi `$”({Value(”saving_actions”)} + {Value(”financial_goal_actions”)} +
            // {Value(”insurance_actions”)} + {Value(”loan_repayment_actions”)}) ÷ {Value(”total_main_actions”)} × 100%”`; nilai ekspresi di dalam kurung
            // kurawal disisipkan saat program berjalan sebagai hasil switch.
            "planning-horizon" =>
                // Menggunakan teks interpolasi `$”({Value(”saving_actions”)} + {Value(”financial_goal_actions”)} + {Value(”insurance_actions”)} +
                // {Value(”loan_repayment_actions”)}) ÷ {Value(”total_main_actions”)} × 100%”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program
                // berjalan sebagai bagian ekspresi yang sedang disusun dalam BuildActualCalculation.
                $"({Value("saving_and_goal_actions")} + {Value("insurance_actions")} + {Value("loan_repayment_actions")}) ÷ {Value("total_main_actions")} × 100%",
            // Untuk pola `”fulfillment-diversity”`, menghasilkan teks interpolasi `$”[1 − ({Value(”primary_need_share”)}² + {Value(”secondary_need_share”)}² +
            // {Value(”tertiary_need_share”)}²)] ÷ (1 − ⅓) × 100%”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan sebagai hasil
            // switch.
            "fulfillment-diversity" =>
                // Menggunakan teks interpolasi `$”[1 − ({Value(”primary_need_share”)}² + {Value(”secondary_need_share”)}² + {Value(”tertiary_need_share”)}²)] ÷ (1
                // − ⅓) × 100%”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan sebagai bagian ekspresi yang sedang disusun dalam
                // BuildActualCalculation.
                $"[1 − ({Value("primary_need_share")}² + {Value("secondary_need_share")}² + {Value("tertiary_need_share")}²)] ÷ (1 − ⅓) × 100%",
            // Untuk pola `”donation-commitment”`, menghasilkan teks interpolasi `$”min(100, max(0, {Value(”donation_stability_index”)} ×
            // {Value(”donated_resource_share”)} × {Value(”friday_participation_rate”)}))”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program
            // berjalan sebagai hasil switch.
            "donation-commitment" =>
                // Menggunakan teks interpolasi `$”min(100, max(0, {Value(”donation_stability_index”)} × {Value(”donated_resource_share”)} ×
                // {Value(”friday_participation_rate”)}))”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan sebagai bagian ekspresi yang
                // sedang disusun dalam BuildActualCalculation.
                $"min(100, max(0, {Value("donation_stability_index")} × {Value("donated_resource_share")} × {Value("friday_participation_rate")}))",
            "happiness-portfolio" or "happiness-portfolio-beginner" => BuildHappinessDiversityCalculation(
                values, analysisKey == "happiness-portfolio", unavailableText, culture),
            _ => unavailableText
        // Menutup scope pemetaan switch atas `analysisKey`; bagian berikut berada di luar batas blok tersebut dalam BuildActualCalculation.
        };

        // Mengembalikan teks interpolasi `$”{expression} = {result}”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan kepada
        // pemanggil dalam BuildActualCalculation; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return $"{expression} = {result}";
    // Menutup scope metode BuildActualCalculation; bagian berikut berada di luar batas blok tersebut dalam BuildActualCalculation.
    }

    private static string BuildHappinessDiversityCalculation(
        IReadOnlyDictionary<string, string> values, bool advancedMode, string unavailableText, CultureInfo culture)
    {
        var paths = new List<string>
        {
            "need_card_points", "need_set_bonus_points", "donation_points", "gold_points", "pension_points"
        };
        if (advancedMode) paths.Add("financial_goal_points");
        var points = new List<double>();
        foreach (var path in paths)
        {
            if (!values.TryGetValue(path, out var rawValue) ||
                !double.TryParse(rawValue, NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
                return unavailableText;
            points.Add(Math.Max(0, value));
        }
        var total = points.Sum();
        if (total <= 0) return unavailableText;
        var terms = points.Select(value =>
            $"({FormatCalculationNumber(value, culture)} ÷ {FormatCalculationNumber(total, culture)})²");
        return $"[1 − ({string.Join(" + ", terms)})] ÷ (1 − 1/{points.Count}) × 100%";
    }

    // Mendefinisikan metode `FormatCalculationNumber` dengan hasil bertipe `string`; operasi ini menangani format calculation number. Masukan:
    // Parameter `value` bertipe `double` membawa nilai nilai; Parameter `culture` bertipe `CultureInfo` membawa nilai culture.
    private static string FormatCalculationNumber(double value, CultureInfo culture)
    // Membuka scope metode FormatCalculationNumber; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam FormatCalculationNumber.
    {
        // Mengembalikan hasil pemilihan bersyarat: ketika `Math.Abs(value - Math.Round(value)) < 0.0000001` benar gunakan `value.ToString(”N0”, culture)`,
        // jika tidak gunakan `value.ToString(”0.######”, culture)` kepada pemanggil dalam FormatCalculationNumber; eksekusi jalur ini selesai setelah nilai
        // hasil ditentukan.
        return Math.Abs(value - Math.Round(value)) < 0.0000001
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: value.ToString(”N0”, culture) dalam FormatCalculationNumber.
            ? value.ToString("N0", culture)
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: value.ToString(”0.######”, culture); dalam FormatCalculationNumber.
            : value.ToString("0.######", culture);
    // Menutup scope metode FormatCalculationNumber; bagian berikut berada di luar batas blok tersebut dalam FormatCalculationNumber.
    }
// Menutup scope tipe PlayerMetricCollectionHelper; bagian berikut berada di luar batas blok tersebut.
}
