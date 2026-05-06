// Fungsi file: Memvalidasi assignment misi dan tie breaker yang bergantung pada histori pemain.
using Cashflowpoly.Api.Data;
using Cashflowpoly.Contracts;
using Microsoft.AspNetCore.Http;
using static Cashflowpoly.Api.Domain.EventPayloadReader;

namespace Cashflowpoly.Api.Domain;

internal static class EventAssignmentValidator
{
    private const int RulebookMissionPenaltyPoints = 10;

    internal static bool TryValidate(
        EventRequest request,
        IEnumerable<EventDb> history,
        out EventDomainValidationResult result)
    {
        if (string.Equals(request.ActionType, "mission.assigned", StringComparison.OrdinalIgnoreCase))
        {
            result = ValidateMission(request, history);
            return true;
        }

        if (string.Equals(request.ActionType, "tie_breaker.assigned", StringComparison.OrdinalIgnoreCase))
        {
            result = ValidateTieBreaker(request, history);
            return true;
        }

        result = EventDomainValidationResult.Valid;
        return false;
    }

    private static EventDomainValidationResult ValidateMission(EventRequest request, IEnumerable<EventDb> history)
    {
        var playerCheck = RequirePlayer(request);
        if (!playerCheck.IsValid)
        {
            return playerCheck;
        }

        if (!TryReadMissionAssigned(request.Payload, out var missionId, out var targetCardId, out var penaltyPoints))
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
            e.PlayerId == request.PlayerId &&
            e.ActionType == "mission.assigned");
        if (alreadyAssigned)
        {
            return EventDomainValidationResult.Fail(
                StatusCodes.Status422UnprocessableEntity,
                "DOMAIN_RULE_VIOLATION",
                "Misi sudah ditetapkan untuk pemain");
        }

        return EventDomainValidationResult.Valid;
    }

    private static EventDomainValidationResult ValidateTieBreaker(EventRequest request, IEnumerable<EventDb> history)
    {
        var playerCheck = RequirePlayer(request);
        if (!playerCheck.IsValid)
        {
            return playerCheck;
        }

        if (!TryReadTieBreaker(request.Payload, out var number))
        {
            return EventDomainValidationResult.Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Payload tie breaker tidak valid",
                new ErrorDetail("payload.number", "REQUIRED"));
        }

        if (number <= 0)
        {
            return EventDomainValidationResult.Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Nomor tie breaker tidak valid",
                new ErrorDetail("payload.number", "OUT_OF_RANGE"));
        }

        var alreadyAssigned = history.Any(e =>
            e.PlayerId == request.PlayerId &&
            e.ActionType == "tie_breaker.assigned");
        if (alreadyAssigned)
        {
            return EventDomainValidationResult.Fail(
                StatusCodes.Status422UnprocessableEntity,
                "DOMAIN_RULE_VIOLATION",
                "Tie breaker sudah ditetapkan untuk pemain");
        }

        return EventDomainValidationResult.Valid;
    }

    private static EventDomainValidationResult RequirePlayer(EventRequest request)
    {
        if (request.PlayerId is not null)
        {
            return EventDomainValidationResult.Valid;
        }

        return EventDomainValidationResult.Fail(
            StatusCodes.Status400BadRequest,
            "VALIDATION_ERROR",
            "Player wajib diisi",
            new ErrorDetail("player_id", "REQUIRED"));
    }
}
