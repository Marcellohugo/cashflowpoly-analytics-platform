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
                new[] { "value" },
                items.Select(item => (IReadOnlyList<string>)new[]
                {
                    FormatCollectionValue(item, trueText, falseText, nullText)
                }).ToList());
        }
        catch (JsonException)
        {
            return null;
        }
    }

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
