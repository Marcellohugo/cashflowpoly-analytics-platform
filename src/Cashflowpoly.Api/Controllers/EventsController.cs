using System.Text.Json;
using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Domain;
using Cashflowpoly.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace Cashflowpoly.Api.Controllers;

[ApiController]
[Route("api")]
public sealed class EventsController : ControllerBase
{
    private sealed record ValidationOutcome(bool IsValid, int StatusCode, ErrorResponse? Error);

    private static readonly ValidationOutcome Valid = new(true, StatusCodes.Status200OK, null);

    private readonly SessionRepository _sessions;
    private readonly RulesetRepository _rulesets;
    private readonly EventRepository _events;

    public EventsController(SessionRepository sessions, RulesetRepository rulesets, EventRepository events)
    {
        _sessions = sessions;
        _rulesets = rulesets;
        _events = events;
    }

    private static readonly HashSet<string> AllowedActorTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "PLAYER",
        "SYSTEM"
    };

    private static readonly HashSet<string> AllowedWeekdays = new(StringComparer.OrdinalIgnoreCase)
    {
        "MON", "TUE", "WED", "THU", "FRI", "SAT", "SUN"
    };

    [HttpPost("events")]
    public async Task<IActionResult> CreateEvent([FromBody] EventRequest request, CancellationToken ct)
    {
        var validation = await ValidateEventAsync(request, ct);
        if (!validation.IsValid)
        {
            await _events.InsertValidationLogAsync(
                request.SessionId,
                request.EventId,
                null,
                false,
                validation.Error?.ErrorCode,
                validation.Error?.Message,
                BuildValidationDetailsJson(request, validation.Error),
                ct);

            return StatusCode(validation.StatusCode, validation.Error);
        }

        try
        {
            var eventPk = await StoreEventAsync(request, ct);
            await _events.InsertValidationLogAsync(request.SessionId, request.EventId, eventPk, true, null, null, null, ct);
            return StatusCode(StatusCodes.Status201Created, new EventStoredResponse(true, request.EventId));
        }
        catch (PostgresException ex) when (ex.SqlState == "23505")
        {
            var error = ApiErrorHelper.BuildError(HttpContext, "DUPLICATE", "Event sudah ada");
            await _events.InsertValidationLogAsync(request.SessionId, request.EventId, null, false, error.ErrorCode, error.Message,
                BuildValidationDetailsJson(request, error), ct);
            return Conflict(error);
        }
    }

    [HttpPost("events/batch")]
    public async Task<IActionResult> CreateEventsBatch([FromBody] EventBatchRequest request, CancellationToken ct)
    {
        var failed = new List<EventBatchFailed>();
        var storedCount = 0;

        foreach (var evt in request.Events)
        {
            var validation = await ValidateEventAsync(evt, ct);
            if (!validation.IsValid)
            {
                failed.Add(new EventBatchFailed(evt.EventId, validation.Error?.ErrorCode ?? "VALIDATION_ERROR"));
                await _events.InsertValidationLogAsync(
                    evt.SessionId,
                    evt.EventId,
                    null,
                    false,
                    validation.Error?.ErrorCode,
                    validation.Error?.Message,
                    BuildValidationDetailsJson(evt, validation.Error),
                    ct);
                continue;
            }

            try
            {
                var eventPk = await StoreEventAsync(evt, ct);
                await _events.InsertValidationLogAsync(evt.SessionId, evt.EventId, eventPk, true, null, null, null, ct);
                storedCount++;
            }
            catch (PostgresException ex) when (ex.SqlState == "23505")
            {
                failed.Add(new EventBatchFailed(evt.EventId, "DUPLICATE"));
            }
        }

        return Ok(new EventBatchResponse(storedCount, failed));
    }

    [HttpGet("sessions/{sessionId:guid}/events")]
    public async Task<IActionResult> GetEventsBySession(Guid sessionId, [FromQuery] long fromSeq = 0, [FromQuery] int limit = 200, CancellationToken ct = default)
    {
        var session = await _sessions.GetSessionAsync(sessionId, ct);
        if (session is null)
        {
            return NotFound(ApiErrorHelper.BuildError(HttpContext, "NOT_FOUND", "Session tidak ditemukan"));
        }

        var events = await _events.GetEventsBySessionAsync(sessionId, fromSeq, limit, ct);
        var responseEvents = events.Select(ToEventRequest).ToList();
        return Ok(new EventsBySessionResponse(sessionId, responseEvents));
    }

    private async Task<ValidationOutcome> ValidateEventAsync(EventRequest request, CancellationToken ct)
    {
        var session = await _sessions.GetSessionAsync(request.SessionId, ct);
        if (session is null)
        {
            return BuildOutcome(StatusCodes.Status404NotFound, "NOT_FOUND", "Session tidak ditemukan");
        }

        var rulesetVersion = await _rulesets.GetRulesetVersionByIdAsync(request.RulesetVersionId, ct);
        if (rulesetVersion is null)
        {
            return BuildOutcome(StatusCodes.Status404NotFound, "NOT_FOUND", "Ruleset version tidak ditemukan");
        }

        var activeRulesetVersionId = await _sessions.GetActiveRulesetVersionIdAsync(request.SessionId, ct);
        if (activeRulesetVersionId != request.RulesetVersionId)
        {
            return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Ruleset version tidak aktif");
        }

        if (!AllowedActorTypes.Contains(request.ActorType))
        {
            return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Actor type tidak valid",
                new ErrorDetail("actor_type", "INVALID_ENUM"));
        }

        if (!AllowedWeekdays.Contains(request.Weekday))
        {
            return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Weekday tidak valid",
                new ErrorDetail("weekday", "INVALID_ENUM"));
        }

        if (string.Equals(request.ActorType, "PLAYER", StringComparison.OrdinalIgnoreCase) && request.PlayerId is null)
        {
            return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Player wajib diisi untuk actor PLAYER",
                new ErrorDetail("player_id", "REQUIRED"));
        }

        if (string.IsNullOrWhiteSpace(request.ActionType))
        {
            return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Action type wajib diisi",
                new ErrorDetail("action_type", "REQUIRED"));
        }

        var maxSequence = await _events.GetMaxSequenceNumberAsync(request.SessionId, ct);
        if (maxSequence.HasValue && request.SequenceNumber < maxSequence.Value)
        {
            return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Sequence number lebih kecil dari event terakhir");
        }

        if (await _events.EventIdExistsAsync(request.SessionId, request.EventId, ct))
        {
            return BuildOutcome(StatusCodes.Status409Conflict, "DUPLICATE", "Event sudah ada");
        }

        if (await _events.SequenceNumberExistsAsync(request.SessionId, request.SequenceNumber, ct))
        {
            return BuildOutcome(StatusCodes.Status409Conflict, "DUPLICATE", "Sequence number sudah ada");
        }

        if (!RulesetConfigParser.TryParse(rulesetVersion.ConfigJson, out var config, out _))
        {
            return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Ruleset config tidak valid");
        }

        var domainValidation = await ValidateDomainRulesAsync(request, config!, ct);
        if (!domainValidation.IsValid)
        {
            return domainValidation;
        }

        return Valid;
    }

    private async Task<Guid> StoreEventAsync(EventRequest request, CancellationToken ct)
    {
        var eventPk = Guid.NewGuid();
        var record = new EventDb(
            eventPk,
            request.EventId,
            request.SessionId,
            request.PlayerId,
            request.ActorType.ToUpperInvariant(),
            request.Timestamp,
            request.DayIndex,
            request.Weekday.ToUpperInvariant(),
            request.TurnNumber,
            request.SequenceNumber,
            request.ActionType,
            request.RulesetVersionId,
            request.Payload.GetRawText(),
            DateTimeOffset.UtcNow,
            request.ClientRequestId);

        await _events.InsertEventAsync(record, ct);

        if (TryBuildCashflowProjection(request, eventPk, out var projection))
        {
            await _events.InsertCashflowProjectionAsync(projection, ct);
        }

        return eventPk;
    }

    private static EventRequest ToEventRequest(EventDb record)
    {
        using var document = JsonDocument.Parse(record.Payload);
        var payload = document.RootElement.Clone();

        return new EventRequest(
            record.EventId,
            record.SessionId,
            record.PlayerId,
            record.ActorType,
            record.Timestamp,
            record.DayIndex,
            record.Weekday,
            record.TurnNumber,
            record.SequenceNumber,
            record.ActionType,
            record.RulesetVersionId,
            payload,
            record.ClientRequestId);
    }

    private ValidationOutcome BuildOutcome(int statusCode, string code, string message, params ErrorDetail[] details)
    {
        var error = ApiErrorHelper.BuildError(HttpContext, code, message, details);
        return new ValidationOutcome(false, statusCode, error);
    }

    private async Task<ValidationOutcome> ValidateDomainRulesAsync(EventRequest request, RulesetConfig config, CancellationToken ct)
    {
        var actionType = request.ActionType;
        var payload = request.Payload;

        if (string.Equals(actionType, "transaction.recorded", StringComparison.OrdinalIgnoreCase))
        {
            if (!TryReadTransaction(payload, out var direction, out var amount, out var category, out var counterparty))
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Payload transaksi tidak valid",
                    new ErrorDetail("payload", "INVALID_STRUCTURE"));
            }

            if (!string.Equals(direction, "IN", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(direction, "OUT", StringComparison.OrdinalIgnoreCase))
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Direction tidak valid",
                    new ErrorDetail("payload.direction", "INVALID_ENUM"));
            }

            if (amount <= 0)
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Amount harus > 0",
                    new ErrorDetail("payload.amount", "OUT_OF_RANGE"));
            }

            if (string.IsNullOrWhiteSpace(category))
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Category wajib diisi",
                    new ErrorDetail("payload.category", "REQUIRED"));
            }

            if (!string.IsNullOrWhiteSpace(counterparty) &&
                !string.Equals(counterparty, "BANK", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(counterparty, "PLAYER", StringComparison.OrdinalIgnoreCase))
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Counterparty tidak valid",
                    new ErrorDetail("payload.counterparty", "INVALID_ENUM"));
            }

            return Valid;
        }

        if (string.Equals(actionType, "day.friday.donation", StringComparison.OrdinalIgnoreCase))
        {
            if (!config.FridayEnabled)
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Fitur donasi Jumat tidak aktif");
            }

            if (!string.Equals(request.Weekday, "FRI", StringComparison.OrdinalIgnoreCase))
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Weekday harus FRI",
                    new ErrorDetail("weekday", "INVALID_VALUE"));
            }

            if (!TryReadAmount(payload, out var amount))
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Payload donasi tidak valid",
                    new ErrorDetail("payload.amount", "REQUIRED"));
            }

            if (amount < config.DonationMin || amount > config.DonationMax)
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Jumlah donasi di luar batas");
            }

            return Valid;
        }

        if (string.Equals(actionType, "day.saturday.gold_trade", StringComparison.OrdinalIgnoreCase))
        {
            if (!config.SaturdayEnabled)
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Fitur perdagangan emas tidak aktif");
            }

            if (!string.Equals(request.Weekday, "SAT", StringComparison.OrdinalIgnoreCase))
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Weekday harus SAT",
                    new ErrorDetail("weekday", "INVALID_VALUE"));
            }

            if (!TryReadGoldTrade(payload, out var tradeType, out var qty, out var unitPrice, out var amount))
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Payload gold trade tidak valid",
                    new ErrorDetail("payload", "INVALID_STRUCTURE"));
            }

            if (!string.Equals(tradeType, "BUY", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(tradeType, "SELL", StringComparison.OrdinalIgnoreCase))
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Trade type tidak valid",
                    new ErrorDetail("payload.trade_type", "INVALID_ENUM"));
            }

            if (qty <= 0 || unitPrice <= 0 || amount <= 0)
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Nilai qty/unit_price/amount tidak valid",
                    new ErrorDetail("payload.amount", "OUT_OF_RANGE"));
            }

            if (amount != unitPrice * qty)
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Amount tidak sesuai unit_price * qty");
            }

            if (string.Equals(tradeType, "BUY", StringComparison.OrdinalIgnoreCase) && !config.GoldAllowBuy)
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Ruleset melarang BUY emas");
            }

            if (string.Equals(tradeType, "SELL", StringComparison.OrdinalIgnoreCase) && !config.GoldAllowSell)
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Ruleset melarang SELL emas");
            }

            return Valid;
        }

        if (string.Equals(actionType, "turn.action.used", StringComparison.OrdinalIgnoreCase))
        {
            if (!TryReadActionUsed(payload, out var used, out var remaining))
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Payload action used tidak valid",
                    new ErrorDetail("payload", "INVALID_STRUCTURE"));
            }

            if (used < 0 || remaining < 0)
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Nilai used/remaining tidak valid",
                    new ErrorDetail("payload.used", "OUT_OF_RANGE"));
            }

            if (request.PlayerId is null)
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Player wajib diisi",
                    new ErrorDetail("player_id", "REQUIRED"));
            }

            var events = await _events.GetAllEventsBySessionAsync(request.SessionId, ct);
            var usedSoFar = 0;
            foreach (var evt in events.Where(e =>
                         e.PlayerId == request.PlayerId &&
                         e.TurnNumber == request.TurnNumber &&
                         e.ActionType == "turn.action.used"))
            {
                using var doc = JsonDocument.Parse(evt.Payload);
                if (TryReadActionUsed(doc.RootElement, out var usedValue, out _))
                {
                    usedSoFar += usedValue;
                }
            }

            if (usedSoFar + used > config.ActionsPerTurn)
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Jumlah token aksi melebihi batas ruleset");
            }

            return Valid;
        }

        if (string.Equals(actionType, "need.primary.purchased", StringComparison.OrdinalIgnoreCase))
        {
            if (!TryReadAmount(payload, out var amount))
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Payload kebutuhan primer tidak valid",
                    new ErrorDetail("payload.amount", "REQUIRED"));
            }

            if (amount <= 0)
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Amount harus > 0",
                    new ErrorDetail("payload.amount", "OUT_OF_RANGE"));
            }

            if (config.PrimaryNeedMaxPerDay == 0)
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Ruleset melarang pembelian kebutuhan primer");
            }

            if (request.PlayerId is null)
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Player wajib diisi",
                    new ErrorDetail("player_id", "REQUIRED"));
            }

            var events = await _events.GetAllEventsBySessionAsync(request.SessionId, ct);
            var primaryCount = events.Count(e =>
                e.PlayerId == request.PlayerId &&
                e.DayIndex == request.DayIndex &&
                e.ActionType == "need.primary.purchased");

            if (primaryCount >= config.PrimaryNeedMaxPerDay)
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Pembelian kebutuhan primer melebihi batas harian");
            }

            return Valid;
        }

        if (string.Equals(actionType, "need.secondary.purchased", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(actionType, "need.tertiary.purchased", StringComparison.OrdinalIgnoreCase))
        {
            if (!TryReadAmount(payload, out var amount))
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Payload kebutuhan tidak valid",
                    new ErrorDetail("payload.amount", "REQUIRED"));
            }

            if (amount <= 0)
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Amount harus > 0",
                    new ErrorDetail("payload.amount", "OUT_OF_RANGE"));
            }

            if (config.RequirePrimaryBeforeOthers && request.PlayerId is not null)
            {
                var events = await _events.GetAllEventsBySessionAsync(request.SessionId, ct);
                var hasPrimary = events.Any(e =>
                    e.PlayerId == request.PlayerId &&
                    e.DayIndex == request.DayIndex &&
                    e.ActionType == "need.primary.purchased");

                if (!hasPrimary)
                {
                    return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Kebutuhan primer harus dibeli terlebih dahulu");
                }
            }

            return Valid;
        }

        if (string.Equals(actionType, "ingredient.purchased", StringComparison.OrdinalIgnoreCase))
        {
            if (!TryReadIngredientPurchase(payload, out var cardId, out var amount))
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Payload ingredient tidak valid",
                    new ErrorDetail("payload.card_id", "REQUIRED"));
            }

            if (amount <= 0)
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Amount harus > 0",
                    new ErrorDetail("payload.amount", "OUT_OF_RANGE"));
            }

            if (request.PlayerId is null)
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Player wajib diisi",
                    new ErrorDetail("player_id", "REQUIRED"));
            }

            var events = await _events.GetAllEventsBySessionAsync(request.SessionId, ct);
            var inventory = BuildIngredientInventory(events, request.PlayerId.Value);

            if (inventory.Total + amount > config.MaxIngredientTotal)
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Total kartu bahan melebihi batas ruleset");
            }

            var currentSame = inventory.ByCardId.TryGetValue(cardId, out var currentQty) ? currentQty : 0;
            if (currentSame + amount > config.MaxSameIngredient)
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Jumlah kartu bahan sejenis melebihi batas ruleset");
            }

            return Valid;
        }

        if (string.Equals(actionType, "order.claimed", StringComparison.OrdinalIgnoreCase))
        {
            if (request.PlayerId is null)
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Player wajib diisi",
                    new ErrorDetail("player_id", "REQUIRED"));
            }

            if (!TryReadOrderClaim(payload, out var requiredCards, out var income))
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Payload order claim tidak valid",
                    new ErrorDetail("payload.required_ingredient_card_ids", "REQUIRED"));
            }

            if (income <= 0)
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Income harus > 0",
                    new ErrorDetail("payload.income", "OUT_OF_RANGE"));
            }

            var events = await _events.GetAllEventsBySessionAsync(request.SessionId, ct);
            var inventory = BuildIngredientInventory(events, request.PlayerId.Value);

            foreach (var card in requiredCards)
            {
                if (!inventory.ByCardId.TryGetValue(card, out var qty) || qty <= 0)
                {
                    return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Bahan tidak mencukupi untuk klaim order");
                }

                inventory.ByCardId[card] = qty - 1;
            }

            return Valid;
        }

        if (string.Equals(actionType, "loan.syariah.taken", StringComparison.OrdinalIgnoreCase))
        {
            if (!config.LoanEnabled)
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Fitur pinjaman tidak aktif");
            }

            if (!TryReadLoanTaken(payload, out var principal, out var installment, out var duration))
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Payload loan taken tidak valid",
                    new ErrorDetail("payload.principal", "REQUIRED"));
            }

            if (principal <= 0 || installment <= 0 || duration <= 0)
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Nilai pinjaman tidak valid",
                    new ErrorDetail("payload.principal", "OUT_OF_RANGE"));
            }

            return Valid;
        }

        if (string.Equals(actionType, "loan.syariah.repaid", StringComparison.OrdinalIgnoreCase))
        {
            if (!config.LoanEnabled)
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Fitur pinjaman tidak aktif");
            }

            if (!TryReadLoanRepay(payload, out var amount))
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Payload loan repaid tidak valid",
                    new ErrorDetail("payload.amount", "REQUIRED"));
            }

            if (amount <= 0)
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Amount harus > 0",
                    new ErrorDetail("payload.amount", "OUT_OF_RANGE"));
            }

            return Valid;
        }

        if (string.Equals(actionType, "insurance.multirisk.purchased", StringComparison.OrdinalIgnoreCase))
        {
            if (!config.InsuranceEnabled)
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Fitur asuransi tidak aktif");
            }

            if (!TryReadInsurance(payload, out var premium))
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Payload insurance tidak valid",
                    new ErrorDetail("payload.premium", "REQUIRED"));
            }

            if (premium <= 0)
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Premium harus > 0",
                    new ErrorDetail("payload.premium", "OUT_OF_RANGE"));
            }

            return Valid;
        }

        return Valid;
    }

    private static bool TryReadTransaction(JsonElement payload, out string direction, out double amount, out string category, out string? counterparty)
    {
        direction = string.Empty;
        category = string.Empty;
        counterparty = null;
        amount = 0;

        if (!payload.TryGetProperty("direction", out var directionProp) ||
            !payload.TryGetProperty("amount", out var amountProp) ||
            !payload.TryGetProperty("category", out var categoryProp))
        {
            return false;
        }

        direction = directionProp.GetString() ?? string.Empty;
        category = categoryProp.GetString() ?? string.Empty;
        amount = amountProp.GetDouble();
        if (payload.TryGetProperty("counterparty", out var counterpartyProp))
        {
            counterparty = counterpartyProp.GetString();
        }

        return true;
    }

    private static bool TryReadAmount(JsonElement payload, out double amount)
    {
        amount = 0;
        if (!payload.TryGetProperty("amount", out var amountProp))
        {
            return false;
        }

        amount = amountProp.GetDouble();
        return true;
    }

    private static bool TryReadGoldTrade(JsonElement payload, out string tradeType, out int qty, out int unitPrice, out int amount)
    {
        tradeType = string.Empty;
        qty = 0;
        unitPrice = 0;
        amount = 0;

        if (!payload.TryGetProperty("trade_type", out var tradeTypeProp) ||
            !payload.TryGetProperty("qty", out var qtyProp) ||
            !payload.TryGetProperty("unit_price", out var unitPriceProp) ||
            !payload.TryGetProperty("amount", out var amountProp))
        {
            return false;
        }

        tradeType = tradeTypeProp.GetString() ?? string.Empty;
        qty = qtyProp.GetInt32();
        unitPrice = unitPriceProp.GetInt32();
        amount = amountProp.GetInt32();
        return true;
    }

    private static bool TryReadActionUsed(JsonElement payload, out int used, out int remaining)
    {
        used = 0;
        remaining = 0;
        if (!payload.TryGetProperty("used", out var usedProp) ||
            !payload.TryGetProperty("remaining", out var remainingProp))
        {
            return false;
        }

        used = usedProp.GetInt32();
        remaining = remainingProp.GetInt32();
        return true;
    }

    private static bool TryReadIngredientPurchase(JsonElement payload, out string cardId, out int amount)
    {
        cardId = string.Empty;
        amount = 0;

        if (!payload.TryGetProperty("card_id", out var cardIdProp) ||
            !payload.TryGetProperty("amount", out var amountProp))
        {
            return false;
        }

        cardId = cardIdProp.GetString() ?? string.Empty;
        amount = amountProp.GetInt32();
        return !string.IsNullOrWhiteSpace(cardId);
    }

    private static bool TryReadOrderClaim(JsonElement payload, out List<string> requiredCards, out int income)
    {
        requiredCards = new List<string>();
        income = 0;

        if (!payload.TryGetProperty("required_ingredient_card_ids", out var cardsProp) ||
            cardsProp.ValueKind != JsonValueKind.Array ||
            !payload.TryGetProperty("income", out var incomeProp))
        {
            return false;
        }

        income = incomeProp.GetInt32();
        foreach (var item in cardsProp.EnumerateArray())
        {
            var cardId = item.GetString();
            if (!string.IsNullOrWhiteSpace(cardId))
            {
                requiredCards.Add(cardId);
            }
        }

        return requiredCards.Count > 0;
    }

    private static bool TryReadLoanTaken(JsonElement payload, out int principal, out int installment, out int duration)
    {
        principal = 0;
        installment = 0;
        duration = 0;

        if (!payload.TryGetProperty("principal", out var principalProp) ||
            !payload.TryGetProperty("installment", out var installmentProp) ||
            !payload.TryGetProperty("duration_turn", out var durationProp))
        {
            return false;
        }

        principal = principalProp.GetInt32();
        installment = installmentProp.GetInt32();
        duration = durationProp.GetInt32();
        return true;
    }

    private static bool TryReadLoanRepay(JsonElement payload, out int amount)
    {
        amount = 0;
        if (!payload.TryGetProperty("amount", out var amountProp))
        {
            return false;
        }

        amount = amountProp.GetInt32();
        return true;
    }

    private static bool TryReadInsurance(JsonElement payload, out int premium)
    {
        premium = 0;
        if (!payload.TryGetProperty("premium", out var premiumProp))
        {
            return false;
        }

        premium = premiumProp.GetInt32();
        return true;
    }

    private static IngredientInventory BuildIngredientInventory(IEnumerable<EventDb> events, Guid playerId)
    {
        var inventory = new IngredientInventory();

        foreach (var evt in events.Where(e => e.PlayerId == playerId))
        {
            if (evt.ActionType == "ingredient.purchased" &&
                TryReadIngredientPurchase(ReadPayload(evt.Payload), out var cardId, out var amount))
            {
                inventory.Total += amount;
                inventory.ByCardId[cardId] = inventory.ByCardId.TryGetValue(cardId, out var qty) ? qty + amount : amount;
            }

            if (evt.ActionType == "order.claimed" &&
                TryReadOrderClaim(ReadPayload(evt.Payload), out var requiredCards, out _))
            {
                foreach (var card in requiredCards)
                {
                    if (inventory.ByCardId.TryGetValue(card, out var qty) && qty > 0)
                    {
                        inventory.ByCardId[card] = qty - 1;
                        inventory.Total = Math.Max(0, inventory.Total - 1);
                    }
                }
            }
        }

        return inventory;
    }

    private static JsonElement ReadPayload(string payload)
    {
        using var doc = JsonDocument.Parse(payload);
        return doc.RootElement.Clone();
    }

    private static string? BuildValidationDetailsJson(EventRequest request, ErrorResponse? error)
    {
        if (error is null)
        {
            return null;
        }

        var payload = new
        {
            player_id = request.PlayerId,
            action_type = request.ActionType,
            details = error.Details
        };

        return JsonSerializer.Serialize(payload);
    }

    private sealed class IngredientInventory
    {
        internal int Total { get; set; }
        internal Dictionary<string, int> ByCardId { get; } = new(StringComparer.OrdinalIgnoreCase);
    }

    private static bool TryBuildCashflowProjection(EventRequest request, Guid eventPk, out CashflowProjectionDb projection)
    {
        projection = default!;
        if (request.PlayerId is null)
        {
            return false;
        }

        var action = request.ActionType;
        var timestamp = request.Timestamp;
        var playerId = request.PlayerId.Value;
        var direction = string.Empty;
        var amount = 0;
        var category = string.Empty;
        string? counterparty = null;
        string? reference = null;
        string? note = null;

        if (string.Equals(action, "transaction.recorded", StringComparison.OrdinalIgnoreCase) &&
            TryReadTransaction(request.Payload, out var dir, out var amt, out var cat, out var cp))
        {
            direction = dir.ToUpperInvariant();
            amount = (int)Math.Round(amt);
            category = cat;
            counterparty = cp;
        }
        else if (string.Equals(action, "day.friday.donation", StringComparison.OrdinalIgnoreCase) &&
                 TryReadAmount(request.Payload, out var donationAmount))
        {
            direction = "OUT";
            amount = (int)Math.Round(donationAmount);
            category = "DONATION";
        }
        else if (string.Equals(action, "day.saturday.gold_trade", StringComparison.OrdinalIgnoreCase) &&
                 TryReadGoldTrade(request.Payload, out var tradeType, out _, out _, out var tradeAmount))
        {
            direction = string.Equals(tradeType, "BUY", StringComparison.OrdinalIgnoreCase) ? "OUT" : "IN";
            amount = tradeAmount;
            category = "GOLD_TRADE";
        }
        else if (string.Equals(action, "ingredient.purchased", StringComparison.OrdinalIgnoreCase) &&
                 TryReadAmount(request.Payload, out var ingredientAmount))
        {
            direction = "OUT";
            amount = (int)Math.Round(ingredientAmount);
            category = "INGREDIENT";
        }
        else if (string.Equals(action, "order.claimed", StringComparison.OrdinalIgnoreCase) &&
                 TryReadOrderClaim(request.Payload, out _, out var income))
        {
            direction = "IN";
            amount = income;
            category = "ORDER";
        }
        else if (string.Equals(action, "need.primary.purchased", StringComparison.OrdinalIgnoreCase) &&
                 TryReadAmount(request.Payload, out var primaryAmount))
        {
            direction = "OUT";
            amount = (int)Math.Round(primaryAmount);
            category = "NEED_PRIMARY";
        }
        else if (string.Equals(action, "need.secondary.purchased", StringComparison.OrdinalIgnoreCase) &&
                 TryReadAmount(request.Payload, out var secondaryAmount))
        {
            direction = "OUT";
            amount = (int)Math.Round(secondaryAmount);
            category = "NEED_SECONDARY";
        }
        else if (string.Equals(action, "need.tertiary.purchased", StringComparison.OrdinalIgnoreCase) &&
                 TryReadAmount(request.Payload, out var tertiaryAmount))
        {
            direction = "OUT";
            amount = (int)Math.Round(tertiaryAmount);
            category = "NEED_TERTIARY";
        }
        else if (string.Equals(action, "loan.syariah.taken", StringComparison.OrdinalIgnoreCase) &&
                 TryReadLoanTaken(request.Payload, out var principal, out _, out _))
        {
            direction = "IN";
            amount = principal;
            category = "LOAN_TAKEN";
        }
        else if (string.Equals(action, "loan.syariah.repaid", StringComparison.OrdinalIgnoreCase) &&
                 TryReadLoanRepay(request.Payload, out var repayAmount))
        {
            direction = "OUT";
            amount = repayAmount;
            category = "LOAN_REPAID";
        }
        else if (string.Equals(action, "insurance.multirisk.purchased", StringComparison.OrdinalIgnoreCase) &&
                 TryReadInsurance(request.Payload, out var premium))
        {
            direction = "OUT";
            amount = premium;
            category = "INSURANCE_PREMIUM";
        }
        else
        {
            return false;
        }

        if (amount <= 0 || string.IsNullOrWhiteSpace(direction))
        {
            return false;
        }

        projection = new CashflowProjectionDb(
            Guid.NewGuid(),
            request.SessionId,
            playerId,
            eventPk,
            request.EventId,
            timestamp,
            direction,
            amount,
            category,
            counterparty,
            reference,
            note);

        return true;
    }
}
