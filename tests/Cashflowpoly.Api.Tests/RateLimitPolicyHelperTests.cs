// Fungsi file: Memverifikasi perilaku API, database, atau domain melalui RateLimitPolicyHelperTests.
// Mengimpor namespace `System.Net` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Net;
// Mengimpor namespace `System.Security.Claims` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Security.Claims;
// Mengimpor namespace `Cashflowpoly.Api.Security` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Security;
// Mengimpor namespace `Microsoft.AspNetCore.Http` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.AspNetCore.Http;
// Mengimpor namespace `Xunit` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Xunit;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Tests` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Tests;

/// <summary>
/// Kelas pengujian unit untuk memvalidasi bahwa RateLimitPolicyHelper
/// menghitung permit limit per endpoint dan membangun partition key yang tepat.
/// </summary>
// Mendefinisikan tipe class `RateLimitPolicyHelperTests`; sealed mencegah tipe ini diturunkan lagi.
public sealed class RateLimitPolicyHelperTests
// Membuka scope tipe RateLimitPolicyHelperTests; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // menandai metode sebagai pengujian xUnit yang dijalankan untuk setiap kombinasi data.
    [Theory]
    // menyediakan satu kombinasi masukan pengujian (”/api/v1/events”, 240).
    [InlineData("/api/v1/events", 240)]
    // menyediakan satu kombinasi masukan pengujian (”/api/v1/sessions”, 300).
    [InlineData("/api/v1/sessions", 300)]
    // menyediakan satu kombinasi masukan pengujian (”/api/events”, 300).
    [InlineData("/api/events", 300)]
    // menyediakan satu kombinasi masukan pengujian (”/api/v1/auth/login”, 10).
    [InlineData("/api/v1/auth/login", 10)]
    /// <summary>
    /// Memvalidasi bahwa ResolvePermitLimit mengembalikan batas request yang sesuai
    /// untuk setiap path endpoint API (events, sessions, auth, dll).
    /// </summary>
    // Mendefinisikan metode `ResolvePermitLimit_ReturnsExpectedValue` dengan hasil bertipe `void`; operasi ini menangani resolve permit limit returns
    // yang diharapkan nilai. Masukan: Parameter `path` bertipe `string` membawa nilai path; Parameter `expectedLimit` bertipe `int` membawa nilai yang
    // diharapkan limit.
    public void ResolvePermitLimit_ReturnsExpectedValue(string path, int expectedLimit)
    // Membuka scope metode ResolvePermitLimit_ReturnsExpectedValue; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // ResolvePermitLimit_ReturnsExpectedValue.
    {
        // Menyiapkan variabel lokal `result` untuk nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya dengan memanggil
        // `RateLimitPolicyHelper.ResolvePermitLimit` dengan `new PathString(path)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var result = RateLimitPolicyHelper.ResolvePermitLimit(new PathString(path));
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`expectedLimit`, `result`); pengujian gagal
        // jika keduanya berbeda dalam ResolvePermitLimit_ReturnsExpectedValue.
        Assert.Equal(expectedLimit, result);
    // Menutup scope metode ResolvePermitLimit_ReturnsExpectedValue; bagian berikut berada di luar batas blok tersebut dalam
    // ResolvePermitLimit_ReturnsExpectedValue.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    /// <summary>
    /// Memvalidasi bahwa BuildPartitionKey menggunakan user ID dari klaim
    /// NameIdentifier ketika pengguna sudah terotentikasi.
    /// </summary>
    // Mendefinisikan metode `BuildPartitionKey_UsesUserId_WhenAuthenticated` dengan hasil bertipe `void`; operasi ini menangani build partition kunci
    // uses pengguna identitas when authenticated.
    public void BuildPartitionKey_UsesUserId_WhenAuthenticated()
    // Membuka scope metode BuildPartitionKey_UsesUserId_WhenAuthenticated; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // BuildPartitionKey_UsesUserId_WhenAuthenticated.
    {
        // Menyiapkan variabel lokal `context` untuk konteks operasi yang menyediakan data lingkungan pemrosesan saat ini dengan objek baru bertipe
        // `DefaultHttpContext` dengan nilai awal sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var context = new DefaultHttpContext();
        // Memperbarui `context.Request.Path` menggunakan nilai literal `”/api/v1/events”` dalam BuildPartitionKey_UsesUserId_WhenAuthenticated.
        context.Request.Path = "/api/v1/events";
        // Memperbarui `context.User` menggunakan objek baru bertipe `ClaimsPrincipal` dengan argumen (new ClaimsIdentity( [new
        // Claim(ClaimTypes.NameIdentifier, ”user-123”)], authenticationType: ”test”)) dalam BuildPartitionKey_UsesUserId_WhenAuthenticated.
        context.User = new ClaimsPrincipal(new ClaimsIdentity(
            // Meneruskan koleksi berisi new Claim(ClaimTypes.NameIdentifier, ”user-123”) sebagai argumen ke konstruktor `ClaimsIdentity`; Meneruskan
            // `ClaimTypes.NameIdentifier` (nilai nama identifier) sebagai argumen ke konstruktor `Claim`; Meneruskan nilai literal `”user-123”` sebagai argumen
            // ke konstruktor `Claim`.
            [new Claim(ClaimTypes.NameIdentifier, "user-123")],
            // Meneruskan nilai literal `”test”` sebagai argumen bernama `authenticationType`.
            authenticationType: "test"));

        // Menyiapkan variabel lokal `key` untuk nilai kunci dengan memanggil `RateLimitPolicyHelper.BuildPartitionKey` dengan `context`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var key = RateLimitPolicyHelper.BuildPartitionKey(context);

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”ingest:user:user-123”`, `key`); pengujian
        // gagal jika keduanya berbeda dalam BuildPartitionKey_UsesUserId_WhenAuthenticated.
        Assert.Equal("ingest:user:user-123", key);
    // Menutup scope metode BuildPartitionKey_UsesUserId_WhenAuthenticated; bagian berikut berada di luar batas blok tersebut dalam
    // BuildPartitionKey_UsesUserId_WhenAuthenticated.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    /// <summary>
    /// Memvalidasi bahwa BuildPartitionKey menggunakan klaim "sub" sebagai fallback
    /// ketika klaim NameIdentifier tidak tersedia pada ClaimsPrincipal.
    /// </summary>
    // Mendefinisikan metode `BuildPartitionKey_FallsBackToSubClaim_WhenNameIdentifierMissing` dengan hasil bertipe `void`; operasi ini menangani build
    // partition kunci falls back ke sub claim when nama identifier missing.
    public void BuildPartitionKey_FallsBackToSubClaim_WhenNameIdentifierMissing()
    // Membuka scope metode BuildPartitionKey_FallsBackToSubClaim_WhenNameIdentifierMissing; pernyataan/deklarasi berikut berada di dalam batas blok ini
    // dalam BuildPartitionKey_FallsBackToSubClaim_WhenNameIdentifierMissing.
    {
        // Menyiapkan variabel lokal `context` untuk konteks operasi yang menyediakan data lingkungan pemrosesan saat ini dengan objek baru bertipe
        // `DefaultHttpContext` dengan nilai awal sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var context = new DefaultHttpContext();
        // Memperbarui `context.Request.Path` menggunakan nilai literal `”/api/v1/sessions”` dalam
        // BuildPartitionKey_FallsBackToSubClaim_WhenNameIdentifierMissing.
        context.Request.Path = "/api/v1/sessions";
        // Memperbarui `context.User` menggunakan objek baru bertipe `ClaimsPrincipal` dengan argumen (new ClaimsIdentity( [new Claim(”sub”,
        // ”subject-456”)], authenticationType: ”test”)) dalam BuildPartitionKey_FallsBackToSubClaim_WhenNameIdentifierMissing.
        context.User = new ClaimsPrincipal(new ClaimsIdentity(
            // Meneruskan koleksi berisi new Claim(”sub”, ”subject-456”) sebagai argumen ke konstruktor `ClaimsIdentity`; Meneruskan nilai literal `”sub”`
            // sebagai argumen ke konstruktor `Claim`; Meneruskan nilai literal `”subject-456”` sebagai argumen ke konstruktor `Claim`.
            [new Claim("sub", "subject-456")],
            // Meneruskan nilai literal `”test”` sebagai argumen bernama `authenticationType`.
            authenticationType: "test"));

        // Menyiapkan variabel lokal `key` untuk nilai kunci dengan memanggil `RateLimitPolicyHelper.BuildPartitionKey` dengan `context`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var key = RateLimitPolicyHelper.BuildPartitionKey(context);

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”default:user:subject-456”`, `key`); pengujian
        // gagal jika keduanya berbeda dalam BuildPartitionKey_FallsBackToSubClaim_WhenNameIdentifierMissing.
        Assert.Equal("default:user:subject-456", key);
    // Menutup scope metode BuildPartitionKey_FallsBackToSubClaim_WhenNameIdentifierMissing; bagian berikut berada di luar batas blok tersebut dalam
    // BuildPartitionKey_FallsBackToSubClaim_WhenNameIdentifierMissing.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    /// <summary>
    /// Memvalidasi bahwa BuildPartitionKey menggunakan alamat IP remote
    /// sebagai partition key untuk request anonim tanpa autentikasi.
    /// </summary>
    // Mendefinisikan metode `BuildPartitionKey_UsesRemoteIp_ForAnonymousRequest` dengan hasil bertipe `void`; operasi ini menangani build partition
    // kunci uses remote ip untuk anonymous permintaan.
    public void BuildPartitionKey_UsesRemoteIp_ForAnonymousRequest()
    // Membuka scope metode BuildPartitionKey_UsesRemoteIp_ForAnonymousRequest; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // BuildPartitionKey_UsesRemoteIp_ForAnonymousRequest.
    {
        // Menyiapkan variabel lokal `context` untuk konteks operasi yang menyediakan data lingkungan pemrosesan saat ini dengan objek baru bertipe
        // `DefaultHttpContext` dengan nilai awal sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var context = new DefaultHttpContext();
        // Memperbarui `context.Request.Path` menggunakan nilai literal `”/api/v1/sessions”` dalam BuildPartitionKey_UsesRemoteIp_ForAnonymousRequest.
        context.Request.Path = "/api/v1/sessions";
        // Memperbarui `context.Connection.RemoteIpAddress` menggunakan memanggil `IPAddress.Parse` dengan `”10.10.10.1”` dalam
        // BuildPartitionKey_UsesRemoteIp_ForAnonymousRequest.
        context.Connection.RemoteIpAddress = IPAddress.Parse("10.10.10.1");

        // Menyiapkan variabel lokal `key` untuk nilai kunci dengan memanggil `RateLimitPolicyHelper.BuildPartitionKey` dengan `context`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var key = RateLimitPolicyHelper.BuildPartitionKey(context);

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”default:ip:10.10.10.1”`, `key`); pengujian
        // gagal jika keduanya berbeda dalam BuildPartitionKey_UsesRemoteIp_ForAnonymousRequest.
        Assert.Equal("default:ip:10.10.10.1", key);
    // Menutup scope metode BuildPartitionKey_UsesRemoteIp_ForAnonymousRequest; bagian berikut berada di luar batas blok tersebut dalam
    // BuildPartitionKey_UsesRemoteIp_ForAnonymousRequest.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    /// <summary>
    /// Memvalidasi bahwa BuildPartitionKey mengembalikan "unknown" sebagai IP
    /// ketika RemoteIpAddress pada koneksi tidak tersedia (null).
    /// </summary>
    // Mendefinisikan metode `BuildPartitionKey_UsesUnknown_WhenRemoteIpMissing` dengan hasil bertipe `void`; operasi ini menangani build partition
    // kunci uses unknown when remote ip missing.
    public void BuildPartitionKey_UsesUnknown_WhenRemoteIpMissing()
    // Membuka scope metode BuildPartitionKey_UsesUnknown_WhenRemoteIpMissing; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // BuildPartitionKey_UsesUnknown_WhenRemoteIpMissing.
    {
        // Menyiapkan variabel lokal `context` untuk konteks operasi yang menyediakan data lingkungan pemrosesan saat ini dengan objek baru bertipe
        // `DefaultHttpContext` dengan nilai awal sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var context = new DefaultHttpContext();
        // Memperbarui `context.Request.Path` menggunakan nilai literal `”/api/v1/sessions”` dalam BuildPartitionKey_UsesUnknown_WhenRemoteIpMissing.
        context.Request.Path = "/api/v1/sessions";

        // Menyiapkan variabel lokal `key` untuk nilai kunci dengan memanggil `RateLimitPolicyHelper.BuildPartitionKey` dengan `context`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var key = RateLimitPolicyHelper.BuildPartitionKey(context);

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”default:ip:unknown”`, `key`); pengujian gagal
        // jika keduanya berbeda dalam BuildPartitionKey_UsesUnknown_WhenRemoteIpMissing.
        Assert.Equal("default:ip:unknown", key);
    // Menutup scope metode BuildPartitionKey_UsesUnknown_WhenRemoteIpMissing; bagian berikut berada di luar batas blok tersebut dalam
    // BuildPartitionKey_UsesUnknown_WhenRemoteIpMissing.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `BuildPartitionKey_UsesDedicatedAuthScope` dengan hasil bertipe `void`; operasi ini menangani build partition kunci uses
    // dedicated auth cakupan.
    public void BuildPartitionKey_UsesDedicatedAuthScope()
    // Membuka scope metode BuildPartitionKey_UsesDedicatedAuthScope; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // BuildPartitionKey_UsesDedicatedAuthScope.
    {
        // Menyiapkan variabel lokal `context` untuk konteks operasi yang menyediakan data lingkungan pemrosesan saat ini dengan objek baru bertipe
        // `DefaultHttpContext` dengan nilai awal sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var context = new DefaultHttpContext();
        // Memperbarui `context.Request.Path` menggunakan nilai literal `”/api/v1/auth/login”` dalam BuildPartitionKey_UsesDedicatedAuthScope.
        context.Request.Path = "/api/v1/auth/login";
        // Memperbarui `context.Connection.RemoteIpAddress` menggunakan memanggil `IPAddress.Parse` dengan `”10.10.10.2”` dalam
        // BuildPartitionKey_UsesDedicatedAuthScope.
        context.Connection.RemoteIpAddress = IPAddress.Parse("10.10.10.2");

        // Menyiapkan variabel lokal `key` untuk nilai kunci dengan memanggil `RateLimitPolicyHelper.BuildPartitionKey` dengan `context`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var key = RateLimitPolicyHelper.BuildPartitionKey(context);

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”auth:ip:10.10.10.2”`, `key`); pengujian gagal
        // jika keduanya berbeda dalam BuildPartitionKey_UsesDedicatedAuthScope.
        Assert.Equal("auth:ip:10.10.10.2", key);
    // Menutup scope metode BuildPartitionKey_UsesDedicatedAuthScope; bagian berikut berada di luar batas blok tersebut dalam
    // BuildPartitionKey_UsesDedicatedAuthScope.
    }
// Menutup scope tipe RateLimitPolicyHelperTests; bagian berikut berada di luar batas blok tersebut.
}
