using System.Net;
using Cashflowpoly.Ui.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cashflowpoly.Ui.Infrastructure;

public static class ApiAuthHelper
{
    public static IActionResult? HandleUnauthorizedApiResponse(this Controller controller, HttpResponseMessage response)
    {
        if (response.StatusCode != HttpStatusCode.Unauthorized)
        {
            return null;
        }

        controller.HttpContext.Session.Remove(AuthConstants.SessionRoleKey);
        controller.HttpContext.Session.Remove(AuthConstants.SessionUsernameKey);
        controller.HttpContext.Session.Remove(AuthConstants.SessionAccessTokenKey);
        controller.HttpContext.Session.Remove(AuthConstants.SessionTokenExpiresAtKey);

        var returnUrl = $"{controller.HttpContext.Request.Path}{controller.HttpContext.Request.QueryString}";
        return controller.Redirect($"/auth/login?returnUrl={Uri.EscapeDataString(returnUrl)}");
    }
}
