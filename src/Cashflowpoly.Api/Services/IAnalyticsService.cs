// Fungsi file: Mengorkestrasi alur aplikasi dan domain melalui IAnalyticsService.
// Mengimpor namespace `System.Security.Claims` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Security.Claims;
// Mengimpor namespace `Cashflowpoly.Api.Contracts` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Contracts;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Services` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Services;

// Mendefinisikan interface sebagai kontrak operasi `IAnalyticsService`.
public interface IAnalyticsService
// Membuka scope tipe IAnalyticsService; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan metode `GetSessionAnalyticsAsync` dengan hasil bertipe `Task<(AnalyticsSessionResponse? Result, int StatusCode, ErrorResponse?
    // Error)>`; operasi ini menangani get sesi analytics asinkron. Masukan: Parameter `sessionId` bertipe `Guid` membawa identitas unik sesi permainan
    // yang menjadi batas data operasi ini; Parameter `user` bertipe `ClaimsPrincipal` membawa pengguna yang sedang diproses beserta identitas atau
    // klaim akses yang dimilikinya; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil
    // membatalkan permintaan atau aplikasi berhenti.
    Task<(AnalyticsSessionResponse? Result, int StatusCode, ErrorResponse? Error)> GetSessionAnalyticsAsync(
        // Parameter `sessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini.
        Guid sessionId, ClaimsPrincipal user, CancellationToken ct);

    // Mendefinisikan metode `RecomputeAsync` dengan hasil bertipe `Task<(AnalyticsSessionResponse? Result, int StatusCode, ErrorResponse? Error)>`;
    // operasi ini menangani recompute asinkron. Masukan: Parameter `sessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas
    // data operasi ini; Parameter `user` bertipe `ClaimsPrincipal` membawa pengguna yang sedang diproses beserta identitas atau klaim akses yang
    // dimilikinya; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan
    // permintaan atau aplikasi berhenti.
    Task<(AnalyticsSessionResponse? Result, int StatusCode, ErrorResponse? Error)> RecomputeAsync(
        // Parameter `sessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini.
        Guid sessionId, ClaimsPrincipal user, CancellationToken ct);

    // Mendefinisikan metode `GetTransactionsAsync` dengan hasil bertipe `Task<(TransactionHistoryResponse? Result, int StatusCode, ErrorResponse?
    // Error)>`; operasi ini menangani get transactions asinkron. Masukan: Parameter `sessionId` bertipe `Guid` membawa identitas unik sesi permainan
    // yang menjadi batas data operasi ini; Parameter `playerId` bertipe `Guid?` membawa nilai pemain identitas; nilai null diizinkan ketika data
    // opsional belum tersedia; Parameter `cursor` bertipe `string?` membawa penanda halaman untuk melanjutkan pembacaan setelah elemen sebelumnya;
    // nilai null diizinkan ketika data opsional belum tersedia; Parameter `limit` bertipe `int` membawa batas jumlah hasil yang diminta pada satu
    // operasi; Parameter `user` bertipe `ClaimsPrincipal` membawa pengguna yang sedang diproses beserta identitas atau klaim akses yang dimilikinya;
    // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
    // aplikasi berhenti.
    Task<(TransactionHistoryResponse? Result, int StatusCode, ErrorResponse? Error)> GetTransactionsAsync(
        // Parameter `sessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini.
        Guid sessionId, Guid? playerId, string? cursor, int limit, ClaimsPrincipal user, CancellationToken ct);

    // Mendefinisikan metode `GetGameplayMetricsAsync` dengan hasil bertipe `Task<(GameplayMetricsResponse? Result, int StatusCode, ErrorResponse?
    // Error)>`; operasi ini menangani get gameplay metrics asinkron. Masukan: Parameter `sessionId` bertipe `Guid` membawa identitas unik sesi
    // permainan yang menjadi batas data operasi ini; Parameter `playerId` bertipe `Guid` membawa nilai pemain identitas; Parameter `user` bertipe
    // `ClaimsPrincipal` membawa pengguna yang sedang diproses beserta identitas atau klaim akses yang dimilikinya; Parameter `ct` bertipe
    // `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    Task<(GameplayMetricsResponse? Result, int StatusCode, ErrorResponse? Error)> GetGameplayMetricsAsync(
        // Parameter `sessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini.
        Guid sessionId, Guid playerId, ClaimsPrincipal user, CancellationToken ct);

    // Mendefinisikan metode `GetRulesetAnalyticsSummaryAsync` dengan hasil bertipe `Task<(RulesetAnalyticsSummaryResponse? Result, int StatusCode,
    // ErrorResponse? Error)>`; operasi ini menangani get aturan analytics summary asinkron. Masukan: Parameter `rulesetId` bertipe `Guid` membawa
    // identitas kumpulan aturan permainan; Parameter `user` bertipe `ClaimsPrincipal` membawa pengguna yang sedang diproses beserta identitas atau
    // klaim akses yang dimilikinya; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil
    // membatalkan permintaan atau aplikasi berhenti.
    Task<(RulesetAnalyticsSummaryResponse? Result, int StatusCode, ErrorResponse? Error)> GetRulesetAnalyticsSummaryAsync(
        // Parameter `rulesetId` bertipe `Guid` membawa identitas kumpulan aturan permainan.
        Guid rulesetId, ClaimsPrincipal user, CancellationToken ct);
// Menutup scope tipe IAnalyticsService; bagian berikut berada di luar batas blok tersebut.
}
