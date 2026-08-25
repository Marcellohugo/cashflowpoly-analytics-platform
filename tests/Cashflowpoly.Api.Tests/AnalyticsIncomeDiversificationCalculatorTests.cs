// Fungsi file: Memverifikasi perilaku API, database, atau domain melalui AnalyticsIncomeDiversificationCalculatorTests.
using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Domain;
using Xunit;

namespace Cashflowpoly.Api.Tests;

public sealed class AnalyticsIncomeDiversificationCalculatorTests
{
    [Fact]
    public void Compute_CalculatesIncomeSharesAndDiversificationIndex()
    {
        var playerId = Guid.NewGuid();
        var sessionId = Guid.NewGuid();
        var events = new List<EventDb>
        {
            CreateEvent(sessionId, playerId, "KerjaLepas", """{"amount":5}"""),
            CreateEvent(sessionId, playerId, "KerjaLepas", """{"amount":5}""")
        };
        var projections = new List<CashflowProjectionDb>
        {
            CreateProjection(sessionId, playerId, "IN", 10, "DONATION_RECEIVED"),
            CreateProjection(sessionId, playerId, "IN", 60, "LOAN_TAKEN")
        };

        var metrics = new IncomeDiversificationCalculator().Compute(
            events,
            mealOrderIncomeTotal: 20,
            goldInvestmentEarned: 0);

        Assert.Equal(10, metrics.FreelanceIncome);
        Assert.Equal(20, metrics.MealIncome);
        Assert.Equal(0, metrics.GoldIncome);
        Assert.Equal(2, metrics.ActiveIncomeSourceCount);
        Assert.Equal(1d / 3d, metrics.IncomeShares["freelance_income"], precision: 6);
        Assert.Equal(2d / 3d, metrics.IncomeShares["meal_order_income"], precision: 6);
        Assert.Equal(88.888889, metrics.IncomeDiversificationIndex!.Value, precision: 6);
        Assert.False(metrics.RequiresIncomeNote);
    }

    [Fact]
    public void Compute_ReportsIncomeNoteWhenTotalIncomeIsZero()
    {
        var metrics = new IncomeDiversificationCalculator().Compute(
            Array.Empty<EventDb>(),
            mealOrderIncomeTotal: 0,
            goldInvestmentEarned: 0);

        Assert.Empty(metrics.IncomeShares);
        Assert.Null(metrics.IncomeDiversificationIndex);
        Assert.True(metrics.RequiresIncomeNote);
    }

    private static EventDb CreateEvent(Guid sessionId, Guid playerId, string actionType, string payload)
    {
        return new EventDb
        {
            EventId = Guid.NewGuid(),
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
            UserId = playerId,
            EventPk = Guid.NewGuid(),
            EventId = Guid.NewGuid(),
            Timestamp = new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero),
            Direction = direction,
            Amount = amount,
            Category = category
        };
    }
}
