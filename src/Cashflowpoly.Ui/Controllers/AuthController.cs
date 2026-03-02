// Fungsi file: Menangani permintaan HTTP untuk autentikasi pengguna, termasuk login, registrasi, dan logout melalui sesi HTTP.
using System.Net.Http.Json;
using System.Net.Http.Headers;
using Cashflowpoly.Ui.Infrastructure;
using Cashflowpoly.Ui.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cashflowpoly.Ui.Controllers;

[Route("auth")]
/// <summary>
/// Controller MVC yang mengelola alur autentikasi pengguna, termasuk login,
/// registrasi akun baru, dan logout dengan pengelolaan token sesi.
/// </summary>
public sealed class AuthController : Controller
{
    private readonly IHttpClientFactory _clientFactory;

    /// <summary>
    /// Menginisialisasi controller autentikasi dengan factory HTTP client untuk komunikasi ke API backend.
    /// </summary>
    /// <param name="clientFactory">Factory untuk membuat instance <see cref="HttpClient"/> ke API backend.</param>
    public AuthController(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    [HttpGet("login")]
    /// <summary>
    /// Menampilkan halaman formulir login, atau mengarahkan ke beranda jika pengguna sudah terautentikasi.
    /// </summary>
    /// <param name="returnUrl">URL tujuan setelah login berhasil (opsional).</param>
    /// <returns>View formulir login atau redirect ke beranda.</returns>
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
    /// Memproses pengiriman formulir login, memvalidasi kredensial melalui API, dan menyimpan token sesi jika berhasil.
    /// </summary>
    /// <param name="model">ViewModel berisi username, password, dan URL tujuan setelah login.</param>
    /// <returns>Redirect ke halaman tujuan jika berhasil, atau formulir login dengan pesan kesalahan.</returns>
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
            var error = await response.Content.TryReadFromJsonAsync<ApiErrorResponseDto>();
            model.ErrorMessage = error?.Message ?? HttpContext.T("auth.error.login_failed");
            model.Password = string.Empty;
            return View("Login", model);
        }

        var data = await response.Content.TryReadFromJsonAsync<LoginResponseDto>();
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
    /// Menampilkan halaman formulir registrasi akun baru.
    /// </summary>
    /// <param name="returnUrl">URL tujuan setelah registrasi berhasil (opsional).</param>
    /// <returns>View formulir registrasi.</returns>
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
    /// Memproses pengiriman formulir registrasi, membuat akun baru melalui API, dan langsung login jika berhasil.
    /// </summary>
    /// <param name="model">ViewModel berisi display name, username, password, konfirmasi password, dan peran.</param>
    /// <returns>Redirect ke halaman tujuan jika berhasil, atau formulir registrasi dengan pesan kesalahan.</returns>
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
            var error = await response.Content.TryReadFromJsonAsync<ApiErrorResponseDto>();
            model.ErrorMessage = error?.Message ?? HttpContext.T("auth.error.register_failed");
            model.Password = string.Empty;
            model.ConfirmPassword = string.Empty;
            return View(model);
        }

        var created = await response.Content.TryReadFromJsonAsync<RegisterResponseDto>();
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
    /// Menghapus semua data autentikasi dari sesi HTTP dan mengarahkan pengguna ke halaman login.
    /// </summary>
    /// <returns>Redirect ke halaman login.</returns>
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
    /// Mengambil nama tampilan pengguna dari API players berdasarkan token akses dan userId.
    /// </summary>
    /// <param name="accessToken">Token akses JWT untuk otorisasi API.</param>
    /// <param name="userId">Identifier unik pengguna.</param>
    /// <param name="fallback">Nama fallback jika nama tampilan tidak ditemukan.</param>
    /// <param name="ct">Token pembatalan untuk membatalkan permintaan.</param>
    /// <returns>Nama tampilan pengguna, atau fallback jika tidak ditemukan.</returns>
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

        var players = await response.Content.TryReadFromJsonAsync<PlayerListResponseDto>(cancellationToken: ct);
        var displayName = players?.Items.FirstOrDefault(item => item.PlayerId == userId)?.DisplayName;
        return string.IsNullOrWhiteSpace(displayName) ? fallback : displayName;
    }
}

