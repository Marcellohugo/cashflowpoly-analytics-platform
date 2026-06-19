using System.Text.Json;

namespace Cashflowpoly.Api.Domain;

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
    public const string SundayRest = "HariMingguLibur";
    public const string PinjamanSyariah = "PinjamanSyariah";
    public const string BayarPinjaman = "BayarPinjaman";
    public const string Asuransi = "Asuransi";
    public const string RisikoKehidupan = "RisikoKehidupan";
    public const string RiskEmergencyUsed = "GunakanOpsiDarurat";
    public const string DonationRankAwarded = "PoinPeringkatDonasi";
    public const string DonationWinnersAnnounced = "UmumkanJuaraDonasi";
    public const string GoldPointsAwarded = "PoinEmas";
    public const string PensionRankAwarded = "PoinPeringkatPensiun";
    public const string GoldInitialGranted = "BagikanEmasAwal";
    public const string TieBreakerAssigned = "BagikanTieBreaker";
    public const string MissionAssigned = "BagikanMisiKoleksi";
    public const string CardDrawn = "AmbilKartuDariDeck";
    public const string CardTaken = "KartuDiambilDariPasar";
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
            "hariminggulibur" => SundayRest,
            "pinjamansyariah" => PinjamanSyariah,
            "bayarpinjaman" => BayarPinjaman,
            "asuransi" => Asuransi,
            "risikokehidupan" => RisikoKehidupan,
            "gunakanopsidarurat" => RiskEmergencyUsed,
            "poinperingkatdonasi" => DonationRankAwarded,
            "umumkanjuaradonasi" => DonationWinnersAnnounced,
            "poinemas" => GoldPointsAwarded,
            "poinperingkatpensiun" => PensionRankAwarded,
            "bagikanemasawal" => GoldInitialGranted,
            "bagikantiebreaker" => TieBreakerAssigned,
            "bagikanmisikoleksi" => MissionAssigned,
            "ambilkartudarideck" => CardDrawn,
            "kartudiambildaripasar" => CardTaken,
            "kartumasukdiscard" => CardDiscarded,
            "isiulangpasar" => MarketRefilled,
            "mulaisesi" => SessionStarted,
            "akhirisesi" => SessionEnded,
            "akhirgiliran" => AkhirGiliran,
            _ => null
        };
    }
}
