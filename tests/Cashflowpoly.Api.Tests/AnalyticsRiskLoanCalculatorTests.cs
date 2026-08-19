// Fungsi file: Memverifikasi perilaku API, database, atau domain melalui AnalyticsRiskLoanCalculatorTests.
using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Domain;
using Xunit;

namespace Cashflowpoly.Api.Tests;

public sealed class AnalyticsRiskLoanCalculatorTests
{
    [Fact]
    public void Compute_IncludesSetupAndEmergencyLoanInstances()
    {
        var playerId = Guid.NewGuid();
        var sessionId = Guid.NewGuid();
        var events = new List<EventDb>
        {
            CreateEvent(Guid.NewGuid(), sessionId, playerId, "SetupPinjamanAwal", """{"loan_id":"setup-loan","principal":10,"penalty_points":15}"""),
            CreateEvent(Guid.NewGuid(), sessionId, playerId, "GunakanOpsiDarurat", """{"option_type":"TAKE_SHARIA_LOAN","loan_id":"emergency-loan","principal":10,"penalty_points":15}"""),
            CreateEvent(Guid.NewGuid(), sessionId, playerId, "BayarPinjaman", """{"loan_id":"setup-loan","amount":10}""")
        };

        var metrics = new RiskLoanCalculator().Compute(events, [], 10, 10, 20);

        Assert.Equal(2, metrics.LoansTaken);
        Assert.Equal(1, metrics.LoansRepaid);
        Assert.Equal(1, metrics.LoansUnpaid);
        Assert.Equal(10, metrics.LoansOutstandingAmount);
    }

    [Fact]
    public void Compute_SummarizesRiskAndLoanMetrics()
    {
        var playerId = Guid.NewGuid();
        var sessionId = Guid.NewGuid();
        var riskOne = Guid.NewGuid();
        var riskTwo = Guid.NewGuid();
        var events = new List<EventDb>
        {
            CreateEvent(riskOne, sessionId, playerId, "RisikoKehidupan", """{"risk_id":"risk-1","direction":"OUT","amount":6}"""),
            CreateEvent(riskTwo, sessionId, playerId, "RisikoKehidupan", """{"risk_id":"risk-2","direction":"OUT","amount":4}"""),
            CreateEvent(Guid.NewGuid(), sessionId, playerId, "Asuransi", """{"risk_event_id":"risk-1"}"""),
            CreateEvent(Guid.NewGuid(), sessionId, playerId, "GunakanOpsiDarurat", """{"risk_event_id":"risk-2","option_type":"OTHER","direction":"OUT","amount":2}"""),
            CreateEvent(Guid.NewGuid(), sessionId, playerId, "PinjamanSyariah", """{"loan_id":"loan-a","principal":10,"repayment_amount":5,"duration_turns":2,"penalty_points":15}"""),
            CreateEvent(Guid.NewGuid(), sessionId, playerId, "BayarPinjaman", """{"loan_id":"loan-a","amount":4}""")
        };
        var projections = new List<CashflowProjectionDb>
        {
            CreateProjection(riskOne, sessionId, playerId, "OUT", 6, "RISK_LIFE"),
            CreateProjection(Guid.NewGuid(), sessionId, playerId, "OUT", 4, "RISK_LIFE", riskTwo.ToString()),
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
        Assert.Equal(12.5, metrics.RiskAppetiteScore);
        Assert.Equal(20, metrics.DebtLeverageRatio);
        Assert.Equal(0, metrics.LoanRepaymentDiscipline);
        Assert.Equal(1, metrics.DebtRatio);
    }

    [Fact]
    public void Compute_RiskAppetiteIncreasesWhenUnprotectedRisksAreAccepted()
    {
        var playerId = Guid.NewGuid();
        var sessionId = Guid.NewGuid();
        var risk = Guid.NewGuid();
        var events = new[]
        {
            CreateEvent(risk, sessionId, playerId, "RisikoKehidupan", """{"risk_id":"risk-1","direction":"OUT","amount":10}""")
        };
        var projections = new[]
        {
            CreateProjection(risk, sessionId, playerId, "OUT", 10, "RISK_LIFE")
        };

        var metrics = new RiskLoanCalculator().Compute(events, projections, startingCoins: 20, coinsNetEndGame: 10, totalIncome: 20);

        Assert.Equal(1, metrics.RiskAcceptanceRate);
        Assert.Equal(0, metrics.InsuranceCoverageRate);
        Assert.Equal(50, metrics.RiskAppetiteScore);
    }

    private static EventDb CreateEvent(Guid eventId, Guid sessionId, Guid playerId, string actionType, string payload)
    {
        return new EventDb
        {
            EventId = eventId,
            SessionId = sessionId,
            UserId = playerId,
            ActorType = "PLAYER",
            Timestamp = new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero),
            DayIndex = 0,
            Weekday = "MON",
            ActionSlot = 1,
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
        string category,
        string? reference = null)
    {
        return new CashflowProjectionDb
        {
            ProjectionId = Guid.NewGuid(),
            SessionId = sessionId,
            UserId = playerId,
            EventPk = Guid.NewGuid(),
            EventId = eventId,
            Timestamp = new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero),
            Direction = direction,
            Amount = amount,
            Category = category,
            Reference = reference
        };
    }
}
