// Fungsi file: Menguji kalkulasi diversifikasi sumber income gameplay.
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
            CreateEvent(sessionId, playerId, "work.freelance.completed", """{"amount":5}"""),
            CreateEvent(sessionId, playerId, "work.freelance.completed", """{"amount":5}""")
        };
        var projections = new List<CashflowProjectionDb>
        {
            CreateProjection(sessionId, playerId, "IN", 10, "DONATION_RECEIVED")
        };

        var metrics = new IncomeDiversificationCalculator().Compute(
            events,
            projections,
            totalIncome: 40,
            mealOrderIncomeTotal: 20,
            goldInvestmentEarned: 0);

        Assert.Equal(10, metrics.FreelanceIncome);
        Assert.Equal(20, metrics.MealIncome);
        Assert.Equal(0, metrics.GoldIncome);
        Assert.Equal(10, metrics.DonationIncome);
        Assert.Equal(3, metrics.ActiveIncomeSourceCount);
        Assert.Equal(0.25, metrics.IncomeShares["freelance_income"]);
        Assert.Equal(0.5, metrics.IncomeShares["meal_income"]);
        Assert.Equal(0.25, metrics.IncomeShares["donations_received"]);
        Assert.Equal(93.75, metrics.IncomeDiversification!.Value, precision: 6);
        Assert.False(metrics.RequiresIncomeNote);
    }

    [Fact]
    public void Compute_ReportsIncomeNoteWhenTotalIncomeIsZero()
    {
        var metrics = new IncomeDiversificationCalculator().Compute(
            Array.Empty<EventDb>(),
            Array.Empty<CashflowProjectionDb>(),
            totalIncome: 0,
            mealOrderIncomeTotal: 0,
            goldInvestmentEarned: 0);

        Assert.Empty(metrics.IncomeShares);
        Assert.Null(metrics.IncomeDiversification);
        Assert.True(metrics.RequiresIncomeNote);
    }

    private static EventDb CreateEvent(Guid sessionId, Guid playerId, string actionType, string payload)
    {
        return new EventDb
        {
            EventId = Guid.NewGuid(),
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
            EventId = Guid.NewGuid(),
            Timestamp = new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero),
            Direction = direction,
            Amount = amount,
            Category = category
        };
    }
}
