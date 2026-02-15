using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Security;
using Microsoft.Extensions.Options;
using Xunit;

namespace Cashflowpoly.Api.Tests;

public sealed class JwtTokenServiceTests
{
    [Fact]
    public void Ctor_Throws_WhenSigningKeyMissing()
    {
        var options = Options.Create(new JwtOptions
        {
            SigningKey = string.Empty
        });

        var ex = Assert.Throws<InvalidOperationException>(() => new JwtTokenService(options));
        Assert.Contains("Jwt:SigningKey", ex.Message);
    }

    [Fact]
    public void Ctor_Throws_WhenSigningKeyTooShort()
    {
        var options = Options.Create(new JwtOptions
        {
            SigningKey = "short-key"
        });

        var ex = Assert.Throws<InvalidOperationException>(() => new JwtTokenService(options));
        Assert.Contains("minimal 32 karakter", ex.Message);
    }

    [Fact]
    public void IssueToken_ContainsExpectedClaims_AndExpiry()
    {
        var options = Options.Create(new JwtOptions
        {
            Issuer = "Cashflowpoly.Test.Issuer",
            Audience = "Cashflowpoly.Test.Audience",
            SigningKey = new string('k', 32),
            AccessTokenMinutes = 15
        });
        var sut = new JwtTokenService(options);
        var user = new AuthenticatedUserDb(
            Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"),
            "alice",
            "player",
            true);

        var beforeIssue = DateTimeOffset.UtcNow;
        var issued = sut.IssueToken(user);
        var afterIssue = DateTimeOffset.UtcNow;

        var token = new JwtSecurityTokenHandler().ReadJwtToken(issued.AccessToken);
        Assert.Equal("Cashflowpoly.Test.Issuer", token.Issuer);
        Assert.Contains("Cashflowpoly.Test.Audience", token.Audiences);
        Assert.Equal(user.UserId.ToString(), token.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value);
        Assert.Equal(user.Username, token.Claims.First(c => c.Type == ClaimTypes.Name).Value);
        Assert.Equal(user.UserId.ToString(), token.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
        Assert.Equal("PLAYER", token.Claims.First(c => c.Type == ClaimTypes.Role).Value);

        var minExpected = beforeIssue.AddMinutes(15).AddSeconds(-2);
        var maxExpected = afterIssue.AddMinutes(15).AddSeconds(2);
        Assert.InRange(issued.ExpiresAt, minExpected, maxExpected);
    }
}
