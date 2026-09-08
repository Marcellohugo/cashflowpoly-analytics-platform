// Fungsi file: Memverifikasi perilaku, lokalisasi, atau tata letak UI melalui HttpsRedirectionPolicyTests.
// Mengimpor namespace `Cashflowpoly.Ui.Infrastructure` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Ui.Infrastructure;
// Mengimpor namespace `Microsoft.AspNetCore.Http` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.AspNetCore.Http;
// Mengimpor namespace `Microsoft.Extensions.Configuration` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.Extensions.Configuration;
// Mengimpor namespace `Xunit` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Xunit;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Tests` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Tests;

// Mendefinisikan tipe class `HttpsRedirectionPolicyTests`; sealed mencegah tipe ini diturunkan lagi.
public sealed class HttpsRedirectionPolicyTests
// Membuka scope tipe HttpsRedirectionPolicyTests; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `ShouldUseHttpsRedirection_ReturnsFalse_ForHttpOnlyUrls` dengan hasil bertipe `void`; operasi ini menangani should use
    // https redirection returns false untuk HTTP only urls.
    public void ShouldUseHttpsRedirection_ReturnsFalse_ForHttpOnlyUrls()
    // Membuka scope metode ShouldUseHttpsRedirection_ReturnsFalse_ForHttpOnlyUrls; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // ShouldUseHttpsRedirection_ReturnsFalse_ForHttpOnlyUrls.
    {
        // Menyiapkan variabel lokal `configuration` untuk konfigurasi aplikasi yang menyediakan nilai pengaturan dari sumber terdaftar dengan memanggil
        // `new ConfigurationBuilder() .AddInMemoryCollection(new Dictionary<string, string?> { [”ASPNETCORE_URLS”] = ”http://+:5203” }) .Build` dengan
        // tanpa argumen. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var configuration = new ConfigurationBuilder()
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .AddInMemoryCollection(new Dictionary<string, string?> dalam
            // ShouldUseHttpsRedirection_ReturnsFalse_ForHttpOnlyUrls; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .AddInMemoryCollection(new Dictionary<string, string?>
            // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ShouldUseHttpsRedirection_ReturnsFalse_ForHttpOnlyUrls.
            {
                // Memperbarui `[”ASPNETCORE_URLS”]` menggunakan nilai literal `”http://+:5203”` dalam ShouldUseHttpsRedirection_ReturnsFalse_ForHttpOnlyUrls.
                ["ASPNETCORE_URLS"] = "http://+:5203"
            // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
            // ShouldUseHttpsRedirection_ReturnsFalse_ForHttpOnlyUrls.
            })
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Build(); dalam ShouldUseHttpsRedirection_ReturnsFalse_ForHttpOnlyUrls; token
            // pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Build();

        // Menyiapkan variabel lokal `result` untuk nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya dengan memanggil
        // `HttpsRedirectionPolicy.ShouldUseHttpsRedirection` dengan `configuration`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var result = HttpsRedirectionPolicy.ShouldUseHttpsRedirection(configuration);

        // Menjalankan pemeriksaan bahwa `result` bernilai salah; pengujian gagal jika kondisi justru terpenuhi dalam
        // ShouldUseHttpsRedirection_ReturnsFalse_ForHttpOnlyUrls.
        Assert.False(result);
    // Menutup scope metode ShouldUseHttpsRedirection_ReturnsFalse_ForHttpOnlyUrls; bagian berikut berada di luar batas blok tersebut dalam
    // ShouldUseHttpsRedirection_ReturnsFalse_ForHttpOnlyUrls.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `ShouldUseHttpsRedirection_ReturnsTrue_WhenHttpsPortConfigured` dengan hasil bertipe `void`; operasi ini menangani should
    // use https redirection returns true when https port configured.
    public void ShouldUseHttpsRedirection_ReturnsTrue_WhenHttpsPortConfigured()
    // Membuka scope metode ShouldUseHttpsRedirection_ReturnsTrue_WhenHttpsPortConfigured; pernyataan/deklarasi berikut berada di dalam batas blok ini
    // dalam ShouldUseHttpsRedirection_ReturnsTrue_WhenHttpsPortConfigured.
    {
        // Menyiapkan variabel lokal `configuration` untuk konfigurasi aplikasi yang menyediakan nilai pengaturan dari sumber terdaftar dengan memanggil
        // `new ConfigurationBuilder() .AddInMemoryCollection(new Dictionary<string, string?> { [”ASPNETCORE_HTTPS_PORT”] = ”443” }) .Build` dengan tanpa
        // argumen. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var configuration = new ConfigurationBuilder()
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .AddInMemoryCollection(new Dictionary<string, string?> dalam
            // ShouldUseHttpsRedirection_ReturnsTrue_WhenHttpsPortConfigured; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .AddInMemoryCollection(new Dictionary<string, string?>
            // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ShouldUseHttpsRedirection_ReturnsTrue_WhenHttpsPortConfigured.
            {
                // Memperbarui `[”ASPNETCORE_HTTPS_PORT”]` menggunakan nilai literal `”443”` dalam ShouldUseHttpsRedirection_ReturnsTrue_WhenHttpsPortConfigured.
                ["ASPNETCORE_HTTPS_PORT"] = "443"
            // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
            // ShouldUseHttpsRedirection_ReturnsTrue_WhenHttpsPortConfigured.
            })
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Build(); dalam ShouldUseHttpsRedirection_ReturnsTrue_WhenHttpsPortConfigured;
            // token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Build();

        // Menyiapkan variabel lokal `result` untuk nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya dengan memanggil
        // `HttpsRedirectionPolicy.ShouldUseHttpsRedirection` dengan `configuration`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var result = HttpsRedirectionPolicy.ShouldUseHttpsRedirection(configuration);

        // Menjalankan pemeriksaan bahwa `result` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // ShouldUseHttpsRedirection_ReturnsTrue_WhenHttpsPortConfigured.
        Assert.True(result);
    // Menutup scope metode ShouldUseHttpsRedirection_ReturnsTrue_WhenHttpsPortConfigured; bagian berikut berada di luar batas blok tersebut dalam
    // ShouldUseHttpsRedirection_ReturnsTrue_WhenHttpsPortConfigured.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `ShouldUseHttpsRedirection_ReturnsTrue_WhenUrlsIncludeHttps` dengan hasil bertipe `void`; operasi ini menangani should use
    // https redirection returns true when urls include https.
    public void ShouldUseHttpsRedirection_ReturnsTrue_WhenUrlsIncludeHttps()
    // Membuka scope metode ShouldUseHttpsRedirection_ReturnsTrue_WhenUrlsIncludeHttps; pernyataan/deklarasi berikut berada di dalam batas blok ini
    // dalam ShouldUseHttpsRedirection_ReturnsTrue_WhenUrlsIncludeHttps.
    {
        // Menyiapkan variabel lokal `configuration` untuk konfigurasi aplikasi yang menyediakan nilai pengaturan dari sumber terdaftar dengan memanggil
        // `new ConfigurationBuilder() .AddInMemoryCollection(new Dictionary<string, string?> { [”URLS”] = ”http://+:5203;https://+:5443” }) .Build` dengan
        // tanpa argumen. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var configuration = new ConfigurationBuilder()
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .AddInMemoryCollection(new Dictionary<string, string?> dalam
            // ShouldUseHttpsRedirection_ReturnsTrue_WhenUrlsIncludeHttps; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .AddInMemoryCollection(new Dictionary<string, string?>
            // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ShouldUseHttpsRedirection_ReturnsTrue_WhenUrlsIncludeHttps.
            {
                // Memperbarui `[”URLS”]` menggunakan nilai literal `”http://+:5203;https://+:5443”` dalam
                // ShouldUseHttpsRedirection_ReturnsTrue_WhenUrlsIncludeHttps.
                ["URLS"] = "http://+:5203;https://+:5443"
            // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
            // ShouldUseHttpsRedirection_ReturnsTrue_WhenUrlsIncludeHttps.
            })
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Build(); dalam ShouldUseHttpsRedirection_ReturnsTrue_WhenUrlsIncludeHttps;
            // token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Build();

        // Menyiapkan variabel lokal `result` untuk nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya dengan memanggil
        // `HttpsRedirectionPolicy.ShouldUseHttpsRedirection` dengan `configuration`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var result = HttpsRedirectionPolicy.ShouldUseHttpsRedirection(configuration);

        // Menjalankan pemeriksaan bahwa `result` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // ShouldUseHttpsRedirection_ReturnsTrue_WhenUrlsIncludeHttps.
        Assert.True(result);
    // Menutup scope metode ShouldUseHttpsRedirection_ReturnsTrue_WhenUrlsIncludeHttps; bagian berikut berada di luar batas blok tersebut dalam
    // ShouldUseHttpsRedirection_ReturnsTrue_WhenUrlsIncludeHttps.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `ResolveCookieSecurePolicy_ReturnsSameAsRequest_ForProductionHttpOnlyLocalCompose` dengan hasil bertipe `void`; operasi ini
    // menangani resolve cookie secure policy returns same as permintaan untuk production HTTP only local compose.
    public void ResolveCookieSecurePolicy_ReturnsSameAsRequest_ForProductionHttpOnlyLocalCompose()
    // Membuka scope metode ResolveCookieSecurePolicy_ReturnsSameAsRequest_ForProductionHttpOnlyLocalCompose; pernyataan/deklarasi berikut berada di
    // dalam batas blok ini dalam ResolveCookieSecurePolicy_ReturnsSameAsRequest_ForProductionHttpOnlyLocalCompose.
    {
        // Menyiapkan variabel lokal `configuration` untuk konfigurasi aplikasi yang menyediakan nilai pengaturan dari sumber terdaftar dengan memanggil
        // `new ConfigurationBuilder() .AddInMemoryCollection(new Dictionary<string, string?> { [”ASPNETCORE_ENVIRONMENT”] = ”Production”,
        // [”ASPNETCORE_URLS”] = ”http://+:5203” }) .Build` dengan tanpa argumen. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var configuration = new ConfigurationBuilder()
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .AddInMemoryCollection(new Dictionary<string, string?> dalam
            // ResolveCookieSecurePolicy_ReturnsSameAsRequest_ForProductionHttpOnlyLocalCompose; token pada baris ini menyambungkan bagian kode sebelum dan
            // sesudahnya.
            .AddInMemoryCollection(new Dictionary<string, string?>
            // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ResolveCookieSecurePolicy_ReturnsSameAsRequest_ForProductionHttpOnlyLocalCompose.
            {
                // Memperbarui `[”ASPNETCORE_ENVIRONMENT”]` menggunakan nilai literal `”Production”` dalam
                // ResolveCookieSecurePolicy_ReturnsSameAsRequest_ForProductionHttpOnlyLocalCompose.
                ["ASPNETCORE_ENVIRONMENT"] = "Production",
                // Memperbarui `[”ASPNETCORE_URLS”]` menggunakan nilai literal `”http://+:5203”` dalam
                // ResolveCookieSecurePolicy_ReturnsSameAsRequest_ForProductionHttpOnlyLocalCompose.
                ["ASPNETCORE_URLS"] = "http://+:5203"
            // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
            // ResolveCookieSecurePolicy_ReturnsSameAsRequest_ForProductionHttpOnlyLocalCompose.
            })
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Build(); dalam
            // ResolveCookieSecurePolicy_ReturnsSameAsRequest_ForProductionHttpOnlyLocalCompose; token pada baris ini menyambungkan bagian kode sebelum dan
            // sesudahnya.
            .Build();

        // Menyiapkan variabel lokal `result` untuk nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya dengan memanggil
        // `HttpsRedirectionPolicy.ResolveCookieSecurePolicy` dengan `configuration`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var result = HttpsRedirectionPolicy.ResolveCookieSecurePolicy(configuration);

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`CookieSecurePolicy.SameAsRequest`, `result`);
        // pengujian gagal jika keduanya berbeda dalam ResolveCookieSecurePolicy_ReturnsSameAsRequest_ForProductionHttpOnlyLocalCompose.
        Assert.Equal(CookieSecurePolicy.SameAsRequest, result);
    // Menutup scope metode ResolveCookieSecurePolicy_ReturnsSameAsRequest_ForProductionHttpOnlyLocalCompose; bagian berikut berada di luar batas blok
    // tersebut dalam ResolveCookieSecurePolicy_ReturnsSameAsRequest_ForProductionHttpOnlyLocalCompose.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `ResolveCookieSecurePolicy_ReturnsAlways_WhenHttpsIsConfigured` dengan hasil bertipe `void`; operasi ini menangani resolve
    // cookie secure policy returns always when https berstatus configured.
    public void ResolveCookieSecurePolicy_ReturnsAlways_WhenHttpsIsConfigured()
    // Membuka scope metode ResolveCookieSecurePolicy_ReturnsAlways_WhenHttpsIsConfigured; pernyataan/deklarasi berikut berada di dalam batas blok ini
    // dalam ResolveCookieSecurePolicy_ReturnsAlways_WhenHttpsIsConfigured.
    {
        // Menyiapkan variabel lokal `configuration` untuk konfigurasi aplikasi yang menyediakan nilai pengaturan dari sumber terdaftar dengan memanggil
        // `new ConfigurationBuilder() .AddInMemoryCollection(new Dictionary<string, string?> { [”ASPNETCORE_ENVIRONMENT”] = ”Production”,
        // [”ASPNETCORE_URLS”] = ”http://+:5203;https://+:5...` dengan tanpa argumen. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var configuration = new ConfigurationBuilder()
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .AddInMemoryCollection(new Dictionary<string, string?> dalam
            // ResolveCookieSecurePolicy_ReturnsAlways_WhenHttpsIsConfigured; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .AddInMemoryCollection(new Dictionary<string, string?>
            // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ResolveCookieSecurePolicy_ReturnsAlways_WhenHttpsIsConfigured.
            {
                // Memperbarui `[”ASPNETCORE_ENVIRONMENT”]` menggunakan nilai literal `”Production”` dalam
                // ResolveCookieSecurePolicy_ReturnsAlways_WhenHttpsIsConfigured.
                ["ASPNETCORE_ENVIRONMENT"] = "Production",
                // Memperbarui `[”ASPNETCORE_URLS”]` menggunakan nilai literal `”http://+:5203;https://+:5443”` dalam
                // ResolveCookieSecurePolicy_ReturnsAlways_WhenHttpsIsConfigured.
                ["ASPNETCORE_URLS"] = "http://+:5203;https://+:5443"
            // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
            // ResolveCookieSecurePolicy_ReturnsAlways_WhenHttpsIsConfigured.
            })
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Build(); dalam ResolveCookieSecurePolicy_ReturnsAlways_WhenHttpsIsConfigured;
            // token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Build();

        // Menyiapkan variabel lokal `result` untuk nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya dengan memanggil
        // `HttpsRedirectionPolicy.ResolveCookieSecurePolicy` dengan `configuration`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var result = HttpsRedirectionPolicy.ResolveCookieSecurePolicy(configuration);

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`CookieSecurePolicy.Always`, `result`);
        // pengujian gagal jika keduanya berbeda dalam ResolveCookieSecurePolicy_ReturnsAlways_WhenHttpsIsConfigured.
        Assert.Equal(CookieSecurePolicy.Always, result);
    // Menutup scope metode ResolveCookieSecurePolicy_ReturnsAlways_WhenHttpsIsConfigured; bagian berikut berada di luar batas blok tersebut dalam
    // ResolveCookieSecurePolicy_ReturnsAlways_WhenHttpsIsConfigured.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `RequireHttpsConfiguration_EnablesRedirectionAndSecureCookiesBehindTlsProxy` dengan hasil bertipe `void`; operasi ini
    // menangani require https configuration enables redirection dan secure cookies behind tls proxy.
    public void RequireHttpsConfiguration_EnablesRedirectionAndSecureCookiesBehindTlsProxy()
    // Membuka scope metode RequireHttpsConfiguration_EnablesRedirectionAndSecureCookiesBehindTlsProxy; pernyataan/deklarasi berikut berada di dalam
    // batas blok ini dalam RequireHttpsConfiguration_EnablesRedirectionAndSecureCookiesBehindTlsProxy.
    {
        // Menyiapkan variabel lokal `configuration` untuk konfigurasi aplikasi yang menyediakan nilai pengaturan dari sumber terdaftar dengan memanggil
        // `new ConfigurationBuilder() .AddInMemoryCollection(new Dictionary<string, string?> { [”ASPNETCORE_URLS”] = ”http://+:5203”,
        // [”Security:RequireHttps”] = ”true” }) .Build` dengan tanpa argumen. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var configuration = new ConfigurationBuilder()
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .AddInMemoryCollection(new Dictionary<string, string?> dalam
            // RequireHttpsConfiguration_EnablesRedirectionAndSecureCookiesBehindTlsProxy; token pada baris ini menyambungkan bagian kode sebelum dan
            // sesudahnya.
            .AddInMemoryCollection(new Dictionary<string, string?>
            // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // RequireHttpsConfiguration_EnablesRedirectionAndSecureCookiesBehindTlsProxy.
            {
                // Memperbarui `[”ASPNETCORE_URLS”]` menggunakan nilai literal `”http://+:5203”` dalam
                // RequireHttpsConfiguration_EnablesRedirectionAndSecureCookiesBehindTlsProxy.
                ["ASPNETCORE_URLS"] = "http://+:5203",
                // Memperbarui `[”Security:RequireHttps”]` menggunakan nilai literal `”true”` dalam
                // RequireHttpsConfiguration_EnablesRedirectionAndSecureCookiesBehindTlsProxy.
                ["Security:RequireHttps"] = "true"
            // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
            // RequireHttpsConfiguration_EnablesRedirectionAndSecureCookiesBehindTlsProxy.
            })
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Build(); dalam
            // RequireHttpsConfiguration_EnablesRedirectionAndSecureCookiesBehindTlsProxy; token pada baris ini menyambungkan bagian kode sebelum dan
            // sesudahnya.
            .Build();

        // Menjalankan pemeriksaan bahwa `HttpsRedirectionPolicy.ShouldUseHttpsRedirection(configuration)` bernilai benar; pengujian gagal jika kondisi
        // tidak terpenuhi dalam RequireHttpsConfiguration_EnablesRedirectionAndSecureCookiesBehindTlsProxy.
        Assert.True(HttpsRedirectionPolicy.ShouldUseHttpsRedirection(configuration));
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`CookieSecurePolicy.Always`,
        // `HttpsRedirectionPolicy.ResolveCookieSecurePolicy(configuration)`); pengujian gagal jika keduanya berbeda dalam
        // RequireHttpsConfiguration_EnablesRedirectionAndSecureCookiesBehindTlsProxy.
        Assert.Equal(CookieSecurePolicy.Always, HttpsRedirectionPolicy.ResolveCookieSecurePolicy(configuration));
    // Menutup scope metode RequireHttpsConfiguration_EnablesRedirectionAndSecureCookiesBehindTlsProxy; bagian berikut berada di luar batas blok
    // tersebut dalam RequireHttpsConfiguration_EnablesRedirectionAndSecureCookiesBehindTlsProxy.
    }
// Menutup scope tipe HttpsRedirectionPolicyTests; bagian berikut berada di luar batas blok tersebut.
}
