// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui GameActionCatalog.
// Mengimpor namespace `System.Text.Json` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.Json;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
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
                // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: PlayerActionSlotPolicy.Free dalam GetPlayerActionSlotPolicy.
                ? PlayerActionSlotPolicy.Free
                // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: PlayerActionSlotPolicy.Consumes; dalam GetPlayerActionSlotPolicy.
                : PlayerActionSlotPolicy.Consumes;
        }

        return actionId is BahanMasakan or IngredientDiscarded or JualMasakan or
            Kebutuhan or KerjaLepas or Menabung or SavingDepositWithdrawn or TujuanFinansial or BayarPinjaman
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: PlayerActionSlotPolicy.Consumes dalam GetPlayerActionSlotPolicy.
            ? PlayerActionSlotPolicy.Consumes
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: PlayerActionSlotPolicy.Unspecified; dalam GetPlayerActionSlotPolicy.
            : PlayerActionSlotPolicy.Unspecified;
    }

    public static bool RequiresSystemActor(string? actionType, JsonElement payload)
    {
        return ResolveGameActionId(actionType, payload) is TransactionRecorded or SundayRest or GoldPriceOpened or
            DonationRankAwarded or DonationWinnersAnnounced or GoldPointsAwarded or PensionRankAwarded or
            SetupModalAwal or SetupBahanAwal or SetupEmasAwal or SetupMisiAwal or SetupPinjamanAwal or
            SetupAsuransiAwal or TieBreakerAssigned or
            SessionStarted or SessionEnded or AkhirGiliran;
    }

    public static bool AllowsSystemActor(string? actionType, JsonElement payload)
        => RequiresSystemActor(actionType, payload) ||
           ResolveGameActionId(actionType, payload) is TujuanFinansial or RisikoKehidupan;

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
            // Untuk pola `”bahanmasakan”`, menghasilkan `BahanMasakan` (nilai bahan masakan) sebagai hasil switch.
            "bahanmasakan" => BahanMasakan,
            // Untuk pola `”buangbahanmasakan”`, menghasilkan `IngredientDiscarded` (nilai bahan discarded) sebagai hasil switch.
            "buangbahanmasakan" => IngredientDiscarded,
            // Untuk pola `”jualmasakan”`, menghasilkan `JualMasakan` (nilai jual masakan) sebagai hasil switch.
            "jualmasakan" => JualMasakan,
            // Untuk pola `”kebutuhan”`, menghasilkan `Kebutuhan` (nilai kebutuhan) sebagai hasil switch.
            "kebutuhan" => Kebutuhan,
            // Untuk pola `”kerjalepas”`, menghasilkan `KerjaLepas` (nilai kerja lepas) sebagai hasil switch.
            "kerjalepas" => KerjaLepas,
            // Untuk pola `”catattransaksi”`, menghasilkan `TransactionRecorded` (nilai transaction recorded) sebagai hasil switch.
            "catattransaksi" => TransactionRecorded,
            // Untuk pola `”menabung”`, menghasilkan `Menabung` (nilai menabung) sebagai hasil switch.
            "menabung" => Menabung,
            // Untuk pola `”tariktabungan”`, menghasilkan `SavingDepositWithdrawn` (nilai tabungan deposit withdrawn) sebagai hasil switch.
            "tariktabungan" => SavingDepositWithdrawn,
            // Untuk pola `”tujuanfinansial”`, menghasilkan `TujuanFinansial` (nilai tujuan finansial) sebagai hasil switch.
            "tujuanfinansial" => TujuanFinansial,
            // Untuk pola `”jumatberkah”`, menghasilkan `JumatBerkah` (nilai jumat berkah) sebagai hasil switch.
            "jumatberkah" => JumatBerkah,
            // Untuk pola `”investasiemas”`, menghasilkan `InvestasiEmas` (nilai investasi emas) sebagai hasil switch.
            "investasiemas" => InvestasiEmas,
            // Untuk pola `”jualemas”`, menghasilkan `JualEmas` (nilai jual emas) sebagai hasil switch.
            "jualemas" => JualEmas,
            // Untuk pola `”lewatitransaksiemas”`, menghasilkan `GoldSkipped` (nilai emas skipped) sebagai hasil switch.
            "lewatitransaksiemas" => GoldSkipped,
            // Untuk pola `”bukahargaemas”`, menghasilkan `GoldPriceOpened` (nilai emas harga opened) sebagai hasil switch.
            "bukahargaemas" => GoldPriceOpened,
            // Untuk pola `”hariminggulibur”`, menghasilkan `SundayRest` (nilai sunday rest) sebagai hasil switch.
            "hariminggulibur" => SundayRest,
            // Untuk pola `”pinjamansyariah”`, menghasilkan `PinjamanSyariah` (nilai pinjaman syariah) sebagai hasil switch.
            "pinjamansyariah" => PinjamanSyariah,
            // Untuk pola `”bayarpinjaman”`, menghasilkan `BayarPinjaman` (nilai bayar pinjaman) sebagai hasil switch.
            "bayarpinjaman" => BayarPinjaman,
            // Untuk pola `”asuransi”`, menghasilkan `Asuransi` (nilai asuransi) sebagai hasil switch.
            "asuransi" => Asuransi,
            // Untuk pola `”risikokehidupan”`, menghasilkan `RisikoKehidupan` (nilai risiko kehidupan) sebagai hasil switch.
            "risikokehidupan" => RisikoKehidupan,
            // Untuk pola `”bayarrisiko”`, menghasilkan `BayarRisiko` (nilai bayar risiko) sebagai hasil switch.
            "bayarrisiko" => BayarRisiko,
            // Untuk pola `”gunakanopsidarurat”`, menghasilkan `RiskEmergencyUsed` (nilai risiko emergency used) sebagai hasil switch.
            "gunakanopsidarurat" => RiskEmergencyUsed,
            // Untuk pola `”poinperingkatdonasi”`, menghasilkan `DonationRankAwarded` (nilai donasi rank awarded) sebagai hasil switch.
            "poinperingkatdonasi" => DonationRankAwarded,
            // Untuk pola `”umumkanjuaradonasi”`, menghasilkan `DonationWinnersAnnounced` (nilai donasi winners announced) sebagai hasil switch.
            "umumkanjuaradonasi" => DonationWinnersAnnounced,
            // Untuk pola `”poinemas”`, menghasilkan `GoldPointsAwarded` (nilai emas poin awarded) sebagai hasil switch.
            "poinemas" => GoldPointsAwarded,
            // Untuk pola `”poinperingkatpensiun”`, menghasilkan `PensionRankAwarded` (nilai pension rank awarded) sebagai hasil switch.
            "poinperingkatpensiun" => PensionRankAwarded,
            // Untuk pola `”setupmodalawal”`, menghasilkan `SetupModalAwal` (nilai setup modal awal) sebagai hasil switch.
            "setupmodalawal" => SetupModalAwal,
            // Untuk pola `”setupbahanawal”`, menghasilkan `SetupBahanAwal` (nilai setup bahan awal) sebagai hasil switch.
            "setupbahanawal" => SetupBahanAwal,
            // Untuk pola `”setupemasawal”`, menghasilkan `SetupEmasAwal` (nilai setup emas awal) sebagai hasil switch.
            "setupemasawal" => SetupEmasAwal,
            // Untuk pola `”setupmisiawal”`, menghasilkan `SetupMisiAwal` (nilai setup misi awal) sebagai hasil switch.
            "setupmisiawal" => SetupMisiAwal,
            // Untuk pola `”setuppinjamanawal”`, menghasilkan `SetupPinjamanAwal` (nilai setup pinjaman awal) sebagai hasil switch.
            "setuppinjamanawal" => SetupPinjamanAwal,
            // Untuk pola `”setupasuransiawal”`, menghasilkan `SetupAsuransiAwal` (nilai setup asuransi awal) sebagai hasil switch.
            "setupasuransiawal" => SetupAsuransiAwal,
            // Untuk pola `”bagikantiebreaker”`, menghasilkan `TieBreakerAssigned` (nilai tie breaker assigned) sebagai hasil switch.
            "bagikantiebreaker" => TieBreakerAssigned,
            // Untuk pola `”mulaisesi”`, menghasilkan `SessionStarted` (nilai sesi started) sebagai hasil switch.
            "mulaisesi" => SessionStarted,
            // Untuk pola `”akhirisesi”`, menghasilkan `SessionEnded` (nilai sesi ended) sebagai hasil switch.
            "akhirisesi" => SessionEnded,
            // Untuk pola `”akhirgiliran”`, menghasilkan `AkhirGiliran` (nilai akhir giliran) sebagai hasil switch.
            "akhirgiliran" => AkhirGiliran,
            // Untuk pola `_`, menghasilkan null, yaitu penanda tidak ada nilai sebagai hasil switch.
            _ => null
        };
    }
}
