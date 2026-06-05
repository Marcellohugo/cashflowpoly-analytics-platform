using System.Text.Json;

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
        return actionType.Trim().ToLowerInvariant() switch
        {
            "transaction.recorded" => "transaction.recorded",
            "ingredient.purchased" => "BahanMasakan",
            "ingredient.discarded" => "ingredient.discarded",
            "order.claimed" => "JualMasakan",
            "order.passed" => "order.passed",
            "need.primary.purchased" or "need.secondary.purchased" or "need.tertiary.purchased" => "Kebutuhan",
            "work.freelance.completed" or "freelance.worked" => "KerjaLepas",
            "saving.deposit.created" => "Menabung",
            "saving.deposit.withdrawn" => "saving.deposit.withdrawn",
            "saving.goal.achieved" => "TujuanFinansial",
            "day.friday.donation" => "JumatBerkah",
            "day.saturday.gold_trade" => ResolveGoldTradeActionId(payload),
            "loan.syariah.taken" => "PinjamanSyariah",
            "loan.syariah.repaid" => "BayarPinjaman",
            "insurance.multirisk.purchased" or "insurance.multirisk.used" => "Asuransi",
            "risk.life.drawn" => "RisikoKehidupan",
            "risk.emergency.used" => "risk.emergency.used",
            "donation.rank.awarded" => "donation.rank.awarded",
            "gold.points.awarded" => "gold.points.awarded",
            "pension.rank.awarded" => "pension.rank.awarded",
            "tie_breaker.assigned" => "TieBreakerAssigned",
            "mission.assigned" => "MissionAssigned",
            "session.ended" => "SessionEnded",
            "turn.action.used" => "AkhirGiliran",
            _ => null
        };
    }

    private static string ResolveGoldTradeActionId(JsonElement payload)
    {
        if (payload.ValueKind == JsonValueKind.Object &&
            payload.TryGetProperty("trade_type", out var tradeTypeProp) &&
            tradeTypeProp.ValueKind == JsonValueKind.String &&
            string.Equals(tradeTypeProp.GetString(), "SELL", StringComparison.OrdinalIgnoreCase))
        {
            return "JualEmas";
        }

        return "InvestasiEmas";
    }
}
