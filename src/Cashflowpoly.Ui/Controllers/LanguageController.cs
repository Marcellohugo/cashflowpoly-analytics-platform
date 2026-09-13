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
public sealed class LanguageController : Controller
{
    // mendaftarkan action untuk metode HTTP POST pada rute (”set”).
    [HttpPost("set")]
    // memvalidasi token antiforgery untuk memastikan permintaan formulir membawa token yang sesuai.
    [ValidateAntiForgeryToken]
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
