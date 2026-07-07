using System.Collections.Frozen;
using System.Text.Json;
using Cashflowpoly.Api.Contracts;
using Microsoft.AspNetCore.Http;

namespace Cashflowpoly.Api.Domain;

internal sealed class EventRequestShapeValidator : IEventRequestShapeValidator
{
    private static readonly FrozenSet<string> AllowedActorTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "PLAYER",
        "SYSTEM"
    }.ToFrozenSet(StringComparer.OrdinalIgnoreCase);

    private static readonly FrozenSet<string> AllowedWeekdays = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "MON", "TUE", "WED", "THU", "FRI", "SAT", "SUN"
    }.ToFrozenSet(StringComparer.OrdinalIgnoreCase);

    public EventDomainValidationResult Validate(EventRequest request, Guid? scopedPlayerId)
    {
        if (!AllowedActorTypes.Contains(request.ActorType))
        {
            return EventDomainValidationResult.Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Actor type tidak valid",
                new ErrorDetail("actor_type", "INVALID_ENUM"));
        }

        if (scopedPlayerId.HasValue &&
            !string.Equals(request.ActorType, "PLAYER", StringComparison.OrdinalIgnoreCase))
        {
            return EventDomainValidationResult.Fail(
                StatusCodes.Status403Forbidden,
                "FORBIDDEN",
                "Player hanya dapat mengirim event actor PLAYER");
        }

        if (!AllowedWeekdays.Contains(request.Weekday))
        {
            return EventDomainValidationResult.Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Weekday tidak valid",
                new ErrorDetail("weekday", "INVALID_ENUM"));
        }

        if (request.ActionSlot < 0)
        {
            return EventDomainValidationResult.Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Action slot minimal 0",
                new ErrorDetail("action_slot", "OUT_OF_RANGE"));
        }

        if (request.TurnNumber < 0)
        {
            return EventDomainValidationResult.Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Turn number minimal 0",
                new ErrorDetail("turn_number", "OUT_OF_RANGE"));
        }

        if (string.Equals(request.ActorType, "SYSTEM", StringComparison.OrdinalIgnoreCase) &&
            request.TurnNumber != 0)
        {
            return EventDomainValidationResult.Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "System event harus memakai turn number 0",
                new ErrorDetail("turn_number", "INVALID_FOR_ACTOR"));
        }

        if (string.Equals(request.ActorType, "SYSTEM", StringComparison.OrdinalIgnoreCase) &&
            request.ActionSlot != 0)
        {
            return EventDomainValidationResult.Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "System event harus memakai action slot 0",
                new ErrorDetail("action_slot", "INVALID_FOR_ACTOR"));
        }

        if (string.Equals(request.ActorType, "PLAYER", StringComparison.OrdinalIgnoreCase) &&
            request.TurnNumber is < 1 or > 4)
        {
            return EventDomainValidationResult.Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Player event harus memakai turn number 1 sampai 4",
                new ErrorDetail("turn_number", "OUT_OF_RANGE"));
        }

        if (string.Equals(request.ActorType, "PLAYER", StringComparison.OrdinalIgnoreCase) &&
            request.ActionSlot < 1 &&
            !AllowsFreePlayerActionSlot(request.ActionType, request.Payload))
        {
            return EventDomainValidationResult.Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Player event harus memakai action slot minimal 1",
                new ErrorDetail("action_slot", "OUT_OF_RANGE"));
        }

        if (request.DayIndex < 0)
        {
            return EventDomainValidationResult.Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Day index minimal 0",
                new ErrorDetail("day_index", "OUT_OF_RANGE"));
        }

        if (request.SequenceNumber < 0)
        {
            return EventDomainValidationResult.Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Sequence number minimal 0",
                new ErrorDetail("sequence_number", "OUT_OF_RANGE"));
        }

        if (string.Equals(request.ActorType, "PLAYER", StringComparison.OrdinalIgnoreCase) &&
            request.UserId is null)
        {
            return EventDomainValidationResult.Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Player wajib diisi untuk actor PLAYER",
                new ErrorDetail("user_id", "REQUIRED"));
        }

        if (scopedPlayerId.HasValue && request.UserId != scopedPlayerId.Value)
        {
            return EventDomainValidationResult.Fail(
                StatusCodes.Status403Forbidden,
                "FORBIDDEN",
                "Player hanya dapat mengirim event miliknya");
        }

        if (string.IsNullOrWhiteSpace(request.ActionType))
        {
            return EventDomainValidationResult.Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Action type wajib diisi",
                new ErrorDetail("action_type", "REQUIRED"));
        }

        return EventDomainValidationResult.Valid;
    }

    private static bool AllowsFreePlayerActionSlot(string actionType, JsonElement payload)
    {
        return actionType.Equals("JumatBerkah", StringComparison.OrdinalIgnoreCase) ||
               actionType.Equals("RisikoKehidupan", StringComparison.OrdinalIgnoreCase) ||
               actionType.Equals("GunakanOpsiDarurat", StringComparison.OrdinalIgnoreCase) ||
               IsRiskResponse(actionType, payload);
    }

    private static bool IsRiskResponse(string actionType, JsonElement payload)
    {
        return (actionType.Equals("Asuransi", StringComparison.OrdinalIgnoreCase) ||
                actionType.Equals("PinjamanSyariah", StringComparison.OrdinalIgnoreCase)) &&
               (payload.TryGetProperty("risk_event_id", out _) ||
                payload.TryGetProperty("risk_event_ref", out _));
    }
}
