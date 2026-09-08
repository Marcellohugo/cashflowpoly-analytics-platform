// Fungsi file: Menjelaskan implementasi RulesetDefinitionMapper pada platform analitika Cashflowpoly.
// Mengimpor namespace `System.Text.Json` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.Json;
// Mengimpor namespace `System.Text.Json.Nodes` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.Json.Nodes;
// Mengimpor namespace `Cashflowpoly.Ui.Contracts` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Ui.Contracts;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Domain;

// Mendefinisikan tipe class `RulesetDefinitionMapper`.
internal static class RulesetDefinitionMapper
// Membuka scope tipe RulesetDefinitionMapper; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan metode `FromConfigJson` dengan hasil bertipe `RulesetDefinitionDto`; operasi ini menangani dari konfigurasi JSON. Masukan:
    // Parameter `configJson` bertipe `string` membawa nilai konfigurasi JSON.
    internal static RulesetDefinitionDto FromConfigJson(string configJson)
    // Membuka scope metode FromConfigJson; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam FromConfigJson.
    {
        // Menyiapkan variabel lokal `document` untuk nilai document dengan memanggil `JsonDocument.Parse` dengan `configJson`. Tipe variabel disimpulkan
        // dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var document = JsonDocument.Parse(configJson);
        // Menyiapkan variabel lokal `root` untuk nilai root dengan `document.RootElement` (nilai root element). Tipe variabel disimpulkan dari ekspresi
        // nilai awal.
        var root = document.RootElement;
        // Menyiapkan variabel lokal `componentCatalog` untuk nilai komponen catalog dengan memanggil `root.GetProperty` dengan `”component_catalog”`. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var componentCatalog = root.GetProperty("component_catalog");
        // Menyiapkan variabel lokal `gameConfig` untuk nilai game konfigurasi dengan memanggil `componentCatalog.GetProperty` dengan `”gameConfig”`. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var gameConfig = componentCatalog.GetProperty("gameConfig");
        // Menyiapkan variabel lokal `weekdayRules` untuk nilai weekday rules dengan memanggil `root.GetProperty` dengan `”weekday_rules”`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var weekdayRules = root.GetProperty("weekday_rules");
        // Menyiapkan variabel lokal `constraints` untuk nilai constraints dengan memanggil `root.GetProperty` dengan `”constraints”`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var constraints = root.GetProperty("constraints");
        // Menyiapkan variabel lokal `donation` untuk nilai donasi dengan memanggil `root.GetProperty` dengan `”donation”`. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var donation = root.GetProperty("donation");
        // Menyiapkan variabel lokal `goldTrade` untuk nilai emas trade dengan memanggil `root.GetProperty` dengan `”gold_trade”`. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var goldTrade = root.GetProperty("gold_trade");
        // Menyiapkan variabel lokal `advanced` untuk nilai advanced dengan memanggil `root.GetProperty` dengan `”advanced”`. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var advanced = root.GetProperty("advanced");
        // Menyiapkan variabel lokal `scoring` untuk nilai scoring dengan hasil pemilihan bersyarat: ketika `root.TryGetProperty(”scoring”, out var
        // scoringElement)` benar gunakan `scoringElement`, jika tidak gunakan `default`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var scoring = root.TryGetProperty("scoring", out var scoringElement) ? scoringElement : default;
        // Menyiapkan variabel lokal `freelance` untuk nilai freelance dengan hasil pemilihan bersyarat: ketika `root.TryGetProperty(”freelance”, out var
        // freelanceElement)` benar gunakan `freelanceElement`, jika tidak gunakan `default`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var freelance = root.TryGetProperty("freelance", out var freelanceElement) ? freelanceElement : default;

        // Mengembalikan objek baru bertipe `RulesetDefinitionDto` dengan nilai awal sesuai konstruktornya kepada pemanggil dalam FromConfigJson; eksekusi
        // jalur ini selesai setelah nilai hasil ditentukan.
        return new RulesetDefinitionDto
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam FromConfigJson.
        {
            // Memperbarui `Mode` menggunakan memanggil `ReadString` dengan `root`, `”mode”`, `”MAHIR”` dalam FromConfigJson.
            Mode = ReadString(root, "mode", "MAHIR"),
            // Memperbarui `Settings` menggunakan objek baru bertipe `RulesetSettingsDto` dengan nilai awal sesuai konstruktornya dalam FromConfigJson.
            Settings = new RulesetSettingsDto
            // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam FromConfigJson.
            {
                // Memperbarui `ActionsPerTurn` menggunakan memanggil `ReadInt` dengan `root`, `”actions_per_turn”`, `2` dalam FromConfigJson.
                ActionsPerTurn = ReadInt(root, "actions_per_turn", 2),
                // Memperbarui `StartingCash` menggunakan memanggil `ReadInt` dengan `root`, `”starting_cash”`, `ReadInt(gameConfig, ”initialCoins”, 0)` dalam
                // FromConfigJson.
                StartingCash = ReadInt(root, "starting_cash", ReadInt(gameConfig, "initialCoins", 0)),
                // Memperbarui `InitialCoins` menggunakan memanggil `ReadInt` dengan `gameConfig`, `”initialCoins”`, `ReadInt(root, ”starting_cash”, 0)` dalam
                // FromConfigJson.
                InitialCoins = ReadInt(gameConfig, "initialCoins", ReadInt(root, "starting_cash", 0)),
                // Memperbarui `InitialHappiness` menggunakan memanggil `ReadInt` dengan `gameConfig`, `”initialHappiness”`, `0` dalam FromConfigJson.
                InitialHappiness = ReadInt(gameConfig, "initialHappiness", 0),
                // Memperbarui `InitialSaving` menggunakan memanggil `ReadInt` dengan `gameConfig`, `”initialSaving”`, `0` dalam FromConfigJson.
                InitialSaving = ReadInt(gameConfig, "initialSaving", 0),
                // Memperbarui `FinishDay` menggunakan memanggil `ReadInt` dengan `gameConfig`, `”finishDay”`, `25` dalam FromConfigJson.
                FinishDay = ReadInt(gameConfig, "finishDay", 25),
                // Memperbarui `MinPlayers` menggunakan memanggil `ReadInt` dengan `gameConfig`, `”minPlayers”`, `2` dalam FromConfigJson.
                MinPlayers = ReadInt(gameConfig, "minPlayers", 2),
                // Memperbarui `MaxPlayers` menggunakan memanggil `ReadInt` dengan `gameConfig`, `”maxPlayers”`, `4` dalam FromConfigJson.
                MaxPlayers = ReadInt(gameConfig, "maxPlayers", 4),
                // Memperbarui `CashMin` menggunakan memanggil `ReadInt` dengan `constraints`, `”cash_min”`, `0` dalam FromConfigJson.
                CashMin = ReadInt(constraints, "cash_min", 0),
                // Memperbarui `MaxIngredientTotal` menggunakan memanggil `ReadInt` dengan `constraints`, `”max_ingredient_total”`, `0` dalam FromConfigJson.
                MaxIngredientTotal = ReadInt(constraints, "max_ingredient_total", 0),
                // Memperbarui `MaxSameIngredient` menggunakan memanggil `ReadInt` dengan `constraints`, `”max_same_ingredient”`, `0` dalam FromConfigJson.
                MaxSameIngredient = ReadInt(constraints, "max_same_ingredient", 0),
                // Memperbarui `PrimaryNeedMaxPerDay` menggunakan memanggil `ReadInt` dengan `constraints`, `”primary_need_max_per_day”`, `0` dalam FromConfigJson.
                PrimaryNeedMaxPerDay = ReadInt(constraints, "primary_need_max_per_day", 0),
                // Memperbarui `RequirePrimaryBeforeOthers` menggunakan memanggil `ReadBool` dengan `constraints`, `”require_primary_before_others”`, `true` dalam
                // FromConfigJson.
                RequirePrimaryBeforeOthers = ReadBool(constraints, "require_primary_before_others", true),
                // Memperbarui `DonationMinAmount` menggunakan memanggil `ReadInt` dengan `donation`, `”min_amount”`, `1` dalam FromConfigJson.
                DonationMinAmount = ReadInt(donation, "min_amount", 1),
                // Memperbarui `DonationMaxAmount` menggunakan memanggil `ReadInt` dengan `donation`, `”max_amount”`, `1` dalam FromConfigJson.
                DonationMaxAmount = ReadInt(donation, "max_amount", 1),
                // Memperbarui `GoldTradeAllowBuy` menggunakan memanggil `ReadBool` dengan `goldTrade`, `”allow_buy”`, `true` dalam FromConfigJson.
                GoldTradeAllowBuy = ReadBool(goldTrade, "allow_buy", true),
                // Memperbarui `GoldTradeAllowSell` menggunakan memanggil `ReadBool` dengan `goldTrade`, `”allow_sell”`, `true` dalam FromConfigJson.
                GoldTradeAllowSell = ReadBool(goldTrade, "allow_sell", true),
                // Memperbarui `LoanEnabled` menggunakan memanggil `ReadNestedBool` dengan `advanced`, `”loan”`, `”enabled”` dalam FromConfigJson.
                LoanEnabled = ReadNestedBool(advanced, "loan", "enabled"),
                // Memperbarui `InsuranceEnabled` menggunakan memanggil `ReadNestedBool` dengan `advanced`, `”insurance”`, `”enabled”` dalam FromConfigJson.
                InsuranceEnabled = ReadNestedBool(advanced, "insurance", "enabled"),
                // Memperbarui `SavingGoalEnabled` menggunakan memanggil `ReadNestedBool` dengan `advanced`, `”saving_goal”`, `”enabled”` dalam FromConfigJson.
                SavingGoalEnabled = ReadNestedBool(advanced, "saving_goal", "enabled"),
                // Memperbarui `FreelanceIncome` menggunakan hasil pemilihan bersyarat: ketika `freelance.ValueKind == JsonValueKind.Object` benar gunakan
                // `ReadInt(freelance, ”income”, 1)`, jika tidak gunakan `1` dalam FromConfigJson.
                FreelanceIncome = freelance.ValueKind == JsonValueKind.Object ? ReadInt(freelance, "income", 1) : 1
            // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam FromConfigJson.
            },
            // Memperbarui `PlayerOrdering` menggunakan objek baru bertipe `RulesetPlayerOrderingDto` dengan nilai awal sesuai konstruktornya dalam
            // FromConfigJson.
            PlayerOrdering = new RulesetPlayerOrderingDto
            // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam FromConfigJson.
            {
                // Memperbarui `OrderingCode` menggunakan memanggil `ReadString` dengan `root`, `”player_ordering”`, `”PLAYER_ORDER”` dalam FromConfigJson.
                OrderingCode = ReadString(root, "player_ordering", "PLAYER_ORDER"),
                // Memperbarui `FridayFeature` menggunakan memanggil `ReadNestedWeekdayFeature` dengan `weekdayRules`, `”friday”`, `”FRI”`, `”DONATION”` dalam
                // FromConfigJson.
                FridayFeature = ReadNestedWeekdayFeature(weekdayRules, "friday", "FRI", "DONATION"),
                // Memperbarui `FridayEnabled` menggunakan memanggil `ReadNestedWeekdayEnabled` dengan `weekdayRules`, `”friday”`, `”FRI”`, `true` dalam
                // FromConfigJson.
                FridayEnabled = ReadNestedWeekdayEnabled(weekdayRules, "friday", "FRI", true),
                // Memperbarui `SaturdayFeature` menggunakan memanggil `ReadNestedWeekdayFeature` dengan `weekdayRules`, `”saturday”`, `”SAT”`, `”GOLD_TRADE”` dalam
                // FromConfigJson.
                SaturdayFeature = ReadNestedWeekdayFeature(weekdayRules, "saturday", "SAT", "GOLD_TRADE"),
                // Memperbarui `SaturdayEnabled` menggunakan memanggil `ReadNestedWeekdayEnabled` dengan `weekdayRules`, `”saturday”`, `”SAT”`, `true` dalam
                // FromConfigJson.
                SaturdayEnabled = ReadNestedWeekdayEnabled(weekdayRules, "saturday", "SAT", true),
                // Memperbarui `SundayFeature` menggunakan memanggil `ReadNestedWeekdayFeature` dengan `weekdayRules`, `”sunday”`, `”SUN”`, `”REST”` dalam
                // FromConfigJson.
                SundayFeature = ReadNestedWeekdayFeature(weekdayRules, "sunday", "SUN", "REST"),
                // Memperbarui `SundayEnabled` menggunakan memanggil `ReadNestedWeekdayEnabled` dengan `weekdayRules`, `”sunday”`, `”SUN”`, `true` dalam
                // FromConfigJson.
                SundayEnabled = ReadNestedWeekdayEnabled(weekdayRules, "sunday", "SUN", true)
            // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam FromConfigJson.
            },
            // Memperbarui `Ingredients` menggunakan memanggil `ReadIngredients` dengan `componentCatalog` dalam FromConfigJson.
            Ingredients = ReadIngredients(componentCatalog),
            // Memperbarui `Orders` menggunakan memanggil `ReadOrders` dengan `componentCatalog` dalam FromConfigJson.
            Orders = ReadOrders(componentCatalog),
            // Memperbarui `Needs` menggunakan memanggil `ReadNeeds` dengan `componentCatalog` dalam FromConfigJson.
            Needs = ReadNeeds(componentCatalog),
            // Memperbarui `CollectionMissions` menggunakan memanggil `ReadCollectionMissions` dengan `componentCatalog` dalam FromConfigJson.
            CollectionMissions = ReadCollectionMissions(componentCatalog),
            // Memperbarui `FinancialGoals` menggunakan memanggil `ReadFinancialGoals` dengan `componentCatalog` dalam FromConfigJson.
            FinancialGoals = ReadFinancialGoals(componentCatalog),
            // Memperbarui `Narratives` menggunakan memanggil `ReadNarratives` dengan `componentCatalog` dalam FromConfigJson.
            Narratives = ReadNarratives(componentCatalog),
            // Memperbarui `DonationRankPoints` menggunakan mematerialisasi urutan `ReadRankPoints(scoring, ”donation_rank_points”) .Select(item => new
            // RulesetDonationRankPointDto { Rank = item.Rank, Points = item.Points })` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori
            // dalam FromConfigJson.
            DonationRankPoints = ReadRankPoints(scoring, "donation_rank_points")
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Select(item => new RulesetDonationRankPointDto { Rank = item.Rank, Points =
                // item.Points }) dalam FromConfigJson; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
                .Select(item => new RulesetDonationRankPointDto { Rank = item.Rank, Points = item.Points })
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList(), dalam FromConfigJson; token pada baris ini menyambungkan bagian kode
                // sebelum dan sesudahnya.
                .ToList(),
            // Memperbarui `GoldPointsByQty` menggunakan mematerialisasi urutan `ReadQtyPoints(scoring, ”gold_points_by_qty”) .Select(item => new
            // RulesetGoldPointDto { Qty = item.Qty, Points = item.Points })` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori dalam
            // FromConfigJson.
            GoldPointsByQty = ReadQtyPoints(scoring, "gold_points_by_qty")
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Select(item => new RulesetGoldPointDto { Qty = item.Qty, Points = item.Points
                // }) dalam FromConfigJson; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
                .Select(item => new RulesetGoldPointDto { Qty = item.Qty, Points = item.Points })
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList(), dalam FromConfigJson; token pada baris ini menyambungkan bagian kode
                // sebelum dan sesudahnya.
                .ToList(),
            // Memperbarui `PensionRankPoints` menggunakan mematerialisasi urutan `ReadRankPoints(scoring, ”pension_rank_points”) .Select(item => new
            // RulesetPensionRankPointDto { Rank = item.Rank, Points = item.Points })` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori
            // dalam FromConfigJson.
            PensionRankPoints = ReadRankPoints(scoring, "pension_rank_points")
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Select(item => new RulesetPensionRankPointDto { Rank = item.Rank, Points =
                // item.Points }) dalam FromConfigJson; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
                .Select(item => new RulesetPensionRankPointDto { Rank = item.Rank, Points = item.Points })
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList(), dalam FromConfigJson; token pada baris ini menyambungkan bagian kode
                // sebelum dan sesudahnya.
                .ToList(),
            // Memperbarui `NeedSetBonuses` menggunakan memanggil `ReadNeedSetBonuses` dengan `root` dalam FromConfigJson.
            NeedSetBonuses = ReadNeedSetBonuses(root),
            // Memperbarui `GoldPrices` menggunakan memanggil `ReadGoldPrices` dengan `root` dalam FromConfigJson.
            GoldPrices = ReadGoldPrices(root),
            // Memperbarui `TieBreakers` menggunakan memanggil `ReadTieBreakers` dengan `root` dalam FromConfigJson.
            TieBreakers = ReadTieBreakers(root),
            // Memperbarui `ShariaLoans` menggunakan memanggil `ReadShariaLoans` dengan `root` dalam FromConfigJson.
            ShariaLoans = ReadShariaLoans(root),
            // Memperbarui `InsuranceProducts` menggunakan memanggil `ReadInsuranceProducts` dengan `root` dalam FromConfigJson.
            InsuranceProducts = ReadInsuranceProducts(root),
            // Memperbarui `LifeRisks` menggunakan memanggil `ReadLifeRisks` dengan `root` dalam FromConfigJson.
            LifeRisks = ReadLifeRisks(root)
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam FromConfigJson.
        };
    // Menutup scope metode FromConfigJson; bagian berikut berada di luar batas blok tersebut dalam FromConfigJson.
    }

    // Mendefinisikan metode `ToConfigJson` dengan hasil bertipe `string`; operasi ini menangani ke konfigurasi JSON. Masukan: Parameter `definition`
    // bertipe `RulesetDefinitionDto` membawa definisi terstruktur komponen serta parameter aturan permainan.
    internal static string ToConfigJson(RulesetDefinitionDto definition)
    // Membuka scope metode ToConfigJson; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ToConfigJson.
    {
        // Menyiapkan variabel lokal `root` untuk nilai root dengan objek baru bertipe `JsonObject` dengan nilai awal sesuai konstruktornya. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var root = new JsonObject
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ToConfigJson.
        {
            // Memperbarui `[”mode”]` menggunakan `definition.Mode` (mode permainan yang menentukan kelompok aturan yang digunakan) dalam ToConfigJson.
            ["mode"] = definition.Mode,
            // Memperbarui `[”actions_per_turn”]` menggunakan `definition.Settings.ActionsPerTurn` (nilai aksi per giliran) dalam ToConfigJson.
            ["actions_per_turn"] = definition.Settings.ActionsPerTurn,
            // Memperbarui `[”starting_cash”]` menggunakan `definition.Settings.StartingCash` (nilai starting uang tunai) dalam ToConfigJson.
            ["starting_cash"] = definition.Settings.StartingCash,
            // Memperbarui `[”player_ordering”]` menggunakan `definition.PlayerOrdering.OrderingCode` (nilai ordering kode) dalam ToConfigJson.
            ["player_ordering"] = definition.PlayerOrdering.OrderingCode,
            // Memperbarui `[”weekday_rules”]` menggunakan objek baru bertipe `JsonObject` dengan nilai awal sesuai konstruktornya dalam ToConfigJson.
            ["weekday_rules"] = new JsonObject
            // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ToConfigJson.
            {
                // Memperbarui `[”friday”]` menggunakan objek baru bertipe `JsonObject` dengan nilai awal sesuai konstruktornya dalam ToConfigJson.
                ["friday"] = new JsonObject
                // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ToConfigJson.
                {
                    // Memperbarui `[”feature”]` menggunakan `definition.PlayerOrdering.FridayFeature` (nilai friday feature) dalam ToConfigJson.
                    ["feature"] = definition.PlayerOrdering.FridayFeature,
                    // Memperbarui `[”enabled”]` menggunakan `definition.PlayerOrdering.FridayEnabled` (nilai friday enabled) dalam ToConfigJson.
                    ["enabled"] = definition.PlayerOrdering.FridayEnabled
                // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam ToConfigJson.
                },
                // Memperbarui `[”saturday”]` menggunakan objek baru bertipe `JsonObject` dengan nilai awal sesuai konstruktornya dalam ToConfigJson.
                ["saturday"] = new JsonObject
                // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ToConfigJson.
                {
                    // Memperbarui `[”feature”]` menggunakan `definition.PlayerOrdering.SaturdayFeature` (nilai saturday feature) dalam ToConfigJson.
                    ["feature"] = definition.PlayerOrdering.SaturdayFeature,
                    // Memperbarui `[”enabled”]` menggunakan `definition.PlayerOrdering.SaturdayEnabled` (nilai saturday enabled) dalam ToConfigJson.
                    ["enabled"] = definition.PlayerOrdering.SaturdayEnabled
                // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam ToConfigJson.
                },
                // Memperbarui `[”sunday”]` menggunakan objek baru bertipe `JsonObject` dengan nilai awal sesuai konstruktornya dalam ToConfigJson.
                ["sunday"] = new JsonObject
                // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ToConfigJson.
                {
                    // Memperbarui `[”feature”]` menggunakan `definition.PlayerOrdering.SundayFeature` (nilai sunday feature) dalam ToConfigJson.
                    ["feature"] = definition.PlayerOrdering.SundayFeature,
                    // Memperbarui `[”enabled”]` menggunakan `definition.PlayerOrdering.SundayEnabled` (nilai sunday enabled) dalam ToConfigJson.
                    ["enabled"] = definition.PlayerOrdering.SundayEnabled
                // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam ToConfigJson.
                }
            // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam ToConfigJson.
            },
            // Memperbarui `[”constraints”]` menggunakan objek baru bertipe `JsonObject` dengan nilai awal sesuai konstruktornya dalam ToConfigJson.
            ["constraints"] = new JsonObject
            // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ToConfigJson.
            {
                // Memperbarui `[”cash_min”]` menggunakan `definition.Settings.CashMin` (nilai uang tunai minimum) dalam ToConfigJson.
                ["cash_min"] = definition.Settings.CashMin,
                // Memperbarui `[”max_ingredient_total”]` menggunakan `definition.Settings.MaxIngredientTotal` (nilai maksimum bahan total) dalam ToConfigJson.
                ["max_ingredient_total"] = definition.Settings.MaxIngredientTotal,
                // Memperbarui `[”max_same_ingredient”]` menggunakan `definition.Settings.MaxSameIngredient` (nilai maksimum same bahan) dalam ToConfigJson.
                ["max_same_ingredient"] = definition.Settings.MaxSameIngredient,
                // Memperbarui `[”primary_need_max_per_day”]` menggunakan `definition.Settings.PrimaryNeedMaxPerDay` (nilai primary kebutuhan maksimum per hari)
                // dalam ToConfigJson.
                ["primary_need_max_per_day"] = definition.Settings.PrimaryNeedMaxPerDay,
                // Memperbarui `[”require_primary_before_others”]` menggunakan `definition.Settings.RequirePrimaryBeforeOthers` (nilai require primary before
                // others) dalam ToConfigJson.
                ["require_primary_before_others"] = definition.Settings.RequirePrimaryBeforeOthers
            // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam ToConfigJson.
            },
            // Memperbarui `[”donation”]` menggunakan objek baru bertipe `JsonObject` dengan nilai awal sesuai konstruktornya dalam ToConfigJson.
            ["donation"] = new JsonObject
            // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ToConfigJson.
            {
                // Memperbarui `[”min_amount”]` menggunakan `definition.Settings.DonationMinAmount` (nilai donasi minimum nominal) dalam ToConfigJson.
                ["min_amount"] = definition.Settings.DonationMinAmount,
                // Memperbarui `[”max_amount”]` menggunakan `definition.Settings.DonationMaxAmount` (nilai donasi maksimum nominal) dalam ToConfigJson.
                ["max_amount"] = definition.Settings.DonationMaxAmount
            // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam ToConfigJson.
            },
            // Memperbarui `[”gold_trade”]` menggunakan objek baru bertipe `JsonObject` dengan nilai awal sesuai konstruktornya dalam ToConfigJson.
            ["gold_trade"] = new JsonObject
            // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ToConfigJson.
            {
                // Memperbarui `[”allow_buy”]` menggunakan `definition.Settings.GoldTradeAllowBuy` (nilai emas trade allow buy) dalam ToConfigJson.
                ["allow_buy"] = definition.Settings.GoldTradeAllowBuy,
                // Memperbarui `[”allow_sell”]` menggunakan `definition.Settings.GoldTradeAllowSell` (nilai emas trade allow sell) dalam ToConfigJson.
                ["allow_sell"] = definition.Settings.GoldTradeAllowSell
            // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam ToConfigJson.
            },
            // Memperbarui `[”advanced”]` menggunakan objek baru bertipe `JsonObject` dengan nilai awal sesuai konstruktornya dalam ToConfigJson.
            ["advanced"] = new JsonObject
            // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ToConfigJson.
            {
                // Memperbarui `[”loan”]` menggunakan objek baru bertipe `JsonObject` dengan nilai awal sesuai konstruktornya dalam ToConfigJson.
                ["loan"] = new JsonObject { ["enabled"] = definition.Settings.LoanEnabled },
                // Memperbarui `[”insurance”]` menggunakan objek baru bertipe `JsonObject` dengan nilai awal sesuai konstruktornya dalam ToConfigJson.
                ["insurance"] = new JsonObject { ["enabled"] = definition.Settings.InsuranceEnabled },
                // Memperbarui `[”saving_goal”]` menggunakan objek baru bertipe `JsonObject` dengan nilai awal sesuai konstruktornya dalam ToConfigJson.
                ["saving_goal"] = new JsonObject { ["enabled"] = definition.Settings.SavingGoalEnabled }
            // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam ToConfigJson.
            },
            // Memperbarui `[”freelance”]` menggunakan objek baru bertipe `JsonObject` dengan nilai awal sesuai konstruktornya dalam ToConfigJson.
            ["freelance"] = new JsonObject
            // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ToConfigJson.
            {
                // Memperbarui `[”income”]` menggunakan `definition.Settings.FreelanceIncome` (nilai freelance pemasukan) dalam ToConfigJson.
                ["income"] = definition.Settings.FreelanceIncome
            // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam ToConfigJson.
            },
            // Memperbarui `[”scoring”]` menggunakan objek baru bertipe `JsonObject` dengan nilai awal sesuai konstruktornya dalam ToConfigJson.
            ["scoring"] = new JsonObject
            // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ToConfigJson.
            {
                // Memperbarui `[”donation_rank_points”]` menggunakan objek baru bertipe `JsonArray` dengan argumen (definition.DonationRankPoints .Select(item =>
                // (JsonNode)new JsonObject { [”rank”] = item.Rank, [”points”] = item.Points }).ToArray()) dalam ToConfigJson.
                ["donation_rank_points"] = new JsonArray(definition.DonationRankPoints
                    // Meneruskan fungsi lambda `item => (JsonNode)new JsonObject { [”rank”] = item.Rank, [”points”] = item.Points }` yang dijalankan oleh operasi
                    // pemanggil untuk memproses setiap masukan sebagai argumen ke `definition.DonationRankPoints .Select`.
                    .Select(item => (JsonNode)new JsonObject
                    // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ToConfigJson.
                    {
                        // Memperbarui `[”rank”]` menggunakan `item.Rank` (nilai rank) dalam ToConfigJson.
                        ["rank"] = item.Rank,
                        // Memperbarui `[”points”]` menggunakan `item.Points` (nilai poin) dalam ToConfigJson.
                        ["points"] = item.Points
                    // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam ToConfigJson.
                    }).ToArray()),
                // Memperbarui `[”gold_points_by_qty”]` menggunakan objek baru bertipe `JsonArray` dengan argumen (definition.GoldPointsByQty .Select(item =>
                // (JsonNode)new JsonObject { [”qty”] = item.Qty, [”points”] = item.Points }).ToArray()) dalam ToConfigJson.
                ["gold_points_by_qty"] = new JsonArray(definition.GoldPointsByQty
                    // Meneruskan fungsi lambda `item => (JsonNode)new JsonObject { [”qty”] = item.Qty, [”points”] = item.Points }` yang dijalankan oleh operasi
                    // pemanggil untuk memproses setiap masukan sebagai argumen ke `definition.GoldPointsByQty .Select`.
                    .Select(item => (JsonNode)new JsonObject
                    // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ToConfigJson.
                    {
                        // Memperbarui `[”qty”]` menggunakan `item.Qty` (nilai qty) dalam ToConfigJson.
                        ["qty"] = item.Qty,
                        // Memperbarui `[”points”]` menggunakan `item.Points` (nilai poin) dalam ToConfigJson.
                        ["points"] = item.Points
                    // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam ToConfigJson.
                    }).ToArray()),
                // Memperbarui `[”pension_rank_points”]` menggunakan objek baru bertipe `JsonArray` dengan argumen (definition.PensionRankPoints .Select(item =>
                // (JsonNode)new JsonObject { [”rank”] = item.Rank, [”points”] = item.Points }).ToArray()) dalam ToConfigJson.
                ["pension_rank_points"] = new JsonArray(definition.PensionRankPoints
                    // Meneruskan fungsi lambda `item => (JsonNode)new JsonObject { [”rank”] = item.Rank, [”points”] = item.Points }` yang dijalankan oleh operasi
                    // pemanggil untuk memproses setiap masukan sebagai argumen ke `definition.PensionRankPoints .Select`.
                    .Select(item => (JsonNode)new JsonObject
                    // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ToConfigJson.
                    {
                        // Memperbarui `[”rank”]` menggunakan `item.Rank` (nilai rank) dalam ToConfigJson.
                        ["rank"] = item.Rank,
                        // Memperbarui `[”points”]` menggunakan `item.Points` (nilai poin) dalam ToConfigJson.
                        ["points"] = item.Points
                    // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam ToConfigJson.
                    }).ToArray())
            // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam ToConfigJson.
            },
            // Memperbarui `[”need_set_bonuses”]` menggunakan objek baru bertipe `JsonArray` dengan argumen (definition.NeedSetBonuses.Select(item =>
            // (JsonNode)new JsonObject { [”pattern_code”] = item.PatternCode, [”required_count”] = item.RequiredCount, [”points”] = ... dalam ToConfigJson.
            ["need_set_bonuses"] = new JsonArray(definition.NeedSetBonuses.Select(item => (JsonNode)new JsonObject
            // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ToConfigJson.
            {
                // Memperbarui `[”pattern_code”]` menggunakan `item.PatternCode` (nilai pattern kode) dalam ToConfigJson.
                ["pattern_code"] = item.PatternCode,
                // Memperbarui `[”required_count”]` menggunakan `item.RequiredCount` (nilai required jumlah) dalam ToConfigJson.
                ["required_count"] = item.RequiredCount,
                // Memperbarui `[”points”]` menggunakan `item.Points` (nilai poin) dalam ToConfigJson.
                ["points"] = item.Points
            // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam ToConfigJson.
            }).ToArray()),
            // Memperbarui `[”gold_prices”]` menggunakan objek baru bertipe `JsonArray` dengan argumen (definition.GoldPrices.Select(item => (JsonNode)new
            // JsonObject { [”price_code”] = item.PriceCode, [”qty”] = item.Qty, [”unit_price”] = item.UnitPrice, [”card_qt... dalam ToConfigJson.
            ["gold_prices"] = new JsonArray(definition.GoldPrices.Select(item => (JsonNode)new JsonObject
            // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ToConfigJson.
            {
                // Memperbarui `[”price_code”]` menggunakan `item.PriceCode` (nilai harga kode) dalam ToConfigJson.
                ["price_code"] = item.PriceCode,
                // Memperbarui `[”qty”]` menggunakan `item.Qty` (nilai qty) dalam ToConfigJson.
                ["qty"] = item.Qty,
                // Memperbarui `[”unit_price”]` menggunakan `item.UnitPrice` (nilai unit harga) dalam ToConfigJson.
                ["unit_price"] = item.UnitPrice,
                // Memperbarui `[”card_qty”]` menggunakan `item.CardQty` (nilai kartu qty) dalam ToConfigJson.
                ["card_qty"] = item.CardQty
            // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam ToConfigJson.
            }).ToArray()),
            // Memperbarui `[”tie_breakers”]` menggunakan objek baru bertipe `JsonArray` dengan argumen (definition.TieBreakers.Select(item => (JsonNode)new
            // JsonObject { [”tie_breaker_code”] = item.TieBreakerCode, [”tie_number”] = item.TieNumber, [”card_qty”] = it... dalam ToConfigJson.
            ["tie_breakers"] = new JsonArray(definition.TieBreakers.Select(item => (JsonNode)new JsonObject
            // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ToConfigJson.
            {
                // Memperbarui `[”tie_breaker_code”]` menggunakan `item.TieBreakerCode` (kode kartu penentu urutan saat nilai pemain sama) dalam ToConfigJson.
                ["tie_breaker_code"] = item.TieBreakerCode,
                // Memperbarui `[”tie_number”]` menggunakan `item.TieNumber` (nilai tie number) dalam ToConfigJson.
                ["tie_number"] = item.TieNumber,
                // Memperbarui `[”card_qty”]` menggunakan `item.CardQty` (nilai kartu qty) dalam ToConfigJson.
                ["card_qty"] = item.CardQty
            // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam ToConfigJson.
            }).ToArray()),
            // Memperbarui `[”sharia_loans”]` menggunakan objek baru bertipe `JsonArray` dengan argumen (definition.ShariaLoans.Select(item => (JsonNode)new
            // JsonObject { [”loan_code”] = item.LoanCode, [”item_name”] = item.ItemName, [”principal”] = item.Principal, ... dalam ToConfigJson.
            ["sharia_loans"] = new JsonArray(definition.ShariaLoans.Select(item => (JsonNode)new JsonObject
            // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ToConfigJson.
            {
                // Memperbarui `[”loan_code”]` menggunakan `item.LoanCode` (kode produk pinjaman syariah) dalam ToConfigJson.
                ["loan_code"] = item.LoanCode,
                // Memperbarui `[”item_name”]` menggunakan `item.ItemName` (nilai elemen nama) dalam ToConfigJson.
                ["item_name"] = item.ItemName,
                // Memperbarui `[”principal”]` menggunakan `item.Principal` (nilai principal) dalam ToConfigJson.
                ["principal"] = item.Principal,
                // Memperbarui `[”repayment_amount”]` menggunakan `item.RepaymentAmount` (nilai repayment nominal) dalam ToConfigJson.
                ["repayment_amount"] = item.RepaymentAmount,
                // Memperbarui `[”duration_days”]` menggunakan `item.DurationDays` (nilai duration hari) dalam ToConfigJson.
                ["duration_days"] = item.DurationDays,
                // Memperbarui `[”penalty_points”]` menggunakan `item.PenaltyPoints` (nilai penalti poin) dalam ToConfigJson.
                ["penalty_points"] = item.PenaltyPoints,
                // Memperbarui `[”card_qty”]` menggunakan `item.CardQty` (nilai kartu qty) dalam ToConfigJson.
                ["card_qty"] = item.CardQty
            // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam ToConfigJson.
            }).ToArray()),
            // Memperbarui `[”insurance_products”]` menggunakan objek baru bertipe `JsonArray` dengan argumen (definition.InsuranceProducts.Select(item =>
            // (JsonNode)new JsonObject { [”product_code”] = item.ProductCode, [”item_name”] = item.ItemName, [”premium”] = item.P... dalam ToConfigJson.
            ["insurance_products"] = new JsonArray(definition.InsuranceProducts.Select(item => (JsonNode)new JsonObject
            // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ToConfigJson.
            {
                // Memperbarui `[”product_code”]` menggunakan `item.ProductCode` (nilai product kode) dalam ToConfigJson.
                ["product_code"] = item.ProductCode,
                // Memperbarui `[”item_name”]` menggunakan `item.ItemName` (nilai elemen nama) dalam ToConfigJson.
                ["item_name"] = item.ItemName,
                // Memperbarui `[”premium”]` menggunakan `item.Premium` (nilai premium) dalam ToConfigJson.
                ["premium"] = item.Premium,
                // Memperbarui `[”usage_limit”]` menggunakan `item.UsageLimit` (nilai usage limit) dalam ToConfigJson.
                ["usage_limit"] = item.UsageLimit,
                // Memperbarui `[”card_qty”]` menggunakan `item.CardQty` (nilai kartu qty) dalam ToConfigJson.
                ["card_qty"] = item.CardQty
            // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam ToConfigJson.
            }).ToArray()),
            // Memperbarui `[”life_risks”]` menggunakan objek baru bertipe `JsonArray` dengan argumen (definition.LifeRisks.Select(item => (JsonNode)new
            // JsonObject { [”risk_code”] = item.RiskCode, [”item_name”] = item.ItemName, [”effect_type”] = item.EffectType,... dalam ToConfigJson.
            ["life_risks"] = new JsonArray(definition.LifeRisks.Select(item => (JsonNode)new JsonObject
            // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ToConfigJson.
            {
                // Memperbarui `[”risk_code”]` menggunakan `item.RiskCode` (nilai risiko kode) dalam ToConfigJson.
                ["risk_code"] = item.RiskCode,
                // Memperbarui `[”item_name”]` menggunakan `item.ItemName` (nilai elemen nama) dalam ToConfigJson.
                ["item_name"] = item.ItemName,
                // Memperbarui `[”effect_type”]` menggunakan `item.EffectType` (nilai effect jenis) dalam ToConfigJson.
                ["effect_type"] = item.EffectType,
                // Memperbarui `[”direction”]` menggunakan `item.Direction` (nilai direction) dalam ToConfigJson.
                ["direction"] = item.Direction,
                // Memperbarui `[”amount”]` menggunakan `item.Amount` (nominal uang atau nilai transaksi yang dipakai dalam operasi) dalam ToConfigJson.
                ["amount"] = item.Amount,
                // Memperbarui `[”target_scope”]` menggunakan `item.TargetScope` (nilai target cakupan) dalam ToConfigJson.
                ["target_scope"] = item.TargetScope,
                // Memperbarui `[”value_delta”]` menggunakan `item.ValueDelta` (nilai nilai delta) dalam ToConfigJson.
                ["value_delta"] = item.ValueDelta,
                // Memperbarui `[”duration_days”]` menggunakan `item.DurationDays` (nilai duration hari) dalam ToConfigJson.
                ["duration_days"] = item.DurationDays,
                // Memperbarui `[”card_qty”]` menggunakan `item.CardQty` (nilai kartu qty) dalam ToConfigJson.
                ["card_qty"] = item.CardQty
            // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam ToConfigJson.
            }).ToArray()),
            // Memperbarui `[”component_catalog”]` menggunakan objek baru bertipe `JsonObject` dengan nilai awal sesuai konstruktornya dalam ToConfigJson.
            ["component_catalog"] = new JsonObject
            // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ToConfigJson.
            {
                // Memperbarui `[”gameConfig”]` menggunakan objek baru bertipe `JsonObject` dengan nilai awal sesuai konstruktornya dalam ToConfigJson.
                ["gameConfig"] = new JsonObject
                // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ToConfigJson.
                {
                    // Memperbarui `[”initialCoins”]` menggunakan `definition.Settings.InitialCoins` (nilai awal coins) dalam ToConfigJson.
                    ["initialCoins"] = definition.Settings.InitialCoins,
                    // Memperbarui `[”initialHappiness”]` menggunakan `definition.Settings.InitialHappiness` (nilai awal kebahagiaan) dalam ToConfigJson.
                    ["initialHappiness"] = definition.Settings.InitialHappiness,
                    // Memperbarui `[”initialSaving”]` menggunakan `definition.Settings.InitialSaving` (nilai awal tabungan) dalam ToConfigJson.
                    ["initialSaving"] = definition.Settings.InitialSaving,
                    // Memperbarui `[”actionsPerTurn”]` menggunakan `definition.Settings.ActionsPerTurn` (nilai aksi per giliran) dalam ToConfigJson.
                    ["actionsPerTurn"] = definition.Settings.ActionsPerTurn,
                    // Memperbarui `[”finishDay”]` menggunakan `definition.Settings.FinishDay` (nilai finish hari) dalam ToConfigJson.
                    ["finishDay"] = definition.Settings.FinishDay,
                    // Memperbarui `[”minPlayers”]` menggunakan `definition.Settings.MinPlayers` (nilai minimum pemain) dalam ToConfigJson.
                    ["minPlayers"] = definition.Settings.MinPlayers,
                    // Memperbarui `[”maxPlayers”]` menggunakan `definition.Settings.MaxPlayers` (nilai maksimum pemain) dalam ToConfigJson.
                    ["maxPlayers"] = definition.Settings.MaxPlayers
                // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam ToConfigJson.
                },
                // Memperbarui `[”bahan”]` menggunakan objek baru bertipe `JsonArray` dengan argumen (definition.Ingredients.Select(item => (JsonNode)new JsonObject
                // { [”id”] = item.Id, [”nama”] = item.Nama, [”hargaBeli”] = item.HargaBeli, [”cardQty”] = item.Car... dalam ToConfigJson.
                ["bahan"] = new JsonArray(definition.Ingredients.Select(item => (JsonNode)new JsonObject
                // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ToConfigJson.
                {
                    // Memperbarui `[”id”]` menggunakan `item.Id` (nilai identitas) dalam ToConfigJson.
                    ["id"] = item.Id,
                    // Memperbarui `[”nama”]` menggunakan `item.Nama` (nilai nama) dalam ToConfigJson.
                    ["nama"] = item.Nama,
                    // Memperbarui `[”hargaBeli”]` menggunakan `item.HargaBeli` (nilai harga beli) dalam ToConfigJson.
                    ["hargaBeli"] = item.HargaBeli,
                    // Memperbarui `[”cardQty”]` menggunakan `item.CardQty` (nilai kartu qty) dalam ToConfigJson.
                    ["cardQty"] = item.CardQty
                // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam ToConfigJson.
                }).ToArray()),
                // Memperbarui `[”resep”]` menggunakan objek baru bertipe `JsonArray` dengan argumen (definition.Orders.Select(item => (JsonNode)new JsonObject {
                // [”id”] = item.Id, [”nama”] = item.Nama, [”hargaJual”] = item.HargaJual, [”poinKebahagiaan”] = item.... dalam ToConfigJson.
                ["resep"] = new JsonArray(definition.Orders.Select(item => (JsonNode)new JsonObject
                // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ToConfigJson.
                {
                    // Memperbarui `[”id”]` menggunakan `item.Id` (nilai identitas) dalam ToConfigJson.
                    ["id"] = item.Id,
                    // Memperbarui `[”nama”]` menggunakan `item.Nama` (nilai nama) dalam ToConfigJson.
                    ["nama"] = item.Nama,
                    // Memperbarui `[”hargaJual”]` menggunakan `item.HargaJual` (nilai harga jual) dalam ToConfigJson.
                    ["hargaJual"] = item.HargaJual,
                    // Memperbarui `[”poinKebahagiaan”]` menggunakan `item.PoinKebahagiaan` (nilai poin kebahagiaan) dalam ToConfigJson.
                    ["poinKebahagiaan"] = item.PoinKebahagiaan,
                    // Memperbarui `[”bahan”]` menggunakan objek baru bertipe `JsonArray` dengan argumen (item.Bahan.Select(name => (JsonNode)name).ToArray()) dalam
                    // ToConfigJson.
                    ["bahan"] = new JsonArray(item.Bahan.Select(name => (JsonNode)name).ToArray()),
                    // Memperbarui `[”cardQty”]` menggunakan `item.CardQty` (nilai kartu qty) dalam ToConfigJson.
                    ["cardQty"] = item.CardQty
                // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam ToConfigJson.
                }).ToArray()),
                // Memperbarui `[”kebutuhan”]` menggunakan objek baru bertipe `JsonArray` dengan argumen (definition.Needs.Select(item => (JsonNode)new JsonObject {
                // [”id”] = item.Id, [”nama”] = item.Nama, [”tipe”] = item.Tipe, [”hargaBeli”] = item.HargaBeli, [”poin... dalam ToConfigJson.
                ["kebutuhan"] = new JsonArray(definition.Needs.Select(item => (JsonNode)new JsonObject
                // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ToConfigJson.
                {
                    // Memperbarui `[”id”]` menggunakan `item.Id` (nilai identitas) dalam ToConfigJson.
                    ["id"] = item.Id,
                    // Memperbarui `[”nama”]` menggunakan `item.Nama` (nilai nama) dalam ToConfigJson.
                    ["nama"] = item.Nama,
                    // Memperbarui `[”tipe”]` menggunakan `item.Tipe` (nilai tipe) dalam ToConfigJson.
                    ["tipe"] = item.Tipe,
                    // Memperbarui `[”hargaBeli”]` menggunakan `item.HargaBeli` (nilai harga beli) dalam ToConfigJson.
                    ["hargaBeli"] = item.HargaBeli,
                    // Memperbarui `[”poinKebahagiaan”]` menggunakan `item.PoinKebahagiaan` (nilai poin kebahagiaan) dalam ToConfigJson.
                    ["poinKebahagiaan"] = item.PoinKebahagiaan,
                    // Memperbarui `[”cardQty”]` menggunakan `item.CardQty` (nilai kartu qty) dalam ToConfigJson.
                    ["cardQty"] = item.CardQty
                // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam ToConfigJson.
                }).ToArray()),
                // Memperbarui `[”targetKebutuhan”]` menggunakan objek baru bertipe `JsonArray` dengan argumen (definition.CollectionMissions.Select(item =>
                // (JsonNode)new JsonObject { [”id”] = item.Id, [”nama”] = item.Nama, [”success_points”] = item.SuccessPoints, [”fail... dalam ToConfigJson.
                ["targetKebutuhan"] = new JsonArray(definition.CollectionMissions.Select(item => (JsonNode)new JsonObject
                // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ToConfigJson.
                {
                    // Memperbarui `[”id”]` menggunakan `item.Id` (nilai identitas) dalam ToConfigJson.
                    ["id"] = item.Id,
                    // Memperbarui `[”nama”]` menggunakan `item.Nama` (nilai nama) dalam ToConfigJson.
                    ["nama"] = item.Nama,
                    // Memperbarui `[”success_points”]` menggunakan `item.SuccessPoints` (nilai success poin) dalam ToConfigJson.
                    ["success_points"] = item.SuccessPoints,
                    // Memperbarui `[”failure_points”]` menggunakan `item.FailurePoints` (nilai failure poin) dalam ToConfigJson.
                    ["failure_points"] = item.FailurePoints,
                    // Memperbarui `[”penaltyPoints”]` menggunakan `item.PenaltyPoints` (nilai penalti poin) dalam ToConfigJson.
                    ["penaltyPoints"] = item.PenaltyPoints,
                    // Memperbarui `[”kebutuhanTarget”]` menggunakan objek baru bertipe `JsonArray` dengan argumen (item.KebutuhanTarget.Select(requirement =>
                    // (JsonNode)new JsonObject { [”order”] = requirement.Order, [”type”] = requirement.Type, [”value”] = requirement.Value... dalam ToConfigJson.
                    ["kebutuhanTarget"] = new JsonArray(item.KebutuhanTarget.Select(requirement => (JsonNode)new JsonObject
                    // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ToConfigJson.
                    {
                        // Memperbarui `[”order”]` menggunakan `requirement.Order` (nilai urutan/pesanan) dalam ToConfigJson.
                        ["order"] = requirement.Order,
                        // Memperbarui `[”type”]` menggunakan `requirement.Type` (nilai jenis) dalam ToConfigJson.
                        ["type"] = requirement.Type,
                        // Memperbarui `[”value”]` menggunakan `requirement.Value`, yaitu nilai yang dibungkus objek/nullable dalam ToConfigJson.
                        ["value"] = requirement.Value
                    // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam ToConfigJson.
                    }).ToArray())
                // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam ToConfigJson.
                }).ToArray()),
                // Memperbarui `[”tujuanFinansial”]` menggunakan objek baru bertipe `JsonArray` dengan argumen (definition.FinancialGoals.Select(item =>
                // (JsonNode)new JsonObject { [”id”] = item.Id, [”nama”] = item.Nama, [”hargaBeli”] = item.HargaBeli, [”poinKebahagiaan”]... dalam ToConfigJson.
                ["tujuanFinansial"] = new JsonArray(definition.FinancialGoals.Select(item => (JsonNode)new JsonObject
                // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ToConfigJson.
                {
                    // Memperbarui `[”id”]` menggunakan `item.Id` (nilai identitas) dalam ToConfigJson.
                    ["id"] = item.Id,
                    // Memperbarui `[”nama”]` menggunakan `item.Nama` (nilai nama) dalam ToConfigJson.
                    ["nama"] = item.Nama,
                    // Memperbarui `[”hargaBeli”]` menggunakan `item.HargaBeli` (nilai harga beli) dalam ToConfigJson.
                    ["hargaBeli"] = item.HargaBeli,
                    // Memperbarui `[”poinKebahagiaan”]` menggunakan `item.PoinKebahagiaan` (nilai poin kebahagiaan) dalam ToConfigJson.
                    ["poinKebahagiaan"] = item.PoinKebahagiaan,
                    // Memperbarui `[”cardQty”]` menggunakan `item.CardQty` (nilai kartu qty) dalam ToConfigJson.
                    ["cardQty"] = item.CardQty
                // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam ToConfigJson.
                }).ToArray()),
                // Memperbarui `[”narasi”]` menggunakan objek baru bertipe `JsonArray` dengan argumen (definition.Narratives.Select(item => (JsonNode)new JsonObject
                // { [”id”] = item.Id, [”nama”] = item.Nama, [”teks”] = new JsonArray(item.Teks.Select(text => (Json... dalam ToConfigJson.
                ["narasi"] = new JsonArray(definition.Narratives.Select(item => (JsonNode)new JsonObject
                // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ToConfigJson.
                {
                    // Memperbarui `[”id”]` menggunakan `item.Id` (nilai identitas) dalam ToConfigJson.
                    ["id"] = item.Id,
                    // Memperbarui `[”nama”]` menggunakan `item.Nama` (nilai nama) dalam ToConfigJson.
                    ["nama"] = item.Nama,
                    // Memperbarui `[”teks”]` menggunakan objek baru bertipe `JsonArray` dengan argumen (item.Teks.Select(text => (JsonNode)text).ToArray()) dalam
                    // ToConfigJson.
                    ["teks"] = new JsonArray(item.Teks.Select(text => (JsonNode)text).ToArray()),
                    // Memperbarui `[”prerequisiteAksi”]` menggunakan objek baru bertipe `JsonArray` dengan argumen (item.PrerequisiteAksi.Select(prereq =>
                    // (JsonNode)new JsonObject { [”aksi”] = prereq.Aksi, [”value”] = prereq.Value }).ToArray()) dalam ToConfigJson.
                    ["prerequisiteAksi"] = new JsonArray(item.PrerequisiteAksi.Select(prereq => (JsonNode)new JsonObject
                    // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ToConfigJson.
                    {
                        // Memperbarui `[”aksi”]` menggunakan `prereq.Aksi` (nilai aksi) dalam ToConfigJson.
                        ["aksi"] = prereq.Aksi,
                        // Memperbarui `[”value”]` menggunakan `prereq.Value`, yaitu nilai yang dibungkus objek/nullable dalam ToConfigJson.
                        ["value"] = prereq.Value
                    // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam ToConfigJson.
                    }).ToArray())
                // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam ToConfigJson.
                }).ToArray())
            // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam ToConfigJson.
            }
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam ToConfigJson.
        };


        // Mengembalikan memanggil `root.ToJsonString` dengan `new JsonSerializerOptions { WriteIndented = false }` kepada pemanggil dalam ToConfigJson;
        // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return root.ToJsonString(new JsonSerializerOptions
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ToConfigJson.
        {
            // Memperbarui `WriteIndented` menggunakan false, yaitu kondisi nonaktif/tidak terpenuhi dalam ToConfigJson.
            WriteIndented = false
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam ToConfigJson.
        });
    // Menutup scope metode ToConfigJson; bagian berikut berada di luar batas blok tersebut dalam ToConfigJson.
    }

    // Mendefinisikan metode `ToConfigElement` dengan hasil bertipe `JsonElement?`; operasi ini menangani ke konfigurasi element. Masukan: Parameter
    // `definition` bertipe `RulesetDefinitionDto?` membawa definisi terstruktur komponen serta parameter aturan permainan; nilai null diizinkan ketika
    // data opsional belum tersedia.
    internal static JsonElement? ToConfigElement(RulesetDefinitionDto? definition)
    // Membuka scope metode ToConfigElement; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ToConfigElement.
    {
        // Memeriksa hasil pencocokan `definition` dengan pola `null`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ToConfigElement.
        if (definition is null)
        // Membuka scope cabang if untuk kondisi `definition is null`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ToConfigElement.
        {
            // Mengembalikan null, yaitu penanda tidak ada nilai kepada pemanggil dalam ToConfigElement; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return null;
        // Menutup scope cabang if untuk kondisi `definition is null`; bagian berikut berada di luar batas blok tersebut dalam ToConfigElement.
        }

        // Memulai blok try dalam ToConfigElement; exception dari blok ini dapat dialihkan ke catch, sedangkan finally (jika ada) tetap dijalankan saat
        // keluar.
        try
        // Membuka scope penanganan operasi try; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ToConfigElement.
        {
            // Menyiapkan variabel lokal `document` untuk nilai document dengan memanggil `JsonDocument.Parse` dengan `ToConfigJson(definition)`. Tipe variabel
            // disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
            using var document = JsonDocument.Parse(ToConfigJson(definition));
            // Mengembalikan membuat salinan `document.RootElement` agar hasil dapat digunakan terpisah dari objek sumber kepada pemanggil dalam
            // ToConfigElement; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return document.RootElement.Clone();
        // Menutup scope penanganan operasi try; bagian berikut berada di luar batas blok tersebut dalam ToConfigElement.
        }
        // Menangani exception `JsonException` melalui variabel dalam ToConfigElement.
        catch (JsonException)
        // Membuka scope penanganan exception catch; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ToConfigElement.
        {
            // Mengembalikan null, yaitu penanda tidak ada nilai kepada pemanggil dalam ToConfigElement; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return null;
        // Menutup scope penanganan exception catch; bagian berikut berada di luar batas blok tersebut dalam ToConfigElement.
        }
    // Menutup scope metode ToConfigElement; bagian berikut berada di luar batas blok tersebut dalam ToConfigElement.
    }

    // Mendefinisikan metode `ReadIngredients` dengan hasil bertipe `List<RulesetIngredientDto>`; operasi ini menangani read bahan. Masukan: Parameter
    // `componentCatalog` bertipe `JsonElement` membawa nilai komponen catalog.
    private static List<RulesetIngredientDto> ReadIngredients(JsonElement componentCatalog)
    // Membuka scope metode ReadIngredients; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ReadIngredients.
    {
        // Mengembalikan memanggil `ReadArray` dengan `componentCatalog`, `”bahan”`, `item => new RulesetIngredientDto { Id = ReadString(item, ”id”,
        // string.Empty), Nama = ReadString(item, ”nama”, string.Empty), HargaBeli = ReadInt(item, ”hargaBeli”, 0), CardQty...` kepada pemanggil dalam
        // ReadIngredients; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return ReadArray(componentCatalog, "bahan", item => new RulesetIngredientDto
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ReadIngredients.
        {
            // Memperbarui `Id` menggunakan memanggil `ReadString` dengan `item`, `”id”`, `string.Empty` dalam ReadIngredients.
            Id = ReadString(item, "id", string.Empty),
            // Memperbarui `Nama` menggunakan memanggil `ReadString` dengan `item`, `”nama”`, `string.Empty` dalam ReadIngredients.
            Nama = ReadString(item, "nama", string.Empty),
            // Memperbarui `HargaBeli` menggunakan memanggil `ReadInt` dengan `item`, `”hargaBeli”`, `0` dalam ReadIngredients.
            HargaBeli = ReadInt(item, "hargaBeli", 0),
            // Memperbarui `CardQty` menggunakan memanggil `ReadNullableInt` dengan `item`, `”cardQty”` dalam ReadIngredients.
            CardQty = ReadNullableInt(item, "cardQty")
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam ReadIngredients.
        });
    // Menutup scope metode ReadIngredients; bagian berikut berada di luar batas blok tersebut dalam ReadIngredients.
    }

    // Mendefinisikan metode `ReadOrders` dengan hasil bertipe `List<RulesetOrderDto>`; operasi ini menangani read pesanan. Masukan: Parameter
    // `componentCatalog` bertipe `JsonElement` membawa nilai komponen catalog.
    private static List<RulesetOrderDto> ReadOrders(JsonElement componentCatalog)
    // Membuka scope metode ReadOrders; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ReadOrders.
    {
        // Mengembalikan memanggil `ReadArray` dengan `componentCatalog`, `”resep”`, `item => new RulesetOrderDto { Id = ReadString(item, ”id”,
        // string.Empty), Nama = ReadString(item, ”nama”, string.Empty), HargaJual = ReadInt(item, ”hargaJual”, 0), PoinKebahagi...` kepada pemanggil dalam
        // ReadOrders; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return ReadArray(componentCatalog, "resep", item => new RulesetOrderDto
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ReadOrders.
        {
            // Memperbarui `Id` menggunakan memanggil `ReadString` dengan `item`, `”id”`, `string.Empty` dalam ReadOrders.
            Id = ReadString(item, "id", string.Empty),
            // Memperbarui `Nama` menggunakan memanggil `ReadString` dengan `item`, `”nama”`, `string.Empty` dalam ReadOrders.
            Nama = ReadString(item, "nama", string.Empty),
            // Memperbarui `HargaJual` menggunakan memanggil `ReadInt` dengan `item`, `”hargaJual”`, `0` dalam ReadOrders.
            HargaJual = ReadInt(item, "hargaJual", 0),
            // Memperbarui `PoinKebahagiaan` menggunakan memanggil `ReadInt` dengan `item`, `”poinKebahagiaan”`, `0` dalam ReadOrders.
            PoinKebahagiaan = ReadInt(item, "poinKebahagiaan", 0),
            // Memperbarui `Bahan` menggunakan hasil pemilihan bersyarat: ketika `item.TryGetProperty(”bahan”, out var bahan) && bahan.ValueKind ==
            // JsonValueKind.Array` benar gunakan `bahan.EnumerateArray() .Where(x => x.ValueKind == JsonValueKind.String) .Select(x => x.GetString() ??
            // string.Empty) .Where(x => !string.IsNullOrWhiteSpace(x)) .ToList()`, jika tidak gunakan `[]` dalam ReadOrders.
            Bahan = item.TryGetProperty("bahan", out var bahan) && bahan.ValueKind == JsonValueKind.Array
                // Meneruskan fungsi lambda `item => new RulesetOrderDto { Id = ReadString(item, ”id”, string.Empty), Nama = ReadString(item, ”nama”, string.Empty),
                // HargaJual = ReadInt(item, ”hargaJual”, 0), PoinKebahagi...` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen
                // ke `ReadArray`.
                ? bahan.EnumerateArray()
                    // Meneruskan fungsi lambda `x => x.ValueKind == JsonValueKind.String` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai
                    // argumen ke `bahan.EnumerateArray() .Where`.
                    .Where(x => x.ValueKind == JsonValueKind.String)
                    // Meneruskan fungsi lambda `x => x.GetString() ?? string.Empty` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai
                    // argumen ke `bahan.EnumerateArray() .Where(x => x.ValueKind == JsonValueKind.String) .Select`.
                    .Select(x => x.GetString() ?? string.Empty)
                    // Meneruskan fungsi lambda `x => !string.IsNullOrWhiteSpace(x)` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai
                    // argumen ke `bahan.EnumerateArray() .Where(x => x.ValueKind == JsonValueKind.String) .Select(x => x.GetString() ?? string.Empty) .Where`;
                    // Meneruskan `x` (nilai x) sebagai argumen ke `string.IsNullOrWhiteSpace`.
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    // Meneruskan fungsi lambda `item => new RulesetOrderDto { Id = ReadString(item, ”id”, string.Empty), Nama = ReadString(item, ”nama”, string.Empty),
                    // HargaJual = ReadInt(item, ”hargaJual”, 0), PoinKebahagi...` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen
                    // ke `ReadArray`.
                    .ToList()
                // Meneruskan fungsi lambda `item => new RulesetOrderDto { Id = ReadString(item, ”id”, string.Empty), Nama = ReadString(item, ”nama”, string.Empty),
                // HargaJual = ReadInt(item, ”hargaJual”, 0), PoinKebahagi...` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen
                // ke `ReadArray`.
                : [],
            // Memperbarui `CardQty` menggunakan hasil pemilihan bersyarat: ketika `item.TryGetProperty(”cardQty”, out var cq) && cq.ValueKind ==
            // JsonValueKind.Number` benar gunakan `cq.GetInt32()`, jika tidak gunakan `(int?)null` dalam ReadOrders.
            CardQty = item.TryGetProperty("cardQty", out var cq) && cq.ValueKind == JsonValueKind.Number ? cq.GetInt32() : (int?)null
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam ReadOrders.
        });
    // Menutup scope metode ReadOrders; bagian berikut berada di luar batas blok tersebut dalam ReadOrders.
    }

    // Mendefinisikan metode `ReadNeeds` dengan hasil bertipe `List<RulesetNeedDto>`; operasi ini menangani read kebutuhan. Masukan: Parameter
    // `componentCatalog` bertipe `JsonElement` membawa nilai komponen catalog.
    private static List<RulesetNeedDto> ReadNeeds(JsonElement componentCatalog)
    // Membuka scope metode ReadNeeds; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ReadNeeds.
    {
        // Mengembalikan memanggil `ReadArray` dengan `componentCatalog`, `”kebutuhan”`, `item => new RulesetNeedDto { Id = ReadString(item, ”id”,
        // string.Empty), Nama = ReadString(item, ”nama”, string.Empty), Tipe = ReadString(item, ”tipe”, string.Empty), HargaBeli...` kepada pemanggil dalam
        // ReadNeeds; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return ReadArray(componentCatalog, "kebutuhan", item => new RulesetNeedDto
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ReadNeeds.
        {
            // Memperbarui `Id` menggunakan memanggil `ReadString` dengan `item`, `”id”`, `string.Empty` dalam ReadNeeds.
            Id = ReadString(item, "id", string.Empty),
            // Memperbarui `Nama` menggunakan memanggil `ReadString` dengan `item`, `”nama”`, `string.Empty` dalam ReadNeeds.
            Nama = ReadString(item, "nama", string.Empty),
            // Memperbarui `Tipe` menggunakan memanggil `ReadString` dengan `item`, `”tipe”`, `string.Empty` dalam ReadNeeds.
            Tipe = ReadString(item, "tipe", string.Empty),
            // Memperbarui `HargaBeli` menggunakan memanggil `ReadInt` dengan `item`, `”hargaBeli”`, `0` dalam ReadNeeds.
            HargaBeli = ReadInt(item, "hargaBeli", 0),
            // Memperbarui `PoinKebahagiaan` menggunakan memanggil `ReadInt` dengan `item`, `”poinKebahagiaan”`, `0` dalam ReadNeeds.
            PoinKebahagiaan = ReadInt(item, "poinKebahagiaan", 0),
            // Memperbarui `CardQty` menggunakan memanggil `ReadNullableInt` dengan `item`, `”cardQty”` dalam ReadNeeds.
            CardQty = ReadNullableInt(item, "cardQty")
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam ReadNeeds.
        });
    // Menutup scope metode ReadNeeds; bagian berikut berada di luar batas blok tersebut dalam ReadNeeds.
    }

    // Mendefinisikan metode `ReadCollectionMissions` dengan hasil bertipe `List<RulesetCollectionMissionDto>`; operasi ini menangani read collection
    // misi. Masukan: Parameter `componentCatalog` bertipe `JsonElement` membawa nilai komponen catalog.
    private static List<RulesetCollectionMissionDto> ReadCollectionMissions(JsonElement componentCatalog)
    // Membuka scope metode ReadCollectionMissions; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ReadCollectionMissions.
    {
        // Mengembalikan memanggil `ReadArray` dengan `componentCatalog`, `”targetKebutuhan”`, `item => new RulesetCollectionMissionDto { Id =
        // ReadString(item, ”id”, string.Empty), Nama = ReadString(item, ”nama”, string.Empty), SuccessPoints = ReadInt(item, ”success_poin...` kepada
        // pemanggil dalam ReadCollectionMissions; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return ReadArray(componentCatalog, "targetKebutuhan", item => new RulesetCollectionMissionDto
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ReadCollectionMissions.
        {
            // Memperbarui `Id` menggunakan memanggil `ReadString` dengan `item`, `”id”`, `string.Empty` dalam ReadCollectionMissions.
            Id = ReadString(item, "id", string.Empty),
            // Memperbarui `Nama` menggunakan memanggil `ReadString` dengan `item`, `”nama”`, `string.Empty` dalam ReadCollectionMissions.
            Nama = ReadString(item, "nama", string.Empty),
            // Memperbarui `SuccessPoints` menggunakan memanggil `ReadInt` dengan `item`, `”success_points”`, `0` dalam ReadCollectionMissions.
            SuccessPoints = ReadInt(item, "success_points", 0),
            // Memperbarui `FailurePoints` menggunakan memanggil `ReadInt` dengan `item`, `”failure_points”`, `0` dalam ReadCollectionMissions.
            FailurePoints = ReadInt(item, "failure_points", 0),
            // Memperbarui `PenaltyPoints` menggunakan memanggil `ReadInt` dengan `item`, `”penaltyPoints”`, `0` dalam ReadCollectionMissions.
            PenaltyPoints = ReadInt(item, "penaltyPoints", 0),
            // Memperbarui `KebutuhanTarget` menggunakan hasil pemilihan bersyarat: ketika `item.TryGetProperty(”kebutuhanTarget”, out var requirements) &&
            // requirements.ValueKind == JsonValueKind.Array` benar gunakan `requirements.EnumerateArray().Select(requirement => new
            // RulesetCollectionMissionRequirementDto { Order = ReadInt(requirement, ”order”, 0), Type = ReadString(requirement, ”type...`, jika tidak gunakan
            // `[]` dalam ReadCollectionMissions.
            KebutuhanTarget = item.TryGetProperty("kebutuhanTarget", out var requirements) && requirements.ValueKind == JsonValueKind.Array
                // Meneruskan fungsi lambda `requirement => new RulesetCollectionMissionRequirementDto { Order = ReadInt(requirement, ”order”, 0), Type =
                // ReadString(requirement, ”type”, string.Empty), Value = ReadString(...` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan
                // sebagai argumen ke `requirements.EnumerateArray().Select`.
                ? requirements.EnumerateArray().Select(requirement => new RulesetCollectionMissionRequirementDto
                // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // ReadCollectionMissions.
                {
                    // Memperbarui `Order` menggunakan memanggil `ReadInt` dengan `requirement`, `”order”`, `0` dalam ReadCollectionMissions.
                    Order = ReadInt(requirement, "order", 0),
                    // Memperbarui `Type` menggunakan memanggil `ReadString` dengan `requirement`, `”type”`, `string.Empty` dalam ReadCollectionMissions.
                    Type = ReadString(requirement, "type", string.Empty),
                    // Memperbarui `Value` menggunakan memanggil `ReadString` dengan `requirement`, `”value”`, `string.Empty` dalam ReadCollectionMissions.
                    Value = ReadString(requirement, "value", string.Empty)
                // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam ReadCollectionMissions.
                }).ToList()
                // Meneruskan fungsi lambda `item => new RulesetCollectionMissionDto { Id = ReadString(item, ”id”, string.Empty), Nama = ReadString(item, ”nama”,
                // string.Empty), SuccessPoints = ReadInt(item, ”success_poin...` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai
                // argumen ke `ReadArray`.
                : []
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam ReadCollectionMissions.
        });
    // Menutup scope metode ReadCollectionMissions; bagian berikut berada di luar batas blok tersebut dalam ReadCollectionMissions.
    }

    // Mendefinisikan metode `ReadFinancialGoals` dengan hasil bertipe `List<RulesetFinancialGoalDto>`; operasi ini menangani read keuangan target.
    // Masukan: Parameter `componentCatalog` bertipe `JsonElement` membawa nilai komponen catalog.
    private static List<RulesetFinancialGoalDto> ReadFinancialGoals(JsonElement componentCatalog)
    // Membuka scope metode ReadFinancialGoals; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ReadFinancialGoals.
    {
        // Mengembalikan memanggil `ReadArray` dengan `componentCatalog`, `”tujuanFinansial”`, `item => new RulesetFinancialGoalDto { Id = ReadString(item,
        // ”id”, string.Empty), Nama = ReadString(item, ”nama”, string.Empty), HargaBeli = ReadInt(item, ”hargaBeli”, 0), Poin...` kepada pemanggil dalam
        // ReadFinancialGoals; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return ReadArray(componentCatalog, "tujuanFinansial", item => new RulesetFinancialGoalDto
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ReadFinancialGoals.
        {
            // Memperbarui `Id` menggunakan memanggil `ReadString` dengan `item`, `”id”`, `string.Empty` dalam ReadFinancialGoals.
            Id = ReadString(item, "id", string.Empty),
            // Memperbarui `Nama` menggunakan memanggil `ReadString` dengan `item`, `”nama”`, `string.Empty` dalam ReadFinancialGoals.
            Nama = ReadString(item, "nama", string.Empty),
            // Memperbarui `HargaBeli` menggunakan memanggil `ReadInt` dengan `item`, `”hargaBeli”`, `0` dalam ReadFinancialGoals.
            HargaBeli = ReadInt(item, "hargaBeli", 0),
            // Memperbarui `PoinKebahagiaan` menggunakan memanggil `ReadInt` dengan `item`, `”poinKebahagiaan”`, `0` dalam ReadFinancialGoals.
            PoinKebahagiaan = ReadInt(item, "poinKebahagiaan", 0),
            // Memperbarui `CardQty` menggunakan memanggil `ReadNullableInt` dengan `item`, `”cardQty”` dalam ReadFinancialGoals.
            CardQty = ReadNullableInt(item, "cardQty")
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam ReadFinancialGoals.
        });
    // Menutup scope metode ReadFinancialGoals; bagian berikut berada di luar batas blok tersebut dalam ReadFinancialGoals.
    }

    // Mendefinisikan metode `ReadNarratives` dengan hasil bertipe `List<RulesetNarrativeDto>`; operasi ini menangani read narratives. Masukan:
    // Parameter `componentCatalog` bertipe `JsonElement` membawa nilai komponen catalog.
    private static List<RulesetNarrativeDto> ReadNarratives(JsonElement componentCatalog)
    // Membuka scope metode ReadNarratives; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ReadNarratives.
    {
        // Mengembalikan memanggil `ReadArray` dengan `componentCatalog`, `”narasi”`, `item => new RulesetNarrativeDto { Id = ReadString(item, ”id”,
        // string.Empty), Nama = ReadString(item, ”nama”, string.Empty), Teks = item.TryGetProperty(”teks”, out var teks) &&...` kepada pemanggil dalam
        // ReadNarratives; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return ReadArray(componentCatalog, "narasi", item => new RulesetNarrativeDto
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ReadNarratives.
        {
            // Memperbarui `Id` menggunakan memanggil `ReadString` dengan `item`, `”id”`, `string.Empty` dalam ReadNarratives.
            Id = ReadString(item, "id", string.Empty),
            // Memperbarui `Nama` menggunakan memanggil `ReadString` dengan `item`, `”nama”`, `string.Empty` dalam ReadNarratives.
            Nama = ReadString(item, "nama", string.Empty),
            // Memperbarui `Teks` menggunakan hasil pemilihan bersyarat: ketika `item.TryGetProperty(”teks”, out var teks) && teks.ValueKind ==
            // JsonValueKind.Array` benar gunakan `teks.EnumerateArray().Where(x => x.ValueKind == JsonValueKind.String).Select(x => x.GetString() ??
            // string.Empty).ToList()`, jika tidak gunakan `[]` dalam ReadNarratives.
            Teks = item.TryGetProperty("teks", out var teks) && teks.ValueKind == JsonValueKind.Array
                // Meneruskan fungsi lambda `x => x.ValueKind == JsonValueKind.String` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai
                // argumen ke `teks.EnumerateArray().Where`; Meneruskan fungsi lambda `x => x.GetString() ?? string.Empty` yang dijalankan oleh operasi pemanggil
                // untuk memproses setiap masukan sebagai argumen ke `teks.EnumerateArray().Where(x => x.ValueKind == JsonValueKind.String).Select`.
                ? teks.EnumerateArray().Where(x => x.ValueKind == JsonValueKind.String).Select(x => x.GetString() ?? string.Empty).ToList()
                // Meneruskan fungsi lambda `item => new RulesetNarrativeDto { Id = ReadString(item, ”id”, string.Empty), Nama = ReadString(item, ”nama”,
                // string.Empty), Teks = item.TryGetProperty(”teks”, out var teks) &&...` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan
                // sebagai argumen ke `ReadArray`.
                : [],
            // Memperbarui `PrerequisiteAksi` menggunakan hasil pemilihan bersyarat: ketika `item.TryGetProperty(”prerequisiteAksi”, out var prerequisites) &&
            // prerequisites.ValueKind == JsonValueKind.Array` benar gunakan `prerequisites.EnumerateArray().Select(prerequisite => new
            // RulesetNarrativePrerequisiteDto { Aksi = ReadString(prerequisite, ”aksi”, string.Empty), Value = ReadInt(prerequisite...`, jika tidak gunakan
            // `[]` dalam ReadNarratives.
            PrerequisiteAksi = item.TryGetProperty("prerequisiteAksi", out var prerequisites) && prerequisites.ValueKind == JsonValueKind.Array
                // Meneruskan fungsi lambda `prerequisite => new RulesetNarrativePrerequisiteDto { Aksi = ReadString(prerequisite, ”aksi”, string.Empty), Value =
                // ReadInt(prerequisite, ”value”, 0) }` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen ke
                // `prerequisites.EnumerateArray().Select`.
                ? prerequisites.EnumerateArray().Select(prerequisite => new RulesetNarrativePrerequisiteDto
                // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ReadNarratives.
                {
                    // Memperbarui `Aksi` menggunakan memanggil `ReadString` dengan `prerequisite`, `”aksi”`, `string.Empty` dalam ReadNarratives.
                    Aksi = ReadString(prerequisite, "aksi", string.Empty),
                    // Memperbarui `Value` menggunakan memanggil `ReadInt` dengan `prerequisite`, `”value”`, `0` dalam ReadNarratives.
                    Value = ReadInt(prerequisite, "value", 0)
                // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam ReadNarratives.
                }).ToList()
                // Meneruskan fungsi lambda `item => new RulesetNarrativeDto { Id = ReadString(item, ”id”, string.Empty), Nama = ReadString(item, ”nama”,
                // string.Empty), Teks = item.TryGetProperty(”teks”, out var teks) &&...` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan
                // sebagai argumen ke `ReadArray`.
                : []
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam ReadNarratives.
        });
    // Menutup scope metode ReadNarratives; bagian berikut berada di luar batas blok tersebut dalam ReadNarratives.
    }

    // Mendefinisikan metode `ReadNeedSetBonuses` dengan hasil bertipe `List<RulesetNeedSetBonusDto>`; operasi ini menangani read kebutuhan set bonuses.
    // Masukan: Parameter `root` bertipe `JsonElement` membawa nilai root.
    private static List<RulesetNeedSetBonusDto> ReadNeedSetBonuses(JsonElement root)
    // Membuka scope metode ReadNeedSetBonuses; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ReadNeedSetBonuses.
    {
        // Mengembalikan memanggil `ReadArray` dengan `root`, `”need_set_bonuses”`, `item => new RulesetNeedSetBonusDto { PatternCode = ReadString(item,
        // ”pattern_code”, string.Empty), RequiredCount = ReadInt(item, ”required_count”, 0), Points = ReadInt(item, ”...` kepada pemanggil dalam
        // ReadNeedSetBonuses; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return ReadArray(root, "need_set_bonuses", item => new RulesetNeedSetBonusDto
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ReadNeedSetBonuses.
        {
            // Memperbarui `PatternCode` menggunakan memanggil `ReadString` dengan `item`, `”pattern_code”`, `string.Empty` dalam ReadNeedSetBonuses.
            PatternCode = ReadString(item, "pattern_code", string.Empty),
            // Memperbarui `RequiredCount` menggunakan memanggil `ReadInt` dengan `item`, `”required_count”`, `0` dalam ReadNeedSetBonuses.
            RequiredCount = ReadInt(item, "required_count", 0),
            // Memperbarui `Points` menggunakan memanggil `ReadInt` dengan `item`, `”points”`, `0` dalam ReadNeedSetBonuses.
            Points = ReadInt(item, "points", 0)
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam ReadNeedSetBonuses.
        });
    // Menutup scope metode ReadNeedSetBonuses; bagian berikut berada di luar batas blok tersebut dalam ReadNeedSetBonuses.
    }

    // Mendefinisikan metode `ReadGoldPrices` dengan hasil bertipe `List<RulesetGoldPriceDto>`; operasi ini menangani read emas prices. Masukan:
    // Parameter `root` bertipe `JsonElement` membawa nilai root.
    private static List<RulesetGoldPriceDto> ReadGoldPrices(JsonElement root)
    // Membuka scope metode ReadGoldPrices; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ReadGoldPrices.
    {
        // Mengembalikan memanggil `ReadArray` dengan `root`, `”gold_prices”`, `item => new RulesetGoldPriceDto { PriceCode = ReadString(item, ”price_code”,
        // string.Empty), Qty = ReadInt(item, ”qty”, 0), UnitPrice = ReadInt(item, ”unit_price”, 0), CardQty ...` kepada pemanggil dalam ReadGoldPrices;
        // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return ReadArray(root, "gold_prices", item => new RulesetGoldPriceDto
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ReadGoldPrices.
        {
            // Memperbarui `PriceCode` menggunakan memanggil `ReadString` dengan `item`, `”price_code”`, `string.Empty` dalam ReadGoldPrices.
            PriceCode = ReadString(item, "price_code", string.Empty),
            // Memperbarui `Qty` menggunakan memanggil `ReadInt` dengan `item`, `”qty”`, `0` dalam ReadGoldPrices.
            Qty = ReadInt(item, "qty", 0),
            // Memperbarui `UnitPrice` menggunakan memanggil `ReadInt` dengan `item`, `”unit_price”`, `0` dalam ReadGoldPrices.
            UnitPrice = ReadInt(item, "unit_price", 0),
            // Memperbarui `CardQty` menggunakan memanggil `ReadNullableInt` dengan `item`, `”card_qty”` dalam ReadGoldPrices.
            CardQty = ReadNullableInt(item, "card_qty")
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam ReadGoldPrices.
        });
    // Menutup scope metode ReadGoldPrices; bagian berikut berada di luar batas blok tersebut dalam ReadGoldPrices.
    }

    // Mendefinisikan metode `ReadTieBreakers` dengan hasil bertipe `List<RulesetTieBreakerDto>`; operasi ini menangani read tie breakers. Masukan:
    // Parameter `root` bertipe `JsonElement` membawa nilai root.
    private static List<RulesetTieBreakerDto> ReadTieBreakers(JsonElement root)
    // Membuka scope metode ReadTieBreakers; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ReadTieBreakers.
    {
        // Mengembalikan memanggil `ReadArray` dengan `root`, `”tie_breakers”`, `item => new RulesetTieBreakerDto { TieBreakerCode = ReadString(item,
        // ”tie_breaker_code”, string.Empty), TieNumber = ReadInt(item, ”tie_number”, 0), CardQty = ReadNullableInt(i...` kepada pemanggil dalam
        // ReadTieBreakers; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return ReadArray(root, "tie_breakers", item => new RulesetTieBreakerDto
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ReadTieBreakers.
        {
            // Memperbarui `TieBreakerCode` menggunakan memanggil `ReadString` dengan `item`, `”tie_breaker_code”`, `string.Empty` dalam ReadTieBreakers.
            TieBreakerCode = ReadString(item, "tie_breaker_code", string.Empty),
            // Memperbarui `TieNumber` menggunakan memanggil `ReadInt` dengan `item`, `”tie_number”`, `0` dalam ReadTieBreakers.
            TieNumber = ReadInt(item, "tie_number", 0),
            // Memperbarui `CardQty` menggunakan memanggil `ReadNullableInt` dengan `item`, `”card_qty”` dalam ReadTieBreakers.
            CardQty = ReadNullableInt(item, "card_qty")
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam ReadTieBreakers.
        });
    // Menutup scope metode ReadTieBreakers; bagian berikut berada di luar batas blok tersebut dalam ReadTieBreakers.
    }

    // Mendefinisikan metode `ReadShariaLoans` dengan hasil bertipe `List<RulesetShariaLoanDto>`; operasi ini menangani read sharia pinjaman. Masukan:
    // Parameter `root` bertipe `JsonElement` membawa nilai root.
    private static List<RulesetShariaLoanDto> ReadShariaLoans(JsonElement root)
    // Membuka scope metode ReadShariaLoans; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ReadShariaLoans.
    {
        // Mengembalikan memanggil `ReadArray` dengan `root`, `”sharia_loans”`, `item => new RulesetShariaLoanDto { LoanCode = ReadString(item, ”loan_code”,
        // string.Empty), ItemName = ReadString(item, ”item_name”, string.Empty), Principal = ReadInt(item, ”p...` kepada pemanggil dalam ReadShariaLoans;
        // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return ReadArray(root, "sharia_loans", item => new RulesetShariaLoanDto
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ReadShariaLoans.
        {
            // Memperbarui `LoanCode` menggunakan memanggil `ReadString` dengan `item`, `”loan_code”`, `string.Empty` dalam ReadShariaLoans.
            LoanCode = ReadString(item, "loan_code", string.Empty),
            // Memperbarui `ItemName` menggunakan memanggil `ReadString` dengan `item`, `”item_name”`, `string.Empty` dalam ReadShariaLoans.
            ItemName = ReadString(item, "item_name", string.Empty),
            // Memperbarui `Principal` menggunakan memanggil `ReadInt` dengan `item`, `”principal”`, `0` dalam ReadShariaLoans.
            Principal = ReadInt(item, "principal", 0),
            // Memperbarui `RepaymentAmount` menggunakan memanggil `ReadInt` dengan `item`, `”repayment_amount”`, `0` dalam ReadShariaLoans.
            RepaymentAmount = ReadInt(item, "repayment_amount", 0),
            // Memperbarui `DurationDays` menggunakan memanggil `ReadInt` dengan `item`, `”duration_days”`, `0` dalam ReadShariaLoans.
            DurationDays = ReadInt(item, "duration_days", 0),
            // Memperbarui `PenaltyPoints` menggunakan memanggil `ReadInt` dengan `item`, `”penalty_points”`, `0` dalam ReadShariaLoans.
            PenaltyPoints = ReadInt(item, "penalty_points", 0),
            // Memperbarui `CardQty` menggunakan memanggil `ReadNullableInt` dengan `item`, `”card_qty”` dalam ReadShariaLoans.
            CardQty = ReadNullableInt(item, "card_qty")
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam ReadShariaLoans.
        });
    // Menutup scope metode ReadShariaLoans; bagian berikut berada di luar batas blok tersebut dalam ReadShariaLoans.
    }

    // Mendefinisikan metode `ReadInsuranceProducts` dengan hasil bertipe `List<RulesetInsuranceProductDto>`; operasi ini menangani read asuransi
    // products. Masukan: Parameter `root` bertipe `JsonElement` membawa nilai root.
    private static List<RulesetInsuranceProductDto> ReadInsuranceProducts(JsonElement root)
    // Membuka scope metode ReadInsuranceProducts; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ReadInsuranceProducts.
    {
        // Mengembalikan memanggil `ReadArray` dengan `root`, `”insurance_products”`, `item => new RulesetInsuranceProductDto { ProductCode =
        // ReadString(item, ”product_code”, string.Empty), ItemName = ReadString(item, ”item_name”, string.Empty), Premium = ReadIn...` kepada pemanggil
        // dalam ReadInsuranceProducts; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return ReadArray(root, "insurance_products", item => new RulesetInsuranceProductDto
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ReadInsuranceProducts.
        {
            // Memperbarui `ProductCode` menggunakan memanggil `ReadString` dengan `item`, `”product_code”`, `string.Empty` dalam ReadInsuranceProducts.
            ProductCode = ReadString(item, "product_code", string.Empty),
            // Memperbarui `ItemName` menggunakan memanggil `ReadString` dengan `item`, `”item_name”`, `string.Empty` dalam ReadInsuranceProducts.
            ItemName = ReadString(item, "item_name", string.Empty),
            // Memperbarui `Premium` menggunakan memanggil `ReadInt` dengan `item`, `”premium”`, `0` dalam ReadInsuranceProducts.
            Premium = ReadInt(item, "premium", 0),
            // Memperbarui `UsageLimit` menggunakan memanggil `ReadInt` dengan `item`, `”usage_limit”`, `0` dalam ReadInsuranceProducts.
            UsageLimit = ReadInt(item, "usage_limit", 0),
            // Memperbarui `CardQty` menggunakan memanggil `ReadNullableInt` dengan `item`, `”card_qty”` dalam ReadInsuranceProducts.
            CardQty = ReadNullableInt(item, "card_qty")
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam ReadInsuranceProducts.
        });
    // Menutup scope metode ReadInsuranceProducts; bagian berikut berada di luar batas blok tersebut dalam ReadInsuranceProducts.
    }

    // Mendefinisikan metode `ReadLifeRisks` dengan hasil bertipe `List<RulesetLifeRiskDto>`; operasi ini menangani read life risks. Masukan: Parameter
    // `root` bertipe `JsonElement` membawa nilai root.
    private static List<RulesetLifeRiskDto> ReadLifeRisks(JsonElement root)
    // Membuka scope metode ReadLifeRisks; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ReadLifeRisks.
    {
        // Mengembalikan memanggil `ReadArray` dengan `root`, `”life_risks”`, `item => new RulesetLifeRiskDto { RiskCode = ReadString(item, ”risk_code”,
        // string.Empty), ItemName = ReadString(item, ”item_name”, string.Empty), EffectType = ReadString(item, ...` kepada pemanggil dalam ReadLifeRisks;
        // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return ReadArray(root, "life_risks", item => new RulesetLifeRiskDto
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ReadLifeRisks.
        {
            // Memperbarui `RiskCode` menggunakan memanggil `ReadString` dengan `item`, `”risk_code”`, `string.Empty` dalam ReadLifeRisks.
            RiskCode = ReadString(item, "risk_code", string.Empty),
            // Memperbarui `ItemName` menggunakan memanggil `ReadString` dengan `item`, `”item_name”`, `string.Empty` dalam ReadLifeRisks.
            ItemName = ReadString(item, "item_name", string.Empty),
            // Memperbarui `EffectType` menggunakan memanggil `ReadString` dengan `item`, `”effect_type”`, `string.Empty` dalam ReadLifeRisks.
            EffectType = ReadString(item, "effect_type", string.Empty),
            // Memperbarui `Direction` menggunakan memanggil `ReadString` dengan `item`, `”direction”`, `string.Empty` dalam ReadLifeRisks.
            Direction = ReadString(item, "direction", string.Empty),
            // Memperbarui `Amount` menggunakan memanggil `ReadInt` dengan `item`, `”amount”`, `0` dalam ReadLifeRisks.
            Amount = ReadInt(item, "amount", 0),
            // Memperbarui `TargetScope` menggunakan memanggil `ReadString` dengan `item`, `”target_scope”`, `”SELF”` dalam ReadLifeRisks.
            TargetScope = ReadString(item, "target_scope", "SELF"),
            // Memperbarui `ValueDelta` menggunakan memanggil `ReadNullableInt` dengan `item`, `”value_delta”` dalam ReadLifeRisks.
            ValueDelta = ReadNullableInt(item, "value_delta"),
            // Memperbarui `DurationDays` menggunakan memanggil `ReadNullableInt` dengan `item`, `”duration_days”` dalam ReadLifeRisks.
            DurationDays = ReadNullableInt(item, "duration_days"),
            // Memperbarui `CardQty` menggunakan memanggil `ReadNullableInt` dengan `item`, `”card_qty”` dalam ReadLifeRisks.
            CardQty = ReadNullableInt(item, "card_qty")
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam ReadLifeRisks.
        });
    // Menutup scope metode ReadLifeRisks; bagian berikut berada di luar batas blok tersebut dalam ReadLifeRisks.
    }

    // Mendefinisikan metode `ReadRankPoints` dengan hasil bertipe `List<RankPointShape>`; operasi ini menangani read rank poin. Masukan: Parameter
    // `root` bertipe `JsonElement` membawa nilai root; Parameter `propertyName` bertipe `string` membawa nilai property nama.
    private static List<RankPointShape> ReadRankPoints(JsonElement root, string propertyName)
    // Membuka scope metode ReadRankPoints; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ReadRankPoints.
    {
        // Mengembalikan hasil pemilihan bersyarat: ketika `root.ValueKind == JsonValueKind.Object && root.TryGetProperty(propertyName, out var array) &&
        // array.ValueKind == JsonValueKind.Array` benar gunakan `array.EnumerateArray().Select(item => new RankPointShape(ReadInt(item, ”rank”, 0),
        // ReadInt(item, ”points”, 0))).ToList()`, jika tidak gunakan `[]` kepada pemanggil dalam ReadRankPoints; eksekusi jalur ini selesai setelah nilai
        // hasil ditentukan.
        return root.ValueKind == JsonValueKind.Object && root.TryGetProperty(propertyName, out var array) && array.ValueKind == JsonValueKind.Array
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: array.EnumerateArray().Select(item => new
            // RankPointShape(ReadInt(item, ”rank”, 0), ReadInt(item, ”points”, 0))).ToList() dalam ReadRankPoints.
            ? array.EnumerateArray().Select(item => new RankPointShape(ReadInt(item, "rank", 0), ReadInt(item, "points", 0))).ToList()
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: []; dalam ReadRankPoints.
            : [];
    // Menutup scope metode ReadRankPoints; bagian berikut berada di luar batas blok tersebut dalam ReadRankPoints.
    }

    // Mendefinisikan metode `ReadQtyPoints` dengan hasil bertipe `List<QtyPointShape>`; operasi ini menangani read qty poin. Masukan: Parameter `root`
    // bertipe `JsonElement` membawa nilai root; Parameter `propertyName` bertipe `string` membawa nilai property nama.
    private static List<QtyPointShape> ReadQtyPoints(JsonElement root, string propertyName)
    // Membuka scope metode ReadQtyPoints; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ReadQtyPoints.
    {
        // Mengembalikan hasil pemilihan bersyarat: ketika `root.ValueKind == JsonValueKind.Object && root.TryGetProperty(propertyName, out var array) &&
        // array.ValueKind == JsonValueKind.Array` benar gunakan `array.EnumerateArray().Select(item => new QtyPointShape(ReadInt(item, ”qty”, 0),
        // ReadInt(item, ”points”, 0))).ToList()`, jika tidak gunakan `[]` kepada pemanggil dalam ReadQtyPoints; eksekusi jalur ini selesai setelah nilai
        // hasil ditentukan.
        return root.ValueKind == JsonValueKind.Object && root.TryGetProperty(propertyName, out var array) && array.ValueKind == JsonValueKind.Array
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: array.EnumerateArray().Select(item => new QtyPointShape(ReadInt(item,
            // ”qty”, 0), ReadInt(item, ”points”, 0))).ToList() dalam ReadQtyPoints.
            ? array.EnumerateArray().Select(item => new QtyPointShape(ReadInt(item, "qty", 0), ReadInt(item, "points", 0))).ToList()
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: []; dalam ReadQtyPoints.
            : [];
    // Menutup scope metode ReadQtyPoints; bagian berikut berada di luar batas blok tersebut dalam ReadQtyPoints.
    }

    // Mendefinisikan metode `ReadArray` dengan hasil bertipe `List<T>`; operasi ini menangani read array. Masukan: Parameter `root` bertipe
    // `JsonElement` membawa nilai root; Parameter `propertyName` bertipe `string` membawa nilai property nama; Parameter `selector` bertipe
    // `Func<JsonElement, T>` membawa nilai selector.
    private static List<T> ReadArray<T>(JsonElement root, string propertyName, Func<JsonElement, T> selector)
    // Membuka scope metode ReadArray; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ReadArray.
    {
        // Mengembalikan hasil pemilihan bersyarat: ketika `root.TryGetProperty(propertyName, out var array) && array.ValueKind == JsonValueKind.Array`
        // benar gunakan `array.EnumerateArray().Select(selector).ToList()`, jika tidak gunakan `[]` kepada pemanggil dalam ReadArray; eksekusi jalur ini
        // selesai setelah nilai hasil ditentukan.
        return root.TryGetProperty(propertyName, out var array) && array.ValueKind == JsonValueKind.Array
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: array.EnumerateArray().Select(selector).ToList() dalam ReadArray.
            ? array.EnumerateArray().Select(selector).ToList()
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: []; dalam ReadArray.
            : [];
    // Menutup scope metode ReadArray; bagian berikut berada di luar batas blok tersebut dalam ReadArray.
    }

    // Mendefinisikan metode `ReadString` dengan hasil bertipe `string`; operasi ini menangani read string. Masukan: Parameter `root` bertipe
    // `JsonElement` membawa nilai root; Parameter `propertyName` bertipe `string` membawa nilai property nama; Parameter `fallback` bertipe `string`
    // membawa nilai fallback.
    private static string ReadString(JsonElement root, string propertyName, string fallback)
    // Membuka scope metode ReadString; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ReadString.
    {
        // Mengembalikan hasil pemilihan bersyarat: ketika `root.TryGetProperty(propertyName, out var property) && property.ValueKind ==
        // JsonValueKind.String` benar gunakan `property.GetString() ?? fallback`, jika tidak gunakan `fallback` kepada pemanggil dalam ReadString; eksekusi
        // jalur ini selesai setelah nilai hasil ditentukan.
        return root.TryGetProperty(propertyName, out var property) && property.ValueKind == JsonValueKind.String
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: property.GetString() ?? fallback dalam ReadString.
            ? property.GetString() ?? fallback
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: fallback; dalam ReadString.
            : fallback;
    // Menutup scope metode ReadString; bagian berikut berada di luar batas blok tersebut dalam ReadString.
    }

    // Mendefinisikan metode `ReadNullableString` dengan hasil bertipe `string?`; operasi ini menangani read nullable string. Masukan: Parameter `root`
    // bertipe `JsonElement` membawa nilai root; Parameter `propertyName` bertipe `string` membawa nilai property nama.
    private static string? ReadNullableString(JsonElement root, string propertyName)
    // Membuka scope metode ReadNullableString; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ReadNullableString.
    {
        // Mengembalikan hasil pemilihan bersyarat: ketika `root.TryGetProperty(propertyName, out var property) && property.ValueKind ==
        // JsonValueKind.String` benar gunakan `property.GetString()`, jika tidak gunakan `null` kepada pemanggil dalam ReadNullableString; eksekusi jalur
        // ini selesai setelah nilai hasil ditentukan.
        return root.TryGetProperty(propertyName, out var property) && property.ValueKind == JsonValueKind.String
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: property.GetString() dalam ReadNullableString.
            ? property.GetString()
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: null; dalam ReadNullableString.
            : null;
    // Menutup scope metode ReadNullableString; bagian berikut berada di luar batas blok tersebut dalam ReadNullableString.
    }

    // Mendefinisikan metode `ReadInt` dengan hasil bertipe `int`; operasi ini menangani read int. Masukan: Parameter `root` bertipe `JsonElement`
    // membawa nilai root; Parameter `propertyName` bertipe `string` membawa nilai property nama; Parameter `fallback` bertipe `int` membawa nilai
    // fallback.
    private static int ReadInt(JsonElement root, string propertyName, int fallback)
    // Membuka scope metode ReadInt; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ReadInt.
    {
        // Mengembalikan hasil pemilihan bersyarat: ketika `root.TryGetProperty(propertyName, out var property) && property.ValueKind ==
        // JsonValueKind.Number && property.TryGetInt32(out var value)` benar gunakan `value`, jika tidak gunakan `fallback` kepada pemanggil dalam ReadInt;
        // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return root.TryGetProperty(propertyName, out var property) &&
               // Melanjutkan ekspresi dengan perbandingan kesamaan antara `property.ValueKind` dan `JsonValueKind.Number` dalam ReadInt.
               property.ValueKind == JsonValueKind.Number &&
               // Melanjutkan pengolahan dengan memanggil `property.TryGetInt32` dengan `var value` dalam ReadInt.
               property.TryGetInt32(out var value)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: value dalam ReadInt.
            ? value
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: fallback; dalam ReadInt.
            : fallback;
    // Menutup scope metode ReadInt; bagian berikut berada di luar batas blok tersebut dalam ReadInt.
    }

    // Mendefinisikan metode `ReadNullableInt` dengan hasil bertipe `int?`; operasi ini menangani read nullable int. Masukan: Parameter `root` bertipe
    // `JsonElement` membawa nilai root; Parameter `propertyName` bertipe `string` membawa nilai property nama.
    private static int? ReadNullableInt(JsonElement root, string propertyName)
    // Membuka scope metode ReadNullableInt; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ReadNullableInt.
    {
        // Mengembalikan hasil pemilihan bersyarat: ketika `root.TryGetProperty(propertyName, out var property) && property.ValueKind ==
        // JsonValueKind.Number && property.TryGetInt32(out var value)` benar gunakan `value`, jika tidak gunakan `null` kepada pemanggil dalam
        // ReadNullableInt; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return root.TryGetProperty(propertyName, out var property) &&
               // Melanjutkan ekspresi dengan perbandingan kesamaan antara `property.ValueKind` dan `JsonValueKind.Number` dalam ReadNullableInt.
               property.ValueKind == JsonValueKind.Number &&
               // Melanjutkan pengolahan dengan memanggil `property.TryGetInt32` dengan `var value` dalam ReadNullableInt.
               property.TryGetInt32(out var value)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: value dalam ReadNullableInt.
            ? value
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: null; dalam ReadNullableInt.
            : null;
    // Menutup scope metode ReadNullableInt; bagian berikut berada di luar batas blok tersebut dalam ReadNullableInt.
    }

    // Mendefinisikan metode `ReadBool` dengan hasil bertipe `bool`; operasi ini menangani read bool. Masukan: Parameter `root` bertipe `JsonElement`
    // membawa nilai root; Parameter `propertyName` bertipe `string` membawa nilai property nama; Parameter `fallback` bertipe `bool` membawa nilai
    // fallback.
    private static bool ReadBool(JsonElement root, string propertyName, bool fallback)
    // Membuka scope metode ReadBool; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ReadBool.
    {
        // Mengembalikan hasil pemilihan bersyarat: ketika `root.TryGetProperty(propertyName, out var property) && property.ValueKind is JsonValueKind.True
        // or JsonValueKind.False` benar gunakan `property.GetBoolean()`, jika tidak gunakan `fallback` kepada pemanggil dalam ReadBool; eksekusi jalur ini
        // selesai setelah nilai hasil ditentukan.
        return root.TryGetProperty(propertyName, out var property) && property.ValueKind is JsonValueKind.True or JsonValueKind.False
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: property.GetBoolean() dalam ReadBool.
            ? property.GetBoolean()
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: fallback; dalam ReadBool.
            : fallback;
    // Menutup scope metode ReadBool; bagian berikut berada di luar batas blok tersebut dalam ReadBool.
    }

    // Mendefinisikan metode `ReadNestedBool` dengan hasil bertipe `bool`; operasi ini menangani read nested bool. Masukan: Parameter `root` bertipe
    // `JsonElement` membawa nilai root; Parameter `propertyName` bertipe `string` membawa nilai property nama; Parameter `nestedPropertyName` bertipe
    // `string` membawa nilai nested property nama.
    private static bool ReadNestedBool(JsonElement root, string propertyName, string nestedPropertyName)
    // Membuka scope metode ReadNestedBool; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ReadNestedBool.
    {
        // Mengembalikan gabungan syarat AND: kedua kondisi wajib benar antara `root.TryGetProperty(propertyName, out var property) && property.ValueKind ==
        // JsonValueKind.Object && property.TryGetProperty(nestedPropertyName, out var nested) && nested.Valu...` dan `nested.GetBoolean()`; sisi kanan
        // diperiksa hanya jika sisi kiri benar kepada pemanggil dalam ReadNestedBool; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return root.TryGetProperty(propertyName, out var property) &&
               // Melanjutkan ekspresi dengan perbandingan kesamaan antara `property.ValueKind` dan `JsonValueKind.Object` dalam ReadNestedBool.
               property.ValueKind == JsonValueKind.Object &&
               // Melanjutkan pengolahan dengan mencari properti JSON `nestedPropertyName`, `var nested` pada `property` tanpa menganggap propertinya selalu
               // tersedia dalam ReadNestedBool.
               property.TryGetProperty(nestedPropertyName, out var nested) &&
               // Menggunakan `nested` (nilai nested) sebagai bagian ekspresi yang sedang disusun dalam ReadNestedBool.
               nested.ValueKind is JsonValueKind.True or JsonValueKind.False &&
               // Melanjutkan pengolahan dengan memanggil `nested.GetBoolean` dengan tanpa argumen dalam ReadNestedBool.
               nested.GetBoolean();
    // Menutup scope metode ReadNestedBool; bagian berikut berada di luar batas blok tersebut dalam ReadNestedBool.
    }

    // Mendefinisikan metode `ReadNestedWeekdayFeature` dengan hasil bertipe `string`; operasi ini menangani read nested weekday feature. Masukan:
    // Parameter `weekdayRules` bertipe `JsonElement` membawa nilai weekday rules; Parameter `primaryName` bertipe `string` membawa nilai primary nama;
    // Parameter `fallbackName` bertipe `string` membawa nilai fallback nama; Parameter `fallback` bertipe `string` membawa nilai fallback.
    private static string ReadNestedWeekdayFeature(JsonElement weekdayRules, string primaryName, string fallbackName, string fallback)
    // Membuka scope metode ReadNestedWeekdayFeature; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ReadNestedWeekdayFeature.
    {
        // Mengembalikan `TryGetWeekdayProperty(weekdayRules, primaryName, ”feature”)` bila tidak null; jika null gunakan
        // `TryGetWeekdayProperty(weekdayRules, fallbackName, ”feature”) ?? fallback` sebagai nilai pengganti kepada pemanggil dalam
        // ReadNestedWeekdayFeature; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return TryGetWeekdayProperty(weekdayRules, primaryName, "feature")
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: TryGetWeekdayProperty(weekdayRules, fallbackName, ”feature”) dalam
            // ReadNestedWeekdayFeature.
            ?? TryGetWeekdayProperty(weekdayRules, fallbackName, "feature")
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: fallback; dalam ReadNestedWeekdayFeature.
            ?? fallback;
    // Menutup scope metode ReadNestedWeekdayFeature; bagian berikut berada di luar batas blok tersebut dalam ReadNestedWeekdayFeature.
    }

    // Mendefinisikan metode `ReadNestedWeekdayEnabled` dengan hasil bertipe `bool`; operasi ini menangani read nested weekday enabled. Masukan:
    // Parameter `weekdayRules` bertipe `JsonElement` membawa nilai weekday rules; Parameter `primaryName` bertipe `string` membawa nilai primary nama;
    // Parameter `fallbackName` bertipe `string` membawa nilai fallback nama; Parameter `fallback` bertipe `bool` membawa nilai fallback.
    private static bool ReadNestedWeekdayEnabled(JsonElement weekdayRules, string primaryName, string fallbackName, bool fallback)
    // Membuka scope metode ReadNestedWeekdayEnabled; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ReadNestedWeekdayEnabled.
    {
        // Mengembalikan `TryGetWeekdayPropertyBool(weekdayRules, primaryName, ”enabled”)` bila tidak null; jika null gunakan
        // `TryGetWeekdayPropertyBool(weekdayRules, fallbackName, ”enabled”) ?? fallback` sebagai nilai pengganti kepada pemanggil dalam
        // ReadNestedWeekdayEnabled; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return TryGetWeekdayPropertyBool(weekdayRules, primaryName, "enabled")
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: TryGetWeekdayPropertyBool(weekdayRules, fallbackName, ”enabled”)
            // dalam ReadNestedWeekdayEnabled.
            ?? TryGetWeekdayPropertyBool(weekdayRules, fallbackName, "enabled")
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: fallback; dalam ReadNestedWeekdayEnabled.
            ?? fallback;
    // Menutup scope metode ReadNestedWeekdayEnabled; bagian berikut berada di luar batas blok tersebut dalam ReadNestedWeekdayEnabled.
    }

    // Mendefinisikan metode `TryGetWeekdayProperty` dengan hasil bertipe `string?`; operasi ini menangani try get weekday property. Masukan: Parameter
    // `weekdayRules` bertipe `JsonElement` membawa nilai weekday rules; Parameter `weekdayName` bertipe `string` membawa nilai weekday nama; Parameter
    // `propertyName` bertipe `string` membawa nilai property nama.
    private static string? TryGetWeekdayProperty(JsonElement weekdayRules, string weekdayName, string propertyName)
    // Membuka scope metode TryGetWeekdayProperty; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryGetWeekdayProperty.
    {
        // Mengembalikan hasil pemilihan bersyarat: ketika `weekdayRules.TryGetProperty(weekdayName, out var weekday) && weekday.ValueKind ==
        // JsonValueKind.Object && weekday.TryGetProperty(propertyName, out var property) && property.Va...` benar gunakan `property.GetString()`, jika
        // tidak gunakan `null` kepada pemanggil dalam TryGetWeekdayProperty; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return weekdayRules.TryGetProperty(weekdayName, out var weekday) &&
               // Melanjutkan ekspresi dengan perbandingan kesamaan antara `weekday.ValueKind` dan `JsonValueKind.Object` dalam TryGetWeekdayProperty.
               weekday.ValueKind == JsonValueKind.Object &&
               // Melanjutkan pengolahan dengan mencari properti JSON `propertyName`, `var property` pada `weekday` tanpa menganggap propertinya selalu tersedia
               // dalam TryGetWeekdayProperty.
               weekday.TryGetProperty(propertyName, out var property) &&
               // Melanjutkan ekspresi dengan perbandingan kesamaan antara `property.ValueKind` dan `JsonValueKind.String` dalam TryGetWeekdayProperty.
               property.ValueKind == JsonValueKind.String
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: property.GetString() dalam TryGetWeekdayProperty.
            ? property.GetString()
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: null; dalam TryGetWeekdayProperty.
            : null;
    // Menutup scope metode TryGetWeekdayProperty; bagian berikut berada di luar batas blok tersebut dalam TryGetWeekdayProperty.
    }

    // Mendefinisikan metode `TryGetWeekdayPropertyBool` dengan hasil bertipe `bool?`; operasi ini menangani try get weekday property bool. Masukan:
    // Parameter `weekdayRules` bertipe `JsonElement` membawa nilai weekday rules; Parameter `weekdayName` bertipe `string` membawa nilai weekday nama;
    // Parameter `propertyName` bertipe `string` membawa nilai property nama.
    private static bool? TryGetWeekdayPropertyBool(JsonElement weekdayRules, string weekdayName, string propertyName)
    // Membuka scope metode TryGetWeekdayPropertyBool; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryGetWeekdayPropertyBool.
    {
        // Mengembalikan hasil pemilihan bersyarat: ketika `weekdayRules.TryGetProperty(weekdayName, out var weekday) && weekday.ValueKind ==
        // JsonValueKind.Object && weekday.TryGetProperty(propertyName, out var property) && property.Va...` benar gunakan `property.GetBoolean()`, jika
        // tidak gunakan `null` kepada pemanggil dalam TryGetWeekdayPropertyBool; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return weekdayRules.TryGetProperty(weekdayName, out var weekday) &&
               // Melanjutkan ekspresi dengan perbandingan kesamaan antara `weekday.ValueKind` dan `JsonValueKind.Object` dalam TryGetWeekdayPropertyBool.
               weekday.ValueKind == JsonValueKind.Object &&
               // Melanjutkan pengolahan dengan mencari properti JSON `propertyName`, `var property` pada `weekday` tanpa menganggap propertinya selalu tersedia
               // dalam TryGetWeekdayPropertyBool.
               weekday.TryGetProperty(propertyName, out var property) &&
               // Menggunakan `property` (nilai property) sebagai bagian ekspresi yang sedang disusun dalam TryGetWeekdayPropertyBool.
               property.ValueKind is JsonValueKind.True or JsonValueKind.False
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: property.GetBoolean() dalam TryGetWeekdayPropertyBool.
            ? property.GetBoolean()
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: null; dalam TryGetWeekdayPropertyBool.
            : null;
    // Menutup scope metode TryGetWeekdayPropertyBool; bagian berikut berada di luar batas blok tersebut dalam TryGetWeekdayPropertyBool.
    }

    // Mendefinisikan metode `ReadStringList` dengan hasil bertipe `List<string>`; operasi ini menangani read string daftar. Masukan: Parameter `root`
    // bertipe `JsonElement` membawa nilai root; Parameter `propertyName` bertipe `string` membawa nilai property nama.
    private static List<string> ReadStringList(JsonElement root, string propertyName)
    // Membuka scope metode ReadStringList; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ReadStringList.
    {
        // Mengembalikan hasil pemilihan bersyarat: ketika `root.TryGetProperty(propertyName, out var array) && array.ValueKind == JsonValueKind.Array`
        // benar gunakan `array.EnumerateArray() .Where(x => x.ValueKind == JsonValueKind.String) .Select(x => x.GetString() ?? string.Empty) .Where(x =>
        // !string.IsNullOrWhiteSpace(x)) .ToList()`, jika tidak gunakan `[]` kepada pemanggil dalam ReadStringList; eksekusi jalur ini selesai setelah
        // nilai hasil ditentukan.
        return root.TryGetProperty(propertyName, out var array) && array.ValueKind == JsonValueKind.Array
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: array.EnumerateArray() dalam ReadStringList.
            ? array.EnumerateArray()
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(x => x.ValueKind == JsonValueKind.String) dalam ReadStringList; token
                // pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
                .Where(x => x.ValueKind == JsonValueKind.String)
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Select(x => x.GetString() ?? string.Empty) dalam ReadStringList; token pada
                // baris ini menyambungkan bagian kode sebelum dan sesudahnya.
                .Select(x => x.GetString() ?? string.Empty)
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(x => !string.IsNullOrWhiteSpace(x)) dalam ReadStringList; token pada
                // baris ini menyambungkan bagian kode sebelum dan sesudahnya.
                .Where(x => !string.IsNullOrWhiteSpace(x))
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList() dalam ReadStringList; token pada baris ini menyambungkan bagian kode
                // sebelum dan sesudahnya.
                .ToList()
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: []; dalam ReadStringList.
            : [];
    // Menutup scope metode ReadStringList; bagian berikut berada di luar batas blok tersebut dalam ReadStringList.
    }

    // Mendefinisikan metode `ReadPayload` dengan hasil bertipe `JsonElement`; operasi ini menangani read payload. Masukan: Parameter `script` bertipe
    // `JsonElement` membawa nilai script.
    private static JsonElement ReadPayload(JsonElement script)
    // Membuka scope metode ReadPayload; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ReadPayload.
    {
        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `script.TryGetProperty(”payload”, out var payload)` dan
        // `script.TryGetProperty(”payload_json”, out payload)`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi
        // ini bernilai benar dalam ReadPayload.
        if (script.TryGetProperty("payload", out var payload) || script.TryGetProperty("payload_json", out payload))
        // Membuka scope cabang if untuk kondisi `script.TryGetProperty(”payload”, out var payload) || script.TryGetProperty(”payload_json”, out payload)`;
        // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ReadPayload.
        {
            // Mengembalikan membuat salinan `payload` agar hasil dapat digunakan terpisah dari objek sumber kepada pemanggil dalam ReadPayload; eksekusi jalur
            // ini selesai setelah nilai hasil ditentukan.
            return payload.Clone();
        // Menutup scope cabang if untuk kondisi `script.TryGetProperty(”payload”, out var payload) || script.TryGetProperty(”payload_json”, out payload)`;
        // bagian berikut berada di luar batas blok tersebut dalam ReadPayload.
        }

        // Menyiapkan variabel lokal `empty` untuk nilai empty dengan memanggil `JsonDocument.Parse` dengan `”{}”`. Tipe variabel disimpulkan dari ekspresi
        // nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var empty = JsonDocument.Parse("{}");
        // Mengembalikan membuat salinan `empty.RootElement` agar hasil dapat digunakan terpisah dari objek sumber kepada pemanggil dalam ReadPayload;
        // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return empty.RootElement.Clone();
    // Menutup scope metode ReadPayload; bagian berikut berada di luar batas blok tersebut dalam ReadPayload.
    }

    // Mendefinisikan metode `CloneJson` dengan hasil bertipe `JsonNode`; operasi ini menangani clone JSON. Masukan: Parameter `element` bertipe
    // `JsonElement` membawa nilai element.
    private static JsonNode CloneJson(JsonElement element)
    // Membuka scope metode CloneJson; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CloneJson.
    {
        // Mengembalikan `JsonNode.Parse(element.ValueKind is JsonValueKind.Undefined or JsonValueKind.Null ? ”{}” : element.GetRawText())` bila tidak null;
        // jika null gunakan `new JsonObject()` sebagai nilai pengganti kepada pemanggil dalam CloneJson; eksekusi jalur ini selesai setelah nilai hasil
        // ditentukan.
        return JsonNode.Parse(element.ValueKind is JsonValueKind.Undefined or JsonValueKind.Null
            // Meneruskan hasil pemilihan bersyarat: ketika `element.ValueKind is JsonValueKind.Undefined or JsonValueKind.Null` benar gunakan `”{}”`, jika
            // tidak gunakan `element.GetRawText()` sebagai argumen ke `JsonNode.Parse`.
            ? "{}"
            // Meneruskan hasil pemilihan bersyarat: ketika `element.ValueKind is JsonValueKind.Undefined or JsonValueKind.Null` benar gunakan `”{}”`, jika
            // tidak gunakan `element.GetRawText()` sebagai argumen ke `JsonNode.Parse`.
            : element.GetRawText()) ?? new JsonObject();
    // Menutup scope metode CloneJson; bagian berikut berada di luar batas blok tersebut dalam CloneJson.
    }

    // Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `RankPointShape`; sealed mencegah tipe ini diturunkan lagi.
    private sealed record RankPointShape(int Rank, int Points);
    // Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `QtyPointShape`; sealed mencegah tipe ini diturunkan lagi.
    private sealed record QtyPointShape(int Qty, int Points);
// Menutup scope tipe RulesetDefinitionMapper; bagian berikut berada di luar batas blok tersebut.
}
