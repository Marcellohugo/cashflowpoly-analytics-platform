// Fungsi file: Memverifikasi validasi pembelian kebutuhan dan aturan urutan kebutuhan primer.
using System.Text.Json;
using Cashflowpoly.Api.Contracts;
using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Domain;
using Xunit;

namespace Cashflowpoly.Api.Tests;

public sealed class EventNeedPurchaseValidatorTests
{
    [Fact]
    public void TryValidate_ReturnsFalseForUnhandledAction()
    {
        var request = CreateRequest("CatatTransaksi", """{"amount":1}""");

        var handled = new EventNeedPurchaseValidator().TryValidate(request, CreateConfig(), [], out var result);

        Assert.False(handled);
        Assert.True(result.Validation.IsValid);
        Assert.Null(result.OutgoingAmount);
    }

    [Fact]
    public void TryValidatePrimary_ReturnsOutgoingAmount()
    {
        var playerId = Guid.NewGuid();
        var request = CreateRequest(
            "Kebutuhan",
            """{"card_id":"rice","amount":5,"points":2,"need_tier":"primer"}""",
            playerId);

        var handled = new EventNeedPurchaseValidator().TryValidate(request, CreateConfig(), [], out var result);

        Assert.True(handled);
        Assert.True(result.Validation.IsValid);
        Assert.Equal(5, result.OutgoingAmount);
    }

    [Fact]
    public void TryValidatePrimary_RejectsPurchasePastDailyLimit()
    {
        var playerId = Guid.NewGuid();
        var request = CreateRequest(
            "Kebutuhan",
            """{"card_id":"rice-2","amount":5,"points":2,"need_tier":"primer"}""",
            playerId);
        var history = new[]
        {
            CreateEvent(request, """{"card_id":"rice-1","amount":3,"points":1,"need_tier":"primer"}""")
        };

        var handled = new EventNeedPurchaseValidator().TryValidate(request, CreateConfig(), history, out var result);

        Assert.True(handled);
        Assert.False(result.Validation.IsValid);
        Assert.Equal("DOMAIN_RULE_VIOLATION", result.Validation.ErrorCode);
    }

    [Fact]
    public void TryValidateSecondary_RejectsPurchaseWithoutPrimaryWhenRequired()
    {
        var playerId = Guid.NewGuid();
        var request = CreateRequest(
            "Kebutuhan",
            """{"card_id":"book","amount":4,"points":1,"need_tier":"sekunder"}""",
            playerId);

        var handled = new EventNeedPurchaseValidator().TryValidate(request, CreateConfig(), [], out var result);

        Assert.True(handled);
        Assert.False(result.Validation.IsValid);
        Assert.Equal("DOMAIN_RULE_VIOLATION", result.Validation.ErrorCode);
    }

    [Fact]
    public void TryValidateSecondary_AllowsPurchaseAfterPrimary()
    {
        var playerId = Guid.NewGuid();
        var request = CreateRequest(
            "Kebutuhan",
            """{"card_id":"book","amount":4,"points":1,"need_tier":"sekunder"}""",
            playerId);
        var history = new[]
        {
            CreateEvent(request, """{"card_id":"rice","amount":3,"points":1,"need_tier":"primer"}""")
        };

        var handled = new EventNeedPurchaseValidator().TryValidate(request, CreateConfig(), history, out var result);

        Assert.True(handled);
        Assert.True(result.Validation.IsValid, result.Validation.Message);
        Assert.Equal(4, result.OutgoingAmount);
    }

    [Fact]
    public void TryValidateSecondary_AllowsPurchaseWhenRuleIsDisabled()
    {
        var playerId = Guid.NewGuid();
        var request = CreateRequest(
            "Kebutuhan",
            """{"card_id":"book","amount":4,"points":1,"need_tier":"sekunder"}""",
            playerId);

        var handled = new EventNeedPurchaseValidator().TryValidate(
            request,
            CreateConfig() with { RequirePrimaryBeforeOthers = false },
            [],
            out var result);

        Assert.True(handled);
        Assert.True(result.Validation.IsValid, result.Validation.Message);
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

    private static EventDb CreateEvent(EventRequest request, string payload) => new()
    {
        EventId = Guid.NewGuid(),
        SessionId = request.SessionId,
        UserId = request.UserId,
        ActorType = "PLAYER",
        Timestamp = request.Timestamp,
        DayIndex = request.DayIndex,
        Weekday = request.Weekday,
        ActionSlot = 1,
        SequenceNumber = 1,
        ActionType = "Kebutuhan",
        RulesetVersionId = request.RulesetVersionId,
        Payload = payload
    };

    private static RulesetConfig CreateConfig() => new(
        "PEMULA",
        ActionsPerTurn: 2,
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
        Scoring: null);
}
