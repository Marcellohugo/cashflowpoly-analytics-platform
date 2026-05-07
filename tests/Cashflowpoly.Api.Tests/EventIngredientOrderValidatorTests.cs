// Fungsi file: Menguji validasi ingredient dan order berbasis inventory histori event.
using System.Text.Json;
using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Domain;
using Cashflowpoly.Contracts;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Cashflowpoly.Api.Tests;

public sealed class EventIngredientOrderValidatorTests
{
    [Fact]
    public void TryValidate_ReturnsFalseForUnhandledAction()
    {
        var request = CreateRequest("transaction.recorded", """{"amount":1}""", Guid.NewGuid());

        var handled = new EventIngredientOrderValidator().TryValidate(request, CreateConfig(), Array.Empty<EventDb>(), out var result);

        Assert.False(handled);
        Assert.True(result.Validation.IsValid);
        Assert.Null(result.OutgoingAmount);
    }

    [Fact]
    public void TryValidatePurchase_RejectsIngredientTotalLimit()
    {
        var playerId = Guid.NewGuid();
        var request = CreateRequest("ingredient.purchased", """{"card_id":"flour","amount":2}""", playerId);
        var history = new[]
        {
            CreateEvent("ingredient.purchased", """{"card_id":"egg","amount":2}""", playerId)
        };

        var handled = new EventIngredientOrderValidator().TryValidate(request, CreateConfig(maxIngredientTotal: 3), history, out var result);

        Assert.True(handled);
        Assert.False(result.Validation.IsValid);
        Assert.Equal(StatusCodes.Status422UnprocessableEntity, result.Validation.StatusCode);
        Assert.Equal("Total kartu bahan melebihi batas ruleset", result.Validation.Message);
        Assert.Null(result.OutgoingAmount);
    }

    [Fact]
    public void TryValidateDiscard_RejectsDiscardAboveStock()
    {
        var playerId = Guid.NewGuid();
        var request = CreateRequest("ingredient.discarded", """{"card_id":"flour","amount":2}""", playerId);
        var history = new[]
        {
            CreateEvent("ingredient.purchased", """{"card_id":"flour","amount":1}""", playerId)
        };

        var handled = new EventIngredientOrderValidator().TryValidate(request, CreateConfig(), history, out var result);

        Assert.True(handled);
        Assert.False(result.Validation.IsValid);
        Assert.Equal("Jumlah discard melebihi stok bahan", result.Validation.Message);
    }

    [Fact]
    public void TryValidateOrderClaim_ReturnsValidWhenInventoryCoversRequiredCards()
    {
        var playerId = Guid.NewGuid();
        var request = CreateRequest("order.claimed", """{"required_ingredient_card_ids":["flour","egg"],"income":8}""", playerId);
        var history = new[]
        {
            CreateEvent("ingredient.purchased", """{"card_id":"flour","amount":1}""", playerId),
            CreateEvent("ingredient.purchased", """{"card_id":"egg","amount":1}""", playerId)
        };

        var handled = new EventIngredientOrderValidator().TryValidate(request, CreateConfig(), history, out var result);

        Assert.True(handled);
        Assert.True(result.Validation.IsValid);
        Assert.Null(result.OutgoingAmount);
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

    private static RulesetConfig CreateConfig(int maxIngredientTotal = 10, int maxSameIngredient = 5)
    {
        return new RulesetConfig(
            "PEMULA",
            ActionsPerTurn: 3,
            StartingCash: 20,
            PlayerOrdering.JoinOrder,
            CashMin: 0,
            maxIngredientTotal,
            maxSameIngredient,
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
            Scoring: null);
    }
}
