using Microsoft.AspNetCore.Mvc;

namespace Cashflowpoly.Ui.Controllers;

public sealed class AnalyticsController : Controller
{
    [HttpGet]
    public IActionResult Index(string? sessionId = null)
    {
        return ResolveLegacyAnalyticsTarget(sessionId);
    }

    [HttpPost]
    [ActionName("Index")]
    public IActionResult IndexPost(string? sessionId = null)
    {
        return ResolveLegacyAnalyticsTarget(sessionId);
    }

    private IActionResult ResolveLegacyAnalyticsTarget(string? sessionId)
    {
        if (Guid.TryParse(sessionId, out var parsedSessionId))
        {
            return RedirectToAction("Details", "Sessions", new { sessionId = parsedSessionId });
        }

        return RedirectToAction("Index", "Sessions");
    }
}

