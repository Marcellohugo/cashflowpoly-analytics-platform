// Fungsi file: Memvalidasi action event sederhana yang tidak membutuhkan histori event atau query database.
using Cashflowpoly.Contracts;
using Microsoft.AspNetCore.Http;
using static Cashflowpoly.Api.Domain.EventPayloadReader;

namespace Cashflowpoly.Api.Domain;

internal static class EventSimpleActionValidator
{
    internal static bool TryValidate(
        EventRequest request,
        RulesetConfig config,
        out EventDomainValidationResult result)
    {
        var actionType = request.ActionType;
        var payload = request.Payload;

        if (string.Equals(actionType, "order.passed", StringComparison.OrdinalIgnoreCase))
        {
            result = ValidateOrderPassed(request, payload);
            return true;
        }

        if (string.Equals(actionType, "work.freelance.completed", StringComparison.OrdinalIgnoreCase))
        {
            result = ValidateFreelanceCompleted(request, payload, config.FreelanceIncome);
            return true;
        }

        if (string.Equals(actionType, "donation.rank.awarded", StringComparison.OrdinalIgnoreCase))
        {
            result = ValidateRankAwarded(request, payload, "Payload donasi tidak valid");
            return true;
        }

        if (string.Equals(actionType, "gold.points.awarded", StringComparison.OrdinalIgnoreCase))
        {
            result = ValidateGoldPointsAwarded(request, payload);
            return true;
        }

        if (string.Equals(actionType, "pension.rank.awarded", StringComparison.OrdinalIgnoreCase))
        {
            result = ValidateRankAwarded(request, payload, "Payload pension tidak valid");
            return true;
        }

        result = EventDomainValidationResult.Valid;
        return false;
    }

    private static EventDomainValidationResult ValidateOrderPassed(EventRequest request, System.Text.Json.JsonElement payload)
    {
        var playerCheck = RequirePlayer(request);
        if (!playerCheck.IsValid)
        {
            return playerCheck;
        }

        if (!TryReadOrderClaim(payload, out _, out var income))
        {
            return EventDomainValidationResult.Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Payload order pass tidak valid",
                new ErrorDetail("payload.required_ingredient_card_ids", "REQUIRED"));
        }

        if (income <= 0)
        {
            return EventDomainValidationResult.Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Income harus > 0",
                new ErrorDetail("payload.income", "OUT_OF_RANGE"));
        }

        return EventDomainValidationResult.Valid;
    }

    private static EventDomainValidationResult ValidateFreelanceCompleted(
        EventRequest request,
        System.Text.Json.JsonElement payload,
        int expectedIncome)
    {
        var playerCheck = RequirePlayer(request);
        if (!playerCheck.IsValid)
        {
            return playerCheck;
        }

        if (!TryReadAmount(payload, out var amount))
        {
            return EventDomainValidationResult.Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Payload kerja lepas tidak valid",
                new ErrorDetail("payload.amount", "REQUIRED"));
        }

        if (amount <= 0)
        {
            return EventDomainValidationResult.Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Amount harus > 0",
                new ErrorDetail("payload.amount", "OUT_OF_RANGE"));
        }

        var rounded = (int)Math.Round(amount);
        if (rounded != expectedIncome)
        {
            return EventDomainValidationResult.Fail(
                StatusCodes.Status422UnprocessableEntity,
                "DOMAIN_RULE_VIOLATION",
                "Amount kerja lepas tidak sesuai ruleset");
        }

        return EventDomainValidationResult.Valid;
    }

    private static EventDomainValidationResult ValidateRankAwarded(
        EventRequest request,
        System.Text.Json.JsonElement payload,
        string invalidPayloadMessage)
    {
        var playerCheck = RequirePlayer(request);
        if (!playerCheck.IsValid)
        {
            return playerCheck;
        }

        if (!TryReadRankAwarded(payload, out var rank, out var points))
        {
            return EventDomainValidationResult.Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                invalidPayloadMessage,
                new ErrorDetail("payload.rank", "REQUIRED"));
        }

        if (rank <= 0 || points < 0)
        {
            return EventDomainValidationResult.Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Nilai rank/points tidak valid",
                new ErrorDetail("payload.points", "OUT_OF_RANGE"));
        }

        return EventDomainValidationResult.Valid;
    }

    private static EventDomainValidationResult ValidateGoldPointsAwarded(
        EventRequest request,
        System.Text.Json.JsonElement payload)
    {
        var playerCheck = RequirePlayer(request);
        if (!playerCheck.IsValid)
        {
            return playerCheck;
        }

        if (!TryReadPointsAwarded(payload, out var points))
        {
            return EventDomainValidationResult.Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Payload gold points tidak valid",
                new ErrorDetail("payload.points", "REQUIRED"));
        }

        if (points < 0)
        {
            return EventDomainValidationResult.Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Points tidak valid",
                new ErrorDetail("payload.points", "OUT_OF_RANGE"));
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
