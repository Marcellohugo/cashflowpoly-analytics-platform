using System.Diagnostics.CodeAnalysis;
using Cashflowpoly.Api.Data;
using Cashflowpoly.Contracts;

namespace Cashflowpoly.Api.Domain;

public interface IEventDerivedStateCalculator
{
    EventIngredientInventory BuildIngredientInventory(IEnumerable<EventDb> events, Guid playerId);

    int ComputeSavingBalance(IEnumerable<EventDb> events, Guid playerId, string goalId);
}

public interface IEventPlayerBalanceCalculator
{
    double Compute(
        Guid playerId,
        int startingCash,
        IReadOnlyCollection<CashflowProjectionDb> projections);
}

