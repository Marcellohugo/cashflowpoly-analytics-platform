// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui IEventProjectionBuilders.
// Mengimpor namespace `System.Diagnostics.CodeAnalysis` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Diagnostics.CodeAnalysis;
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;
// Mengimpor namespace `Cashflowpoly.Api.Contracts` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Contracts;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Domain;

public interface IEventCashflowProjectionBuilder
{
    bool TryBuild(
        // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        EventRequest request,
        // Parameter `timestamp` bertipe `DateTimeOffset` membawa waktu kejadian yang menjaga urutan kronologis data.
        DateTimeOffset timestamp,
        // Parameter `eventPk` bertipe `Guid` membawa nilai event pk.
        Guid eventPk,
        // Parameter `projection` bertipe `CashflowProjectionDb?` membawa nilai projection; nilai null diizinkan ketika data opsional belum tersedia; out
        // mengembalikan nilai melalui parameter dan harus diisi oleh metode; menerapkan metadata `NotNullWhen(true)` pada deklarasi berikut agar
        // framework/compiler dapat mengenali pengaturannya.
        [NotNullWhen(true)] out CashflowProjectionDb? projection);
}
