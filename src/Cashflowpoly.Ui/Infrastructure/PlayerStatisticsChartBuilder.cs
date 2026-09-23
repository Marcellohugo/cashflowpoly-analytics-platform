// Fungsi file: Membentuk grafik lintas sesi dengan nilai, satuan, dan arti yang sama dengan analitika pemain per sesi.
using System.Globalization;
using System.Text.Json;
using Cashflowpoly.Ui.Contracts;
using Cashflowpoly.Ui.Models;

namespace Cashflowpoly.Ui.Infrastructure;

public static class PlayerStatisticsChartBuilder
{
    public static List<PlayerStatisticsChart> Build(IReadOnlyList<PlayerStatisticsSession> sessions, Func<string, string> t)
    {
        if (sessions.Select(s => s.Session.Mode).Distinct().Skip(1).Any())
            throw new ArgumentException("Statistik harus memakai satu mode permainan.", nameof(sessions));
        var charts = new List<PlayerStatisticsChart>();
        Add("total_happiness_points", "overview", false, false, g => g.Score.HappinessPointsTotal);
        Add("coins_net_end_game", "overview", false, false, g => g.Economy.StartingCash + g.Economy.CashflowNetTotal);
        Add("cash_in_total", "money", false, false, g => g.Economy.CashInTotal);
        Add("cash_out_total", "money", false, false, g => g.Economy.CashOutTotal);
        Add("cash_growth_percent", "money");
        Add("income_diversification_index", "money");
        Add("business_expense_share_percent", "money");
        Add("meal_order_profit_margin_percent", "money");
        Add("income_action_focus_percent", "play");
        Add("ingredient_utilization_percent", "play");
        Add("meal_orders_claimed", "play", false, false, g => g.Progress.OrdersCompletedCount);
        Add("risk_readiness_percent", "future", true, true);
        Add("sharia_loans_taken", "future", false, true,
            g => Number(g.RawJson, "financial_goals", "sharia_loans_taken"));
        Add("sharia_loans_repaid", "future", false, true,
            g => Number(g.RawJson, "financial_goals", "sharia_loans_repaid"));
        Add("financial_goal_completion_percent", "future", true, true);
        Add("long_term_action_share_percent", "future", true, true);
        Add("need_fulfillment_diversity_percent", "happiness");
        Add("donation_commitment_score", "happiness");
        Add("happiness_source_diversity_percent", "happiness");
        return charts;

        void Add(string key, string group, bool derived = true, bool advancedOnly = false,
            Func<GameplayMetricsResponse, double?>? select = null)
        {
            if (advancedOnly && !sessions.Any(s => s.Session.Mode == "MAHIR")) return;
            var points = sessions.Select((entry, index) =>
            {
                var value = entry.Gameplay is { } gameplay ? select is null
                    ? Number(gameplay.DerivedJson, key) : select(gameplay) : null;
                if (advancedOnly && entry.Session.Mode != "MAHIR") value = null;
                var presentation = PlayerMetricLabelFormatter.DescribeMetric(key,
                    value?.ToString(CultureInfo.InvariantCulture) ?? t("players.support.value.unavailable"),
                    derived, entry.Session.Mode == "MAHIR", t("players.support.value.unavailable"), t);
                // cash_growth_percent disimpan sebagai perbandingan terhadap modal; UI menyajikan perubahan.
                var plotted = value.HasValue && key == "cash_growth_percent" ? value - 100 : value;
                if (presentation.State is "unavailable" or "not_applicable") plotted = null;
                return new PlayerStatisticsPoint($"{index + 1}. {entry.Session.SessionName}", plotted, presentation);
            }).ToList();
            var exemplar = points.FirstOrDefault(p => p.Value.HasValue)?.Presentation
                ?? PlayerMetricLabelFormatter.DescribeMetric(key, "1", derived, true, t("players.support.value.unavailable"), t);
            var source = t("statistics.source") + " " + exemplar.Explanation;
            var json = JsonSerializer.Serialize(new
            {
                chartType = "line", labels = points.Select((_, i) => $"{t("statistics.session_ordinal_prefix")}{i + 1}"),
                formulas = points.Select(p => $"{p.Label}: {p.Presentation.DisplayValue} {p.Presentation.Unit}. {p.Presentation.Guidance}"),
                pointDetails = points.Select((p, i) => new {
                    sessionId = sessions[i].Session.SessionId,
                    sessionName = sessions[i].Session.SessionName,
                    displayValue = p.Presentation.DisplayValue,
                    unit = p.Presentation.Unit,
                    guidance = p.Presentation.Guidance
                }),
                detailLabel = t("statistics.meaning"), detailFallback = source,
                series = new[] { new { name = exemplar.Unit, values = points.Select(p => p.Value) } }
            });
            charts.Add(new PlayerStatisticsChart(key, group, exemplar.Label, exemplar.Unit, source, json, points));
        }
    }

    private static double? Number(JsonElement? root, params string[] path)
    {
        if (!root.HasValue) return null;
        var value = root.Value;
        foreach (var key in path)
            if (value.ValueKind != JsonValueKind.Object || !value.TryGetProperty(key, out value)) return null;
        return value.ValueKind == JsonValueKind.Number && value.TryGetDouble(out var number) && double.IsFinite(number)
            ? number : null;
    }
}
