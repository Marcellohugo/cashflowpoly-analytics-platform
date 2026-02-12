using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Models;
using Cashflowpoly.Api.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Cashflowpoly.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
[Route("api/auth")]
[AllowAnonymous]
public sealed class AuthController : ControllerBase
{
    private readonly JwtTokenService _tokens;
    private readonly UserRepository _users;
    private readonly AuthOptions _authOptions;

    public AuthController(UserRepository users, JwtTokenService tokens, IOptions<AuthOptions> authOptions)
    {
        _users = users;
        _tokens = tokens;
        _authOptions = authOptions.Value;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(ApiErrorHelper.BuildError(HttpContext, "VALIDATION_ERROR", "Username dan password wajib diisi"));
        }

        var user = await _users.AuthenticateAsync(request.Username, request.Password, ct);
        if (user is null)
        {
            return Unauthorized(ApiErrorHelper.BuildError(HttpContext, "INVALID_CREDENTIALS", "Username atau password salah"));
        }

        var issued = _tokens.IssueToken(user);
        return Ok(new LoginResponse(user.UserId, user.Username, user.Role, issued.AccessToken, issued.ExpiresAt));
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(ApiErrorHelper.BuildError(HttpContext, "VALIDATION_ERROR", "Username dan password wajib diisi"));
        }

        var username = request.Username.Trim();
        if (username.Length < 3 || username.Length > 80)
        {
            return BadRequest(ApiErrorHelper.BuildError(HttpContext, "VALIDATION_ERROR", "Username harus 3-80 karakter"));
        }

        if (request.Password.Length < 6)
        {
            return BadRequest(ApiErrorHelper.BuildError(HttpContext, "VALIDATION_ERROR", "Password minimal 6 karakter"));
        }

        if (string.IsNullOrWhiteSpace(request.Role))
        {
            return BadRequest(ApiErrorHelper.BuildError(HttpContext, "VALIDATION_ERROR", "Role wajib diisi"));
        }

        var normalizedRole = request.Role.ToUpperInvariant();
        if (!string.Equals(normalizedRole, "INSTRUCTOR", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(normalizedRole, "PLAYER", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(ApiErrorHelper.BuildError(HttpContext, "VALIDATION_ERROR", "Role tidak valid"));
        }

        if (string.Equals(normalizedRole, "INSTRUCTOR", StringComparison.OrdinalIgnoreCase) &&
            !_authOptions.AllowPublicInstructorRegistration)
        {
            var hasInstructor = await _users.HasActiveInstructorAsync(ct);
            if (hasInstructor)
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    ApiErrorHelper.BuildError(
                        HttpContext,
                        "FORBIDDEN",
                        "Registrasi INSTRUCTOR publik ditutup. Gunakan akun instruktur yang sudah ada."));
            }
        }

        var exists = await _users.UsernameExistsAsync(username, ct);
        if (exists)
        {
            return Conflict(ApiErrorHelper.BuildError(HttpContext, "DUPLICATE", "Username sudah digunakan"));
        }

        var created = await _users.CreateUserAsync(username, request.Password, normalizedRole, ct);
        var issued = _tokens.IssueToken(created);
        return Created(
            $"/api/v1/auth/users/{created.UserId}",
            new RegisterResponse(created.UserId, created.Username, created.Role, issued.AccessToken, issued.ExpiresAt));
    }
}
