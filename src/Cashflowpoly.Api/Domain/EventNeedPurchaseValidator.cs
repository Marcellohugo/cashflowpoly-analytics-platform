using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Contracts;
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
        if (!GameActionCatalog.Is(request.ActionType, request.Payload, GameActionCatalog.Kebutuhan))
        {
            result = new EventNeedPurchaseValidation(EventDomainValidationResult.Valid, null);
            return false;
        }

        if (!_payloadReader.TryReadNeedPurchase(request.Payload, out var cardId, out _, out _))
        {
            var payloadValidation = ValidatePayload(request, primary: false, out _, out _);
            result = new EventNeedPurchaseValidation(payloadValidation, null);
            return true;
        }

        var needTier = NeedTierClassifier.FromPayload(request.Payload, cardId);
        result = needTier == NeedTier.Primary
            ? ValidatePrimary(request, config, history)
            : ValidateSecondaryOrTertiary(request, config, history);
        return true;
    }

    private EventNeedPurchaseValidation ValidatePrimary(
        EventRequest request,
        RulesetConfig config,
        IEnumerable<EventDb> history)
    {
        var payloadValidation = ValidatePayload(request, primary: true, out _, out var amount);
        if (!payloadValidation.IsValid)
        {
            return new EventNeedPurchaseValidation(payloadValidation, null);
        }

        if (request.UserId is null)
        {
            return Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Player wajib diisi",
                new ErrorDetail("user_id", "REQUIRED"));
        }

        var primaryCount = history.Count(e =>
            e.UserId == request.UserId &&
            e.DayIndex == request.DayIndex &&
            GameActionCatalog.Is(e.ActionType, _payloadReader.ReadPayload(e.Payload), GameActionCatalog.Kebutuhan) &&
            NeedTierClassifier.FromPayloadJson(e.Payload) == NeedTier.Primary);

        if (config.PrimaryNeedMaxPerDay is > 0 && primaryCount >= config.PrimaryNeedMaxPerDay.Value)
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
        var payloadValidation = ValidatePayload(request, primary: false, out _, out var amount);
        if (!payloadValidation.IsValid)
        {
            return new EventNeedPurchaseValidation(payloadValidation, null);
        }

        if (config.RequirePrimaryBeforeOthers && request.UserId is not null)
        {
            var hasPrimary = history.Any(e =>
                e.UserId == request.UserId &&
                GameActionCatalog.Is(e.ActionType, _payloadReader.ReadPayload(e.Payload), GameActionCatalog.Kebutuhan) &&
                NeedTierClassifier.FromPayloadJson(e.Payload) == NeedTier.Primary);

            if (!hasPrimary)
            {
                return Fail(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Kebutuhan primer harus dibeli terlebih dahulu");
            }
        }

        return new EventNeedPurchaseValidation(EventDomainValidationResult.Valid, request.UserId is null ? null : amount);
    }

    private EventDomainValidationResult ValidatePayload(EventRequest request, bool primary, out string cardId, out int amount)
    {
        cardId = string.Empty;
        amount = 0;
        if (!_payloadReader.TryReadNeedPurchase(request.Payload, out cardId, out amount, out var points))
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

        if (NeedTierClassifier.FromPayload(request.Payload, cardId) == NeedTier.Unknown)
        {
            return EventDomainValidationResult.Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Tipe kebutuhan tidak valid",
                new ErrorDetail("payload.need_tier", "INVALID_ENUM"));
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
