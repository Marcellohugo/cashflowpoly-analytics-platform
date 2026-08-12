// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui EventTurnProgressValidator.
using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Contracts;
using Microsoft.AspNetCore.Http;

namespace Cashflowpoly.Api.Domain;

internal sealed class EventTurnProgressValidator : IEventTurnProgressValidator
{
    private static readonly EventPayloadReader _payloadReader = new();

    public bool RequiresHistory(EventRequest request, RulesetConfig config)
    {
        return GameActionCatalog.Is(request.ActionType, request.Payload, GameActionCatalog.AkhirGiliran) &&
               string.Equals(config.Mode, "MAHIR", StringComparison.OrdinalIgnoreCase);
    }

    public bool TryValidate(
        EventRequest request,
        RulesetConfig config,
        IEnumerable<EventDb> history,
        out EventDomainValidationResult result)
    {
        if (GameActionCatalog.Is(request.ActionType, request.Payload, GameActionCatalog.AkhirGiliran) &&
            string.Equals(config.Mode, "MAHIR", StringComparison.OrdinalIgnoreCase))
        {
            result = ValidateTurnEndedMahir(request, history);
            return true;
        }

        result = EventDomainValidationResult.Valid;
        return false;
    }

    private EventDomainValidationResult ValidateTurnEndedMahir(EventRequest request, IEnumerable<EventDb> history)
    {
        var turnEvents = history
            .Where(e => e.SessionId == request.SessionId &&
                        e.DayIndex == request.DayIndex &&
                        e.UserId.HasValue)
            .ToList();

        var orderCounts = turnEvents
            .Where(e => GameActionCatalog.Is(e.ActionType, _payloadReader.ReadPayload(e.Payload), GameActionCatalog.JualMasakan))
            .GroupBy(e => e.UserId!.Value)
            .ToDictionary(g => g.Key, g => g.Count());

        var riskCounts = turnEvents
            .Where(e => GameActionCatalog.Is(e.ActionType, _payloadReader.ReadPayload(e.Payload), GameActionCatalog.RisikoKehidupan))
            .GroupBy(e => e.UserId!.Value)
            .ToDictionary(g => g.Key, g => g.Count());

        foreach (var playerId in orderCounts.Keys.Union(riskCounts.Keys))
        {
            orderCounts.TryGetValue(playerId, out var orders);
            riskCounts.TryGetValue(playerId, out var risks);
            if (orders != risks)
            {
                return EventDomainValidationResult.Fail(
                    StatusCodes.Status422UnprocessableEntity,
                    "DOMAIN_RULE_VIOLATION",
                    "Setiap klaim pesanan harus diikuti pengambilan risiko pada mode MAHIR");
            }
        }

        return EventDomainValidationResult.Valid;
    }

}
