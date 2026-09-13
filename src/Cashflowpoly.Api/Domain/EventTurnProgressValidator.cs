// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui EventTurnProgressValidator.
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;
// Mengimpor namespace `Cashflowpoly.Api.Contracts` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Contracts;
// Mengimpor namespace `Microsoft.AspNetCore.Http` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.AspNetCore.Http;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Domain;

internal sealed class EventTurnProgressValidator : IEventTurnProgressValidator
{
    private static readonly EventPayloadReader _payloadReader = new();

    public bool RequiresHistory(EventRequest request, RulesetConfig config)
    {
        return GameActionCatalog.Is(request.ActionType, request.Payload, GameActionCatalog.AkhirGiliran);
    }

    public bool TryValidate(
        // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        EventRequest request,
        // Parameter `config` bertipe `RulesetConfig` membawa konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan.
        RulesetConfig config,
        // Parameter `history` bertipe `IEnumerable<EventDb>` membawa nilai history.
        IEnumerable<EventDb> history,
        // Parameter `participantCount` bertipe `int` membawa nilai participant jumlah.
        int participantCount,
        // Parameter `result` bertipe `EventDomainValidationResult` membawa nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya; out
        // mengembalikan nilai melalui parameter dan harus diisi oleh metode.
        out EventDomainValidationResult result)
    {
        if (GameActionCatalog.Is(request.ActionType, request.Payload, GameActionCatalog.AkhirGiliran))
        {
            result = ValidateTurnEnded(request, config, history, participantCount);
            return true;
        }

        result = EventDomainValidationResult.Valid;
        return false;
    }

    private EventDomainValidationResult ValidateTurnEnded(
        // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        EventRequest request,
        // Parameter `config` bertipe `RulesetConfig` membawa konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan.
        RulesetConfig config,
        // Parameter `history` bertipe `IEnumerable<EventDb>` membawa nilai history.
        IEnumerable<EventDb> history,
        // Parameter `participantCount` bertipe `int` membawa nilai participant jumlah.
        int participantCount)
    {
        var sessionEvents = history
            .Where(e => e.SessionId == request.SessionId)
            .ToList();
        var turnEvents = sessionEvents
            .Where(e => e.SessionId == request.SessionId &&
                        e.DayIndex == request.DayIndex &&
                        e.UserId.HasValue)
            .ToList();
        var consumingActions = turnEvents
            .Where(e => string.Equals(e.ActorType, "PLAYER", StringComparison.OrdinalIgnoreCase) &&
                        GameActionCatalog.GetPlayerActionSlotPolicy(
                            e.ActionType,
                            _payloadReader.ReadPayload(string.IsNullOrWhiteSpace(e.Payload) ? "{}" : e.Payload)) == PlayerActionSlotPolicy.Consumes)
            .ToList();

        if (IsRegularActionWeekday(request.Weekday))
        {
            var participantIds = consumingActions
                .Where(e => e.UserId.HasValue)
                .Select(e => e.UserId!.Value)
                .Distinct()
                .ToList();
            if (participantIds.Count != participantCount || participantIds.Any(playerId => consumingActions
                    .Where(e => e.UserId == playerId)
                    .Select(e => e.ActionSlot)
                    .Distinct()
                    .Count() != config.ActionsPerTurn))
            {
                return EventDomainValidationResult.Fail(
                    StatusCodes.Status422UnprocessableEntity,
                    "DOMAIN_RULE_VIOLATION",
                    "Setiap pemain harus menyelesaikan seluruh jatah aksi sebelum giliran berakhir");
            }
        }
        else if (request.Weekday.Equals("FRI", StringComparison.OrdinalIgnoreCase))
        {
            var donations = turnEvents
                .Where(e => GameActionCatalog.Is(e.ActionType, _payloadReader.ReadPayload(e.Payload), GameActionCatalog.JumatBerkah))
                .ToList();
            if (donations.Select(e => e.UserId!.Value).Distinct().Count() != participantCount ||
                donations.GroupBy(e => e.UserId!.Value).Any(group => group.Count() != 1))
            {
                return EventDomainValidationResult.Fail(
                    StatusCodes.Status422UnprocessableEntity,
                    "DOMAIN_RULE_VIOLATION",
                    "Setiap pemain harus menyelesaikan tepat satu donasi Jumat sebelum hari berakhir");
            }
        }
        else if (request.Weekday.Equals("SAT", StringComparison.OrdinalIgnoreCase))
        {
            var hasGoldPrice = sessionEvents.Any(e => e.DayIndex == request.DayIndex &&
                GameActionCatalog.Is(e.ActionType, _payloadReader.ReadPayload(e.Payload), GameActionCatalog.GoldPriceOpened));
            var decisions = turnEvents
                .Where(e => IsScheduledGoldDecision(e.ActionType, _payloadReader.ReadPayload(e.Payload)))
                .ToList();
            if (!hasGoldPrice ||
                decisions.Select(e => e.UserId!.Value).Distinct().Count() != participantCount ||
                decisions.GroupBy(e => e.UserId!.Value).Any(group => group.Count() != 1))
            {
                return EventDomainValidationResult.Fail(
                    StatusCodes.Status422UnprocessableEntity,
                    "DOMAIN_RULE_VIOLATION",
                    "Harga emas harus dibuka dan setiap pemain harus memilih beli, jual, atau lewati tepat satu kali pada Sabtu");
            }
        }

        var hasUsed = request.Payload.TryGetProperty("used", out _);
        var hasRemaining = request.Payload.TryGetProperty("remaining", out _);
        if (hasUsed || hasRemaining)
        {
            if (!hasUsed || !hasRemaining ||
                !_payloadReader.TryReadActionUsed(request.Payload, out var used, out var remaining) ||
                used != consumingActions.Count ||
                remaining != 0)
            {
                return EventDomainValidationResult.Fail(
                    StatusCodes.Status422UnprocessableEntity,
                    "DOMAIN_RULE_VIOLATION",
                    "Nilai used/remaining tidak sesuai dengan riwayat aksi");
            }
        }

        var hasFromDay = request.Payload.TryGetProperty("from_day", out _);
        var hasToDay = request.Payload.TryGetProperty("to_day", out _);
        if (hasFromDay || hasToDay)
        {
            if (!_payloadReader.TryGetInt32(request.Payload, "from_day", out var fromDay) ||
                !_payloadReader.TryGetInt32(request.Payload, "to_day", out var toDay) ||
                fromDay != request.DayIndex ||
                toDay != request.DayIndex + 1)
            {
                return EventDomainValidationResult.Fail(
                    StatusCodes.Status422UnprocessableEntity,
                    "DOMAIN_RULE_VIOLATION",
                    "Transisi hari pada AkhirGiliran tidak sesuai");
            }
        }

        if (request.Payload.TryGetProperty("completed_players", out _))
        {
            if (!_payloadReader.TryGetInt32(request.Payload, "completed_players", out var completedPlayers) ||
                completedPlayers != participantCount)
            {
                return EventDomainValidationResult.Fail(
                    StatusCodes.Status422UnprocessableEntity,
                    "DOMAIN_RULE_VIOLATION",
                    "Jumlah completed_players tidak sesuai peserta sesi");
            }
        }

        if (!string.Equals(config.Mode, "MAHIR", StringComparison.OrdinalIgnoreCase))
        {
            return EventDomainValidationResult.Valid;
        }

        var orderCounts = turnEvents
            .Where(e => GameActionCatalog.Is(e.ActionType, _payloadReader.ReadPayload(e.Payload), GameActionCatalog.JualMasakan))
            .GroupBy(e => e.UserId!.Value)
            .ToDictionary(g => g.Key, g => g.Count());

        var riskCounts = turnEvents
            .Where(e => GameActionCatalog.Is(e.ActionType, _payloadReader.ReadPayload(e.Payload), GameActionCatalog.RisikoKehidupan))
            .GroupBy(e => e.UserId!.Value)
            .ToDictionary(g => g.Key, g => g.Count());

        // Mengulangi setiap elemen `orderCounts.Keys.Union(riskCounts.Keys)`; elemen saat ini disimpan sebagai `playerId` bertipe `var` untuk diproses oleh
        // badan loop dalam ValidateTurnEnded.
        foreach (var playerId in orderCounts.Keys.Union(riskCounts.Keys))
        {
            orderCounts.TryGetValue(playerId, out var orders);
            riskCounts.TryGetValue(playerId, out var risks);
            if (orders != risks)
            {
                return EventDomainValidationResult.Fail(
                    StatusCodes.Status422UnprocessableEntity,
                    "DOMAIN_RULE_VIOLATION",
                    "Setiap klaim pesanan harus diikuti pengambilan risiko pada mode MAHIR");
            }
        }

        return EventDomainValidationResult.Valid;
    }

    private static bool IsRegularActionWeekday(string weekday)
        => weekday.Equals("MON", StringComparison.OrdinalIgnoreCase) ||
           weekday.Equals("TUE", StringComparison.OrdinalIgnoreCase) ||
           weekday.Equals("WED", StringComparison.OrdinalIgnoreCase) ||
           weekday.Equals("THU", StringComparison.OrdinalIgnoreCase);

    private static bool IsScheduledGoldDecision(string actionType, System.Text.Json.JsonElement payload)
    {
        var isDecision = GameActionCatalog.Is(actionType, payload, GameActionCatalog.InvestasiEmas) ||
                         GameActionCatalog.Is(actionType, payload, GameActionCatalog.JualEmas) ||
                         GameActionCatalog.Is(actionType, payload, GameActionCatalog.GoldSkipped);
        return isDecision && !(payload.TryGetProperty("risk_event_id", out var riskEventId) &&
                               riskEventId.ValueKind == System.Text.Json.JsonValueKind.String &&
                               Guid.TryParse(riskEventId.GetString(), out _));
    }

}
