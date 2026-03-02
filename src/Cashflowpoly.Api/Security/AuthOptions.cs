// Fungsi file: Opsi konfigurasi bootstrap autentikasi (seed user default) dan kebijakan password.
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

/// <summary>
/// Kebijakan panjang minimum password yang berlaku pada registrasi dan reset.
/// </summary>
public static class PasswordPolicy
{
    public const int MinPasswordLength = 12;
}
