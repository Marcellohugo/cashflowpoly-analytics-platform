// Fungsi file: Menyediakan transformasi, lokalisasi, atau koneksi UI melalui PlayerMetricLabelFormatter.
using System.Globalization;
using System.Text;

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
        "cash_growth_percent",
        "business_expense_share_percent",
        "meal_order_profit_margin_percent",
        "risk_readiness_percent",
        "loan_burden_percent",
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
        var noLoanRecorded = metricKey == "day_when_debt_introduced" &&
                             isUnavailable &&
                             isAdvancedMode;
        var isGameDayMetric = IsGameDayMetric(metricKey);
        var state = isAdvancedOnly && !isAdvancedMode
            ? "not_applicable"
            : noLoanRecorded
                ? "recorded"
            : isUnavailable
                ? "unavailable"
                : hasNumericValue && isGameDayMetric
                    ? "recorded"
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
        if (noLoanRecorded)
        {
            displayValue = translate("players.support.value.no_loan_recorded");
            unit = string.Empty;
        }
        else if (hasNumericValue && isGameDayMetric)
        {
            displayValue = numericValue <= 0
                ? translate(metricKey == "day_when_debt_introduced"
                    ? "players.support.value.preparation_loan"
                    : "players.support.value.preparation")
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

        if (metricKey is "n_active_income_sources" or "active_income_source_count")
        {
            return "players.support.unit.sources";
        }

        if (metricKey is "income_producing_actions" or "income_main_actions" or
                "all_player_actions" or "total_main_actions" or
                "savings_actions" or "saving_actions" or "financial_goal_actions" or
                "insurance_premium_actions" or "insurance_actions" or "loan_repayment_actions" ||
            leafMetricKey is "income_producing_actions" or "income_main_actions" or
                "all_player_actions" or "total_main_actions" or
                "savings_actions" or "saving_actions" or "financial_goal_actions" or
                "insurance_premium_actions" or "insurance_actions" or "loan_repayment_actions")
        {
            return "players.support.unit.actions";
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
            "day_when_debt_introduced" => "players.support.meaning.first_loan_day",
            "day_when_first_risk_hit" => "players.support.meaning.first_risk_day",
            "day_game_completion" or "latest_day_index" => "players.support.meaning.last_activity_day",
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
            "ingredients_held_current" => "players.support.meaning.ingredients_remaining",
            "ingredients_used_per_meal" => "players.support.meaning.ingredients_per_order",
            "meal_orders_per_turn_average" => "players.support.meaning.orders_per_active_day",
            "meal_order_income_per_order" => "players.support.meaning.income_per_order",
            "ingredient_types_held" => "players.support.meaning.ingredient_types_remaining",
            "ingredients_wasted" => "players.support.meaning.ingredients_discarded",
            "gold_cards_purchased" => "players.support.meaning.gold_purchased",
            "gold_cards_held_end" => "players.support.meaning.gold_remaining",
            "gold_investment_net" => "players.support.meaning.gold_cashflow",
            "ingredient_cards_value_end" => "players.support.meaning.pension_ingredient_value",
            "life_risk_costs_per_card" or "life_risk_costs_total" => "players.support.meaning.risk_nominal_cost",
            "donation_rank_per_friday" => "players.support.meaning.donation_rank",
            "donation_history" => "players.support.meaning.donation_history",
            "donation_happiness_points" or "donations_pts" => "players.support.meaning.donation_happiness",
            "financial_goals_completed" => "players.support.meaning.completed_goals",
            "sharia_loans_outstanding_coins" => "players.support.meaning.outstanding_loan",
            "need_cards_purchased" => "players.support.meaning.need_cards_purchased",
            "need_cards_owned_current" => "players.support.meaning.need_cards_owned",
            "primary_needs_owned" or "secondary_needs_owned" or "tertiary_needs_owned" => "players.support.meaning.need_level_owned",
            "specific_tertiary_need" => "players.support.meaning.mission_need_owned",
            "collection_mission_complete" => "players.support.meaning.collection_mission",
            "need_cards_coins_spent" => "players.support.meaning.need_purchase_cost",
            "ingredients_collected" => "players.support.meaning.ingredients_collected",
            "ingredients_used_total" => "players.support.meaning.ingredients_used_total",
            "ingredients_used_per_meal_average" => "players.support.meaning.ingredients_used_average",
            "ingredient_investment_coins_total" => "players.support.meaning.ingredient_purchase_cost",
            "meal_orders_claimed" => "players.support.meaning.orders_completed",
            "meal_order_income_total" => "players.support.meaning.order_income_total",
            "gold_cards_initial" => "players.support.meaning.gold_initial",
            "gold_cards_sold" => "players.support.meaning.gold_sold",
            "gold_prices_per_purchase" => "players.support.meaning.gold_purchase_prices",
            "gold_price_per_sale" => "players.support.meaning.gold_sale_prices",
            "gold_investment_coins_spent" => "players.support.meaning.gold_purchase_cost",
            "gold_investment_coins_earned" => "players.support.meaning.gold_sale_income",
            "pension_fund_total" => "players.support.meaning.pension_total",
            "pension_fund_rank_per_game" => "players.support.meaning.pension_rank",
            "pension_fund_happiness_points" => "players.support.meaning.pension_happiness",
            "life_risk_cards_drawn" => "players.support.meaning.risk_cards_drawn",
            "life_risk_mitigated_with_insurance" => "players.support.meaning.risk_insured",
            "insurance_payments_made" => "players.support.meaning.insurance_premium",
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
                "cash_growth_percent" or "net_worth_index" or "growth_pattern_ratio" => "players.support.meaning.growth",
                "income_diversification_index" or "income_diversification_ratio" or "income_share_i" or
                    "n_active_income_sources" => "players.support.meaning.income_mix",
                "business_expense_share_percent" or "expense_management_efficiency" => "players.support.meaning.expense_efficiency",
                "meal_order_profit_margin_percent" or "business_profit_margin" or "business_efficiency_ratio" => "players.support.meaning.business",
                "gold_roi_percentage" => "players.support.meaning.gold_return",
                "risk_exposure_percentage" or "risk_cost_intensity" => "players.support.meaning.risk_impact",
                "risk_readiness_percent" => "players.support.meaning.risk_readiness",
                "risk_mitigation_effectiveness" or "insurance_activation_rate" or "insurance_coverage_rate" => "players.support.meaning.risk_protection",
                "risk_appetite_score" or "risk_appetite_score_normalized" or "risk_acceptance_rate" => "players.support.meaning.risk_appetite",
                "loan_burden_percent" => "players.support.meaning.loan_burden",
                "debt_leverage_ratio" or "debt_ratio" or "loan_repayment_discipline" => "players.support.meaning.debt",
                "financial_goal_progress_percent" or "goal_ambition" or "goal_ambition_index" or "goal_setting_ambition" or "goal_attempt_rate" or "goal_investment_rate" => "players.support.meaning.goals",
                "income_action_focus_percent" or "action_efficiency" or "action_efficiency_percent" or "action_diversity_score_avg" => "players.support.meaning.actions",
                "ingredient_utilization_percent" => "players.support.meaning.ingredient_utilization",
                "meal_order_success_rate" => "players.support.meaning.orders",
                "long_term_action_share_percent" or "planning_horizon" or "planning_horizon_percent" => "players.support.meaning.planning",
                "need_fulfillment_diversity_percent" or "fulfillment_diversity" or "fulfillment_diversity_document_formula" or "p_primary" or "p_secondary" or "p_tertiary" => "players.support.meaning.need_balance",
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
                "cash_growth_percent" or "net_worth_index" when numericValue >= 300 => "players.support.guide.wealth_exceptional",
                "cash_growth_percent" or "net_worth_index" when numericValue >= 200 => "players.support.guide.wealth_strong",
                "cash_growth_percent" or "net_worth_index" when numericValue >= 100 => "players.support.guide.wealth_growing",
                "cash_growth_percent" or "net_worth_index" => "players.support.guide.wealth_declining",
                "income_diversification_index" or "income_diversification_ratio" when numericValue >= 67 => "players.support.guide.income_diverse",
                "income_diversification_index" or "income_diversification_ratio" when numericValue < 34 => "players.support.guide.income_concentrated",
                "income_diversification_index" or "income_diversification_ratio" => "players.support.guide.income_mixed",
                "business_expense_share_percent" or "expense_management_efficiency" => "players.support.guide.expense_efficiency",
                "meal_order_profit_margin_percent" or "business_profit_margin" or "gold_roi_percentage" when numericValue > 0 => "players.support.guide.return_positive",
                "meal_order_profit_margin_percent" or "business_profit_margin" or "gold_roi_percentage" when numericValue < 0 => "players.support.guide.return_negative",
                "meal_order_profit_margin_percent" or "business_profit_margin" or "gold_roi_percentage" => "players.support.guide.return_even",
                "business_efficiency_ratio" when numericValue > 2 => "players.support.guide.business_efficient",
                "business_efficiency_ratio" when numericValue < 1.5 => "players.support.guide.business_inefficient",
                "business_efficiency_ratio" => "players.support.guide.business_moderate",
                "risk_exposure_percentage" when numericValue > 30 => "players.support.guide.risk_high",
                "risk_exposure_percentage" when numericValue < 10 => "players.support.guide.risk_low",
                "risk_exposure_percentage" => "players.support.guide.risk_moderate",
                "risk_appetite_score" or "risk_appetite_score_normalized" when numericValue < 25 => "players.support.guide.appetite_cautious",
                "risk_appetite_score" or "risk_appetite_score_normalized" when numericValue <= 75 => "players.support.guide.appetite_balanced",
                "risk_appetite_score" or "risk_appetite_score_normalized" => "players.support.guide.appetite_high",
                "risk_readiness_percent" when numericValue >= 99.5 => "players.support.guide.risk_readiness_complete",
                "risk_readiness_percent" when numericValue >= 50 => "players.support.guide.risk_readiness_most",
                "risk_readiness_percent" => "players.support.guide.risk_readiness_limited",
                "loan_burden_percent" or "debt_leverage_ratio" when numericValue > 75 => "players.support.guide.debt_high",
                "loan_burden_percent" or "debt_leverage_ratio" when numericValue <= 25 => "players.support.guide.debt_low",
                "loan_burden_percent" or "debt_leverage_ratio" => "players.support.guide.debt_moderate",
                "financial_goal_progress_percent" or "goal_ambition" or "goal_ambition_index" when numericValue >= 99.5 => "players.support.guide.goals_complete",
                "financial_goal_progress_percent" or "goal_ambition" or "goal_ambition_index" when numericValue >= 67 => "players.support.guide.goals_strong",
                "financial_goal_progress_percent" or "goal_ambition" or "goal_ambition_index" when numericValue < 34 => "players.support.guide.goals_limited",
                "financial_goal_progress_percent" or "goal_ambition" or "goal_ambition_index" => "players.support.guide.goals_moderate",
                "loan_repayment_discipline" when numericValue >= 99.5 => "players.support.guide.loan_repaid",
                "loan_repayment_discipline" => "players.support.guide.loan_remaining",
                "income_action_focus_percent" or "action_efficiency_percent" when numericValue > 60 => "players.support.guide.action_income",
                "income_action_focus_percent" or "action_efficiency_percent" when numericValue < 40 => "players.support.guide.action_exploration",
                "income_action_focus_percent" or "action_efficiency_percent" => "players.support.guide.action_balanced",
                "ingredient_utilization_percent" when numericValue >= 80 => "players.support.guide.ingredient_use_high",
                "ingredient_utilization_percent" when numericValue < 60 => "players.support.guide.ingredient_use_low",
                "ingredient_utilization_percent" => "players.support.guide.ingredient_use_moderate",
                "meal_order_success_rate" when numericValue >= 80 => "players.support.guide.orders_strong",
                "meal_order_success_rate" when numericValue < 60 => "players.support.guide.orders_review",
                "meal_order_success_rate" => "players.support.guide.orders_moderate",
                "long_term_action_share_percent" or "planning_horizon_percent" when numericValue > 40 => "players.support.guide.planning_long",
                "long_term_action_share_percent" or "planning_horizon_percent" when numericValue < 20 => "players.support.guide.planning_short",
                "long_term_action_share_percent" or "planning_horizon_percent" => "players.support.guide.planning_balanced",
                "planning_horizon" when normalizedRatio > 0.4 => "players.support.guide.planning_long",
                "planning_horizon" when normalizedRatio < 0.2 => "players.support.guide.planning_short",
                "planning_horizon" => "players.support.guide.planning_balanced",
                "need_fulfillment_diversity_percent" when numericValue >= 67 => "players.support.guide.need_balance_high",
                "need_fulfillment_diversity_percent" when numericValue < 34 => "players.support.guide.need_balance_low",
                "need_fulfillment_diversity_percent" => "players.support.guide.need_balance_moderate",
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
                "cash_growth_percent" or "net_worth_index" when numericValue < 100 => "players.support.recommendation.cash_recover",
                "income_diversification_index" or "income_diversification_ratio" when numericValue < 34 => "players.support.recommendation.income_mix_recover",
                "meal_order_profit_margin_percent" or "business_profit_margin" when numericValue <= 0 => "players.support.recommendation.business_recover",
                "risk_appetite_score" or "risk_appetite_score_normalized" when numericValue > 75 => "players.support.recommendation.risk_reduce",
                "loan_burden_percent" or "debt_leverage_ratio" when numericValue > 75 => "players.support.recommendation.debt_reduce",
                "financial_goal_progress_percent" or "goal_ambition" or "goal_ambition_index" when numericValue < 50 => "players.support.recommendation.goals_focus",
                "income_action_focus_percent" or "action_efficiency_percent" when numericValue < 40 => "players.support.recommendation.action_income",
                "ingredient_utilization_percent" or "meal_order_success_rate" when numericValue < 60 => "players.support.recommendation.orders_improve",
                "long_term_action_share_percent" or "planning_horizon_percent" when numericValue < 20 => "players.support.recommendation.planning_build",
                "planning_horizon" when numericValue < 0.2 => "players.support.recommendation.planning_build",
                "need_fulfillment_diversity_percent" when numericValue < 67 => "players.support.recommendation.needs_balance",
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
            "cash_growth_percent" or "net_worth_index" or "growth_pattern_ratio" => "wealth",
            "income_diversification_index" or "income_diversification_ratio" or "income_share_i" or
                "n_active_income_sources" => "income_mix",
            "business_expense_share_percent" or "expense_management_efficiency" => "productive_spending",
            "risk_readiness_percent" or "risk_mitigation_effectiveness" or "insurance_activation_rate" or "insurance_coverage_rate" => "protection",
            "income_action_focus_percent" or "action_efficiency" or "action_efficiency_percent" => "action_balance",
            "long_term_action_share_percent" or "planning_horizon" or "planning_horizon_percent" => "planning",
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
            "cash_growth_percent" or "net_worth_index" => "players.support.formula.net_worth",
            "income_diversification_index" => "players.support.formula.income_diversification_index",
            "income_diversification_ratio" => "players.support.formula.income_diversification",
            "business_expense_share_percent" or "expense_management_efficiency" => "players.support.formula.expense_efficiency",
            "meal_order_profit_margin_percent" or "business_profit_margin" => "players.support.formula.business_margin",
            "business_efficiency_ratio" => "players.support.formula.business_efficiency",
            "gold_roi_percentage" => "players.support.formula.gold_roi",
            "risk_exposure_percentage" => "players.support.formula.risk_exposure",
            "risk_mitigation_effectiveness" => "players.support.formula.risk_mitigation",
            "risk_readiness_percent" or "risk_appetite_score" or "risk_appetite_score_normalized" => "players.support.formula.risk_appetite",
            "loan_burden_percent" or "debt_leverage_ratio" => "players.support.formula.debt_leverage",
            "loan_repayment_discipline" => "players.support.formula.loan_discipline",
            "debt_ratio" => "players.support.formula.debt_ratio",
            "financial_goal_progress_percent" or "goal_ambition" or "goal_ambition_index" => "players.support.formula.goal_ambition_index",
            "goal_setting_ambition" => "players.support.formula.goal_ambition",
            "income_action_focus_percent" or "action_efficiency" or "action_efficiency_percent" => "players.support.formula.action_efficiency",
            "ingredient_utilization_percent" or "meal_order_success_rate" => "players.support.formula.order_success",
            "long_term_action_share_percent" or "planning_horizon" or "planning_horizon_percent" => "players.support.formula.planning",
            "need_fulfillment_diversity_percent" or "fulfillment_diversity" => "players.support.formula.fulfillment",
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
