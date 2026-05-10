using Cashflowpoly.Api.Data;
using Cashflowpoly.Contracts;
using Microsoft.AspNetCore.Http;

namespace Cashflowpoly.Api.Domain;

public sealed record EventNeedPurchaseValidation(
    EventDomainValidationResult Validation,
    int? OutgoingAmount);

internal sealed class EventNeedPurchaseValidator : IEventNeedPurchaseValidator
{
    private static readonly EventPayloadReader _payloadReader = new();

    public bool TryValidate(
        EventRequest request,
        RulesetConfig config,
        IEnumerable<EventDb> history,
        out EventNeedPurchaseValidation result)
    {
        if (string.Equals(request.ActionType, "need.primary.purchased", StringComparison.OrdinalIgnoreCase))
        {
            result = ValidatePrimary(request, config, history);
            return true;
        }

        if (string.Equals(request.ActionType, "need.secondary.purchased", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(request.ActionType, "need.tertiary.purchased", StringComparison.OrdinalIgnoreCase))
        {
            result = ValidateSecondaryOrTertiary(request, config, history);
            return true;
        }

        result = new EventNeedPurchaseValidation(EventDomainValidationResult.Valid, null);
        return false;
    }

    private EventNeedPurchaseValidation ValidatePrimary(
        EventRequest request,
        RulesetConfig config,
        IEnumerable<EventDb> history)
    {
        var payloadValidation = ValidatePayload(request, primary: true, out var amount);
        if (!payloadValidation.IsValid)
        {
            return new EventNeedPurchaseValidation(payloadValidation, null);
        }

        if (config.PrimaryNeedMaxPerDay == 0)
        {
            return Fail(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Ruleset melarang pembelian kebutuhan primer");
        }

        if (request.PlayerId is null)
        {
            return Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Player wajib diisi",
                new ErrorDetail("player_id", "REQUIRED"));
        }

        var primaryCount = history.Count(e =>
            e.PlayerId == request.PlayerId &&
            e.DayIndex == request.DayIndex &&
            e.ActionType == "need.primary.purchased");

        if (primaryCount >= config.PrimaryNeedMaxPerDay)
        {
            return Fail(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Pembelian kebutuhan primer melebihi batas harian");
        }

        return new EventNeedPurchaseValidation(EventDomainValidationResult.Valid, amount);
    }

    private EventNeedPurchaseValidation ValidateSecondaryOrTertiary(
        EventRequest request,
        RulesetConfig config,
        IEnumerable<EventDb> history)
    {
        var payloadValidation = ValidatePayload(request, primary: false, out var amount);
        if (!payloadValidation.IsValid)
        {
            return new EventNeedPurchaseValidation(payloadValidation, null);
        }

        if (config.RequirePrimaryBeforeOthers && request.PlayerId is not null)
        {
            var hasPrimary = history.Any(e =>
                e.PlayerId == request.PlayerId &&
                e.DayIndex == request.DayIndex &&
                e.ActionType == "need.primary.purchased");

            if (!hasPrimary)
            {
                return Fail(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Kebutuhan primer harus dibeli terlebih dahulu");
            }
        }

        return new EventNeedPurchaseValidation(EventDomainValidationResult.Valid, request.PlayerId is null ? null : amount);
    }

    private EventDomainValidationResult ValidatePayload(EventRequest request, bool primary, out int amount)
    {
        amount = 0;
        if (!_payloadReader.TryReadNeedPurchase(request.Payload, out var cardId, out amount, out var points))
        {
            return EventDomainValidationResult.Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                primary ? "Payload kebutuhan primer tidak valid" : "Payload kebutuhan tidak valid",
                new ErrorDetail("payload.card_id", "REQUIRED"));
        }

        if (amount <= 0)
        {
            return EventDomainValidationResult.Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Amount harus > 0",
                new ErrorDetail("payload.amount", "OUT_OF_RANGE"));
        }

        if (string.IsNullOrWhiteSpace(cardId))
        {
            return EventDomainValidationResult.Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Card ID wajib diisi",
                new ErrorDetail("payload.card_id", "REQUIRED"));
        }

        if (points < 0)
        {
            return EventDomainValidationResult.Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Points tidak valid",
                new ErrorDetail("payload.points", "OUT_OF_RANGE"));
        }

        if (!request.Payload.TryGetProperty("points", out _))
        {
            return EventDomainValidationResult.Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Points wajib diisi",
                new ErrorDetail("payload.points", "REQUIRED"));
        }

        return EventDomainValidationResult.Valid;
    }

    private EventNeedPurchaseValidation Fail(
        int statusCode,
        string errorCode,
        string message,
        params ErrorDetail[] details)
    {
        return new EventNeedPurchaseValidation(
            EventDomainValidationResult.Fail(statusCode, errorCode, message, details),
            null);
    }
}
