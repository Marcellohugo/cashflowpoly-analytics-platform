// Fungsi file: Menguji validasi envelope event sebelum validasi domain/action spesifik.
using System.Text.Json;
using Cashflowpoly.Api.Domain;
using Cashflowpoly.Contracts;
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
        var request = CreateRequest() with { ActorType = "SYSTEM", PlayerId = playerId };

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
        var request = CreateRequest() with { PlayerId = Guid.NewGuid() };

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
        var request = CreateRequest() with { PlayerId = playerId };

        var result = new EventRequestShapeValidator().Validate(request, scopedPlayerId: playerId);

        Assert.True(result.IsValid);
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        Assert.Null(result.ErrorCode);
        Assert.Empty(result.Details);
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
            "transaction.recorded",
            Guid.NewGuid(),
            document.RootElement.Clone(),
            "client-123");
    }
}
