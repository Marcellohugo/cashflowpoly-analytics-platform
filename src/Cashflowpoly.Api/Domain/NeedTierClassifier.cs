// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui NeedTierClassifier.
// Mengimpor namespace `System.Text.Json` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.Json;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Domain;

internal enum NeedTier
{
    Unknown,
    Primary,
    Secondary,
    Tertiary
}

internal static class NeedTierClassifier
{
    public static NeedTier FromPayload(JsonElement payload, string? cardId = null)
    {
        // Mengulangi setiap elemen `new[] { ”need_tier”, ”tier”, ”need_type” }`; elemen saat ini disimpan sebagai `propertyName` bertipe `var` untuk
        // diproses oleh badan loop dalam FromPayload.
        foreach (var propertyName in new[] { "need_tier", "tier", "need_type" })
        {
            if (payload.TryGetProperty(propertyName, out var property) &&
                property.ValueKind == JsonValueKind.String)
            {
                var fromTier = FromText(property.GetString());
                if (fromTier != NeedTier.Unknown)
                {
                    return fromTier;
                }
            }
        }

        if (string.IsNullOrWhiteSpace(cardId) &&
            payload.TryGetProperty("card_id", out var cardProperty) &&
            cardProperty.ValueKind == JsonValueKind.String)
        {
            cardId = cardProperty.GetString();
        }

        return FromCardId(cardId);
    }

    public static NeedTier FromPayloadJson(string payloadJson)
    {
        if (string.IsNullOrWhiteSpace(payloadJson))
        {
            return NeedTier.Unknown;
        }

        try
        {
            using var document = JsonDocument.Parse(payloadJson);
            return FromPayload(document.RootElement);
        }
        // Menangani exception `JsonException` melalui variabel dalam FromPayloadJson.
        catch (JsonException)
        {
            return NeedTier.Unknown;
        }
    }

    private static NeedTier FromText(string? value)
    {
        return (value ?? string.Empty).Trim().ToLowerInvariant() switch
        {
            // Untuk pola `”primary” or ”primer”`, menghasilkan `NeedTier.Primary` (nilai primary) sebagai hasil switch.
            "primary" or "primer" => NeedTier.Primary,
            // Untuk pola `”secondary” or ”sekunder”`, menghasilkan `NeedTier.Secondary` (nilai secondary) sebagai hasil switch.
            "secondary" or "sekunder" => NeedTier.Secondary,
            // Untuk pola `”tertiary” or ”tersier”`, menghasilkan `NeedTier.Tertiary` (nilai tertiary) sebagai hasil switch.
            "tertiary" or "tersier" => NeedTier.Tertiary,
            // Untuk pola `_`, menghasilkan `NeedTier.Unknown` (nilai unknown) sebagai hasil switch.
            _ => NeedTier.Unknown
        };
    }

    private static NeedTier FromCardId(string? cardId)
    {
        cardId = System.Text.RegularExpressions.Regex.Replace(cardId ?? string.Empty, "_[0-9]+$", "");
        var value = cardId.Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(value))
        {
            return NeedTier.Unknown;
        }

        if (value.StartsWith("primary", StringComparison.Ordinal) ||
            value.StartsWith("rice", StringComparison.Ordinal) ||
            value.StartsWith("water", StringComparison.Ordinal) ||
            value.Contains("food", StringComparison.Ordinal) ||
            value == "buku")
        {
            return NeedTier.Primary;
        }

        if (value.StartsWith("secondary", StringComparison.Ordinal) ||
            value.Contains("school", StringComparison.Ordinal) ||
            value == "book" ||
            value == "sepatu")
        {
            return NeedTier.Secondary;
        }

        if (value.StartsWith("tertiary", StringComparison.Ordinal) ||
            value.Contains("bike", StringComparison.Ordinal) ||
            value is "boneka" or "gameboy" or "hiburan")
        {
            return NeedTier.Tertiary;
        }

        return NeedTier.Tertiary;
    }
}
