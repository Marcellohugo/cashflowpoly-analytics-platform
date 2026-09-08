// Fungsi file: Menerapkan kontrol keamanan aplikasi melalui JwtTokenService.
// Mengimpor namespace `System.IdentityModel.Tokens.Jwt` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.IdentityModel.Tokens.Jwt;
// Mengimpor namespace `System.Security.Claims` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Security.Claims;
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;
// Mengimpor namespace `Microsoft.Extensions.Options` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.Extensions.Options;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Security` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Security;

/// <summary>
/// Menerbitkan JWT access token yang ditandatangani dengan key aktif dari JwtSigningKeyProvider.
/// </summary>
// Mendefinisikan tipe class `JwtTokenService`; sealed mencegah tipe ini diturunkan lagi.
public sealed class JwtTokenService
// Membuka scope tipe JwtTokenService; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendeklarasikan field bertipe `string`: `DemoAccountClaim` menyimpan nilai demo account claim dengan nilai awal nilai literal `”is_demo”`.
    public const string DemoAccountClaim = "is_demo";
    // Mendeklarasikan field bertipe `IOptions<JwtOptions>`: `_options` menyimpan kumpulan pengaturan yang mengendalikan perilaku komponen. readonly
    // membatasi penggantian referensi/nilai field pada deklarasi atau konstruktor.
    private readonly IOptions<JwtOptions> _options;
    // Mendeklarasikan field bertipe `JwtSigningKeyProvider`: `_signingKeyProvider` menyimpan nilai signing kunci provider. readonly membatasi
    // penggantian referensi/nilai field pada deklarasi atau konstruktor.
    private readonly JwtSigningKeyProvider _signingKeyProvider;

    /// <summary>
    /// Menerima opsi JWT dan penyedia signing key.
    /// </summary>
    // Mendefinisikan konstruktor JwtTokenService yang menyiapkan objek dan menerima dependency/nilai awal dari pemanggil; parameter: Parameter
    // `options` bertipe `IOptions<JwtOptions>` membawa kumpulan pengaturan yang mengendalikan perilaku komponen; Parameter `signingKeyProvider` bertipe
    // `JwtSigningKeyProvider` membawa nilai signing kunci provider.
    public JwtTokenService(IOptions<JwtOptions> options, JwtSigningKeyProvider signingKeyProvider)
    // Membuka scope konstruktor JwtTokenService; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam JwtTokenService.
    {
        // Memperbarui `_options` menggunakan `options` (kumpulan pengaturan yang mengendalikan perilaku komponen) dalam JwtTokenService.
        _options = options;
        // Memperbarui `_signingKeyProvider` menggunakan `signingKeyProvider` (nilai signing kunci provider) dalam JwtTokenService.
        _signingKeyProvider = signingKeyProvider;
    // Menutup scope konstruktor JwtTokenService; bagian berikut berada di luar batas blok tersebut dalam JwtTokenService.
    }

    /// <summary>
    /// Membuat dan menandatangani JWT berisi claim identitas dan role pengguna.
    /// </summary>
    // Mendefinisikan metode `IssueToken` dengan hasil bertipe `IssuedToken`. Membuat dan menandatangani JWT berisi claim identitas dan role pengguna.
    // Masukan: Parameter `user` bertipe `AuthenticatedUserDb` membawa pengguna yang sedang diproses beserta identitas atau klaim akses yang
    // dimilikinya.
    public IssuedToken IssueToken(AuthenticatedUserDb user)
    // Membuka scope metode IssueToken; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam IssueToken.
    {
        // Menyiapkan variabel lokal `options` untuk kumpulan pengaturan yang mengendalikan perilaku komponen dengan `_options.Value`, yaitu nilai yang
        // dibungkus objek/nullable. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var options = _options.Value;
        // Memeriksa pemeriksaan lebih kecil atau sama antara `options.AccessTokenMinutes` dan `0`; blok if hanya dijalankan ketika kondisi ini bernilai
        // benar dalam IssueToken.
        if (options.AccessTokenMinutes <= 0)
        // Membuka scope cabang if untuk kondisi `options.AccessTokenMinutes <= 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // IssueToken.
        {
            // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen (”Jwt:AccessTokenMinutes harus lebih besar dari
            // 0.”) dalam IssueToken; pemanggil atau middleware penanganan error menerima kegagalan ini.
            throw new InvalidOperationException("Jwt:AccessTokenMinutes harus lebih besar dari 0.");
        // Menutup scope cabang if untuk kondisi `options.AccessTokenMinutes <= 0`; bagian berikut berada di luar batas blok tersebut dalam IssueToken.
        }

        // Menyiapkan variabel lokal `signing` untuk nilai signing dengan memanggil `_signingKeyProvider.GetActiveSigningMaterial` dengan tanpa argumen.
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var signing = _signingKeyProvider.GetActiveSigningMaterial();
        // Menyiapkan variabel lokal `now` untuk nilai now dengan `DateTimeOffset.UtcNow`, yaitu waktu UTC saat operasi dilakukan. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var now = DateTimeOffset.UtcNow;
        // Menyiapkan variabel lokal `expiresAt` untuk nilai expires at dengan memanggil `now.AddMinutes` dengan `options.AccessTokenMinutes`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var expiresAt = now.AddMinutes(options.AccessTokenMinutes);

        // Menyiapkan variabel lokal `claims` untuk nilai claims dengan objek baru bertipe `List<Claim>` dengan nilai awal sesuai konstruktornya. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var claims = new List<Claim>
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam IssueToken.
        {
            // Menggunakan objek baru dengan tipe mengikuti konteks tujuan dan argumen (JwtRegisteredClaimNames.Sub, user.UserId.ToString()) sebagai bagian
            // ekspresi yang sedang disusun dalam IssueToken.
            new(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            // Menggunakan objek baru dengan tipe mengikuti konteks tujuan dan argumen (JwtRegisteredClaimNames.UniqueName, user.Username) sebagai bagian
            // ekspresi yang sedang disusun dalam IssueToken.
            new(JwtRegisteredClaimNames.UniqueName, user.Username),
            // Menggunakan objek baru dengan tipe mengikuti konteks tujuan dan argumen (ClaimTypes.NameIdentifier, user.UserId.ToString()) sebagai bagian
            // ekspresi yang sedang disusun dalam IssueToken.
            new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            // Menggunakan objek baru dengan tipe mengikuti konteks tujuan dan argumen (ClaimTypes.Name, user.Username) sebagai bagian ekspresi yang sedang
            // disusun dalam IssueToken.
            new(ClaimTypes.Name, user.Username),
            // Menggunakan objek baru dengan tipe mengikuti konteks tujuan dan argumen (ClaimTypes.Role, user.Role.ToUpperInvariant()) sebagai bagian ekspresi
            // yang sedang disusun dalam IssueToken.
            new(ClaimTypes.Role, user.Role.ToUpperInvariant()),
            // Menggunakan objek baru dengan tipe mengikuti konteks tujuan dan argumen (DemoAccountClaim, user.IsDemo ? ”true” : ”false”) sebagai bagian
            // ekspresi yang sedang disusun dalam IssueToken.
            new(DemoAccountClaim, user.IsDemo ? "true" : "false")
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam IssueToken.
        };

        // Menyiapkan variabel lokal `descriptor` untuk nilai descriptor dengan objek baru bertipe `JwtSecurityToken` dengan argumen ( issuer:
        // options.Issuer, audience: options.Audience, claims: claims, notBefore: now.UtcDateTime, expires: expiresAt.UtcDateTime, signingCredentials:
        // signing.Si.... Tipe variabel disimpulkan dari ekspresi nilai awal.
        var descriptor = new JwtSecurityToken(
            // Meneruskan `options.Issuer` (nilai issuer) sebagai argumen bernama `issuer`.
            issuer: options.Issuer,
            // Meneruskan `options.Audience` (nilai audience) sebagai argumen bernama `audience`.
            audience: options.Audience,
            // Meneruskan `claims` (nilai claims) sebagai argumen bernama `claims`.
            claims: claims,
            // Meneruskan `now.UtcDateTime` (nilai utc date time) sebagai argumen bernama `notBefore`.
            notBefore: now.UtcDateTime,
            // Meneruskan `expiresAt.UtcDateTime` (nilai utc date time) sebagai argumen bernama `expires`.
            expires: expiresAt.UtcDateTime,
            // Meneruskan `signing.SigningCredentials` (nilai signing credentials) sebagai argumen bernama `signingCredentials`.
            signingCredentials: signing.SigningCredentials);
        // Memperbarui `descriptor.Header[”kid”]` menggunakan `signing.KeyId` (nilai kunci identitas) dalam IssueToken.
        descriptor.Header["kid"] = signing.KeyId;

        // Menyiapkan variabel lokal `token` untuk token yang diteruskan pada operasi terkait dengan memanggil `new JwtSecurityTokenHandler().WriteToken`
        // dengan `descriptor`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var token = new JwtSecurityTokenHandler().WriteToken(descriptor);
        // Mengembalikan objek baru bertipe `IssuedToken` dengan argumen (token, expiresAt) kepada pemanggil dalam IssueToken; eksekusi jalur ini selesai
        // setelah nilai hasil ditentukan.
        return new IssuedToken(token, expiresAt);
    // Menutup scope metode IssueToken; bagian berikut berada di luar batas blok tersebut dalam IssueToken.
    }
// Menutup scope tipe JwtTokenService; bagian berikut berada di luar batas blok tersebut.
}

/// <summary>
/// DTO hasil penerbitan token: string JWT dan waktu kedaluwarsa.
/// </summary>
// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `IssuedToken`; sealed mencegah tipe ini diturunkan lagi.
public sealed record IssuedToken(string AccessToken, DateTimeOffset ExpiresAt);
