// Fungsi file: Memetakan JSON metrik pemain menjadi baris dan grup tampilan untuk halaman detail pemain.
using System.Text.Json;

namespace Cashflowpoly.Ui.Infrastructure;

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
}
