// Fungsi file: Mengelola alur halaman UI untuk domain AuthController termasuk komunikasi ke API backend.
using System.Net.Http.Json;
using System.Net.Http.Headers;
using Cashflowpoly.Ui.Infrastructure;
using Cashflowpoly.Ui.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cashflowpoly.Ui.Controllers;

[Route("auth")]
/// <summary>
/// Menyatakan peran utama tipe AuthController pada modul ini.
/// </summary>
public sealed class AuthController : Controller
{
    private readonly IHttpClientFactory _clientFactory;

    /// <summary>
    /// Menjalankan fungsi AuthController sebagai bagian dari alur file ini.
    /// </summary>
    public AuthController(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    [HttpGet("login")]
    /// <summary>
    /// Menjalankan fungsi Login sebagai bagian dari alur file ini.
    /// </summary>
    public IActionResult Login([FromQuery] string? returnUrl = null)
    {
        var existingRole = HttpContext.Session.GetString(AuthConstants.SessionRoleKey);
        var existingToken = HttpContext.Session.GetString(AuthConstants.SessionAccessTokenKey);
        if (AuthConstants.IsValidRole(existingRole) &&
            !string.IsNullOrWhiteSpace(existingToken) &&
            string.IsNullOrWhiteSpace(returnUrl))
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
    /// <summary>
    /// Menjalankan fungsi Login sebagai bagian dari alur file ini.
    /// </summary>
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.Password))
        {
            model.ErrorMessage = HttpContext.T("auth.error.login_required");
            model.Password = string.Empty;
            return View(model);
        }

        var client = _clientFactory.CreateClient("Api");
        var payload = new LoginRequestDto(model.Username.Trim(), model.Password);
        var response = await client.PostAsJsonAsync("api/v1/auth/login", payload);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadFromJsonAsync<ApiErrorResponseDto>();
            model.ErrorMessage = error?.Message ?? HttpContext.T("auth.error.login_failed");
            model.Password = string.Empty;
            return View("Login", model);
        }

        var data = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
        if (data is null ||
            !AuthConstants.IsValidRole(data.Role) ||
            string.IsNullOrWhiteSpace(data.AccessToken))
        {
            model.ErrorMessage = HttpContext.T("auth.error.login_response_invalid");
            model.Password = string.Empty;
            return View("Login", model);
        }

        HttpContext.Session.SetString(AuthConstants.SessionRoleKey, data.Role.ToUpperInvariant());
        HttpContext.Session.SetString(AuthConstants.SessionUserIdKey, data.UserId.ToString());
        HttpContext.Session.SetString(AuthConstants.SessionUsernameKey, data.Username);
        var displayName = await ResolveDisplayNameAsync(data.AccessToken, data.UserId, data.Username, HttpContext.RequestAborted);
        HttpContext.Session.SetString(AuthConstants.SessionDisplayNameKey, displayName);
        HttpContext.Session.SetString(AuthConstants.SessionAccessTokenKey, data.AccessToken);
        HttpContext.Session.SetString(
            AuthConstants.SessionTokenExpiresAtKey,
            data.ExpiresAt.ToUniversalTime().ToString("O"));

        if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
        {
            return Redirect(model.ReturnUrl);
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpGet("register")]
    /// <summary>
    /// Menjalankan fungsi Register sebagai bagian dari alur file ini.
    /// </summary>
    public IActionResult Register([FromQuery] string? returnUrl = null)
    {
        return View(new RegisterViewModel
        {
            ReturnUrl = returnUrl
        });
    }

    [HttpPost("register")]
    [ValidateAntiForgeryToken]
    /// <summary>
    /// Menjalankan fungsi Register sebagai bagian dari alur file ini.
    /// </summary>
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

        if (!AuthConstants.IsValidRole(model.Role))
        {
            model.ErrorMessage = HttpContext.T("auth.error.role_invalid");
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
        var payload = new RegisterRequestDto(
            model.Username.Trim(),
            model.Password,
            model.Role.ToUpperInvariant(),
            model.DisplayName.Trim());
        var response = await client.PostAsJsonAsync("api/v1/auth/register", payload);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadFromJsonAsync<ApiErrorResponseDto>();
            model.ErrorMessage = error?.Message ?? HttpContext.T("auth.error.register_failed");
            model.Password = string.Empty;
            model.ConfirmPassword = string.Empty;
            return View(model);
        }

        var created = await response.Content.ReadFromJsonAsync<RegisterResponseDto>();
        if (created is null || string.IsNullOrWhiteSpace(created.AccessToken))
        {
            model.ErrorMessage = HttpContext.T("auth.error.register_response_invalid");
            model.Password = string.Empty;
            model.ConfirmPassword = string.Empty;
            return View(model);
        }

        HttpContext.Session.SetString(AuthConstants.SessionRoleKey, created.Role.ToUpperInvariant());
        HttpContext.Session.SetString(AuthConstants.SessionUserIdKey, created.UserId.ToString());
        HttpContext.Session.SetString(AuthConstants.SessionUsernameKey, created.Username);
        var displayName = await ResolveDisplayNameAsync(created.AccessToken, created.UserId, created.Username, HttpContext.RequestAborted);
        HttpContext.Session.SetString(AuthConstants.SessionDisplayNameKey, displayName);
        HttpContext.Session.SetString(AuthConstants.SessionAccessTokenKey, created.AccessToken);
        HttpContext.Session.SetString(
            AuthConstants.SessionTokenExpiresAtKey,
            created.ExpiresAt.ToUniversalTime().ToString("O"));

        if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
        {
            return Redirect(model.ReturnUrl);
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpPost("logout")]
    [ValidateAntiForgeryToken]
    /// <summary>
    /// Menjalankan fungsi Logout sebagai bagian dari alur file ini.
    /// </summary>
    public IActionResult Logout()
    {
        HttpContext.Session.Remove(AuthConstants.SessionUserIdKey);
        HttpContext.Session.Remove(AuthConstants.SessionRoleKey);
        HttpContext.Session.Remove(AuthConstants.SessionDisplayNameKey);
        HttpContext.Session.Remove(AuthConstants.SessionUsernameKey);
        HttpContext.Session.Remove(AuthConstants.SessionAccessTokenKey);
        HttpContext.Session.Remove(AuthConstants.SessionTokenExpiresAtKey);
        return RedirectToAction(nameof(Login));
    }

    /// <summary>
    /// Menjalankan fungsi ResolveDisplayNameAsync sebagai bagian dari alur file ini.
    /// </summary>
    private async Task<string> ResolveDisplayNameAsync(string accessToken, Guid userId, string fallback, CancellationToken ct)
    {
        var client = _clientFactory.CreateClient("Api");
        using var request = new HttpRequestMessage(HttpMethod.Get, "api/v1/players");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await client.SendAsync(request, ct);
        if (!response.IsSuccessStatusCode)
        {
            return fallback;
        }

        var players = await response.Content.ReadFromJsonAsync<PlayerListResponseDto>(cancellationToken: ct);
        var displayName = players?.Items.FirstOrDefault(item => item.PlayerId == userId)?.DisplayName;
        return string.IsNullOrWhiteSpace(displayName) ? fallback : displayName;
    }
}

