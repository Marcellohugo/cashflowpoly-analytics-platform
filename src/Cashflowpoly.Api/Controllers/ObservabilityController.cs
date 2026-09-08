// Fungsi file: Menangani endpoint, validasi akses, dan response HTTP untuk ObservabilityController.
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
// menetapkan pola rute (”api/v1/observability”) untuk pencocokan URL permintaan.
[Route("api/v1/observability")]
// mewajibkan otorisasi pengguna dengan ketentuan (Roles = ”INSTRUCTOR”).
[Authorize(Roles = "INSTRUCTOR")]
// menerapkan metadata `ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)` pada deklarasi berikut agar
// framework/compiler dapat mengenali pengaturannya.
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
// menerapkan metadata `ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)` pada deklarasi berikut agar framework/compiler
// dapat mengenali pengaturannya.
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
// menerapkan metadata `ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status429TooManyRequests)` pada deklarasi berikut agar
// framework/compiler dapat mengenali pengaturannya.
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status429TooManyRequests)]
// menerapkan metadata `ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)` pada deklarasi berikut agar
// framework/compiler dapat mengenali pengaturannya.
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
// Mendefinisikan tipe class `ObservabilityController` yang mewarisi atau menerapkan `ControllerBase`; sealed mencegah tipe ini diturunkan lagi.
public sealed class ObservabilityController : ControllerBase
// Membuka scope tipe ObservabilityController; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // mendaftarkan action untuk metode HTTP GET pada rute (”metrics/summary”).
    [HttpGet("metrics/summary")]
    // menerapkan metadata `ProducesResponseType(typeof(object), StatusCodes.Status200OK)` pada deklarasi berikut agar framework/compiler dapat
    // mengenali pengaturannya.
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    // Mendefinisikan metode `GetMetricsSummary` dengan hasil bertipe `IActionResult`; operasi ini menangani get metrics summary.
    public IActionResult GetMetricsSummary()
    // Membuka scope metode GetMetricsSummary; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam GetMetricsSummary.
    {
        // Mengembalikan membentuk respons HTTP 200 dengan `new { message = ”Metrics available at /metrics (Prometheus format)” }` sebagai hasil berhasil
        // kepada pemanggil dalam GetMetricsSummary; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return Ok(new { message = "Metrics available at /metrics (Prometheus format)" });
    // Menutup scope metode GetMetricsSummary; bagian berikut berada di luar batas blok tersebut dalam GetMetricsSummary.
    }
// Menutup scope tipe ObservabilityController; bagian berikut berada di luar batas blok tersebut.
}
