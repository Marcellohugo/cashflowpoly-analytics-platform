// Fungsi file: Menyediakan komponen keamanan aplikasi untuk domain JwtOptions (JWT, audit, atau rate limiting).
namespace Cashflowpoly.Api.Security;

/// <summary>
/// Menyatakan peran utama tipe JwtOptions pada modul ini.
/// </summary>
public sealed class JwtOptions
{
    public string Issuer { get; set; } = "Cashflowpoly.Api";
    public string Audience { get; set; } = "Cashflowpoly.Client";
    public string SigningKey { get; set; } = string.Empty;
    public string SigningKeyFile { get; set; } = string.Empty;
    public string SigningKeysFile { get; set; } = string.Empty;
    public string SigningKeysJson { get; set; } = string.Empty;
    public string SigningKeyEnvironmentVariable { get; set; } = "JWT_SIGNING_KEY";
    public string SigningKeysJsonEnvironmentVariable { get; set; } = "JWT_SIGNING_KEYS_JSON";
    public string ActiveKeyId { get; set; } = "legacy";
    /// <summary>
    /// Menjalankan fungsi new sebagai bagian dari alur file ini.
    /// </summary>
    public List<JwtSigningKeyOptions> SigningKeys { get; set; } = new();
    public int AccessTokenMinutes { get; set; } = 480;
}

/// <summary>
/// Menyatakan peran utama tipe JwtSigningKeyOptions pada modul ini.
/// </summary>
public sealed class JwtSigningKeyOptions
{
    public string KeyId { get; set; } = string.Empty;
    public string SigningKey { get; set; } = string.Empty;
    public string SigningKeyFile { get; set; } = string.Empty;
    public DateTimeOffset? ActivateAtUtc { get; set; }
    public DateTimeOffset? RetireAtUtc { get; set; }
}
