using Cashflowpoly.Ui.Infrastructure;
using Cashflowpoly.Ui.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cashflowpoly.Ui.Controllers;

[Route("language")]
public sealed class LanguageController : Controller
{
    [HttpPost("set")]
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

    [HttpPost("toggle")]
    [ValidateAntiForgeryToken]
    public IActionResult Toggle([FromForm] string? returnUrl = null)
    {
        var current = UiText.NormalizeLanguage(HttpContext.Session.GetString(AuthConstants.SessionLanguageKey));
        var next = current == AuthConstants.LanguageEn ? AuthConstants.LanguageId : AuthConstants.LanguageEn;
        HttpContext.Session.SetString(AuthConstants.SessionLanguageKey, next);

        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("Index", "Home");
    }
}
