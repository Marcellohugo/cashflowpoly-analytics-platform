// Fungsi file: Menguji perilaku dan kontrak komponen pada domain JwtTokenServiceTests.
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Security;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace Cashflowpoly.Api.Tests;

/// <summary>
/// Menyatakan peran utama tipe JwtTokenServiceTests pada modul ini.
/// </summary>
public sealed class JwtTokenServiceTests
{
    [Fact]
    /// <summary>
    /// Menjalankan fungsi Ctor_Throws_WhenSigningKeyMissing sebagai bagian dari alur file ini.
    /// </summary>
    public void Ctor_Throws_WhenSigningKeyMissing()
    {
        var options = new JwtOptions
        {
            SigningKey = string.Empty
        };

        var ex = Assert.Throws<InvalidOperationException>(() => CreateSut(options));
        Assert.True(ex.Message.Contains("JWT signing key", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    /// <summary>
    /// Menjalankan fungsi Ctor_Throws_WhenSigningKeyTooShort sebagai bagian dari alur file ini.
    /// </summary>
    public void Ctor_Throws_WhenSigningKeyTooShort()
    {
        var options = new JwtOptions
        {
            SigningKey = "short-key"
        };

        var ex = Assert.Throws<InvalidOperationException>(() => CreateSut(options));
        Assert.Contains("minimal 32 karakter", ex.Message);
    }

    [Fact]
    /// <summary>
    /// Menjalankan fungsi IssueToken_ContainsExpectedClaims_AndExpiry sebagai bagian dari alur file ini.
    /// </summary>
    public void IssueToken_ContainsExpectedClaims_AndExpiry()
    {
        var options = new JwtOptions
        {
            Issuer = "Cashflowpoly.Test.Issuer",
            Audience = "Cashflowpoly.Test.Audience",
            SigningKey = new string('k', 32),
            AccessTokenMinutes = 15
        };
        var sut = CreateSut(options);
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
        Assert.Equal("legacy", token.Header.Kid);

        var minExpected = beforeIssue.AddMinutes(15).AddSeconds(-2);
        var maxExpected = afterIssue.AddMinutes(15).AddSeconds(2);
        Assert.InRange(issued.ExpiresAt, minExpected, maxExpected);
    }

    [Fact]
    /// <summary>
    /// Menjalankan fungsi IssueToken_UsesActiveKeyId_WhenMultipleKeysConfigured sebagai bagian dari alur file ini.
    /// </summary>
    public void IssueToken_UsesActiveKeyId_WhenMultipleKeysConfigured()
    {
        var now = DateTimeOffset.UtcNow;
        var options = new JwtOptions
        {
            Issuer = "Cashflowpoly.Test.Issuer",
            Audience = "Cashflowpoly.Test.Audience",
            AccessTokenMinutes = 15,
            SigningKeys = new List<JwtSigningKeyOptions>
            {
                new JwtSigningKeyOptions
                {
                    KeyId = "k-2025",
                    SigningKey = new string('a', 32),
                    ActivateAtUtc = now.AddDays(-10),
                    RetireAtUtc = now.AddDays(-1)
                },
                new JwtSigningKeyOptions
                {
                    KeyId = "k-2026",
                    SigningKey = new string('b', 32),
                    ActivateAtUtc = now.AddDays(-1)
                }
            }
        };

        var sut = CreateSut(options);
        var user = new AuthenticatedUserDb(Guid.NewGuid(), "bob", "INSTRUCTOR", true);

        var issued = sut.IssueToken(user);
        var token = new JwtSecurityTokenHandler().ReadJwtToken(issued.AccessToken);

        Assert.Equal("k-2026", token.Header.Kid);
    }

    /// <summary>
    /// Menjalankan fungsi CreateSut sebagai bagian dari alur file ini.
    /// </summary>
    private static JwtTokenService CreateSut(JwtOptions options)
    {
        var optionsWrapper = Options.Create(options);
        var provider = new JwtSigningKeyProvider(optionsWrapper, NullLogger<JwtSigningKeyProvider>.Instance);
        return new JwtTokenService(optionsWrapper, provider);
    }
}
