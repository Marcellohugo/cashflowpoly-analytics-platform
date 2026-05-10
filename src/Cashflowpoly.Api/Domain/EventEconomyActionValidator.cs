using Cashflowpoly.Api.Data;
using Cashflowpoly.Contracts;
using Microsoft.AspNetCore.Http;

namespace Cashflowpoly.Api.Domain;

public sealed record EventEconomyActionValidation(
    EventDomainValidationResult Validation,
    double? OutgoingAmount);

internal sealed class EventEconomyActionValidator : IEventEconomyActionValidator
{
    private static readonly EventPayloadReader _payloadReader = new();

    public bool TryValidate(
        EventRequest request,
        RulesetConfig config,
        IEnumerable<EventDb> history,
        out EventEconomyActionValidation result)
    {
        if (string.Equals(request.ActionType, "transaction.recorded", StringComparison.OrdinalIgnoreCase))
        {
            result = ValidateTransaction(request);
            return true;
        }

        if (string.Equals(request.ActionType, "day.friday.donation", StringComparison.OrdinalIgnoreCase))
        {
            result = ValidateFridayDonation(request, config);
            return true;
        }

        if (string.Equals(request.ActionType, "day.saturday.gold_trade", StringComparison.OrdinalIgnoreCase))
        {
            result = ValidateGoldTrade(request, config, history);
            return true;
        }

        result = new EventEconomyActionValidation(EventDomainValidationResult.Valid, null);
        return false;
    }

    private EventEconomyActionValidation ValidateTransaction(EventRequest request)
    {
        if (!_payloadReader.TryReadTransaction(request.Payload, out var direction, out var amount, out var category, out var counterparty))
        {
            return Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Payload transaksi tidak valid",
                new ErrorDetail("payload", "INVALID_STRUCTURE"));
        }

        if (!string.Equals(direction, "IN", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(direction, "OUT", StringComparison.OrdinalIgnoreCase))
        {
            return Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Direction tidak valid",
                new ErrorDetail("payload.direction", "INVALID_ENUM"));
        }

        if (amount <= 0)
        {
            return Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Amount harus > 0",
                new ErrorDetail("payload.amount", "OUT_OF_RANGE"));
        }

        if (string.IsNullOrWhiteSpace(category))
        {
            return Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Category wajib diisi",
                new ErrorDetail("payload.category", "REQUIRED"));
        }

        if (!string.IsNullOrWhiteSpace(counterparty) &&
            !string.Equals(counterparty, "BANK", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(counterparty, "PLAYER", StringComparison.OrdinalIgnoreCase))
        {
            return Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Counterparty tidak valid",
                new ErrorDetail("payload.counterparty", "INVALID_ENUM"));
        }

        var outgoing = string.Equals(direction, "OUT", StringComparison.OrdinalIgnoreCase) && request.PlayerId is not null
            ? amount
            : (double?)null;
        return new EventEconomyActionValidation(EventDomainValidationResult.Valid, outgoing);
    }

    private EventEconomyActionValidation ValidateFridayDonation(EventRequest request, RulesetConfig config)
    {
        if (!config.FridayEnabled)
        {
            return Fail(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Fitur donasi Jumat tidak aktif");
        }

        if (!string.Equals(request.Weekday, "FRI", StringComparison.OrdinalIgnoreCase))
        {
            return Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Weekday harus FRI",
                new ErrorDetail("weekday", "INVALID_VALUE"));
        }

        if (!_payloadReader.TryReadAmount(request.Payload, out var amount))
        {
            return Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Payload donasi tidak valid",
                new ErrorDetail("payload.amount", "REQUIRED"));
        }

        if (amount < config.DonationMin || amount > config.DonationMax)
        {
            return Fail(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Jumlah donasi di luar batas");
        }

        return new EventEconomyActionValidation(EventDomainValidationResult.Valid, request.PlayerId is null ? null : amount);
    }

    private EventEconomyActionValidation ValidateGoldTrade(
        EventRequest request,
        RulesetConfig config,
        IEnumerable<EventDb> history)
    {
        if (!config.SaturdayEnabled)
        {
            return Fail(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Fitur perdagangan emas tidak aktif");
        }

        if (!string.Equals(request.Weekday, "SAT", StringComparison.OrdinalIgnoreCase))
        {
            return Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Weekday harus SAT",
                new ErrorDetail("weekday", "INVALID_VALUE"));
        }

        if (!_payloadReader.TryReadGoldTrade(request.Payload, out var tradeType, out var qty, out var unitPrice, out var amount))
        {
            return Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Payload gold trade tidak valid",
                new ErrorDetail("payload", "INVALID_STRUCTURE"));
        }

        if (!string.Equals(tradeType, "BUY", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(tradeType, "SELL", StringComparison.OrdinalIgnoreCase))
        {
            return Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Trade type tidak valid",
                new ErrorDetail("payload.trade_type", "INVALID_ENUM"));
        }

        if (qty <= 0 || unitPrice <= 0 || amount <= 0)
        {
            return Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Nilai qty/unit_price/amount tidak valid",
                new ErrorDetail("payload.amount", "OUT_OF_RANGE"));
        }

        if (amount != unitPrice * qty)
        {
            return Fail(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Amount tidak sesuai unit_price * qty");
        }

        if (string.Equals(tradeType, "BUY", StringComparison.OrdinalIgnoreCase) && !config.GoldAllowBuy)
        {
            return Fail(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Ruleset melarang BUY emas");
        }

        if (string.Equals(tradeType, "SELL", StringComparison.OrdinalIgnoreCase) && !config.GoldAllowSell)
        {
            return Fail(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Ruleset melarang SELL emas");
        }

        if (string.Equals(tradeType, "SELL", StringComparison.OrdinalIgnoreCase) && request.PlayerId is not null)
        {
            var goldQty = 0;
            foreach (var evt in history.Where(e =>
                         e.PlayerId == request.PlayerId &&
                         e.ActionType == "day.saturday.gold_trade"))
            {
                if (!_payloadReader.TryReadGoldTrade(_payloadReader.ReadPayload(evt.Payload), out var evtTradeType, out var evtQty, out _, out _))
                {
                    continue;
                }

                goldQty += string.Equals(evtTradeType, "BUY", StringComparison.OrdinalIgnoreCase) ? evtQty : -evtQty;
            }

            if (goldQty < qty)
            {
                return Fail(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Kepemilikan emas tidak mencukupi");
            }
        }

        var outgoing = string.Equals(tradeType, "BUY", StringComparison.OrdinalIgnoreCase) && request.PlayerId is not null
            ? amount
            : (double?)null;
        return new EventEconomyActionValidation(EventDomainValidationResult.Valid, outgoing);
    }

    private EventEconomyActionValidation Fail(
        int statusCode,
        string errorCode,
        string message,
        params ErrorDetail[] details)
    {
        return new EventEconomyActionValidation(
            EventDomainValidationResult.Fail(statusCode, errorCode, message, details),
            null);
    }
}
