// Fungsi file: Menangani endpoint, validasi akses, dan response HTTP untuk AnalyticsController.
// Mengimpor namespace `Cashflowpoly.Api.Services` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Services;
// Mengimpor namespace `Cashflowpoly.Api.Contracts` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Contracts;
// Mengimpor namespace `Microsoft.AspNetCore.Authorization` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.AspNetCore.Authorization;
// Mengimpor namespace `Microsoft.AspNetCore.Mvc` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.AspNetCore.Mvc;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Controllers` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Controllers;

// mengaktifkan perilaku API controller, termasuk inferensi binding dan respons otomatis atas model tidak valid.
[ApiController]
// menetapkan pola rute (”api/v1/analytics”) untuk pencocokan URL permintaan.
[Route("api/v1/analytics")]
// mewajibkan otorisasi pengguna sesuai kebijakan autentikasi aplikasi.
[Authorize]
// menerapkan metadata `ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)` pada deklarasi berikut agar framework/compiler
// dapat mengenali pengaturannya.
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
// menerapkan metadata `ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)` pada deklarasi berikut agar
// framework/compiler dapat mengenali pengaturannya.
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
// menerapkan metadata `ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)` pada deklarasi berikut agar framework/compiler
// dapat mengenali pengaturannya.
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
// menerapkan metadata `ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)` pada deklarasi berikut agar framework/compiler
// dapat mengenali pengaturannya.
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
// menerapkan metadata `ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)` pada deklarasi berikut agar
// framework/compiler dapat mengenali pengaturannya.
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
// menerapkan metadata `ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status429TooManyRequests)` pada deklarasi berikut agar
// framework/compiler dapat mengenali pengaturannya.
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status429TooManyRequests)]
// menerapkan metadata `ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)` pada deklarasi berikut agar
// framework/compiler dapat mengenali pengaturannya.
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
// Mendefinisikan tipe class `AnalyticsController` yang mewarisi atau menerapkan `ControllerBase`; sealed mencegah tipe ini diturunkan lagi.
public sealed class AnalyticsController : ControllerBase
// Membuka scope tipe AnalyticsController; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendeklarasikan field bertipe `IAnalyticsService`: `_analytics` menyimpan nilai analytics. readonly membatasi penggantian referensi/nilai field
    // pada deklarasi atau konstruktor.
    private readonly IAnalyticsService _analytics;

    // Mendefinisikan konstruktor AnalyticsController yang menyiapkan objek dan menerima dependency/nilai awal dari pemanggil; parameter: Parameter
    // `analytics` bertipe `IAnalyticsService` membawa nilai analytics.
    public AnalyticsController(IAnalyticsService analytics) => _analytics = analytics;

    // mendaftarkan action untuk metode HTTP POST pada rute (”sessions/{sessionId:guid}/recompute”).
    [HttpPost("sessions/{sessionId:guid}/recompute")]
    // mewajibkan otorisasi pengguna dengan ketentuan (Roles = ”INSTRUCTOR”).
    [Authorize(Roles = "INSTRUCTOR")]
    // menerapkan metadata `ProducesResponseType(typeof(AnalyticsSessionResponse), StatusCodes.Status200OK)` pada deklarasi berikut agar
    // framework/compiler dapat mengenali pengaturannya.
    [ProducesResponseType(typeof(AnalyticsSessionResponse), StatusCodes.Status200OK)]
    // Mendefinisikan metode `Recompute` dengan hasil bertipe `Task<IActionResult>`; operasi ini menangani recompute. async memungkinkan metode menunggu
    // operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `sessionId` bertipe `Guid` membawa identitas unik sesi
    // permainan yang menjadi batas data operasi ini; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan
    // ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<IActionResult> Recompute(Guid sessionId, CancellationToken ct)
    // Membuka scope metode Recompute; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Recompute.
    {
        // Memperbarui `var (result, status, error)` menggunakan hasil operasi asinkron memanggil `_analytics.RecomputeAsync` dengan `sessionId`, `User`,
        // `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam Recompute.
        var (result, status, error) = await _analytics.RecomputeAsync(sessionId, User, ct);
        // Mengembalikan hasil pemilihan bersyarat: ketika `status == 200` benar gunakan `Ok(result)`, jika tidak gunakan `StatusCode(status, error)` kepada
        // pemanggil dalam Recompute; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return status == 200 ? Ok(result) : StatusCode(status, error);
    // Menutup scope metode Recompute; bagian berikut berada di luar batas blok tersebut dalam Recompute.
    }

    // mendaftarkan action untuk metode HTTP GET pada rute (”sessions/{sessionId:guid}”).
    [HttpGet("sessions/{sessionId:guid}")]
    // menerapkan metadata `ProducesResponseType(typeof(AnalyticsSessionResponse), StatusCodes.Status200OK)` pada deklarasi berikut agar
    // framework/compiler dapat mengenali pengaturannya.
    [ProducesResponseType(typeof(AnalyticsSessionResponse), StatusCodes.Status200OK)]
    // Mendefinisikan metode `GetSessionAnalytics` dengan hasil bertipe `Task<IActionResult>`; operasi ini menangani get sesi analytics. async
    // memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `sessionId` bertipe `Guid`
    // membawa identitas unik sesi permainan yang menjadi batas data operasi ini; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan
    // agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<IActionResult> GetSessionAnalytics(Guid sessionId, CancellationToken ct)
    // Membuka scope metode GetSessionAnalytics; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam GetSessionAnalytics.
    {
        // Memperbarui `var (result, status, error)` menggunakan hasil operasi asinkron memanggil `_analytics.GetSessionAnalyticsAsync` dengan `sessionId`,
        // `User`, `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam GetSessionAnalytics.
        var (result, status, error) = await _analytics.GetSessionAnalyticsAsync(sessionId, User, ct);
        // Mengembalikan hasil pemilihan bersyarat: ketika `status == 200` benar gunakan `Ok(result)`, jika tidak gunakan `StatusCode(status, error)` kepada
        // pemanggil dalam GetSessionAnalytics; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return status == 200 ? Ok(result) : StatusCode(status, error);
    // Menutup scope metode GetSessionAnalytics; bagian berikut berada di luar batas blok tersebut dalam GetSessionAnalytics.
    }

    // mendaftarkan action untuk metode HTTP GET pada rute (”sessions/{sessionId:guid}/transactions”).
    [HttpGet("sessions/{sessionId:guid}/transactions")]
    // menerapkan metadata `ProducesResponseType(typeof(TransactionHistoryResponse), StatusCodes.Status200OK)` pada deklarasi berikut agar
    // framework/compiler dapat mengenali pengaturannya.
    [ProducesResponseType(typeof(TransactionHistoryResponse), StatusCodes.Status200OK)]
    // Mendefinisikan metode `GetTransactions` dengan hasil bertipe `Task<IActionResult>`; operasi ini menangani get transactions. async memungkinkan
    // metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `sessionId` bertipe `Guid` membawa
    // identitas unik sesi permainan yang menjadi batas data operasi ini; Parameter `userId` bertipe `Guid?` membawa identitas akun pengguna yang
    // datanya sedang diproses; nilai null diizinkan ketika data opsional belum tersedia; bila argumen tidak diberikan digunakan null, yaitu penanda
    // tidak ada nilai; mengambil nilai parameter dari query string URL; Parameter `cursor` bertipe `string?` membawa penanda halaman untuk melanjutkan
    // pembacaan setelah elemen sebelumnya; nilai null diizinkan ketika data opsional belum tersedia; bila argumen tidak diberikan digunakan null, yaitu
    // penanda tidak ada nilai; mengambil nilai parameter dari query string URL; Parameter `limit` bertipe `int` membawa batas jumlah hasil yang diminta
    // pada satu operasi; bila argumen tidak diberikan digunakan nilai literal `50`; mengambil nilai parameter dari query string URL; Parameter `ct`
    // bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi
    // berhenti; bila argumen tidak diberikan digunakan nilai literal `default`.
    public async Task<IActionResult> GetTransactions(Guid sessionId, [FromQuery] Guid? userId = null, [FromQuery] string? cursor = null, [FromQuery] int limit = 50, CancellationToken ct = default)
    // Membuka scope metode GetTransactions; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam GetTransactions.
    {
        // Memperbarui `var (result, status, error)` menggunakan hasil operasi asinkron memanggil `_analytics.GetTransactionsAsync` dengan `sessionId`,
        // `userId`, `cursor`, `limit`, `User`, `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam GetTransactions.
        var (result, status, error) = await _analytics.GetTransactionsAsync(sessionId, userId, cursor, limit, User, ct);
        // Mengembalikan hasil pemilihan bersyarat: ketika `status == 200` benar gunakan `Ok(result)`, jika tidak gunakan `StatusCode(status, error)` kepada
        // pemanggil dalam GetTransactions; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return status == 200 ? Ok(result) : StatusCode(status, error);
    // Menutup scope metode GetTransactions; bagian berikut berada di luar batas blok tersebut dalam GetTransactions.
    }

    // mendaftarkan action untuk metode HTTP GET pada rute (”sessions/{sessionId:guid}/players/{userId:guid}/gameplay”).
    [HttpGet("sessions/{sessionId:guid}/players/{userId:guid}/gameplay")]
    // menerapkan metadata `ProducesResponseType(typeof(GameplayMetricsResponse), StatusCodes.Status200OK)` pada deklarasi berikut agar
    // framework/compiler dapat mengenali pengaturannya.
    [ProducesResponseType(typeof(GameplayMetricsResponse), StatusCodes.Status200OK)]
    // Mendefinisikan metode `GetGameplayMetrics` dengan hasil bertipe `Task<IActionResult>`; operasi ini menangani get gameplay metrics. async
    // memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `sessionId` bertipe `Guid`
    // membawa identitas unik sesi permainan yang menjadi batas data operasi ini; Parameter `userId` bertipe `Guid` membawa identitas akun pengguna yang
    // datanya sedang diproses; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil
    // membatalkan permintaan atau aplikasi berhenti.
    public async Task<IActionResult> GetGameplayMetrics(Guid sessionId, Guid userId, CancellationToken ct)
    // Membuka scope metode GetGameplayMetrics; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam GetGameplayMetrics.
    {
        // Memperbarui `var (result, status, error)` menggunakan hasil operasi asinkron memanggil `_analytics.GetGameplayMetricsAsync` dengan `sessionId`,
        // `userId`, `User`, `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam GetGameplayMetrics.
        var (result, status, error) = await _analytics.GetGameplayMetricsAsync(sessionId, userId, User, ct);
        // Mengembalikan hasil pemilihan bersyarat: ketika `status == 200` benar gunakan `Ok(result)`, jika tidak gunakan `StatusCode(status, error)` kepada
        // pemanggil dalam GetGameplayMetrics; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return status == 200 ? Ok(result) : StatusCode(status, error);
    // Menutup scope metode GetGameplayMetrics; bagian berikut berada di luar batas blok tersebut dalam GetGameplayMetrics.
    }

    // mendaftarkan action untuk metode HTTP GET pada rute (”rulesets/{rulesetId:guid}/summary”).
    [HttpGet("rulesets/{rulesetId:guid}/summary")]
    // menerapkan metadata `ProducesResponseType(typeof(RulesetAnalyticsSummaryResponse), StatusCodes.Status200OK)` pada deklarasi berikut agar
    // framework/compiler dapat mengenali pengaturannya.
    [ProducesResponseType(typeof(RulesetAnalyticsSummaryResponse), StatusCodes.Status200OK)]
    // Mendefinisikan metode `GetRulesetAnalyticsSummary` dengan hasil bertipe `Task<IActionResult>`; operasi ini menangani get aturan analytics
    // summary. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `rulesetId`
    // bertipe `Guid` membawa identitas kumpulan aturan permainan; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi
    // dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<IActionResult> GetRulesetAnalyticsSummary(Guid rulesetId, CancellationToken ct)
    // Membuka scope metode GetRulesetAnalyticsSummary; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam GetRulesetAnalyticsSummary.
    {
        // Memperbarui `var (result, status, error)` menggunakan hasil operasi asinkron memanggil `_analytics.GetRulesetAnalyticsSummaryAsync` dengan
        // `rulesetId`, `User`, `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam GetRulesetAnalyticsSummary.
        var (result, status, error) = await _analytics.GetRulesetAnalyticsSummaryAsync(rulesetId, User, ct);
        // Mengembalikan hasil pemilihan bersyarat: ketika `status == 200` benar gunakan `Ok(result)`, jika tidak gunakan `StatusCode(status, error)` kepada
        // pemanggil dalam GetRulesetAnalyticsSummary; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return status == 200 ? Ok(result) : StatusCode(status, error);
    // Menutup scope metode GetRulesetAnalyticsSummary; bagian berikut berada di luar batas blok tersebut dalam GetRulesetAnalyticsSummary.
    }
// Menutup scope tipe AnalyticsController; bagian berikut berada di luar batas blok tersebut.
}
