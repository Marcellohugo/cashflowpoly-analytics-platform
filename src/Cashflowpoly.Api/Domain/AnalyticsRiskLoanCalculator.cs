// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui AnalyticsRiskLoanCalculator.
using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Contracts;
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
        double totalIncome,
        IReadOnlyCollection<RulesetLifeRiskDto>? lifeRisks = null)
    {
        var riskEvents = playerEvents.Where(e => e.ActionType == "RisikoKehidupan").ToList();
        var riskDefinitions = (lifeRisks ?? [])
            .ToDictionary(item => item.RiskCode, StringComparer.OrdinalIgnoreCase);
        var riskCostsPerCard = new List<int>();
        foreach (var riskEvent in riskEvents)
        {
            var cost = TryReadRiskId(riskEvent.Payload, out var riskId) &&
                       riskDefinitions.TryGetValue(riskId, out var definition)
                ? definition.Direction == "OUT" && definition.EffectType.Contains("COIN_EFFECT", StringComparison.OrdinalIgnoreCase)
                    ? definition.Amount
                    : 0
                : playerProjections
                    .Where(p => p.Category == "RISK_LIFE" &&
                                p.Direction == "OUT" &&
                                (p.EventId == riskEvent.EventId || ReferencesRiskEvent(p.Reference, riskEvent.EventId)))
                    .Sum(p => p.Amount);
            riskCostsPerCard.Add(cost);
        }

        var riskCostsTotal = riskCostsPerCard.Sum();
        var riskCardsDrawn = riskEvents.Count;
        var riskMitigated = playerEvents.Count(e =>
            e.ActionType == GameActionCatalog.Asuransi &&
            e.Payload.Contains("\"risk_event_id\"", StringComparison.OrdinalIgnoreCase));
        var riskAccepted = Math.Max(0, riskCardsDrawn - riskMitigated);
        var insurancePayments = playerProjections
            .Where(p => p.Category == "INSURANCE_PREMIUM" && p.Direction == "OUT")
            .Sum(p => p.Amount);
        var emergencyOptionsUsed = playerEvents.Count(e => e.ActionType == "GunakanOpsiDarurat");

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
            riskCostIntensity.HasValue
                ? Clamp(riskAcceptanceRate.Value * riskCostIntensity.Value * 100, 0, 100)
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

    private static bool ReferencesRiskEvent(string? reference, Guid riskEventId)
        => Guid.TryParse(reference, out var referencedEventId) && referencedEventId == riskEventId;

    private static bool TryReadRiskId(string payload, out string riskId)
    {
        riskId = string.Empty;
        try
        {
            using var document = System.Text.Json.JsonDocument.Parse(payload);
            if (!document.RootElement.TryGetProperty("risk_id", out var riskIdElement))
            {
                return false;
            }

            riskId = riskIdElement.GetString() ?? string.Empty;
            return riskId.Length > 0;
        }
        catch (System.Text.Json.JsonException)
        {
            return false;
        }
    }

    private Dictionary<string, LoanState> BuildLoanStates(IEnumerable<EventDb> playerEvents)
    {
        var loanStates = new Dictionary<string, LoanState>(StringComparer.OrdinalIgnoreCase);
        foreach (var evt in playerEvents)
        {
            if ((evt.ActionType == GameActionCatalog.PinjamanSyariah ||
                 evt.ActionType == GameActionCatalog.SetupPinjamanAwal ||
                 IsEmergencyLoan(evt.Payload)) &&
                _payloadReader.TryReadLoanTaken(evt.Payload, out var loanId, out var principal, out var penaltyPoints))
            {
                loanStates[loanId] = new LoanState(loanId, principal, penaltyPoints, 0);
            }

            if (evt.ActionType == "BayarPinjaman" &&
                _payloadReader.TryReadLoanRepay(evt.Payload, out var repayLoanId, out var repayAmount) &&
                loanStates.TryGetValue(repayLoanId, out var state))
            {
                loanStates[repayLoanId] = state with { RepaidAmount = state.RepaidAmount + repayAmount };
            }
        }

        return loanStates;
    }

    private static bool IsEmergencyLoan(string payload)
    {
        try
        {
            using var document = System.Text.Json.JsonDocument.Parse(payload);
            return document.RootElement.TryGetProperty("option_type", out var optionType) &&
                   string.Equals(optionType.GetString(), "TAKE_SHARIA_LOAN", StringComparison.OrdinalIgnoreCase);
        }
        catch (System.Text.Json.JsonException)
        {
            return false;
        }
    }

    private sealed record LoanState(
        string LoanId,
        int Principal,
        int PenaltyPoints,
        double RepaidAmount);
}
