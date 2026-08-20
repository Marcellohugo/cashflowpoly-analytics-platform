// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui RulesetConfig.
using System.Text.Json;
using Cashflowpoly.Api.Contracts;

namespace Cashflowpoly.Api.Domain;

/// <summary>
/// Model domain konfigurasi ruleset berisi parameter permainan: mode, aksi harian pemain, kas awal, constraint, dan scoring.
/// </summary>
public sealed record RulesetConfig(
    string Mode,
    int ActionsPerTurn,
    int StartingCash,
    PlayerOrdering PlayerOrdering,
    int CashMin,
    int MaxIngredientTotal,
    int MaxSameIngredient,
    int? PrimaryNeedMaxPerDay,
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
    RulesetScoringConfig? Scoring)
{
    public int FinishDay { get; init; } = 25;

    public IReadOnlyList<RulesetNeedSetBonusDto> NeedSetBonuses { get; init; } = [];

    public IReadOnlyList<RulesetGoldPriceDto> GoldPrices { get; init; } = [];

    public IReadOnlyList<RulesetShariaLoanDto> ShariaLoans { get; init; } = [];

    public IReadOnlyList<RulesetInsuranceProductDto> InsuranceProducts { get; init; } = [];

    public IReadOnlyList<RulesetLifeRiskDto> LifeRisks { get; init; } = [];

    public IReadOnlyList<RulesetOrderDto> Orders { get; init; } = [];

    public IReadOnlyList<RulesetIngredientDto> Ingredients { get; init; } = [];

    public IReadOnlyList<RulesetFinancialGoalDto> FinancialGoals { get; init; } = [];

    public IReadOnlyList<RulesetNeedDto> Needs { get; init; } = [];

    public IReadOnlyList<RulesetActionDto> Actions { get; init; } = [];
}

/// <summary>
/// Konfigurasi skoring ruleset berisi pemetaan poin berdasarkan peringkat donasi, jumlah emas, dan peringkat pensiun.
/// </summary>
public sealed record RulesetScoringConfig(
    IReadOnlyList<RankPoint> DonationRankPoints,
    IReadOnlyList<QtyPoint> GoldPointsByQty,
    IReadOnlyList<RankPoint> PensionRankPoints);

/// <summary>
/// Pemetaan peringkat ke poin (untuk donasi dan pensiun).
/// </summary>
public sealed record RankPoint(int Rank, int Points);

/// <summary>
/// Pemetaan jumlah (quantity) ke poin (untuk emas).
/// </summary>
public sealed record QtyPoint(int Qty, int Points);

/// <summary>
/// Enum urutan pemain dalam sesi: berdasarkan join order, instruktur, urutan event, player ID, atau username.
/// </summary>
public enum PlayerOrdering
{
    PlayerOrder,
    EventSequence,
    PlayerId,
    Username
}

internal static class RulesetRuntimeMapper
{
    internal static bool TryBuildConfig(
        RulesetDefinitionDto definition,
        out RulesetConfig? config,
        out List<ErrorDetail> errors)
    {
        config = null;
        errors = new List<ErrorDetail>();

        var mode = string.IsNullOrWhiteSpace(definition.Mode)
            ? "MAHIR"
            : definition.Mode.Trim().ToUpperInvariant();
        if (mode is not ("PEMULA" or "MAHIR"))
        {
            errors.Add(new ErrorDetail("definition.mode", "INVALID_ENUM"));
        }

        var settings = definition.Settings ?? new RulesetSettingsDto();
        if (settings.ActionsPerTurn < 1 || settings.ActionsPerTurn > 10)
        {
            errors.Add(new ErrorDetail("definition.settings.actions_per_turn", "OUT_OF_RANGE"));
        }

        if (settings.StartingCash < 0)
        {
            errors.Add(new ErrorDetail("definition.settings.starting_cash", "OUT_OF_RANGE"));
        }

        if (settings.CashMin < 0)
        {
            errors.Add(new ErrorDetail("definition.settings.cash_min", "OUT_OF_RANGE"));
        }

        if (settings.MaxIngredientTotal < 0)
        {
            errors.Add(new ErrorDetail("definition.settings.max_ingredient_total", "OUT_OF_RANGE"));
        }

        if (settings.MaxSameIngredient < 0 || settings.MaxSameIngredient > settings.MaxIngredientTotal)
        {
            errors.Add(new ErrorDetail("definition.settings.max_same_ingredient", "INVALID_RELATION"));
        }

        if (settings.PrimaryNeedMaxPerDay < 0)
        {
            errors.Add(new ErrorDetail("definition.settings.primary_need_max_per_day", "OUT_OF_RANGE"));
        }

        if (settings.MinPlayers < 2 || settings.MaxPlayers > 4 || settings.MinPlayers > settings.MaxPlayers)
        {
            errors.Add(new ErrorDetail("definition.settings.min_players", "INVALID_RANGE"));
        }

        if (settings.DonationMinAmount < 1 || settings.DonationMaxAmount < settings.DonationMinAmount)
        {
            errors.Add(new ErrorDetail("definition.settings.donation_min_amount", "INVALID_RANGE"));
        }

        if (settings.FreelanceIncome < 1)
        {
            errors.Add(new ErrorDetail("definition.settings.freelance_income", "OUT_OF_RANGE"));
        }

        if (mode == "PEMULA" && (settings.LoanEnabled || settings.InsuranceEnabled || settings.SavingGoalEnabled))
        {
            errors.Add(new ErrorDetail("definition.settings", "DISALLOWED_FOR_MODE"));
        }

        if (!RulesetConfigParser.TryParsePlayerOrdering(definition.PlayerOrdering?.OrderingCode ?? "PLAYER_ORDER", out var playerOrdering))
        {
            errors.Add(new ErrorDetail("definition.player_ordering.ordering_code", "INVALID_ENUM"));
        }

        ValidateRankPoints(definition.DonationRankPoints, "definition.donation_rank_points", errors);
        ValidateRankPoints(definition.PensionRankPoints, "definition.pension_rank_points", errors);
        ValidateQtyPoints(definition.GoldPointsByQty, "definition.gold_points_by_qty", errors);
        ValidateNeedSetBonuses(definition.NeedSetBonuses, errors);
        ValidateGoldPrices(definition.GoldPrices, errors);
        ValidateShariaLoans(definition.ShariaLoans, settings.LoanEnabled, errors);
        ValidateInsuranceProducts(definition.InsuranceProducts, settings.InsuranceEnabled, errors);
        ValidateLifeRisks(definition.LifeRisks, errors);

        if (errors.Count > 0)
        {
            return false;
        }

        config = new RulesetConfig(
            mode,
            settings.ActionsPerTurn,
            settings.StartingCash,
            playerOrdering,
            settings.CashMin,
            settings.MaxIngredientTotal,
            settings.MaxSameIngredient,
            settings.PrimaryNeedMaxPerDay,
            settings.RequirePrimaryBeforeOthers,
            definition.PlayerOrdering?.FridayEnabled ?? true,
            definition.PlayerOrdering?.SaturdayEnabled ?? true,
            definition.PlayerOrdering?.SundayEnabled ?? true,
            settings.DonationMinAmount,
            settings.DonationMaxAmount,
            settings.GoldTradeAllowBuy,
            settings.GoldTradeAllowSell,
            settings.LoanEnabled,
            settings.InsuranceEnabled,
            settings.SavingGoalEnabled,
            settings.FreelanceIncome,
            new RulesetScoringConfig(
                definition.DonationRankPoints.Select(item => new RankPoint(item.Rank, item.Points)).ToList(),
                definition.GoldPointsByQty.Select(item => new QtyPoint(item.Qty, item.Points)).ToList(),
                definition.PensionRankPoints.Select(item => new RankPoint(item.Rank, item.Points)).ToList()))
        {
            FinishDay = settings.FinishDay,
            NeedSetBonuses = definition.NeedSetBonuses.ToList(),
            GoldPrices = definition.GoldPrices.ToList(),
            ShariaLoans = definition.ShariaLoans.ToList(),
            InsuranceProducts = definition.InsuranceProducts.ToList(),
            LifeRisks = definition.LifeRisks.ToList(),
            Orders = definition.Orders.ToList(),
            Ingredients = definition.Ingredients.ToList(),
            FinancialGoals = definition.FinancialGoals.ToList(),
            Needs = definition.Needs.ToList(),
            Actions = definition.Actions.ToList()
        };
        return true;
    }

    private static void ValidateNeedSetBonuses(
        IReadOnlyCollection<RulesetNeedSetBonusDto> bonuses,
        List<ErrorDetail> errors)
    {
        var seenPatterns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var bonus in bonuses)
        {
            var patternCode = bonus.PatternCode?.Trim().ToUpperInvariant() ?? string.Empty;
            if (patternCode is not ("THREE_DIFFERENT" or "THREE_SAME") ||
                bonus.RequiredCount <= 0 ||
                bonus.Points < 0)
            {
                errors.Add(new ErrorDetail("definition.need_set_bonuses", "INVALID"));
            }

            if (!seenPatterns.Add(patternCode))
            {
                errors.Add(new ErrorDetail("definition.need_set_bonuses", "DUPLICATE"));
            }
        }
    }

    private static void ValidateGoldPrices(
        IReadOnlyCollection<RulesetGoldPriceDto> prices,
        List<ErrorDetail> errors)
    {
        var seenCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var price in prices)
        {
            if (string.IsNullOrWhiteSpace(price.PriceCode) || price.Qty <= 0 || price.UnitPrice <= 0)
            {
                errors.Add(new ErrorDetail("definition.gold_prices", "OUT_OF_RANGE"));
            }

            if (!seenCodes.Add(price.PriceCode))
            {
                errors.Add(new ErrorDetail("definition.gold_prices", "DUPLICATE"));
            }
        }
    }

    private static void ValidateShariaLoans(
        IReadOnlyCollection<RulesetShariaLoanDto> loans,
        bool enabled,
        List<ErrorDetail> errors)
    {
        if (enabled && loans.Count == 0)
        {
            errors.Add(new ErrorDetail("definition.sharia_loans", "REQUIRED"));
            return;
        }

        var seenCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var loan in loans)
        {
            if (string.IsNullOrWhiteSpace(loan.LoanCode) ||
                loan.Principal <= 0 ||
                loan.RepaymentAmount < 0 ||
                loan.DurationDays <= 0 ||
                loan.PenaltyPoints < 0)
            {
                errors.Add(new ErrorDetail("definition.sharia_loans", "OUT_OF_RANGE"));
            }

            if (!seenCodes.Add(loan.LoanCode))
            {
                errors.Add(new ErrorDetail("definition.sharia_loans", "DUPLICATE"));
            }
        }
    }

    private static void ValidateInsuranceProducts(
        IReadOnlyCollection<RulesetInsuranceProductDto> products,
        bool enabled,
        List<ErrorDetail> errors)
    {
        if (enabled && products.Count == 0)
        {
            errors.Add(new ErrorDetail("definition.insurance_products", "REQUIRED"));
            return;
        }

        var seenCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var product in products)
        {
            if (string.IsNullOrWhiteSpace(product.ProductCode) ||
                product.Premium <= 0 ||
                product.UsageLimit <= 0)
            {
                errors.Add(new ErrorDetail("definition.insurance_products", "OUT_OF_RANGE"));
            }

            if (!seenCodes.Add(product.ProductCode))
            {
                errors.Add(new ErrorDetail("definition.insurance_products", "DUPLICATE"));
            }
        }
    }

    private static void ValidateLifeRisks(
        IReadOnlyCollection<RulesetLifeRiskDto> risks,
        List<ErrorDetail> errors)
    {
        var seenCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var risk in risks)
        {
            if (string.IsNullOrWhiteSpace(risk.RiskCode) ||
                string.IsNullOrWhiteSpace(risk.EffectType) ||
                (!string.IsNullOrWhiteSpace(risk.Direction) && risk.Direction is not ("IN" or "OUT")) ||
                risk.Amount < 0)
            {
                errors.Add(new ErrorDetail("definition.life_risks", "INVALID"));
            }

            if (!seenCodes.Add(risk.RiskCode))
            {
                errors.Add(new ErrorDetail("definition.life_risks", "DUPLICATE"));
            }
        }
    }

    private static void ValidateRankPoints(IReadOnlyCollection<RulesetDonationRankPointDto> points, string field, List<ErrorDetail> errors)
    {
        var seen = new HashSet<int>();
        foreach (var point in points)
        {
            if (point.Rank <= 0 || point.Points < 0)
            {
                errors.Add(new ErrorDetail(field, "OUT_OF_RANGE"));
                continue;
            }

            if (!seen.Add(point.Rank))
            {
                errors.Add(new ErrorDetail(field, "DUPLICATE"));
            }
        }
    }

    private static void ValidateRankPoints(IReadOnlyCollection<RulesetPensionRankPointDto> points, string field, List<ErrorDetail> errors)
    {
        var seen = new HashSet<int>();
        foreach (var point in points)
        {
            if (point.Rank <= 0 || point.Points < 0)
            {
                errors.Add(new ErrorDetail(field, "OUT_OF_RANGE"));
                continue;
            }

            if (!seen.Add(point.Rank))
            {
                errors.Add(new ErrorDetail(field, "DUPLICATE"));
            }
        }
    }

    private static void ValidateQtyPoints(IReadOnlyCollection<RulesetGoldPointDto> points, string field, List<ErrorDetail> errors)
    {
        var seen = new HashSet<int>();
        foreach (var point in points)
        {
            if (point.Qty <= 0 || point.Points < 0)
            {
                errors.Add(new ErrorDetail(field, "OUT_OF_RANGE"));
                continue;
            }

            if (!seen.Add(point.Qty))
            {
                errors.Add(new ErrorDetail(field, "DUPLICATE"));
            }
        }
    }
}

/// <summary>
/// Parser statis yang mengubah JSON konfigurasi ruleset menjadi objek <see cref="RulesetConfig"/> dengan validasi struktur dan batasan domain.
/// </summary>
internal static class RulesetConfigParser
{
    /// <summary>
    /// Mem-parse string JSON menjadi <see cref="RulesetConfig"/>, mengembalikan daftar error validasi jika gagal.
    /// </summary>
    /// <param name="json">String JSON konfigurasi ruleset.</param>
    /// <param name="config">Hasil parse konfigurasi, null jika gagal.</param>
    /// <param name="errors">Daftar detail error validasi.</param>
    /// <returns>True jika parsing dan validasi berhasil.</returns>
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

    /// <summary>
    /// Mem-parse JsonElement root menjadi <see cref="RulesetConfig"/> dengan validasi lengkap setiap field dan constraint domain.
    /// </summary>
    /// <param name="root">Elemen JSON root dari konfigurasi ruleset.</param>
    /// <param name="config">Hasil parse konfigurasi, null jika gagal.</param>
    /// <param name="errors">Daftar detail error validasi.</param>
    /// <returns>True jika parsing dan validasi berhasil.</returns>
    internal static bool TryParse(JsonElement root, out RulesetConfig? config, out List<ErrorDetail> errors)
    {
        config = null;
        errors = new List<ErrorDetail>();

        if (!TryGetString(root, "mode", out var mode, errors)) return false;
        if (!TryGetInt(root, "actions_per_turn", out var actionsPerTurn, errors)) return false;
        if (!TryGetInt(root, "starting_cash", out var startingCash, errors)) return false;
        var playerOrdering = PlayerOrdering.PlayerOrder;
        if (root.TryGetProperty("player_ordering", out var playerOrderingElement))
        {
            if (playerOrderingElement.ValueKind != JsonValueKind.String)
            {
                errors.Add(new ErrorDetail("config.player_ordering", "INVALID_TYPE"));
                return false;
            }

            var playerOrderingRaw = playerOrderingElement.GetString() ?? string.Empty;
            if (!TryParsePlayerOrdering(playerOrderingRaw, out playerOrdering))
            {
                errors.Add(new ErrorDetail("config.player_ordering", "INVALID_ENUM"));
            }
        }

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
            playerOrdering,
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

    /// <summary>
    /// Mengonversi string urutan pemain (snake_case) ke enum <see cref="PlayerOrdering"/>.
    /// </summary>
    /// <param name="rawValue">Nilai string mentah dari JSON.</param>
    /// <param name="ordering">Hasil konversi enum.</param>
    /// <returns>True jika konversi berhasil.</returns>
    internal static bool TryParsePlayerOrdering(string rawValue, out PlayerOrdering ordering)
    {
        ordering = PlayerOrdering.PlayerOrder;
        if (string.IsNullOrWhiteSpace(rawValue))
        {
            return false;
        }

        switch (rawValue.Trim().ToUpperInvariant())
        {
            case "PLAYER_ORDER":
                ordering = PlayerOrdering.PlayerOrder;
                return true;
            case "EVENT_SEQUENCE":
                ordering = PlayerOrdering.EventSequence;
                return true;
            case "PLAYER_ID":
                ordering = PlayerOrdering.PlayerId;
                return true;
            case "USERNAME":
                ordering = PlayerOrdering.Username;
                return true;
            default:
                return false;
        }
    }

    /// <summary>
    /// Mengekstrak properti JSON bertipe object, menambah error jika tidak ditemukan atau bukan object.
    /// </summary>
    private static bool TryGetObject(JsonElement root, string name, out JsonElement element, List<ErrorDetail> errors)
    {
        if (!root.TryGetProperty(name, out element) || element.ValueKind != JsonValueKind.Object)
        {
            errors.Add(new ErrorDetail($"config.{name}", "REQUIRED"));
            return false;
        }

        return true;
    }

    /// <summary>
    /// Mengekstrak properti JSON bertipe string, menambah error jika tidak ditemukan atau bukan string.
    /// </summary>
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

    /// <summary>
    /// Mengekstrak properti JSON bertipe integer, menambah error jika tidak ditemukan atau bukan angka.
    /// </summary>
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

    /// <summary>
    /// Mengekstrak properti JSON bertipe boolean, menambah error jika tidak ditemukan atau bukan boolean.
    /// </summary>
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
