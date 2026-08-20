// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui EventEconomyActionValidator.
using System.Text.Json;
using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Contracts;
using Microsoft.AspNetCore.Http;

namespace Cashflowpoly.Api.Domain;

public sealed record EventEconomyActionValidation(
    EventDomainValidationResult Validation,
    double? OutgoingAmount);

internal sealed class EventEconomyActionValidator : IEventEconomyActionValidator
{
    private static readonly EventPayloadReader _payloadReader = new();
    private const int RulebookGoldCardSupply = 20;

    public bool TryValidate(
        EventRequest request,
        RulesetConfig config,
        IEnumerable<EventDb> history,
        out EventEconomyActionValidation result)
    {
        if (string.Equals(request.ActionType, "CatatTransaksi", StringComparison.OrdinalIgnoreCase))
        {
            result = ValidateTransaction(request);
            return true;
        }

        if (GameActionCatalog.Is(request.ActionType, request.Payload, GameActionCatalog.JumatBerkah))
        {
            result = ValidateFridayDonation(request, config, history);
            return true;
        }

        if (GameActionCatalog.Is(request.ActionType, request.Payload, GameActionCatalog.GoldPriceOpened))
        {
            result = ValidateGoldPrice(request, config, history);
            return true;
        }

        if (GameActionCatalog.Is(request.ActionType, request.Payload, GameActionCatalog.InvestasiEmas) ||
            GameActionCatalog.Is(request.ActionType, request.Payload, GameActionCatalog.JualEmas))
        {
            result = ValidateGoldTrade(request, config, history);
            return true;
        }

        result = new EventEconomyActionValidation(EventDomainValidationResult.Valid, null);
        return false;
    }

    private EventEconomyActionValidation ValidateGoldPrice(
        EventRequest request,
        RulesetConfig config,
        IEnumerable<EventDb> history)
    {
        if (!_payloadReader.TryGetInt32(request.Payload, "gold_price", out var price) || price <= 0)
        {
            return Fail(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Harga emas tidak valid",
                new ErrorDetail("payload.gold_price", "OUT_OF_RANGE"));
        }

        if (config.GoldPrices.All(item => item.UnitPrice != price))
        {
            return Fail(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION",
                "Harga emas harus berasal dari Kartu Harga Emas ruleset");
        }

        var events = history.ToList();
        if (events.Any(e => e.DayIndex == request.DayIndex &&
            GameActionCatalog.Is(e.ActionType, _payloadReader.ReadPayload(e.Payload), GameActionCatalog.GoldPriceOpened)))
        {
            return Fail(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Harga emas hari ini sudah dibuka");
        }

        if (!request.Weekday.Equals("SAT", StringComparison.OrdinalIgnoreCase) &&
            !HasActiveGoldRiskOnDay(request.DayIndex, config, events))
        {
            return Fail(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION",
                "Harga emas hanya dibuka pada Sabtu atau saat efek Risiko Kehidupan emas aktif");
        }

        return new EventEconomyActionValidation(EventDomainValidationResult.Valid, null);
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

        var outgoing = string.Equals(direction, "OUT", StringComparison.OrdinalIgnoreCase) && request.UserId is not null
            ? amount
            : (double?)null;
        return new EventEconomyActionValidation(EventDomainValidationResult.Valid, outgoing);
    }

    private EventEconomyActionValidation ValidateFridayDonation(
        EventRequest request,
        RulesetConfig config,
        IEnumerable<EventDb> history)
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

        if (request.UserId.HasValue && history.Any(e =>
                e.UserId == request.UserId &&
                e.DayIndex == request.DayIndex &&
                GameActionCatalog.Is(e.ActionType, _payloadReader.ReadPayload(e.Payload), GameActionCatalog.JumatBerkah)))
        {
            return Fail(StatusCodes.Status422UnprocessableEntity, "DONATION_ALREADY_SUBMITTED", "Donasi Jumat sudah dikirim pemain pada hari ini");
        }

        return new EventEconomyActionValidation(EventDomainValidationResult.Valid, request.UserId is null ? null : amount);
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

        var referencedGoldRiskIsActive = HasActiveReferencedGoldRisk(request, config, history);
        if (!string.Equals(request.Weekday, "SAT", StringComparison.OrdinalIgnoreCase) &&
            !referencedGoldRiskIsActive)
        {
            return Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Weekday harus SAT",
                new ErrorDetail("weekday", "INVALID_VALUE"));
        }

        var canonicalAction = GameActionCatalog.ResolveGameActionId(request.ActionType, request.Payload);
        var hasFullGoldPayload = _payloadReader.TryReadGoldTrade(request.Payload, out var tradeType, out var qty, out var unitPrice, out var amount);
        if (!hasFullGoldPayload)
        {
            if (!_payloadReader.TryGetInt32(request.Payload, "qty", out qty) ||
                !_payloadReader.TryGetInt32(request.Payload, "unit_price", out unitPrice) ||
                !_payloadReader.TryGetInt32(request.Payload, "amount", out amount))
            {
                return Fail(
                    StatusCodes.Status400BadRequest,
                    "VALIDATION_ERROR",
                    "Payload gold trade tidak valid",
                    new ErrorDetail("payload", "INVALID_STRUCTURE"));
            }

            tradeType = string.Equals(canonicalAction, GameActionCatalog.JualEmas, StringComparison.OrdinalIgnoreCase)
                ? "SELL"
                : "BUY";
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

        if (string.Equals(canonicalAction, GameActionCatalog.InvestasiEmas, StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(tradeType, "BUY", StringComparison.OrdinalIgnoreCase))
        {
            return Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "InvestasiEmas wajib memakai trade_type BUY",
                new ErrorDetail("payload.trade_type", "INVALID_ENUM"));
        }

        if (string.Equals(canonicalAction, GameActionCatalog.JualEmas, StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(tradeType, "SELL", StringComparison.OrdinalIgnoreCase))
        {
            return Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "JualEmas wajib memakai trade_type SELL",
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

        var activePriceEvent = history
            .Where(e => GameActionCatalog.Is(e.ActionType, _payloadReader.ReadPayload(e.Payload), GameActionCatalog.GoldPriceOpened) &&
                        e.DayIndex == request.DayIndex)
            .OrderByDescending(e => e.SequenceNumber)
            .FirstOrDefault();
        if (activePriceEvent is null ||
            !_payloadReader.TryGetInt32(_payloadReader.ReadPayload(activePriceEvent.Payload), "gold_price", out var activePrice) ||
            activePrice != unitPrice)
        {
            return Fail(
                StatusCodes.Status422UnprocessableEntity,
                "DOMAIN_RULE_VIOLATION",
                "Harga emas harus berasal dari BukaHargaEmas pada hari transaksi");
        }

        var goldTradeOpened = false;
        foreach (var evt in history)
        {
            var eventPayload = _payloadReader.ReadPayload(evt.Payload);
            if (GameActionCatalog.Is(evt.ActionType, eventPayload, GameActionCatalog.RisikoKehidupan) &&
                _payloadReader.TryGetString(eventPayload, "risk_id", out var riskId))
            {
                var risk = config.LifeRisks.FirstOrDefault(r => string.Equals(r.RiskCode, riskId, StringComparison.OrdinalIgnoreCase));
                if (risk is not null && string.Equals(risk.EffectType, "GOLD_TRADE", StringComparison.OrdinalIgnoreCase))
                {
                    var startDay = evt.DayIndex;
                    var endDay = startDay + (risk.DurationDays ?? 1) - 1;
                    if (request.DayIndex >= startDay && request.DayIndex <= endDay)
                    {
                        goldTradeOpened = true;
                        break;
                    }
                }
            }
        }

        if (string.Equals(tradeType, "BUY", StringComparison.OrdinalIgnoreCase) && !config.GoldAllowBuy && !goldTradeOpened)
        {
            return Fail(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Ruleset melarang BUY emas");
        }

        if (string.Equals(tradeType, "SELL", StringComparison.OrdinalIgnoreCase) && !config.GoldAllowSell && !goldTradeOpened)
        {
            return Fail(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Ruleset melarang SELL emas");
        }

        if (string.Equals(tradeType, "BUY", StringComparison.OrdinalIgnoreCase))
        {
            var cardsHeld = new GoldGameplayCalculator().Compute(history).GoldHeldEnd;
            if (cardsHeld + qty > RulebookGoldCardSupply)
            {
                return Fail(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION",
                    $"Stok fisik Kartu Emas tidak cukup; tersedia {Math.Max(0, RulebookGoldCardSupply - cardsHeld)} kartu");
            }
        }

        var outgoing = string.Equals(tradeType, "BUY", StringComparison.OrdinalIgnoreCase) && request.UserId is not null
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

    private bool HasActiveReferencedGoldRisk(
        EventRequest request,
        RulesetConfig config,
        IEnumerable<EventDb> history)
    {
        if (!_payloadReader.TryGetString(request.Payload, "risk_event_id", out var riskEventIdText) ||
            !Guid.TryParse(riskEventIdText, out var riskEventId))
        {
            return false;
        }

        var riskEvent = history.FirstOrDefault(e => e.EventId == riskEventId && e.SessionId == request.SessionId);
        if (riskEvent is null ||
            !GameActionCatalog.Is(riskEvent.ActionType, _payloadReader.ReadPayload(riskEvent.Payload), GameActionCatalog.RisikoKehidupan))
        {
            return false;
        }

        var riskPayload = _payloadReader.ReadPayload(riskEvent.Payload);
        if (!_payloadReader.TryGetString(riskPayload, "risk_id", out var riskId))
        {
            return false;
        }

        var risk = config.LifeRisks.FirstOrDefault(item =>
            string.Equals(item.RiskCode, riskId, StringComparison.OrdinalIgnoreCase));
        var endDay = riskEvent.DayIndex + (risk?.DurationDays ?? 1) - 1;
        return risk is not null &&
               string.Equals(risk.EffectType, "GOLD_TRADE", StringComparison.OrdinalIgnoreCase) &&
               request.DayIndex >= riskEvent.DayIndex &&
               request.DayIndex <= endDay;
    }

    private static bool HasActiveGoldRiskOnDay(
        int dayIndex,
        RulesetConfig config,
        IEnumerable<EventDb> history)
    {
        foreach (var evt in history)
        {
            var payload = _payloadReader.ReadPayload(evt.Payload);
            if (!GameActionCatalog.Is(evt.ActionType, payload, GameActionCatalog.RisikoKehidupan) ||
                !_payloadReader.TryGetString(payload, "risk_id", out var riskId))
            {
                continue;
            }

            var risk = config.LifeRisks.FirstOrDefault(item =>
                string.Equals(item.RiskCode, riskId, StringComparison.OrdinalIgnoreCase));
            var endDay = evt.DayIndex + (risk?.DurationDays ?? 1) - 1;
            if (risk is not null &&
                string.Equals(risk.EffectType, "GOLD_TRADE", StringComparison.OrdinalIgnoreCase) &&
                dayIndex >= evt.DayIndex && dayIndex <= endDay)
            {
                return true;
            }
        }

        return false;
    }
}
