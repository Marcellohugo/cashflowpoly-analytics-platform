// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui EventSimpleActionValidator.
// Mengimpor namespace `Cashflowpoly.Api.Contracts` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Contracts;
// Mengimpor namespace `Microsoft.AspNetCore.Http` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.AspNetCore.Http;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Domain;

internal sealed class EventSimpleActionValidator : IEventSimpleActionValidator
{
    private static readonly EventPayloadReader _payloadReader = new();

    public bool TryValidate(
        // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        EventRequest request,
        // Parameter `config` bertipe `RulesetConfig` membawa konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan.
        RulesetConfig config,
        // Parameter `result` bertipe `EventDomainValidationResult` membawa nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya; out
        // mengembalikan nilai melalui parameter dan harus diisi oleh metode.
        out EventDomainValidationResult result)
    {
        var actionType = request.ActionType;
        var payload = request.Payload;

        if (string.Equals(actionType, "KerjaLepas", StringComparison.OrdinalIgnoreCase))
        {
            result = ValidateFreelanceCompleted(request, payload, config.FreelanceIncome);
            return true;
        }

        if (string.Equals(actionType, "PoinPeringkatDonasi", StringComparison.OrdinalIgnoreCase))
        {
            result = ValidateRankAwarded(request, payload, "Payload donasi tidak valid");
            return true;
        }

        if (string.Equals(actionType, "UmumkanJuaraDonasi", StringComparison.OrdinalIgnoreCase))
        {
            result = ValidateDonationWinnersAnnouncement(request, payload);
            return true;
        }

        if (string.Equals(actionType, "PoinEmas", StringComparison.OrdinalIgnoreCase))
        {
            result = ValidateGoldPointsAwarded(request, payload);
            return true;
        }

        if (string.Equals(actionType, "PoinPeringkatPensiun", StringComparison.OrdinalIgnoreCase))
        {
            result = ValidateRankAwarded(request, payload, "Payload pension tidak valid");
            return true;
        }

        result = EventDomainValidationResult.Valid;
        return false;
    }

    private EventDomainValidationResult ValidateFreelanceCompleted(
        // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        EventRequest request,
        // Parameter `payload` bertipe `System.Text.Json.JsonElement` membawa muatan detail event dalam format JSON.
        System.Text.Json.JsonElement payload,
        // Parameter `expectedIncome` bertipe `int` membawa nilai yang diharapkan pemasukan.
        int expectedIncome)
    {
        var playerCheck = RequirePlayer(request);
        if (!playerCheck.IsValid)
        {
            return playerCheck;
        }

        if (!_payloadReader.TryGetInt32(payload, "amount", out var amount))
        {
            return EventDomainValidationResult.Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Payload kerja lepas tidak valid",
                new ErrorDetail("payload.amount", "REQUIRED"));
        }

        if (amount <= 0)
        {
            return EventDomainValidationResult.Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Amount harus > 0",
                new ErrorDetail("payload.amount", "OUT_OF_RANGE"));
        }

        if (amount != expectedIncome)
        {
            return EventDomainValidationResult.Fail(
                StatusCodes.Status422UnprocessableEntity,
                "DOMAIN_RULE_VIOLATION",
                "Amount kerja lepas tidak sesuai ruleset");
        }

        return EventDomainValidationResult.Valid;
    }

    private EventDomainValidationResult ValidateRankAwarded(
        // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        EventRequest request,
        // Parameter `payload` bertipe `System.Text.Json.JsonElement` membawa muatan detail event dalam format JSON.
        System.Text.Json.JsonElement payload,
        // Parameter `invalidPayloadMessage` bertipe `string` membawa nilai invalid payload pesan.
        string invalidPayloadMessage)
    {
        var playerCheck = RequirePlayer(request);
        if (!playerCheck.IsValid)
        {
            return playerCheck;
        }

        if (!_payloadReader.TryReadRankAwarded(payload, out var rank, out var points))
        {
            return EventDomainValidationResult.Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                invalidPayloadMessage,
                new ErrorDetail("payload.rank", "REQUIRED"));
        }

        if (rank <= 0 || points < 0)
        {
            return EventDomainValidationResult.Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Nilai rank/points tidak valid",
                new ErrorDetail("payload.points", "OUT_OF_RANGE"));
        }

        return EventDomainValidationResult.Valid;
    }

    private static EventDomainValidationResult ValidateDonationWinnersAnnouncement(
        // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        EventRequest request,
        // Parameter `payload` bertipe `System.Text.Json.JsonElement` membawa muatan detail event dalam format JSON.
        System.Text.Json.JsonElement payload)
    {
        if (!string.Equals(request.ActorType, "SYSTEM", StringComparison.OrdinalIgnoreCase) ||
            request.UserId is not null)
        {
            return EventDomainValidationResult.Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Pengumuman juara donasi wajib dibuat oleh sistem",
                new ErrorDetail("actor_type", "SYSTEM_REQUIRED"));
        }

        if (payload.ValueKind != System.Text.Json.JsonValueKind.Object ||
            !payload.TryGetProperty("winners", out var winners) ||
            winners.ValueKind != System.Text.Json.JsonValueKind.Array)
        {
            return EventDomainValidationResult.Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Payload juara donasi tidak valid",
                new ErrorDetail("payload.winners", "REQUIRED"));
        }

        var winnerCount = winners.GetArrayLength();
        if (winnerCount is < 1 or > 3)
        {
            return EventDomainValidationResult.Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Jumlah juara donasi harus 1 sampai 3",
                new ErrorDetail("payload.winners", "OUT_OF_RANGE"));
        }

        var expectedRank = 1;
        // Mengulangi setiap elemen `winners.EnumerateArray()`; elemen saat ini disimpan sebagai `winner` bertipe `var` untuk diproses oleh badan loop dalam
        // ValidateDonationWinnersAnnouncement.
        foreach (var winner in winners.EnumerateArray())
        {
            if (winner.ValueKind != System.Text.Json.JsonValueKind.Object ||
                !_payloadReader.TryGetInt32(winner, "rank", out var rank) ||
                rank != expectedRank)
            {
                return EventDomainValidationResult.Fail(
                    StatusCodes.Status400BadRequest,
                    "VALIDATION_ERROR",
                    "Urutan rank juara donasi tidak valid",
                    new ErrorDetail("payload.winners", "INVALID_RANK_SEQUENCE"));
            }

            if (!winner.TryGetProperty("player_name", out var playerNameProperty) ||
                playerNameProperty.ValueKind != System.Text.Json.JsonValueKind.String ||
                string.IsNullOrWhiteSpace(playerNameProperty.GetString()))
            {
                return EventDomainValidationResult.Fail(
                    StatusCodes.Status400BadRequest,
                    "VALIDATION_ERROR",
                    "Nama pemain juara donasi wajib diisi",
                    new ErrorDetail("payload.winners.player_name", "REQUIRED"));
            }

            if (!_payloadReader.TryGetInt32(winner, "points", out var points) ||
                points < 0)
            {
                return EventDomainValidationResult.Fail(
                    StatusCodes.Status400BadRequest,
                    "VALIDATION_ERROR",
                    "Poin juara donasi tidak valid",
                    new ErrorDetail("payload.winners.points", "OUT_OF_RANGE"));
            }

            if (winner.TryGetProperty("player_order_no", out var playerOrderProperty) &&
                (playerOrderProperty.ValueKind != System.Text.Json.JsonValueKind.Number ||
                 !playerOrderProperty.TryGetInt32(out var playerOrderNo) || playerOrderNo <= 0))
            {
                return EventDomainValidationResult.Fail(
                    StatusCodes.Status400BadRequest,
                    "VALIDATION_ERROR",
                    "Urutan pemain juara donasi tidak valid",
                    new ErrorDetail("payload.winners.player_order_no", "OUT_OF_RANGE"));
            }

            expectedRank++;
        }

        return EventDomainValidationResult.Valid;
    }

    private EventDomainValidationResult ValidateGoldPointsAwarded(
        // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        EventRequest request,
        // Parameter `payload` bertipe `System.Text.Json.JsonElement` membawa muatan detail event dalam format JSON.
        System.Text.Json.JsonElement payload)
    {
        var playerCheck = RequirePlayer(request);
        if (!playerCheck.IsValid)
        {
            return playerCheck;
        }

        if (!_payloadReader.TryReadPointsAwarded(payload, out var points))
        {
            return EventDomainValidationResult.Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Payload gold points tidak valid",
                new ErrorDetail("payload.points", "REQUIRED"));
        }

        if (points < 0)
        {
            return EventDomainValidationResult.Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Points tidak valid",
                new ErrorDetail("payload.points", "OUT_OF_RANGE"));
        }

        return EventDomainValidationResult.Valid;
    }

    private EventDomainValidationResult RequirePlayer(EventRequest request)
    {
        if (request.UserId is not null)
        {
            return EventDomainValidationResult.Valid;
        }

        return EventDomainValidationResult.Fail(
            StatusCodes.Status400BadRequest,
            "VALIDATION_ERROR",
            "Player wajib diisi",
            new ErrorDetail("user_id", "REQUIRED"));
    }
}
