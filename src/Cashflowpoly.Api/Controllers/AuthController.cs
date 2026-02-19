// Fungsi file: Mengelola endpoint API untuk domain AuthController termasuk validasi request dan respons standar.
using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Models;
using Cashflowpoly.Api.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cashflowpoly.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
[AllowAnonymous]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status429TooManyRequests)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
/// <summary>
/// Menyatakan peran utama tipe AuthController pada modul ini.
/// </summary>
public sealed class AuthController : ControllerBase
{
    private readonly JwtTokenService _tokens;
    private readonly UserRepository _users;
    private readonly SecurityAuditService _securityAudit;

    /// <summary>
    /// Menjalankan fungsi AuthController sebagai bagian dari alur file ini.
    /// </summary>
    public AuthController(
        UserRepository users,
        JwtTokenService tokens,
        SecurityAuditService securityAudit)
    {
        _users = users;
        _tokens = tokens;
        _securityAudit = securityAudit;
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    /// <summary>
    /// Menjalankan fungsi Login sebagai bagian dari alur file ini.
    /// </summary>
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            await _securityAudit.LogAsync(
                HttpContext,
                SecurityAuditEventTypes.LoginFailed,
                SecurityAuditOutcomes.Failure,
                StatusCodes.Status400BadRequest,
                new
                {
                    reason = "VALIDATION_ERROR",
                    issue = "username_or_password_required"
                },
                ct);
            return BadRequest(ApiErrorHelper.BuildError(HttpContext, "VALIDATION_ERROR", "Username dan password wajib diisi"));
        }

        var username = request.Username.Trim();
        var user = await _users.AuthenticateAsync(username, request.Password, ct);
        if (user is null)
        {
            await _securityAudit.LogAsync(
                HttpContext,
                SecurityAuditEventTypes.LoginFailed,
                SecurityAuditOutcomes.Failure,
                StatusCodes.Status401Unauthorized,
                new
                {
                    reason = "INVALID_CREDENTIALS",
                    username
                },
                ct);
            return Unauthorized(ApiErrorHelper.BuildError(HttpContext, "INVALID_CREDENTIALS", "Username atau password salah"));
        }

        if (string.Equals(user.Role, "PLAYER", StringComparison.OrdinalIgnoreCase))
        {
            await _users.EnsurePlayerLinkAsync(user.UserId, user.Username, ct);
        }

        var issued = _tokens.IssueToken(user);
        await _securityAudit.LogAsync(
            HttpContext,
            SecurityAuditEventTypes.LoginSuccess,
            SecurityAuditOutcomes.Success,
            StatusCodes.Status200OK,
            new
            {
                user_id = user.UserId,
                username = user.Username,
                role = user.Role
            },
            ct);
        return Ok(new LoginResponse(user.UserId, user.Username, user.Role, issued.AccessToken, issued.ExpiresAt));
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(RegisterResponse), StatusCodes.Status201Created)]
    /// <summary>
    /// Menjalankan fungsi Register sebagai bagian dari alur file ini.
    /// </summary>
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            await _securityAudit.LogAsync(
                HttpContext,
                SecurityAuditEventTypes.RegisterDenied,
                SecurityAuditOutcomes.Denied,
                StatusCodes.Status400BadRequest,
                new
                {
                    reason = "VALIDATION_ERROR",
                    issue = "username_or_password_required"
                },
                ct);
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
            await _securityAudit.LogAsync(
                HttpContext,
                SecurityAuditEventTypes.RegisterDenied,
                SecurityAuditOutcomes.Denied,
                StatusCodes.Status409Conflict,
                new
                {
                    reason = "DUPLICATE_USERNAME",
                    username
                },
                ct);
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
        await _securityAudit.LogAsync(
            HttpContext,
            SecurityAuditEventTypes.RegisterSuccess,
            SecurityAuditOutcomes.Success,
            StatusCodes.Status201Created,
            new
            {
                user_id = created.UserId,
                username = created.Username,
                role = created.Role
            },
            ct);
        return StatusCode(
            StatusCodes.Status201Created,
            new RegisterResponse(created.UserId, created.Username, created.Role, issued.AccessToken, issued.ExpiresAt));
    }
}
