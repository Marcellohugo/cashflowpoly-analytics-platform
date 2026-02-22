// Fungsi file: Menyediakan kompatibilitas route lama Analytics dengan mengarahkannya ke halaman sesi permainan.
using Microsoft.AspNetCore.Mvc;

namespace Cashflowpoly.Ui.Controllers;

/// <summary>
/// Controller kompatibilitas untuk route Analytics lama.
/// </summary>
public sealed class AnalyticsController : Controller
{
    [HttpGet]
    /// <summary>
    /// Mengarahkan route analytics lama ke daftar sesi atau detail sesi.
    /// </summary>
    public IActionResult Index(string? sessionId = null)
    {
        return ResolveLegacyAnalyticsTarget(sessionId);
    }

    [HttpPost]
    [ActionName("Index")]
    /// <summary>
    /// Mengarahkan submit lama analytics ke daftar sesi atau detail sesi.
    /// </summary>
    public IActionResult IndexPost(string? sessionId = null)
    {
        return ResolveLegacyAnalyticsTarget(sessionId);
    }

    /// <summary>
    /// Menentukan tujuan redirect dari route analytics lama.
    /// </summary>
    private IActionResult ResolveLegacyAnalyticsTarget(string? sessionId)
    {
        if (Guid.TryParse(sessionId, out var parsedSessionId))
        {
            return RedirectToAction("Details", "Sessions", new { sessionId = parsedSessionId });
        }

        return RedirectToAction("Index", "Sessions");
    }
}

