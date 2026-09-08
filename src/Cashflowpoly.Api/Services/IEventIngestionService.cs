// Fungsi file: Mengorkestrasi alur aplikasi dan domain melalui IEventIngestionService.
// Mengimpor namespace `System.Security.Claims` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Security.Claims;
// Mengimpor namespace `Cashflowpoly.Api.Contracts` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Contracts;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Services` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Services;

// Mendefinisikan interface sebagai kontrak operasi `IEventIngestionService`.
public interface IEventIngestionService
// Membuka scope tipe IEventIngestionService; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan metode `IngestEventAsync` dengan hasil bertipe `Task<(EventStoredResponse? Result, int StatusCode, ErrorResponse? Error)>`;
    // operasi ini menangani ingest event asinkron. Masukan: Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan
    // divalidasi atau diteruskan ke layanan; Parameter `user` bertipe `ClaimsPrincipal` membawa pengguna yang sedang diproses beserta identitas atau
    // klaim akses yang dimilikinya; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil
    // membatalkan permintaan atau aplikasi berhenti.
    Task<(EventStoredResponse? Result, int StatusCode, ErrorResponse? Error)> IngestEventAsync(
        // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        EventRequest request, ClaimsPrincipal user, CancellationToken ct);

    // Mendefinisikan metode `IngestBatchAsync` dengan hasil bertipe `Task<(EventBatchResponse? Result, int StatusCode, ErrorResponse? Error)>`; operasi
    // ini menangani ingest batch asinkron. Masukan: Parameter `request` bertipe `EventBatchRequest` membawa data masukan permintaan yang akan
    // divalidasi atau diteruskan ke layanan; Parameter `user` bertipe `ClaimsPrincipal` membawa pengguna yang sedang diproses beserta identitas atau
    // klaim akses yang dimilikinya; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil
    // membatalkan permintaan atau aplikasi berhenti.
    Task<(EventBatchResponse? Result, int StatusCode, ErrorResponse? Error)> IngestBatchAsync(
        // Parameter `request` bertipe `EventBatchRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        EventBatchRequest request, ClaimsPrincipal user, CancellationToken ct);

    // Mendefinisikan metode `GetEventsBySessionAsync` dengan hasil bertipe `Task<(EventsBySessionResponse? Result, int StatusCode, ErrorResponse?
    // Error)>`; operasi ini menangani get event berdasarkan sesi asinkron. Masukan: Parameter `sessionId` bertipe `Guid` membawa identitas unik sesi
    // permainan yang menjadi batas data operasi ini; Parameter `user` bertipe `ClaimsPrincipal` membawa pengguna yang sedang diproses beserta identitas
    // atau klaim akses yang dimilikinya; Parameter `cursor` bertipe `string?` membawa penanda halaman untuk melanjutkan pembacaan setelah elemen
    // sebelumnya; nilai null diizinkan ketika data opsional belum tersedia; Parameter `limit` bertipe `int` membawa batas jumlah hasil yang diminta
    // pada satu operasi; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil
    // membatalkan permintaan atau aplikasi berhenti.
    Task<(EventsBySessionResponse? Result, int StatusCode, ErrorResponse? Error)> GetEventsBySessionAsync(
        // Parameter `sessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini.
        Guid sessionId, ClaimsPrincipal user, string? cursor, int limit, CancellationToken ct);
// Menutup scope tipe IEventIngestionService; bagian berikut berada di luar batas blok tersebut.
}
