using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Contracts;

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
        if (request.UserId is null)
        {
            return false;
        }

        var sourceAction = request.ActionType.Trim();
        var action = GameActionCatalog.ResolveGameActionId(sourceAction, request.Payload) ?? sourceAction;
        var playerId = request.UserId.Value;
        var direction = string.Empty;
        var amount = 0;
        var category = string.Empty;
        string? counterparty = null;
        string? reference = null;
        string? note = null;

        if (string.Equals(action, "CatatTransaksi", StringComparison.OrdinalIgnoreCase) &&
            _payloadReader.TryReadTransaction(request.Payload, out var dir, out var amt, out var cat, out var cp))
        {
            direction = dir.ToUpperInvariant();
            amount = (int)Math.Round(amt);
            category = cat;
            counterparty = cp;
        }
        else if (string.Equals(action, GameActionCatalog.JumatBerkah, StringComparison.OrdinalIgnoreCase) &&
                 _payloadReader.TryReadAmount(request.Payload, out var donationAmount))
        {
            direction = "OUT";
            amount = (int)Math.Round(donationAmount);
            category = "DONATION";
        }
        else if ((string.Equals(action, GameActionCatalog.InvestasiEmas, StringComparison.OrdinalIgnoreCase) ||
                  string.Equals(action, GameActionCatalog.JualEmas, StringComparison.OrdinalIgnoreCase)) &&
                 TryReadGoldAmount(request.Payload, out var tradeAmount))
        {
            direction = string.Equals(action, GameActionCatalog.JualEmas, StringComparison.OrdinalIgnoreCase) ? "IN" : "OUT";
            amount = tradeAmount;
            category = "GOLD_TRADE";
        }
        else if (string.Equals(action, GameActionCatalog.BahanMasakan, StringComparison.OrdinalIgnoreCase) &&
                 _payloadReader.TryReadIngredientPurchase(request.Payload, out _, out var ingredientAmount))
        {
            direction = "OUT";
            amount = ingredientAmount;
            category = "INGREDIENT";
        }
        else if (string.Equals(action, GameActionCatalog.JualMasakan, StringComparison.OrdinalIgnoreCase) &&
                 _payloadReader.TryReadOrderClaim(request.Payload, out _, out var income))
        {
            direction = "IN";
            amount = income;
            category = "ORDER";
        }
        else if (string.Equals(action, GameActionCatalog.KerjaLepas, StringComparison.OrdinalIgnoreCase) &&
                 _payloadReader.TryReadAmount(request.Payload, out var freelanceAmount))
        {
            direction = "IN";
            amount = (int)Math.Round(freelanceAmount);
            category = "FREELANCE";
        }
        else if (string.Equals(action, GameActionCatalog.Kebutuhan, StringComparison.OrdinalIgnoreCase) &&
                 _payloadReader.TryReadNeedPurchase(request.Payload, out var needCardId, out var needAmount, out _))
        {
            direction = "OUT";
            amount = needAmount;
            category = NeedTierClassifier.FromPayload(request.Payload, needCardId) switch
            {
                NeedTier.Primary => "NEED_PRIMARY",
                NeedTier.Secondary => "NEED_SECONDARY",
                NeedTier.Tertiary => "NEED_TERTIARY",
                _ => "NEED"
            };
        }
        else if (string.Equals(action, GameActionCatalog.Menabung, StringComparison.OrdinalIgnoreCase) &&
                 _payloadReader.TryReadSavingDeposit(request.Payload, out _, out var savingAmount))
        {
            direction = "OUT";
            amount = savingAmount;
            category = "SAVING_DEPOSIT";
        }
        else if (string.Equals(action, GameActionCatalog.SavingDepositWithdrawn, StringComparison.OrdinalIgnoreCase) &&
                 _payloadReader.TryReadSavingDeposit(request.Payload, out _, out var savingWithdrawAmount))
        {
            direction = "IN";
            amount = savingWithdrawAmount;
            category = "SAVING_WITHDRAW";
        }
        else if (string.Equals(action, GameActionCatalog.RisikoKehidupan, StringComparison.OrdinalIgnoreCase) &&
                 _payloadReader.TryReadRiskLife(request.Payload, out _, out var riskDirection, out var riskAmount))
        {
            direction = riskDirection.ToUpperInvariant();
            amount = riskAmount;
            category = "RISK_LIFE";
        }
        else if ((string.Equals(action, GameActionCatalog.PinjamanSyariah, StringComparison.OrdinalIgnoreCase) ||
                  string.Equals(action, GameActionCatalog.SetupPinjamanAwal, StringComparison.OrdinalIgnoreCase)) &&
                 _payloadReader.TryReadLoanTaken(request.Payload, out _, out var principal, out _, out _, out _))
        {
            direction = "IN";
            amount = principal;
            category = "LOAN_TAKEN";
        }
        else if (string.Equals(action, GameActionCatalog.BayarPinjaman, StringComparison.OrdinalIgnoreCase) &&
                 _payloadReader.TryReadLoanRepay(request.Payload, out _, out var repayAmount))
        {
            direction = "OUT";
            amount = repayAmount;
            category = "LOAN_REPAID";
        }
        else if (string.Equals(action, GameActionCatalog.Asuransi, StringComparison.OrdinalIgnoreCase) &&
                 _payloadReader.TryReadInsurance(request.Payload, out var premium))
        {
            direction = "OUT";
            amount = premium;
            category = "INSURANCE_PREMIUM";
        }
        else if (string.Equals(action, "GunakanOpsiDarurat", StringComparison.OrdinalIgnoreCase) &&
                 _payloadReader.TryReadEmergencyOption(request.Payload, out _, out var emergencyOption, out var emergencyDirection, out var emergencyAmount) &&
                 emergencyOption.ToUpperInvariant() is "SELL_NEED" or "SELL_GOLD" or "TAKE_SHARIA_LOAN")
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
            UserId = playerId,
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

    private static bool TryReadGoldAmount(JsonElement payload, out int amount)
    {
        amount = 0;
        if (_payloadReader.TryReadGoldTrade(payload, out _, out _, out _, out amount))
        {
            return true;
        }

        return _payloadReader.TryGetInt32(payload, "qty", out var qty) &&
               _payloadReader.TryGetInt32(payload, "unit_price", out var unitPrice) &&
               _payloadReader.TryGetInt32(payload, "amount", out amount) &&
               qty > 0 &&
               unitPrice > 0;
    }
}
