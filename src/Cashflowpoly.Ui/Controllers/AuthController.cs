// Fungsi file: Menangani request MVC dan penyusunan tampilan untuk AuthController.
using System.Security.Claims;
using System.Net.Http.Json;
using Cashflowpoly.Ui.Contracts;
using Cashflowpoly.Ui.Infrastructure;
using Cashflowpoly.Ui.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cashflowpoly.Ui.Controllers;

[Route("auth")]
public sealed class AuthController : Controller
{
    private readonly IHttpClientFactory _clientFactory;

    public AuthController(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    [HttpGet("login")]
    public IActionResult Login([FromQuery] string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true && string.IsNullOrWhiteSpace(returnUrl))
        {
            return RedirectToAction("Index", "Home");
        }

        return View(new LoginViewModel
        {
            ReturnUrl = returnUrl
        });
    }

    [HttpPost("login")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.Password))
        {
            model.ErrorMessage = HttpContext.T("auth.error.login_required");
            model.Password = string.Empty;
            return View(model);
        }

        var client = _clientFactory.CreateClient("Api");
        var payload = new LoginRequest(model.Username.Trim(), model.Password);
        HttpResponseMessage response;
        try
        {
            response = await client.PostAsJsonAsync("api/v1/auth/login", payload);
        }
        catch (HttpRequestException)
        {
            model.ErrorMessage = HttpContext.T("auth.error.api_unavailable");
            model.Password = string.Empty;
            return View("Login", model);
        }
        catch (TaskCanceledException)
        {
            model.ErrorMessage = HttpContext.T("auth.error.api_unavailable");
            model.Password = string.Empty;
            return View("Login", model);
        }

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.TryReadFromJsonAsync<ErrorResponse>();
            model.ErrorMessage = error?.Message ?? HttpContext.T("auth.error.login_failed");
            model.Password = string.Empty;
            return View("Login", model);
        }

        var data = await response.Content.TryReadFromJsonAsync<LoginResponse>();
        if (data is null ||
            !AuthConstants.IsValidRole(data.Role) ||
            string.IsNullOrWhiteSpace(data.AccessToken))
        {
            model.ErrorMessage = HttpContext.T("auth.error.login_response_invalid");
            model.Password = string.Empty;
            return View("Login", model);
        }

        await SignInAsync(
            data.UserId,
            data.Username,
            data.DisplayName,
            data.Role,
            data.AccessToken,
            data.ExpiresAt);

        if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
        {
            return Redirect(model.ReturnUrl);
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpGet("register")]
    public IActionResult Register([FromQuery] string? returnUrl = null)
    {
        return View(new RegisterViewModel
        {
            ReturnUrl = returnUrl
        });
    }

    [HttpPost("register")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (string.IsNullOrWhiteSpace(model.DisplayName) ||
            string.IsNullOrWhiteSpace(model.Username) ||
            string.IsNullOrWhiteSpace(model.Password) ||
            string.IsNullOrWhiteSpace(model.ConfirmPassword))
        {
            model.ErrorMessage = HttpContext.T("auth.error.register_required");
            model.Password = string.Empty;
            model.ConfirmPassword = string.Empty;
            return View(model);
        }

        if (!string.Equals(model.Password, model.ConfirmPassword, StringComparison.Ordinal))
        {
            model.ErrorMessage = HttpContext.T("auth.error.confirm_mismatch");
            model.Password = string.Empty;
            model.ConfirmPassword = string.Empty;
            return View(model);
        }

        var client = _clientFactory.CreateClient("Api");
        var payload = new RegisterRequest(
            model.Username.Trim(),
            model.Password,
            AuthConstants.PlayerRole,
            model.DisplayName.Trim());
        HttpResponseMessage response;
        try
        {
            response = await client.PostAsJsonAsync("api/v1/auth/register", payload);
        }
        catch (HttpRequestException)
        {
            model.ErrorMessage = HttpContext.T("auth.error.api_unavailable");
            model.Password = string.Empty;
            model.ConfirmPassword = string.Empty;
            return View(model);
        }
        catch (TaskCanceledException)
        {
            model.ErrorMessage = HttpContext.T("auth.error.api_unavailable");
            model.Password = string.Empty;
            model.ConfirmPassword = string.Empty;
            return View(model);
        }

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.TryReadFromJsonAsync<ErrorResponse>();
            model.ErrorMessage = error?.Message ?? HttpContext.T("auth.error.register_failed");
            model.Password = string.Empty;
            model.ConfirmPassword = string.Empty;
            return View(model);
        }

        var created = await response.Content.TryReadFromJsonAsync<RegisterResponse>();
        if (created is null || string.IsNullOrWhiteSpace(created.AccessToken))
        {
            model.ErrorMessage = HttpContext.T("auth.error.register_response_invalid");
            model.Password = string.Empty;
            model.ConfirmPassword = string.Empty;
            return View(model);
        }

        await SignInAsync(
            created.UserId,
            created.Username,
            created.DisplayName,
            created.Role,
            created.AccessToken,
            created.ExpiresAt);

        if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
        {
            return Redirect(model.ReturnUrl);
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpPost("logout")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Login));
    }

    private Task SignInAsync(
        Guid userId,
        string username,
        string? displayName,
        string role,
        string accessToken,
        DateTimeOffset expiresAt)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, role.ToUpperInvariant()),
            new Claim(AuthConstants.DisplayNameClaim, string.IsNullOrWhiteSpace(displayName) ? username : displayName),
            new Claim(AuthConstants.AccessTokenClaim, accessToken)
        };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
        return HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = true,
                AllowRefresh = false,
                ExpiresUtc = expiresAt.ToUniversalTime()
            });
    }

}
