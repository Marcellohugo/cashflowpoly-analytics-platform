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
    {
        if (response.StatusCode != HttpStatusCode.Unauthorized)
        {
            return null;
        }

        controller.Response.Cookies.Delete(AuthConstants.AuthenticationCookieName);

        var returnUrl = $"{controller.HttpContext.Request.Path}{controller.HttpContext.Request.QueryString}";
        return controller.Redirect($"/auth/login?returnUrl={Uri.EscapeDataString(returnUrl)}");
    }
}
