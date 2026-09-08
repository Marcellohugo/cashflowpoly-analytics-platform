// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui EventDomainValidationResult.
// Mengimpor namespace `Cashflowpoly.Api.Contracts` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Contracts;
// Mengimpor namespace `Microsoft.AspNetCore.Http` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.AspNetCore.Http;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Domain;

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `EventDomainValidationResult`; sealed mencegah tipe ini diturunkan
// lagi.
public sealed record EventDomainValidationResult(
    // Parameter `IsValid` bertipe `bool` membawa penanda apakah validasi telah memenuhi syarat.
    bool IsValid,
    // Parameter `StatusCode` bertipe `int` membawa kode status hasil HTTP yang mengomunikasikan keberhasilan atau kegagalan.
    int StatusCode,
    // Parameter `ErrorCode` bertipe `string?` membawa nilai kesalahan kode; nilai null diizinkan ketika data opsional belum tersedia.
    string? ErrorCode,
    // Parameter `Message` bertipe `string?` membawa nilai pesan; nilai null diizinkan ketika data opsional belum tersedia.
    string? Message,
    // Parameter `Details` bertipe `IReadOnlyList<ErrorDetail>` membawa nilai rincian.
    IReadOnlyList<ErrorDetail> Details)
// Membuka scope tipe EventDomainValidationResult; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendeklarasikan field bertipe `EventDomainValidationResult`: `Valid` menyimpan penanda apakah validasi telah memenuhi syarat dengan nilai awal
    // objek baru dengan tipe mengikuti konteks tujuan dan argumen (true, StatusCodes.Status200OK, null, null, Array.Empty<ErrorDetail>()). readonly
    // membatasi penggantian referensi/nilai field pada deklarasi atau konstruktor. static membuat field menjadi milik tipe dan dibagikan antar
    // instance.
    public static readonly EventDomainValidationResult Valid =
        // Menggunakan objek baru dengan tipe mengikuti konteks tujuan dan argumen (true, StatusCodes.Status200OK, null, null, Array.Empty<ErrorDetail>())
        // sebagai bagian ekspresi yang sedang disusun.
        new(true, StatusCodes.Status200OK, null, null, Array.Empty<ErrorDetail>());

    // Mendefinisikan metode `Fail` dengan hasil bertipe `EventDomainValidationResult`; operasi ini menangani fail. Masukan: Parameter `statusCode`
    // bertipe `int` membawa kode status hasil HTTP yang mengomunikasikan keberhasilan atau kegagalan; Parameter `errorCode` bertipe `string` membawa
    // nilai kesalahan kode; Parameter `message` bertipe `string` membawa nilai pesan; Parameter `details` bertipe `ErrorDetail[]` membawa nilai
    // rincian.
    public static EventDomainValidationResult Fail(
        // Parameter `statusCode` bertipe `int` membawa kode status hasil HTTP yang mengomunikasikan keberhasilan atau kegagalan.
        int statusCode,
        // Parameter `errorCode` bertipe `string` membawa nilai kesalahan kode.
        string errorCode,
        // Parameter `message` bertipe `string` membawa nilai pesan.
        string message,
        // Parameter `details` bertipe `ErrorDetail[]` membawa nilai rincian.
        params ErrorDetail[] details)
    // Membuka scope metode Fail; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Fail.
    {
        // Mengembalikan objek baru bertipe `EventDomainValidationResult` dengan argumen (false, statusCode, errorCode, message, details) kepada pemanggil
        // dalam Fail; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return new EventDomainValidationResult(false, statusCode, errorCode, message, details);
    // Menutup scope metode Fail; bagian berikut berada di luar batas blok tersebut dalam Fail.
    }
// Menutup scope tipe EventDomainValidationResult; bagian berikut berada di luar batas blok tersebut.
}
