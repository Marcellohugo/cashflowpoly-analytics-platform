// Fungsi file: Membangun payload JSON chart metrik pemain untuk halaman detail.
using System.Globalization;
using System.Text.Json;

namespace Cashflowpoly.Ui.Infrastructure;

/// <summary>
/// Builder payload chart metrik pemain yang dapat diuji tanpa Razor/browser.
/// </summary>
public static class PlayerMetricChartPayloadBuilder
{
    /// <summary>
    /// Membangun label sumber data untuk path metrik.
    /// </summary>
    public static string BuildMetricSourceLabel(string? metricPath, bool isRawDomain, Func<string, string> translate)
    {
        var root = isRawDomain
            ? translate("players.details.raw_title")
            : translate("players.details.derived_title");
        var normalized = (metricPath ?? string.Empty).Trim().Trim('.');
        if (string.IsNullOrWhiteSpace(normalized))
        {
            return root;
        }

        return $"{root} -> {PlayerMetricLabelFormatter.FormatMetricPathLabel(normalized, translate)}";
    }

    /// <summary>
    /// Membangun chart batang ringkasan gabungan dari rows numerik.
    /// </summary>
    public static (string Title, string Json)? BuildMergedRowChart(
        string domainTitle,
        IEnumerable<(string Path, string Value)> rows,
        bool isRawDomain,
        Func<string, string> translate)
    {
        var numericRows = rows
            .Where(row => !string.IsNullOrWhiteSpace(row.Path))
            .Where(row => PlayerMetricLabelFormatter.TryParseMetricNumber(row.Value, out _))
            .ToList();

        var summaryRows = numericRows
            .Where(row => PlayerMetricLabelFormatter.IsPreferredCombinedSummaryPath(row.Path))
            .ToList();
        if (summaryRows.Count == 0)
        {
            summaryRows = numericRows
                .Where(row => PlayerMetricLabelFormatter.IsFallbackCombinedSummaryPath(row.Path))
                .ToList();
        }

        if (summaryRows.Count == 0)
        {
            return null;
        }

        var points = new List<(string Path, string Label, double Value, string Formula)>();
        var labelUsage = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        foreach (var row in summaryRows)
        {
            if (PlayerMetricLabelFormatter.TryParseMetricNumber(row.Value, out var value))
            {
                var baseLabel = PlayerMetricLabelFormatter.FormatMetricPathLabel(row.Path, translate).Trim();
                if (string.IsNullOrWhiteSpace(baseLabel))
                {
                    baseLabel = translate("players.details.metric_fallback");
                }

                if (!labelUsage.TryGetValue(baseLabel, out var usageCount))
                {
                    labelUsage[baseLabel] = 1;
                    points.Add((row.Path, baseLabel, value, BuildFormulaHint(row.Path, isRawDomain, translate)));
                    continue;
                }

                usageCount += 1;
                labelUsage[baseLabel] = usageCount;
                points.Add((row.Path, $"{baseLabel} ({usageCount})", value, BuildFormulaHint(row.Path, isRawDomain, translate)));
            }
        }

        const int maxPoints = 36;
        if (points.Count > maxPoints)
        {
            points = points.Take(maxPoints).ToList();
        }

        var payload = new
        {
            chartType = "bar",
            labels = points.Select(point => point.Label).ToList(),
            keys = points.Select(point => point.Path).ToList(),
            formulas = points.Select(point => point.Formula).ToList(),
            detailLabel = translate("players.details.source_calc_label"),
            detailFallback = isRawDomain
                ? translate("players.details.source_raw_summary")
                : translate("players.details.source_derived_summary"),
            series = new[]
            {
                new
                {
                    name = translate("common.value"),
                    values = points.Select(point => (double?)point.Value).ToList()
                }
            }
        };

        return ($"{domainTitle}: {translate("players.details.combined_snapshot")}", JsonSerializer.Serialize(payload));
    }

    /// <summary>
    /// Membangun semua chart line dari properti object group yang renderable.
    /// </summary>
    public static List<(string Title, string Json)> BuildLineChartsForGroup(
        string groupKey,
        JsonElement groupElement,
        bool isRawDomain,
        Func<string, string> translate)
    {
        var charts = new List<(string Title, string Json)>();
        if (groupElement.ValueKind != JsonValueKind.Object)
        {
            return charts;
        }

        foreach (var property in groupElement.EnumerateObject())
        {
            var chart = BuildLineChartPayload($"{groupKey}.{property.Name}", property.Value, isRawDomain, translate);
            if (chart.HasValue)
            {
                charts.Add(chart.Value);
            }
        }

        return charts;
    }

    /// <summary>
    /// Membangun payload chart line/bar dari JsonElement array atau object numerik.
    /// </summary>
    public static (string Title, string Json)? BuildLineChartPayload(
        string path,
        JsonElement element,
        bool isRawDomain,
        Func<string, string> translate)
    {
        var labels = new List<string>();
        var series = new Dictionary<string, List<double?>>(StringComparer.OrdinalIgnoreCase);

        void AppendLabel(string label)
        {
            labels.Add(label);
            foreach (var values in series.Values)
            {
                values.Add(null);
            }
        }

        void SetSeriesValue(string seriesName, double value)
        {
            if (!series.TryGetValue(seriesName, out var values))
            {
                values = Enumerable.Repeat<double?>(null, labels.Count).ToList();
                series[seriesName] = values;
            }

            values[labels.Count - 1] = value;
        }

        if (element.ValueKind == JsonValueKind.Array)
        {
            var items = element.EnumerateArray().ToList();
            if (items.Count == 0)
            {
                return null;
            }

            if (items.All(item => PlayerMetricJsonMapper.TryGetNumericValue(item, out _)))
            {
                for (var index = 0; index < items.Count; index++)
                {
                    AppendLabel((index + 1).ToString());
                    if (PlayerMetricJsonMapper.TryGetNumericValue(items[index], out var numericValue))
                    {
                        SetSeriesValue("value", numericValue);
                    }
                }
            }
            else if (items.All(item => item.ValueKind == JsonValueKind.Object))
            {
                BuildSeriesFromObjectArray(items, labels, series, AppendLabel, SetSeriesValue, translate);
            }
            else
            {
                return null;
            }
        }
        else if (element.ValueKind == JsonValueKind.Object &&
                 path.Contains("_per_", StringComparison.OrdinalIgnoreCase))
        {
            foreach (var property in element.EnumerateObject())
            {
                if (!PlayerMetricJsonMapper.TryGetNumericValue(property.Value, out var numericValue))
                {
                    continue;
                }

                AppendLabel(PlayerMetricLabelFormatter.HumanizeMetricKey(property.Name, translate));
                SetSeriesValue("value", numericValue);
            }
        }
        else
        {
            return null;
        }

        var activeSeries = series
            .Where(item => item.Value.Any(value => value.HasValue))
            .Select(item => new
            {
                name = PlayerMetricLabelFormatter.HumanizeMetricKey(item.Key, translate),
                values = item.Value
            })
            .ToList();
        if (labels.Count == 0 || activeSeries.Count == 0)
        {
            return null;
        }

        var pathParts = path.Split('.', StringSplitOptions.RemoveEmptyEntries);
        var groupLabel = pathParts.Length > 0
            ? PlayerMetricLabelFormatter.HumanizeMetricKey(pathParts[0], translate)
            : translate("players.details.series");
        var metricLabel = pathParts.Length > 1
            ? PlayerMetricLabelFormatter.HumanizeMetricKey(pathParts[^1], translate)
            : PlayerMetricLabelFormatter.HumanizeMetricKey(path, translate);
        var title = $"{groupLabel}: {metricLabel}";
        var sourcePath = BuildMetricSourceLabel(path, isRawDomain, translate);
        var payload = new
        {
            labels,
            series = activeSeries,
            detailLabel = translate("players.details.source_calc_label"),
            detailFallback = isRawDomain
                ? string.Format(
                    CultureInfo.CurrentCulture,
                    translate("players.details.source_line_raw_template"),
                    sourcePath)
                : string.Format(
                    CultureInfo.CurrentCulture,
                    translate("players.details.source_line_derived_template"),
                    sourcePath)
        };

        return (title, JsonSerializer.Serialize(payload));
    }

    private static string BuildFormulaHint(string metricPath, bool isRawDomain, Func<string, string> translate)
    {
        var key = (metricPath ?? string.Empty).ToLowerInvariant();
        var sourcePath = BuildMetricSourceLabel(metricPath, isRawDomain, translate);

        string WithSource(string formulaKey)
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                translate("players.details.source_calc_template"),
                sourcePath,
                translate(formulaKey));
        }

        if (isRawDomain &&
            (key.Contains("coins_net_end_game") || key.Contains("coins_net_end")))
        {
            return WithSource("players.details.formula.coins_net_end");
        }

        if (isRawDomain)
        {
            if (key.Contains("cash_in") || key.Contains("earned"))
            {
                return WithSource("players.details.formula.raw.cash_in_sum");
            }

            if (key.Contains("cash_out") || key.Contains("spent"))
            {
                return WithSource("players.details.formula.raw.cash_out_sum");
            }

            if (key.Contains("per_turn") || key.Contains("progression"))
            {
                return WithSource("players.details.formula.raw.per_turn_aggregate");
            }

            return string.Format(
                CultureInfo.CurrentCulture,
                translate("players.details.source_line_raw_template"),
                sourcePath);
        }

        if (key.Contains("happiness_points"))
        {
            return WithSource("players.details.formula.derived.happiness_points");
        }

        if (key.Contains("cashflow_net") || key.Contains("net_cashflow") || key.Contains("coins_net"))
        {
            return WithSource("players.details.formula.derived.net_cashflow");
        }

        if (key.Contains("donation_points"))
        {
            return WithSource("players.details.formula.derived.donation_points");
        }

        if (key.Contains("pension_points"))
        {
            return WithSource("players.details.formula.derived.pension_points");
        }

        if (key.Contains("saving_goal_points"))
        {
            return WithSource("players.details.formula.derived.saving_goal_points");
        }

        if (key.Contains("need_points"))
        {
            return WithSource("players.details.formula.derived.need_points");
        }

        if (key.Contains("penalty"))
        {
            return WithSource("players.details.formula.derived.penalty");
        }

        if (key.Contains("points"))
        {
            return WithSource("players.details.formula.derived.points_weight");
        }

        return WithSource("players.details.formula.derived.default");
    }

    private static void BuildSeriesFromObjectArray(
        List<JsonElement> items,
        List<string> labels,
        Dictionary<string, List<double?>> series,
        Action<string> appendLabel,
        Action<string, double> setSeriesValue,
        Func<string, string> translate)
    {
        var xAxisCandidates = new[]
        {
            "turn_number",
            "day_index",
            "friday_index",
            "order_index",
            "card_index",
            "goal_index",
            "sequence_number",
            "index"
        };

        string? xKey = null;
        foreach (var candidate in xAxisCandidates)
        {
            if (items.Any(item => item.TryGetProperty(candidate, out _)))
            {
                xKey = candidate;
                break;
            }
        }

        if (xKey is null)
        {
            xKey = items
                .SelectMany(item => item.EnumerateObject().Select(prop => prop.Name))
                .FirstOrDefault(name =>
                    name.EndsWith("_number", StringComparison.OrdinalIgnoreCase) ||
                    name.EndsWith("_index", StringComparison.OrdinalIgnoreCase));
        }

        for (var index = 0; index < items.Count; index++)
        {
            var item = items[index];
            var label = (index + 1).ToString();
            var rowValues = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
            foreach (var property in item.EnumerateObject())
            {
                if (!string.IsNullOrWhiteSpace(xKey) &&
                    property.Name.Equals(xKey, StringComparison.OrdinalIgnoreCase))
                {
                    label = PlayerMetricJsonMapper.FormatJsonLeafValue(
                        property.Value,
                        translate("state.true"),
                        translate("state.false"),
                        translate("state.null"));
                    continue;
                }

                if (PlayerMetricJsonMapper.TryGetNumericValue(property.Value, out var numericValue))
                {
                    rowValues[property.Name] = numericValue;
                    continue;
                }

                if (property.Value.ValueKind == JsonValueKind.Array &&
                    property.Name.Equals("actions", StringComparison.OrdinalIgnoreCase))
                {
                    rowValues["actions_count"] = property.Value.GetArrayLength();
                }
            }

            appendLabel(label);
            foreach (var rowValue in rowValues)
            {
                setSeriesValue(rowValue.Key, rowValue.Value);
            }
        }
    }
}
