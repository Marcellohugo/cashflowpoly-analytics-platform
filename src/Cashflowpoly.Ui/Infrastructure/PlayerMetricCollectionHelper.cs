// Fungsi file: Helper koleksi row/chart metrik pemain untuk penyusunan section UI.
namespace Cashflowpoly.Ui.Infrastructure;

/// <summary>
/// Helper murni untuk menggabungkan dan memfilter baris serta chart metrik.
/// </summary>
public static class PlayerMetricCollectionHelper
{
    /// <summary>
    /// Menggabungkan baris berdasarkan path unik, mempertahankan kemunculan pertama.
    /// </summary>
    public static List<(string Path, string Value)> MergeUniqueRows(params IEnumerable<(string Path, string Value)>[] rowSets)
    {
        var result = new List<(string Path, string Value)>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var rowSet in rowSets)
        {
            foreach (var row in rowSet)
            {
                if (string.IsNullOrWhiteSpace(row.Path))
                {
                    continue;
                }

                if (seen.Add(row.Path))
                {
                    result.Add(row);
                }
            }
        }

        return result;
    }

    /// <summary>
    /// Mengambil baris dari beberapa grup sesuai urutan key yang diminta.
    /// </summary>
    public static List<(string Path, string Value)> GetGroupRows(
        Dictionary<string, List<(string Path, string Value)>> source,
        params string[] keys)
    {
        var rows = new List<(string Path, string Value)>();
        foreach (var key in keys)
        {
            if (!source.TryGetValue(key, out var groupRows))
            {
                continue;
            }

            rows.AddRange(groupRows);
        }

        return rows;
    }

    /// <summary>
    /// Memfilter baris yang path-nya mengandung salah satu keyword.
    /// </summary>
    public static List<(string Path, string Value)> FilterRowsByKeywords(
        IEnumerable<(string Path, string Value)> rows,
        params string[] keywords)
    {
        return rows
            .Where(row => keywords.Any(keyword => row.Path.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
            .ToList();
    }

    /// <summary>
    /// Menggabungkan chart berdasarkan title unik, mempertahankan kemunculan pertama.
    /// </summary>
    public static List<(string Title, string Json)> MergeUniqueCharts(params IEnumerable<(string Title, string Json)>[] chartSets)
    {
        var result = new List<(string Title, string Json)>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var chartSet in chartSets)
        {
            foreach (var chart in chartSet)
            {
                if (string.IsNullOrWhiteSpace(chart.Title))
                {
                    continue;
                }

                if (seen.Add(chart.Title))
                {
                    result.Add(chart);
                }
            }
        }

        return result;
    }

    /// <summary>
    /// Memfilter chart yang title-nya mengandung salah satu keyword.
    /// </summary>
    public static List<(string Title, string Json)> FilterChartsByKeywords(
        IEnumerable<(string Title, string Json)> charts,
        params string[] keywords)
    {
        if (keywords is null || keywords.Length == 0)
        {
            return charts.ToList();
        }

        return charts
            .Where(chart => keywords.Any(keyword => chart.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
            .ToList();
    }

    /// <summary>
    /// Mengambil chart dari beberapa grup sesuai urutan key yang diminta.
    /// </summary>
    public static List<(string Title, string Json)> GetGroupCharts(
        Dictionary<string, List<(string Title, string Json)>> source,
        params string[] keys)
    {
        var charts = new List<(string Title, string Json)>();
        foreach (var key in keys)
        {
            if (!source.TryGetValue(key, out var groupCharts))
            {
                continue;
            }

            charts.AddRange(groupCharts);
        }

        return charts;
    }
}
