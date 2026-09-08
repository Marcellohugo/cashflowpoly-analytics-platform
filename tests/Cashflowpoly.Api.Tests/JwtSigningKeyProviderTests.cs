// Fungsi file: Memverifikasi perilaku API, database, atau domain melalui JwtSigningKeyProviderTests.
// Mengimpor namespace `Cashflowpoly.Api.Security` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Security;
// Mengimpor namespace `Microsoft.Extensions.Logging.Abstractions` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama
// lengkapnya.
using Microsoft.Extensions.Logging.Abstractions;
// Mengimpor namespace `Microsoft.Extensions.Options` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.Extensions.Options;
// Mengimpor namespace `Xunit` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Xunit;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Tests` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Tests;

/// <summary>
/// Kelas pengujian unit untuk memvalidasi bahwa JwtSigningKeyProvider
/// mengelola seleksi signing key aktif dan resolusi validation key dengan benar.
/// </summary>
// Mendefinisikan tipe class `JwtSigningKeyProviderTests`; sealed mencegah tipe ini diturunkan lagi.
public sealed class JwtSigningKeyProviderTests
// Membuka scope tipe JwtSigningKeyProviderTests; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    /// <summary>
    /// Memvalidasi bahwa GetActiveSigningMaterial melempar InvalidOperationException
    /// ketika tidak ada signing key yang dikonfigurasi sama sekali.
    /// </summary>
    // Mendefinisikan metode `GetActiveSigningMaterial_Throws_WhenNoKeyConfigured` dengan hasil bertipe `void`; operasi ini menangani get aktif signing
    // material throws when no kunci configured.
    public void GetActiveSigningMaterial_Throws_WhenNoKeyConfigured()
    // Membuka scope metode GetActiveSigningMaterial_Throws_WhenNoKeyConfigured; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // GetActiveSigningMaterial_Throws_WhenNoKeyConfigured.
    {
        // Menyiapkan variabel lokal `options` untuk kumpulan pengaturan yang mengendalikan perilaku komponen dengan memanggil `Options.Create` dengan `new
        // JwtOptions { SigningKey = string.Empty, SigningKeys = new List<JwtSigningKeyOptions>() }`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var options = Options.Create(new JwtOptions
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // GetActiveSigningMaterial_Throws_WhenNoKeyConfigured.
        {
            // Memperbarui `SigningKey` menggunakan `string.Empty`, yaitu nilai kosong bawaan tipe terkait dalam
            // GetActiveSigningMaterial_Throws_WhenNoKeyConfigured.
            SigningKey = string.Empty,
            // Memperbarui `SigningKeys` menggunakan objek baru bertipe `List<JwtSigningKeyOptions>` dengan nilai awal sesuai konstruktornya dalam
            // GetActiveSigningMaterial_Throws_WhenNoKeyConfigured.
            SigningKeys = new List<JwtSigningKeyOptions>()
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // GetActiveSigningMaterial_Throws_WhenNoKeyConfigured.
        });
        // Menyiapkan variabel lokal `sut` untuk nilai sut dengan objek baru bertipe `JwtSigningKeyProvider` dengan argumen (options,
        // NullLogger<JwtSigningKeyProvider>.Instance). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var sut = new JwtSigningKeyProvider(options, NullLogger<JwtSigningKeyProvider>.Instance);

        // Menyiapkan variabel lokal `ex` untuk nilai ex dengan pemeriksaan bahwa operasi `() => sut.GetActiveSigningMaterial()` melempar jenis exception
        // yang diharapkan; pengujian gagal jika perilaku kesalahan berbeda. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var ex = Assert.Throws<InvalidOperationException>(() => sut.GetActiveSigningMaterial());
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”signing key”`, `ex.Message`,
        // `StringComparison.OrdinalIgnoreCase` dalam GetActiveSigningMaterial_Throws_WhenNoKeyConfigured.
        Assert.Contains("signing key", ex.Message, StringComparison.OrdinalIgnoreCase);
    // Menutup scope metode GetActiveSigningMaterial_Throws_WhenNoKeyConfigured; bagian berikut berada di luar batas blok tersebut dalam
    // GetActiveSigningMaterial_Throws_WhenNoKeyConfigured.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    /// <summary>
    /// Memvalidasi bahwa GetActiveSigningMaterial memilih key dengan tanggal aktivasi
    /// terbaru yang belum di-retire, dan ResolveValidationKeys menyertakan key tersebut.
    /// </summary>
    // Mendefinisikan metode `GetActiveSigningMaterial_PicksLatestActiveKey` dengan hasil bertipe `void`; operasi ini menangani get aktif signing
    // material picks latest aktif kunci.
    public void GetActiveSigningMaterial_PicksLatestActiveKey()
    // Membuka scope metode GetActiveSigningMaterial_PicksLatestActiveKey; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // GetActiveSigningMaterial_PicksLatestActiveKey.
    {
        // Menyiapkan variabel lokal `now` untuk nilai now dengan `DateTimeOffset.UtcNow`, yaitu waktu UTC saat operasi dilakukan. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var now = DateTimeOffset.UtcNow;
        // Menyiapkan variabel lokal `options` untuk kumpulan pengaturan yang mengendalikan perilaku komponen dengan memanggil `Options.Create` dengan `new
        // JwtOptions { SigningKeys = new List<JwtSigningKeyOptions> { new() { KeyId = ”k-2025”, SigningKey = new string('a', 32), ActivateAtUtc =
        // now.AddDays(-30), RetireAtUtc = now...`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var options = Options.Create(new JwtOptions
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // GetActiveSigningMaterial_PicksLatestActiveKey.
        {
            // Memperbarui `SigningKeys` menggunakan objek baru bertipe `List<JwtSigningKeyOptions>` dengan nilai awal sesuai konstruktornya dalam
            // GetActiveSigningMaterial_PicksLatestActiveKey.
            SigningKeys = new List<JwtSigningKeyOptions>
            // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // GetActiveSigningMaterial_PicksLatestActiveKey.
            {
                // Meneruskan objek baru bertipe `JwtOptions` dengan nilai awal sesuai konstruktornya sebagai argumen ke `Options.Create`.
                new()
                // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // GetActiveSigningMaterial_PicksLatestActiveKey.
                {
                    // Memperbarui `KeyId` menggunakan nilai literal `”k-2025”` dalam GetActiveSigningMaterial_PicksLatestActiveKey.
                    KeyId = "k-2025",
                    // Memperbarui `SigningKey` menggunakan objek baru bertipe `string` dengan argumen ('a', 32) dalam GetActiveSigningMaterial_PicksLatestActiveKey.
                    SigningKey = new string('a', 32),
                    // Memperbarui `ActivateAtUtc` menggunakan memanggil `now.AddDays` dengan `-30` dalam GetActiveSigningMaterial_PicksLatestActiveKey.
                    ActivateAtUtc = now.AddDays(-30),
                    // Memperbarui `RetireAtUtc` menggunakan memanggil `now.AddDays` dengan `-1` dalam GetActiveSigningMaterial_PicksLatestActiveKey.
                    RetireAtUtc = now.AddDays(-1)
                // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
                // GetActiveSigningMaterial_PicksLatestActiveKey.
                },
                // Meneruskan objek baru bertipe `JwtOptions` dengan nilai awal sesuai konstruktornya sebagai argumen ke `Options.Create`.
                new()
                // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // GetActiveSigningMaterial_PicksLatestActiveKey.
                {
                    // Memperbarui `KeyId` menggunakan nilai literal `”k-2026”` dalam GetActiveSigningMaterial_PicksLatestActiveKey.
                    KeyId = "k-2026",
                    // Memperbarui `SigningKey` menggunakan objek baru bertipe `string` dengan argumen ('b', 32) dalam GetActiveSigningMaterial_PicksLatestActiveKey.
                    SigningKey = new string('b', 32),
                    // Memperbarui `ActivateAtUtc` menggunakan memanggil `now.AddHours` dengan `-1` dalam GetActiveSigningMaterial_PicksLatestActiveKey.
                    ActivateAtUtc = now.AddHours(-1)
                // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
                // GetActiveSigningMaterial_PicksLatestActiveKey.
                }
            // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
            // GetActiveSigningMaterial_PicksLatestActiveKey.
            }
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // GetActiveSigningMaterial_PicksLatestActiveKey.
        });
        // Menyiapkan variabel lokal `sut` untuk nilai sut dengan objek baru bertipe `JwtSigningKeyProvider` dengan argumen (options,
        // NullLogger<JwtSigningKeyProvider>.Instance). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var sut = new JwtSigningKeyProvider(options, NullLogger<JwtSigningKeyProvider>.Instance);

        // Menyiapkan variabel lokal `active` untuk nilai aktif dengan memanggil `sut.GetActiveSigningMaterial` dengan tanpa argumen. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var active = sut.GetActiveSigningMaterial();

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”k-2026”`, `active.KeyId`); pengujian gagal
        // jika keduanya berbeda dalam GetActiveSigningMaterial_PicksLatestActiveKey.
        Assert.Equal("k-2026", active.KeyId);
        // Menyiapkan variabel lokal `validationKeys` untuk nilai validasi kunci dengan memanggil `sut.ResolveValidationKeys` dengan `null`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var validationKeys = sut.ResolveValidationKeys(null);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `validationKeys`, `key =>
        // string.Equals(key.KeyId, ”k-2026”, StringComparison.Ordinal)` dalam GetActiveSigningMaterial_PicksLatestActiveKey.
        Assert.Contains(validationKeys, key => string.Equals(key.KeyId, "k-2026", StringComparison.Ordinal));
    // Menutup scope metode GetActiveSigningMaterial_PicksLatestActiveKey; bagian berikut berada di luar batas blok tersebut dalam
    // GetActiveSigningMaterial_PicksLatestActiveKey.
    }
// Menutup scope tipe JwtSigningKeyProviderTests; bagian berikut berada di luar batas blok tersebut.
}
