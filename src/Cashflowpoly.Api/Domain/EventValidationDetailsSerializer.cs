// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui EventValidationDetailsSerializer.
using System.Text.Json;
using Cashflowpoly.Api.Contracts;

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
            user_id = request.UserId,
            action_type = request.ActionType,
            details = error.Details
        };

        return JsonSerializer.Serialize(payload);
    }
}
