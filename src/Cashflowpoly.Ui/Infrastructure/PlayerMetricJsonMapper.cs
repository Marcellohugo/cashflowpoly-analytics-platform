// Fungsi file: Menyediakan transformasi, lokalisasi, atau koneksi UI melalui PlayerMetricJsonMapper.
// Mengimpor namespace `System.Text.Json` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.Json;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Infrastructure` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Infrastructure;

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `PlayerMetricCollectionTable`; sealed mencegah tipe ini diturunkan
// lagi.
public sealed record PlayerMetricCollectionTable(
    // Parameter `Columns` bertipe `IReadOnlyList<string>` membawa nilai columns.
    IReadOnlyList<string> Columns,
    // Parameter `Rows` bertipe `IReadOnlyList<IReadOnlyList<string>>` membawa nilai baris.
    IReadOnlyList<IReadOnlyList<string>> Rows);

/// <summary>
/// Mapper JSON metrik gameplay menjadi struktur baris/grup yang siap dirender oleh Razor.
/// </summary>
// Mendefinisikan tipe class `PlayerMetricJsonMapper`.
public static class PlayerMetricJsonMapper
// Membuka scope tipe PlayerMetricJsonMapper; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    /// <summary>
    /// Memformat nilai leaf JsonElement menjadi teks tampilan.
    /// </summary>
    // Mendefinisikan metode `FormatJsonLeafValue` dengan hasil bertipe `string`. Memformat nilai leaf JsonElement menjadi teks tampilan. Masukan:
    // Parameter `element` bertipe `JsonElement` membawa nilai element; Parameter `trueText` bertipe `string` membawa nilai true text; Parameter
    // `falseText` bertipe `string` membawa nilai false text; Parameter `nullText` bertipe `string` membawa nilai null text.
    public static string FormatJsonLeafValue(JsonElement element, string trueText, string falseText, string nullText)
    // Membuka scope metode FormatJsonLeafValue; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam FormatJsonLeafValue.
    {
        // Mengembalikan hasil pemetaan `element.ValueKind` melalui cabang pola switch yang cocok kepada pemanggil dalam FormatJsonLeafValue; eksekusi jalur
        // ini selesai setelah nilai hasil ditentukan.
        return element.ValueKind switch
        // Membuka scope pemetaan switch atas `element.ValueKind`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam FormatJsonLeafValue.
        {
            // Untuk pola `JsonValueKind.String`, menghasilkan `element.GetString()` bila tidak null; jika null gunakan `string.Empty` sebagai nilai pengganti
            // sebagai hasil switch.
            JsonValueKind.String => element.GetString() ?? string.Empty,
            // Untuk pola `JsonValueKind.True`, menghasilkan `trueText` (nilai true text) sebagai hasil switch.
            JsonValueKind.True => trueText,
            // Untuk pola `JsonValueKind.False`, menghasilkan `falseText` (nilai false text) sebagai hasil switch.
            JsonValueKind.False => falseText,
            // Untuk pola `JsonValueKind.Number`, menghasilkan mengambil representasi JSON mentah dari `element` untuk disimpan atau diteruskan sebagai hasil
            // switch.
            JsonValueKind.Number => element.GetRawText(),
            // Untuk pola `JsonValueKind.Null`, menghasilkan `nullText` (nilai null text) sebagai hasil switch.
            JsonValueKind.Null => nullText,
            // Untuk pola `_`, menghasilkan mengambil representasi JSON mentah dari `element` untuk disimpan atau diteruskan sebagai hasil switch.
            _ => element.GetRawText()
        // Menutup scope pemetaan switch atas `element.ValueKind`; bagian berikut berada di luar batas blok tersebut dalam FormatJsonLeafValue.
        };
    // Menutup scope metode FormatJsonLeafValue; bagian berikut berada di luar batas blok tersebut dalam FormatJsonLeafValue.
    }

    /// <summary>
    /// Mengubah JSON bertingkat menjadi daftar path daun dan value tampilan.
    /// </summary>
    // Mendefinisikan metode `FlattenJsonLeaves` dengan hasil bertipe `List<(string Path, string Value)>`. Mengubah JSON bertingkat menjadi daftar path
    // daun dan value tampilan. Masukan: Parameter `root` bertipe `JsonElement?` membawa nilai root; nilai null diizinkan ketika data opsional belum
    // tersedia; Parameter `trueText` bertipe `string` membawa nilai true text; Parameter `falseText` bertipe `string` membawa nilai false text;
    // Parameter `nullText` bertipe `string` membawa nilai null text.
    public static List<(string Path, string Value)> FlattenJsonLeaves(
        // Parameter `root` bertipe `JsonElement?` membawa nilai root; nilai null diizinkan ketika data opsional belum tersedia.
        JsonElement? root,
        // Parameter `trueText` bertipe `string` membawa nilai true text.
        string trueText,
        // Parameter `falseText` bertipe `string` membawa nilai false text.
        string falseText,
        // Parameter `nullText` bertipe `string` membawa nilai null text.
        string nullText)
    // Membuka scope metode FlattenJsonLeaves; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam FlattenJsonLeaves.
    {
        // Menyiapkan variabel lokal `rows` untuk nilai baris dengan objek baru bertipe `List<(string Path, string Value)>` dengan nilai awal sesuai
        // konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var rows = new List<(string Path, string Value)>();
        // Memeriksa kebalikan kondisi `root.HasValue`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam FlattenJsonLeaves.
        if (!root.HasValue)
        // Membuka scope cabang if untuk kondisi `!root.HasValue`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam FlattenJsonLeaves.
        {
            // Mengembalikan `rows` (nilai baris) kepada pemanggil dalam FlattenJsonLeaves; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return rows;
        // Menutup scope cabang if untuk kondisi `!root.HasValue`; bagian berikut berada di luar batas blok tersebut dalam FlattenJsonLeaves.
        }

        // Menjalankan memanggil `Visit` dengan `root.Value`, `string.Empty`, `rows`, `trueText`, `falseText`, `nullText` dalam FlattenJsonLeaves.
        Visit(root.Value, string.Empty, rows, trueText, falseText, nullText);
        // Mengembalikan `rows` (nilai baris) kepada pemanggil dalam FlattenJsonLeaves; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return rows;
    // Menutup scope metode FlattenJsonLeaves; bagian berikut berada di luar batas blok tersebut dalam FlattenJsonLeaves.
    }

    /// <summary>
    /// Mengelompokkan properti top-level JSON, dengan scalar masuk ke grup summary.
    /// </summary>
    // Mendefinisikan metode `BuildMetricGroups` dengan hasil bertipe `List<(string GroupKey, List<(string Path, string Value)> Rows)>`. Mengelompokkan
    // properti top-level JSON, dengan scalar masuk ke grup summary. Masukan: Parameter `root` bertipe `JsonElement?` membawa nilai root; nilai null
    // diizinkan ketika data opsional belum tersedia; Parameter `trueText` bertipe `string` membawa nilai true text; Parameter `falseText` bertipe
    // `string` membawa nilai false text; Parameter `nullText` bertipe `string` membawa nilai null text.
    public static List<(string GroupKey, List<(string Path, string Value)> Rows)> BuildMetricGroups(
        // Parameter `root` bertipe `JsonElement?` membawa nilai root; nilai null diizinkan ketika data opsional belum tersedia.
        JsonElement? root,
        // Parameter `trueText` bertipe `string` membawa nilai true text.
        string trueText,
        // Parameter `falseText` bertipe `string` membawa nilai false text.
        string falseText,
        // Parameter `nullText` bertipe `string` membawa nilai null text.
        string nullText)
    // Membuka scope metode BuildMetricGroups; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildMetricGroups.
    {
        // Menyiapkan variabel lokal `groups` untuk nilai groups dengan objek baru bertipe `List<(string GroupKey, List<(string Path, string Value)> Rows)>`
        // dengan nilai awal sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var groups = new List<(string GroupKey, List<(string Path, string Value)> Rows)>();
        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!root.HasValue` dan `root.Value.ValueKind != JsonValueKind.Object`;
        // sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam BuildMetricGroups.
        if (!root.HasValue || root.Value.ValueKind != JsonValueKind.Object)
        // Membuka scope cabang if untuk kondisi `!root.HasValue || root.Value.ValueKind != JsonValueKind.Object`; pernyataan/deklarasi berikut berada di
        // dalam batas blok ini dalam BuildMetricGroups.
        {
            // Mengembalikan `groups` (nilai groups) kepada pemanggil dalam BuildMetricGroups; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return groups;
        // Menutup scope cabang if untuk kondisi `!root.HasValue || root.Value.ValueKind != JsonValueKind.Object`; bagian berikut berada di luar batas blok
        // tersebut dalam BuildMetricGroups.
        }

        // Menyiapkan variabel lokal `scalarRows` untuk nilai scalar baris dengan objek baru bertipe `List<(string Path, string Value)>` dengan nilai awal
        // sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var scalarRows = new List<(string Path, string Value)>();
        // Mengulangi setiap elemen `root.Value.EnumerateObject()`; elemen saat ini disimpan sebagai `property` bertipe `var` untuk diproses oleh badan loop
        // dalam BuildMetricGroups.
        foreach (var property in root.Value.EnumerateObject())
        // Membuka scope loop setiap property dari `root.Value.EnumerateObject()`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // BuildMetricGroups.
        {
            // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `property.Value.ValueKind == JsonValueKind.Object` dan
            // `property.Value.ValueKind == JsonValueKind.Array`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini
            // bernilai benar dalam BuildMetricGroups.
            if (property.Value.ValueKind == JsonValueKind.Object || property.Value.ValueKind == JsonValueKind.Array)
            // Membuka scope cabang if untuk kondisi `property.Value.ValueKind == JsonValueKind.Object || property.Value.ValueKind == JsonValueKind.Array`;
            // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildMetricGroups.
            {
                // Menyiapkan variabel lokal `rows` untuk nilai baris dengan memanggil `FlattenJsonLeaves` dengan `property.Value`, `trueText`, `falseText`,
                // `nullText`. Tipe variabel disimpulkan dari ekspresi nilai awal.
                var rows = FlattenJsonLeaves(property.Value, trueText, falseText, nullText);
                // Memeriksa perbandingan kesamaan antara `rows.Count` dan `0`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam BuildMetricGroups.
                if (rows.Count == 0)
                // Membuka scope cabang if untuk kondisi `rows.Count == 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildMetricGroups.
                {
                    // Menjalankan menambahkan `(”value”, FormatJsonLeafValue(property.Value, trueText, falseText, nullText))` ke `rows` dalam BuildMetricGroups.
                    rows.Add(("value", FormatJsonLeafValue(property.Value, trueText, falseText, nullText)));
                // Menutup scope cabang if untuk kondisi `rows.Count == 0`; bagian berikut berada di luar batas blok tersebut dalam BuildMetricGroups.
                }

                // Menjalankan menambahkan `(property.Name, rows)` ke `groups` dalam BuildMetricGroups.
                groups.Add((property.Name, rows));
                // Melewati sisa pernyataan pada iterasi saat ini dan melanjutkan ke elemen/iterasi berikutnya dalam BuildMetricGroups.
                continue;
            // Menutup scope cabang if untuk kondisi `property.Value.ValueKind == JsonValueKind.Object || property.Value.ValueKind == JsonValueKind.Array`;
            // bagian berikut berada di luar batas blok tersebut dalam BuildMetricGroups.
            }

            // Menjalankan menambahkan `(property.Name, FormatJsonLeafValue(property.Value, trueText, falseText, nullText))` ke `scalarRows` dalam
            // BuildMetricGroups.
            scalarRows.Add((property.Name, FormatJsonLeafValue(property.Value, trueText, falseText, nullText)));
        // Menutup scope loop setiap property dari `root.Value.EnumerateObject()`; bagian berikut berada di luar batas blok tersebut dalam
        // BuildMetricGroups.
        }

        // Memeriksa pemeriksaan lebih besar antara `scalarRows.Count` dan `0`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // BuildMetricGroups.
        if (scalarRows.Count > 0)
        // Membuka scope cabang if untuk kondisi `scalarRows.Count > 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // BuildMetricGroups.
        {
            // Menjalankan memanggil `groups.Insert` dengan `0`, `(”summary”, scalarRows)` dalam BuildMetricGroups.
            groups.Insert(0, ("summary", scalarRows));
        // Menutup scope cabang if untuk kondisi `scalarRows.Count > 0`; bagian berikut berada di luar batas blok tersebut dalam BuildMetricGroups.
        }

        // Mengembalikan `groups` (nilai groups) kepada pemanggil dalam BuildMetricGroups; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return groups;
    // Menutup scope metode BuildMetricGroups; bagian berikut berada di luar batas blok tersebut dalam BuildMetricGroups.
    }

    /// <summary>
    /// Mengelompokkan variabel top-level tanpa memecah isi array menjadi kartu per item.
    /// Objek tetap diratakan agar subvariabel seperti profil kebutuhan dapat dibaca sendiri.
    /// </summary>
    // Mendefinisikan metode `BuildMetricVariableGroups` dengan hasil bertipe `List<(string GroupKey, List<(string Path, string Value)> Rows)>`.
    // Mengelompokkan variabel top-level tanpa memecah isi array menjadi kartu per item. Objek tetap diratakan agar subvariabel seperti profil kebutuhan
    // dapat dibaca sendiri. Masukan: Parameter `root` bertipe `JsonElement?` membawa nilai root; nilai null diizinkan ketika data opsional belum
    // tersedia; Parameter `trueText` bertipe `string` membawa nilai true text; Parameter `falseText` bertipe `string` membawa nilai false text;
    // Parameter `nullText` bertipe `string` membawa nilai null text.
    public static List<(string GroupKey, List<(string Path, string Value)> Rows)> BuildMetricVariableGroups(
        // Parameter `root` bertipe `JsonElement?` membawa nilai root; nilai null diizinkan ketika data opsional belum tersedia.
        JsonElement? root,
        // Parameter `trueText` bertipe `string` membawa nilai true text.
        string trueText,
        // Parameter `falseText` bertipe `string` membawa nilai false text.
        string falseText,
        // Parameter `nullText` bertipe `string` membawa nilai null text.
        string nullText)
    // Membuka scope metode BuildMetricVariableGroups; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildMetricVariableGroups.
    {
        // Menyiapkan variabel lokal `groups` untuk nilai groups dengan objek baru bertipe `List<(string GroupKey, List<(string Path, string Value)> Rows)>`
        // dengan nilai awal sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var groups = new List<(string GroupKey, List<(string Path, string Value)> Rows)>();
        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!root.HasValue` dan `root.Value.ValueKind != JsonValueKind.Object`;
        // sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam BuildMetricVariableGroups.
        if (!root.HasValue || root.Value.ValueKind != JsonValueKind.Object)
        // Membuka scope cabang if untuk kondisi `!root.HasValue || root.Value.ValueKind != JsonValueKind.Object`; pernyataan/deklarasi berikut berada di
        // dalam batas blok ini dalam BuildMetricVariableGroups.
        {
            // Mengembalikan `groups` (nilai groups) kepada pemanggil dalam BuildMetricVariableGroups; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return groups;
        // Menutup scope cabang if untuk kondisi `!root.HasValue || root.Value.ValueKind != JsonValueKind.Object`; bagian berikut berada di luar batas blok
        // tersebut dalam BuildMetricVariableGroups.
        }

        // Mengulangi setiap elemen `root.Value.EnumerateObject()`; elemen saat ini disimpan sebagai `group` bertipe `var` untuk diproses oleh badan loop
        // dalam BuildMetricVariableGroups.
        foreach (var group in root.Value.EnumerateObject())
        // Membuka scope loop setiap group dari `root.Value.EnumerateObject()`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // BuildMetricVariableGroups.
        {
            // Memeriksa perbandingan ketidaksamaan antara `group.Value.ValueKind` dan `JsonValueKind.Object`; blok if hanya dijalankan ketika kondisi ini
            // bernilai benar dalam BuildMetricVariableGroups.
            if (group.Value.ValueKind != JsonValueKind.Object)
            // Membuka scope cabang if untuk kondisi `group.Value.ValueKind != JsonValueKind.Object`; pernyataan/deklarasi berikut berada di dalam batas blok
            // ini dalam BuildMetricVariableGroups.
            {
                // Menjalankan menambahkan `(group.Name, new List<(string Path, string Value)> { (group.Name, FormatJsonLeafValue(group.Value, trueText, falseText,
                // nullText)) })` ke `groups` dalam BuildMetricVariableGroups.
                groups.Add((group.Name, new List<(string Path, string Value)>
                // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // BuildMetricVariableGroups.
                {
                    // Meneruskan `group.Name` (nilai nama) sebagai argumen ke konstruktor `List<(string Path, string Value)>`; Meneruskan memanggil
                    // `FormatJsonLeafValue` dengan `group.Value`, `trueText`, `falseText`, `nullText` sebagai argumen ke konstruktor `List<(string Path, string
                    // Value)>`; Meneruskan `group.Value`, yaitu nilai yang dibungkus objek/nullable sebagai argumen ke `FormatJsonLeafValue`; Meneruskan `trueText`
                    // (nilai true text) sebagai argumen ke `FormatJsonLeafValue`; Meneruskan `falseText` (nilai false text) sebagai argumen ke `FormatJsonLeafValue`;
                    // Meneruskan `nullText` (nilai null text) sebagai argumen ke `FormatJsonLeafValue`.
                    (group.Name, FormatJsonLeafValue(group.Value, trueText, falseText, nullText))
                // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam BuildMetricVariableGroups.
                }));
                // Melewati sisa pernyataan pada iterasi saat ini dan melanjutkan ke elemen/iterasi berikutnya dalam BuildMetricVariableGroups.
                continue;
            // Menutup scope cabang if untuk kondisi `group.Value.ValueKind != JsonValueKind.Object`; bagian berikut berada di luar batas blok tersebut dalam
            // BuildMetricVariableGroups.
            }

            // Menyiapkan variabel lokal `rows` untuk nilai baris dengan objek baru bertipe `List<(string Path, string Value)>` dengan nilai awal sesuai
            // konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var rows = new List<(string Path, string Value)>();
            // Menjalankan memanggil `VisitVariables` dengan `group.Value`, `string.Empty`, `rows`, `trueText`, `falseText`, `nullText` dalam
            // BuildMetricVariableGroups.
            VisitVariables(group.Value, string.Empty, rows, trueText, falseText, nullText);
            // Menjalankan menambahkan `(group.Name, rows)` ke `groups` dalam BuildMetricVariableGroups.
            groups.Add((group.Name, rows));
        // Menutup scope loop setiap group dari `root.Value.EnumerateObject()`; bagian berikut berada di luar batas blok tersebut dalam
        // BuildMetricVariableGroups.
        }

        // Mengembalikan `groups` (nilai groups) kepada pemanggil dalam BuildMetricVariableGroups; eksekusi jalur ini selesai setelah nilai hasil
        // ditentukan.
        return groups;
    // Menutup scope metode BuildMetricVariableGroups; bagian berikut berada di luar batas blok tersebut dalam BuildMetricVariableGroups.
    }

    /// <summary>
    /// Membangun map properti top-level JSON ke elemen clone agar aman dipakai setelah JsonDocument asal dispose.
    /// </summary>
    // Mendefinisikan metode `BuildMetricGroupElements` dengan hasil bertipe `Dictionary<string, JsonElement>`. Membangun map properti top-level JSON ke
    // elemen clone agar aman dipakai setelah JsonDocument asal dispose. Masukan: Parameter `root` bertipe `JsonElement?` membawa nilai root; nilai null
    // diizinkan ketika data opsional belum tersedia.
    public static Dictionary<string, JsonElement> BuildMetricGroupElements(JsonElement? root)
    // Membuka scope metode BuildMetricGroupElements; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildMetricGroupElements.
    {
        // Menyiapkan variabel lokal `groups` untuk nilai groups dengan objek baru bertipe `Dictionary<string, JsonElement>` dengan argumen
        // (StringComparer.OrdinalIgnoreCase). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var groups = new Dictionary<string, JsonElement>(StringComparer.OrdinalIgnoreCase);
        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!root.HasValue` dan `root.Value.ValueKind != JsonValueKind.Object`;
        // sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam BuildMetricGroupElements.
        if (!root.HasValue || root.Value.ValueKind != JsonValueKind.Object)
        // Membuka scope cabang if untuk kondisi `!root.HasValue || root.Value.ValueKind != JsonValueKind.Object`; pernyataan/deklarasi berikut berada di
        // dalam batas blok ini dalam BuildMetricGroupElements.
        {
            // Mengembalikan `groups` (nilai groups) kepada pemanggil dalam BuildMetricGroupElements; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return groups;
        // Menutup scope cabang if untuk kondisi `!root.HasValue || root.Value.ValueKind != JsonValueKind.Object`; bagian berikut berada di luar batas blok
        // tersebut dalam BuildMetricGroupElements.
        }

        // Mengulangi setiap elemen `root.Value.EnumerateObject()`; elemen saat ini disimpan sebagai `property` bertipe `var` untuk diproses oleh badan loop
        // dalam BuildMetricGroupElements.
        foreach (var property in root.Value.EnumerateObject())
        // Membuka scope loop setiap property dari `root.Value.EnumerateObject()`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // BuildMetricGroupElements.
        {
            // Memperbarui `groups[property.Name]` menggunakan membuat salinan `property.Value` agar hasil dapat digunakan terpisah dari objek sumber dalam
            // BuildMetricGroupElements.
            groups[property.Name] = property.Value.Clone();
        // Menutup scope loop setiap property dari `root.Value.EnumerateObject()`; bagian berikut berada di luar batas blok tersebut dalam
        // BuildMetricGroupElements.
        }

        // Mengembalikan `groups` (nilai groups) kepada pemanggil dalam BuildMetricGroupElements; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return groups;
    // Menutup scope metode BuildMetricGroupElements; bagian berikut berada di luar batas blok tersebut dalam BuildMetricGroupElements.
    }

    /// <summary>
    /// Membaca nilai numerik dari JsonElement number atau string numerik.
    /// </summary>
    // Mendefinisikan metode `TryGetNumericValue` dengan hasil bertipe `bool`. Membaca nilai numerik dari JsonElement number atau string numerik.
    // Masukan: Parameter `element` bertipe `JsonElement` membawa nilai element; Parameter `value` bertipe `double` membawa nilai nilai; out
    // mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    public static bool TryGetNumericValue(JsonElement element, out double value)
    // Membuka scope metode TryGetNumericValue; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryGetNumericValue.
    {
        // Memilih cabang berdasarkan `element.ValueKind` (nilai nilai kind); label case menentukan perlakuan untuk setiap nilai yang dikenali dalam
        // TryGetNumericValue.
        switch (element.ValueKind)
        // Membuka scope pemilihan switch atas `element.ValueKind`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryGetNumericValue.
        {
            // Menetapkan label cabang `case JsonValueKind.Number:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            case JsonValueKind.Number:
                // Mengembalikan memanggil `element.TryGetDouble` dengan `value` kepada pemanggil dalam TryGetNumericValue; eksekusi jalur ini selesai setelah nilai
                // hasil ditentukan.
                return element.TryGetDouble(out value);
            // Menetapkan label cabang `case JsonValueKind.String:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            case JsonValueKind.String:
                // Memeriksa mencoba mengonversi `element.GetString()`, `var parsed` melalui `double.TryParse`; keberhasilan dilaporkan sebagai boolean dan hasil
                // ditempatkan pada argumen out; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam TryGetNumericValue.
                if (double.TryParse(element.GetString(), out var parsed))
                // Membuka scope cabang if untuk kondisi `double.TryParse(element.GetString(), out var parsed)`; pernyataan/deklarasi berikut berada di dalam batas
                // blok ini dalam TryGetNumericValue.
                {
                    // Memperbarui `value` menggunakan `parsed` (nilai parsed) dalam TryGetNumericValue.
                    value = parsed;
                    // Mengembalikan true, yaitu kondisi aktif/terpenuhi kepada pemanggil dalam TryGetNumericValue; eksekusi jalur ini selesai setelah nilai hasil
                    // ditentukan.
                    return true;
                // Menutup scope cabang if untuk kondisi `double.TryParse(element.GetString(), out var parsed)`; bagian berikut berada di luar batas blok tersebut
                // dalam TryGetNumericValue.
                }

                // Mengakhiri loop atau cabang switch terdekat, kemudian melanjutkan setelah blok tersebut dalam TryGetNumericValue.
                break;
        // Menutup scope pemilihan switch atas `element.ValueKind`; bagian berikut berada di luar batas blok tersebut dalam TryGetNumericValue.
        }

        // Memperbarui `value` menggunakan nilai literal `0` dalam TryGetNumericValue.
        value = 0;
        // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryGetNumericValue; eksekusi jalur ini selesai setelah nilai
        // hasil ditentukan.
        return false;
    // Menutup scope metode TryGetNumericValue; bagian berikut berada di luar batas blok tersebut dalam TryGetNumericValue.
    }

    /// <summary>
    /// Mengubah array atau object JSON menjadi tabel sederhana untuk tampilan manusia.
    /// </summary>
    // Mendefinisikan metode `BuildCollectionTable` dengan hasil bertipe `PlayerMetricCollectionTable?`. Mengubah array atau object JSON menjadi tabel
    // sederhana untuk tampilan manusia. Masukan: Parameter `rawValue` bertipe `string` membawa nilai raw nilai; Parameter `trueText` bertipe `string`
    // membawa nilai true text; Parameter `falseText` bertipe `string` membawa nilai false text; Parameter `nullText` bertipe `string` membawa nilai
    // null text.
    public static PlayerMetricCollectionTable? BuildCollectionTable(
        // Parameter `rawValue` bertipe `string` membawa nilai raw nilai.
        string rawValue,
        // Parameter `trueText` bertipe `string` membawa nilai true text.
        string trueText,
        // Parameter `falseText` bertipe `string` membawa nilai false text.
        string falseText,
        // Parameter `nullText` bertipe `string` membawa nilai null text.
        string nullText)
    // Membuka scope metode BuildCollectionTable; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildCollectionTable.
    {
        // Memulai blok try dalam BuildCollectionTable; exception dari blok ini dapat dialihkan ke catch, sedangkan finally (jika ada) tetap dijalankan saat
        // keluar.
        try
        // Membuka scope penanganan operasi try; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildCollectionTable.
        {
            // Menyiapkan variabel lokal `document` untuk nilai document dengan memanggil `JsonDocument.Parse` dengan `rawValue`. Tipe variabel disimpulkan dari
            // ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
            using var document = JsonDocument.Parse(rawValue);
            // Menyiapkan variabel lokal `root` untuk nilai root dengan `document.RootElement` (nilai root element). Tipe variabel disimpulkan dari ekspresi
            // nilai awal.
            var root = document.RootElement;

            // Memeriksa perbandingan kesamaan antara `root.ValueKind` dan `JsonValueKind.Object`; blok if hanya dijalankan ketika kondisi ini bernilai benar
            // dalam BuildCollectionTable.
            if (root.ValueKind == JsonValueKind.Object)
            // Membuka scope cabang if untuk kondisi `root.ValueKind == JsonValueKind.Object`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // BuildCollectionTable.
            {
                // Menyiapkan variabel lokal `rows` untuk nilai baris dengan mematerialisasi urutan `root.EnumerateObject() .Select(property =>
                // (IReadOnlyList<string>)new[] { property.Name, FormatCollectionValue(property.Value, trueText, falseText, nullText) })` menjadi List; enumerasi
                // dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
                var rows = root.EnumerateObject()
                    // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Select(property => (IReadOnlyList<string>)new[] dalam BuildCollectionTable;
                    // token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
                    .Select(property => (IReadOnlyList<string>)new[]
                    // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                    // BuildCollectionTable.
                    {
                        // Meneruskan fungsi lambda `property => (IReadOnlyList<string>)new[] { property.Name, FormatCollectionValue(property.Value, trueText, falseText,
                        // nullText) }` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen ke `root.EnumerateObject() .Select`.
                        property.Name,
                        // Meneruskan `property.Value`, yaitu nilai yang dibungkus objek/nullable sebagai argumen ke `FormatCollectionValue`; Meneruskan `trueText` (nilai
                        // true text) sebagai argumen ke `FormatCollectionValue`; Meneruskan `falseText` (nilai false text) sebagai argumen ke `FormatCollectionValue`;
                        // Meneruskan `nullText` (nilai null text) sebagai argumen ke `FormatCollectionValue`.
                        FormatCollectionValue(property.Value, trueText, falseText, nullText)
                    // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam BuildCollectionTable.
                    })
                    // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList(); dalam BuildCollectionTable; token pada baris ini menyambungkan
                    // bagian kode sebelum dan sesudahnya.
                    .ToList();
                // Mengembalikan hasil pemilihan bersyarat: ketika `rows.Count == 0` benar gunakan `null`, jika tidak gunakan `new PlayerMetricCollectionTable(new[]
                // { ”series”, ”value” }, rows)` kepada pemanggil dalam BuildCollectionTable; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
                return rows.Count == 0
                    // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: null dalam BuildCollectionTable.
                    ? null
                    // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: new PlayerMetricCollectionTable(new[] { ”series”, ”value” }, rows);
                    // dalam BuildCollectionTable.
                    : new PlayerMetricCollectionTable(new[] { "series", "value" }, rows);
            // Menutup scope cabang if untuk kondisi `root.ValueKind == JsonValueKind.Object`; bagian berikut berada di luar batas blok tersebut dalam
            // BuildCollectionTable.
            }

            // Memeriksa perbandingan ketidaksamaan antara `root.ValueKind` dan `JsonValueKind.Array`; blok if hanya dijalankan ketika kondisi ini bernilai
            // benar dalam BuildCollectionTable.
            if (root.ValueKind != JsonValueKind.Array)
            // Membuka scope cabang if untuk kondisi `root.ValueKind != JsonValueKind.Array`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // BuildCollectionTable.
            {
                // Mengembalikan null, yaitu penanda tidak ada nilai kepada pemanggil dalam BuildCollectionTable; eksekusi jalur ini selesai setelah nilai hasil
                // ditentukan.
                return null;
            // Menutup scope cabang if untuk kondisi `root.ValueKind != JsonValueKind.Array`; bagian berikut berada di luar batas blok tersebut dalam
            // BuildCollectionTable.
            }

            // Menyiapkan variabel lokal `items` untuk nilai elemen dengan mematerialisasi urutan `root.EnumerateArray()` menjadi List; enumerasi dijalankan dan
            // hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var items = root.EnumerateArray().ToList();
            // Memeriksa perbandingan kesamaan antara `items.Count` dan `0`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
            // BuildCollectionTable.
            if (items.Count == 0)
            // Membuka scope cabang if untuk kondisi `items.Count == 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildCollectionTable.
            {
                // Mengembalikan null, yaitu penanda tidak ada nilai kepada pemanggil dalam BuildCollectionTable; eksekusi jalur ini selesai setelah nilai hasil
                // ditentukan.
                return null;
            // Menutup scope cabang if untuk kondisi `items.Count == 0`; bagian berikut berada di luar batas blok tersebut dalam BuildCollectionTable.
            }

            // Memeriksa memeriksa apakah seluruh elemen `items` memenuhi `item => item.ValueKind == JsonValueKind.Object`; koleksi kosong menghasilkan true;
            // blok if hanya dijalankan ketika kondisi ini bernilai benar dalam BuildCollectionTable.
            if (items.All(item => item.ValueKind == JsonValueKind.Object))
            // Membuka scope cabang if untuk kondisi `items.All(item => item.ValueKind == JsonValueKind.Object)`; pernyataan/deklarasi berikut berada di dalam
            // batas blok ini dalam BuildCollectionTable.
            {
                // Menyiapkan variabel lokal `columns` untuk nilai columns dengan mematerialisasi urutan `items .SelectMany(item =>
                // item.EnumerateObject().Select(property => property.Name)) .Distinct(StringComparer.OrdinalIgnoreCase)` menjadi List; enumerasi dijalankan dan
                // hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
                var columns = items
                    // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .SelectMany(item => item.EnumerateObject().Select(property => property.Name))
                    // dalam BuildCollectionTable; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
                    .SelectMany(item => item.EnumerateObject().Select(property => property.Name))
                    // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Distinct(StringComparer.OrdinalIgnoreCase) dalam BuildCollectionTable; token
                    // pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList(); dalam BuildCollectionTable; token pada baris ini menyambungkan
                    // bagian kode sebelum dan sesudahnya.
                    .ToList();
                // Menyiapkan variabel lokal `rows` untuk nilai baris dengan mematerialisasi urutan `items .Select(item => (IReadOnlyList<string>)columns
                // .Select(column => item.TryGetProperty(column, out var value) ? FormatCollectionValue(value, trueText, falseText, nullText)...` menjadi List;
                // enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
                var rows = items
                    // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Select(item => (IReadOnlyList<string>)columns dalam BuildCollectionTable;
                    // token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
                    .Select(item => (IReadOnlyList<string>)columns
                        // Meneruskan fungsi lambda `column => item.TryGetProperty(column, out var value) ? FormatCollectionValue(value, trueText, falseText, nullText) :
                        // nullText` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen ke `columns .Select`; Meneruskan `column` (nilai
                        // column) sebagai argumen ke `item.TryGetProperty`; Meneruskan `var value` sebagai argumen ke `item.TryGetProperty`.
                        .Select(column => item.TryGetProperty(column, out var value)
                            // Meneruskan `value` (nilai nilai) sebagai argumen ke `FormatCollectionValue`; Meneruskan `trueText` (nilai true text) sebagai argumen ke
                            // `FormatCollectionValue`; Meneruskan `falseText` (nilai false text) sebagai argumen ke `FormatCollectionValue`; Meneruskan `nullText` (nilai null
                            // text) sebagai argumen ke `FormatCollectionValue`.
                            ? FormatCollectionValue(value, trueText, falseText, nullText)
                            // Meneruskan fungsi lambda `column => item.TryGetProperty(column, out var value) ? FormatCollectionValue(value, trueText, falseText, nullText) :
                            // nullText` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen ke `columns .Select`.
                            : nullText)
                        // Meneruskan fungsi lambda `item => (IReadOnlyList<string>)columns .Select(column => item.TryGetProperty(column, out var value) ?
                        // FormatCollectionValue(value, trueText, falseText, nullText) : nullText) ....` yang dijalankan oleh operasi pemanggil untuk memproses setiap
                        // masukan sebagai argumen ke `items .Select`.
                        .ToList())
                    // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList(); dalam BuildCollectionTable; token pada baris ini menyambungkan
                    // bagian kode sebelum dan sesudahnya.
                    .ToList();
                // Mengembalikan objek baru bertipe `PlayerMetricCollectionTable` dengan argumen (columns, rows) kepada pemanggil dalam BuildCollectionTable;
                // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
                return new PlayerMetricCollectionTable(columns, rows);
            // Menutup scope cabang if untuk kondisi `items.All(item => item.ValueKind == JsonValueKind.Object)`; bagian berikut berada di luar batas blok
            // tersebut dalam BuildCollectionTable.
            }

            // Mengembalikan objek baru bertipe `PlayerMetricCollectionTable` dengan argumen ( new[] { ”item_index”, ”value” }, items.Select((item, index) =>
            // (IReadOnlyList<string>)new[] { (index + 1).ToString(System.Globalization.CultureInfo.InvariantC... kepada pemanggil dalam BuildCollectionTable;
            // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return new PlayerMetricCollectionTable(
                // Meneruskan array baru dengan tipe elemen disimpulkan dari nilai initializer sebagai argumen ke konstruktor `PlayerMetricCollectionTable`.
                new[] { "item_index", "value" },
                // Meneruskan mematerialisasi urutan `items.Select((item, index) => (IReadOnlyList<string>)new[] { (index +
                // 1).ToString(System.Globalization.CultureInfo.InvariantCulture), FormatCollectionValue(item, trueText, fal...` menjadi List; enumerasi dijalankan
                // dan hasilnya disimpan dalam memori sebagai argumen ke konstruktor `PlayerMetricCollectionTable`; Meneruskan fungsi lambda `(item, index) =>
                // (IReadOnlyList<string>)new[] { (index + 1).ToString(System.Globalization.CultureInfo.InvariantCulture), FormatCollectionValue(item, trueText,
                // falseText, nullT...` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen ke `items.Select`.
                items.Select((item, index) => (IReadOnlyList<string>)new[]
                // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // BuildCollectionTable.
                {
                    // Meneruskan `System.Globalization.CultureInfo.InvariantCulture` (nilai invariant culture) sebagai argumen ke `(index + 1).ToString`.
                    (index + 1).ToString(System.Globalization.CultureInfo.InvariantCulture),
                    // Meneruskan `item` (nilai elemen) sebagai argumen ke `FormatCollectionValue`; Meneruskan `trueText` (nilai true text) sebagai argumen ke
                    // `FormatCollectionValue`; Meneruskan `falseText` (nilai false text) sebagai argumen ke `FormatCollectionValue`; Meneruskan `nullText` (nilai null
                    // text) sebagai argumen ke `FormatCollectionValue`.
                    FormatCollectionValue(item, trueText, falseText, nullText)
                // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam BuildCollectionTable.
                }).ToList());
        // Menutup scope penanganan operasi try; bagian berikut berada di luar batas blok tersebut dalam BuildCollectionTable.
        }
        // Menangani exception `JsonException` melalui variabel dalam BuildCollectionTable.
        catch (JsonException)
        // Membuka scope penanganan exception catch; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildCollectionTable.
        {
            // Mengembalikan null, yaitu penanda tidak ada nilai kepada pemanggil dalam BuildCollectionTable; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return null;
        // Menutup scope penanganan exception catch; bagian berikut berada di luar batas blok tersebut dalam BuildCollectionTable.
        }
    // Menutup scope metode BuildCollectionTable; bagian berikut berada di luar batas blok tersebut dalam BuildCollectionTable.
    }

    /// <summary>
    /// Memasangkan jumlah dan peringkat donasi berdasarkan hari, bukan posisi array.
    /// </summary>
    // Mendefinisikan metode `BuildDonationHistoryJson` dengan hasil bertipe `string`. Memasangkan jumlah dan peringkat donasi berdasarkan hari, bukan
    // posisi array. Masukan: Parameter `amountsRawValue` bertipe `string` membawa nilai amounts raw nilai; Parameter `ranksRawValue` bertipe `string`
    // membawa nilai ranks raw nilai.
    public static string BuildDonationHistoryJson(string amountsRawValue, string ranksRawValue)
    // Membuka scope metode BuildDonationHistoryJson; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildDonationHistoryJson.
    {
        // Menyiapkan variabel lokal `donations` untuk nilai donations dengan objek baru bertipe `SortedDictionary<int, Dictionary<string, object?>>` dengan
        // nilai awal sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var donations = new SortedDictionary<int, Dictionary<string, object?>>();
        // Melengkapi struktur ekspresi ForEachVariableStatement melalui foreach (var (rawValue, property) in new[] { (amountsRawValue, ”amount”),
        // (ranksRawValue, ”rank”) }) dalam BuildDonationHistoryJson; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
        foreach (var (rawValue, property) in new[] { (amountsRawValue, "amount"), (ranksRawValue, "rank") })
        // Membuka scope blok ForEachVariableStatement; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildDonationHistoryJson.
        {
            // Memulai blok try dalam BuildDonationHistoryJson; exception dari blok ini dapat dialihkan ke catch, sedangkan finally (jika ada) tetap dijalankan
            // saat keluar.
            try
            // Membuka scope penanganan operasi try; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildDonationHistoryJson.
            {
                // Menyiapkan variabel lokal `document` untuk nilai document dengan memanggil `JsonDocument.Parse` dengan `rawValue`. Tipe variabel disimpulkan dari
                // ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
                using var document = JsonDocument.Parse(rawValue);
                // Memeriksa perbandingan ketidaksamaan antara `document.RootElement.ValueKind` dan `JsonValueKind.Array`; blok if hanya dijalankan ketika kondisi
                // ini bernilai benar dalam BuildDonationHistoryJson.
                if (document.RootElement.ValueKind != JsonValueKind.Array)
                // Membuka scope cabang if untuk kondisi `document.RootElement.ValueKind != JsonValueKind.Array`; pernyataan/deklarasi berikut berada di dalam batas
                // blok ini dalam BuildDonationHistoryJson.
                {
                    // Melewati sisa pernyataan pada iterasi saat ini dan melanjutkan ke elemen/iterasi berikutnya dalam BuildDonationHistoryJson.
                    continue;
                // Menutup scope cabang if untuk kondisi `document.RootElement.ValueKind != JsonValueKind.Array`; bagian berikut berada di luar batas blok tersebut
                // dalam BuildDonationHistoryJson.
                }

                // Mengulangi setiap elemen `document.RootElement.EnumerateArray()`; elemen saat ini disimpan sebagai `item` bertipe `var` untuk diproses oleh badan
                // loop dalam BuildDonationHistoryJson.
                foreach (var item in document.RootElement.EnumerateArray())
                // Membuka scope loop setiap item dari `document.RootElement.EnumerateArray()`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // BuildDonationHistoryJson.
                {
                    // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `item.ValueKind != JsonValueKind.Object ||
                    // !item.TryGetProperty(”day_index”, out var dayValue) || dayValue.ValueKind != JsonValueKind.Number || !dayValue.TryGetInt32(out var da...` dan
                    // `day <= 0`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
                    // BuildDonationHistoryJson.
                    if (item.ValueKind != JsonValueKind.Object
                        // Melengkapi struktur ekspresi LogicalOrExpression melalui || !item.TryGetProperty(”day_index”, out var dayValue) dalam BuildDonationHistoryJson;
                        // token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
                        || !item.TryGetProperty("day_index", out var dayValue)
                        // Melengkapi struktur ekspresi LogicalOrExpression melalui || dayValue.ValueKind != JsonValueKind.Number dalam BuildDonationHistoryJson; token pada
                        // baris ini menyambungkan bagian kode sebelum dan sesudahnya.
                        || dayValue.ValueKind != JsonValueKind.Number
                        // Melengkapi struktur ekspresi LogicalOrExpression melalui || !dayValue.TryGetInt32(out var day) dalam BuildDonationHistoryJson; token pada baris
                        // ini menyambungkan bagian kode sebelum dan sesudahnya.
                        || !dayValue.TryGetInt32(out var day)
                        // Melengkapi struktur ekspresi LogicalOrExpression melalui || day <= 0) dalam BuildDonationHistoryJson; token pada baris ini menyambungkan bagian
                        // kode sebelum dan sesudahnya.
                        || day <= 0)
                    // Membuka scope cabang if untuk kondisi `item.ValueKind != JsonValueKind.Object || !item.TryGetProperty(”day_index”, out var dayValue) ||
                    // dayValue.ValueKind != JsonValueKind.Number || !dayValue.TryGetInt32(out var da...`; pernyataan/deklarasi berikut berada di dalam batas blok ini
                    // dalam BuildDonationHistoryJson.
                    {
                        // Melewati sisa pernyataan pada iterasi saat ini dan melanjutkan ke elemen/iterasi berikutnya dalam BuildDonationHistoryJson.
                        continue;
                    // Menutup scope cabang if untuk kondisi `item.ValueKind != JsonValueKind.Object || !item.TryGetProperty(”day_index”, out var dayValue) ||
                    // dayValue.ValueKind != JsonValueKind.Number || !dayValue.TryGetInt32(out var da...`; bagian berikut berada di luar batas blok tersebut dalam
                    // BuildDonationHistoryJson.
                    }

                    // Memeriksa kebalikan kondisi `donations.TryGetValue(day, out var values)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
                    // BuildDonationHistoryJson.
                    if (!donations.TryGetValue(day, out var values))
                    // Membuka scope cabang if untuk kondisi `!donations.TryGetValue(day, out var values)`; pernyataan/deklarasi berikut berada di dalam batas blok ini
                    // dalam BuildDonationHistoryJson.
                    {
                        // Memperbarui `values` menggunakan objek baru bertipe `Dictionary<string, object?>` dengan nilai awal sesuai konstruktornya dalam
                        // BuildDonationHistoryJson.
                        values = new Dictionary<string, object?>
                        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                        // BuildDonationHistoryJson.
                        {
                            // Memperbarui `[”day_index”]` menggunakan `day` (nomor hari permainan yang menjadi konteks aktivitas) dalam BuildDonationHistoryJson.
                            ["day_index"] = day,
                            // Memperbarui `[”amount”]` menggunakan null, yaitu penanda tidak ada nilai dalam BuildDonationHistoryJson.
                            ["amount"] = null,
                            // Memperbarui `[”rank”]` menggunakan null, yaitu penanda tidak ada nilai dalam BuildDonationHistoryJson.
                            ["rank"] = null
                        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam BuildDonationHistoryJson.
                        };
                        // Memperbarui `donations[day]` menggunakan `values` (nilai nilai) dalam BuildDonationHistoryJson.
                        donations[day] = values;
                    // Menutup scope cabang if untuk kondisi `!donations.TryGetValue(day, out var values)`; bagian berikut berada di luar batas blok tersebut dalam
                    // BuildDonationHistoryJson.
                    }

                    // Memperbarui `values[property]` menggunakan memanggil `ReadClonedProperty` dengan `item`, `property` dalam BuildDonationHistoryJson.
                    values[property] = ReadClonedProperty(item, property);
                // Menutup scope loop setiap item dari `document.RootElement.EnumerateArray()`; bagian berikut berada di luar batas blok tersebut dalam
                // BuildDonationHistoryJson.
                }
            // Menutup scope penanganan operasi try; bagian berikut berada di luar batas blok tersebut dalam BuildDonationHistoryJson.
            }
            // Menangani exception `JsonException` melalui variabel dalam BuildDonationHistoryJson.
            catch (JsonException)
            // Membuka scope penanganan exception catch; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildDonationHistoryJson.
            {
                // Seri yang tidak tersedia tidak menghilangkan data dari seri lainnya.
            // Menutup scope penanganan exception catch; bagian berikut berada di luar batas blok tersebut dalam BuildDonationHistoryJson.
            }
        // Menutup scope blok ForEachVariableStatement; bagian berikut berada di luar batas blok tersebut dalam BuildDonationHistoryJson.
        }

        // Mengembalikan menserialisasi `donations.Values` menjadi JSON melalui `JsonSerializer.Serialize` kepada pemanggil dalam BuildDonationHistoryJson;
        // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return JsonSerializer.Serialize(donations.Values);
    // Menutup scope metode BuildDonationHistoryJson; bagian berikut berada di luar batas blok tersebut dalam BuildDonationHistoryJson.
    }

    /// <summary>
    /// Menggabungkan transaksi, perubahan bersih, dan saldo menjadi satu riwayat
    /// yang tersusun berdasarkan nomor urut kejadian.
    /// </summary>
    // Mendefinisikan metode `BuildTransactionHistoryJson` dengan hasil bertipe `string?`. Menggabungkan transaksi, perubahan bersih, dan saldo menjadi
    // satu riwayat yang tersusun berdasarkan nomor urut kejadian. Masukan: Parameter `outgoingRawValue` bertipe `string` membawa nilai outgoing raw
    // nilai; Parameter `incomingRawValue` bertipe `string` membawa nilai incoming raw nilai; Parameter `netChangeRawValue` bertipe `string` membawa
    // nilai net change raw nilai; Parameter `balanceRawValue` bertipe `string` membawa nilai saldo raw nilai.
    public static string? BuildTransactionHistoryJson(
        // Parameter `outgoingRawValue` bertipe `string` membawa nilai outgoing raw nilai.
        string outgoingRawValue,
        // Parameter `incomingRawValue` bertipe `string` membawa nilai incoming raw nilai.
        string incomingRawValue,
        // Parameter `netChangeRawValue` bertipe `string` membawa nilai net change raw nilai.
        string netChangeRawValue,
        // Parameter `balanceRawValue` bertipe `string` membawa nilai saldo raw nilai.
        string balanceRawValue)
    // Membuka scope metode BuildTransactionHistoryJson; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildTransactionHistoryJson.
    {
        // Menyiapkan variabel lokal `transactions` untuk nilai transactions dengan objek baru bertipe `Dictionary<long, Dictionary<string, object?>>`
        // dengan nilai awal sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var transactions = new Dictionary<long, Dictionary<string, object?>>();
        // Menjalankan memanggil `AppendTransactionRows` dengan `outgoingRawValue`, `”amount”`, `”coins_out_event”`, `transactions` dalam
        // BuildTransactionHistoryJson.
        AppendTransactionRows(outgoingRawValue, "amount", "coins_out_event", transactions);
        // Menjalankan memanggil `AppendTransactionRows` dengan `incomingRawValue`, `”amount”`, `”coins_in_event”`, `transactions` dalam
        // BuildTransactionHistoryJson.
        AppendTransactionRows(incomingRawValue, "amount", "coins_in_event", transactions);
        // Menjalankan memanggil `AppendTransactionRows` dengan `netChangeRawValue`, `”net”`, `”coin_change”`, `transactions` dalam
        // BuildTransactionHistoryJson.
        AppendTransactionRows(netChangeRawValue, "net", "coin_change", transactions);
        // Menjalankan memanggil `AppendTransactionRows` dengan `balanceRawValue`, `”coins”`, `”coin_balance_after_event”`, `transactions` dalam
        // BuildTransactionHistoryJson.
        AppendTransactionRows(balanceRawValue, "coins", "coin_balance_after_event", transactions);

        // Mengembalikan hasil pemilihan bersyarat: ketika `transactions.Count == 0` benar gunakan `null`, jika tidak gunakan
        // `JsonSerializer.Serialize(transactions .OrderBy(item => item.Key) .Select(item => item.Value))` kepada pemanggil dalam
        // BuildTransactionHistoryJson; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return transactions.Count == 0
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: null dalam BuildTransactionHistoryJson.
            ? null
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: JsonSerializer.Serialize(transactions dalam
            // BuildTransactionHistoryJson.
            : JsonSerializer.Serialize(transactions
                // Meneruskan fungsi lambda `item => item.Key` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen ke
                // `transactions .OrderBy`.
                .OrderBy(item => item.Key)
                // Meneruskan fungsi lambda `item => item.Value` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen ke
                // `transactions .OrderBy(item => item.Key) .Select`.
                .Select(item => item.Value));
    // Menutup scope metode BuildTransactionHistoryJson; bagian berikut berada di luar batas blok tersebut dalam BuildTransactionHistoryJson.
    }

    /// <summary>
    /// Menggabungkan urutan aksi dan ringkasan pengulangan per hari agar informasi
    /// yang sama tidak ditampilkan dalam dua tabel terpisah.
    /// </summary>
    // Mendefinisikan metode `BuildActionUsageHistoryJson` dengan hasil bertipe `string?`. Menggabungkan urutan aksi dan ringkasan pengulangan per hari
    // agar informasi yang sama tidak ditampilkan dalam dua tabel terpisah. Masukan: Parameter `actionSequenceRawValue` bertipe `string` membawa nilai
    // aksi sequence raw nilai; Parameter `actionRepetitionRawValue` bertipe `string` membawa nilai aksi repetition raw nilai.
    public static string? BuildActionUsageHistoryJson(
        // Parameter `actionSequenceRawValue` bertipe `string` membawa nilai aksi sequence raw nilai.
        string actionSequenceRawValue,
        // Parameter `actionRepetitionRawValue` bertipe `string` membawa nilai aksi repetition raw nilai.
        string actionRepetitionRawValue)
    // Membuka scope metode BuildActionUsageHistoryJson; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildActionUsageHistoryJson.
    {
        // Menyiapkan variabel lokal `days` untuk nilai hari dengan objek baru bertipe `SortedDictionary<int, Dictionary<string, object?>>` dengan nilai
        // awal sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var days = new SortedDictionary<int, Dictionary<string, object?>>();
        // Menjalankan memanggil `AppendActionUsageRows` dengan `actionSequenceRawValue`, `[”actions”]`, `days` dalam BuildActionUsageHistoryJson.
        AppendActionUsageRows(
            // Meneruskan `actionSequenceRawValue` (nilai aksi sequence raw nilai) sebagai argumen ke `AppendActionUsageRows`.
            actionSequenceRawValue,
            // Meneruskan koleksi berisi ”actions” sebagai argumen ke `AppendActionUsageRows`.
            ["actions"],
            // Meneruskan `days` (nilai hari) sebagai argumen ke `AppendActionUsageRows`.
            days);
        // Menjalankan memanggil `AppendActionUsageRows` dengan `actionRepetitionRawValue`, `[”total_actions”, ”distinct_actions”, ”repeated_actions”,
        // ”diversity_score”]`, `days` dalam BuildActionUsageHistoryJson.
        AppendActionUsageRows(
            // Meneruskan `actionRepetitionRawValue` (nilai aksi repetition raw nilai) sebagai argumen ke `AppendActionUsageRows`.
            actionRepetitionRawValue,
            // Meneruskan koleksi berisi ”total_actions”, ”distinct_actions”, ”repeated_actions”, ”diversity_score” sebagai argumen ke `AppendActionUsageRows`.
            ["total_actions", "distinct_actions", "repeated_actions", "diversity_score"],
            // Meneruskan `days` (nilai hari) sebagai argumen ke `AppendActionUsageRows`.
            days);

        // Mengembalikan hasil pemilihan bersyarat: ketika `days.Count == 0` benar gunakan `null`, jika tidak gunakan
        // `JsonSerializer.Serialize(days.Values)` kepada pemanggil dalam BuildActionUsageHistoryJson; eksekusi jalur ini selesai setelah nilai hasil
        // ditentukan.
        foreach (var day in days.Values)
        {
            day.Remove("repeated_actions", out var repeated);
            day.Remove("diversity_score");
            day["action_pattern"] = day["total_actions"] is JsonElement { ValueKind: JsonValueKind.Number } total && total.TryGetInt32(out var count)
                && repeated is JsonElement { ValueKind: JsonValueKind.Number } repetition && repetition.TryGetInt32(out var repeats)
                    ? count == 0 ? "none" : count == 1 ? "single" : repeats > 0 ? "repeated" : "different"
                    : null;
        }

        return days.Count == 0
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: null dalam BuildActionUsageHistoryJson.
            ? null
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: JsonSerializer.Serialize(days.Values); dalam
            // BuildActionUsageHistoryJson.
            : JsonSerializer.Serialize(days.Values);
    // Menutup scope metode BuildActionUsageHistoryJson; bagian berikut berada di luar batas blok tersebut dalam BuildActionUsageHistoryJson.
    }

    // Mendefinisikan metode `AppendActionUsageRows` dengan hasil bertipe `void`; operasi ini menangani append aksi usage baris. Masukan: Parameter
    // `rawValue` bertipe `string` membawa nilai raw nilai; Parameter `propertyNames` bertipe `IReadOnlyList<string>` membawa nilai property nama;
    // Parameter `output` bertipe `IDictionary<int, Dictionary<string, object?>>` membawa nilai output.
    private static void AppendActionUsageRows(
        // Parameter `rawValue` bertipe `string` membawa nilai raw nilai.
        string rawValue,
        // Parameter `propertyNames` bertipe `IReadOnlyList<string>` membawa nilai property nama.
        IReadOnlyList<string> propertyNames,
        // Parameter `output` bertipe `IDictionary<int, Dictionary<string, object?>>` membawa nilai output.
        IDictionary<int, Dictionary<string, object?>> output)
    // Membuka scope metode AppendActionUsageRows; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam AppendActionUsageRows.
    {
        // Memulai blok try dalam AppendActionUsageRows; exception dari blok ini dapat dialihkan ke catch, sedangkan finally (jika ada) tetap dijalankan
        // saat keluar.
        try
        // Membuka scope penanganan operasi try; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam AppendActionUsageRows.
        {
            // Menyiapkan variabel lokal `document` untuk nilai document dengan memanggil `JsonDocument.Parse` dengan `rawValue`. Tipe variabel disimpulkan dari
            // ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
            using var document = JsonDocument.Parse(rawValue);
            // Memeriksa perbandingan ketidaksamaan antara `document.RootElement.ValueKind` dan `JsonValueKind.Array`; blok if hanya dijalankan ketika kondisi
            // ini bernilai benar dalam AppendActionUsageRows.
            if (document.RootElement.ValueKind != JsonValueKind.Array)
            // Membuka scope cabang if untuk kondisi `document.RootElement.ValueKind != JsonValueKind.Array`; pernyataan/deklarasi berikut berada di dalam batas
            // blok ini dalam AppendActionUsageRows.
            {
                // Mengakhiri eksekusi lebih awal dalam AppendActionUsageRows tanpa mengembalikan nilai; pernyataan sesudah return pada jalur ini tidak dijalankan.
                return;
            // Menutup scope cabang if untuk kondisi `document.RootElement.ValueKind != JsonValueKind.Array`; bagian berikut berada di luar batas blok tersebut
            // dalam AppendActionUsageRows.
            }

            // Mengulangi setiap elemen `document.RootElement.EnumerateArray().Where(item => item.ValueKind == JsonValueKind.Object)`; elemen saat ini disimpan
            // sebagai `item` bertipe `var` untuk diproses oleh badan loop dalam AppendActionUsageRows.
            foreach (var item in document.RootElement.EnumerateArray().Where(item => item.ValueKind == JsonValueKind.Object))
            // Membuka scope loop setiap item dari `document.RootElement.EnumerateArray().Where(item => item.ValueKind == JsonValueKind.Object)`;
            // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam AppendActionUsageRows.
            {
                // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!item.TryGetProperty(”day_index”, out var dayValue) ||
                // !dayValue.TryGetInt32(out var dayIndex)` dan `dayIndex <= 0`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika
                // kondisi ini bernilai benar dalam AppendActionUsageRows.
                if (!item.TryGetProperty("day_index", out var dayValue)
                    // Melengkapi struktur ekspresi LogicalOrExpression melalui || !dayValue.TryGetInt32(out var dayIndex) dalam AppendActionUsageRows; token pada baris
                    // ini menyambungkan bagian kode sebelum dan sesudahnya.
                    || !dayValue.TryGetInt32(out var dayIndex)
                    // Melengkapi struktur ekspresi LogicalOrExpression melalui || dayIndex <= 0) dalam AppendActionUsageRows; token pada baris ini menyambungkan bagian
                    // kode sebelum dan sesudahnya.
                    || dayIndex <= 0)
                // Membuka scope cabang if untuk kondisi `!item.TryGetProperty(”day_index”, out var dayValue) || !dayValue.TryGetInt32(out var dayIndex) || dayIndex
                // <= 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam AppendActionUsageRows.
                {
                    // Melewati sisa pernyataan pada iterasi saat ini dan melanjutkan ke elemen/iterasi berikutnya dalam AppendActionUsageRows.
                    continue;
                // Menutup scope cabang if untuk kondisi `!item.TryGetProperty(”day_index”, out var dayValue) || !dayValue.TryGetInt32(out var dayIndex) || dayIndex
                // <= 0`; bagian berikut berada di luar batas blok tersebut dalam AppendActionUsageRows.
                }

                // Memeriksa kebalikan kondisi `output.TryGetValue(dayIndex, out var values)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
                // AppendActionUsageRows.
                if (!output.TryGetValue(dayIndex, out var values))
                // Membuka scope cabang if untuk kondisi `!output.TryGetValue(dayIndex, out var values)`; pernyataan/deklarasi berikut berada di dalam batas blok
                // ini dalam AppendActionUsageRows.
                {
                    // Memperbarui `values` menggunakan objek baru bertipe `Dictionary<string, object?>` dengan nilai awal sesuai konstruktornya dalam
                    // AppendActionUsageRows.
                    values = new Dictionary<string, object?>
                    // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                    // AppendActionUsageRows.
                    {
                        // Memperbarui `[”day_index”]` menggunakan `dayIndex` (nilai hari index) dalam AppendActionUsageRows.
                        ["day_index"] = dayIndex,
                        // Memperbarui `[”actions”]` menggunakan null, yaitu penanda tidak ada nilai dalam AppendActionUsageRows.
                        ["actions"] = null,
                        // Memperbarui `[”total_actions”]` menggunakan null, yaitu penanda tidak ada nilai dalam AppendActionUsageRows.
                        ["total_actions"] = null,
                        // Memperbarui `[”distinct_actions”]` menggunakan null, yaitu penanda tidak ada nilai dalam AppendActionUsageRows.
                        ["distinct_actions"] = null,
                        // Memperbarui `[”repeated_actions”]` menggunakan null, yaitu penanda tidak ada nilai dalam AppendActionUsageRows.
                        ["repeated_actions"] = null,
                        // Memperbarui `[”diversity_score”]` menggunakan null, yaitu penanda tidak ada nilai dalam AppendActionUsageRows.
                        ["diversity_score"] = null
                    // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam AppendActionUsageRows.
                    };
                    // Memperbarui `output[dayIndex]` menggunakan `values` (nilai nilai) dalam AppendActionUsageRows.
                    output[dayIndex] = values;
                // Menutup scope cabang if untuk kondisi `!output.TryGetValue(dayIndex, out var values)`; bagian berikut berada di luar batas blok tersebut dalam
                // AppendActionUsageRows.
                }

                // Mengulangi setiap elemen `propertyNames`; elemen saat ini disimpan sebagai `propertyName` bertipe `var` untuk diproses oleh badan loop dalam
                // AppendActionUsageRows.
                foreach (var propertyName in propertyNames)
                // Membuka scope loop setiap propertyName dari `propertyNames`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // AppendActionUsageRows.
                {
                    // Memperbarui `values[propertyName]` menggunakan memanggil `ReadClonedProperty` dengan `item`, `propertyName` dalam AppendActionUsageRows.
                    values[propertyName] = ReadClonedProperty(item, propertyName);
                // Menutup scope loop setiap propertyName dari `propertyNames`; bagian berikut berada di luar batas blok tersebut dalam AppendActionUsageRows.
                }
            // Menutup scope loop setiap item dari `document.RootElement.EnumerateArray().Where(item => item.ValueKind == JsonValueKind.Object)`; bagian berikut
            // berada di luar batas blok tersebut dalam AppendActionUsageRows.
            }
        // Menutup scope penanganan operasi try; bagian berikut berada di luar batas blok tersebut dalam AppendActionUsageRows.
        }
        // Menangani exception `JsonException` melalui variabel dalam AppendActionUsageRows.
        catch (JsonException)
        // Membuka scope penanganan exception catch; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam AppendActionUsageRows.
        {
            // Satu seri yang rusak tidak menghilangkan data valid dari seri lainnya.
        // Menutup scope penanganan exception catch; bagian berikut berada di luar batas blok tersebut dalam AppendActionUsageRows.
        }
    // Menutup scope metode AppendActionUsageRows; bagian berikut berada di luar batas blok tersebut dalam AppendActionUsageRows.
    }

    // Mendefinisikan metode `AppendTransactionRows` dengan hasil bertipe `void`; operasi ini menangani append transaction baris. Masukan: Parameter
    // `rawValue` bertipe `string` membawa nilai raw nilai; Parameter `sourceProperty` bertipe `string` membawa nilai source property; Parameter
    // `targetProperty` bertipe `string` membawa nilai target property; Parameter `output` bertipe `IDictionary<long, Dictionary<string, object?>>`
    // membawa nilai output.
    private static void AppendTransactionRows(
        // Parameter `rawValue` bertipe `string` membawa nilai raw nilai.
        string rawValue,
        // Parameter `sourceProperty` bertipe `string` membawa nilai source property.
        string sourceProperty,
        // Parameter `targetProperty` bertipe `string` membawa nilai target property.
        string targetProperty,
        // Parameter `output` bertipe `IDictionary<long, Dictionary<string, object?>>` membawa nilai output.
        IDictionary<long, Dictionary<string, object?>> output)
    // Membuka scope metode AppendTransactionRows; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam AppendTransactionRows.
    {
        // Memulai blok try dalam AppendTransactionRows; exception dari blok ini dapat dialihkan ke catch, sedangkan finally (jika ada) tetap dijalankan
        // saat keluar.
        try
        // Membuka scope penanganan operasi try; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam AppendTransactionRows.
        {
            // Menyiapkan variabel lokal `document` untuk nilai document dengan memanggil `JsonDocument.Parse` dengan `rawValue`. Tipe variabel disimpulkan dari
            // ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
            using var document = JsonDocument.Parse(rawValue);
            // Memeriksa perbandingan ketidaksamaan antara `document.RootElement.ValueKind` dan `JsonValueKind.Array`; blok if hanya dijalankan ketika kondisi
            // ini bernilai benar dalam AppendTransactionRows.
            if (document.RootElement.ValueKind != JsonValueKind.Array)
            // Membuka scope cabang if untuk kondisi `document.RootElement.ValueKind != JsonValueKind.Array`; pernyataan/deklarasi berikut berada di dalam batas
            // blok ini dalam AppendTransactionRows.
            {
                // Mengakhiri eksekusi lebih awal dalam AppendTransactionRows tanpa mengembalikan nilai; pernyataan sesudah return pada jalur ini tidak dijalankan.
                return;
            // Menutup scope cabang if untuk kondisi `document.RootElement.ValueKind != JsonValueKind.Array`; bagian berikut berada di luar batas blok tersebut
            // dalam AppendTransactionRows.
            }

            // Mengulangi setiap elemen `document.RootElement.EnumerateArray().Where(item => item.ValueKind == JsonValueKind.Object)`; elemen saat ini disimpan
            // sebagai `item` bertipe `var` untuk diproses oleh badan loop dalam AppendTransactionRows.
            foreach (var item in document.RootElement.EnumerateArray().Where(item => item.ValueKind == JsonValueKind.Object))
            // Membuka scope loop setiap item dari `document.RootElement.EnumerateArray().Where(item => item.ValueKind == JsonValueKind.Object)`;
            // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam AppendTransactionRows.
            {
                // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!item.TryGetProperty(”sequence_number”, out var sequenceValue)` dan
                // `!sequenceValue.TryGetInt64(out var sequenceNumber)`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi
                // ini bernilai benar dalam AppendTransactionRows.
                if (!item.TryGetProperty("sequence_number", out var sequenceValue)
                    // Melengkapi struktur ekspresi LogicalOrExpression melalui || !sequenceValue.TryGetInt64(out var sequenceNumber)) dalam AppendTransactionRows;
                    // token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
                    || !sequenceValue.TryGetInt64(out var sequenceNumber))
                // Membuka scope cabang if untuk kondisi `!item.TryGetProperty(”sequence_number”, out var sequenceValue) || !sequenceValue.TryGetInt64(out var
                // sequenceNumber)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam AppendTransactionRows.
                {
                    // Melewati sisa pernyataan pada iterasi saat ini dan melanjutkan ke elemen/iterasi berikutnya dalam AppendTransactionRows.
                    continue;
                // Menutup scope cabang if untuk kondisi `!item.TryGetProperty(”sequence_number”, out var sequenceValue) || !sequenceValue.TryGetInt64(out var
                // sequenceNumber)`; bagian berikut berada di luar batas blok tersebut dalam AppendTransactionRows.
                }

                // Memeriksa kebalikan kondisi `output.TryGetValue(sequenceNumber, out var values)`; blok if hanya dijalankan ketika kondisi ini bernilai benar
                // dalam AppendTransactionRows.
                if (!output.TryGetValue(sequenceNumber, out var values))
                // Membuka scope cabang if untuk kondisi `!output.TryGetValue(sequenceNumber, out var values)`; pernyataan/deklarasi berikut berada di dalam batas
                // blok ini dalam AppendTransactionRows.
                {
                    // Memperbarui `values` menggunakan objek baru bertipe `Dictionary<string, object?>` dengan nilai awal sesuai konstruktornya dalam
                    // AppendTransactionRows.
                    values = new Dictionary<string, object?>
                    // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                    // AppendTransactionRows.
                    {
                        // Memperbarui `[”day_index”]` menggunakan null, yaitu penanda tidak ada nilai dalam AppendTransactionRows.
                        ["day_index"] = null,
                        // Memperbarui `[”action_slot”]` menggunakan null, yaitu penanda tidak ada nilai dalam AppendTransactionRows.
                        ["action_slot"] = null,
                        // Memperbarui `[”sequence_number”]` menggunakan `sequenceNumber` (nomor urut event yang menentukan urutan pemrosesan riwayat permainan) dalam
                        // AppendTransactionRows.
                        ["sequence_number"] = sequenceNumber,
                        // Memperbarui `[”cashflow_category”]` menggunakan null, yaitu penanda tidak ada nilai dalam AppendTransactionRows.
                        ["cashflow_category"] = null,
                        // Memperbarui `[”coins_in_event”]` menggunakan nilai literal `0` dalam AppendTransactionRows.
                        ["coins_in_event"] = 0,
                        // Memperbarui `[”coins_out_event”]` menggunakan nilai literal `0` dalam AppendTransactionRows.
                        ["coins_out_event"] = 0,
                        // Memperbarui `[”coin_change”]` menggunakan null, yaitu penanda tidak ada nilai dalam AppendTransactionRows.
                        ["coin_change"] = null,
                        // Memperbarui `[”coin_balance_after_event”]` menggunakan null, yaitu penanda tidak ada nilai dalam AppendTransactionRows.
                        ["coin_balance_after_event"] = null
                    // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam AppendTransactionRows.
                    };
                    // Memperbarui `output[sequenceNumber]` menggunakan `values` (nilai nilai) dalam AppendTransactionRows.
                    output[sequenceNumber] = values;
                // Menutup scope cabang if untuk kondisi `!output.TryGetValue(sequenceNumber, out var values)`; bagian berikut berada di luar batas blok tersebut
                // dalam AppendTransactionRows.
                }

                // Menjalankan memanggil `SetContextValueIfMissing` dengan `values`, `item`, `”day_index”` dalam AppendTransactionRows.
                SetContextValueIfMissing(values, item, "day_index");
                // Menjalankan memanggil `SetContextValueIfMissing` dengan `values`, `item`, `”action_slot”` dalam AppendTransactionRows.
                SetContextValueIfMissing(values, item, "action_slot");
                // Menjalankan memanggil `SetContextValueIfMissing` dengan `values`, `item`, `”cashflow_category”` dalam AppendTransactionRows.
                SetContextValueIfMissing(values, item, "cashflow_category");
                // Memperbarui `values[targetProperty]` menggunakan memanggil `ReadClonedProperty` dengan `item`, `sourceProperty` dalam AppendTransactionRows.
                values[targetProperty] = ReadClonedProperty(item, sourceProperty);
            // Menutup scope loop setiap item dari `document.RootElement.EnumerateArray().Where(item => item.ValueKind == JsonValueKind.Object)`; bagian berikut
            // berada di luar batas blok tersebut dalam AppendTransactionRows.
            }
        // Menutup scope penanganan operasi try; bagian berikut berada di luar batas blok tersebut dalam AppendTransactionRows.
        }
        // Menangani exception `JsonException` melalui variabel dalam AppendTransactionRows.
        catch (JsonException)
        // Membuka scope penanganan exception catch; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam AppendTransactionRows.
        {
            // Satu seri yang rusak tidak menghalangi seri valid lainnya untuk ditampilkan.
        // Menutup scope penanganan exception catch; bagian berikut berada di luar batas blok tersebut dalam AppendTransactionRows.
        }
    // Menutup scope metode AppendTransactionRows; bagian berikut berada di luar batas blok tersebut dalam AppendTransactionRows.
    }

    // Mendefinisikan metode `SetContextValueIfMissing` dengan hasil bertipe `void`; operasi ini menangani set context nilai if missing. Masukan:
    // Parameter `values` bertipe `IDictionary<string, object?>` membawa nilai nilai; Parameter `item` bertipe `JsonElement` membawa nilai elemen;
    // Parameter `propertyName` bertipe `string` membawa nilai property nama.
    private static void SetContextValueIfMissing(
        // Parameter `values` bertipe `IDictionary<string, object?>` membawa nilai nilai.
        IDictionary<string, object?> values,
        // Parameter `item` bertipe `JsonElement` membawa nilai elemen.
        JsonElement item,
        // Parameter `propertyName` bertipe `string` membawa nilai property nama.
        string propertyName)
    // Membuka scope metode SetContextValueIfMissing; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam SetContextValueIfMissing.
    {
        // Memeriksa hasil pencocokan `values[propertyName]` dengan pola `null`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // SetContextValueIfMissing.
        if (values[propertyName] is null)
        // Membuka scope cabang if untuk kondisi `values[propertyName] is null`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // SetContextValueIfMissing.
        {
            // Memperbarui `values[propertyName]` menggunakan memanggil `ReadClonedProperty` dengan `item`, `propertyName` dalam SetContextValueIfMissing.
            values[propertyName] = ReadClonedProperty(item, propertyName);
        // Menutup scope cabang if untuk kondisi `values[propertyName] is null`; bagian berikut berada di luar batas blok tersebut dalam
        // SetContextValueIfMissing.
        }
    // Menutup scope metode SetContextValueIfMissing; bagian berikut berada di luar batas blok tersebut dalam SetContextValueIfMissing.
    }

    // Mendefinisikan metode `ReadClonedProperty` dengan hasil bertipe `object?`; operasi ini menangani read cloned property. Masukan: Parameter `item`
    // bertipe `JsonElement` membawa nilai elemen; Parameter `propertyName` bertipe `string` membawa nilai property nama. Nilai hasil langsung berasal
    // dari hasil pemilihan bersyarat: ketika `item.TryGetProperty(propertyName, out var value)` benar gunakan `value.Clone()`, jika tidak gunakan
    // `null`.
    private static object? ReadClonedProperty(JsonElement item, string propertyName) =>
        // Melanjutkan pengolahan dengan mencari properti JSON `propertyName`, `var value` pada `item` tanpa menganggap propertinya selalu tersedia dalam
        // ReadClonedProperty.
        item.TryGetProperty(propertyName, out var value) ? value.Clone() : null;

    // Mendefinisikan metode `FormatCollectionValue` dengan hasil bertipe `string`; operasi ini menangani format collection nilai. Masukan: Parameter
    // `element` bertipe `JsonElement` membawa nilai element; Parameter `trueText` bertipe `string` membawa nilai true text; Parameter `falseText`
    // bertipe `string` membawa nilai false text; Parameter `nullText` bertipe `string` membawa nilai null text.
    private static string FormatCollectionValue(
        // Parameter `element` bertipe `JsonElement` membawa nilai element.
        JsonElement element,
        // Parameter `trueText` bertipe `string` membawa nilai true text.
        string trueText,
        // Parameter `falseText` bertipe `string` membawa nilai false text.
        string falseText,
        // Parameter `nullText` bertipe `string` membawa nilai null text.
        string nullText)
    // Membuka scope metode FormatCollectionValue; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam FormatCollectionValue.
    {
        // Mengembalikan hasil pemetaan `element.ValueKind` melalui cabang pola switch yang cocok kepada pemanggil dalam FormatCollectionValue; eksekusi
        // jalur ini selesai setelah nilai hasil ditentukan.
        return element.ValueKind switch
        // Membuka scope pemetaan switch atas `element.ValueKind`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam FormatCollectionValue.
        {
            // Untuk pola `JsonValueKind.Array`, menghasilkan memanggil `string.Join` dengan `”, ”`, `element.EnumerateArray() .Select(item =>
            // FormatCollectionValue(item, trueText, falseText, nullText))` sebagai hasil switch.
            JsonValueKind.Array => string.Join(", ", element.EnumerateArray()
                // Meneruskan fungsi lambda `item => FormatCollectionValue(item, trueText, falseText, nullText)` yang dijalankan oleh operasi pemanggil untuk
                // memproses setiap masukan sebagai argumen ke `element.EnumerateArray() .Select`; Meneruskan `item` (nilai elemen) sebagai argumen ke
                // `FormatCollectionValue`; Meneruskan `trueText` (nilai true text) sebagai argumen ke `FormatCollectionValue`; Meneruskan `falseText` (nilai false
                // text) sebagai argumen ke `FormatCollectionValue`; Meneruskan `nullText` (nilai null text) sebagai argumen ke `FormatCollectionValue`.
                .Select(item => FormatCollectionValue(item, trueText, falseText, nullText))),
            // Untuk pola `JsonValueKind.Object`, menghasilkan memanggil `string.Join` dengan `” · ”`, `element.EnumerateObject() .Select(property =>
            // $”{property.Name}: {FormatCollectionValue(property.Value, trueText, falseText, nullText)}”)` sebagai hasil switch.
            JsonValueKind.Object => string.Join(" · ", element.EnumerateObject()
                // Meneruskan fungsi lambda `property => $”{property.Name}: {FormatCollectionValue(property.Value, trueText, falseText, nullText)}”` yang dijalankan
                // oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen ke `element.EnumerateObject() .Select`; Meneruskan `property.Value`, yaitu
                // nilai yang dibungkus objek/nullable sebagai argumen ke `FormatCollectionValue`; Meneruskan `trueText` (nilai true text) sebagai argumen ke
                // `FormatCollectionValue`; Meneruskan `falseText` (nilai false text) sebagai argumen ke `FormatCollectionValue`; Meneruskan `nullText` (nilai null
                // text) sebagai argumen ke `FormatCollectionValue`.
                .Select(property => $"{property.Name}: {FormatCollectionValue(property.Value, trueText, falseText, nullText)}")),
            // Untuk pola `_`, menghasilkan memanggil `FormatJsonLeafValue` dengan `element`, `trueText`, `falseText`, `nullText` sebagai hasil switch.
            _ => FormatJsonLeafValue(element, trueText, falseText, nullText)
        // Menutup scope pemetaan switch atas `element.ValueKind`; bagian berikut berada di luar batas blok tersebut dalam FormatCollectionValue.
        };
    // Menutup scope metode FormatCollectionValue; bagian berikut berada di luar batas blok tersebut dalam FormatCollectionValue.
    }

    // Mendefinisikan metode `Visit` dengan hasil bertipe `void`; operasi ini menangani visit. Masukan: Parameter `element` bertipe `JsonElement`
    // membawa nilai element; Parameter `path` bertipe `string` membawa nilai path; Parameter `output` bertipe `List<(string Path, string Value)>`
    // membawa nilai output; Parameter `trueText` bertipe `string` membawa nilai true text; Parameter `falseText` bertipe `string` membawa nilai false
    // text; Parameter `nullText` bertipe `string` membawa nilai null text.
    private static void Visit(
        // Parameter `element` bertipe `JsonElement` membawa nilai element.
        JsonElement element,
        // Parameter `path` bertipe `string` membawa nilai path.
        string path,
        // Parameter `output` bertipe `List<(string Path, string Value)>` membawa nilai output.
        List<(string Path, string Value)> output,
        // Parameter `trueText` bertipe `string` membawa nilai true text.
        string trueText,
        // Parameter `falseText` bertipe `string` membawa nilai false text.
        string falseText,
        // Parameter `nullText` bertipe `string` membawa nilai null text.
        string nullText)
    // Membuka scope metode Visit; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Visit.
    {
        // Memilih cabang berdasarkan `element.ValueKind` (nilai nilai kind); label case menentukan perlakuan untuk setiap nilai yang dikenali dalam Visit.
        switch (element.ValueKind)
        // Membuka scope pemilihan switch atas `element.ValueKind`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Visit.
        {
            // Menetapkan label cabang `case JsonValueKind.Object:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            case JsonValueKind.Object:
                // Membuka scope blok SwitchSection; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Visit.
                {
                    // Menyiapkan variabel lokal `hasProperty` untuk nilai memiliki property dengan false, yaitu kondisi nonaktif/tidak terpenuhi. Tipe variabel
                    // disimpulkan dari ekspresi nilai awal.
                    var hasProperty = false;
                    // Mengulangi setiap elemen `element.EnumerateObject()`; elemen saat ini disimpan sebagai `property` bertipe `var` untuk diproses oleh badan loop
                    // dalam Visit.
                    foreach (var property in element.EnumerateObject())
                    // Membuka scope loop setiap property dari `element.EnumerateObject()`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Visit.
                    {
                        // Memperbarui `hasProperty` menggunakan true, yaitu kondisi aktif/terpenuhi dalam Visit.
                        hasProperty = true;
                        // Menyiapkan variabel lokal `nextPath` untuk nilai next path dengan hasil pemilihan bersyarat: ketika `string.IsNullOrWhiteSpace(path)` benar
                        // gunakan `property.Name`, jika tidak gunakan `$”{path}.{property.Name}”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
                        var nextPath = string.IsNullOrWhiteSpace(path) ? property.Name : $"{path}.{property.Name}";
                        // Menjalankan memanggil `Visit` dengan `property.Value`, `nextPath`, `output`, `trueText`, `falseText`, `nullText` dalam Visit.
                        Visit(property.Value, nextPath, output, trueText, falseText, nullText);
                    // Menutup scope loop setiap property dari `element.EnumerateObject()`; bagian berikut berada di luar batas blok tersebut dalam Visit.
                    }

                    // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `!hasProperty` dan `!string.IsNullOrWhiteSpace(path)`; sisi kanan diperiksa hanya
                    // jika sisi kiri benar; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam Visit.
                    if (!hasProperty && !string.IsNullOrWhiteSpace(path))
                    // Membuka scope cabang if untuk kondisi `!hasProperty && !string.IsNullOrWhiteSpace(path)`; pernyataan/deklarasi berikut berada di dalam batas blok
                    // ini dalam Visit.
                    {
                        // Menjalankan menambahkan `(path, ”{}”)` ke `output` dalam Visit.
                        output.Add((path, "{}"));
                    // Menutup scope cabang if untuk kondisi `!hasProperty && !string.IsNullOrWhiteSpace(path)`; bagian berikut berada di luar batas blok tersebut dalam
                    // Visit.
                    }

                    // Mengakhiri loop atau cabang switch terdekat, kemudian melanjutkan setelah blok tersebut dalam Visit.
                    break;
                // Menutup scope blok SwitchSection; bagian berikut berada di luar batas blok tersebut dalam Visit.
                }
            // Menetapkan label cabang `case JsonValueKind.Array:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            case JsonValueKind.Array:
                // Membuka scope blok SwitchSection; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Visit.
                {
                    // Menyiapkan variabel lokal `index` untuk nilai index dengan nilai literal `0`. Tipe variabel disimpulkan dari ekspresi nilai awal.
                    var index = 0;
                    // Mengulangi setiap elemen `element.EnumerateArray()`; elemen saat ini disimpan sebagai `item` bertipe `var` untuk diproses oleh badan loop dalam
                    // Visit.
                    foreach (var item in element.EnumerateArray())
                    // Membuka scope loop setiap item dari `element.EnumerateArray()`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Visit.
                    {
                        // Menjalankan memanggil `Visit` dengan `item`, `$”{path}[{index}]”`, `output`, `trueText`, `falseText`, `nullText` dalam Visit.
                        Visit(item, $"{path}[{index}]", output, trueText, falseText, nullText);
                        // Memperbarui `index` dengan menambahkan nilai literal `1` dalam Visit.
                        index += 1;
                    // Menutup scope loop setiap item dari `element.EnumerateArray()`; bagian berikut berada di luar batas blok tersebut dalam Visit.
                    }

                    // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `index == 0` dan `!string.IsNullOrWhiteSpace(path)`; sisi kanan diperiksa hanya
                    // jika sisi kiri benar; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam Visit.
                    if (index == 0 && !string.IsNullOrWhiteSpace(path))
                    // Membuka scope cabang if untuk kondisi `index == 0 && !string.IsNullOrWhiteSpace(path)`; pernyataan/deklarasi berikut berada di dalam batas blok
                    // ini dalam Visit.
                    {
                        // Menjalankan menambahkan `(path, ”[]”)` ke `output` dalam Visit.
                        output.Add((path, "[]"));
                    // Menutup scope cabang if untuk kondisi `index == 0 && !string.IsNullOrWhiteSpace(path)`; bagian berikut berada di luar batas blok tersebut dalam
                    // Visit.
                    }

                    // Mengakhiri loop atau cabang switch terdekat, kemudian melanjutkan setelah blok tersebut dalam Visit.
                    break;
                // Menutup scope blok SwitchSection; bagian berikut berada di luar batas blok tersebut dalam Visit.
                }
            // Menetapkan label cabang `case JsonValueKind.String:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            case JsonValueKind.String:
            // Menetapkan label cabang `case JsonValueKind.True:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            case JsonValueKind.True:
            // Menetapkan label cabang `case JsonValueKind.False:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            case JsonValueKind.False:
            // Menetapkan label cabang `case JsonValueKind.Number:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            case JsonValueKind.Number:
            // Menetapkan label cabang `case JsonValueKind.Null:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            case JsonValueKind.Null:
                // Menjalankan menambahkan `( string.IsNullOrWhiteSpace(path) ? ”value” : path, FormatJsonLeafValue(element, trueText, falseText, nullText))` ke
                // `output` dalam Visit.
                output.Add((
                    // Meneruskan hasil pemilihan bersyarat: ketika `string.IsNullOrWhiteSpace(path)` benar gunakan `”value”`, jika tidak gunakan `path` sebagai argumen
                    // ke `output.Add`; Meneruskan `path` (nilai path) sebagai argumen ke `string.IsNullOrWhiteSpace`.
                    string.IsNullOrWhiteSpace(path) ? "value" : path,
                    // Meneruskan memanggil `FormatJsonLeafValue` dengan `element`, `trueText`, `falseText`, `nullText` sebagai argumen ke `output.Add`; Meneruskan
                    // `element` (nilai element) sebagai argumen ke `FormatJsonLeafValue`; Meneruskan `trueText` (nilai true text) sebagai argumen ke
                    // `FormatJsonLeafValue`; Meneruskan `falseText` (nilai false text) sebagai argumen ke `FormatJsonLeafValue`; Meneruskan `nullText` (nilai null
                    // text) sebagai argumen ke `FormatJsonLeafValue`.
                    FormatJsonLeafValue(element, trueText, falseText, nullText)));
                // Mengakhiri loop atau cabang switch terdekat, kemudian melanjutkan setelah blok tersebut dalam Visit.
                break;
            // Menetapkan label cabang `default:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            default:
                // Menjalankan menambahkan `( string.IsNullOrWhiteSpace(path) ? ”value” : path, element.ToString())` ke `output` dalam Visit.
                output.Add((
                    // Meneruskan hasil pemilihan bersyarat: ketika `string.IsNullOrWhiteSpace(path)` benar gunakan `”value”`, jika tidak gunakan `path` sebagai argumen
                    // ke `output.Add`; Meneruskan `path` (nilai path) sebagai argumen ke `string.IsNullOrWhiteSpace`.
                    string.IsNullOrWhiteSpace(path) ? "value" : path,
                    // Meneruskan mengubah `element` menjadi teks sebagai argumen ke `output.Add`.
                    element.ToString()));
                // Mengakhiri loop atau cabang switch terdekat, kemudian melanjutkan setelah blok tersebut dalam Visit.
                break;
        // Menutup scope pemilihan switch atas `element.ValueKind`; bagian berikut berada di luar batas blok tersebut dalam Visit.
        }
    // Menutup scope metode Visit; bagian berikut berada di luar batas blok tersebut dalam Visit.
    }

    // Mendefinisikan metode `VisitVariables` dengan hasil bertipe `void`; operasi ini menangani visit variables. Masukan: Parameter `element` bertipe
    // `JsonElement` membawa nilai element; Parameter `path` bertipe `string` membawa nilai path; Parameter `output` bertipe `List<(string Path, string
    // Value)>` membawa nilai output; Parameter `trueText` bertipe `string` membawa nilai true text; Parameter `falseText` bertipe `string` membawa
    // nilai false text; Parameter `nullText` bertipe `string` membawa nilai null text.
    private static void VisitVariables(
        // Parameter `element` bertipe `JsonElement` membawa nilai element.
        JsonElement element,
        // Parameter `path` bertipe `string` membawa nilai path.
        string path,
        // Parameter `output` bertipe `List<(string Path, string Value)>` membawa nilai output.
        List<(string Path, string Value)> output,
        // Parameter `trueText` bertipe `string` membawa nilai true text.
        string trueText,
        // Parameter `falseText` bertipe `string` membawa nilai false text.
        string falseText,
        // Parameter `nullText` bertipe `string` membawa nilai null text.
        string nullText)
    // Membuka scope metode VisitVariables; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam VisitVariables.
    {
        // Memeriksa perbandingan ketidaksamaan antara `element.ValueKind` dan `JsonValueKind.Object`; blok if hanya dijalankan ketika kondisi ini bernilai
        // benar dalam VisitVariables.
        if (element.ValueKind != JsonValueKind.Object)
        // Membuka scope cabang if untuk kondisi `element.ValueKind != JsonValueKind.Object`; pernyataan/deklarasi berikut berada di dalam batas blok ini
        // dalam VisitVariables.
        {
            // Menjalankan menambahkan `( string.IsNullOrWhiteSpace(path) ? ”value” : path, FormatJsonLeafValue(element, trueText, falseText, nullText))` ke
            // `output` dalam VisitVariables.
            output.Add((
                // Meneruskan hasil pemilihan bersyarat: ketika `string.IsNullOrWhiteSpace(path)` benar gunakan `”value”`, jika tidak gunakan `path` sebagai argumen
                // ke `output.Add`; Meneruskan `path` (nilai path) sebagai argumen ke `string.IsNullOrWhiteSpace`.
                string.IsNullOrWhiteSpace(path) ? "value" : path,
                // Meneruskan memanggil `FormatJsonLeafValue` dengan `element`, `trueText`, `falseText`, `nullText` sebagai argumen ke `output.Add`; Meneruskan
                // `element` (nilai element) sebagai argumen ke `FormatJsonLeafValue`; Meneruskan `trueText` (nilai true text) sebagai argumen ke
                // `FormatJsonLeafValue`; Meneruskan `falseText` (nilai false text) sebagai argumen ke `FormatJsonLeafValue`; Meneruskan `nullText` (nilai null
                // text) sebagai argumen ke `FormatJsonLeafValue`.
                FormatJsonLeafValue(element, trueText, falseText, nullText)));
            // Mengakhiri eksekusi lebih awal dalam VisitVariables tanpa mengembalikan nilai; pernyataan sesudah return pada jalur ini tidak dijalankan.
            return;
        // Menutup scope cabang if untuk kondisi `element.ValueKind != JsonValueKind.Object`; bagian berikut berada di luar batas blok tersebut dalam
        // VisitVariables.
        }

        // Mengulangi setiap elemen `element.EnumerateObject()`; elemen saat ini disimpan sebagai `property` bertipe `var` untuk diproses oleh badan loop
        // dalam VisitVariables.
        foreach (var property in element.EnumerateObject())
        // Membuka scope loop setiap property dari `element.EnumerateObject()`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // VisitVariables.
        {
            // Menyiapkan variabel lokal `nextPath` untuk nilai next path dengan hasil pemilihan bersyarat: ketika `string.IsNullOrWhiteSpace(path)` benar
            // gunakan `property.Name`, jika tidak gunakan `$”{path}.{property.Name}”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var nextPath = string.IsNullOrWhiteSpace(path) ? property.Name : $"{path}.{property.Name}";
            // Memeriksa perbandingan kesamaan antara `property.Value.ValueKind` dan `JsonValueKind.Object`; blok if hanya dijalankan ketika kondisi ini
            // bernilai benar dalam VisitVariables.
            if (property.Value.ValueKind == JsonValueKind.Object)
            // Membuka scope cabang if untuk kondisi `property.Value.ValueKind == JsonValueKind.Object`; pernyataan/deklarasi berikut berada di dalam batas blok
            // ini dalam VisitVariables.
            {
                // Menjalankan memanggil `VisitVariables` dengan `property.Value`, `nextPath`, `output`, `trueText`, `falseText`, `nullText` dalam VisitVariables.
                VisitVariables(property.Value, nextPath, output, trueText, falseText, nullText);
                // Melewati sisa pernyataan pada iterasi saat ini dan melanjutkan ke elemen/iterasi berikutnya dalam VisitVariables.
                continue;
            // Menutup scope cabang if untuk kondisi `property.Value.ValueKind == JsonValueKind.Object`; bagian berikut berada di luar batas blok tersebut dalam
            // VisitVariables.
            }

            // Menjalankan menambahkan `(nextPath, FormatJsonLeafValue(property.Value, trueText, falseText, nullText))` ke `output` dalam VisitVariables.
            output.Add((nextPath, FormatJsonLeafValue(property.Value, trueText, falseText, nullText)));
        // Menutup scope loop setiap property dari `element.EnumerateObject()`; bagian berikut berada di luar batas blok tersebut dalam VisitVariables.
        }
    // Menutup scope metode VisitVariables; bagian berikut berada di luar batas blok tersebut dalam VisitVariables.
    }
// Menutup scope tipe PlayerMetricJsonMapper; bagian berikut berada di luar batas blok tersebut.
}
