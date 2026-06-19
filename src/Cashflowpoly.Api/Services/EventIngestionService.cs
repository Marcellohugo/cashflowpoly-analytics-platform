using System.Collections.Frozen;
using System.Security.Claims;
using System.Text.Json;
using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Domain;
using Cashflowpoly.Api.Infrastructure;
using Cashflowpoly.Api.Contracts;
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
        "SELL_NEED", "SELL_GOLD", "SELL_GOAL", "OTHER"
    }.ToFrozenSet(StringComparer.OrdinalIgnoreCase);

    private static readonly FrozenSet<string> OrderedActionTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        GameActionCatalog.BahanMasakan,
        GameActionCatalog.IngredientDiscarded,
        GameActionCatalog.JualMasakan,
        GameActionCatalog.OrderPassed,
        GameActionCatalog.Kebutuhan,
        GameActionCatalog.KerjaLepas,
        GameActionCatalog.Menabung,
        GameActionCatalog.SavingDepositWithdrawn,
        GameActionCatalog.TujuanFinansial,
        GameActionCatalog.JumatBerkah,
        GameActionCatalog.InvestasiEmas,
        GameActionCatalog.JualEmas,
        GameActionCatalog.PinjamanSyariah,
        GameActionCatalog.BayarPinjaman,
        GameActionCatalog.Asuransi,
        GameActionCatalog.RisikoKehidupan,
        GameActionCatalog.RiskEmergencyUsed,
        GameActionCatalog.AkhirGiliran
    }.ToFrozenSet(StringComparer.OrdinalIgnoreCase);

    private readonly SessionRepository _sessions;
    private readonly RulesetRepository _rulesets;
    private readonly EventRepository _events;
    private readonly PlayerRepository _players;
    private readonly UserRepository _users;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IEventPayloadReader _payloadReader;
    private readonly IEventValidationDetailsSerializer _validationSerializer;
    private readonly IEventRecordMapper _recordMapper;
    private readonly IEventRequestShapeValidator _shapeValidator;
    private readonly IEventInsuranceOffsetBuilder _insuranceOffsetBuilder;
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

    public EventIngestionService(
        SessionRepository sessions,
        RulesetRepository rulesets,
        EventRepository events,
        PlayerRepository players,
        UserRepository users,
        IHttpContextAccessor httpContextAccessor,
        IEventPayloadReader payloadReader,
        IEventValidationDetailsSerializer validationSerializer,
        IEventRecordMapper recordMapper,
        IEventRequestShapeValidator shapeValidator,
        IEventInsuranceOffsetBuilder insuranceOffsetBuilder,
        IEventCashflowProjectionBuilder projectionBuilder,
        IEventSimpleActionValidator simpleActionValidator,
        IEventTurnProgressValidator turnProgressValidator,
        IEventNeedPurchaseValidator needPurchaseValidator,
        IEventIngredientOrderValidator ingredientOrderValidator,
        IEventSavingGoalValidator savingGoalValidator,
        IEventEconomyActionValidator economyActionValidator,
        IEventAssignmentValidator assignmentValidator,
        IEventPlayerBalanceCalculator playerBalanceCalc,
        SessionEventProjector projector)
    {
        _sessions = sessions;
        _rulesets = rulesets;
        _events = events;
        _players = players;
        _users = users;
        _httpContextAccessor = httpContextAccessor;
        _payloadReader = payloadReader;
        _validationSerializer = validationSerializer;
        _recordMapper = recordMapper;
        _shapeValidator = shapeValidator;
        _insuranceOffsetBuilder = insuranceOffsetBuilder;
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
    }

    /// <summary>
    /// Menerima satu event gameplay, memvalidasi aturan domain, menyimpan ke database, dan mencatat log validasi.
    /// </summary>
    public async Task<(EventStoredResponse? Result, int StatusCode, ErrorResponse? Error)> IngestEventAsync(
        EventRequest request, ClaimsPrincipal user, CancellationToken ct)
    {
        var validation = await ValidateEventAsync(request, user, ct);
        if (!validation.IsValid)
        {
            await _events.InsertValidationLogAsync(
                request.SessionId,
                request.EventId,
                request.RulesetVersionId,
                request.Payload.GetRawText(),
                validation.Error?.ErrorCode,
                validation.Error?.Message,
                _validationSerializer.BuildValidationDetailsJson(request, validation.Error),
                ct);

            return (null, validation.StatusCode, validation.Error);
        }

        try
        {
            await StoreEventAsync(request, ct);
            return (new EventStoredResponse(true, request.EventId), StatusCodes.Status201Created, null);
        }
        catch (PostgresException ex) when (ex.SqlState == "23505")
        {
            var error = BuildError("DUPLICATE", "Event sudah ada");
            await _events.InsertValidationLogAsync(request.SessionId, request.EventId, request.RulesetVersionId, request.Payload.GetRawText(), error.ErrorCode, error.Message,
                _validationSerializer.BuildValidationDetailsJson(request, error), ct);
            return (null, StatusCodes.Status409Conflict, error);
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
            var validation = await ValidateEventAsync(evt, user, ct);
            if (!validation.IsValid)
            {
                failed.Add(new EventBatchFailed(evt.EventId, validation.Error?.ErrorCode ?? "VALIDATION_ERROR"));
                await _events.InsertValidationLogAsync(
                    evt.SessionId,
                    evt.EventId,
                    evt.RulesetVersionId,
                    evt.Payload.GetRawText(),
                    validation.Error?.ErrorCode,
                    validation.Error?.Message,
                    _validationSerializer.BuildValidationDetailsJson(evt, validation.Error),
                    ct);
                continue;
            }

            try
            {
                await StoreEventAsync(evt, ct);
                storedCount++;
            }
            catch (PostgresException ex) when (ex.SqlState == "23505")
            {
                failed.Add(new EventBatchFailed(evt.EventId, "DUPLICATE"));
                var dupError = BuildError("DUPLICATE", "Event sudah ada");
                await _events.InsertValidationLogAsync(evt.SessionId, evt.EventId, evt.RulesetVersionId, evt.Payload.GetRawText(), dupError.ErrorCode, dupError.Message,
                    _validationSerializer.BuildValidationDetailsJson(evt, dupError), ct);
            }
        }

        return (new EventBatchResponse(storedCount, failed), StatusCodes.Status200OK, null);
    }

    /// <summary>
    /// Mengambil daftar event sesi dengan pagination berbasis sequence number.
    /// </summary>
    public async Task<(EventsBySessionResponse? Result, int StatusCode, ErrorResponse? Error)> GetEventsBySessionAsync(
        Guid sessionId, ClaimsPrincipal user, long fromSeq, int limit, CancellationToken ct)
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

        if (fromSeq < 0)
        {
            return (null, StatusCodes.Status400BadRequest, BuildError("VALIDATION_ERROR", "fromSeq tidak boleh negatif",
                new ErrorDetail("fromSeq", "OUT_OF_RANGE")));
        }

        if (limit is < 1 or > 1000)
        {
            return (null, StatusCodes.Status400BadRequest, BuildError("VALIDATION_ERROR", "limit harus antara 1 sampai 1000",
                new ErrorDetail("limit", "OUT_OF_RANGE")));
        }

        var events = await _events.GetEventsBySessionAsync(sessionId, fromSeq, limit, ct);
        var responseEvents = events.Select(_recordMapper.ToEventRequest).ToList();
        return (new EventsBySessionResponse(sessionId, responseEvents), StatusCodes.Status200OK, null);
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

        return Valid;
    }

    private async Task<ValidationOutcome> ValidateDailyActionOrderAsync(EventRequest request, RulesetConfig config, CancellationToken ct)
    {
        if (request.ActorType.Equals("SYSTEM", StringComparison.OrdinalIgnoreCase))
        {
            return Valid;
        }

        if (!OrderedActionTypes.Contains(request.ActionType))
        {
            return Valid;
        }

        if (request.UserId is null)
        {
            return BuildOutcome(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Player wajib diisi",
                new ErrorDetail("user_id", "REQUIRED"));
        }

        if (request.ActionSlot < 1 || request.ActionSlot > config.ActionsPerTurn)
        {
            return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION",
                "Aksi pemain di luar slot aksi harian",
                new ErrorDetail("action_slot", "OUT_OF_RANGE"));
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

        var events = await _events.GetAllEventsBySessionAsync(request.SessionId, ct);
        var dayEvents = events
            .Where(e => e.DayIndex == request.DayIndex && e.SessionPlayerId.HasValue && OrderedActionTypes.Contains(e.ActionType))
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

        // Kumpulkan proyeksi sebelum transaksi
        CashflowProjectionDb? insuranceOffset = null;
        if (_insuranceOffsetBuilder.TryReadRiskEventReference(request, out _, out var riskEventId))
        {
            var riskEvent = await _events.GetEventByIdAsync(request.SessionId, riskEventId, ct);
            if (!TryBuildCatalogInsuranceOffset(request, timestamp, eventPk, riskEvent, config, out insuranceOffset))
            {
                _insuranceOffsetBuilder.TryBuild(request, timestamp, eventPk, riskEvent, out insuranceOffset);
            }
        }

        var projections = new List<CashflowProjectionDb>();
        if (!TryBuildCatalogRiskProjection(request, timestamp, eventPk, config, out var projection))
        {
            _projectionBuilder.TryBuild(request, timestamp, eventPk, out projection);
        }

        if (projection is not null)
        {
            projections.Add(projection);
        }

        if (insuranceOffset is not null)
        {
            projections.Add(insuranceOffset);
        }

        // Simpan event + seluruh proyeksi state dalam satu transaksi.
        await using var conn = await _events.OpenConnectionAsync(ct);
        await using var tx = await conn.BeginTransactionAsync(ct);

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
            case GameActionCatalog.OrderPassed:
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
            case GameActionCatalog.GoldInitialGranted:
                Add(references, payload, "GOLD", "asset_code", "TARGET");
                if (references.Count == 0)
                {
                    references.Add(new EventAssetReferenceInput("GOLD", "gold_card_1", "TARGET", "$.asset_code"));
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
            case GameActionCatalog.CardTaken:
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
            IsAction(request, GameActionCatalog.InvestasiEmas) ||
            IsAction(request, GameActionCatalog.JualEmas))
        {
            IEnumerable<EventDb> events = IsAction(request, GameActionCatalog.InvestasiEmas) ||
                                          IsAction(request, GameActionCatalog.JualEmas)
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

        if (IsAction(request, GameActionCatalog.MissionAssigned) ||
            IsAction(request, GameActionCatalog.TieBreakerAssigned))
        {
            var events = await _events.GetAllEventsBySessionAsync(request.SessionId, ct);
            if (_assignmentValidator.TryValidate(request, events, out var assignmentValidation))
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

            var events = await _events.GetAllEventsBySessionAsync(request.SessionId, ct);
            var turnEvents = events.Where(e =>
                e.UserId == request.UserId &&
                e.ActionSlot == request.ActionSlot);

            var orderCount = turnEvents.Count(e => IsEventAction(e, GameActionCatalog.JualMasakan));
            var riskCount = turnEvents.Count(e => IsEventAction(e, GameActionCatalog.RisikoKehidupan));

            if (riskCount >= orderCount)
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION",
                    "Risiko hanya dapat diambil setelah klaim pesanan");
            }

            if (string.Equals(risk.Direction, "OUT", StringComparison.OrdinalIgnoreCase))
            {
                var balanceCheck = await EnsureSufficientBalanceAsync(request, config, risk.Amount, ct);
                if (!balanceCheck.IsValid)
                {
                    return balanceCheck;
                }
            }

            return Valid;
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

            var hasPurchased = events.Any(e =>
                e.UserId == request.UserId &&
                IsEventAction(e, GameActionCatalog.Asuransi) &&
                _payloadReader.TryReadInsurance(_payloadReader.ReadPayload(e.Payload), out _));
            if (!hasPurchased)
            {
                return BuildOutcome(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Pemain belum membeli asuransi");
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
                !string.Equals(risk.Direction, "OUT", StringComparison.OrdinalIgnoreCase) ||
                risk.Amount <= 0)
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
                !string.Equals(risk.Direction, "OUT", StringComparison.OrdinalIgnoreCase))
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

            var matchesLoanCatalog = config.ShariaLoans.Any(loan =>
                loan.Principal == principal &&
                loan.Installment == installment &&
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

            var events = await _events.GetAllEventsBySessionAsync(request.SessionId, ct);
            var loan = events.FirstOrDefault(e =>
                e.UserId == request.UserId &&
                IsEventAction(e, GameActionCatalog.PinjamanSyariah) &&
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
                    e.UserId == request.UserId &&
                    IsEventAction(e, GameActionCatalog.BayarPinjaman) &&
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
