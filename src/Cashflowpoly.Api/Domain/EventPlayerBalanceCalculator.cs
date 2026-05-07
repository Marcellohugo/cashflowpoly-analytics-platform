// Fungsi file: Menghitung saldo pemain dari starting cash dan proyeksi arus kas.
using Cashflowpoly.Api.Data;

namespace Cashflowpoly.Api.Domain;

internal sealed class EventPlayerBalanceCalculator : IEventPlayerBalanceCalculator
{
    public double Compute(
        Guid playerId,
        int startingCash,
        IReadOnlyCollection<CashflowProjectionDb> projections)
    {
        var net = projections
            .Where(p => p.PlayerId == playerId)
            .Sum(p => p.Direction == "IN" ? p.Amount : -p.Amount);

        return startingCash + net;
    }
}
