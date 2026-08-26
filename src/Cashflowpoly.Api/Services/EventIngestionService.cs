// Fungsi file: Mengorkestrasi alur aplikasi dan domain melalui EventIngestionService.
using System.Collections.Frozen;
using System.Security.Cryptography;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Nodes;
using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Domain;
using Cashflowpoly.Api.Infrastructure;
using Cashflowpoly.Api.Contracts;
using Dapper;
using Microsoft.AspNetCore.Http;
using Npgsql;

namespace Cashflowpoly.Api.Services;

internal sealed class EventIngestionService : IEventIngestionService
{
    /// <summary>
    /// Hasil validasi event: valid/tidak, kode status HTTP, dan detail error jika gagal.
    /// </summary>
    private sealed record ValidationOutcome(bool IsValid, int StatusCode, ErrorResponse? Error);
    /// <summary>
    /// Hasil validasi akses sesi: mencakup status, error, dan scoped player ID untuk role PLAYER.
    /// </summary>
    private sealed record SessionAccessOutcome(bool IsValid, int StatusCode, ErrorResponse? Error, Guid? ScopedPlayerId);
    private sealed class MarketSlotRow
    {
        public string SlotGroup { get; init; } = string.Empty;
        public string SlotCode { get; init; } = string.Empty;
        public string AssetType { get; init; } = string.Empty;
    }

    private sealed record AutomaticMarketRefill(
        string SlotGroup,
        string SlotCode,
        string AssetType,
        string AssetCode);
    /// <summary>
    /// Singleton outcome validasi sukses untuk menghindari alokasi berulang.
    /// </summary>
    private static readonly ValidationOutcome Valid = new(true, StatusCodes.Status200OK, null);
    /// <summary>
    /// Singleton outcome akses sesi sukses (tanpa scope player) untuk instruktur.
    /// </summary>
    private static readonly SessionAccessOutcome AccessValid = new(true, StatusCodes.Status200OK, null, null);

    /// <summary>
    /// Opsi emergency action yang diperbolehkan oleh aturan domain.
    /// </summary>
    private static readonly FrozenSet<string> AllowedEmergencyOptions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "SELL_NEED", "SELL_GOLD", "TAKE_SHARIA_LOAN"
    }.ToFrozenSet(StringComparer.OrdinalIgnoreCase);

    private readonly SessionRepository _sessions;
    private readonly RulesetRepository _rulesets;
    private readonly EventRepository _events;
    private readonly PlayerRepository _players;
    private readonly UserRepository _users;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IEventPayloadReader _payloadReader;
    private readonly IEventRecordMapper _recordMapper;
    private readonly IEventRequestShapeValidator _shapeValidator;
    private readonly IEventCashflowProjectionBuilder _projectionBuilder;
    private readonly IEventSimpleActionValidator _simpleActionValidator;
    private readonly IEventTurnProgressValidator _turnProgressValidator;
    private readonly IEventNeedPurchaseValidator _needPurchaseValidator;
    private readonly IEventIngredientOrderValidator _ingredientOrderValidator;
    private readonly IEventSavingGoalValidator _savingGoalValidator;
    private readonly IEventEconomyActionValidator _economyActionValidator;
    private readonly IEventAssignmentValidator _assignmentValidator;
    private readonly IEventPlayerBalanceCalculator _playerBalanceCalc;
    private readonly SessionEventProjector _projector;
    private readonly ILogger<EventIngestionService> _logger;

    public EventIngestionService(
        SessionRepository sessions,
        RulesetRepository rulesets,
        EventRepository events,
        PlayerRepository players,
        UserRepository users,
        IHttpContextAccessor httpContextAccessor,
        IEventPayloadReader payloadReader,
        IEventRecordMapper recordMapper,
        IEventRequestShapeValidator shapeValidator,
        IEventCashflowProjectionBuilder projectionBuilder,
        IEventSimpleActionValidator simpleActionValidator,
        IEventTurnProgressValidator turnProgressValidator,
        IEventNeedPurchaseValidator needPurchaseValidator,
        IEventIngredientOrderValidator ingredientOrderValidator,
        IEventSavingGoalValidator savingGoalValidator,
        IEventEconomyActionValidator economyActionValidator,
        IEventAssignmentValidator assignmentValidator,
        IEventPlayerBalanceCalculator playerBalanceCalc,
        SessionEventProjector projector,
        ILogger<EventIngestionService> logger)
    {
        _sessions = sessions;
        _rulesets = rulesets;
        _events = events;
        _players = players;
        _users = users;
        _httpContextAccessor = httpContextAccessor;
        _payloadReader = payloadReader;
        _recordMapper = recordMapper;
        _shapeValidator = shapeValidator;
        _projectionBuilder = projectionBuilder;
        _simpleActionValidator = simpleActionValidator;
        _turnProgressValidator = turnProgressValidator;
        _needPurchaseValidator = needPurchaseValidator;
        _ingredientOrderValidator = ingredientOrderValidator;
        _savingGoalValidator = savingGoalValidator;
        _economyActionValidator = economyActionValidator;
        _assignmentValidator = assignmentValidator;
        _playerBalanceCalc = playerBalanceCalc;
        _projector = projector;
        _logger = logger;
    }

    /// <summary>
    /// Menerima satu event gameplay, memvalidasi aturan domain, menyimpan ke database, dan mencatat log validasi.
    /// </summary>
    public async Task<(EventStoredResponse? Result, int StatusCode, ErrorResponse? Error)> IngestEventAsync(
        EventRequest request, ClaimsPrincipal user, CancellationToken ct)
    {
        var enrichedRequest = await EnrichEventRequestAsync(request, ct);
        var validation = await ValidateEventAsync(enrichedRequest, user, ct);
        if (!validation.IsValid)
        {
            await _events.InsertValidationLogAsync(
                enrichedRequest.SessionId,
                enrichedRequest.EventId,
                enrichedRequest.RulesetVersionId,
                validation.Error?.ErrorCode,
                validation.Error?.Message,
                validation.StatusCode,
                validation.Error?.TraceId ?? "unknown",
                ct);

            return (null, validation.StatusCode, validation.Error);
        }

        try
        {
            await StoreEventAsync(enrichedRequest, ct);
            return (new EventStoredResponse(true, enrichedRequest.EventId), StatusCodes.Status201Created, null);
        }
        catch (PostgresException ex) when (ex.SqlState == "23505")
        {
            var error = BuildError("DUPLICATE", "Event sudah ada");
            await _events.InsertValidationLogAsync(
                enrichedRequest.SessionId,
                enrichedRequest.EventId,
                enrichedRequest.RulesetVersionId,
                error.ErrorCode,
                error.Message,
                StatusCodes.Status409Conflict,
                error.TraceId,
                ct);
            return (null, StatusCodes.Status409Conflict, error);
        }
        catch (PostgresException ex) when (ex.SqlState == "23514")
        {
            _logger.LogWarning(ex, "Database rejected gameplay event {EventId} for session {SessionId}", enrichedRequest.EventId, enrichedRequest.SessionId);
            var error = BuildError("DOMAIN_RULE_VIOLATION", "Aktivitas ditolak karena melanggar aturan permainan");
            await _events.InsertValidationLogAsync(
                enrichedRequest.SessionId,
                enrichedRequest.EventId,
                enrichedRequest.RulesetVersionId,
                error.ErrorCode,
                error.Message,
                StatusCodes.Status422UnprocessableEntity,
                error.TraceId,
                ct);
            return (null, StatusCodes.Status422UnprocessableEntity, error);
        }
    }

    /// <summary>
    /// Menerima batch event gameplay, memvalidasi masing-masing, dan mengembalikan ringkasan sukses/gagal.
    /// </summary>
    public async Task<(EventBatchResponse? Result, int StatusCode, ErrorResponse? Error)> IngestBatchAsync(
        EventBatchRequest request, ClaimsPrincipal user, CancellationToken ct)
    {
        const int MaxBatchSize = 500;

        if (request.Events is null || request.Events.Count == 0)
        {
            return (null, StatusCodes.Status400BadRequest, BuildError("VALIDATION_ERROR", "Daftar event batch wajib diisi",
                new ErrorDetail("events", "REQUIRED")));
        }

        if (request.Events.Count > MaxBatchSize)
        {
            return (null, StatusCodes.Status400BadRequest, BuildError("VALIDATION_ERROR",
                "Batch maksimal 500 event",
                new ErrorDetail("events", "MAX_LENGTH")));
        }

        var failed = new List<EventBatchFailed>();
        var storedCount = 0;

        foreach (var evt in request.Events)
        {
            var enrichedRequest = await EnrichEventRequestAsync(evt, ct);
            var validation = await ValidateEventAsync(enrichedRequest, user, ct);
            if (!validation.IsValid)
            {
                failed.Add(new EventBatchFailed(enrichedRequest.EventId, validation.Error?.ErrorCode ?? "VALIDATION_ERROR"));
                await _events.InsertValidationLogAsync(
                    enrichedRequest.SessionId,
                    enrichedRequest.EventId,
                    enrichedRequest.RulesetVersionId,
                    validation.Error?.ErrorCode,
                    validation.Error?.Message,
                    validation.StatusCode,
                    validation.Error?.TraceId ?? "unknown",
                    ct);
                continue;
            }

            try
            {
                await StoreEventAsync(enrichedRequest, ct);
                storedCount++;
            }
            catch (PostgresException ex) when (ex.SqlState == "23505")
            {
                failed.Add(new EventBatchFailed(enrichedRequest.EventId, "DUPLICATE"));
                var dupError = BuildError("DUPLICATE", "Event sudah ada");
                await _events.InsertValidationLogAsync(
                    enrichedRequest.SessionId,
                    enrichedRequest.EventId,
                    enrichedRequest.RulesetVersionId,
                    dupError.ErrorCode,
                    dupError.Message,
                    StatusCodes.Status409Conflict,
                    dupError.TraceId,
                    ct);
            }
            catch (PostgresException ex) when (ex.SqlState == "23514")
            {
                _logger.LogWarning(ex, "Database rejected gameplay event {EventId} from batch for session {SessionId}", enrichedRequest.EventId, enrichedRequest.SessionId);
                failed.Add(new EventBatchFailed(enrichedRequest.EventId, "DOMAIN_RULE_VIOLATION"));
                var domainError = BuildError("DOMAIN_RULE_VIOLATION", "Aktivitas ditolak karena melanggar aturan permainan");
                await _events.InsertValidationLogAsync(
                    enrichedRequest.SessionId,
                    enrichedRequest.EventId,
                    enrichedRequest.RulesetVersionId,
                    domainError.ErrorCode,
                    domainError.Message,
                    StatusCodes.Status422UnprocessableEntity,
                    domainError.TraceId,
                    ct);
            }
        }

        return (new EventBatchResponse(storedCount, failed), StatusCodes.Status200OK, null);
    }

    /// <summary>
    /// Mengambil daftar event sesi dengan pagination berbasis sequence number.
    /// </summary>
    public async Task<(EventsBySessionResponse? Result, int StatusCode, ErrorResponse? Error)> GetEventsBySessionAsync(
        Guid sessionId, ClaimsPrincipal user, string? cursor, int limit, CancellationToken ct)
    {
        var accessScopeCheck = await ValidateSessionAccessAsync(sessionId, user, ct);
        if (!accessScopeCheck.IsValid)
        {
            return (null, accessScopeCheck.StatusCode, accessScopeCheck.Error);
        }

        var session = await _sessions.GetSessionAsync(sessionId, ct);
        if (session is null)
        {
            return (null, StatusCodes.Status404NotFound, BuildError("NOT_FOUND", "Session tidak ditemukan"));
        }

        if (!OpaqueCursor.TryDecodeEvent(cursor, out var afterSequence))
        {
            return (null, StatusCodes.Status400BadRequest, BuildError("VALIDATION_ERROR", "Cursor event tidak valid",
                new ErrorDetail("cursor", "INVALID_FORMAT")));
        }

        if (limit is < 1 or > 100)
        {
            return (null, StatusCodes.Status400BadRequest, BuildError("VALIDATION_ERROR", "limit harus antara 1 sampai 100",
                new ErrorDetail("limit", "OUT_OF_RANGE")));
        }

        var events = await _events.GetEventsBySessionAsync(sessionId, afterSequence, limit + 1, ct);
        var hasMore = events.Count > limit;
        if (hasMore)
        {
            events.RemoveAt(events.Count - 1);
        }
        var allEvents = await _events.GetAllEventsBySessionAsync(sessionId, ct);
        var participantCount = await _players.CountPlayersInSessionAsync(sessionId, ct);
        var sealedDonationDays = allEvents
            .Where(item => IsEventAction(item, GameActionCatalog.JumatBerkah) && item.UserId.HasValue)
            .GroupBy(item => item.DayIndex)
            .Where(group => group.Select(item => item.UserId!.Value).Distinct().Count() < participantCount)
            .Select(group => group.Key)
            .ToHashSet();
        var responseEvents = events.Select(item =>
        {
            var mapped = _recordMapper.ToEventRequest(item);
            if (IsEventAction(item, GameActionCatalog.JumatBerkah) && sealedDonationDays.Contains(item.DayIndex))
            {
                return mapped with { Payload = JsonSerializer.SerializeToElement(new { status = "SEALED" }) };
            }

            if (accessScopeCheck.ScopedPlayerId.HasValue &&
                !string.Equals(session.Status, "ENDED", StringComparison.OrdinalIgnoreCase) &&
                IsEventAction(item, GameActionCatalog.SetupMisiAwal) &&
                item.UserId != accessScopeCheck.ScopedPlayerId.Value)
            {
                return mapped with { Payload = JsonSerializer.SerializeToElement(new { status = "HIDDEN" }) };
            }

            return mapped;
        }).ToList();
        var nextCursor = events.Count > 0
            ? OpaqueCursor.EncodeEvent(events[^1].SequenceNumber)
            : null;
        return (new EventsBySessionResponse(sessionId, responseEvents, nextCursor, hasMore), StatusCodes.Status200OK, null);
    }

    /// <summary>
    /// Menjalankan seluruh pipeline validasi event: akses sesi, keberadaan entitas, enum, domain rules, dan duplikasi.
    /// </summary>
    private async Task<ValidationOutcome> ValidateEventAsync(EventRequest request, ClaimsPrincipal user, CancellationToken ct)
    {
        var accessScopeCheck = await ValidateSessionAccessAsync(request.SessionId, user, ct);
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

        if (request.UserId is not null)
        {
            var player = await _players.GetPlayerAsync(request.UserId.Value, ct);
            if (player is null)
            {
                return BuildOutcome(StatusCodes.Status404NotFound, "NOT_FOUND", "Player tidak ditemukan");
            }

            var inSession = await _players.IsPlayerInSessionAsync(request.SessionId, request.UserId.Value, ct);
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

        if (rulesetVersion.Definition is null ||
            !RulesetRuntimeMapper.TryBuildConfig(rulesetVersion.Definition, out var config, out _))
        {
            return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Definition ruleset tidak valid");
        }

        var canonicalAction = GameActionCatalog.ResolveGameActionId(request.ActionType, request.Payload);
        if (canonicalAction is null ||
            (config!.Actions.Count > 0 && config.Actions.All(action =>
                !string.Equals(action.ActionId, canonicalAction, StringComparison.OrdinalIgnoreCase))))
        {
            return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION",
                "Aksi tidak tersedia pada ruleset aktif",
                new ErrorDetail("action_type", "NOT_ACTIVE"));
        }

        var dayValidation = await ValidateActiveDayAsync(request, ct);
        if (!dayValidation.IsValid)
        {
            return dayValidation;
        }

        var actionOrderValidation = await ValidateDailyActionOrderAsync(request, config!, ct);
        if (!actionOrderValidation.IsValid)
        {
            return actionOrderValidation;
        }

        var domainValidation = await ValidateDomainRulesAsync(request, config!, ct);
        if (!domainValidation.IsValid)
        {
            return domainValidation;
        }

        if (IsAction(request, GameActionCatalog.AkhirGiliran) &&
            await _events.HasPendingLifeRiskAsync(request.SessionId, ct))
        {
            return BuildOutcome(
                StatusCodes.Status422UnprocessableEntity,
                "DOMAIN_RULE_VIOLATION",
                "Seluruh risiko pengeluaran harus diselesaikan sebelum giliran berakhir");
        }

        return Valid;
    }

    private async Task<ValidationOutcome> ValidateActiveDayAsync(
        EventRequest request,
        CancellationToken ct)
    {
        var progress = await _sessions.GetProgressAsync(request.SessionId, ct);
        if (progress is null)
        {
            return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "State sesi tidak ditemukan");
        }

        if (request.DayIndex != progress.Day)
        {
            return BuildOutcome(
                StatusCodes.Status422UnprocessableEntity,
                "DOMAIN_RULE_VIOLATION",
                $"Event harus dicatat pada hari aktif {progress.Day}",
                new ErrorDetail("day_index", "MISMATCH"));
        }

        var expectedWeekday = ResolveWeekday(progress.Day);
        if (!string.Equals(request.Weekday, expectedWeekday, StringComparison.OrdinalIgnoreCase))
        {
            return BuildOutcome(
                StatusCodes.Status422UnprocessableEntity,
                "DOMAIN_RULE_VIOLATION",
                $"Hari {progress.Day} harus memakai weekday {expectedWeekday}",
                new ErrorDetail("weekday", "MISMATCH"));
        }

        if (IsAction(request, GameActionCatalog.AkhirGiliran) && progress.Day >= progress.FinishDay)
        {
            return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION",
                "Hari terakhir harus ditutup dengan AkhiriSesi, bukan AkhirGiliran");
        }

        if (IsAction(request, GameActionCatalog.SessionEnded) && progress.Day != progress.FinishDay)
        {
            return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION",
                $"Sesi hanya dapat diakhiri pada hari {progress.FinishDay}");
        }

        return Valid;
    }

    private static string ResolveWeekday(int day)
        => (((day - 1) % 7 + 7) % 7) switch
        {
            0 => "MON",
            1 => "TUE",
            2 => "WED",
            3 => "THU",
            4 => "FRI",
            5 => "SAT",
            _ => "SUN"
        };

    private async Task<ValidationOutcome> ValidateDailyActionOrderAsync(EventRequest request, RulesetConfig config, CancellationToken ct)
    {
        if (request.ActorType.Equals("SYSTEM", StringComparison.OrdinalIgnoreCase))
        {
            return Valid;
        }

        if (request.UserId is null)
        {
            return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Player wajib diisi",
                new ErrorDetail("user_id", "REQUIRED"));
        }

        var participantId = await _players.GetSessionParticipantIdAsync(request.SessionId, request.UserId.Value, ct);
        if (participantId is null)
        {
            return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Player belum terdaftar pada sesi");
        }

        var playerOrders = await _players.GetSessionParticipantPlayerOrderMapAsync(request.SessionId, ct);
        if (!playerOrders.TryGetValue(participantId.Value, out var currentPlayerOrder))
        {
            return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Urutan pemain tidak ditemukan");
        }

        if (request.TurnNumber != currentPlayerOrder)
        {
            return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION",
                "Turn number harus sesuai urutan pemain pada sesi",
                new ErrorDetail("turn_number", "MISMATCH"));
        }

        var slotPolicy = GameActionCatalog.GetPlayerActionSlotPolicy(request.ActionType, request.Payload);
        var scheduledFreeAction = ResolveScheduledFreeAction(request);
        if (slotPolicy != PlayerActionSlotPolicy.Consumes && scheduledFreeAction is null)
        {
            return Valid;
        }


        var events = await _events.GetAllEventsBySessionAsync(request.SessionId, ct);
        if (scheduledFreeAction is not null)
        {
            var scheduledEvents = events
                .Where(e => e.DayIndex == request.DayIndex &&
                            e.SessionPlayerId.HasValue &&
                            string.Equals(ResolveScheduledFreeAction(e), scheduledFreeAction, StringComparison.Ordinal))
                .ToList();
            if (scheduledEvents.Any(e => e.SessionPlayerId == participantId.Value))
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION",
                    "Pilihan khusus hari ini sudah dicatat untuk pemain");
            }

            foreach (var priorPlayer in playerOrders.Where(item => item.Value < currentPlayerOrder).OrderBy(item => item.Value))
            {
                if (scheduledEvents.All(e => e.SessionPlayerId != priorPlayer.Key))
                {
                    return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION",
                        "Pemain sebelumnya belum menyelesaikan pilihan khusus hari ini");
                }
            }

            return Valid;
        }

        if (request.ActionSlot < 1 || request.ActionSlot > config.ActionsPerTurn)
        {
            return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION",
                "Aksi pemain di luar slot aksi harian",
                new ErrorDetail("action_slot", "OUT_OF_RANGE"));
        }

        var dayEvents = events
            .Where(e => e.DayIndex == request.DayIndex &&
                        e.SessionPlayerId.HasValue &&
                        GameActionCatalog.GetPlayerActionSlotPolicy(
                            e.ActionType,
                            _payloadReader.ReadPayload(string.IsNullOrWhiteSpace(e.Payload) ? "{}" : e.Payload)) == PlayerActionSlotPolicy.Consumes)
            .ToList();

        var currentPlayerUsedSlots = dayEvents
            .Where(e => e.SessionPlayerId == participantId.Value)
            .Select(e => e.ActionSlot)
            .ToHashSet();
        if (currentPlayerUsedSlots.Contains(request.ActionSlot))
        {
            return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION",
                "Slot aksi pemain sudah dipakai pada hari ini",
                new ErrorDetail("action_slot", "DUPLICATE"));
        }

        foreach (var priorPlayer in playerOrders.Where(item => item.Value < currentPlayerOrder).OrderBy(item => item.Value))
        {
            var usedSlots = dayEvents
                .Where(e => e.SessionPlayerId == priorPlayer.Key)
                .Select(e => e.ActionSlot)
                .Distinct()
                .Count();
            if (usedSlots < config.ActionsPerTurn)
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION",
                    "Pemain sebelumnya belum menyelesaikan jatah aksi hari ini");
            }
        }

        var expectedSlot = currentPlayerUsedSlots.Count + 1;
        if (request.ActionSlot != expectedSlot)
        {
            return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION",
                "Slot aksi harus mengikuti urutan aksi pemain pada hari yang sama",
                new ErrorDetail("action_slot", "OUT_OF_SEQUENCE"));
        }

        return Valid;
    }

    private static string? ResolveScheduledFreeAction(EventRequest request)
        => ResolveScheduledFreeAction(request.Weekday, request.ActionType, request.Payload);

    private string? ResolveScheduledFreeAction(EventDb record)
        => ResolveScheduledFreeAction(
            record.Weekday,
            record.ActionType,
            _payloadReader.ReadPayload(string.IsNullOrWhiteSpace(record.Payload) ? "{}" : record.Payload));

    private static string? ResolveScheduledFreeAction(string weekday, string actionType, JsonElement payload)
    {
        if (weekday.Equals("FRI", StringComparison.OrdinalIgnoreCase) &&
            GameActionCatalog.Is(actionType, payload, GameActionCatalog.JumatBerkah))
        {
            return "FRIDAY_DONATION";
        }

        var hasRiskReference = payload.TryGetProperty("risk_event_id", out var riskEventId) &&
                               riskEventId.ValueKind == JsonValueKind.String &&
                               Guid.TryParse(riskEventId.GetString(), out _);
        var isGoldDecision = GameActionCatalog.Is(actionType, payload, GameActionCatalog.InvestasiEmas) ||
                             GameActionCatalog.Is(actionType, payload, GameActionCatalog.JualEmas) ||
                             GameActionCatalog.Is(actionType, payload, GameActionCatalog.GoldSkipped);
        return weekday.Equals("SAT", StringComparison.OrdinalIgnoreCase) && isGoldDecision && !hasRiskReference
            ? "SATURDAY_GOLD"
            : null;
    }

    /// <summary>
    /// Menyimpan event ke tabel events, membangun proyeksi arus kas, dan menangani offset asuransi jika berlaku.
    /// </summary>
    private async Task<Guid> StoreEventAsync(EventRequest request, CancellationToken ct)
    {
        var eventPk = Guid.NewGuid();
        var timestamp = request.Timestamp.ToUniversalTime();
        var config = await GetRulesetConfigAsync(request.RulesetVersionId, ct);
        var record = new EventDb
        {
            EventPk = eventPk,
            EventId = request.EventId,
            SessionId = request.SessionId,
            UserId = request.UserId,
            ActorType = request.ActorType.ToUpperInvariant(),
            Timestamp = timestamp,
            DayIndex = request.DayIndex,
            Weekday = request.Weekday.ToUpperInvariant(),
            TurnNumber = request.TurnNumber,
            ActionSlot = request.ActionSlot,
            SequenceNumber = request.SequenceNumber,
            ActionType = request.ActionType,
            RulesetVersionId = request.RulesetVersionId,
            Payload = request.Payload.GetRawText(),
            ReceivedAt = DateTimeOffset.UtcNow,
            ClientRequestId = request.ClientRequestId
        };

        // Simpan event + seluruh proyeksi state dalam satu transaksi.
        await using var conn = await _events.OpenConnectionAsync(ct);
        await using var tx = await conn.BeginTransactionAsync(ct);

        var projections = new List<CashflowProjectionDb>();
        var isLifeRisk = string.Equals(request.ActionType, GameActionCatalog.RisikoKehidupan, StringComparison.OrdinalIgnoreCase);
        RulesetLifeRiskDto? activeRisk = null;
        var deferRiskCashflow = false;
        if (isLifeRisk && config is not null && TryResolveLifeRisk(config, request.Payload, out activeRisk))
        {
            if (request.UserId.HasValue &&
                string.Equals(activeRisk.EffectType, "COIN_EFFECT", StringComparison.OrdinalIgnoreCase) &&
                string.Equals(activeRisk.Direction, "OUT", StringComparison.OrdinalIgnoreCase))
            {
                deferRiskCashflow = true;
            }

            if (string.Equals(activeRisk.EffectType, "ALL_PLAYERS_COIN_EFFECT", StringComparison.OrdinalIgnoreCase))
            {
                var participantUserIds = new List<Guid>();
                const string sql = "select user_id from session_participants where session_id = @sessionId";
                await using (var cmd = new NpgsqlCommand(sql, conn, tx))
                {
                    cmd.Parameters.AddWithValue("sessionId", request.SessionId);
                    await using (var reader = await cmd.ExecuteReaderAsync(ct))
                    {
                        while (await reader.ReadAsync(ct))
                        {
                            participantUserIds.Add(reader.GetGuid(0));
                        }
                    }
                }

                foreach (var pUserId in participantUserIds)
                {
                    projections.Add(new CashflowProjectionDb
                    {
                        ProjectionId = Guid.NewGuid(),
                        SessionId = request.SessionId,
                        UserId = pUserId,
                        EventPk = eventPk,
                        EventId = request.EventId,
                        Timestamp = timestamp,
                        Direction = "OUT",
                        Amount = activeRisk.Amount,
                        Category = "RISK_LIFE",
                        Reference = activeRisk.RiskCode,
                        Note = null
                    });
                }
            }
            else if (string.Equals(activeRisk.EffectType, "PLAYER_TO_PLAYER_TRANSFER", StringComparison.OrdinalIgnoreCase) && request.UserId.HasValue)
            {
                var participantUserIds = new List<Guid>();
                const string sql = "select user_id from session_participants where session_id = @sessionId";
                await using (var cmd = new NpgsqlCommand(sql, conn, tx))
                {
                    cmd.Parameters.AddWithValue("sessionId", request.SessionId);
                    await using (var reader = await cmd.ExecuteReaderAsync(ct))
                    {
                        while (await reader.ReadAsync(ct))
                        {
                            participantUserIds.Add(reader.GetGuid(0));
                        }
                    }
                }

                var otherPlayersCount = 0;
                foreach (var pUserId in participantUserIds)
                {
                    if (pUserId != request.UserId.Value)
                    {
                        otherPlayersCount++;
                        projections.Add(new CashflowProjectionDb
                        {
                            ProjectionId = Guid.NewGuid(),
                            SessionId = request.SessionId,
                            UserId = pUserId,
                            EventPk = eventPk,
                            EventId = request.EventId,
                            Timestamp = timestamp,
                            Direction = "OUT",
                            Amount = 1,
                            Category = "RISK_LIFE",
                            Reference = activeRisk.RiskCode,
                            Note = null
                        });
                    }
                }

                if (otherPlayersCount > 0)
                {
                    projections.Add(new CashflowProjectionDb
                    {
                        ProjectionId = Guid.NewGuid(),
                        SessionId = request.SessionId,
                        UserId = request.UserId.Value,
                        EventPk = eventPk,
                        EventId = request.EventId,
                        Timestamp = timestamp,
                        Direction = "IN",
                        Amount = otherPlayersCount,
                        Category = "RISK_LIFE",
                        Reference = activeRisk.RiskCode,
                        Note = null
                    });
                }
            }
        }

        if (projections.Count == 0 && !deferRiskCashflow)
        {
            if (!TryBuildCatalogRiskProjection(request, timestamp, eventPk, config, out var projection))
            {
                _projectionBuilder.TryBuild(request, timestamp, eventPk, out projection);
            }

            if (projection is not null)
            {
                projections.Add(projection);
            }
        }

        if (config is not null &&
            IsInsuranceResolution(request) &&
            TryGetRiskEventReference(request.Payload, out var insuranceRiskEventId))
        {
            var insuranceRiskEvent = await _events.GetEventByIdAsync(request.SessionId, insuranceRiskEventId, ct);
            if (!TryBuildCatalogInsuranceOffset(
                    request, timestamp, eventPk, insuranceRiskEvent, config, out var insuranceOffset) ||
                insuranceOffset is null)
            {
                throw new InvalidOperationException("Offset asuransi tidak dapat dibangun dari risiko yang dirujuk.");
            }

            projections.Add(insuranceOffset);
        }

        if (config is not null &&
            TryGetRiskEventReference(request.Payload, out var linkedRiskEventId) &&
            !await _events.IsRiskResolvedAsync(request.SessionId, linkedRiskEventId, ct))
        {
            var linkedRiskEvent = await _events.GetEventByIdAsync(request.SessionId, linkedRiskEventId, ct);
            if (linkedRiskEvent is not null &&
                linkedRiskEvent.UserId == request.UserId &&
                TryResolveLifeRisk(config, _payloadReader.ReadPayload(linkedRiskEvent.Payload), out var linkedRisk) &&
                string.Equals(linkedRisk.Direction, "OUT", StringComparison.OrdinalIgnoreCase))
            {
                var incoming = projections
                    .Where(item => item.UserId == request.UserId && string.Equals(item.Direction, "IN", StringComparison.OrdinalIgnoreCase))
                    .Sum(item => item.Amount);
                var resolvesWithInsurance = IsInsuranceResolution(request);
                var currentBalance = await GetCurrentCashBalanceAsync(request, config, ct);
                if (resolvesWithInsurance || currentBalance + incoming >= linkedRisk.Amount)
                {
                    projections.Add(new CashflowProjectionDb
                    {
                        ProjectionId = Guid.NewGuid(),
                        SessionId = request.SessionId,
                        UserId = request.UserId!.Value,
                        EventPk = eventPk,
                        EventId = request.EventId,
                        Timestamp = timestamp,
                        Direction = "OUT",
                        Amount = linkedRisk.Amount,
                        Category = "RISK_LIFE",
                        Reference = linkedRiskEventId.ToString(),
                        Note = "Penyelesaian risiko"
                    });
                }
            }
        }

        if (request.UserId.HasValue)
        {
            record.SessionPlayerId = await _events.ResolveSessionParticipantIdAsync(
                request.SessionId,
                request.UserId.Value,
                conn,
                tx,
                ct);
        }

        await _events.InsertEventAsync(record, conn, tx, ct);
        await _events.InsertEventAssetReferencesAsync(
            record,
            BuildAssetReferences(request),
            conn,
            tx,
            ct);

        for (var i = 0; i < projections.Count; i++)
        {
            var cashflowProjection = projections[i];
            cashflowProjection.ProjectionOrder = i + 1;
            await _events.InsertCashflowProjectionAsync(cashflowProjection, conn, tx, ct);
        }

        await _projector.ProjectAsync(request, record, projections, conn, tx, ct);

        await tx.CommitAsync(ct);

        return eventPk;
    }

    private static bool ShouldRefillMarket(EventRequest request, RulesetConfig config)
    {
        return string.Equals(request.ActorType, "PLAYER", StringComparison.OrdinalIgnoreCase) &&
               request.ActionSlot == config.ActionsPerTurn &&
               GameActionCatalog.GetPlayerActionSlotPolicy(request.ActionType, request.Payload) == PlayerActionSlotPolicy.Consumes;
    }

    private async Task ApplyAutomaticMarketRefillsAsync(
        EventRequest sourceRequest,
        EventDb sourceRecord,
        NpgsqlConnection conn,
        NpgsqlTransaction tx,
        CancellationToken ct)
    {
        var refills = new List<AutomaticMarketRefill>();
        while (refills.Count < 15)
        {
            var emptySlots = (await conn.QueryAsync<MarketSlotRow>(new CommandDefinition(
                """
                with slots(slot_group, slot_code, asset_type, slot_order) as (
                    values
                        ('INGREDIENT_MARKET', 'SLOT_1', 'INGREDIENT', 1),
                        ('INGREDIENT_MARKET', 'SLOT_2', 'INGREDIENT', 2),
                        ('INGREDIENT_MARKET', 'SLOT_3', 'INGREDIENT', 3),
                        ('INGREDIENT_MARKET', 'SLOT_4', 'INGREDIENT', 4),
                        ('INGREDIENT_MARKET', 'SLOT_5', 'INGREDIENT', 5),
                        ('ORDER_MARKET', 'SLOT_1', 'ORDER', 6),
                        ('ORDER_MARKET', 'SLOT_2', 'ORDER', 7),
                        ('ORDER_MARKET', 'SLOT_3', 'ORDER', 8),
                        ('ORDER_MARKET', 'SLOT_4', 'ORDER', 9),
                        ('ORDER_MARKET', 'SLOT_5', 'ORDER', 10),
                        ('NEED_MARKET', 'SLOT_1', 'NEED', 11),
                        ('NEED_MARKET', 'SLOT_2', 'NEED', 12),
                        ('NEED_MARKET', 'SLOT_3', 'NEED', 13),
                        ('NEED_MARKET', 'SLOT_4', 'NEED', 14),
                        ('NEED_MARKET', 'SLOT_5', 'NEED', 15)
                )
                select
                    slot.slot_group as "SlotGroup",
                    slot.slot_code as "SlotCode",
                    slot.asset_type as "AssetType"
                from slots slot
                where not exists (
                    select 1
                    from session_card_positions position
                    where position.session_id = @sessionId
                      and position.zone = 'MARKET'
                      and position.status = 'ACTIVE'
                      and position.slot_group = slot.slot_group
                      and position.slot_code = slot.slot_code
                )
                order by slot.slot_order
                """,
                new { sessionId = sourceRequest.SessionId },
                tx,
                cancellationToken: ct))).ToList();

            if (emptySlots.Count == 0)
            {
                break;
            }

            var filledSlot = false;
            foreach (var slot in emptySlots)
            {
                var candidates = await GetMarketRefillCandidatesAsync(
                    sourceRequest.SessionId,
                    sourceRequest.RulesetVersionId,
                    slot,
                    conn,
                    tx,
                    ct);
                if (candidates.Count == 0)
                {
                    continue;
                }

                var assetCode = candidates[RandomNumberGenerator.GetInt32(candidates.Count)];
                await SessionEventProjector.ProjectMarketRefillAsync(
                    sourceRequest.SessionId,
                    sourceRequest.RulesetVersionId,
                    sourceRecord.EventId,
                    slot.SlotGroup,
                    slot.SlotCode,
                    slot.AssetType,
                    assetCode,
                    conn,
                    tx,
                    ct);
                refills.Add(new AutomaticMarketRefill(
                    slot.SlotGroup,
                    slot.SlotCode,
                    slot.AssetType,
                    assetCode));
                filledSlot = true;
                break;
            }

            if (!filledSlot)
            {
                break;
            }
        }

        if (refills.Count == 0)
        {
            return;
        }

        var payload = JsonNode.Parse(sourceRecord.Payload)?.AsObject() ?? new JsonObject();
        var refillPayload = new JsonArray();
        foreach (var refill in refills)
        {
            refillPayload.Add(new JsonObject
            {
                ["slot_group"] = refill.SlotGroup,
                ["slot_code"] = refill.SlotCode,
                ["asset_type"] = refill.AssetType,
                ["asset_code"] = refill.AssetCode
            });
        }

        payload["market_refills"] = refillPayload;
        sourceRecord.Payload = payload.ToJsonString();
        await conn.ExecuteAsync(new CommandDefinition(
            """
            update events
            set payload = @payload::jsonb
            where event_pk = @eventPk
            """,
            new { payload = sourceRecord.Payload, eventPk = sourceRecord.EventPk },
            tx,
            cancellationToken: ct));

        var references = refills.Select((refill, index) => new EventAssetReferenceInput(
            refill.AssetType,
            refill.AssetCode,
            "AUTO_REFILL",
            $"payload.market_refills[{index}].asset_code")).ToList();
        await _events.InsertEventAssetReferencesAsync(sourceRecord, references, conn, tx, ct);
    }

    private static async Task<List<string>> GetMarketRefillCandidatesAsync(
        Guid sessionId,
        Guid rulesetVersionId,
        MarketSlotRow slot,
        NpgsqlConnection conn,
        NpgsqlTransaction tx,
        CancellationToken ct)
    {
        if (string.Equals(slot.AssetType, "INGREDIENT", StringComparison.OrdinalIgnoreCase))
        {
            var ingredientCodes = await conn.QueryAsync<string>(new CommandDefinition(
                """
                select asset.asset_code
                from ruleset_game_assets asset
                where asset.ruleset_version_id = @rulesetVersionId
                  and asset.asset_type = 'INGREDIENT'
                  and asset.is_active
                  and (
                      select count(*)
                      from session_card_positions position
                      where position.session_id = @sessionId
                        and position.ruleset_game_asset_id = asset.ruleset_game_asset_id
                        and position.zone = 'MARKET'
                        and position.status = 'ACTIVE'
                        and position.slot_group = 'INGREDIENT_MARKET'
                  ) < 2
                order by asset.asset_code
                """,
                new { sessionId, rulesetVersionId },
                tx,
                cancellationToken: ct));
            return ingredientCodes.ToList();
        }

        var cardCodes = await conn.QueryAsync<string>(new CommandDefinition(
            """
            with candidates as (
                select asset.asset_code, position.zone
                from session_card_positions position
                join ruleset_game_assets asset
                  on asset.ruleset_game_asset_id = position.ruleset_game_asset_id
                 and asset.ruleset_version_id = position.ruleset_version_id
                where position.session_id = @sessionId
                  and position.status = 'ACTIVE'
                  and position.zone in ('DECK', 'DISCARD')
                  and asset.asset_type = @assetType
                  and asset.is_active
            )
            select candidate.asset_code
            from candidates candidate
            where candidate.zone = case
                when exists (select 1 from candidates deck where deck.zone = 'DECK') then 'DECK'
                else 'DISCARD'
            end
            order by candidate.asset_code
            """,
            new { sessionId, assetType = slot.AssetType },
            tx,
            cancellationToken: ct));
        return cardCodes.ToList();
    }

    private static IReadOnlyCollection<EventAssetReferenceInput> BuildAssetReferences(EventRequest request)
    {
        var references = new List<EventAssetReferenceInput>();
        var payload = request.Payload;

        static void Add(
            ICollection<EventAssetReferenceInput> target,
            JsonElement source,
            string assetType,
            string propertyName,
            string role)
        {
            if (source.TryGetProperty(propertyName, out var value) &&
                value.ValueKind == JsonValueKind.String &&
                !string.IsNullOrWhiteSpace(value.GetString()))
            {
                target.Add(new EventAssetReferenceInput(
                    assetType,
                    value.GetString()!.Trim(),
                    role,
                    $"$.{propertyName}"));
            }
        }

        var canonicalAction = GameActionCatalog.ResolveGameActionId(request.ActionType, payload) ?? request.ActionType.Trim();
        switch (canonicalAction)
        {
            case GameActionCatalog.BahanMasakan:
            case GameActionCatalog.IngredientDiscarded:
                Add(references, payload, "INGREDIENT", "card_id", "TARGET");
                Add(references, payload, "INGREDIENT", "ingredient_id", "TARGET");
                Add(references, payload, "INGREDIENT", "ingredient_name", "TARGET");
                break;
            case GameActionCatalog.JualMasakan:
                Add(references, payload, "ORDER", "order_id", "TARGET");
                Add(references, payload, "ORDER", "card_id", "TARGET");
                if (payload.TryGetProperty("required_ingredient_card_ids", out var ingredients) &&
                    ingredients.ValueKind == JsonValueKind.Array)
                {
                    var index = 0;
                    foreach (var ingredient in ingredients.EnumerateArray())
                    {
                        if (ingredient.ValueKind == JsonValueKind.String &&
                            !string.IsNullOrWhiteSpace(ingredient.GetString()))
                        {
                            references.Add(new EventAssetReferenceInput(
                                "INGREDIENT",
                                ingredient.GetString()!.Trim(),
                                "REQUIREMENT",
                                $"$.required_ingredient_card_ids[{index}]"));
                        }

                        index++;
                    }
                }
                break;
            case GameActionCatalog.Kebutuhan:
                Add(references, payload, "NEED", "card_id", "TARGET");
                Add(references, payload, "NEED", "need_id", "TARGET");
                break;
            case GameActionCatalog.SetupEmasAwal:
                Add(references, payload, "GOLD", "asset_code", "TARGET");
                if (references.Count == 0)
                {
                    references.Add(new EventAssetReferenceInput("GOLD", "gold_card", "TARGET", "$.asset_code"));
                }
                break;
            case GameActionCatalog.InvestasiEmas:
            case GameActionCatalog.JualEmas:
                Add(references, payload, "GOLD_PRICE", "price_code", "PRICE");
                break;
            case GameActionCatalog.RisikoKehidupan:
                Add(references, payload, "RISK", "risk_id", "TARGET");
                break;
            case GameActionCatalog.TieBreakerAssigned:
                Add(references, payload, "TIE_BREAKER", "card_code", "TARGET");
                Add(references, payload, "TIE_BREAKER", "tie_breaker_code", "TARGET");
                break;
            case GameActionCatalog.CardDrawn:
            case GameActionCatalog.CardDiscarded:
            case GameActionCatalog.MarketRefilled:
                if (payload.TryGetProperty("asset_type", out var assetType) &&
                    assetType.ValueKind == JsonValueKind.String &&
                    payload.TryGetProperty("asset_code", out var assetCode) &&
                    assetCode.ValueKind == JsonValueKind.String &&
                    !string.IsNullOrWhiteSpace(assetType.GetString()) &&
                    !string.IsNullOrWhiteSpace(assetCode.GetString()))
                {
                    var resolvedAssetType = assetType.GetString()!.Trim().ToUpperInvariant();
                    if (resolvedAssetType is "INGREDIENT" or "ORDER" or "NEED" or "RISK" or "GOLD" or "GOLD_PRICE" or "TIE_BREAKER")
                    {
                        references.Add(new EventAssetReferenceInput(
                            resolvedAssetType,
                            assetCode.GetString()!.Trim(),
                            "CARD",
                            "$.asset_code"));
                    }
                }
                break;
        }

        return references
            .Distinct()
            .ToArray();
    }

    private async Task<RulesetConfig?> GetRulesetConfigAsync(Guid rulesetVersionId, CancellationToken ct)
    {
        var rulesetVersion = await _rulesets.GetRulesetVersionByIdAsync(rulesetVersionId, ct);
        return rulesetVersion?.Definition is not null &&
               RulesetRuntimeMapper.TryBuildConfig(rulesetVersion.Definition, out var config, out _)
            ? config
            : null;
    }

    private async Task<EventRequest> EnrichEventRequestAsync(EventRequest request, CancellationToken ct)
    {
        if (string.Equals(request.ActionType, GameActionCatalog.JualMasakan, StringComparison.OrdinalIgnoreCase))
        {
            if (request.Payload.TryGetProperty("order_card_id", out var orderCardIdProp) &&
                orderCardIdProp.ValueKind == JsonValueKind.String)
            {
                var orderCardId = orderCardIdProp.GetString()!;
                var config = await GetRulesetConfigAsync(request.RulesetVersionId, ct);
                if (config is not null)
                {
                    var order = config.Orders.FirstOrDefault(o => string.Equals(o.Id, orderCardId, StringComparison.OrdinalIgnoreCase));
                    if (order is not null)
                    {
                        var requiredCardIds = new List<string>();
                        foreach (var bahanName in order.Bahan)
                        {
                            var matchedIng = config.Ingredients.FirstOrDefault(i => string.Equals(i.Nama, bahanName, StringComparison.OrdinalIgnoreCase));
                            var cardId = matchedIng?.Id ?? bahanName.ToLowerInvariant().Replace(" ", "_");
                            requiredCardIds.Add(cardId);
                        }

                        var node = System.Text.Json.Nodes.JsonNode.Parse(request.Payload.GetRawText());
                        if (node is System.Text.Json.Nodes.JsonObject obj)
                        {
                            obj["required_ingredient_card_ids"] = System.Text.Json.JsonSerializer.SerializeToNode(requiredCardIds);
                            obj["income"] = order.HargaJual;
                            var newPayload = JsonSerializer.Deserialize<JsonElement>(obj.ToJsonString());
                            request = request with { Payload = newPayload };
                        }
                    }
                }
            }
        }

        if (IsAction(request, GameActionCatalog.RiskEmergencyUsed))
        {
            request = await EnrichEmergencyOptionAsync(request, ct);
        }

        return request;
    }

    private async Task<EventRequest> EnrichEmergencyOptionAsync(EventRequest request, CancellationToken ct)
    {
        var node = System.Text.Json.Nodes.JsonNode.Parse(request.Payload.GetRawText()) as System.Text.Json.Nodes.JsonObject;
        if (node is null)
        {
            return request;
        }

        node.Remove("direction");
        node.Remove("amount");
        if (request.UserId is not { } userId ||
            !_payloadReader.TryGetString(request.Payload, "option_type", out var optionType))
        {
            return request with { Payload = JsonSerializer.Deserialize<JsonElement>(node.ToJsonString()) };
        }

        switch (optionType.ToUpperInvariant())
        {
            case "SELL_NEED":
                {
                    if (!_payloadReader.TryGetString(request.Payload, "need_card_id", out var cardId) &&
                        !_payloadReader.TryGetString(request.Payload, "card_id", out cardId))
                    {
                        break;
                    }

                    node["card_id"] = cardId;
                    var amount = await _events.GetOwnedNeedSaleAmountAsync(request.SessionId, userId, cardId, ct);
                    if (amount > 0)
                    {
                        node["amount"] = amount.Value;
                    }
                    break;
                }
            case "SELL_GOLD":
                {
                    if (!_payloadReader.TryGetInt32(request.Payload, "qty", out var qty) || qty <= 0 ||
                        !_payloadReader.TryGetString(request.Payload, "gold_price_event_id", out var priceEventIdText) ||
                        !Guid.TryParse(priceEventIdText, out var priceEventId))
                    {
                        break;
                    }

                    var priceEvent = await _events.GetEventByIdAsync(request.SessionId, priceEventId, ct);
                    var pricePayload = priceEvent is null ? default : _payloadReader.ReadPayload(priceEvent.Payload);
                    if (priceEvent is not null &&
                        string.Equals(priceEvent.ActionType, "BukaHargaEmas", StringComparison.OrdinalIgnoreCase) &&
                        priceEvent.DayIndex == request.DayIndex &&
                        _payloadReader.TryGetInt32(pricePayload, "gold_price", out var unitPrice) &&
                        unitPrice > 0)
                    {
                        node["unit_price"] = unitPrice;
                        node["amount"] = qty * unitPrice;
                        node["asset_code"] = "gold_card";
                    }
                    break;
                }
            case "TAKE_SHARIA_LOAN":
                {
                    if (!_payloadReader.TryGetString(request.Payload, "loan_code", out var loanCode))
                    {
                        break;
                    }

                    var config = await GetRulesetConfigAsync(request.RulesetVersionId, ct);
                    var loan = config?.ShariaLoans.FirstOrDefault(item =>
                        string.Equals(item.LoanCode, loanCode, StringComparison.OrdinalIgnoreCase));
                    if (loan is not null)
                    {
                        node["loan_code"] = loan.LoanCode;
                        node["loan_id"] = request.EventId.ToString();
                        node["principal"] = loan.Principal;
                        node["amount"] = loan.Principal;
                        node["repayment_amount"] = loan.RepaymentAmount;
                        node["duration_days"] = loan.DurationDays;
                        node["penalty_points"] = loan.PenaltyPoints;
                    }
                    break;
                }
        }

        return request with { Payload = JsonSerializer.Deserialize<JsonElement>(node.ToJsonString()) };
    }

    private bool TryBuildCatalogRiskProjection(
        EventRequest request,
        DateTimeOffset timestamp,
        Guid eventPk,
        RulesetConfig? config,
        out CashflowProjectionDb? projection)
    {
        projection = null;
        if (!IsAction(request, GameActionCatalog.RisikoKehidupan) ||
            request.UserId is null ||
            !TryResolveLifeRisk(config, request.Payload, out var risk) ||
            risk.Amount <= 0)
        {
            return false;
        }

        projection = new CashflowProjectionDb
        {
            ProjectionId = Guid.NewGuid(),
            SessionId = request.SessionId,
            UserId = request.UserId.Value,
            EventPk = eventPk,
            EventId = request.EventId,
            Timestamp = timestamp,
            Direction = risk.Direction.ToUpperInvariant(),
            Amount = risk.Amount,
            Category = "RISK_LIFE",
            Reference = risk.RiskCode,
            Note = null
        };

        return true;
    }

    private bool TryBuildCatalogInsuranceOffset(
        EventRequest request,
        DateTimeOffset timestamp,
        Guid eventPk,
        EventDb? riskEvent,
        RulesetConfig? config,
        out CashflowProjectionDb? projection)
    {
        projection = null;
        if (request.UserId is null ||
            riskEvent is null ||
            !IsEventAction(riskEvent, GameActionCatalog.RisikoKehidupan) ||
            riskEvent.UserId != request.UserId)
        {
            return false;
        }

        var riskPayload = _payloadReader.ReadPayload(riskEvent.Payload);
        if (!TryResolveLifeRisk(config, riskPayload, out var risk) ||
            !string.Equals(risk.Direction, "OUT", StringComparison.OrdinalIgnoreCase) ||
            risk.Amount <= 0)
        {
            return false;
        }

        projection = new CashflowProjectionDb
        {
            ProjectionId = Guid.NewGuid(),
            SessionId = request.SessionId,
            UserId = request.UserId.Value,
            EventPk = eventPk,
            EventId = request.EventId,
            Timestamp = timestamp,
            Direction = "IN",
            Amount = risk.Amount,
            Category = "INSURANCE_OFFSET",
            Reference = riskEvent.EventId.ToString(),
            Note = "Offset risiko oleh asuransi multirisk"
        };

        return true;
    }

    private bool TryResolveLifeRisk(
        RulesetConfig? config,
        JsonElement payload,
        out RulesetLifeRiskDto risk)
    {
        risk = default!;
        if (config is null || !_payloadReader.TryGetString(payload, "risk_id", out var riskId))
        {
            return false;
        }

        var matched = config.LifeRisks.FirstOrDefault(item =>
            string.Equals(item.RiskCode, riskId, StringComparison.OrdinalIgnoreCase));
        if (matched is null)
        {
            return false;
        }

        risk = matched;
        return true;
    }

    private static bool IsPersonalCoinOutRisk(RulesetLifeRiskDto risk) =>
        string.Equals(risk.EffectType, "COIN_EFFECT", StringComparison.OrdinalIgnoreCase) &&
        string.Equals(risk.Direction, "OUT", StringComparison.OrdinalIgnoreCase) &&
        string.Equals(risk.TargetScope, "SELF", StringComparison.OrdinalIgnoreCase) &&
        risk.Amount > 0;

    private async Task<double> GetCurrentCashBalanceAsync(
        EventRequest request,
        RulesetConfig config,
        CancellationToken ct)
    {
        if (request.UserId is null)
        {
            return config.StartingCash;
        }

        var projections = await _events.GetCashflowProjectionsAsync(request.SessionId, ct);
        return _playerBalanceCalc.Compute(request.UserId.Value, config.StartingCash, projections);
    }

    private static bool TryGetRiskEventReference(JsonElement payload, out Guid riskEventId)
    {
        riskEventId = Guid.Empty;
        return payload.ValueKind == JsonValueKind.Object &&
               payload.TryGetProperty("risk_event_id", out var id) &&
               id.ValueKind == JsonValueKind.String &&
               Guid.TryParse(id.GetString(), out riskEventId);
    }

    private bool IsInsuranceResolution(EventRequest request)
    {
        return IsAction(request, GameActionCatalog.Asuransi) &&
               _payloadReader.TryReadInsuranceUsed(request.Payload, out _);
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
            var participantCount = await _players.CountPlayersInSessionAsync(request.SessionId, ct);
            if (_turnProgressValidator.TryValidate(request, config, events, participantCount, out var turnValidation))
            {
                return BuildOutcome(turnValidation);
            }
        }

        if (IsAction(request, GameActionCatalog.Kebutuhan))
        {
            var events = await _events.GetAllEventsBySessionAsync(request.SessionId, ct);
            if (_needPurchaseValidator.TryValidate(request, config, events, out var needValidation))
            {
                if (!needValidation.Validation.IsValid)
                {
                    return BuildOutcome(needValidation.Validation);
                }

                if (needValidation.OutgoingAmount.HasValue && request.UserId is not null)
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

        if (IsAction(request, GameActionCatalog.BahanMasakan) ||
            IsAction(request, GameActionCatalog.IngredientDiscarded) ||
            IsAction(request, GameActionCatalog.JualMasakan))
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

        if (IsAction(request, GameActionCatalog.Menabung) ||
            IsAction(request, GameActionCatalog.SavingDepositWithdrawn) ||
            IsAction(request, GameActionCatalog.TujuanFinansial))
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

        if (IsAction(request, GameActionCatalog.TransactionRecorded) ||
            IsAction(request, GameActionCatalog.JumatBerkah) ||
            IsAction(request, GameActionCatalog.GoldPriceOpened) ||
            IsAction(request, GameActionCatalog.InvestasiEmas) ||
            IsAction(request, GameActionCatalog.JualEmas))
        {
            IEnumerable<EventDb> events = IsAction(request, GameActionCatalog.JumatBerkah) ||
                                          IsAction(request, GameActionCatalog.GoldPriceOpened) ||
                                          IsAction(request, GameActionCatalog.InvestasiEmas) ||
                                          IsAction(request, GameActionCatalog.JualEmas)
                ? await _events.GetAllEventsBySessionAsync(request.SessionId, ct)
                : Array.Empty<EventDb>();
            if (_economyActionValidator.TryValidate(request, config, events, out var economyValidation))
            {
                if (!economyValidation.Validation.IsValid)
                {
                    return BuildOutcome(economyValidation.Validation);
                }

                if (IsAction(request, GameActionCatalog.JualEmas) &&
                    request.UserId.HasValue &&
                    _payloadReader.TryGetInt32(request.Payload, "qty", out var sellQty) &&
                    await _events.GetGoldQuantityAsync(request.SessionId, request.UserId.Value, ct) < sellQty)
                {
                    return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION",
                        "Kepemilikan emas tidak mencukupi");
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

        if (IsAction(request, GameActionCatalog.SetupMisiAwal) ||
            IsAction(request, GameActionCatalog.TieBreakerAssigned))
        {
            var events = await _events.GetAllEventsBySessionAsync(request.SessionId, ct);
            var participantCount = await _players.CountPlayersInSessionAsync(request.SessionId, ct);
            if (_assignmentValidator.TryValidate(request, events, participantCount, out var assignmentValidation))
            {
                return BuildOutcome(assignmentValidation);
            }
        }

        if (IsAction(request, GameActionCatalog.RisikoKehidupan))
        {
            if (!string.Equals(config.Mode, "MAHIR", StringComparison.OrdinalIgnoreCase))
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Fitur risiko hanya tersedia di mode MAHIR");
            }

            if (request.UserId is null)
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Player wajib diisi",
                    new ErrorDetail("user_id", "REQUIRED"));
            }

            if (!_payloadReader.TryGetString(payload, "risk_id", out var riskId) ||
                string.IsNullOrWhiteSpace(riskId))
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Payload risiko tidak valid",
                    new ErrorDetail("payload.risk_id", "REQUIRED"));
            }

            if (!TryResolveLifeRisk(config, payload, out var risk))
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION",
                    "Risk ID tidak tersedia pada katalog ruleset aktif");
            }

            if (!_payloadReader.TryGetString(payload, "source_order_event_id", out var sourceOrderEventIdText) ||
                !Guid.TryParse(sourceOrderEventIdText, out var sourceOrderEventId))
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR",
                    "Risiko wajib merujuk event JualMasakan",
                    new ErrorDetail("payload.source_order_event_id", "REQUIRED"));
            }

            var events = await _events.GetAllEventsBySessionAsync(request.SessionId, ct);
            var sourceOrder = events.FirstOrDefault(e => e.EventId == sourceOrderEventId);
            if (sourceOrder is null ||
                sourceOrder.UserId != request.UserId ||
                sourceOrder.DayIndex != request.DayIndex ||
                !IsEventAction(sourceOrder, GameActionCatalog.JualMasakan))
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION",
                    "Event sumber risiko bukan JualMasakan pemain pada hari yang sama");
            }

            var sourceAlreadyPaired = events.Any(e =>
                IsEventAction(e, GameActionCatalog.RisikoKehidupan) &&
                _payloadReader.TryGetString(_payloadReader.ReadPayload(e.Payload), "source_order_event_id", out var pairedSource) &&
                string.Equals(pairedSource, sourceOrderEventIdText, StringComparison.OrdinalIgnoreCase));
            if (sourceAlreadyPaired)
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION",
                    "Event JualMasakan sudah dipasangkan dengan risiko lain");
            }

            return Valid;
        }

        if (IsAction(request, GameActionCatalog.BayarRisiko))
        {
            if (!string.Equals(config.Mode, "MAHIR", StringComparison.OrdinalIgnoreCase))
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION",
                    "Pembayaran risiko hanya tersedia di mode MAHIR");
            }

            if (request.UserId is null ||
                !_payloadReader.TryGetString(payload, "risk_event_id", out var riskEventIdText) ||
                !Guid.TryParse(riskEventIdText, out var riskEventId))
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR",
                    "BayarRisiko wajib merujuk risk_event_id",
                    new ErrorDetail("payload.risk_event_id", "REQUIRED"));
            }

            var events = await _events.GetAllEventsBySessionAsync(request.SessionId, ct);
            var riskEvent = events.FirstOrDefault(e => e.EventId == riskEventId);
            if (riskEvent is null ||
                riskEvent.UserId != request.UserId ||
                !IsEventAction(riskEvent, GameActionCatalog.RisikoKehidupan) ||
                !TryResolveLifeRisk(config, _payloadReader.ReadPayload(riskEvent.Payload), out var risk) ||
                !IsPersonalCoinOutRisk(risk))
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION",
                    "BayarRisiko hanya dapat menyelesaikan risiko OUT milik pemain");
            }

            if (await _events.IsRiskResolvedAsync(request.SessionId, riskEventId, ct))
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION",
                    "Risk event sudah diselesaikan");
            }

            return await EnsureSufficientBalanceAsync(request, config, risk.Amount, ct);
        }

        if (IsAction(request, GameActionCatalog.Asuransi) &&
            _payloadReader.TryReadInsuranceUsed(payload, out _))
        {
            if (!config.InsuranceEnabled)
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Fitur asuransi tidak aktif");
            }

            if (request.UserId is null)
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Player wajib diisi",
                    new ErrorDetail("user_id", "REQUIRED"));
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

            if (!await _events.HasActiveInsuranceAsync(request.SessionId, request.UserId.Value, ct))
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Polis asuransi tidak aktif atau sudah habis");
            }

            var riskEvent = events.FirstOrDefault(e => e.EventId == riskEventId);
            if (riskEvent is null || !IsEventAction(riskEvent, GameActionCatalog.RisikoKehidupan))
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Risk event tidak ditemukan");
            }

            if (riskEvent.UserId != request.UserId)
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Risk event bukan milik pemain");
            }

            var riskPayload = _payloadReader.ReadPayload(riskEvent.Payload);
            if (!TryResolveLifeRisk(config, riskPayload, out var risk) ||
                !IsPersonalCoinOutRisk(risk))
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Asuransi hanya berlaku untuk risiko OUT");
            }

            var alreadyUsed = events.Any(e =>
                IsEventAction(e, GameActionCatalog.Asuransi) &&
                _payloadReader.TryReadInsuranceUsed(_payloadReader.ReadPayload(e.Payload), out var usedRiskEventId) &&
                string.Equals(usedRiskEventId, riskEventIdText, StringComparison.OrdinalIgnoreCase));

            if (alreadyUsed)
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Risk event sudah ditangkal asuransi");
            }

            if (await _events.IsRiskResolvedAsync(request.SessionId, riskEventId, ct))
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Risk event sudah diselesaikan");
            }

            return Valid;
        }

        if (string.Equals(actionType, "GunakanOpsiDarurat", StringComparison.OrdinalIgnoreCase))
        {
            if (!string.Equals(config.Mode, "MAHIR", StringComparison.OrdinalIgnoreCase))
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Fitur darurat hanya tersedia di mode MAHIR");
            }

            if (request.UserId is null)
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Player wajib diisi",
                    new ErrorDetail("user_id", "REQUIRED"));
            }

            if (!_payloadReader.TryReadEmergencyOption(payload, out var riskEventIdText, out var optionType, out _, out var amount))
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

            if (string.IsNullOrWhiteSpace(optionType) ||
                !AllowedEmergencyOptions.Contains(optionType))
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Option type tidak valid",
                    new ErrorDetail("payload.option_type", "INVALID_ENUM"));
            }

            var events = await _events.GetAllEventsBySessionAsync(request.SessionId, ct);
            var riskEvent = events.FirstOrDefault(e => e.EventId == riskEventId);
            if (riskEvent is null || !IsEventAction(riskEvent, GameActionCatalog.RisikoKehidupan))
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Risk event tidak ditemukan");
            }

            if (riskEvent.UserId != request.UserId)
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Risk event bukan milik pemain");
            }

            var riskPayload = _payloadReader.ReadPayload(riskEvent.Payload);
            if (!TryResolveLifeRisk(config, riskPayload, out var risk) ||
                !IsPersonalCoinOutRisk(risk))
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Emergency option hanya berlaku untuk risiko OUT");
            }

            if (await _events.IsRiskResolvedAsync(request.SessionId, riskEventId, ct))
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Risk event sudah diselesaikan");
            }

            var projections = await _events.GetCashflowProjectionsAsync(request.SessionId, ct);
            var currentBalance = _playerBalanceCalc.Compute(request.UserId.Value, config.StartingCash, projections);
            if (currentBalance >= risk.Amount)
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION",
                    "Opsi darurat hanya dapat digunakan saat saldo tidak cukup membayar risiko");
            }

            switch (optionType.ToUpperInvariant())
            {
                case "SELL_NEED":
                    if (!_payloadReader.TryGetString(payload, "card_id", out var cardId) ||
                        await _events.GetOwnedNeedSaleAmountAsync(request.SessionId, request.UserId.Value, cardId, ct) != amount)
                    {
                        return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION",
                            "Kartu kebutuhan tidak dimiliki atau nilai jualnya tidak valid");
                    }
                    break;
                case "SELL_GOLD":
                    if (!_payloadReader.TryGetInt32(payload, "qty", out var qty) || qty <= 0 ||
                        !_payloadReader.TryGetInt32(payload, "unit_price", out var unitPrice) ||
                        amount != qty * unitPrice ||
                        await _events.GetGoldQuantityAsync(request.SessionId, request.UserId.Value, ct) < qty ||
                        !_payloadReader.TryGetString(payload, "gold_price_event_id", out var priceEventIdText) ||
                        !Guid.TryParse(priceEventIdText, out var priceEventId))
                    {
                        return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION",
                            "Emas atau referensi harga emas tidak valid");
                    }

                    var activePriceEvent = events
                        .Where(e => string.Equals(e.ActionType, "BukaHargaEmas", StringComparison.OrdinalIgnoreCase) &&
                                    e.DayIndex == request.DayIndex)
                        .OrderByDescending(e => e.SequenceNumber)
                        .FirstOrDefault();
                    if (activePriceEvent?.EventId != priceEventId ||
                        !_payloadReader.TryGetInt32(_payloadReader.ReadPayload(activePriceEvent.Payload), "gold_price", out var activePrice) ||
                        activePrice != unitPrice)
                    {
                        return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION",
                            "Harga emas tidak berasal dari event harga yang aktif");
                    }
                    break;
                case "TAKE_SHARIA_LOAN":
                    if (!_payloadReader.TryGetString(payload, "loan_code", out var loanCode) ||
                        !_payloadReader.TryReadLoanTaken(payload, out _, out var principal, out var repayment, out var duration, out var penalty) ||
                        !config.ShariaLoans.Any(loan =>
                            string.Equals(loan.LoanCode, loanCode, StringComparison.OrdinalIgnoreCase) &&
                            loan.Principal == principal &&
                            loan.RepaymentAmount == repayment &&
                            loan.DurationDays == duration &&
                            loan.PenaltyPoints == penalty))
                    {
                        return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION",
                            "Pinjaman darurat tidak sesuai katalog ruleset");
                    }

                    if (events.Any(e =>
                            IsEventAction(e, GameActionCatalog.RiskEmergencyUsed) &&
                            _payloadReader.TryGetString(_payloadReader.ReadPayload(e.Payload), "risk_event_id", out var usedRisk) &&
                            string.Equals(usedRisk, riskEventIdText, StringComparison.OrdinalIgnoreCase) &&
                            _payloadReader.TryGetString(_payloadReader.ReadPayload(e.Payload), "option_type", out var usedOption) &&
                            string.Equals(usedOption, "TAKE_SHARIA_LOAN", StringComparison.OrdinalIgnoreCase)))
                    {
                        return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION",
                            "Risk event sudah memakai pinjaman darurat");
                    }
                    break;
            }

            return Valid;
        }

        if (string.Equals(actionType, "PinjamanSyariah", StringComparison.OrdinalIgnoreCase))
        {
            if (!config.LoanEnabled)
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Fitur pinjaman tidak aktif");
            }

            if (request.UserId is null)
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Player wajib diisi",
                    new ErrorDetail("user_id", "REQUIRED"));
            }

            if (!_payloadReader.TryReadLoanTaken(payload, out var loanId, out var principal, out var repaymentAmount, out var duration, out var penaltyPoints))
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Payload loan taken tidak valid",
                    new ErrorDetail("payload.loan_id", "REQUIRED"));
            }

            if (principal <= 0 || repaymentAmount < 0 || duration <= 0)
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Nilai pinjaman tidak valid",
                    new ErrorDetail("payload.principal", "OUT_OF_RANGE"));
            }

            if (penaltyPoints < 0)
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Penalty points tidak valid",
                    new ErrorDetail("payload.penalty_points", "OUT_OF_RANGE"));
            }

            var loanCode = _payloadReader.TryGetString(payload, "loan_code", out var requestedLoanCode)
                ? requestedLoanCode
                : loanId;
            var matchesLoanCatalog = config.ShariaLoans.Any(loan =>
                string.Equals(loan.LoanCode, loanCode, StringComparison.OrdinalIgnoreCase) &&
                loan.Principal == principal &&
                loan.RepaymentAmount == repaymentAmount &&
                loan.DurationDays == duration &&
                loan.PenaltyPoints == penaltyPoints);
            if (!matchesLoanCatalog)
            {
                return BuildOutcome(
                    StatusCodes.Status422UnprocessableEntity,
                    "DOMAIN_RULE_VIOLATION",
                    "Detail pinjaman tidak tersedia pada katalog ruleset aktif");
            }

            var events = await _events.GetAllEventsBySessionAsync(request.SessionId, ct);
            if (_payloadReader.TryGetString(payload, "risk_event_id", out var loanRiskEventIdText))
            {
                if (!Guid.TryParse(loanRiskEventIdText, out var loanRiskEventId))
                {
                    return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Risk event id tidak valid",
                        new ErrorDetail("payload.risk_event_id", "INVALID_FORMAT"));
                }

                var loanRiskEvent = events.FirstOrDefault(e => e.EventId == loanRiskEventId);
                if (loanRiskEvent is null ||
                    loanRiskEvent.UserId != request.UserId ||
                    !IsEventAction(loanRiskEvent, GameActionCatalog.RisikoKehidupan) ||
                    !TryResolveLifeRisk(config, _payloadReader.ReadPayload(loanRiskEvent.Payload), out var loanRisk) ||
                    !IsPersonalCoinOutRisk(loanRisk) ||
                    await _events.IsRiskResolvedAsync(request.SessionId, loanRiskEventId, ct))
                {
                    return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION",
                        "Pinjaman slot 0 wajib merujuk risiko PENDING milik pemain");
                }

                var projections = await _events.GetCashflowProjectionsAsync(request.SessionId, ct);
                var currentBalance = _playerBalanceCalc.Compute(request.UserId.Value, config.StartingCash, projections);
                if (currentBalance >= loanRisk.Amount)
                {
                    return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION",
                        "Saldo pemain masih cukup untuk membayar risiko");
                }

                if (events.Any(e =>
                        e.UserId == request.UserId &&
                        IsEventAction(e, GameActionCatalog.PinjamanSyariah) &&
                        _payloadReader.TryGetString(_payloadReader.ReadPayload(e.Payload), "risk_event_id", out var priorRiskId) &&
                        string.Equals(priorRiskId, loanRiskEventIdText, StringComparison.OrdinalIgnoreCase)))
                {
                    return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION",
                        "Risk event sudah memakai pinjaman syariah");
                }
            }

            var exists = events.Any(e =>
                e.UserId == request.UserId &&
                IsEventAction(e, GameActionCatalog.PinjamanSyariah) &&
                _payloadReader.TryReadLoanTaken(_payloadReader.ReadPayload(e.Payload), out var existingLoanId, out _, out _, out _, out _) &&
                string.Equals(existingLoanId, loanId, StringComparison.OrdinalIgnoreCase));
            if (exists)
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Loan ID sudah dipakai");
            }

            return Valid;
        }

        if (string.Equals(actionType, "BayarPinjaman", StringComparison.OrdinalIgnoreCase))
        {
            if (!config.LoanEnabled)
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Fitur pinjaman tidak aktif");
            }

            if (request.UserId is null)
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Player wajib diisi",
                    new ErrorDetail("user_id", "REQUIRED"));
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

            var outstanding = await _events.GetActiveLoanOutstandingAsync(
                request.SessionId,
                request.UserId.Value,
                loanId,
                ct);
            if (!outstanding.HasValue)
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Pinjaman aktif tidak ditemukan");
            }

            if (amount != outstanding.Value)
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION",
                    "Pembayaran harus melunasi seluruh sisa pinjaman dalam satu aksi");
            }

            var balanceCheck = await EnsureSufficientBalanceAsync(request, config, amount, ct);
            if (!balanceCheck.IsValid)
            {
                return balanceCheck;
            }

            return Valid;
        }

        if (IsAction(request, GameActionCatalog.Asuransi) &&
            !_payloadReader.TryReadInsuranceUsed(payload, out _))
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

            var isInitialSetup = string.Equals(request.ActorType, "SYSTEM", StringComparison.OrdinalIgnoreCase) &&
                                 _payloadReader.TryGetOptionalString(payload, "setup", out var setupValue) &&
                                 string.Equals(setupValue, "INITIAL", StringComparison.OrdinalIgnoreCase);
            if (premium <= 0 && !isInitialSetup)
            {
                return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Premium harus > 0",
                    new ErrorDetail("payload.premium", "OUT_OF_RANGE"));
            }

            if (!isInitialSetup && !config.InsuranceProducts.Any(product => product.Premium == premium))
            {
                return BuildOutcome(
                    StatusCodes.Status422UnprocessableEntity,
                    "DOMAIN_RULE_VIOLATION",
                    "Premium asuransi tidak tersedia pada katalog ruleset aktif");
            }

            if (!isInitialSetup && request.UserId is not null)
            {
                if (await _events.HasActiveInsuranceAsync(request.SessionId, request.UserId.Value, ct))
                {
                    return BuildOutcome(
                        StatusCodes.Status422UnprocessableEntity,
                        "DOMAIN_RULE_VIOLATION",
                        "Polis asuransi masih aktif dan tidak dapat diaktifkan ulang");
                }

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
        if (request.UserId is null)
        {
            return Valid;
        }

        var projections = await _events.GetCashflowProjectionsAsync(request.SessionId, ct);
        var currentBalance = _playerBalanceCalc.Compute(request.UserId.Value, config.StartingCash, projections);
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
    private async Task<SessionAccessOutcome> ValidateSessionAccessAsync(Guid sessionId, ClaimsPrincipal user, CancellationToken ct)
    {
        var role = user.FindFirstValue(ClaimTypes.Role);
        var userIdRaw = user.FindFirstValue(ClaimTypes.NameIdentifier);
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
            var playerUserId = await _users.GetPlayerUserIdAsync(userId, ct);
            if (!playerUserId.HasValue)
            {
                return BuildAccessOutcome(
                    StatusCodes.Status403Forbidden,
                    "FORBIDDEN",
                    "Akun PLAYER belum terhubung ke profil pemain");
            }

            var inSession = await _players.IsPlayerInSessionAsync(sessionId, playerUserId.Value, ct);
            if (!inSession)
            {
                return BuildAccessOutcome(
                    StatusCodes.Status403Forbidden,
                    "FORBIDDEN",
                    "Player tidak terdaftar di sesi ini");
            }

            return new SessionAccessOutcome(true, StatusCodes.Status200OK, null, playerUserId.Value);
        }

        return BuildAccessOutcome(StatusCodes.Status403Forbidden, "FORBIDDEN", "Role tidak diizinkan");
    }

    /// <summary>
    /// Membangun ValidationOutcome gagal dari kode status, kode error, dan pesan.
    /// </summary>
    private ValidationOutcome BuildOutcome(int statusCode, string code, string message, params ErrorDetail[] details)
    {
        var error = BuildError(code, message, details);
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

    private static bool IsAction(EventRequest request, string actionId)
    {
        return GameActionCatalog.Is(request.ActionType, request.Payload, actionId);
    }

    private bool IsEventAction(EventDb evt, string actionId)
    {
        var payload = _payloadReader.ReadPayload(string.IsNullOrWhiteSpace(evt.Payload) ? "{}" : evt.Payload);
        return GameActionCatalog.Is(evt.ActionType, payload, actionId);
    }

    /// <summary>
    /// Membangun SessionAccessOutcome gagal dari kode status, kode error, dan pesan.
    /// </summary>
    private SessionAccessOutcome BuildAccessOutcome(int statusCode, string code, string message)
    {
        var error = BuildError(code, message);
        return new SessionAccessOutcome(false, statusCode, error, null);
    }

    /// <summary>
    /// Membangun ErrorResponse menggunakan HttpContext dari IHttpContextAccessor.
    /// </summary>
    private ErrorResponse BuildError(string code, string message, params ErrorDetail[] details)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext is not null)
        {
            return ApiErrorHelper.BuildError(httpContext, code, message, details);
        }

        return new ErrorResponse(code, message, details.ToList(), "unknown");
    }
}
