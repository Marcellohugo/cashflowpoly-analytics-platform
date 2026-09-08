// Fungsi file: Memverifikasi perilaku API, database, atau domain melalui ApiErrorHelperTests.
// Mengimpor namespace `Cashflowpoly.Api.Infrastructure` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Infrastructure;
// Mengimpor namespace `Cashflowpoly.Api.Contracts` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Contracts;
// Mengimpor namespace `Microsoft.AspNetCore.Http` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.AspNetCore.Http;
// Mengimpor namespace `Xunit` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Xunit;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Tests` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Tests;

/// <summary>
/// Kelas pengujian unit untuk memvalidasi bahwa ApiErrorHelper membangun
/// objek error response dengan field inti, trace ID, dan detail yang sesuai.
/// </summary>
// Mendefinisikan tipe class `ApiErrorHelperTests`; sealed mencegah tipe ini diturunkan lagi.
public sealed class ApiErrorHelperTests
// Membuka scope tipe ApiErrorHelperTests; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    /// <summary>
    /// Memvalidasi bahwa BuildError memetakan status code, message, dan traceId
    /// dengan benar ke dalam objek ErrorResponse beserta detail error-nya.
    /// </summary>
    // Mendefinisikan metode `BuildError_MapsCoreFields_AndTraceId` dengan hasil bertipe `void`; operasi ini menangani build kesalahan maps core fields
    // dan trace identitas.
    public void BuildError_MapsCoreFields_AndTraceId()
    // Membuka scope metode BuildError_MapsCoreFields_AndTraceId; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // BuildError_MapsCoreFields_AndTraceId.
    {
        // Menyiapkan variabel lokal `context` untuk konteks operasi yang menyediakan data lingkungan pemrosesan saat ini dengan objek baru bertipe
        // `DefaultHttpContext` dengan nilai awal sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var context = new DefaultHttpContext
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // BuildError_MapsCoreFields_AndTraceId.
        {
            // Memperbarui `TraceIdentifier` menggunakan nilai literal `”trace-001”` dalam BuildError_MapsCoreFields_AndTraceId.
            TraceIdentifier = "trace-001"
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // BuildError_MapsCoreFields_AndTraceId.
        };
        // Menyiapkan variabel lokal `detail` untuk nilai detail dengan objek baru bertipe `ErrorDetail` dengan argumen (”username”, ”wajib diisi”). Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var detail = new ErrorDetail("username", "wajib diisi");

        // Menyiapkan variabel lokal `error` untuk informasi kesalahan yang dikembalikan atau dicatat ketika operasi tidak berhasil dengan memanggil
        // `ApiErrorHelper.BuildError` dengan `context`, `”VALIDATION_ERROR”`, `”Input tidak valid”`, `detail`. Tipe variabel disimpulkan dari ekspresi
        // nilai awal.
        var error = ApiErrorHelper.BuildError(context, "VALIDATION_ERROR", "Input tidak valid", detail);

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”VALIDATION_ERROR”`, `error.ErrorCode`);
        // pengujian gagal jika keduanya berbeda dalam BuildError_MapsCoreFields_AndTraceId.
        Assert.Equal("VALIDATION_ERROR", error.ErrorCode);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”Input tidak valid”`, `error.Message`);
        // pengujian gagal jika keduanya berbeda dalam BuildError_MapsCoreFields_AndTraceId.
        Assert.Equal("Input tidak valid", error.Message);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”trace-001”`, `error.TraceId`); pengujian
        // gagal jika keduanya berbeda dalam BuildError_MapsCoreFields_AndTraceId.
        Assert.Equal("trace-001", error.TraceId);
        // Menjalankan pemeriksaan hasil dengan `Assert.Single` menggunakan `error.Details`; ketidaksesuaian dengan ekspektasi membuat pengujian gagal dalam
        // BuildError_MapsCoreFields_AndTraceId.
        Assert.Single(error.Details);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”username”`, `error.Details[0].Field`);
        // pengujian gagal jika keduanya berbeda dalam BuildError_MapsCoreFields_AndTraceId.
        Assert.Equal("username", error.Details[0].Field);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”wajib diisi”`, `error.Details[0].Issue`);
        // pengujian gagal jika keduanya berbeda dalam BuildError_MapsCoreFields_AndTraceId.
        Assert.Equal("wajib diisi", error.Details[0].Issue);
    // Menutup scope metode BuildError_MapsCoreFields_AndTraceId; bagian berikut berada di luar batas blok tersebut dalam
    // BuildError_MapsCoreFields_AndTraceId.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    /// <summary>
    /// Memvalidasi bahwa BuildError mengembalikan daftar detail kosong
    /// ketika tidak ada ErrorDetail yang diberikan pada parameter.
    /// </summary>
    // Mendefinisikan metode `BuildError_ReturnsEmptyDetails_WhenNoDetailProvided` dengan hasil bertipe `void`; operasi ini menangani build kesalahan
    // returns empty rincian when no detail provided.
    public void BuildError_ReturnsEmptyDetails_WhenNoDetailProvided()
    // Membuka scope metode BuildError_ReturnsEmptyDetails_WhenNoDetailProvided; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // BuildError_ReturnsEmptyDetails_WhenNoDetailProvided.
    {
        // Menyiapkan variabel lokal `context` untuk konteks operasi yang menyediakan data lingkungan pemrosesan saat ini dengan objek baru bertipe
        // `DefaultHttpContext` dengan nilai awal sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var context = new DefaultHttpContext
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // BuildError_ReturnsEmptyDetails_WhenNoDetailProvided.
        {
            // Memperbarui `TraceIdentifier` menggunakan nilai literal `”trace-002”` dalam BuildError_ReturnsEmptyDetails_WhenNoDetailProvided.
            TraceIdentifier = "trace-002"
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // BuildError_ReturnsEmptyDetails_WhenNoDetailProvided.
        };

        // Menyiapkan variabel lokal `error` untuk informasi kesalahan yang dikembalikan atau dicatat ketika operasi tidak berhasil dengan memanggil
        // `ApiErrorHelper.BuildError` dengan `context`, `”NOT_FOUND”`, `”Data tidak ditemukan”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var error = ApiErrorHelper.BuildError(context, "NOT_FOUND", "Data tidak ditemukan");

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”NOT_FOUND”`, `error.ErrorCode`); pengujian
        // gagal jika keduanya berbeda dalam BuildError_ReturnsEmptyDetails_WhenNoDetailProvided.
        Assert.Equal("NOT_FOUND", error.ErrorCode);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”trace-002”`, `error.TraceId`); pengujian
        // gagal jika keduanya berbeda dalam BuildError_ReturnsEmptyDetails_WhenNoDetailProvided.
        Assert.Equal("trace-002", error.TraceId);
        // Menjalankan pemeriksaan hasil dengan `Assert.Empty` menggunakan `error.Details`; ketidaksesuaian dengan ekspektasi membuat pengujian gagal dalam
        // BuildError_ReturnsEmptyDetails_WhenNoDetailProvided.
        Assert.Empty(error.Details);
    // Menutup scope metode BuildError_ReturnsEmptyDetails_WhenNoDetailProvided; bagian berikut berada di luar batas blok tersebut dalam
    // BuildError_ReturnsEmptyDetails_WhenNoDetailProvided.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `BuildError_TranslatesCoveredMessage_WhenEnglishIsPreferred` dengan hasil bertipe `void`; operasi ini menangani build
    // kesalahan translates covered pesan when english berstatus preferred.
    public void BuildError_TranslatesCoveredMessage_WhenEnglishIsPreferred()
    // Membuka scope metode BuildError_TranslatesCoveredMessage_WhenEnglishIsPreferred; pernyataan/deklarasi berikut berada di dalam batas blok ini
    // dalam BuildError_TranslatesCoveredMessage_WhenEnglishIsPreferred.
    {
        // Menyiapkan variabel lokal `context` untuk konteks operasi yang menyediakan data lingkungan pemrosesan saat ini dengan objek baru bertipe
        // `DefaultHttpContext` dengan nilai awal sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var context = new DefaultHttpContext();
        // Memperbarui `context.Request.Headers.AcceptLanguage` menggunakan nilai literal `”en-US,en;q=0.9”` dalam
        // BuildError_TranslatesCoveredMessage_WhenEnglishIsPreferred.
        context.Request.Headers.AcceptLanguage = "en-US,en;q=0.9";

        // Menyiapkan variabel lokal `error` untuk informasi kesalahan yang dikembalikan atau dicatat ketika operasi tidak berhasil dengan memanggil
        // `ApiErrorHelper.BuildError` dengan `context`, `”NOT_FOUND”`, `”Ruleset tidak memiliki definisi relasional yang lengkap”`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var error = ApiErrorHelper.BuildError(
            // Meneruskan `context` (konteks operasi yang menyediakan data lingkungan pemrosesan saat ini) sebagai argumen ke `ApiErrorHelper.BuildError`.
            context,
            // Meneruskan nilai literal `”NOT_FOUND”` sebagai argumen ke `ApiErrorHelper.BuildError`.
            "NOT_FOUND",
            // Meneruskan nilai literal `”Ruleset tidak memiliki definisi relasional yang lengkap”` sebagai argumen ke `ApiErrorHelper.BuildError`.
            "Ruleset tidak memiliki definisi relasional yang lengkap");

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”Ruleset does not have a complete relational
        // definition”`, `error.Message`); pengujian gagal jika keduanya berbeda dalam BuildError_TranslatesCoveredMessage_WhenEnglishIsPreferred.
        Assert.Equal("Ruleset does not have a complete relational definition", error.Message);
    // Menutup scope metode BuildError_TranslatesCoveredMessage_WhenEnglishIsPreferred; bagian berikut berada di luar batas blok tersebut dalam
    // BuildError_TranslatesCoveredMessage_WhenEnglishIsPreferred.
    }
// Menutup scope tipe ApiErrorHelperTests; bagian berikut berada di luar batas blok tersebut.
}
