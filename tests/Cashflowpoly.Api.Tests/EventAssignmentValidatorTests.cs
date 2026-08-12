// Fungsi file: Memverifikasi perilaku API, database, atau domain melalui EventAssignmentValidatorTests.
using System.Text.Json;
using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Domain;
using Cashflowpoly.Api.Contracts;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Cashflowpoly.Api.Tests;

public sealed class EventAssignmentValidatorTests
{
    [Fact]
    public void TryValidate_MissionRejectsDuplicateAssignmentForPlayer()
    {
        var playerId = Guid.NewGuid();
        var request = CreateRequest(
            playerId,
            "SetupMisiAwal",
            """{"mission_id":"m-1","target_tertiary_card_id":"bike","penalty_points":10}""");
        var history = new[]
        {
            CreateEvent(playerId, "SetupMisiAwal", """{"mission_id":"m-old","target_tertiary_card_id":"phone","penalty_points":10}""")
        };

        var handled = new EventAssignmentValidator().TryValidate(request, history, 4, out var result);

        Assert.True(handled);
        Assert.False(result.IsValid);
        Assert.Equal(StatusCodes.Status422UnprocessableEntity, result.StatusCode);
        Assert.Equal("Misi sudah ditetapkan untuk pemain", result.Message);
    }

    [Fact]
    public void TryValidate_TieBreakerAcceptsPositiveNumber()
    {
        var request = CreateRequest(Guid.NewGuid(), "BagikanTieBreaker", """{"number":3}""");

        var handled = new EventAssignmentValidator().TryValidate(request, Array.Empty<EventDb>(), 4, out var result);

        Assert.True(handled);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void TryValidate_MissionRejectsCardAssignedToAnotherPlayer()
    {
        var request = CreateRequest(
            Guid.NewGuid(),
            "SetupMisiAwal",
            """{"mission_id":"m-1","target_tertiary_card_id":"bike","penalty_points":10}""");
        var history = new[]
        {
            CreateEvent(
                Guid.NewGuid(),
                "SetupMisiAwal",
                """{"mission_id":"m-1","target_tertiary_card_id":"bike","penalty_points":10}""")
        };

        var handled = new EventAssignmentValidator().TryValidate(request, history, 4, out var result);

        Assert.True(handled);
        Assert.False(result.IsValid);
        Assert.Equal("Kartu Misi Koleksi sudah ditetapkan untuk pemain lain", result.Message);
    }

    [Fact]
    public void TryValidate_TieBreakerRejectsNumberAssignedToAnotherPlayer()
    {
        var request = CreateRequest(Guid.NewGuid(), "BagikanTieBreaker", """{"number":3}""");
        var history = new[]
        {
            CreateEvent(Guid.NewGuid(), "BagikanTieBreaker", """{"number":3}""")
        };

        var handled = new EventAssignmentValidator().TryValidate(request, history, 4, out var result);

        Assert.True(handled);
        Assert.False(result.IsValid);
        Assert.Equal("Nomor tie breaker sudah ditetapkan untuk pemain lain", result.Message);
    }

    [Fact]
    public void TryValidate_TieBreakerRejectsNumberOutsideParticipantRange()
    {
        var request = CreateRequest(Guid.NewGuid(), "BagikanTieBreaker", """{"number":5}""");

        var handled = new EventAssignmentValidator().TryValidate(request, Array.Empty<EventDb>(), 4, out var result);

        Assert.True(handled);
        Assert.False(result.IsValid);
        Assert.Equal(StatusCodes.Status422UnprocessableEntity, result.StatusCode);
        Assert.Equal("payload.number", Assert.Single(result.Details).Field);
    }

    private static EventRequest CreateRequest(Guid playerId, string actionType, string payloadJson)
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

    private static EventDb CreateEvent(Guid playerId, string actionType, string payload)
    {
        return new EventDb
        {
            EventId = Guid.NewGuid(),
            SessionId = Guid.NewGuid(),
            UserId = playerId,
            ActorType = "PLAYER",
            Timestamp = new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero),
            DayIndex = 0,
            Weekday = "MON",
            ActionSlot = 1,
            SequenceNumber = 1,
            ActionType = actionType,
            RulesetVersionId = Guid.NewGuid(),
            Payload = payload
        };
    }
}
