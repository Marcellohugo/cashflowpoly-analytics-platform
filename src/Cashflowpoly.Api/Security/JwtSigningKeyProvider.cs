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
// Membuka scope tipe JwtSigningKeyProvider; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendeklarasikan field bertipe `int`: `MinSigningKeyLength` menyimpan nilai minimum signing kunci length dengan nilai awal nilai literal `32`.
    private const int MinSigningKeyLength = 32;

    // Mendeklarasikan field bertipe `IOptions<JwtOptions>`: `_options` menyimpan kumpulan pengaturan yang mengendalikan perilaku komponen. readonly
    // membatasi penggantian referensi/nilai field pada deklarasi atau konstruktor.
    private readonly IOptions<JwtOptions> _options;
    // Mendeklarasikan field bertipe `ILogger<JwtSigningKeyProvider>`: `_logger` menyimpan pencatat log terstruktur untuk memantau proses dan
    // mendiagnosis kegagalan. readonly membatasi penggantian referensi/nilai field pada deklarasi atau konstruktor.
    private readonly ILogger<JwtSigningKeyProvider> _logger;
    // Mendeklarasikan field bertipe `string`: `_lastActiveKeyId` menyimpan nilai last aktif kunci identitas dengan nilai awal `string.Empty`, yaitu
    // nilai kosong bawaan tipe terkait.
    private string _lastActiveKeyId = string.Empty;

    /// <summary>
    /// Menerima opsi JWT dan logger untuk resolusi dan pemantauan rotasi signing key.
    /// </summary>
    // Mendefinisikan konstruktor JwtSigningKeyProvider yang menyiapkan objek dan menerima dependency/nilai awal dari pemanggil; parameter: Parameter
    // `options` bertipe `IOptions<JwtOptions>` membawa kumpulan pengaturan yang mengendalikan perilaku komponen; Parameter `logger` bertipe
    // `ILogger<JwtSigningKeyProvider>` membawa pencatat log terstruktur untuk memantau proses dan mendiagnosis kegagalan.
    public JwtSigningKeyProvider(IOptions<JwtOptions> options, ILogger<JwtSigningKeyProvider> logger)
    // Membuka scope konstruktor JwtSigningKeyProvider; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam JwtSigningKeyProvider.
    {
        // Memperbarui `_options` menggunakan `options` (kumpulan pengaturan yang mengendalikan perilaku komponen) dalam JwtSigningKeyProvider.
        _options = options;
        // Memperbarui `_logger` menggunakan `logger` (pencatat log terstruktur untuk memantau proses dan mendiagnosis kegagalan) dalam
        // JwtSigningKeyProvider.
        _logger = logger;
    // Menutup scope konstruktor JwtSigningKeyProvider; bagian berikut berada di luar batas blok tersebut dalam JwtSigningKeyProvider.
    }

    /// <summary>
    /// Mengambil material signing key aktif berdasarkan jendela waktu ActivateAtUtc/RetireAtUtc.
    /// </summary>
    // Mendefinisikan metode `GetActiveSigningMaterial` dengan hasil bertipe `JwtSigningMaterial`. Mengambil material signing key aktif berdasarkan
    // jendela waktu ActivateAtUtc/RetireAtUtc.
    public JwtSigningMaterial GetActiveSigningMaterial()
    // Membuka scope metode GetActiveSigningMaterial; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam GetActiveSigningMaterial.
    {
        // Menyiapkan variabel lokal `now` untuk nilai now dengan `DateTimeOffset.UtcNow`, yaitu waktu UTC saat operasi dilakukan. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var now = DateTimeOffset.UtcNow;
        // Menyiapkan variabel lokal `keys` untuk nilai kunci dengan memanggil `ResolveConfiguredKeys` dengan `_options.Value`. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var keys = ResolveConfiguredKeys(_options.Value);
        // Menyiapkan variabel lokal `active` untuk nilai aktif dengan mengambil elemen terakhir `keys .Where(key => IsActiveAt(key, now)) .OrderBy(key =>
        // key.ActivateAtUtc ?? DateTimeOffset.MinValue)` sesuai tanpa argumen; hasil default bila tidak ada elemen cocok. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var active = keys
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(key => IsActiveAt(key, now)) dalam GetActiveSigningMaterial; token pada
            // baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Where(key => IsActiveAt(key, now))
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .OrderBy(key => key.ActivateAtUtc ?? DateTimeOffset.MinValue) dalam
            // GetActiveSigningMaterial; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .OrderBy(key => key.ActivateAtUtc ?? DateTimeOffset.MinValue)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .LastOrDefault(); dalam GetActiveSigningMaterial; token pada baris ini
            // menyambungkan bagian kode sebelum dan sesudahnya.
            .LastOrDefault();

        // Memeriksa hasil pencocokan `active` dengan pola `null`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // GetActiveSigningMaterial.
        if (active is null)
        // Membuka scope cabang if untuk kondisi `active is null`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // GetActiveSigningMaterial.
        {
            // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen ( ”Tidak ada JWT signing key aktif. Periksa
            // konfigurasi ActivateAtUtc/RetireAtUtc.”) dalam GetActiveSigningMaterial; pemanggil atau middleware penanganan error menerima kegagalan ini.
            throw new InvalidOperationException(
                // Meneruskan nilai literal `”Tidak ada JWT signing key aktif. Periksa konfigurasi ActivateAtUtc/RetireAtUtc.”` sebagai argumen ke konstruktor
                // `InvalidOperationException`.
                "Tidak ada JWT signing key aktif. Periksa konfigurasi ActivateAtUtc/RetireAtUtc.");
        // Menutup scope cabang if untuk kondisi `active is null`; bagian berikut berada di luar batas blok tersebut dalam GetActiveSigningMaterial.
        }

        // Menyiapkan variabel lokal `previousKeyId` untuk nilai previous kunci identitas dengan memanggil `Interlocked.Exchange` dengan `_lastActiveKeyId`,
        // `active.KeyId`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var previousKeyId = Interlocked.Exchange(ref _lastActiveKeyId, active.KeyId);
        // Memeriksa kebalikan kondisi `string.Equals(previousKeyId, active.KeyId, StringComparison.Ordinal)`; blok if hanya dijalankan ketika kondisi ini
        // bernilai benar dalam GetActiveSigningMaterial.
        if (!string.Equals(previousKeyId, active.KeyId, StringComparison.Ordinal))
        // Membuka scope cabang if untuk kondisi `!string.Equals(previousKeyId, active.KeyId, StringComparison.Ordinal)`; pernyataan/deklarasi berikut
        // berada di dalam batas blok ini dalam GetActiveSigningMaterial.
        {
            // Menjalankan mencatat log tingkat Information melalui `_logger` dengan pesan dan data `”jwt_signing_key_rotated previous_kid={PreviousKeyId}
            // current_kid={CurrentKeyId} activate_at_utc={ActivateAtUtc}”`, `string.IsNullOrWhiteSpace(previousKeyId) ? ”none” : previousKeyId`,
            // `active.KeyId`, `active.ActivateAtUtc` dalam GetActiveSigningMaterial.
            _logger.LogInformation(
                // Meneruskan nilai literal `”jwt_signing_key_rotated previous_kid={PreviousKeyId} current_kid={CurrentKeyId} activate_at_utc={ActivateAtUtc}”`
                // sebagai argumen ke `_logger.LogInformation`.
                "jwt_signing_key_rotated previous_kid={PreviousKeyId} current_kid={CurrentKeyId} activate_at_utc={ActivateAtUtc}",
                // Meneruskan hasil pemilihan bersyarat: ketika `string.IsNullOrWhiteSpace(previousKeyId)` benar gunakan `”none”`, jika tidak gunakan
                // `previousKeyId` sebagai argumen ke `_logger.LogInformation`; Meneruskan `previousKeyId` (nilai previous kunci identitas) sebagai argumen ke
                // `string.IsNullOrWhiteSpace`.
                string.IsNullOrWhiteSpace(previousKeyId) ? "none" : previousKeyId,
                // Meneruskan `active.KeyId` (nilai kunci identitas) sebagai argumen ke `_logger.LogInformation`.
                active.KeyId,
                // Meneruskan `active.ActivateAtUtc` (nilai activate at utc) sebagai argumen ke `_logger.LogInformation`.
                active.ActivateAtUtc);
        // Menutup scope cabang if untuk kondisi `!string.Equals(previousKeyId, active.KeyId, StringComparison.Ordinal)`; bagian berikut berada di luar
        // batas blok tersebut dalam GetActiveSigningMaterial.
        }

        // Menyiapkan variabel lokal `securityKey` untuk nilai security kunci dengan memanggil `BuildSymmetricSecurityKey` dengan `active.KeyId`,
        // `active.SigningKey`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var securityKey = BuildSymmetricSecurityKey(active.KeyId, active.SigningKey);
        // Menyiapkan variabel lokal `signingCredentials` untuk nilai signing credentials dengan objek baru bertipe `SigningCredentials` dengan argumen
        // (securityKey, SecurityAlgorithms.HmacSha256). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        // Mengembalikan objek baru bertipe `JwtSigningMaterial` dengan argumen (active.KeyId, securityKey, signingCredentials) kepada pemanggil dalam
        // GetActiveSigningMaterial; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return new JwtSigningMaterial(active.KeyId, securityKey, signingCredentials);
    // Menutup scope metode GetActiveSigningMaterial; bagian berikut berada di luar batas blok tersebut dalam GetActiveSigningMaterial.
    }

    /// <summary>
    /// Mengembalikan daftar SecurityKey yang masih valid (belum retire) untuk validasi token; opsional filter by kid.
    /// </summary>
    // Mendefinisikan metode `ResolveValidationKeys` dengan hasil bertipe `IReadOnlyCollection<SecurityKey>`. Mengembalikan daftar SecurityKey yang
    // masih valid (belum retire) untuk validasi token; opsional filter by kid. Masukan: Parameter `keyId` bertipe `string?` membawa nilai kunci
    // identitas; nilai null diizinkan ketika data opsional belum tersedia.
    public IReadOnlyCollection<SecurityKey> ResolveValidationKeys(string? keyId)
    // Membuka scope metode ResolveValidationKeys; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveValidationKeys.
    {
        // Menyiapkan variabel lokal `now` untuk nilai now dengan `DateTimeOffset.UtcNow`, yaitu waktu UTC saat operasi dilakukan. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var now = DateTimeOffset.UtcNow;
        // Menyiapkan variabel lokal `options` untuk kumpulan pengaturan yang mengendalikan perilaku komponen dengan `_options.Value`, yaitu nilai yang
        // dibungkus objek/nullable. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var options = _options.Value;
        // Menyiapkan variabel lokal `keys` untuk nilai kunci dengan memanggil `ResolveConfiguredKeys` dengan `options`. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var keys = ResolveConfiguredKeys(options);

        // Menyiapkan variabel lokal `candidates` untuk nilai candidates dengan menyaring elemen `keys` dengan predikat `key => !key.RetireAtUtc.HasValue ||
        // key.RetireAtUtc.Value > now.AddMinutes(-(options.AccessTokenMinutes + 5))`; hanya elemen yang memenuhi kondisi diteruskan. Tipe yang dipakai
        // adalah `IEnumerable<ResolvedJwtSigningKey>`.
        IEnumerable<ResolvedJwtSigningKey> candidates = keys.Where(key =>
            // Meneruskan fungsi lambda `key => !key.RetireAtUtc.HasValue || key.RetireAtUtc.Value > now.AddMinutes(-(options.AccessTokenMinutes + 5))` yang
            // dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen ke `keys.Where`.
            !key.RetireAtUtc.HasValue ||
            // Meneruskan `-(options.AccessTokenMinutes + 5)` sebagai argumen ke `now.AddMinutes`.
            key.RetireAtUtc.Value > now.AddMinutes(-(options.AccessTokenMinutes + 5)));

        // Memeriksa kebalikan kondisi `string.IsNullOrWhiteSpace(keyId)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // ResolveValidationKeys.
        if (!string.IsNullOrWhiteSpace(keyId))
        // Membuka scope cabang if untuk kondisi `!string.IsNullOrWhiteSpace(keyId)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ResolveValidationKeys.
        {
            // Memperbarui `candidates` menggunakan menyaring elemen `candidates` dengan predikat `key => string.Equals(key.KeyId, keyId.Trim(),
            // StringComparison.Ordinal)`; hanya elemen yang memenuhi kondisi diteruskan dalam ResolveValidationKeys.
            candidates = candidates.Where(key =>
                // Meneruskan `key.KeyId` (nilai kunci identitas) sebagai argumen ke `string.Equals`; Meneruskan membersihkan karakter tepi pada `keyId` memakai
                // tanpa argumen sebagai argumen ke `string.Equals`; Meneruskan `StringComparison.Ordinal` (nilai ordinal) sebagai argumen ke `string.Equals`.
                string.Equals(key.KeyId, keyId.Trim(), StringComparison.Ordinal));
        // Menutup scope cabang if untuk kondisi `!string.IsNullOrWhiteSpace(keyId)`; bagian berikut berada di luar batas blok tersebut dalam
        // ResolveValidationKeys.
        }

        // Mengembalikan mematerialisasi urutan `candidates .Select(key => BuildSymmetricSecurityKey(key.KeyId, key.SigningKey)) .Cast<SecurityKey>()`
        // menjadi array dengan elemen hasil saat ini kepada pemanggil dalam ResolveValidationKeys; eksekusi jalur ini selesai setelah nilai hasil
        // ditentukan.
        return candidates
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Select(key => BuildSymmetricSecurityKey(key.KeyId, key.SigningKey)) dalam
            // ResolveValidationKeys; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Select(key => BuildSymmetricSecurityKey(key.KeyId, key.SigningKey))
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Cast<SecurityKey>() dalam ResolveValidationKeys; token pada baris ini
            // menyambungkan bagian kode sebelum dan sesudahnya.
            .Cast<SecurityKey>()
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToArray(); dalam ResolveValidationKeys; token pada baris ini menyambungkan
            // bagian kode sebelum dan sesudahnya.
            .ToArray();
    // Menutup scope metode ResolveValidationKeys; bagian berikut berada di luar batas blok tersebut dalam ResolveValidationKeys.
    }

    /// <summary>
    /// Memvalidasi bahwa minimal satu key aktif dan daftar validasi tersedia.
    /// </summary>
    // Mendefinisikan metode `ValidateConfiguration` dengan hasil bertipe `void`. Memvalidasi bahwa minimal satu key aktif dan daftar validasi tersedia.
    public void ValidateConfiguration()
    // Membuka scope metode ValidateConfiguration; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateConfiguration.
    {
        // Memperbarui `_` menggunakan memanggil `GetActiveSigningMaterial` dengan tanpa argumen dalam ValidateConfiguration.
        _ = GetActiveSigningMaterial();
        // Memperbarui `_` menggunakan memanggil `ResolveValidationKeys` dengan `null` dalam ValidateConfiguration.
        _ = ResolveValidationKeys(null);
    // Menutup scope metode ValidateConfiguration; bagian berikut berada di luar batas blok tersebut dalam ValidateConfiguration.
    }

    /// <summary>
    /// Memeriksa apakah key aktif pada waktu tertentu (sudah diaktifkan dan belum di-retire).
    /// </summary>
    // Mendefinisikan metode `IsActiveAt` dengan hasil bertipe `bool`. Memeriksa apakah key aktif pada waktu tertentu (sudah diaktifkan dan belum
    // di-retire). Masukan: Parameter `key` bertipe `ResolvedJwtSigningKey` membawa nilai kunci; Parameter `now` bertipe `DateTimeOffset` membawa nilai
    // now.
    private static bool IsActiveAt(ResolvedJwtSigningKey key, DateTimeOffset now)
    // Membuka scope metode IsActiveAt; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam IsActiveAt.
    {
        // Menyiapkan variabel lokal `activateAt` untuk nilai activate at dengan `key.ActivateAtUtc` bila tidak null; jika null gunakan
        // `DateTimeOffset.MinValue` sebagai nilai pengganti. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var activateAt = key.ActivateAtUtc ?? DateTimeOffset.MinValue;
        // Menyiapkan variabel lokal `notRetired` untuk nilai not retired dengan gabungan syarat OR: setidaknya satu kondisi wajib benar antara
        // `!key.RetireAtUtc.HasValue` dan `key.RetireAtUtc.Value > now`; sisi kanan diperiksa hanya jika sisi kiri salah. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var notRetired = !key.RetireAtUtc.HasValue || key.RetireAtUtc.Value > now;
        // Mengembalikan gabungan syarat AND: kedua kondisi wajib benar antara `activateAt <= now` dan `notRetired`; sisi kanan diperiksa hanya jika sisi
        // kiri benar kepada pemanggil dalam IsActiveAt; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return activateAt <= now && notRetired;
    // Menutup scope metode IsActiveAt; bagian berikut berada di luar batas blok tersebut dalam IsActiveAt.
    }

    /// <summary>
    /// Membuat SymmetricSecurityKey dari string signing key dengan kid terlampir.
    /// </summary>
    // Mendefinisikan metode `BuildSymmetricSecurityKey` dengan hasil bertipe `SymmetricSecurityKey`. Membuat SymmetricSecurityKey dari string signing
    // key dengan kid terlampir. Masukan: Parameter `keyId` bertipe `string` membawa nilai kunci identitas; Parameter `signingKey` bertipe `string`
    // membawa kunci untuk membentuk atau memverifikasi tanda tangan token JWT.
    private static SymmetricSecurityKey BuildSymmetricSecurityKey(string keyId, string signingKey)
    // Membuka scope metode BuildSymmetricSecurityKey; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildSymmetricSecurityKey.
    {
        // Menyiapkan variabel lokal `key` untuk nilai kunci dengan objek baru bertipe `SymmetricSecurityKey` dengan argumen
        // (Encoding.UTF8.GetBytes(signingKey)). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey))
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // BuildSymmetricSecurityKey.
        {
            // Memperbarui `KeyId` menggunakan `keyId` (nilai kunci identitas) dalam BuildSymmetricSecurityKey.
            KeyId = keyId
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam BuildSymmetricSecurityKey.
        };
        // Mengembalikan `key` (nilai kunci) kepada pemanggil dalam BuildSymmetricSecurityKey; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return key;
    // Menutup scope metode BuildSymmetricSecurityKey; bagian berikut berada di luar batas blok tersebut dalam BuildSymmetricSecurityKey.
    }

    /// <summary>
    /// Mengumpulkan seluruh key dari berbagai sumber (config, JSON string, file, env), lalu memvalidasi dan mengurutkannya.
    /// </summary>
    // Mendefinisikan metode `ResolveConfiguredKeys` dengan hasil bertipe `List<ResolvedJwtSigningKey>`. Mengumpulkan seluruh key dari berbagai sumber
    // (config, JSON string, file, env), lalu memvalidasi dan mengurutkannya. Masukan: Parameter `options` bertipe `JwtOptions` membawa kumpulan
    // pengaturan yang mengendalikan perilaku komponen.
    private static List<ResolvedJwtSigningKey> ResolveConfiguredKeys(JwtOptions options)
    // Membuka scope metode ResolveConfiguredKeys; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveConfiguredKeys.
    {
        // Menyiapkan variabel lokal `configuredKeys` untuk nilai configured kunci dengan objek baru bertipe `List<JwtSigningKeyOptions>` dengan nilai awal
        // sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var configuredKeys = new List<JwtSigningKeyOptions>();

        // Memeriksa pemeriksaan lebih besar antara `options.SigningKeys.Count` dan `0`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // ResolveConfiguredKeys.
        if (options.SigningKeys.Count > 0)
        // Membuka scope cabang if untuk kondisi `options.SigningKeys.Count > 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ResolveConfiguredKeys.
        {
            // Menjalankan menambahkan seluruh elemen `options.SigningKeys` ke `configuredKeys` dalam ResolveConfiguredKeys.
            configuredKeys.AddRange(options.SigningKeys);
        // Menutup scope cabang if untuk kondisi `options.SigningKeys.Count > 0`; bagian berikut berada di luar batas blok tersebut dalam
        // ResolveConfiguredKeys.
        }

        // Menjalankan memanggil `AppendKeysFromJson` dengan `configuredKeys`, `options.SigningKeysJson` dalam ResolveConfiguredKeys.
        AppendKeysFromJson(configuredKeys, options.SigningKeysJson);
        // Menjalankan memanggil `AppendKeysFromJson` dengan `configuredKeys`, `ReadSecretFile(options.SigningKeysFile)` dalam ResolveConfiguredKeys.
        AppendKeysFromJson(configuredKeys, ReadSecretFile(options.SigningKeysFile));
        // Menjalankan memanggil `AppendKeysFromJson` dengan `configuredKeys`, `ReadEnvironmentVariable(options.SigningKeysJsonEnvironmentVariable,
        // ”JWT_SIGNING_KEYS_JSON”)` dalam ResolveConfiguredKeys.
        AppendKeysFromJson(
            // Meneruskan `configuredKeys` (nilai configured kunci) sebagai argumen ke `AppendKeysFromJson`.
            configuredKeys,
            // Meneruskan memanggil `ReadEnvironmentVariable` dengan `options.SigningKeysJsonEnvironmentVariable`, `”JWT_SIGNING_KEYS_JSON”` sebagai argumen ke
            // `AppendKeysFromJson`; Meneruskan `options.SigningKeysJsonEnvironmentVariable` (nilai signing kunci JSON environment variable) sebagai argumen ke
            // `ReadEnvironmentVariable`; Meneruskan nilai literal `”JWT_SIGNING_KEYS_JSON”` sebagai argumen ke `ReadEnvironmentVariable`.
            ReadEnvironmentVariable(options.SigningKeysJsonEnvironmentVariable, "JWT_SIGNING_KEYS_JSON"));

        // Memeriksa perbandingan kesamaan antara `configuredKeys.Count` dan `0`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // ResolveConfiguredKeys.
        if (configuredKeys.Count == 0)
        // Membuka scope cabang if untuk kondisi `configuredKeys.Count == 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ResolveConfiguredKeys.
        {
            // Menyiapkan variabel lokal `fallbackSigningKey` untuk nilai fallback signing kunci dengan memanggil `ResolveFallbackSigningKey` dengan `options`.
            // Tipe variabel disimpulkan dari ekspresi nilai awal.
            var fallbackSigningKey = ResolveFallbackSigningKey(options);
            // Memeriksa memeriksa apakah `fallbackSigningKey` null, kosong, atau hanya berisi karakter spasi; blok if hanya dijalankan ketika kondisi ini
            // bernilai benar dalam ResolveConfiguredKeys.
            if (string.IsNullOrWhiteSpace(fallbackSigningKey))
            // Membuka scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(fallbackSigningKey)`; pernyataan/deklarasi berikut berada di dalam batas blok
            // ini dalam ResolveConfiguredKeys.
            {
                // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen ( ”JWT signing key belum dikonfigurasi. Gunakan
                // Jwt:SigningKeys/Jwt:SigningKey atau secret file/env.”) dalam ResolveConfiguredKeys; pemanggil atau middleware penanganan error menerima kegagalan
                // ini.
                throw new InvalidOperationException(
                    // Meneruskan nilai literal `”JWT signing key belum dikonfigurasi. Gunakan Jwt:SigningKeys/Jwt:SigningKey atau secret file/env.”` sebagai argumen ke
                    // konstruktor `InvalidOperationException`.
                    "JWT signing key belum dikonfigurasi. Gunakan Jwt:SigningKeys/Jwt:SigningKey atau secret file/env.");
            // Menutup scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(fallbackSigningKey)`; bagian berikut berada di luar batas blok tersebut dalam
            // ResolveConfiguredKeys.
            }

            // Menjalankan menambahkan `new JwtSigningKeyOptions { KeyId = string.IsNullOrWhiteSpace(options.ActiveKeyId) ? ”default” :
            // options.ActiveKeyId.Trim(), SigningKey = fallbackSigningKey, ActivateAtUtc = Da...` ke `configuredKeys` dalam ResolveConfiguredKeys.
            configuredKeys.Add(new JwtSigningKeyOptions
            // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ResolveConfiguredKeys.
            {
                // Memperbarui `KeyId` menggunakan hasil pemilihan bersyarat: ketika `string.IsNullOrWhiteSpace(options.ActiveKeyId)` benar gunakan `”default”`,
                // jika tidak gunakan `options.ActiveKeyId.Trim()` dalam ResolveConfiguredKeys.
                KeyId = string.IsNullOrWhiteSpace(options.ActiveKeyId) ? "default" : options.ActiveKeyId.Trim(),
                // Memperbarui `SigningKey` menggunakan `fallbackSigningKey` (nilai fallback signing kunci) dalam ResolveConfiguredKeys.
                SigningKey = fallbackSigningKey,
                // Memperbarui `ActivateAtUtc` menggunakan `DateTimeOffset.MinValue` (nilai minimum nilai) dalam ResolveConfiguredKeys.
                ActivateAtUtc = DateTimeOffset.MinValue
            // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam ResolveConfiguredKeys.
            });
        // Menutup scope cabang if untuk kondisi `configuredKeys.Count == 0`; bagian berikut berada di luar batas blok tersebut dalam ResolveConfiguredKeys.
        }

        // Menyiapkan variabel lokal `keyIds` untuk nilai kunci identitas dengan objek baru bertipe `HashSet<string>` dengan argumen
        // (StringComparer.Ordinal). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var keyIds = new HashSet<string>(StringComparer.Ordinal);
        // Menyiapkan variabel lokal `resolved` untuk nilai hasil resolusi dengan objek baru bertipe `List<ResolvedJwtSigningKey>` dengan nilai awal sesuai
        // konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var resolved = new List<ResolvedJwtSigningKey>();

        // Memulai loop dengan inisialisasi `var i = 0`, berjalan selama `i < configuredKeys.Count`, lalu memperbarui pencacah melalui `i++` dalam
        // ResolveConfiguredKeys.
        for (var i = 0; i < configuredKeys.Count; i++)
        // Membuka scope loop dengan syarat `i < configuredKeys.Count`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ResolveConfiguredKeys.
        {
            // Menyiapkan variabel lokal `item` untuk nilai elemen dengan `configuredKeys[i]`, yaitu elemen koleksi yang dipilih melalui indeks atau kunci
            // tersebut. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var item = configuredKeys[i];
            // Menyiapkan variabel lokal `keyId` untuk nilai kunci identitas dengan hasil pemilihan bersyarat: ketika `string.IsNullOrWhiteSpace(item.KeyId)`
            // benar gunakan `$”key-{i + 1:00}”`, jika tidak gunakan `item.KeyId.Trim()`. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var keyId = string.IsNullOrWhiteSpace(item.KeyId)
                // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: $”key-{i + 1:00}” dalam ResolveConfiguredKeys.
                ? $"key-{i + 1:00}"
                // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: item.KeyId.Trim(); dalam ResolveConfiguredKeys.
                : item.KeyId.Trim();
            // Memeriksa kebalikan kondisi `keyIds.Add(keyId)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ResolveConfiguredKeys.
            if (!keyIds.Add(keyId))
            // Membuka scope cabang if untuk kondisi `!keyIds.Add(keyId)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ResolveConfiguredKeys.
            {
                // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen ($”Duplikasi Jwt key id terdeteksi: '{keyId}'.”)
                // dalam ResolveConfiguredKeys; pemanggil atau middleware penanganan error menerima kegagalan ini.
                throw new InvalidOperationException($"Duplikasi Jwt key id terdeteksi: '{keyId}'.");
            // Menutup scope cabang if untuk kondisi `!keyIds.Add(keyId)`; bagian berikut berada di luar batas blok tersebut dalam ResolveConfiguredKeys.
            }

            // Menyiapkan variabel lokal `signingKey` untuk kunci untuk membentuk atau memverifikasi tanda tangan token JWT dengan memanggil `ResolveSigningKey`
            // dengan `item`. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var signingKey = ResolveSigningKey(item);
            // Memeriksa memeriksa apakah `signingKey` null, kosong, atau hanya berisi karakter spasi; blok if hanya dijalankan ketika kondisi ini bernilai
            // benar dalam ResolveConfiguredKeys.
            if (string.IsNullOrWhiteSpace(signingKey))
            // Membuka scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(signingKey)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ResolveConfiguredKeys.
            {
                // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen ($”Signing key untuk kid '{keyId}' belum
                // dikonfigurasi.”) dalam ResolveConfiguredKeys; pemanggil atau middleware penanganan error menerima kegagalan ini.
                throw new InvalidOperationException($"Signing key untuk kid '{keyId}' belum dikonfigurasi.");
            // Menutup scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(signingKey)`; bagian berikut berada di luar batas blok tersebut dalam
            // ResolveConfiguredKeys.
            }

            // Memeriksa pemeriksaan lebih kecil antara `signingKey.Length` dan `MinSigningKeyLength`; blok if hanya dijalankan ketika kondisi ini bernilai
            // benar dalam ResolveConfiguredKeys.
            if (signingKey.Length < MinSigningKeyLength)
            // Membuka scope cabang if untuk kondisi `signingKey.Length < MinSigningKeyLength`; pernyataan/deklarasi berikut berada di dalam batas blok ini
            // dalam ResolveConfiguredKeys.
            {
                // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen ( $”Signing key untuk kid '{keyId}' minimal
                // {MinSigningKeyLength} karakter.”) dalam ResolveConfiguredKeys; pemanggil atau middleware penanganan error menerima kegagalan ini.
                throw new InvalidOperationException(
                    // Meneruskan teks interpolasi `$”Signing key untuk kid '{keyId}' minimal {MinSigningKeyLength} karakter.”`; nilai ekspresi di dalam kurung kurawal
                    // disisipkan saat program berjalan sebagai argumen ke konstruktor `InvalidOperationException`.
                    $"Signing key untuk kid '{keyId}' minimal {MinSigningKeyLength} karakter.");
            // Menutup scope cabang if untuk kondisi `signingKey.Length < MinSigningKeyLength`; bagian berikut berada di luar batas blok tersebut dalam
            // ResolveConfiguredKeys.
            }

            // Memeriksa membandingkan kesamaan `string` dengan `signingKey`, `”change-this-jwt-signing-key-for-production-2026”`, `StringComparison.Ordinal`;
            // aturan perbandingan mengikuti overload dan comparer yang diberikan; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
            // ResolveConfiguredKeys.
            if (string.Equals(signingKey, "change-this-jwt-signing-key-for-production-2026", StringComparison.Ordinal))
            // Membuka scope cabang if untuk kondisi `string.Equals(signingKey, ”change-this-jwt-signing-key-for-production-2026”, StringComparison.Ordinal)`;
            // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveConfiguredKeys.
            {
                // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen ( $”Signing key untuk kid '{keyId}' masih
                // placeholder. Gunakan secret yang kuat.”) dalam ResolveConfiguredKeys; pemanggil atau middleware penanganan error menerima kegagalan ini.
                throw new InvalidOperationException(
                    // Meneruskan teks interpolasi `$”Signing key untuk kid '{keyId}' masih placeholder. Gunakan secret yang kuat.”`; nilai ekspresi di dalam kurung
                    // kurawal disisipkan saat program berjalan sebagai argumen ke konstruktor `InvalidOperationException`.
                    $"Signing key untuk kid '{keyId}' masih placeholder. Gunakan secret yang kuat.");
            // Menutup scope cabang if untuk kondisi `string.Equals(signingKey, ”change-this-jwt-signing-key-for-production-2026”, StringComparison.Ordinal)`;
            // bagian berikut berada di luar batas blok tersebut dalam ResolveConfiguredKeys.
            }

            // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `item.RetireAtUtc.HasValue && item.ActivateAtUtc.HasValue` dan `item.RetireAtUtc
            // <= item.ActivateAtUtc`; sisi kanan diperiksa hanya jika sisi kiri benar; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
            // ResolveConfiguredKeys.
            if (item.RetireAtUtc.HasValue && item.ActivateAtUtc.HasValue && item.RetireAtUtc <= item.ActivateAtUtc)
            // Membuka scope cabang if untuk kondisi `item.RetireAtUtc.HasValue && item.ActivateAtUtc.HasValue && item.RetireAtUtc <= item.ActivateAtUtc`;
            // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveConfiguredKeys.
            {
                // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen ( $”Rentang aktif key '{keyId}' tidak valid:
                // RetireAtUtc harus lebih besar dari ActivateAtUtc.”) dalam ResolveConfiguredKeys; pemanggil atau middleware penanganan error menerima kegagalan
                // ini.
                throw new InvalidOperationException(
                    // Meneruskan teks interpolasi `$”Rentang aktif key '{keyId}' tidak valid: RetireAtUtc harus lebih besar dari ActivateAtUtc.”`; nilai ekspresi di
                    // dalam kurung kurawal disisipkan saat program berjalan sebagai argumen ke konstruktor `InvalidOperationException`.
                    $"Rentang aktif key '{keyId}' tidak valid: RetireAtUtc harus lebih besar dari ActivateAtUtc.");
            // Menutup scope cabang if untuk kondisi `item.RetireAtUtc.HasValue && item.ActivateAtUtc.HasValue && item.RetireAtUtc <= item.ActivateAtUtc`;
            // bagian berikut berada di luar batas blok tersebut dalam ResolveConfiguredKeys.
            }

            // Menjalankan menambahkan `new ResolvedJwtSigningKey( keyId, signingKey, item.ActivateAtUtc, item.RetireAtUtc)` ke `resolved` dalam
            // ResolveConfiguredKeys.
            resolved.Add(new ResolvedJwtSigningKey(
                // Meneruskan `keyId` (nilai kunci identitas) sebagai argumen ke konstruktor `ResolvedJwtSigningKey`.
                keyId,
                // Meneruskan `signingKey` (kunci untuk membentuk atau memverifikasi tanda tangan token JWT) sebagai argumen ke konstruktor `ResolvedJwtSigningKey`.
                signingKey,
                // Meneruskan `item.ActivateAtUtc` (nilai activate at utc) sebagai argumen ke konstruktor `ResolvedJwtSigningKey`.
                item.ActivateAtUtc,
                // Meneruskan `item.RetireAtUtc` (nilai retire at utc) sebagai argumen ke konstruktor `ResolvedJwtSigningKey`.
                item.RetireAtUtc));
        // Menutup scope loop dengan syarat `i < configuredKeys.Count`; bagian berikut berada di luar batas blok tersebut dalam ResolveConfiguredKeys.
        }

        // Mengembalikan mematerialisasi urutan `resolved .OrderBy(key => key.ActivateAtUtc ?? DateTimeOffset.MinValue) .ThenBy(key => key.KeyId,
        // StringComparer.Ordinal)` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori kepada pemanggil dalam ResolveConfiguredKeys;
        // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return resolved
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .OrderBy(key => key.ActivateAtUtc ?? DateTimeOffset.MinValue) dalam
            // ResolveConfiguredKeys; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .OrderBy(key => key.ActivateAtUtc ?? DateTimeOffset.MinValue)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ThenBy(key => key.KeyId, StringComparer.Ordinal) dalam ResolveConfiguredKeys;
            // token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .ThenBy(key => key.KeyId, StringComparer.Ordinal)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList(); dalam ResolveConfiguredKeys; token pada baris ini menyambungkan
            // bagian kode sebelum dan sesudahnya.
            .ToList();
    // Menutup scope metode ResolveConfiguredKeys; bagian berikut berada di luar batas blok tersebut dalam ResolveConfiguredKeys.
    }

    /// <summary>
    /// Memparsing JSON array berisi konfigurasi key dan menambahkannya ke daftar target.
    /// </summary>
    // Mendefinisikan metode `AppendKeysFromJson` dengan hasil bertipe `void`. Memparsing JSON array berisi konfigurasi key dan menambahkannya ke daftar
    // target. Masukan: Parameter `target` bertipe `List<JwtSigningKeyOptions>` membawa nilai target; Parameter `json` bertipe `string` membawa nilai
    // JSON.
    private static void AppendKeysFromJson(List<JwtSigningKeyOptions> target, string json)
    // Membuka scope metode AppendKeysFromJson; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam AppendKeysFromJson.
    {
        // Memeriksa memeriksa apakah `json` null, kosong, atau hanya berisi karakter spasi; blok if hanya dijalankan ketika kondisi ini bernilai benar
        // dalam AppendKeysFromJson.
        if (string.IsNullOrWhiteSpace(json))
        // Membuka scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(json)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // AppendKeysFromJson.
        {
            // Mengakhiri eksekusi lebih awal dalam AppendKeysFromJson tanpa mengembalikan nilai; pernyataan sesudah return pada jalur ini tidak dijalankan.
            return;
        // Menutup scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(json)`; bagian berikut berada di luar batas blok tersebut dalam
        // AppendKeysFromJson.
        }

        // Memulai blok try dalam AppendKeysFromJson; exception dari blok ini dapat dialihkan ke catch, sedangkan finally (jika ada) tetap dijalankan saat
        // keluar.
        try
        // Membuka scope penanganan operasi try; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam AppendKeysFromJson.
        {
            // Menyiapkan variabel lokal `parsed` untuk nilai parsed dengan membaca `json`, `new JsonSerializerOptions(JsonSerializerDefaults.Web)` menjadi
            // objek bertipe sesuai kontrak JSON melalui `JsonSerializer.Deserialize<List<JwtSigningKeyOptions>>`. Tipe variabel disimpulkan dari ekspresi nilai
            // awal.
            var parsed = JsonSerializer.Deserialize<List<JwtSigningKeyOptions>>(
                // Meneruskan `json` (nilai JSON) sebagai argumen ke `JsonSerializer.Deserialize<List<JwtSigningKeyOptions>>`.
                json,
                // Meneruskan objek baru bertipe `JsonSerializerOptions` dengan argumen (JsonSerializerDefaults.Web) sebagai argumen ke
                // `JsonSerializer.Deserialize<List<JwtSigningKeyOptions>>`; Meneruskan `JsonSerializerDefaults.Web` (nilai web) sebagai argumen ke konstruktor
                // `JsonSerializerOptions`.
                new JsonSerializerOptions(JsonSerializerDefaults.Web));
            // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `parsed is null` dan `parsed.Count == 0`; sisi kanan diperiksa hanya
            // jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam AppendKeysFromJson.
            if (parsed is null || parsed.Count == 0)
            // Membuka scope cabang if untuk kondisi `parsed is null || parsed.Count == 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // AppendKeysFromJson.
            {
                // Mengakhiri eksekusi lebih awal dalam AppendKeysFromJson tanpa mengembalikan nilai; pernyataan sesudah return pada jalur ini tidak dijalankan.
                return;
            // Menutup scope cabang if untuk kondisi `parsed is null || parsed.Count == 0`; bagian berikut berada di luar batas blok tersebut dalam
            // AppendKeysFromJson.
            }

            // Menjalankan menambahkan seluruh elemen `parsed` ke `target` dalam AppendKeysFromJson.
            target.AddRange(parsed);
        // Menutup scope penanganan operasi try; bagian berikut berada di luar batas blok tersebut dalam AppendKeysFromJson.
        }
        // Menangani exception `JsonException` melalui variabel ex dalam AppendKeysFromJson.
        catch (JsonException ex)
        // Membuka scope penanganan exception catch; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam AppendKeysFromJson.
        {
            // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen (”Format JSON untuk konfigurasi JWT signing keys
            // tidak valid.”, ex) dalam AppendKeysFromJson; pemanggil atau middleware penanganan error menerima kegagalan ini.
            throw new InvalidOperationException("Format JSON untuk konfigurasi JWT signing keys tidak valid.", ex);
        // Menutup scope penanganan exception catch; bagian berikut berada di luar batas blok tersebut dalam AppendKeysFromJson.
        }
    // Menutup scope metode AppendKeysFromJson; bagian berikut berada di luar batas blok tersebut dalam AppendKeysFromJson.
    }

    /// <summary>
    /// Mengambil signing key fallback dari opsi langsung, file, atau environment variable.
    /// </summary>
    // Mendefinisikan metode `ResolveFallbackSigningKey` dengan hasil bertipe `string`. Mengambil signing key fallback dari opsi langsung, file, atau
    // environment variable. Masukan: Parameter `options` bertipe `JwtOptions` membawa kumpulan pengaturan yang mengendalikan perilaku komponen.
    private static string ResolveFallbackSigningKey(JwtOptions options)
    // Membuka scope metode ResolveFallbackSigningKey; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveFallbackSigningKey.
    {
        // Memeriksa kebalikan kondisi `string.IsNullOrWhiteSpace(options.SigningKey)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // ResolveFallbackSigningKey.
        if (!string.IsNullOrWhiteSpace(options.SigningKey))
        // Membuka scope cabang if untuk kondisi `!string.IsNullOrWhiteSpace(options.SigningKey)`; pernyataan/deklarasi berikut berada di dalam batas blok
        // ini dalam ResolveFallbackSigningKey.
        {
            // Mengembalikan membersihkan karakter tepi pada `options.SigningKey` memakai tanpa argumen kepada pemanggil dalam ResolveFallbackSigningKey;
            // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return options.SigningKey.Trim();
        // Menutup scope cabang if untuk kondisi `!string.IsNullOrWhiteSpace(options.SigningKey)`; bagian berikut berada di luar batas blok tersebut dalam
        // ResolveFallbackSigningKey.
        }

        // Menyiapkan variabel lokal `fileValue` untuk nilai file nilai dengan memanggil `ReadSecretFile` dengan `options.SigningKeyFile`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var fileValue = ReadSecretFile(options.SigningKeyFile);
        // Memeriksa kebalikan kondisi `string.IsNullOrWhiteSpace(fileValue)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // ResolveFallbackSigningKey.
        if (!string.IsNullOrWhiteSpace(fileValue))
        // Membuka scope cabang if untuk kondisi `!string.IsNullOrWhiteSpace(fileValue)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ResolveFallbackSigningKey.
        {
            // Mengembalikan `fileValue` (nilai file nilai) kepada pemanggil dalam ResolveFallbackSigningKey; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return fileValue;
        // Menutup scope cabang if untuk kondisi `!string.IsNullOrWhiteSpace(fileValue)`; bagian berikut berada di luar batas blok tersebut dalam
        // ResolveFallbackSigningKey.
        }

        // Mengembalikan memanggil `ReadEnvironmentVariable` dengan `options.SigningKeyEnvironmentVariable`, `”JWT_SIGNING_KEY”` kepada pemanggil dalam
        // ResolveFallbackSigningKey; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return ReadEnvironmentVariable(options.SigningKeyEnvironmentVariable, "JWT_SIGNING_KEY");
    // Menutup scope metode ResolveFallbackSigningKey; bagian berikut berada di luar batas blok tersebut dalam ResolveFallbackSigningKey.
    }

    /// <summary>
    /// Mengambil signing key dari opsi inline atau dari file secret.
    /// </summary>
    // Mendefinisikan metode `ResolveSigningKey` dengan hasil bertipe `string`. Mengambil signing key dari opsi inline atau dari file secret. Masukan:
    // Parameter `options` bertipe `JwtSigningKeyOptions` membawa kumpulan pengaturan yang mengendalikan perilaku komponen.
    private static string ResolveSigningKey(JwtSigningKeyOptions options)
    // Membuka scope metode ResolveSigningKey; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveSigningKey.
    {
        // Memeriksa kebalikan kondisi `string.IsNullOrWhiteSpace(options.SigningKey)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // ResolveSigningKey.
        if (!string.IsNullOrWhiteSpace(options.SigningKey))
        // Membuka scope cabang if untuk kondisi `!string.IsNullOrWhiteSpace(options.SigningKey)`; pernyataan/deklarasi berikut berada di dalam batas blok
        // ini dalam ResolveSigningKey.
        {
            // Mengembalikan membersihkan karakter tepi pada `options.SigningKey` memakai tanpa argumen kepada pemanggil dalam ResolveSigningKey; eksekusi jalur
            // ini selesai setelah nilai hasil ditentukan.
            return options.SigningKey.Trim();
        // Menutup scope cabang if untuk kondisi `!string.IsNullOrWhiteSpace(options.SigningKey)`; bagian berikut berada di luar batas blok tersebut dalam
        // ResolveSigningKey.
        }

        // Mengembalikan memanggil `ReadSecretFile` dengan `options.SigningKeyFile` kepada pemanggil dalam ResolveSigningKey; eksekusi jalur ini selesai
        // setelah nilai hasil ditentukan.
        return ReadSecretFile(options.SigningKeyFile);
    // Menutup scope metode ResolveSigningKey; bagian berikut berada di luar batas blok tersebut dalam ResolveSigningKey.
    }

    /// <summary>
    /// Membaca nilai environment variable; jika nama konfigurasi kosong, gunakan fallback.
    /// </summary>
    // Mendefinisikan metode `ReadEnvironmentVariable` dengan hasil bertipe `string`. Membaca nilai environment variable; jika nama konfigurasi kosong,
    // gunakan fallback. Masukan: Parameter `configuredName` bertipe `string` membawa nilai configured nama; Parameter `fallbackName` bertipe `string`
    // membawa nilai fallback nama.
    private static string ReadEnvironmentVariable(string configuredName, string fallbackName)
    // Membuka scope metode ReadEnvironmentVariable; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ReadEnvironmentVariable.
    {
        // Menyiapkan variabel lokal `envName` untuk nilai env nama dengan hasil pemilihan bersyarat: ketika `string.IsNullOrWhiteSpace(configuredName)`
        // benar gunakan `fallbackName`, jika tidak gunakan `configuredName.Trim()`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var envName = string.IsNullOrWhiteSpace(configuredName) ? fallbackName : configuredName.Trim();
        // Mengembalikan membersihkan karakter tepi pada `(Environment.GetEnvironmentVariable(envName) ?? string.Empty)` memakai tanpa argumen kepada
        // pemanggil dalam ReadEnvironmentVariable; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return (Environment.GetEnvironmentVariable(envName) ?? string.Empty).Trim();
    // Menutup scope metode ReadEnvironmentVariable; bagian berikut berada di luar batas blok tersebut dalam ReadEnvironmentVariable.
    }

    /// <summary>
    /// Membaca isi file secret dari path yang diberikan; mengembalikan string kosong jika file tidak ada.
    /// </summary>
    // Mendefinisikan metode `ReadSecretFile` dengan hasil bertipe `string`. Membaca isi file secret dari path yang diberikan; mengembalikan string
    // kosong jika file tidak ada. Masukan: Parameter `path` bertipe `string` membawa nilai path.
    private static string ReadSecretFile(string path)
    // Membuka scope metode ReadSecretFile; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ReadSecretFile.
    {
        // Memeriksa memeriksa apakah `path` null, kosong, atau hanya berisi karakter spasi; blok if hanya dijalankan ketika kondisi ini bernilai benar
        // dalam ReadSecretFile.
        if (string.IsNullOrWhiteSpace(path))
        // Membuka scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(path)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ReadSecretFile.
        {
            // Mengembalikan `string.Empty`, yaitu nilai kosong bawaan tipe terkait kepada pemanggil dalam ReadSecretFile; eksekusi jalur ini selesai setelah
            // nilai hasil ditentukan.
            return string.Empty;
        // Menutup scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(path)`; bagian berikut berada di luar batas blok tersebut dalam ReadSecretFile.
        }

        // Menyiapkan variabel lokal `normalizedPath` untuk nilai normalized path dengan membersihkan karakter tepi pada `path` memakai tanpa argumen. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var normalizedPath = path.Trim();
        // Memeriksa kebalikan kondisi `File.Exists(normalizedPath)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ReadSecretFile.
        if (!File.Exists(normalizedPath))
        // Membuka scope cabang if untuk kondisi `!File.Exists(normalizedPath)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ReadSecretFile.
        {
            // Mengembalikan `string.Empty`, yaitu nilai kosong bawaan tipe terkait kepada pemanggil dalam ReadSecretFile; eksekusi jalur ini selesai setelah
            // nilai hasil ditentukan.
            return string.Empty;
        // Menutup scope cabang if untuk kondisi `!File.Exists(normalizedPath)`; bagian berikut berada di luar batas blok tersebut dalam ReadSecretFile.
        }

        // Mengembalikan membersihkan karakter tepi pada `File.ReadAllText(normalizedPath)` memakai tanpa argumen kepada pemanggil dalam ReadSecretFile;
        // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return File.ReadAllText(normalizedPath).Trim();
    // Menutup scope metode ReadSecretFile; bagian berikut berada di luar batas blok tersebut dalam ReadSecretFile.
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
// Menutup scope tipe JwtSigningKeyProvider; bagian berikut berada di luar batas blok tersebut.
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
