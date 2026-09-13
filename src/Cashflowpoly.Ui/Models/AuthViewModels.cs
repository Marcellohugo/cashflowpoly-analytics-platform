// Fungsi file: Mendefinisikan model tampilan dan state UI untuk AuthViewModels.
// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Models` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Models;

/// <summary>
/// Kelas statis yang menyimpan konstanta cookie/claim autentikasi, definisi peran,
/// dan kunci bahasa UI.
/// </summary>
// Mendefinisikan tipe class `AuthConstants`.
public static class AuthConstants
{
    public const string AuthenticationCookieName = ".Cashflowpoly.Ui.Auth";
    public const string DisplayNameClaim = "cashflowpoly:display_name";
    public const string AccessTokenClaim = "cashflowpoly:access_token";
    public const string SessionLanguageKey = "current_language";
    public const string InstructorRole = "INSTRUCTOR";
    public const string PlayerRole = "PLAYER";
    public const string LanguageId = "id";
    public const string LanguageEn = "en";

    /// <summary>
    /// Memeriksa apakah string peran yang diberikan merupakan peran valid (INSTRUCTOR atau PLAYER), tanpa memperhatikan huruf besar/kecil.
    /// </summary>
    // Mendefinisikan metode `IsValidRole` dengan hasil bertipe `bool`. Memeriksa apakah string peran yang diberikan merupakan peran valid (INSTRUCTOR
    // atau PLAYER), tanpa memperhatikan huruf besar/kecil. Masukan: Parameter `role` bertipe `string?` membawa peran pengguna yang menentukan hak
    // akses; nilai null diizinkan ketika data opsional belum tersedia. Nilai hasil langsung berasal dari gabungan syarat OR: setidaknya satu kondisi
    // wajib benar antara `string.Equals(role, InstructorRole, StringComparison.OrdinalIgnoreCase)` dan `string.Equals(role, PlayerRole,
    // StringComparison.OrdinalIgnoreCase)`; sisi kanan diperiksa hanya jika sisi kiri salah.
    public static bool IsValidRole(string? role) =>
        string.Equals(role, InstructorRole, StringComparison.OrdinalIgnoreCase) ||
        string.Equals(role, PlayerRole, StringComparison.OrdinalIgnoreCase);
}

/// <summary>
/// ViewModel formulir login yang menampung nama pengguna, kata sandi, URL kembali, dan pesan error.
/// </summary>
// Mendefinisikan tipe class `LoginViewModel`; sealed mencegah tipe ini diturunkan lagi.
public sealed class LoginViewModel
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? ReturnUrl { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// ViewModel formulir registrasi yang menampung nama tampilan, nama pengguna, kata sandi,
/// konfirmasi kata sandi, URL kembali, dan pesan error.
/// </summary>
// Mendefinisikan tipe class `RegisterViewModel`; sealed mencegah tipe ini diturunkan lagi.
public sealed class RegisterViewModel
{
    public string Role { get; set; } = AuthConstants.PlayerRole;
    public string DisplayName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public string? ReturnUrl { get; set; }
    public string? ErrorMessage { get; set; }
}
