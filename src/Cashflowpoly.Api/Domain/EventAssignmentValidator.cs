using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Contracts;
using Microsoft.AspNetCore.Http;

namespace Cashflowpoly.Api.Domain;

internal sealed class EventAssignmentValidator : IEventAssignmentValidator
{
    private static readonly EventPayloadReader _payloadReader = new();
    private const int RulebookMissionPenaltyPoints = 10;

    public bool TryValidate(
        EventRequest request,
        IEnumerable<EventDb> history,
        int participantCount,
        out EventDomainValidationResult result)
    {
        if (string.Equals(request.ActionType, GameActionCatalog.SetupMisiAwal, StringComparison.OrdinalIgnoreCase))
        {
            result = ValidateMission(request, history);
            return true;
        }

        if (string.Equals(request.ActionType, "BagikanTieBreaker", StringComparison.OrdinalIgnoreCase))
        {
            result = ValidateTieBreaker(request, history, participantCount);
            return true;
        }

        result = EventDomainValidationResult.Valid;
        return false;
    }

    private EventDomainValidationResult ValidateMission(EventRequest request, IEnumerable<EventDb> history)
    {
        var playerCheck = RequirePlayer(request);
        if (!playerCheck.IsValid)
        {
            return playerCheck;
        }

        if (!_payloadReader.TryReadMissionAssigned(request.Payload, out var missionId, out var targetCardId, out var penaltyPoints))
        {
            return EventDomainValidationResult.Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Payload mission tidak valid",
                new ErrorDetail("payload.mission_id", "REQUIRED"));
        }

        if (string.IsNullOrWhiteSpace(missionId) || string.IsNullOrWhiteSpace(targetCardId))
        {
            return EventDomainValidationResult.Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Mission ID dan target wajib diisi",
                new ErrorDetail("payload.target_tertiary_card_id", "REQUIRED"));
        }

        if (penaltyPoints < 0)
        {
            return EventDomainValidationResult.Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Penalty points tidak valid",
                new ErrorDetail("payload.penalty_points", "OUT_OF_RANGE"));
        }

        if (penaltyPoints != RulebookMissionPenaltyPoints)
        {
            return EventDomainValidationResult.Fail(
                StatusCodes.Status422UnprocessableEntity,
                "DOMAIN_RULE_VIOLATION",
                "Penalty misi harus 10 poin");
        }

        var alreadyAssigned = history.Any(e =>
            e.UserId == request.UserId &&
            string.Equals(e.ActionType, GameActionCatalog.SetupMisiAwal, StringComparison.OrdinalIgnoreCase));
        if (alreadyAssigned)
        {
            return EventDomainValidationResult.Fail(
                StatusCodes.Status422UnprocessableEntity,
                "DOMAIN_RULE_VIOLATION",
                "Misi sudah ditetapkan untuk pemain");
        }

        var missionAlreadyAssigned = history.Any(e =>
            e.UserId != request.UserId &&
            string.Equals(e.ActionType, GameActionCatalog.SetupMisiAwal, StringComparison.OrdinalIgnoreCase) &&
            _payloadReader.TryReadMissionAssigned(
                _payloadReader.ReadPayload(e.Payload),
                out var assignedMissionId,
                out _,
                out _) &&
            string.Equals(assignedMissionId, missionId, StringComparison.OrdinalIgnoreCase));
        if (missionAlreadyAssigned)
        {
            return EventDomainValidationResult.Fail(
                StatusCodes.Status422UnprocessableEntity,
                "DOMAIN_RULE_VIOLATION",
                "Kartu Misi Koleksi sudah ditetapkan untuk pemain lain");
        }

        return EventDomainValidationResult.Valid;
    }

    private EventDomainValidationResult ValidateTieBreaker(
        EventRequest request,
        IEnumerable<EventDb> history,
        int participantCount)
    {
        var playerCheck = RequirePlayer(request);
        if (!playerCheck.IsValid)
        {
            return playerCheck;
        }

        if (!_payloadReader.TryReadTieBreaker(request.Payload, out var number))
        {
            return EventDomainValidationResult.Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Payload tie breaker tidak valid",
                new ErrorDetail("payload.number", "REQUIRED"));
        }

        if (number < 1 || number > participantCount)
        {
            return EventDomainValidationResult.Fail(
                StatusCodes.Status422UnprocessableEntity,
                "DOMAIN_RULE_VIOLATION",
                $"Nomor tie breaker harus berada pada rentang 1 sampai {participantCount}",
                new ErrorDetail("payload.number", "OUT_OF_RANGE"));
        }

        var alreadyAssigned = history.Any(e =>
            e.UserId == request.UserId &&
            e.ActionType == "BagikanTieBreaker");
        if (alreadyAssigned)
        {
            return EventDomainValidationResult.Fail(
                StatusCodes.Status422UnprocessableEntity,
                "DOMAIN_RULE_VIOLATION",
                "Tie breaker sudah ditetapkan untuk pemain");
        }

        var numberAlreadyAssigned = history.Any(e =>
            e.UserId != request.UserId &&
            string.Equals(e.ActionType, GameActionCatalog.TieBreakerAssigned, StringComparison.OrdinalIgnoreCase) &&
            _payloadReader.TryReadTieBreaker(_payloadReader.ReadPayload(e.Payload), out var assignedNumber) &&
            assignedNumber == number);
        if (numberAlreadyAssigned)
        {
            return EventDomainValidationResult.Fail(
                StatusCodes.Status422UnprocessableEntity,
                "DOMAIN_RULE_VIOLATION",
                "Nomor tie breaker sudah ditetapkan untuk pemain lain");
        }

        return EventDomainValidationResult.Valid;
    }

    private EventDomainValidationResult RequirePlayer(EventRequest request)
    {
        if (request.UserId is not null)
        {
            return EventDomainValidationResult.Valid;
        }

        return EventDomainValidationResult.Fail(
            StatusCodes.Status400BadRequest,
            "VALIDATION_ERROR",
            "Player wajib diisi",
            new ErrorDetail("user_id", "REQUIRED"));
    }
}
