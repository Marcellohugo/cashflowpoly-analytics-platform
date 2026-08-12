// Fungsi file: Menangani request MVC dan penyusunan tampilan untuk AnalyticsController.
using Microsoft.AspNetCore.Mvc;

namespace Cashflowpoly.Ui.Controllers;

public sealed class AnalyticsController : Controller
{
    [HttpGet]
    public IActionResult Index(string? sessionId = null)
    {
        return ResolveAnalyticsRedirectTarget(sessionId);
    }

    [HttpPost]
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

