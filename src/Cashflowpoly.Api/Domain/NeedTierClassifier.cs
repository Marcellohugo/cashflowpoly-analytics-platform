using System.Text.Json;

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
        catch (JsonException)
        {
            return NeedTier.Unknown;
        }
    }

    private static NeedTier FromText(string? value)
    {
        return (value ?? string.Empty).Trim().ToLowerInvariant() switch
        {
            "primary" or "primer" => NeedTier.Primary,
            "secondary" or "sekunder" => NeedTier.Secondary,
            "tertiary" or "tersier" => NeedTier.Tertiary,
            _ => NeedTier.Unknown
        };
    }

    private static NeedTier FromCardId(string? cardId)
    {
        var value = (cardId ?? string.Empty).Trim().ToLowerInvariant();
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
