// Fungsi file: Menguji pembuatan dan validasi JWT token termasuk klaim, expiry, dan rotasi signing key.
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Security;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace Cashflowpoly.Api.Tests;

/// <summary>
/// Kelas pengujian unit untuk memvalidasi bahwa JwtTokenService menghasilkan token
/// dengan klaim, issuer, audience, expiry, dan key ID yang benar.
/// </summary>
public sealed class JwtTokenServiceTests
{
    [Fact]
    /// <summary>
    /// Memvalidasi bahwa JwtSigningKeyProvider.ValidateConfiguration melempar InvalidOperationException
    /// ketika signing key tidak dikonfigurasi (kosong).
    /// </summary>
    public void ValidateConfiguration_Throws_WhenSigningKeyMissing()
    {
        var options = new JwtOptions
        {
            SigningKey = string.Empty
        };

        var provider = CreateSigningKeyProvider(options);
        var ex = Assert.Throws<InvalidOperationException>(() => provider.ValidateConfiguration());
        Assert.True(ex.Message.Contains("JWT signing key", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    /// <summary>
    /// Memvalidasi bahwa JwtSigningKeyProvider.ValidateConfiguration melempar InvalidOperationException
    /// ketika signing key lebih pendek dari batas minimal 32 karakter.
    /// </summary>
    public void ValidateConfiguration_Throws_WhenSigningKeyTooShort()
    {
        var options = new JwtOptions
        {
            SigningKey = "short-key"
        };

        var provider = CreateSigningKeyProvider(options);
        var ex = Assert.Throws<InvalidOperationException>(() => provider.ValidateConfiguration());
        Assert.Contains("minimal 32 karakter", ex.Message);
    }

    [Fact]
    /// <summary>
    /// Memvalidasi bahwa IssueToken menghasilkan JWT dengan klaim sub, name, role,
    /// issuer, audience, kid, dan waktu kadaluarsa yang sesuai.
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
    /// Memvalidasi bahwa IssueToken menggunakan key ID dari signing key aktif terbaru
    /// ketika beberapa signing key dikonfigurasi dengan jadwal rotasi.
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
    /// Helper untuk membuat instance JwtTokenService dengan opsi JWT yang ditentukan.
    /// </summary>
    private static JwtTokenService CreateSut(JwtOptions options)
    {
        var optionsWrapper = Options.Create(options);
        var provider = new JwtSigningKeyProvider(optionsWrapper, NullLogger<JwtSigningKeyProvider>.Instance);
        return new JwtTokenService(optionsWrapper, provider);
    }

    /// <summary>
    /// Helper untuk membuat instance JwtSigningKeyProvider dengan opsi JWT yang ditentukan.
    /// </summary>
    private static JwtSigningKeyProvider CreateSigningKeyProvider(JwtOptions options)
    {
        var optionsWrapper = Options.Create(options);
        return new JwtSigningKeyProvider(optionsWrapper, NullLogger<JwtSigningKeyProvider>.Instance);
    }
}
