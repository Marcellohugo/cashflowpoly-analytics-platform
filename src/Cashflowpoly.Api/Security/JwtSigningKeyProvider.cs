using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Cashflowpoly.Api.Security;

/// <summary>
/// Menyediakan signing key JWT aktif dan daftar key validasi dengan dukungan rotasi.
/// </summary>
public sealed class JwtSigningKeyProvider
{
    private const int MinSigningKeyLength = 32;

    private readonly IOptions<JwtOptions> _options;
    private readonly ILogger<JwtSigningKeyProvider> _logger;
    private string _lastActiveKeyId = string.Empty;

    public JwtSigningKeyProvider(IOptions<JwtOptions> options, ILogger<JwtSigningKeyProvider> logger)
    {
        _options = options;
        _logger = logger;
    }

    public JwtSigningMaterial GetActiveSigningMaterial()
    {
        var now = DateTimeOffset.UtcNow;
        var keys = ResolveConfiguredKeys(_options.Value);
        var active = keys
            .Where(key => IsActiveAt(key, now))
            .OrderBy(key => key.ActivateAtUtc ?? DateTimeOffset.MinValue)
            .LastOrDefault();

        if (active is null)
        {
            throw new InvalidOperationException(
                "Tidak ada JWT signing key aktif. Periksa konfigurasi ActivateAtUtc/RetireAtUtc.");
        }

        var previousKeyId = Interlocked.Exchange(ref _lastActiveKeyId, active.KeyId);
        if (!string.Equals(previousKeyId, active.KeyId, StringComparison.Ordinal))
        {
            _logger.LogInformation(
                "jwt_signing_key_rotated previous_kid={PreviousKeyId} current_kid={CurrentKeyId} activate_at_utc={ActivateAtUtc}",
                string.IsNullOrWhiteSpace(previousKeyId) ? "none" : previousKeyId,
                active.KeyId,
                active.ActivateAtUtc);
        }

        var securityKey = BuildSymmetricSecurityKey(active.KeyId, active.SigningKey);
        var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        return new JwtSigningMaterial(active.KeyId, securityKey, signingCredentials);
    }

    public IReadOnlyCollection<SecurityKey> ResolveValidationKeys(string? keyId)
    {
        var now = DateTimeOffset.UtcNow;
        var options = _options.Value;
        var keys = ResolveConfiguredKeys(options);

        IEnumerable<ResolvedJwtSigningKey> candidates = keys.Where(key =>
            !key.RetireAtUtc.HasValue ||
            key.RetireAtUtc.Value > now.AddMinutes(-(options.AccessTokenMinutes + 5)));

        if (!string.IsNullOrWhiteSpace(keyId))
        {
            candidates = candidates.Where(key =>
                string.Equals(key.KeyId, keyId.Trim(), StringComparison.Ordinal));
        }

        return candidates
            .Select(key => BuildSymmetricSecurityKey(key.KeyId, key.SigningKey))
            .Cast<SecurityKey>()
            .ToArray();
    }

    public void ValidateConfiguration()
    {
        _ = GetActiveSigningMaterial();
        _ = ResolveValidationKeys(null);
    }

    private static bool IsActiveAt(ResolvedJwtSigningKey key, DateTimeOffset now)
    {
        var activateAt = key.ActivateAtUtc ?? DateTimeOffset.MinValue;
        var notRetired = !key.RetireAtUtc.HasValue || key.RetireAtUtc.Value > now;
        return activateAt <= now && notRetired;
    }

    private static SymmetricSecurityKey BuildSymmetricSecurityKey(string keyId, string signingKey)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey))
        {
            KeyId = keyId
        };
        return key;
    }

    private static List<ResolvedJwtSigningKey> ResolveConfiguredKeys(JwtOptions options)
    {
        var configuredKeys = new List<JwtSigningKeyOptions>();

        if (options.SigningKeys.Count > 0)
        {
            configuredKeys.AddRange(options.SigningKeys);
        }

        AppendKeysFromJson(configuredKeys, options.SigningKeysJson);
        AppendKeysFromJson(configuredKeys, ReadSecretFile(options.SigningKeysFile));
        AppendKeysFromJson(
            configuredKeys,
            ReadEnvironmentVariable(options.SigningKeysJsonEnvironmentVariable, "JWT_SIGNING_KEYS_JSON"));

        if (configuredKeys.Count == 0)
        {
            var fallbackSigningKey = ResolveFallbackSigningKey(options);
            if (string.IsNullOrWhiteSpace(fallbackSigningKey))
            {
                throw new InvalidOperationException(
                    "JWT signing key belum dikonfigurasi. Gunakan Jwt:SigningKeys/Jwt:SigningKey atau secret file/env.");
            }

            configuredKeys.Add(new JwtSigningKeyOptions
            {
                KeyId = string.IsNullOrWhiteSpace(options.ActiveKeyId) ? "legacy" : options.ActiveKeyId.Trim(),
                SigningKey = fallbackSigningKey,
                ActivateAtUtc = DateTimeOffset.MinValue
            });
        }

        var keyIds = new HashSet<string>(StringComparer.Ordinal);
        var resolved = new List<ResolvedJwtSigningKey>();

        for (var i = 0; i < configuredKeys.Count; i++)
        {
            var item = configuredKeys[i];
            var keyId = string.IsNullOrWhiteSpace(item.KeyId)
                ? $"key-{i + 1:00}"
                : item.KeyId.Trim();
            if (!keyIds.Add(keyId))
            {
                throw new InvalidOperationException($"Duplikasi Jwt key id terdeteksi: '{keyId}'.");
            }

            var signingKey = ResolveSigningKey(item);
            if (string.IsNullOrWhiteSpace(signingKey))
            {
                throw new InvalidOperationException($"Signing key untuk kid '{keyId}' belum dikonfigurasi.");
            }

            if (signingKey.Length < MinSigningKeyLength)
            {
                throw new InvalidOperationException(
                    $"Signing key untuk kid '{keyId}' minimal {MinSigningKeyLength} karakter.");
            }

            if (string.Equals(signingKey, "change-this-jwt-signing-key-for-production-2026", StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    $"Signing key untuk kid '{keyId}' masih placeholder. Gunakan secret yang kuat.");
            }

            if (item.RetireAtUtc.HasValue && item.ActivateAtUtc.HasValue && item.RetireAtUtc <= item.ActivateAtUtc)
            {
                throw new InvalidOperationException(
                    $"Rentang aktif key '{keyId}' tidak valid: RetireAtUtc harus lebih besar dari ActivateAtUtc.");
            }

            resolved.Add(new ResolvedJwtSigningKey(
                keyId,
                signingKey,
                item.ActivateAtUtc,
                item.RetireAtUtc));
        }

        return resolved
            .OrderBy(key => key.ActivateAtUtc ?? DateTimeOffset.MinValue)
            .ThenBy(key => key.KeyId, StringComparer.Ordinal)
            .ToList();
    }

    private static void AppendKeysFromJson(List<JwtSigningKeyOptions> target, string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return;
        }

        try
        {
            var parsed = JsonSerializer.Deserialize<List<JwtSigningKeyOptions>>(
                json,
                new JsonSerializerOptions(JsonSerializerDefaults.Web));
            if (parsed is null || parsed.Count == 0)
            {
                return;
            }

            target.AddRange(parsed);
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException("Format JSON untuk konfigurasi JWT signing keys tidak valid.", ex);
        }
    }

    private static string ResolveFallbackSigningKey(JwtOptions options)
    {
        if (!string.IsNullOrWhiteSpace(options.SigningKey))
        {
            return options.SigningKey.Trim();
        }

        var fileValue = ReadSecretFile(options.SigningKeyFile);
        if (!string.IsNullOrWhiteSpace(fileValue))
        {
            return fileValue;
        }

        return ReadEnvironmentVariable(options.SigningKeyEnvironmentVariable, "JWT_SIGNING_KEY");
    }

    private static string ResolveSigningKey(JwtSigningKeyOptions options)
    {
        if (!string.IsNullOrWhiteSpace(options.SigningKey))
        {
            return options.SigningKey.Trim();
        }

        return ReadSecretFile(options.SigningKeyFile);
    }

    private static string ReadEnvironmentVariable(string configuredName, string fallbackName)
    {
        var envName = string.IsNullOrWhiteSpace(configuredName) ? fallbackName : configuredName.Trim();
        return (Environment.GetEnvironmentVariable(envName) ?? string.Empty).Trim();
    }

    private static string ReadSecretFile(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return string.Empty;
        }

        var normalizedPath = path.Trim();
        if (!File.Exists(normalizedPath))
        {
            return string.Empty;
        }

        return File.ReadAllText(normalizedPath).Trim();
    }

    private sealed record ResolvedJwtSigningKey(
        string KeyId,
        string SigningKey,
        DateTimeOffset? ActivateAtUtc,
        DateTimeOffset? RetireAtUtc);
}

public sealed record JwtSigningMaterial(
    string KeyId,
    SecurityKey SecurityKey,
    SigningCredentials SigningCredentials);
