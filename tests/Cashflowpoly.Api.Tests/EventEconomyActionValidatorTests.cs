using System.Text.Json;
using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Domain;
using Cashflowpoly.Api.Contracts;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Cashflowpoly.Api.Tests;

public sealed class EventEconomyActionValidatorTests
{
    [Fact]
    public void TryValidate_TransactionOut_ReturnsOutgoingAmountForBalanceCheck()
    {
        var request = CreateRequest("CatatTransaksi", """{"direction":"OUT","amount":6,"category":"CUSTOM","counterparty":"BANK"}""");

        var handled = new EventEconomyActionValidator().TryValidate(request, CreateConfig(), Array.Empty<EventDb>(), out var result);

        Assert.True(handled);
        Assert.True(result.Validation.IsValid);
        Assert.Equal(6, result.OutgoingAmount);
    }

    [Fact]
    public void TryValidate_DonationRejectsWrongWeekday()
    {
        var request = CreateRequest("JumatBerkah", """{"amount":3}""", weekday: "MON");

        var handled = new EventEconomyActionValidator().TryValidate(request, CreateConfig(), Array.Empty<EventDb>(), out var result);

        Assert.True(handled);
        Assert.False(result.Validation.IsValid);
        Assert.Equal(StatusCodes.Status400BadRequest, result.Validation.StatusCode);
        Assert.Contains(result.Validation.Details, detail => detail.Field == "weekday" && detail.Issue == "INVALID_VALUE");
    }

    [Fact]
    public void TryValidate_DonationRejectsSecondSubmissionOnSameFriday()
    {
        var playerId = Guid.NewGuid();
        var request = CreateRequest("JumatBerkah", """{"amount":3}""", playerId, weekday: "FRI");
        var history = new[] { CreateEvent(playerId, "JumatBerkah", """{"amount":2}""", sessionId: request.SessionId) };

        new EventEconomyActionValidator().TryValidate(request, CreateConfig(), history, out var result);

        Assert.False(result.Validation.IsValid);
        Assert.Equal("DONATION_ALREADY_SUBMITTED", result.Validation.ErrorCode);
    }

    [Fact]
    public void TryValidate_GoldSellLeavesInventoryCheckToRelationalHolding()
    {
        var playerId = Guid.NewGuid();
        var request = CreateRequest(
            "JualEmas",
            """{"trade_type":"SELL","qty":2,"unit_price":5,"amount":10}""",
            playerId,
            weekday: "SAT");
        var history = new[]
        {
            CreateEvent(playerId, "BukaHargaEmas", """{"gold_price":5}""", sessionId: request.SessionId)
        };

        var handled = new EventEconomyActionValidator().TryValidate(request, CreateConfig(), history, out var result);

        Assert.True(handled);
        Assert.True(result.Validation.IsValid);
    }

    [Fact]
    public void TryValidate_GoldTradeAllowsNonSaturdayWhenTriggeredByLifeRisk()
    {
        var riskEventId = Guid.NewGuid();
        var request = CreateRequest(
            "InvestasiEmas",
            $$"""{"trade_type":"BUY","qty":1,"unit_price":5,"amount":5,"risk_event_id":"{{riskEventId}}"}""",
            weekday: "MON");
        var history = new[]
        {
            CreateEvent(request.UserId!.Value, "BukaHargaEmas", """{"gold_price":5}""", sessionId: request.SessionId),
            CreateEvent(
                request.UserId!.Value,
                "RisikoKehidupan",
                """{"risk_id":"risk_gold"}""",
                riskEventId,
                request.SessionId)
        };

        var handled = new EventEconomyActionValidator().TryValidate(request, CreateConfig(), history, out var result);

        Assert.True(handled);
        Assert.True(result.Validation.IsValid);
        Assert.Equal(5, result.OutgoingAmount);
    }

    [Fact]
    public void TryValidate_GoldTradeRejectsPriceFromPreviousSaturday()
    {
        var playerId = Guid.NewGuid();
        var request = CreateRequest(
            "InvestasiEmas",
            """{"trade_type":"BUY","qty":1,"unit_price":5,"amount":5}""",
            playerId,
            weekday: "SAT",
            dayIndex: 13);
        var history = new[]
        {
            CreateEvent(playerId, "BukaHargaEmas", """{"gold_price":5}""", sessionId: request.SessionId, dayIndex: 6)
        };

        new EventEconomyActionValidator().TryValidate(request, CreateConfig(), history, out var result);

        Assert.False(result.Validation.IsValid);
        Assert.Equal(StatusCodes.Status422UnprocessableEntity, result.Validation.StatusCode);
    }

    [Fact]
    public void TryValidate_GoldTradeRejectsFakeRiskReferenceOutsideSaturday()
    {
        var request = CreateRequest(
            "InvestasiEmas",
            $$"""{"trade_type":"BUY","qty":1,"unit_price":5,"amount":5,"risk_event_id":"{{Guid.NewGuid()}}"}""",
            weekday: "MON");

        new EventEconomyActionValidator().TryValidate(request, CreateConfig(), Array.Empty<EventDb>(), out var result);

        Assert.False(result.Validation.IsValid);
        Assert.Equal(StatusCodes.Status400BadRequest, result.Validation.StatusCode);
    }

    private static EventRequest CreateRequest(
        string actionType,
        string payloadJson,
        Guid? playerId = null,
        string weekday = "MON",
        int dayIndex = 0)
    {
        using var document = JsonDocument.Parse(payloadJson);
        return new EventRequest(
            Guid.NewGuid(),
            Guid.NewGuid(),
            playerId ?? Guid.NewGuid(),
            "PLAYER",
            new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero),
            dayIndex,
            weekday,
            1,
            0,
            actionType,
            Guid.NewGuid(),
            document.RootElement.Clone(),
            "client-123");
    }

    private static EventDb CreateEvent(
        Guid playerId,
        string actionType,
        string payload,
        Guid? eventId = null,
        Guid? sessionId = null,
        int dayIndex = 0)
    {
        return new EventDb
        {
            EventId = eventId ?? Guid.NewGuid(),
            SessionId = sessionId ?? Guid.NewGuid(),
            UserId = playerId,
            ActorType = "PLAYER",
            Timestamp = new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero),
            DayIndex = dayIndex,
            Weekday = "SAT",
            ActionSlot = 1,
            SequenceNumber = 1,
            ActionType = actionType,
            RulesetVersionId = Guid.NewGuid(),
            Payload = payload
        };
    }

    private static RulesetConfig CreateConfig()
    {
        return new RulesetConfig(
            "PEMULA",
            ActionsPerTurn: 3,
            StartingCash: 20,
            PlayerOrdering.PlayerOrder,
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
            LoanEnabled: false,
            InsuranceEnabled: false,
            SavingGoalEnabled: false,
            FreelanceIncome: 5,
            Scoring: null)
        {
            LifeRisks =
            [
                new RulesetLifeRiskDto
                {
                    RiskCode = "risk_gold",
                    ItemName = "Gold trade",
                    EffectType = "GOLD_TRADE",
                    Direction = "IN",
                    Amount = 0,
                    DurationDays = 1
                }
            ]
        };
    }
}
