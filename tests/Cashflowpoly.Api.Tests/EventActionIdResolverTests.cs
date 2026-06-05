using System.Text.Json;
using Cashflowpoly.Api.Data;
using Xunit;

namespace Cashflowpoly.Api.Tests;

public sealed class EventActionIdResolverTests
{
    [Theory]
    [InlineData("ingredient.purchased", """{"card_id":"nasi_putih","amount":1}""", "BahanMasakan")]
    [InlineData("ingredient.discarded", """{"card_id":"nasi_putih","amount":1}""", "ingredient.discarded")]
    [InlineData("order.claimed", """{"required_ingredient_card_ids":["nasi_putih"],"income":15}""", "JualMasakan")]
    [InlineData("order.passed", """{"required_ingredient_card_ids":["nasi_putih"],"income":15}""", "order.passed")]
    [InlineData("work.freelance.completed", """{"amount":5}""", "KerjaLepas")]
    [InlineData("transaction.recorded", """{"direction":"OUT","amount":5,"category":"BANK"}""", "transaction.recorded")]
    [InlineData("mission.assigned", """{"mission_id":"misi_boneka","target_tertiary_card_id":"boneka","penalty_points":10}""", "MissionAssigned")]
    [InlineData("insurance.multirisk.used", """{"risk_event_id":"b9ddcbcb-6e77-4f91-8bb3-eb58fb146001"}""", "Asuransi")]
    [InlineData("saving.deposit.withdrawn", """{"goal_id":"general-saving","amount":5}""", "saving.deposit.withdrawn")]
    [InlineData("risk.emergency.used", """{"risk_event_id":"b9ddcbcb-6e77-4f91-8bb3-eb58fb146001","option_type":"SELL_GOLD","direction":"IN","amount":3}""", "risk.emergency.used")]
    [InlineData("donation.rank.awarded", """{"rank":1,"points":5}""", "donation.rank.awarded")]
    [InlineData("gold.points.awarded", """{"points":3}""", "gold.points.awarded")]
    [InlineData("pension.rank.awarded", """{"rank":2,"points":4}""", "pension.rank.awarded")]
    public void Resolve_ReturnsExpectedActionId_ForMappedActionTypes(
        string actionType,
        string payloadJson,
        string expectedActionId)
    {
        var actionId = EventActionIdResolver.Resolve(actionType, Parse(payloadJson));

        Assert.Equal(expectedActionId, actionId);
    }

    [Theory]
    [InlineData("""{"trade_type":"BUY","qty":1,"unit_price":4,"amount":4}""", "InvestasiEmas")]
    [InlineData("""{"trade_type":"SELL","qty":1,"unit_price":5,"amount":5}""", "JualEmas")]
    public void Resolve_MapsGoldTradeBasedOnTradeType(string payloadJson, string expectedActionId)
    {
        var actionId = EventActionIdResolver.Resolve("day.saturday.gold_trade", Parse(payloadJson));

        Assert.Equal(expectedActionId, actionId);
    }

    [Fact]
    public void Resolve_ReturnsNull_ForUnmappedActionTypes()
    {
        var actionId = EventActionIdResolver.Resolve("unknown.action.type", Parse("""{"amount":5}"""));

        Assert.Null(actionId);
    }

    private static JsonElement Parse(string json)
    {
        using var document = JsonDocument.Parse(json);
        return document.RootElement.Clone();
    }
}
