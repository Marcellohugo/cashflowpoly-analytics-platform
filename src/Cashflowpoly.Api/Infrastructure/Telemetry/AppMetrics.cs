// Fungsi file: Menyediakan dukungan infrastruktur API melalui AppMetrics.
// Mengimpor namespace `System.Diagnostics.Metrics` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Diagnostics.Metrics;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Infrastructure.Telemetry` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Infrastructure.Telemetry;

// Mendefinisikan tipe class `AppMetrics`.
public static class AppMetrics
// Membuka scope tipe AppMetrics; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendeklarasikan field bertipe `string`: `MeterName` menyimpan nilai meter nama dengan nilai awal nilai literal `”Cashflowpoly.Api”`.
    public const string MeterName = "Cashflowpoly.Api";

    // Mendeklarasikan field bertipe `Meter`: `Meter` menyimpan nilai meter dengan nilai awal objek baru dengan tipe mengikuti konteks tujuan dan
    // argumen (MeterName, ”1.0”). readonly membatasi penggantian referensi/nilai field pada deklarasi atau konstruktor. static membuat field menjadi
    // milik tipe dan dibagikan antar instance.
    private static readonly Meter Meter = new(MeterName, "1.0");

    // Mendeklarasikan field bertipe `Counter<long>`: `RequestsTotal` menyimpan nilai requests total dengan nilai awal memanggil
    // `Meter.CreateCounter<long>` dengan `”http_requests_total”`, `”requests”`, `”Total HTTP requests processed”`. readonly membatasi penggantian
    // referensi/nilai field pada deklarasi atau konstruktor. static membuat field menjadi milik tipe dan dibagikan antar instance.
    public static readonly Counter<long> RequestsTotal =
        // Melanjutkan pengolahan dengan memanggil `Meter.CreateCounter<long>` dengan `”http_requests_total”`, `”requests”`, `”Total HTTP requests
        // processed”`.
        Meter.CreateCounter<long>("http_requests_total", "requests", "Total HTTP requests processed");

    // Mendeklarasikan field bertipe `Histogram<double>`: `RequestDurationMs` menyimpan nilai permintaan duration ms dengan nilai awal memanggil
    // `Meter.CreateHistogram<double>` dengan `”http_request_duration_ms”`, `”ms”`, `”HTTP request duration in milliseconds”`. readonly membatasi
    // penggantian referensi/nilai field pada deklarasi atau konstruktor. static membuat field menjadi milik tipe dan dibagikan antar instance.
    public static readonly Histogram<double> RequestDurationMs =
        // Melanjutkan pengolahan dengan memanggil `Meter.CreateHistogram<double>` dengan `”http_request_duration_ms”`, `”ms”`, `”HTTP request duration in
        // milliseconds”`.
        Meter.CreateHistogram<double>("http_request_duration_ms", "ms", "HTTP request duration in milliseconds");

    // Mendeklarasikan field bertipe `Counter<long>`: `RequestErrorsTotal` menyimpan nilai permintaan kesalahan total dengan nilai awal memanggil
    // `Meter.CreateCounter<long>` dengan `”http_request_errors_total”`, `”requests”`, `”Total HTTP requests that returned 4xx or 5xx”`. readonly
    // membatasi penggantian referensi/nilai field pada deklarasi atau konstruktor. static membuat field menjadi milik tipe dan dibagikan antar
    // instance.
    public static readonly Counter<long> RequestErrorsTotal =
        // Melanjutkan pengolahan dengan memanggil `Meter.CreateCounter<long>` dengan `”http_request_errors_total”`, `”requests”`, `”Total HTTP requests
        // that returned 4xx or 5xx”`.
        Meter.CreateCounter<long>("http_request_errors_total", "requests", "Total HTTP requests that returned 4xx or 5xx");
// Menutup scope tipe AppMetrics; bagian berikut berada di luar batas blok tersebut.
}
