using System.Text.Json;
using Cashflowpoly.Contracts;

namespace Cashflowpoly.Api.Domain;

internal sealed class EventValidationDetailsSerializer : IEventValidationDetailsSerializer
{
    public string? BuildValidationDetailsJson(EventRequest request, ErrorResponse? error)
    {
        if (error is null)
        {
            return null;
        }

        var payload = new
        {
            player_id = request.PlayerId,
            action_type = request.ActionType,
            details = error.Details
        };

        return JsonSerializer.Serialize(payload);
    }
}
