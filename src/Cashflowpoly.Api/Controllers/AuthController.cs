using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cashflowpoly.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly UserRepository _users;

    public AuthController(UserRepository users)
    {
        _users = users;
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

        return Ok(new LoginResponse(user.UserId, user.Username, user.Role));
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

        var exists = await _users.UsernameExistsAsync(username, ct);
        if (exists)
        {
            return Conflict(ApiErrorHelper.BuildError(HttpContext, "DUPLICATE", "Username sudah digunakan"));
        }

        var created = await _users.CreateUserAsync(username, request.Password, normalizedRole, ct);
        return Created($"/api/auth/users/{created.UserId}", new RegisterResponse(created.UserId, created.Username, created.Role));
    }
}
