// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui EventPlayerBalanceCalculator.
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Domain;

// Mendefinisikan tipe class `EventPlayerBalanceCalculator` yang mewarisi atau menerapkan `IEventPlayerBalanceCalculator`; sealed mencegah tipe ini
// diturunkan lagi.
internal sealed class EventPlayerBalanceCalculator : IEventPlayerBalanceCalculator
// Membuka scope tipe EventPlayerBalanceCalculator; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan metode `Compute` dengan hasil bertipe `double`; operasi ini menangani compute. Masukan: Parameter `playerId` bertipe `Guid`
    // membawa nilai pemain identitas; Parameter `startingCash` bertipe `int` membawa nilai starting uang tunai; Parameter `projections` bertipe
    // `IReadOnlyCollection<CashflowProjectionDb>` membawa proyeksi transaksi arus kas yang diturunkan dari event permainan.
    public double Compute(
        // Parameter `playerId` bertipe `Guid` membawa nilai pemain identitas.
        Guid playerId,
        // Parameter `startingCash` bertipe `int` membawa nilai starting uang tunai.
        int startingCash,
        // Parameter `projections` bertipe `IReadOnlyCollection<CashflowProjectionDb>` membawa proyeksi transaksi arus kas yang diturunkan dari event
        // permainan.
        IReadOnlyCollection<CashflowProjectionDb> projections)
    // Membuka scope metode Compute; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Compute.
    {
        // Menyiapkan variabel lokal `net` untuk nilai net dengan menjumlahkan nilai `projections .Where(p => p.UserId == playerId)` berdasarkan `p =>
        // p.Direction == ”IN” ? p.Amount : -p.Amount`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var net = projections
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(p => p.UserId == playerId) dalam Compute; token pada baris ini
            // menyambungkan bagian kode sebelum dan sesudahnya.
            .Where(p => p.UserId == playerId)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Sum(p => p.Direction == ”IN” ? p.Amount : -p.Amount); dalam Compute; token
            // pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Sum(p => p.Direction == "IN" ? p.Amount : -p.Amount);

        // Mengembalikan penjumlahan/penggabungan antara `startingCash` dan `net` kepada pemanggil dalam Compute; eksekusi jalur ini selesai setelah nilai
        // hasil ditentukan.
        return startingCash + net;
    // Menutup scope metode Compute; bagian berikut berada di luar batas blok tersebut dalam Compute.
    }
// Menutup scope tipe EventPlayerBalanceCalculator; bagian berikut berada di luar batas blok tersebut.
}
