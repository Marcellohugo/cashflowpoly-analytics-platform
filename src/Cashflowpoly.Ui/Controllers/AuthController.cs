// Fungsi file: Menangani request MVC dan penyusunan tampilan untuk AuthController.
// Mengimpor namespace `System.Security.Claims` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Security.Claims;
// Mengimpor namespace `System.Net.Http.Json` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Net.Http.Json;
// Mengimpor namespace `Cashflowpoly.Ui.Contracts` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Ui.Contracts;
// Mengimpor namespace `Cashflowpoly.Ui.Infrastructure` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Ui.Infrastructure;
// Mengimpor namespace `Cashflowpoly.Ui.Models` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Ui.Models;
// Mengimpor namespace `Microsoft.AspNetCore.Authentication` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.AspNetCore.Authentication;
// Mengimpor namespace `Microsoft.AspNetCore.Authentication.Cookies` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama
// lengkapnya.
using Microsoft.AspNetCore.Authentication.Cookies;
// Mengimpor namespace `Microsoft.AspNetCore.Http` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.AspNetCore.Http;
// Mengimpor namespace `Microsoft.AspNetCore.Mvc` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.AspNetCore.Mvc;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Controllers` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Controllers;

// menetapkan pola rute (”auth”) untuk pencocokan URL permintaan.
[Route("auth")]
public sealed class AuthController : Controller
{
    private readonly IHttpClientFactory _clientFactory;

    public AuthController(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    // mendaftarkan action untuk metode HTTP GET pada rute (”login”).
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

    // mendaftarkan action untuk metode HTTP POST pada rute (”login”).
    [HttpPost("login")]
    // memvalidasi token antiforgery untuk memastikan permintaan formulir membawa token yang sesuai.
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
        // Menangani exception `HttpRequestException` melalui variabel dalam Login.
        catch (HttpRequestException)
        {
            model.ErrorMessage = HttpContext.T("auth.error.api_unavailable");
            model.Password = string.Empty;
            return View("Login", model);
        }
        // Menangani exception `TaskCanceledException` melalui variabel dalam Login.
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

    // mendaftarkan action untuk metode HTTP GET pada rute (”register”).
    [HttpGet("register")]
    public IActionResult Register([FromQuery] string? returnUrl = null)
    {
        return View(new RegisterViewModel
        {
            ReturnUrl = returnUrl
        });
    }

    // mendaftarkan action untuk metode HTTP POST pada rute (”register”).
    [HttpPost("register")]
    // memvalidasi token antiforgery untuk memastikan permintaan formulir membawa token yang sesuai.
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        model.Role = model.Role?.Trim().ToUpperInvariant() ?? string.Empty;
        if (!AuthConstants.IsValidRole(model.Role))
        {
            model.ErrorMessage = HttpContext.T("auth.error.invalid_role");
            model.Password = string.Empty;
            model.ConfirmPassword = string.Empty;
            return View(model);
        }
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

        if (model.Username.Trim().Length is < 3 or > 80 || model.DisplayName.Trim().Length > 80)
        {
            model.ErrorMessage = HttpContext.T(model.Username.Trim().Length is < 3 or > 80
                ? "auth.username_hint" : "auth.display_name_hint");
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

        if (model.Password.Length < 12 || System.Text.Encoding.UTF8.GetByteCount(model.Password) > 72)
        {
            model.ErrorMessage = HttpContext.T("auth.password_hint");
            model.Password = string.Empty;
            model.ConfirmPassword = string.Empty;
            return View(model);
        }

        var client = _clientFactory.CreateClient("Api");
        var payload = new RegisterRequest(
            model.Username.Trim(),
            model.Password,
            model.Role,
            model.DisplayName.Trim());
        HttpResponseMessage response;
        try
        {
            response = await client.PostAsJsonAsync("api/v1/auth/register", payload);
        }
        // Menangani exception `HttpRequestException` melalui variabel dalam Register.
        catch (HttpRequestException)
        {
            model.ErrorMessage = HttpContext.T("auth.error.api_unavailable");
            model.Password = string.Empty;
            model.ConfirmPassword = string.Empty;
            return View(model);
        }
        // Menangani exception `TaskCanceledException` melalui variabel dalam Register.
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
        if (created is null || string.IsNullOrWhiteSpace(created.AccessToken) ||
            !string.Equals(created.Role, model.Role, StringComparison.OrdinalIgnoreCase))
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

    // mendaftarkan action untuk metode HTTP POST pada rute (”logout”).
    [HttpPost("logout")]
    // memvalidasi token antiforgery untuk memastikan permintaan formulir membawa token yang sesuai.
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Login));
    }

    private Task SignInAsync(
        // Parameter `userId` bertipe `Guid` membawa identitas akun pengguna yang datanya sedang diproses.
        Guid userId,
        // Parameter `username` bertipe `string` membawa nama akun yang dipakai saat autentikasi.
        string username,
        // Parameter `displayName` bertipe `string?` membawa nilai display nama; nilai null diizinkan ketika data opsional belum tersedia.
        string? displayName,
        // Parameter `role` bertipe `string` membawa peran pengguna yang menentukan hak akses.
        string role,
        // Parameter `accessToken` bertipe `string` membawa nilai akses token.
        string accessToken,
        // Parameter `expiresAt` bertipe `DateTimeOffset` membawa nilai expires at.
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
