// Fungsi file: Layanan penerbitan JWT access token untuk autentikasi pengguna.
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Cashflowpoly.Api.Data;
using Microsoft.Extensions.Options;

namespace Cashflowpoly.Api.Security;

/// <summary>
/// Menerbitkan JWT access token yang ditandatangani dengan key aktif dari JwtSigningKeyProvider.
/// </summary>
public sealed class JwtTokenService
{
    private readonly IOptions<JwtOptions> _options;
    private readonly JwtSigningKeyProvider _signingKeyProvider;

    /// <summary>
    /// Menerima opsi JWT dan penyedia signing key.
    /// </summary>
    public JwtTokenService(IOptions<JwtOptions> options, JwtSigningKeyProvider signingKeyProvider)
    {
        _options = options;
        _signingKeyProvider = signingKeyProvider;
    }

    /// <summary>
    /// Membuat dan menandatangani JWT berisi claim identitas dan role pengguna.
    /// </summary>
    public IssuedToken IssueToken(AuthenticatedUserDb user)
    {
        var options = _options.Value;
        if (options.AccessTokenMinutes <= 0)
        {
            throw new InvalidOperationException("Jwt:AccessTokenMinutes harus lebih besar dari 0.");
        }

        var signing = _signingKeyProvider.GetActiveSigningMaterial();
        var now = DateTimeOffset.UtcNow;
        var expiresAt = now.AddMinutes(options.AccessTokenMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, user.Username),
            new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Role, user.Role.ToUpperInvariant())
        };

        var descriptor = new JwtSecurityToken(
            issuer: options.Issuer,
            audience: options.Audience,
            claims: claims,
            notBefore: now.UtcDateTime,
            expires: expiresAt.UtcDateTime,
            signingCredentials: signing.SigningCredentials);
        descriptor.Header["kid"] = signing.KeyId;

        var token = new JwtSecurityTokenHandler().WriteToken(descriptor);
        return new IssuedToken(token, expiresAt);
    }
}

/// <summary>
/// DTO hasil penerbitan token: string JWT dan waktu kedaluwarsa.
/// </summary>
public sealed record IssuedToken(string AccessToken, DateTimeOffset ExpiresAt);
