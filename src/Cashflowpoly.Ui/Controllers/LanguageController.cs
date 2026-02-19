// Fungsi file: Mengelola alur halaman UI untuk domain LanguageController termasuk komunikasi ke API backend.
using Cashflowpoly.Ui.Infrastructure;
using Cashflowpoly.Ui.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cashflowpoly.Ui.Controllers;

[Route("language")]
/// <summary>
/// Menyatakan peran utama tipe LanguageController pada modul ini.
/// </summary>
public sealed class LanguageController : Controller
{
    [HttpPost("set")]
    [ValidateAntiForgeryToken]
    /// <summary>
    /// Menjalankan fungsi Set sebagai bagian dari alur file ini.
    /// </summary>
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
