// Fungsi file: Menguji kalkulasi saldo pemain dari starting cash dan proyeksi cashflow.
using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Domain;
using Xunit;

namespace Cashflowpoly.Api.Tests;

public sealed class EventPlayerBalanceCalculatorTests
{
    [Fact]
    public void Compute_ReturnsStartingCashPlusPlayerNetCashflow()
    {
        var playerId = Guid.NewGuid();
        var otherPlayerId = Guid.NewGuid();
        var projections = new List<CashflowProjectionDb>
        {
            CreateProjection(playerId, "IN", 12),
            CreateProjection(playerId, "OUT", 5),
            CreateProjection(otherPlayerId, "OUT", 99)
        };

        var balance = new EventPlayerBalanceCalculator().Compute(playerId, startingCash: 20, projections);

        Assert.Equal(27, balance);
    }

    private static CashflowProjectionDb CreateProjection(Guid playerId, string direction, int amount)
    {
        return new CashflowProjectionDb
        {
            ProjectionId = Guid.NewGuid(),
            SessionId = Guid.NewGuid(),
            PlayerId = playerId,
            EventPk = Guid.NewGuid(),
            EventId = Guid.NewGuid(),
            Timestamp = new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero),
            Direction = direction,
            Amount = amount,
            Category = "TEST"
        };
    }
}
