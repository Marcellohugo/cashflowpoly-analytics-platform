namespace Cashflowpoly.Api.Security;

public sealed class JwtOptions
{
    public string Issuer { get; set; } = "Cashflowpoly.Api";
    public string Audience { get; set; } = "Cashflowpoly.Client";
    public string SigningKey { get; set; } = string.Empty;
    public int AccessTokenMinutes { get; set; } = 480;
}
