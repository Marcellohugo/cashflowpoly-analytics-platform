using System.Collections.Frozen;
using Cashflowpoly.Contracts;
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

        if (request.TurnNumber < 1)
        {
            return EventDomainValidationResult.Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Turn number minimal 1",
                new ErrorDetail("turn_number", "OUT_OF_RANGE"));
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
            request.PlayerId is null)
        {
            return EventDomainValidationResult.Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Player wajib diisi untuk actor PLAYER",
                new ErrorDetail("player_id", "REQUIRED"));
        }

        if (scopedPlayerId.HasValue && request.PlayerId != scopedPlayerId.Value)
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
}
