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
{
    /// <summary>
    /// Menggabungkan baris berdasarkan path unik, mempertahankan kemunculan pertama.
    /// </summary>
    // Mendefinisikan metode `MergeUniqueRows` dengan hasil bertipe `List<(string Path, string Value)>`. Menggabungkan baris berdasarkan path unik,
    // mempertahankan kemunculan pertama. Masukan: Parameter `rowSets` bertipe `IEnumerable<(string Path, string Value)>[]` membawa nilai baris sets.
    public static List<(string Path, string Value)> MergeUniqueRows(params IEnumerable<(string Path, string Value)>[] rowSets)
    {
        var result = new List<(string Path, string Value)>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        // Mengulangi setiap elemen `rowSets`; elemen saat ini disimpan sebagai `rowSet` bertipe `var` untuk diproses oleh badan loop dalam MergeUniqueRows.
        foreach (var rowSet in rowSets)
        {
            // Mengulangi setiap elemen `rowSet`; elemen saat ini disimpan sebagai `row` bertipe `var` untuk diproses oleh badan loop dalam MergeUniqueRows.
            foreach (var row in rowSet)
            {
                if (string.IsNullOrWhiteSpace(row.Path))
                {
                    // Melewati sisa pernyataan pada iterasi saat ini dan melanjutkan ke elemen/iterasi berikutnya dalam MergeUniqueRows.
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
    // Mendefinisikan metode `GetGroupRows` dengan hasil bertipe `List<(string Path, string Value)>`. Mengambil baris dari beberapa grup sesuai urutan
    // key yang diminta. Masukan: Parameter `source` bertipe `Dictionary<string, List<(string Path, string Value)>>` membawa nilai source; Parameter
    // `keys` bertipe `string[]` membawa nilai kunci.
    public static List<(string Path, string Value)> GetGroupRows(
        // Parameter `source` bertipe `Dictionary<string, List<(string Path, string Value)>>` membawa nilai source.
        Dictionary<string, List<(string Path, string Value)>> source,
        // Parameter `keys` bertipe `string[]` membawa nilai kunci.
        params string[] keys)
    {
        var rows = new List<(string Path, string Value)>();
        // Mengulangi setiap elemen `keys`; elemen saat ini disimpan sebagai `key` bertipe `var` untuk diproses oleh badan loop dalam GetGroupRows.
        foreach (var key in keys)
        {
            if (!source.TryGetValue(key, out var groupRows))
            {
                // Melewati sisa pernyataan pada iterasi saat ini dan melanjutkan ke elemen/iterasi berikutnya dalam GetGroupRows.
                continue;
            }

            rows.AddRange(groupRows);
        }

        return rows;
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
    {
        return rows
            .Where(row => keywords.Any(keyword => row.Path.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
            .ToList();
    }

    /// <summary>
    /// Menggabungkan chart berdasarkan title unik, mempertahankan kemunculan pertama.
    /// </summary>
    // Mendefinisikan metode `MergeUniqueCharts` dengan hasil bertipe `List<(string Title, string Json)>`. Menggabungkan chart berdasarkan title unik,
    // mempertahankan kemunculan pertama. Masukan: Parameter `chartSets` bertipe `IEnumerable<(string Title, string Json)>[]` membawa nilai chart sets.
    public static List<(string Title, string Json)> MergeUniqueCharts(params IEnumerable<(string Title, string Json)>[] chartSets)
    {
        var result = new List<(string Title, string Json)>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        // Mengulangi setiap elemen `chartSets`; elemen saat ini disimpan sebagai `chartSet` bertipe `var` untuk diproses oleh badan loop dalam
        // MergeUniqueCharts.
        foreach (var chartSet in chartSets)
        {
            // Mengulangi setiap elemen `chartSet`; elemen saat ini disimpan sebagai `chart` bertipe `var` untuk diproses oleh badan loop dalam
            // MergeUniqueCharts.
            foreach (var chart in chartSet)
            {
                if (string.IsNullOrWhiteSpace(chart.Title))
                {
                    // Melewati sisa pernyataan pada iterasi saat ini dan melanjutkan ke elemen/iterasi berikutnya dalam MergeUniqueCharts.
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
    // Mendefinisikan metode `FilterChartsByKeywords` dengan hasil bertipe `List<(string Title, string Json)>`. Memfilter chart yang title-nya
    // mengandung salah satu keyword. Masukan: Parameter `charts` bertipe `IEnumerable<(string Title, string Json)>` membawa nilai charts; Parameter
    // `keywords` bertipe `string[]` membawa nilai keywords.
    public static List<(string Title, string Json)> FilterChartsByKeywords(
        // Parameter `charts` bertipe `IEnumerable<(string Title, string Json)>` membawa nilai charts.
        IEnumerable<(string Title, string Json)> charts,
        // Parameter `keywords` bertipe `string[]` membawa nilai keywords.
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
    // Mendefinisikan metode `GetGroupCharts` dengan hasil bertipe `List<(string Title, string Json)>`. Mengambil chart dari beberapa grup sesuai urutan
    // key yang diminta. Masukan: Parameter `source` bertipe `Dictionary<string, List<(string Title, string Json)>>` membawa nilai source; Parameter
    // `keys` bertipe `string[]` membawa nilai kunci.
    public static List<(string Title, string Json)> GetGroupCharts(
        // Parameter `source` bertipe `Dictionary<string, List<(string Title, string Json)>>` membawa nilai source.
        Dictionary<string, List<(string Title, string Json)>> source,
        // Parameter `keys` bertipe `string[]` membawa nilai kunci.
        params string[] keys)
    {
        var charts = new List<(string Title, string Json)>();
        // Mengulangi setiap elemen `keys`; elemen saat ini disimpan sebagai `key` bertipe `var` untuk diproses oleh badan loop dalam GetGroupCharts.
        foreach (var key in keys)
        {
            if (!source.TryGetValue(key, out var groupCharts))
            {
                // Melewati sisa pernyataan pada iterasi saat ini dan melanjutkan ke elemen/iterasi berikutnya dalam GetGroupCharts.
                continue;
            }

            charts.AddRange(groupCharts);
        }

        return charts;
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
    {
        var values = rows
            .Where(row => !string.IsNullOrWhiteSpace(row.Path))
            .GroupBy(row => row.Path, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.First().Value, StringComparer.OrdinalIgnoreCase);

        if (string.Equals(resultValue, unavailableText, StringComparison.OrdinalIgnoreCase))
        {
            return unavailableText;
        }

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
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: resultValue dalam BuildActualCalculation.
            ? resultValue
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: resultUnit == ”%” dalam BuildActualCalculation.
            : resultUnit == "%"
                // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: $”{resultValue}{resultUnit}” dalam BuildActualCalculation.
                ? $"{resultValue}{resultUnit}"
                // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: $”{resultValue} {resultUnit}”; dalam BuildActualCalculation.
                : $"{resultValue} {resultUnit}";

        if (analysisKey == "income-diversification" &&
            values.TryGetValue("active_income_source_count", out var sourceCount) &&
            int.TryParse(sourceCount, out var count) && count == 1)
            return result;

        var incomeShares = values
            .Where(item => item.Key.StartsWith("income_shares.", StringComparison.OrdinalIgnoreCase))
            .OrderBy(item => item.Key, StringComparer.OrdinalIgnoreCase)
            .Select(item => double.TryParse(item.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var share)
                ? $"({FormatCalculationNumber(share * 100, culture)} ÷ 100)²"
                : $"({unavailableText} ÷ 100)²")
            .ToList();

        var expression = analysisKey switch
        {
            // Untuk pola `”net-worth”`, menghasilkan teks interpolasi `$”{Value(”coins_net_end_game”)} ÷ {Value(”starting_coins”)} × 100%”`; nilai ekspresi di
            // dalam kurung kurawal disisipkan saat program berjalan sebagai hasil switch.
            "net-worth" => $"({Value("coins_net_end_game")} − {Value("starting_coins")}) ÷ {Value("starting_coins")} × 100%",
            // Untuk pola `”income-diversification”`, menghasilkan teks interpolasi `$”[1 − ({(incomeShares.Count > 0 ? string.Join(” + ”, incomeShares) :
            // unavailableText)})] ÷ [1 − (1 ÷ {Value(”active_income_source_count”)})] × 100%”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program
            // berjalan sebagai hasil switch.
            "income-diversification" =>
                $"[1 − ({(incomeShares.Count > 0 ? string.Join(" + ", incomeShares) : unavailableText)})] ÷ [1 − (1 ÷ {Value("active_income_source_count")})] × 100%",
            // Untuk pola `”expense-efficiency”`, menghasilkan teks interpolasi `$”{Value(”ingredient_investment_coins_total”)} ÷ {Value(”total_cash_out”)} ×
            // 100%”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan sebagai hasil switch.
            "expense-efficiency" => $"{Value("ingredient_investment_coins_total")} ÷ {Value("total_cash_out")} × 100%",
            // Untuk pola `”business-margin”`, menghasilkan teks interpolasi `$”({Value(”meal_order_income_total”)} − {Value(”ingredient_cost_used”)}) ÷
            // {Value(”meal_order_income_total”)} × 100%”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan sebagai hasil switch.
            "business-margin" =>
                $"({Value("meal_order_income_total")} − {Value("ingredient_cost_used")}) ÷ {Value("meal_order_income_total")} × 100%",
            // Untuk pola `”risk-appetite”`, menghasilkan teks interpolasi `$”{Value(”risks_resolved_without_emergency”)} ÷ {Value(”life_risk_cards_drawn”)} ×
            // 100%”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan sebagai hasil switch.
            "risk-appetite" =>
                $"{Value("risks_resolved_without_emergency")} ÷ {Value("life_risk_cards_drawn")} × 100%",
            // Untuk pola `”debt-discipline”`, menghasilkan teks interpolasi `$”{Value(”outstanding_loan”)} ÷ ({Value(”outstanding_loan”)} +
            // {Value(”liquid_assets”)}) × 100%”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan sebagai hasil switch.
            "debt-discipline" =>
                $"{Value("sharia_loans_repaid")} + {Value("sharia_loans_unpaid_end")}",
            "goal-ambition" =>
                $"{Value("financial_goals_completed")} ÷ {Value("financial_goals_available_total")} × 100%",
            "goal-purchases" => Value("financial_goals_completed"),
            // Untuk pola `”goal-ambition”`, menghasilkan teks interpolasi `$”{Value(”coins_committed_to_goals”)} ÷ {Value(”attempted_goal_target_total”)} ×
            // 100%”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan sebagai hasil switch.
            // Untuk pola `”action-efficiency”`, menghasilkan teks interpolasi `$”{Value(”income_main_actions”)} ÷ {Value(”total_main_actions”)} × 100%”`; nilai
            // ekspresi di dalam kurung kurawal disisipkan saat program berjalan sebagai hasil switch.
            "action-efficiency" =>
                $"{Value("income_main_actions")} ÷ {Value("total_main_actions")} × 100%",
            // Untuk pola `”meal-success”`, menghasilkan teks interpolasi `$”{Value(”ingredients_used_in_completed_orders”)} ÷ {Value(”ingredients_collected”)}
            // × 100%”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan sebagai hasil switch.
            "meal-success" =>
                $"{Value("ingredients_used_in_completed_orders")} ÷ {Value("ingredients_collected")} × 100%",
            // Untuk pola `”planning-horizon”`, menghasilkan teks interpolasi `$”({Value(”saving_actions”)} + {Value(”financial_goal_actions”)} +
            // {Value(”insurance_actions”)} + {Value(”loan_repayment_actions”)}) ÷ {Value(”total_main_actions”)} × 100%”`; nilai ekspresi di dalam kurung
            // kurawal disisipkan saat program berjalan sebagai hasil switch.
            "planning-horizon" =>
                $"({Value("saving_and_goal_actions")} + {Value("insurance_actions")} + {Value("loan_repayment_actions")}) ÷ {Value("total_main_actions")} × 100%",
            // Untuk pola `”fulfillment-diversity”`, menghasilkan teks interpolasi `$”[1 − ({Value(”primary_need_share”)}² + {Value(”secondary_need_share”)}² +
            // {Value(”tertiary_need_share”)}²)] ÷ (1 − ⅓) × 100%”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan sebagai hasil
            // switch.
            "fulfillment-diversity" =>
                $"[1 − ({Value("primary_need_share")}² + {Value("secondary_need_share")}² + {Value("tertiary_need_share")}²)] ÷ (1 − ⅓) × 100%",
            // Untuk pola `”donation-commitment”`, menghasilkan teks interpolasi `$”min(100, max(0, {Value(”donation_stability_index”)} ×
            // {Value(”donated_resource_share”)} × {Value(”friday_participation_rate”)}))”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program
            // berjalan sebagai hasil switch.
            "donation-commitment" =>
                $"({Value("donation_stability_index")} ÷ 100) × ({Value("donated_resource_share", 100)} ÷ 100) × ({Value("friday_participation_rate", 100)} ÷ 100) × 100%",
            "happiness-portfolio" or "happiness-portfolio-beginner" => BuildHappinessDiversityCalculation(
                values, analysisKey == "happiness-portfolio", unavailableText, culture),
            _ => unavailableText
        };

        return $"{expression} = {result}";
    }

    private static string BuildHappinessDiversityCalculation(
        IReadOnlyDictionary<string, string> values, bool advancedMode, string unavailableText, CultureInfo culture)
    {
        var paths = new List<string>
        {
            "need_card_points", "need_set_bonus_points", "donation_points", "gold_points", "pension_points"
        };
        if (advancedMode) paths.Add("financial_goal_points");
        foreach (var path in new[] { "initial_happiness_points", "mission_reward_points" })
            if (values.TryGetValue(path, out var additionalPoints) &&
                double.TryParse(additionalPoints, NumberStyles.Float, CultureInfo.InvariantCulture, out var amount) && amount > 0)
                paths.Add(path);
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

    private static string FormatCalculationNumber(double value, CultureInfo culture)
    {
        return Math.Abs(value - Math.Round(value)) < 0.0000001
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: value.ToString(”N0”, culture) dalam FormatCalculationNumber.
            ? value.ToString("N0", culture)
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: value.ToString(”0.######”, culture); dalam FormatCalculationNumber.
            : value.ToString("0.######", culture);
    }
}
