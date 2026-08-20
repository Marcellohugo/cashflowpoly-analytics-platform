// Fungsi file: Menyediakan transformasi, lokalisasi, atau koneksi UI melalui PlayerMetricCollectionHelper.
using System.Globalization;

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

    /// <summary>
    /// Mengganti variabel rumus analitik dengan angka pemain yang benar-benar digunakan.
    /// </summary>
    public static string BuildActualCalculation(
        string analysisKey,
        IEnumerable<(string Path, string Value)> rows,
        string resultValue,
        string resultUnit,
        string unavailableText,
        CultureInfo culture)
    {
        var values = rows
            .Where(row => !string.IsNullOrWhiteSpace(row.Path))
            .GroupBy(row => row.Path, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.First().Value, StringComparer.OrdinalIgnoreCase);

        string Value(string path, double scale = 1)
        {
            if (!values.TryGetValue(path, out var rawValue) ||
                !double.TryParse(rawValue, NumberStyles.Float, CultureInfo.InvariantCulture, out var numericValue))
            {
                return unavailableText;
            }

            return FormatCalculationNumber(numericValue * scale, culture);
        }

        var result = string.IsNullOrWhiteSpace(resultUnit) ||
                     string.Equals(resultValue, unavailableText, StringComparison.OrdinalIgnoreCase)
            ? resultValue
            : resultUnit == "%"
                ? $"{resultValue}{resultUnit}"
                : $"{resultValue} {resultUnit}";
        var incomeShares = values
            .Where(item => item.Key.StartsWith("Income_Share_i.", StringComparison.OrdinalIgnoreCase))
            .OrderBy(item => item.Key, StringComparer.OrdinalIgnoreCase)
            .Select(item => double.TryParse(item.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var share)
                ? $"({FormatCalculationNumber(share * 100, culture)} ÷ 100)²"
                : $"({unavailableText} ÷ 100)²")
            .ToList();

        var expression = analysisKey switch
        {
            "net-worth" => $"{Value("coins_held_current")} ÷ {Value("starting_coins")} × 100%",
            "income-diversification" =>
                $"[1 − ({(incomeShares.Count > 0 ? string.Join(" + ", incomeShares) : unavailableText)})] ÷ [1 − (1 ÷ {Value("N_active_income_sources")})] × 100%",
            "expense-efficiency" => $"{Value("essential_expenses")} ÷ {Value("total_expenses")} × 100%",
            "business-margin" =>
                $"({Value("meal_order_income_total")} − {Value("ingredient_investment_coins_total")}) ÷ {Value("meal_order_income_total")} × 100%",
            "risk-appetite" =>
                $"min(100, max(0, ({Value("risk_acceptance_rate", 100)} ÷ 100) × ({Value("Risk_Cost_Intensity", 100)} ÷ 100) × 100))",
            "debt-discipline" =>
                $"{Value("sharia_loans_outstanding_coins")} ÷ {Value("coins_held_current")} × 100%",
            "goal-ambition" =>
                $"min(100, max(0, ({Value("Goal_Attempt_Rate", 100)} + {Value("Goal_Investment_Rate", 100)}) ÷ 2))",
            "action-efficiency" =>
                $"{Value("income_producing_actions")} ÷ {Value("all_player_actions")} × 100%",
            "meal-success" =>
                $"{Value("meal_orders_claimed")} ÷ ({Value("meal_orders_claimed")} + {Value("meal_orders_available_passed")}) × 100%",
            "planning-horizon" =>
                $"({Value("savings_actions")} + {Value("financial_goal_actions")} + {Value("insurance_premium_actions")}) ÷ {Value("all_player_actions")} × 100%",
            "fulfillment-diversity" =>
                $"[1 − (({Value("p_primary", 100)} ÷ 100)² + ({Value("p_secondary", 100)} ÷ 100)² + ({Value("p_tertiary", 100)} ÷ 100)²)] ÷ (1 − ⅓) × 100%",
            "donation-commitment" =>
                $"min(100, max(0, {Value("donation_stability_index")} × ({Value("donation_ratio", 100)} ÷ 100) × ({Value("friday_participation_rate", 100)} ÷ 100)))",
            "happiness-portfolio" => BuildSignedSum(
                values,
                [
                    "need_cards_pts",
                    "need_set_bonus_pts",
                    "donations_pts",
                    "gold_pts",
                    "pension_pts",
                    "financial_goals_pts",
                    "mission_bonus_pts",
                    "loan_penalty_pts"
                ],
                unavailableText,
                culture),
            "happiness-portfolio-beginner" => BuildSignedSum(
                values,
                [
                    "need_cards_pts",
                    "need_set_bonus_pts",
                    "donations_pts",
                    "gold_pts",
                    "pension_pts",
                    "mission_bonus_pts"
                ],
                unavailableText,
                culture),
            _ => unavailableText
        };

        return $"{expression} = {result}";
    }

    private static string BuildSignedSum(
        IReadOnlyDictionary<string, string> values,
        IReadOnlyList<string> paths,
        string unavailableText,
        CultureInfo culture)
    {
        var terms = new List<string>();
        foreach (var path in paths)
        {
            if (!values.TryGetValue(path, out var rawValue) ||
                !double.TryParse(rawValue, NumberStyles.Float, CultureInfo.InvariantCulture, out var numericValue))
            {
                terms.Add(terms.Count == 0 ? unavailableText : $"+ {unavailableText}");
                continue;
            }

            var formattedValue = FormatCalculationNumber(Math.Abs(numericValue), culture);
            if (terms.Count == 0)
            {
                terms.Add(numericValue < 0 ? $"−{formattedValue}" : formattedValue);
            }
            else
            {
                terms.Add(numericValue < 0 ? $"− {formattedValue}" : $"+ {formattedValue}");
            }
        }

        return string.Join(" ", terms);
    }

    private static string FormatCalculationNumber(double value, CultureInfo culture)
    {
        return Math.Abs(value - Math.Round(value)) < 0.0000001
            ? value.ToString("N0", culture)
            : value.ToString("N2", culture);
    }
}
