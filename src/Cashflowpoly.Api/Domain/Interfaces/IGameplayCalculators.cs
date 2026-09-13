// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui IGameplayCalculators.
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Domain;

public interface ICashTimelineCalculator
{
    AnalyticsCashTimeline Compute(
        // Parameter `sessionEvents` bertipe `IReadOnlyCollection<EventDb>` membawa nilai sesi event.
        IReadOnlyCollection<EventDb> sessionEvents,
        // Parameter `playerProjections` bertipe `IReadOnlyCollection<CashflowProjectionDb>` membawa nilai pemain projections.
        IReadOnlyCollection<CashflowProjectionDb> playerProjections,
        // Parameter `startingCoins` bertipe `int` membawa nilai starting coins.
        int startingCoins);
}

public interface IDonationGameplayCalculator
{
    AnalyticsDonationGameplayMetrics Compute(
        // Parameter `playerEvents` bertipe `IEnumerable<EventDb>` membawa nilai pemain event.
        IEnumerable<EventDb> playerEvents,
        // Parameter `allEvents` bertipe `IEnumerable<EventDb>` membawa nilai all event.
        IEnumerable<EventDb> allEvents,
        // Parameter `coinsNetEndGame` bertipe `double` membawa nilai coins net end game.
        double coinsNetEndGame);
}

public interface ISavingGoalCalculator
{
    AnalyticsSavingGoalMetrics Compute(IEnumerable<EventDb> playerEvents, int? availableGoalCount = null);
}

public interface IIngredientMealCalculator
{
    AnalyticsIngredientMealMetrics Compute(
        // Parameter `playerEvents` bertipe `IReadOnlyCollection<EventDb>` membawa nilai pemain event.
        IReadOnlyCollection<EventDb> playerEvents,
        // Parameter `playerProjections` bertipe `IReadOnlyCollection<CashflowProjectionDb>` membawa nilai pemain projections.
        IReadOnlyCollection<CashflowProjectionDb> playerProjections);
}

public interface IGoldGameplayCalculator
{
    AnalyticsGoldGameplayMetrics Compute(IEnumerable<EventDb> playerEvents);
}

public interface INeedMissionCalculator
{
    AnalyticsNeedMissionMetrics Compute(
        // Parameter `playerEvents` bertipe `IEnumerable<EventDb>` membawa nilai pemain event.
        IEnumerable<EventDb> playerEvents,
        // Parameter `playerProjections` bertipe `IEnumerable<CashflowProjectionDb>` membawa nilai pemain projections.
        IEnumerable<CashflowProjectionDb> playerProjections);
}

public interface IRiskLoanCalculator
{
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
}

public interface IActionUsageCalculator
{
    AnalyticsActionUsageMetrics Compute(
        // Parameter `playerEvents` bertipe `IReadOnlyCollection<EventDb>` membawa nilai pemain event.
        IReadOnlyCollection<EventDb> playerEvents,
        // Parameter `playerProjections` bertipe `IReadOnlyCollection<CashflowProjectionDb>` membawa nilai pemain projections.
        IReadOnlyCollection<CashflowProjectionDb> playerProjections,
        // Parameter `latestDayIndex` bertipe `int` membawa nilai latest hari index.
        int latestDayIndex,
        // Parameter `actionsPerTurn` bertipe `int` membawa nilai aksi per giliran.
        int actionsPerTurn);
}

public interface IIncomeDiversificationCalculator
{
    AnalyticsIncomeDiversificationMetrics Compute(
        // Parameter `playerEvents` bertipe `IReadOnlyCollection<EventDb>` membawa nilai pemain event.
        IReadOnlyCollection<EventDb> playerEvents,
        // Parameter `mealOrderIncomeTotal` bertipe `int` membawa nilai meal urutan/pesanan pemasukan total.
        int mealOrderIncomeTotal,
        // Parameter `goldInvestmentEarned` bertipe `int` membawa nilai emas investment earned.
        int goldInvestmentEarned);
}
