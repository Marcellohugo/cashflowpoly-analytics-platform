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

public interface IEventInsuranceOffsetBuilder
{
    bool TryReadRiskEventReference(
        EventRequest request,
        out string riskEventIdText,
        out Guid riskEventId);

    bool TryBuild(
        EventRequest request,
        DateTimeOffset timestamp,
        Guid eventPk,
        EventDb? riskEvent,
        [NotNullWhen(true)] out CashflowProjectionDb? projection);
}
