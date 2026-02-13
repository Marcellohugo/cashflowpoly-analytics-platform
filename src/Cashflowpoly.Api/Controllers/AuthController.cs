using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Models;
using Cashflowpoly.Api.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cashflowpoly.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
[Route("api/auth")]
[AllowAnonymous]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status429TooManyRequests)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
public sealed class AuthController : ControllerBase
{
    private readonly JwtTokenService _tokens;
    private readonly UserRepository _users;

    public AuthController(UserRepository users, JwtTokenService tokens)
    {
        _users = users;
        _tokens = tokens;
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
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

        if (string.Equals(user.Role, "PLAYER", StringComparison.OrdinalIgnoreCase))
        {
            await _users.EnsurePlayerLinkAsync(user.UserId, user.Username, ct);
        }

        var issued = _tokens.IssueToken(user);
        return Ok(new LoginResponse(user.UserId, user.Username, user.Role, issued.AccessToken, issued.ExpiresAt));
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(RegisterResponse), StatusCodes.Status201Created)]
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

        var displayName = string.IsNullOrWhiteSpace(request.DisplayName) ? username : request.DisplayName.Trim();
        if (displayName.Length > 80)
        {
            return BadRequest(ApiErrorHelper.BuildError(HttpContext, "VALIDATION_ERROR", "Display name maksimal 80 karakter"));
        }

        var created = await _users.CreateUserAsync(username, request.Password, normalizedRole, displayName, ct);
        if (string.Equals(created.Role, "PLAYER", StringComparison.OrdinalIgnoreCase))
        {
            await _users.EnsurePlayerLinkAsync(created.UserId, created.Username, ct);
        }
        var issued = _tokens.IssueToken(created);
        return Created(
            $"/api/v1/auth/users/{created.UserId}",
            new RegisterResponse(created.UserId, created.Username, created.Role, issued.AccessToken, issued.ExpiresAt));
    }
}
