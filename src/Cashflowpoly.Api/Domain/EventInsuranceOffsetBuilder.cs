using System.Diagnostics.CodeAnalysis;
using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Contracts;

namespace Cashflowpoly.Api.Domain;

internal sealed class EventInsuranceOffsetBuilder : IEventInsuranceOffsetBuilder
{
    private static readonly EventPayloadReader _payloadReader = new();

    public bool TryReadRiskEventReference(
        EventRequest request,
        out string riskEventIdText,
        out Guid riskEventId)
    {
        riskEventIdText = string.Empty;
        riskEventId = Guid.Empty;
        if (request.UserId is null ||
            !GameActionCatalog.Is(request.ActionType, request.Payload, GameActionCatalog.Asuransi) ||
            !_payloadReader.TryReadInsuranceUsed(request.Payload, out riskEventIdText))
        {
            return false;
        }

        return Guid.TryParse(riskEventIdText, out riskEventId);
    }

    public bool TryBuild(
        EventRequest request,
        DateTimeOffset timestamp,
        Guid eventPk,
        EventDb? riskEvent,
        [NotNullWhen(true)] out CashflowProjectionDb? projection)
    {
        projection = null;
        if (request.UserId is not { } userId ||
            !TryReadRiskEventReference(request, out var riskEventIdText, out _))
        {
            return false;
        }

        if (riskEvent is null ||
            riskEvent.UserId != userId ||
            !GameActionCatalog.Is(riskEvent.ActionType, _payloadReader.ReadPayload(riskEvent.Payload), GameActionCatalog.RisikoKehidupan))
        {
            return false;
        }

        var riskPayload = _payloadReader.ReadPayload(riskEvent.Payload);
        if (!_payloadReader.TryReadRiskLife(riskPayload, out _, out var direction, out var amount) ||
            !string.Equals(direction, "OUT", StringComparison.OrdinalIgnoreCase) ||
            amount <= 0)
        {
            return false;
        }

        projection = new CashflowProjectionDb
        {
            ProjectionId = Guid.NewGuid(),
            SessionId = request.SessionId,
            UserId = userId,
            EventPk = eventPk,
            EventId = request.EventId,
            Timestamp = timestamp,
            Direction = "IN",
            Amount = amount,
            Category = "INSURANCE_CLAIM",
            Counterparty = "BANK",
            Reference = riskEventIdText,
            Note = "Offset risiko via asuransi"
        };
        return true;
    }
}
