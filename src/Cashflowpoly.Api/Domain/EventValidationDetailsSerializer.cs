// Fungsi file: Membentuk payload JSON detail validasi event untuk audit/logging internal.
using System.Text.Json;
using Cashflowpoly.Contracts;

namespace Cashflowpoly.Api.Domain;

internal static class EventValidationDetailsSerializer
{
    internal static string? BuildValidationDetailsJson(EventRequest request, ErrorResponse? error)
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
