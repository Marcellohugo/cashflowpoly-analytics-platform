// Fungsi file: Menerapkan kontrol keamanan aplikasi melalui JwtSigningKeyProvider.
// Mengimpor namespace `System.Text` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text;
// Mengimpor namespace `System.Text.Json` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.Json;
// Mengimpor namespace `Microsoft.Extensions.Options` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.Extensions.Options;
// Mengimpor namespace `Microsoft.IdentityModel.Tokens` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.IdentityModel.Tokens;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Security` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Security;

/// <summary>
/// Menyediakan signing key JWT aktif dan daftar key validasi dengan dukungan rotasi.
/// </summary>
// Mendefinisikan tipe class `JwtSigningKeyProvider`; sealed mencegah tipe ini diturunkan lagi.
public sealed class JwtSigningKeyProvider
{
    private const int MinSigningKeyLength = 32;

    private readonly IOptions<JwtOptions> _options;
    private readonly ILogger<JwtSigningKeyProvider> _logger;
    private string _lastActiveKeyId = string.Empty;

    /// <summary>
    /// Menerima opsi JWT dan logger untuk resolusi dan pemantauan rotasi signing key.
    /// </summary>
    // Mendefinisikan konstruktor JwtSigningKeyProvider yang menyiapkan objek dan menerima dependency/nilai awal dari pemanggil; parameter: Parameter
    // `options` bertipe `IOptions<JwtOptions>` membawa kumpulan pengaturan yang mengendalikan perilaku komponen; Parameter `logger` bertipe
    // `ILogger<JwtSigningKeyProvider>` membawa pencatat log terstruktur untuk memantau proses dan mendiagnosis kegagalan.
    public JwtSigningKeyProvider(IOptions<JwtOptions> options, ILogger<JwtSigningKeyProvider> logger)
    {
        _options = options;
        _logger = logger;
    }

    /// <summary>
    /// Mengambil material signing key aktif berdasarkan jendela waktu ActivateAtUtc/RetireAtUtc.
    /// </summary>
    // Mendefinisikan metode `GetActiveSigningMaterial` dengan hasil bertipe `JwtSigningMaterial`. Mengambil material signing key aktif berdasarkan
    // jendela waktu ActivateAtUtc/RetireAtUtc.
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
            // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen ( ”Tidak ada JWT signing key aktif. Periksa
            // konfigurasi ActivateAtUtc/RetireAtUtc.”) dalam GetActiveSigningMaterial; pemanggil atau middleware penanganan error menerima kegagalan ini.
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

    /// <summary>
    /// Mengembalikan daftar SecurityKey yang masih valid (belum retire) untuk validasi token; opsional filter by kid.
    /// </summary>
    // Mendefinisikan metode `ResolveValidationKeys` dengan hasil bertipe `IReadOnlyCollection<SecurityKey>`. Mengembalikan daftar SecurityKey yang
    // masih valid (belum retire) untuk validasi token; opsional filter by kid. Masukan: Parameter `keyId` bertipe `string?` membawa nilai kunci
    // identitas; nilai null diizinkan ketika data opsional belum tersedia.
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

    /// <summary>
    /// Memvalidasi bahwa minimal satu key aktif dan daftar validasi tersedia.
    /// </summary>
    // Mendefinisikan metode `ValidateConfiguration` dengan hasil bertipe `void`. Memvalidasi bahwa minimal satu key aktif dan daftar validasi tersedia.
    public void ValidateConfiguration()
    {
        _ = GetActiveSigningMaterial();
        _ = ResolveValidationKeys(null);
    }

    /// <summary>
    /// Memeriksa apakah key aktif pada waktu tertentu (sudah diaktifkan dan belum di-retire).
    /// </summary>
    // Mendefinisikan metode `IsActiveAt` dengan hasil bertipe `bool`. Memeriksa apakah key aktif pada waktu tertentu (sudah diaktifkan dan belum
    // di-retire). Masukan: Parameter `key` bertipe `ResolvedJwtSigningKey` membawa nilai kunci; Parameter `now` bertipe `DateTimeOffset` membawa nilai
    // now.
    private static bool IsActiveAt(ResolvedJwtSigningKey key, DateTimeOffset now)
    {
        var activateAt = key.ActivateAtUtc ?? DateTimeOffset.MinValue;
        var notRetired = !key.RetireAtUtc.HasValue || key.RetireAtUtc.Value > now;
        return activateAt <= now && notRetired;
    }

    /// <summary>
    /// Membuat SymmetricSecurityKey dari string signing key dengan kid terlampir.
    /// </summary>
    // Mendefinisikan metode `BuildSymmetricSecurityKey` dengan hasil bertipe `SymmetricSecurityKey`. Membuat SymmetricSecurityKey dari string signing
    // key dengan kid terlampir. Masukan: Parameter `keyId` bertipe `string` membawa nilai kunci identitas; Parameter `signingKey` bertipe `string`
    // membawa kunci untuk membentuk atau memverifikasi tanda tangan token JWT.
    private static SymmetricSecurityKey BuildSymmetricSecurityKey(string keyId, string signingKey)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey))
        {
            KeyId = keyId
        };
        return key;
    }

    /// <summary>
    /// Mengumpulkan seluruh key dari berbagai sumber (config, JSON string, file, env), lalu memvalidasi dan mengurutkannya.
    /// </summary>
    // Mendefinisikan metode `ResolveConfiguredKeys` dengan hasil bertipe `List<ResolvedJwtSigningKey>`. Mengumpulkan seluruh key dari berbagai sumber
    // (config, JSON string, file, env), lalu memvalidasi dan mengurutkannya. Masukan: Parameter `options` bertipe `JwtOptions` membawa kumpulan
    // pengaturan yang mengendalikan perilaku komponen.
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
                // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen ( ”JWT signing key belum dikonfigurasi. Gunakan
                // Jwt:SigningKeys/Jwt:SigningKey atau secret file/env.”) dalam ResolveConfiguredKeys; pemanggil atau middleware penanganan error menerima kegagalan
                // ini.
                throw new InvalidOperationException(
                    "JWT signing key belum dikonfigurasi. Gunakan Jwt:SigningKeys/Jwt:SigningKey atau secret file/env.");
            }

            configuredKeys.Add(new JwtSigningKeyOptions
            {
                KeyId = string.IsNullOrWhiteSpace(options.ActiveKeyId) ? "default" : options.ActiveKeyId.Trim(),
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
                // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: $”key-{i + 1:00}” dalam ResolveConfiguredKeys.
                ? $"key-{i + 1:00}"
                // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: item.KeyId.Trim(); dalam ResolveConfiguredKeys.
                : item.KeyId.Trim();
            if (!keyIds.Add(keyId))
            {
                // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen ($”Duplikasi Jwt key id terdeteksi: '{keyId}'.”)
                // dalam ResolveConfiguredKeys; pemanggil atau middleware penanganan error menerima kegagalan ini.
                throw new InvalidOperationException($"Duplikasi Jwt key id terdeteksi: '{keyId}'.");
            }

            var signingKey = ResolveSigningKey(item);
            if (string.IsNullOrWhiteSpace(signingKey))
            {
                // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen ($”Signing key untuk kid '{keyId}' belum
                // dikonfigurasi.”) dalam ResolveConfiguredKeys; pemanggil atau middleware penanganan error menerima kegagalan ini.
                throw new InvalidOperationException($"Signing key untuk kid '{keyId}' belum dikonfigurasi.");
            }

            if (signingKey.Length < MinSigningKeyLength)
            {
                // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen ( $”Signing key untuk kid '{keyId}' minimal
                // {MinSigningKeyLength} karakter.”) dalam ResolveConfiguredKeys; pemanggil atau middleware penanganan error menerima kegagalan ini.
                throw new InvalidOperationException(
                    $"Signing key untuk kid '{keyId}' minimal {MinSigningKeyLength} karakter.");
            }

            if (string.Equals(signingKey, "change-this-jwt-signing-key-for-production-2026", StringComparison.Ordinal))
            {
                // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen ( $”Signing key untuk kid '{keyId}' masih
                // placeholder. Gunakan secret yang kuat.”) dalam ResolveConfiguredKeys; pemanggil atau middleware penanganan error menerima kegagalan ini.
                throw new InvalidOperationException(
                    $"Signing key untuk kid '{keyId}' masih placeholder. Gunakan secret yang kuat.");
            }

            if (item.RetireAtUtc.HasValue && item.ActivateAtUtc.HasValue && item.RetireAtUtc <= item.ActivateAtUtc)
            {
                // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen ( $”Rentang aktif key '{keyId}' tidak valid:
                // RetireAtUtc harus lebih besar dari ActivateAtUtc.”) dalam ResolveConfiguredKeys; pemanggil atau middleware penanganan error menerima kegagalan
                // ini.
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

    /// <summary>
    /// Memparsing JSON array berisi konfigurasi key dan menambahkannya ke daftar target.
    /// </summary>
    // Mendefinisikan metode `AppendKeysFromJson` dengan hasil bertipe `void`. Memparsing JSON array berisi konfigurasi key dan menambahkannya ke daftar
    // target. Masukan: Parameter `target` bertipe `List<JwtSigningKeyOptions>` membawa nilai target; Parameter `json` bertipe `string` membawa nilai
    // JSON.
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
        // Menangani exception `JsonException` melalui variabel ex dalam AppendKeysFromJson.
        catch (JsonException ex)
        {
            // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen (”Format JSON untuk konfigurasi JWT signing keys
            // tidak valid.”, ex) dalam AppendKeysFromJson; pemanggil atau middleware penanganan error menerima kegagalan ini.
            throw new InvalidOperationException("Format JSON untuk konfigurasi JWT signing keys tidak valid.", ex);
        }
    }

    /// <summary>
    /// Mengambil signing key fallback dari opsi langsung, file, atau environment variable.
    /// </summary>
    // Mendefinisikan metode `ResolveFallbackSigningKey` dengan hasil bertipe `string`. Mengambil signing key fallback dari opsi langsung, file, atau
    // environment variable. Masukan: Parameter `options` bertipe `JwtOptions` membawa kumpulan pengaturan yang mengendalikan perilaku komponen.
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

    /// <summary>
    /// Mengambil signing key dari opsi inline atau dari file secret.
    /// </summary>
    // Mendefinisikan metode `ResolveSigningKey` dengan hasil bertipe `string`. Mengambil signing key dari opsi inline atau dari file secret. Masukan:
    // Parameter `options` bertipe `JwtSigningKeyOptions` membawa kumpulan pengaturan yang mengendalikan perilaku komponen.
    private static string ResolveSigningKey(JwtSigningKeyOptions options)
    {
        if (!string.IsNullOrWhiteSpace(options.SigningKey))
        {
            return options.SigningKey.Trim();
        }

        return ReadSecretFile(options.SigningKeyFile);
    }

    /// <summary>
    /// Membaca nilai environment variable; jika nama konfigurasi kosong, gunakan fallback.
    /// </summary>
    // Mendefinisikan metode `ReadEnvironmentVariable` dengan hasil bertipe `string`. Membaca nilai environment variable; jika nama konfigurasi kosong,
    // gunakan fallback. Masukan: Parameter `configuredName` bertipe `string` membawa nilai configured nama; Parameter `fallbackName` bertipe `string`
    // membawa nilai fallback nama.
    private static string ReadEnvironmentVariable(string configuredName, string fallbackName)
    {
        var envName = string.IsNullOrWhiteSpace(configuredName) ? fallbackName : configuredName.Trim();
        return (Environment.GetEnvironmentVariable(envName) ?? string.Empty).Trim();
    }

    /// <summary>
    /// Membaca isi file secret dari path yang diberikan; mengembalikan string kosong jika file tidak ada.
    /// </summary>
    // Mendefinisikan metode `ReadSecretFile` dengan hasil bertipe `string`. Membaca isi file secret dari path yang diberikan; mengembalikan string
    // kosong jika file tidak ada. Masukan: Parameter `path` bertipe `string` membawa nilai path.
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

    /// <summary>
    /// DTO internal hasil resolusi satu signing key beserta jendela waktu aktifnya.
    /// </summary>
    // Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `ResolvedJwtSigningKey`; sealed mencegah tipe ini diturunkan lagi.
    private sealed record ResolvedJwtSigningKey(
        // Parameter `KeyId` bertipe `string` membawa nilai kunci identitas.
        string KeyId,
        // Parameter `SigningKey` bertipe `string` membawa kunci untuk membentuk atau memverifikasi tanda tangan token JWT.
        string SigningKey,
        // Parameter `ActivateAtUtc` bertipe `DateTimeOffset?` membawa nilai activate at utc; nilai null diizinkan ketika data opsional belum tersedia.
        DateTimeOffset? ActivateAtUtc,
        // Parameter `RetireAtUtc` bertipe `DateTimeOffset?` membawa nilai retire at utc; nilai null diizinkan ketika data opsional belum tersedia.
        DateTimeOffset? RetireAtUtc);
}

/// <summary>
/// Material signing JWT yang siap pakai: key ID, SecurityKey, dan SigningCredentials.
/// </summary>
// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `JwtSigningMaterial`; sealed mencegah tipe ini diturunkan lagi.
public sealed record JwtSigningMaterial(
    // Parameter `KeyId` bertipe `string` membawa nilai kunci identitas.
    string KeyId,
    // Parameter `SecurityKey` bertipe `SecurityKey` membawa nilai security kunci.
    SecurityKey SecurityKey,
    // Parameter `SigningCredentials` bertipe `SigningCredentials` membawa nilai signing credentials.
    SigningCredentials SigningCredentials);
