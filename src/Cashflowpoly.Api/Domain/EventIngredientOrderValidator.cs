using System.Linq;
using System.Text.Json;
using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Contracts;
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
        if (GameActionCatalog.Is(request.ActionType, request.Payload, GameActionCatalog.BahanMasakan))
        {
            result = ValidatePurchase(request, config, history);
            return true;
        }

        if (GameActionCatalog.Is(request.ActionType, request.Payload, GameActionCatalog.IngredientDiscarded))
        {
            result = ValidateDiscard(request, history);
            return true;
        }

        if (GameActionCatalog.Is(request.ActionType, request.Payload, GameActionCatalog.JualMasakan))
        {
            result = ValidateOrderClaim(request, config, history);
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

        var matchedIng = config.Ingredients.FirstOrDefault(i => string.Equals(i.Id, cardId, StringComparison.OrdinalIgnoreCase));
        if (matchedIng is null)
        {
            return Fail(
                StatusCodes.Status422UnprocessableEntity,
                "DOMAIN_RULE_VIOLATION",
                $"Bahan {cardId} tidak terdaftar pada katalog ruleset aktif");
        }

        var modifier = 0;
        foreach (var evt in history)
        {
            var eventPayload = _payloadReader.ReadPayload(evt.Payload);
            if (GameActionCatalog.Is(evt.ActionType, eventPayload, GameActionCatalog.RisikoKehidupan) &&
                _payloadReader.TryGetString(eventPayload, "risk_id", out var riskId))
            {
                var risk = config.LifeRisks.FirstOrDefault(r => string.Equals(r.RiskCode, riskId, StringComparison.OrdinalIgnoreCase));
                if (risk is not null && string.Equals(risk.EffectType, "INGREDIENT_PRICE_MODIFIER", StringComparison.OrdinalIgnoreCase))
                {
                    var startDay = evt.DayIndex;
                    var endDay = startDay + (risk.DurationDays ?? 1) - 1;
                    if (request.DayIndex >= startDay && request.DayIndex <= endDay)
                    {
                        modifier += string.Equals(risk.Direction, "IN", StringComparison.OrdinalIgnoreCase) ? risk.Amount : -risk.Amount;
                    }
                }
            }
        }

        var expectedPrice = Math.Max(0, matchedIng.HargaBeli + modifier);
        if (amount != expectedPrice)
        {
            return Fail(
                StatusCodes.Status422UnprocessableEntity,
                "DOMAIN_RULE_VIOLATION",
                $"Harga pembelian bahan {cardId} ({amount}) tidak sesuai dengan harga katalog modified ({expectedPrice})");
        }

        var inventory = _derivedState.BuildIngredientInventory(history, request.UserId!.Value);
        if (inventory.Total + 1 > config.MaxIngredientTotal)
        {
            return Fail(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Total kartu bahan melebihi batas ruleset");
        }

        var currentSame = inventory.ByCardId.TryGetValue(cardId, out var currentQty) ? currentQty : 0;
        if (currentSame + 1 > config.MaxSameIngredient)
        {
            return Fail(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Jumlah kartu bahan sejenis melebihi batas ruleset");
        }

        return new EventIngredientOrderValidation(EventDomainValidationResult.Valid, amount);
    }

    private EventIngredientOrderValidation ValidateDiscard(EventRequest request, IEnumerable<EventDb> history)
    {
        if (request.UserId is null)
        {
            return Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Player wajib diisi",
                new ErrorDetail("user_id", "REQUIRED"));
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

        var inventory = _derivedState.BuildIngredientInventory(history, request.UserId.Value);
        var currentQty = inventory.ByCardId.TryGetValue(cardId, out var qty) ? qty : 0;
        if (currentQty < amount)
        {
            return Fail(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Jumlah discard melebihi stok bahan");
        }

        return new EventIngredientOrderValidation(EventDomainValidationResult.Valid, null);
    }

    private EventIngredientOrderValidation ValidateOrderClaim(EventRequest request, RulesetConfig config, IEnumerable<EventDb> history)
    {
        if (request.UserId is null)
        {
            return Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Player wajib diisi",
                new ErrorDetail("user_id", "REQUIRED"));
        }

        if (!request.Payload.TryGetProperty("order_card_id", out var orderCardIdProp) ||
            orderCardIdProp.ValueKind != JsonValueKind.String)
        {
            return Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Payload order claim tidak valid",
                new ErrorDetail("payload.order_card_id", "REQUIRED"));
        }

        var orderCardId = orderCardIdProp.GetString()!;
        var order = config.Orders.FirstOrDefault(o => string.Equals(o.Id, orderCardId, StringComparison.OrdinalIgnoreCase));
        if (order is null)
        {
            return Fail(
                StatusCodes.Status422UnprocessableEntity,
                "DOMAIN_RULE_VIOLATION",
                $"Kartu pesanan {orderCardId} tidak terdaftar pada katalog ruleset aktif");
        }

        if (order.CardQty.HasValue)
        {
            var claimedCount = history.Count(e =>
                string.Equals(e.ActionType, GameActionCatalog.JualMasakan, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(
                    _payloadReader.ReadPayload(e.Payload).TryGetProperty("order_card_id", out var idProp) && idProp.ValueKind == JsonValueKind.String ? idProp.GetString() : null,
                    orderCardId,
                    StringComparison.OrdinalIgnoreCase
                ));

            if (claimedCount >= order.CardQty.Value)
            {
                return Fail(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", $"Kartu pesanan {orderCardId} telah mencapai batas kuantitas fisik ({order.CardQty.Value})");
            }
        }

        var requiredCards = new List<string>();
        foreach (var bahanName in order.Bahan)
        {
            var matchedIng = config.Ingredients.FirstOrDefault(i => string.Equals(i.Nama, bahanName, StringComparison.OrdinalIgnoreCase));
            var cardId = matchedIng?.Id ?? bahanName.ToLowerInvariant().Replace(" ", "_");
            requiredCards.Add(cardId);
        }

        var inventory = _derivedState.BuildIngredientInventory(history, request.UserId.Value);
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

        if (request.UserId is null)
        {
            return EventDomainValidationResult.Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Player wajib diisi",
                new ErrorDetail("user_id", "REQUIRED"));
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
