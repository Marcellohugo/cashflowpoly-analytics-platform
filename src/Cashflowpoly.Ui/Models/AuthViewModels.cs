// Fungsi file: Mendefinisikan model tampilan dan state UI untuk AuthViewModels.
namespace Cashflowpoly.Ui.Models;

/// <summary>
/// Kelas statis yang menyimpan konstanta cookie/claim autentikasi, definisi peran,
/// dan kunci bahasa UI.
/// </summary>
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
/// konfirmasi kata sandi, URL kembali, dan pesan error.
/// </summary>
public sealed class RegisterViewModel
{
    public string DisplayName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public string? ReturnUrl { get; set; }
    public string? ErrorMessage { get; set; }
}
