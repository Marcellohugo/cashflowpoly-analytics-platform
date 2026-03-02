// Fungsi file: Menangani pergantian bahasa antarmuka pengguna dengan menyimpan preferensi bahasa ke sesi HTTP.
using Cashflowpoly.Ui.Infrastructure;
using Cashflowpoly.Ui.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cashflowpoly.Ui.Controllers;

[Route("language")]
/// <summary>
/// Controller MVC yang menangani pergantian bahasa tampilan antarmuka pengguna
/// dengan menyimpan preferensi bahasa ke dalam sesi HTTP.
/// </summary>
public sealed class LanguageController : Controller
{
    [HttpPost("set")]
    [ValidateAntiForgeryToken]
    /// <summary>
    /// Menyimpan preferensi bahasa yang dipilih pengguna ke sesi dan mengarahkan kembali ke halaman sebelumnya.
    /// </summary>
    /// <param name="language">Kode bahasa yang dipilih pengguna (misalnya "id" atau "en").</param>
    /// <param name="returnUrl">URL tujuan setelah bahasa diubah (opsional).</param>
    /// <returns>Redirect ke returnUrl jika valid, atau ke halaman beranda.</returns>
    public IActionResult Set([FromForm] string language, [FromForm] string? returnUrl = null)
    {
        var next = UiText.NormalizeLanguage(language);
        HttpContext.Session.SetString(AuthConstants.SessionLanguageKey, next);

        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("Index", "Home");
    }
}
