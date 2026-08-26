// Fungsi file: Mengorkestrasi alur aplikasi dan domain melalui AnalyticsService.
using System.Security.Claims;
using System.Text.Json;
using Cashflowpoly.Api.Infrastructure;
using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Domain;
using Cashflowpoly.Api.Contracts;
using Microsoft.AspNetCore.Http;

namespace Cashflowpoly.Api.Services;

internal sealed class AnalyticsService : IAnalyticsService
{
    private sealed record ActiveRulesetContext(Guid? VersionId, Guid? RulesetId, string? Name, RulesetConfig? Config);
    private static readonly EventPayloadReader _eventPayloadReader = new();

    private readonly SessionRepository _sessions;
    private readonly EventRepository _events;
    private readonly RulesetRepository _rulesets;
    private readonly MetricsRepository _metrics;
    private readonly PlayerRepository _players;
    private readonly UserRepository _users;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IHappinessCalculator _happinessCalc;
    private readonly IIngredientInventoryCalculator _inventoryCalc;
    private readonly INeedMissionCalculator _needMissionCalculator;
    private readonly IPlayerOrdering _playerOrdering;
    private readonly ISessionMetricCalculator _sessionMetricCalc;
    private readonly IMetricSnapshotBuilder _metricSnapshotBuilder;
    private readonly IScoreCalculator _scoreCalc;
    private readonly IAnalyticsPayloadReader _payloadReader;
    private readonly IGameplaySnapshotBuilder _gameplaySnapshotBuilder;

    public AnalyticsService(
        SessionRepository sessions,
        EventRepository events,
        RulesetRepository rulesets,
        MetricsRepository metrics,
        PlayerRepository players,
        UserRepository users,
        IHttpContextAccessor httpContextAccessor,
        IHappinessCalculator happinessCalc,
        IIngredientInventoryCalculator inventoryCalc,
        INeedMissionCalculator needMissionCalculator,
        IPlayerOrdering playerOrdering,
        ISessionMetricCalculator sessionMetricCalc,
        IMetricSnapshotBuilder metricSnapshotBuilder,
        IScoreCalculator scoreCalc,
        IAnalyticsPayloadReader payloadReader,
        IGameplaySnapshotBuilder gameplaySnapshotBuilder)
    {
        _sessions = sessions;
        _events = events;
        _rulesets = rulesets;
        _metrics = metrics;
        _players = players;
        _users = users;
        _httpContextAccessor = httpContextAccessor;
        _happinessCalc = happinessCalc;
        _inventoryCalc = inventoryCalc;
        _needMissionCalculator = needMissionCalculator;
        _playerOrdering = playerOrdering;
        _sessionMetricCalc = sessionMetricCalc;
        _metricSnapshotBuilder = metricSnapshotBuilder;
        _scoreCalc = scoreCalc;
        _payloadReader = payloadReader;
        _gameplaySnapshotBuilder = gameplaySnapshotBuilder;
    }

    public async Task<(AnalyticsSessionResponse? Result, int StatusCode, ErrorResponse? Error)> RecomputeAsync(
        Guid sessionId, ClaimsPrincipal user, CancellationToken ct)
    {
        var access = await ResolveSessionAccessAsync(sessionId, user, ct);
        if (access.Error is not null)
        {
            return (null, access.StatusCode, access.Error);
        }

        var events = await _events.GetAllEventsBySessionAsync(sessionId, ct);
        var projections = await _events.GetCashflowProjectionsAsync(sessionId, ct);
        var activeRuleset = await GetActiveRulesetContextAsync(sessionId, ct);
        var happinessByPlayer = _happinessCalc.ComputeByPlayer(events, projections, activeRuleset.Config);
        var finalScores = await ResolveFinalScoresAsync(sessionId, access.Session?.Status, ct);
        happinessByPlayer = ApplyFinalScores(happinessByPlayer, finalScores);
        var summary = _scoreCalc.BuildSummary(events, projections);
        var playerPlayerOrders = await _players.GetSessionPlayerPlayerOrderMapAsync(sessionId, ct);
        var byPlayer = await BuildByPlayerAsync(sessionId, events, projections, happinessByPlayer, activeRuleset.Config, playerPlayerOrders, ct);
        var leaderboard = string.Equals(access.Session?.Status, "ENDED", StringComparison.OrdinalIgnoreCase)
            ? BuildFinalLeaderboard(byPlayer, finalScores)
            : [];

        if (activeRuleset.VersionId.HasValue)
        {
            await WriteSnapshotsAsync(
                sessionId,
                activeRuleset.VersionId.Value,
                events,
                projections,
                activeRuleset.Config,
                happinessByPlayer,
                finalScores,
                string.Equals(access.Session?.Status, "ENDED", StringComparison.OrdinalIgnoreCase),
                ct);
        }

        return (new AnalyticsSessionResponse(sessionId, summary, byPlayer, activeRuleset.RulesetId, activeRuleset.Name, leaderboard), 200, null);
    }

    public async Task<(AnalyticsSessionResponse? Result, int StatusCode, ErrorResponse? Error)> GetSessionAnalyticsAsync(
        Guid sessionId, ClaimsPrincipal user, CancellationToken ct)
    {
        var access = await ResolveSessionAccessAsync(sessionId, user, ct);
        if (access.Error is not null)
        {
            return (null, access.StatusCode, access.Error);
        }

        var scope = await ResolvePlayerScopeAsync(sessionId, user, ct);
        if (scope.Error is not null)
        {
            return (null, scope.Error.Value.StatusCode, scope.Error.Value.ErrorResponse);
        }

        var events = await _events.GetAllEventsBySessionAsync(sessionId, ct);
        var projections = await _events.GetCashflowProjectionsAsync(sessionId, ct);
        var activeRuleset = await GetActiveRulesetContextAsync(sessionId, ct);
        var happinessByPlayer = _happinessCalc.ComputeByPlayer(events, projections, activeRuleset.Config);
        var finalScores = await ResolveFinalScoresAsync(sessionId, access.Session?.Status, ct);
        happinessByPlayer = ApplyFinalScores(happinessByPlayer, finalScores);
        var summary = _scoreCalc.BuildSummary(events, projections);
        var playerPlayerOrders = await _players.GetSessionPlayerPlayerOrderMapAsync(sessionId, ct);
        var byPlayer = await BuildByPlayerAsync(sessionId, events, projections, happinessByPlayer, activeRuleset.Config, playerPlayerOrders, ct);
        var leaderboard = string.Equals(access.Session?.Status, "ENDED", StringComparison.OrdinalIgnoreCase)
            ? BuildFinalLeaderboard(byPlayer, finalScores)
            : [];
        if (scope.UserId.HasValue)
        {
            byPlayer = byPlayer.Where(item => item.UserId == scope.UserId.Value).ToList();
        }

        return (new AnalyticsSessionResponse(sessionId, summary, byPlayer, activeRuleset.RulesetId, activeRuleset.Name, leaderboard), 200, null);
    }

    public async Task<(TransactionHistoryResponse? Result, int StatusCode, ErrorResponse? Error)> GetTransactionsAsync(
        Guid sessionId, Guid? userId, string? cursor, int limit, ClaimsPrincipal user, CancellationToken ct)
    {
        var access = await ResolveSessionAccessAsync(sessionId, user, ct);
        if (access.Error is not null)
        {
            return (null, access.StatusCode, access.Error);
        }

        var scope = await ResolvePlayerScopeAsync(sessionId, user, ct);
        if (scope.Error is not null)
        {
            return (null, scope.Error.Value.StatusCode, scope.Error.Value.ErrorResponse);
        }

        var effectiveUserId = scope.UserId ?? userId;

        if (!OpaqueCursor.TryDecodeTransaction(cursor, out var afterTimestamp, out var afterTransactionId))
        {
            return (null, StatusCodes.Status400BadRequest, BuildError("VALIDATION_ERROR", "Cursor transaksi tidak valid",
                new ErrorDetail("cursor", "INVALID_FORMAT")));
        }

        if (limit is < 1 or > 100)
        {
            return (null, StatusCodes.Status400BadRequest, BuildError("VALIDATION_ERROR", "limit harus antara 1 sampai 100",
                new ErrorDetail("limit", "OUT_OF_RANGE")));
        }

        var projections = await _events.GetCashflowProjectionPageAsync(
            sessionId,
            effectiveUserId,
            string.IsNullOrWhiteSpace(cursor) ? null : afterTimestamp,
            string.IsNullOrWhiteSpace(cursor) ? null : afterTransactionId,
            limit + 1,
            ct);
        var hasMore = projections.Count > limit;
        if (hasMore)
        {
            projections.RemoveAt(projections.Count - 1);
        }

        var items = projections
            .Select(p => new TransactionHistoryItem(p.ProjectionId, p.Timestamp, p.Direction, p.Amount, p.Category))
            .ToList();

        var nextCursor = projections.Count > 0
            ? OpaqueCursor.EncodeTransaction(projections[^1].Timestamp, projections[^1].ProjectionId)
            : null;
        return (new TransactionHistoryResponse(items, nextCursor, hasMore), 200, null);
    }

    public async Task<(GameplayMetricsResponse? Result, int StatusCode, ErrorResponse? Error)> GetGameplayMetricsAsync(
        Guid sessionId, Guid userId, ClaimsPrincipal user, CancellationToken ct)
    {
        var access = await ResolveSessionAccessAsync(sessionId, user, ct);
        if (access.Error is not null)
        {
            return (null, access.StatusCode, access.Error);
        }

        var scope = await ResolvePlayerScopeAsync(sessionId, user, ct);
        if (scope.Error is not null)
        {
            return (null, scope.Error.Value.StatusCode, scope.Error.Value.ErrorResponse);
        }

        if (scope.UserId.HasValue && scope.UserId.Value != userId)
        {
            return (null, 403, BuildError("FORBIDDEN", "Player hanya dapat melihat metrik miliknya"));
        }

        var activeRuleset = await GetActiveRulesetContextAsync(sessionId, ct);
        var startingCash = activeRuleset.Config?.StartingCash ?? 0;
        Dictionary<string, double> values;
        DateTimeOffset? computedAt;
        string? rawJsonText;
        string? derivedJsonText;

        if (activeRuleset.VersionId.HasValue)
        {
            var events = await _events.GetAllEventsBySessionAsync(sessionId, ct);
            var projections = await _events.GetCashflowProjectionsAsync(sessionId, ct);
            var happinessByPlayer = _happinessCalc.ComputeByPlayer(events, projections, activeRuleset.Config);
            var finalScores = await ResolveFinalScoresAsync(sessionId, access.Session?.Status, ct);
            happinessByPlayer = ApplyFinalScores(happinessByPlayer, finalScores);
            var playerAlias = (await _players.ListSessionPlayersAsync(sessionId, ct))
                .FirstOrDefault(player => player.UserId == userId)?.DisplayName;
            var liveMetrics = ComputePlayerMetrics(
                userId,
                events,
                projections,
                happinessByPlayer.GetValueOrDefault(userId),
                activeRuleset.Config,
                finalScores.FirstOrDefault(score => score.UserId == userId),
                playerAlias,
                string.Equals(access.Session?.Status, "ENDED", StringComparison.OrdinalIgnoreCase));
            values = liveMetrics.ToDictionary(
                item => item.Key,
                item => item.Value.Numeric ?? 0d,
                StringComparer.Ordinal);
            computedAt = events
                .Where(item => item.UserId == userId)
                .Select(item => (DateTimeOffset?)item.Timestamp)
                .Max();
            rawJsonText = liveMetrics.GetValueOrDefault("gameplay.raw.variables").Json;
            derivedJsonText = liveMetrics.GetValueOrDefault("gameplay.derived.metrics").Json;
        }
        else
        {
            var metricNames = new[]
            {
                "cashflow.in.total", "cashflow.out.total", "cashflow.net.total", "donation.total",
                "gold.qty.current", "orders.completed.count", "inventory.ingredient.total", "actions.used.total",
                "happiness.points.total", "happiness.need.points", "happiness.need.bonus", "happiness.donation.points",
                "happiness.gold.points", "happiness.pension.points", "happiness.saving_goal.points",
                "happiness.mission.penalty", "happiness.loan.penalty", "loan.unpaid.flag",
                "needs.fulfillment_diversity"
            };
            var snapshots = await _metrics.GetLatestMetricValuesAsync(sessionId, userId, metricNames, ct);
            values = snapshots.ToDictionary(
                item => item.MetricName,
                item => item.MetricValueNumeric ?? 0d,
                StringComparer.Ordinal);
            computedAt = snapshots.Count == 0 ? null : snapshots.Max(item => item.ComputedAt);
            var snapshotsJson = await _metrics.GetLatestGameplaySnapshotsAsync(sessionId, userId, ct);
            rawJsonText = snapshotsJson.FirstOrDefault(s => s.MetricName == "gameplay.raw.variables")?.MetricValueJson;
            derivedJsonText = snapshotsJson.FirstOrDefault(s => s.MetricName == "gameplay.derived.metrics")?.MetricValueJson;
        }

        JsonElement? rawJson = null;
        JsonElement? derivedJson = null;

        if (!string.IsNullOrWhiteSpace(rawJsonText))
        {
            try
            {
                rawJson = JsonDocument.Parse(rawJsonText).RootElement.Clone();
            }
            catch (JsonException)
            {
            }
        }
        if (!string.IsNullOrWhiteSpace(derivedJsonText))
        {
            try
            {
                derivedJson = JsonDocument.Parse(derivedJsonText).RootElement.Clone();
            }
            catch (JsonException)
            {
            }
        }

        return (new GameplayMetricsResponse(
            sessionId,
            userId,
            computedAt,
            new GameplayEconomyMetrics(
                startingCash,
                ReadMetric(values, "cashflow.in.total"),
                ReadMetric(values, "cashflow.out.total"),
                ReadMetric(values, "cashflow.net.total"),
                ReadMetric(values, "donation.total")),
            new GameplayProgressMetrics(
                (int)ReadMetric(values, "gold.qty.current"),
                (int)ReadMetric(values, "orders.completed.count"),
                (int)ReadMetric(values, "inventory.ingredient.total"),
                (int)ReadMetric(values, "actions.used.total")),
            new GameplayScoreMetrics(
                ReadMetric(values, "happiness.points.total"),
                ReadMetric(values, "happiness.need.points"),
                ReadMetric(values, "happiness.need.bonus"),
                ReadMetric(values, "happiness.donation.points"),
                ReadMetric(values, "happiness.gold.points"),
                ReadMetric(values, "happiness.pension.points"),
                ReadMetric(values, "happiness.saving_goal.points"),
                ReadMetric(values, "happiness.mission.penalty"),
                ReadMetric(values, "happiness.loan.penalty"),
                ReadMetric(values, "loan.unpaid.flag") > 0.5d),
            new GameplayNeedMetrics(
                ReadMetric(values, "needs.fulfillment_diversity")),
            rawJson,
            derivedJson), 200, null);
    }

    public async Task<(RulesetAnalyticsSummaryResponse? Result, int StatusCode, ErrorResponse? Error)> GetRulesetAnalyticsSummaryAsync(
        Guid rulesetId, ClaimsPrincipal user, CancellationToken ct)
    {
        var role = user.FindFirstValue(ClaimTypes.Role);
        var isInstructor = string.Equals(role, "INSTRUCTOR", StringComparison.OrdinalIgnoreCase);
        var isPlayer = string.Equals(role, "PLAYER", StringComparison.OrdinalIgnoreCase);
        RulesetDb? ruleset = null;
        List<SessionDb> sessions;
        Guid? scopedUserId = null;

        if (isInstructor)
        {
            if (!TryGetCurrentUserId(user, out var instructorUserId))
            {
                return (null, 401, BuildError("UNAUTHORIZED", "Token user tidak valid"));
            }

            ruleset = await _rulesets.GetRulesetForInstructorAsync(rulesetId, instructorUserId, ct);
            if (ruleset is null)
            {
                return (null, 404, BuildError("NOT_FOUND", "Ruleset tidak ditemukan"));
            }

            sessions = await _sessions.ListSessionsByInstructorAsync(instructorUserId, ct);
        }
        else if (isPlayer)
        {
            var scope = await ResolvePlayerScopeAsync(null, user, ct);
            if (scope.Error is not null)
            {
                return (null, scope.Error.Value.StatusCode, scope.Error.Value.ErrorResponse);
            }

            scopedUserId = scope.UserId;
            sessions = await _sessions.ListSessionsAsync(ct);
        }
        else
        {
            return (null, 403, BuildError("FORBIDDEN", "Role tidak diizinkan"));
        }

        var sessionItems = new List<RulesetAnalyticsSessionItem>();

        foreach (var session in sessions)
        {
            var activeRulesetVersionId = await _sessions.GetActiveRulesetVersionIdAsync(session.SessionId, ct);
            if (!activeRulesetVersionId.HasValue)
            {
                continue;
            }

            var activeVersion = await _rulesets.GetRulesetVersionByIdAsync(activeRulesetVersionId.Value, ct);
            if (activeVersion is null || activeVersion.RulesetId != rulesetId)
            {
                continue;
            }

            if (scopedUserId.HasValue)
            {
                var inSession = await _players.IsPlayerInSessionAsync(session.SessionId, scopedUserId.Value, ct);
                if (!inSession)
                {
                    continue;
                }
            }

            var events = await _events.GetAllEventsBySessionAsync(session.SessionId, ct);
            var projections = await _events.GetCashflowProjectionsAsync(session.SessionId, ct);
            RulesetConfig? config = null;
            TryBuildRuntimeConfig(activeVersion, out config);

            var happinessByPlayer = _happinessCalc.ComputeByPlayer(events, projections, config);
            var finalScores = await ResolveFinalScoresAsync(session.SessionId, session.Status, ct);
            happinessByPlayer = ApplyFinalScores(happinessByPlayer, finalScores);
            var playerPlayerOrders = await _players.GetSessionPlayerPlayerOrderMapAsync(session.SessionId, ct);
            var byPlayer = await BuildByPlayerAsync(session.SessionId, events, projections, happinessByPlayer, config, playerPlayerOrders, ct);
            var allPlayerItems = new List<RulesetAnalyticsPlayerItem>();

            foreach (var player in byPlayer)
            {
                var learningScore = _scoreCalc.ComputeLearningPerformanceScore(
                    player.CashInTotal,
                    player.CashOutTotal,
                    player.HappinessPointsTotal,
                    player.FulfillmentDiversity);

                var missionScore = _scoreCalc.ComputeMissionPerformanceScore(
                    player.MissionPenaltyTotal,
                    player.LoanPenaltyTotal);

                allPlayerItems.Add(new RulesetAnalyticsPlayerItem(player.UserId, learningScore, missionScore));
            }

            var learningAggregate = _scoreCalc.AverageNullable(allPlayerItems.Select(item => item.LearningPerformanceIndividualScore));
            var missionAggregate = _scoreCalc.AverageNullable(allPlayerItems.Select(item => item.MissionPerformanceIndividualScore));
            var visiblePlayers = scopedUserId.HasValue
                ? allPlayerItems.Where(item => item.UserId == scopedUserId.Value).ToList()
                : allPlayerItems;

            sessionItems.Add(new RulesetAnalyticsSessionItem(
                session.SessionId,
                session.SessionName,
                session.Status,
                events.Count,
                learningAggregate,
                missionAggregate,
                visiblePlayers));
        }

        if (isPlayer && sessionItems.Count == 0)
        {
            return (null, 404, BuildError("NOT_FOUND", "Ruleset tidak ditemukan"));
        }

        if (!isInstructor)
        {
            ruleset = await _rulesets.GetRulesetAsync(rulesetId, ct);
            if (ruleset is null)
            {
                return (null, 404, BuildError("NOT_FOUND", "Ruleset tidak ditemukan"));
            }
        }

        var learningOverall = _scoreCalc.AverageNullable(sessionItems.Select(item => item.LearningPerformanceAggregateScore));
        var missionOverall = _scoreCalc.AverageNullable(sessionItems.Select(item => item.MissionPerformanceAggregateScore));

        return (new RulesetAnalyticsSummaryResponse(
            rulesetId,
            ruleset!.Name,
            sessionItems.Count,
            learningOverall,
            missionOverall,
            sessionItems), 200, null);
    }

    private async Task<(SessionDb? Session, bool IsInstructor, int StatusCode, ErrorResponse? Error)> ResolveSessionAccessAsync(
        Guid sessionId, ClaimsPrincipal user, CancellationToken ct)
    {
        var role = user.FindFirstValue(ClaimTypes.Role);
        var isInstructor = string.Equals(role, "INSTRUCTOR", StringComparison.OrdinalIgnoreCase);
        var isPlayer = string.Equals(role, "PLAYER", StringComparison.OrdinalIgnoreCase);
        if (!isInstructor && !isPlayer)
        {
            return (null, false, 403, BuildError("FORBIDDEN", "Role tidak dikenali"));
        }

        var (session, errorStatus, errorResponse) = await EnsureInstructorSessionAccessAsync(sessionId, user, isInstructor, ct);
        if (errorResponse is not null)
        {
            return (null, isInstructor, errorStatus, errorResponse);
        }

        session ??= await _sessions.GetSessionAsync(sessionId, ct);
        if (session is null)
        {
            return (null, isInstructor, 404, BuildError("NOT_FOUND", "Session tidak ditemukan"));
        }

        return (session, isInstructor, 200, null);
    }

    private async Task<ActiveRulesetContext> GetActiveRulesetContextAsync(Guid sessionId, CancellationToken ct)
    {
        var versionId = await _sessions.GetActiveRulesetVersionIdAsync(sessionId, ct);
        if (!versionId.HasValue)
        {
            return new ActiveRulesetContext(null, null, null, null);
        }

        var rulesetVersion = await _rulesets.GetRulesetVersionByIdAsync(versionId.Value, ct);
        if (rulesetVersion is null)
        {
            return new ActiveRulesetContext(versionId.Value, null, null, null);
        }

        var ruleset = await _rulesets.GetRulesetAsync(rulesetVersion.RulesetId, ct);
        TryBuildRuntimeConfig(rulesetVersion, out var config);

        return new ActiveRulesetContext(versionId.Value, rulesetVersion.RulesetId, ruleset?.Name, config);
    }

    private async Task<(Guid? UserId, (int StatusCode, ErrorResponse ErrorResponse)? Error)> ResolvePlayerScopeAsync(
        Guid? sessionId, ClaimsPrincipal user, CancellationToken ct)
    {
        var role = user.FindFirstValue(ClaimTypes.Role);
        if (string.Equals(role, "INSTRUCTOR", StringComparison.OrdinalIgnoreCase))
        {
            return (null, null);
        }

        if (!string.Equals(role, "PLAYER", StringComparison.OrdinalIgnoreCase))
        {
            return (null, (403, BuildError("FORBIDDEN", "Role tidak dikenali")));
        }

        var userIdRaw = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdRaw, out var userId))
        {
            return (null, (401, BuildError("UNAUTHORIZED", "Token user tidak valid")));
        }

        var playerUserId = await _users.GetPlayerUserIdAsync(userId, ct);
        if (!playerUserId.HasValue)
        {
            return (null, (403, BuildError("FORBIDDEN", "Akun PLAYER belum terhubung ke profil pemain")));
        }

        if (sessionId.HasValue)
        {
            var inSession = await _players.IsPlayerInSessionAsync(sessionId.Value, playerUserId.Value, ct);
            if (!inSession)
            {
                return (null, (403, BuildError("FORBIDDEN", "Player tidak terdaftar di sesi ini")));
            }
        }

        return (playerUserId.Value, null);
    }

    private async Task<(SessionDb? Session, int StatusCode, ErrorResponse? Error)> EnsureInstructorSessionAccessAsync(
        Guid sessionId, ClaimsPrincipal user, bool isInstructor, CancellationToken ct)
    {
        if (!isInstructor)
        {
            return (null, 0, null);
        }

        if (!TryGetCurrentUserId(user, out var instructorUserId))
        {
            return (null, 401, BuildError("UNAUTHORIZED", "Token user tidak valid"));
        }

        var session = await _sessions.GetSessionForInstructorAsync(sessionId, instructorUserId, ct);
        if (session is null)
        {
            return (null, 404, BuildError("NOT_FOUND", "Session tidak ditemukan"));
        }

        return (session, 0, null);
    }

    private static bool TryGetCurrentUserId(ClaimsPrincipal user, out Guid userId)
    {
        var userIdRaw = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userIdRaw, out userId);
    }

    private ErrorResponse BuildError(string code, string message, params ErrorDetail[] details)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext is not null)
        {
            return ApiErrorHelper.BuildError(httpContext, code, message, details);
        }

        return new ErrorResponse(code, message, details.ToList(), "unknown");
    }

    private static double ReadMetric(IReadOnlyDictionary<string, double> values, string name)
        => values.TryGetValue(name, out var value) ? value : 0d;

    private async Task<List<AnalyticsByPlayerItem>> BuildByPlayerAsync(
        Guid sessionId,
        List<EventDb> events,
        List<CashflowProjectionDb> projections,
        Dictionary<Guid, AnalyticsHappinessBreakdown> happinessByPlayer,
        RulesetConfig? config,
        Dictionary<Guid, int> playerPlayerOrders,
        CancellationToken ct)
    {
        var cashTotals = projections
            .GroupBy(p => p.UserId)
            .ToDictionary(
                g => g.Key,
                g => new
                {
                    In = g.Where(p => p.Direction == "IN").Sum(p => (double)p.Amount),
                    Out = g.Where(p => p.Direction == "OUT").Sum(p => (double)p.Amount)
                });

        var result = new List<AnalyticsByPlayerItem>();
        var eventsByPlayer = events.Where(e => e.UserId.HasValue)
            .GroupBy(e => e.UserId!.Value)
            .ToDictionary(group => group.Key, group => group.ToList());
        var playerIds = playerPlayerOrders.Keys
            .Union(eventsByPlayer.Keys)
            .Distinct()
            .ToList();
        var firstEventSequenceByPlayer = playerIds.ToDictionary(
            playerId => playerId,
            playerId => eventsByPlayer.TryGetValue(playerId, out var items) && items.Count > 0
                ? items.Min(item => item.SequenceNumber)
                : long.MaxValue);
        var usernamesByPlayer = await _users.GetUsernamesByUserIdsAsync(playerIds, ct);

        foreach (var playerId in playerIds)
        {
            var playerEvents = eventsByPlayer.TryGetValue(playerId, out var items)
                ? items
                : [];
            var playerOrder = playerPlayerOrders.TryGetValue(playerId, out var assignedPlayerOrder) ? assignedPlayerOrder : 0;

            var totals = cashTotals.TryGetValue(playerId, out var t) ? t : new { In = 0d, Out = 0d };
            var donationTotal = SumDonationTotal(playerEvents);
            var goldQty = SumGoldQuantity(playerEvents);
            var ordersCompletedCount = playerEvents.Count(e => e.ActionType == "JualMasakan");
            var inventoryIngredientTotal = _inventoryCalc.BuildIngredientInventory(playerEvents).Total;
            var actionsUsedTotal = SumActionsUsed(playerEvents);
            var playerProjections = projections.Where(p => p.UserId == playerId).ToList();
            var fulfillmentDiversity = _needMissionCalculator.Compute(playerEvents, playerProjections).FulfillmentDiversity ?? 0d;
            var happiness = happinessByPlayer.TryGetValue(playerId, out var breakdown)
                ? breakdown
                : _happinessCalc.ComputeBreakdown(playerEvents, 0, 0, 0);

            result.Add(new AnalyticsByPlayerItem(
                playerId,
                playerOrder,
                totals.In,
                totals.Out,
                donationTotal,
                goldQty,
                ordersCompletedCount,
                inventoryIngredientTotal,
                actionsUsedTotal,
                fulfillmentDiversity,
                happiness.Total,
                happiness.NeedPoints,
                happiness.NeedSetBonusPoints,
                happiness.DonationPoints,
                happiness.GoldPoints,
                happiness.PensionPoints,
                happiness.SavingGoalPointsEffective,
                happiness.MissionPenaltyPoints,
                happiness.LoanPenaltyPoints,
                happiness.HasUnpaidLoan));
        }

        return _playerOrdering.OrderPlayers(
            result,
            config?.PlayerOrdering ?? PlayerOrdering.PlayerOrder,
            playerPlayerOrders,
            firstEventSequenceByPlayer,
            usernamesByPlayer);
    }

    internal static List<AnalyticsLeaderboardItem> BuildFinalLeaderboard(
        IEnumerable<AnalyticsByPlayerItem> players,
        IReadOnlyCollection<SessionFinalScoreDb>? finalScores = null)
    {
        if (finalScores is { Count: > 0 })
        {
            return finalScores
                .OrderBy(score => score.Rank)
                .ThenBy(score => score.PlayerOrder)
                .Select(score => new AnalyticsLeaderboardItem(
                    score.UserId,
                    score.PlayerOrder,
                    score.Rank,
                    score.TotalPoints))
                .ToList();
        }

        return players
            .OrderByDescending(player => player.HappinessPointsTotal)
            .ThenByDescending(player => player.CashInTotal - player.CashOutTotal)
            .ThenBy(player => player.PlayerOrder > 0 ? player.PlayerOrder : int.MaxValue)
            .ThenBy(player => player.UserId)
            .Select((player, index) => new AnalyticsLeaderboardItem(
                player.UserId,
                player.PlayerOrder,
                index + 1,
                player.HappinessPointsTotal))
            .ToList();
    }

    /// <summary>
    /// Mengganti hasil hitung sementara dengan skor final database setelah sesi berstatus ENDED.
    /// </summary>
    internal static Dictionary<Guid, AnalyticsHappinessBreakdown> ApplyFinalScores(
        Dictionary<Guid, AnalyticsHappinessBreakdown> computed,
        IReadOnlyCollection<SessionFinalScoreDb> finalScores)
    {
        if (finalScores.Count == 0)
        {
            return computed;
        }

        var authoritative = new Dictionary<Guid, AnalyticsHappinessBreakdown>(computed);
        foreach (var score in finalScores)
        {
            authoritative[score.UserId] = new AnalyticsHappinessBreakdown(
                score.TotalPoints,
                score.NeedPoints,
                score.NeedSetBonusPoints,
                score.DonationPoints,
                score.GoldPoints,
                score.PensionPoints,
                score.SavingGoalPoints,
                score.MissionPenaltyPoints,
                score.LoanPenaltyPoints,
                score.HasUnpaidLoan);
        }

        return authoritative;
    }

    private async Task<List<SessionFinalScoreDb>> ResolveFinalScoresAsync(
        Guid sessionId,
        string? status,
        CancellationToken ct)
    {
        return string.Equals(status, "ENDED", StringComparison.OrdinalIgnoreCase)
            ? await _sessions.GetFinalScoresAsync(sessionId, ct)
            : [];
    }

    private async Task WriteSnapshotsAsync(
        Guid sessionId,
        Guid rulesetVersionId,
        List<EventDb> events,
        List<CashflowProjectionDb> projections,
        RulesetConfig? config,
        Dictionary<Guid, AnalyticsHappinessBreakdown> happinessByPlayer,
        IReadOnlyCollection<SessionFinalScoreDb> finalScores,
        bool sessionEnded,
        CancellationToken ct)
    {
        var computedAt = DateTimeOffset.UtcNow;
        var snapshots = new List<MetricSnapshotDb>();

        var sessionMetrics = _sessionMetricCalc.ComputeSessionMetrics(events, projections, happinessByPlayer);
        snapshots.AddRange(_metricSnapshotBuilder.BuildMetricSnapshots(sessionId, null, rulesetVersionId, computedAt, sessionMetrics));

        var playerConfig = config;
        if (playerConfig is null)
        {
            var rulesetVersion = await _rulesets.GetRulesetVersionByIdAsync(rulesetVersionId, ct);
            TryBuildRuntimeConfig(rulesetVersion, out playerConfig);
        }

        var players = events.Where(e => e.UserId.HasValue).Select(e => e.UserId!.Value).Distinct().ToList();
        var aliasesByPlayer = (await _players.ListSessionPlayersAsync(sessionId, ct))
            .ToDictionary(player => player.UserId, player => player.DisplayName);
        foreach (var playerId in players)
        {
            var hasHappiness = happinessByPlayer.TryGetValue(playerId, out var breakdown);
            var playerMetrics = ComputePlayerMetrics(playerId, events, projections,
                hasHappiness ? breakdown : null,
                playerConfig,
                finalScores.FirstOrDefault(score => score.UserId == playerId),
                aliasesByPlayer.GetValueOrDefault(playerId),
                sessionEnded);
            snapshots.AddRange(_metricSnapshotBuilder.BuildMetricSnapshots(sessionId, playerId, rulesetVersionId, computedAt, playerMetrics));
        }

        if (snapshots.Count > 0)
        {
            var lastEventId = events
                .OrderByDescending(item => item.SequenceNumber)
                .Select(item => (Guid?)item.EventId)
                .FirstOrDefault();
            foreach (var snapshot in snapshots)
            {
                snapshot.LastEventId = lastEventId;
            }

            await _metrics.InsertSnapshotsAsync(snapshots, ct);
        }
    }

    private Dictionary<string, (double? Numeric, string? Json)> ComputePlayerMetrics(
        Guid playerId,
        List<EventDb> events,
        List<CashflowProjectionDb> projections,
        AnalyticsHappinessBreakdown? happiness,
        RulesetConfig? config,
        SessionFinalScoreDb? finalScore,
        string? playerAlias,
        bool sessionEnded)
    {
        var metrics = new Dictionary<string, (double? Numeric, string? Json)>();
        var playerEvents = events.Where(e => e.UserId == playerId).ToList();
        var playerProjections = projections.Where(p => p.UserId == playerId).ToList();

        var cashIn = playerProjections.Where(p => p.Direction == "IN").Sum(p => (double)p.Amount);
        var cashOut = playerProjections.Where(p => p.Direction == "OUT").Sum(p => (double)p.Amount);
        metrics["cashflow.in.total"] = (cashIn, null);
        metrics["cashflow.out.total"] = (cashOut, null);
        metrics["cashflow.net.total"] = (cashIn - cashOut, null);

        var donationTotal = SumDonationTotal(playerEvents);
        metrics["donation.total"] = (donationTotal, null);

        var goldQty = SumGoldQuantity(playerEvents);
        metrics["gold.qty.current"] = (goldQty, null);

        var ordersCompleted = playerEvents.Count(e => e.ActionType == "JualMasakan");
        metrics["orders.completed.count"] = (ordersCompleted, null);

        var inventory = _inventoryCalc.BuildIngredientInventory(playerEvents);
        metrics["inventory.ingredient.total"] = (inventory.Total, null);

        var actionsUsed = SumActionsUsed(playerEvents);
        metrics["actions.used.total"] = (actionsUsed, null);

        var fulfillmentDiversity = _needMissionCalculator.Compute(playerEvents, playerProjections).FulfillmentDiversity;
        metrics["needs.fulfillment_diversity"] = (fulfillmentDiversity, null);

        var resolvedHappiness = happiness ?? _happinessCalc.ComputeBreakdown(
            playerEvents,
            _happinessCalc.SumRankAwarded(playerEvents, "PoinPeringkatDonasi"),
            _happinessCalc.SumPointsAwarded(playerEvents, "PoinEmas"),
            _happinessCalc.SumRankAwarded(playerEvents, "PoinPeringkatPensiun"));
        var pensionRank = config is not null && sessionEnded
            ? _happinessCalc.ComputePensionRanks(events, projections, config).GetValueOrDefault(playerId)
            : (int?)null;

        metrics["happiness.points.total"] = (resolvedHappiness.Total, null);
        metrics["happiness.need.points"] = (resolvedHappiness.NeedPoints, null);
        metrics["happiness.need.bonus"] = (resolvedHappiness.NeedSetBonusPoints, null);
        metrics["happiness.donation.points"] = (resolvedHappiness.DonationPoints, null);
        metrics["happiness.gold.points"] = (resolvedHappiness.GoldPoints, null);
        metrics["happiness.pension.points"] = (resolvedHappiness.PensionPoints, null);
        metrics["happiness.saving_goal.points"] = (resolvedHappiness.SavingGoalPointsEffective, null);
        metrics["happiness.mission.penalty"] = (resolvedHappiness.MissionPenaltyPoints, null);
        metrics["happiness.loan.penalty"] = (resolvedHappiness.LoanPenaltyPoints, null);
        metrics["loan.unpaid.flag"] = (resolvedHappiness.HasUnpaidLoan ? 1 : 0, null);

        var gameplaySnapshots = _gameplaySnapshotBuilder.Build(
            playerEvents,
            playerProjections,
            events,
            config,
            resolvedHappiness,
            finalScore,
            pensionRank,
            playerAlias,
            sessionEnded);
        metrics["gameplay.raw.variables"] = (null, gameplaySnapshots.RawJson);
        metrics["gameplay.derived.metrics"] = (null, gameplaySnapshots.DerivedJson);

        return metrics;
    }

    private double SumDonationTotal(IEnumerable<EventDb> events)
        => events
            .Where(e => e.ActionType == "JumatBerkah")
            .Select(e => _payloadReader.TryReadAmount(e.Payload, out var amount) ? amount : 0)
            .Sum();

    private int SumGoldQuantity(IEnumerable<EventDb> events)
        => events
            .Where(e => e.ActionType == GameActionCatalog.SetupEmasAwal ||
                        e.ActionType == GameActionCatalog.InvestasiEmas ||
                        e.ActionType == GameActionCatalog.JualEmas ||
                        IsEmergencyGoldSale(e))
            .Select(e =>
            {
                if (e.ActionType == GameActionCatalog.SetupEmasAwal)
                {
                    return TryReadPayloadInt(e.Payload, "qty", out var initialQty) ? initialQty : 1;
                }

                if (e.ActionType == GameActionCatalog.RiskEmergencyUsed)
                {
                    return TryReadPayloadInt(e.Payload, "qty", out var emergencyQty) ? -emergencyQty : 0;
                }

                if (!_payloadReader.TryReadGoldTrade(e.Payload, out var tradeType, out var qty))
                {
                    return 0;
                }

                return e.ActionType == GameActionCatalog.JualEmas ||
                       string.Equals(tradeType, "SELL", StringComparison.OrdinalIgnoreCase)
                    ? -qty
                    : qty;
            })
            .Sum();

    private bool IsEmergencyGoldSale(EventDb evt) =>
        evt.ActionType == GameActionCatalog.RiskEmergencyUsed &&
        TryReadPayloadString(evt.Payload, "option_type", out var optionType) &&
        string.Equals(optionType, "SELL_GOLD", StringComparison.OrdinalIgnoreCase);

    private static bool TryReadPayloadString(string payload, string propertyName, out string value)
    {
        value = string.Empty;
        try
        {
            using var document = JsonDocument.Parse(payload);
            if (!document.RootElement.TryGetProperty(propertyName, out var property) || property.ValueKind != JsonValueKind.String)
            {
                return false;
            }

            value = property.GetString() ?? string.Empty;
            return value.Length > 0;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    private static bool TryReadPayloadInt(string payload, string propertyName, out int value)
    {
        value = 0;
        try
        {
            using var document = JsonDocument.Parse(payload);
            return document.RootElement.TryGetProperty(propertyName, out var property) && property.TryGetInt32(out value);
        }
        catch (JsonException)
        {
            return false;
        }
    }

    private int SumActionsUsed(IEnumerable<EventDb> events)
        => events
            .Count(e => string.Equals(e.ActorType, "PLAYER", StringComparison.OrdinalIgnoreCase) &&
                        GameActionCatalog.GetPlayerActionSlotPolicy(
                            e.ActionType,
                            _eventPayloadReader.ReadPayload(string.IsNullOrWhiteSpace(e.Payload) ? "{}" : e.Payload)) == PlayerActionSlotPolicy.Consumes);

    private static bool TryBuildRuntimeConfig(RulesetVersionDb? rulesetVersion, out RulesetConfig? config)
    {
        config = null;
        return rulesetVersion?.Definition is not null &&
               RulesetRuntimeMapper.TryBuildConfig(rulesetVersion.Definition, out config, out _);
    }
}
