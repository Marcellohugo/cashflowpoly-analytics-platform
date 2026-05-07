// Fungsi file: Menguji validasi event ekonomi sederhana: transaksi, donasi Jumat, dan perdagangan emas.
using System.Text.Json;
using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Domain;
using Cashflowpoly.Contracts;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Cashflowpoly.Api.Tests;

public sealed class EventEconomyActionValidatorTests
{
    [Fact]
    public void TryValidate_TransactionOut_ReturnsOutgoingAmountForBalanceCheck()
    {
        var request = CreateRequest("transaction.recorded", """{"direction":"OUT","amount":6,"category":"CUSTOM","counterparty":"BANK"}""");

        var handled = new EventEconomyActionValidator().TryValidate(request, CreateConfig(), Array.Empty<EventDb>(), out var result);

        Assert.True(handled);
        Assert.True(result.Validation.IsValid);
        Assert.Equal(6, result.OutgoingAmount);
    }

    [Fact]
    public void TryValidate_DonationRejectsWrongWeekday()
    {
        var request = CreateRequest("day.friday.donation", """{"amount":3}""", weekday: "MON");

        var handled = new EventEconomyActionValidator().TryValidate(request, CreateConfig(), Array.Empty<EventDb>(), out var result);

        Assert.True(handled);
        Assert.False(result.Validation.IsValid);
        Assert.Equal(StatusCodes.Status400BadRequest, result.Validation.StatusCode);
        Assert.Contains(result.Validation.Details, detail => detail.Field == "weekday" && detail.Issue == "INVALID_VALUE");
    }

    [Fact]
    public void TryValidate_GoldSellRejectsInsufficientInventory()
    {
        var playerId = Guid.NewGuid();
        var request = CreateRequest(
            "day.saturday.gold_trade",
            """{"trade_type":"SELL","qty":2,"unit_price":5,"amount":10}""",
            playerId,
            weekday: "SAT");
        var history = new[]
        {
            CreateEvent(playerId, "day.saturday.gold_trade", """{"trade_type":"BUY","qty":1,"unit_price":5,"amount":5}""")
        };

        var handled = new EventEconomyActionValidator().TryValidate(request, CreateConfig(), history, out var result);

        Assert.True(handled);
        Assert.False(result.Validation.IsValid);
        Assert.Equal(StatusCodes.Status422UnprocessableEntity, result.Validation.StatusCode);
        Assert.Equal("Kepemilikan emas tidak mencukupi", result.Validation.Message);
    }

    private static EventRequest CreateRequest(
        string actionType,
        string payloadJson,
        Guid? playerId = null,
        string weekday = "MON")
    {
        using var document = JsonDocument.Parse(payloadJson);
        return new EventRequest(
            Guid.NewGuid(),
            Guid.NewGuid(),
            playerId ?? Guid.NewGuid(),
            "PLAYER",
            new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero),
            0,
            weekday,
            1,
            0,
            actionType,
            Guid.NewGuid(),
            document.RootElement.Clone(),
            "client-123");
    }

    private static EventDb CreateEvent(Guid playerId, string actionType, string payload)
    {
        return new EventDb
        {
            EventId = Guid.NewGuid(),
            SessionId = Guid.NewGuid(),
            PlayerId = playerId,
            ActorType = "PLAYER",
            Timestamp = new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero),
            DayIndex = 0,
            Weekday = "SAT",
            TurnNumber = 1,
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
            LoanEnabled: false,
            InsuranceEnabled: false,
            SavingGoalEnabled: false,
            FreelanceIncome: 5,
            Scoring: null);
    }
}
