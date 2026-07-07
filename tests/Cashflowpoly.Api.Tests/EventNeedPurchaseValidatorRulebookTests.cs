using System.Text.Json;
using Cashflowpoly.Api.Contracts;
using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Domain;
using Xunit;

namespace Cashflowpoly.Api.Tests;

public sealed class EventNeedPurchaseValidatorRulebookTests
{
    [Fact]
    public void TryValidate_PrimaryNeedMaxPerDayZero_AllowsSecondPrimaryNeedPurchase()
    {
        var playerId = Guid.NewGuid();
        var request = CreateRequest("Kebutuhan", """{"card_id":"buku","amount":2,"points":1,"need_tier":"primer"}""", playerId);
        var history = new[]
        {
            CreateEvent("Kebutuhan", """{"card_id":"buku","amount":2,"points":1,"need_tier":"primer"}""", playerId)
        };

        var handled = new EventNeedPurchaseValidator().TryValidate(
            request,
            CreateConfig(primaryNeedMaxPerDay: 0),
            history,
            out var result);

        Assert.True(handled);
        Assert.True(result.Validation.IsValid);
        Assert.Equal(2, result.OutgoingAmount);
    }

    private static EventRequest CreateRequest(string actionType, string payloadJson, Guid playerId)
    {
        using var document = JsonDocument.Parse(payloadJson);
        return new EventRequest(
            Guid.NewGuid(),
            Guid.NewGuid(),
            playerId,
            "PLAYER",
            new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero),
            2,
            "TUE",
            1,
            2,
            actionType,
            Guid.NewGuid(),
            document.RootElement.Clone(),
            "client-need");
    }

    private static EventDb CreateEvent(string actionType, string payloadJson, Guid playerId)
    {
        return new EventDb
        {
            EventId = Guid.NewGuid(),
            SessionId = Guid.NewGuid(),
            UserId = playerId,
            ActorType = "PLAYER",
            Timestamp = new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero),
            DayIndex = 2,
            Weekday = "TUE",
            ActionSlot = 1,
            SequenceNumber = 1,
            ActionType = actionType,
            RulesetVersionId = Guid.NewGuid(),
            Payload = payloadJson
        };
    }

    private static RulesetConfig CreateConfig(int primaryNeedMaxPerDay)
    {
        return new RulesetConfig(
            "PEMULA",
            ActionsPerTurn: 2,
            StartingCash: 20,
            PlayerOrdering.PlayerOrder,
            CashMin: 0,
            MaxIngredientTotal: 6,
            MaxSameIngredient: 3,
            PrimaryNeedMaxPerDay: primaryNeedMaxPerDay,
            RequirePrimaryBeforeOthers: true,
            FridayEnabled: true,
            SaturdayEnabled: true,
            SundayEnabled: true,
            DonationMin: 1,
            DonationMax: 999999,
            GoldAllowBuy: true,
            GoldAllowSell: true,
            LoanEnabled: false,
            InsuranceEnabled: false,
            SavingGoalEnabled: false,
            FreelanceIncome: 1,
            Scoring: null);
    }
}
