// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui EventCashflowProjectionBuilder.
// Mengimpor namespace `System.Diagnostics.CodeAnalysis` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Diagnostics.CodeAnalysis;
// Mengimpor namespace `System.Text.Json` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.Json;
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;
// Mengimpor namespace `Cashflowpoly.Api.Contracts` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Contracts;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Domain;

/// <summary>
/// Builder proyeksi cashflow dari event gameplay.
/// </summary>
// Mendefinisikan tipe class `EventCashflowProjectionBuilder` yang mewarisi atau menerapkan `IEventCashflowProjectionBuilder`; sealed mencegah tipe
// ini diturunkan lagi.
internal sealed class EventCashflowProjectionBuilder : IEventCashflowProjectionBuilder
{
    private static readonly EventPayloadReader _payloadReader = new();

    /// <summary>
    /// Mencoba membangun proyeksi arus kas dari event yang berdampak pada saldo pemain.
    /// </summary>
    // Mendefinisikan metode `TryBuild` dengan hasil bertipe `bool`. Mencoba membangun proyeksi arus kas dari event yang berdampak pada saldo pemain.
    // Masukan: Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan; Parameter
    // `timestamp` bertipe `DateTimeOffset` membawa waktu kejadian yang menjaga urutan kronologis data; Parameter `eventPk` bertipe `Guid` membawa nilai
    // event pk; Parameter `projection` bertipe `CashflowProjectionDb?` membawa nilai projection; nilai null diizinkan ketika data opsional belum
    // tersedia; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; menerapkan metadata `NotNullWhen(true)` pada deklarasi berikut
    // agar framework/compiler dapat mengenali pengaturannya.
    public bool TryBuild(
        // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        EventRequest request,
        // Parameter `timestamp` bertipe `DateTimeOffset` membawa waktu kejadian yang menjaga urutan kronologis data.
        DateTimeOffset timestamp,
        // Parameter `eventPk` bertipe `Guid` membawa nilai event pk.
        Guid eventPk,
        // Parameter `projection` bertipe `CashflowProjectionDb?` membawa nilai projection; nilai null diizinkan ketika data opsional belum tersedia; out
        // mengembalikan nilai melalui parameter dan harus diisi oleh metode; menerapkan metadata `NotNullWhen(true)` pada deklarasi berikut agar
        // framework/compiler dapat mengenali pengaturannya.
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
                // Untuk pola `NeedTier.Primary`, menghasilkan nilai literal `”NEED_PRIMARY”` sebagai hasil switch.
                NeedTier.Primary => "NEED_PRIMARY",
                // Untuk pola `NeedTier.Secondary`, menghasilkan nilai literal `”NEED_SECONDARY”` sebagai hasil switch.
                NeedTier.Secondary => "NEED_SECONDARY",
                // Untuk pola `NeedTier.Tertiary`, menghasilkan nilai literal `”NEED_TERTIARY”` sebagai hasil switch.
                NeedTier.Tertiary => "NEED_TERTIARY",
                // Untuk pola `_`, menghasilkan nilai literal `”NEED”` sebagai hasil switch.
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
