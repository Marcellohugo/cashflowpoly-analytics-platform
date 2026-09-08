// Fungsi file: Menerapkan kontrol keamanan aplikasi melalui JwtOptions.
// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Security` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Security;

/// <summary>
/// Opsi konfigurasi JWT: issuer, audience, berbagai sumber signing key, dan durasi token.
/// </summary>
// Mendefinisikan tipe class `JwtOptions`; sealed mencegah tipe ini diturunkan lagi.
public sealed class JwtOptions
// Membuka scope tipe JwtOptions; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan properti `Issuer` bertipe `string` untuk nilai issuer; get menyediakan pembacaan nilai, set mengizinkan penggantian nilai; nilai
    // awalnya nilai literal `”Cashflowpoly.Api”`.
    public string Issuer { get; set; } = "Cashflowpoly.Api";
    // Mendefinisikan properti `Audience` bertipe `string` untuk nilai audience; get menyediakan pembacaan nilai, set mengizinkan penggantian nilai;
    // nilai awalnya nilai literal `”Cashflowpoly.Client”`.
    public string Audience { get; set; } = "Cashflowpoly.Client";
    // Mendefinisikan properti `SigningKey` bertipe `string` untuk kunci untuk membentuk atau memverifikasi tanda tangan token JWT; get menyediakan
    // pembacaan nilai, set mengizinkan penggantian nilai; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string SigningKey { get; set; } = string.Empty;
    // Mendefinisikan properti `SigningKeyFile` bertipe `string` untuk nilai signing kunci file; get menyediakan pembacaan nilai, set mengizinkan
    // penggantian nilai; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string SigningKeyFile { get; set; } = string.Empty;
    // Mendefinisikan properti `SigningKeysFile` bertipe `string` untuk nilai signing kunci file; get menyediakan pembacaan nilai, set mengizinkan
    // penggantian nilai; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string SigningKeysFile { get; set; } = string.Empty;
    // Mendefinisikan properti `SigningKeysJson` bertipe `string` untuk nilai signing kunci JSON; get menyediakan pembacaan nilai, set mengizinkan
    // penggantian nilai; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string SigningKeysJson { get; set; } = string.Empty;
    // Mendefinisikan properti `SigningKeyEnvironmentVariable` bertipe `string` untuk nilai signing kunci environment variable; get menyediakan
    // pembacaan nilai, set mengizinkan penggantian nilai; nilai awalnya nilai literal `”JWT_SIGNING_KEY”`.
    public string SigningKeyEnvironmentVariable { get; set; } = "JWT_SIGNING_KEY";
    // Mendefinisikan properti `SigningKeysJsonEnvironmentVariable` bertipe `string` untuk nilai signing kunci JSON environment variable; get
    // menyediakan pembacaan nilai, set mengizinkan penggantian nilai; nilai awalnya nilai literal `”JWT_SIGNING_KEYS_JSON”`.
    public string SigningKeysJsonEnvironmentVariable { get; set; } = "JWT_SIGNING_KEYS_JSON";
    // Mendefinisikan properti `ActiveKeyId` bertipe `string` untuk nilai aktif kunci identitas; get menyediakan pembacaan nilai, set mengizinkan
    // penggantian nilai; nilai awalnya nilai literal `”default”`.
    public string ActiveKeyId { get; set; } = "default";
    /// <summary>
    /// Daftar konfigurasi signing key yang mendukung rotasi (multi-key).
    /// </summary>
    // Mendefinisikan properti `SigningKeys` bertipe `List<JwtSigningKeyOptions>` untuk nilai signing kunci; get menyediakan pembacaan nilai, set
    // mengizinkan penggantian nilai; nilai awalnya objek baru dengan tipe mengikuti konteks tujuan dan argumen ().
    public List<JwtSigningKeyOptions> SigningKeys { get; set; } = new();
    // Mendefinisikan properti `AccessTokenMinutes` bertipe `int` untuk nilai akses token minutes; get menyediakan pembacaan nilai, set mengizinkan
    // penggantian nilai; nilai awalnya nilai literal `480`.
    public int AccessTokenMinutes { get; set; } = 480;
// Menutup scope tipe JwtOptions; bagian berikut berada di luar batas blok tersebut.
}

/// <summary>
/// Konfigurasi satu signing key: key ID, nilai key, file secret, dan jendela waktu aktif.
/// </summary>
// Mendefinisikan tipe class `JwtSigningKeyOptions`; sealed mencegah tipe ini diturunkan lagi.
public sealed class JwtSigningKeyOptions
// Membuka scope tipe JwtSigningKeyOptions; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan properti `KeyId` bertipe `string` untuk nilai kunci identitas; get menyediakan pembacaan nilai, set mengizinkan penggantian nilai;
    // nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string KeyId { get; set; } = string.Empty;
    // Mendefinisikan properti `SigningKey` bertipe `string` untuk kunci untuk membentuk atau memverifikasi tanda tangan token JWT; get menyediakan
    // pembacaan nilai, set mengizinkan penggantian nilai; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string SigningKey { get; set; } = string.Empty;
    // Mendefinisikan properti `SigningKeyFile` bertipe `string` untuk nilai signing kunci file; get menyediakan pembacaan nilai, set mengizinkan
    // penggantian nilai; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string SigningKeyFile { get; set; } = string.Empty;
    // Mendefinisikan properti `ActivateAtUtc` bertipe `DateTimeOffset?` untuk nilai activate at utc; get menyediakan pembacaan nilai, set mengizinkan
    // penggantian nilai; tanda ? mengizinkan nilai null.
    public DateTimeOffset? ActivateAtUtc { get; set; }
    // Mendefinisikan properti `RetireAtUtc` bertipe `DateTimeOffset?` untuk nilai retire at utc; get menyediakan pembacaan nilai, set mengizinkan
    // penggantian nilai; tanda ? mengizinkan nilai null.
    public DateTimeOffset? RetireAtUtc { get; set; }
// Menutup scope tipe JwtSigningKeyOptions; bagian berikut berada di luar batas blok tersebut.
}
