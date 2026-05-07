// Fungsi file: Menghitung poin happiness analitik pemain dari event gameplay, cashflow, dan scoring ruleset.
using Cashflowpoly.Api.Data;

namespace Cashflowpoly.Api.Domain;

/// <summary>
/// Kalkulator murni untuk breakdown happiness pemain.
/// </summary>
internal sealed class HappinessCalculator : IHappinessCalculator
{
    private static readonly AnalyticsPayloadReader _payloadReader = new();
    /// <summary>
    /// Menghitung breakdown happiness per pemain termasuk donasi, emas, pensiun, dan penalti.
    /// </summary>
    public Dictionary<Guid, AnalyticsHappinessBreakdown> ComputeByPlayer(
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

        if (hasScoring && config!.Scoring!.PensionRankPoints.Count > 0)
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

        var result = new Dictionary<Guid, AnalyticsHappinessBreakdown>();
        foreach (var (playerId, playerEvents) in playerGroups)
        {
            donationPointsByPlayer.TryGetValue(playerId, out var donationPoints);
            goldPointsByPlayer.TryGetValue(playerId, out var goldPoints);
            pensionPointsByPlayer.TryGetValue(playerId, out var pensionPoints);

            result[playerId] = ComputeBreakdown(playerEvents, donationPoints, goldPoints, pensionPoints);
        }

        return result;
    }

    /// <summary>
    /// Menghitung detail breakdown happiness satu pemain dari event kebutuhan, donasi, emas, tabungan, misi, dan pinjaman.
    /// </summary>
    public AnalyticsHappinessBreakdown ComputeBreakdown(
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
                _payloadReader.TryReadNeedPurchase(evt.Payload, out _, out _, out var needPointsValue))
            {
                primaryCount += 1;
                needPoints += needPointsValue;
            }

            if (evt.ActionType == "need.secondary.purchased" &&
                _payloadReader.TryReadNeedPurchase(evt.Payload, out _, out _, out var needPointsValueSecondary))
            {
                secondaryCount += 1;
                needPoints += needPointsValueSecondary;
            }

            if (evt.ActionType == "need.tertiary.purchased" &&
                _payloadReader.TryReadNeedPurchase(evt.Payload, out _, out var tertiaryCardId, out var needPointsValueTertiary))
            {
                tertiaryCount += 1;
                needPoints += needPointsValueTertiary;
                if (!string.IsNullOrWhiteSpace(tertiaryCardId))
                {
                    tertiaryCardIds.Add(tertiaryCardId);
                }
            }

            if (evt.ActionType == "mission.assigned" &&
                _payloadReader.TryReadMissionAssigned(evt.Payload, out var missionId, out var targetCardId, out var penaltyPoints, out var requirePrimary, out var requireSecondary))
            {
                missions.Add(new MissionAssignment(missionId, targetCardId, penaltyPoints, requirePrimary, requireSecondary));
            }

            if (evt.ActionType == "saving.goal.achieved" && _payloadReader.TryReadSavingGoalAchieved(evt.Payload, out var savingPoints))
            {
                savingGoalPoints += savingPoints;
            }

            if (evt.ActionType == "loan.syariah.taken" && _payloadReader.TryReadLoanTaken(evt.Payload, out var loanId, out var principal, out var penaltyPointsValue))
            {
                loans[loanId] = new LoanState(loanId, principal, penaltyPointsValue, 0);
            }

            if (evt.ActionType == "loan.syariah.repaid" && _payloadReader.TryReadLoanRepay(evt.Payload, out var repayLoanId, out var repayAmount))
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

        return new AnalyticsHappinessBreakdown(
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

    private Dictionary<Guid, int> BuildTieBreakerLookup(IEnumerable<EventDb> events)
    {
        return events.Where(e => e.PlayerId.HasValue && e.ActionType == "tie_breaker.assigned")
            .OrderBy(e => e.SequenceNumber)
            .GroupBy(e => e.PlayerId!.Value)
            .ToDictionary(
                g => g.Key,
                g =>
                {
                    var last = g.Last();
                    return _payloadReader.TryReadTieBreaker(last.Payload, out var number) ? number : 0;
                });
    }

    private Dictionary<Guid, double> ComputeDonationPointsFromScoring(
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
                    var total = g.Sum(e => _payloadReader.TryReadAmount(e.Payload, out var amount) ? amount : 0);
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

    private Dictionary<Guid, double> ComputeGoldPointsFromScoring(
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
                    if (!_payloadReader.TryReadGoldTrade(e.Payload, out var tradeType, out var qty))
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

    private Dictionary<Guid, double> ComputePensionPointsFromScoring(
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

    private int ResolvePointsByQty(int qty, IReadOnlyList<QtyPoint> table)
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

    public double SumRankAwarded(IEnumerable<EventDb> events, string actionType)
    {
        return events.Where(e => e.ActionType == actionType)
            .Sum(e => _payloadReader.TryReadRankAwarded(e.Payload, out _, out var points) ? points : 0);
    }

    public double SumPointsAwarded(IEnumerable<EventDb> events, string actionType)
    {
        return events.Where(e => e.ActionType == actionType)
            .Sum(e => _payloadReader.TryReadPointsAwarded(e.Payload, out var points) ? points : 0);
    }

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
}
