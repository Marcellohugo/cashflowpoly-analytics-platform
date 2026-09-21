// Fungsi file: Menguji konsistensi biaya bahan, saldo awal, peringkat pensiun, dan misi koleksi.
using System.Text.Json;
using Cashflowpoly.Api.Contracts;
using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Domain;
using Xunit;

namespace Cashflowpoly.Api.Tests;

public sealed class AnalyticsAuditRegressionTests
{
    [Fact]
    public void InitialHappinessIncludesRosterPlayersWithoutEvents()
    {
        var player = Guid.NewGuid();
        var result = new HappinessCalculator().ComputeByPlayer([], [], Config() with { InitialHappiness = 20 }, [player]);
        Assert.Equal(20, result[player].Total);
        Assert.Equal(20, result[player].InitialHappinessPoints);
        Assert.Equal(0, result[player].MissionRewardPoints);
    }

    [Fact]
    public void DiscardedFreeIngredientsDoNotDiscountLaterOrders()
    {
        var events = new List<EventDb>
        {
            Event("SetupBahanAwal", """{"card_id":"flour","amount":0}""", 1),
            Event("BuangBahanMasakan", """{"card_id":"flour","amount":1}""", 2),
            Event("BahanMasakan", """{"card_id":"flour","amount":1}""", 3),
            Event("BahanMasakan", """{"card_id":"egg","amount":1}""", 4),
            Event("JualMasakan", """{"required_ingredient_card_ids":["flour","egg"],"income":4}""", 5)
        };

        var result = new IngredientMealCalculator().Compute(events, []);

        Assert.Equal(2, result.EssentialIngredientExpenses);
        Assert.Equal(0, result.Inventory.Total);
        Assert.Equal(1, result.IngredientsWasted);
        Assert.Equal(50, (result.MealOrderIncomeTotal - result.EssentialIngredientExpenses) / result.MealOrderIncomeTotal * 100);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(20)]
    public void InitialSavingIsIncludedOnceInSavingsPensionAndLiquidAssets(int initialSaving)
    {
        var config = Config() with { InitialSaving = initialSaving };
        List<EventDb> events = [Event("SetupBahanAwal", """{"card_id":"flour","amount":0}""", 1)];
        var happiness = new HappinessCalculator().ComputeBreakdown(events, 0, 0, 0);
        var snapshot = new GameplaySnapshotBuilder().Build(events, [], events, config, happiness);
        using var raw = JsonDocument.Parse(snapshot.RawJson);
        using var derived = JsonDocument.Parse(snapshot.DerivedJson);

        Assert.Equal(initialSaving, raw.RootElement.GetProperty("coins").GetProperty("coins_saved").GetInt32());
        Assert.Equal(51 + initialSaving, raw.RootElement.GetProperty("pension").GetProperty("pension_fund_total").GetInt32());
        Assert.Equal(50 + initialSaving, derived.RootElement.GetProperty("loan_burden_components").GetProperty("liquid_assets").GetDouble());

        events.Add(Event("Menabung", """{"goal_id":"a","amount":10}""", 2));
        events.Add(Event("TarikTabungan", """{"goal_id":"a","amount":3}""", 3));
        events.Add(Event("TujuanFinansial", """{"goal_id":"a","cost":5,"points":1}""", 4));
        Assert.Equal(initialSaving + 2, new SavingGoalCalculator().Compute(events, initialSaving: initialSaving).CoinsSaved);
        Assert.Equal(13, new SavingGoalCalculator().Compute(
            [Event("TarikTabungan", """{"goal_id":"a","amount":7}""", 1)], initialSaving: 20).CoinsSaved);
    }

    [Fact]
    public void PensionIncludesParticipantsWithoutCashProjectionsOrEvents()
    {
        var untouched = Guid.NewGuid();
        var spender = Guid.NewGuid();
        var noEvents = Guid.NewGuid();
        List<EventDb> events =
        [
            Event("SetupBahanAwal", """{"card_id":"flour","amount":0}""", 1, untouched),
            Event("Kebutuhan", """{"card_id":"buku","amount":2,"points":1}""", 2, spender)
        ];
        List<CashflowProjectionDb> projections = [new() { UserId = spender, Direction = "OUT", Amount = 2 }];
        var config = Config() with
        {
            Scoring = new RulesetScoringConfig([], [], [new RankPoint(1, 10), new RankPoint(2, 5), new RankPoint(3, 2)])
        };
        var calculator = new HappinessCalculator();

        var ranks = calculator.ComputePensionRanks(events, projections, config, [untouched, spender, noEvents]);
        var points = calculator.ComputeByPlayer(events, projections, config, [untouched, spender, noEvents]);

        Assert.Equal(1, ranks[untouched]); // 51
        Assert.Equal(2, ranks[noEvents]); // 50
        Assert.Equal(3, ranks[spender]); // 48
        Assert.Equal(5, points[noEvents].PensionPoints);
    }

    [Theory]
    [InlineData("ASSET", "buku", "buku", "primer")]
    [InlineData("NAME", "buku", "buku", "primer")]
    [InlineData("TIER", "primer", "custom_card", "primer")]
    [InlineData("NEED_TIER", "sekunder", "custom_card", "sekunder")]
    [InlineData("NEED_TIER", "tersier", "custom_card", "tersier")]
    [InlineData("FAMILY", "gameboy", "gameboy_2", "tersier")]
    [InlineData("NEED_FAMILY", "buku", "buku_2", "primer")]
    public void MissionUsesActualRequirementForCompletionAndPenalty(string type, string value, string cardId, string tier)
    {
        var requirement = new RulesetCollectionMissionRequirementDto { Type = type, Value = value };
        var setup = Event("SetupMisiAwal", JsonSerializer.Serialize(new
        {
            mission_id = "custom_mission", target_tertiary_card_id = "Display name is not a target",
            penalty_points = 10, require_primary = true, require_secondary = true,
            requirements = new[] { requirement }
        }), 1);
        var purchase = Event("Kebutuhan", JsonSerializer.Serialize(new { card_id = cardId, need_tier = tier, amount = 3, points = 1 }), 2);
        List<EventDb> events = [setup];
        var calculator = new NeedMissionCalculator();
        var happiness = new HappinessCalculator();
        Assert.False(calculator.Compute(events, []).CollectionMissionComplete);
        Assert.Equal(10, happiness.ComputeBreakdown(events, 0, 0, 0).MissionPenaltyPoints);

        events.Add(purchase);
        Assert.True(calculator.Compute(events, []).CollectionMissionComplete);
        Assert.Equal(0, happiness.ComputeBreakdown(events, 0, 0, 0).MissionPenaltyPoints);

        // Completion is based on purchase history, as for existing mission semantics.
        events.Add(Event("GunakanOpsiDarurat", JsonSerializer.Serialize(new { option_type = "SELL_NEED", card_id = cardId }), 3));
        Assert.True(calculator.Compute(events, []).CollectionMissionComplete);
        Assert.Equal(0, happiness.ComputeBreakdown(events, 0, 0, 0).MissionPenaltyPoints);
    }

    [Fact]
    public void ExistingMissionEventsResolveRequirementsFromLockedRuleset()
    {
        var config = Config() with
        {
            CollectionMissions = [new RulesetCollectionMissionDto
            {
                Id = "custom_mission", Nama = "Display name is not a target", PenaltyPoints = 10,
                KebutuhanTarget = [new() { Type = "ASSET", Value = "buku" }, new() { Type = "NEED_TIER", Value = "sekunder" }]
            }]
        };
        var playerId = Guid.NewGuid();
        List<EventDb> events =
        [
            Event("SetupMisiAwal", """{"mission_id":"custom_mission","target_tertiary_card_id":"Display name is not a target","penalty_points":10}""", 1, playerId),
            Event("Kebutuhan", """{"card_id":"buku","need_tier":"primer","amount":3,"points":1}""", 2, playerId)
        ];
        Assert.False(new NeedMissionCalculator().Compute(events, [], config).CollectionMissionComplete);
        events.Add(Event("Kebutuhan", """{"card_id":"sepatu","need_tier":"sekunder","amount":4,"points":2}""", 3, playerId));

        Assert.True(new NeedMissionCalculator().Compute(events, [], config).CollectionMissionComplete);
        Assert.Equal(0, new HappinessCalculator().ComputeByPlayer(events, [], config)[playerId].MissionPenaltyPoints);
    }

    [Fact]
    public void MissionFamilyUsesCatalogFamilyInsteadOfCardName()
    {
        var config = Config() with
        {
            Needs = [new() { Id = "custom_card_1", Family = "school_supplies", Tipe = "primer" }],
            CollectionMissions = [new()
            {
                Id = "school", KebutuhanTarget = [new() { Type = "FAMILY", Value = "school_supplies" }]
            }]
        };
        List<EventDb> events =
        [
            Event("SetupMisiAwal", """{"mission_id":"school","target_tertiary_card_id":"","penalty_points":10}""", 1),
            Event("Kebutuhan", """{"card_id":"custom_card_1","amount":3,"points":1}""", 2)
        ];
        Assert.True(new NeedMissionCalculator().Compute(events, [], config).CollectionMissionComplete);
    }

    private static RulesetConfig Config() => new("MAHIR", 2, 50, PlayerOrdering.PlayerOrder,
        0, 6, 3, 1, true, true, true, true, 1, 999, true, true, true, true, true, 1, null);

    private static EventDb Event(string action, string payload, long sequence, Guid? playerId = null) => new()
    {
        EventId = Guid.NewGuid(), SessionId = Guid.Parse("95000000-0000-0000-0000-000000000001"),
        UserId = playerId ?? Guid.Parse("95000000-0000-0000-0000-000000000002"),
        ActorType = "PLAYER", DayIndex = 1, SequenceNumber = sequence, ActionType = action, Payload = payload
    };
}
