// Fungsi file: Controller ingest event gameplay — menerima, memvalidasi aturan domain, dan menyimpan event beserta proyeksi arus kas.
using System.Collections.Frozen;
using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Domain;
using Cashflowpoly.Api.Infrastructure;
using Cashflowpoly.Contracts;
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
/// Controller REST untuk ingest event tunggal/batch, validasi aturan domain per action type, dan query histori event sesi.
/// </summary>
public sealed class EventsController : ControllerBase
{
    /// <summary>
    /// Hasil validasi event: valid/tidak, kode status HTTP, dan detail error jika gagal.
    /// </summary>
    private sealed record ValidationOutcome(bool IsValid, int StatusCode, ErrorResponse? Error);
    /// <summary>
    /// Hasil validasi akses sesi: mencakup status, error, dan scoped player ID untuk role PLAYER.
    /// </summary>
    private sealed record SessionAccessOutcome(bool IsValid, int StatusCode, ErrorResponse? Error, Guid? ScopedPlayerId);

    /// <summary>
    /// Singleton outcome validasi sukses untuk menghindari alokasi berulang.
    /// </summary>
    private static readonly ValidationOutcome Valid = new(true, StatusCodes.Status200OK, null);
    /// <summary>
    /// Singleton outcome akses sesi sukses (tanpa scope player) untuk instruktur.
    /// </summary>
    private static readonly SessionAccessOutcome AccessValid = new(true, StatusCodes.Status200OK, null, null);

    private static readonly EventPayloadReader _payloadReader = new();
    private static readonly EventValidationDetailsSerializer _validationSerializer = new();
    private static readonly EventRecordMapper _recordMapper = new();
    private static readonly EventRequestShapeValidator _shapeValidator = new();
    private static readonly EventInsuranceOffsetBuilder _insuranceOffsetBuilder = new();
    private static readonly EventCashflowProjectionBuilder _projectionBuilder = new();
    private static readonly EventSimpleActionValidator _simpleActionValidator = new();
    private static readonly EventTurnProgressValidator _turnProgressValidator = new();
    private static readonly EventNeedPurchaseValidator _needPurchaseValidator = new();
    private static readonly EventIngredientOrderValidator _ingredientOrderValidator = new();
    private static readonly EventSavingGoalValidator _savingGoalValidator = new();
    private static readonly EventEconomyActionValidator _economyActionValidator = new();
    private static readonly EventAssignmentValidator _assignmentValidator = new();
    private static readonly EventPlayerBalanceCalculator _playerBalanceCalc = new();

    private readonly SessionRepository _sessions;
    private readonly RulesetRepository _rulesets;
    private readonly EventRepository _events;
    private readonly PlayerRepository _players;
    private readonly UserRepository _users;

    /// <summary>
    /// Controller untuk ingest event dan akses histori event.
    /// </summary>
    public EventsController(
        SessionRepository sessions,
        RulesetRepository rulesets,
        EventRepository events,
        PlayerRepository players,
        UserRepository users)
    {
        _sessions = sessions;
        _rulesets = rulesets;
        _events = events;
        _players = players;
        _users = users;
    }

    /// <summary>
    /// Opsi emergency action yang diperbolehkan oleh aturan domain.
    /// </summary>
    private static readonly FrozenSet<string> AllowedEmergencyOptions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "SELL_NEED", "SELL_GOLD", "SELL_GOAL", "OTHER"
    }.ToFrozenSet(StringComparer.OrdinalIgnoreCase);

    private const int RulebookLoanPrincipal = 10;
    private const int RulebookLoanPenaltyPoints = 15;
    private const int RulebookInsurancePremium = 1;

    [HttpPost("events")]
    [ProducesResponseType(typeof(EventStoredResponse), StatusCodes.Status201Created)]
    /// <summary>
    /// Menerima satu event gameplay, memvalidasi aturan domain, menyimpan ke database, dan mencatat log validasi.
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
                _validationSerializer.BuildValidationDetailsJson(request, validation.Error),
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
                _validationSerializer.BuildValidationDetailsJson(request, error), ct);
            return Conflict(error);
        }
    }

    [HttpPost("events/batch")]
    [ProducesResponseType(typeof(EventBatchResponse), StatusCodes.Status200OK)]
    /// <summary>
    /// Menerima batch event gameplay, memvalidasi masing-masing, dan mengembalikan ringkasan sukses/gagal.
    /// </summary>
    public async Task<IActionResult> CreateEventsBatch([FromBody] EventBatchRequest request, CancellationToken ct)
    {
        const int MaxBatchSize = 500;

        if (request.Events is null || request.Events.Count == 0)
        {
            return BadRequest(ApiErrorHelper.BuildError(HttpContext, "VALIDATION_ERROR", "Daftar event batch wajib diisi",
                new ErrorDetail("events", "REQUIRED")));
        }

        if (request.Events.Count > MaxBatchSize)
        {
            return BadRequest(ApiErrorHelper.BuildError(HttpContext, "VALIDATION_ERROR",
                "Batch maksimal 500 event",
                new ErrorDetail("events", "MAX_LENGTH")));
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
                    _validationSerializer.BuildValidationDetailsJson(evt, validation.Error),
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
                var dupError = ApiErrorHelper.BuildError(HttpContext, "DUPLICATE", "Event sudah ada");
                await _events.InsertValidationLogAsync(evt.SessionId, evt.EventId, null, false, dupError.ErrorCode, dupError.Message,
                    _validationSerializer.BuildValidationDetailsJson(evt, dupError), ct);
            }
        }

        return Ok(new EventBatchResponse(storedCount, failed));
    }

    [HttpGet("sessions/{sessionId:guid}/events")]
    [ProducesResponseType(typeof(EventsBySessionResponse), StatusCodes.Status200OK)]
    /// <summary>
    /// Mengambil daftar event sesi dengan pagination berbasis sequence number.
    /// </summary>
    public async Task<IActionResult> GetEventsBySession(Guid sessionId, [FromQuery] long fromSeq = 0, [FromQuery] int limit = 200, CancellationToken ct = default)
    {
        var accessScopeCheck = await ValidateSessionAccessAsync(sessionId, ct);
        if (!accessScopeCheck.IsValid)
        {
            return StatusCode(accessScopeCheck.StatusCode, accessScopeCheck.Error);
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
        var responseEvents = events.Select(_recordMapper.ToEventRequest).ToList();
        return Ok(new EventsBySessionResponse(sessionId, responseEvents));
    }

    /// <summary>
    /// Menjalankan seluruh pipeline validasi event: akses sesi, keberadaan entitas, enum, domain rules, dan duplikasi.
    /// </summary>
    private async Task<ValidationOutcome> ValidateEventAsync(EventRequest request, CancellationToken ct)
    {
        var accessScopeCheck = await ValidateSessionAccessAsync(request.SessionId, ct);
        if (!accessScopeCheck.IsValid)
        {
            return new ValidationOutcome(false, accessScopeCheck.StatusCode, accessScopeCheck.Error);
        }

        var session = await _sessions.GetSessionAsync(request.SessionId, ct);
        if (session is null)
        {
            return BuildOutcome(StatusCodes.Status404NotFound, "NOT_FOUND", "Session tidak ditemukan");
        }

        if (!string.Equals(session.Status, "STARTED", StringComparison.OrdinalIgnoreCase))
        {
            return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Session harus berstatus STARTED untuk menerima event");
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

        var shapeValidation = _shapeValidator.Validate(request, accessScopeCheck.ScopedPlayerId);
        if (!shapeValidation.IsValid)
        {
            return BuildOutcome(shapeValidation);
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
    /// Menyimpan event ke tabel events, membangun proyeksi arus kas, dan menangani offset asuransi jika berlaku.
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

        // Kumpulkan proyeksi sebelum transaksi
        CashflowProjectionDb? insuranceOffset = null;
        if (_insuranceOffsetBuilder.TryReadRiskEventReference(request, out _, out var riskEventId))
        {
            var riskEvent = await _events.GetEventByIdAsync(request.SessionId, riskEventId, ct);
            _insuranceOffsetBuilder.TryBuild(request, timestamp, eventPk, riskEvent, out insuranceOffset);
        }

        // Simpan event + proyeksi dalam satu transaksi
        await using var conn = await _events.OpenConnectionAsync(ct);
        await using var tx = await conn.BeginTransactionAsync(ct);

        await _events.InsertEventAsync(record, conn, tx, ct);

        if (_projectionBuilder.TryBuild(request, timestamp, eventPk, out var projection))
        {
            await _events.InsertCashflowProjectionAsync(projection, conn, tx, ct);
        }

        if (insuranceOffset is not null)
        {
            await _events.InsertCashflowProjectionAsync(insuranceOffset, conn, tx, ct);
        }

        await tx.CommitAsync(ct);

        return eventPk;
    }

    /// <summary>
    /// Membangun ValidationOutcome gagal dari kode status, kode error, dan pesan.
    /// </summary>
    private ValidationOutcome BuildOutcome(int statusCode, string code, string message, params ErrorDetail[] details)
    {
        var error = ApiErrorHelper.BuildError(HttpContext, code, message, details);
        return new ValidationOutcome(false, statusCode, error);
    }

    private ValidationOutcome BuildOutcome(EventDomainValidationResult result)
    {
        if (result.IsValid)
        {
            return Valid;
        }

        return BuildOutcome(result.StatusCode, result.ErrorCode!, result.Message!, result.Details.ToArray());
    }

    /// <summary>
    /// Memvalidasi aturan domain spesifik per action type terhadap konfigurasi ruleset aktif.
    /// </summary>
    private async Task<ValidationOutcome> ValidateDomainRulesAsync(EventRequest request, RulesetConfig config, CancellationToken ct)
    {
        var actionType = request.ActionType;
        var payload = request.Payload;

        if (_simpleActionValidator.TryValidate(request, config, out var simpleValidation))
        {
            return BuildOutcome(simpleValidation);
        }

        if (_turnProgressValidator.RequiresHistory(request, config))
        {
            var events = await _events.GetAllEventsBySessionAsync(request.SessionId, ct);
            if (_turnProgressValidator.TryValidate(request, config, events, out var turnValidation))
            {
                return BuildOutcome(turnValidation);
            }
        }

        if (string.Equals(actionType, "need.primary.purchased", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(actionType, "need.secondary.purchased", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(actionType, "need.tertiary.purchased", StringComparison.OrdinalIgnoreCase))
        {
            var events = await _events.GetAllEventsBySessionAsync(request.SessionId, ct);
            if (_needPurchaseValidator.TryValidate(request, config, events, out var needValidation))
            {
                if (!needValidation.Validation.IsValid)
                {
                    return BuildOutcome(needValidation.Validation);
                }

                if (needValidation.OutgoingAmount.HasValue && request.PlayerId is not null)
                {
                    var balanceCheck = await EnsureSufficientBalanceAsync(request, config, needValidation.OutgoingAmount.Value, ct);
                    if (!balanceCheck.IsValid)
                    {
                        return balanceCheck;
                    }
                }

                return Valid;
            }
        }

        if (string.Equals(actionType, "ingredient.purchased", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(actionType, "ingredient.discarded", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(actionType, "order.claimed", StringComparison.OrdinalIgnoreCase))
        {
            var events = await _events.GetAllEventsBySessionAsync(request.SessionId, ct);
            if (_ingredientOrderValidator.TryValidate(request, config, events, out var ingredientValidation))
            {
                if (!ingredientValidation.Validation.IsValid)
                {
                    return BuildOutcome(ingredientValidation.Validation);
                }

                if (ingredientValidation.OutgoingAmount.HasValue)
                {
                    var balanceCheck = await EnsureSufficientBalanceAsync(request, config, ingredientValidation.OutgoingAmount.Value, ct);
                    if (!balanceCheck.IsValid)
                    {
                        return balanceCheck;
                    }
                }

                return Valid;
            }
        }

        if (string.Equals(actionType, "saving.deposit.created", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(actionType, "saving.deposit.withdrawn", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(actionType, "saving.goal.achieved", StringComparison.OrdinalIgnoreCase))
        {
            var events = await _events.GetAllEventsBySessionAsync(request.SessionId, ct);
            if (_savingGoalValidator.TryValidate(request, config, events, out var savingValidation))
            {
                if (!savingValidation.Validation.IsValid)
                {
                    return BuildOutcome(savingValidation.Validation);
                }

                if (savingValidation.OutgoingAmount.HasValue)
                {
                    var balanceCheck = await EnsureSufficientBalanceAsync(request, config, savingValidation.OutgoingAmount.Value, ct);
                    if (!balanceCheck.IsValid)
                    {
                        return balanceCheck;
                    }
                }

                return Valid;
            }
        }

        if (string.Equals(actionType, "transaction.recorded", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(actionType, "day.friday.donation", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(actionType, "day.saturday.gold_trade", StringComparison.OrdinalIgnoreCase))
        {
            IEnumerable<EventDb> events = string.Equals(actionType, "day.saturday.gold_trade", StringComparison.OrdinalIgnoreCase)
                ? await _events.GetAllEventsBySessionAsync(request.SessionId, ct)
                : Array.Empty<EventDb>();
            if (_economyActionValidator.TryValidate(request, config, events, out var economyValidation))
            {
                if (!economyValidation.Validation.IsValid)
                {
                    return BuildOutcome(economyValidation.Validation);
                }

                if (economyValidation.OutgoingAmount.HasValue)
                {
                    var balanceCheck = await EnsureSufficientBalanceAsync(request, config, economyValidation.OutgoingAmount.Value, ct);
                    if (!balanceCheck.IsValid)
                    {
                        return balanceCheck;
                    }
                }

                return Valid;
            }
        }

        if (string.Equals(actionType, "mission.assigned", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(actionType, "tie_breaker.assigned", StringComparison.OrdinalIgnoreCase))
        {
            var events = await _events.GetAllEventsBySessionAsync(request.SessionId, ct);
            if (_assignmentValidator.TryValidate(request, events, out var assignmentValidation))
            {
                return BuildOutcome(assignmentValidation);
            }
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

            if (!_payloadReader.TryReadRiskLife(payload, out var riskId, out var direction, out var amount))
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

            if (!_payloadReader.TryReadInsuranceUsed(payload, out var riskEventIdText))
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

            var hasPurchased = events.Any(e =>
                e.PlayerId == request.PlayerId &&
                string.Equals(e.ActionType, "insurance.multirisk.purchased", StringComparison.OrdinalIgnoreCase));
            if (!hasPurchased)
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Pemain belum membeli asuransi");
            }

            var riskEvent = events.FirstOrDefault(e => e.EventId == riskEventId);
            if (riskEvent is null || !string.Equals(riskEvent.ActionType, "risk.life.drawn", StringComparison.OrdinalIgnoreCase))
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Risk event tidak ditemukan");
            }

            if (riskEvent.PlayerId != request.PlayerId)
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Risk event bukan milik pemain");
            }

            var riskPayload = _payloadReader.ReadPayload(riskEvent.Payload);
            if (!_payloadReader.TryReadRiskLife(riskPayload, out _, out var direction, out var amount) ||
                !string.Equals(direction, "OUT", StringComparison.OrdinalIgnoreCase) ||
                amount <= 0)
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Asuransi hanya berlaku untuk risiko OUT");
            }

            var alreadyUsed = events.Any(e =>
                string.Equals(e.ActionType, "insurance.multirisk.used", StringComparison.OrdinalIgnoreCase) &&
                _payloadReader.TryReadInsuranceUsed(_payloadReader.ReadPayload(e.Payload), out var usedRiskEventId) &&
                string.Equals(usedRiskEventId, riskEventIdText, StringComparison.OrdinalIgnoreCase));

            if (alreadyUsed)
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Risk event sudah ditangkal asuransi");
            }

            return Valid;
        }

        if (string.Equals(actionType, "risk.emergency.used", StringComparison.OrdinalIgnoreCase))
        {
            if (!string.Equals(config.Mode, "MAHIR", StringComparison.OrdinalIgnoreCase))
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Fitur darurat hanya tersedia di mode MAHIR");
            }

            if (request.PlayerId is null)
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Player wajib diisi",
                    new ErrorDetail("player_id", "REQUIRED"));
            }

            if (!_payloadReader.TryReadEmergencyOption(payload, out var riskEventIdText, out var optionType, out var direction, out var amount))
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

            if (string.IsNullOrWhiteSpace(optionType) ||
                !AllowedEmergencyOptions.Contains(optionType))
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

            var riskPayload = _payloadReader.ReadPayload(riskEvent.Payload);
            if (!_payloadReader.TryReadRiskLife(riskPayload, out _, out var riskDirection, out _) ||
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

            if (!_payloadReader.TryReadLoanTaken(payload, out var loanId, out var principal, out var installment, out var duration, out var penaltyPoints))
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
                _payloadReader.TryReadLoanTaken(_payloadReader.ReadPayload(e.Payload), out var existingLoanId, out _, out _, out _, out _) &&
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

            if (!_payloadReader.TryReadLoanRepay(payload, out var loanId, out var amount))
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
                _payloadReader.TryReadLoanTaken(_payloadReader.ReadPayload(e.Payload), out var existingLoanId, out _, out _, out _, out _) &&
                string.Equals(existingLoanId, loanId, StringComparison.OrdinalIgnoreCase));

            if (loan is null)
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Loan ID tidak ditemukan");
            }

            var principal = 0;
            if (_payloadReader.TryReadLoanTaken(_payloadReader.ReadPayload(loan.Payload), out _, out var principalValue, out _, out _, out _))
            {
                principal = principalValue;
            }

            var repaidSoFar = events.Where(e =>
                    e.PlayerId == request.PlayerId &&
                    e.ActionType == "loan.syariah.repaid" &&
                    _payloadReader.TryReadLoanRepay(_payloadReader.ReadPayload(e.Payload), out var existingLoanId, out _) &&
                    string.Equals(existingLoanId, loanId, StringComparison.OrdinalIgnoreCase))
                .Sum(e => _payloadReader.TryReadLoanRepay(_payloadReader.ReadPayload(e.Payload), out _, out var repaidAmount) ? repaidAmount : 0);

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

            if (!_payloadReader.TryReadInsurance(payload, out var premium))
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
    /// Memeriksa apakah saldo pemain cukup untuk pengeluaran, berdasarkan proyeksi arus kas dan starting cash.
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

        var projections = await _events.GetCashflowProjectionsAsync(request.SessionId, ct);
        var currentBalance = _playerBalanceCalc.Compute(request.PlayerId.Value, config.StartingCash, projections);
        var projectedBalance = currentBalance - outgoingAmount;

        if (projectedBalance < config.CashMin)
        {
            return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Saldo tidak mencukupi");
        }

        return Valid;
    }

    /// <summary>
    /// Memvalidasi akses sesi berdasarkan role JWT: instruktur harus pemilik sesi, player harus terdaftar di sesi.
    /// </summary>
    private async Task<SessionAccessOutcome> ValidateSessionAccessAsync(Guid sessionId, CancellationToken ct)
    {
        var role = User.FindFirstValue(ClaimTypes.Role);
        var userIdRaw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdRaw, out var userId))
        {
            return BuildAccessOutcome(StatusCodes.Status401Unauthorized, "UNAUTHORIZED", "Token user tidak valid");
        }

        if (string.Equals(role, "INSTRUCTOR", StringComparison.OrdinalIgnoreCase))
        {
            var ownedSession = await _sessions.GetSessionForInstructorAsync(sessionId, userId, ct);
            if (ownedSession is null)
            {
                return BuildAccessOutcome(StatusCodes.Status404NotFound, "NOT_FOUND", "Session tidak ditemukan");
            }

            return AccessValid;
        }

        if (string.Equals(role, "PLAYER", StringComparison.OrdinalIgnoreCase))
        {
            var linkedPlayerId = await _users.GetLinkedPlayerIdAsync(userId, ct);
            if (!linkedPlayerId.HasValue)
            {
                return BuildAccessOutcome(
                    StatusCodes.Status403Forbidden,
                    "FORBIDDEN",
                    "Akun PLAYER belum terhubung ke profil pemain");
            }

            var inSession = await _players.IsPlayerInSessionAsync(sessionId, linkedPlayerId.Value, ct);
            if (!inSession)
            {
                return BuildAccessOutcome(
                    StatusCodes.Status403Forbidden,
                    "FORBIDDEN",
                    "Player tidak terdaftar di sesi ini");
            }

            return new SessionAccessOutcome(true, StatusCodes.Status200OK, null, linkedPlayerId.Value);
        }

        return BuildAccessOutcome(StatusCodes.Status403Forbidden, "FORBIDDEN", "Role tidak diizinkan");
    }

    /// <summary>
    /// Membangun SessionAccessOutcome gagal dari kode status, kode error, dan pesan.
    /// </summary>
    private SessionAccessOutcome BuildAccessOutcome(int statusCode, string code, string message)
    {
        var error = ApiErrorHelper.BuildError(HttpContext, code, message);
        return new SessionAccessOutcome(false, statusCode, error, null);
    }

}
