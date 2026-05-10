using System.Text.Json;
using Cashflowpoly.Api.Domain;
using Cashflowpoly.Contracts;
using Xunit;

namespace Cashflowpoly.Api.Tests;

public sealed class EventValidationDetailsSerializerTests
{
    [Fact]
    public void BuildValidationDetailsJson_ReturnsNullWhenErrorIsMissing()
    {
        var request = CreateEventRequest(Guid.NewGuid(), "transaction.recorded");

        var json = new EventValidationDetailsSerializer().BuildValidationDetailsJson(request, null);

        Assert.Null(json);
    }

    [Fact]
    public void BuildValidationDetailsJson_SerializesPlayerActionAndErrorDetails()
    {
        var playerId = Guid.NewGuid();
        var request = CreateEventRequest(playerId, "transaction.recorded");
        var error = new ErrorResponse(
            "VALIDATION_ERROR",
            "Payload transaksi tidak valid",
            new List<ErrorDetail> { new("payload.amount", "OUT_OF_RANGE") },
            "trace-123");

        var json = new EventValidationDetailsSerializer().BuildValidationDetailsJson(request, error);

        using var document = JsonDocument.Parse(json!);
        var root = document.RootElement;
        Assert.Equal(playerId, root.GetProperty("player_id").GetGuid());
        Assert.Equal("transaction.recorded", root.GetProperty("action_type").GetString());
        var detail = root.GetProperty("details")[0];
        Assert.Equal("payload.amount", detail.GetProperty("field").GetString());
        Assert.Equal("OUT_OF_RANGE", detail.GetProperty("issue").GetString());
    }

    private static EventRequest CreateEventRequest(Guid? playerId, string actionType)
    {
        using var document = JsonDocument.Parse("{}");
        return new EventRequest(
            Guid.NewGuid(),
            Guid.NewGuid(),
            playerId,
            "player",
            new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero),
            1,
            "Friday",
            1,
            1,
            actionType,
            Guid.NewGuid(),
            document.RootElement.Clone(),
            "client-123");
    }
}
