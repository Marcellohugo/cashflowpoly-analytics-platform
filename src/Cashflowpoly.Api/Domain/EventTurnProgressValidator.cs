using Cashflowpoly.Api.Data;
using Cashflowpoly.Contracts;
using Microsoft.AspNetCore.Http;

namespace Cashflowpoly.Api.Domain;

internal sealed class EventTurnProgressValidator : IEventTurnProgressValidator
{
    private static readonly EventPayloadReader _payloadReader = new();

    public bool RequiresHistory(EventRequest request, RulesetConfig config)
    {
        return string.Equals(request.ActionType, "turn.action.used", StringComparison.OrdinalIgnoreCase) ||
               (string.Equals(request.ActionType, "turn.ended", StringComparison.OrdinalIgnoreCase) &&
                string.Equals(config.Mode, "MAHIR", StringComparison.OrdinalIgnoreCase));
    }

    public bool TryValidate(
        EventRequest request,
        RulesetConfig config,
        IEnumerable<EventDb> history,
        out EventDomainValidationResult result)
    {
        if (string.Equals(request.ActionType, "turn.action.used", StringComparison.OrdinalIgnoreCase))
        {
            result = ValidateActionUsed(request, config, history);
            return true;
        }

        if (string.Equals(request.ActionType, "turn.ended", StringComparison.OrdinalIgnoreCase) &&
            string.Equals(config.Mode, "MAHIR", StringComparison.OrdinalIgnoreCase))
        {
            result = ValidateTurnEndedMahir(request, history);
            return true;
        }

        result = EventDomainValidationResult.Valid;
        return false;
    }

    private EventDomainValidationResult ValidateActionUsed(
        EventRequest request,
        RulesetConfig config,
        IEnumerable<EventDb> history)
    {
        if (!_payloadReader.TryReadActionUsed(request.Payload, out var used, out var remaining))
        {
            return EventDomainValidationResult.Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Payload action used tidak valid",
                new ErrorDetail("payload", "INVALID_STRUCTURE"));
        }

        if (used < 0 || remaining < 0)
        {
            return EventDomainValidationResult.Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Nilai used/remaining tidak valid",
                new ErrorDetail("payload.used", "OUT_OF_RANGE"));
        }

        if (request.PlayerId is null)
        {
            return EventDomainValidationResult.Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Player wajib diisi",
                new ErrorDetail("player_id", "REQUIRED"));
        }

        var usedSoFar = 0;
        foreach (var evt in history.Where(e =>
                     e.PlayerId == request.PlayerId &&
                     e.TurnNumber == request.TurnNumber &&
                     e.ActionType == "turn.action.used"))
        {
            if (_payloadReader.TryReadActionUsed(_payloadReader.ReadPayload(evt.Payload), out var usedValue, out _))
            {
                usedSoFar += usedValue;
            }
        }

        if (usedSoFar + used > config.ActionsPerTurn)
        {
            return EventDomainValidationResult.Fail(
                StatusCodes.Status422UnprocessableEntity,
                "DOMAIN_RULE_VIOLATION",
                "Jumlah token aksi melebihi batas ruleset");
        }

        return EventDomainValidationResult.Valid;
    }

    private EventDomainValidationResult ValidateTurnEndedMahir(EventRequest request, IEnumerable<EventDb> history)
    {
        var turnEvents = history.Where(e => e.TurnNumber == request.TurnNumber && e.PlayerId.HasValue).ToList();

        var orderCounts = turnEvents
            .Where(e => e.ActionType == "order.claimed")
            .GroupBy(e => e.PlayerId!.Value)
            .ToDictionary(g => g.Key, g => g.Count());

        var riskCounts = turnEvents
            .Where(e => e.ActionType == "risk.life.drawn")
            .GroupBy(e => e.PlayerId!.Value)
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
