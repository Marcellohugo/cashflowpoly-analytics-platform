// Fungsi file: Menguji perilaku dan kontrak komponen pada domain JwtSigningKeyProviderTests.
using Cashflowpoly.Api.Security;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace Cashflowpoly.Api.Tests;

/// <summary>
/// Menyatakan peran utama tipe JwtSigningKeyProviderTests pada modul ini.
/// </summary>
public sealed class JwtSigningKeyProviderTests
{
    [Fact]
    /// <summary>
    /// Menjalankan fungsi GetActiveSigningMaterial_Throws_WhenNoKeyConfigured sebagai bagian dari alur file ini.
    /// </summary>
    public void GetActiveSigningMaterial_Throws_WhenNoKeyConfigured()
    {
        var options = Options.Create(new JwtOptions
        {
            SigningKey = string.Empty,
            SigningKeys = new List<JwtSigningKeyOptions>()
        });
        var sut = new JwtSigningKeyProvider(options, NullLogger<JwtSigningKeyProvider>.Instance);

        var ex = Assert.Throws<InvalidOperationException>(() => sut.GetActiveSigningMaterial());
        Assert.Contains("signing key", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    /// <summary>
    /// Menjalankan fungsi GetActiveSigningMaterial_PicksLatestActiveKey sebagai bagian dari alur file ini.
    /// </summary>
    public void GetActiveSigningMaterial_PicksLatestActiveKey()
    {
        var now = DateTimeOffset.UtcNow;
        var options = Options.Create(new JwtOptions
        {
            SigningKeys = new List<JwtSigningKeyOptions>
            {
                new()
                {
                    KeyId = "k-2025",
                    SigningKey = new string('a', 32),
                    ActivateAtUtc = now.AddDays(-30),
                    RetireAtUtc = now.AddDays(-1)
                },
                new()
                {
                    KeyId = "k-2026",
                    SigningKey = new string('b', 32),
                    ActivateAtUtc = now.AddHours(-1)
                }
            }
        });
        var sut = new JwtSigningKeyProvider(options, NullLogger<JwtSigningKeyProvider>.Instance);

        var active = sut.GetActiveSigningMaterial();

        Assert.Equal("k-2026", active.KeyId);
        var validationKeys = sut.ResolveValidationKeys(null);
        Assert.Contains(validationKeys, key => string.Equals(key.KeyId, "k-2026", StringComparison.Ordinal));
    }
}
