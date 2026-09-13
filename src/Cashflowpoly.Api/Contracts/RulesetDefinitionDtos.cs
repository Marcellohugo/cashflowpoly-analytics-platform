// Fungsi file: Mendefinisikan kontrak data RulesetDefinitionDtos untuk request dan response API.
// Mengimpor namespace `System.Text.Json.Serialization` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.Json.Serialization;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Contracts` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Contracts;

public sealed class RulesetDefinitionDto
{
    // memetakan nama properti JSON menjadi (”mode”).
    [JsonPropertyName("mode")]
    public string Mode { get; init; } = "MAHIR";

    // memetakan nama properti JSON menjadi (”settings”).
    [JsonPropertyName("settings")]
    public RulesetSettingsDto Settings { get; init; } = new();

    // memetakan nama properti JSON menjadi (”player_ordering”).
    [JsonPropertyName("player_ordering")]
    public RulesetPlayerOrderingDto PlayerOrdering { get; init; } = new();

    // memetakan nama properti JSON menjadi (”actions”).
    [JsonPropertyName("actions")]
    public List<RulesetActionDto> Actions { get; init; } = [];

    // memetakan nama properti JSON menjadi (”ingredients”).
    [JsonPropertyName("ingredients")]
    public List<RulesetIngredientDto> Ingredients { get; init; } = [];

    // memetakan nama properti JSON menjadi (”orders”).
    [JsonPropertyName("orders")]
    public List<RulesetOrderDto> Orders { get; init; } = [];

    // memetakan nama properti JSON menjadi (”needs”).
    [JsonPropertyName("needs")]
    public List<RulesetNeedDto> Needs { get; init; } = [];

    // memetakan nama properti JSON menjadi (”need_set_bonuses”).
    [JsonPropertyName("need_set_bonuses")]
    public List<RulesetNeedSetBonusDto> NeedSetBonuses { get; init; } = [];

    // memetakan nama properti JSON menjadi (”collection_missions”).
    [JsonPropertyName("collection_missions")]
    public List<RulesetCollectionMissionDto> CollectionMissions { get; init; } = [];

    // memetakan nama properti JSON menjadi (”financial_goals”).
    [JsonPropertyName("financial_goals")]
    public List<RulesetFinancialGoalDto> FinancialGoals { get; init; } = [];

    // memetakan nama properti JSON menjadi (”narratives”).
    [JsonPropertyName("narratives")]
    public List<RulesetNarrativeDto> Narratives { get; init; } = [];

    // memetakan nama properti JSON menjadi (”donation_rank_points”).
    [JsonPropertyName("donation_rank_points")]
    public List<RulesetDonationRankPointDto> DonationRankPoints { get; init; } = [];

    // memetakan nama properti JSON menjadi (”gold_points_by_qty”).
    [JsonPropertyName("gold_points_by_qty")]
    public List<RulesetGoldPointDto> GoldPointsByQty { get; init; } = [];

    // memetakan nama properti JSON menjadi (”gold_prices”).
    [JsonPropertyName("gold_prices")]
    public List<RulesetGoldPriceDto> GoldPrices { get; init; } = [];

    // memetakan nama properti JSON menjadi (”pension_rank_points”).
    [JsonPropertyName("pension_rank_points")]
    public List<RulesetPensionRankPointDto> PensionRankPoints { get; init; } = [];

    // memetakan nama properti JSON menjadi (”tie_breakers”).
    [JsonPropertyName("tie_breakers")]
    public List<RulesetTieBreakerDto> TieBreakers { get; init; } = [];

    // memetakan nama properti JSON menjadi (”sharia_loans”).
    [JsonPropertyName("sharia_loans")]
    public List<RulesetShariaLoanDto> ShariaLoans { get; init; } = [];

    // memetakan nama properti JSON menjadi (”insurance_products”).
    [JsonPropertyName("insurance_products")]
    public List<RulesetInsuranceProductDto> InsuranceProducts { get; init; } = [];

    // memetakan nama properti JSON menjadi (”life_risks”).
    [JsonPropertyName("life_risks")]
    public List<RulesetLifeRiskDto> LifeRisks { get; init; } = [];

}

public sealed class RulesetSettingsDto
{
    // memetakan nama properti JSON menjadi (”actions_per_turn”).
    [JsonPropertyName("actions_per_turn")]
    public int ActionsPerTurn { get; init; } = 2;

    // memetakan nama properti JSON menjadi (”starting_cash”).
    [JsonPropertyName("starting_cash")]
    public int StartingCash { get; init; }

    // memetakan nama properti JSON menjadi (”initial_coins”).
    [JsonPropertyName("initial_coins")]
    public int InitialCoins { get; init; }

    // memetakan nama properti JSON menjadi (”initial_happiness”).
    [JsonPropertyName("initial_happiness")]
    public int InitialHappiness { get; init; }

    // memetakan nama properti JSON menjadi (”initial_saving”).
    [JsonPropertyName("initial_saving")]
    public int InitialSaving { get; init; }

    // memetakan nama properti JSON menjadi (”finish_day”).
    [JsonPropertyName("finish_day")]
    public int FinishDay { get; init; } = 25;

    // memetakan nama properti JSON menjadi (”min_players”).
    [JsonPropertyName("min_players")]
    public int MinPlayers { get; init; } = 2;

    // memetakan nama properti JSON menjadi (”max_players”).
    [JsonPropertyName("max_players")]
    public int MaxPlayers { get; init; } = 4;

    // memetakan nama properti JSON menjadi (”cash_min”).
    [JsonPropertyName("cash_min")]
    public int CashMin { get; init; }

    // memetakan nama properti JSON menjadi (”max_ingredient_total”).
    [JsonPropertyName("max_ingredient_total")]
    public int MaxIngredientTotal { get; init; }

    // memetakan nama properti JSON menjadi (”max_same_ingredient”).
    [JsonPropertyName("max_same_ingredient")]
    public int MaxSameIngredient { get; init; }

    // memetakan nama properti JSON menjadi (”primary_need_max_per_day”).
    [JsonPropertyName("primary_need_max_per_day")]
    public int? PrimaryNeedMaxPerDay { get; init; }

    // memetakan nama properti JSON menjadi (”require_primary_before_others”).
    [JsonPropertyName("require_primary_before_others")]
    public bool RequirePrimaryBeforeOthers { get; init; } = true;

    // memetakan nama properti JSON menjadi (”donation_min_amount”).
    [JsonPropertyName("donation_min_amount")]
    public int DonationMinAmount { get; init; } = 1;

    // memetakan nama properti JSON menjadi (”donation_max_amount”).
    [JsonPropertyName("donation_max_amount")]
    public int DonationMaxAmount { get; init; } = 1;

    // memetakan nama properti JSON menjadi (”gold_trade_allow_buy”).
    [JsonPropertyName("gold_trade_allow_buy")]
    public bool GoldTradeAllowBuy { get; init; } = true;

    // memetakan nama properti JSON menjadi (”gold_trade_allow_sell”).
    [JsonPropertyName("gold_trade_allow_sell")]
    public bool GoldTradeAllowSell { get; init; } = true;

    // memetakan nama properti JSON menjadi (”loan_enabled”).
    [JsonPropertyName("loan_enabled")]
    public bool LoanEnabled { get; init; }

    // memetakan nama properti JSON menjadi (”insurance_enabled”).
    [JsonPropertyName("insurance_enabled")]
    public bool InsuranceEnabled { get; init; }

    // memetakan nama properti JSON menjadi (”saving_goal_enabled”).
    [JsonPropertyName("saving_goal_enabled")]
    public bool SavingGoalEnabled { get; init; }

    // memetakan nama properti JSON menjadi (”freelance_income”).
    [JsonPropertyName("freelance_income")]
    public int FreelanceIncome { get; init; } = 1;
}

public sealed class RulesetPlayerOrderingDto
{
    // memetakan nama properti JSON menjadi (”ordering_code”).
    [JsonPropertyName("ordering_code")]
    public string OrderingCode { get; init; } = "PLAYER_ORDER";

    // memetakan nama properti JSON menjadi (”friday_feature”).
    [JsonPropertyName("friday_feature")]
    public string FridayFeature { get; init; } = "DONATION";

    // memetakan nama properti JSON menjadi (”friday_enabled”).
    [JsonPropertyName("friday_enabled")]
    public bool FridayEnabled { get; init; } = true;

    // memetakan nama properti JSON menjadi (”saturday_feature”).
    [JsonPropertyName("saturday_feature")]
    public string SaturdayFeature { get; init; } = "GOLD_TRADE";

    // memetakan nama properti JSON menjadi (”saturday_enabled”).
    [JsonPropertyName("saturday_enabled")]
    public bool SaturdayEnabled { get; init; } = true;

    // memetakan nama properti JSON menjadi (”sunday_feature”).
    [JsonPropertyName("sunday_feature")]
    public string SundayFeature { get; init; } = "REST";

    // memetakan nama properti JSON menjadi (”sunday_enabled”).
    [JsonPropertyName("sunday_enabled")]
    public bool SundayEnabled { get; init; } = true;
}

public sealed class RulesetActionDto
{
    // memetakan nama properti JSON menjadi (”action_id”).
    [JsonPropertyName("action_id")]
    public string ActionId { get; init; } = string.Empty;
}

public sealed class RulesetIngredientDto
{
    // memetakan nama properti JSON menjadi (”id”).
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    // memetakan nama properti JSON menjadi (”nama”).
    [JsonPropertyName("nama")]
    public string Nama { get; init; } = string.Empty;

    // memetakan nama properti JSON menjadi (”hargaBeli”).
    [JsonPropertyName("hargaBeli")]
    public int HargaBeli { get; init; }

    // memetakan nama properti JSON menjadi (”cardQty”).
    [JsonPropertyName("cardQty")]
    public int? CardQty { get; init; }
}

public sealed class RulesetOrderDto
{
    // memetakan nama properti JSON menjadi (”id”).
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    // memetakan nama properti JSON menjadi (”nama”).
    [JsonPropertyName("nama")]
    public string Nama { get; init; } = string.Empty;

    // memetakan nama properti JSON menjadi (”hargaJual”).
    [JsonPropertyName("hargaJual")]
    public int HargaJual { get; init; }

    // memetakan nama properti JSON menjadi (”poinKebahagiaan”).
    [JsonPropertyName("poinKebahagiaan")]
    public int PoinKebahagiaan { get; init; }

    // memetakan nama properti JSON menjadi (”bahan”).
    [JsonPropertyName("bahan")]
    public List<string> Bahan { get; init; } = [];

    // memetakan nama properti JSON menjadi (”cardQty”).
    [JsonPropertyName("cardQty")]
    public int? CardQty { get; init; }
}

public sealed class RulesetNeedDto
{
    // memetakan nama properti JSON menjadi (”id”).
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    // memetakan nama properti JSON menjadi (”nama”).
    [JsonPropertyName("nama")]
    public string Nama { get; init; } = string.Empty;

    // memetakan nama properti JSON menjadi (”family”).
    [JsonPropertyName("family")]
    public string? Family { get; init; }

    // memetakan nama properti JSON menjadi (”tipe”).
    [JsonPropertyName("tipe")]
    public string Tipe { get; init; } = string.Empty;

    // memetakan nama properti JSON menjadi (”hargaBeli”).
    [JsonPropertyName("hargaBeli")]
    public int HargaBeli { get; init; }

    // memetakan nama properti JSON menjadi (”poinKebahagiaan”).
    [JsonPropertyName("poinKebahagiaan")]
    public int PoinKebahagiaan { get; init; }

    // memetakan nama properti JSON menjadi (”cardQty”).
    [JsonPropertyName("cardQty")]
    public int? CardQty { get; init; }
}

public sealed class RulesetNeedSetBonusDto
{
    // memetakan nama properti JSON menjadi (”pattern_code”).
    [JsonPropertyName("pattern_code")]
    public string PatternCode { get; init; } = string.Empty;

    // memetakan nama properti JSON menjadi (”required_count”).
    [JsonPropertyName("required_count")]
    public int RequiredCount { get; init; }

    // memetakan nama properti JSON menjadi (”points”).
    [JsonPropertyName("points")]
    public int Points { get; init; }
}

public sealed class RulesetCollectionMissionDto
{
    // memetakan nama properti JSON menjadi (”id”).
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    // memetakan nama properti JSON menjadi (”nama”).
    [JsonPropertyName("nama")]
    public string Nama { get; init; } = string.Empty;

    // memetakan nama properti JSON menjadi (”success_points”).
    [JsonPropertyName("success_points")]
    public int SuccessPoints { get; init; }

    // memetakan nama properti JSON menjadi (”failure_points”).
    [JsonPropertyName("failure_points")]
    public int FailurePoints { get; init; }

    // memetakan nama properti JSON menjadi (”penaltyPoints”).
    [JsonPropertyName("penaltyPoints")]
    public int PenaltyPoints { get; init; }

    // memetakan nama properti JSON menjadi (”kebutuhanTarget”).
    [JsonPropertyName("kebutuhanTarget")]
    public List<RulesetCollectionMissionRequirementDto> KebutuhanTarget { get; init; } = [];
}

public sealed class RulesetCollectionMissionRequirementDto
{
    // memetakan nama properti JSON menjadi (”order”).
    [JsonPropertyName("order")]
    public int Order { get; init; }

    // memetakan nama properti JSON menjadi (”type”).
    [JsonPropertyName("type")]
    public string Type { get; init; } = string.Empty;

    // memetakan nama properti JSON menjadi (”value”).
    [JsonPropertyName("value")]
    public string Value { get; init; } = string.Empty;
}

public sealed class RulesetFinancialGoalDto
{
    // memetakan nama properti JSON menjadi (”id”).
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    // memetakan nama properti JSON menjadi (”nama”).
    [JsonPropertyName("nama")]
    public string Nama { get; init; } = string.Empty;

    // memetakan nama properti JSON menjadi (”hargaBeli”).
    [JsonPropertyName("hargaBeli")]
    public int HargaBeli { get; init; }

    // memetakan nama properti JSON menjadi (”poinKebahagiaan”).
    [JsonPropertyName("poinKebahagiaan")]
    public int PoinKebahagiaan { get; init; }

    // memetakan nama properti JSON menjadi (”cardQty”).
    [JsonPropertyName("cardQty")]
    public int? CardQty { get; init; }
}

public sealed class RulesetNarrativeDto
{
    // memetakan nama properti JSON menjadi (”id”).
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    // memetakan nama properti JSON menjadi (”nama”).
    [JsonPropertyName("nama")]
    public string Nama { get; init; } = string.Empty;

    // memetakan nama properti JSON menjadi (”teks”).
    [JsonPropertyName("teks")]
    public List<string> Teks { get; init; } = [];

    // memetakan nama properti JSON menjadi (”prerequisiteAksi”).
    [JsonPropertyName("prerequisiteAksi")]
    public List<RulesetNarrativePrerequisiteDto> PrerequisiteAksi { get; init; } = [];

}

public sealed class RulesetNarrativePrerequisiteDto
{
    // memetakan nama properti JSON menjadi (”aksi”).
    [JsonPropertyName("aksi")]
    public string Aksi { get; init; } = string.Empty;

    // memetakan nama properti JSON menjadi (”value”).
    [JsonPropertyName("value")]
    public int Value { get; init; }
}

public sealed class RulesetDonationRankPointDto
{
    // memetakan nama properti JSON menjadi (”rank”).
    [JsonPropertyName("rank")]
    public int Rank { get; init; }

    // memetakan nama properti JSON menjadi (”points”).
    [JsonPropertyName("points")]
    public int Points { get; init; }
}

public sealed class RulesetGoldPointDto
{
    // memetakan nama properti JSON menjadi (”qty”).
    [JsonPropertyName("qty")]
    public int Qty { get; init; }

    // memetakan nama properti JSON menjadi (”points”).
    [JsonPropertyName("points")]
    public int Points { get; init; }
}

public sealed class RulesetGoldPriceDto
{
    // memetakan nama properti JSON menjadi (”price_code”).
    [JsonPropertyName("price_code")]
    public string PriceCode { get; init; } = string.Empty;

    // memetakan nama properti JSON menjadi (”qty”).
    [JsonPropertyName("qty")]
    public int Qty { get; init; }

    // memetakan nama properti JSON menjadi (”unit_price”).
    [JsonPropertyName("unit_price")]
    public int UnitPrice { get; init; }

    // memetakan nama properti JSON menjadi (”card_qty”).
    [JsonPropertyName("card_qty")]
    public int? CardQty { get; init; }
}

public sealed class RulesetPensionRankPointDto
{
    // memetakan nama properti JSON menjadi (”rank”).
    [JsonPropertyName("rank")]
    public int Rank { get; init; }

    // memetakan nama properti JSON menjadi (”points”).
    [JsonPropertyName("points")]
    public int Points { get; init; }
}

public sealed class RulesetTieBreakerDto
{
    // memetakan nama properti JSON menjadi (”tie_breaker_code”).
    [JsonPropertyName("tie_breaker_code")]
    public string TieBreakerCode { get; init; } = string.Empty;

    // memetakan nama properti JSON menjadi (”tie_number”).
    [JsonPropertyName("tie_number")]
    public int TieNumber { get; init; }

    // memetakan nama properti JSON menjadi (”card_qty”).
    [JsonPropertyName("card_qty")]
    public int? CardQty { get; init; }
}

public sealed class RulesetShariaLoanDto
{
    // memetakan nama properti JSON menjadi (”loan_code”).
    [JsonPropertyName("loan_code")]
    public string LoanCode { get; init; } = string.Empty;

    // memetakan nama properti JSON menjadi (”item_name”).
    [JsonPropertyName("item_name")]
    public string ItemName { get; init; } = string.Empty;

    // memetakan nama properti JSON menjadi (”principal”).
    [JsonPropertyName("principal")]
    public int Principal { get; init; }

    // memetakan nama properti JSON menjadi (”repayment_amount”).
    [JsonPropertyName("repayment_amount")]
    public int RepaymentAmount { get; init; }

    // memetakan nama properti JSON menjadi (”duration_days”).
    [JsonPropertyName("duration_days")]
    public int DurationDays { get; init; }

    // memetakan nama properti JSON menjadi (”penalty_points”).
    [JsonPropertyName("penalty_points")]
    public int PenaltyPoints { get; init; }

    // memetakan nama properti JSON menjadi (”card_qty”).
    [JsonPropertyName("card_qty")]
    public int? CardQty { get; init; }
}

public sealed class RulesetInsuranceProductDto
{
    // memetakan nama properti JSON menjadi (”product_code”).
    [JsonPropertyName("product_code")]
    public string ProductCode { get; init; } = string.Empty;

    // memetakan nama properti JSON menjadi (”item_name”).
    [JsonPropertyName("item_name")]
    public string ItemName { get; init; } = string.Empty;

    // memetakan nama properti JSON menjadi (”premium”).
    [JsonPropertyName("premium")]
    public int Premium { get; init; }

    // memetakan nama properti JSON menjadi (”usage_limit”).
    [JsonPropertyName("usage_limit")]
    public int UsageLimit { get; init; }

    // memetakan nama properti JSON menjadi (”card_qty”).
    [JsonPropertyName("card_qty")]
    public int? CardQty { get; init; }
}

public sealed class RulesetLifeRiskDto
{
    // memetakan nama properti JSON menjadi (”risk_code”).
    [JsonPropertyName("risk_code")]
    public string RiskCode { get; init; } = string.Empty;

    // memetakan nama properti JSON menjadi (”item_name”).
    [JsonPropertyName("item_name")]
    public string ItemName { get; init; } = string.Empty;

    // memetakan nama properti JSON menjadi (”effect_type”).
    [JsonPropertyName("effect_type")]
    public string EffectType { get; init; } = string.Empty;

    // memetakan nama properti JSON menjadi (”direction”).
    [JsonPropertyName("direction")]
    public string Direction { get; init; } = string.Empty;

    // memetakan nama properti JSON menjadi (”amount”).
    [JsonPropertyName("amount")]
    public int Amount { get; init; }

    // memetakan nama properti JSON menjadi (”target_scope”).
    [JsonPropertyName("target_scope")]
    public string TargetScope { get; init; } = "SELF";

    // memetakan nama properti JSON menjadi (”value_delta”).
    [JsonPropertyName("value_delta")]
    public int? ValueDelta { get; init; }

    // memetakan nama properti JSON menjadi (”duration_days”).
    [JsonPropertyName("duration_days")]
    public int? DurationDays { get; init; }

    // memetakan nama properti JSON menjadi (”card_qty”).
    [JsonPropertyName("card_qty")]
    public int? CardQty { get; init; }
}
