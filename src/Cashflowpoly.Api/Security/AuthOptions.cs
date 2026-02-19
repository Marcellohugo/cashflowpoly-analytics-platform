namespace Cashflowpoly.Api.Security;

public sealed class AuthBootstrapOptions
{
    public bool SeedDefaultUsers { get; set; }
    public string? InstructorUsername { get; set; }
    public string? InstructorPassword { get; set; }
    public string? PlayerUsername { get; set; }
    public string? PlayerPassword { get; set; }
}
