// Fungsi file: Menerapkan kontrol keamanan aplikasi melalui JwtOptions.
// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Security` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Security;

/// <summary>
/// Opsi konfigurasi JWT: issuer, audience, berbagai sumber signing key, dan durasi token.
/// </summary>
// Mendefinisikan tipe class `JwtOptions`; sealed mencegah tipe ini diturunkan lagi.
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
    public string ActiveKeyId { get; set; } = "default";
    /// <summary>
    /// Daftar konfigurasi signing key yang mendukung rotasi (multi-key).
    /// </summary>
    // Mendefinisikan properti `SigningKeys` bertipe `List<JwtSigningKeyOptions>` untuk nilai signing kunci; get menyediakan pembacaan nilai, set
    // mengizinkan penggantian nilai; nilai awalnya objek baru dengan tipe mengikuti konteks tujuan dan argumen ().
    public List<JwtSigningKeyOptions> SigningKeys { get; set; } = new();
    public int AccessTokenMinutes { get; set; } = 480;
}

/// <summary>
/// Konfigurasi satu signing key: key ID, nilai key, file secret, dan jendela waktu aktif.
/// </summary>
// Mendefinisikan tipe class `JwtSigningKeyOptions`; sealed mencegah tipe ini diturunkan lagi.
public sealed class JwtSigningKeyOptions
{
    public string KeyId { get; set; } = string.Empty;
    public string SigningKey { get; set; } = string.Empty;
    public string SigningKeyFile { get; set; } = string.Empty;
    public DateTimeOffset? ActivateAtUtc { get; set; }
    public DateTimeOffset? RetireAtUtc { get; set; }
}
