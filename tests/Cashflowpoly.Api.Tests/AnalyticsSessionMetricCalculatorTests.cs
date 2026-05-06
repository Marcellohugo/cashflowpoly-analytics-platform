// Fungsi file: Menguji kalkulasi metrik agregat sesi analitik tanpa akses database.
using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Domain;
using Xunit;

namespace Cashflowpoly.Api.Tests;

public sealed class AnalyticsSessionMetricCalculatorTests
{
    [Fact]
    public void ComputeSessionMetrics_ComputesCashflowDonationAndHappinessTotals()
    {
        var events = new List<EventDb>
        {
            new()
            {
                EventId = Guid.NewGuid(),
                ActionType = "day.friday.donation",
                Payload = """{"amount":5}"""
            },
            new()
            {
                EventId = Guid.NewGuid(),
                ActionType = "work.freelance.completed",
                Payload = """{"amount":3}"""
            }
        };
        var projections = new List<CashflowProjectionDb>
        {
            new() { Direction = "IN", Amount = 10 },
            new() { Direction = "OUT", Amount = 4 },
            new() { Direction = "IN", Amount = 2 }
        };
        var happinessByPlayer = new Dictionary<Guid, AnalyticsHappinessBreakdown>
        {
            [Guid.NewGuid()] = new(11, 1, 2, 3, 4, 5, 6, 7, 8, false),
            [Guid.NewGuid()] = new(-2, 0, 0, 0, 0, 0, 0, 1, 1, true)
        };

        var metrics = AnalyticsSessionMetricCalculator.ComputeSessionMetrics(events, projections, happinessByPlayer);

        Assert.Equal(12, metrics["cashflow.in.total"].Numeric);
        Assert.Equal(4, metrics["cashflow.out.total"].Numeric);
        Assert.Equal(8, metrics["cashflow.net.total"].Numeric);
        Assert.Equal(5, metrics["donation.total"].Numeric);
        Assert.Equal(9, metrics["happiness.points.total"].Numeric);
    }
}
