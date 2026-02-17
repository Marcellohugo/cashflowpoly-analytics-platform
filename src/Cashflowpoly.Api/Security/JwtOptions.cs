namespace Cashflowpoly.Api.Security;

public sealed class JwtOptions
{
    public string Issuer { get; set; } = "Cashflowpoly.Api";
    public string Audience { get; set; } = "Cashflowpoly.Client";
    public string SigningKey { get; set; } = string.Empty;
    public string SigningKeyFile { get; set; } = string.Empty;
    public string SigningKeysFile { get; set; } = string.Empty;
    public string SigningKeysJson { get; set; } = string.Empty;
    public string SigningKeyEnvironmentVariable { get; set; } = "JWT_SIGNING_KEY";
    public string SigningKeysJsonEnvironmentVariable { get; set; } = "JWT_SIGNING_KEYS_JSON";
    public string ActiveKeyId { get; set; } = "legacy";
    public List<JwtSigningKeyOptions> SigningKeys { get; set; } = new();
    public int AccessTokenMinutes { get; set; } = 480;
}

public sealed class JwtSigningKeyOptions
{
    public string KeyId { get; set; } = string.Empty;
    public string SigningKey { get; set; } = string.Empty;
    public string SigningKeyFile { get; set; } = string.Empty;
    public DateTimeOffset? ActivateAtUtc { get; set; }
    public DateTimeOffset? RetireAtUtc { get; set; }
}
