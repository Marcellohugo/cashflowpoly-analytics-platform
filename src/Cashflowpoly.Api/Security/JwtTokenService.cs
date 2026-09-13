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
{
    public const string DemoAccountClaim = "is_demo";
    private readonly IOptions<JwtOptions> _options;
    private readonly JwtSigningKeyProvider _signingKeyProvider;

    /// <summary>
    /// Menerima opsi JWT dan penyedia signing key.
    /// </summary>
    // Mendefinisikan konstruktor JwtTokenService yang menyiapkan objek dan menerima dependency/nilai awal dari pemanggil; parameter: Parameter
    // `options` bertipe `IOptions<JwtOptions>` membawa kumpulan pengaturan yang mengendalikan perilaku komponen; Parameter `signingKeyProvider` bertipe
    // `JwtSigningKeyProvider` membawa nilai signing kunci provider.
    public JwtTokenService(IOptions<JwtOptions> options, JwtSigningKeyProvider signingKeyProvider)
    {
        _options = options;
        _signingKeyProvider = signingKeyProvider;
    }

    /// <summary>
    /// Membuat dan menandatangani JWT berisi claim identitas dan role pengguna.
    /// </summary>
    // Mendefinisikan metode `IssueToken` dengan hasil bertipe `IssuedToken`. Membuat dan menandatangani JWT berisi claim identitas dan role pengguna.
    // Masukan: Parameter `user` bertipe `AuthenticatedUserDb` membawa pengguna yang sedang diproses beserta identitas atau klaim akses yang
    // dimilikinya.
    public IssuedToken IssueToken(AuthenticatedUserDb user)
    {
        var options = _options.Value;
        if (options.AccessTokenMinutes <= 0)
        {
            // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen (”Jwt:AccessTokenMinutes harus lebih besar dari
            // 0.”) dalam IssueToken; pemanggil atau middleware penanganan error menerima kegagalan ini.
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
            new(ClaimTypes.Role, user.Role.ToUpperInvariant()),
            new(DemoAccountClaim, user.IsDemo ? "true" : "false")
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
// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `IssuedToken`; sealed mencegah tipe ini diturunkan lagi.
public sealed record IssuedToken(string AccessToken, DateTimeOffset ExpiresAt);
