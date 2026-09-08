// Fungsi file: Menangani request MVC dan penyusunan tampilan untuk AnalyticsController.
// Mengimpor namespace `Microsoft.AspNetCore.Mvc` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.AspNetCore.Mvc;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Controllers` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Controllers;

// Mendefinisikan tipe class `AnalyticsController` yang mewarisi atau menerapkan `Controller`; sealed mencegah tipe ini diturunkan lagi.
public sealed class AnalyticsController : Controller
// Membuka scope tipe AnalyticsController; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // mendaftarkan action untuk metode HTTP GET pada rute controller saat ini.
    [HttpGet]
    // Mendefinisikan metode `Index` dengan hasil bertipe `IActionResult`; operasi ini menangani index. Masukan: Parameter `sessionId` bertipe `string?`
    // membawa identitas unik sesi permainan yang menjadi batas data operasi ini; nilai null diizinkan ketika data opsional belum tersedia; bila argumen
    // tidak diberikan digunakan null, yaitu penanda tidak ada nilai.
    public IActionResult Index(string? sessionId = null)
    // Membuka scope metode Index; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Index.
    {
        // Mengembalikan memanggil `ResolveAnalyticsRedirectTarget` dengan `sessionId` kepada pemanggil dalam Index; eksekusi jalur ini selesai setelah
        // nilai hasil ditentukan.
        return ResolveAnalyticsRedirectTarget(sessionId);
    // Menutup scope metode Index; bagian berikut berada di luar batas blok tersebut dalam Index.
    }

    // mendaftarkan action untuk metode HTTP POST pada rute controller saat ini.
    [HttpPost]
    // menerapkan metadata `ActionName(”Index”)` pada deklarasi berikut agar framework/compiler dapat mengenali pengaturannya.
    [ActionName("Index")]
    // Mendefinisikan metode `IndexPost` dengan hasil bertipe `IActionResult`; operasi ini menangani index post. Masukan: Parameter `sessionId` bertipe
    // `string?` membawa identitas unik sesi permainan yang menjadi batas data operasi ini; nilai null diizinkan ketika data opsional belum tersedia;
    // bila argumen tidak diberikan digunakan null, yaitu penanda tidak ada nilai.
    public IActionResult IndexPost(string? sessionId = null)
    // Membuka scope metode IndexPost; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam IndexPost.
    {
        // Mengembalikan memanggil `ResolveAnalyticsRedirectTarget` dengan `sessionId` kepada pemanggil dalam IndexPost; eksekusi jalur ini selesai setelah
        // nilai hasil ditentukan.
        return ResolveAnalyticsRedirectTarget(sessionId);
    // Menutup scope metode IndexPost; bagian berikut berada di luar batas blok tersebut dalam IndexPost.
    }

    // Mendefinisikan metode `ResolveAnalyticsRedirectTarget` dengan hasil bertipe `IActionResult`; operasi ini menangani resolve analytics redirect
    // target. Masukan: Parameter `sessionId` bertipe `string?` membawa identitas unik sesi permainan yang menjadi batas data operasi ini; nilai null
    // diizinkan ketika data opsional belum tersedia.
    private IActionResult ResolveAnalyticsRedirectTarget(string? sessionId)
    // Membuka scope metode ResolveAnalyticsRedirectTarget; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // ResolveAnalyticsRedirectTarget.
    {
        // Memeriksa mencoba mengonversi `sessionId`, `var parsedSessionId` melalui `Guid.TryParse`; keberhasilan dilaporkan sebagai boolean dan hasil
        // ditempatkan pada argumen out; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ResolveAnalyticsRedirectTarget.
        if (Guid.TryParse(sessionId, out var parsedSessionId))
        // Membuka scope cabang if untuk kondisi `Guid.TryParse(sessionId, out var parsedSessionId)`; pernyataan/deklarasi berikut berada di dalam batas
        // blok ini dalam ResolveAnalyticsRedirectTarget.
        {
            // Mengembalikan mengarahkan browser ke action `”Details”`, `”Sessions”`, `new { sessionId = parsedSessionId }` setelah pemrosesan selesai kepada
            // pemanggil dalam ResolveAnalyticsRedirectTarget; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return RedirectToAction("Details", "Sessions", new { sessionId = parsedSessionId });
        // Menutup scope cabang if untuk kondisi `Guid.TryParse(sessionId, out var parsedSessionId)`; bagian berikut berada di luar batas blok tersebut
        // dalam ResolveAnalyticsRedirectTarget.
        }

        // Mengembalikan mengarahkan browser ke action `”Index”`, `”Sessions”` setelah pemrosesan selesai kepada pemanggil dalam
        // ResolveAnalyticsRedirectTarget; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return RedirectToAction("Index", "Sessions");
    // Menutup scope metode ResolveAnalyticsRedirectTarget; bagian berikut berada di luar batas blok tersebut dalam ResolveAnalyticsRedirectTarget.
    }
// Menutup scope tipe AnalyticsController; bagian berikut berada di luar batas blok tersebut.
}

