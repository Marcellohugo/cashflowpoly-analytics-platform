// Fungsi file: Memastikan API dan databasenya siap sebelum UI dinyatakan siap.
// Mengimpor namespace `Microsoft.Extensions.Diagnostics.HealthChecks` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama
// lengkapnya.
using Microsoft.Extensions.Diagnostics.HealthChecks;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Infrastructure` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Infrastructure;

// Mendefinisikan tipe class `ApiHealthCheck` yang mewarisi atau menerapkan `IHealthCheck`; sealed mencegah tipe ini diturunkan lagi.
public sealed class ApiHealthCheck(IHttpClientFactory clientFactory) : IHealthCheck
// Membuka scope tipe ApiHealthCheck; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan metode `CheckHealthAsync` dengan hasil bertipe `Task<HealthCheckResult>`; operasi ini menangani check health asinkron. async
    // memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `context` bertipe
    // `HealthCheckContext` membawa konteks operasi yang menyediakan data lingkungan pemrosesan saat ini; Parameter `cancellationToken` bertipe
    // `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti; bila
    // argumen tidak diberikan digunakan nilai literal `default`.
    public async Task<HealthCheckResult> CheckHealthAsync(
        // Parameter `context` bertipe `HealthCheckContext` membawa konteks operasi yang menyediakan data lingkungan pemrosesan saat ini.
        HealthCheckContext context,
        // Parameter `cancellationToken` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan
        // permintaan atau aplikasi berhenti; bila argumen tidak diberikan digunakan nilai literal `default`.
        CancellationToken cancellationToken = default)
    // Membuka scope metode CheckHealthAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CheckHealthAsync.
    {
        // Memulai blok try dalam CheckHealthAsync; exception dari blok ini dapat dialihkan ke catch, sedangkan finally (jika ada) tetap dijalankan saat
        // keluar.
        try
        // Membuka scope penanganan operasi try; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CheckHealthAsync.
        {
            // Menyiapkan variabel lokal `response` untuk hasil respons yang akan dibaca atau dikirim kepada pemanggil dengan hasil operasi asinkron memanggil
            // `clientFactory.CreateClient(”ApiHealth”) .GetAsync` dengan `”health/ready”`, `cancellationToken`; await menunggu hasil tanpa memblokir thread
            // selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope
            // berakhir.
            using var response = await clientFactory.CreateClient("ApiHealth")
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .GetAsync(”health/ready”, cancellationToken); dalam CheckHealthAsync; token
                // pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
                .GetAsync("health/ready", cancellationToken);
            // Mengembalikan hasil pemilihan bersyarat: ketika `response.IsSuccessStatusCode` benar gunakan `HealthCheckResult.Healthy()`, jika tidak gunakan
            // `HealthCheckResult.Unhealthy($”API readiness mengembalikan HTTP {(int)response.StatusCode}.”)` kepada pemanggil dalam CheckHealthAsync; eksekusi
            // jalur ini selesai setelah nilai hasil ditentukan.
            return response.IsSuccessStatusCode
                // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: HealthCheckResult.Healthy() dalam CheckHealthAsync.
                ? HealthCheckResult.Healthy()
                // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: HealthCheckResult.Unhealthy($”API readiness mengembalikan HTTP
                // {(int)response.StatusCode}.”); dalam CheckHealthAsync.
                : HealthCheckResult.Unhealthy($"API readiness mengembalikan HTTP {(int)response.StatusCode}.");
        // Menutup scope penanganan operasi try; bagian berikut berada di luar batas blok tersebut dalam CheckHealthAsync.
        }
        // Menangani exception `Exception` melalui variabel exception dalam CheckHealthAsync.
        catch (Exception exception)
        // Membuka scope penanganan exception catch; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CheckHealthAsync.
        {
            // Mengembalikan memanggil `HealthCheckResult.Unhealthy` dengan `”API readiness tidak dapat dijangkau.”`, `exception` kepada pemanggil dalam
            // CheckHealthAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return HealthCheckResult.Unhealthy("API readiness tidak dapat dijangkau.", exception);
        // Menutup scope penanganan exception catch; bagian berikut berada di luar batas blok tersebut dalam CheckHealthAsync.
        }
    // Menutup scope metode CheckHealthAsync; bagian berikut berada di luar batas blok tersebut dalam CheckHealthAsync.
    }
// Menutup scope tipe ApiHealthCheck; bagian berikut berada di luar batas blok tersebut.
}
