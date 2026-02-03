using System.Text.Json;
using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Domain;
using Cashflowpoly.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cashflowpoly.Api.Controllers;

[ApiController]
[Route("api/analytics")]
public sealed class AnalyticsController : ControllerBase
{
    private readonly SessionRepository _sessions;
    private readonly EventRepository _events;
    private readonly RulesetRepository _rulesets;
    private readonly MetricsRepository _metrics;

    public AnalyticsController(SessionRepository sessions, EventRepository events, RulesetRepository rulesets, MetricsRepository metrics)
    {
        _sessions = sessions;
        _events = events;
        _rulesets = rulesets;
        _metrics = metrics;
    }

    /// <summary>
    /// Menghitung ulang metrik dan menyimpan snapshot terbaru.
    /// </summary>
    [HttpPost("sessions/{sessionId:guid}/recompute")]
    public async Task<IActionResult> Recompute(Guid sessionId, CancellationToken ct)
    {
        var session = await _sessions.GetSessionAsync(sessionId, ct);
        if (session is null)
        {
            return NotFound(ApiErrorHelper.BuildError(HttpContext, "NOT_FOUND", "Session tidak ditemukan"));
        }

        var events = await _events.GetAllEventsBySessionAsync(sessionId, ct);
        var projections = await _events.GetCashflowProjectionsAsync(sessionId, ct);
        var violations = await _metrics.CountValidationViolationsAsync(sessionId, null, ct);

        var activeRulesetVersionId = await _sessions.GetActiveRulesetVersionIdAsync(sessionId, ct);
        RulesetConfig? config = null;
        if (activeRulesetVersionId.HasValue)
        {
            var rulesetVersion = await _rulesets.GetRulesetVersionByIdAsync(activeRulesetVersionId.Value, ct);
            if (rulesetVersion is not null &&
                RulesetConfigParser.TryParse(rulesetVersion.ConfigJson, out var parsed, out _))
            {
                config = parsed;
            }
        }

        var happinessByPlayer = ComputeHappinessByPlayer(events, projections, config);
        var summary = BuildSummary(events, projections, violations);
        var byPlayer = BuildByPlayer(events, projections, happinessByPlayer);

        if (activeRulesetVersionId.HasValue)
        {
            await WriteSnapshotsAsync(sessionId, activeRulesetVersionId.Value, events, projections, config, happinessByPlayer, ct);
        }

        return Ok(new AnalyticsSessionResponse(sessionId, summary, byPlayer));
    }

    [HttpGet("sessions/{sessionId:guid}")]
    public async Task<IActionResult> GetSessionAnalytics(Guid sessionId, CancellationToken ct)
    {
        var session = await _sessions.GetSessionAsync(sessionId, ct);
        if (session is null)
        {
            return NotFound(ApiErrorHelper.BuildError(HttpContext, "NOT_FOUND", "Session tidak ditemukan"));
        }

        var events = await _events.GetAllEventsBySessionAsync(sessionId, ct);
        var projections = await _events.GetCashflowProjectionsAsync(sessionId, ct);
        var violations = await _metrics.CountValidationViolationsAsync(sessionId, null, ct);

        var activeRulesetVersionId = await _sessions.GetActiveRulesetVersionIdAsync(sessionId, ct);
        RulesetConfig? config = null;
        if (activeRulesetVersionId.HasValue)
        {
            var rulesetVersion = await _rulesets.GetRulesetVersionByIdAsync(activeRulesetVersionId.Value, ct);
            if (rulesetVersion is not null &&
                RulesetConfigParser.TryParse(rulesetVersion.ConfigJson, out var parsed, out _))
            {
                config = parsed;
            }
        }

        var happinessByPlayer = ComputeHappinessByPlayer(events, projections, config);
        var summary = BuildSummary(events, projections, violations);
        var byPlayer = BuildByPlayer(events, projections, happinessByPlayer);

        if (activeRulesetVersionId.HasValue)
        {
            await WriteSnapshotsAsync(sessionId, activeRulesetVersionId.Value, events, projections, config, happinessByPlayer, ct);
        }

        return Ok(new AnalyticsSessionResponse(sessionId, summary, byPlayer));
    }

    [HttpGet("sessions/{sessionId:guid}/transactions")]
    public async Task<IActionResult> GetTransactions(Guid sessionId, [FromQuery] Guid? playerId = null, CancellationToken ct = default)
    {
        var session = await _sessions.GetSessionAsync(sessionId, ct);
        if (session is null)
        {
            return NotFound(ApiErrorHelper.BuildError(HttpContext, "NOT_FOUND", "Session tidak ditemukan"));
        }

        var projections = await _events.GetCashflowProjectionsAsync(sessionId, ct);
        var items = projections
            .Where(p => !playerId.HasValue || p.PlayerId == playerId.Value)
            .OrderBy(p => p.Timestamp)
            .Select(p => new TransactionHistoryItem(p.Timestamp, p.Direction, p.Amount, p.Category))
            .ToList();

        return Ok(new TransactionHistoryResponse(items));
    }

    [HttpGet("sessions/{sessionId:guid}/players/{playerId:guid}/gameplay")]
    public async Task<IActionResult> GetGameplayMetrics(Guid sessionId, Guid playerId, CancellationToken ct)
    {
        var session = await _sessions.GetSessionAsync(sessionId, ct);
        if (session is null)
        {
            return NotFound(ApiErrorHelper.BuildError(HttpContext, "NOT_FOUND", "Session tidak ditemukan"));
        }

        var snapshots = await _metrics.GetLatestGameplaySnapshotsAsync(sessionId, playerId, ct);
        var rawJson = snapshots.FirstOrDefault(item => item.MetricName == "gameplay.raw.variables")?.MetricValueJson;
        var derivedJson = snapshots.FirstOrDefault(item => item.MetricName == "gameplay.derived.metrics")?.MetricValueJson;
        var computedAt = snapshots.Count == 0 ? (DateTimeOffset?)null : snapshots.Max(item => item.ComputedAt);

        return Ok(new GameplayMetricsResponse(
            sessionId,
            playerId,
            computedAt,
            ParseJsonElement(rawJson),
            ParseJsonElement(derivedJson)));
    }

    private static AnalyticsSessionSummary BuildSummary(List<EventDb> events, List<CashflowProjectionDb> projections, int rulesViolationsCount)
    {
        var cashInTotal = projections.Where(p => p.Direction == "IN").Sum(p => (double)p.Amount);
        var cashOutTotal = projections.Where(p => p.Direction == "OUT").Sum(p => (double)p.Amount);
        var cashflowNetTotal = cashInTotal - cashOutTotal;
        return new AnalyticsSessionSummary(events.Count, cashInTotal, cashOutTotal, cashflowNetTotal, rulesViolationsCount);
    }

    private static JsonElement? ParseJsonElement(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        using var doc = JsonDocument.Parse(json);
        return doc.RootElement.Clone();
    }

    private static List<AnalyticsByPlayerItem> BuildByPlayer(
        List<EventDb> events,
        List<CashflowProjectionDb> projections,
        Dictionary<Guid, HappinessBreakdown> happinessByPlayer)
    {
        var cashTotals = projections
            .GroupBy(p => p.PlayerId)
            .ToDictionary(
                g => g.Key,
                g => new
                {
                    In = g.Where(p => p.Direction == "IN").Sum(p => (double)p.Amount),
                    Out = g.Where(p => p.Direction == "OUT").Sum(p => (double)p.Amount)
                });

        var result = new List<AnalyticsByPlayerItem>();
        var grouped = events.Where(e => e.PlayerId.HasValue)
            .GroupBy(e => e.PlayerId!.Value)
            .ToList();

        foreach (var group in grouped)
        {
            var playerId = group.Key;
            var playerEvents = group.ToList();

            var totals = cashTotals.TryGetValue(playerId, out var t) ? t : new { In = 0d, Out = 0d };
            var donationTotal = playerEvents.Where(e => e.ActionType == "day.friday.donation")
                .Select(e => TryReadAmount(e.Payload, out var amount) ? amount : 0)
                .Sum();

            var goldQty = playerEvents.Where(e => e.ActionType == "day.saturday.gold_trade")
                .Select(e =>
                {
                    if (!TryReadGoldTrade(e.Payload, out var tradeType, out var qty))
                    {
                        return 0;
                    }

                    return string.Equals(tradeType, "BUY", StringComparison.OrdinalIgnoreCase) ? qty : -qty;
                })
                .Sum();

            var happiness = happinessByPlayer.TryGetValue(playerId, out var breakdown)
                ? breakdown
                : ComputeHappinessBreakdown(playerEvents, 0, 0, 0);

            result.Add(new AnalyticsByPlayerItem(
                playerId,
                totals.In,
                totals.Out,
                donationTotal,
                goldQty,
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

        return result;
    }

    private static bool TryReadTransaction(string payloadJson, out string direction, out double amount, out string category)
    {
        direction = string.Empty;
        category = string.Empty;
        amount = 0;

        try
        {
            using var doc = JsonDocument.Parse(payloadJson);
            var root = doc.RootElement;
            if (!root.TryGetProperty("direction", out var directionProp) ||
                !root.TryGetProperty("amount", out var amountProp) ||
                !root.TryGetProperty("category", out var categoryProp))
            {
                return false;
            }

            direction = directionProp.GetString() ?? string.Empty;
            category = categoryProp.GetString() ?? string.Empty;
            amount = amountProp.GetDouble();
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    private static bool TryReadAmount(string payloadJson, out double amount)
    {
        amount = 0;
        try
        {
            using var doc = JsonDocument.Parse(payloadJson);
            if (!doc.RootElement.TryGetProperty("amount", out var amountProp))
            {
                return false;
            }

            amount = amountProp.GetDouble();
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    private static bool TryReadGoldTrade(string payloadJson, out string tradeType, out int qty)
    {
        tradeType = string.Empty;
        qty = 0;
        try
        {
            using var doc = JsonDocument.Parse(payloadJson);
            var root = doc.RootElement;
            if (!root.TryGetProperty("trade_type", out var tradeTypeProp) ||
                !root.TryGetProperty("qty", out var qtyProp))
            {
                return false;
            }

            tradeType = tradeTypeProp.GetString() ?? string.Empty;
            qty = qtyProp.GetInt32();
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    private async Task WriteSnapshotsAsync(
        Guid sessionId,
        Guid rulesetVersionId,
        List<EventDb> events,
        List<CashflowProjectionDb> projections,
        RulesetConfig? config,
        Dictionary<Guid, HappinessBreakdown> happinessByPlayer,
        CancellationToken ct)
    {
        var computedAt = DateTimeOffset.UtcNow;
        var snapshots = new List<MetricSnapshotDb>();

        var sessionMetrics = ComputeSessionMetrics(events, projections, happinessByPlayer);
        var sessionViolations = await _metrics.CountValidationViolationsAsync(sessionId, null, ct);
        sessionMetrics["rules.violations.count"] = (sessionViolations, null);
        snapshots.AddRange(BuildMetricSnapshots(sessionId, null, rulesetVersionId, computedAt, sessionMetrics));

        var players = events.Where(e => e.PlayerId.HasValue).Select(e => e.PlayerId!.Value).Distinct().ToList();
        foreach (var playerId in players)
        {
            var hasHappiness = happinessByPlayer.TryGetValue(playerId, out var breakdown);
            var playerMetrics = await ComputePlayerMetricsAsync(sessionId, playerId, rulesetVersionId, events, projections,
                hasHappiness ? breakdown : null, ct);
            snapshots.AddRange(BuildMetricSnapshots(sessionId, playerId, rulesetVersionId, computedAt, playerMetrics));
        }

        if (snapshots.Count > 0)
        {
            await _metrics.InsertSnapshotsAsync(snapshots, ct);
        }
    }

    private static Dictionary<string, (double? Numeric, string? Json)> ComputeSessionMetrics(
        List<EventDb> events,
        List<CashflowProjectionDb> projections,
        Dictionary<Guid, HappinessBreakdown> happinessByPlayer)
    {
        var metrics = new Dictionary<string, (double? Numeric, string? Json)>();

        var cashIn = projections.Where(p => p.Direction == "IN").Sum(p => (double)p.Amount);
        var cashOut = projections.Where(p => p.Direction == "OUT").Sum(p => (double)p.Amount);
        metrics["cashflow.in.total"] = (cashIn, null);
        metrics["cashflow.out.total"] = (cashOut, null);
        metrics["cashflow.net.total"] = (cashIn - cashOut, null);

        var donationTotal = events.Where(e => e.ActionType == "day.friday.donation")
            .Select(e => TryReadAmount(e.Payload, out var amount) ? amount : 0)
            .Sum();
        metrics["donation.total"] = (donationTotal, null);

        var happinessTotal = happinessByPlayer.Values.Sum(item => item.Total);
        metrics["happiness.points.total"] = (happinessTotal, null);

        return metrics;
    }

    private async Task<Dictionary<string, (double? Numeric, string? Json)>> ComputePlayerMetricsAsync(
        Guid sessionId,
        Guid playerId,
        Guid rulesetVersionId,
        List<EventDb> events,
        List<CashflowProjectionDb> projections,
        HappinessBreakdown? happiness,
        CancellationToken ct)
    {
        var metrics = new Dictionary<string, (double? Numeric, string? Json)>();
        var playerEvents = events.Where(e => e.PlayerId == playerId).ToList();
        var playerProjections = projections.Where(p => p.PlayerId == playerId).ToList();
        RulesetConfig? config = null;
        var rulesetVersion = await _rulesets.GetRulesetVersionByIdAsync(rulesetVersionId, ct);
        if (rulesetVersion is not null &&
            RulesetConfigParser.TryParse(rulesetVersion.ConfigJson, out var parsed, out _))
        {
            config = parsed;
        }

        var cashIn = playerProjections.Where(p => p.Direction == "IN").Sum(p => (double)p.Amount);
        var cashOut = playerProjections.Where(p => p.Direction == "OUT").Sum(p => (double)p.Amount);
        metrics["cashflow.in.total"] = (cashIn, null);
        metrics["cashflow.out.total"] = (cashOut, null);
        metrics["cashflow.net.total"] = (cashIn - cashOut, null);

        var donationTotal = playerEvents.Where(e => e.ActionType == "day.friday.donation")
            .Select(e => TryReadAmount(e.Payload, out var amount) ? amount : 0)
            .Sum();
        metrics["donation.total"] = (donationTotal, null);

        var goldQty = playerEvents.Where(e => e.ActionType == "day.saturday.gold_trade")
            .Select(e =>
            {
                if (!TryReadGoldTrade(e.Payload, out var tradeType, out var qty))
                {
                    return 0;
                }

                return string.Equals(tradeType, "BUY", StringComparison.OrdinalIgnoreCase) ? qty : -qty;
            })
            .Sum();
        metrics["gold.qty.current"] = (goldQty, null);

        var ordersCompleted = playerEvents.Count(e => e.ActionType == "order.claimed");
        metrics["orders.completed.count"] = (ordersCompleted, null);

        var inventory = BuildIngredientInventory(playerEvents);
        metrics["inventory.ingredient.total"] = (inventory.Total, null);

        var actionsUsed = playerEvents.Where(e => e.ActionType == "turn.action.used")
            .Select(e => TryReadActionUsed(e.Payload, out var used, out _) ? used : 0)
            .Sum();
        metrics["actions.used.total"] = (actionsUsed, null);

        var compliance = await ComputePrimaryNeedComplianceAsync(rulesetVersionId, playerEvents, ct);
        metrics["compliance.primary_need.rate"] = (compliance.Rate, compliance.JsonDetail);

        var violations = await _metrics.CountValidationViolationsAsync(sessionId, playerId, ct);
        metrics["rules.violations.count"] = (violations, null);

        var resolvedHappiness = happiness ?? ComputeHappinessBreakdown(
            playerEvents,
            SumRankAwarded(playerEvents, "donation.rank.awarded"),
            SumPointsAwarded(playerEvents, "gold.points.awarded"),
            SumRankAwarded(playerEvents, "pension.rank.awarded"));

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

        var gameplaySnapshots = BuildGameplaySnapshots(playerEvents, playerProjections, events, config, resolvedHappiness);
        metrics["gameplay.raw.variables"] = (null, gameplaySnapshots.RawJson);
        metrics["gameplay.derived.metrics"] = (null, gameplaySnapshots.DerivedJson);

        return metrics;
    }

    private static List<MetricSnapshotDb> BuildMetricSnapshots(
        Guid sessionId,
        Guid? playerId,
        Guid rulesetVersionId,
        DateTimeOffset computedAt,
        Dictionary<string, (double? Numeric, string? Json)> metrics)
    {
        var list = new List<MetricSnapshotDb>();
        foreach (var item in metrics)
        {
            list.Add(new MetricSnapshotDb
            {
                MetricSnapshotId = Guid.NewGuid(),
                SessionId = sessionId,
                PlayerId = playerId,
                ComputedAt = computedAt,
                MetricName = item.Key,
                MetricValueNumeric = item.Value.Numeric,
                MetricValueJson = item.Value.Json,
                RulesetVersionId = rulesetVersionId
            });
        }

        return list;
    }

    private async Task<(double Rate, string? JsonDetail)> ComputePrimaryNeedComplianceAsync(
        Guid rulesetVersionId,
        List<EventDb> playerEvents,
        CancellationToken ct)
    {
        var rulesetVersion = await _rulesets.GetRulesetVersionByIdAsync(rulesetVersionId, ct);
        if (rulesetVersion is null || !RulesetConfigParser.TryParse(rulesetVersion.ConfigJson, out var config, out _))
        {
            return (0, null);
        }

        var days = playerEvents
            .Select(e => e.DayIndex)
            .Distinct()
            .OrderBy(d => d)
            .ToList();

        if (days.Count == 0)
        {
            return (0, null);
        }

        var details = new List<object>();
        var compliantDays = 0;

        foreach (var dayIndex in days)
        {
            var dayEvents = playerEvents.Where(e => e.DayIndex == dayIndex).OrderBy(e => e.SequenceNumber).ToList();
            var primaryCount = dayEvents.Count(e => e.ActionType == "need.primary.purchased");
            var violationReasons = new List<string>();

            if (primaryCount > config!.PrimaryNeedMaxPerDay)
            {
                violationReasons.Add("PRIMARY_NEED_MAX_EXCEEDED");
            }

            if (config.RequirePrimaryBeforeOthers)
            {
                var primarySeen = false;
                foreach (var evt in dayEvents)
                {
                    if (evt.ActionType == "need.primary.purchased")
                    {
                        primarySeen = true;
                    }

                    if (!primarySeen &&
                        (evt.ActionType == "need.secondary.purchased" || evt.ActionType == "need.tertiary.purchased"))
                    {
                        violationReasons.Add("BOUGHT_OTHER_BEFORE_PRIMARY");
                        break;
                    }
                }
            }

            var compliant = violationReasons.Count == 0;
            if (compliant)
            {
                compliantDays++;
            }

            details.Add(new
            {
                day_index = dayIndex,
                compliant,
                reason = violationReasons
            });
        }

        var rate = (double)compliantDays / days.Count;
        var json = JsonSerializer.Serialize(new
        {
            days = details,
            evaluated_days = days.Count,
            compliant_days = compliantDays
        });

        return (rate, json);
    }

    private static IngredientInventory BuildIngredientInventory(List<EventDb> events)
    {
        var inventory = new IngredientInventory();

        foreach (var evt in events)
        {
            if (evt.ActionType == "ingredient.purchased" &&
                TryReadIngredientPurchase(evt.Payload, out var cardId, out var amount))
            {
                inventory.Total += amount;
                inventory.ByCardId[cardId] = inventory.ByCardId.TryGetValue(cardId, out var qty) ? qty + amount : amount;
            }

            if (evt.ActionType == "order.claimed" &&
                TryReadOrderClaim(evt.Payload, out var requiredCards, out _))
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

    private static Dictionary<Guid, HappinessBreakdown> ComputeHappinessByPlayer(
        List<EventDb> events,
        List<CashflowProjectionDb> projections,
        RulesetConfig? config)
    {
        var playerGroups = events.Where(e => e.PlayerId.HasValue)
            .GroupBy(e => e.PlayerId!.Value)
            .ToDictionary(g => g.Key, g => g.ToList());

        var donationPointsByPlayer = new Dictionary<Guid, double>();
        var goldPointsByPlayer = new Dictionary<Guid, double>();
        var pensionPointsByPlayer = new Dictionary<Guid, double>();

        var hasScoring = config?.Scoring is not null;
        if (hasScoring && config!.Scoring!.DonationRankPoints.Count > 0)
        {
            var tieBreakers = BuildTieBreakerLookup(events);
            donationPointsByPlayer = ComputeDonationPointsFromScoring(events, config.Scoring, tieBreakers);
        }
        else
        {
            foreach (var (playerId, playerEvents) in playerGroups)
            {
                donationPointsByPlayer[playerId] = SumRankAwarded(playerEvents, "donation.rank.awarded");
            }
        }

        if (hasScoring && config!.Scoring!.GoldPointsByQty.Count > 0)
        {
            goldPointsByPlayer = ComputeGoldPointsFromScoring(events, config.Scoring);
        }
        else
        {
            foreach (var (playerId, playerEvents) in playerGroups)
            {
                goldPointsByPlayer[playerId] = SumPointsAwarded(playerEvents, "gold.points.awarded");
            }
        }

        if (hasScoring && config!.Scoring!.PensionRankPoints.Count > 0 && config is not null)
        {
            var tieBreakers = BuildTieBreakerLookup(events);
            pensionPointsByPlayer = ComputePensionPointsFromScoring(events, projections, config, tieBreakers);
        }
        else
        {
            foreach (var (playerId, playerEvents) in playerGroups)
            {
                pensionPointsByPlayer[playerId] = SumRankAwarded(playerEvents, "pension.rank.awarded");
            }
        }

        var result = new Dictionary<Guid, HappinessBreakdown>();
        foreach (var (playerId, playerEvents) in playerGroups)
        {
            donationPointsByPlayer.TryGetValue(playerId, out var donationPoints);
            goldPointsByPlayer.TryGetValue(playerId, out var goldPoints);
            pensionPointsByPlayer.TryGetValue(playerId, out var pensionPoints);

            result[playerId] = ComputeHappinessBreakdown(playerEvents, donationPoints, goldPoints, pensionPoints);
        }

        return result;
    }

    private static Dictionary<Guid, int> BuildTieBreakerLookup(IEnumerable<EventDb> events)
    {
        return events.Where(e => e.PlayerId.HasValue && e.ActionType == "tie_breaker.assigned")
            .OrderBy(e => e.SequenceNumber)
            .GroupBy(e => e.PlayerId!.Value)
            .ToDictionary(
                g => g.Key,
                g =>
                {
                    var last = g.Last();
                    return TryReadTieBreaker(last.Payload, out var number) ? number : 0;
                });
    }

    private static Dictionary<Guid, double> ComputeDonationPointsFromScoring(
        List<EventDb> events,
        RulesetScoringConfig scoring,
        Dictionary<Guid, int> tieBreakers)
    {
        var pointsByRank = scoring.DonationRankPoints.ToDictionary(item => item.Rank, item => item.Points);
        var result = new Dictionary<Guid, double>();

        var fridayGroups = events.Where(e => e.ActionType == "day.friday.donation" && e.PlayerId.HasValue)
            .GroupBy(e => e.DayIndex);

        foreach (var dayGroup in fridayGroups)
        {
            var totals = dayGroup
                .GroupBy(e => e.PlayerId!.Value)
                .Select(g =>
                {
                    var total = g.Sum(e => TryReadAmount(e.Payload, out var amount) ? amount : 0);
                    tieBreakers.TryGetValue(g.Key, out var tieNumber);
                    return new { PlayerId = g.Key, Amount = total, Tie = tieNumber };
                })
                .Where(item => item.Amount > 0)
                .OrderByDescending(item => item.Amount)
                .ThenByDescending(item => item.Tie)
                .ThenBy(item => item.PlayerId)
                .ToList();

            var rank = 1;
            foreach (var item in totals)
            {
                if (pointsByRank.TryGetValue(rank, out var points) && points > 0)
                {
                    result[item.PlayerId] = result.TryGetValue(item.PlayerId, out var existing) ? existing + points : points;
                }

                rank += 1;
            }
        }

        return result;
    }

    private static Dictionary<Guid, double> ComputeGoldPointsFromScoring(
        List<EventDb> events,
        RulesetScoringConfig scoring)
    {
        var table = scoring.GoldPointsByQty
            .OrderBy(item => item.Qty)
            .ToList();

        var goldQtyByPlayer = events.Where(e => e.ActionType == "day.saturday.gold_trade" && e.PlayerId.HasValue)
            .GroupBy(e => e.PlayerId!.Value)
            .ToDictionary(
                g => g.Key,
                g => g.Sum(e =>
                {
                    if (!TryReadGoldTrade(e.Payload, out var tradeType, out var qty))
                    {
                        return 0;
                    }

                    return string.Equals(tradeType, "BUY", StringComparison.OrdinalIgnoreCase) ? qty : -qty;
                }));

        var result = new Dictionary<Guid, double>();
        foreach (var (playerId, qty) in goldQtyByPlayer)
        {
            var points = ResolvePointsByQty(qty, table);
            if (points > 0)
            {
                result[playerId] = points;
            }
        }

        return result;
    }

    private static Dictionary<Guid, double> ComputePensionPointsFromScoring(
        List<EventDb> events,
        List<CashflowProjectionDb> projections,
        RulesetConfig config,
        Dictionary<Guid, int> tieBreakers)
    {
        var pointsByRank = config.Scoring?.PensionRankPoints.ToDictionary(item => item.Rank, item => item.Points)
                           ?? new Dictionary<int, int>();

        var cashByPlayer = projections
            .GroupBy(p => p.PlayerId)
            .ToDictionary(
                g => g.Key,
                g => config.StartingCash + g.Sum(p => p.Direction == "IN" ? p.Amount : -p.Amount));

        var players = events.Where(e => e.PlayerId.HasValue).Select(e => e.PlayerId!.Value).Distinct().ToList();
        var ranking = players.Select(playerId =>
            {
                cashByPlayer.TryGetValue(playerId, out var cash);
                tieBreakers.TryGetValue(playerId, out var tieNumber);
                return new { PlayerId = playerId, Cash = cash, Tie = tieNumber };
            })
            .OrderByDescending(item => item.Cash)
            .ThenByDescending(item => item.Tie)
            .ThenBy(item => item.PlayerId)
            .ToList();

        var result = new Dictionary<Guid, double>();
        var rank = 1;
        foreach (var item in ranking)
        {
            if (pointsByRank.TryGetValue(rank, out var points) && points > 0)
            {
                result[item.PlayerId] = points;
            }

            rank += 1;
        }

        return result;
    }

    private static int ResolvePointsByQty(int qty, IReadOnlyList<QtyPoint> table)
    {
        var bestQty = 0;
        var bestPoints = 0;
        foreach (var entry in table)
        {
            if (entry.Qty <= qty && entry.Qty >= bestQty)
            {
                bestQty = entry.Qty;
                bestPoints = entry.Points;
            }
        }

        return bestPoints;
    }

    private static double SumRankAwarded(IEnumerable<EventDb> events, string actionType)
    {
        return events.Where(e => e.ActionType == actionType)
            .Sum(e => TryReadRankAwarded(e.Payload, out _, out var points) ? points : 0);
    }

    private static double SumPointsAwarded(IEnumerable<EventDb> events, string actionType)
    {
        return events.Where(e => e.ActionType == actionType)
            .Sum(e => TryReadPointsAwarded(e.Payload, out var points) ? points : 0);
    }

    private sealed record HappinessBreakdown(
        double Total,
        double NeedPoints,
        double NeedSetBonusPoints,
        double DonationPoints,
        double GoldPoints,
        double PensionPoints,
        double SavingGoalPointsEffective,
        double MissionPenaltyPoints,
        double LoanPenaltyPoints,
        bool HasUnpaidLoan);

    private sealed record MissionAssignment(
        string MissionId,
        string TargetTertiaryCardId,
        int PenaltyPoints,
        bool RequirePrimary,
        bool RequireSecondary);

    private sealed record LoanState(
        string LoanId,
        int Principal,
        int PenaltyPoints,
        double RepaidAmount);

    private static HappinessBreakdown ComputeHappinessBreakdown(
        List<EventDb> playerEvents,
        double donationPoints,
        double goldPoints,
        double pensionPoints)
    {
        double needPoints = 0;
        var primaryCount = 0;
        var secondaryCount = 0;
        var tertiaryCount = 0;
        var tertiaryCardIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var missions = new List<MissionAssignment>();
        var loans = new Dictionary<string, LoanState>(StringComparer.OrdinalIgnoreCase);
        double savingGoalPoints = 0;

        foreach (var evt in playerEvents)
        {
            if (evt.ActionType == "need.primary.purchased" &&
                TryReadNeedPurchase(evt.Payload, out _, out _, out var needPointsValue))
            {
                primaryCount += 1;
                needPoints += needPointsValue;
            }

            if (evt.ActionType == "need.secondary.purchased" &&
                TryReadNeedPurchase(evt.Payload, out _, out _, out var needPointsValueSecondary))
            {
                secondaryCount += 1;
                needPoints += needPointsValueSecondary;
            }

            if (evt.ActionType == "need.tertiary.purchased" &&
                TryReadNeedPurchase(evt.Payload, out _, out var tertiaryCardId, out var needPointsValueTertiary))
            {
                tertiaryCount += 1;
                needPoints += needPointsValueTertiary;
                if (!string.IsNullOrWhiteSpace(tertiaryCardId))
                {
                    tertiaryCardIds.Add(tertiaryCardId);
                }
            }

            if (evt.ActionType == "mission.assigned" &&
                TryReadMissionAssigned(evt.Payload, out var missionId, out var targetCardId, out var penaltyPoints, out var requirePrimary, out var requireSecondary))
            {
                missions.Add(new MissionAssignment(missionId, targetCardId, penaltyPoints, requirePrimary, requireSecondary));
            }

            if (evt.ActionType == "saving.goal.achieved" && TryReadSavingGoalAchieved(evt.Payload, out var savingPoints))
            {
                savingGoalPoints += savingPoints;
            }

            if (evt.ActionType == "loan.syariah.taken" && TryReadLoanTaken(evt.Payload, out var loanId, out var principal, out var penaltyPointsValue))
            {
                loans[loanId] = new LoanState(loanId, principal, penaltyPointsValue, 0);
            }

            if (evt.ActionType == "loan.syariah.repaid" && TryReadLoanRepay(evt.Payload, out var repayLoanId, out var repayAmount))
            {
                if (loans.TryGetValue(repayLoanId, out var state))
                {
                    loans[repayLoanId] = state with { RepaidAmount = state.RepaidAmount + repayAmount };
                }
            }
        }

        var mixedSets = Math.Min(primaryCount, Math.Min(secondaryCount, tertiaryCount));
        var remainingPrimary = primaryCount - mixedSets;
        var remainingSecondary = secondaryCount - mixedSets;
        var remainingTertiary = tertiaryCount - mixedSets;
        var sameSets = (remainingPrimary / 3) + (remainingSecondary / 3) + (remainingTertiary / 3);
        var needSetBonusPoints = mixedSets * 4 + sameSets * 2;

        var hasPrimary = primaryCount > 0;
        var hasSecondary = secondaryCount > 0;
        var missionPenaltyPoints = 0d;
        foreach (var mission in missions)
        {
            var hasTargetTertiary = string.IsNullOrWhiteSpace(mission.TargetTertiaryCardId) ||
                                    tertiaryCardIds.Contains(mission.TargetTertiaryCardId);
            var requiresPrimary = mission.RequirePrimary;
            var requiresSecondary = mission.RequireSecondary;

            var satisfied = (!requiresPrimary || hasPrimary) &&
                            (!requiresSecondary || hasSecondary) &&
                            hasTargetTertiary;

            if (!satisfied)
            {
                missionPenaltyPoints += mission.PenaltyPoints;
            }
        }

        var loanPenaltyPoints = 0d;
        var hasUnpaidLoan = false;
        foreach (var loan in loans.Values)
        {
            if (loan.RepaidAmount < loan.Principal)
            {
                hasUnpaidLoan = true;
                loanPenaltyPoints += loan.PenaltyPoints;
            }
        }

        var savingGoalPointsEffective = hasUnpaidLoan ? 0 : savingGoalPoints;

        var total = needPoints +
                    needSetBonusPoints +
                    donationPoints +
                    goldPoints +
                    pensionPoints +
                    savingGoalPointsEffective -
                    missionPenaltyPoints -
                    loanPenaltyPoints;

        return new HappinessBreakdown(
            total,
            needPoints,
            needSetBonusPoints,
            donationPoints,
            goldPoints,
            pensionPoints,
            savingGoalPointsEffective,
            missionPenaltyPoints,
            loanPenaltyPoints,
            hasUnpaidLoan);
    }

    private sealed class IngredientInventory
    {
        internal int Total { get; set; }
        internal Dictionary<string, int> ByCardId { get; } = new(StringComparer.OrdinalIgnoreCase);
    }

    private sealed record GameplaySnapshot(string RawJson, string DerivedJson);

    private static GameplaySnapshot BuildGameplaySnapshots(
        List<EventDb> playerEvents,
        List<CashflowProjectionDb> playerProjections,
        List<EventDb> allEvents,
        RulesetConfig? config,
        HappinessBreakdown happiness)
    {
        var notesRaw = new List<string>();
        var notesDerived = new List<string>();

        var startingCoins = config?.StartingCash ?? 0;
        var cashInTotal = playerProjections.Where(p => p.Direction == "IN").Sum(p => (double)p.Amount);
        var cashOutTotal = playerProjections.Where(p => p.Direction == "OUT").Sum(p => (double)p.Amount);
        var coinsNetEndGame = startingCoins + cashInTotal - cashOutTotal;
        var coinsHeldCurrent = coinsNetEndGame;

        var eventById = playerEvents
            .GroupBy(e => e.EventId)
            .ToDictionary(g => g.Key, g => g.First());

        var spentByTurn = new Dictionary<int, double>();
        var earnedByTurn = new Dictionary<int, double>();

        foreach (var projection in playerProjections)
        {
            if (!eventById.TryGetValue(projection.EventId, out var evt))
            {
                continue;
            }

            var turn = evt.TurnNumber;
            if (string.Equals(projection.Direction, "OUT", StringComparison.OrdinalIgnoreCase))
            {
                spentByTurn[turn] = spentByTurn.TryGetValue(turn, out var existing) ? existing + projection.Amount : projection.Amount;
            }
            else if (string.Equals(projection.Direction, "IN", StringComparison.OrdinalIgnoreCase))
            {
                earnedByTurn[turn] = earnedByTurn.TryGetValue(turn, out var existing) ? existing + projection.Amount : projection.Amount;
            }
        }

        var turnNumbers = spentByTurn.Keys.Union(earnedByTurn.Keys).OrderBy(t => t).ToList();
        var coinsSpentPerTurn = spentByTurn
            .OrderBy(k => k.Key)
            .Select(k => new { turn_number = k.Key, amount = k.Value })
            .ToList();
        var coinsEarnedPerTurn = earnedByTurn
            .OrderBy(k => k.Key)
            .Select(k => new { turn_number = k.Key, amount = k.Value })
            .ToList();

        var netIncomePerTurn = new List<object>();
        var coinsProgression = new List<object>();
        var runningCoins = (double)startingCoins;
        foreach (var turn in turnNumbers)
        {
            var spent = spentByTurn.TryGetValue(turn, out var spentAmount) ? spentAmount : 0;
            var earned = earnedByTurn.TryGetValue(turn, out var earnedAmount) ? earnedAmount : 0;
            var net = earned - spent;
            runningCoins += net;
            netIncomePerTurn.Add(new { turn_number = turn, net });
            coinsProgression.Add(new { turn_number = turn, coins = runningCoins });
        }

        var donationByDay = playerEvents
            .Where(e => e.ActionType == "day.friday.donation")
            .GroupBy(e => e.DayIndex)
            .Select(g => new
            {
                day_index = g.Key,
                amount = g.Sum(e => TryReadAmount(e.Payload, out var amount) ? amount : 0)
            })
            .OrderBy(item => item.day_index)
            .ToList();
        var donationTotal = donationByDay.Sum(item => item.amount);

        var donationRanks = playerEvents
            .Where(e => e.ActionType == "donation.rank.awarded")
            .GroupBy(e => e.DayIndex)
            .Select(g =>
            {
                var rank = 0;
                foreach (var evt in g)
                {
                    if (TryReadRankAwarded(evt.Payload, out var awardedRank, out _))
                    {
                        rank = awardedRank;
                        break;
                    }
                }

                return new { day_index = g.Key, rank = rank == 0 ? (int?)null : rank };
            })
            .OrderBy(item => item.day_index)
            .ToList();

        var donationChampionCards = playerEvents.Count(e => e.ActionType == "donation.rank.awarded");

        var savingDepositTotal = playerProjections
            .Where(p => p.Category == "SAVING_DEPOSIT" && p.Direction == "OUT")
            .Sum(p => p.Amount);
        var savingWithdrawTotal = playerProjections
            .Where(p => p.Category == "SAVING_WITHDRAW" && p.Direction == "IN")
            .Sum(p => p.Amount);
        var coinsSaved = savingDepositTotal - savingWithdrawTotal;

        var ingredientPurchaseMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var ingredientsCollected = 0;
        var ingredientPurchaseEvents = playerEvents.Where(e => e.ActionType == "ingredient.purchased").ToList();
        foreach (var evt in ingredientPurchaseEvents)
        {
            if (TryReadIngredientPurchaseDetailed(evt.Payload, out var cardId, out var ingredientName, out var amount))
            {
                ingredientsCollected += amount;
                if (!string.IsNullOrWhiteSpace(cardId) && !string.IsNullOrWhiteSpace(ingredientName))
                {
                    ingredientPurchaseMap[cardId] = ingredientName;
                }
            }
            else if (TryReadIngredientPurchase(evt.Payload, out var fallbackCardId, out var fallbackAmount))
            {
                ingredientsCollected += fallbackAmount;
                if (!string.IsNullOrWhiteSpace(fallbackCardId) && !ingredientPurchaseMap.ContainsKey(fallbackCardId))
                {
                    ingredientPurchaseMap[fallbackCardId] = fallbackCardId;
                }
            }
        }

        var inventory = BuildIngredientInventory(playerEvents);
        var ingredientTypesHeld = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        foreach (var (cardId, qty) in inventory.ByCardId)
        {
            if (qty <= 0)
            {
                continue;
            }

            var name = ingredientPurchaseMap.TryGetValue(cardId, out var ingredientName) ? ingredientName : cardId;
            ingredientTypesHeld[name] = ingredientTypesHeld.TryGetValue(name, out var existing) ? existing + qty : qty;
        }

        var ingredientsUsedTotal = playerEvents
            .Where(e => e.ActionType == "order.claimed")
            .Select(e => TryReadOrderClaim(e.Payload, out var cards, out _) ? cards.Count : 0)
            .Sum();

        var ingredientInvestmentTotal = playerProjections
            .Where(p => p.Category == "INGREDIENT" && p.Direction == "OUT")
            .Sum(p => p.Amount);

        var mealOrderIncomeValues = new List<int>();
        foreach (var evt in playerEvents.Where(e => e.ActionType == "order.claimed"))
        {
            if (TryReadOrderClaim(evt.Payload, out _, out var income))
            {
                mealOrderIncomeValues.Add(income);
            }
        }

        var mealOrdersClaimed = mealOrderIncomeValues.Count;
        var mealOrderIncomeTotal = mealOrderIncomeValues.Sum();
        var maxTurnNumber = playerEvents.Count == 0 ? 0 : playerEvents.Max(e => e.TurnNumber);
        var mealOrdersPerTurnAverage = maxTurnNumber > 0 ? (double)mealOrdersClaimed / maxTurnNumber : 0;

        var primaryNeeds = 0;
        var secondaryNeeds = 0;
        var tertiaryNeeds = 0;
        var tertiaryCardIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var missions = new List<MissionAssignment>();
        foreach (var evt in playerEvents)
        {
            if (evt.ActionType == "need.primary.purchased" &&
                TryReadNeedPurchase(evt.Payload, out _, out _, out _))
            {
                primaryNeeds += 1;
            }

            if (evt.ActionType == "need.secondary.purchased" &&
                TryReadNeedPurchase(evt.Payload, out _, out _, out _))
            {
                secondaryNeeds += 1;
            }

            if (evt.ActionType == "need.tertiary.purchased" &&
                TryReadNeedPurchase(evt.Payload, out _, out var cardId, out _))
            {
                tertiaryNeeds += 1;
                if (!string.IsNullOrWhiteSpace(cardId))
                {
                    tertiaryCardIds.Add(cardId);
                }
            }

            if (evt.ActionType == "mission.assigned" &&
                TryReadMissionAssigned(evt.Payload, out var missionId, out var targetCardId, out var penaltyPoints, out var requirePrimary, out var requireSecondary))
            {
                missions.Add(new MissionAssignment(missionId, targetCardId, penaltyPoints, requirePrimary, requireSecondary));
            }
        }

        var needCardsPurchased = primaryNeeds + secondaryNeeds + tertiaryNeeds;
        var needCoinsSpent = playerProjections
            .Where(p => p.Direction == "OUT" &&
                        (p.Category == "NEED_PRIMARY" || p.Category == "NEED_SECONDARY" || p.Category == "NEED_TERTIARY"))
            .Sum(p => p.Amount);

        bool? specificTertiaryAcquired = null;
        bool? collectionMissionComplete = null;
        if (missions.Count > 0)
        {
            specificTertiaryAcquired = missions.Any(m => !string.IsNullOrWhiteSpace(m.TargetTertiaryCardId) &&
                                                        tertiaryCardIds.Contains(m.TargetTertiaryCardId));

            var hasPrimary = primaryNeeds > 0;
            var hasSecondary = secondaryNeeds > 0;
            collectionMissionComplete = missions.All(m =>
            {
                var hasTarget = string.IsNullOrWhiteSpace(m.TargetTertiaryCardId) ||
                                tertiaryCardIds.Contains(m.TargetTertiaryCardId);
                var requirePrimary = !m.RequirePrimary || hasPrimary;
                var requireSecondary = !m.RequireSecondary || hasSecondary;
                return hasTarget && requirePrimary && requireSecondary;
            });
        }

        var goldBuyQty = 0;
        var goldSellQty = 0;
        var goldPurchasePrices = new List<int>();
        var goldSalePrices = new List<int>();
        var goldInvestmentSpent = 0;
        var goldInvestmentEarned = 0;

        foreach (var evt in playerEvents.Where(e => e.ActionType == "day.saturday.gold_trade"))
        {
            if (!TryReadGoldTradeDetailed(evt.Payload, out var tradeType, out var qty, out var unitPrice, out var amount))
            {
                continue;
            }

            if (string.Equals(tradeType, "BUY", StringComparison.OrdinalIgnoreCase))
            {
                goldBuyQty += qty;
                goldInvestmentSpent += amount;
                if (unitPrice > 0)
                {
                    goldPurchasePrices.Add(unitPrice);
                }
            }
            else if (string.Equals(tradeType, "SELL", StringComparison.OrdinalIgnoreCase))
            {
                goldSellQty += qty;
                goldInvestmentEarned += amount;
                if (unitPrice > 0)
                {
                    goldSalePrices.Add(unitPrice);
                }
            }
        }

        var goldHeldEnd = goldBuyQty - goldSellQty;
        var goldInvestmentNet = goldInvestmentEarned - goldInvestmentSpent;

        var pensionRank = playerEvents
            .Where(e => e.ActionType == "pension.rank.awarded")
            .Select(e => TryReadRankAwarded(e.Payload, out var rank, out _) ? rank : 0)
            .FirstOrDefault(rank => rank > 0);

        var riskEvents = playerEvents.Where(e => e.ActionType == "risk.life.drawn").ToList();
        var riskCostsPerCard = new List<int>();
        foreach (var riskEvent in riskEvents)
        {
            var cost = playerProjections
                .Where(p => p.EventId == riskEvent.EventId && p.Category == "RISK_LIFE" && p.Direction == "OUT")
                .Sum(p => p.Amount);
            if (cost > 0)
            {
                riskCostsPerCard.Add(cost);
            }
        }

        var riskCostsTotal = riskCostsPerCard.Sum();
        var riskCardsDrawn = riskEvents.Count;
        var riskMitigated = playerEvents.Count(e => e.ActionType == "insurance.multirisk.used");
        var insurancePayments = playerProjections
            .Where(p => p.Category == "INSURANCE_PREMIUM" && p.Direction == "OUT")
            .Sum(p => p.Amount);

        var savingDepositsByGoal = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        foreach (var evt in playerEvents.Where(e => e.ActionType == "saving.deposit.created"))
        {
            if (TryReadSavingDeposit(evt.Payload, out var goalId, out var amount))
            {
                savingDepositsByGoal[goalId] = savingDepositsByGoal.TryGetValue(goalId, out var existing) ? existing + amount : amount;
            }
        }

        var financialGoalsAttempted = savingDepositsByGoal.Count;
        var financialGoalsCompleted = playerEvents.Count(e => e.ActionType == "saving.goal.achieved");

        var loanStates = new Dictionary<string, LoanState>(StringComparer.OrdinalIgnoreCase);
        foreach (var evt in playerEvents)
        {
            if (evt.ActionType == "loan.syariah.taken" &&
                TryReadLoanTaken(evt.Payload, out var loanId, out var principal, out var penaltyPoints))
            {
                loanStates[loanId] = new LoanState(loanId, principal, penaltyPoints, 0);
            }

            if (evt.ActionType == "loan.syariah.repaid" &&
                TryReadLoanRepay(evt.Payload, out var repayLoanId, out var repayAmount))
            {
                if (loanStates.TryGetValue(repayLoanId, out var state))
                {
                    loanStates[repayLoanId] = state with { RepaidAmount = state.RepaidAmount + repayAmount };
                }
            }
        }

        var loansTaken = loanStates.Count;
        var loansRepaid = loanStates.Values.Count(l => l.RepaidAmount >= l.Principal);
        var loansUnpaid = loanStates.Values.Count(l => l.RepaidAmount < l.Principal);
        var loansOutstandingAmount = loanStates.Values.Sum(l => Math.Max(0, l.Principal - l.RepaidAmount));

        var actionEvents = playerEvents
            .Where(e => IsActionEvent(e.ActionType))
            .OrderBy(e => e.SequenceNumber)
            .ToList();

        var actionSequences = actionEvents
            .GroupBy(e => e.TurnNumber)
            .OrderBy(g => g.Key)
            .Select(g => new
            {
                turn_number = g.Key,
                actions = g.Select(e => e.ActionType).ToList()
            })
            .ToList();

        var actionRepetitions = actionEvents
            .GroupBy(e => e.TurnNumber)
            .OrderBy(g => g.Key)
            .Select(g =>
            {
                var distinctActions = g.Select(e => e.ActionType).Distinct(StringComparer.OrdinalIgnoreCase).Count();
                var totalActions = g.Count();
                var repeatedActions = Math.Max(0, totalActions - distinctActions);
                var diversityBase = config?.ActionsPerTurn ?? 2;
                var diversityScore = diversityBase > 0 ? (double)distinctActions / diversityBase : 0;
                return new
                {
                    turn_number = g.Key,
                    total_actions = totalActions,
                    distinct_actions = distinctActions,
                    repeated_actions = repeatedActions,
                    diversity_score = diversityScore
                };
            })
            .ToList();

        var actionTurns = actionRepetitions.Select(item => item.turn_number).ToHashSet();
        var actionsSkipped = maxTurnNumber > 0 ? Math.Max(0, maxTurnNumber - actionTurns.Count) : 0;

        var raw = new
        {
            coins = new
            {
                starting_coins = startingCoins,
                coins_held_current = coinsHeldCurrent,
                coins_spent_per_turn = coinsSpentPerTurn,
                coins_earned_per_turn = coinsEarnedPerTurn,
                coins_donated = donationTotal,
                coins_saved = coinsSaved,
                coins_net_end_game = coinsNetEndGame
            },
            ingredients = new
            {
                ingredients_collected = ingredientsCollected,
                ingredients_held_current = inventory.Total,
                ingredient_types_held = ingredientTypesHeld,
                ingredients_used_per_meal = ingredientsUsedTotal,
                ingredients_wasted = (int?)null,
                ingredient_investment_coins_total = ingredientInvestmentTotal
            },
            meal_orders = new
            {
                meal_orders_claimed = mealOrdersClaimed,
                meal_orders_available_passed = (int?)null,
                meal_order_income_per_order = mealOrderIncomeValues,
                meal_order_income_total = mealOrderIncomeTotal,
                meal_orders_per_turn_average = mealOrdersPerTurnAverage
            },
            needs = new
            {
                need_cards_purchased = needCardsPurchased,
                primary_needs_owned = primaryNeeds,
                secondary_needs_owned = secondaryNeeds,
                tertiary_needs_owned = tertiaryNeeds,
                specific_tertiary_need = specificTertiaryAcquired,
                collection_mission_complete = collectionMissionComplete,
                need_cards_coins_spent = needCoinsSpent
            },
            donations = new
            {
                donation_amount_per_friday = donationByDay,
                donation_rank_per_friday = donationRanks,
                donation_total_coins = donationTotal,
                donation_champion_cards_earned = donationChampionCards,
                donation_happiness_points = happiness.DonationPoints
            },
            gold = new
            {
                gold_cards_purchased = goldBuyQty,
                gold_cards_sold = goldSellQty,
                gold_cards_held_end = goldHeldEnd,
                gold_prices_per_purchase = goldPurchasePrices,
                gold_price_per_sale = goldSalePrices,
                gold_investment_coins_spent = goldInvestmentSpent,
                gold_investment_coins_earned = goldInvestmentEarned,
                gold_investment_net = goldInvestmentNet
            },
            pension = new
            {
                leftover_coins_end_game = coinsHeldCurrent,
                ingredient_cards_value_end = inventory.Total,
                coins_in_savings_goal = coinsSaved,
                pension_fund_total = coinsHeldCurrent + inventory.Total + coinsSaved,
                pension_fund_rank_per_game = pensionRank == 0 ? (int?)null : pensionRank,
                pension_fund_happiness_points = happiness.PensionPoints
            },
            life_risk = new
            {
                life_risk_cards_drawn = riskCardsDrawn,
                life_risk_costs_per_card = riskCostsPerCard,
                life_risk_costs_total = riskCostsTotal,
                life_risk_mitigated_with_insurance = riskMitigated,
                insurance_payments_made = insurancePayments,
                emergency_options_used = (int?)null
            },
            financial_goals = new
            {
                financial_goals_attempted = financialGoalsAttempted,
                financial_goals_completed = financialGoalsCompleted,
                financial_goals_coins_per_goal = savingDepositsByGoal,
                financial_goals_coins_total_invested = savingDepositsByGoal.Values.Sum(),
                financial_goals_incomplete_coins_wasted = (int?)null,
                sharia_loan_cards_taken = loansTaken,
                sharia_loans_repaid = loansRepaid,
                sharia_loans_unpaid_end = loansUnpaid,
                loan_penalty_if_unpaid = happiness.LoanPenaltyPoints
            },
            actions = new
            {
                actions_per_turn = config?.ActionsPerTurn ?? 2,
                action_repetitions_per_turn = actionRepetitions,
                action_sequence = actionSequences,
                actions_skipped = actionsSkipped
            },
            turns = new
            {
                coins_per_turn_progression = coinsProgression,
                net_income_per_turn = netIncomePerTurn,
                turn_number_when_debt_introduced = playerEvents
                    .Where(e => e.ActionType == "loan.syariah.taken")
                    .Select(e => (int?)e.TurnNumber)
                    .OrderBy(t => t)
                    .FirstOrDefault(),
                turn_number_when_first_risk_hit = playerEvents
                    .Where(e => e.ActionType == "risk.life.drawn")
                    .Select(e => (int?)e.TurnNumber)
                    .OrderBy(t => t)
                    .FirstOrDefault(),
                turn_number_game_completion = maxTurnNumber == 0 ? (int?)null : maxTurnNumber
            },
            notes = notesRaw
        };

        notesRaw.Add("meal_orders_available_passed_not_tracked");
        notesRaw.Add("ingredients_wasted_requires_discard_event");
        notesRaw.Add("emergency_options_used_requires_sell_event");

        var totalIncome = cashInTotal;
        var totalExpenses = cashOutTotal;
        var businessEfficiencyRatio = SafeRatio(mealOrderIncomeTotal, ingredientInvestmentTotal);
        var goldRoiPercentage = SafeRatio(goldInvestmentNet, goldInvestmentSpent, true);
        var riskExposurePercentage = SafeRatio(riskCostsTotal, totalIncome, true);
        var riskMitigationEffectiveness = SafeRatio(riskMitigated, riskCardsDrawn, true);

        var netWorthIndex = SafeRatio(coinsNetEndGame, startingCoins, true);
        var incomeDiversification = SafeRatio(
            playerEvents.Where(e => e.ActionType == "work.freelance.completed")
                .Select(e => TryReadAmount(e.Payload, out var amount) ? amount : 0)
                .Sum()
            + mealOrderIncomeTotal
            + goldInvestmentEarned,
            totalIncome,
            true);

        if (incomeDiversification is null)
        {
            notesDerived.Add("income_diversification_requires_income");
        }

        var expenseEfficiency = SafeRatio(ingredientInvestmentTotal, totalExpenses, true);
        var businessProfitMargin = SafeRatio(mealOrderIncomeTotal - ingredientInvestmentTotal, mealOrderIncomeTotal, true);

        var averageRiskCost = riskCardsDrawn > 0 ? (double)riskCostsTotal / riskCardsDrawn : 0;
        var insuranceActivationRate = riskCardsDrawn > 0 ? (double)riskMitigated / riskCardsDrawn : 0;
        var riskAppetiteScore = riskCardsDrawn > 0 ? averageRiskCost * insuranceActivationRate : (double?)null;
        if (riskCardsDrawn == 0)
        {
            notesDerived.Add("risk_appetite_requires_risk_events");
        }

        var debtLeverageRatio = SafeRatio(loansOutstandingAmount, coinsNetEndGame, true);
        var loanRepaymentDiscipline = SafeRatio(loansRepaid, loansTaken, true);
        var debtRatio = SafeRatio(loansUnpaid, loansTaken);
        var goalSettingAmbition = coinsNetEndGame > 0
            ? (financialGoalsAttempted + (savingDepositsByGoal.Values.Sum() / coinsNetEndGame)) * 100
            : (double?)null;

        var incomeActionEventIds = playerProjections
            .Where(p => p.Direction == "IN")
            .Select(p => p.EventId)
            .ToHashSet();
        var incomeActions = actionEvents.Count(e => incomeActionEventIds.Contains(e.EventId));
        var actionEfficiency = SafeRatio(incomeActions, actionEvents.Count);
        var actionDiversityAverage = actionRepetitions.Count > 0
            ? actionRepetitions.Average(item => item.diversity_score)
            : 0;

        var mealOrderSuccessRate = (double?)null;
        notesDerived.Add("meal_orders_available_passed_not_tracked");

        var planningHorizon = SafeRatio(
            savingDepositsByGoal.Values.Sum() + financialGoalsAttempted + playerEvents.Count(e => e.ActionType == "insurance.multirisk.purchased"),
            actionEvents.Count);

        var totalNeeds = needCardsPurchased;
        var fulfillmentDiversity = totalNeeds > 0
            ? Math.Sqrt(primaryNeeds * primaryNeeds + secondaryNeeds * secondaryNeeds + tertiaryNeeds * tertiaryNeeds) / totalNeeds
            : (double?)null;

        var missionAchievement = collectionMissionComplete.HasValue
            ? (collectionMissionComplete.Value ? 1 : 0)
            : (int?)null;

        var donationAmounts = donationByDay.Select(d => (double)d.amount).ToList();
        var donationStability = donationAmounts.Count > 0 ? 100 - StdDev(donationAmounts) : (double?)null;
        var donationRatio = SafeRatio(donationTotal, coinsNetEndGame);
        var totalFridays = allEvents.Where(e => string.Equals(e.Weekday, "FRI", StringComparison.OrdinalIgnoreCase))
            .Select(e => e.DayIndex)
            .Distinct()
            .Count();
        var fridayParticipationRate = totalFridays > 0 ? (double)donationByDay.Count / totalFridays : (double?)null;
        var donationCommitmentScore = donationStability.HasValue && donationRatio.HasValue && fridayParticipationRate.HasValue
            ? donationStability.Value * donationRatio.Value * fridayParticipationRate.Value
            : (double?)null;

        var derived = new
        {
            net_worth_index = netWorthIndex,
            income_diversification_ratio = incomeDiversification,
            expense_management_efficiency = expenseEfficiency,
            business_profit_margin = businessProfitMargin,
            business_efficiency_ratio = businessEfficiencyRatio,
            gold_roi_percentage = goldRoiPercentage,
            risk_exposure_percentage = riskExposurePercentage,
            risk_mitigation_effectiveness = riskMitigationEffectiveness,
            risk_appetite_score = riskAppetiteScore,
            debt_leverage_ratio = debtLeverageRatio,
            loan_repayment_discipline = loanRepaymentDiscipline,
            debt_ratio = debtRatio,
            goal_setting_ambition = goalSettingAmbition,
            action_efficiency = actionEfficiency,
            action_diversity_score_avg = actionDiversityAverage,
            meal_order_success_rate = mealOrderSuccessRate,
            planning_horizon = planningHorizon,
            fulfillment_diversity = fulfillmentDiversity,
            mission_achievement = missionAchievement,
            donation_commitment_score = donationCommitmentScore,
            happiness_portfolio = new
            {
                need_cards_pts = happiness.NeedPoints,
                donations_pts = happiness.DonationPoints,
                gold_pts = happiness.GoldPoints,
                pension_pts = happiness.PensionPoints,
                financial_goals_pts = happiness.SavingGoalPointsEffective,
                mission_bonus_pts = 0 - happiness.MissionPenaltyPoints
            },
            notes = notesDerived
        };

        var rawJson = JsonSerializer.Serialize(raw);
        var derivedJson = JsonSerializer.Serialize(derived);
        return new GameplaySnapshot(rawJson, derivedJson);
    }

    private static double? SafeRatio(double numerator, double denominator, bool percent = false)
    {
        if (Math.Abs(denominator) < 0.000001)
        {
            return null;
        }

        var value = numerator / denominator;
        return percent ? value * 100 : value;
    }

    private static double StdDev(IReadOnlyList<double> values)
    {
        if (values.Count == 0)
        {
            return 0;
        }

        var mean = values.Average();
        var variance = values.Sum(v => Math.Pow(v - mean, 2)) / values.Count;
        return Math.Sqrt(variance);
    }

    private static bool IsActionEvent(string actionType)
    {
        if (string.IsNullOrWhiteSpace(actionType))
        {
            return false;
        }

        return !actionType.Equals("turn.action.used", StringComparison.OrdinalIgnoreCase) &&
               !actionType.EndsWith(".awarded", StringComparison.OrdinalIgnoreCase) &&
               !actionType.Equals("tie_breaker.assigned", StringComparison.OrdinalIgnoreCase) &&
               !actionType.Equals("mission.assigned", StringComparison.OrdinalIgnoreCase);
    }

    private static bool TryReadActionUsed(string payloadJson, out int used, out int remaining)
    {
        used = 0;
        remaining = 0;
        try
        {
            using var doc = JsonDocument.Parse(payloadJson);
            if (!doc.RootElement.TryGetProperty("used", out var usedProp) ||
                !doc.RootElement.TryGetProperty("remaining", out var remainingProp))
            {
                return false;
            }

            used = usedProp.GetInt32();
            remaining = remainingProp.GetInt32();
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    private static bool TryReadGoldTradeDetailed(
        string payloadJson,
        out string tradeType,
        out int qty,
        out int unitPrice,
        out int amount)
    {
        tradeType = string.Empty;
        qty = 0;
        unitPrice = 0;
        amount = 0;

        try
        {
            using var doc = JsonDocument.Parse(payloadJson);
            var root = doc.RootElement;
            if (!root.TryGetProperty("trade_type", out var tradeTypeProp) ||
                !root.TryGetProperty("qty", out var qtyProp) ||
                !root.TryGetProperty("unit_price", out var unitPriceProp) ||
                !root.TryGetProperty("amount", out var amountProp))
            {
                return false;
            }

            tradeType = tradeTypeProp.GetString() ?? string.Empty;
            qty = qtyProp.GetInt32();
            unitPrice = unitPriceProp.GetInt32();
            amount = amountProp.GetInt32();
            return qty > 0;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    private static bool TryReadIngredientPurchaseDetailed(
        string payloadJson,
        out string cardId,
        out string ingredientName,
        out int amount)
    {
        cardId = string.Empty;
        ingredientName = string.Empty;
        amount = 0;

        try
        {
            using var doc = JsonDocument.Parse(payloadJson);
            var root = doc.RootElement;
            if (!root.TryGetProperty("card_id", out var cardIdProp) ||
                !root.TryGetProperty("ingredient_name", out var nameProp) ||
                !root.TryGetProperty("amount", out var amountProp))
            {
                return false;
            }

            cardId = cardIdProp.GetString() ?? string.Empty;
            ingredientName = nameProp.GetString() ?? string.Empty;
            amount = amountProp.GetInt32();
            return !string.IsNullOrWhiteSpace(cardId);
        }
        catch (JsonException)
        {
            return false;
        }
    }

    private static bool TryReadSavingDeposit(string payloadJson, out string goalId, out int amount)
    {
        goalId = string.Empty;
        amount = 0;

        try
        {
            using var doc = JsonDocument.Parse(payloadJson);
            var root = doc.RootElement;
            if (!root.TryGetProperty("goal_id", out var goalProp) ||
                !root.TryGetProperty("amount", out var amountProp))
            {
                return false;
            }

            goalId = goalProp.GetString() ?? string.Empty;
            amount = amountProp.GetInt32();
            return !string.IsNullOrWhiteSpace(goalId);
        }
        catch (JsonException)
        {
            return false;
        }
    }

    private static bool TryReadIngredientPurchase(string payloadJson, out string cardId, out int amount)
    {
        cardId = string.Empty;
        amount = 0;
        try
        {
            using var doc = JsonDocument.Parse(payloadJson);
            if (!doc.RootElement.TryGetProperty("card_id", out var cardIdProp) ||
                !doc.RootElement.TryGetProperty("amount", out var amountProp))
            {
                return false;
            }

            cardId = cardIdProp.GetString() ?? string.Empty;
            amount = amountProp.GetInt32();
            return !string.IsNullOrWhiteSpace(cardId);
        }
        catch (JsonException)
        {
            return false;
        }
    }

    private static bool TryReadNeedPurchase(string payloadJson, out int amount, out string cardId, out int points)
    {
        amount = 0;
        cardId = string.Empty;
        points = 0;
        try
        {
            using var doc = JsonDocument.Parse(payloadJson);
            if (!doc.RootElement.TryGetProperty("amount", out var amountProp))
            {
                return false;
            }

            amount = amountProp.GetInt32();
            if (doc.RootElement.TryGetProperty("card_id", out var cardIdProp))
            {
                cardId = cardIdProp.GetString() ?? string.Empty;
            }

            if (doc.RootElement.TryGetProperty("points", out var pointsProp))
            {
                points = pointsProp.GetInt32();
            }

            return amount > 0;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    private static bool TryReadMissionAssigned(
        string payloadJson,
        out string missionId,
        out string targetTertiaryCardId,
        out int penaltyPoints,
        out bool requirePrimary,
        out bool requireSecondary)
    {
        missionId = string.Empty;
        targetTertiaryCardId = string.Empty;
        penaltyPoints = 0;
        requirePrimary = true;
        requireSecondary = true;

        try
        {
            using var doc = JsonDocument.Parse(payloadJson);
            var root = doc.RootElement;
            if (!root.TryGetProperty("mission_id", out var missionIdProp) ||
                !root.TryGetProperty("target_tertiary_card_id", out var targetProp) ||
                !root.TryGetProperty("penalty_points", out var penaltyProp))
            {
                return false;
            }

            missionId = missionIdProp.GetString() ?? string.Empty;
            targetTertiaryCardId = targetProp.GetString() ?? string.Empty;
            penaltyPoints = penaltyProp.GetInt32();

            if (root.TryGetProperty("require_primary", out var requirePrimaryProp))
            {
                requirePrimary = requirePrimaryProp.GetBoolean();
            }

            if (root.TryGetProperty("require_secondary", out var requireSecondaryProp))
            {
                requireSecondary = requireSecondaryProp.GetBoolean();
            }

            return !string.IsNullOrWhiteSpace(missionId);
        }
        catch (JsonException)
        {
            return false;
        }
    }

    private static bool TryReadTieBreaker(string payloadJson, out int number)
    {
        number = 0;
        try
        {
            using var doc = JsonDocument.Parse(payloadJson);
            if (!doc.RootElement.TryGetProperty("number", out var numberProp))
            {
                return false;
            }

            number = numberProp.GetInt32();
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    private static bool TryReadRankAwarded(string payloadJson, out int rank, out int points)
    {
        rank = 0;
        points = 0;
        try
        {
            using var doc = JsonDocument.Parse(payloadJson);
            if (!doc.RootElement.TryGetProperty("rank", out var rankProp) ||
                !doc.RootElement.TryGetProperty("points", out var pointsProp))
            {
                return false;
            }

            rank = rankProp.GetInt32();
            points = pointsProp.GetInt32();
            return rank > 0;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    private static bool TryReadPointsAwarded(string payloadJson, out int points)
    {
        points = 0;
        try
        {
            using var doc = JsonDocument.Parse(payloadJson);
            if (!doc.RootElement.TryGetProperty("points", out var pointsProp))
            {
                return false;
            }

            points = pointsProp.GetInt32();
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    private static bool TryReadSavingGoalAchieved(string payloadJson, out int points)
    {
        points = 0;
        try
        {
            using var doc = JsonDocument.Parse(payloadJson);
            if (!doc.RootElement.TryGetProperty("points", out var pointsProp))
            {
                return false;
            }

            points = pointsProp.GetInt32();
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    private static bool TryReadLoanTaken(string payloadJson, out string loanId, out int principal, out int penaltyPoints)
    {
        loanId = string.Empty;
        principal = 0;
        penaltyPoints = 0;
        try
        {
            using var doc = JsonDocument.Parse(payloadJson);
            if (!doc.RootElement.TryGetProperty("loan_id", out var loanIdProp) ||
                !doc.RootElement.TryGetProperty("principal", out var principalProp) ||
                !doc.RootElement.TryGetProperty("penalty_points", out var penaltyProp))
            {
                return false;
            }

            loanId = loanIdProp.GetString() ?? string.Empty;
            principal = principalProp.GetInt32();
            penaltyPoints = penaltyProp.GetInt32();
            return !string.IsNullOrWhiteSpace(loanId);
        }
        catch (JsonException)
        {
            return false;
        }
    }

    private static bool TryReadLoanRepay(string payloadJson, out string loanId, out int amount)
    {
        loanId = string.Empty;
        amount = 0;
        try
        {
            using var doc = JsonDocument.Parse(payloadJson);
            if (!doc.RootElement.TryGetProperty("loan_id", out var loanIdProp) ||
                !doc.RootElement.TryGetProperty("amount", out var amountProp))
            {
                return false;
            }

            loanId = loanIdProp.GetString() ?? string.Empty;
            amount = amountProp.GetInt32();
            return !string.IsNullOrWhiteSpace(loanId);
        }
        catch (JsonException)
        {
            return false;
        }
    }

    private static bool TryReadOrderClaim(string payloadJson, out List<string> requiredCards, out int income)
    {
        requiredCards = new List<string>();
        income = 0;
        try
        {
            using var doc = JsonDocument.Parse(payloadJson);
            if (!doc.RootElement.TryGetProperty("required_ingredient_card_ids", out var cardsProp) ||
                cardsProp.ValueKind != JsonValueKind.Array ||
                !doc.RootElement.TryGetProperty("income", out var incomeProp))
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
        catch (JsonException)
        {
            return false;
        }
    }
}
