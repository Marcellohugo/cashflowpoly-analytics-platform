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

    private static AnalyticsSessionSummary BuildSummary(List<EventDb> events, List<CashflowProjectionDb> projections, int rulesViolationsCount)
    {
        var cashInTotal = projections.Where(p => p.Direction == "IN").Sum(p => (double)p.Amount);
        var cashOutTotal = projections.Where(p => p.Direction == "OUT").Sum(p => (double)p.Amount);
        var cashflowNetTotal = cashInTotal - cashOutTotal;
        return new AnalyticsSessionSummary(events.Count, cashInTotal, cashOutTotal, cashflowNetTotal, rulesViolationsCount);
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
