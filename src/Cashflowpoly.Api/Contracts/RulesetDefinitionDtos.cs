// Fungsi file: Mendefinisikan kontrak data RulesetDefinitionDtos untuk request dan response API.
// Mengimpor namespace `System.Text.Json.Serialization` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.Json.Serialization;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Contracts` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Contracts;

// Mendefinisikan tipe class `RulesetDefinitionDto`; sealed mencegah tipe ini diturunkan lagi.
public sealed class RulesetDefinitionDto
// Membuka scope tipe RulesetDefinitionDto; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // memetakan nama properti JSON menjadi (”mode”).
    [JsonPropertyName("mode")]
    // Mendefinisikan properti `Mode` bertipe `string` untuk mode permainan yang menentukan kelompok aturan yang digunakan; get menyediakan pembacaan
    // nilai, init membatasi pengisian saat inisialisasi objek; nilai awalnya nilai literal `”MAHIR”`.
    public string Mode { get; init; } = "MAHIR";

    // memetakan nama properti JSON menjadi (”settings”).
    [JsonPropertyName("settings")]
    // Mendefinisikan properti `Settings` bertipe `RulesetSettingsDto` untuk nilai settings; get menyediakan pembacaan nilai, init membatasi pengisian
    // saat inisialisasi objek; nilai awalnya objek baru dengan tipe mengikuti konteks tujuan dan argumen ().
    public RulesetSettingsDto Settings { get; init; } = new();

    // memetakan nama properti JSON menjadi (”player_ordering”).
    [JsonPropertyName("player_ordering")]
    // Mendefinisikan properti `PlayerOrdering` bertipe `RulesetPlayerOrderingDto` untuk nilai pemain ordering; get menyediakan pembacaan nilai, init
    // membatasi pengisian saat inisialisasi objek; nilai awalnya objek baru dengan tipe mengikuti konteks tujuan dan argumen ().
    public RulesetPlayerOrderingDto PlayerOrdering { get; init; } = new();

    // memetakan nama properti JSON menjadi (”actions”).
    [JsonPropertyName("actions")]
    // Mendefinisikan properti `Actions` bertipe `List<RulesetActionDto>` untuk nilai aksi; get menyediakan pembacaan nilai, init membatasi pengisian
    // saat inisialisasi objek; nilai awalnya koleksi kosong dengan tipe mengikuti konteks tujuan.
    public List<RulesetActionDto> Actions { get; init; } = [];

    // memetakan nama properti JSON menjadi (”ingredients”).
    [JsonPropertyName("ingredients")]
    // Mendefinisikan properti `Ingredients` bertipe `List<RulesetIngredientDto>` untuk nilai bahan; get menyediakan pembacaan nilai, init membatasi
    // pengisian saat inisialisasi objek; nilai awalnya koleksi kosong dengan tipe mengikuti konteks tujuan.
    public List<RulesetIngredientDto> Ingredients { get; init; } = [];

    // memetakan nama properti JSON menjadi (”orders”).
    [JsonPropertyName("orders")]
    // Mendefinisikan properti `Orders` bertipe `List<RulesetOrderDto>` untuk nilai pesanan; get menyediakan pembacaan nilai, init membatasi pengisian
    // saat inisialisasi objek; nilai awalnya koleksi kosong dengan tipe mengikuti konteks tujuan.
    public List<RulesetOrderDto> Orders { get; init; } = [];

    // memetakan nama properti JSON menjadi (”needs”).
    [JsonPropertyName("needs")]
    // Mendefinisikan properti `Needs` bertipe `List<RulesetNeedDto>` untuk nilai kebutuhan; get menyediakan pembacaan nilai, init membatasi pengisian
    // saat inisialisasi objek; nilai awalnya koleksi kosong dengan tipe mengikuti konteks tujuan.
    public List<RulesetNeedDto> Needs { get; init; } = [];

    // memetakan nama properti JSON menjadi (”need_set_bonuses”).
    [JsonPropertyName("need_set_bonuses")]
    // Mendefinisikan properti `NeedSetBonuses` bertipe `List<RulesetNeedSetBonusDto>` untuk nilai kebutuhan set bonuses; get menyediakan pembacaan
    // nilai, init membatasi pengisian saat inisialisasi objek; nilai awalnya koleksi kosong dengan tipe mengikuti konteks tujuan.
    public List<RulesetNeedSetBonusDto> NeedSetBonuses { get; init; } = [];

    // memetakan nama properti JSON menjadi (”collection_missions”).
    [JsonPropertyName("collection_missions")]
    // Mendefinisikan properti `CollectionMissions` bertipe `List<RulesetCollectionMissionDto>` untuk nilai collection misi; get menyediakan pembacaan
    // nilai, init membatasi pengisian saat inisialisasi objek; nilai awalnya koleksi kosong dengan tipe mengikuti konteks tujuan.
    public List<RulesetCollectionMissionDto> CollectionMissions { get; init; } = [];

    // memetakan nama properti JSON menjadi (”financial_goals”).
    [JsonPropertyName("financial_goals")]
    // Mendefinisikan properti `FinancialGoals` bertipe `List<RulesetFinancialGoalDto>` untuk nilai keuangan target; get menyediakan pembacaan nilai,
    // init membatasi pengisian saat inisialisasi objek; nilai awalnya koleksi kosong dengan tipe mengikuti konteks tujuan.
    public List<RulesetFinancialGoalDto> FinancialGoals { get; init; } = [];

    // memetakan nama properti JSON menjadi (”narratives”).
    [JsonPropertyName("narratives")]
    // Mendefinisikan properti `Narratives` bertipe `List<RulesetNarrativeDto>` untuk nilai narratives; get menyediakan pembacaan nilai, init membatasi
    // pengisian saat inisialisasi objek; nilai awalnya koleksi kosong dengan tipe mengikuti konteks tujuan.
    public List<RulesetNarrativeDto> Narratives { get; init; } = [];

    // memetakan nama properti JSON menjadi (”donation_rank_points”).
    [JsonPropertyName("donation_rank_points")]
    // Mendefinisikan properti `DonationRankPoints` bertipe `List<RulesetDonationRankPointDto>` untuk nilai donasi rank poin; get menyediakan pembacaan
    // nilai, init membatasi pengisian saat inisialisasi objek; nilai awalnya koleksi kosong dengan tipe mengikuti konteks tujuan.
    public List<RulesetDonationRankPointDto> DonationRankPoints { get; init; } = [];

    // memetakan nama properti JSON menjadi (”gold_points_by_qty”).
    [JsonPropertyName("gold_points_by_qty")]
    // Mendefinisikan properti `GoldPointsByQty` bertipe `List<RulesetGoldPointDto>` untuk nilai emas poin berdasarkan qty; get menyediakan pembacaan
    // nilai, init membatasi pengisian saat inisialisasi objek; nilai awalnya koleksi kosong dengan tipe mengikuti konteks tujuan.
    public List<RulesetGoldPointDto> GoldPointsByQty { get; init; } = [];

    // memetakan nama properti JSON menjadi (”gold_prices”).
    [JsonPropertyName("gold_prices")]
    // Mendefinisikan properti `GoldPrices` bertipe `List<RulesetGoldPriceDto>` untuk nilai emas prices; get menyediakan pembacaan nilai, init membatasi
    // pengisian saat inisialisasi objek; nilai awalnya koleksi kosong dengan tipe mengikuti konteks tujuan.
    public List<RulesetGoldPriceDto> GoldPrices { get; init; } = [];

    // memetakan nama properti JSON menjadi (”pension_rank_points”).
    [JsonPropertyName("pension_rank_points")]
    // Mendefinisikan properti `PensionRankPoints` bertipe `List<RulesetPensionRankPointDto>` untuk nilai pension rank poin; get menyediakan pembacaan
    // nilai, init membatasi pengisian saat inisialisasi objek; nilai awalnya koleksi kosong dengan tipe mengikuti konteks tujuan.
    public List<RulesetPensionRankPointDto> PensionRankPoints { get; init; } = [];

    // memetakan nama properti JSON menjadi (”tie_breakers”).
    [JsonPropertyName("tie_breakers")]
    // Mendefinisikan properti `TieBreakers` bertipe `List<RulesetTieBreakerDto>` untuk nilai tie breakers; get menyediakan pembacaan nilai, init
    // membatasi pengisian saat inisialisasi objek; nilai awalnya koleksi kosong dengan tipe mengikuti konteks tujuan.
    public List<RulesetTieBreakerDto> TieBreakers { get; init; } = [];

    // memetakan nama properti JSON menjadi (”sharia_loans”).
    [JsonPropertyName("sharia_loans")]
    // Mendefinisikan properti `ShariaLoans` bertipe `List<RulesetShariaLoanDto>` untuk nilai sharia pinjaman; get menyediakan pembacaan nilai, init
    // membatasi pengisian saat inisialisasi objek; nilai awalnya koleksi kosong dengan tipe mengikuti konteks tujuan.
    public List<RulesetShariaLoanDto> ShariaLoans { get; init; } = [];

    // memetakan nama properti JSON menjadi (”insurance_products”).
    [JsonPropertyName("insurance_products")]
    // Mendefinisikan properti `InsuranceProducts` bertipe `List<RulesetInsuranceProductDto>` untuk nilai asuransi products; get menyediakan pembacaan
    // nilai, init membatasi pengisian saat inisialisasi objek; nilai awalnya koleksi kosong dengan tipe mengikuti konteks tujuan.
    public List<RulesetInsuranceProductDto> InsuranceProducts { get; init; } = [];

    // memetakan nama properti JSON menjadi (”life_risks”).
    [JsonPropertyName("life_risks")]
    // Mendefinisikan properti `LifeRisks` bertipe `List<RulesetLifeRiskDto>` untuk nilai life risks; get menyediakan pembacaan nilai, init membatasi
    // pengisian saat inisialisasi objek; nilai awalnya koleksi kosong dengan tipe mengikuti konteks tujuan.
    public List<RulesetLifeRiskDto> LifeRisks { get; init; } = [];

// Menutup scope tipe RulesetDefinitionDto; bagian berikut berada di luar batas blok tersebut.
}

// Mendefinisikan tipe class `RulesetSettingsDto`; sealed mencegah tipe ini diturunkan lagi.
public sealed class RulesetSettingsDto
// Membuka scope tipe RulesetSettingsDto; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // memetakan nama properti JSON menjadi (”actions_per_turn”).
    [JsonPropertyName("actions_per_turn")]
    // Mendefinisikan properti `ActionsPerTurn` bertipe `int` untuk nilai aksi per giliran; get menyediakan pembacaan nilai, init membatasi pengisian
    // saat inisialisasi objek; nilai awalnya nilai literal `2`.
    public int ActionsPerTurn { get; init; } = 2;

    // memetakan nama properti JSON menjadi (”starting_cash”).
    [JsonPropertyName("starting_cash")]
    // Mendefinisikan properti `StartingCash` bertipe `int` untuk nilai starting uang tunai; get menyediakan pembacaan nilai, init membatasi pengisian
    // saat inisialisasi objek.
    public int StartingCash { get; init; }

    // memetakan nama properti JSON menjadi (”initial_coins”).
    [JsonPropertyName("initial_coins")]
    // Mendefinisikan properti `InitialCoins` bertipe `int` untuk nilai awal coins; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek.
    public int InitialCoins { get; init; }

    // memetakan nama properti JSON menjadi (”initial_happiness”).
    [JsonPropertyName("initial_happiness")]
    // Mendefinisikan properti `InitialHappiness` bertipe `int` untuk nilai awal kebahagiaan; get menyediakan pembacaan nilai, init membatasi pengisian
    // saat inisialisasi objek.
    public int InitialHappiness { get; init; }

    // memetakan nama properti JSON menjadi (”initial_saving”).
    [JsonPropertyName("initial_saving")]
    // Mendefinisikan properti `InitialSaving` bertipe `int` untuk nilai awal tabungan; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek.
    public int InitialSaving { get; init; }

    // memetakan nama properti JSON menjadi (”finish_day”).
    [JsonPropertyName("finish_day")]
    // Mendefinisikan properti `FinishDay` bertipe `int` untuk nilai finish hari; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek; nilai awalnya nilai literal `25`.
    public int FinishDay { get; init; } = 25;

    // memetakan nama properti JSON menjadi (”min_players”).
    [JsonPropertyName("min_players")]
    // Mendefinisikan properti `MinPlayers` bertipe `int` untuk nilai minimum pemain; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek; nilai awalnya nilai literal `2`.
    public int MinPlayers { get; init; } = 2;

    // memetakan nama properti JSON menjadi (”max_players”).
    [JsonPropertyName("max_players")]
    // Mendefinisikan properti `MaxPlayers` bertipe `int` untuk nilai maksimum pemain; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek; nilai awalnya nilai literal `4`.
    public int MaxPlayers { get; init; } = 4;

    // memetakan nama properti JSON menjadi (”cash_min”).
    [JsonPropertyName("cash_min")]
    // Mendefinisikan properti `CashMin` bertipe `int` untuk nilai uang tunai minimum; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek.
    public int CashMin { get; init; }

    // memetakan nama properti JSON menjadi (”max_ingredient_total”).
    [JsonPropertyName("max_ingredient_total")]
    // Mendefinisikan properti `MaxIngredientTotal` bertipe `int` untuk nilai maksimum bahan total; get menyediakan pembacaan nilai, init membatasi
    // pengisian saat inisialisasi objek.
    public int MaxIngredientTotal { get; init; }

    // memetakan nama properti JSON menjadi (”max_same_ingredient”).
    [JsonPropertyName("max_same_ingredient")]
    // Mendefinisikan properti `MaxSameIngredient` bertipe `int` untuk nilai maksimum same bahan; get menyediakan pembacaan nilai, init membatasi
    // pengisian saat inisialisasi objek.
    public int MaxSameIngredient { get; init; }

    // memetakan nama properti JSON menjadi (”primary_need_max_per_day”).
    [JsonPropertyName("primary_need_max_per_day")]
    // Mendefinisikan properti `PrimaryNeedMaxPerDay` bertipe `int?` untuk nilai primary kebutuhan maksimum per hari; get menyediakan pembacaan nilai,
    // init membatasi pengisian saat inisialisasi objek; tanda ? mengizinkan nilai null.
    public int? PrimaryNeedMaxPerDay { get; init; }

    // memetakan nama properti JSON menjadi (”require_primary_before_others”).
    [JsonPropertyName("require_primary_before_others")]
    // Mendefinisikan properti `RequirePrimaryBeforeOthers` bertipe `bool` untuk nilai require primary before others; get menyediakan pembacaan nilai,
    // init membatasi pengisian saat inisialisasi objek; nilai awalnya true, yaitu kondisi aktif/terpenuhi.
    public bool RequirePrimaryBeforeOthers { get; init; } = true;

    // memetakan nama properti JSON menjadi (”donation_min_amount”).
    [JsonPropertyName("donation_min_amount")]
    // Mendefinisikan properti `DonationMinAmount` bertipe `int` untuk nilai donasi minimum nominal; get menyediakan pembacaan nilai, init membatasi
    // pengisian saat inisialisasi objek; nilai awalnya nilai literal `1`.
    public int DonationMinAmount { get; init; } = 1;

    // memetakan nama properti JSON menjadi (”donation_max_amount”).
    [JsonPropertyName("donation_max_amount")]
    // Mendefinisikan properti `DonationMaxAmount` bertipe `int` untuk nilai donasi maksimum nominal; get menyediakan pembacaan nilai, init membatasi
    // pengisian saat inisialisasi objek; nilai awalnya nilai literal `1`.
    public int DonationMaxAmount { get; init; } = 1;

    // memetakan nama properti JSON menjadi (”gold_trade_allow_buy”).
    [JsonPropertyName("gold_trade_allow_buy")]
    // Mendefinisikan properti `GoldTradeAllowBuy` bertipe `bool` untuk nilai emas trade allow buy; get menyediakan pembacaan nilai, init membatasi
    // pengisian saat inisialisasi objek; nilai awalnya true, yaitu kondisi aktif/terpenuhi.
    public bool GoldTradeAllowBuy { get; init; } = true;

    // memetakan nama properti JSON menjadi (”gold_trade_allow_sell”).
    [JsonPropertyName("gold_trade_allow_sell")]
    // Mendefinisikan properti `GoldTradeAllowSell` bertipe `bool` untuk nilai emas trade allow sell; get menyediakan pembacaan nilai, init membatasi
    // pengisian saat inisialisasi objek; nilai awalnya true, yaitu kondisi aktif/terpenuhi.
    public bool GoldTradeAllowSell { get; init; } = true;

    // memetakan nama properti JSON menjadi (”loan_enabled”).
    [JsonPropertyName("loan_enabled")]
    // Mendefinisikan properti `LoanEnabled` bertipe `bool` untuk nilai pinjaman enabled; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek.
    public bool LoanEnabled { get; init; }

    // memetakan nama properti JSON menjadi (”insurance_enabled”).
    [JsonPropertyName("insurance_enabled")]
    // Mendefinisikan properti `InsuranceEnabled` bertipe `bool` untuk nilai asuransi enabled; get menyediakan pembacaan nilai, init membatasi pengisian
    // saat inisialisasi objek.
    public bool InsuranceEnabled { get; init; }

    // memetakan nama properti JSON menjadi (”saving_goal_enabled”).
    [JsonPropertyName("saving_goal_enabled")]
    // Mendefinisikan properti `SavingGoalEnabled` bertipe `bool` untuk nilai tabungan target enabled; get menyediakan pembacaan nilai, init membatasi
    // pengisian saat inisialisasi objek.
    public bool SavingGoalEnabled { get; init; }

    // memetakan nama properti JSON menjadi (”freelance_income”).
    [JsonPropertyName("freelance_income")]
    // Mendefinisikan properti `FreelanceIncome` bertipe `int` untuk nilai freelance pemasukan; get menyediakan pembacaan nilai, init membatasi
    // pengisian saat inisialisasi objek; nilai awalnya nilai literal `1`.
    public int FreelanceIncome { get; init; } = 1;
// Menutup scope tipe RulesetSettingsDto; bagian berikut berada di luar batas blok tersebut.
}

// Mendefinisikan tipe class `RulesetPlayerOrderingDto`; sealed mencegah tipe ini diturunkan lagi.
public sealed class RulesetPlayerOrderingDto
// Membuka scope tipe RulesetPlayerOrderingDto; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // memetakan nama properti JSON menjadi (”ordering_code”).
    [JsonPropertyName("ordering_code")]
    // Mendefinisikan properti `OrderingCode` bertipe `string` untuk nilai ordering kode; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek; nilai awalnya nilai literal `”PLAYER_ORDER”`.
    public string OrderingCode { get; init; } = "PLAYER_ORDER";

    // memetakan nama properti JSON menjadi (”friday_feature”).
    [JsonPropertyName("friday_feature")]
    // Mendefinisikan properti `FridayFeature` bertipe `string` untuk nilai friday feature; get menyediakan pembacaan nilai, init membatasi pengisian
    // saat inisialisasi objek; nilai awalnya nilai literal `”DONATION”`.
    public string FridayFeature { get; init; } = "DONATION";

    // memetakan nama properti JSON menjadi (”friday_enabled”).
    [JsonPropertyName("friday_enabled")]
    // Mendefinisikan properti `FridayEnabled` bertipe `bool` untuk nilai friday enabled; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek; nilai awalnya true, yaitu kondisi aktif/terpenuhi.
    public bool FridayEnabled { get; init; } = true;

    // memetakan nama properti JSON menjadi (”saturday_feature”).
    [JsonPropertyName("saturday_feature")]
    // Mendefinisikan properti `SaturdayFeature` bertipe `string` untuk nilai saturday feature; get menyediakan pembacaan nilai, init membatasi
    // pengisian saat inisialisasi objek; nilai awalnya nilai literal `”GOLD_TRADE”`.
    public string SaturdayFeature { get; init; } = "GOLD_TRADE";

    // memetakan nama properti JSON menjadi (”saturday_enabled”).
    [JsonPropertyName("saturday_enabled")]
    // Mendefinisikan properti `SaturdayEnabled` bertipe `bool` untuk nilai saturday enabled; get menyediakan pembacaan nilai, init membatasi pengisian
    // saat inisialisasi objek; nilai awalnya true, yaitu kondisi aktif/terpenuhi.
    public bool SaturdayEnabled { get; init; } = true;

    // memetakan nama properti JSON menjadi (”sunday_feature”).
    [JsonPropertyName("sunday_feature")]
    // Mendefinisikan properti `SundayFeature` bertipe `string` untuk nilai sunday feature; get menyediakan pembacaan nilai, init membatasi pengisian
    // saat inisialisasi objek; nilai awalnya nilai literal `”REST”`.
    public string SundayFeature { get; init; } = "REST";

    // memetakan nama properti JSON menjadi (”sunday_enabled”).
    [JsonPropertyName("sunday_enabled")]
    // Mendefinisikan properti `SundayEnabled` bertipe `bool` untuk nilai sunday enabled; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek; nilai awalnya true, yaitu kondisi aktif/terpenuhi.
    public bool SundayEnabled { get; init; } = true;
// Menutup scope tipe RulesetPlayerOrderingDto; bagian berikut berada di luar batas blok tersebut.
}

// Mendefinisikan tipe class `RulesetActionDto`; sealed mencegah tipe ini diturunkan lagi.
public sealed class RulesetActionDto
// Membuka scope tipe RulesetActionDto; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // memetakan nama properti JSON menjadi (”action_id”).
    [JsonPropertyName("action_id")]
    // Mendefinisikan properti `ActionId` bertipe `string` untuk kode aksi yang dipetakan terhadap katalog aturan; get menyediakan pembacaan nilai, init
    // membatasi pengisian saat inisialisasi objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string ActionId { get; init; } = string.Empty;
// Menutup scope tipe RulesetActionDto; bagian berikut berada di luar batas blok tersebut.
}

// Mendefinisikan tipe class `RulesetIngredientDto`; sealed mencegah tipe ini diturunkan lagi.
public sealed class RulesetIngredientDto
// Membuka scope tipe RulesetIngredientDto; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // memetakan nama properti JSON menjadi (”id”).
    [JsonPropertyName("id")]
    // Mendefinisikan properti `Id` bertipe `string` untuk nilai identitas; get menyediakan pembacaan nilai, init membatasi pengisian saat inisialisasi
    // objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string Id { get; init; } = string.Empty;

    // memetakan nama properti JSON menjadi (”nama”).
    [JsonPropertyName("nama")]
    // Mendefinisikan properti `Nama` bertipe `string` untuk nilai nama; get menyediakan pembacaan nilai, init membatasi pengisian saat inisialisasi
    // objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string Nama { get; init; } = string.Empty;

    // memetakan nama properti JSON menjadi (”hargaBeli”).
    [JsonPropertyName("hargaBeli")]
    // Mendefinisikan properti `HargaBeli` bertipe `int` untuk nilai harga beli; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek.
    public int HargaBeli { get; init; }

    // memetakan nama properti JSON menjadi (”cardQty”).
    [JsonPropertyName("cardQty")]
    // Mendefinisikan properti `CardQty` bertipe `int?` untuk nilai kartu qty; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek; tanda ? mengizinkan nilai null.
    public int? CardQty { get; init; }
// Menutup scope tipe RulesetIngredientDto; bagian berikut berada di luar batas blok tersebut.
}

// Mendefinisikan tipe class `RulesetOrderDto`; sealed mencegah tipe ini diturunkan lagi.
public sealed class RulesetOrderDto
// Membuka scope tipe RulesetOrderDto; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // memetakan nama properti JSON menjadi (”id”).
    [JsonPropertyName("id")]
    // Mendefinisikan properti `Id` bertipe `string` untuk nilai identitas; get menyediakan pembacaan nilai, init membatasi pengisian saat inisialisasi
    // objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string Id { get; init; } = string.Empty;

    // memetakan nama properti JSON menjadi (”nama”).
    [JsonPropertyName("nama")]
    // Mendefinisikan properti `Nama` bertipe `string` untuk nilai nama; get menyediakan pembacaan nilai, init membatasi pengisian saat inisialisasi
    // objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string Nama { get; init; } = string.Empty;

    // memetakan nama properti JSON menjadi (”hargaJual”).
    [JsonPropertyName("hargaJual")]
    // Mendefinisikan properti `HargaJual` bertipe `int` untuk nilai harga jual; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek.
    public int HargaJual { get; init; }

    // memetakan nama properti JSON menjadi (”poinKebahagiaan”).
    [JsonPropertyName("poinKebahagiaan")]
    // Mendefinisikan properti `PoinKebahagiaan` bertipe `int` untuk nilai poin kebahagiaan; get menyediakan pembacaan nilai, init membatasi pengisian
    // saat inisialisasi objek.
    public int PoinKebahagiaan { get; init; }

    // memetakan nama properti JSON menjadi (”bahan”).
    [JsonPropertyName("bahan")]
    // Mendefinisikan properti `Bahan` bertipe `List<string>` untuk nilai bahan; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek; nilai awalnya koleksi kosong dengan tipe mengikuti konteks tujuan.
    public List<string> Bahan { get; init; } = [];

    // memetakan nama properti JSON menjadi (”cardQty”).
    [JsonPropertyName("cardQty")]
    // Mendefinisikan properti `CardQty` bertipe `int?` untuk nilai kartu qty; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek; tanda ? mengizinkan nilai null.
    public int? CardQty { get; init; }
// Menutup scope tipe RulesetOrderDto; bagian berikut berada di luar batas blok tersebut.
}

// Mendefinisikan tipe class `RulesetNeedDto`; sealed mencegah tipe ini diturunkan lagi.
public sealed class RulesetNeedDto
// Membuka scope tipe RulesetNeedDto; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // memetakan nama properti JSON menjadi (”id”).
    [JsonPropertyName("id")]
    // Mendefinisikan properti `Id` bertipe `string` untuk nilai identitas; get menyediakan pembacaan nilai, init membatasi pengisian saat inisialisasi
    // objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string Id { get; init; } = string.Empty;

    // memetakan nama properti JSON menjadi (”nama”).
    [JsonPropertyName("nama")]
    // Mendefinisikan properti `Nama` bertipe `string` untuk nilai nama; get menyediakan pembacaan nilai, init membatasi pengisian saat inisialisasi
    // objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string Nama { get; init; } = string.Empty;

    // memetakan nama properti JSON menjadi (”family”).
    [JsonPropertyName("family")]
    // Mendefinisikan properti `Family` bertipe `string?` untuk nilai kelompok; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek; tanda ? mengizinkan nilai null.
    public string? Family { get; init; }

    // memetakan nama properti JSON menjadi (”tipe”).
    [JsonPropertyName("tipe")]
    // Mendefinisikan properti `Tipe` bertipe `string` untuk nilai tipe; get menyediakan pembacaan nilai, init membatasi pengisian saat inisialisasi
    // objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string Tipe { get; init; } = string.Empty;

    // memetakan nama properti JSON menjadi (”hargaBeli”).
    [JsonPropertyName("hargaBeli")]
    // Mendefinisikan properti `HargaBeli` bertipe `int` untuk nilai harga beli; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek.
    public int HargaBeli { get; init; }

    // memetakan nama properti JSON menjadi (”poinKebahagiaan”).
    [JsonPropertyName("poinKebahagiaan")]
    // Mendefinisikan properti `PoinKebahagiaan` bertipe `int` untuk nilai poin kebahagiaan; get menyediakan pembacaan nilai, init membatasi pengisian
    // saat inisialisasi objek.
    public int PoinKebahagiaan { get; init; }

    // memetakan nama properti JSON menjadi (”cardQty”).
    [JsonPropertyName("cardQty")]
    // Mendefinisikan properti `CardQty` bertipe `int?` untuk nilai kartu qty; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek; tanda ? mengizinkan nilai null.
    public int? CardQty { get; init; }
// Menutup scope tipe RulesetNeedDto; bagian berikut berada di luar batas blok tersebut.
}

// Mendefinisikan tipe class `RulesetNeedSetBonusDto`; sealed mencegah tipe ini diturunkan lagi.
public sealed class RulesetNeedSetBonusDto
// Membuka scope tipe RulesetNeedSetBonusDto; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // memetakan nama properti JSON menjadi (”pattern_code”).
    [JsonPropertyName("pattern_code")]
    // Mendefinisikan properti `PatternCode` bertipe `string` untuk nilai pattern kode; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string PatternCode { get; init; } = string.Empty;

    // memetakan nama properti JSON menjadi (”required_count”).
    [JsonPropertyName("required_count")]
    // Mendefinisikan properti `RequiredCount` bertipe `int` untuk nilai required jumlah; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek.
    public int RequiredCount { get; init; }

    // memetakan nama properti JSON menjadi (”points”).
    [JsonPropertyName("points")]
    // Mendefinisikan properti `Points` bertipe `int` untuk nilai poin; get menyediakan pembacaan nilai, init membatasi pengisian saat inisialisasi
    // objek.
    public int Points { get; init; }
// Menutup scope tipe RulesetNeedSetBonusDto; bagian berikut berada di luar batas blok tersebut.
}

// Mendefinisikan tipe class `RulesetCollectionMissionDto`; sealed mencegah tipe ini diturunkan lagi.
public sealed class RulesetCollectionMissionDto
// Membuka scope tipe RulesetCollectionMissionDto; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // memetakan nama properti JSON menjadi (”id”).
    [JsonPropertyName("id")]
    // Mendefinisikan properti `Id` bertipe `string` untuk nilai identitas; get menyediakan pembacaan nilai, init membatasi pengisian saat inisialisasi
    // objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string Id { get; init; } = string.Empty;

    // memetakan nama properti JSON menjadi (”nama”).
    [JsonPropertyName("nama")]
    // Mendefinisikan properti `Nama` bertipe `string` untuk nilai nama; get menyediakan pembacaan nilai, init membatasi pengisian saat inisialisasi
    // objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string Nama { get; init; } = string.Empty;

    // memetakan nama properti JSON menjadi (”success_points”).
    [JsonPropertyName("success_points")]
    // Mendefinisikan properti `SuccessPoints` bertipe `int` untuk nilai success poin; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek.
    public int SuccessPoints { get; init; }

    // memetakan nama properti JSON menjadi (”failure_points”).
    [JsonPropertyName("failure_points")]
    // Mendefinisikan properti `FailurePoints` bertipe `int` untuk nilai failure poin; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek.
    public int FailurePoints { get; init; }

    // memetakan nama properti JSON menjadi (”penaltyPoints”).
    [JsonPropertyName("penaltyPoints")]
    // Mendefinisikan properti `PenaltyPoints` bertipe `int` untuk nilai penalti poin; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek.
    public int PenaltyPoints { get; init; }

    // memetakan nama properti JSON menjadi (”kebutuhanTarget”).
    [JsonPropertyName("kebutuhanTarget")]
    // Mendefinisikan properti `KebutuhanTarget` bertipe `List<RulesetCollectionMissionRequirementDto>` untuk nilai kebutuhan target; get menyediakan
    // pembacaan nilai, init membatasi pengisian saat inisialisasi objek; nilai awalnya koleksi kosong dengan tipe mengikuti konteks tujuan.
    public List<RulesetCollectionMissionRequirementDto> KebutuhanTarget { get; init; } = [];
// Menutup scope tipe RulesetCollectionMissionDto; bagian berikut berada di luar batas blok tersebut.
}

// Mendefinisikan tipe class `RulesetCollectionMissionRequirementDto`; sealed mencegah tipe ini diturunkan lagi.
public sealed class RulesetCollectionMissionRequirementDto
// Membuka scope tipe RulesetCollectionMissionRequirementDto; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // memetakan nama properti JSON menjadi (”order”).
    [JsonPropertyName("order")]
    // Mendefinisikan properti `Order` bertipe `int` untuk nilai urutan/pesanan; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek.
    public int Order { get; init; }

    // memetakan nama properti JSON menjadi (”type”).
    [JsonPropertyName("type")]
    // Mendefinisikan properti `Type` bertipe `string` untuk nilai jenis; get menyediakan pembacaan nilai, init membatasi pengisian saat inisialisasi
    // objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string Type { get; init; } = string.Empty;

    // memetakan nama properti JSON menjadi (”value”).
    [JsonPropertyName("value")]
    // Mendefinisikan properti `Value` bertipe `string` untuk nilai nilai; get menyediakan pembacaan nilai, init membatasi pengisian saat inisialisasi
    // objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string Value { get; init; } = string.Empty;
// Menutup scope tipe RulesetCollectionMissionRequirementDto; bagian berikut berada di luar batas blok tersebut.
}

// Mendefinisikan tipe class `RulesetFinancialGoalDto`; sealed mencegah tipe ini diturunkan lagi.
public sealed class RulesetFinancialGoalDto
// Membuka scope tipe RulesetFinancialGoalDto; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // memetakan nama properti JSON menjadi (”id”).
    [JsonPropertyName("id")]
    // Mendefinisikan properti `Id` bertipe `string` untuk nilai identitas; get menyediakan pembacaan nilai, init membatasi pengisian saat inisialisasi
    // objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string Id { get; init; } = string.Empty;

    // memetakan nama properti JSON menjadi (”nama”).
    [JsonPropertyName("nama")]
    // Mendefinisikan properti `Nama` bertipe `string` untuk nilai nama; get menyediakan pembacaan nilai, init membatasi pengisian saat inisialisasi
    // objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string Nama { get; init; } = string.Empty;

    // memetakan nama properti JSON menjadi (”hargaBeli”).
    [JsonPropertyName("hargaBeli")]
    // Mendefinisikan properti `HargaBeli` bertipe `int` untuk nilai harga beli; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek.
    public int HargaBeli { get; init; }

    // memetakan nama properti JSON menjadi (”poinKebahagiaan”).
    [JsonPropertyName("poinKebahagiaan")]
    // Mendefinisikan properti `PoinKebahagiaan` bertipe `int` untuk nilai poin kebahagiaan; get menyediakan pembacaan nilai, init membatasi pengisian
    // saat inisialisasi objek.
    public int PoinKebahagiaan { get; init; }

    // memetakan nama properti JSON menjadi (”cardQty”).
    [JsonPropertyName("cardQty")]
    // Mendefinisikan properti `CardQty` bertipe `int?` untuk nilai kartu qty; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek; tanda ? mengizinkan nilai null.
    public int? CardQty { get; init; }
// Menutup scope tipe RulesetFinancialGoalDto; bagian berikut berada di luar batas blok tersebut.
}

// Mendefinisikan tipe class `RulesetNarrativeDto`; sealed mencegah tipe ini diturunkan lagi.
public sealed class RulesetNarrativeDto
// Membuka scope tipe RulesetNarrativeDto; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // memetakan nama properti JSON menjadi (”id”).
    [JsonPropertyName("id")]
    // Mendefinisikan properti `Id` bertipe `string` untuk nilai identitas; get menyediakan pembacaan nilai, init membatasi pengisian saat inisialisasi
    // objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string Id { get; init; } = string.Empty;

    // memetakan nama properti JSON menjadi (”nama”).
    [JsonPropertyName("nama")]
    // Mendefinisikan properti `Nama` bertipe `string` untuk nilai nama; get menyediakan pembacaan nilai, init membatasi pengisian saat inisialisasi
    // objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string Nama { get; init; } = string.Empty;

    // memetakan nama properti JSON menjadi (”teks”).
    [JsonPropertyName("teks")]
    // Mendefinisikan properti `Teks` bertipe `List<string>` untuk nilai teks; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek; nilai awalnya koleksi kosong dengan tipe mengikuti konteks tujuan.
    public List<string> Teks { get; init; } = [];

    // memetakan nama properti JSON menjadi (”prerequisiteAksi”).
    [JsonPropertyName("prerequisiteAksi")]
    // Mendefinisikan properti `PrerequisiteAksi` bertipe `List<RulesetNarrativePrerequisiteDto>` untuk nilai prerequisite aksi; get menyediakan
    // pembacaan nilai, init membatasi pengisian saat inisialisasi objek; nilai awalnya koleksi kosong dengan tipe mengikuti konteks tujuan.
    public List<RulesetNarrativePrerequisiteDto> PrerequisiteAksi { get; init; } = [];

// Menutup scope tipe RulesetNarrativeDto; bagian berikut berada di luar batas blok tersebut.
}

// Mendefinisikan tipe class `RulesetNarrativePrerequisiteDto`; sealed mencegah tipe ini diturunkan lagi.
public sealed class RulesetNarrativePrerequisiteDto
// Membuka scope tipe RulesetNarrativePrerequisiteDto; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // memetakan nama properti JSON menjadi (”aksi”).
    [JsonPropertyName("aksi")]
    // Mendefinisikan properti `Aksi` bertipe `string` untuk nilai aksi; get menyediakan pembacaan nilai, init membatasi pengisian saat inisialisasi
    // objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string Aksi { get; init; } = string.Empty;

    // memetakan nama properti JSON menjadi (”value”).
    [JsonPropertyName("value")]
    // Mendefinisikan properti `Value` bertipe `int` untuk nilai nilai; get menyediakan pembacaan nilai, init membatasi pengisian saat inisialisasi
    // objek.
    public int Value { get; init; }
// Menutup scope tipe RulesetNarrativePrerequisiteDto; bagian berikut berada di luar batas blok tersebut.
}

// Mendefinisikan tipe class `RulesetDonationRankPointDto`; sealed mencegah tipe ini diturunkan lagi.
public sealed class RulesetDonationRankPointDto
// Membuka scope tipe RulesetDonationRankPointDto; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // memetakan nama properti JSON menjadi (”rank”).
    [JsonPropertyName("rank")]
    // Mendefinisikan properti `Rank` bertipe `int` untuk nilai rank; get menyediakan pembacaan nilai, init membatasi pengisian saat inisialisasi objek.
    public int Rank { get; init; }

    // memetakan nama properti JSON menjadi (”points”).
    [JsonPropertyName("points")]
    // Mendefinisikan properti `Points` bertipe `int` untuk nilai poin; get menyediakan pembacaan nilai, init membatasi pengisian saat inisialisasi
    // objek.
    public int Points { get; init; }
// Menutup scope tipe RulesetDonationRankPointDto; bagian berikut berada di luar batas blok tersebut.
}

// Mendefinisikan tipe class `RulesetGoldPointDto`; sealed mencegah tipe ini diturunkan lagi.
public sealed class RulesetGoldPointDto
// Membuka scope tipe RulesetGoldPointDto; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // memetakan nama properti JSON menjadi (”qty”).
    [JsonPropertyName("qty")]
    // Mendefinisikan properti `Qty` bertipe `int` untuk nilai qty; get menyediakan pembacaan nilai, init membatasi pengisian saat inisialisasi objek.
    public int Qty { get; init; }

    // memetakan nama properti JSON menjadi (”points”).
    [JsonPropertyName("points")]
    // Mendefinisikan properti `Points` bertipe `int` untuk nilai poin; get menyediakan pembacaan nilai, init membatasi pengisian saat inisialisasi
    // objek.
    public int Points { get; init; }
// Menutup scope tipe RulesetGoldPointDto; bagian berikut berada di luar batas blok tersebut.
}

// Mendefinisikan tipe class `RulesetGoldPriceDto`; sealed mencegah tipe ini diturunkan lagi.
public sealed class RulesetGoldPriceDto
// Membuka scope tipe RulesetGoldPriceDto; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // memetakan nama properti JSON menjadi (”price_code”).
    [JsonPropertyName("price_code")]
    // Mendefinisikan properti `PriceCode` bertipe `string` untuk nilai harga kode; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string PriceCode { get; init; } = string.Empty;

    // memetakan nama properti JSON menjadi (”qty”).
    [JsonPropertyName("qty")]
    // Mendefinisikan properti `Qty` bertipe `int` untuk nilai qty; get menyediakan pembacaan nilai, init membatasi pengisian saat inisialisasi objek.
    public int Qty { get; init; }

    // memetakan nama properti JSON menjadi (”unit_price”).
    [JsonPropertyName("unit_price")]
    // Mendefinisikan properti `UnitPrice` bertipe `int` untuk nilai unit harga; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek.
    public int UnitPrice { get; init; }

    // memetakan nama properti JSON menjadi (”card_qty”).
    [JsonPropertyName("card_qty")]
    // Mendefinisikan properti `CardQty` bertipe `int?` untuk nilai kartu qty; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek; tanda ? mengizinkan nilai null.
    public int? CardQty { get; init; }
// Menutup scope tipe RulesetGoldPriceDto; bagian berikut berada di luar batas blok tersebut.
}

// Mendefinisikan tipe class `RulesetPensionRankPointDto`; sealed mencegah tipe ini diturunkan lagi.
public sealed class RulesetPensionRankPointDto
// Membuka scope tipe RulesetPensionRankPointDto; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // memetakan nama properti JSON menjadi (”rank”).
    [JsonPropertyName("rank")]
    // Mendefinisikan properti `Rank` bertipe `int` untuk nilai rank; get menyediakan pembacaan nilai, init membatasi pengisian saat inisialisasi objek.
    public int Rank { get; init; }

    // memetakan nama properti JSON menjadi (”points”).
    [JsonPropertyName("points")]
    // Mendefinisikan properti `Points` bertipe `int` untuk nilai poin; get menyediakan pembacaan nilai, init membatasi pengisian saat inisialisasi
    // objek.
    public int Points { get; init; }
// Menutup scope tipe RulesetPensionRankPointDto; bagian berikut berada di luar batas blok tersebut.
}

// Mendefinisikan tipe class `RulesetTieBreakerDto`; sealed mencegah tipe ini diturunkan lagi.
public sealed class RulesetTieBreakerDto
// Membuka scope tipe RulesetTieBreakerDto; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // memetakan nama properti JSON menjadi (”tie_breaker_code”).
    [JsonPropertyName("tie_breaker_code")]
    // Mendefinisikan properti `TieBreakerCode` bertipe `string` untuk kode kartu penentu urutan saat nilai pemain sama; get menyediakan pembacaan
    // nilai, init membatasi pengisian saat inisialisasi objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string TieBreakerCode { get; init; } = string.Empty;

    // memetakan nama properti JSON menjadi (”tie_number”).
    [JsonPropertyName("tie_number")]
    // Mendefinisikan properti `TieNumber` bertipe `int` untuk nilai tie number; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek.
    public int TieNumber { get; init; }

    // memetakan nama properti JSON menjadi (”card_qty”).
    [JsonPropertyName("card_qty")]
    // Mendefinisikan properti `CardQty` bertipe `int?` untuk nilai kartu qty; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek; tanda ? mengizinkan nilai null.
    public int? CardQty { get; init; }
// Menutup scope tipe RulesetTieBreakerDto; bagian berikut berada di luar batas blok tersebut.
}

// Mendefinisikan tipe class `RulesetShariaLoanDto`; sealed mencegah tipe ini diturunkan lagi.
public sealed class RulesetShariaLoanDto
// Membuka scope tipe RulesetShariaLoanDto; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // memetakan nama properti JSON menjadi (”loan_code”).
    [JsonPropertyName("loan_code")]
    // Mendefinisikan properti `LoanCode` bertipe `string` untuk kode produk pinjaman syariah; get menyediakan pembacaan nilai, init membatasi pengisian
    // saat inisialisasi objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string LoanCode { get; init; } = string.Empty;

    // memetakan nama properti JSON menjadi (”item_name”).
    [JsonPropertyName("item_name")]
    // Mendefinisikan properti `ItemName` bertipe `string` untuk nilai elemen nama; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string ItemName { get; init; } = string.Empty;

    // memetakan nama properti JSON menjadi (”principal”).
    [JsonPropertyName("principal")]
    // Mendefinisikan properti `Principal` bertipe `int` untuk nilai principal; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek.
    public int Principal { get; init; }

    // memetakan nama properti JSON menjadi (”repayment_amount”).
    [JsonPropertyName("repayment_amount")]
    // Mendefinisikan properti `RepaymentAmount` bertipe `int` untuk nilai repayment nominal; get menyediakan pembacaan nilai, init membatasi pengisian
    // saat inisialisasi objek.
    public int RepaymentAmount { get; init; }

    // memetakan nama properti JSON menjadi (”duration_days”).
    [JsonPropertyName("duration_days")]
    // Mendefinisikan properti `DurationDays` bertipe `int` untuk nilai duration hari; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek.
    public int DurationDays { get; init; }

    // memetakan nama properti JSON menjadi (”penalty_points”).
    [JsonPropertyName("penalty_points")]
    // Mendefinisikan properti `PenaltyPoints` bertipe `int` untuk nilai penalti poin; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek.
    public int PenaltyPoints { get; init; }

    // memetakan nama properti JSON menjadi (”card_qty”).
    [JsonPropertyName("card_qty")]
    // Mendefinisikan properti `CardQty` bertipe `int?` untuk nilai kartu qty; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek; tanda ? mengizinkan nilai null.
    public int? CardQty { get; init; }
// Menutup scope tipe RulesetShariaLoanDto; bagian berikut berada di luar batas blok tersebut.
}

// Mendefinisikan tipe class `RulesetInsuranceProductDto`; sealed mencegah tipe ini diturunkan lagi.
public sealed class RulesetInsuranceProductDto
// Membuka scope tipe RulesetInsuranceProductDto; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // memetakan nama properti JSON menjadi (”product_code”).
    [JsonPropertyName("product_code")]
    // Mendefinisikan properti `ProductCode` bertipe `string` untuk nilai product kode; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string ProductCode { get; init; } = string.Empty;

    // memetakan nama properti JSON menjadi (”item_name”).
    [JsonPropertyName("item_name")]
    // Mendefinisikan properti `ItemName` bertipe `string` untuk nilai elemen nama; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string ItemName { get; init; } = string.Empty;

    // memetakan nama properti JSON menjadi (”premium”).
    [JsonPropertyName("premium")]
    // Mendefinisikan properti `Premium` bertipe `int` untuk nilai premium; get menyediakan pembacaan nilai, init membatasi pengisian saat inisialisasi
    // objek.
    public int Premium { get; init; }

    // memetakan nama properti JSON menjadi (”usage_limit”).
    [JsonPropertyName("usage_limit")]
    // Mendefinisikan properti `UsageLimit` bertipe `int` untuk nilai usage limit; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek.
    public int UsageLimit { get; init; }

    // memetakan nama properti JSON menjadi (”card_qty”).
    [JsonPropertyName("card_qty")]
    // Mendefinisikan properti `CardQty` bertipe `int?` untuk nilai kartu qty; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek; tanda ? mengizinkan nilai null.
    public int? CardQty { get; init; }
// Menutup scope tipe RulesetInsuranceProductDto; bagian berikut berada di luar batas blok tersebut.
}

// Mendefinisikan tipe class `RulesetLifeRiskDto`; sealed mencegah tipe ini diturunkan lagi.
public sealed class RulesetLifeRiskDto
// Membuka scope tipe RulesetLifeRiskDto; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // memetakan nama properti JSON menjadi (”risk_code”).
    [JsonPropertyName("risk_code")]
    // Mendefinisikan properti `RiskCode` bertipe `string` untuk nilai risiko kode; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string RiskCode { get; init; } = string.Empty;

    // memetakan nama properti JSON menjadi (”item_name”).
    [JsonPropertyName("item_name")]
    // Mendefinisikan properti `ItemName` bertipe `string` untuk nilai elemen nama; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string ItemName { get; init; } = string.Empty;

    // memetakan nama properti JSON menjadi (”effect_type”).
    [JsonPropertyName("effect_type")]
    // Mendefinisikan properti `EffectType` bertipe `string` untuk nilai effect jenis; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string EffectType { get; init; } = string.Empty;

    // memetakan nama properti JSON menjadi (”direction”).
    [JsonPropertyName("direction")]
    // Mendefinisikan properti `Direction` bertipe `string` untuk nilai direction; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string Direction { get; init; } = string.Empty;

    // memetakan nama properti JSON menjadi (”amount”).
    [JsonPropertyName("amount")]
    // Mendefinisikan properti `Amount` bertipe `int` untuk nominal uang atau nilai transaksi yang dipakai dalam operasi; get menyediakan pembacaan
    // nilai, init membatasi pengisian saat inisialisasi objek.
    public int Amount { get; init; }

    // memetakan nama properti JSON menjadi (”target_scope”).
    [JsonPropertyName("target_scope")]
    // Mendefinisikan properti `TargetScope` bertipe `string` untuk nilai target cakupan; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek; nilai awalnya nilai literal `”SELF”`.
    public string TargetScope { get; init; } = "SELF";

    // memetakan nama properti JSON menjadi (”value_delta”).
    [JsonPropertyName("value_delta")]
    // Mendefinisikan properti `ValueDelta` bertipe `int?` untuk nilai nilai delta; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek; tanda ? mengizinkan nilai null.
    public int? ValueDelta { get; init; }

    // memetakan nama properti JSON menjadi (”duration_days”).
    [JsonPropertyName("duration_days")]
    // Mendefinisikan properti `DurationDays` bertipe `int?` untuk nilai duration hari; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek; tanda ? mengizinkan nilai null.
    public int? DurationDays { get; init; }

    // memetakan nama properti JSON menjadi (”card_qty”).
    [JsonPropertyName("card_qty")]
    // Mendefinisikan properti `CardQty` bertipe `int?` untuk nilai kartu qty; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek; tanda ? mengizinkan nilai null.
    public int? CardQty { get; init; }
// Menutup scope tipe RulesetLifeRiskDto; bagian berikut berada di luar batas blok tersebut.
}
