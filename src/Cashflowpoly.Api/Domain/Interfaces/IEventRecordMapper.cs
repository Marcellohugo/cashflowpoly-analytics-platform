// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui IEventRecordMapper.
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;
// Mengimpor namespace `Cashflowpoly.Api.Contracts` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Contracts;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Domain;

// Mendefinisikan interface sebagai kontrak operasi `IEventRecordMapper`.
public interface IEventRecordMapper
// Membuka scope tipe IEventRecordMapper; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan metode `ToEventRequest` dengan hasil bertipe `EventRequest`; operasi ini menangani ke event permintaan. Masukan: Parameter
    // `record` bertipe `EventDb` membawa nilai rekaman.
    EventRequest ToEventRequest(EventDb record);
// Menutup scope tipe IEventRecordMapper; bagian berikut berada di luar batas blok tersebut.
}
