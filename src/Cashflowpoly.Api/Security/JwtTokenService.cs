using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Cashflowpoly.Api.Data;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Cashflowpoly.Api.Security;

public sealed class JwtTokenService
{
    private readonly JwtOptions _options;
    private readonly SigningCredentials _signingCredentials;

    public JwtTokenService(IOptions<JwtOptions> options)
    {
        _options = options.Value;
        if (string.IsNullOrWhiteSpace(_options.SigningKey))
        {
            throw new InvalidOperationException("Jwt:SigningKey belum dikonfigurasi.");
        }

        if (_options.SigningKey.Length < 32)
        {
            throw new InvalidOperationException("Jwt:SigningKey minimal 32 karakter.");
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SigningKey));
        _signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    }

    public IssuedToken IssueToken(AuthenticatedUserDb user)
    {
        var now = DateTimeOffset.UtcNow;
        var expiresAt = now.AddMinutes(_options.AccessTokenMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, user.Username),
            new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Role, user.Role.ToUpperInvariant())
        };

        var descriptor = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: now.UtcDateTime,
            expires: expiresAt.UtcDateTime,
            signingCredentials: _signingCredentials);

        var token = new JwtSecurityTokenHandler().WriteToken(descriptor);
        return new IssuedToken(token, expiresAt);
    }
}

public sealed record IssuedToken(string AccessToken, DateTimeOffset ExpiresAt);
