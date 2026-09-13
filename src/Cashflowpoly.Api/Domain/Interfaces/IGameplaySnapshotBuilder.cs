// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui IGameplaySnapshotBuilder.
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Domain;

public interface IGameplaySnapshotBuilder
{
    AnalyticsGameplaySnapshot Build(
        // Parameter `playerEvents` bertipe `List<EventDb>` membawa nilai pemain event.
        List<EventDb> playerEvents,
        // Parameter `playerProjections` bertipe `List<CashflowProjectionDb>` membawa nilai pemain projections.
        List<CashflowProjectionDb> playerProjections,
        // Parameter `allEvents` bertipe `List<EventDb>` membawa nilai all event.
        List<EventDb> allEvents,
        // Parameter `config` bertipe `RulesetConfig?` membawa konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan; nilai null
        // diizinkan ketika data opsional belum tersedia.
        RulesetConfig? config,
        // Parameter `happiness` bertipe `AnalyticsHappinessBreakdown` membawa nilai kebahagiaan.
        AnalyticsHappinessBreakdown happiness,
        // Parameter `finalScore` bertipe `SessionFinalScoreDb?` membawa nilai akhir skor; nilai null diizinkan ketika data opsional belum tersedia; bila
        // argumen tidak diberikan digunakan null, yaitu penanda tidak ada nilai.
        SessionFinalScoreDb? finalScore = null,
        // Parameter `pensionRank` bertipe `int?` membawa nilai pension rank; nilai null diizinkan ketika data opsional belum tersedia; bila argumen tidak
        // diberikan digunakan null, yaitu penanda tidak ada nilai.
        int? pensionRank = null,
        // Parameter `playerAlias` bertipe `string?` membawa nilai pemain alias; nilai null diizinkan ketika data opsional belum tersedia; bila argumen
        // tidak diberikan digunakan null, yaitu penanda tidak ada nilai.
        string? playerAlias = null,
        // Parameter `sessionEnded` bertipe `bool` membawa nilai sesi ended; bila argumen tidak diberikan digunakan false, yaitu kondisi nonaktif/tidak
        // terpenuhi.
        bool sessionEnded = false);
}
