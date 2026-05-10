using System.Text.Json;
using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Domain;
using Cashflowpoly.Contracts;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Cashflowpoly.Api.Tests;

public sealed class EventTurnProgressValidatorTests
{
    [Fact]
    public void RequiresHistory_ReturnsTrueForActionUsed()
    {
        var request = CreateRequest("turn.action.used", """{"used":1,"remaining":2}""");

        var requiresHistory = new EventTurnProgressValidator().RequiresHistory(request, CreateConfig());

        Assert.True(requiresHistory);
    }

    [Fact]
    public void TryValidateActionUsed_RejectsExceededActionLimit()
    {
        var playerId = Guid.NewGuid();
        var sessionId = Guid.NewGuid();
        var request = CreateRequest("turn.action.used", """{"used":2,"remaining":0}""", sessionId, playerId);
        var history = new[]
        {
            CreateEvent("turn.action.used", """{"used":2,"remaining":1}""", sessionId, playerId, turnNumber: 1)
        };

        var handled = new EventTurnProgressValidator().TryValidate(request, CreateConfig(actionsPerTurn: 3), history, out var result);

        Assert.True(handled);
        Assert.False(result.IsValid);
        Assert.Equal(StatusCodes.Status422UnprocessableEntity, result.StatusCode);
        Assert.Equal("DOMAIN_RULE_VIOLATION", result.ErrorCode);
        Assert.Equal("Jumlah token aksi melebihi batas ruleset", result.Message);
    }

    [Fact]
    public void TryValidateActionUsed_AcceptsUsageWithinLimit()
    {
        var playerId = Guid.NewGuid();
        var sessionId = Guid.NewGuid();
        var request = CreateRequest("turn.action.used", """{"used":1,"remaining":1}""", sessionId, playerId);
        var history = new[]
        {
            CreateEvent("turn.action.used", """{"used":1,"remaining":2}""", sessionId, playerId, turnNumber: 1)
        };

        var handled = new EventTurnProgressValidator().TryValidate(request, CreateConfig(actionsPerTurn: 3), history, out var result);

        Assert.True(handled);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void TryValidateTurnEndedMahir_RejectsOrderRiskMismatch()
    {
        var playerId = Guid.NewGuid();
        var sessionId = Guid.NewGuid();
        var request = CreateRequest("turn.ended", "{}", sessionId, playerId);
        var history = new[]
        {
            CreateEvent("order.claimed", """{"required_ingredient_card_ids":["A"],"income":5}""", sessionId, playerId, turnNumber: 1)
        };

        var handled = new EventTurnProgressValidator().TryValidate(request, CreateConfig(mode: "MAHIR"), history, out var result);

        Assert.True(handled);
        Assert.False(result.IsValid);
        Assert.Equal(StatusCodes.Status422UnprocessableEntity, result.StatusCode);
        Assert.Equal("Setiap klaim pesanan harus diikuti pengambilan risiko pada mode MAHIR", result.Message);
    }

    private static EventRequest CreateRequest(
        string actionType,
        string payloadJson,
        Guid? sessionId = null,
        Guid? playerId = null)
    {
        using var document = JsonDocument.Parse(payloadJson);
        return new EventRequest(
            Guid.NewGuid(),
            sessionId ?? Guid.NewGuid(),
            playerId ?? Guid.NewGuid(),
            "PLAYER",
            new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero),
            0,
            "MON",
            1,
            0,
            actionType,
            Guid.NewGuid(),
            document.RootElement.Clone(),
            "client-123");
    }

    private static EventDb CreateEvent(
        string actionType,
        string payloadJson,
        Guid sessionId,
        Guid playerId,
        int turnNumber)
    {
        return new EventDb
        {
            EventId = Guid.NewGuid(),
            SessionId = sessionId,
            PlayerId = playerId,
            ActorType = "PLAYER",
            Timestamp = new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero),
            DayIndex = 0,
            Weekday = "MON",
            TurnNumber = turnNumber,
            SequenceNumber = 0,
            ActionType = actionType,
            RulesetVersionId = Guid.NewGuid(),
            Payload = payloadJson
        };
    }

    private static RulesetConfig CreateConfig(string mode = "PEMULA", int actionsPerTurn = 3)
    {
        return new RulesetConfig(
            mode,
            actionsPerTurn,
            StartingCash: 20,
            PlayerOrdering.JoinOrder,
            CashMin: 0,
            MaxIngredientTotal: 10,
            MaxSameIngredient: 5,
            PrimaryNeedMaxPerDay: 1,
            RequirePrimaryBeforeOthers: true,
            FridayEnabled: true,
            SaturdayEnabled: true,
            SundayEnabled: true,
            DonationMin: 1,
            DonationMax: 10,
            GoldAllowBuy: true,
            GoldAllowSell: true,
            LoanEnabled: mode == "MAHIR",
            InsuranceEnabled: mode == "MAHIR",
            SavingGoalEnabled: mode == "MAHIR",
            FreelanceIncome: 5,
            Scoring: null);
    }
}
