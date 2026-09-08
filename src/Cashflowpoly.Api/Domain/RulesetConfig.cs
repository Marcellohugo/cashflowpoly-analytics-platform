// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui RulesetConfig.
// Mengimpor namespace `System.Text.Json` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.Json;
// Mengimpor namespace `Cashflowpoly.Api.Contracts` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Contracts;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Domain;

/// <summary>
/// Model domain konfigurasi ruleset berisi parameter permainan: mode, aksi harian pemain, kas awal, constraint, dan scoring.
/// </summary>
// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `RulesetConfig`; sealed mencegah tipe ini diturunkan lagi.
public sealed record RulesetConfig(
    // Parameter `Mode` bertipe `string` membawa mode permainan yang menentukan kelompok aturan yang digunakan.
    string Mode,
    // Parameter `ActionsPerTurn` bertipe `int` membawa nilai aksi per giliran.
    int ActionsPerTurn,
    // Parameter `StartingCash` bertipe `int` membawa nilai starting uang tunai.
    int StartingCash,
    // Parameter `PlayerOrdering` bertipe `PlayerOrdering` membawa nilai pemain ordering.
    PlayerOrdering PlayerOrdering,
    // Parameter `CashMin` bertipe `int` membawa nilai uang tunai minimum.
    int CashMin,
    // Parameter `MaxIngredientTotal` bertipe `int` membawa nilai maksimum bahan total.
    int MaxIngredientTotal,
    // Parameter `MaxSameIngredient` bertipe `int` membawa nilai maksimum same bahan.
    int MaxSameIngredient,
    // Parameter `PrimaryNeedMaxPerDay` bertipe `int?` membawa nilai primary kebutuhan maksimum per hari; nilai null diizinkan ketika data opsional
    // belum tersedia.
    int? PrimaryNeedMaxPerDay,
    // Parameter `RequirePrimaryBeforeOthers` bertipe `bool` membawa nilai require primary before others.
    bool RequirePrimaryBeforeOthers,
    // Parameter `FridayEnabled` bertipe `bool` membawa nilai friday enabled.
    bool FridayEnabled,
    // Parameter `SaturdayEnabled` bertipe `bool` membawa nilai saturday enabled.
    bool SaturdayEnabled,
    // Parameter `SundayEnabled` bertipe `bool` membawa nilai sunday enabled.
    bool SundayEnabled,
    // Parameter `DonationMin` bertipe `int` membawa nilai donasi minimum.
    int DonationMin,
    // Parameter `DonationMax` bertipe `int` membawa nilai donasi maksimum.
    int DonationMax,
    // Parameter `GoldAllowBuy` bertipe `bool` membawa nilai emas allow buy.
    bool GoldAllowBuy,
    // Parameter `GoldAllowSell` bertipe `bool` membawa nilai emas allow sell.
    bool GoldAllowSell,
    // Parameter `LoanEnabled` bertipe `bool` membawa nilai pinjaman enabled.
    bool LoanEnabled,
    // Parameter `InsuranceEnabled` bertipe `bool` membawa nilai asuransi enabled.
    bool InsuranceEnabled,
    // Parameter `SavingGoalEnabled` bertipe `bool` membawa nilai tabungan target enabled.
    bool SavingGoalEnabled,
    // Parameter `FreelanceIncome` bertipe `int` membawa nilai freelance pemasukan.
    int FreelanceIncome,
    // Parameter `Scoring` bertipe `RulesetScoringConfig?` membawa nilai scoring; nilai null diizinkan ketika data opsional belum tersedia.
    RulesetScoringConfig? Scoring)
// Membuka scope tipe RulesetConfig; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan properti `FinishDay` bertipe `int` untuk nilai finish hari; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek; nilai awalnya nilai literal `25`.
    public int FinishDay { get; init; } = 25;

    // Mendefinisikan properti `NeedSetBonuses` bertipe `IReadOnlyList<RulesetNeedSetBonusDto>` untuk nilai kebutuhan set bonuses; get menyediakan
    // pembacaan nilai, init membatasi pengisian saat inisialisasi objek; nilai awalnya koleksi kosong dengan tipe mengikuti konteks tujuan.
    public IReadOnlyList<RulesetNeedSetBonusDto> NeedSetBonuses { get; init; } = [];

    // Mendefinisikan properti `GoldPrices` bertipe `IReadOnlyList<RulesetGoldPriceDto>` untuk nilai emas prices; get menyediakan pembacaan nilai, init
    // membatasi pengisian saat inisialisasi objek; nilai awalnya koleksi kosong dengan tipe mengikuti konteks tujuan.
    public IReadOnlyList<RulesetGoldPriceDto> GoldPrices { get; init; } = [];

    // Mendefinisikan properti `ShariaLoans` bertipe `IReadOnlyList<RulesetShariaLoanDto>` untuk nilai sharia pinjaman; get menyediakan pembacaan nilai,
    // init membatasi pengisian saat inisialisasi objek; nilai awalnya koleksi kosong dengan tipe mengikuti konteks tujuan.
    public IReadOnlyList<RulesetShariaLoanDto> ShariaLoans { get; init; } = [];

    // Mendefinisikan properti `InsuranceProducts` bertipe `IReadOnlyList<RulesetInsuranceProductDto>` untuk nilai asuransi products; get menyediakan
    // pembacaan nilai, init membatasi pengisian saat inisialisasi objek; nilai awalnya koleksi kosong dengan tipe mengikuti konteks tujuan.
    public IReadOnlyList<RulesetInsuranceProductDto> InsuranceProducts { get; init; } = [];

    // Mendefinisikan properti `LifeRisks` bertipe `IReadOnlyList<RulesetLifeRiskDto>` untuk nilai life risks; get menyediakan pembacaan nilai, init
    // membatasi pengisian saat inisialisasi objek; nilai awalnya koleksi kosong dengan tipe mengikuti konteks tujuan.
    public IReadOnlyList<RulesetLifeRiskDto> LifeRisks { get; init; } = [];

    // Mendefinisikan properti `Orders` bertipe `IReadOnlyList<RulesetOrderDto>` untuk nilai pesanan; get menyediakan pembacaan nilai, init membatasi
    // pengisian saat inisialisasi objek; nilai awalnya koleksi kosong dengan tipe mengikuti konteks tujuan.
    public IReadOnlyList<RulesetOrderDto> Orders { get; init; } = [];

    // Mendefinisikan properti `Ingredients` bertipe `IReadOnlyList<RulesetIngredientDto>` untuk nilai bahan; get menyediakan pembacaan nilai, init
    // membatasi pengisian saat inisialisasi objek; nilai awalnya koleksi kosong dengan tipe mengikuti konteks tujuan.
    public IReadOnlyList<RulesetIngredientDto> Ingredients { get; init; } = [];

    // Mendefinisikan properti `FinancialGoals` bertipe `IReadOnlyList<RulesetFinancialGoalDto>` untuk nilai keuangan target; get menyediakan pembacaan
    // nilai, init membatasi pengisian saat inisialisasi objek; nilai awalnya koleksi kosong dengan tipe mengikuti konteks tujuan.
    public IReadOnlyList<RulesetFinancialGoalDto> FinancialGoals { get; init; } = [];

    // Mendefinisikan properti `Needs` bertipe `IReadOnlyList<RulesetNeedDto>` untuk nilai kebutuhan; get menyediakan pembacaan nilai, init membatasi
    // pengisian saat inisialisasi objek; nilai awalnya koleksi kosong dengan tipe mengikuti konteks tujuan.
    public IReadOnlyList<RulesetNeedDto> Needs { get; init; } = [];

    // Mendefinisikan properti `Actions` bertipe `IReadOnlyList<RulesetActionDto>` untuk nilai aksi; get menyediakan pembacaan nilai, init membatasi
    // pengisian saat inisialisasi objek; nilai awalnya koleksi kosong dengan tipe mengikuti konteks tujuan.
    public IReadOnlyList<RulesetActionDto> Actions { get; init; } = [];
// Menutup scope tipe RulesetConfig; bagian berikut berada di luar batas blok tersebut.
}

/// <summary>
/// Konfigurasi skoring ruleset berisi pemetaan poin berdasarkan peringkat donasi, jumlah emas, dan peringkat pensiun.
/// </summary>
// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `RulesetScoringConfig`; sealed mencegah tipe ini diturunkan lagi.
public sealed record RulesetScoringConfig(
    // Parameter `DonationRankPoints` bertipe `IReadOnlyList<RankPoint>` membawa nilai donasi rank poin.
    IReadOnlyList<RankPoint> DonationRankPoints,
    // Parameter `GoldPointsByQty` bertipe `IReadOnlyList<QtyPoint>` membawa nilai emas poin berdasarkan qty.
    IReadOnlyList<QtyPoint> GoldPointsByQty,
    // Parameter `PensionRankPoints` bertipe `IReadOnlyList<RankPoint>` membawa nilai pension rank poin.
    IReadOnlyList<RankPoint> PensionRankPoints);

/// <summary>
/// Pemetaan peringkat ke poin (untuk donasi dan pensiun).
/// </summary>
// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `RankPoint`; sealed mencegah tipe ini diturunkan lagi.
public sealed record RankPoint(int Rank, int Points);

/// <summary>
/// Pemetaan jumlah (quantity) ke poin (untuk emas).
/// </summary>
// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `QtyPoint`; sealed mencegah tipe ini diturunkan lagi.
public sealed record QtyPoint(int Qty, int Points);

/// <summary>
/// Enum urutan pemain dalam sesi: berdasarkan join order, instruktur, urutan event, player ID, atau username.
/// </summary>
// Mendefinisikan enum untuk membatasi pilihan nilai bernama `PlayerOrdering`.
public enum PlayerOrdering
// Membuka scope tipe PlayerOrdering; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan pilihan enum `PlayerOrder`; nilai bilangan mengikuti urutan deklarasi enum.
    PlayerOrder,
    // Mendefinisikan pilihan enum `EventSequence`; nilai bilangan mengikuti urutan deklarasi enum.
    EventSequence,
    // Mendefinisikan pilihan enum `PlayerId`; nilai bilangan mengikuti urutan deklarasi enum.
    PlayerId,
    // Mendefinisikan pilihan enum `Username`; nilai bilangan mengikuti urutan deklarasi enum.
    Username
// Menutup scope tipe PlayerOrdering; bagian berikut berada di luar batas blok tersebut.
}

// Mendefinisikan tipe class `RulesetRuntimeMapper`.
internal static class RulesetRuntimeMapper
// Membuka scope tipe RulesetRuntimeMapper; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan metode `TryBuildConfig` dengan hasil bertipe `bool`; operasi ini menangani try build konfigurasi. Masukan: Parameter `definition`
    // bertipe `RulesetDefinitionDto` membawa definisi terstruktur komponen serta parameter aturan permainan; Parameter `config` bertipe
    // `RulesetConfig?` membawa konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan; nilai null diizinkan ketika data opsional
    // belum tersedia; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `errors` bertipe `List<ErrorDetail>` membawa
    // nilai kesalahan; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    internal static bool TryBuildConfig(
        // Parameter `definition` bertipe `RulesetDefinitionDto` membawa definisi terstruktur komponen serta parameter aturan permainan.
        RulesetDefinitionDto definition,
        // Parameter `config` bertipe `RulesetConfig?` membawa konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan; nilai null
        // diizinkan ketika data opsional belum tersedia; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
        out RulesetConfig? config,
        // Parameter `errors` bertipe `List<ErrorDetail>` membawa nilai kesalahan; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
        out List<ErrorDetail> errors)
    // Membuka scope metode TryBuildConfig; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryBuildConfig.
    {
        // Memperbarui `config` menggunakan null, yaitu penanda tidak ada nilai dalam TryBuildConfig.
        config = null;
        // Memperbarui `errors` menggunakan objek baru bertipe `List<ErrorDetail>` dengan nilai awal sesuai konstruktornya dalam TryBuildConfig.
        errors = new List<ErrorDetail>();

        // Menyiapkan variabel lokal `mode` untuk mode permainan yang menentukan kelompok aturan yang digunakan dengan hasil pemilihan bersyarat: ketika
        // `string.IsNullOrWhiteSpace(definition.Mode)` benar gunakan `”MAHIR”`, jika tidak gunakan `definition.Mode.Trim().ToUpperInvariant()`. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var mode = string.IsNullOrWhiteSpace(definition.Mode)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: ”MAHIR” dalam TryBuildConfig.
            ? "MAHIR"
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: definition.Mode.Trim().ToUpperInvariant(); dalam TryBuildConfig.
            : definition.Mode.Trim().ToUpperInvariant();
        // Memeriksa hasil pencocokan `mode` dengan pola `not (”PEMULA” or ”MAHIR”)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // TryBuildConfig.
        if (mode is not ("PEMULA" or "MAHIR"))
        // Membuka scope cabang if untuk kondisi `mode is not (”PEMULA” or ”MAHIR”)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // TryBuildConfig.
        {
            // Menjalankan menambahkan `new ErrorDetail(”definition.mode”, ”INVALID_ENUM”)` ke `errors` dalam TryBuildConfig.
            errors.Add(new ErrorDetail("definition.mode", "INVALID_ENUM"));
        // Menutup scope cabang if untuk kondisi `mode is not (”PEMULA” or ”MAHIR”)`; bagian berikut berada di luar batas blok tersebut dalam
        // TryBuildConfig.
        }

        // Menyiapkan variabel lokal `settings` untuk nilai settings dengan `definition.Settings` bila tidak null; jika null gunakan `new
        // RulesetSettingsDto()` sebagai nilai pengganti. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var settings = definition.Settings ?? new RulesetSettingsDto();
        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `settings.ActionsPerTurn < 1` dan `settings.ActionsPerTurn > 10`; sisi
        // kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam TryBuildConfig.
        if (settings.ActionsPerTurn < 1 || settings.ActionsPerTurn > 10)
        // Membuka scope cabang if untuk kondisi `settings.ActionsPerTurn < 1 || settings.ActionsPerTurn > 10`; pernyataan/deklarasi berikut berada di dalam
        // batas blok ini dalam TryBuildConfig.
        {
            // Menjalankan menambahkan `new ErrorDetail(”definition.settings.actions_per_turn”, ”OUT_OF_RANGE”)` ke `errors` dalam TryBuildConfig.
            errors.Add(new ErrorDetail("definition.settings.actions_per_turn", "OUT_OF_RANGE"));
        // Menutup scope cabang if untuk kondisi `settings.ActionsPerTurn < 1 || settings.ActionsPerTurn > 10`; bagian berikut berada di luar batas blok
        // tersebut dalam TryBuildConfig.
        }

        // Memeriksa pemeriksaan lebih kecil antara `settings.StartingCash` dan `0`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // TryBuildConfig.
        if (settings.StartingCash < 0)
        // Membuka scope cabang if untuk kondisi `settings.StartingCash < 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // TryBuildConfig.
        {
            // Menjalankan menambahkan `new ErrorDetail(”definition.settings.starting_cash”, ”OUT_OF_RANGE”)` ke `errors` dalam TryBuildConfig.
            errors.Add(new ErrorDetail("definition.settings.starting_cash", "OUT_OF_RANGE"));
        // Menutup scope cabang if untuk kondisi `settings.StartingCash < 0`; bagian berikut berada di luar batas blok tersebut dalam TryBuildConfig.
        }

        // Memeriksa pemeriksaan lebih kecil antara `settings.CashMin` dan `0`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // TryBuildConfig.
        if (settings.CashMin < 0)
        // Membuka scope cabang if untuk kondisi `settings.CashMin < 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryBuildConfig.
        {
            // Menjalankan menambahkan `new ErrorDetail(”definition.settings.cash_min”, ”OUT_OF_RANGE”)` ke `errors` dalam TryBuildConfig.
            errors.Add(new ErrorDetail("definition.settings.cash_min", "OUT_OF_RANGE"));
        // Menutup scope cabang if untuk kondisi `settings.CashMin < 0`; bagian berikut berada di luar batas blok tersebut dalam TryBuildConfig.
        }

        // Memeriksa pemeriksaan lebih kecil antara `settings.MaxIngredientTotal` dan `0`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // TryBuildConfig.
        if (settings.MaxIngredientTotal < 0)
        // Membuka scope cabang if untuk kondisi `settings.MaxIngredientTotal < 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // TryBuildConfig.
        {
            // Menjalankan menambahkan `new ErrorDetail(”definition.settings.max_ingredient_total”, ”OUT_OF_RANGE”)` ke `errors` dalam TryBuildConfig.
            errors.Add(new ErrorDetail("definition.settings.max_ingredient_total", "OUT_OF_RANGE"));
        // Menutup scope cabang if untuk kondisi `settings.MaxIngredientTotal < 0`; bagian berikut berada di luar batas blok tersebut dalam TryBuildConfig.
        }

        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `settings.MaxSameIngredient < 0` dan `settings.MaxSameIngredient >
        // settings.MaxIngredientTotal`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // TryBuildConfig.
        if (settings.MaxSameIngredient < 0 || settings.MaxSameIngredient > settings.MaxIngredientTotal)
        // Membuka scope cabang if untuk kondisi `settings.MaxSameIngredient < 0 || settings.MaxSameIngredient > settings.MaxIngredientTotal`;
        // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryBuildConfig.
        {
            // Menjalankan menambahkan `new ErrorDetail(”definition.settings.max_same_ingredient”, ”INVALID_RELATION”)` ke `errors` dalam TryBuildConfig.
            errors.Add(new ErrorDetail("definition.settings.max_same_ingredient", "INVALID_RELATION"));
        // Menutup scope cabang if untuk kondisi `settings.MaxSameIngredient < 0 || settings.MaxSameIngredient > settings.MaxIngredientTotal`; bagian
        // berikut berada di luar batas blok tersebut dalam TryBuildConfig.
        }

        // Memeriksa pemeriksaan lebih kecil antara `settings.PrimaryNeedMaxPerDay` dan `0`; blok if hanya dijalankan ketika kondisi ini bernilai benar
        // dalam TryBuildConfig.
        if (settings.PrimaryNeedMaxPerDay < 0)
        // Membuka scope cabang if untuk kondisi `settings.PrimaryNeedMaxPerDay < 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // TryBuildConfig.
        {
            // Menjalankan menambahkan `new ErrorDetail(”definition.settings.primary_need_max_per_day”, ”OUT_OF_RANGE”)` ke `errors` dalam TryBuildConfig.
            errors.Add(new ErrorDetail("definition.settings.primary_need_max_per_day", "OUT_OF_RANGE"));
        // Menutup scope cabang if untuk kondisi `settings.PrimaryNeedMaxPerDay < 0`; bagian berikut berada di luar batas blok tersebut dalam
        // TryBuildConfig.
        }

        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `settings.MinPlayers < 2 || settings.MaxPlayers > 4` dan
        // `settings.MinPlayers > settings.MaxPlayers`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini
        // bernilai benar dalam TryBuildConfig.
        if (settings.MinPlayers < 2 || settings.MaxPlayers > 4 || settings.MinPlayers > settings.MaxPlayers)
        // Membuka scope cabang if untuk kondisi `settings.MinPlayers < 2 || settings.MaxPlayers > 4 || settings.MinPlayers > settings.MaxPlayers`;
        // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryBuildConfig.
        {
            // Menjalankan menambahkan `new ErrorDetail(”definition.settings.min_players”, ”INVALID_RANGE”)` ke `errors` dalam TryBuildConfig.
            errors.Add(new ErrorDetail("definition.settings.min_players", "INVALID_RANGE"));
        // Menutup scope cabang if untuk kondisi `settings.MinPlayers < 2 || settings.MaxPlayers > 4 || settings.MinPlayers > settings.MaxPlayers`; bagian
        // berikut berada di luar batas blok tersebut dalam TryBuildConfig.
        }

        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `settings.DonationMinAmount < 1` dan `settings.DonationMaxAmount <
        // settings.DonationMinAmount`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // TryBuildConfig.
        if (settings.DonationMinAmount < 1 || settings.DonationMaxAmount < settings.DonationMinAmount)
        // Membuka scope cabang if untuk kondisi `settings.DonationMinAmount < 1 || settings.DonationMaxAmount < settings.DonationMinAmount`;
        // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryBuildConfig.
        {
            // Menjalankan menambahkan `new ErrorDetail(”definition.settings.donation_min_amount”, ”INVALID_RANGE”)` ke `errors` dalam TryBuildConfig.
            errors.Add(new ErrorDetail("definition.settings.donation_min_amount", "INVALID_RANGE"));
        // Menutup scope cabang if untuk kondisi `settings.DonationMinAmount < 1 || settings.DonationMaxAmount < settings.DonationMinAmount`; bagian berikut
        // berada di luar batas blok tersebut dalam TryBuildConfig.
        }

        // Memeriksa pemeriksaan lebih kecil antara `settings.FreelanceIncome` dan `1`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // TryBuildConfig.
        if (settings.FreelanceIncome < 1)
        // Membuka scope cabang if untuk kondisi `settings.FreelanceIncome < 1`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // TryBuildConfig.
        {
            // Menjalankan menambahkan `new ErrorDetail(”definition.settings.freelance_income”, ”OUT_OF_RANGE”)` ke `errors` dalam TryBuildConfig.
            errors.Add(new ErrorDetail("definition.settings.freelance_income", "OUT_OF_RANGE"));
        // Menutup scope cabang if untuk kondisi `settings.FreelanceIncome < 1`; bagian berikut berada di luar batas blok tersebut dalam TryBuildConfig.
        }

        // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `mode == ”PEMULA”` dan `(settings.LoanEnabled || settings.InsuranceEnabled ||
        // settings.SavingGoalEnabled)`; sisi kanan diperiksa hanya jika sisi kiri benar; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // TryBuildConfig.
        if (mode == "PEMULA" && (settings.LoanEnabled || settings.InsuranceEnabled || settings.SavingGoalEnabled))
        // Membuka scope cabang if untuk kondisi `mode == ”PEMULA” && (settings.LoanEnabled || settings.InsuranceEnabled || settings.SavingGoalEnabled)`;
        // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryBuildConfig.
        {
            // Menjalankan menambahkan `new ErrorDetail(”definition.settings”, ”DISALLOWED_FOR_MODE”)` ke `errors` dalam TryBuildConfig.
            errors.Add(new ErrorDetail("definition.settings", "DISALLOWED_FOR_MODE"));
        // Menutup scope cabang if untuk kondisi `mode == ”PEMULA” && (settings.LoanEnabled || settings.InsuranceEnabled || settings.SavingGoalEnabled)`;
        // bagian berikut berada di luar batas blok tersebut dalam TryBuildConfig.
        }

        // Memeriksa kebalikan kondisi `RulesetConfigParser.TryParsePlayerOrdering(definition.PlayerOrdering?.OrderingCode ?? ”PLAYER_ORDER”, out var
        // playerOrdering)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam TryBuildConfig.
        if (!RulesetConfigParser.TryParsePlayerOrdering(definition.PlayerOrdering?.OrderingCode ?? "PLAYER_ORDER", out var playerOrdering))
        // Membuka scope cabang if untuk kondisi `!RulesetConfigParser.TryParsePlayerOrdering(definition.PlayerOrdering?.OrderingCode ?? ”PLAYER_ORDER”, out
        // var playerOrdering)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryBuildConfig.
        {
            // Menjalankan menambahkan `new ErrorDetail(”definition.player_ordering.ordering_code”, ”INVALID_ENUM”)` ke `errors` dalam TryBuildConfig.
            errors.Add(new ErrorDetail("definition.player_ordering.ordering_code", "INVALID_ENUM"));
        // Menutup scope cabang if untuk kondisi `!RulesetConfigParser.TryParsePlayerOrdering(definition.PlayerOrdering?.OrderingCode ?? ”PLAYER_ORDER”, out
        // var playerOrdering)`; bagian berikut berada di luar batas blok tersebut dalam TryBuildConfig.
        }

        // Menjalankan memanggil `ValidateRankPoints` dengan `definition.DonationRankPoints`, `”definition.donation_rank_points”`, `errors` dalam
        // TryBuildConfig.
        ValidateRankPoints(definition.DonationRankPoints, "definition.donation_rank_points", errors);
        // Menjalankan memanggil `ValidateRankPoints` dengan `definition.PensionRankPoints`, `”definition.pension_rank_points”`, `errors` dalam
        // TryBuildConfig.
        ValidateRankPoints(definition.PensionRankPoints, "definition.pension_rank_points", errors);
        // Menjalankan memanggil `ValidateQtyPoints` dengan `definition.GoldPointsByQty`, `”definition.gold_points_by_qty”`, `errors` dalam TryBuildConfig.
        ValidateQtyPoints(definition.GoldPointsByQty, "definition.gold_points_by_qty", errors);
        // Menjalankan memanggil `ValidateNeedSetBonuses` dengan `definition.NeedSetBonuses`, `errors` dalam TryBuildConfig.
        ValidateNeedSetBonuses(definition.NeedSetBonuses, errors);
        // Menjalankan memanggil `ValidateGoldPrices` dengan `definition.GoldPrices`, `errors` dalam TryBuildConfig.
        ValidateGoldPrices(definition.GoldPrices, errors);
        // Menjalankan memanggil `ValidateShariaLoans` dengan `definition.ShariaLoans`, `settings.LoanEnabled`, `errors` dalam TryBuildConfig.
        ValidateShariaLoans(definition.ShariaLoans, settings.LoanEnabled, errors);
        // Menjalankan memanggil `ValidateInsuranceProducts` dengan `definition.InsuranceProducts`, `settings.InsuranceEnabled`, `errors` dalam
        // TryBuildConfig.
        ValidateInsuranceProducts(definition.InsuranceProducts, settings.InsuranceEnabled, errors);
        // Menjalankan memanggil `ValidateLifeRisks` dengan `definition.LifeRisks`, `errors` dalam TryBuildConfig.
        ValidateLifeRisks(definition.LifeRisks, errors);

        // Memeriksa pemeriksaan lebih besar antara `errors.Count` dan `0`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam TryBuildConfig.
        if (errors.Count > 0)
        // Membuka scope cabang if untuk kondisi `errors.Count > 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryBuildConfig.
        {
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryBuildConfig; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return false;
        // Menutup scope cabang if untuk kondisi `errors.Count > 0`; bagian berikut berada di luar batas blok tersebut dalam TryBuildConfig.
        }

        // Memperbarui `config` menggunakan objek baru bertipe `RulesetConfig` dengan argumen ( mode, settings.ActionsPerTurn, settings.StartingCash,
        // playerOrdering, settings.CashMin, settings.MaxIngredientTotal, settings.MaxSameIngredient, settings.Prim... dalam TryBuildConfig.
        config = new RulesetConfig(
            // Meneruskan `mode` (mode permainan yang menentukan kelompok aturan yang digunakan) sebagai argumen ke konstruktor `RulesetConfig`.
            mode,
            // Meneruskan `settings.ActionsPerTurn` (nilai aksi per giliran) sebagai argumen ke konstruktor `RulesetConfig`.
            settings.ActionsPerTurn,
            // Meneruskan `settings.StartingCash` (nilai starting uang tunai) sebagai argumen ke konstruktor `RulesetConfig`.
            settings.StartingCash,
            // Meneruskan `playerOrdering` (nilai pemain ordering) sebagai argumen ke konstruktor `RulesetConfig`.
            playerOrdering,
            // Meneruskan `settings.CashMin` (nilai uang tunai minimum) sebagai argumen ke konstruktor `RulesetConfig`.
            settings.CashMin,
            // Meneruskan `settings.MaxIngredientTotal` (nilai maksimum bahan total) sebagai argumen ke konstruktor `RulesetConfig`.
            settings.MaxIngredientTotal,
            // Meneruskan `settings.MaxSameIngredient` (nilai maksimum same bahan) sebagai argumen ke konstruktor `RulesetConfig`.
            settings.MaxSameIngredient,
            // Meneruskan `settings.PrimaryNeedMaxPerDay` (nilai primary kebutuhan maksimum per hari) sebagai argumen ke konstruktor `RulesetConfig`.
            settings.PrimaryNeedMaxPerDay,
            // Meneruskan `settings.RequirePrimaryBeforeOthers` (nilai require primary before others) sebagai argumen ke konstruktor `RulesetConfig`.
            settings.RequirePrimaryBeforeOthers,
            // Meneruskan `definition.PlayerOrdering?.FridayEnabled` bila tidak null; jika null gunakan `true` sebagai nilai pengganti sebagai argumen ke
            // konstruktor `RulesetConfig`.
            definition.PlayerOrdering?.FridayEnabled ?? true,
            // Meneruskan `definition.PlayerOrdering?.SaturdayEnabled` bila tidak null; jika null gunakan `true` sebagai nilai pengganti sebagai argumen ke
            // konstruktor `RulesetConfig`.
            definition.PlayerOrdering?.SaturdayEnabled ?? true,
            // Meneruskan `definition.PlayerOrdering?.SundayEnabled` bila tidak null; jika null gunakan `true` sebagai nilai pengganti sebagai argumen ke
            // konstruktor `RulesetConfig`.
            definition.PlayerOrdering?.SundayEnabled ?? true,
            // Meneruskan `settings.DonationMinAmount` (nilai donasi minimum nominal) sebagai argumen ke konstruktor `RulesetConfig`.
            settings.DonationMinAmount,
            // Meneruskan `settings.DonationMaxAmount` (nilai donasi maksimum nominal) sebagai argumen ke konstruktor `RulesetConfig`.
            settings.DonationMaxAmount,
            // Meneruskan `settings.GoldTradeAllowBuy` (nilai emas trade allow buy) sebagai argumen ke konstruktor `RulesetConfig`.
            settings.GoldTradeAllowBuy,
            // Meneruskan `settings.GoldTradeAllowSell` (nilai emas trade allow sell) sebagai argumen ke konstruktor `RulesetConfig`.
            settings.GoldTradeAllowSell,
            // Meneruskan `settings.LoanEnabled` (nilai pinjaman enabled) sebagai argumen ke konstruktor `RulesetConfig`.
            settings.LoanEnabled,
            // Meneruskan `settings.InsuranceEnabled` (nilai asuransi enabled) sebagai argumen ke konstruktor `RulesetConfig`.
            settings.InsuranceEnabled,
            // Meneruskan `settings.SavingGoalEnabled` (nilai tabungan target enabled) sebagai argumen ke konstruktor `RulesetConfig`.
            settings.SavingGoalEnabled,
            // Meneruskan `settings.FreelanceIncome` (nilai freelance pemasukan) sebagai argumen ke konstruktor `RulesetConfig`.
            settings.FreelanceIncome,
            // Meneruskan objek baru bertipe `RulesetScoringConfig` dengan argumen ( definition.DonationRankPoints.Select(item => new RankPoint(item.Rank,
            // item.Points)).ToList(), definition.GoldPointsByQty.Select(item => new QtyPoint(item.Qty,... sebagai argumen ke konstruktor `RulesetConfig`.
            new RulesetScoringConfig(
                // Meneruskan mematerialisasi urutan `definition.DonationRankPoints.Select(item => new RankPoint(item.Rank, item.Points))` menjadi List; enumerasi
                // dijalankan dan hasilnya disimpan dalam memori sebagai argumen ke konstruktor `RulesetScoringConfig`; Meneruskan fungsi lambda `item => new
                // RankPoint(item.Rank, item.Points)` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen ke
                // `definition.DonationRankPoints.Select`; Meneruskan `item.Rank` (nilai rank) sebagai argumen ke konstruktor `RankPoint`; Meneruskan `item.Points`
                // (nilai poin) sebagai argumen ke konstruktor `RankPoint`.
                definition.DonationRankPoints.Select(item => new RankPoint(item.Rank, item.Points)).ToList(),
                // Meneruskan mematerialisasi urutan `definition.GoldPointsByQty.Select(item => new QtyPoint(item.Qty, item.Points))` menjadi List; enumerasi
                // dijalankan dan hasilnya disimpan dalam memori sebagai argumen ke konstruktor `RulesetScoringConfig`; Meneruskan fungsi lambda `item => new
                // QtyPoint(item.Qty, item.Points)` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen ke
                // `definition.GoldPointsByQty.Select`; Meneruskan `item.Qty` (nilai qty) sebagai argumen ke konstruktor `QtyPoint`; Meneruskan `item.Points` (nilai
                // poin) sebagai argumen ke konstruktor `QtyPoint`.
                definition.GoldPointsByQty.Select(item => new QtyPoint(item.Qty, item.Points)).ToList(),
                // Meneruskan mematerialisasi urutan `definition.PensionRankPoints.Select(item => new RankPoint(item.Rank, item.Points))` menjadi List; enumerasi
                // dijalankan dan hasilnya disimpan dalam memori sebagai argumen ke konstruktor `RulesetScoringConfig`; Meneruskan fungsi lambda `item => new
                // RankPoint(item.Rank, item.Points)` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen ke
                // `definition.PensionRankPoints.Select`; Meneruskan `item.Rank` (nilai rank) sebagai argumen ke konstruktor `RankPoint`; Meneruskan `item.Points`
                // (nilai poin) sebagai argumen ke konstruktor `RankPoint`.
                definition.PensionRankPoints.Select(item => new RankPoint(item.Rank, item.Points)).ToList()))
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryBuildConfig.
        {
            // Memperbarui `FinishDay` menggunakan `settings.FinishDay` (nilai finish hari) dalam TryBuildConfig.
            FinishDay = settings.FinishDay,
            // Memperbarui `NeedSetBonuses` menggunakan mematerialisasi urutan `definition.NeedSetBonuses` menjadi List; enumerasi dijalankan dan hasilnya
            // disimpan dalam memori dalam TryBuildConfig.
            NeedSetBonuses = definition.NeedSetBonuses.ToList(),
            // Memperbarui `GoldPrices` menggunakan mematerialisasi urutan `definition.GoldPrices` menjadi List; enumerasi dijalankan dan hasilnya disimpan
            // dalam memori dalam TryBuildConfig.
            GoldPrices = definition.GoldPrices.ToList(),
            // Memperbarui `ShariaLoans` menggunakan mematerialisasi urutan `definition.ShariaLoans` menjadi List; enumerasi dijalankan dan hasilnya disimpan
            // dalam memori dalam TryBuildConfig.
            ShariaLoans = definition.ShariaLoans.ToList(),
            // Memperbarui `InsuranceProducts` menggunakan mematerialisasi urutan `definition.InsuranceProducts` menjadi List; enumerasi dijalankan dan hasilnya
            // disimpan dalam memori dalam TryBuildConfig.
            InsuranceProducts = definition.InsuranceProducts.ToList(),
            // Memperbarui `LifeRisks` menggunakan mematerialisasi urutan `definition.LifeRisks` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam
            // memori dalam TryBuildConfig.
            LifeRisks = definition.LifeRisks.ToList(),
            // Memperbarui `Orders` menggunakan mematerialisasi urutan `definition.Orders` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori
            // dalam TryBuildConfig.
            Orders = definition.Orders.ToList(),
            // Memperbarui `Ingredients` menggunakan mematerialisasi urutan `definition.Ingredients` menjadi List; enumerasi dijalankan dan hasilnya disimpan
            // dalam memori dalam TryBuildConfig.
            Ingredients = definition.Ingredients.ToList(),
            // Memperbarui `FinancialGoals` menggunakan mematerialisasi urutan `definition.FinancialGoals` menjadi List; enumerasi dijalankan dan hasilnya
            // disimpan dalam memori dalam TryBuildConfig.
            FinancialGoals = definition.FinancialGoals.ToList(),
            // Memperbarui `Needs` menggunakan mematerialisasi urutan `definition.Needs` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori
            // dalam TryBuildConfig.
            Needs = definition.Needs.ToList(),
            // Memperbarui `Actions` menggunakan mematerialisasi urutan `definition.Actions` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam
            // memori dalam TryBuildConfig.
            Actions = definition.Actions.ToList()
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam TryBuildConfig.
        };
        // Mengembalikan true, yaitu kondisi aktif/terpenuhi kepada pemanggil dalam TryBuildConfig; eksekusi jalur ini selesai setelah nilai hasil
        // ditentukan.
        return true;
    // Menutup scope metode TryBuildConfig; bagian berikut berada di luar batas blok tersebut dalam TryBuildConfig.
    }

    // Mendefinisikan metode `ValidateNeedSetBonuses` dengan hasil bertipe `void`; operasi ini menangani validate kebutuhan set bonuses. Masukan:
    // Parameter `bonuses` bertipe `IReadOnlyCollection<RulesetNeedSetBonusDto>` membawa nilai bonuses; Parameter `errors` bertipe `List<ErrorDetail>`
    // membawa nilai kesalahan.
    private static void ValidateNeedSetBonuses(
        // Parameter `bonuses` bertipe `IReadOnlyCollection<RulesetNeedSetBonusDto>` membawa nilai bonuses.
        IReadOnlyCollection<RulesetNeedSetBonusDto> bonuses,
        // Parameter `errors` bertipe `List<ErrorDetail>` membawa nilai kesalahan.
        List<ErrorDetail> errors)
    // Membuka scope metode ValidateNeedSetBonuses; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateNeedSetBonuses.
    {
        // Menyiapkan variabel lokal `seenPatterns` untuk nilai seen patterns dengan objek baru bertipe `HashSet<string>` dengan argumen
        // (StringComparer.OrdinalIgnoreCase). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var seenPatterns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        // Mengulangi setiap elemen `bonuses`; elemen saat ini disimpan sebagai `bonus` bertipe `var` untuk diproses oleh badan loop dalam
        // ValidateNeedSetBonuses.
        foreach (var bonus in bonuses)
        // Membuka scope loop setiap bonus dari `bonuses`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateNeedSetBonuses.
        {
            // Menyiapkan variabel lokal `patternCode` untuk nilai pattern kode dengan `bonus.PatternCode?.Trim().ToUpperInvariant()` bila tidak null; jika null
            // gunakan `string.Empty` sebagai nilai pengganti. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var patternCode = bonus.PatternCode?.Trim().ToUpperInvariant() ?? string.Empty;
            // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `patternCode is not (”THREE_DIFFERENT” or ”THREE_SAME”) ||
            // bonus.RequiredCount <= 0` dan `bonus.Points < 0`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini
            // bernilai benar dalam ValidateNeedSetBonuses.
            if (patternCode is not ("THREE_DIFFERENT" or "THREE_SAME") ||
                // Melanjutkan ekspresi dengan pemeriksaan lebih kecil atau sama antara `bonus.RequiredCount` dan `0` dalam ValidateNeedSetBonuses.
                bonus.RequiredCount <= 0 ||
                // Melanjutkan ekspresi dengan pemeriksaan lebih kecil antara `bonus.Points` dan `0` dalam ValidateNeedSetBonuses.
                bonus.Points < 0)
            // Membuka scope cabang if untuk kondisi `patternCode is not (”THREE_DIFFERENT” or ”THREE_SAME”) || bonus.RequiredCount <= 0 || bonus.Points < 0`;
            // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateNeedSetBonuses.
            {
                // Menjalankan menambahkan `new ErrorDetail(”definition.need_set_bonuses”, ”INVALID”)` ke `errors` dalam ValidateNeedSetBonuses.
                errors.Add(new ErrorDetail("definition.need_set_bonuses", "INVALID"));
            // Menutup scope cabang if untuk kondisi `patternCode is not (”THREE_DIFFERENT” or ”THREE_SAME”) || bonus.RequiredCount <= 0 || bonus.Points < 0`;
            // bagian berikut berada di luar batas blok tersebut dalam ValidateNeedSetBonuses.
            }

            // Memeriksa kebalikan kondisi `seenPatterns.Add(patternCode)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
            // ValidateNeedSetBonuses.
            if (!seenPatterns.Add(patternCode))
            // Membuka scope cabang if untuk kondisi `!seenPatterns.Add(patternCode)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ValidateNeedSetBonuses.
            {
                // Menjalankan menambahkan `new ErrorDetail(”definition.need_set_bonuses”, ”DUPLICATE”)` ke `errors` dalam ValidateNeedSetBonuses.
                errors.Add(new ErrorDetail("definition.need_set_bonuses", "DUPLICATE"));
            // Menutup scope cabang if untuk kondisi `!seenPatterns.Add(patternCode)`; bagian berikut berada di luar batas blok tersebut dalam
            // ValidateNeedSetBonuses.
            }
        // Menutup scope loop setiap bonus dari `bonuses`; bagian berikut berada di luar batas blok tersebut dalam ValidateNeedSetBonuses.
        }
    // Menutup scope metode ValidateNeedSetBonuses; bagian berikut berada di luar batas blok tersebut dalam ValidateNeedSetBonuses.
    }

    // Mendefinisikan metode `ValidateGoldPrices` dengan hasil bertipe `void`; operasi ini menangani validate emas prices. Masukan: Parameter `prices`
    // bertipe `IReadOnlyCollection<RulesetGoldPriceDto>` membawa nilai prices; Parameter `errors` bertipe `List<ErrorDetail>` membawa nilai kesalahan.
    private static void ValidateGoldPrices(
        // Parameter `prices` bertipe `IReadOnlyCollection<RulesetGoldPriceDto>` membawa nilai prices.
        IReadOnlyCollection<RulesetGoldPriceDto> prices,
        // Parameter `errors` bertipe `List<ErrorDetail>` membawa nilai kesalahan.
        List<ErrorDetail> errors)
    // Membuka scope metode ValidateGoldPrices; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateGoldPrices.
    {
        // Menyiapkan variabel lokal `seenCodes` untuk nilai seen kode dengan objek baru bertipe `HashSet<string>` dengan argumen
        // (StringComparer.OrdinalIgnoreCase). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var seenCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        // Mengulangi setiap elemen `prices`; elemen saat ini disimpan sebagai `price` bertipe `var` untuk diproses oleh badan loop dalam
        // ValidateGoldPrices.
        foreach (var price in prices)
        // Membuka scope loop setiap price dari `prices`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateGoldPrices.
        {
            // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `string.IsNullOrWhiteSpace(price.PriceCode) || price.Qty <= 0` dan
            // `price.UnitPrice <= 0`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
            // ValidateGoldPrices.
            if (string.IsNullOrWhiteSpace(price.PriceCode) || price.Qty <= 0 || price.UnitPrice <= 0)
            // Membuka scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(price.PriceCode) || price.Qty <= 0 || price.UnitPrice <= 0`;
            // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateGoldPrices.
            {
                // Menjalankan menambahkan `new ErrorDetail(”definition.gold_prices”, ”OUT_OF_RANGE”)` ke `errors` dalam ValidateGoldPrices.
                errors.Add(new ErrorDetail("definition.gold_prices", "OUT_OF_RANGE"));
            // Menutup scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(price.PriceCode) || price.Qty <= 0 || price.UnitPrice <= 0`; bagian berikut
            // berada di luar batas blok tersebut dalam ValidateGoldPrices.
            }

            // Memeriksa kebalikan kondisi `seenCodes.Add(price.PriceCode)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
            // ValidateGoldPrices.
            if (!seenCodes.Add(price.PriceCode))
            // Membuka scope cabang if untuk kondisi `!seenCodes.Add(price.PriceCode)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ValidateGoldPrices.
            {
                // Menjalankan menambahkan `new ErrorDetail(”definition.gold_prices”, ”DUPLICATE”)` ke `errors` dalam ValidateGoldPrices.
                errors.Add(new ErrorDetail("definition.gold_prices", "DUPLICATE"));
            // Menutup scope cabang if untuk kondisi `!seenCodes.Add(price.PriceCode)`; bagian berikut berada di luar batas blok tersebut dalam
            // ValidateGoldPrices.
            }
        // Menutup scope loop setiap price dari `prices`; bagian berikut berada di luar batas blok tersebut dalam ValidateGoldPrices.
        }
    // Menutup scope metode ValidateGoldPrices; bagian berikut berada di luar batas blok tersebut dalam ValidateGoldPrices.
    }

    // Mendefinisikan metode `ValidateShariaLoans` dengan hasil bertipe `void`; operasi ini menangani validate sharia pinjaman. Masukan: Parameter
    // `loans` bertipe `IReadOnlyCollection<RulesetShariaLoanDto>` membawa nilai pinjaman; Parameter `enabled` bertipe `bool` membawa nilai enabled;
    // Parameter `errors` bertipe `List<ErrorDetail>` membawa nilai kesalahan.
    private static void ValidateShariaLoans(
        // Parameter `loans` bertipe `IReadOnlyCollection<RulesetShariaLoanDto>` membawa nilai pinjaman.
        IReadOnlyCollection<RulesetShariaLoanDto> loans,
        // Parameter `enabled` bertipe `bool` membawa nilai enabled.
        bool enabled,
        // Parameter `errors` bertipe `List<ErrorDetail>` membawa nilai kesalahan.
        List<ErrorDetail> errors)
    // Membuka scope metode ValidateShariaLoans; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateShariaLoans.
    {
        // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `enabled` dan `loans.Count == 0`; sisi kanan diperiksa hanya jika sisi kiri
        // benar; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ValidateShariaLoans.
        if (enabled && loans.Count == 0)
        // Membuka scope cabang if untuk kondisi `enabled && loans.Count == 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ValidateShariaLoans.
        {
            // Menjalankan menambahkan `new ErrorDetail(”definition.sharia_loans”, ”REQUIRED”)` ke `errors` dalam ValidateShariaLoans.
            errors.Add(new ErrorDetail("definition.sharia_loans", "REQUIRED"));
            // Mengakhiri eksekusi lebih awal dalam ValidateShariaLoans tanpa mengembalikan nilai; pernyataan sesudah return pada jalur ini tidak dijalankan.
            return;
        // Menutup scope cabang if untuk kondisi `enabled && loans.Count == 0`; bagian berikut berada di luar batas blok tersebut dalam ValidateShariaLoans.
        }

        // Menyiapkan variabel lokal `seenCodes` untuk nilai seen kode dengan objek baru bertipe `HashSet<string>` dengan argumen
        // (StringComparer.OrdinalIgnoreCase). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var seenCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        // Mengulangi setiap elemen `loans`; elemen saat ini disimpan sebagai `loan` bertipe `var` untuk diproses oleh badan loop dalam ValidateShariaLoans.
        foreach (var loan in loans)
        // Membuka scope loop setiap loan dari `loans`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateShariaLoans.
        {
            // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `string.IsNullOrWhiteSpace(loan.LoanCode) || loan.Principal <= 0 ||
            // loan.RepaymentAmount < 0 || loan.DurationDays <= 0` dan `loan.PenaltyPoints < 0`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya
            // dijalankan ketika kondisi ini bernilai benar dalam ValidateShariaLoans.
            if (string.IsNullOrWhiteSpace(loan.LoanCode) ||
                // Melanjutkan ekspresi dengan pemeriksaan lebih kecil atau sama antara `loan.Principal` dan `0` dalam ValidateShariaLoans.
                loan.Principal <= 0 ||
                // Melanjutkan ekspresi dengan pemeriksaan lebih kecil antara `loan.RepaymentAmount` dan `0` dalam ValidateShariaLoans.
                loan.RepaymentAmount < 0 ||
                // Melanjutkan ekspresi dengan pemeriksaan lebih kecil atau sama antara `loan.DurationDays` dan `0` dalam ValidateShariaLoans.
                loan.DurationDays <= 0 ||
                // Melanjutkan ekspresi dengan pemeriksaan lebih kecil antara `loan.PenaltyPoints` dan `0` dalam ValidateShariaLoans.
                loan.PenaltyPoints < 0)
            // Membuka scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(loan.LoanCode) || loan.Principal <= 0 || loan.RepaymentAmount < 0 ||
            // loan.DurationDays <= 0 || loan.PenaltyPoints < 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateShariaLoans.
            {
                // Menjalankan menambahkan `new ErrorDetail(”definition.sharia_loans”, ”OUT_OF_RANGE”)` ke `errors` dalam ValidateShariaLoans.
                errors.Add(new ErrorDetail("definition.sharia_loans", "OUT_OF_RANGE"));
            // Menutup scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(loan.LoanCode) || loan.Principal <= 0 || loan.RepaymentAmount < 0 ||
            // loan.DurationDays <= 0 || loan.PenaltyPoints < 0`; bagian berikut berada di luar batas blok tersebut dalam ValidateShariaLoans.
            }

            // Memeriksa kebalikan kondisi `seenCodes.Add(loan.LoanCode)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ValidateShariaLoans.
            if (!seenCodes.Add(loan.LoanCode))
            // Membuka scope cabang if untuk kondisi `!seenCodes.Add(loan.LoanCode)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ValidateShariaLoans.
            {
                // Menjalankan menambahkan `new ErrorDetail(”definition.sharia_loans”, ”DUPLICATE”)` ke `errors` dalam ValidateShariaLoans.
                errors.Add(new ErrorDetail("definition.sharia_loans", "DUPLICATE"));
            // Menutup scope cabang if untuk kondisi `!seenCodes.Add(loan.LoanCode)`; bagian berikut berada di luar batas blok tersebut dalam
            // ValidateShariaLoans.
            }
        // Menutup scope loop setiap loan dari `loans`; bagian berikut berada di luar batas blok tersebut dalam ValidateShariaLoans.
        }
    // Menutup scope metode ValidateShariaLoans; bagian berikut berada di luar batas blok tersebut dalam ValidateShariaLoans.
    }

    // Mendefinisikan metode `ValidateInsuranceProducts` dengan hasil bertipe `void`; operasi ini menangani validate asuransi products. Masukan:
    // Parameter `products` bertipe `IReadOnlyCollection<RulesetInsuranceProductDto>` membawa nilai products; Parameter `enabled` bertipe `bool` membawa
    // nilai enabled; Parameter `errors` bertipe `List<ErrorDetail>` membawa nilai kesalahan.
    private static void ValidateInsuranceProducts(
        // Parameter `products` bertipe `IReadOnlyCollection<RulesetInsuranceProductDto>` membawa nilai products.
        IReadOnlyCollection<RulesetInsuranceProductDto> products,
        // Parameter `enabled` bertipe `bool` membawa nilai enabled.
        bool enabled,
        // Parameter `errors` bertipe `List<ErrorDetail>` membawa nilai kesalahan.
        List<ErrorDetail> errors)
    // Membuka scope metode ValidateInsuranceProducts; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateInsuranceProducts.
    {
        // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `enabled` dan `products.Count == 0`; sisi kanan diperiksa hanya jika sisi kiri
        // benar; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ValidateInsuranceProducts.
        if (enabled && products.Count == 0)
        // Membuka scope cabang if untuk kondisi `enabled && products.Count == 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ValidateInsuranceProducts.
        {
            // Menjalankan menambahkan `new ErrorDetail(”definition.insurance_products”, ”REQUIRED”)` ke `errors` dalam ValidateInsuranceProducts.
            errors.Add(new ErrorDetail("definition.insurance_products", "REQUIRED"));
            // Mengakhiri eksekusi lebih awal dalam ValidateInsuranceProducts tanpa mengembalikan nilai; pernyataan sesudah return pada jalur ini tidak
            // dijalankan.
            return;
        // Menutup scope cabang if untuk kondisi `enabled && products.Count == 0`; bagian berikut berada di luar batas blok tersebut dalam
        // ValidateInsuranceProducts.
        }

        // Menyiapkan variabel lokal `seenCodes` untuk nilai seen kode dengan objek baru bertipe `HashSet<string>` dengan argumen
        // (StringComparer.OrdinalIgnoreCase). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var seenCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        // Mengulangi setiap elemen `products`; elemen saat ini disimpan sebagai `product` bertipe `var` untuk diproses oleh badan loop dalam
        // ValidateInsuranceProducts.
        foreach (var product in products)
        // Membuka scope loop setiap product dari `products`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateInsuranceProducts.
        {
            // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `string.IsNullOrWhiteSpace(product.ProductCode) || product.Premium <= 0`
            // dan `product.UsageLimit <= 0`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
            // ValidateInsuranceProducts.
            if (string.IsNullOrWhiteSpace(product.ProductCode) ||
                // Melanjutkan ekspresi dengan pemeriksaan lebih kecil atau sama antara `product.Premium` dan `0` dalam ValidateInsuranceProducts.
                product.Premium <= 0 ||
                // Melanjutkan ekspresi dengan pemeriksaan lebih kecil atau sama antara `product.UsageLimit` dan `0` dalam ValidateInsuranceProducts.
                product.UsageLimit <= 0)
            // Membuka scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(product.ProductCode) || product.Premium <= 0 || product.UsageLimit <= 0`;
            // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateInsuranceProducts.
            {
                // Menjalankan menambahkan `new ErrorDetail(”definition.insurance_products”, ”OUT_OF_RANGE”)` ke `errors` dalam ValidateInsuranceProducts.
                errors.Add(new ErrorDetail("definition.insurance_products", "OUT_OF_RANGE"));
            // Menutup scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(product.ProductCode) || product.Premium <= 0 || product.UsageLimit <= 0`; bagian
            // berikut berada di luar batas blok tersebut dalam ValidateInsuranceProducts.
            }

            // Memeriksa kebalikan kondisi `seenCodes.Add(product.ProductCode)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
            // ValidateInsuranceProducts.
            if (!seenCodes.Add(product.ProductCode))
            // Membuka scope cabang if untuk kondisi `!seenCodes.Add(product.ProductCode)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ValidateInsuranceProducts.
            {
                // Menjalankan menambahkan `new ErrorDetail(”definition.insurance_products”, ”DUPLICATE”)` ke `errors` dalam ValidateInsuranceProducts.
                errors.Add(new ErrorDetail("definition.insurance_products", "DUPLICATE"));
            // Menutup scope cabang if untuk kondisi `!seenCodes.Add(product.ProductCode)`; bagian berikut berada di luar batas blok tersebut dalam
            // ValidateInsuranceProducts.
            }
        // Menutup scope loop setiap product dari `products`; bagian berikut berada di luar batas blok tersebut dalam ValidateInsuranceProducts.
        }
    // Menutup scope metode ValidateInsuranceProducts; bagian berikut berada di luar batas blok tersebut dalam ValidateInsuranceProducts.
    }

    // Mendefinisikan metode `ValidateLifeRisks` dengan hasil bertipe `void`; operasi ini menangani validate life risks. Masukan: Parameter `risks`
    // bertipe `IReadOnlyCollection<RulesetLifeRiskDto>` membawa nilai risks; Parameter `errors` bertipe `List<ErrorDetail>` membawa nilai kesalahan.
    private static void ValidateLifeRisks(
        // Parameter `risks` bertipe `IReadOnlyCollection<RulesetLifeRiskDto>` membawa nilai risks.
        IReadOnlyCollection<RulesetLifeRiskDto> risks,
        // Parameter `errors` bertipe `List<ErrorDetail>` membawa nilai kesalahan.
        List<ErrorDetail> errors)
    // Membuka scope metode ValidateLifeRisks; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateLifeRisks.
    {
        // Menyiapkan variabel lokal `seenCodes` untuk nilai seen kode dengan objek baru bertipe `HashSet<string>` dengan argumen
        // (StringComparer.OrdinalIgnoreCase). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var seenCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        // Mengulangi setiap elemen `risks`; elemen saat ini disimpan sebagai `risk` bertipe `var` untuk diproses oleh badan loop dalam ValidateLifeRisks.
        foreach (var risk in risks)
        // Membuka scope loop setiap risk dari `risks`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateLifeRisks.
        {
            // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `string.IsNullOrWhiteSpace(risk.RiskCode) ||
            // string.IsNullOrWhiteSpace(risk.EffectType) || (!string.IsNullOrWhiteSpace(risk.Direction) && risk.Direction is not (”IN” or ”OUT”))` dan
            // `risk.Amount < 0`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
            // ValidateLifeRisks.
            if (string.IsNullOrWhiteSpace(risk.RiskCode) ||
                // Melanjutkan pengolahan dengan memeriksa apakah `risk.EffectType` null, kosong, atau hanya berisi karakter spasi dalam ValidateLifeRisks.
                string.IsNullOrWhiteSpace(risk.EffectType) ||
                // Menggunakan gabungan syarat AND: kedua kondisi wajib benar antara `!string.IsNullOrWhiteSpace(risk.Direction)` dan `risk.Direction is not (”IN”
                // or ”OUT”)`; sisi kanan diperiksa hanya jika sisi kiri benar sebagai bagian ekspresi yang sedang disusun dalam ValidateLifeRisks.
                (!string.IsNullOrWhiteSpace(risk.Direction) && risk.Direction is not ("IN" or "OUT")) ||
                // Melanjutkan ekspresi dengan pemeriksaan lebih kecil antara `risk.Amount` dan `0` dalam ValidateLifeRisks.
                risk.Amount < 0)
            // Membuka scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(risk.RiskCode) || string.IsNullOrWhiteSpace(risk.EffectType) ||
            // (!string.IsNullOrWhiteSpace(risk.Direction) && risk.Direction is not (”IN” or ”OUT”))...`; pernyataan/deklarasi berikut berada di dalam batas
            // blok ini dalam ValidateLifeRisks.
            {
                // Menjalankan menambahkan `new ErrorDetail(”definition.life_risks”, ”INVALID”)` ke `errors` dalam ValidateLifeRisks.
                errors.Add(new ErrorDetail("definition.life_risks", "INVALID"));
            // Menutup scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(risk.RiskCode) || string.IsNullOrWhiteSpace(risk.EffectType) ||
            // (!string.IsNullOrWhiteSpace(risk.Direction) && risk.Direction is not (”IN” or ”OUT”))...`; bagian berikut berada di luar batas blok tersebut
            // dalam ValidateLifeRisks.
            }

            // Memeriksa kebalikan kondisi `seenCodes.Add(risk.RiskCode)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ValidateLifeRisks.
            if (!seenCodes.Add(risk.RiskCode))
            // Membuka scope cabang if untuk kondisi `!seenCodes.Add(risk.RiskCode)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ValidateLifeRisks.
            {
                // Menjalankan menambahkan `new ErrorDetail(”definition.life_risks”, ”DUPLICATE”)` ke `errors` dalam ValidateLifeRisks.
                errors.Add(new ErrorDetail("definition.life_risks", "DUPLICATE"));
            // Menutup scope cabang if untuk kondisi `!seenCodes.Add(risk.RiskCode)`; bagian berikut berada di luar batas blok tersebut dalam ValidateLifeRisks.
            }
        // Menutup scope loop setiap risk dari `risks`; bagian berikut berada di luar batas blok tersebut dalam ValidateLifeRisks.
        }
    // Menutup scope metode ValidateLifeRisks; bagian berikut berada di luar batas blok tersebut dalam ValidateLifeRisks.
    }

    // Mendefinisikan metode `ValidateRankPoints` dengan hasil bertipe `void`; operasi ini menangani validate rank poin. Masukan: Parameter `points`
    // bertipe `IReadOnlyCollection<RulesetDonationRankPointDto>` membawa nilai poin; Parameter `field` bertipe `string` membawa nilai field; Parameter
    // `errors` bertipe `List<ErrorDetail>` membawa nilai kesalahan.
    private static void ValidateRankPoints(IReadOnlyCollection<RulesetDonationRankPointDto> points, string field, List<ErrorDetail> errors)
    // Membuka scope metode ValidateRankPoints; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateRankPoints.
    {
        // Menyiapkan variabel lokal `seen` untuk nilai seen dengan objek baru bertipe `HashSet<int>` dengan nilai awal sesuai konstruktornya. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var seen = new HashSet<int>();
        // Mengulangi setiap elemen `points`; elemen saat ini disimpan sebagai `point` bertipe `var` untuk diproses oleh badan loop dalam
        // ValidateRankPoints.
        foreach (var point in points)
        // Membuka scope loop setiap point dari `points`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateRankPoints.
        {
            // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `point.Rank <= 0` dan `point.Points < 0`; sisi kanan diperiksa hanya
            // jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ValidateRankPoints.
            if (point.Rank <= 0 || point.Points < 0)
            // Membuka scope cabang if untuk kondisi `point.Rank <= 0 || point.Points < 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ValidateRankPoints.
            {
                // Menjalankan menambahkan `new ErrorDetail(field, ”OUT_OF_RANGE”)` ke `errors` dalam ValidateRankPoints.
                errors.Add(new ErrorDetail(field, "OUT_OF_RANGE"));
                // Melewati sisa pernyataan pada iterasi saat ini dan melanjutkan ke elemen/iterasi berikutnya dalam ValidateRankPoints.
                continue;
            // Menutup scope cabang if untuk kondisi `point.Rank <= 0 || point.Points < 0`; bagian berikut berada di luar batas blok tersebut dalam
            // ValidateRankPoints.
            }

            // Memeriksa kebalikan kondisi `seen.Add(point.Rank)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ValidateRankPoints.
            if (!seen.Add(point.Rank))
            // Membuka scope cabang if untuk kondisi `!seen.Add(point.Rank)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ValidateRankPoints.
            {
                // Menjalankan menambahkan `new ErrorDetail(field, ”DUPLICATE”)` ke `errors` dalam ValidateRankPoints.
                errors.Add(new ErrorDetail(field, "DUPLICATE"));
            // Menutup scope cabang if untuk kondisi `!seen.Add(point.Rank)`; bagian berikut berada di luar batas blok tersebut dalam ValidateRankPoints.
            }
        // Menutup scope loop setiap point dari `points`; bagian berikut berada di luar batas blok tersebut dalam ValidateRankPoints.
        }
    // Menutup scope metode ValidateRankPoints; bagian berikut berada di luar batas blok tersebut dalam ValidateRankPoints.
    }

    // Mendefinisikan metode `ValidateRankPoints` dengan hasil bertipe `void`; operasi ini menangani validate rank poin. Masukan: Parameter `points`
    // bertipe `IReadOnlyCollection<RulesetPensionRankPointDto>` membawa nilai poin; Parameter `field` bertipe `string` membawa nilai field; Parameter
    // `errors` bertipe `List<ErrorDetail>` membawa nilai kesalahan.
    private static void ValidateRankPoints(IReadOnlyCollection<RulesetPensionRankPointDto> points, string field, List<ErrorDetail> errors)
    // Membuka scope metode ValidateRankPoints; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateRankPoints.
    {
        // Menyiapkan variabel lokal `seen` untuk nilai seen dengan objek baru bertipe `HashSet<int>` dengan nilai awal sesuai konstruktornya. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var seen = new HashSet<int>();
        // Mengulangi setiap elemen `points`; elemen saat ini disimpan sebagai `point` bertipe `var` untuk diproses oleh badan loop dalam
        // ValidateRankPoints.
        foreach (var point in points)
        // Membuka scope loop setiap point dari `points`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateRankPoints.
        {
            // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `point.Rank <= 0` dan `point.Points < 0`; sisi kanan diperiksa hanya
            // jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ValidateRankPoints.
            if (point.Rank <= 0 || point.Points < 0)
            // Membuka scope cabang if untuk kondisi `point.Rank <= 0 || point.Points < 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ValidateRankPoints.
            {
                // Menjalankan menambahkan `new ErrorDetail(field, ”OUT_OF_RANGE”)` ke `errors` dalam ValidateRankPoints.
                errors.Add(new ErrorDetail(field, "OUT_OF_RANGE"));
                // Melewati sisa pernyataan pada iterasi saat ini dan melanjutkan ke elemen/iterasi berikutnya dalam ValidateRankPoints.
                continue;
            // Menutup scope cabang if untuk kondisi `point.Rank <= 0 || point.Points < 0`; bagian berikut berada di luar batas blok tersebut dalam
            // ValidateRankPoints.
            }

            // Memeriksa kebalikan kondisi `seen.Add(point.Rank)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ValidateRankPoints.
            if (!seen.Add(point.Rank))
            // Membuka scope cabang if untuk kondisi `!seen.Add(point.Rank)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ValidateRankPoints.
            {
                // Menjalankan menambahkan `new ErrorDetail(field, ”DUPLICATE”)` ke `errors` dalam ValidateRankPoints.
                errors.Add(new ErrorDetail(field, "DUPLICATE"));
            // Menutup scope cabang if untuk kondisi `!seen.Add(point.Rank)`; bagian berikut berada di luar batas blok tersebut dalam ValidateRankPoints.
            }
        // Menutup scope loop setiap point dari `points`; bagian berikut berada di luar batas blok tersebut dalam ValidateRankPoints.
        }
    // Menutup scope metode ValidateRankPoints; bagian berikut berada di luar batas blok tersebut dalam ValidateRankPoints.
    }

    // Mendefinisikan metode `ValidateQtyPoints` dengan hasil bertipe `void`; operasi ini menangani validate qty poin. Masukan: Parameter `points`
    // bertipe `IReadOnlyCollection<RulesetGoldPointDto>` membawa nilai poin; Parameter `field` bertipe `string` membawa nilai field; Parameter `errors`
    // bertipe `List<ErrorDetail>` membawa nilai kesalahan.
    private static void ValidateQtyPoints(IReadOnlyCollection<RulesetGoldPointDto> points, string field, List<ErrorDetail> errors)
    // Membuka scope metode ValidateQtyPoints; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateQtyPoints.
    {
        // Menyiapkan variabel lokal `seen` untuk nilai seen dengan objek baru bertipe `HashSet<int>` dengan nilai awal sesuai konstruktornya. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var seen = new HashSet<int>();
        // Mengulangi setiap elemen `points`; elemen saat ini disimpan sebagai `point` bertipe `var` untuk diproses oleh badan loop dalam ValidateQtyPoints.
        foreach (var point in points)
        // Membuka scope loop setiap point dari `points`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateQtyPoints.
        {
            // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `point.Qty <= 0` dan `point.Points < 0`; sisi kanan diperiksa hanya jika
            // sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ValidateQtyPoints.
            if (point.Qty <= 0 || point.Points < 0)
            // Membuka scope cabang if untuk kondisi `point.Qty <= 0 || point.Points < 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ValidateQtyPoints.
            {
                // Menjalankan menambahkan `new ErrorDetail(field, ”OUT_OF_RANGE”)` ke `errors` dalam ValidateQtyPoints.
                errors.Add(new ErrorDetail(field, "OUT_OF_RANGE"));
                // Melewati sisa pernyataan pada iterasi saat ini dan melanjutkan ke elemen/iterasi berikutnya dalam ValidateQtyPoints.
                continue;
            // Menutup scope cabang if untuk kondisi `point.Qty <= 0 || point.Points < 0`; bagian berikut berada di luar batas blok tersebut dalam
            // ValidateQtyPoints.
            }

            // Memeriksa kebalikan kondisi `seen.Add(point.Qty)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ValidateQtyPoints.
            if (!seen.Add(point.Qty))
            // Membuka scope cabang if untuk kondisi `!seen.Add(point.Qty)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ValidateQtyPoints.
            {
                // Menjalankan menambahkan `new ErrorDetail(field, ”DUPLICATE”)` ke `errors` dalam ValidateQtyPoints.
                errors.Add(new ErrorDetail(field, "DUPLICATE"));
            // Menutup scope cabang if untuk kondisi `!seen.Add(point.Qty)`; bagian berikut berada di luar batas blok tersebut dalam ValidateQtyPoints.
            }
        // Menutup scope loop setiap point dari `points`; bagian berikut berada di luar batas blok tersebut dalam ValidateQtyPoints.
        }
    // Menutup scope metode ValidateQtyPoints; bagian berikut berada di luar batas blok tersebut dalam ValidateQtyPoints.
    }
// Menutup scope tipe RulesetRuntimeMapper; bagian berikut berada di luar batas blok tersebut.
}

/// <summary>
/// Parser statis yang mengubah JSON konfigurasi ruleset menjadi objek <see cref="RulesetConfig"/> dengan validasi struktur dan batasan domain.
/// </summary>
// Mendefinisikan tipe class `RulesetConfigParser`.
internal static class RulesetConfigParser
// Membuka scope tipe RulesetConfigParser; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    /// <summary>
    /// Mem-parse string JSON menjadi <see cref="RulesetConfig"/>, mengembalikan daftar error validasi jika gagal.
    /// </summary>
    /// <param name="json">String JSON konfigurasi ruleset.</param>
    /// <param name="config">Hasil parse konfigurasi, null jika gagal.</param>
    /// <param name="errors">Daftar detail error validasi.</param>
    /// <returns>True jika parsing dan validasi berhasil.</returns>
    // Mendefinisikan metode `TryParse` dengan hasil bertipe `bool`. Mem-parse string JSON menjadi , mengembalikan daftar error validasi jika gagal.
    // Masukan: Parameter `json` bertipe `string` membawa nilai JSON; Parameter `config` bertipe `RulesetConfig?` membawa konfigurasi aturan permainan
    // yang dipakai untuk validasi dan perhitungan; nilai null diizinkan ketika data opsional belum tersedia; out mengembalikan nilai melalui parameter
    // dan harus diisi oleh metode; Parameter `errors` bertipe `List<ErrorDetail>` membawa nilai kesalahan; out mengembalikan nilai melalui parameter
    // dan harus diisi oleh metode.
    internal static bool TryParse(string json, out RulesetConfig? config, out List<ErrorDetail> errors)
    // Membuka scope metode TryParse; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryParse.
    {
        // Memperbarui `config` menggunakan null, yaitu penanda tidak ada nilai dalam TryParse.
        config = null;
        // Memperbarui `errors` menggunakan objek baru bertipe `List<ErrorDetail>` dengan nilai awal sesuai konstruktornya dalam TryParse.
        errors = new List<ErrorDetail>();

        // Memulai blok try dalam TryParse; exception dari blok ini dapat dialihkan ke catch, sedangkan finally (jika ada) tetap dijalankan saat keluar.
        try
        // Membuka scope penanganan operasi try; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryParse.
        {
            // Menyiapkan variabel lokal `doc` untuk nilai doc dengan memanggil `JsonDocument.Parse` dengan `json`. Tipe variabel disimpulkan dari ekspresi
            // nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
            using var doc = JsonDocument.Parse(json);
            // Mengembalikan mencoba mengonversi `doc.RootElement`, `config`, `errors` melalui `TryParse`; keberhasilan dilaporkan sebagai boolean dan hasil
            // ditempatkan pada argumen out kepada pemanggil dalam TryParse; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return TryParse(doc.RootElement, out config, out errors);
        // Menutup scope penanganan operasi try; bagian berikut berada di luar batas blok tersebut dalam TryParse.
        }
        // Menangani exception `JsonException` melalui variabel dalam TryParse.
        catch (JsonException)
        // Membuka scope penanganan exception catch; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryParse.
        {
            // Menjalankan menambahkan `new ErrorDetail(”config”, ”INVALID_JSON”)` ke `errors` dalam TryParse.
            errors.Add(new ErrorDetail("config", "INVALID_JSON"));
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryParse; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return false;
        // Menutup scope penanganan exception catch; bagian berikut berada di luar batas blok tersebut dalam TryParse.
        }
    // Menutup scope metode TryParse; bagian berikut berada di luar batas blok tersebut dalam TryParse.
    }

    /// <summary>
    /// Mem-parse JsonElement root menjadi <see cref="RulesetConfig"/> dengan validasi lengkap setiap field dan constraint domain.
    /// </summary>
    /// <param name="root">Elemen JSON root dari konfigurasi ruleset.</param>
    /// <param name="config">Hasil parse konfigurasi, null jika gagal.</param>
    /// <param name="errors">Daftar detail error validasi.</param>
    /// <returns>True jika parsing dan validasi berhasil.</returns>
    // Mendefinisikan metode `TryParse` dengan hasil bertipe `bool`. Mem-parse JsonElement root menjadi dengan validasi lengkap setiap field dan
    // constraint domain. Masukan: Parameter `root` bertipe `JsonElement` membawa nilai root; Parameter `config` bertipe `RulesetConfig?` membawa
    // konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan; nilai null diizinkan ketika data opsional belum tersedia; out
    // mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `errors` bertipe `List<ErrorDetail>` membawa nilai kesalahan; out
    // mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    internal static bool TryParse(JsonElement root, out RulesetConfig? config, out List<ErrorDetail> errors)
    // Membuka scope metode TryParse; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryParse.
    {
        // Memperbarui `config` menggunakan null, yaitu penanda tidak ada nilai dalam TryParse.
        config = null;
        // Memperbarui `errors` menggunakan objek baru bertipe `List<ErrorDetail>` dengan nilai awal sesuai konstruktornya dalam TryParse.
        errors = new List<ErrorDetail>();

        // Memeriksa kebalikan kondisi `TryGetString(root, ”mode”, out var mode, errors)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // TryParse.
        if (!TryGetString(root, "mode", out var mode, errors)) return false;
        // Memeriksa kebalikan kondisi `TryGetInt(root, ”actions_per_turn”, out var actionsPerTurn, errors)`; blok if hanya dijalankan ketika kondisi ini
        // bernilai benar dalam TryParse.
        if (!TryGetInt(root, "actions_per_turn", out var actionsPerTurn, errors)) return false;
        // Memeriksa kebalikan kondisi `TryGetInt(root, ”starting_cash”, out var startingCash, errors)`; blok if hanya dijalankan ketika kondisi ini
        // bernilai benar dalam TryParse.
        if (!TryGetInt(root, "starting_cash", out var startingCash, errors)) return false;
        // Menyiapkan variabel lokal `playerOrdering` untuk nilai pemain ordering dengan `PlayerOrdering.PlayerOrder` (nomor urut pemain untuk menentukan
        // urutan tindakan). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var playerOrdering = PlayerOrdering.PlayerOrder;
        // Memeriksa mencari properti JSON `”player_ordering”`, `var playerOrderingElement` pada `root` tanpa menganggap propertinya selalu tersedia; blok
        // if hanya dijalankan ketika kondisi ini bernilai benar dalam TryParse.
        if (root.TryGetProperty("player_ordering", out var playerOrderingElement))
        // Membuka scope cabang if untuk kondisi `root.TryGetProperty(”player_ordering”, out var playerOrderingElement)`; pernyataan/deklarasi berikut
        // berada di dalam batas blok ini dalam TryParse.
        {
            // Memeriksa perbandingan ketidaksamaan antara `playerOrderingElement.ValueKind` dan `JsonValueKind.String`; blok if hanya dijalankan ketika kondisi
            // ini bernilai benar dalam TryParse.
            if (playerOrderingElement.ValueKind != JsonValueKind.String)
            // Membuka scope cabang if untuk kondisi `playerOrderingElement.ValueKind != JsonValueKind.String`; pernyataan/deklarasi berikut berada di dalam
            // batas blok ini dalam TryParse.
            {
                // Menjalankan menambahkan `new ErrorDetail(”config.player_ordering”, ”INVALID_TYPE”)` ke `errors` dalam TryParse.
                errors.Add(new ErrorDetail("config.player_ordering", "INVALID_TYPE"));
                // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryParse; eksekusi jalur ini selesai setelah nilai hasil
                // ditentukan.
                return false;
            // Menutup scope cabang if untuk kondisi `playerOrderingElement.ValueKind != JsonValueKind.String`; bagian berikut berada di luar batas blok
            // tersebut dalam TryParse.
            }

            // Menyiapkan variabel lokal `playerOrderingRaw` untuk nilai pemain ordering raw dengan `playerOrderingElement.GetString()` bila tidak null; jika
            // null gunakan `string.Empty` sebagai nilai pengganti. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var playerOrderingRaw = playerOrderingElement.GetString() ?? string.Empty;
            // Memeriksa kebalikan kondisi `TryParsePlayerOrdering(playerOrderingRaw, out playerOrdering)`; blok if hanya dijalankan ketika kondisi ini bernilai
            // benar dalam TryParse.
            if (!TryParsePlayerOrdering(playerOrderingRaw, out playerOrdering))
            // Membuka scope cabang if untuk kondisi `!TryParsePlayerOrdering(playerOrderingRaw, out playerOrdering)`; pernyataan/deklarasi berikut berada di
            // dalam batas blok ini dalam TryParse.
            {
                // Menjalankan menambahkan `new ErrorDetail(”config.player_ordering”, ”INVALID_ENUM”)` ke `errors` dalam TryParse.
                errors.Add(new ErrorDetail("config.player_ordering", "INVALID_ENUM"));
            // Menutup scope cabang if untuk kondisi `!TryParsePlayerOrdering(playerOrderingRaw, out playerOrdering)`; bagian berikut berada di luar batas blok
            // tersebut dalam TryParse.
            }
        // Menutup scope cabang if untuk kondisi `root.TryGetProperty(”player_ordering”, out var playerOrderingElement)`; bagian berikut berada di luar
        // batas blok tersebut dalam TryParse.
        }

        // Memeriksa kebalikan kondisi `TryGetObject(root, ”weekday_rules”, out var weekdayRules, errors)`; blok if hanya dijalankan ketika kondisi ini
        // bernilai benar dalam TryParse.
        if (!TryGetObject(root, "weekday_rules", out var weekdayRules, errors)) return false;
        // Memeriksa kebalikan kondisi `TryGetObject(weekdayRules, ”friday”, out var fridayRules, errors)`; blok if hanya dijalankan ketika kondisi ini
        // bernilai benar dalam TryParse.
        if (!TryGetObject(weekdayRules, "friday", out var fridayRules, errors)) return false;
        // Memeriksa kebalikan kondisi `TryGetObject(weekdayRules, ”saturday”, out var saturdayRules, errors)`; blok if hanya dijalankan ketika kondisi ini
        // bernilai benar dalam TryParse.
        if (!TryGetObject(weekdayRules, "saturday", out var saturdayRules, errors)) return false;
        // Memeriksa kebalikan kondisi `TryGetObject(weekdayRules, ”sunday”, out var sundayRules, errors)`; blok if hanya dijalankan ketika kondisi ini
        // bernilai benar dalam TryParse.
        if (!TryGetObject(weekdayRules, "sunday", out var sundayRules, errors)) return false;

        // Memeriksa kebalikan kondisi `TryGetBool(fridayRules, ”enabled”, out var fridayEnabled, errors)`; blok if hanya dijalankan ketika kondisi ini
        // bernilai benar dalam TryParse.
        if (!TryGetBool(fridayRules, "enabled", out var fridayEnabled, errors)) return false;
        // Memeriksa kebalikan kondisi `TryGetBool(saturdayRules, ”enabled”, out var saturdayEnabled, errors)`; blok if hanya dijalankan ketika kondisi ini
        // bernilai benar dalam TryParse.
        if (!TryGetBool(saturdayRules, "enabled", out var saturdayEnabled, errors)) return false;
        // Memeriksa kebalikan kondisi `TryGetBool(sundayRules, ”enabled”, out var sundayEnabled, errors)`; blok if hanya dijalankan ketika kondisi ini
        // bernilai benar dalam TryParse.
        if (!TryGetBool(sundayRules, "enabled", out var sundayEnabled, errors)) return false;

        // Memeriksa kebalikan kondisi `TryGetObject(root, ”constraints”, out var constraints, errors)`; blok if hanya dijalankan ketika kondisi ini
        // bernilai benar dalam TryParse.
        if (!TryGetObject(root, "constraints", out var constraints, errors)) return false;
        // Memeriksa kebalikan kondisi `TryGetInt(constraints, ”cash_min”, out var cashMin, errors)`; blok if hanya dijalankan ketika kondisi ini bernilai
        // benar dalam TryParse.
        if (!TryGetInt(constraints, "cash_min", out var cashMin, errors)) return false;
        // Memeriksa kebalikan kondisi `TryGetInt(constraints, ”max_ingredient_total”, out var maxIngredientTotal, errors)`; blok if hanya dijalankan ketika
        // kondisi ini bernilai benar dalam TryParse.
        if (!TryGetInt(constraints, "max_ingredient_total", out var maxIngredientTotal, errors)) return false;
        // Memeriksa kebalikan kondisi `TryGetInt(constraints, ”max_same_ingredient”, out var maxSameIngredient, errors)`; blok if hanya dijalankan ketika
        // kondisi ini bernilai benar dalam TryParse.
        if (!TryGetInt(constraints, "max_same_ingredient", out var maxSameIngredient, errors)) return false;
        // Memeriksa kebalikan kondisi `TryGetInt(constraints, ”primary_need_max_per_day”, out var primaryNeedMaxPerDay, errors)`; blok if hanya dijalankan
        // ketika kondisi ini bernilai benar dalam TryParse.
        if (!TryGetInt(constraints, "primary_need_max_per_day", out var primaryNeedMaxPerDay, errors)) return false;
        // Memeriksa kebalikan kondisi `TryGetBool(constraints, ”require_primary_before_others”, out var requirePrimaryBeforeOthers, errors)`; blok if hanya
        // dijalankan ketika kondisi ini bernilai benar dalam TryParse.
        if (!TryGetBool(constraints, "require_primary_before_others", out var requirePrimaryBeforeOthers, errors)) return false;

        // Memeriksa kebalikan kondisi `TryGetObject(root, ”donation”, out var donation, errors)`; blok if hanya dijalankan ketika kondisi ini bernilai
        // benar dalam TryParse.
        if (!TryGetObject(root, "donation", out var donation, errors)) return false;
        // Memeriksa kebalikan kondisi `TryGetInt(donation, ”min_amount”, out var donationMin, errors)`; blok if hanya dijalankan ketika kondisi ini
        // bernilai benar dalam TryParse.
        if (!TryGetInt(donation, "min_amount", out var donationMin, errors)) return false;
        // Memeriksa kebalikan kondisi `TryGetInt(donation, ”max_amount”, out var donationMax, errors)`; blok if hanya dijalankan ketika kondisi ini
        // bernilai benar dalam TryParse.
        if (!TryGetInt(donation, "max_amount", out var donationMax, errors)) return false;

        // Memeriksa kebalikan kondisi `TryGetObject(root, ”gold_trade”, out var goldTrade, errors)`; blok if hanya dijalankan ketika kondisi ini bernilai
        // benar dalam TryParse.
        if (!TryGetObject(root, "gold_trade", out var goldTrade, errors)) return false;
        // Memeriksa kebalikan kondisi `TryGetBool(goldTrade, ”allow_buy”, out var allowBuy, errors)`; blok if hanya dijalankan ketika kondisi ini bernilai
        // benar dalam TryParse.
        if (!TryGetBool(goldTrade, "allow_buy", out var allowBuy, errors)) return false;
        // Memeriksa kebalikan kondisi `TryGetBool(goldTrade, ”allow_sell”, out var allowSell, errors)`; blok if hanya dijalankan ketika kondisi ini
        // bernilai benar dalam TryParse.
        if (!TryGetBool(goldTrade, "allow_sell", out var allowSell, errors)) return false;

        // Memeriksa kebalikan kondisi `TryGetObject(root, ”advanced”, out var advanced, errors)`; blok if hanya dijalankan ketika kondisi ini bernilai
        // benar dalam TryParse.
        if (!TryGetObject(root, "advanced", out var advanced, errors)) return false;
        // Memeriksa kebalikan kondisi `TryGetObject(advanced, ”loan”, out var loan, errors)`; blok if hanya dijalankan ketika kondisi ini bernilai benar
        // dalam TryParse.
        if (!TryGetObject(advanced, "loan", out var loan, errors)) return false;
        // Memeriksa kebalikan kondisi `TryGetObject(advanced, ”insurance”, out var insurance, errors)`; blok if hanya dijalankan ketika kondisi ini
        // bernilai benar dalam TryParse.
        if (!TryGetObject(advanced, "insurance", out var insurance, errors)) return false;
        // Memeriksa kebalikan kondisi `TryGetObject(advanced, ”saving_goal”, out var saving, errors)`; blok if hanya dijalankan ketika kondisi ini bernilai
        // benar dalam TryParse.
        if (!TryGetObject(advanced, "saving_goal", out var saving, errors)) return false;
        // Memeriksa kebalikan kondisi `TryGetBool(loan, ”enabled”, out var loanEnabled, errors)`; blok if hanya dijalankan ketika kondisi ini bernilai
        // benar dalam TryParse.
        if (!TryGetBool(loan, "enabled", out var loanEnabled, errors)) return false;
        // Memeriksa kebalikan kondisi `TryGetBool(insurance, ”enabled”, out var insuranceEnabled, errors)`; blok if hanya dijalankan ketika kondisi ini
        // bernilai benar dalam TryParse.
        if (!TryGetBool(insurance, "enabled", out var insuranceEnabled, errors)) return false;
        // Memeriksa kebalikan kondisi `TryGetBool(saving, ”enabled”, out var savingGoalEnabled, errors)`; blok if hanya dijalankan ketika kondisi ini
        // bernilai benar dalam TryParse.
        if (!TryGetBool(saving, "enabled", out var savingGoalEnabled, errors)) return false;

        // Menyiapkan variabel lokal `freelanceIncome` untuk nilai freelance pemasukan dengan nilai literal `1`. Tipe variabel disimpulkan dari ekspresi
        // nilai awal.
        var freelanceIncome = 1;
        // Memeriksa mencari properti JSON `”freelance”`, `var freelanceElement` pada `root` tanpa menganggap propertinya selalu tersedia; blok if hanya
        // dijalankan ketika kondisi ini bernilai benar dalam TryParse.
        if (root.TryGetProperty("freelance", out var freelanceElement))
        // Membuka scope cabang if untuk kondisi `root.TryGetProperty(”freelance”, out var freelanceElement)`; pernyataan/deklarasi berikut berada di dalam
        // batas blok ini dalam TryParse.
        {
            // Memeriksa perbandingan ketidaksamaan antara `freelanceElement.ValueKind` dan `JsonValueKind.Object`; blok if hanya dijalankan ketika kondisi ini
            // bernilai benar dalam TryParse.
            if (freelanceElement.ValueKind != JsonValueKind.Object)
            // Membuka scope cabang if untuk kondisi `freelanceElement.ValueKind != JsonValueKind.Object`; pernyataan/deklarasi berikut berada di dalam batas
            // blok ini dalam TryParse.
            {
                // Menjalankan menambahkan `new ErrorDetail(”config.freelance”, ”INVALID_TYPE”)` ke `errors` dalam TryParse.
                errors.Add(new ErrorDetail("config.freelance", "INVALID_TYPE"));
                // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryParse; eksekusi jalur ini selesai setelah nilai hasil
                // ditentukan.
                return false;
            // Menutup scope cabang if untuk kondisi `freelanceElement.ValueKind != JsonValueKind.Object`; bagian berikut berada di luar batas blok tersebut
            // dalam TryParse.
            }

            // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!freelanceElement.TryGetProperty(”income”, out var incomeProp) ||
            // incomeProp.ValueKind != JsonValueKind.Number` dan `!incomeProp.TryGetInt32(out freelanceIncome)`; sisi kanan diperiksa hanya jika sisi kiri
            // salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam TryParse.
            if (!freelanceElement.TryGetProperty("income", out var incomeProp) ||
                // Melanjutkan ekspresi dengan perbandingan ketidaksamaan antara `incomeProp.ValueKind` dan `JsonValueKind.Number` dalam TryParse.
                incomeProp.ValueKind != JsonValueKind.Number ||
                // Menggunakan kebalikan kondisi `incomeProp.TryGetInt32(out freelanceIncome)` sebagai bagian ekspresi yang sedang disusun dalam TryParse.
                !incomeProp.TryGetInt32(out freelanceIncome))
            // Membuka scope cabang if untuk kondisi `!freelanceElement.TryGetProperty(”income”, out var incomeProp) || incomeProp.ValueKind !=
            // JsonValueKind.Number || !incomeProp.TryGetInt32(out freelanceIncome)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // TryParse.
            {
                // Menjalankan menambahkan `new ErrorDetail(”config.freelance.income”, ”REQUIRED”)` ke `errors` dalam TryParse.
                errors.Add(new ErrorDetail("config.freelance.income", "REQUIRED"));
                // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryParse; eksekusi jalur ini selesai setelah nilai hasil
                // ditentukan.
                return false;
            // Menutup scope cabang if untuk kondisi `!freelanceElement.TryGetProperty(”income”, out var incomeProp) || incomeProp.ValueKind !=
            // JsonValueKind.Number || !incomeProp.TryGetInt32(out freelanceIncome)`; bagian berikut berada di luar batas blok tersebut dalam TryParse.
            }
        // Menutup scope cabang if untuk kondisi `root.TryGetProperty(”freelance”, out var freelanceElement)`; bagian berikut berada di luar batas blok
        // tersebut dalam TryParse.
        }

        // Menyiapkan variabel lokal `scoring` untuk nilai scoring dengan null, yaitu penanda tidak ada nilai. Tipe yang dipakai adalah
        // `RulesetScoringConfig?`.
        RulesetScoringConfig? scoring = null;
        // Memeriksa mencari properti JSON `”scoring”`, `var scoringElement` pada `root` tanpa menganggap propertinya selalu tersedia; blok if hanya
        // dijalankan ketika kondisi ini bernilai benar dalam TryParse.
        if (root.TryGetProperty("scoring", out var scoringElement))
        // Membuka scope cabang if untuk kondisi `root.TryGetProperty(”scoring”, out var scoringElement)`; pernyataan/deklarasi berikut berada di dalam
        // batas blok ini dalam TryParse.
        {
            // Memeriksa perbandingan ketidaksamaan antara `scoringElement.ValueKind` dan `JsonValueKind.Object`; blok if hanya dijalankan ketika kondisi ini
            // bernilai benar dalam TryParse.
            if (scoringElement.ValueKind != JsonValueKind.Object)
            // Membuka scope cabang if untuk kondisi `scoringElement.ValueKind != JsonValueKind.Object`; pernyataan/deklarasi berikut berada di dalam batas blok
            // ini dalam TryParse.
            {
                // Menjalankan menambahkan `new ErrorDetail(”config.scoring”, ”INVALID_TYPE”)` ke `errors` dalam TryParse.
                errors.Add(new ErrorDetail("config.scoring", "INVALID_TYPE"));
                // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryParse; eksekusi jalur ini selesai setelah nilai hasil
                // ditentukan.
                return false;
            // Menutup scope cabang if untuk kondisi `scoringElement.ValueKind != JsonValueKind.Object`; bagian berikut berada di luar batas blok tersebut dalam
            // TryParse.
            }

            // Menyiapkan variabel lokal `donationRankPoints` untuk nilai donasi rank poin dengan objek baru bertipe `List<RankPoint>` dengan nilai awal sesuai
            // konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var donationRankPoints = new List<RankPoint>();
            // Menyiapkan variabel lokal `goldPointsByQty` untuk nilai emas poin berdasarkan qty dengan objek baru bertipe `List<QtyPoint>` dengan nilai awal
            // sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var goldPointsByQty = new List<QtyPoint>();
            // Menyiapkan variabel lokal `pensionRankPoints` untuk nilai pension rank poin dengan objek baru bertipe `List<RankPoint>` dengan nilai awal sesuai
            // konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var pensionRankPoints = new List<RankPoint>();

            // Memeriksa mencari properti JSON `”donation_rank_points”`, `var donationElement` pada `scoringElement` tanpa menganggap propertinya selalu
            // tersedia; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam TryParse.
            if (scoringElement.TryGetProperty("donation_rank_points", out var donationElement))
            // Membuka scope cabang if untuk kondisi `scoringElement.TryGetProperty(”donation_rank_points”, out var donationElement)`; pernyataan/deklarasi
            // berikut berada di dalam batas blok ini dalam TryParse.
            {
                // Memeriksa perbandingan ketidaksamaan antara `donationElement.ValueKind` dan `JsonValueKind.Array`; blok if hanya dijalankan ketika kondisi ini
                // bernilai benar dalam TryParse.
                if (donationElement.ValueKind != JsonValueKind.Array)
                // Membuka scope cabang if untuk kondisi `donationElement.ValueKind != JsonValueKind.Array`; pernyataan/deklarasi berikut berada di dalam batas blok
                // ini dalam TryParse.
                {
                    // Menjalankan menambahkan `new ErrorDetail(”config.scoring.donation_rank_points”, ”INVALID_TYPE”)` ke `errors` dalam TryParse.
                    errors.Add(new ErrorDetail("config.scoring.donation_rank_points", "INVALID_TYPE"));
                    // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryParse; eksekusi jalur ini selesai setelah nilai hasil
                    // ditentukan.
                    return false;
                // Menutup scope cabang if untuk kondisi `donationElement.ValueKind != JsonValueKind.Array`; bagian berikut berada di luar batas blok tersebut dalam
                // TryParse.
                }

                // Menyiapkan variabel lokal `seenRanks` untuk nilai seen ranks dengan objek baru bertipe `HashSet<int>` dengan nilai awal sesuai konstruktornya.
                // Tipe variabel disimpulkan dari ekspresi nilai awal.
                var seenRanks = new HashSet<int>();
                // Mengulangi setiap elemen `donationElement.EnumerateArray()`; elemen saat ini disimpan sebagai `item` bertipe `var` untuk diproses oleh badan loop
                // dalam TryParse.
                foreach (var item in donationElement.EnumerateArray())
                // Membuka scope loop setiap item dari `donationElement.EnumerateArray()`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // TryParse.
                {
                    // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!item.TryGetProperty(”rank”, out var rankProp) ||
                    // !item.TryGetProperty(”points”, out var pointsProp) || !rankProp.TryGetInt32(out var rankValue)` dan `!pointsProp.TryGetInt32(out var
                    // pointsValue)`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam TryParse.
                    if (!item.TryGetProperty("rank", out var rankProp) ||
                        // Menggunakan kebalikan kondisi `item.TryGetProperty(”points”, out var pointsProp)` sebagai bagian ekspresi yang sedang disusun dalam TryParse.
                        !item.TryGetProperty("points", out var pointsProp) ||
                        // Menggunakan kebalikan kondisi `rankProp.TryGetInt32(out var rankValue)` sebagai bagian ekspresi yang sedang disusun dalam TryParse.
                        !rankProp.TryGetInt32(out var rankValue) ||
                        // Menggunakan kebalikan kondisi `pointsProp.TryGetInt32(out var pointsValue)` sebagai bagian ekspresi yang sedang disusun dalam TryParse.
                        !pointsProp.TryGetInt32(out var pointsValue))
                    // Membuka scope cabang if untuk kondisi `!item.TryGetProperty(”rank”, out var rankProp) || !item.TryGetProperty(”points”, out var pointsProp) ||
                    // !rankProp.TryGetInt32(out var rankValue) || !pointsProp.TryGetInt32(out...`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                    // TryParse.
                    {
                        // Menjalankan menambahkan `new ErrorDetail(”config.scoring.donation_rank_points”, ”INVALID_ITEM”)` ke `errors` dalam TryParse.
                        errors.Add(new ErrorDetail("config.scoring.donation_rank_points", "INVALID_ITEM"));
                        // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryParse; eksekusi jalur ini selesai setelah nilai hasil
                        // ditentukan.
                        return false;
                    // Menutup scope cabang if untuk kondisi `!item.TryGetProperty(”rank”, out var rankProp) || !item.TryGetProperty(”points”, out var pointsProp) ||
                    // !rankProp.TryGetInt32(out var rankValue) || !pointsProp.TryGetInt32(out...`; bagian berikut berada di luar batas blok tersebut dalam TryParse.
                    }

                    // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `rankValue <= 0` dan `pointsValue < 0`; sisi kanan diperiksa hanya jika
                    // sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam TryParse.
                    if (rankValue <= 0 || pointsValue < 0)
                    // Membuka scope cabang if untuk kondisi `rankValue <= 0 || pointsValue < 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                    // TryParse.
                    {
                        // Menjalankan menambahkan `new ErrorDetail(”config.scoring.donation_rank_points”, ”OUT_OF_RANGE”)` ke `errors` dalam TryParse.
                        errors.Add(new ErrorDetail("config.scoring.donation_rank_points", "OUT_OF_RANGE"));
                        // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryParse; eksekusi jalur ini selesai setelah nilai hasil
                        // ditentukan.
                        return false;
                    // Menutup scope cabang if untuk kondisi `rankValue <= 0 || pointsValue < 0`; bagian berikut berada di luar batas blok tersebut dalam TryParse.
                    }

                    // Memeriksa kebalikan kondisi `seenRanks.Add(rankValue)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam TryParse.
                    if (!seenRanks.Add(rankValue))
                    // Membuka scope cabang if untuk kondisi `!seenRanks.Add(rankValue)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryParse.
                    {
                        // Menjalankan menambahkan `new ErrorDetail(”config.scoring.donation_rank_points”, ”DUPLICATE”)` ke `errors` dalam TryParse.
                        errors.Add(new ErrorDetail("config.scoring.donation_rank_points", "DUPLICATE"));
                        // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryParse; eksekusi jalur ini selesai setelah nilai hasil
                        // ditentukan.
                        return false;
                    // Menutup scope cabang if untuk kondisi `!seenRanks.Add(rankValue)`; bagian berikut berada di luar batas blok tersebut dalam TryParse.
                    }

                    // Menjalankan menambahkan `new RankPoint(rankValue, pointsValue)` ke `donationRankPoints` dalam TryParse.
                    donationRankPoints.Add(new RankPoint(rankValue, pointsValue));
                // Menutup scope loop setiap item dari `donationElement.EnumerateArray()`; bagian berikut berada di luar batas blok tersebut dalam TryParse.
                }
            // Menutup scope cabang if untuk kondisi `scoringElement.TryGetProperty(”donation_rank_points”, out var donationElement)`; bagian berikut berada di
            // luar batas blok tersebut dalam TryParse.
            }

            // Memeriksa mencari properti JSON `”gold_points_by_qty”`, `var goldElement` pada `scoringElement` tanpa menganggap propertinya selalu tersedia;
            // blok if hanya dijalankan ketika kondisi ini bernilai benar dalam TryParse.
            if (scoringElement.TryGetProperty("gold_points_by_qty", out var goldElement))
            // Membuka scope cabang if untuk kondisi `scoringElement.TryGetProperty(”gold_points_by_qty”, out var goldElement)`; pernyataan/deklarasi berikut
            // berada di dalam batas blok ini dalam TryParse.
            {
                // Memeriksa perbandingan ketidaksamaan antara `goldElement.ValueKind` dan `JsonValueKind.Array`; blok if hanya dijalankan ketika kondisi ini
                // bernilai benar dalam TryParse.
                if (goldElement.ValueKind != JsonValueKind.Array)
                // Membuka scope cabang if untuk kondisi `goldElement.ValueKind != JsonValueKind.Array`; pernyataan/deklarasi berikut berada di dalam batas blok ini
                // dalam TryParse.
                {
                    // Menjalankan menambahkan `new ErrorDetail(”config.scoring.gold_points_by_qty”, ”INVALID_TYPE”)` ke `errors` dalam TryParse.
                    errors.Add(new ErrorDetail("config.scoring.gold_points_by_qty", "INVALID_TYPE"));
                    // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryParse; eksekusi jalur ini selesai setelah nilai hasil
                    // ditentukan.
                    return false;
                // Menutup scope cabang if untuk kondisi `goldElement.ValueKind != JsonValueKind.Array`; bagian berikut berada di luar batas blok tersebut dalam
                // TryParse.
                }

                // Menyiapkan variabel lokal `seenQty` untuk nilai seen qty dengan objek baru bertipe `HashSet<int>` dengan nilai awal sesuai konstruktornya. Tipe
                // variabel disimpulkan dari ekspresi nilai awal.
                var seenQty = new HashSet<int>();
                // Mengulangi setiap elemen `goldElement.EnumerateArray()`; elemen saat ini disimpan sebagai `item` bertipe `var` untuk diproses oleh badan loop
                // dalam TryParse.
                foreach (var item in goldElement.EnumerateArray())
                // Membuka scope loop setiap item dari `goldElement.EnumerateArray()`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryParse.
                {
                    // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!item.TryGetProperty(”qty”, out var qtyProp) ||
                    // !item.TryGetProperty(”points”, out var pointsProp) || !qtyProp.TryGetInt32(out var qtyValue)` dan `!pointsProp.TryGetInt32(out var pointsValue)`;
                    // sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam TryParse.
                    if (!item.TryGetProperty("qty", out var qtyProp) ||
                        // Menggunakan kebalikan kondisi `item.TryGetProperty(”points”, out var pointsProp)` sebagai bagian ekspresi yang sedang disusun dalam TryParse.
                        !item.TryGetProperty("points", out var pointsProp) ||
                        // Menggunakan kebalikan kondisi `qtyProp.TryGetInt32(out var qtyValue)` sebagai bagian ekspresi yang sedang disusun dalam TryParse.
                        !qtyProp.TryGetInt32(out var qtyValue) ||
                        // Menggunakan kebalikan kondisi `pointsProp.TryGetInt32(out var pointsValue)` sebagai bagian ekspresi yang sedang disusun dalam TryParse.
                        !pointsProp.TryGetInt32(out var pointsValue))
                    // Membuka scope cabang if untuk kondisi `!item.TryGetProperty(”qty”, out var qtyProp) || !item.TryGetProperty(”points”, out var pointsProp) ||
                    // !qtyProp.TryGetInt32(out var qtyValue) || !pointsProp.TryGetInt32(out var...`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                    // TryParse.
                    {
                        // Menjalankan menambahkan `new ErrorDetail(”config.scoring.gold_points_by_qty”, ”INVALID_ITEM”)` ke `errors` dalam TryParse.
                        errors.Add(new ErrorDetail("config.scoring.gold_points_by_qty", "INVALID_ITEM"));
                        // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryParse; eksekusi jalur ini selesai setelah nilai hasil
                        // ditentukan.
                        return false;
                    // Menutup scope cabang if untuk kondisi `!item.TryGetProperty(”qty”, out var qtyProp) || !item.TryGetProperty(”points”, out var pointsProp) ||
                    // !qtyProp.TryGetInt32(out var qtyValue) || !pointsProp.TryGetInt32(out var...`; bagian berikut berada di luar batas blok tersebut dalam TryParse.
                    }

                    // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `qtyValue <= 0` dan `pointsValue < 0`; sisi kanan diperiksa hanya jika
                    // sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam TryParse.
                    if (qtyValue <= 0 || pointsValue < 0)
                    // Membuka scope cabang if untuk kondisi `qtyValue <= 0 || pointsValue < 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                    // TryParse.
                    {
                        // Menjalankan menambahkan `new ErrorDetail(”config.scoring.gold_points_by_qty”, ”OUT_OF_RANGE”)` ke `errors` dalam TryParse.
                        errors.Add(new ErrorDetail("config.scoring.gold_points_by_qty", "OUT_OF_RANGE"));
                        // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryParse; eksekusi jalur ini selesai setelah nilai hasil
                        // ditentukan.
                        return false;
                    // Menutup scope cabang if untuk kondisi `qtyValue <= 0 || pointsValue < 0`; bagian berikut berada di luar batas blok tersebut dalam TryParse.
                    }

                    // Memeriksa kebalikan kondisi `seenQty.Add(qtyValue)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam TryParse.
                    if (!seenQty.Add(qtyValue))
                    // Membuka scope cabang if untuk kondisi `!seenQty.Add(qtyValue)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryParse.
                    {
                        // Menjalankan menambahkan `new ErrorDetail(”config.scoring.gold_points_by_qty”, ”DUPLICATE”)` ke `errors` dalam TryParse.
                        errors.Add(new ErrorDetail("config.scoring.gold_points_by_qty", "DUPLICATE"));
                        // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryParse; eksekusi jalur ini selesai setelah nilai hasil
                        // ditentukan.
                        return false;
                    // Menutup scope cabang if untuk kondisi `!seenQty.Add(qtyValue)`; bagian berikut berada di luar batas blok tersebut dalam TryParse.
                    }

                    // Menjalankan menambahkan `new QtyPoint(qtyValue, pointsValue)` ke `goldPointsByQty` dalam TryParse.
                    goldPointsByQty.Add(new QtyPoint(qtyValue, pointsValue));
                // Menutup scope loop setiap item dari `goldElement.EnumerateArray()`; bagian berikut berada di luar batas blok tersebut dalam TryParse.
                }
            // Menutup scope cabang if untuk kondisi `scoringElement.TryGetProperty(”gold_points_by_qty”, out var goldElement)`; bagian berikut berada di luar
            // batas blok tersebut dalam TryParse.
            }

            // Memeriksa mencari properti JSON `”pension_rank_points”`, `var pensionElement` pada `scoringElement` tanpa menganggap propertinya selalu tersedia;
            // blok if hanya dijalankan ketika kondisi ini bernilai benar dalam TryParse.
            if (scoringElement.TryGetProperty("pension_rank_points", out var pensionElement))
            // Membuka scope cabang if untuk kondisi `scoringElement.TryGetProperty(”pension_rank_points”, out var pensionElement)`; pernyataan/deklarasi
            // berikut berada di dalam batas blok ini dalam TryParse.
            {
                // Memeriksa perbandingan ketidaksamaan antara `pensionElement.ValueKind` dan `JsonValueKind.Array`; blok if hanya dijalankan ketika kondisi ini
                // bernilai benar dalam TryParse.
                if (pensionElement.ValueKind != JsonValueKind.Array)
                // Membuka scope cabang if untuk kondisi `pensionElement.ValueKind != JsonValueKind.Array`; pernyataan/deklarasi berikut berada di dalam batas blok
                // ini dalam TryParse.
                {
                    // Menjalankan menambahkan `new ErrorDetail(”config.scoring.pension_rank_points”, ”INVALID_TYPE”)` ke `errors` dalam TryParse.
                    errors.Add(new ErrorDetail("config.scoring.pension_rank_points", "INVALID_TYPE"));
                    // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryParse; eksekusi jalur ini selesai setelah nilai hasil
                    // ditentukan.
                    return false;
                // Menutup scope cabang if untuk kondisi `pensionElement.ValueKind != JsonValueKind.Array`; bagian berikut berada di luar batas blok tersebut dalam
                // TryParse.
                }

                // Menyiapkan variabel lokal `seenRanks` untuk nilai seen ranks dengan objek baru bertipe `HashSet<int>` dengan nilai awal sesuai konstruktornya.
                // Tipe variabel disimpulkan dari ekspresi nilai awal.
                var seenRanks = new HashSet<int>();
                // Mengulangi setiap elemen `pensionElement.EnumerateArray()`; elemen saat ini disimpan sebagai `item` bertipe `var` untuk diproses oleh badan loop
                // dalam TryParse.
                foreach (var item in pensionElement.EnumerateArray())
                // Membuka scope loop setiap item dari `pensionElement.EnumerateArray()`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // TryParse.
                {
                    // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!item.TryGetProperty(”rank”, out var rankProp) ||
                    // !item.TryGetProperty(”points”, out var pointsProp) || !rankProp.TryGetInt32(out var rankValue)` dan `!pointsProp.TryGetInt32(out var
                    // pointsValue)`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam TryParse.
                    if (!item.TryGetProperty("rank", out var rankProp) ||
                        // Menggunakan kebalikan kondisi `item.TryGetProperty(”points”, out var pointsProp)` sebagai bagian ekspresi yang sedang disusun dalam TryParse.
                        !item.TryGetProperty("points", out var pointsProp) ||
                        // Menggunakan kebalikan kondisi `rankProp.TryGetInt32(out var rankValue)` sebagai bagian ekspresi yang sedang disusun dalam TryParse.
                        !rankProp.TryGetInt32(out var rankValue) ||
                        // Menggunakan kebalikan kondisi `pointsProp.TryGetInt32(out var pointsValue)` sebagai bagian ekspresi yang sedang disusun dalam TryParse.
                        !pointsProp.TryGetInt32(out var pointsValue))
                    // Membuka scope cabang if untuk kondisi `!item.TryGetProperty(”rank”, out var rankProp) || !item.TryGetProperty(”points”, out var pointsProp) ||
                    // !rankProp.TryGetInt32(out var rankValue) || !pointsProp.TryGetInt32(out...`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                    // TryParse.
                    {
                        // Menjalankan menambahkan `new ErrorDetail(”config.scoring.pension_rank_points”, ”INVALID_ITEM”)` ke `errors` dalam TryParse.
                        errors.Add(new ErrorDetail("config.scoring.pension_rank_points", "INVALID_ITEM"));
                        // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryParse; eksekusi jalur ini selesai setelah nilai hasil
                        // ditentukan.
                        return false;
                    // Menutup scope cabang if untuk kondisi `!item.TryGetProperty(”rank”, out var rankProp) || !item.TryGetProperty(”points”, out var pointsProp) ||
                    // !rankProp.TryGetInt32(out var rankValue) || !pointsProp.TryGetInt32(out...`; bagian berikut berada di luar batas blok tersebut dalam TryParse.
                    }

                    // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `rankValue <= 0` dan `pointsValue < 0`; sisi kanan diperiksa hanya jika
                    // sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam TryParse.
                    if (rankValue <= 0 || pointsValue < 0)
                    // Membuka scope cabang if untuk kondisi `rankValue <= 0 || pointsValue < 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                    // TryParse.
                    {
                        // Menjalankan menambahkan `new ErrorDetail(”config.scoring.pension_rank_points”, ”OUT_OF_RANGE”)` ke `errors` dalam TryParse.
                        errors.Add(new ErrorDetail("config.scoring.pension_rank_points", "OUT_OF_RANGE"));
                        // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryParse; eksekusi jalur ini selesai setelah nilai hasil
                        // ditentukan.
                        return false;
                    // Menutup scope cabang if untuk kondisi `rankValue <= 0 || pointsValue < 0`; bagian berikut berada di luar batas blok tersebut dalam TryParse.
                    }

                    // Memeriksa kebalikan kondisi `seenRanks.Add(rankValue)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam TryParse.
                    if (!seenRanks.Add(rankValue))
                    // Membuka scope cabang if untuk kondisi `!seenRanks.Add(rankValue)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryParse.
                    {
                        // Menjalankan menambahkan `new ErrorDetail(”config.scoring.pension_rank_points”, ”DUPLICATE”)` ke `errors` dalam TryParse.
                        errors.Add(new ErrorDetail("config.scoring.pension_rank_points", "DUPLICATE"));
                        // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryParse; eksekusi jalur ini selesai setelah nilai hasil
                        // ditentukan.
                        return false;
                    // Menutup scope cabang if untuk kondisi `!seenRanks.Add(rankValue)`; bagian berikut berada di luar batas blok tersebut dalam TryParse.
                    }

                    // Menjalankan menambahkan `new RankPoint(rankValue, pointsValue)` ke `pensionRankPoints` dalam TryParse.
                    pensionRankPoints.Add(new RankPoint(rankValue, pointsValue));
                // Menutup scope loop setiap item dari `pensionElement.EnumerateArray()`; bagian berikut berada di luar batas blok tersebut dalam TryParse.
                }
            // Menutup scope cabang if untuk kondisi `scoringElement.TryGetProperty(”pension_rank_points”, out var pensionElement)`; bagian berikut berada di
            // luar batas blok tersebut dalam TryParse.
            }

            // Memperbarui `scoring` menggunakan objek baru bertipe `RulesetScoringConfig` dengan argumen (donationRankPoints, goldPointsByQty,
            // pensionRankPoints) dalam TryParse.
            scoring = new RulesetScoringConfig(donationRankPoints, goldPointsByQty, pensionRankPoints);
        // Menutup scope cabang if untuk kondisi `root.TryGetProperty(”scoring”, out var scoringElement)`; bagian berikut berada di luar batas blok tersebut
        // dalam TryParse.
        }

        // Menyiapkan variabel lokal `upperMode` untuk nilai upper mode dengan menormalisasi `mode` menjadi huruf besar dengan aturan kultur invariant. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var upperMode = mode.ToUpperInvariant();
        // Memeriksa hasil pencocokan `upperMode` dengan pola `not (”PEMULA” or ”MAHIR”)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // TryParse.
        if (upperMode is not ("PEMULA" or "MAHIR"))
        // Membuka scope cabang if untuk kondisi `upperMode is not (”PEMULA” or ”MAHIR”)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // TryParse.
        {
            // Menjalankan menambahkan `new ErrorDetail(”config.mode”, ”INVALID_ENUM”)` ke `errors` dalam TryParse.
            errors.Add(new ErrorDetail("config.mode", "INVALID_ENUM"));
        // Menutup scope cabang if untuk kondisi `upperMode is not (”PEMULA” or ”MAHIR”)`; bagian berikut berada di luar batas blok tersebut dalam TryParse.
        }

        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `actionsPerTurn < 1` dan `actionsPerTurn > 10`; sisi kanan diperiksa
        // hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam TryParse.
        if (actionsPerTurn < 1 || actionsPerTurn > 10)
        // Membuka scope cabang if untuk kondisi `actionsPerTurn < 1 || actionsPerTurn > 10`; pernyataan/deklarasi berikut berada di dalam batas blok ini
        // dalam TryParse.
        {
            // Menjalankan menambahkan `new ErrorDetail(”config.actions_per_turn”, ”OUT_OF_RANGE”)` ke `errors` dalam TryParse.
            errors.Add(new ErrorDetail("config.actions_per_turn", "OUT_OF_RANGE"));
        // Menutup scope cabang if untuk kondisi `actionsPerTurn < 1 || actionsPerTurn > 10`; bagian berikut berada di luar batas blok tersebut dalam
        // TryParse.
        }

        // Memeriksa pemeriksaan lebih kecil antara `startingCash` dan `0`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam TryParse.
        if (startingCash < 0)
        // Membuka scope cabang if untuk kondisi `startingCash < 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryParse.
        {
            // Menjalankan menambahkan `new ErrorDetail(”config.starting_cash”, ”OUT_OF_RANGE”)` ke `errors` dalam TryParse.
            errors.Add(new ErrorDetail("config.starting_cash", "OUT_OF_RANGE"));
        // Menutup scope cabang if untuk kondisi `startingCash < 0`; bagian berikut berada di luar batas blok tersebut dalam TryParse.
        }

        // Memeriksa pemeriksaan lebih kecil antara `cashMin` dan `0`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam TryParse.
        if (cashMin < 0)
        // Membuka scope cabang if untuk kondisi `cashMin < 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryParse.
        {
            // Menjalankan menambahkan `new ErrorDetail(”config.constraints.cash_min”, ”OUT_OF_RANGE”)` ke `errors` dalam TryParse.
            errors.Add(new ErrorDetail("config.constraints.cash_min", "OUT_OF_RANGE"));
        // Menutup scope cabang if untuk kondisi `cashMin < 0`; bagian berikut berada di luar batas blok tersebut dalam TryParse.
        }

        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `maxIngredientTotal < 0` dan `maxIngredientTotal > 50`; sisi kanan
        // diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam TryParse.
        if (maxIngredientTotal < 0 || maxIngredientTotal > 50)
        // Membuka scope cabang if untuk kondisi `maxIngredientTotal < 0 || maxIngredientTotal > 50`; pernyataan/deklarasi berikut berada di dalam batas
        // blok ini dalam TryParse.
        {
            // Menjalankan menambahkan `new ErrorDetail(”config.constraints.max_ingredient_total”, ”OUT_OF_RANGE”)` ke `errors` dalam TryParse.
            errors.Add(new ErrorDetail("config.constraints.max_ingredient_total", "OUT_OF_RANGE"));
        // Menutup scope cabang if untuk kondisi `maxIngredientTotal < 0 || maxIngredientTotal > 50`; bagian berikut berada di luar batas blok tersebut
        // dalam TryParse.
        }

        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `maxSameIngredient < 0` dan `maxSameIngredient > 50`; sisi kanan
        // diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam TryParse.
        if (maxSameIngredient < 0 || maxSameIngredient > 50)
        // Membuka scope cabang if untuk kondisi `maxSameIngredient < 0 || maxSameIngredient > 50`; pernyataan/deklarasi berikut berada di dalam batas blok
        // ini dalam TryParse.
        {
            // Menjalankan menambahkan `new ErrorDetail(”config.constraints.max_same_ingredient”, ”OUT_OF_RANGE”)` ke `errors` dalam TryParse.
            errors.Add(new ErrorDetail("config.constraints.max_same_ingredient", "OUT_OF_RANGE"));
        // Menutup scope cabang if untuk kondisi `maxSameIngredient < 0 || maxSameIngredient > 50`; bagian berikut berada di luar batas blok tersebut dalam
        // TryParse.
        }

        // Memeriksa pemeriksaan lebih besar antara `maxSameIngredient` dan `maxIngredientTotal`; blok if hanya dijalankan ketika kondisi ini bernilai benar
        // dalam TryParse.
        if (maxSameIngredient > maxIngredientTotal)
        // Membuka scope cabang if untuk kondisi `maxSameIngredient > maxIngredientTotal`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // TryParse.
        {
            // Menjalankan menambahkan `new ErrorDetail(”config.constraints.max_same_ingredient”, ”INVALID_RELATION”)` ke `errors` dalam TryParse.
            errors.Add(new ErrorDetail("config.constraints.max_same_ingredient", "INVALID_RELATION"));
        // Menutup scope cabang if untuk kondisi `maxSameIngredient > maxIngredientTotal`; bagian berikut berada di luar batas blok tersebut dalam TryParse.
        }

        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `primaryNeedMaxPerDay < 0` dan `primaryNeedMaxPerDay > 10`; sisi kanan
        // diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam TryParse.
        if (primaryNeedMaxPerDay < 0 || primaryNeedMaxPerDay > 10)
        // Membuka scope cabang if untuk kondisi `primaryNeedMaxPerDay < 0 || primaryNeedMaxPerDay > 10`; pernyataan/deklarasi berikut berada di dalam batas
        // blok ini dalam TryParse.
        {
            // Menjalankan menambahkan `new ErrorDetail(”config.constraints.primary_need_max_per_day”, ”OUT_OF_RANGE”)` ke `errors` dalam TryParse.
            errors.Add(new ErrorDetail("config.constraints.primary_need_max_per_day", "OUT_OF_RANGE"));
        // Menutup scope cabang if untuk kondisi `primaryNeedMaxPerDay < 0 || primaryNeedMaxPerDay > 10`; bagian berikut berada di luar batas blok tersebut
        // dalam TryParse.
        }

        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `donationMin < 1 || donationMax < 1` dan `donationMin > donationMax`;
        // sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam TryParse.
        if (donationMin < 1 || donationMax < 1 || donationMin > donationMax)
        // Membuka scope cabang if untuk kondisi `donationMin < 1 || donationMax < 1 || donationMin > donationMax`; pernyataan/deklarasi berikut berada di
        // dalam batas blok ini dalam TryParse.
        {
            // Menjalankan menambahkan `new ErrorDetail(”config.donation.min_amount”, ”INVALID_RANGE”)` ke `errors` dalam TryParse.
            errors.Add(new ErrorDetail("config.donation.min_amount", "INVALID_RANGE"));
        // Menutup scope cabang if untuk kondisi `donationMin < 1 || donationMax < 1 || donationMin > donationMax`; bagian berikut berada di luar batas blok
        // tersebut dalam TryParse.
        }

        // Memeriksa pemeriksaan lebih kecil atau sama antara `freelanceIncome` dan `0`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // TryParse.
        if (freelanceIncome <= 0)
        // Membuka scope cabang if untuk kondisi `freelanceIncome <= 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryParse.
        {
            // Menjalankan menambahkan `new ErrorDetail(”config.freelance.income”, ”OUT_OF_RANGE”)` ke `errors` dalam TryParse.
            errors.Add(new ErrorDetail("config.freelance.income", "OUT_OF_RANGE"));
        // Menutup scope cabang if untuk kondisi `freelanceIncome <= 0`; bagian berikut berada di luar batas blok tersebut dalam TryParse.
        }

        // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `upperMode == ”PEMULA”` dan `(loanEnabled || insuranceEnabled ||
        // savingGoalEnabled)`; sisi kanan diperiksa hanya jika sisi kiri benar; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam TryParse.
        if (upperMode == "PEMULA" && (loanEnabled || insuranceEnabled || savingGoalEnabled))
        // Membuka scope cabang if untuk kondisi `upperMode == ”PEMULA” && (loanEnabled || insuranceEnabled || savingGoalEnabled)`; pernyataan/deklarasi
        // berikut berada di dalam batas blok ini dalam TryParse.
        {
            // Menjalankan menambahkan `new ErrorDetail(”config.advanced”, ”DISALLOWED_FOR_MODE”)` ke `errors` dalam TryParse.
            errors.Add(new ErrorDetail("config.advanced", "DISALLOWED_FOR_MODE"));
        // Menutup scope cabang if untuk kondisi `upperMode == ”PEMULA” && (loanEnabled || insuranceEnabled || savingGoalEnabled)`; bagian berikut berada di
        // luar batas blok tersebut dalam TryParse.
        }

        // Memeriksa pemeriksaan lebih besar antara `errors.Count` dan `0`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam TryParse.
        if (errors.Count > 0)
        // Membuka scope cabang if untuk kondisi `errors.Count > 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryParse.
        {
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryParse; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return false;
        // Menutup scope cabang if untuk kondisi `errors.Count > 0`; bagian berikut berada di luar batas blok tersebut dalam TryParse.
        }

        // Memperbarui `config` menggunakan objek baru bertipe `RulesetConfig` dengan argumen ( upperMode, actionsPerTurn, startingCash, playerOrdering,
        // cashMin, maxIngredientTotal, maxSameIngredient, primaryNeedMaxPerDay, requirePrimaryBeforeOthers, fri... dalam TryParse.
        config = new RulesetConfig(
            // Meneruskan `upperMode` (nilai upper mode) sebagai argumen ke konstruktor `RulesetConfig`.
            upperMode,
            // Meneruskan `actionsPerTurn` (nilai aksi per giliran) sebagai argumen ke konstruktor `RulesetConfig`.
            actionsPerTurn,
            // Meneruskan `startingCash` (nilai starting uang tunai) sebagai argumen ke konstruktor `RulesetConfig`.
            startingCash,
            // Meneruskan `playerOrdering` (nilai pemain ordering) sebagai argumen ke konstruktor `RulesetConfig`.
            playerOrdering,
            // Meneruskan `cashMin` (nilai uang tunai minimum) sebagai argumen ke konstruktor `RulesetConfig`.
            cashMin,
            // Meneruskan `maxIngredientTotal` (nilai maksimum bahan total) sebagai argumen ke konstruktor `RulesetConfig`.
            maxIngredientTotal,
            // Meneruskan `maxSameIngredient` (nilai maksimum same bahan) sebagai argumen ke konstruktor `RulesetConfig`.
            maxSameIngredient,
            // Meneruskan `primaryNeedMaxPerDay` (nilai primary kebutuhan maksimum per hari) sebagai argumen ke konstruktor `RulesetConfig`.
            primaryNeedMaxPerDay,
            // Meneruskan `requirePrimaryBeforeOthers` (nilai require primary before others) sebagai argumen ke konstruktor `RulesetConfig`.
            requirePrimaryBeforeOthers,
            // Meneruskan `fridayEnabled` (nilai friday enabled) sebagai argumen ke konstruktor `RulesetConfig`.
            fridayEnabled,
            // Meneruskan `saturdayEnabled` (nilai saturday enabled) sebagai argumen ke konstruktor `RulesetConfig`.
            saturdayEnabled,
            // Meneruskan `sundayEnabled` (nilai sunday enabled) sebagai argumen ke konstruktor `RulesetConfig`.
            sundayEnabled,
            // Meneruskan `donationMin` (nilai donasi minimum) sebagai argumen ke konstruktor `RulesetConfig`.
            donationMin,
            // Meneruskan `donationMax` (nilai donasi maksimum) sebagai argumen ke konstruktor `RulesetConfig`.
            donationMax,
            // Meneruskan `allowBuy` (nilai allow buy) sebagai argumen ke konstruktor `RulesetConfig`.
            allowBuy,
            // Meneruskan `allowSell` (nilai allow sell) sebagai argumen ke konstruktor `RulesetConfig`.
            allowSell,
            // Meneruskan `loanEnabled` (nilai pinjaman enabled) sebagai argumen ke konstruktor `RulesetConfig`.
            loanEnabled,
            // Meneruskan `insuranceEnabled` (nilai asuransi enabled) sebagai argumen ke konstruktor `RulesetConfig`.
            insuranceEnabled,
            // Meneruskan `savingGoalEnabled` (nilai tabungan target enabled) sebagai argumen ke konstruktor `RulesetConfig`.
            savingGoalEnabled,
            // Meneruskan `freelanceIncome` (nilai freelance pemasukan) sebagai argumen ke konstruktor `RulesetConfig`.
            freelanceIncome,
            // Meneruskan `scoring` (nilai scoring) sebagai argumen ke konstruktor `RulesetConfig`.
            scoring);

        // Mengembalikan true, yaitu kondisi aktif/terpenuhi kepada pemanggil dalam TryParse; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return true;
    // Menutup scope metode TryParse; bagian berikut berada di luar batas blok tersebut dalam TryParse.
    }

    /// <summary>
    /// Mengonversi string urutan pemain (snake_case) ke enum <see cref="PlayerOrdering"/>.
    /// </summary>
    /// <param name="rawValue">Nilai string mentah dari JSON.</param>
    /// <param name="ordering">Hasil konversi enum.</param>
    /// <returns>True jika konversi berhasil.</returns>
    // Mendefinisikan metode `TryParsePlayerOrdering` dengan hasil bertipe `bool`. Mengonversi string urutan pemain (snake_case) ke enum . Masukan:
    // Parameter `rawValue` bertipe `string` membawa nilai raw nilai; Parameter `ordering` bertipe `PlayerOrdering` membawa nilai ordering; out
    // mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    internal static bool TryParsePlayerOrdering(string rawValue, out PlayerOrdering ordering)
    // Membuka scope metode TryParsePlayerOrdering; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryParsePlayerOrdering.
    {
        // Memperbarui `ordering` menggunakan `PlayerOrdering.PlayerOrder` (nomor urut pemain untuk menentukan urutan tindakan) dalam
        // TryParsePlayerOrdering.
        ordering = PlayerOrdering.PlayerOrder;
        // Memeriksa memeriksa apakah `rawValue` null, kosong, atau hanya berisi karakter spasi; blok if hanya dijalankan ketika kondisi ini bernilai benar
        // dalam TryParsePlayerOrdering.
        if (string.IsNullOrWhiteSpace(rawValue))
        // Membuka scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(rawValue)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // TryParsePlayerOrdering.
        {
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryParsePlayerOrdering; eksekusi jalur ini selesai setelah
            // nilai hasil ditentukan.
            return false;
        // Menutup scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(rawValue)`; bagian berikut berada di luar batas blok tersebut dalam
        // TryParsePlayerOrdering.
        }

        // Memilih cabang berdasarkan menormalisasi `rawValue.Trim()` menjadi huruf besar dengan aturan kultur invariant; label case menentukan perlakuan
        // untuk setiap nilai yang dikenali dalam TryParsePlayerOrdering.
        switch (rawValue.Trim().ToUpperInvariant())
        // Membuka scope pemilihan switch atas `rawValue.Trim().ToUpperInvariant()`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // TryParsePlayerOrdering.
        {
            // Menetapkan label cabang `case ”PLAYER_ORDER”:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            case "PLAYER_ORDER":
                // Memperbarui `ordering` menggunakan `PlayerOrdering.PlayerOrder` (nomor urut pemain untuk menentukan urutan tindakan) dalam
                // TryParsePlayerOrdering.
                ordering = PlayerOrdering.PlayerOrder;
                // Mengembalikan true, yaitu kondisi aktif/terpenuhi kepada pemanggil dalam TryParsePlayerOrdering; eksekusi jalur ini selesai setelah nilai hasil
                // ditentukan.
                return true;
            // Menetapkan label cabang `case ”EVENT_SEQUENCE”:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            case "EVENT_SEQUENCE":
                // Memperbarui `ordering` menggunakan `PlayerOrdering.EventSequence` (nilai event sequence) dalam TryParsePlayerOrdering.
                ordering = PlayerOrdering.EventSequence;
                // Mengembalikan true, yaitu kondisi aktif/terpenuhi kepada pemanggil dalam TryParsePlayerOrdering; eksekusi jalur ini selesai setelah nilai hasil
                // ditentukan.
                return true;
            // Menetapkan label cabang `case ”PLAYER_ID”:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            case "PLAYER_ID":
                // Memperbarui `ordering` menggunakan `PlayerOrdering.PlayerId` (nilai pemain identitas) dalam TryParsePlayerOrdering.
                ordering = PlayerOrdering.PlayerId;
                // Mengembalikan true, yaitu kondisi aktif/terpenuhi kepada pemanggil dalam TryParsePlayerOrdering; eksekusi jalur ini selesai setelah nilai hasil
                // ditentukan.
                return true;
            // Menetapkan label cabang `case ”USERNAME”:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            case "USERNAME":
                // Memperbarui `ordering` menggunakan `PlayerOrdering.Username` (nama akun yang dipakai saat autentikasi) dalam TryParsePlayerOrdering.
                ordering = PlayerOrdering.Username;
                // Mengembalikan true, yaitu kondisi aktif/terpenuhi kepada pemanggil dalam TryParsePlayerOrdering; eksekusi jalur ini selesai setelah nilai hasil
                // ditentukan.
                return true;
            // Menetapkan label cabang `default:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            default:
                // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryParsePlayerOrdering; eksekusi jalur ini selesai setelah
                // nilai hasil ditentukan.
                return false;
        // Menutup scope pemilihan switch atas `rawValue.Trim().ToUpperInvariant()`; bagian berikut berada di luar batas blok tersebut dalam
        // TryParsePlayerOrdering.
        }
    // Menutup scope metode TryParsePlayerOrdering; bagian berikut berada di luar batas blok tersebut dalam TryParsePlayerOrdering.
    }

    /// <summary>
    /// Mengekstrak properti JSON bertipe object, menambah error jika tidak ditemukan atau bukan object.
    /// </summary>
    // Mendefinisikan metode `TryGetObject` dengan hasil bertipe `bool`. Mengekstrak properti JSON bertipe object, menambah error jika tidak ditemukan
    // atau bukan object. Masukan: Parameter `root` bertipe `JsonElement` membawa nilai root; Parameter `name` bertipe `string` membawa nilai nama;
    // Parameter `element` bertipe `JsonElement` membawa nilai element; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter
    // `errors` bertipe `List<ErrorDetail>` membawa nilai kesalahan.
    private static bool TryGetObject(JsonElement root, string name, out JsonElement element, List<ErrorDetail> errors)
    // Membuka scope metode TryGetObject; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryGetObject.
    {
        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!root.TryGetProperty(name, out element)` dan `element.ValueKind !=
        // JsonValueKind.Object`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // TryGetObject.
        if (!root.TryGetProperty(name, out element) || element.ValueKind != JsonValueKind.Object)
        // Membuka scope cabang if untuk kondisi `!root.TryGetProperty(name, out element) || element.ValueKind != JsonValueKind.Object`;
        // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryGetObject.
        {
            // Menjalankan menambahkan `new ErrorDetail($”config.{name}”, ”REQUIRED”)` ke `errors` dalam TryGetObject.
            errors.Add(new ErrorDetail($"config.{name}", "REQUIRED"));
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryGetObject; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return false;
        // Menutup scope cabang if untuk kondisi `!root.TryGetProperty(name, out element) || element.ValueKind != JsonValueKind.Object`; bagian berikut
        // berada di luar batas blok tersebut dalam TryGetObject.
        }

        // Mengembalikan true, yaitu kondisi aktif/terpenuhi kepada pemanggil dalam TryGetObject; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return true;
    // Menutup scope metode TryGetObject; bagian berikut berada di luar batas blok tersebut dalam TryGetObject.
    }

    /// <summary>
    /// Mengekstrak properti JSON bertipe string, menambah error jika tidak ditemukan atau bukan string.
    /// </summary>
    // Mendefinisikan metode `TryGetString` dengan hasil bertipe `bool`. Mengekstrak properti JSON bertipe string, menambah error jika tidak ditemukan
    // atau bukan string. Masukan: Parameter `root` bertipe `JsonElement` membawa nilai root; Parameter `name` bertipe `string` membawa nilai nama;
    // Parameter `value` bertipe `string` membawa nilai nilai; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `errors`
    // bertipe `List<ErrorDetail>` membawa nilai kesalahan.
    private static bool TryGetString(JsonElement root, string name, out string value, List<ErrorDetail> errors)
    // Membuka scope metode TryGetString; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryGetString.
    {
        // Memperbarui `value` menggunakan `string.Empty`, yaitu nilai kosong bawaan tipe terkait dalam TryGetString.
        value = string.Empty;
        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!root.TryGetProperty(name, out var element)` dan `element.ValueKind !=
        // JsonValueKind.String`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // TryGetString.
        if (!root.TryGetProperty(name, out var element) || element.ValueKind != JsonValueKind.String)
        // Membuka scope cabang if untuk kondisi `!root.TryGetProperty(name, out var element) || element.ValueKind != JsonValueKind.String`;
        // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryGetString.
        {
            // Menjalankan menambahkan `new ErrorDetail($”config.{name}”, ”REQUIRED”)` ke `errors` dalam TryGetString.
            errors.Add(new ErrorDetail($"config.{name}", "REQUIRED"));
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryGetString; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return false;
        // Menutup scope cabang if untuk kondisi `!root.TryGetProperty(name, out var element) || element.ValueKind != JsonValueKind.String`; bagian berikut
        // berada di luar batas blok tersebut dalam TryGetString.
        }

        // Memperbarui `value` menggunakan `element.GetString()` bila tidak null; jika null gunakan `string.Empty` sebagai nilai pengganti dalam
        // TryGetString.
        value = element.GetString() ?? string.Empty;
        // Mengembalikan true, yaitu kondisi aktif/terpenuhi kepada pemanggil dalam TryGetString; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return true;
    // Menutup scope metode TryGetString; bagian berikut berada di luar batas blok tersebut dalam TryGetString.
    }

    /// <summary>
    /// Mengekstrak properti JSON bertipe integer, menambah error jika tidak ditemukan atau bukan angka.
    /// </summary>
    // Mendefinisikan metode `TryGetInt` dengan hasil bertipe `bool`. Mengekstrak properti JSON bertipe integer, menambah error jika tidak ditemukan
    // atau bukan angka. Masukan: Parameter `root` bertipe `JsonElement` membawa nilai root; Parameter `name` bertipe `string` membawa nilai nama;
    // Parameter `value` bertipe `int` membawa nilai nilai; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `errors`
    // bertipe `List<ErrorDetail>` membawa nilai kesalahan.
    private static bool TryGetInt(JsonElement root, string name, out int value, List<ErrorDetail> errors)
    // Membuka scope metode TryGetInt; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryGetInt.
    {
        // Memperbarui `value` menggunakan nilai literal `0` dalam TryGetInt.
        value = 0;
        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!root.TryGetProperty(name, out var element) || element.ValueKind !=
        // JsonValueKind.Number` dan `!element.TryGetInt32(out value)`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika
        // kondisi ini bernilai benar dalam TryGetInt.
        if (!root.TryGetProperty(name, out var element) || element.ValueKind != JsonValueKind.Number || !element.TryGetInt32(out value))
        // Membuka scope cabang if untuk kondisi `!root.TryGetProperty(name, out var element) || element.ValueKind != JsonValueKind.Number ||
        // !element.TryGetInt32(out value)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryGetInt.
        {
            // Menjalankan menambahkan `new ErrorDetail($”config.{name}”, ”REQUIRED”)` ke `errors` dalam TryGetInt.
            errors.Add(new ErrorDetail($"config.{name}", "REQUIRED"));
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryGetInt; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return false;
        // Menutup scope cabang if untuk kondisi `!root.TryGetProperty(name, out var element) || element.ValueKind != JsonValueKind.Number ||
        // !element.TryGetInt32(out value)`; bagian berikut berada di luar batas blok tersebut dalam TryGetInt.
        }

        // Mengembalikan true, yaitu kondisi aktif/terpenuhi kepada pemanggil dalam TryGetInt; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return true;
    // Menutup scope metode TryGetInt; bagian berikut berada di luar batas blok tersebut dalam TryGetInt.
    }

    /// <summary>
    /// Mengekstrak properti JSON bertipe boolean, menambah error jika tidak ditemukan atau bukan boolean.
    /// </summary>
    // Mendefinisikan metode `TryGetBool` dengan hasil bertipe `bool`. Mengekstrak properti JSON bertipe boolean, menambah error jika tidak ditemukan
    // atau bukan boolean. Masukan: Parameter `root` bertipe `JsonElement` membawa nilai root; Parameter `name` bertipe `string` membawa nilai nama;
    // Parameter `value` bertipe `bool` membawa nilai nilai; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `errors`
    // bertipe `List<ErrorDetail>` membawa nilai kesalahan.
    private static bool TryGetBool(JsonElement root, string name, out bool value, List<ErrorDetail> errors)
    // Membuka scope metode TryGetBool; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryGetBool.
    {
        // Memperbarui `value` menggunakan false, yaitu kondisi nonaktif/tidak terpenuhi dalam TryGetBool.
        value = false;
        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!root.TryGetProperty(name, out var element)` dan `(element.ValueKind !=
        // JsonValueKind.True && element.ValueKind != JsonValueKind.False)`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan
        // ketika kondisi ini bernilai benar dalam TryGetBool.
        if (!root.TryGetProperty(name, out var element) || (element.ValueKind != JsonValueKind.True && element.ValueKind != JsonValueKind.False))
        // Membuka scope cabang if untuk kondisi `!root.TryGetProperty(name, out var element) || (element.ValueKind != JsonValueKind.True &&
        // element.ValueKind != JsonValueKind.False)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryGetBool.
        {
            // Menjalankan menambahkan `new ErrorDetail($”config.{name}”, ”REQUIRED”)` ke `errors` dalam TryGetBool.
            errors.Add(new ErrorDetail($"config.{name}", "REQUIRED"));
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryGetBool; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return false;
        // Menutup scope cabang if untuk kondisi `!root.TryGetProperty(name, out var element) || (element.ValueKind != JsonValueKind.True &&
        // element.ValueKind != JsonValueKind.False)`; bagian berikut berada di luar batas blok tersebut dalam TryGetBool.
        }

        // Memperbarui `value` menggunakan memanggil `element.GetBoolean` dengan tanpa argumen dalam TryGetBool.
        value = element.GetBoolean();
        // Mengembalikan true, yaitu kondisi aktif/terpenuhi kepada pemanggil dalam TryGetBool; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return true;
    // Menutup scope metode TryGetBool; bagian berikut berada di luar batas blok tersebut dalam TryGetBool.
    }
// Menutup scope tipe RulesetConfigParser; bagian berikut berada di luar batas blok tersebut.
}
