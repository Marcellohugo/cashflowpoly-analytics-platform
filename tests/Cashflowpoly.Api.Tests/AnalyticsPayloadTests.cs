using System.Text.Json;
using Xunit;

namespace Cashflowpoly.Api.Tests;

public class AnalyticsPayloadTests
{
    [Fact]
    public void Parse_transaction_payload_succeeds()
    {
        var json = """{ "direction": "IN", "amount": 5, "category": "NEED_PRIMARY" }""";
        using var doc = JsonDocument.Parse(json);
        Assert.True(doc.RootElement.TryGetProperty("direction", out _));
        Assert.True(doc.RootElement.TryGetProperty("amount", out _));
        Assert.True(doc.RootElement.TryGetProperty("category", out _));
    }

    [Fact]
    public void Parse_mission_payload_succeeds()
    {
        var json = """{ "mission_id": "MIS-001", "target_tertiary_card_id": "NEED-T-003", "penalty_points": 10 }""";
        using var doc = JsonDocument.Parse(json);
        Assert.True(doc.RootElement.TryGetProperty("mission_id", out _));
        Assert.True(doc.RootElement.TryGetProperty("target_tertiary_card_id", out _));
        Assert.True(doc.RootElement.TryGetProperty("penalty_points", out _));
    }

    [Fact]
    public void Parse_saving_goal_payload_succeeds()
    {
        var json = """{ "goal_id": "GOAL-001", "points": 5, "cost": 15 }""";
        using var doc = JsonDocument.Parse(json);
        Assert.True(doc.RootElement.TryGetProperty("goal_id", out _));
        Assert.True(doc.RootElement.TryGetProperty("points", out _));
    }
}
