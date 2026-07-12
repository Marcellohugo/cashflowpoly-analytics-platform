using System.Text.Json;
using Cashflowpoly.Api.Domain;
using Cashflowpoly.Api.Contracts;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Cashflowpoly.Api.Tests;

public sealed class EventRequestShapeValidatorTests
{
    [Fact]
    public void Validate_RejectsInvalidActorType()
    {
        var request = CreateRequest() with { ActorType = "BANK" };

        var result = new EventRequestShapeValidator().Validate(request, scopedPlayerId: null);

        Assert.False(result.IsValid);
        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        Assert.Equal("VALIDATION_ERROR", result.ErrorCode);
        Assert.Equal("Actor type tidak valid", result.Message);
        Assert.Contains(result.Details, detail => detail.Field == "actor_type" && detail.Issue == "INVALID_ENUM");
    }

    [Fact]
    public void Validate_RejectsSystemActorFromScopedPlayer()
    {
        var playerId = Guid.NewGuid();
        var request = CreateRequest() with { ActorType = "SYSTEM", UserId = playerId };

        var result = new EventRequestShapeValidator().Validate(request, scopedPlayerId: playerId);

        Assert.False(result.IsValid);
        Assert.Equal(StatusCodes.Status403Forbidden, result.StatusCode);
        Assert.Equal("FORBIDDEN", result.ErrorCode);
        Assert.Equal("Player hanya dapat mengirim event actor PLAYER", result.Message);
        Assert.Empty(result.Details);
    }

    [Fact]
    public void Validate_RejectsMismatchedScopedPlayer()
    {
        var scopedPlayerId = Guid.NewGuid();
        var request = CreateRequest() with { UserId = Guid.NewGuid() };

        var result = new EventRequestShapeValidator().Validate(request, scopedPlayerId);

        Assert.False(result.IsValid);
        Assert.Equal(StatusCodes.Status403Forbidden, result.StatusCode);
        Assert.Equal("FORBIDDEN", result.ErrorCode);
        Assert.Equal("Player hanya dapat mengirim event miliknya", result.Message);
    }

    [Fact]
    public void Validate_AcceptsValidPlayerEvent()
    {
        var playerId = Guid.NewGuid();
        var request = CreateRequest() with { UserId = playerId };

        var result = new EventRequestShapeValidator().Validate(request, scopedPlayerId: playerId);

        Assert.True(result.IsValid);
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        Assert.Null(result.ErrorCode);
        Assert.Empty(result.Details);
    }

    [Fact]
    public void Validate_AcceptsValidSystemEventWithZeroTurnAndActionSlot()
    {
        var request = CreateRequest() with
        {
            ActorType = "SYSTEM",
            UserId = null,
            ActionSlot = 0,
            TurnNumber = 0
        };

        var result = new EventRequestShapeValidator().Validate(request, scopedPlayerId: null);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_AcceptsActionSlotAboveTwoForRulesetDrivenLimit()
    {
        var playerId = Guid.NewGuid();
        var request = CreateRequest() with { UserId = playerId, ActionSlot = 3 };

        var result = new EventRequestShapeValidator().Validate(request, scopedPlayerId: playerId);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_RejectsSystemActionSlotAboveZero()
    {
        var request = CreateRequest() with
        {
            ActorType = "SYSTEM",
            UserId = null,
            ActionSlot = 1,
            TurnNumber = 0
        };

        var result = new EventRequestShapeValidator().Validate(request, scopedPlayerId: null);

        Assert.False(result.IsValid);
        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        Assert.Contains(result.Details, detail => detail.Field == "action_slot" && detail.Issue == "INVALID_FOR_ACTOR");
    }

    [Fact]
    public void Validate_RejectsPlayerActionSlotZero()
    {
        var request = CreateRequest() with { ActionSlot = 0 };

        var result = new EventRequestShapeValidator().Validate(request, scopedPlayerId: null);

        Assert.False(result.IsValid);
        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        Assert.Contains(result.Details, detail => detail.Field == "action_slot" && detail.Issue == "OUT_OF_RANGE");
    }

    [Theory]
    [InlineData("JumatBerkah")]
    [InlineData("RisikoKehidupan")]
    [InlineData("BayarRisiko")]
    [InlineData("GunakanOpsiDarurat")]
    [InlineData("InvestasiEmas")]
    [InlineData("JualEmas")]
    [InlineData("LewatiTransaksiEmas")]
    public void Validate_AcceptsPlayerActionSlotZeroForRulebookFreeActions(string actionType)
    {
        var request = CreateRequest() with { ActionType = actionType, ActionSlot = 0 };

        var result = new EventRequestShapeValidator().Validate(request, scopedPlayerId: null);

        Assert.True(result.IsValid, result.Message);
    }

    [Theory]
    [InlineData("JumatBerkah")]
    [InlineData("RisikoKehidupan")]
    [InlineData("GunakanOpsiDarurat")]
    [InlineData("InvestasiEmas")]
    [InlineData("JualEmas")]
    [InlineData("LewatiTransaksiEmas")]
    public void Validate_RejectsNonZeroSlotForRulebookFreeActions(string actionType)
    {
        var request = CreateRequest() with { ActionType = actionType, ActionSlot = 1 };

        var result = new EventRequestShapeValidator().Validate(request, scopedPlayerId: null);

        Assert.False(result.IsValid);
        Assert.Contains(result.Details, detail => detail.Field == "action_slot" && detail.Issue == "INVALID_FOR_ACTION");
    }

    [Theory]
    [InlineData("HariMingguLibur")]
    [InlineData("AkhirGiliran")]
    public void Validate_RejectsSystemOnlyActionsFromPlayer(string actionType)
    {
        var request = CreateRequest() with { ActionType = actionType, ActionSlot = 0 };

        var result = new EventRequestShapeValidator().Validate(request, scopedPlayerId: null);

        Assert.False(result.IsValid);
        Assert.Contains(result.Details, detail => detail.Field == "actor_type" && detail.Issue == "SYSTEM_REQUIRED");
    }

    [Theory]
    [InlineData("HariMingguLibur")]
    [InlineData("AkhirGiliran")]
    public void Validate_AcceptsSystemOnlyActionsWithZeroTurnAndSlot(string actionType)
    {
        var request = CreateRequest() with
        {
            ActionType = actionType,
            ActorType = "SYSTEM",
            UserId = null,
            TurnNumber = 0,
            ActionSlot = 0
        };

        var result = new EventRequestShapeValidator().Validate(request, scopedPlayerId: null);

        Assert.True(result.IsValid, result.Message);
    }

    [Theory]
    [InlineData("Asuransi")]
    [InlineData("PinjamanSyariah")]
    public void Validate_AcceptsPlayerActionSlotZeroForRiskResponses(string actionType)
    {
        using var document = JsonDocument.Parse("""{"risk_event_id":"95000000-0000-0000-0000-000000000123"}""");
        var request = CreateRequest() with
        {
            ActionType = actionType,
            ActionSlot = 0,
            Payload = document.RootElement.Clone()
        };

        var result = new EventRequestShapeValidator().Validate(request, scopedPlayerId: null);

        Assert.True(result.IsValid, result.Message);
    }

    [Theory]
    [InlineData("Asuransi")]
    [InlineData("PinjamanSyariah")]
    public void Validate_RejectsPlayerActionSlotZeroForRiskActionsWithoutRiskReference(string actionType)
    {
        var request = CreateRequest() with { ActionType = actionType, ActionSlot = 0 };

        var result = new EventRequestShapeValidator().Validate(request, scopedPlayerId: null);

        Assert.False(result.IsValid);
        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        Assert.Contains(result.Details, detail => detail.Field == "action_slot" && detail.Issue == "OUT_OF_RANGE");
    }

    [Fact]
    public void Validate_RejectsNegativeTurnNumber()
    {
        var request = CreateRequest() with { TurnNumber = -1 };

        var result = new EventRequestShapeValidator().Validate(request, scopedPlayerId: null);

        Assert.False(result.IsValid);
        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        Assert.Contains(result.Details, detail => detail.Field == "turn_number" && detail.Issue == "OUT_OF_RANGE");
    }

    private static EventRequest CreateRequest()
    {
        using var document = JsonDocument.Parse("{}");
        return new EventRequest(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "PLAYER",
            new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero),
            0,
            "MON",
            1,
            0,
            "CatatTransaksi",
            Guid.NewGuid(),
            document.RootElement.Clone(),
            "client-123",
            1);
    }
}
