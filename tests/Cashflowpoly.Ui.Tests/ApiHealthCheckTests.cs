// Fungsi file: Memastikan readiness UI mengikuti status readiness API.
// Mengimpor namespace `System.Net` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Net;
// Mengimpor namespace `Cashflowpoly.Ui.Infrastructure` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Ui.Infrastructure;
// Mengimpor namespace `Microsoft.Extensions.Diagnostics.HealthChecks` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama
// lengkapnya.
using Microsoft.Extensions.Diagnostics.HealthChecks;
// Mengimpor namespace `Xunit` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Xunit;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Tests` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Tests;

// Mendefinisikan tipe class `ApiHealthCheckTests`; sealed mencegah tipe ini diturunkan lagi.
public sealed class ApiHealthCheckTests
// Membuka scope tipe ApiHealthCheckTests; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // menandai metode sebagai pengujian xUnit yang dijalankan untuk setiap kombinasi data.
    [Theory]
    // menyediakan satu kombinasi masukan pengujian (HttpStatusCode.OK, HealthStatus.Healthy).
    [InlineData(HttpStatusCode.OK, HealthStatus.Healthy)]
    // menyediakan satu kombinasi masukan pengujian (HttpStatusCode.ServiceUnavailable, HealthStatus.Unhealthy).
    [InlineData(HttpStatusCode.ServiceUnavailable, HealthStatus.Unhealthy)]
    // Mendefinisikan metode `CheckHealthAsync_ReflectsApiStatus` dengan hasil bertipe `Task`; operasi ini menangani check health asinkron reflects api
    // status. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `apiStatus`
    // bertipe `HttpStatusCode` membawa nilai api status; Parameter `expected` bertipe `HealthStatus` membawa nilai yang diharapkan.
    public async Task CheckHealthAsync_ReflectsApiStatus(HttpStatusCode apiStatus, HealthStatus expected)
    // Membuka scope metode CheckHealthAsync_ReflectsApiStatus; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // CheckHealthAsync_ReflectsApiStatus.
    {
        // Menyiapkan variabel lokal `check` untuk nilai check dengan objek baru bertipe `ApiHealthCheck` dengan argumen (new StubClientFactory(apiStatus)).
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var check = new ApiHealthCheck(new StubClientFactory(apiStatus));

        // Menyiapkan variabel lokal `result` untuk nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya dengan hasil operasi asinkron memanggil
        // `check.CheckHealthAsync` dengan `new HealthCheckContext()`, `TestContext.Current.CancellationToken`; await menunggu hasil tanpa memblokir thread
        // selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var result = await check.CheckHealthAsync(new HealthCheckContext(), TestContext.Current.CancellationToken);

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`expected`, `result.Status`); pengujian gagal
        // jika keduanya berbeda dalam CheckHealthAsync_ReflectsApiStatus.
        Assert.Equal(expected, result.Status);
    // Menutup scope metode CheckHealthAsync_ReflectsApiStatus; bagian berikut berada di luar batas blok tersebut dalam
    // CheckHealthAsync_ReflectsApiStatus.
    }

    // Mendefinisikan tipe class `StubClientFactory` yang mewarisi atau menerapkan `IHttpClientFactory`; sealed mencegah tipe ini diturunkan lagi.
    private sealed class StubClientFactory(HttpStatusCode statusCode) : IHttpClientFactory
    // Membuka scope tipe StubClientFactory; pernyataan/deklarasi berikut berada di dalam batas blok ini.
    {
        // Mendefinisikan metode `CreateClient` dengan hasil bertipe `HttpClient`; operasi ini menangani create client. Masukan: Parameter `name` bertipe
        // `string` membawa nilai nama. Nilai hasil langsung berasal dari objek baru dengan tipe mengikuti konteks tujuan dan argumen (new
        // StubHandler(statusCode)).
        public HttpClient CreateClient(string name) => new(new StubHandler(statusCode))
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CreateClient.
        {
            // Memperbarui `BaseAddress` menggunakan objek baru bertipe `Uri` dengan argumen (”http://api.test/”) dalam CreateClient.
            BaseAddress = new Uri("http://api.test/")
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam CreateClient.
        };
    // Menutup scope tipe StubClientFactory; bagian berikut berada di luar batas blok tersebut.
    }

    // Mendefinisikan tipe class `StubHandler` yang mewarisi atau menerapkan `HttpMessageHandler`; sealed mencegah tipe ini diturunkan lagi.
    private sealed class StubHandler(HttpStatusCode statusCode) : HttpMessageHandler
    // Membuka scope tipe StubHandler; pernyataan/deklarasi berikut berada di dalam batas blok ini.
    {
        // Mendefinisikan metode `SendAsync` dengan hasil bertipe `Task<HttpResponseMessage>`; operasi ini menangani send asinkron. Masukan: Parameter
        // `request` bertipe `HttpRequestMessage` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan; Parameter
        // `cancellationToken` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan
        // atau aplikasi berhenti. Nilai hasil langsung berasal dari memanggil `Task.FromResult` dengan `new HttpResponseMessage(statusCode)`.
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
            // Melanjutkan pengolahan dengan memanggil `Task.FromResult` dengan `new HttpResponseMessage(statusCode)` dalam SendAsync.
            Task.FromResult(new HttpResponseMessage(statusCode));
    // Menutup scope tipe StubHandler; bagian berikut berada di luar batas blok tersebut.
    }
// Menutup scope tipe ApiHealthCheckTests; bagian berikut berada di luar batas blok tersebut.
}
