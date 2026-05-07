// Fungsi file: Mengevaluasi kepatuhan kebutuhan primer pemain berdasarkan event dan ruleset.
using Cashflowpoly.Api.Data;

namespace Cashflowpoly.Api.Domain;

/// <summary>
/// Evaluator murni untuk compliance kebutuhan primer per hari.
/// </summary>
internal sealed class PrimaryNeedComplianceEvaluator : IPrimaryNeedComplianceEvaluator
{
    /// <summary>
    /// Mengevaluasi kepatuhan kebutuhan primer per hari dengan pengecekan urutan beli dan batas harian.
    /// </summary>
    public PrimaryNeedComplianceResult Evaluate(List<EventDb> playerEvents, RulesetConfig? config)
    {
        if (config is null)
        {
            return new PrimaryNeedComplianceResult(0, [], 0, 0);
        }

        var days = playerEvents
            .Select(e => e.DayIndex)
            .Distinct()
            .OrderBy(d => d)
            .ToList();

        if (days.Count == 0)
        {
            return new PrimaryNeedComplianceResult(0, [], 0, 0);
        }

        var details = new List<PrimaryNeedComplianceDayDetail>();
        var compliantDays = 0;

        foreach (var dayIndex in days)
        {
            var dayEvents = playerEvents.Where(e => e.DayIndex == dayIndex).OrderBy(e => e.SequenceNumber).ToList();
            var primaryCount = dayEvents.Count(e => e.ActionType == "need.primary.purchased");
            var violationReasons = new List<string>();

            if (primaryCount > config.PrimaryNeedMaxPerDay)
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

            details.Add(new PrimaryNeedComplianceDayDetail(dayIndex, compliant, violationReasons));
        }

        var rate = (double)compliantDays / days.Count;
        return new PrimaryNeedComplianceResult(rate, details, days.Count, compliantDays);
    }
}
