// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui EventDomainValidationResult.
// Mengimpor namespace `Cashflowpoly.Api.Contracts` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Contracts;
// Mengimpor namespace `Microsoft.AspNetCore.Http` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.AspNetCore.Http;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Domain;

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
{
    public static readonly EventDomainValidationResult Valid =
        new(true, StatusCodes.Status200OK, null, null, Array.Empty<ErrorDetail>());

    public static EventDomainValidationResult Fail(
        // Parameter `statusCode` bertipe `int` membawa kode status hasil HTTP yang mengomunikasikan keberhasilan atau kegagalan.
        int statusCode,
        // Parameter `errorCode` bertipe `string` membawa nilai kesalahan kode.
        string errorCode,
        // Parameter `message` bertipe `string` membawa nilai pesan.
        string message,
        // Parameter `details` bertipe `ErrorDetail[]` membawa nilai rincian.
        params ErrorDetail[] details)
    {
        return new EventDomainValidationResult(false, statusCode, errorCode, message, details);
    }
}
