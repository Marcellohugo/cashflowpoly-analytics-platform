using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Cashflowpoly.Ui.Models;
using Cashflowpoly.Ui.Infrastructure;

namespace Cashflowpoly.Ui.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Rulebook()
    {
        return View("Privacy", model: RulebookContent.Build());
    }

    public IActionResult Privacy()
    {
        return RedirectToAction(nameof(Rulebook));
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
