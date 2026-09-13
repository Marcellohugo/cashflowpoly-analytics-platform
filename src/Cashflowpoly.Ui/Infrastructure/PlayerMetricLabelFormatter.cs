// Fungsi file: Menyediakan transformasi, lokalisasi, atau koneksi UI melalui PlayerMetricLabelFormatter.
// Mengimpor namespace `System.Globalization` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Globalization;
// Mengimpor namespace `System.Text` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Infrastructure` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Infrastructure;

/// <summary>
/// Teks siap tampil untuk satu variabel pendukung pemain.
/// </summary>
// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `PlayerMetricPresentation`; sealed mencegah tipe ini diturunkan lagi.
public sealed record PlayerMetricPresentation(
    // Parameter `Label` bertipe `string` membawa nilai label.
    string Label,
    // Parameter `DisplayValue` bertipe `string` membawa nilai display nilai.
    string DisplayValue,
    // Parameter `Unit` bertipe `string` membawa nilai unit.
    string Unit,
    // Parameter `Explanation` bertipe `string` membawa nilai explanation.
    string Explanation,
    // Parameter `Guidance` bertipe `string` membawa nilai guidance.
    string Guidance,
    // Parameter `Recommendation` bertipe `string` membawa nilai recommendation.
    string Recommendation,
    // Parameter `Formula` bertipe `string` membawa nilai formula.
    string Formula,
    // Parameter `IsAdvancedOnly` bertipe `bool` membawa nilai berstatus advanced only.
    bool IsAdvancedOnly,
    // Parameter `State` bertipe `string` membawa keadaan permainan yang menjadi sumber atau hasil pembaruan.
    string State);

/// <summary>
/// Formatter label metrik pemain yang tidak bergantung pada Razor runtime.
/// </summary>
// Mendefinisikan tipe class `PlayerMetricLabelFormatter`.
public static class PlayerMetricLabelFormatter
{
    private static readonly HashSet<string> PercentageMetricKeys = new(StringComparer.OrdinalIgnoreCase)
    {
        "cash_growth_percent",
        "happiness_source_diversity_percent",
        "business_expense_share_percent",
        "meal_order_profit_margin_percent",
        "risk_readiness_percent",
        "loan_burden_percent",
        "financial_goal_completion_percent",
        "donation_commitment_score",
        "financial_goal_progress_percent",
        "income_action_focus_percent",
        "ingredient_utilization_percent",
        "long_term_action_share_percent",
        "need_fulfillment_diversity_percent",
        "net_worth_index",
        "income_diversification_index",
        "income_diversification_ratio",
        "expense_management_efficiency",
        "business_profit_margin",
        "gold_roi_percentage",
        "risk_exposure_percentage",
        "risk_mitigation_effectiveness",
        "debt_leverage_ratio",
        "loan_repayment_discipline",
        "goal_ambition",
        "goal_ambition_index",
        "action_efficiency_percent",
        "meal_order_success_rate",
        "planning_horizon_percent",
        "donation_aggressiveness_percent",
        "donation_stability",
        "donation_stability_index",
        "risk_appetite_score_normalized"
    };

    private static readonly HashSet<string> FractionPercentageMetricKeys = new(StringComparer.OrdinalIgnoreCase)
    {
        "risk_acceptance_rate",
        "insurance_activation_rate",
        "insurance_coverage_rate",
        "risk_cost_intensity",
        "debt_ratio",
        "goal_attempt_rate",
        "goal_investment_rate",
        "action_efficiency",
        "diversity_score",
        "action_diversity_score_avg",
        "planning_horizon",
        "fulfillment_diversity",
        "fulfillment_diversity_document_formula",
        "p_primary",
        "p_secondary",
        "p_tertiary",
        "donation_ratio",
        "friday_participation_rate",
        "income_share_i",
        "income_shares",
        "primary_need_share",
        "secondary_need_share",
        "tertiary_need_share",
        "donated_resource_share"
    };

    private static readonly HashSet<string> MultiplierMetricKeys = new(StringComparer.OrdinalIgnoreCase)
    {
        "business_efficiency_ratio",
        "growth_pattern_ratio"
    };

    private static readonly string[] AdvancedMetricFragments =
    {
        "life_risk",
        "insurance",
        "emergency",
        "financial_goal",
        "sharia_loan",
        "loan_",
        "risk_",
        "debt_",
        "goal_",
        "planning_horizon",
        "long_term"
    };

    /// <summary>
    /// Mengubah key metrik menjadi label manusiawi dengan dukungan lokalisasi.
    /// </summary>
    // Mendefinisikan metode `HumanizeMetricKey` dengan hasil bertipe `string`. Mengubah key metrik menjadi label manusiawi dengan dukungan lokalisasi.
    // Masukan: Parameter `key` bertipe `string` membawa nilai kunci; Parameter `translate` bertipe `Func<string, string>` membawa nilai translate.
    public static string HumanizeMetricKey(string key, Func<string, string> translate)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return translate("players.details.metric_fallback");
        }

        if (string.Equals(key, "value", StringComparison.OrdinalIgnoreCase))
        {
            return translate("common.value");
        }

        if (string.Equals(key, "series", StringComparison.OrdinalIgnoreCase))
        {
            return translate("players.details.series");
        }

        var normalized = NormalizeMetricLexiconKey(key);
        var aliasLexiconKey = normalized switch
        {
            // Untuk pola `”coins_net_end”`, menghasilkan nilai literal `”players.raw.coins_net_end_game”` sebagai hasil switch.
            "coins_net_end" => "players.raw.coins_net_end_game",
            // Untuk pola `”coins_net_end_game”`, menghasilkan nilai literal `”players.raw.coins_net_end_game”` sebagai hasil switch.
            "coins_net_end_game" => "players.raw.coins_net_end_game",
            // Untuk pola `_`, menghasilkan `string.Empty`, yaitu nilai kosong bawaan tipe terkait sebagai hasil switch.
            _ => string.Empty
        };

        var localizedLabel =
            TryTranslateMetricLexicon(aliasLexiconKey, translate) ??
            TryTranslateMetricLexicon($"players.raw.{normalized}", translate) ??
            TryTranslateMetricLexicon($"players.group.{normalized}", translate) ??
            TryTranslateMetricLexicon($"players.metric.{normalized}", translate);
        if (!string.IsNullOrWhiteSpace(localizedLabel))
        {
            return localizedLabel;
        }

        var text = normalized.Replace('_', ' ').Trim();
        if (text.Length == 0)
        {
            return translate("players.details.metric_fallback");
        }

        return char.ToUpperInvariant(text[0]) + text[1..];
    }

    /// <summary>
    /// Mengubah path metrik bertingkat menjadi label bersegmen.
    /// </summary>
    // Mendefinisikan metode `FormatMetricPathLabel` dengan hasil bertipe `string`. Mengubah path metrik bertingkat menjadi label bersegmen. Masukan:
    // Parameter `rawPath` bertipe `string` membawa nilai raw path; Parameter `translate` bertipe `Func<string, string>` membawa nilai translate.
    public static string FormatMetricPathLabel(string rawPath, Func<string, string> translate)
    {
        if (string.IsNullOrWhiteSpace(rawPath))
        {
            return translate("players.details.metric_fallback");
        }

        var tokens = new List<string>();
        var buffer = new StringBuilder();

        void FlushBuffer()
        {
            if (buffer.Length == 0)
            {
                return;
            }

            tokens.Add(HumanizeMetricKey(buffer.ToString(), translate));
            buffer.Clear();
        }

        for (var index = 0; index < rawPath.Length; index++)
        {
            var current = rawPath[index];
            if (current == '.')
            {
                FlushBuffer();
                // Melewati sisa pernyataan pada iterasi saat ini dan melanjutkan ke elemen/iterasi berikutnya dalam FormatMetricPathLabel.
                continue;
            }

            if (current == '[')
            {
                FlushBuffer();

                var closeIndex = rawPath.IndexOf(']', index);
                if (closeIndex > index)
                {
                    var indexToken = rawPath.Substring(index + 1, closeIndex - index - 1);
                    if (int.TryParse(indexToken, out var parsedIndex))
                    {
                        tokens.Add($"{translate("players.details.item")} {parsedIndex + 1}");
                    }
                    else if (!string.IsNullOrWhiteSpace(indexToken))
                    {
                        tokens.Add($"{translate("players.details.item")} {indexToken}");
                    }

                    index = closeIndex;
                    // Melewati sisa pernyataan pada iterasi saat ini dan melanjutkan ke elemen/iterasi berikutnya dalam FormatMetricPathLabel.
                    continue;
                }
            }

            buffer.Append(current);
        }

        FlushBuffer();

        var displayTokens = tokens
            .Where(token => !string.IsNullOrWhiteSpace(token))
            .ToList();

        if (displayTokens.Count == 0)
        {
            return HumanizeMetricKey(rawPath, translate);
        }

        return string.Join(" - ", displayTokens);
    }

    /// <summary>
    /// Menambahkan satuan, arti, cara baca, dan status mode pada nilai metrik mentah maupun turunan.
    /// </summary>
    // Mendefinisikan metode `DescribeMetric` dengan hasil bertipe `PlayerMetricPresentation`. Menambahkan satuan, arti, cara baca, dan status mode pada
    // nilai metrik mentah maupun turunan. Masukan: Parameter `rawPath` bertipe `string` membawa nilai raw path; Parameter `rawValue` bertipe `string`
    // membawa nilai raw nilai; Parameter `isDerived` bertipe `bool` membawa nilai berstatus derived; Parameter `isAdvancedMode` bertipe `bool` membawa
    // nilai berstatus advanced mode; Parameter `nullText` bertipe `string` membawa nilai null text; Parameter `translate` bertipe `Func<string,
    // string>` membawa nilai translate.
    public static PlayerMetricPresentation DescribeMetric(
        // Parameter `rawPath` bertipe `string` membawa nilai raw path.
        string rawPath,
        // Parameter `rawValue` bertipe `string` membawa nilai raw nilai.
        string rawValue,
        // Parameter `isDerived` bertipe `bool` membawa nilai berstatus derived.
        bool isDerived,
        // Parameter `isAdvancedMode` bertipe `bool` membawa nilai berstatus advanced mode.
        bool isAdvancedMode,
        // Parameter `nullText` bertipe `string` membawa nilai null text.
        string nullText,
        // Parameter `translate` bertipe `Func<string, string>` membawa nilai translate.
        Func<string, string> translate)
    {
        var label = FormatMetricPathLabel(rawPath, translate);
        var metricKey = GetRootMetricKey(rawPath);
        var leafMetricKey = GetLeafMetricKey(rawPath);
        var normalizedPath = NormalizeMetricLexiconKey(rawPath);
        var isAdvancedOnly = AdvancedMetricFragments.Any(fragment =>
            normalizedPath.Contains(fragment, StringComparison.OrdinalIgnoreCase));
        var isUnavailable = string.IsNullOrWhiteSpace(rawValue) ||
                            string.Equals(rawValue, nullText, StringComparison.OrdinalIgnoreCase) ||
                            rawValue is "[]" or "{}";

        var unitKey = ResolveUnitKey(metricKey, leafMetricKey, normalizedPath);
        var unit = string.IsNullOrWhiteSpace(unitKey) ? string.Empty : translate(unitKey);
        var hasNumericValue = TryParseMetricNumber(rawValue, out var numericValue);
        var isCashGrowthMetric = metricKey is "cash_growth_percent" or "net_worth_index";
        if (hasNumericValue && isCashGrowthMetric)
        {
            // Snapshot menyimpan perbandingan saldo (100 = koin awal); tampilkan perubahan dari awal.
            numericValue -= 100;
        }
        var noLoanRecorded = metricKey == "day_when_debt_introduced" &&
                             isUnavailable &&
                             isAdvancedMode;
        var isGameDayMetric = IsGameDayMetric(metricKey);
        var state = isAdvancedOnly && !isAdvancedMode
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: ”not_applicable” dalam DescribeMetric.
            ? "not_applicable"
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: noLoanRecorded dalam DescribeMetric.
            : noLoanRecorded
                // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: ”recorded” dalam DescribeMetric.
                ? "recorded"
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: isUnavailable dalam DescribeMetric.
            : isUnavailable
                // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: ”unavailable” dalam DescribeMetric.
                ? "unavailable"
                // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: hasNumericValue && isGameDayMetric dalam DescribeMetric.
                : hasNumericValue && isGameDayMetric
                    // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: ”recorded” dalam DescribeMetric.
                    ? "recorded"
                // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: hasNumericValue && Math.Abs(numericValue) < 0.0000001 dalam
                // DescribeMetric.
                : hasNumericValue && Math.Abs(numericValue) < 0.0000001
                    // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: ”zero” dalam DescribeMetric.
                    ? "zero"
                    // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: ”recorded”; dalam DescribeMetric.
                    : "recorded";

        var displayNumericValue = FractionPercentageMetricKeys.Contains(metricKey) ||
                                  FractionPercentageMetricKeys.Contains(leafMetricKey)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: numericValue * 100 dalam DescribeMetric.
            ? numericValue * 100
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: numericValue; dalam DescribeMetric.
            : numericValue;
        var displayValue = state is "not_applicable" or "unavailable"
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: translate(”players.support.value.unavailable”) dalam DescribeMetric.
            ? translate("players.support.value.unavailable")
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: hasNumericValue dalam DescribeMetric.
            : hasNumericValue
                // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: FormatDisplayNumber(displayNumericValue) dalam DescribeMetric.
                ? isCashGrowthMetric
                    ? displayNumericValue.ToString("+0.##;-0.##;0", CultureInfo.CurrentCulture)
                    : FormatDisplayNumber(displayNumericValue)
                // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: rawValue; dalam DescribeMetric.
                : rawValue;
        if (noLoanRecorded)
        {
            displayValue = translate("players.support.value.no_loan_recorded");
            unit = string.Empty;
        }
        else if (hasNumericValue && isGameDayMetric)
        {
            displayValue = numericValue <= 0
                // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: translate(metricKey == ”day_when_debt_introduced” dalam
                // DescribeMetric.
                ? translate(metricKey == "day_when_debt_introduced"
                    ? "players.support.value.preparation_loan"
                    : "players.support.value.preparation")
                // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: string.Format( dalam DescribeMetric.
                : string.Format(
                    CultureInfo.CurrentCulture,
                    translate("players.support.value.day_index"),
                    FormatDisplayNumber(numericValue));
            unit = string.Empty;
        }
        else if (metricKey == "pension_fund_rank_per_game" && hasNumericValue && numericValue > 0)
        {
            displayValue = string.Format(
                CultureInfo.CurrentCulture,
                translate("players.support.value.rank_position"),
                FormatDisplayNumber(numericValue));
            unit = string.Empty;
        }
        if (state is "not_applicable" or "unavailable")
        {
            unit = string.Empty;
        }

        var explanationKey = ResolveExplanationKey(metricKey, isDerived, unitKey);
        var explanation = string.Format(
            CultureInfo.CurrentCulture,
            translate(explanationKey),
            label);
        if (metricKey == "need_cards_owned_current" && isAdvancedMode)
        {
            explanation += " " + translate("players.support.meaning.need_cards_sold_note");
        }
        var guidanceKey = state switch
        {
            // Untuk pola `”not_applicable”`, menghasilkan nilai literal `”players.support.guide.not_applicable”` sebagai hasil switch.
            "not_applicable" => "players.support.guide.not_applicable",
            // Untuk pola `”unavailable”`, menghasilkan nilai literal `”players.support.guide.unavailable”` sebagai hasil switch.
            "unavailable" => "players.support.guide.unavailable",
            // Untuk pola `_`, menghasilkan memanggil `ResolveGuidanceKey` dengan `metricKey`, `numericValue`, `hasNumericValue`, `isDerived`, `unitKey` sebagai
            // hasil switch.
            _ => ResolveGuidanceKey(metricKey, numericValue, hasNumericValue, isDerived, unitKey)
        };
        var recommendationKey = state switch
        {
            // Untuk pola `”not_applicable”`, menghasilkan nilai literal `”players.support.recommendation.not_applicable”` sebagai hasil switch.
            "not_applicable" => "players.support.recommendation.not_applicable",
            // Untuk pola `”unavailable”`, menghasilkan nilai literal `”players.support.recommendation.unavailable”` sebagai hasil switch.
            "unavailable" => "players.support.recommendation.unavailable",
            // Untuk pola `_`, menghasilkan memanggil `ResolveRecommendationKey` dengan `metricKey`, `numericValue`, `hasNumericValue` sebagai hasil switch.
            _ => ResolveRecommendationKey(metricKey, numericValue, hasNumericValue)
        };
        var formulaKey = isDerived ? ResolveFormulaKey(metricKey) : string.Empty;

        return new PlayerMetricPresentation(
            label,
            displayValue,
            unit,
            explanation,
            translate(guidanceKey),
            translate(recommendationKey),
            string.IsNullOrWhiteSpace(formulaKey) ? string.Empty : translate(formulaKey),
            isAdvancedOnly,
            state);
    }

    /// <summary>
    /// Mengubah kategori transaksi menjadi label lokal.
    /// </summary>
    // Mendefinisikan metode `HumanizeTransactionCategory` dengan hasil bertipe `string`. Mengubah kategori transaksi menjadi label lokal. Masukan:
    // Parameter `category` bertipe `string` membawa nilai category; Parameter `translate` bertipe `Func<string, string>` membawa nilai translate.
    public static string HumanizeTransactionCategory(string category, Func<string, string> translate)
    {
        if (string.IsNullOrWhiteSpace(category))
        {
            return translate("players.details.transaction_label");
        }

        var normalized = category.Trim().ToUpperInvariant();
        var mapped = normalized switch
        {
            // Untuk pola `”DONATION”`, menghasilkan memanggil `translate` dengan `”players.details.transaction.category.donation”` sebagai hasil switch.
            "DONATION" => translate("players.details.transaction.category.donation"),
            // Untuk pola `”GOLD_TRADE”`, menghasilkan memanggil `translate` dengan `”players.details.transaction.category.gold_trade”` sebagai hasil switch.
            "GOLD_TRADE" => translate("players.details.transaction.category.gold_trade"),
            // Untuk pola `”INGREDIENT”`, menghasilkan memanggil `translate` dengan `”players.details.transaction.category.ingredient”` sebagai hasil switch.
            "INGREDIENT" => translate("players.details.transaction.category.ingredient"),
            "INITIAL_INGREDIENT" => translate("players.details.transaction.category.initial_ingredient"),
            // Untuk pola `”ORDER”`, menghasilkan memanggil `translate` dengan `”players.details.transaction.category.order”` sebagai hasil switch.
            "ORDER" => translate("players.details.transaction.category.order"),
            // Untuk pola `”FREELANCE”`, menghasilkan memanggil `translate` dengan `”players.details.transaction.category.freelance”` sebagai hasil switch.
            "FREELANCE" => translate("players.details.transaction.category.freelance"),
            // Untuk pola `”NEED_PRIMARY”`, menghasilkan memanggil `translate` dengan `”players.details.transaction.category.need_primary”` sebagai hasil
            // switch.
            "NEED_PRIMARY" => translate("players.details.transaction.category.need_primary"),
            // Untuk pola `”NEED_SECONDARY”`, menghasilkan memanggil `translate` dengan `”players.details.transaction.category.need_secondary”` sebagai hasil
            // switch.
            "NEED_SECONDARY" => translate("players.details.transaction.category.need_secondary"),
            // Untuk pola `”NEED_TERTIARY”`, menghasilkan memanggil `translate` dengan `”players.details.transaction.category.need_tertiary”` sebagai hasil
            // switch.
            "NEED_TERTIARY" => translate("players.details.transaction.category.need_tertiary"),
            // Untuk pola `”SAVING_DEPOSIT”`, menghasilkan memanggil `translate` dengan `”players.details.transaction.category.saving_deposit”` sebagai hasil
            // switch.
            "SAVING_DEPOSIT" => translate("players.details.transaction.category.saving_deposit"),
            // Untuk pola `”SAVING_WITHDRAW”`, menghasilkan memanggil `translate` dengan `”players.details.transaction.category.saving_withdraw”` sebagai hasil
            // switch.
            "SAVING_WITHDRAW" => translate("players.details.transaction.category.saving_withdraw"),
            // Untuk pola `”RISK_LIFE”`, menghasilkan memanggil `translate` dengan `”players.details.transaction.category.risk_life”` sebagai hasil switch.
            "RISK_LIFE" => translate("players.details.transaction.category.risk_life"),
            // Untuk pola `”LOAN_TAKEN”`, menghasilkan memanggil `translate` dengan `”players.details.transaction.category.loan_taken”` sebagai hasil switch.
            "LOAN_TAKEN" => translate("players.details.transaction.category.loan_taken"),
            // Untuk pola `”LOAN_REPAID”`, menghasilkan memanggil `translate` dengan `”players.details.transaction.category.loan_repaid”` sebagai hasil switch.
            "LOAN_REPAID" => translate("players.details.transaction.category.loan_repaid"),
            // Untuk pola `”INSURANCE_PREMIUM”`, menghasilkan memanggil `translate` dengan `”players.details.transaction.category.insurance_premium”` sebagai
            // hasil switch.
            "INSURANCE_PREMIUM" => translate("players.details.transaction.category.insurance_premium"),
            // Untuk pola `”INSURANCE_OFFSET”`, menghasilkan memanggil `translate` dengan `”players.details.transaction.category.insurance_claim”` sebagai hasil
            // switch.
            "INSURANCE_OFFSET" => translate("players.details.transaction.category.insurance_claim"),
            // Untuk pola `”EMERGENCY_OPTION”`, menghasilkan memanggil `translate` dengan `”players.details.transaction.category.emergency_option”` sebagai
            // hasil switch.
            "EMERGENCY_OPTION" => translate("players.details.transaction.category.emergency_option"),
            // Untuk pola `_`, menghasilkan `string.Empty`, yaitu nilai kosong bawaan tipe terkait sebagai hasil switch.
            _ => string.Empty
        };

        if (!string.IsNullOrWhiteSpace(mapped))
        {
            return mapped;
        }

        return HumanizeMetricKey(category.Trim().ToLowerInvariant(), translate);
    }

    /// <summary>
    /// Mencoba membaca teks metrik menjadi angka untuk chart.
    /// </summary>
    // Mendefinisikan metode `TryParseMetricNumber` dengan hasil bertipe `bool`. Mencoba membaca teks metrik menjadi angka untuk chart. Masukan:
    // Parameter `rawValue` bertipe `string` membawa nilai raw nilai; Parameter `numericValue` bertipe `double` membawa nilai numerik nilai; out
    // mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    public static bool TryParseMetricNumber(string rawValue, out double numericValue)
    {
        if (string.IsNullOrWhiteSpace(rawValue))
        {
            numericValue = 0;
            return false;
        }

        if (string.Equals(rawValue, "true", StringComparison.OrdinalIgnoreCase))
        {
            numericValue = 1;
            return true;
        }

        if (string.Equals(rawValue, "false", StringComparison.OrdinalIgnoreCase))
        {
            numericValue = 0;
            return true;
        }

        if (double.TryParse(rawValue, NumberStyles.Float, CultureInfo.InvariantCulture, out numericValue))
        {
            return true;
        }

        if (double.TryParse(rawValue, NumberStyles.Float, CultureInfo.CurrentCulture, out numericValue))
        {
            return true;
        }

        numericValue = 0;
        return false;
    }

    /// <summary>
    /// Mengklasifikasikan path scalar compact untuk ringkasan gabungan utama.
    /// </summary>
    // Mendefinisikan metode `IsPreferredCombinedSummaryPath` dengan hasil bertipe `bool`. Mengklasifikasikan path scalar compact untuk ringkasan
    // gabungan utama. Masukan: Parameter `path` bertipe `string` membawa nilai path.
    public static bool IsPreferredCombinedSummaryPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return false;
        }

        return !path.Contains('[') && !path.Contains('.');
    }

    /// <summary>
    /// Mengklasifikasikan path fallback ringkasan gabungan yang masih menolak array.
    /// </summary>
    // Mendefinisikan metode `IsFallbackCombinedSummaryPath` dengan hasil bertipe `bool`. Mengklasifikasikan path fallback ringkasan gabungan yang masih
    // menolak array. Masukan: Parameter `path` bertipe `string` membawa nilai path.
    public static bool IsFallbackCombinedSummaryPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return false;
        }

        return !path.Contains('[');
    }

    private static string? TryTranslateMetricLexicon(string lexiconKey, Func<string, string> translate)
    {
        if (string.IsNullOrWhiteSpace(lexiconKey))
        {
            return null;
        }

        var translated = translate(lexiconKey);
        if (string.Equals(translated, lexiconKey, StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        return translated;
    }

    private static string GetRootMetricKey(string rawPath)
    {
        if (string.IsNullOrWhiteSpace(rawPath))
        {
            return string.Empty;
        }

        var separatorIndex = rawPath.IndexOfAny(['.', '[']);
        var root = separatorIndex < 0 ? rawPath : rawPath[..separatorIndex];
        return NormalizeMetricLexiconKey(root);
    }

    private static string GetLeafMetricKey(string rawPath)
    {
        if (string.IsNullOrWhiteSpace(rawPath))
        {
            return string.Empty;
        }

        var leaf = rawPath[(rawPath.LastIndexOf('.') + 1)..];
        var arrayIndex = leaf.IndexOf('[');
        return NormalizeMetricLexiconKey(arrayIndex < 0 ? leaf : leaf[..arrayIndex]);
    }

    private static string ResolveUnitKey(string metricKey, string leafMetricKey, string normalizedPath)
    {
        if (PercentageMetricKeys.Contains(metricKey))
        {
            return "players.support.unit.percent";
        }

        if (FractionPercentageMetricKeys.Contains(metricKey) ||
            FractionPercentageMetricKeys.Contains(leafMetricKey))
        {
            return "players.support.unit.percent";
        }

        if (MultiplierMetricKeys.Contains(metricKey))
        {
            return "players.support.unit.multiplier";
        }

        if (!string.Equals(metricKey, leafMetricKey, StringComparison.OrdinalIgnoreCase))
        {
            if (leafMetricKey == "day_index") return "players.support.unit.day";
            if (leafMetricKey == "action_slot") return "players.support.unit.action_slot";
            if (leafMetricKey == "rank") return "players.support.unit.rank";
            if (leafMetricKey is "amount" or "net" or "coins") return "players.support.unit.coins";
            if (leafMetricKey == "total_actions") return "players.support.unit.action_tokens";
            if (leafMetricKey is "distinct_actions" or "repeated_actions") return "players.support.unit.actions";
            if (leafMetricKey is "action_type" or "action_index" or "actions" or
                "basic_profile" or "collector_profile" or "specialist_profile") return string.Empty;
        }

        if (metricKey.Contains("rank", StringComparison.OrdinalIgnoreCase))
        {
            return "players.support.unit.rank";
        }

        if (metricKey.Contains("point", StringComparison.OrdinalIgnoreCase) ||
            metricKey.EndsWith("_pts", StringComparison.OrdinalIgnoreCase) ||
            metricKey.Contains("penalty", StringComparison.OrdinalIgnoreCase))
        {
            return "players.support.unit.points";
        }

        if (metricKey.Contains("day_", StringComparison.OrdinalIgnoreCase) ||
            metricKey is "day_index" or "latest_day_index")
        {
            return "players.support.unit.day";
        }

        if (metricKey.StartsWith("turn_number_", StringComparison.OrdinalIgnoreCase))
        {
            return "players.support.unit.turn";
        }

        if (metricKey.Contains("action_slot", StringComparison.OrdinalIgnoreCase))
        {
            return "players.support.unit.action_slot";
        }

        if (metricKey is "meal_orders_per_turn_average")
        {
            return "players.support.unit.orders_per_turn";
        }

        if (metricKey is "ingredients_used_per_meal_average")
        {
            return "players.support.unit.ingredients_per_order";
        }

        if (metricKey is "donation_stability_std_deviation")
        {
            return "players.support.unit.coins";
        }

        if (metricKey is "donation_events")
        {
            return "players.support.unit.day";
        }

        if (metricKey is "n_active_income_sources" or "active_income_source_count")
        {
            return "players.support.unit.sources";
        }

        if (metricKey is "saving_and_goal_actions" or "total_actions" or "actions_per_turn" or "income_producing_actions" or "income_main_actions" or
                "all_player_actions" or "total_main_actions" or
                "savings_actions" or "saving_actions" or "financial_goal_actions" or
                "insurance_premium_actions" or "insurance_actions" or "loan_repayment_actions" ||
            leafMetricKey is "saving_and_goal_actions" or "income_producing_actions" or "income_main_actions" or
                "all_player_actions" or "total_main_actions" or
                "savings_actions" or "saving_actions" or "financial_goal_actions" or
                "insurance_premium_actions" or "insurance_actions" or "loan_repayment_actions")
        {
            return "players.support.unit.action_tokens";
        }

        if (metricKey is "outstanding_loan" or "liquid_assets" or "attempted_goal_target_total" ||
            leafMetricKey is "outstanding_loan" or "liquid_assets" or "attempted_goal_target_total")
        {
            return "players.support.unit.coins";
        }

        if (metricKey is "risks_resolved_without_emergency" or "life_risk_cards_drawn" ||
            leafMetricKey is "risks_resolved_without_emergency" or "life_risk_cards_drawn")
        {
            return "players.support.unit.risk_events";
        }

        if (normalizedPath.Contains("financial_goals_balance_per_goal", StringComparison.OrdinalIgnoreCase))
        {
            return "players.support.unit.coins";
        }

        if (metricKey is "specific_tertiary_need" or "collection_mission_complete")
        {
            return string.Empty;
        }

        var isMoney = metricKey.Contains("coin", StringComparison.OrdinalIgnoreCase) ||
                      leafMetricKey.Contains("coin", StringComparison.OrdinalIgnoreCase) ||
                      metricKey.Contains("cash", StringComparison.OrdinalIgnoreCase) ||
                      metricKey.Contains("income", StringComparison.OrdinalIgnoreCase) ||
                      metricKey.Contains("expense", StringComparison.OrdinalIgnoreCase) ||
                      metricKey.Contains("cost", StringComparison.OrdinalIgnoreCase) ||
                      metricKey.Contains("price", StringComparison.OrdinalIgnoreCase) ||
                      metricKey.Contains("investment", StringComparison.OrdinalIgnoreCase) ||
                      metricKey is "amount" or "pension_fund_total" or "donation_total_coins" or
                          "insurance_payments_made" or "ingredient_cards_value_end";
        if (isMoney)
        {
            return "players.support.unit.coins";
        }

        if (metricKey.Contains("score", StringComparison.OrdinalIgnoreCase) ||
            metricKey is "donation_stability" or "mission_achievement")
        {
            return "players.support.unit.score";
        }

        if (normalizedPath.Contains("ingredient", StringComparison.OrdinalIgnoreCase)) return "players.support.unit.ingredient_cards";
        if (normalizedPath.Contains("meal_order", StringComparison.OrdinalIgnoreCase)) return "players.support.unit.orders";
        if (normalizedPath.Contains("need", StringComparison.OrdinalIgnoreCase)) return "players.support.unit.need_cards";
        if (normalizedPath.Contains("gold_card", StringComparison.OrdinalIgnoreCase)) return "players.support.unit.gold_cards";
        if (normalizedPath.Contains("card", StringComparison.OrdinalIgnoreCase)) return "players.support.unit.cards";
        if (normalizedPath.Contains("risk", StringComparison.OrdinalIgnoreCase)) return "players.support.unit.risk_events";
        if (normalizedPath.Contains("financial_goal", StringComparison.OrdinalIgnoreCase)) return "players.support.unit.goals";
        if (normalizedPath.Contains("loan", StringComparison.OrdinalIgnoreCase)) return "players.support.unit.loans";
        if (normalizedPath.Contains("action", StringComparison.OrdinalIgnoreCase)) return "players.support.unit.actions";
        if (normalizedPath.Contains("event", StringComparison.OrdinalIgnoreCase)) return "players.support.unit.events";
        if (normalizedPath.Contains("transaction", StringComparison.OrdinalIgnoreCase)) return "players.support.unit.transactions";
        if (normalizedPath.Contains("donation", StringComparison.OrdinalIgnoreCase) ||
            normalizedPath.Contains("option", StringComparison.OrdinalIgnoreCase)) return "players.support.unit.times";
        return string.Empty;
    }

    private static bool IsGameDayMetric(string metricKey) => metricKey is
        "day_when_debt_introduced" or
        "day_when_first_risk_hit" or
        "day_game_completion" or
        "latest_day_index";

    private static string ResolveExplanationKey(string metricKey, bool isDerived, string unitKey)
    {
        var timelineExplanationKey = metricKey switch
        {
            // Untuk pola `”day_when_debt_introduced”`, menghasilkan nilai literal `”players.support.meaning.first_loan_day”` sebagai hasil switch.
            "day_when_debt_introduced" => "players.support.meaning.first_loan_day",
            // Untuk pola `”day_when_first_risk_hit”`, menghasilkan nilai literal `”players.support.meaning.first_risk_day”` sebagai hasil switch.
            "day_when_first_risk_hit" => "players.support.meaning.first_risk_day",
            // Untuk pola `”day_game_completion” or ”latest_day_index”`, menghasilkan nilai literal `”players.support.meaning.last_activity_day”` sebagai hasil
            // switch.
            "day_game_completion" or "latest_day_index" => "players.support.meaning.last_activity_day",
            // Untuk pola `_`, menghasilkan `string.Empty`, yaitu nilai kosong bawaan tipe terkait sebagai hasil switch.
            _ => string.Empty
        };
        if (!string.IsNullOrWhiteSpace(timelineExplanationKey))
        {
            return timelineExplanationKey;
        }

        if (metricKey == "net_income_per_turn")
        {
            return "players.support.meaning.coin_change_event";
        }

        if (metricKey == "transaction_history")
        {
            return "players.support.meaning.transaction_history";
        }

        if (metricKey == "action_usage_history")
        {
            return "players.support.meaning.action_usage_history";
        }

        if (metricKey == "emergency_options_used")
        {
            return "players.support.meaning.emergency_actions";
        }

        var rawExplanationKey = metricKey switch
        {
            // Untuk pola `”ingredients_held_current”`, menghasilkan nilai literal `”players.support.meaning.ingredients_remaining”` sebagai hasil switch.
            "ingredients_held_current" => "players.support.meaning.ingredients_remaining",
            // Untuk pola `”ingredients_used_per_meal”`, menghasilkan nilai literal `”players.support.meaning.ingredients_per_order”` sebagai hasil switch.
            "ingredients_used_per_meal" => "players.support.meaning.ingredients_per_order",
            // Untuk pola `”meal_orders_per_turn_average”`, menghasilkan nilai literal `”players.support.meaning.orders_per_active_day”` sebagai hasil switch.
            "meal_orders_per_turn_average" => "players.support.meaning.orders_per_active_day",
            // Untuk pola `”meal_order_income_per_order”`, menghasilkan nilai literal `”players.support.meaning.income_per_order”` sebagai hasil switch.
            "meal_order_income_per_order" => "players.support.meaning.income_per_order",
            // Untuk pola `”ingredient_types_held”`, menghasilkan nilai literal `”players.support.meaning.ingredient_types_remaining”` sebagai hasil switch.
            "ingredient_types_held" => "players.support.meaning.ingredient_types_remaining",
            // Untuk pola `”ingredients_wasted”`, menghasilkan nilai literal `”players.support.meaning.ingredients_discarded”` sebagai hasil switch.
            "ingredients_wasted" => "players.support.meaning.ingredients_discarded",
            // Untuk pola `”gold_cards_purchased”`, menghasilkan nilai literal `”players.support.meaning.gold_purchased”` sebagai hasil switch.
            "gold_cards_purchased" => "players.support.meaning.gold_purchased",
            // Untuk pola `”gold_cards_held_end”`, menghasilkan nilai literal `”players.support.meaning.gold_remaining”` sebagai hasil switch.
            "gold_cards_held_end" => "players.support.meaning.gold_remaining",
            // Untuk pola `”gold_investment_net”`, menghasilkan nilai literal `”players.support.meaning.gold_cashflow”` sebagai hasil switch.
            "gold_investment_net" => "players.support.meaning.gold_cashflow",
            // Untuk pola `”ingredient_cards_value_end”`, menghasilkan nilai literal `”players.support.meaning.pension_ingredient_value”` sebagai hasil switch.
            "ingredient_cards_value_end" => "players.support.meaning.pension_ingredient_value",
            // Untuk pola `”life_risk_costs_per_card” or ”life_risk_costs_total”`, menghasilkan nilai literal `”players.support.meaning.risk_nominal_cost”`
            // sebagai hasil switch.
            "life_risk_costs_per_card" or "life_risk_costs_total" => "players.support.meaning.risk_nominal_cost",
            // Untuk pola `”donation_rank_per_friday”`, menghasilkan nilai literal `”players.support.meaning.donation_rank”` sebagai hasil switch.
            "donation_rank_per_friday" => "players.support.meaning.donation_rank",
            // Untuk pola `”donation_history”`, menghasilkan nilai literal `”players.support.meaning.donation_history”` sebagai hasil switch.
            "donation_history" => "players.support.meaning.donation_history",
            // Untuk pola `”donation_happiness_points” or ”donations_pts”`, menghasilkan nilai literal `”players.support.meaning.donation_happiness”` sebagai
            // hasil switch.
            "donation_happiness_points" or "donations_pts" => "players.support.meaning.donation_happiness",
            // Untuk pola `”financial_goals_completed”`, menghasilkan nilai literal `”players.support.meaning.completed_goals”` sebagai hasil switch.
            "financial_goals_completed" => "players.support.meaning.completed_goals",
            "financial_goals_attempted" => "players.support.meaning.attempted_goals",
            "sharia_loans_taken" => "players.support.meaning.loans_taken",
            "sharia_loans_repaid" => "players.support.meaning.loans_repaid",
            "sharia_loans_unpaid_end" => "players.support.meaning.loans_unpaid",
            // Untuk pola `”sharia_loans_outstanding_coins”`, menghasilkan nilai literal `”players.support.meaning.outstanding_loan”` sebagai hasil switch.
            "sharia_loans_outstanding_coins" => "players.support.meaning.outstanding_loan",
            // Untuk pola `”need_cards_purchased”`, menghasilkan nilai literal `”players.support.meaning.need_cards_purchased”` sebagai hasil switch.
            "need_cards_purchased" => "players.support.meaning.need_cards_purchased",
            // Untuk pola `”need_cards_owned_current”`, menghasilkan nilai literal `”players.support.meaning.need_cards_owned”` sebagai hasil switch.
            "need_cards_owned_current" => "players.support.meaning.need_cards_owned",
            // Untuk pola `”primary_needs_owned” or ”secondary_needs_owned” or ”tertiary_needs_owned”`, menghasilkan nilai literal
            // `”players.support.meaning.need_level_owned”` sebagai hasil switch.
            "primary_needs_owned" or "secondary_needs_owned" or "tertiary_needs_owned" => "players.support.meaning.need_level_owned",
            // Untuk pola `”specific_tertiary_need”`, menghasilkan nilai literal `”players.support.meaning.mission_need_owned”` sebagai hasil switch.
            "specific_tertiary_need" => "players.support.meaning.mission_need_owned",
            // Untuk pola `”collection_mission_complete”`, menghasilkan nilai literal `”players.support.meaning.collection_mission”` sebagai hasil switch.
            "collection_mission_complete" => "players.support.meaning.collection_mission",
            // Untuk pola `”need_cards_coins_spent”`, menghasilkan nilai literal `”players.support.meaning.need_purchase_cost”` sebagai hasil switch.
            "need_cards_coins_spent" => "players.support.meaning.need_purchase_cost",
            // Untuk pola `”ingredients_collected”`, menghasilkan nilai literal `”players.support.meaning.ingredients_collected”` sebagai hasil switch.
            "ingredients_collected" => "players.support.meaning.ingredients_collected",
            // Untuk pola `”ingredients_used_total”`, menghasilkan nilai literal `”players.support.meaning.ingredients_used_total”` sebagai hasil switch.
            "ingredients_used_total" => "players.support.meaning.ingredients_used_total",
            // Untuk pola `”ingredients_used_per_meal_average”`, menghasilkan nilai literal `”players.support.meaning.ingredients_used_average”` sebagai hasil
            // switch.
            "ingredients_used_per_meal_average" => "players.support.meaning.ingredients_used_average",
            // Untuk pola `”ingredient_investment_coins_total”`, menghasilkan nilai literal `”players.support.meaning.ingredient_purchase_cost”` sebagai hasil
            // switch.
            "ingredient_investment_coins_total" => "players.support.meaning.ingredient_purchase_cost",
            // Untuk pola `”meal_orders_claimed”`, menghasilkan nilai literal `”players.support.meaning.orders_completed”` sebagai hasil switch.
            "meal_orders_claimed" => "players.support.meaning.orders_completed",
            // Untuk pola `”meal_order_income_total”`, menghasilkan nilai literal `”players.support.meaning.order_income_total”` sebagai hasil switch.
            "meal_order_income_total" => "players.support.meaning.order_income_total",
            // Untuk pola `”gold_cards_initial”`, menghasilkan nilai literal `”players.support.meaning.gold_initial”` sebagai hasil switch.
            "gold_cards_initial" => "players.support.meaning.gold_initial",
            // Untuk pola `”gold_cards_sold”`, menghasilkan nilai literal `”players.support.meaning.gold_sold”` sebagai hasil switch.
            "gold_cards_sold" => "players.support.meaning.gold_sold",
            // Untuk pola `”gold_prices_per_purchase”`, menghasilkan nilai literal `”players.support.meaning.gold_purchase_prices”` sebagai hasil switch.
            "gold_prices_per_purchase" => "players.support.meaning.gold_purchase_prices",
            // Untuk pola `”gold_price_per_sale”`, menghasilkan nilai literal `”players.support.meaning.gold_sale_prices”` sebagai hasil switch.
            "gold_price_per_sale" => "players.support.meaning.gold_sale_prices",
            // Untuk pola `”gold_investment_coins_spent”`, menghasilkan nilai literal `”players.support.meaning.gold_purchase_cost”` sebagai hasil switch.
            "gold_investment_coins_spent" => "players.support.meaning.gold_purchase_cost",
            // Untuk pola `”gold_investment_coins_earned”`, menghasilkan nilai literal `”players.support.meaning.gold_sale_income”` sebagai hasil switch.
            "gold_investment_coins_earned" => "players.support.meaning.gold_sale_income",
            // Untuk pola `”pension_fund_total”`, menghasilkan nilai literal `”players.support.meaning.pension_total”` sebagai hasil switch.
            "pension_fund_total" => "players.support.meaning.pension_total",
            // Untuk pola `”pension_fund_rank_per_game”`, menghasilkan nilai literal `”players.support.meaning.pension_rank”` sebagai hasil switch.
            "pension_fund_rank_per_game" => "players.support.meaning.pension_rank",
            // Untuk pola `”pension_fund_happiness_points”`, menghasilkan nilai literal `”players.support.meaning.pension_happiness”` sebagai hasil switch.
            "pension_fund_happiness_points" => "players.support.meaning.pension_happiness",
            // Untuk pola `”life_risk_cards_drawn”`, menghasilkan nilai literal `”players.support.meaning.risk_cards_drawn”` sebagai hasil switch.
            "life_risk_cards_drawn" => "players.support.meaning.risk_cards_drawn",
            // Untuk pola `”life_risk_mitigated_with_insurance”`, menghasilkan nilai literal `”players.support.meaning.risk_insured”` sebagai hasil switch.
            "life_risk_mitigated_with_insurance" => "players.support.meaning.risk_insured",
            // Untuk pola `”insurance_payments_made”`, menghasilkan nilai literal `”players.support.meaning.insurance_premium”` sebagai hasil switch.
            "insurance_payments_made" => "players.support.meaning.insurance_premium",
            // Untuk pola `_`, menghasilkan `string.Empty`, yaitu nilai kosong bawaan tipe terkait sebagai hasil switch.
            _ => string.Empty
        };
        if (!string.IsNullOrWhiteSpace(rawExplanationKey))
        {
            return rawExplanationKey;
        }

        if (isDerived)
        {
            var derivedKey = metricKey switch
            {
                // Untuk pola `”cash_growth_percent” or ”net_worth_index” or ”growth_pattern_ratio”`, menghasilkan nilai literal `”players.support.meaning.growth”`
                // sebagai hasil switch.
                "cash_growth_percent" or "net_worth_index" => "players.support.meaning.cash_growth",
                "happiness_source_diversity_percent" => "players.support.meaning.happiness_diversity",
                "growth_pattern_ratio" => "players.support.meaning.growth",
                // Untuk pola `”income_diversification_index” or ”income_diversification_ratio” or ”income_share_i” or ”n_active_income_sources”`, menghasilkan
                // nilai literal `”players.support.meaning.income_mix”` sebagai hasil switch.
                "income_diversification_index" or "income_diversification_ratio" or "income_share_i" or
                    "n_active_income_sources" => "players.support.meaning.income_mix",
                // Untuk pola `”business_expense_share_percent” or ”expense_management_efficiency”`, menghasilkan nilai literal
                // `”players.support.meaning.expense_efficiency”` sebagai hasil switch.
                "business_expense_share_percent" or "expense_management_efficiency" => "players.support.meaning.expense_efficiency",
                // Untuk pola `”meal_order_profit_margin_percent” or ”business_profit_margin” or ”business_efficiency_ratio”`, menghasilkan nilai literal
                // `”players.support.meaning.business”` sebagai hasil switch.
                "meal_order_profit_margin_percent" or "business_profit_margin" or "business_efficiency_ratio" => "players.support.meaning.business",
                // Untuk pola `”gold_roi_percentage”`, menghasilkan nilai literal `”players.support.meaning.gold_return”` sebagai hasil switch.
                "gold_roi_percentage" => "players.support.meaning.gold_return",
                // Untuk pola `”risk_exposure_percentage” or ”risk_cost_intensity”`, menghasilkan nilai literal `”players.support.meaning.risk_impact”` sebagai
                // hasil switch.
                "risk_exposure_percentage" or "risk_cost_intensity" => "players.support.meaning.risk_impact",
                // Untuk pola `”risk_readiness_percent”`, menghasilkan nilai literal `”players.support.meaning.risk_readiness”` sebagai hasil switch.
                "risk_readiness_percent" => "players.support.meaning.risk_readiness",
                // Untuk pola `”risk_mitigation_effectiveness” or ”insurance_activation_rate” or ”insurance_coverage_rate”`, menghasilkan nilai literal
                // `”players.support.meaning.risk_protection”` sebagai hasil switch.
                "risk_mitigation_effectiveness" or "insurance_activation_rate" or "insurance_coverage_rate" => "players.support.meaning.risk_protection",
                // Untuk pola `”risk_appetite_score” or ”risk_appetite_score_normalized” or ”risk_acceptance_rate”`, menghasilkan nilai literal
                // `”players.support.meaning.risk_appetite”` sebagai hasil switch.
                "risk_appetite_score" or "risk_appetite_score_normalized" or "risk_acceptance_rate" => "players.support.meaning.risk_appetite",
                // Untuk pola `”loan_burden_percent”`, menghasilkan nilai literal `”players.support.meaning.loan_burden”` sebagai hasil switch.
                "loan_burden_percent" => "players.support.meaning.loan_burden",
                "financial_goal_completion_percent" => "players.support.meaning.goal_completion",
                "donation_commitment_score" => "players.support.meaning.donation_commitment",
                // Untuk pola `”debt_leverage_ratio” or ”debt_ratio” or ”loan_repayment_discipline”`, menghasilkan nilai literal `”players.support.meaning.debt”`
                // sebagai hasil switch.
                "debt_leverage_ratio" or "debt_ratio" or "loan_repayment_discipline" => "players.support.meaning.debt",
                // Untuk pola `”financial_goal_progress_percent” or ”goal_ambition” or ”goal_ambition_index” or ”goal_setting_ambition” or ”goal_attempt_rate” or
                // ”goal_investment_rate”`, menghasilkan nilai literal `”players.support.meaning.goals”` sebagai hasil switch.
                "financial_goal_progress_percent" or "goal_ambition" or "goal_ambition_index" or "goal_setting_ambition" or "goal_attempt_rate" or "goal_investment_rate" => "players.support.meaning.goals",
                // Untuk pola `”income_action_focus_percent” or ”action_efficiency” or ”action_efficiency_percent” or ”action_diversity_score_avg”`, menghasilkan
                // nilai literal `”players.support.meaning.actions”` sebagai hasil switch.
                "income_action_focus_percent" or "action_efficiency" or "action_efficiency_percent" or "action_diversity_score_avg" => "players.support.meaning.actions",
                // Untuk pola `”ingredient_utilization_percent”`, menghasilkan nilai literal `”players.support.meaning.ingredient_utilization”` sebagai hasil
                // switch.
                "ingredient_utilization_percent" => "players.support.meaning.ingredient_utilization",
                // Untuk pola `”meal_order_success_rate”`, menghasilkan nilai literal `”players.support.meaning.orders”` sebagai hasil switch.
                "meal_order_success_rate" => "players.support.meaning.orders",
                // Untuk pola `”long_term_action_share_percent” or ”planning_horizon” or ”planning_horizon_percent”`, menghasilkan nilai literal
                // `”players.support.meaning.planning”` sebagai hasil switch.
                "long_term_action_share_percent" or "planning_horizon" or "planning_horizon_percent" => "players.support.meaning.planning",
                // Untuk pola `”need_fulfillment_diversity_percent” or ”fulfillment_diversity” or ”fulfillment_diversity_document_formula” or ”p_primary” or
                // ”p_secondary” or ”p_tertiary”`, menghasilkan nilai literal `”players.support.meaning.need_balance”` sebagai hasil switch.
                "need_fulfillment_diversity_percent" or "fulfillment_diversity" or "fulfillment_diversity_document_formula" or "p_primary" or "p_secondary" or "p_tertiary" => "players.support.meaning.need_balance",
                // Untuk pola `”mission_achievement”`, menghasilkan nilai literal `”players.support.meaning.needs”` sebagai hasil switch.
                "mission_achievement" => "players.support.meaning.needs",
                // Untuk pola `”donation_aggressiveness_percent” or ”donation_stability_std_deviation” or ”donation_ratio” or ”friday_participation_rate” or
                // ”donation_commitment_score” or ”donation_stabilit...`, menghasilkan nilai literal `”players.support.meaning.donation”` sebagai hasil switch.
                "donation_aggressiveness_percent" or "donation_stability_std_deviation" or "donation_ratio" or
                    "friday_participation_rate" or "donation_stability" or "donation_stability_index" => "players.support.meaning.donation",
                // Untuk pola `_`, menghasilkan `string.Empty`, yaitu nilai kosong bawaan tipe terkait sebagai hasil switch.
                _ => string.Empty
            };

            if (!string.IsNullOrWhiteSpace(derivedKey))
            {
                return derivedKey;
            }
        }

        var functionCategory = ResolveFunctionCategory(metricKey);
        if (!string.IsNullOrWhiteSpace(functionCategory))
        {
            return $"players.support.meaning.{functionCategory}";
        }

        if (isDerived)
        {
            return "players.support.meaning.derived";
        }

        return unitKey switch
        {
            // Untuk pola `”players.support.unit.coins”`, menghasilkan nilai literal `”players.support.meaning.raw_coins”` sebagai hasil switch.
            "players.support.unit.coins" => "players.support.meaning.raw_coins",
            // Untuk pola `”players.support.unit.points”`, menghasilkan nilai literal `”players.support.meaning.raw_points”` sebagai hasil switch.
            "players.support.unit.points" => "players.support.meaning.raw_points",
            // Untuk pola `”players.support.unit.rank”`, menghasilkan nilai literal `”players.support.meaning.raw_rank”` sebagai hasil switch.
            "players.support.unit.rank" => "players.support.meaning.raw_rank",
            // Untuk pola `”players.support.unit.day” or ”players.support.unit.turn” or ”players.support.unit.action_slot”`, menghasilkan nilai literal
            // `”players.support.meaning.raw_timeline”` sebagai hasil switch.
            "players.support.unit.day" or "players.support.unit.turn" or "players.support.unit.action_slot" => "players.support.meaning.raw_timeline",
            // Untuk pola `_` dengan syarat tambahan `unitKey.StartsWith(”players.support.unit.”, StringComparison.Ordinal) && unitKey !=
            // ”players.support.unit.coins” && unitKey != ”players.support.unit.points” && unitKey != ”pla...`, menghasilkan nilai literal
            // `”players.support.meaning.raw_count”` sebagai hasil switch.
            _ when unitKey.StartsWith("players.support.unit.", StringComparison.Ordinal) &&
                   unitKey != "players.support.unit.coins" &&
                   unitKey != "players.support.unit.points" &&
                   unitKey != "players.support.unit.percent" &&
                   unitKey != "players.support.unit.multiplier" => "players.support.meaning.raw_count",
            // Untuk pola `_`, menghasilkan nilai literal `”players.support.meaning.raw”` sebagai hasil switch.
            _ => "players.support.meaning.raw"
        };
    }

    private static string ResolveGuidanceKey(
        // Parameter `metricKey` bertipe `string` membawa nilai metric kunci.
        string metricKey,
        // Parameter `numericValue` bertipe `double` membawa nilai numerik nilai.
        double numericValue,
        // Parameter `hasNumericValue` bertipe `bool` membawa nilai memiliki numerik nilai.
        bool hasNumericValue,
        // Parameter `isDerived` bertipe `bool` membawa nilai berstatus derived.
        bool isDerived,
        // Parameter `unitKey` bertipe `string` membawa nilai unit kunci.
        string unitKey)
    {
        if (hasNumericValue)
        {
            var normalizedRatio = numericValue;

            return metricKey switch
            {
                // Untuk pola `”cash_growth_percent” or ”net_worth_index”` dengan syarat tambahan `numericValue >= 300`, menghasilkan nilai literal
                // `”players.support.guide.wealth_exceptional”` sebagai hasil switch.
                "cash_growth_percent" or "net_worth_index" when numericValue >= 200 => "players.support.guide.wealth_exceptional",
                // Untuk pola `”cash_growth_percent” or ”net_worth_index”` dengan syarat tambahan `numericValue >= 200`, menghasilkan nilai literal
                // `”players.support.guide.wealth_strong”` sebagai hasil switch.
                "cash_growth_percent" or "net_worth_index" when numericValue >= 100 => "players.support.guide.wealth_strong",
                // Untuk pola `”cash_growth_percent” or ”net_worth_index”` dengan syarat tambahan `numericValue >= 100`, menghasilkan nilai literal
                // `”players.support.guide.wealth_growing”` sebagai hasil switch.
                "cash_growth_percent" or "net_worth_index" when numericValue > 0 => "players.support.guide.wealth_growing",
                "cash_growth_percent" or "net_worth_index" when numericValue == 0 => "players.support.guide.wealth_unchanged",
                // Untuk pola `”cash_growth_percent” or ”net_worth_index”`, menghasilkan nilai literal `”players.support.guide.wealth_declining”` sebagai hasil
                // switch.
                "cash_growth_percent" or "net_worth_index" => "players.support.guide.wealth_declining",
                "happiness_source_diversity_percent" when numericValue >= 67 => "players.support.guide.happiness_diverse",
                "happiness_source_diversity_percent" when numericValue >= 34 => "players.support.guide.happiness_mixed",
                "happiness_source_diversity_percent" => "players.support.guide.happiness_concentrated",
                // Untuk pola `”income_diversification_index” or ”income_diversification_ratio”` dengan syarat tambahan `numericValue >= 67`, menghasilkan nilai
                // literal `”players.support.guide.income_diverse”` sebagai hasil switch.
                "income_diversification_index" or "income_diversification_ratio" when numericValue >= 67 => "players.support.guide.income_diverse",
                // Untuk pola `”income_diversification_index” or ”income_diversification_ratio”` dengan syarat tambahan `numericValue < 34`, menghasilkan nilai
                // literal `”players.support.guide.income_concentrated”` sebagai hasil switch.
                "income_diversification_index" or "income_diversification_ratio" when numericValue < 34 => "players.support.guide.income_concentrated",
                // Untuk pola `”income_diversification_index” or ”income_diversification_ratio”`, menghasilkan nilai literal `”players.support.guide.income_mixed”`
                // sebagai hasil switch.
                "income_diversification_index" or "income_diversification_ratio" => "players.support.guide.income_mixed",
                // Untuk pola `”business_expense_share_percent” or ”expense_management_efficiency”`, menghasilkan nilai literal
                // `”players.support.guide.expense_efficiency”` sebagai hasil switch.
                "business_expense_share_percent" or "expense_management_efficiency" => "players.support.guide.expense_efficiency",
                // Untuk pola `”meal_order_profit_margin_percent” or ”business_profit_margin” or ”gold_roi_percentage”` dengan syarat tambahan `numericValue > 0`,
                // menghasilkan nilai literal `”players.support.guide.return_positive”` sebagai hasil switch.
                "meal_order_profit_margin_percent" or "business_profit_margin" or "gold_roi_percentage" when numericValue > 0 => "players.support.guide.return_positive",
                // Untuk pola `”meal_order_profit_margin_percent” or ”business_profit_margin” or ”gold_roi_percentage”` dengan syarat tambahan `numericValue < 0`,
                // menghasilkan nilai literal `”players.support.guide.return_negative”` sebagai hasil switch.
                "meal_order_profit_margin_percent" or "business_profit_margin" or "gold_roi_percentage" when numericValue < 0 => "players.support.guide.return_negative",
                // Untuk pola `”meal_order_profit_margin_percent” or ”business_profit_margin” or ”gold_roi_percentage”`, menghasilkan nilai literal
                // `”players.support.guide.return_even”` sebagai hasil switch.
                "meal_order_profit_margin_percent" or "business_profit_margin" or "gold_roi_percentage" => "players.support.guide.return_even",
                // Untuk pola `”business_efficiency_ratio”` dengan syarat tambahan `numericValue > 2`, menghasilkan nilai literal
                // `”players.support.guide.business_efficient”` sebagai hasil switch.
                "business_efficiency_ratio" when numericValue > 2 => "players.support.guide.business_efficient",
                // Untuk pola `”business_efficiency_ratio”` dengan syarat tambahan `numericValue < 1.5`, menghasilkan nilai literal
                // `”players.support.guide.business_inefficient”` sebagai hasil switch.
                "business_efficiency_ratio" when numericValue < 1.5 => "players.support.guide.business_inefficient",
                // Untuk pola `”business_efficiency_ratio”`, menghasilkan nilai literal `”players.support.guide.business_moderate”` sebagai hasil switch.
                "business_efficiency_ratio" => "players.support.guide.business_moderate",
                // Untuk pola `”risk_exposure_percentage”` dengan syarat tambahan `numericValue > 30`, menghasilkan nilai literal
                // `”players.support.guide.risk_high”` sebagai hasil switch.
                "risk_exposure_percentage" when numericValue > 30 => "players.support.guide.risk_high",
                // Untuk pola `”risk_exposure_percentage”` dengan syarat tambahan `numericValue < 10`, menghasilkan nilai literal `”players.support.guide.risk_low”`
                // sebagai hasil switch.
                "risk_exposure_percentage" when numericValue < 10 => "players.support.guide.risk_low",
                // Untuk pola `”risk_exposure_percentage”`, menghasilkan nilai literal `”players.support.guide.risk_moderate”` sebagai hasil switch.
                "risk_exposure_percentage" => "players.support.guide.risk_moderate",
                // Untuk pola `”risk_appetite_score” or ”risk_appetite_score_normalized”` dengan syarat tambahan `numericValue < 25`, menghasilkan nilai literal
                // `”players.support.guide.appetite_cautious”` sebagai hasil switch.
                "risk_appetite_score" or "risk_appetite_score_normalized" when numericValue < 25 => "players.support.guide.appetite_cautious",
                // Untuk pola `”risk_appetite_score” or ”risk_appetite_score_normalized”` dengan syarat tambahan `numericValue <= 75`, menghasilkan nilai literal
                // `”players.support.guide.appetite_balanced”` sebagai hasil switch.
                "risk_appetite_score" or "risk_appetite_score_normalized" when numericValue <= 75 => "players.support.guide.appetite_balanced",
                // Untuk pola `”risk_appetite_score” or ”risk_appetite_score_normalized”`, menghasilkan nilai literal `”players.support.guide.appetite_high”`
                // sebagai hasil switch.
                "risk_appetite_score" or "risk_appetite_score_normalized" => "players.support.guide.appetite_high",
                // Untuk pola `”risk_readiness_percent”` dengan syarat tambahan `numericValue >= 99.5`, menghasilkan nilai literal
                // `”players.support.guide.risk_readiness_complete”` sebagai hasil switch.
                "risk_readiness_percent" when numericValue >= 100 => "players.support.guide.risk_readiness_complete",
                // Untuk pola `”risk_readiness_percent”` dengan syarat tambahan `numericValue >= 50`, menghasilkan nilai literal
                // `”players.support.guide.risk_readiness_most”` sebagai hasil switch.
                "risk_readiness_percent" when numericValue >= 50 => "players.support.guide.risk_readiness_most",
                // Untuk pola `”risk_readiness_percent”`, menghasilkan nilai literal `”players.support.guide.risk_readiness_limited”` sebagai hasil switch.
                "risk_readiness_percent" => "players.support.guide.risk_readiness_limited",
                // Untuk pola `”loan_burden_percent” or ”debt_leverage_ratio”` dengan syarat tambahan `numericValue > 75`, menghasilkan nilai literal
                // `”players.support.guide.debt_high”` sebagai hasil switch.
                "loan_burden_percent" or "debt_leverage_ratio" when numericValue > 75 => "players.support.guide.debt_high",
                "financial_goal_completion_percent" when numericValue >= 100 => "players.support.guide.goal_completion_all",
                "financial_goal_completion_percent" when numericValue <= 0 => "players.support.guide.goal_completion_none",
                "financial_goal_completion_percent" => "players.support.guide.goal_completion_partial",
                "sharia_loans_taken" => "players.support.meaning.loans_taken",
                "sharia_loans_repaid" => "players.support.meaning.loans_repaid",
                "sharia_loans_unpaid_end" => "players.support.meaning.loans_unpaid",
                // Untuk pola `”loan_burden_percent” or ”debt_leverage_ratio”` dengan syarat tambahan `numericValue <= 25`, menghasilkan nilai literal
                // `”players.support.guide.debt_low”` sebagai hasil switch.
                "loan_burden_percent" or "debt_leverage_ratio" when numericValue <= 25 => "players.support.guide.debt_low",
                // Untuk pola `”loan_burden_percent” or ”debt_leverage_ratio”`, menghasilkan nilai literal `”players.support.guide.debt_moderate”` sebagai hasil
                // switch.
                "loan_burden_percent" or "debt_leverage_ratio" => "players.support.guide.debt_moderate",
                // Untuk pola `”financial_goal_progress_percent” or ”goal_ambition” or ”goal_ambition_index”` dengan syarat tambahan `numericValue >= 99.5`,
                // menghasilkan nilai literal `”players.support.guide.goals_complete”` sebagai hasil switch.
                "financial_goal_progress_percent" or "goal_ambition" or "goal_ambition_index" when numericValue >= 99.5 => "players.support.guide.goals_complete",
                // Untuk pola `”financial_goal_progress_percent” or ”goal_ambition” or ”goal_ambition_index”` dengan syarat tambahan `numericValue >= 67`,
                // menghasilkan nilai literal `”players.support.guide.goals_strong”` sebagai hasil switch.
                "financial_goal_progress_percent" or "goal_ambition" or "goal_ambition_index" when numericValue >= 67 => "players.support.guide.goals_strong",
                // Untuk pola `”financial_goal_progress_percent” or ”goal_ambition” or ”goal_ambition_index”` dengan syarat tambahan `numericValue < 34`,
                // menghasilkan nilai literal `”players.support.guide.goals_limited”` sebagai hasil switch.
                "financial_goal_progress_percent" or "goal_ambition" or "goal_ambition_index" when numericValue < 34 => "players.support.guide.goals_limited",
                // Untuk pola `”financial_goal_progress_percent” or ”goal_ambition” or ”goal_ambition_index”`, menghasilkan nilai literal
                // `”players.support.guide.goals_moderate”` sebagai hasil switch.
                "financial_goal_progress_percent" or "goal_ambition" or "goal_ambition_index" => "players.support.guide.goals_moderate",
                // Untuk pola `”loan_repayment_discipline”` dengan syarat tambahan `numericValue >= 99.5`, menghasilkan nilai literal
                // `”players.support.guide.loan_repaid”` sebagai hasil switch.
                "loan_repayment_discipline" when numericValue >= 99.5 => "players.support.guide.loan_repaid",
                // Untuk pola `”loan_repayment_discipline”`, menghasilkan nilai literal `”players.support.guide.loan_remaining”` sebagai hasil switch.
                "loan_repayment_discipline" => "players.support.guide.loan_remaining",
                // Untuk pola `”income_action_focus_percent” or ”action_efficiency_percent”` dengan syarat tambahan `numericValue > 60`, menghasilkan nilai literal
                // `”players.support.guide.action_income”` sebagai hasil switch.
                "income_action_focus_percent" or "action_efficiency_percent" when numericValue > 60 => "players.support.guide.action_income",
                // Untuk pola `”income_action_focus_percent” or ”action_efficiency_percent”` dengan syarat tambahan `numericValue < 40`, menghasilkan nilai literal
                // `”players.support.guide.action_exploration”` sebagai hasil switch.
                "income_action_focus_percent" or "action_efficiency_percent" when numericValue < 40 => "players.support.guide.action_exploration",
                // Untuk pola `”income_action_focus_percent” or ”action_efficiency_percent”`, menghasilkan nilai literal `”players.support.guide.action_balanced”`
                // sebagai hasil switch.
                "income_action_focus_percent" or "action_efficiency_percent" => "players.support.guide.action_balanced",
                // Untuk pola `”ingredient_utilization_percent”` dengan syarat tambahan `numericValue >= 80`, menghasilkan nilai literal
                // `”players.support.guide.ingredient_use_high”` sebagai hasil switch.
                "ingredient_utilization_percent" when numericValue >= 80 => "players.support.guide.ingredient_use_high",
                // Untuk pola `”ingredient_utilization_percent”` dengan syarat tambahan `numericValue < 60`, menghasilkan nilai literal
                // `”players.support.guide.ingredient_use_low”` sebagai hasil switch.
                "ingredient_utilization_percent" when numericValue < 60 => "players.support.guide.ingredient_use_low",
                // Untuk pola `”ingredient_utilization_percent”`, menghasilkan nilai literal `”players.support.guide.ingredient_use_moderate”` sebagai hasil switch.
                "ingredient_utilization_percent" => "players.support.guide.ingredient_use_moderate",
                // Untuk pola `”meal_order_success_rate”` dengan syarat tambahan `numericValue >= 80`, menghasilkan nilai literal
                // `”players.support.guide.orders_strong”` sebagai hasil switch.
                "meal_order_success_rate" when numericValue >= 80 => "players.support.guide.orders_strong",
                // Untuk pola `”meal_order_success_rate”` dengan syarat tambahan `numericValue < 60`, menghasilkan nilai literal
                // `”players.support.guide.orders_review”` sebagai hasil switch.
                "meal_order_success_rate" when numericValue < 60 => "players.support.guide.orders_review",
                // Untuk pola `”meal_order_success_rate”`, menghasilkan nilai literal `”players.support.guide.orders_moderate”` sebagai hasil switch.
                "meal_order_success_rate" => "players.support.guide.orders_moderate",
                // Untuk pola `”long_term_action_share_percent” or ”planning_horizon_percent”` dengan syarat tambahan `numericValue > 40`, menghasilkan nilai
                // literal `”players.support.guide.planning_long”` sebagai hasil switch.
                "long_term_action_share_percent" or "planning_horizon_percent" when numericValue > 40 => "players.support.guide.planning_long",
                // Untuk pola `”long_term_action_share_percent” or ”planning_horizon_percent”` dengan syarat tambahan `numericValue < 20`, menghasilkan nilai
                // literal `”players.support.guide.planning_short”` sebagai hasil switch.
                "long_term_action_share_percent" or "planning_horizon_percent" when numericValue < 20 => "players.support.guide.planning_short",
                // Untuk pola `”long_term_action_share_percent” or ”planning_horizon_percent”`, menghasilkan nilai literal
                // `”players.support.guide.planning_balanced”` sebagai hasil switch.
                "long_term_action_share_percent" or "planning_horizon_percent" => "players.support.guide.planning_balanced",
                // Untuk pola `”planning_horizon”` dengan syarat tambahan `normalizedRatio > 0.4`, menghasilkan nilai literal
                // `”players.support.guide.planning_long”` sebagai hasil switch.
                "planning_horizon" when normalizedRatio > 0.4 => "players.support.guide.planning_long",
                // Untuk pola `”planning_horizon”` dengan syarat tambahan `normalizedRatio < 0.2`, menghasilkan nilai literal
                // `”players.support.guide.planning_short”` sebagai hasil switch.
                "planning_horizon" when normalizedRatio < 0.2 => "players.support.guide.planning_short",
                // Untuk pola `”planning_horizon”`, menghasilkan nilai literal `”players.support.guide.planning_balanced”` sebagai hasil switch.
                "planning_horizon" => "players.support.guide.planning_balanced",
                // Untuk pola `”need_fulfillment_diversity_percent”` dengan syarat tambahan `numericValue >= 67`, menghasilkan nilai literal
                // `”players.support.guide.need_balance_high”` sebagai hasil switch.
                "need_fulfillment_diversity_percent" when numericValue >= 67 => "players.support.guide.need_balance_high",
                // Untuk pola `”need_fulfillment_diversity_percent”` dengan syarat tambahan `numericValue < 34`, menghasilkan nilai literal
                // `”players.support.guide.need_balance_low”` sebagai hasil switch.
                "need_fulfillment_diversity_percent" when numericValue < 34 => "players.support.guide.need_balance_low",
                // Untuk pola `”need_fulfillment_diversity_percent”`, menghasilkan nilai literal `”players.support.guide.need_balance_moderate”` sebagai hasil
                // switch.
                "need_fulfillment_diversity_percent" => "players.support.guide.need_balance_moderate",
                // Untuk pola `”fulfillment_diversity”` dengan syarat tambahan `numericValue >= 0.67`, menghasilkan nilai literal
                // `”players.support.guide.need_balance_high”` sebagai hasil switch.
                "fulfillment_diversity" when numericValue >= 0.67 => "players.support.guide.need_balance_high",
                // Untuk pola `”fulfillment_diversity”` dengan syarat tambahan `numericValue < 0.34`, menghasilkan nilai literal
                // `”players.support.guide.need_balance_low”` sebagai hasil switch.
                "fulfillment_diversity" when numericValue < 0.34 => "players.support.guide.need_balance_low",
                // Untuk pola `”fulfillment_diversity”`, menghasilkan nilai literal `”players.support.guide.need_balance_moderate”` sebagai hasil switch.
                "fulfillment_diversity" => "players.support.guide.need_balance_moderate",
                // Untuk pola `”growth_pattern_ratio”` dengan syarat tambahan `numericValue > 3`, menghasilkan nilai literal `”players.support.guide.growth_strong”`
                // sebagai hasil switch.
                "growth_pattern_ratio" when numericValue > 3 => "players.support.guide.growth_strong",
                // Untuk pola `”growth_pattern_ratio”` dengan syarat tambahan `numericValue < 1`, menghasilkan nilai literal
                // `”players.support.guide.growth_declining”` sebagai hasil switch.
                "growth_pattern_ratio" when numericValue < 1 => "players.support.guide.growth_declining",
                // Untuk pola `”growth_pattern_ratio”`, menghasilkan nilai literal `”players.support.guide.growth_steady”` sebagai hasil switch.
                "growth_pattern_ratio" => "players.support.guide.growth_steady",
                // Untuk pola `”donation_aggressiveness_percent”` dengan syarat tambahan `numericValue > 30`, menghasilkan nilai literal
                // `”players.support.guide.donation_aggressive”` sebagai hasil switch.
                "donation_aggressiveness_percent" when numericValue > 30 => "players.support.guide.donation_aggressive",
                // Untuk pola `”donation_aggressiveness_percent”` dengan syarat tambahan `numericValue < 10`, menghasilkan nilai literal
                // `”players.support.guide.donation_conservative”` sebagai hasil switch.
                "donation_aggressiveness_percent" when numericValue < 10 => "players.support.guide.donation_conservative",
                // Untuk pola `”donation_aggressiveness_percent”`, menghasilkan nilai literal `”players.support.guide.donation_moderate”` sebagai hasil switch.
                "donation_aggressiveness_percent" => "players.support.guide.donation_moderate",
                // Untuk pola `”donation_stability_std_deviation”` dengan syarat tambahan `numericValue <= 1`, menghasilkan nilai literal
                // `”players.support.guide.donation_stable”` sebagai hasil switch.
                "donation_stability_std_deviation" when numericValue <= 1 => "players.support.guide.donation_stable",
                // Untuk pola `”donation_stability_std_deviation”`, menghasilkan nilai literal `”players.support.guide.donation_variable”` sebagai hasil switch.
                "donation_stability_std_deviation" => "players.support.guide.donation_variable",
                // Untuk pola `”donation_commitment_score”` dengan syarat tambahan `numericValue >= 67`, menghasilkan nilai literal
                // `”players.support.guide.donation_commitment_strong”` sebagai hasil switch.
                "donation_commitment_score" when numericValue >= 67 => "players.support.guide.donation_commitment_strong",
                // Untuk pola `”donation_commitment_score”` dengan syarat tambahan `numericValue < 34`, menghasilkan nilai literal
                // `”players.support.guide.donation_commitment_weak”` sebagai hasil switch.
                "donation_commitment_score" when numericValue < 34 => "players.support.guide.donation_commitment_weak",
                // Untuk pola `”donation_commitment_score”`, menghasilkan nilai literal `”players.support.guide.donation_commitment_moderate”` sebagai hasil switch.
                "donation_commitment_score" => "players.support.guide.donation_commitment_moderate",
                // Untuk pola `”mission_achievement”` dengan syarat tambahan `numericValue >= 1`, menghasilkan nilai literal
                // `”players.support.guide.mission_complete”` sebagai hasil switch.
                "mission_achievement" when numericValue >= 1 => "players.support.guide.mission_complete",
                // Untuk pola `”mission_achievement”`, menghasilkan nilai literal `”players.support.guide.mission_incomplete”` sebagai hasil switch.
                "mission_achievement" => "players.support.guide.mission_incomplete",
                // Untuk pola `”friday_participation_rate”` dengan syarat tambahan `normalizedRatio >= 0.8`, menghasilkan nilai literal
                // `”players.support.guide.participation_high”` sebagai hasil switch.
                "friday_participation_rate" when normalizedRatio >= 0.8 => "players.support.guide.participation_high",
                // Untuk pola `”friday_participation_rate”`, menghasilkan nilai literal `”players.support.guide.participation_partial”` sebagai hasil switch.
                "friday_participation_rate" => "players.support.guide.participation_partial",
                // Untuk pola `_`, menghasilkan `string.Empty`, yaitu nilai kosong bawaan tipe terkait sebagai hasil switch.
                _ => string.Empty
            } is { Length: > 0 } specificKey
                // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: specificKey dalam ResolveGuidanceKey.
                ? specificKey
                // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: ResolveGenericGuidanceKey(metricKey, isDerived, unitKey); dalam
                // ResolveGuidanceKey.
                : ResolveGenericGuidanceKey(metricKey, isDerived, unitKey);
        }

        return ResolveGenericGuidanceKey(metricKey, isDerived, unitKey);
    }

    private static string ResolveGenericGuidanceKey(string metricKey, bool isDerived, string unitKey)
    {
        var functionCategory = ResolveFunctionCategory(metricKey);
        if (!string.IsNullOrWhiteSpace(functionCategory))
        {
            return $"players.support.guide.{functionCategory}";
        }

        if (isDerived)
        {
            return unitKey switch
            {
                // Untuk pola `”players.support.unit.percent”`, menghasilkan nilai literal `”players.support.guide.percent”` sebagai hasil switch.
                "players.support.unit.percent" => "players.support.guide.percent",
                // Untuk pola `”players.support.unit.multiplier”`, menghasilkan nilai literal `”players.support.guide.ratio”` sebagai hasil switch.
                "players.support.unit.multiplier" => "players.support.guide.ratio",
                // Untuk pola `_`, menghasilkan nilai literal `”players.support.guide.derived”` sebagai hasil switch.
                _ => "players.support.guide.derived"
            };
        }

        return unitKey switch
        {
            // Untuk pola `”players.support.unit.coins”`, menghasilkan nilai literal `”players.support.guide.coins”` sebagai hasil switch.
            "players.support.unit.coins" => "players.support.guide.coins",
            // Untuk pola `”players.support.unit.points”`, menghasilkan nilai literal `”players.support.guide.points”` sebagai hasil switch.
            "players.support.unit.points" => "players.support.guide.points",
            // Untuk pola `”players.support.unit.rank”`, menghasilkan nilai literal `”players.support.guide.rank”` sebagai hasil switch.
            "players.support.unit.rank" => "players.support.guide.rank",
            // Untuk pola `”players.support.unit.day” or ”players.support.unit.turn” or ”players.support.unit.action_slot”`, menghasilkan nilai literal
            // `”players.support.guide.turn”` sebagai hasil switch.
            "players.support.unit.day" or "players.support.unit.turn" or "players.support.unit.action_slot" => "players.support.guide.turn",
            // Untuk pola `_` dengan syarat tambahan `unitKey.StartsWith(”players.support.unit.”, StringComparison.Ordinal)`, menghasilkan nilai literal
            // `”players.support.guide.count”` sebagai hasil switch.
            _ when unitKey.StartsWith("players.support.unit.", StringComparison.Ordinal) => "players.support.guide.count",
            // Untuk pola `_`, menghasilkan nilai literal `”players.support.guide.raw”` sebagai hasil switch.
            _ => "players.support.guide.raw"
        };
    }

    private static string ResolveFunctionCategory(string metricKey)
    {
        return metricKey switch
        {
            // Untuk pola `”starting_coins”`, menghasilkan nilai literal `”baseline”` sebagai hasil switch.
            "starting_coins" => "baseline",
            // Untuk pola `”cash_in_total” or ”coins_earned_total” or ”freelance_income” or ”meal_income” or ”gold_income” or ”donations_received” or
            // ”other_income” or ”total_income”`, menghasilkan nilai literal `”income”` sebagai hasil switch.
            "cash_in_total" or "coins_earned_total" or "freelance_income" or "meal_income" or
                "gold_income" or "donations_received" or "other_income" or "total_income" => "income",
            // Untuk pola `”cash_out_total” or ”coins_spent_total” or ”essential_expenses” or ”total_expenses”`, menghasilkan nilai literal `”spending”` sebagai
            // hasil switch.
            "cash_out_total" or "coins_spent_total" or "essential_expenses" or "total_expenses" => "spending",
            // Untuk pola `”coins_held_current” or ”coins_net_end” or ”coins_net_end_game” or ”cash_net_total”`, menghasilkan nilai literal `”cash_position”`
            // sebagai hasil switch.
            "coins_held_current" or "coins_net_end" or "coins_net_end_game" or "cash_net_total" => "cash_position",
            // Untuk pola `”coins_saved” or ”coins_in_savings_goal”`, menghasilkan nilai literal `”savings”` sebagai hasil switch.
            "coins_saved" or "coins_in_savings_goal" => "savings",
            // Untuk pola `”mission_bonus_pts” or ”total_happiness_points” or ”final_rank” or ”winner_flag” or ”finish_line_reached” or ”dnf_flag” or ”value”`,
            // menghasilkan nilai literal `”outcome”` sebagai hasil switch.
            "mission_bonus_pts" or "total_happiness_points" or "final_rank" or "winner_flag" or
                "finish_line_reached" or "dnf_flag" or "value" => "outcome",
            // Untuk pola `”game_id” or ”session_id” or ”user_id” or ”player_alias” or ”game_mode” or ”seed_source”`, menghasilkan nilai literal
            // `”session_context”` sebagai hasil switch.
            "game_id" or "session_id" or "user_id" or "player_alias" or "game_mode" or "seed_source" => "session_context",
            // Untuk pola `_` dengan syarat tambahan `metricKey.Contains(”ingredient”, StringComparison.OrdinalIgnoreCase)`, menghasilkan nilai literal
            // `”inventory”` sebagai hasil switch.
            _ when metricKey.Contains("ingredient", StringComparison.OrdinalIgnoreCase) => "inventory",
            // Untuk pola `_` dengan syarat tambahan `metricKey.Contains(”meal_order”, StringComparison.OrdinalIgnoreCase) || metricKey.Contains(”business”,
            // StringComparison.OrdinalIgnoreCase)`, menghasilkan nilai literal `”business_activity”` sebagai hasil switch.
            _ when metricKey.Contains("meal_order", StringComparison.OrdinalIgnoreCase) ||
                   metricKey.Contains("business", StringComparison.OrdinalIgnoreCase) => "business_activity",
            // Untuk pola `_` dengan syarat tambahan `metricKey.Contains(”need”, StringComparison.OrdinalIgnoreCase) || metricKey.Contains(”fulfillment”,
            // StringComparison.OrdinalIgnoreCase) || metricKey is ”collection_mission_com...`, menghasilkan nilai literal `”needs”` sebagai hasil switch.
            _ when metricKey.Contains("need", StringComparison.OrdinalIgnoreCase) ||
                   metricKey.Contains("fulfillment", StringComparison.OrdinalIgnoreCase) ||
                   metricKey is "collection_mission_complete" or "specific_tertiary_need" or "mission_achievement" or
                       "p_primary" or "p_secondary" or "p_tertiary" => "needs",
            // Untuk pola `_` dengan syarat tambahan `metricKey.Contains(”donat”, StringComparison.OrdinalIgnoreCase) || metricKey.Contains(”friday”,
            // StringComparison.OrdinalIgnoreCase)`, menghasilkan nilai literal `”donation”` sebagai hasil switch.
            _ when metricKey.Contains("donat", StringComparison.OrdinalIgnoreCase) ||
                   metricKey.Contains("friday", StringComparison.OrdinalIgnoreCase) => "donation",
            // Untuk pola `_` dengan syarat tambahan `metricKey.Contains(”per_turn”, StringComparison.OrdinalIgnoreCase) || metricKey.Contains(”action”,
            // StringComparison.OrdinalIgnoreCase) || metricKey.Contains(”event”, StringCom...`, menghasilkan nilai literal `”timeline”` sebagai hasil switch.
            _ when metricKey.Contains("per_turn", StringComparison.OrdinalIgnoreCase) ||
                   metricKey.Contains("action", StringComparison.OrdinalIgnoreCase) ||
                   metricKey.Contains("event", StringComparison.OrdinalIgnoreCase) ||
                   metricKey.Contains("transaction", StringComparison.OrdinalIgnoreCase) ||
                   metricKey.Contains("sequence", StringComparison.OrdinalIgnoreCase) ||
                   metricKey.StartsWith("day_", StringComparison.OrdinalIgnoreCase) ||
                   metricKey.Contains("_day_", StringComparison.OrdinalIgnoreCase) ||
                   metricKey.Contains("timestamp", StringComparison.OrdinalIgnoreCase) => "timeline",
            // Untuk pola `_` dengan syarat tambahan `metricKey.Contains(”gold”, StringComparison.OrdinalIgnoreCase)`, menghasilkan nilai literal
            // `”gold_investment”` sebagai hasil switch.
            _ when metricKey.Contains("gold", StringComparison.OrdinalIgnoreCase) => "gold_investment",
            // Untuk pola `_` dengan syarat tambahan `metricKey.Contains(”pension”, StringComparison.OrdinalIgnoreCase) || metricKey ==
            // ”leftover_coins_end_game”`, menghasilkan nilai literal `”pension”` sebagai hasil switch.
            _ when metricKey.Contains("pension", StringComparison.OrdinalIgnoreCase) ||
                   metricKey == "leftover_coins_end_game" => "pension",
            // Untuk pola `_` dengan syarat tambahan `metricKey.Contains(”risk”, StringComparison.OrdinalIgnoreCase) || metricKey.Contains(”insurance”,
            // StringComparison.OrdinalIgnoreCase) || metricKey.Contains(”emergency”, String...`, menghasilkan nilai literal `”risk”` sebagai hasil switch.
            _ when metricKey.Contains("risk", StringComparison.OrdinalIgnoreCase) ||
                   metricKey.Contains("insurance", StringComparison.OrdinalIgnoreCase) ||
                   metricKey.Contains("emergency", StringComparison.OrdinalIgnoreCase) => "risk",
            // Untuk pola `_` dengan syarat tambahan `metricKey.Contains(”financial_goal”, StringComparison.OrdinalIgnoreCase) || metricKey.StartsWith(”goal_”,
            // StringComparison.OrdinalIgnoreCase)`, menghasilkan nilai literal `”goals”` sebagai hasil switch.
            _ when metricKey.Contains("financial_goal", StringComparison.OrdinalIgnoreCase) ||
                   metricKey.StartsWith("goal_", StringComparison.OrdinalIgnoreCase) => "goals",
            // Untuk pola `_` dengan syarat tambahan `metricKey.Contains(”loan”, StringComparison.OrdinalIgnoreCase) || metricKey.Contains(”debt”,
            // StringComparison.OrdinalIgnoreCase)`, menghasilkan nilai literal `”debt”` sebagai hasil switch.
            _ when metricKey.Contains("loan", StringComparison.OrdinalIgnoreCase) ||
                   metricKey.Contains("debt", StringComparison.OrdinalIgnoreCase) => "debt",
            // Untuk pola `_` dengan syarat tambahan `metricKey.Contains(”point”, StringComparison.OrdinalIgnoreCase) || metricKey.EndsWith(”_pts”,
            // StringComparison.OrdinalIgnoreCase) || metricKey.Contains(”rank”, StringCompariso...`, menghasilkan nilai literal `”outcome”` sebagai hasil
            // switch.
            _ when metricKey.Contains("point", StringComparison.OrdinalIgnoreCase) ||
                   metricKey.EndsWith("_pts", StringComparison.OrdinalIgnoreCase) ||
                   metricKey.Contains("rank", StringComparison.OrdinalIgnoreCase) => "outcome",
            // Untuk pola `_`, menghasilkan `string.Empty`, yaitu nilai kosong bawaan tipe terkait sebagai hasil switch.
            _ => string.Empty
        };
    }

    private static string ResolveRecommendationKey(string metricKey, double numericValue, bool hasNumericValue)
    {
        if (hasNumericValue)
        {
            var resultSpecificKey = metricKey switch
            {
                // Untuk pola `”cash_growth_percent” or ”net_worth_index”` dengan syarat tambahan `numericValue < 100`, menghasilkan nilai literal
                // `”players.support.recommendation.cash_recover”` sebagai hasil switch.
                "cash_growth_percent" or "net_worth_index" when numericValue < 0 => "players.support.recommendation.cash_recover",
                // Untuk pola `”income_diversification_index” or ”income_diversification_ratio”` dengan syarat tambahan `numericValue < 34`, menghasilkan nilai
                // literal `”players.support.recommendation.income_mix_recover”` sebagai hasil switch.
                "income_diversification_index" or "income_diversification_ratio" when numericValue < 34 => "players.support.recommendation.income_mix_recover",
                // Untuk pola `”meal_order_profit_margin_percent” or ”business_profit_margin”` dengan syarat tambahan `numericValue <= 0`, menghasilkan nilai
                // literal `”players.support.recommendation.business_recover”` sebagai hasil switch.
                "meal_order_profit_margin_percent" or "business_profit_margin" when numericValue <= 0 => "players.support.recommendation.business_recover",
                // Untuk pola `”risk_appetite_score” or ”risk_appetite_score_normalized”` dengan syarat tambahan `numericValue > 75`, menghasilkan nilai literal
                // `”players.support.recommendation.risk_reduce”` sebagai hasil switch.
                "risk_appetite_score" or "risk_appetite_score_normalized" when numericValue > 75 => "players.support.recommendation.risk_reduce",
                // Untuk pola `”loan_burden_percent” or ”debt_leverage_ratio”` dengan syarat tambahan `numericValue > 75`, menghasilkan nilai literal
                // `”players.support.recommendation.debt_reduce”` sebagai hasil switch.
                "loan_burden_percent" or "debt_leverage_ratio" when numericValue > 75 => "players.support.recommendation.debt_reduce",
                // Untuk pola `”financial_goal_progress_percent” or ”goal_ambition” or ”goal_ambition_index”` dengan syarat tambahan `numericValue < 50`,
                // menghasilkan nilai literal `”players.support.recommendation.goals_focus”` sebagai hasil switch.
                "financial_goal_completion_percent" when numericValue < 100 => "players.support.recommendation.goals_focus",
                "financial_goal_progress_percent" or "goal_ambition" or "goal_ambition_index" when numericValue < 50 => "players.support.recommendation.goals_focus",
                // Untuk pola `”income_action_focus_percent” or ”action_efficiency_percent”` dengan syarat tambahan `numericValue < 40`, menghasilkan nilai literal
                // `”players.support.recommendation.action_income”` sebagai hasil switch.
                "income_action_focus_percent" or "action_efficiency_percent" when numericValue < 40 => "players.support.recommendation.action_income",
                // Untuk pola `”ingredient_utilization_percent” or ”meal_order_success_rate”` dengan syarat tambahan `numericValue < 60`, menghasilkan nilai literal
                // `”players.support.recommendation.orders_improve”` sebagai hasil switch.
                "ingredient_utilization_percent" or "meal_order_success_rate" when numericValue < 60 => "players.support.recommendation.orders_improve",
                // Untuk pola `”long_term_action_share_percent” or ”planning_horizon_percent”` dengan syarat tambahan `numericValue < 20`, menghasilkan nilai
                // literal `”players.support.recommendation.planning_build”` sebagai hasil switch.
                "long_term_action_share_percent" or "planning_horizon_percent" when numericValue < 20 => "players.support.recommendation.planning_build",
                // Untuk pola `”planning_horizon”` dengan syarat tambahan `numericValue < 0.2`, menghasilkan nilai literal
                // `”players.support.recommendation.planning_build”` sebagai hasil switch.
                "planning_horizon" when numericValue < 0.2 => "players.support.recommendation.planning_build",
                // Untuk pola `”need_fulfillment_diversity_percent”` dengan syarat tambahan `numericValue < 67`, menghasilkan nilai literal
                // `”players.support.recommendation.needs_balance”` sebagai hasil switch.
                "need_fulfillment_diversity_percent" when numericValue < 67 => "players.support.recommendation.needs_balance",
                // Untuk pola `”fulfillment_diversity”` dengan syarat tambahan `numericValue < 0.67`, menghasilkan nilai literal
                // `”players.support.recommendation.needs_balance”` sebagai hasil switch.
                "fulfillment_diversity" when numericValue < 0.67 => "players.support.recommendation.needs_balance",
                // Untuk pola `”donation_commitment_score”` dengan syarat tambahan `numericValue < 34`, menghasilkan nilai literal
                // `”players.support.recommendation.donation_stabilize”` sebagai hasil switch.
                "donation_commitment_score" when numericValue < 34 => "players.support.recommendation.donation_stabilize",
                // Untuk pola `”total_happiness_pts”` dengan syarat tambahan `numericValue < 0`, menghasilkan nilai literal
                // `”players.support.recommendation.outcome_recover”` sebagai hasil switch.
                "total_happiness_pts" when numericValue < 0 => "players.support.recommendation.outcome_recover",
                // Untuk pola `_`, menghasilkan `string.Empty`, yaitu nilai kosong bawaan tipe terkait sebagai hasil switch.
                _ => string.Empty
            };

            if (!string.IsNullOrWhiteSpace(resultSpecificKey))
            {
                return resultSpecificKey;
            }
        }

        var recommendationCategory = metricKey switch
        {
            // Untuk pola `”cash_growth_percent” or ”net_worth_index” or ”growth_pattern_ratio”`, menghasilkan nilai literal `”wealth”` sebagai hasil switch.
            "cash_growth_percent" or "net_worth_index" or "growth_pattern_ratio" => "wealth",
            "happiness_source_diversity_percent" => "happiness_diversity",
            // Untuk pola `”income_diversification_index” or ”income_diversification_ratio” or ”income_share_i” or ”n_active_income_sources”`, menghasilkan
            // nilai literal `”income_mix”` sebagai hasil switch.
            "income_diversification_index" or "income_diversification_ratio" or "income_share_i" or
                "n_active_income_sources" => "income_mix",
            // Untuk pola `”business_expense_share_percent” or ”expense_management_efficiency”`, menghasilkan nilai literal `”productive_spending”` sebagai
            // hasil switch.
            "business_expense_share_percent" or "expense_management_efficiency" => "productive_spending",
            // Untuk pola `”risk_readiness_percent” or ”risk_mitigation_effectiveness” or ”insurance_activation_rate” or ”insurance_coverage_rate”`,
            // menghasilkan nilai literal `”protection”` sebagai hasil switch.
            "risk_readiness_percent" or "risk_mitigation_effectiveness" or "insurance_activation_rate" or "insurance_coverage_rate" => "protection",
            // Untuk pola `”income_action_focus_percent” or ”action_efficiency” or ”action_efficiency_percent”`, menghasilkan nilai literal `”action_balance”`
            // sebagai hasil switch.
            "income_action_focus_percent" or "action_efficiency" or "action_efficiency_percent" => "action_balance",
            // Untuk pola `”long_term_action_share_percent” or ”planning_horizon” or ”planning_horizon_percent”`, menghasilkan nilai literal `”planning”`
            // sebagai hasil switch.
            "long_term_action_share_percent" or "planning_horizon" or "planning_horizon_percent" => "planning",
            // Untuk pola `_`, menghasilkan memanggil `ResolveFunctionCategory` dengan `metricKey` sebagai hasil switch.
            _ => ResolveFunctionCategory(metricKey)
        };

        return string.IsNullOrWhiteSpace(recommendationCategory)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: ”players.support.recommendation.balance” dalam
            // ResolveRecommendationKey.
            ? "players.support.recommendation.balance"
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: $”players.support.recommendation.{recommendationCategory}”; dalam
            // ResolveRecommendationKey.
            : $"players.support.recommendation.{recommendationCategory}";
    }

    private static string ResolveFormulaKey(string metricKey)
    {
        return metricKey switch
        {
            // Untuk pola `”cash_growth_percent” or ”net_worth_index”`, menghasilkan nilai literal `”players.support.formula.net_worth”` sebagai hasil switch.
            "cash_growth_percent" or "net_worth_index" => "players.support.formula.net_worth",
            "happiness_source_diversity_percent" => "players.support.formula.happiness_portfolio",
            // Untuk pola `”income_diversification_index”`, menghasilkan nilai literal `”players.support.formula.income_diversification_index”` sebagai hasil
            // switch.
            "income_diversification_index" => "players.support.formula.income_diversification_index",
            // Untuk pola `”income_diversification_ratio”`, menghasilkan nilai literal `”players.support.formula.income_diversification”` sebagai hasil switch.
            "income_diversification_ratio" => "players.support.formula.income_diversification",
            // Untuk pola `”business_expense_share_percent” or ”expense_management_efficiency”`, menghasilkan nilai literal
            // `”players.support.formula.expense_efficiency”` sebagai hasil switch.
            "business_expense_share_percent" or "expense_management_efficiency" => "players.support.formula.expense_efficiency",
            // Untuk pola `”meal_order_profit_margin_percent” or ”business_profit_margin”`, menghasilkan nilai literal
            // `”players.support.formula.business_margin”` sebagai hasil switch.
            "meal_order_profit_margin_percent" or "business_profit_margin" => "players.support.formula.business_margin",
            // Untuk pola `”business_efficiency_ratio”`, menghasilkan nilai literal `”players.support.formula.business_efficiency”` sebagai hasil switch.
            "business_efficiency_ratio" => "players.support.formula.business_efficiency",
            // Untuk pola `”gold_roi_percentage”`, menghasilkan nilai literal `”players.support.formula.gold_roi”` sebagai hasil switch.
            "gold_roi_percentage" => "players.support.formula.gold_roi",
            // Untuk pola `”risk_exposure_percentage”`, menghasilkan nilai literal `”players.support.formula.risk_exposure”` sebagai hasil switch.
            "risk_exposure_percentage" => "players.support.formula.risk_exposure",
            // Untuk pola `”risk_mitigation_effectiveness”`, menghasilkan nilai literal `”players.support.formula.risk_mitigation”` sebagai hasil switch.
            "risk_mitigation_effectiveness" => "players.support.formula.risk_mitigation",
            // Untuk pola `”risk_readiness_percent” or ”risk_appetite_score” or ”risk_appetite_score_normalized”`, menghasilkan nilai literal
            // `”players.support.formula.risk_appetite”` sebagai hasil switch.
            "risk_readiness_percent" or "risk_appetite_score" or "risk_appetite_score_normalized" => "players.support.formula.risk_appetite",
            // Untuk pola `”loan_burden_percent” or ”debt_leverage_ratio”`, menghasilkan nilai literal `”players.support.formula.debt_leverage”` sebagai hasil
            // switch.
            "loan_burden_percent" or "debt_leverage_ratio" => "players.support.formula.debt_leverage",
            "sharia_loans_taken" or "sharia_loans_repaid" or "sharia_loans_unpaid_end" => "players.support.formula.loan_counts",
            "financial_goal_completion_percent" => "players.support.formula.goal_completion",
            // Untuk pola `”loan_repayment_discipline”`, menghasilkan nilai literal `”players.support.formula.loan_discipline”` sebagai hasil switch.
            "loan_repayment_discipline" => "players.support.formula.loan_discipline",
            // Untuk pola `”debt_ratio”`, menghasilkan nilai literal `”players.support.formula.debt_ratio”` sebagai hasil switch.
            "debt_ratio" => "players.support.formula.debt_ratio",
            // Untuk pola `”financial_goal_progress_percent” or ”goal_ambition” or ”goal_ambition_index”`, menghasilkan nilai literal
            // `”players.support.formula.goal_ambition_index”` sebagai hasil switch.
            "financial_goal_progress_percent" or "goal_ambition" or "goal_ambition_index" => "players.support.formula.goal_ambition_index",
            // Untuk pola `”goal_setting_ambition”`, menghasilkan nilai literal `”players.support.formula.goal_ambition”` sebagai hasil switch.
            "goal_setting_ambition" => "players.support.formula.goal_ambition",
            // Untuk pola `”income_action_focus_percent” or ”action_efficiency” or ”action_efficiency_percent”`, menghasilkan nilai literal
            // `”players.support.formula.action_efficiency”` sebagai hasil switch.
            "income_action_focus_percent" or "action_efficiency" or "action_efficiency_percent" => "players.support.formula.action_efficiency",
            // Untuk pola `”ingredient_utilization_percent” or ”meal_order_success_rate”`, menghasilkan nilai literal `”players.support.formula.order_success”`
            // sebagai hasil switch.
            "ingredient_utilization_percent" or "meal_order_success_rate" => "players.support.formula.order_success",
            // Untuk pola `”long_term_action_share_percent” or ”planning_horizon” or ”planning_horizon_percent”`, menghasilkan nilai literal
            // `”players.support.formula.planning”` sebagai hasil switch.
            "long_term_action_share_percent" or "planning_horizon" or "planning_horizon_percent" => "players.support.formula.planning",
            // Untuk pola `”need_fulfillment_diversity_percent” or ”fulfillment_diversity”`, menghasilkan nilai literal `”players.support.formula.fulfillment”`
            // sebagai hasil switch.
            "need_fulfillment_diversity_percent" or "fulfillment_diversity" => "players.support.formula.fulfillment",
            // Untuk pola `”fulfillment_diversity_document_formula”`, menghasilkan nilai literal `”players.support.formula.fulfillment_document”` sebagai hasil
            // switch.
            "fulfillment_diversity_document_formula" => "players.support.formula.fulfillment_document",
            // Untuk pola `”growth_pattern_ratio”`, menghasilkan nilai literal `”players.support.formula.growth”` sebagai hasil switch.
            "growth_pattern_ratio" => "players.support.formula.growth",
            // Untuk pola `”donation_aggressiveness_percent”`, menghasilkan nilai literal `”players.support.formula.donation_aggressiveness”` sebagai hasil
            // switch.
            "donation_aggressiveness_percent" => "players.support.formula.donation_aggressiveness",
            // Untuk pola `”donation_stability” or ”donation_stability_std_deviation”`, menghasilkan nilai literal
            // `”players.support.formula.donation_stability”` sebagai hasil switch.
            "donation_stability" or "donation_stability_std_deviation" => "players.support.formula.donation_stability",
            // Untuk pola `”donation_stability_index”`, menghasilkan nilai literal `”players.support.formula.donation_stability_index”` sebagai hasil switch.
            "donation_stability_index" => "players.support.formula.donation_stability_index",
            // Untuk pola `”donation_commitment_score”`, menghasilkan nilai literal `”players.support.formula.donation_commitment”` sebagai hasil switch.
            "donation_commitment_score" => "players.support.formula.donation_commitment",
            // Untuk pola `_`, menghasilkan `string.Empty`, yaitu nilai kosong bawaan tipe terkait sebagai hasil switch.
            _ => string.Empty
        };
    }

    private static string FormatDisplayNumber(double value)
    {
        return Math.Abs(value - Math.Round(value)) < 0.0000001
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: value.ToString(”N0”, CultureInfo.CurrentCulture) dalam
            // FormatDisplayNumber.
            ? value.ToString("N0", CultureInfo.CurrentCulture)
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: value.ToString(”N2”, CultureInfo.CurrentCulture); dalam
            // FormatDisplayNumber.
            : value.ToString("N2", CultureInfo.CurrentCulture);
    }

    private static string NormalizeMetricLexiconKey(string key)
    {
        var trimmed = key.Trim();
        var builder = new StringBuilder(trimmed.Length);

        for (var index = 0; index < trimmed.Length; index++)
        {
            var current = trimmed[index];
            if (char.IsLetterOrDigit(current))
            {
                if (builder.Length > 0 &&
                    char.IsUpper(current) &&
                    ShouldInsertWordBoundary(trimmed, index))
                {
                    AppendSeparator(builder);
                }

                builder.Append(char.ToLowerInvariant(current));
                // Melewati sisa pernyataan pada iterasi saat ini dan melanjutkan ke elemen/iterasi berikutnya dalam NormalizeMetricLexiconKey.
                continue;
            }

            AppendSeparator(builder);
        }

        return builder.ToString().Trim('_');
    }

    private static bool ShouldInsertWordBoundary(string value, int index)
    {
        if (index == 0)
        {
            return false;
        }

        var previous = value[index - 1];
        if (!char.IsLetterOrDigit(previous))
        {
            return false;
        }

        if (char.IsLower(previous) || char.IsDigit(previous))
        {
            return true;
        }

        return index + 1 < value.Length && char.IsLower(value[index + 1]);
    }

    private static void AppendSeparator(StringBuilder builder)
    {
        if (builder.Length > 0 && builder[^1] != '_')
        {
            builder.Append('_');
        }
    }
}
