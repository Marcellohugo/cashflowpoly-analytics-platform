namespace Cashflowpoly.Ui.Models;

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

    public static bool IsValidRole(string? role) =>
        string.Equals(role, InstructorRole, StringComparison.OrdinalIgnoreCase) ||
        string.Equals(role, PlayerRole, StringComparison.OrdinalIgnoreCase);
}

public sealed class LoginViewModel
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? ReturnUrl { get; set; }
    public string? ErrorMessage { get; set; }
}

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
