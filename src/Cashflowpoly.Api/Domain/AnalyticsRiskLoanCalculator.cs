// Fungsi file: Menghitung metrik risiko, asuransi, emergency option, dan pinjaman gameplay.
using Cashflowpoly.Api.Data;
using static Cashflowpoly.Api.Domain.AnalyticsMath;

namespace Cashflowpoly.Api.Domain;

public sealed record AnalyticsRiskLoanMetrics(
    IReadOnlyList<int> RiskCostsPerCard,
    int RiskCostsTotal,
    int RiskCardsDrawn,
    int RiskMitigated,
    int RiskAccepted,
    int InsurancePayments,
    int EmergencyOptionsUsed,
    int LoansTaken,
    int LoansRepaid,
    int LoansUnpaid,
    double LoansOutstandingAmount,
    double? RiskExposurePercentage,
    double? RiskMitigationEffectiveness,
    double AverageRiskCost,
    double? RiskAcceptanceRate,
    double? InsuranceCoverageRate,
    double? RiskCostIntensity,
    double? RiskAppetiteScore,
    double? DebtLeverageRatio,
    double? LoanRepaymentDiscipline,
    double? DebtRatio);

internal sealed class RiskLoanCalculator : IRiskLoanCalculator
{
    private static readonly AnalyticsPayloadReader _payloadReader = new();

    public AnalyticsRiskLoanMetrics Compute(
        IReadOnlyCollection<EventDb> playerEvents,
        IReadOnlyCollection<CashflowProjectionDb> playerProjections,
        int startingCoins,
        double coinsNetEndGame,
        double totalIncome)
    {
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
        var riskAccepted = Math.Max(0, riskCardsDrawn - riskMitigated);
        var insurancePayments = playerProjections
            .Where(p => p.Category == "INSURANCE_PREMIUM" && p.Direction == "OUT")
            .Sum(p => p.Amount);
        var emergencyOptionsUsed = playerEvents.Count(e => e.ActionType == "risk.emergency.used");

        var loanStates = BuildLoanStates(playerEvents);
        var loansTaken = loanStates.Count;
        var loansRepaid = loanStates.Values.Count(l => l.RepaidAmount >= l.Principal);
        var loansUnpaid = loanStates.Values.Count(l => l.RepaidAmount < l.Principal);
        var loansOutstandingAmount = loanStates.Values.Sum(l => Math.Max(0, l.Principal - l.RepaidAmount));

        var averageRiskCost = riskCardsDrawn > 0 ? (double)riskCostsTotal / riskCardsDrawn : 0;
        var riskAcceptanceRate = SafeRatio(riskAccepted, riskCardsDrawn);
        var insuranceCoverageRate = SafeRatio(riskMitigated, riskCardsDrawn);
        var riskCostIntensity = SafeRatio(averageRiskCost, startingCoins);
        var riskAppetiteScore =
            riskAcceptanceRate.HasValue &&
            insuranceCoverageRate.HasValue &&
            riskCostIntensity.HasValue
                ? riskAcceptanceRate.Value * (1 - insuranceCoverageRate.Value) * riskCostIntensity.Value * 100
                : (double?)null;

        return new AnalyticsRiskLoanMetrics(
            riskCostsPerCard,
            riskCostsTotal,
            riskCardsDrawn,
            riskMitigated,
            riskAccepted,
            insurancePayments,
            emergencyOptionsUsed,
            loansTaken,
            loansRepaid,
            loansUnpaid,
            loansOutstandingAmount,
            SafeRatio(riskCostsTotal, totalIncome, true),
            SafeRatio(riskMitigated, riskCardsDrawn, true),
            averageRiskCost,
            riskAcceptanceRate,
            insuranceCoverageRate,
            riskCostIntensity,
            riskAppetiteScore,
            SafeRatio(loansOutstandingAmount, coinsNetEndGame, true),
            SafeRatio(loansRepaid, loansTaken, true),
            SafeRatio(loansUnpaid, loansTaken));
    }

    private Dictionary<string, LoanState> BuildLoanStates(IEnumerable<EventDb> playerEvents)
    {
        var loanStates = new Dictionary<string, LoanState>(StringComparer.OrdinalIgnoreCase);
        foreach (var evt in playerEvents)
        {
            if (evt.ActionType == "loan.syariah.taken" &&
                _payloadReader.TryReadLoanTaken(evt.Payload, out var loanId, out var principal, out var penaltyPoints))
            {
                loanStates[loanId] = new LoanState(loanId, principal, penaltyPoints, 0);
            }

            if (evt.ActionType == "loan.syariah.repaid" &&
                _payloadReader.TryReadLoanRepay(evt.Payload, out var repayLoanId, out var repayAmount) &&
                loanStates.TryGetValue(repayLoanId, out var state))
            {
                loanStates[repayLoanId] = state with { RepaidAmount = state.RepaidAmount + repayAmount };
            }
        }

        return loanStates;
    }

    private sealed record LoanState(
        string LoanId,
        int Principal,
        int PenaltyPoints,
        double RepaidAmount);
}
