// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui RulesetDefinitionMapper.
using System.Text.Json;
using System.Text.Json.Nodes;
using Cashflowpoly.Api.Contracts;

namespace Cashflowpoly.Api.Domain;

public static class RulesetDefinitionMapper
{
    public static RulesetDefinitionDto FromConfigJson(string configJson)
    {
        using var document = JsonDocument.Parse(configJson);
        var root = document.RootElement;
        var componentCatalog = root.GetProperty("component_catalog");
        var gameConfig = componentCatalog.GetProperty("gameConfig");
        var weekdayRules = root.GetProperty("weekday_rules");
        var constraints = root.GetProperty("constraints");
        var donation = root.GetProperty("donation");
        var goldTrade = root.GetProperty("gold_trade");
        var advanced = root.GetProperty("advanced");
        var scoring = root.TryGetProperty("scoring", out var scoringElement) ? scoringElement : default;
        var freelance = root.TryGetProperty("freelance", out var freelanceElement) ? freelanceElement : default;

        return new RulesetDefinitionDto
        {
            Mode = ReadString(root, "mode", "MAHIR"),
            Settings = new RulesetSettingsDto
            {
                ActionsPerTurn = ReadInt(root, "actions_per_turn", 2),
                StartingCash = ReadInt(root, "starting_cash", ReadInt(gameConfig, "initialCoins", 0)),
                InitialCoins = ReadInt(gameConfig, "initialCoins", ReadInt(root, "starting_cash", 0)),
                InitialHappiness = ReadInt(gameConfig, "initialHappiness", 0),
                InitialSaving = ReadInt(gameConfig, "initialSaving", 0),
                FinishDay = ReadInt(gameConfig, "finishDay", 25),
                MinPlayers = ReadInt(gameConfig, "minPlayers", 2),
                MaxPlayers = ReadInt(gameConfig, "maxPlayers", 4),
                CashMin = ReadInt(constraints, "cash_min", 0),
                MaxIngredientTotal = ReadInt(constraints, "max_ingredient_total", 0),
                MaxSameIngredient = ReadInt(constraints, "max_same_ingredient", 0),
                PrimaryNeedMaxPerDay = ReadNullableInt(constraints, "primary_need_max_per_day"),
                RequirePrimaryBeforeOthers = ReadBool(constraints, "require_primary_before_others", true),
                DonationMinAmount = ReadInt(donation, "min_amount", 1),
                DonationMaxAmount = ReadInt(donation, "max_amount", 1),
                GoldTradeAllowBuy = ReadBool(goldTrade, "allow_buy", true),
                GoldTradeAllowSell = ReadBool(goldTrade, "allow_sell", true),
                LoanEnabled = ReadNestedBool(advanced, "loan", "enabled"),
                InsuranceEnabled = ReadNestedBool(advanced, "insurance", "enabled"),
                SavingGoalEnabled = ReadNestedBool(advanced, "saving_goal", "enabled"),
                FreelanceIncome = freelance.ValueKind == JsonValueKind.Object ? ReadInt(freelance, "income", 1) : 1
            },
            PlayerOrdering = new RulesetPlayerOrderingDto
            {
                OrderingCode = ReadString(root, "player_ordering", "PLAYER_ORDER"),
                FridayFeature = ReadNestedWeekdayFeature(weekdayRules, "friday", "FRI", "DONATION"),
                FridayEnabled = ReadNestedWeekdayEnabled(weekdayRules, "friday", "FRI", true),
                SaturdayFeature = ReadNestedWeekdayFeature(weekdayRules, "saturday", "SAT", "GOLD_TRADE"),
                SaturdayEnabled = ReadNestedWeekdayEnabled(weekdayRules, "saturday", "SAT", true),
                SundayFeature = ReadNestedWeekdayFeature(weekdayRules, "sunday", "SUN", "REST"),
                SundayEnabled = ReadNestedWeekdayEnabled(weekdayRules, "sunday", "SUN", true)
            },
            Ingredients = ReadIngredients(componentCatalog),
            Orders = ReadOrders(componentCatalog),
            Needs = ReadNeeds(componentCatalog),
            CollectionMissions = ReadCollectionMissions(componentCatalog),
            FinancialGoals = ReadFinancialGoals(componentCatalog),
            Narratives = ReadNarratives(componentCatalog),
            DonationRankPoints = ReadRankPoints(scoring, "donation_rank_points").Select(x => new RulesetDonationRankPointDto { Rank = x.Rank, Points = x.Points }).ToList(),
            GoldPointsByQty = ReadQtyPoints(scoring, "gold_points_by_qty").Select(x => new RulesetGoldPointDto { Qty = x.Qty, Points = x.Points }).ToList(),
            PensionRankPoints = ReadRankPoints(scoring, "pension_rank_points").Select(x => new RulesetPensionRankPointDto { Rank = x.Rank, Points = x.Points }).ToList(),
            NeedSetBonuses = ReadNeedSetBonuses(root),
            GoldPrices = ReadGoldPrices(root),
            TieBreakers = ReadTieBreakers(root),
            ShariaLoans = ReadShariaLoans(root),
            InsuranceProducts = ReadInsuranceProducts(root),
            LifeRisks = ReadLifeRisks(root)
        };
    }

    public static string ToConfigJson(RulesetDefinitionDto definition)
    {
        var root = new JsonObject
        {
            ["mode"] = definition.Mode,
            ["actions_per_turn"] = definition.Settings.ActionsPerTurn,
            ["starting_cash"] = definition.Settings.StartingCash,
            ["player_ordering"] = definition.PlayerOrdering.OrderingCode,
            ["weekday_rules"] = new JsonObject
            {
                ["friday"] = new JsonObject
                {
                    ["feature"] = definition.PlayerOrdering.FridayFeature,
                    ["enabled"] = definition.PlayerOrdering.FridayEnabled
                },
                ["saturday"] = new JsonObject
                {
                    ["feature"] = definition.PlayerOrdering.SaturdayFeature,
                    ["enabled"] = definition.PlayerOrdering.SaturdayEnabled
                },
                ["sunday"] = new JsonObject
                {
                    ["feature"] = definition.PlayerOrdering.SundayFeature,
                    ["enabled"] = definition.PlayerOrdering.SundayEnabled
                }
            },
            ["constraints"] = new JsonObject
            {
                ["cash_min"] = definition.Settings.CashMin,
                ["max_ingredient_total"] = definition.Settings.MaxIngredientTotal,
                ["max_same_ingredient"] = definition.Settings.MaxSameIngredient,
                ["primary_need_max_per_day"] = definition.Settings.PrimaryNeedMaxPerDay.HasValue
                    ? JsonValue.Create(definition.Settings.PrimaryNeedMaxPerDay.Value)
                    : null,
                ["require_primary_before_others"] = definition.Settings.RequirePrimaryBeforeOthers
            },
            ["donation"] = new JsonObject
            {
                ["min_amount"] = definition.Settings.DonationMinAmount,
                ["max_amount"] = definition.Settings.DonationMaxAmount
            },
            ["gold_trade"] = new JsonObject
            {
                ["allow_buy"] = definition.Settings.GoldTradeAllowBuy,
                ["allow_sell"] = definition.Settings.GoldTradeAllowSell
            },
            ["advanced"] = new JsonObject
            {
                ["loan"] = new JsonObject { ["enabled"] = definition.Settings.LoanEnabled },
                ["insurance"] = new JsonObject { ["enabled"] = definition.Settings.InsuranceEnabled },
                ["saving_goal"] = new JsonObject { ["enabled"] = definition.Settings.SavingGoalEnabled }
            },
            ["freelance"] = new JsonObject
            {
                ["income"] = definition.Settings.FreelanceIncome
            },
            ["scoring"] = new JsonObject
            {
                ["donation_rank_points"] = new JsonArray(definition.DonationRankPoints
                    .Select(item => (JsonNode)new JsonObject
                    {
                        ["rank"] = item.Rank,
                        ["points"] = item.Points
                    }).ToArray()),
                ["gold_points_by_qty"] = new JsonArray(definition.GoldPointsByQty
                    .Select(item => (JsonNode)new JsonObject
                    {
                        ["qty"] = item.Qty,
                        ["points"] = item.Points
                    }).ToArray()),
                ["pension_rank_points"] = new JsonArray(definition.PensionRankPoints
                    .Select(item => (JsonNode)new JsonObject
                    {
                        ["rank"] = item.Rank,
                        ["points"] = item.Points
                    }).ToArray())
            },
            ["need_set_bonuses"] = new JsonArray(definition.NeedSetBonuses.Select(item => (JsonNode)new JsonObject
            {
                ["pattern_code"] = item.PatternCode,
                ["required_count"] = item.RequiredCount,
                ["points"] = item.Points
            }).ToArray()),
            ["gold_prices"] = new JsonArray(definition.GoldPrices.Select(item => (JsonNode)new JsonObject
            {
                ["price_code"] = item.PriceCode,
                ["qty"] = item.Qty,
                ["unit_price"] = item.UnitPrice,
                ["card_qty"] = item.CardQty
            }).ToArray()),
            ["tie_breakers"] = new JsonArray(definition.TieBreakers.Select(item => (JsonNode)new JsonObject
            {
                ["tie_breaker_code"] = item.TieBreakerCode,
                ["tie_number"] = item.TieNumber,
                ["card_qty"] = item.CardQty
            }).ToArray()),
            ["sharia_loans"] = new JsonArray(definition.ShariaLoans.Select(item => (JsonNode)new JsonObject
            {
                ["loan_code"] = item.LoanCode,
                ["item_name"] = item.ItemName,
                ["principal"] = item.Principal,
                ["repayment_amount"] = item.RepaymentAmount,
                ["duration_days"] = item.DurationDays,
                ["penalty_points"] = item.PenaltyPoints,
                ["card_qty"] = item.CardQty
            }).ToArray()),
            ["insurance_products"] = new JsonArray(definition.InsuranceProducts.Select(item => (JsonNode)new JsonObject
            {
                ["product_code"] = item.ProductCode,
                ["item_name"] = item.ItemName,
                ["premium"] = item.Premium,
                ["usage_limit"] = item.UsageLimit,
                ["card_qty"] = item.CardQty
            }).ToArray()),
            ["life_risks"] = new JsonArray(definition.LifeRisks.Select(item => (JsonNode)new JsonObject
            {
                ["risk_code"] = item.RiskCode,
                ["item_name"] = item.ItemName,
                ["effect_type"] = item.EffectType,
                ["direction"] = item.Direction,
                ["amount"] = item.Amount,
                ["target_scope"] = item.TargetScope,
                ["value_delta"] = item.ValueDelta,
                ["duration_days"] = item.DurationDays,
                ["card_qty"] = item.CardQty
            }).ToArray()),
            ["component_catalog"] = new JsonObject
            {
                ["gameConfig"] = new JsonObject
                {
                    ["initialCoins"] = definition.Settings.InitialCoins,
                    ["initialHappiness"] = definition.Settings.InitialHappiness,
                    ["initialSaving"] = definition.Settings.InitialSaving,
                    ["actionsPerTurn"] = definition.Settings.ActionsPerTurn,
                    ["finishDay"] = definition.Settings.FinishDay,
                    ["minPlayers"] = definition.Settings.MinPlayers,
                    ["maxPlayers"] = definition.Settings.MaxPlayers
                },
                ["bahan"] = new JsonArray(definition.Ingredients.Select(item => (JsonNode)new JsonObject
                {
                    ["id"] = item.Id,
                    ["nama"] = item.Nama,
                    ["hargaBeli"] = item.HargaBeli,
                    ["cardQty"] = item.CardQty
                }).ToArray()),
                ["resep"] = new JsonArray(definition.Orders.Select(item => (JsonNode)new JsonObject
                {
                    ["id"] = item.Id,
                    ["nama"] = item.Nama,
                    ["hargaJual"] = item.HargaJual,
                    ["poinKebahagiaan"] = item.PoinKebahagiaan,
                    ["bahan"] = new JsonArray(item.Bahan.Select(name => (JsonNode)name).ToArray()),
                    ["cardQty"] = item.CardQty
                }).ToArray()),
                ["kebutuhan"] = new JsonArray(definition.Needs.Select(item => (JsonNode)new JsonObject
                {
                    ["id"] = item.Id,
                    ["nama"] = item.Nama,
                    ["family"] = string.IsNullOrWhiteSpace(item.Family) ? item.Id : item.Family,
                    ["tipe"] = item.Tipe,
                    ["hargaBeli"] = item.HargaBeli,
                    ["poinKebahagiaan"] = item.PoinKebahagiaan,
                    ["cardQty"] = item.CardQty
                }).ToArray()),
                ["targetKebutuhan"] = new JsonArray(definition.CollectionMissions.Select(item => (JsonNode)new JsonObject
                {
                    ["id"] = item.Id,
                    ["nama"] = item.Nama,
                    ["success_points"] = item.SuccessPoints,
                    ["failure_points"] = item.FailurePoints,
                    ["penaltyPoints"] = item.PenaltyPoints,
                    ["kebutuhanTarget"] = new JsonArray(item.KebutuhanTarget.Select(requirement => (JsonNode)new JsonObject
                    {
                        ["order"] = requirement.Order,
                        ["type"] = requirement.Type,
                        ["value"] = requirement.Value
                    }).ToArray())
                }).ToArray()),
                ["tujuanFinansial"] = new JsonArray(definition.FinancialGoals.Select(item => (JsonNode)new JsonObject
                {
                    ["id"] = item.Id,
                    ["nama"] = item.Nama,
                    ["hargaBeli"] = item.HargaBeli,
                    ["poinKebahagiaan"] = item.PoinKebahagiaan,
                    ["cardQty"] = item.CardQty
                }).ToArray()),
                ["narasi"] = new JsonArray(definition.Narratives.Select(item => (JsonNode)new JsonObject
                {
                    ["id"] = item.Id,
                    ["nama"] = item.Nama,
                    ["teks"] = new JsonArray(item.Teks.Select(text => (JsonNode)text).ToArray()),
                    ["prerequisiteAksi"] = new JsonArray(item.PrerequisiteAksi.Select(prereq => (JsonNode)new JsonObject
                    {
                        ["aksi"] = prereq.Aksi,
                        ["value"] = prereq.Value
                    }).ToArray())
                }).ToArray())
            }
        };

        return root.ToJsonString(new JsonSerializerOptions
        {
            WriteIndented = false
        });
    }

    private static List<RulesetIngredientDto> ReadIngredients(JsonElement componentCatalog)
    {
        return ReadArray(componentCatalog, "bahan", item => new RulesetIngredientDto
        {
            Id = ReadString(item, "id", string.Empty),
            Nama = ReadString(item, "nama", string.Empty),
            HargaBeli = ReadInt(item, "hargaBeli", 0),
            CardQty = ReadNullableInt(item, "cardQty")
        });
    }

    private static List<RulesetOrderDto> ReadOrders(JsonElement componentCatalog)
    {
        return ReadArray(componentCatalog, "resep", item => new RulesetOrderDto
        {
            Id = ReadString(item, "id", string.Empty),
            Nama = ReadString(item, "nama", string.Empty),
            HargaJual = ReadInt(item, "hargaJual", 0),
            PoinKebahagiaan = ReadInt(item, "poinKebahagiaan", 0),
            Bahan = item.TryGetProperty("bahan", out var bahan) && bahan.ValueKind == JsonValueKind.Array
                ? bahan.EnumerateArray().SelectMany(ReadOrderIngredientCodes).ToList()
                : [],
            CardQty = item.TryGetProperty("cardQty", out var cq) && cq.ValueKind == JsonValueKind.Number ? cq.GetInt32() : (int?)null
        });
    }

    private static IEnumerable<string> ReadOrderIngredientCodes(JsonElement item)
    {
        if (item.ValueKind == JsonValueKind.String)
        {
            var value = item.GetString();
            if (!string.IsNullOrWhiteSpace(value))
            {
                yield return value;
            }

            yield break;
        }

        if (item.ValueKind != JsonValueKind.Object)
        {
            yield break;
        }

        var code = ReadString(item, "ingredientCode", string.Empty);
        if (string.IsNullOrWhiteSpace(code))
        {
            yield break;
        }

        var qty = Math.Max(1, ReadInt(item, "qty", 1));
        for (var i = 0; i < qty; i++)
        {
            yield return code;
        }
    }

    private static List<RulesetNeedDto> ReadNeeds(JsonElement componentCatalog)
    {
        return ReadArray(componentCatalog, "kebutuhan", item => new RulesetNeedDto
        {
            Id = ReadString(item, "id", string.Empty),
            Nama = ReadString(item, "nama", string.Empty),
            Family = ReadString(item, "family", string.Empty),
            Tipe = ReadString(item, "tipe", string.Empty),
            HargaBeli = ReadInt(item, "hargaBeli", 0),
            PoinKebahagiaan = ReadInt(item, "poinKebahagiaan", 0),
            CardQty = ReadNullableInt(item, "cardQty")
        });
    }

    private static List<RulesetCollectionMissionDto> ReadCollectionMissions(JsonElement componentCatalog)
    {
        return ReadArray(componentCatalog, "targetKebutuhan", item => new RulesetCollectionMissionDto
        {
            Id = ReadString(item, "id", string.Empty),
            Nama = ReadString(item, "nama", string.Empty),
            SuccessPoints = ReadInt(item, "success_points", 0),
            FailurePoints = ReadInt(item, "failure_points", 0),
            PenaltyPoints = ReadInt(item, "penaltyPoints", 0),
            KebutuhanTarget = item.TryGetProperty("kebutuhanTarget", out var requirements) && requirements.ValueKind == JsonValueKind.Array
                ? requirements.EnumerateArray().Select(requirement => new RulesetCollectionMissionRequirementDto
                {
                    Order = ReadInt(requirement, "order", 0),
                    Type = ReadString(requirement, "type", string.Empty),
                    Value = ReadString(requirement, "value", string.Empty)
                }).ToList()
                : []
        });
    }

    private static List<RulesetFinancialGoalDto> ReadFinancialGoals(JsonElement componentCatalog)
    {
        return ReadArray(componentCatalog, "tujuanFinansial", item => new RulesetFinancialGoalDto
        {
            Id = ReadString(item, "id", string.Empty),
            Nama = ReadString(item, "nama", string.Empty),
            HargaBeli = ReadInt(item, "hargaBeli", 0),
            PoinKebahagiaan = ReadInt(item, "poinKebahagiaan", 0),
            CardQty = ReadNullableInt(item, "cardQty")
        });
    }

    private static List<RulesetNarrativeDto> ReadNarratives(JsonElement componentCatalog)
    {
        return ReadArray(componentCatalog, "narasi", item => new RulesetNarrativeDto
        {
            Id = ReadString(item, "id", string.Empty),
            Nama = ReadString(item, "nama", string.Empty),
            Teks = item.TryGetProperty("teks", out var teks) && teks.ValueKind == JsonValueKind.Array
                ? teks.EnumerateArray().Where(x => x.ValueKind == JsonValueKind.String).Select(x => x.GetString() ?? string.Empty).ToList()
                : [],
            PrerequisiteAksi = item.TryGetProperty("prerequisiteAksi", out var prerequisites) && prerequisites.ValueKind == JsonValueKind.Array
                ? prerequisites.EnumerateArray().Select(prerequisite => new RulesetNarrativePrerequisiteDto
                {
                    Aksi = ReadString(prerequisite, "aksi", string.Empty),
                    Value = ReadInt(prerequisite, "value", 0)
                }).ToList()
                : []
        });
    }

    private static List<RulesetNeedSetBonusDto> ReadNeedSetBonuses(JsonElement root)
    {
        return ReadArray(root, "need_set_bonuses", item => new RulesetNeedSetBonusDto
        {
            PatternCode = ReadString(item, "pattern_code", string.Empty),
            RequiredCount = ReadInt(item, "required_count", 0),
            Points = ReadInt(item, "points", 0)
        });
    }

    private static List<RulesetGoldPriceDto> ReadGoldPrices(JsonElement root)
    {
        return ReadArray(root, "gold_prices", item => new RulesetGoldPriceDto
        {
            PriceCode = ReadString(item, "price_code", string.Empty),
            Qty = ReadInt(item, "qty", 0),
            UnitPrice = ReadInt(item, "unit_price", 0),
            CardQty = ReadNullableInt(item, "card_qty")
        });
    }

    private static List<RulesetTieBreakerDto> ReadTieBreakers(JsonElement root)
    {
        return ReadArray(root, "tie_breakers", item => new RulesetTieBreakerDto
        {
            TieBreakerCode = ReadString(item, "tie_breaker_code", string.Empty),
            TieNumber = ReadInt(item, "tie_number", 0),
            CardQty = ReadNullableInt(item, "card_qty")
        });
    }

    private static List<RulesetShariaLoanDto> ReadShariaLoans(JsonElement root)
    {
        return ReadArray(root, "sharia_loans", item => new RulesetShariaLoanDto
        {
            LoanCode = ReadString(item, "loan_code", string.Empty),
            ItemName = ReadString(item, "item_name", string.Empty),
            Principal = ReadInt(item, "principal", 0),
            RepaymentAmount = ReadInt(item, "repayment_amount", 0),
            DurationDays = ReadInt(item, "duration_days", 0),
            PenaltyPoints = ReadInt(item, "penalty_points", 0),
            CardQty = ReadNullableInt(item, "card_qty")
        });
    }

    private static List<RulesetInsuranceProductDto> ReadInsuranceProducts(JsonElement root)
    {
        return ReadArray(root, "insurance_products", item => new RulesetInsuranceProductDto
        {
            ProductCode = ReadString(item, "product_code", string.Empty),
            ItemName = ReadString(item, "item_name", string.Empty),
            Premium = ReadInt(item, "premium", 0),
            UsageLimit = ReadInt(item, "usage_limit", 0),
            CardQty = ReadNullableInt(item, "card_qty")
        });
    }

    private static List<RulesetLifeRiskDto> ReadLifeRisks(JsonElement root)
    {
        return ReadArray(root, "life_risks", item => new RulesetLifeRiskDto
        {
            RiskCode = ReadString(item, "risk_code", string.Empty),
            ItemName = ReadString(item, "item_name", string.Empty),
            EffectType = ReadString(item, "effect_type", string.Empty),
            Direction = ReadString(item, "direction", string.Empty),
            Amount = ReadInt(item, "amount", 0),
            TargetScope = ReadString(item, "target_scope", "SELF"),
            ValueDelta = ReadNullableInt(item, "value_delta"),
            DurationDays = ReadNullableInt(item, "duration_days"),
            CardQty = ReadNullableInt(item, "card_qty")
        });
    }

    private static List<RankPointShape> ReadRankPoints(JsonElement root, string propertyName)
    {
        return root.ValueKind == JsonValueKind.Object && root.TryGetProperty(propertyName, out var array) && array.ValueKind == JsonValueKind.Array
            ? array.EnumerateArray().Select(item => new RankPointShape(ReadInt(item, "rank", 0), ReadInt(item, "points", 0))).ToList()
            : [];
    }

    private static List<QtyPointShape> ReadQtyPoints(JsonElement root, string propertyName)
    {
        return root.ValueKind == JsonValueKind.Object && root.TryGetProperty(propertyName, out var array) && array.ValueKind == JsonValueKind.Array
            ? array.EnumerateArray().Select(item => new QtyPointShape(ReadInt(item, "qty", 0), ReadInt(item, "points", 0))).ToList()
            : [];
    }

    private static List<T> ReadArray<T>(JsonElement root, string propertyName, Func<JsonElement, T> selector)
    {
        return root.TryGetProperty(propertyName, out var array) && array.ValueKind == JsonValueKind.Array
            ? array.EnumerateArray().Select(selector).ToList()
            : [];
    }

    private static string ReadString(JsonElement root, string propertyName, string fallback)
    {
        return root.TryGetProperty(propertyName, out var property) && property.ValueKind == JsonValueKind.String
            ? property.GetString() ?? fallback
            : fallback;
    }

    private static string? ReadNullableString(JsonElement root, string propertyName)
    {
        return root.TryGetProperty(propertyName, out var property) && property.ValueKind == JsonValueKind.String
            ? property.GetString()
            : null;
    }

    private static int ReadInt(JsonElement root, string propertyName, int fallback)
    {
        return root.TryGetProperty(propertyName, out var property) &&
               property.ValueKind == JsonValueKind.Number &&
               property.TryGetInt32(out var value)
            ? value
            : fallback;
    }

    private static int? ReadNullableInt(JsonElement root, string propertyName)
    {
        return root.TryGetProperty(propertyName, out var property) &&
               property.ValueKind == JsonValueKind.Number &&
               property.TryGetInt32(out var value)
            ? value
            : null;
    }

    private static bool ReadBool(JsonElement root, string propertyName, bool fallback)
    {
        return root.TryGetProperty(propertyName, out var property) && property.ValueKind is JsonValueKind.True or JsonValueKind.False
            ? property.GetBoolean()
            : fallback;
    }

    private static bool ReadNestedBool(JsonElement root, string propertyName, string nestedPropertyName)
    {
        return root.TryGetProperty(propertyName, out var property) &&
               property.ValueKind == JsonValueKind.Object &&
               property.TryGetProperty(nestedPropertyName, out var nested) &&
               nested.ValueKind is JsonValueKind.True or JsonValueKind.False &&
               nested.GetBoolean();
    }

    private static string ReadNestedWeekdayFeature(JsonElement weekdayRules, string primaryName, string fallbackName, string fallback)
    {
        return TryGetWeekdayProperty(weekdayRules, primaryName, "feature")
            ?? TryGetWeekdayProperty(weekdayRules, fallbackName, "feature")
            ?? fallback;
    }

    private static bool ReadNestedWeekdayEnabled(JsonElement weekdayRules, string primaryName, string fallbackName, bool fallback)
    {
        return TryGetWeekdayPropertyBool(weekdayRules, primaryName, "enabled")
            ?? TryGetWeekdayPropertyBool(weekdayRules, fallbackName, "enabled")
            ?? fallback;
    }

    private static string? TryGetWeekdayProperty(JsonElement weekdayRules, string weekdayName, string propertyName)
    {
        return weekdayRules.TryGetProperty(weekdayName, out var weekday) &&
               weekday.ValueKind == JsonValueKind.Object &&
               weekday.TryGetProperty(propertyName, out var property) &&
               property.ValueKind == JsonValueKind.String
            ? property.GetString()
            : null;
    }

    private static bool? TryGetWeekdayPropertyBool(JsonElement weekdayRules, string weekdayName, string propertyName)
    {
        return weekdayRules.TryGetProperty(weekdayName, out var weekday) &&
               weekday.ValueKind == JsonValueKind.Object &&
               weekday.TryGetProperty(propertyName, out var property) &&
               property.ValueKind is JsonValueKind.True or JsonValueKind.False
            ? property.GetBoolean()
            : null;
    }

    private static JsonElement ReadPayload(JsonElement script)
    {
        if (script.TryGetProperty("payload", out var payload) || script.TryGetProperty("payload_json", out payload))
        {
            return payload.Clone();
        }

        using var empty = JsonDocument.Parse("{}");
        return empty.RootElement.Clone();
    }

    private static JsonNode CloneJson(JsonElement payload)
    {
        return payload.ValueKind is JsonValueKind.Undefined or JsonValueKind.Null
            ? new JsonObject()
            : JsonNode.Parse(payload.GetRawText()) ?? new JsonObject();
    }

    private sealed record RankPointShape(int Rank, int Points);
    private sealed record QtyPointShape(int Qty, int Points);
}
