using System.Text.Json;
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

        var summary = BuildSummary(events, projections);
        var byPlayer = BuildByPlayer(events, projections);

        var activeRulesetVersionId = await _sessions.GetActiveRulesetVersionIdAsync(sessionId, ct);
        if (activeRulesetVersionId.HasValue)
        {
            await WriteSnapshotsAsync(sessionId, activeRulesetVersionId.Value, events, projections, ct);
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

        var events = await _events.GetAllEventsBySessionAsync(sessionId, ct);

        var items = new List<TransactionHistoryItem>();

        foreach (var evt in events.Where(e => e.ActionType == "transaction.recorded"))
        {
            if (playerId.HasValue && evt.PlayerId != playerId.Value)
            {
                continue;
            }

            if (!TryReadTransaction(evt.Payload, out var direction, out var amount, out var category))
            {
                continue;
            }

            items.Add(new TransactionHistoryItem(evt.Timestamp, direction, amount, category));
        }

        return Ok(new TransactionHistoryResponse(items));
    }

    private static AnalyticsSessionSummary BuildSummary(List<EventDb> events, List<CashflowProjectionDb> projections)
    {
        var cashInTotal = projections.Where(p => p.Direction == "IN").Sum(p => (double)p.Amount);
        var cashOutTotal = projections.Where(p => p.Direction == "OUT").Sum(p => (double)p.Amount);
        return new AnalyticsSessionSummary(events.Count, cashInTotal, cashOutTotal);
    }

    private static List<AnalyticsByPlayerItem> BuildByPlayer(List<EventDb> events, List<CashflowProjectionDb> projections)
    {
        var byPlayer = new Dictionary<Guid, AnalyticsByPlayerItem>();
        var cashTotals = projections
            .GroupBy(p => p.PlayerId)
            .ToDictionary(
                g => g.Key,
                g => new
                {
                    In = g.Where(p => p.Direction == "IN").Sum(p => (double)p.Amount),
                    Out = g.Where(p => p.Direction == "OUT").Sum(p => (double)p.Amount)
                });

        foreach (var evt in events)
        {
            if (evt.PlayerId is null)
            {
                continue;
            }

            var playerId = evt.PlayerId.Value;
            if (!byPlayer.TryGetValue(playerId, out var item))
            {
                var totals = cashTotals.TryGetValue(playerId, out var t) ? t : new { In = 0d, Out = 0d };
                item = new AnalyticsByPlayerItem(playerId, totals.In, totals.Out, 0, 0);
            }

            if (evt.ActionType == "day.friday.donation" && TryReadAmount(evt.Payload, out var donationAmount))
            {
                item = item with { DonationTotal = item.DonationTotal + donationAmount };
            }

            if (evt.ActionType == "day.saturday.gold_trade" &&
                TryReadGoldTrade(evt.Payload, out var tradeType, out var qty))
            {
                item = item with
                {
                    GoldQty = item.GoldQty + (string.Equals(tradeType, "BUY", StringComparison.OrdinalIgnoreCase) ? qty : -qty)
                };
            }

            byPlayer[playerId] = item;
        }

        return byPlayer.Values.ToList();
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
        CancellationToken ct)
    {
        var computedAt = DateTimeOffset.UtcNow;
        var snapshots = new List<MetricSnapshotDb>();

        var sessionMetrics = ComputeSessionMetrics(events, projections);
        var sessionViolations = await _metrics.CountValidationViolationsAsync(sessionId, null, ct);
        sessionMetrics["rules.violations.count"] = (sessionViolations, null);
        snapshots.AddRange(BuildMetricSnapshots(sessionId, null, rulesetVersionId, computedAt, sessionMetrics));

        var players = events.Where(e => e.PlayerId.HasValue).Select(e => e.PlayerId!.Value).Distinct().ToList();
        foreach (var playerId in players)
        {
            var playerMetrics = await ComputePlayerMetricsAsync(sessionId, playerId, rulesetVersionId, events, projections, ct);
            snapshots.AddRange(BuildMetricSnapshots(sessionId, playerId, rulesetVersionId, computedAt, playerMetrics));
        }

        if (snapshots.Count > 0)
        {
            await _metrics.InsertSnapshotsAsync(snapshots, ct);
        }
    }

    private static Dictionary<string, (double? Numeric, string? Json)> ComputeSessionMetrics(
        List<EventDb> events,
        List<CashflowProjectionDb> projections)
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

        return metrics;
    }

    private async Task<Dictionary<string, (double? Numeric, string? Json)>> ComputePlayerMetricsAsync(
        Guid sessionId,
        Guid playerId,
        Guid rulesetVersionId,
        List<EventDb> events,
        List<CashflowProjectionDb> projections,
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
            list.Add(new MetricSnapshotDb(
                Guid.NewGuid(),
                sessionId,
                playerId,
                computedAt,
                item.Key,
                item.Value.Numeric,
                item.Value.Json,
                rulesetVersionId));
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
