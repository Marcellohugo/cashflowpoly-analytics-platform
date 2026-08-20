// Fungsi file: Menyediakan transformasi, lokalisasi, atau koneksi UI melalui PlayerMetricLabelFormatter.
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Cashflowpoly.Ui.Infrastructure;

/// <summary>
/// Teks siap tampil untuk satu variabel pendukung pemain.
/// </summary>
public sealed record PlayerMetricPresentation(
    string Label,
    string DisplayValue,
    string Unit,
    string Explanation,
    string Guidance,
    string Recommendation,
    string Formula,
    bool IsAdvancedOnly,
    string State);

/// <summary>
/// Formatter label metrik pemain yang tidak bergantung pada Razor runtime.
/// </summary>
public static class PlayerMetricLabelFormatter
{
    private static readonly HashSet<string> PercentageMetricKeys = new(StringComparer.OrdinalIgnoreCase)
    {
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
        "income_share_i"
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
        "planning_horizon"
    };

    /// <summary>
    /// Mengubah key metrik menjadi label manusiawi dengan dukungan lokalisasi.
    /// </summary>
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
            "coins_net_end" => "players.raw.coins_net_end_game",
            "coins_net_end_game" => "players.raw.coins_net_end_game",
            "meal_orders_available_passed" => "players.raw.meal_orders_passed",
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
    public static PlayerMetricPresentation DescribeMetric(
        string rawPath,
        string rawValue,
        bool isDerived,
        bool isAdvancedMode,
        string nullText,
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
        var state = isAdvancedOnly && !isAdvancedMode
            ? "not_applicable"
            : isUnavailable
                ? "unavailable"
                : hasNumericValue && Math.Abs(numericValue) < 0.0000001
                    ? "zero"
                    : "recorded";

        var displayNumericValue = FractionPercentageMetricKeys.Contains(metricKey) ||
                                  FractionPercentageMetricKeys.Contains(leafMetricKey)
            ? numericValue * 100
            : numericValue;
        var displayValue = state is "not_applicable" or "unavailable"
            ? translate("players.support.value.unavailable")
            : hasNumericValue
                ? FormatDisplayNumber(displayNumericValue)
                : rawValue;
        if (state is "not_applicable" or "unavailable")
        {
            unit = string.Empty;
        }

        var explanationKey = ResolveExplanationKey(metricKey, isDerived, unitKey);
        var explanation = string.Format(
            CultureInfo.CurrentCulture,
            translate(explanationKey),
            label);
        var guidanceKey = state switch
        {
            "not_applicable" => "players.support.guide.not_applicable",
            "unavailable" => "players.support.guide.unavailable",
            _ => ResolveGuidanceKey(metricKey, numericValue, hasNumericValue, isDerived, unitKey)
        };
        var recommendationKey = state switch
        {
            "not_applicable" => "players.support.recommendation.not_applicable",
            "unavailable" => "players.support.recommendation.unavailable",
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
    public static string HumanizeTransactionCategory(string category, Func<string, string> translate)
    {
        if (string.IsNullOrWhiteSpace(category))
        {
            return translate("players.details.transaction_label");
        }

        var normalized = category.Trim().ToUpperInvariant();
        var mapped = normalized switch
        {
            "DONATION" => translate("players.details.transaction.category.donation"),
            "GOLD_TRADE" => translate("players.details.transaction.category.gold_trade"),
            "INGREDIENT" => translate("players.details.transaction.category.ingredient"),
            "ORDER" => translate("players.details.transaction.category.order"),
            "FREELANCE" => translate("players.details.transaction.category.freelance"),
            "NEED_PRIMARY" => translate("players.details.transaction.category.need_primary"),
            "NEED_SECONDARY" => translate("players.details.transaction.category.need_secondary"),
            "NEED_TERTIARY" => translate("players.details.transaction.category.need_tertiary"),
            "SAVING_DEPOSIT" => translate("players.details.transaction.category.saving_deposit"),
            "SAVING_WITHDRAW" => translate("players.details.transaction.category.saving_withdraw"),
            "RISK_LIFE" => translate("players.details.transaction.category.risk_life"),
            "LOAN_TAKEN" => translate("players.details.transaction.category.loan_taken"),
            "LOAN_REPAID" => translate("players.details.transaction.category.loan_repaid"),
            "INSURANCE_PREMIUM" => translate("players.details.transaction.category.insurance_premium"),
            "INSURANCE_OFFSET" => translate("players.details.transaction.category.insurance_claim"),
            "EMERGENCY_OPTION" => translate("players.details.transaction.category.emergency_option"),
            _ => string.Empty
        };

        if (!string.IsNullOrWhiteSpace(mapped))
        {
            return mapped;
        }

        return HumanizeMetricKey(category.Trim().ToLowerInvariant(), translate);
    }

    /// <summary>
    /// Melokalkan detail transaksi berformat "DIRECTION - CATEGORY (amount)".
    /// </summary>
    public static string LocalizeTransactionDetail(string rawDetail, Func<string, string> translate)
    {
        if (string.IsNullOrWhiteSpace(rawDetail))
        {
            return translate("players.details.transaction_label");
        }

        var text = rawDetail.Trim();
        var match = Regex.Match(text, @"^(?<dir>[A-Za-z_]+)\s*-\s*(?<cat>.+?)(?:\s*\((?<amt>[^)]+)\))?$");
        if (!match.Success)
        {
            return text;
        }

        var rawDirection = match.Groups["dir"].Value.Trim();
        var rawCategory = match.Groups["cat"].Value.Trim();
        var amountText = match.Groups["amt"].Success ? match.Groups["amt"].Value.Trim() : string.Empty;

        if (rawDirection.Equals("START", StringComparison.OrdinalIgnoreCase))
        {
            return string.IsNullOrWhiteSpace(amountText)
                ? translate("players.details.transaction.opening_cash")
                : string.Format(
                    CultureInfo.CurrentCulture,
                    translate("players.details.transaction.opening_cash_with_amount"),
                    amountText);
        }

        var directionText = rawDirection.Equals("IN", StringComparison.OrdinalIgnoreCase)
            ? translate("players.details.transaction.cash_in")
            : rawDirection.Equals("OUT", StringComparison.OrdinalIgnoreCase)
                ? translate("players.details.transaction.cash_out")
                : HumanizeMetricKey(rawDirection.ToLowerInvariant(), translate);
        var categoryText = HumanizeTransactionCategory(rawCategory, translate);

        return string.IsNullOrWhiteSpace(amountText)
            ? $"{directionText} - {categoryText}"
            : $"{directionText} - {categoryText} ({amountText})";
    }

    /// <summary>
    /// Mencoba membaca teks metrik menjadi angka untuk chart.
    /// </summary>
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
            if (leafMetricKey is "total_actions" or "distinct_actions" or "repeated_actions") return "players.support.unit.actions";
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

        if (metricKey is "actions_skipped")
        {
            return "players.support.unit.day";
        }

        if (metricKey is "action_slots_unused")
        {
            return "players.support.unit.action_slot";
        }

        if (metricKey is "n_active_income_sources")
        {
            return "players.support.unit.sources";
        }

        if (metricKey is "income_producing_actions" or "all_player_actions" or
                "savings_actions" or "financial_goal_actions" or "insurance_premium_actions" ||
            leafMetricKey is "income_producing_actions" or "all_player_actions" or
                "savings_actions" or "financial_goal_actions" or "insurance_premium_actions")
        {
            return "players.support.unit.actions";
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

    private static string ResolveExplanationKey(string metricKey, bool isDerived, string unitKey)
    {
        if (isDerived)
        {
            var derivedKey = metricKey switch
            {
                "net_worth_index" or "growth_pattern_ratio" => "players.support.meaning.growth",
                "income_diversification_index" or "income_diversification_ratio" or "income_share_i" or
                    "n_active_income_sources" => "players.support.meaning.income_mix",
                "expense_management_efficiency" => "players.support.meaning.expense_efficiency",
                "business_profit_margin" or "business_efficiency_ratio" => "players.support.meaning.business",
                "gold_roi_percentage" => "players.support.meaning.gold_return",
                "risk_exposure_percentage" or "risk_cost_intensity" => "players.support.meaning.risk_impact",
                "risk_mitigation_effectiveness" or "insurance_activation_rate" or "insurance_coverage_rate" => "players.support.meaning.risk_protection",
                "risk_appetite_score" or "risk_appetite_score_normalized" or "risk_acceptance_rate" => "players.support.meaning.risk_appetite",
                "debt_leverage_ratio" or "debt_ratio" or "loan_repayment_discipline" => "players.support.meaning.debt",
                "goal_ambition" or "goal_ambition_index" or "goal_setting_ambition" or "goal_attempt_rate" or "goal_investment_rate" => "players.support.meaning.goals",
                "action_efficiency" or "action_efficiency_percent" or "action_diversity_score_avg" => "players.support.meaning.actions",
                "meal_order_success_rate" => "players.support.meaning.orders",
                "planning_horizon" or "planning_horizon_percent" => "players.support.meaning.planning",
                "fulfillment_diversity" or "fulfillment_diversity_document_formula" or "p_primary" or "p_secondary" or "p_tertiary" => "players.support.meaning.need_balance",
                "mission_achievement" => "players.support.meaning.needs",
                "donation_aggressiveness_percent" or "donation_stability_std_deviation" or "donation_ratio" or
                    "friday_participation_rate" or "donation_commitment_score" or "donation_stability" or "donation_stability_index" => "players.support.meaning.donation",
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
            "players.support.unit.coins" => "players.support.meaning.raw_coins",
            "players.support.unit.points" => "players.support.meaning.raw_points",
            "players.support.unit.rank" => "players.support.meaning.raw_rank",
            "players.support.unit.day" or "players.support.unit.turn" or "players.support.unit.action_slot" => "players.support.meaning.raw_timeline",
            _ when unitKey.StartsWith("players.support.unit.", StringComparison.Ordinal) &&
                   unitKey != "players.support.unit.coins" &&
                   unitKey != "players.support.unit.points" &&
                   unitKey != "players.support.unit.percent" &&
                   unitKey != "players.support.unit.multiplier" => "players.support.meaning.raw_count",
            _ => "players.support.meaning.raw"
        };
    }

    private static string ResolveGuidanceKey(
        string metricKey,
        double numericValue,
        bool hasNumericValue,
        bool isDerived,
        string unitKey)
    {
        if (hasNumericValue)
        {
            var normalizedRatio = numericValue;

            return metricKey switch
            {
                "net_worth_index" when numericValue >= 300 => "players.support.guide.wealth_exceptional",
                "net_worth_index" when numericValue >= 200 => "players.support.guide.wealth_strong",
                "net_worth_index" when numericValue >= 100 => "players.support.guide.wealth_growing",
                "net_worth_index" => "players.support.guide.wealth_declining",
                "income_diversification_index" or "income_diversification_ratio" when numericValue >= 67 => "players.support.guide.income_diverse",
                "income_diversification_index" or "income_diversification_ratio" when numericValue < 34 => "players.support.guide.income_concentrated",
                "income_diversification_index" or "income_diversification_ratio" => "players.support.guide.income_mixed",
                "expense_management_efficiency" => "players.support.guide.expense_efficiency",
                "business_profit_margin" or "gold_roi_percentage" when numericValue > 0 => "players.support.guide.return_positive",
                "business_profit_margin" or "gold_roi_percentage" when numericValue < 0 => "players.support.guide.return_negative",
                "business_profit_margin" or "gold_roi_percentage" => "players.support.guide.return_even",
                "business_efficiency_ratio" when numericValue > 2 => "players.support.guide.business_efficient",
                "business_efficiency_ratio" when numericValue < 1.5 => "players.support.guide.business_inefficient",
                "business_efficiency_ratio" => "players.support.guide.business_moderate",
                "risk_exposure_percentage" when numericValue > 30 => "players.support.guide.risk_high",
                "risk_exposure_percentage" when numericValue < 10 => "players.support.guide.risk_low",
                "risk_exposure_percentage" => "players.support.guide.risk_moderate",
                "risk_appetite_score" or "risk_appetite_score_normalized" when numericValue < 25 => "players.support.guide.appetite_cautious",
                "risk_appetite_score" or "risk_appetite_score_normalized" when numericValue <= 75 => "players.support.guide.appetite_balanced",
                "risk_appetite_score" or "risk_appetite_score_normalized" => "players.support.guide.appetite_high",
                "debt_leverage_ratio" when numericValue > 75 => "players.support.guide.debt_high",
                "debt_leverage_ratio" when numericValue <= 25 => "players.support.guide.debt_low",
                "debt_leverage_ratio" => "players.support.guide.debt_moderate",
                "goal_ambition" or "goal_ambition_index" when numericValue >= 67 => "players.support.guide.goals_strong",
                "goal_ambition" or "goal_ambition_index" when numericValue < 34 => "players.support.guide.goals_limited",
                "goal_ambition" or "goal_ambition_index" => "players.support.guide.goals_moderate",
                "loan_repayment_discipline" when numericValue >= 99.5 => "players.support.guide.loan_repaid",
                "loan_repayment_discipline" => "players.support.guide.loan_remaining",
                "action_efficiency_percent" when numericValue > 60 => "players.support.guide.action_income",
                "action_efficiency_percent" when numericValue < 40 => "players.support.guide.action_exploration",
                "action_efficiency_percent" => "players.support.guide.action_balanced",
                "meal_order_success_rate" when numericValue >= 80 => "players.support.guide.orders_strong",
                "meal_order_success_rate" when numericValue < 60 => "players.support.guide.orders_review",
                "meal_order_success_rate" => "players.support.guide.orders_moderate",
                "planning_horizon_percent" when numericValue > 40 => "players.support.guide.planning_long",
                "planning_horizon_percent" when numericValue < 20 => "players.support.guide.planning_short",
                "planning_horizon_percent" => "players.support.guide.planning_balanced",
                "planning_horizon" when normalizedRatio > 0.4 => "players.support.guide.planning_long",
                "planning_horizon" when normalizedRatio < 0.2 => "players.support.guide.planning_short",
                "planning_horizon" => "players.support.guide.planning_balanced",
                "fulfillment_diversity" when numericValue >= 0.67 => "players.support.guide.need_balance_high",
                "fulfillment_diversity" when numericValue < 0.34 => "players.support.guide.need_balance_low",
                "fulfillment_diversity" => "players.support.guide.need_balance_moderate",
                "growth_pattern_ratio" when numericValue > 3 => "players.support.guide.growth_strong",
                "growth_pattern_ratio" when numericValue < 1 => "players.support.guide.growth_declining",
                "growth_pattern_ratio" => "players.support.guide.growth_steady",
                "donation_aggressiveness_percent" when numericValue > 30 => "players.support.guide.donation_aggressive",
                "donation_aggressiveness_percent" when numericValue < 10 => "players.support.guide.donation_conservative",
                "donation_aggressiveness_percent" => "players.support.guide.donation_moderate",
                "donation_stability_std_deviation" when numericValue <= 1 => "players.support.guide.donation_stable",
                "donation_stability_std_deviation" => "players.support.guide.donation_variable",
                "donation_commitment_score" when numericValue >= 67 => "players.support.guide.donation_commitment_strong",
                "donation_commitment_score" when numericValue < 34 => "players.support.guide.donation_commitment_weak",
                "donation_commitment_score" => "players.support.guide.donation_commitment_moderate",
                "mission_achievement" when numericValue >= 1 => "players.support.guide.mission_complete",
                "mission_achievement" => "players.support.guide.mission_incomplete",
                "friday_participation_rate" when normalizedRatio >= 0.8 => "players.support.guide.participation_high",
                "friday_participation_rate" => "players.support.guide.participation_partial",
                _ => string.Empty
            } is { Length: > 0 } specificKey
                ? specificKey
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
                "players.support.unit.percent" => "players.support.guide.percent",
                "players.support.unit.multiplier" => "players.support.guide.ratio",
                _ => "players.support.guide.derived"
            };
        }

        return unitKey switch
        {
            "players.support.unit.coins" => "players.support.guide.coins",
            "players.support.unit.points" => "players.support.guide.points",
            "players.support.unit.rank" => "players.support.guide.rank",
            "players.support.unit.day" or "players.support.unit.turn" or "players.support.unit.action_slot" => "players.support.guide.turn",
            _ when unitKey.StartsWith("players.support.unit.", StringComparison.Ordinal) => "players.support.guide.count",
            _ => "players.support.guide.raw"
        };
    }

    private static string ResolveFunctionCategory(string metricKey)
    {
        return metricKey switch
        {
            "starting_coins" => "baseline",
            "cash_in_total" or "coins_earned_total" or "freelance_income" or "meal_income" or
                "gold_income" or "donations_received" or "other_income" or "total_income" => "income",
            "cash_out_total" or "coins_spent_total" or "essential_expenses" or "total_expenses" => "spending",
            "coins_held_current" or "coins_net_end" or "coins_net_end_game" or "cash_net_total" => "cash_position",
            "coins_saved" or "coins_in_savings_goal" => "savings",
            "mission_bonus_pts" or "total_happiness_points" or "final_rank" or "winner_flag" or
                "finish_line_reached" or "dnf_flag" or "value" => "outcome",
            "game_id" or "session_id" or "user_id" or "player_alias" or "game_mode" or "seed_source" => "session_context",
            _ when metricKey.Contains("ingredient", StringComparison.OrdinalIgnoreCase) => "inventory",
            _ when metricKey.Contains("meal_order", StringComparison.OrdinalIgnoreCase) ||
                   metricKey.Contains("business", StringComparison.OrdinalIgnoreCase) => "business_activity",
            _ when metricKey.Contains("need", StringComparison.OrdinalIgnoreCase) ||
                   metricKey.Contains("fulfillment", StringComparison.OrdinalIgnoreCase) ||
                   metricKey is "collection_mission_complete" or "specific_tertiary_need" or "mission_achievement" or
                       "p_primary" or "p_secondary" or "p_tertiary" => "needs",
            _ when metricKey.Contains("donat", StringComparison.OrdinalIgnoreCase) ||
                   metricKey.Contains("friday", StringComparison.OrdinalIgnoreCase) => "donation",
            _ when metricKey.Contains("per_turn", StringComparison.OrdinalIgnoreCase) ||
                   metricKey.Contains("action", StringComparison.OrdinalIgnoreCase) ||
                   metricKey.Contains("event", StringComparison.OrdinalIgnoreCase) ||
                   metricKey.Contains("transaction", StringComparison.OrdinalIgnoreCase) ||
                   metricKey.Contains("sequence", StringComparison.OrdinalIgnoreCase) ||
                   metricKey.StartsWith("day_", StringComparison.OrdinalIgnoreCase) ||
                   metricKey.Contains("_day_", StringComparison.OrdinalIgnoreCase) ||
                   metricKey.Contains("timestamp", StringComparison.OrdinalIgnoreCase) => "timeline",
            _ when metricKey.Contains("gold", StringComparison.OrdinalIgnoreCase) => "gold_investment",
            _ when metricKey.Contains("pension", StringComparison.OrdinalIgnoreCase) ||
                   metricKey == "leftover_coins_end_game" => "pension",
            _ when metricKey.Contains("risk", StringComparison.OrdinalIgnoreCase) ||
                   metricKey.Contains("insurance", StringComparison.OrdinalIgnoreCase) ||
                   metricKey.Contains("emergency", StringComparison.OrdinalIgnoreCase) => "risk",
            _ when metricKey.Contains("financial_goal", StringComparison.OrdinalIgnoreCase) ||
                   metricKey.StartsWith("goal_", StringComparison.OrdinalIgnoreCase) => "goals",
            _ when metricKey.Contains("loan", StringComparison.OrdinalIgnoreCase) ||
                   metricKey.Contains("debt", StringComparison.OrdinalIgnoreCase) => "debt",
            _ when metricKey.Contains("point", StringComparison.OrdinalIgnoreCase) ||
                   metricKey.EndsWith("_pts", StringComparison.OrdinalIgnoreCase) ||
                   metricKey.Contains("rank", StringComparison.OrdinalIgnoreCase) => "outcome",
            _ => string.Empty
        };
    }

    private static string ResolveRecommendationKey(string metricKey, double numericValue, bool hasNumericValue)
    {
        if (hasNumericValue)
        {
            var resultSpecificKey = metricKey switch
            {
                "net_worth_index" when numericValue < 100 => "players.support.recommendation.cash_recover",
                "income_diversification_index" or "income_diversification_ratio" when numericValue < 34 => "players.support.recommendation.income_mix_recover",
                "business_profit_margin" when numericValue <= 0 => "players.support.recommendation.business_recover",
                "risk_appetite_score" or "risk_appetite_score_normalized" when numericValue > 75 => "players.support.recommendation.risk_reduce",
                "debt_leverage_ratio" when numericValue > 75 => "players.support.recommendation.debt_reduce",
                "goal_ambition" or "goal_ambition_index" when numericValue < 50 => "players.support.recommendation.goals_focus",
                "action_efficiency_percent" when numericValue < 40 => "players.support.recommendation.action_income",
                "meal_order_success_rate" when numericValue < 60 => "players.support.recommendation.orders_improve",
                "planning_horizon_percent" when numericValue < 20 => "players.support.recommendation.planning_build",
                "planning_horizon" when numericValue < 0.2 => "players.support.recommendation.planning_build",
                "fulfillment_diversity" when numericValue < 0.67 => "players.support.recommendation.needs_balance",
                "donation_commitment_score" when numericValue < 34 => "players.support.recommendation.donation_stabilize",
                "total_happiness_pts" when numericValue < 0 => "players.support.recommendation.outcome_recover",
                _ => string.Empty
            };

            if (!string.IsNullOrWhiteSpace(resultSpecificKey))
            {
                return resultSpecificKey;
            }
        }

        var recommendationCategory = metricKey switch
        {
            "net_worth_index" or "growth_pattern_ratio" => "wealth",
            "income_diversification_index" or "income_diversification_ratio" or "income_share_i" or
                "n_active_income_sources" => "income_mix",
            "expense_management_efficiency" => "productive_spending",
            "risk_mitigation_effectiveness" or "insurance_activation_rate" or "insurance_coverage_rate" => "protection",
            "action_efficiency" or "action_efficiency_percent" => "action_balance",
            "planning_horizon" or "planning_horizon_percent" => "planning",
            _ => ResolveFunctionCategory(metricKey)
        };

        return string.IsNullOrWhiteSpace(recommendationCategory)
            ? "players.support.recommendation.balance"
            : $"players.support.recommendation.{recommendationCategory}";
    }

    private static string ResolveFormulaKey(string metricKey)
    {
        return metricKey switch
        {
            "net_worth_index" => "players.support.formula.net_worth",
            "income_diversification_index" => "players.support.formula.income_diversification_index",
            "income_diversification_ratio" => "players.support.formula.income_diversification",
            "expense_management_efficiency" => "players.support.formula.expense_efficiency",
            "business_profit_margin" => "players.support.formula.business_margin",
            "business_efficiency_ratio" => "players.support.formula.business_efficiency",
            "gold_roi_percentage" => "players.support.formula.gold_roi",
            "risk_exposure_percentage" => "players.support.formula.risk_exposure",
            "risk_mitigation_effectiveness" => "players.support.formula.risk_mitigation",
            "risk_appetite_score" or "risk_appetite_score_normalized" => "players.support.formula.risk_appetite",
            "debt_leverage_ratio" => "players.support.formula.debt_leverage",
            "loan_repayment_discipline" => "players.support.formula.loan_discipline",
            "debt_ratio" => "players.support.formula.debt_ratio",
            "goal_ambition" or "goal_ambition_index" => "players.support.formula.goal_ambition_index",
            "goal_setting_ambition" => "players.support.formula.goal_ambition",
            "action_efficiency" or "action_efficiency_percent" => "players.support.formula.action_efficiency",
            "meal_order_success_rate" => "players.support.formula.order_success",
            "planning_horizon" or "planning_horizon_percent" => "players.support.formula.planning",
            "fulfillment_diversity" => "players.support.formula.fulfillment",
            "fulfillment_diversity_document_formula" => "players.support.formula.fulfillment_document",
            "growth_pattern_ratio" => "players.support.formula.growth",
            "donation_aggressiveness_percent" => "players.support.formula.donation_aggressiveness",
            "donation_stability" or "donation_stability_std_deviation" => "players.support.formula.donation_stability",
            "donation_stability_index" => "players.support.formula.donation_stability_index",
            "donation_commitment_score" => "players.support.formula.donation_commitment",
            _ => string.Empty
        };
    }

    private static string FormatDisplayNumber(double value)
    {
        return Math.Abs(value - Math.Round(value)) < 0.0000001
            ? value.ToString("N0", CultureInfo.CurrentCulture)
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
