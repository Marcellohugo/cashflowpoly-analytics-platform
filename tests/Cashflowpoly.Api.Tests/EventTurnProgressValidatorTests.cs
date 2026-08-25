// Fungsi file: Memverifikasi perilaku API, database, atau domain melalui EventTurnProgressValidatorTests.
using System.Text.Json;
using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Domain;
using Cashflowpoly.Api.Contracts;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Cashflowpoly.Api.Tests;

public sealed class EventTurnProgressValidatorTests
{
    [Fact]
    public void RequiresHistory_ReturnsFalseForRemovedActionUsed()
    {
        var request = CreateRequest("turn.action.used", """{"used":1,"remaining":2}""");

        var requiresHistory = new EventTurnProgressValidator().RequiresHistory(request, CreateConfig());

        Assert.False(requiresHistory);
    }

    [Fact]
    public void RequiresHistory_ReturnsTrueForAkhirGiliranMahir()
    {
        var request = CreateRequest("AkhirGiliran", "{}");

        var requiresHistory = new EventTurnProgressValidator().RequiresHistory(request, CreateConfig(mode: "MAHIR"));

        Assert.True(requiresHistory);
    }

    [Fact]
    public void TryValidateTurnEndedMahir_RejectsOrderRiskMismatch()
    {
        var playerId = Guid.NewGuid();
        var sessionId = Guid.NewGuid();
        var request = CreateRequest("AkhirGiliran", "{}", sessionId, playerId);
        var history = new[]
        {
            CreateEvent("JualMasakan", """{"required_ingredient_card_ids":["A"],"income":5}""", sessionId, playerId, actionSlot: 1),
            CreateEvent("BahanMasakan", """{"card_id":"A","amount":1}""", sessionId, playerId, actionSlot: 2)
        };

        var handled = new EventTurnProgressValidator().TryValidate(request, CreateConfig(mode: "MAHIR"), history, 1, out var result);

        Assert.True(handled);
        Assert.False(result.IsValid);
        Assert.Equal(StatusCodes.Status422UnprocessableEntity, result.StatusCode);
        Assert.Equal("Setiap klaim pesanan harus diikuti pengambilan risiko pada mode MAHIR", result.Message);
    }

    [Fact]
    public void TryValidateTurnEndedMahir_AcceptsOrderPairedWithFreeRiskSlot()
    {
        var playerId = Guid.NewGuid();
        var sessionId = Guid.NewGuid();
        var request = CreateRequest("AkhirGiliran", "{}", sessionId, playerId);
        var history = new[]
        {
            CreateEvent("JualMasakan", """{"required_ingredient_card_ids":["A"],"income":5}""", sessionId, playerId, actionSlot: 1),
            CreateEvent("BahanMasakan", """{"card_id":"A","amount":1}""", sessionId, playerId, actionSlot: 2),
            CreateEvent("RisikoKehidupan", """{"risk_id":"risk-a"}""", sessionId, playerId, actionSlot: 0)
        };

        var handled = new EventTurnProgressValidator().TryValidate(request, CreateConfig(mode: "MAHIR"), history, 1, out var result);

        Assert.True(handled);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void TryValidateTurnEnded_RejectsParticipantWithoutActions()
    {
        var playerId = Guid.NewGuid();
        var sessionId = Guid.NewGuid();
        var request = CreateRequest("AkhirGiliran", "{}", sessionId, playerId);
        var history = new[]
        {
            CreateEvent("KerjaLepas", "{}", sessionId, playerId, actionSlot: 1),
            CreateEvent("Menabung", "{}", sessionId, playerId, actionSlot: 2)
        };

        var handled = new EventTurnProgressValidator().TryValidate(request, CreateConfig(), history, 2, out var result);

        Assert.True(handled);
        Assert.False(result.IsValid);
        Assert.Equal("Setiap pemain harus menyelesaikan seluruh jatah aksi sebelum giliran berakhir", result.Message);
    }

    [Fact]
    public void TryValidateTurnEnded_AcceptsTheSameActionInBothSlots()
    {
        var playerId = Guid.NewGuid();
        var sessionId = Guid.NewGuid();
        var request = CreateRequest("AkhirGiliran", "{}", sessionId, playerId);
        var history = new[]
        {
            CreateEvent("KerjaLepas", "{\"amount\":1}", sessionId, playerId, actionSlot: 1),
            CreateEvent("KerjaLepas", "{\"amount\":1}", sessionId, playerId, actionSlot: 2)
        };

        var handled = new EventTurnProgressValidator().TryValidate(request, CreateConfig(), history, 1, out var result);

        Assert.True(handled);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void TryValidateTurnEndedFriday_RejectsMissingPlayerDonation()
    {
        var firstPlayer = Guid.NewGuid();
        var sessionId = Guid.NewGuid();
        var request = CreateRequest("AkhirGiliran", "{}", sessionId, firstPlayer, "FRI");
        var history = new[]
        {
            CreateEvent("JumatBerkah", """{"amount":1}""", sessionId, firstPlayer, 0, "FRI")
        };

        new EventTurnProgressValidator().TryValidate(request, CreateConfig(), history, 2, out var result);

        Assert.False(result.IsValid);
        Assert.Contains("setiap pemain", result.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void TryValidateTurnEndedSaturday_AcceptsOneDecisionPerPlayerAfterPriceOpened()
    {
        var firstPlayer = Guid.NewGuid();
        var secondPlayer = Guid.NewGuid();
        var sessionId = Guid.NewGuid();
        var request = CreateRequest("AkhirGiliran", "{}", sessionId, firstPlayer, "SAT");
        var history = new[]
        {
            CreateEvent("BukaHargaEmas", """{"gold_price":5}""", sessionId, null, 0, "SAT", "SYSTEM"),
            CreateEvent("InvestasiEmas", """{"trade_type":"BUY","qty":2,"unit_price":5,"amount":10}""", sessionId, firstPlayer, 0, "SAT"),
            CreateEvent("LewatiTransaksiEmas", "{}", sessionId, secondPlayer, 0, "SAT")
        };

        new EventTurnProgressValidator().TryValidate(request, CreateConfig(), history, 2, out var result);

        Assert.True(result.IsValid);
    }

    private static EventRequest CreateRequest(
        string actionType,
        string payloadJson,
        Guid? sessionId = null,
        Guid? playerId = null,
        string weekday = "MON")
    {
        using var document = JsonDocument.Parse(payloadJson);
        return new EventRequest(
            Guid.NewGuid(),
            sessionId ?? Guid.NewGuid(),
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

    private static EventDb CreateEvent(
        string actionType,
        string payloadJson,
        Guid sessionId,
        Guid? playerId,
        int actionSlot,
        string weekday = "MON",
        string actorType = "PLAYER")
    {
        return new EventDb
        {
            EventId = Guid.NewGuid(),
            SessionId = sessionId,
            UserId = playerId,
            ActorType = actorType,
            Timestamp = new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero),
            DayIndex = 0,
            Weekday = weekday,
            ActionSlot = actionSlot,
            SequenceNumber = 0,
            ActionType = actionType,
            RulesetVersionId = Guid.NewGuid(),
            Payload = payloadJson
        };
    }

    private static RulesetConfig CreateConfig(string mode = "PEMULA", int actionsPerTurn = 2)
    {
        return new RulesetConfig(
            mode,
            actionsPerTurn,
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
            LoanEnabled: mode == "MAHIR",
            InsuranceEnabled: mode == "MAHIR",
            SavingGoalEnabled: mode == "MAHIR",
            FreelanceIncome: 5,
            Scoring: null);
    }
}
