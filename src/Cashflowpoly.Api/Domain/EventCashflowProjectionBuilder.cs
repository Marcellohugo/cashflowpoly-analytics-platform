// Fungsi file: Membangun proyeksi arus kas dari event gameplay secara murni.
using System.Diagnostics.CodeAnalysis;
using Cashflowpoly.Api.Data;
using Cashflowpoly.Contracts;

namespace Cashflowpoly.Api.Domain;

/// <summary>
/// Builder proyeksi cashflow dari event gameplay.
/// </summary>
internal sealed class EventCashflowProjectionBuilder : IEventCashflowProjectionBuilder
{
    private static readonly EventPayloadReader _payloadReader = new();

    /// <summary>
    /// Mencoba membangun proyeksi arus kas dari event yang berdampak pada saldo pemain.
    /// </summary>
    public bool TryBuild(
        EventRequest request,
        DateTimeOffset timestamp,
        Guid eventPk,
        [NotNullWhen(true)] out CashflowProjectionDb? projection)
    {
        projection = null;
        if (request.PlayerId is null)
        {
            return false;
        }

        var action = request.ActionType;
        var playerId = request.PlayerId.Value;
        var direction = string.Empty;
        var amount = 0;
        var category = string.Empty;
        string? counterparty = null;
        string? reference = null;
        string? note = null;

        if (string.Equals(action, "transaction.recorded", StringComparison.OrdinalIgnoreCase) &&
            _payloadReader.TryReadTransaction(request.Payload, out var dir, out var amt, out var cat, out var cp))
        {
            direction = dir.ToUpperInvariant();
            amount = (int)Math.Round(amt);
            category = cat;
            counterparty = cp;
        }
        else if (string.Equals(action, "day.friday.donation", StringComparison.OrdinalIgnoreCase) &&
                 _payloadReader.TryReadAmount(request.Payload, out var donationAmount))
        {
            direction = "OUT";
            amount = (int)Math.Round(donationAmount);
            category = "DONATION";
        }
        else if (string.Equals(action, "day.saturday.gold_trade", StringComparison.OrdinalIgnoreCase) &&
                 _payloadReader.TryReadGoldTrade(request.Payload, out var tradeType, out _, out _, out var tradeAmount))
        {
            direction = string.Equals(tradeType, "BUY", StringComparison.OrdinalIgnoreCase) ? "OUT" : "IN";
            amount = tradeAmount;
            category = "GOLD_TRADE";
        }
        else if (string.Equals(action, "ingredient.purchased", StringComparison.OrdinalIgnoreCase) &&
                 _payloadReader.TryReadIngredientPurchase(request.Payload, out _, out var ingredientAmount))
        {
            direction = "OUT";
            amount = ingredientAmount;
            category = "INGREDIENT";
        }
        else if (string.Equals(action, "order.claimed", StringComparison.OrdinalIgnoreCase) &&
                 _payloadReader.TryReadOrderClaim(request.Payload, out _, out var income))
        {
            direction = "IN";
            amount = income;
            category = "ORDER";
        }
        else if (string.Equals(action, "work.freelance.completed", StringComparison.OrdinalIgnoreCase) &&
                 _payloadReader.TryReadAmount(request.Payload, out var freelanceAmount))
        {
            direction = "IN";
            amount = (int)Math.Round(freelanceAmount);
            category = "FREELANCE";
        }
        else if (string.Equals(action, "need.primary.purchased", StringComparison.OrdinalIgnoreCase) &&
                 _payloadReader.TryReadNeedPurchase(request.Payload, out _, out var primaryAmount, out _))
        {
            direction = "OUT";
            amount = primaryAmount;
            category = "NEED_PRIMARY";
        }
        else if (string.Equals(action, "need.secondary.purchased", StringComparison.OrdinalIgnoreCase) &&
                 _payloadReader.TryReadNeedPurchase(request.Payload, out _, out var secondaryAmount, out _))
        {
            direction = "OUT";
            amount = secondaryAmount;
            category = "NEED_SECONDARY";
        }
        else if (string.Equals(action, "need.tertiary.purchased", StringComparison.OrdinalIgnoreCase) &&
                 _payloadReader.TryReadNeedPurchase(request.Payload, out _, out var tertiaryAmount, out _))
        {
            direction = "OUT";
            amount = tertiaryAmount;
            category = "NEED_TERTIARY";
        }
        else if (string.Equals(action, "saving.deposit.created", StringComparison.OrdinalIgnoreCase) &&
                 _payloadReader.TryReadSavingDeposit(request.Payload, out _, out var savingAmount))
        {
            direction = "OUT";
            amount = savingAmount;
            category = "SAVING_DEPOSIT";
        }
        else if (string.Equals(action, "saving.deposit.withdrawn", StringComparison.OrdinalIgnoreCase) &&
                 _payloadReader.TryReadSavingDeposit(request.Payload, out _, out var savingWithdrawAmount))
        {
            direction = "IN";
            amount = savingWithdrawAmount;
            category = "SAVING_WITHDRAW";
        }
        else if (string.Equals(action, "risk.life.drawn", StringComparison.OrdinalIgnoreCase) &&
                 _payloadReader.TryReadRiskLife(request.Payload, out _, out var riskDirection, out var riskAmount))
        {
            direction = riskDirection.ToUpperInvariant();
            amount = riskAmount;
            category = "RISK_LIFE";
        }
        else if (string.Equals(action, "loan.syariah.taken", StringComparison.OrdinalIgnoreCase) &&
                 _payloadReader.TryReadLoanTaken(request.Payload, out _, out var principal, out _, out _, out _))
        {
            direction = "IN";
            amount = principal;
            category = "LOAN_TAKEN";
        }
        else if (string.Equals(action, "loan.syariah.repaid", StringComparison.OrdinalIgnoreCase) &&
                 _payloadReader.TryReadLoanRepay(request.Payload, out _, out var repayAmount))
        {
            direction = "OUT";
            amount = repayAmount;
            category = "LOAN_REPAID";
        }
        else if (string.Equals(action, "insurance.multirisk.purchased", StringComparison.OrdinalIgnoreCase) &&
                 _payloadReader.TryReadInsurance(request.Payload, out var premium))
        {
            direction = "OUT";
            amount = premium;
            category = "INSURANCE_PREMIUM";
        }
        else if (string.Equals(action, "risk.emergency.used", StringComparison.OrdinalIgnoreCase) &&
                 _payloadReader.TryReadEmergencyOption(request.Payload, out _, out _, out var emergencyDirection, out var emergencyAmount))
        {
            direction = emergencyDirection.ToUpperInvariant();
            amount = emergencyAmount;
            category = "EMERGENCY_OPTION";
        }
        else
        {
            return false;
        }

        if (amount <= 0 || string.IsNullOrWhiteSpace(direction))
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
            Direction = direction,
            Amount = amount,
            Category = category,
            Counterparty = counterparty,
            Reference = reference,
            Note = note
        };

        return true;
    }
}
