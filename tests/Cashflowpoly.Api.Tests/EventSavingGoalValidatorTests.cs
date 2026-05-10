using System.Text.Json;
using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Domain;
using Cashflowpoly.Contracts;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Cashflowpoly.Api.Tests;

public sealed class EventSavingGoalValidatorTests
{
    [Fact]
    public void TryValidate_ReturnsFalseForUnhandledAction()
    {
        var request = CreateRequest("transaction.recorded", """{"amount":1}""", Guid.NewGuid());

        var handled = new EventSavingGoalValidator().TryValidate(request, CreateConfig(), Array.Empty<EventDb>(), out var result);

        Assert.False(handled);
        Assert.True(result.Validation.IsValid);
    }

    [Fact]
    public void TryValidateDeposit_RejectsDisabledFeature()
    {
        var request = CreateRequest("saving.deposit.created", """{"goal_id":"goal-a","amount":5}""", Guid.NewGuid());

        var handled = new EventSavingGoalValidator().TryValidate(request, CreateConfig(enabled: false), Array.Empty<EventDb>(), out var result);

        Assert.True(handled);
        Assert.False(result.Validation.IsValid);
        Assert.Equal(StatusCodes.Status422UnprocessableEntity, result.Validation.StatusCode);
        Assert.Equal("Fitur tabungan tujuan tidak aktif", result.Validation.Message);
    }

    [Fact]
    public void TryValidateDeposit_ReturnsOutgoingAmountWhenValid()
    {
        var request = CreateRequest("saving.deposit.created", """{"goal_id":"goal-a","amount":15}""", Guid.NewGuid());

        var handled = new EventSavingGoalValidator().TryValidate(request, CreateConfig(), Array.Empty<EventDb>(), out var result);

        Assert.True(handled);
        Assert.True(result.Validation.IsValid);
        Assert.Equal(15, result.OutgoingAmount);
    }

    [Fact]
    public void TryValidateWithdraw_RejectsInsufficientSavingBalance()
    {
        var playerId = Guid.NewGuid();
        var request = CreateRequest("saving.deposit.withdrawn", """{"goal_id":"goal-a","amount":8}""", playerId);
        var history = new[]
        {
            CreateEvent("saving.deposit.created", """{"goal_id":"goal-a","amount":5}""", playerId)
        };

        var handled = new EventSavingGoalValidator().TryValidate(request, CreateConfig(), history, out var result);

        Assert.True(handled);
        Assert.False(result.Validation.IsValid);
        Assert.Equal("Saldo tabungan tidak mencukupi", result.Validation.Message);
    }

    [Fact]
    public void TryValidateGoalAchieved_RejectsCostAboveSavingBalance()
    {
        var playerId = Guid.NewGuid();
        var request = CreateRequest("saving.goal.achieved", """{"goal_id":"goal-a","points":10,"cost":8}""", playerId);
        var history = new[]
        {
            CreateEvent("saving.deposit.created", """{"goal_id":"goal-a","amount":5}""", playerId)
        };

        var handled = new EventSavingGoalValidator().TryValidate(request, CreateConfig(), history, out var result);

        Assert.True(handled);
        Assert.False(result.Validation.IsValid);
        Assert.Equal("Saldo tabungan tidak mencukupi untuk goal", result.Validation.Message);
    }

    private static EventRequest CreateRequest(string actionType, string payloadJson, Guid? playerId)
    {
        using var document = JsonDocument.Parse(payloadJson);
        return new EventRequest(
            Guid.NewGuid(),
            Guid.NewGuid(),
            playerId,
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

    private static EventDb CreateEvent(string actionType, string payloadJson, Guid playerId)
    {
        return new EventDb
        {
            EventId = Guid.NewGuid(),
            SessionId = Guid.NewGuid(),
            PlayerId = playerId,
            ActorType = "PLAYER",
            Timestamp = new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero),
            DayIndex = 0,
            Weekday = "MON",
            TurnNumber = 1,
            SequenceNumber = 1,
            ActionType = actionType,
            RulesetVersionId = Guid.NewGuid(),
            Payload = payloadJson
        };
    }

    private static RulesetConfig CreateConfig(bool enabled = true)
    {
        return new RulesetConfig(
            "MAHIR",
            ActionsPerTurn: 3,
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
            LoanEnabled: true,
            InsuranceEnabled: true,
            enabled,
            FreelanceIncome: 5,
            Scoring: null);
    }
}
