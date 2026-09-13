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
{
    /// <summary>
    /// Membangun label sumber data untuk path metrik.
    /// </summary>
    // Mendefinisikan metode `BuildMetricSourceLabel` dengan hasil bertipe `string`. Membangun label sumber data untuk path metrik. Masukan: Parameter
    // `metricPath` bertipe `string?` membawa nilai metric path; nilai null diizinkan ketika data opsional belum tersedia; Parameter `isRawDomain`
    // bertipe `bool` membawa nilai berstatus raw domain; Parameter `translate` bertipe `Func<string, string>` membawa nilai translate.
    public static string BuildMetricSourceLabel(string? metricPath, bool isRawDomain, Func<string, string> translate)
    {
        var root = isRawDomain
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: translate(”players.details.raw_title”) dalam BuildMetricSourceLabel.
            ? translate("players.details.raw_title")
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: translate(”players.details.derived_title”); dalam
            // BuildMetricSourceLabel.
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
    {
        var candidateRows = rows
            .Where(row => !string.IsNullOrWhiteSpace(row.Path))
            .ToList();
        var numericRows = candidateRows
            .Where(row => PlayerMetricLabelFormatter.TryParseMetricNumber(row.Value, out _))
            .ToList();

        var summaryRows = SelectSummaryRows(numericRows);
        var useZeroFallback = false;
        if (summaryRows.Count == 0 && numericRows.Count > 0)
        {
            summaryRows = numericRows.Take(8).ToList();
        }

        if (summaryRows.Count == 0)
        {
            summaryRows = SelectSummaryRows(candidateRows);
            useZeroFallback = summaryRows.Count > 0;
        }

        if (summaryRows.Count == 0 && candidateRows.Count > 0)
        {
            summaryRows = candidateRows.Take(8).ToList();
            useZeroFallback = true;
        }

        var points = BuildChartPoints(summaryRows, isRawDomain, translate, useZeroFallback);
        if (points.Count == 0)
        {
            points =
            [
                (
                    string.Empty,
                    translate("players.details.metric_fallback"),
                    0d,
                    BuildSummaryFallbackDetail(isRawDomain, translate))
            ];
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
                // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: translate(”players.details.source_raw_summary”) dalam
                // BuildMergedRowChart.
                ? translate("players.details.source_raw_summary")
                // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: translate(”players.details.source_derived_summary”), dalam
                // BuildMergedRowChart.
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

    private static List<(string Path, string Value)> SelectSummaryRows(IEnumerable<(string Path, string Value)> rows)
    {
        var preferredRows = rows
            .Where(row => PlayerMetricLabelFormatter.IsPreferredCombinedSummaryPath(row.Path))
            .ToList();
        if (preferredRows.Count > 0)
        {
            return preferredRows;
        }

        return rows
            .Where(row => PlayerMetricLabelFormatter.IsFallbackCombinedSummaryPath(row.Path))
            .ToList();
    }

    private static List<(string Path, string Label, double Value, string Formula)> BuildChartPoints(
        // Parameter `rows` bertipe `IEnumerable<(string Path, string Value)>` membawa nilai baris.
        IEnumerable<(string Path, string Value)> rows,
        // Parameter `isRawDomain` bertipe `bool` membawa nilai berstatus raw domain.
        bool isRawDomain,
        // Parameter `translate` bertipe `Func<string, string>` membawa nilai translate.
        Func<string, string> translate,
        // Parameter `allowZeroFallback` bertipe `bool` membawa nilai allow zero fallback.
        bool allowZeroFallback)
    {
        var points = new List<(string Path, string Label, double Value, string Formula)>();
        var labelUsage = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        // Mengulangi setiap elemen `rows`; elemen saat ini disimpan sebagai `row` bertipe `var` untuk diproses oleh badan loop dalam BuildChartPoints.
        foreach (var row in rows)
        {
            var hasNumericValue = PlayerMetricLabelFormatter.TryParseMetricNumber(row.Value, out var value);
            if (!hasNumericValue && !allowZeroFallback)
            {
                // Melewati sisa pernyataan pada iterasi saat ini dan melanjutkan ke elemen/iterasi berikutnya dalam BuildChartPoints.
                continue;
            }

            var resolvedValue = hasNumericValue ? value : 0d;
            var baseLabel = PlayerMetricLabelFormatter.FormatMetricPathLabel(row.Path, translate).Trim();
            if (string.IsNullOrWhiteSpace(baseLabel))
            {
                baseLabel = translate("players.details.metric_fallback");
            }

            if (!labelUsage.TryGetValue(baseLabel, out var usageCount))
            {
                labelUsage[baseLabel] = 1;
                points.Add((row.Path, baseLabel, resolvedValue, BuildFormulaHint(row.Path, isRawDomain, translate)));
                // Melewati sisa pernyataan pada iterasi saat ini dan melanjutkan ke elemen/iterasi berikutnya dalam BuildChartPoints.
                continue;
            }

            usageCount += 1;
            labelUsage[baseLabel] = usageCount;
            points.Add((row.Path, $"{baseLabel} ({usageCount})", resolvedValue, BuildFormulaHint(row.Path, isRawDomain, translate)));
        }

        return points;
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
    {
        var charts = new List<(string Title, string Json)>();
        if (groupElement.ValueKind != JsonValueKind.Object)
        {
            return charts;
        }

        // Mengulangi setiap elemen `groupElement.EnumerateObject()`; elemen saat ini disimpan sebagai `property` bertipe `var` untuk diproses oleh badan
        // loop dalam BuildLineChartsForGroup.
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
    {
        var labels = new List<string>();
        var series = new Dictionary<string, List<double?>>(StringComparer.OrdinalIgnoreCase);

        void AppendLabel(string label)
        {
            labels.Add(label);
            // Mengulangi setiap elemen `series.Values`; elemen saat ini disimpan sebagai `values` bertipe `var` untuk diproses oleh badan loop dalam
            // AppendLabel.
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
            // Mengulangi setiap elemen `element.EnumerateObject()`; elemen saat ini disimpan sebagai `property` bertipe `var` untuk diproses oleh badan loop
            // dalam BuildLineChartPayload.
            foreach (var property in element.EnumerateObject())
            {
                if (!PlayerMetricJsonMapper.TryGetNumericValue(property.Value, out var numericValue))
                {
                    // Melewati sisa pernyataan pada iterasi saat ini dan melanjutkan ke elemen/iterasi berikutnya dalam BuildLineChartPayload.
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
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: PlayerMetricLabelFormatter.HumanizeMetricKey(pathParts[0], translate)
            // dalam BuildLineChartPayload.
            ? PlayerMetricLabelFormatter.HumanizeMetricKey(pathParts[0], translate)
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: translate(”players.details.series”); dalam BuildLineChartPayload.
            : translate("players.details.series");
        var metricLabel = pathParts.Length > 1
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: PlayerMetricLabelFormatter.HumanizeMetricKey(pathParts[^1],
            // translate) dalam BuildLineChartPayload.
            ? PlayerMetricLabelFormatter.HumanizeMetricKey(pathParts[^1], translate)
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: PlayerMetricLabelFormatter.HumanizeMetricKey(path, translate); dalam
            // BuildLineChartPayload.
            : PlayerMetricLabelFormatter.HumanizeMetricKey(path, translate);
        var title = $"{groupLabel}: {metricLabel}";
        var sourcePath = BuildMetricSourceLabel(path, isRawDomain, translate);
        var payload = new
        {
            labels,
            series = activeSeries,
            detailLabel = translate("players.details.source_calc_label"),
            detailFallback = isRawDomain
                // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: string.Format( dalam BuildLineChartPayload.
                ? string.Format(
                    CultureInfo.CurrentCulture,
                    translate("players.details.source_line_raw_template"),
                    sourcePath)
                // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: string.Format( dalam BuildLineChartPayload.
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

    private static string BuildSummaryFallbackDetail(bool isRawDomain, Func<string, string> translate)
    {
        return isRawDomain
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: translate(”players.details.source_raw_summary”) dalam
            // BuildSummaryFallbackDetail.
            ? translate("players.details.source_raw_summary")
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: translate(”players.details.source_derived_summary”); dalam
            // BuildSummaryFallbackDetail.
            : translate("players.details.source_derived_summary");
    }

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
    {
        var xAxisCandidates = new[]
        {
            "action_slot",
            "day_index",
            "friday_index",
            "order_index",
            "card_index",
            "goal_index",
            "sequence_number",
            "index"
        };

        string? xKey = null;
        // Mengulangi setiap elemen `xAxisCandidates`; elemen saat ini disimpan sebagai `candidate` bertipe `var` untuk diproses oleh badan loop dalam
        // BuildSeriesFromObjectArray.
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
            // Mengulangi setiap elemen `item.EnumerateObject()`; elemen saat ini disimpan sebagai `property` bertipe `var` untuk diproses oleh badan loop dalam
            // BuildSeriesFromObjectArray.
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
                    // Melewati sisa pernyataan pada iterasi saat ini dan melanjutkan ke elemen/iterasi berikutnya dalam BuildSeriesFromObjectArray.
                    continue;
                }

                if (PlayerMetricJsonMapper.TryGetNumericValue(property.Value, out var numericValue))
                {
                    rowValues[property.Name] = numericValue;
                    // Melewati sisa pernyataan pada iterasi saat ini dan melanjutkan ke elemen/iterasi berikutnya dalam BuildSeriesFromObjectArray.
                    continue;
                }

                if (property.Value.ValueKind == JsonValueKind.Array &&
                    property.Name.Equals("actions", StringComparison.OrdinalIgnoreCase))
                {
                    rowValues["actions_count"] = property.Value.GetArrayLength();
                }
            }

            appendLabel(label);
            // Mengulangi setiap elemen `rowValues`; elemen saat ini disimpan sebagai `rowValue` bertipe `var` untuk diproses oleh badan loop dalam
            // BuildSeriesFromObjectArray.
            foreach (var rowValue in rowValues)
            {
                setSeriesValue(rowValue.Key, rowValue.Value);
            }
        }
    }
}
