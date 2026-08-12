// Fungsi file: Memverifikasi perilaku API, database, atau domain melalui EventNeedPurchaseValidatorTests.
using System.Text.Json;
using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Domain;
using Cashflowpoly.Api.Contracts;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Cashflowpoly.Api.Tests;

public sealed class EventNeedPurchaseValidatorTests
{
    [Fact]
    public void TryValidate_ReturnsFalseForUnhandledAction()
    {
        var request = CreateRequest("CatatTransaksi", """{"amount":1}""");

        var handled = new EventNeedPurchaseValidator().TryValidate(request, CreateConfig(), Array.Empty<EventDb>(), out var result);

        Assert.False(handled);
        Assert.True(result.Validation.IsValid);
        Assert.Null(result.OutgoingAmount);
    }

    [Fact]
    public void TryValidatePrimary_RejectsDailyLimit()
    {
        var playerId = Guid.NewGuid();
        var request = CreateRequest("Kebutuhan", """{"card_id":"rice","amount":5,"points":2}""", playerId);
        var history = new[]
        {
            CreateEvent("Kebutuhan", """{"card_id":"water","amount":3,"points":1}""", playerId, dayIndex: 0)
        };

        var handled = new EventNeedPurchaseValidator().TryValidate(request, CreateConfig(primaryNeedMaxPerDay: 1), history, out var result);

        Assert.True(handled);
        Assert.False(result.Validation.IsValid);
        Assert.Equal(StatusCodes.Status422UnprocessableEntity, result.Validation.StatusCode);
        Assert.Equal("Pembelian kebutuhan primer melebihi batas harian", result.Validation.Message);
        Assert.Null(result.OutgoingAmount);
    }

    [Fact]
    public void TryValidateSecondary_RequiresPrimaryWhenConfigured()
    {
        var playerId = Guid.NewGuid();
        var request = CreateRequest("Kebutuhan", """{"card_id":"book","amount":4,"points":1}""", playerId);

        var handled = new EventNeedPurchaseValidator().TryValidate(request, CreateConfig(requirePrimaryBeforeOthers: true), Array.Empty<EventDb>(), out var result);

        Assert.True(handled);
        Assert.False(result.Validation.IsValid);
        Assert.Equal(StatusCodes.Status422UnprocessableEntity, result.Validation.StatusCode);
        Assert.Equal("Kebutuhan primer harus dibeli terlebih dahulu", result.Validation.Message);
    }

    [Fact]
    public void TryValidateTertiary_ReturnsOutgoingAmountWhenValid()
    {
        var playerId = Guid.NewGuid();
        var request = CreateRequest("Kebutuhan", """{"card_id":"bike","amount":7,"points":3}""", playerId);
        var history = new[]
        {
            CreateEvent("Kebutuhan", """{"card_id":"rice","amount":3,"points":1}""", playerId, dayIndex: 0)
        };

        var handled = new EventNeedPurchaseValidator().TryValidate(request, CreateConfig(requirePrimaryBeforeOthers: true), history, out var result);

        Assert.True(handled);
        Assert.True(result.Validation.IsValid);
        Assert.Equal(7, result.OutgoingAmount);
    }

    private static EventRequest CreateRequest(string actionType, string payloadJson, Guid? playerId = null)
    {
        using var document = JsonDocument.Parse(payloadJson);
        return new EventRequest(
            Guid.NewGuid(),
            Guid.NewGuid(),
            playerId,
            playerId.HasValue ? "PLAYER" : "SYSTEM",
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

    private static EventDb CreateEvent(string actionType, string payloadJson, Guid playerId, int dayIndex)
    {
        return new EventDb
        {
            EventId = Guid.NewGuid(),
            SessionId = Guid.NewGuid(),
            UserId = playerId,
            ActorType = "PLAYER",
            Timestamp = new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero),
            DayIndex = dayIndex,
            Weekday = "MON",
            ActionSlot = 1,
            SequenceNumber = 1,
            ActionType = actionType,
            RulesetVersionId = Guid.NewGuid(),
            Payload = payloadJson
        };
    }

    private static RulesetConfig CreateConfig(
        int primaryNeedMaxPerDay = 1,
        bool requirePrimaryBeforeOthers = false)
    {
        return new RulesetConfig(
            "PEMULA",
            ActionsPerTurn: 3,
            StartingCash: 20,
            PlayerOrdering.PlayerOrder,
            CashMin: 0,
            MaxIngredientTotal: 10,
            MaxSameIngredient: 5,
            primaryNeedMaxPerDay,
            requirePrimaryBeforeOthers,
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
