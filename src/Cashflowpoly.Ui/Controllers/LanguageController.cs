// Fungsi file: Menangani request MVC dan penyusunan tampilan untuk LanguageController.
// Mengimpor namespace `Cashflowpoly.Ui.Infrastructure` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Ui.Infrastructure;
// Mengimpor namespace `Cashflowpoly.Ui.Models` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Ui.Models;
// Mengimpor namespace `Microsoft.AspNetCore.Http` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.AspNetCore.Http;
// Mengimpor namespace `Microsoft.AspNetCore.Mvc` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.AspNetCore.Mvc;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Controllers` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Controllers;

// menetapkan pola rute (”language”) untuk pencocokan URL permintaan.
[Route("language")]
// Mendefinisikan tipe class `LanguageController` yang mewarisi atau menerapkan `Controller`; sealed mencegah tipe ini diturunkan lagi.
public sealed class LanguageController : Controller
// Membuka scope tipe LanguageController; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // mendaftarkan action untuk metode HTTP POST pada rute (”set”).
    [HttpPost("set")]
    // memvalidasi token antiforgery untuk memastikan permintaan formulir membawa token yang sesuai.
    [ValidateAntiForgeryToken]
    // Mendefinisikan metode `Set` dengan hasil bertipe `IActionResult`; operasi ini menangani set. Masukan: Parameter `language` bertipe `string`
    // membawa nilai language; menerapkan metadata `FromForm` pada deklarasi berikut agar framework/compiler dapat mengenali pengaturannya; Parameter
    // `returnUrl` bertipe `string?` membawa nilai return url; nilai null diizinkan ketika data opsional belum tersedia; bila argumen tidak diberikan
    // digunakan null, yaitu penanda tidak ada nilai; menerapkan metadata `FromForm` pada deklarasi berikut agar framework/compiler dapat mengenali
    // pengaturannya.
    public IActionResult Set([FromForm] string language, [FromForm] string? returnUrl = null)
    // Membuka scope metode Set; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Set.
    {
        // Menyiapkan variabel lokal `next` untuk nilai next dengan memanggil `UiText.NormalizeLanguage` dengan `language`. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var next = UiText.NormalizeLanguage(language);
        // Menjalankan memanggil `HttpContext.Session.SetString` dengan `AuthConstants.SessionLanguageKey`, `next` dalam Set.
        HttpContext.Session.SetString(AuthConstants.SessionLanguageKey, next);

        // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `!string.IsNullOrWhiteSpace(returnUrl)` dan `Url.IsLocalUrl(returnUrl)`; sisi
        // kanan diperiksa hanya jika sisi kiri benar; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam Set.
        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        // Membuka scope cabang if untuk kondisi `!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl)`; pernyataan/deklarasi berikut berada
        // di dalam batas blok ini dalam Set.
        {
            // Mengembalikan memanggil `Redirect` dengan `returnUrl` kepada pemanggil dalam Set; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return Redirect(returnUrl);
        // Menutup scope cabang if untuk kondisi `!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl)`; bagian berikut berada di luar batas
        // blok tersebut dalam Set.
        }

        // Mengembalikan mengarahkan browser ke action `”Index”`, `”Home”` setelah pemrosesan selesai kepada pemanggil dalam Set; eksekusi jalur ini selesai
        // setelah nilai hasil ditentukan.
        return RedirectToAction("Index", "Home");
    // Menutup scope metode Set; bagian berikut berada di luar batas blok tersebut dalam Set.
    }
// Menutup scope tipe LanguageController; bagian berikut berada di luar batas blok tersebut.
}
