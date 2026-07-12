using System.Text.Json;

namespace Cashflowpoly.Api.Domain;

internal enum PlayerActionSlotPolicy
{
    Unspecified,
    Free,
    Consumes
}

internal static class GameActionCatalog
{
    public const string BahanMasakan = "BahanMasakan";
    public const string IngredientDiscarded = "BuangBahanMasakan";
    public const string JualMasakan = "JualMasakan";
    public const string OrderPassed = "LewatiOrder";
    public const string Kebutuhan = "Kebutuhan";
    public const string KerjaLepas = "KerjaLepas";
    public const string TransactionRecorded = "CatatTransaksi";
    public const string Menabung = "Menabung";
    public const string SavingDepositWithdrawn = "TarikTabungan";
    public const string TujuanFinansial = "TujuanFinansial";
    public const string JumatBerkah = "JumatBerkah";
    public const string InvestasiEmas = "InvestasiEmas";
    public const string JualEmas = "JualEmas";
    public const string GoldSkipped = "LewatiTransaksiEmas";
    public const string GoldPriceOpened = "BukaHargaEmas";
    public const string SundayRest = "HariMingguLibur";
    public const string PinjamanSyariah = "PinjamanSyariah";
    public const string BayarPinjaman = "BayarPinjaman";
    public const string Asuransi = "Asuransi";
    public const string RisikoKehidupan = "RisikoKehidupan";
    public const string BayarRisiko = "BayarRisiko";
    public const string RiskEmergencyUsed = "GunakanOpsiDarurat";
    public const string DonationRankAwarded = "PoinPeringkatDonasi";
    public const string DonationWinnersAnnounced = "UmumkanJuaraDonasi";
    public const string GoldPointsAwarded = "PoinEmas";
    public const string PensionRankAwarded = "PoinPeringkatPensiun";
    public const string SetupModalAwal = "SetupModalAwal";
    public const string SetupBahanAwal = "SetupBahanAwal";
    public const string SetupEmasAwal = "SetupEmasAwal";
    public const string SetupMisiAwal = "SetupMisiAwal";
    public const string SetupPinjamanAwal = "SetupPinjamanAwal";
    public const string SetupAsuransiAwal = "SetupAsuransiAwal";
    public const string TieBreakerAssigned = "BagikanTieBreaker";
    public const string CardDrawn = "AmbilKartuDariDeck";
    public const string CardDiscarded = "KartuMasukDiscard";
    public const string MarketRefilled = "IsiUlangPasar";
    public const string SessionStarted = "MulaiSesi";
    public const string SessionEnded = "AkhiriSesi";
    public const string AkhirGiliran = "AkhirGiliran";

    public static string? ResolveGameActionId(string? actionType, JsonElement payload)
    {
        if (string.IsNullOrWhiteSpace(actionType))
        {
            return null;
        }

        var normalized = actionType.Trim();
        return ResolveCanonicalGameAction(normalized);
    }

    public static bool Is(string? actionType, JsonElement payload, string canonicalActionId)
    {
        return string.Equals(
            ResolveGameActionId(actionType, payload),
            canonicalActionId,
            StringComparison.OrdinalIgnoreCase);
    }

    public static PlayerActionSlotPolicy GetPlayerActionSlotPolicy(string? actionType, JsonElement payload)
    {
        var actionId = ResolveGameActionId(actionType, payload);
        if (actionId is JumatBerkah or RisikoKehidupan or BayarRisiko or RiskEmergencyUsed or
            InvestasiEmas or JualEmas or GoldSkipped)
        {
            return PlayerActionSlotPolicy.Free;
        }

        if (actionId is Asuransi or PinjamanSyariah)
        {
            return HasRiskReference(payload)
                ? PlayerActionSlotPolicy.Free
                : PlayerActionSlotPolicy.Consumes;
        }

        return actionId is BahanMasakan or IngredientDiscarded or JualMasakan or OrderPassed or
            Kebutuhan or KerjaLepas or Menabung or SavingDepositWithdrawn or TujuanFinansial or BayarPinjaman
            ? PlayerActionSlotPolicy.Consumes
            : PlayerActionSlotPolicy.Unspecified;
    }

    public static bool RequiresSystemActor(string? actionType, JsonElement payload)
    {
        return ResolveGameActionId(actionType, payload) is SundayRest or GoldPriceOpened or AkhirGiliran or
            CardDrawn or CardDiscarded or MarketRefilled;
    }

    private static bool HasRiskReference(JsonElement payload)
    {
        if (payload.ValueKind != JsonValueKind.Object)
        {
            return false;
        }

        return payload.TryGetProperty("risk_event_id", out var id) &&
               id.ValueKind == JsonValueKind.String &&
               Guid.TryParse(id.GetString(), out _);
    }

    private static string? ResolveCanonicalGameAction(string actionType)
    {
        return actionType.ToLowerInvariant() switch
        {
            "bahanmasakan" => BahanMasakan,
            "buangbahanmasakan" => IngredientDiscarded,
            "jualmasakan" => JualMasakan,
            "lewatiorder" => OrderPassed,
            "kebutuhan" => Kebutuhan,
            "kerjalepas" => KerjaLepas,
            "catattransaksi" => TransactionRecorded,
            "menabung" => Menabung,
            "tariktabungan" => SavingDepositWithdrawn,
            "tujuanfinansial" => TujuanFinansial,
            "jumatberkah" => JumatBerkah,
            "investasiemas" => InvestasiEmas,
            "jualemas" => JualEmas,
            "lewatitransaksiemas" => GoldSkipped,
            "bukahargaemas" => GoldPriceOpened,
            "hariminggulibur" => SundayRest,
            "pinjamansyariah" => PinjamanSyariah,
            "bayarpinjaman" => BayarPinjaman,
            "asuransi" => Asuransi,
            "risikokehidupan" => RisikoKehidupan,
            "bayarrisiko" => BayarRisiko,
            "gunakanopsidarurat" => RiskEmergencyUsed,
            "poinperingkatdonasi" => DonationRankAwarded,
            "umumkanjuaradonasi" => DonationWinnersAnnounced,
            "poinemas" => GoldPointsAwarded,
            "poinperingkatpensiun" => PensionRankAwarded,
            "setupmodalawal" => SetupModalAwal,
            "setupbahanawal" => SetupBahanAwal,
            "setupemasawal" => SetupEmasAwal,
            "setupmisiawal" => SetupMisiAwal,
            "setuppinjamanawal" => SetupPinjamanAwal,
            "setupasuransiawal" => SetupAsuransiAwal,
            "bagikantiebreaker" => TieBreakerAssigned,
            "ambilkartudarideck" => CardDrawn,
            "kartumasukdiscard" => CardDiscarded,
            "isiulangpasar" => MarketRefilled,
            "mulaisesi" => SessionStarted,
            "akhirisesi" => SessionEnded,
            "akhirgiliran" => AkhirGiliran,
            _ => null
        };
    }
}
