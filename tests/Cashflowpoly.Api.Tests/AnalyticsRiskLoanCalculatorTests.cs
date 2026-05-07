// Fungsi file: Menguji kalkulasi risiko, asuransi, emergency option, dan pinjaman gameplay.
using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Domain;
using Xunit;

namespace Cashflowpoly.Api.Tests;

public sealed class AnalyticsRiskLoanCalculatorTests
{
    [Fact]
    public void Compute_SummarizesRiskAndLoanMetrics()
    {
        var playerId = Guid.NewGuid();
        var sessionId = Guid.NewGuid();
        var riskOne = Guid.NewGuid();
        var riskTwo = Guid.NewGuid();
        var events = new List<EventDb>
        {
            CreateEvent(riskOne, sessionId, playerId, "risk.life.drawn", """{"risk_id":"risk-1","direction":"OUT","amount":6}"""),
            CreateEvent(riskTwo, sessionId, playerId, "risk.life.drawn", """{"risk_id":"risk-2","direction":"OUT","amount":4}"""),
            CreateEvent(Guid.NewGuid(), sessionId, playerId, "insurance.multirisk.used", """{"risk_event_id":"risk-1"}"""),
            CreateEvent(Guid.NewGuid(), sessionId, playerId, "risk.emergency.used", """{"risk_event_id":"risk-2","option_type":"OTHER","direction":"OUT","amount":2}"""),
            CreateEvent(Guid.NewGuid(), sessionId, playerId, "loan.syariah.taken", """{"loan_id":"loan-a","principal":10,"installment":5,"duration_turns":2,"penalty_points":15}"""),
            CreateEvent(Guid.NewGuid(), sessionId, playerId, "loan.syariah.repaid", """{"loan_id":"loan-a","amount":4}""")
        };
        var projections = new List<CashflowProjectionDb>
        {
            CreateProjection(riskOne, sessionId, playerId, "OUT", 6, "RISK_LIFE"),
            CreateProjection(riskTwo, sessionId, playerId, "OUT", 4, "RISK_LIFE"),
            CreateProjection(Guid.NewGuid(), sessionId, playerId, "OUT", 1, "INSURANCE_PREMIUM")
        };

        var metrics = new RiskLoanCalculator().Compute(events, projections, startingCoins: 20, coinsNetEndGame: 30, totalIncome: 50);

        Assert.Equal(new[] { 6, 4 }, metrics.RiskCostsPerCard);
        Assert.Equal(10, metrics.RiskCostsTotal);
        Assert.Equal(2, metrics.RiskCardsDrawn);
        Assert.Equal(1, metrics.RiskMitigated);
        Assert.Equal(1, metrics.RiskAccepted);
        Assert.Equal(1, metrics.InsurancePayments);
        Assert.Equal(1, metrics.EmergencyOptionsUsed);
        Assert.Equal(1, metrics.LoansTaken);
        Assert.Equal(0, metrics.LoansRepaid);
        Assert.Equal(1, metrics.LoansUnpaid);
        Assert.Equal(6, metrics.LoansOutstandingAmount);
        Assert.Equal(20, metrics.RiskExposurePercentage);
        Assert.Equal(50, metrics.RiskMitigationEffectiveness);
        Assert.Equal(5, metrics.AverageRiskCost);
        Assert.Equal(0.5, metrics.RiskAcceptanceRate);
        Assert.Equal(0.5, metrics.InsuranceCoverageRate);
        Assert.Equal(0.25, metrics.RiskCostIntensity);
        Assert.Equal(6.25, metrics.RiskAppetiteScore);
        Assert.Equal(20, metrics.DebtLeverageRatio);
        Assert.Equal(0, metrics.LoanRepaymentDiscipline);
        Assert.Equal(1, metrics.DebtRatio);
    }

    private static EventDb CreateEvent(Guid eventId, Guid sessionId, Guid playerId, string actionType, string payload)
    {
        return new EventDb
        {
            EventId = eventId,
            SessionId = sessionId,
            PlayerId = playerId,
            ActorType = "PLAYER",
            Timestamp = new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero),
            DayIndex = 0,
            Weekday = "MON",
            TurnNumber = 1,
            SequenceNumber = 1,
            ActionType = actionType,
            RulesetVersionId = Guid.NewGuid(),
            Payload = payload
        };
    }

    private static CashflowProjectionDb CreateProjection(
        Guid eventId,
        Guid sessionId,
        Guid playerId,
        string direction,
        int amount,
        string category)
    {
        return new CashflowProjectionDb
        {
            ProjectionId = Guid.NewGuid(),
            SessionId = sessionId,
            PlayerId = playerId,
            EventPk = Guid.NewGuid(),
            EventId = eventId,
            Timestamp = new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero),
            Direction = direction,
            Amount = amount,
            Category = category
        };
    }
}
