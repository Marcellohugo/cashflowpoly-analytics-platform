// Fungsi file: Membangun proyeksi offset cashflow saat risiko ditangkal asuransi.
using System.Diagnostics.CodeAnalysis;
using Cashflowpoly.Api.Data;
using Cashflowpoly.Contracts;
using static Cashflowpoly.Api.Domain.EventPayloadReader;

namespace Cashflowpoly.Api.Domain;

internal static class EventInsuranceOffsetBuilder
{
    internal static bool TryReadRiskEventReference(
        EventRequest request,
        out string riskEventIdText,
        out Guid riskEventId)
    {
        riskEventIdText = string.Empty;
        riskEventId = Guid.Empty;
        if (request.PlayerId is null ||
            !string.Equals(request.ActionType, "insurance.multirisk.used", StringComparison.OrdinalIgnoreCase) ||
            !TryReadInsuranceUsed(request.Payload, out riskEventIdText))
        {
            return false;
        }

        return Guid.TryParse(riskEventIdText, out riskEventId);
    }

    internal static bool TryBuild(
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

        var riskPayload = ReadPayload(riskEvent.Payload);
        if (!TryReadRiskLife(riskPayload, out _, out var direction, out var amount) ||
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
