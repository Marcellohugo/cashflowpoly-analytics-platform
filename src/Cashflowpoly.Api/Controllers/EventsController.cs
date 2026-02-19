// Fungsi file: Mengelola endpoint API untuk domain EventsController termasuk validasi request dan respons standar.
using System.Text.Json;
using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Domain;
using Cashflowpoly.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Security.Claims;

namespace Cashflowpoly.Api.Controllers;

[ApiController]
[Route("api/v1")]
[Authorize]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status429TooManyRequests)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
/// <summary>
/// Menyatakan peran utama tipe EventsController pada modul ini.
/// </summary>
public sealed class EventsController : ControllerBase
{
    /// <summary>
    /// Menyatakan peran utama tipe ValidationOutcome pada modul ini.
    /// </summary>
    private sealed record ValidationOutcome(bool IsValid, int StatusCode, ErrorResponse? Error);

    /// <summary>
    /// Menjalankan fungsi new sebagai bagian dari alur file ini.
    /// </summary>
    private static readonly ValidationOutcome Valid = new(true, StatusCodes.Status200OK, null);

    private readonly SessionRepository _sessions;
    private readonly RulesetRepository _rulesets;
    private readonly EventRepository _events;
    private readonly PlayerRepository _players;

    /// <summary>
    /// Controller untuk ingest event dan akses histori event.
    /// </summary>
    public EventsController(SessionRepository sessions, RulesetRepository rulesets, EventRepository events, PlayerRepository players)
    {
        _sessions = sessions;
        _rulesets = rulesets;
        _events = events;
        _players = players;
    }

    /// <summary>
    /// Menjalankan fungsi new sebagai bagian dari alur file ini.
    /// </summary>
    private static readonly HashSet<string> AllowedActorTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "PLAYER",
        "SYSTEM"
    };

    /// <summary>
    /// Menjalankan fungsi new sebagai bagian dari alur file ini.
    /// </summary>
    private static readonly HashSet<string> AllowedWeekdays = new(StringComparer.OrdinalIgnoreCase)
    {
        "MON", "TUE", "WED", "THU", "FRI", "SAT", "SUN"
    };

    private const int RulebookMissionPenaltyPoints = 10;
    private const int RulebookLoanPrincipal = 10;
    private const int RulebookLoanPenaltyPoints = 15;
    private const int RulebookInsurancePremium = 1;
    private const int RulebookSavingMaxDeposit = 15;

    [HttpPost("events")]
    [ProducesResponseType(typeof(EventStoredResponse), StatusCodes.Status201Created)]
    /// <summary>
    /// Menjalankan fungsi CreateEvent sebagai bagian dari alur file ini.
    /// </summary>
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
    [ProducesResponseType(typeof(EventBatchResponse), StatusCodes.Status200OK)]
    /// <summary>
    /// Menjalankan fungsi CreateEventsBatch sebagai bagian dari alur file ini.
    /// </summary>
    public async Task<IActionResult> CreateEventsBatch([FromBody] EventBatchRequest request, CancellationToken ct)
    {
        if (request.Events is null || request.Events.Count == 0)
        {
            return BadRequest(ApiErrorHelper.BuildError(HttpContext, "VALIDATION_ERROR", "Daftar event batch wajib diisi",
                new ErrorDetail("events", "REQUIRED")));
        }

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
    [ProducesResponseType(typeof(EventsBySessionResponse), StatusCodes.Status200OK)]
    /// <summary>
    /// Menjalankan fungsi GetEventsBySession sebagai bagian dari alur file ini.
    /// </summary>
    public async Task<IActionResult> GetEventsBySession(Guid sessionId, [FromQuery] long fromSeq = 0, [FromQuery] int limit = 200, CancellationToken ct = default)
    {
        var instructorScopeCheck = await ValidateInstructorSessionAccessAsync(sessionId, ct);
        if (!instructorScopeCheck.IsValid)
        {
            return StatusCode(instructorScopeCheck.StatusCode, instructorScopeCheck.Error);
        }

        var session = await _sessions.GetSessionAsync(sessionId, ct);
        if (session is null)
        {
            return NotFound(ApiErrorHelper.BuildError(HttpContext, "NOT_FOUND", "Session tidak ditemukan"));
        }

        if (fromSeq < 0)
        {
            return BadRequest(ApiErrorHelper.BuildError(HttpContext, "VALIDATION_ERROR", "fromSeq tidak boleh negatif",
                new ErrorDetail("fromSeq", "OUT_OF_RANGE")));
        }

        if (limit is < 1 or > 1000)
        {
            return BadRequest(ApiErrorHelper.BuildError(HttpContext, "VALIDATION_ERROR", "limit harus antara 1 sampai 1000",
                new ErrorDetail("limit", "OUT_OF_RANGE")));
        }

        var events = await _events.GetEventsBySessionAsync(sessionId, fromSeq, limit, ct);
        var responseEvents = events.Select(ToEventRequest).ToList();
        return Ok(new EventsBySessionResponse(sessionId, responseEvents));
    }

    /// <summary>
    /// Menjalankan fungsi ValidateEventAsync sebagai bagian dari alur file ini.
    /// </summary>
    private async Task<ValidationOutcome> ValidateEventAsync(EventRequest request, CancellationToken ct)
    {
        var instructorScopeCheck = await ValidateInstructorSessionAccessAsync(request.SessionId, ct);
        if (!instructorScopeCheck.IsValid)
        {
            return instructorScopeCheck;
        }

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

        if (request.TurnNumber < 1)
        {
            return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Turn number minimal 1",
                new ErrorDetail("turn_number", "OUT_OF_RANGE"));
        }

        if (request.DayIndex < 0)
        {
            return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Day index minimal 0",
                new ErrorDetail("day_index", "OUT_OF_RANGE"));
        }

        if (request.SequenceNumber < 0)
        {
            return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Sequence number minimal 0",
                new ErrorDetail("sequence_number", "OUT_OF_RANGE"));
        }

        if (string.Equals(request.ActorType, "PLAYER", StringComparison.OrdinalIgnoreCase) && request.PlayerId is null)
        {
            return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Player wajib diisi untuk actor PLAYER",
                new ErrorDetail("player_id", "REQUIRED"));
        }

        if (request.PlayerId is not null)
        {
            var player = await _players.GetPlayerAsync(request.PlayerId.Value, ct);
            if (player is null)
            {
                return BuildOutcome(StatusCodes.Status404NotFound, "NOT_FOUND", "Player tidak ditemukan");
            }

            var inSession = await _players.IsPlayerInSessionAsync(request.SessionId, request.PlayerId.Value, ct);
            if (!inSession)
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Player belum terdaftar pada sesi");
            }
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

        if (maxSequence.HasValue && request.SequenceNumber > maxSequence.Value + 1)
        {
            return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Sequence number loncat dari event terakhir");
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

    /// <summary>
    /// Menjalankan fungsi StoreEventAsync sebagai bagian dari alur file ini.
    /// </summary>
    private async Task<Guid> StoreEventAsync(EventRequest request, CancellationToken ct)
    {
        var eventPk = Guid.NewGuid();
        var timestamp = request.Timestamp.ToUniversalTime();
        var record = new EventDb
        {
            EventPk = eventPk,
            EventId = request.EventId,
            SessionId = request.SessionId,
            PlayerId = request.PlayerId,
            ActorType = request.ActorType.ToUpperInvariant(),
            Timestamp = timestamp,
            DayIndex = request.DayIndex,
            Weekday = request.Weekday.ToUpperInvariant(),
            TurnNumber = request.TurnNumber,
            SequenceNumber = request.SequenceNumber,
            ActionType = request.ActionType,
            RulesetVersionId = request.RulesetVersionId,
            Payload = request.Payload.GetRawText(),
            ReceivedAt = DateTimeOffset.UtcNow,
            ClientRequestId = request.ClientRequestId
        };

        await _events.InsertEventAsync(record, ct);

        if (TryBuildCashflowProjection(request, timestamp, eventPk, out var projection))
        {
            await _events.InsertCashflowProjectionAsync(projection, ct);
        }

        if (request.PlayerId is { } insurancePlayerId &&
            string.Equals(request.ActionType, "insurance.multirisk.used", StringComparison.OrdinalIgnoreCase) &&
            TryReadInsuranceUsed(request.Payload, out var riskEventIdText) &&
            Guid.TryParse(riskEventIdText, out var riskEventId))
        {
            var riskEvent = await _events.GetEventByIdAsync(request.SessionId, riskEventId, ct);
            if (riskEvent is not null &&
                riskEvent.PlayerId == request.PlayerId &&
                string.Equals(riskEvent.ActionType, "risk.life.drawn", StringComparison.OrdinalIgnoreCase))
            {
                var riskPayload = ReadPayload(riskEvent.Payload);
                if (TryReadRiskLife(riskPayload, out _, out var direction, out var amount) &&
                    string.Equals(direction, "OUT", StringComparison.OrdinalIgnoreCase) &&
                    amount > 0)
                {
                    var offset = new CashflowProjectionDb
                    {
                        ProjectionId = Guid.NewGuid(),
                        SessionId = request.SessionId,
                        PlayerId = insurancePlayerId,
                        EventPk = eventPk,
                        EventId = request.EventId,
                        Timestamp = timestamp,
                        Direction = "IN",
                        Amount = amount,
                        Category = "INSURANCE_CLAIM",
                        Counterparty = "BANK",
                        Reference = riskEventIdText,
                        Note = "Offset risiko via asuransi"
                    };
                    await _events.InsertCashflowProjectionAsync(offset, ct);
                }
            }
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

    /// <summary>
    /// Menjalankan fungsi BuildOutcome sebagai bagian dari alur file ini.
    /// </summary>
    private ValidationOutcome BuildOutcome(int statusCode, string code, string message, params ErrorDetail[] details)
    {
        var error = ApiErrorHelper.BuildError(HttpContext, code, message, details);
        return new ValidationOutcome(false, statusCode, error);
    }

    /// <summary>
    /// Menjalankan fungsi ValidateDomainRulesAsync sebagai bagian dari alur file ini.
    /// </summary>
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

            if (string.Equals(direction, "OUT", StringComparison.OrdinalIgnoreCase) && request.PlayerId is not null)
            {
                var balanceCheck = await EnsureSufficientBalanceAsync(request, config, amount, ct);
                if (!balanceCheck.IsValid)
                {
                    return balanceCheck;
                }
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

            if (request.PlayerId is not null)
            {
                var balanceCheck = await EnsureSufficientBalanceAsync(request, config, amount, ct);
                if (!balanceCheck.IsValid)
                {
                    return balanceCheck;
                }
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

            if (string.Equals(tradeType, "SELL", StringComparison.OrdinalIgnoreCase) && request.PlayerId is not null)
            {
                var events = await _events.GetAllEventsBySessionAsync(request.SessionId, ct);
                var goldQty = 0;
                foreach (var evt in events.Where(e =>
                             e.PlayerId == request.PlayerId &&
                             e.ActionType == "day.saturday.gold_trade"))
                {
                    if (!TryReadGoldTrade(ReadPayload(evt.Payload), out var evtTradeType, out var evtQty, out _, out _))
                    {
                        continue;
                    }

                    goldQty += string.Equals(evtTradeType, "BUY", StringComparison.OrdinalIgnoreCase) ? evtQty : -evtQty;
                }

                if (goldQty < qty)
                {
                    return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Kepemilikan emas tidak mencukupi");
                }
            }

            if (string.Equals(tradeType, "BUY", StringComparison.OrdinalIgnoreCase) && request.PlayerId is not null)
            {
                var balanceCheck = await EnsureSufficientBalanceAsync(request, config, amount, ct);
                if (!balanceCheck.IsValid)
                {
                    return balanceCheck;
                }
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

        if (string.Equals(actionType, "turn.ended", StringComparison.OrdinalIgnoreCase) &&
            string.Equals(config.Mode, "MAHIR", StringComparison.OrdinalIgnoreCase))
        {
            var events = await _events.GetAllEventsBySessionAsync(request.SessionId, ct);
            var turnEvents = events.Where(e => e.TurnNumber == request.TurnNumber && e.PlayerId.HasValue).ToList();

            var orderCounts = turnEvents
                .Where(e => e.ActionType == "order.claimed")
                .GroupBy(e => e.PlayerId!.Value)
                .ToDictionary(g => g.Key, g => g.Count());

            var riskCounts = turnEvents
                .Where(e => e.ActionType == "risk.life.drawn")
                .GroupBy(e => e.PlayerId!.Value)
                .ToDictionary(g => g.Key, g => g.Count());

            foreach (var playerId in orderCounts.Keys.Union(riskCounts.Keys))
            {
                orderCounts.TryGetValue(playerId, out var orders);
                riskCounts.TryGetValue(playerId, out var risks);
                if (orders != risks)
                {
                    return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION",
                        "Setiap klaim pesanan harus diikuti pengambilan risiko pada mode MAHIR");
                }
            }

            return Valid;
        }

        if (string.Equals(actionType, "need.primary.purchased", StringComparison.OrdinalIgnoreCase))
        {
            if (!TryReadNeedPurchase(payload, out var cardId, out var amount, out var points))
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Payload kebutuhan primer tidak valid",
                    new ErrorDetail("payload.card_id", "REQUIRED"));
            }

            if (amount <= 0)
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Amount harus > 0",
                    new ErrorDetail("payload.amount", "OUT_OF_RANGE"));
            }

            if (string.IsNullOrWhiteSpace(cardId))
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Card ID wajib diisi",
                    new ErrorDetail("payload.card_id", "REQUIRED"));
            }

            if (points < 0)
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Points tidak valid",
                    new ErrorDetail("payload.points", "OUT_OF_RANGE"));
            }

            if (!payload.TryGetProperty("points", out _))
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Points wajib diisi",
                    new ErrorDetail("payload.points", "REQUIRED"));
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

            if (request.PlayerId is not null)
            {
                var balanceCheck = await EnsureSufficientBalanceAsync(request, config, amount, ct);
                if (!balanceCheck.IsValid)
                {
                    return balanceCheck;
                }
            }

            return Valid;
        }

        if (string.Equals(actionType, "need.secondary.purchased", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(actionType, "need.tertiary.purchased", StringComparison.OrdinalIgnoreCase))
        {
            if (!TryReadNeedPurchase(payload, out var cardId, out var amount, out var points))
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Payload kebutuhan tidak valid",
                    new ErrorDetail("payload.card_id", "REQUIRED"));
            }

            if (amount <= 0)
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Amount harus > 0",
                    new ErrorDetail("payload.amount", "OUT_OF_RANGE"));
            }

            if (string.IsNullOrWhiteSpace(cardId))
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Card ID wajib diisi",
                    new ErrorDetail("payload.card_id", "REQUIRED"));
            }

            if (points < 0)
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Points tidak valid",
                    new ErrorDetail("payload.points", "OUT_OF_RANGE"));
            }

            if (!payload.TryGetProperty("points", out _))
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Points wajib diisi",
                    new ErrorDetail("payload.points", "REQUIRED"));
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

            if (request.PlayerId is not null)
            {
                var balanceCheck = await EnsureSufficientBalanceAsync(request, config, amount, ct);
                if (!balanceCheck.IsValid)
                {
                    return balanceCheck;
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

            var balanceCheck = await EnsureSufficientBalanceAsync(request, config, amount, ct);
            if (!balanceCheck.IsValid)
            {
                return balanceCheck;
            }

            return Valid;
        }

        if (string.Equals(actionType, "ingredient.discarded", StringComparison.OrdinalIgnoreCase))
        {
            if (request.PlayerId is null)
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Player wajib diisi",
                    new ErrorDetail("player_id", "REQUIRED"));
            }

            if (!TryReadIngredientPurchase(payload, out var cardId, out var amount))
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Payload discard ingredient tidak valid",
                    new ErrorDetail("payload.card_id", "REQUIRED"));
            }

            if (amount <= 0)
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Amount harus > 0",
                    new ErrorDetail("payload.amount", "OUT_OF_RANGE"));
            }

            var events = await _events.GetAllEventsBySessionAsync(request.SessionId, ct);
            var inventory = BuildIngredientInventory(events, request.PlayerId.Value);
            var currentQty = inventory.ByCardId.TryGetValue(cardId, out var qty) ? qty : 0;

            if (currentQty < amount)
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Jumlah discard melebihi stok bahan");
            }

            return Valid;
        }

        if (string.Equals(actionType, "order.passed", StringComparison.OrdinalIgnoreCase))
        {
            if (request.PlayerId is null)
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Player wajib diisi",
                    new ErrorDetail("player_id", "REQUIRED"));
            }

            if (!TryReadOrderClaim(payload, out _, out var income))
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Payload order pass tidak valid",
                    new ErrorDetail("payload.required_ingredient_card_ids", "REQUIRED"));
            }

            if (income <= 0)
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Income harus > 0",
                    new ErrorDetail("payload.income", "OUT_OF_RANGE"));
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

        if (string.Equals(actionType, "work.freelance.completed", StringComparison.OrdinalIgnoreCase))
        {
            if (request.PlayerId is null)
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Player wajib diisi",
                    new ErrorDetail("player_id", "REQUIRED"));
            }

            if (!TryReadAmount(payload, out var amount))
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Payload kerja lepas tidak valid",
                    new ErrorDetail("payload.amount", "REQUIRED"));
            }

            if (amount <= 0)
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Amount harus > 0",
                    new ErrorDetail("payload.amount", "OUT_OF_RANGE"));
            }

            var rounded = (int)Math.Round(amount);
            if (rounded != config.FreelanceIncome)
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Amount kerja lepas tidak sesuai ruleset");
            }

            return Valid;
        }

        if (string.Equals(actionType, "mission.assigned", StringComparison.OrdinalIgnoreCase))
        {
            if (request.PlayerId is null)
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Player wajib diisi",
                    new ErrorDetail("player_id", "REQUIRED"));
            }

            if (!TryReadMissionAssigned(payload, out var missionId, out var targetCardId, out var penaltyPoints))
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Payload mission tidak valid",
                    new ErrorDetail("payload.mission_id", "REQUIRED"));
            }

            if (string.IsNullOrWhiteSpace(missionId) || string.IsNullOrWhiteSpace(targetCardId))
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Mission ID dan target wajib diisi",
                    new ErrorDetail("payload.target_tertiary_card_id", "REQUIRED"));
            }

            if (penaltyPoints < 0)
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Penalty points tidak valid",
                    new ErrorDetail("payload.penalty_points", "OUT_OF_RANGE"));
            }

            if (penaltyPoints != RulebookMissionPenaltyPoints)
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Penalty misi harus 10 poin");
            }

            var events = await _events.GetAllEventsBySessionAsync(request.SessionId, ct);
            var alreadyAssigned = events.Any(e =>
                e.PlayerId == request.PlayerId &&
                e.ActionType == "mission.assigned");
            if (alreadyAssigned)
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Misi sudah ditetapkan untuk pemain");
            }

            return Valid;
        }

        if (string.Equals(actionType, "tie_breaker.assigned", StringComparison.OrdinalIgnoreCase))
        {
            if (request.PlayerId is null)
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Player wajib diisi",
                    new ErrorDetail("player_id", "REQUIRED"));
            }

            if (!TryReadTieBreaker(payload, out var number))
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Payload tie breaker tidak valid",
                    new ErrorDetail("payload.number", "REQUIRED"));
            }

            if (number <= 0)
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Nomor tie breaker tidak valid",
                    new ErrorDetail("payload.number", "OUT_OF_RANGE"));
            }

            var events = await _events.GetAllEventsBySessionAsync(request.SessionId, ct);
            var alreadyAssigned = events.Any(e =>
                e.PlayerId == request.PlayerId &&
                e.ActionType == "tie_breaker.assigned");
            if (alreadyAssigned)
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Tie breaker sudah ditetapkan untuk pemain");
            }

            return Valid;
        }

        if (string.Equals(actionType, "donation.rank.awarded", StringComparison.OrdinalIgnoreCase))
        {
            if (request.PlayerId is null)
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Player wajib diisi",
                    new ErrorDetail("player_id", "REQUIRED"));
            }

            if (!TryReadRankAwarded(payload, out var rank, out var points))
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Payload donasi tidak valid",
                    new ErrorDetail("payload.rank", "REQUIRED"));
            }

            if (rank <= 0 || points < 0)
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Nilai rank/points tidak valid",
                    new ErrorDetail("payload.points", "OUT_OF_RANGE"));
            }

            return Valid;
        }

        if (string.Equals(actionType, "gold.points.awarded", StringComparison.OrdinalIgnoreCase))
        {
            if (request.PlayerId is null)
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Player wajib diisi",
                    new ErrorDetail("player_id", "REQUIRED"));
            }

            if (!TryReadPointsAwarded(payload, out var points))
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Payload gold points tidak valid",
                    new ErrorDetail("payload.points", "REQUIRED"));
            }

            if (points < 0)
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Points tidak valid",
                    new ErrorDetail("payload.points", "OUT_OF_RANGE"));
            }

            return Valid;
        }

        if (string.Equals(actionType, "pension.rank.awarded", StringComparison.OrdinalIgnoreCase))
        {
            if (request.PlayerId is null)
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Player wajib diisi",
                    new ErrorDetail("player_id", "REQUIRED"));
            }

            if (!TryReadRankAwarded(payload, out var rank, out var points))
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Payload pension tidak valid",
                    new ErrorDetail("payload.rank", "REQUIRED"));
            }

            if (rank <= 0 || points < 0)
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Nilai rank/points tidak valid",
                    new ErrorDetail("payload.points", "OUT_OF_RANGE"));
            }

            return Valid;
        }

        if (string.Equals(actionType, "saving.deposit.created", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(actionType, "saving.deposit.withdrawn", StringComparison.OrdinalIgnoreCase))
        {
            if (!config.SavingGoalEnabled)
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Fitur tabungan tujuan tidak aktif");
            }

            if (request.PlayerId is null)
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Player wajib diisi",
                    new ErrorDetail("player_id", "REQUIRED"));
            }

            if (!TryReadSavingDeposit(payload, out var goalId, out var amount))
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Payload tabungan tidak valid",
                    new ErrorDetail("payload.amount", "REQUIRED"));
            }

            if (string.IsNullOrWhiteSpace(goalId))
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Goal ID wajib diisi",
                    new ErrorDetail("payload.goal_id", "REQUIRED"));
            }

            if (amount <= 0)
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Amount harus > 0",
                    new ErrorDetail("payload.amount", "OUT_OF_RANGE"));
            }

            if (string.Equals(actionType, "saving.deposit.created", StringComparison.OrdinalIgnoreCase) &&
                amount > RulebookSavingMaxDeposit)
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Maksimal tabungan per aksi adalah 15 koin");
            }

            var events = await _events.GetAllEventsBySessionAsync(request.SessionId, ct);
            var balance = ComputeSavingBalance(events, request.PlayerId.Value, goalId);

            if (string.Equals(actionType, "saving.deposit.withdrawn", StringComparison.OrdinalIgnoreCase) && balance < amount)
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Saldo tabungan tidak mencukupi");
            }

            if (string.Equals(actionType, "saving.deposit.created", StringComparison.OrdinalIgnoreCase))
            {
                var balanceCheck = await EnsureSufficientBalanceAsync(request, config, amount, ct);
                if (!balanceCheck.IsValid)
                {
                    return balanceCheck;
                }
            }

            return Valid;
        }

        if (string.Equals(actionType, "saving.goal.achieved", StringComparison.OrdinalIgnoreCase))
        {
            if (!config.SavingGoalEnabled)
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Fitur tabungan tujuan tidak aktif");
            }

            if (request.PlayerId is null)
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Player wajib diisi",
                    new ErrorDetail("player_id", "REQUIRED"));
            }

            if (!TryReadSavingGoalAchieved(payload, out var goalId, out var points, out var cost))
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Payload goal tidak valid",
                    new ErrorDetail("payload.goal_id", "REQUIRED"));
            }

            if (points < 0)
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Points tidak valid",
                    new ErrorDetail("payload.points", "OUT_OF_RANGE"));
            }

            if (cost < 0)
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Cost tidak valid",
                    new ErrorDetail("payload.cost", "OUT_OF_RANGE"));
            }

            var events = await _events.GetAllEventsBySessionAsync(request.SessionId, ct);
            var balance = ComputeSavingBalance(events, request.PlayerId.Value, goalId);
            if (cost > 0 && balance < cost)
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Saldo tabungan tidak mencukupi untuk goal");
            }

            return Valid;
        }

        if (string.Equals(actionType, "risk.life.drawn", StringComparison.OrdinalIgnoreCase))
        {
            if (!string.Equals(config.Mode, "MAHIR", StringComparison.OrdinalIgnoreCase))
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Fitur risiko hanya tersedia di mode MAHIR");
            }

            if (request.PlayerId is null)
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Player wajib diisi",
                    new ErrorDetail("player_id", "REQUIRED"));
            }

            if (!TryReadRiskLife(payload, out var riskId, out var direction, out var amount))
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Payload risiko tidak valid",
                    new ErrorDetail("payload.amount", "REQUIRED"));
            }

            if (string.IsNullOrWhiteSpace(riskId))
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Risk ID wajib diisi",
                    new ErrorDetail("payload.risk_id", "REQUIRED"));
            }

            if (amount <= 0)
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Amount harus > 0",
                    new ErrorDetail("payload.amount", "OUT_OF_RANGE"));
            }

            var events = await _events.GetAllEventsBySessionAsync(request.SessionId, ct);
            var turnEvents = events.Where(e =>
                e.PlayerId == request.PlayerId &&
                e.TurnNumber == request.TurnNumber);

            var orderCount = turnEvents.Count(e => e.ActionType == "order.claimed");
            var riskCount = turnEvents.Count(e => e.ActionType == "risk.life.drawn");

            if (riskCount >= orderCount)
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION",
                    "Risiko hanya dapat diambil setelah klaim pesanan");
            }

            if (string.Equals(direction, "OUT", StringComparison.OrdinalIgnoreCase))
            {
                var balanceCheck = await EnsureSufficientBalanceAsync(request, config, amount, ct);
                if (!balanceCheck.IsValid)
                {
                    return balanceCheck;
                }
            }

            return Valid;
        }

        if (string.Equals(actionType, "insurance.multirisk.used", StringComparison.OrdinalIgnoreCase))
        {
            if (!config.InsuranceEnabled)
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Fitur asuransi tidak aktif");
            }

            if (request.PlayerId is null)
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Player wajib diisi",
                    new ErrorDetail("player_id", "REQUIRED"));
            }

            if (!TryReadInsuranceUsed(payload, out var riskEventIdText))
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Payload insurance used tidak valid",
                    new ErrorDetail("payload.risk_event_id", "REQUIRED"));
            }

            if (!Guid.TryParse(riskEventIdText, out var riskEventId))
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Risk event id tidak valid",
                    new ErrorDetail("payload.risk_event_id", "INVALID_FORMAT"));
            }

            var events = await _events.GetAllEventsBySessionAsync(request.SessionId, ct);
            var riskEvent = events.FirstOrDefault(e => e.EventId == riskEventId);
            if (riskEvent is null || !string.Equals(riskEvent.ActionType, "risk.life.drawn", StringComparison.OrdinalIgnoreCase))
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Risk event tidak ditemukan");
            }

            if (riskEvent.PlayerId != request.PlayerId)
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Risk event bukan milik pemain");
            }

            var riskPayload = ReadPayload(riskEvent.Payload);
            if (!TryReadRiskLife(riskPayload, out _, out var direction, out var amount) ||
                !string.Equals(direction, "OUT", StringComparison.OrdinalIgnoreCase) ||
                amount <= 0)
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Asuransi hanya berlaku untuk risiko OUT");
            }

            var alreadyUsed = events.Any(e =>
                string.Equals(e.ActionType, "insurance.multirisk.used", StringComparison.OrdinalIgnoreCase) &&
                TryReadInsuranceUsed(ReadPayload(e.Payload), out var usedRiskEventId) &&
                string.Equals(usedRiskEventId, riskEventIdText, StringComparison.OrdinalIgnoreCase));

            if (alreadyUsed)
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Risk event sudah ditangkal asuransi");
            }

            return Valid;
        }

        if (string.Equals(actionType, "risk.emergency.used", StringComparison.OrdinalIgnoreCase))
        {
            if (request.PlayerId is null)
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Player wajib diisi",
                    new ErrorDetail("player_id", "REQUIRED"));
            }

            if (!TryReadEmergencyOption(payload, out var riskEventIdText, out var optionType, out var direction, out var amount))
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Payload emergency option tidak valid",
                    new ErrorDetail("payload.risk_event_id", "REQUIRED"));
            }

            if (!Guid.TryParse(riskEventIdText, out var riskEventId))
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Risk event id tidak valid",
                    new ErrorDetail("payload.risk_event_id", "INVALID_FORMAT"));
            }

            if (amount <= 0)
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Amount harus > 0",
                    new ErrorDetail("payload.amount", "OUT_OF_RANGE"));
            }

            if (!string.Equals(direction, "IN", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(direction, "OUT", StringComparison.OrdinalIgnoreCase))
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Direction tidak valid",
                    new ErrorDetail("payload.direction", "INVALID_ENUM"));
            }

            var allowedOptions = new[] { "SELL_NEED", "SELL_GOLD", "SELL_GOAL", "OTHER" };
            if (string.IsNullOrWhiteSpace(optionType) ||
                !allowedOptions.Any(opt => string.Equals(opt, optionType, StringComparison.OrdinalIgnoreCase)))
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Option type tidak valid",
                    new ErrorDetail("payload.option_type", "INVALID_ENUM"));
            }

            var events = await _events.GetAllEventsBySessionAsync(request.SessionId, ct);
            var riskEvent = events.FirstOrDefault(e => e.EventId == riskEventId);
            if (riskEvent is null || !string.Equals(riskEvent.ActionType, "risk.life.drawn", StringComparison.OrdinalIgnoreCase))
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Risk event tidak ditemukan");
            }

            if (riskEvent.PlayerId != request.PlayerId)
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Risk event bukan milik pemain");
            }

            var riskPayload = ReadPayload(riskEvent.Payload);
            if (!TryReadRiskLife(riskPayload, out _, out var riskDirection, out _) ||
                !string.Equals(riskDirection, "OUT", StringComparison.OrdinalIgnoreCase))
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Emergency option hanya berlaku untuk risiko OUT");
            }

            if (string.Equals(direction, "OUT", StringComparison.OrdinalIgnoreCase))
            {
                var balanceCheck = await EnsureSufficientBalanceAsync(request, config, amount, ct);
                if (!balanceCheck.IsValid)
                {
                    return balanceCheck;
                }
            }

            return Valid;
        }

        if (string.Equals(actionType, "loan.syariah.taken", StringComparison.OrdinalIgnoreCase))
        {
            if (!config.LoanEnabled)
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Fitur pinjaman tidak aktif");
            }

            if (request.PlayerId is null)
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Player wajib diisi",
                    new ErrorDetail("player_id", "REQUIRED"));
            }

            if (!TryReadLoanTaken(payload, out var loanId, out var principal, out var installment, out var duration, out var penaltyPoints))
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Payload loan taken tidak valid",
                    new ErrorDetail("payload.loan_id", "REQUIRED"));
            }

            if (principal <= 0 || installment <= 0 || duration <= 0)
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Nilai pinjaman tidak valid",
                    new ErrorDetail("payload.principal", "OUT_OF_RANGE"));
            }

            if (penaltyPoints < 0)
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Penalty points tidak valid",
                    new ErrorDetail("payload.penalty_points", "OUT_OF_RANGE"));
            }

            if (principal != RulebookLoanPrincipal)
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Nilai pinjaman harus 10 koin");
            }

            if (penaltyPoints != RulebookLoanPenaltyPoints)
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Penalty pinjaman harus 15 poin");
            }

            var events = await _events.GetAllEventsBySessionAsync(request.SessionId, ct);
            var exists = events.Any(e =>
                e.PlayerId == request.PlayerId &&
                e.ActionType == "loan.syariah.taken" &&
                TryReadLoanTaken(ReadPayload(e.Payload), out var existingLoanId, out _, out _, out _, out _) &&
                string.Equals(existingLoanId, loanId, StringComparison.OrdinalIgnoreCase));
            if (exists)
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Loan ID sudah dipakai");
            }

            return Valid;
        }

        if (string.Equals(actionType, "loan.syariah.repaid", StringComparison.OrdinalIgnoreCase))
        {
            if (!config.LoanEnabled)
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Fitur pinjaman tidak aktif");
            }

            if (request.PlayerId is null)
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Player wajib diisi",
                    new ErrorDetail("player_id", "REQUIRED"));
            }

            if (!TryReadLoanRepay(payload, out var loanId, out var amount))
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Payload loan repaid tidak valid",
                    new ErrorDetail("payload.loan_id", "REQUIRED"));
            }

            if (amount <= 0)
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Amount harus > 0",
                    new ErrorDetail("payload.amount", "OUT_OF_RANGE"));
            }

            var events = await _events.GetAllEventsBySessionAsync(request.SessionId, ct);
            var loan = events.FirstOrDefault(e =>
                e.PlayerId == request.PlayerId &&
                e.ActionType == "loan.syariah.taken" &&
                TryReadLoanTaken(ReadPayload(e.Payload), out var existingLoanId, out _, out _, out _, out _) &&
                string.Equals(existingLoanId, loanId, StringComparison.OrdinalIgnoreCase));

            if (loan is null)
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Loan ID tidak ditemukan");
            }

            var principal = 0;
            if (TryReadLoanTaken(ReadPayload(loan.Payload), out _, out var principalValue, out _, out _, out _))
            {
                principal = principalValue;
            }

            var repaidSoFar = events.Where(e =>
                    e.PlayerId == request.PlayerId &&
                    e.ActionType == "loan.syariah.repaid" &&
                    TryReadLoanRepay(ReadPayload(e.Payload), out var existingLoanId, out _) &&
                    string.Equals(existingLoanId, loanId, StringComparison.OrdinalIgnoreCase))
                .Sum(e => TryReadLoanRepay(ReadPayload(e.Payload), out _, out var repaidAmount) ? repaidAmount : 0);

            if (principal > 0 && repaidSoFar + amount > principal)
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Pembayaran melebihi sisa pinjaman");
            }

            var balanceCheck = await EnsureSufficientBalanceAsync(request, config, amount, ct);
            if (!balanceCheck.IsValid)
            {
                return balanceCheck;
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

            if (premium != RulebookInsurancePremium)
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Premium asuransi harus 1 koin");
            }

            if (request.PlayerId is not null)
            {
                var balanceCheck = await EnsureSufficientBalanceAsync(request, config, premium, ct);
                if (!balanceCheck.IsValid)
                {
                    return balanceCheck;
                }
            }

            return Valid;
        }

        return Valid;
    }

    /// <summary>
    /// Menjalankan fungsi TryGetString sebagai bagian dari alur file ini.
    /// </summary>
    private static bool TryGetString(JsonElement payload, string propertyName, out string value)
    {
        value = string.Empty;
        if (!payload.TryGetProperty(propertyName, out var property))
        {
            return false;
        }

        if (property.ValueKind is JsonValueKind.String or JsonValueKind.Null)
        {
            value = property.GetString() ?? string.Empty;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Menjalankan fungsi TryGetOptionalString sebagai bagian dari alur file ini.
    /// </summary>
    private static bool TryGetOptionalString(JsonElement payload, string propertyName, out string? value)
    {
        value = null;
        if (!payload.TryGetProperty(propertyName, out var property))
        {
            return true;
        }

        if (property.ValueKind is JsonValueKind.String or JsonValueKind.Null)
        {
            value = property.GetString();
            return true;
        }

        return false;
    }

    /// <summary>
    /// Menjalankan fungsi TryGetInt32 sebagai bagian dari alur file ini.
    /// </summary>
    private static bool TryGetInt32(JsonElement payload, string propertyName, out int value, bool required = true)
    {
        value = 0;
        if (!payload.TryGetProperty(propertyName, out var property))
        {
            return !required;
        }

        if (property.ValueKind != JsonValueKind.Number || !property.TryGetInt32(out value))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Menjalankan fungsi TryGetDouble sebagai bagian dari alur file ini.
    /// </summary>
    private static bool TryGetDouble(JsonElement payload, string propertyName, out double value, bool required = true)
    {
        value = 0;
        if (!payload.TryGetProperty(propertyName, out var property))
        {
            return !required;
        }

        if (property.ValueKind != JsonValueKind.Number || !property.TryGetDouble(out value))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Menjalankan fungsi TryReadTransaction sebagai bagian dari alur file ini.
    /// </summary>
    private static bool TryReadTransaction(JsonElement payload, out string direction, out double amount, out string category, out string? counterparty)
    {
        direction = string.Empty;
        category = string.Empty;
        counterparty = null;
        amount = 0;

        if (!TryGetString(payload, "direction", out direction) ||
            !TryGetDouble(payload, "amount", out amount) ||
            !TryGetString(payload, "category", out category))
        {
            return false;
        }

        if (!TryGetOptionalString(payload, "counterparty", out counterparty))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Menjalankan fungsi TryReadAmount sebagai bagian dari alur file ini.
    /// </summary>
    private static bool TryReadAmount(JsonElement payload, out double amount)
    {
        return TryGetDouble(payload, "amount", out amount);
    }

    /// <summary>
    /// Menjalankan fungsi TryReadGoldTrade sebagai bagian dari alur file ini.
    /// </summary>
    private static bool TryReadGoldTrade(JsonElement payload, out string tradeType, out int qty, out int unitPrice, out int amount)
    {
        tradeType = string.Empty;
        qty = 0;
        unitPrice = 0;
        amount = 0;

        if (!TryGetString(payload, "trade_type", out tradeType) ||
            !TryGetInt32(payload, "qty", out qty) ||
            !TryGetInt32(payload, "unit_price", out unitPrice) ||
            !TryGetInt32(payload, "amount", out amount))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Menjalankan fungsi TryReadActionUsed sebagai bagian dari alur file ini.
    /// </summary>
    private static bool TryReadActionUsed(JsonElement payload, out int used, out int remaining)
    {
        used = 0;
        remaining = 0;
        if (!TryGetInt32(payload, "used", out used) ||
            !TryGetInt32(payload, "remaining", out remaining))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Menjalankan fungsi TryReadIngredientPurchase sebagai bagian dari alur file ini.
    /// </summary>
    private static bool TryReadIngredientPurchase(JsonElement payload, out string cardId, out int amount)
    {
        cardId = string.Empty;
        amount = 0;

        if (!TryGetString(payload, "card_id", out cardId) ||
            !TryGetInt32(payload, "amount", out amount))
        {
            return false;
        }

        return !string.IsNullOrWhiteSpace(cardId);
    }

    /// <summary>
    /// Menjalankan fungsi TryReadOrderClaim sebagai bagian dari alur file ini.
    /// </summary>
    private static bool TryReadOrderClaim(JsonElement payload, out List<string> requiredCards, out int income)
    {
        requiredCards = new List<string>();
        income = 0;

        if (!payload.TryGetProperty("required_ingredient_card_ids", out var cardsProp) ||
            cardsProp.ValueKind != JsonValueKind.Array ||
            !TryGetInt32(payload, "income", out income))
        {
            return false;
        }

        foreach (var item in cardsProp.EnumerateArray())
        {
            if (item.ValueKind != JsonValueKind.String)
            {
                return false;
            }

            var cardId = item.GetString();
            if (!string.IsNullOrWhiteSpace(cardId))
            {
                requiredCards.Add(cardId);
            }
        }

        return requiredCards.Count > 0;
    }

    /// <summary>
    /// Menjalankan fungsi TryReadNeedPurchase sebagai bagian dari alur file ini.
    /// </summary>
    private static bool TryReadNeedPurchase(JsonElement payload, out string cardId, out int amount, out int points)
    {
        cardId = string.Empty;
        amount = 0;
        points = 0;

        if (!TryGetString(payload, "card_id", out cardId) ||
            !TryGetInt32(payload, "amount", out amount) ||
            !TryGetInt32(payload, "points", out points, required: false))
        {
            return false;
        }

        return !string.IsNullOrWhiteSpace(cardId);
    }

    /// <summary>
    /// Menjalankan fungsi TryReadMissionAssigned sebagai bagian dari alur file ini.
    /// </summary>
    private static bool TryReadMissionAssigned(JsonElement payload, out string missionId, out string targetCardId, out int penaltyPoints)
    {
        missionId = string.Empty;
        targetCardId = string.Empty;
        penaltyPoints = 0;

        if (!TryGetString(payload, "mission_id", out missionId) ||
            !TryGetString(payload, "target_tertiary_card_id", out targetCardId) ||
            !TryGetInt32(payload, "penalty_points", out penaltyPoints))
        {
            return false;
        }

        return !string.IsNullOrWhiteSpace(missionId);
    }

    /// <summary>
    /// Menjalankan fungsi TryReadTieBreaker sebagai bagian dari alur file ini.
    /// </summary>
    private static bool TryReadTieBreaker(JsonElement payload, out int number)
    {
        return TryGetInt32(payload, "number", out number);
    }

    /// <summary>
    /// Menjalankan fungsi TryReadRankAwarded sebagai bagian dari alur file ini.
    /// </summary>
    private static bool TryReadRankAwarded(JsonElement payload, out int rank, out int points)
    {
        rank = 0;
        points = 0;
        if (!TryGetInt32(payload, "rank", out rank) ||
            !TryGetInt32(payload, "points", out points))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Menjalankan fungsi TryReadPointsAwarded sebagai bagian dari alur file ini.
    /// </summary>
    private static bool TryReadPointsAwarded(JsonElement payload, out int points)
    {
        return TryGetInt32(payload, "points", out points);
    }

    /// <summary>
    /// Menjalankan fungsi TryReadSavingDeposit sebagai bagian dari alur file ini.
    /// </summary>
    private static bool TryReadSavingDeposit(JsonElement payload, out string goalId, out int amount)
    {
        goalId = string.Empty;
        amount = 0;
        if (!TryGetString(payload, "goal_id", out goalId) ||
            !TryGetInt32(payload, "amount", out amount))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Menjalankan fungsi TryReadSavingGoalAchieved sebagai bagian dari alur file ini.
    /// </summary>
    private static bool TryReadSavingGoalAchieved(JsonElement payload, out string goalId, out int points, out int cost)
    {
        goalId = string.Empty;
        points = 0;
        cost = 0;
        if (!TryGetString(payload, "goal_id", out goalId) ||
            !TryGetInt32(payload, "points", out points) ||
            !TryGetInt32(payload, "cost", out cost, required: false))
        {
            return false;
        }

        return !string.IsNullOrWhiteSpace(goalId);
    }

    /// <summary>
    /// Menjalankan fungsi TryReadRiskLife sebagai bagian dari alur file ini.
    /// </summary>
    private static bool TryReadRiskLife(JsonElement payload, out string riskId, out string direction, out int amount)
    {
        riskId = string.Empty;
        direction = string.Empty;
        amount = 0;
        if (!TryGetString(payload, "risk_id", out riskId) ||
            !TryGetString(payload, "direction", out direction) ||
            !TryGetInt32(payload, "amount", out amount))
        {
            return false;
        }

        return direction.Equals("IN", StringComparison.OrdinalIgnoreCase) ||
               direction.Equals("OUT", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Menjalankan fungsi TryReadInsuranceUsed sebagai bagian dari alur file ini.
    /// </summary>
    private static bool TryReadInsuranceUsed(JsonElement payload, out string riskEventId)
    {
        riskEventId = string.Empty;
        if (!TryGetString(payload, "risk_event_id", out riskEventId))
        {
            return false;
        }

        return !string.IsNullOrWhiteSpace(riskEventId);
    }

    /// <summary>
    /// Menjalankan fungsi TryReadEmergencyOption sebagai bagian dari alur file ini.
    /// </summary>
    private static bool TryReadEmergencyOption(
        JsonElement payload,
        out string riskEventId,
        out string optionType,
        out string direction,
        out int amount)
    {
        riskEventId = string.Empty;
        optionType = string.Empty;
        direction = string.Empty;
        amount = 0;

        if (!TryGetString(payload, "risk_event_id", out riskEventId) ||
            !TryGetString(payload, "option_type", out optionType) ||
            !TryGetString(payload, "direction", out direction) ||
            !TryGetInt32(payload, "amount", out amount))
        {
            return false;
        }

        return !string.IsNullOrWhiteSpace(riskEventId);
    }

    /// <summary>
    /// Menjalankan fungsi TryReadLoanTaken sebagai bagian dari alur file ini.
    /// </summary>
    private static bool TryReadLoanTaken(
        JsonElement payload,
        out string loanId,
        out int principal,
        out int installment,
        out int duration,
        out int penaltyPoints)
    {
        loanId = string.Empty;
        principal = 0;
        installment = 0;
        duration = 0;
        penaltyPoints = 0;

        if (!TryGetString(payload, "loan_id", out loanId) ||
            !TryGetInt32(payload, "principal", out principal) ||
            !TryGetInt32(payload, "installment", out installment) ||
            !TryGetInt32(payload, "duration_turn", out duration) ||
            !TryGetInt32(payload, "penalty_points", out penaltyPoints))
        {
            return false;
        }

        return !string.IsNullOrWhiteSpace(loanId);
    }

    /// <summary>
    /// Menjalankan fungsi TryReadLoanRepay sebagai bagian dari alur file ini.
    /// </summary>
    private static bool TryReadLoanRepay(JsonElement payload, out string loanId, out int amount)
    {
        loanId = string.Empty;
        amount = 0;
        if (!TryGetString(payload, "loan_id", out loanId) ||
            !TryGetInt32(payload, "amount", out amount))
        {
            return false;
        }

        return !string.IsNullOrWhiteSpace(loanId);
    }

    /// <summary>
    /// Menjalankan fungsi TryReadInsurance sebagai bagian dari alur file ini.
    /// </summary>
    private static bool TryReadInsurance(JsonElement payload, out int premium)
    {
        return TryGetInt32(payload, "premium", out premium);
    }

    /// <summary>
    /// Menjalankan fungsi BuildIngredientInventory sebagai bagian dari alur file ini.
    /// </summary>
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

            if (evt.ActionType == "ingredient.discarded" &&
                TryReadIngredientPurchase(ReadPayload(evt.Payload), out var discardCardId, out var discardAmount))
            {
                if (inventory.ByCardId.TryGetValue(discardCardId, out var qty))
                {
                    var newQty = Math.Max(0, qty - discardAmount);
                    inventory.ByCardId[discardCardId] = newQty;
                    inventory.Total = Math.Max(0, inventory.Total - discardAmount);
                }
            }
        }

        return inventory;
    }

    /// <summary>
    /// Menjalankan fungsi ComputeSavingBalance sebagai bagian dari alur file ini.
    /// </summary>
    private static int ComputeSavingBalance(IEnumerable<EventDb> events, Guid playerId, string goalId)
    {
        var balance = 0;

        foreach (var evt in events.Where(e => e.PlayerId == playerId))
        {
            if (evt.ActionType == "saving.deposit.created" &&
                TryReadSavingDeposit(ReadPayload(evt.Payload), out var existingGoalId, out var amount) &&
                string.Equals(existingGoalId, goalId, StringComparison.OrdinalIgnoreCase))
            {
                balance += amount;
            }

            if (evt.ActionType == "saving.deposit.withdrawn" &&
                TryReadSavingDeposit(ReadPayload(evt.Payload), out var withdrawGoalId, out var amountWithdraw) &&
                string.Equals(withdrawGoalId, goalId, StringComparison.OrdinalIgnoreCase))
            {
                balance -= amountWithdraw;
            }

            if (evt.ActionType == "saving.goal.achieved" &&
                TryReadSavingGoalAchieved(ReadPayload(evt.Payload), out var achievedGoalId, out _, out var cost) &&
                string.Equals(achievedGoalId, goalId, StringComparison.OrdinalIgnoreCase))
            {
                balance -= cost;
            }
        }

        return balance;
    }

    /// <summary>
    /// Menjalankan fungsi ReadPayload sebagai bagian dari alur file ini.
    /// </summary>
    private static JsonElement ReadPayload(string payload)
    {
        using var doc = JsonDocument.Parse(payload);
        return doc.RootElement.Clone();
    }

    /// <summary>
    /// Menjalankan fungsi BuildValidationDetailsJson sebagai bagian dari alur file ini.
    /// </summary>
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

    /// <summary>
    /// Menyatakan peran utama tipe IngredientInventory pada modul ini.
    /// </summary>
    private sealed class IngredientInventory
    {
        internal int Total { get; set; }
        /// <summary>
        /// Menjalankan fungsi new sebagai bagian dari alur file ini.
        /// </summary>
        internal Dictionary<string, int> ByCardId { get; } = new(StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Menjalankan fungsi EnsureSufficientBalanceAsync sebagai bagian dari alur file ini.
    /// </summary>
    private async Task<ValidationOutcome> EnsureSufficientBalanceAsync(
        EventRequest request,
        RulesetConfig config,
        double outgoingAmount,
        CancellationToken ct)
    {
        if (request.PlayerId is null)
        {
            return Valid;
        }

        var currentBalance = await GetPlayerBalanceAsync(request.SessionId, request.PlayerId.Value, config.StartingCash, ct);
        var projectedBalance = currentBalance - outgoingAmount;

        if (projectedBalance < config.CashMin)
        {
            return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Saldo tidak mencukupi");
        }

        return Valid;
    }

    /// <summary>
    /// Menjalankan fungsi ValidateInstructorSessionAccessAsync sebagai bagian dari alur file ini.
    /// </summary>
    private async Task<ValidationOutcome> ValidateInstructorSessionAccessAsync(Guid sessionId, CancellationToken ct)
    {
        var role = User.FindFirstValue(ClaimTypes.Role);
        if (!string.Equals(role, "INSTRUCTOR", StringComparison.OrdinalIgnoreCase))
        {
            return Valid;
        }

        var userIdRaw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdRaw, out var instructorUserId))
        {
            return BuildOutcome(StatusCodes.Status401Unauthorized, "UNAUTHORIZED", "Token user tidak valid");
        }

        var ownedSession = await _sessions.GetSessionForInstructorAsync(sessionId, instructorUserId, ct);
        if (ownedSession is null)
        {
            return BuildOutcome(StatusCodes.Status404NotFound, "NOT_FOUND", "Session tidak ditemukan");
        }

        return Valid;
    }

    /// <summary>
    /// Menjalankan fungsi GetPlayerBalanceAsync sebagai bagian dari alur file ini.
    /// </summary>
    private async Task<double> GetPlayerBalanceAsync(Guid sessionId, Guid playerId, int startingCash, CancellationToken ct)
    {
        var projections = await _events.GetCashflowProjectionsAsync(sessionId, ct);
        var net = projections
            .Where(p => p.PlayerId == playerId)
            .Sum(p => p.Direction == "IN" ? p.Amount : -p.Amount);

        return startingCash + net;
    }

    /// <summary>
    /// Menjalankan fungsi TryBuildCashflowProjection sebagai bagian dari alur file ini.
    /// </summary>
    private static bool TryBuildCashflowProjection(EventRequest request, DateTimeOffset timestamp, Guid eventPk, out CashflowProjectionDb projection)
    {
        projection = default!;
        if (request.PlayerId is null)
        {
            return false;
        }

        var action = request.ActionType;
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
                 TryReadIngredientPurchase(request.Payload, out _, out var ingredientAmount))
        {
            direction = "OUT";
            amount = ingredientAmount;
            category = "INGREDIENT";
        }
        else if (string.Equals(action, "order.claimed", StringComparison.OrdinalIgnoreCase) &&
                 TryReadOrderClaim(request.Payload, out _, out var income))
        {
            direction = "IN";
            amount = income;
            category = "ORDER";
        }
        else if (string.Equals(action, "work.freelance.completed", StringComparison.OrdinalIgnoreCase) &&
                 TryReadAmount(request.Payload, out var freelanceAmount))
        {
            direction = "IN";
            amount = (int)Math.Round(freelanceAmount);
            category = "FREELANCE";
        }
        else if (string.Equals(action, "need.primary.purchased", StringComparison.OrdinalIgnoreCase) &&
                 TryReadNeedPurchase(request.Payload, out _, out var primaryAmount, out _))
        {
            direction = "OUT";
            amount = primaryAmount;
            category = "NEED_PRIMARY";
        }
        else if (string.Equals(action, "need.secondary.purchased", StringComparison.OrdinalIgnoreCase) &&
                 TryReadNeedPurchase(request.Payload, out _, out var secondaryAmount, out _))
        {
            direction = "OUT";
            amount = secondaryAmount;
            category = "NEED_SECONDARY";
        }
        else if (string.Equals(action, "need.tertiary.purchased", StringComparison.OrdinalIgnoreCase) &&
                 TryReadNeedPurchase(request.Payload, out _, out var tertiaryAmount, out _))
        {
            direction = "OUT";
            amount = tertiaryAmount;
            category = "NEED_TERTIARY";
        }
        else if (string.Equals(action, "saving.deposit.created", StringComparison.OrdinalIgnoreCase) &&
                 TryReadSavingDeposit(request.Payload, out _, out var savingAmount))
        {
            direction = "OUT";
            amount = savingAmount;
            category = "SAVING_DEPOSIT";
        }
        else if (string.Equals(action, "saving.deposit.withdrawn", StringComparison.OrdinalIgnoreCase) &&
                 TryReadSavingDeposit(request.Payload, out _, out var savingWithdrawAmount))
        {
            direction = "IN";
            amount = savingWithdrawAmount;
            category = "SAVING_WITHDRAW";
        }
        else if (string.Equals(action, "risk.life.drawn", StringComparison.OrdinalIgnoreCase) &&
                 TryReadRiskLife(request.Payload, out _, out var riskDirection, out var riskAmount))
        {
            direction = riskDirection.ToUpperInvariant();
            amount = riskAmount;
            category = "RISK_LIFE";
        }
        else if (string.Equals(action, "loan.syariah.taken", StringComparison.OrdinalIgnoreCase) &&
                 TryReadLoanTaken(request.Payload, out _, out var principal, out _, out _, out _))
        {
            direction = "IN";
            amount = principal;
            category = "LOAN_TAKEN";
        }
        else if (string.Equals(action, "loan.syariah.repaid", StringComparison.OrdinalIgnoreCase) &&
                 TryReadLoanRepay(request.Payload, out _, out var repayAmount))
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
        else if (string.Equals(action, "risk.emergency.used", StringComparison.OrdinalIgnoreCase) &&
                 TryReadEmergencyOption(request.Payload, out _, out _, out var emergencyDirection, out var emergencyAmount))
        {
            direction = emergencyDirection.ToUpperInvariant();
            amount = emergencyAmount;
            category = "EMERGENCY_OPTION";
        }
        else
        {
            return false;
        }

        if (amount <= 0 || string.IsNullOrWhiteSpace(direction))
        {
            return false;
        }

        projection = new CashflowProjectionDb
        {
            ProjectionId = Guid.NewGuid(),
            SessionId = request.SessionId,
            PlayerId = playerId,
            EventPk = eventPk,
            EventId = request.EventId,
            Timestamp = timestamp,
            Direction = direction,
            Amount = amount,
            Category = category,
            Counterparty = counterparty,
            Reference = reference,
            Note = note
        };

        return true;
    }
}
