// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui GameActionCatalog.
// Mengimpor namespace `System.Text.Json` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.Json;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Domain;

// Mendefinisikan enum untuk membatasi pilihan nilai bernama `PlayerActionSlotPolicy`.
internal enum PlayerActionSlotPolicy
// Membuka scope tipe PlayerActionSlotPolicy; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan pilihan enum `Unspecified`; nilai bilangan mengikuti urutan deklarasi enum.
    Unspecified,
    // Mendefinisikan pilihan enum `Free`; nilai bilangan mengikuti urutan deklarasi enum.
    Free,
    // Mendefinisikan pilihan enum `Consumes`; nilai bilangan mengikuti urutan deklarasi enum.
    Consumes
// Menutup scope tipe PlayerActionSlotPolicy; bagian berikut berada di luar batas blok tersebut.
}

// Mendefinisikan tipe class `GameActionCatalog`.
internal static class GameActionCatalog
// Membuka scope tipe GameActionCatalog; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendeklarasikan field bertipe `string`: `BahanMasakan` menyimpan nilai bahan masakan dengan nilai awal nilai literal `”BahanMasakan”`.
    public const string BahanMasakan = "BahanMasakan";
    // Mendeklarasikan field bertipe `string`: `IngredientDiscarded` menyimpan nilai bahan discarded dengan nilai awal nilai literal
    // `”BuangBahanMasakan”`.
    public const string IngredientDiscarded = "BuangBahanMasakan";
    // Mendeklarasikan field bertipe `string`: `JualMasakan` menyimpan nilai jual masakan dengan nilai awal nilai literal `”JualMasakan”`.
    public const string JualMasakan = "JualMasakan";
    // Mendeklarasikan field bertipe `string`: `Kebutuhan` menyimpan nilai kebutuhan dengan nilai awal nilai literal `”Kebutuhan”`.
    public const string Kebutuhan = "Kebutuhan";
    // Mendeklarasikan field bertipe `string`: `KerjaLepas` menyimpan nilai kerja lepas dengan nilai awal nilai literal `”KerjaLepas”`.
    public const string KerjaLepas = "KerjaLepas";
    // Mendeklarasikan field bertipe `string`: `TransactionRecorded` menyimpan nilai transaction recorded dengan nilai awal nilai literal
    // `”CatatTransaksi”`.
    public const string TransactionRecorded = "CatatTransaksi";
    // Mendeklarasikan field bertipe `string`: `Menabung` menyimpan nilai menabung dengan nilai awal nilai literal `”Menabung”`.
    public const string Menabung = "Menabung";
    // Mendeklarasikan field bertipe `string`: `SavingDepositWithdrawn` menyimpan nilai tabungan deposit withdrawn dengan nilai awal nilai literal
    // `”TarikTabungan”`.
    public const string SavingDepositWithdrawn = "TarikTabungan";
    // Mendeklarasikan field bertipe `string`: `TujuanFinansial` menyimpan nilai tujuan finansial dengan nilai awal nilai literal `”TujuanFinansial”`.
    public const string TujuanFinansial = "TujuanFinansial";
    // Mendeklarasikan field bertipe `string`: `JumatBerkah` menyimpan nilai jumat berkah dengan nilai awal nilai literal `”JumatBerkah”`.
    public const string JumatBerkah = "JumatBerkah";
    // Mendeklarasikan field bertipe `string`: `InvestasiEmas` menyimpan nilai investasi emas dengan nilai awal nilai literal `”InvestasiEmas”`.
    public const string InvestasiEmas = "InvestasiEmas";
    // Mendeklarasikan field bertipe `string`: `JualEmas` menyimpan nilai jual emas dengan nilai awal nilai literal `”JualEmas”`.
    public const string JualEmas = "JualEmas";
    // Mendeklarasikan field bertipe `string`: `GoldSkipped` menyimpan nilai emas skipped dengan nilai awal nilai literal `”LewatiTransaksiEmas”`.
    public const string GoldSkipped = "LewatiTransaksiEmas";
    // Mendeklarasikan field bertipe `string`: `GoldPriceOpened` menyimpan nilai emas harga opened dengan nilai awal nilai literal `”BukaHargaEmas”`.
    public const string GoldPriceOpened = "BukaHargaEmas";
    // Mendeklarasikan field bertipe `string`: `SundayRest` menyimpan nilai sunday rest dengan nilai awal nilai literal `”HariMingguLibur”`.
    public const string SundayRest = "HariMingguLibur";
    // Mendeklarasikan field bertipe `string`: `PinjamanSyariah` menyimpan nilai pinjaman syariah dengan nilai awal nilai literal `”PinjamanSyariah”`.
    public const string PinjamanSyariah = "PinjamanSyariah";
    // Mendeklarasikan field bertipe `string`: `BayarPinjaman` menyimpan nilai bayar pinjaman dengan nilai awal nilai literal `”BayarPinjaman”`.
    public const string BayarPinjaman = "BayarPinjaman";
    // Mendeklarasikan field bertipe `string`: `Asuransi` menyimpan nilai asuransi dengan nilai awal nilai literal `”Asuransi”`.
    public const string Asuransi = "Asuransi";
    // Mendeklarasikan field bertipe `string`: `RisikoKehidupan` menyimpan nilai risiko kehidupan dengan nilai awal nilai literal `”RisikoKehidupan”`.
    public const string RisikoKehidupan = "RisikoKehidupan";
    // Mendeklarasikan field bertipe `string`: `BayarRisiko` menyimpan nilai bayar risiko dengan nilai awal nilai literal `”BayarRisiko”`.
    public const string BayarRisiko = "BayarRisiko";
    // Mendeklarasikan field bertipe `string`: `RiskEmergencyUsed` menyimpan nilai risiko emergency used dengan nilai awal nilai literal
    // `”GunakanOpsiDarurat”`.
    public const string RiskEmergencyUsed = "GunakanOpsiDarurat";
    // Mendeklarasikan field bertipe `string`: `DonationRankAwarded` menyimpan nilai donasi rank awarded dengan nilai awal nilai literal
    // `”PoinPeringkatDonasi”`.
    public const string DonationRankAwarded = "PoinPeringkatDonasi";
    // Mendeklarasikan field bertipe `string`: `DonationWinnersAnnounced` menyimpan nilai donasi winners announced dengan nilai awal nilai literal
    // `”UmumkanJuaraDonasi”`.
    public const string DonationWinnersAnnounced = "UmumkanJuaraDonasi";
    // Mendeklarasikan field bertipe `string`: `GoldPointsAwarded` menyimpan nilai emas poin awarded dengan nilai awal nilai literal `”PoinEmas”`.
    public const string GoldPointsAwarded = "PoinEmas";
    // Mendeklarasikan field bertipe `string`: `PensionRankAwarded` menyimpan nilai pension rank awarded dengan nilai awal nilai literal
    // `”PoinPeringkatPensiun”`.
    public const string PensionRankAwarded = "PoinPeringkatPensiun";
    // Mendeklarasikan field bertipe `string`: `SetupModalAwal` menyimpan nilai setup modal awal dengan nilai awal nilai literal `”SetupModalAwal”`.
    public const string SetupModalAwal = "SetupModalAwal";
    // Mendeklarasikan field bertipe `string`: `SetupBahanAwal` menyimpan nilai setup bahan awal dengan nilai awal nilai literal `”SetupBahanAwal”`.
    public const string SetupBahanAwal = "SetupBahanAwal";
    // Mendeklarasikan field bertipe `string`: `SetupEmasAwal` menyimpan nilai setup emas awal dengan nilai awal nilai literal `”SetupEmasAwal”`.
    public const string SetupEmasAwal = "SetupEmasAwal";
    // Mendeklarasikan field bertipe `string`: `SetupMisiAwal` menyimpan nilai setup misi awal dengan nilai awal nilai literal `”SetupMisiAwal”`.
    public const string SetupMisiAwal = "SetupMisiAwal";
    // Mendeklarasikan field bertipe `string`: `SetupPinjamanAwal` menyimpan nilai setup pinjaman awal dengan nilai awal nilai literal
    // `”SetupPinjamanAwal”`.
    public const string SetupPinjamanAwal = "SetupPinjamanAwal";
    // Mendeklarasikan field bertipe `string`: `SetupAsuransiAwal` menyimpan nilai setup asuransi awal dengan nilai awal nilai literal
    // `”SetupAsuransiAwal”`.
    public const string SetupAsuransiAwal = "SetupAsuransiAwal";
    // Mendeklarasikan field bertipe `string`: `TieBreakerAssigned` menyimpan nilai tie breaker assigned dengan nilai awal nilai literal
    // `”BagikanTieBreaker”`.
    public const string TieBreakerAssigned = "BagikanTieBreaker";
    // Mendeklarasikan field bertipe `string`: `CardDrawn` menyimpan nilai kartu drawn dengan nilai awal nilai literal `”AmbilKartuDariDeck”`.
    public const string CardDrawn = "AmbilKartuDariDeck";
    // Mendeklarasikan field bertipe `string`: `CardDiscarded` menyimpan nilai kartu discarded dengan nilai awal nilai literal `”KartuMasukDiscard”`.
    public const string CardDiscarded = "KartuMasukDiscard";
    // Mendeklarasikan field bertipe `string`: `MarketRefilled` menyimpan nilai pasar refilled dengan nilai awal nilai literal `”IsiUlangPasar”`.
    public const string MarketRefilled = "IsiUlangPasar";
    // Mendeklarasikan field bertipe `string`: `SessionStarted` menyimpan nilai sesi started dengan nilai awal nilai literal `”MulaiSesi”`.
    public const string SessionStarted = "MulaiSesi";
    // Mendeklarasikan field bertipe `string`: `SessionEnded` menyimpan nilai sesi ended dengan nilai awal nilai literal `”AkhiriSesi”`.
    public const string SessionEnded = "AkhiriSesi";
    // Mendeklarasikan field bertipe `string`: `AkhirGiliran` menyimpan nilai akhir giliran dengan nilai awal nilai literal `”AkhirGiliran”`.
    public const string AkhirGiliran = "AkhirGiliran";

    // Mendefinisikan metode `ResolveGameActionId` dengan hasil bertipe `string?`; operasi ini menangani resolve game aksi identitas. Masukan: Parameter
    // `actionType` bertipe `string?` membawa nilai aksi jenis; nilai null diizinkan ketika data opsional belum tersedia; Parameter `payload` bertipe
    // `JsonElement` membawa muatan detail event dalam format JSON.
    public static string? ResolveGameActionId(string? actionType, JsonElement payload)
    // Membuka scope metode ResolveGameActionId; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveGameActionId.
    {
        // Memeriksa memeriksa apakah `actionType` null, kosong, atau hanya berisi karakter spasi; blok if hanya dijalankan ketika kondisi ini bernilai
        // benar dalam ResolveGameActionId.
        if (string.IsNullOrWhiteSpace(actionType))
        // Membuka scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(actionType)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ResolveGameActionId.
        {
            // Mengembalikan null, yaitu penanda tidak ada nilai kepada pemanggil dalam ResolveGameActionId; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return null;
        // Menutup scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(actionType)`; bagian berikut berada di luar batas blok tersebut dalam
        // ResolveGameActionId.
        }

        // Menyiapkan variabel lokal `normalized` untuk nilai normalized dengan membersihkan karakter tepi pada `actionType` memakai tanpa argumen. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var normalized = actionType.Trim();
        // Mengembalikan memanggil `ResolveCanonicalGameAction` dengan `normalized` kepada pemanggil dalam ResolveGameActionId; eksekusi jalur ini selesai
        // setelah nilai hasil ditentukan.
        return ResolveCanonicalGameAction(normalized);
    // Menutup scope metode ResolveGameActionId; bagian berikut berada di luar batas blok tersebut dalam ResolveGameActionId.
    }

    // Mendefinisikan metode `Is` dengan hasil bertipe `bool`; operasi ini menangani berstatus. Masukan: Parameter `actionType` bertipe `string?`
    // membawa nilai aksi jenis; nilai null diizinkan ketika data opsional belum tersedia; Parameter `payload` bertipe `JsonElement` membawa muatan
    // detail event dalam format JSON; Parameter `canonicalActionId` bertipe `string` membawa nilai canonical aksi identitas.
    public static bool Is(string? actionType, JsonElement payload, string canonicalActionId)
    // Membuka scope metode Is; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Is.
    {
        // Mengembalikan membandingkan kesamaan `string` dengan `ResolveGameActionId(actionType, payload)`, `canonicalActionId`,
        // `StringComparison.OrdinalIgnoreCase`; aturan perbandingan mengikuti overload dan comparer yang diberikan kepada pemanggil dalam Is; eksekusi
        // jalur ini selesai setelah nilai hasil ditentukan.
        return string.Equals(
            // Meneruskan memanggil `ResolveGameActionId` dengan `actionType`, `payload` sebagai argumen ke `string.Equals`; Meneruskan `actionType` (nilai aksi
            // jenis) sebagai argumen ke `ResolveGameActionId`; Meneruskan `payload` (muatan detail event dalam format JSON) sebagai argumen ke
            // `ResolveGameActionId`.
            ResolveGameActionId(actionType, payload),
            // Meneruskan `canonicalActionId` (nilai canonical aksi identitas) sebagai argumen ke `string.Equals`.
            canonicalActionId,
            // Meneruskan `StringComparison.OrdinalIgnoreCase` (nilai ordinal ignore case) sebagai argumen ke `string.Equals`.
            StringComparison.OrdinalIgnoreCase);
    // Menutup scope metode Is; bagian berikut berada di luar batas blok tersebut dalam Is.
    }

    // Mendefinisikan metode `GetPlayerActionSlotPolicy` dengan hasil bertipe `PlayerActionSlotPolicy`; operasi ini menangani get pemain aksi slot
    // policy. Masukan: Parameter `actionType` bertipe `string?` membawa nilai aksi jenis; nilai null diizinkan ketika data opsional belum tersedia;
    // Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON.
    public static PlayerActionSlotPolicy GetPlayerActionSlotPolicy(string? actionType, JsonElement payload)
    // Membuka scope metode GetPlayerActionSlotPolicy; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam GetPlayerActionSlotPolicy.
    {
        // Menyiapkan variabel lokal `actionId` untuk kode aksi yang dipetakan terhadap katalog aturan dengan memanggil `ResolveGameActionId` dengan
        // `actionType`, `payload`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var actionId = ResolveGameActionId(actionType, payload);
        // Memeriksa hasil pencocokan `actionId` dengan pola `JumatBerkah or RisikoKehidupan or BayarRisiko or RiskEmergencyUsed or InvestasiEmas or
        // JualEmas or GoldSkipped`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam GetPlayerActionSlotPolicy.
        if (actionId is JumatBerkah or RisikoKehidupan or BayarRisiko or RiskEmergencyUsed or
            // Menggunakan `InvestasiEmas` (nilai investasi emas) sebagai bagian ekspresi yang sedang disusun dalam GetPlayerActionSlotPolicy.
            InvestasiEmas or JualEmas or GoldSkipped)
        // Membuka scope cabang if untuk kondisi `actionId is JumatBerkah or RisikoKehidupan or BayarRisiko or RiskEmergencyUsed or InvestasiEmas or
        // JualEmas or GoldSkipped`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam GetPlayerActionSlotPolicy.
        {
            // Mengembalikan `PlayerActionSlotPolicy.Free` (nilai free) kepada pemanggil dalam GetPlayerActionSlotPolicy; eksekusi jalur ini selesai setelah
            // nilai hasil ditentukan.
            return PlayerActionSlotPolicy.Free;
        // Menutup scope cabang if untuk kondisi `actionId is JumatBerkah or RisikoKehidupan or BayarRisiko or RiskEmergencyUsed or InvestasiEmas or
        // JualEmas or GoldSkipped`; bagian berikut berada di luar batas blok tersebut dalam GetPlayerActionSlotPolicy.
        }

        // Memeriksa hasil pencocokan `actionId` dengan pola `Asuransi or PinjamanSyariah`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // GetPlayerActionSlotPolicy.
        if (actionId is Asuransi or PinjamanSyariah)
        // Membuka scope cabang if untuk kondisi `actionId is Asuransi or PinjamanSyariah`; pernyataan/deklarasi berikut berada di dalam batas blok ini
        // dalam GetPlayerActionSlotPolicy.
        {
            // Mengembalikan hasil pemilihan bersyarat: ketika `HasRiskReference(payload)` benar gunakan `PlayerActionSlotPolicy.Free`, jika tidak gunakan
            // `PlayerActionSlotPolicy.Consumes` kepada pemanggil dalam GetPlayerActionSlotPolicy; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return HasRiskReference(payload)
                // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: PlayerActionSlotPolicy.Free dalam GetPlayerActionSlotPolicy.
                ? PlayerActionSlotPolicy.Free
                // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: PlayerActionSlotPolicy.Consumes; dalam GetPlayerActionSlotPolicy.
                : PlayerActionSlotPolicy.Consumes;
        // Menutup scope cabang if untuk kondisi `actionId is Asuransi or PinjamanSyariah`; bagian berikut berada di luar batas blok tersebut dalam
        // GetPlayerActionSlotPolicy.
        }

        // Mengembalikan hasil pemilihan bersyarat: ketika `actionId is BahanMasakan or IngredientDiscarded or JualMasakan or Kebutuhan or KerjaLepas or
        // Menabung or SavingDepositWithdrawn or TujuanFinansial or BayarPinjaman` benar gunakan `PlayerActionSlotPolicy.Consumes`, jika tidak gunakan
        // `PlayerActionSlotPolicy.Unspecified` kepada pemanggil dalam GetPlayerActionSlotPolicy; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return actionId is BahanMasakan or IngredientDiscarded or JualMasakan or
            // Menggunakan `Kebutuhan` (nilai kebutuhan) sebagai bagian ekspresi yang sedang disusun dalam GetPlayerActionSlotPolicy.
            Kebutuhan or KerjaLepas or Menabung or SavingDepositWithdrawn or TujuanFinansial or BayarPinjaman
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: PlayerActionSlotPolicy.Consumes dalam GetPlayerActionSlotPolicy.
            ? PlayerActionSlotPolicy.Consumes
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: PlayerActionSlotPolicy.Unspecified; dalam GetPlayerActionSlotPolicy.
            : PlayerActionSlotPolicy.Unspecified;
    // Menutup scope metode GetPlayerActionSlotPolicy; bagian berikut berada di luar batas blok tersebut dalam GetPlayerActionSlotPolicy.
    }

    // Mendefinisikan metode `RequiresSystemActor` dengan hasil bertipe `bool`; operasi ini menangani requires system actor. Masukan: Parameter
    // `actionType` bertipe `string?` membawa nilai aksi jenis; nilai null diizinkan ketika data opsional belum tersedia; Parameter `payload` bertipe
    // `JsonElement` membawa muatan detail event dalam format JSON.
    public static bool RequiresSystemActor(string? actionType, JsonElement payload)
    // Membuka scope metode RequiresSystemActor; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam RequiresSystemActor.
    {
        // Mengembalikan hasil pencocokan `ResolveGameActionId(actionType, payload)` dengan pola `TransactionRecorded or SundayRest or GoldPriceOpened or
        // DonationRankAwarded or DonationWinnersAnnounced or GoldPointsAwarded or PensionRankAwarded or SetupModalAwal or SetupBa...` kepada pemanggil
        // dalam RequiresSystemActor; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return ResolveGameActionId(actionType, payload) is TransactionRecorded or SundayRest or GoldPriceOpened or
            // Menggunakan `DonationRankAwarded` (nilai donasi rank awarded) sebagai bagian ekspresi yang sedang disusun dalam RequiresSystemActor.
            DonationRankAwarded or DonationWinnersAnnounced or GoldPointsAwarded or PensionRankAwarded or
            // Menggunakan `SetupModalAwal` (nilai setup modal awal) sebagai bagian ekspresi yang sedang disusun dalam RequiresSystemActor.
            SetupModalAwal or SetupBahanAwal or SetupEmasAwal or SetupMisiAwal or SetupPinjamanAwal or
            // Menggunakan `SetupAsuransiAwal` (nilai setup asuransi awal) sebagai bagian ekspresi yang sedang disusun dalam RequiresSystemActor.
            SetupAsuransiAwal or TieBreakerAssigned or
            // Menggunakan `SessionStarted` (nilai sesi started) sebagai bagian ekspresi yang sedang disusun dalam RequiresSystemActor.
            SessionStarted or SessionEnded or AkhirGiliran;
    // Menutup scope metode RequiresSystemActor; bagian berikut berada di luar batas blok tersebut dalam RequiresSystemActor.
    }

    // Mendefinisikan metode `HasRiskReference` dengan hasil bertipe `bool`; operasi ini menangani memiliki risiko reference. Masukan: Parameter
    // `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON.
    private static bool HasRiskReference(JsonElement payload)
    // Membuka scope metode HasRiskReference; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam HasRiskReference.
    {
        // Memeriksa perbandingan ketidaksamaan antara `payload.ValueKind` dan `JsonValueKind.Object`; blok if hanya dijalankan ketika kondisi ini bernilai
        // benar dalam HasRiskReference.
        if (payload.ValueKind != JsonValueKind.Object)
        // Membuka scope cabang if untuk kondisi `payload.ValueKind != JsonValueKind.Object`; pernyataan/deklarasi berikut berada di dalam batas blok ini
        // dalam HasRiskReference.
        {
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam HasRiskReference; eksekusi jalur ini selesai setelah nilai
            // hasil ditentukan.
            return false;
        // Menutup scope cabang if untuk kondisi `payload.ValueKind != JsonValueKind.Object`; bagian berikut berada di luar batas blok tersebut dalam
        // HasRiskReference.
        }

        // Mengembalikan gabungan syarat AND: kedua kondisi wajib benar antara `payload.TryGetProperty(”risk_event_id”, out var id) && id.ValueKind ==
        // JsonValueKind.String` dan `Guid.TryParse(id.GetString(), out _)`; sisi kanan diperiksa hanya jika sisi kiri benar kepada pemanggil dalam
        // HasRiskReference; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return payload.TryGetProperty("risk_event_id", out var id) &&
               // Melanjutkan ekspresi dengan perbandingan kesamaan antara `id.ValueKind` dan `JsonValueKind.String` dalam HasRiskReference.
               id.ValueKind == JsonValueKind.String &&
               // Melanjutkan pengolahan dengan mencoba mengonversi `id.GetString()`, `_` melalui `Guid.TryParse`; keberhasilan dilaporkan sebagai boolean dan
               // hasil ditempatkan pada argumen out dalam HasRiskReference.
               Guid.TryParse(id.GetString(), out _);
    // Menutup scope metode HasRiskReference; bagian berikut berada di luar batas blok tersebut dalam HasRiskReference.
    }

    // Mendefinisikan metode `ResolveCanonicalGameAction` dengan hasil bertipe `string?`; operasi ini menangani resolve canonical game aksi. Masukan:
    // Parameter `actionType` bertipe `string` membawa nilai aksi jenis.
    private static string? ResolveCanonicalGameAction(string actionType)
    // Membuka scope metode ResolveCanonicalGameAction; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveCanonicalGameAction.
    {
        // Mengembalikan hasil pemetaan `actionType.ToLowerInvariant()` melalui cabang pola switch yang cocok kepada pemanggil dalam
        // ResolveCanonicalGameAction; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return actionType.ToLowerInvariant() switch
        // Membuka scope pemetaan switch atas `actionType.ToLowerInvariant()`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ResolveCanonicalGameAction.
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
        // Menutup scope pemetaan switch atas `actionType.ToLowerInvariant()`; bagian berikut berada di luar batas blok tersebut dalam
        // ResolveCanonicalGameAction.
        };
    // Menutup scope metode ResolveCanonicalGameAction; bagian berikut berada di luar batas blok tersebut dalam ResolveCanonicalGameAction.
    }
// Menutup scope tipe GameActionCatalog; bagian berikut berada di luar batas blok tersebut.
}
