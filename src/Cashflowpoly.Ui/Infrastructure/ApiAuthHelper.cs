// Fungsi file: Menyediakan transformasi, lokalisasi, atau koneksi UI melalui ApiAuthHelper.
// Mengimpor namespace `System.Net` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Net;
// Mengimpor namespace `Cashflowpoly.Ui.Models` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Ui.Models;
// Mengimpor namespace `Microsoft.AspNetCore.Mvc` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.AspNetCore.Mvc;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Infrastructure` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Infrastructure;

/// <summary>
/// Kelas statis berisi extension method pada Controller untuk menangani
/// respons API yang mengembalikan status 401 Unauthorized secara terpusat.
/// </summary>
// Mendefinisikan tipe class `ApiAuthHelper`.
public static class ApiAuthHelper
// Membuka scope tipe ApiAuthHelper; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    /// <summary>
    /// Memeriksa apakah respons API berstatus 401 Unauthorized. Jika ya, menghapus
    /// cookie autentikasi dan mengembalikan redirect ke halaman login
    /// dengan menyertakan URL halaman asal sebagai parameter returnUrl.
    /// </summary>
    /// <param name="controller">Instance controller MVC yang memanggil API.</param>
    /// <param name="response">Respons HTTP dari panggilan API backend.</param>
    /// <returns>RedirectResult ke halaman login jika 401, atau null jika bukan 401.</returns>
    // Mendefinisikan metode `HandleUnauthorizedApiResponse` dengan hasil bertipe `IActionResult?`. Memeriksa apakah respons API berstatus 401
    // Unauthorized. Jika ya, menghapus cookie autentikasi dan mengembalikan redirect ke halaman login dengan menyertakan URL halaman asal sebagai
    // parameter returnUrl. Masukan: Parameter `controller` bertipe `Controller` membawa nilai controller; Parameter `response` bertipe
    // `HttpResponseMessage` membawa hasil respons yang akan dibaca atau dikirim kepada pemanggil.
    public static IActionResult? HandleUnauthorizedApiResponse(this Controller controller, HttpResponseMessage response)
    // Membuka scope metode HandleUnauthorizedApiResponse; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // HandleUnauthorizedApiResponse.
    {
        // Memeriksa perbandingan ketidaksamaan antara `response.StatusCode` dan `HttpStatusCode.Unauthorized`; blok if hanya dijalankan ketika kondisi ini
        // bernilai benar dalam HandleUnauthorizedApiResponse.
        if (response.StatusCode != HttpStatusCode.Unauthorized)
        // Membuka scope cabang if untuk kondisi `response.StatusCode != HttpStatusCode.Unauthorized`; pernyataan/deklarasi berikut berada di dalam batas
        // blok ini dalam HandleUnauthorizedApiResponse.
        {
            // Mengembalikan null, yaitu penanda tidak ada nilai kepada pemanggil dalam HandleUnauthorizedApiResponse; eksekusi jalur ini selesai setelah nilai
            // hasil ditentukan.
            return null;
        // Menutup scope cabang if untuk kondisi `response.StatusCode != HttpStatusCode.Unauthorized`; bagian berikut berada di luar batas blok tersebut
        // dalam HandleUnauthorizedApiResponse.
        }

        // Menjalankan memanggil `controller.Response.Cookies.Delete` dengan `AuthConstants.AuthenticationCookieName` dalam HandleUnauthorizedApiResponse.
        controller.Response.Cookies.Delete(AuthConstants.AuthenticationCookieName);

        // Menyiapkan variabel lokal `returnUrl` untuk nilai return url dengan teks interpolasi
        // `$”{controller.HttpContext.Request.Path}{controller.HttpContext.Request.QueryString}”`; nilai ekspresi di dalam kurung kurawal disisipkan saat
        // program berjalan. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var returnUrl = $"{controller.HttpContext.Request.Path}{controller.HttpContext.Request.QueryString}";
        // Mengembalikan memanggil `controller.Redirect` dengan `$”/auth/login?returnUrl={Uri.EscapeDataString(returnUrl)}”` kepada pemanggil dalam
        // HandleUnauthorizedApiResponse; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return controller.Redirect($"/auth/login?returnUrl={Uri.EscapeDataString(returnUrl)}");
    // Menutup scope metode HandleUnauthorizedApiResponse; bagian berikut berada di luar batas blok tersebut dalam HandleUnauthorizedApiResponse.
    }
// Menutup scope tipe ApiAuthHelper; bagian berikut berada di luar batas blok tersebut.
}
