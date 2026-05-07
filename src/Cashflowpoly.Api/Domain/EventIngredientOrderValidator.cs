// Fungsi file: Memvalidasi event ingredient dan order berbasis inventory histori event.
using Cashflowpoly.Api.Data;
using Cashflowpoly.Contracts;
using Microsoft.AspNetCore.Http;

namespace Cashflowpoly.Api.Domain;

public sealed record EventIngredientOrderValidation(
    EventDomainValidationResult Validation,
    int? OutgoingAmount);

internal sealed class EventIngredientOrderValidator : IEventIngredientOrderValidator
{
    private static readonly EventPayloadReader _payloadReader = new();
    private static readonly EventDerivedStateCalculator _derivedState = new();

    public bool TryValidate(
        EventRequest request,
        RulesetConfig config,
        IEnumerable<EventDb> history,
        out EventIngredientOrderValidation result)
    {
        if (string.Equals(request.ActionType, "ingredient.purchased", StringComparison.OrdinalIgnoreCase))
        {
            result = ValidatePurchase(request, config, history);
            return true;
        }

        if (string.Equals(request.ActionType, "ingredient.discarded", StringComparison.OrdinalIgnoreCase))
        {
            result = ValidateDiscard(request, history);
            return true;
        }

        if (string.Equals(request.ActionType, "order.claimed", StringComparison.OrdinalIgnoreCase))
        {
            result = ValidateOrderClaim(request, history);
            return true;
        }

        result = new EventIngredientOrderValidation(EventDomainValidationResult.Valid, null);
        return false;
    }

    private EventIngredientOrderValidation ValidatePurchase(
        EventRequest request,
        RulesetConfig config,
        IEnumerable<EventDb> history)
    {
        if (!_payloadReader.TryReadIngredientPurchase(request.Payload, out var cardId, out var amount))
        {
            return Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Payload ingredient tidak valid",
                new ErrorDetail("payload.card_id", "REQUIRED"));
        }

        var commonValidation = ValidatePositiveAmountAndPlayer(request, amount);
        if (!commonValidation.IsValid)
        {
            return new EventIngredientOrderValidation(commonValidation, null);
        }

        var inventory = _derivedState.BuildIngredientInventory(history, request.PlayerId!.Value);
        if (inventory.Total + amount > config.MaxIngredientTotal)
        {
            return Fail(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Total kartu bahan melebihi batas ruleset");
        }

        var currentSame = inventory.ByCardId.TryGetValue(cardId, out var currentQty) ? currentQty : 0;
        if (currentSame + amount > config.MaxSameIngredient)
        {
            return Fail(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Jumlah kartu bahan sejenis melebihi batas ruleset");
        }

        return new EventIngredientOrderValidation(EventDomainValidationResult.Valid, amount);
    }

    private EventIngredientOrderValidation ValidateDiscard(EventRequest request, IEnumerable<EventDb> history)
    {
        if (request.PlayerId is null)
        {
            return Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Player wajib diisi",
                new ErrorDetail("player_id", "REQUIRED"));
        }

        if (!_payloadReader.TryReadIngredientPurchase(request.Payload, out var cardId, out var amount))
        {
            return Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Payload discard ingredient tidak valid",
                new ErrorDetail("payload.card_id", "REQUIRED"));
        }

        var amountValidation = ValidatePositiveAmount(amount);
        if (!amountValidation.IsValid)
        {
            return new EventIngredientOrderValidation(amountValidation, null);
        }

        var inventory = _derivedState.BuildIngredientInventory(history, request.PlayerId.Value);
        var currentQty = inventory.ByCardId.TryGetValue(cardId, out var qty) ? qty : 0;
        if (currentQty < amount)
        {
            return Fail(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Jumlah discard melebihi stok bahan");
        }

        return new EventIngredientOrderValidation(EventDomainValidationResult.Valid, null);
    }

    private EventIngredientOrderValidation ValidateOrderClaim(EventRequest request, IEnumerable<EventDb> history)
    {
        if (request.PlayerId is null)
        {
            return Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Player wajib diisi",
                new ErrorDetail("player_id", "REQUIRED"));
        }

        if (!_payloadReader.TryReadOrderClaim(request.Payload, out var requiredCards, out var income))
        {
            return Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Payload order claim tidak valid",
                new ErrorDetail("payload.required_ingredient_card_ids", "REQUIRED"));
        }

        if (income <= 0)
        {
            return Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Income harus > 0",
                new ErrorDetail("payload.income", "OUT_OF_RANGE"));
        }

        var inventory = _derivedState.BuildIngredientInventory(history, request.PlayerId.Value);
        foreach (var card in requiredCards)
        {
            if (!inventory.ByCardId.TryGetValue(card, out var qty) || qty <= 0)
            {
                return Fail(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Bahan tidak mencukupi untuk klaim order");
            }

            inventory.ByCardId[card] = qty - 1;
        }

        return new EventIngredientOrderValidation(EventDomainValidationResult.Valid, null);
    }

    private EventDomainValidationResult ValidatePositiveAmountAndPlayer(EventRequest request, int amount)
    {
        var amountValidation = ValidatePositiveAmount(amount);
        if (!amountValidation.IsValid)
        {
            return amountValidation;
        }

        if (request.PlayerId is null)
        {
            return EventDomainValidationResult.Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Player wajib diisi",
                new ErrorDetail("player_id", "REQUIRED"));
        }

        return EventDomainValidationResult.Valid;
    }

    private EventDomainValidationResult ValidatePositiveAmount(int amount)
    {
        if (amount > 0)
        {
            return EventDomainValidationResult.Valid;
        }

        return EventDomainValidationResult.Fail(
            StatusCodes.Status400BadRequest,
            "VALIDATION_ERROR",
            "Amount harus > 0",
            new ErrorDetail("payload.amount", "OUT_OF_RANGE"));
    }

    private EventIngredientOrderValidation Fail(
        int statusCode,
        string errorCode,
        string message,
        params ErrorDetail[] details)
    {
        return new EventIngredientOrderValidation(
            EventDomainValidationResult.Fail(statusCode, errorCode, message, details),
            null);
    }
}
