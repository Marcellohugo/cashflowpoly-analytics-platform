using System.Diagnostics.CodeAnalysis;
using Cashflowpoly.Api.Data;
using Cashflowpoly.Contracts;

namespace Cashflowpoly.Api.Domain;

public interface IEventCashflowProjectionBuilder
{
    bool TryBuild(
        EventRequest request,
        DateTimeOffset timestamp,
        Guid eventPk,
        [NotNullWhen(true)] out CashflowProjectionDb? projection);
}
