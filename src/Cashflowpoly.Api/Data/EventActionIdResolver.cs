// Fungsi file: Mengelola pemetaan dan akses PostgreSQL untuk EventActionIdResolver.
// Mengimpor namespace `System.Text.Json` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.Json;
// Mengimpor namespace `Cashflowpoly.Api.Domain` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Domain;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Data` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
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
        // Menangani exception `JsonException` melalui variabel dalam Resolve.
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
