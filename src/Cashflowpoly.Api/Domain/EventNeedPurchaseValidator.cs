// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui EventNeedPurchaseValidator.
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;
// Mengimpor namespace `Cashflowpoly.Api.Contracts` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Contracts;
// Mengimpor namespace `Microsoft.AspNetCore.Http` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.AspNetCore.Http;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Domain;

public sealed record EventNeedPurchaseValidation(
    // Parameter `Validation` bertipe `EventDomainValidationResult` membawa nilai validasi.
    EventDomainValidationResult Validation,
    // Parameter `OutgoingAmount` bertipe `int?` membawa nilai outgoing nominal; nilai null diizinkan ketika data opsional belum tersedia.
    int? OutgoingAmount);

internal sealed class EventNeedPurchaseValidator : IEventNeedPurchaseValidator
{
    private static readonly EventPayloadReader _payloadReader = new();

    public bool TryValidate(
        // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        EventRequest request,
        // Parameter `config` bertipe `RulesetConfig` membawa konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan.
        RulesetConfig config,
        // Parameter `history` bertipe `IEnumerable<EventDb>` membawa nilai history.
        IEnumerable<EventDb> history,
        // Parameter `result` bertipe `EventNeedPurchaseValidation` membawa nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya; out
        // mengembalikan nilai melalui parameter dan harus diisi oleh metode.
        out EventNeedPurchaseValidation result)
    {
        if (!GameActionCatalog.Is(request.ActionType, request.Payload, GameActionCatalog.Kebutuhan))
        {
            result = new EventNeedPurchaseValidation(EventDomainValidationResult.Valid, null);
            return false;
        }

        if (!_payloadReader.TryReadNeedPurchase(request.Payload, out var cardId, out var amount, out var points))
        {
            var payloadValidation = ValidatePayload(request, primary: false, out _, out _);
            result = new EventNeedPurchaseValidation(payloadValidation, null);
            return true;
        }

        var needTier = NeedTierClassifier.FromPayload(request.Payload, cardId);
        if (config.Needs.Count > 0)
        {
            var catalogItem = config.Needs.FirstOrDefault(item =>
                string.Equals(item.Id, cardId, StringComparison.OrdinalIgnoreCase));
            if (catalogItem is null)
            {
                result = Fail(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION",
                    "Kartu kebutuhan tidak ditemukan pada ruleset");
                return true;
            }

            var catalogTier = catalogItem.Tipe.Trim().ToLowerInvariant() switch
            {
                // Untuk pola `”primer” or ”primary”`, menghasilkan `NeedTier.Primary` (nilai primary) sebagai hasil switch.
                "primer" or "primary" => NeedTier.Primary,
                // Untuk pola `”sekunder” or ”secondary”`, menghasilkan `NeedTier.Secondary` (nilai secondary) sebagai hasil switch.
                "sekunder" or "secondary" => NeedTier.Secondary,
                // Untuk pola `”tersier” or ”tertiary”`, menghasilkan `NeedTier.Tertiary` (nilai tertiary) sebagai hasil switch.
                "tersier" or "tertiary" => NeedTier.Tertiary,
                // Untuk pola `_`, menghasilkan `NeedTier.Unknown` (nilai unknown) sebagai hasil switch.
                _ => NeedTier.Unknown
            };
            if (amount != catalogItem.HargaBeli || points != catalogItem.PoinKebahagiaan || needTier != catalogTier)
            {
                result = Fail(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION",
                    "Harga, poin, atau jenis kebutuhan tidak sesuai katalog ruleset");
                return true;
            }
        }

        result = needTier == NeedTier.Primary
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: ValidatePrimary(request, config, history) dalam TryValidate.
            ? ValidatePrimary(request, config, history)
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: ValidateSecondaryOrTertiary(request, config, history); dalam
            // TryValidate.
            : ValidateSecondaryOrTertiary(request, config, history);
        return true;
    }

    private EventNeedPurchaseValidation ValidatePrimary(
        // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        EventRequest request,
        // Parameter `config` bertipe `RulesetConfig` membawa konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan.
        RulesetConfig config,
        // Parameter `history` bertipe `IEnumerable<EventDb>` membawa nilai history.
        IEnumerable<EventDb> history)
    {
        var payloadValidation = ValidatePayload(request, primary: true, out _, out var amount);
        if (!payloadValidation.IsValid)
        {
            return new EventNeedPurchaseValidation(payloadValidation, null);
        }

        if (request.UserId is null)
        {
            return Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Player wajib diisi",
                new ErrorDetail("user_id", "REQUIRED"));
        }

        var primaryCount = history.Count(e =>
            e.UserId == request.UserId &&
            e.DayIndex == request.DayIndex &&
            GameActionCatalog.Is(e.ActionType, _payloadReader.ReadPayload(e.Payload), GameActionCatalog.Kebutuhan) &&
            NeedTierClassifier.FromPayloadJson(e.Payload) == NeedTier.Primary);

        if (config.PrimaryNeedMaxPerDay is > 0 && primaryCount >= config.PrimaryNeedMaxPerDay.Value)
        {
            return Fail(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Pembelian kebutuhan primer melebihi batas harian");
        }

        return new EventNeedPurchaseValidation(EventDomainValidationResult.Valid, amount);
    }

    private EventNeedPurchaseValidation ValidateSecondaryOrTertiary(
        // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        EventRequest request,
        // Parameter `config` bertipe `RulesetConfig` membawa konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan.
        RulesetConfig config,
        // Parameter `history` bertipe `IEnumerable<EventDb>` membawa nilai history.
        IEnumerable<EventDb> history)
    {
        var payloadValidation = ValidatePayload(request, primary: false, out _, out var amount);
        if (!payloadValidation.IsValid)
        {
            return new EventNeedPurchaseValidation(payloadValidation, null);
        }

        if (config.RequirePrimaryBeforeOthers && request.UserId is not null)
        {
            var hasPrimary = history.Any(e =>
                e.UserId == request.UserId &&
                GameActionCatalog.Is(e.ActionType, _payloadReader.ReadPayload(e.Payload), GameActionCatalog.Kebutuhan) &&
                NeedTierClassifier.FromPayloadJson(e.Payload) == NeedTier.Primary);

            if (!hasPrimary)
            {
                return Fail(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Kebutuhan primer harus dibeli terlebih dahulu");
            }
        }

        return new EventNeedPurchaseValidation(EventDomainValidationResult.Valid, request.UserId is null ? null : amount);
    }

    private EventDomainValidationResult ValidatePayload(EventRequest request, bool primary, out string cardId, out int amount)
    {
        cardId = string.Empty;
        amount = 0;
        if (!_payloadReader.TryReadNeedPurchase(request.Payload, out cardId, out amount, out var points))
        {
            return EventDomainValidationResult.Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                primary ? "Payload kebutuhan primer tidak valid" : "Payload kebutuhan tidak valid",
                new ErrorDetail("payload.card_id", "REQUIRED"));
        }

        if (amount <= 0)
        {
            return EventDomainValidationResult.Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Amount harus > 0",
                new ErrorDetail("payload.amount", "OUT_OF_RANGE"));
        }

        if (string.IsNullOrWhiteSpace(cardId))
        {
            return EventDomainValidationResult.Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Card ID wajib diisi",
                new ErrorDetail("payload.card_id", "REQUIRED"));
        }

        if (points < 0)
        {
            return EventDomainValidationResult.Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Points tidak valid",
                new ErrorDetail("payload.points", "OUT_OF_RANGE"));
        }

        if (!request.Payload.TryGetProperty("points", out _))
        {
            return EventDomainValidationResult.Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Points wajib diisi",
                new ErrorDetail("payload.points", "REQUIRED"));
        }

        if (NeedTierClassifier.FromPayload(request.Payload, cardId) == NeedTier.Unknown)
        {
            return EventDomainValidationResult.Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Tipe kebutuhan tidak valid",
                new ErrorDetail("payload.need_tier", "INVALID_ENUM"));
        }

        return EventDomainValidationResult.Valid;
    }

    private EventNeedPurchaseValidation Fail(
        // Parameter `statusCode` bertipe `int` membawa kode status hasil HTTP yang mengomunikasikan keberhasilan atau kegagalan.
        int statusCode,
        // Parameter `errorCode` bertipe `string` membawa nilai kesalahan kode.
        string errorCode,
        // Parameter `message` bertipe `string` membawa nilai pesan.
        string message,
        // Parameter `details` bertipe `ErrorDetail[]` membawa nilai rincian.
        params ErrorDetail[] details)
    {
        return new EventNeedPurchaseValidation(
            EventDomainValidationResult.Fail(statusCode, errorCode, message, details),
            null);
    }
}
