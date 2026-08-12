// Fungsi file: Mengelola pemetaan dan akses PostgreSQL untuk EventActionIdResolver.
using System.Text.Json;
using Cashflowpoly.Api.Domain;

namespace Cashflowpoly.Api.Data;

public static class EventActionIdResolver
{
    public static string? Resolve(string actionType, string payloadJson)
    {
        if (string.IsNullOrWhiteSpace(payloadJson))
        {
            using var emptyDocument = JsonDocument.Parse("{}");
            return Resolve(actionType, emptyDocument.RootElement);
        }

        try
        {
            using var document = JsonDocument.Parse(payloadJson);
            return Resolve(actionType, document.RootElement);
        }
        catch (JsonException)
        {
            return Resolve(actionType, default(JsonElement));
        }
    }

    public static string? Resolve(string actionType, JsonElement payload)
    {
        return GameActionCatalog.ResolveGameActionId(actionType, payload);
    }
}
