// Fungsi file: Menyediakan komponen keamanan aplikasi untuk domain AuthOptions (JWT, audit, atau rate limiting).
namespace Cashflowpoly.Api.Security;

/// <summary>
/// Menyatakan peran utama tipe AuthBootstrapOptions pada modul ini.
/// </summary>
public sealed class AuthBootstrapOptions
{
    public bool SeedDefaultUsers { get; set; }
    public string? InstructorUsername { get; set; }
    public string? InstructorPassword { get; set; }
    public string? PlayerUsername { get; set; }
    public string? PlayerPassword { get; set; }
}
