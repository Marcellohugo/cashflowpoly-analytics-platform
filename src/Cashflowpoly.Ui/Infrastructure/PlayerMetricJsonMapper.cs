// Fungsi file: Menyediakan transformasi, lokalisasi, atau koneksi UI melalui PlayerMetricJsonMapper.
// Mengimpor namespace `System.Text.Json` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.Json;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Infrastructure` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Infrastructure;

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
{
    /// <summary>
    /// Memformat nilai leaf JsonElement menjadi teks tampilan.
    /// </summary>
    // Mendefinisikan metode `FormatJsonLeafValue` dengan hasil bertipe `string`. Memformat nilai leaf JsonElement menjadi teks tampilan. Masukan:
    // Parameter `element` bertipe `JsonElement` membawa nilai element; Parameter `trueText` bertipe `string` membawa nilai true text; Parameter
    // `falseText` bertipe `string` membawa nilai false text; Parameter `nullText` bertipe `string` membawa nilai null text.
    public static string FormatJsonLeafValue(JsonElement element, string trueText, string falseText, string nullText)
    {
        return element.ValueKind switch
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
        };
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
    {
        var rows = new List<(string Path, string Value)>();
        if (!root.HasValue)
        {
            return rows;
        }

        Visit(root.Value, string.Empty, rows, trueText, falseText, nullText);
        return rows;
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
    {
        var groups = new List<(string GroupKey, List<(string Path, string Value)> Rows)>();
        if (!root.HasValue || root.Value.ValueKind != JsonValueKind.Object)
        {
            return groups;
        }

        var scalarRows = new List<(string Path, string Value)>();
        // Mengulangi setiap elemen `root.Value.EnumerateObject()`; elemen saat ini disimpan sebagai `property` bertipe `var` untuk diproses oleh badan loop
        // dalam BuildMetricGroups.
        foreach (var property in root.Value.EnumerateObject())
        {
            if (property.Value.ValueKind == JsonValueKind.Object || property.Value.ValueKind == JsonValueKind.Array)
            {
                var rows = FlattenJsonLeaves(property.Value, trueText, falseText, nullText);
                if (rows.Count == 0)
                {
                    rows.Add(("value", FormatJsonLeafValue(property.Value, trueText, falseText, nullText)));
                }

                groups.Add((property.Name, rows));
                // Melewati sisa pernyataan pada iterasi saat ini dan melanjutkan ke elemen/iterasi berikutnya dalam BuildMetricGroups.
                continue;
            }

            scalarRows.Add((property.Name, FormatJsonLeafValue(property.Value, trueText, falseText, nullText)));
        }

        if (scalarRows.Count > 0)
        {
            groups.Insert(0, ("summary", scalarRows));
        }

        return groups;
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
    {
        var groups = new List<(string GroupKey, List<(string Path, string Value)> Rows)>();
        if (!root.HasValue || root.Value.ValueKind != JsonValueKind.Object)
        {
            return groups;
        }

        // Mengulangi setiap elemen `root.Value.EnumerateObject()`; elemen saat ini disimpan sebagai `group` bertipe `var` untuk diproses oleh badan loop
        // dalam BuildMetricVariableGroups.
        foreach (var group in root.Value.EnumerateObject())
        {
            if (group.Value.ValueKind != JsonValueKind.Object)
            {
                groups.Add((group.Name, new List<(string Path, string Value)>
                {
                    (group.Name, FormatJsonLeafValue(group.Value, trueText, falseText, nullText))
                }));
                // Melewati sisa pernyataan pada iterasi saat ini dan melanjutkan ke elemen/iterasi berikutnya dalam BuildMetricVariableGroups.
                continue;
            }

            var rows = new List<(string Path, string Value)>();
            VisitVariables(group.Value, string.Empty, rows, trueText, falseText, nullText);
            groups.Add((group.Name, rows));
        }

        return groups;
    }

    /// <summary>
    /// Membangun map properti top-level JSON ke elemen clone agar aman dipakai setelah JsonDocument asal dispose.
    /// </summary>
    // Mendefinisikan metode `BuildMetricGroupElements` dengan hasil bertipe `Dictionary<string, JsonElement>`. Membangun map properti top-level JSON ke
    // elemen clone agar aman dipakai setelah JsonDocument asal dispose. Masukan: Parameter `root` bertipe `JsonElement?` membawa nilai root; nilai null
    // diizinkan ketika data opsional belum tersedia.
    public static Dictionary<string, JsonElement> BuildMetricGroupElements(JsonElement? root)
    {
        var groups = new Dictionary<string, JsonElement>(StringComparer.OrdinalIgnoreCase);
        if (!root.HasValue || root.Value.ValueKind != JsonValueKind.Object)
        {
            return groups;
        }

        // Mengulangi setiap elemen `root.Value.EnumerateObject()`; elemen saat ini disimpan sebagai `property` bertipe `var` untuk diproses oleh badan loop
        // dalam BuildMetricGroupElements.
        foreach (var property in root.Value.EnumerateObject())
        {
            groups[property.Name] = property.Value.Clone();
        }

        return groups;
    }

    /// <summary>
    /// Membaca nilai numerik dari JsonElement number atau string numerik.
    /// </summary>
    // Mendefinisikan metode `TryGetNumericValue` dengan hasil bertipe `bool`. Membaca nilai numerik dari JsonElement number atau string numerik.
    // Masukan: Parameter `element` bertipe `JsonElement` membawa nilai element; Parameter `value` bertipe `double` membawa nilai nilai; out
    // mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    public static bool TryGetNumericValue(JsonElement element, out double value)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Number:
                return element.TryGetDouble(out value);
            case JsonValueKind.String:
                if (double.TryParse(element.GetString(), out var parsed))
                {
                    value = parsed;
                    return true;
                }

                break;
        }

        value = 0;
        return false;
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
    {
        try
        {
            using var document = JsonDocument.Parse(rawValue);
            var root = document.RootElement;

            if (root.ValueKind == JsonValueKind.Object)
            {
                var rows = root.EnumerateObject()
                    .Select(property => (IReadOnlyList<string>)new[]
                    {
                        property.Name,
                        FormatCollectionValue(property.Value, trueText, falseText, nullText)
                    })
                    .ToList();
                return rows.Count == 0
                    // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: null dalam BuildCollectionTable.
                    ? null
                    // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: new PlayerMetricCollectionTable(new[] { ”series”, ”value” }, rows);
                    // dalam BuildCollectionTable.
                    : new PlayerMetricCollectionTable(new[] { "series", "value" }, rows);
            }

            if (root.ValueKind != JsonValueKind.Array)
            {
                return null;
            }

            var items = root.EnumerateArray().ToList();
            if (items.Count == 0)
            {
                return null;
            }

            if (items.All(item => item.ValueKind == JsonValueKind.Object))
            {
                var columns = items
                    .SelectMany(item => item.EnumerateObject().Select(property => property.Name))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();
                var rows = items
                    .Select(item => (IReadOnlyList<string>)columns
                        .Select(column => item.TryGetProperty(column, out var value)
                            ? FormatCollectionValue(value, trueText, falseText, nullText)
                            : nullText)
                        .ToList())
                    .ToList();
                return new PlayerMetricCollectionTable(columns, rows);
            }

            return new PlayerMetricCollectionTable(
                new[] { "item_index", "value" },
                items.Select((item, index) => (IReadOnlyList<string>)new[]
                {
                    (index + 1).ToString(System.Globalization.CultureInfo.InvariantCulture),
                    FormatCollectionValue(item, trueText, falseText, nullText)
                }).ToList());
        }
        // Menangani exception `JsonException` melalui variabel dalam BuildCollectionTable.
        catch (JsonException)
        {
            return null;
        }
    }

    /// <summary>
    /// Memasangkan jumlah dan peringkat donasi berdasarkan hari, bukan posisi array.
    /// </summary>
    // Mendefinisikan metode `BuildDonationHistoryJson` dengan hasil bertipe `string`. Memasangkan jumlah dan peringkat donasi berdasarkan hari, bukan
    // posisi array. Masukan: Parameter `amountsRawValue` bertipe `string` membawa nilai amounts raw nilai; Parameter `ranksRawValue` bertipe `string`
    // membawa nilai ranks raw nilai.
    public static string BuildDonationHistoryJson(string amountsRawValue, string ranksRawValue)
    {
        var donations = new SortedDictionary<int, Dictionary<string, object?>>();
        foreach (var (rawValue, property) in new[] { (amountsRawValue, "amount"), (ranksRawValue, "rank") })
        {
            try
            {
                using var document = JsonDocument.Parse(rawValue);
                if (document.RootElement.ValueKind != JsonValueKind.Array)
                {
                    // Melewati sisa pernyataan pada iterasi saat ini dan melanjutkan ke elemen/iterasi berikutnya dalam BuildDonationHistoryJson.
                    continue;
                }

                // Mengulangi setiap elemen `document.RootElement.EnumerateArray()`; elemen saat ini disimpan sebagai `item` bertipe `var` untuk diproses oleh badan
                // loop dalam BuildDonationHistoryJson.
                foreach (var item in document.RootElement.EnumerateArray())
                {
                    if (item.ValueKind != JsonValueKind.Object
                        || !item.TryGetProperty("day_index", out var dayValue)
                        || dayValue.ValueKind != JsonValueKind.Number
                        || !dayValue.TryGetInt32(out var day)
                        || day <= 0)
                    {
                        // Melewati sisa pernyataan pada iterasi saat ini dan melanjutkan ke elemen/iterasi berikutnya dalam BuildDonationHistoryJson.
                        continue;
                    }

                    if (!donations.TryGetValue(day, out var values))
                    {
                        values = new Dictionary<string, object?>
                        {
                            ["day_index"] = day,
                            ["amount"] = null,
                            ["rank"] = null
                        };
                        donations[day] = values;
                    }

                    values[property] = ReadClonedProperty(item, property);
                }
            }
            // Menangani exception `JsonException` melalui variabel dalam BuildDonationHistoryJson.
            catch (JsonException)
            {
                // Seri yang tidak tersedia tidak menghilangkan data dari seri lainnya.
            // Menutup scope penanganan exception catch; bagian berikut berada di luar batas blok tersebut dalam BuildDonationHistoryJson.
            }
        }

        return JsonSerializer.Serialize(donations.Values);
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
    {
        var transactions = new Dictionary<long, Dictionary<string, object?>>();
        AppendTransactionRows(outgoingRawValue, "amount", "coins_out_event", transactions);
        AppendTransactionRows(incomingRawValue, "amount", "coins_in_event", transactions);
        AppendTransactionRows(netChangeRawValue, "net", "coin_change", transactions);
        AppendTransactionRows(balanceRawValue, "coins", "coin_balance_after_event", transactions);

        return transactions.Count == 0
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: null dalam BuildTransactionHistoryJson.
            ? null
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: JsonSerializer.Serialize(transactions dalam
            // BuildTransactionHistoryJson.
            : JsonSerializer.Serialize(transactions
                .OrderBy(item => item.Key)
                .Select(item => item.Value));
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
    {
        var days = new SortedDictionary<int, Dictionary<string, object?>>();
        AppendActionUsageRows(
            actionSequenceRawValue,
            ["actions"],
            days);
        AppendActionUsageRows(
            actionRepetitionRawValue,
            ["total_actions", "distinct_actions", "repeated_actions", "diversity_score"],
            days);

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
    }

    private static void AppendActionUsageRows(
        // Parameter `rawValue` bertipe `string` membawa nilai raw nilai.
        string rawValue,
        // Parameter `propertyNames` bertipe `IReadOnlyList<string>` membawa nilai property nama.
        IReadOnlyList<string> propertyNames,
        // Parameter `output` bertipe `IDictionary<int, Dictionary<string, object?>>` membawa nilai output.
        IDictionary<int, Dictionary<string, object?>> output)
    {
        try
        {
            using var document = JsonDocument.Parse(rawValue);
            if (document.RootElement.ValueKind != JsonValueKind.Array)
            {
                return;
            }

            // Mengulangi setiap elemen `document.RootElement.EnumerateArray().Where(item => item.ValueKind == JsonValueKind.Object)`; elemen saat ini disimpan
            // sebagai `item` bertipe `var` untuk diproses oleh badan loop dalam AppendActionUsageRows.
            foreach (var item in document.RootElement.EnumerateArray().Where(item => item.ValueKind == JsonValueKind.Object))
            {
                if (!item.TryGetProperty("day_index", out var dayValue)
                    || !dayValue.TryGetInt32(out var dayIndex)
                    || dayIndex <= 0)
                {
                    // Melewati sisa pernyataan pada iterasi saat ini dan melanjutkan ke elemen/iterasi berikutnya dalam AppendActionUsageRows.
                    continue;
                }

                if (!output.TryGetValue(dayIndex, out var values))
                {
                    values = new Dictionary<string, object?>
                    {
                        ["day_index"] = dayIndex,
                        ["actions"] = null,
                        ["total_actions"] = null,
                        ["distinct_actions"] = null,
                        ["repeated_actions"] = null,
                        ["diversity_score"] = null
                    };
                    output[dayIndex] = values;
                }

                // Mengulangi setiap elemen `propertyNames`; elemen saat ini disimpan sebagai `propertyName` bertipe `var` untuk diproses oleh badan loop dalam
                // AppendActionUsageRows.
                foreach (var propertyName in propertyNames)
                {
                    values[propertyName] = ReadClonedProperty(item, propertyName);
                }
            }
        }
        // Menangani exception `JsonException` melalui variabel dalam AppendActionUsageRows.
        catch (JsonException)
        {
            // Satu seri yang rusak tidak menghilangkan data valid dari seri lainnya.
        // Menutup scope penanganan exception catch; bagian berikut berada di luar batas blok tersebut dalam AppendActionUsageRows.
        }
    }

    private static void AppendTransactionRows(
        // Parameter `rawValue` bertipe `string` membawa nilai raw nilai.
        string rawValue,
        // Parameter `sourceProperty` bertipe `string` membawa nilai source property.
        string sourceProperty,
        // Parameter `targetProperty` bertipe `string` membawa nilai target property.
        string targetProperty,
        // Parameter `output` bertipe `IDictionary<long, Dictionary<string, object?>>` membawa nilai output.
        IDictionary<long, Dictionary<string, object?>> output)
    {
        try
        {
            using var document = JsonDocument.Parse(rawValue);
            if (document.RootElement.ValueKind != JsonValueKind.Array)
            {
                return;
            }

            // Mengulangi setiap elemen `document.RootElement.EnumerateArray().Where(item => item.ValueKind == JsonValueKind.Object)`; elemen saat ini disimpan
            // sebagai `item` bertipe `var` untuk diproses oleh badan loop dalam AppendTransactionRows.
            foreach (var item in document.RootElement.EnumerateArray().Where(item => item.ValueKind == JsonValueKind.Object))
            {
                if (!item.TryGetProperty("sequence_number", out var sequenceValue)
                    || !sequenceValue.TryGetInt64(out var sequenceNumber))
                {
                    // Melewati sisa pernyataan pada iterasi saat ini dan melanjutkan ke elemen/iterasi berikutnya dalam AppendTransactionRows.
                    continue;
                }

                if (!output.TryGetValue(sequenceNumber, out var values))
                {
                    values = new Dictionary<string, object?>
                    {
                        ["day_index"] = null,
                        ["action_slot"] = null,
                        ["sequence_number"] = sequenceNumber,
                        ["cashflow_category"] = null,
                        ["coins_in_event"] = 0,
                        ["coins_out_event"] = 0,
                        ["coin_change"] = null,
                        ["coin_balance_after_event"] = null
                    };
                    output[sequenceNumber] = values;
                }

                SetContextValueIfMissing(values, item, "day_index");
                SetContextValueIfMissing(values, item, "action_slot");
                SetContextValueIfMissing(values, item, "cashflow_category");
                values[targetProperty] = ReadClonedProperty(item, sourceProperty);
            }
        }
        // Menangani exception `JsonException` melalui variabel dalam AppendTransactionRows.
        catch (JsonException)
        {
            // Satu seri yang rusak tidak menghalangi seri valid lainnya untuk ditampilkan.
        // Menutup scope penanganan exception catch; bagian berikut berada di luar batas blok tersebut dalam AppendTransactionRows.
        }
    }

    private static void SetContextValueIfMissing(
        // Parameter `values` bertipe `IDictionary<string, object?>` membawa nilai nilai.
        IDictionary<string, object?> values,
        // Parameter `item` bertipe `JsonElement` membawa nilai elemen.
        JsonElement item,
        // Parameter `propertyName` bertipe `string` membawa nilai property nama.
        string propertyName)
    {
        if (values[propertyName] is null)
        {
            values[propertyName] = ReadClonedProperty(item, propertyName);
        }
    }

    private static object? ReadClonedProperty(JsonElement item, string propertyName) =>
        item.TryGetProperty(propertyName, out var value) ? value.Clone() : null;

    private static string FormatCollectionValue(
        // Parameter `element` bertipe `JsonElement` membawa nilai element.
        JsonElement element,
        // Parameter `trueText` bertipe `string` membawa nilai true text.
        string trueText,
        // Parameter `falseText` bertipe `string` membawa nilai false text.
        string falseText,
        // Parameter `nullText` bertipe `string` membawa nilai null text.
        string nullText)
    {
        return element.ValueKind switch
        {
            // Untuk pola `JsonValueKind.Array`, menghasilkan memanggil `string.Join` dengan `”, ”`, `element.EnumerateArray() .Select(item =>
            // FormatCollectionValue(item, trueText, falseText, nullText))` sebagai hasil switch.
            JsonValueKind.Array => string.Join(", ", element.EnumerateArray()
                .Select(item => FormatCollectionValue(item, trueText, falseText, nullText))),
            // Untuk pola `JsonValueKind.Object`, menghasilkan memanggil `string.Join` dengan `” · ”`, `element.EnumerateObject() .Select(property =>
            // $”{property.Name}: {FormatCollectionValue(property.Value, trueText, falseText, nullText)}”)` sebagai hasil switch.
            JsonValueKind.Object => string.Join(" · ", element.EnumerateObject()
                .Select(property => $"{property.Name}: {FormatCollectionValue(property.Value, trueText, falseText, nullText)}")),
            // Untuk pola `_`, menghasilkan memanggil `FormatJsonLeafValue` dengan `element`, `trueText`, `falseText`, `nullText` sebagai hasil switch.
            _ => FormatJsonLeafValue(element, trueText, falseText, nullText)
        };
    }

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
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                {
                    var hasProperty = false;
                    // Mengulangi setiap elemen `element.EnumerateObject()`; elemen saat ini disimpan sebagai `property` bertipe `var` untuk diproses oleh badan loop
                    // dalam Visit.
                    foreach (var property in element.EnumerateObject())
                    {
                        hasProperty = true;
                        var nextPath = string.IsNullOrWhiteSpace(path) ? property.Name : $"{path}.{property.Name}";
                        Visit(property.Value, nextPath, output, trueText, falseText, nullText);
                    }

                    if (!hasProperty && !string.IsNullOrWhiteSpace(path))
                    {
                        output.Add((path, "{}"));
                    }

                    break;
                }
            case JsonValueKind.Array:
                {
                    var index = 0;
                    // Mengulangi setiap elemen `element.EnumerateArray()`; elemen saat ini disimpan sebagai `item` bertipe `var` untuk diproses oleh badan loop dalam
                    // Visit.
                    foreach (var item in element.EnumerateArray())
                    {
                        Visit(item, $"{path}[{index}]", output, trueText, falseText, nullText);
                        index += 1;
                    }

                    if (index == 0 && !string.IsNullOrWhiteSpace(path))
                    {
                        output.Add((path, "[]"));
                    }

                    break;
                }
            case JsonValueKind.String:
            case JsonValueKind.True:
            case JsonValueKind.False:
            case JsonValueKind.Number:
            case JsonValueKind.Null:
                output.Add((
                    string.IsNullOrWhiteSpace(path) ? "value" : path,
                    FormatJsonLeafValue(element, trueText, falseText, nullText)));
                break;
            default:
                output.Add((
                    string.IsNullOrWhiteSpace(path) ? "value" : path,
                    element.ToString()));
                break;
        }
    }

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
    {
        if (element.ValueKind != JsonValueKind.Object)
        {
            output.Add((
                string.IsNullOrWhiteSpace(path) ? "value" : path,
                FormatJsonLeafValue(element, trueText, falseText, nullText)));
            return;
        }

        // Mengulangi setiap elemen `element.EnumerateObject()`; elemen saat ini disimpan sebagai `property` bertipe `var` untuk diproses oleh badan loop
        // dalam VisitVariables.
        foreach (var property in element.EnumerateObject())
        {
            var nextPath = string.IsNullOrWhiteSpace(path) ? property.Name : $"{path}.{property.Name}";
            if (property.Value.ValueKind == JsonValueKind.Object)
            {
                VisitVariables(property.Value, nextPath, output, trueText, falseText, nullText);
                // Melewati sisa pernyataan pada iterasi saat ini dan melanjutkan ke elemen/iterasi berikutnya dalam VisitVariables.
                continue;
            }

            output.Add((nextPath, FormatJsonLeafValue(property.Value, trueText, falseText, nullText)));
        }
    }
}
