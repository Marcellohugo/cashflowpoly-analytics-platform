// Fungsi file: Memverifikasi perilaku API, database, atau domain melalui JwtTokenServiceTests.
// Mengimpor namespace `System.IdentityModel.Tokens.Jwt` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.IdentityModel.Tokens.Jwt;
// Mengimpor namespace `System.Security.Claims` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Security.Claims;
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;
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
/// Kelas pengujian unit untuk memvalidasi bahwa JwtTokenService menghasilkan token
/// dengan klaim, issuer, audience, expiry, dan key ID yang benar.
/// </summary>
// Mendefinisikan tipe class `JwtTokenServiceTests`; sealed mencegah tipe ini diturunkan lagi.
public sealed class JwtTokenServiceTests
// Membuka scope tipe JwtTokenServiceTests; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    /// <summary>
    /// Memvalidasi bahwa JwtSigningKeyProvider.ValidateConfiguration melempar InvalidOperationException
    /// ketika signing key tidak dikonfigurasi (kosong).
    /// </summary>
    // Mendefinisikan metode `ValidateConfiguration_Throws_WhenSigningKeyMissing` dengan hasil bertipe `void`; operasi ini menangani validate
    // configuration throws when signing kunci missing.
    public void ValidateConfiguration_Throws_WhenSigningKeyMissing()
    // Membuka scope metode ValidateConfiguration_Throws_WhenSigningKeyMissing; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // ValidateConfiguration_Throws_WhenSigningKeyMissing.
    {
        // Menyiapkan variabel lokal `options` untuk kumpulan pengaturan yang mengendalikan perilaku komponen dengan objek baru bertipe `JwtOptions` dengan
        // nilai awal sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var options = new JwtOptions
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ValidateConfiguration_Throws_WhenSigningKeyMissing.
        {
            // Memperbarui `SigningKey` menggunakan `string.Empty`, yaitu nilai kosong bawaan tipe terkait dalam
            // ValidateConfiguration_Throws_WhenSigningKeyMissing.
            SigningKey = string.Empty
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // ValidateConfiguration_Throws_WhenSigningKeyMissing.
        };

        // Menyiapkan variabel lokal `provider` untuk nilai provider dengan memanggil `CreateSigningKeyProvider` dengan `options`. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var provider = CreateSigningKeyProvider(options);
        // Menyiapkan variabel lokal `ex` untuk nilai ex dengan pemeriksaan bahwa operasi `() => provider.ValidateConfiguration()` melempar jenis exception
        // yang diharapkan; pengujian gagal jika perilaku kesalahan berbeda. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var ex = Assert.Throws<InvalidOperationException>(() => provider.ValidateConfiguration());
        // Menjalankan pemeriksaan bahwa `ex.Message.Contains(”JWT signing key”, StringComparison.OrdinalIgnoreCase)` bernilai benar; pengujian gagal jika
        // kondisi tidak terpenuhi dalam ValidateConfiguration_Throws_WhenSigningKeyMissing.
        Assert.True(ex.Message.Contains("JWT signing key", StringComparison.OrdinalIgnoreCase));
    // Menutup scope metode ValidateConfiguration_Throws_WhenSigningKeyMissing; bagian berikut berada di luar batas blok tersebut dalam
    // ValidateConfiguration_Throws_WhenSigningKeyMissing.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    /// <summary>
    /// Memvalidasi bahwa JwtSigningKeyProvider.ValidateConfiguration melempar InvalidOperationException
    /// ketika signing key lebih pendek dari batas minimal 32 karakter.
    /// </summary>
    // Mendefinisikan metode `ValidateConfiguration_Throws_WhenSigningKeyTooShort` dengan hasil bertipe `void`; operasi ini menangani validate
    // configuration throws when signing kunci too short.
    public void ValidateConfiguration_Throws_WhenSigningKeyTooShort()
    // Membuka scope metode ValidateConfiguration_Throws_WhenSigningKeyTooShort; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // ValidateConfiguration_Throws_WhenSigningKeyTooShort.
    {
        // Menyiapkan variabel lokal `options` untuk kumpulan pengaturan yang mengendalikan perilaku komponen dengan objek baru bertipe `JwtOptions` dengan
        // nilai awal sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var options = new JwtOptions
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ValidateConfiguration_Throws_WhenSigningKeyTooShort.
        {
            // Memperbarui `SigningKey` menggunakan nilai literal `”short-key”` dalam ValidateConfiguration_Throws_WhenSigningKeyTooShort.
            SigningKey = "short-key"
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // ValidateConfiguration_Throws_WhenSigningKeyTooShort.
        };

        // Menyiapkan variabel lokal `provider` untuk nilai provider dengan memanggil `CreateSigningKeyProvider` dengan `options`. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var provider = CreateSigningKeyProvider(options);
        // Menyiapkan variabel lokal `ex` untuk nilai ex dengan pemeriksaan bahwa operasi `() => provider.ValidateConfiguration()` melempar jenis exception
        // yang diharapkan; pengujian gagal jika perilaku kesalahan berbeda. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var ex = Assert.Throws<InvalidOperationException>(() => provider.ValidateConfiguration());
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”minimal 32 karakter”`, `ex.Message`
        // dalam ValidateConfiguration_Throws_WhenSigningKeyTooShort.
        Assert.Contains("minimal 32 karakter", ex.Message);
    // Menutup scope metode ValidateConfiguration_Throws_WhenSigningKeyTooShort; bagian berikut berada di luar batas blok tersebut dalam
    // ValidateConfiguration_Throws_WhenSigningKeyTooShort.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    /// <summary>
    /// Memvalidasi bahwa IssueToken menghasilkan JWT dengan klaim sub, name, role,
    /// issuer, audience, kid, dan waktu kadaluarsa yang sesuai.
    /// </summary>
    // Mendefinisikan metode `IssueToken_ContainsExpectedClaims_AndExpiry` dengan hasil bertipe `void`; operasi ini menangani issue token contains yang
    // diharapkan claims dan expiry.
    public void IssueToken_ContainsExpectedClaims_AndExpiry()
    // Membuka scope metode IssueToken_ContainsExpectedClaims_AndExpiry; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // IssueToken_ContainsExpectedClaims_AndExpiry.
    {
        // Menyiapkan variabel lokal `options` untuk kumpulan pengaturan yang mengendalikan perilaku komponen dengan objek baru bertipe `JwtOptions` dengan
        // nilai awal sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var options = new JwtOptions
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // IssueToken_ContainsExpectedClaims_AndExpiry.
        {
            // Memperbarui `Issuer` menggunakan nilai literal `”Cashflowpoly.Test.Issuer”` dalam IssueToken_ContainsExpectedClaims_AndExpiry.
            Issuer = "Cashflowpoly.Test.Issuer",
            // Memperbarui `Audience` menggunakan nilai literal `”Cashflowpoly.Test.Audience”` dalam IssueToken_ContainsExpectedClaims_AndExpiry.
            Audience = "Cashflowpoly.Test.Audience",
            // Memperbarui `SigningKey` menggunakan objek baru bertipe `string` dengan argumen ('k', 32) dalam IssueToken_ContainsExpectedClaims_AndExpiry.
            SigningKey = new string('k', 32),
            // Memperbarui `AccessTokenMinutes` menggunakan nilai literal `15` dalam IssueToken_ContainsExpectedClaims_AndExpiry.
            AccessTokenMinutes = 15
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // IssueToken_ContainsExpectedClaims_AndExpiry.
        };
        // Menyiapkan variabel lokal `sut` untuk nilai sut dengan memanggil `CreateSut` dengan `options`. Tipe variabel disimpulkan dari ekspresi nilai
        // awal.
        var sut = CreateSut(options);
        // Menyiapkan variabel lokal `user` untuk pengguna yang sedang diproses beserta identitas atau klaim akses yang dimilikinya dengan objek baru
        // bertipe `AuthenticatedUserDb` dengan argumen ( Guid.Parse(”aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee”), ”alice”, ”Alice”, ”PLAYER”, true). Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var user = new AuthenticatedUserDb(
            // Meneruskan memanggil `Guid.Parse` dengan `”aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee”` sebagai argumen ke konstruktor `AuthenticatedUserDb`;
            // Meneruskan nilai literal `”aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee”` sebagai argumen ke `Guid.Parse`.
            Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"),
            // Meneruskan nilai literal `”alice”` sebagai argumen ke konstruktor `AuthenticatedUserDb`.
            "alice",
            // Meneruskan nilai literal `”Alice”` sebagai argumen ke konstruktor `AuthenticatedUserDb`.
            "Alice",
            // Meneruskan nilai literal `”PLAYER”` sebagai argumen ke konstruktor `AuthenticatedUserDb`.
            "PLAYER",
            // Meneruskan true, yaitu kondisi aktif/terpenuhi sebagai argumen ke konstruktor `AuthenticatedUserDb`.
            true);

        // Menyiapkan variabel lokal `beforeIssue` untuk nilai before issue dengan `DateTimeOffset.UtcNow`, yaitu waktu UTC saat operasi dilakukan. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var beforeIssue = DateTimeOffset.UtcNow;
        // Menyiapkan variabel lokal `issued` untuk nilai issued dengan memanggil `sut.IssueToken` dengan `user`. Tipe variabel disimpulkan dari ekspresi
        // nilai awal.
        var issued = sut.IssueToken(user);
        // Menyiapkan variabel lokal `afterIssue` untuk nilai after issue dengan `DateTimeOffset.UtcNow`, yaitu waktu UTC saat operasi dilakukan. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var afterIssue = DateTimeOffset.UtcNow;

        // Menyiapkan variabel lokal `token` untuk token yang diteruskan pada operasi terkait dengan memanggil `new JwtSecurityTokenHandler().ReadJwtToken`
        // dengan `issued.AccessToken`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var token = new JwtSecurityTokenHandler().ReadJwtToken(issued.AccessToken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”Cashflowpoly.Test.Issuer”`, `token.Issuer`);
        // pengujian gagal jika keduanya berbeda dalam IssueToken_ContainsExpectedClaims_AndExpiry.
        Assert.Equal("Cashflowpoly.Test.Issuer", token.Issuer);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”Cashflowpoly.Test.Audience”`,
        // `token.Audiences` dalam IssueToken_ContainsExpectedClaims_AndExpiry.
        Assert.Contains("Cashflowpoly.Test.Audience", token.Audiences);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`user.UserId.ToString()`, `token.Claims.First(c
        // => c.Type == JwtRegisteredClaimNames.Sub).Value`); pengujian gagal jika keduanya berbeda dalam IssueToken_ContainsExpectedClaims_AndExpiry.
        Assert.Equal(user.UserId.ToString(), token.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`user.Username`, `token.Claims.First(c =>
        // c.Type == ClaimTypes.Name).Value`); pengujian gagal jika keduanya berbeda dalam IssueToken_ContainsExpectedClaims_AndExpiry.
        Assert.Equal(user.Username, token.Claims.First(c => c.Type == ClaimTypes.Name).Value);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`user.UserId.ToString()`, `token.Claims.First(c
        // => c.Type == ClaimTypes.NameIdentifier).Value`); pengujian gagal jika keduanya berbeda dalam IssueToken_ContainsExpectedClaims_AndExpiry.
        Assert.Equal(user.UserId.ToString(), token.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”PLAYER”`, `token.Claims.First(c => c.Type ==
        // ClaimTypes.Role).Value`); pengujian gagal jika keduanya berbeda dalam IssueToken_ContainsExpectedClaims_AndExpiry.
        Assert.Equal("PLAYER", token.Claims.First(c => c.Type == ClaimTypes.Role).Value);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”false”`, `token.Claims.First(c => c.Type ==
        // JwtTokenService.DemoAccountClaim).Value`); pengujian gagal jika keduanya berbeda dalam IssueToken_ContainsExpectedClaims_AndExpiry.
        Assert.Equal("false", token.Claims.First(c => c.Type == JwtTokenService.DemoAccountClaim).Value);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”default”`, `token.Header.Kid`); pengujian
        // gagal jika keduanya berbeda dalam IssueToken_ContainsExpectedClaims_AndExpiry.
        Assert.Equal("default", token.Header.Kid);

        // Menyiapkan variabel lokal `minExpected` untuk nilai minimum yang diharapkan dengan memanggil `beforeIssue.AddMinutes(15).AddSeconds` dengan `-2`.
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var minExpected = beforeIssue.AddMinutes(15).AddSeconds(-2);
        // Menyiapkan variabel lokal `maxExpected` untuk nilai maksimum yang diharapkan dengan memanggil `afterIssue.AddMinutes(15).AddSeconds` dengan `2`.
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var maxExpected = afterIssue.AddMinutes(15).AddSeconds(2);
        // Menjalankan pemeriksaan hasil dengan `Assert.InRange` menggunakan `issued.ExpiresAt`, `minExpected`, `maxExpected`; ketidaksesuaian dengan
        // ekspektasi membuat pengujian gagal dalam IssueToken_ContainsExpectedClaims_AndExpiry.
        Assert.InRange(issued.ExpiresAt, minExpected, maxExpected);
    // Menutup scope metode IssueToken_ContainsExpectedClaims_AndExpiry; bagian berikut berada di luar batas blok tersebut dalam
    // IssueToken_ContainsExpectedClaims_AndExpiry.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `IssueToken_MarksDemoAccountWithoutChangingTokenLifetime` dengan hasil bertipe `void`; operasi ini menangani issue token
    // marks demo account tanpa changing token lifetime.
    public void IssueToken_MarksDemoAccountWithoutChangingTokenLifetime()
    // Membuka scope metode IssueToken_MarksDemoAccountWithoutChangingTokenLifetime; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // IssueToken_MarksDemoAccountWithoutChangingTokenLifetime.
    {
        // Menyiapkan variabel lokal `options` untuk kumpulan pengaturan yang mengendalikan perilaku komponen dengan objek baru bertipe `JwtOptions` dengan
        // nilai awal sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var options = new JwtOptions
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // IssueToken_MarksDemoAccountWithoutChangingTokenLifetime.
        {
            // Memperbarui `Issuer` menggunakan nilai literal `”Cashflowpoly.Test.Issuer”` dalam IssueToken_MarksDemoAccountWithoutChangingTokenLifetime.
            Issuer = "Cashflowpoly.Test.Issuer",
            // Memperbarui `Audience` menggunakan nilai literal `”Cashflowpoly.Test.Audience”` dalam IssueToken_MarksDemoAccountWithoutChangingTokenLifetime.
            Audience = "Cashflowpoly.Test.Audience",
            // Memperbarui `SigningKey` menggunakan objek baru bertipe `string` dengan argumen ('d', 32) dalam
            // IssueToken_MarksDemoAccountWithoutChangingTokenLifetime.
            SigningKey = new string('d', 32),
            // Memperbarui `AccessTokenMinutes` menggunakan nilai literal `480` dalam IssueToken_MarksDemoAccountWithoutChangingTokenLifetime.
            AccessTokenMinutes = 480
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // IssueToken_MarksDemoAccountWithoutChangingTokenLifetime.
        };
        // Menyiapkan variabel lokal `sut` untuk nilai sut dengan memanggil `CreateSut` dengan `options`. Tipe variabel disimpulkan dari ekspresi nilai
        // awal.
        var sut = CreateSut(options);
        // Menyiapkan variabel lokal `user` untuk pengguna yang sedang diproses beserta identitas atau klaim akses yang dimilikinya dengan objek baru
        // bertipe `AuthenticatedUserDb` dengan argumen (Guid.NewGuid(), ”demo”, ”Demo”, ”PLAYER”, true, IsDemo: true). Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var user = new AuthenticatedUserDb(Guid.NewGuid(), "demo", "Demo", "PLAYER", true, IsDemo: true);

        // Menyiapkan variabel lokal `issued` untuk nilai issued dengan memanggil `sut.IssueToken` dengan `user`. Tipe variabel disimpulkan dari ekspresi
        // nilai awal.
        var issued = sut.IssueToken(user);
        // Menyiapkan variabel lokal `token` untuk token yang diteruskan pada operasi terkait dengan memanggil `new JwtSecurityTokenHandler().ReadJwtToken`
        // dengan `issued.AccessToken`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var token = new JwtSecurityTokenHandler().ReadJwtToken(issued.AccessToken);

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”true”`, `token.Claims.First(c => c.Type ==
        // JwtTokenService.DemoAccountClaim).Value`); pengujian gagal jika keduanya berbeda dalam IssueToken_MarksDemoAccountWithoutChangingTokenLifetime.
        Assert.Equal("true", token.Claims.First(c => c.Type == JwtTokenService.DemoAccountClaim).Value);
        // Menjalankan pemeriksaan hasil dengan `Assert.InRange` menggunakan `issued.ExpiresAt`, `DateTimeOffset.UtcNow.AddHours(7.9)`,
        // `DateTimeOffset.UtcNow.AddHours(8.1)`; ketidaksesuaian dengan ekspektasi membuat pengujian gagal dalam
        // IssueToken_MarksDemoAccountWithoutChangingTokenLifetime.
        Assert.InRange(issued.ExpiresAt, DateTimeOffset.UtcNow.AddHours(7.9), DateTimeOffset.UtcNow.AddHours(8.1));
    // Menutup scope metode IssueToken_MarksDemoAccountWithoutChangingTokenLifetime; bagian berikut berada di luar batas blok tersebut dalam
    // IssueToken_MarksDemoAccountWithoutChangingTokenLifetime.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    /// <summary>
    /// Memvalidasi bahwa IssueToken menggunakan key ID dari signing key aktif terbaru
    /// ketika beberapa signing key dikonfigurasi dengan jadwal rotasi.
    /// </summary>
    // Mendefinisikan metode `IssueToken_UsesActiveKeyId_WhenMultipleKeysConfigured` dengan hasil bertipe `void`; operasi ini menangani issue token uses
    // aktif kunci identitas when multiple kunci configured.
    public void IssueToken_UsesActiveKeyId_WhenMultipleKeysConfigured()
    // Membuka scope metode IssueToken_UsesActiveKeyId_WhenMultipleKeysConfigured; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // IssueToken_UsesActiveKeyId_WhenMultipleKeysConfigured.
    {
        // Menyiapkan variabel lokal `now` untuk nilai now dengan `DateTimeOffset.UtcNow`, yaitu waktu UTC saat operasi dilakukan. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var now = DateTimeOffset.UtcNow;
        // Menyiapkan variabel lokal `options` untuk kumpulan pengaturan yang mengendalikan perilaku komponen dengan objek baru bertipe `JwtOptions` dengan
        // nilai awal sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var options = new JwtOptions
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // IssueToken_UsesActiveKeyId_WhenMultipleKeysConfigured.
        {
            // Memperbarui `Issuer` menggunakan nilai literal `”Cashflowpoly.Test.Issuer”` dalam IssueToken_UsesActiveKeyId_WhenMultipleKeysConfigured.
            Issuer = "Cashflowpoly.Test.Issuer",
            // Memperbarui `Audience` menggunakan nilai literal `”Cashflowpoly.Test.Audience”` dalam IssueToken_UsesActiveKeyId_WhenMultipleKeysConfigured.
            Audience = "Cashflowpoly.Test.Audience",
            // Memperbarui `AccessTokenMinutes` menggunakan nilai literal `15` dalam IssueToken_UsesActiveKeyId_WhenMultipleKeysConfigured.
            AccessTokenMinutes = 15,
            // Memperbarui `SigningKeys` menggunakan objek baru bertipe `List<JwtSigningKeyOptions>` dengan nilai awal sesuai konstruktornya dalam
            // IssueToken_UsesActiveKeyId_WhenMultipleKeysConfigured.
            SigningKeys = new List<JwtSigningKeyOptions>
            // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // IssueToken_UsesActiveKeyId_WhenMultipleKeysConfigured.
            {
                // Menggunakan objek baru bertipe `JwtSigningKeyOptions` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun dalam
                // IssueToken_UsesActiveKeyId_WhenMultipleKeysConfigured.
                new JwtSigningKeyOptions
                // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // IssueToken_UsesActiveKeyId_WhenMultipleKeysConfigured.
                {
                    // Memperbarui `KeyId` menggunakan nilai literal `”k-2025”` dalam IssueToken_UsesActiveKeyId_WhenMultipleKeysConfigured.
                    KeyId = "k-2025",
                    // Memperbarui `SigningKey` menggunakan objek baru bertipe `string` dengan argumen ('a', 32) dalam
                    // IssueToken_UsesActiveKeyId_WhenMultipleKeysConfigured.
                    SigningKey = new string('a', 32),
                    // Memperbarui `ActivateAtUtc` menggunakan memanggil `now.AddDays` dengan `-10` dalam IssueToken_UsesActiveKeyId_WhenMultipleKeysConfigured.
                    ActivateAtUtc = now.AddDays(-10),
                    // Memperbarui `RetireAtUtc` menggunakan memanggil `now.AddDays` dengan `-1` dalam IssueToken_UsesActiveKeyId_WhenMultipleKeysConfigured.
                    RetireAtUtc = now.AddDays(-1)
                // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
                // IssueToken_UsesActiveKeyId_WhenMultipleKeysConfigured.
                },
                // Menggunakan objek baru bertipe `JwtSigningKeyOptions` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun dalam
                // IssueToken_UsesActiveKeyId_WhenMultipleKeysConfigured.
                new JwtSigningKeyOptions
                // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // IssueToken_UsesActiveKeyId_WhenMultipleKeysConfigured.
                {
                    // Memperbarui `KeyId` menggunakan nilai literal `”k-2026”` dalam IssueToken_UsesActiveKeyId_WhenMultipleKeysConfigured.
                    KeyId = "k-2026",
                    // Memperbarui `SigningKey` menggunakan objek baru bertipe `string` dengan argumen ('b', 32) dalam
                    // IssueToken_UsesActiveKeyId_WhenMultipleKeysConfigured.
                    SigningKey = new string('b', 32),
                    // Memperbarui `ActivateAtUtc` menggunakan memanggil `now.AddDays` dengan `-1` dalam IssueToken_UsesActiveKeyId_WhenMultipleKeysConfigured.
                    ActivateAtUtc = now.AddDays(-1)
                // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
                // IssueToken_UsesActiveKeyId_WhenMultipleKeysConfigured.
                }
            // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
            // IssueToken_UsesActiveKeyId_WhenMultipleKeysConfigured.
            }
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // IssueToken_UsesActiveKeyId_WhenMultipleKeysConfigured.
        };

        // Menyiapkan variabel lokal `sut` untuk nilai sut dengan memanggil `CreateSut` dengan `options`. Tipe variabel disimpulkan dari ekspresi nilai
        // awal.
        var sut = CreateSut(options);
        // Menyiapkan variabel lokal `user` untuk pengguna yang sedang diproses beserta identitas atau klaim akses yang dimilikinya dengan objek baru
        // bertipe `AuthenticatedUserDb` dengan argumen (Guid.NewGuid(), ”bob”, ”Bob”, ”INSTRUCTOR”, true). Tipe variabel disimpulkan dari ekspresi nilai
        // awal.
        var user = new AuthenticatedUserDb(Guid.NewGuid(), "bob", "Bob", "INSTRUCTOR", true);

        // Menyiapkan variabel lokal `issued` untuk nilai issued dengan memanggil `sut.IssueToken` dengan `user`. Tipe variabel disimpulkan dari ekspresi
        // nilai awal.
        var issued = sut.IssueToken(user);
        // Menyiapkan variabel lokal `token` untuk token yang diteruskan pada operasi terkait dengan memanggil `new JwtSecurityTokenHandler().ReadJwtToken`
        // dengan `issued.AccessToken`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var token = new JwtSecurityTokenHandler().ReadJwtToken(issued.AccessToken);

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”k-2026”`, `token.Header.Kid`); pengujian
        // gagal jika keduanya berbeda dalam IssueToken_UsesActiveKeyId_WhenMultipleKeysConfigured.
        Assert.Equal("k-2026", token.Header.Kid);
    // Menutup scope metode IssueToken_UsesActiveKeyId_WhenMultipleKeysConfigured; bagian berikut berada di luar batas blok tersebut dalam
    // IssueToken_UsesActiveKeyId_WhenMultipleKeysConfigured.
    }

    /// <summary>
    /// Helper untuk membuat instance JwtTokenService dengan opsi JWT yang ditentukan.
    /// </summary>
    // Mendefinisikan metode `CreateSut` dengan hasil bertipe `JwtTokenService`. Helper untuk membuat instance JwtTokenService dengan opsi JWT yang
    // ditentukan. Masukan: Parameter `options` bertipe `JwtOptions` membawa kumpulan pengaturan yang mengendalikan perilaku komponen.
    private static JwtTokenService CreateSut(JwtOptions options)
    // Membuka scope metode CreateSut; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CreateSut.
    {
        // Menyiapkan variabel lokal `optionsWrapper` untuk nilai options wrapper dengan memanggil `Options.Create` dengan `options`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var optionsWrapper = Options.Create(options);
        // Menyiapkan variabel lokal `provider` untuk nilai provider dengan objek baru bertipe `JwtSigningKeyProvider` dengan argumen (optionsWrapper,
        // NullLogger<JwtSigningKeyProvider>.Instance). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var provider = new JwtSigningKeyProvider(optionsWrapper, NullLogger<JwtSigningKeyProvider>.Instance);
        // Mengembalikan objek baru bertipe `JwtTokenService` dengan argumen (optionsWrapper, provider) kepada pemanggil dalam CreateSut; eksekusi jalur ini
        // selesai setelah nilai hasil ditentukan.
        return new JwtTokenService(optionsWrapper, provider);
    // Menutup scope metode CreateSut; bagian berikut berada di luar batas blok tersebut dalam CreateSut.
    }

    /// <summary>
    /// Helper untuk membuat instance JwtSigningKeyProvider dengan opsi JWT yang ditentukan.
    /// </summary>
    // Mendefinisikan metode `CreateSigningKeyProvider` dengan hasil bertipe `JwtSigningKeyProvider`. Helper untuk membuat instance
    // JwtSigningKeyProvider dengan opsi JWT yang ditentukan. Masukan: Parameter `options` bertipe `JwtOptions` membawa kumpulan pengaturan yang
    // mengendalikan perilaku komponen.
    private static JwtSigningKeyProvider CreateSigningKeyProvider(JwtOptions options)
    // Membuka scope metode CreateSigningKeyProvider; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CreateSigningKeyProvider.
    {
        // Menyiapkan variabel lokal `optionsWrapper` untuk nilai options wrapper dengan memanggil `Options.Create` dengan `options`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var optionsWrapper = Options.Create(options);
        // Mengembalikan objek baru bertipe `JwtSigningKeyProvider` dengan argumen (optionsWrapper, NullLogger<JwtSigningKeyProvider>.Instance) kepada
        // pemanggil dalam CreateSigningKeyProvider; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return new JwtSigningKeyProvider(optionsWrapper, NullLogger<JwtSigningKeyProvider>.Instance);
    // Menutup scope metode CreateSigningKeyProvider; bagian berikut berada di luar batas blok tersebut dalam CreateSigningKeyProvider.
    }
// Menutup scope tipe JwtTokenServiceTests; bagian berikut berada di luar batas blok tersebut.
}
