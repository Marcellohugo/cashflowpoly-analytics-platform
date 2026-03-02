// Fungsi file: Mendefinisikan konstanta autentikasi sesi, serta ViewModel untuk formulir login dan registrasi pengguna.
namespace Cashflowpoly.Ui.Models;

/// <summary>
/// Kelas statis yang menyimpan konstanta kunci sesi, definisi peran (Instructor/Player),
/// dan kode bahasa yang digunakan pada modul autentikasi aplikasi.
/// </summary>
public static class AuthConstants
{
    public const string SessionUserIdKey = "current_user_id";
    public const string SessionRoleKey = "current_role";
    public const string SessionDisplayNameKey = "current_display_name";
    public const string SessionUsernameKey = "current_username";
    public const string SessionAccessTokenKey = "current_access_token";
    public const string SessionTokenExpiresAtKey = "current_token_expires_at";
    public const string SessionLanguageKey = "current_language";
    public const string InstructorRole = "INSTRUCTOR";
    public const string PlayerRole = "PLAYER";
    public const string LanguageId = "id";
    public const string LanguageEn = "en";

    /// <summary>
    /// Memeriksa apakah string peran yang diberikan merupakan peran valid (INSTRUCTOR atau PLAYER), tanpa memperhatikan huruf besar/kecil.
    /// </summary>
    public static bool IsValidRole(string? role) =>
        string.Equals(role, InstructorRole, StringComparison.OrdinalIgnoreCase) ||
        string.Equals(role, PlayerRole, StringComparison.OrdinalIgnoreCase);
}

/// <summary>
/// ViewModel formulir login yang menampung nama pengguna, kata sandi, URL kembali, dan pesan error.
/// </summary>
public sealed class LoginViewModel
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? ReturnUrl { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// ViewModel formulir registrasi yang menampung nama tampilan, nama pengguna, kata sandi,
/// konfirmasi kata sandi, pemilihan peran, URL kembali, dan pesan error.
/// </summary>
public sealed class RegisterViewModel
{
    public string DisplayName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public string Role { get; set; } = AuthConstants.PlayerRole;
    public string? ReturnUrl { get; set; }
    public string? ErrorMessage { get; set; }
}
