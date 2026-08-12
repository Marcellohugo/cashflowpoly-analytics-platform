// Fungsi file: Mendefinisikan kontrak data RulesetDefinitionDtos untuk request dan response API.
using System.Text.Json.Serialization;

namespace Cashflowpoly.Api.Contracts;

public sealed class RulesetDefinitionDto
{
    [JsonPropertyName("mode")]
    public string Mode { get; init; } = "MAHIR";

    [JsonPropertyName("settings")]
    public RulesetSettingsDto Settings { get; init; } = new();

    [JsonPropertyName("player_ordering")]
    public RulesetPlayerOrderingDto PlayerOrdering { get; init; } = new();

    [JsonPropertyName("actions")]
    public List<RulesetActionDto> Actions { get; init; } = [];

    [JsonPropertyName("ingredients")]
    public List<RulesetIngredientDto> Ingredients { get; init; } = [];

    [JsonPropertyName("orders")]
    public List<RulesetOrderDto> Orders { get; init; } = [];

    [JsonPropertyName("needs")]
    public List<RulesetNeedDto> Needs { get; init; } = [];

    [JsonPropertyName("need_set_bonuses")]
    public List<RulesetNeedSetBonusDto> NeedSetBonuses { get; init; } = [];

    [JsonPropertyName("collection_missions")]
    public List<RulesetCollectionMissionDto> CollectionMissions { get; init; } = [];

    [JsonPropertyName("financial_goals")]
    public List<RulesetFinancialGoalDto> FinancialGoals { get; init; } = [];

    [JsonPropertyName("narratives")]
    public List<RulesetNarrativeDto> Narratives { get; init; } = [];

    [JsonPropertyName("donation_rank_points")]
    public List<RulesetDonationRankPointDto> DonationRankPoints { get; init; } = [];

    [JsonPropertyName("gold_points_by_qty")]
    public List<RulesetGoldPointDto> GoldPointsByQty { get; init; } = [];

    [JsonPropertyName("gold_prices")]
    public List<RulesetGoldPriceDto> GoldPrices { get; init; } = [];

    [JsonPropertyName("pension_rank_points")]
    public List<RulesetPensionRankPointDto> PensionRankPoints { get; init; } = [];

    [JsonPropertyName("tie_breakers")]
    public List<RulesetTieBreakerDto> TieBreakers { get; init; } = [];

    [JsonPropertyName("sharia_loans")]
    public List<RulesetShariaLoanDto> ShariaLoans { get; init; } = [];

    [JsonPropertyName("insurance_products")]
    public List<RulesetInsuranceProductDto> InsuranceProducts { get; init; } = [];

    [JsonPropertyName("life_risks")]
    public List<RulesetLifeRiskDto> LifeRisks { get; init; } = [];

}

public sealed class RulesetSettingsDto
{
    [JsonPropertyName("actions_per_turn")]
    public int ActionsPerTurn { get; init; } = 2;

    [JsonPropertyName("starting_cash")]
    public int StartingCash { get; init; }

    [JsonPropertyName("initial_coins")]
    public int InitialCoins { get; init; }

    [JsonPropertyName("initial_happiness")]
    public int InitialHappiness { get; init; }

    [JsonPropertyName("initial_saving")]
    public int InitialSaving { get; init; }

    [JsonPropertyName("finish_day")]
    public int FinishDay { get; init; } = 25;

    [JsonPropertyName("min_players")]
    public int MinPlayers { get; init; } = 2;

    [JsonPropertyName("max_players")]
    public int MaxPlayers { get; init; } = 4;

    [JsonPropertyName("cash_min")]
    public int CashMin { get; init; }

    [JsonPropertyName("max_ingredient_total")]
    public int MaxIngredientTotal { get; init; }

    [JsonPropertyName("max_same_ingredient")]
    public int MaxSameIngredient { get; init; }

    [JsonPropertyName("primary_need_max_per_day")]
    public int? PrimaryNeedMaxPerDay { get; init; }

    [JsonPropertyName("require_primary_before_others")]
    public bool RequirePrimaryBeforeOthers { get; init; } = true;

    [JsonPropertyName("donation_min_amount")]
    public int DonationMinAmount { get; init; } = 1;

    [JsonPropertyName("donation_max_amount")]
    public int DonationMaxAmount { get; init; } = 1;

    [JsonPropertyName("gold_trade_allow_buy")]
    public bool GoldTradeAllowBuy { get; init; } = true;

    [JsonPropertyName("gold_trade_allow_sell")]
    public bool GoldTradeAllowSell { get; init; } = true;

    [JsonPropertyName("loan_enabled")]
    public bool LoanEnabled { get; init; }

    [JsonPropertyName("insurance_enabled")]
    public bool InsuranceEnabled { get; init; }

    [JsonPropertyName("saving_goal_enabled")]
    public bool SavingGoalEnabled { get; init; }

    [JsonPropertyName("freelance_income")]
    public int FreelanceIncome { get; init; } = 1;
}

public sealed class RulesetPlayerOrderingDto
{
    [JsonPropertyName("ordering_code")]
    public string OrderingCode { get; init; } = "PLAYER_ORDER";

    [JsonPropertyName("friday_feature")]
    public string FridayFeature { get; init; } = "DONATION";

    [JsonPropertyName("friday_enabled")]
    public bool FridayEnabled { get; init; } = true;

    [JsonPropertyName("saturday_feature")]
    public string SaturdayFeature { get; init; } = "GOLD_TRADE";

    [JsonPropertyName("saturday_enabled")]
    public bool SaturdayEnabled { get; init; } = true;

    [JsonPropertyName("sunday_feature")]
    public string SundayFeature { get; init; } = "REST";

    [JsonPropertyName("sunday_enabled")]
    public bool SundayEnabled { get; init; } = true;
}

public sealed class RulesetActionDto
{
    [JsonPropertyName("action_id")]
    public string ActionId { get; init; } = string.Empty;
}

public sealed class RulesetIngredientDto
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    [JsonPropertyName("nama")]
    public string Nama { get; init; } = string.Empty;

    [JsonPropertyName("hargaBeli")]
    public int HargaBeli { get; init; }
}

public sealed class RulesetOrderDto
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    [JsonPropertyName("nama")]
    public string Nama { get; init; } = string.Empty;

    [JsonPropertyName("hargaJual")]
    public int HargaJual { get; init; }

    [JsonPropertyName("poinKebahagiaan")]
    public int PoinKebahagiaan { get; init; }

    [JsonPropertyName("bahan")]
    public List<string> Bahan { get; init; } = [];

    [JsonPropertyName("cardQty")]
    public int? CardQty { get; init; }
}

public sealed class RulesetNeedDto
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    [JsonPropertyName("nama")]
    public string Nama { get; init; } = string.Empty;

    [JsonPropertyName("family")]
    public string? Family { get; init; }

    [JsonPropertyName("tipe")]
    public string Tipe { get; init; } = string.Empty;

    [JsonPropertyName("hargaBeli")]
    public int HargaBeli { get; init; }

    [JsonPropertyName("poinKebahagiaan")]
    public int PoinKebahagiaan { get; init; }
}

public sealed class RulesetNeedSetBonusDto
{
    [JsonPropertyName("pattern_code")]
    public string PatternCode { get; init; } = string.Empty;

    [JsonPropertyName("required_count")]
    public int RequiredCount { get; init; }

    [JsonPropertyName("points")]
    public int Points { get; init; }
}

public sealed class RulesetCollectionMissionDto
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    [JsonPropertyName("nama")]
    public string Nama { get; init; } = string.Empty;

    [JsonPropertyName("success_points")]
    public int SuccessPoints { get; init; }

    [JsonPropertyName("failure_points")]
    public int FailurePoints { get; init; }

    [JsonPropertyName("penaltyPoints")]
    public int PenaltyPoints { get; init; }

    [JsonPropertyName("kebutuhanTarget")]
    public List<RulesetCollectionMissionRequirementDto> KebutuhanTarget { get; init; } = [];
}

public sealed class RulesetCollectionMissionRequirementDto
{
    [JsonPropertyName("order")]
    public int Order { get; init; }

    [JsonPropertyName("type")]
    public string Type { get; init; } = string.Empty;

    [JsonPropertyName("value")]
    public string Value { get; init; } = string.Empty;
}

public sealed class RulesetFinancialGoalDto
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    [JsonPropertyName("nama")]
    public string Nama { get; init; } = string.Empty;

    [JsonPropertyName("hargaBeli")]
    public int HargaBeli { get; init; }

    [JsonPropertyName("poinKebahagiaan")]
    public int PoinKebahagiaan { get; init; }
}

public sealed class RulesetNarrativeDto
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    [JsonPropertyName("nama")]
    public string Nama { get; init; } = string.Empty;

    [JsonPropertyName("teks")]
    public List<string> Teks { get; init; } = [];

    [JsonPropertyName("prerequisiteAksi")]
    public List<RulesetNarrativePrerequisiteDto> PrerequisiteAksi { get; init; } = [];

}

public sealed class RulesetNarrativePrerequisiteDto
{
    [JsonPropertyName("aksi")]
    public string Aksi { get; init; } = string.Empty;

    [JsonPropertyName("value")]
    public int Value { get; init; }
}

public sealed class RulesetDonationRankPointDto
{
    [JsonPropertyName("rank")]
    public int Rank { get; init; }

    [JsonPropertyName("points")]
    public int Points { get; init; }
}

public sealed class RulesetGoldPointDto
{
    [JsonPropertyName("qty")]
    public int Qty { get; init; }

    [JsonPropertyName("points")]
    public int Points { get; init; }
}

public sealed class RulesetGoldPriceDto
{
    [JsonPropertyName("price_code")]
    public string PriceCode { get; init; } = string.Empty;

    [JsonPropertyName("qty")]
    public int Qty { get; init; }

    [JsonPropertyName("unit_price")]
    public int UnitPrice { get; init; }

    [JsonPropertyName("card_qty")]
    public int? CardQty { get; init; }
}

public sealed class RulesetPensionRankPointDto
{
    [JsonPropertyName("rank")]
    public int Rank { get; init; }

    [JsonPropertyName("points")]
    public int Points { get; init; }
}

public sealed class RulesetTieBreakerDto
{
    [JsonPropertyName("tie_breaker_code")]
    public string TieBreakerCode { get; init; } = string.Empty;

    [JsonPropertyName("tie_number")]
    public int TieNumber { get; init; }

    [JsonPropertyName("card_qty")]
    public int? CardQty { get; init; }
}

public sealed class RulesetShariaLoanDto
{
    [JsonPropertyName("loan_code")]
    public string LoanCode { get; init; } = string.Empty;

    [JsonPropertyName("item_name")]
    public string ItemName { get; init; } = string.Empty;

    [JsonPropertyName("principal")]
    public int Principal { get; init; }

    [JsonPropertyName("repayment_amount")]
    public int RepaymentAmount { get; init; }

    [JsonPropertyName("duration_days")]
    public int DurationDays { get; init; }

    [JsonPropertyName("penalty_points")]
    public int PenaltyPoints { get; init; }

    [JsonPropertyName("card_qty")]
    public int? CardQty { get; init; }
}

public sealed class RulesetInsuranceProductDto
{
    [JsonPropertyName("product_code")]
    public string ProductCode { get; init; } = string.Empty;

    [JsonPropertyName("item_name")]
    public string ItemName { get; init; } = string.Empty;

    [JsonPropertyName("premium")]
    public int Premium { get; init; }

    [JsonPropertyName("usage_limit")]
    public int UsageLimit { get; init; }

    [JsonPropertyName("card_qty")]
    public int? CardQty { get; init; }
}

public sealed class RulesetLifeRiskDto
{
    [JsonPropertyName("risk_code")]
    public string RiskCode { get; init; } = string.Empty;

    [JsonPropertyName("item_name")]
    public string ItemName { get; init; } = string.Empty;

    [JsonPropertyName("effect_type")]
    public string EffectType { get; init; } = string.Empty;

    [JsonPropertyName("direction")]
    public string Direction { get; init; } = string.Empty;

    [JsonPropertyName("amount")]
    public int Amount { get; init; }

    [JsonPropertyName("target_scope")]
    public string TargetScope { get; init; } = "SELF";

    [JsonPropertyName("value_delta")]
    public int? ValueDelta { get; init; }

    [JsonPropertyName("duration_days")]
    public int? DurationDays { get; init; }

    [JsonPropertyName("card_qty")]
    public int? CardQty { get; init; }
}
