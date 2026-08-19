// Fungsi file: Menerapkan kontrol keamanan aplikasi melalui AuthOptions.
using System.Text;

namespace Cashflowpoly.Api.Security;

/// <summary>
/// Opsi untuk men-seed akun instruktur dan pemain default saat startup.
/// </summary>
public sealed class AuthBootstrapOptions
{
    public bool SeedDefaultUsers { get; set; }
    public string? InstructorUsername { get; set; }
    public string? InstructorPassword { get; set; }
    public string? PlayerUsername { get; set; }
    public string? PlayerPassword { get; set; }
}

public sealed class AuthRegistrationOptions
{
    public bool AllowPublicInstructorRegistration { get; set; }

    public bool CanRegisterPublicly(string role) =>
        AllowPublicInstructorRegistration ||
        !string.Equals(role, "INSTRUCTOR", StringComparison.OrdinalIgnoreCase);
}

/// <summary>
/// Kebijakan panjang minimum password yang berlaku pada registrasi dan reset.
/// </summary>
public static class PasswordPolicy
{
    public const int MinPasswordLength = 12;
    public const int MaxPasswordUtf8Bytes = 72;

    public static bool IsWithinBcryptLimit(string password) =>
        Encoding.UTF8.GetByteCount(password) <= MaxPasswordUtf8Bytes;
}
