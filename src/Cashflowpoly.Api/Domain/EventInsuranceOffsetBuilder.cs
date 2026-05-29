using System.Diagnostics.CodeAnalysis;
using Cashflowpoly.Api.Data;
using Cashflowpoly.Contracts;

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
        if (request.PlayerId is null ||
            !string.Equals(request.ActionType, "insurance.multirisk.used", StringComparison.OrdinalIgnoreCase) ||
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
        if (request.PlayerId is not { } playerId ||
            !TryReadRiskEventReference(request, out var riskEventIdText, out _))
        {
            return false;
        }

        if (riskEvent is null ||
            riskEvent.PlayerId != playerId ||
            !string.Equals(riskEvent.ActionType, "risk.life.drawn", StringComparison.OrdinalIgnoreCase))
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
            PlayerId = playerId,
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
