// Fungsi file: Menyediakan helper untuk menangani respons 401 Unauthorized dari API dengan membersihkan sesi dan mengarahkan ulang ke halaman login.
using System.Net;
using Cashflowpoly.Ui.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cashflowpoly.Ui.Infrastructure;

/// <summary>
/// Kelas statis berisi extension method pada Controller untuk menangani
/// respons API yang mengembalikan status 401 Unauthorized secara terpusat.
/// </summary>
public static class ApiAuthHelper
{
    /// <summary>
    /// Memeriksa apakah respons API berstatus 401 Unauthorized. Jika ya, menghapus
    /// seluruh data autentikasi dari sesi dan mengembalikan redirect ke halaman login
    /// dengan menyertakan URL halaman asal sebagai parameter returnUrl.
    /// </summary>
    /// <param name="controller">Instance controller MVC yang memanggil API.</param>
    /// <param name="response">Respons HTTP dari panggilan API backend.</param>
    /// <returns>RedirectResult ke halaman login jika 401, atau null jika bukan 401.</returns>
    public static IActionResult? HandleUnauthorizedApiResponse(this Controller controller, HttpResponseMessage response)
    {
        if (response.StatusCode != HttpStatusCode.Unauthorized)
        {
            return null;
        }

        controller.HttpContext.Session.Remove(AuthConstants.SessionUserIdKey);
        controller.HttpContext.Session.Remove(AuthConstants.SessionDisplayNameKey);
        controller.HttpContext.Session.Remove(AuthConstants.SessionRoleKey);
        controller.HttpContext.Session.Remove(AuthConstants.SessionUsernameKey);
        controller.HttpContext.Session.Remove(AuthConstants.SessionAccessTokenKey);
        controller.HttpContext.Session.Remove(AuthConstants.SessionTokenExpiresAtKey);

        var returnUrl = $"{controller.HttpContext.Request.Path}{controller.HttpContext.Request.QueryString}";
        return controller.Redirect($"/auth/login?returnUrl={Uri.EscapeDataString(returnUrl)}");
    }
}
