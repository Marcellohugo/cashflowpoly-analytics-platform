// Fungsi file: Menyediakan transformasi, lokalisasi, atau koneksi UI melalui PlayerMetricJsonMapper.
using System.Text.Json;

namespace Cashflowpoly.Ui.Infrastructure;

public sealed record PlayerMetricCollectionTable(
    IReadOnlyList<string> Columns,
    IReadOnlyList<IReadOnlyList<string>> Rows);

/// <summary>
/// Mapper JSON metrik gameplay menjadi struktur baris/grup yang siap dirender oleh Razor.
/// </summary>
public static class PlayerMetricJsonMapper
{
    /// <summary>
    /// Memformat nilai leaf JsonElement menjadi teks tampilan.
    /// </summary>
    public static string FormatJsonLeafValue(JsonElement element, string trueText, string falseText, string nullText)
    {
        return element.ValueKind switch
        {
            JsonValueKind.String => element.GetString() ?? string.Empty,
            JsonValueKind.True => trueText,
            JsonValueKind.False => falseText,
            JsonValueKind.Number => element.GetRawText(),
            JsonValueKind.Null => nullText,
            _ => element.GetRawText()
        };
    }

    /// <summary>
    /// Mengubah JSON bertingkat menjadi daftar path daun dan value tampilan.
    /// </summary>
    public static List<(string Path, string Value)> FlattenJsonLeaves(
        JsonElement? root,
        string trueText,
        string falseText,
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
    public static List<(string GroupKey, List<(string Path, string Value)> Rows)> BuildMetricGroups(
        JsonElement? root,
        string trueText,
        string falseText,
        string nullText)
    {
        var groups = new List<(string GroupKey, List<(string Path, string Value)> Rows)>();
        if (!root.HasValue || root.Value.ValueKind != JsonValueKind.Object)
        {
            return groups;
        }

        var scalarRows = new List<(string Path, string Value)>();
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
    public static List<(string GroupKey, List<(string Path, string Value)> Rows)> BuildMetricVariableGroups(
        JsonElement? root,
        string trueText,
        string falseText,
        string nullText)
    {
        var groups = new List<(string GroupKey, List<(string Path, string Value)> Rows)>();
        if (!root.HasValue || root.Value.ValueKind != JsonValueKind.Object)
        {
            return groups;
        }

        foreach (var group in root.Value.EnumerateObject())
        {
            if (group.Value.ValueKind != JsonValueKind.Object)
            {
                groups.Add((group.Name, new List<(string Path, string Value)>
                {
                    (group.Name, FormatJsonLeafValue(group.Value, trueText, falseText, nullText))
                }));
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
    public static Dictionary<string, JsonElement> BuildMetricGroupElements(JsonElement? root)
    {
        var groups = new Dictionary<string, JsonElement>(StringComparer.OrdinalIgnoreCase);
        if (!root.HasValue || root.Value.ValueKind != JsonValueKind.Object)
        {
            return groups;
        }

        foreach (var property in root.Value.EnumerateObject())
        {
            groups[property.Name] = property.Value.Clone();
        }

        return groups;
    }

    /// <summary>
    /// Membaca nilai numerik dari JsonElement number atau string numerik.
    /// </summary>
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
    public static PlayerMetricCollectionTable? BuildCollectionTable(
        string rawValue,
        string trueText,
        string falseText,
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
                    ? null
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
        catch (JsonException)
        {
            return null;
        }
    }

    /// <summary>
    /// Memasangkan jumlah dan peringkat donasi berdasarkan hari, bukan posisi array.
    /// </summary>
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
                    continue;
                }

                foreach (var item in document.RootElement.EnumerateArray())
                {
                    if (item.ValueKind != JsonValueKind.Object
                        || !item.TryGetProperty("day_index", out var dayValue)
                        || dayValue.ValueKind != JsonValueKind.Number
                        || !dayValue.TryGetInt32(out var day)
                        || day <= 0)
                    {
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
            catch (JsonException)
            {
                // Seri yang tidak tersedia tidak menghilangkan data dari seri lainnya.
            }
        }

        return JsonSerializer.Serialize(donations.Values);
    }

    /// <summary>
    /// Menggabungkan transaksi, perubahan bersih, dan saldo menjadi satu riwayat
    /// yang tersusun berdasarkan nomor urut kejadian.
    /// </summary>
    public static string? BuildTransactionHistoryJson(
        string outgoingRawValue,
        string incomingRawValue,
        string netChangeRawValue,
        string balanceRawValue)
    {
        var transactions = new Dictionary<long, Dictionary<string, object?>>();
        AppendTransactionRows(outgoingRawValue, "amount", "coins_out_event", transactions);
        AppendTransactionRows(incomingRawValue, "amount", "coins_in_event", transactions);
        AppendTransactionRows(netChangeRawValue, "net", "coin_change", transactions);
        AppendTransactionRows(balanceRawValue, "coins", "coin_balance_after_event", transactions);

        return transactions.Count == 0
            ? null
            : JsonSerializer.Serialize(transactions
                .OrderBy(item => item.Key)
                .Select(item => item.Value));
    }

    /// <summary>
    /// Menggabungkan urutan aksi dan ringkasan pengulangan per hari agar informasi
    /// yang sama tidak ditampilkan dalam dua tabel terpisah.
    /// </summary>
    public static string? BuildActionUsageHistoryJson(
        string actionSequenceRawValue,
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

        return days.Count == 0
            ? null
            : JsonSerializer.Serialize(days.Values);
    }

    private static void AppendActionUsageRows(
        string rawValue,
        IReadOnlyList<string> propertyNames,
        IDictionary<int, Dictionary<string, object?>> output)
    {
        try
        {
            using var document = JsonDocument.Parse(rawValue);
            if (document.RootElement.ValueKind != JsonValueKind.Array)
            {
                return;
            }

            foreach (var item in document.RootElement.EnumerateArray().Where(item => item.ValueKind == JsonValueKind.Object))
            {
                if (!item.TryGetProperty("day_index", out var dayValue)
                    || !dayValue.TryGetInt32(out var dayIndex)
                    || dayIndex <= 0)
                {
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

                foreach (var propertyName in propertyNames)
                {
                    values[propertyName] = ReadClonedProperty(item, propertyName);
                }
            }
        }
        catch (JsonException)
        {
            // Satu seri yang rusak tidak menghilangkan data valid dari seri lainnya.
        }
    }

    private static void AppendTransactionRows(
        string rawValue,
        string sourceProperty,
        string targetProperty,
        IDictionary<long, Dictionary<string, object?>> output)
    {
        try
        {
            using var document = JsonDocument.Parse(rawValue);
            if (document.RootElement.ValueKind != JsonValueKind.Array)
            {
                return;
            }

            foreach (var item in document.RootElement.EnumerateArray().Where(item => item.ValueKind == JsonValueKind.Object))
            {
                if (!item.TryGetProperty("sequence_number", out var sequenceValue)
                    || !sequenceValue.TryGetInt64(out var sequenceNumber))
                {
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
        catch (JsonException)
        {
            // Satu seri yang rusak tidak menghalangi seri valid lainnya untuk ditampilkan.
        }
    }

    private static void SetContextValueIfMissing(
        IDictionary<string, object?> values,
        JsonElement item,
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
        JsonElement element,
        string trueText,
        string falseText,
        string nullText)
    {
        return element.ValueKind switch
        {
            JsonValueKind.Array => string.Join(", ", element.EnumerateArray()
                .Select(item => FormatCollectionValue(item, trueText, falseText, nullText))),
            JsonValueKind.Object => string.Join(" · ", element.EnumerateObject()
                .Select(property => $"{property.Name}: {FormatCollectionValue(property.Value, trueText, falseText, nullText)}")),
            _ => FormatJsonLeafValue(element, trueText, falseText, nullText)
        };
    }

    private static void Visit(
        JsonElement element,
        string path,
        List<(string Path, string Value)> output,
        string trueText,
        string falseText,
        string nullText)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                {
                    var hasProperty = false;
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
        JsonElement element,
        string path,
        List<(string Path, string Value)> output,
        string trueText,
        string falseText,
        string nullText)
    {
        if (element.ValueKind != JsonValueKind.Object)
        {
            output.Add((
                string.IsNullOrWhiteSpace(path) ? "value" : path,
                FormatJsonLeafValue(element, trueText, falseText, nullText)));
            return;
        }

        foreach (var property in element.EnumerateObject())
        {
            var nextPath = string.IsNullOrWhiteSpace(path) ? property.Name : $"{path}.{property.Name}";
            if (property.Value.ValueKind == JsonValueKind.Object)
            {
                VisitVariables(property.Value, nextPath, output, trueText, falseText, nullText);
                continue;
            }

            output.Add((nextPath, FormatJsonLeafValue(property.Value, trueText, falseText, nullText)));
        }
    }
}
