// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui IGameplayCalculators.
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Domain;

// Mendefinisikan interface sebagai kontrak operasi `ICashTimelineCalculator`.
public interface ICashTimelineCalculator
// Membuka scope tipe ICashTimelineCalculator; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan metode `Compute` dengan hasil bertipe `AnalyticsCashTimeline`; operasi ini menangani compute. Masukan: Parameter `sessionEvents`
    // bertipe `IReadOnlyCollection<EventDb>` membawa nilai sesi event; Parameter `playerProjections` bertipe
    // `IReadOnlyCollection<CashflowProjectionDb>` membawa nilai pemain projections; Parameter `startingCoins` bertipe `int` membawa nilai starting
    // coins.
    AnalyticsCashTimeline Compute(
        // Parameter `sessionEvents` bertipe `IReadOnlyCollection<EventDb>` membawa nilai sesi event.
        IReadOnlyCollection<EventDb> sessionEvents,
        // Parameter `playerProjections` bertipe `IReadOnlyCollection<CashflowProjectionDb>` membawa nilai pemain projections.
        IReadOnlyCollection<CashflowProjectionDb> playerProjections,
        // Parameter `startingCoins` bertipe `int` membawa nilai starting coins.
        int startingCoins);
// Menutup scope tipe ICashTimelineCalculator; bagian berikut berada di luar batas blok tersebut.
}

// Mendefinisikan interface sebagai kontrak operasi `IDonationGameplayCalculator`.
public interface IDonationGameplayCalculator
// Membuka scope tipe IDonationGameplayCalculator; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan metode `Compute` dengan hasil bertipe `AnalyticsDonationGameplayMetrics`; operasi ini menangani compute. Masukan: Parameter
    // `playerEvents` bertipe `IEnumerable<EventDb>` membawa nilai pemain event; Parameter `allEvents` bertipe `IEnumerable<EventDb>` membawa nilai all
    // event; Parameter `coinsNetEndGame` bertipe `double` membawa nilai coins net end game.
    AnalyticsDonationGameplayMetrics Compute(
        // Parameter `playerEvents` bertipe `IEnumerable<EventDb>` membawa nilai pemain event.
        IEnumerable<EventDb> playerEvents,
        // Parameter `allEvents` bertipe `IEnumerable<EventDb>` membawa nilai all event.
        IEnumerable<EventDb> allEvents,
        // Parameter `coinsNetEndGame` bertipe `double` membawa nilai coins net end game.
        double coinsNetEndGame);
// Menutup scope tipe IDonationGameplayCalculator; bagian berikut berada di luar batas blok tersebut.
}

// Mendefinisikan interface sebagai kontrak operasi `ISavingGoalCalculator`.
public interface ISavingGoalCalculator
// Membuka scope tipe ISavingGoalCalculator; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan metode `Compute` dengan hasil bertipe `AnalyticsSavingGoalMetrics`; operasi ini menangani compute. Masukan: Parameter
    // `playerEvents` bertipe `IEnumerable<EventDb>` membawa nilai pemain event; Parameter `availableGoalCount` bertipe `int?` membawa nilai tersedia
    // target jumlah; nilai null diizinkan ketika data opsional belum tersedia; bila argumen tidak diberikan digunakan null, yaitu penanda tidak ada
    // nilai.
    AnalyticsSavingGoalMetrics Compute(IEnumerable<EventDb> playerEvents, int? availableGoalCount = null);
// Menutup scope tipe ISavingGoalCalculator; bagian berikut berada di luar batas blok tersebut.
}

// Mendefinisikan interface sebagai kontrak operasi `IIngredientMealCalculator`.
public interface IIngredientMealCalculator
// Membuka scope tipe IIngredientMealCalculator; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan metode `Compute` dengan hasil bertipe `AnalyticsIngredientMealMetrics`; operasi ini menangani compute. Masukan: Parameter
    // `playerEvents` bertipe `IReadOnlyCollection<EventDb>` membawa nilai pemain event; Parameter `playerProjections` bertipe
    // `IReadOnlyCollection<CashflowProjectionDb>` membawa nilai pemain projections.
    AnalyticsIngredientMealMetrics Compute(
        // Parameter `playerEvents` bertipe `IReadOnlyCollection<EventDb>` membawa nilai pemain event.
        IReadOnlyCollection<EventDb> playerEvents,
        // Parameter `playerProjections` bertipe `IReadOnlyCollection<CashflowProjectionDb>` membawa nilai pemain projections.
        IReadOnlyCollection<CashflowProjectionDb> playerProjections);
// Menutup scope tipe IIngredientMealCalculator; bagian berikut berada di luar batas blok tersebut.
}

// Mendefinisikan interface sebagai kontrak operasi `IGoldGameplayCalculator`.
public interface IGoldGameplayCalculator
// Membuka scope tipe IGoldGameplayCalculator; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan metode `Compute` dengan hasil bertipe `AnalyticsGoldGameplayMetrics`; operasi ini menangani compute. Masukan: Parameter
    // `playerEvents` bertipe `IEnumerable<EventDb>` membawa nilai pemain event.
    AnalyticsGoldGameplayMetrics Compute(IEnumerable<EventDb> playerEvents);
// Menutup scope tipe IGoldGameplayCalculator; bagian berikut berada di luar batas blok tersebut.
}

// Mendefinisikan interface sebagai kontrak operasi `INeedMissionCalculator`.
public interface INeedMissionCalculator
// Membuka scope tipe INeedMissionCalculator; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan metode `Compute` dengan hasil bertipe `AnalyticsNeedMissionMetrics`; operasi ini menangani compute. Masukan: Parameter
    // `playerEvents` bertipe `IEnumerable<EventDb>` membawa nilai pemain event; Parameter `playerProjections` bertipe
    // `IEnumerable<CashflowProjectionDb>` membawa nilai pemain projections.
    AnalyticsNeedMissionMetrics Compute(
        // Parameter `playerEvents` bertipe `IEnumerable<EventDb>` membawa nilai pemain event.
        IEnumerable<EventDb> playerEvents,
        // Parameter `playerProjections` bertipe `IEnumerable<CashflowProjectionDb>` membawa nilai pemain projections.
        IEnumerable<CashflowProjectionDb> playerProjections);
// Menutup scope tipe INeedMissionCalculator; bagian berikut berada di luar batas blok tersebut.
}

// Mendefinisikan interface sebagai kontrak operasi `IRiskLoanCalculator`.
public interface IRiskLoanCalculator
// Membuka scope tipe IRiskLoanCalculator; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan metode `Compute` dengan hasil bertipe `AnalyticsRiskLoanMetrics`; operasi ini menangani compute. Masukan: Parameter `playerEvents`
    // bertipe `IReadOnlyCollection<EventDb>` membawa nilai pemain event; Parameter `playerProjections` bertipe
    // `IReadOnlyCollection<CashflowProjectionDb>` membawa nilai pemain projections; Parameter `startingCoins` bertipe `int` membawa nilai starting
    // coins; Parameter `coinsNetEndGame` bertipe `double` membawa nilai coins net end game; Parameter `totalIncome` bertipe `double` membawa nilai
    // total pemasukan; Parameter `lifeRisks` bertipe `IReadOnlyCollection<Cashflowpoly.Api.Contracts.RulesetLifeRiskDto>?` membawa nilai life risks;
    // nilai null diizinkan ketika data opsional belum tersedia; bila argumen tidak diberikan digunakan null, yaitu penanda tidak ada nilai.
    AnalyticsRiskLoanMetrics Compute(
        // Parameter `playerEvents` bertipe `IReadOnlyCollection<EventDb>` membawa nilai pemain event.
        IReadOnlyCollection<EventDb> playerEvents,
        // Parameter `playerProjections` bertipe `IReadOnlyCollection<CashflowProjectionDb>` membawa nilai pemain projections.
        IReadOnlyCollection<CashflowProjectionDb> playerProjections,
        // Parameter `startingCoins` bertipe `int` membawa nilai starting coins.
        int startingCoins,
        // Parameter `coinsNetEndGame` bertipe `double` membawa nilai coins net end game.
        double coinsNetEndGame,
        // Parameter `totalIncome` bertipe `double` membawa nilai total pemasukan.
        double totalIncome,
        // Parameter `lifeRisks` bertipe `IReadOnlyCollection<Cashflowpoly.Api.Contracts.RulesetLifeRiskDto>?` membawa nilai life risks; nilai null
        // diizinkan ketika data opsional belum tersedia; bila argumen tidak diberikan digunakan null, yaitu penanda tidak ada nilai.
        IReadOnlyCollection<Cashflowpoly.Api.Contracts.RulesetLifeRiskDto>? lifeRisks = null);
// Menutup scope tipe IRiskLoanCalculator; bagian berikut berada di luar batas blok tersebut.
}

// Mendefinisikan interface sebagai kontrak operasi `IActionUsageCalculator`.
public interface IActionUsageCalculator
// Membuka scope tipe IActionUsageCalculator; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan metode `Compute` dengan hasil bertipe `AnalyticsActionUsageMetrics`; operasi ini menangani compute. Masukan: Parameter
    // `playerEvents` bertipe `IReadOnlyCollection<EventDb>` membawa nilai pemain event; Parameter `playerProjections` bertipe
    // `IReadOnlyCollection<CashflowProjectionDb>` membawa nilai pemain projections; Parameter `latestDayIndex` bertipe `int` membawa nilai latest hari
    // index; Parameter `actionsPerTurn` bertipe `int` membawa nilai aksi per giliran.
    AnalyticsActionUsageMetrics Compute(
        // Parameter `playerEvents` bertipe `IReadOnlyCollection<EventDb>` membawa nilai pemain event.
        IReadOnlyCollection<EventDb> playerEvents,
        // Parameter `playerProjections` bertipe `IReadOnlyCollection<CashflowProjectionDb>` membawa nilai pemain projections.
        IReadOnlyCollection<CashflowProjectionDb> playerProjections,
        // Parameter `latestDayIndex` bertipe `int` membawa nilai latest hari index.
        int latestDayIndex,
        // Parameter `actionsPerTurn` bertipe `int` membawa nilai aksi per giliran.
        int actionsPerTurn);
// Menutup scope tipe IActionUsageCalculator; bagian berikut berada di luar batas blok tersebut.
}

// Mendefinisikan interface sebagai kontrak operasi `IIncomeDiversificationCalculator`.
public interface IIncomeDiversificationCalculator
// Membuka scope tipe IIncomeDiversificationCalculator; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan metode `Compute` dengan hasil bertipe `AnalyticsIncomeDiversificationMetrics`; operasi ini menangani compute. Masukan: Parameter
    // `playerEvents` bertipe `IReadOnlyCollection<EventDb>` membawa nilai pemain event; Parameter `mealOrderIncomeTotal` bertipe `int` membawa nilai
    // meal urutan/pesanan pemasukan total; Parameter `goldInvestmentEarned` bertipe `int` membawa nilai emas investment earned.
    AnalyticsIncomeDiversificationMetrics Compute(
        // Parameter `playerEvents` bertipe `IReadOnlyCollection<EventDb>` membawa nilai pemain event.
        IReadOnlyCollection<EventDb> playerEvents,
        // Parameter `mealOrderIncomeTotal` bertipe `int` membawa nilai meal urutan/pesanan pemasukan total.
        int mealOrderIncomeTotal,
        // Parameter `goldInvestmentEarned` bertipe `int` membawa nilai emas investment earned.
        int goldInvestmentEarned);
// Menutup scope tipe IIncomeDiversificationCalculator; bagian berikut berada di luar batas blok tersebut.
}
