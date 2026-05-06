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
