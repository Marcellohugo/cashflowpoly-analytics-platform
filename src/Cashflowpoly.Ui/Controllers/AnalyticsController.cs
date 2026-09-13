// Fungsi file: Menangani request MVC dan penyusunan tampilan untuk AnalyticsController.
// Mengimpor namespace `Microsoft.AspNetCore.Mvc` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.AspNetCore.Mvc;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Controllers` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Controllers;

public sealed class AnalyticsController : Controller
{
    // mendaftarkan action untuk metode HTTP GET pada rute controller saat ini.
    [HttpGet]
    public IActionResult Index(string? sessionId = null)
    {
        return ResolveAnalyticsRedirectTarget(sessionId);
    }

    // mendaftarkan action untuk metode HTTP POST pada rute controller saat ini.
    [HttpPost]
    // menerapkan metadata `ActionName(”Index”)` pada deklarasi berikut agar framework/compiler dapat mengenali pengaturannya.
    [ActionName("Index")]
    public IActionResult IndexPost(string? sessionId = null)
    {
        return ResolveAnalyticsRedirectTarget(sessionId);
    }

    private IActionResult ResolveAnalyticsRedirectTarget(string? sessionId)
    {
        if (Guid.TryParse(sessionId, out var parsedSessionId))
        {
            return RedirectToAction("Details", "Sessions", new { sessionId = parsedSessionId });
        }

        return RedirectToAction("Index", "Sessions");
    }
}

