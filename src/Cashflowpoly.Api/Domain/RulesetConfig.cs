using System.Text.Json;
using Cashflowpoly.Api.Models;

namespace Cashflowpoly.Api.Domain;

internal sealed record RulesetConfig(
    string Mode,
    int ActionsPerTurn,
    int StartingCash,
    int CashMin,
    int MaxIngredientTotal,
    int MaxSameIngredient,
    int PrimaryNeedMaxPerDay,
    bool RequirePrimaryBeforeOthers,
    bool FridayEnabled,
    bool SaturdayEnabled,
    bool SundayEnabled,
    int DonationMin,
    int DonationMax,
    bool GoldAllowBuy,
    bool GoldAllowSell,
    bool LoanEnabled,
    bool InsuranceEnabled,
    bool SavingGoalEnabled,
    int FreelanceIncome,
    RulesetScoringConfig? Scoring);

internal sealed record RulesetScoringConfig(
    IReadOnlyList<RankPoint> DonationRankPoints,
    IReadOnlyList<QtyPoint> GoldPointsByQty,
    IReadOnlyList<RankPoint> PensionRankPoints);

internal sealed record RankPoint(int Rank, int Points);

internal sealed record QtyPoint(int Qty, int Points);

internal static class RulesetConfigParser
{
    internal static bool TryParse(string json, out RulesetConfig? config, out List<ErrorDetail> errors)
    {
        config = null;
        errors = new List<ErrorDetail>();

        try
        {
            using var doc = JsonDocument.Parse(json);
            return TryParse(doc.RootElement, out config, out errors);
        }
        catch (JsonException)
        {
            errors.Add(new ErrorDetail("config", "INVALID_JSON"));
            return false;
        }
    }

    internal static bool TryParse(JsonElement root, out RulesetConfig? config, out List<ErrorDetail> errors)
    {
        config = null;
        errors = new List<ErrorDetail>();

        if (!TryGetString(root, "mode", out var mode, errors)) return false;
        if (!TryGetInt(root, "actions_per_turn", out var actionsPerTurn, errors)) return false;
        if (!TryGetInt(root, "starting_cash", out var startingCash, errors)) return false;

        if (!TryGetObject(root, "weekday_rules", out var weekdayRules, errors)) return false;
        if (!TryGetObject(weekdayRules, "friday", out var fridayRules, errors)) return false;
        if (!TryGetObject(weekdayRules, "saturday", out var saturdayRules, errors)) return false;
        if (!TryGetObject(weekdayRules, "sunday", out var sundayRules, errors)) return false;

        if (!TryGetBool(fridayRules, "enabled", out var fridayEnabled, errors)) return false;
        if (!TryGetBool(saturdayRules, "enabled", out var saturdayEnabled, errors)) return false;
        if (!TryGetBool(sundayRules, "enabled", out var sundayEnabled, errors)) return false;

        if (!TryGetObject(root, "constraints", out var constraints, errors)) return false;
        if (!TryGetInt(constraints, "cash_min", out var cashMin, errors)) return false;
        if (!TryGetInt(constraints, "max_ingredient_total", out var maxIngredientTotal, errors)) return false;
        if (!TryGetInt(constraints, "max_same_ingredient", out var maxSameIngredient, errors)) return false;
        if (!TryGetInt(constraints, "primary_need_max_per_day", out var primaryNeedMaxPerDay, errors)) return false;
        if (!TryGetBool(constraints, "require_primary_before_others", out var requirePrimaryBeforeOthers, errors)) return false;

        if (!TryGetObject(root, "donation", out var donation, errors)) return false;
        if (!TryGetInt(donation, "min_amount", out var donationMin, errors)) return false;
        if (!TryGetInt(donation, "max_amount", out var donationMax, errors)) return false;

        if (!TryGetObject(root, "gold_trade", out var goldTrade, errors)) return false;
        if (!TryGetBool(goldTrade, "allow_buy", out var allowBuy, errors)) return false;
        if (!TryGetBool(goldTrade, "allow_sell", out var allowSell, errors)) return false;

        if (!TryGetObject(root, "advanced", out var advanced, errors)) return false;
        if (!TryGetObject(advanced, "loan", out var loan, errors)) return false;
        if (!TryGetObject(advanced, "insurance", out var insurance, errors)) return false;
        if (!TryGetObject(advanced, "saving_goal", out var saving, errors)) return false;
        if (!TryGetBool(loan, "enabled", out var loanEnabled, errors)) return false;
        if (!TryGetBool(insurance, "enabled", out var insuranceEnabled, errors)) return false;
        if (!TryGetBool(saving, "enabled", out var savingGoalEnabled, errors)) return false;

        var freelanceIncome = 1;
        if (root.TryGetProperty("freelance", out var freelanceElement))
        {
            if (freelanceElement.ValueKind != JsonValueKind.Object)
            {
                errors.Add(new ErrorDetail("config.freelance", "INVALID_TYPE"));
                return false;
            }

            if (!freelanceElement.TryGetProperty("income", out var incomeProp) ||
                incomeProp.ValueKind != JsonValueKind.Number ||
                !incomeProp.TryGetInt32(out freelanceIncome))
            {
                errors.Add(new ErrorDetail("config.freelance.income", "REQUIRED"));
                return false;
            }
        }

        RulesetScoringConfig? scoring = null;
        if (root.TryGetProperty("scoring", out var scoringElement))
        {
            if (scoringElement.ValueKind != JsonValueKind.Object)
            {
                errors.Add(new ErrorDetail("config.scoring", "INVALID_TYPE"));
                return false;
            }

            var donationRankPoints = new List<RankPoint>();
            var goldPointsByQty = new List<QtyPoint>();
            var pensionRankPoints = new List<RankPoint>();

            if (scoringElement.TryGetProperty("donation_rank_points", out var donationElement))
            {
                if (donationElement.ValueKind != JsonValueKind.Array)
                {
                    errors.Add(new ErrorDetail("config.scoring.donation_rank_points", "INVALID_TYPE"));
                    return false;
                }

                var seenRanks = new HashSet<int>();
                foreach (var item in donationElement.EnumerateArray())
                {
                    if (!item.TryGetProperty("rank", out var rankProp) ||
                        !item.TryGetProperty("points", out var pointsProp) ||
                        !rankProp.TryGetInt32(out var rankValue) ||
                        !pointsProp.TryGetInt32(out var pointsValue))
                    {
                        errors.Add(new ErrorDetail("config.scoring.donation_rank_points", "INVALID_ITEM"));
                        return false;
                    }

                    if (rankValue <= 0 || pointsValue < 0)
                    {
                        errors.Add(new ErrorDetail("config.scoring.donation_rank_points", "OUT_OF_RANGE"));
                        return false;
                    }

                    if (!seenRanks.Add(rankValue))
                    {
                        errors.Add(new ErrorDetail("config.scoring.donation_rank_points", "DUPLICATE"));
                        return false;
                    }

                    donationRankPoints.Add(new RankPoint(rankValue, pointsValue));
                }
            }

            if (scoringElement.TryGetProperty("gold_points_by_qty", out var goldElement))
            {
                if (goldElement.ValueKind != JsonValueKind.Array)
                {
                    errors.Add(new ErrorDetail("config.scoring.gold_points_by_qty", "INVALID_TYPE"));
                    return false;
                }

                var seenQty = new HashSet<int>();
                foreach (var item in goldElement.EnumerateArray())
                {
                    if (!item.TryGetProperty("qty", out var qtyProp) ||
                        !item.TryGetProperty("points", out var pointsProp) ||
                        !qtyProp.TryGetInt32(out var qtyValue) ||
                        !pointsProp.TryGetInt32(out var pointsValue))
                    {
                        errors.Add(new ErrorDetail("config.scoring.gold_points_by_qty", "INVALID_ITEM"));
                        return false;
                    }

                    if (qtyValue <= 0 || pointsValue < 0)
                    {
                        errors.Add(new ErrorDetail("config.scoring.gold_points_by_qty", "OUT_OF_RANGE"));
                        return false;
                    }

                    if (!seenQty.Add(qtyValue))
                    {
                        errors.Add(new ErrorDetail("config.scoring.gold_points_by_qty", "DUPLICATE"));
                        return false;
                    }

                    goldPointsByQty.Add(new QtyPoint(qtyValue, pointsValue));
                }
            }

            if (scoringElement.TryGetProperty("pension_rank_points", out var pensionElement))
            {
                if (pensionElement.ValueKind != JsonValueKind.Array)
                {
                    errors.Add(new ErrorDetail("config.scoring.pension_rank_points", "INVALID_TYPE"));
                    return false;
                }

                var seenRanks = new HashSet<int>();
                foreach (var item in pensionElement.EnumerateArray())
                {
                    if (!item.TryGetProperty("rank", out var rankProp) ||
                        !item.TryGetProperty("points", out var pointsProp) ||
                        !rankProp.TryGetInt32(out var rankValue) ||
                        !pointsProp.TryGetInt32(out var pointsValue))
                    {
                        errors.Add(new ErrorDetail("config.scoring.pension_rank_points", "INVALID_ITEM"));
                        return false;
                    }

                    if (rankValue <= 0 || pointsValue < 0)
                    {
                        errors.Add(new ErrorDetail("config.scoring.pension_rank_points", "OUT_OF_RANGE"));
                        return false;
                    }

                    if (!seenRanks.Add(rankValue))
                    {
                        errors.Add(new ErrorDetail("config.scoring.pension_rank_points", "DUPLICATE"));
                        return false;
                    }

                    pensionRankPoints.Add(new RankPoint(rankValue, pointsValue));
                }
            }

            scoring = new RulesetScoringConfig(donationRankPoints, goldPointsByQty, pensionRankPoints);
        }

        var upperMode = mode.ToUpperInvariant();
        if (upperMode is not ("PEMULA" or "MAHIR"))
        {
            errors.Add(new ErrorDetail("config.mode", "INVALID_ENUM"));
        }

        if (actionsPerTurn < 1 || actionsPerTurn > 10)
        {
            errors.Add(new ErrorDetail("config.actions_per_turn", "OUT_OF_RANGE"));
        }

        if (startingCash < 0)
        {
            errors.Add(new ErrorDetail("config.starting_cash", "OUT_OF_RANGE"));
        }

        if (cashMin < 0)
        {
            errors.Add(new ErrorDetail("config.constraints.cash_min", "OUT_OF_RANGE"));
        }

        if (maxIngredientTotal < 0 || maxIngredientTotal > 50)
        {
            errors.Add(new ErrorDetail("config.constraints.max_ingredient_total", "OUT_OF_RANGE"));
        }

        if (maxSameIngredient < 0 || maxSameIngredient > 50)
        {
            errors.Add(new ErrorDetail("config.constraints.max_same_ingredient", "OUT_OF_RANGE"));
        }

        if (maxSameIngredient > maxIngredientTotal)
        {
            errors.Add(new ErrorDetail("config.constraints.max_same_ingredient", "INVALID_RELATION"));
        }

        if (primaryNeedMaxPerDay < 0 || primaryNeedMaxPerDay > 10)
        {
            errors.Add(new ErrorDetail("config.constraints.primary_need_max_per_day", "OUT_OF_RANGE"));
        }

        if (donationMin < 1 || donationMax < 1 || donationMin > donationMax)
        {
            errors.Add(new ErrorDetail("config.donation.min_amount", "INVALID_RANGE"));
        }

        if (freelanceIncome <= 0)
        {
            errors.Add(new ErrorDetail("config.freelance.income", "OUT_OF_RANGE"));
        }

        if (upperMode == "PEMULA" && (loanEnabled || insuranceEnabled || savingGoalEnabled))
        {
            errors.Add(new ErrorDetail("config.advanced", "DISALLOWED_FOR_MODE"));
        }

        if (errors.Count > 0)
        {
            return false;
        }

        config = new RulesetConfig(
            upperMode,
            actionsPerTurn,
            startingCash,
            cashMin,
            maxIngredientTotal,
            maxSameIngredient,
            primaryNeedMaxPerDay,
            requirePrimaryBeforeOthers,
            fridayEnabled,
            saturdayEnabled,
            sundayEnabled,
            donationMin,
            donationMax,
            allowBuy,
            allowSell,
            loanEnabled,
            insuranceEnabled,
            savingGoalEnabled,
            freelanceIncome,
            scoring);

        return true;
    }

    private static bool TryGetObject(JsonElement root, string name, out JsonElement element, List<ErrorDetail> errors)
    {
        if (!root.TryGetProperty(name, out element) || element.ValueKind != JsonValueKind.Object)
        {
            errors.Add(new ErrorDetail($"config.{name}", "REQUIRED"));
            return false;
        }

        return true;
    }

    private static bool TryGetString(JsonElement root, string name, out string value, List<ErrorDetail> errors)
    {
        value = string.Empty;
        if (!root.TryGetProperty(name, out var element) || element.ValueKind != JsonValueKind.String)
        {
            errors.Add(new ErrorDetail($"config.{name}", "REQUIRED"));
            return false;
        }

        value = element.GetString() ?? string.Empty;
        return true;
    }

    private static bool TryGetInt(JsonElement root, string name, out int value, List<ErrorDetail> errors)
    {
        value = 0;
        if (!root.TryGetProperty(name, out var element) || element.ValueKind != JsonValueKind.Number || !element.TryGetInt32(out value))
        {
            errors.Add(new ErrorDetail($"config.{name}", "REQUIRED"));
            return false;
        }

        return true;
    }

    private static bool TryGetBool(JsonElement root, string name, out bool value, List<ErrorDetail> errors)
    {
        value = false;
        if (!root.TryGetProperty(name, out var element) || (element.ValueKind != JsonValueKind.True && element.ValueKind != JsonValueKind.False))
        {
            errors.Add(new ErrorDetail($"config.{name}", "REQUIRED"));
            return false;
        }

        value = element.GetBoolean();
        return true;
    }
}
